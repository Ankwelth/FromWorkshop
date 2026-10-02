const string VERSION = "1.1.0";
// ========================================================================================
// PRC - Project Required Components
// Lase Update : 2026/05/05
// ========================================================================================
//
// Displays required vs available components for the currently projecting blueprint.
//
// What it does
//  - Finds tagged Projector, Cargo Containers, Assemblers, and LCD SCRIPT surfaces.
//  - Computes required components from projector RemainingBlocksPerType
//    (optionally adds RemainingArmorBlocks as light/heavy armor).
//  - Counts available components from Cargo Container inventories (components only).
//  - Renders a table: REQ / AVL / MIS with optional component icons.
//  - Supports paging on a single LCD surface (auto-cycles by PgSec).
//  - Optional: enqueues missing components to tagged assemblers (distributed, no-dup, excludes Survival Kits).
//
// PB Run arguments
//  - reload : reload settings/localization from CustomData
//  - reset  : clear all caches (Storage + runtime)
//  - queue  : enqueue missing components to tagged assemblers
//
// Inventory scope
//  - Cargo Containers only
//  - If any tagged cargos exist -> count only tagged cargos
//  - Else -> count all cargos on the PB grid
//
// Notes
//  - Auto reloads when PB CustomData changes (hash-based)
//  - Block scanning is cached (scan_ticks)
//  - Icon resolution is cached (Storage per surface) to reduce sprite scanning
// ========================================================================================

// ================================
// CustomData headers and templates
// ================================

const string SETTINGS_HEADER = "[PRC SETTINGS]";
const string LOC_HEADER      = "[PRC LOCALIZATION]";

const string SETTINGS_TEMPLATE =
@"[PRC SETTINGS]
# PB Run arguments:
#   reload : reload settings/localization from CustomData
#   reset  : clear all caches (Storage + runtime)
#   queue  : enqueue missing components to tagged assemblers
#
# LCD: use '@<index> PRC' in block CustomData to select LCD surface index.

tag=[PRC]

armor_type=light    # light | heavy | none
font_size=0.75

highlight_text_color=#FF5555
highlight_bg_color=#FF5555
highlight_bg=true
highlight_bg_alpha=5    # 0 ~ 255

component_icons=colorful   # off | colorful | vanilla
icon_size=1.0

show_avl=true
show_mis=true

PgSec=5    # page auto cycle seconds (0=off)

refresh_ticks=10    # 1 | 10 | 100
scan_ticks=100       # 1 | 10 | 100
";

const string LOC_TEMPLATE =
@"[PRC LOCALIZATION]
# Format:
#   Subtype=Display Name

SteelPlate=Steel Plate
InteriorPlate=Interior Plate
ConstructionComponent=Construction Comp.
ComputerComponent=Computer
MotorComponent=Motor
SmallTube=Small Steel Tube
LargeTube=Large Steel Tube
MetalGrid=Metal Grid
Display=Display
PowerCell=Power Cell
SolarCell=Solar Cell
GirderComponent=Girder
ReactorComponent=Reactor Comp.
DetectorComponent=Detector Comp.
MedicalComponent=Medical Comp.
ExplosivesComponent=Explosives
GravityGeneratorComponent=Gravity Comp.
RadioCommunicationComponent=Radio-comm Comp.
Superconductor=Superconductor.
ThrustComponent=Thruster Comp.
BulletproofGlass=Bulletproof Glass
ZoneChip=Zone Chip
PrototechFrame=Proto. Frame
PrototechPanel=Proto. Panel
PrototechCapacitor=Proto. Capacitor
PrototechPropulsionUnit=Proto. Propulsion Unit
PrototechMachinery=Proto. Machinery
PrototechCircuitry=Proto. Circuitry
PrototechCoolingUnit=Proto. Cooling Unit
";

// ============
// UI constants
// ============

const string TITLE           = "Project Required Components";
const string TITLE_NEEDED    = "REQ";
const string TITLE_AVAILABLE = "AVL";
const string TITLE_MISSING   = "MIS";

const string BP_PREF   = "MyObjectBuilder_BlueprintDefinition/";
const string COMP_PREF = "MyObjectBuilder_Component/";
const string SQ        = "SquareSimple";
const string CIR       = "Circle";
const string TRI       = "Triangle";

static readonly Color PB_SPLASH_FG = new Color(30, 130, 220);
static readonly Color PB_SPLASH_BG = new Color(0, 0, 0);

// ==================
// Enums and settings
// ==================

enum ArmorType { Light, Heavy, None }
enum ComponentIconMode { Off, Colorful, Vanilla }

class Settings
{
    public string Tag = "[PRC]";
    public ArmorType Armor = ArmorType.Light;

    public float FontScale = 0.75f;

    public Color HighlightTextColor = new Color(255, 85, 85);
    public Color HighlightBgColor   = new Color(255, 85, 85);

    public bool HighlightBg = true;
    public byte HighlightBgAlpha = 5;

    public int RefreshTicks = 10;
    public int ScanTicks = 100;

    public ComponentIconMode IconMode = ComponentIconMode.Colorful;
    public float IconSize = 1.0f;
	
	public bool ShowAvl = true;
	public bool ShowMis = true;
	
	public float PgSec = 5f;

    public UpdateFrequency ToUpdateFrequency()
    {
        if (RefreshTicks <= 1) return UpdateFrequency.Update1;
        if (RefreshTicks <= 10) return UpdateFrequency.Update10;
        return UpdateFrequency.Update100;
    }

    public int NormalizedScanTicks()
    {
        if (ScanTicks <= 1) return 1;
        if (ScanTicks <= 10) return 10;
        return 100;
    }
}

// =============================
// State (runtime + persistence)
// =============================

Settings cfg = new Settings();

ComponentIconMode lastIm = ComponentIconMode.Colorful;
float lastIs = 1.0f;

Dictionary<string, string> loc = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

Dictionary<string, Dictionary<string, int>> bps = new Dictionary<string, Dictionary<string, int>>();
bool bpsOk = false;

uint cdHash = 0;

int tk = 0;
int lastScan = int.MinValue;

int pg = 0;
double pgAcc = 0;

readonly Dictionary<long, Dictionary<string, string>> imap = new Dictionary<long, Dictionary<string, string>>();
Dictionary<long, IconResolver> rcache = new Dictionary<long, IconResolver>();

// ===========
// Block cache
// ===========

bool cacheOk = false;

readonly List<SurfaceRef> surf = new List<SurfaceRef>(16);
IMyProjector pj = null;

readonly List<IMyCargoContainer> cg = new List<IMyCargoContainer>(64);
readonly List<IMyAssembler> asm = new List<IMyAssembler>(16);

// ========================================
// Temporary buffers (allocation reduction)
// ========================================

readonly HashSet<long> am = new HashSet<long>();

readonly List<int> idxs = new List<int>(8);

readonly List<IMyTextSurfaceProvider> tp = new List<IMyTextSurfaceProvider>(32);
readonly List<IMyProjector> tpj = new List<IMyProjector>(8);

readonly List<IMyCargoContainer> tcg = new List<IMyCargoContainer>(16);
readonly List<IMyCargoContainer> acg = new List<IMyCargoContainer>(64);
readonly List<MyInventoryItem> ti = new List<MyInventoryItem>(256);

readonly List<IMyAssembler> tasm = new List<IMyAssembler>(16);
readonly List<MyProductionItem> tq = new List<MyProductionItem>(128);

readonly StringBuilder sbAg  = new StringBuilder("Ag");
readonly StringBuilder sb999 = new StringBuilder("999,999");
readonly StringBuilder sbQ   = new StringBuilder("Q");

readonly List<Row> rows = new List<Row>(64);
readonly Dictionary<string, int> avl = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
readonly HashSet<string> qsub = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
string lastMisText = "";
string lastMisTitle = "";

// ============
// Data structs
// ============

struct SurfaceRef
{
    public IMyTextSurface S;
    public IMyTerminalBlock B;
    public long K;
}

struct Row
{
    public string Sub;   // subtype
    public string Name;  // display
    public int Avl;      // available
    public int Req;      // needed
    public int Mis;      // missing
}

// =========================================
// Icon resolving (runtime + persistent map)
// =========================================

class IconResolver
{
    readonly HashSet<string> all;

    static readonly HashSet<string> ign = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        SQ, CIR, TRI
    };

    static readonly Dictionary<string, string[]> al = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        ["ComputerComponent"] = new[] { "Computer" },
        ["ConstructionComponent"] = new[] { "Construction" },
        ["DetectorComponent"] = new[] { "Detector" },
        ["ExplosivesComponent"] = new[] { "Explosives" },
        ["GirderComponent"] = new[] { "Girder" },
        ["GravityGeneratorComponent"] = new[] { "GravityGenerator" },
        ["MedicalComponent"] = new[] { "Medical" },
        ["MotorComponent"] = new[] { "Motor" },
        ["RadioCommunicationComponent"] = new[] { "RadioCommunication" },
        ["ReactorComponent"] = new[] { "Reactor" },
        ["ThrustComponent"] = new[] { "Thrust" },
        ["BulletproofGlass"] = new[] { "BulletproofGlass", "Glass" },
        ["LargeTube"] = new[] { "LargeTube", "LargeSteelTube" },
        ["SmallTube"] = new[] { "SmallTube", "SmallSteelTube" },
    };

    public IconResolver(IMyTextSurface s)
    {
        all = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var sp = new List<string>();
        s.GetSprites(sp);
        for (int i = 0; i < sp.Count; i++)
        {
            var id = sp[i];
            if (ign.Contains(id)) continue;
            all.Add(id);
        }
    }

    IEnumerable<string> Cand(string sub)
    {
        yield return sub;
        if (sub.EndsWith("Component", StringComparison.OrdinalIgnoreCase))
            yield return sub.Substring(0, sub.Length - "Component".Length);

        string[] a;
        if (al.TryGetValue(sub, out a))
            for (int i = 0; i < a.Length; i++) yield return a[i];
    }

    public bool TryResolve(string sub, ComponentIconMode m, out string sid)
    {
        sid = null;
        if (m == ComponentIconMode.Off) return false;

        if (m == ComponentIconMode.Colorful)
        {
            foreach (var t in Cand(sub))
            {
                string c = "ColorfulIcons_Component/" + t;
                if (all.Contains(c)) { sid = c; return true; }
            }
        }

        foreach (var t in Cand(sub))
        {
            string v = COMP_PREF + t;
            if (all.Contains(v)) { sid = v; return true; }
        }

        return false;
    }
}

bool TryGetSpriteId(SurfaceRef sr, string sub, out string sid)
{
    sid = null;
    if (cfg.IconMode == ComponentIconMode.Off) return false;

    Dictionary<string, string> m;
    if (imap.TryGetValue(sr.K, out m))
    {
        string c;
        if (m.TryGetValue(sub, out c))
        {
            if (!string.IsNullOrEmpty(c)) { sid = c; return true; }
            return false;
        }
    }

    long id = sr.K >> 8;

    IconResolver r;
    if (!rcache.TryGetValue(id, out r))
    {
        r = new IconResolver(sr.S);
        rcache[id] = r;
    }

    string rs;
    bool ok = r.TryResolve(sub, cfg.IconMode, out rs);

    if (m == null)
    {
        m = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        imap[sr.K] = m;
    }
    if (ok) m[sub] = rs;
    if (ok) sid = rs;
    return ok;
}

// =================
// Program lifecycle
// =================

public Program()
{
    LoadPS(Storage);
    EnsureCDT();
    StartBpsParse();
    Reload(Me.CustomData ?? "");
}

public void Save() { Storage = BuildPS(); }

public void Main(string arg, UpdateType ur)
{
    tk += TickStep(ur);

    bool rld = false, que = false, rst = false;

    if (!string.IsNullOrWhiteSpace(arg))
    {
        var a = arg.Trim();
        if (a.Equals("reload", StringComparison.OrdinalIgnoreCase)) rld = true;
        if (a.Equals("reset", StringComparison.OrdinalIgnoreCase)) rst = true;
        if (a.Equals("queue", StringComparison.OrdinalIgnoreCase)) que = true;
    }

    if (rst)
    {
        ResetAll(true);
        return;
    }

    if (rld)
    {
        EnsureCDT();
        Reload(Me.CustomData ?? "");
        InvalidateCache();
    }
    else if (AutoReload())
    {
        InvalidateCache();
    }

    if (bpsRun)
    {
        StepBpsParse(8);
        DrawMsg(surf, "Parsing blockDefinitionData...\nLoaded definitions : " + bps.Count);
        return;
    }

    if (!bpsOk)
    {
        DrawMsg(surf, "blockDefinitionData is empty or parse failed.");
        return;
    }

    EnsureCache();

    DrawPbSplash();

    if (pj == null) { DrawMsg(surf, "No tagged projector found."); return; }
    if (!pj.IsProjecting) { DrawMsg(surf, pj.CustomName + " is not projecting."); return; }

    var need = Need(pj, cfg.Armor);
    var have = AvlFromCargos(avl);

    rows.Clear();
    foreach (var kv in need)
    {
        string bp = kv.Key;
        int req = kv.Value;

        string sub = StripBP(bp);
        string inv = sub.Replace("Component", "");

        int a = GetI(have, inv);
        int mis = req - a; if (mis < 0) mis = 0;

        rows.Add(new Row { Sub = sub, Name = Tr(sub), Avl = a, Req = req, Mis = mis });
    }

    rows.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

    UpdateMisText(surf, rows);

    if (que) QueueNoDup(rows);

    BuildQSub(qsub);
    pgAcc += Runtime.TimeSinceLastRun.TotalSeconds;
    DrawTable(surf, TITLE, rows);
}

// ============
// Tick helpers
// ============

int TickStep(UpdateType ur)
{
    if ((ur & UpdateType.Update1) != 0) return 1;
    if ((ur & UpdateType.Update10) != 0) return 10;
    if ((ur & UpdateType.Update100) != 0) return 100;
    return 0;
}

// ================================
// CustomData parsing + auto reload
// ================================

void FEL(string t, Action<string> a)
{
    if (string.IsNullOrEmpty(t)) return;
    int st = 0, n = t.Length;
    while (st < n)
    {
        int nl = t.IndexOf('\n', st);
        if (nl < 0) nl = n;

        int ed = nl;
        if (ed > st && t[ed - 1] == '\r') ed--;

        a(t.Substring(st, ed - st));
        st = nl + 1;
    }
}

void Reload(string cd)
{
    var nc = LoadCfg(cd);

    bool iconCh = nc.IconMode != lastIm || Math.Abs(nc.IconSize - lastIs) > 0.0001f;
    if (iconCh) ResetIconCache();

    lastIm = nc.IconMode;
    lastIs = nc.IconSize;

    cfg = nc;
    loc = LoadLoc(cd);

    cdHash = Fnv1a32(cd);

    Runtime.UpdateFrequency = cfg.ToUpdateFrequency();
}

bool AutoReload()
{
    string cd = Me.CustomData ?? "";
    uint h = Fnv1a32(cd);
    if (h == cdHash) return false;

    EnsureCDT();
    cd = Me.CustomData ?? "";
    Reload(cd);
    return true;
}

void EnsureCDT()
{
    if (string.IsNullOrWhiteSpace(Me.CustomData))
    {
        Me.CustomData = SETTINGS_TEMPLATE + "\n\n" + LOC_TEMPLATE;
        return;
    }

    if (Me.CustomData.IndexOf(SETTINGS_HEADER, StringComparison.OrdinalIgnoreCase) < 0)
        Me.CustomData = Me.CustomData.TrimEnd() + "\n\n" + SETTINGS_TEMPLATE;

    if (Me.CustomData.IndexOf(LOC_HEADER, StringComparison.OrdinalIgnoreCase) < 0)
        Me.CustomData = Me.CustomData.TrimEnd() + "\n\n" + LOC_TEMPLATE;
}

string StripC(string ln)
{
    if (ln == null) return "";
    for (int i = 0; i < ln.Length; i++)
    {
        if (ln[i] != '#') continue;
        if (i == 0) return "";
        if (char.IsWhiteSpace(ln[i - 1]))
            return ln.Substring(0, i).TrimEnd();
    }
    return ln;
}

