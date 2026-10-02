#region SupplyStation
const string VERSION = "2.8.0";

/*
 * SupplyStation Script v2.8.0
 * Protocol: v2 (compatible with OrderStation v2.x, CargoShip v2.x)
 *
 * Version History:
 * 2.8.0 - Sprite display, themes, ---DISPLAY--- config, multi-panel support
 * 2.7.0 - Mission lifecycle display (Loading/En Route/Returning/Complete), auto-clear
 * 2.6.0 - Configurable storage tags, GSIM/GOAT compatibility
 * 2.5.0 - Startup validation system (CRITICAL/WARNING/INFO)
 * 2.4.0 - Remote drone recovery, prefers local drones for dispatch
 * 2.3.0 - INVENTORY_REQUEST/STATION_INVENTORY for remote inventory queries
 * 2.2.1 - Fixed double-dispatch race condition (pendingDispatch tracking)
 * 2.2.0 - STOCK_UNAVAILABLE broadcast, cancels unfulfillable orders
 * 2.1.0 - Pickup requests (export-only collection)
 * 2.0.0 - Fire-and-forget dispatch with manifest, removed ORDER_STATUS
 * 1.0.0 - Initial release
 */

// Startup Validation System
public enum ValidationSeverity { CRITICAL, WARNING, INFO }

public struct ValidationResult {
    public ValidationSeverity Severity;
    public string Category;
    public string Message;
}

public class StartupValidator {
    List<ValidationResult> results = new List<ValidationResult>();

    public void Critical(string category, string message) {
        results.Add(new ValidationResult { Severity = ValidationSeverity.CRITICAL, Category = category, Message = message });
    }

    public void Warning(string category, string message) {
        results.Add(new ValidationResult { Severity = ValidationSeverity.WARNING, Category = category, Message = message });
    }

    public void Info(string category, string message) {
        results.Add(new ValidationResult { Severity = ValidationSeverity.INFO, Category = category, Message = message });
    }

    public bool HasCritical() {
        foreach (var r in results) {
            if (r.Severity == ValidationSeverity.CRITICAL) return true;
        }
        return false;
    }

    public string FormatForLcd() {
        var sb = new StringBuilder();
        sb.AppendLine("=== VALIDATION ===");

        if (results.Count == 0) {
            sb.AppendLine("[OK] All checks passed");
            return sb.ToString();
        }

        int critical = 0, warnings = 0;
        foreach (var r in results) {
            if (r.Severity == ValidationSeverity.CRITICAL) critical++;
            else if (r.Severity == ValidationSeverity.WARNING) warnings++;
        }

        if (critical > 0 || warnings > 0) {
            sb.AppendLine($"[!] {critical} critical, {warnings} warnings");
            sb.AppendLine();
        }

        if (critical > 0) {
            sb.AppendLine("--- CRITICAL ---");
            foreach (var r in results) {
                if (r.Severity == ValidationSeverity.CRITICAL) {
                    sb.AppendLine($"[X] {r.Category}: {r.Message}");
                }
            }
        }

        if (warnings > 0) {
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
            string prefix = "";
            if (r.Severity == ValidationSeverity.CRITICAL) prefix = "[CRITICAL]";
            else if (r.Severity == ValidationSeverity.WARNING) prefix = "[WARNING]";
            else prefix = "[INFO]";

            sb.AppendLine($"{prefix} {r.Category}: {r.Message}");
        }

        return sb.ToString();
    }

    public void Clear() {
        results.Clear();
    }
}

// Configuration
string stationId = "";
string statusLcdTag = "[STATUS]";
string storageContainerTag = "[STOCK]";
bool stationIdFromConfig = false; // Track if StationID was explicitly configured
Dictionary<string, Dictionary<string, MyFixedPoint>> activeOrders = new Dictionary<string, Dictionary<string, MyFixedPoint>>();
Dictionary<string, Dictionary<string, MyFixedPoint>> cachedAvailability = new Dictionary<string, Dictionary<string, MyFixedPoint>>();

// Startup Validation
StartupValidator validator = new StartupValidator();
bool validationFailed = false;

// Drone dispatch system
Dictionary<string, DateTime> availableDrones = new Dictionary<string, DateTime>(); // DroneID -> Last seen time
Dictionary<string, List<string>> droneTags = new Dictionary<string, List<string>>(); // DroneID -> List of capability tags
Dictionary<string, string> droneCurrentStation = new Dictionary<string, string>(); // DroneID -> Current station (for remote drone recovery)
Dictionary<string, List<string>> destinationRequirements = new Dictionary<string, List<string>>(); // Destination -> Required tags
HashSet<string> assignedOrders = new HashSet<string>(); // Track which orders have been assigned to drones
HashSet<string> pendingDispatch = new HashSet<string>(); // Orders awaiting CLAIMED confirmation
Dictionary<string, DateTime> pendingDispatchTime = new Dictionary<string, DateTime>(); // When dispatch was sent
Dictionary<string, string> orderAssignments = new Dictionary<string, string>(); // Destination -> DroneID
Dictionary<string, string> missionStates = new Dictionary<string, string>(); // Destination -> State (ASSIGNED, OUTBOUND, RETURNING, COMPLETE)
Dictionary<string, DateTime> missionCompletionTime = new Dictionary<string, DateTime>(); // Destination -> when COMPLETE received
int completedMissionTimeout = 300; // seconds before completed missions clear from display

StringBuilder statusLog = new StringBuilder();

// Display System
struct ThemeColors {
    public Color BG, HDR, LBL, VAL, GOOD, WARN, CRIT, DIM;
}
static ThemeColors ThemeDefault() { return new ThemeColors {
    BG = new Color(8, 8, 18), HDR = new Color(0, 210, 210), LBL = new Color(0, 160, 160),
    VAL = new Color(200, 200, 215), GOOD = new Color(0, 210, 90), WARN = new Color(255, 185, 0),
    CRIT = new Color(255, 55, 55), DIM = new Color(35, 40, 50)
};}
static ThemeColors ThemeMilitary() { return new ThemeColors {
    BG = new Color(10, 12, 8), HDR = new Color(180, 190, 100), LBL = new Color(140, 150, 80),
    VAL = new Color(190, 195, 170), GOOD = new Color(80, 180, 50), WARN = new Color(220, 170, 30),
    CRIT = new Color(220, 50, 40), DIM = new Color(30, 35, 25)
};}
static ThemeColors ThemeHighContrast() { return new ThemeColors {
    BG = new Color(0, 0, 0), HDR = new Color(255, 255, 255), LBL = new Color(200, 200, 200),
    VAL = new Color(255, 255, 255), GOOD = new Color(0, 255, 0), WARN = new Color(255, 255, 0),
    CRIT = new Color(255, 0, 0), DIM = new Color(60, 60, 60)
};}
static ThemeColors ThemeMinimal() { return new ThemeColors {
    BG = new Color(15, 15, 20), HDR = new Color(160, 165, 175), LBL = new Color(120, 125, 135),
    VAL = new Color(180, 180, 190), GOOD = new Color(100, 200, 120), WARN = new Color(220, 180, 60),
    CRIT = new Color(220, 70, 60), DIM = new Color(40, 42, 48)
};}
static ThemeColors ThemePM() { return new ThemeColors {
    BG = new Color(0, 0, 0), HDR = new Color(100, 200, 255), LBL = new Color(255, 200, 50),
    VAL = new Color(200, 200, 200), GOOD = new Color(80, 220, 80), WARN = new Color(255, 170, 0),
    CRIT = new Color(255, 60, 60), DIM = new Color(55, 55, 65)
};}

