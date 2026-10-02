//  TACTICAL INDUSTRIAL MONITORING SYSTEM  //  T.I.M.S.
//  CLASSIFIED // FLEET INDUSTRIAL COMMAND  //  RED/AMBER LCARS-MIL
//  Tag LCDs by name OR Custom Data with one of:
//    [TIMS:REFINERY]   [TIMS:ASSEMBLER]   [TIMS:POWER]
//    [TIMS:DEBUG]      [TIMS:CAL]         [TIMS]  (= setup instructions)
//  Optional surface index for cockpit screens:
//    [TIMS:POWER:0]
//  TUNING (if text feels too large/small on your specific LCD):
//    Edit BASE_TEXT_SCALE below. Default 0.55.

// Version: major.minor.build
// - major bumps for paradigm shifts (text -> sprites was the last one)
// - minor bumps for feature additions
// - build increments on every iteration / bug fix
const string TIMS_VERSION = "3.2.23";

const float BASE_TEXT_SCALE = 0.55f;  // body text scale
const float TITLE_TEXT_SCALE = 0.90f; // header title scale
const float SMALL_TEXT_SCALE = 0.45f; // captions/small labels

// USER CONFIG
// Change PALETTE to one of the following, then recompile.
//   red    : red/orange on black, TIMS-MIL military aesthetic (default)
//   violet : violet/blue on white, TIMS-CIV civilian utility
//   grey   : monochrome greyscale, TIMS-CB color blind
//   cyan   : green/cyan on black, TIMS-NAV navigation/bridge
const string PALETTE = "cyan";

// Palette values resolved from PALETTE at script init.
static Color C_BG       = PaletteBG();
static Color C_FRAME    = PaletteFrame();
static Color C_PRIMARY  = PalettePrimary();
static Color C_ACCENT   = PaletteAccent();
static Color C_HOT      = PaletteHot();
static Color C_DIM      = PaletteDim();
static Color C_LABEL    = PaletteLabel();
static Color C_TEXT     = PaletteText();

static Color PaletteBG()
{
    if (PALETTE == "violet") return new Color(245, 245, 250);
    if (PALETTE == "grey")   return new Color(15, 15, 15);
    if (PALETTE == "cyan")   return new Color(2, 8, 10);
    return new Color(8, 4, 2);
}
static Color PaletteFrame()
{
    if (PALETTE == "violet") return new Color(80, 30, 180);
    if (PALETTE == "grey")   return new Color(180, 180, 180);
    if (PALETTE == "cyan")   return new Color(30, 200, 180);
    return new Color(255, 120, 30);
}
static Color PalettePrimary()
{
    if (PALETTE == "violet") return new Color(60, 80, 220);
    if (PALETTE == "grey")   return new Color(220, 220, 220);
    if (PALETTE == "cyan")   return new Color(80, 220, 160);
    return new Color(255, 160, 60);
}
static Color PaletteAccent()
{
    if (PALETTE == "violet") return new Color(120, 50, 200);
    if (PALETTE == "grey")   return new Color(150, 150, 150);
    if (PALETTE == "cyan")   return new Color(30, 180, 220);
    return new Color(255, 80, 30);
}
static Color PaletteHot()
{
    if (PALETTE == "violet") return new Color(200, 30, 80);
    if (PALETTE == "grey")   return new Color(255, 255, 255);
    if (PALETTE == "cyan")   return new Color(255, 200, 50);
    return new Color(255, 50, 50);
}
static Color PaletteDim()
{
    if (PALETTE == "violet") return new Color(200, 200, 220);
    if (PALETTE == "grey")   return new Color(60, 60, 60);
    if (PALETTE == "cyan")   return new Color(20, 70, 70);
    return new Color(110, 45, 18);
}
static Color PaletteLabel()
{
    if (PALETTE == "violet") return new Color(60, 30, 140);
    if (PALETTE == "grey")   return new Color(200, 200, 200);
    if (PALETTE == "cyan")   return new Color(140, 240, 200);
    return new Color(255, 200, 120);
}
static Color PaletteText()
{
    if (PALETTE == "violet") return new Color(20, 10, 60);
    if (PALETTE == "grey")   return new Color(240, 240, 240);
    if (PALETTE == "cyan")   return new Color(180, 240, 220);
    return new Color(255, 220, 160);
}

int _tick = 0;
int _scanCounter = 0;

// Per-tick DetailedInfo parse cache. Avoids re-parsing the same block's
// DetailedInfo many times when one screen reads current input, max input,
// speed mod, yield mod, etc all in the same Main() call. Key = block EntityId.
// Cleared at the start of each Main().
Dictionary<long, BlockInfoCache> _infoCache = new Dictionary<long, BlockInfoCache>();

struct BlockInfoCache
{
    public float CurrentInput, MaxInput;
    public float SpeedFactor, YieldFactor, PowerEffFactor;
    public int ModUsed, ModTotal;
    public bool Loaded;
}

// Debug capture: rolling log of recent errors and stats for the debug screen
Queue<string> _errorLog = new Queue<string>();
const int MAX_ERROR_LOG = 6;
double _lastMainMs = 0;
double _peakMainMs = 0;

List<IMyRefinery> _refineries = new List<IMyRefinery>();
List<IMyAssembler> _assemblers = new List<IMyAssembler>();
List<IMyPowerProducer> _powerProducers = new List<IMyPowerProducer>();
List<IMyBatteryBlock> _batteries = new List<IMyBatteryBlock>();
List<IMyTerminalBlock> _allBlocks = new List<IMyTerminalBlock>();
List<DisplayTarget> _targets = new List<DisplayTarget>();

// DVD logo bounce state for PB's own screen
Vector2 _logoPos    = new Vector2(80, 60);
Vector2 _logoVel    = new Vector2(2.5f, 1.8f);
int     _logoColorIdx = 0;
static readonly Color[] _logoColors = new Color[]
{
    new Color(255, 80, 30),    // accent red-orange
    new Color(255, 160, 60),   // primary orange
    new Color(255, 220, 100),  // bright amber
    new Color(255, 50, 50),    // hot red
    new Color(255, 200, 120),  // pale amber
    new Color(220, 100, 40),   // burnt orange
};

public Program()
{
    // Update10 drives the bouncing logo animation; Update100 paces the heavy
    // data work. The logo updates every Update10 tick; everything else only
    // refreshes on Update100 ticks.
    Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;
    Echo("T.I.M.S. ONLINE v" + TIMS_VERSION);
    RebuildCaches();
}

public void Main(string argument, UpdateType updateSource)
{
    // Capture timing from the PREVIOUS run (this tick's value will be set by
    // SE after we return). LastRunTimeMs is the official, sub-millisecond
    // accurate measurement; DateTime resolution was too coarse.
    _lastMainMs = Runtime.LastRunTimeMs;
    if (_lastMainMs > _peakMainMs) _peakMainMs = _lastMainMs;

    _infoCache.Clear(); // fresh DetailedInfo parses each tick
    bool heavy = (updateSource & UpdateType.Update100) != 0
              || (updateSource & (UpdateType.Trigger | UpdateType.Terminal | UpdateType.Script)) != 0;
    bool light = (updateSource & UpdateType.Update10) != 0;

    if (heavy)
    {
        _tick++;
        _scanCounter++;
        if (_scanCounter >= 18) { RebuildCaches(); _scanCounter = 0; }

        foreach (var t in _targets)
        {
            try
            {
                var surface = GetSurface(t);
                if (surface == null) continue;
                surface.ContentType = ContentType.SCRIPT;
                surface.Script = "";

                if (t.Tag == TagType.Setup)
                {
                    DrawSetupInstructions(surface, t.Block);
                    continue;
                }
                if (t.Tag == TagType.Debug)
                {
                    DrawDebugScreen(surface, t.Block);
                    continue;
                }
                if (t.Tag == TagType.Cal)
                {
                    DrawCalibrationScreen(surface);
                    continue;
                }

                var v = GetViewport(surface, t.Block);
                if (IsTooSmall(v))
                {
                    string typeName = t.Tag == TagType.Refinery ? "REFINERY OPS"
                                    : t.Tag == TagType.Assembler ? "ASSEMBLER OPS"
                                    : "POWER COMMAND";
                    DrawIncompatibleScreen(surface, t.Block, typeName);
                    continue;
                }

                switch (t.Tag)
                {
                    case TagType.Refinery:  DrawRefineryScreen(surface, t.Block);  break;
                    case TagType.Assembler: DrawAssemblerScreen(surface, t.Block); break;
                    case TagType.Power:     DrawPowerScreen(surface, t.Block);     break;
                }
            }
            catch (Exception ex)
            {
                LogError(t.Block.CustomName + " " + t.Tag + ": " + ex.Message);
                Echo("ERR: " + ex.Message);
            }
        }

        Echo("TIMS v" + TIMS_VERSION + " tick " + _tick + " | tgt:" + _targets.Count
            + " | r:" + _refineries.Count + " | a:" + _assemblers.Count
            + " | p:" + _powerProducers.Count + " | b:" + _batteries.Count);
        Echo("see PB Custom Data for full debug dump");

        // Full debug dump written to the PB's Custom Data so the user can
        // open the PB, copy the text directly, and share it. Lives entirely
        // in Custom Data so it doesn't clutter the small Detail Info area.
        var dbg = new System.Text.StringBuilder();
        dbg.Append("T.I.M.S. v").Append(TIMS_VERSION)
           .Append(" debug dump (tick ").Append(_tick)
           .Append(", T+").Append(FmtUptime(_tick)).Append(")\n");
        dbg.Append("==========================================================\n");
        dbg.Append("RUNTIME\n");
        dbg.Append("  last main : ").Append(_lastMainMs.ToString("0.000")).Append(" ms\n");
        dbg.Append("  peak main : ").Append(_peakMainMs.ToString("0.000")).Append(" ms\n");
        dbg.Append("  update    : ").Append(Runtime.UpdateFrequency.ToString()).Append("\n");
        dbg.Append("BLOCKS\n");
        dbg.Append("  refineries: ").Append(_refineries.Count).Append("\n");
        dbg.Append("  assemblers: ").Append(_assemblers.Count).Append("\n");
        dbg.Append("  producers : ").Append(_powerProducers.Count).Append("\n");
        dbg.Append("  batteries : ").Append(_batteries.Count).Append("\n");
        dbg.Append("  all blocks: ").Append(_allBlocks.Count).Append("\n");
        dbg.Append("TARGETS (").Append(_targets.Count).Append(")\n");
        foreach (var t in _targets)
        {
            var surf = GetSurface(t);
            if (surf == null)
            {
                dbg.Append("  ").Append(t.Block.CustomName).Append(" : null surface\n");
                continue;
            }
            var vp = GetViewport(surf, t.Block);
            string note = IsTooSmall(vp) ? " INCOMPAT" : (IsWideViewport(vp) ? " WIDE" : "");
            dbg.Append("  ").Append(t.Block.CustomName)
               .Append(" [").Append(t.Tag.ToString().ToUpper()).Append(":").Append(t.SurfaceIndex).Append("]")
               .Append(" sub=").Append(t.Block.BlockDefinition.SubtypeId).Append("\n")
               .Append("    surf=").Append(surf.SurfaceSize.X.ToString("0")).Append("x").Append(surf.SurfaceSize.Y.ToString("0"))
               .Append(" tex=").Append(surf.TextureSize.X.ToString("0")).Append("x").Append(surf.TextureSize.Y.ToString("0"))
               .Append(" origin=").Append(vp.Origin.X.ToString("0")).Append(",").Append(vp.Origin.Y.ToString("0"))
               .Append(note).Append("\n");
        }
        if (_refineries.Count > 0)
        {
            var r = _refineries[0];
            dbg.Append("SAMPLE REFINERY: ").Append(r.CustomName).Append("\n");
            dbg.Append("  Required Input    : ").Append(GetCurrentInputMW(r).ToString("0.000")).Append(" MW\n");
            dbg.Append("  Max Required Input: ").Append(GetMaxInputMW(r).ToString("0.000")).Append(" MW\n");
            dbg.Append("  Refine Speed      : x").Append(GetSpeedFactor(r).ToString("0.00")).Append("\n");
            dbg.Append("  Yield Rate        : x").Append(GetYieldFactor(r).ToString("0.00")).Append("\n");
            dbg.Append("  Power Efficiency  : x").Append(GetPowerEffFactor(r).ToString("0.00")).Append("\n");
            int mu, mt; GetModuleSlots(r, out mu, out mt);
            dbg.Append("  modules           : ").Append(mu).Append("/").Append(mt).Append("\n");
            dbg.Append("  status            : ").Append(GetProdStatus(r).ToString().ToUpper()).Append("\n");
            dbg.Append("  IsProducing       : ").Append(r.IsProducing).Append("\n");
            dbg.Append("  IsQueueEmpty      : ").Append(r.IsQueueEmpty).Append("\n");
            dbg.Append("  IsWorking         : ").Append(r.IsWorking).Append("\n");
            dbg.Append("  IsFunctional      : ").Append(r.IsFunctional).Append("\n");
            dbg.Append("  Enabled           : ").Append(r.Enabled).Append("\n");
        }
        if (_assemblers.Count > 0)
        {
            var a = _assemblers[0];
            dbg.Append("SAMPLE ASSEMBLER: ").Append(a.CustomName).Append("\n");
            dbg.Append("  Required Input    : ").Append(GetCurrentInputMW(a).ToString("0.000")).Append(" MW\n");
            dbg.Append("  Productivity      : x").Append(GetSpeedFactor(a).ToString("0.00")).Append("\n");
            dbg.Append("  Effectiveness     : x").Append(GetYieldFactor(a).ToString("0.00")).Append("\n");
            dbg.Append("  status            : ").Append(GetProdStatus(a).ToString().ToUpper()).Append("\n");
            dbg.Append("  mode              : ").Append(a.Mode).Append("\n");
            dbg.Append("  cooperative       : ").Append(a.CooperativeMode).Append("\n");
        }
        if (_errorLog.Count > 0)
        {
            dbg.Append("ERRORS\n");
            foreach (var e in _errorLog) dbg.Append("  ").Append(e).Append("\n");
        }
        Me.CustomData = dbg.ToString();
    }

    if (heavy || light)
    {
        try { DrawServerLogoScreen(); } catch (Exception ex) { LogError("LOGO: " + ex.Message); Echo("LOGO ERR: " + ex.Message); }
    }
}

