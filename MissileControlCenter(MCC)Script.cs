// ═══════════════════════════════════════════════════════════════════════
//  MFC - Missile Fire Control v2.0
//  Space Engineers Programmable Block Script
//
//  Controls VLS (Vertical Launch System) style missile grids with
//  door management, sequential/salvo firing, target tracking,
//  auto-fire, and sprite-based status displays.
//
//  COMMANDS:
//    fire                       Fire first available missile (any grid)
//    fire [Grid]                Fire first available missile from grid
//    fire [Grid] [Number]       Fire specific missile
//    fire [Number]              Fire specific missile from first grid
//    salvo [Number]             Fire N missiles from all grids
//    salvo [Grid] [Number]      Fire N missiles from specific grid
//    lock / unlock              Enable/disable firing
//    autofire on/off            Toggle automatic firing
//    holdfire                   Toggle hold-fire (pauses auto-fire)
//    TargetSubsystem [W/P/E]    Set subsystem (Weapons/Propulsion/Energy)
//    TargetPriority [C/L/S]     Set priority (Closest/Largest/Smallest)
//    page next/prev/[Name]      Navigate LCD pages
//    deletecommand [Grid]       Cancel active firing commands
//    validate                   Re-validate configuration
//    refresh                    Reload CustomData + validate
//    log clear                  Clear event log
//    help                       Show command list
//
//  LCD SETUP:
//    Add "MFC" (or your LCDTag) to any LCD/cockpit name.
//    Configure pages in the LCD's CustomData.
//    Pages: Overview, Grids, Targeting, Diagnostics, Log
//
//  CHANGELOG v2.0:
//    - Sprite-based displays with 5 navigable pages
//    - Target tracking via turrets + AI blocks
//    - Auto-fire with configurable range, interval, MinLockTime
//    - Event log with color-coded entries
//    - Config validation on refresh
//    - State persistence (lock/autofire/targeting survive recompile)
//    - Block caching for performance
//    - Fired missile counter (per grid + total)
// ═══════════════════════════════════════════════════════════════════════

const string SCRIPT_VERSION = "2.0";

#region Variables
List<MissileGrid> missileGrids = new List<MissileGrid>();
double MinimumHydrogenLevel = 0.9;
double MissileFireDelay = 0.5;
string LCDTag = "MFC";
double CommandTimeout = 8.0;
double DoorClosingDelay = 1.0;
bool EnableTargetingSettings = false;

// AutoFire
bool AutoFireEnabled = false;
double AutoFireMinRange = 200;
double AutoFireMaxRange = 2000;
int AutoFireSalvoSize = 1;
double AutoFireInterval = 3.0;
bool AutoFireRequireClosing = true;
double AutoFireMinLockTime = 1.5;
double autoFireCooldownRemaining = 0;
double currentLockDuration = 0;
bool holdFire = false;

// Target tracking
string TurretTag = "";
string AIBlockTag = "";

enum TargetSubsystem { Weapons, Propulsion, Energy }
enum TargetPriority { Closest, Largest, Smallest }

Dictionary<TargetSubsystem, string> TargetSubsystemTimerNames = new Dictionary<TargetSubsystem, string>();
Dictionary<TargetPriority, string> TargetPriorityTimerNames = new Dictionary<TargetPriority, string>();
TargetSubsystem currentTargetSubsystem = TargetSubsystem.Weapons;
TargetPriority currentTargetPriority = TargetPriority.Closest;

enum DoorState { Idle, OpeningDoors, FiringMissiles, WaitingToCloseDoors }

class MissileGrid
{
    public string Name;
    public List<string> DoorNames = new List<string>();
    public List<string> TimerBlockNames = new List<string>();
    public int TotalFired = 0;
}

class MissileTask
{
    public MissileGrid Grid;
    public int MissileIndex;
    public IMyTimerBlock TimerBlock;
    public List<IMyDoor> Doors;
}

class FiringCommand
{
    public List<MissileTask> missileTasks = new List<MissileTask>();
    public DoorState doorState = DoorState.OpeningDoors;
    public int currentTaskIndex = 0;
    public double elapsedTime = 0;
    public string commandDescription;
    public double commandTimeout = 8.0;
    public double commandElapsedTime = 0;
    public bool isAborted = false;
    public double doorClosingDelay = 1.0;
    public double doorDelayElapsedTime = 0;
    public bool targetingTimersTriggered = false;
}

Dictionary<string, Queue<FiringCommand>> firingCommandQueues = new Dictionary<string, Queue<FiringCommand>>();
bool isLocked = false;
string lastCommandResult = "";
string currentSystemActivity = "";
int totalFiredAllTime = 0;
double runtimeClock = 0;

// Block cache — refreshed every few seconds, not every tick
List<IMyLargeTurretBase> cachedTurrets = new List<IMyLargeTurretBase>();
List<IMyTurretControlBlock> cachedAIBlocks = new List<IMyTurretControlBlock>();
List<IMyShipController> cachedControllers = new List<IMyShipController>();
List<IMyTextPanel> cachedLCDs = new List<IMyTextPanel>();
List<IMyCockpit> cachedCockpits = new List<IMyCockpit>();
double blockCacheAge = 999; // force immediate refresh
const double BLOCK_CACHE_INTERVAL = 3.0;

static readonly System.Globalization.CultureInfo INV = System.Globalization.CultureInfo.InvariantCulture;
static bool TryParseDouble(string s, out double v) => double.TryParse(s, System.Globalization.NumberStyles.Float, INV, out v);
static bool TryParseFloat(string s, out float v) => float.TryParse(s, System.Globalization.NumberStyles.Float, INV, out v);
static bool TryParseInt(string s, out int v) => int.TryParse(s, System.Globalization.NumberStyles.Integer, INV, out v);
#endregion

#region Event Log
enum LogLevel { Info, Fire, Warn, Error }

class LogEntry
{
    public double Timestamp;
    public LogLevel Level;
    public string Message;
}

const int MAX_LOG_ENTRIES = 40;
List<LogEntry> eventLog = new List<LogEntry>();

void Log(LogLevel level, string message)
{
    eventLog.Add(new LogEntry { Timestamp = runtimeClock, Level = level, Message = message });
    if (eventLog.Count > MAX_LOG_ENTRIES) eventLog.RemoveAt(0);
}

string FormatTimestamp(double t)
{
    int total = (int)t;
    return string.Format("{0:D2}:{1:D2}:{2:D2}", (total / 3600) % 100, (total / 60) % 60, total % 60);
}
#endregion

#region Target Tracking
class TrackedTarget
{
    public bool HasTarget;
    public string Name;
    public string TypeStr;
    public double Range;
    public double RelativeSpeed;
    public bool Closing;
    public Vector3D Position;
    public Vector3D Velocity;
    public string SourceBlock;
}

TrackedTarget currentTarget = new TrackedTarget { HasTarget = false };

void UpdateTargetTracking()
{
    TrackedTarget best = null;
    double bestRange = double.MaxValue;

    Vector3D ownPos = Me.GetPosition();
    Vector3D ownVel = Vector3D.Zero;
    if (cachedControllers.Count > 0)
        ownVel = cachedControllers[0].GetShipVelocities().LinearVelocity;

    // Scan turrets (IMyLargeTurretBase)
    foreach (var turret in cachedTurrets)
    {
        if (!turret.HasTarget) continue;
        var info = turret.GetTargetedEntity();
        if (info.IsEmpty()) continue;
        var candidate = EvaluateTarget(info, ownPos, ownVel, turret.CustomName);
        if (candidate != null && candidate.Range < bestRange)
        {
            best = candidate;
            bestRange = candidate.Range;
        }
    }

    // Scan AI blocks (IMyTurretControlBlock)
    foreach (var ai in cachedAIBlocks)
    {
        if (!ai.HasTarget) continue;
        var info = ai.GetTargetedEntity();
        if (info.IsEmpty()) continue;
        var candidate = EvaluateTarget(info, ownPos, ownVel, ai.CustomName);
        if (candidate != null && candidate.Range < bestRange)
        {
            best = candidate;
            bestRange = candidate.Range;
        }
    }

    if (best != null)
    {
        if (!currentTarget.HasTarget || currentTarget.Name != best.Name)
        {
            Log(LogLevel.Info, $"Target acquired: {best.Name} @ {best.Range:F0}m");
            currentLockDuration = 0;
        }
        currentTarget = best;
    }
    else
    {
        if (currentTarget.HasTarget)
            Log(LogLevel.Info, "Target lost.");
        currentTarget = new TrackedTarget { HasTarget = false };
        currentLockDuration = 0;
    }
}

