// ===== DUTC POWER DECK ===== v1.1
// v1.1: fixed docked ships hijacking each other's LCDs; script now only touches
//       blocks on its own construct unless INCLUDE_DOCKED is set to true.
// Battery manager + 5 display types. Tag LCD names:
//   [BATT]  = battery status       [POWER] = power producers on/off list
//   [H2]    = hydrogen tanks       [O2]    = oxygen tanks
//   [GAS]   = hydrogen + oxygen on one screen
// Cockpit screens: [BATT:0] = first screen, [POWER:1] = second screen, etc.
// Toolbar arguments (optional): "on" = force backups on, "off" = force off, "auto" = automatic

double LOW = 20;               // % - backups turn ON at or below
double HIGH = 90;              // % - backups turn OFF at or above
bool CONTROL_HYDROGEN = true;  // auto-toggle hydrogen engines
bool CONTROL_REACTORS = true;  // auto-toggle reactors
bool INCLUDE_DOCKED = false;   // false = only this ship/station (subgrids with rotors/pistons included)
                               // true  = also monitor and control ships docked via connectors
double PAGE_SECONDS = 3;       // how long each list page shows before scrolling
string IGNORE_TAG = "[ignore]"; // exempt a block from control/monitoring

//======= no need to edit below =======

Color COL_BG    = new Color(8, 10, 16);
Color COL_FRAME = new Color(95, 105, 125);
Color COL_TEXT  = new Color(210, 220, 235);
Color COL_DIM   = new Color(110, 120, 140);
Color COL_GOOD  = new Color(80, 220, 120);
Color COL_WARN  = new Color(255, 190, 60);
Color COL_BAD   = new Color(255, 80, 70);
Color COL_H2    = new Color(255, 140, 60);
Color COL_O2    = new Color(80, 180, 255);

string mode = "auto";
bool backupsOn = false;
int rescan = 0, tick = 0;
int hydroCount = 0, reactorCount = 0, battCount = 0;
double battPct, battNet, battIn, battOut, prodOut, prodMax;
double h2Stored, h2Cap, o2Stored, o2Cap;
string battTime = "";

List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyPowerProducer> producers = new List<IMyPowerProducer>();
List<IMyPowerProducer> backups = new List<IMyPowerProducer>();
List<IMyPowerProducer> aliveProds = new List<IMyPowerProducer>();
List<IMyGasTank> h2Tanks = new List<IMyGasTank>();
List<IMyGasTank> o2Tanks = new List<IMyGasTank>();
List<IMyGasTank> aliveTanks = new List<IMyGasTank>();
List<IMyTerminalBlock> tagged = new List<IMyTerminalBlock>();
List<IMyTextSurface> battScreens = new List<IMyTextSurface>();
List<IMyTextSurface> powerScreens = new List<IMyTextSurface>();
List<IMyTextSurface> h2Screens = new List<IMyTextSurface>();
List<IMyTextSurface> o2Screens = new List<IMyTextSurface>();
List<IMyTextSurface> gasScreens = new List<IMyTextSurface>();