bool GetPrcIdx(string cd, List<int> outIdx, IMyTerminalBlock tb=null, IMyTextSurfaceProvider p=null)
{
    outIdx.Clear();
    if (string.IsNullOrWhiteSpace(cd)) cd = "";

    bool found = false;

    FEL(cd, raw =>
    {
        var ln = StripC(raw).Trim();
        if (ln.Length < 2 || ln[0] != '@') return;

        int sp = ln.IndexOf(' ');
        string s = (sp < 0) ? ln.Substring(1) : ln.Substring(1, sp - 1);

        int idx;
        if (!int.TryParse(s, out idx)) return;

        outIdx.Add(idx);
        found = true;
    });

    if (!found && tb != null && p != null && p.SurfaceCount > 0)
    {
        tb.CustomData = (cd.Length > 0 ? cd.TrimEnd() + "\n\n" : "")
            + "# PRC auto configuration\n# @0 -> LCD index 0\n@0 PRC";

        outIdx.Add(0);
        return true;
    }

    return found;
}

Settings LoadCfg(string cd)
{
    var s = new Settings();
    bool sec = false, done = false;

    FEL(cd, rl =>
    {
        if (done) return;

        var ln = rl.Trim();
        if (ln.Length == 0) return;

        if (!sec)
        {
            if (ln.Equals(SETTINGS_HEADER, StringComparison.OrdinalIgnoreCase)) sec = true;
            return;
        }

        if (ln.StartsWith("[") && ln.EndsWith("]") && !ln.Equals(SETTINGS_HEADER, StringComparison.OrdinalIgnoreCase))
        { done = true; return; }

        ln = StripC(ln).Trim();
        if (ln.Length == 0) return;

        int eq = ln.IndexOf('=');
        if (eq <= 0) return;

        string k = ln.Substring(0, eq).Trim().ToLowerInvariant();
        string v = ln.Substring(eq + 1).Trim();

        if (k == "tag")
        {
            if (!string.IsNullOrWhiteSpace(v)) s.Tag = v;
        }
        else if (k == "armor_type")
        {
            var vv = v.ToLowerInvariant();
            if (vv == "light") s.Armor = ArmorType.Light;
            else if (vv == "heavy") s.Armor = ArmorType.Heavy;
            else if (vv == "none") s.Armor = ArmorType.None;
        }
        else if (k == "font_size")
        {
            float f;
            if (float.TryParse(v, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f))
            {
                if (f < 0.3f) f = 0.3f;
                if (f > 3.0f) f = 3.0f;
                s.FontScale = f;
            }
        }
        else if (k == "highlight_text_color")
        {
            Color c;
            if (TryParseHexColor(v, out c)) s.HighlightTextColor = c;
        }
        else if (k == "highlight_bg_color")
        {
            Color c;
            if (TryParseHexColor(v, out c)) s.HighlightBgColor = c;
        }
        else if (k == "highlight_color")
        {
            Color c;
            if (TryParseHexColor(v, out c))
            {
                s.HighlightTextColor = c;
                s.HighlightBgColor = c;
            }
        }
        else if (k == "highlight_bg")
        {
            s.HighlightBg = v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("1");
        }
        else if (k == "highlight_bg_alpha")
        {
            int a;
            if (int.TryParse(v, out a))
            {
                if (a < 0) a = 0;
                if (a > 255) a = 255;
                s.HighlightBgAlpha = (byte)a;
            }
        }
        else if (k == "refresh_ticks")
        {
            int t;
            if (int.TryParse(v, out t))
            {
                if (t <= 1) s.RefreshTicks = 1;
                else if (t <= 10) s.RefreshTicks = 10;
                else s.RefreshTicks = 100;
            }
        }
        else if (k == "scan_ticks")
        {
            int t;
            if (int.TryParse(v, out t))
            {
                if (t <= 1) s.ScanTicks = 1;
                else if (t <= 10) s.ScanTicks = 10;
                else s.ScanTicks = 100;
            }
        }
        else if (k == "show_avl")
        {
        s.ShowAvl = v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("1");
        }
        else if (k == "show_mis")
        {
        s.ShowMis = v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("1");
        }
        else if (k == "component_icons")
        {
            var vv = v.Trim().ToLowerInvariant();
            if (vv == "off") s.IconMode = ComponentIconMode.Off;
            else if (vv == "vanilla") s.IconMode = ComponentIconMode.Vanilla;
            else s.IconMode = ComponentIconMode.Colorful; // default
        }
        else if (k == "icon_size")
        {
            float f;
            if (float.TryParse(v, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f))
            {
                if (f < 0.3f) f = 0.3f;
                if (f > 1.5f) f = 1.5f;
                s.IconSize = f;
            }
        }
		else if (k == "pgsec")
        {
            float f;
            if (float.TryParse(v, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f))
            {
                if (f < 0f) f = 0f;
                s.PgSec = f;
            }
        }
    });

    return s;
}

Dictionary<string, string> LoadLoc(string cd)
{
    var m = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    bool sec = false, done = false;

    FEL(cd, rl =>
    {
        if (done) return;

        var ln = rl.Trim();
        if (ln.Length == 0) return;

        if (!sec)
        {
            if (ln.Equals(LOC_HEADER, StringComparison.OrdinalIgnoreCase)) sec = true;
            return;
        }

        if (ln.StartsWith("[") && ln.EndsWith("]") && !ln.Equals(LOC_HEADER, StringComparison.OrdinalIgnoreCase))
        { done = true; return; }

        ln = StripC(ln).Trim();
        if (ln.Length == 0) return;

        int eq = ln.IndexOf('=');
        if (eq <= 0) return;

        string k = ln.Substring(0, eq).Trim();
        string v = ln.Substring(eq + 1).Trim();
        if (k.Length == 0 || v.Length == 0) return;

        m[k] = v;
    });

    return m;
}

// ====================
// Block scanning cache
// ====================

void InvalidateCache()
{
    cacheOk = false;
    lastScan = int.MinValue;
    rcache.Clear();
}

void EnsureCache()
{
    int st = cfg.NormalizedScanTicks();
    if (cacheOk && (tk - lastScan) < st) return;

    lastScan = tk;
    RebuildCache();
}

void RebuildCache()
{
    surf.Clear();
    pj = null;
    cg.Clear();
    asm.Clear();

    tp.Clear();
    GridTerminalSystem.GetBlocksOfType(tp, b =>
    {
        var tb = b as IMyTerminalBlock;
        if (tb == null) return false;
        if (tb is IMyProgrammableBlock) return false;
        if (!HasTag(tb.CustomName, cfg.Tag)) return false;
        if (!tb.IsSameConstructAs(Me)) return false;
        return true;
    });

    for (int pi = 0; pi < tp.Count; pi++)
    {
        var p = tp[pi];
        var tb = p as IMyTerminalBlock;
        if (tb == null) continue;

        int cnt = p.SurfaceCount;

        bool has = GetPrcIdx(tb.CustomData, idxs, tb, p);
        if (!has) continue;

        for (int j = 0; j < idxs.Count; j++)
        {
            int i = idxs[j];
            if (i < 0 || i >= cnt) continue;

            var s = p.GetSurface(i);
            s.ContentType = ContentType.SCRIPT;

            long key = (tb.EntityId << 8) | (uint)i;
            surf.Add(new SurfaceRef { S = s, B = tb, K = key });
        }
    }

    tpj.Clear();
    GridTerminalSystem.GetBlocksOfType(tpj, p => HasTag(p.CustomName, cfg.Tag) && p.IsSameConstructAs(Me));
    for (int i = 0; i < tpj.Count; i++)
        if (tpj[i].IsProjecting) { pj = tpj[i]; break; }
    if (pj == null && tpj.Count > 0) pj = tpj[0];

    tcg.Clear();
    GridTerminalSystem.GetBlocksOfType(tcg, c => HasTag(c.CustomName, cfg.Tag) && c.IsSameConstructAs(Me));
    if (tcg.Count > 0)
    {
        for (int i = 0; i < tcg.Count; i++) cg.Add(tcg[i]);
    }
    else
    {
        acg.Clear();
        GridTerminalSystem.GetBlocksOfType(acg, c => c.CubeGrid == Me.CubeGrid);
        for (int i = 0; i < acg.Count; i++) cg.Add(acg[i]);
    }

    tasm.Clear();
    GridTerminalSystem.GetBlocksOfType(tasm, a => HasTag(a.CustomName, cfg.Tag) && a.IsSameConstructAs(Me));
    for (int i = 0; i < tasm.Count; i++)
        if (!IsSK(tasm[i])) asm.Add(tasm[i]);

    cacheOk = true;
}

// =================
// Blueprint parsing
// =================

int bpsPos = 0;
string[] bpsCn = null;
bool bpsRun = false;

void StartBpsParse()
{
    bps.Clear();
    bpsOk = false;
    bpsRun = false;
    bpsPos = 0;
    bpsCn = null;

    if (string.IsNullOrWhiteSpace(blockDefinitionData)) return;

    int d = blockDefinitionData.IndexOf('$');
    if (d <= 0) return;

    var cn = blockDefinitionData.Substring(0, d).Split('*');
    for (int i = 0; i < cn.Length; i++)
        cn[i] = BP_PREF + cn[i];

    bpsCn = cn;
    bpsPos = d + 1;
    bpsRun = true;
}

void StepBpsParse(int max)
{
    if (!bpsRun || bpsCn == null) return;

    string src = blockDefinitionData;
    int n = src.Length;

    for (int step = 0; step < max && bpsPos < n; step++)
    {
        int ed = src.IndexOf('$', bpsPos);
        if (ed < 0) ed = n;

        ParseBpsBlock(src, bpsPos, ed);

        bpsPos = ed + 1;
    }

    if (bpsPos >= n)
    {
        bpsRun = false;
        bpsOk = bps.Count > 0;
    }
}

void ParseBpsBlock(string s, int st, int ed)
{
    if (st >= ed) return;

    int p = s.IndexOf('*', st);
    if (p < 0 || p >= ed) return;

    string tn = "MyObjectBuilder_" + s.Substring(st, p - st);

    int a = p + 1;
    while (a < ed)
    {
        int b = s.IndexOf('*', a);
        if (b < 0 || b > ed) b = ed;

        int eq = s.IndexOf('=', a);
        if (eq > a && eq < b)
        {
            string sub = Ds(s.Substring(a, eq - a));
            string bn = tn + "/" + sub;

            Dictionary<string, int> d;
            if (!bps.TryGetValue(bn, out d))
            {
                d = new Dictionary<string, int>();
                bps[bn] = d;
            }

            ParseBpsComps(s, eq + 1, b, d);
        }

        a = b + 1;
    }
}

void ParseBpsComps(string s, int st, int ed, Dictionary<string, int> d)
{
    int p = st;

    while (p < ed)
    {
        int idx = 0;
        int amt = 0;
        bool ok = false;

        while (p < ed && s[p] >= '0' && s[p] <= '9')
        {
            idx = idx * 10 + (s[p] - '0');
            p++;
        }

        if (p < ed && s[p] == ':')
        {
            p++;

            while (p < ed && s[p] >= '0' && s[p] <= '9')
            {
                amt = amt * 10 + (s[p] - '0');
                p++;
                ok = true;
            }
        }

        if (ok && idx >= 0 && idx < bpsCn.Length)
        {
            string cdef = bpsCn[idx];

            int cur;
            if (!d.TryGetValue(cdef, out cur)) cur = 0;
            d[cdef] = cur + amt;
        }

        while (p < ed && s[p] != ',') p++;
        if (p < ed && s[p] == ',') p++;
    }
}

string Ds(string s)
{
    if (string.IsNullOrEmpty(s)) return s;

    StringBuilder b = null;

    for (int i = 0; i < s.Length; i++)
    {
        string r = null;

        switch (s[i])
        {
            case 'Б': r = "Large"; break;
            case 'Г': r = "Small"; break;
            case 'Д': r = "Block"; break;
            case 'Ж': r = "Armor"; break;
            case 'З': r = "Slope"; break;
            case 'И': r = "Corner"; break;
            case 'Й': r = "Conveyor"; break;
            case 'Л': r = "Panel"; break;
            case 'П': r = "Window"; break;
            case 'Ф': r = "RealWheel"; break;
            case 'Ц': r = "Suspension"; break;
            case 'Ч': r = "Offroad"; break;
            case 'Ш': r = "Symbol"; break;
            case 'Щ': r = "Reskin"; break;
            case 'Ы': r = "Round"; break;
            case 'Э': r = "Hydrogen"; break;
            case 'Ю': r = "Light"; break;
            case 'Я': r = "Heavy"; break;
            case 'Ъ': r = "LargeBlock"; break;
            case 'Ь': r = "SmallBlock"; break;
            case 'Є': r = "HeavyArmor"; break;
        }

        if (r != null)
        {
            if (b == null)
            {
                b = new StringBuilder(s.Length * 2);
                if (i > 0) b.Append(s, 0, i);
            }
            b.Append(r);
        }
        else if (b != null)
        {
            b.Append(s[i]);
        }
    }

    return b == null ? s : b.ToString();
}

Dictionary<string, int> GetComps(string def)
{
    Dictionary<string, int> c;
    if (bps.TryGetValue(def, out c)) return c;
    return null;
}

void AddComps(Dictionary<string, int> to, Dictionary<string, int> from, int times)
{
    if (from == null) return;

    foreach (var kv in from)
    {
        int cur;
        if (!to.TryGetValue(kv.Key, out cur)) cur = 0;
        to[kv.Key] = cur + kv.Value * times;
    }
}

// ===============================
// Required components (Projector)
// ===============================

Dictionary<string, int> Need(IMyProjector p, ArmorType at)
{
    var tot = new Dictionary<string, int>();

    foreach (var kv in p.RemainingBlocksPerType)
    {
        string def = kv.Key.ToString();
        int amt = kv.Value;

        var c = GetComps(def);
        AddComps(tot, c, amt);
    }

    if (at != ArmorType.None && p.RemainingArmorBlocks > 0)
    {
        string ad = ArmorDef(p, at);
        if (!string.IsNullOrEmpty(ad))
        {
            var ac = GetComps(ad);
            AddComps(tot, ac, p.RemainingArmorBlocks);
        }
    }

    return tot;
}

string ArmorDef(IMyProjector p, ArmorType at)
{
    bool lg = p.BlockDefinition.SubtypeId == "LargeProjector";

    if (lg)
    {
        if (at == ArmorType.Heavy) return "MyObjectBuilder_CubeBlock/LargeHeavyBlockArmorBlock";
        return "MyObjectBuilder_CubeBlock/LargeBlockArmorBlock";
    }
    else
    {
        if (at == ArmorType.Heavy) return "MyObjectBuilder_CubeBlock/SmallHeavyBlockArmorBlock";
        return "MyObjectBuilder_CubeBlock/SmallBlockArmorBlock";
    }
}

// ============================================
// Inventory counting (Cached Cargo Containers)
// ============================================

Dictionary<string, int> AvlFromCargos(Dictionary<string, int> res)
{
    res.Clear();

    for (int ci = 0; ci < cg.Count; ci++)
    {
        var c = cg[ci];
        for (int i = 0; i < c.InventoryCount; i++)
        {
            var inv = c.GetInventory(i);

            ti.Clear();
            inv.GetItems(ti);

            for (int k = 0; k < ti.Count; k++)
            {
                var it = ti[k];
                if (it.Type.TypeId != "MyObjectBuilder_Component") continue;

                string sub = it.Type.SubtypeId;
                int amt = (int)it.Amount.ToIntSafe();

                int cur;
                if (!res.TryGetValue(sub, out cur)) cur = 0;
                res[sub] = cur + amt;
            }
        }
    }

    return res;
}

// ============
// Localization
// ============

string Tr(string sub)
{
    if (string.IsNullOrWhiteSpace(sub)) return sub;

    string v;
    if (loc != null && loc.TryGetValue(sub, out v)) return v;

    const string SFX = "Component";
    if (sub.EndsWith(SFX, StringComparison.OrdinalIgnoreCase))
    {
        string ns = sub.Substring(0, sub.Length - SFX.Length);
        if (loc != null && loc.TryGetValue(ns, out v)) return v;
    }
    else
    {
        string ws = sub + SFX;
        if (loc != null && loc.TryGetValue(ws, out v)) return v;
    }

    return sub;
}

string StripBP(string bp) { return bp.Replace(BP_PREF, ""); }

// ===========================
// Rendering (SCRIPT surfaces)
// ===========================

