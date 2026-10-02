// ============================================================
//  Tactical Azimuth & Navigation Kit (TANK-HUD)
//  Space Engineers Programmable Block - v1.0
// ============================================================
//
//  SETUP:
//    1. Tag your blocks (in each block's Custom Data):
//       Cockpit / Seat        ->  [TankHUD:Cockpit]
//       LCD panel             ->  [TankHUD:LCD]
//       Azimuth rotor/hinge   ->  [TankHUD:Azimuth]  or  [TankHUD:Azimuth:45]
//       Elevation rotor/hinge ->  [TankHUD:Elevation] or  [TankHUD:Elevation:-90]
//       Aux rotor (optional)  ->  [TankHUD:AuxAzimuth]
//       Auto turret (optional)->  [TankHUD:AutoTurret]
//       Sound block (optional)->  [TankHUD:Speaker]
//
//    2. Compile and run the PB. Any LCD tagged [TankHUD:LCD] that
//       hasn't been configured yet will automatically receive a
//       full config block with all options and default values.
//
//    3. Edit the config values in each LCD's Custom Data as needed.
//       Run RELOAD when done.
//
//  CONFIG FORMAT (auto-written to each LCD):
//    [TankHUD:Key:Value]  - Description. Options.
//
//    [TankHUD:LCD:0]                        Display index. 0,1,2,3 etc.
//    [TankHUD:Azimuth:True]                 Show azimuth panel. True, False.
//    [TankHUD:Elevation:True]               Show elevation panel. True, False.
//    [TankHUD:Pitch:True]                   Show pitch panel. True, False.
//    [TankHUD:Roll:True]                    Show roll panel. True, False.
//    [TankHUD:Layout:None]                  None, Row, RowTop, RowBottom, Column, ColumnLeft, ColumnRight.
//    [TankHUD:Align:None]                   None, Center, CenterTop, CenterBottom, Top, Bottom, Stretch.
//    [TankHUD:Scaling:False]                True, False.
//    [TankHUD:Scale:1.0]                    0.1 to 1.0.
//    [TankHUD:Alpha:1.0]                    0.0 to 1.0.
//    [TankHUD:Mirror:False]                 True, False.
//    [TankHUD:HideCrosshair:False]          True, False.
//    [TankHUD:InvertElevation:False]        True, False.
//    [TankHUD:InvertAzimuth:False]          True, False.
//    [TankHUD:InvertElevationReadout:False] True, False.
//    [TankHUD:InvertAzimuthReadout:False]   True, False.
//    [TankHUD:InvertAux:False]              Flip aux rotor arrow direction. True, False.
//    [TankHUD:InvertAutoTurret:False]       Flip auto turret needle direction. True, False.
//    [TankHUD:InvertPitch:False]            Flip pitch readout sign only. True, False.
//    [TankHUD:InvertRoll:False]             Flip roll readout sign only. True, False.
//    [TankHUD:PitchOffset:0]               Add degrees to pitch graphic only. e.g. 90 or -90.
//    [TankHUD:RollOffset:0]                Add degrees to roll graphic only. e.g. 90 or -90.
//    [TankHUD:ElevationRotate:Left]        Rotate elevation graphic. Left, Right, Up, Down.
//    [TankHUD:Rotate:Down]                 Rotate entire display. Down, Left, Right, Up.
//
//  SPEAKER / WARNING CONFIG (auto-written to Speaker Custom Data):
//    Format: [TankHUD:Key:Active:FlashHUD:Sound:Angle+:Angle-:Interval]
//    When Active=True and a speaker is tagged, warning icons ghost at 15% opacity
//    while within limits, then snap full brightness and flash when triggered.
//
//    [TankHUD:AuxWarning:False:True:True:1:-1:1]
//    [TankHUD:AutoWarning:True:True:True:1:-1:1]
//    [TankHUD:MainWarning:False:True:True:1:-1:1]
//    [TankHUD:PitchWarning:False:True:True:45:-45:5]
//    [TankHUD:RollWarning:False:True:True:45:-45:5]
//
//  PB CUSTOM DATA:
//    [TankHUD:InvertElevation:True]  -- global elevation graphic invert
//
//  Runtime argument:
//    RELOAD  -- re-init new LCDs, reload config, rescan surfaces
// ============================================================

const UpdateFrequency TICK_RATE = UpdateFrequency.Update10;

// Tag prefixes
const string TAG_LCD       = "[TankHUD:LCD";
const string TAG_COCKPIT   = "[TankHUD:Cockpit]";
const string TAG_AZIMUTH   = "[TankHUD:Azimuth";
const string TAG_ELEVATION = "[TankHUD:Elevation";
const string TAG_AUX       = "[TankHUD:AuxAzimuth";
const string TAG_TURRET    = "[TankHUD:AutoTurret";
const string TAG_SPEAKER   = "[TankHUD:Speaker]";

// ----------------------------------------------------------------
//  Read "[TankHUD:KEY:VALUE] ..." -> returns VALUE string, or
//  defaultVal if tag absent. Ignores trailing description text.
// ----------------------------------------------------------------
static string TagValue(string cd, string key, string defaultVal = "")
{
    string search = "[TankHUD:" + key + ":";
    int start = cd.IndexOf(search);
    if (start < 0) return defaultVal;
    int valStart = start + search.Length;
    int end = cd.IndexOf(']', valStart);
    if (end < 0) return defaultVal;
    return cd.Substring(valStart, end - valStart).Trim();
}

static bool TagBool(string cd, string key, bool defaultVal = false)
{
    string v = TagValue(cd, key, defaultVal ? "True" : "False");
    return v.Equals("True", StringComparison.OrdinalIgnoreCase);
}

static float TagFloat(string cd, string key, float defaultVal = 0f)
{
    string v = TagValue(cd, key);
    float result;
    return float.TryParse(v, out result) ? result : defaultVal;
}

// ----------------------------------------------------------------
//  Warning configuration parsed from multi-value tag
//  Format: [TankHUD:KEY:Active:FlashHUD:Sound:Angle+:Angle-:Interval]
// ----------------------------------------------------------------
struct WarningConfig
{
    public bool  Active;
    public bool  FlashHUD;
    public bool  Sound;
    public float AnglePlus;
    public float AngleMinus;
    public float Interval;

    public WarningConfig(bool active, bool flash, bool sound,
                         float plus, float minus, float interval)
    {
        Active     = active;
        FlashHUD   = flash;
        Sound      = sound;
        AnglePlus  = MathHelper.ToRadians(plus);
        AngleMinus = MathHelper.ToRadians(minus);
        Interval   = interval;
    }
}

static WarningConfig ParseWarning(string cd, string key,
    bool defActive, float defPlus, float defMinus, float defInterval)
{
    string search = "[TankHUD:" + key + ":";
    int start = cd.IndexOf(search);
    if (start < 0)
        return new WarningConfig(defActive, true, true, defPlus, defMinus, defInterval);
    int valStart = start + search.Length;
    int end = cd.IndexOf(']', valStart);
    if (end < 0)
        return new WarningConfig(defActive, true, true, defPlus, defMinus, defInterval);
    string[] parts = cd.Substring(valStart, end - valStart).Split(':');
    bool   active   = parts.Length > 0 && parts[0].Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
    bool   flash    = parts.Length > 1 && parts[1].Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
    bool   sound    = parts.Length > 2 && parts[2].Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
    float  plus     = defPlus;
    float  minus    = defMinus;
    float  interval = defInterval;
    if (parts.Length > 3) float.TryParse(parts[3].Trim(), out plus);
    if (parts.Length > 4) float.TryParse(parts[4].Trim(), out minus);
    if (parts.Length > 5) float.TryParse(parts[5].Trim(), out interval);
    return new WarningConfig(active, flash, sound, plus, minus, interval);
}

// Per-display visibility flags bundled with its surface
struct SurfaceEntry
{
    public IMyTextSurface Surface;
    public bool  ShowAzimuth;
    public bool  ShowElevation;
    public bool  ShowPitch;
    public bool  ShowRoll;
    public bool  Mirror;
    public bool  MirrorGraphicsOnly;
    public float Scale;
    public bool  Scaling;
    public bool  AlignCenter;
    public bool  AlignTop;
    public bool  AlignBottom;
    public bool  AlignStretch;
    public string Layout;
    public bool  HideCrosshair;
    public bool  InvertElevation;
    public bool  InvertAzimuth;
    public bool  InvertElevationReadout;
    public bool  InvertAzimuthReadout;
    public bool  InvertAux;
    public bool  InvertAutoTurret;
    public bool  InvertPitch;
    public bool  InvertRoll;
    public float PitchOffset;
    public float RollOffset;
    public float ElevationRotate;
    public float Alpha;
    public float Rotate;

