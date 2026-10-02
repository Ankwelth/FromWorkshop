/*═══════════════════════════════════════════════════════════════════
   NAVAL STABILIZER v1.0
   Space Engineers Programmable Block Script
═══════════════════════════════════════════════════════════════════
   Keeps naval vessels level by driving gyroscopes to align the
   ship's up vector with the gravity-defined "up".  Pitch and roll
   are corrected; yaw is left entirely free so helmsmen can steer.

   Works in natural gravity only (planets/moons).  In zero-g the
   overrides are released and the script idles.

   SETUP:
     1. Paste script into a Programmable Block and compile.
     2. All gyroscopes on the same grid are used automatically.
     3. The cockpit/seat marked "Main Cockpit" is used as the
        orientation reference.  If none is marked the first found
        is used.  Override with CTRL_NAME below.
     4. Optional: name an LCD panel "Stabilizer Display".

   TUNING:
     • KP  — proportional gain.  Higher = snappier corrections.
     • KD  — derivative gain.   Higher = less overshoot/oscillation.
     • DEADBAND_DEG — ignore tilt smaller than this (degrees).
     Start with defaults, then raise KP if corrections feel sluggish
     or lower KP / raise KD if the ship rocks back and forth.

   COMMANDS  (run argument / button):
     on      — enable stabilization
     off     — disable  (releases all gyro overrides)
     toggle  — flip enabled state
     rebuild — re-scan for gyros / controller (after adding blocks)
═══════════════════════════════════════════════════════════════════*/

// ════════════════════════════ Configuration ══════════════════════

const string LCD_NAME     = "Stabilizer Display";
const string CTRL_NAME    = "";      // blank = auto-detect main cockpit

const double KP           = 10.0;   // proportional gain
const double KD           = 8.0;    // derivative gain
const double DEADBAND_DEG = 0.4;    // degrees — no correction below this
const double MAX_RAD_S    = 15.0;   // gyro override clamp (rad/s)

// ════════════════════════════ State ══════════════════════════════

bool              _enabled  = true;
IMyShipController _ctrl;
IMyTextPanel      _lcd;
List<IMyGyro>     _gyros    = new List<IMyGyro>();
Vector3D          _prevErr  = Vector3D.Zero;
int               _rebuildIn = 0;

// ════════════════════════════ Lifecycle ══════════════════════════

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    Rebuild();
}

public void Save() { }

public void Main(string argument, UpdateType updateSource) {
    switch (argument.Trim().ToLower()) {
        case "on":      _enabled = true;   break;
        case "off":     _enabled = false;  break;
        case "toggle":  _enabled = !_enabled; break;
        case "rebuild": Rebuild(); break;
    }

    // Periodic re-scan in case blocks are added / come online
    if (--_rebuildIn <= 0) { Rebuild(); _rebuildIn = 600; }

    if (_ctrl == null || _gyros.Count == 0) {
        WriteDisplay("NO CONTROLLER / GYROS", 0, 0, 0);
        return;
    }

    if (!_enabled) {
        ReleaseAll();
        WriteDisplay("OFFLINE", 0, 0, 0);
        return;
    }

    Stabilize();
}

// ════════════════════════════ Block Discovery ════════════════════

void Rebuild() {
    // Controller
    if (!string.IsNullOrEmpty(CTRL_NAME)) {
        _ctrl = GridTerminalSystem.GetBlockWithName(CTRL_NAME) as IMyShipController;
    } else {
        var list = new List<IMyShipController>();
        GridTerminalSystem.GetBlocksOfType(list,
            b => b.CubeGrid == Me.CubeGrid && b.IsMainCockpit);
        if (list.Count == 0)
            GridTerminalSystem.GetBlocksOfType(list, b => b.CubeGrid == Me.CubeGrid);
        _ctrl = list.Count > 0 ? list[0] : null;
    }

    // Gyros
    _gyros.Clear();
    GridTerminalSystem.GetBlocksOfType(_gyros, b => b.CubeGrid == Me.CubeGrid);

    // LCD
    _lcd = GridTerminalSystem.GetBlockWithName(LCD_NAME) as IMyTextPanel;
    if (_lcd != null) {
        _lcd.ContentType     = ContentType.TEXT_AND_IMAGE;
        _lcd.Font            = "Monospace";
        _lcd.FontSize        = 0.55f;
        _lcd.FontColor       = new Color(0, 200, 255);
        _lcd.BackgroundColor = Color.Black;
        _lcd.TextPadding     = 1.5f;
    }
}

// ════════════════════════════ Stabilization ═════════════════════

