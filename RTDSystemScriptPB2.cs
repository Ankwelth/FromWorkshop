// PB2: Display Engine (LCD updates + config templates). Requires PB1 Core Engine.

const string IGC_CHANNEL = "INV_DATA";

// LCD Tags
const string LCD_TAG_DEF = "[INV:TagDef]";
const string LCD_CRAFT = "[INV:Craft]";
const string LCD_CRAFT_QUEUE = "[INV:CraftQueue]";
const string LCD_STATUS = "[INV:Status]";
const string LCD_POWER = "[INV:Power]";
const string LCD_LOG = "[INV:Log]";
const string LCD_WARN = "[INV:Warnings]";
const string LCD_SMELT = "[INV:Smelt]";
const string LCD_STASIS = "[INV:Stasis]";
const string LCD_LOADOUT = "[INV:Loadout:";
const string LCD_DOCKED = "[INV:Docked]";

// Block Tags
const string TAG_NO_SORT = "[NoSort]";
const string TAG_NO_MANAGE = "[NoManage]";
const string TAG_MANUAL = "[Manual]";

// Exclusion Keywords
string[] LOCKED_KEYWORDS = { "[Locked]", "Control Station", "Control Seat", "Safe Zone" };
string[] HIDDEN_KEYWORDS = { "[Hidden]" };

// Item type strings
const string OB = "MyObjectBuilder_";
const string T_ORE = "Ore";
const string T_INGOT = "Ingot";
const string T_COMP = "Component";
const string T_AMMO = "AmmoMagazine";
const string T_OXYB = "OxygenContainerObject";
const string T_GASB = "GasContainerObject";
const string T_GUN = "PhysicalGunObject";
const string T_PHYS = "PhysicalObject";
const string T_FOOD = "ConsumableItem";
const string T_DATA = "Datapad";

// Display defaults
string defaultFont = "Monospace";
float defaultFontSize = 0.65f;
float defaultPadding = 2f;

static readonly Dictionary<string, string> DISPLAY_NAMES = new Dictionary<string, string> {
    {"Construction", "Construction Comp."}, {"MetalGrid", "Metal Grid"}, {"InteriorPlate", "Interior Plate"},
    {"SteelPlate", "Steel Plate"}, {"Girder", "Girder"}, {"SmallTube", "Small Tube"}, {"LargeTube", "Large Tube"},
    {"Motor", "Motor"}, {"Display", "Display"}, {"BulletproofGlass", "Bulletproof Glass"}, {"Computer", "Computer"},
    {"Reactor", "Reactor Comp."}, {"Thrust", "Thruster Comp."}, {"GravityGenerator", "Gravity Gen. Comp."},
    {"Medical", "Medical Comp."}, {"RadioCommunication", "Radio-comm Comp."}, {"Detector", "Detector Comp."},
    {"Explosives", "Explosives"}, {"SolarCell", "Solar Cell"}, {"PowerCell", "Power Cell"}, {"Superconductor", "Superconductor"},
    {"Canvas", "Canvas"}, {"ZoneChip", "Zone Chip"},
    {"NATO_25x184mm", "Gatling Ammo"}, {"Missile200mm", "Rockets"}, {"AutocannonClip", "Autocannon Mag"},
    {"MediumCalibreAmmo", "Assault Cannon Shell"}, {"LargeCalibreAmmo", "Artillery Shell"},
    {"SmallRailgunAmmo", "Small Railgun Sabot"}, {"LargeRailgunAmmo", "Large Railgun Sabot"},
    {"SemiAutoPistolMagazine", "S-10 Mag"}, {"ElitePistolMagazine", "S-10E Mag"}, {"FullAutoPistolMagazine", "S-20A Mag"},
    {"AutomaticRifleGun_Mag_20rd", "MR-20 Mag"}, {"RapidFireAutomaticRifleGun_Mag_50rd", "MR-50A Mag"},
    {"PreciseAutomaticRifleGun_Mag_5rd", "MR-8P Mag"}, {"UltimateAutomaticRifleGun_Mag_30rd", "MR-30E Mag"},
    {"AngleGrinderItem", "Angle Grinder"}, {"AngleGrinder2Item", "Enh. Grinder"}, {"AngleGrinder3Item", "Prof. Grinder"},
    {"AngleGrinder4Item", "Elite Grinder"}, {"HandDrillItem", "Hand Drill"}, {"HandDrill2Item", "Enh. Drill"},
    {"HandDrill3Item", "Prof. Drill"}, {"HandDrill4Item", "Elite Drill"}, {"WelderItem", "Welder"},
    {"Welder2Item", "Enh. Welder"}, {"Welder3Item", "Prof. Welder"}, {"Welder4Item", "Elite Welder"},
    {"AutomaticRifleItem", "MR-20 Rifle"}, {"RapidFireAutomaticRifleItem", "MR-50A Rifle"},
    {"PreciseAutomaticRifleItem", "MR-8P Rifle"}, {"UltimateAutomaticRifleItem", "MR-30E Rifle"},
    {"SemiAutoPistolItem", "S-10 Pistol"}, {"FullAutoPistolItem", "S-20A Pistol"}, {"ElitePistolItem", "S-10E Pistol"},
    {"BasicHandHeldLauncherItem", "RO-1 Launcher"}, {"AdvancedHandHeldLauncherItem", "PRO-1 Launcher"},
    {"OxygenBottle", "Oxygen Bottle"}, {"HydrogenBottle", "Hydrogen Bottle"}, {"Medkit", "Medkit"}, {"Powerkit", "Powerkit"},
    {"Scrap", "Scrap Metal"},
};

IMyBroadcastListener coreListener;
DateTime lastCoreMsg = DateTime.MinValue;
List<string> coreLog = new List<string>();
List<string> coreWarn = new List<string>();
Dictionary<string, string> coreState = new Dictionary<string, string>();

// Scanned blocks
List<IMyTextPanel> panels = new List<IMyTextPanel>();
List<IMyTerminalBlock> invBlocks = new List<IMyTerminalBlock>();
List<IMyShipConnector> connectors = new List<IMyShipConnector>();
HashSet<IMyCubeGrid> noSortGrids = new HashSet<IMyCubeGrid>();
HashSet<IMyCubeGrid> noManageGrids = new HashSet<IMyCubeGrid>();
List<IMyAssembler> assemblers = new List<IMyAssembler>();
List<IMyRefinery> refineries = new List<IMyRefinery>();
List<IMyReactor> reactors = new List<IMyReactor>();
List<IMyGasGenerator> generators = new List<IMyGasGenerator>();
List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyTerminalBlock> hydrogenEngines = new List<IMyTerminalBlock>();
List<IMyGasTank> gasTanks = new List<IMyGasTank>();
List<IMySolarPanel> solarPanels = new List<IMySolarPanel>();

// Data for displays
Dictionary<MyItemType, ItemInfo> itemDb = new Dictionary<MyItemType, ItemInfo>();
Dictionary<MyItemType, double> globalCounts = new Dictionary<MyItemType, double>();
Dictionary<MyItemType, double> craftTargets = new Dictionary<MyItemType, double>();
Dictionary<MyItemType, int> craftPriorities = new Dictionary<MyItemType, int>();
Dictionary<MyItemType, double> ingotTargets = new Dictionary<MyItemType, double>();
Dictionary<MyItemType, int> ingotPriorities = new Dictionary<MyItemType, int>();
Dictionary<string, string> customNames = new Dictionary<string, string>();
Dictionary<MyItemType, double> stasisTargets = new Dictionary<MyItemType, double>();

// Scratch field used by CompareCatItems during status display sort
List<string> _sortBottomItems = new List<string>();

// Power config (display only)
bool powerDischargeMode = false;
bool gasStockpileMode = false;
double powerUpperThreshold = 95;
double powerLowerThreshold = 80;
double gasUpperThreshold = 95;
double gasLowerThreshold = 80;

// --- Performance ---
// Server limit is 1.0ms — keep maxRuntimeMs below that
double maxRuntimeMs = 0.75;
bool enableAutoThrottle = true;
int throttleSkip = 0;
int pb2Tick = 0;
const int SCAN_INTERVAL = 8; // only rescan blocks every 8th tick (low-load mode)
int startupDelayTicks = 10;  // first start warmup (~16s at Update100)

Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    coreListener = IGC.RegisterBroadcastListener(IGC_CHANNEL);
    coreListener.SetMessageCallback(IGC_CHANNEL);
    PrePopulateItemDb();
}

string lastError = "";
string pb2Step = "Init";
int pb2Runs = 0;

void Main(string argument, UpdateType updateSource) {
    if (startupDelayTicks > 0) { startupDelayTicks--; return; }
    if (enableAutoThrottle && throttleSkip > 0) { throttleSkip--; return; }
    pb2Runs++;
    pb2Tick++;
    try {
        if (argument == "rescan") { pb2Tick = SCAN_INTERVAL; }
        pb2Step = "IGC"; ProcessCoreIgc();
        // Heavy scanning only every SCAN_INTERVAL ticks (~12.8s at Update100)
        if (pb2Tick >= SCAN_INTERVAL) {
            pb2Tick = 0;
            pb2Step = "ScanConn"; ScanConnectors();
            pb2Step = "ScanBlocks"; ScanBlocks();
            pb2Step = "CountItems"; CountAllItems();
            pb2Step = "Templates"; EnsureTemplatesAndReadNames();
            pb2Step = "CraftTargets"; ReadCraftTargets();
            pb2Step = "SmeltTargets"; ReadSmeltTargets();
            pb2Step = "StasisTargets"; ReadStasisTargets();
            pb2Step = "PowerCfg"; ReadPowerConfig();
        }
        pb2Step = "SyncFlags"; SyncCoreStateFlags();
        // Log and warnings update every tick for responsiveness
        foreach (var p in panels) {
            string n = p.CustomName;
            if (n.Contains(LCD_LOG)) UpdateLogDisplay(p);
            else if (n.Contains(LCD_WARN)) UpdateWarningsDisplay(p);
        }
        // All other displays every 2nd tick (~3.2s at Update100)
        if (pb2Tick % 2 == 0) {
            pb2Step = "Displays"; UpdateDisplays();
        }
        pb2Step = "OK";
        lastError = "";
    } catch (Exception e) {
        lastError = e.Message + "\n" + e.StackTrace;
    }
    UpdateEcho();
    if (enableAutoThrottle && Runtime.LastRunTimeMs > maxRuntimeMs)
        throttleSkip = (int)(Runtime.LastRunTimeMs / maxRuntimeMs) + 1;
}