struct DisplayLine {
    public string Text;
    public string RightText;
    public Color Col;
    public Color DotCol;
    public bool IsSep;
    public bool IsBanner;
    public DisplayLine(string text, Color col, string rightText = "") {
        Text = text; Col = col; RightText = rightText;
        DotCol = default(Color); IsSep = false; IsBanner = false;
    }
}
class DisplayGroup {
    public string Tag;
    public string[] Sections;
    public List<IMyTextSurface> Panels = new List<IMyTextSurface>();
}

List<DisplayLine> _displayLines = new List<DisplayLine>();
List<DisplayGroup> _displayGroups = new List<DisplayGroup>();
ThemeColors _theme;
string _themeName = "pm";
int _renderGroupIdx = 0;
int _renderTick = 0;
string _lastCustomData = null;

void DL(string text, Color col) { _displayLines.Add(new DisplayLine(text, col)); }
void DLR(string text, Color col, string right) { _displayLines.Add(new DisplayLine(text, col, right)); }
void DLDot(string text, Color col, Color dot, string right = "") {
    _displayLines.Add(new DisplayLine(text, col, right) { DotCol = dot });
}
void DLSep() { _displayLines.Add(new DisplayLine("", _theme.VAL) { IsSep = true }); }
void DLBanner(string title, string subtitle) {
    _displayLines.Add(new DisplayLine(title, _theme.HDR, subtitle) { IsBanner = true });
}

static MySprite Txt(string s, Vector2 pos, Color col, float scale, TextAlignment align = TextAlignment.LEFT)
    => new MySprite(SpriteType.TEXT, s, pos, null, col, "Monospace", align, scale);
static MySprite Rect(Vector2 center, Vector2 size, Color col)
    => new MySprite(SpriteType.TEXTURE, "SquareSimple", center, size, col);
static Color SaltColor(Color c, int a) => new Color(c.R, c.G, c.B, (byte)Math.Min(a, c.A));

IMyBroadcastListener orderListener;
IMyBroadcastListener droneAvailableListener;
IMyBroadcastListener cargoClaimedListener;
IMyBroadcastListener missionCompleteListener;
IMyBroadcastListener droneDepartedListener;
IMyBroadcastListener inventoryRequestListener;