public Program()
{
    Scan();
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string arg, UpdateType src)
{
    tick++;
    arg = arg.Trim().ToLower();
    if (arg == "on" || arg == "off" || arg == "auto") mode = arg;
    if (--rescan <= 0) { Scan(); rescan = 60; } // re-scan every ~10s

    // ---- batteries ----
    double stored = 0, max = 0;
    battIn = 0; battOut = 0; battCount = 0;
    foreach (var b in batteries)
    {
        if (b.Closed) continue;
        battCount++;
        stored += b.CurrentStoredPower; max += b.MaxStoredPower;
        battIn += b.CurrentInput; battOut += b.CurrentOutput;
    }
    battPct = max > 0 ? stored / max * 100.0 : 0;
    battNet = battIn - battOut;
    battTime = battNet > 0.001 && max > 0 ? "FULL IN " + FmtTime((max - stored) / battNet)
             : battNet < -0.001 ? "EMPTY IN " + FmtTime(stored / -battNet) : "";

    // ---- backup control (hysteresis) ----
    if (mode == "on") backupsOn = true;
    else if (mode == "off") backupsOn = false;
    else
    {
        if (battPct <= LOW) backupsOn = true;
        else if (battPct >= HIGH) backupsOn = false;
    }
    foreach (var p in backups) if (!p.Closed) p.Enabled = backupsOn;

    // ---- producers ----
    prodOut = 0; prodMax = 0;
    foreach (var p in producers)
    {
        if (p.Closed) continue;
        prodOut += p.CurrentOutput; prodMax += p.MaxOutput;
    }

    // ---- gas ----
    h2Stored = 0; h2Cap = 0; o2Stored = 0; o2Cap = 0;
    foreach (var t in h2Tanks) { if (t.Closed) continue; h2Cap += t.Capacity; h2Stored += t.Capacity * t.FilledRatio; }
    foreach (var t in o2Tanks) { if (t.Closed) continue; o2Cap += t.Capacity; o2Stored += t.Capacity * t.FilledRatio; }

    Echo("Dutc Power Deck [" + mode.ToUpper() + "]");
    Echo("Batteries: " + battPct.ToString("0.0") + "%  Backups: " + (backupsOn ? "ON" : "OFF"));
    Echo("H2: " + Pct(h2Stored, h2Cap) + "%  O2: " + Pct(o2Stored, o2Cap) + "%");
    Echo("Screens B/P/H/O/G: " + battScreens.Count + "/" + powerScreens.Count + "/" +
        h2Screens.Count + "/" + o2Screens.Count + "/" + gasScreens.Count);

    foreach (var s in battScreens) DrawBatt(s);
    foreach (var s in powerScreens) DrawPower(s);
    foreach (var s in h2Screens) DrawGas(s, "HYDROGEN", COL_H2, h2Tanks, h2Stored, h2Cap);
    foreach (var s in o2Screens) DrawGas(s, "OXYGEN", COL_O2, o2Tanks, o2Stored, o2Cap);
    foreach (var s in gasScreens) DrawGasSplit(s);
}

// true if this block belongs to us: same construct as the PB (rotors/pistons included,
// connector-docked grids excluded), or anything reachable when INCLUDE_DOCKED is on
bool Mine(IMyTerminalBlock b)
{
    if (INCLUDE_DOCKED) return true;
    return b.IsSameConstructAs(Me);
}

void Scan()
{
    GridTerminalSystem.GetBlocksOfType(batteries, b =>
        Mine(b) && !b.CustomName.Contains(IGNORE_TAG));

    producers.Clear(); backups.Clear();
    hydroCount = 0; reactorCount = 0;
    var prods = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType(prods, b =>
        Mine(b) && !b.CustomName.Contains(IGNORE_TAG));
    foreach (var p in prods)
    {
        if (p is IMyBatteryBlock) continue;
        producers.Add(p);
        bool hydro = p.BlockDefinition.TypeIdString.Contains("HydrogenEngine");
        if (hydro && CONTROL_HYDROGEN) { backups.Add(p); hydroCount++; }
        else if (!hydro && p is IMyReactor && CONTROL_REACTORS) { backups.Add(p); reactorCount++; }
    }

    h2Tanks.Clear(); o2Tanks.Clear();
    var tanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType(tanks, b =>
        Mine(b) && !b.CustomName.Contains(IGNORE_TAG));
    foreach (var t in tanks)
    {
        if (t.BlockDefinition.SubtypeId.Contains("Hydrogen")) h2Tanks.Add(t);
        else o2Tanks.Add(t);
    }

    // screens are grid-filtered too, otherwise a docked ship's script hijacks the base's LCDs
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(tagged, b =>
        Mine(b) && b.CustomName.Contains("["));
    Collect("BATT", battScreens);
    Collect("POWER", powerScreens);
    Collect("H2", h2Screens);
    Collect("O2", o2Screens);
    Collect("GAS", gasScreens);
    battScreens.Add(Me.GetSurface(0)); // PB's own screen shows battery view
}

void Collect(string tag, List<IMyTextSurface> list)
{
    list.Clear();
    foreach (var b in tagged)
    {
        string name = b.CustomName.ToUpper();
        int i = name.IndexOf("[" + tag);
        if (i < 0) continue;
        int end = name.IndexOf(']', i);
        if (end < 0) continue;
        string inner = name.Substring(i + 1, end - i - 1);
        string baseTag = inner; int idx = 0;
        int colon = inner.IndexOf(':');
        if (colon >= 0)
        {
            baseTag = inner.Substring(0, colon);
            int.TryParse(inner.Substring(colon + 1), out idx);
        }
        if (baseTag != tag) continue;

        var prov = b as IMyTextSurfaceProvider;
        var surf = b as IMyTextSurface;
        if (prov != null && idx >= 0 && idx < prov.SurfaceCount) list.Add(prov.GetSurface(idx));
        else if (surf != null) list.Add(surf);
    }
}

