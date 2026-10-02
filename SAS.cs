/*
 * ==== SAS ====
 * Stability Assist System
 * orbital tools
 * 
 * Author: DFPercush
 * Source code: github.com/DFPercush/SE-SAS
 * Feed the programmer: ko-fi.com/DFPercush
 * 
 * Setup:
 * If you want to use a cockpit besides the main one as the reference,
 * set 'DefaultControlBlock' below. Or use the 'control' command to set.
 * Make sure your craft has some gyros, power, and enable forward/back
 * thrusters for station keeping. That's about it.
 * 
 * Update: While locked to any direction, you can still give roll commands
 * by using Q and E from the active cockpit or control block. This may
 * only work from that one cockpit.
 * 
 * Commands:
 * 
 * off / stop:          Halt program and turn off gyro overrides.
 * 
 * pro / prograde:      The direction your ship is moving. Forward.
 * 
 * retro / retrograde:  Facing backwards along your direction of movement.
 * 
 * norm / normal:       Perpendicular to the plane of your orbit.
 *                      If you are moving east, normal would be north.
 * 
 * anti / antinormal:   Opposite normal. If moving east, antinormal is south.
 * 
 * out:                 Radial out. Facing perpendicular to your orbit but
 *                      in the same plane. Basically, away from the planet.
 * 
 * in:                  Radial in. Towards the ground.
 * 
 * disp:                Display debugging information.
 * 
 * keep:                Maintains a certain min and max altitude.
 *     (no args)            Toggle on/off. Maintain the current orbit.
 *     [number]             Maintain a circular orbit of [number] height,
 *                          in meters, from the center of the planet.
 *     [max] [min]          Maintain an orbit of apogee [max] and perigee [min].
 *     on|off|toggle        Enable or disable this mode.
 * 
 * control [block]      Sets the active cockpit, remote control, or ship controller.
 */


// ==== CONFIG ====

// This is probably the only thing you'll need to change
const string DefaultControlBlock = "Cockpit";

const double BaseGyroSensitivity = 0.05;  // Controls how "twitchy" your craft spins
const double TorquePerGyroMultiplier = 60000;  // Best guess at SE's physics

const double ORBIT_PRECISION = 500.0; // How far off can the altitude be before a correction burn is scheduled.
const double THROTTLE_BACK_TIME = 2.0; // Seconds before burn completes to start fine adjustment
const double MINIMUM_THRUST = 0.01; // Must be > 0, in the range (0, 1]
const double MAX_THRUST = 1.0;  // 0-1 multiplier for thrust override
const double ROLL_SPEED = 6.0;  // Target RPM of roll inputs while direction locked.

// Should not change unless using custom models with different coordinate system.
readonly Vector3D cockpitLocalForward = new Vector3D(0, 0, -1);
readonly Vector3D cockpitLocalUp = new Vector3D(0, 1, 0);
readonly Vector3D cockpitLocalRight = new Vector3D(1, 0, 0);

// ==== END CONFIG ====

static Program prg;
const double deg2rad = Math.PI / 180.0;
const double rad2deg = 180.0 / Math.PI;
enum Mode
{
    Disabled,
    Prograde,
    Retrograde,
    Normal,
    Antinormal,
    RadialIn,
    RadialOut,
    DisplayOnly
};
Mode mode;
IMyGridTerminalSystem G;
IMyShipController cockpit;
List<IMyGyro> gyros = new List<IMyGyro>();
List<GyroTranslator> gyt = new List<GyroTranslator>();
PID pidYaw;
PID pidPitch;

double TorqueToMassRatio = 1.0;
Orbit orb = new Orbit(
    new Vector3D(1,0,0),
    new Vector3D(0,0,1),
    new Vector3D(0,-1,0)
    );
bool keep;
double keepAp;
double keepPe;
enum ManeuverStatus
{
    IDLE,
    PLANNED,
    RUNNING
};
ManeuverStatus mstat;
delegate double getd();
static double get0() { return 0; }
getd mGetDelay = get0;
getd mGetTargetVel = get0;
double mTargetVel;
double prevFrameSpeed;
uint mFrameStartedBurn;
Base6Directions.Direction mThrustDir = Base6Directions.Direction.Backward;
string ControlBlock = DefaultControlBlock;

List<IMyShipController> shipControllers = new List<IMyShipController>();

public void FindCockpit()
{
    cockpit = G.GetBlockWithName(ControlBlock) as IMyShipController;
    if (cockpit == null || !(cockpit is IMyShipController))
    {
        G.GetBlocksOfType(shipControllers);
        foreach (var sc in shipControllers)
        {
            if (sc.IsMainCockpit)
            {
                Echo($"Warning: Control block '{ControlBlock}' not found.");
                Echo($"...Falling back to default main cockpit '{sc.CustomName}'.");
                Echo("Use 'control [block]' to change setting.");
                cockpit = sc;
                break;
            }
        }
        if (cockpit == null && shipControllers.Count > 0)
        {
            cockpit = shipControllers[0];
            Echo($"Warning: Control block '{ControlBlock}' not found.");
            Echo($"...Falling back to default main cockpit '{cockpit.CustomName}'.");
            Echo("Use 'control [block]' to change setting.");
        }
        if (cockpit == null)
        {
            Echo("Error: No ship control block found.");
            throw new NullReferenceException("Cockpit or ship controller not found.");
        }
    }
}
public Program()
{
    prg = this;
    G = GridTerminalSystem;

    var lines = Storage.Split('\n');
    foreach (var line in lines)
    {
        var sp = line.Split('=');
        if (sp.Length != 2) { continue; }
        switch (sp[0])
        {
            case "mode": mode = (Mode)Enum.Parse(typeof(Mode), sp[1]); break;
            case "keep": keep = bool.Parse(sp[1]); break;
            case "keepApo": keepAp = double.Parse(sp[1]); break;
            case "keepPeri": keepPe = double.Parse(sp[1]); break;
            case "ControlBlock": ControlBlock = line.Substring(13); break;
            default: Echo($"Warning: Unknown data saved in storage: {line}"); break;
        }
    }

    FindCockpit();
    FindGyros(cockpit);
    if (gyros.Count == 0)
    {
        Echo("Error: No gyros.");
        return;
    }
    TorqueToMassRatio = 0;
    foreach (var g in gyros)
    {
        TorqueToMassRatio += g.GyroPower;
    }
    TorqueToMassRatio /= cockpit.CalculateShipMass().TotalMass;
    double K = BaseGyroSensitivity * TorqueToMassRatio * TorquePerGyroMultiplier;
    pidYaw = new PID
    {
        Kp = K,
        Ki = K * 0.05,
        Kd = 0,
        limMin = -K * 10,
        limMax = K * 10,
        limMinInt = -K,
        limMaxInt = K,
        lowPass = 1.0,
        dt = 1.0 / 60,
        setpoint = 0
    };
    pidPitch = new PID
    {
        Kp = K,
        Ki = K * 0.5,
        Kd = 0,
        limMin = -K * 10,
        limMax = K * 10,
        limMinInt = -K,
        limMaxInt = K,
        lowPass = 1.0,
        dt = 1.0 / 60,
        setpoint = 0
    };

    if (mode != Mode.Disabled) { Runtime.UpdateFrequency = UpdateFrequency.Update1; }
    else { Runtime.UpdateFrequency = UpdateFrequency.None; }
}