    public SurfaceEntry(IMyTextSurface s, string cd)
    {
        Surface       = s;

        ShowAzimuth   = TagBool(cd, "Azimuth",   true);
        ShowElevation = TagBool(cd, "Elevation",  true);
        ShowPitch     = TagBool(cd, "Pitch",      true);
        ShowRoll      = TagBool(cd, "Roll",       true);

        Mirror             = TagBool(cd, "Mirror",      false);
        MirrorGraphicsOnly = cd.Contains("[TankHUD:MirrorGraphicsOnly]");

        string align = TagValue(cd, "Align", "None");
        AlignCenter  = align == "Center" || align == "CenterTop" || align == "CenterBottom";
        AlignTop     = align == "Top"    || align == "CenterTop";
        AlignBottom  = align == "Bottom" || align == "CenterBottom";
        AlignStretch = align == "Stretch";

        string layout = TagValue(cd, "Layout", "None");
        Layout = (layout == "None") ? "" : layout;

        Scaling                = TagBool(cd, "Scaling",                false);
        HideCrosshair          = TagBool(cd, "HideCrosshair",          false);
        InvertElevation        = TagBool(cd, "InvertElevation",        false);
        InvertAzimuth          = TagBool(cd, "InvertAzimuth",          false);
        InvertElevationReadout = TagBool(cd, "InvertElevationReadout", false);
        InvertAzimuthReadout   = TagBool(cd, "InvertAzimuthReadout",   false);
        InvertAux              = TagBool(cd, "InvertAux",              false);
        InvertAutoTurret       = TagBool(cd, "InvertAutoTurret",       false);
        InvertPitch            = TagBool(cd, "InvertPitch",            false);
        InvertRoll             = TagBool(cd, "InvertRoll",             false);

        PitchOffset = MathHelper.ToRadians(TagFloat(cd, "PitchOffset", 0f));
        RollOffset  = MathHelper.ToRadians(TagFloat(cd, "RollOffset",  0f));

        string elevRot = TagValue(cd, "ElevationRotate", "Left");
        ElevationRotate = elevRot == "Right"  ?  MathHelper.Pi :
                          elevRot == "Up"     ?  MathHelper.PiOver2 :
                          elevRot == "Down"   ? -MathHelper.PiOver2 : 0f;

        Scale = Math.Max(TagFloat(cd, "Scale", 1.0f), 0.1f);
        Alpha = MathHelper.Clamp(TagFloat(cd, "Alpha", 1.0f), 0.0f, 1.0f);

        string rot = TagValue(cd, "Rotate", "Down");
        Rotate = rot == "Right" ?  MathHelper.PiOver2 :
                 rot == "Up"    ?  MathHelper.Pi :
                 rot == "Left"  ? -MathHelper.PiOver2 : 0f;
    }
}

IMyShipController       _controller;
IMyMotorStator          _azimuth;
IMyMotorStator          _elevation;
IMyMotorStator          _aux;
IMyLargeTurretBase      _autoTurret;
List<IMySoundBlock>     _speakers  = new List<IMySoundBlock>();
List<SurfaceEntry>      _surfaces  = new List<SurfaceEntry>();

float _aziOffset;
float _elevOffset;
float _auxOffset;
float _turretOffset;
bool  _invertElevation;
float _mx     = float.NaN;
float _mxText = float.NaN;
float _alpha  = 1f;
float _rot    = 0f;
float _vcx    = 0f;
float _vcy    = 0f;

// Warning flash timers (seconds since last trigger per warning type)
float _timerAux    = 0f;
float _timerAT     = 0f;
float _timerMain   = 0f;
float _timerPitch  = 0f;
float _timerRoll   = 0f;
bool  _flashAux    = false;
bool  _flashAT     = false;
bool  _flashMain   = false;
bool  _flashPitch  = false;
bool  _flashRoll   = false;
bool  _activeAux   = false;
bool  _activeAT    = false;
bool  _activeMain  = false;
bool  _activePitch = false;
bool  _activeRoll  = false;

WarningConfig _warnAux        = new WarningConfig(false, true, true, 1f,  -1f,  1f);
WarningConfig _warnAutoTurret = new WarningConfig(true,  true, true, 1f,  -1f,  1f);
WarningConfig _warnMain       = new WarningConfig(false, true, true, 1f,  -1f,  1f);
WarningConfig _warnPitch      = new WarningConfig(false, true, true, 45f, -45f, 5f);
WarningConfig _warnRoll       = new WarningConfig(false, true, true, 45f, -45f, 5f);

// ----------------------------------------------------------------
//  Generic tag search
// ----------------------------------------------------------------
T FindByTag<T>(string tag) where T : class, IMyTerminalBlock
{
    var list = new List<T>();
    GridTerminalSystem.GetBlocksOfType<T>(list, b => b.CustomData.Contains(tag));
    return list.Count > 0 ? list[0] : null;
}

// ----------------------------------------------------------------
//  Parse "[TankHUD:KEY:VALUE]" -> VALUE as float (degrees), returns 0 if absent/malformed
// ----------------------------------------------------------------
float ParseTagOffset(string customData, string keyPrefix)
{
    int start = customData.IndexOf(keyPrefix);
    if (start < 0) return 0f;
    int colon = customData.IndexOf(':', start + keyPrefix.Length);
    if (colon < 0) return 0f;
    int end = customData.IndexOf(']', colon);
    if (end < 0) return 0f;
    float val;
    return float.TryParse(customData.Substring(colon + 1, end - colon - 1).Trim(), out val) ? val : 0f;
}

// ----------------------------------------------------------------
//  Parse screen index from "[TankHUD:LCD:N]"
// ----------------------------------------------------------------
int ParseLCDIndex(string customData)
{
    string v = TagValue(customData, "LCD", "0");
    int idx;
    return int.TryParse(v, out idx) ? idx : 0;
}

// ----------------------------------------------------------------
//  Find ALL tagged LCD surfaces, each paired with its hide flags
// ----------------------------------------------------------------
List<SurfaceEntry> FindAllLCDSurfaces()
{
    var result = new List<SurfaceEntry>();

    var panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, b => b.CustomData.Contains(TAG_LCD));
    foreach (var p in panels)
        result.Add(new SurfaceEntry(p as IMyTextSurface, p.CustomData));

    var cockpits = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(cockpits, b => b.CustomData.Contains(TAG_LCD));
    foreach (var c in cockpits)
    {
        if (c is IMyTextSurfaceProvider)
        {
            int idx = ParseLCDIndex(c.CustomData);
            result.Add(new SurfaceEntry(
                ((IMyTextSurfaceProvider)c).GetSurface(idx), c.CustomData));
        }
    }
    return result;
}

// ----------------------------------------------------------------
//  Default config block written to fresh [TankHUD:LCD] blocks
//  Sentinel: presence of [TankHUD:LCD: means already initialized
// ----------------------------------------------------------------
const string LCD_DEFAULTS =
    "[TankHUD:LCD:0]                        - Display index. 0, 1, 2, 3 etc.\n" +
    "\n" +
    "--- PANELS ---\n" +
    "[TankHUD:Azimuth:True]                 - Show azimuth panel. True, False.\n" +
    "[TankHUD:Elevation:True]               - Show elevation panel. True, False.\n" +
    "[TankHUD:Pitch:True]                   - Show pitch panel. True, False.\n" +
    "[TankHUD:Roll:True]                    - Show roll panel. True, False.\n" +
    "\n" +
    "--- LAYOUT ---\n" +
    "[TankHUD:Layout:None]                  - Strip layout. None, Row, RowTop, RowBottom, Column, ColumnLeft, ColumnRight.\n" +
    "[TankHUD:Align:None]                   - Anchor point. None, Center, CenterTop, CenterBottom, Top, Bottom, Stretch.\n" +
    "[TankHUD:Scaling:False]                - 2+1 config: solo panel grows at 50% rate of paired panels. True, False.\n" +
    "[TankHUD:Scale:1.0]                    - Panel scale. 0.1 minimum, suggested max 5.0.\n" +
    "[TankHUD:Alpha:1.0]                    - Brightness. 0.0 to 1.0.\n" +
    "[TankHUD:Rotate:Down]                  - Rotate entire display. Down, Left, Right, Up.\n" +
    "[TankHUD:Mirror:False]                 - Mirror all graphics. Use on transparent back-facing displays. True, False.\n" +
    "[TankHUD:HideCrosshair:False]          - Hide divider lines between panels. True, False.\n" +
    "\n" +
    "--- AZIMUTH ---\n" +
    "[TankHUD:InvertAzimuth:False]          - Flip azimuth graphic direction. True, False.\n" +
    "[TankHUD:InvertAzimuthReadout:False]   - Flip azimuth number sign only. True, False.\n" +
    "[TankHUD:InvertAux:False]              - Flip aux rotor arrow direction. True, False.\n" +
    "[TankHUD:InvertAutoTurret:False]       - Flip auto turret needle direction. True, False.\n" +
    "\n" +
    "--- ELEVATION ---\n" +
    "[TankHUD:InvertElevation:False]        - Flip elevation graphic direction. True, False.\n" +
    "[TankHUD:InvertElevationReadout:False] - Flip elevation number sign only. True, False.\n" +
    "[TankHUD:ElevationRotate:Left]         - Rotate elevation graphic. Left, Right, Up, Down.\n" +
    "\n" +
    "--- PITCH ---\n" +
    "[TankHUD:InvertPitch:False]            - Flip pitch readout sign only. True, False.\n" +
    "[TankHUD:PitchOffset:0]               - Add degrees to pitch graphic only. e.g. 90 or -90.\n" +
    "\n" +
    "--- ROLL ---\n" +
    "[TankHUD:InvertRoll:False]             - Flip roll readout sign only. True, False.\n" +
    "[TankHUD:RollOffset:0]                - Add degrees to roll graphic only. e.g. 90 or -90.\n";

// Auto-initializes any [TankHUD:LCD] block that hasn't been configured yet
void AutoInitLCDs()
{
    int written = 0;
    var panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, b => b.CustomData.Contains(TAG_LCD));
    foreach (var p in panels)
    {
        if (p.CustomData.Contains("[TankHUD:LCD:")) continue;
        string sep = p.CustomData.Length > 0 ? "\n" : "";
        if (p.CustomData.Contains("[TankHUD:LCD]"))
            p.CustomData = p.CustomData.TrimEnd() + sep + LCD_DEFAULTS;
        else
            p.CustomData = p.CustomData.TrimEnd() + sep + "[TankHUD:LCD]\n" + LCD_DEFAULTS;
        written++;
    }
    var cockpits = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(cockpits, b => b.CustomData.Contains(TAG_LCD));
    foreach (var c in cockpits)
    {
        if (c.CustomData.Contains("[TankHUD:LCD:")) continue;
        string sep = c.CustomData.Length > 0 ? "\n" : "";
        if (c.CustomData.Contains("[TankHUD:LCD]"))
            c.CustomData = c.CustomData.TrimEnd() + sep + LCD_DEFAULTS;
        else
            c.CustomData = c.CustomData.TrimEnd() + sep + "[TankHUD:LCD]\n" + LCD_DEFAULTS;
        written++;
    }
    if (written > 0) Echo($"[Init] Defaults written to {written} new display(s).");
}