public Program() {
    // First-time setup: write default config if CustomData is empty
    if (string.IsNullOrWhiteSpace(Me.CustomData)) {
        Me.CustomData = GetDefaultConfig();
        Echo("=== FIRST-TIME SETUP ===");
        Echo("Default config written to CustomData.");
        Echo("Edit CustomData and recompile.");
        Runtime.UpdateFrequency = UpdateFrequency.None;
        return;
    }

    LoadConfig();
    ScanDisplayPanels();
    _lastCustomData = Me.CustomData;
    RunValidation();

    // Setup antenna listener for orders
    orderListener = IGC.RegisterBroadcastListener("STATION_ORDERS");
    orderListener.SetMessageCallback("ORDER_RECEIVED");

    // Setup listeners for drone dispatch system
    droneAvailableListener = IGC.RegisterBroadcastListener("DRONE_AVAILABLE");
    droneAvailableListener.SetMessageCallback("DRONE_AVAILABLE");

    cargoClaimedListener = IGC.RegisterBroadcastListener("CARGO_CLAIMED");
    cargoClaimedListener.SetMessageCallback("CARGO_CLAIMED");

    missionCompleteListener = IGC.RegisterBroadcastListener("MISSION_COMPLETE");
    missionCompleteListener.SetMessageCallback("MISSION_COMPLETE");

    droneDepartedListener = IGC.RegisterBroadcastListener("DRONE_DEPARTED");
    droneDepartedListener.SetMessageCallback("DRONE_DEPARTED");

    inventoryRequestListener = IGC.RegisterBroadcastListener("INVENTORY_REQUEST");
    inventoryRequestListener.SetMessageCallback("INVENTORY_REQUEST");

    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Save() {}

public void Main(string argument, UpdateType updateSource) {
    statusLog.Clear();

    // Handle VALIDATE argument (always allowed)
    if (!string.IsNullOrEmpty(argument) && argument.ToUpper() == "VALIDATE") {
        RunValidation();
        UpdateDisplays();
        RenderDisplays();
        return;
    }

    // Handle reload argument - re-run validation
    if (!string.IsNullOrEmpty(argument) && argument.ToUpper() == "RELOAD") {
        LoadConfig();
        ScanDisplayPanels();
        _lastCustomData = Me.CustomData;
        RunValidation();
        UpdateDisplays();
        RenderDisplays();
        return;
    }

    // Auto-detect CustomData changes
    if (Me.CustomData != _lastCustomData) {
        LoadConfig();
        ScanDisplayPanels();
        _lastCustomData = Me.CustomData;
    }

    // HALT if critical validation errors exist
    if (validationFailed) {
        Echo("HALTED: Fix critical errors and recompile");
        Echo(validator.FormatForEcho());
        return;
    }

    try {
        LogStatus($"Supply Station: {stationId}");
        LogStatus("===================");

        // Always check for incoming orders (not just on IGC update)
        ProcessIncomingOrders();

        // Process inventory requests
        ProcessInventoryRequests();

        // Process drone availability announcements
        ProcessDroneAvailability();

        // Check availability for active orders
        CheckAvailability();

        // Dispatch missions to available drones
        DispatchMissions();

        // Auto-clear completed missions after timeout
        CleanupCompletedMissions();

        // Display active orders (not yet assigned to a drone)
        LogStatus("\n---ACTIVE ORDERS---");
        bool hasActiveOrders = false;
        foreach (var order in activeOrders) {
            var destination = order.Key;
            // Skip orders that have a drone assigned (shown in MISSIONS)
            if (assignedOrders.Contains(destination) || orderAssignments.ContainsKey(destination)) {
                continue;
            }
            hasActiveOrders = true;
            LogStatus($"\nOrder for {destination}:");
            if (cachedAvailability.ContainsKey(destination)) {
                var avail = cachedAvailability[destination];
                foreach (var item in order.Value) {
                    MyFixedPoint available = avail.ContainsKey(item.Key) ? avail[item.Key] : (MyFixedPoint)0;
                    var requested = item.Value;
                    if (available >= requested) {
                        LogStatus($"  {item.Key}: AVAILABLE ({(int)available}/{(int)requested})");
                    } else if (available > 0) {
                        LogStatus($"  {item.Key}: PARTIAL ({(int)available}/{(int)requested})");
                    } else {
                        LogStatus($"  {item.Key}: UNAVAILABLE (0/{(int)requested})");
                    }
                }
            }
            if (pendingDispatch.Contains(destination)) {
                LogStatus("  Status: Dispatching...");
            } else {
                LogStatus("  Status: Waiting for drone");
            }
        }
        if (!hasActiveOrders) {
            LogStatus("No active orders");
        }

        // Display missions (assigned drones and in-progress deliveries)
        if (orderAssignments.Count > 0) {
            LogStatus("\n---MISSIONS---");
            foreach (var assignment in orderAssignments) {
                var destination = assignment.Key;
                var droneId = assignment.Value;
                var state = missionStates.ContainsKey(destination) ? missionStates[destination] : "ASSIGNED";

                if (state == "ASSIGNED") {
                    LogStatus($"  {destination} -> {droneId} (LOADING)");
                } else if (state == "OUTBOUND") {
                    LogStatus($"  {destination} -> {droneId} (EN ROUTE)");
                } else if (state == "RETURNING") {
                    LogStatus($"  {destination} -> {droneId} (RETURNING)");
                } else if (state == "COMPLETE") {
                    var sinceComplete = DateTime.Now - missionCompletionTime[destination];
                    var remaining = completedMissionTimeout - (int)sinceComplete.TotalSeconds;
                    LogStatus($"  {destination} -> {droneId} (COMPLETE - clears in {FormatElapsed(TimeSpan.FromSeconds(remaining))})");
                }
            }
        }

        // Display drone status
        if (availableDrones.Count > 0) {
            LogStatus("\n---AVAILABLE DRONES---");
            foreach (var drone in availableDrones) {
                var droneId = drone.Key;

                // Show tags if available
                if (droneTags.ContainsKey(droneId) && droneTags[droneId].Count > 0) {
                    var tagsStr = string.Join(",", droneTags[droneId]);
                    LogStatus($"  {droneId} [{tagsStr}]");
                } else {
                    LogStatus($"  {droneId}");
                }
            }
        }

        // Display destination requirements if configured
        if (destinationRequirements.Count > 0) {
            LogStatus("\n---DESTINATION REQUIREMENTS---");
            foreach (var req in destinationRequirements) {
                var tagsStr = string.Join(",", req.Value);
                LogStatus($"  {req.Key}: {tagsStr}");
            }
        }

        // Broadcast availability
        BroadcastStationStatus();

    } catch (Exception ex) {
        LogStatus($"ERROR: {ex.Message}");
        LogStatus($"Stack: {ex.StackTrace}");
    }

    UpdateDisplays();
    RenderDisplays();
}

void LoadConfig() {
    var config = Me.CustomData;
    var lines = config.Split('\n');
    bool inDestinationRequirements = false;
    bool inDisplay = false;

    _displayGroups.Clear();

    foreach (var line in lines) {
        var trimmed = line.Trim();
        if (trimmed.StartsWith(";")) continue;

        // Section detection
        if (trimmed == "---DESTINATION-REQUIREMENTS---") {
            inDestinationRequirements = true;
            inDisplay = false;
            continue;
        }
        if (trimmed == "---DISPLAY---") {
            inDisplay = true;
            inDestinationRequirements = false;
            continue;
        }
        if (trimmed.StartsWith("---")) {
            inDestinationRequirements = false;
            inDisplay = false;
            continue;
        }

        if (!trimmed.Contains("=")) continue;

        var parts = trimmed.Split(new[] { '=' }, 2);
        var key = parts[0].Trim();
        var value = parts[1].Trim();

        if (inDisplay) {
            // Display config
            if (key == "Theme") {
                _themeName = value.ToLower();
            } else if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value)) {
                _displayGroups.Add(new DisplayGroup {
                    Tag = "[" + key + "]",
                    Sections = value.Split(',')
                });
            }
        } else if (inDestinationRequirements) {
            // Destination requirements: StationID=TAG,TAG,TAG
            var tags = new List<string>();
            var tagArray = value.Split(',');
            foreach (var tag in tagArray) {
                var trimmedTag = tag.Trim().ToUpper();
                if (!string.IsNullOrEmpty(trimmedTag)) {
                    tags.Add(trimmedTag);
                }
            }
            if (tags.Count > 0) {
                destinationRequirements[key] = tags;
            }
        } else {
            // Main config settings
            if (key == "StationID") {
                stationId = value;
                stationIdFromConfig = !string.IsNullOrEmpty(value);
            }
            else if (key == "StatusLcdTag") statusLcdTag = value;
            else if (key == "StorageTag") storageContainerTag = value;
            else if (key == "CompletedMissionTimeout") {
                int timeout;
                if (int.TryParse(value, out timeout) && timeout > 0) {
                    completedMissionTimeout = timeout;
                }
            }
        }
    }

    // Fall back to grid name if StationID not configured
    if (string.IsNullOrEmpty(stationId)) {
        stationId = Me.CubeGrid.CustomName;
    }

    // Apply theme
    switch (_themeName) {
        case "military":     _theme = ThemeMilitary(); break;
        case "highcontrast": _theme = ThemeHighContrast(); break;
        case "minimal":      _theme = ThemeMinimal(); break;
        case "pm":           _theme = ThemePM(); break;
        default:             _theme = ThemeDefault(); break;
    }

    // Auto-migrate: if no ---DISPLAY--- section, generate one from StatusLcdTag
    if (_displayGroups.Count == 0) {
        var sb = new StringBuilder();
        sb.AppendLine("\n---DISPLAY---");
        sb.AppendLine("; Theme: default, military, highcontrast, minimal, pm");
        sb.AppendLine("Theme=pm");
        sb.AppendLine("; Tag=Sections  (Sections: Orders, Missions, Drones, Status)");

        string statusTag = string.IsNullOrEmpty(statusLcdTag) ? "[STATUS]" : statusLcdTag;
        string statusKey = statusTag.TrimStart('[').TrimEnd(']');
        sb.AppendLine($"{statusKey}=Orders,Missions,Drones,Status");

        Me.CustomData += sb.ToString();

        _displayGroups.Add(new DisplayGroup {
            Tag = statusTag,
            Sections = new[] { "Orders", "Missions", "Drones", "Status" }
        });
    }
}

