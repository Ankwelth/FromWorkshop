// ===== DUTC BUILD AND REPAIR =====
// Requires: SKO Nanobot Build and Repair System mod (maintained version).
//
// What it does:
//  - Auto-configures every newly placed BaR block with your settings (below).
//  - Auto-queues missing components to your assemblers.
//  - Dutc-style LCD screens.
//
// Profile priority: a captured profile in this PB's Custom Data (run 'capture')
// beats the built-in defaults below. Delete Custom Data to go back to defaults.
//
// Screen tags (put in LCD/cockpit name):
//   [BAR]       = status overview        [BAR-WELD] = weld target list
//   [BAR-GRIND] = grind target list      [BAR-MISS] = missing components
//   [BAR-PROG]  = live weld progress (active welds first, bar + % per block)
//   Cockpit surfaces: [BAR:1] = second screen, etc.
//
// Toolbar / terminal arguments:
//   capture = save settings from your BaR block as the profile
//             (tag one block [BAR-SRC] to pick which; otherwise first found)
//   apply   = force-apply profile to ALL BaR blocks now
//   queue   = toggle auto-queuing on/off
//   on/off  = enable/disable all BaR systems

// ===== DEFAULT PROFILE - edit freely =====
static long DEF_SearchMode = 2;              // 1=Grids, 2=BoundingBox
static long DEF_WorkMode = 1;                // 1=WeldBeforeGrind, 2=GrindBeforeWeld, 8=WeldOnly, 16=GrindOnly
static long DEF_WeldMode = 0;                // 0=WeldFull, 1=WeldFunctional, 2=WeldSkeleton
static bool DEF_AllowBuild = true;           // build projected blocks
static bool DEF_UseIgnoreColor = true;
static bool DEF_UseGrindColor = true;
static bool DEF_GrindJanitorEnemies = true;
static bool DEF_GrindJanitorNotOwned = false;
static bool DEF_GrindJanitorNeutrals = false;
static bool DEF_JanitorDisableOnly = false;
static bool DEF_JanitorHackOnly = false;
static bool DEF_ScriptControlled = false;
static bool DEF_CollectIfIdle = false;
static bool DEF_PushIngotOreImmediately = true;
static float DEF_AreaWidth = 200, DEF_AreaHeight = 200, DEF_AreaDepth = 200;
static float DEF_AreaOffsetLeftRight = 0, DEF_AreaOffsetUpDown = 0, DEF_AreaOffsetFrontBack = 0;
// (Ignore/Grind COLOR values and priority lists are not in the defaults -
//  run 'capture' once if you use custom ones; captured profile includes them.)

// ===== GENERAL SETTINGS =====
int GRID_MODE = 2;              // which grids the script sees (BaR systems, assemblers, LCDs):
                                //   1 = this grid only
                                //   2 = this ship: local grid + rotors/pistons/wheels (DEFAULT - docked grids ignored)
                                //   3 = everything connected (connectors too) - for connector-joined multi-grid ships
bool SETUP_DOCKED = false;      // true = auto-setup also configures BaR blocks on connector-joined grids
                                // (only matters with GRID_MODE = 3)
double PAGE_SECONDS = 3;        // list page flip time (weld/grind/missing lists)
bool PROG_CYCLE = false;        // false = [BAR-PROG] stays on the active welds + next queue (no page cycling)
bool AUTO_QUEUE = true;         // queue missing components automatically
bool USE_SURVIVAL_KITS = false; // false = survival kits never receive queue jobs (they count as assemblers!)
bool AUTO_SETUP = true;         // apply profile to newly placed BaR blocks
string ASSEMBLER_GROUP = "";    // "" = use all assemblers the script sees
string IGNORE_TAG = "[ignore]"; // exempt assemblers/blocks

//======= no need to edit below =======

Color COL_BG    = new Color(8, 10, 16);
Color COL_FRAME = new Color(95, 105, 125);
Color COL_TEXT  = new Color(210, 220, 235);
Color COL_DIM   = new Color(110, 120, 140);
Color COL_GOOD  = new Color(80, 220, 120);
Color COL_WARN  = new Color(255, 190, 60);
Color COL_BAD   = new Color(255, 80, 70);
Color COL_ACC   = new Color(120, 160, 255);
Color COL_PANEL = new Color(18, 22, 32);

int tick = 0, rescan = 0, refresh = 0, queueTimer = 0;
bool autoQueue;
int totalBars = 0, onlineBars = 0, collectCount = 0, appliedCount = 0;
string curWeld = "", curGrind = "", lastMsg = "", profileSource = "";
string queueStatus = "STARTING";
bool showAsm = false;
List<string> asmNames = new List<string>();

List<IMyShipWelder> bars = new List<IMyShipWelder>();
IMyShipWelder firstBar = null;
List<long> assemblerIds = new List<long>();
List<IMyAssembler> asmList = new List<IMyAssembler>();
Dictionary<string, MyDefinitionId> bpCache = new Dictionary<string, MyDefinitionId>();
HashSet<string> noBp = new HashSet<string>();
HashSet<long> appliedIds = new HashSet<long>();
Dictionary<string, string> profile = new Dictionary<string, string>();
Dictionary<MyDefinitionId, int> missing = new Dictionary<MyDefinitionId, int>();
List<string> weldNames = new List<string>();
List<string> grindNames = new List<string>();
List<string> missNames = new List<string>();
List<string> missAmts = new List<string>();
List<KeyValuePair<MyDefinitionId, int>> missSort = new List<KeyValuePair<MyDefinitionId, int>>();
HashSet<string> seenWeld = new HashSet<string>();
HashSet<string> seenGrind = new HashSet<string>();
HashSet<long> seenCollect = new HashSet<long>();