void ReadConfig()
{
    _invertElevation = Me.CustomData.Contains("[TankHUD:InvertElevation:True]");
}

public Program()
{
    Runtime.UpdateFrequency = TICK_RATE;
    InitSegFont();

    _controller = FindByTag<IMyShipController>(TAG_COCKPIT);
    if (_controller == null) Echo($"[WARN] No block tagged '{TAG_COCKPIT}'");

    _azimuth = FindByTag<IMyMotorStator>(TAG_AZIMUTH);
    if (_azimuth != null)
        _aziOffset = MathHelper.ToRadians(ParseTagOffset(_azimuth.CustomData, "[TankHUD:Azimuth"));
    else
        Echo($"[WARN] No block tagged '{TAG_AZIMUTH}...'");

    _elevation = FindByTag<IMyMotorStator>(TAG_ELEVATION);
    if (_elevation != null)
        _elevOffset = MathHelper.ToRadians(ParseTagOffset(_elevation.CustomData, "[TankHUD:Elevation"));
    else
        Echo($"[WARN] No block tagged '{TAG_ELEVATION}...'");

    _aux = FindByTag<IMyMotorStator>(TAG_AUX);
    if (_aux != null)
        _auxOffset = MathHelper.ToRadians(ParseTagOffset(_aux.CustomData, "[TankHUD:AuxAzimuth"));

    _autoTurret = FindByTag<IMyLargeTurretBase>(TAG_TURRET);
    if (_autoTurret != null)
        _turretOffset = MathHelper.ToRadians(ParseTagOffset(_autoTurret.CustomData, "[TankHUD:AutoTurret"));

    GridTerminalSystem.GetBlocksOfType(_speakers, b => b.CustomData.Contains(TAG_SPEAKER));
    AutoInitSpeakers();

    ReadConfig();
    Echo($"[INFO] Azi offset: {MathHelper.ToDegrees(_aziOffset):F1}  Elev offset: {MathHelper.ToDegrees(_elevOffset):F1}");

    AutoInitLCDs();
    _surfaces = FindAllLCDSurfaces();
    if (_surfaces.Count > 0)
    {
        foreach (var e in _surfaces)
        {
            e.Surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
            e.Surface.Script      = "";
            e.Surface.ScriptBackgroundColor = Color.Black;
        }
        Echo($"[OK] {_surfaces.Count} display(s) ready.");
    }
    else Echo("[ERROR] No LCD surfaces found.");
}

public void Main(string argument, UpdateType updateSource)
{
    if (argument == "RELOAD") { ReadConfig(); AutoInitLCDs(); _surfaces = FindAllLCDSurfaces(); GridTerminalSystem.GetBlocksOfType(_speakers, b => b.CustomData.Contains(TAG_SPEAKER)); AutoInitSpeakers(); Echo("Reloaded."); return; }

    if (_surfaces.Count == 0 || _controller == null) return;

    // ---- Gravity orientation (pitch/roll only) ----
    Vector3D gravity = _controller.GetNaturalGravity();
    if (gravity.LengthSquared() < 0.001) gravity = _controller.GetArtificialGravity();
    if (gravity.LengthSquared() < 0.001) gravity = Vector3D.Down;
    gravity.Normalize();

    var wm = _controller.WorldMatrix;
    float pitch = (float)Math.Atan2(
        Vector3D.Dot(gravity, wm.Forward),
        Vector3D.Dot(gravity, -wm.Up));
    float roll = (float)Math.Atan2(
        Vector3D.Dot(gravity, wm.Right),
        Vector3D.Dot(gravity, -wm.Up));

    // ---- Rotor/hinge angles (relative to block, offset-corrected) ----
    float aziAngle    = (_azimuth    != null ? _azimuth.Angle    : 0f) + _aziOffset;
    float elevRaw     = (_elevation  != null ? _elevation.Angle  : 0f) + _elevOffset;
    float elevAngle   = _invertElevation ? -elevRaw : elevRaw;
    float auxAngle    = (_aux        != null) ? _aux.Angle    + _auxOffset    : float.NaN;
    float turretAngle = (_autoTurret != null) ? _autoTurret.Azimuth + _turretOffset : float.NaN;

    // ---- Compute display angles for warning eval (using first surface's settings) ----
    float warnAzi    = aziAngle;
    float warnAux    = auxAngle;
    float warnAT     = turretAngle;
    float warnPitch  = pitch;
    float warnRoll   = roll;
    if (_surfaces.Count > 0)
    {
        var s0 = _surfaces[0];
        if (s0.InvertAzimuth)                               warnAzi   = -warnAzi;
        if (s0.InvertAux   && !float.IsNaN(warnAux))        warnAux   = -warnAux;
        if (s0.InvertAutoTurret && !float.IsNaN(warnAT))    warnAT    = -warnAT;
        warnPitch += s0.PitchOffset;
        warnRoll  += s0.RollOffset;
    }

    // Normalize rotational angles from 0-2pi to -pi to pi for warning comparison
    if (warnAzi > Math.PI) warnAzi -= 2f * (float)Math.PI;
    if (!float.IsNaN(warnAux) && warnAux > Math.PI) warnAux -= 2f * (float)Math.PI;
    if (!float.IsNaN(warnAT)  && warnAT  > Math.PI) warnAT  -= 2f * (float)Math.PI;

    // ---- Warning evaluation -- only runs if speakers are present ----
    float dt = (float)Runtime.TimeSinceLastRun.TotalSeconds;
    if (_speakers.Count > 0)
    {
        EvalWarning(ref _flashAux,   ref _activeAux,   ref _timerAux,   _warnAux,        warnAux,   dt);
        EvalWarning(ref _flashAT,    ref _activeAT,    ref _timerAT,    _warnAutoTurret, warnAT,    dt);
        EvalWarning(ref _flashMain,  ref _activeMain,  ref _timerMain,  _warnMain,       warnAzi,   dt);
        EvalWarning(ref _flashPitch, ref _activePitch, ref _timerPitch, _warnPitch,      warnPitch, dt);
        EvalWarning(ref _flashRoll,  ref _activeRoll,  ref _timerRoll,  _warnRoll,       warnRoll,  dt);
    }
    else
    {
        _flashAux = _flashAT = _flashMain = _flashPitch = _flashRoll = false;
        _activeAux = _activeAT = _activeMain = _activePitch = _activeRoll = false;
    }

    // ---- Draw to every registered surface ----
    foreach (var entry in _surfaces)
    {
        Vector2 texSize  = entry.Surface.TextureSize;
        Vector2 surfSize = entry.Surface.SurfaceSize;
        Vector2 baseOff  = (texSize - surfSize) * 0.5f;

        // Viewport always covers the full LCD surface area
        float vW = surfSize.X, vH = surfSize.Y;
        float vX = baseOff.X,  vY = baseOff.Y;
        var vp = new RectangleF(new Vector2(vX, vY), new Vector2(vW, vH));

        bool doMirror = entry.Mirror || entry.MirrorGraphicsOnly;
        _mx     = doMirror ? (vp.X + vp.Width * 0.5f) : float.NaN;
        _mxText = entry.Mirror ? _mx : float.NaN;
        _alpha  = entry.Alpha;
        _rot    = entry.Rotate;
        _vcx    = vp.X + vp.Width  * 0.5f;
        _vcy    = vp.Y + vp.Height * 0.5f;

        using (var frame = entry.Surface.DrawFrame())
        {
            DrawRect(ref frame,
                new Vector2(texSize.X * 0.5f, texSize.Y * 0.5f),
                texSize, Color.Black);

            DrawLayout(ref frame, vp, aziAngle, elevAngle, pitch, roll, auxAngle, turretAngle,
                       entry.ShowAzimuth,   entry.ShowElevation,
                       entry.ShowPitch,     entry.ShowRoll,
                       entry.AlignCenter,   entry.AlignTop,    entry.AlignBottom,
                       entry.AlignStretch,  entry.Scale,       entry.Scaling,
                       entry.Layout,        entry.HideCrosshair,
                       entry.InvertElevation,        entry.InvertAzimuth,
                       entry.InvertElevationReadout, entry.InvertAzimuthReadout,
                       entry.InvertAux,              entry.InvertAutoTurret,
                       entry.PitchOffset,            entry.RollOffset,       entry.ElevationRotate,
                       _flashAux,  _flashAT,  _flashMain,  _flashPitch,  _flashRoll,
                       _activeAux, _activeAT, _activeMain, _activePitch, _activeRoll,
                       _warnAux.Active        && _speakers.Count > 0,
                       _warnAutoTurret.Active && _speakers.Count > 0,
                       _warnMain.Active       && _speakers.Count > 0,
                       _warnPitch.Active      && _speakers.Count > 0,
                       _warnRoll.Active       && _speakers.Count > 0,
                       entry.InvertPitch, entry.InvertRoll);
        }
    }
}