void LogError(string msg)
{
    string stamped = "T+" + _tick + " " + msg;
    _errorLog.Enqueue(stamped);
    while (_errorLog.Count > MAX_ERROR_LOG) _errorLog.Dequeue();
}

void DrawServerLogoScreen()
{
    if (Me.SurfaceCount == 0) return;
    var s = Me.GetSurface(0);
    if (s == null) return;
    s.ContentType = ContentType.SCRIPT;
    s.Script = "";

    using (var frame = s.DrawFrame())
    {
        var v = GetViewport(s, Me);

        // Background
        AddBox(frame, v.Origin, v.Size, C_BG);

        // Subtle frame border
        float bw = 2;
        AddBox(frame, v.Origin, new Vector2(v.W, bw), C_DIM);
        AddBox(frame, v.Origin + new Vector2(0, v.H - bw), new Vector2(v.W, bw), C_DIM);
        AddBox(frame, v.Origin, new Vector2(bw, v.H), C_DIM);
        AddBox(frame, v.Origin + new Vector2(v.W - bw, 0), new Vector2(bw, v.H), C_DIM);

        // Logo dimensions - "T.I.M.S." text at title scale
        // Approximate width: 8 chars * 0.6 * 28px (1.0 scale base) for monospace
        float logoScale = Math.Max(0.6f, v.H / 200f);
        string logoText = "T.I.M.S.";
        // Rough size estimate so we can clamp the position to viewport
        float logoW = logoText.Length * 14f * logoScale; // monospace char ~14px at 1.0
        float logoH = 28f * logoScale;

        // Update position
        _logoPos += _logoVel;

        // Bounce off walls and change color on hit
        bool hit = false;
        if (_logoPos.X < 4)                { _logoPos.X = 4;                 _logoVel.X = -_logoVel.X; hit = true; }
        if (_logoPos.Y < 4)                { _logoPos.Y = 4;                 _logoVel.Y = -_logoVel.Y; hit = true; }
        if (_logoPos.X + logoW > v.W - 4)  { _logoPos.X = v.W - logoW - 4;   _logoVel.X = -_logoVel.X; hit = true; }
        if (_logoPos.Y + logoH > v.H - 4)  { _logoPos.Y = v.H - logoH - 4;   _logoVel.Y = -_logoVel.Y; hit = true; }
        if (hit) _logoColorIdx = (_logoColorIdx + 1) % _logoColors.Length;

        Color c = _logoColors[_logoColorIdx];

        // Draw subtle ghost trail (faded)
        Color ghost = new Color((byte)(c.R / 4), (byte)(c.G / 4), (byte)(c.B / 4), (byte)255);
        AddText(frame, logoText,
                v.Origin + _logoPos - _logoVel * 2,
                logoScale, ghost, TextAlignment.LEFT);

        // Main logo
        AddText(frame, logoText,
                v.Origin + _logoPos,
                logoScale, c, TextAlignment.LEFT);

        // Status footer
        string footer = "tgt:" + _targets.Count
            + " r:" + _refineries.Count + " a:" + _assemblers.Count
            + " p:" + _powerProducers.Count + " b:" + _batteries.Count;
        AddText(frame, footer,
                v.Origin + new Vector2(v.W * 0.5f, v.H - 18),
                0.4f, C_LABEL, TextAlignment.CENTER);
    }
}

//  CACHING

void RebuildCaches()
{
    _refineries.Clear(); _assemblers.Clear(); _powerProducers.Clear();
    _batteries.Clear(); _allBlocks.Clear(); _targets.Clear();

    GridTerminalSystem.GetBlocksOfType(_refineries, b => b.IsSameConstructAs(Me)
        && !((b.BlockDefinition.SubtypeId ?? "").ToUpperInvariant().Contains("SURVIVALKIT")));
    GridTerminalSystem.GetBlocksOfType(_assemblers, b => b.IsSameConstructAs(Me)
        && !((b.BlockDefinition.SubtypeId ?? "").ToUpperInvariant().Contains("SURVIVALKIT")));
    GridTerminalSystem.GetBlocksOfType(_powerProducers, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(_batteries, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(_allBlocks, b => b.IsSameConstructAs(Me));

    var providers = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(providers, b => b is IMyTextSurfaceProvider && b.IsSameConstructAs(Me));

    foreach (var b in providers)
    {
        string nm = b.CustomName ?? "";
        string cd = b.CustomData ?? "";
        if (!string.IsNullOrEmpty(nm)) AddTargets(b, nm);
        if (!string.IsNullOrEmpty(cd)) AddTargets(b, cd);
    }
}

void AddTargets(IMyTerminalBlock b, string src)
{
    int idx = 0;
    while (idx < src.Length)
    {
        int open = src.IndexOf('[', idx);
        if (open < 0) break;
        int close = src.IndexOf(']', open);
        if (close < 0) break;
        string token = src.Substring(open + 1, close - open - 1).Trim();
        idx = close + 1;
        // Accept either "TIMS" alone (= setup screen) or "TIMS:<TYPE>" forms
        if (!token.StartsWith("TIMS", StringComparison.OrdinalIgnoreCase)) continue;
        if (token.Length > 4 && token[4] != ':') continue; // e.g. "TIMSX..." is not us

        TagType tag;
        int si = 0;
        if (token.Length == 4)
        {
            // Just "[TIMS]" - show setup instructions
            tag = TagType.Setup;
        }
        else
        {
            var parts = token.Split(':');
            if (parts.Length < 2) continue;
            switch (parts[1].ToUpperInvariant())
            {
                case "REFINERY":  tag = TagType.Refinery;  break;
                case "ASSEMBLER": tag = TagType.Assembler; break;
                case "POWER":     tag = TagType.Power;     break;
                case "DEBUG":     tag = TagType.Debug;     break;
                case "CAL":       tag = TagType.Cal;       break;
                case "":          tag = TagType.Setup;     break;
                default: continue;
            }
            if (parts.Length >= 3) int.TryParse(parts[2], out si);
        }
        bool dup = false;
        foreach (var t in _targets)
            if (t.Block == b && t.SurfaceIndex == si && t.Tag == tag) { dup = true; break; }
        if (!dup) _targets.Add(new DisplayTarget { Block = b, SurfaceIndex = si, Tag = tag });
    }
}

IMyTextSurface GetSurface(DisplayTarget t)
{
    var prov = t.Block as IMyTextSurfaceProvider;
    if (prov == null) return null;
    if (t.SurfaceIndex < 0 || t.SurfaceIndex >= prov.SurfaceCount) return null;
    return prov.GetSurface(t.SurfaceIndex);
}

//  DRAW PRIMITIVES

struct Viewport
{
    public Vector2 Origin;
    public Vector2 Size;
    public float W { get { return Size.X; } }
    public float H { get { return Size.Y; } }
}

// Existing single-arg version. Tries to identify the block from the surface
// directly (only works when surface IS the block, i.e. an IMyTextPanel).
// For cockpit and PB surfaces, callers should use the overload that takes
// the block explicitly.
Viewport GetViewport(IMyTextSurface s)
{
    var asBlock = s as IMyTerminalBlock;
    return GetViewport(s, asBlock);
}

Viewport GetViewport(IMyTextSurface s, IMyTerminalBlock block)
{
    var size = s.SurfaceSize;
    var texSize = s.TextureSize;

    // Default: center the surface within the texture (SE's documented behavior)
    bool sane = texSize.X <= size.X * 2.0f && texSize.Y <= size.Y * 2.0f
             && texSize.X >= size.X && texSize.Y >= size.Y;
    Vector2 origin;
    if (sane) origin = (texSize - size) * 0.5f;
    else      origin = new Vector2(0, 0);

    // Per-LCD-type overrides. None active. SE's reported SurfaceSize and
    // centered origin are correct. Earlier theories about hidden bezels were
    // wrong; the symptom was actually a bug in DrawHeader's return value
    // (relative vs absolute Y), now fixed.
    // If a specific block type ever needs an override, add it here:
    //   if (sub == "<SUBTYPE>") return new Viewport { Origin = ..., Size = ... };

    return new Viewport { Origin = origin, Size = size };
}

void AddBox(MySpriteDrawFrame frame, Vector2 pos, Vector2 size, Color color)
{
    if (size.X <= 0 || size.Y <= 0) return;
    frame.Add(new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        Position = pos + size * 0.5f,
        Size = size,
        Color = color,
        Alignment = TextAlignment.CENTER
    });
}

// LCARS rounded "cap" - half-circle on one end of a strip. Drawn as a circle
// that overlaps with the strip rectangle.
void AddCap(MySpriteDrawFrame frame, Vector2 pos, float diameter, Color color)
{
    frame.Add(new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "Circle",
        Position = pos + new Vector2(diameter * 0.5f, diameter * 0.5f),
        Size = new Vector2(diameter, diameter),
        Color = color,
        Alignment = TextAlignment.CENTER
    });
}

// Horizontal LCARS pill (rounded on both ends). Only draws caps if the
// pill is wider than it is tall (otherwise circles overlap rectangle).
void AddPill(MySpriteDrawFrame frame, Vector2 pos, Vector2 size, Color color)
{
    if (size.X <= size.Y)
    {
        // Just draw a rectangle if it's too narrow for caps
        AddBox(frame, pos, size, color);
        return;
    }
    float h = size.Y;
    // Middle rectangle (between caps)
    AddBox(frame, pos + new Vector2(h * 0.5f, 0), new Vector2(size.X - h, size.Y), color);
    // End caps
    AddCap(frame, pos, h, color);
    AddCap(frame, pos + new Vector2(size.X - h, 0), h, color);
}

void AddText(MySpriteDrawFrame frame, string text, Vector2 pos, float scale, Color color,
             TextAlignment align = TextAlignment.LEFT)
{
    if (string.IsNullOrEmpty(text)) return;
    frame.Add(new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = text,
        Position = pos,
        RotationOrScale = scale,
        Color = color,
        FontId = "Monospace",
        Alignment = align
    });
}

// Horizontal bar gauge with optional segment ticks
void AddBar(MySpriteDrawFrame frame, Vector2 pos, Vector2 size, float frac,
            Color fill, Color back, bool segments = true)
{
    if (frac < 0) frac = 0; if (frac > 1) frac = 1;
    AddBox(frame, pos, size, back);
    if (frac > 0)
        AddBox(frame, pos, new Vector2(size.X * frac, size.Y), fill);
    if (segments && size.X > 40)
    {
        int n = 10;
        for (int i = 1; i < n; i++)
        {
            float x = size.X * i / (float)n;
            AddBox(frame, pos + new Vector2(x - 1, 0), new Vector2(1, size.Y), C_BG);
        }
    }
}

void AddDot(MySpriteDrawFrame frame, Vector2 center, float radius, Color color)
{
    frame.Add(new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "Circle",
        Position = center,
        Size = new Vector2(radius * 2, radius * 2),
        Color = color,
        Alignment = TextAlignment.CENTER
    });
}

//  HEADER (shared LCARS style, scaled for any LCD aspect)
//  Layout: background fill + top horizontal pill bar with title baked in.
//  Returns Y coordinate where content can start.

float DrawHeader(MySpriteDrawFrame frame, Viewport v, string title)
{
    // Background fill (full viewport)
    AddBox(frame, v.Origin, v.Size, C_BG);

    // Header height: ~9% of viewport but clamped
    float headerH = Math.Max(28, Math.Min(60, v.H * 0.09f));
    float padX = 8f;

    // Left bracket pill (small accent)
    float leftPillW = headerH * 2.5f;
    AddPill(frame, v.Origin + new Vector2(padX, 4), new Vector2(leftPillW, headerH - 8), C_ACCENT);

    // Middle long pill (title bar)
    float midPillX = padX + leftPillW + 6;
    float midPillW = v.W - midPillX - padX - leftPillW * 0.6f - 6;
    AddPill(frame, v.Origin + new Vector2(midPillX, 4),
            new Vector2(midPillW, headerH - 8), C_FRAME);

    // Right pill
    float rightPillW = leftPillW * 0.6f;
    AddPill(frame, v.Origin + new Vector2(v.W - rightPillW - padX, 4),
            new Vector2(rightPillW, headerH - 8), C_PRIMARY);

    // Title text centered on middle pill
    AddText(frame, title.ToUpper(),
            v.Origin + new Vector2(midPillX + midPillW * 0.5f, 4 + (headerH - 8) * 0.5f - 12),
            TITLE_TEXT_SCALE, C_BG, TextAlignment.CENTER);

    // Sub-line below header: cycle and uptime
    float subY = headerH + 2;
    string sub = "T+" + FmtUptime(_tick) + "   CYC " + _tick.ToString("D6") + "   CLASSIFIED // FLEET CMD";
    AddText(frame, sub,
            v.Origin + new Vector2(v.W * 0.5f, subY),
            SMALL_TEXT_SCALE, C_LABEL, TextAlignment.CENTER);

    // Thin divider line under sub
    AddBox(frame, v.Origin + new Vector2(padX, subY + 20),
           new Vector2(v.W - padX * 2, 1), C_DIM);

    // Return absolute Y where content can start drawing
    return v.Origin.Y + subY + 24;
}

