#region CargoShip

/*
 * CargoShip Script v2.6.0
 * Protocol: v2 (compatible with OrderStation v2.x, SupplyStation v2.x)
 *
 * Version History:
 * 2.6.0 - Cockpit/multi-screen LCD support; GetDisplaySurfaces() scans both
 *         IMyTextPanel blocks and IMyTextSurfaceProvider blocks (cockpits, control seats);
 *         configure cockpit surface index with [TAG]:N in block CustomData (default :0)
 * 2.5.0 - Reads StorageTag from station PB CustomData; supports custom storage
 *         container tags (e.g., SupplyStation StorageTag config); falls back to
 *         [STORAGE]/[STOCK] if not configured; aborts mission if no items loaded
 *         (damaged conveyors, full cargo) instead of flying empty
 * 2.4.0 - Added startup validation system; default config template on first run;
 *         VALIDATE argument for on-demand checks; halts on critical errors
 * 2.3.0 - Added Category.ALL export support (e.g., Ingot.ALL, Component.ALL exports all items in category)
 * 2.2.1 - Fixed LCD status showing wrong message during remote drone recovery transit
 * 2.2.0 - Remote drone recovery: broadcasts AVAILABLE from any station, flies home first when assigned while remote
 * 2.1.0 - Removed [CARGOMASTER] tag requirement; exports now discovered via StationID in PB CustomData
 * 2.0.0 - Loads from [STORAGE]/[STOCK] per manifest; reports cargo status in DEPARTED
 * 1.2.0 - Added drone capability tags (DroneTags) for destination restrictions
 * 1.1.0 - Added departure notifications and mission state tracking
 * 1.0.0 - Initial release with automated cargo transfer
 */

// ============== STARTUP VALIDATOR ==============
public enum ValidationSeverity { CRITICAL, WARNING, INFO }

public struct ValidationResult {
    public ValidationSeverity Severity;
    public string Category;
    public string Message;

    public ValidationResult(ValidationSeverity severity, string category, string message) {
        Severity = severity;
        Category = category;
        Message = message;
    }
}

public class StartupValidator {
    List<ValidationResult> results = new List<ValidationResult>();

    public void Critical(string category, string message) {
        results.Add(new ValidationResult(ValidationSeverity.CRITICAL, category, message));
    }

    public void Warning(string category, string message) {
        results.Add(new ValidationResult(ValidationSeverity.WARNING, category, message));
    }

    public void Info(string category, string message) {
        results.Add(new ValidationResult(ValidationSeverity.INFO, category, message));
    }

    public bool HasCritical() {
        foreach (var r in results) {
            if (r.Severity == ValidationSeverity.CRITICAL) return true;
        }
        return false;
    }

    public int CriticalCount() {
        int count = 0;
        foreach (var r in results) {
            if (r.Severity == ValidationSeverity.CRITICAL) count++;
        }
        return count;
    }

    public int WarningCount() {
        int count = 0;
        foreach (var r in results) {
            if (r.Severity == ValidationSeverity.WARNING) count++;
        }
        return count;
    }

    public void Clear() { results.Clear(); }

    public string FormatForLcd() {
        var sb = new StringBuilder();
        sb.AppendLine("=== VALIDATION ===");

        int critCount = CriticalCount();
        int warnCount = WarningCount();

        if (critCount == 0 && warnCount == 0) {
            sb.AppendLine("[OK] All checks passed");
            return sb.ToString();
        }

        sb.AppendLine($"[!] {critCount} critical, {warnCount} warnings");
        sb.AppendLine();

        if (critCount > 0) {
            sb.AppendLine("--- CRITICAL ---");
            foreach (var r in results) {
                if (r.Severity == ValidationSeverity.CRITICAL) {
                    sb.AppendLine($"[X] {r.Category}: {r.Message}");
                }
            }
        }

        if (warnCount > 0) {
            sb.AppendLine("--- WARNINGS ---");
            foreach (var r in results) {
                if (r.Severity == ValidationSeverity.WARNING) {
                    sb.AppendLine($"[!] {r.Category}: {r.Message}");
                }
            }
        }

        return sb.ToString();
    }

    public string FormatForEcho() {
        var sb = new StringBuilder();
        sb.AppendLine("=== STARTUP VALIDATION ===");

        if (results.Count == 0) {
            sb.AppendLine("[OK] All checks passed");
            return sb.ToString();
        }

        foreach (var r in results) {
            string prefix = r.Severity == ValidationSeverity.CRITICAL ? "CRITICAL"
                          : r.Severity == ValidationSeverity.WARNING ? "WARNING"
                          : "INFO";
            sb.AppendLine($"[{prefix}] {r.Category}: {r.Message}");
        }

        return sb.ToString();
    }
}
// ============== END VALIDATOR ==============

// Validation
StartupValidator validator = new StartupValidator();
bool validationFailed = false;

// Configuration
string cargoContainerTag = "[CARGO]";
string statusLcdTag = "[STATUS]";
string homeStation = ""; // From CustomData: HomeStation=X
string droneId = ""; // From CustomData: DroneID=X
string autopilotPBName = ""; // From CustomData: AutopilotPBName=X
List<string> droneTags = new List<string>(); // From CustomData: DroneTags=TAG1,TAG2,TAG3
Dictionary<string, string> connectorNames = new Dictionary<string, string>(); // StationID -> Connector name mapping

// State
bool isDocked = false;
string currentStationId = "";
string lastProcessedStation = "";
bool cargoOperationsComplete = false;
int currentPhase = 0; // 0=Unload, 1=LoadExports, 2=LoadOrders, 3=Complete
int itemIndex = 0; // Track which item we're processing
Dictionary<string, MyFixedPoint> exportLoadedAmounts = new Dictionary<string, MyFixedPoint>(); // Track loaded amounts per export item
StringBuilder statusLog = new StringBuilder();

