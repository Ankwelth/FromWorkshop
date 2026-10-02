// ============================================================================
//  SPACE ENGINEERS INVENTORY MANAGER — FULL REFERENCE
//  PB3: Automation + Visual Engine (Solar, Doors, Airlocks, Fancy Displays,
//       Floor Plan). Optional IGC from PB1 for status overlays.
// ============================================================================
//
//  THREE PROGRAMMABLE BLOCKS:
//    PB1 = Core Engine — sorting, crafting, disassembly, machines, fuel, loadouts
//    PB2 = Display Engine — LCD rendering, CustomData templates
//    PB3 (this) = Automation + Visual — solar, doors, airlocks, sprites, floor plan
//
// ========================= QUICK REFERENCE ===================================
//
//  CONTAINER TAGS  (put in block name)
//    [Ore]        Ore storage
//    [Ingot]      Ingot/refined material storage
//    [Component]  Component storage
//    [Ammo]       Ammo storage
//    [Tool]       Tool storage
//    [Bottle]     Oxygen/hydrogen bottle storage
//    [Food]       Consumable item storage
//    [Overflow]   Catch-all for items with no tagged destination
//    [P:N]        Container priority — higher N fills first (e.g. [P:5])
//
//  BLOCK MODIFIER TAGS  (put in block name)
//    [Manual]     Exclude block from ALL script management (sorting, crafting, etc.)
//    [Locked]     Exclude block from all inventory operations entirely
//    [Hidden]     Block's items won't appear in inventory counts or displays
//    [Learn]      Assembler will learn new blueprint recipes from its output tray
//                 and add them to the [INV:Craft] LCD automatically
//
//  LCD PANEL TAGS  (put in LCD name — PB2 auto-fills CustomData template)
//    [INV:TagDef]          Custom tag definitions + item renaming
//    [INV:Craft]           Auto-crafting targets — set amounts in CustomData
//    [INV:Craft:Category]  Filtered craft display (filter by category name)
//    [INV:CraftQueue]      Shows all craft targets with Target/Have/Need columns
//                          and a progress bar. Optional — does not affect crafting.
//    [INV:Smelt]           Ore smelting targets with optional refinery priority
//    [INV:Status]          Full inventory overview. Add a [Stasis] section to
//                          set auto-disassembly limits (see AUTO-DISASSEMBLY below)
//                          PAGING: Status display auto-pages every 3 seconds.
//                          To pin to a specific page, use [INV:Status:1], [INV:Status:2]
//                          etc. in the LCD name. Page 1 = first screen of items.
//    [INV:Status:Category] Filtered status display (filter by category name,
//                          e.g. [INV:Status:Components] or [INV:Status:Ammo])
//    [INV:Power]           Power & gas management display + threshold config
//    [INV:Log]             PB1 action log
//    [INV:Warnings]        PB1 warnings
//    [INV:Loadout:Name]    Loadout profile — defines items to stock on docked ships
//    [INV:Solar]           PB3 solar alignment stats
//    [INV:Fancy]           PB3 sprite-based status bars
//    [INV:Floor]           PB3 floor plan view
//
//  CONNECTOR TAGS  (put in connector name, either side)
//    [Fuel]           Auto-fuel docked ship (recharge batteries, stockpile tanks,
//                     stock uranium/ice). Auto-disconnects at 98%.
//    [Loadout:Name]   Unload docked ship then restock per [INV:Loadout:Name] LCD
//    [Loadout:Tags]   Unload + stock per custom tag quotas on the ship (no LCD needed)
//    [NoSort]         Don't sort items on the connected grid
//    [NoManage]       Don't manage connected grid at all
//
//  AUTO-CRAFTING  (in [INV:Craft] LCD CustomData)
//    Format:  SubtypeId=Amount  or  SubtypeId:DisplayName=Amount
//    Example: SteelPlate=5000
//             Computer=2000
//
//    PRIORITY (optional):  SubtypeId=Amount,P:N
//    Example: SteelPlate=5000,P:3
//             Computer=2000,P:2
//             Motor=1000,P:1
//             SmallTube=3000          (no P: = P:0, lowest priority)
//
//    Higher P: items appear first in every assembler's queue so they are
//    worked on first. Items at the same priority are sub-sorted by how far
//    below their target they are. All assemblers get equal shares of every
//    item — each assembler holds ceil(deficit / assemblerCount) of each item.
//    Seeds (Wheat_Seed, Corn_Seed, etc.) are supported as craft targets.
//
//  AUTO-DISASSEMBLY  ([Stasis] section in [INV:Status] LCD CustomData)
//    Add a [Stasis] section below [Display]:
//      [Stasis]
//      SteelPlate=5000000
//      Computer=500000
//    Any item exceeding its stasis limit gets queued for disassembly.
//    While crafting is active: 1 assembler handles disassembly.
//    While nothing needs crafting: ALL assemblers switch to disassembly.
//    Assemblers return to assembly mode automatically when disassembly is done.
//
//  SMELT PRIORITY  (in [INV:Smelt] LCD CustomData)
//    Format:  IngotName=Amount  or  IngotName=Amount,Priority
//    Example: Iron=100M
//             Platinum=50K,3    (higher number = refined first)
//    Same-priority ores are spread evenly across refineries.
//
//  LOADOUTS  (in [INV:Loadout:Name] LCD CustomData)
//    Steel Plate=500   Gatling Ammo=200   (0 = don't stock this item)
//    Fuel is handled separately — add [Fuel] tag to the connector.
//    Fuel overrides (on same [Fuel] connector):
//      ReactorUranium=50
//      GeneratorIce=50000
//
//  TAG-BASED LOADOUTS  ([Loadout:Tags] on connector)
//    No LCD needed. Containers on the docked ship tagged with [MyTag]
//    get stocked according to the quota defined in [INV:TagDef].
//
//  CUSTOM TAGS  (in [INV:TagDef] LCD CustomData)
//    Define a [Tag:MyTag] section with item=quota lines.
//    Any container named [MyTag] receives those items up to quota.
//    Items in custom tags are hidden from global counts.
//    Deficits in custom tags are automatically added to craft targets.
//
//  SHIP FUELING  ([Fuel] on connector)
//    Batteries → set to recharge mode
//    Gas tanks → set to stockpile mode
//    Reactors → topped up with uranium (up to uraniumPerLargeReactor limit)
//    Generators → topped up with ice (up to icePerLargeGenerator limit)
//    Auto-disconnects when ship reaches 98% on all systems.
//    Combine [Fuel] + [Loadout:Name] on the same connector for full service.
//
//  DISPLAY OPTIONS  (in any LCD CustomData, under [Display] section)
//    Font=Monospace
//    FontSize=0.65
//    Padding=2
//    Add a [Hide] section to hide specific items or categories from that LCD.
//    Add a [Rename] section to override display names: SubtypeId=My Name
//
//  POWER MANAGEMENT  (in [INV:Power] LCD CustomData)
//    UpperThreshold=95      Switch to discharge mode above this battery %
//    LowerThreshold=80      Return to normal mode below this battery %
//    GasUpperThreshold=95   Turn generators off above this H2 tank %
//    GasLowerThreshold=80   Turn generators on below this H2 tank %
//    IrrigationIce=5000     Ice to keep stocked in [Irrigation] blocks (0=off)
//
//  IRRIGATION SYSTEM  (tag any block that needs ice in its inventory)
//    Add [Irrigation] to the block name. PB1 will keep it stocked with ice
//    up to the IrrigationIce amount set in [INV:Power] CustomData.
//    Default: 5000 ice. Set IrrigationIce=0 to disable.
//    Works with any block that has an inventory, regardless of mod.
//
//  CARGO FILL LEVEL  (automatic — no config needed)
//    All tagged containers ([Ore], [Ingot], [Component] etc.) automatically
//    show their fill percentage in their block name, e.g.:
//      "Main Ore Storage [Ore] [P:5] (73.4%)"
//    This updates every cycle. To disable, set showFillLevel=false in PB1.
//
//  PB1 COMMANDS  (run argument on PB1)
//    rescan    Force immediate block rescan
//    reset     Full reset — clears blueprint cache and all queues
//
//  PB2 COMMANDS  (run argument on PB2)
//    rescan    Force immediate block rescan
//
//  PB3 COMMANDS  (run argument on PB3)
//    pause             Pause all PB3 automation
//    solar_pause       Pause solar tracking only
//    solar_resume      Resume solar tracking
//    rescan            Force immediate block rescan
//    floor_up          Move floor plan view up one level
//    floor_down        Move floor plan view down one level
//    floor_zoom_in     Zoom in on floor plan
//    floor_zoom_out    Zoom out on floor plan
//    floor_reset       Reset floor plan to default level and zoom
//
//  PB3 CONFIG  (in PB3 CustomData)
//    [Solar]
//    RotorSpeed=0.3
//    NightThreshold=10
//    RealignPercent=2
//
//    [Doors]
//    CloseDelay=3
//    IncludeHangarDoors=false
//
//  AIRLOCKS  (by naming convention)
//    Doors must contain "Interior" or "Exterior" AND "Airlock" in their name.
//    Group by shared prefix:
//      "Bay1 Airlock Interior Door"
//      "Bay1 Airlock Exterior Door"
//    Or group by tag: add [Airlock:Bay1] to each door in the group.
//    When one side opens, the other side locks. Both re-enable on close.
//    Optional: lights named "Bay1 Airlock Light" turn red when open.
//    Optional: sound blocks named "Bay1 Airlock Sound" play when open.
//    Exclude doors with [NoDoor] or [Manual] in their name.
//
//  FANCY DISPLAY  (in [INV:Fancy] LCD CustomData)
//    ShowStats then list what to show, one per line:
//      Battery, Hydrogen Tank, Oxygen Tank, Reactor, Solar,
//      Cargo, Hydrogen Engine, Wind, O2 Generator, <group/block name>
//    Modifiers (add after item name): widebar, optional, nosubgrids
//    Use --- to start a plain text section.
//
//  FLOOR PLAN  (in [INV:Floor] LCD CustomData)
//    [Floor Plan]
//    Level=0       Y-offset from PB (0=same level, +1=above, -1=below)
//    Scale=auto    Pixels per block (auto fits to LCD size)
//
// ============================================================================