//  STRIP RENDERER  (rounded-cap row with colored side tab + label + content)

void DrawStripFrame(MySpriteDrawFrame frame, float x, float y, float w, float h, Color tabColor)
{
    // Background
    AddBox(frame, new Vector2(x, y), new Vector2(w, h), C_BG);
    // Left tab (LCARS side accent)
    float tabW = 6;
    AddBox(frame, new Vector2(x, y), new Vector2(tabW, h), tabColor);
    // Top divider hairline
    AddBox(frame, new Vector2(x, y), new Vector2(w, 1), C_DIM);
}

//  SIDE PANEL // wide-LCD secondary stats column

// Detect whether a viewport is "wide" enough to warrant 2-column layout.
// Needs both width AND aspect ratio: tall LCDs with 512x512 don't qualify,
// only LCDs that are actually rectangular wide. The threshold 1.6 covers
// horizontal wall LCDs (e.g. 1280x512) without catching sloped panels
// (512x362, aspect 1.41).
bool IsWideViewport(Viewport v) { return (v.W / Math.Max(1f, v.H)) >= 1.6f && v.W >= 700f; }

// Detect viewports that are too small / wrong aspect to render the TIMS UI.
// Below these thresholds we display a polite incompatibility notice instead
// of garbled content. Corner LCDs, narrow billboards, etc. fall into this.
// Detect viewports that genuinely can't render the TIMS UI. Only the extreme
// cases - corner LCDs that are 73px tall, sliver displays, etc. Portrait
// monitors that report taller-than-wide get a vertical layout instead of
// being rejected outright.
bool IsTooSmall(Viewport v)
{
    // Corner LCDs (73px or 128px) fall here.
    if (v.H < 140) return true;
    if (v.W < 180) return true;
    float aspect = v.W / Math.Max(1f, v.H);
    if (aspect < 0.3f || aspect > 8.0f) return true;
    // Portrait surfaces are billboard variants.
    if (aspect < 0.85f) return true;
    return false;
}

// True for surfaces taller than wide. (Currently unused since IsTooSmall
// rejects portrait surfaces outright, but kept available for future use.)
bool IsPortrait(Viewport v) { return v.H > v.W * 1.10f; }

// Render a polite incompatibility notice when an LCD is too small or wrong
// shape for the TIMS UI to fit. Centered single panel with red accent.
void DrawIncompatibleScreen(IMyTextSurface s, IMyTerminalBlock block, string screenType)
{
    using (var frame = s.DrawFrame())
    {
        var v = GetViewport(s, block);
        AddBox(frame, v.Origin, v.Size, C_BG);
        // Red border to draw attention
        float bw = 3;
        AddBox(frame, v.Origin, new Vector2(v.W, bw), C_HOT);
        AddBox(frame, v.Origin + new Vector2(0, v.H - bw), new Vector2(v.W, bw), C_HOT);
        AddBox(frame, v.Origin, new Vector2(bw, v.H), C_HOT);
        AddBox(frame, v.Origin + new Vector2(v.W - bw, 0), new Vector2(bw, v.H), C_HOT);

        float cx = v.Origin.X + v.W * 0.5f;
        float cy = v.Origin.Y + v.H * 0.5f;

        // Simple stacked text: title, reason, screen type
        AddText(frame, "T.I.M.S. INCOMPATIBLE",
                new Vector2(cx, cy - v.H * 0.18f),
                Math.Min(0.9f, v.H / 200f), C_HOT, TextAlignment.CENTER);
        AddText(frame, "This LCD is too small or",
                new Vector2(cx, cy - v.H * 0.04f),
                Math.Min(0.55f, v.H / 320f), C_LABEL, TextAlignment.CENTER);
        AddText(frame, "wrong shape for " + screenType,
                new Vector2(cx, cy + v.H * 0.06f),
                Math.Min(0.55f, v.H / 320f), C_LABEL, TextAlignment.CENTER);
        AddText(frame, "Use a wall LCD or wide panel",
                new Vector2(cx, cy + v.H * 0.20f),
                Math.Min(0.45f, v.H / 380f), C_TEXT, TextAlignment.CENTER);
    }
}

// Render setup instructions when a block is tagged [TIMS] with no screen type.
// Shows the three valid tag forms.
void DrawSetupInstructions(IMyTextSurface s, IMyTerminalBlock block)
{
    using (var frame = s.DrawFrame())
    {
        var v = GetViewport(s, block);

        // If the LCD is too small for instructions either, fall back to minimal text
        if (IsTooSmall(v))
        {
            AddBox(frame, v.Origin, v.Size, C_BG);
            AddText(frame, "TAG WITH",
                    v.Origin + new Vector2(v.W * 0.5f, v.H * 0.20f),
                    Math.Min(0.6f, v.H / 200f), C_LABEL, TextAlignment.CENTER);
            AddText(frame, "[TIMS:REFINERY]",
                    v.Origin + new Vector2(v.W * 0.5f, v.H * 0.45f),
                    Math.Min(0.5f, v.H / 240f), C_PRIMARY, TextAlignment.CENTER);
            AddText(frame, "[TIMS:ASSEMBLER]",
                    v.Origin + new Vector2(v.W * 0.5f, v.H * 0.65f),
                    Math.Min(0.5f, v.H / 240f), C_PRIMARY, TextAlignment.CENTER);
            AddText(frame, "[TIMS:POWER]",
                    v.Origin + new Vector2(v.W * 0.5f, v.H * 0.85f),
                    Math.Min(0.5f, v.H / 240f), C_PRIMARY, TextAlignment.CENTER);
            return;
        }

        float contentY = DrawHeader(frame, v, "T.I.M.S. SETUP");
        float x = v.Origin.X + 16;
        float w = v.W - 32;

        AddText(frame, "TAG THIS LCD WITH ONE OF THE FOLLOWING",
                new Vector2(x, contentY + 4),
                BASE_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);
        AddText(frame, "in the block name OR Custom Data:",
                new Vector2(x, contentY + 24),
                SMALL_TEXT_SCALE, C_TEXT, TextAlignment.LEFT);

        float y = contentY + 56;
        float lineH = v.H * 0.085f;

        DrawSetupLine(frame, x, y + 0 * lineH, w, lineH, "[TIMS:REFINERY]",
                      "Refinery operations + ore queue + module mods",
                      C_PRIMARY);
        DrawSetupLine(frame, x, y + 1 * lineH, w, lineH, "[TIMS:ASSEMBLER]",
                      "Assembler operations + production queue + mods",
                      C_ACCENT);
        DrawSetupLine(frame, x, y + 2 * lineH, w, lineH, "[TIMS:POWER]",
                      "Power generation, batteries, consumption doctrine",
                      C_FRAME);
        DrawSetupLine(frame, x, y + 3 * lineH, w, lineH, "[TIMS:DEBUG]",
                      "Diagnostics: block counts, LCD geometry, errors",
                      C_HOT);

        y += 4 * lineH + 10;
        AddText(frame, "MULTI-SURFACE BLOCKS (cockpits, etc):",
                new Vector2(x, y), SMALL_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);
        AddText(frame, "  append surface index: [TIMS:POWER:1]",
                new Vector2(x, y + 16), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.LEFT);

        y += 44;
        AddText(frame, "FLEET INDUSTRIAL COMMAND // CLASSIFIED",
                new Vector2(v.Origin.X + v.W * 0.5f, v.H + v.Origin.Y - 24),
                SMALL_TEXT_SCALE, C_DIM, TextAlignment.CENTER);
    }
}

void DrawSetupLine(MySpriteDrawFrame frame, float x, float y, float w, float h,
                   string tag, string desc, Color color)
{
    AddBox(frame, new Vector2(x, y + h * 0.10f), new Vector2(4, h * 0.80f), color);
    AddText(frame, tag, new Vector2(x + 12, y + 4),
            BASE_TEXT_SCALE, color, TextAlignment.LEFT);
    AddText(frame, desc, new Vector2(x + 12, y + h * 0.50f),
            SMALL_TEXT_SCALE, C_TEXT, TextAlignment.LEFT);
}

//  CALIBRATION SCREEN  // [TIMS:CAL]
//  Bypasses all viewport math and draws a grid across the full texture with
//  pixel coordinate labels. This lets you SEE exactly where the visible
//  window of an LCD is within its texture, so we can compute correct offsets
//  for misbehaving block models.
//  Usage: tag a problem LCD with [TIMS:CAL]. The visible area will show
//  some portion of a grid with coordinate labels. Report the visible labels
//  back and we'll know the visible region's (x,y) extents in texture space.

void DrawCalibrationScreen(IMyTextSurface s)
{
    using (var frame = s.DrawFrame())
    {
        var texSize = s.TextureSize;
        var surfSize = s.SurfaceSize;

        // Draw across the ENTIRE texture, ignoring SurfaceSize and Origin
        Vector2 origin = new Vector2(0, 0);
        Vector2 size = texSize;

        // Solid black background filling entire texture
        AddBox(frame, origin, size, C_BG);

        // Bright border around the WHOLE texture
        float bw = 4;
        AddBox(frame, origin, new Vector2(size.X, bw), Color.Lime);
        AddBox(frame, origin + new Vector2(0, size.Y - bw), new Vector2(size.X, bw), Color.Lime);
        AddBox(frame, origin, new Vector2(bw, size.Y), Color.Lime);
        AddBox(frame, origin + new Vector2(size.X - bw, 0), new Vector2(bw, size.Y), Color.Lime);

        // Diagonal corner markers so visible corners are obvious
        AddText(frame, "TL", new Vector2(10, 10), 0.6f, Color.Lime, TextAlignment.LEFT);
        AddText(frame, "TR", new Vector2(size.X - 10, 10), 0.6f, Color.Lime, TextAlignment.RIGHT);
        AddText(frame, "BL", new Vector2(10, size.Y - 24), 0.6f, Color.Lime, TextAlignment.LEFT);
        AddText(frame, "BR", new Vector2(size.X - 10, size.Y - 24), 0.6f, Color.Lime, TextAlignment.RIGHT);

        // Grid lines every 64 pixels with coordinate labels
        float step = 64;
        for (float x = step; x < size.X; x += step)
        {
            AddBox(frame, new Vector2(x, 0), new Vector2(1, size.Y), new Color(60, 60, 60));
        }
        for (float y = step; y < size.Y; y += step)
        {
            AddBox(frame, new Vector2(0, y), new Vector2(size.X, 1), new Color(60, 60, 60));
        }

        // Coordinate labels at each grid intersection
        for (float x = 0; x < size.X; x += step)
        {
            for (float y = 0; y < size.Y; y += step)
            {
                AddText(frame, x.ToString("0") + "," + y.ToString("0"),
                        new Vector2(x + 4, y + 2), 0.4f, Color.Yellow, TextAlignment.LEFT);
            }
        }

        // Big text in the center showing reported dimensions
        float cx = size.X * 0.5f;
        float cy = size.Y * 0.5f;
        AddText(frame, "CALIBRATION", new Vector2(cx, cy - 60), 1.0f, Color.Cyan, TextAlignment.CENTER);
        AddText(frame, "tex " + texSize.X.ToString("0") + "x" + texSize.Y.ToString("0"),
                new Vector2(cx, cy - 20), 0.7f, Color.Cyan, TextAlignment.CENTER);
        AddText(frame, "surf " + surfSize.X.ToString("0") + "x" + surfSize.Y.ToString("0"),
                new Vector2(cx, cy + 10), 0.7f, Color.Cyan, TextAlignment.CENTER);

        // What SE thinks the visible window is - draw a red dashed rectangle
        var v = (texSize - surfSize) * 0.5f;
        if (v.X < 0) v.X = 0;
        if (v.Y < 0) v.Y = 0;
        // Top border of expected visible area
        AddBox(frame, v, new Vector2(surfSize.X, 2), Color.Red);
        // Bottom border
        AddBox(frame, v + new Vector2(0, surfSize.Y - 2), new Vector2(surfSize.X, 2), Color.Red);
        // Left border
        AddBox(frame, v, new Vector2(2, surfSize.Y), Color.Red);
        // Right border
        AddBox(frame, v + new Vector2(surfSize.X - 2, 0), new Vector2(2, surfSize.Y), Color.Red);
        AddText(frame, "[SE says visible]",
                v + new Vector2(8, 8), 0.45f, Color.Red, TextAlignment.LEFT);
    }
}


//  Activate with [TIMS:DEBUG] in a block's name or Custom Data. Shows:
//    - Script version, uptime, tick rate, execution timing
//    - Block counts and target list with geometry of each LCD
//    - Sample DetailedInfo from a refinery and assembler (for parser changes)
//    - Recent error log
//  Designed for a single tall column of text on any reasonably sized LCD.