// current page for a list: cycles automatically every PAGE_SECONDS
int Page(int pageCount)
{
    if (pageCount <= 1) return 0;
    int hold = Math.Max(1, (int)(PAGE_SECONDS * 6)); // Update10 = 6 runs/second
    return (tick / hold) % pageCount;
}

// ================= DISPLAYS =================

void DrawBatt(IMyTextSurface s)
{
    var frame = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f;

    Color col = battPct >= 50 ? COL_GOOD : (battPct >= LOW ? COL_WARN : COL_BAD);
    frame.Add(Txt("BATTERY STATUS", off + new Vector2(W * 0.5f, H * 0.03f), 1.1f * sc, COL_DIM));

    float bodyW = W * 0.62f, bodyH = H * 0.30f, t = 6f * sc;
    Vector2 c = off + new Vector2(W * 0.46f, H * 0.32f);
    frame.Add(Box(c, new Vector2(bodyW + t * 2, bodyH + t * 2), COL_FRAME));
    frame.Add(Box(c, new Vector2(bodyW, bodyH), COL_BG));
    frame.Add(Box(c + new Vector2(bodyW * 0.5f + t + W * 0.02f, 0), new Vector2(W * 0.04f, bodyH * 0.4f), COL_FRAME));
    float pad = 5f * sc;
    float fw = (float)((bodyW - pad * 2) * Clamp01(battPct / 100.0));
    if (fw > 1) frame.Add(Box(c + new Vector2(-(bodyW - pad * 2) * 0.5f + fw * 0.5f, 0), new Vector2(fw, bodyH - pad * 2), col));
    frame.Add(Txt(battPct.ToString("0") + "%", c - new Vector2(0, H * 0.075f), 2.4f * sc, COL_TEXT));

    string flow = battNet > 0.001 ? "CHARGING  +" + FmtPower(battNet)
                : battNet < -0.001 ? "DISCHARGING  -" + FmtPower(-battNet) : "IDLE";
    Color fcol = battNet > 0.001 ? COL_GOOD : (battNet < -0.001 ? COL_WARN : COL_DIM);
    frame.Add(Txt(flow, off + new Vector2(W * 0.5f, H * 0.53f), 0.95f * sc, fcol));
    frame.Add(Txt("IN " + FmtPower(battIn) + "    OUT " + FmtPower(battOut), off + new Vector2(W * 0.5f, H * 0.62f), 0.8f * sc, COL_DIM));
    if (battTime != "") frame.Add(Txt(battTime, off + new Vector2(W * 0.5f, H * 0.69f), 0.8f * sc, COL_TEXT));

    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.79f), new Vector2(W * 0.86f, 2f * sc), COL_FRAME));
    string bk = "BACKUPS " + (backupsOn ? "ON" : "OFF");
    if (mode != "auto") bk += "  [MANUAL]";
    frame.Add(Txt(bk, off + new Vector2(W * 0.5f, H * 0.83f), 1.0f * sc, backupsOn ? COL_WARN : COL_GOOD));
    frame.Add(Txt("H2 x" + hydroCount + "   REACTOR x" + reactorCount + "   BATT x" + battCount,
        off + new Vector2(W * 0.5f, H * 0.92f), 0.75f * sc, COL_DIM));
    frame.Dispose();
}