void SyncCoreStateFlags() {
    string pm; if (coreState.TryGetValue("powerMode", out pm)) powerDischargeMode = pm == "DISCHARGE";
    string gm; if (coreState.TryGetValue("gasMode", out gm)) gasStockpileMode = gm == "STOCKPILE";
}

// ======================== IGC (PB1 -> PB2) ===================================

void ProcessCoreIgc() {
    while (coreListener.HasPendingMessage) {
        var msg = coreListener.AcceptMessage();
        var data = msg.Data as string;
        if (data == null) continue;
        ParseCoreMessage(data);
        lastCoreMsg = DateTime.UtcNow;
    }
}

void ParseCoreMessage(string data) {
    coreLog.Clear();
    coreWarn.Clear();
    coreState.Clear();
    string section = "";
    var lines = data.Split('\n');
    foreach (var raw in lines) {
        var line = raw.TrimEnd('\r');
        if (line == "[LOG]" || line == "[WARN]" || line == "[STATE]") { section = line; continue; }
        if (string.IsNullOrWhiteSpace(line)) continue;
        if (section == "[LOG]") coreLog.Add(line);
        else if (section == "[WARN]") coreWarn.Add(line);
        else if (section == "[STATE]") {
            int eq = line.IndexOf('=');
            if (eq > 0) coreState[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
        }
    }
}

// ======================== SCANNING ===========================================

void ScanConnectors() {
    noSortGrids.Clear();
    noManageGrids.Clear();
    connectors.Clear();
    GridTerminalSystem.GetBlocksOfType(connectors);
    foreach (var conn in connectors) {
        if (conn.Status != MyShipConnectorStatus.Connected) continue;
        if (conn.OtherConnector == null) continue;
        bool noSort = conn.CustomName.Contains(TAG_NO_SORT) || conn.OtherConnector.CustomName.Contains(TAG_NO_SORT);
        bool noManage = conn.CustomName.Contains(TAG_NO_MANAGE) || conn.OtherConnector.CustomName.Contains(TAG_NO_MANAGE);
        if (noSort) {
            var otherGrid = conn.OtherConnector.CubeGrid;
            if (otherGrid != Me.CubeGrid) noSortGrids.Add(otherGrid);
            else noSortGrids.Add(conn.CubeGrid);
        }
        if (noManage) {
            var otherGrid = conn.OtherConnector.CubeGrid;
            if (otherGrid != Me.CubeGrid) noManageGrids.Add(otherGrid);
            else noManageGrids.Add(conn.CubeGrid);
        }
    }
}

void ScanBlocks() {
    panels.Clear();
    invBlocks.Clear();
    assemblers.Clear();
    refineries.Clear();
    reactors.Clear();
    generators.Clear();
    batteries.Clear();
    hydrogenEngines.Clear();
    gasTanks.Clear();
    solarPanels.Clear();

    var all = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(all, b => b.HasInventory || b is IMyTextPanel);

    foreach (var b in all) {
        if (noManageGrids.Contains(b.CubeGrid)) continue;

        if (b is IMyTextPanel && b.IsSameConstructAs(Me))
            panels.Add(b as IMyTextPanel);

        if (!b.HasInventory) continue;
        if (IsLocked(b)) continue;

        bool onNoSortGrid = noSortGrids.Contains(b.CubeGrid);
        if (onNoSortGrid && !(b is IMyReactor) && !(b is IMyGasGenerator)) continue;

        invBlocks.Add(b);

        string name = b.CustomName;
        if (b is IMyAssembler && b.IsSameConstructAs(Me) && !name.Contains(TAG_MANUAL))
            assemblers.Add(b as IMyAssembler);
        if (b is IMyRefinery && b.IsSameConstructAs(Me) && !name.Contains(TAG_MANUAL))
            refineries.Add(b as IMyRefinery);
        if (b is IMyReactor && b.IsSameConstructAs(Me) && !name.Contains(TAG_MANUAL))
            reactors.Add(b as IMyReactor);
        if (b is IMyGasGenerator && b.IsSameConstructAs(Me) && !name.Contains(TAG_MANUAL))
            generators.Add(b as IMyGasGenerator);
    }

    var tempBat = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType(tempBat, x => x.IsSameConstructAs(Me)
        && !noManageGrids.Contains(x.CubeGrid) && !x.CustomName.Contains(TAG_MANUAL));
    batteries.AddRange(tempBat);

    var tempProd = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType(tempProd, x => x.IsSameConstructAs(Me)
        && !noManageGrids.Contains(x.CubeGrid) && !x.CustomName.Contains(TAG_MANUAL)
        && x.BlockDefinition.SubtypeId.Contains("HydrogenEngine"));
    foreach (var p in tempProd) hydrogenEngines.Add(p);

    var tempTanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType(tempTanks, x => x.IsSameConstructAs(Me)
        && !noManageGrids.Contains(x.CubeGrid) && !x.CustomName.Contains(TAG_MANUAL));
    gasTanks.AddRange(tempTanks);

    var tempSolar = new List<IMySolarPanel>();
    GridTerminalSystem.GetBlocksOfType(tempSolar, x => x.IsSameConstructAs(Me)
        && !noManageGrids.Contains(x.CubeGrid) && !x.CustomName.Contains(TAG_MANUAL));
    solarPanels.AddRange(tempSolar);
}

bool IsLocked(IMyTerminalBlock b) {
    string name = b.CustomName;
    foreach (var kw in LOCKED_KEYWORDS) if (name.Contains(kw)) return true;
    return false;
}

bool IsHidden(IMyTerminalBlock b) {
    string name = b.CustomName;
    foreach (var kw in HIDDEN_KEYWORDS) if (name.Contains(kw)) return true;
    return false;
}

void CountAllItems() {
    globalCounts.Clear();
    foreach (var b in invBlocks) {
        if (IsHidden(b)) continue;
        for (int invIdx = 0; invIdx < b.InventoryCount; invIdx++) {
            var inv = b.GetInventory(invIdx);
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            foreach (var it in items) {
                if (!itemDb.ContainsKey(it.Type)) itemDb[it.Type] = new ItemInfo(it.Type);
                double amt = (double)it.Amount;
                double cur;
                if (globalCounts.TryGetValue(it.Type, out cur)) globalCounts[it.Type] = cur + amt;
                else globalCounts[it.Type] = amt;
            }
        }
    }
}

double GetGlobalCount(MyItemType t) {
    double v; globalCounts.TryGetValue(t, out v); return v;
}

// ======================== PANEL TEMPLATES + NAMES ============================

void EnsureTemplatesAndReadNames() {
    customNames.Clear();

    foreach (var p in panels) {
        string n = p.CustomName;

        if (n.Contains(LCD_TAG_DEF)) {
            if (string.IsNullOrWhiteSpace(p.CustomData) || !p.CustomData.Contains("="))
                InitTagDefPanel(p);
        }

        string statusFilter = GetLcdFilter(n, "[INV:Status");
        if (statusFilter != null) {
            if (string.IsNullOrWhiteSpace(p.CustomData))
                InitStatusPanel(p, statusFilter);
        }

        string craftFilter = GetLcdFilter(n, "[INV:Craft");
        if (craftFilter != null) {
            if (string.IsNullOrWhiteSpace(p.CustomData)) {
                string cf = craftFilter.ToLower();
                if (cf == "" || cf == "targets") InitCraftPanel(p);
            }
        }

        string smeltFilter = GetLcdFilter(n, "[INV:Smelt");
        if (smeltFilter != null) {
            if (string.IsNullOrWhiteSpace(p.CustomData))
                InitSmeltPanel(p);
        }

        if (n.Contains(LCD_POWER)) {
            if (string.IsNullOrWhiteSpace(p.CustomData) || !p.CustomData.Contains("="))
                InitPowerPanel(p);
        }

        if (n.Contains(LCD_LOADOUT)) {
            if (string.IsNullOrWhiteSpace(p.CustomData)) {
                string prof = ExtractBracketValue(n, LCD_LOADOUT);
                if (!string.IsNullOrEmpty(prof)) InitLoadoutPanel(p, prof);
            }
        }


        if (n.Contains(LCD_DOCKED)) {
            if (string.IsNullOrWhiteSpace(p.CustomData) || !p.CustomData.Contains("[Display]"))
                InitDockedPanel(p);
        }

        if (n.Contains(LCD_LOG) || n.Contains(LCD_WARN)) {
            if (string.IsNullOrWhiteSpace(p.CustomData) || !p.CustomData.Contains("[Display]"))
                InitSimplePanel(p);
        }

        ParseRenameSection(p.CustomData);
    }
}

void ParseRenameSection(string data) {
    if (string.IsNullOrWhiteSpace(data) || !data.Contains("[Rename]")) return;
    bool inRename = false;
    foreach (var raw in data.Split('\n')) {
        string line = raw.Trim();
        if (line == "[Rename]") { inRename = true; continue; }
        if (line.StartsWith("[")) { inRename = false; continue; }
        if (!inRename || line.StartsWith("#") || !line.Contains("=")) continue;
        var parts = line.Split(new char[]{'='}, 2);
        if (parts.Length != 2) continue;
        string k = parts[0].Trim();
        string v = parts[1].Trim();
        if (k.Length == 0 || v.Length == 0) continue;
        customNames[k] = v;
    }
}

string ExtractBracketValue(string name, string prefix) {
    int start = name.IndexOf(prefix);
    if (start < 0) return null;
    start += prefix.Length;
    int end = name.IndexOf(']', start);
    if (end < 0) return null;
    return name.Substring(start, end - start);
}

// ======================== TARGET PARSING =====================================

void ReadCraftTargets() {
    craftTargets.Clear();
    craftPriorities.Clear();
    foreach (var p in panels) {
        string craftFilter = GetLcdFilter(p.CustomName, "[INV:Craft");
        if (craftFilter == null) continue;
        string data = p.CustomData;
        if (string.IsNullOrWhiteSpace(data)) continue;
        foreach (var rawLine in data.Split('\n')) {
            string line = rawLine.Trim();
            if (line.StartsWith("[") || line.StartsWith("#") || line.StartsWith("~") || !line.Contains("=")) continue;
            var eq = line.Split(new char[]{'='}, 2);
            if (eq.Length != 2) continue;
            string itemSpec = eq[0].Trim();
            if (itemSpec == "Font" || itemSpec == "FontSize" || itemSpec == "Padding") continue;
            // Parse value: amount and optional ,P:N priority
            string valStr = eq[1].Trim();
            int priority = 0;
            int commaIdx = valStr.IndexOf(",P:", StringComparison.OrdinalIgnoreCase);
            if (commaIdx >= 0) {
                int.TryParse(valStr.Substring(commaIdx + 3).Trim(), out priority);
                valStr = valStr.Substring(0, commaIdx).Trim();
            }
            double target;
            if (!double.TryParse(valStr, out target)) continue;
            if (target <= 0) continue;
            string lookupId = itemSpec;
            string customName = null;
            int colonIdx = itemSpec.IndexOf(':');
            if (colonIdx > 0) {
                lookupId = itemSpec.Substring(0, colonIdx).Trim();
                customName = itemSpec.Substring(colonIdx + 1).Trim();
            }
            bool found = false;
            foreach (var kv in itemDb) {
                if (kv.Key.SubtypeId == lookupId || kv.Value.displayName == lookupId || kv.Value.displayName == itemSpec) {
                    craftTargets[kv.Key] = target;
                    craftPriorities[kv.Key] = priority;
                    if (!string.IsNullOrEmpty(customName) && customName != kv.Value.displayName)
                        customNames[kv.Key.SubtypeId] = customName;
                    found = true;
                    break;
                }
            }
            if (!found) {
                if (lookupId.Contains("/")) {
                    var tp = lookupId.Split('/');
                    if (tp.Length == 2) {
                        var it = new MyItemType(tp[0], tp[1]);
                        craftTargets[it] = target;
                        craftPriorities[it] = priority;
                        if (!string.IsNullOrEmpty(customName)) customNames[tp[1]] = customName;
                    }
                } else {
                    string[] tryTypes = { OB+T_COMP, OB+T_AMMO, OB+T_GUN, OB+T_FOOD, OB+T_OXYB, OB+T_GASB, OB+T_PHYS };
                    bool matched = false;
                    foreach (var tid in tryTypes) {
                        var candidate = new MyItemType(tid, lookupId);
                        if (globalCounts.ContainsKey(candidate)) {
                            craftTargets[candidate] = target;
                            craftPriorities[candidate] = priority;
                            matched = true;
                            break;
                        }
                    }
                    if (!matched) {
                        var fb = new MyItemType(OB + T_COMP, lookupId);
                        craftTargets[fb] = target;
                        craftPriorities[fb] = priority;
                    }
                }
            }
        }
    }
}

void ReadSmeltTargets() {
    ingotTargets.Clear();
    ingotPriorities.Clear();
    foreach (var p in panels) {
        string smeltFilter = GetLcdFilter(p.CustomName, "[INV:Smelt");
        if (smeltFilter == null) continue;
        string data = p.CustomData;
        if (string.IsNullOrWhiteSpace(data)) continue;
        foreach (var rawLine in data.Split('\n')) {
            string line = rawLine.Trim();
            if (line.StartsWith("[") || line.StartsWith("#") || !line.Contains("=")) continue;
            var parts = line.Split(new char[]{'='}, 2);
            if (parts.Length != 2) continue;
            string ingotName = parts[0].Trim();
            if (ingotName == "Font" || ingotName == "FontSize" || ingotName == "Padding") continue;
            string valStr = parts[1].Trim();
            int priority = 0;
            int commaIdx = valStr.IndexOf(',');
            if (commaIdx >= 0) {
                int.TryParse(valStr.Substring(commaIdx + 1).Trim(), out priority);
                valStr = valStr.Substring(0, commaIdx);
            }
            double target = ParseAmount(valStr);
            if (target <= 0) continue;
            var ingotType = new MyItemType(OB + T_INGOT, ingotName);
            ingotTargets[ingotType] = target;
            if (priority > 0) ingotPriorities[ingotType] = priority;
        }
    }
}

double ParseAmount(string str) {
    str = str.Trim().ToUpper();
    double mult = 1;
    if (str.EndsWith("G")) { mult = 1000000000; str = str.Substring(0, str.Length - 1); }
    else if (str.EndsWith("M")) { mult = 1000000; str = str.Substring(0, str.Length - 1); }
    else if (str.EndsWith("K")) { mult = 1000; str = str.Substring(0, str.Length - 1); }
    double val; if (!double.TryParse(str.Trim(), out val)) return -1;
    return val * mult;
}

void ReadStasisTargets() {
    stasisTargets.Clear();
    foreach (var p in panels) {
        if (GetLcdFilter(p.CustomName, "[INV:Status") == null) continue;
        string data = p.CustomData;
        if (string.IsNullOrWhiteSpace(data)) continue;
        foreach (var rawLine in data.Split('\n')) {
            string line = rawLine.Trim();
            if (line.StartsWith("[") || line.StartsWith("#") || !line.Contains("=")) continue;
            var parts = line.Split(new char[]{'='}, 2);
            if (parts.Length != 2) continue;
            string s = parts[0].Trim();
            if (s == "Font" || s == "FontSize" || s == "Padding") continue;
            double amt = ParseAmount(parts[1].Trim());
            string id = s; string cn = null; int ci = s.IndexOf(':');
            if (ci > 0) { id = s.Substring(0, ci).Trim(); cn = s.Substring(ci+1).Trim(); }
            if (amt <= 0) continue;
            bool found = false;
            foreach (var kv in itemDb) {
                if (kv.Key.SubtypeId == id || kv.Value.displayName == id) {
                    stasisTargets[kv.Key] = amt;
                    // Store custom name keyed by full TypeId/SubtypeId so ore and
                    // ingot with the same SubtypeId each get the right name
                    if (!string.IsNullOrEmpty(cn))
                        customNames[kv.Key.TypeId + "/" + kv.Key.SubtypeId] = cn;
                    found = true;
                    // Don't break — keep iterating so ALL types with this SubtypeId
                    // get a stasis entry (e.g. both Ore/Cobalt and Ingot/Cobalt)
                }
            }
            if (!found) {
                // Store by SubtypeId only when we can't resolve the exact type
                if (!string.IsNullOrEmpty(cn)) customNames[id] = cn;
                string[] tt = { OB+T_COMP, OB+T_AMMO, OB+T_GUN, OB+T_FOOD, OB+T_OXYB, OB+T_GASB, OB+T_PHYS };
                foreach (var tid in tt) {
                    var c = new MyItemType(tid, id);
                    if (globalCounts.ContainsKey(c)) { stasisTargets[c] = amt; break; }
                }
            }
        }
    }
}

void ReadPowerConfig() {
    foreach (var p in panels) {
        if (!p.CustomName.Contains(LCD_POWER)) continue;
        string data = p.CustomData;
        if (string.IsNullOrWhiteSpace(data)) continue;
        foreach (var rawLine in data.Split('\n')) {
            string line = rawLine.Trim();
            if (line.StartsWith("[") || line.StartsWith("#") || !line.Contains("=")) continue;
            var parts = line.Split('=');
            if (parts.Length != 2) continue;
            string key = parts[0].Trim();
            string val = parts[1].Trim();
            if (key == "UpperThreshold") double.TryParse(val, out powerUpperThreshold);
            else if (key == "LowerThreshold") double.TryParse(val, out powerLowerThreshold);
            else if (key == "GasUpperThreshold") double.TryParse(val, out gasUpperThreshold);
            else if (key == "GasLowerThreshold") double.TryParse(val, out gasLowerThreshold);
        }
    }
}

// ======================== DISPLAYS ===========================================

void UpdateDisplays() {
    foreach (var p in panels) {
        string n = p.CustomName;

        string statusFilter = GetLcdFilter(n, "[INV:Status");
        if (statusFilter != null) { UpdateStatusDisplay(p, statusFilter); continue; }

        string craftFilter = GetLcdFilter(n, "[INV:Craft");
        if (craftFilter != null) { UpdateCraftDisplay(p, craftFilter); continue; }

        if (n.Contains(LCD_CRAFT_QUEUE)) { UpdateCraftQueueDisplay(p); continue; }

        string smeltFilter = GetLcdFilter(n, "[INV:Smelt");
        if (smeltFilter != null) { UpdateSmeltDisplay(p, smeltFilter); continue; }

        string stasisFilter = GetLcdFilter(n, "[INV:Stasis");
        if (stasisFilter != null) { UpdateStasisDisplay(p, stasisFilter); continue; }

        if (n.Contains(LCD_POWER)) { UpdatePowerDisplay(p); continue; }
        if (n.Contains(LCD_LOG)) continue; // updated every tick in Main
        if (n.Contains(LCD_WARN)) continue; // updated every tick in Main
        if (n.Contains(LCD_DOCKED)) { UpdateDockedDisplay(p); continue; }
    }
}

void UpdateStatusDisplay(IMyTextPanel p, string filter) {
    ApplyPanelDefaults(p);

    bool hasFilter = !string.IsNullOrEmpty(filter);
    int fixedPage = -1; if (hasFilter) int.TryParse(filter, out fixedPage);
    string[] filterWords = (hasFilter && fixedPage <= 0) ? filter.ToLower().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries) : null;

    var hiddenItems = new HashSet<string>();
    var bottomItems = new List<string>();
    string statusData = p.CustomData;
    if (!string.IsNullOrWhiteSpace(statusData)) {
        bool inHide = false, inBottom = false;
        foreach (var raw in statusData.Split('\n')) {
            string l = raw.Trim();
            if (l == "[Hide]") { inHide = true; inBottom = false; continue; }
            if (l == "[Bottom]") { inBottom = true; inHide = false; continue; }
            if (l.StartsWith("[")) { inHide = false; inBottom = false; continue; }
            if (l.StartsWith("#") || string.IsNullOrEmpty(l)) continue;
            if (inHide) hiddenItems.Add(l);
            if (inBottom) bottomItems.Add(l);
        }
    }

    var categories = new Dictionary<string, List<KeyValuePair<MyItemType, double>>>();
    foreach (var kv in globalCounts) {
        string cat = GetCategory(kv.Key);
        List<KeyValuePair<MyItemType, double>> list;
        if (!categories.TryGetValue(cat, out list)) { list = new List<KeyValuePair<MyItemType, double>>(); categories[cat] = list; }
        list.Add(kv);
    }
    foreach (var ct in craftTargets) {
        if (globalCounts.ContainsKey(ct.Key)) continue;
        string cat = GetCategory(ct.Key);
        List<KeyValuePair<MyItemType, double>> list;
        if (!categories.TryGetValue(cat, out list)) { list = new List<KeyValuePair<MyItemType, double>>(); categories[cat] = list; }
        list.Add(new KeyValuePair<MyItemType, double>(ct.Key, 0));
    }

    var allLines = new List<string>();
    allLines.Add(filterWords != null ? "=== " + filter + " ===" : "=== Inventory Status ===");
    allLines.Add("");

    var catKeys = new List<string>(categories.Keys);
    catKeys.Sort();
    foreach (var catName in catKeys) {
        var catItems = categories[catName];
        if (filterWords != null) {
            bool match = false;
            string c = catName.ToLower();
            foreach (var w in filterWords) { if (c.Contains(w)) { match = true; break; } }
            if (!match) continue;
        }
        if (hiddenItems.Contains(catName)) continue;
        allLines.Add("--- " + catName + " ---");
        _sortBottomItems = bottomItems;
        catItems.Sort(CompareCatItems);
        foreach (var itemKv in catItems) {
            string dn = GetDisplayName(itemKv.Key);
            if (hiddenItems.Count > 0 && (hiddenItems.Contains(dn) || hiddenItems.Contains(itemKv.Key.SubtypeId))) continue;
            double target;
            double stasisLimit;
            bool hasStasis = stasisTargets.TryGetValue(itemKv.Key, out stasisLimit) && stasisLimit > 0;
            string overTag = (hasStasis && itemKv.Value > stasisLimit) ? " OVER" : "";
            if (craftTargets.TryGetValue(itemKv.Key, out target) && target > 0) {
                double ratio = Math.Min(itemKv.Value / target, 1.0);
                string counts = FormatNumber(itemKv.Value) + "/" + FormatNumber(target);
                allLines.Add("  " + dn.PadRight(18) + counts.PadLeft(13) + " " + MakeBar(ratio, 8) + overTag);
            } else {
                allLines.Add("  " + dn.PadRight(18) + FormatNumber(itemKv.Value).PadLeft(8) + overTag);
            }
        }
    }

    WritePaged(p, allLines, fixedPage);
}

