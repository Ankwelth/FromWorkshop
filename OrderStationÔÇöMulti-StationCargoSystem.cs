#region OrderStation
const string VERSION = "2.9.0";

/*
 * OrderStation Script v2.9.0
 * Protocol: v2 (compatible with SupplyStation v2.x, CargoShip v2.x)
 *
 * Version History:
 * 2.9.0 - Add Comms display section for laser antenna status; fix PM theme banner
 * 2.8.0 - Sprite display, themes, ---DISPLAY--- config, auto-refresh inventory
 * 2.7.0 - Auto-reload CustomData on order command, preserve in-progress orders
 * 2.6.0 - Order lifecycle display (Loading/InTransit/Delivered), auto-clear
 * 2.5.0 - Cockpit/multi-screen support, surface index via [TAG]:N
 * 2.4.0 - Startup validation, default config template, VALIDATE command
 * 2.3.1 - Improved inventory number formatting (1k, 1.1m)
 * 2.3.0 - Inventory display, InventorySourceStation config
 * 2.2.0 - STOCK_UNAVAILABLE handling
 * 2.1.1 - Fixed stale LCD data on reload, Active/Pending sections
 * 2.1.0 - Pickup command, fixed stale display on config change
 * 2.0.0 - Protocol v2: removed AllowPartial, added sent-amount tracking
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

// ============== DISPLAY SYSTEM ==============
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
float _invRefreshTimer = 0f;
float _invRefreshSecs = 120f;

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
// ============== END DISPLAY SYSTEM ==============

// Validation
StartupValidator validator = new StartupValidator();
bool validationFailed = false;

// Configuration
string stationId = "";
string statusLcdTag = "[STATUS]";
string inventoryLcdTag = "[INVENTORY]";
string inventorySourceStation = "";
HashSet<string> inventoryExclude = new HashSet<string>();
Dictionary<string, Dictionary<string, MyFixedPoint>> orders = new Dictionary<string, Dictionary<string, MyFixedPoint>>();
Dictionary<string, MyFixedPoint> exports = new Dictionary<string, MyFixedPoint>();
Dictionary<string, DateTime> orderTimestamps = new Dictionary<string, DateTime>();
Dictionary<string, string> orderTransitStatus = new Dictionary<string, string>(); // Station -> DroneID (drone assigned, loading at supply)
Dictionary<string, Dictionary<string, MyFixedPoint>> orderSentAmounts = new Dictionary<string, Dictionary<string, MyFixedPoint>>();
Dictionary<string, bool> lastShipmentCargoFull = new Dictionary<string, bool>();
Dictionary<string, DateTime> orderUnavailableTime = new Dictionary<string, DateTime>();
Dictionary<string, string> dronesInTransit = new Dictionary<string, string>(); // DroneID -> SourceStation (drone flying TO this station)
Dictionary<string, DateTime> orderCompletionTime = new Dictionary<string, DateTime>(); // Station -> delivery confirmation time
int completedOrderTimeout = 300; // seconds before delivered orders clear from display
string commsTag = "[COMMS]";
string commsGroup = "";
List<IMyLaserAntenna> _commsAntennas = new List<IMyLaserAntenna>();

enum OrderStatus { Pending, Sent, Loading, InTransit, Delivered, Unavailable }

// Inventory display cache
Dictionary<string, MyFixedPoint> inventoryCache = new Dictionary<string, MyFixedPoint>();
string inventoryCacheSource = "";
DateTime lastInventoryUpdate;

string _lastCustomData = null;
StringBuilder statusLog = new StringBuilder();
IMyBroadcastListener assignmentListener;
IMyBroadcastListener departureListener;
IMyBroadcastListener stockUnavailableListener;
IMyBroadcastListener inventoryListener;

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

    LoadConfig();
    ScanDisplayPanels();
    ScanCommsAntennas();
    _lastCustomData = Me.CustomData;

    // Run startup validation
    RunValidation();

    // Setup listener for cargo assignments (to track when orders are assigned to drones)
    assignmentListener = IGC.RegisterBroadcastListener("CARGO_ASSIGNED");
    assignmentListener.SetMessageCallback("CARGO_ASSIGNED");

    // Setup listener for drone departures (to track when drones leave for delivery)
    departureListener = IGC.RegisterBroadcastListener("DRONE_DEPARTED");
    departureListener.SetMessageCallback("DRONE_DEPARTED");

    // Setup listener for stock unavailable notifications
    stockUnavailableListener = IGC.RegisterBroadcastListener("STOCK_UNAVAILABLE");
    stockUnavailableListener.SetMessageCallback("STOCK_UNAVAILABLE");

    // Setup listener for inventory responses
    inventoryListener = IGC.RegisterBroadcastListener("STATION_INVENTORY");
    inventoryListener.SetMessageCallback("STATION_INVENTORY");

    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

string GetDefaultConfig() {
    return @"StationID=CHANGE_ME
InventorySourceStation=
InventoryExclude=
; Seconds before delivered orders clear from display (default 300 = 5 min)
CompletedOrderTimeout=300
; Tag on laser antenna block names to show in Comms display section
CommsTag=[COMMS]
; Optional block group name for laser antennas (empty = disabled)
CommsGroup=
; Legacy display tags (only used if ---DISPLAY--- section is missing)
; StatusLcdTag=[STATUS]
; InventoryLcdTag=[INVENTORY]

---ORDERS---
; Configure orders from supply stations
; [SupplyStationName]
; Ingot.Iron:1000
; Component.SteelPlate:500

---EXPORTS---
; Items this station exports (for pickup)
; Ore.Iron:5000
; Ore.Stone

---DISPLAY---
; Theme: default, military, highcontrast, minimal, pm
Theme=pm
; Auto-refresh inventory from supply station (seconds, 0 = disabled)
InventoryRefreshSecs=120
; Tag=Sections  (Sections: Ore, Ingot, Component, Ammo, Orders, Status, Comms)
; Tag names are written WITHOUT brackets here; brackets are added automatically
INVENTORY=Ore,Ingot,Component,Ammo
STATUS=Orders,Status,Comms
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
    // INFO: StationID using grid name fallback
    if (stationId == Me.CubeGrid.CustomName) {
        validator.Info("Config", $"StationID using grid name: {stationId}");
    }

    // WARNING: No exports configured
    if (exports.Count == 0) {
        validator.Warning("Config", "No ---EXPORTS--- configured - station has nothing to export");
    }

    // Validate item format in orders
    foreach (var order in orders) {
        foreach (var item in order.Value) {
            if (!ValidateItemFormat(item.Key)) {
                validator.Warning("Config", $"Invalid item format: {item.Key}");
            }
        }
    }

    // Validate export item format
    foreach (var export in exports) {
        if (!ValidateItemFormat(export.Key)) {
            validator.Warning("Config", $"Invalid export format: {export.Key}");
        }
    }
}

void ValidateBlocks() {
    // CRITICAL: Antenna required for IGC
    var antennas = new List<IMyRadioAntenna>();
    GridTerminalSystem.GetBlocksOfType(antennas, a => a.CubeGrid == Me.CubeGrid);
    if (antennas.Count == 0) {
        validator.Critical("Blocks", "No antenna found - IGC requires antenna");
    }

    // WARNING: Incoming containers
    var incomingContainers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(incomingContainers, c =>
        c.CubeGrid == Me.CubeGrid &&
        !c.CustomName.Contains("[EXCLUDE]") && !c.CustomData.Contains("[EXCLUDE]") &&
        !c.CustomName.Contains("[NO-AUTO]") && !c.CustomData.Contains("[NO-AUTO]") &&
        (c.CustomName.Contains("[INCOMING]") || c.CustomData.Contains("[INCOMING]"))
    );
    if (incomingContainers.Count == 0) {
        validator.Warning("Blocks", "No [INCOMING] containers - nowhere to receive cargo");
    }

    // WARNING: Export containers (only if exports configured)
    if (exports.Count > 0) {
        var exportContainers = new List<IMyCargoContainer>();
        GridTerminalSystem.GetBlocksOfType(exportContainers, c =>
            c.CubeGrid == Me.CubeGrid &&
            !c.CustomName.Contains("[EXCLUDE]") && !c.CustomData.Contains("[EXCLUDE]") &&
            !c.CustomName.Contains("[NO-AUTO]") && !c.CustomData.Contains("[NO-AUTO]") &&
            (c.CustomName.Contains("[EXPORT]") || c.CustomData.Contains("[EXPORT]"))
        );
        if (exportContainers.Count == 0) {
            validator.Warning("Blocks", "Exports configured but no [EXPORT] containers");
        }
    }

    // INFO: Display panels
    bool hasAnyPanels = false;
    bool hasInventorySection = false;
    foreach (var grp in _displayGroups) {
        if (grp.Panels.Count > 0) hasAnyPanels = true;
        foreach (var sec in grp.Sections) {
            var s = sec.Trim();
            if (s == "Ore" || s == "Ingot" || s == "Component" || s == "Ammo")
                hasInventorySection = true;
        }
    }
    if (!hasAnyPanels && _displayGroups.Count > 0) {
        validator.Info("Blocks", "No LCDs found matching display tags (optional)");
    }
    if (hasInventorySection && string.IsNullOrEmpty(inventorySourceStation)) {
        validator.Info("Config", "InventorySourceStation not configured -inventory sections will be empty");
    }
}

bool ValidateItemFormat(string itemString) {
    if (!itemString.Contains(".")) return false;

    var parts = itemString.Split('.');
    if (parts.Length != 2) return false;

    var category = parts[0].Trim();
    var validCategories = new HashSet<string> {
        "Ore", "Ingot", "Component", "AmmoMagazine",
        "PhysicalGunObject", "ConsumableItem"
    };

    return validCategories.Contains(category);
}

public void Save() {}

public void Main(string argument, UpdateType updateSource) {
    statusLog.Clear();

    // Handle VALIDATE argument (always allowed, even when halted)
    if (!string.IsNullOrEmpty(argument) && argument.ToUpper() == "VALIDATE") {
        LoadConfig();
        ScanDisplayPanels();
    ScanCommsAntennas();
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

    // Auto-detect CustomData changes
    if (Me.CustomData != _lastCustomData) {
        LoadConfig();
        ScanDisplayPanels();
    ScanCommsAntennas();
        _lastCustomData = Me.CustomData;
    }

    try {
        LogStatus($"Order Station: {stationId}");
        LogStatus("===================");

        // Check for transit updates (assignments and departures)
        ProcessTransitUpdates();

        // Auto-clear delivered orders after timeout
        CleanupCompletedOrders();

        // Check for stock unavailable notifications
        ProcessStockUnavailable();

        // Check for inventory responses
        ProcessInventoryResponse();

        // Auto-refresh inventory on timer
        if (_invRefreshSecs > 0 && !string.IsNullOrEmpty(inventorySourceStation)) {
            _invRefreshTimer -= 100f / 60f; // Update100 tick time
            if (_invRefreshTimer <= 0f) {
                RequestInventory();
                _invRefreshTimer = _invRefreshSecs;
            }
        }

        // Handle arguments
        if (!string.IsNullOrEmpty(argument)) {
            var cmd = argument.ToLower().Trim();

            if (cmd == "order") {
                SendOrders();
            } else if (cmd == "pickup") {
                SendPickupRequest();
            } else if (cmd == "inventory") {
                RequestInventory();
            } else if (cmd == "reload") {
                LoadConfig();
                ScanDisplayPanels();
    ScanCommsAntennas();
                RunValidation();
            }
        }

        // Display ACTIVE orders (only those that have been sent - have a timestamp)
        LogStatus("\n---ACTIVE ORDERS---");
        bool hasActiveOrders = false;
        foreach (var order in orders) {
            var sourceStation = order.Key;

            // Only show orders that have actually been sent (have a timestamp)
            if (!orderTimestamps.ContainsKey(sourceStation)) {
                continue;
            }

            hasActiveOrders = true;
            LogStatus($"From {sourceStation}:");

            // Order-level status line
            var status = GetOrderStatus(sourceStation);
            switch (status) {
                case OrderStatus.Sent:
                    var elapsed = DateTime.Now - orderTimestamps[sourceStation];
                    LogStatus($"  Status: Awaiting Response ({FormatElapsed(elapsed)} ago)");
                    break;
                case OrderStatus.Loading:
                    LogStatus($"  Status: Loading - Drone: {orderTransitStatus[sourceStation]}");
                    break;
                case OrderStatus.InTransit:
                    string transitDrone = "";
                    foreach (var dt in dronesInTransit) {
                        if (dt.Value == sourceStation) { transitDrone = dt.Key; break; }
                    }
                    LogStatus($"  Status: In Transit - Drone: {transitDrone}");
                    break;
                case OrderStatus.Delivered:
                    var sinceDelivery = DateTime.Now - orderCompletionTime[sourceStation];
                    var remaining = completedOrderTimeout - (int)sinceDelivery.TotalSeconds;
                    LogStatus($"  Status: Delivered (clears in {FormatElapsed(TimeSpan.FromSeconds(remaining))})");
                    break;
                case OrderStatus.Unavailable:
                    LogStatus("  Status: UNAVAILABLE - Items not in stock");
                    break;
            }

            // Item-level details
            var hasSentData = orderSentAmounts.ContainsKey(sourceStation);
            var cargoFull = lastShipmentCargoFull.ContainsKey(sourceStation) && lastShipmentCargoFull[sourceStation];

            foreach (var item in order.Value) {
                var ordered = (int)item.Value;
                var sent = 0;
                if (hasSentData && orderSentAmounts[sourceStation].ContainsKey(item.Key)) {
                    sent = (int)orderSentAmounts[sourceStation][item.Key];
                }

                string itemNote = "";
                if (status == OrderStatus.Unavailable) {
                    itemNote = " [no stock]";
                } else if (hasSentData && sent < ordered && cargoFull) {
                    itemNote = " [drone full]";
                } else if (hasSentData && sent < ordered && sent > 0) {
                    itemNote = " [low stock]";
                }

                LogStatus($"  {item.Key}: {sent}/{ordered}{itemNote}");
            }
        }
        if (!hasActiveOrders) {
            LogStatus("No active orders");
        }

        // Display PENDING orders (configured but not yet sent - no timestamp)
        LogStatus("\n---PENDING ORDERS---");
        bool hasPendingOrders = false;
        foreach (var order in orders) {
            var sourceStation = order.Key;

            // Only show orders that have NOT been sent yet (no timestamp)
            if (orderTimestamps.ContainsKey(sourceStation)) {
                continue;
            }

            hasPendingOrders = true;
            LogStatus($"From {sourceStation}:");
            foreach (var item in order.Value) {
                LogStatus($"  {item.Key}: {(int)item.Value}");
            }
        }
        if (!hasPendingOrders) {
            LogStatus("No pending orders (use ORDER to send)");
        }

        // Validate containers
        LogStatus("\n---CONTAINERS---");
        ValidateContainers();

    } catch (Exception ex) {
        LogStatus($"ERROR: {ex.Message}");
    }

    UpdateDisplays();
    if (_displayGroups.Count == 0) {
        UpdateInventoryDisplays();
    }
    RenderDisplays();
}

void LoadConfig() {
    // Save old orders to detect changes
    var oldOrders = new Dictionary<string, Dictionary<string, MyFixedPoint>>();
    foreach (var order in orders) {
        oldOrders[order.Key] = new Dictionary<string, MyFixedPoint>(order.Value);
    }

    orders.Clear();
    exports.Clear();
    _displayGroups.Clear();

    var config = Me.CustomData;
    var lines = config.Split('\n');
    bool inOrders = false;
    bool inExports = false;
    bool inDisplay = false;
    string currentSupplier = "";

    foreach (var line in lines) {
        var trimmed = line.Trim();

        // Skip comments
        if (trimmed.StartsWith(";")) continue;

        // Parse config settings
        if (trimmed.Contains("=") && !trimmed.StartsWith("---") && !inOrders && !inExports && !inDisplay) {
            var parts = trimmed.Split('=');
            var key = parts[0].Trim();
            var value = parts[1].Trim();

            if (key == "StationID") stationId = value;
            else if (key == "StatusLcdTag") statusLcdTag = value;
            else if (key == "InventoryLcdTag") inventoryLcdTag = value;
            else if (key == "InventorySourceStation") inventorySourceStation = value;
            else if (key == "InventoryExclude") {
                inventoryExclude.Clear();
                var excludeParts = value.Split(',');
                foreach (var part in excludeParts) {
                    var excludeItem = part.Trim().ToLower();
                    if (!string.IsNullOrEmpty(excludeItem)) {
                        inventoryExclude.Add(excludeItem);
                    }
                }
            }
            else if (key == "CompletedOrderTimeout") {
                int timeout;
                if (int.TryParse(value, out timeout) && timeout > 0) {
                    completedOrderTimeout = timeout;
                }
            }
            else if (key == "CommsTag") {
                commsTag = value;
                if (!commsTag.StartsWith("[")) commsTag = "[" + commsTag;
                if (!commsTag.EndsWith("]")) commsTag = commsTag + "]";
            }
            else if (key == "CommsGroup") commsGroup = value;
        }

        // Section markers
        if (trimmed == "---ORDERS---") {
            inOrders = true;
            inExports = false;
            continue;
        }

        if (trimmed == "---EXPORTS---") {
            inExports = true;
            inOrders = false;
            inDisplay = false;
            continue;
        }

        if (trimmed == "---DISPLAY---") {
            inDisplay = true;
            inOrders = false;
            inExports = false;
            continue;
        }

        if (trimmed.StartsWith("---") && trimmed != "---ORDERS---" && trimmed != "---EXPORTS---" && trimmed != "---DISPLAY---") {
            inOrders = false;
            inExports = false;
            inDisplay = false;
        }

        // Parse display settings
        if (inDisplay && trimmed.Contains("=")) {
            var parts = trimmed.Split(new[] { '=' }, 2);
            var key = parts[0].Trim();
            var value = parts[1].Trim();

            if (key == "Theme") _themeName = value.Trim().ToLower();
            else if (key == "InventoryRefreshSecs") {
                float secs;
                if (float.TryParse(value, out secs) && secs >= 0f)
                    _invRefreshSecs = secs;
            }
            else if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value)) {
                // Tag=Section1,Section2 mapping
                _displayGroups.Add(new DisplayGroup {
                    Tag = "[" + key + "]",
                    Sections = value.Split(',')
                });
            }
        }

        // Parse orders - Format: [FromStation] then items
        if (inOrders) {
            if (trimmed.StartsWith("[") && trimmed.EndsWith("]")) {
                currentSupplier = trimmed.Substring(1, trimmed.Length - 2);
                if (!orders.ContainsKey(currentSupplier)) {
                    orders[currentSupplier] = new Dictionary<string, MyFixedPoint>();
                }
            } else if (trimmed.Contains(":") && !string.IsNullOrEmpty(currentSupplier)) {
                var parts = trimmed.Split(':');
                if (parts.Length == 2) {
                    var itemName = parts[0].Trim();
                    int qty;
                    if (int.TryParse(parts[1].Trim(), out qty)) {
                        orders[currentSupplier][itemName] = qty;
                    }
                }
            }
        }

        // Parse exports
        if (inExports && !string.IsNullOrEmpty(trimmed)) {
            // Support two formats:
            // 1. "Item.Type:Amount" - export up to specified amount
            // 2. "Item.Type" - export as much as ship can hold
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
                // No quantity specified - use max value to indicate unlimited
                exports[trimmed] = int.MaxValue;
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

    // Auto-migrate: if no ---DISPLAY--- section found, generate one from legacy tags
    if (_displayGroups.Count == 0) {
        var sb = new StringBuilder();
        sb.AppendLine("\n---DISPLAY---");
        sb.AppendLine("; Theme: default, military, highcontrast, minimal, pm");
        sb.AppendLine("Theme=pm");
        sb.AppendLine("; Auto-refresh inventory (seconds, 0 = disabled)");
        sb.AppendLine("InventoryRefreshSecs=120");
        sb.AppendLine("; Tag=Sections  (Sections: Ore, Ingot, Component, Ammo, Orders, Status)");

        // Map legacy tags to display groups
        string statusTag = string.IsNullOrEmpty(statusLcdTag) ? "[STATUS]" : statusLcdTag;
        string invTag = string.IsNullOrEmpty(inventoryLcdTag) ? "[INVENTORY]" : inventoryLcdTag;

        // Strip brackets for config (they get added back during parsing)
        string statusKey = statusTag.TrimStart('[').TrimEnd(']');
        string invKey = invTag.TrimStart('[').TrimEnd(']');

        sb.AppendLine($"{invKey}=Ore,Ingot,Component,Ammo");
        sb.AppendLine($"{statusKey}=Orders,Status");

        // Append to CustomData
        Me.CustomData += sb.ToString();

        // Create the groups in memory
        _displayGroups.Add(new DisplayGroup {
            Tag = invTag,
            Sections = new[] { "Ore", "Ingot", "Component", "Ammo" }
        });
        _displayGroups.Add(new DisplayGroup {
            Tag = statusTag,
            Sections = new[] { "Orders", "Status" }
        });
    }

    // Clear tracking for orders that have changed or been removed
    foreach (var oldOrder in oldOrders) {
        var supplier = oldOrder.Key;
        bool orderChanged = false;

        // Check if order was removed
        if (!orders.ContainsKey(supplier)) {
            orderChanged = true;
        } else {
            // Check if items changed
            var newItems = orders[supplier];
            var oldItems = oldOrder.Value;

            if (newItems.Count != oldItems.Count) {
                orderChanged = true;
            } else {
                foreach (var oldItem in oldItems) {
                    if (!newItems.ContainsKey(oldItem.Key) || newItems[oldItem.Key] != oldItem.Value) {
                        orderChanged = true;
                        break;
                    }
                }
            }
        }

        // Clear tracking data for changed orders
        if (orderChanged) {
            orderTimestamps.Remove(supplier);
            orderTransitStatus.Remove(supplier);
            orderSentAmounts.Remove(supplier);
            lastShipmentCargoFull.Remove(supplier);
            orderUnavailableTime.Remove(supplier);
            orderCompletionTime.Remove(supplier);
            // Clean dronesInTransit entries for this supplier
            var staleDrones = new List<string>();
            foreach (var dt in dronesInTransit) {
                if (dt.Value == supplier) staleDrones.Add(dt.Key);
            }
            foreach (var d in staleDrones) dronesInTransit.Remove(d);
        }
    }
}

void ProcessTransitUpdates() {
    // Process cargo assignment messages (drone selected at supply station, loading)
    while (assignmentListener.HasPendingMessage) {
        var message = assignmentListener.AcceptMessage();
        var data = message.Data.ToString();

        // Format: "ASSIGNED|DroneID|Destination|Item:Qty,Item:Qty,..."
        var parts = data.Split('|');

        if (parts.Length < 3 || parts[0] != "ASSIGNED") {
            continue;
        }

        var droneId = parts[1];
        var destination = parts[2];

        // Only track if this station is the destination
        if (destination != stationId) {
            continue;
        }

        // Find the first sent order without a drone assigned
        foreach (var order in orders) {
            if (orderTimestamps.ContainsKey(order.Key)
                && !orderTransitStatus.ContainsKey(order.Key)
                && !orderCompletionTime.ContainsKey(order.Key)) {
                orderTransitStatus[order.Key] = droneId;
                // Clear unavailable status if a drone is now assigned
                orderUnavailableTime.Remove(order.Key);
                break;
            }
        }
    }

    // Process drone departure messages
    while (departureListener.HasPendingMessage) {
        var message = departureListener.AcceptMessage();
        var data = message.Data.ToString();

        // Format: "DEPARTED|DroneID|Destination|FULL or OK|Item:Qty,Item:Qty,..."
        var parts = data.Split('|');

        if (parts.Length < 4 || parts[0] != "DEPARTED") {
            continue;
        }

        var droneId = parts[1];
        var destination = parts[2];
        var cargoStatus = parts[3];

        // Case 1: Drone departed supply station heading TO us
        if (destination == stationId) {
            // Find source station by matching drone to loading status
            string sourceStation = null;
            foreach (var transit in orderTransitStatus) {
                if (transit.Value == droneId) {
                    sourceStation = transit.Key;
                    break;
                }
            }

            // Fallback: first sent order without tracking
            if (sourceStation == null) {
                foreach (var order in orders) {
                    if (orderTimestamps.ContainsKey(order.Key)
                        && !orderTransitStatus.ContainsKey(order.Key)
                        && !orderCompletionTime.ContainsKey(order.Key)) {
                        sourceStation = order.Key;
                        break;
                    }
                }
            }

            if (sourceStation == null) continue;

            // Parse loaded items and accumulate sent amounts
            if (parts.Length >= 5) {
                var itemPairs = parts[4].Split(',');

                if (!orderSentAmounts.ContainsKey(sourceStation)) {
                    orderSentAmounts[sourceStation] = new Dictionary<string, MyFixedPoint>();
                }

                foreach (var pair in itemPairs) {
                    var itemParts = pair.Split(':');
                    if (itemParts.Length == 2) {
                        int qty;
                        if (int.TryParse(itemParts[1].Trim(), out qty)) {
                            var itemName = itemParts[0].Trim();
                            if (orderSentAmounts[sourceStation].ContainsKey(itemName)) {
                                orderSentAmounts[sourceStation][itemName] += qty;
                            } else {
                                orderSentAmounts[sourceStation][itemName] = qty;
                            }
                        }
                    }
                }
            }

            lastShipmentCargoFull[sourceStation] = (cargoStatus == "FULL");

            // Transition: Loading -> InTransit
            orderTransitStatus.Remove(sourceStation);
            dronesInTransit[droneId] = sourceStation;
        }
        // Case 2: A tracked drone departed from somewhere else (leaving our station after delivery)
        else if (dronesInTransit.ContainsKey(droneId)) {
            var sourceStation = dronesInTransit[droneId];
            dronesInTransit.Remove(droneId);
            orderCompletionTime[sourceStation] = DateTime.Now;
            // Trigger inventory refresh after delivery
            _invRefreshTimer = 5f;
        }
    }
}

void CleanupCompletedOrders() {
    var toRemove = new List<string>();
    foreach (var completion in orderCompletionTime) {
        var elapsed = (DateTime.Now - completion.Value).TotalSeconds;
        if (elapsed > completedOrderTimeout) {
            toRemove.Add(completion.Key);
        }
    }
    foreach (var station in toRemove) {
        orderCompletionTime.Remove(station);
        orderTimestamps.Remove(station);
        orderSentAmounts.Remove(station);
        lastShipmentCargoFull.Remove(station);
        orderTransitStatus.Remove(station);
    }
}

void ProcessStockUnavailable() {
    while (stockUnavailableListener.HasPendingMessage) {
        var message = stockUnavailableListener.AcceptMessage();
        var data = message.Data.ToString();

        // Format: "STOCK_UNAVAILABLE|SupplyStationID|OrderingStationID|Item:0,Item:0,..."
        var parts = data.Split('|');

        if (parts.Length < 3 || parts[0] != "STOCK_UNAVAILABLE") {
            continue;
        }

        var supplyStation = parts[1];
        var orderingStation = parts[2];

        // Only process if this message is for us
        if (orderingStation != stationId) {
            continue;
        }

        // Mark this order as unavailable (keep timestamp so it shows in ACTIVE ORDERS)
        orderUnavailableTime[supplyStation] = DateTime.Now;

        // Clear transit/delivery tracking (but keep timestamp for display)
        orderTransitStatus.Remove(supplyStation);
        orderSentAmounts.Remove(supplyStation);
        lastShipmentCargoFull.Remove(supplyStation);
        orderCompletionTime.Remove(supplyStation);
        // Clean dronesInTransit entries for this supplier
        var staleDrones = new List<string>();
        foreach (var dt in dronesInTransit) {
            if (dt.Value == supplyStation) staleDrones.Add(dt.Key);
        }
        foreach (var d in staleDrones) dronesInTransit.Remove(d);
    }
}

void SendOrders() {
    // Re-read CustomData so new/changed orders are picked up without reload.
    // LoadConfig() preserves tracking state for unchanged in-progress orders.
    LoadConfig();
    ScanDisplayPanels();
    ScanCommsAntennas();

    if (orders.Count == 0) {
        LogStatus("  No orders to send");
        return;
    }

    foreach (var order in orders) {
        var destStation = order.Key;
        var items = order.Value;

        if (items.Count == 0) continue;

        // Build order message
        var itemsList = new List<string>();
        foreach (var item in items) {
            itemsList.Add($"{item.Key}:{item.Value}");
        }

        var itemsString = string.Join(",", itemsList);

        // Format: "ORDER|SourceStationID|DestStationID|Item:Qty,Item:Qty,..."
        var message = $"ORDER|{stationId}|{destStation}|{itemsString}";

        // Broadcast order
        IGC.SendBroadcastMessage("STATION_ORDERS", message);

        // Record timestamp
        orderTimestamps[destStation] = DateTime.Now;

        // Reset tracking for this supplier
        orderSentAmounts.Remove(destStation);
        lastShipmentCargoFull.Remove(destStation);
        orderUnavailableTime.Remove(destStation);
        orderCompletionTime.Remove(destStation);
        // Clear any stale dronesInTransit entries for this source
        var staleDrones = new List<string>();
        foreach (var dt in dronesInTransit) {
            if (dt.Value == destStation) staleDrones.Add(dt.Key);
        }
        foreach (var d in staleDrones) dronesInTransit.Remove(d);

        LogStatus($"  Sent order to {destStation}");
        foreach (var item in items) {
            LogStatus($"    {item.Key}: {item.Value}");
        }
    }
}

void SendPickupRequest() {
    // Get unique supplier stations from orders config
    var suppliers = new HashSet<string>();
    foreach (var order in orders) {
        suppliers.Add(order.Key);
    }

    if (suppliers.Count == 0) {
        LogStatus("  No suppliers configured - add orders to define suppliers");
        return;
    }

    foreach (var supplier in suppliers) {
        // Send pickup request (order with no items)
        // Format: "ORDER|SourceStationID|DestStationID|PICKUP"
        var message = $"ORDER|{stationId}|{supplier}|PICKUP";

        // Broadcast pickup request
        IGC.SendBroadcastMessage("STATION_ORDERS", message);

        LogStatus($"  Sent pickup request to {supplier}");
    }
}

void RequestInventory() {
    if (string.IsNullOrEmpty(inventorySourceStation)) {
        LogStatus("  ERROR: InventorySourceStation not configured");
        return;
    }

    // Format: "INVENTORY_REQUEST|RequestingStationID|TargetStationID"
    var message = $"INVENTORY_REQUEST|{stationId}|{inventorySourceStation}";
    IGC.SendBroadcastMessage("INVENTORY_REQUEST", message);

    LogStatus($"  Requesting inventory from {inventorySourceStation}...");
}

void ProcessInventoryResponse() {
    while (inventoryListener.HasPendingMessage) {
        var message = inventoryListener.AcceptMessage();
        var data = message.Data.ToString();

        // Format: "INVENTORY|StationID|Category:Subtype:Amount,..."
        var parts = data.Split('|');

        if (parts.Length < 2 || parts[0] != "INVENTORY") {
            continue;
        }

        var sourceStation = parts[1];

        // Only accept inventory from our configured source station
        if (sourceStation != inventorySourceStation) {
            continue;
        }

        // Clear and update cache
        inventoryCache.Clear();
        inventoryCacheSource = sourceStation;
        lastInventoryUpdate = DateTime.Now;

        // Parse inventory items (may be empty)
        if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2])) {
            var itemPairs = parts[2].Split(',');
            foreach (var pair in itemPairs) {
                // Format: "Category:Subtype:Amount"
                var itemParts = pair.Split(':');
                if (itemParts.Length == 3) {
                    var category = itemParts[0].Trim();
                    var subtype = itemParts[1].Trim();
                    int amount;
                    if (int.TryParse(itemParts[2].Trim(), out amount)) {
                        var key = $"{category}.{subtype}";
                        inventoryCache[key] = amount;
                    }
                }
            }
        }

        LogStatus($"\n  Received inventory from {sourceStation} ({inventoryCache.Count} items)");

        // Update inventory displays immediately (legacy text mode only)
        if (_displayGroups.Count == 0)
            UpdateInventoryDisplays();
    }
}

void ValidateContainers() {
    // Check for incoming containers
    var incomingContainers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(incomingContainers, c =>
        c.CubeGrid == Me.CubeGrid &&
        !c.CustomName.Contains("[EXCLUDE]") && !c.CustomData.Contains("[EXCLUDE]") &&
        !c.CustomName.Contains("[NO-AUTO]") && !c.CustomData.Contains("[NO-AUTO]") &&
        (c.CustomName.Contains($"[{stationId}]") || c.CustomData.Contains($"StationID={stationId}") ||
         c.CustomName.Contains("[INCOMING]") || c.CustomData.Contains("[INCOMING]"))
    );

    LogStatus($"Incoming containers: {incomingContainers.Count}");

    // Get ONLY [EXPORT] tagged containers for export inventory counting
    // This matches the CargoShip behavior which only loads from [EXPORT] containers
    var exportContainers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(exportContainers, c =>
        c.CubeGrid == Me.CubeGrid &&
        !c.CustomName.Contains("[EXCLUDE]") && !c.CustomData.Contains("[EXCLUDE]") &&
        !c.CustomName.Contains("[NO-AUTO]") && !c.CustomData.Contains("[NO-AUTO]") &&
        (c.CustomName.Contains("[EXPORT]") || c.CustomData.Contains("[EXPORT]"))
    );

    LogStatus($"Export containers: {exportContainers.Count}");

    // Show available quantities of export items
    if (exports.Count > 0) {
        if (exportContainers.Count == 0) {
            LogStatus("\nWARNING: No [EXPORT] containers found!");
            LogStatus("Tag containers with [EXPORT] to enable exports");
        } else {
            LogStatus("\nExport Inventory:");
            foreach (var export in exports) {
                var itemType = ParseItemType(export.Key);
                if (itemType == null) continue;

                MyFixedPoint totalAmount = 0;
                foreach (var container in exportContainers) {
                    var inventory = container.GetInventory(0);
                    var items = new List<MyInventoryItem>();
                    inventory.GetItems(items);

                    foreach (var item in items) {
                        if (item.Type == itemType.Value) {
                            totalAmount += item.Amount;
                        }
                    }
                }

                var maxAmount = export.Value;
                if (maxAmount == int.MaxValue) {
                    LogStatus($"  {export.Key}: {(int)totalAmount} available (unlimited)");
                } else {
                    LogStatus($"  {export.Key}: {(int)totalAmount}/{(int)maxAmount} available");
                }
            }
        }
    }
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

string FormatDistance(double km) {
    if (km >= 1000) return $"{km / 1000.0:0.#} Mm";
    if (km >= 10) return $"{km:0.#} km";
    if (km >= 1) return $"{km:0.##} km";
    return $"{km * 1000:0} m";
}

string GetConnectedTarget(IMyLaserAntenna ant) {
    if (ant.Status != MyLaserAntennaStatus.Connected) return "";
    var info = ant.DetailedInfo;
    if (string.IsNullOrEmpty(info)) return "";
    var lines = info.Split('\n');
    for (int i = 0; i < lines.Length; i++) {
        var line = lines[i].Trim();
        if (line.StartsWith("Connected to "))
            return line.Substring(13).Trim();
    }
    return "";
}

double GetAntennaDistanceKm(IMyLaserAntenna ant) {
    var target = ant.TargetCoords;
    if (target == Vector3D.Zero) return -1;
    return Vector3D.Distance(ant.GetPosition(), target) / 1000.0;
}

void LogStatus(string message) {
    statusLog.AppendLine(message);
}

OrderStatus GetOrderStatus(string sourceStation) {
    if (orderUnavailableTime.ContainsKey(sourceStation))
        return OrderStatus.Unavailable;
    if (orderCompletionTime.ContainsKey(sourceStation))
        return OrderStatus.Delivered;
    foreach (var dt in dronesInTransit) {
        if (dt.Value == sourceStation) return OrderStatus.InTransit;
    }
    if (orderTransitStatus.ContainsKey(sourceStation))
        return OrderStatus.Loading;
    if (orderTimestamps.ContainsKey(sourceStation))
        return OrderStatus.Sent;
    return OrderStatus.Pending;
}

string FormatElapsed(TimeSpan elapsed) {
    if (elapsed.TotalMinutes < 1) return $"{(int)elapsed.TotalSeconds}s";
    if (elapsed.TotalHours < 1) return $"{(int)elapsed.TotalMinutes}m";
    return $"{(int)elapsed.TotalHours}h {elapsed.Minutes}m";
}

void UpdateInventoryDisplays() {
    var surfaces = GetDisplaySurfaces(inventoryLcdTag);
    if (surfaces.Count == 0) return;

    // Build full content
    var content = BuildInventoryContent();
    var lines = content.Split('\n');

    int currentLine = 0;

    for (int i = 0; i < surfaces.Count; i++) {
        var surface = surfaces[i];
        surface.ContentType = ContentType.TEXT_AND_IMAGE;

        if (currentLine >= lines.Length) {
            // No more content - clear remaining surfaces
            surface.WriteText("");
            continue;
        }

        // Calculate how many lines this surface can display
        int linesPerSurface = GetSurfaceLineCapacity(surface);

        // Take lines for this surface
        int linesToTake = Math.Min(linesPerSurface, lines.Length - currentLine);
        var surfaceContent = string.Join("\n",
            lines.Skip(currentLine).Take(linesToTake));

        surface.WriteText(surfaceContent);
        currentLine += linesToTake;
    }
}

int GetSurfaceLineCapacity(IMyTextSurface surface) {
    // Approximate: 17 lines at font size 1.0 for standard LCD/cockpit screen
    var fontSize = surface.FontSize;
    int baseLines = 17;
    int estimatedLines = (int)(baseLines / fontSize);
    return Math.Max(1, estimatedLines);
}

// Returns all IMyTextSurface instances tagged with the given tag, sorted by block name.
// Supports both standalone LCD panels (IMyTextPanel) and multi-surface blocks such as
// cockpits and control seats (IMyTextSurfaceProvider).
//
// For IMyTextPanel: tag in block name OR CustomData (backward compatible).
// For IMyTextSurfaceProvider: tag in CustomData with optional ":N" surface index.
//   e.g. "[INVENTORY]" -> surface 0, "[INVENTORY]:1" -> surface 1
List<IMyTextSurface> GetDisplaySurfaces(string tag) {
    var entries = new List<KeyValuePair<string, IMyTextSurface>>();

    // Standard LCD panels -tag matched in name or CustomData
    var panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, p =>
        p.CubeGrid == Me.CubeGrid &&
        (p.CustomName.Contains(tag) || p.CustomData.Contains(tag)));
    foreach (var p in panels) {
        entries.Add(new KeyValuePair<string, IMyTextSurface>(p.CustomName, p));
    }

    // Multi-surface providers (cockpits, control seats, etc.) -tag matched in CustomData
    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(allBlocks, b =>
        b.CubeGrid == Me.CubeGrid &&
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
            return 0; // Tag found without index -default surface 0
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

void ScanCommsAntennas() {
    _commsAntennas.Clear();
    GridTerminalSystem.GetBlocksOfType(_commsAntennas, b =>
        b.CubeGrid == Me.CubeGrid && b.CustomName.Contains(commsTag));
    if (!string.IsNullOrEmpty(commsGroup)) {
        var group = GridTerminalSystem.GetBlockGroupWithName(commsGroup);
        if (group != null) {
            var gl = new List<IMyLaserAntenna>();
            group.GetBlocksOfType(gl);
            var ids = new HashSet<long>();
            for (int i = 0; i < _commsAntennas.Count; i++) ids.Add(_commsAntennas[i].EntityId);
            for (int i = 0; i < gl.Count; i++)
                if (!ids.Contains(gl[i].EntityId) && gl[i].CubeGrid == Me.CubeGrid)
                    _commsAntennas.Add(gl[i]);
        }
    }
    _commsAntennas.Sort((a, b) => string.Compare(a.CustomName, b.CustomName));
}

// ============== SPRITE RENDER PIPELINE ==============
void PrepareDisplay(string[] sections) {
    DLBanner($"ORDER STATION v{VERSION}", stationId);
    foreach (var sec in sections) {
        switch (sec.Trim()) {
            case "Ore":       PrepareInventorySection("Ore", "ORES"); break;
            case "Ingot":     PrepareInventorySection("Ingot", "INGOTS"); break;
            case "Component": PrepareInventorySection("Component", "COMPONENTS"); break;
            case "Ammo":      PrepareInventorySection("AmmoMagazine", "AMMO"); break;
            case "Orders":    PrepareOrdersSection(); break;
            case "Status":    PrepareStatusSection(); break;
            case "Comms":     PrepareCommsSection(); break;
        }
    }
}

void PrepareInventorySection(string category, string label) {
    // Collect matching items
    var items = new List<KeyValuePair<string, MyFixedPoint>>();
    foreach (var item in inventoryCache) {
        var parts = item.Key.Split('.');
        if (parts.Length != 2 || parts[0] != category) continue;
        if (inventoryExclude.Contains(item.Key.ToLower())) continue;
        if (inventoryExclude.Contains(category.ToLower())) continue;
        items.Add(item);
    }
    if (items.Count == 0) return;

    items.Sort((a, b) => string.Compare(a.Key, b.Key));

    DLSep();
    DL("-- " + label + " --", _theme.LBL);
    int shown = 0;
    foreach (var item in items) {
        if (shown >= 30) {
            DL($"  ... and {items.Count - 30} more", _theme.DIM);
            break;
        }
        var subtype = item.Key.Split('.')[1];
        DLR("  " + subtype, _theme.VAL, FormatAmount(item.Value));
        shown++;
    }
}

void PrepareOrdersSection() {
    DLSep();
    DL("-- ORDERS --", _theme.LBL);

    bool hasOrders = false;
    foreach (var order in orders) {
        var station = order.Key;

        var status = GetOrderStatus(station);
        Color statusCol;
        string statusText;

        if (order.Value.Count == 0) {
            statusCol = _theme.DIM;
            statusText = "N/A";
        } else {
            switch (status) {
                case OrderStatus.Loading:     statusCol = _theme.WARN; statusText = "Loading"; break;
                case OrderStatus.InTransit:   statusCol = _theme.HDR;  statusText = "In Transit"; break;
                case OrderStatus.Delivered:   statusCol = _theme.GOOD; statusText = "Delivered"; break;
                case OrderStatus.Unavailable: statusCol = _theme.CRIT; statusText = "Unavailable"; break;
                case OrderStatus.Sent:        statusCol = _theme.VAL;  statusText = "Awaiting Response"; break;
                default:                      statusCol = _theme.DIM;  statusText = "Ready"; break;
            }
        }
        DLDot("  " + station, statusCol, statusCol, statusText);
        hasOrders = true;

        // Show items
        foreach (var item in order.Value) {
            var itemParts = item.Key.Split('.');
            var subtype = itemParts.Length > 1 ? itemParts[1] : item.Key;
            MyFixedPoint sent = 0;
            if (orderSentAmounts.ContainsKey(station) && orderSentAmounts[station].ContainsKey(item.Key))
                sent = orderSentAmounts[station][item.Key];
            DLR("    " + subtype, _theme.DIM, $"{FormatAmount(sent)}/{FormatAmount(item.Value)}");
        }
    }
    if (!hasOrders) DL("  No active orders", _theme.DIM);
}

void PrepareStatusSection() {
    DLSep();
    DL("-- SYSTEM --", _theme.LBL);
    DLR("  Station", _theme.VAL, stationId);
    if (!string.IsNullOrEmpty(inventorySourceStation)) {
        DLR("  Inventory from", _theme.VAL, inventorySourceStation);
        if (!string.IsNullOrEmpty(inventoryCacheSource)) {
            var elapsed = (DateTime.Now - lastInventoryUpdate).TotalSeconds;
            string age = elapsed < 60 ? $"{(int)elapsed}s ago" : $"{(int)(elapsed / 60)}m ago";
            DLR("  Last update", _theme.DIM, age);
        } else {
            DLR("  Last update", _theme.DIM, "never");
        }
        if (_invRefreshSecs > 0)
            DLR("  Auto-refresh", _theme.DIM, $"{(int)_invRefreshSecs}s");
    }
    DLR("  Exports", _theme.DIM, exports.Count.ToString() + " items");
}

void PrepareCommsSection() {
    DLSep();
    DL("-- COMMS --", _theme.LBL);
    if (_commsAntennas.Count == 0) {
        DL("  No antennas found", _theme.DIM);
        return;
    }
    for (int i = 0; i < _commsAntennas.Count; i++) {
        var ant = _commsAntennas[i];
        string name = ant.CustomName.Replace(commsTag, "").Trim();
        if (name.Length == 0) name = "Laser Antenna";

        Color dot; string statusText;
        switch (ant.Status) {
            case MyLaserAntennaStatus.Connected:
                dot = _theme.GOOD; statusText = "Connected"; break;
            case MyLaserAntennaStatus.RotatingToTarget:
                dot = _theme.WARN; statusText = "Rotating"; break;
            case MyLaserAntennaStatus.SearchingTargetForAntenna:
                dot = _theme.WARN; statusText = "Searching"; break;
            case MyLaserAntennaStatus.Connecting:
                dot = _theme.WARN; statusText = "Connecting"; break;
            case MyLaserAntennaStatus.OutOfRange:
                dot = _theme.CRIT; statusText = "Out of Range"; break;
            default:
                dot = _theme.CRIT; statusText = "Idle"; break;
        }

        DLDot("  " + name, dot, dot, statusText);

        if (ant.Status == MyLaserAntennaStatus.Connected) {
            var target = GetConnectedTarget(ant);
            double km = GetAntennaDistanceKm(ant);
            string dist = km >= 0 ? FormatDistance(km) : "";
            if (target.Length > 0)
                DLR("    \u2192 " + target, _theme.DIM, dist);
            else if (dist.Length > 0)
                DLR("    \u2192 connected", _theme.DIM, dist);
            else
                DL("    \u2192 connected", _theme.DIM);
        } else if (ant.Status == MyLaserAntennaStatus.OutOfRange) {
            double km = GetAntennaDistanceKm(ant);
            double maxKm = ant.Range / 1000.0;
            if (km >= 0)
                DL($"    \u2192 {FormatDistance(km)} (max {FormatDistance(maxKm)})", _theme.DIM);
            else
                DL("    \u2192 --", _theme.DIM);
        } else {
            DL("    \u2192 --", _theme.DIM);
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
        float charW = fontSize * 21.56f;
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

        float cx = vp.X + surfSize.X * 0.5f;       // horizontal center
        float rightX = vp.X + surfSize.X - pad;    // right edge with padding
        float maxY = vp.Y + surfSize.Y - pad;      // bottom clipping edge

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

            // Section header (LBL color = amber bg with dark text)
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
// ============== END SPRITE RENDER PIPELINE ==============

string BuildInventoryContent() {
    // Handle empty/not configured states
    if (string.IsNullOrEmpty(inventorySourceStation)) {
        return "InventorySourceStation not configured";
    }

    if (inventoryCache.Count == 0 && string.IsNullOrEmpty(inventoryCacheSource)) {
        return "No inventory data.\nRun 'inventory' to request.";
    }

    if (inventoryCache.Count == 0 && !string.IsNullOrEmpty(inventoryCacheSource)) {
        return $"No items in stock at {inventoryCacheSource}";
    }

    var sb = new StringBuilder();
    sb.AppendLine($"=== INVENTORY: {inventoryCacheSource} ===");
    sb.AppendLine($"Snapshot: {lastInventoryUpdate:yyyy-MM-dd HH:mm:ss}");

    // Group items by category in order: Ore, Ingot, Component, AmmoMagazine
    var categories = new[] { "Ore", "Ingot", "Component", "AmmoMagazine" };

    foreach (var category in categories) {
        var categoryLower = category.ToLower();

        // Skip if entire category is excluded
        if (inventoryExclude.Contains(categoryLower)) continue;

        var categoryItems = new List<KeyValuePair<string, MyFixedPoint>>();

        foreach (var item in inventoryCache) {
            var parts = item.Key.Split('.');
            if (parts.Length != 2) continue;

            var itemCategory = parts[0];
            if (itemCategory != category) continue;

            var itemKeyLower = item.Key.ToLower();

            // Skip if specific item is excluded
            if (inventoryExclude.Contains(itemKeyLower)) continue;

            categoryItems.Add(item);
        }

        if (categoryItems.Count == 0) continue;

        // Add category header
        sb.AppendLine();
        sb.AppendLine($"--- {category.ToUpper()}S ---");

        // Sort items by name and add them
        foreach (var item in categoryItems.OrderBy(x => x.Key)) {
            var subtype = item.Key.Split('.')[1];
            sb.AppendLine($"{subtype}: {FormatAmount(item.Value)}");
        }
    }

    sb.AppendLine();
    sb.AppendLine("(Run 'inventory' to refresh)");

    return sb.ToString();
}

void UpdateDisplays() {
    // Echo to PB screen
    Echo(statusLog.ToString());

    // Legacy text display -only when no sprite display groups are configured
    if (_displayGroups.Count == 0) {
        foreach (var surface in GetDisplaySurfaces(statusLcdTag)) {
            surface.ContentType = ContentType.TEXT_AND_IMAGE;
            surface.WriteText(statusLog.ToString());
        }
    }
}

#endregion // OrderStation