void DrawPower(IMyTextSurface s)
{
    var frame = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f;

    frame.Add(Txt("POWER PRODUCERS", off + new Vector2(W * 0.5f, H * 0.03f), 1.1f * sc, COL_DIM));
    frame.Add(Txt("OUTPUT  " + FmtPower(prodOut) + "  /  " + FmtPower(prodMax),
        off + new Vector2(W * 0.5f, H * 0.10f), 0.95f * sc, COL_TEXT));
    string bk = "BACKUPS " + (backupsOn ? "ON" : "OFF") + (mode != "auto" ? "  [MANUAL]" : "");
    frame.Add(Txt(bk, off + new Vector2(W * 0.5f, H * 0.17f), 0.85f * sc, backupsOn ? COL_WARN : COL_GOOD));
    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.235f), new Vector2(W * 0.9f, 2f * sc), COL_FRAME));

    aliveProds.Clear();
    foreach (var p in producers) if (!p.Closed) aliveProds.Add(p);
    int total = aliveProds.Count;
    if (total == 0)
    {
        frame.Add(Txt("NO PRODUCERS FOUND", off + new Vector2(W * 0.5f, H * 0.5f), 0.9f * sc, COL_DIM));
        frame.Dispose(); return;
    }

    float y = 0.28f, step = 0.062f;
    int maxRows = Math.Max(1, (int)((0.93f - y) / step));
    int pageCount = (total + maxRows - 1) / maxRows;
    int page = Page(pageCount);
    int start = page * maxRows;
    int maxChars = (int)(W * 0.55f / (12f * sc));

    for (int i = start; i < total && i < start + maxRows; i++)
    {
        var p = aliveProds[i];
        bool en = p.Enabled;
        Color c = !en ? COL_BAD : (p.CurrentOutput > 0.0001 ? COL_GOOD : COL_WARN);
        Vector2 rowPos = off + new Vector2(0, H * (y + (i - start) * step));
        frame.Add(Box(rowPos + new Vector2(W * 0.055f, H * 0.022f), new Vector2(10f * sc, 10f * sc), c));
        frame.Add(Txt(Trunc(p.CustomName, maxChars), rowPos + new Vector2(W * 0.10f, 0), 0.68f * sc, COL_TEXT, TextAlignment.LEFT));
        frame.Add(Txt(en ? FmtPower(p.CurrentOutput) : "OFF", rowPos + new Vector2(W * 0.95f, 0), 0.68f * sc, c, TextAlignment.RIGHT));
    }
    if (pageCount > 1)
        frame.Add(Txt("PAGE " + (page + 1) + "/" + pageCount + "   (" + total + " TOTAL)",
            off + new Vector2(W * 0.5f, H * 0.955f), 0.6f * sc, COL_DIM));
    frame.Dispose();
}

void DrawGas(IMyTextSurface s, string title, Color accent, List<IMyGasTank> tanks, double gStored, double gCap)
{
    var frame = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f;

    double pct = gCap > 0 ? gStored / gCap * 100.0 : 0;
    Color col = pct < 15 ? COL_BAD : accent;

    frame.Add(Txt(title, off + new Vector2(W * 0.5f, H * 0.03f), 1.1f * sc, COL_DIM));

    float gw = W * 0.86f, gh = H * 0.10f, t = 4f * sc;
    Vector2 gc = off + new Vector2(W * 0.5f, H * 0.155f);
    frame.Add(Box(gc, new Vector2(gw + t * 2, gh + t * 2), COL_FRAME));
    frame.Add(Box(gc, new Vector2(gw, gh), COL_BG));
    float fw = (float)(gw * Clamp01(pct / 100.0));
    if (fw > 1) frame.Add(Box(gc + new Vector2(-gw * 0.5f + fw * 0.5f, 0), new Vector2(fw, gh), col));

    frame.Add(Txt(pct.ToString("0") + "%", off + new Vector2(W * 0.5f, H * 0.235f), 1.9f * sc, col));
    frame.Add(Txt(FmtGas(gStored) + "  /  " + FmtGas(gCap), off + new Vector2(W * 0.5f, H * 0.37f), 0.8f * sc, COL_DIM));
    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.44f), new Vector2(W * 0.9f, 2f * sc), COL_FRAME));

    aliveTanks.Clear();
    foreach (var tk in tanks) if (!tk.Closed) aliveTanks.Add(tk);
    int total = aliveTanks.Count;
    if (total == 0)
    {
        frame.Add(Txt("NO TANKS FOUND", off + new Vector2(W * 0.5f, H * 0.6f), 0.9f * sc, COL_DIM));
        frame.Dispose(); return;
    }

    float y = 0.48f, step = 0.068f;
    int maxRows = Math.Max(1, (int)((0.93f - y) / step));
    int pageCount = (total + maxRows - 1) / maxRows;
    int page = Page(pageCount);
    int start = page * maxRows;
    int maxChars = (int)(W * 0.42f / (12f * sc));

    for (int i = start; i < total && i < start + maxRows; i++)
    {
        var tk = aliveTanks[i];
        double tp = tk.FilledRatio * 100.0;
        Color tc = tp < 15 ? COL_BAD : accent;
        Vector2 rowPos = off + new Vector2(0, H * (y + (i - start) * step));
        frame.Add(Txt(Trunc(tk.CustomName, maxChars), rowPos + new Vector2(W * 0.05f, 0), 0.65f * sc, COL_TEXT, TextAlignment.LEFT));
        float bw = W * 0.30f, bh = 12f * sc;
        Vector2 bc = rowPos + new Vector2(W * 0.71f, H * 0.021f);
        frame.Add(Box(bc, new Vector2(bw + 4f * sc, bh + 4f * sc), COL_FRAME));
        frame.Add(Box(bc, new Vector2(bw, bh), COL_BG));
        float tfw = (float)(bw * Clamp01(tp / 100.0));
        if (tfw > 1) frame.Add(Box(bc + new Vector2(-bw * 0.5f + tfw * 0.5f, 0), new Vector2(tfw, bh), tc));
        frame.Add(Txt(tp.ToString("0"), rowPos + new Vector2(W * 0.95f, 0), 0.65f * sc, tc, TextAlignment.RIGHT));
    }
    if (pageCount > 1)
        frame.Add(Txt("PAGE " + (page + 1) + "/" + pageCount + "   (" + total + " TOTAL)",
            off + new Vector2(W * 0.5f, H * 0.955f), 0.6f * sc, COL_DIM));
    frame.Dispose();
}