void UpdateCraftDisplay(IMyTextPanel p, string filter) {
    ApplyPanelDefaults(p);

    bool hasFilter = !string.IsNullOrEmpty(filter);
    int fixedPage = -1; if (hasFilter) int.TryParse(filter, out fixedPage);
    string[] filterWords = (hasFilter && fixedPage <= 0) ? filter.ToLower().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries) : null;

    bool showActivity = true, showTargets = true;
    var catFilters = new List<string>();
    if (filterWords != null) {
        showActivity = false; showTargets = false;
        foreach (var w in filterWords) {
            if ("activity".Contains(w) || "assembler".Contains(w) || "assemblers".Contains(w)) showActivity = true;
            else if ("targets".Contains(w)) showTargets = true;
            else { showTargets = true; catFilters.Add(w); }
        }
        if (!showActivity && !showTargets) { showActivity = true; showTargets = true; }
    }

    var allLines = new List<string>();
    allLines.Add("=== " + (filterWords != null ? "Crafting: " + filter : "Auto-Crafting") + " ===");
    allLines.Add("Assemblers: " + assemblers.Count + " | Targets: " + craftTargets.Count);
    allLines.Add("");

    if (showActivity) {
        allLines.Add("--- Assembler Activity ---");
        foreach (var asm in assemblers) {
            string asmStatus = "Idle";
            var q = new List<MyProductionItem>();
            asm.GetQueue(q);
            if (asm.IsProducing && q.Count > 0) asmStatus = q[0].BlueprintId.SubtypeName + " x" + q[0].Amount;
            else if (q.Count > 0) asmStatus = "Waiting (" + q.Count + " queued)";
            string an = asm.CustomName; if (an.Length > 22) an = an.Substring(0, 22) + "..";
            allLines.Add("  " + an + ": " + asmStatus);
        }
        allLines.Add("");
    }

    if (showTargets) {
        if (craftTargets.Count == 0) {
            allLines.Add("No targets set.");
            allLines.Add("Edit [INV:Craft] CustomData: set values > 0");
        } else {
            allLines.Add("--- Craft Targets ---");
            var entries = new List<CraftEntry>();
            foreach (var kv in craftTargets) {
                string cat = GetCategory(kv.Key);
                int pri = 0; craftPriorities.TryGetValue(kv.Key, out pri);
                entries.Add(new CraftEntry { cat = cat, name = GetDisplayName(kv.Key), type = kv.Key, target = kv.Value, priority = pri });
            }
            entries.Sort(CompareCraftEntries);
            string lastCat = "";
            foreach (var e in entries) {
                if (catFilters.Count > 0) {
                    bool match = false;
                    string cl = e.cat.ToLower();
                    foreach (var w in catFilters) { if (cl.Contains(w)) { match = true; break; } }
                    if (!match) continue;
                }
                if (e.cat != lastCat) { allLines.Add("--- " + e.cat + " ---"); lastCat = e.cat; }
                double current = GetGlobalCount(e.type);
                double ratio = e.target > 0 ? Math.Min(current / e.target, 1.0) : 0;
                string counts = FormatNumber(current) + "/" + FormatNumber(e.target);
                string priStr = e.priority > 0 ? " P" + e.priority : "";
                allLines.Add("  " + e.name.PadRight(18) + counts.PadLeft(13) + " " + MakeBar(ratio, 8) + priStr);
            }
        }
    }

    WritePaged(p, allLines, fixedPage);
}