void Icon(MySpriteDrawFrame f, string sid, Vector2 c, float sz)
{
    var sp = new MySprite(SpriteType.TEXTURE, sid);
    sp.Position = c;
    sp.Size = new Vector2(sz, sz);
    sp.Color = Color.White;
    f.Add(sp);
}

void BG(IMyTextSurface s, MySpriteDrawFrame f)
{
    var bg = new MySprite(SpriteType.TEXTURE, SQ);
    bg.Color = s.ScriptBackgroundColor;
    bg.Position = s.TextureSize / 2f;
    bg.Size = s.TextureSize;
    f.Add(bg);
}

void DT(MySpriteDrawFrame f, string t, Vector2 p, Color c, float sc, TextAlignment al, string font)
{
    var sp = MySprite.CreateText(t, font, c, sc, al);
    sp.Position = p;
    f.Add(sp);
}
void DT(MySpriteDrawFrame f, string t, Vector2 p, Color c, float sc) { DT(f, t, p, c, sc, TextAlignment.LEFT, "Debug"); }
void DT(MySpriteDrawFrame f, string t, Vector2 p, Color c, float sc, TextAlignment al) { DT(f, t, p, c, sc, al, "Debug"); }

void DrawPbSplash()
{
    var tsp = Me as IMyTextSurfaceProvider;
    if (tsp == null || tsp.SurfaceCount <= 0) return;

    var s = tsp.GetSurface(0);
    s.ContentType = ContentType.SCRIPT;

    string top = "PRC Script";
    string bot = "Project Required Components";

    using (var f = s.DrawFrame())
    {
        var bg = new MySprite(SpriteType.TEXTURE, SQ);
        bg.Color = PB_SPLASH_BG;
        bg.Position = s.TextureSize / 2f;
        bg.Size = s.TextureSize;
        f.Add(bg);

        var vp = new RectangleF((s.TextureSize - s.SurfaceSize) / 2f, s.SurfaceSize);

        float pad = 14f;
        float w = Math.Max(1f, vp.Width - pad * 2f);

        float bsc = FitW(s, bot, "Monospace", w);
        float tsc = bsc * 2.25f;

        Vector2 tsz = s.MeasureStringInPixels(new StringBuilder(top), "Monospace", tsc);
        Vector2 bsz = s.MeasureStringInPixels(new StringBuilder(bot), "Monospace", bsc);

        float gap = Math.Max(5f, bsz.Y * 0.30f);
        float ulg = Math.Max(40f, bsz.Y * 0.10f);
        float ulh = Math.Max(2f, bsz.Y * 0.18f);

        Vector2 ch = s.MeasureStringInPixels(new StringBuilder("M"), "Monospace", tsc);
        float ulw = tsz.X + (ch.X * 0.5f * 2f);

        float cx = vp.Position.X + vp.Width * 0.50f;
        float cy = vp.Position.Y + vp.Height * 0.44f;

        float d1 = (tsz.Y * 0.5f) + ulg + (ulh * 0.5f);
        float d2 = (ulh * 0.5f) + gap + (bsz.Y * 0.5f);

        float uly = cy + (d1 - d2) * 0.5f;
        float ty = uly - d1;
        float by = uly + d2;

        DT(f, top, new Vector2(cx, ty), PB_SPLASH_FG, tsc, TextAlignment.CENTER, "Monospace");

        var ul = new MySprite(SpriteType.TEXTURE, SQ);
        ul.Color = PB_SPLASH_FG;
        ul.Size = new Vector2(ulw, ulh);
        ul.Position = new Vector2(cx * 0.99f, uly);
        f.Add(ul);

        DT(f, bot, new Vector2(cx, by), PB_SPLASH_FG, bsc, TextAlignment.CENTER, "Monospace");
    }
}

float FitW(IMyTextSurface s, string t, string font, float tw)
{
    if (string.IsNullOrEmpty(t)) return 1f;
    Vector2 sz = s.MeasureStringInPixels(new StringBuilder(t), font, 1.0f);
    float w = Math.Max(1f, sz.X);
    float sc = (tw * 0.98f) / w;
    if (sc < 0.2f) sc = 0.2f;
    if (sc > 10f) sc = 10f;
    return sc;
}

void DrawMsg(List<SurfaceRef> ss, string msg)
{
    for (int i = 0; i < ss.Count; i++)
    {
        var s = ss[i].S;
        s.ContentType = ContentType.SCRIPT;

        using (var f = s.DrawFrame())
        {
            BG(s, f);
            var vp = new RectangleF((s.TextureSize - s.SurfaceSize) / 2f, s.SurfaceSize);
            DT(f, msg, vp.Position + new Vector2(10f, 10f), s.ScriptForegroundColor, cfg.FontScale);
        }
    }
}

void DrawTable(List<SurfaceRef> ss, string title, List<Row> rs)
{
    const float BIAS = 0.06f;

    for (int si = 0; si < ss.Count; si++)
    {
        var sr = ss[si];
        var s = sr.S;
        s.ContentType = ContentType.SCRIPT;

        using (var f = s.DrawFrame())
        {
            BG(s, f);

            Color fg = s.ScriptForegroundColor;
            Color ht = cfg.HighlightTextColor;
            Color hb = cfg.HighlightBgColor;
            float sc = cfg.FontScale;

            float pad = 10f;
            var vp = new RectangleF((s.TextureSize - s.SurfaceSize) / 2f, s.SurfaceSize);
            Vector2 tl = vp.Position + new Vector2(pad, pad);
            float w = vp.Width - pad * 2f;

            var lm = s.MeasureStringInPixels(sbAg, "Debug", sc);
            float lh = lm.Y * 1.0f;
            float tlh = lm.Y * 1.5f;
            float rh = lh * 1.2f;

            float xI = tl.X;
            float iW = Math.Max(rh * 0.9f, rh * cfg.IconSize);
            float xN = xI + iW + 6f;

            float gap = lh * 0.75f;

            float xr = tl.X + w;

            sb999.Clear(); sb999.Append(TITLE_NEEDED);
            float wReq = s.MeasureStringInPixels(sb999, "Debug", sc).X;

            sb999.Clear(); sb999.Append(TITLE_AVAILABLE);
            float wAvl = s.MeasureStringInPixels(sb999, "Debug", sc).X;

            sb999.Clear(); sb999.Append(TITLE_MISSING);
            float wMis = s.MeasureStringInPixels(sb999, "Debug", sc).X;

            float yBody0 = tl.Y + tlh + lh + (lh * 0.25f);
            float yMax0 = vp.Bottom - rh;

            float yy = yBody0;
            for (int i = 0; i < rs.Count; i++)
            {
                if (yy > yMax0) break;
                var r0 = rs[i];

                sb999.Clear(); sb999.Append(Fmt(r0.Req));
                wReq = Math.Max(wReq, s.MeasureStringInPixels(sb999, "Debug", sc).X);

                sb999.Clear(); sb999.Append(Fmt(r0.Avl));
                wAvl = Math.Max(wAvl, s.MeasureStringInPixels(sb999, "Debug", sc).X);

                sb999.Clear(); sb999.Append(Fmt(r0.Mis));
                wMis = Math.Max(wMis, s.MeasureStringInPixels(sb999, "Debug", sc).X);

                yy += rh;
            }

            wReq += 2f; wAvl += 2f; wMis += 2f;

            float xMis = xr;
            float xAvl = xr;
            float xReq = xr;

            if (cfg.ShowMis)
            {
                xMis = xr;
               xr = xMis - (wMis + gap);
            }

            if (cfg.ShowAvl)
            {
                xAvl = xr;
                xr = xAvl - (wAvl + gap);
            }

            xReq = xr;

            xReq = Math.Max(xReq, xN + 40f);

            int pp = (int)((vp.Bottom - rh - yBody0) / rh);
            if (pp < 1) pp = 1;

            int pc = (rs.Count + pp - 1) / pp;
            if (pc < 1) pc = 1;

            int lpg = pg;
            if (lpg >= pc) lpg = pc - 1;
            if (lpg < 0) lpg = 0;

            if (si == 0)
            {
                float pgs = cfg.PgSec;
                if (pgs > 0f && pc > 1 && pgAcc >= pgs)
                {
                    int step = (int)(pgAcc / pgs);
                    pgAcc -= step * pgs;
                    pg = (pg + step) % pc;
                }
                if (pg >= pc) pg = pc - 1;
                if (pg < 0) pg = 0;

                lpg = pg;
            }

            int st = lpg * pp;
            int ed = Math.Min(rs.Count, st + pp);

            float y = tl.Y;

            sb999.Clear();
			sb999.Append(TITLE).Append(" ( ").Append(lpg + 1).Append("/").Append(pc).Append(" )");
			DT(f, sb999.ToString(), new Vector2(tl.X, y), fg, sc * 1.3f);

            y += tlh;

            DT(f, TITLE_NEEDED, new Vector2(xReq, y), fg, sc, TextAlignment.RIGHT);
            if (cfg.ShowAvl) DT(f, TITLE_AVAILABLE, new Vector2(xAvl, y), fg, sc, TextAlignment.RIGHT);
            if (cfg.ShowMis) DT(f, TITLE_MISSING, new Vector2(xMis, y), fg, sc, TextAlignment.RIGHT);

            y += lh;

            var sep = new MySprite(SpriteType.TEXTURE, SQ);
            sep.Color = fg;
            sep.Size = new Vector2(w, 1f);
            sep.Position = new Vector2(tl.X + w / 2f, y);
            f.Add(sep);

            y += lh * 0.25f;

            for (int i = st; i < ed; i++)
            {
                var r = rs[i];
                if (y > vp.Bottom - rh) break;

                bool mis = r.Mis > 0;
                Color rc = mis ? ht : fg;

                float isz = rh * cfg.IconSize * 1.0f;
				float cy = y + (rh * 0.5f);
				float ty = cy - (lh * 0.5f) + (lh * BIAS);

                if (mis && cfg.HighlightBg)
                {
                    float bh = Math.Max(1f, isz - 4f);
                    var br = new MySprite(SpriteType.TEXTURE, SQ);
                    br.Color = new Color(hb.R, hb.G, hb.B, cfg.HighlightBgAlpha);
                    br.Size = new Vector2(w, bh);
                    br.Position = new Vector2(tl.X + w / 2f, cy + 2f);
                    f.Add(br);
                }

                string sid;
                Vector2 ic = new Vector2(xI + iW * 0.5f, cy + 2f);

                if (TryGetSpriteId(sr, r.Sub, out sid))
                {
                    Icon(f, sid, ic, isz);

                    if (qsub.Contains(r.Sub) && r.Mis > 0)
                    {
                        float osz = isz / 1.75f;
                        Vector2 oc = ic + new Vector2(isz * 0.3f, isz * 0.185f);

                        var q = new MySprite(SpriteType.TEXTURE, "Construction");
                        q.Position = oc;
                        q.Size = new Vector2(osz, osz);
                        q.Color = Color.White;
                        f.Add(q);
                    }
                }

                DT(f, r.Name, new Vector2(xN, ty), rc, sc);
                DT(f, Fmt(r.Req), new Vector2(xReq, ty), rc, sc, TextAlignment.RIGHT);
                if (cfg.ShowAvl) DT(f, Fmt(r.Avl), new Vector2(xAvl, ty), rc, sc, TextAlignment.RIGHT);
                if (cfg.ShowMis) DT(f, Fmt(r.Mis), new Vector2(xMis, ty), rc, sc, TextAlignment.RIGHT);

                y += rh;
            }
        }
    }
}

// ===============================
// Queue logic (Cached Assemblers)
// ===============================

bool IsSK(IMyAssembler a)
{
    string sub = a.BlockDefinition.SubtypeId ?? "";
    return sub.IndexOf("SurvivalKit", StringComparison.OrdinalIgnoreCase) >= 0;
}

void QueueNoDup(List<Row> rs)
{
    if (asm.Count == 0) return;

    var plan = new Dictionary<MyDefinitionId, MyFixedPoint>();

    for (int i = 0; i < rs.Count; i++)
    {
        var r = rs[i];
        if (r.Mis <= 0) continue;

        var bp = MyDefinitionId.Parse(BP_PREF + r.Sub);
        plan[bp] = (MyFixedPoint)r.Mis;
    }

    if (plan.Count == 0) return;

    var qt = QT(asm);

    var keys = new List<MyDefinitionId>(plan.Keys);
    for (int i = 0; i < keys.Count; i++)
    {
        var bp = keys[i];
        MyFixedPoint q;
        if (qt.TryGetValue(bp, out q))
        {
            var rem = plan[bp] - q;
            if (rem <= 0) plan.Remove(bp);
            else plan[bp] = rem;
        }
    }

    if (plan.Count == 0) return;

    int idx = 0;
    foreach (var kv in plan)
    {
        var bp = kv.Key;

        int tot = (int)Math.Ceiling((double)(decimal)kv.Value);
        if (tot <= 0) continue;

        int cnt = asm.Count;
        int b = tot / cnt;
        int ex = tot % cnt;

        for (int i = 0; i < cnt; i++)
        {
            var a = asm[(idx + i) % cnt];
            int add = b + (i < ex ? 1 : 0);
            if (add > 0) a.AddQueueItem(bp, (MyFixedPoint)add);
        }

        idx++;
    }
}

Dictionary<MyDefinitionId, MyFixedPoint> QT(List<IMyAssembler> asms)
{
    var t = new Dictionary<MyDefinitionId, MyFixedPoint>();

    for (int i = 0; i < asms.Count; i++)
    {
        tq.Clear();
        asms[i].GetQueue(tq);

        for (int j = 0; j < tq.Count; j++)
        {
            var it = tq[j];
            var id = it.BlueprintId;

            MyFixedPoint cur;
            if (!t.TryGetValue(id, out cur)) cur = 0;
            t[id] = cur + it.Amount;
        }
    }

    return t;
}

bool BuildQSub(HashSet<string> q)
{
    q.Clear();

    for (int i = 0; i < asm.Count; i++)
    {
        tq.Clear();
        asm[i].GetQueue(tq);

        for (int j = 0; j < tq.Count; j++)
        {
            string id = tq[j].BlueprintId.ToString();

            if (!id.StartsWith(BP_PREF, StringComparison.OrdinalIgnoreCase))
                continue;

            q.Add(id.Substring(BP_PREF.Length));
        }
    }

    return q.Count > 0;
}

// =============
// Small helpers
// =============

void ResetIconCache()
{
	rcache.Clear();
    imap.Clear();
}

void ResetAll(bool clearStorage)
{
    if (clearStorage) Storage = "";

    ResetIconCache();

    am.Clear();

    cdHash = 0;

    InvalidateCache();
}

bool HasTag(string n, string tag)
{
    if (string.IsNullOrEmpty(tag) || n == null) return false;
    return n.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0;
}

int GetI(Dictionary<string, int> d, string k)
{
    int v;
    if (d.TryGetValue(k, out v)) return v;
    return 0;
}

string Fmt(int v) { return v.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture); }

uint Fnv1a32(string s)
{
    unchecked
    {
        uint h = 2166136261u;
        for (int i = 0; i < s.Length; i++) { h ^= s[i]; h *= 16777619u; }
        return h;
    }
}

bool TryParseHexColor(string hex, out Color c)
{
    c = Color.White;
    if (string.IsNullOrWhiteSpace(hex)) return false;

    var s = hex.Trim();
    if (s.StartsWith("#")) s = s.Substring(1);

    if (s.Length == 6)
    {
        byte r, g, b;
        if (!HexB(s, 0, out r)) return false;
        if (!HexB(s, 2, out g)) return false;
        if (!HexB(s, 4, out b)) return false;
        c = new Color(r, g, b);
        return true;
    }

    if (s.Length == 8)
    {
        byte a, r, g, b;
        if (!HexB(s, 0, out a)) return false;
        if (!HexB(s, 2, out r)) return false;
        if (!HexB(s, 4, out g)) return false;
        if (!HexB(s, 6, out b)) return false;
        c = new Color(r, g, b, a);
        return true;
    }

    return false;
}

bool HexB(string s, int i, out byte b)
{
    b = 0;
    if (i + 2 > s.Length) return false;

    int v;
    if (!int.TryParse(s.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out v))
        return false;

    b = (byte)v;
    return true;
}