// Mission state
string assignedDestination = ""; // Where to deliver cargo
bool missionActive = false; // Currently on a delivery mission?
bool wasDocked = false; // Track previous dock state to detect undock events
Dictionary<string, MyFixedPoint> missionManifest = new Dictionary<string, MyFixedPoint>();
List<string> manifestKeys = new List<string>(); // Stable key list for one-item-per-tick indexing
Dictionary<string, MyFixedPoint> lastLoadedAmounts = new Dictionary<string, MyFixedPoint>();
bool lastCargoFull = false;

// IGC listeners
IMyBroadcastListener assignmentListener;

// Autopilot integration
IMyProgrammableBlock autopilotPB = null;

public Program() {
    // First-time setup: write default config template
    if (string.IsNullOrWhiteSpace(Me.CustomData)) {
        Me.CustomData = GetDefaultConfig();
        Echo("=== FIRST-TIME SETUP ===");
        Echo("Default config written to CustomData.");
        Echo("Edit CustomData and recompile.");
        Runtime.UpdateFrequency = UpdateFrequency.None;
        return;
    }

    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    LoadConfig();

    // Find autopilot PB (before validation so we can check if it's enabled)
    if (!string.IsNullOrEmpty(autopilotPBName)) {
        var autopilots = new List<IMyProgrammableBlock>();
        GridTerminalSystem.GetBlocksOfType(autopilots, pb =>
            pb.IsSameConstructAs(Me) &&
            pb.CustomName.Contains(autopilotPBName)
        );
        if (autopilots.Count > 0) {
            autopilotPB = autopilots[0];
        }
    }

    // Run startup validation
    RunValidation();

    // Register for cargo assignments
    assignmentListener = IGC.RegisterBroadcastListener("CARGO_ASSIGNED");
    assignmentListener.SetMessageCallback("CARGO_ASSIGNED");
}

string GetDefaultConfig() {
    return @"HomeStation=CHANGE_ME
DroneID=
AutopilotPBName=
CargoContainerTag=[CARGO]
StatusLcdTag=[STATUS]
DroneTags=

---CONNECTORS---
; Map station IDs to connector names
; Example: SupplyStation=Connector Front
";
}

void RunValidation() {
    validator.Clear();
    ValidateConfig();
    ValidateBlocks();
    validationFailed = validator.HasCritical();
    Echo(validator.FormatForEcho());
}

void ValidateConfig() {
    // CRITICAL: HomeStation required
    if (string.IsNullOrEmpty(homeStation) || homeStation == "CHANGE_ME") {
        validator.Critical("Config", "HomeStation not configured in CustomData");
    }

    // INFO: DroneID using grid name fallback
    if (droneId == Me.CubeGrid.CustomName) {
        validator.Info("Config", $"DroneID using grid name: {droneId}");
    }

    // INFO: Empty connectors section
    if (connectorNames.Count == 0) {
        validator.Info("Config", "No connector mappings configured");
    }
}

void ValidateBlocks() {
    // CRITICAL: Antenna required for IGC
    var antennas = new List<IMyRadioAntenna>();
    GridTerminalSystem.GetBlocksOfType(antennas, a => a.IsSameConstructAs(Me));
    if (antennas.Count == 0) {
        validator.Critical("Blocks", "No antenna found - IGC requires antenna");
    }

    // CRITICAL: Connector required
    var connectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(connectors, c => c.IsSameConstructAs(Me));
    if (connectors.Count == 0) {
        validator.Critical("Blocks", "No connector found on ship");
    }

    // WARNING: Cargo containers
    var cargoContainers = GetShipContainers(cargoContainerTag);
    if (cargoContainers.Count == 0) {
        validator.Warning("Blocks", $"No cargo containers with {cargoContainerTag} tag");
    }

    // WARNING: Autopilot PB configured but not found
    if (!string.IsNullOrEmpty(autopilotPBName)) {
        if (autopilotPB == null) {
            validator.Warning("Blocks", $"Autopilot PB '{autopilotPBName}' not found");
        } else if (!autopilotPB.Enabled) {
            validator.Warning("Blocks", $"Autopilot PB '{autopilotPBName}' is turned OFF");
        }
    }

    // INFO: Status displays (panels or cockpit surfaces)
    var statusSurfaces = GetDisplaySurfaces(statusLcdTag, null);
    if (statusSurfaces.Count == 0) {
        validator.Info("Blocks", $"No display with {statusLcdTag} tag (optional)");
    }
}

public void Save() {}