void UpdateCraftQueueDisplay(IMyTextPanel p) {
    ApplyPanelDefaults(p);
    var allLines = new List<string>();
    allLines.Add("=== Craft Queue ===");
    allLines.Add("");

    // Sum up queued amounts per blueprint across all assembly-mode assemblers
    var queued = new Dictionary<MyItemType, double>();
    foreach (var asm in assemblers) {
        if (!asm.IsWorking || asm.Mode != MyAssemblerMode.Assembly) continue;
        var q = new List<MyProductionItem>();
        asm.GetQueue(q);
        foreach (var qi in q) {
            // Match blueprint back to an itemType via craftTargets
            foreach (var ct in craftTargets) {
                string sub = ct.Key.SubtypeId;
                string bpSub = qi.BlueprintId.SubtypeId.ToString();
                // Blueprint subtype often contains the item subtype
                if (bpSub.Contains(sub) || sub.Contains(bpSub)) {
                    double cur; queued.TryGetValue(ct.Key, out cur);
                    queued[ct.Key] = cur + (double)qi.Amount;
                    break;
                }
            }
        }
    }

    if (queued.Count == 0 && craftTargets.Count == 0) {
        allLines.Add("No craft targets set.");
        allLines.Add("Add targets to [INV:Craft] LCD.");
        WritePaged(p, allLines, -1);
        return;
    }

    // Show all craft targets: queued to make + already in storage
    allLines.Add("Item".PadRight(20) + "Target".PadLeft(8) + "  Have".PadLeft(8) + "  Need".PadLeft(8));
    allLines.Add(new string('-', 46));

    // Sort by priority then name
    var sorted = new List<MyItemType>(craftTargets.Keys);
    sorted.Sort(CompareCraftQueueItems);

    foreach (var itemType in sorted) {
        double target = craftTargets[itemType];
        double have = GetGlobalCount(itemType);
        double need = Math.Max(0, target - have);
        double q = 0; queued.TryGetValue(itemType, out q);
        string name = GetDisplayName(itemType);
        if (name.Length > 19) name = name.Substring(0, 17) + "..";
        double ratio = target > 0 ? Math.Min(have / target, 1.0) : 0;
        string bar = MakeBar(ratio, 6);
        string needStr = need > 0 ? FormatNumber(need) : "ok";
        allLines.Add(name.PadRight(20) + FormatNumber(target).PadLeft(8)
            + FormatNumber(have).PadLeft(8) + needStr.PadLeft(8) + " " + bar);
    }

    allLines.Add("");
    int totalQueued = 0;
    foreach (var asm in assemblers) {
        if (!asm.IsWorking || asm.Mode != MyAssemblerMode.Assembly) continue;
        var q = new List<MyProductionItem>(); asm.GetQueue(q);
        totalQueued += q.Count;
    }
    allLines.Add("Assemblers: " + assemblers.Count + "  Queue slots: " + totalQueued);
    WritePaged(p, allLines, -1);
}


void UpdateSmeltDisplay(IMyTextPanel p, string filter) {
    ApplyPanelDefaults(p);

    int fixedPage = -1; if (!string.IsNullOrEmpty(filter)) int.TryParse(filter, out fixedPage);

    var allLines = new List<string>();
    allLines.Add("=== Ore Smelting ===");
    allLines.Add("");

    if (ingotTargets.Count > 0) {
        allLines.Add("--- Ingot Targets ---");
        var smeltEntries = new List<KeyValuePair<MyItemType, double>>(ingotTargets);
        smeltEntries.Sort(CompareSmeltEntries);
        foreach (var kv in smeltEntries) {
            double cur = GetGlobalCount(kv.Key);
            double tgt = kv.Value;
            double ratio = Math.Min(cur / tgt, 1.0);
            bool met = cur >= tgt;
            string name = GetDisplayName(kv.Key);
            string counts = FormatNumber(cur) + "/" + FormatNumber(tgt);
            int pri = 0; ingotPriorities.TryGetValue(kv.Key, out pri);
            string priStr = pri > 0 ? " P" + pri : "";
            allLines.Add("  " + name.PadRight(14) + counts.PadLeft(13) + " " + MakeBar(ratio, 8) + (met ? " ok" : " <<") + priStr);
        }
        allLines.Add("");
    } else {
        allLines.Add("No ingot targets set.");
        allLines.Add("Edit [INV:Smelt] CustomData to set targets.");
        allLines.Add("");
    }

    allLines.Add("--- Refineries (" + refineries.Count + ") ---");
    foreach (var r in refineries) {
        var inv = r.GetInventory(0);
        var items = new List<MyInventoryItem>();
        inv.GetItems(items);
        string oreName = "Empty";
        double amt = 0;
        foreach (var it in items) {
            if (!it.Type.TypeId.Contains(T_ORE)) continue;
            oreName = it.Type.SubtypeId;
            amt = (double)it.Amount;
            break;
        }
        string rn = r.CustomName; if (rn.Length > 20) rn = rn.Substring(0, 20);
        if (oreName == "Empty") allLines.Add("  " + rn.PadRight(20) + " Empty");
        else allLines.Add("  " + rn.PadRight(20) + " " + oreName + " (" + FormatNumber(amt) + ")");
    }

    WritePaged(p, allLines, fixedPage);
}