// ================================================================
//  DRAW LAYOUT
// ================================================================
void DrawLayout(ref MySpriteDrawFrame frame, RectangleF vp,
                float aziAngle, float elevAngle, float pitch, float roll, float auxAngle, float turretAngle,
                bool showAzimuth, bool showElevation, bool showPitch, bool showRoll,
                bool alignCenter, bool alignTop, bool alignBottom,
                bool stretch, float scale, bool scaling,
                string layout, bool hideCrosshair,
                bool invertElev, bool invertAzi,
                bool invertElevReadout, bool invertAziReadout,
                bool invertAux, bool invertAutoTurret,
                float pitchOffset, float rollOffset, float elevationRotate,
                bool flashAux, bool flashAT, bool flashMain, bool flashPitch, bool flashRoll,
                bool activeAux, bool activeAT, bool activeMain, bool activePitch, bool activeRoll,
                bool armedAux, bool armedAT, bool armedMain, bool armedPitch, bool armedRoll,
                bool invertPitch = false, bool invertRoll = false)
{
    // Apply per-display graphic inversions
    if (invertElev) elevAngle   = -elevAngle;
    if (invertAzi)  aziAngle    = -aziAngle;
    if (invertAux  && !float.IsNaN(auxAngle))         auxAngle    = -auxAngle;
    if (invertAutoTurret && !float.IsNaN(turretAngle)) turretAngle = -turretAngle;

    int topCount = (showAzimuth ? 1 : 0) + (showElevation ? 1 : 0);
    int botCount = (showPitch   ? 1 : 0) + (showRoll      ? 1 : 0);
    int total    = topCount + botCount;
    if (total == 0) return;

    float w  = vp.Width,  h  = vp.Height;
    float hw = w * 0.5f,  hh = h * 0.5f;

    // ---- ROW LAYOUT ----
    if (layout == "Row" || layout == "RowTop" || layout == "RowBottom")
    {
        // Square cell size: w/n capped at h/2
        float cellSz = Math.Min(w / total, hh) * scale;
        float totalW = cellSz * total;

        // Vertical anchor
        float rowY;
        if      (layout == "RowTop")    rowY = vp.Y;
        else if (layout == "RowBottom") rowY = vp.Y + h - cellSz;
        else                            rowY = vp.Y + (h - cellSz) * 0.5f;

        // Horizontal start -- center the strip
        float startX = vp.X + (w - totalW) * 0.5f;

        // Dividers between cells
        if (!hideCrosshair)
        {
            for (int i = 1; i < total; i++)
                DrawLine(ref frame,
                    new Vector2(startX + cellSz * i, rowY),
                    new Vector2(startX + cellSz * i, rowY + cellSz),
                    new Color(40,40,40), 2f);
        }

        float xPos = startX;
        bool[] show = { showAzimuth, showElevation, showPitch, showRoll };
        for (int i = 0; i < 4; i++)
        {
            if (!show[i]) continue;
            var cell = new RectangleF(xPos, rowY, cellSz, cellSz);
            switch (i)
            {
                case 0: DrawAzimuthPanel(ref frame, cell, aziAngle, "Azm", auxAngle, invertAziReadout, turretAngle, flashAux && _warnAux.FlashHUD, flashAT && _warnAutoTurret.FlashHUD, flashMain && _warnMain.FlashHUD, activeAux, activeAT, activeMain, armedAux, armedAT, armedMain); break;
                case 1: DrawElevationPanel(ref frame, cell, elevAngle, "Elev", invertElevReadout, elevationRotate); break;
                case 2: DrawOrientationPanel(ref frame, cell, pitch, "PITCH", flashPitch && _warnPitch.FlashHUD, activePitch, armedPitch, pitchOffset, invertPitch); break;
                case 3: DrawOrientationPanel(ref frame, cell, roll, "ROLL", flashRoll && _warnRoll.FlashHUD, activeRoll, armedRoll, rollOffset, invertRoll); break;
            }
            xPos += cellSz;
        }
        return;
    }

    // ---- COLUMN LAYOUT ----
    if (layout == "Column" || layout == "ColumnLeft" || layout == "ColumnRight")
    {
        // Square cell size: h/n capped at w/2
        float cellSz = Math.Min(h / total, hw) * scale;
        float totalH = cellSz * total;

        // Horizontal anchor
        float colX;
        if      (layout == "ColumnLeft")  colX = vp.X;
        else if (layout == "ColumnRight") colX = vp.X + w - cellSz;
        else                              colX = vp.X + (w - cellSz) * 0.5f;

        float startY = vp.Y + (h - totalH) * 0.5f;

        if (!hideCrosshair)
        {
            for (int i = 1; i < total; i++)
                DrawLine(ref frame,
                    new Vector2(colX, startY + cellSz * i),
                    new Vector2(colX + cellSz, startY + cellSz * i),
                    new Color(40,40,40), 2f);
        }

        float yPos = startY;
        bool[] show2 = { showAzimuth, showElevation, showPitch, showRoll };
        for (int i = 0; i < 4; i++)
        {
            if (!show2[i]) continue;
            var cell = new RectangleF(colX, yPos, cellSz, cellSz);
            switch (i)
            {
                case 0: DrawAzimuthPanel(ref frame, cell, aziAngle, "Azm", auxAngle, invertAziReadout, turretAngle, flashAux && _warnAux.FlashHUD, flashAT && _warnAutoTurret.FlashHUD, flashMain && _warnMain.FlashHUD, activeAux, activeAT, activeMain, armedAux, armedAT, armedMain); break;
                case 1: DrawElevationPanel(ref frame, cell, elevAngle, "Elev", invertElevReadout, elevationRotate); break;
                case 2: DrawOrientationPanel(ref frame, cell, pitch, "PITCH", flashPitch && _warnPitch.FlashHUD, activePitch, armedPitch, pitchOffset, invertPitch); break;
                case 3: DrawOrientationPanel(ref frame, cell, roll, "ROLL", flashRoll && _warnRoll.FlashHUD, activeRoll, armedRoll, rollOffset, invertRoll); break;
            }
            yPos += cellSz;
        }
        return;
    }

    // ---- GRID LAYOUT (default) ----
    bool is21 = (total == 3);

    float topY = vp.Y;
    float botY = (topCount > 0) ? vp.Y + hh : vp.Y;
    float topH = (botCount > 0) ? hh : h;
    float botH = (topCount > 0) ? hh : h;

    if (!hideCrosshair)
    {
        bool stretchCol2top = stretch && topCount == 2 && botCount == 0;
        bool stretchCol2bot = stretch && botCount == 2 && topCount == 0;
        if (topCount > 0 && botCount > 0)
            DrawLine(ref frame,
                new Vector2(vp.X, vp.Y + hh), new Vector2(vp.X + w, vp.Y + hh),
                new Color(40,40,40), 2f);
        if (topCount == 2)
            DrawLine(ref frame,
                new Vector2(vp.X + hw, topY),
                new Vector2(vp.X + hw, stretchCol2top ? vp.Y + h : topY + topH),
                new Color(40,40,40), 2f);
        if (botCount == 2)
            DrawLine(ref frame,
                new Vector2(vp.X + hw, botY),
                new Vector2(vp.X + hw, stretchCol2bot ? vp.Y + h : botY + botH),
                new Color(40,40,40), 2f);
    }

    if (showAzimuth)
        DrawAzimuthPanel(ref frame,
            PanelRect(vp, topY, topH, hw, hh, scale, scaling, is21,
                      isLeft:true, isTop:true, rowCount:topCount, total:total,
                      alignCenter:alignCenter, alignTop:alignTop, alignBottom:alignBottom, stretch:stretch),
            aziAngle, "Azm", auxAngle, invertAziReadout, turretAngle,
            flashAux && _warnAux.FlashHUD, flashAT && _warnAutoTurret.FlashHUD, flashMain && _warnMain.FlashHUD, activeAux, activeAT, activeMain, armedAux, armedAT, armedMain);

    if (showElevation)
        DrawElevationPanel(ref frame,
            PanelRect(vp, topY, topH, hw, hh, scale, scaling, is21,
                      isLeft:false, isTop:true, rowCount:topCount, total:total,
                      alignCenter:alignCenter, alignTop:alignTop, alignBottom:alignBottom, stretch:stretch),
            elevAngle, "Elev", invertElevReadout, elevationRotate);

    if (showPitch)
        DrawOrientationPanel(ref frame,
            PanelRect(vp, botY, botH, hw, hh, scale, scaling, is21,
                      isLeft:true, isTop:false, rowCount:botCount, total:total,
                      alignCenter:alignCenter, alignTop:alignTop, alignBottom:alignBottom, stretch:stretch),
            pitch, "PITCH", flashPitch && _warnPitch.FlashHUD, activePitch, armedPitch, pitchOffset, invertPitch);

    if (showRoll)
        DrawOrientationPanel(ref frame,
            PanelRect(vp, botY, botH, hw, hh, scale, scaling, is21,
                      isLeft:false, isTop:false, rowCount:botCount, total:total,
                      alignCenter:alignCenter, alignTop:alignTop, alignBottom:alignBottom, stretch:stretch),
            roll, "ROLL", flashRoll && _warnRoll.FlashHUD, activeRoll, armedRoll, rollOffset, invertRoll);
}