public void Main(string argument, UpdateType updateSource) {
    statusLog.Clear();

    // Handle VALIDATE argument (always allowed, even when halted)
    if (!string.IsNullOrEmpty(argument) && argument.ToUpper() == "VALIDATE") {
        LoadConfig();
        // Re-find autopilot PB for validation
        if (!string.IsNullOrEmpty(autopilotPBName)) {
            var autopilots = new List<IMyProgrammableBlock>();
            GridTerminalSystem.GetBlocksOfType(autopilots, pb =>
                pb.IsSameConstructAs(Me) &&
                pb.CustomName.Contains(autopilotPBName)
            );
            autopilotPB = autopilots.Count > 0 ? autopilots[0] : null;
        }
        RunValidation();
        statusLog.AppendLine(validator.FormatForLcd());
        UpdateDisplays();
        return;
    }

    // HALT if critical validation errors exist
    if (validationFailed) {
        statusLog.AppendLine("=== HALTED ===");
        statusLog.AppendLine("Fix critical errors and recompile.");
        statusLog.AppendLine();
        statusLog.AppendLine(validator.FormatForLcd());
        UpdateDisplays();
        Echo("HALTED: Fix critical errors and recompile");
        Echo(validator.FormatForEcho());
        return;
    }

    try {
        // Check if docked
        var connector = GetShipConnector();

        if (connector == null) {
            LogStatus("No connector found on ship!");
            UpdateDisplays();
            return;
        }

        isDocked = connector.Status == MyShipConnectorStatus.Connected;

        // Detect undock event - send departure notification if on a mission
        if (wasDocked && !isDocked && missionActive && !string.IsNullOrEmpty(assignedDestination)) {
            // Build departure message with cargo status and loaded amounts
            var cargoStatus = lastCargoFull ? "FULL" : "OK";
            var manifestParts = new List<string>();
            foreach (var item in lastLoadedAmounts) {
                manifestParts.Add($"{item.Key}:{(int)item.Value}");
            }
            var manifestString = string.Join(",", manifestParts);
            var departureMessage = $"DEPARTED|{droneId}|{assignedDestination}|{cargoStatus}|{manifestString}";
            IGC.SendBroadcastMessage("DRONE_DEPARTED", departureMessage);
        }

        wasDocked = isDocked;

        if (!isDocked) {
            // Show mission context while in transit
            if (missionActive && !string.IsNullOrEmpty(assignedDestination)) {
                LogStatus("IN TRANSIT");
                LogStatus($"Mission: Deliver to {assignedDestination}");
                if (assignedDestination == homeStation) {
                    // Returning home after delivery
                    LogStatus("Status: Returning home");
                } else if (currentStationId == homeStation) {
                    // Just left home, flying to destination to deliver
                    LogStatus("Status: Flying to destination");
                } else {
                    // At remote station, flying home first to load cargo
                    LogStatus("Status: Returning home to load cargo");
                }
            } else {
                LogStatus("Ship not docked. Waiting...");
            }
            // Reset state when undocked
            lastProcessedStation = "";
            cargoOperationsComplete = false;
            currentPhase = 0;
            itemIndex = 0;
            UpdateDisplays();
            return;
        }

        // Get station ID
        currentStationId = GetStationId(connector);

        if (string.IsNullOrEmpty(currentStationId)) {
            LogStatus("Docked but cannot identify station!");
            UpdateDisplays();
            return;
        }

        LogStatus($"Docked at: {currentStationId}");
        LogStatus("===================");

        // Check if we're docked at a different station - reset state
        if (lastProcessedStation != currentStationId && !string.IsNullOrEmpty(lastProcessedStation)) {
            currentPhase = 0;
            itemIndex = 0;
            cargoOperationsComplete = false;
            exportLoadedAmounts.Clear();
        }

        // If this is a new station we haven't processed, set it as current
        if (string.IsNullOrEmpty(lastProcessedStation) || lastProcessedStation != currentStationId) {
            if (currentPhase == 0 && itemIndex == 0 && !cargoOperationsComplete) {
                lastProcessedStation = currentStationId;
            }
        }

        LogStatus($"State: Phase={currentPhase}, Item={itemIndex}, Complete={cargoOperationsComplete}");
        if (missionActive) {
            LogStatus($"Mission: Active - Destination={assignedDestination}");
        }

        // Check if we're idle - broadcast availability and check for missions (from any station)
        if (!missionActive && cargoOperationsComplete) {
            BroadcastAvailability();
            ProcessAssignments();

            if (currentStationId == homeStation) {
                LogStatus("\n===================");
                LogStatus("IDLE - Waiting for mission assignment");
                LogStatus($"Home: {homeStation}");
                LogStatus($"Drone ID: {droneId}");
            } else {
                LogStatus("\n===================");
                LogStatus($"IDLE at remote station: {currentStationId}");
                LogStatus($"Waiting for mission (will fly home first)");
            }
            UpdateDisplays();
            return;
        }

        // Check if we already processed cargo for this station
        if (cargoOperationsComplete && lastProcessedStation == currentStationId) {
            LogStatus("\n===================");
            LogStatus("Cargo operations complete!");
            LogStatus("Waiting for undock...");
            UpdateDisplays();
            return;
        }

        // Multi-tick state machine - process one phase per tick
        bool phaseComplete = false;

        if (currentPhase == 0) {
            LogStatus("\nPhase 0: Unloading cargo...");
            phaseComplete = UnloadCargoToStationMultiTick(connector);
            if (phaseComplete) {
                LogStatus("  Phase 0 complete!");
                currentPhase = 1;
                itemIndex = 0;
            }
        } else if (currentPhase == 1) {
            LogStatus("\nPhase 1: Loading exports from station...");
            phaseComplete = LoadExportsFromStationMultiTick(connector);
            if (phaseComplete) {
                LogStatus("  Phase 1 complete!");
                // Don't clear exportLoadedAmounts yet - save for final summary in Phase 3
                currentPhase = 2;
                itemIndex = 0;
            }
        } else if (currentPhase == 2) {
            LogStatus("\nPhase 2: Loading orders from station...");
            phaseComplete = LoadOrdersFromStationMultiTick(connector);
            if (phaseComplete) {
                LogStatus("  Phase 2 complete!");

                // Check if we're on a mission and just loaded cargo at home station
                if (missionActive && currentStationId == homeStation && !string.IsNullOrEmpty(assignedDestination) && assignedDestination != homeStation) {
                    // Abort if nothing was loaded and we have a manifest (not a pickup)
                    if (lastLoadedAmounts.Count == 0 && missionManifest.Count > 0) {
                        LogStatus($"\nABORTED: No items loaded - check conveyor connections");
                        LogStatus("Mission cancelled. Returning to idle.");
                        missionActive = false;
                        assignedDestination = "";
                        missionManifest.Clear();
                        manifestKeys.Clear();
                        lastCargoFull = false;
                    } else {
                        LogStatus($"\nLaunching to {assignedDestination}...");
                        TriggerAutopilot(assignedDestination);
                    }
                }

                currentPhase = 3;
            }
        }

        // All phases complete
        if (currentPhase == 3) {
            // Check if we completed delivery mission at destination
            if (missionActive && currentStationId == assignedDestination && assignedDestination != homeStation) {
                LogStatus($"\nDelivery complete! Returning to {homeStation}...");
                TriggerAutopilot(homeStation);
                assignedDestination = homeStation; // Now returning home
            }
            // Check if we returned home from mission
            else if (missionActive && currentStationId == homeStation && assignedDestination == homeStation) {
                LogStatus("\nMission complete! Ready for next assignment.");

                // Notify home station that mission is complete
                var completionMessage = $"COMPLETE|{droneId}";
                IGC.SendBroadcastMessage("MISSION_COMPLETE", completionMessage);

                missionActive = false;
                assignedDestination = "";
                missionManifest.Clear();
                manifestKeys.Clear();
                lastLoadedAmounts.Clear();
                lastCargoFull = false;
            }

            // Print final summary of what was loaded
            LogStatus("\n===================");
            LogStatus("CARGO OPERATIONS SUMMARY");
            LogStatus("===================");



            if (exportLoadedAmounts.Count > 0) {
                LogStatus("\nExports Loaded:");
                foreach (var export in exportLoadedAmounts) {
                    LogStatus($"  {export.Key}: {(int)export.Value} units");
                }
            } else {
                LogStatus("\nNo exports loaded");
            }

            LogStatus("\n===================");
            LogStatus("All operations complete!");

            // Clear tracking now that we've printed the summary
            exportLoadedAmounts.Clear();

            // Mark operations as complete for this station
            lastProcessedStation = currentStationId;
            cargoOperationsComplete = true;
        }

    } catch (Exception ex) {
        LogStatus($"ERROR: {ex.Message}");
    }

    UpdateDisplays();
}

