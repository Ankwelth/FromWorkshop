/*
    XENE'S Light Missile Guidance
    
   Basic setup, have this programmable block and a gyroscope.
    The gyroscope MUST be orientated correctly as this script will use its orientation as the aiming orientation.
    To use WeapCcore guidance modes have any WeaponCore weapon on the missile (i recommend Cosnty's empty weapon)
    lock a target or input a gps into the programmable block's custom data and reload the PB.
    When you wish to fire simply run the FIRE argument.
    I recommend using a timer to activate the release of the missile from the platform,
    enable its gyro and activate some form of thrust system as this script only controls the gyroscope assigned in the custom data.

    optional addons, add some wing pieces to make the missile fly more realistically and glide better but it is not necessary.
    the script aims using the prograde motion of the grid so as long as there is thrust or lift it will hit the target (not the ground in front)
    
    GPS / WC Lock / WC GPS-proximity modes

    Arguments:
    FIRE
    RESET

    Custom Data:
    GPS>:GPS:Target:0:0:0:
    GPS Guided>:1
    GPS Proximity Guided>:0
    Weapon Core Lock Guided>:0
    Gyro Tag>:GUIDE
    Safety>:60
*/
//EDIT THESE
const double MIN_PROGRADE_SPEED = 5.0;

//gyro tuning
const double GYRO_GAIN = 18.0; //higher value turns faster
const double GYRO_DAMPING_GAIN = 0.3; //higher value more stable

// Prograde correction tuning
const double PROGRADE_CORRECTION_GAIN = 1.5;
const double MAX_PROGRADE_AIM_OFFSET_DEG = 15.0;

const bool SUPPRESS_ROLL = false; //enables or disables roll control

const double ARRIVAL_DISTANCE = 10.0;
const double GLOBAL_TIMESTEP = 1.0 / 60.0;

//NO NOT EDIT THIS
string gpsRaw = "";
bool gpsGuided = false;
bool gpsProximityGuided = false;
bool weaponCoreLockGuided = false;
string gyroTag = "GUIDE";
int safetyTicks = 60;

bool fired = false;
int ticksSinceFire = 0;

Vector3D gpsPoint;
bool hasGps = false;

List<IMyGyro> gyros = new List<IMyGyro>();
Dictionary<MyDetectedEntityInfo, float> threats = new Dictionary<MyDetectedEntityInfo, float>();

Vector3D lastPosition = Vector3D.Zero;
bool hasLastPosition = false;

double prevYaw = 0;
double prevPitch = 0;

double lastSpeed = 0;
double lastYaw = 0;
double lastPitch = 0;
double lastProgradeErrorDeg = 0;
double lastAimOffsetDeg = 0;
bool lastUsingProgradeCorrection = false;

WcPbApi wc = new WcPbApi();
bool wcReady = false;

// Cached WC target survives separation / WC drop
bool cachedWcTarget = false;
long cachedWcEntityId = 0;
Vector3D cachedWcPosition = Vector3D.Zero;
Vector3D cachedWcVelocity = Vector3D.Zero;
int cachedWcTick = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;

    EnsureCustomData();
    LoadConfig();
    RefreshBlocks();

    wcReady = wc.Activate(Me);
}