RectangleF PanelRect(RectangleF vp, float rowY, float rowH, float hw, float hh,
                     float scale, bool scaling, bool is21,
                     bool isLeft, bool isTop, int rowCount, int total,
                     bool alignCenter, bool alignTop, bool alignBottom, bool stretch)
{
    float w = vp.Width;
    float h = vp.Height;
    bool soloInRow    = (rowCount == 1);
    bool soloOnScreen = (total   == 1);
    bool isSoloIn21   = is21 && soloInRow;
    bool oppositeEmpty = soloInRow ? (total == 1) : (total == rowCount); // only one row has panels

    // Horizontal slice this panel owns
    float ownX = soloInRow ? vp.X : (isLeft ? vp.X : vp.X + hw);
    float ownW = soloInRow ? w    : hw;

    // Render size
    float rW, rH;
    if (isSoloIn21 && scaling)
    {
        float s = 1f - (1f - scale) * 0.5f;
        rW = w  * s;
        rH = hh * s;
    }
    else if (stretch && soloInRow && !soloOnScreen)
    {
        // 1+1 case: each panel is solo in its row, stretch to full LCD height
        rW = w * scale;
        rH = h * scale;
    }
    else if (stretch && !soloInRow && total == rowCount)
    {
        // 2+0 or 0+2 case: two panels, no opposite row -- go column, full height, half width
        rW = hw * scale;
        rH = h  * scale;
    }
    else
    {
        rW = hw * scale;
        rH = hh * scale;
    }

    // Vertical anchor:
    //   AlignTop    -> 0   (top of row)
    //   AlignBottom -> 1   (bottom of row)
    //   AlignCenter -> 0.5 (middle of row)
    //   default     -> home corner (top panels=0, bottom panels=1)
    float anchorY;
    if      (alignTop)    anchorY = 0f;
    else if (alignBottom) anchorY = 1f;
    else if (alignCenter) anchorY = 0.5f;
    else                  anchorY = isTop ? 0f : 1f;

    // Horizontal anchor:
    //   AlignCenter -> 0.5 (center of owned column)
    //   default     -> home corner (left=0, right=1)
    float anchorX = alignCenter ? 0.5f : (isLeft ? 0f : 1f);

    float rx = ownX + (ownW - rW) * anchorX;
    float ry = rowY + (rowH - rH) * anchorY;
    return new RectangleF(rx, ry, rW, rH);
}

// ================================================================
//  AZIMUTH PANEL
// ================================================================
void DrawAzimuthPanel(ref MySpriteDrawFrame frame, RectangleF quad,
                      float angle, string label, float auxAngle = float.NaN,
                      bool invertReadout = false, float turretAngle = float.NaN,
                      bool flashAux = false, bool flashAT = false, bool flashMain = false,
                      bool activeAux = false, bool activeAT = false, bool activeMain = false,
                      bool armedAux = false, bool armedAT = false, bool armedMain = false)
{
    Vector2 center = QuadCenter(quad);
    float   radius = Math.Min(quad.Width, quad.Height) * 0.30f;

    Color ringCol    = new Color(50, 50, 70);
    Color tickCol    = new Color(100, 100, 130);
    Color activeCol  = new Color(220, 50, 50);
    Color bearingCol = new Color(255, 200, 50);

    // Ring: 36 lines every 10°. Cardinals (N/E/S/W) = thickness 4, diagonals = 2, rest = 1
    float innerR = radius * 1.14f;
    float outerR = radius * 1.22f;
    for (int ri = 0; ri < 36; ri++)
    {
        float ta = MathHelper.TwoPi * ((float)ri / 36f);
        float lw = (ri % 9 == 0) ? 4f : 1f;
        Vector2 rp0 = center + new Vector2((float)Math.Sin(ta) * innerR, -(float)Math.Cos(ta) * innerR);
        Vector2 rp1 = center + new Vector2((float)Math.Sin(ta) * outerR, -(float)Math.Cos(ta) * outerR);
        DrawLine(ref frame, rp0, rp1, tickCol, lw);
    }
    foreach (float diagDeg in new float[]{ 45f, 135f, 225f, 315f })
    {
        float ta = MathHelper.ToRadians(diagDeg);
        Vector2 rp0 = center + new Vector2((float)Math.Sin(ta) * innerR, -(float)Math.Cos(ta) * innerR);
        Vector2 rp1 = center + new Vector2((float)Math.Sin(ta) * outerR, -(float)Math.Cos(ta) * outerR);
        DrawLine(ref frame, rp0, rp1, tickCol, 2f);
    }

    // Static hull box
    DrawRectOutline(ref frame, center, radius * 0.80f, radius * 1.00f, activeCol, 4f);
    DrawLine(ref frame, center+new Vector2(-5,0), center+new Vector2(5,0), activeCol, 1f);
    DrawLine(ref frame, center+new Vector2(0,-5), center+new Vector2(0,5), activeCol, 1f);

    // Rotating turret icon
    DrawTurretIcon(ref frame, center, radius, angle, activeCol);

    // Red turret needle -- drawn when auto turret is tagged
    if (!float.IsNaN(turretAngle))
    {
        Color turretCol = new Color(220, 50, 50);
        Vector2 needleBase = center + new Vector2( (float)Math.Sin(turretAngle) * radius * 1.10f,
                                                  -(float)Math.Cos(turretAngle) * radius * 1.10f);
        Vector2 needleTip  = center + new Vector2( (float)Math.Sin(turretAngle) * radius * 1.32f,
                                                  -(float)Math.Cos(turretAngle) * radius * 1.32f);
        DrawLine(ref frame, needleBase, needleTip, turretCol, 3f);
    }

    // Yellow vector arrow -- drawn when aux rotor is tagged
    if (!float.IsNaN(auxAngle))
    {
        Color auxCol = new Color(255, 200, 0);
        float arrowLen  = radius * 0.32f;
        float arrowWing = radius * 0.12f;

        // Arrow tip points outward at auxAngle
        Vector2 tip   = center + new Vector2( (float)Math.Sin(auxAngle) * radius * 0.50f,
                                             -(float)Math.Cos(auxAngle) * radius * 0.50f);
        Vector2 base2 = center + new Vector2( (float)Math.Sin(auxAngle) * radius * 0f,
                                             -(float)Math.Cos(auxAngle) * radius * 0f);

        // Shoulder point partway up stem where wings branch off
        Vector2 shoulder = base2 + new Vector2( (float)Math.Sin(auxAngle) * arrowLen * 1f,
                                               -(float)Math.Cos(auxAngle) * arrowLen * 1f);

        // Perpendicular for wings
        Vector2 perp  = new Vector2((float)Math.Cos(auxAngle), (float)Math.Sin(auxAngle));
        Vector2 wingL = shoulder + perp * arrowWing;
        Vector2 wingR = shoulder - perp * arrowWing;

        DrawLine(ref frame, base2, tip,  auxCol, 2f);  // stem
        DrawLine(ref frame, wingL, tip,  auxCol, 2f);  // left wing
        DrawLine(ref frame, wingR, tip,  auxCol, 2f);  // right wing
    }

    float bearing = MathHelper.ToDegrees(angle) % 360f;
    if (bearing < 0) bearing += 360f;
    if (invertReadout) bearing = bearing == 0f ? 0f : 360f - bearing;
    string bearingStr = $"{bearing:F1}";

    float labelScale = quad.Height * 0.055f;
    DrawSegString(ref frame, label,
        new Vector2(quad.X + quad.Width * 0.5f, quad.Y + quad.Height * 0.04f),
        bearingCol, labelScale, true);

    float numScale = quad.Height * 0.055f;
    DrawSegBoxed(ref frame, bearingStr,
        new Vector2(quad.X + quad.Width * 0.5f, quad.Y + quad.Height - 28f),
        bearingCol, new Color(80, 60, 10), numScale);

    // Warning triangles -- ghost at 15% when armed, full + blink when triggered
    float triSz  = quad.Width * 0.12f;
    float triOff = triSz * 1.1f;
    if (armedAux || activeAux)
        DrawWarningTriangle(ref frame,
            new Vector2(quad.X + triOff, quad.Y + triOff), triSz, "AUX",
            new Color(255,200,0), new Color(220,50,50),
            activeAux ? (flashAux ? 1f : 0.15f) : 0.15f,
            activeAux ? 1f : 0.15f, false);
    if (armedAT || activeAT)
        DrawWarningTriangle(ref frame,
            new Vector2(quad.X + quad.Width - triOff, quad.Y + triOff), triSz, "LOCK",
            new Color(220,50,50), new Color(255,200,0),
            activeAT ? (flashAT ? 1f : 0.15f) : 0.15f,
            activeAT ? 1f : 0.15f, false);
    if (armedMain || activeMain)
        DrawWarningTriangle(ref frame,
            new Vector2(quad.X + triOff, quad.Y + quad.Height - triOff), triSz, "MG",
            new Color(255,200,0), new Color(220,50,50),
            activeMain ? (flashMain ? 1f : 0.15f) : 0.15f,
            activeMain ? 1f : 0.15f, true);
}

void DrawTurretIcon(ref MySpriteDrawFrame frame, Vector2 pivot, float size, float angle, Color col)
{
    float tbW = size * 0.50f, tbH = size * 0.38f;
    DrawRect(ref frame, pivot + RotV(new Vector2(0, -size * 0.08f), angle), new Vector2(tbW, tbH), col, angle);
    DrawRect(ref frame, pivot + RotV(new Vector2(0, -size * 0.48f), angle), new Vector2(size * 0.11f, size * 0.55f), col, angle);
    DrawSprite(ref frame, "Triangle",
        pivot + RotV(new Vector2(0, -size * 0.27f), angle),
        new Vector2(tbW * 0.85f, tbH * 0.55f), col, angle);
    DrawLine(ref frame,
        pivot + RotV(new Vector2(0, -size * 0.76f), angle),
        pivot + RotV(new Vector2(0, -size * 0.99f), angle), col, 2f);
}