public void Save()
{
    Storage = $"mode={mode}\nkeep={keep}\nkeepApo={keepAp}\nkeepPeri={keepPe}\nControlBlock={ControlBlock}";
}


public void Main(string arg, UpdateType updateSource)
{
    frameNum++;
    bool planetPresent = false;
    if (updateSource == UpdateType.Update1)
    {
        UpdateOrbit();
        Vector3D pp;
        planetPresent = cockpit.TryGetPlanetPosition(out pp);
        if (!planetPresent)
        {
            Echo("No planet.");
            if (mode == Mode.RadialIn || mode == Mode.RadialOut
                || mode == Mode.Normal || mode == Mode.Antinormal)
            {
                Echo($"{mode} not available.");
                return;
            }
        }
        if (planetPresent)
        {
            Vector3D vel = cockpit.GetShipVelocities().LinearVelocity;
            Vector3D posRtPlanet = cockpit.GetPosition() - pp;
            double r2 = posRtPlanet.LengthSquared();
            double r = Math.Sqrt(r2);
            double mu = cockpit.GetNaturalGravity().Length() * r2;
            orb.UpdateFromPosVel(posRtPlanet, vel, mu);

            Echo($"r = {r:F0}");
            Echo($"Ap: {orb.Ap:F0} ({timestr(orb.TimeToAp())})");
            Echo($"Pe: {orb.Pe:F0} ({timestr(orb.TimeToPe())})");
            Echo($"Inc: {orb.i*rad2deg:F2}");

            Echo($"Period: {timestr(orb.T)}");
            Echo($"Maneuver {mstat}");
            if (mstat == ManeuverStatus.PLANNED) { Echo($"Burn in {timestr(mGetDelay())}"); }
            if (mstat == ManeuverStatus.RUNNING) { Echo($"Vel {cockpit.GetShipSpeed():F0} / {mGetTargetVel():F0}"); }



        }
        Vector3D planetMe = cockpit.CenterOfMass - pp;
        Vector3D wvFwd = mul(cockpit.WorldMatrix.GetOrientation(), cockpitLocalForward);
        Vector3D vPro = cockpit.GetShipVelocities().LinearVelocity; vPro.Normalize();
        Vector3D vNorm = planetMe.Cross(vPro); vNorm.Normalize();
        Vector3D vOut = vPro.Cross(vNorm); vOut.Normalize();
        QuaternionD qDest = new QuaternionD();
        Vector3D vDest = new Vector3D();
        switch (mode)
        {
            case Mode.Disabled:
                Stop();
                break;
            case Mode.Prograde:
                vDest = vPro;
                asn(ref qDest, vPro);
                break;
            case Mode.Retrograde: vDest = -vPro; asn(ref qDest, -vPro); break;
            case Mode.Normal: vDest = vNorm; asn(ref qDest, vNorm); break;
            case Mode.Antinormal: vDest = -vNorm; asn(ref qDest, -vNorm); break;
            case Mode.RadialIn: vDest = -vOut; asn(ref qDest, -vOut); break;
            case Mode.RadialOut: vDest = vOut; asn(ref qDest, vOut); break;
            default: vDest = wvFwd; break;
        }
        Echo($"Holding {mode} {Spinner()}");
        Vector3 wvUp = mul(cockpit.WorldMatrix.GetOrientation(), cockpitLocalUp);
        Vector3 wvRight = mul(cockpit.WorldMatrix.GetOrientation(), cockpitLocalRight);
        QuaternionD qFacing = v2q(wvFwd);
        QuaternionD qStep = QuaternionD.Slerp(qFacing, qDest, 0.1);
        Vector3D vStep = q2v(qStep); vStep.Normalize();
        Vector3D vToward = vStep - wvFwd;
        double vTowardMag = vToward.Normalize();



        double dYaw = vToward.Dot(wvRight);
        double dPitch = vToward.Dot(wvUp);


        double angleDiff = AngleBetweenDeg(wvFwd, vDest);
        if (double.IsNaN(angleDiff)) { angleDiff = 0; }
        pidYaw.Update(Math.Sign(angleDiff - 90) * angleDiff * dYaw);
        pidPitch.Update(Math.Sign(angleDiff - 90) * angleDiff * dPitch);
        if (mode != Mode.DisplayOnly && mode != Mode.Disabled)
        {
            foreach (var g in gyt)
            {
                g.g.GyroOverride = true;
                g.setYaw(g.g, pidYaw.Output);
                g.setPitch(g.g, pidPitch.Output);

                g.setRoll(g.g, cockpit.RollIndicator * ROLL_SPEED);
            }
        }


        if (validOrbit)
        {
            if (keep) { Echo($"Keep: {keepAp:F0} - {keepPe:F0} +/- {ORBIT_PRECISION:F0}"); }

            if (mstat == ManeuverStatus.RUNNING)
            {
                double dv = (cockpit.GetShipSpeed() - prevFrameSpeed) * 60;
                double dTarget = mTargetVel - cockpit.GetShipSpeed();
                Echo($"Vel {cockpit.GetShipSpeed():F0} -> {mTargetVel}");
                UpdateThrust();
                if ((mThrustDir == Base6Directions.Direction.Forward && dTarget > 0) ||
                    (mThrustDir == Base6Directions.Direction.Backward && dTarget < 0))
                {
                    SetThrust(mThrustDir, (dTarget / dv / THROTTLE_BACK_TIME) + MINIMUM_THRUST);
                }
                else
                {
                    StopThrust();
                    mstat = ManeuverStatus.IDLE;
                }
            }
            else if (mstat == ManeuverStatus.PLANNED)
            {
                mGetTargetVel();
                if (mGetDelay() < THROTTLE_BACK_TIME)
                {
                    double targetSpeed = mGetTargetVel();
                    mThrustDir = (targetSpeed > cockpit.GetShipSpeed()) ? Base6Directions.Direction.Forward : Base6Directions.Direction.Backward;
                    mTargetVel = mGetTargetVel();
                    mstat = ManeuverStatus.RUNNING;
                    mFrameStartedBurn = frameNum;
                }
            }
            else if (keep)
            {
                UpdateOrbit();
                double ttap = orb.TimeToAp();
                double ttpe = orb.TimeToPe();
                double speed = cockpit.GetShipSpeed();
                if ((orb.Ap < keepAp - ORBIT_PRECISION && orb.Pe < keepPe - ORBIT_PRECISION)
                    )
                {
                    mGetDelay = () => 0;
                    mGetTargetVel = () => Orbit.RequiredVelocity(keepAp, keepPe, planetMe.Length(), orb.Mu);
                    mstat = ManeuverStatus.PLANNED;
                }
                else if (ttap < ttpe)
                {
                    if (Math.Abs(orb.Pe - keepPe) > ORBIT_PRECISION)
                    {
                        mGetDelay = () => orb.TimeToAp();
                        mGetTargetVel = () =>
                        {
                            double vel = Orbit.RequiredVelocity(orb.Ap, keepPe, GetShipPosRTPlanet().Length(), orb.Mu);
                            return vel;
                        };
                        mstat = ManeuverStatus.PLANNED;
                    }
                }
                else
                {
                    if (Math.Abs(orb.Ap - keepAp) > ORBIT_PRECISION)
                    {
                        mGetDelay = () => orb.TimeToPe();
                        mGetTargetVel = () =>
                        {
                            double vel = Orbit.RequiredVelocity(keepAp, orb.Pe, GetShipPosRTPlanet().Length(), orb.Mu);
                            return vel;
                        };
                        mstat = ManeuverStatus.PLANNED;
                    }
                }
            }


        }
































































        prevFrameSpeed = cockpit.GetShipSpeed();

    }
    else
    {
        if (arg.StartsWith("pro"))
        {
            mode = Mode.Prograde;
            mstat = ManeuverStatus.IDLE;
            keep = false;
            StopThrust();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }
        else if (arg.StartsWith("ret"))
        {
            mode = Mode.Retrograde;
            mstat = ManeuverStatus.IDLE;
            keep = false;
            StopThrust();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }
        else if (arg.StartsWith("nor"))
        {
            mode = Mode.Normal;
            mstat = ManeuverStatus.IDLE;
            keep = false;
            StopThrust();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }
        else if (arg.StartsWith("ant"))
        {
            mode = Mode.Antinormal;
            mstat = ManeuverStatus.IDLE;
            keep = false;
            StopThrust();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }
        else if (arg.StartsWith("in"))
        {
            mode = Mode.RadialIn;
            mstat = ManeuverStatus.IDLE;
            keep = false;
            StopThrust();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }
        else if (arg.StartsWith("out"))
        {
            mode = Mode.RadialOut;
            mstat = ManeuverStatus.IDLE;
            keep = false;
            StopThrust();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }
        else if (arg.StartsWith("off") || arg.StartsWith("stop"))
        {
            Stop();
        }
        else if (arg.StartsWith("control "))
        {
            ControlBlock = arg.Substring(8);
            FindCockpit();
        }
        else if (arg=="right")
        {
            foreach (var g in gyt) { g.g.GyroOverride = true; g.setYaw(g.g, 1.0); }
        }
        else if (arg=="left")
        {
            foreach (var g in gyt) { g.g.GyroOverride = true; g.setYaw(g.g, -1.0); }
        }
        else if (arg=="up")
        {
            foreach (var g in gyt) { g.g.GyroOverride = true; g.setPitch(g.g, 1.0); }
        }
        else if (arg=="down")
        {
            foreach (var g in gyt) { g.g.GyroOverride = true; g.setPitch(g.g, -1.0); }
        }
        else if (arg.StartsWith("disp"))
        {
            Stop();
            mode = Mode.DisplayOnly;
            mstat = ManeuverStatus.IDLE;
            keep = false;
            StopThrust();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }
        else if (arg.StartsWith("keep"))
        {
            Echo("arg.StarsWith('keep')");
            var sp = arg.Split(' ');
            double tmpAp, tmpPe;
            Echo($"sp.Length: {sp.Length}");
            if (sp.Length == 3)
            {
                if (double.TryParse(sp[1], out tmpAp) && double.TryParse(sp[2], out tmpPe))
                {
                    Echo("Setting Ap/Pe");
                    keepAp = tmpAp;
                    keepPe = tmpPe;
                    keep = true;
                }
                else
                {
                    Echo("keep 2-arg form expected [apoapsis] [periapsis]");
                }
            }
            else if (sp.Length == 2)
            {
                double tmpd;
                if (sp[1] == "on")
                {
                    Echo("keep on");
                    keep = true;
                    UpdateOrbit();
                    keepAp = orb.Ap;
                    keepPe = orb.Pe;
                }
                else if (sp[1] == "off")
                {
                    Echo("keep off");
                    StopThrust();
                    mstat = ManeuverStatus.IDLE;
                    keep = false;
                }
                else if (sp[1] == "toggle")
                {
                    Echo("keep toggle");
                    keep = !keep;
                    if (keep)
                    {
                        Echo("keep on");
                        UpdateOrbit();
                        keepAp = orb.Ap;
                        keepPe = orb.Pe;
                    }
                    else
                    {
                        Echo("keep off");
                        StopThrust();
                        mstat = ManeuverStatus.IDLE;
                    }
                }
                else if (double.TryParse(sp[1], out tmpd))
                {
                    Echo("keep on");
                    keep = true;
                    keepAp = keepPe = tmpd;
                    mode = Mode.Prograde;
                }
            }
            else if (sp.Length == 1)
            {
                keep = !keep;
                UpdateOrbit();
                keepAp = orb.Ap;
                keepPe = orb.Pe;
            }


            if (keep)
            {
                mode = Mode.Prograde;
                Runtime.UpdateFrequency = UpdateFrequency.Update1;
            }
            Echo($"keep={keep}, Ap={keepAp:0F}, Pe={keepPe:0F}");
            Echo($"mode = {mode}");
        }
    }

    Echo($"CPU: {(100.0f * (float)Runtime.CurrentInstructionCount / (float)Runtime.MaxInstructionCount):F2}%");
}