// --- Performance ---
// Server limit is 1.0ms — keep maxRuntimeMs below that
double maxRuntimeMs = 0.75;
bool enableAutoThrottle = true;
int throttleSkip = 0;

// --- Constants ---
const string IGC_CHANNEL = "INV_DATA";
const string LCD_SOLAR = "[INV:Solar]";
const string LCD_FANCY = "[INV:Fancy]";
const string LCD_FLOOR = "[INV:Floor]";
const string TAG_NO_DOOR = "[NoDoor]";
const string TAG_MANUAL = "[Manual]";
const string TAG_SOLAR = "[Solar]";
const string SOLAR_GROUP_NAME = "Solar Rotors";
const int SPRITE_BUDGET = 980;
const int FLOOR_YIELD = 50;

// --- Sprite Colors ---
static readonly Color BG = new Color(0, 0, 0);
static readonly Color BAR_BG = new Color(25, 25, 30);
static readonly Color BORDER = new Color(50, 55, 60);
static readonly Color TXT = new Color(220, 220, 220);
static readonly Color DIM = new Color(120, 120, 130);
static readonly Color TITLE_CLR = new Color(80, 170, 255);
static readonly Color WARN_CLR = new Color(255, 180, 40);
static readonly Color FL_LIGHT = new Color(90, 90, 95, 210);
static readonly Color FL_HEAVY = new Color(55, 55, 60, 210);
static readonly Color FL_DOOR = new Color(130, 95, 45, 210);
static readonly Color FL_GLASS = new Color(110, 190, 245, 140);
static readonly Color FL_CONV = new Color(150, 130, 35, 190);
static readonly Color FL_FUNC = new Color(55, 170, 55, 190);
static readonly Color FL_POWER = new Color(55, 210, 95, 190);
static readonly Color FL_THRUST = new Color(65, 130, 245, 190);
static readonly Color FL_WPN = new Color(245, 130, 0, 190);
static readonly Color FL_CARGO = new Color(170, 130, 55, 190);
static readonly Color FL_DMG = new Color(245, 245, 0, 200);

// --- Door Config ---
float doorCloseDelay = 3f;
bool includeHangarDoors = false;

// --- Solar Config ---
float solarRotorSpeed = 0.3f;
float nightThreshold = 10f;
float realignPercent = 2f;

// --- Power Fallback Config ---
bool useReactorFallback = true;
bool useH2Fallback = true;
float lowBatteryPct = 10f;
float overloadPct = 90f;

// ======================== CORE FIELDS ========================================

List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyReactor> reactors = new List<IMyReactor>();
List<IMyTerminalBlock> h2Engines = new List<IMyTerminalBlock>();
List<IMySolarPanel> solarPanels = new List<IMySolarPanel>();
List<IMyGasTank> gasTanks = new List<IMyGasTank>();
List<IMyGasGenerator> o2Gens = new List<IMyGasGenerator>();
List<IMyTerminalBlock> cargoBlocks = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> windTurbines = new List<IMyTerminalBlock>();

List<IMyDoor> doors = new List<IMyDoor>();
Dictionary<long, DateTime> doorOpenTimes = new Dictionary<long, DateTime>();
Dictionary<long, float> doorCustomDelay = new Dictionary<long, float>();
Dictionary<string, AirlockGroup> airlocks = new Dictionary<string, AirlockGroup>();

List<IMyMotorStator> solarRotors = new List<IMyMotorStator>();
float lastSolarOutput = 0;
float bestSolarOutput = 0;
int solarDir = 1;
bool nightMode = false;
string solarStatus = "INIT";
bool fallbackActive = false;

List<IMyTextPanel> solarLcds = new List<IMyTextPanel>();
List<IMyTextPanel> fancyPanels = new List<IMyTextPanel>();

IMyBroadcastListener coreListener;
Dictionary<string, string> coreState = new Dictionary<string, string>();
List<string> coreWarn = new List<string>();

int floorLevel = 0;
float floorZoom = 0;
IEnumerator<int> floorCo = null;
bool floorActive = false;
bool floorReady = false;
bool floorRescan = true;
int floorTick = 0;
List<FloorCell> floorCells = new List<FloorCell>();
int fMinX, fMaxX, fMinZ, fMaxZ;

int tick = 0;
string lastError = "";
string pb3Step = "Init";
int pb3Runs = 0;
int startupDelayTicks = 20; // first start warmup (~32s at Update100)