void UpdateStasisDisplay(IMyTextPanel p, string filter) {
    ApplyPanelDefaults(p);
    int fixedPage = -1; if (!string.IsNullOrEmpty(filter)) int.TryParse(filter, out fixedPage);
    var allLines = new List<string>();
    allLines.Add("=== Stasis Limits ===");
    allLines.Add("Items above limit will be disassembled.");
    allLines.Add("");
    if (stasisTargets.Count == 0) {
        allLines.Add("No stasis limits set.");
        allLines.Add("Edit [INV:Stasis] CustomData to add limits.");
        allLines.Add("Format: ItemName=MaxAmount");
    } else {
        var entries = new List<KeyValuePair<MyItemType, double>>(stasisTargets);
        entries.Sort((a, b) => GetDisplayName(a.Key).CompareTo(GetDisplayName(b.Key)));
        string lastCat = "";
        foreach (var kv in entries) {
            string cat = GetCategory(kv.Key);
            if (cat != lastCat) { allLines.Add("--- " + cat + " ---"); lastCat = cat; }
            double current = GetGlobalCount(kv.Key);
            double limit = kv.Value;
            double ratio = Math.Min(current / limit, 1.0);
            bool over = current > limit;
            string counts = FormatNumber(current) + "/" + FormatNumber(limit);
            string name = GetDisplayName(kv.Key);
            allLines.Add("  " + name.PadRight(18) + counts.PadLeft(13) + " " + MakeBar(ratio, 8) + (over ? " OVER" : ""));
        }
    }
    WritePaged(p, allLines, fixedPage);
}

void UpdatePowerDisplay(IMyTextPanel p) {
    ApplyPanelDefaults(p);

    var sb = new System.Text.StringBuilder();
    sb.AppendLine("=== Power & Gas ===");
    sb.AppendLine("");

    float totalStored = 0, totalMax = 0, totalInput = 0, totalOutput = 0;
    foreach (var b in batteries) {
        totalStored += b.CurrentStoredPower;
        totalMax += b.MaxStoredPower;
        totalInput += b.CurrentInput;
        totalOutput += b.CurrentOutput;
    }
    double chargePercent = totalMax > 0 ? (totalStored / totalMax) * 100 : 0;
    double ratio = totalMax > 0 ? (double)(totalStored / totalMax) : 0;

    // Total max power generation across all sources
    float reactorMax = 0, reactorCur = 0;
    foreach (var r in reactors) { if (r.IsWorking) { reactorMax += r.MaxOutput; reactorCur += r.CurrentOutput; } }
    float solarCur = 0, solarMax = 0;
    foreach (var s in solarPanels) { solarCur += s.CurrentOutput; solarMax += s.MaxOutput; }
    float h2Cur = 0, h2Max = 0;
    foreach (var e in hydrogenEngines) { var fb = e as IMyPowerProducer; if (fb != null) { h2Cur += fb.CurrentOutput; h2Max += fb.MaxOutput; } }
    float totalGenCur = reactorCur + solarCur + h2Cur + totalInput; // totalInput = battery charge input
    float totalGenMax = reactorMax + solarMax + h2Max;

    // Grid consumption = all output from batteries + all generation currently producing
    float gridConsumption = totalOutput + reactorCur + solarCur + h2Cur;

    sb.AppendLine("Mode: " + (powerDischargeMode ? "DISCHARGE" : "NORMAL"));
    sb.AppendLine("Battery: " + chargePercent.ToString("0.0") + "% " + MakeBar(ratio, 15));
    sb.AppendLine("  Stored:  " + FormatPower(totalStored * 1000) + " / " + FormatPower(totalMax * 1000));
    sb.AppendLine("  In:  " + FormatPower(totalInput * 1000) + "  Out: " + FormatPower(totalOutput * 1000));
    sb.AppendLine("Thresholds: " + powerLowerThreshold + "% - " + powerUpperThreshold + "%");
    sb.AppendLine("");
    sb.AppendLine("--- Power Generation ---");
    sb.AppendLine("Output:  " + FormatPower(gridConsumption * 1000));
    sb.AppendLine("Max Gen: " + FormatPower(totalGenMax * 1000));

    if (reactors.Count > 0) {
        int on = 0; foreach (var r in reactors) if (r.Enabled) on++;
        sb.AppendLine("Reactors: " + on + "/" + reactors.Count + "  " + FormatPower(reactorCur * 1000) + " / " + FormatPower(reactorMax * 1000));
    }
    if (solarPanels.Count > 0) {
        sb.AppendLine("Solar: " + solarPanels.Count + " panels  " + FormatPower(solarCur * 1000) + " / " + FormatPower(solarMax * 1000));
    }
    if (hydrogenEngines.Count > 0) {
        int on = 0; foreach (var e in hydrogenEngines) if ((e as IMyFunctionalBlock).Enabled) on++;
        sb.AppendLine("H2 Engines: " + on + "/" + hydrogenEngines.Count + "  " + FormatPower(h2Cur * 1000) + " / " + FormatPower(h2Max * 1000));
    }

    if (gasTanks.Count > 0) {
        sb.AppendLine("");
        sb.AppendLine("--- Gas ---");
        double h2Total = 0; int h2Count = 0;
        double o2Total = 0; int o2Count = 0;
        foreach (var t in gasTanks) {
            if (t.BlockDefinition.SubtypeId.Contains("Hydrogen")) { h2Total += t.FilledRatio; h2Count++; }
            else { o2Total += t.FilledRatio; o2Count++; }
        }
        if (h2Count > 0) {
            double h2Pct = (h2Total / h2Count) * 100;
            sb.AppendLine("H2: " + h2Pct.ToString("0.0") + "% " + MakeBar(h2Total / h2Count, 12) + " (" + h2Count + " tanks)");
        }
        if (o2Count > 0) {
            double o2Pct = (o2Total / o2Count) * 100;
            sb.AppendLine("O2: " + o2Pct.ToString("0.0") + "% " + MakeBar(o2Total / o2Count, 12) + " (" + o2Count + " tanks)");
        }
        if (generators.Count > 0) {
            int on = 0; foreach (var g in generators) if (g.Enabled) on++;
            sb.AppendLine("O2 Gens: " + on + "/" + generators.Count + (gasStockpileMode ? " OFF (full)" : " ON"));
            sb.AppendLine("Gas Thresholds: " + gasLowerThreshold + "% - " + gasUpperThreshold + "%");
        }
    }

    sb.AppendLine("");
    sb.AppendLine("--- Top Power Consumers ---");
    var consumers = new List<IMyFunctionalBlock>();
    GridTerminalSystem.GetBlocksOfType(consumers, b => b.IsSameConstructAs(Me) && b.Enabled);
    var powerList = new List<KeyValuePair<string, double>>();
    foreach (var fb in consumers) {
        double draw = GetBlockPowerDraw(fb);
        if (draw > 0.001) {
            string bn = fb.CustomName;
            if (bn.Length > 22) bn = bn.Substring(0, 22);
            powerList.Add(new KeyValuePair<string, double>(bn, draw));
        }
    }
    powerList.Sort((a, b) => b.Value.CompareTo(a.Value));
    int top = Math.Min(5, powerList.Count);
    for (int i = 0; i < top; i++) {
        sb.AppendLine("  " + powerList[i].Key.PadRight(22) + " " + FormatPower(powerList[i].Value));
    }
    if (powerList.Count == 0) sb.AppendLine("  (no power data)");

    p.WriteText(sb.ToString());
}

void UpdateLogDisplay(IMyTextPanel p) {
    ApplyPanelDefaults(p);
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("=== Inventory Manager Log ===");
    string step; string status;
    coreState.TryGetValue("step", out step);
    coreState.TryGetValue("status", out status);
    if (!string.IsNullOrEmpty(step) || !string.IsNullOrEmpty(status))
        sb.AppendLine("Step: " + step + " | " + status);
    if (lastCoreMsg == DateTime.MinValue)
        sb.AppendLine("IGC: waiting for PB1...");
    else
        sb.AppendLine("IGC: " + (DateTime.UtcNow - lastCoreMsg).TotalSeconds.ToString("0.0") + "s ago");
    sb.AppendLine("");

    int maxLines = GetVisibleLines(p) - 4;
    int count = Math.Min(coreLog.Count, maxLines);
    for (int i = 0; i < count; i++) sb.AppendLine(coreLog[i]);
    if (coreLog.Count == 0) sb.AppendLine("(no log entries yet)");
    p.WriteText(sb.ToString());
}