void Stop()
{
    mode = Mode.Disabled;
    keep = false;
    mstat = ManeuverStatus.IDLE;
    Runtime.UpdateFrequency = UpdateFrequency.None;
    StopThrust();
    foreach (var g in gyros)
    {
        g.Yaw = 0;
        g.Pitch = 0;
        g.Roll = 0;
        g.GyroOverride = false;
    }
    Echo("Stopped");
}

public static string timestr(TimeSpan t) { return $"{t.Hours:00}:{t.Minutes:00}:{t.Seconds:00}"; }
public static string timestr(double seconds)
{
    if (double.IsNaN(seconds)) { return "(NaN)"; }
    if (double.IsNegativeInfinity(seconds)) { return "(-infinity)"; }
    if (double.IsPositiveInfinity(seconds)) { return "(+infinity)"; }
    TimeSpan t = TimeSpan.FromSeconds(seconds);
    return $"{t.Hours:00}:{t.Minutes:00}:{t.Seconds:00}";
}
public static string timestr(DateTime t) { return $"{t.Hour:00}:{t.Minute:00}:{t.Second:00}"; }

bool validOrbit = false;
uint orbitUpdatedFrame = 0;
uint frameNum = 0;
void UpdateOrbit()
{
    if (orbitUpdatedFrame == frameNum) { return; }
    Vector3D pp;
    if (!cockpit.TryGetPlanetPosition(out pp)) { validOrbit = false; }
    else { validOrbit = true; }
    Vector3D shipPosRelativeToPlanet = cockpit.CenterOfMass - pp;
    orb.UpdateFromPosVel(
        shipPosRelativeToPlanet,
        cockpit.GetShipVelocities().LinearVelocity,
        cockpit.GetNaturalGravity().Length() * shipPosRelativeToPlanet.LengthSquared()
    );
    orbitUpdatedFrame = frameNum;
}

