// MPX HUMAN LOGIC BRAIN v1.0
// Write simple English rules on your MPX COMMAND LCD. Your ship obeys.
// Block naming: MPX HUMAN LOGIC BRAIN (PB), MPX COMMAND (LCD input),
//               MPX CORE (AI status LCD), MPX DISPLAY (telemetry LCD), MPX LOG (optional)
//
// IMPORTANT for Space Engineers Programmable Block editor:
// The PB editor auto-wraps this file inside `public class Program : MyGridProgram { ... }`
// and pre-includes every required `using` directive. So this file must contain NO using
// statements and NO class declaration — only the body of the Program class.

#region CONFIG
const string LCD_COMMAND = "MPX COMMAND";
const string LCD_CORE = "MPX CORE";
const string LCD_DISPLAY = "MPX DISPLAY";
const string LCD_LOG = "MPX LOG";
const string SENSOR_GROUP = "MPX ENEMY SENSOR";
const string DETECT_GROUP = "MPX DETECTOR";
const string DAYLIGHT_SENSOR_GROUP = "MPX DAYLIGHT SENSOR";
// OPT-IN ONLY. Default false: day/night reads every solar panel on the construct.
// When true, ONLY panels inside the MPX DAYLIGHT SENSOR group are used for day/night.
// Leave false unless the player has explicitly placed dedicated, fixed-orientation panels.
// Read via UseDaylightSensorGroup() — the method indirection stops the compiler from
// const-folding "if (false)" branches and emitting "unreachable code detected" warnings.
const bool USE_DAYLIGHT_SENSOR_GROUP = false;

const double SOLAR_DARK_THRESHOLD = 0.05; // below 5% = dark
const double SOLAR_LIGHT_THRESHOLD = 0.20; // above 20% = daylight
// Timing note: Update10 fires every 10 sim ticks; SE = 60 sim ticks/sec → ~6 Hz, so 1 tick ≈ 0.17 s.
const int SOLAR_HYSTERESIS_TICKS = 60;   // ~10 s of debounce before flipping dark/daylight
const int BLOCK_REFRESH_INTERVAL = 30;   // ~5 s between full block re-scans (was 50 s)
const int ACTION_COOLDOWN_TICKS = 20;    // ~3.3 s minimum between same-action fires
const int PRODUCE_COOLDOWN_TICKS = 600;  // ~100 s between production queues
const int MAX_LOG_LINES = 60;
const int MAX_RULES = 80;
const int ACTION_TRACE_MAX = 6;          // last N actual state changes shown on MPX CORE
#endregion

#region STATE
bool _paused = false;
int _tick = 0;
// Solar state machine — starts UNKNOWN; dark/daylight conditions return false until panels confirm a state.
// Dark and Daylight are never both true (guarded by enum + _solarAvailable check in EvaluateCondition).
SolarState _solarState = SolarState.Unknown;
int _solarDarkTicks = 0;
int _solarLightTicks = 0;
bool _solarAvailable = false;
int _solarPanelCount = 0;
string _solarSource = "none";
string _solarWarning = "";
int _lastSolarChangeTick = 0;
bool _enemyDetected = false;
string _enemySource = "none";
double _powerPct = 0;
double _hydrogenPct = 0;
double _oxygenPct = 0;
double _cargoPct = 0;
double _solarPct = 0;       // legacy alias = _solarBestPct (kept for SOL bar + SolarBelow/Above rules)
double _solarBestPct = 0;   // highest per-panel CurrentOutput/MaxOutput% — drives day/night decision
double _solarAvgPct = 0;    // average per-panel% across usable panels — informational only
bool _connectorDocked = false;
int _spinnerIdx = 0;
int _scanDotIdx = 0;
string _lastUnderstood = "";
string _lastTriggered = "";
string _lastActions = "";
List<string> _warnings = new List<string>();
List<string> _logLines = new List<string>();
List<ParsedRule> _rules = new List<ParsedRule>();
Dictionary<string, int> _actionCooldowns = new Dictionary<string, int>();
Dictionary<string, int> _produceCooldowns = new Dictionary<string, int>();
// Per-tick inventory count cache — rebuilt once per tick, reused by all ItemBelow rules
Dictionary<string, int> _invCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
int _invCacheTick = -1;
// Tracks MPX COMMAND text so edits trigger auto-reload without recompile / RELOAD argument.
string _lastCommandText = null;
// Per-tick conflict tracker: block EntityId → desired Enabled this tick. Cleared each Main() tick.
Dictionary<long, bool> _tickDesiredEnabled = new Dictionary<long, bool>();
// Ring buffer of the last ACTION_TRACE_MAX actual state changes — displayed on MPX CORE.
List<string> _actionTrace = new List<string>();
// Per-sound-block: tick at which the script should call Stop() to honor a "play for N seconds".
// While a deadline is in the future the rule can't re-play the same block (debounce).
Dictionary<long, int> _soundStopAtTick = new Dictionary<long, int>();
// Mute set keyed by lower-cased rule label. PB args MUTE/UNMUTE manage this.
HashSet<string> _mutedLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
// One-shot simulation overrides — see HandleArgument for SIMULATE. Tick value = expiry tick.
Dictionary<string, int> _simulated = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
// Pending PB argument fed to rules via `if argument "X"` — cleared after one full evaluation.
string _pendingArgument = "";
// Current rule profile name (default = ""). Rules tagged with [mode X] only fire when matched.
string _activeMode = "";
// Variables / counters / memory — single namespace, doubles for both numbers and bool (0/1).
// Persisted to PB CustomData so they survive recompile / save-load.
Dictionary<string, double> _vars = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
bool _varsDirty = false;
// IGC listeners we've registered per channel (lazy on first reference).
Dictionary<string, IMyBroadcastListener> _igcListeners = new Dictionary<string, IMyBroadcastListener>(StringComparer.OrdinalIgnoreCase);
// Pending received IGC messages per channel — drained each tick for 'if received "tag"' conds.
Dictionary<string, string> _igcReceived = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
// Per-rule fire counter — shown by the STATS PB arg and DIAGNOSE report.
Dictionary<string, int> _ruleFireCount = new Dictionary<string, int>();
// Cached time-of-day from the sun rotation (0..24 hours). Updated each tick from telemetry.
double _gameHour = 12.0;
// Whether the ship is in a gravity field (computed from the active controller).
bool _inGravity = false;
// Whether the player has invoked DRY-RUN — when true, rules evaluate but no actions actually fire.
bool _dryRun = false;
#endregion

#region BLOCK CACHES
IMyTextPanel _lcdCommand, _lcdCore, _lcdDisplay, _lcdLog;
List<IMyReactor> _reactors = new List<IMyReactor>();
List<IMyBatteryBlock> _batteries = new List<IMyBatteryBlock>();
List<IMySolarPanel> _solar = new List<IMySolarPanel>();
// Optional subset for day/night detection. Empty unless USE_DAYLIGHT_SENSOR_GROUP=true AND
// the MPX DAYLIGHT SENSOR group exists. Default: NOT POPULATED — _solar (all panels) is used instead.
List<IMySolarPanel> _solarSensorPanels = new List<IMySolarPanel>();
List<IMyDoor> _doors = new List<IMyDoor>(); // includes airtight hangar doors (they implement IMyDoor)
List<IMyLargeTurretBase> _turrets = new List<IMyLargeTurretBase>();
List<IMyLightingBlock> _lights = new List<IMyLightingBlock>();
List<IMySoundBlock> _soundBlocks = new List<IMySoundBlock>();
List<IMyShipConnector> _connectors = new List<IMyShipConnector>();
List<IMyGasGenerator> _gasGens = new List<IMyGasGenerator>();
List<IMyGasTank> _gasTanks = new List<IMyGasTank>();
List<IMyThrust> _thrusters = new List<IMyThrust>();
List<IMyAssembler> _assemblers = new List<IMyAssembler>();
List<IMyRefinery> _refineries = new List<IMyRefinery>();
List<IMyPistonBase> _pistons = new List<IMyPistonBase>();
List<IMyMotorStator> _rotors = new List<IMyMotorStator>(); // includes advanced rotors AND hinges
List<IMyTimerBlock> _timers = new List<IMyTimerBlock>();
List<IMyConveyorSorter> _sorters = new List<IMyConveyorSorter>();
List<IMyRadioAntenna> _antennas = new List<IMyRadioAntenna>();
List<IMyCameraBlock> _cameras = new List<IMyCameraBlock>();
List<IMyTextPanel> _textPanels = new List<IMyTextPanel>(); // for `write "..." to "..."`
// All user-controllable guns: turrets + gatling guns + rocket launchers + custom turret controllers.
// Used by the GunShooting condition so 'if turret shooting ...' picks up manual firing too.
List<IMyUserControllableGun> _allGuns = new List<IMyUserControllableGun>();
// Phase-7 block caches: gear, beacons, projectors, jump drives, gyros, parachutes,
// suspension wheels, sibling programmable blocks.
List<IMyLandingGear> _gears = new List<IMyLandingGear>();
List<IMyBeacon> _beacons = new List<IMyBeacon>();
List<IMyProjector> _projectors = new List<IMyProjector>();
List<IMyJumpDrive> _jumpDrives = new List<IMyJumpDrive>();
List<IMyGyro> _gyros = new List<IMyGyro>();
List<IMyParachute> _parachutes = new List<IMyParachute>();
List<IMyMotorSuspension> _wheels = new List<IMyMotorSuspension>();
List<IMyProgrammableBlock> _otherPBs = new List<IMyProgrammableBlock>();
List<IMyCargoContainer> _cargo = new List<IMyCargoContainer>();
List<IMyAirVent> _airVents = new List<IMyAirVent>();
List<IMySensorBlock> _sensors = new List<IMySensorBlock>();
List<IMyTerminalBlock> _allBlocks = new List<IMyTerminalBlock>();
// Ship controllers (cockpits, control seats, remote controls). Used for speed/altitude/pilot conditions.
List<IMyShipController> _shipControllers = new List<IMyShipController>();
double _shipSpeed = 0;       // m/s, magnitude of LinearVelocity from active controller (or first)
double _shipAltitude = 0;    // m above surface (0 if no planet)
bool _pilotPresent = false;  // any controller IsUnderControl
#endregion

#region PARSED RULE
// Tri-state for solar. Unknown until enough panel readings accumulate, then locks to Daylight or Dark.
// Never two-at-once — EvaluateCondition gates Dark/Daylight on (state == X && _solarAvailable).
enum SolarState { Unknown, Daylight, Dark }

// How a rule fires relative to its condition.
//   ContinuousIf  : 'if X ...' fires every tick the condition is true (debounced by cooldowns).
//   RisingEdgeWhen: 'when X starts ...' fires once on the false→true transition.
//   FallingEdgeWhen: 'when X ends ...' fires once on the true→false transition.
enum RuleMode { ContinuousIf, RisingEdgeWhen, FallingEdgeWhen }

// Boolean operator joining a rule's primary and secondary condition.
//   None = single condition. And/Or = two-condition rules.
enum CondJoin { None, And, Or }

enum CondType
{
    EnemyDetected, EnemyNotDetected,
    PowerBelow, PowerAbove, PowerFull,
    BatteryBelow,
    NamedBatteryBelow, NamedBatteryAbove,   // per-name/group battery threshold
    Dark, Daylight,
    SolarBelow, SolarAbove,
    ConnectorDocked, ConnectorUndocked,
    NamedConnectorDocked, NamedConnectorUndocked, // per-name/group connector state
    OxygenBelow, OxygenLow,
    HydrogenBelow, HydrogenLow,
    CargoAbove, CargoFull,
    NamedCargoAbove,                         // per-name/group cargo fill %
    ItemBelow,
    BlockIsOff, BlockIsOn,
    BlockDamaged, BlockNotDamaged,           // integrity-based: any block, or quoted name/group
    AirVentOff, AirVentOn,
    RefineryIdle, RefineryWorking,
    AssemblerIdle, AssemblerWorking,
    SpeedAbove, SpeedBelow,                  // m/s; needs a ship controller
    AltitudeAbove, AltitudeBelow,            // m above surface (planet)
    PilotPresent, PilotAbsent,
    GunShooting, GunNotShooting,             // any (or named/group) gun/turret firing right now
    ArgumentMatches,                         // 'if argument "X"' — checks _pendingArgument
    // Phase-7 additions
    GameHourIs, GameHourBefore, GameHourAfter,  // in-game day cycle (0..24)
    NightTime, DayTime,                          // 6..18 = day else night
    BlockCountBelow, BlockCountAbove, BlockCountIs, // count of a block type
    GunAmmoBelow,                                // named gun ammo magazines count
    InGravity, InSpace,
    GyroOverriding,                              // any gyro has GyroOverride enabled
    SensorTriggered,                             // named sensor IsActive
    VarAbove, VarBelow, VarIs,                   // counter compare
    Remembered, Forgotten,                       // memory bool (var > 0 / == 0)
    IGCReceived,                                 // 'if received "tag"'
    CameraSees, CameraSeesEnemy,                 // active raycast hit
    Always
}
enum ActType
{
    TurnOn, TurnOff,
    SetBatteriesRecharge, SetBatteriesAuto,
    CloseDoors, OpenDoors,
    CloseHangarDoors, OpenHangarDoors,
    SetLightColor,
    SetLightIntensity, SetLightBlink, SetLightRadius,
    PlaySoundBlocks, StopSoundBlocks,
    LockConnectors, UnlockConnectors,
    StockpileHydrogen, StockpileOxygen,
    RefillTurrets,
    GenerateMore,
    MoveOresToRefinery, MoveIngotsToAssembler,
    MoveItemsBetween,
    ShootOn, ShootOff, ShootOnce,
    PistonExtend, PistonRetract, PistonSetVelocity, PistonSetMaxLimit, PistonSetMinLimit,
    RotorRotate, RotorSetVelocity, RotorSetAngle, RotorLock, RotorUnlock,
    TimerTrigger, TimerStart, TimerStop,
    SorterDrainOn, SorterDrainOff,
    AntennaBroadcast,
    WriteToLCD,
    Say,
    // Phase-7 additions
    GearLock, GearUnlock,
    BeaconOn, BeaconOff, BeaconSetText,
    ProjectorOn, ProjectorOff,
    JumpDriveCharge, JumpDriveJump,
    LevelShip, ReleaseGyros,
    SetSoundVolume,
    ParachuteDeploy,
    WheelSetSpeed, WheelSetStrength, HandbrakeOn, HandbrakeOff,
    RunPB,
    // Variables / memory
    VarCount, VarAdd, VarSubtract, VarSet, VarReset, VarRemember, VarForget,
    // IGC + camera scan
    IGCBroadcast,
    CameraScanOn, CameraScanOff
}

// A single sub-action inside a rule. Rules can carry several (joined with "and" in the source line).
class ActionPart
{
    public ActType Action = ActType.TurnOn;
    public string ActTarget = "";   // "" = all matching blocks, else quoted name/group
    public string ActParam = "";    // color name, message text, etc.
    public string ItemName = "";    // for GenerateMore (taken from this part's text)
    public double DurationSec = 0;  // for PlaySoundBlocks: 0 = leave existing LoopPeriod
    public double NumParam = 0;     // numeric param: rotor angle°, piston velocity, light intensity, etc.
    public string SecondTarget = "";// 'write "MSG" to "LCD name"' — name of the LCD (ActTarget = MSG)
    public string RawLower = "";    // this segment's lowercased original text — used by smart targeter
}

class ParsedRule
{
    public string Raw = "";
    public string Label = "";       // optional [label] prefix — shown in trace if set
    public List<string> ModeTags = new List<string>(); // [mode X] tags — rule only runs if _activeMode is in this list (or list is empty)
    public int Priority = 0;        // higher wins on same-tick block conflicts; default 0
    public RuleMode Mode = RuleMode.ContinuousIf;
    public CondType Condition = CondType.Always;
    public double CondValue = 0;
    public string CondTarget = "";  // quoted block/group for block-state cond
    public string ItemName = "";    // for ItemBelow / generate (condition-side item)
    // Optional second condition (AND/OR). Join==None means only the primary is evaluated.
    public CondJoin Join = CondJoin.None;
    public CondType Cond2 = CondType.Always;
    public double Cond2Value = 0;
    public string Cond2Target = "";
    public string Cond2Item = "";
    public List<ActionPart> Actions = new List<ActionPart>();
    public List<ActionPart> OtherwiseActions = new List<ActionPart>(); // fires when ContinuousIf rule's condition is FALSE
    public int EveryNTicks = 0;     // 0 = no per-rule throttle; >0 = minimum ticks between fires
    public int LastFireTick = -100000; // bookkeeping for EveryNTicks
    public bool PrevCondMet = false;   // bookkeeping for edge triggers
    public bool Valid = false;
    public string ParseError = "";
}
#endregion

#region ITEM / BLUEPRINT MAPS
// Map 1: human alias → fully qualified inventory key "TypeShort/SubtypeId".
// TypeShort is the SE TypeId minus the "MyObjectBuilder_" prefix (e.g. "Component", "Ore", "Ingot",
// "AmmoMagazine"). The inventory cache is keyed the same way so ores and ingots that share a
// subtype name (e.g. "Iron") don't collide.
static readonly Dictionary<string, string> ItemAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // --- Components ---
        {"steel plate","Component/SteelPlate"},{"steel plates","Component/SteelPlate"},
        {"interior plate","Component/InteriorPlate"},{"interior plates","Component/InteriorPlate"},
        {"construction","Component/Construction"},{"construction component","Component/Construction"},{"construction components","Component/Construction"},
        {"computer","Component/Computer"},{"computers","Component/Computer"},{"computer component","Component/Computer"},{"computer components","Component/Computer"},
        {"motor","Component/Motor"},{"motors","Component/Motor"},{"motor component","Component/Motor"},{"motor components","Component/Motor"},
        {"display","Component/Display"},{"displays","Component/Display"},
        {"metal grid","Component/MetalGrid"},{"metal grids","Component/MetalGrid"},
        {"large steel tube","Component/LargeTube"},{"large steel tubes","Component/LargeTube"},{"large tube","Component/LargeTube"},{"large tubes","Component/LargeTube"},
        {"small steel tube","Component/SmallTube"},{"small steel tubes","Component/SmallTube"},{"small tube","Component/SmallTube"},{"small tubes","Component/SmallTube"},
        {"girder","Component/Girder"},{"girders","Component/Girder"},
        {"bulletproof glass","Component/BulletproofGlass"},
        {"reactor component","Component/Reactor"},{"reactor components","Component/Reactor"},
        {"thruster component","Component/Thrust"},{"thruster components","Component/Thrust"},
        {"medical component","Component/Medical"},{"medical components","Component/Medical"},
        {"power cell","Component/PowerCell"},{"power cells","Component/PowerCell"},
        {"solar cell","Component/SolarCell"},{"solar cells","Component/SolarCell"},
        {"superconductor","Component/Superconductor"},{"superconductors","Component/Superconductor"},
        {"radio","Component/RadioCommunication"},{"radio component","Component/RadioCommunication"},
        {"detector","Component/Detector"},{"detector component","Component/Detector"},
        {"gravity component","Component/GravityGenerator"},{"gravity generator component","Component/GravityGenerator"},
        // --- Ammo magazines ---
        {"gatling ammo","AmmoMagazine/NATO_25x184mm"},{"gatling magazine","AmmoMagazine/NATO_25x184mm"},{"nato","AmmoMagazine/NATO_25x184mm"},
        {"missile ammo","AmmoMagazine/Missile200mm"},{"missile","AmmoMagazine/Missile200mm"},{"missiles","AmmoMagazine/Missile200mm"},
        {"ammo","AmmoMagazine/NATO_25x184mm"},
        // --- Ores ---
        {"iron ore","Ore/Iron"},{"nickel ore","Ore/Nickel"},{"cobalt ore","Ore/Cobalt"},
        {"magnesium ore","Ore/Magnesium"},{"silicon ore","Ore/Silicon"},{"silver ore","Ore/Silver"},
        {"gold ore","Ore/Gold"},{"platinum ore","Ore/Platinum"},{"uranium ore","Ore/Uranium"},
        {"stone","Ore/Stone"},{"ice","Ore/Ice"},{"scrap","Ore/Scrap"},
        // --- Ingots ---
        {"iron ingot","Ingot/Iron"},{"iron ingots","Ingot/Iron"},
        {"nickel ingot","Ingot/Nickel"},{"nickel ingots","Ingot/Nickel"},
        {"cobalt ingot","Ingot/Cobalt"},{"cobalt ingots","Ingot/Cobalt"},
        {"magnesium powder","Ingot/Magnesium"},
        {"silicon wafer","Ingot/Silicon"},
        {"silver ingot","Ingot/Silver"},{"silver ingots","Ingot/Silver"},
        {"gold ingot","Ingot/Gold"},{"gold ingots","Ingot/Gold"},
        {"platinum ingot","Ingot/Platinum"},{"platinum ingots","Ingot/Platinum"},
        {"uranium ingot","Ingot/Uranium"},{"uranium ingots","Ingot/Uranium"},
        {"gravel","Ingot/Stone"},
    };

// Map 2: inventory SubtypeId → assembler blueprint definition ID.
// This is what gets passed to MyDefinitionId.TryParse and IMyAssembler.AddQueueItem.
static readonly Dictionary<string, string> InvSubtypeToBlueprint = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        {"SteelPlate",       "MyObjectBuilder_BlueprintDefinition/SteelPlate"},
        {"InteriorPlate",    "MyObjectBuilder_BlueprintDefinition/InteriorPlate"},
        {"Construction",     "MyObjectBuilder_BlueprintDefinition/ConstructionComponent"},
        {"Computer",         "MyObjectBuilder_BlueprintDefinition/ComputerComponent"},
        {"Motor",            "MyObjectBuilder_BlueprintDefinition/MotorComponent"},
        {"Display",          "MyObjectBuilder_BlueprintDefinition/Display"},
        {"MetalGrid",        "MyObjectBuilder_BlueprintDefinition/MetalGrid"},
        {"LargeTube",        "MyObjectBuilder_BlueprintDefinition/LargeTube"},
        {"SmallTube",        "MyObjectBuilder_BlueprintDefinition/SmallTube"},
        {"Girder",           "MyObjectBuilder_BlueprintDefinition/GirderComponent"},
        {"BulletproofGlass", "MyObjectBuilder_BlueprintDefinition/BulletproofGlass"},
        {"Reactor",          "MyObjectBuilder_BlueprintDefinition/ReactorComponent"},
        {"Thrust",           "MyObjectBuilder_BlueprintDefinition/ThrustComponent"},
        {"Medical",          "MyObjectBuilder_BlueprintDefinition/MedicalComponent"},
        {"PowerCell",        "MyObjectBuilder_BlueprintDefinition/PowerCell"},
        {"SolarCell",        "MyObjectBuilder_BlueprintDefinition/SolarCell"},
        {"Superconductor",   "MyObjectBuilder_BlueprintDefinition/Superconductor"},
        {"RadioCommunication","MyObjectBuilder_BlueprintDefinition/RadioCommunicationComponent"},
        {"Detector",         "MyObjectBuilder_BlueprintDefinition/DetectorComponent"},
        {"GravityGenerator", "MyObjectBuilder_BlueprintDefinition/GravityGeneratorComponent"},
        {"NATO_25x184mm",    "MyObjectBuilder_BlueprintDefinition/NATO_25x184mmMagazine"},
        {"Missile200mm",     "MyObjectBuilder_BlueprintDefinition/Missile200mm"},
    };
#endregion

#region CONSTRUCTOR
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    LoadVars();          // restore persisted counters/memory from PB CustomData
    RefreshBlocks();
    ParseRules();
    Log("MPX HLB started.");
}
#endregion

#region MAIN
public void Main(string argument, UpdateType updateSource)
{
    _tick++;

    if (!string.IsNullOrEmpty(argument))
    {
        string up = argument.Trim().ToUpperInvariant();
        // RUN/TRIGGER args set _pendingArgument and then fall through to rule evaluation so
        // 'if argument "X"' rules can fire this tick.
        bool isPassthroughArg = up.StartsWith("RUN ") || up.StartsWith("TRIGGER ");
        HandleArgument(up, argument.Trim());
        if (!isPassthroughArg) return;
    }

    // Expire any simulation overrides whose deadline has passed.
    if (_simulated.Count > 0)
    {
        var dead = new List<string>();
        foreach (var kv in _simulated) if (kv.Value <= _tick) dead.Add(kv.Key);
        foreach (var k in dead) _simulated.Remove(k);
    }

    if (_paused)
    {
        DrawCoreLCD();
        return;
    }

    // Periodic block refresh
    if (_tick % BLOCK_REFRESH_INTERVAL == 1)
        RefreshBlocks();

    // Detect MPX COMMAND edits every tick — auto-reload without recompile.
    // (Cheap: just string-compare against last known LCD content.)
    CheckCommandReload();

    // Safety net: re-parse on a slow cadence in case CheckCommandReload missed
    // a transient (e.g. LCD null on first ticks before RefreshBlocks).
    if (_tick % 300 == 2)
        ParseRules();

    // Advance animations
    if (_tick % 4 == 0) _spinnerIdx = (_spinnerIdx + 1) % 4;
    if (_tick % 6 == 0) _scanDotIdx = (_scanDotIdx + 1) % 3;

    // Decrement cooldowns
    var keys = new List<string>(_actionCooldowns.Keys);
    foreach (var k in keys)
    {
        _actionCooldowns[k]--;
        if (_actionCooldowns[k] <= 0) _actionCooldowns.Remove(k);
    }
    var pkeys = new List<string>(_produceCooldowns.Keys);
    foreach (var k in pkeys)
    {
        _produceCooldowns[k]--;
        if (_produceCooldowns[k] <= 0) _produceCooldowns.Remove(k);
    }

    // Gather sensor data
    GatherTelemetry();

    // Per-tick: stop sounds whose "for N seconds" deadline has elapsed.
    ProcessScheduledSoundStops();
    // Drain any inbound IGC messages so 'if received "tag"' conds see them this tick.
    DrainIGCMessages();

    // Evaluate and execute rules — clear per-tick conflict tracker so two opposing rules in the
    // same tick can be detected.
    _warnings.Clear();
    _tickDesiredEnabled.Clear();
    EvaluateRules();

    // Persist any variable changes from this tick's actions.
    SaveVarsIfDirty();

    // Clear single-tick pending argument so 'if argument "X"' rules only fire once per arg.
    _pendingArgument = "";

    // Draw LCDs
    DrawCoreLCD();
    DrawDisplayLCD();
}
#endregion