// ======================== CONSTRUCTOR ========================================

Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    coreListener = IGC.RegisterBroadcastListener(IGC_CHANNEL);
    coreListener.SetMessageCallback(IGC_CHANNEL);
    ParseConfig();
    ScanAll();
}

// ======================== MAIN ENTRY =========================================

void Main(string argument, UpdateType updateSource) {
    if (startupDelayTicks > 0) { startupDelayTicks--; return; }
    if (enableAutoThrottle && throttleSkip > 0) { throttleSkip--; return; }
    pb3Runs++;
    try {
        tick++;
        if (!string.IsNullOrEmpty(argument)) HandleArg(argument.ToLower().Trim());

        pb3Step = "Doors"; CheckDoorTimers();
        if (tick % 2 == 0) { pb3Step = "Airlocks"; UpdateAirlocks(); }
        if (tick % 2 == 0) { pb3Step = "Solar"; SolarStep(); }
        if (tick % 6 == 0) { pb3Step = "SolarLCD"; UpdateSolarLcds(); }

        if (tick % 2 == 0) {
            pb3Step = "IGC"; ProcessIgc();
            floorTick++;
            if (floorTick >= 12) { floorRescan = true; floorTick = 0; }
            pb3Step = "Floor"; RunFloorStep();
            pb3Step = "Fancy";
            foreach (var p in fancyPanels) {
                if (p.CustomName.Contains(LCD_FANCY)) RenderFancy(p);
                else if (p.CustomName.Contains(LCD_FLOOR) && floorReady) RenderFloor(p);
            }
        }

        if (tick % 12 == 0) { pb3Step = "Rescan"; ScanAll(); ParseConfig(); }
        pb3Step = "OK";
        lastError = "";
    } catch (Exception e) {
        lastError = e.Message + "\n" + e.StackTrace;
    }

    if (enableAutoThrottle && Runtime.LastRunTimeMs > maxRuntimeMs)
        throttleSkip = (int)(Runtime.LastRunTimeMs / maxRuntimeMs) + 1;
    UpdateEcho();
}

void HandleArg(string arg) {
    if (arg == "pause" || arg == "solar_pause") {
        solarStatus = solarStatus == "PAUSED" ? "TRACKING" : "PAUSED";
        if (solarStatus == "PAUSED")
            foreach (var r in solarRotors) r.TargetVelocityRPM = 0;
    } else if (arg == "solar_resume") {
        solarStatus = "TRACKING";
    } else if (arg == "rescan") {
        ScanAll();
    } else if (arg == "floor_up") { floorLevel++; floorRescan = true; }
    else if (arg == "floor_down") { floorLevel--; floorRescan = true; }
    else if (arg == "floor_zoom_in") { floorZoom = Math.Max(2, (floorZoom <= 0 ? 8 : floorZoom) + 2); }
    else if (arg == "floor_zoom_out") { floorZoom = Math.Max(2, (floorZoom <= 0 ? 8 : floorZoom) - 2); }
    else if (arg == "floor_reset") { floorLevel = 0; floorZoom = 0; floorRescan = true; }
}

// ======================== CONFIG =============================================

void ParseConfig() {
    string data = Me.CustomData;
    if (string.IsNullOrWhiteSpace(data) || !data.Contains("[")) { InitConfig(); data = Me.CustomData; }
    string section = "";
    foreach (var raw in data.Split('\n')) {
        string line = raw.Trim();
        if (line.StartsWith("[")) { section = line; continue; }
        if (line.StartsWith("#") || !line.Contains("=")) continue;
        var parts = line.Split(new char[]{'='}, 2);
        if (parts.Length != 2) continue;
        string k = parts[0].Trim(), v = parts[1].Trim();
        if (section == "[Solar]") {
            if (k == "RotorSpeed") float.TryParse(v, out solarRotorSpeed);
            else if (k == "NightThreshold") float.TryParse(v, out nightThreshold);
            else if (k == "RealignPercent") float.TryParse(v, out realignPercent);
        } else if (section == "[Power Fallback]") {
            if (k == "UseReactors") bool.TryParse(v, out useReactorFallback);
            else if (k == "UseH2Engines") bool.TryParse(v, out useH2Fallback);
            else if (k == "LowBatteryPercent") float.TryParse(v, out lowBatteryPct);
            else if (k == "OverloadPercent") float.TryParse(v, out overloadPct);
        } else if (section == "[Doors]") {
            if (k == "CloseDelay") float.TryParse(v, out doorCloseDelay);
            else if (k == "IncludeHangarDoors") bool.TryParse(v, out includeHangarDoors);
        }
    }
}

void InitConfig() {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("# PB3: Automation + Visual Engine Configuration");
    sb.AppendLine("");
    sb.AppendLine("[Solar]");
    sb.AppendLine("RotorSpeed=0.3");
    sb.AppendLine("NightThreshold=10");
    sb.AppendLine("RealignPercent=2");
    sb.AppendLine("");
    sb.AppendLine("[Doors]");
    sb.AppendLine("CloseDelay=3");
    sb.AppendLine("IncludeHangarDoors=false");
    Me.CustomData = sb.ToString();
}

// ======================== SCANNING ===========================================