public void Main(string argument, UpdateType updateSource)
{
    if (!string.IsNullOrWhiteSpace(argument))
    {
        if (argument.Equals("FIRE", StringComparison.OrdinalIgnoreCase))
        {
            EnsureCustomData();
            LoadConfig();
            RefreshBlocks();
            wcReady = wc.Activate(Me);

            fired = true;
            ticksSinceFire = 0;
            hasLastPosition = false;
            prevYaw = 0;
            prevPitch = 0;
            cachedWcTarget = false;

            CaptureInitialWcTarget();
        }
        else if (argument.Equals("RESET", StringComparison.OrdinalIgnoreCase))
        {
            fired = false;
            ticksSinceFire = 0;
            hasLastPosition = false;
            prevYaw = 0;
            prevPitch = 0;
            cachedWcTarget = false;
            DisableGyros();
        }
        else if (argument.Equals("reload", StringComparison.OrdinalIgnoreCase))
        {
            EnsureCustomData();
            LoadConfig();
            RefreshBlocks();
            wcReady = wc.Activate(Me);
        }
    }

    if (!fired)
    {
        DisableGyros();
        Echo("SAFE");
        Echo("Gyros: " + gyros.Count);
        Echo("Tag: " + gyroTag);
        Echo("WC Ready: " + wcReady);
        Echo("Cached WC: " + cachedWcTarget);
        return;
    }

    ticksSinceFire++;

    if (ticksSinceFire <= safetyTicks)
    {
        DisableGyros();

        Echo("SAFETY");
        Echo("Ticks: " + ticksSinceFire + " / " + safetyTicks);
        Echo("Cached WC: " + cachedWcTarget);
        return;
    }

    if (gyros.Count == 0)
    {
        Echo("No gyro found");
        Echo("Tag: " + gyroTag);
        return;
    }

    Vector3D targetPos;
    string mode;

    if (!TryGetTarget(out targetPos, out mode))
    {
        DisableGyros();

        Echo("No valid target");
        Echo("WC Ready: " + wcReady);
        Echo("Cached WC: " + cachedWcTarget);
        Echo("GPS Valid: " + hasGps);
        Echo("GPS Guided: " + gpsGuided);
        Echo("GPS Prox: " + gpsProximityGuided);
        Echo("WC Lock: " + weaponCoreLockGuided);
        return;
    }

    GuideTo(targetPos);

    Echo("GUIDING");
    Echo("Mode: " + mode);
    Echo("Dist: " + Vector3D.Distance(Me.GetPosition(), targetPos).ToString("0"));
    Echo("Speed: " + lastSpeed.ToString("0.0"));
    Echo("WC Ready: " + wcReady);
    Echo("Cached WC: " + cachedWcTarget);
    Echo("Prograde Err: " + lastProgradeErrorDeg.ToString("0.0"));
    Echo("Aim Offset: " + lastAimOffsetDeg.ToString("0.0"));
    Echo("Yaw: " + lastYaw.ToString("0.000"));
    Echo("Pitch: " + lastPitch.ToString("0.000"));
}

void CaptureInitialWcTarget()
{
    if (!wcReady)
        return;

    if (weaponCoreLockGuided)
    {
        MyDetectedEntityInfo focus = wc.GetAiFocus(Me.CubeGrid.EntityId, 0);

        if (!focus.IsEmpty() && IsGrid(focus))
        {
            CacheWcTarget(focus);
            return;
        }
    }

    if (gpsProximityGuided && hasGps)
    {
        MyDetectedEntityInfo nearest;

        if (TryGetNearestHostileToGps(out nearest))
            CacheWcTarget(nearest);
    }
}

bool TryGetTarget(out Vector3D targetPos, out string mode)
{
    targetPos = Vector3D.Zero;
    mode = "None";

    if (weaponCoreLockGuided)
    {
        if (wcReady)
        {
            MyDetectedEntityInfo focus = wc.GetAiFocus(Me.CubeGrid.EntityId, 0);

            if (!focus.IsEmpty() && IsGrid(focus))
            {
                CacheWcTarget(focus);

                targetPos = focus.Position;
                mode = "WC Live Lock";
                return true;
            }

            MyDetectedEntityInfo refreshed;

            if (TryRefreshCachedTargetFromThreats(out refreshed))
            {
                CacheWcTarget(refreshed);

                targetPos = refreshed.Position;
                mode = "WC Cached Target Refresh";
                return true;
            }
        }

        if (cachedWcTarget)
        {
            targetPos = GetDeadReckonedCachedTarget();
            mode = "WC Dead Reckoning";
            return true;
        }
    }

    if (gpsProximityGuided)
    {
        if (wcReady && hasGps)
        {
            MyDetectedEntityInfo nearest;

            if (TryGetNearestHostileToGps(out nearest))
            {
                CacheWcTarget(nearest);

                targetPos = nearest.Position;
                mode = "WC Nearest Hostile To GPS";
                return true;
            }
        }

        if (cachedWcTarget)
        {
            targetPos = GetDeadReckonedCachedTarget();
            mode = "GPS-Prox Dead Reckoning";
            return true;
        }
    }

    if (gpsGuided && hasGps)
    {
        targetPos = gpsPoint;
        mode = "GPS";
        return true;
    }

    return false;
}