TrackedTarget EvaluateTarget(MyDetectedEntityInfo info, Vector3D ownPos, Vector3D ownVel, string sourceName)
{
    Vector3D tgtPos = info.Position;
    Vector3D tgtVel = info.Velocity;
    double range = Vector3D.Distance(ownPos, tgtPos);
    Vector3D relVel = tgtVel - ownVel;
    Vector3D toTarget = tgtPos - ownPos;
    if (toTarget.LengthSquared() < 0.001) return null;
    toTarget.Normalize();
    double closingSpeed = -Vector3D.Dot(relVel, toTarget);

    return new TrackedTarget
    {
        HasTarget = true,
        Name = string.IsNullOrEmpty(info.Name) ? "Unknown" : info.Name,
        TypeStr = info.Type.ToString(),
        Range = range,
        RelativeSpeed = relVel.Length(),
        Closing = closingSpeed > 0,
        Position = tgtPos,
        Velocity = tgtVel,
        SourceBlock = sourceName
    };
}
#endregion

#region Auto-Fire
void UpdateAutoFire(double deltaSeconds)
{
    if (autoFireCooldownRemaining > 0)
        autoFireCooldownRemaining -= deltaSeconds;

    if (!AutoFireEnabled || holdFire || isLocked) return;
    if (!currentTarget.HasTarget) return;

    currentLockDuration += deltaSeconds;
    if (currentLockDuration < AutoFireMinLockTime) return;
    if (currentTarget.Range < AutoFireMinRange || currentTarget.Range > AutoFireMaxRange) return;
    if (AutoFireRequireClosing && !currentTarget.Closing) return;
    if (autoFireCooldownRemaining > 0) return;

    int readyTotal = missileGrids.Sum(g => GetReadyMissileCount(g));
    if (readyTotal == 0) return;

    int salvoSize = Math.Min(AutoFireSalvoSize, readyTotal);
    string result = CreateFiringCommandForMultipleMissiles(null, salvoSize);
    Log(LogLevel.Fire, $"AUTO: {result}");
    lastCommandResult = $"AUTO: {result}";
    autoFireCooldownRemaining = AutoFireInterval;
}
#endregion

#region Config Validation
class ValidationResult
{
    public List<string> Errors = new List<string>();
    public List<string> Warnings = new List<string>();
    public bool Ok => Errors.Count == 0;
}

ValidationResult lastValidation = new ValidationResult();

void ValidateConfig()
{
    var r = new ValidationResult();

    if (missileGrids.Count == 0)
        r.Warnings.Add("No missile grids defined.");

    foreach (var grid in missileGrids)
    {
        if (grid.TimerBlockNames.Count == 0)
            r.Warnings.Add($"'{grid.Name}': no timers.");
        if (grid.DoorNames.Count == 0)
            r.Warnings.Add($"'{grid.Name}': no doors.");

        foreach (var tn in grid.TimerBlockNames)
        {
            var t = GridTerminalSystem.GetBlockWithName(tn);
            if (t == null) r.Errors.Add($"Timer '{tn}' ({grid.Name}) missing.");
            else if (!(t is IMyTimerBlock)) r.Errors.Add($"'{tn}' not a Timer.");
        }
        foreach (var dn in grid.DoorNames)
        {
            var d = GridTerminalSystem.GetBlockWithName(dn);
            if (d == null) r.Errors.Add($"Door '{dn}' ({grid.Name}) missing.");
            else if (!(d is IMyDoor)) r.Errors.Add($"'{dn}' not a Door.");
        }
    }

    if (EnableTargetingSettings)
    {
        foreach (var kvp in TargetSubsystemTimerNames)
        {
            if (string.IsNullOrEmpty(kvp.Value)) continue;
            if (!(GridTerminalSystem.GetBlockWithName(kvp.Value) is IMyTimerBlock))
                r.Warnings.Add($"TGT timer '{kvp.Value}' ({kvp.Key}) missing.");
        }
        foreach (var kvp in TargetPriorityTimerNames)
        {
            if (string.IsNullOrEmpty(kvp.Value)) continue;
            if (!(GridTerminalSystem.GetBlockWithName(kvp.Value) is IMyTimerBlock))
                r.Warnings.Add($"Priority timer '{kvp.Value}' ({kvp.Key}) missing.");
        }
    }

    lastValidation = r;
    foreach (var e in r.Errors) Log(LogLevel.Error, e);
    foreach (var w in r.Warnings) Log(LogLevel.Warn, w);
    if (r.Ok && r.Warnings.Count == 0) Log(LogLevel.Info, "Config valid.");
}
#endregion

#region Block Cache
void RefreshBlockCache(double delta)
{
    blockCacheAge += delta;
    if (blockCacheAge < BLOCK_CACHE_INTERVAL) return;
    blockCacheAge = 0;

    cachedTurrets.Clear();
    GridTerminalSystem.GetBlocksOfType(cachedTurrets,
        t => t.CubeGrid == Me.CubeGrid && (string.IsNullOrEmpty(TurretTag) || t.CustomName.Contains(TurretTag)));

    cachedAIBlocks.Clear();
    GridTerminalSystem.GetBlocksOfType(cachedAIBlocks,
        t => t.CubeGrid == Me.CubeGrid && (string.IsNullOrEmpty(AIBlockTag) || t.CustomName.Contains(AIBlockTag)));

    cachedControllers.Clear();
    GridTerminalSystem.GetBlocksOfType(cachedControllers, c => c.CubeGrid == Me.CubeGrid);

    cachedLCDs.Clear();
    GridTerminalSystem.GetBlocksOfType(cachedLCDs, lcd => lcd.CustomName.Contains(LCDTag));

    cachedCockpits.Clear();
    GridTerminalSystem.GetBlocksOfType(cachedCockpits, c => c.CustomName.Contains(LCDTag));
}
#endregion

#region State Persistence
public void Save()
{
    // Persist important state across recompile/reload
    var sb = new StringBuilder();
    sb.AppendLine($"locked={isLocked}");
    sb.AppendLine($"autofire={AutoFireEnabled}");
    sb.AppendLine($"holdfire={holdFire}");
    sb.AppendLine($"subsystem={currentTargetSubsystem}");
    sb.AppendLine($"priority={currentTargetPriority}");
    sb.AppendLine($"totalfired={totalFiredAllTime}");
    // Per-grid fired counts
    foreach (var g in missileGrids)
        sb.AppendLine($"gridfired:{g.Name}={g.TotalFired}");
    Storage = sb.ToString();
}

void LoadState()
{
    if (string.IsNullOrEmpty(Storage)) return;
    var lines = Storage.Split('\n');
    foreach (var line in lines)
    {
        var t = line.Trim();
        var parts = t.Split('=');
        if (parts.Length != 2) continue;
        var key = parts[0].Trim();
        var val = parts[1].Trim();

        if (key == "locked") bool.TryParse(val, out isLocked);
        else if (key == "autofire") bool.TryParse(val, out AutoFireEnabled);
        else if (key == "holdfire") bool.TryParse(val, out holdFire);
        else if (key == "subsystem") { TargetSubsystem s; if (Enum.TryParse(val, true, out s)) currentTargetSubsystem = s; }
        else if (key == "priority") { TargetPriority p; if (Enum.TryParse(val, true, out p)) currentTargetPriority = p; }
        else if (key == "totalfired") TryParseInt(val, out totalFiredAllTime);
        else if (key.StartsWith("gridfired:"))
        {
            var gridName = key.Substring("gridfired:".Length);
            int count;
            if (TryParseInt(val, out count))
            {
                var grid = missileGrids.FirstOrDefault(g => g.Name == gridName);
                if (grid != null) grid.TotalFired = count;
            }
        }
    }
}
#endregion