void DrawDebugScreen(IMyTextSurface s, IMyTerminalBlock block)
{
    using (var frame = s.DrawFrame())
    {
        var v = GetViewport(s, block);
        AddBox(frame, v.Origin, v.Size, C_BG);

        float headerH = Math.Max(24, Math.Min(50, v.H * 0.08f));
        AddBox(frame, v.Origin, new Vector2(v.W, headerH), C_HOT);
        AddText(frame, "T.I.M.S. DEBUG",
                v.Origin + new Vector2(8, 6),
                Math.Min(0.85f, v.H / 360f), C_BG, TextAlignment.LEFT);
        AddText(frame, "v" + TIMS_VERSION + "  CYC " + _tick.ToString("D6") + "  T+" + FmtUptime(_tick),
                v.Origin + new Vector2(v.W - 8, 6),
                Math.Min(0.55f, v.H / 460f), C_BG, TextAlignment.RIGHT);

        // Build content as a list of (label, value, color) rows.
        var rows = new List<DbgRow>();

        rows.Add(new DbgRow("== RUNTIME ==", "", C_LABEL));
        rows.Add(new DbgRow("last main",    _lastMainMs.ToString("0.00") + " ms", C_TEXT));
        rows.Add(new DbgRow("peak main",    _peakMainMs.ToString("0.00") + " ms",
                 _peakMainMs > 0.5 ? C_HOT : C_TEXT));
        rows.Add(new DbgRow("update freq",  Runtime.UpdateFrequency.ToString(), C_TEXT));

        rows.Add(new DbgRow("== BLOCKS ==", "", C_LABEL));
        rows.Add(new DbgRow("refineries",   _refineries.Count.ToString(), C_TEXT));
        rows.Add(new DbgRow("assemblers",   _assemblers.Count.ToString(), C_TEXT));
        rows.Add(new DbgRow("producers",    _powerProducers.Count.ToString(), C_TEXT));
        rows.Add(new DbgRow("batteries",    _batteries.Count.ToString(), C_TEXT));
        rows.Add(new DbgRow("all blocks",   _allBlocks.Count.ToString(), C_TEXT));

        rows.Add(new DbgRow("== TARGETS (" + _targets.Count + ") ==", "", C_LABEL));
        int shown = 0;
        foreach (var t in _targets)
        {
            if (shown >= 10) { rows.Add(new DbgRow("... " + (_targets.Count - shown) + " more", "", C_DIM)); break; }
            var surf = GetSurface(t);
            string size = "?";
            string compatNote = "";
            string geom = "";
            if (surf != null)
            {
                var ss = surf.SurfaceSize;
                var ts = surf.TextureSize;
                size = ss.X.ToString("0") + "x" + ss.Y.ToString("0");
                var vp = GetViewport(surf, t.Block);
                geom = " o" + vp.Origin.X.ToString("0") + "," + vp.Origin.Y.ToString("0");
                if (IsTooSmall(vp)) compatNote = " INCOMPAT";
                else if (IsWideViewport(vp)) compatNote = " WIDE";
                else if (IsPortrait(vp)) compatNote = " PORTRAIT";
                // Show tex size if different from surface size
                if (Math.Abs(ts.X - ss.X) > 1 || Math.Abs(ts.Y - ss.Y) > 1)
                    geom += " t" + ts.X.ToString("0") + "x" + ts.Y.ToString("0");
            }
            string name = Truncate(t.Block.CustomName, 20);
            string val = t.Tag.ToString().Substring(0, Math.Min(3, t.Tag.ToString().Length)).ToUpper()
                       + ":" + t.SurfaceIndex + " " + size + geom + compatNote;
            Color rowC = compatNote.Contains("INCOMPAT") ? C_HOT : C_TEXT;
            rows.Add(new DbgRow(name, val, rowC));
            shown++;
        }

        // Sample DetailedInfo from first refinery (helps verify parser keys)
        if (_refineries.Count > 0)
        {
            var r = _refineries[0];
            rows.Add(new DbgRow("== SAMPLE REFINERY ==", "", C_LABEL));
            rows.Add(new DbgRow("name", Truncate(r.CustomName, 28), C_TEXT));
            rows.Add(new DbgRow("Required Input", GetCurrentInputMW(r).ToString("0.000") + " MW", C_TEXT));
            rows.Add(new DbgRow("Max Required Input", GetMaxInputMW(r).ToString("0.000") + " MW", C_TEXT));
            rows.Add(new DbgRow("Refine Speed", "x" + GetSpeedFactor(r).ToString("0.00"), C_TEXT));
            rows.Add(new DbgRow("Yield Rate",   "x" + GetYieldFactor(r).ToString("0.00"), C_TEXT));
            rows.Add(new DbgRow("Power Eff",    "x" + GetPowerEffFactor(r).ToString("0.00"), C_TEXT));
            int mu, mt; GetModuleSlots(r, out mu, out mt);
            rows.Add(new DbgRow("modules", mu + "/" + mt, C_TEXT));
            rows.Add(new DbgRow("status", GetProdStatus(r).ToString().ToUpper(), C_TEXT));
        }
        if (_assemblers.Count > 0)
        {
            var a = _assemblers[0];
            rows.Add(new DbgRow("== SAMPLE ASSEMBLER ==", "", C_LABEL));
            rows.Add(new DbgRow("name", Truncate(a.CustomName, 28), C_TEXT));
            rows.Add(new DbgRow("Required Input", GetCurrentInputMW(a).ToString("0.000") + " MW", C_TEXT));
            rows.Add(new DbgRow("Productivity", "x" + GetSpeedFactor(a).ToString("0.00"), C_TEXT));
            rows.Add(new DbgRow("Effectiveness", "x" + GetYieldFactor(a).ToString("0.00"), C_TEXT));
            rows.Add(new DbgRow("status", GetProdStatus(a).ToString().ToUpper(), C_TEXT));
            rows.Add(new DbgRow("mode", a.Mode.ToString().ToUpper(), C_TEXT));
        }

        // Error log
        rows.Add(new DbgRow("== ERRORS (" + _errorLog.Count + ") ==", "", C_LABEL));
        if (_errorLog.Count == 0)
        {
            rows.Add(new DbgRow("none", "", C_DIM));
        }
        else
        {
            foreach (var e in _errorLog)
            {
                rows.Add(new DbgRow(Truncate(e, 50), "", C_HOT));
            }
        }

        // Render rows
        float y = v.Origin.Y + headerH + 6;
        float lineH = Math.Max(12, v.H * 0.028f);
        float scale = Math.Min(0.5f, v.H / 480f);
        if (scale < 0.35f) scale = 0.35f;
        float labelX = v.Origin.X + 8;
        float valX   = v.Origin.X + v.W - 8;
        foreach (var r in rows)
        {
            if (y + lineH > v.Origin.Y + v.H - 4) break;
            AddText(frame, r.Label, new Vector2(labelX, y), scale, r.Color, TextAlignment.LEFT);
            if (!string.IsNullOrEmpty(r.Value))
                AddText(frame, r.Value, new Vector2(valX, y), scale, r.Color, TextAlignment.RIGHT);
            y += lineH;
        }
    }
}

struct DbgRow
{
    public string Label;
    public string Value;
    public Color Color;
    public DbgRow(string l, string v, Color c) { Label = l; Value = v; Color = c; }
}

string Truncate(string s, int len)
{
    if (string.IsNullOrEmpty(s)) return "";
    return s.Length <= len ? s : s.Substring(0, len);
}

// Draw a vertical stack of label/value rows in a panel. Each row is a short
// label on the left and an updating value on the right.
void DrawSidePanel(MySpriteDrawFrame frame, float x, float y, float w, float h,
                   string title, Color tabColor, string[] labels, string[] values, Color[] valueColors)
{
    DrawStripFrame(frame, x, y, w, h, tabColor);
    AddText(frame, title, new Vector2(x + 10, y + 2),
            BASE_TEXT_SCALE, tabColor, TextAlignment.LEFT);

    int n = labels.Length;
    float topPad = 24;
    float rowH = (h - topPad - 6) / Math.Max(1, n);
    for (int i = 0; i < n; i++)
    {
        float ry = y + topPad + i * rowH;
        AddText(frame, labels[i], new Vector2(x + 10, ry),
                SMALL_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);
        Color vc = (valueColors != null && i < valueColors.Length) ? valueColors[i] : C_TEXT;
        AddText(frame, values[i], new Vector2(x + w - 8, ry),
                SMALL_TEXT_SCALE, vc, TextAlignment.RIGHT);
        // Thin separator line below each row
        if (i < n - 1)
            AddBox(frame, new Vector2(x + 6, ry + rowH - 1),
                   new Vector2(w - 12, 1), C_DIM);
    }
}

// Generate the cosmetic refinery side-panel stats. Deterministic from _tick so
// it doesn't jitter unpredictably but does change visibly.
void GetRefinerySideStats(out string[] labels, out string[] values, out Color[] colors)
{
    int t = _tick;
    int churn   = 240 + ((t * 17) % 90);
    int slag    = 1200 + ((t * 13) % 240);
    float purity = 98.5f + ((t * 7) % 14) / 10f;          // 98.5 - 99.8
    string plasma  = (t % 11 < 8) ? "STABLE" : ((t % 11 < 10) ? "SURGING" : "COOLDOWN");
    Color  plasmaC = plasma == "STABLE" ? C_PRIMARY : (plasma == "SURGING" ? C_HOT : C_LABEL);
    int coolant = 145 + ((t * 3) % 12);
    float voltage = 4.80f + ((t * 5) % 30) / 100f;
    int arcK    = 8400 + ((t * 11) % 320);
    string vent = (t % 7 < 5) ? "CLOSED" : "OPEN";
    Color  ventC = vent == "CLOSED" ? C_PRIMARY : C_FRAME;

    // Commodity ticker as last 4 rows
    string[] syms  = new string[] { "Fe", "Ni", "Pt", "U" };
    float[] bases  = new float[]   { 0.42f, 1.18f, 7.80f, 12.4f };
    string[] commLabels = new string[4];
    string[] commVals   = new string[4];
    Color[]  commCols   = new Color[4];
    for (int i = 0; i < 4; i++)
    {
        int seed = syms[i].GetHashCode() ^ t;
        float delta = ((seed % 200) - 100) / 1000f;
        float price = bases[i] * (1f + delta);
        commLabels[i] = "MKT " + syms[i];
        commVals[i]   = price.ToString("0.00") + " "
                      + (delta >= 0 ? "+" : "") + (delta * 100f).ToString("0.0") + "%";
        commCols[i]   = delta >= 0 ? C_PRIMARY : C_HOT;
    }

    labels = new string[]
    {
        "ORE CHURN", "SLAG OUT", "INGOT PURITY", "PLASMA",
        "COOLANT", "ELECTRO", "ARC TEMP", "VENT",
        commLabels[0], commLabels[1], commLabels[2], commLabels[3]
    };
    values = new string[]
    {
        churn + "/h", slag + "kg", purity.ToString("0.00") + "%", plasma,
        coolant + "psi", voltage.ToString("0.00") + "kV", arcK + "K", vent,
        commVals[0], commVals[1], commVals[2], commVals[3]
    };
    colors = new Color[]
    {
        C_TEXT, C_TEXT, C_PRIMARY, plasmaC,
        C_TEXT, C_TEXT, C_PRIMARY, ventC,
        commCols[0], commCols[1], commCols[2], commCols[3]
    };
}

void GetAssemblerSideStats(out string[] labels, out string[] values, out Color[] colors)
{
    int t = _tick;
    float align   = 96.0f + ((t * 4) % 40) / 10f;          // 96.0 - 99.9
    string bitFeed = FmtHex(t * 1337).Substring(0, 6);
    int sinter    = 1850 + ((t * 9) % 220);
    int tolerance = 8 + ((t * 3) % 6);
    string coolant = (t % 9 < 7) ? "STABLE" : "LOAD";
    Color coolantC = coolant == "STABLE" ? C_PRIMARY : C_FRAME;
    int rpm       = 4400 + ((t * 7) % 600);
    int buffer    = 285 + ((t * 5) % 40);
    int qaRej     = (t * 3) % 12;
    Color qaC     = qaRej < 5 ? C_PRIMARY : C_HOT;
    float thermal = ((t * 11) % 50 - 25) / 10f;            // -2.5 to +2.5
    string shift  = "SHF-" + ((t / 7) % 999).ToString("D3");

    labels = new string[]
    {
        "ALIGN", "BIT FEED", "SINTER", "TOLERANCE",
        "COOLANT", "HEAD RPM", "BUFFER", "QA REJ",
        "THERM Δ", "SHIFT"
    };
    values = new string[]
    {
        align.ToString("0.0") + "%", "0x" + bitFeed, sinter + "W/mm²", tolerance + "μm",
        coolant, rpm + "rpm", buffer + "kPa", qaRej.ToString(),
        (thermal >= 0 ? "+" : "") + thermal.ToString("0.0") + "°C", shift
    };
    colors = new Color[]
    {
        C_PRIMARY, C_TEXT, C_TEXT, C_TEXT,
        coolantC, C_TEXT, C_TEXT, qaC,
        C_TEXT, C_LABEL
    };
}

//  REFINERY SCREEN