Vector3D GetShipPosRTPlanet()
{
    Vector3D pp;
    if (!cockpit.TryGetPlanetPosition(out pp)) { return new Vector3D(0, 0, 0); }
    return cockpit.CenterOfMass - pp;
}

string ppv(Vector3D v, int rounding = 2)
{
    return $"({Math.Round(v.X, rounding)}, {Math.Round(v.Y, rounding)}, {Math.Round(v.Z, rounding)})";
}
void asn(ref QuaternionD q, Vector3D v)
{
    q.X = v.X;
    q.Y = v.Y;
    q.Z = v.Z;
    q.W = 0;
}
QuaternionD v2q(Vector3D v)
{
    return new QuaternionD(v.X, v.Y, v.Z, 0);
}
Vector3D q2v(QuaternionD q)
{
    return new Vector3D(q.X, q.Y, q.Z);
}



Dictionary<Base6Directions.Direction, List<IMyThrust>> dirThrust = new Dictionary<Base6Directions.Direction, List<IMyThrust>>();
uint frameUpdateThrust = 0;
void UpdateThrust()
{
    if (frameUpdateThrust == frameNum) { return; }
    G.GetBlocksOfType(allThrust);
    if (dirThrust.Count == 0)
    {
        dirThrust[Base6Directions.Direction.Backward] = new List<IMyThrust>();
        dirThrust[Base6Directions.Direction.Down] = new List<IMyThrust>();
        dirThrust[Base6Directions.Direction.Forward] = new List<IMyThrust>();
        dirThrust[Base6Directions.Direction.Left] = new List<IMyThrust>();
        dirThrust[Base6Directions.Direction.Right] = new List<IMyThrust>();
        dirThrust[Base6Directions.Direction.Up] = new List<IMyThrust>();
    }
    foreach (var list in dirThrust) { list.Value.Clear(); }
    foreach (var t in allThrust)
    {
        dirThrust[Base6Directions.GetOppositeDirection(Base6Directions.GetDirection(t.GridThrustDirection))].Add(t);
    }
    frameUpdateThrust = frameNum;
}
double GetAvailableThrust(Base6Directions.Direction dir)
{
    double ret = 0;
    UpdateThrust();
    foreach (var t in dirThrust[dir]) { ret += t.MaxEffectiveThrust; }
    return ret;
}
void SetThrust(Base6Directions.Direction dir, double overrideFactor)
{
    overrideFactor = Math.Max(0, Math.Min(MAX_THRUST, overrideFactor));
    UpdateThrust();
    foreach (var t in dirThrust[dir])
    {
        t.ThrustOverride = t.MaxEffectiveThrust * (float)overrideFactor;
    }
}
void StopThrust()
{
    G.GetBlocksOfType(allThrust);
    foreach (var t in allThrust) { t.ThrustOverride = 0; }
}