#region Main Program
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    ParseCustomData();
    LoadState();
    ValidateConfig();
    lastCommandResult = "MFC v" + SCRIPT_VERSION + " ready.";
    Log(LogLevel.Info, "System online.");
}

public void Main(string argument, UpdateType updateSource)
{
    double delta = Runtime.TimeSinceLastRun.TotalSeconds;
    runtimeClock += delta;

    if ((updateSource & (UpdateType.Update10 | UpdateType.Update1)) != 0)
    {
        RefreshBlockCache(delta);
        UpdateTargetTracking();
        UpdateAutoFire(delta);
        UpdateState();
        UpdateDisplays();
        return;
    }

    ParseCustomData();
    string cmd = (argument ?? "").Trim();

    if (string.IsNullOrEmpty(cmd) || cmd.Equals("refresh", StringComparison.OrdinalIgnoreCase))
    {
        ValidateConfig();
        lastCommandResult = lastValidation.Ok ? "Refreshed." : $"Config: {lastValidation.Errors.Count} error(s).";
    }
    else if (cmd.Equals("validate", StringComparison.OrdinalIgnoreCase))
    {
        ValidateConfig();
        lastCommandResult = lastValidation.Ok ? "Config valid." : $"Config: {lastValidation.Errors.Count} error(s).";
    }
    else if (cmd.Equals("help", StringComparison.OrdinalIgnoreCase))
    {
        lastCommandResult = "fire|salvo|lock|unlock|autofire|holdfire|page|help";
        Log(LogLevel.Info, "Help requested.");
    }
    else if (cmd.Equals("lock", StringComparison.OrdinalIgnoreCase))
    {
        isLocked = true;
        lastCommandResult = "Locked.";
        Log(LogLevel.Info, "System locked.");
    }
    else if (cmd.Equals("unlock", StringComparison.OrdinalIgnoreCase))
    {
        isLocked = false;
        lastCommandResult = "Unlocked.";
        Log(LogLevel.Info, "System unlocked.");
    }
    else if (cmd.StartsWith("page ", StringComparison.OrdinalIgnoreCase) || cmd.Equals("page", StringComparison.OrdinalIgnoreCase))
    {
        lastCommandResult = HandlePageCommand(cmd.Split(' '));
    }
    else if (cmd.StartsWith("autofire", StringComparison.OrdinalIgnoreCase))
    {
        var parts = cmd.Split(' ');
        if (parts.Length >= 2 && parts[1].Equals("on", StringComparison.OrdinalIgnoreCase))
        {
            AutoFireEnabled = true; lastCommandResult = "Auto-fire ON."; Log(LogLevel.Info, "Auto-fire ON");
        }
        else if (parts.Length >= 2 && parts[1].Equals("off", StringComparison.OrdinalIgnoreCase))
        {
            AutoFireEnabled = false; lastCommandResult = "Auto-fire OFF."; Log(LogLevel.Info, "Auto-fire OFF");
        }
        else lastCommandResult = "Use: autofire on|off";
    }
    else if (cmd.Equals("holdfire", StringComparison.OrdinalIgnoreCase))
    {
        holdFire = !holdFire;
        lastCommandResult = holdFire ? "Hold-fire ON." : "Hold-fire OFF.";
        Log(LogLevel.Info, lastCommandResult);
    }
    else if (cmd.StartsWith("log ", StringComparison.OrdinalIgnoreCase) || cmd.Equals("log", StringComparison.OrdinalIgnoreCase))
    {
        if (cmd.Equals("log clear", StringComparison.OrdinalIgnoreCase))
        {
            eventLog.Clear(); lastCommandResult = "Log cleared.";
        }
        else lastCommandResult = $"Log: {eventLog.Count} entries.";
    }
    else if (isLocked)
    {
        lastCommandResult = "Locked. Command refused.";
    }
    else
    {
        var args = cmd.Split(' ');
        var c = args[0];

        if (c.Equals("fire", StringComparison.OrdinalIgnoreCase))
            lastCommandResult = HandleFireCommand(args);
        else if (c.Equals("salvo", StringComparison.OrdinalIgnoreCase))
            lastCommandResult = HandleSalvoCommand(args);
        else if (c.Equals("deletecommand", StringComparison.OrdinalIgnoreCase))
            lastCommandResult = HandleDeleteCommand(args);
        else if (c.Equals("TargetSubsystem", StringComparison.OrdinalIgnoreCase))
            lastCommandResult = HandleSetSubsystemCommand(args);
        else if (c.Equals("TargetPriority", StringComparison.OrdinalIgnoreCase))
            lastCommandResult = HandleSetPriorityCommand(args);
        else
            lastCommandResult = $"Unknown: '{c}'. Try 'help'.";

        Log(LogLevel.Info, $"{cmd} -> {lastCommandResult}");
    }

    Echo(lastCommandResult);
    UpdateDisplays();
}
#endregion

#region Command Handlers
string HandleFireCommand(string[] args)
{
    // fire              → first available from any grid
    // fire 3            → missile 3 from first grid
    // fire Grid1        → first available from Grid1
    // fire Grid1 3      → missile 3 from Grid1
    if (args.Length == 1)
    {
        // fire → first available from first grid that has one
        foreach (var g in missileGrids)
        {
            var result = CreateFiringCommandForFirstAvailableMissile(g.Name);
            if (!result.Contains("not ready") && !result.Contains("not found") && !result.Contains("No missiles"))
                return result;
        }
        return "No missiles ready.";
    }
    else if (args.Length == 2)
    {
        int n;
        if (TryParseInt(args[1], out n))
        {
            if (missileGrids.Count > 0) return CreateFiringCommandForSpecificMissile(missileGrids[0].Name, n);
            return "No grids configured.";
        }
        return CreateFiringCommandForFirstAvailableMissile(args[1]);
    }
    else if (args.Length == 3)
    {
        int n;
        if (TryParseInt(args[2], out n)) return CreateFiringCommandForSpecificMissile(args[1], n);
        return "Invalid missile number.";
    }
    return "Use: fire [Grid] [Number]";
}

string HandleSalvoCommand(string[] args)
{
    if (args.Length == 2)
    {
        int n; if (TryParseInt(args[1], out n)) return CreateFiringCommandForMultipleMissiles(null, n);
        return "Invalid number.";
    }
    else if (args.Length == 3)
    {
        int n; if (TryParseInt(args[2], out n)) return CreateFiringCommandForMultipleMissiles(args[1], n);
        return "Invalid number.";
    }
    return "Use: salvo [Grid] Number";
}

string HandleDeleteCommand(string[] args)
{
    if (args.Length == 1)
    {
        firingCommandQueues.Clear(); CloseAllDoors();
        Log(LogLevel.Info, "All commands deleted."); return "Commands deleted.";
    }
    else if (args.Length == 2)
    {
        if (firingCommandQueues.ContainsKey(args[1]))
        {
            firingCommandQueues.Remove(args[1]); CloseDoorsForGrid(args[1]);
            return $"Commands for '{args[1]}' deleted.";
        }
        return $"No commands for '{args[1]}'.";
    }
    return "Use: deletecommand [Grid]";
}

string HandleSetSubsystemCommand(string[] args)
{
    if (!EnableTargetingSettings) return "Targeting disabled.";
    if (args.Length != 2) return "Use: TargetSubsystem Weapons|Propulsion|Energy";
    TargetSubsystem s;
    if (Enum.TryParse(args[1], true, out s)) { currentTargetSubsystem = s; return $"Subsystem: {s}"; }
    return "Invalid. Use: Weapons, Propulsion, Energy";
}