void UpdateWarningsDisplay(IMyTextPanel p) {
    ApplyPanelDefaults(p);

    var sb = new System.Text.StringBuilder();
    sb.AppendLine("=== Warnings & Issues ===");
    sb.AppendLine("");

    if (coreWarn.Count == 0) sb.AppendLine("  No issues detected.");
    else {
        for (int i = 0; i < coreWarn.Count; i++) sb.AppendLine((i + 1) + ". " + coreWarn[i]);
    }

    sb.AppendLine("");
    string blocks, items, inst, maxInst, rt;
    coreState.TryGetValue("blocks", out blocks);
    coreState.TryGetValue("items", out items);
    coreState.TryGetValue("instructions", out inst);
    coreState.TryGetValue("maxInst", out maxInst);
    coreState.TryGetValue("runtime", out rt);
    if (!string.IsNullOrEmpty(blocks) || !string.IsNullOrEmpty(items))
        sb.AppendLine("Blocks: " + blocks + " | Items: " + items);
    if (!string.IsNullOrEmpty(inst) || !string.IsNullOrEmpty(maxInst))
        sb.AppendLine("Instructions: " + inst + "/" + maxInst);
    if (!string.IsNullOrEmpty(rt))
        sb.AppendLine("Runtime: " + rt + "ms");

    if (lastCoreMsg != DateTime.MinValue) {
        double age = (DateTime.UtcNow - lastCoreMsg).TotalSeconds;
        sb.AppendLine("Core msg age: " + age.ToString("0.0") + "s");
    } else {
        sb.AppendLine("Core msg age: (none)");
    }

    p.WriteText(sb.ToString());
}

void UpdateDockedDisplay(IMyTextPanel p) {
    ApplyPanelDefaults(p);
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("=== Docked Ships ===");
    sb.AppendLine("");

    int shipCount = 0;
    int freeCount = 0;
    var seen = new HashSet<long>();

    foreach (var conn in connectors) {
        if (!conn.IsSameConstructAs(Me)) continue;
        if (conn.Status == MyShipConnectorStatus.Unconnected) { freeCount++; continue; }
        if (conn.Status != MyShipConnectorStatus.Connected || conn.OtherConnector == null) continue;
        var otherGrid = conn.OtherConnector.CubeGrid;
        if (otherGrid == Me.CubeGrid) continue;
        if (!seen.Add(otherGrid.EntityId)) continue;
        shipCount++;
        if (shipCount > 3) continue;

        string gn = otherGrid.CustomName;
        if (gn.Length > 25) gn = gn.Substring(0, 25);
        sb.AppendLine("--- " + gn + " ---");

        var blocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType(blocks, b => b.CubeGrid == otherGrid);

        float batStored = 0, batMax = 0; int batCnt = 0;
        double h2Fill = 0, o2Fill = 0; int h2Cnt = 0, o2Cnt = 0;
        long usedVol = 0, maxVol = 0;

        foreach (var b in blocks) {
            var bat = b as IMyBatteryBlock;
            if (bat != null) { batStored += bat.CurrentStoredPower; batMax += bat.MaxStoredPower; batCnt++; }
            var tank = b as IMyGasTank;
            if (tank != null) {
                if (tank.BlockDefinition.SubtypeId.Contains("Hydrogen")) { h2Fill += tank.FilledRatio; h2Cnt++; }
                else { o2Fill += tank.FilledRatio; o2Cnt++; }
            }
            if (b.HasInventory) {
                for (int i = 0; i < b.InventoryCount; i++) {
                    var inv = b.GetInventory(i);
                    usedVol += inv.CurrentVolume.RawValue;
                    maxVol += inv.MaxVolume.RawValue;
                }
            }
        }

        if (batCnt > 0) {
            double pct = batMax > 0 ? (batStored / batMax) * 100 : 0;
            sb.AppendLine("  Bat: " + pct.ToString("0.0") + "% " + MakeBar(batMax > 0 ? batStored / batMax : 0, 10) + " (" + batCnt + ")");
        }
        if (h2Cnt > 0) {
            double pct = (h2Fill / h2Cnt) * 100;
            sb.AppendLine("  H2:  " + pct.ToString("0.0") + "% " + MakeBar(h2Fill / h2Cnt, 10) + " (" + h2Cnt + ")");
        }
        if (o2Cnt > 0) {
            double pct = (o2Fill / o2Cnt) * 100;
            sb.AppendLine("  O2:  " + pct.ToString("0.0") + "% " + MakeBar(o2Fill / o2Cnt, 10) + " (" + o2Cnt + ")");
        }
        if (maxVol > 0) {
            double pct = (double)usedVol / maxVol * 100;
            sb.AppendLine("  Cargo: " + pct.ToString("0.0") + "% " + MakeBar((double)usedVol / maxVol, 10));
        }
        sb.AppendLine("");
    }

    if (shipCount == 0) sb.AppendLine("No ships docked.");
    else if (shipCount > 3) sb.AppendLine("(+" + (shipCount - 3) + " more ships)");
    sb.AppendLine("Free ports: " + freeCount);

    p.WriteText(sb.ToString());
}

// ======================== HELPERS ============================================

void ApplyPanelDefaults(IMyTextPanel p) {
    p.ContentType = ContentType.TEXT_AND_IMAGE;
    string font = defaultFont;
    float fontSize = defaultFontSize;
    float padding = defaultPadding;

    string data = p.CustomData;
    if (!string.IsNullOrWhiteSpace(data) && data.Contains("[Display]")) {
        bool inDisplay = false;
        foreach (var raw in data.Split('\n')) {
            string line = raw.Trim();
            if (line == "[Display]") { inDisplay = true; continue; }
            if (line.StartsWith("[")) { inDisplay = false; continue; }
            if (!inDisplay || line.StartsWith("#") || !line.Contains("=")) continue;
            var parts = line.Split(new char[]{'='}, 2);
            if (parts.Length != 2) continue;
            string key = parts[0].Trim();
            string val = parts[1].Trim();
            if (key == "Font" && val.Length > 0) font = val;
            else if (key == "FontSize") { float fs; if (float.TryParse(val, out fs) && fs > 0) fontSize = fs; }
            else if (key == "Padding") { float pd; if (float.TryParse(val, out pd) && pd >= 0) padding = pd; }
        }
    }

    p.Font = font;
    p.FontSize = fontSize;
    p.TextPadding = padding;
}

void WritePaged(IMyTextPanel p, List<string> lines, int fixedPage) {
    int per = GetVisibleLines(p);
    int totalPages = Math.Max(1, (int)Math.Ceiling((double)lines.Count / per));
    int page;
    if (fixedPage > 0) page = Math.Min(fixedPage - 1, totalPages - 1);
    else page = (int)(DateTime.UtcNow.TimeOfDay.TotalSeconds / 3) % totalPages;

    int start = page * per;
    int end = Math.Min(start + per, lines.Count);
    var sb = new System.Text.StringBuilder();
    for (int i = start; i < end; i++) sb.AppendLine(lines[i]);
    if (totalPages > 1) sb.AppendLine("Page " + (page + 1) + "/" + totalPages);
    p.WriteText(sb.ToString());
}

int GetVisibleLines(IMyTextPanel p) {
    float height = p.SurfaceSize.Y;
    float pad = p.TextPadding / 100f;
    float usable = height * (1f - 2f * pad);
    float lineHeight = p.FontSize * 28.8f;
    int lines = (int)(usable / lineHeight);
    return Math.Max(10, lines - 1);
}

string GetLcdFilter(string name, string tagPrefix) {
    int idx = name.IndexOf(tagPrefix);
    if (idx < 0) return null;
    int after = idx + tagPrefix.Length;
    if (after >= name.Length || name[after] == ']') return "";
    if (name[after] == ':') {
        int end = name.IndexOf(']', after);
        if (end > after) return name.Substring(after + 1, end - after - 1).Trim();
    }
    return "";
}

string GetCategory(MyItemType t) {
    string tid = t.TypeId;
    if (tid.Contains("Ore")) return "Ores";
    if (tid.Contains("Ingot")) return "Ingots";
    if (tid.Contains("Component")) return "Components";
    if (tid.Contains("AmmoMagazine")) return "Ammo";
    if (tid.Contains("OxygenContainer") || tid.Contains("GasContainer")) return "Bottles";
    if (tid.Contains("PhysicalGunObject")) return "Tools";
    if (tid.Contains("ConsumableItem")) return "Food";
    if (tid.Contains("Datapad")) return "Misc";
    return "Other";
}

string GetDisplayName(MyItemType t) {
    string custom;
    // Check full TypeId/SubtypeId key first — avoids ore/ingot collision
    if (customNames.TryGetValue(t.TypeId + "/" + t.SubtypeId, out custom)) return custom;
    // Check itemDb next — gives correct type-aware name (e.g. "Cobalt Ingot" vs "Cobalt Ore")
    ItemInfo info;
    if (itemDb.TryGetValue(t, out info)) return info.displayName;
    // SubtypeId-only custom name as last resort
    if (customNames.TryGetValue(t.SubtypeId, out custom)) return custom;
    // Fallback: derive from TypeId
    string sub = t.SubtypeId;
    if (t.TypeId.Contains("Ingot")) return sub == "Stone" ? "Gravel" : sub + " Ingot";
    if (t.TypeId.Contains("Ore") && !sub.Contains("Scrap")) return sub + " Ore";
    return sub;
}

string FormatNumber(double val) {
    double abs = Math.Abs(val);
    if (abs >= 1000000000) return (val / 1000000000).ToString("0.#") + "G";
    if (abs >= 1000000) return (val / 1000000).ToString("0.#") + "M";
    if (abs >= 10000) return (val / 1000).ToString("0.#") + "K";
    return val.ToString("#,0");
}

string MakeBar(double ratio, int width) {
    int filled = (int)(ratio * width + 0.5);
    if (filled < 0) filled = 0;
    if (filled > width) filled = width;
    return "[" + new string('\u2588', filled) + new string('\u2591', width - filled) + "]";
}

string FormatPower(double kw) {
    double abs = Math.Abs(kw);
    if (abs >= 1000000) return (kw / 1000000).ToString("0.##") + " GW";
    if (abs >= 1000) return (kw / 1000).ToString("0.##") + " MW";
    if (abs >= 1) return kw.ToString("0.#") + " kW";
    return (kw * 1000).ToString("0.#") + " W";
}

double GetBlockPowerDraw(IMyTerminalBlock block) {
    string info = block.DetailedInfo;
    if (string.IsNullOrEmpty(info)) return 0;
    var lines = info.Split('\n');
    foreach (var rawLine in lines) {
        string line = rawLine.Trim();
        int idx = line.IndexOf("Current Input:");
        if (idx < 0) idx = line.IndexOf("Required Input:");
        if (idx < 0) continue;
        int colon = line.IndexOf(':', idx);
        if (colon < 0) continue;
        string valPart = line.Substring(colon + 1).Trim();
        var parts = valPart.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) continue;
        double value;
        if (!double.TryParse(parts[0], out value)) continue;
        string unit = parts[1].ToLower();
        if (unit == "w") return value / 1000;
        if (unit == "kw") return value;
        if (unit == "mw") return value * 1000;
        if (unit == "gw") return value * 1000000;
    }
    return 0;
}