void ProcessIncomingOrders() {
    while (orderListener.HasPendingMessage) {
        var message = orderListener.AcceptMessage();
        var data = message.Data.ToString();

        // Format: "ORDER|SourceStationID|DestStationID|Item:Qty,Item:Qty,..." or "ORDER|Source|Dest|PICKUP"
        var parts = data.Split('|');

        if (parts.Length < 4 || parts[0] != "ORDER") {
            continue;
        }

        var sourceStation = parts[1];
        var destStation = parts[2];
        var itemsData = parts[3];

        // Only process if we're the destination
        if (destStation != stationId) {
            continue;
        }

        // Check if this is a pickup-only request (no items, just export collection)
        if (itemsData.Trim().ToUpper() == "PICKUP") {
            LogStatus($"\nReceived PICKUP request from {sourceStation}");
            LogStatus("  (Drone will collect exports only)");
            activeOrders[sourceStation] = new Dictionary<string, MyFixedPoint>(); // Empty order
            continue;
        }

        LogStatus($"\nReceived order from {sourceStation}:");

        var orderItems = new Dictionary<string, MyFixedPoint>();
        var itemPairs = itemsData.Split(',');

        foreach (var pair in itemPairs) {
            var itemParts = pair.Split(':');
            if (itemParts.Length == 2) {
                var itemName = itemParts[0].Trim();
                int qty;
                if (int.TryParse(itemParts[1].Trim(), out qty)) {
                    orderItems[itemName] = qty;
                    LogStatus($"  {itemName}: {qty}");
                }
            }
        }

        if (orderItems.Count > 0) {
            activeOrders[sourceStation] = orderItems;
        }
    }
}

void ProcessInventoryRequests() {
    while (inventoryRequestListener.HasPendingMessage) {
        var message = inventoryRequestListener.AcceptMessage();
        var data = message.Data.ToString();

        // Format: "INVENTORY_REQUEST|RequestingStationID|TargetStationID"
        var parts = data.Split('|');

        if (parts.Length < 3 || parts[0] != "INVENTORY_REQUEST") {
            continue;
        }

        var requestingStation = parts[1];
        var targetStation = parts[2];

        // Only respond if we're the target station
        if (targetStation != stationId) {
            continue;
        }

        LogStatus($"\nInventory request from {requestingStation}");

        // Scan all storage containers and build inventory totals
        var storageContainers = GetContainers(storageContainerTag);
        var inventoryTotals = new Dictionary<string, MyFixedPoint>();

        foreach (var container in storageContainers) {
            var inventory = container.GetInventory(0);
            var items = new List<MyInventoryItem>();
            inventory.GetItems(items);

            foreach (var item in items) {
                var category = GetItemCategory(item.Type);
                if (string.IsNullOrEmpty(category)) continue;

                var key = $"{category}:{item.Type.SubtypeId}";
                if (inventoryTotals.ContainsKey(key)) {
                    inventoryTotals[key] += item.Amount;
                } else {
                    inventoryTotals[key] = item.Amount;
                }
            }
        }

        // Build response message
        var itemParts = new List<string>();
        foreach (var kvp in inventoryTotals) {
            itemParts.Add($"{kvp.Key}:{(int)kvp.Value}");
        }
        var itemsString = string.Join(",", itemParts);

        // Format: "INVENTORY|StationID|Category:Subtype:Amount,..."
        var response = $"INVENTORY|{stationId}|{itemsString}";
        IGC.SendBroadcastMessage("STATION_INVENTORY", response);

        LogStatus($"  Sent inventory ({inventoryTotals.Count} item types)");
    }
}

string GetItemCategory(MyItemType itemType) {
    var typeId = itemType.TypeId;
    if (typeId == "MyObjectBuilder_Ore") return "Ore";
    if (typeId == "MyObjectBuilder_Ingot") return "Ingot";
    if (typeId == "MyObjectBuilder_Component") return "Component";
    if (typeId == "MyObjectBuilder_AmmoMagazine") return "AmmoMagazine";
    return null;
}

void CheckAvailability() {
    cachedAvailability.Clear();
    var storageContainers = GetContainers(storageContainerTag);

    foreach (var order in activeOrders) {
        var destStation = order.Key;
        var items = order.Value;

        cachedAvailability[destStation] = new Dictionary<string, MyFixedPoint>();

        foreach (var item in items) {
            var itemType = ParseItemType(item.Key);
            if (itemType == null) continue;

            MyFixedPoint available = 0;
            foreach (var storage in storageContainers) {
                var inventory = storage.GetInventory(0);
                var inventoryItems = new List<MyInventoryItem>();
                inventory.GetItems(inventoryItems);

                foreach (var invItem in inventoryItems) {
                    if (invItem.Type == itemType.Value) {
                        available += invItem.Amount;
                    }
                }
            }

            cachedAvailability[destStation][item.Key] = available;
        }
    }
}

void BroadcastStationStatus() {
    // Broadcast station availability
    // Format: "STATUS|StationID|Ready"
    var message = $"STATUS|{stationId}|Ready";
    IGC.SendBroadcastMessage("STATION_STATUS", message);
}