void Stabilize() {
    Vector3D gravity = _ctrl.GetNaturalGravity();
    double   gSq     = gravity.LengthSquared();

    if (gSq < 0.01) {
        // Zero-g — nothing to level against
        ReleaseAll();
        WriteDisplay("NO GRAVITY", 0, 0, 0);
        return;
    }

    Vector3D gUnit    = gravity / Math.Sqrt(gSq);  // unit vector pointing down
    Vector3D targetUp = -gUnit;                     // where ship Up should point
    Vector3D shipUp   = _ctrl.WorldMatrix.Up;

    // Cross product gives world-space axis to rotate around, magnitude = sin(angle)
    Vector3D errVec   = Vector3D.Cross(shipUp, targetUp);
    double   sinErr   = errVec.Length();
    // clamp for Asin domain safety
    if (sinErr > 1.0) sinErr = 1.0;
    double   errDeg   = Math.Asin(sinErr) * (180.0 / Math.PI);

    // Inside deadband — release overrides and coast
    if (errDeg < DEADBAND_DEG) {
        ReleaseAll();
        WriteDisplay("LEVEL", errDeg, 0, _gyros.Count);
        return;
    }

    // PD controller — derivative damps oscillation on waves
    Vector3D deriv  = (errVec - _prevErr) * 6.0;   // d/dt at Update10 (6 calls/sec)
    _prevErr        = errVec;

    Vector3D angVel = errVec * KP + deriv * KD;

    // Clamp magnitude
    double mag = angVel.Length();
    if (mag > MAX_RAD_S) angVel *= MAX_RAD_S / mag;

    double effort = Math.Min(mag / MAX_RAD_S * 100.0, 100.0);

    foreach (var gyro in _gyros)
        ApplyGyro(gyro, angVel);

    string status = errDeg > 15.0 ? "CORRECTING" :
                    errDeg >  3.0 ? "LEVELING"   : "TRIMMING";
    WriteDisplay(status, errDeg, effort, _gyros.Count);
}

// Transform world-space angular velocity into each gyro's local frame.
// SE's Pitch/Yaw/Roll overrides are all sign-inverted vs the right-hand-rule
// convention used in the rest of the math, so negate all three axes.
void ApplyGyro(IMyGyro gyro, Vector3D worldAngVel) {
    var toLocal = MatrixD.Transpose(gyro.WorldMatrix);
    Vector3D lv = Vector3D.TransformNormal(worldAngVel, toLocal);

    gyro.GyroOverride = true;
    gyro.Pitch        = (float)-lv.X;
    gyro.Yaw          = (float)-lv.Y;
    gyro.Roll         = (float)-lv.Z;
}

void ReleaseAll() {
    _prevErr = Vector3D.Zero;
    foreach (var g in _gyros) {
        g.GyroOverride = false;
        g.Pitch = 0f; g.Yaw = 0f; g.Roll = 0f;
    }
}

// ════════════════════════════ Display ═══════════════════════════

void WriteDisplay(string status, double tilt, double effort, int gyroCount) {
    if (_lcd == null) return;

    string tiltBar = TiltBar(tilt);
    string effortS = effort.ToString("F0").PadLeft(3) + "%";

    var sb = new StringBuilder();
    sb.AppendLine("╔══════════════════════╗");
    sb.AppendLine("║   NAVAL STABILIZER   ║");
    sb.AppendLine("╠══════════════════════╣");
    sb.AppendLine("║ STATUS : " + status.PadRight(13) + "║");
    sb.AppendLine("╠══════════════════════╣");
    sb.AppendLine("║ TILT   :  " + tilt.ToString("F1").PadLeft(5) + "°        ║");
    sb.AppendLine("║ " + tiltBar + " ║");
    sb.AppendLine("╠══════════════════════╣");
    sb.AppendLine("║ EFFORT : " + effortS.PadRight(13) + "║");
    sb.AppendLine("║ GYROS  : " + gyroCount.ToString().PadRight(13) + "║");
    sb.AppendLine("╠══════════════════════╣");
    sb.AppendLine("║  on · off · toggle   ║");
    sb.AppendLine("╚══════════════════════╝");
    _lcd.WriteText(sb.ToString());
}

// Visual tilt indicator — centre = level, pipe moves with tilt
string TiltBar(double tiltDeg) {
    const int W = 20;
    // clamp to ±45°
    double norm  = tiltDeg / 45.0;
    if (norm > 1.0) norm = 1.0;
    int    pos   = (int)(norm * (W / 2)) + W / 2;
    if (pos < 0) pos = 0;
    if (pos >= W) pos = W - 1;
    char[] bar = new string('-', W).ToCharArray();
    bar[W / 2] = '┼';   // centre mark
    bar[pos]   = '█';
    return "[" + new string(bar) + "]";
}