void LoadConfig() {
    var config = Me.CustomData;
    var lines = config.Split('\n');
    bool inConnectors = false;

    foreach (var line in lines) {
        var trimmed = line.Trim();

        // Check for connector mapping section
        if (trimmed == "---CONNECTORS---") {
            inConnectors = true;
            continue;
        }

        if (trimmed.StartsWith("---") && trimmed != "---CONNECTORS---") {
            inConnectors = false;
        }

        if (trimmed.Contains("=")) {
            var parts = trimmed.Split('=');
            var key = parts[0].Trim();
            var value = parts[1].Trim();

            if (!inConnectors) {
                // Main config settings
                if (key == "CargoContainerTag") cargoContainerTag = value;
                else if (key == "StatusLcdTag") statusLcdTag = value;
                else if (key == "HomeStation") homeStation = value;
                else if (key == "DroneID") droneId = value;
                else if (key == "AutopilotPBName") autopilotPBName = value;
                else if (key == "DroneTags") {
                    // Parse comma-separated tags
                    droneTags.Clear();
                    var tags = value.Split(',');
                    foreach (var tag in tags) {
                        var trimmedTag = tag.Trim().ToUpper(); // Normalize to uppercase
                        if (!string.IsNullOrEmpty(trimmedTag)) {
                            droneTags.Add(trimmedTag);
                        }
                    }
                }
            } else {
                // Connector mappings: StationID=ConnectorName
                connectorNames[key] = value;
            }
        }
    }

    // Generate drone ID from grid name if not configured
    if (string.IsNullOrEmpty(droneId)) {
        droneId = Me.CubeGrid.CustomName;
    }
}

IMyShipConnector GetShipConnector() {
    var connectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(connectors, c => c.IsSameConstructAs(Me));
    return connectors.FirstOrDefault();
}

string GetStationId(IMyShipConnector connector) {
    // First check connector custom data for override
    var customData = connector.CustomData;
    if (!string.IsNullOrEmpty(customData)) {
        var lines = customData.Split('\n');
        foreach (var line in lines) {
            if (line.Trim().StartsWith("StationID=")) {
                return line.Split('=')[1].Trim();
            }
        }
    }

    // Fall back to connected grid name
    var otherConnector = connector.OtherConnector;
    if (otherConnector != null) {
        return otherConnector.CubeGrid.CustomName;
    }

    return "";
}

bool UnloadCargoToStationMultiTick(IMyShipConnector connector) {
    var shipCargoContainers = GetShipContainers(cargoContainerTag);
    var stationIncomingContainers = GetStationContainers(connector, "[INCOMING]");

    if (itemIndex == 0) {
        LogStatus($"  Found {shipCargoContainers.Count} ship cargo containers");
        LogStatus($"  Found {stationIncomingContainers.Count} station incoming containers");
    }

    if (shipCargoContainers.Count == 0) {
        LogStatus("  No ship cargo containers found");
        return true; // Nothing to do, phase complete
    }

    if (stationIncomingContainers.Count == 0) {
        LogStatus("  No station incoming containers found");
        return true; // Nothing to do, phase complete
    }

    // Process ONE container per tick to avoid instruction limit
    if (itemIndex < shipCargoContainers.Count) {
        var shipContainer = shipCargoContainers[itemIndex];
        var inventory = shipContainer.GetInventory(0);

        var items = new List<MyInventoryItem>();
        inventory.GetItems(items);

        LogStatus($"  Container {itemIndex + 1}/{shipCargoContainers.Count}: {shipContainer.CustomName}");

        if (items.Count > 0) {
            LogStatus($"    ({items.Count} item types)");

            MyFixedPoint containerMoved = 0;
            MyFixedPoint minTransferAmount = (MyFixedPoint)0.01;

            foreach (var item in items) {
                // Skip very small amounts that can't be transferred (< 0.01)
                if (item.Amount < minTransferAmount) {
                    LogStatus($"    Skipping {item.Type.SubtypeId} ({item.Amount} - too small)");
                    continue;
                }

                var moved = TransferItemRoundRobin(inventory, stationIncomingContainers, item, item.Amount);
                containerMoved += moved;

                if (moved == 0 && item.Amount >= minTransferAmount) {
                    LogStatus($"    WARNING: Could not unload {item.Type.SubtypeId} (station full?)");
                }
            }
            if (containerMoved > 0) {
                LogStatus($"    Moved {(int)containerMoved} items");
            }
        } else {
            LogStatus($"    Empty - skipping");
        }

        itemIndex++;

        // Check if we're done
        if (itemIndex >= shipCargoContainers.Count) {
            LogStatus($"  Unload complete - processed {itemIndex} containers");
            return true;
        }
        return false; // More containers to process
    }

    // Fallback - shouldn't reach here but just in case
    LogStatus($"  Unload complete (fallback) - processed {itemIndex} containers");
    return true;
}