void ProcessDroneAvailability() {
    // Process drone availability announcements
    while (droneAvailableListener.HasPendingMessage) {
        var message = droneAvailableListener.AcceptMessage();
        var data = message.Data.ToString();

        // Format: "AVAILABLE|DroneID|HomeStation|CurrentStation|Tags"
        var parts = data.Split('|');

        if (parts.Length < 3 || parts[0] != "AVAILABLE") {
            continue;
        }

        var droneId = parts[1];
        var droneHomeStation = parts[2];

        // Only track drones whose home is THIS station
        if (droneHomeStation != stationId) {
            continue;
        }

        // Update drone availability with current timestamp
        availableDrones[droneId] = DateTime.Now;

        // Store current station for remote drone recovery (parts[3])
        if (parts.Length >= 4 && !string.IsNullOrEmpty(parts[3])) {
            droneCurrentStation[droneId] = parts[3];
        }

        // Parse drone tags if provided (parts[4])
        if (parts.Length >= 5 && !string.IsNullOrEmpty(parts[4])) {
            var tags = new List<string>();
            var tagArray = parts[4].Split(',');
            foreach (var tag in tagArray) {
                var trimmedTag = tag.Trim().ToUpper(); // Normalize to uppercase
                if (!string.IsNullOrEmpty(trimmedTag)) {
                    tags.Add(trimmedTag);
                }
            }
            droneTags[droneId] = tags;
        } else {
            // No tags specified - drone can go anywhere (empty tag list)
            droneTags[droneId] = new List<string>();
        }
    }

    // Process cargo claimed confirmations
    while (cargoClaimedListener.HasPendingMessage) {
        var message = cargoClaimedListener.AcceptMessage();
        var data = message.Data.ToString();

        // Format: "CLAIMED|DroneID|Destination"
        var parts = data.Split('|');

        if (parts.Length < 3 || parts[0] != "CLAIMED") {
            continue;
        }

        var droneId = parts[1];
        var destination = parts[2];

        // Remove drone from available list (it's now on a mission)
        if (availableDrones.ContainsKey(droneId)) {
            availableDrones.Remove(droneId);
        }

        // Clear from pending dispatch (race condition protection)
        pendingDispatch.Remove(destination);
        pendingDispatchTime.Remove(destination);

        // Mark order as assigned and track which drone has it
        if (!assignedOrders.Contains(destination)) {
            assignedOrders.Add(destination);
        }
        orderAssignments[destination] = droneId;
        missionStates[destination] = "ASSIGNED"; // Initial state
    }

    // Process drone departure messages — transition mission states
    while (droneDepartedListener.HasPendingMessage) {
        var message = droneDepartedListener.AcceptMessage();
        var data = message.Data.ToString();

        var parts = data.Split('|');
        if (parts.Length < 3 || parts[0] != "DEPARTED") continue;

        var droneId = parts[1];
        var destination = parts[2];

        // Find which order this drone is assigned to
        string assignedDest = "";
        foreach (var assignment in orderAssignments) {
            if (assignment.Value == droneId) {
                assignedDest = assignment.Key;
                break;
            }
        }

        if (string.IsNullOrEmpty(assignedDest)) continue;

        // Case 1: First DEPARTED — drone left supply station heading to destination
        if (destination == assignedDest) {
            missionStates[assignedDest] = "OUTBOUND";
            // Order fulfilled from supply perspective — clean up order data
            activeOrders.Remove(assignedDest);
            assignedOrders.Remove(assignedDest);
            cachedAvailability.Remove(assignedDest);
            pendingDispatch.Remove(assignedDest);
            pendingDispatchTime.Remove(assignedDest);
            // KEEP orderAssignments and missionStates for lifecycle tracking
        }
        // Case 2: Second DEPARTED — drone left order station heading home
        else if (orderAssignments.ContainsKey(assignedDest)) {
            missionStates[assignedDest] = "RETURNING";
        }
    }

    // Process mission completion messages — drone returned home
    while (missionCompleteListener.HasPendingMessage) {
        var message = missionCompleteListener.AcceptMessage();
        var data = message.Data.ToString();

        var parts = data.Split('|');
        if (parts.Length < 2 || parts[0] != "COMPLETE") continue;

        var droneId = parts[1];

        // Find which destination this drone was assigned to
        string completedDestination = "";
        foreach (var assignment in orderAssignments) {
            if (assignment.Value == droneId) {
                completedDestination = assignment.Key;
                break;
            }
        }

        if (!string.IsNullOrEmpty(completedDestination)) {
            missionStates[completedDestination] = "COMPLETE";
            missionCompletionTime[completedDestination] = DateTime.Now;
        }
    }

    // Remove stale pending dispatches (no CLAIMED received within 30 seconds)
    var stalePending = new List<string>();
    foreach (var pending in pendingDispatchTime) {
        var timeSinceSent = DateTime.Now - pending.Value;
        if (timeSinceSent.TotalSeconds > 30) {
            stalePending.Add(pending.Key);
        }
    }

    foreach (var stale in stalePending) {
        pendingDispatch.Remove(stale);
        pendingDispatchTime.Remove(stale);
        LogStatus($"WARNING: Dispatch to {stale} timed out (no CLAIMED)");
    }

    // Remove stale drones (haven't checked in for 5 seconds)
    var staleDrones = new List<string>();
    foreach (var drone in availableDrones) {
        var timeSinceLastSeen = DateTime.Now - drone.Value;
        if (timeSinceLastSeen.TotalSeconds > 5) {
            staleDrones.Add(drone.Key);
        }
    }

    foreach (var staleDrone in staleDrones) {
        availableDrones.Remove(staleDrone);
        droneTags.Remove(staleDrone); // Also remove tag info
        droneCurrentStation.Remove(staleDrone); // Also remove current station info
    }
}

void DispatchMissions() {
    // Try to dispatch orders to available drones
    if (availableDrones.Count == 0) {
        return; // No drones available
    }

    foreach (var order in activeOrders.ToList()) {
        var destination = order.Key;

        // Skip if already assigned OR pending dispatch confirmation
        if (assignedOrders.Contains(destination) || pendingDispatch.Contains(destination)) {
            continue;
        }

        // Build manifest from cached availability
        var availability = cachedAvailability.ContainsKey(destination)
            ? cachedAvailability[destination]
            : new Dictionary<string, MyFixedPoint>();

        // Check if this is a pickup-only request (empty order)
        var isPickupRequest = order.Value.Count == 0;

        var manifestParts = new List<string>();
        foreach (var item in order.Value) {
            MyFixedPoint available = availability.ContainsKey(item.Key) ? availability[item.Key] : 0;
            var toLoad = (int)MyFixedPoint.Min(available, item.Value);
            if (toLoad > 0) {
                manifestParts.Add($"{item.Key}:{toLoad}");
            }
        }

        // If no items to ship AND not a pickup request, notify OrderStation and cancel order
        if (manifestParts.Count == 0 && !isPickupRequest) {
            // Build list of unavailable items
            var unavailableParts = new List<string>();
            foreach (var item in order.Value) {
                unavailableParts.Add($"{item.Key}:0");
            }
            var unavailableString = string.Join(",", unavailableParts);

            // Notify OrderStation that stock is unavailable
            var unavailableMsg = $"STOCK_UNAVAILABLE|{stationId}|{destination}|{unavailableString}";
            IGC.SendBroadcastMessage("STOCK_UNAVAILABLE", unavailableMsg);

            LogStatus($"\n>>> Order for {destination}: NO STOCK AVAILABLE");
            LogStatus($"    Notified OrderStation and cancelled order");

            // Remove this order since we can't fulfill it
            activeOrders.Remove(destination);
            cachedAvailability.Remove(destination);
            continue;
        }

        // Find an available drone that meets destination requirements
        if (availableDrones.Count == 0) {
            break; // No more drones available
        }

        // Find a compatible drone (prefer local drones over remote ones)
        var compatibleDroneId = "";
        var incompatibleReason = "";

        // Sort drones: local first (at this station), then remote
        var sortedDrones = availableDrones
            .OrderByDescending(d => droneCurrentStation.ContainsKey(d.Key)
                && droneCurrentStation[d.Key] == stationId)
            .ToList();

        foreach (var drone in sortedDrones) {
            var droneId = drone.Key;

            if (CanDroneDeliverTo(droneId, destination, out incompatibleReason)) {
                compatibleDroneId = droneId;
                break;
            }
        }

        if (string.IsNullOrEmpty(compatibleDroneId)) {
            if (!string.IsNullOrEmpty(incompatibleReason)) {
                LogStatus($"\nOrder for {destination}: Waiting for compatible drone");
                LogStatus($"  Reason: {incompatibleReason}");
            }
            continue;
        }

        // Assign mission with manifest
        var manifestString = string.Join(",", manifestParts);
        var message = $"ASSIGNED|{compatibleDroneId}|{destination}|{manifestString}";
        IGC.SendBroadcastMessage("CARGO_ASSIGNED", message);

        // Mark as pending dispatch immediately to prevent race condition
        pendingDispatch.Add(destination);
        pendingDispatchTime[destination] = DateTime.Now;

        if (isPickupRequest) {
            LogStatus($"\n>>> Dispatched {compatibleDroneId} for PICKUP at {destination}");
            LogStatus($"    (Export collection only - no cargo to deliver)");
        } else {
            LogStatus($"\n>>> Dispatched {compatibleDroneId} to deliver to {destination}");
            LogStatus($"    Manifest: {manifestString}");
        }

        // Don't remove from availableDrones yet - wait for CLAIMED confirmation
    }
}

