// ============================================================
//  SBOS - Ship Board Operating System  v3.1
//  Space Engineers Programmable Block Script
//  C# 6 Compatible | Sprite Drawing | Cockpit Support
// ============================================================
//
//  NAMING:
//    LCD Panel(s)      → "SBOS LCD 1", "SBOS LCD 2", etc.
//    Cockpit surface 0 → "SBOS Cockpit"
//    Cockpit surface N → "SBOS Cockpit:N"  (N = surface index)
//    Status bar        → "SBOS Status"
//    Button Panel      → "SBOS Buttons"
//
//  BUTTONS (Run Programmable Block with argument):
//    Button 1 → BTN1   context action
//    Button 2 → BTN2   context action
//    Button 3 → BTN3   1st press = AUTO-SCROLL ON
//                      while auto-scrolling, press again = manual step + OFF
//    Button 4 → BTN4   NEXT SCREEN
//
//  DEFAULT CUSTOM DATA (auto-written on first run, edit freely):
// ──────────────────────────────────────────────────────────
// *power
// *cargo
// *production
// *docked
// *lifesupport
// *damage
// *lines-shown - 14
// *color-power - green
// *color-h2 - red
// *color-o2 - cyan
// *color-cargo - yellow
// *color-damage - orange
// *color-text - teal
// *color-header - skyblue
// *color-background - darknavy
// ──────────────────────────────────────────────────────────
//  Delete a *screen line to hide that screen.
//  Supported color words: red, green, blue, cyan, yellow,
//  orange, white, teal, skyblue, purple, pink, lime,
//  darknavy, gray, darkgray, gold, coral, or R,G,B numbers.
// ============================================================

// ── SCREEN INDICES ────────────────────────────────────────
const int SCR_POWER = 0, SCR_CARGO = 1, SCR_PROD = 2,
          SCR_DOCK  = 3, SCR_LIFE  = 4, SCR_DMG  = 5;
static readonly string[] SCREEN_KEYS  = {"*power","*cargo","*production","*docked","*lifesupport","*damage"};
static readonly string[] SCREEN_NAMES = {"POWER GRID","CARGO","PRODUCTION","DOCKED SHIPS","LIFE SUPPORT","DAMAGE"};
static readonly string[] PROD_LABELS  = {"ALL","ASSEMBLERS","REFINERIES"};
static readonly string[] PWR_LABELS   = {"OVERVIEW","SOURCES","CONSUMERS"};

// ── NAMED COLOR TABLE ─────────────────────────────────────
static readonly string[] COLOR_NAMES = {
    "red","green","blue","cyan","yellow","orange","white","teal",
    "skyblue","purple","pink","lime","darknavy","gray","darkgray",
    "gold","coral","magenta","navy","olive"
};
static readonly Color[] COLOR_VALUES = {
    new Color(220,50,50),   // red
    new Color(0,220,80),    // green
    new Color(50,100,255),  // blue
    new Color(0,220,220),   // cyan
    new Color(220,210,0),   // yellow
    new Color(220,120,0),   // orange
    new Color(220,220,220), // white
    new Color(0,180,160),   // teal
    new Color(80,180,255),  // skyblue
    new Color(160,60,220),  // purple
    new Color(220,80,180),  // pink
    new Color(80,220,40),   // lime
    new Color(5,10,20),     // darknavy
    new Color(120,130,140), // gray
    new Color(60,65,70),    // darkgray
    new Color(220,185,0),   // gold
    new Color(220,90,70),   // coral
    new Color(200,0,200),   // magenta
    new Color(20,40,120),   // navy
    new Color(100,110,0),   // olive
};

// ── BAR TYPE TAGS (single control chars) ─────────────────
const string T_PWR = "\x01";
const string T_H2  = "\x02";
const string T_O2  = "\x03";
const string T_CRG = "\x04";
const string T_DMG = "\x05";

// ── STATE ─────────────────────────────────────────────────
int  _screen    = 0;
int  _powerView = 0;
int  _prodFilt  = 0;
int[] _scroll   = new int[6];

bool _autoScroll     = false;
int  _autoScrollTick = 0;
const int AUTO_SCROLL_INTERVAL = 200; // ~3.3 seconds at Update100

bool _firstRun = true;
int  _tick     = 0;

List<string> _lastLines = new List<string>();

// ── BLOCK LISTS ───────────────────────────────────────────
List<IMyBatteryBlock>   _batteries  = new List<IMyBatteryBlock>();
List<IMyReactor>        _reactors   = new List<IMyReactor>();
List<IMySolarPanel>     _solar      = new List<IMySolarPanel>();
List<IMyGasTank>        _h2Tanks    = new List<IMyGasTank>();
List<IMyGasTank>        _o2Tanks    = new List<IMyGasTank>();
List<IMyGasGenerator>   _gasGens    = new List<IMyGasGenerator>();
List<IMyCargoContainer> _cargo      = new List<IMyCargoContainer>();
List<IMyAssembler>      _assemblers = new List<IMyAssembler>();
List<IMyRefinery>       _refineries = new List<IMyRefinery>();
List<IMyShipConnector>  _connectors = new List<IMyShipConnector>();
List<IMyOxygenFarm>     _oxyFarms   = new List<IMyOxygenFarm>();
List<IMyAirVent>        _vents      = new List<IMyAirVent>();
List<IMyPowerProducer>  _wind       = new List<IMyPowerProducer>();