void DrawRefineryScreen(IMyTextSurface s, IMyTerminalBlock block)
{
    using (var frame = s.DrawFrame())
    {
        var v = GetViewport(s, block);
        float contentY = DrawHeader(frame, v, "REFINERY OPS");

        var tiers = AggregateRefineries();

        int totalN = 0, totalA = 0;
        float totalC = 0f, totalM = 0f;
        long totalQ = 0;
        int totalStarved = 0, totalNoPwr = 0, totalOffl = 0, totalDmg = 0, totalIdle = 0;
        foreach (var t in tiers)
        {
            totalN += t.N; totalA += t.A;
            totalC += t.C; totalM += t.M; totalQ += t.Q;
            totalStarved += t.StarvedCount; totalNoPwr += t.NoPowerCount;
            totalOffl += t.OfflineCount; totalDmg += t.DamagedCount; totalIdle += t.IdleCount;
        }

        bool wide = IsWideViewport(v);
        float pad = 4;

        // Column layout: on wide LCDs, left column = real data (tiers + aggregate),
        // right column = cosmetic side panel. On narrow/tall LCDs, single column
        // with the merchant strip at the bottom.
        float leftX, leftW;
        if (wide)
        {
            // 62% left, 36% right, 2% gap
            leftX = v.Origin.X + 8;
            leftW = v.W * 0.60f - 8;
            float rightX = v.Origin.X + v.W * 0.64f;
            float rightW = v.W * 0.36f - 8;
            float availH = v.H - (contentY - v.Origin.Y) - 8;

            // Right column: cosmetic stats panel
            string[] sl, sv; Color[] sc;
            GetRefinerySideStats(out sl, out sv, out sc);
            DrawSidePanel(frame, rightX, contentY, rightW, availH,
                          "MERCHANT // INSTRUMENTATION", C_LABEL, sl, sv, sc);
        }
        else
        {
            leftX = v.Origin.X + 8;
            leftW = v.W - 16;
        }

        float availLeftH = v.H - (contentY - v.Origin.Y) - 8;
        // On wide, no merchant strip (it's in the right panel). Strips = tiers + aggregate.
        // On narrow, strips = tiers + aggregate + merchant.
        int strips = tiers.Count + (wide ? 1 : 2);
        if (strips < (wide ? 1 : 2)) strips = (wide ? 1 : 2);
        float stripH = (availLeftH - pad * (strips - 1)) / strips;

        float ry = contentY;
        for (int i = 0; i < tiers.Count; i++)
        {
            DrawRefTierStrip(frame, leftX, ry, leftW, stripH, tiers[i]);
            ry += stripH + pad;
        }

        if (tiers.Count == 0)
        {
            DrawStripFrame(frame, leftX, ry, leftW, stripH, C_DIM);
            AddText(frame, "NO REFINERY UNITS DETECTED",
                    new Vector2(leftX + leftW * 0.5f, ry + stripH * 0.40f),
                    BASE_TEXT_SCALE, C_DIM, TextAlignment.CENTER);
            ry += stripH + pad;
        }

        DrawRefAggStrip(frame, leftX, ry, leftW, stripH, totalN, totalA, totalC, totalM, totalQ,
                        totalStarved, totalNoPwr, totalOffl, totalDmg, totalIdle);
        ry += stripH + pad;

        if (!wide)
        {
            DrawMerchantStrip(frame, leftX, ry, leftW, stripH);
        }
    }
}

struct RefTier
{
    public string Label;
    public Color Color;
    public int N, A;
    public float C, M;
    public long Q;
    public float SpeedAvg, YieldAvg, PowerEffAvg;
    public int ModUsed, ModTotal;
    public int StarvedCount, NoPowerCount, OfflineCount, DamagedCount, IdleCount;
}

// Aggregate refineries into a list of tier records. Empty tiers are skipped.
List<RefTier> AggregateRefineries()
{
    // Working accumulators indexed by tier (Basic, Standard, Industrial, Prototech)
    int[] n = new int[4], a = new int[4];
    float[] cur = new float[4], max = new float[4];
    long[] qu = new long[4];
    float[] spd = new float[4], yld = new float[4], eff = new float[4];
    int[] mu = new int[4], mt = new int[4];
    int[] starv = new int[4], nopwr = new int[4], offl = new int[4], dmg = new int[4], idle = new int[4];

    foreach (var r in _refineries)
    {
        string sub = (r.BlockDefinition.SubtypeId ?? "").ToUpperInvariant();
        int t;
        if (sub.Contains("PROTOTECH")) t = 3;
        else if (sub.Contains("INDUSTRIAL")) t = 2;
        else if (sub.Contains("BLAST") || sub.Contains("FURNACE") || sub.Contains("BASIC")) t = 0;
        else t = 1;

        n[t]++;
        cur[t] += GetCurrentInputMW(r);
        max[t] += GetMaxInputMW(r);
        qu[t]  += CountInventoryItems(r);
        if (r.IsProducing) a[t]++;
        spd[t] += GetSpeedFactor(r);
        yld[t] += GetYieldFactor(r);
        eff[t] += GetPowerEffFactor(r);
        int u, tot; GetModuleSlots(r, out u, out tot);
        mu[t] += u; mt[t] += tot;
        switch (GetProdStatus(r))
        {
            case ProdStatus.Starved: starv[t]++; break;
            case ProdStatus.NoPower: nopwr[t]++; break;
            case ProdStatus.Offline: offl[t]++; break;
            case ProdStatus.Damaged: dmg[t]++; break;
            case ProdStatus.Idle:    idle[t]++; break;
        }
    }

    string[] labels = new string[] { "BASIC", "STANDARD", "INDUSTRIAL", "PROTOTECH" };
    Color[]  colors = new Color[]  { C_PRIMARY, C_FRAME, C_ACCENT, C_HOT };

    var result = new List<RefTier>();
    for (int t = 0; t < 4; t++)
    {
        if (n[t] == 0) continue;
        result.Add(new RefTier
        {
            Label = labels[t], Color = colors[t],
            N = n[t], A = a[t], C = cur[t], M = max[t], Q = qu[t],
            SpeedAvg    = spd[t] / n[t],
            YieldAvg    = yld[t] / n[t],
            PowerEffAvg = eff[t] / n[t],
            ModUsed = mu[t], ModTotal = mt[t],
            StarvedCount = starv[t], NoPowerCount = nopwr[t],
            OfflineCount = offl[t], DamagedCount = dmg[t], IdleCount = idle[t]
        });
    }
    return result;
}

void DrawRefTierStrip(MySpriteDrawFrame frame, float x, float y, float w, float h, RefTier t)
{
    DrawStripFrame(frame, x, y, w, h, t.Color);

    float labelW = w * 0.22f;
    float barX = x + labelW + 4;
    float barW = w * 0.50f;

    // Fixed row spacing so layout works at any strip height. Each text row is
    // ~16px. Bar absorbs remaining vertical space between row 1 (top) and the
    // mod/bottom rows.
    const float ROW_H = 16f;
    float topY    = y + 4;            // tier label + current power
    float barY    = y + ROW_H + 4;    // bar
    float modY    = y + h - ROW_H * 2 - 4;  // mod/stats row
    float bottomY = y + h - ROW_H;    // unit count + dots + queue/badge
    float barH    = Math.Max(6, modY - barY - 4);
    if (barH > ROW_H) barH = ROW_H;   // cap bar height so it doesn't dominate

    // Row 1: tier label LEFT, current/max power RIGHT (combined to avoid
    // collision with the mod row on short strips)
    AddText(frame, t.Label, new Vector2(x + 12, topY),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);
    AddText(frame, FmtPower(t.C) + " / " + FmtPower(t.M),
            new Vector2(x + w - 8, topY),
            BASE_TEXT_SCALE, C_TEXT, TextAlignment.RIGHT);

    // Bar (no overlaid text)
    float load = t.M > 0 ? t.C / t.M : 0f;
    AddBar(frame, new Vector2(barX, barY), new Vector2(barW, barH), load, C_ACCENT, C_DIM);

    // Row 3: SPD / YLD / PWR / MOD slots
    string modLine = "SPD x" + t.SpeedAvg.ToString("0.00")
                  + "  YLD x" + t.YieldAvg.ToString("0.00")
                  + "  PWR x" + t.PowerEffAvg.ToString("0.00");
    AddText(frame, modLine, new Vector2(barX, modY),
            SMALL_TEXT_SCALE, C_TEXT, TextAlignment.LEFT);
    AddText(frame, "MOD " + t.ModUsed + "/" + t.ModTotal,
            new Vector2(x + w - 8, modY),
            SMALL_TEXT_SCALE, C_LABEL, TextAlignment.RIGHT);

    // Status badge (worst issue first)
    string badge = "";
    Color badgeC = C_PRIMARY;
    if (t.DamagedCount > 0)      { badge = "DMG:"     + t.DamagedCount; badgeC = C_HOT; }
    else if (t.NoPowerCount > 0) { badge = "NOPWR:"   + t.NoPowerCount; badgeC = C_HOT; }
    else if (t.StarvedCount > 0) { badge = "STARVED:" + t.StarvedCount; badgeC = C_HOT; }
    else if (t.OfflineCount > 0) { badge = "OFF:"     + t.OfflineCount; badgeC = C_DIM; }
    else if (t.IdleCount > 0)    { badge = "IDLE:"    + t.IdleCount;    badgeC = C_LABEL; }

    // Row 4: unit count + active dots + status badge + queue
    AddText(frame, t.N.ToString("00") + " UN", new Vector2(x + 12, bottomY),
            SMALL_TEXT_SCALE, C_PRIMARY, TextAlignment.LEFT);
    float dotY = bottomY + 6;
    int dotsToShow = Math.Min(t.N, 10);
    for (int i = 0; i < dotsToShow; i++)
    {
        bool on = i < t.A;
        AddDot(frame, new Vector2(barX + 4 + i * 10, dotY), 3, on ? C_PRIMARY : C_DIM);
    }
    if (t.N > 10)
        AddText(frame, "+" + (t.N - 10), new Vector2(barX + 4 + 10 * 10 + 4, dotY - 6),
                SMALL_TEXT_SCALE, C_PRIMARY, TextAlignment.LEFT);
    string rightLine = (badge.Length > 0 ? badge + "  " : "") + "Q " + t.Q;
    AddText(frame, rightLine, new Vector2(x + w - 8, bottomY),
            SMALL_TEXT_SCALE, badge.Length > 0 ? badgeC : C_TEXT, TextAlignment.RIGHT);
}

void DrawRefAggStrip(MySpriteDrawFrame frame, float x, float y, float w, float h,
                     int total, int active, float cur, float max, long queue,
                     int starved, int noPwr, int offl, int dmg, int idle)
{
    DrawStripFrame(frame, x, y, w, h, C_HOT);
    const float ROW_H = 16f;
    float topY    = y + 4;
    float barY    = y + ROW_H + 4;
    float dataY   = y + h - ROW_H * 2 - 4;
    float bottomY = y + h - ROW_H;
    float barH    = Math.Max(6, dataY - barY - 4);
    if (barH > ROW_H) barH = ROW_H;

    AddText(frame, "FLEET AGGREGATE", new Vector2(x + 12, topY),
            BASE_TEXT_SCALE, C_HOT, TextAlignment.LEFT);
    AddText(frame, active + "/" + total + " ACTIVE", new Vector2(x + w - 8, topY),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.RIGHT);

    float load = max > 0 ? cur / max : 0f;
    AddBar(frame, new Vector2(x + 12, barY), new Vector2(w - 24, barH), load, C_HOT, C_DIM);

    AddText(frame, FmtPower(cur) + " / " + FmtPower(max) + "   " + (load * 100f).ToString("0.0") + "%",
            new Vector2(x + 12, dataY), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.LEFT);
    AddText(frame, "ORE Q " + queue,
            new Vector2(x + w - 8, dataY), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.RIGHT);

    // Bottom row: status badges. Only show non-zero counts.
    string badges = "";
    if (starved > 0) badges += "STARVED:" + starved + "  ";
    if (noPwr > 0)   badges += "NOPWR:" + noPwr + "  ";
    if (dmg > 0)     badges += "DMG:" + dmg + "  ";
    if (offl > 0)    badges += "OFF:" + offl + "  ";
    if (idle > 0)    badges += "IDLE:" + idle;
    Color badgeColor = (starved + noPwr + dmg) > 0 ? C_HOT : C_LABEL;
    if (badges.Length > 0)
        AddText(frame, badges.TrimEnd(), new Vector2(x + 12, bottomY),
                SMALL_TEXT_SCALE, badgeColor, TextAlignment.LEFT);
    else
        AddText(frame, "ALL NOMINAL", new Vector2(x + 12, bottomY),
                SMALL_TEXT_SCALE, C_PRIMARY, TextAlignment.LEFT);
}

void DrawMerchantStrip(MySpriteDrawFrame frame, float x, float y, float w, float h)
{
    DrawStripFrame(frame, x, y, w, h, C_LABEL);
    AddText(frame, "MERCHANT INTEL", new Vector2(x + 12, y + 4),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);

    string[] syms  = new string[] { "Fe", "Ni", "Pt", "U" };
    float[] bases  = new float[]   { 0.42f, 1.18f, 7.80f, 12.4f };
    // Span full width below the title bar to avoid collision with the label.
    float colsX = x + 12;
    float colsW = w - 24;
    int n = syms.Length;
    float colW = colsW / n;
    float symY   = y + h * 0.32f;
    float priceY = y + h * 0.55f;
    float deltaY = y + h * 0.80f;
    for (int i = 0; i < n; i++)
    {
        int seed = syms[i].GetHashCode() ^ _tick;
        float delta = ((seed % 200) - 100) / 1000f;
        float price = bases[i] * (1f + delta);
        Color c = delta >= 0 ? C_PRIMARY : C_HOT;
        float colX = colsX + i * colW;
        AddText(frame, syms[i], new Vector2(colX + colW * 0.5f, symY),
                SMALL_TEXT_SCALE, C_LABEL, TextAlignment.CENTER);
        AddText(frame, price.ToString("0.00"), new Vector2(colX + colW * 0.5f, priceY),
                BASE_TEXT_SCALE, C_TEXT, TextAlignment.CENTER);
        AddText(frame, (delta >= 0 ? "+" : "") + (delta * 100f).ToString("0.0") + "%",
                new Vector2(colX + colW * 0.5f, deltaY),
                SMALL_TEXT_SCALE, c, TextAlignment.CENTER);
    }
}

//  ASSEMBLER SCREEN