void ScanAll() {
    solarRotors.Clear();
    var group = GridTerminalSystem.GetBlockGroupWithName(SOLAR_GROUP_NAME);
    if (group != null) {
        var rl = new List<IMyMotorStator>();
        group.GetBlocksOfType(rl);
        solarRotors.AddRange(rl);
    }
    var extra = new List<IMyMotorStator>();
    GridTerminalSystem.GetBlocksOfType(extra, r => r.IsSameConstructAs(Me) && r.CustomName.Contains(TAG_SOLAR));
    foreach (var r in extra) if (!solarRotors.Contains(r)) solarRotors.Add(r);

    solarPanels.Clear();
    GridTerminalSystem.GetBlocksOfType(solarPanels, p => p.IsSameConstructAs(Me));

    batteries.Clear(); reactors.Clear(); h2Engines.Clear();
    GridTerminalSystem.GetBlocksOfType(batteries, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(reactors, r => r.IsSameConstructAs(Me) && !r.CustomName.Contains(TAG_MANUAL));
    var hp = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType(hp, p => p.IsSameConstructAs(Me)
        && !p.CustomName.Contains(TAG_MANUAL) && p.BlockDefinition.SubtypeId.Contains("HydrogenEngine"));
    foreach (var p in hp) h2Engines.Add(p);

    gasTanks.Clear(); o2Gens.Clear();
    GridTerminalSystem.GetBlocksOfType(gasTanks, t => t.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(o2Gens, g => g.IsSameConstructAs(Me));

    cargoBlocks.Clear();
    var allCargo = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(allCargo, b => b.IsSameConstructAs(Me) && b.HasInventory
        && !(b is IMyReactor) && !(b is IMyGasGenerator));
    cargoBlocks.AddRange(allCargo);

    windTurbines.Clear();
    var wp = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType(wp, p => p.IsSameConstructAs(Me) && p.BlockDefinition.SubtypeId.Contains("Wind"));
    foreach (var p in wp) windTurbines.Add(p);

    ScanDoors();

    solarLcds.Clear(); fancyPanels.Clear();
    var allPanels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(allPanels, p => p.IsSameConstructAs(Me));
    foreach (var p in allPanels) {
        string n = p.CustomName;
        if (n.Contains(LCD_SOLAR)) solarLcds.Add(p);
        if (n.Contains(LCD_FANCY) || n.Contains(LCD_FLOOR)) fancyPanels.Add(p);
    }
}

void ScanDoors() {
    doors.Clear();
    doorCustomDelay.Clear();
    airlocks.Clear();
    GridTerminalSystem.GetBlocksOfType(doors, d => d.IsSameConstructAs(Me)
        && !d.CustomName.Contains(TAG_NO_DOOR) && !d.CustomName.Contains(TAG_MANUAL));

    var allLights = new List<IMyLightingBlock>();
    var allSounds = new List<IMySoundBlock>();
    GridTerminalSystem.GetBlocksOfType(allLights, l => l.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(allSounds, s => s.IsSameConstructAs(Me));

    foreach (var door in doors) {
        string dd = door.CustomData;
        if (!string.IsNullOrWhiteSpace(dd)) {
            foreach (var raw in dd.Split('\n')) {
                string l = raw.Trim().ToLower();
                if (l.StartsWith("closetime=")) {
                    float ct; if (float.TryParse(l.Substring(10), out ct) && ct > 0)
                        doorCustomDelay[door.EntityId] = ct;
                }
            }
        }
        string name = door.CustomName;
        bool isInt = name.IndexOf("Interior", StringComparison.OrdinalIgnoreCase) >= 0;
        bool isExt = name.IndexOf("Exterior", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!isInt && !isExt) continue;
        string airlockId = null;
        int bIdx = name.IndexOf("[Airlock:", StringComparison.OrdinalIgnoreCase);
        if (bIdx >= 0) {
            int bEnd = name.IndexOf(']', bIdx + 9);
            if (bEnd > bIdx + 9) airlockId = name.Substring(bIdx + 9, bEnd - bIdx - 9).Trim();
        } else {
            int idx = name.IndexOf("Airlock", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0) airlockId = name.Substring(0, idx).Trim();
        }
        if (string.IsNullOrEmpty(airlockId)) continue;
        AirlockGroup grp;
        if (!airlocks.TryGetValue(airlockId, out grp)) { grp = new AirlockGroup(); airlocks[airlockId] = grp; }
        if (isInt) grp.interior.Add(door); else grp.exterior.Add(door);
    }

    foreach (var kv in airlocks) {
        string pfx = kv.Key;
        foreach (var lt in allLights) {
            string ln = lt.CustomName;
            if (ln.Contains(pfx) && ln.IndexOf("Airlock", StringComparison.OrdinalIgnoreCase) >= 0
                && ln.IndexOf("Light", StringComparison.OrdinalIgnoreCase) >= 0)
                kv.Value.lights.Add(lt);
        }
        foreach (var sn in allSounds) {
            string sname = sn.CustomName;
            if (sname.Contains(pfx) && sname.IndexOf("Airlock", StringComparison.OrdinalIgnoreCase) >= 0
                && sname.IndexOf("Sound", StringComparison.OrdinalIgnoreCase) >= 0)
                kv.Value.sounds.Add(sn);
        }
    }
}

// ======================== DOOR SYSTEM ========================================

bool IsHangarDoor(IMyDoor d) {
    return d.BlockDefinition.TypeId.ToString().Contains("Hangar");
}

void CheckDoorTimers() {
    var now = DateTime.UtcNow;
    foreach (var door in doors) {
        if (!includeHangarDoors && IsHangarDoor(door)) continue;
        if (!door.Enabled) continue;
        long id = door.EntityId;
        float delay = doorCloseDelay;
        float custom;
        if (doorCustomDelay.TryGetValue(id, out custom) && custom > 0) delay = custom;
        if (door.Status == DoorStatus.Open) {
            DateTime opened;
            if (!doorOpenTimes.TryGetValue(id, out opened)) {
                doorOpenTimes[id] = now;
            } else if ((now - opened).TotalSeconds >= delay) {
                door.CloseDoor();
                doorOpenTimes.Remove(id);
            }
        } else {
            doorOpenTimes.Remove(id);
        }
    }
}

void UpdateAirlocks() {
    foreach (var kv in airlocks) {
        var g = kv.Value;
        if (g.interior.Count == 0 || g.exterior.Count == 0) continue;
        bool intNotClosed = false, extNotClosed = false;
        foreach (var d in g.interior) if (d.Status != DoorStatus.Closed) { intNotClosed = true; break; }
        foreach (var d in g.exterior) if (d.Status != DoorStatus.Closed) { extNotClosed = true; break; }
        if (intNotClosed) {
            foreach (var d in g.exterior) d.Enabled = false;
            foreach (var d in g.interior) d.Enabled = true;
        } else if (extNotClosed) {
            foreach (var d in g.interior) d.Enabled = false;
            foreach (var d in g.exterior) d.Enabled = true;
        } else {
            foreach (var d in g.interior) d.Enabled = true;
            foreach (var d in g.exterior) d.Enabled = true;
        }
        bool open = intNotClosed || extNotClosed;
        foreach (var lt in g.lights) {
            lt.Color = open ? new Color(255, 40, 40) : new Color(80, 160, 255);
            lt.BlinkIntervalSeconds = open ? 0.8f : 0f;
            lt.BlinkLength = open ? 50f : 100f;
            lt.Enabled = true;
        }
        if (open != g.wasOpen) {
            foreach (var s in g.sounds) { if (open) s.Play(); else s.Stop(); }
        }
        g.wasOpen = open;
    }
}

// ======================== SOLAR ALIGNMENT ====================================

void SolarStep() {
    if (solarStatus == "PAUSED" || solarRotors.Count == 0) return;
    float output = 0;
    foreach (var p in solarPanels) output += p.MaxOutput * 1000f;
    if (output < nightThreshold) {
        if (!nightMode) {
            nightMode = true;
            solarStatus = "NIGHT";
            foreach (var r in solarRotors) r.TargetVelocityRPM = 0;
        }
        lastSolarOutput = output;
        return;
    }
    if (nightMode) {
        nightMode = false;
        bestSolarOutput = output;
        solarDir = 1;
        solarStatus = "TRACKING";
    }
    if (solarStatus == "INIT") {
        bestSolarOutput = output;
        solarStatus = "TRACKING";
    }
    if (output > bestSolarOutput) bestSolarOutput = output;
    if (output >= lastSolarOutput) {
        foreach (var r in solarRotors) r.TargetVelocityRPM = solarRotorSpeed * solarDir;
        solarStatus = output >= bestSolarOutput * 0.99f ? "LOCKED" : "TRACKING";
    } else {
        float drop = bestSolarOutput > 0 ? ((bestSolarOutput - output) / bestSolarOutput * 100f) : 0;
        if (drop > realignPercent) {
            solarDir = -solarDir;
            foreach (var r in solarRotors) r.TargetVelocityRPM = solarRotorSpeed * solarDir;
            solarStatus = "REALIGN";
        } else {
            foreach (var r in solarRotors) r.TargetVelocityRPM = 0;
            solarStatus = "LOCKED";
        }
    }
    lastSolarOutput = output;
}

// ======================== POWER FALLBACK =====================================

void PowerFallbackCheck() {
    if (!useReactorFallback && !useH2Fallback) return;
    float stored = 0, max = 0;
    foreach (var b in batteries) { stored += b.CurrentStoredPower; max += b.MaxStoredPower; }
    float pct = max > 0 ? (stored / max * 100f) : 100f;
    if (pct < lowBatteryPct && !fallbackActive) {
        fallbackActive = true;
        if (useReactorFallback) foreach (var r in reactors) r.Enabled = true;
        if (useH2Fallback) foreach (var e in h2Engines) (e as IMyFunctionalBlock).Enabled = true;
    } else if (fallbackActive && pct > lowBatteryPct + 15) {
        fallbackActive = false;
        if (useReactorFallback) foreach (var r in reactors) r.Enabled = false;
        if (useH2Fallback) foreach (var e in h2Engines) (e as IMyFunctionalBlock).Enabled = false;
    }
}

// ======================== IGC ================================================

void ProcessIgc() {
    while (coreListener.HasPendingMessage) {
        var msg = coreListener.AcceptMessage();
        var data = msg.Data as string;
        if (data == null) continue;
        coreWarn.Clear(); coreState.Clear();
        string sec = "";
        foreach (var raw in data.Split('\n')) {
            var line = raw.TrimEnd('\r');
            if (line == "[LOG]" || line == "[WARN]" || line == "[STATE]") { sec = line; continue; }
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (sec == "[WARN]") coreWarn.Add(line);
            else if (sec == "[STATE]") {
                int eq = line.IndexOf('=');
                if (eq > 0) coreState[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
        }
    }
}

// ======================== FANCY DISPLAYS =====================================

void RenderFancy(IMyTextPanel panel) {
    if (string.IsNullOrWhiteSpace(panel.CustomData) || !panel.CustomData.Contains("ShowStats"))
        InitFancy(panel);
    panel.ContentType = ContentType.SCRIPT;
    panel.ScriptBackgroundColor = BG;
    var vp = new RectangleF((panel.TextureSize - panel.SurfaceSize) / 2f, panel.SurfaceSize);
    var frame = panel.DrawFrame();
    int sc = 0;
    float x0 = vp.X + 8f, y = vp.Y + 5f, pw = vp.Width - 16f;
    string gn = Me.CubeGrid.CustomName;
    if (gn.Length > 22) gn = gn.Substring(0, 22);
    frame.Add(new MySprite(SpriteType.TEXT, gn,
        new Vector2(vp.X + vp.Width / 2f, y), null, TITLE_CLR, "White", TextAlignment.CENTER, 0.7f));
    sc++; y += 28f;
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
        new Vector2(vp.X + vp.Width / 2f, y), new Vector2(pw, 1), BORDER));
    sc++; y += 5f;
    bool inStats = false, inText = false;
    foreach (var rawLine in panel.CustomData.Split('\n')) {
        if (sc >= SPRITE_BUDGET - 20 || y > vp.Y + vp.Height - 10) break;
        string line = rawLine.Trim();
        if (line == "ShowStats") { inStats = true; inText = false; continue; }
        if (line == "---") { inStats = false; inText = true; y += 6f; continue; }
        if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;
        if (line.StartsWith("[")) { inStats = false; inText = false; continue; }
        if (inText) { RenderText(frame, vp, ref y, pw, line, ref sc); continue; }
        if (!inStats) continue;
        var parts = line.Split(',');
        string sn = parts[0].Trim();
        bool wide = false, opt = false, nosub = false;
        for (int i = 1; i < parts.Length; i++) {
            string m = parts[i].Trim().ToLower();
            if (m == "widebar") wide = true;
            else if (m == "optional") opt = true;
            else if (m == "nosubgrids") nosub = true;
        }
        var sd = GetStat(sn, opt, nosub);
        if (!sd.hasData) continue;
        DrawBar(frame, x0, ref y, pw, sd, wide, ref sc);
    }
    frame.Dispose();
}

struct StatData {
    public string label, value, detail;
    public double ratio;
    public int count;
    public bool hasData;
}

StatData GetStat(string name, bool opt, bool nosub) {
    string nl = name.ToLower();
    var d = new StatData();
    if (nl == "battery" || nl == "batteries") {
        float st = 0, mx = 0, inp = 0, outp = 0; int cnt = 0;
        foreach (var b in batteries) {
            if (nosub && b.CubeGrid != Me.CubeGrid) continue;
            st += b.CurrentStoredPower; mx += b.MaxStoredPower;
            inp += b.CurrentInput; outp += b.CurrentOutput; cnt++;
        }
        d.ratio = mx > 0 ? st / mx : 0; d.count = cnt;
        d.label = "Battery (" + cnt + ")";
        d.value = (d.ratio * 100).ToString("0.0") + "%";
        d.detail = opt ? "In:" + FmtPow(inp) + " Out:" + FmtPow(outp)
            : st.ToString("0.#") + "/" + mx.ToString("0.#") + " MWh";
        d.hasData = cnt > 0;
    }
    else if (nl == "hydrogen tank" || nl == "h2 tank" || nl == "h2") {
        double fill = 0; int cnt = 0;
        foreach (var t in gasTanks) {
            if (!t.BlockDefinition.SubtypeId.Contains("Hydrogen")) continue;
            if (nosub && t.CubeGrid != Me.CubeGrid) continue;
            fill += t.FilledRatio; cnt++;
        }
        d.ratio = cnt > 0 ? fill / cnt : 0; d.count = cnt;
        d.label = "H2 Tank (" + cnt + ")";
        d.value = (d.ratio * 100).ToString("0.0") + "%";
        d.hasData = cnt > 0;
    }
    else if (nl == "oxygen tank" || nl == "o2 tank" || nl == "o2") {
        double fill = 0; int cnt = 0;
        foreach (var t in gasTanks) {
            if (t.BlockDefinition.SubtypeId.Contains("Hydrogen")) continue;
            if (nosub && t.CubeGrid != Me.CubeGrid) continue;
            fill += t.FilledRatio; cnt++;
        }
        d.ratio = cnt > 0 ? fill / cnt : 0; d.count = cnt;
        d.label = "O2 Tank (" + cnt + ")";
        d.value = (d.ratio * 100).ToString("0.0") + "%";
        d.hasData = cnt > 0;
    }
    else if (nl == "reactor" || nl == "reactors") {
        float curO = 0, maxO = 0; double urani = 0; int cnt = 0;
        foreach (var r in reactors) {
            if (nosub && r.CubeGrid != Me.CubeGrid) continue;
            curO += r.CurrentOutput; maxO += r.MaxOutput; cnt++;
            if (opt) {
                var inv = r.GetInventory(0);
                if (inv != null) {
                    var items = new List<MyInventoryItem>();
                    inv.GetItems(items);
                    foreach (var it in items) urani += (double)it.Amount;
                }
            }
        }
        d.ratio = maxO > 0 ? curO / maxO : 0; d.count = cnt;
        d.label = "Reactor (" + cnt + ")";
        if (opt) { d.value = urani.ToString("0.#") + " kg U"; d.detail = FmtPow(curO) + " out"; }
        else { d.value = FmtPow(curO) + "/" + FmtPow(maxO); }
        d.hasData = cnt > 0;
    }
    else if (nl == "solar" || nl == "solar panel" || nl == "solar panels") {
        float cur = 0, mx = 0; int cnt = 0;
        foreach (var s in solarPanels) {
            if (nosub && s.CubeGrid != Me.CubeGrid) continue;
            cur += s.CurrentOutput; mx += s.MaxOutput; cnt++;
        }
        d.ratio = mx > 0 ? cur / mx : 0; d.count = cnt;
        d.label = "Solar (" + cnt + ")";
        d.value = FmtPow(cur) + "/" + FmtPow(mx);
        d.hasData = cnt > 0;
    }
    else if (nl == "cargo" || nl == "inventory") {
        long uv = 0, mv = 0; int cnt = 0;
        foreach (var c in cargoBlocks) {
            if (nosub && c.CubeGrid != Me.CubeGrid) continue;
            cnt++;
            for (int i = 0; i < c.InventoryCount; i++) {
                var inv = c.GetInventory(i);
                uv += inv.CurrentVolume.RawValue; mv += inv.MaxVolume.RawValue;
            }
        }
        d.ratio = mv > 0 ? (double)uv / mv : 0; d.count = cnt;
        d.label = "Cargo";
        d.value = (d.ratio * 100).ToString("0.0") + "%";
        d.hasData = cnt > 0;
    }
    else if (nl == "hydrogen engine" || nl == "h2 engine") {
        float cur = 0, mx = 0; int on = 0, cnt = 0;
        foreach (var e in h2Engines) {
            if (nosub && e.CubeGrid != Me.CubeGrid) continue;
            cnt++;
            var pp = e as IMyPowerProducer;
            if (pp != null) { cur += pp.CurrentOutput; mx += pp.MaxOutput; }
            if ((e as IMyFunctionalBlock).Enabled) on++;
        }
        d.ratio = mx > 0 ? cur / mx : 0; d.count = cnt;
        d.label = "H2 Engine (" + cnt + ")";
        d.value = on + "/" + cnt + " ON";
        d.detail = FmtPow(cur);
        d.hasData = cnt > 0;
    }
    else if (nl == "o2 generator" || nl == "h2/o2 generator" || nl == "generator") {
        int on = 0, cnt = 0; double ice = 0;
        foreach (var g in o2Gens) {
            if (nosub && g.CubeGrid != Me.CubeGrid) continue;
            cnt++;
            if (g.Enabled) on++;
            var inv = g.GetInventory(0);
            if (inv != null) {
                var items = new List<MyInventoryItem>();
                inv.GetItems(items);
                foreach (var it in items) ice += (double)it.Amount;
            }
        }
        d.ratio = ice > 0 ? Math.Min(ice / 10000, 1.0) : 0; d.count = cnt;
        d.label = "O2/H2 Gen (" + cnt + ")";
        d.value = on + "/" + cnt + " ON";
        d.detail = FmtNum(ice) + " ice";
        d.hasData = cnt > 0;
    }
    else if (nl == "wind" || nl == "wind turbine") {
        float cur = 0, mx = 0; int cnt = 0;
        foreach (var w in windTurbines) {
            if (nosub && w.CubeGrid != Me.CubeGrid) continue;
            cnt++;
            var pp = w as IMyPowerProducer;
            if (pp != null) { cur += pp.CurrentOutput; mx += pp.MaxOutput; }
        }
        d.ratio = mx > 0 ? cur / mx : 0; d.count = cnt;
        d.label = "Wind (" + cnt + ")";
        d.value = FmtPow(cur) + "/" + FmtPow(mx);
        d.hasData = cnt > 0;
    }
    else {
        d = GetCustomStat(name, nosub);
    }
    return d;
}

StatData GetCustomStat(string name, bool nosub) {
    var d = new StatData();
    var grp = GridTerminalSystem.GetBlockGroupWithName(name);
    List<IMyTerminalBlock> blocks;
    if (grp != null) {
        blocks = new List<IMyTerminalBlock>();
        grp.GetBlocks(blocks);
    } else {
        blocks = new List<IMyTerminalBlock>();
        string lower = name.ToLower();
        foreach (var b in cargoBlocks)
            if (b.CustomName.ToLower().Contains(lower)) blocks.Add(b);
    }
    long uv = 0, mv = 0;
    foreach (var b in blocks) {
        if (nosub && b.CubeGrid != Me.CubeGrid) continue;
        if (!b.HasInventory) continue;
        d.count++;
        for (int i = 0; i < b.InventoryCount; i++) {
            var inv = b.GetInventory(i);
            uv += inv.CurrentVolume.RawValue; mv += inv.MaxVolume.RawValue;
        }
    }
    d.ratio = mv > 0 ? (double)uv / mv : 0;
    d.label = name + " (" + d.count + ")";
    d.value = (d.ratio * 100).ToString("0.0") + "%";
    d.hasData = d.count > 0;
    return d;
}

void DrawBar(MySpriteDrawFrame frame, float x0, ref float y, float pw, StatData data, bool wide, ref int sc) {
    frame.Add(new MySprite(SpriteType.TEXT, data.label,
        new Vector2(x0, y), null, TXT, "White", TextAlignment.LEFT, 0.5f));
    sc++;
    frame.Add(new MySprite(SpriteType.TEXT, data.value,
        new Vector2(x0 + pw, y), null, TXT, "White", TextAlignment.RIGHT, 0.5f));
    sc++; y += 19f;
    float bh = wide ? 16f : 12f;
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
        new Vector2(x0 + pw / 2, y + bh / 2), new Vector2(pw, bh), BAR_BG));
    sc++;
    float fill = (float)(pw * Math.Max(0, Math.Min(1, data.ratio)));
    if (fill > 0.5f) {
        frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
            new Vector2(x0 + fill / 2, y + bh / 2), new Vector2(fill, bh - 2), BarColor(data.ratio)));
        sc++;
    }
    if (!string.IsNullOrEmpty(data.detail)) {
        frame.Add(new MySprite(SpriteType.TEXT, data.detail,
            new Vector2(x0 + pw / 2, y + 1), null, DIM, "White", TextAlignment.CENTER, 0.35f));
        sc++;
    }
    y += bh + 7f;
}