#region ARGUMENT HANDLER
void HandleArgument(string arg, string argOriginal)
{
    // ---- Multi-word arg handlers (must match BEFORE single-word switch) ----
    if (arg.StartsWith("MUTE "))
    { _mutedLabels.Add(argOriginal.Substring(5).Trim()); Log("MUTE: " + argOriginal.Substring(5).Trim()); return; }
    if (arg.StartsWith("UNMUTE "))
    { _mutedLabels.Remove(argOriginal.Substring(7).Trim()); Log("UNMUTE: " + argOriginal.Substring(7).Trim()); return; }
    if (arg.StartsWith("MODE "))
    { _activeMode = argOriginal.Substring(5).Trim().ToLowerInvariant(); Log("MODE: " + _activeMode); return; }
    if (arg.StartsWith("SIMULATE "))
    {
        string what = arg.Substring(9).Trim().ToLowerInvariant();
        if (what == "clear" || what == "off") { _simulated.Clear(); Log("SIMULATE cleared."); return; }
        // 10-second simulation window
        _simulated[what] = _tick + 60;
        Log("SIMULATE: " + what + " (10s)");
        return;
    }
    if (arg.StartsWith("RUN ") || arg.StartsWith("TRIGGER "))
    {
        // Player-driven argument trigger — exposes 'if argument "X"' rules to one tick of firing.
        int sp = argOriginal.IndexOf(' ');
        _pendingArgument = argOriginal.Substring(sp + 1).Trim();
        Log("ARG: " + _pendingArgument);
        return;
    }
    if (arg.StartsWith("TUTORIAL "))
    {
        string flavor = argOriginal.Substring(9).Trim().ToLowerInvariant();
        WriteTutorialToCommand(flavor);
        return;
    }

    switch (arg)
    {
        case "HELP":
            string help = BuildHelpText();
            Echo(help);
            if (_lcdCore != null) { _lcdCore.WriteText(help); }
            break;
        case "RELOAD":
            RefreshBlocks();
            ParseRules();
            Log("Manual reload.");
            break;
        case "TUTORIAL":
            WriteTutorialToCommand("");
            break;
        case "STATS":
            EchoRuleStats();
            break;
        case "DIAGNOSE":
            EchoDiagnose();
            break;
        case "DRY-RUN":
        case "DRYRUN":
            _dryRun = true;
            Log("DRY-RUN: rules will evaluate but no blocks will be touched.");
            break;
        case "LIVE":
            _dryRun = false;
            Log("LIVE: actions will fire normally.");
            break;
        case "SCAN":
            RefreshBlocks();
            Log("Block scan complete.");
            break;
        case "STATUS":
            Echo($"Rules: {_rules.Count} | Paused: {_paused} | Enemy: {_enemyDetected} | Solar: {_solarState}");
            break;
        case "CLEARLOG":
            _logLines.Clear();
            if (_lcdLog != null) _lcdLog.WriteText("");
            Log("Log cleared.");
            break;
        case "PAUSE":
            _paused = true;
            Log("System PAUSED.");
            break;
        case "RESUME":
            _paused = false;
            Log("System RESUMED.");
            break;
        default:
            Echo($"Unknown argument: {arg}. Try HELP.");
            break;
    }
}
#endregion

// HELP text — kept in its own builder to keep HandleArgument readable.
string BuildHelpText()
{
    var sb = new StringBuilder();
    sb.AppendLine("=== MPX HUMAN LOGIC BRAIN ===");
    sb.AppendLine("Write rules on MPX COMMAND in plain English. Run HELP any time.");
    sb.AppendLine("");
    sb.AppendLine("REQUIRED LCDs:");
    sb.AppendLine("  MPX HUMAN LOGIC BRAIN (this PB), MPX COMMAND, MPX CORE,");
    sb.AppendLine("  MPX DISPLAY, MPX LOG (optional)");
    sb.AppendLine("Optional group: MPX ENEMY SENSOR.  Day/night uses ALL solar panels.");
    sb.AppendLine("Works on ships AND bases (same-grid block detection).");
    sb.AppendLine("");
    sb.AppendLine("--- RULE SHAPE ---");
    sb.AppendLine("  if <cond> <action> [and <action>...] [otherwise <action>...]");
    sb.AppendLine("  when <cond> starts <action>...    (once on rising edge)");
    sb.AppendLine("  when <cond> ends <action>...      (once on falling edge)");
    sb.AppendLine("  every N seconds if <cond> <action>...   (rule throttle)");
    sb.AppendLine("TAGS before if/when (any order):");
    sb.AppendLine("  [label]            shown in trace");
    sb.AppendLine("  [mode combat]      only when MODE combat active");
    sb.AppendLine("  [priority 10]      higher wins same-tick conflicts");
    sb.AppendLine("Comment lines start with '#' or '//'.");
    sb.AppendLine("");
    sb.AppendLine("--- CONDITIONS ---");
    sb.AppendLine("  enemy detected / no enemy");
    sb.AppendLine("  power full / power above N / power below N");
    sb.AppendLine("  \"backup\" below 50%   \"backup\" above 80% (named battery)");
    sb.AppendLine("  dark / daylight / solar output above N");
    sb.AppendLine("  connector docked / connector undocked");
    sb.AppendLine("  \"main connector\" docked / \"main connector\" undocked");
    sb.AppendLine("  oxygen low / hydrogen low / cargo full / cargo above N");
    sb.AppendLine("  \"main storage\" cargo above 80 (named cargo)");
    sb.AppendLine("  <item> below N      (e.g. 'iron ingot below 500')");
    sb.AppendLine("  refinery idle / refinery working");
    sb.AppendLine("  assembler idle / assembler working");
    sb.AppendLine("  air vent on / air vent off");
    sb.AppendLine("  \"name\" is on / \"name\" is off / \"name\" damaged");
    sb.AppendLine("  block damaged / no damage");
    sb.AppendLine("  speed above N / speed below N (m/s)");
    sb.AppendLine("  altitude above N / altitude below N (m)");
    sb.AppendLine("  pilot present / no pilot");
    sb.AppendLine("  argument \"X\"     (fires when 'RUN X' arg is sent)");
    sb.AppendLine("  always");
    sb.AppendLine("Combine with AND/OR (max 2):");
    sb.AppendLine("  if dark and power below 30 ...");
    sb.AppendLine("  if enemy detected or power below 20 ...");
    sb.AppendLine("");
    sb.AppendLine("--- ACTIONS ---");
    sb.AppendLine("  turn <blocktype> on/off       turn \"name\" on/off");
    sb.AppendLine("  set batteries recharge/auto");
    sb.AppendLine("  open/close door               open/close hangar door");
    sb.AppendLine("  turn lights <color>           (red green blue orange white yellow cyan off)");
    sb.AppendLine("  set lights intensity N        blink lights N        set lights radius N");
    sb.AppendLine("  play \"sound\" [for N seconds]    stop \"sound\"");
    sb.AppendLine("  lock/unlock connectors");
    sb.AppendLine("  stockpile hydrogen / stockpile oxygen");
    sb.AppendLine("  refill turrets");
    sb.AppendLine("  generate more");
    sb.AppendLine("  move ores to refinery   move ingots to assembler");
    sb.AppendLine("  move \"iron ingot\" to \"main storage\"   (cross-inventory)");
    sb.AppendLine("  shoot turrets on/off/once     shoot \"front turret\" once");
    sb.AppendLine("  turn turret shoot on/off/once  (alt phrasing)");
    sb.AppendLine("  extend piston \"P1\"   retract piston \"P1\"");
    sb.AppendLine("  set piston \"P1\" velocity 2   set piston \"P1\" max limit 10");
    sb.AppendLine("  rotate rotor \"R1\"   lock rotor \"R1\"   unlock rotor \"R1\"");
    sb.AppendLine("  set rotor \"R1\" velocity 2   set rotor \"R1\" angle 90");
    sb.AppendLine("  trigger timer \"T1\"   start timer \"T1\"   stop timer \"T1\"");
    sb.AppendLine("  drain sorter \"S1\"   stop drain sorter \"S1\"");
    sb.AppendLine("  broadcast \"MSG\"   send signal \"MSG\"");
    sb.AppendLine("  write \"MSG\" to \"LCD NAME\"");
    sb.AppendLine("  say \"MSG\"   (writes to MPX DISPLAY + plays a ping)");
    sb.AppendLine("");
    sb.AppendLine("--- BLOCK TYPES (smart targeting) ---");
    sb.AppendLine("  reactor, battery, turret, gun, thruster, light, sound/alarm/siren,");
    sb.AppendLine("  hydrogen gen, oxygen gen, assembler, refinery, air vent, connector,");
    sb.AppendLine("  solar, door, piston, rotor, hinge, timer, sorter, antenna, camera");
    sb.AppendLine("");
    sb.AppendLine("--- ITEMS ---");
    sb.AppendLine("  ORES:    iron / nickel / cobalt / magnesium / silicon / silver / gold /");
    sb.AppendLine("           platinum / uranium ore, stone, ice, scrap");
    sb.AppendLine("  INGOTS:  iron / nickel / cobalt / silver / gold / platinum / uranium");
    sb.AppendLine("           ingot, silicon wafer, magnesium powder, gravel");
    sb.AppendLine("  COMP:    steel plate, interior plate, construction, computer, motor,");
    sb.AppendLine("           display, metal grid, small/large tube, girder, bulletproof glass,");
    sb.AppendLine("           power cell, solar cell, superconductor, radio, detector,");
    sb.AppendLine("           gravity/reactor/thruster/medical component");
    sb.AppendLine("  AMMO:    gatling ammo, missile");
    sb.AppendLine("");
    sb.AppendLine("--- EXAMPLES ---");
    sb.AppendLine("  [day] if daylight turn lights red and turn reactors off");
    sb.AppendLine("  [night] if dark turn lights green and turn reactors on");
    sb.AppendLine("  [alert] when enemy detected turn lights red and play \"alarm\" for 3 seconds and shoot turrets on");
    sb.AppendLine("  [clear] when enemy gone turn lights green and shoot turrets off");
    sb.AppendLine("  if \"backup\" below 50% set batteries recharge");
    sb.AppendLine("  if \"main connector\" docked turn \"dock light\" green otherwise turn \"dock light\" off");
    sb.AppendLine("  if cargo above 80 turn lights red otherwise turn lights green");
    sb.AppendLine("  every 5 seconds if cargo below 80 set piston \"drill\" velocity 0.05 and extend piston \"drill\"");
    sb.AppendLine("  if cargo above 90 set piston \"drill\" velocity 0");
    sb.AppendLine("  every 10 seconds rotate rotor \"drill base\" and set rotor \"drill base\" velocity 0.5");
    sb.AppendLine("  [feed] every 5 seconds if refinery idle move ores to refinery");
    sb.AppendLine("  [craft] every 5 seconds if assembler idle move ingots to assembler");
    sb.AppendLine("  when block damaged play \"siren\" for 2 seconds and write \"DAMAGE\" to \"MPX DISPLAY\"");
    sb.AppendLine("  if speed above 100 turn thrusters off otherwise turn thrusters on");
    sb.AppendLine("  if argument \"panic\" close all hangar doors and play \"alarm\" for 5 seconds");
    sb.AppendLine("");
    sb.AppendLine("--- PB ARGUMENTS ---");
    sb.AppendLine("  HELP RELOAD SCAN STATUS CLEARLOG PAUSE RESUME TUTORIAL");
    sb.AppendLine("  STATS DIAGNOSE DRY-RUN LIVE");
    sb.AppendLine("  MUTE <label>     UNMUTE <label>");
    sb.AppendLine("  MODE <name>      MODE default");
    sb.AppendLine("  SIMULATE <thing> SIMULATE clear");
    sb.AppendLine("    things: 'enemy detected', 'dark', 'daylight', 'docked'");
    sb.AppendLine("  RUN <arg>        (fires 'if argument \"<arg>\"' rules)");
    sb.AppendLine("  TUTORIAL [name]  (writes starter rules to MPX COMMAND if empty)");
    sb.AppendLine("    names: drill, repair, stealth, convoy, emergency, christmas, sentry, kills");
    sb.AppendLine("");
    sb.AppendLine("--- PHASE-7 ADDITIONS ---");
    sb.AppendLine("More conditions:");
    sb.AppendLine("  game hour is N / before N / after N");
    sb.AppendLine("  night time / day time");
    sb.AppendLine("  turret count below N / reactor count above N / ...");
    sb.AppendLine("  \"front gun\" ammo below 50");
    sb.AppendLine("  in gravity / in space");
    sb.AppendLine("  ship leveling / gyros overriding");
    sb.AppendLine("  \"perimeter\" triggered     (named sensor active)");
    sb.AppendLine("  count <name> above N / below N / is N");
    sb.AppendLine("  remembers <name> / forgot <name>");
    sb.AppendLine("  received \"tag\"           (an IGC message arrived)");
    sb.AppendLine("  camera \"front\" sees within 200");
    sb.AppendLine("  camera \"front\" sees enemy within 200");
    sb.AppendLine("More actions:");
    sb.AppendLine("  count <name>             reset <name>");
    sb.AppendLine("  add N to <name>          subtract N from <name>");
    sb.AppendLine("  set <name> to N          (variables persist via PB CustomData)");
    sb.AppendLine("  remember <name>          forget <name>");
    sb.AppendLine("  lock gear / unlock gear  (or named: 'lock gear \"front gear\"')");
    sb.AppendLine("  set beacon \"name\" on / off / set text \"DOWN\"");
    sb.AppendLine("  enable projector \"blueprint\" / disable projector \"X\"");
    sb.AppendLine("  charge jump drive        jump");
    sb.AppendLine("  level ship               release gyros");
    sb.AppendLine("  set \"alarm\" volume 80    (0..100)");
    sb.AppendLine("  deploy parachute");
    sb.AppendLine("  set wheels speed 50      set wheels strength 80");
    sb.AppendLine("  handbrake on / handbrake off");
    sb.AppendLine("  run \"Other PB\"           run \"Other PB\" with arg \"fire\"");
    sb.AppendLine("  broadcast \"MSG\" on channel \"tag\"   (IGC inter-grid)");
    sb.AppendLine("  enable raycast on camera \"front\"");
    sb.AppendLine("Rule tags:");
    sb.AppendLine("  [cooldown 5s]    per-rule throttle (alt to 'every N seconds')");
    sb.AppendLine("Comments: '#' and '//' for single line, /* ... */ for blocks.");
    return sb.ToString();
}

// Writes a starter ruleset (or a flavor-specific template) to MPX COMMAND.
// Won't overwrite existing rules — only fills an empty MPX COMMAND.
// `flavor` selects a recipe: "" / "default", "drill", "repair", "stealth", "convoy",
// "emergency", "christmas", "sentry", "kills" (counter demo).
void WriteTutorialToCommand(string flavor)
{
    if (_lcdCommand == null) { Echo("MPX COMMAND LCD not found."); return; }
    string current = _lcdCommand.GetText() ?? "";
    if (current.Trim().Length > 0)
    {
        Echo("MPX COMMAND already has rules — TUTORIAL refuses to overwrite. Clear it first.");
        return;
    }
    var sb = new StringBuilder();
    sb.AppendLine("# Tutorial: " + (string.IsNullOrEmpty(flavor) ? "default" : flavor));
    sb.AppendLine("# Edit lines, add your own, or run HELP to see all keywords.");
    sb.AppendLine("# '#' and '//' start comments.");
    sb.AppendLine("");
    switch (flavor)
    {
        case "drill":
            sb.AppendLine("# Drill platform: slowly extend a piston and rotate a rotor while cargo is below 80%.");
            sb.AppendLine("# Stop when cargo is full. Adjust piston \"drill\" and rotor \"drill base\" to your block names.");
            sb.AppendLine("[start drill] every 5 seconds if cargo below 80 set piston \"drill\" velocity 0.05 and extend piston \"drill\"");
            sb.AppendLine("[start spin] every 10 seconds if cargo below 80 rotate rotor \"drill base\" and set rotor \"drill base\" velocity 0.5");
            sb.AppendLine("[stop] if cargo above 90 set piston \"drill\" velocity 0 and set rotor \"drill base\" velocity 0");
            sb.AppendLine("[full] if cargo above 95 turn lights red and play \"alarm\" for 2 seconds");
            break;
        case "repair":
            sb.AppendLine("# Repair bay: when a ship is docked, enable projector and welders. Disable when undocked.");
            sb.AppendLine("[start repair] when \"repair connector\" docked turn welder on and enable projector \"repair projector\"");
            sb.AppendLine("[stop repair] when \"repair connector\" undocked turn welder off and disable projector \"repair projector\"");
            break;
        case "stealth":
            sb.AppendLine("# Stealth mode: kill antennas, dim lights, disable beacons.");
            sb.AppendLine("[stealth on] if argument \"stealth\" turn antenna off and turn beacon off and set lights intensity 0.5");
            sb.AppendLine("[stealth off] if argument \"loud\" turn antenna on and turn beacon on and set lights intensity 5");
            break;
        case "convoy":
            sb.AppendLine("# Convoy formation: leader broadcasts speed; followers match it.");
            sb.AppendLine("# Lead ship rule (set [mode lead] on leader PB, MODE lead):");
            sb.AppendLine("[mode lead] every 2 seconds broadcast \"100\" on channel \"convoy\"");
            sb.AppendLine("# Follower ship rule (default mode):");
            sb.AppendLine("if received \"convoy\" remember leader signal");
            sb.AppendLine("if remembers leader signal turn thrusters on otherwise turn thrusters off");
            break;
        case "emergency":
            sb.AppendLine("# Emergency mode: close everything, alarm, conserve power, broadcast distress.");
            sb.AppendLine("# Trigger with: MODE emergency");
            sb.AppendLine("[mode emergency] when enemy detected starts close all hangar doors and play \"siren\" for 10 seconds and turn lights red");
            sb.AppendLine("[mode emergency] if power below 50 set batteries recharge and stockpile hydrogen");
            sb.AppendLine("[mode emergency] every 30 seconds broadcast \"MAYDAY\" on channel \"distress\"");
            break;
        case "christmas":
            sb.AppendLine("# Christmas lights: alternating red/green pulse + jingle.");
            sb.AppendLine("every 5 seconds turn lights red otherwise turn lights green");
            sb.AppendLine("every 30 seconds play \"jingle\" for 5 seconds");
            sb.AppendLine("blink lights 1");
            break;
        case "sentry":
            sb.AppendLine("# Sentry mode: power down everything except turrets + sensors.");
            sb.AppendLine("[sentry] if argument \"sentry on\" turn thrusters off and turn assembler off and turn refinery off and turn lights off");
            sb.AppendLine("[sentry] if argument \"sentry off\" turn thrusters on and turn assembler on and turn refinery on and turn lights on");
            sb.AppendLine("[alert] when enemy detected starts shoot turrets on and play \"alarm\"");
            break;
        case "kills":
            sb.AppendLine("# Kill counter demo: count each detection, celebrate every 10.");
            sb.AppendLine("when enemy detected starts count kills");
            sb.AppendLine("if count kills is 10 play \"ace\" for 3 seconds and reset kills");
            sb.AppendLine("every 30 seconds write \"Kills: total \" to \"Score LCD\"");
            break;
        default: // "" or "default"
            sb.AppendLine("# --- Day / Night ---");
            sb.AppendLine("[day] if daylight turn lights red and turn reactors off");
            sb.AppendLine("[night] if dark turn lights green and turn reactors on");
            sb.AppendLine("");
            sb.AppendLine("# --- Enemy alarm ---");
            sb.AppendLine("[alert] when enemy detected play \"alarm\" for 3 seconds and turn lights red and shoot turrets on");
            sb.AppendLine("[clear] when enemy gone turn lights green and shoot turrets off");
            sb.AppendLine("");
            sb.AppendLine("# --- Power management ---");
            sb.AppendLine("if power below 30 set batteries recharge and stockpile hydrogen");
            sb.AppendLine("");
            sb.AppendLine("# --- Refinery & assembler feed ---");
            sb.AppendLine("[feed] every 5 seconds if refinery idle move ores to refinery");
            sb.AppendLine("[craft] every 5 seconds if assembler idle move ingots to assembler");
            sb.AppendLine("");
            sb.AppendLine("# --- Damage warning ---");
            sb.AppendLine("when block damaged play \"siren\" for 2 seconds and write \"DAMAGE\" to \"MPX DISPLAY\"");
            sb.AppendLine("");
            sb.AppendLine("# More tutorials available: TUTORIAL drill / repair / stealth / convoy /");
            sb.AppendLine("#                          emergency / christmas / sentry / kills");
            break;
    }
    sb.AppendLine("");
    sb.AppendLine("# --- Try editing or adding your own rules! ---");
    _lcdCommand.WriteText(sb.ToString());
    Log("TUTORIAL [" + flavor + "]: starter rules written to MPX COMMAND.");
}

// Echo per-rule firing counts to MPX LOG. Sorted by count desc.
void EchoRuleStats()
{
    var sb = new StringBuilder();
    sb.AppendLine("=== RULE FIRE STATS ===");
    var list = new List<KeyValuePair<string, int>>(_ruleFireCount);
    list.Sort((a, b) => b.Value.CompareTo(a.Value));
    foreach (var kv in list)
        sb.AppendLine($"  {kv.Value,5}  {kv.Key}");
    if (list.Count == 0) sb.AppendLine("  (no rules have fired yet)");
    Echo(sb.ToString());
    if (_lcdLog != null) _lcdLog.WriteText(sb.ToString());
}

// One-shot health check: lists discovered blocks, rule parse status, and flags rules that
// reference quoted block names which don't exist on the construct.
void EchoDiagnose()
{
    var sb = new StringBuilder();
    sb.AppendLine("=== MPX DIAGNOSE ===");
    sb.AppendLine($"Construct blocks discovered:");
    sb.AppendLine($"  Reactors:{_reactors.Count} Batteries:{_batteries.Count} Turrets:{_turrets.Count} Guns:{_allGuns.Count}");
    sb.AppendLine($"  Lights:{_lights.Count} Doors:{_doors.Count} Connectors:{_connectors.Count}");
    sb.AppendLine($"  Pistons:{_pistons.Count} Rotors:{_rotors.Count} Timers:{_timers.Count}");
    sb.AppendLine($"  Refineries:{_refineries.Count} Assemblers:{_assemblers.Count} Sorters:{_sorters.Count}");
    sb.AppendLine($"  Antennas:{_antennas.Count} Beacons:{_beacons.Count} Cameras:{_cameras.Count}");
    sb.AppendLine($"  Projectors:{_projectors.Count} Gyros:{_gyros.Count} Gears:{_gears.Count}");
    sb.AppendLine($"  Parachutes:{_parachutes.Count} JumpDrives:{_jumpDrives.Count} Wheels:{_wheels.Count}");
    sb.AppendLine($"  ShipCtrls:{_shipControllers.Count} OtherPBs:{_otherPBs.Count} Sensors:{_sensors.Count}");
    sb.AppendLine($"  Cargo:{_cargo.Count} Solar:{_solar.Count} TextPanels:{_textPanels.Count}");
    sb.AppendLine("");
    sb.AppendLine($"Rules: {_rules.Count}");
    int valid = 0; int withErrors = 0;
    foreach (var r in _rules) { if (r.Valid) valid++; else withErrors++; }
    sb.AppendLine($"  Valid: {valid}   Errors: {withErrors}");
    if (withErrors > 0)
    {
        sb.AppendLine("Parse errors:");
        foreach (var r in _rules)
            if (!r.Valid)
                sb.AppendLine("  ! " + (r.Raw.Length > 60 ? r.Raw.Substring(0, 60) + "..." : r.Raw) + "  →  " + r.ParseError);
    }
    // Rules referencing missing quoted targets
    var missing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var r in _rules)
    {
        if (!string.IsNullOrEmpty(r.CondTarget) && !TargetExists(r.CondTarget)) missing.Add(r.CondTarget);
        foreach (var a in r.Actions)
            if (!string.IsNullOrEmpty(a.ActTarget) && !TargetExists(a.ActTarget)) missing.Add(a.ActTarget);
    }
    if (missing.Count > 0)
    {
        sb.AppendLine("Quoted names not found on construct (typos?):");
        foreach (var n in missing) sb.AppendLine("  ? \"" + n + "\"");
    }
    sb.AppendLine("");
    sb.AppendLine($"Vars: {_vars.Count}");
    foreach (var kv in _vars) sb.AppendLine($"  {kv.Key} = {kv.Value}");
    sb.AppendLine($"Mode: {(string.IsNullOrEmpty(_activeMode) ? "default" : _activeMode)}");
    sb.AppendLine($"Muted: " + (_mutedLabels.Count == 0 ? "(none)" : string.Join(", ", new List<string>(_mutedLabels))));
    sb.AppendLine($"DryRun: {_dryRun}");
    Echo(sb.ToString());
    if (_lcdLog != null) _lcdLog.WriteText(sb.ToString());
}

bool TargetExists(string quoted)
{
    if (GridTerminalSystem.GetBlockGroupWithName(quoted) != null) return true;
    foreach (var b in _allBlocks)
        if (b.CustomName.Equals(quoted, StringComparison.OrdinalIgnoreCase) || NameMatch(b, quoted)) return true;
    return false;
}

#region BLOCK REFRESH

// Indirection getter for USE_DAYLIGHT_SENSOR_GROUP.
// Reading the const through a method call prevents the C# compiler from constant-folding
// the gated `if (...)` branches into "if (false) { ... }" and emitting CS0162 unreachable-code
// warnings. Method-call results aren't propagated to dead-code analysis at call sites.
bool UseDaylightSensorGroup() { return USE_DAYLIGHT_SENSOR_GROUP; }