List<AsmTier> AggregateAssemblers()
{
    int[] n = new int[4], a = new int[4];
    float[] cur = new float[4], max = new float[4];
    int[] qu = new int[4];
    float[] spd = new float[4], yld = new float[4], eff = new float[4];
    int[] mu = new int[4], mt = new int[4];
    int[] coop = new int[4], das = new int[4];
    int[] starv = new int[4], nopwr = new int[4], offl = new int[4], dmg = new int[4], idle = new int[4];

    foreach (var b in _assemblers)
    {
        string sub = (b.BlockDefinition.SubtypeId ?? "").ToUpperInvariant();
        int t;
        if (sub.Contains("PROTOTECH")) t = 3;
        else if (sub.Contains("INDUSTRIAL")) t = 2;
        else if (sub.Contains("BASIC")) t = 0;
        else t = 1;

        n[t]++;
        cur[t] += GetCurrentInputMW(b);
        max[t] += GetMaxInputMW(b);
        var qList = new List<MyProductionItem>();
        b.GetQueue(qList);
        qu[t] += qList.Count;
        if (b.IsProducing) a[t]++;
        spd[t] += GetSpeedFactor(b);
        yld[t] += GetYieldFactor(b);
        eff[t] += GetPowerEffFactor(b);
        int u, tot; GetModuleSlots(b, out u, out tot);
        mu[t] += u; mt[t] += tot;
        if (b.CooperativeMode) coop[t]++;
        if (b.Mode == MyAssemblerMode.Disassembly) das[t]++;
        switch (GetProdStatus(b))
        {
            case ProdStatus.Starved: starv[t]++; break;
            case ProdStatus.NoPower: nopwr[t]++; break;
            case ProdStatus.Offline: offl[t]++; break;
            case ProdStatus.Damaged: dmg[t]++; break;
            case ProdStatus.Idle:    idle[t]++; break;
        }
    }

    string[] labels = new string[] { "BASIC", "STANDARD", "INDUSTRIAL", "PROTOTECH" };
    Color[]  colors = new Color[]  { C_PRIMARY, C_FRAME, C_ACCENT, C_HOT };

    var result = new List<AsmTier>();
    for (int t = 0; t < 4; t++)
    {
        if (n[t] == 0) continue;
        result.Add(new AsmTier
        {
            Label = labels[t], Color = colors[t],
            N = n[t], A = a[t], C = cur[t], M = max[t], Q = qu[t],
            SpeedAvg    = spd[t] / n[t],
            YieldAvg    = yld[t] / n[t],
            PowerEffAvg = eff[t] / n[t],
            ModUsed = mu[t], ModTotal = mt[t],
            CoopCount = coop[t], DisasmCount = das[t],
            StarvedCount = starv[t], NoPowerCount = nopwr[t],
            OfflineCount = offl[t], DamagedCount = dmg[t], IdleCount = idle[t]
        });
    }
    return result;
}

void DrawAssemblerScreen(IMyTextSurface s, IMyTerminalBlock block)
{
    using (var frame = s.DrawFrame())
    {
        var v = GetViewport(s, block);
        float contentY = DrawHeader(frame, v, "ASSEMBLER OPS");

        var tiers = AggregateAssemblers();
        int totalN = 0, totalA = 0, totalQ = 0;
        float totalC = 0f, totalM = 0f;
        int totalStarved = 0, totalNoPwr = 0, totalOffl = 0, totalDmg = 0, totalIdle = 0;
        foreach (var t in tiers)
        {
            totalN += t.N; totalA += t.A; totalQ += t.Q;
            totalC += t.C; totalM += t.M;
            totalStarved += t.StarvedCount; totalNoPwr += t.NoPowerCount;
            totalOffl += t.OfflineCount; totalDmg += t.DamagedCount; totalIdle += t.IdleCount;
        }

        bool wide = IsWideViewport(v);
        float pad = 4;
        float leftX, leftW;
        if (wide)
        {
            leftX = v.Origin.X + 8;
            leftW = v.W * 0.60f - 8;
            float rightX = v.Origin.X + v.W * 0.64f;
            float rightW = v.W * 0.36f - 8;
            float availH = v.H - (contentY - v.Origin.Y) - 8;
            string[] sl, sv; Color[] sc;
            GetAssemblerSideStats(out sl, out sv, out sc);
            DrawSidePanel(frame, rightX, contentY, rightW, availH,
                          "FAB INSTRUMENTATION", C_LABEL, sl, sv, sc);
        }
        else
        {
            leftX = v.Origin.X + 8;
            leftW = v.W - 16;
        }

        float availLeftH = v.H - (contentY - v.Origin.Y) - 8;
        int strips = tiers.Count + (wide ? 1 : 2);
        if (strips < (wide ? 1 : 2)) strips = (wide ? 1 : 2);
        float stripH = (availLeftH - pad * (strips - 1)) / strips;

        float ry = contentY;
        for (int i = 0; i < tiers.Count; i++)
        {
            DrawAsmTierStrip(frame, leftX, ry, leftW, stripH, tiers[i]);
            ry += stripH + pad;
        }

        if (tiers.Count == 0)
        {
            DrawStripFrame(frame, leftX, ry, leftW, stripH, C_DIM);
            AddText(frame, "NO ASSEMBLER UNITS DETECTED",
                    new Vector2(leftX + leftW * 0.5f, ry + stripH * 0.40f),
                    BASE_TEXT_SCALE, C_DIM, TextAlignment.CENTER);
            ry += stripH + pad;
        }

        DrawAsmAggStrip(frame, leftX, ry, leftW, stripH,
                        totalN, totalA, totalC, totalM, totalQ,
                        totalStarved, totalNoPwr, totalOffl, totalDmg, totalIdle);
        ry += stripH + pad;

        if (!wide)
        {
            DrawTelemetryStrip(frame, leftX, ry, leftW, stripH);
        }
    }
}

void DrawAsmAggStrip(MySpriteDrawFrame frame, float x, float y, float w, float h,
                     int total, int active, float cur, float max, int queue,
                     int starved, int noPwr, int offl, int dmg, int idle)
{
    DrawStripFrame(frame, x, y, w, h, C_HOT);
    const float ROW_H = 16f;
    float topY    = y + 4;
    float barY    = y + ROW_H + 4;
    float dataY   = y + h - ROW_H * 2 - 4;
    float bottomY = y + h - ROW_H;
    float barH    = Math.Max(6, dataY - barY - 4);
    if (barH > ROW_H) barH = ROW_H;

    AddText(frame, "FAB AGGREGATE", new Vector2(x + 12, topY),
            BASE_TEXT_SCALE, C_HOT, TextAlignment.LEFT);
    AddText(frame, active + "/" + total + " ACTIVE", new Vector2(x + w - 8, topY),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.RIGHT);

    float load = max > 0 ? cur / max : 0f;
    AddBar(frame, new Vector2(x + 12, barY), new Vector2(w - 24, barH), load, C_HOT, C_DIM);

    AddText(frame, FmtPower(cur) + " / " + FmtPower(max),
            new Vector2(x + 12, dataY), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.LEFT);
    AddText(frame, "Q " + queue + " ORDERS",
            new Vector2(x + w - 8, dataY), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.RIGHT);

    string badges = "";
    if (starved > 0) badges += "STARVED:" + starved + "  ";
    if (noPwr > 0)   badges += "NOPWR:" + noPwr + "  ";
    if (dmg > 0)     badges += "DMG:" + dmg + "  ";
    if (offl > 0)    badges += "OFF:" + offl + "  ";
    if (idle > 0)    badges += "IDLE:" + idle;
    Color badgeColor = (starved + noPwr + dmg) > 0 ? C_HOT : C_LABEL;
    if (badges.Length > 0)
        AddText(frame, badges.TrimEnd(), new Vector2(x + 12, bottomY),
                SMALL_TEXT_SCALE, badgeColor, TextAlignment.LEFT);
    else
        AddText(frame, "ALL NOMINAL", new Vector2(x + 12, bottomY),
                SMALL_TEXT_SCALE, C_PRIMARY, TextAlignment.LEFT);
}

struct AsmTier
{
    public string Label;
    public Color Color;
    public int N, A, Q;
    public float C, M;
    public float SpeedAvg, YieldAvg, PowerEffAvg;
    public int ModUsed, ModTotal;
    public int CoopCount;
    public int DisasmCount;
    public int StarvedCount, NoPowerCount, OfflineCount, DamagedCount, IdleCount;
}

void DrawAsmTierStrip(MySpriteDrawFrame frame, float x, float y, float w, float h, AsmTier t)
{
    DrawStripFrame(frame, x, y, w, h, t.Color);

    float labelW = w * 0.22f;
    float barX = x + labelW + 4;
    float barW = w * 0.50f;

    const float ROW_H = 16f;
    float topY    = y + 4;
    float barY    = y + ROW_H + 4;
    float modY    = y + h - ROW_H * 2 - 4;
    float bottomY = y + h - ROW_H;
    float barH    = Math.Max(6, modY - barY - 4);
    if (barH > ROW_H) barH = ROW_H;

    AddText(frame, t.Label, new Vector2(x + 12, topY),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);
    AddText(frame, FmtPower(t.C) + " / " + FmtPower(t.M),
            new Vector2(x + w - 8, topY),
            BASE_TEXT_SCALE, C_TEXT, TextAlignment.RIGHT);

    float load = t.M > 0 ? t.C / t.M : 0f;
    AddBar(frame, new Vector2(barX, barY), new Vector2(barW, barH), load, C_ACCENT, C_DIM);

    string modLine = "SPD x" + t.SpeedAvg.ToString("0.00")
                  + "  YLD x" + t.YieldAvg.ToString("0.00")
                  + "  PWR x" + t.PowerEffAvg.ToString("0.00");
    AddText(frame, modLine, new Vector2(barX, modY),
            SMALL_TEXT_SCALE, C_TEXT, TextAlignment.LEFT);
    AddText(frame, "MOD " + t.ModUsed + "/" + t.ModTotal,
            new Vector2(x + w - 8, modY),
            SMALL_TEXT_SCALE, C_LABEL, TextAlignment.RIGHT);

    // Status badge: pick worst issue
    string badge = "";
    Color badgeC = C_PRIMARY;
    if (t.DamagedCount > 0)      { badge = "DMG:"     + t.DamagedCount; badgeC = C_HOT; }
    else if (t.NoPowerCount > 0) { badge = "NOPWR:"   + t.NoPowerCount; badgeC = C_HOT; }
    else if (t.StarvedCount > 0) { badge = "STARVED:" + t.StarvedCount; badgeC = C_HOT; }
    else if (t.OfflineCount > 0) { badge = "OFF:"     + t.OfflineCount; badgeC = C_DIM; }
    else if (t.IdleCount > 0)    { badge = "IDLE:"    + t.IdleCount;    badgeC = C_LABEL; }

    AddText(frame, t.N.ToString("00") + " UN", new Vector2(x + 12, bottomY),
            SMALL_TEXT_SCALE, C_PRIMARY, TextAlignment.LEFT);

    float dotY = bottomY + 6;
    int dotsToShow = Math.Min(t.N, 10);
    for (int i = 0; i < dotsToShow; i++)
    {
        bool on = i < t.A;
        AddDot(frame, new Vector2(barX + 4 + i * 10, dotY), 3, on ? C_PRIMARY : C_DIM);
    }

    string modeFlags = "";
    if (t.DisasmCount > 0) modeFlags += "DASM:" + t.DisasmCount + " ";
    if (t.CoopCount > 0)   modeFlags += "COOP:" + t.CoopCount + " ";
    string rightLine = (badge.Length > 0 ? badge + "  " : "") + modeFlags + "Q " + t.Q;
    AddText(frame, rightLine, new Vector2(x + w - 8, bottomY),
            SMALL_TEXT_SCALE, badge.Length > 0 ? badgeC : C_TEXT, TextAlignment.RIGHT);
}

void DrawTelemetryStrip(MySpriteDrawFrame frame, float x, float y, float w, float h)
{
    DrawStripFrame(frame, x, y, w, h, C_LABEL);
    AddText(frame, "PRODUCTION TELEMETRY", new Vector2(x + 12, y + 4),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);

    string[] labels = new string[] { "THRU", "REJECT", "ALIGN", "COOLANT" };
    string[] vals = new string[]
    {
        (220 + (_tick * 13) % 80) + "/h",
        (((_tick * 7) % 30) / 10f).ToString("0.0") + "%",
        (96 + (_tick % 4)) + "%",
        (_tick % 2 == 0) ? "OK" : "[OK]"
    };
    int n = labels.Length;
    // Columns span full width below the title so headers don't collide
    // with the "PRODUCTION TELEMETRY" label on the top row.
    float colsX = x + 12;
    float colsW = w - 24;
    float colW = colsW / n;
    float labelY = y + h * 0.40f;
    float valY   = y + h * 0.68f;
    for (int i = 0; i < n; i++)
    {
        float colX = colsX + i * colW;
        AddText(frame, labels[i], new Vector2(colX + colW * 0.5f, labelY),
                SMALL_TEXT_SCALE, C_LABEL, TextAlignment.CENTER);
        AddText(frame, vals[i], new Vector2(colX + colW * 0.5f, valY),
                BASE_TEXT_SCALE, C_TEXT, TextAlignment.CENTER);
    }
}

//  POWER SCREEN