bool CanDroneDeliverTo(string droneId, string destination, out string reason) {
    reason = "";

    // Check if destination has requirements
    if (!destinationRequirements.ContainsKey(destination)) {
        return true; // No requirements = any drone can go
    }

    // Check if we have tag info for this drone
    if (!droneTags.ContainsKey(droneId)) {
        return true; // No tag info = assume compatible (backward compatibility)
    }

    var requiredTags = destinationRequirements[destination];
    var availableTags = droneTags[droneId];

    // Check if drone has ALL required tags
    var missingTags = new List<string>();
    foreach (var requiredTag in requiredTags) {
        if (!availableTags.Contains(requiredTag)) {
            missingTags.Add(requiredTag);
        }
    }

    if (missingTags.Count > 0) {
        reason = $"Missing tags: {string.Join(",", missingTags)}";
        return false;
    }

    return true; // Has all required tags
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

List<IMyCargoContainer> GetContainers(params string[] tags) {
    var containers = new List<IMyCargoContainer>();

    GridTerminalSystem.GetBlocksOfType(containers, c => {
        if (c.CubeGrid != Me.CubeGrid) return false;

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

void LogStatus(string message) {
    statusLog.AppendLine(message);
}

void CleanupCompletedMissions() {
    var toRemove = new List<string>();
    foreach (var completion in missionCompletionTime) {
        var elapsed = (DateTime.Now - completion.Value).TotalSeconds;
        if (elapsed > completedMissionTimeout) {
            toRemove.Add(completion.Key);
        }
    }
    foreach (var dest in toRemove) {
        missionCompletionTime.Remove(dest);
        orderAssignments.Remove(dest);
        missionStates.Remove(dest);
    }
}

string FormatElapsed(TimeSpan elapsed) {
    if (elapsed.TotalMinutes < 1) return $"{(int)elapsed.TotalSeconds}s";
    if (elapsed.TotalHours < 1) return $"{(int)elapsed.TotalMinutes}m";
    return $"{(int)elapsed.TotalHours}h {elapsed.Minutes}m";
}

void UpdateDisplays() {
    // Echo to PB screen
    Echo(statusLog.ToString());

    // Legacy text mode: only when no sprite display groups configured
    if (_displayGroups.Count == 0) {
        var lcds = new List<IMyTextPanel>();
        GridTerminalSystem.GetBlocksOfType(lcds, lcd =>
            lcd.CubeGrid == Me.CubeGrid &&
            (lcd.CustomName.Contains(statusLcdTag) || lcd.CustomData.Contains(statusLcdTag))
        );

        foreach (var lcd in lcds) {
            lcd.ContentType = ContentType.TEXT_AND_IMAGE;
            lcd.WriteText(statusLog.ToString());
        }
    }
}

// ============== DISPLAY RENDERING ==============

string FormatAmount(MyFixedPoint amount) {
    int value = (int)amount;

    if (value >= 1000000) {
        double millions = value / 1000000.0;
        return millions % 1 == 0
            ? $"{(int)millions}m"
            : $"{millions:0.#}m";
    }

    if (value >= 1000) {
        double thousands = value / 1000.0;
        return thousands % 1 == 0
            ? $"{(int)thousands}k"
            : $"{thousands:0.#}k";
    }

    return value.ToString();
}

void PrepareDisplay(string[] sections) {
    DLBanner($"SUPPLY STATION v{VERSION}", stationId);
    foreach (var sec in sections) {
        switch (sec.Trim()) {
            case "Orders":   PrepareOrdersSection(); break;
            case "Missions": PrepareMissionsSection(); break;
            case "Drones":   PrepareDronesSection(); break;
            case "Status":   PrepareStatusSection(); break;
        }
    }
}

void PrepareOrdersSection() {
    DLSep();
    DL("-- ORDERS --", _theme.LBL);

    bool hasActiveOrders = false;
    foreach (var order in activeOrders) {
        var destination = order.Key;
        // Skip orders that have a drone assigned (shown in MISSIONS)
        if (assignedOrders.Contains(destination) || orderAssignments.ContainsKey(destination)) {
            continue;
        }
        hasActiveOrders = true;

        Color statusCol;
        string statusText;
        if (pendingDispatch.Contains(destination)) {
            statusCol = _theme.WARN;
            statusText = "Dispatching";
        } else {
            statusCol = _theme.DIM;
            statusText = "Waiting for drone";
        }
        DLDot("  " + destination, statusCol, statusCol, statusText);

        // Show item availability
        if (cachedAvailability.ContainsKey(destination)) {
            var avail = cachedAvailability[destination];
            foreach (var item in order.Value) {
                MyFixedPoint available = avail.ContainsKey(item.Key) ? avail[item.Key] : (MyFixedPoint)0;
                var requested = item.Value;
                var subtype = item.Key.Contains(".") ? item.Key.Split('.')[1] : item.Key;

                if (available >= requested) {
                    DLR("    " + subtype, _theme.GOOD, $"{FormatAmount(available)}/{FormatAmount(requested)}");
                } else if (available > 0) {
                    DLR("    " + subtype, _theme.WARN, $"{FormatAmount(available)}/{FormatAmount(requested)}");
                } else {
                    DLR("    " + subtype, _theme.CRIT, $"0/{FormatAmount(requested)}");
                }
            }
        }
    }
    if (!hasActiveOrders) {
        DL("  No active orders", _theme.DIM);
    }
}

void PrepareMissionsSection() {
    DLSep();
    DL("-- MISSIONS --", _theme.LBL);

    if (orderAssignments.Count == 0) {
        DL("  No active missions", _theme.DIM);
        return;
    }

    foreach (var assignment in orderAssignments) {
        var destination = assignment.Key;
        var droneId = assignment.Value;
        var state = missionStates.ContainsKey(destination) ? missionStates[destination] : "ASSIGNED";

        Color dotCol;
        string statusText;

        if (state == "ASSIGNED") {
            dotCol = _theme.WARN;
            statusText = "Loading";
        } else if (state == "OUTBOUND") {
            dotCol = _theme.HDR;
            statusText = "En Route";
        } else if (state == "RETURNING") {
            dotCol = _theme.VAL;
            statusText = "Returning";
        } else if (state == "COMPLETE") {
            dotCol = _theme.GOOD;
            var sinceComplete = DateTime.Now - missionCompletionTime[destination];
            var remaining = completedMissionTimeout - (int)sinceComplete.TotalSeconds;
            statusText = $"Complete ({FormatElapsed(TimeSpan.FromSeconds(remaining))})";
        } else {
            dotCol = _theme.DIM;
            statusText = state;
        }

        DLDot("  " + destination, dotCol, dotCol, statusText);
        DLR("    -> " + droneId, _theme.DIM, "");
    }
}

void PrepareDronesSection() {
    DLSep();
    DL("-- DRONES --", _theme.LBL);

    if (availableDrones.Count == 0) {
        DL("  No drones available", _theme.DIM);
        return;
    }

    foreach (var drone in availableDrones) {
        var droneId = drone.Key;
        string tagsStr = "";
        if (droneTags.ContainsKey(droneId) && droneTags[droneId].Count > 0) {
            tagsStr = string.Join(",", droneTags[droneId]);
        }
        DLDot("  " + droneId, _theme.GOOD, _theme.GOOD, tagsStr);
    }
}

void PrepareStatusSection() {
    DLSep();
    DL("-- SYSTEM --", _theme.LBL);
    DLR("  Station", _theme.VAL, stationId);
    DLR("  Storage tag", _theme.DIM, storageContainerTag);
    DLR("  Mission timeout", _theme.DIM, completedMissionTimeout + "s");
    DLR("  Active orders", _theme.DIM, activeOrders.Count.ToString());
    DLR("  Drones available", _theme.DIM, availableDrones.Count.ToString());

    if (destinationRequirements.Count > 0) {
        DLSep();
        DL("-- REQUIREMENTS --", _theme.LBL);
        foreach (var req in destinationRequirements) {
            var tagsStr = string.Join(",", req.Value);
            DLR("  " + req.Key, _theme.VAL, tagsStr);
        }
    }
}

// Exact tag match: [TAG] must appear as a complete token, not inside a larger tag.
// Matches: "LCD [STATUS]", "[STATUS]:1", "[STATUS] Panel"
// Rejects: "[SUPPLYSTATUS]", "StatusLcdTag=[STATUS]", "[STATUS]Backup"
static bool HasTag(string text, string tag) {
    int pos = text.IndexOf(tag);
    while (pos >= 0) {
        int end = pos + tag.Length;
        bool validEnd = end >= text.Length || !char.IsLetterOrDigit(text[end]);
        bool validStart = pos == 0 || !char.IsLetterOrDigit(text[pos - 1]);
        if (validStart && validEnd) return true;
        pos = text.IndexOf(tag, pos + 1);
    }
    return false;
}

// Line-by-line exact tag match for CustomData.
// Tag must be the entire line, or followed by :N (surface index).
static bool CustomDataHasTag(string customData, string tag) {
    foreach (var line in customData.Split('\n')) {
        var t = line.Trim();
        if (t == tag) return true;
        if (t.Length > tag.Length && t.StartsWith(tag) && t[tag.Length] == ':') return true;
    }
    return false;
}

List<IMyTextSurface> GetDisplaySurfaces(string tag) {
    var entries = new List<KeyValuePair<string, IMyTextSurface>>();

    // Standard LCD panels - exact tag match in name or CustomData
    var panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, p =>
        p.CubeGrid == Me.CubeGrid &&
        (HasTag(p.CustomName, tag) || CustomDataHasTag(p.CustomData, tag)));
    foreach (var p in panels) {
        entries.Add(new KeyValuePair<string, IMyTextSurface>(p.CustomName, p));
    }

    // Multi-surface providers (cockpits, control seats, etc.) - exact tag match in CustomData
    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(allBlocks, b =>
        b.CubeGrid == Me.CubeGrid &&
        !(b is IMyTextPanel) &&
        b is IMyTextSurfaceProvider &&
        CustomDataHasTag(b.CustomData, tag));
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

int ParseSurfaceIndex(string customData, string tag) {
    foreach (var line in customData.Split('\n')) {
        var trimmed = line.Trim();
        if (trimmed == tag) return 0;
        if (trimmed.Length > tag.Length && trimmed.StartsWith(tag) && trimmed[tag.Length] == ':') {
            int idx;
            if (int.TryParse(trimmed.Substring(tag.Length + 1).Trim(), out idx)) {
                return idx;
            }
            return 0;
        }
    }
    return 0;
}

void ScanDisplayPanels() {
    foreach (var grp in _displayGroups) {
        grp.Panels.Clear();
        var surfaces = GetDisplaySurfaces(grp.Tag);
        foreach (var surf in surfaces) {
            surf.ContentType = ContentType.SCRIPT;
            surf.Script = "";
            surf.ScriptBackgroundColor = _theme.BG;
            surf.Font = "Monospace";
            surf.FontSize = 0.5f;
            grp.Panels.Add(surf);
        }
    }
}

void RenderDisplays() {
    if (_displayGroups.Count == 0) return;
    int target = _renderGroupIdx % _displayGroups.Count;
    var grp = _displayGroups[target];
    if (grp.Panels.Count > 0) {
        _displayLines.Clear();
        PrepareDisplay(grp.Sections);
        DrawLinesToSurfaces(grp.Panels, _displayLines);
    }
    _renderGroupIdx++;
}

void DrawLinesToSurfaces(List<IMyTextSurface> panels, List<DisplayLine> lines) {
    if (panels.Count == 0) return;
    if (lines.Count == 0) {
        foreach (var p in panels) {
            p.ContentType = ContentType.SCRIPT;
            p.Script = "";
            using (p.DrawFrame()) { }
        }
        return;
    }

    int total = lines.Count;
    int lineIdx = 0;

    for (int p = 0; p < panels.Count; p++) {
        var surface = panels[p];
        surface.ContentType = ContentType.SCRIPT;
        surface.Script = "";

        float fontSize = surface.FontSize;
        float lineH = fontSize * 34.4f;
        float dotR = lineH * 0.38f;
        float sepH = Math.Max(1f, lineH * 0.06f);
        var surfSize = surface.SurfaceSize;
        var vp = (surface.TextureSize - surfSize) / 2f;
        float pad = 8f;
        float leftX = vp.X + pad;
        float topY = vp.Y + pad;

        // Distribute lines across remaining panels
        int remaining = total - lineIdx;
        int panelsLeft = panels.Count - p;
        int perPanel = panelsLeft == 1 ? remaining : (int)Math.Ceiling((double)remaining / panelsLeft);
        int end = Math.Min(lineIdx + perPanel, total);

        // Smart split: avoid cutting mid-section
        if (end < total) {
            int check = end - 1;
            while (check > lineIdx && lines[check].IsSep) check--;
            if (check >= lineIdx && lines[check].Col == _theme.LBL) {
                int afterHeader = check + 1;
                while (afterHeader < total && lines[afterHeader].IsSep) afterHeader++;
                if (afterHeader < total && lines[afterHeader].Col != _theme.LBL) end = check;
            }
        }

        // Repeat section header if this panel starts mid-section
        bool repeatHeader = false;
        DisplayLine sectionHeader = new DisplayLine();
        if (p > 0 && lineIdx < total && lines[lineIdx].Col != _theme.LBL) {
            for (int i = lineIdx - 1; i >= 0; i--) {
                if (lines[i].Col == _theme.LBL) { sectionHeader = lines[i]; repeatHeader = true; break; }
            }
            if (repeatHeader) {
                int firstReal = lineIdx;
                while (firstReal < end && lines[firstReal].IsSep) firstReal++;
                if (firstReal >= end || lines[firstReal].Col == _theme.LBL) repeatHeader = false;
            }
        }

        _renderTick++;
        int sA = 254 + _renderTick / 5 % 2;

        float cx = vp.X + surfSize.X * 0.5f;
        float rightX = vp.X + surfSize.X - pad;
        float maxY = vp.Y + surfSize.Y - pad;

        var frame = surface.DrawFrame();
        float contentY = topY;

        // Draw repeated section header
        if (repeatHeader) {
            frame.Add(Rect(new Vector2(cx, contentY + lineH * 0.5f),
                new Vector2(surfSize.X, lineH),
                SaltColor(new Color(sectionHeader.Col.R, sectionHeader.Col.G, sectionHeader.Col.B, 210), sA)));
            frame.Add(Txt(sectionHeader.Text, new Vector2(leftX, contentY), SaltColor(new Color(20, 15, 5), sA), fontSize));
            contentY += lineH;
        }

        for (int l = lineIdx; l < end; l++) {
            float y = contentY + (l - lineIdx) * lineH;
            if (repeatHeader) y += lineH;
            if (y > maxY) break;
            var line = lines[l];

            // Banner
            if (line.IsBanner) {
                float bh = lineH * 2.0f;
                frame.Add(Rect(new Vector2(cx, y + bh * 0.5f), new Vector2(surfSize.X, bh), SaltColor(new Color(8, 25, 38), sA)));
                frame.Add(Rect(new Vector2(cx, y + 1f), new Vector2(surfSize.X, 2f), SaltColor(_theme.HDR, sA)));
                frame.Add(Rect(new Vector2(cx, y + bh - 1f), new Vector2(surfSize.X, 2f), SaltColor(_theme.HDR, sA)));
                frame.Add(Rect(new Vector2(leftX * 0.5f, y + bh * 0.5f), new Vector2(leftX, bh), SaltColor(_theme.HDR, sA)));
                frame.Add(Rect(new Vector2(vp.X + surfSize.X - leftX * 0.5f, y + bh * 0.5f), new Vector2(leftX, bh), SaltColor(_theme.HDR, sA)));
                frame.Add(Txt(line.Text, new Vector2(cx, y + lineH * 0.1f), SaltColor(Color.White, sA), fontSize * 1.1f, TextAlignment.CENTER));
                if (line.RightText.Length > 0)
                    frame.Add(Txt(line.RightText, new Vector2(rightX - 8f, y + lineH * 0.1f),
                        SaltColor(_theme.DIM, sA), fontSize * 0.85f, TextAlignment.RIGHT));
                continue;
            }

            // Separator
            if (line.IsSep) {
                frame.Add(Rect(new Vector2(cx, y + lineH * 0.5f),
                    new Vector2(surfSize.X - 16f, sepH),
                    SaltColor(new Color(55, 55, 65), sA)));
                continue;
            }

            if (line.Text.Length == 0) continue;

            // Section header (LBL color = colored bg with dark text)
            bool isHeader = line.Col == _theme.LBL;
            if (isHeader) {
                frame.Add(Rect(new Vector2(cx, y + lineH * 0.5f),
                    new Vector2(surfSize.X, lineH),
                    SaltColor(new Color(line.Col.R, line.Col.G, line.Col.B, 210), sA)));
            }

            float textX = leftX;

            // Status dot
            if (line.DotCol.A > 0) {
                frame.Add(new MySprite(SpriteType.TEXTURE, "Circle",
                    new Vector2(leftX + dotR, y + lineH * 0.5f),
                    new Vector2(dotR * 2f, dotR * 2f),
                    SaltColor(line.DotCol, sA)));
                textX = leftX + dotR * 2f + 3f;
            }

            // Main text
            Color textCol = isHeader ? new Color(20, 15, 5) : line.Col;
            frame.Add(Txt(line.Text, new Vector2(textX, y), SaltColor(textCol, sA), fontSize));

            // Right-aligned text
            if (line.RightText.Length > 0 && !isHeader) {
                frame.Add(Txt(line.RightText, new Vector2(rightX, y),
                    SaltColor(line.Col, sA), fontSize, TextAlignment.RIGHT));
            }
        }

        frame.Dispose();
        lineIdx = end;

        // Clear remaining panels if all content rendered
        if (lineIdx >= total) {
            for (int q = p + 1; q < panels.Count; q++) {
                panels[q].ContentType = ContentType.SCRIPT;
                panels[q].Script = "";
                using (panels[q].DrawFrame()) { }
            }
            break;
        }
    }
}

string GetDefaultConfig() {
    return @"StationID=CHANGE_ME
StorageTag=[STOCK]
; Seconds before completed missions clear from display (default 300 = 5 min)
CompletedMissionTimeout=300

---DESTINATION-REQUIREMENTS---
; Require drone tags for destinations
; Example: RemoteBase=LONG_RANGE

---DISPLAY---
; Theme: default, military, highcontrast, minimal, pm
Theme=pm
; Tag=Sections  (Sections: Orders, Missions, Drones, Status)
STATUS=Orders,Missions,Drones,Status
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
    // Check if StationID is using grid name fallback
    if (!stationIdFromConfig) {
        validator.Info("Config", $"StationID using grid name: {stationId}");
    }

    // Check for empty destination requirements (just informational)
    if (destinationRequirements.Count == 0) {
        validator.Info("Config", "No destination requirements configured");
    }
}

void ValidateBlocks() {
    // CRITICAL: Check for antenna (required for IGC)
    var antennas = new List<IMyRadioAntenna>();
    GridTerminalSystem.GetBlocksOfType(antennas, a => a.CubeGrid == Me.CubeGrid);
    if (antennas.Count == 0) {
        validator.Critical("Blocks", "No antenna found - IGC requires antenna");
    }

    // CRITICAL: Check for storage containers
    var storageContainers = GetContainers(storageContainerTag);
    if (storageContainers.Count == 0) {
        validator.Critical("Blocks", $"No storage containers with tag {storageContainerTag} - nothing to supply");
    }

    // INFO: Check for display panels
    bool hasAnyPanels = false;
    foreach (var grp in _displayGroups) {
        if (grp.Panels.Count > 0) { hasAnyPanels = true; break; }
    }
    if (!hasAnyPanels && _displayGroups.Count > 0) {
        validator.Info("Blocks", "No LCDs found matching display tags (optional)");
    }
}

#endregion // SupplyStation
