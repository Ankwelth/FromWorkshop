/* ============================================================================
 * PLANETARY OPERATIONS OS (ALIGNMENT + ALTIMETER + VERTICAL CONTROL)
 * ============================================================================
 * SETUP:
 * 1. Name your primary control seat exactly: Flight Seat
 * 2. Name your 5 downward cameras: Camera FL, Camera FR, Camera Center, Camera BL, Camera BR
 * 3. Name your altimeter LCD: Altimeter Screen
 * 4. Group your upward-pushing (lift) thrusters and name the group: [Lift]
 * ============================================================================
 * HOTBAR ARGUMENTS:
 * "ALIGN:START"       -> Locks ship flat to gravity (0 degrees).
 * "ALIGN:15"          -> Locks ship to gravity and pitches nose up 15 degrees.
 * "ALIGN:STOP"        -> Releases gyros.
 * "ALT:START"         -> Activates 5-point topographic laser scan.
 * "ALT:STOP"          -> Deactivates lasers to save server UPS.
 * "LAND:START"        -> Engages automated descent (defaults to 0.5 m/s).
 * "LAND:START:1.5"    -> Engages automated descent at exactly 1.5 m/s.
 * "LAND:STOP"         -> Releases lift thruster overrides.
 * "LIFT:START"        -> Engages automated lift-off (defaults to 2.0 m/s).
 * "LIFT:START:5"      -> Engages automated lift-off at exactly 5.0 m/s.
 * "LIFT:STOP"         -> Releases lift thruster overrides.
 * "ABORT_ALL"         -> Instantly shuts down all 3 systems (Panic Button).
 * ============================================================================
 */

// --- CONFIGURATION ---
const string SEAT_NAME = "Flight Seat";
const string ALT_LCD_NAME = "Altimeter Screen";
const string LIFT_GROUP = "[Lift]";

const double KP_ALIGN = 3.0; 
const double MANUAL_DEADZONE = 0.002; 
const double ALIGN_TOLERANCE = 0.005; 
const bool INVERT_ALIGNMENT = true; 

const double ABS_MAX_RANGE = 2500;
const double DEFAULT_DESCENT_SPEED = 0.5;
const double DEFAULT_LIFT_SPEED = 2.0;

// --- STATE VARIABLES ---
bool _alignActive = false;
double _pitchOffset = 0;

bool _altActive = false;
bool _verticalControlActive = false;
double _targetVerticalSpeed = 0; // Positive = Down (Descent), Negative = Up (Lift)
int _tick = 0;

IMyShipController _seat;
IMyTextPanel _altLcd;
List<IMyGyro> _gyros = new List<IMyGyro>();
List<IMyThrust> _liftThrusters = new List<IMyThrust>();

class Scanner 
{
    public string Name;
    public string Label;
    public IMyCameraBlock Cam;
    public double TargetDistance;
    public double LastHitDistance;
    public string Status;

    public Scanner(string name, string label)
    {
        Name = name; Label = label; TargetDistance = ABS_MAX_RANGE; LastHitDistance = -1; Status = "OFFLINE";
    }
}
List<Scanner> _scanners = new List<Scanner>();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.None;
    
    _scanners.Add(new Scanner("Camera FL", "FL"));
    _scanners.Add(new Scanner("Camera FR", "FR"));
    _scanners.Add(new Scanner("Camera Center", "CENTER"));
    _scanners.Add(new Scanner("Camera BL", "BL"));
    _scanners.Add(new Scanner("Camera BR", "BR"));
}