void DrawPowerScreen(IMyTextSurface s, IMyTerminalBlock block)
{
    using (var frame = s.DrawFrame())
    {
        var v = GetViewport(s, block);
        float contentY = DrawHeader(frame, v, "POWER COMMAND");

        float x = v.Origin.X + 8;
        float w = v.W - 16;
        float availH = v.H - (contentY - v.Origin.Y) - 8;

        // 5 strips, weighted: generation, battery, doctrine, tactical, status
        // doctrine is taller because it has 6 sub-rows
        int strips = 5;
        float pad = 4;
        // Allocate: gen 18%, batt 18%, doctrine 30%, tactical 22%, status 12%
        float[] weights = new float[] { 0.18f, 0.18f, 0.30f, 0.22f, 0.12f };
        float[] heights = new float[strips];
        float remaining = availH - pad * (strips - 1);
        for (int i = 0; i < strips; i++) heights[i] = remaining * weights[i];

        // GENERATION
        float rCur=0,rMax=0; int rN=0;
        float sCur=0,sMax=0; int sN=0;
        float wCur=0,wMax=0; int wN=0;
        float hCur=0,hMax=0; int hN=0;
        float oCur=0,oMax=0; int oN=0;
        foreach (var p in _powerProducers)
        {
            if (p is IMyBatteryBlock) continue;
            string sub = (p.BlockDefinition.SubtypeId ?? "").ToUpperInvariant();
            string typ = (p.BlockDefinition.TypeIdString ?? "").ToUpperInvariant();
            if (typ.Contains("REACTOR"))                            { rCur += p.CurrentOutput; rMax += p.MaxOutput; rN++; }
            else if (typ.Contains("SOLAR") || sub.Contains("SOLAR")){ sCur += p.CurrentOutput; sMax += p.MaxOutput; sN++; }
            else if (typ.Contains("WIND")  || sub.Contains("WIND")) { wCur += p.CurrentOutput; wMax += p.MaxOutput; wN++; }
            else if (typ.Contains("HYDRO") || sub.Contains("HYDRO")){ hCur += p.CurrentOutput; hMax += p.MaxOutput; hN++; }
            else                                                    { oCur += p.CurrentOutput; oMax += p.MaxOutput; oN++; }
        }

        float bCur=0,bMax=0,bIn=0,bStored=0,bCap=0; int bN=0,bChg=0;
        foreach (var b in _batteries)
        {
            bCur += b.CurrentOutput; bMax += b.MaxOutput; bIn += b.CurrentInput;
            bStored += b.CurrentStoredPower; bCap += b.MaxStoredPower; bN++;
            if (b.ChargeMode == ChargeMode.Recharge || b.CurrentInput > b.CurrentOutput) bChg++;
        }
        // Generators are ALL producers except batteries (so rCur,sCur,wCur,hCur,oCur).
        // genCur below INCLUDES bCur (battery discharge output) for sizing purposes.
        float genCur = rCur + sCur + wCur + hCur + oCur + bCur;
        float genMax = rMax + sMax + wMax + hMax + oMax + bMax;

        // REAL total consumption: by conservation of energy, what the ship's loads
        // are actually drawing equals what's being produced minus what's going
        // into battery storage. (Producers + battery_discharge) - battery_charge.
        // This is the only ground truth available; per-block estimates from
        // GetResourceSinkInput are approximations and don't sum to reality.
        float producersOnly = rCur + sCur + wCur + hCur + oCur;
        float netBatteryCharge = bIn - bCur;  // positive when net charging
        float realTotalDraw = producersOnly + bCur - bIn;  // = producers + dischrg - chrg
        if (realTotalDraw < 0f) realTotalDraw = 0f;

        // Per-block category breakdown (estimates only - used for proportions).
        var cats = ClassifyLoads();
        float estTotalDraw = 0f;
        foreach (var kv in cats) estTotalDraw += kv.Value;

        // Rescale estimates so their sum matches the real total. This preserves
        // the relative proportions (Industry dominates etc) while making the
        // displayed numbers consistent with PRIMARY GENERATION and BATTERY ARRAY.
        if (estTotalDraw > 0.001f && realTotalDraw > 0.001f)
        {
            float scale = realTotalDraw / estTotalDraw;
            var rescaled = new Dictionary<Category, float>();
            foreach (var kv in cats) rescaled[kv.Key] = kv.Value * scale;
            cats = rescaled;
        }
        float totalDraw = realTotalDraw;

        float ry = contentY;
        DrawGenStrip(frame, x, ry, w, heights[0],
            new string[] { "REACTOR", "SOLAR", "WIND", "HYDRO", "OTHER" },
            new int[]    { rN, sN, wN, hN, oN },
            new float[]  { rCur, sCur, wCur, hCur, oCur },
            new float[]  { rMax, sMax, wMax, hMax, oMax });
        ry += heights[0] + pad;

        DrawBatteryStrip(frame, x, ry, w, heights[1], bN, bChg, bStored, bCap, bCur, bMax, bIn);
        ry += heights[1] + pad;

        DrawDoctrineStrip(frame, x, ry, w, heights[2], cats, totalDraw);
        ry += heights[2] + pad;

        DrawTacticalStrip(frame, x, ry, w, heights[3], totalDraw, genMax, genCur, bStored, bCap);
        ry += heights[3] + pad;

        DrawStatusStrip(frame, x, ry, w, heights[4]);
    }
}

void DrawGenStrip(MySpriteDrawFrame frame, float x, float y, float w, float h,
                  string[] labels, int[] counts, float[] cur, float[] max)
{
    DrawStripFrame(frame, x, y, w, h, C_PRIMARY);
    const float ROW_H = 16f;
    float headerY = y + 4;
    // Anchor labels and values to the BOTTOM but never let them collide
    // with the header. Header occupies roughly y..y+ROW_H.
    float valY    = Math.Max(y + ROW_H * 2 + 4, y + h - ROW_H);
    float labelY  = Math.Max(y + ROW_H + 4,     valY - ROW_H);
    float barY    = headerY + ROW_H + 2;
    float barH    = Math.Max(4, labelY - barY - 2);
    if (barH > ROW_H) barH = ROW_H;

    AddText(frame, "PRIMARY GENERATION", new Vector2(x + 12, headerY),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);

    int n = labels.Length;
    float colsX = x + 12;
    float colsW = w - 24;
    float colW = colsW / n;

    for (int i = 0; i < n; i++)
    {
        float colX = colsX + i * colW;
        Color c = counts[i] > 0 ? C_PRIMARY : C_DIM;
        float frac = max[i] > 0 ? cur[i] / max[i] : 0f;
        AddBar(frame, new Vector2(colX + 4, barY), new Vector2(colW - 8, barH), frac, c, C_DIM, false);

        AddText(frame, labels[i] + " " + counts[i].ToString("00"),
                new Vector2(colX + colW * 0.5f, labelY),
                SMALL_TEXT_SCALE, c, TextAlignment.CENTER);
        AddText(frame, FmtPower(cur[i]),
                new Vector2(colX + colW * 0.5f, valY),
                SMALL_TEXT_SCALE, C_TEXT, TextAlignment.CENTER);
    }
}

void DrawBatteryStrip(MySpriteDrawFrame frame, float x, float y, float w, float h,
                      int n, int chg, float stored, float cap, float curOut, float maxOut, float curIn)
{
    DrawStripFrame(frame, x, y, w, h, C_ACCENT);
    const float ROW_H = 16f;
    float topY    = y + 4;
    float barY    = y + ROW_H + 4;
    float bottomY = y + h - ROW_H;
    float barH    = Math.Max(8, bottomY - barY - 4);
    if (barH > ROW_H + 4) barH = ROW_H + 4;

    AddText(frame, "BATTERY ARRAY", new Vector2(x + 12, topY),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);
    AddText(frame, n + " CELLS  " + chg + " CHG",
            new Vector2(x + w - 8, topY), BASE_TEXT_SCALE, C_PRIMARY, TextAlignment.RIGHT);

    float frac = cap > 0 ? stored / cap : 0f;
    AddBar(frame, new Vector2(x + 12, barY), new Vector2(w - 24, barH), frac, C_ACCENT, C_DIM);

    float net = curOut - curIn;
    string netStr = (net >= 0 ? "DRAIN " : "CHRG ") + FmtPower(Math.Abs(net));
    Color netC = net >= 0 ? C_HOT : C_PRIMARY;

    AddText(frame, FmtPower(stored) + "h/" + FmtPower(cap) + "h " + (frac * 100f).ToString("0.0") + "%",
            new Vector2(x + 12, bottomY), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.LEFT);
    AddText(frame, netStr,
            new Vector2(x + w - 8, bottomY), SMALL_TEXT_SCALE, netC, TextAlignment.RIGHT);
}

void DrawDoctrineStrip(MySpriteDrawFrame frame, float x, float y, float w, float h,
                       Dictionary<Category, float> cats, float total)
{
    DrawStripFrame(frame, x, y, w, h, C_FRAME);
    const float ROW_H = 14f;
    float topY = y + 4;

    AddText(frame, "CONSUMPTION DOCTRINE", new Vector2(x + 12, topY),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);
    AddText(frame, "TOTAL " + FmtPower(total),
            new Vector2(x + w - 8, topY), SMALL_TEXT_SCALE, C_PRIMARY, TextAlignment.RIGHT);

    string[] labels = new string[] { "LIFE", "INDUSTRY", "OPS", "MANEUV", "WEAPON", "COMFORT" };
    Category[] keys = new Category[]
    {
        Category.LifeSupport, Category.Industry, Category.Operations,
        Category.Maneuvering, Category.Weapons, Category.Comforts
    };
    Color[] cs = new Color[] { C_PRIMARY, C_ACCENT, C_FRAME, C_LABEL, C_HOT, C_TEXT };

    int n = labels.Length;
    // Fixed row spacing so right-side values don't stack on top of each other.
    // Header takes ROW_H + 6 px, then n rows of ROW_H each starting at rowY.
    float rowY = y + ROW_H + 8;

    float labelW = w * 0.20f;
    float barX = x + 12 + labelW;
    float numW = w * 0.30f;
    float barW = w - labelW - numW - 24;

    for (int i = 0; i < n; i++)
    {
        float ry = rowY + i * ROW_H;
        AddText(frame, labels[i], new Vector2(x + 12, ry),
                SMALL_TEXT_SCALE, cs[i], TextAlignment.LEFT);
        float draw = cats[keys[i]];
        float frac = total > 0 ? draw / total : 0f;
        float subBarH = ROW_H - 4;
        AddBar(frame, new Vector2(barX, ry + 2),
               new Vector2(barW, subBarH), frac, cs[i], C_DIM, false);
        AddText(frame, FmtPower(draw) + "  " + (frac * 100f).ToString("0.0") + "%",
                new Vector2(x + w - 8, ry),
                SMALL_TEXT_SCALE, C_TEXT, TextAlignment.RIGHT);
    }
}

void DrawTacticalStrip(MySpriteDrawFrame frame, float x, float y, float w, float h,
                       float draw, float max, float genCur, float stored, float cap)
{
    DrawStripFrame(frame, x, y, w, h, C_HOT);
    const float ROW_H = 16f;
    float topY    = y + 4;
    float row1Y   = y + ROW_H + 6;
    float row2Y   = y + h - ROW_H;

    AddText(frame, "TACTICAL READOUT", new Vector2(x + 12, topY),
            BASE_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);

    string cond = ComputeDEFCON(draw, max, stored, cap);
    Color condC = cond.Contains("DEFCON 1") ? C_HOT
                : cond.Contains("DEFCON 2") ? C_ACCENT
                : cond.Contains("DEFCON 3") ? C_FRAME
                : cond.Contains("DEFCON 4") ? C_PRIMARY
                : C_LABEL;
    AddText(frame, cond, new Vector2(x + w - 8, topY),
            BASE_TEXT_SCALE, condC, TextAlignment.RIGHT);

    AddText(frame, "ENDURANCE " + EstimateEndurance(stored, draw - genCur),
            new Vector2(x + 12, row1Y), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.LEFT);
    AddText(frame, "INTEGRITY " + (97 + (_tick % 3)) + ".4%",
            new Vector2(x + w - 8, row1Y), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.RIGHT);

    AddText(frame, "HEADROOM", new Vector2(x + 12, row2Y),
            SMALL_TEXT_SCALE, C_LABEL, TextAlignment.LEFT);
    float head = max > 0 ? (max - draw) / max : 0f;
    float barH = ROW_H - 4;
    AddBar(frame, new Vector2(x + 12 + w * 0.22f, row2Y + 2),
           new Vector2(w * 0.48f, barH), head, C_PRIMARY, C_DIM);
    AddText(frame, (head * 100f).ToString("0.0") + "%",
            new Vector2(x + w - 8, row2Y), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.RIGHT);
}

void DrawStatusStrip(MySpriteDrawFrame frame, float x, float y, float w, float h)
{
    AddBox(frame, new Vector2(x, y), new Vector2(w, h), C_DIM);
    AddText(frame, "EMI " + ((_tick % 2 == 0) ? "ENGAGED" : "[ENGAGED]"),
            new Vector2(x + 12, y + h * 0.20f), SMALL_TEXT_SCALE, C_PRIMARY, TextAlignment.LEFT);
    AddText(frame, "AUTH 0x" + FmtHex(_tick * 65537).Substring(0, 4),
            new Vector2(x + w - 8, y + h * 0.20f), SMALL_TEXT_SCALE, C_TEXT, TextAlignment.RIGHT);
}

//  CATEGORY CLASSIFICATION

enum Category { LifeSupport, Industry, Operations, Maneuvering, Weapons, Comforts }

Dictionary<Category, float> ClassifyLoads()
{
    var dict = new Dictionary<Category, float>
    {
        { Category.LifeSupport, 0f }, { Category.Industry, 0f }, { Category.Operations, 0f },
        { Category.Maneuvering, 0f }, { Category.Weapons, 0f }, { Category.Comforts, 0f },
    };
    foreach (var b in _allBlocks)
    {
        if (b is IMyPowerProducer) continue;
        if (b is IMyBatteryBlock)  continue;
        float draw = GetResourceSinkInput(b);
        if (draw <= 0f) continue;
        dict[ClassifyBlock(b)] += draw;
    }
    return dict;
}