void UpdateEcho() {
    Echo("=== Inventory Manager (Display) ===");
    Echo("Runs: " + pb2Runs + " | Step: " + pb2Step);
    if (!string.IsNullOrEmpty(lastError)) {
        Echo("ERROR:\n" + lastError);
    }
    if (lastCoreMsg == DateTime.MinValue) Echo("Core: no IGC yet");
    else Echo("Core: last msg " + (DateTime.UtcNow - lastCoreMsg).TotalSeconds.ToString("0.0") + "s ago");
    Echo("Panels: " + panels.Count + " | Items counted: " + globalCounts.Count);
    Echo("Runtime: " + Runtime.LastRunTimeMs.ToString("0.000") + "ms");
}

// ======================== TEMPLATES ==========================================

void InitTagDefPanel(IMyTextPanel p) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Display]");
    sb.AppendLine("# Per-LCD overrides (change these to customize this panel)");
    sb.AppendLine("Font=" + defaultFont);
    sb.AppendLine("FontSize=" + defaultFontSize);
    sb.AppendLine("Padding=" + defaultPadding);
    sb.AppendLine("");
    sb.AppendLine("# ===================== CUSTOM TAG DEFINITIONS =====================");
    sb.AppendLine("# HOW THIS WORKS:");
    sb.AppendLine("#   1. This LCD panel MUST stay placed - the scripts read it every cycle");
    sb.AppendLine("#   2. Change [Tag:ExampleTag] below to your own name, e.g. [Tag:Ammo Bay]");
    sb.AppendLine("#   3. Name a container with that tag, e.g. 'Large Cargo [Ammo Bay]'");
    sb.AppendLine("#   4. Set item values to control what goes in that container:");
    sb.AppendLine("#        0     = do NOT sort this item to this tag");
    sb.AppendLine("#        500   = keep exactly 500 (quota) in this tagged container");
    sb.AppendLine("#        all   = sort ALL of this item to this container (unlimited)");
    sb.AppendLine("#   5. Priority sets which tag wins if an item matches multiple tags");
    sb.AppendLine("#      (higher number = higher priority)");
    sb.AppendLine("#");
    sb.AppendLine("# You can add multiple [Tag:Name] sections in this panel.");
    sb.AppendLine("# Copy the section below and change the name/values.");
    sb.AppendLine("");
    sb.AppendLine("[Tag:ExampleTag]");
    sb.AppendLine("Priority=0");

    var cats = new SortedDictionary<string, List<string>>();
    foreach (var kv in itemDb) {
        string cat = kv.Value.category;
        List<string> list;
        if (!cats.TryGetValue(cat, out list)) { list = new List<string>(); cats[cat] = list; }
        list.Add(kv.Value.displayName + "=0");
    }
    foreach (var kv in cats) {
        sb.AppendLine("# --- " + kv.Key + " ---");
        kv.Value.Sort();
        foreach (var line in kv.Value) sb.AppendLine(line);
    }
    sb.AppendLine("");
    sb.AppendLine("[Rename]");
    sb.AppendLine("# SubtypeId=Custom Name");
    p.CustomData = sb.ToString();
    ApplyPanelDefaults(p);
}

void InitStatusPanel(IMyTextPanel p, string filter) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Display]");
    sb.AppendLine("Font=" + defaultFont);
    sb.AppendLine("FontSize=" + defaultFontSize);
    sb.AppendLine("Padding=" + defaultPadding);
    sb.AppendLine("");
    sb.AppendLine("[Hide]");
    sb.AppendLine("# Items to hide (one per line)");
    sb.AppendLine("");
    sb.AppendLine("[Bottom]");
    sb.AppendLine("# Items listed here appear at the bottom of their category");
    sb.AppendLine("");
    sb.AppendLine("# SubtypeId:Display Name=Max Stock (0=no limit, supports K/M/G)");
    sb.AppendLine("# Change name after : to rename. Set number to cap stock (disassembles excess).");
    sb.AppendLine("");
    string[] filterWords = null;
    if (!string.IsNullOrEmpty(filter)) {
        int pg; if (!int.TryParse(filter, out pg))
            filterWords = filter.ToLower().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
    }
    var cats = new SortedDictionary<string, List<KeyValuePair<string, string>>>();
    foreach (var kv in itemDb) {
        string cat = kv.Value.category;
        if (filterWords != null) {
            bool match = false; string cl = cat.ToLower();
            foreach (var w in filterWords) { if (cl.Contains(w)) { match = true; break; } }
            if (!match) continue;
        }
        List<KeyValuePair<string, string>> list;
        if (!cats.TryGetValue(cat, out list)) { list = new List<KeyValuePair<string, string>>(); cats[cat] = list; }
        list.Add(new KeyValuePair<string, string>(kv.Key.SubtypeId, kv.Value.displayName));
    }
    foreach (var c in cats) c.Value.Sort((a, b) => a.Value.CompareTo(b.Value));
    foreach (var catKv in cats) {
        sb.AppendLine("# --- " + catKv.Key + " ---");
        foreach (var item in catKv.Value)
            sb.AppendLine(item.Key + ":" + item.Value + "=0");
        sb.AppendLine("");
    }
    p.CustomData = sb.ToString();
    ApplyPanelDefaults(p);
}

void InitCraftPanel(IMyTextPanel p) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Display]");
    sb.AppendLine("Font=" + defaultFont);
    sb.AppendLine("FontSize=" + defaultFontSize);
    sb.AppendLine("Padding=" + defaultPadding);
    sb.AppendLine("");
    sb.AppendLine("# Auto-Crafting Targets");
    sb.AppendLine("# SubtypeId:Display Name=Target");
    sb.AppendLine("# SteelPlate:Steel Plate=5000");
    sb.AppendLine("# Optional priority: SteelPlate=5000,P:2");
    sb.AppendLine("# Higher P: = crafted first in queue (P:3 before P:1)");
    sb.AppendLine("# No P: tag = P:0 (lowest, equal priority)");
    sb.AppendLine("");

    var cats = new SortedDictionary<string, List<string>>();
    foreach (var kv in itemDb) {
        if (!IsCraftable(kv.Key)) continue;
        List<string> list;
        if (!cats.TryGetValue(kv.Value.category, out list)) { list = new List<string>(); cats[kv.Value.category] = list; }
        list.Add(kv.Key.SubtypeId + ":" + kv.Value.displayName + "=0");
    }
    foreach (var kv in cats) {
        sb.AppendLine("# --- " + kv.Key + " ---");
        kv.Value.Sort();
        foreach (var line in kv.Value) sb.AppendLine(line);
        sb.AppendLine("");
    }
    sb.AppendLine("# --- Modded Items ---");
    sb.AppendLine("# TypeId/SubtypeId:Name=Target");

    p.CustomData = sb.ToString();
    ApplyPanelDefaults(p);
}

bool IsCraftable(MyItemType t) {
    string tid = t.TypeId;
    return tid.Contains(T_COMP) || tid.Contains(T_AMMO) || tid.Contains(T_GUN) || tid.Contains(T_FOOD) ||
           tid.Contains(T_OXYB) || tid.Contains(T_GASB) || tid.Contains(T_PHYS);
}

void InitSmeltPanel(IMyTextPanel p) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Display]");
    sb.AppendLine("Font=" + defaultFont);
    sb.AppendLine("FontSize=" + defaultFontSize);
    sb.AppendLine("Padding=" + defaultPadding);
    sb.AppendLine("");
    sb.AppendLine("# Ore Smelting Targets (Ingots)");
    sb.AppendLine("# Format: IngotName=Amount  or  IngotName=Amount,Priority");
    sb.AppendLine("# Supports K/M/G suffix: Iron=100M");
    sb.AppendLine("# Priority (optional): higher number = smelt first");
    sb.AppendLine("# Same priority ores spread across refineries");
    sb.AppendLine("# No priority = auto (by deficit ratio)");
    sb.AppendLine("# Set 0 to disable a target");
    sb.AppendLine("");
    sb.AppendLine("Iron=0");
    sb.AppendLine("Nickel=0");
    sb.AppendLine("Cobalt=0");
    sb.AppendLine("Silicon=0");
    sb.AppendLine("Silver=0");
    sb.AppendLine("Gold=0");
    sb.AppendLine("Platinum=0");
    sb.AppendLine("Uranium=0");
    sb.AppendLine("Magnesium=0");
    sb.AppendLine("Stone=0");
    p.CustomData = sb.ToString();
    ApplyPanelDefaults(p);
}

void InitStasisPanel(IMyTextPanel p) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Display]");
    sb.AppendLine("Font=" + defaultFont);
    sb.AppendLine("FontSize=" + defaultFontSize);
    sb.AppendLine("Padding=" + defaultPadding);
    sb.AppendLine("");
    sb.AppendLine("# Stasis (Max Stock) Thresholds");
    sb.AppendLine("# Items exceeding the limit will be disassembled.");
    sb.AppendLine("# Format: ItemName=MaxAmount (supports K/M/G suffixes)");
    sb.AppendLine("# Use SubtypeId or display name");
    sb.AppendLine("");
    sb.AppendLine("# --- Examples ---");
    sb.AppendLine("# SteelPlate=5K");
    sb.AppendLine("# Computer=2K");
    sb.AppendLine("# Motor=1K");
    sb.AppendLine("");
    var cats = new SortedDictionary<string, List<string>>();
    foreach (var kv in itemDb) {
        if (!IsCraftable(kv.Key)) continue;
        List<string> list;
        if (!cats.TryGetValue(kv.Value.category, out list)) { list = new List<string>(); cats[kv.Value.category] = list; }
        list.Add("# " + kv.Key.SubtypeId + "=0");
    }
    foreach (var kv in cats) {
        sb.AppendLine("# --- " + kv.Key + " ---");
        kv.Value.Sort();
        foreach (var line in kv.Value) sb.AppendLine(line);
    }
    p.CustomData = sb.ToString();
    ApplyPanelDefaults(p);
}