// ================================================================
//  ELEVATION PANEL
//  Red box on the right side. Barrel pivots from CENTER of box,
//  sweeps leftward (toward screen center).
//  0° = horizontal left, positive = tip up, negative = tip down.
// ================================================================
void DrawElevationPanel(ref MySpriteDrawFrame frame, RectangleF quad,
                        float elevAngle, string label, bool invertReadout = false,
                        float panelRotate = 0f)
{
    Vector2 center = QuadCenter(quad);
    float   boxW   = quad.Width  * 0.20f;
    float   boxH   = quad.Height * 0.42f;
    float   lineLen = quad.Width  * 0.50f;

    Color boxCol  = new Color(220, 50, 50);
    Color lineCol = new Color(220, 50, 50);
    Color refCol  = new Color(50, 50, 70);
    Color textCol = new Color(100, 200, 255);
    Color tickCol = new Color(70, 70, 90);

    // Helper: rotate a point around panel center
    Func<Vector2, Vector2> R = (v) => center + RotV(v - center, panelRotate);

    // Box sits right-of-center; rotated around panel center
    float boxOffX  = quad.Width * 0.18f;
    Vector2 boxCtrLocal = center + new Vector2(boxOffX, 0);
    Vector2 boxCtr = R(boxCtrLocal);
    DrawRectOutline(ref frame, boxCtr, boxW, boxH, boxCol, 2f, panelRotate);

    Vector2 pivot = boxCtr;

    // Base barrel direction rotated by panelRotate
    float baseAngle = panelRotate; // left=0, right=Pi, up=-PiOver2, down=PiOver2

    // Reference line
    Vector2 refEnd = pivot + RotV(new Vector2(-lineLen, 0), panelRotate);
    DrawLine(ref frame, pivot, refEnd, refCol, 1f);

    // Tick marks
    for (int t = -6; t <= 6; t++)
    {
        if (t == 0) continue;
        float ta  = MathHelper.ToRadians(t * 15f);
        Vector2 dir = RotV(new Vector2(-(float)Math.Cos(ta), -(float)Math.Sin(ta)), panelRotate);
        Vector2 tp  = pivot + dir * lineLen * 0.62f;
        float tickH = Math.Abs(t) % 2 == 0 ? 8f : 5f;
        Vector2 perp = new Vector2(-dir.Y, dir.X);
        DrawLine(ref frame, tp - perp * tickH * 0.5f, tp + perp * tickH * 0.5f, tickCol, 2f);
    }

    // Barrel line
    float clampedElev = MathHelper.Clamp(elevAngle, -MathHelper.PiOver2, MathHelper.PiOver2);
    Vector2 barrelDir = RotV(new Vector2(-(float)Math.Cos(clampedElev), (float)Math.Sin(clampedElev)), panelRotate);
    Vector2 barrelTip = pivot + barrelDir * lineLen;
    DrawLine(ref frame, pivot, barrelTip, lineCol, 3f);

    // Blue dot -- scaled to panel, solid circle
    Vector2 elevDot = pivot + barrelDir * lineLen * 0.62f;
    float dotSize = lineLen * 0.08f;
    DrawSprite(ref frame, "Circle", elevDot, new Vector2(dotSize, dotSize), textCol);

    // Boxed readout -- centered, padded from bottom
    float deg  = MathHelper.ToDegrees(elevAngle);
    if (invertReadout) deg = -deg;
    string sgn = deg >= 0 ? "+" : "";
    string elevStr = $"{sgn}{deg:F1}";
    float eLabelScale = quad.Height * 0.055f;
    float eNumScale   = quad.Height * 0.055f;
    DrawSegString(ref frame, label,
        new Vector2(quad.X + quad.Width * 0.5f, quad.Y + quad.Height * 0.06f),
        textCol, eLabelScale, true);
    DrawSegBoxed(ref frame, elevStr,
        new Vector2(quad.X + quad.Width * 0.5f, quad.Y + quad.Height - 28f),
        textCol, new Color(10, 40, 60), eNumScale);
}

// ================================================================
//  ORIENTATION PANEL (Pitch / Roll)
//  Static white box + rotating gold U-shape representing ground.
//  U = arc from left, curving DOWN through bottom, to right.
// ================================================================
void DrawOrientationPanel(ref MySpriteDrawFrame frame, RectangleF quad,
                          float tiltAngle, string label,
                          bool flashWrn = false, bool activeWrn = false, bool armedWrn = false,
                          float graphicOffset = 0f, bool invertReadout = false)
{
    float drawAngle = tiltAngle + graphicOffset;
    Vector2 center = QuadCenter(quad);
    float   r      = Math.Min(quad.Width, quad.Height) * 0.28f;
    float   cutLen = r * 0.28f;

    Color arcCol = new Color(200, 160, 10);
    Color boxCol = Color.White;

    // ---- Rotating ground arc ----
    int segments = 28;
    Vector2 prevLocal = Vector2.Zero;
    for (int i = 0; i <= segments; i++)
    {
        float a      = MathHelper.Pi - MathHelper.Pi * ((float)i / segments);
        Vector2 local = new Vector2((float)Math.Cos(a) * r, (float)Math.Sin(a) * r);
        if (i > 0)
        {
            Vector2 prev = center + RotV(prevLocal, drawAngle);
            Vector2 curr = center + RotV(local,     drawAngle);
            DrawLine(ref frame, prev, curr, arcCol, 3f);
        }
        prevLocal = local;
    }

    // Inward horizontal cuts at arc ends (left and right, pointing inward)
    Vector2 leftLocal  = new Vector2(-r, 0);
    Vector2 rightLocal = new Vector2( r, 0);
    Vector2 leftCutEnd  = leftLocal  + new Vector2( cutLen, 0);
    Vector2 rightCutEnd = rightLocal + new Vector2(-cutLen, 0);
    DrawLine(ref frame,
        center + RotV(leftLocal,   drawAngle),
        center + RotV(leftCutEnd,  drawAngle), arcCol, 3f);
    DrawLine(ref frame,
        center + RotV(rightLocal,  drawAngle),
        center + RotV(rightCutEnd, drawAngle), arcCol, 3f);

    // Static vehicle box (never rotates)
    DrawRectOutline(ref frame, center, r * 0.90f, r * 0.48f, boxCol, 2f);

    // Label just above the vehicle box
    Color labelCol = new Color(220, 200, 50);
    float oLabelScale = quad.Height * 0.055f;
    float oNumScale   = Math.Min(quad.Width, quad.Height) * 0.10f;
    DrawSegString(ref frame, label,
        Clamp2D(new Vector2(center.X, center.Y - r * 1.55f), quad, 6f),
        labelCol, oLabelScale, true);

    // Boxed number below the arc -- tiltAngle only, not offset; invert if requested
    float deg  = MathHelper.ToDegrees(invertReadout ? -tiltAngle : tiltAngle);
    string sgn = deg >= 0 ? "+" : "";
    string numStr = $"{sgn}{deg:F1}";
    DrawSegBoxed(ref frame, numStr,
        Clamp2D(center + new Vector2(0, r * 1.32f), quad, 14f),
        labelCol, new Color(60, 50, 10), oNumScale);

    // Warning triangle -- TOP corners, ghost at 15% when armed, full + blink when triggered
    if (armedWrn || activeWrn)
    {
        float triSz  = quad.Width * 0.12f;
        float triOff = triSz * 1.1f;
        bool  isRoll = label == "ROLL";
        float triX   = isRoll ? quad.X + quad.Width - triOff : quad.X + triOff;
        float triY   = quad.Y + triOff;
        DrawWarningTriangle(ref frame, new Vector2(triX, triY), triSz, "WRN",
            Color.White, Color.White,
            activeWrn ? (flashWrn ? 1f : 0.15f) : 0.15f,
            activeWrn ? 1f : 0.15f, false);
    }
}

// ================================================================
//  WARNING SYSTEM
// ================================================================
void EvalWarning(ref bool flash, ref bool active, ref float timer,
                 WarningConfig cfg, float angle, float dt)
{
    if (!cfg.Active || _speakers.Count == 0 || float.IsNaN(angle))
    { flash = false; active = false; return; }

    bool outside = angle > cfg.AnglePlus || angle < cfg.AngleMinus;
    active = outside;

    if (!outside) { flash = false; timer = cfg.Interval; return; }

    timer -= dt;
    if (timer <= 0f)
    {
        timer = cfg.Interval;
        flash = !flash;  // toggle for visual flash

        if (cfg.Sound && _speakers.Count > 0)
            foreach (var sp in _speakers) { sp.Play(); }
    }
}

// ----------------------------------------------------------------
//  DrawWarningTriangle: vector triangle + label
//  triAlpha  = opacity for triangle and exclamation (0.15 ghost, 1.0 full)
//  labelAlpha = opacity for label text
// ----------------------------------------------------------------
void DrawWarningTriangle(ref MySpriteDrawFrame frame, Vector2 center,
                         float size, string label,
                         Color triCol, Color exclCol,
                         float triAlpha, float labelAlpha, bool labelAbove = false)
{
    float h = size;
    float w = h * 0.866f;

    Vector2 tip = center + new Vector2(0,    -h * 0.65f);
    Vector2 bl  = center + new Vector2(-w,    h * 0.35f);
    Vector2 br  = center + new Vector2( w,    h * 0.35f);

    // Ghost state: darken colors AND apply alpha for double suppression
    bool ghost = triAlpha < 1f;
    Color drawTriCol  = ghost ? new Color(triCol.R  / 4, triCol.G  / 4, triCol.B  / 4) : triCol;
    Color drawExclCol = ghost ? new Color(exclCol.R / 4, exclCol.G / 4, exclCol.B / 4) : exclCol;
    Color drawLblCol  = labelAlpha < 1f ? new Color(triCol.R / 4, triCol.G / 4, triCol.B / 4) : triCol;

    Color tc = WA(drawTriCol,  triAlpha);
    Color ec = WA(drawExclCol, triAlpha);
    DrawLine(ref frame, tip, bl,  tc, 2f);
    DrawLine(ref frame, bl,  br,  tc, 2f);
    DrawLine(ref frame, br,  tip, tc, 2f);

    Vector2 exclTop = center + new Vector2(0, -h * 0.25f);
    Vector2 exclBot = center + new Vector2(0,  h * 0.05f);
    Vector2 dot     = center + new Vector2(0,  h * 0.22f);
    DrawLine(ref frame, exclTop, exclBot, ec, 2f);
    DrawLine(ref frame, dot, dot + new Vector2(0, 1f), ec, 3f);

    float lblScale = size * 0.35f;
    float lblY     = labelAbove ? -h * 0.90f : h * 0.72f;
    DrawSegString(ref frame, label,
        center + new Vector2(0, lblY),
        WA(drawLblCol, labelAlpha), lblScale, true);
}