Color BarColor(double r) {
    if (r < 0.25) return new Color(190, 35, 35, 220);
    if (r < 0.5) return new Color(210, 150, 25, 220);
    if (r < 0.75) return new Color(170, 195, 35, 220);
    return new Color(45, 190, 55, 220);
}

void RenderText(MySpriteDrawFrame frame, RectangleF vp, ref float y, float pw, string key, ref int sc) {
    string kl = key.ToLower().Trim();
    float x0 = vp.X + 8;
    if (kl == "warnings") {
        if (coreWarn.Count == 0) {
            frame.Add(new MySprite(SpriteType.TEXT, "No warnings",
                new Vector2(x0, y), null, DIM, "White", TextAlignment.LEFT, 0.42f));
            sc++; y += 17f;
        } else {
            frame.Add(new MySprite(SpriteType.TEXT, "Warnings (" + coreWarn.Count + "):",
                new Vector2(x0, y), null, WARN_CLR, "White", TextAlignment.LEFT, 0.45f));
            sc++; y += 19f;
            int mx = Math.Min(coreWarn.Count, 5);
            for (int i = 0; i < mx && sc < SPRITE_BUDGET - 5; i++) {
                string w = coreWarn[i]; if (w.Length > 32) w = w.Substring(0, 32);
                frame.Add(new MySprite(SpriteType.TEXT, " " + w,
                    new Vector2(x0, y), null, TXT, "White", TextAlignment.LEFT, 0.38f));
                sc++; y += 16f;
            }
        }
    }
    else if (kl == "status" || kl == "craftstatus") {
        string step = "", status = "", inst = "", maxI = "";
        coreState.TryGetValue("step", out step);
        coreState.TryGetValue("status", out status);
        coreState.TryGetValue("instructions", out inst);
        coreState.TryGetValue("maxInst", out maxI);
        string t = "Core: " + step;
        if (!string.IsNullOrEmpty(status)) t += " | " + status;
        frame.Add(new MySprite(SpriteType.TEXT, t,
            new Vector2(x0, y), null, DIM, "White", TextAlignment.LEFT, 0.4f));
        sc++; y += 17f;
        if (!string.IsNullOrEmpty(inst)) {
            frame.Add(new MySprite(SpriteType.TEXT, "Instructions: " + inst + "/" + maxI,
                new Vector2(x0, y), null, DIM, "White", TextAlignment.LEFT, 0.38f));
            sc++; y += 16f;
        }
    }
    else {
        frame.Add(new MySprite(SpriteType.TEXT, key,
            new Vector2(x0, y), null, TXT, "White", TextAlignment.LEFT, 0.42f));
        sc++; y += 17f;
    }
}