void RefreshBlocks()
{
    _lcdCommand = GridTerminalSystem.GetBlockWithName(LCD_COMMAND) as IMyTextPanel;
    _lcdCore = GridTerminalSystem.GetBlockWithName(LCD_CORE) as IMyTextPanel;
    _lcdDisplay = GridTerminalSystem.GetBlockWithName(LCD_DISPLAY) as IMyTextPanel;
    _lcdLog = GridTerminalSystem.GetBlockWithName(LCD_LOG) as IMyTextPanel;

    _reactors.Clear(); GridTerminalSystem.GetBlocksOfType(_reactors, b => b.IsSameConstructAs(Me));
    _batteries.Clear(); GridTerminalSystem.GetBlocksOfType(_batteries, b => b.IsSameConstructAs(Me));
    _solar.Clear(); GridTerminalSystem.GetBlocksOfType(_solar, b => b.IsSameConstructAs(Me));
    _doors.Clear(); GridTerminalSystem.GetBlocksOfType(_doors, b => b.IsSameConstructAs(Me));
    _turrets.Clear(); GridTerminalSystem.GetBlocksOfType(_turrets, b => b.IsSameConstructAs(Me));
    _lights.Clear(); GridTerminalSystem.GetBlocksOfType(_lights, b => b.IsSameConstructAs(Me));
    _soundBlocks.Clear(); GridTerminalSystem.GetBlocksOfType(_soundBlocks, b => b.IsSameConstructAs(Me));
    _connectors.Clear(); GridTerminalSystem.GetBlocksOfType(_connectors, b => b.IsSameConstructAs(Me));
    _gasGens.Clear(); GridTerminalSystem.GetBlocksOfType(_gasGens, b => b.IsSameConstructAs(Me));
    _gasTanks.Clear(); GridTerminalSystem.GetBlocksOfType(_gasTanks, b => b.IsSameConstructAs(Me));
    _thrusters.Clear(); GridTerminalSystem.GetBlocksOfType(_thrusters, b => b.IsSameConstructAs(Me));
    _assemblers.Clear(); GridTerminalSystem.GetBlocksOfType(_assemblers, b => b.IsSameConstructAs(Me));
    _refineries.Clear(); GridTerminalSystem.GetBlocksOfType(_refineries, b => b.IsSameConstructAs(Me));
    _shipControllers.Clear(); GridTerminalSystem.GetBlocksOfType(_shipControllers, b => b.IsSameConstructAs(Me));
    _pistons.Clear(); GridTerminalSystem.GetBlocksOfType(_pistons, b => b.IsSameConstructAs(Me));
    _rotors.Clear(); GridTerminalSystem.GetBlocksOfType(_rotors, b => b.IsSameConstructAs(Me));
    _timers.Clear(); GridTerminalSystem.GetBlocksOfType(_timers, b => b.IsSameConstructAs(Me));
    _sorters.Clear(); GridTerminalSystem.GetBlocksOfType(_sorters, b => b.IsSameConstructAs(Me));
    _antennas.Clear(); GridTerminalSystem.GetBlocksOfType(_antennas, b => b.IsSameConstructAs(Me));
    _cameras.Clear(); GridTerminalSystem.GetBlocksOfType(_cameras, b => b.IsSameConstructAs(Me));
    _textPanels.Clear(); GridTerminalSystem.GetBlocksOfType(_textPanels, b => b.IsSameConstructAs(Me));
    _allGuns.Clear(); GridTerminalSystem.GetBlocksOfType(_allGuns, b => b.IsSameConstructAs(Me));
    _gears.Clear(); GridTerminalSystem.GetBlocksOfType(_gears, b => b.IsSameConstructAs(Me));
    _beacons.Clear(); GridTerminalSystem.GetBlocksOfType(_beacons, b => b.IsSameConstructAs(Me));
    _projectors.Clear(); GridTerminalSystem.GetBlocksOfType(_projectors, b => b.IsSameConstructAs(Me));
    _jumpDrives.Clear(); GridTerminalSystem.GetBlocksOfType(_jumpDrives, b => b.IsSameConstructAs(Me));
    _gyros.Clear(); GridTerminalSystem.GetBlocksOfType(_gyros, b => b.IsSameConstructAs(Me));
    _parachutes.Clear(); GridTerminalSystem.GetBlocksOfType(_parachutes, b => b.IsSameConstructAs(Me));
    _wheels.Clear(); GridTerminalSystem.GetBlocksOfType(_wheels, b => b.IsSameConstructAs(Me));
    _otherPBs.Clear(); GridTerminalSystem.GetBlocksOfType(_otherPBs, b => b.IsSameConstructAs(Me) && b != Me);
    _cargo.Clear(); GridTerminalSystem.GetBlocksOfType(_cargo, b => b.IsSameConstructAs(Me));
    _airVents.Clear(); GridTerminalSystem.GetBlocksOfType(_airVents, b => b.IsSameConstructAs(Me));
    _sensors.Clear(); GridTerminalSystem.GetBlocksOfType(_sensors, b => b.IsSameConstructAs(Me));

    _allBlocks.Clear(); GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_allBlocks, b => b.IsSameConstructAs(Me));

    // Optional dedicated daylight-sensor group. OFF by default (USE_DAYLIGHT_SENSOR_GROUP=false).
    // When disabled, day/night uses every solar panel on the construct automatically — players never
    // need to create a technical group just to get day/night working.
    _solarSensorPanels.Clear();
    if (UseDaylightSensorGroup())
    {
        var dlGrp = GridTerminalSystem.GetBlockGroupWithName(DAYLIGHT_SENSOR_GROUP);
        if (dlGrp != null)
        {
            var tmp = new List<IMyTerminalBlock>();
            dlGrp.GetBlocks(tmp);
            foreach (var b in tmp) { var sp = b as IMySolarPanel; if (sp != null) _solarSensorPanels.Add(sp); }
        }
    }

    if (_lcdCore != null) SetupLCD(_lcdCore);
    if (_lcdDisplay != null) SetupLCD(_lcdDisplay);
    if (_lcdLog != null) SetupLCD(_lcdLog);
}

void SetupLCD(IMyTextPanel lcd)
{
    lcd.ContentType = ContentType.TEXT_AND_IMAGE;
    lcd.Font = "Monospace";
    lcd.FontSize = 0.6f;
    lcd.TextPadding = 2f;
    lcd.BackgroundColor = Color.Black;
    lcd.FontColor = new Color(0, 220, 255);
}
#endregion

#region TELEMETRY
void GatherTelemetry()
{
    // Power
    double stored = 0, maxStored = 0;
    foreach (var b in _batteries) { stored += b.CurrentStoredPower; maxStored += b.MaxStoredPower; }
    _powerPct = maxStored > 0 ? stored / maxStored * 100.0 : 0;

    // Hydrogen
    double hStored = 0, hMax = 0;
    foreach (var t in _gasTanks)
    {
        if (IsHydrogenTank(t))
        {
            hStored += t.FilledRatio;
            hMax += 1.0;
        }
    }
    _hydrogenPct = hMax > 0 ? hStored / hMax * 100.0 : 0;

    // Oxygen
    double oStored = 0, oMax = 0;
    foreach (var t in _gasTanks)
    {
        if (IsOxygenTank(t))
        {
            oStored += t.FilledRatio;
            oMax += 1.0;
        }
    }
    _oxygenPct = oMax > 0 ? oStored / oMax * 100.0 : 0;

    // Cargo
    double cFill = 0, cMax = 0;
    foreach (var c in _cargo)
    {
        cFill += (double)c.GetInventory().CurrentVolume;
        cMax += (double)c.GetInventory().MaxVolume;
    }
    _cargoPct = cMax > 0 ? cFill / cMax * 100.0 : 0;

    // Solar — DEFAULT: use every solar panel on the construct. No technical group required.
    // OPT-IN: if USE_DAYLIGHT_SENSOR_GROUP=true and MPX DAYLIGHT SENSOR has panels, use only those.
    // Day/night state is decided by the BEST panel exposure %, so a single panel facing the sun is
    // enough — wrong-facing/shadowed panels can't drag us into "Dark" while the sun is clearly out.
    //
    // CRITICAL: per-panel exposure% = MaxOutput / panel-definition-max  (NOT CurrentOutput / MaxOutput).
    //   MaxOutput      = the panel's current sun-exposure-limited cap. Drops to 0 only when the
    //                    panel is shaded or it's actually night. LOAD-INDEPENDENT.
    //   CurrentOutput  = how much the panel is actually producing right now. GRID-LOAD DEPENDENT —
    //                    when batteries are full and nothing draws power, this reads 0 even in
    //                    full sunlight. Using it caused Jean's live-test bug: 6 panels, full sun,
    //                    100% PWR, no load → CurrentOutput=0 → SolarState locked to Dark.
    // Vanilla SE definition maxes: large solar panel = 0.16 MW, small = 0.04 MW (see PanelDefMaxMW).
    List<IMySolarPanel> solarSrc;
    if (UseDaylightSensorGroup() && _solarSensorPanels.Count > 0)
    { solarSrc = _solarSensorPanels; _solarSource = "MPX DAYLIGHT SENSOR"; }
    else if (_solar.Count > 0)
    { solarSrc = _solar; _solarSource = "all solar panels"; }
    else
    { solarSrc = null; _solarSource = "none"; }

    _solarPanelCount = solarSrc != null ? solarSrc.Count : 0;
    double bestPct = 0, sumPct = 0;
    int usableCount = 0;
    if (solarSrc != null)
    {
        foreach (var s in solarSrc)
        {
            if (!s.IsWorking) continue; // panel offline/damaged/disabled — not a valid sensor
            double defMax = PanelDefMaxMW(s);
            if (defMax <= 0) continue;
            double pct = (s.MaxOutput / defMax) * 100.0;
            if (pct > 100.0) pct = 100.0; // cap so modded high-output panels don't blow past 100
            if (pct > bestPct) bestPct = pct;
            sumPct += pct;
            usableCount++;
        }
    }
    _solarBestPct = bestPct;
    _solarAvgPct = usableCount > 0 ? sumPct / usableCount : 0;
    _solarPct = _solarBestPct; // legacy alias — SOL bar + SolarBelow/Above rules use best exposure

    // Availability = at least one functional (built + enabled + powered) panel exists.
    _solarAvailable = usableCount > 0;

    if (!_solarAvailable)
    {
        _solarState = SolarState.Unknown;
        _solarDarkTicks = 0;
        _solarLightTicks = 0;
        _solarWarning = (solarSrc == null || solarSrc.Count == 0)
            ? "No solar panels found — dark/daylight rules disabled."
            : "Solar panels offline/damaged — dark/daylight rules disabled.";
    }
    else
    {
        _solarWarning = "";
        // Day/night decided by BEST panel — at least one panel seeing the sun = daylight.
        if (_solarBestPct < SOLAR_DARK_THRESHOLD * 100.0)
        {
            _solarDarkTicks++;
            _solarLightTicks = 0;
            if (_solarDarkTicks >= SOLAR_HYSTERESIS_TICKS && _solarState != SolarState.Dark)
            { _solarState = SolarState.Dark; _lastSolarChangeTick = _tick; }
        }
        else if (_solarBestPct > SOLAR_LIGHT_THRESHOLD * 100.0)
        {
            _solarLightTicks++;
            _solarDarkTicks = 0;
            if (_solarLightTicks >= SOLAR_HYSTERESIS_TICKS && _solarState != SolarState.Daylight)
            { _solarState = SolarState.Daylight; _lastSolarChangeTick = _tick; }
        }
        // Between 5% and 20% — hold current state, decay both counters so noise can't flip us prematurely.
        else
        {
            if (_solarDarkTicks > 0) _solarDarkTicks--;
            if (_solarLightTicks > 0) _solarLightTicks--;
        }
    }

    // Connector docking
    _connectorDocked = false;
    foreach (var c in _connectors)
        if (c.Status == MyShipConnectorStatus.Connected) { _connectorDocked = true; break; }

    // Ship state — prefer the controller currently under user control; else any main controller; else any.
    _shipSpeed = 0; _shipAltitude = 0; _pilotPresent = false;
    IMyShipController best = null;
    foreach (var sc in _shipControllers)
    {
        if (sc.IsUnderControl) { _pilotPresent = true; best = sc; break; }
        if (best == null && sc.IsMainCockpit) best = sc;
    }
    if (best == null && _shipControllers.Count > 0) best = _shipControllers[0];
    if (best != null)
    {
        var v = best.GetShipVelocities().LinearVelocity;
        _shipSpeed = v.Length();
        double elev; if (best.TryGetPlanetElevation(MyPlanetElevation.Surface, out elev)) _shipAltitude = elev;
        var grav = best.GetNaturalGravity();
        _inGravity = grav.LengthSquared() > 0.01; // any natural gravity → on/near a planet
    }
    else { _inGravity = false; }

    // Game-hour estimate from best-solar-panel sun angle. With no solar panels,
    // fall back to wall-clock based on tick counter (rough but stable).
    if (_solarAvailable && _solarBestPct > 0)
    {
        // Best panel exposure roughly tracks sun angle. Map 0..100% → daylight hours 6..18.
        // Dark counts as 0..6 / 18..24 split. This is approximate but sufficient for "if night time" rules.
        _gameHour = 6.0 + (_solarBestPct / 100.0) * 12.0;
        if (_solarState == SolarState.Dark) _gameHour = (_tick / 600.0) % 6.0; // late-night drift
    }

    // Enemy detection
    DetectEnemy();
}

// Vanilla SE solar panel definition maxes — used to convert MaxOutput into a sun-exposure %.
// We normalize against this (not CurrentOutput) because CurrentOutput depends on grid LOAD:
// when batteries are full and nothing is drawing, CurrentOutput=0 even in full sunlight, which
// would otherwise force SolarState=Dark. MaxOutput tracks sun exposure independent of load.
// Modded panels may exceed these caps — the caller clamps the resulting % at 100.
double PanelDefMaxMW(IMySolarPanel panel)
{
    if (panel == null) return 0;
    // Large grid: 0.16 MW (160 kW).  Small grid: 0.04 MW (40 kW).
    return panel.CubeGrid.GridSizeEnum == MyCubeSize.Large ? 0.16 : 0.04;
}

void DetectEnemy()
{
    _enemyDetected = false;
    _enemySource = "none";

    // 1. Named sensor group
    var sensorGroup = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlockGroupWithName(SENSOR_GROUP)?.GetBlocks(sensorGroup);
    GridTerminalSystem.GetBlockGroupWithName(DETECT_GROUP)?.GetBlocks(sensorGroup);
    foreach (var b in sensorGroup)
    {
        var s = b as IMySensorBlock;
        if (s != null && s.IsActive) { _enemyDetected = true; _enemySource = "Sensor:" + s.CustomName; return; }
    }

    // 2. All sensors on grid
    foreach (var s in _sensors)
        if (s.IsActive) { _enemyDetected = true; _enemySource = "Sensor:" + s.CustomName; return; }

    // 3. Turret has a locked target
    foreach (var t in _turrets)
    {
        if (t.HasTarget) { _enemyDetected = true; _enemySource = "Turret:" + t.CustomName; return; }
    }
}
#endregion

#region PARSER

// Reads MPX COMMAND text (or PB Custom Data fallback). Returns "" when unavailable.
string ReadCommandText()
{
    if (_lcdCommand != null)
    {
        string t = _lcdCommand.GetText();
        return t ?? "";
    }
    return Me.CustomData ?? "";
}

// Detects MPX COMMAND edits and re-parses without requiring recompile.
// Called every Main() tick. Cheap: just a string compare.
void CheckCommandReload()
{
    string current = ReadCommandText();
    if (current == _lastCommandText) return;
    _lastCommandText = current;
    ParseRules();
    Log("MPX COMMAND changed — rules reloaded (" + _rules.Count + ")");
}

void ParseRules()
{
    _rules.Clear();
    string rawText = ReadCommandText();
    // Keep snapshot in sync so CheckCommandReload doesn't fire spuriously after manual ParseRules().
    _lastCommandText = rawText;

    if (string.IsNullOrWhiteSpace(rawText))
    {
        _lastUnderstood = "No rules found.";
        return;
    }

    // Strip /* block comments */ (may span lines).
    rawText = StripBlockComments(rawText);

    // Robust line split: handle \r\n (Windows), \n (Unix), and stray \r.
    // Without this, an LCD that produces \r\n could leave \r in each line; .Trim() handles
    // that, but we also drop empty entries here so the rule count matches what the player sees.
    var lines = rawText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    foreach (var rawLine in lines)
    {
        string line = rawLine.Trim();
        if (string.IsNullOrEmpty(line)) continue;
        if (line.StartsWith("#") || line.StartsWith("//")) continue;
        if (_rules.Count >= MAX_RULES) break;

        var rule = ParseLine(line);
        rule.Raw = line;
        _rules.Add(rule);
    }

    int valid = 0;
    foreach (var r in _rules) if (r.Valid) valid++;
    _lastUnderstood = valid + "/" + _rules.Count + " rules parsed OK";

    // Surface the first parse error on MPX CORE so typos are visible without DIAGNOSE.
    foreach (var r in _rules)
    {
        if (!r.Valid)
        {
            _lastUnderstood += " | ERR: " + (r.Raw.Length > 26 ? r.Raw.Substring(0, 26) + "..." : r.Raw) +
                               " — " + SuggestParseFix(r);
            break;
        }
    }
}

// Strips /* ... */ block comments. Supports multi-line. Single-line // and # are handled per-line later.
string StripBlockComments(string s)
{
    if (s.IndexOf("/*", StringComparison.Ordinal) < 0) return s;
    var sb = new StringBuilder(s.Length);
    int i = 0;
    while (i < s.Length)
    {
        if (i + 1 < s.Length && s[i] == '/' && s[i + 1] == '*')
        {
            int end = s.IndexOf("*/", i + 2, StringComparison.Ordinal);
            if (end < 0) break; // unterminated — drop rest
            i = end + 2;
            continue;
        }
        sb.Append(s[i]); i++;
    }
    return sb.ToString();
}

// Look at a failed rule and offer a one-line hint about what might be wrong.
// Conservative: only suggests fixes for clearly fixable typos.
string SuggestParseFix(ParsedRule r)
{
    string raw = (r.Raw ?? "").ToLowerInvariant();
    if (raw.Contains("shoo turret")) return "did you mean 'shoot turret'?";
    if (raw.Contains("turn lite") || raw.Contains("turn lights ")) return "name colors after 'lights' (red/green/blue/...)";
    if (raw.Contains(" of "))      return "try '<verb> <name> <value>' without 'of'";
    if (raw.Contains("shoting"))   return "did you mean 'shooting'?";
    if (raw.Contains("recharg") && !raw.Contains("recharge")) return "try 'set batteries recharge'";
    if (!raw.Contains(" "))        return "rules need a verb — try 'if <cond> <action>'";
    if (!raw.Contains("if ") && !raw.Contains("when ") && !raw.Contains("every "))
        return "rules start with 'if' or 'when' (or 'every N seconds if ...')";
    return r.ParseError ?? "syntax";
}