void DrawGasSplit(IMyTextSurface s)
{
    var frame = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f;

    frame.Add(Txt("GAS STORAGE", off + new Vector2(W * 0.5f, H * 0.03f), 1.1f * sc, COL_DIM));

    int h2Count = 0, o2Count = 0;
    foreach (var t in h2Tanks) if (!t.Closed) h2Count++;
    foreach (var t in o2Tanks) if (!t.Closed) o2Count++;

    GasSection(frame, off, size, 0.12f, "HYDROGEN", COL_H2, h2Stored, h2Cap, h2Count);
    frame.Add(Box(off + new Vector2(W * 0.5f, H * 0.52f), new Vector2(W * 0.9f, 2f * sc), COL_FRAME));
    GasSection(frame, off, size, 0.57f, "OXYGEN", COL_O2, o2Stored, o2Cap, o2Count);

    frame.Dispose();
}

void GasSection(MySpriteDrawFrame frame, Vector2 off, Vector2 size, float yTop,
    string title, Color accent, double gStored, double gCap, int count)
{
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f;
    double pct = gCap > 0 ? gStored / gCap * 100.0 : 0;
    Color col = pct < 15 ? COL_BAD : accent;

    frame.Add(Txt(title, off + new Vector2(W * 0.5f, H * yTop), 0.9f * sc, COL_DIM));

    float gw = W * 0.86f, gh = H * 0.085f, t = 4f * sc;
    Vector2 gc = off + new Vector2(W * 0.5f, H * (yTop + 0.115f));
    frame.Add(Box(gc, new Vector2(gw + t * 2, gh + t * 2), COL_FRAME));
    frame.Add(Box(gc, new Vector2(gw, gh), COL_BG));
    float fw = (float)(gw * Clamp01(pct / 100.0));
    if (fw > 1) frame.Add(Box(gc + new Vector2(-gw * 0.5f + fw * 0.5f, 0), new Vector2(fw, gh), col));

    frame.Add(Txt(pct.ToString("0") + "%", off + new Vector2(W * 0.5f, H * (yTop + 0.175f)), 1.5f * sc, col));
    frame.Add(Txt(FmtGas(gStored) + " / " + FmtGas(gCap) + "   (" + count + " TANKS)",
        off + new Vector2(W * 0.5f, H * (yTop + 0.28f)), 0.7f * sc, COL_DIM));
}

// ================= HELPERS =================

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

double Clamp01(double v) { return v < 0 ? 0 : (v > 1 ? 1 : v); }
string Pct(double a, double b) { return b > 0 ? (a / b * 100).ToString("0") : "0"; }
string Trunc(string t, int max) { return max > 2 && t.Length > max ? t.Substring(0, max - 2) + ".." : t; }

string FmtPower(double mw)
{
    if (Math.Abs(mw) >= 1) return mw.ToString("0.0") + " MW";
    return (mw * 1000).ToString("0") + " kW";
}

string FmtGas(double l)
{
    if (l >= 1000000) return (l / 1000000).ToString("0.0") + "M L";
    if (l >= 1000) return (l / 1000).ToString("0") + "k L";
    return l.ToString("0") + " L";
}

string FmtTime(double h)
{
    if (h > 72) return ">3 DAYS";
    int m = (int)Math.Round(h * 60);
    if (m >= 60) return (m / 60) + "H " + (m % 60) + "M";
    return m + "M";
}