void InitFancy(IMyTextPanel p) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("# Fancy Status Display (PB3)");
    sb.AppendLine("# Types: Battery, Hydrogen Tank, Oxygen Tank, Reactor,");
    sb.AppendLine("#   Solar, Cargo, Hydrogen Engine, Wind, O2 Generator");
    sb.AppendLine("#   or any block group / name substring");
    sb.AppendLine("# Modifiers: widebar, optional, nosubgrids");
    sb.AppendLine("# Use --- to add text section (Warnings, Status)");
    sb.AppendLine("");
    sb.AppendLine("ShowStats");
    sb.AppendLine("Battery");
    sb.AppendLine("Hydrogen Tank,widebar");
    sb.AppendLine("Oxygen Tank,widebar");
    sb.AppendLine("Cargo,widebar");
    sb.AppendLine("Solar");
    sb.AppendLine("Reactor,optional");
    sb.AppendLine("---");
    sb.AppendLine("Warnings");
    sb.AppendLine("Status");
    p.CustomData = sb.ToString();
}

// ======================== FLOOR PLAN =========================================

void RunFloorStep() {
    if (floorRescan && !floorActive) {
        floorCo = ScanFloor();
        floorActive = true;
        floorRescan = false;
    }
    if (floorActive && floorCo != null) {
        if (!floorCo.MoveNext() || floorCo.Current < 0) {
            floorCo.Dispose(); floorCo = null; floorActive = false;
        }
    }
}