bool LoadExportsFromStationMultiTick(IMyShipConnector connector) {
    var shipCargoContainers = GetShipContainers(cargoContainerTag);

    if (shipCargoContainers.Count == 0) {
        LogStatus("  No ship cargo containers found");
        return true; // Nothing to do, phase complete
    }

    // Get export configuration from station
    var exports = GetStationExports(connector);

    if (exports.Count == 0) {
        LogStatus("  No exports configured at this station");
        return true; // Nothing to do, phase complete
    }

    // Get ONLY [EXPORT] tagged containers from station
    var stationExportContainers = GetStationContainers(connector, "[EXPORT]");

    if (itemIndex == 0) {
        LogStatus("  Loading exports (only configured items):");
        LogStatus($"    Export config read: {exports.Count} items");
        foreach (var export in exports) {
            if (export.Value == int.MaxValue) {
                LogStatus($"      {export.Key}: unlimited");
            } else {
                LogStatus($"      {export.Key}: {export.Value}");
            }
        }
        LogStatus($"    Found {stationExportContainers.Count} [EXPORT] containers on station");
        LogStatus($"    Found {shipCargoContainers.Count} ship cargo containers");

        // Reset tracking at start of phase
        exportLoadedAmounts.Clear();
    }

    if (stationExportContainers.Count == 0) {
        LogStatus("    WARNING: No [EXPORT] tagged containers found on station");
        LogStatus("    Tag containers with [EXPORT] to enable export loading");
        return true; // Nothing to do, phase complete
    }

    // Process ONE export container per tick to avoid instruction limit
    if (itemIndex < stationExportContainers.Count) {
        var stationContainer = stationExportContainers[itemIndex];
        var inventory = stationContainer.GetInventory(0);

        var items = new List<MyInventoryItem>();
        inventory.GetItems(items);

        if (items.Count > 0) {
            MyFixedPoint containerMoved = 0;

            // Check each item in this container against export config
            foreach (var item in items) {
                // Find matching export config
                foreach (var export in exports) {
                    // Supports exact match ("Ingot.Iron") or category wildcard ("Ingot.ALL")
                    if (ItemMatchesExport(item.Type, export.Key)) {
                        var maxAmount = export.Value;

                        // Track how much we've already loaded of this item
                        MyFixedPoint alreadyLoaded = 0;
                        if (exportLoadedAmounts.ContainsKey(export.Key)) {
                            alreadyLoaded = exportLoadedAmounts[export.Key];
                        }

                        // Calculate remaining amount we can still load
                        MyFixedPoint remainingToLoad = maxAmount - alreadyLoaded;

                        // If we've already loaded the max, skip this item
                        if (remainingToLoad <= 0 && maxAmount != int.MaxValue) {
                            break;
                        }

                        // Determine how much to move from this container
                        var amountToMove = item.Amount;
                        if (maxAmount != int.MaxValue && remainingToLoad < item.Amount) {
                            amountToMove = remainingToLoad;
                        }

                        var moved = TransferItemRoundRobin(inventory, shipCargoContainers, item, amountToMove);

                        if (moved > 0) {
                            containerMoved += moved;

                            // Update tracking
                            if (!exportLoadedAmounts.ContainsKey(export.Key)) {
                                exportLoadedAmounts[export.Key] = 0;
                            }
                            exportLoadedAmounts[export.Key] += moved;

                            LogStatus($"    Loaded {(int)moved} x {item.Type.SubtypeId}");
                        }

                        if (moved == 0) {
                            LogStatus($"  Ship full, stopping export loading");
                            // Ship is full, skip remaining containers
                            itemIndex = stationExportContainers.Count;
                            return true;
                        }

                        break; // Found matching export, move to next item
                    }
                }
            }

            if (containerMoved > 0) {
                LogStatus($"  {stationContainer.CustomName}: Moved {(int)containerMoved} items");
            }
        }

        itemIndex++;
        return false; // More containers to process
    }

    // All containers processed
    LogStatus($"  Export loading complete - processed {itemIndex} containers");
    return true;
}