string HandleSetPriorityCommand(string[] args)
{
    if (!EnableTargetingSettings) return "Targeting disabled.";
    if (args.Length != 2) return "Use: TargetPriority Closest|Largest|Smallest";
    TargetPriority p;
    if (Enum.TryParse(args[1], true, out p)) { currentTargetPriority = p; return $"Priority: {p}"; }
    return "Invalid. Use: Closest, Largest, Smallest";
}

string HandlePageCommand(string[] args)
{
    if (args.Length < 2) return "Use: page next|prev|[Name]";
    string action = args[1];
    string lcdFilter = args.Length >= 3 ? string.Join(" ", args.Skip(2)) : null;

    var candidates = new List<IMyTerminalBlock>();
    foreach (var l in cachedLCDs) candidates.Add(l);
    foreach (var c in cachedCockpits) candidates.Add(c);
    if (candidates.Count == 0) return $"No displays with tag '{LCDTag}'.";

    int changed = 0, skipped = 0;
    foreach (var b in candidates)
    {
        if (!string.IsNullOrEmpty(lcdFilter) && !b.CustomName.Contains(lcdFilter)) continue;
        var cfg = GetLCDConfig(b);
        if (cfg.Locked || cfg.Pages.Count <= 1) { skipped++; continue; }

        int idx = cfg.Pages.FindIndex(p => p.Equals(cfg.CurrentPage, StringComparison.OrdinalIgnoreCase));
        if (idx < 0) idx = 0;

        string newPage = null;
        if (action.Equals("next", StringComparison.OrdinalIgnoreCase))
            newPage = cfg.Pages[(idx + 1) % cfg.Pages.Count];
        else if (action.Equals("prev", StringComparison.OrdinalIgnoreCase))
            newPage = cfg.Pages[(idx - 1 + cfg.Pages.Count) % cfg.Pages.Count];
        else
        {
            int found = cfg.Pages.FindIndex(p => p.Equals(action, StringComparison.OrdinalIgnoreCase));
            if (found >= 0) newPage = cfg.Pages[found];
        }
        if (newPage == null) continue;
        SetLCDPage(b, newPage);
        changed++;
    }

    if (changed == 0) return skipped > 0 ? $"No navigable displays ({skipped} locked)." : "No matching displays.";
    return $"Page changed ({changed}).";
}
#endregion

#region CustomData Parsing
void ParseCustomData()
{
    if (string.IsNullOrWhiteSpace(Me.CustomData))
    {
        Me.CustomData = GetDefaultCustomData();
        Echo("Default config created.");
    }

    missileGrids.Clear();
    MinimumHydrogenLevel = 0.9; MissileFireDelay = 0.5; LCDTag = "MFC";
    CommandTimeout = 8.0; DoorClosingDelay = 1.0; EnableTargetingSettings = false;
    AutoFireMinRange = 200; AutoFireMaxRange = 2000; AutoFireSalvoSize = 1;
    AutoFireInterval = 3.0; AutoFireRequireClosing = true; AutoFireMinLockTime = 1.5;
    TurretTag = ""; AIBlockTag = "";
    TargetSubsystemTimerNames.Clear(); TargetPriorityTimerNames.Clear();

    var lines = Me.CustomData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    MissileGrid currentGrid = null;
    string section = "";

    foreach (var line in lines)
    {
        var t = line.Trim();
        if (t.StartsWith(";") || t == "") continue;

        if (t.StartsWith("[") && t.EndsWith("]"))
        {
            section = t.Substring(1, t.Length - 2);
            if (section != "Settings" && section != "AutoFire" &&
                section != "Targeting" && section != "TargetingTimers")
            {
                currentGrid = new MissileGrid { Name = section };
                missileGrids.Add(currentGrid);
            }
            continue;
        }

        var parts = t.Split('=');
        if (parts.Length != 2) continue;
        var key = parts[0].Trim();
        var val = parts[1].Trim();

        switch (section)
        {
            case "Settings":
                if (key == "MinimumHydrogenLevel") TryParseDouble(val, out MinimumHydrogenLevel);
                else if (key == "MissileFireDelay") TryParseDouble(val, out MissileFireDelay);
                else if (key == "LCDTag") LCDTag = val;
                else if (key == "CommandTimeout") TryParseDouble(val, out CommandTimeout);
                else if (key == "DoorClosingDelay") TryParseDouble(val, out DoorClosingDelay);
                break;
            case "AutoFire":
                if (key == "Enabled") bool.TryParse(val, out AutoFireEnabled);
                else if (key == "MinRange") TryParseDouble(val, out AutoFireMinRange);
                else if (key == "MaxRange") TryParseDouble(val, out AutoFireMaxRange);
                else if (key == "SalvoSize") TryParseInt(val, out AutoFireSalvoSize);
                else if (key == "IntervalSeconds") TryParseDouble(val, out AutoFireInterval);
                else if (key == "RequireClosing") bool.TryParse(val, out AutoFireRequireClosing);
                else if (key == "MinLockTime") TryParseDouble(val, out AutoFireMinLockTime);
                break;
            case "Targeting":
                if (key == "Enabled") bool.TryParse(val, out EnableTargetingSettings);
                else if (key == "TurretTag") TurretTag = val;
                else if (key == "AIBlockTag") AIBlockTag = val;
                break;
            case "TargetingTimers":
                if (key == "Weapons") TargetSubsystemTimerNames[TargetSubsystem.Weapons] = val;
                else if (key == "Propulsion") TargetSubsystemTimerNames[TargetSubsystem.Propulsion] = val;
                else if (key == "Energy") TargetSubsystemTimerNames[TargetSubsystem.Energy] = val;
                else if (key == "Closest") TargetPriorityTimerNames[TargetPriority.Closest] = val;
                else if (key == "Smallest") TargetPriorityTimerNames[TargetPriority.Smallest] = val;
                else if (key == "Largest") TargetPriorityTimerNames[TargetPriority.Largest] = val;
                break;
            default:
                if (currentGrid != null)
                {
                    if (key == "Doors")
                        currentGrid.DoorNames.AddRange(val.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0));
                    else if (key == "Timers")
                        currentGrid.TimerBlockNames.AddRange(val.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0));
                }
                break;
        }
    }
}

string GetDefaultCustomData()
{
    return @"; ═══ MFC v2.0 Configuration ═══

; ── Missile Grids ──
; Each [SectionName] that isn't a reserved name becomes a missile grid.
; Define doors and timer blocks per grid.

[Grid1]
Doors=VLS Door 1, VLS Door 2
Timers=TB - Missile 1 - Fire, TB - Missile 2 - Fire

[Grid2]
Doors=VLS Door 3, VLS Door 4
Timers=TB - Missile 3 - Fire, TB - Missile 4 - Fire

; ── General Settings ──
[Settings]
MinimumHydrogenLevel=0.9
MissileFireDelay=0.5
LCDTag=MFC
CommandTimeout=8.0
DoorClosingDelay=1.0

; ── Auto-Fire ──
[AutoFire]
Enabled=false
MinRange=200
MaxRange=2000
SalvoSize=1
IntervalSeconds=3.0
RequireClosing=true
; Seconds a target must be continuously locked before auto-fire triggers.
; Prevents wasting missiles on brief/flickering turret locks.
MinLockTime=1.5

; ── Target Tracking ──
; Tags to filter which turrets/AI blocks are used for tracking.
; Leave empty to use ALL turrets/AI blocks on this grid.
[Targeting]
Enabled=false
TurretTag=
AIBlockTag=

; ── Targeting Timers ──
; Timer blocks triggered when firing to set subsystem/priority.
; Leave empty if not used.
[TargetingTimers]
Weapons=
Propulsion=
Energy=
Closest=
Smallest=
Largest=
";
}
#endregion