void CacheWcTarget(MyDetectedEntityInfo info)
{
    cachedWcTarget = true;
    cachedWcEntityId = info.EntityId;
    cachedWcPosition = info.Position;
    cachedWcVelocity = info.Velocity;
    cachedWcTick = ticksSinceFire;
}

Vector3D GetDeadReckonedCachedTarget()
{
    double dt = Math.Max(0, ticksSinceFire - cachedWcTick) * GLOBAL_TIMESTEP;
    return cachedWcPosition + cachedWcVelocity * dt;
}

bool TryRefreshCachedTargetFromThreats(out MyDetectedEntityInfo refreshed)
{
    refreshed = new MyDetectedEntityInfo();

    if (!cachedWcTarget || cachedWcEntityId == 0)
        return false;

    threats.Clear();
    wc.GetSortedThreats(Me, threats);

    foreach (var kv in threats)
    {
        MyDetectedEntityInfo info = kv.Key;

        if (info.IsEmpty()) continue;
        if (!IsGrid(info)) continue;

        if (info.EntityId == cachedWcEntityId)
        {
            refreshed = info;
            return true;
        }
    }

    return false;
}

bool TryGetNearestHostileToGps(out MyDetectedEntityInfo nearest)
{
    nearest = new MyDetectedEntityInfo();

    if (!hasGps)
        return false;

    threats.Clear();
    wc.GetSortedThreats(Me, threats);

    double bestDistSq = double.MaxValue;
    bool found = false;

    foreach (var kv in threats)
    {
        MyDetectedEntityInfo info = kv.Key;

        if (info.IsEmpty()) continue;
        if (!IsGrid(info)) continue;
        if (info.Relationship != MyRelationsBetweenPlayerAndBlock.Enemies) continue;

        double dSq = Vector3D.DistanceSquared(info.Position, gpsPoint);

        if (dSq < bestDistSq)
        {
            bestDistSq = dSq;
            nearest = info;
            found = true;
        }
    }

    return found;
}

bool IsGrid(MyDetectedEntityInfo info)
{
    return info.Type == MyDetectedEntityType.LargeGrid ||
           info.Type == MyDetectedEntityType.SmallGrid;
}

void GuideTo(Vector3D target)
{
    Vector3D pos = Me.GetPosition();
    Vector3D toTarget = target - pos;

    if (toTarget.LengthSquared() < ARRIVAL_DISTANCE * ARRIVAL_DISTANCE)
    {
        DisableGyros();
        return;
    }

    Vector3D desired = Vector3D.Normalize(toTarget);

    Vector3D velocity = GetVelocity();
    lastSpeed = velocity.Length();

    Vector3D targetVector = desired;
    lastUsingProgradeCorrection = false;
    lastProgradeErrorDeg = 0;
    lastAimOffsetDeg = 0;

    if (lastSpeed > MIN_PROGRADE_SPEED)
    {
        Vector3D prograde = Vector3D.Normalize(velocity);

        double progradeDot = MathHelper.Clamp(Vector3D.Dot(prograde, desired), -1.0, 1.0);
        lastProgradeErrorDeg = MathHelper.ToDegrees(Math.Acos(progradeDot));

        Vector3D correction = desired - prograde;
        targetVector = Vector3D.Normalize(desired + correction * PROGRADE_CORRECTION_GAIN);

        targetVector = LimitAimCone(desired, targetVector, MAX_PROGRADE_AIM_OFFSET_DEG);

        double aimDot = MathHelper.Clamp(Vector3D.Dot(desired, targetVector), -1.0, 1.0);
        lastAimOffsetDeg = MathHelper.ToDegrees(Math.Acos(aimDot));
        lastUsingProgradeCorrection = true;
    }

    IMyGyro refGyro = gyros[0];

    double newPitch = 0;
    double newYaw = 0;

    foreach (IMyGyro gyro in gyros)
    {
        GyroTurnRdav(
            targetVector,
            GYRO_GAIN,
            GYRO_DAMPING_GAIN,
            refGyro,
            gyro,
            prevYaw,
            prevPitch,
            out newPitch,
            out newYaw
        );
    }

    prevYaw = newYaw;
    prevPitch = newPitch;

    lastYaw = newYaw;
    lastPitch = newPitch;
}