bool LoadOrdersFromStationMultiTick(IMyShipConnector connector) {
    // Only load when on a mission at home station with a manifest
    if (!missionActive || currentStationId != homeStation || string.IsNullOrEmpty(assignedDestination)) {
        if (!missionActive) {
            LogStatus("  No active mission - skipping order loading");
        } else {
            LogStatus("  Not at home station - skipping order loading");
        }
        return true;
    }

    if (missionManifest.Count == 0) {
        LogStatus("  Empty manifest - nothing to load");
        return true;
    }

    var shipCargoContainers = GetShipContainers(cargoContainerTag);
    if (shipCargoContainers.Count == 0) {
        LogStatus("  No ship cargo containers found");
        return true;
    }

    // Initialize on first tick of this phase
    if (itemIndex == 0) {
        manifestKeys = new List<string>(missionManifest.Keys);
        lastLoadedAmounts.Clear();
        lastCargoFull = false;
        LogStatus($"  Loading manifest for {assignedDestination} ({manifestKeys.Count} item types)");
    }

    // Process ONE manifest item per tick to stay within SE instruction limit
    if (itemIndex < manifestKeys.Count) {
        var itemName = manifestKeys[itemIndex];
        var requestedQty = missionManifest[itemName];
        var itemType = ParseItemType(itemName);

        if (itemType == null) {
            LogStatus($"  ERROR: Invalid item type {itemName}");
            itemIndex++;
            return itemIndex >= manifestKeys.Count;
        }

        // Get station storage containers using station's configured StorageTag
        var storageTags = GetStationStorageTags(connector);
        var stationContainers = GetStationContainers(connector, storageTags);
        MyFixedPoint totalLoaded = 0;
        MyFixedPoint remainingToLoad = requestedQty;

        foreach (var stationContainer in stationContainers) {
            if (remainingToLoad <= 0) break;

            var inventory = stationContainer.GetInventory(0);
            var items = new List<MyInventoryItem>();
            inventory.GetItems(items);

            foreach (var invItem in items) {
                if (invItem.Type != itemType.Value) continue;
                if (remainingToLoad <= 0) break;

                var amountToMove = MyFixedPoint.Min(invItem.Amount, remainingToLoad);
                MyFixedPoint amountBefore = GetItemAmount(inventory, invItem.Type);

                var moved = TransferItemRoundRobin(inventory, shipCargoContainers, invItem, amountToMove);

                if (moved > 0) {
                    totalLoaded += moved;
                    remainingToLoad -= moved;
                }

                // If we tried to move items but couldn't move everything,
                // and source still has stock, cargo is full
                if (moved < amountToMove) {
                    MyFixedPoint amountAfter = GetItemAmount(inventory, invItem.Type);
                    if (amountAfter > 0) {
                        lastCargoFull = true;
                    }
                }

                if (remainingToLoad <= 0) break;
            }
        }

        // Track loaded amounts
        if (totalLoaded > 0) {
            if (!lastLoadedAmounts.ContainsKey(itemName)) {
                lastLoadedAmounts[itemName] = 0;
            }
            lastLoadedAmounts[itemName] += totalLoaded;
        }

        LogStatus($"  Loaded {(int)totalLoaded}/{(int)requestedQty} of {itemName}");

        itemIndex++;
        return itemIndex >= manifestKeys.Count;
    }

    return true;
}


Dictionary<string, MyFixedPoint> GetStationExports(IMyShipConnector connector) {
    var exports = new Dictionary<string, MyFixedPoint>();

    // Find station's programmable block
    var otherGrid = connector.OtherConnector?.CubeGrid;
    if (otherGrid == null) {
        return exports;
    }

    var programmableBlocks = new List<IMyProgrammableBlock>();
    GridTerminalSystem.GetBlocksOfType(programmableBlocks, pb => pb.CubeGrid == otherGrid);

    if (programmableBlocks.Count == 0) return exports;

    // Find PB with StationID configured in its CustomData
    IMyProgrammableBlock stationPB = null;
    foreach (var pb in programmableBlocks) {
        var pbLines = pb.CustomData.Split('\n');
        foreach (var pbLine in pbLines) {
            var trimmed = pbLine.Trim();
            if (trimmed.StartsWith("StationID=") || trimmed.StartsWith("StationID =")) {
                stationPB = pb;
                break;
            }
        }
        if (stationPB != null) break;
    }

    if (stationPB == null) {
        LogStatus("  No PB with StationID found - exports disabled");
        return exports;
    }

    // Read the tagged PB's custom data for exports
    var customData = stationPB.CustomData;
    var lines = customData.Split('\n');

    bool inExports = false;

    foreach (var line in lines) {
        var trimmed = line.Trim();

        if (trimmed == "---EXPORTS---") {
            inExports = true;
            continue;
        }

        if (trimmed.StartsWith("---") || trimmed.StartsWith("[")) {
            inExports = false;
        }

        if (inExports && !string.IsNullOrEmpty(trimmed)) {
            // Supported formats:
            // 1. "Item.Type:Amount" - take up to specified amount (e.g., Ingot.Iron:1000)
            // 2. "Item.Type" - take as much as ship can hold (e.g., Ingot.Iron)
            // 3. "Category.ALL:Amount" - take up to amount of ANY item in category (e.g., Ingot.ALL:5000)
            // 4. "Category.ALL" - take as much as possible of ANY item in category (e.g., Component.ALL)
            if (trimmed.Contains(":")) {
                var parts = trimmed.Split(':');
                if (parts.Length == 2) {
                    var itemName = parts[0].Trim();
                    int amount;
                    if (int.TryParse(parts[1].Trim(), out amount)) {
                        exports[itemName] = amount;
                    }
                }
            } else {
                // No quantity specified - use max value to take as much as possible
                exports[trimmed] = int.MaxValue;
            }
        }
    }

    return exports;
}

MyItemType? ParseItemType(string itemString) {
    // Format: "Ore.Iron" or "Ingot.Iron" or "Component.SteelPlate"
    if (!itemString.Contains(".")) return null;

    var parts = itemString.Split('.');
    if (parts.Length != 2) return null;

    var category = parts[0].Trim();
    var subtype = parts[1].Trim();

    string typeId = "";

    if (category == "Ore") typeId = "MyObjectBuilder_Ore";
    else if (category == "Ingot") typeId = "MyObjectBuilder_Ingot";
    else if (category == "Component") typeId = "MyObjectBuilder_Component";
    else if (category == "AmmoMagazine") typeId = "MyObjectBuilder_AmmoMagazine";
    else if (category == "PhysicalGunObject") typeId = "MyObjectBuilder_PhysicalGunObject";
    else if (category == "ConsumableItem") typeId = "MyObjectBuilder_ConsumableItem";
    else return null;

    return new MyItemType(typeId, subtype);
}