public void Main(string arg, UpdateType updateSource)
{
    // 1. COMMAND PARSER
    if ((updateSource & (UpdateType.Trigger | UpdateType.Terminal)) != 0 && !string.IsNullOrWhiteSpace(arg))
    {
        string cmd = arg.Trim().ToUpper();
        
        if (cmd == "ABORT_ALL")
        {
            _alignActive = false;
            _altActive = false;
            _verticalControlActive = false;
            ClearOverrides();
        }
        else if (cmd.StartsWith("ALIGN:"))
        {
            string param = cmd.Substring(6);
            if (param == "STOP")
            {
                _alignActive = false;
                foreach (var g in _gyros) g.GyroOverride = false;
            }
            else
            {
                if (param == "START") _pitchOffset = 0;
                else double.TryParse(param, out _pitchOffset);
                _alignActive = true;
            }
        }
        else if (cmd.StartsWith("ALT:"))
        {
            if (cmd == "ALT:START") _altActive = true;
            else if (cmd == "ALT:STOP")
            {
                _altActive = false;
                if (_altLcd != null) _altLcd.WriteText("=== TOPOGRAPHIC CLEARANCE ===\n\n[ SYSTEM OFFLINE ]");
            }
        }
        else if (cmd.StartsWith("LAND:"))
        {
            string[] parts = cmd.Split(':');
            if (parts.Length > 1)
            {
                if (parts[1] == "START")
                {
                    _verticalControlActive = true;
                    _targetVerticalSpeed = DEFAULT_DESCENT_SPEED;
                    if (parts.Length > 2) double.TryParse(parts[2], out _targetVerticalSpeed);
                    _targetVerticalSpeed = Math.Abs(_targetVerticalSpeed); // Ensure descent is positive
                }
                else if (parts[1] == "STOP" || parts[1] == "ABORT")
                {
                    _verticalControlActive = false;
                    foreach (var t in _liftThrusters) t.ThrustOverridePercentage = 0f;
                }
            }
        }
        else if (cmd.StartsWith("LIFT:"))
        {
            string[] parts = cmd.Split(':');
            if (parts.Length > 1)
            {
                if (parts[1] == "START")
                {
                    _verticalControlActive = true;
                    _targetVerticalSpeed = -DEFAULT_LIFT_SPEED; // Negative vector for upward velocity
                    if (parts.Length > 2)
                    {
                        double speed;
                        if (double.TryParse(parts[2], out speed)) 
                        {
                            _targetVerticalSpeed = -Math.Abs(speed);
                        }
                    }
                }
                else if (parts[1] == "STOP" || parts[1] == "ABORT")
                {
                    _verticalControlActive = false;
                    foreach (var t in _liftThrusters) t.ThrustOverridePercentage = 0f;
                }
            }
        }
        
        RefreshBlocks(); 
        ManageClock();
    }

    // 2. MAIN EXECUTION LOOP
    if ((updateSource & UpdateType.Update1) != 0)
    {
        if (_alignActive) ProcessAlignment();
        if (_verticalControlActive) ProcessVerticalControl();
        
        if (_tick % 10 == 0 && _altActive) ProcessAltimeter();
        if (_tick % 30 == 0) DrawOSStatus();

        _tick++;
    }
}

void ManageClock()
{
    if (_alignActive || _verticalControlActive || _altActive) Runtime.UpdateFrequency = UpdateFrequency.Update1;
    else Runtime.UpdateFrequency = UpdateFrequency.None;
}

void RefreshBlocks()
{
    _seat = GridTerminalSystem.GetBlockWithName(SEAT_NAME) as IMyShipController;

    _gyros.Clear();
    GridTerminalSystem.GetBlocksOfType(_gyros, b => b.IsSameConstructAs(Me));

    _altLcd = GridTerminalSystem.GetBlockWithName(ALT_LCD_NAME) as IMyTextPanel;

    foreach (var sc in _scanners) sc.Cam = GridTerminalSystem.GetBlockWithName(sc.Name) as IMyCameraBlock;

    _liftThrusters.Clear();
    IMyBlockGroup liftGroup = GridTerminalSystem.GetBlockGroupWithName(LIFT_GROUP);
    if (liftGroup != null) liftGroup.GetBlocksOfType(_liftThrusters, t => t.IsSameConstructAs(Me));
}

void ClearOverrides()
{
    foreach (var g in _gyros) g.GyroOverride = false;
    foreach (var t in _liftThrusters) t.ThrustOverridePercentage = 0f;
    ManageClock();
}

// ==========================================
// SYSTEM 1: GRAVITY ALIGNMENT
// ==========================================
void ProcessAlignment()
{
    if (_seat == null || _gyros.Count == 0) return;

    Vector3D gravity = _seat.GetNaturalGravity();
    if (gravity.LengthSquared() == 0)
    {
        foreach (var g in _gyros) g.GyroOverride = false;
        return; 
    }

    double pitchInput = _seat.RotationIndicator.X;
    double yawInput = _seat.RotationIndicator.Y;
    double rollInput = _seat.RollIndicator;

    bool isSteering = Math.Abs(pitchInput) > MANUAL_DEADZONE || Math.Abs(yawInput) > MANUAL_DEADZONE || Math.Abs(rollInput) > MANUAL_DEADZONE;
    if (isSteering)
    {
        foreach (var g in _gyros) g.GyroOverride = false;
        return; 
    }

    gravity.Normalize();
    Vector3D shipDown = INVERT_ALIGNMENT ? _seat.WorldMatrix.Up : _seat.WorldMatrix.Down;

    if (Math.Abs(_pitchOffset) > 0.01)
    {
        MatrixD pitchMatrix = MatrixD.CreateFromAxisAngle(_seat.WorldMatrix.Right, MathHelper.ToRadians(_pitchOffset));
        shipDown = Vector3D.Transform(shipDown, pitchMatrix);
    }

    Vector3D alignmentVector = Vector3D.Cross(shipDown, gravity);
    if (alignmentVector.LengthSquared() < (ALIGN_TOLERANCE * ALIGN_TOLERANCE)) alignmentVector = Vector3D.Zero;

    Vector3D targetRotation = alignmentVector * KP_ALIGN;

    foreach (var g in _gyros)
    {
        Vector3D localRot = Vector3D.TransformNormal(targetRotation, MatrixD.Transpose(g.WorldMatrix));
        g.Pitch = (float)localRot.X;
        g.Yaw   = (float)localRot.Y;
        g.Roll  = (float)localRot.Z;
        g.GyroOverride = true;
    }
}