string BuildMisText(List<Row> rs)
{
    var sb = new StringBuilder();

    for (int i = 0; i < rs.Count; i++)
    {
        var r = rs[i];
        if (r.Mis <= 0) continue;

        if (sb.Length > 0) sb.Append('\n');

        sb.Append(r.Name);
        sb.Append(" : ");
        sb.Append(Fmt(r.Mis));
    }

    if (sb.Length == 0) return "PRC OK";

    return sb.ToString();
}

void UpdateMisText(List<SurfaceRef> ss, List<Row> rs)
{
    string t = BuildMisText(rs);
    string ttl = t == "PRC OK" ? "PRC OK" : "PRC Missing Components";

    if (t == lastMisText && ttl == lastMisTitle) return;

    lastMisText = t;
    lastMisTitle = ttl;

    for (int i = 0; i < ss.Count; i++)
    {
        var p = ss[i].B as IMyTextPanel;
        if (p == null) continue;

        p.WritePublicTitle(ttl, false);
        p.WriteText(t, false);
    }
}

// ==========================
// Persistent state (Storage)
// ==========================

void LoadPS(string st)
{
    imap.Clear();
    rcache.Clear();
    am.Clear();

    cdHash = 0;

    if (string.IsNullOrWhiteSpace(st)) return;

    var ls = st.Split('\n');
    for (int i = 0; i < ls.Length; i++)
    {
        var ln = ls[i].Trim();
        if (ln.Length == 0) continue;

        if (ln.StartsWith("CDHASH=", StringComparison.OrdinalIgnoreCase))
        { uint v; if (uint.TryParse(ln.Substring(7).Trim(), out v)) cdHash = v; continue; }

        if (ln.StartsWith("M|", StringComparison.OrdinalIgnoreCase))
        {
            var p = ln.Split('|');
            if (p.Length != 2) continue;

            long id;
            if (long.TryParse(p[1], out id)) am.Add(id);
            continue;
        }

        if (ln.StartsWith("I|", StringComparison.OrdinalIgnoreCase))
        {
            var p = ln.Split('|');
            if (p.Length != 4) continue;

            long k;
            if (!long.TryParse(p[1], out k)) continue;

            string sub = p[2];
            string sid = p[3];

            Dictionary<string, string> m;
            if (!imap.TryGetValue(k, out m))
            {
                m = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                imap[k] = m;
            }
            m[sub] = sid;
            continue;
        }
    }
}

string BuildPS()
{
    var sb = new StringBuilder(4096);

    sb.Append("CDHASH=").Append(cdHash).Append('\n');

    foreach (var id in am) sb.Append("M|").Append(id).Append('\n');

    foreach (var kv in imap)
    {
        long k = kv.Key;
        var m = kv.Value;
        if (m == null) continue;

        foreach (var e in m)
        {
            string sub = e.Key ?? "";
            string sid = e.Value ?? "";
            sb.Append("I|").Append(k).Append('|').Append(sub).Append('|').Append(sid).Append('\n');
        }
    }

    return sb.ToString();
}

// =============================================
// User-provided BlockDefinitionExtractor output
// =============================================