public class ProgEntry { public string Name; public float Ratio; public bool Active; }
List<ProgEntry> progress = new List<ProgEntry>();
List<IMyTextSurface> progScreens = new List<IMyTextSurface>();

List<IMyTerminalBlock> tagged = new List<IMyTerminalBlock>();
List<IMyTextSurface> statScreens = new List<IMyTextSurface>();
List<IMyTextSurface> weldScreens = new List<IMyTextSurface>();
List<IMyTextSurface> grindScreens = new List<IMyTextSurface>();
List<IMyTextSurface> missScreens = new List<IMyTextSurface>();

static string[] BoolProps = { "AllowBuild", "UseIgnoreColor", "UseGrindColor",
    "GrindJanitorEnemies", "GrindJanitorNotOwned", "GrindJanitorNeutrals",
    "GrindJanitorOptionDisableOnly", "GrindJanitorOptionHackOnly",
    "ScriptControlled", "CollectIfIdle", "PushIngotOreImmediately" };
static string[] LongProps = { "Mode", "WorkMode", "WeldMode" };
static string[] FloatProps = { "AreaWidth", "AreaHeight", "AreaDepth",
    "AreaOffsetLeftRight", "AreaOffsetUpDown", "AreaOffsetFrontBack" };
static string[] Vec3Props = { "IgnoreColor", "GrindColor" };