// ----------------------------------------------------------------
//  Auto-write warning configs to fresh [TankHUD:Speaker] blocks
// ----------------------------------------------------------------
const string SPEAKER_DEFAULTS =
    "--- FORMAT: Active:FlashHUD:Sound:AnglePlus:AngleMinus:Interval ---\n" +
    "\n" +
    "--- MAIN GUN ---\n" +
    "[TankHUD:MainWarning:False:True:True:1:-1:1]      - Warn when main gun azimuth exceeds limits.\n" +
    "\n" +
    "--- AUX TURRET ---\n" +
    "[TankHUD:AuxWarning:False:True:True:1:-1:1]       - Warn when aux rotor exceeds limits.\n" +
    "\n" +
    "--- AUTO TURRET ---\n" +
    "[TankHUD:AutoWarning:True:True:True:1:-1:1]       - Warn when auto turret acquires a target.\n" +
    "\n" +
    "--- PITCH ---\n" +
    "[TankHUD:PitchWarning:False:True:True:45:-45:5]   - Warn when vehicle pitch exceeds limits.\n" +
    "\n" +
    "--- ROLL ---\n" +
    "[TankHUD:RollWarning:False:True:True:45:-45:5]    - Warn when vehicle roll exceeds limits.\n";

void AutoInitSpeakers()
{
    int written = 0;
    foreach (var sp in _speakers)
    {
        if (sp.CustomData.Contains("[TankHUD:AuxWarning:")) continue;
        string sep = sp.CustomData.Length > 0 ? "\n" : "";
        if (sp.CustomData.Contains(TAG_SPEAKER))
            sp.CustomData = sp.CustomData.TrimEnd() + sep + SPEAKER_DEFAULTS;
        else
            sp.CustomData = sp.CustomData.TrimEnd() + sep + TAG_SPEAKER + "\n" + SPEAKER_DEFAULTS;
        written++;
    }
    if (written > 0) Echo($"[Init] Warning configs written to {written} speaker(s).");
    LoadWarnings();
}

void LoadWarnings()
{
    if (_speakers.Count == 0) return;
    string cd = _speakers[0].CustomData;
    _warnAux        = ParseWarning(cd, "AuxWarning",   false, 1f,  -1f,  1f);
    _warnAutoTurret = ParseWarning(cd, "AutoWarning",  true,  1f,  -1f,  1f);
    _warnMain       = ParseWarning(cd, "MainWarning",  false, 1f,  -1f,  1f);
    _warnPitch      = ParseWarning(cd, "PitchWarning", false, 45f, -45f, 5f);
    _warnRoll       = ParseWarning(cd, "RollWarning",  false, 45f, -45f, 5f);
}

// ================================================================
//  HELPERS
// ================================================================
Vector2 QuadCenter(RectangleF q) =>
    new Vector2(q.X + q.Width * 0.5f, q.Y + q.Height * 0.5f);

// Clamp a text anchor point inside a quad with padding
Vector2 Clamp2D(Vector2 pt, RectangleF quad, float pad)
{
    return new Vector2(
        MathHelper.Clamp(pt.X, quad.X + pad, quad.X + quad.Width  - pad),
        MathHelper.Clamp(pt.Y, quad.Y + pad, quad.Y + quad.Height - pad));
}

void DrawArcDots(ref MySpriteDrawFrame frame, Vector2 center, float radius,
                 float startAngle, float endAngle, int count, Color col, float dotSize)
{
    for (int i = 0; i <= count; i++)
    {
        float a = startAngle + (endAngle - startAngle) * ((float)i / count);
        DrawSprite(ref frame, "CircleHollow",
            center + new Vector2((float)Math.Cos(a) * radius, (float)Math.Sin(a) * radius),
            new Vector2(dotSize, dotSize), col);
    }
}

// ----------------------------------------------------------------
//  Mirror + viewport rotation -- applied to every sprite position
// ----------------------------------------------------------------
Vector2 T(Vector2 pos)
{
    // Mirror first
    if (!float.IsNaN(_mx)) pos = new Vector2(2f * _mx - pos.X, pos.Y);
    // Then rotate around viewport center
    if (_rot != 0f)
    {
        float dx  = pos.X - _vcx, dy = pos.Y - _vcy;
        float cos = (float)Math.Cos(_rot), sin = (float)Math.Sin(_rot);
        pos = new Vector2(_vcx + dx * cos - dy * sin,
                          _vcy + dx * sin + dy * cos);
    }
    return pos;
}
float TR(float rotation)
{
    rotation = float.IsNaN(_mx) ? rotation : -rotation;  // mirror
    return rotation + _rot;                               // viewport rotation
}

// Apply per-surface alpha to a color
Color AC(Color c) => new Color(c.R / 255f, c.G / 255f, c.B / 255f, (c.A / 255f) * _alpha);

// Set alpha channel on a color (used for warning opacity before AC() applies surface alpha)
Color WA(Color c, float a) => new Color(c.R / 255f, c.G / 255f, c.B / 255f, a);

void DrawRect(ref MySpriteDrawFrame frame, Vector2 pos, Vector2 size,
              Color color, float rotation = 0f)
{
    frame.Add(new MySprite(VRage.Game.GUI.TextPanel.SpriteType.TEXTURE,
        "SquareSimple", T(pos), size, AC(color), rotation: TR(rotation)));
}

void DrawSprite(ref MySpriteDrawFrame frame, string type, Vector2 pos, Vector2 size,
                Color color, float rotation = 0f)
{
    frame.Add(new MySprite(VRage.Game.GUI.TextPanel.SpriteType.TEXTURE,
        type, T(pos), size, AC(color), rotation: TR(rotation)));
}

void DrawLine(ref MySpriteDrawFrame frame, Vector2 from, Vector2 to,
              Color color, float width = 2f)
{
    Vector2 mFrom = T(from);
    Vector2 mTo   = T(to);
    Vector2 dir   = mTo - mFrom;
    float length  = dir.Length();
    if (length < 0.001f) return;
    frame.Add(new MySprite(VRage.Game.GUI.TextPanel.SpriteType.TEXTURE,
        "SquareSimple", (mFrom + mTo) * 0.5f, new Vector2(width, length), AC(color),
        rotation: (float)Math.Atan2(dir.Y, dir.X) - MathHelper.PiOver2));
}

void DrawRectOutline(ref MySpriteDrawFrame frame, Vector2 center, float w, float h,
                     Color color, float thickness)
{
    float hw = w * 0.5f, hh = h * 0.5f;
    DrawLine(ref frame, center+new Vector2(-hw,-hh), center+new Vector2( hw,-hh), color, thickness);
    DrawLine(ref frame, center+new Vector2( hw,-hh), center+new Vector2( hw, hh), color, thickness);
    DrawLine(ref frame, center+new Vector2( hw, hh), center+new Vector2(-hw, hh), color, thickness);
    DrawLine(ref frame, center+new Vector2(-hw, hh), center+new Vector2(-hw,-hh), color, thickness);
}

void DrawRectOutline(ref MySpriteDrawFrame frame, Vector2 center, float w, float h,
                     Color color, float thickness, float angle)
{
    float hw = w * 0.5f, hh = h * 0.5f;
    Vector2 tl = center + RotV(new Vector2(-hw,-hh), angle);
    Vector2 tr = center + RotV(new Vector2( hw,-hh), angle);
    Vector2 br = center + RotV(new Vector2( hw, hh), angle);
    Vector2 bl = center + RotV(new Vector2(-hw, hh), angle);
    DrawLine(ref frame, tl, tr, color, thickness);
    DrawLine(ref frame, tr, br, color, thickness);
    DrawLine(ref frame, br, bl, color, thickness);
    DrawLine(ref frame, bl, tl, color, thickness);
}

Vector2 RotV(Vector2 v, float angle)
{
    float cos = (float)Math.Cos(angle);
    float sin = (float)Math.Sin(angle);
    return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
}

// ================================================================
//  16-SEGMENT VECTOR FONT
//  Segments per character stored as a ushort bitmask.
//  Segment layout (viewed front-on):
//
//   --a-- --b--
//  |     |     |
//  f     g     c
//  |     |     |
//   --h-- --i--
//  |     |     |
//  e     j     d
//  |     |     |
//   --k-- --l--
//
//  Bit  0 = a  (top-left horiz)
//  Bit  1 = b  (top-right horiz)
//  Bit  2 = c  (upper-right vert)
//  Bit  3 = d  (lower-right vert)
//  Bit  4 = e  (lower-left vert)
//  Bit  5 = f  (upper-left vert)
//  Bit  6 = g  (upper-left diag  \)
//  Bit  7 = h  (mid-left horiz)
//  Bit  8 = i  (mid-right horiz)
//  Bit  9 = j  (lower-left diag  /)
//  Bit 10 = k  (bot-left horiz)
//  Bit 11 = l  (bot-right horiz)
//  Bit 12 = m  (upper-right diag /)
//  Bit 13 = n  (lower-right diag \)
//  Bit 14 = o  (top center vert)
//  Bit 15 = p  (bot center vert)
// ================================================================