void InitPowerPanel(IMyTextPanel p) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Display]");
    sb.AppendLine("Font=" + defaultFont);
    sb.AppendLine("FontSize=" + defaultFontSize);
    sb.AppendLine("Padding=" + defaultPadding);
    sb.AppendLine("");
    sb.AppendLine("# Power & Gas Management Configuration");
    sb.AppendLine("UpperThreshold=95");
    sb.AppendLine("LowerThreshold=80");
    sb.AppendLine("GasUpperThreshold=95");
    sb.AppendLine("GasLowerThreshold=80");
    sb.AppendLine("# Ice to keep stocked in blocks tagged [Irrigation] (0 = disabled)");
    sb.AppendLine("IrrigationIce=5000");
    p.CustomData = sb.ToString();
    ApplyPanelDefaults(p);
}

void InitLoadoutPanel(IMyTextPanel p, string profileName) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Display]");
    sb.AppendLine("Font=" + defaultFont);
    sb.AppendLine("FontSize=" + defaultFontSize);
    sb.AppendLine("Padding=" + defaultPadding);
    sb.AppendLine("");
    sb.AppendLine("# Loadout Profile: " + profileName);
    sb.AppendLine("# Put [Loadout:" + profileName + "] on a connector name");
    sb.AppendLine("# When a ship docks, it will be unloaded then stocked");
    sb.AppendLine("# with the items listed below.");
    sb.AppendLine("# Set amount to 0 = don't stock, any number = stock that many");
    sb.AppendLine("# Fuel (uranium/ice) is handled separately by [Fuel] tag");
    sb.AppendLine("# Fuel overrides (with [Fuel] on same connector):");
    sb.AppendLine("# ReactorUranium=50");
    sb.AppendLine("# GeneratorIce=50000");
    sb.AppendLine("");

    var cats = new SortedDictionary<string, List<string>>();
    foreach (var kv in itemDb) {
        string cat = kv.Value.category;
        if (cat == "Ores" || cat == "Ingots") continue;
        List<string> list;
        if (!cats.TryGetValue(cat, out list)) { list = new List<string>(); cats[cat] = list; }
        list.Add(kv.Value.displayName + "=0");
    }
    foreach (var kv in cats) {
        sb.AppendLine("# --- " + kv.Key + " ---");
        kv.Value.Sort();
        foreach (var line in kv.Value) sb.AppendLine(line);
        sb.AppendLine("");
    }
    sb.AppendLine("# --- Ores/Ingots (if needed) ---");
    sb.AppendLine("# Ice=0");
    sb.AppendLine("# Iron Ingot=0");

    p.CustomData = sb.ToString();
    ApplyPanelDefaults(p);
}

void InitSimplePanel(IMyTextPanel p) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Display]");
    sb.AppendLine("Font=" + defaultFont);
    sb.AppendLine("FontSize=" + defaultFontSize);
    sb.AppendLine("Padding=" + defaultPadding);
    p.CustomData = sb.ToString();
    ApplyPanelDefaults(p);
}

void InitDockedPanel(IMyTextPanel p) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Display]");
    sb.AppendLine("Font=" + defaultFont);
    sb.AppendLine("FontSize=" + defaultFontSize);
    sb.AppendLine("Padding=" + defaultPadding);
    sb.AppendLine("");
    sb.AppendLine("# Docked Ships Monitor");
    sb.AppendLine("# Shows battery, gas, and cargo for connected ships");
    sb.AppendLine("# Max 3 ships displayed per cycle");
    p.CustomData = sb.ToString();
    ApplyPanelDefaults(p);
}

void PrePopulateItemDb() {
    string[] comps = { "BulletproofGlass","Canvas","Computer","Construction","Detector","Display",
        "Explosives","Girder","GravityGenerator","InteriorPlate","LargeTube","Medical","MetalGrid",
        "Motor","PowerCell","RadioCommunication","Reactor","SmallTube","SolarCell","SteelPlate",
        "Superconductor","Thrust","ZoneChip" };
    foreach (var s in comps) itemDb[new MyItemType(OB+T_COMP, s)] = new ItemInfo(new MyItemType(OB+T_COMP, s));
    string[] ammo = { "NATO_25x184mm","Missile200mm","AutocannonClip","MediumCalibreAmmo",
        "LargeCalibreAmmo","SmallRailgunAmmo","LargeRailgunAmmo","SemiAutoPistolMagazine",
        "ElitePistolMagazine","FullAutoPistolMagazine","AutomaticRifleGun_Mag_20rd",
        "RapidFireAutomaticRifleGun_Mag_50rd","PreciseAutomaticRifleGun_Mag_5rd",
        "UltimateAutomaticRifleGun_Mag_30rd" };
    foreach (var s in ammo) itemDb[new MyItemType(OB+T_AMMO, s)] = new ItemInfo(new MyItemType(OB+T_AMMO, s));
    string[] guns = { "AngleGrinderItem","AngleGrinder2Item","AngleGrinder3Item","AngleGrinder4Item",
        "HandDrillItem","HandDrill2Item","HandDrill3Item","HandDrill4Item",
        "WelderItem","Welder2Item","Welder3Item","Welder4Item",
        "AutomaticRifleItem","RapidFireAutomaticRifleItem","PreciseAutomaticRifleItem",
        "UltimateAutomaticRifleItem","SemiAutoPistolItem","FullAutoPistolItem","ElitePistolItem",
        "BasicHandHeldLauncherItem","AdvancedHandHeldLauncherItem" };
    foreach (var s in guns) itemDb[new MyItemType(OB+T_GUN, s)] = new ItemInfo(new MyItemType(OB+T_GUN, s));

    itemDb[new MyItemType(OB+T_OXYB, "OxygenBottle")] = new ItemInfo(new MyItemType(OB+T_OXYB, "OxygenBottle"));
    itemDb[new MyItemType(OB+T_GASB, "HydrogenBottle")] = new ItemInfo(new MyItemType(OB+T_GASB, "HydrogenBottle"));

    foreach (var s in new[]{ "Medkit","Powerkit" }) itemDb[new MyItemType(OB+T_FOOD, s)] = new ItemInfo(new MyItemType(OB+T_FOOD, s));

    foreach (var s in new[]{ "Iron","Nickel","Cobalt","Silicon","Silver","Gold","Platinum","Uranium","Magnesium","Stone","Ice","Scrap" })
        itemDb[new MyItemType(OB+T_ORE, s)] = new ItemInfo(new MyItemType(OB+T_ORE, s));

    foreach (var s in new[]{ "Iron","Nickel","Cobalt","Silicon","Silver","Gold","Platinum","Uranium","Magnesium","Stone" })
        itemDb[new MyItemType(OB+T_INGOT, s)] = new ItemInfo(new MyItemType(OB+T_INGOT, s));
}

int CompareCraftQueueItems(MyItemType a, MyItemType b) {
    int pa = 0, pb = 0;
    craftPriorities.TryGetValue(a, out pa);
    craftPriorities.TryGetValue(b, out pb);
    if (pb != pa) return pb.CompareTo(pa);
    return GetDisplayName(a).CompareTo(GetDisplayName(b));
}
int CompareCatItems(KeyValuePair<MyItemType, double> a, KeyValuePair<MyItemType, double> b) {
    string na = GetDisplayName(a.Key), nb = GetDisplayName(b.Key);
    int ia = _sortBottomItems.IndexOf(na), ib = _sortBottomItems.IndexOf(nb);
    if (ia < 0) ia = _sortBottomItems.IndexOf(a.Key.SubtypeId);
    if (ib < 0) ib = _sortBottomItems.IndexOf(b.Key.SubtypeId);
    if (ia >= 0 && ib >= 0) return ia.CompareTo(ib);
    if (ia >= 0) return 1;
    if (ib >= 0) return -1;
    return na.CompareTo(nb);
}
int CompareCraftEntries(CraftEntry a, CraftEntry b) {
    if (b.priority != a.priority) return b.priority.CompareTo(a.priority);
    int c = a.cat.CompareTo(b.cat);
    if (c != 0) return c;
    return a.name.CompareTo(b.name);
}
int CompareSmeltEntries(KeyValuePair<MyItemType, double> a, KeyValuePair<MyItemType, double> b) {
    int priA = 0, priB = 0;
    ingotPriorities.TryGetValue(a.Key, out priA);
    ingotPriorities.TryGetValue(b.Key, out priB);
    if (priA != priB) return priB.CompareTo(priA);
    return GetDisplayName(a.Key).CompareTo(GetDisplayName(b.Key));
}
class ItemInfo {
    public MyItemType type;
    public string displayName;
    public string category;
    public ItemInfo(MyItemType type) {
        this.type = type;
        this.category = "Other";
        string sub = type.SubtypeId;
        string name;
        if (DISPLAY_NAMES.TryGetValue(sub, out name)) displayName = name;
        else if (type.TypeId.Contains("Ore") && !sub.Contains("Scrap")) displayName = sub + " Ore";
        else if (type.TypeId.Contains("Ingot")) displayName = sub == "Stone" ? "Gravel" : sub + " Ingot";
        else {
            var csb = new System.Text.StringBuilder();
            for (int i = 0; i < sub.Length; i++) {
                if (i > 0 && char.IsUpper(sub[i]) && i < sub.Length - 1
                    && (!char.IsUpper(sub[i - 1]) || !char.IsUpper(sub[Math.Min(i + 1, sub.Length - 1)])))
                    csb.Append(' ');
                csb.Append(sub[i]);
            }
            displayName = csb.ToString();
        }
        string tid = type.TypeId;
        if (tid.Contains("Ore")) category = "Ores";
        else if (tid.Contains("Ingot")) category = "Ingots";
        else if (tid.Contains("Component")) category = "Components";
        else if (tid.Contains("AmmoMagazine")) category = "Ammo";
        else if (tid.Contains("OxygenContainer") || tid.Contains("GasContainer")) category = "Bottles";
        else if (tid.Contains("PhysicalGunObject")) category = "Tools";
        else if (tid.Contains("ConsumableItem")) category = "Food";
        else if (tid.Contains("Datapad")) category = "Misc";
    }
}

struct CraftEntry {
    public string cat;
    public string name;
    public MyItemType type;
    public double target;
    public int priority;
}