#region Firing Commands
double CalculateDynamicTimeout(int missileCount)
{
    return CommandTimeout + (missileCount * MissileFireDelay) + DoorClosingDelay + 2.0;
}

string CreateFiringCommandForMultipleMissiles(string gridName, int count)
{
    if (count <= 0) return "Count must be > 0.";

    var tasksPerGrid = new Dictionary<MissileGrid, List<MissileTask>>();
    int needed = count;
    IEnumerable<MissileGrid> grids = gridName != null
        ? missileGrids.Where(g => g.Name.Equals(gridName, StringComparison.OrdinalIgnoreCase))
        : missileGrids;

    if (gridName != null && !grids.Any())
        return $"Grid '{gridName}' not found.";

    foreach (var grid in grids)
    {
        if (needed == 0) break;
        var tasks = new List<MissileTask>();
        for (int i = 0; i < grid.TimerBlockNames.Count && needed > 0; i++)
        {
            var tb = GetTimerBlockByName(grid.TimerBlockNames[i]);
            if (tb != null && IsMissileReady(tb))
            {
                tasks.Add(new MissileTask { Grid = grid, MissileIndex = i, TimerBlock = tb, Doors = GetDoorsByNames(grid.DoorNames) });
                needed--;
            }
        }
        if (tasks.Count > 0) tasksPerGrid.Add(grid, tasks);
    }

    if (tasksPerGrid.Count == 0) return "No missiles ready.";

    foreach (var kvp in tasksPerGrid)
    {
        var fc = new FiringCommand
        {
            doorState = DoorState.OpeningDoors,
            commandDescription = $"Firing {kvp.Value.Count} from '{kvp.Key.Name}'",
            commandTimeout = CalculateDynamicTimeout(kvp.Value.Count),
            doorClosingDelay = DoorClosingDelay
        };
        fc.missileTasks.AddRange(kvp.Value);
        if (!firingCommandQueues.ContainsKey(kvp.Key.Name)) firingCommandQueues[kvp.Key.Name] = new Queue<FiringCommand>();
        firingCommandQueues[kvp.Key.Name].Enqueue(fc);
    }

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    string suffix = gridName != null ? $" from '{gridName}'" : "";
    return $"Firing {count} missile(s){suffix}...";
}

string CreateFiringCommandForSpecificMissile(string gridName, int missileNumber)
{
    var grid = missileGrids.FirstOrDefault(g => g.Name.Equals(gridName, StringComparison.OrdinalIgnoreCase));
    if (grid == null) return $"Grid '{gridName}' not found.";
    if (missileNumber < 1 || missileNumber > grid.TimerBlockNames.Count) return "Invalid missile number.";

    int idx = missileNumber - 1;
    var tb = GetTimerBlockByName(grid.TimerBlockNames[idx]);
    if (tb == null) return $"Timer for M{missileNumber} ({gridName}) missing.";
    if (!IsMissileReady(tb)) return $"M{missileNumber} in '{gridName}' not ready.";

    var fc = new FiringCommand
    {
        doorState = DoorState.OpeningDoors,
        commandDescription = $"M{missileNumber} from '{gridName}'",
        commandTimeout = CalculateDynamicTimeout(1),
        doorClosingDelay = DoorClosingDelay
    };
    fc.missileTasks.Add(new MissileTask { Grid = grid, MissileIndex = idx, TimerBlock = tb, Doors = GetDoorsByNames(grid.DoorNames) });
    if (!firingCommandQueues.ContainsKey(grid.Name)) firingCommandQueues[grid.Name] = new Queue<FiringCommand>();
    firingCommandQueues[grid.Name].Enqueue(fc);
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    return fc.commandDescription;
}

string CreateFiringCommandForFirstAvailableMissile(string gridName)
{
    var grid = missileGrids.FirstOrDefault(g => g.Name.Equals(gridName, StringComparison.OrdinalIgnoreCase));
    if (grid == null) return $"Grid '{gridName}' not found.";

    for (int i = 0; i < grid.TimerBlockNames.Count; i++)
    {
        var tb = GetTimerBlockByName(grid.TimerBlockNames[i]);
        if (tb != null && IsMissileReady(tb))
        {
            var fc = new FiringCommand
            {
                doorState = DoorState.OpeningDoors,
                commandDescription = $"M{i + 1} from '{gridName}'",
                commandTimeout = CalculateDynamicTimeout(1),
                doorClosingDelay = DoorClosingDelay
            };
            fc.missileTasks.Add(new MissileTask { Grid = grid, MissileIndex = i, TimerBlock = tb, Doors = GetDoorsByNames(grid.DoorNames) });
            if (!firingCommandQueues.ContainsKey(grid.Name)) firingCommandQueues[grid.Name] = new Queue<FiringCommand>();
            firingCommandQueues[grid.Name].Enqueue(fc);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            return fc.commandDescription;
        }
    }
    return $"No missiles ready in '{gridName}'.";
}
#endregion

#region Utilities
IMyTimerBlock GetTimerBlockByName(string name) => GridTerminalSystem.GetBlockWithName(name) as IMyTimerBlock;

bool IsMissileReady(IMyTimerBlock tb) =>
    tb != null && tb.IsFunctional && tb.Enabled && IsHydrogenLevelSufficient(tb);

bool IsHydrogenLevelSufficient(IMyTimerBlock tb)
{
    if (tb == null || tb.CubeGrid == null) return false;
    var tanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType(tanks,
        t => t != null && t.CubeGrid == tb.CubeGrid && t.BlockDefinition.SubtypeId.ToLower().Contains("hydrogen"));
    if (tanks.Count == 0) return true;
    foreach (var tank in tanks) if (tank.FilledRatio < MinimumHydrogenLevel) return false;
    return true;
}

List<IMyDoor> GetDoorsByNames(List<string> names)
{
    var doors = new List<IMyDoor>();
    foreach (var name in names)
    {
        var d = GridTerminalSystem.GetBlockWithName(name) as IMyDoor;
        if (d != null) doors.Add(d);
    }
    return doors;
}

int GetReadyMissileCount(MissileGrid grid)
{
    int c = 0;
    foreach (var n in grid.TimerBlockNames) if (IsMissileReady(GetTimerBlockByName(n))) c++;
    return c;
}
#endregion

#region State Machine
void UpdateState()
{
    if (firingCommandQueues.Count == 0) { currentSystemActivity = ""; return; }

    foreach (var kv in firingCommandQueues.ToList())
    {
        var queue = kv.Value;
        if (queue.Count == 0) continue;
        var fc = queue.Peek();
        UpdateFiringCommand(fc);

        if (fc.doorState == DoorState.Idle || fc.isAborted)
        {
            queue.Dequeue();
            if (fc.isAborted)
            {
                lastCommandResult = "Command timed out.";
                Log(LogLevel.Warn, $"Timeout: {fc.commandDescription}");
                CloseDoors(fc);
            }
            if (queue.Count == 0) firingCommandQueues.Remove(kv.Key);
            else { var nx = queue.Peek(); nx.doorState = DoorState.OpeningDoors; nx.elapsedTime = 0; }
        }
    }
}