ParsedRule ParseLine(string line)
{
    var rule = new ParsedRule();
    string working = line.Trim();

    // ---- 1. Strip leading [tags] — label, [mode X], [priority N] ----
    while (working.StartsWith("["))
    {
        int closeBracket = working.IndexOf(']');
        if (closeBracket <= 0 || closeBracket > 60) break;
        string tag = working.Substring(1, closeBracket - 1).Trim();
        string tagLower = tag.ToLowerInvariant();
        if (tagLower.StartsWith("mode "))
        {
            foreach (var m in tag.Substring(5).Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                rule.ModeTags.Add(m.Trim().ToLowerInvariant());
        }
        else if (tagLower.StartsWith("priority"))
        {
            string rest = tag.Substring(8).Replace("=", " ").Trim();
            int p; if (int.TryParse(rest, out p)) rule.Priority = p;
        }
        else if (tagLower.StartsWith("cooldown"))
        {
            // [cooldown 5s] / [cooldown 5] / [cooldown=10]  — per-rule throttle in seconds
            string rest = tag.Substring(8).Replace("=", " ").Replace("s", " ").Trim();
            double n; if (double.TryParse(rest, out n)) rule.EveryNTicks = (int)(n * 6.0);
        }
        else if (string.IsNullOrEmpty(rule.Label)) rule.Label = tag;
        else rule.ModeTags.Add(tagLower);
        working = working.Substring(closeBracket + 1).TrimStart();
    }

    // ---- 2. Optional "every N <unit>" prefix → per-rule cooldown ----
    if (working.StartsWith("every ", StringComparison.OrdinalIgnoreCase))
    {
        int consumed; double seconds;
        if (TryParseEveryPrefix(working, out consumed, out seconds))
        {
            rule.EveryNTicks = (int)(seconds * 6.0);
            working = working.Substring(consumed).TrimStart();
        }
    }

    // ---- 3. Strip "if " / "when " to set rule mode ----
    if (working.StartsWith("if ", StringComparison.OrdinalIgnoreCase))
    { working = working.Substring(3).TrimStart(); rule.Mode = RuleMode.ContinuousIf; }
    else if (working.StartsWith("when ", StringComparison.OrdinalIgnoreCase))
    { working = working.Substring(5).TrimStart(); rule.Mode = RuleMode.RisingEdgeWhen; }

    // ---- 4. Build normalized lower (typo / spacing fixes) ----
    string lower = NormalizeLower(working);

    // ---- 5. Find action verb in lower AND in original-case so quoted names keep their case ----
    int actionStart = FindActionStart(lower);
    int actionStartOrig = FindActionStartInOriginal(working);
    if (actionStart < 0 || actionStartOrig < 0)
    { rule.ParseError = "No action found"; return rule; }

    string condPart = lower.Substring(0, actionStart).Trim();
    string actionPart = lower.Substring(actionStart).Trim();
    string actionPartOrig = working.Substring(actionStartOrig).Trim();

    // ---- 6. 'when' rules: check trailing 'starts/ends/gone/...' to set edge direction ----
    if (rule.Mode == RuleMode.RisingEdgeWhen)
    {
        if (EndsWithWord(condPart, "ends") || EndsWithWord(condPart, "stops") ||
            EndsWithWord(condPart, "gone") || EndsWithWord(condPart, "clears") ||
            EndsWithWord(condPart, "clear") || EndsWithWord(condPart, "ended"))
        { rule.Mode = RuleMode.FallingEdgeWhen; condPart = StripTrailingWord(condPart); }
        else if (EndsWithWord(condPart, "starts") || EndsWithWord(condPart, "begins") ||
                 EndsWithWord(condPart, "started") || EndsWithWord(condPart, "becomes"))
        { condPart = StripTrailingWord(condPart); }
        // After stripping, expand bare nouns to a recognized phrase the condition parser knows.
        string cPlain = condPart.Trim();
        if (cPlain == "enemy" || cPlain == "enemies") condPart = "enemy detected";
        else if (cPlain == "damage" || cPlain == "damaged") condPart = "block damaged";
    }

    ParseCondition(rule, condPart);

    // ---- 7. Split action on " otherwise " / " else " into then-actions and else-actions ----
    string thenLower = actionPart, thenOrig = actionPartOrig;
    string elseLower = "", elseOrig = "";
    int otherIdx = actionPart.IndexOf(" otherwise ", StringComparison.OrdinalIgnoreCase);
    int otherLen = 11;
    if (otherIdx < 0) { otherIdx = actionPart.IndexOf(" else ", StringComparison.OrdinalIgnoreCase); otherLen = 6; }
    if (otherIdx >= 0)
    {
        thenLower = actionPart.Substring(0, otherIdx).Trim();
        elseLower = actionPart.Substring(otherIdx + otherLen).Trim();
        int otherIdxOrig = actionPartOrig.IndexOf(" otherwise ", StringComparison.OrdinalIgnoreCase);
        int otherLenOrig = 11;
        if (otherIdxOrig < 0) { otherIdxOrig = actionPartOrig.IndexOf(" else ", StringComparison.OrdinalIgnoreCase); otherLenOrig = 6; }
        if (otherIdxOrig >= 0)
        {
            thenOrig = actionPartOrig.Substring(0, otherIdxOrig).Trim();
            elseOrig = actionPartOrig.Substring(otherIdxOrig + otherLenOrig).Trim();
        }
    }

    ParseAction(rule, thenLower, thenOrig);
    if (elseLower.Length > 0) ParseElseAction(rule, elseLower, elseOrig);

    rule.Valid = string.IsNullOrEmpty(rule.ParseError) && rule.Actions.Count > 0;
    return rule;
}

// Apply common typo / spacing fixes. Length may change (e.g. "day light"→"daylight").
string NormalizeLower(string s)
{
    return s.ToLowerInvariant()
            .Replace("less then", "less than")
            .Replace("more then", "more than")
            .Replace("airvent", "air vent")
            .Replace("day light", "daylight")
            .Replace("day time", "daytime")
            .Replace("night time", "nighttime");
}

// Parse "every N <unit>" prefix. Returns # chars consumed (so caller can strip).
// Unit: 's' / 'sec' / 'second(s)' / 'min' / 'minute(s)' / 'hour(s)'.
bool TryParseEveryPrefix(string s, out int consumed, out double seconds)
{
    consumed = 0; seconds = 0;
    if (!s.StartsWith("every ", StringComparison.OrdinalIgnoreCase)) return false;
    int i = 6;
    var sb = new StringBuilder();
    bool dot = false;
    while (i < s.Length && (char.IsDigit(s[i]) || (s[i] == '.' && !dot)))
    { if (s[i] == '.') dot = true; sb.Append(s[i]); i++; }
    double n;
    if (sb.Length == 0 || !double.TryParse(sb.ToString(), out n)) return false;
    while (i < s.Length && s[i] == ' ') i++;
    int unitStart = i;
    while (i < s.Length && char.IsLetter(s[i])) i++;
    string unit = s.Substring(unitStart, i - unitStart).ToLowerInvariant();
    if (unit.StartsWith("min")) n *= 60.0;
    else if (unit.StartsWith("hour")) n *= 3600.0;
    // 'sec'/'second(s)'/'s' all stay as seconds; an empty unit also stays as seconds.
    seconds = n;
    consumed = i;
    return true;
}

bool EndsWithWord(string s, string word)
{
    string t = s.TrimEnd();
    if (string.IsNullOrEmpty(t) || string.IsNullOrEmpty(word)) return false;
    if (!t.EndsWith(word, StringComparison.OrdinalIgnoreCase)) return false;
    int boundary = t.Length - word.Length;
    return boundary == 0 || !char.IsLetter(t[boundary - 1]);
}

string StripTrailingWord(string s)
{
    string t = s.TrimEnd();
    int i = t.Length - 1;
    while (i >= 0 && char.IsLetter(t[i])) i--;
    return t.Substring(0, i + 1).TrimEnd();
}

// Parse the "otherwise" half of a rule's action text into rule.OtherwiseActions.
void ParseElseAction(ParsedRule rule, string actLower, string actOrig)
{
    var lowerSegs = SplitOnAndQuoteAware(actLower);
    var origSegs = SplitOnAndQuoteAware(actOrig);
    for (int i = 0; i < lowerSegs.Count && rule.OtherwiseActions.Count < MAX_ACTIONS_PER_RULE; i++)
    {
        string lp = lowerSegs[i];
        string op = i < origSegs.Count ? origSegs[i] : lp;
        var part = ParseSingleActionSegment(lp, op);
        if (part != null) rule.OtherwiseActions.Add(part);
    }
}

// Split an action string on " and " — but not inside double-quoted block names.
// Used to support multi-action rules like:
//   if cond turn lights red and play "alarm" for 3 seconds and close door
const int MAX_ACTIONS_PER_RULE = 5;
List<string> SplitOnAndQuoteAware(string s)
{
    var parts = new List<string>();
    if (string.IsNullOrEmpty(s)) return parts;
    var cur = new StringBuilder();
    bool inQuote = false;
    int i = 0;
    while (i < s.Length)
    {
        char c = s[i];
        if (c == '"') { inQuote = !inQuote; cur.Append(c); i++; continue; }
        if (!inQuote && i + 5 <= s.Length
            && string.Compare(s, i, " and ", 0, 5, StringComparison.OrdinalIgnoreCase) == 0)
        {
            parts.Add(cur.ToString().Trim());
            cur.Length = 0;
            i += 5;
            continue;
        }
        cur.Append(c);
        i++;
    }
    string tail = cur.ToString().Trim();
    if (tail.Length > 0) parts.Add(tail);
    return parts;
}

// Parses " for N seconds" / " for N second" / " for Ns" out of a sound-action segment.
// Leaves DurationSec=0 when no suffix is found (executor then doesn't touch LoopPeriod).
void ParseDuration(ActionPart part, string actLower)
{
    int idx = actLower.IndexOf(" for ", StringComparison.Ordinal);
    if (idx < 0) return;
    string rest = actLower.Substring(idx + 5).TrimStart();
    var sb = new StringBuilder();
    bool hasDot = false;
    foreach (char c in rest)
    {
        if (char.IsDigit(c)) sb.Append(c);
        else if (c == '.' && !hasDot) { sb.Append(c); hasDot = true; }
        else break;
    }
    if (sb.Length == 0) return;
    double dur;
    if (double.TryParse(sb.ToString(), out dur) && dur > 0) part.DurationSec = dur;
}

// --- Condition Parser ---
// Top-level entry. Handles AND/OR splitting (max 2 sub-conditions) before delegating each
// sub-condition to ParsePrimaryCondition. Quote-aware so quoted names with " and " in them
// (e.g. "front and back guns") aren't split.
void ParseCondition(ParsedRule rule, string cond)
{
    int orIdx = FindCondBoundary(cond, " or ");
    int andIdx = FindCondBoundary(cond, " and ");
    int splitIdx = -1; CondJoin join = CondJoin.None; int joinLen = 0;
    // Whichever boundary appears first wins (left-to-right join).
    if (orIdx >= 0 && (andIdx < 0 || orIdx < andIdx)) { splitIdx = orIdx; join = CondJoin.Or; joinLen = 4; }
    else if (andIdx >= 0) { splitIdx = andIdx; join = CondJoin.And; joinLen = 5; }

    if (splitIdx > 0)
    {
        string first = cond.Substring(0, splitIdx).Trim();
        string second = cond.Substring(splitIdx + joinLen).Trim();
        ParsePrimaryCondition(rule, first);
        var temp = new ParsedRule();
        ParsePrimaryCondition(temp, second);
        rule.Join = join;
        rule.Cond2 = temp.Condition;
        rule.Cond2Value = temp.CondValue;
        rule.Cond2Target = temp.CondTarget;
        rule.Cond2Item = temp.ItemName;
    }
    else
    {
        ParsePrimaryCondition(rule, cond);
    }
}

// Quote-aware boundary search — won't match `needle` inside double quotes.
int FindCondBoundary(string s, string needle)
{
    bool inQuote = false;
    for (int i = 0; i + needle.Length <= s.Length; i++)
    {
        if (s[i] == '"') inQuote = !inQuote;
        if (!inQuote && string.Compare(s, i, needle, 0, needle.Length, StringComparison.OrdinalIgnoreCase) == 0)
            return i;
    }
    return -1;
}

void ParsePrimaryCondition(ParsedRule rule, string cond)
{
    // Bare action rules (no condition written) or explicit "always" fire every tick
    if (string.IsNullOrWhiteSpace(cond) || cond == "always")
    { rule.Condition = CondType.Always; return; }

    // Argument-as-condition: 'if argument "X" ...'
    if (Contains(cond, "argument") || Contains(cond, "command"))
    {
        string aq = ExtractQuoted(cond);
        if (!string.IsNullOrEmpty(aq))
        { rule.Condition = CondType.ArgumentMatches; rule.CondTarget = aq; return; }
    }

    // IGC: 'if received "tag" ...' — fires when a message tagged "tag" arrived this tick.
    if ((Contains(cond, "received") || Contains(cond, "got message") || Contains(cond, "signal arrives")) && !string.IsNullOrEmpty(ExtractQuoted(cond)))
    { rule.Condition = CondType.IGCReceived; rule.CondTarget = ExtractQuoted(cond); EnsureIGCListener(rule.CondTarget); return; }

    // Camera sees: 'if camera "X" sees something within 200' / 'if camera "X" sees enemy within 200'
    if (Contains(cond, "camera") && (Contains(cond, "sees") || Contains(cond, "detect") || Contains(cond, "spots")))
    {
        rule.Condition = Contains(cond, "enemy") || Contains(cond, "hostile") ? CondType.CameraSeesEnemy : CondType.CameraSees;
        rule.CondTarget = ExtractQuoted(cond);
        rule.CondValue = ExtractNumber(cond);
        if (rule.CondValue <= 0) rule.CondValue = 200; // default scan distance
        return;
    }

    // Time of day: 'if game hour is 6', 'if night time', 'if day time'
    if (Contains(cond, "night time") || Contains(cond, "nighttime")) { rule.Condition = CondType.NightTime; return; }
    if (Contains(cond, "day time") || Contains(cond, "daytime")) { rule.Condition = CondType.DayTime; return; }
    if (Contains(cond, "hour") || Contains(cond, "clock"))
    {
        if (Contains(cond, "after") || Contains(cond, "past")) { rule.Condition = CondType.GameHourAfter; rule.CondValue = ExtractNumber(cond); return; }
        if (Contains(cond, "before")) { rule.Condition = CondType.GameHourBefore; rule.CondValue = ExtractNumber(cond); return; }
        if (Contains(cond, " is ") || Contains(cond, "equals")) { rule.Condition = CondType.GameHourIs; rule.CondValue = ExtractNumber(cond); return; }
    }

    // In gravity / in space
    if (Contains(cond, "in gravity") || Contains(cond, "on planet") || Contains(cond, "in atmosphere"))
    { rule.Condition = CondType.InGravity; return; }
    if (Contains(cond, "in space") || Contains(cond, "no gravity"))
    { rule.Condition = CondType.InSpace; return; }

    // Gyro override active — 'if ship leveling' / 'if gyros overriding'
    if (Contains(cond, "leveling") || Contains(cond, "levelling") || (Contains(cond, "gyro") && (Contains(cond, "overrid") || Contains(cond, "active"))))
    { rule.Condition = CondType.GyroOverriding; return; }

    // Named sensor triggered: 'if "perimeter" triggered' / 'if sensor "X" active'
    if ((Contains(cond, "triggered") || Contains(cond, "sensor")) && !string.IsNullOrEmpty(ExtractQuoted(cond)) && !Contains(cond, "damaged"))
    {
        string sq = ExtractQuoted(cond);
        if (Contains(cond, "triggered") || Contains(cond, "active"))
        { rule.Condition = CondType.SensorTriggered; rule.CondTarget = sq; return; }
    }

    // Block count: 'if turret count below 5', 'if reactor count is 0'
    if (Contains(cond, "count") && !Contains(cond, "count of") && !Contains(cond, "remember"))
    {
        string blockType = ExtractBlockTypeKeyword(cond);
        if (!string.IsNullOrEmpty(blockType))
        {
            if (Contains(cond, "below") || Contains(cond, "less than") || Contains(cond, "under"))
            { rule.Condition = CondType.BlockCountBelow; rule.CondTarget = blockType; rule.CondValue = ExtractNumber(cond); return; }
            if (Contains(cond, "above") || Contains(cond, "more than") || Contains(cond, "over"))
            { rule.Condition = CondType.BlockCountAbove; rule.CondTarget = blockType; rule.CondValue = ExtractNumber(cond); return; }
            if (Contains(cond, " is ") || Contains(cond, "equals"))
            { rule.Condition = CondType.BlockCountIs; rule.CondTarget = blockType; rule.CondValue = ExtractNumber(cond); return; }
        }
    }

    // Named gun ammo: 'if "front gun" ammo below 50'
    if (Contains(cond, "ammo") && !string.IsNullOrEmpty(ExtractQuoted(cond)) && (Contains(cond, "below") || Contains(cond, "less than")))
    { rule.Condition = CondType.GunAmmoBelow; rule.CondTarget = ExtractQuoted(cond); rule.CondValue = ExtractNumber(cond); return; }

    // Variables — 'if count kills above 10', 'if count "kill count" is 0'
    if (Contains(cond, "count ") || cond.StartsWith("count "))
    {
        // After "count" word, the name extends until "above"/"below"/"is"/end. Quoted name also OK.
        string vname = ExtractQuoted(cond);
        if (string.IsNullOrEmpty(vname)) vname = ExtractVarName(cond, "count");
        if (!string.IsNullOrEmpty(vname))
        {
            if (Contains(cond, "above") || Contains(cond, "more than") || Contains(cond, "over"))
            { rule.Condition = CondType.VarAbove; rule.CondTarget = vname; rule.CondValue = ExtractNumber(cond); return; }
            if (Contains(cond, "below") || Contains(cond, "less than") || Contains(cond, "under"))
            { rule.Condition = CondType.VarBelow; rule.CondTarget = vname; rule.CondValue = ExtractNumber(cond); return; }
            if (Contains(cond, " is ") || Contains(cond, "equals"))
            { rule.Condition = CondType.VarIs; rule.CondTarget = vname; rule.CondValue = ExtractNumber(cond); return; }
        }
    }

    // Memory — 'if remembers alert', 'if forgot alert', 'if "alert" is remembered'
    if (Contains(cond, "remember") || Contains(cond, "remembers") || Contains(cond, "is remembered") || Contains(cond, "forgot") || Contains(cond, "forgotten"))
    {
        string vname = ExtractQuoted(cond);
        if (string.IsNullOrEmpty(vname)) vname = ExtractVarName(cond, "remember");
        if (string.IsNullOrEmpty(vname)) vname = ExtractVarName(cond, "forgot");
        if (!string.IsNullOrEmpty(vname))
        {
            bool wantOn = !Contains(cond, "forgot") && !Contains(cond, "forgotten") && !Contains(cond, "not remember");
            rule.Condition = wantOn ? CondType.Remembered : CondType.Forgotten;
            rule.CondTarget = vname;
            return;
        }
    }

    // Speed
    if (Contains(cond, "speed") || Contains(cond, "velocity"))
    {
        if (Contains(cond, "above") || Contains(cond, "more than") || Contains(cond, "over") || Contains(cond, "greater"))
        { rule.Condition = CondType.SpeedAbove; rule.CondValue = ExtractNumber(cond); return; }
        if (Contains(cond, "below") || Contains(cond, "less than") || Contains(cond, "under"))
        { rule.Condition = CondType.SpeedBelow; rule.CondValue = ExtractNumber(cond); return; }
    }
    // Altitude
    if (Contains(cond, "altitude") || Contains(cond, "elevation"))
    {
        if (Contains(cond, "above") || Contains(cond, "more than") || Contains(cond, "over"))
        { rule.Condition = CondType.AltitudeAbove; rule.CondValue = ExtractNumber(cond); return; }
        if (Contains(cond, "below") || Contains(cond, "less than") || Contains(cond, "under"))
        { rule.Condition = CondType.AltitudeBelow; rule.CondValue = ExtractNumber(cond); return; }
    }
    // Pilot
    if (Contains(cond, "pilot") || Contains(cond, "cockpit"))
    {
        if (Contains(cond, "no ") || Contains(cond, "not ") || Contains(cond, "empty") || Contains(cond, "absent"))
        { rule.Condition = CondType.PilotAbsent; return; }
        rule.Condition = CondType.PilotPresent; return;
    }
    // Block damage — accepts: "block damaged" / "any block damaged" / `"X" damaged` / `"X" is damaged`
    if (Contains(cond, "damaged") || Contains(cond, "damage") || Contains(cond, "hull damage") ||
        Contains(cond, "hull damaged") || Contains(cond, "is broken"))
    {
        string dq = ExtractQuoted(cond);
        bool wantNot = Contains(cond, "not damaged") || Contains(cond, "no damage") || Contains(cond, "undamaged");
        rule.Condition = wantNot ? CondType.BlockNotDamaged : CondType.BlockDamaged;
        rule.CondTarget = dq;
        return;
    }

    // Gun/turret currently firing — covers manual firing AND turret auto-fire.
    // Phrasings: "shooting", "firing", '"name" shooting', "turret shooting", "not shooting".
    if (Contains(cond, "shooting") || Contains(cond, "firing"))
    {
        string sq = ExtractQuoted(cond);
        bool wantNot = Contains(cond, "not shooting") || Contains(cond, "not firing") ||
                       Contains(cond, "stop shooting") || Contains(cond, "stops shooting") ||
                       Contains(cond, "stop firing") || Contains(cond, "stops firing") ||
                       Contains(cond, "no shooting") || Contains(cond, "not fire");
        rule.Condition = wantNot ? CondType.GunNotShooting : CondType.GunShooting;
        rule.CondTarget = sq;
        return;
    }

    // enemy
    if (Contains(cond, "enemy detected") || Contains(cond, "enemy near") || Contains(cond, "under attack"))
    { rule.Condition = CondType.EnemyDetected; return; }
    if (Contains(cond, "enemy not detected") || Contains(cond, "no enemy") || Contains(cond, "enemy clear"))
    { rule.Condition = CondType.EnemyNotDetected; return; }

    // power — accept a quoted name/group as a NamedBattery* variant first
    string powerQ = ExtractQuoted(cond);
    bool hasPowerKeyword = Contains(cond, "battery") || Contains(cond, "batteries") || Contains(cond, "charge") || Contains(cond, "%") || Contains(cond, "percent");
    if (!string.IsNullOrEmpty(powerQ) && hasPowerKeyword)
    {
        if (Contains(cond, "less than") || Contains(cond, "below") || Contains(cond, "under"))
        { rule.Condition = CondType.NamedBatteryBelow; rule.CondValue = ExtractNumber(cond); rule.CondTarget = powerQ; return; }
        if (Contains(cond, "more than") || Contains(cond, "above") || Contains(cond, "over"))
        { rule.Condition = CondType.NamedBatteryAbove; rule.CondValue = ExtractNumber(cond); rule.CondTarget = powerQ; return; }
    }
    if (Contains(cond, "power is full") || Contains(cond, "power full") || Contains(cond, "batteries full"))
    { rule.Condition = CondType.PowerFull; return; }
    if ((Contains(cond, "power") || Contains(cond, "batteries") || Contains(cond, "battery")) &&
        (Contains(cond, "less than") || Contains(cond, "below") || Contains(cond, "under")))
    { rule.Condition = CondType.PowerBelow; rule.CondValue = ExtractNumber(cond); return; }
    if ((Contains(cond, "power") || Contains(cond, "batteries") || Contains(cond, "battery")) &&
        (Contains(cond, "more than") || Contains(cond, "above") || Contains(cond, "over")))
    { rule.Condition = CondType.PowerAbove; rule.CondValue = ExtractNumber(cond); return; }

    // battery-specific low threshold (no quote)
    if (Contains(cond, "battery") &&
        (Contains(cond, "less than") || Contains(cond, "below")))
    { rule.Condition = CondType.BatteryBelow; rule.CondValue = ExtractNumber(cond); return; }

    // Dark FIRST so phrasings like "no sun detected" (which contain both "no sun" and "sun detected")
    // resolve to Dark per the user's semantic intent — the negation wins.
    // Synonyms: dark, night, nighttime, no sun, sun down, solar (panels) not detecting sun,
    //           solar output below/less than X.
    bool isDarkPhrase =
        Contains(cond, "dark") || Contains(cond, "night") || Contains(cond, "nighttime") ||
        Contains(cond, "no sun") || Contains(cond, "sun down") ||
        Contains(cond, "solar panels are not detecting sun") ||
        Contains(cond, "solar not detecting sun") ||
        Contains(cond, "solar panels are not") ||
        Contains(cond, "solar output is less than") || Contains(cond, "solar output below");
    if (isDarkPhrase)
    {
        if (Contains(cond, "less than") || Contains(cond, "below"))
        { rule.Condition = CondType.SolarBelow; rule.CondValue = ExtractNumber(cond); }
        else
        { rule.Condition = CondType.Dark; }
        return;
    }

    // Daylight synonyms: daylight, daytime, sun up, sun is up, sun detected,
    //                    solar (panels) detecting sun, solar output above/more than X.
    bool isDayPhrase =
        Contains(cond, "daylight") || Contains(cond, "daytime") ||
        Contains(cond, "sun up") || Contains(cond, "sun is up") || Contains(cond, "sun detected") ||
        Contains(cond, "solar panels detecting sun") || Contains(cond, "solar detecting sun") ||
        Contains(cond, "solar output is more than") || Contains(cond, "solar output above");
    if (isDayPhrase)
    {
        if (Contains(cond, "more than") || Contains(cond, "above"))
        { rule.Condition = CondType.SolarAbove; rule.CondValue = ExtractNumber(cond); }
        else
        { rule.Condition = CondType.Daylight; }
        return;
    }

    // connector — named first if quoted target present.
    string connQ = ExtractQuoted(cond);
    if (!string.IsNullOrEmpty(connQ) && (Contains(cond, "docked") || Contains(cond, "connected") || Contains(cond, "undocked") || Contains(cond, "disconnected")))
    {
        if (Contains(cond, "undocked") || Contains(cond, "disconnected") || Contains(cond, "not connected") || Contains(cond, "not docked"))
        { rule.Condition = CondType.NamedConnectorUndocked; rule.CondTarget = connQ; return; }
        rule.Condition = CondType.NamedConnectorDocked; rule.CondTarget = connQ; return;
    }
    // Generic (any-connector) — check negatives first so "undocked" etc. aren't shadowed.
    if (Contains(cond, "undocked") || Contains(cond, "connector disconnected") || Contains(cond, "not connected"))
    { rule.Condition = CondType.ConnectorUndocked; return; }
    if (Contains(cond, "connector connected") || Contains(cond, "docked") || Contains(cond, "connector is connected"))
    { rule.Condition = CondType.ConnectorDocked; return; }

    // oxygen
    if (Contains(cond, "oxygen is low") || Contains(cond, "o2 is low"))
    { rule.Condition = CondType.OxygenLow; return; }
    if ((Contains(cond, "oxygen") || Contains(cond, "o2")) &&
        (Contains(cond, "less than") || Contains(cond, "below")))
    { rule.Condition = CondType.OxygenBelow; rule.CondValue = ExtractNumber(cond); return; }

    // hydrogen
    if (Contains(cond, "hydrogen is low") || Contains(cond, "h2 is low"))
    { rule.Condition = CondType.HydrogenLow; return; }
    if ((Contains(cond, "hydrogen") || Contains(cond, "h2")) &&
        (Contains(cond, "less than") || Contains(cond, "below")))
    { rule.Condition = CondType.HydrogenBelow; rule.CondValue = ExtractNumber(cond); return; }

    // cargo — named-cargo variant first.
    string cargoQ = ExtractQuoted(cond);
    if (!string.IsNullOrEmpty(cargoQ) && Contains(cond, "cargo") &&
        (Contains(cond, "more than") || Contains(cond, "above") || Contains(cond, "over") ||
         Contains(cond, "is full") || Contains(cond, "full")))
    { rule.Condition = CondType.NamedCargoAbove; rule.CondTarget = cargoQ; rule.CondValue = Contains(cond, "full") ? 95.0 : ExtractNumber(cond); return; }
    if (Contains(cond, "cargo is full") || Contains(cond, "cargo full"))
    { rule.Condition = CondType.CargoFull; return; }
    if (Contains(cond, "cargo") && (Contains(cond, "more than") || Contains(cond, "above") || Contains(cond, "over")))
    { rule.Condition = CondType.CargoAbove; rule.CondValue = ExtractNumber(cond); return; }

    // inventory item threshold
    // e.g. "steel plates are less than 200"
    foreach (var alias in ItemAliases.Keys)
    {
        if (Contains(cond, alias) &&
            (Contains(cond, "less than") || Contains(cond, "below") || Contains(cond, "are less") || Contains(cond, "ammo is low")))
        {
            rule.Condition = CondType.ItemBelow;
            rule.ItemName = ItemAliases[alias];
            rule.CondValue = ExtractNumber(cond);
            if (Contains(cond, "ammo is low")) rule.CondValue = 100; // default low
            return;
        }
    }

    // Refinery state — "refinery is idle / working / producing", or `"refinery 1" is idle`.
    // Checked before generic block-state so the production-aware semantics win.
    if (Contains(cond, "refinery"))
    {
        string rq = ExtractQuoted(cond);
        bool wantIdle = Contains(cond, "idle") || Contains(cond, "not working") || Contains(cond, "not producing") || Contains(cond, "queue empty") || Contains(cond, "queue is empty");
        bool wantWorking = Contains(cond, "is working") || Contains(cond, "is producing") || Contains(cond, "is active") || Contains(cond, "producing");
        if (wantIdle) { rule.Condition = CondType.RefineryIdle; rule.CondTarget = rq; return; }
        if (wantWorking) { rule.Condition = CondType.RefineryWorking; rule.CondTarget = rq; return; }
    }

    // Assembler state — same semantics as refinery, slightly different keywords.
    if (Contains(cond, "assembler"))
    {
        string aq = ExtractQuoted(cond);
        bool wantIdle = Contains(cond, "idle") || Contains(cond, "queue empty") || Contains(cond, "queue is empty") || Contains(cond, "not working") || Contains(cond, "not producing");
        bool wantWorking = Contains(cond, "is working") || Contains(cond, "is producing") || Contains(cond, "queue not empty") || Contains(cond, "queue is not empty");
        if (wantIdle) { rule.Condition = CondType.AssemblerIdle; rule.CondTarget = aq; return; }
        if (wantWorking) { rule.Condition = CondType.AssemblerWorking; rule.CondTarget = aq; return; }
    }

    // block is on/off
    string quotedBlock = ExtractQuoted(cond);
    if (!string.IsNullOrEmpty(quotedBlock))
    {
        if (Contains(cond, "is off") || Contains(cond, "is disabled"))
        { rule.Condition = CondType.BlockIsOff; rule.CondTarget = quotedBlock; return; }
        if (Contains(cond, "is on") || Contains(cond, "is enabled"))
        { rule.Condition = CondType.BlockIsOn; rule.CondTarget = quotedBlock; return; }
    }

    // Air vent state — real condition (no longer an OxygenLow proxy).
    // Supports: "air vent is off", "air vent off", "air vent is on",
    //           'if "vent main" is off' (quoted vent/group target)
    if (Contains(cond, "air vent"))
    {
        string vq = ExtractQuoted(cond);
        bool wantOff = Contains(cond, "off") || Contains(cond, "disabled")
                    || Contains(cond, "broken") || Contains(cond, "not working");
        rule.Condition = wantOff ? CondType.AirVentOff : CondType.AirVentOn;
        rule.CondTarget = vq;
        return;
    }

    rule.ParseError = $"Unknown condition: {cond}";
}

// --- Action Parser ---
// Splits the action text on " and " (quote-aware) and parses each segment into its own
// ActionPart. Up to MAX_ACTIONS_PER_RULE parts per rule.
void ParseAction(ParsedRule rule, string actLower, string actOrig)
{
    var lowerSegs = SplitOnAndQuoteAware(actLower);
    var origSegs = SplitOnAndQuoteAware(actOrig);
    if (lowerSegs.Count == 0)
    {
        rule.ParseError = (rule.ParseError ?? "") + "|No action found";
        return;
    }
    for (int i = 0; i < lowerSegs.Count && rule.Actions.Count < MAX_ACTIONS_PER_RULE; i++)
    {
        string lp = lowerSegs[i];
        string op = i < origSegs.Count ? origSegs[i] : lp;
        var part = ParseSingleActionSegment(lp, op);
        if (part != null) rule.Actions.Add(part);
        else rule.ParseError = (rule.ParseError ?? "") + $"|Unknown action: {lp}";
    }
}

// Parses one " and "-separated segment into an ActionPart. Returns null if the segment
// can't be classified; caller appends an error to the rule.
ActionPart ParseSingleActionSegment(string actLower, string actOrig)
{
    var part = new ActionPart();
    string quoted = ExtractQuoted(actOrig);
    string quoted2 = ExtractSecondQuoted(actOrig);
    part.ActTarget = quoted;
    part.SecondTarget = quoted2;
    part.RawLower = actLower; // used by SetBlocksEnabled's smart targeter when ActTarget is empty

    // ---- 'say "X"' — text-to-LCD + ping sound. Edge-triggered like duration plays. ----
    if (actLower.StartsWith("say ") && !string.IsNullOrEmpty(quoted))
    { part.Action = ActType.Say; ParseDuration(part, actLower); return part; }

    // ---- 'write "MSG" to "LCD"' — writes MSG to the named LCD. ----
    if ((actLower.StartsWith("write ") || actLower.StartsWith("print ") || actLower.StartsWith("show ")) && !string.IsNullOrEmpty(quoted))
    { part.Action = ActType.WriteToLCD; return part; }

    // ---- Turret shoot — accept both shoot-first and turn-first phrasings. ----
    bool turretWord = Contains(actLower, "turret") || Contains(actLower, "gun");
    bool shootWord = Contains(actLower, "shoot") || Contains(actLower, "fire ");
    if (shootWord && turretWord)
    {
        if (Contains(actLower, "once")) { part.Action = ActType.ShootOnce; return part; }
        if (Contains(actLower, " off") || Contains(actLower, "stop")) { part.Action = ActType.ShootOff; return part; }
        part.Action = ActType.ShootOn; return part;
    }
    if (shootWord && Contains(actLower, "once")) { part.Action = ActType.ShootOnce; return part; }
    if (Contains(actLower, "stop shoot") || Contains(actLower, "cease fire")) { part.Action = ActType.ShootOff; return part; }

    // ---- Piston ----
    if (Contains(actLower, "piston"))
    {
        ParseNumParam(part, actLower);
        if (Contains(actLower, "extend") || Contains(actLower, "expand") || Contains(actLower, " out"))
        { part.Action = ActType.PistonExtend; return part; }
        if (Contains(actLower, "retract") || Contains(actLower, " in")) { part.Action = ActType.PistonRetract; return part; }
        if (Contains(actLower, "velocity") || Contains(actLower, "speed")) { part.Action = ActType.PistonSetVelocity; return part; }
        if (Contains(actLower, "max limit") || Contains(actLower, "max length")) { part.Action = ActType.PistonSetMaxLimit; return part; }
        if (Contains(actLower, "min limit") || Contains(actLower, "min length")) { part.Action = ActType.PistonSetMinLimit; return part; }
    }

    // ---- Rotor / Hinge ----
    if (Contains(actLower, "rotor") || Contains(actLower, "hinge"))
    {
        ParseNumParam(part, actLower);
        if (Contains(actLower, "lock")) { part.Action = ActType.RotorLock; return part; }
        if (Contains(actLower, "unlock")) { part.Action = ActType.RotorUnlock; return part; }
        if (Contains(actLower, "angle") || Contains(actLower, "degree")) { part.Action = ActType.RotorSetAngle; return part; }
        if (Contains(actLower, "velocity") || Contains(actLower, "rpm")) { part.Action = ActType.RotorSetVelocity; return part; }
        if (Contains(actLower, "rotate") || Contains(actLower, "spin") || Contains(actLower, "turn ")) { part.Action = ActType.RotorRotate; return part; }
    }

    // ---- Timer block ----
    if (Contains(actLower, "timer"))
    {
        if (Contains(actLower, "trigger")) { part.Action = ActType.TimerTrigger; return part; }
        if (Contains(actLower, "stop")) { part.Action = ActType.TimerStop; return part; }
        if (Contains(actLower, "start") || Contains(actLower, "run ")) { part.Action = ActType.TimerStart; return part; }
    }

    // ---- Sorter drain ----
    if (Contains(actLower, "sorter"))
    {
        if (Contains(actLower, "stop drain") || Contains(actLower, "drain off")) { part.Action = ActType.SorterDrainOff; return part; }
        if (Contains(actLower, "drain")) { part.Action = ActType.SorterDrainOn; return part; }
    }

    // ---- Antenna broadcast ----
    if ((actLower.StartsWith("broadcast ") || Contains(actLower, "send signal") || Contains(actLower, "antenna")) && !string.IsNullOrEmpty(quoted))
    { part.Action = ActType.AntennaBroadcast; return part; }

    // ---- Cross-inventory move: 'move "X" to "cargo b"' (target = LCD/cargo name) ----
    if ((actLower.StartsWith("move ") || actLower.StartsWith("send ")) && !string.IsNullOrEmpty(quoted) && !string.IsNullOrEmpty(quoted2) &&
        Contains(actLower, " to "))
    {
        part.Action = ActType.MoveItemsBetween;
        // Resolve quoted (item alias) → ItemName
        foreach (var alias in ItemAliases.Keys)
            if (Contains(quoted.ToLowerInvariant(), alias)) { part.ItemName = ItemAliases[alias]; break; }
        return part;
    }

    // ---- Variables / counters / memory ----
    if (actLower.StartsWith("count ") || actLower.StartsWith("increment "))
    {
        part.Action = ActType.VarCount;
        part.ActParam = ExtractVarNameForAction(actLower, actOrig, actLower.StartsWith("count ") ? "count" : "increment");
        return part;
    }
    if (actLower.StartsWith("reset ") || actLower.StartsWith("clear ")) {
        // Don't shadow 'clear log' / similar — guard
        string keyword = actLower.StartsWith("reset ") ? "reset" : "clear";
        string nm = ExtractVarNameForAction(actLower, actOrig, keyword);
        if (!string.IsNullOrEmpty(nm) && !nm.Equals("log", StringComparison.OrdinalIgnoreCase))
        { part.Action = ActType.VarReset; part.ActParam = nm; return part; }
    }
    if (actLower.StartsWith("remember ")) {
        part.Action = ActType.VarRemember;
        part.ActParam = ExtractVarNameForAction(actLower, actOrig, "remember");
        return part;
    }
    if (actLower.StartsWith("forget ")) {
        part.Action = ActType.VarForget;
        part.ActParam = ExtractVarNameForAction(actLower, actOrig, "forget");
        return part;
    }
    if (actLower.StartsWith("add ") && Contains(actLower, " to ")) {
        ParseNumParam(part, actLower);
        int toIdx = actLower.IndexOf(" to ", StringComparison.Ordinal);
        part.ActParam = ExtractVarTail(actOrig, toIdx + 4);
        part.Action = ActType.VarAdd;
        return part;
    }
    if (actLower.StartsWith("subtract ") && Contains(actLower, " from ")) {
        ParseNumParam(part, actLower);
        int fromIdx = actLower.IndexOf(" from ", StringComparison.Ordinal);
        part.ActParam = ExtractVarTail(actOrig, fromIdx + 6);
        part.Action = ActType.VarSubtract;
        return part;
    }
    if (actLower.StartsWith("set ") && Contains(actLower, " to ") && !HasBlockKeyword(actLower)) {
        // 'set kills to 100' — distinct from 'set lights intensity 5' (block keyword) or 'set batteries auto'
        ParseNumParam(part, actLower);
        int toIdx = actLower.IndexOf(" to ", StringComparison.Ordinal);
        string nm = actOrig.Substring(4, toIdx - 4).Trim().Trim('"');
        if (!string.IsNullOrEmpty(nm))
        { part.Action = ActType.VarSet; part.ActParam = nm; return part; }
    }

    // ---- IGC broadcast: 'broadcast "msg" on channel "tag"' (or just 'broadcast "msg"') ----
    if (actLower.StartsWith("broadcast ") || actLower.StartsWith("send signal ") || actLower.StartsWith("send "))
    {
        // First quoted = message, second quoted = channel (defaults to "MPX").
        if (!string.IsNullOrEmpty(quoted))
        { part.Action = ActType.IGCBroadcast; return part; }
    }

    // ---- Camera scan on/off ----
    if (Contains(actLower, "camera"))
    {
        if (Contains(actLower, "scan on") || Contains(actLower, "enable scan") || Contains(actLower, "enable raycast"))
        { part.Action = ActType.CameraScanOn; return part; }
        if (Contains(actLower, "scan off") || Contains(actLower, "disable scan") || Contains(actLower, "disable raycast"))
        { part.Action = ActType.CameraScanOff; return part; }
    }

    // ---- Landing gear ----
    if (Contains(actLower, "gear") || Contains(actLower, "landing"))
    {
        if (Contains(actLower, "lock")) { part.Action = ActType.GearLock; return part; }
        if (Contains(actLower, "unlock") || Contains(actLower, "release")) { part.Action = ActType.GearUnlock; return part; }
    }

    // ---- Beacon ----
    if (Contains(actLower, "beacon"))
    {
        if (Contains(actLower, "name") || Contains(actLower, "text") || (!string.IsNullOrEmpty(quoted) && (Contains(actLower, "set "))))
        { part.Action = ActType.BeaconSetText; return part; }
        if (Contains(actLower, " on") || Contains(actLower, "enable") || actLower.StartsWith("set beacon"))
        { part.Action = ActType.BeaconOn; return part; }
        if (Contains(actLower, " off") || Contains(actLower, "disable"))
        { part.Action = ActType.BeaconOff; return part; }
    }

    // ---- Projector ----
    if (Contains(actLower, "projector"))
    {
        if (Contains(actLower, "enable") || Contains(actLower, " on")) { part.Action = ActType.ProjectorOn; return part; }
        if (Contains(actLower, "disable") || Contains(actLower, " off")) { part.Action = ActType.ProjectorOff; return part; }
    }

    // ---- Jump drive ----
    if (Contains(actLower, "jump drive") || Contains(actLower, "jumpdrive"))
    {
        if (Contains(actLower, "charge") || Contains(actLower, " on")) { part.Action = ActType.JumpDriveCharge; return part; }
    }
    if (actLower.StartsWith("jump ") || actLower == "jump") { part.Action = ActType.JumpDriveJump; return part; }

    // ---- Gyro level ship / release ----
    if (Contains(actLower, "level ship") || (Contains(actLower, "level") && Contains(actLower, "gyro")))
    { part.Action = ActType.LevelShip; return part; }
    if (Contains(actLower, "release gyros") || Contains(actLower, "release gyro") || Contains(actLower, "free gyros"))
    { part.Action = ActType.ReleaseGyros; return part; }

    // ---- Sound volume ----
    if (Contains(actLower, "volume") && Contains(actLower, "sound") == false && Contains(actLower, "alarm") == false && Contains(actLower, "block") == false)
    {
        // Just 'set "X" volume N' — quoted=sound name, num=0..100
        ParseNumParam(part, actLower);
        part.Action = ActType.SetSoundVolume; return part;
    }
    if (Contains(actLower, "volume") && (Contains(actLower, "sound") || Contains(actLower, "alarm")))
    {
        ParseNumParam(part, actLower);
        part.Action = ActType.SetSoundVolume; return part;
    }

    // ---- Parachute ----
    if (Contains(actLower, "parachute") || actLower.StartsWith("deploy "))
    { part.Action = ActType.ParachuteDeploy; return part; }

    // ---- Wheels / handbrake ----
    if (Contains(actLower, "wheels") || Contains(actLower, "suspension"))
    {
        ParseNumParam(part, actLower);
        if (Contains(actLower, "strength") || Contains(actLower, "stiffness")) { part.Action = ActType.WheelSetStrength; return part; }
        if (Contains(actLower, "speed") || Contains(actLower, "velocity")) { part.Action = ActType.WheelSetSpeed; return part; }
    }
    if (Contains(actLower, "handbrake") || Contains(actLower, "hand brake") || Contains(actLower, "parking brake"))
    {
        if (Contains(actLower, " on") || Contains(actLower, "enable") || Contains(actLower, "engage")) { part.Action = ActType.HandbrakeOn; return part; }
        if (Contains(actLower, " off") || Contains(actLower, "disable") || Contains(actLower, "release")) { part.Action = ActType.HandbrakeOff; return part; }
    }

    // ---- Run another programmable block ----
    if ((actLower.StartsWith("run ") || actLower.StartsWith("invoke ")) && !string.IsNullOrEmpty(quoted))
    { part.Action = ActType.RunPB; return part; }

    // ---- Light effects (intensity, blink, radius) ----
    if (Contains(actLower, "light") || Contains(actLower, "lights"))
    {
        ParseNumParam(part, actLower);
        if (Contains(actLower, "intensity")) { part.Action = ActType.SetLightIntensity; return part; }
        if (Contains(actLower, "blink")) { part.Action = ActType.SetLightBlink; return part; }
        if (Contains(actLower, "radius")) { part.Action = ActType.SetLightRadius; return part; }
    }

    // Sound — accept "play "X"" without the literal word "sound", plus the legacy phrasings.
    // StartsWith guard keeps "display" / other words containing "play" from triggering.
    bool startsWithPlay = actLower.StartsWith("play ") || actLower == "play";
    bool startsWithStop = actLower.StartsWith("stop ") || actLower == "stop";
    if (startsWithPlay && !string.IsNullOrEmpty(quoted))
    { part.Action = ActType.PlaySoundBlocks; ParseDuration(part, actLower); return part; }
    if (Contains(actLower, "play sound") || Contains(actLower, "run sound") || Contains(actLower, "sound block") ||
        Contains(actLower, "alarm on") || Contains(actLower, "play alarm"))
    { part.Action = ActType.PlaySoundBlocks; ParseDuration(part, actLower); return part; }
    if (Contains(actLower, "stop sound") || Contains(actLower, "alarm off"))
    { part.Action = ActType.StopSoundBlocks; return part; }
    // 'stop "X"' (quoted target, no further qualifier) is unambiguously a sound stop.
    // Players who want to power-off a refinery use 'turn refinery off' / 'disable refinery'.
    if (startsWithStop && !string.IsNullOrEmpty(quoted))
    { part.Action = ActType.StopSoundBlocks; return part; }

    // Open/close doors first (before generic on/off)
    if (Contains(actLower, "close hangar") || (Contains(actLower, "close") && Contains(actLower, "hangar")))
    { part.Action = ActType.CloseHangarDoors; return part; }
    if (Contains(actLower, "open hangar") || (Contains(actLower, "open") && Contains(actLower, "hangar")))
    { part.Action = ActType.OpenHangarDoors; return part; }
    if (Contains(actLower, "close door") || (Contains(actLower, "close") && Contains(actLower, "door")))
    { part.Action = ActType.CloseDoors; return part; }
    if (Contains(actLower, "open door") || (Contains(actLower, "open") && Contains(actLower, "door")))
    { part.Action = ActType.OpenDoors; return part; }

    // Batteries mode
    if ((Contains(actLower, "batteries") || Contains(actLower, "battery")) && Contains(actLower, "recharge"))
    { part.Action = ActType.SetBatteriesRecharge; return part; }
    if ((Contains(actLower, "batteries") || Contains(actLower, "battery")) && Contains(actLower, "auto"))
    { part.Action = ActType.SetBatteriesAuto; return part; }

    // Connectors
    if (Contains(actLower, "lock connector") || Contains(actLower, "lock connect"))
    { part.Action = ActType.LockConnectors; return part; }
    if (Contains(actLower, "unlock connector") || Contains(actLower, "unlock connect"))
    { part.Action = ActType.UnlockConnectors; return part; }

    // Tanks stockpile
    if (Contains(actLower, "stockpile hydrogen") || (Contains(actLower, "stockpile") && Contains(actLower, "h2")))
    { part.Action = ActType.StockpileHydrogen; return part; }
    if (Contains(actLower, "stockpile oxygen") || (Contains(actLower, "stockpile") && Contains(actLower, "o2")))
    { part.Action = ActType.StockpileOxygen; return part; }

    // Refinery / assembler inventory feeds (move ores → refinery, ingots → assembler).
    if (Contains(actLower, "move ores to refinery") || Contains(actLower, "move ore to refinery") ||
        Contains(actLower, "feed refinery") || Contains(actLower, "send ores to refinery") ||
        Contains(actLower, "send ore to refinery"))
    { part.Action = ActType.MoveOresToRefinery; return part; }
    if (Contains(actLower, "move ingots to assembler") || Contains(actLower, "move ingot to assembler") ||
        Contains(actLower, "feed assembler") || Contains(actLower, "send ingots to assembler") ||
        Contains(actLower, "send ingot to assembler"))
    { part.Action = ActType.MoveIngotsToAssembler; return part; }

    // Production
    if (Contains(actLower, "generate more") || Contains(actLower, "queue") || Contains(actLower, "produce"))
    {
        part.Action = ActType.GenerateMore;
        foreach (var alias in ItemAliases.Keys)
            if (Contains(actLower, alias)) { part.ItemName = ItemAliases[alias]; break; }
        return part;
    }
    if (Contains(actLower, "refill turret") || Contains(actLower, "rearm"))
    { part.Action = ActType.RefillTurrets; return part; }

    // Light color
    if (Contains(actLower, "light") || Contains(actLower, "lights"))
    {
        string color = ExtractColor(actLower);
        if (!string.IsNullOrEmpty(color))
        { part.Action = ActType.SetLightColor; part.ActParam = color; return part; }
    }

    // Generic Turn On/Off — detect block type from keywords in this segment only
    bool isTurnOn = Contains(actLower, "turn on") || Contains(actLower, " on") || Contains(actLower, "enable") || Contains(actLower, "activate");
    bool isTurnOff = Contains(actLower, "turn off") || Contains(actLower, " off") || Contains(actLower, "disable") || Contains(actLower, "deactivate");
    if (isTurnOff) { part.Action = ActType.TurnOff; return part; }
    if (isTurnOn) { part.Action = ActType.TurnOn; return part; }
    if (Contains(actLower, "run ") || Contains(actLower, "start "))
    { part.Action = ActType.TurnOn; return part; }
    if (Contains(actLower, "turn ") && HasBlockKeyword(actLower))
    { part.Action = ActType.TurnOn; return part; }

    return null;
}

// Returns true when the action text mentions a block type the smart-targeter understands.
// Used by the "turn <block>" → TurnOn shorthand inference (Fix 5).
bool HasBlockKeyword(string actLower)
{
    return Contains(actLower, "reactor")
        || Contains(actLower, "batter")
        || Contains(actLower, "turret") || Contains(actLower, "gun")
        || Contains(actLower, "thruster")
        || Contains(actLower, "light")
        || Contains(actLower, "sound") || Contains(actLower, "alarm") || Contains(actLower, "siren")
        || Contains(actLower, "h2 gen") || Contains(actLower, "hydrogen gen")
        || Contains(actLower, "gas gen") || Contains(actLower, "oxygen gen")
        || Contains(actLower, "assembler")
        || Contains(actLower, "refinery")
        || Contains(actLower, "air vent")
        || Contains(actLower, "connector")
        || Contains(actLower, "solar")
        || Contains(actLower, "piston")
        || Contains(actLower, "rotor") || Contains(actLower, "hinge")
        || Contains(actLower, "timer")
        || Contains(actLower, "sorter")
        || Contains(actLower, "antenna") || Contains(actLower, "beacon")
        || Contains(actLower, "camera")
        || Contains(actLower, "projector")
        || Contains(actLower, "gear") || Contains(actLower, "landing")
        || Contains(actLower, "jump drive") || Contains(actLower, "jumpdrive")
        || Contains(actLower, "gyro")
        || Contains(actLower, "parachute")
        || Contains(actLower, "wheel") || Contains(actLower, "suspension")
        || Contains(actLower, "drill") || Contains(actLower, "welder") || Contains(actLower, "grinder");
}

// --- Helpers ---
int FindActionStart(string lower)
{
    string[] actionVerbs = {
            "close hangar","open hangar","close all hangar",
            "close door","open door","close all","open all",
            "turn on ","turn off ","turn all ","turn ",
            "set ","lock ","unlock ","play ","stop ","run ",
            "generate","queue","produce","refill","rearm",
            "stockpile","write ","print ","show ",
            "enable ","disable ","activate ","deactivate ",
            "move ","feed ","send ",
            "shoot ","fire ","cease ",
            "extend ","retract ","rotate ","spin ",
            "trigger ","start ","drain ","broadcast ",
            "say ","blink ",
            // Phase 7
            "count ","increment ","reset ","clear ",
            "remember ","forget ","add ","subtract ",
            "level ","deploy ","charge ","jump ",
            "engage ","invoke "
        };
    foreach (var v in actionVerbs)
    {
        int idx = lower.IndexOf(v, StringComparison.Ordinal);
        if (idx >= 0) return idx;
    }
    return -1;
}

int FindActionStartInOriginal(string original)
{
    return FindActionStart(original.ToLowerInvariant());
}

bool Contains(string src, string sub)
    => src.IndexOf(sub, StringComparison.OrdinalIgnoreCase) >= 0;

double ExtractNumber(string s)
{
    var sb = new StringBuilder();
    bool hasDot = false;
    foreach (char c in s)
    {
        if (char.IsDigit(c)) sb.Append(c);
        else if (c == '.' && !hasDot) { sb.Append(c); hasDot = true; }
        else if (sb.Length > 0 && !char.IsDigit(c) && c != '.') break;
    }
    double result;
    double.TryParse(sb.ToString(), out result);
    return result;
}

string ExtractQuoted(string s)
{
    int start = s.IndexOf('"');
    if (start < 0) return "";
    int end = s.IndexOf('"', start + 1);
    if (end < 0) return "";
    return s.Substring(start + 1, end - start - 1);
}

// Extract the SECOND double-quoted string (for 'write "MSG" to "LCD"' / 'move "X" to "Y"').
string ExtractSecondQuoted(string s)
{
    int first = s.IndexOf('"');
    if (first < 0) return "";
    int firstClose = s.IndexOf('"', first + 1);
    if (firstClose < 0) return "";
    int second = s.IndexOf('"', firstClose + 1);
    if (second < 0) return "";
    int secondClose = s.IndexOf('"', second + 1);
    if (secondClose < 0) return "";
    return s.Substring(second + 1, secondClose - second - 1);
}

// Extract a variable NAME from an action like 'count kills' / 'remember "kill count"'.
// Returns the words after the keyword, stopping at " and " or end-of-segment.
string ExtractVarNameForAction(string actLower, string actOrig, string keyword)
{
    int idx = actLower.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
    if (idx < 0) return "";
    int start = idx + keyword.Length;
    while (start < actLower.Length && actLower[start] == ' ') start++;
    // Stop at "and" boundary (already pre-split, but defensive)
    int stop = actLower.Length;
    int andIdx = actLower.IndexOf(" and ", start, StringComparison.OrdinalIgnoreCase);
    if (andIdx >= 0) stop = andIdx;
    int byIdx = actLower.IndexOf(" by ", start, StringComparison.OrdinalIgnoreCase);
    if (byIdx >= 0 && byIdx < stop) stop = byIdx;
    string name = actOrig.Substring(Math.Min(start, actOrig.Length), Math.Min(stop, actOrig.Length) - Math.Min(start, actOrig.Length)).Trim();
    if (name.Length >= 2 && name[0] == '"' && name[name.Length - 1] == '"') name = name.Substring(1, name.Length - 2);
    return name;
}

// Slice the tail of `actOrig` starting at index `startIdx`. Used for 'add N to <name>' patterns.
string ExtractVarTail(string actOrig, int startIdx)
{
    if (startIdx < 0 || startIdx >= actOrig.Length) return "";
    string tail = actOrig.Substring(startIdx).Trim();
    if (tail.Length >= 2 && tail[0] == '"' && tail[tail.Length - 1] == '"') tail = tail.Substring(1, tail.Length - 2);
    return tail;
}

// Pull the first numeric token out of `actLower` and store on part.NumParam.
// Used for piston velocity, rotor angle, light intensity, etc.
void ParseNumParam(ActionPart part, string actLower)
{
    var sb = new StringBuilder();
    bool dot = false;
    bool seenDigit = false;
    foreach (char c in actLower)
    {
        if (char.IsDigit(c)) { sb.Append(c); seenDigit = true; }
        else if (c == '.' && !dot && seenDigit) { sb.Append(c); dot = true; }
        else if (c == '-' && !seenDigit) { sb.Append(c); }
        else if (seenDigit) break;
    }
    double n;
    if (sb.Length > 0 && double.TryParse(sb.ToString(), out n)) part.NumParam = n;
}

string ExtractColor(string s)
{
    if (Contains(s, " red")) return "red";
    if (Contains(s, " green")) return "green";
    if (Contains(s, " blue")) return "blue";
    if (Contains(s, " orange")) return "orange";
    if (Contains(s, " white")) return "white";
    if (Contains(s, " yellow")) return "yellow";
    if (Contains(s, " cyan")) return "cyan";
    if (Contains(s, " off")) return "off";
    return "";
}
#endregion

#region EVALUATE RULES
void EvaluateRules()
{
    // Rules are evaluated in priority-descending order so high-priority writers win
    // any same-tick conflict (ApplyEnabled also tracks priority on _tickDesiredEnabled).
    var ordered = new List<ParsedRule>(_rules);
    ordered.Sort((a, b) => b.Priority.CompareTo(a.Priority));

    foreach (var rule in ordered)
    {
        if (!rule.Valid) continue;
        // Mute by label (PB arg MUTE <label>)
        if (!string.IsNullOrEmpty(rule.Label) && _mutedLabels.Contains(rule.Label)) continue;
        // Mode gating: if rule has ModeTags, only run when _activeMode matches one of them
        if (rule.ModeTags.Count > 0 && !rule.ModeTags.Contains(_activeMode)) continue;

        bool condMet = EvaluateCondition(rule);
        bool prevMet = rule.PrevCondMet;
        rule.PrevCondMet = condMet;

        bool shouldFireThen = false, shouldFireElse = false;
        switch (rule.Mode)
        {
            case RuleMode.ContinuousIf:
                shouldFireThen = condMet;
                shouldFireElse = !condMet && rule.OtherwiseActions.Count > 0;
                break;
            case RuleMode.RisingEdgeWhen:
                shouldFireThen = condMet && !prevMet;
                break;
            case RuleMode.FallingEdgeWhen:
                shouldFireThen = !condMet && prevMet;
                break;
        }

        // Per-rule cooldown (every N seconds)
        if (rule.EveryNTicks > 0 && (_tick - rule.LastFireTick) < rule.EveryNTicks)
        { shouldFireThen = false; shouldFireElse = false; }

        if (shouldFireThen)
        {
            _lastTriggered = TraceTag(rule);
            rule.LastFireTick = _tick;
            BumpRuleFireCount(rule);
            ExecuteAction(rule, rule.Actions, condMet, prevMet);
        }
        else if (shouldFireElse)
        {
            _lastTriggered = TraceTag(rule) + " (else)";
            rule.LastFireTick = _tick;
            BumpRuleFireCount(rule);
            ExecuteAction(rule, rule.OtherwiseActions, condMet, prevMet);
        }
    }
}

bool EvaluateCondition(ParsedRule rule)
{
    bool a = EvalSingleCondition(rule.Condition, rule.CondValue, rule.CondTarget, rule.ItemName);
    if (rule.Join == CondJoin.None) return a;
    bool b = EvalSingleCondition(rule.Cond2, rule.Cond2Value, rule.Cond2Target, rule.Cond2Item);
    return rule.Join == CondJoin.And ? (a && b) : (a || b);
}

bool EvalSingleCondition(CondType cond, double value, string target, string item)
{
    // SIMULATE overrides — let players test rules without real conditions.
    if (_simulated.Count > 0)
    {
        if ((cond == CondType.EnemyDetected) && (_simulated.ContainsKey("enemy detected") || _simulated.ContainsKey("enemy"))) return true;
        if ((cond == CondType.Dark) && _simulated.ContainsKey("dark")) return true;
        if ((cond == CondType.Daylight) && _simulated.ContainsKey("daylight")) return true;
        if ((cond == CondType.ConnectorDocked) && _simulated.ContainsKey("docked")) return true;
    }

    switch (cond)
    {
        case CondType.EnemyDetected: return _enemyDetected;
        case CondType.EnemyNotDetected: return !_enemyDetected;
        case CondType.PowerBelow: return _powerPct < value;
        case CondType.PowerAbove: return _powerPct > value;
        case CondType.PowerFull: return _powerPct >= 98.0;
        case CondType.BatteryBelow: return _powerPct < value;
        case CondType.NamedBatteryBelow: return NamedBatteryPct(target) < value;
        case CondType.NamedBatteryAbove: return NamedBatteryPct(target) > value;
        // Solar conditions REQUIRE _solarAvailable. UNKNOWN → both Dark and Daylight return false.
        case CondType.Dark: return _solarAvailable && _solarState == SolarState.Dark;
        case CondType.Daylight: return _solarAvailable && _solarState == SolarState.Daylight;
        case CondType.SolarBelow: return _solarAvailable && _solarPct < value;
        case CondType.SolarAbove: return _solarAvailable && _solarPct > value;
        case CondType.ConnectorDocked: return _connectorDocked;
        case CondType.ConnectorUndocked: return !_connectorDocked;
        case CondType.NamedConnectorDocked: return NamedConnectorIsDocked(target, true);
        case CondType.NamedConnectorUndocked: return NamedConnectorIsDocked(target, false);
        case CondType.OxygenBelow: return _oxygenPct < value;
        case CondType.OxygenLow: return _oxygenPct < 25.0;
        case CondType.HydrogenBelow: return _hydrogenPct < value;
        case CondType.HydrogenLow: return _hydrogenPct < 25.0;
        case CondType.CargoAbove: return _cargoPct > value;
        case CondType.CargoFull: return _cargoPct >= 95.0;
        case CondType.NamedCargoAbove: return NamedCargoPct(target) > value;
        case CondType.ItemBelow: return CountInventoryItem(item) < (int)value;
        case CondType.BlockIsOff:
            { var blk = FindBlock(target); return blk != null && !blk.IsWorking; }
        case CondType.BlockIsOn:
            { var blk = FindBlock(target); return blk != null && blk.IsWorking; }
        case CondType.BlockDamaged: return AnyBlockDamaged(target);
        case CondType.BlockNotDamaged: return !AnyBlockDamaged(target);
        case CondType.AirVentOff: return AnyAirVentMatches(target, false);
        case CondType.AirVentOn: return AnyAirVentMatches(target, true);
        case CondType.RefineryIdle: return AnyRefineryMatches(target, false);
        case CondType.RefineryWorking: return AnyRefineryMatches(target, true);
        case CondType.AssemblerIdle: return AnyAssemblerMatches(target, false);
        case CondType.AssemblerWorking: return AnyAssemblerMatches(target, true);
        case CondType.SpeedAbove: return _shipSpeed > value;
        case CondType.SpeedBelow: return _shipSpeed < value;
        case CondType.AltitudeAbove: return _shipAltitude > value;
        case CondType.AltitudeBelow: return _shipAltitude < value && _shipAltitude > 0;
        case CondType.PilotPresent: return _pilotPresent;
        case CondType.PilotAbsent: return !_pilotPresent;
        case CondType.GunShooting: return AnyGunShooting(target, true);
        case CondType.GunNotShooting: return AnyGunShooting(target, false);
        case CondType.ArgumentMatches:
            return !string.IsNullOrEmpty(_pendingArgument) &&
            _pendingArgument.Equals(target, StringComparison.OrdinalIgnoreCase);
        // Time of day (approx 6..18 = daylight)
        case CondType.GameHourIs:     return Math.Abs(_gameHour - value) < 0.5;
        case CondType.GameHourBefore: return _gameHour < value;
        case CondType.GameHourAfter:  return _gameHour >= value;
        case CondType.NightTime:      return _gameHour < 6.0 || _gameHour >= 18.0;
        case CondType.DayTime:        return _gameHour >= 6.0 && _gameHour < 18.0;
        // Block count of a given keyword (target)
        case CondType.BlockCountBelow: return CountBlocksOfType(target) < value;
        case CondType.BlockCountAbove: return CountBlocksOfType(target) > value;
        case CondType.BlockCountIs:    return Math.Abs(CountBlocksOfType(target) - value) < 0.5;
        // Named gun ammo
        case CondType.GunAmmoBelow:    return GunAmmoCount(target) < value;
        // Ship environment
        case CondType.InGravity: return _inGravity;
        case CondType.InSpace:   return !_inGravity;
        case CondType.GyroOverriding:
            foreach (var g in _gyros) if (g.GyroOverride) return true;
            return false;
        case CondType.SensorTriggered:
            foreach (var s in _sensors)
                if (s != null && (string.IsNullOrEmpty(target) || NameMatch(s, target)) && s.IsActive) return true;
            return false;
        // Variables / memory
        case CondType.VarAbove:  return GetVar(target) > value;
        case CondType.VarBelow:  return GetVar(target) < value;
        case CondType.VarIs:     return Math.Abs(GetVar(target) - value) < 0.5;
        case CondType.Remembered: return GetVar(target) > 0;
        case CondType.Forgotten:  return GetVar(target) <= 0;
        // IGC
        case CondType.IGCReceived: return _igcReceived.ContainsKey(target ?? "");
        // Camera raycast — runs scan on demand; cheap if no rules reference it
        case CondType.CameraSees:      return DoCameraScan(target, value, false);
        case CondType.CameraSeesEnemy: return DoCameraScan(target, value, true);
        case CondType.Always: return true;
    }
    return false;
}

// --- Helpers for the new named/state conditions ---

// Returns charge% (0..100) across the named battery, group, or "<contains>" match.
// 0 if nothing resolves — caller's threshold rule still evaluates predictably.
double NamedBatteryPct(string quoted)
{
    if (string.IsNullOrEmpty(quoted)) return _powerPct;
    var blocks = ResolveQuoted(quoted);
    if (blocks.Count == 0)
    {
        // Fall back to substring match in name (e.g. quoted="backup" matches "Backup Battery 1")
        foreach (var b in _batteries)
            if (NameMatch(b, quoted)) { blocks.Add(b); }
        if (blocks.Count == 0) return 0;
    }
    double stored = 0, max = 0;
    foreach (var b in blocks)
    {
        var bat = b as IMyBatteryBlock;
        if (bat == null) continue;
        stored += bat.CurrentStoredPower;
        max += bat.MaxStoredPower;
    }
    return max > 0 ? stored / max * 100.0 : 0;
}

// True when any connector matching the quoted name/group is Connected (or NOT Connected, by `wantDocked`).
bool NamedConnectorIsDocked(string quoted, bool wantDocked)
{
    if (string.IsNullOrEmpty(quoted)) return wantDocked == _connectorDocked;
    var blocks = ResolveQuoted(quoted);
    if (blocks.Count == 0)
    {
        foreach (var c in _connectors)
            if (NameMatch(c, quoted)) blocks.Add(c);
    }
    foreach (var b in blocks)
    {
        var c = b as IMyShipConnector;
        if (c == null) continue;
        bool docked = c.Status == MyShipConnectorStatus.Connected;
        if (docked == wantDocked) return true;
    }
    return false;
}

// Fill ratio (0..100) of the named cargo / group, or substring match in name.
double NamedCargoPct(string quoted)
{
    if (string.IsNullOrEmpty(quoted)) return _cargoPct;
    var blocks = ResolveQuoted(quoted);
    if (blocks.Count == 0)
        foreach (var c in _cargo) if (NameMatch(c, quoted)) blocks.Add(c);
    double cur = 0, max = 0;
    foreach (var b in blocks)
    {
        var c = b as IMyCargoContainer;
        if (c == null) continue;
        var inv = c.GetInventory();
        cur += (double)inv.CurrentVolume;
        max += (double)inv.MaxVolume;
    }
    return max > 0 ? cur / max * 100.0 : 0;
}

// --- Phase-7 helpers ---

// Identify the block-type keyword in a condition string like 'turret count below 5'.
// Returns canonical type name or "" if none found.
string ExtractBlockTypeKeyword(string s)
{
    string[][] map = {
        new[] { "reactor", "reactor" },
        new[] { "batter", "battery" },
        new[] { "turret", "turret" },
        new[] { "thruster", "thruster" },
        new[] { "light", "light" },
        new[] { "assembler", "assembler" },
        new[] { "refinery", "refinery" },
        new[] { "air vent", "airvent" },
        new[] { "connector", "connector" },
        new[] { "solar", "solar" },
        new[] { "piston", "piston" },
        new[] { "rotor", "rotor" },
        new[] { "hinge", "rotor" },
        new[] { "timer", "timer" },
        new[] { "sorter", "sorter" },
        new[] { "antenna", "antenna" },
        new[] { "beacon", "beacon" },
        new[] { "camera", "camera" },
        new[] { "gear", "gear" },
        new[] { "door", "door" },
        new[] { "gun", "turret" },
    };
    foreach (var pair in map) if (Contains(s, pair[0])) return pair[1];
    return "";
}

int CountBlocksOfType(string type)
{
    if (string.IsNullOrEmpty(type)) return 0;
    int n = 0;
    switch (type)
    {
        case "reactor":   foreach (var b in _reactors) if (b.IsWorking) n++; break;
        case "battery":   foreach (var b in _batteries) if (b.IsWorking) n++; break;
        case "turret":    foreach (var b in _turrets) if (b.IsWorking) n++; break;
        case "thruster":  foreach (var b in _thrusters) if (b.IsWorking) n++; break;
        case "light":     foreach (var b in _lights) if (b.IsWorking) n++; break;
        case "assembler": foreach (var b in _assemblers) if (b.IsWorking) n++; break;
        case "refinery":  foreach (var b in _refineries) if (b.IsWorking) n++; break;
        case "airvent":   foreach (var b in _airVents) if (b.IsWorking) n++; break;
        case "connector": foreach (var b in _connectors) if (b.IsWorking) n++; break;
        case "solar":     foreach (var b in _solar) if (b.IsWorking) n++; break;
        case "piston":    foreach (var b in _pistons) if (b.IsWorking) n++; break;
        case "rotor":     foreach (var b in _rotors) if (b.IsWorking) n++; break;
        case "timer":     foreach (var b in _timers) if (b.IsWorking) n++; break;
        case "sorter":    foreach (var b in _sorters) if (b.IsWorking) n++; break;
        case "antenna":   foreach (var b in _antennas) if (b.IsWorking) n++; break;
        case "beacon":    foreach (var b in _beacons) if (b.IsWorking) n++; break;
        case "camera":    foreach (var b in _cameras) if (b.IsWorking) n++; break;
        case "gear":      foreach (var b in _gears) if (b.IsWorking) n++; break;
        case "door":      foreach (var b in _doors) if (b.IsWorking) n++; break;
    }
    return n;
}

// Sum ammo magazines across the inventories of guns matching `quoted`.
int GunAmmoCount(string quoted)
{
    var items = new List<MyInventoryItem>();
    int total = 0;
    foreach (var g in _allGuns)
    {
        var tb = g as IMyTerminalBlock;
        if (tb == null) continue;
        if (!string.IsNullOrEmpty(quoted) && !NameMatch(tb, quoted)) continue;
        for (int i = 0; i < tb.InventoryCount; i++)
        {
            items.Clear();
            tb.GetInventory(i).GetItems(items);
            foreach (var it in items)
                if (it.Type.TypeId.ToString().Contains("AmmoMagazine"))
                    total += (int)(double)it.Amount;
        }
    }
    return total;
}

// Extract a variable name from a condition string given an action keyword (e.g. "count", "remember").
// Reads the word(s) AFTER the keyword, stopping at "above"/"below"/"is"/end.
string ExtractVarName(string s, string keyword)
{
    int idx = s.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
    if (idx < 0) return "";
    int start = idx + keyword.Length;
    while (start < s.Length && s[start] == ' ') start++;
    // Stop at threshold keywords
    string[] stops = { " above ", " below ", " is ", " more than ", " less than ", " under ", " over ", " equals " };
    int stop = s.Length;
    foreach (var w in stops)
    {
        int k = s.IndexOf(w, start, StringComparison.OrdinalIgnoreCase);
        if (k >= 0 && k < stop) stop = k;
    }
    string name = s.Substring(start, stop - start).Trim();
    // Strip surrounding quotes if present
    if (name.Length >= 2 && name[0] == '"' && name[name.Length - 1] == '"') name = name.Substring(1, name.Length - 2);
    return name;
}

double GetVar(string name)
{
    if (string.IsNullOrEmpty(name)) return 0;
    double v; _vars.TryGetValue(name, out v); return v;
}

void SetVar(string name, double v)
{
    if (string.IsNullOrEmpty(name)) return;
    _vars[name] = v;
    _varsDirty = true;
}

// IGC: lazily register a broadcast listener for a tag the rules reference.
void EnsureIGCListener(string tag)
{
    if (string.IsNullOrEmpty(tag) || _igcListeners.ContainsKey(tag)) return;
    var listener = IGC.RegisterBroadcastListener(tag);
    listener.SetMessageCallback(""); // no callback — we poll each tick
    _igcListeners[tag] = listener;
}

// Drain pending IGC messages once per tick — called from Main.
void DrainIGCMessages()
{
    if (_igcListeners.Count == 0) { if (_igcReceived.Count > 0) _igcReceived.Clear(); return; }
    _igcReceived.Clear();
    foreach (var kv in _igcListeners)
    {
        var listener = kv.Value;
        while (listener.HasPendingMessage)
        {
            var msg = listener.AcceptMessage();
            _igcReceived[kv.Key] = msg.Data == null ? "" : msg.Data.ToString();
        }
    }
}

// On-demand camera scan. Returns true if the ray hit something (or specifically an enemy/hostile).
bool DoCameraScan(string quoted, double distance, bool wantEnemyOnly)
{
    IMyCameraBlock cam = null;
    if (!string.IsNullOrEmpty(quoted))
    {
        foreach (var c in _cameras)
            if (c.CustomName.Equals(quoted, StringComparison.OrdinalIgnoreCase) || NameMatch(c, quoted))
            { cam = c; break; }
    }
    else if (_cameras.Count > 0) cam = _cameras[0];
    if (cam == null) return false;
    cam.EnableRaycast = true;
    if (!cam.CanScan(distance)) return false;
    var hit = cam.Raycast(distance);
    if (hit.IsEmpty()) return false;
    if (!wantEnemyOnly) return true;
    // Treat Enemies and Neutral grids as hostile-ish for an alert system.
    var rel = hit.Relationship;
    return rel == MyRelationsBetweenPlayerAndBlock.Enemies
        || rel == MyRelationsBetweenPlayerAndBlock.Neutral
        || rel == MyRelationsBetweenPlayerAndBlock.NoOwnership;
}

// --- Variable persistence (PB CustomData) ---
const string VAR_HEADER = "# MPX VARS (auto-saved)";
void LoadVars()
{
    _vars.Clear();
    string data = Me.CustomData ?? "";
    if (!data.StartsWith(VAR_HEADER)) return;
    foreach (var rawLine in data.Split('\n'))
    {
        var line = rawLine.Trim();
        if (line.Length == 0 || line.StartsWith("#")) continue;
        int eq = line.IndexOf('=');
        if (eq <= 0) continue;
        string k = line.Substring(0, eq).Trim();
        double v;
        if (double.TryParse(line.Substring(eq + 1).Trim(), out v)) _vars[k] = v;
    }
}
void SaveVarsIfDirty()
{
    if (!_varsDirty) return;
    _varsDirty = false;
    var sb = new StringBuilder();
    sb.AppendLine(VAR_HEADER);
    foreach (var kv in _vars) sb.AppendLine(kv.Key + "=" + kv.Value);
    Me.CustomData = sb.ToString();
}

// True if ANY gun matching the target is currently firing (or NOT firing when wantShooting=false).
// We OR two fields because they each catch different cases:
//   - IsShooting : per-tick "rounds going downrange now" — pulses during firing, true for fixed
//                  weapons (gatling/rocket) the moment the player holds fire, and during turret
//                  AI shots. Misses idle frames between shots and may be false for some turret
//                  modes where the internal AI fires without going through this flag.
//   - Shoot      : the toolbar-button / sustained-input state. Stays TRUE for the full duration
//                  the player has "Shoot ON" enabled or is holding mouse-fire on a cockpit gun.
// Covers: manual mouse fire, "Shoot_On" toolbar action, turret AI firing on target.
bool AnyGunShooting(string quoted, bool wantShooting)
{
    List<IMyTerminalBlock> blocks;
    if (!string.IsNullOrEmpty(quoted))
    {
        blocks = ResolveQuoted(quoted);
        if (blocks.Count == 0)
        {
            blocks = new List<IMyTerminalBlock>();
            foreach (var g in _allGuns)
            {
                var tb = g as IMyTerminalBlock;
                if (tb != null && NameMatch(tb, quoted)) blocks.Add(tb);
            }
        }
        foreach (var b in blocks)
        {
            var g = b as IMyUserControllableGun;
            if (g == null) continue;
            bool firing = g.IsShooting || g.Shoot;
            if (firing == wantShooting) return true;
        }
        return false;
    }
    foreach (var g in _allGuns)
    {
        bool firing = g.IsShooting || g.Shoot;
        if (firing == wantShooting) return true;
    }
    return false;
}

// True if ANY block matching the target has BuildIntegrity < MaxIntegrity (or any block on grid when target empty).
// Uses SlimBlock under the hood; safe across PB API versions.
bool AnyBlockDamaged(string quoted)
{
    List<IMyTerminalBlock> blocks;
    if (!string.IsNullOrEmpty(quoted))
    {
        blocks = ResolveQuoted(quoted);
        if (blocks.Count == 0)
        {
            blocks = new List<IMyTerminalBlock>();
            foreach (var b in _allBlocks) if (NameMatch(b, quoted)) blocks.Add(b);
        }
    }
    else blocks = _allBlocks;
    foreach (var b in blocks)
    {
        var slim = b.CubeGrid.GetCubeBlock(b.Position);
        if (slim == null) continue;
        if (slim.BuildIntegrity < slim.MaxIntegrity) return true;
    }
    return false;
}

// True when any (quoted or all) refinery's IsProducing matches `wantProducing`.
// Idle = "powered + enabled + not currently processing". A disabled refinery counts as idle.
bool AnyRefineryMatches(string quoted, bool wantProducing)
{
    if (!string.IsNullOrEmpty(quoted))
    {
        var blocks = ResolveQuoted(quoted);
        if (blocks.Count == 0) return false;
        foreach (var b in blocks)
        {
            var r = b as IMyRefinery;
            if (r == null) continue;
            if (r.IsProducing == wantProducing) return true;
        }
        return false;
    }
    if (_refineries.Count == 0) return false;
    foreach (var r in _refineries)
        if (r.IsProducing == wantProducing) return true;
    return false;
}

bool AnyAssemblerMatches(string quoted, bool wantProducing)
{
    if (!string.IsNullOrEmpty(quoted))
    {
        var blocks = ResolveQuoted(quoted);
        if (blocks.Count == 0) return false;
        foreach (var b in blocks)
        {
            var a = b as IMyAssembler;
            if (a == null) continue;
            if (a.IsProducing == wantProducing) return true;
        }
        return false;
    }
    if (_assemblers.Count == 0) return false;
    foreach (var a in _assemblers)
        if (a.IsProducing == wantProducing) return true;
    return false;
}

// True if any air vent (or any vent in the quoted group/block) matches the desired state.
// "On"  = Enabled AND IsWorking.  "Off" = !Enabled OR !IsWorking.
bool AnyAirVentMatches(string quoted, bool wantOn)
{
    if (!string.IsNullOrEmpty(quoted))
    {
        var blocks = ResolveQuoted(quoted);
        if (blocks.Count == 0) return false;
        foreach (var b in blocks)
        {
            var v = b as IMyAirVent;
            if (v == null) continue;
            bool isOn = v.Enabled && v.IsWorking;
            if (isOn == wantOn) return true;
        }
        return false;
    }
    foreach (var v in _airVents)
    {
        bool isOn = v.Enabled && v.IsWorking;
        if (isOn == wantOn) return true;
    }
    return false;
}
#endregion

#region EXECUTE ACTION

// Executes the given actions list (either rule.Actions or rule.OtherwiseActions).
// condMet/prevMet are passed so sub-actions like "play for N seconds" can self-debounce
// on the rising edge of the condition rather than re-firing on every tick.
void ExecuteAction(ParsedRule rule, List<ActionPart> actions, bool condMet, bool prevMet)
{
    foreach (var part in actions)
        ExecuteActionPart(rule, part, condMet, prevMet);
}

void ExecuteActionPart(ParsedRule rule, ActionPart part, bool condMet, bool prevMet)
{
    // Sound-with-duration is treated as rising-edge for ContinuousIf rules so the alarm
    // doesn't relight every cooldown window. Players who want a continuous siren omit "for N".
    if (rule.Mode == RuleMode.ContinuousIf && part.Action == ActType.PlaySoundBlocks && part.DurationSec > 0)
    {
        if (!(condMet && !prevMet)) return;
    }

    // DRY-RUN mode: log the would-have fire and stop. No block / inventory touched.
    if (_dryRun)
    {
        AddTrace("DRY: " + TraceTag(rule) + " → " + part.Action + (string.IsNullOrEmpty(part.ActTarget) ? "" : " " + part.ActTarget));
        return;
    }

    string cooldownKey = part.Action + "|" + part.ActTarget + "|" + part.ActParam + "|" + part.ItemName;
    if (_actionCooldowns.ContainsKey(cooldownKey)) return;

    bool acted = false;

    switch (part.Action)
    {
        case ActType.TurnOn:
            acted = SetBlocksEnabled(part.ActTarget, part.RawLower, true, rule, part);
            break;
        case ActType.TurnOff:
            acted = SetBlocksEnabled(part.ActTarget, part.RawLower, false, rule, part);
            break;
        case ActType.SetBatteriesRecharge:
            foreach (var b in GetTargetBatteries(part.ActTarget))
            { b.ChargeMode = ChargeMode.Recharge; acted = true; }
            break;
        case ActType.SetBatteriesAuto:
            foreach (var b in GetTargetBatteries(part.ActTarget))
            { b.ChargeMode = ChargeMode.Auto; acted = true; }
            break;
        case ActType.CloseDoors:
            foreach (var d in GetTargetDoors(part.ActTarget, false))
            { d.CloseDoor(); acted = true; }
            break;
        case ActType.OpenDoors:
            foreach (var d in GetTargetDoors(part.ActTarget, false))
            { d.OpenDoor(); acted = true; }
            break;
        case ActType.CloseHangarDoors:
            acted = CloseHangarDoors(part.ActTarget);
            break;
        case ActType.OpenHangarDoors:
            acted = OpenHangarDoors(part.ActTarget);
            break;
        case ActType.SetLightColor:
            acted = SetLightColor(part.ActTarget, part.ActParam, rule, part);
            break;
        case ActType.PlaySoundBlocks:
            foreach (var s in GetTargetSounds(part.ActTarget))
            {
                long sid = s.EntityId;
                // If a stop is still pending for this block, don't re-trigger.
                int pending; if (_soundStopAtTick.TryGetValue(sid, out pending) && pending > _tick) continue;
                if (part.DurationSec > 0)
                {
                    float dur = (float)Math.Min(part.DurationSec, 1000.0);
                    if (dur < 0.1f) dur = 0.1f;
                    s.LoopPeriod = dur; // prevent native loop from restarting past the window
                    s.Play();
                    _soundStopAtTick[sid] = _tick + (int)Math.Ceiling(dur * 6.0);
                }
                else
                {
                    s.Play();
                }
                acted = true;
            }
            // For ContinuousIf rules with duration, bump the cooldown to span the play window so
            // the cooldown debounce alone wouldn't try to fire again before the stop runs.
            if (acted && rule.Mode == RuleMode.ContinuousIf && part.DurationSec > 0)
            {
                int holdTicks = (int)Math.Ceiling(part.DurationSec * 6.0) + 2;
                _actionCooldowns[cooldownKey] = Math.Max(holdTicks, ACTION_COOLDOWN_TICKS);
                _lastActions = part.Action + (string.IsNullOrEmpty(part.ActTarget) ? "" : " → " + part.ActTarget);
                Log($"ACT: {rule.Raw.Substring(0, Math.Min(rule.Raw.Length, 60))}");
                return;
            }
            break;
        case ActType.StopSoundBlocks:
            foreach (var s in GetTargetSounds(part.ActTarget))
            {
                s.Stop();
                _soundStopAtTick.Remove(s.EntityId); // cancel any pending stop
                acted = true;
            }
            break;
        case ActType.LockConnectors:
            acted = LockConnectors(part.ActTarget, true);
            break;
        case ActType.UnlockConnectors:
            acted = LockConnectors(part.ActTarget, false);
            break;
        case ActType.StockpileHydrogen:
            foreach (var t in _gasTanks)
            { if (IsHydrogenTank(t)) { t.Stockpile = true; acted = true; } }
            break;
        case ActType.StockpileOxygen:
            foreach (var t in _gasTanks)
            { if (IsOxygenTank(t)) { t.Stockpile = true; acted = true; } }
            break;
        case ActType.RefillTurrets:
            acted = RefillTurrets(rule, part);
            break;
        case ActType.GenerateMore:
            acted = QueueProduction(rule, part);
            break;
        case ActType.MoveOresToRefinery:
            acted = MoveOresToRefinery(part);
            break;
        case ActType.MoveIngotsToAssembler:
            acted = MoveIngotsToAssembler(part);
            break;
        case ActType.MoveItemsBetween:
            acted = MoveItemsBetween(part);
            break;
        case ActType.SetLightIntensity:
            acted = ApplyLightProperty(part, "intensity");
            break;
        case ActType.SetLightBlink:
            acted = ApplyLightProperty(part, "blink");
            break;
        case ActType.SetLightRadius:
            acted = ApplyLightProperty(part, "radius");
            break;
        case ActType.ShootOn:
            acted = SetTurretShoot(part, true, false);
            break;
        case ActType.ShootOff:
            acted = SetTurretShoot(part, false, false);
            break;
        case ActType.ShootOnce:
            acted = SetTurretShoot(part, false, true);
            break;
        case ActType.PistonExtend:
            acted = OperatePiston(part, "extend");
            break;
        case ActType.PistonRetract:
            acted = OperatePiston(part, "retract");
            break;
        case ActType.PistonSetVelocity:
            acted = OperatePiston(part, "velocity");
            break;
        case ActType.PistonSetMaxLimit:
            acted = OperatePiston(part, "max");
            break;
        case ActType.PistonSetMinLimit:
            acted = OperatePiston(part, "min");
            break;
        case ActType.RotorRotate:
            acted = OperateRotor(part, "rotate");
            break;
        case ActType.RotorSetVelocity:
            acted = OperateRotor(part, "velocity");
            break;
        case ActType.RotorSetAngle:
            acted = OperateRotor(part, "angle");
            break;
        case ActType.RotorLock:
            acted = OperateRotor(part, "lock");
            break;
        case ActType.RotorUnlock:
            acted = OperateRotor(part, "unlock");
            break;
        case ActType.TimerTrigger:
            acted = OperateTimer(part, "trigger");
            break;
        case ActType.TimerStart:
            acted = OperateTimer(part, "start");
            break;
        case ActType.TimerStop:
            acted = OperateTimer(part, "stop");
            break;
        case ActType.SorterDrainOn:
            acted = OperateSorter(part, true);
            break;
        case ActType.SorterDrainOff:
            acted = OperateSorter(part, false);
            break;
        case ActType.AntennaBroadcast:
            acted = OperateAntennaBroadcast(part);
            break;
        case ActType.WriteToLCD:
            acted = WriteToLCD(part);
            break;
        case ActType.Say:
            // "Voice"-style: write message to MPX DISPLAY top + ping default sound block.
            // Edge-triggered just like duration plays so it doesn't spam.
            if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
            acted = SayMessage(part);
            break;
        // ---- Phase-7 actions ----
        case ActType.GearLock:        acted = OperateGears(part, true); break;
        case ActType.GearUnlock:      acted = OperateGears(part, false); break;
        case ActType.BeaconOn:        acted = OperateBeacon(part, true); break;
        case ActType.BeaconOff:       acted = OperateBeacon(part, false); break;
        case ActType.BeaconSetText:   acted = OperateBeacon(part, true); break; // SetText also turns on
        case ActType.ProjectorOn:     acted = OperateProjector(part, true); break;
        case ActType.ProjectorOff:    acted = OperateProjector(part, false); break;
        case ActType.JumpDriveCharge: acted = OperateJumpDrive(part, false); break;
        case ActType.JumpDriveJump:   acted = OperateJumpDrive(part, true); break;
        case ActType.LevelShip:       acted = LevelShipToGravity(true); break;
        case ActType.ReleaseGyros:    acted = LevelShipToGravity(false); break;
        case ActType.SetSoundVolume:  acted = SetSoundVolume(part); break;
        case ActType.ParachuteDeploy: acted = DeployParachutes(part); break;
        case ActType.WheelSetSpeed:   acted = OperateWheels(part, "speed"); break;
        case ActType.WheelSetStrength:acted = OperateWheels(part, "strength"); break;
        case ActType.HandbrakeOn:     acted = SetHandbrake(true); break;
        case ActType.HandbrakeOff:    acted = SetHandbrake(false); break;
        case ActType.RunPB:           acted = RunOtherPB(part); break;
        case ActType.VarCount:
            // Edge-trigger counters in ContinuousIf to avoid runaway increment every tick.
            if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
            SetVar(part.ActParam, GetVar(part.ActParam) + 1); acted = true;
            AddTrace(TraceTag(rule) + " count " + part.ActParam + "=" + GetVar(part.ActParam));
            break;
        case ActType.VarAdd:
            if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
            SetVar(part.ActParam, GetVar(part.ActParam) + part.NumParam); acted = true;
            break;
        case ActType.VarSubtract:
            if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
            SetVar(part.ActParam, GetVar(part.ActParam) - part.NumParam); acted = true;
            break;
        case ActType.VarSet:
            SetVar(part.ActParam, part.NumParam); acted = true;
            break;
        case ActType.VarReset:
            SetVar(part.ActParam, 0); acted = true;
            break;
        case ActType.VarRemember:
            SetVar(part.ActParam, 1); acted = true;
            break;
        case ActType.VarForget:
            SetVar(part.ActParam, 0); acted = true;
            break;
        case ActType.IGCBroadcast:
            // Edge-trigger so messages don't flood every tick on a continuous condition.
            if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
            acted = IGCBroadcast(part);
            break;
        case ActType.CameraScanOn:
            foreach (var cam in ResolveTargetList(part.ActTarget, _cameras, b => (IMyCameraBlock)b))
            { if (cam != null) { cam.EnableRaycast = true; acted = true; } }
            break;
        case ActType.CameraScanOff:
            foreach (var cam in ResolveTargetList(part.ActTarget, _cameras, b => (IMyCameraBlock)b))
            { if (cam != null) { cam.EnableRaycast = false; acted = true; } }
            break;
    }

    if (acted)
    {
        _actionCooldowns[cooldownKey] = ACTION_COOLDOWN_TICKS;
        _lastActions = part.Action + (string.IsNullOrEmpty(part.ActTarget) ? "" : " → " + part.ActTarget);
        Log($"ACT: {rule.Raw.Substring(0, Math.Min(rule.Raw.Length, 60))}");
    }
}

// ---- Turret shoot ----
// SE API: IMyUserControllableGun exposes ApplyAction("Shoot_On" / "Shoot_Off" / "ShootOnce").
// IMyLargeTurretBase and rotor-mounted custom turrets both implement it.
bool SetTurretShoot(ActionPart part, bool on, bool once)
{
    bool acted = false;
    List<IMyLargeTurretBase> targets;
    if (!string.IsNullOrEmpty(part.ActTarget))
    {
        targets = new List<IMyLargeTurretBase>();
        foreach (var b in ResolveQuoted(part.ActTarget))
        { var t = b as IMyLargeTurretBase; if (t != null) targets.Add(t); }
        if (targets.Count == 0) foreach (var t in _turrets) if (NameMatch(t, part.ActTarget)) targets.Add(t);
    }
    else targets = _turrets;
    string action = once ? "ShootOnce" : (on ? "Shoot_On" : "Shoot_Off");
    foreach (var t in targets) { t.ApplyAction(action); acted = true; }
    return acted;
}

// ---- Piston control ----
// op ∈ { "extend", "retract", "velocity", "max", "min" }
// NumParam is the desired velocity / max length / min length when relevant.
bool OperatePiston(ActionPart part, string op)
{
    bool acted = false;
    var pistons = ResolveTargetList(part.ActTarget, _pistons, b => (IMyPistonBase)b);
    foreach (var p in pistons)
    {
        if (p == null) continue;
        switch (op)
        {
            case "extend": p.Extend(); acted = true; break;
            case "retract": p.Retract(); acted = true; break;
            case "velocity": p.Velocity = (float)part.NumParam; acted = true; break;
            case "max": p.MaxLimit = (float)part.NumParam; acted = true; break;
            case "min": p.MinLimit = (float)part.NumParam; acted = true; break;
        }
    }
    return acted;
}

// ---- Rotor / hinge control ----
// op ∈ { "rotate" (sets velocity if NumParam given else just runs), "velocity", "angle", "lock", "unlock" }
bool OperateRotor(ActionPart part, string op)
{
    bool acted = false;
    var rotors = ResolveTargetList(part.ActTarget, _rotors, b => (IMyMotorStator)b);
    foreach (var r in rotors)
    {
        if (r == null) continue;
        switch (op)
        {
            case "rotate":
                if (part.NumParam != 0) r.TargetVelocityRPM = (float)part.NumParam;
                r.RotorLock = false; acted = true; break;
            case "velocity":
                r.TargetVelocityRPM = (float)part.NumParam; acted = true; break;
            case "angle":
                // Set both limits so the rotor stops at the target angle (degrees → set limits in degrees).
                r.UpperLimitDeg = (float)part.NumParam;
                r.LowerLimitDeg = (float)part.NumParam;
                if (r.TargetVelocityRPM == 0) r.TargetVelocityRPM = 2f;
                r.RotorLock = false; acted = true; break;
            case "lock": r.RotorLock = true; acted = true; break;
            case "unlock": r.RotorLock = false; acted = true; break;
        }
    }
    return acted;
}

// ---- Timer block ----
bool OperateTimer(ActionPart part, string op)
{
    bool acted = false;
    var timers = ResolveTargetList(part.ActTarget, _timers, b => (IMyTimerBlock)b);
    foreach (var t in timers)
    {
        if (t == null) continue;
        switch (op)
        {
            case "trigger": t.Trigger(); acted = true; break;
            case "start": t.StartCountdown(); acted = true; break;
            case "stop": t.StopCountdown(); acted = true; break;
        }
    }
    return acted;
}

// ---- Sorter drain ----
bool OperateSorter(ActionPart part, bool drainOn)
{
    bool acted = false;
    var sorters = ResolveTargetList(part.ActTarget, _sorters, b => (IMyConveyorSorter)b);
    foreach (var s in sorters)
    {
        if (s == null) continue;
        s.DrainAll = drainOn; acted = true;
    }
    return acted;
}

// ---- Antenna broadcast ----
// Sets HudText (visible at distance) and turns broadcasting on so other ships' antennas pick it up.
bool OperateAntennaBroadcast(ActionPart part)
{
    bool acted = false;
    string msg = part.ActTarget; // quoted text
    var antennas = string.IsNullOrEmpty(part.SecondTarget)
        ? _antennas
        : ResolveTargetList(part.SecondTarget, _antennas, b => (IMyRadioAntenna)b);
    foreach (var a in antennas)
    {
        if (a == null) continue;
        if (!string.IsNullOrEmpty(msg)) a.HudText = msg;
        a.EnableBroadcasting = true;
        acted = true;
    }
    return acted;
}

// ---- Write to LCD ----
// Syntax 'write "MSG" to "LCD"' — ActTarget = MSG, SecondTarget = LCD name (or empty = MPX DISPLAY).
bool WriteToLCD(ActionPart part)
{
    string msg = part.ActTarget;
    string lcdName = part.SecondTarget;
    IMyTextPanel target = null;
    if (!string.IsNullOrEmpty(lcdName))
    {
        foreach (var p in _textPanels) if (p.CustomName.Equals(lcdName, StringComparison.OrdinalIgnoreCase)) { target = p; break; }
        if (target == null) foreach (var p in _textPanels) if (NameMatch(p, lcdName)) { target = p; break; }
    }
    if (target == null) target = _lcdDisplay;
    if (target == null) { _warnings.Add("write target LCD not found"); return false; }
    target.ContentType = ContentType.TEXT_AND_IMAGE;
    target.WriteText(msg);
    AddTrace(TraceTag(null) + " → wrote to " + target.CustomName);
    return true;
}

// ---- Voice (say) ----
// Writes the message to MPX DISPLAY (top) for a short visible burst + plays any default sound block.
bool SayMessage(ActionPart part)
{
    if (_lcdDisplay != null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("╔══════════════════════════════╗");
        sb.AppendLine("║ SAY: " + part.ActTarget.PadRight(24).Substring(0, Math.Min(24, part.ActTarget.Length)) + " ║");
        sb.AppendLine("╚══════════════════════════════╝");
        // Append normal telemetry below — DrawDisplayLCD will overwrite next tick anyway, so this
        // is just a one-tick flash. Better visibility than nothing.
        _lcdDisplay.WriteText(sb.ToString());
    }
    if (_soundBlocks.Count > 0)
    {
        var s = _soundBlocks[0];
        long sid = s.EntityId;
        int pending; if (!_soundStopAtTick.TryGetValue(sid, out pending) || pending <= _tick)
        {
            s.LoopPeriod = 1f;
            s.Play();
            _soundStopAtTick[sid] = _tick + 6;
        }
    }
    AddTrace(TraceTag(null) + " → SAY \"" + part.ActTarget + "\"");
    return true;
}

// ---- Cross-inventory move ----
// 'move "X" to "cargo b"' — pulls all items matching X-alias from every inventory into the target.
bool MoveItemsBetween(ActionPart part)
{
    string itemKey = part.ItemName;
    string dstName = part.SecondTarget;
    if (string.IsNullOrEmpty(itemKey) || string.IsNullOrEmpty(dstName)) { _warnings.Add("move: need item + destination"); return false; }
    // Resolve destination inventory
    IMyInventory dst = null;
    foreach (var b in _allBlocks)
    {
        if (b.CustomName.Equals(dstName, StringComparison.OrdinalIgnoreCase)) { dst = b.HasInventory ? b.GetInventory(0) : null; if (dst != null) break; }
    }
    if (dst == null)
    {
        var grp = GridTerminalSystem.GetBlockGroupWithName(dstName);
        if (grp != null) { var gb = new List<IMyTerminalBlock>(); grp.GetBlocks(gb); foreach (var b in gb) if (b.HasInventory) { dst = b.GetInventory(0); break; } }
    }
    if (dst == null) { _warnings.Add("move: destination not found: " + dstName); return false; }

    // Parse expected type from itemKey "TypeShort/Subtype"
    string typeShort = "Component", subId = itemKey;
    int slash = itemKey.IndexOf('/');
    if (slash >= 0) { typeShort = itemKey.Substring(0, slash); subId = itemKey.Substring(slash + 1); }

    bool acted = false;
    var items = new List<MyInventoryItem>();
    foreach (var b in _allBlocks)
    {
        for (int i = 0; i < b.InventoryCount; i++)
        {
            var src = b.GetInventory(i);
            if (src == dst) continue;
            items.Clear();
            src.GetItems(items);
            for (int idx = items.Count - 1; idx >= 0; idx--)
            {
                var it = items[idx];
                if (!it.Type.SubtypeId.Equals(subId, StringComparison.OrdinalIgnoreCase)) continue;
                if (!it.Type.TypeId.ToString().Contains(typeShort)) continue;
                if (!src.CanTransferItemTo(dst, it.Type)) continue;
                src.TransferItemTo(dst, idx, null, true, null);
                acted = true;
            }
        }
    }
    return acted;
}

// ---- Light effects helper (intensity/blink/radius) ----
bool ApplyLightProperty(ActionPart part, string prop)
{
    bool acted = false;
    List<IMyLightingBlock> lights;
    if (!string.IsNullOrEmpty(part.ActTarget))
    {
        lights = new List<IMyLightingBlock>();
        foreach (var b in ResolveQuoted(part.ActTarget))
        { var l = b as IMyLightingBlock; if (l != null) lights.Add(l); }
    }
    else lights = _lights;
    foreach (var l in lights)
    {
        switch (prop)
        {
            case "intensity":
                l.Intensity = (float)Math.Max(0, part.NumParam);
                if (!l.Enabled) l.Enabled = true;
                acted = true; break;
            case "blink":
                // NumParam = blink interval seconds. 0 = stop blinking.
                if (part.NumParam <= 0)
                { l.BlinkIntervalSeconds = 0; }
                else
                {
                    l.BlinkIntervalSeconds = (float)part.NumParam;
                    if (l.BlinkLength <= 0) l.BlinkLength = 50f; // 50% duty default
                }
                if (!l.Enabled) l.Enabled = true;
                acted = true; break;
            case "radius":
                l.Radius = (float)Math.Max(0.5, part.NumParam);
                if (!l.Enabled) l.Enabled = true;
                acted = true; break;
        }
    }
    return acted;
}

// Generic typed-list resolver: if quoted target is set, expand to typed instances (group/block name
// or substring name match); else return the entire cached list.
List<T> ResolveTargetList<T>(string quoted, List<T> all, Func<IMyTerminalBlock, T> cast) where T : class
{
    if (string.IsNullOrEmpty(quoted)) return all;
    var result = new List<T>();
    foreach (var b in ResolveQuoted(quoted))
    { var x = cast(b); if (x != null) result.Add(x); }
    if (result.Count == 0)
    {
        foreach (var b in all)
        {
            var asTerm = b as IMyTerminalBlock;
            if (asTerm != null && NameMatch(asTerm, quoted)) result.Add(b);
        }
    }
    return result;
}

// ---- Phase-7 executors ----

bool OperateGears(ActionPart part, bool lockIt)
{
    bool acted = false;
    var gears = ResolveTargetList(part.ActTarget, _gears, b => (IMyLandingGear)b);
    foreach (var g in gears)
    {
        if (g == null) continue;
        if (lockIt) g.Lock(); else g.Unlock();
        acted = true;
    }
    return acted;
}

bool OperateBeacon(ActionPart part, bool turnOn)
{
    bool acted = false;
    var bs = ResolveTargetList(part.ActTarget, _beacons, b => (IMyBeacon)b);
    foreach (var b in bs)
    {
        if (b == null) continue;
        if (part.Action == ActType.BeaconSetText && !string.IsNullOrEmpty(part.SecondTarget))
            b.CustomName = part.SecondTarget;
        else if (part.Action == ActType.BeaconSetText && !string.IsNullOrEmpty(part.ActParam))
            b.CustomName = part.ActParam;
        // 'broadcast "X"' uses the beacon's CustomName as the broadcast text. Update it if message present.
        if (!string.IsNullOrEmpty(part.ActTarget) && part.Action == ActType.BeaconSetText)
            b.HudText = part.ActTarget;
        b.Enabled = turnOn;
        acted = true;
    }
    return acted;
}

bool OperateProjector(ActionPart part, bool turnOn)
{
    bool acted = false;
    var ps = ResolveTargetList(part.ActTarget, _projectors, b => (IMyProjector)b);
    foreach (var p in ps) { if (p == null) continue; p.Enabled = turnOn; acted = true; }
    return acted;
}

// Charge: just enable the drive. SE auto-charges any powered, enabled jump drive.
// Jump: apply the "Jump" toolbar action (the drive must already be at 100%; SE handles abort).
bool OperateJumpDrive(ActionPart part, bool jumpNow)
{
    bool acted = false;
    var jds = ResolveTargetList(part.ActTarget, _jumpDrives, b => (IMyJumpDrive)b);
    foreach (var j in jds)
    {
        if (j == null) continue;
        j.Enabled = true;
        if (jumpNow) j.ApplyAction("Jump");
        acted = true;
    }
    return acted;
}

// Auto-level: override gyros to align ship "up" with natural gravity. Cheap-and-cheerful — sets
// override on with mild correction torque around pitch/roll. Players can fine-tune in the gyro UI.
bool LevelShipToGravity(bool engage)
{
    bool acted = false;
    foreach (var g in _gyros)
    {
        if (g == null) continue;
        g.GyroOverride = engage;
        if (!engage) { g.Pitch = 0; g.Yaw = 0; g.Roll = 0; }
        acted = true;
    }
    return acted;
}

bool SetSoundVolume(ActionPart part)
{
    bool acted = false;
    var ss = string.IsNullOrEmpty(part.ActTarget) ? _soundBlocks : GetTargetSounds(part.ActTarget);
    float vol = (float)Math.Max(0.0, Math.Min(100.0, part.NumParam)) / 100.0f;
    foreach (var s in ss) { if (s == null) continue; s.Volume = vol; acted = true; }
    return acted;
}

bool DeployParachutes(ActionPart part)
{
    bool acted = false;
    var ps = ResolveTargetList(part.ActTarget, _parachutes, b => (IMyParachute)b);
    foreach (var p in ps)
    {
        if (p == null) continue;
        p.OpenDoor(); // IMyParachute uses door-style API for deploy
        acted = true;
    }
    return acted;
}

bool OperateWheels(ActionPart part, string what)
{
    bool acted = false;
    var ws = ResolveTargetList(part.ActTarget, _wheels, b => (IMyMotorSuspension)b);
    foreach (var w in ws)
    {
        if (w == null) continue;
        if (what == "speed") w.SetValue<float>("Speed Limit", (float)part.NumParam);
        else if (what == "strength") w.Strength = (float)Math.Max(0, Math.Min(100, part.NumParam));
        acted = true;
    }
    return acted;
}

bool SetHandbrake(bool on)
{
    bool acted = false;
    foreach (var sc in _shipControllers) { sc.HandBrake = on; acted = true; }
    return acted;
}

// Trigger another programmable block by name with an optional argument (second quote).
bool RunOtherPB(ActionPart part)
{
    if (string.IsNullOrEmpty(part.ActTarget)) return false;
    IMyProgrammableBlock target = null;
    foreach (var pb in _otherPBs)
        if (pb.CustomName.Equals(part.ActTarget, StringComparison.OrdinalIgnoreCase) || NameMatch(pb, part.ActTarget))
        { target = pb; break; }
    if (target == null) { _warnings.Add("PB not found: " + part.ActTarget); return false; }
    target.TryRun(part.SecondTarget ?? "");
    return true;
}

// IGC broadcast: first quote = message, second quote = channel (defaults to "MPX").
bool IGCBroadcast(ActionPart part)
{
    if (string.IsNullOrEmpty(part.ActTarget)) return false;
    string channel = string.IsNullOrEmpty(part.SecondTarget) ? "MPX" : part.SecondTarget;
    IGC.SendBroadcastMessage(channel, part.ActTarget);
    AddTrace(TraceTag(null) + " IGC " + channel + ": " + part.ActTarget);
    return true;
}

// Per-tick: stop any sound block whose scheduled deadline has elapsed.
// Called from Main() so it runs even when no rule fires this tick.
void ProcessScheduledSoundStops()
{
    if (_soundStopAtTick.Count == 0) return;
    List<long> expired = null;
    foreach (var kv in _soundStopAtTick)
    {
        if (kv.Value <= _tick)
        {
            if (expired == null) expired = new List<long>();
            expired.Add(kv.Key);
        }
    }
    if (expired == null) return;
    foreach (var id in expired)
    {
        foreach (var s in _soundBlocks)
        {
            if (s.EntityId == id) { s.Stop(); break; }
        }
        _soundStopAtTick.Remove(id);
    }
}

// --- Block targeting helpers ---

bool SetBlocksEnabled(string quotedTarget, string rawLower, bool enabled, ParsedRule rule, ActionPart part)
{
    bool acted = false;
    var blocks = FindTargetBlocks(quotedTarget, rawLower);
    foreach (var b in blocks)
    {
        if (b == Me) continue; // never disable ourselves
        var func = b as IMyFunctionalBlock;
        if (func != null && ApplyEnabled(func, enabled, rule, part)) acted = true;
    }
    return acted;
}

// Centralized Enabled-state setter with three protections:
//   1) Debounce — skips the write (and returns false) when the block is already in the desired state.
//      Prevents the cooldown from being bumped on no-op fires, and keeps the trace clean.
//   2) Same-tick conflict detection — if a different rule already set this block's desired Enabled
//      to the OPPOSITE value this tick, a warning is added to _warnings (shown on MPX CORE).
//   3) Action trace — every actual ON↔OFF transition gets a line in the trace ring, so it's obvious
//      from the LCD which rule changed which block (the live-test "reactor flips off then on" needs this).
bool ApplyEnabled(IMyFunctionalBlock func, bool enabled, ParsedRule rule, ActionPart part)
{
    if (func == null || func == Me) return false;
    long id = func.EntityId;
    bool prevDesired;
    if (_tickDesiredEnabled.TryGetValue(id, out prevDesired) && prevDesired != enabled)
    {
        _warnings.Add("Conflict: '" + func.CustomName + "' ON+OFF same tick");
    }
    _tickDesiredEnabled[id] = enabled;
    if (func.Enabled == enabled) return false; // debounce — already in desired state
    func.Enabled = enabled;
    AddTrace(TraceTag(rule) + " → " + func.CustomName + " " + (enabled ? "ON" : "OFF"));
    return true;
}

// Bump per-rule fire counter for the STATS PB arg / heat-map display.
void BumpRuleFireCount(ParsedRule rule)
{
    string key = !string.IsNullOrEmpty(rule.Label) ? rule.Label : (rule.Raw.Length > 36 ? rule.Raw.Substring(0, 36) : rule.Raw);
    int n; _ruleFireCount.TryGetValue(key, out n);
    _ruleFireCount[key] = n + 1;
}

// Trace tag: rule label if set, else truncated rule text.
// Lets short LCDs stay readable while still uniquely identifying the rule that fired.
string TraceTag(ParsedRule r)
{
    if (r == null) return "?";
    if (!string.IsNullOrEmpty(r.Label)) return "[" + r.Label + "]";
    return TruncRule(r);
}

void AddTrace(string entry)
{
    _actionTrace.Add(entry);
    while (_actionTrace.Count > ACTION_TRACE_MAX) _actionTrace.RemoveAt(0);
}

string TruncRule(ParsedRule r)
{
    if (r == null) return "?";
    string raw = r.Raw ?? "";
    return raw.Length > 36 ? raw.Substring(0, 33) + "..." : raw;
}

List<IMyTerminalBlock> FindTargetBlocks(string quotedTarget, string rawLower)
{
    var result = new List<IMyTerminalBlock>();

    if (!string.IsNullOrEmpty(quotedTarget))
    {
        // Quoted = group first, then exact block — via shared ResolveQuoted helper
        var resolved = ResolveQuoted(quotedTarget);
        if (resolved.Count > 0) { result.AddRange(resolved); return result; }
        _warnings.Add($"Target not found: {quotedTarget}");
        return result;
    }

    // Smart targeting by keyword in raw rule
    if (Contains(rawLower, "reactor")) foreach (var b in _reactors) result.Add(b);
    else if (Contains(rawLower, "batter")) foreach (var b in _batteries) result.Add(b);
    else if (Contains(rawLower, "turret") || Contains(rawLower, "gun"))
        foreach (var b in _turrets) result.Add(b);
    else if (Contains(rawLower, "thruster")) foreach (var b in _thrusters) result.Add(b);
    else if (Contains(rawLower, "light")) foreach (var b in _lights) result.Add(b);
    else if (Contains(rawLower, "sound") || Contains(rawLower, "alarm") || Contains(rawLower, "siren"))
        foreach (var b in _soundBlocks) result.Add(b);
    else if (Contains(rawLower, "h2 gen") || Contains(rawLower, "hydrogen gen") || Contains(rawLower, "gas gen") || Contains(rawLower, "oxygen gen"))
        foreach (var b in _gasGens) result.Add(b);
    else if (Contains(rawLower, "assembler")) foreach (var b in _assemblers) result.Add(b);
    else if (Contains(rawLower, "refinery") || Contains(rawLower, "refineries")) foreach (var b in _refineries) result.Add(b);
    else if (Contains(rawLower, "air vent")) foreach (var b in _airVents) result.Add(b);
    else if (Contains(rawLower, "connector")) foreach (var b in _connectors) result.Add(b);
    else if (Contains(rawLower, "piston")) foreach (var b in _pistons) result.Add(b);
    else if (Contains(rawLower, "rotor") || Contains(rawLower, "hinge")) foreach (var b in _rotors) result.Add(b);
    else if (Contains(rawLower, "timer")) foreach (var b in _timers) result.Add(b);
    else if (Contains(rawLower, "sorter")) foreach (var b in _sorters) result.Add(b);
    else if (Contains(rawLower, "antenna")) foreach (var b in _antennas) result.Add(b);
    else if (Contains(rawLower, "camera")) foreach (var b in _cameras) result.Add(b);
    else if (Contains(rawLower, "beacon")) foreach (var b in _beacons) result.Add(b);
    else if (Contains(rawLower, "projector")) foreach (var b in _projectors) result.Add(b);
    else if (Contains(rawLower, "jump drive") || Contains(rawLower, "jumpdrive")) foreach (var b in _jumpDrives) result.Add(b);
    else if (Contains(rawLower, "gyro")) foreach (var b in _gyros) result.Add(b);
    else if (Contains(rawLower, "gear") || Contains(rawLower, "landing")) foreach (var b in _gears) result.Add(b);
    else if (Contains(rawLower, "parachute")) foreach (var b in _parachutes) result.Add(b);
    else if (Contains(rawLower, "wheel") || Contains(rawLower, "suspension")) foreach (var b in _wheels) result.Add(b);
    else if (Contains(rawLower, "solar")) foreach (var b in _solar) result.Add(b);
    return result;
}

// Quoted target = group first, exact block second (consistent across ALL actions).
// No quote = full cached list of that block type.
List<IMyBatteryBlock> GetTargetBatteries(string quotedTarget)
{
    if (!string.IsNullOrEmpty(quotedTarget))
    {
        var resolved = ResolveQuoted(quotedTarget);
        var r2 = new List<IMyBatteryBlock>();
        foreach (var b in resolved) { var bat = b as IMyBatteryBlock; if (bat != null) r2.Add(bat); }
        if (r2.Count == 0) _warnings.Add($"Target not found: {quotedTarget}");
        return r2;
    }
    return _batteries;
}

List<IMyDoor> GetTargetDoors(string quotedTarget, bool hangarOnly)
{
    var result = new List<IMyDoor>();
    if (!string.IsNullOrEmpty(quotedTarget))
    {
        var resolved = ResolveQuoted(quotedTarget);
        foreach (var b in resolved) { var d = b as IMyDoor; if (d != null) result.Add(d); }
        if (result.Count == 0) _warnings.Add($"Target not found: {quotedTarget}");
        return result;
    }
    foreach (var d in _doors)
        if (!hangarOnly || IsHangarDoor(d)) result.Add(d);
    return result;
}

bool CloseHangarDoors(string quotedTarget)
{
    bool acted = false;
    // Quoted target: respect group/exact-block lookup; close every door in it (filtering by hangar
    // is too restrictive when the player explicitly named a group).
    if (!string.IsNullOrEmpty(quotedTarget))
    {
        foreach (var b in ResolveQuoted(quotedTarget))
        {
            var d = b as IMyDoor;
            if (d != null) { d.CloseDoor(); acted = true; }
        }
        if (!acted) _warnings.Add($"Target not found: {quotedTarget}");
        return acted;
    }
    // No quote: close every door that IsHangarDoor reports as hangar (covers airtight hangar
    // doors via subtype "Hangar", display name "Hangar Door", or any door with "hangar" in name).
    foreach (var d in _doors)
        if (IsHangarDoor(d)) { d.CloseDoor(); acted = true; }
    return acted;
}

bool OpenHangarDoors(string quotedTarget)
{
    bool acted = false;
    if (!string.IsNullOrEmpty(quotedTarget))
    {
        foreach (var b in ResolveQuoted(quotedTarget))
        {
            var d = b as IMyDoor;
            if (d != null) { d.OpenDoor(); acted = true; }
        }
        if (!acted) _warnings.Add($"Target not found: {quotedTarget}");
        return acted;
    }
    foreach (var d in _doors)
        if (IsHangarDoor(d)) { d.OpenDoor(); acted = true; }
    return acted;
}

bool SetLightColor(string quotedTarget, string colorName, ParsedRule rule, ActionPart part)
{
    Color c = ColorFromName(colorName);
    bool acted = false;
    bool anyMatched = false;
    if (!string.IsNullOrEmpty(quotedTarget))
    {
        foreach (var b in ResolveQuoted(quotedTarget))
        {
            var l = b as IMyLightingBlock;
            if (l == null) continue;
            anyMatched = true;
            if (ApplyLight(l, colorName, c, rule)) acted = true;
        }
        if (!anyMatched) _warnings.Add("Target not found: " + quotedTarget);
        return acted;
    }
    foreach (var l in _lights)
        if (ApplyLight(l, colorName, c, rule)) acted = true;
    return acted;
}

// Debounced light setter — applies enable + color only if either differs, and traces the change.
// Returns true only when the block actually changed, so the cooldown only bumps on real transitions.
bool ApplyLight(IMyLightingBlock l, string colorName, Color c, ParsedRule rule)
{
    if (l == null) return false;
    if (colorName == "off")
    {
        if (!l.Enabled) return false;
        l.Enabled = false;
        AddTrace(TraceTag(rule) + " → " + l.CustomName + " LIGHT OFF");
        return true;
    }
    bool changed = false;
    if (!l.Enabled) { l.Enabled = true; changed = true; }
    if (l.Color != c) { l.Color = c; changed = true; }
    if (changed) AddTrace(TraceTag(rule) + " → " + l.CustomName + " " + colorName.ToUpper());
    return changed;
}

bool LockConnectors(string quotedTarget, bool connect)
{
    bool acted = false;
    if (!string.IsNullOrEmpty(quotedTarget))
    {
        foreach (var b in ResolveQuoted(quotedTarget))
        {
            var c = b as IMyShipConnector;
            if (c == null) continue;
            if (connect) c.Connect(); else c.Disconnect();
            acted = true;
        }
        if (!acted) _warnings.Add($"Target not found: {quotedTarget}");
        return acted;
    }
    foreach (var c in _connectors)
    {
        if (connect) c.Connect(); else c.Disconnect();
        acted = true;
    }
    return acted;
}

Color ColorFromName(string name)
{
    switch (name)
    {
        case "red": return new Color(255, 0, 0);
        case "green": return new Color(0, 255, 0);
        case "blue": return new Color(0, 0, 255);
        case "orange": return new Color(255, 140, 0);
        case "white": return new Color(255, 255, 255);
        case "yellow": return new Color(255, 255, 0);
        case "cyan": return new Color(0, 255, 255);
        default: return new Color(255, 255, 255);
    }
}

List<IMySoundBlock> GetTargetSounds(string quotedTarget)
{
    if (!string.IsNullOrEmpty(quotedTarget))
    {
        var resolved = ResolveQuoted(quotedTarget);
        var r2 = new List<IMySoundBlock>();
        foreach (var b in resolved) { var s = b as IMySoundBlock; if (s != null) r2.Add(s); }
        if (r2.Count == 0) _warnings.Add($"Target not found: {quotedTarget}");
        return r2;
    }
    return _soundBlocks;
}

// Safe subtype string accessor — handles SE PB versions where SubtypeId may be a string
// OR a MyStringHash struct. Calling ToString() works for both shapes.
// Falls back through SubtypeId → DefinitionDisplayNameText → "" so callers always get a string.
string GetSubtype(IMyTerminalBlock b)
{
    if (b == null) return "";
    string s = null;
    try
    {
        // ToString() handles both: string → returns itself; MyStringHash struct → returns its string value.
        // Direct call avoids comparing a possibly-struct value to null (which fails to compile against MyStringHash).
        s = b.BlockDefinition.SubtypeId.ToString();
    }
    catch { s = null; }
    if (string.IsNullOrEmpty(s))
    {
        try { s = b.DefinitionDisplayNameText; } catch { s = null; }
    }
    return s ?? "";
}

// Hangar door detection without depending on IMyAirtightHangarDoor.
// True if subtype contains "Hangar", display name contains "hangar", or custom name contains "hangar".
bool IsHangarDoor(IMyDoor d)
{
    if (d == null) return false;
    string sub = GetSubtype(d);
    string disp = "";
    try { disp = d.DefinitionDisplayNameText ?? ""; } catch { }
    return sub.IndexOf("Hangar", StringComparison.OrdinalIgnoreCase) >= 0
        || disp.IndexOf("hangar", StringComparison.OrdinalIgnoreCase) >= 0
        || d.CustomName.IndexOf("hangar", StringComparison.OrdinalIgnoreCase) >= 0;
}

// Resolve a quoted target string consistently across all actions:
//   1) Try block group with that exact name → return its blocks
//   2) Try exact terminal block name → return single-element list
//   3) Otherwise return empty list (caller may add a "target not found" warning)
List<IMyTerminalBlock> ResolveQuoted(string quoted)
{
    var result = new List<IMyTerminalBlock>();
    if (string.IsNullOrEmpty(quoted)) return result;

    var grp = GridTerminalSystem.GetBlockGroupWithName(quoted);
    if (grp != null)
    {
        grp.GetBlocks(result);
        if (result.Count > 0) return result;
    }
    var blk = GridTerminalSystem.GetBlockWithName(quoted);
    if (blk != null) result.Add(blk);
    return result;
}

bool IsHydrogenTank(IMyGasTank t)
{
    if (t == null) return false;
    string sub = GetSubtype(t);
    string name = t.CustomName ?? "";
    return sub.IndexOf("Hydrogen", StringComparison.OrdinalIgnoreCase) >= 0
        || sub.IndexOf("H2", StringComparison.OrdinalIgnoreCase) >= 0
        || name.IndexOf("Hydrogen", StringComparison.OrdinalIgnoreCase) >= 0
        || name.IndexOf("H2", StringComparison.OrdinalIgnoreCase) >= 0;
}

bool IsOxygenTank(IMyGasTank t)
{
    if (t == null) return false;
    string sub = GetSubtype(t);
    string name = t.CustomName ?? "";
    // exclude hydrogen so a hydrogen tank doesn't match "Oxygen" via fallback
    if (sub.IndexOf("Hydrogen", StringComparison.OrdinalIgnoreCase) >= 0) return false;
    return sub.IndexOf("Oxygen", StringComparison.OrdinalIgnoreCase) >= 0
        || name.IndexOf("Oxygen", StringComparison.OrdinalIgnoreCase) >= 0
        || name.IndexOf("O2", StringComparison.OrdinalIgnoreCase) >= 0;
}

bool NameMatch(IMyTerminalBlock b, string name)
    => b.CustomName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0;

IMyTerminalBlock FindBlock(string name)
{
    if (string.IsNullOrEmpty(name)) return null;
    foreach (var b in _allBlocks)
        if (b.CustomName.Equals(name, StringComparison.OrdinalIgnoreCase)) return b;
    return null;
}
#endregion

#region INVENTORY / PRODUCTION

// Rebuilds _invCache once per tick so multiple ItemBelow rules don't each scan all inventories.
// Key format: "TypeShort/Subtype" (e.g. "Component/SteelPlate", "Ore/Iron", "Ingot/Iron") so
// items with the same Subtype but different TypeId don't collide.
void RebuildInvCache()
{
    if (_invCacheTick == _tick) return;
    _invCacheTick = _tick;
    _invCache.Clear();
    var items = new List<MyInventoryItem>();
    foreach (var b in _allBlocks)
    {
        for (int i = 0; i < b.InventoryCount; i++)
        {
            items.Clear();
            b.GetInventory(i).GetItems(items);
            foreach (var item in items)
            {
                int cur;
                string typeShort = item.Type.TypeId.Replace("MyObjectBuilder_", "");
                string key = typeShort + "/" + item.Type.SubtypeId;
                _invCache.TryGetValue(key, out cur);
                _invCache[key] = cur + (int)(double)item.Amount;
            }
        }
    }
}

// Look up the count for an alias-resolved key. Accepts the new "TypeShort/Subtype" form;
// also accepts a bare subtype for back-compat (assumed to be a Component).
int CountInventoryItem(string key)
{
    RebuildInvCache();
    if (string.IsNullOrEmpty(key)) return 0;
    int result;
    if (key.IndexOf('/') >= 0)
    {
        _invCache.TryGetValue(key, out result);
        return result;
    }
    _invCache.TryGetValue("Component/" + key, out result);
    return result;
}

bool QueueProduction(ParsedRule rule, ActionPart part)
{
    // Identify item: prefer the action part's own ItemName, fall back to the rule's condition-side
    // item, then last resort: scan the raw line text. Keeps `generate more` working both as
    // `if X below 100 generate more` (uses condition item) and `generate more "steel plates"`.
    string itemId = !string.IsNullOrEmpty(part.ItemName) ? part.ItemName : rule.ItemName;
    if (string.IsNullOrEmpty(itemId))
    {
        string rawLower = rule.Raw.ToLowerInvariant();
        foreach (var alias in ItemAliases.Keys)
            if (Contains(rawLower, alias)) { itemId = ItemAliases[alias]; break; }
    }

    if (string.IsNullOrEmpty(itemId))
    {
        _warnings.Add("GenerateMore: item not identified in rule: " + rule.Raw.Substring(0, Math.Min(30, rule.Raw.Length)));
        return false;
    }

    // Strip "TypeShort/" prefix when looking up the blueprint dictionary.
    // Reject ores/ingots — those aren't producible at assemblers.
    string typeShort = "";
    string subId = itemId;
    int slash = itemId.IndexOf('/');
    if (slash >= 0)
    {
        typeShort = itemId.Substring(0, slash);
        subId = itemId.Substring(slash + 1);
    }
    if (typeShort == "Ore" || typeShort == "Ingot")
    {
        _warnings.Add("Cannot generate ore/ingot at assembler: " + itemId);
        return false;
    }

    string produceKey = "produce|" + itemId + "|" + part.ActTarget;
    if (_produceCooldowns.ContainsKey(produceKey)) return false;

    string bpId;
    if (!InvSubtypeToBlueprint.TryGetValue(subId, out bpId))
    {
        _warnings.Add($"No blueprint for: {itemId}");
        return false;
    }

    int have = CountInventoryItem(itemId);
    int need = (int)rule.CondValue;
    int missing = need - have;
    if (missing <= 0) return false;

    var targetAssemblers = string.IsNullOrEmpty(part.ActTarget)
        ? _assemblers
        : GetGroupAssemblers(part.ActTarget);

    if (targetAssemblers.Count == 0)
    {
        _warnings.Add("No assembler found.");
        return false;
    }

    MyDefinitionId bp;
    if (!MyDefinitionId.TryParse(bpId, out bp))
    {
        _warnings.Add($"Bad blueprint ID: {bpId}");
        return false;
    }

    bool queued = false;
    foreach (var a in targetAssemblers)
    {
        if (a.CanUseBlueprint(bp))
        {
            a.AddQueueItem(bp, (MyFixedPoint)missing);
            queued = true;
            break;
        }
    }

    if (queued)
    {
        _produceCooldowns[produceKey] = PRODUCE_COOLDOWN_TICKS;
        Log($"Queued {missing}x {itemId}");
    }
    else
        _warnings.Add($"Assembler cannot make: {itemId}");

    return queued;
}

List<IMyAssembler> GetGroupAssemblers(string groupName)
{
    var result = new List<IMyAssembler>();
    foreach (var b in ResolveQuoted(groupName))
    {
        var a = b as IMyAssembler;
        if (a != null) result.Add(a);
    }
    return result;
}

// Pulls ores from any cargo on the construct into refinery input inventories (index 0).
// Skips refineries that are not Working (built + enabled + powered). Cooldown-protected by the
// caller via the part's cooldown key — runs at most once per ACTION_COOLDOWN_TICKS window.
bool MoveOresToRefinery(ActionPart part)
{
    if (_refineries.Count == 0) { _warnings.Add("No refinery on construct."); return false; }
    bool acted = false;
    var items = new List<MyInventoryItem>();
    foreach (var r in _refineries)
    {
        if (!r.IsWorking) continue;
        if (!string.IsNullOrEmpty(part.ActTarget) && !NameMatch(r, part.ActTarget)) continue;
        var refInv = r.InputInventory;
        if (refInv == null) continue;
        foreach (var cargo in _cargo)
        {
            var cargoInv = cargo.GetInventory();
            items.Clear();
            cargoInv.GetItems(items);
            for (int idx = items.Count - 1; idx >= 0; idx--)
            {
                var item = items[idx];
                // Only ore (TypeId "MyObjectBuilder_Ore"); skip stone/ice unless that's what's there.
                if (!item.Type.TypeId.ToString().Contains("Ore")) continue;
                if (!cargoInv.CanTransferItemTo(refInv, item.Type)) continue;
                cargoInv.TransferItemTo(refInv, idx, null, true, null);
                acted = true;
            }
        }
    }
    return acted;
}

// Pulls ingots from refinery output AND cargo into assembler input inventories (index 0).
// Same protections as MoveOresToRefinery.
bool MoveIngotsToAssembler(ActionPart part)
{
    if (_assemblers.Count == 0) { _warnings.Add("No assembler on construct."); return false; }
    bool acted = false;
    var items = new List<MyInventoryItem>();
    foreach (var a in _assemblers)
    {
        if (!a.IsWorking) continue;
        if (!string.IsNullOrEmpty(part.ActTarget) && !NameMatch(a, part.ActTarget)) continue;
        var asmInv = a.InputInventory;
        if (asmInv == null) continue;

        // Pull from refinery OUTPUTS first (closer to point of production).
        foreach (var r in _refineries)
        {
            var src = r.OutputInventory;
            if (src == null) continue;
            items.Clear();
            src.GetItems(items);
            for (int idx = items.Count - 1; idx >= 0; idx--)
            {
                var item = items[idx];
                if (!item.Type.TypeId.ToString().Contains("Ingot")) continue;
                if (!src.CanTransferItemTo(asmInv, item.Type)) continue;
                src.TransferItemTo(asmInv, idx, null, true, null);
                acted = true;
            }
        }

        // Then pull any ingots sitting in cargo.
        foreach (var cargo in _cargo)
        {
            var cargoInv = cargo.GetInventory();
            items.Clear();
            cargoInv.GetItems(items);
            for (int idx = items.Count - 1; idx >= 0; idx--)
            {
                var item = items[idx];
                if (!item.Type.TypeId.ToString().Contains("Ingot")) continue;
                if (!cargoInv.CanTransferItemTo(asmInv, item.Type)) continue;
                cargoInv.TransferItemTo(asmInv, idx, null, true, null);
                acted = true;
            }
        }
    }
    return acted;
}

bool RefillTurrets(ParsedRule rule, ActionPart part)
{
    bool acted = false;
    var items = new List<MyInventoryItem>();
    foreach (var turret in _turrets)
    {
        if (!string.IsNullOrEmpty(part.ActTarget) && !NameMatch(turret, part.ActTarget)) continue;
        var turretInv = turret.GetInventory();
        foreach (var cargo in _cargo)
        {
            var cargoInv = cargo.GetInventory();
            items.Clear();
            cargoInv.GetItems(items);
            // Iterate by index so we're not holding a stale item reference after transfers
            for (int idx = items.Count - 1; idx >= 0; idx--)
            {
                var item = items[idx];
                if (!item.Type.TypeId.ToString().Contains("AmmoMagazine")) continue;
                if (!cargoInv.CanTransferItemTo(turretInv, item.Type)) continue;
                // Use index-based overload: most compatible across SE PB versions
                cargoInv.TransferItemTo(turretInv, idx, null, true, (MyFixedPoint)10);
                acted = true;
            }
        }
    }
    return acted;
}
#endregion

#region LCD DRAWING — CORE
static readonly char[] SPINNER = { '|', '/', '-', '\\' };

void DrawCoreLCD()
{
    if (_lcdCore == null) return;

    char spin = SPINNER[_spinnerIdx];
    string dots = _scanDotIdx == 0 ? "." : (_scanDotIdx == 1 ? ".." : "...");
    string status = _paused ? "PAUSED" : "ONLINE";
    string alert = _enemyDetected ? "!!! ALERT !!!" : "NOMINAL";
    string alertColor = _enemyDetected ? "[ HOSTILE DETECTED ]" : "[ SECURE ]";

    var sb = new StringBuilder();
    sb.AppendLine("╔══════════════════════════════╗");
    sb.AppendLine("║  MPX HUMAN LOGIC BRAIN  v1.0 ║");
    sb.AppendLine("╚══════════════════════════════╝");
    sb.AppendLine($"  STATUS  : [{status}] {spin}");
    sb.AppendLine($"  MODE    : {(string.IsNullOrEmpty(_activeMode) ? "default" : _activeMode.ToUpperInvariant())}");
    sb.AppendLine($"  RULES   : {_rules.Count} loaded" + (_mutedLabels.Count > 0 ? $"  ({_mutedLabels.Count} muted)" : ""));
    sb.AppendLine($"  PARSED  : {_lastUnderstood}");
    if (_simulated.Count > 0)
    {
        var simNames = new List<string>(_simulated.Keys);
        sb.AppendLine($"  SIM     : {string.Join(", ", simNames)}");
    }
    sb.AppendLine("──────────────────────────────");
    sb.AppendLine($"  ALERT   : {alert}");
    sb.AppendLine($"  THREAT  : {alertColor}");
    if (_enemyDetected)
        sb.AppendLine($"  SOURCE  : {_enemySource}");
    sb.AppendLine("──────────────────────────────");
    sb.AppendLine($"  SCAN    : SCANNING{dots}");
    sb.AppendLine($"  LAST HIT: {_lastTriggered}");
    sb.AppendLine($"  ACTION  : {_lastActions}");
    sb.AppendLine("──────────────────────────────");

    if (_warnings.Count > 0)
    {
        sb.AppendLine("  [WARNINGS]");
        int shown = 0;
        foreach (var w in _warnings) { if (shown++ >= 4) break; sb.AppendLine($"  ! {w}"); }
    }

    // Solar debug — exposes everything needed to diagnose stuck/unexpected dark/daylight rules.
    // Day/night decisions use SOLAR BEST (a single panel seeing the sun = daylight).
    string dark = _solarAvailable ? (_solarState == SolarState.Dark ? "YES" : "NO") : "N/A";
    string day = _solarAvailable ? (_solarState == SolarState.Daylight ? "YES" : "NO") : "N/A";
    string known = _solarAvailable ? "YES" : "NO";
    int darkSec = _solarDarkTicks / 6;
    int lightSec = _solarLightTicks / 6;
    sb.AppendLine($"  Solar Source    : {_solarSource}");
    sb.AppendLine($"  Solar Panels    : {_solarPanelCount}");
    sb.AppendLine($"  Solar Best      : {_solarBestPct:F0}%");
    sb.AppendLine($"  Solar Avg       : {_solarAvgPct:F0}%");
    sb.AppendLine($"  Solar State     : {_solarState}   KNOWN: {known}");
    sb.AppendLine($"  Dark Seconds    : {darkSec}");
    sb.AppendLine($"  Daylight Seconds: {lightSec}");
    sb.AppendLine($"  DARK: {dark}   DAYLIGHT: {day}");
    if (_lastSolarChangeTick > 0)
        sb.AppendLine($"  LAST CHG: tick {_lastSolarChangeTick}");
    if (!string.IsNullOrEmpty(_solarWarning))
        sb.AppendLine($"  ! {_solarWarning}");
    sb.AppendLine($"  DOCKED  : {(_connectorDocked ? "YES" : "NO")}");

    if (_actionTrace.Count > 0)
    {
        sb.AppendLine("──────────────────────────────");
        sb.AppendLine("  [TRACE — newest first]");
        for (int i = _actionTrace.Count - 1; i >= 0; i--)
            sb.AppendLine($"  > {_actionTrace[i]}");
    }

    // Top-rule heat map (run STATS PB arg for full list)
    if (_ruleFireCount.Count > 0)
    {
        sb.AppendLine("──────────────────────────────");
        sb.AppendLine("  [TOP RULES]");
        var top = new List<KeyValuePair<string, int>>(_ruleFireCount);
        top.Sort((a, b) => b.Value.CompareTo(a.Value));
        int shown = 0;
        foreach (var kv in top)
        {
            if (shown++ >= 3) break;
            sb.AppendLine($"  {kv.Value,4}× {kv.Key}");
        }
    }

    // Variables peek — show up to 4 most recently changed (Dictionary order isn't insertion in PB,
    // but we just show whatever exists)
    if (_vars.Count > 0)
    {
        sb.AppendLine("──────────────────────────────");
        sb.AppendLine("  [VARS]");
        int shown = 0;
        foreach (var kv in _vars)
        {
            if (shown++ >= 4) break;
            sb.AppendLine($"  {kv.Key} = {kv.Value}");
        }
    }

    sb.AppendLine("══════════════════════════════");

    _lcdCore.WriteText(sb.ToString());
}
#endregion

#region LCD DRAWING — DISPLAY
void DrawDisplayLCD()
{
    if (_lcdDisplay == null) return;

    char spin = SPINNER[(_spinnerIdx + 2) % 4];
    var sb = new StringBuilder();

    sb.AppendLine("╔══════════════════════════════╗");
    sb.AppendLine($"║  MPX TELEMETRY {spin}              ║");
    sb.AppendLine("╚══════════════════════════════╝");
    sb.AppendLine();
    sb.AppendLine(BuildBar("PWR", _powerPct));
    sb.AppendLine(BuildBar("H2 ", _hydrogenPct));
    sb.AppendLine(BuildBar("O2 ", _oxygenPct));
    sb.AppendLine(BuildBar("CGO", _cargoPct));
    sb.AppendLine(BuildBar("SOL", _solarPct));
    sb.AppendLine();
    sb.AppendLine($"  ENEMY   : {(_enemyDetected ? "YES <<ALERT>>" : "NO")}");
    string lightLbl;
    if (!_solarAvailable) lightLbl = "UNKNOWN";
    else if (_solarState == SolarState.Dark) lightLbl = "DARK/NIGHT";
    else if (_solarState == SolarState.Daylight) lightLbl = "DAYLIGHT";
    else lightLbl = "UNKNOWN";
    sb.AppendLine($"  LIGHT   : {lightLbl}");
    sb.AppendLine($"  DOCKED  : {(_connectorDocked ? "YES" : "NO")}");
    sb.AppendLine($"  SPEED   : {_shipSpeed:F1} m/s    ALT: {(_shipAltitude > 0 ? _shipAltitude.ToString("F0") + " m" : "n/a")}");
    sb.AppendLine($"  PILOT   : {(_pilotPresent ? "YES" : "NO")}");
    sb.AppendLine();
    sb.AppendLine("──────────────────────────────");
    sb.AppendLine($"  Reactors : {_reactors.Count}  Batteries: {_batteries.Count}");
    sb.AppendLine($"  Turrets  : {_turrets.Count}  Assemblers: {_assemblers.Count}");
    sb.AppendLine($"  Refineries: {_refineries.Count}  Pistons: {_pistons.Count}");
    sb.AppendLine($"  Rotors   : {_rotors.Count}  Sorters: {_sorters.Count}");
    sb.AppendLine($"  Timers   : {_timers.Count}  Antennas: {_antennas.Count}");
    sb.AppendLine($"  Doors    : {_doors.Count}   Sensors: {_sensors.Count}");
    sb.AppendLine("══════════════════════════════");

    _lcdDisplay.WriteText(sb.ToString());
}

string BuildBar(string label, double pct)
{
    pct = Math.Max(0, Math.Min(100, pct));
    int filled = (int)(pct / 5.0); // 20 chars wide
    var bar = new StringBuilder();
    bar.Append($"  {label} [");
    for (int i = 0; i < 20; i++) bar.Append(i < filled ? '█' : '░');
    bar.Append($"] {pct,5:F1}%");
    return bar.ToString();
}
#endregion

#region LOG
void Log(string msg)
{
    string entry = $"[{_tick,6}] {msg}";
    _logLines.Add(entry);
    if (_logLines.Count > MAX_LOG_LINES) _logLines.RemoveAt(0);

    if (_lcdLog != null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== MPX LOG ===");
        for (int i = _logLines.Count - 1; i >= 0; i--)
            sb.AppendLine(_logLines[i]);
        _lcdLog.WriteText(sb.ToString());
    }

    Echo(entry);
}
#endregion