// Lookup table indexed by ASCII code (32-90 covers space, symbols, digits, uppercase)
static readonly ushort[] SEG16 = new ushort[128];

void InitSegFont()
{
    // Manual definitions -- S() helper sets each character's segment bitmask
    S('0', a:1,b:1,c:1,d:1,e:1,f:1,k:1,l:1);
    S('1', c:1,d:1);
    S('2', a:1,b:1,c:1,h:1,i:1,e:1,k:1,l:1);
    S('3', a:1,b:1,c:1,d:1,h:1,i:1,k:1,l:1);
    S('4', f:1,c:1,d:1,h:1,i:1);
    S('5', a:1,b:1,f:1,d:1,h:1,i:1,k:1,l:1);
    S('6', a:1,b:1,f:1,e:1,d:1,h:1,i:1,k:1,l:1);
    S('7', a:1,b:1,c:1,d:1);
    S('8', a:1,b:1,c:1,d:1,e:1,f:1,h:1,i:1,k:1,l:1);
    S('9', a:1,b:1,c:1,d:1,f:1,h:1,i:1,k:1,l:1);
    // Symbols
    S('.', l:1);
    S('-', h:1,i:1);
    S('+', h:1,i:1,o:1,p:1);
    S(' ');
    // Letters
    S('A', a:1,b:1,c:1,d:1,e:1,f:1,h:1,i:1);
    S('B', a:1,b:1,c:1,d:1,e:1,f:1,h:1,i:1,k:1,l:1,o:1,p:1);
    S('C', a:1,b:1,e:1,f:1,k:1,l:1);
    S('D', a:1,b:1,c:1,d:1,e:1,f:1,k:1,l:1,o:1,p:1);
    S('E', a:1,b:1,f:1,e:1,h:1,i:1,k:1,l:1);
    S('F', a:1,b:1,f:1,e:1,h:1,i:1);
    S('G', a:1,b:1,f:1,e:1,d:1,i:1,k:1,l:1);
    S('H', c:1,d:1,e:1,f:1,h:1,i:1);
    S('I', a:1,b:1,k:1,l:1,o:1,p:1);
    S('J', c:1,d:1,e:1,k:1,l:1);
    S('K', e:1,f:1,h:1,i:1,m:1,n:1); // diagonals
    S('L', e:1,f:1,k:1,l:1);
    S('M', c:1,d:1,e:1,f:1,g:1,m:1); // diagonals
    S('N', c:1,d:1,e:1,f:1,g:1,n:1);
    S('O', a:1,b:1,c:1,d:1,e:1,f:1,k:1,l:1);
    S('P', a:1,b:1,c:1,e:1,f:1,h:1,i:1);
    S('Q', a:1,b:1,c:1,d:1,e:1,f:1,k:1,l:1,n:1);
    S('R', a:1,b:1,c:1,e:1,f:1,h:1,i:1,n:1);
    S('S', a:1,b:1,f:1,h:1,i:1,d:1,k:1,l:1);
    S('T', a:1,b:1,o:1,p:1);
    S('U', c:1,d:1,e:1,f:1,k:1,l:1);
    S('V', e:1,f:1,j:1,m:1);
    S('W', c:1,d:1,e:1,f:1,j:1,n:1);
    S('X', g:1,j:1,m:1,n:1);
    S('Y', f:1,g:1,i:1,m:1,d:1);
    S('Z', a:1,b:1,j:1,m:1,k:1,l:1);
    // Lowercase -- map to uppercase visuals that read well small
    for (int lc = 'a'; lc <= 'z'; lc++)
        SEG16[lc] = SEG16[lc - 32];
}

void S(char ch,
    int a=0,int b=0,int c=0,int d=0,int e=0,int f=0,
    int g=0,int h=0,int i=0,int j=0,int k=0,int l=0,
    int m=0,int n=0,int o=0,int p=0)
{
    SEG16[(int)ch] = (ushort)(
        (a<<0)|(b<<1)|(c<<2)|(d<<3)|(e<<4)|(f<<5)|
        (g<<6)|(h<<7)|(i<<8)|(j<<9)|(k<<10)|(l<<11)|
        (m<<12)|(n<<13)|(o<<14)|(p<<15));
}

// Draw a single 16-seg character.
// origin = top-left corner of the cell. w/h = cell dimensions.
void DrawSegChar(ref MySpriteDrawFrame frame, char c, Vector2 origin,
                 float w, float h, Color col, float lw = 1.5f)
{
    int code = (int)c;
    if (code < 0 || code >= 128) return;

    // Degree symbol -- small circle in top-right of cell
    if (c == '~' || c == '\xB0')
    {
        float r = w * 0.22f;
        DrawSprite(ref frame, "Circle",
            new Vector2(origin.X + w * 0.62f, origin.Y + r * 1.1f),
            new Vector2(r * 2f, r * 2f), col);
        return;
    }
    ushort mask = SEG16[code];
    if (mask == 0) return;

    float x0 = origin.X, x1 = origin.X + w * 0.5f, x2 = origin.X + w;
    float y0 = origin.Y, y1 = origin.Y + h * 0.5f, y2 = origin.Y + h;
    float gap = w * 0.06f; // inset from corners for segment ends

    // Horizontal segments (a,b,h,i,k,l)
    if ((mask & (1<<0))  != 0) DrawLine(ref frame, new Vector2(x0+gap,y0), new Vector2(x1-gap,y0), col, lw); // a top-left
    if ((mask & (1<<1))  != 0) DrawLine(ref frame, new Vector2(x1+gap,y0), new Vector2(x2-gap,y0), col, lw); // b top-right
    if ((mask & (1<<7))  != 0) DrawLine(ref frame, new Vector2(x0+gap,y1), new Vector2(x1-gap,y1), col, lw); // h mid-left
    if ((mask & (1<<8))  != 0) DrawLine(ref frame, new Vector2(x1+gap,y1), new Vector2(x2-gap,y1), col, lw); // i mid-right
    if ((mask & (1<<10)) != 0) DrawLine(ref frame, new Vector2(x0+gap,y2), new Vector2(x1-gap,y2), col, lw); // k bot-left
    if ((mask & (1<<11)) != 0) DrawLine(ref frame, new Vector2(x1+gap,y2), new Vector2(x2-gap,y2), col, lw); // l bot-right
    // Vertical segments (c,d,e,f,o,p)
    if ((mask & (1<<2))  != 0) DrawLine(ref frame, new Vector2(x2,y0+gap), new Vector2(x2,y1-gap), col, lw); // c upper-right
    if ((mask & (1<<3))  != 0) DrawLine(ref frame, new Vector2(x2,y1+gap), new Vector2(x2,y2-gap), col, lw); // d lower-right
    if ((mask & (1<<4))  != 0) DrawLine(ref frame, new Vector2(x0,y1+gap), new Vector2(x0,y2-gap), col, lw); // e lower-left
    if ((mask & (1<<5))  != 0) DrawLine(ref frame, new Vector2(x0,y0+gap), new Vector2(x0,y1-gap), col, lw); // f upper-left
    if ((mask & (1<<14)) != 0) DrawLine(ref frame, new Vector2(x1,y0+gap), new Vector2(x1,y1-gap), col, lw); // o top center
    if ((mask & (1<<15)) != 0) DrawLine(ref frame, new Vector2(x1,y1+gap), new Vector2(x1,y2-gap), col, lw); // p bot center
    // Diagonal segments (g,j,m,n)
    if ((mask & (1<<6))  != 0) DrawLine(ref frame, new Vector2(x0+gap,y0+gap), new Vector2(x1-gap,y1-gap), col, lw); // g upper-left diag \
    if ((mask & (1<<9))  != 0) DrawLine(ref frame, new Vector2(x0+gap,y2-gap), new Vector2(x1-gap,y1+gap), col, lw); // j lower-left diag /
    if ((mask & (1<<12)) != 0) DrawLine(ref frame, new Vector2(x2-gap,y0+gap), new Vector2(x1+gap,y1-gap), col, lw); // m upper-right diag /
    if ((mask & (1<<13)) != 0) DrawLine(ref frame, new Vector2(x1+gap,y1+gap), new Vector2(x2-gap,y2-gap), col, lw); // n lower-right diag \
}

// charH = pixel height of each character cell; aspect ~0.6 wide per unit tall
void DrawSegString(ref MySpriteDrawFrame frame, string text, Vector2 center,
                   Color col, float charH, bool centered = true)
{
    if (string.IsNullOrEmpty(text)) return;
    float charW   = charH * 0.62f;
    float spacing = charW * 1.35f;  // more breathing room between chars
    float totalW  = spacing * (text.Length - 1) + charW;
    float startX  = centered ? center.X - totalW * 0.5f : center.X;
    float startY  = center.Y - charH * 0.5f;
    for (int i = 0; i < text.Length; i++)
        DrawSegChar(ref frame, text[i], new Vector2(startX + i * spacing, startY), charW, charH, col);
}

// Segment string with a surrounding box (replaces DrawBoxedText)
void DrawSegBoxed(ref MySpriteDrawFrame frame, string text, Vector2 center,
                  Color textCol, Color boxCol, float charH)
{
    if (string.IsNullOrEmpty(text)) return;
    float charW   = charH * 0.62f;
    float spacing = charW * 1.35f;
    float totalW  = spacing * (text.Length - 1) + charW;
    float padX = charH * 0.25f, padY = charH * 0.20f;
    float bW = totalW + padX * 2f;
    float bH = charH  + padY * 2f;
    DrawRectOutline(ref frame, center, bW, bH, boxCol, 1.5f);
    DrawSegString(ref frame, text, center, textCol, charH);
}