public Program()
{
    autoQueue = AUTO_QUEUE;
    // remember which blocks were already configured (survives recompile)
    if (!string.IsNullOrEmpty(Storage))
    {
        foreach (var part in Storage.Split(';'))
        {
            long id;
            if (long.TryParse(part, out id)) appliedIds.Add(id);
        }
    }
    LoadProfile();
    Scan();
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Save()
{
    var sb = new StringBuilder();
    foreach (var id in appliedIds) { if (sb.Length > 0) sb.Append(';'); sb.Append(id); }
    Storage = sb.ToString();
}

public void Main(string arg, UpdateType src)
{
    tick++;
    arg = arg.Trim().ToLower();
    if (arg == "capture") Capture();
    else if (arg == "apply") ApplyAll();
    else if (arg == "queue") { autoQueue = !autoQueue; lastMsg = "Auto-queue " + (autoQueue ? "ON" : "OFF"); }
    else if (arg == "asm") { showAsm = !showAsm; lastMsg = "Assembler listing " + (showAsm ? "ON" : "OFF"); }
    else if (arg == "on" || arg == "off")
    {
        bool en = arg == "on";
        foreach (var w in bars) if (!w.Closed) w.Enabled = en;
        lastMsg = "BaR systems " + (en ? "ENABLED" : "DISABLED");
    }

    if (--rescan <= 0) { Scan(); rescan = 90; }     // ~15s
    if (--refresh <= 0) { Refresh(); refresh = 12; } // ~2s
    if (--queueTimer <= 0) { AutoQueue(); queueTimer = 30; } // ~5s

    Echo("Dutc Build & Repair");
    Echo("Systems: " + onlineBars + "/" + totalBars + " online");
    Echo("Profile: " + profileSource);
    Echo("Auto-queue: " + (autoQueue ? "ON (" + assemblerIds.Count + " asm)" : "OFF") + " - " + queueStatus);
    Echo("Auto-setup: " + appliedCount + " configured this session");
    if (lastMsg != "") Echo(lastMsg);
    if (showAsm)
    {
        Echo("--- Assemblers found (run 'asm' to hide) ---");
        foreach (var n in asmNames) Echo("  " + n);
    }

    foreach (var s in statScreens) DrawStatus(s);
    foreach (var s in progScreens) DrawProgress(s);
    foreach (var s in weldScreens) DrawList(s, "WELD TARGETS", COL_GOOD, weldNames, null);
    foreach (var s in grindScreens) DrawList(s, "GRIND TARGETS", COL_WARN, grindNames, null);
    foreach (var s in missScreens) DrawList(s, "MISSING COMPONENTS", COL_BAD, missNames, missAmts);
}

// ================= PROFILE =================

void LoadProfile()
{
    profile.Clear();
    var cd = Me.CustomData;
    if (!string.IsNullOrEmpty(cd))
    {
        var start = cd.IndexOf("@DutcBaR");
        var end = cd.IndexOf("@/DutcBaR");
        if (start >= 0 && end > start)
        {
            var body = cd.Substring(start, end - start);
            foreach (var rawLine in body.Split('\n'))
            {
                var line = rawLine.Trim();
                var eq = line.IndexOf('=');
                if (eq <= 0 || line.StartsWith("@") || line.StartsWith("#")) continue;
                profile[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
        }
    }
    if (profile.Count > 0) { profileSource = "CAPTURED (Custom Data)"; return; }
    BuildDefaultProfile();
    profileSource = "BUILT-IN DEFAULTS";
}

void BuildDefaultProfile()
{
    profile["Mode"] = DEF_SearchMode.ToString();
    profile["WorkMode"] = DEF_WorkMode.ToString();
    profile["WeldMode"] = DEF_WeldMode.ToString();
    profile["AllowBuild"] = DEF_AllowBuild.ToString();
    profile["UseIgnoreColor"] = DEF_UseIgnoreColor.ToString();
    profile["UseGrindColor"] = DEF_UseGrindColor.ToString();
    profile["GrindJanitorEnemies"] = DEF_GrindJanitorEnemies.ToString();
    profile["GrindJanitorNotOwned"] = DEF_GrindJanitorNotOwned.ToString();
    profile["GrindJanitorNeutrals"] = DEF_GrindJanitorNeutrals.ToString();
    profile["GrindJanitorOptionDisableOnly"] = DEF_JanitorDisableOnly.ToString();
    profile["GrindJanitorOptionHackOnly"] = DEF_JanitorHackOnly.ToString();
    profile["ScriptControlled"] = DEF_ScriptControlled.ToString();
    profile["CollectIfIdle"] = DEF_CollectIfIdle.ToString();
    profile["PushIngotOreImmediately"] = DEF_PushIngotOreImmediately.ToString();
    profile["AreaWidth"] = DEF_AreaWidth.ToString("0.##");
    profile["AreaHeight"] = DEF_AreaHeight.ToString("0.##");
    profile["AreaDepth"] = DEF_AreaDepth.ToString("0.##");
    profile["AreaOffsetLeftRight"] = DEF_AreaOffsetLeftRight.ToString("0.##");
    profile["AreaOffsetUpDown"] = DEF_AreaOffsetUpDown.ToString("0.##");
    profile["AreaOffsetFrontBack"] = DEF_AreaOffsetFrontBack.ToString("0.##");
}

// ================= BaR DETECTION + SETUP =================

bool IsBaR(IMyShipWelder w)
{
    try { w.GetValueBool("BuildAndRepair.ScriptControlled"); return true; } catch { }
    try { w.GetValue<long>("BuildAndRepair.Mode"); return true; } catch { }
    return false;
}

// grid filter honoring GRID_MODE: 1=same grid, 2=same construct, 3=anything connected
bool GridOk(IMyTerminalBlock b)
{
    if (GRID_MODE <= 1) return b.CubeGrid == Me.CubeGrid;
    if (GRID_MODE == 2) return b.CubeGrid.IsSameConstructAs(Me.CubeGrid);
    return true;
}

void Scan()
{
    GridTerminalSystem.GetBlocksOfType(bars, b => GridOk(b) && IsBaR(b));
    totalBars = bars.Count;
    firstBar = null;
    foreach (var w in bars) { if (!w.Closed) { firstBar = w; break; } }

    assemblerIds.Clear();
    var asms = new List<IMyAssembler>();
    if (ASSEMBLER_GROUP != "")
    {
        var g = GridTerminalSystem.GetBlockGroupWithName(ASSEMBLER_GROUP);
        if (g != null) g.GetBlocksOfType(asms);
    }
    else
    {
        GridTerminalSystem.GetBlocksOfType(asms, b => GridOk(b) && !b.CustomName.Contains(IGNORE_TAG));
    }
    asmNames.Clear();
    asmList.Clear();
    foreach (var a in asms)
    {
        // only queue into finished, powered assemblers set to assemble
        // (skips half-welded ones, disassembly mode, and survival kits by default)
        if (a.Closed || !a.IsFunctional || !a.IsWorking) continue;
        if (a.Mode != MyAssemblerMode.Assembly) continue;
        if (!USE_SURVIVAL_KITS && a.BlockDefinition.TypeIdString.Contains("SurvivalKit")) continue;
        assemblerIds.Add(a.EntityId);
        asmList.Add(a);
        asmNames.Add(a.CustomName);
    }

    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(tagged, b => GridOk(b) && b.CustomName.Contains("["));
    Collect("BAR", statScreens);
    Collect("BAR-WELD", weldScreens);
    Collect("BAR-GRIND", grindScreens);
    Collect("BAR-MISS", missScreens);
    Collect("BAR-PROG", progScreens);
    statScreens.Add(Me.GetSurface(0));

    // auto-setup: newly placed BaR blocks get the profile once
    if (AUTO_SETUP && profile.Count > 0)
    {
        foreach (var w in bars)
        {
            if (w.Closed || appliedIds.Contains(w.EntityId)) continue;
            // optionally leave connector-docked grids alone (base PB vs visiting ships)
            if (!SETUP_DOCKED && !w.CubeGrid.IsSameConstructAs(Me.CubeGrid)) continue;
            ApplyProfile(w);
            appliedIds.Add(w.EntityId);
            appliedCount++;
            lastMsg = "Auto-configured: " + w.CustomName;
        }
    }
}

void Capture()
{
    IMyShipWelder src = null;
    foreach (var w in bars)
        if (!w.Closed && w.CustomName.ToUpper().Contains("[BAR-SRC]")) { src = w; break; }
    if (src == null) src = firstBar;
    if (src == null) { lastMsg = "Capture failed: no BaR system found."; return; }

    var sb = new StringBuilder();
    sb.Append("@DutcBaR  (profile captured from: ").Append(src.CustomName).Append(")\n");
    foreach (var p in LongProps)
    {
        long v; if (TryGetLong(src, p, out v)) sb.Append(p).Append('=').Append(v).Append('\n');
    }
    foreach (var p in BoolProps)
    {
        bool v; if (TryGetBool(src, p, out v)) sb.Append(p).Append('=').Append(v).Append('\n');
    }
    foreach (var p in FloatProps)
    {
        float v; if (TryGetFloat(src, p, out v)) sb.Append(p).Append('=').Append(v.ToString("0.####")).Append('\n');
    }
    foreach (var p in Vec3Props)
    {
        Vector3 v; if (TryGetVec(src, p, out v))
            sb.Append(p).Append('=').Append(v.X.ToString("0.####")).Append('|')
              .Append(v.Y.ToString("0.####")).Append('|').Append(v.Z.ToString("0.####")).Append('\n');
    }
    sb.Append("WeldPrio=").Append(CapturePrio(src, "GetWeldPriority", "GetWeldEnabled", 20)).Append('\n');
    sb.Append("GrindPrio=").Append(CapturePrio(src, "GetGrindPriority", "GetGrindEnabled", 20)).Append('\n');
    sb.Append("CollectPrio=").Append(CapturePrio(src, "GetCollectPriority", "GetCollectEnabled", 5)).Append('\n');
    sb.Append("@/DutcBaR\n");

    Me.CustomData = sb.ToString();
    LoadProfile();

    // existing blocks are already configured by the user - don't touch them
    foreach (var w in bars) if (!w.Closed) appliedIds.Add(w.EntityId);
    lastMsg = "Profile captured from: " + src.CustomName;
}

string CapturePrio(IMyShipWelder src, string getPrioProp, string getEnProp, int classCount)
{
    var sb = new StringBuilder();
    try
    {
        var getP = src.GetValue<Func<int, int>>("BuildAndRepair." + getPrioProp);
        var getE = src.GetValue<Func<int, bool>>("BuildAndRepair." + getEnProp);
        if (getP == null || getE == null) return "";
        for (int c = 1; c <= classCount; c++)
        {
            if (sb.Length > 0) sb.Append(',');
            sb.Append(c).Append(':').Append(getP(c)).Append(':').Append(getE(c));
        }
    }
    catch { return ""; }
    return sb.ToString();
}

void ApplyAll()
{
    if (profile.Count == 0) { lastMsg = "No profile available."; return; }
    int n = 0;
    foreach (var w in bars)
    {
        if (w.Closed) continue;
        ApplyProfile(w);
        appliedIds.Add(w.EntityId);
        n++;
    }
    lastMsg = "Profile applied to " + n + " system" + (n == 1 ? "" : "s") + ".";
}

void ApplyProfile(IMyShipWelder w)
{
    string v;
    foreach (var p in LongProps)
    {
        long lv;
        if (profile.TryGetValue(p, out v) && long.TryParse(v, out lv))
            try { w.SetValue<long>("BuildAndRepair." + p, lv); } catch { }
    }
    foreach (var p in BoolProps)
    {
        bool bv;
        if (profile.TryGetValue(p, out v) && bool.TryParse(v, out bv))
            try { w.SetValueBool("BuildAndRepair." + p, bv); } catch { }
    }
    foreach (var p in FloatProps)
    {
        float fv;
        if (profile.TryGetValue(p, out v) && float.TryParse(v, out fv))
            try { w.SetValueFloat("BuildAndRepair." + p, fv); } catch { }
    }
    foreach (var p in Vec3Props)
    {
        if (profile.TryGetValue(p, out v))
        {
            var parts = v.Split('|');
            float x, y, z;
            if (parts.Length == 3 && float.TryParse(parts[0], out x) && float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z))
                try { w.SetValue<Vector3>("BuildAndRepair." + p, new Vector3(x, y, z)); } catch { }
        }
    }
    if (profile.TryGetValue("WeldPrio", out v)) ApplyPrio(w, v, "SetWeldPriority", "SetWeldEnabled");
    if (profile.TryGetValue("GrindPrio", out v)) ApplyPrio(w, v, "SetGrindPriority", "SetGrindEnabled");
    if (profile.TryGetValue("CollectPrio", out v)) ApplyPrio(w, v, "SetCollectPriority", "SetCollectEnabled");
    try { w.Enabled = true; } catch { }
}

void ApplyPrio(IMyShipWelder w, string data, string setPrioProp, string setEnProp)
{
    if (string.IsNullOrEmpty(data)) return;
    try
    {
        var setP = w.GetValue<Action<int, int>>("BuildAndRepair." + setPrioProp);
        var setE = w.GetValue<Action<int, bool>>("BuildAndRepair." + setEnProp);
        if (setP == null || setE == null) return;
        foreach (var entry in data.Split(','))
        {
            var parts = entry.Split(':');
            int cls, prio; bool en;
            if (parts.Length == 3 && int.TryParse(parts[0], out cls) && int.TryParse(parts[1], out prio) && bool.TryParse(parts[2], out en))
            {
                setP(cls, prio);
                setE(cls, en);
            }
        }
    }
    catch { }
}

// ================= DATA =================

void Refresh()
{
    onlineBars = 0;
    foreach (var w in bars) { if (!w.Closed && w.IsWorking && w.IsFunctional) onlineBars++; }

    curWeld = ""; curGrind = ""; collectCount = 0;
    weldNames.Clear(); grindNames.Clear(); progress.Clear();
    missing.Clear(); missNames.Clear(); missAmts.Clear();
    seenWeld.Clear(); seenGrind.Clear(); seenCollect.Clear();
    if (firstBar == null || firstBar.Closed) return;

    // pass 1: what each builder is welding RIGHT NOW - these lead the list
    foreach (var w in bars)
    {
        if (w.Closed) continue;
        var cur = SafeGet<IMySlimBlock>(w, "BuildAndRepair.CurrentTarget");
        if (cur != null && AddSlim(seenWeld, cur))
            progress.Add(new ProgEntry { Name = SlimName(cur), Ratio = RatioOf(cur), Active = true });
        if (curGrind == "") curGrind = SlimName(SafeGet<IMySlimBlock>(w, "BuildAndRepair.CurrentGrindTarget"));
    }
    if (progress.Count > 0) curWeld = progress[0].Name;

    // pass 2: queued targets from ALL builders, kept in each builder's work order
    foreach (var w in bars)
    {
        if (w.Closed) continue;

        var wl = SafeGet<MemorySafeList<IMySlimBlock>>(w, "BuildAndRepair.PossibleTargets");
        if (wl != null) foreach (var t in wl)
        {
            if (progress.Count >= 200) break;
            if (AddSlim(seenWeld, t))
                progress.Add(new ProgEntry { Name = SlimName(t), Ratio = RatioOf(t), Active = false });
        }
        var gl = SafeGet<MemorySafeList<IMySlimBlock>>(w, "BuildAndRepair.PossibleGrindTargets");
        if (gl != null) foreach (var t in gl)
        {
            if (grindNames.Count >= 200) break;
            if (AddSlim(seenGrind, t)) grindNames.Add(SlimName(t));
        }
        var cl = SafeGet<MemorySafeList<IMyEntity>>(w, "BuildAndRepair.PossibleCollectTargets");
        if (cl != null) foreach (var e in cl)
        {
            if (e == null) continue;
            try { seenCollect.Add(e.EntityId); } catch { }
        }

        // missing components merged (max per item, not sum - overlapping systems report the same parts)
        var dict = SafeGet<MemorySafeDictionary<MyDefinitionId, int>>(w, "BuildAndRepair.MissingComponents");
        if (dict != null) foreach (var kv in dict)
        {
            int cur2;
            if (missing.TryGetValue(kv.Key, out cur2)) { if (kv.Value > cur2) missing[kv.Key] = kv.Value; }
            else missing.Add(kv.Key, kv.Value);
        }
    }
    collectCount = seenCollect.Count;
    foreach (var p in progress) weldNames.Add(p.Name); // weld list shares the same next-up ordering
    missSort.Clear();
    foreach (var kv in missing) missSort.Add(kv);
    missSort.Sort((a, b) => b.Value.CompareTo(a.Value));
    foreach (var kv in missSort)
    {
        missNames.Add(kv.Key.SubtypeName);
        missAmts.Add(kv.Value.ToString());
    }
}

void AutoQueue()
{
    if (!autoQueue) { queueStatus = "OFF"; return; }
    if (firstBar == null || firstBar.Closed) { queueStatus = "NO BaR SYSTEM"; return; }
    if (assemblerIds.Count == 0) { queueStatus = "NO READY ASSEMBLERS"; return; }
    if (missing.Count == 0) { queueStatus = "NOTHING MISSING"; return; }
    var fn = SafeGet<Func<IEnumerable<long>, MyDefinitionId, int, int>>(firstBar, "BuildAndRepair.ProductionBlock.EnsureQueued");
    int kinds = 0;
    if (fn != null)
    {
        foreach (var kv in missing)
        {
            if (kv.Value > 0)
            {
                try { fn(assemblerIds, kv.Key, kv.Value); kinds++; } catch { }
            }
        }
        queueStatus = "QUEUED " + kinds + " ITEM TYPES";
        return;
    }
    // mod queue API not available on this BaR version: queue directly ourselves
    var q = new List<MyProductionItem>();
    foreach (var kv in missing)
    {
        if (kv.Value <= 0) continue;
        MyDefinitionId bp;
        if (!ResolveBp(kv.Key.SubtypeName, out bp)) continue;
        double inQ = 0;
        foreach (var a in asmList)
        {
            q.Clear(); a.GetQueue(q);
            foreach (var pi in q) if (pi.BlueprintId == bp) inQ += (double)pi.Amount;
        }
        double need = kv.Value - inQ;
        if (need < 1) continue;
        IMyAssembler best = null; int bestQ = int.MaxValue;
        foreach (var a in asmList)
        {
            if (!a.CanUseBlueprint(bp)) continue;
            q.Clear(); a.GetQueue(q);
            if (q.Count < bestQ) { bestQ = q.Count; best = a; }
        }
        if (best != null)
        {
            best.AddQueueItem(bp, (VRage.MyFixedPoint)need);
            kinds++;
        }
        if (Runtime.CurrentInstructionCount > 30000) break;
    }
    queueStatus = kinds > 0 ? "QUEUED " + kinds + " TYPES (direct)" : "QUEUED NOTHING - NO BLUEPRINTS FOUND";
}

bool ResolveBp(string name, out MyDefinitionId bp)
{
    bp = new MyDefinitionId();
    if (bpCache.TryGetValue(name, out bp)) return true;
    if (noBp.Contains(name)) return false;
    if (asmList.Count == 0) return false;
    string baseN = name.Replace("Item", "");
    var variants = new List<string> { name, name + "Component", name + "Magazine", baseN, baseN + "Component", baseN + "Magazine" };
    for (int n = 1; n <= 200; n = n < 30 ? n + 1 : n + 5)
    {
        string pre = "Position" + n.ToString("0000") + "_";
        variants.Add(pre + name);
        variants.Add(pre + baseN);
    }
    foreach (var s in variants)
    {
        MyDefinitionId id;
        if (!MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + s, out id)) continue;
        foreach (var a in asmList)
            if (a.CanUseBlueprint(id)) { bpCache[name] = id; bp = id; return true; }
        if (Runtime.CurrentInstructionCount > 35000) return false;
    }
    noBp.Add(name);
    return false;
}

T SafeGet<T>(IMyTerminalBlock b, string prop)
{
    try { return b.GetValue<T>(prop); } catch { return default(T); }
}
bool TryGetBool(IMyTerminalBlock b, string p, out bool v) { v = false; try { v = b.GetValueBool("BuildAndRepair." + p); return true; } catch { return false; } }
bool TryGetLong(IMyTerminalBlock b, string p, out long v) { v = 0; try { v = b.GetValue<long>("BuildAndRepair." + p); return true; } catch { return false; } }
bool TryGetFloat(IMyTerminalBlock b, string p, out float v) { v = 0; try { v = b.GetValueFloat("BuildAndRepair." + p); return true; } catch { return false; } }
bool TryGetVec(IMyTerminalBlock b, string p, out Vector3 v) { v = Vector3.Zero; try { v = b.GetValue<Vector3>("BuildAndRepair." + p); return true; } catch { return false; } }

// build completion 0..1 for a slim block
float RatioOf(IMySlimBlock b)
{
    if (b == null) return 0f;
    try { return b.BuildLevelRatio; }
    catch
    {
        try { return b.MaxIntegrity > 0 ? b.BuildIntegrity / b.MaxIntegrity : 0f; }
        catch { return 0f; }
    }
}

// dedupe key so a block seen by two overlapping builders is only listed once
bool AddSlim(HashSet<string> set, IMySlimBlock b)
{
    if (b == null) return false;
    string key;
    try { key = (b.CubeGrid != null ? b.CubeGrid.EntityId : 0) + ":" + b.Position; }
    catch { key = SlimName(b); }
    return set.Add(key);
}

string SlimName(IMySlimBlock b)
{
    if (b == null) return "";
    var fat = b.FatBlock as IMyTerminalBlock;
    if (fat != null) return fat.CustomName;
    try { return b.BlockDefinition.SubtypeName; } catch { return "Block"; }
}

// ================= SCREENS =================

void Collect(string tag, List<IMyTextSurface> list)
{
    list.Clear();
    foreach (var b in tagged)
    {
        string name = b.CustomName.ToUpper();
        int pos = 0;
        while (pos < name.Length)
        {
            int i = name.IndexOf('[', pos);
            if (i < 0) break;
            int end = name.IndexOf(']', i);
            if (end < 0) break;
            pos = end + 1;
            string inner = name.Substring(i + 1, end - i - 1);
            string baseTag = inner; int idx = 0;
            int colon = inner.IndexOf(':');
            if (colon >= 0)
            {
                baseTag = inner.Substring(0, colon);
                int.TryParse(inner.Substring(colon + 1), out idx);
            }
            if (baseTag.Trim() != tag) continue;

            var prov = b as IMyTextSurfaceProvider;
            var surf = b as IMyTextSurface;
            if (prov != null && idx >= 0 && idx < prov.SurfaceCount) list.Add(prov.GetSurface(idx));
            else if (surf != null) list.Add(surf);
        }
    }
}

int Page(int pageCount)
{
    if (pageCount <= 1) return 0;
    int hold = Math.Max(1, (int)(PAGE_SECONDS * 6));
    return (tick / hold) % pageCount;
}

void DrawStatus(IMyTextSurface s)
{
    var frame = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f;

    if (totalBars == 0)
    {
        frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.006f), new Vector2(W, H * 0.012f), COL_BAD));
        frame.Add(Txt("BUILD & REPAIR", off + new Vector2(W * 0.5f, H * 0.035f), 1.1f * sc, COL_DIM));
        frame.Add(Icon(frame, "Danger", off + new Vector2(W * 0.5f, H * 0.35f), 60f * sc, COL_BAD));
        frame.Add(Txt("NO BaR SYSTEMS FOUND", off + new Vector2(W * 0.5f, H * 0.45f), 1.0f * sc, COL_BAD));
        frame.Add(Txt("Nanobot Build & Repair mod required", off + new Vector2(W * 0.5f, H * 0.53f), 0.7f * sc, COL_DIM));
        frame.Dispose(); return;
    }

    string state; Color stateCol; string target = "";
    if (onlineBars == 0) { state = "OFFLINE"; stateCol = COL_BAD; }
    else if (curWeld != "") { state = "WELDING"; stateCol = COL_GOOD; target = curWeld; }
    else if (curGrind != "") { state = "GRINDING"; stateCol = COL_WARN; target = curGrind; }
    else { state = "STANDBY"; stateCol = COL_DIM; }

    // top accent strip colored by state
    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.006f), new Vector2(W, H * 0.012f), stateCol));
    frame.Add(Txt("BUILD & REPAIR", off + new Vector2(W * 0.5f, H * 0.035f), 1.1f * sc, COL_DIM));

    // one status dot per BaR system
    int shown = Math.Min(totalBars, 16);
    float spacing = 24f * sc;
    float x0 = W * 0.5f - (shown - 1) * spacing * 0.5f;
    int di = 0;
    foreach (var w in bars)
    {
        if (di >= shown) break;
        if (w.Closed) continue;
        Color dc = (w.IsWorking && w.IsFunctional) ? COL_GOOD : COL_BAD;
        frame.Add(Icon(frame, "Circle", off + new Vector2(x0 + di * spacing, H * 0.125f), 12f * sc, dc));
        di++;
    }
    frame.Add(Txt(onlineBars + "/" + totalBars + " ONLINE", off + new Vector2(W * 0.5f, H * 0.155f), 0.6f * sc, COL_DIM));

    // state banner with pulsing side dots while working
    if (curWeld != "" || curGrind != "")
    {
        float pulse = (10f + 3f * (float)Math.Sin(tick * 0.35)) * sc;
        frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.14f, H * 0.265f), pulse, stateCol));
        frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.86f, H * 0.265f), pulse, stateCol));
    }
    frame.Add(Txt(state, off + new Vector2(W * 0.5f, H * 0.215f), 1.7f * sc, stateCol));
    if (target != "")
        frame.Add(Txt(Trunc(target, (int)(W * 0.66f / (12f * sc))), off + new Vector2(W * 0.5f, H * 0.335f), 0.72f * sc, COL_TEXT));

    // 2x2 stat tiles
    Tile(frame, off, size, 0.28f, 0.475f, "WELD TARGETS", weldNames.Count.ToString(), COL_GOOD);
    Tile(frame, off, size, 0.72f, 0.475f, "GRIND TARGETS", grindNames.Count.ToString(), COL_WARN);
    Tile(frame, off, size, 0.28f, 0.645f, "FLOATING ITEMS", collectCount.ToString(), COL_ACC);
    Tile(frame, off, size, 0.72f, 0.645f, "MISSING TYPES", missing.Count.ToString(), missing.Count > 0 ? COL_BAD : COL_GOOD);
    if (missing.Count > 0)
        frame.Add(Icon(frame, "Danger", off + new Vector2(W * 0.885f, H * 0.655f), 26f * sc, COL_BAD));

    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.755f), new Vector2(W * 0.92f, 2f * sc), COL_FRAME));

    // footer
    Color qCol = autoQueue && assemblerIds.Count > 0 ? COL_GOOD : COL_DIM;
    bool qErr = queueStatus.Contains("MISSING -") || queueStatus == "NO READY ASSEMBLERS" || queueStatus == "NO BaR SYSTEM";
    Color qsCol = COL_DIM;
    if (qErr) qsCol = COL_BAD;
    else if (queueStatus.StartsWith("QUEUED")) qsCol = COL_GOOD;
    frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.09f, H * 0.787f), 9f * sc, qCol));
    frame.Add(Txt("AUTO-QUEUE " + (autoQueue ? "ON  (" + assemblerIds.Count + " ASM)" : "OFF"),
        off + new Vector2(W * 0.13f, H * 0.772f), 0.68f * sc, qCol, TextAlignment.LEFT));
    frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.09f, H * 0.842f), 9f * sc, qsCol));
    frame.Add(Txt(queueStatus, off + new Vector2(W * 0.13f, H * 0.827f), 0.62f * sc, qsCol, TextAlignment.LEFT));
    frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.09f, H * 0.897f), 9f * sc, COL_ACC));
    frame.Add(Txt("PROFILE: " + profileSource.ToUpper(),
        off + new Vector2(W * 0.13f, H * 0.882f), 0.62f * sc, COL_ACC, TextAlignment.LEFT));
    frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.09f, H * 0.95f), 9f * sc, COL_DIM));
    frame.Add(Txt("AUTO-SETUP: " + appliedCount + " CONFIGURED",
        off + new Vector2(W * 0.13f, H * 0.935f), 0.62f * sc, COL_DIM, TextAlignment.LEFT));

    frame.Dispose();
}