Vector3D LimitAimCone(Vector3D centerDir, Vector3D aimDir, double maxDeg)
{
    centerDir = Vector3D.Normalize(centerDir);
    aimDir = Vector3D.Normalize(aimDir);

    double dot = MathHelper.Clamp(Vector3D.Dot(centerDir, aimDir), -1.0, 1.0);
    double angle = Math.Acos(dot);
    double maxRad = MathHelper.ToRadians(maxDeg);

    if (angle <= maxRad)
        return aimDir;

    Vector3D sideAxis = aimDir - centerDir * dot;

    if (sideAxis.LengthSquared() < 0.0001)
        return centerDir;

    sideAxis = Vector3D.Normalize(sideAxis);

    return Vector3D.Normalize(centerDir * Math.Cos(maxRad) + sideAxis * Math.Sin(maxRad));
}

void GyroTurnRdav(
    Vector3D targetVector,
    double gain,
    double damping,
    IMyTerminalBlock reference,
    IMyGyro gyro,
    double yawPrev,
    double pitchPrev,
    out double newPitch,
    out double newYaw)
{
    newYaw = 0;
    newPitch = 0;

    Vector3D up = reference.WorldMatrix.Up;
    Vector3D forward = reference.WorldMatrix.Forward;

    Quaternion q = Quaternion.CreateFromForwardUp(forward, up);
    Quaternion inv = Quaternion.Inverse(q);

    Vector3D local = Vector3D.Transform(targetVector, inv);

    double azimuth;
    double elevation;

    Vector3D.GetAzimuthAndElevation(local, out azimuth, out elevation);

    newYaw = azimuth;
    newPitch = elevation;

    azimuth += damping * ((azimuth - yawPrev) / GLOBAL_TIMESTEP);
    elevation += damping * ((elevation - pitchPrev) / GLOBAL_TIMESTEP);

    MatrixD refMatrix = MatrixD.CreateWorld(reference.GetPosition(), forward, up).GetOrientation();

    Vector3D worldVec = Vector3D.Transform(new Vector3D(elevation, azimuth, 0), refMatrix);

    Vector3D gyroLocal = Vector3D.Transform(
        worldVec,
        MatrixD.Transpose(gyro.WorldMatrix.GetOrientation())
    );

    if (double.IsNaN(gyroLocal.X) ||
        double.IsNaN(gyroLocal.Y) ||
        double.IsNaN(gyroLocal.Z))
    {
        return;
    }

    gyro.GyroOverride = true;

    gyro.Pitch = (float)MathHelper.Clamp(-gyroLocal.X * gain, -1000, 1000);
    gyro.Yaw   = (float)MathHelper.Clamp(-gyroLocal.Y * gain, -1000, 1000);

    if (SUPPRESS_ROLL)
        gyro.Roll = 0f;
    else
        gyro.Roll = (float)MathHelper.Clamp(-gyroLocal.Z * gain, -1000, 1000);
}

Vector3D GetVelocity()
{
    Vector3D pos = Me.GetPosition();

    if (!hasLastPosition)
    {
        lastPosition = pos;
        hasLastPosition = true;
        return Vector3D.Zero;
    }

    Vector3D vel = (pos - lastPosition) * 60.0;
    lastPosition = pos;

    return vel;
}

void DisableGyros()
{
    foreach (IMyGyro gyro in gyros)
    {
        gyro.GyroOverride = false;
        gyro.Pitch = 0f;
        gyro.Yaw = 0f;
        gyro.Roll = 0f;
    }
}

void RefreshBlocks()
{
    gyros.Clear();

    GridTerminalSystem.GetBlocksOfType(gyros, g =>
        g.CustomName.IndexOf(gyroTag, StringComparison.OrdinalIgnoreCase) >= 0 &&
        g.CubeGrid == Me.CubeGrid
    );
}