// Check if an item matches an export config entry
// Supports exact match ("Ingot.Iron") or category wildcard ("Ingot.ALL")
bool ItemMatchesExport(MyItemType itemType, string exportKey) {
    if (!exportKey.Contains('.')) return false;

    var parts = exportKey.Split('.');
    if (parts.Length != 2) return false;

    var category = parts[0].Trim();
    var subtype = parts[1].Trim();

    // Get the expected TypeId for this category
    string expectedTypeId;
    if (category == "Ore") expectedTypeId = "MyObjectBuilder_Ore";
    else if (category == "Ingot") expectedTypeId = "MyObjectBuilder_Ingot";
    else if (category == "Component") expectedTypeId = "MyObjectBuilder_Component";
    else if (category == "AmmoMagazine") expectedTypeId = "MyObjectBuilder_AmmoMagazine";
    else if (category == "PhysicalGunObject") expectedTypeId = "MyObjectBuilder_PhysicalGunObject";
    else if (category == "ConsumableItem") expectedTypeId = "MyObjectBuilder_ConsumableItem";
    else return false;

    // Check if item's TypeId matches the category
    if (itemType.TypeId != expectedTypeId) return false;

    // If subtype is "ALL", match any item in this category
    if (subtype.Equals("ALL", StringComparison.OrdinalIgnoreCase)) {
        return true;
    }

    // Otherwise, require exact subtype match
    return itemType.SubtypeId == subtype;
}

List<IMyCargoContainer> GetShipContainers(string tag) {
    var containers = new List<IMyCargoContainer>();
    var connector = GetShipConnector();

    // If docked, exclude station grid; if undocked, include all connected grids
    if (connector != null && connector.Status == MyShipConnectorStatus.Connected) {
        // Docked - exclude station grid
        var stationGrid = connector.OtherConnector?.CubeGrid;
        GridTerminalSystem.GetBlocksOfType(containers, c =>
            c.IsSameConstructAs(Me) &&
            c.CubeGrid != stationGrid &&
            (c.CustomName.Contains(tag) || c.CustomData.Contains(tag))
        );
    } else {
        // Undocked - include all mechanically connected grids
        GridTerminalSystem.GetBlocksOfType(containers, c =>
            c.IsSameConstructAs(Me) &&
            (c.CustomName.Contains(tag) || c.CustomData.Contains(tag))
        );
    }

    return containers;
}

List<IMyCargoContainer> GetStationContainers(IMyShipConnector connector, params string[] tags) {
    var containers = new List<IMyCargoContainer>();
    var otherGrid = connector.OtherConnector?.CubeGrid;

    if (otherGrid == null) return containers;

    GridTerminalSystem.GetBlocksOfType(containers, c => {
        if (c.CubeGrid != otherGrid) return false;

        // Exclude containers marked with [EXCLUDE] or [NO-AUTO]
        if (c.CustomName.Contains("[EXCLUDE]") || c.CustomData.Contains("[EXCLUDE]")) return false;
        if (c.CustomName.Contains("[NO-AUTO]") || c.CustomData.Contains("[NO-AUTO]")) return false;

        foreach (var tag in tags) {
            if (c.CustomName.Contains(tag) || c.CustomData.Contains(tag)) {
                return true;
            }
        }
        return false;
    });

    return containers;
}

string[] GetStationStorageTags(IMyShipConnector connector) {
    var otherGrid = connector.OtherConnector?.CubeGrid;
    if (otherGrid != null) {
        var pbs = new List<IMyProgrammableBlock>();
        GridTerminalSystem.GetBlocksOfType(pbs, pb => pb.CubeGrid == otherGrid);

        foreach (var pb in pbs) {
            var pbLines = pb.CustomData.Split('\n');
            foreach (var pbLine in pbLines) {
                var trimmed = pbLine.Trim();
                if (trimmed.StartsWith("StorageTag=")) {
                    var tag = trimmed.Split('=')[1].Trim();
                    if (!string.IsNullOrEmpty(tag)) {
                        return new string[] { tag };
                    }
                }
            }
        }
    }

    // Default fallback
    return new string[] { "[STORAGE]", "[STOCK]" };
}


MyFixedPoint TransferItemRoundRobin(IMyInventory source, List<IMyCargoContainer> destinations, MyInventoryItem item, MyFixedPoint amount) {
    if (destinations.Count == 0) return 0;

    MyFixedPoint totalMoved = 0;

    // Transfer entire remaining stack to each destination sequentially
    // This eliminates trace amounts by always transferring complete stacks
    foreach (var dest in destinations) {
        var destInv = dest.GetInventory(0);
        MyFixedPoint remainingAmount = amount - totalMoved;
        if (remainingAmount <= 0) break;

        // Get amount before transfer from source inventory (not from stale item object)
        MyFixedPoint amountBefore = GetItemAmount(source, item.Type);

        // Try to transfer entire remaining stack to this destination
        if (source.TransferItemTo(destInv, item, remainingAmount)) {
            // Calculate how much was actually moved by checking source inventory again
            MyFixedPoint amountAfter = GetItemAmount(source, item.Type);
            MyFixedPoint actualMoved = amountBefore - amountAfter;
            totalMoved += actualMoved;

            // If we moved everything, we're done
            if (actualMoved >= remainingAmount) {
                break;
            }
            // Otherwise, destination is full, try next one
        }
    }

    return totalMoved;
}

MyFixedPoint GetItemAmount(IMyInventory inventory, MyItemType itemType) {
    var items = new List<MyInventoryItem>();
    inventory.GetItems(items);

    MyFixedPoint total = 0;
    foreach (var item in items) {
        if (item.Type == itemType) {
            total += item.Amount;
        }
    }
    return total;
}

void BroadcastAvailability() {
    // Broadcast availability to supply stations with tags
    // Format: AVAILABLE|DroneID|HomeStation|CurrentStation|Tags
    var tagsString = string.Join(",", droneTags);
    var message = $"AVAILABLE|{droneId}|{homeStation}|{currentStationId}|{tagsString}";
    IGC.SendBroadcastMessage("DRONE_AVAILABLE", message);
}