void Tile(MySpriteDrawFrame frame, Vector2 off, Vector2 size, float cx, float cy, string label, string value, Color accent)
{
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f;
    Vector2 c = off + new Vector2(W * cx, H * cy);
    frame.Add(Box(c, new Vector2(W * 0.42f, H * 0.15f), COL_PANEL));
    frame.Add(Box(c - new Vector2(0, H * 0.075f - 1.5f * sc), new Vector2(W * 0.42f, 3f * sc), accent));
    frame.Add(Txt(label, c - new Vector2(0, H * 0.062f), 0.55f * sc, COL_DIM));
    frame.Add(Txt(value, c - new Vector2(0, H * 0.028f), 1.15f * sc, accent));
}

MySprite Icon(MySpriteDrawFrame frame, string texture, Vector2 pos, float px, Color c)
{
    return new MySprite() { Type = SpriteType.TEXTURE, Data = texture,
        Position = pos, Size = new Vector2(px, px), Color = c, Alignment = TextAlignment.CENTER };
}

void DrawList(IMyTextSurface s, string title, Color accent, List<string> names, List<string> right)
{
    var frame = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f;

    // top accent strip + header
    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.006f), new Vector2(W, H * 0.012f), accent));
    frame.Add(Txt(title, off + new Vector2(W * 0.5f, H * 0.035f), 1.05f * sc, COL_DIM));

    // count pill
    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.135f), new Vector2(W * 0.22f, H * 0.065f), COL_PANEL));
    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.105f), new Vector2(W * 0.22f, 2.5f * sc), accent));
    frame.Add(Txt(names.Count.ToString(), off + new Vector2(W * 0.5f, H * 0.108f), 1.0f * sc, accent));

    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.195f), new Vector2(W * 0.92f, 2f * sc), COL_FRAME));

    if (names.Count == 0)
    {
        frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.5f, H * 0.47f), 14f * sc, COL_GOOD));
        frame.Add(Txt("ALL CLEAR", off + new Vector2(W * 0.5f, H * 0.52f), 0.85f * sc, COL_DIM));
        frame.Dispose(); return;
    }

    float y = 0.225f, step = 0.058f;
    int maxRows = Math.Max(1, (int)((0.93f - y) / step));
    int pageCount = (names.Count + maxRows - 1) / maxRows;
    int page = Page(pageCount);
    int start = page * maxRows;
    int maxChars = (int)(W * (right != null ? 0.55f : 0.78f) / (12f * sc));

    for (int i = start; i < names.Count && i < start + maxRows; i++)
    {
        float rowY = y + (i - start) * step;
        Vector2 rowPos = off + new Vector2(0, H * rowY);
        if (i % 2 == 0) // zebra stripe
            frame.Add(Box(off + new Vector2(W * 0.485f, H * (rowY + 0.019f)), new Vector2(W * 0.89f, H * step * 0.92f), COL_PANEL));
        frame.Add(Icon(frame, "Circle", rowPos + new Vector2(W * 0.065f, H * 0.019f), 7f * sc, accent));
        frame.Add(Txt(Trunc(names[i], maxChars), rowPos + new Vector2(W * 0.10f, 0), 0.65f * sc, COL_TEXT, TextAlignment.LEFT));
        if (right != null && i < right.Count)
            frame.Add(Txt(right[i], rowPos + new Vector2(W * 0.905f, 0), 0.65f * sc, accent, TextAlignment.RIGHT));
    }

    // scrollbar showing page position
    if (pageCount > 1)
    {
        float trackTop = 0.225f, trackH = 0.70f;
        frame.Add(Box(off + new Vector2(W * 0.965f, H * (trackTop + trackH * 0.5f)), new Vector2(5f * sc, H * trackH), COL_PANEL));
        float thumbH = trackH / pageCount;
        float thumbC = trackTop + thumbH * (page + 0.5f);
        frame.Add(Box(off + new Vector2(W * 0.965f, H * thumbC), new Vector2(5f * sc, H * thumbH), accent));
        frame.Add(Txt("PAGE " + (page + 1) + "/" + pageCount, off + new Vector2(W * 0.5f, H * 0.955f), 0.6f * sc, COL_DIM));
    }
    frame.Dispose();
}