void UpdateFiringCommand(FiringCommand fc)
{
    fc.commandElapsedTime += Runtime.TimeSinceLastRun.TotalSeconds;
    if (fc.commandElapsedTime >= fc.commandTimeout) { fc.isAborted = true; return; }

    switch (fc.doorState)
    {
        case DoorState.OpeningDoors:
            if (EnableTargetingSettings && !fc.targetingTimersTriggered)
            {
                TriggerTargetingTimers(); fc.targetingTimersTriggered = true;
            }
            var doors = new HashSet<IMyDoor>();
            foreach (var t in fc.missileTasks) foreach (var d in t.Doors) doors.Add(d);
            bool allOpen = true;
            foreach (var door in doors)
            {
                if (door.Status == DoorStatus.Closed || door.Status == DoorStatus.Closing) door.OpenDoor();
                if (door.Status != DoorStatus.Open) allOpen = false;
            }
            if (allOpen) { fc.doorState = DoorState.FiringMissiles; fc.elapsedTime = 0; currentSystemActivity = "Doors open."; }
            else currentSystemActivity = "Opening doors...";
            break;

        case DoorState.FiringMissiles:
            if (fc.currentTaskIndex >= fc.missileTasks.Count)
            {
                fc.doorState = DoorState.WaitingToCloseDoors; fc.doorDelayElapsedTime = 0; return;
            }
            fc.elapsedTime += Runtime.TimeSinceLastRun.TotalSeconds;
            if (fc.elapsedTime >= MissileFireDelay)
            {
                var task = fc.missileTasks[fc.currentTaskIndex];
                task.TimerBlock.Trigger();
                string msg = $"M{task.MissileIndex + 1} fired ({task.Grid.Name})";
                lastCommandResult = msg;
                Log(LogLevel.Fire, msg);
                task.Grid.TotalFired++;
                totalFiredAllTime++;
                fc.currentTaskIndex++;
                fc.elapsedTime = 0;
            }
            break;

        case DoorState.WaitingToCloseDoors:
            fc.doorDelayElapsedTime += Runtime.TimeSinceLastRun.TotalSeconds;
            if (fc.doorDelayElapsedTime >= fc.doorClosingDelay) { CloseDoors(fc); fc.doorState = DoorState.Idle; }
            else currentSystemActivity = "Closing doors...";
            break;
    }
}

void TriggerTargetingTimers()
{
    string n;
    if (TargetSubsystemTimerNames.TryGetValue(currentTargetSubsystem, out n) && !string.IsNullOrEmpty(n))
    {
        var t = GridTerminalSystem.GetBlockWithName(n) as IMyTimerBlock;
        if (t != null) t.Trigger();
    }
    if (TargetPriorityTimerNames.TryGetValue(currentTargetPriority, out n) && !string.IsNullOrEmpty(n))
    {
        var t = GridTerminalSystem.GetBlockWithName(n) as IMyTimerBlock;
        if (t != null) t.Trigger();
    }
}

void CloseDoors(FiringCommand fc)
{
    var doors = new HashSet<IMyDoor>();
    foreach (var t in fc.missileTasks) foreach (var d in t.Doors) doors.Add(d);
    foreach (var door in doors)
        if (door.Status == DoorStatus.Open || door.Status == DoorStatus.Opening) door.CloseDoor();
    currentSystemActivity = "Closing doors...";
}

void CloseAllDoors()
{
    foreach (var grid in missileGrids)
        foreach (var door in GetDoorsByNames(grid.DoorNames))
            if (door.Status == DoorStatus.Open || door.Status == DoorStatus.Opening) door.CloseDoor();
}

void CloseDoorsForGrid(string gridName)
{
    var grid = missileGrids.FirstOrDefault(g => g.Name.Equals(gridName, StringComparison.OrdinalIgnoreCase));
    if (grid == null) return;
    foreach (var door in GetDoorsByNames(grid.DoorNames))
        if (door.Status == DoorStatus.Open || door.Status == DoorStatus.Opening) door.CloseDoor();
}
#endregion

#region LCD Config
class LCDConfig
{
    public List<string> Pages = new List<string> { "Overview" };
    public string CurrentPage = "Overview";
    public string GridName = "";
    public int ScreenIndex = 1;
    public float FontSize = 0.8f;
    public bool Locked = false;
}

LCDConfig GetLCDConfig(IMyTerminalBlock block)
{
    var config = new LCDConfig();
    if (string.IsNullOrWhiteSpace(block.CustomData))
        block.CustomData = GetDefaultLCDCustomData();

    var lines = block.CustomData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    string sec = "";
    foreach (var line in lines)
    {
        var t = line.Trim();
        if (t.StartsWith(";") || t == "") continue;
        if (t.StartsWith("[") && t.EndsWith("]")) { sec = t.Substring(1, t.Length - 2); continue; }
        if (sec != "LCD") continue;
        var parts = t.Split('=');
        if (parts.Length != 2) continue;
        var key = parts[0].Trim();
        var val = parts[1].Trim();

        if (key == "Pages") config.Pages = val.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
        else if (key == "CurrentPage") config.CurrentPage = val;
        else if (key == "GridName") config.GridName = val;
        else if (key == "ScreenIndex") TryParseInt(val, out config.ScreenIndex);
        else if (key == "FontSize") TryParseFloat(val, out config.FontSize);
        else if (key == "Locked") bool.TryParse(val, out config.Locked);
    }
    if (config.Pages.Count == 0) config.Pages.Add("Overview");
    if (!config.Pages.Contains(config.CurrentPage)) config.CurrentPage = config.Pages[0];
    return config;
}

void SetLCDPage(IMyTerminalBlock block, string page)
{
    var lines = block.CustomData.Split('\n').ToList();
    bool inLCD = false, done = false;
    for (int i = 0; i < lines.Count; i++)
    {
        var t = lines[i].Trim();
        if (t.StartsWith("[") && t.EndsWith("]")) inLCD = t == "[LCD]";
        if (inLCD && t.StartsWith("CurrentPage=")) { lines[i] = "CurrentPage=" + page; done = true; break; }
    }
    if (!done)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Trim() == "[LCD]") { lines.Insert(i + 1, "CurrentPage=" + page); done = true; break; }
        }
    }
    if (!done) { lines.Add("[LCD]"); lines.Add("CurrentPage=" + page); }
    block.CustomData = string.Join("\n", lines);
}

string GetDefaultLCDCustomData()
{
    return @"[LCD]
; Pages: Overview, Grids, Targeting, Diagnostics, Log
Pages=Overview,Grids,Targeting,Diagnostics,Log
CurrentPage=Overview
; Locked=true prevents 'page next/prev' from changing this display
Locked=false
; ScreenIndex for cockpits (1-based)
ScreenIndex=1
FontSize=0.8
";
}
#endregion

#region Display Rendering
void UpdateDisplays()
{
    foreach (var lcd in cachedLCDs)
    {
        var cfg = GetLCDConfig(lcd);
        RenderSprite(lcd, cfg);
    }
    foreach (var cockpit in cachedCockpits)
    {
        var cfg = GetLCDConfig(cockpit);
        int idx = cfg.ScreenIndex - 1;
        if (idx >= 0 && idx < cockpit.SurfaceCount)
            RenderSprite(cockpit.GetSurface(idx), cfg);
    }
}

void RenderSprite(IMyTextSurface surface, LCDConfig cfg)
{
    surface.ContentType = ContentType.SCRIPT;
    surface.Script = "";
    surface.ScriptBackgroundColor = COL_BG;

    Vector2 offset = (surface.TextureSize - surface.SurfaceSize) / 2f;
    var vp = new RectangleF(offset.X, offset.Y, surface.SurfaceSize.X, surface.SurfaceSize.Y);

    var frame = surface.DrawFrame();
    try
    {
        DrawRect(ref frame, vp, COL_BG);
        DrawHeader(ref frame, vp, cfg);
        DrawNavBar(ref frame, vp, cfg);

        var body = new RectangleF(vp.X + PAD, vp.Y + HDR_H, vp.Width - PAD * 2, vp.Height - HDR_H - NAV_H);
        if (body.Width < 20 || body.Height < 20) return;

        switch (cfg.CurrentPage)
        {
            case "Overview":    PageOverview(ref frame, body); break;
            case "Grids":       PageGrids(ref frame, body); break;
            case "Targeting":   PageTargeting(ref frame, body); break;
            case "Diagnostics": PageDiagnostics(ref frame, body); break;
            case "Log":         PageLog(ref frame, body); break;
        }
    }
    finally { frame.Dispose(); }
}