string blockDefinitionData = "SteelPlate*ConstructionComponent*InteriorPlate*ComputerComponent*MotorComponent*SmallTube*LargeTube*BulletproofGlass*MetalGrid*GirderComponent*Display*RadioCommunicationComponent*DetectorComponent*Superconductor*GravityGeneratorComponent*PowerCell*ThrustComponent*MedicalComponent*ReactorComponent*SolarCell*ExplosivesComponent*ZoneChip*PrototechFrame*PrototechPanel*PrototechCoolingUnit*PrototechCircuitry*PrototechMachinery*PrototechCapacitor*PrototechPropulsionUnit*EngineerPlushie*EngineerPlushieSE2*SabiroidPlushie$AirVent*AirVentFan=0:30,1:20,3:5,4:10*AirVentFanFull=0:45,1:30,3:5,4:10*ГAirVentFan=0:3,1:10,3:5,4:2*ГAirVentFanFull=0:5,1:15,3:5,4:2*(null)=0:30,1:20,3:5,4:10*AirVentFull=0:45,1:30,3:5,4:10*ГAirVent=0:3,1:10,3:5,4:2*ГAirVentFull=0:5,1:15,3:5,4:2$AirtightHangarDoor*(null)=0:350,1:40,3:2,4:16,5:40*AirtightHangarDoorWarfare2A=0:350,1:40,3:2,4:16,5:40*AirtightHangarDoorWarfare2B=0:350,1:40,3:2,4:16,5:40*AirtightHangarDoorWarfare2C=0:350,1:40,3:2,4:16,5:40$AirtightSlideDoor*ЪSlideDoor=0:20,1:40,3:2,4:4,5:4,7:15,10:1$Assembler*БAssemblerIndustrial=0:140,1:80,3:160,4:20,8:10,10:10*БAssembler=0:140,1:80,3:160,4:20,8:10,10:10*BasicAssembler=0:80,1:40,3:80,4:10,10:4*FoodProcessor=0:30,1:40,3:20,4:4,7:10,10:2*БPrototechAssembler=1:130,3:200,8:80,10:10,22:1,23:240,24:2,26:20$BasicMissionBlock*БBasicMission=0:20,1:30,2:20,3:20,4:4,12:20*ГBasicMission=0:2,1:5,2:2,3:10,4:2,12:4$BatteryBlock*ЪBatteryД=0:80,1:30,3:25,15:80*ЬBatteryД=0:25,1:5,3:2,15:20*ЬГBatteryД=0:4,1:2,3:2,15:2*ЪPrototechBattery=1:30,3:25,8:16,22:1,23:60,25:3,27:20*ЬPrototechBattery=1:5,3:2,8:4,22:1,23:6,25:1,27:6*ЪBatteryДWarfare2=0:80,1:30,3:25,15:80*ЬBatteryДWarfare2=0:25,1:5,3:2,15:20$Beacon*ЪBeacon=0:80,1:30,3:10,6:20,11:40*ЬBeacon=0:2,1:1,3:1,5:1,11:4*ЪBeaconЩ=0:80,1:30,3:10,6:20,11:40*ЬBeaconЩ=0:2,1:1,3:1,5:1,11:4$BroadcastController*ЪBroadcastController=1:30,2:10,3:10,10:4,11:5*ЬBroadcastController=1:3,2:2,3:2,10:1,11:1$ButtonPanel*ButtonЛБ=1:20,2:10,3:20*ButtonЛГ=1:2,2:2,3:1*БButtonЛPedestal=1:10,2:5,3:5*ГButtonЛPedestal=1:10,2:5,3:5*ЪModularBridgeButtonЛ=0:10,1:5,3:5,7:30,9:6*ЪInsetButtonЛ=1:20,2:20,3:20,10:10*ЪAccessЛ3=1:8,2:3,3:3*VerticalButtonЛБ=1:10,2:5,3:5*VerticalButtonЛГ=1:10,2:5,3:5*ЪConsoleModuleButtons=1:50,2:50,3:10,10:6*ЬConsoleModuleButtons=1:6,2:6,3:2*БSciFiButtonTerminal=1:10,2:5,3:4,10:4*БSciFiButtonЛ=1:20,2:10,3:20,10:5$CameraBlock*БCameraTopMounted=0:2,3:3*ГCameraTopMounted=0:2,3:3*ГCameraД=0:2,3:3*БCameraД=0:2,3:3$CargoContainer*ЬModularContainer=1:20,2:50,3:5,4:6,10:1*ЪLockerRoom=1:30,2:30,7:10,10:4*ЪLockerRoomИ=1:30,2:25,7:10,10:4*ЪLockers=1:20,2:20,3:2,10:3*ЪInsetBookshelf=1:30,2:30*ЪBulkContainerA=1:160,2:720,3:10,4:40,5:120,8:50,10:2*ЪBulkContainerB=1:160,2:720,3:10,4:40,5:120,8:50,10:2*ЪBulkContainerC=1:160,2:720,3:10,4:40,5:120,8:50,10:2*ЪCargoTerminal=1:40,2:40,3:2,4:4,5:20,8:4,10:1*ЪCargoTerminalHalf=1:30,2:30,3:2,4:4,5:10,8:2,10:1*ЪLabИDesk=1:30,2:30,12:4*ЪLabCabinet=1:30,2:20,4:4,5:8,7:2,10:1*ЪБIndustrialContainer=1:80,2:360,3:8,4:20,5:60,8:24,10:1*ЬГContainer=1:1,2:3,3:1,4:1,10:1*ЬMediumContainer=1:10,2:30,3:4,4:4,10:1*ЬБContainer=1:25,2:75,3:6,4:8,10:1*ЪГContainer=1:40,2:40,3:2,4:4,5:20,8:4,10:1*ЪБContainer=1:80,2:360,3:8,4:20,5:60,8:24,10:1*ЪWeaponRack=1:20,2:30*ЬWeaponRack=1:3,2:3$Cockpit*ЪModularBridgeCockpit=0:15,1:15,2:30,3:100,4:1,7:30,10:4*ЪCaptainDesk=1:50,2:50,3:6,10:4*ЪCockpit=1:20,2:20,3:100,4:2,10:10*ЪCockpitSeat=0:30,1:20,3:100,4:1,7:60,10:8*ЬCockpit=0:10,1:10,3:15,4:1,7:30,10:5*DBЬFighterCockpit=0:20,1:20,2:15,3:20,4:1,7:40,8:10,10:4*CockpitOpen=1:20,2:20,3:100,4:2,10:4*RoverCockpit=1:25,2:30,3:20,4:2,10:4*OpenCockpitГ=1:20,2:20,3:15,4:1,10:2*OpenCockpitБ=1:30,2:30,3:100,4:2,10:6*ЬFlushCockpit=0:20,1:20,3:20,4:2,7:40,10:5*ЪSuspendedControlSeat=1:30,2:30,3:20,4:2,10:6*ЪSuspendedControlSeatB=1:30,2:30,3:20,4:2,10:2*ЬSuspendedControlSeat=1:20,2:20,3:15,4:1,10:4*ЬSuspendedControlSeatB=1:20,2:20,3:15,4:1,10:2*ЪDesk=1:30,2:30*ЪDeskИ=1:20,2:20*ЪDeskИInv=1:60,2:60*ЪCouch=1:30,2:30*ЪCouchИ=1:35,2:35*ЪBathroomOpen=1:30,2:30,4:4,5:8,6:2*ЪBathroom=1:40,2:30,4:4,5:8,6:2*ЪToilet=1:15,2:10,4:2,5:2,6:1*ЬCockpitIndustrial=0:10,1:20,3:20,4:2,5:10,7:60,8:10,10:6*ЪCockpitIndustrial=0:20,1:30,3:60,4:2,5:10,7:80,8:15,10:10*ЬCapCockpit=0:20,1:10,3:15,4:1,7:10,10:4*ЪInsetPlantCouch=1:30,2:30,5:10,7:20*ЪLabDeskSeat=1:30,2:30,3:6,10:4*SpeederCockpit=1:25,2:30,3:20,4:2,10:4*SpeederCockpitCompact=1:25,2:30,3:20,4:2,10:4*PassengerSeatБ=1:20,2:20*PassengerSeatГ=1:20,2:20*PassengerSeatГNew=1:20,2:20*PassengerSeatГOffset=1:20,2:20*BuggyCockpit=1:25,2:30,3:20,4:2,10:4*ЪConsoleModuleInvertedИ=1:80,2:80*ЪConsoleModuleScreens=1:50,2:50,3:10,10:12*PassengerBench=1:20,2:20*ЬStandingCockpit=1:20,2:20,3:20,4:1,10:2*ЪStandingCockpit=1:20,2:20,3:20,4:1,10:2$Collector*Collector=0:45,1:50,3:10,4:8,5:12,10:4*CollectorГ=0:35,1:35,3:8,4:8,5:12,10:2$ContractBlock*ContractД=0:30,1:20,3:10,4:6,10:4$Conveyor*ЪЙPipeJunction=1:30,2:20,4:6,5:20*ЪЙPipeIntersection=1:20,2:18,4:6,5:16*ЪЙPipeT=1:24,2:16,4:6,5:14*ЬЙ=1:4,2:4,4:1*ЬЙConverter=1:8,2:6,4:2,5:6*ЪЙ=1:30,2:20,4:6,5:20*ЙTubeDuctT=0:22,1:24,2:16,4:6,5:14*ЙTubeDuctГT=0:2,1:2,2:2,4:1*ГShipЙHub=1:20,2:15,4:2,5:15*ЙTubeГT=1:2,2:2,4:1*ЙTubeT=1:24,2:16,4:6,5:14$ConveyorConnector*ЪЙPipeSeamless=1:20,2:14,4:6,5:12*ЪЙPipeИ=1:20,2:14,4:6,5:12*ЪЙPipeFlange=1:20,2:14,4:6,5:12*ЪЙPipeEnd=1:20,2:14,4:6,5:12*ЙTube=1:20,2:14,4:6,5:12*ЙTubeDuct=0:25,1:20,2:14,4:6,5:12*ЙTubeDuctCurved=0:25,1:20,2:14,4:6,5:12*ЙTubeГ=1:1,2:1,4:1*ЙTubeDuctГ=0:2,1:1,2:1,4:1*ЙTubeDuctГCurved=0:2,1:1,2:1,4:1*ЙTubeMedium=1:20,2:10,4:6,5:10*ЙFrameMedium=1:12,2:5,4:2,5:5*ЙTubeCurved=1:20,2:14,4:6,5:12*ЙTubeГCurved=1:1,2:1,4:1*ЙTubeCurvedMedium=1:20,2:7,4:6,5:10$ConveyorSorter*ЪЙSorterIndustrial=1:120,2:50,3:20,4:2,5:50*ЪЙSorter=1:120,2:50,3:20,4:2,5:50*MediumДЙSorter=1:12,2:5,3:5,4:2,5:5*ЬЙSorter=1:12,2:5,3:5,4:2,5:5$CryoChamber*ЬBunkBed=1:15,2:20,5:5*ЪBed=1:30,2:30,5:8,7:10*ЪCryoRoom=1:20,2:40,3:30,4:8,7:10,10:8,17:3*ЪHalfBed=1:16,2:14,5:6,10:3*ЪHalfBedOffset=1:16,2:14,5:6,10:3*ЪInsetBed=1:30,2:60,5:8*ЪCryoLabVat=1:10,2:30,3:30,4:8,7:20,17:3*ЪCryoChamber=1:20,2:40,3:30,4:8,7:10,10:8,17:3*ЪBedFree=1:30,2:20,5:8*ЬCryoChamber=1:10,2:20,3:15,4:4,7:5,10:4,17:3$CubeBlock*ЪConduitBoxes=1:20,5:20*ЪConduitBoxesInv=1:20,5:20*ЪConduitИ=1:6,5:10*ЪConduitDown=1:2,5:1*ЪConduitUp=1:20,5:40*ЪConduitJunction=1:15,5:20*ЪConduitJunctionInv=1:15,5:20*ЪConduitJunctionИ=1:20,5:40*ЪConduitStraight=1:10,5:20*ЪConduitTransition=1:10,5:30*ЪConduitTransitionInv=1:10,5:30*БStorageBin1=1:5,2:5*ГStorageBin1=1:5,2:5*БStorageBin2=1:15,2:15*БStorageBin3=1:40,2:40*БWarningSign14=1:4,2:4*БWarningSign15=1:4,2:4*БWarningSign16=1:4,2:2*ГWarningSign14=1:2,2:1*ГWarningSign15=1:2,2:1*ГWarningSign16=1:4,2:2*ЪЖД=0:25*БЯДЖД=0:150,8:50*БЖЛЮ=0:5*БЖCenterЛЮ=0:5*БЖЗdSideЛЮ=0:3*БЖЗdЛЮ=0:6*БЖHalfЛЮ=0:3*БЖHalfCenterЛЮ=0:3*БЖQuarterЛЮ=0:2*БЖ2x1ЗdЛЮ=0:5*БЖ2x1ЗdЛTipЮ=0:5*БЖ2x1ЗdSideBaseЛЮ=0:5*БЖ2x1ЗdSideTipЛЮ=0:3*БЖ2x1ЗdSideBaseЛЮInv=0:5*БЖ2x1ЗdSideTipЛЮInv=0:3*БЖHalfЗdЛЮ=0:4*БЖ2x1HalfЗdЛЮRight=0:3*БЖ2x1HalfЗdTipЛЮRight=0:3*БЖ2x1HalfЗdЛЮLeft=0:3*БЖ2x1HalfЗdTipЛЮLeft=0:3*БЫЖЛЮ=0:8*БЫЖЛИЮ=0:5*БЫЖЛFaceЮ=0:4*БЫЖЛInvertedИЮ=0:5*БЖЛЯ=0:15,8:5*БЖCenterЛЯ=0:15,8:5*БЖЗdSideЛЯ=0:8,8:3*БЖЗdЛЯ=0:21,8:7*БЖHalfЛЯ=0:8,8:3*БЖHalfCenterЛЯ=0:8,8:3*БЖQuarterЛЯ=0:5,8:2*БЖ2x1ЗdЛЯ=0:18,8:6*БЖ2x1ЗdЛTipЯ=0:18,8:6*БЖ2x1ЗdSideBaseЛЯ=0:12,8:4*БЖ2x1ЗdSideTipЛЯ=0:6,8:2*БЖ2x1ЗdSideBaseЛЯInv=0:12,8:4*БЖ2x1ЗdSideTipЛЯInv=0:6,8:2*БЖHalfЗdЛЯ=0:9,8:3*БЖ2x1HalfЗdЛЯRight=0:9,8:3*БЖ2x1HalfЗdTipЛЯRight=0:9,8:3*БЖ2x1HalfЗdЛЯLeft=0:9,8:3*БЖ2x1HalfЗdTipЛЯLeft=0:9,8:3*БЫЖЛЯ=0:24,8:8*БЫЖЛИЯ=0:15,8:5*БЫЖЛFaceЯ=0:12,8:4*БЫЖЛInvertedИЯ=0:15,8:5*ГЖЛЮ=0:1*ГЖCenterЛЮ=0:1*ГЖЗdSideЛЮ=0:1*ГЖЗdЛЮ=0:1*ГЖHalfЛЮ=0:1*ГЖHalfCenterЛЮ=0:1*ГЖQuarterЛЮ=0:1*ГЖ2x1ЗdЛЮ=0:1*ГЖ2x1ЗdЛTipЮ=0:1*ГЖ2x1ЗdSideBaseЛЮ=0:1*ГЖ2x1ЗdSideTipЛЮ=0:1*ГЖ2x1ЗdSideBaseЛЮInv=0:1*ГЖ2x1ЗdSideTipЛЮInv=0:1*ГЖHalfЗdЛЮ=0:1*ГЖ2x1HalfЗdЛЮRight=0:1*ГЖ2x1HalfЗdTipЛЮRight=0:1*ГЖ2x1HalfЗdЛЮLeft=0:1*ГЖ2x1HalfЗdTipЛЮLeft=0:1*ГЫЖЛЮ=0:1*ГЫЖЛИЮ=0:1*ГЫЖЛFaceЮ=0:1*ГЫЖЛInvertedИЮ=0:1*ГЖЛЯ=0:3,8:1*ГЖCenterЛЯ=0:3,8:1*ГЖЗdSideЛЯ=0:2,8:1*ГЖЗdЛЯ=0:3,8:1*ГЖHalfЛЯ=0:2,8:1*ГЖHalfCenterЛЯ=0:2,8:1*ГЖQuarterЛЯ=0:2,8:1*ГЖ2x1ЗdЛЯ=0:3,8:1*ГЖ2x1ЗdЛTipЯ=0:3,8:1*ГЖ2x1ЗdSideBaseЛЯ=0:3,8:1*ГЖ2x1ЗdSideTipЛЯ=0:2,8:1*ГЖ2x1ЗdSideBaseЛЯInv=0:3,8:1*ГЖ2x1ЗdSideTipЛЯInv=0:2,8:1*ГЖHalfЗdЛЯ=0:2,8:1*ГЖ2x1HalfЗdЛЯRight=0:2,8:1*ГЖ2x1HalfЗdTipЛЯRight=0:2,8:1*ГЖ2x1HalfЗdЛЯLeft=0:2,8:1*ГЖ2x1HalfЗdTipЛЯLeft=0:2,8:1*ГЫЖЛЯ=0:3,8:1*ГЫЖЛИЯ=0:2,8:1*ГЫЖЛFaceЯ=0:3,8:1*ГЫЖЛInvertedИЯ=0:3,8:1*ЪModularBridgeИ=0:20,7:20,9:5*ЪModularBridgeИFloorless=0:20,7:20,9:5*ЪModularBridgeRaisedЗdИ=0:24,7:20,9:5*ЪModularBridgeRaisedЗdИFloorless=0:24,7:20,9:5*ЪModularBridgeHalfЗdИ=0:16,7:20,9:5*ЪModularBridgeHalfЗdИFloorless=0:16,7:20,9:5*ЪModularBridgeИ2x1BaseL=0:10,7:10,9:4*ЪModularBridgeИ2x1BaseLFloorless=0:10,7:10,9:4*ЪModularBridgeИ2x1BaseR=0:10,7:10,9:4*ЪModularBridgeИ2x1BaseRFloorless=0:10,7:10,9:4*ЪModularBridgeEmpty=0:10,7:30,9:6*ЪModularBridgeFloor=0:10,7:30,9:6*ЪModularBridgeSideL=0:8,7:10,9:4*ЪModularBridgeSideR=0:8,7:10,9:4*ЪModularBridgeЗdИBase=0:20,7:20,9:5*ЪModularBridgeЗdИBaseFloorless=0:20,7:20,9:5*ЬKitchenSink=1:6,2:4,5:2*ЬKitchenCoffeeMachine=1:6,2:4,4:1*ЪЫEdge=1:10,2:25*ЪЫEdgeB=1:10,2:25*ЪЫEdgeC=1:10,2:25*ЪЫEdgeИ=1:7,2:20*ЪЫEdgeИB=1:7,2:20*ЪЫEdgeИInv=1:10,2:25*ЪЫEdgeИInvB=1:10,2:25*ЪЫHalfEdge=1:6,2:15*ЪЫHalfEdgeИ=1:5,2:10*ЪЫHalfEdgeИB=1:5,2:10*ЪЫHalfEdgeИInv=1:7,2:20*ЪЫHalfEdgeИInvB=1:7,2:20*ЪЫEdgeЗBase=1:7,2:20*ЪЫEdgeЗTip=1:6,2:15*ЬЫEdge=1:3,2:5*ЬЫEdgeИ=1:2,2:4*ЬЫEdgeИB=1:2,2:4*ЬЫEdgeИInv=1:3,2:5*ЬЫEdgeИInvB=1:3,2:5*ЬЫHalfEdge=1:2,2:2*ЬЫHalfEdgeИ=1:2,2:2*ЬЫHalfEdgeИB=1:2,2:2*ЬЫHalfEdgeИInv=1:2,2:2*ЬЫHalfEdgeИInvB=1:2,2:2*ЬЫEdgeЗBase=1:2,2:4*ЬЫEdgeЗTip=1:2,2:2*ЪDeskChairless=1:30,2:30*ЪDeskChairlessИ=1:20,2:20*ЪDeskChairlessИInv=1:60,2:60*Shower=1:20,2:20,5:12,7:8*ПWall=0:8,1:10,7:10*ПWallLeft=0:10,1:10,7:8*ПWallRight=0:10,1:10,7:8*Catwalk=1:16,5:20,9:4*CatwalkИ=1:24,5:32,9:4*CatwalkStraight=1:24,5:32,9:4*CatwalkWall=1:20,5:26,9:4*CatwalkRailingEnd=1:28,5:38,9:4*CatwalkRailingHalfRight=1:28,5:36,9:4*CatwalkRailingHalfLeft=1:28,5:36,9:4*CatwalkHalf=1:10,5:10,9:2*CatwalkHalfRailing=1:18,5:22,9:2*CatwalkHalfCenterRailing=1:14,5:16,9:2*CatwalkHalfOuterRailing=1:14,5:16,9:2*GratedStairs=1:22,2:16,5:12*GratedHalfStairs=1:20,2:8,5:6*GratedHalfStairsMirrored=1:20,2:8,5:6*RailingStraight=1:8,5:6*RailingDouble=1:16,5:12*RailingИ=1:16,5:12*RailingDiagonal=1:12,5:9*RailingHalfRight=1:8,5:4*RailingHalfLeft=1:8,5:4*RailingCenter=1:8,5:6*Railing2x1Right=1:10,5:7*Railing2x1Left=1:10,5:7*Freight1=1:8,2:6*Freight1Г=1:8,2:6*Freight2=1:16,2:12*Freight2Г=1:16,2:12*Freight3=1:24,2:18*Truss=5:10,9:20*TrussГ=5:1,9:2*TrussFrame=5:5,9:10*TrussЗdFrame=5:3,9:6*TrussЗd=5:5,9:10*TrussЗdГ=5:1,9:1*TrussAngled=5:10,9:20*TrussAngledГ=5:1,9:2*TrussHalf=5:5,9:10*TrussHalfГ=5:1,9:1*TrussFloor=1:16,5:30,9:24*TrussFloorT=1:16,5:30,9:22*TrussFloorX=1:16,5:30,9:20*TrussFloorAngled=1:16,5:30,9:24*TrussFloorAngledInverted=1:16,5:30,9:24*TrussFloorHalf=1:14,5:20,9:12*БBarrel=0:5,1:6,6:1*ГBarrel=0:5,1:6,6:1*БBarrelThree=0:15,1:18,6:3*БBarrelStack=0:50,1:60,6:10,8:4,9:8*ЖCenter=0:140*ЖИ=0:120*ЖInvИ=0:135*ЖSide=0:130*ГЖCenter=0:5*ГЖИ=0:5*ГЖInvИ=0:5*ГЖSide=0:5*ЪГVivarium=1:20,2:30,5:10,7:100,9:40*ЪГИVivarium=1:20,2:60,5:10,7:60,9:40*ЪБVivarium=1:100,2:150,5:50,7:500,9:100*ЪБИVivarium=1:100,2:300,5:50,7:300,9:100*ЪNarrowViewport=0:12,1:10,7:5*ЪNarrowViewport_2x1Base=0:20,1:10,7:5*ЪNarrowViewport_2x1BaseInv=0:20,1:10,7:5*ЪNarrowViewport_2x1Tip=0:6,1:10,7:5*ЪNarrowViewport_2x1TipInv=0:6,1:10,7:5*ЪNarrowViewport_Double=0:10,1:10,7:10*ЪNarrowViewport_HalfЗ=0:22,1:10,7:5*ЪNarrowViewport_HalfЗInv=0:22,1:10,7:5*ЪNarrowViewport_З=0:12,1:10,7:5*ЪNarrowViewport_ЗInv=0:12,1:10,7:5*ЪFloorPlanSign1=1:4,2:2*ЪFloorPlanSign2=1:4,2:2*ЪFloorPlanSign3=1:4,2:2*ЪFloorPlanSign4=1:4,2:2*ЪFloorPlanSign5=1:4,2:2*ЪFloorPlanSign6=1:4,2:2*ЪFloorPlanSign7=1:4,2:2*ЪFloorPlanSign8=1:4,2:2*ЪFloorPlanSign9=1:4,2:2*ЪFloorPlanSign10=1:4,2:2*ЪFloorPlanSign11=1:4,2:2*ЪFloorPlanSign12=1:4,2:2*ЪFloorPlanSign13=1:4,2:2*ЪFloorPlanSign14=1:4,2:2*ЪFloorPlanSign15=1:4,2:2*ЪFloorPlanSign16=1:4,2:2*ЪFloorPlanSign17=1:4,2:2*ЪFloorPlanSign18=1:4,2:2*ЪFloorPlanSign19=1:4,2:2*ЪFloorPlanSign20=1:4,2:2*ЪFloorPlanSign21=1:4,2:2*БRailStraight=0:12,1:8,6:4*Monolith=0:130,13:130*Stereolith=0:130,13:130*DeadAstronaut=0:13,13:13*БDeadAstronaut=0:13,13:13*EngineerPlushie=29:1*EngineerPlushieSE2=30:1*SabiroidPlushie=31:1*БWarningSignEaster2=1:4,2:6*ГWarningSignEaster2=1:4,2:6*БWarningSignEaster3=1:4,2:6*ГWarningSignEaster3=1:4,2:6*БWarningSignEaster9=1:4,2:4*ГWarningSignEaster9=1:2,2:1*БWarningSignEaster10=1:4,2:4*ГWarningSignEaster10=1:2,2:1*БWarningSignEaster11=1:4,2:4*ГWarningSignEaster11=1:2,2:1*БWarningSignEaster13=1:4,2:4*ГWarningSignEaster13=1:2,2:1*ЪStatueEngineer=0:60,1:30,9:30*CorridorЫ=1:30,2:100*CorridorЫИ=1:30,2:100*CorridorЫT=1:25,2:70*CorridorЫX=1:20,2:40*CorridorЫTransition=1:30,2:100*ЪFloorCenter=0:20,1:10,2:10*ЪFloorCenterMirrored=0:20,1:10,2:10*ЪFloorEdge=0:20,1:10,2:10*ЪFloorEdgeMirrored=0:20,1:10,2:10*ЪFloorPassage=0:20,1:10,2:10*ЪFloorPassageMirrored=0:20,1:10,2:10*ЪFloorDecal=0:20,1:10,2:10*ЪFloorDecalMirrored=0:20,1:10,2:10*ЪFloorSlab=0:160,1:80,2:80*ЬFloorCenter=0:2,1:1,2:1*ЬFloorCenterMirrored=0:2,1:1,2:1*ЬFloorSlab=0:16,1:8,2:8*ЪLabDesk=1:30,2:30,4:2,12:4*ЪLabSink=1:30,2:30,4:2*ЪPipesStraight1=0:5,1:20,6:12*ЪPipesStraight2=0:10,1:20,6:12*ЪPipesEnd=0:5,1:20,6:12*ЪPipesJunction=0:10,1:30,6:14*ЪPipesИOuter=0:1,1:10,6:6*ЪPipesИ=0:5,1:20,6:12*ЪPipesИInner=0:10,1:30,6:18*DeadBody01=7:1,10:1,11:1*DeadBody02=7:1,10:1,11:1*DeadBody03=7:1,10:1,11:1*DeadBody04=7:1,10:1,11:1*DeadBody05=7:1,10:1,11:1*DeadBody06=7:1,10:1,11:1*AngledInteriorWallA=1:10,2:25*AngledInteriorWallB=1:10,2:25*PipeWorkДA=1:20,2:20,6:10*PipeWorkДB=1:20,2:20,6:10*БWarningSign1=1:4,2:6*БWarningSign2=1:4,2:6*БWarningSign3=1:4,2:6*БWarningSign4=1:4,2:2*БWarningSign5=1:4,2:6*БWarningSign6=1:4,2:6*БWarningSign7=1:4,2:2*БWarningSign8=1:4,2:4*БWarningSign9=1:4,2:4*БWarningSign10=1:4,2:4*БWarningSign11=1:4,2:4*БWarningSign12=1:4,2:4*БWarningSign13=1:4,2:4*ГWarningSign1=1:4,2:6*ГWarningSign2=1:4,2:2*ГWarningSign3=1:4,2:6*ГWarningSign4=1:4,2:2*ГWarningSign5=1:4,2:6*ГWarningSign6=1:4,2:2*ГWarningSign7=1:4,2:2*ГWarningSign8=1:2,2:1*ГWarningSign9=1:2,2:1*ГWarningSign10=1:2,2:1*ГWarningSign11=1:2,2:1*ГWarningSign12=1:2,2:1*ГWarningSign13=1:2,2:1*ЪЙPipeCap=1:10,2:10*ЪCylindricalColumn=1:10,2:25*ЬCylindricalColumn=1:3,2:5*БGridBeamД=0:25*БGridBeamДЗ=0:13*БGridBeamДЫ=0:13*БGridBeamДЗ2x1Base=0:19*БGridBeamДЗ2x1Tip=0:7*БGridBeamДHalf=0:12*БGridBeamДHalfЗ=0:7*БGridBeamДEnd=0:25*БGridBeamДJunction=0:25*БGridBeamДTJunction=0:25*ГGridBeamД=0:1*ГGridBeamДЗ=0:1*ГGridBeamДЫ=0:1*ГGridBeamДЗ2x1Base=0:1*ГGridBeamДЗ2x1Tip=0:1*ГGridBeamДHalf=0:1*ГGridBeamДHalfЗ=0:1*ГGridBeamДEnd=0:1*ГGridBeamДJunction=0:1*ГGridBeamДTJunction=0:1*Passage2=1:20,2:74,5:48*Passage2Wall=1:14,2:50,5:32*БStairs=1:30,2:50*БRamp=1:16,2:70*БSteelCatwalk=1:5,2:27,5:20*БSteelCatwalk2Sides=1:7,2:32,5:25*БSteelCatwalkИ=1:7,2:32,5:25*БSteelCatwalkPlate=1:7,2:23,5:17*БCoverWall=0:4,1:10*БCoverWallHalf=0:2,1:6*БCoverWallHalfMirrored=0:2,1:6*ЪInteriorWall=1:10,2:25*БInteriorPillar=1:10,2:25,5:4*AirDuct1=0:10,1:30,2:20,6:4*AirDuct2=0:10,1:30,2:20,6:4*AirDuctИ=0:10,1:30,2:20,6:4*AirDuctT=0:8,1:30,2:15,6:4*AirDuctX=0:5,1:30,2:10,6:4*AirDuctRamp=0:10,1:30,2:20,6:4*AirDuctGrate=1:10,2:10*ЪЙCap=1:10,2:10*ЬЙCapMedium=1:10,2:10*ЬЙCap=1:2,2:2*Viewport1=0:10,1:10,7:8*Viewport2=0:10,1:10,7:8*BarredП=1:4,9:1*BarredПЗ=1:4,9:1*BarredПSide=1:4,9:1*BarredПFace=1:4,9:1*StorageShelf1=0:50,1:50,2:50,5:50,9:10*StorageShelf2=4:20,8:20,9:30,15:20*StorageShelf3=9:10,13:10,14:2,16:10,18:10*ЪInsetWall=1:10,2:25*ЪInsetWallPillar=1:10,2:25*ЪInsetWallИ=1:10,2:25*ЪInsetWallИInverted=1:10,2:25*ЪInsetWallЗ=1:7,2:19*ЪConsoleModule=1:50,2:50*ЪConsoleModuleИ=1:30,2:30*ЬConsoleModule=1:6,2:6*ЬConsoleModuleИ=1:3,2:3*ЬConsoleModuleInvertedИ=1:3,2:3*ExtendedП=7:25,9:10*ExtendedПRailing=7:25,9:10*ExtendedПИ=7:10,9:4*ExtendedПИInverted=7:50,9:15*ExtendedПИInvertedRailing=7:50,9:15*ExtendedПDiagonal=7:35,9:12*ExtendedПDiagonalRailing=7:35,9:12*ExtendedПEnd=7:25,9:10*ExtendedПDome=7:25,9:10*ЬExtendedП=7:3,9:1*ЬExtendedПИ=7:2,9:1*ЬExtendedПИInverted=7:6,9:2*ЬExtendedПDiagonal=7:4,9:1*ЬExtendedПEnd=7:3,9:1*ЬExtendedПDome=7:6,9:2*Corridor=1:30,2:100*CorridorИ=1:30,2:100*CorridorT=1:25,2:70*CorridorX=1:20,2:40*CorridorП=1:30,2:80,7:6*CorridorDoubleП=1:25,2:65,7:12*CorridorПRoof=1:30,2:80,7:6*CorridorNarrow=1:30,2:100*TrussPillar=1:10,2:25,5:4*TrussPillarИ=1:10,2:25,5:4*TrussPillarSlanted=1:10,2:25,5:4*TrussPillarT=1:12,2:30,5:6*TrussPillarX=1:15,2:35,5:8*TrussPillarDiagonal=1:12,2:30,5:6*TrussPillarГ=1:10,2:25,5:4*TrussPillarOffset=1:10,2:25,5:4*ЪSciFiWall=1:10,2:25*ЪBarCounter=1:10,2:16,4:1,7:6*ЪBarCounterИ=1:14,2:24,4:2,7:10*ЪStructural_Frame=0:100*ЪStructural_SupportBeam5x5=0:100*ЪStructural_SupportBeam4x4=0:75*ЪStructural_SupportBeam3x3=0:50*ЪStructural_PlatformTriangle=0:250*ЪStructural_Platform=0:500*БШA=0:4*БШB=0:4*БШC=0:4*БШD=0:4*БШE=0:4*БШF=0:4*БШG=0:4*БШH=0:4*БШI=0:4*БШJ=0:4*БШK=0:4*БШL=0:4*БШM=0:4*БШN=0:4*БШO=0:4*БШP=0:4*БШQ=0:4*БШR=0:4*БШS=0:4*БШT=0:4*БШU=0:4*БШV=0:4*БШW=0:4*БШX=0:4*БШY=0:4*БШZ=0:4*ГШA=0:1*ГШB=0:1*ГШC=0:1*ГШD=0:1*ГШE=0:1*ГШF=0:1*ГШG=0:1*ГШH=0:1*ГШI=0:1*ГШJ=0:1*ГШK=0:1*ГШL=0:1*ГШM=0:1*ГШN=0:1*ГШO=0:1*ГШP=0:1*ГШQ=0:1*ГШR=0:1*ГШS=0:1*ГШT=0:1*ГШU=0:1*ГШV=0:1*ГШW=0:1*ГШX=0:1*ГШY=0:1*ГШZ=0:1*БШ0=0:4*БШ1=0:4*БШ2=0:4*БШ3=0:4*БШ4=0:4*БШ5=0:4*БШ6=0:4*БШ7=0:4*БШ8=0:4*БШ9=0:4*ГШ0=0:1*ГШ1=0:1*ГШ2=0:1*ГШ3=0:1*ГШ4=0:1*ГШ5=0:1*ГШ6=0:1*ГШ7=0:1*ГШ8=0:1*ГШ9=0:1*БШHyphen=0:4*БШUnderscore=0:4*БШDot=0:4*БШApostrophe=0:4*БШAnd=0:4*БШColon=0:4*БШExclamationMark=0:4*БШQuestionMark=0:4*ГШHyphen=0:1*ГШUnderscore=0:1*ГШDot=0:1*ГШApostrophe=0:1*ГШAnd=0:1*ГШColon=0:1*ГШExclamationMark=0:1*ГШQuestionMark=0:1*FireCover=0:4,1:10*FireCoverИ=0:8,1:20*HalfП=0:10,7:10,9:4*HalfПInv=0:10,7:10,9:4*HalfПИ=0:20,7:20,9:8*HalfПИInv=0:20,7:20,9:8*HalfПDiagonal=0:14,7:14,9:6*HalfПЫ=0:16,7:18,9:7*Embrasure=0:30,1:20,8:10*PassageSciFi=1:20,2:74,5:48*PassageSciFiWall=1:14,2:50,5:32*PassageSciFiIntersection=1:10,2:35,5:25*PassageSciFiGate=1:10,2:35,5:25*PassageScifiИ=1:20,2:74,5:48*PassageSciFiTjunction=1:16,2:55,5:38*PassageSciFiП=1:16,2:60,5:38,7:16*BridgeП1x1З=0:5,2:10,7:25,9:8*BridgeП1x1Face=0:2,2:4,7:18,9:8*BridgeП1x1FaceInverted=0:6,2:12,7:12,9:5*БПSquare=1:8,2:12,5:4*БПEdge=1:12,2:16,5:6*П1x2З=7:55,9:16*П1x2Inv=7:40,9:15*П1x2Face=7:40,9:15*П1x2SideLeft=7:26,9:13*П1x2SideLeftInv=7:26,9:13*П1x2SideRight=7:26,9:13*П1x2SideRightInv=7:26,9:13*П1x1З=7:35,9:12*П1x1Face=7:24,9:11*П1x1Side=7:17,9:9*П1x1SideInv=7:17,9:9*П1x1Inv=7:24,9:11*П1x2Flat=7:50,9:15*П1x2FlatInv=7:50,9:15*П1x1Flat=7:25,9:10*П1x1FlatInv=7:25,9:10*П3x3Flat=7:196,9:40*П3x3FlatInv=7:196,9:40*П2x3Flat=7:140,9:25*П2x3FlatInv=7:140,9:25*ГП1x2З=7:3,9:1*ГП1x2Inv=7:3,9:1*ГП1x2Face=7:3,9:1*ГП1x2SideLeft=7:3,9:1*ГП1x2SideLeftInv=7:3,9:1*ГП1x2SideRight=7:3,9:1*ГП1x2SideRightInv=7:3,9:1*ГП1x1З=7:2,9:1*ГП1x1Face=7:2,9:1*ГП1x1Side=7:2,9:1*ГП1x1SideInv=7:2,9:1*ГП1x1Inv=7:2,9:1*ГП1x2Flat=7:3,9:1*ГП1x2FlatInv=7:3,9:1*ГП1x1Flat=7:2,9:1*ГП1x1FlatInv=7:2,9:1*ГП3x3Flat=7:12,9:3*ГП3x3FlatInv=7:12,9:3*ГП2x3Flat=7:8,9:2*ГП2x3FlatInv=7:8,9:2*ПЫ=7:45,9:15*ПЫInv=7:45,9:15*ПЫИ=7:33,9:13*ПЫИInv=7:33,9:13*ПЫFace=7:21,9:9*ПЫFaceInv=7:21,9:9*ПЫInwardsИ=7:20,9:13*ПЫInwardsИInv=7:20,9:13*ГПЫ=7:2,9:1*ГПЫInv=7:2,9:1*ГПЫИ=7:2,9:1*ГПЫИInv=7:2,9:1*ГПЫFace=7:2,9:1*ГПЫFaceInv=7:2,9:1*ГПЫInwardsИ=7:2,9:1*ГПЫInwardsИInv=7:2,9:1$DebugSphere1*DebugSphereБ=0:10,3:20$DebugSphere2*DebugSphereБ=0:10,3:20$DebugSphere3*DebugSphereБ=0:10,3:20$Decoy*TrussPillarDecoy=0:30,1:10,3:10,6:2,11:1*БDecoy=0:30,1:10,3:10,6:2,11:1*ГDecoy=0:2,1:1,3:1,5:2,11:1$DefensiveCombatBlock*БDefensiveCombat=0:20,1:30,2:20,3:20,4:4,12:20*ГDefensiveCombat=0:2,1:5,2:2,3:10,4:2,12:4$Door*ЪГGate=0:300,1:70,3:6,4:10,5:60*ЪEvenWideDoor=0:300,1:70,3:6,4:10,5:60*ЪCentredDoor=0:30,1:40,3:2,4:4,10:1*ЪCentredDoorGlass=0:30,1:15,3:2,4:4,7:10,10:1*ЪHalfCentredDoor=0:20,1:40,3:2,4:4,10:1*ЪHalfCentredDoorGlass=0:20,1:15,3:2,4:4,7:10,10:1*ЬCentredDoor=0:10,1:30,3:2,4:2,10:1*ЬCentredDoorGlass=0:10,1:10,3:2,4:2,7:10,10:1*(null)=0:8,1:40,2:10,3:2,4:2,5:4,10:1*ГDoor=0:6,1:30,2:8,3:2,4:2,5:4,10:1*ЪAngledDoorA=0:30,1:40,3:2,4:4,10:1*ЪAngledDoorB=0:30,1:40,3:2,4:4,10:1*ЬAngledDoorA=0:20,1:40,3:2,4:2,10:1*CorridorЫDoor=0:45,1:50,3:2,4:4,5:10,10:2*CorridorЫDoorInv=0:45,1:50,3:2,4:4,5:10,10:2*ЪLabDoor=1:20,3:2,4:2,7:20,9:10,10:2*ЪLabDoorInv=1:20,3:2,4:2,7:20,9:10,10:2*ЪGate=0:800,1:100,3:10,4:20,5:100*ЪOffsetDoor=0:25,1:35,3:2,4:4,5:4,7:6,10:1*ЪNarrowDoor=0:35,1:40,3:2,4:4,5:10,10:1*ЪNarrowDoorHalf=0:25,1:40,3:2,4:4,5:10,10:1*ГSideDoor=0:8,1:26,2:10,3:2,4:2,7:4,10:1*SlidingHatchDoor=0:40,1:50,3:2,4:4,5:10,7:10,10:2*SlidingHatchDoorHalf=0:30,1:50,3:2,4:4,5:10,7:10,10:2$Drill*ЪDrillЩ=0:300,1:40,3:5,4:5,6:12*ЬDrillЩ=0:32,1:30,3:1,4:1,6:4*ЪPrototechDrill=1:200,3:20,6:120,8:80,22:1,23:200,24:3,26:20*ЬDrill=0:32,1:30,3:1,4:1,6:4*ЪDrill=0:300,1:40,3:5,4:5,6:12$EmissiveBlock*БNeonTubesStraight1=1:2,2:6,5:6*БNeonTubesStraight2=1:2,2:6,5:6*БNeonTubesИ=1:2,2:6,5:6*БNeonTubesBendUp=1:4,2:12,5:12*БNeonTubesBendDown=1:1,2:3,5:3*БNeonTubesStraightEnd1=1:2,2:6,5:6*БNeonTubesStraightEnd2=1:4,2:10,5:6*БNeonTubesStraightDown=1:3,2:9,5:9*БNeonTubesU=1:6,2:18,5:18*БNeonTubesT=1:3,2:9,5:9*БNeonTubesCircle=1:4,2:12,5:12*ГNeonTubesStraight1=1:1,2:1,5:1*ГNeonTubesStraight2=1:1,2:1,5:1*ГNeonTubesИ=1:1,2:1,5:1*ГNeonTubesBendUp=1:1,2:1,5:1*ГNeonTubesBendDown=1:1,2:1,5:1*ГNeonTubesStraightDown=1:1,2:1,5:1*ГNeonTubesStraightEnd1=1:1,2:1,5:1*ГNeonTubesU=1:1,2:1,5:1*ГNeonTubesT=1:1,2:1,5:1*ГNeonTubesCircle=1:1,2:1,5:1$EmotionControllerBlock*EmotionControllerБ=1:30,2:10,3:20,7:6,10:12*EmotionControllerГ=1:3,2:1,3:5,7:1,10:1$EventControllerBlock*EventControllerБ=1:30,2:10,3:10,10:4*EventControllerГ=1:3,2:2,3:2,10:1$ExhaustBlock*БExhaustCap=1:8,2:10,4:2*ГExhaustCap=1:2,2:2,4:1*ГExhaustPipe=0:2,1:1,4:2,5:2*БExhaustPipe=0:15,1:10,4:4,6:2$ExtendedPistonBase*БPistonBaseЩ=0:50,1:30,3:5,4:10,6:10*ГPistonBaseЩ=0:10,1:5,3:1,4:2,5:4*БPistonBase=0:50,1:30,3:5,4:10,6:10*ГPistonBase=0:10,1:5,3:1,4:2,5:4$FlightMovementBlock*БFlightMovement=0:20,1:30,2:20,3:20,4:4,12:20*ГFlightMovement=0:2,1:5,2:2,3:10,4:2,12:4$FunctionalBlock*ЪAlgaeFarmЩ=0:30,1:30,3:10,5:30,7:60*ЪConduitDamaged=1:10,5:20*ServicesTerminal=0:23,1:12,3:8,4:2,10:4,11:2,12:6*ЪAlgaeFarm=0:30,1:30,3:10,5:30,7:60*ЪFarmPlot=1:40,2:40,5:10$GravityGenerator*(null)=0:150,1:60,3:40,4:6,6:4,14:6$GravityGeneratorSphere*(null)=0:150,1:60,3:40,4:6,6:4,14:6$Gyro*ЪGyro=0:600,1:40,3:5,4:4,6:4,8:50*ЬGyro=0:25,1:5,3:3,4:2,6:1*ЪPrototechGyro=1:40,3:5,6:16,8:50,22:1,23:300,25:1,26:2*ЬPrototechGyro=1:20,3:3,6:4,8:5,22:1,23:70,25:1,26:2$HeatVentBlock*БHeatVentД=0:25,1:20,4:5,6:10*ГHeatVentД=0:2,1:1,4:1,6:1$HydrogenEngine*БЭEngineЩ=0:100,1:70,3:4,4:12,5:20,6:12,15:1*ГЭEngineЩ=0:40,1:20,3:1,4:4,5:6,6:4,15:1*БЭEngine=0:100,1:70,3:4,4:12,5:20,6:12,15:1*ГЭEngine=0:30,1:20,3:1,4:4,5:6,6:4,15:1*БPrototechReactor=1:200,3:100,13:400,18:1000,22:1,23:400,24:30,27:10$InteriorLight*ЪInsetTerrariumDesert=1:30,2:30,4:1,5:10,7:10,14:1*ЪInsetTerrariumForest=1:30,2:30,4:1,5:10,7:10,14:1*БInsetPlanter=1:30,2:30,4:2,5:10,7:30*ЪConduitЮ=1:15,5:20*ЪConduitЮInv=1:15,5:20*ЪTrofferЮ=0:10,1:10,2:20*ЪHalfTrofferЮ=0:5,1:10,2:10*ЪHalfTrofferЮInv=0:5,1:10,2:10*ЪInsetAquarium=1:30,2:30,4:1,5:10,7:10,14:1*ЪInsetKitchen=1:30,2:30,4:6,6:8,7:6*CorridorЫЮ=1:30,2:100*LabEquipment2=1:50,2:40,4:4,6:4,7:100*ЪInsetЮ=0:10,1:10,2:20*ЬInsetЮ=0:1,1:2,2:1*AirDuctЮ=0:10,1:30,2:20,6:4*ГЮ=1:2*ЬГЮ=1:2*ЪЮ_1corner=1:3*ЪЮ_2corner=1:6*ЬЮ_1corner=1:2*ЬЮ_2corner=1:4*OffsetЮ=1:2*ЪInsetWallЮ=1:10,2:25*CorridorЮ=1:30,2:100*CorridorNarrowStowage=1:30,2:100*TrussPillarЮ=1:8,2:12,5:2*TrussPillarЮГ=1:2,2:1,5:1*PassageSciFiЮ=1:20,2:74,5:48*БЮЛ=1:10,2:5*ГЮЛ=1:2,2:1$InteriorTurret*БInteriorTurret=0:4,1:20,2:6,3:5,4:2,5:1$Jukebox*ЬJukeboxЩ=1:2,2:4,3:2,10:1*Jukebox=1:10,2:15,3:4,10:4*ЪInsetEntertainmentИ=1:20,2:30,3:10,10:8$JumpDrive*БJumpDriveЩ=0:60,1:40,3:300,8:50,12:20,13:1000,14:20,15:120*БPrototechJumpDrive=1:180,3:300,13:1400,14:30,22:1,23:200,25:20,27:30*ГPrototechJumpDrive=1:20,3:20,13:100,14:4,22:1,23:15,25:4,27:8*БJumpDrive=0:60,1:40,3:300,8:50,12:20,13:1000,14:20,15:120$Kitchen*ЪKitchen=1:30,2:20,4:6,6:6,7:4$LCDPanelsBlock*LabEquipment=1:15,2:15,3:6,4:1,7:4*MedicalStation=1:15,2:15,3:6,4:2,10:2,17:1*ЪBillboard=1:10,2:10,3:6,7:4,10:10*ЪBillboardЫ=1:15,2:15,3:10,7:6,10:20*ЪLabDeskMicroscope=1:20,2:20,3:6,7:6,10:4,12:8*LabEquipment1=1:30,2:20,4:4,7:40,12:4*LabEquipment3=1:50,2:60,4:12,7:16,13:8,18:2$Ladder2*TrussLadder=1:20,2:10,5:30,9:20*(null)=1:20,2:10,5:10*LadderShaft=1:40,2:80,5:50*LadderГ=1:20,2:10,5:10$LandingGear*ЪLandingGearЩ=0:60,1:10,4:4*ЬLandingGearЩ=0:2,1:3,4:1*ЪMagneticPlate=0:450,1:60,4:20*ЬMagneticPlate=0:6,1:15,4:3*ЪLandingGear=0:150,1:20,4:6*ЬLandingGear=0:2,1:5,4:1*ЪГMagneticPlate=0:15,1:3,4:1*ЬГMagneticPlate=0:2,1:1,4:1$LargeGatlingTurret*БGatlingTurretЩ=0:40,1:40,3:10,4:8,5:6,8:15*ГGatlingTurretЩ=0:15,1:30,3:10,4:4,5:6,8:5*(null)=0:40,1:40,3:10,4:8,5:6,8:15*ГGatlingTurret=0:15,1:30,3:10,4:4,5:6,8:5*AutoCannonTurret=0:20,1:40,3:10,4:4,5:4,8:6$LargeMissileTurret*БMissileTurretЩ=0:40,1:50,3:10,4:16,6:6,8:15*ГMissileTurretЩ=0:15,1:40,3:10,4:8,6:2,8:5*(null)=0:40,1:50,3:10,4:16,6:6,8:15*ГMissileTurret=0:15,1:40,3:10,4:8,6:2,8:5*БCalibreTurret=0:450,1:400,3:20,4:30,6:40,8:50*ЪMediumCalibreTurret=0:300,1:280,3:20,4:20,6:30,8:30*ЬMediumCalibreTurret=0:50,1:100,3:20,4:10,6:6,8:10$LaserAntenna*ЪLaserAntenna=0:50,1:40,3:50,4:16,7:4,11:20,12:30,13:100*ЬLaserAntenna=0:10,1:10,3:30,4:5,5:10,7:2,11:5,13:10$MedicalRoom*БMedicalRoomЩ=1:80,2:240,3:10,5:20,6:5,8:60,10:10,17:15*БMedicalRoom=1:80,2:240,3:10,5:20,6:5,8:60,10:10,17:15*InsetRefillStation=1:30,2:20,4:6,8:2,10:4*БRefillStation=1:10,2:9,4:6,8:2,10:4*ГRefillStation=1:10,2:9,4:6,8:2,10:4$MergeBlock*БShipMergeД=0:12,1:15,3:2,4:2,6:6*ГShipMergeД=0:4,1:5,3:1,4:1,5:2*ГShipГMergeД=0:2,1:3,3:1,4:1,5:1$MotorAdvancedRotor*БAdvancedRotor=0:30,6:10*ГAdvancedRotor=0:20,6:6*ГAdvancedRotorГ=0:12,5:6*БHingeHead=0:15,1:10,6:5*MediumHingeHead=0:8,1:5,6:3*ГHingeHead=0:3,1:2,6:1$MotorAdvancedStator*БAdvancedStator=0:30,1:20,3:5,4:5,6:5*ГAdvancedStator=0:20,1:15,3:3,4:3,6:3*ГAdvancedStatorГ=0:6,1:5,3:1,4:1,5:1*БHinge=0:30,1:20,3:5,4:5,6:5*MediumHinge=0:20,1:15,3:3,4:3,6:3*ГHinge=0:6,1:5,3:1,4:1,6:1$MotorRotor*БRotor=0:30,6:10*ГRotor=0:12,5:6$MotorStator*БStator=0:30,1:20,3:5,4:5,6:5*ГStator=0:20,1:15,3:3,4:3,5:3$MotorSuspension*ЧЦ3x3=0:25,1:15,4:6,5:12,6:6*ЧЦ5x5=0:70,1:40,4:20,5:30,6:20*ЧЦ1x1=0:25,1:15,4:6,5:12,6:6*ЧЦ2x2=0:25,1:15,4:6,5:12,6:6*ЧГЦ3x3=0:8,1:7,4:1,5:2*ЧГЦ5x5=0:16,1:12,4:2,5:4*ЧГЦ1x1=0:8,1:7,4:1,5:2*ЧГЦ2x2=0:8,1:7,4:1,5:2*ЧЦ3x3І=0:25,1:15,4:6,5:12,6:6*ЧЦ5x5І=0:70,1:40,4:20,5:30,6:20*ЧЦ1x1І=0:25,1:15,4:6,5:12,6:6*ЧЦ2x2Mirrored=0:25,1:15,4:6,5:12,6:6*ЧГЦ3x3І=0:8,1:7,4:1,5:2*ЧГЦ5x5І=0:16,1:12,4:2,5:4*ЧГЦ1x1І=0:8,1:7,4:1,5:2*ЧГЦ2x2Mirrored=0:8,1:7,4:1,5:2*ЧShortЦ3x3=0:15,1:10,4:6,5:12,6:6*ЧShortЦ5x5=0:45,1:30,4:20,5:30,6:20*ЧShortЦ1x1=0:15,1:10,4:6,5:12,6:6*ЧShortЦ2x2=0:15,1:10,4:6,5:12,6:6*ЧГShortЦ3x3=0:5,1:5,4:1,5:2*ЧГShortЦ5x5=0:10,1:10,4:2,5:4*ЧГShortЦ1x1=0:5,1:5,4:1,5:2*ЧГShortЦ2x2=0:5,1:5,4:1,5:2*ЧShortЦ3x3І=0:15,1:10,4:6,5:12,6:6*ЧShortЦ5x5І=0:45,1:30,4:20,5:30,6:20*ЧShortЦ1x1І=0:15,1:10,4:6,5:12,6:6*ЧShortЦ2x2Mirrored=0:15,1:10,4:6,5:12,6:6*ЧГShortЦ3x3І=0:5,1:5,4:1,5:2*ЧГShortЦ5x5І=0:10,1:10,4:2,5:4*ЧГShortЦ1x1І=0:5,1:5,4:1,5:2*ЧГShortЦ2x2Mirrored=0:5,1:5,4:1,5:2*Ц3x3=0:25,1:15,4:6,5:12,6:6*Ц5x5=0:70,1:40,4:20,5:30,6:20*Ц1x1=0:25,1:15,4:6,5:12,6:6*Ц2x2=0:25,1:15,4:6,5:12,6:6*ГЦ3x3=0:8,1:7,4:1,5:2*ГЦ5x5=0:16,1:12,4:2,5:4*ГЦ1x1=0:8,1:7,4:1,5:2*ГЦ2x2=0:8,1:7,4:1,5:2*Ц3x3І=0:25,1:15,4:6,5:12,6:6*Ц5x5І=0:70,1:40,4:20,5:30,6:20*Ц1x1І=0:25,1:15,4:6,5:12,6:6*Ц2x2Mirrored=0:25,1:15,4:6,5:12,6:6*ГЦ3x3І=0:8,1:7,4:1,5:2*ГЦ5x5І=0:16,1:12,4:2,5:4*ГЦ1x1І=0:8,1:7,4:1,5:2*ГЦ2x2Mirrored=0:8,1:7,4:1,5:2*ShortЦ3x3=0:15,1:10,4:6,5:12,6:6*ShortЦ5x5=0:45,1:30,4:20,5:30,6:20*ShortЦ1x1=0:15,1:10,4:6,5:12,6:6*ShortЦ2x2=0:15,1:10,4:6,5:12,6:6*ГShortЦ3x3=0:5,1:5,4:1,5:2*ГShortЦ5x5=0:10,1:10,4:2,5:4*ГShortЦ1x1=0:5,1:5,4:1,5:2*ГShortЦ2x2=0:5,1:5,4:1,5:2*ShortЦ3x3І=0:15,1:10,4:6,5:12,6:6*ShortЦ5x5І=0:45,1:30,4:20,5:30,6:20*ShortЦ1x1І=0:15,1:10,4:6,5:12,6:6*ShortЦ2x2Mirrored=0:15,1:10,4:6,5:12,6:6*ГShortЦ3x3І=0:5,1:5,4:1,5:2*ГShortЦ5x5І=0:10,1:10,4:2,5:4*ГShortЦ1x1І=0:5,1:5,4:1,5:2*ГShortЦ2x2Mirrored=0:5,1:5,4:1,5:2$MyObjectBuilder_Projector*БProjector=0:21,1:4,3:2,4:1,6:2*ГProjector=0:2,1:2,3:2,4:1,6:2$MyProgrammableBlock*ГProgrammableД=0:2,1:2,3:2,4:1,6:2,10:1*БProgrammableД=0:21,1:4,3:2,4:1,6:2,10:1*БProgrammableДЩ=0:21,1:4,3:2,4:1,6:2,10:1*ГProgrammableДЩ=0:2,1:2,3:2,4:1,6:2,10:1$OffensiveCombatBlock*БOffensiveCombat=0:20,1:30,2:20,3:20,4:4,12:20*ГOffensiveCombat=0:2,1:5,2:2,3:10,4:2,12:4$OreDetector*БOreDetectorЩ=0:50,1:40,3:25,4:5,12:20*ГOreDetectorЩ=0:3,1:2,3:1,4:1,12:1*БOreDetector=0:50,1:40,3:25,4:5,12:20*ЬOreDetector=0:3,1:2,3:1,4:1,12:1$OxygenFarm*ЪOxygenFarmЩ=0:30,1:30,3:10,5:30,7:60*ЪOxygenFarm=0:30,1:30,3:10,5:30,7:60$OxygenGenerator*ЪOxygenGeneratorLab=0:160,1:20,3:5,4:4,6:4,7:40*ЬOxygenGeneratorLab=0:6,1:8,3:3,4:1,6:2,7:3*(null)=0:120,1:5,3:5,4:4,6:2*OxygenGeneratorГ=0:8,1:8,3:3,4:1,6:2*IrrigationSystem=0:100,1:20,3:5,4:6,6:10$OxygenTank*БЭTankГLab=0:80,1:40,3:8,5:60,6:40*ГЭTankLab=0:40,1:20,3:4,5:30,6:20*ЪOxygenTankLab=0:80,1:40,3:8,5:60,6:40*БЭTankIndustrial=0:280,1:40,3:8,5:60,6:80*OxygenTankГ=0:16,1:10,3:8,5:10,6:8*ГOxygenTankГ=0:2,1:1,3:4,5:1,6:1*(null)=0:80,1:40,3:8,5:60,6:40*БЭTank=0:280,1:40,3:8,5:60,6:80*БЭTankГ=0:80,1:40,3:8,5:60,6:40*ГЭTank=0:40,1:20,3:4,5:30,6:20*ГЭTankГ=0:3,1:2,3:4,5:2,6:1$Parachute*LgParachute=0:9,1:25,3:2,4:3,5:5*SmParachute=0:2,1:2,3:1,4:1,5:1$Passage*(null)=1:20,2:74,5:48$PathRecorderBlock*БPathRecorderД=0:20,1:30,2:20,3:20,4:4,12:20*ГPathRecorderД=0:2,1:5,2:2,3:10,4:2,12:4$PistonBase*БPistonBase=0:50,1:30,3:5,4:10,6:10*ГPistonBase=0:10,1:5,3:1,4:2,5:4$PistonTop*БPistonTopЩ=0:10,6:8*ГPistonTopЩ=0:4,6:2*БPistonTop=0:10,6:8*ГPistonTop=0:4,6:2$Planter*ЪPlanters=1:20,2:10,5:8,7:8$Projector*ЪConsole=1:30,2:20,3:8,10:10$RadioAntenna*ЪRadioAntenna=0:80,1:30,3:8,5:60,6:40,11:40*ЪCompactRadioAntenna=0:40,1:20,3:8,5:30,6:20,11:40*ЬRadioAntenna=0:2,1:1,3:1,5:1,11:4*ЪCompactRadioAntennaЩ=0:40,1:20,2:80,3:8,5:30,11:40*ЬCompactRadioAntennaЩ=0:2,1:1,2:3,3:1,5:1,11:4*ЪRadioAntennaDish=0:80,1:40,3:8,9:120,11:40$Reactor*ЬГGenerator=0:3,1:10,3:10,4:1,6:1,8:2,18:3*ЬБGenerator=0:60,1:9,3:25,4:5,6:3,8:9,18:95*ЪГGenerator=0:80,1:40,3:25,4:6,6:8,8:4,18:100*ЪБGenerator=0:1000,1:70,3:75,4:20,6:40,8:40,13:100,18:2000*ЪГGeneratorWarfare2=0:80,1:40,3:25,4:6,6:8,8:4,18:100*ЪБGeneratorWarfare2=0:1000,1:70,3:75,4:20,6:40,8:40,13:100,18:2000*ЬГGeneratorWarfare2=0:3,1:10,3:10,4:1,6:1,8:2,18:3*ЬБGeneratorWarfare2=0:60,1:9,3:25,4:5,6:3,8:9,18:95$Refinery*БRefineryIndustrial=0:1200,1:40,3:20,4:16,6:20,8:20*БRefinery=0:1200,1:40,3:20,4:16,6:20,8:20*Blast Furnace=0:120,1:20,3:10,4:10*БPrototechRefinery=1:40,3:20,6:20,8:20,22:1,23:675,24:5,26:10*ГPrototechRefinery=1:20,3:20,6:16,8:20,22:1,23:70,24:2,26:3$ReflectorLight*ЪFloodlight=0:8,1:20,2:10,7:4,9:10*ЪFloodlightAngled=0:8,1:20,2:10,7:4,9:10*ЪFloodlightИL=0:8,1:20,2:10,7:4,9:10*ЪFloodlightИR=0:8,1:20,2:10,7:4,9:10*ЬFloodlight=0:1,1:1,2:1,7:2,9:4*ЬFloodlightAngled=0:1,1:1,2:1,7:2,9:4*ЬFloodlightИL=0:1,1:1,2:1,7:2,9:4*ЬFloodlightИR=0:1,1:1,2:1,7:2,9:4*ЬFloodlightDown=0:1,1:1,2:1,7:2,9:4*ЬFloodlightAngledRotated=0:1,1:1,2:1,7:2,9:4*RotatingЮБ=1:3,4:1*RotatingЮГ=1:3,4:1*ЪFrontЮ=0:8,1:15,2:20,6:2,7:4*ЬFrontЮ=0:1,1:1,2:1,6:1,7:2*OffsetSpotlight=1:2,7:1$RemoteControl*ЪRemoteControl=1:10,2:10,3:15,4:1*ЬRemoteControl=1:1,2:2,3:1,4:1$SafeZoneBlock*SafeZoneД=0:800,1:180,3:120,8:80,14:10,21:5*SafeZoneДЩ=0:800,1:180,3:120,8:80,14:10,21:5$Searchlight*ГSearchlight=0:1,1:3,3:5,4:2,6:1,7:2*БSearchlight=0:5,1:20,3:5,4:4,6:2,7:4$SensorBlock*ЬSensor=0:2,1:5,2:5,3:6,11:1,12:6*ЪSensor=0:2,1:5,2:5,3:6,11:1,12:6*ЬSensorЩ=0:2,1:5,2:5,3:6,11:1,12:6*ЪSensorЩ=0:2,1:5,2:5,3:6,11:1,12:6$ShipConnector*Connector=0:150,1:40,3:20,4:8,5:12*ConnectorГ=0:7,1:4,3:4,4:1,5:2*ConnectorMedium=0:21,1:12,3:6,4:6,5:6*ЪInsetConnector=0:150,1:40,3:20,4:8,5:12*ЪInsetConnectorГ=0:150,1:40,3:20,4:8,5:12*ЬInsetConnector=0:7,1:4,3:4,4:1,5:2*ЬInsetConnectorMedium=0:21,1:12,3:6,4:6,5:6*ЪStructural_PlatformConnector=0:500,1:40,3:20,4:8,5:12$ShipGrinder*БShipGrinderЩ=0:20,1:30,3:2,4:4,6:1*ГShipGrinderЩ=0:12,1:17,3:2,4:4,5:4*БShipGrinder=0:20,1:30,3:2,4:4,6:1*ГShipGrinder=0:12,1:17,3:2,4:4,5:4$ShipWelder*БShipWelderЩ=0:20,1:30,3:2,4:2,6:1*ГShipWelderЩ=0:12,1:17,3:2,4:2,5:6*БShipWelder=0:20,1:30,3:2,4:2,6:1*ГShipWelder=0:12,1:17,3:2,4:2,5:6$SmallGatlingGun*ГGatlingGunWarfare2=0:4,1:1,3:1,4:1,5:6,8:2*(null)=0:4,1:1,3:1,4:1,5:6,8:2*ЬAutocannon=0:6,1:2,3:1,4:1,5:2,8:2$SmallMissileLauncher*ГMissileLauncherWarfare2=0:4,1:2,3:1,4:1,6:4,8:1*(null)=0:4,1:2,3:1,4:1,6:4,8:1*БMissileLauncher=0:35,1:8,3:4,4:6,6:25,8:30*ЪБCalibreGun=0:250,1:20,3:5,6:20,8:20*БFlareLauncher=0:20,1:10,3:4,6:10*ГFlareLauncher=0:2,1:1,3:1,6:3$SmallMissileLauncherReload*ГRocketLauncherReload=0:8,1:24,2:50,3:2,4:4,5:50,6:8,8:10*ЬMediumCalibreGun=0:25,1:10,3:1,6:10,8:5*БRailgun=0:350,1:150,3:100,6:60,13:150,15:100*ГRailgun=0:25,1:20,3:20,6:6,13:20,15:10$SolarPanel*ЪColorableSolarЛ=0:4,1:14,3:4,7:4,9:12,19:32*ЪColorableSolarЛИ=0:2,1:7,3:4,7:2,9:6,19:16*ЪColorableSolarЛИInverted=0:2,1:7,3:4,7:2,9:6,19:16*ЬColorableSolarЛ=0:2,1:2,3:1,7:1,9:4,19:8*ЬColorableSolarЛИ=0:1,1:2,3:1,7:1,9:2,19:4*ЬColorableSolarЛИInverted=0:1,1:2,3:1,7:1,9:2,19:4*ЪSolarЛ=0:4,1:14,3:4,7:4,9:12,19:32*ЬSolarЛ=0:2,1:2,3:1,7:1,9:4,19:8$SoundBlock*ЬSoundД=1:6,2:4,3:3*ЪSoundД=1:6,2:4,3:3$SpaceBall*SpaceBallБ=0:225,1:30,3:20,14:3*SpaceBallГ=0:70,1:10,3:7,14:1$StoreBlock*StoreД=0:30,1:20,3:10,4:6,10:4*AtmД=0:20,1:20,3:10,4:2,10:4$SurvivalKit*SurvivalKitБЩ=0:30,1:2,3:5,4:4,10:1,17:3*SurvivalKitГЩ=0:6,1:2,3:5,4:4,10:1,17:3*SurvivalKitБ=0:30,1:2,3:5,4:4,10:1,17:3*SurvivalKit=0:6,1:2,3:5,4:4,10:1,17:3$TargetDummyBlock*TargetDummy=0:15,3:4,4:2,5:10,10:1$TerminalBlock*ЬFirstAidCabinet=1:3,2:3*ЬKitchenOven=1:12,2:8,4:1,7:4*ЬKitchenMicrowave=1:6,2:4,4:1,7:2*ЬKitchenFridge=1:6,2:4,4:1,7:2*ControlЛ=0:1,1:1,3:1,10:1*ГControlЛ=0:1,1:1,3:1,10:1*БControlЛPedestal=1:10,2:5,3:1,10:1*ГControlЛPedestal=1:10,2:5,3:1,10:1*БCrate=0:20,1:24,4:4,5:8*БFreezer=1:20,2:20,5:10,7:10*ЪAccessЛ1=1:15,2:5,3:5*ЪAccessЛ2=1:15,2:5,5:10*ЪAccessЛ4=1:10,2:10*ЬAccessЛ1=1:8,2:2,5:2*ЬAccessЛ2=1:2,2:1,5:1*ЬAccessЛ3=1:4,2:1,5:1*ЬAccessЛ4=1:10,2:10*ЪSciFiTerminal=1:4,2:2,3:2,10:4$TextPanel*TransparentLCDБ=1:8,3:6,7:10,10:10*TransparentLCDГ=1:4,3:4,7:1,10:3*HoloLCDБ=1:10,3:8,4:1*HoloLCDГ=1:5,3:8,4:1*БFullДLCDЛ=1:20,2:20,3:6,7:6,10:10*ГFullДLCDЛ=1:4,2:4,3:4,7:1,10:3*БDiagonalLCDЛ=1:10,2:10,3:6,7:8,10:10*ГDiagonalLCDЛ=1:4,2:4,3:4,7:1,10:3*БCurvedLCDЛ=1:20,2:20,3:6,7:10,10:10*ГCurvedLCDЛ=1:4,2:4,3:4,7:1,10:3*ГTextЛ=1:4,2:1,3:4,7:1,10:3*ГLCDЛWide=1:8,2:1,3:8,7:2,10:6*ГLCDЛ=1:4,2:1,3:4,7:2,10:3*ЪИ_LCD_1=1:5,3:3,10:1*ЪИ_LCD_2=1:5,3:3,10:1*ЪИ_LCD_Flat_1=1:5,3:3,10:1*ЪИ_LCD_Flat_2=1:5,3:3,10:1*ЬИ_LCD_1=1:3,3:2,10:1*ЬИ_LCD_2=1:3,3:2,10:1*ЬИ_LCD_Flat_1=1:3,3:2,10:1*ЬИ_LCD_Flat_2=1:3,3:2,10:1*БTextЛ=1:6,2:1,3:6,7:2,10:10*БLCDЛ=1:6,2:1,3:6,7:6,10:10*БLCDЛWide=1:12,2:2,3:12,7:12,10:20*ЬConsoleModuleScreens=1:6,2:6,3:2,10:2*БLCDЛ5x5=1:150,2:25,3:25,7:150,10:250*БLCDЛ5x3=1:90,2:15,3:15,7:90,10:150*БLCDЛ3x3=1:50,2:10,3:10,7:50,10:90$Thrust*ЪБЭThrustЩ=0:150,1:180,6:40,8:250*ЪГЭThrustЩ=0:25,1:60,6:8,8:40*ЬБЭThrustЩ=0:30,1:30,6:10,8:22*ЬГЭThrustЩ=0:7,1:15,6:2,8:4*ЪБЭThrustIndustrial=0:150,1:180,6:40,8:250*ЪГЭThrustIndustrial=0:25,1:60,6:8,8:40*ЬБЭThrustIndustrial=0:30,1:30,6:10,8:22*ЬГЭThrustIndustrial=0:7,1:15,6:2,8:4*ЪPrototechThruster=1:325,6:160,8:250,22:1,23:500,24:5,28:60*ЬPrototechThruster=1:12,6:1,8:5,22:1,23:10,24:1,28:3*ЬГThrustSciFi=0:2,1:2,6:1,16:1*ЬБThrustSciFi=0:5,1:2,6:5,16:12*ЪГThrustSciFi=0:25,1:60,6:8,16:80*ЪБThrustSciFi=0:150,1:100,6:40,16:960*ЪБAtmosphericThrustSciFi=0:230,1:60,4:1100,6:50,8:40*ЪГAtmosphericThrustSciFi=0:35,1:50,4:110,6:8,8:10*ЬБAtmosphericThrustSciFi=0:20,1:30,4:90,6:4,8:8*ЬГAtmosphericThrustSciFi=0:3,1:22,4:18,6:1,8:1*ЬГThrust=0:2,1:2,6:1,16:1*ЬБThrust=0:5,1:2,6:5,16:12*ЪГThrust=0:25,1:60,6:8,16:80*ЪБThrust=0:150,1:100,6:40,16:960*ЪБЭThrust=0:150,1:180,6:40,8:250*ЪГЭThrust=0:25,1:60,6:8,8:40*ЬБЭThrust=0:30,1:30,6:10,8:22*ЬГЭThrust=0:7,1:15,6:2,8:4*ЪБAtmosphericThrust=0:230,1:60,4:1100,6:50,8:40*ЪГAtmosphericThrust=0:35,1:50,4:110,6:8,8:10*ЬБAtmosphericThrust=0:20,1:30,4:90,6:4,8:8*ЬГAtmosphericThrust=0:3,1:22,4:18,6:1,8:1*ЪБFlatAtmosphericThrust=0:90,1:25,4:400,6:20,8:15*ЪБFlatAtmosphericThrustDShape=0:90,1:25,4:400,6:20,8:15*ЪГFlatAtmosphericThrust=0:15,1:20,4:30,6:3,8:3*ЪГFlatAtmosphericThrustDShape=0:15,1:20,4:30,6:3,8:3*ЬБFlatAtmosphericThrust=0:8,1:14,4:30,6:2,8:3*ЬБFlatAtmosphericThrustDShape=0:8,1:14,4:30,6:2,8:3*ЬГFlatAtmosphericThrust=0:2,1:11,4:6,6:1,8:1*ЬГFlatAtmosphericThrustDShape=0:2,1:11,4:6,6:1,8:1*ЬГModularThruster=0:2,1:2,6:1,16:1*ЬБModularThruster=0:5,1:2,6:5,16:12*ЪГModularThruster=0:25,1:60,6:8,16:80*ЪБModularThruster=0:150,1:100,6:40,16:960$TimerBlock*TimerДБ=1:30,2:6,3:5*TimerДГ=1:3,2:2,3:1*TimerДЩБ=1:30,2:6,3:5*TimerДЩГ=1:3,2:2,3:1$TransponderBlock*ЪTransponder=0:30,1:20,3:10,11:5*ЬTransponder=0:3,1:2,3:2,11:1$TurretControlBlock*БTurretControlД=0:20,1:30,2:20,3:20,4:4,10:6,12:20*ГTurretControlД=0:4,1:10,2:4,3:10,4:2,10:1,12:4$UpgradeModule*БProductivityModule=0:100,1:40,3:60,4:4,5:20*БEffectivenessModule=0:100,1:50,4:4,5:15,13:20*БEnergyModule=0:100,1:40,4:4,5:20,15:20$VendingMachine*FoodDispenser=1:10,2:20,3:10,4:4,10:10*VendingMachine=1:10,2:20,3:10,4:4,10:4$VirtualMass*VirtualMassБ=0:90,1:30,3:20,13:20,14:9*VirtualMassГ=0:3,1:2,3:2,13:2,14:1$Warhead*БExplosiveBarrel=0:5,1:6,3:1,5:2,6:1,20:2*ГExplosiveBarrel=0:5,1:6,3:1,5:2,6:1,20:2*БWarhead=0:20,1:12,3:2,5:12,9:24,20:6*ГWarhead=0:4,1:1,3:1,5:2,9:1,20:2$Wheel*ЧГФ1x1=0:2,1:5,6:1*ЧГФ2x2=0:8,1:15,6:3*ЧГФ=0:8,1:15,6:3*ЧГФ5x5=0:15,1:25,6:5*ЧФ1x1=0:30,1:30,6:10*ЧФ2x2=0:50,1:40,6:15*ЧФ=0:70,1:50,6:20*ЧФ5x5=0:130,1:70,6:30*ЧГФ1x1І=0:2,1:5,6:1*ЧГФ2x2Mirrored=0:8,1:15,6:3*ЧГФІ=0:8,1:15,6:3*ЧГФ5x5І=0:15,1:25,6:5*ЧФ1x1І=0:30,1:30,6:10*ЧФ2x2Mirrored=0:50,1:40,6:15*ЧФІ=0:70,1:50,6:20*ЧФ5x5І=0:130,1:70,6:30*ЧWheel1x1=0:30,1:30,6:10*ЧГWheel1x1=0:2,1:5,6:1*ЧWheel3x3=0:70,1:50,6:20*ЧГWheel3x3=0:8,1:15,6:3*ЧWheel5x5=0:130,1:70,6:30*ЧГWheel5x5=0:15,1:25,6:5*ЧWheel2x2=0:50,1:40,6:15*ЧГWheel2x2=0:5,1:10,6:2*ГФ1x1=0:2,1:5,6:1*ГФ2x2=0:8,1:15,6:3*ГФ=0:8,1:15,6:3*ГФ5x5=0:15,1:25,6:5*Ф1x1=0:30,1:30,6:10*Ф2x2=0:50,1:40,6:15*Ф=0:70,1:50,6:20*Ф5x5=0:130,1:70,6:30*ГФ1x1І=0:2,1:5,6:1*ГФ2x2Mirrored=0:8,1:15,6:3*ГФІ=0:8,1:15,6:3*ГФ5x5І=0:15,1:25,6:5*Ф1x1І=0:30,1:30,6:10*Ф2x2Mirrored=0:50,1:40,6:15*ФІ=0:70,1:50,6:20*Ф5x5І=0:130,1:70,6:30*Wheel1x1=0:30,1:30,6:10*ГWheel1x1=0:2,1:5,6:1*Wheel3x3=0:70,1:50,6:20*ГWheel3x3=0:8,1:15,6:3*Wheel5x5=0:130,1:70,6:30*ГWheel5x5=0:15,1:25,6:5*Wheel2x2=0:50,1:40,6:15*ГWheel2x2=0:5,1:10,6:2$WindTurbine*ЪWindTurbineЩ=1:20,2:40,3:2,4:8,9:24*ЪWindTurbine=1:20,2:40,3:2,4:8,9:24";