void ProcessAssignments() {
    while (assignmentListener.HasPendingMessage) {
        var message = assignmentListener.AcceptMessage();
        var data = message.Data.ToString();

        // Format: "ASSIGNED|DroneID|Destination|Item:Qty,Item:Qty,..."
        var parts = data.Split('|');

        if (parts.Length < 4 || parts[0] != "ASSIGNED") continue;

        var targetDroneId = parts[1];
        var destination = parts[2];

        // Only respond if assignment is for this drone
        if (targetDroneId != droneId) continue;

        // Parse manifest
        missionManifest.Clear();
        var itemPairs = parts[3].Split(',');
        foreach (var pair in itemPairs) {
            var itemParts = pair.Split(':');
            if (itemParts.Length == 2) {
                int qty;
                if (int.TryParse(itemParts[1].Trim(), out qty)) {
                    missionManifest[itemParts[0].Trim()] = qty;
                }
            }
        }

        LogStatus($"\n*** MISSION ASSIGNED: Deliver to {destination} ***");
        LogStatus($"    Manifest: {missionManifest.Count} item type(s)");

        // Claim the mission
        var claimMessage = $"CLAIMED|{droneId}|{destination}";
        IGC.SendBroadcastMessage("CARGO_CLAIMED", claimMessage);

        // Set mission state
        assignedDestination = destination;
        missionActive = true;
        cargoOperationsComplete = false;
        currentPhase = 2; // Skip unload/exports, go straight to loading orders
        itemIndex = 0;

        // Check if we need to fly home first (remote drone recovery)
        if (currentStationId != homeStation) {
            LogStatus($"\n*** Not at home - returning to {homeStation} first ***");
            TriggerAutopilot(homeStation);
            // missionActive, assignedDestination, missionManifest persist across undock
            // When we land at home, existing Phase 0/1/2/3 logic handles the rest
            return;
        }

        LogStatus($"Mission claimed - preparing to load cargo for {destination}");
    }
}

void TriggerAutopilot(string stationId) {
    if (autopilotPB == null) {
        LogStatus("ERROR: Autopilot PB not found!");
        LogStatus("Check AutopilotPBName in CustomData");
        return;
    }

    // Get connector name for this station (defaults to stationId if not mapped)
    string connectorName = stationId;
    if (connectorNames.ContainsKey(stationId)) {
        connectorName = connectorNames[stationId];
    }

    var command = $"GO {connectorName}";
    LogStatus($">>> Triggering autopilot: {command}");

    if (!autopilotPB.TryRun(command)) {
        LogStatus("ERROR: Failed to trigger autopilot!");
    }
}

void LogStatus(string message) {
    statusLog.AppendLine(message);
}

void UpdateDisplays() {
    // Echo to PB screen
    Echo(statusLog.ToString());

    // Update LCD panels and cockpit surfaces
    var connector = GetShipConnector();
    IMyCubeGrid stationGrid = null;
    if (connector != null && connector.Status == MyShipConnectorStatus.Connected) {
        stationGrid = connector.OtherConnector?.CubeGrid;
    }

    var surfaces = GetDisplaySurfaces(statusLcdTag, stationGrid);
    foreach (var surface in surfaces) {
        surface.ContentType = ContentType.TEXT_AND_IMAGE;
        surface.WriteText(statusLog.ToString());
    }
}

// Returns all IMyTextSurface instances tagged with the given tag, sorted by block name.
// Supports both standalone LCD panels (IMyTextPanel) and multi-surface blocks such as
// cockpits and control seats (IMyTextSurfaceProvider).
//
// For IMyTextPanel: tag in block name OR CustomData (backward compatible).
// For IMyTextSurfaceProvider: tag in CustomData with optional ":N" surface index.
//   e.g. "[Status]:1" -> surface 1
// excludeGrid: when docked, pass station grid to exclude station displays.
List<IMyTextSurface> GetDisplaySurfaces(string tag, IMyCubeGrid excludeGrid) {
    var entries = new List<KeyValuePair<string, IMyTextSurface>>();

    // Standard LCD panels — tag matched in name or CustomData
    var panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, p =>
        p.IsSameConstructAs(Me) &&
        (excludeGrid == null || p.CubeGrid != excludeGrid) &&
        (p.CustomName.Contains(tag) || p.CustomData.Contains(tag)));
    foreach (var p in panels) {
        entries.Add(new KeyValuePair<string, IMyTextSurface>(p.CustomName, p));
    }

    // Multi-surface providers (cockpits, control seats, etc.) — tag matched in CustomData
    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(allBlocks, b =>
        b.IsSameConstructAs(Me) &&
        (excludeGrid == null || b.CubeGrid != excludeGrid) &&
        !(b is IMyTextPanel) &&
        b is IMyTextSurfaceProvider &&
        b.CustomData.Contains(tag));
    foreach (var block in allBlocks) {
        var provider = (IMyTextSurfaceProvider)block;
        int idx = ParseSurfaceIndex(block.CustomData, tag);
        if (idx >= 0 && idx < provider.SurfaceCount) {
            entries.Add(new KeyValuePair<string, IMyTextSurface>(block.CustomName, provider.GetSurface(idx)));
        }
    }

    // Sort all surfaces by block name for consistent ordering
    entries.Sort((a, b) => string.Compare(a.Key, b.Key));

    var result = new List<IMyTextSurface>();
    foreach (var entry in entries) result.Add(entry.Value);
    return result;
}

// Parses the surface index from a block's CustomData for a given tag.
// Looks for "tag" or "tag:N" on its own line.
// Returns the index (0 if unspecified).
int ParseSurfaceIndex(string customData, string tag) {
    foreach (var line in customData.Split('\n')) {
        var trimmed = line.Trim();
        if (trimmed.StartsWith(tag)) {
            var rest = trimmed.Substring(tag.Length).Trim();
            if (rest.StartsWith(":")) {
                int idx;
                if (int.TryParse(rest.Substring(1).Trim(), out idx)) {
                    return idx;
                }
            }
            return 0; // Tag found without index — default surface 0
        }
    }
    return 0;
}

#endregion // CargoShip