List<string> _dmgNames = new List<string>();
List<float>  _dmgPct   = new List<float>();

// ── SURFACE DESCRIPTOR ────────────────────────────────────
class SurfaceEntry
{
    public IMyTextSurface  Surface;
    public IMyTerminalBlock Block;   // for reading CustomData
}
List<SurfaceEntry> _surfaces   = new List<SurfaceEntry>();
IMyTextSurface     _statusSurf = null;

// ═══════════════════════════════════════════════════════════
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

// ═══════════════════════════════════════════════════════════
//  MAIN
// ═══════════════════════════════════════════════════════════
public void Main(string argument, UpdateType updateSource)
{
    if (_firstRun) { ScanGrid(); _firstRun = false; }

    _tick++;
    if (_tick >= 300) { ScanGrid(); _tick = 0; }

    ScanDamaged();
    HandleInput(argument);

    if (_autoScroll)
    {
        _autoScrollTick++;
        if (_autoScrollTick >= AUTO_SCROLL_INTERVAL)
        {
            _autoScrollTick = 0;
            AdvanceScroll(_screen);
        }
    }

    RenderAll();
}

// ═══════════════════════════════════════════════════════════
//  GRID SCAN
// ═══════════════════════════════════════════════════════════
void ScanGrid()
{
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>  (_batteries,  b=>b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMyReactor>       (_reactors,   b=>b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMySolarPanel>    (_solar,      b=>b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>  (_gasGens,    b=>b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(_cargo,      b=>b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>     (_assemblers, b=>b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMyRefinery>      (_refineries, b=>b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector> (_connectors, b=>b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMyOxygenFarm>    (_oxyFarms,   b=>b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMyAirVent>       (_vents,      b=>b.IsSameConstructAs(Me));

    var allTanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(allTanks, b=>b.IsSameConstructAs(Me));
    _h2Tanks.Clear(); _o2Tanks.Clear();
    foreach (var t in allTanks)
        if (t.BlockDefinition.SubtypeId.ToLower().Contains("hydrogen")) _h2Tanks.Add(t);
        else _o2Tanks.Add(t);

    var allPwr = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(allPwr, b=>b.IsSameConstructAs(Me));
    _wind.Clear();
    foreach (var p in allPwr)
        if (!(p is IMyBatteryBlock)&&!(p is IMyReactor)&&!(p is IMySolarPanel)&&!(p is IMyGasGenerator))
            _wind.Add(p);

    // ── Collect drawable surfaces ──────────────────────────
    _surfaces.Clear();
    _statusSurf = null;

    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(allBlocks, b=>b.IsSameConstructAs(Me));

    foreach (var block in allBlocks)
    {
        string bname = block.CustomName;

        if (bname == "SBOS Status")
        {
            var sp = block as IMyTextSurface;
            if (sp != null) _statusSurf = sp;
            continue;
        }

        if (!bname.StartsWith("SBOS")) continue;

        // Determine surface index from optional ":N" suffix
        int surfIdx = 0;
        string baseName = bname;
        int colon = bname.LastIndexOf(':');
        if (colon > 0)
        {
            int.TryParse(bname.Substring(colon+1), out surfIdx);
            baseName = bname.Substring(0, colon);
        }

        var provider = block as IMyTextSurfaceProvider;
        if (provider != null && surfIdx < provider.SurfaceCount)
        {
            EnsureCustomData(block);
            _surfaces.Add(new SurfaceEntry { Surface = provider.GetSurface(surfIdx), Block = block });
            continue;
        }

        var plain = block as IMyTextSurface;
        if (plain != null)
        {
            EnsureCustomData(block);
            _surfaces.Add(new SurfaceEntry { Surface = plain, Block = block });
        }
    }
}

void EnsureCustomData(IMyTerminalBlock block)
{
    if (!string.IsNullOrEmpty(block.CustomData.Trim())) return;
    block.CustomData = DefaultCD();
}

string DefaultCD()
{
    return
        "*power\n*cargo\n*production\n*docked\n*lifesupport\n*damage\n" +
        "*lines-shown - 14\n" +
        "*color-power - green\n" +
        "*color-h2 - red\n" +
        "*color-o2 - cyan\n" +
        "*color-cargo - yellow\n" +
        "*color-damage - orange\n" +
        "*color-text - teal\n" +
        "*color-header - skyblue\n" +
        "*color-background - darknavy\n";
}

void ScanDamaged()
{
    _dmgNames.Clear(); _dmgPct.Clear();
    var all = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(all, b=>b.IsSameConstructAs(Me));
    foreach (var b in all)
    {
        IMySlimBlock slim = b.CubeGrid.GetCubeBlock(b.Position);
        if (slim==null||slim.CurrentDamage<=0f) continue;
        float p = slim.MaxIntegrity>0?(slim.BuildIntegrity-slim.CurrentDamage)/slim.MaxIntegrity:0f;
        _dmgNames.Add(b.CustomName); _dmgPct.Add(p);
    }
}

// ═══════════════════════════════════════════════════════════
//  INPUT
// ═══════════════════════════════════════════════════════════
void HandleInput(string arg)
{
    if (string.IsNullOrEmpty(arg)) return;
    switch (arg.ToUpper())
    {
        case "BTN4":
            var se0 = _surfaces.Count>0 ? _surfaces[0] : null;
            _screen = se0!=null ? NextScreen(se0,_screen) : (_screen+1)%SCREEN_NAMES.Length;
            for (int i=0;i<_scroll.Length;i++) _scroll[i]=0;
            _powerView=0; _prodFilt=0;
            _autoScroll=false; _autoScrollTick=0;
            break;

        case "BTN3":
            if (!_autoScroll)
            {
                // First press → start auto scroll
                _autoScroll=true;
                _autoScrollTick=0;
            }
            else
            {
                // Second press while auto-scrolling → manual step + stop auto
                _autoScroll=false;
                _autoScrollTick=0;
                AdvanceScroll(_screen);
            }
            break;

        case "BTN1":
            if (_screen==SCR_POWER) _powerView=(_powerView+1)%PWR_LABELS.Length;
            if (_screen==SCR_PROD)  _prodFilt=0;
            if (_screen==SCR_DMG)   ScanDamaged();
            break;

        case "BTN2":
            if (_screen==SCR_POWER) _powerView=(_powerView+1)%PWR_LABELS.Length;
            if (_screen==SCR_PROD)  _prodFilt=(_prodFilt+1)%PROD_LABELS.Length;
            if (_screen==SCR_DOCK)  ScanGrid();
            break;
    }
}

void AdvanceScroll(int scr)
{
    int pageSize  = _surfaces.Count>0 ? GetPageSize(_surfaces[0]) : 14;
    int maxScroll = Math.Max(0, _lastLines.Count - pageSize);
    if (_scroll[scr] >= maxScroll) _scroll[scr]=0;
    else _scroll[scr]++;
}

// ── Custom data helpers ───────────────────────────────────
string CD(SurfaceEntry se)  { return se!=null&&se.Block!=null ? se.Block.CustomData : ""; }

bool ScreenEnabled(SurfaceEntry se, int idx)
    { return CD(se).ToLower().Contains(SCREEN_KEYS[idx]); }

int NextScreen(SurfaceEntry se, int current)
{
    for (int i=1;i<=SCREEN_KEYS.Length;i++)
    {
        int idx=(current+i)%SCREEN_KEYS.Length;
        if (ScreenEnabled(se,idx)) return idx;
    }
    return current;
}

int GetPageSize(SurfaceEntry se)
{
    int v = ParseInt(CD(se),"*lines-shown",14);
    return Math.Max(4,v);
}

Color GetColor(SurfaceEntry se, string key, Color def)
    { return ParseColor(CD(se),key,def); }

// ── Color parser — words and R,G,B ────────────────────────
Color ParseColor(string cd, string key, Color def)
{
    string lower = cd.ToLower();
    int pos = lower.IndexOf(key.ToLower());
    if (pos<0) return def;
    int dash = cd.IndexOf('-', pos+key.Length);
    if (dash<0) return def;
    // grab everything after dash on same line
    string rest = cd.Substring(dash+1);
    int nl = rest.IndexOf('\n');
    string val = (nl>=0 ? rest.Substring(0,nl) : rest).Trim().ToLower();

    // Try named color
    for (int i=0;i<COLOR_NAMES.Length;i++)
        if (val==COLOR_NAMES[i]) return COLOR_VALUES[i];

    // Try R,G,B
    string[] parts = val.Split(',');
    if (parts.Length>=3)
    {
        int r,g,b;
        if (int.TryParse(parts[0].Trim(),out r)&&
            int.TryParse(parts[1].Trim(),out g)&&
            int.TryParse(parts[2].Trim(),out b))
            return new Color(r,g,b);
    }
    return def;
}

int ParseInt(string cd, string key, int def)
{
    string lower = cd.ToLower();
    int pos = lower.IndexOf(key.ToLower());
    if (pos<0) return def;
    int dash = cd.IndexOf('-', pos+key.Length);
    if (dash<0) return def;
    string rest = cd.Substring(dash+1);
    int nl = rest.IndexOf('\n');
    string val = (nl>=0?rest.Substring(0,nl):rest).Trim();
    int result; return int.TryParse(val,out result)?result:def;
}

// ═══════════════════════════════════════════════════════════
//  RENDER
// ═══════════════════════════════════════════════════════════
void RenderAll()
{
    _lastLines = BuildLines(_screen);

    foreach (var se in _surfaces)
    {
        if (!ScreenEnabled(se,_screen)) { DrawBlank(se); continue; }
        DrawSurface(se);
    }

    DrawStatusBar();
}

void DrawBlank(SurfaceEntry se)
{
    var surf = se.Surface;
    surf.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
    surf.Script = "";
    var f = surf.DrawFrame();
    Color bg = GetColor(se,"*color-background",new Color(5,10,20));
    Color tx = GetColor(se,"*color-text",new Color(0,180,160));
    Vector2 sz = surf.SurfaceSize;
    Rect(ref f, sz*0.5f, sz, bg);
    Txt(ref f,"[ Screen hidden in Custom Data ]",new Vector2(sz.X*0.5f,sz.Y*0.5f),tx,0.5f,TextAlignment.CENTER);
    f.Dispose();
}

// ── Main surface draw ─────────────────────────────────────
void DrawSurface(SurfaceEntry se)
{
    var    surf = se.Surface;
    Vector2 sz  = surf.SurfaceSize;

    // ── Read config ───────────────────────────────────────
    Color cBg  = GetColor(se,"*color-background",new Color(5,10,20));
    Color cTx  = GetColor(se,"*color-text",       new Color(0,180,160));
    Color cHd  = GetColor(se,"*color-header",     new Color(80,180,255));
    Color cPwr = GetColor(se,"*color-power",      new Color(0,220,80));
    Color cH2  = GetColor(se,"*color-h2",         new Color(220,50,50));
    Color cO2  = GetColor(se,"*color-o2",         new Color(0,220,220));
    Color cCrg = GetColor(se,"*color-cargo",      new Color(220,210,0));
    Color cDmg = GetColor(se,"*color-damage",     new Color(220,120,0));
    Color cWrn = new Color(220,150,0);
    Color cCrt = new Color(220,40,40);
    Color cEmp = new Color(25,35,50);
    Color cHdBg= new Color(
        (int)(cBg.R*0.5f)+15,
        (int)(cBg.G*0.5f)+25,
        (int)(cBg.B*0.5f)+35);

    int   pageSize = GetPageSize(se);
    string gridName = Me.CubeGrid.CustomName;

    // ── Layout (all pixel values derived from surface size) ─
    float headerH  = sz.Y * 0.115f;   // fixed header band
    float footerH  = sz.Y * 0.095f;   // fixed footer band
    float divH     = 2f;
    float padX     = sz.X * 0.018f;

    float contentTop    = headerH + divH;
    float contentBottom = sz.Y - footerH - divH;
    float contentH      = contentBottom - contentTop;
    float lineH         = contentH / pageSize;

    // Text scales — tuned so ~512px wide feels comfortable
    float tsMain   = Clamp(sz.X / 780f, 0.38f, 1.1f);
    float tsHdr    = Clamp(sz.X / 700f, 0.40f, 1.1f);
    float tsFooter = Clamp(sz.X / 820f, 0.34f, 1.0f);
    float tsBar    = Clamp(sz.X / 820f, 0.36f, 0.9f);

    surf.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
    surf.Script = "";
    var frame = surf.DrawFrame();

    // ── Background ────────────────────────────────────────
    Rect(ref frame, sz*0.5f, sz, cBg);

    // ── Header band ───────────────────────────────────────
    Rect(ref frame, new Vector2(sz.X*0.5f, headerH*0.5f), new Vector2(sz.X, headerH), cHdBg);
    Rect(ref frame, new Vector2(sz.X*0.5f, headerH), new Vector2(sz.X, divH), cHd);

    // Header row 1: SBOS version | Grid Name | Screen Name
    // Each of the 3 items gets 1/3 of width
    float hRowY = headerH*0.18f;
    float col1x = padX;
    float col2x = sz.X*0.5f;
    float col3x = sz.X - padX;
    Txt(ref frame, "SBOS v3.1",           new Vector2(col1x, hRowY), cHd, tsHdr, TextAlignment.LEFT);
    Txt(ref frame, gridName,              new Vector2(col2x, hRowY), cTx, tsHdr, TextAlignment.CENTER);
    Txt(ref frame, SCREEN_NAMES[_screen], new Vector2(col3x, hRowY), cHd, tsHdr, TextAlignment.RIGHT);

    // Header row 2: auto-scroll indicator (right-aligned, small)
    // Sits in lower half of header so it never overlaps row 1
    if (_autoScroll)
    {
        float hRow2Y = headerH*0.57f;
        Txt(ref frame, "[ AUTO-SCROLL ]", new Vector2(col3x, hRow2Y), cWrn, tsHdr*0.78f, TextAlignment.RIGHT);
    }

    // ── Content lines ─────────────────────────────────────
    int scroll    = _scroll[_screen];
    int maxScroll = Math.Max(0, _lastLines.Count - pageSize);
    if (scroll > maxScroll) { scroll = maxScroll; _scroll[_screen] = scroll; }

    float cy = contentTop + 2f;

    for (int i=scroll; i<_lastLines.Count && i<scroll+pageSize; i++, cy+=lineH)
    {
        string raw = _lastLines[i];
        if (raw.Length==0) continue;

        char tag = raw[0];
        bool isBar = tag==T_PWR[0]||tag==T_H2[0]||tag==T_O2[0]||tag==T_CRG[0]||tag==T_DMG[0];

        if (isBar)
        {
            Color barCol = tag==T_PWR[0]?cPwr:tag==T_H2[0]?cH2:tag==T_O2[0]?cO2:tag==T_CRG[0]?cCrg:cDmg;
            string[] parts = raw.Substring(1).Split('|');
            float ratio=0f; string label="";
            if (parts.Length>=1) float.TryParse(parts[0],out ratio);
            if (parts.Length>=2) label=parts[1];

            // Bar occupies left 48% of content width; label to the right
            float barW  = (sz.X - padX*2f)*0.48f;
            float barH2 = lineH*0.65f;
            float barX  = padX;
            float barY  = cy + (lineH-barH2)*0.5f;

            // Track
            Rect(ref frame, new Vector2(barX+barW*0.5f, barY+barH2*0.5f), new Vector2(barW,barH2), cEmp);
            // Fill
            if (ratio>0f)
            {
                float filled = barW*Clamp(ratio,0f,1f);
                Rect(ref frame, new Vector2(barX+filled*0.5f, barY+barH2*0.5f), new Vector2(filled,barH2), barCol);
            }
            // Pct text just right of bar
            string pct = (ratio*100f).ToString("F0")+"%";
            float pctX = barX+barW+padX*0.5f;
            Txt(ref frame, pct,   new Vector2(pctX, cy), barCol, tsBar, TextAlignment.LEFT);
            // Label further right
            if (!string.IsNullOrEmpty(label))
                Txt(ref frame, label, new Vector2(pctX+sz.X*0.075f, cy), cTx, tsBar*0.88f, TextAlignment.LEFT);
        }
        else
        {
            Color col = cTx;
            if (raw.Contains("CRITICAL")||raw.Contains("! CRIT")) col=cCrt;
            else if (raw.Contains("WARNING")||raw.Contains("! WARN")) col=cWrn;
            else if (raw.StartsWith("  ┌")||raw.StartsWith("  └")||raw.Contains("-- ")) col=cHd;
            Txt(ref frame, raw, new Vector2(padX, cy), col, tsMain, TextAlignment.LEFT);
        }
    }

    // ── Scroll pip (right edge) ────────────────────────────
    if (_lastLines.Count > pageSize && maxScroll>0)
    {
        float pipW     = Math.Max(3f, sz.X*0.008f);
        float pipX     = sz.X - pipW*0.5f - 1f;
        float trackTop = contentTop+2f;
        float trackH   = contentH-4f;
        float pct2     = (float)scroll/maxScroll;
        float thumbH   = Math.Max(pipW*3f, trackH*0.08f);
        float thumbY   = trackTop + pct2*(trackH-thumbH) + thumbH*0.5f;
        Rect(ref frame, new Vector2(pipX, trackTop+trackH*0.5f), new Vector2(pipW, trackH), cEmp);
        Rect(ref frame, new Vector2(pipX, thumbY),               new Vector2(pipW*2.5f, thumbH), cTx);
    }

    // ── Footer band ───────────────────────────────────────
    float footerTop = sz.Y - footerH;
    Rect(ref frame, new Vector2(sz.X*0.5f, footerTop), new Vector2(sz.X, divH), cHd);
    Rect(ref frame, new Vector2(sz.X*0.5f, footerTop+divH+footerH*0.5f),
                    new Vector2(sz.X, footerH), cHdBg);

    // 4 buttons evenly spaced, centered in footer, all on one line
    string[] btnLabels = GetButtonLabels(_screen);
    float btnSpacing = sz.X / 4f;
    float btnY = footerTop + divH + footerH*0.1f;
    for (int b=0;b<4;b++)
    {
        float bx = btnSpacing*b + btnSpacing*0.5f;
        Color bc = (b==3)?cHd:(b==2)?cWrn:cTx;
        Txt(ref frame, btnLabels[b], new Vector2(bx, btnY), bc, tsFooter, TextAlignment.CENTER);
    }

    frame.Dispose();
}

float Clamp(float v, float lo, float hi){ return v<lo?lo:v>hi?hi:v; }

string[] GetButtonLabels(int screen)
{
    switch (screen)
    {
        case SCR_POWER: return new[]{"[1] Cycle View","[2] Cycle View","[3] Scroll/Auto","[4] Next Screen"};
        case SCR_PROD:  return new[]{"[1] Reset Filt","[2] Cycle Filt","[3] Scroll/Auto","[4] Next Screen"};
        case SCR_DOCK:  return new[]{"[1] ------","[2] Rescan","[3] Scroll/Auto","[4] Next Screen"};
        case SCR_DMG:   return new[]{"[1] Rescan","[2] ------","[3] Scroll/Auto","[4] Next Screen"};
        default:        return new[]{"[1] ------","[2] ------","[3] Scroll/Auto","[4] Next Screen"};
    }
}

void DrawStatusBar()
{
    if (_statusSurf==null) return;
    float bat = BatPct();
    _statusSurf.ContentType     = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    _statusSurf.FontColor       = new Color(0,220,180);
    _statusSurf.BackgroundColor = new Color(5,10,20);
    _statusSurf.Font            = "Monospace";
    _statusSurf.FontSize        = 0.8f;
    string s = "SBOS | "+Me.CubeGrid.CustomName+" | "+SCREEN_NAMES[_screen]+"\n";
    s += "PWR ["+FillBar((int)(bat*20f),20)+"] "+(bat*100f).ToString("F0")+"%";
    if (_dmgNames.Count>0) s += "  ! DMG:"+_dmgNames.Count;
    if (_autoScroll) s += "  [AUTO]";
    _statusSurf.WriteText(s);
}

// ── Sprite helpers ────────────────────────────────────────
void Rect(ref MySpriteDrawFrame f, Vector2 center, Vector2 size, Color col)
    { f.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",center,size,col)); }

void Txt(ref MySpriteDrawFrame f, string text, Vector2 pos, Color col, float scale,
         TextAlignment align=TextAlignment.LEFT)
    { f.Add(new MySprite(SpriteType.TEXT,text,pos,null,col,"Monospace",align,scale)); }

// ═══════════════════════════════════════════════════════════
//  LINE BUILDERS
// ═══════════════════════════════════════════════════════════
List<string> BuildLines(int screen)
{
    switch(screen)
    {
        case SCR_POWER: return PowerLines();
        case SCR_CARGO: return CargoLines();
        case SCR_PROD:  return ProdLines();
        case SCR_DOCK:  return DockedLines();
        case SCR_LIFE:  return LifeLines();
        case SCR_DMG:   return DmgLines();
    }
    return new List<string>{"Unknown screen"};
}

// Encode a bar line: tag + ratio (4dp) + | + label
string BL(string tag, float ratio, string label)
    { return tag+Clamp(ratio,0f,1f).ToString("F4")+"|"+label; }

// ── POWER ─────────────────────────────────────────────────
List<string> PowerLines()
{
    var L=new List<string>();
    float bc=0,bm=0,to=0,mo=0,ro=0,so=0,wo=0;
    foreach(var b in _batteries){bc+=b.CurrentStoredPower;bm+=b.MaxStoredPower;to+=b.CurrentOutput;mo+=b.MaxOutput;}
    foreach(var r in _reactors) ro+=r.CurrentOutput;
    foreach(var s in _solar)    so+=s.CurrentOutput;
    foreach(var w in _wind)     wo+=w.CurrentOutput;
    float ti=ro+so+wo,br=bm>0?bc/bm:0f,lr=mo>0?to/mo:0f,net=ti-to;

    L.Add("  -- "+PWR_LABELS[_powerView]+" --"); L.Add("");
    if (_powerView==0)
    {
        L.Add(BL(T_PWR,br,"Battery  "+FmtMW(bc)+" / "+FmtMW(bm)));
        L.Add(BL(T_PWR,lr,"Load     "+FmtMW(to)+" / "+FmtMW(mo)));
        L.Add("  Input  : "+FmtMW(ti));
        L.Add("  Output : "+FmtMW(to));
        L.Add("  Net    : "+(net>=0?"+":"")+FmtMW(net));
        if (br<0.2f) L.Add("  ! WARNING: LOW BATTERY");
        if (net<0f&&br<0.05f) L.Add("  ! CRITICAL: POWER DEFICIT");
    }
    else if (_powerView==1)
    {
        L.Add("  REACTORS ("+_reactors.Count+")");
        foreach(var r in _reactors) L.Add("   "+Pad(Tr(r.CustomName,22),22)+" "+FmtMW(r.CurrentOutput));
        L.Add("  SOLAR  ("+_solar.Count+")  "+FmtMW(so));
        L.Add("  WIND   ("+_wind.Count +")  "+FmtMW(wo));
        L.Add(""); L.Add("  BATTERIES ("+_batteries.Count+")");
        foreach(var b in _batteries){float r2=b.MaxStoredPower>0?b.CurrentStoredPower/b.MaxStoredPower:0f;L.Add(BL(T_PWR,r2,Tr(b.CustomName,22)+"  "+(r2*100f).ToString("F0")+"%"));}
    }
    else
    {
        L.Add("  Draw  : "+FmtMW(to)); L.Add("  Supply: "+FmtMW(mo));
        L.Add(BL(T_PWR,lr,"Load "+(lr*100f).ToString("F0")+"%"));
        L.Add("  Net   : "+(net>=0?"+":"")+FmtMW(net));
        L.Add("  React : "+FmtMW(ro)); L.Add("  Solar : "+FmtMW(so)); L.Add("  Wind  : "+FmtMW(wo));
    }
    return L;
}

// ── CARGO ─────────────────────────────────────────────────
List<string> CargoLines()
{
    var L=new List<string>(); var items=new Dictionary<string,float>(); float vc=0,vm=0;
    foreach(var c in _cargo)
        for(int i=0;i<c.InventoryCount;i++)
        {
            var inv=c.GetInventory(i); vc+=(float)inv.CurrentVolume; vm+=(float)inv.MaxVolume;
            var il=new List<MyInventoryItem>(); inv.GetItems(il);
            foreach(var it in il){string n=it.Type.SubtypeId;float a=(float)it.Amount;if(items.ContainsKey(n))items[n]+=a;else items[n]=a;}
        }
    float vr=vm>0?vc/vm:0f;
    L.Add("  Containers: "+_cargo.Count);
    L.Add(BL(T_CRG,vr,"Volume  "+FmtVol(vc)+" / "+FmtVol(vm)));
    L.Add("  ─────────────────────────────────────");
    var keys=new List<string>(items.Keys); keys.Sort((a,b)=>items[b].CompareTo(items[a]));
    foreach(var k in keys) L.Add("  "+Pad(Tr(k,26),26)+" "+FmtAmt(items[k]));
    if(keys.Count==0) L.Add("  No items found.");
    L.Add(""); L.Add("  Total: "+keys.Count+" type(s)");
    return L;
}

// ── PRODUCTION ────────────────────────────────────────────
List<string> ProdLines()
{
    var L=new List<string>(); L.Add("  Filter: "+PROD_LABELS[_prodFilt]); L.Add("");
    if(_prodFilt==0||_prodFilt==1)
    {
        int ac=0; foreach(var a in _assemblers)if(a.IsProducing)ac++;
        L.Add("  ASSEMBLERS ("+_assemblers.Count+")  Active: "+ac);
        foreach(var a in _assemblers)
        {
            L.Add("  ["+(a.IsProducing?">":"=")+"] "+Tr(a.CustomName,30));
            if(!a.IsQueueEmpty){var q=new List<MyProductionItem>();a.GetQueue(q);if(q.Count>0)L.Add("     -> "+Tr(q[0].BlueprintId.SubtypeName,26)+" x"+((float)q[0].Amount).ToString("F0"));}
        }
        L.Add("");
    }
    if(_prodFilt==0||_prodFilt==2)
    {
        int ac=0; foreach(var r in _refineries)if(r.IsProducing)ac++;
        L.Add("  REFINERIES ("+_refineries.Count+")  Active: "+ac);
        foreach(var r in _refineries)
        {
            L.Add("  ["+(r.IsProducing?">":"=")+"] "+Tr(r.CustomName,30));
            if(!r.IsQueueEmpty){var q=new List<MyProductionItem>();r.GetQueue(q);if(q.Count>0)L.Add("     -> "+Tr(q[0].BlueprintId.SubtypeName,28));}
        }
    }
    return L;
}

// ── DOCKED SHIPS ──────────────────────────────────────────
List<string> DockedLines()
{
    var L=new List<string>(); int docked=0; var seenIds=new List<long>();

    // Scan all connectors in terminal — not filtered by construct
    // so multiplayer foreign grids are visible
    var allCons=new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(allCons);
    var myCons=new List<IMyShipConnector>();
    foreach(var c in allCons) if(c.IsSameConstructAs(Me)) myCons.Add(c);

    foreach(var con in myCons)
    {
        bool conn = con.Status==MyShipConnectorStatus.Connected;
        string st  = conn?"DOCKED":con.Status==MyShipConnectorStatus.Connectable?"READY ":"FREE  ";
        L.Add("  ["+(conn?"O":"o")+"] "+Pad(Tr(con.CustomName,20),20)+" ["+st+"]");

        if(!conn||con.OtherConnector==null){L.Add("");continue;}
        docked++;

        IMyCubeGrid dg=con.OtherConnector.CubeGrid;
        bool seen=false; foreach(var id in seenIds)if(id==dg.EntityId){seen=true;break;}
        if(seen){L.Add("      (same ship — see above)");L.Add("");continue;}
        seenIds.Add(dg.EntityId);

        L.Add("  ┌─ "+Tr(dg.CustomName,32));

        // Power
        var dBat=new List<IMyBatteryBlock>(); GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(dBat,b=>b.CubeGrid.EntityId==dg.EntityId);
        if(dBat.Count>0){float bc2=0,bm2=0,bo2=0;foreach(var b in dBat){bc2+=b.CurrentStoredPower;bm2+=b.MaxStoredPower;bo2+=b.CurrentOutput;}float br2=bm2>0?bc2/bm2:0f;L.Add(BL(T_PWR,br2,"PWR  "+FmtMW(bc2)+"/"+FmtMW(bm2)+" draw:"+FmtMW(bo2)));}
        var dRct=new List<IMyReactor>(); GridTerminalSystem.GetBlocksOfType<IMyReactor>(dRct,b=>b.CubeGrid.EntityId==dg.EntityId);
        if(dRct.Count>0){float ro2=0;foreach(var r in dRct)ro2+=r.CurrentOutput;L.Add("  |  Reactors: "+dRct.Count+"  out:"+FmtMW(ro2));}

        // H2
        var dH2=new List<IMyGasTank>(); GridTerminalSystem.GetBlocksOfType<IMyGasTank>(dH2,b=>b.CubeGrid.EntityId==dg.EntityId&&b.BlockDefinition.SubtypeId.ToLower().Contains("hydrogen"));
        if(dH2.Count>0){float hf=0;foreach(var t in dH2)hf+=(float)t.FilledRatio;float hr=hf/dH2.Count;L.Add(BL(T_H2,hr,"H2   "+(hr*100f).ToString("F0")+"% ("+dH2.Count+" tanks)"));}

        // O2
        var dO2=new List<IMyGasTank>(); GridTerminalSystem.GetBlocksOfType<IMyGasTank>(dO2,b=>b.CubeGrid.EntityId==dg.EntityId&&!b.BlockDefinition.SubtypeId.ToLower().Contains("hydrogen"));
        if(dO2.Count>0){float of2=0;foreach(var t in dO2)of2+=(float)t.FilledRatio;float or2=of2/dO2.Count;L.Add(BL(T_O2,or2,"O2   "+(or2*100f).ToString("F0")+"% ("+dO2.Count+" tanks)"));}

        // Cargo
        var dCrg=new List<IMyCargoContainer>(); GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(dCrg,b=>b.CubeGrid.EntityId==dg.EntityId);
        if(dCrg.Count>0)
        {
            float cc=0,cm=0; var di=new Dictionary<string,float>();
            foreach(var c in dCrg)for(int i=0;i<c.InventoryCount;i++){var inv=c.GetInventory(i);cc+=(float)inv.CurrentVolume;cm+=(float)inv.MaxVolume;var il=new List<MyInventoryItem>();inv.GetItems(il);foreach(var it in il){string n=it.Type.SubtypeId;float a=(float)it.Amount;if(di.ContainsKey(n))di[n]+=a;else di[n]=a;}}
            float cr=cm>0?cc/cm:0f;
            L.Add(BL(T_CRG,cr,"CARGO "+FmtVol(cc)+"/"+FmtVol(cm)+" ("+dCrg.Count+")"));
            var dk=new List<string>(di.Keys);dk.Sort((a,b)=>di[b].CompareTo(di[a]));
            int sh=0;foreach(var k in dk){if(sh>=4)break;L.Add("  |   "+Pad(Tr(k,22),22)+" "+FmtAmt(di[k]));sh++;}
            if(dk.Count>4)L.Add("  |   ...+"+(dk.Count-4)+" more types");
        }

        // Damage
        var dBlk=new List<IMyTerminalBlock>(); GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(dBlk,b=>b.CubeGrid.EntityId==dg.EntityId);
        var dn=new List<string>();var dp2=new List<float>();
        foreach(var b in dBlk){IMySlimBlock sl=b.CubeGrid.GetCubeBlock(b.Position);if(sl==null||sl.CurrentDamage<=0f)continue;float p=sl.MaxIntegrity>0?(sl.BuildIntegrity-sl.CurrentDamage)/sl.MaxIntegrity:0f;dn.Add(b.CustomName);dp2.Add(p);}
        if(dn.Count==0){L.Add("  |  Hull: OK");}
        else
        {
            L.Add("  |  ! DAMAGE: "+dn.Count+" block(s)");
            for(int i=0;i<Math.Min(dn.Count,5);i++)
                L.Add(BL(T_DMG,dp2[i],Pad(Tr(dn[i],22),22)+" "+(dp2[i]*100f).ToString("F0")+"%"));
            if(dn.Count>5)L.Add("  |   ...+"+(dn.Count-5)+" more damaged");
        }

        L.Add("  └──────────────────────────────────"); L.Add("");
    }

    if(myCons.Count==0) L.Add("  No connectors found on grid.");
    L.Add("  Connectors: "+myCons.Count+"   Docked: "+docked+"   Free: "+(myCons.Count-docked));
    return L;
}

// ── LIFE SUPPORT ──────────────────────────────────────────
List<string> LifeLines()
{
    var L=new List<string>();
    float o2f=0;foreach(var t in _o2Tanks)o2f+=(float)t.FilledRatio;
    float o2r=_o2Tanks.Count>0?o2f/_o2Tanks.Count:0f;
    float h2f=0;foreach(var t in _h2Tanks)h2f+=(float)t.FilledRatio;
    float h2r=_h2Tanks.Count>0?h2f/_h2Tanks.Count:0f;
    int vok=0;float tO2l=0;foreach(var v in _vents){if(v.CanPressurize)vok++;tO2l+=v.GetOxygenLevel();}
    float avgO2=_vents.Count>0?tO2l/_vents.Count:0f;
    int ggOn=0;foreach(var g in _gasGens)if(g.Enabled)ggOn++;

    L.Add("  OXYGEN TANKS ("+_o2Tanks.Count+")");
    L.Add(BL(T_O2,o2r,"O2 avg  "+(o2r*100f).ToString("F0")+"%"));
    foreach(var t in _o2Tanks) L.Add(BL(T_O2,(float)t.FilledRatio,Tr(t.CustomName,24)+"  "+((float)t.FilledRatio*100f).ToString("F0")+"%"));
    L.Add("");
    L.Add("  HYDROGEN TANKS ("+_h2Tanks.Count+")");
    L.Add(BL(T_H2,h2r,"H2 avg  "+(h2r*100f).ToString("F0")+"%"));
    foreach(var t in _h2Tanks) L.Add(BL(T_H2,(float)t.FilledRatio,Tr(t.CustomName,24)+"  "+((float)t.FilledRatio*100f).ToString("F0")+"%"));
    L.Add("");
    L.Add("  O2/H2 Generators : "+ggOn+"/"+_gasGens.Count+" active");
    L.Add("  Oxygen Farms     : "+_oxyFarms.Count+" units");
    L.Add("  Air Vents        : "+vok+"/"+_vents.Count+" pressurizable");
    L.Add("  Avg O2 Level     : "+(avgO2*100f).ToString("F1")+"%");
    if(o2r<0.2f){L.Add("");L.Add("  ! WARNING: LOW OXYGEN");}
    if(h2r<0.1f) L.Add("  ! WARNING: LOW HYDROGEN");
    return L;
}

// ── DAMAGE ────────────────────────────────────────────────
List<string> DmgLines()
{
    var L=new List<string>();
    if(_dmgNames.Count==0){L.Add("  All systems nominal.");L.Add("  No structural damage detected.");}
    else
    {
        L.Add("  DAMAGED BLOCKS: "+_dmgNames.Count); L.Add("");
        for(int i=0;i<_dmgNames.Count;i++)
            L.Add(BL(T_DMG,_dmgPct[i],Pad(Tr(_dmgNames[i],26),26)+" "+(_dmgPct[i]*100f).ToString("F0")+"%"));
    }
    return L;
}

// ═══════════════════════════════════════════════════════════
//  UTILITY
// ═══════════════════════════════════════════════════════════
float BatPct(){float c=0,m=0;foreach(var b in _batteries){c+=b.CurrentStoredPower;m+=b.MaxStoredPower;}return m>0?c/m:0f;}
string FillBar(int n,int max){string s="";for(int i=0;i<max;i++)s+=i<n?"█":"░";return s;}
string FmtMW(float v){if(v>=1000f)return(v/1000f).ToString("F1")+"GW";if(v>=1f)return v.ToString("F1")+"MW";return(v*1000f).ToString("F0")+"kW";}
string FmtVol(float v){if(v>=1000f)return(v/1000f).ToString("F1")+"ML";return v.ToString("F1")+"kL";}
string FmtAmt(float v){if(v>=1000000f)return(v/1000000f).ToString("F1")+"M";if(v>=1000f)return(v/1000f).ToString("F1")+"K";return v.ToString("F0");}
string Tr(string s,int m){if(s==null)return"";return s.Length>m?s.Substring(0,m):s;}
string Pad(string s,int w){if(s==null)s="";if(s.Length>=w)return s.Substring(0,w);return s+new string(' ',w-s.Length);}