void DrawProgress(IMyTextSurface s)
{
    var frame = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f;

    int active = 0;
    foreach (var p in progress) if (p.Active) active++;

    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.006f), new Vector2(W, H * 0.012f), COL_GOOD));
    frame.Add(Txt("WELD PROGRESS", off + new Vector2(W * 0.5f, H * 0.035f), 1.05f * sc, COL_DIM));
    frame.Add(Txt(active + " WELDING   -   " + (progress.Count - active) + " QUEUED",
        off + new Vector2(W * 0.5f, H * 0.10f), 0.75f * sc, active > 0 ? COL_GOOD : COL_DIM));
    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.155f), new Vector2(W * 0.92f, 2f * sc), COL_FRAME));

    if (progress.Count == 0)
    {
        frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.5f, H * 0.47f), 14f * sc, COL_GOOD));
        frame.Add(Txt("ALL CLEAR", off + new Vector2(W * 0.5f, H * 0.52f), 0.85f * sc, COL_DIM));
        frame.Dispose(); return;
    }

    float y = 0.185f, step = 0.062f;
    int maxRows = Math.Max(1, (int)((0.93f - y) / step));
    int pageCount = (progress.Count + maxRows - 1) / maxRows;
    int page = 0;
    if (PROG_CYCLE) page = Page(pageCount); // default: pinned to the action, no cycling
    int start = page * maxRows;
    int maxChars = (int)(W * 0.42f / (12f * sc));

    for (int i = start; i < progress.Count && i < start + maxRows; i++)
    {
        var p = progress[i];
        float rowY = y + (i - start) * step;
        Vector2 rowPos = off + new Vector2(0, H * rowY);
        float midY = H * (rowY + 0.021f);

        if (i % 2 == 0)
            frame.Add(Box(off + new Vector2(W * 0.485f, midY), new Vector2(W * 0.89f, H * step * 0.9f), COL_PANEL));

        if (p.Active)
        {
            // green edge marker + pulsing dot = being welded right now
            frame.Add(Box(off + new Vector2(W * 0.045f, midY), new Vector2(4.5f * sc, H * step * 0.9f), COL_GOOD));
            float pd = (9f + 2.5f * (float)Math.Sin(tick * 0.4)) * sc;
            frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.085f, midY), pd, COL_GOOD));
        }
        else
        {
            frame.Add(Icon(frame, "Circle", off + new Vector2(W * 0.085f, midY), 7f * sc, COL_FRAME));
        }

        Color nameCol = p.Active ? COL_TEXT : COL_DIM;
        frame.Add(Txt(Trunc(p.Name, maxChars), rowPos + new Vector2(W * 0.115f, 0), 0.62f * sc, nameCol, TextAlignment.LEFT));

        // progress bar
        float bw = W * 0.26f, bh = 11f * sc;
        Vector2 bc = off + new Vector2(W * 0.70f, midY);
        frame.Add(Box(bc, new Vector2(bw + 4f * sc, bh + 4f * sc), COL_FRAME));
        frame.Add(Box(bc, new Vector2(bw, bh), COL_BG));
        float ratio = Math.Max(0f, Math.Min(1f, p.Ratio));
        float fw = bw * ratio;
        Color barCol = p.Active ? COL_GOOD : (ratio >= 0.999f ? COL_ACC : COL_WARN);
        if (fw > 1) frame.Add(Box(bc + new Vector2(-bw * 0.5f + fw * 0.5f, 0), new Vector2(fw, bh), barCol));

        frame.Add(Txt((int)Math.Round(ratio * 100) + "%", rowPos + new Vector2(W * 0.935f, 0), 0.62f * sc, barCol, TextAlignment.RIGHT));
    }

    if (pageCount > 1)
    {
        if (PROG_CYCLE)
        {
            float trackTop = 0.185f, trackH = 0.74f;
            frame.Add(Box(off + new Vector2(W * 0.965f, H * (trackTop + trackH * 0.5f)), new Vector2(5f * sc, H * trackH), COL_PANEL));
            float thumbH = trackH / pageCount;
            frame.Add(Box(off + new Vector2(W * 0.965f, H * (trackTop + thumbH * (page + 0.5f))), new Vector2(5f * sc, H * thumbH), COL_GOOD));
            frame.Add(Txt("PAGE " + (page + 1) + "/" + pageCount, off + new Vector2(W * 0.5f, H * 0.955f), 0.6f * sc, COL_DIM));
        }
        else
        {
            frame.Add(Txt("+" + (progress.Count - maxRows) + " MORE IN QUEUE", off + new Vector2(W * 0.5f, H * 0.955f), 0.6f * sc, COL_DIM));
        }
    }
    frame.Dispose();
}

// ================= DRAW HELPERS =================

MySpriteDrawFrame Begin(IMyTextSurface s)
{
    s.ContentType = ContentType.SCRIPT;
    s.Script = "";
    var frame = s.DrawFrame();
    frame.Add(Box((s.TextureSize - s.SurfaceSize) * 0.5f + s.SurfaceSize * 0.5f, s.SurfaceSize, COL_BG));
    return frame;
}

MySprite Box(Vector2 pos, Vector2 size, Color c)
{
    return new MySprite() { Type = SpriteType.TEXTURE, Data = "SquareSimple",
        Position = pos, Size = size, Color = c, Alignment = TextAlignment.CENTER };
}

MySprite Txt(string text, Vector2 pos, float scale, Color c, TextAlignment al = TextAlignment.CENTER)
{
    return new MySprite() { Type = SpriteType.TEXT, Data = text, Position = pos,
        RotationOrScale = scale, Color = c, Alignment = al, FontId = "White" };
}

string Trunc(string t, int max) { return max > 2 && t.Length > max ? t.Substring(0, max - 2) + ".." : t; }