float GetResourceSinkInput(IMyTerminalBlock b)
{
    var func = b as IMyFunctionalBlock;
    if (func == null || !func.Enabled) return 0f;
    var refn  = b as IMyRefinery;     if (refn != null)  return GetCurrentInputMW(refn);
    var asm   = b as IMyAssembler;    if (asm != null)   return GetCurrentInputMW(asm);
    var thr   = b as IMyThrust;       if (thr != null)   return thr.CurrentThrust / Math.Max(1f, thr.MaxThrust) * EstimateThrusterMW(thr);
    var gyr   = b as IMyGyro;         if (gyr != null)   return gyr.Enabled ? 0.002f : 0f;
    var lit   = b as IMyLightingBlock;if (lit != null)   return lit.Enabled ? 0.0001f : 0f;
    var rad   = b as IMyRadioAntenna; if (rad != null)   return rad.Enabled ? (rad.Radius / 50000f) * 0.0005f : 0f;
    var bea   = b as IMyBeacon;       if (bea != null)   return bea.Enabled ? (bea.Radius / 50000f) * 0.0005f : 0f;
    var sen   = b as IMySensorBlock;  if (sen != null)   return sen.Enabled ? 0.0003f : 0f;
    var cam   = b as IMyCameraBlock;  if (cam != null)   return cam.Enabled ? 0.0001f : 0f;
    var dril  = b as IMyShipDrill;    if (dril != null)  return dril.Enabled ? 0.003f : 0f;
    var grind = b as IMyShipGrinder;  if (grind != null) return grind.Enabled ? 0.002f : 0f;
    var weld  = b as IMyShipWelder;   if (weld != null)  return weld.Enabled ? 0.002f : 0f;
    var oxy   = b as IMyGasGenerator; if (oxy != null)   return oxy.Enabled ? 0.005f : 0f;
    var ven   = b as IMyAirVent;      if (ven != null)   return ven.Enabled ? 0.0001f : 0f;
    var med   = b as IMyMedicalRoom;  if (med != null)   return med.Enabled ? 0.0008f : 0f;
    var prj   = b as IMyProjector;    if (prj != null)   return prj.Enabled ? 0.001f : 0f;
    var dr    = b as IMyMotorAdvancedStator; if (dr != null) return dr.Enabled ? 0.002f : 0f;
    return 0f;
}

float EstimateThrusterMW(IMyThrust t)
{
    string sub = (t.BlockDefinition.SubtypeId ?? "").ToUpperInvariant();
    if (sub.Contains("ION")) return 0.34f;
    if (sub.Contains("ATMO")) return 0.50f;
    return 0.0f;
}

Category ClassifyBlock(IMyTerminalBlock b)
{
    if (b is IMyAirVent || b is IMyGasGenerator || b is IMyMedicalRoom || b is IMyGasTank) return Category.LifeSupport;
    if (b is IMyRefinery || b is IMyAssembler || b is IMyShipDrill || b is IMyShipGrinder
        || b is IMyShipWelder || b is IMyProjector) return Category.Industry;
    if (b is IMyRadioAntenna || b is IMyBeacon || b is IMySensorBlock || b is IMyCameraBlock
        || b is IMyRemoteControl || b is IMyTextSurfaceProvider || b is IMyProgrammableBlock) return Category.Operations;
    if (b is IMyThrust || b is IMyGyro || b is IMyMotorStator) return Category.Maneuvering;
    if (b is IMyUserControllableGun || b is IMyLargeTurretBase || b is IMyWarhead) return Category.Weapons;
    return Category.Comforts;
}

//  HELPERS

// Build (or retrieve cached) parse of a block's DetailedInfo for this tick.
// We do all the parsing in one pass since DetailedInfo.Split('\n') is the
// expensive operation and we'd rather pay it once per block per tick than
// 5+ times.
BlockInfoCache GetBlockInfo(IMyTerminalBlock b)
{
    BlockInfoCache c;
    if (_infoCache.TryGetValue(b.EntityId, out c) && c.Loaded) return c;

    c = new BlockInfoCache { Loaded = true };
    string info = b.DetailedInfo ?? "";
    if (string.IsNullOrEmpty(info))
    {
        _infoCache[b.EntityId] = c;
        return c;
    }

    // Single pass: walk lines once, populate all fields we know about
    var lines = info.Split('\n');
    for (int li = 0; li < lines.Length; li++)
    {
        string line = lines[li].TrimStart();
        int colon = line.IndexOf(':');
        if (colon <= 0) continue;
        string key = line.Substring(0, colon).Trim();
        string val = line.Substring(colon + 1).Trim();

        // Power values: "<num> <unit>"
        if (key.Equals("Required Input", StringComparison.OrdinalIgnoreCase)
         || key.Equals("Current Input",  StringComparison.OrdinalIgnoreCase))
            c.CurrentInput = ParsePowerValue(val);
        else if (key.Equals("Max Required Input", StringComparison.OrdinalIgnoreCase)
              || key.Equals("Max Input", StringComparison.OrdinalIgnoreCase))
            c.MaxInput = ParsePowerValue(val);

        // Percent values
        else if (key.Equals("Refine Speed", StringComparison.OrdinalIgnoreCase)
              || key.Equals("Productivity", StringComparison.OrdinalIgnoreCase))
            c.SpeedFactor = ParsePercentValue(val);
        else if (key.Equals("Yield Rate", StringComparison.OrdinalIgnoreCase)
              || key.Equals("Effectiveness", StringComparison.OrdinalIgnoreCase))
            c.YieldFactor = ParsePercentValue(val);
        else if (key.Equals("Power Efficiency", StringComparison.OrdinalIgnoreCase))
            c.PowerEffFactor = ParsePercentValue(val);

        // Slot values "N / M"
        else if (key.Equals("Used upgrade module slots", StringComparison.OrdinalIgnoreCase))
        {
            int slash = val.IndexOf('/');
            if (slash > 0)
            {
                int.TryParse(val.Substring(0, slash).Trim(), out c.ModUsed);
                int.TryParse(val.Substring(slash + 1).Trim(), out c.ModTotal);
            }
        }
    }
    // Sensible defaults for percent multipliers when DetailedInfo lacks them
    if (c.SpeedFactor == 0f)    c.SpeedFactor = 1f;
    if (c.YieldFactor == 0f)    c.YieldFactor = 1f;
    if (c.PowerEffFactor == 0f) c.PowerEffFactor = 1f;

    _infoCache[b.EntityId] = c;
    return c;
}

float ParsePowerValue(string val)
{
    int spc = val.IndexOf(' ');
    if (spc < 0) return 0f;
    string numStr = val.Substring(0, spc).Trim();
    string unit  = val.Substring(spc + 1).Trim().ToUpperInvariant();
    float num;
    if (!float.TryParse(numStr, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out num))
        return 0f;
    if (unit.StartsWith("GW")) return num * 1000f;
    if (unit.StartsWith("MW")) return num;
    if (unit.StartsWith("KW")) return num / 1000f;
    if (unit.StartsWith("W"))  return num / 1000000f;
    return num;
}

float ParsePercentValue(string val)
{
    string clean = val.TrimEnd('%', ' ').Trim();
    float num;
    if (float.TryParse(clean, System.Globalization.NumberStyles.Float,
                       System.Globalization.CultureInfo.InvariantCulture, out num))
        return num / 100f;
    return 1f;
}

// Legacy single-key parser still used by the debug screen for ad-hoc lookups.
float ParsePowerFromDetailedInfo(string info, string label)
{
    if (string.IsNullOrEmpty(info)) return 0f;

    // Match the label at the start of a line (after trimming leading whitespace).
    // This avoids "Required Input" accidentally matching inside "Max Required Input".
    var lines = info.Split('\n');
    string trimmedLabel = label.Trim();
    for (int li = 0; li < lines.Length; li++)
    {
        string line = lines[li].TrimStart();
        if (!line.StartsWith(trimmedLabel, StringComparison.OrdinalIgnoreCase)) continue;
        // The character right after the label must be a colon (allowing for
        // optional spaces). This eliminates partial-word matches.
        int afterLabel = trimmedLabel.Length;
        // Skip spaces between label and colon
        while (afterLabel < line.Length && line[afterLabel] == ' ') afterLabel++;
        if (afterLabel >= line.Length || line[afterLabel] != ':') continue;
        // Parse "<num> <unit>" after the colon
        string val = line.Substring(afterLabel + 1).Trim();
        int spc = val.IndexOf(' ');
        if (spc < 0) return 0f;
        string numStr = val.Substring(0, spc).Trim();
        string unit  = val.Substring(spc + 1).Trim().ToUpperInvariant();
        float num;
        if (!float.TryParse(numStr, System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out num))
            return 0f;
        if (unit.StartsWith("GW")) return num * 1000f;
        if (unit.StartsWith("MW")) return num;
        if (unit.StartsWith("KW")) return num / 1000f;
        if (unit.StartsWith("W"))  return num / 1000000f;
        return num;
    }
    return 0f;
}

float GetCurrentInputMW(IMyTerminalBlock b) { return GetBlockInfo(b).CurrentInput; }
float GetMaxInputMW(IMyTerminalBlock b)     { return GetBlockInfo(b).MaxInput; }
float GetSpeedFactor(IMyTerminalBlock b)    { return GetBlockInfo(b).SpeedFactor; }
float GetYieldFactor(IMyTerminalBlock b)    { return GetBlockInfo(b).YieldFactor; }
float GetPowerEffFactor(IMyTerminalBlock b) { return GetBlockInfo(b).PowerEffFactor; }
void GetModuleSlots(IMyTerminalBlock b, out int used, out int total)
{
    var c = GetBlockInfo(b);
    used = c.ModUsed; total = c.ModTotal;
}

// Production block status. Computed from a combination of Enabled, IsWorking,
// IsFunctional, IsQueueEmpty, and IsProducing.
enum ProdStatus { Producing, Idle, Starved, NoPower, Damaged, Offline }

ProdStatus GetProdStatus(IMyProductionBlock p)
{
    if (!p.IsFunctional) return ProdStatus.Damaged;
    if (!p.Enabled)      return ProdStatus.Offline;
    if (!p.IsWorking)    return ProdStatus.NoPower;
    if (p.IsProducing)   return ProdStatus.Producing;
    if (p.IsQueueEmpty)  return ProdStatus.Idle;
    // Enabled, working, has a queue, but not producing => starved for inputs
    return ProdStatus.Starved;
}

string ProdStatusLabel(ProdStatus s)
{
    switch (s)
    {
        case ProdStatus.Producing: return "RUN";
        case ProdStatus.Idle:      return "IDLE";
        case ProdStatus.Starved:   return "STARVED";
        case ProdStatus.NoPower:   return "NO PWR";
        case ProdStatus.Damaged:   return "DMG";
        case ProdStatus.Offline:   return "OFF";
    }
    return "?";
}

Color ProdStatusColor(ProdStatus s)
{
    switch (s)
    {
        case ProdStatus.Producing: return C_PRIMARY;
        case ProdStatus.Idle:      return C_LABEL;
        case ProdStatus.Starved:   return C_HOT;
        case ProdStatus.NoPower:   return C_HOT;
        case ProdStatus.Damaged:   return C_HOT;
        case ProdStatus.Offline:   return C_DIM;
    }
    return C_TEXT;
}

long CountInventoryItems(IMyTerminalBlock b)
{
    long total = 0;
    for (int i = 0; i < b.InventoryCount; i++)
    {
        var inv = b.GetInventory(i);
        if (inv == null) continue;
        var items = new List<MyInventoryItem>();
        inv.GetItems(items);
        foreach (var it in items) total += it.Amount.ToIntSafe();
    }
    return total;
}

string FmtPower(float mw)
{
    if (mw >= 1000f)  return (mw / 1000f).ToString("0.0") + "GW";
    if (mw >= 1f)     return mw.ToString("0.00") + "MW";
    if (mw >= 0.001f) return (mw * 1000f).ToString("0") + "kW";
    return (mw * 1000000f).ToString("0") + "W";
}

string FmtUptime(int t)
{
    int sec = (t * 16) / 10;
    int h = sec / 3600;
    int m = (sec % 3600) / 60;
    int s = sec % 60;
    return h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00");
}

string FmtHex(int v) { return (v & 0x7FFFFFFF).ToString("X8"); }

string ComputeDEFCON(float draw, float max, float stored, float cap)
{
    float headroom = max > 0 ? (max - draw) / max : 0f;
    float reserve  = cap > 0 ? stored / cap : 0f;
    if (headroom < 0.05f || reserve < 0.10f) return "DEFCON 1";
    if (headroom < 0.15f || reserve < 0.25f) return "DEFCON 2";
    if (headroom < 0.30f || reserve < 0.50f) return "DEFCON 3";
    if (headroom < 0.50f)                    return "DEFCON 4";
    return "DEFCON 5";
}

string EstimateEndurance(float stored, float netDrain)
{
    if (netDrain <= 0f) return "INDEFINITE";
    float hours = stored / netDrain;
    if (hours > 999f) return ">999h";
    int h = (int)hours;
    int m = (int)((hours - h) * 60);
    return h.ToString("000") + "h" + m.ToString("00") + "m";
}

enum TagType { Refinery, Assembler, Power, Setup, Debug, Cal }
struct DisplayTarget { public IMyTerminalBlock Block; public int SurfaceIndex; public TagType Tag; }