IEnumerator<int> ScanFloor() {
    floorCells.Clear();
    floorReady = false;
    var grid = Me.CubeGrid;
    int yLvl = Me.Position.Y + floorLevel;
    Vector3I gMin = grid.Min, gMax = grid.Max;
    int n = 0;
    for (int x = gMin.X; x <= gMax.X; x++) {
        for (int z = gMin.Z; z <= gMax.Z; z++) {
            var pos = new Vector3I(x, yLvl, z);
            if (grid.CubeExists(pos)) {
                var slim = grid.GetCubeBlock(pos);
                int ci = ClassifyBlock(slim);
                bool dmg = slim != null && (slim.IsDestroyed ||
                    slim.BuildIntegrity < slim.MaxIntegrity * 0.99);
                floorCells.Add(new FloorCell { x = x, z = z, ci = ci, dmg = dmg });
            }
            n++;
            if (n % FLOOR_YIELD == 0) yield return n;
        }
    }
    if (floorCells.Count > 0) {
        fMinX = fMinZ = int.MaxValue; fMaxX = fMaxZ = int.MinValue;
        foreach (var c in floorCells) {
            if (c.x < fMinX) fMinX = c.x; if (c.x > fMaxX) fMaxX = c.x;
            if (c.z < fMinZ) fMinZ = c.z; if (c.z > fMaxZ) fMaxZ = c.z;
        }
    }
    floorReady = true;
    yield return -1;
}

int ClassifyBlock(IMySlimBlock slim) {
    if (slim == null) return 0;
    if (slim.FatBlock == null) {
        return slim.BlockDefinition.SubtypeName.ToLower().Contains("heavy") ? 2 : 1;
    }
    var f = slim.FatBlock;
    if (f is IMyDoor) return 3;
    if (f is IMyTextPanel) return 4;
    string sn = slim.BlockDefinition.SubtypeName.ToLower();
    if (sn.Contains("window") || sn.Contains("glass")) return 4;
    if (f is IMyConveyor || f is IMyConveyorSorter || sn.Contains("conveyor")) return 5;
    if (f is IMyPowerProducer || f is IMyBatteryBlock) return 7;
    if (f is IMyThrust) return 8;
    if (f is IMyUserControllableGun || f is IMyLargeTurretBase) return 9;
    if (f is IMyCargoContainer || f is IMyShipConnector) return 10;
    return 6;
}

Color FloorColor(int idx) {
    switch (idx) {
        case 1: return FL_LIGHT; case 2: return FL_HEAVY; case 3: return FL_DOOR;
        case 4: return FL_GLASS; case 5: return FL_CONV; case 6: return FL_FUNC;
        case 7: return FL_POWER; case 8: return FL_THRUST; case 9: return FL_WPN;
        case 10: return FL_CARGO; default: return FL_LIGHT;
    }
}

void RenderFloor(IMyTextPanel panel) {
    int lvl = floorLevel; float sc = floorZoom;
    ReadFloorCfg(panel, ref lvl, ref sc);
    if (lvl != floorLevel) { floorLevel = lvl; floorRescan = true; }
    if (floorCells.Count == 0) return;
    panel.ContentType = ContentType.SCRIPT;
    panel.ScriptBackgroundColor = BG;
    var vp = new RectangleF((panel.TextureSize - panel.SurfaceSize) / 2f, panel.SurfaceSize);
    var frame = panel.DrawFrame();
    int spr = 0;
    float titleH = 55f, legendH = 55f, margin = 6f;
    float aW = vp.Width - margin * 2, aH = vp.Height - titleH - legendH - margin;
    int gW = fMaxX - fMinX + 1, gH = fMaxZ - fMinZ + 1;
    if (gW <= 0 || gH <= 0) { frame.Dispose(); return; }
    if (sc <= 0) sc = Math.Min(aW / gW, aH / gH);
    float oX = vp.X + (vp.Width - gW * sc) / 2f;
    float oY = vp.Y + titleH + (aH - gH * sc) / 2f;
    frame.Add(new MySprite(SpriteType.TEXT,
        "Floor Plan  Lv:" + floorLevel + "  Y=" + (Me.Position.Y + floorLevel),
        new Vector2(vp.X + vp.Width / 2f, vp.Y + 3f),
        null, TITLE_CLR, "White", TextAlignment.CENTER, 1.0f));
    frame.Add(new MySprite(SpriteType.TEXT,
        floorCells.Count + " blocks  Scale:" + sc.ToString("0.0"),
        new Vector2(vp.X + vp.Width / 2f, vp.Y + 30f),
        null, DIM, "White", TextAlignment.CENTER, 0.65f));
    spr += 2;
    foreach (var cell in floorCells) {
        if (spr >= SPRITE_BUDGET - 15) break;
        float cx = oX + (cell.x - fMinX + 0.5f) * sc;
        float cy = oY + (cell.z - fMinZ + 0.5f) * sc;
        Color c = cell.dmg ? FL_DMG : FloorColor(cell.ci);
        frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
            new Vector2(cx, cy), new Vector2(sc * 0.9f, sc * 0.9f), c));
        spr++;
    }
    float ly = vp.Y + vp.Height - 40f, lx = vp.X + 5;
    string[] ll = { "Armor", "Heavy", "Door", "Glass", "Conv",
        "Func", "Power", "Thrust", "Wpn", "Cargo", "Dmg" };
    Color[] lc = { FL_LIGHT, FL_HEAVY, FL_DOOR, FL_GLASS, FL_CONV,
        FL_FUNC, FL_POWER, FL_THRUST, FL_WPN, FL_CARGO, FL_DMG };
    float legX = lx;
    for (int i = 0; i < ll.Length && spr < SPRITE_BUDGET; i++) {
        frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
            new Vector2(legX + 4, ly + 5), new Vector2(8, 8), lc[i]));
        frame.Add(new MySprite(SpriteType.TEXT, ll[i],
            new Vector2(legX + 14, ly - 2), null, DIM, "White", TextAlignment.LEFT, 0.5f));
        spr += 2;
        legX += 65f;
        if (legX > vp.X + vp.Width - 50) { legX = lx; ly += 18f; }
    }
    frame.Add(new MySprite(SpriteType.TEXT,
        "Cmd: floor_up/down  floor_zoom_in/out",
        new Vector2(vp.X + vp.Width / 2f, vp.Y + vp.Height - 14),
        null, new Color(60, 60, 60), "White", TextAlignment.CENTER, 0.5f));
    spr++;
    frame.Dispose();
}