void EnsureCustomData()
{
    if (string.IsNullOrWhiteSpace(Me.CustomData))
    {
        Me.CustomData =
@"GPS>:GPS:Target:0:0:0:
GPS Guided>:1
GPS Proximity Guided>:0
Weapon Core Lock Guided>:0
Gyro Tag>:GUIDE
Safety>:60";
        return;
    }

    string cd = Me.CustomData;

    AddMissing(ref cd, "GPS>:", "GPS>:GPS:Target:0:0:0:");
    AddMissing(ref cd, "GPS Guided>:", "GPS Guided>:1");
    AddMissing(ref cd, "GPS Proximity Guided>:", "GPS Proximity Guided>:0");
    AddMissing(ref cd, "Weapon Core Lock Guided>:", "Weapon Core Lock Guided>:0");
    AddMissing(ref cd, "Gyro Tag>:", "Gyro Tag>:GUIDE");
    AddMissing(ref cd, "Safety>:", "Safety>:60");

    Me.CustomData = cd;
}

void AddMissing(ref string data, string key, string line)
{
    if (data.IndexOf(key, StringComparison.OrdinalIgnoreCase) < 0)
    {
        if (!data.EndsWith("\n"))
            data += "\n";

        data += line + "\n";
    }
}

void LoadConfig()
{
    gpsRaw = Get("GPS>:");
    gpsGuided = GetBool("GPS Guided>:");
    gpsProximityGuided = GetBool("GPS Proximity Guided>:");
    weaponCoreLockGuided = GetBool("Weapon Core Lock Guided>:");
    gyroTag = Get("Gyro Tag>:");

    int parsedSafety;
    if (int.TryParse(Get("Safety>:"), out parsedSafety))
        safetyTicks = Math.Max(0, parsedSafety);

    if (string.IsNullOrWhiteSpace(gyroTag))
        gyroTag = "GUIDE";

    hasGps = TryParseGps(gpsRaw, out gpsPoint);
}

string Get(string key)
{
    foreach (string raw in Me.CustomData.Split('\n'))
    {
        string line = raw.Trim();

        if (line.StartsWith(key, StringComparison.OrdinalIgnoreCase))
            return line.Substring(key.Length).Trim();
    }

    return "";
}

bool GetBool(string key)
{
    string v = Get(key);
    return v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase);
}

bool TryParseGps(string text, out Vector3D pos)
{
    pos = Vector3D.Zero;

    if (string.IsNullOrWhiteSpace(text))
        return false;

    text = text.Trim();

    if (!text.StartsWith("GPS:", StringComparison.OrdinalIgnoreCase))
        return false;

    string[] p = text.Split(':');

    if (p.Length < 5)
        return false;

    double x, y, z;

    if (double.TryParse(p[2], out x) &&
        double.TryParse(p[3], out y) &&
        double.TryParse(p[4], out z))
    {
        pos = new Vector3D(x, y, z);
        return true;
    }

    return false;
}

public class WcPbApi
{
    Action<IMyTerminalBlock, IDictionary<MyDetectedEntityInfo, float>> getSortedThreats;
    Func<long, int, MyDetectedEntityInfo> getAiFocus;

    public bool Activate(IMyTerminalBlock pb)
    {
        var prop = pb.GetProperty("WcPbAPI");
        if (prop == null) return false;

        var api = prop.As<IReadOnlyDictionary<string, Delegate>>().GetValue(pb);
        if (api == null) return false;

        Delegate del;

        if (!api.TryGetValue("GetSortedThreats", out del))
            return false;

        getSortedThreats = del as Action<IMyTerminalBlock, IDictionary<MyDetectedEntityInfo, float>>;

        if (!api.TryGetValue("GetAiFocus", out del))
            return false;

        getAiFocus = del as Func<long, int, MyDetectedEntityInfo>;

        return getSortedThreats != null && getAiFocus != null;
    }

    public void GetSortedThreats(IMyTerminalBlock pb, IDictionary<MyDetectedEntityInfo, float> dict)
    {
        if (getSortedThreats != null)
            getSortedThreats(pb, dict);
    }

    public MyDetectedEntityInfo GetAiFocus(long gridId, int priority)
    {
        if (getAiFocus != null)
            return getAiFocus(gridId, priority);

        return new MyDetectedEntityInfo();
    }
}