// ───── Colors ─────
static readonly Color COL_BG    = new Color(0, 0, 0);
static readonly Color COL_BORDER = new Color(30, 40, 55);
static readonly Color COL_ACCENT = new Color(100, 160, 200);
static readonly Color COL_GREEN  = new Color(0, 80, 0);
static readonly Color COL_YELLOW = new Color(80, 70, 0);
static readonly Color COL_RED    = new Color(80, 0, 0);
static readonly Color COL_EMPTY  = new Color(22, 22, 22);
static readonly Color COL_TEXT   = new Color(150, 150, 150);
static readonly Color COL_DIM    = new Color(75, 75, 75);
static readonly Color COL_WHITE  = new Color(200, 200, 200);

const float PAD = 8f, HDR_H = 26f, NAV_H = 18f;

// ───── Primitives ─────
void DrawRect(ref MySpriteDrawFrame f, RectangleF r, Color c) =>
    f.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
        new Vector2(r.X + r.Width / 2f, r.Y + r.Height / 2f), new Vector2(r.Width, r.Height), c));

void DrawText(ref MySpriteDrawFrame f, string text, Vector2 pos, float scale, Color c, TextAlignment align = TextAlignment.LEFT) =>
    f.Add(new MySprite { Type = SpriteType.TEXT, Data = text ?? "", Position = pos,
        RotationOrScale = scale, Color = c, Alignment = align, FontId = "White" });

void DrawCenteredText(ref MySpriteDrawFrame f, RectangleF r, string text, float scale, Color c) =>
    DrawText(ref f, text, new Vector2(r.X + r.Width / 2f, r.Y + r.Height / 2f - 14f * scale), scale, c, TextAlignment.CENTER);

void DrawMissile(ref MySpriteDrawFrame f, Vector2 center, float size, Color color) =>
    f.Add(new MySprite(SpriteType.TEXTURE, "Triangle", center, new Vector2(size * 0.65f, size * 0.9f), color));

void DrawSep(ref MySpriteDrawFrame f, float x, float y, float w) =>
    DrawRect(ref f, new RectangleF(x, y, w, 1), COL_BORDER);

void DrawKV(ref MySpriteDrawFrame f, RectangleF a, ref float y, string label, string value, Color vc)
{
    DrawText(ref f, label, new Vector2(a.X, y), 0.4f, COL_DIM);
    DrawText(ref f, value, new Vector2(a.X + a.Width, y), 0.5f, vc, TextAlignment.RIGHT);
    y += 16;
}

string Trunc(string s, int n) => s == null ? "" : s.Length <= n ? s : s.Substring(0, n - 1) + "~";

// ───── Header ─────
void DrawHeader(ref MySpriteDrawFrame f, RectangleF vp, LCDConfig cfg)
{
    DrawRect(ref f, new RectangleF(vp.X, vp.Y + HDR_H - 1, vp.Width, 1), COL_BORDER);
    DrawText(ref f, cfg.CurrentPage.ToUpper(), new Vector2(vp.X + PAD, vp.Y + 4), 0.7f, COL_ACCENT);
    float rx = vp.X + vp.Width - PAD;
    if (isLocked) { DrawText(ref f, "LOCK", new Vector2(rx, vp.Y + 6), 0.5f, COL_RED, TextAlignment.RIGHT); rx -= 44; }
    if (holdFire) { DrawText(ref f, "HOLD", new Vector2(rx, vp.Y + 6), 0.5f, COL_YELLOW, TextAlignment.RIGHT); rx -= 44; }
    if (AutoFireEnabled) { DrawText(ref f, "AUTO", new Vector2(rx, vp.Y + 6), 0.5f, COL_GREEN, TextAlignment.RIGHT); }
}

// ───── Nav Bar ─────
void DrawNavBar(ref MySpriteDrawFrame f, RectangleF vp, LCDConfig cfg)
{
    float y = vp.Y + vp.Height - NAV_H;
    DrawRect(ref f, new RectangleF(vp.X, y, vp.Width, 1), COL_BORDER);
    if (cfg.Pages.Count <= 1) return;
    int active = cfg.Pages.FindIndex(p => p.Equals(cfg.CurrentPage, StringComparison.OrdinalIgnoreCase));
    if (active < 0) active = 0;
    float dotGap = 10f, totalW = (cfg.Pages.Count - 1) * dotGap;
    float sx = vp.X + vp.Width / 2f - totalW / 2f, dy = y + NAV_H / 2f;
    for (int i = 0; i < cfg.Pages.Count; i++)
        f.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(sx + i * dotGap, dy), new Vector2(4, 4), i == active ? COL_ACCENT : COL_DIM));
}

// ───── Overview ─────
void PageOverview(ref MySpriteDrawFrame f, RectangleF a)
{
    float y = a.Y + 4;
    int totalReady = missileGrids.Sum(g => GetReadyMissileCount(g));
    int totalCap = missileGrids.Sum(g => g.TimerBlockNames.Count);
    DrawText(ref f, $"{totalReady}/{totalCap} READY", new Vector2(a.X, y), 1.0f, COL_WHITE);
    y += 32;

    if (!string.IsNullOrEmpty(lastCommandResult))
    { DrawText(ref f, Trunc(lastCommandResult, 40), new Vector2(a.X, y), 0.45f, COL_TEXT); y += 14; }
    if (!string.IsNullOrEmpty(currentSystemActivity))
    { DrawText(ref f, "> " + Trunc(currentSystemActivity, 38), new Vector2(a.X, y), 0.4f, COL_DIM); y += 14; }

    if (EnableTargetingSettings)
    {
        y += 4; DrawSep(ref f, a.X, y, a.Width); y += 6;
        DrawText(ref f, $"TGT {currentTargetSubsystem.ToString().ToUpper()}  //  {currentTargetPriority.ToString().ToUpper()}",
            new Vector2(a.X, y), 0.55f, COL_ACCENT);
        y += 20;
    }

    y += 2; DrawSep(ref f, a.X, y, a.Width); y += 6;

    float mSize = 18f, mGap = 4f;
    foreach (var g in missileGrids)
    {
        if (y > a.Y + a.Height - mSize - 14) break;
        int ready = GetReadyMissileCount(g);
        DrawText(ref f, g.Name, new Vector2(a.X, y), 0.45f, COL_TEXT);
        DrawText(ref f, $"{ready}/{g.TimerBlockNames.Count}", new Vector2(a.X + a.Width, y), 0.45f, COL_DIM, TextAlignment.RIGHT);
        y += 14;
        for (int i = 0; i < g.TimerBlockNames.Count; i++)
        {
            float mx = a.X + i * (mSize + mGap) + mSize / 2f;
            if (mx + mSize / 2f > a.X + a.Width) break;
            var tb = GetTimerBlockByName(g.TimerBlockNames[i]);
            Color c = tb == null || !tb.IsFunctional ? COL_RED : !IsHydrogenLevelSufficient(tb) ? COL_YELLOW : !tb.Enabled ? COL_EMPTY : COL_GREEN;
            DrawMissile(ref f, new Vector2(mx, y + mSize / 2f), mSize, c);
        }
        y += mSize + 6;
    }
}