const Base6Directions.Direction _up = Base6Directions.Direction.Up;
const Base6Directions.Direction _down = Base6Directions.Direction.Down;
const Base6Directions.Direction _left = Base6Directions.Direction.Left;
const Base6Directions.Direction _right = Base6Directions.Direction.Right;
const Base6Directions.Direction _forward = Base6Directions.Direction.Forward;
const Base6Directions.Direction _backward = Base6Directions.Direction.Backward;
static Base6Directions.Direction OppositeDirection(Base6Directions.Direction d)
{
    return Base6Directions.GetOppositeDirection(d);
}
delegate double GyroOverrideGetter(IMyGyro g);
delegate void GyroOverrideSetter(IMyGyro g, double rpm);
static double gyroGetPosPitch(IMyGyro g) => g.Pitch;
static double gyroGetNegPitch(IMyGyro g) => -g.Pitch;
static double gyroGetPosRoll(IMyGyro g) => g.Roll;
static double gyroGetNegRoll(IMyGyro g) => -g.Roll;
static double gyroGetPosYaw(IMyGyro g) => g.Yaw;
static double gyroGetNegYaw(IMyGyro g) => -g.Yaw;
static void gyroSetPosPitch(IMyGyro g, double rpm)
{
    float before = g.Pitch;
    g.SetValueFloat("Pitch", (float)rpm);
}
static void gyroSetNegPitch(IMyGyro g, double rpm)
{
    g.SetValueFloat("Pitch", (float)(-rpm));
}
static void gyroSetPosRoll(IMyGyro g, double rpm)
{
    g.SetValueFloat("Roll", (float)rpm);
}
static void gyroSetNegRoll(IMyGyro g, double rpm)
{
    g.SetValueFloat("Roll", (float)(-rpm));
}
static void gyroSetPosYaw(IMyGyro g, double rpm)
{
    g.SetValueFloat("Yaw", (float)rpm);
}
static void gyroSetNegYaw(IMyGyro g, double rpm)
{
    g.SetValueFloat("Yaw", (float)(-rpm));
}
static Base6Directions.Direction GridDirectionToBlock(IMyTerminalBlock block, Base6Directions.Direction d)
{
    if (block == null)
    {
        return d;
    }
    if (d == block.Orientation.Up) return _up;
    if (d == OppositeDirection(block.Orientation.Up)) return _down;
    if (d == block.Orientation.Left) return _left;
    if (d == OppositeDirection(block.Orientation.Left)) return _right;
    if (d == block.Orientation.Forward) return _forward;
    if (d == OppositeDirection(block.Orientation.Forward)) return _backward;
    throw new Exception("Unknown Base6Direction");
}
class GyroTranslator
{
    public IMyGyro g;
    public GyroOverrideGetter getPitch;
    public GyroOverrideGetter getRoll;
    public GyroOverrideSetter setRoll;
    public GyroOverrideSetter setPitch;
    public GyroOverrideGetter getYaw;
    public GyroOverrideSetter setYaw;
    public GyroTranslator(IMyGyro gyro, IMyTerminalBlock cockpit)
    {
        g = gyro;
        var ds = new StringBuilder();
        ds.Append(g.CustomName);

        switch (GridDirectionToBlock(cockpit, g.Orientation.Forward))
        {
            case Base6Directions.Direction.Forward:
                ds.Append(" FF");
                getRoll = gyroGetPosRoll;
                setRoll = gyroSetPosRoll;
                break;
            case Base6Directions.Direction.Backward:
                ds.Append(" FB");
                getRoll = gyroGetNegRoll;
                setRoll = gyroSetNegRoll;
                break;
            case Base6Directions.Direction.Right:
                ds.Append(" FR");
                getPitch = gyroGetPosRoll;
                setPitch = gyroSetPosRoll;
                break;
            case Base6Directions.Direction.Left:
                ds.Append(" FL");
                getPitch = gyroGetNegRoll;
                setPitch = gyroSetNegRoll;
                break;
            case Base6Directions.Direction.Up:
                ds.Append(" FU");
                getYaw = gyroGetNegRoll;
                setYaw = gyroSetNegRoll;
                break;
            case Base6Directions.Direction.Down:
                ds.Append(" FD");
                getYaw = gyroGetPosRoll;
                setYaw = gyroSetPosRoll;
                break;
        }
        switch (GridDirectionToBlock(cockpit, g.Orientation.Up))
        {
            case Base6Directions.Direction.Forward:
                ds.Append(" UF");
                getRoll = gyroGetNegYaw;
                setRoll = gyroSetNegYaw;
                break;
            case Base6Directions.Direction.Backward:
                ds.Append(" UB");
                getRoll = gyroGetPosYaw;
                setRoll = gyroSetPosYaw;
                break;
            case Base6Directions.Direction.Right:
                ds.Append(" UR");
                getPitch = gyroGetNegYaw;
                setPitch = gyroSetNegYaw;
                break;
            case Base6Directions.Direction.Left:
                ds.Append(" UL");
                getPitch = gyroGetPosYaw;
                setPitch = gyroSetPosYaw;
                break;
            case Base6Directions.Direction.Up:
                ds.Append(" UU");
                getYaw = gyroGetPosYaw;
                setYaw = gyroSetPosYaw;
                break;
            case Base6Directions.Direction.Down:
                ds.Append(" UD");
                getYaw = gyroGetNegYaw;
                setYaw = gyroSetNegYaw;
                break;
        }
        switch (GridDirectionToBlock(cockpit, g.Orientation.Left))
        {
            case Base6Directions.Direction.Left:
                ds.Append(" LL");
                getPitch = gyroGetPosPitch;
                setPitch = gyroSetPosPitch;
                break;
            case Base6Directions.Direction.Right:
                ds.Append(" LR");
                getPitch = gyroGetNegPitch;
                setPitch = gyroSetNegPitch;
                break;
            case Base6Directions.Direction.Forward:
                ds.Append(" LF");
                getRoll = gyroGetNegPitch;
                setRoll = gyroSetNegPitch;
                break;
            case Base6Directions.Direction.Backward:
                ds.Append(" LB");
                getRoll = gyroGetPosPitch;
                setRoll = gyroSetPosPitch;
                break;
            case Base6Directions.Direction.Up:
                ds.Append(" LU");
                getYaw = gyroGetPosPitch;
                setYaw = gyroSetPosPitch;
                break;
            case Base6Directions.Direction.Down:
                ds.Append(" LD");
                getYaw = gyroGetNegPitch;
                setYaw = gyroSetNegPitch;
                break;
        }
    }
}
void FindGyros(IMyShipController cockpit)
{
    gyros.Clear();
    gyt.Clear();
    G.GetBlocksOfType(gyros);
    foreach (var g in gyros)
    {
        gyt.Add(new GyroTranslator(g, cockpit));
    }


    ReconfigurePIDs();
}

void ReconfigurePIDs()
{
}

Vector3 mul(MatrixD m, Vector3 v)
{
    return new Vector3(
        (m.M11 * v.X) + (m.M21 * v.Y) + (m.M31 * v.Z) + m.M41,
        (m.M12 * v.X) + (m.M22 * v.Y) + (m.M32 * v.Z) + m.M42,
        (m.M13 * v.X) + (m.M23 * v.Y) + (m.M33 * v.Z) + m.M43);
}


static double AngleBetweenDeg(Vector3D pa, Vector3D pb)
{
    return (180.0 / Math.PI) * Math.Acos(pa.Dot(pb) / (pa.Length() * pb.Length()));
}
class PID
{
    public double Kp;
    public double Ki;
    public double Kd;

    public double lowPass;

    public double limMin;
    public double limMax;

    public double limMinInt;
    public double limMaxInt;

    public double dt;

    public double setpoint;

    public double integrator;
    public double prevError;
    public double differentiator;
    public double prevMeasurement;

    private double m_output;
    public double Output { get { return m_output; } }