// ==========================================
// SYSTEM 2: TOPOGRAPHIC ALTIMETER
// ==========================================
void ProcessAltimeter()
{
    if (_altLcd == null) return;
    _altLcd.ContentType = ContentType.TEXT_AND_IMAGE;
    bool anyCritical = false;

    foreach (var sc in _scanners)
    {
        if (sc.Cam == null)
        {
            sc.Status = "ERR"; continue;
        }

        sc.Cam.EnableRaycast = true;
        double actualScan = Math.Min(sc.TargetDistance, sc.Cam.AvailableScanRange);
        if (actualScan < 10)
        {
            sc.Status = "CHG"; continue;
        }

        MyDetectedEntityInfo hit = sc.Cam.Raycast(actualScan);
        if (!hit.IsEmpty() && hit.HitPosition.HasValue)
        {
            double dist = Vector3D.Distance(sc.Cam.GetPosition(), hit.HitPosition.Value);
            sc.LastHitDistance = dist;
            sc.Status = $"{dist:0}m";
            if (dist < 15) anyCritical = true;
            sc.TargetDistance = dist + 50;
        }
        else
        {
            sc.LastHitDistance = -1;
            if (actualScan >= ABS_MAX_RANGE - 1) sc.Status = "HIGH";
            else sc.Status = "SCAN";
            sc.TargetDistance += 500;
        }
        if (sc.TargetDistance > ABS_MAX_RANGE) sc.TargetDistance = ABS_MAX_RANGE;
    }

    string fl = _scanners[0].Status.PadRight(10);
    string fr = _scanners[1].Status.PadLeft(10);
    string center = _scanners[2].Status;
    string bl = _scanners[3].Status.PadRight(10);
    string br = _scanners[4].Status.PadLeft(10);

    string display = "=== TOPOGRAPHIC CLEARANCE ===\n\n";
    display += $"{fl}      {fr}\n\n";
    display += $"         [{center}]\n\n";
    display += $"{bl}      {br}\n\n";

    if (anyCritical) display += "-----------------------------\n!! PROXIMITY WARNING !!";

    _altLcd.WriteText(display);
}

// ==========================================
// SYSTEM 3: VERTICAL CONTROL (DESCENT/LIFT)
// ==========================================
void ProcessVerticalControl()
{
    if (_seat == null || _liftThrusters.Count == 0)
    {
        _verticalControlActive = false; return;
    }

    Vector3D gravity = _seat.GetNaturalGravity();
    if (gravity.LengthSquared() < 0.1)
    {
        _verticalControlActive = false; 
        foreach (var t in _liftThrusters) t.ThrustOverridePercentage = 0f;
        return;
    }

    double mass = _seat.CalculateShipMass().PhysicalMass;
    double weight = mass * gravity.Length();
    Vector3D velocity = _seat.GetShipVelocities().LinearVelocity;
    Vector3D gravNorm = Vector3D.Normalize(gravity);
    
    // Positive speed = falling towards gravity. Negative speed = moving against gravity.
    double currentVerticalSpeed = Vector3D.Dot(velocity, gravNorm);
    double speedError = currentVerticalSpeed - _targetVerticalSpeed;
    double pGain = mass * 2.5; 
    
    double requiredThrust = weight + (speedError * pGain);
    double totalMaxThrust = 0;
    
    foreach (var t in _liftThrusters) totalMaxThrust += t.MaxEffectiveThrust;
    if (totalMaxThrust == 0) return;

    float overridePercentage = (float)(requiredThrust / totalMaxThrust);
    overridePercentage = MathHelper.Clamp(overridePercentage, 0.01f, 1f);

    foreach (var t in _liftThrusters) t.ThrustOverridePercentage = overridePercentage;
}

// ==========================================
// TERMINAL LOGGING
// ==========================================
void DrawOSStatus()
{
    string status = "=== PLANETARY OPERATIONS OS ===\n\n";
    
    status += "GRAVITY ALIGNMENT : " + (_alignActive ? $"[ON] {_pitchOffset}°\n" : "[OFF]\n");
    status += "TOPOGRAPHIC LASER : " + (_altActive ? "[ON]\n" : "[OFF]\n");
    
    if (_verticalControlActive)
    {
        string mode = _targetVerticalSpeed < 0 ? "LIFT" : "DESCENT";
        status += $"VERTICAL CONTROL  : [ON] {Math.Abs(_targetVerticalSpeed)} m/s ({mode})\n";
    }
    else
    {
        status += "VERTICAL CONTROL  : [OFF]\n";
    }

    Echo(status);
}