void ReadFloorCfg(IMyTextPanel panel, ref int level, ref float scale) {
    string data = panel.CustomData;
    if (string.IsNullOrWhiteSpace(data) || !data.Contains("[Floor Plan]")) {
        InitFloorPanel(panel); data = panel.CustomData;
    }
    bool inSec = false;
    foreach (var raw in data.Split('\n')) {
        string line = raw.Trim();
        if (line == "[Floor Plan]") { inSec = true; continue; }
        if (line.StartsWith("[")) { inSec = false; continue; }
        if (!inSec || line.StartsWith("#") || !line.Contains("=")) continue;
        var p = line.Split(new char[]{'='}, 2);
        if (p.Length != 2) continue;
        string k = p[0].Trim(), v = p[1].Trim();
        if (k == "Level") int.TryParse(v, out level);
        else if (k == "Scale" && v.ToLower() != "auto") float.TryParse(v, out scale);
    }
}

void InitFloorPanel(IMyTextPanel p) {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Floor Plan]");
    sb.AppendLine("# Level: Y-offset from PB (0=same, +1=above, -1=below)");
    sb.AppendLine("Level=0");
    sb.AppendLine("# Scale: auto or pixels-per-block");
    sb.AppendLine("Scale=auto");
    sb.AppendLine("");
    sb.AppendLine("# Commands (PB argument): floor_up, floor_down,");
    sb.AppendLine("# floor_zoom_in, floor_zoom_out, floor_reset");
    p.CustomData = sb.ToString();
}

// ======================== DISPLAYS ===========================================

void UpdateSolarLcds() {
    float output = 0;
    foreach (var p in solarPanels) output += p.MaxOutput * 1000f;
    float stored = 0, bmax = 0;
    foreach (var b in batteries) { stored += b.CurrentStoredPower; bmax += b.MaxStoredPower; }
    float batPct = bmax > 0 ? stored / bmax * 100f : 0;
    foreach (var lcd in solarLcds) {
        lcd.ContentType = ContentType.TEXT_AND_IMAGE;
        lcd.Font = "Monospace";
        lcd.FontSize = 0.65f;
        lcd.TextPadding = 2f;
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== Solar Alignment ===");
        sb.AppendLine("");
        sb.AppendLine("Mode: " + solarStatus);
        sb.AppendLine("Rotors: " + solarRotors.Count + " | Panels: " + solarPanels.Count);
        sb.AppendLine("");
        sb.AppendLine("Output: " + Fmt(output));
        if (bestSolarOutput > 0) {
            float eff = output / bestSolarOutput * 100f;
            sb.AppendLine("Efficiency: " + eff.ToString("0.0") + "%");
            sb.AppendLine("Best: " + Fmt(bestSolarOutput));
        }
        sb.AppendLine("");
        sb.AppendLine("Battery: " + batPct.ToString("0.0") + "% " + Bar(batPct / 100, 12));
        sb.AppendLine("Fallback: " + (fallbackActive ? "ACTIVE" : "standby"));
        sb.AppendLine("");
        sb.AppendLine("--- Doors ---");
        sb.AppendLine("Managed: " + doors.Count + " | Airlocks: " + airlocks.Count);
        lcd.WriteText(sb.ToString());
    }
}

void UpdateEcho() {
    Echo("=== Automation + Visual (PB3) ===");
    Echo("Runs: " + pb3Runs + " | Step: " + pb3Step);
    if (!string.IsNullOrEmpty(lastError)) {
        Echo("ERROR:\n" + lastError);
    }
    Echo("Doors:" + doors.Count + " Airlocks:" + airlocks.Count);
    Echo("Solar:" + solarRotors.Count + " rot, " + solarPanels.Count + " pan | " + solarStatus);
    Echo("Output:" + Fmt(lastSolarOutput) + (fallbackActive ? " FALLBACK" : ""));
    Echo("Fancy:" + fancyPanels.Count + " Bat:" + batteries.Count + " Tank:" + gasTanks.Count);
    if (floorActive) Echo("Floor: scanning...");
    else if (floorReady) Echo("Floor: Lv" + floorLevel + " " + floorCells.Count + " blocks");
    Echo("Runtime:" + Runtime.LastRunTimeMs.ToString("0.000") + "ms");
}

// ======================== HELPERS ============================================

string Fmt(float kw) {
    if (kw >= 1000000) return (kw / 1000000).ToString("0.##") + " GW";
    if (kw >= 1000) return (kw / 1000).ToString("0.##") + " MW";
    if (kw >= 1) return kw.ToString("0.#") + " kW";
    return (kw * 1000).ToString("0.#") + " W";
}

string Bar(double ratio, int w) {
    int f = (int)(ratio * w + 0.5);
    if (f < 0) f = 0; if (f > w) f = w;
    return "[" + new string('\u2588', f) + new string('\u2591', w - f) + "]";
}

string FmtPow(float mw) {
    float a = Math.Abs(mw);
    if (a >= 1000) return (mw / 1000).ToString("0.#") + "GW";
    if (a >= 1) return mw.ToString("0.##") + "MW";
    if (a >= 0.001f) return (mw * 1000).ToString("0.#") + "kW";
    return (mw * 1000000).ToString("0") + "W";
}

string FmtNum(double val) {
    if (val >= 1000000) return (val / 1000000).ToString("0.#") + "M";
    if (val >= 10000) return (val / 1000).ToString("0.#") + "K";
    return val.ToString("#,0");
}

// ======================== CLASSES ============================================

class AirlockGroup {
    public List<IMyDoor> interior = new List<IMyDoor>();
    public List<IMyDoor> exterior = new List<IMyDoor>();
    public List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
    public List<IMySoundBlock> sounds = new List<IMySoundBlock>();
    public bool wasOpen = false;
}

struct FloorCell {
    public int x, z, ci;
    public bool dmg;
}