    public double Update(double measurement, bool debugPrint = false)
    {
        double error = setpoint - measurement;
        double proportional = Kp * error;
        double integrator_delta = 0.5f * Ki * dt * (error + prevError);
        integrator += integrator_delta;

        if (integrator > limMaxInt)
        {
            integrator = limMaxInt;
        }
        else if (integrator < limMinInt)
        {
            integrator = limMinInt;
        }


        differentiator = -(2.0f * Kd * (measurement - prevMeasurement)
                            + (2.0f * lowPass - dt) * differentiator)
                            / (2.0f * lowPass + dt);



        m_output = proportional + integrator + differentiator;

        if (m_output > limMax)
        {

            m_output = limMax;

        }
        else if (m_output < limMin)
        {

            m_output = limMin;

        }


        prevError = error;
        prevMeasurement = measurement;

        if (double.IsNaN(integrator)) { integrator = 0; }
        if (double.IsNaN(prevError)) { prevError = 0; }
        if (double.IsNaN(differentiator)) { differentiator = 0; }
        if (double.IsNaN(prevMeasurement)) { prevMeasurement = 0; }
        if (double.IsNaN(m_output)) { m_output = 0; }


        return m_output;
    }
}

static readonly string[] s_spinnerChars = { "/", "-", "\\", "-" };
static int s_spinnerIndex;
string Spinner()
{
    s_spinnerIndex = (s_spinnerIndex + 1) % 4;
    return s_spinnerChars[s_spinnerIndex];
}


public struct PosVel
{
    public Vector3D pos;
    public Vector3D vel;
}
public class Orbit
{


    public double a, e, p, i, Om, w, v;
    public double Ap, Pe, T, b, Area, M, Ea, nMeanMotion, tpe, c, Mu;
    public Vector3D normal;
    public DateTime epoch;

    public Vector3D equinox = new Vector3D(1,0,0);
    public Vector3D eastOfEquinox = new Vector3D(0, 1, 0);
    public Vector3D north = new Vector3D(0, 0, 1);

    public Vector3D basis_x, basis_y, basis_z;
    public Orbit() { }
    public Orbit(Vector3D vEquinox, Vector3D vEastOfEquinox, Vector3D vNorth)
    {
        equinox = vEquinox;
        eastOfEquinox = vEastOfEquinox;
        north = vNorth;
    }
    static Vector3D mul(double s, Vector3 v)
    {
        return new Vector3(s * v.X, s * v.Y, s * v.Z);
    }
    static Vector3D div(Vector3 v, double s)
    {
        return new Vector3(v.X / s, v.Y / s, v.Z / s);
    }

    public static double RequiredVelocity(double Ap, double Pe, double r, double mu)
    {
        return RequiredVelocity((Ap + Pe) / 2, r, mu);
    }
    public static double RequiredVelocity(double sma, double r, double mu)
    {
        return Math.Sqrt((-mu / sma) + (2 * mu / r));
    }

    public void UpdateFromPosVel(
            Vector3D pos,
            Vector3D vel,
            double mu
        )