// ───── Grids ─────
void PageGrids(ref MySpriteDrawFrame f, RectangleF a)
{
    float y = a.Y + 2;
    float iconSize = Math.Min(36f, Math.Max(22f, a.Width / 9f));
    float gap = 4f, rowH = iconSize * 0.35f;

    foreach (var g in missileGrids)
    {
        if (y > a.Y + a.Height - iconSize - 20) break;
        int ready = GetReadyMissileCount(g);
        if (y > a.Y + 6) DrawSep(ref f, a.X, y - 2, a.Width);
        DrawText(ref f, g.Name, new Vector2(a.X, y), 0.55f, COL_ACCENT);
        DrawText(ref f, $"{ready}/{g.TimerBlockNames.Count}", new Vector2(a.X + a.Width, y), 0.5f, COL_TEXT, TextAlignment.RIGHT);
        y += 18;

        int perRow = Math.Max(1, (int)(a.Width / (iconSize + gap)));
        for (int i = 0; i < g.TimerBlockNames.Count; i++)
        {
            int row = i / perRow, col = i % perRow;
            float cx = a.X + col * (iconSize + gap) + iconSize / 2f;
            float cy = y + row * (iconSize + rowH) + iconSize / 2f;
            if (cy + iconSize > a.Y + a.Height) break;

            var tb = GetTimerBlockByName(g.TimerBlockNames[i]);
            Color c = tb == null || !tb.IsFunctional ? COL_RED : !IsHydrogenLevelSufficient(tb) ? COL_YELLOW : !tb.Enabled ? COL_EMPTY : COL_GREEN;
            DrawMissile(ref f, new Vector2(cx, cy), iconSize, c);
            DrawText(ref f, (i + 1).ToString(), new Vector2(cx, cy + iconSize * 0.3f), 0.35f, COL_TEXT, TextAlignment.CENTER);
        }
        y += (int)Math.Ceiling((double)g.TimerBlockNames.Count / perRow) * (iconSize + rowH) + 8;
    }
}

// ───── Targeting ─────
void PageTargeting(ref MySpriteDrawFrame f, RectangleF a)
{
    float y = a.Y + 4;
    if (EnableTargetingSettings)
    {
        DrawKV(ref f, a, ref y, "SUBSYSTEM", currentTargetSubsystem.ToString().ToUpper(), COL_ACCENT);
        DrawKV(ref f, a, ref y, "PRIORITY", currentTargetPriority.ToString().ToUpper(), COL_ACCENT);
        DrawSep(ref f, a.X, y, a.Width); y += 8;
    }

    DrawText(ref f, "TARGET", new Vector2(a.X, y), 0.55f, COL_ACCENT); y += 20;
    if (!currentTarget.HasTarget)
    {
        DrawText(ref f, "NO LOCK", new Vector2(a.X, y), 0.7f, COL_DIM); y += 30;
        DrawSep(ref f, a.X, y, a.Width); y += 8;
        DrawKV(ref f, a, ref y, "AUTOFIRE", AutoFireEnabled ? "ENABLED" : "OFF", AutoFireEnabled ? COL_GREEN : COL_DIM);
        DrawKV(ref f, a, ref y, "RANGE", $"{AutoFireMinRange:F0}-{AutoFireMaxRange:F0}m", COL_DIM);
        return;
    }

    DrawText(ref f, Trunc(currentTarget.Name, 24), new Vector2(a.X, y), 0.6f, COL_WHITE); y += 20;
    DrawText(ref f, currentTarget.TypeStr, new Vector2(a.X, y), 0.4f, COL_DIM); y += 18;
    DrawSep(ref f, a.X, y, a.Width); y += 8;
    DrawKV(ref f, a, ref y, "RANGE", $"{currentTarget.Range:F0} m",
        currentTarget.Range >= AutoFireMinRange && currentTarget.Range <= AutoFireMaxRange ? COL_GREEN : COL_TEXT);
    DrawKV(ref f, a, ref y, "SPEED", $"{currentTarget.RelativeSpeed:F0} m/s", COL_TEXT);
    DrawKV(ref f, a, ref y, "STATUS", currentTarget.Closing ? "CLOSING" : "SEPARATING",
        currentTarget.Closing ? new Color(120, 50, 50) : COL_DIM);
    y += 6; DrawSep(ref f, a.X, y, a.Width); y += 8;
    DrawKV(ref f, a, ref y, "AUTOFIRE", AutoFireEnabled ? "ENABLED" : "OFF", AutoFireEnabled ? COL_GREEN : COL_DIM);
    if (autoFireCooldownRemaining > 0) DrawKV(ref f, a, ref y, "COOLDOWN", $"{autoFireCooldownRemaining:F1}s", COL_DIM);
}

// ───── Diagnostics ─────
void PageDiagnostics(ref MySpriteDrawFrame f, RectangleF a)
{
    float y = a.Y + 4;
    DrawKV(ref f, a, ref y, "VERSION", "MFC v" + SCRIPT_VERSION, COL_ACCENT);
    DrawKV(ref f, a, ref y, "UPTIME", FormatTimestamp(runtimeClock), COL_TEXT);
    DrawKV(ref f, a, ref y, "FIRED", totalFiredAllTime.ToString(), COL_TEXT);
    y += 4; DrawSep(ref f, a.X, y, a.Width); y += 6;
    DrawKV(ref f, a, ref y, "AUTOFIRE", AutoFireEnabled ? "ON" : "OFF", AutoFireEnabled ? COL_GREEN : COL_DIM);
    DrawKV(ref f, a, ref y, "HOLDFIRE", holdFire ? "ON" : "OFF", holdFire ? COL_YELLOW : COL_DIM);
    DrawKV(ref f, a, ref y, "AF RANGE", $"{AutoFireMinRange:F0}-{AutoFireMaxRange:F0}m", COL_TEXT);
    DrawKV(ref f, a, ref y, "AF COOLDOWN", autoFireCooldownRemaining > 0 ? $"{autoFireCooldownRemaining:F1}s" : "READY", COL_TEXT);
    DrawKV(ref f, a, ref y, "MIN LOCK", $"{AutoFireMinLockTime:F1}s", COL_TEXT);

    if (EnableTargetingSettings)
    {
        y += 4; DrawSep(ref f, a.X, y, a.Width); y += 6;
        DrawKV(ref f, a, ref y, "TGT SUBSYS", currentTargetSubsystem.ToString().ToUpper(), COL_ACCENT);
        DrawKV(ref f, a, ref y, "TGT PRIO", currentTargetPriority.ToString().ToUpper(), COL_ACCENT);
    }

    y += 4; DrawSep(ref f, a.X, y, a.Width); y += 6;
    DrawText(ref f, lastValidation.Ok ? "CONFIG VALID" : $"CONFIG: {lastValidation.Errors.Count} ERROR(S)",
        new Vector2(a.X, y), 0.5f, lastValidation.Ok ? COL_GREEN : COL_RED);
    y += 16;
    foreach (var e in lastValidation.Errors.Take(4))
    { if (y > a.Y + a.Height - 12) break; DrawText(ref f, "! " + Trunc(e, 40), new Vector2(a.X, y), 0.38f, COL_RED); y += 12; }
    foreach (var w in lastValidation.Warnings.Take(3))
    { if (y > a.Y + a.Height - 12) break; DrawText(ref f, "- " + Trunc(w, 40), new Vector2(a.X, y), 0.38f, COL_YELLOW); y += 12; }

    // Per-grid fired stats
    y += 6; DrawSep(ref f, a.X, y, a.Width); y += 6;
    foreach (var g in missileGrids)
    {
        if (y > a.Y + a.Height - 12) break;
        DrawKV(ref f, a, ref y, g.Name, $"{g.TotalFired} fired", COL_DIM);
    }
}

// ───── Log ─────
void PageLog(ref MySpriteDrawFrame f, RectangleF a)
{
    float lineH = 12f;
    int maxLines = (int)(a.Height / lineH);
    if (eventLog.Count == 0) { DrawCenteredText(ref f, a, "(no events)", 0.5f, COL_DIM); return; }

    float y = a.Y + 2;
    var entries = eventLog.Skip(Math.Max(0, eventLog.Count - maxLines)).Reverse().ToList();
    foreach (var e in entries)
    {
        if (y > a.Y + a.Height - lineH) break;
        Color c;
        switch (e.Level) { case LogLevel.Fire: c = COL_GREEN; break; case LogLevel.Warn: c = COL_YELLOW; break; case LogLevel.Error: c = COL_RED; break; default: c = COL_DIM; break; }
        DrawText(ref f, FormatTimestamp(e.Timestamp), new Vector2(a.X, y), 0.35f, COL_DIM);
        DrawText(ref f, Trunc(e.Message, 48), new Vector2(a.X + 56, y), 0.4f, c);
        y += lineH;
    }
}
#endregion