    {
        Vector3D rv = new Vector3D(pos.Dot(equinox), pos.Dot(eastOfEquinox), pos.Dot(north));
        Vector3D vv = new Vector3D(vel.Dot(equinox), vel.Dot(eastOfEquinox), vel.Dot(north));



        this.Mu = mu;

        double r = rv.Length();
        double v = vv.Length();

        Vector3D hv = rv.Cross(vv);
        normal = hv.Normalized();
        double h = hv.Length();

        Vector3D khat = new Vector3(0, 0, 1);
        Vector3D ihat = new Vector3(1, 0, 0);

        Vector3D nhat = khat.Cross(hv);
        double n = nhat.Length();

        Vector3D ev = ((((v * v) - (mu / r)) * rv) - (rv.Dot(vv) * vv)) / mu;
        this.e = ev.Length();

        double E = (v * v / 2) - (mu / r);

        if (this.e < 1.0)
        {
            this.a = -mu / (2 * E);

            this.p = this.a * (1 - this.e * this.e);
        }
        else
        {
            this.a = double.PositiveInfinity;
            this.p = h * h / mu;
        }

        this.i = Math.Acos(hv.Z / h);

        if (n == 0) { this.Om = 0; }
        else { this.Om = Math.Acos(nhat.X / n); }
        if (e == 0) { this.w = -Om; }
        else if (n == 0) { this.w = Math.Acos(ev.X / (this.e)); }
        else { this.w = Math.Acos(nhat.Dot(ev) / (n * this.e)); }
        this.v = Math.Acos(ev.Dot(rv) / (this.e * r));
        bool descent = (ev.Cross(rv).Dot(normal) < 0);
        if (descent) { this.v = (2 * Math.PI) - this.v; }
        this.epoch = DateTime.Now;
        this.c = this.a * this.e;
        Ap = (this.a) + c;
        Pe = (this.a) - c;
        b = Math.Sqrt(a*a + c*c);
        Area = Math.PI * a * b;
        Ea = TrueToEccentricAnomaly(this.v);
        M = EccentricToMeanAnomaly(Ea);


        nMeanMotion = Math.Sqrt(mu / Math.Abs(a * a * a));
        T = 2 * Math.PI / nMeanMotion;
        tpe = M / nMeanMotion;


        {
            QuaternionD qpe = QuaternionD.CreateFromAxisAngle(normal, w);
            Vector3D van = new Vector3D(Math.Cos(Om), Math.Sin(Om), 0);
            Vector3D vpe = qpe * van;
            Vector3D vlat = -(vpe.Cross(normal));

            basis_x = vpe; basis_x.Normalize();
            basis_y = vlat; basis_y.Normalize();
            basis_z = normal; basis_z.Normalize();
        }
    }
    public double EccentricToTrueAnomaly(double E)
    {
        double ret = 2 * Math.Atan(Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E / 2));
        if (E > Math.PI) { ret += 2 * Math.PI; }
        return ret;
    }
    public double TrueToEccentricAnomaly(double f)
    {
        double cosf = Math.Cos(f);
        double ret = Math.Acos((e + cosf) / (1 + e * cosf));
        if (f > Math.PI) { ret = 2 * Math.PI - ret; }
        return ret;
    }
    public double EccentricToMeanAnomaly(double E)
    {
        double ret = E - (e * Math.Sin(E));
        return ret;
    }
    public double TrueToMeanAnomaly(double f)
    {
        return EccentricToMeanAnomaly(TrueToEccentricAnomaly(f));
    }
    public double MeanToEccentricAnomaly_estimate(double M)
    {
        if (M <= Math.PI)
        {
            return 1.96 * Math.Pow(M, 0.41213689);
        }
        else
        {
            return (2 * Math.PI) - (1.96 * Math.Pow(2 * Math.PI - M, 0.41213689));
        }
    }
    public double MeanToTrueAnomaly_estimate(double M)
    {
        return EccentricToTrueAnomaly(MeanToEccentricAnomaly_estimate(M));
    }


    public double GetMeanAnomaly(DateTime when)
    {
        double t = (when - epoch).TotalSeconds;
        double ret = M + (t * 2 * Math.PI / T);
        while (ret > 2*Math.PI) { ret -= 2 * Math.PI; }
        while (ret < 0) { ret += 2 * Math.PI; }
        return ret;
    }
    public double MeanToEccentricAnomaly_expensive(double M)
    {
        return BinarySolveMonotonic((double x) => EccentricToMeanAnomaly(x) - M, 0, 2 * Math.PI, 0.0001);
    }
    public double GetEccentricAnomaly_expensive(DateTime when)
    {
        double M = GetMeanAnomaly(when);
        double E = MeanToEccentricAnomaly_expensive(M);
        return E;
    }
    public double GetTrueAnomaly_expensive(DateTime when)
    {
        return EccentricToTrueAnomaly(GetEccentricAnomaly_expensive(when));
    }
    public double TimeToMeanAnomaly(double Mdest)
    {
        double Mdiff = Mdest - M;
        while (Mdiff < 0) { Mdiff += 2 * Math.PI; }
        while (Mdiff > 2 * Math.PI) { Mdiff -= 2 * Math.PI; }
        return (Mdiff * T / (2 * Math.PI));
    }
    public double TimeToEccentricAnomaly(double E)
    {
        double M2 = EccentricToMeanAnomaly(E);
        return TimeToMeanAnomaly(M2);
    }
    public double TimeToTrueAnomaly(double f)
    {
        double M2 = TrueToMeanAnomaly(f);
        return TimeToMeanAnomaly(f);
    }
    public double TimeToAp()
    {
        double r = (T / 2) - tpe;
        while (r < 0) { r += T; }
        while (r > T) { r -= T; }
        return r;
    }
    public double TimeToPe()
    {
        return T - tpe;
    }
    public void UpdateFromTime_expensive(Orbit orb, DateTime when)
    {
        double dt = (when - orb.epoch).TotalSeconds;
        a = orb.a;
        e = orb.e;
        w = orb.w;
        Om = orb.Om;
        i = orb.i;
        this.Ap = orb.Ap;
        this.Pe = orb.Pe;
        this.Area = orb.Area;
        this.b = orb.b;
        this.epoch = when;
        this.nMeanMotion = orb.nMeanMotion;
        this.normal = orb.normal;
        this.p = orb.p;
        this.T = orb.T;
        this.basis_x = orb.basis_x;
        this.basis_y = orb.basis_y;
        this.basis_z = orb.basis_z;

        this.M = orb.GetMeanAnomaly(when);
        while (M > 2*Math.PI) { this.M -= 2 * Math.PI; }
        this.Ea = MeanToEccentricAnomaly_expensive(this.M);
        this.v = EccentricToTrueAnomaly(Ea);
        tpe = M / nMeanMotion;
    }
    public Vector3D GetPositionFromEccentricAnomaly(double E)
    {
        double oa2 = 1.0 / a; oa2 *= oa2;
        double ob2 = 1.0 / b; ob2 *= ob2;
        double sinE = Math.Sin(E);
        double cosE = Math.Cos(E);
        double r = Math.Sqrt(1.0 / ((oa2 * cosE * cosE) + (ob2 * sinE * sinE)));
        double x = r * cosE;
        double y = r * sinE;
        Vector3D local = (x * basis_x) + (y * basis_y);
        Vector3D ret = (local.X * equinox) + (local.Y * eastOfEquinox) + (local.Z * north);
        return ret;
    }
    public PosVel GetPosVelAtEccentricAnomaly(double E)
    {
        PosVel ret = new PosVel();
        Vector3D pa = GetPositionFromEccentricAnomaly(E);
        ret.pos = pa;
        Vector3D pb = GetPositionFromEccentricAnomaly(E + 0.0001);
        double ta = TimeToEccentricAnomaly(E);
        double tb = TimeToEccentricAnomaly(E + 0.0001);
        double dt = tb - ta;
        if (dt < 0) { dt += T; }
        ret.vel = (pb - pa) / dt;
        return ret;
    }
    public PosVel GetPosVelAtTrueAnomaly(double f)
    {
        double E = TrueToEccentricAnomaly(f);
        return GetPosVelAtEccentricAnomaly(E);
    }
}


delegate double MonotonicFunction(double x);
static double BinarySolveMonotonic(MonotonicFunction f, double min, double max, double tolerance)
{
    double x = (min + max) / 2;
    double y;
    double slope = (f(x+((max-min)/1000.0)) - f(x)) * 1000;
    while (max - min > tolerance)
    {
        x = (min + max) / 2;
        y = f(x);
        if (Math.Sign(y) == Math.Sign(slope)) { max = x; }
        else { min = x; }
    }
    return x;
}


public struct ThrustRatio
{
    public IMyThrust thruster;
    public float ratio;
}

List<IMyThrust> allThrust = new List<IMyThrust>();
List<IMyThrust> remThrust = new List<IMyThrust>();
Vector3D xhat = new Vector3D(1, 0, 0);
Vector3D yhat = new Vector3D(0, 1, 0);
Vector3D zhat = new Vector3D(0, 0, 1);
public void BalanceThrust(Vector3I directionRelativeToCockpit, ref List<ThrustRatio> outlist)
{
    G.GetBlocksOfType(allThrust);
    remThrust.Clear();
    foreach (var t in allThrust)
    {
        if (t.GridThrustDirection.Dot(ref directionRelativeToCockpit) > 0.3) { }
    }
}

public delegate double Operation(double lhs, double rhs);
public static double op_div(double lhs, double rhs) { return lhs / rhs; }
public static double op_mul(double lhs, double rhs) { return lhs * rhs; }
public static double op_add(double lhs, double rhs) { return lhs + rhs; }
public static double op_sub(double lhs, double rhs) { return lhs - rhs; }
public static double op_assign(double lhs, double rhs) { return rhs; }

public static void rowOp(double[,] m, int iDestRow, Operation op, int iSrcRow, double scalarMultiplier)
{
    int len = m.GetLength(1);
    for (int i = 0; i < len; i++) { m[iDestRow,i] = op(m[iDestRow,i], m[iSrcRow,i] * scalarMultiplier); }
}
public delegate void ReportProgress(string s);
public static void NullReportFn(string s) { }
public static readonly double[] primes10 = new double[]{ 2,3,5,7,11,13,17,19,23,29 };
public static bool ReducedRowEchelonForm(double[,] m, ReportProgress pr)
{
    int R = m.GetLength(0);
    int C = m.GetLength(1);
    string prs;

    for (int ir = 0; ir < R; ir++)
    {
        if (m[ir,ir] == 0)
        {
            bool found = false;
            for (int ir2 = 0; ir2 < R; ir2++)
            {
                if (m[ir2,ir] != 0)
                {
                    prs = $"M{ir+1} += M{ir2+1} (zero on diagonal check)";
                    rowOp(m, ir, op_add, ir2, primes10[ir2]);
                    pr(prs);
                    found = true;
                }
            }
            if (!found)
            {
                pr("Matrix contains a column of all zeroes, rref can not be solved.");
                return false;
            }
        }
    }

    for (int ir = 0; ir < R; ir++)
    {


        prs =$"M{ir+1} /= {m[ir,ir]}";
        rowOp(m, ir, op_assign, ir, 1.0 / m[ir, ir]);
        pr(prs);

        for (int sr = ir + 1; sr < R; sr++)
        {


            {
                prs = $"M{sr + 1} -= M{ir + 1} * {m[sr, ir]} (M{sr + 1},{ir + 1})";
                rowOp(m, sr, op_sub, ir, m[sr, ir]);
                pr(prs);
            }
            {
            }
        }
        for (int sr = ir + 1; sr < R; sr++)
        {
            if (m[sr, sr] == 0)
            {
                bool found = false;
                for (int sr2 = sr + 1; sr2 < R; sr2++)
                {
                    if (m[sr2, sr] != 0)
                    {
                        prs = $"M{sr + 1} += M{sr2 + 1} (diagonal zero fix 2)";
                        rowOp(m, sr, op_add, sr2, 1);
                        pr(prs);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    pr($"Don't know how to fix the zero in diagonal at M{sr + 1}");
                    return false;
                }
            }
        }
    }
    for (int ir = R - 1; ir >= 0; ir--)
    {
        for (int sr = ir - 1; sr >= 0; sr--)
        {


            prs = ($"M{sr+1} -= M{ir+1} * {m[sr, ir]} (M{sr+1},{ir+1})");
            rowOp(m, sr, op_sub, ir, m[sr, ir]);
            pr(prs);
        }
    }
    return true;
}

void GetThrusterRatios(Vector3D direction, ref List<ThrustRatio> outlist)
{
    direction.Normalize();
    outlist.Clear();
    G.GetBlocksOfType(allThrust);
    MatrixD ori = cockpit.WorldMatrix.GetOrientation();
    Vector3D shipx = mul(ori, xhat);
    Vector3D shipy = mul(ori, yhat);
    Vector3D shipz = mul(ori, zhat);

    Vector3D CenterOfThrustStart = new Vector3D(0,0,0);
    Vector3D ThrustTotal = new Vector3D(0,0,0);
    Vector3D ThrustTotalDir = new Vector3D(0, 0, 0);
    Vector3D am = new Vector3D(0, 0, 0);
    double ThrustMagnitudeTotal = 0;
    Vector3D com = cockpit.CenterOfMass;
    foreach (var t in allThrust)
    {
        if (!t.IsWorking || !t.Enabled) { continue; }
        Vector3D tdir = (t.GridThrustDirection.X * shipx) + (t.GridThrustDirection.Y * shipy) + (t.GridThrustDirection.Z * shipz);
        tdir.Normalize();
        Vector3D tv = t.MaxEffectiveThrust * tdir;
        double tdot = tdir.Dot(direction);
        ThrustRatio tr = new ThrustRatio();
        tr.thruster = t;
        tr.ratio = 0;
        if (tdot > 0.3)
        {
            ThrustMagnitudeTotal += t.MaxEffectiveThrust;
            ThrustTotal += tv;
            CenterOfThrustStart += t.WorldMatrix.Translation * t.MaxEffectiveThrust;
            tr.ratio = 1;
            Vector3D tpos = t.WorldMatrix.Translation;
            am += (tpos - com).Cross(tv);
        }
        outlist.Add(tr);
    }
    ThrustTotalDir = ThrustTotal.Normalized();
    if (ThrustMagnitudeTotal > 0)
    {
        CenterOfThrustStart /= ThrustMagnitudeTotal;
    }
    else
    {
        Echo("Warning: Can not find adequate thrust for station keeping maneuver!");
        return;
    }


    foreach (var tr in outlist)
    {

    }











    while (ThrustTotalDir.Dot(direction) < 0.99)
    {
        Vector3D adjust = direction - ThrustTotalDir;
        IMyThrust matchThruster;
        double matchDot = 0;
        Vector3D matchTv = new Vector3(0,0,0);
        foreach (var tr in outlist)
        {
            IMyThrust t = tr.thruster;
            if (!t.IsWorking || !t.Enabled) { continue; }
            Vector3D tdir = (t.GridThrustDirection.X * shipx) + (t.GridThrustDirection.Y * shipy) + (t.GridThrustDirection.Z * shipz);
            tdir.Normalize();
            Vector3D tv = t.MaxEffectiveThrust * tdir;
            double tdot = tdir.Dot(direction);
            double test = tv.Dot(adjust);
            if (test > matchDot)
            {
                matchDot = test;
                matchThruster = t;
                matchTv = tv;
            }
        }
        Vector3D A = ThrustTotal - matchTv;
        Vector3D B = ThrustTotal;
        Vector3D D = direction;
        double Adot = A.Dot(D) / A.Length();
        double Bdot = B.Dot(D) / B.Length();
        Vector3D ABHalf = (A + 0.5 * (B - A));
        double ABHalfDot = ABHalf.Dot(D) / ABHalf.Length();
        double P = (D.X * B.X) - (D.X * A.X) + (D.Y * B.Y) - (D.Y * A.Y) + (D.Z * B.Z) - (D.Z * A.Z);
        double Q = (D.X * A.X) + (D.Y * A.Y) + (D.Z + A.Z);
    }
}