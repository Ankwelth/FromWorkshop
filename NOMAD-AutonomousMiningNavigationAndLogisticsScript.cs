/*
 NOMAD Core v0.1 - single autonomous miner MVP

 Scope:
 - Fixed base
 - One ship
 - Remote Control cruise by sparse recorded waypoints
 - Drill direction independent from normal flight direction
 - Connector direction independent from normal flight direction
 - Automatic mining, retract, return and docking
 - Main construct only (no drill/connector on moving rotor/hinge/piston subgrids)

 Required names (editable in Custom Data):
 [NMD] Flight      = Remote Control oriented for normal travel
 [NMD] Drill Ref   = Camera/Remote/terminal block whose Forward points into the shaft
 [NMD] Dock        = Ship connector used at home
 [NMD] LCD         = optional text panel

 Commands:
 scan
 home dock
 home approach
 route add
 route pop
 route clear
 mine capture
 start
 return
 pause
 stop
 status
*/

const string VERSION = "0.1.0";

enum Mode
{
    Idle,
    Undock,
    CruiseOut,
    MineApproach,
    MineAlign,
    Mining,
    Retract,
    CruiseHome,
    HomeApproach,
    DockFinal,
    Service,
    Paused,
    Error
}

struct Pose
{
    public bool Valid;
    public Vector3D Position;
    public Vector3D Forward;
    public Vector3D Up;
}

// ---------- Configuration ----------
string FLIGHT_NAME = "[NMD] Flight";
string DRILL_REF_NAME = "[NMD] Drill Ref";
string DOCK_NAME = "[NMD] Dock";
string LCD_NAME = "[NMD] LCD";

double CRUISE_SPEED = 65.0;
bool COLLISION_AVOIDANCE = true;

double MINE_DEPTH = 40.0;
double MINE_SPEED = 0.55;
double RETRACT_SPEED = 2.0;
double MINE_CLEARANCE = 18.0;

double RETURN_CARGO = 0.82;
double RETURN_BATTERY = 0.25;
double RETURN_HYDROGEN = 0.20;

double APPROACH_SPEED = 3.0;
double DOCK_SPEED = 0.35;
double ROUTE_ARRIVAL = 6.0;
double POSE_POSITION_TOLERANCE = 0.55;
double DOCK_POSITION_TOLERANCE = 0.18;
double ANGLE_TOLERANCE_DEG = 2.0;

double POSITION_GAIN = 0.45;
double VELOCITY_GAIN = 1.30;
double MAX_COMMAND_ACCEL = 5.0;
double ATTITUDE_GAIN = 4.0;
double ANGULAR_DAMPING = 1.35;
double MAX_GYRO_COMMAND = 2.0;
double GYRO_SIGN = 1.0;

double STUCK_SECONDS = 15.0;
bool AUTO_REPEAT = false;
double SERVICE_CARGO = 0.05;
double SERVICE_BATTERY = 0.95;
double SERVICE_HYDROGEN = 0.90;
double SERVICE_DELAY = 5.0;

// ---------- Blocks ----------
IMyRemoteControl flight;
IMyTerminalBlock drillRef;
IMyShipConnector dock;
IMyTextPanel lcd;

readonly List<IMyThrust> thrusters = new List<IMyThrust>();
readonly List<IMyGyro> gyros = new List<IMyGyro>();
readonly List<IMyShipDrill> drills = new List<IMyShipDrill>();
readonly List<IMyCargoContainer> cargos = new List<IMyCargoContainer>();
readonly List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
readonly List<IMyGasTank> tanks = new List<IMyGasTank>();
readonly List<IMyShipController> controllers = new List<IMyShipController>();

// Force directions relative to [NMD] Flight: F, B, U, D, L, R
readonly List<IMyThrust>[] thrustGroups = new List<IMyThrust>[6]
{
    new List<IMyThrust>(), new List<IMyThrust>(),
    new List<IMyThrust>(), new List<IMyThrust>(),
    new List<IMyThrust>(), new List<IMyThrust>()
};

// ---------- Mission data ----------
Mode mode = Mode.Idle;
Pose homeDockPose;
Pose homeApproachPose;
Pose minePose;
readonly List<Vector3D> route = new List<Vector3D>();

Vector3D homeApproachFlightPosition;
Vector3D mineApproachFlightPosition;
Vector3D autopilotFinalPosition;

double miningProgress = 0.0;
double lastProgressSample = 0.0;
double stuckTimer = 0.0;
double serviceTimer = 0.0;
double dockFailTimer = 0.0;
double statusTimer = 0.0;
string lastMessage = "Ready";
string scanError = "";
bool ready = false;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    LoadConfig();
    LoadPersistentData();
    ScanBlocks();
    SafeRelease();
    mode = Mode.Idle; // v0.1 never auto-resumes after a reload/restart
    WriteStatus(true);
}

public void Save()
{
    SavePersistentData();
}

public void Main(string argument, UpdateType updateSource)
{
    double dt = Runtime.TimeSinceLastRun.TotalSeconds;
    if (dt <= 0) dt = 1.0 / 60.0;
    if (dt > 5.0) dt = 5.0;

    if (!string.IsNullOrWhiteSpace(argument))
        HandleCommand(argument.Trim());

    if (!ready)
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
        WriteStatus(false);
        return;
    }

    if (IsActiveMode() && IsPlayerControlling())
    {
        PauseMission("Manual control detected");
    }

    switch (mode)
    {
        case Mode.Undock: UpdateUndock(dt); break;
        case Mode.CruiseOut: UpdateCruiseOut(dt); break;
        case Mode.MineApproach: UpdateMineApproach(dt); break;
        case Mode.MineAlign: UpdateMineAlign(dt); break;
        case Mode.Mining: UpdateMining(dt); break;
        case Mode.Retract: UpdateRetract(dt); break;
        case Mode.CruiseHome: UpdateCruiseHome(dt); break;
        case Mode.HomeApproach: UpdateHomeApproach(dt); break;
        case Mode.DockFinal: UpdateDockFinal(dt); break;
        case Mode.Service: UpdateService(dt); break;
    }

    statusTimer += dt;
    if (statusTimer >= 1.0 || updateSource == UpdateType.Trigger || updateSource == UpdateType.Terminal)
    {
        statusTimer = 0;
        WriteStatus(false);
    }
}

// ================================================================
// Commands
// ================================================================
void HandleCommand(string raw)
{
    string cmd = raw.Trim().ToLowerInvariant();

    if (cmd == "scan" || cmd == "rescan")
    {
        SafeRelease();
        LoadConfig();
        ScanBlocks();
        lastMessage = ready ? "Scan complete" : scanError;
        WriteStatus(true);
        return;
    }

    if (cmd == "home dock")
    {
        if (!RequireReady()) return;
        if (dock.Status != MyShipConnectorStatus.Connected)
        {
            lastMessage = "Connect the ship before 'home dock'";
            return;
        }
        homeDockPose = CapturePose(dock);
        SavePersistentData();
        lastMessage = "Home dock pose captured";
        return;
    }

    if (cmd == "home approach")
    {
        if (!RequireReady()) return;
        if (dock.Status == MyShipConnectorStatus.Connected)
        {
            lastMessage = "Disconnect and move to the safe approach point first";
            return;
        }
        homeApproachPose = CapturePose(dock);
        homeApproachFlightPosition = flight.GetPosition();
        SavePersistentData();
        lastMessage = "Home approach captured";
        return;
    }

    if (cmd == "route add")
    {
        if (!RequireReady()) return;
        Vector3D p = flight.GetPosition();
        if (route.Count == 0 || Vector3D.Distance(route[route.Count - 1], p) > 2.0)
        {
            route.Add(p);
            SavePersistentData();
            lastMessage = "Route point " + route.Count + " added";
        }
        else lastMessage = "Route point too close to previous point";
        return;
    }

    if (cmd == "route pop")
    {
        if (route.Count > 0)
        {
            route.RemoveAt(route.Count - 1);
            SavePersistentData();
            lastMessage = "Last route point removed";
        }
        else lastMessage = "Route is already empty";
        return;
    }

    if (cmd == "route clear")
    {
        route.Clear();
        SavePersistentData();
        lastMessage = "Route cleared";
        return;
    }

    if (cmd == "mine capture")
    {
        if (!RequireReady()) return;
        minePose = CapturePose(drillRef);
        mineApproachFlightPosition = flight.GetPosition() - minePose.Forward * MINE_CLEARANCE;
        SavePersistentData();
        lastMessage = "Mine entry and drill direction captured";
        return;
    }

    if (cmd == "start")
    {
        StartMission();
        return;
    }

    if (cmd == "return" || cmd == "recall" || cmd == "home")
    {
        BeginReturn("Return requested");
        return;
    }

    if (cmd == "pause")
    {
        PauseMission("Paused by command");
        return;
    }

    if (cmd == "stop")
    {
        StopMission("Stopped by command");
        return;
    }

    if (cmd == "status")
    {
        WriteStatus(true);
        return;
    }

    lastMessage = "Unknown command: " + raw;
}

bool RequireReady()
{
    if (ready) return true;
    lastMessage = scanError;
    return false;
}

void StartMission()
{
    if (!RequireReady()) return;
    if (!homeDockPose.Valid || !homeApproachPose.Valid || !minePose.Valid)
    {
        lastMessage = "Capture home dock, home approach and mine first";
        return;
    }

    PrepareForFlight();
    miningProgress = 0;
    stuckTimer = 0;
    lastProgressSample = 0;

    if (dock.Status == MyShipConnectorStatus.Connected)
    {
        dock.Disconnect();
        SetMode(Mode.Undock, "Undocking");
    }
    else
    {
        StartCruiseOut();
    }
}

void BeginReturn(string reason)
{
    if (!ready) return;
    lastMessage = reason;

    if (mode == Mode.Mining || mode == Mode.MineAlign || mode == Mode.MineApproach)
    {
        StopDrills();
        SetMode(Mode.Retract, reason + "; retracting");
        return;
    }

    if (mode == Mode.Retract || mode == Mode.CruiseHome || mode == Mode.HomeApproach || mode == Mode.DockFinal)
        return;

    StartCruiseHome();
}

void PauseMission(string reason)
{
    SafeRelease();
    SetMode(Mode.Paused, reason);
}

void StopMission(string reason)
{
    SafeRelease();
    SetMode(Mode.Idle, reason);
}

// ================================================================
// State machine
// ================================================================
void UpdateUndock(double dt)
{
    if (dock.Status == MyShipConnectorStatus.Connected)
        dock.Disconnect();

    bool reached = MovePose(dock, homeApproachPose, APPROACH_SPEED, POSE_POSITION_TOLERANCE, 4.0);
    if (reached)
    {
        SafeReleaseDirectOnly();
        StartCruiseOut();
    }
}

void StartCruiseOut()
{
    PrepareForFlight();
    StartAutopilot(true);
    SetMode(Mode.CruiseOut, "Cruising to mine");
}

void UpdateCruiseOut(double dt)
{
    double dist = Vector3D.Distance(flight.GetPosition(), autopilotFinalPosition);
    if (dist <= ROUTE_ARRIVAL)
    {
        flight.SetAutoPilotEnabled(false);
        SafeReleaseDirectOnly();
        SetMode(Mode.MineApproach, "Aligning at mine approach");
        return;
    }

    if (!flight.IsAutoPilotEnabled && dist > ROUTE_ARRIVAL * 2.0)
        Fail("Autopilot stopped before mine. Distance " + dist.ToString("0") + " m");
}

void UpdateMineApproach(double dt)
{
    Pose p = minePose;
    p.Position = minePose.Position - minePose.Forward * MINE_CLEARANCE;
    bool reached = MovePose(drillRef, p, APPROACH_SPEED, POSE_POSITION_TOLERANCE, 3.0);
    if (reached)
        SetMode(Mode.MineAlign, "Moving to shaft entrance");
}

void UpdateMineAlign(double dt)
{
    bool reached = MovePose(drillRef, minePose, 0.65, POSE_POSITION_TOLERANCE, ANGLE_TOLERANCE_DEG);
    if (reached)
    {
        StartDrills();
        miningProgress = 0;
        lastProgressSample = 0;
        stuckTimer = 0;
        SetMode(Mode.Mining, "Mining");
    }
}

void UpdateMining(double dt)
{
    double cargo = GetCargoRatio();
    double battery = GetBatteryRatio();
    double hydrogen = GetHydrogenRatio();

    if (cargo >= RETURN_CARGO)
    {
        BeginReturn("Cargo limit reached");
        return;
    }
    if (battery <= RETURN_BATTERY)
    {
        BeginReturn("Battery reserve reached");
        return;
    }
    if (tanks.Count > 0 && hydrogen <= RETURN_HYDROGEN)
    {
        BeginReturn("Hydrogen reserve reached");
        return;
    }
    if (!AnyWorkingDrill())
    {
        BeginReturn("No working drill");
        return;
    }

    Vector3D current = drillRef.GetPosition();
    Vector3D fromStart = current - minePose.Position;
    miningProgress = Vector3D.Dot(fromStart, minePose.Forward);

    if (miningProgress >= MINE_DEPTH)
    {
        BeginReturn("Depth complete");
        return;
    }

    // Hold the original shaft axis while moving forward.
    Vector3D axisPoint = minePose.Position + minePose.Forward * miningProgress;
    Vector3D lateralError = axisPoint - current;
    Vector3D desiredVelocity = minePose.Forward * MINE_SPEED + lateralError * 0.55;
    if (desiredVelocity.Length() > MINE_SPEED * 1.6)
        desiredVelocity = Vector3D.Normalize(desiredVelocity) * MINE_SPEED * 1.6;

    ApplyOrientation(drillRef, minePose.Forward, minePose.Up);
    ApplyVelocity(desiredVelocity, MAX_COMMAND_ACCEL);

    if (miningProgress > lastProgressSample + 0.12)
    {
        lastProgressSample = miningProgress;
        stuckTimer = 0;
    }
    else
    {
        stuckTimer += dt;
        if (stuckTimer >= STUCK_SECONDS)
        {
            BeginReturn("Mining progress stopped");
            return;
        }
    }
}

void UpdateRetract(double dt)
{
    StartDrills(); // keeps the exit path clear while backing out
    Pose p = minePose;
    p.Position = minePose.Position - minePose.Forward * MINE_CLEARANCE;
    bool reached = MovePose(drillRef, p, RETRACT_SPEED, POSE_POSITION_TOLERANCE, 3.0);
    if (reached)
    {
        StopDrills();
        SafeReleaseDirectOnly();
        StartCruiseHome();
    }
}

void StartCruiseHome()
{
    if (!homeApproachPose.Valid)
    {
        Fail("Home approach not captured");
        return;
    }
    PrepareForFlight();
    StartAutopilot(false);
    SetMode(Mode.CruiseHome, "Cruising home");
}

void UpdateCruiseHome(double dt)
{
    double dist = Vector3D.Distance(flight.GetPosition(), autopilotFinalPosition);
    if (dist <= ROUTE_ARRIVAL)
    {
        flight.SetAutoPilotEnabled(false);
        SafeReleaseDirectOnly();
        SetMode(Mode.HomeApproach, "Aligning with home approach");
        return;
    }

    if (!flight.IsAutoPilotEnabled && dist > ROUTE_ARRIVAL * 2.0)
        Fail("Autopilot stopped before home. Distance " + dist.ToString("0") + " m");
}

void UpdateHomeApproach(double dt)
{
    bool reached = MovePose(dock, homeApproachPose, APPROACH_SPEED, POSE_POSITION_TOLERANCE, 3.0);
    if (reached)
    {
        dockFailTimer = 0;
        SetMode(Mode.DockFinal, "Final docking");
    }
}

void UpdateDockFinal(double dt)
{
    if (dock.Status == MyShipConnectorStatus.Connected)
    {
        EnterService();
        return;
    }

    bool reached = MovePose(dock, homeDockPose, DOCK_SPEED, DOCK_POSITION_TOLERANCE, ANGLE_TOLERANCE_DEG);

    if (dock.Status == MyShipConnectorStatus.Connectable)
    {
        ZeroThrusters();
        dock.Connect();
    }

    if (reached && dock.Status != MyShipConnectorStatus.Connected)
    {
        dockFailTimer += dt;
        if (dockFailTimer > 8.0)
            Fail("Reached dock pose but connector did not lock");
    }
    else dockFailTimer = 0;
}

void EnterService()
{
    SafeRelease();
    SetBatteryMode(ChargeMode.Recharge);
    SetTankStockpile(true);
    serviceTimer = 0;
    SetMode(Mode.Service, "Docked and servicing");
}

void UpdateService(double dt)
{
    if (dock.Status != MyShipConnectorStatus.Connected)
    {
        SetBatteryMode(ChargeMode.Auto);
        SetTankStockpile(false);
        SetMode(Mode.Paused, "Connector lost during service");
        return;
    }

    if (!AUTO_REPEAT) return;

    if (GetCargoRatio() <= SERVICE_CARGO &&
        GetBatteryRatio() >= SERVICE_BATTERY &&
        (tanks.Count == 0 || GetHydrogenRatio() >= SERVICE_HYDROGEN))
    {
        serviceTimer += dt;
        if (serviceTimer >= SERVICE_DELAY)
            StartMission();
    }
    else serviceTimer = 0;
}

// ================================================================
// Native autopilot for long cruise
// ================================================================
void StartAutopilot(bool outbound)
{
    SafeReleaseDirectOnly();
    flight.ClearWaypoints();
    flight.Direction = Base6Directions.Direction.Forward;
    flight.FlightMode = FlightMode.OneWay;
    flight.SpeedLimit = (float)CRUISE_SPEED;
    flight.SetCollisionAvoidance(COLLISION_AVOIDANCE);
    flight.SetDockingMode(false);
    flight.WaitForFreeWay = true;

    if (outbound)
    {
        int start = FindNearestRouteIndex();
        if (start < 0) start = 0;
        for (int i = start; i < route.Count; i++)
            flight.AddWaypoint(route[i], "SM OUT " + (i + 1));

        autopilotFinalPosition = mineApproachFlightPosition;
        flight.AddWaypoint(autopilotFinalPosition, "SM MINE APPROACH");
    }
    else
    {
        int start = FindNearestRouteIndex();
        if (start < 0) start = route.Count - 1;
        for (int i = start; i >= 0; i--)
            flight.AddWaypoint(route[i], "SM HOME " + (start - i + 1));

        autopilotFinalPosition = homeApproachFlightPosition;
        flight.AddWaypoint(autopilotFinalPosition, "SM HOME APPROACH");
    }

    flight.SetAutoPilotEnabled(true);
}


int FindNearestRouteIndex()
{
    if (route.Count == 0) return -1;
    Vector3D here = flight.GetPosition();
    int best = 0;
    double bestDistance = Vector3D.DistanceSquared(here, route[0]);
    for (int i = 1; i < route.Count; i++)
    {
        double d = Vector3D.DistanceSquared(here, route[i]);
        if (d < bestDistance)
        {
            bestDistance = d;
            best = i;
        }
    }
    return best;
}

// ================================================================
// Direct six-axis flight controller
// ================================================================
bool MovePose(IMyTerminalBlock reference, Pose target, double maxSpeed, double positionTolerance, double angleToleranceDeg)
{
    if (!target.Valid) return false;

    flight.SetAutoPilotEnabled(false);
    flight.DampenersOverride = false;

    double angleError = ApplyOrientation(reference, target.Forward, target.Up);
    Vector3D error = target.Position - reference.GetPosition();
    double distance = error.Length();

    double allowedSpeed = maxSpeed;
    if (angleError > 15.0) allowedSpeed = Math.Min(allowedSpeed, 0.35);
    else if (angleError > 7.0) allowedSpeed = Math.Min(allowedSpeed, 0.80);

    Vector3D desiredVelocity = error * POSITION_GAIN;
    double desiredSpeed = desiredVelocity.Length();
    if (desiredSpeed > allowedSpeed && desiredSpeed > 0.0001)
        desiredVelocity *= allowedSpeed / desiredSpeed;

    ApplyVelocity(desiredVelocity, MAX_COMMAND_ACCEL);

    double speed = flight.GetShipVelocities().LinearVelocity.Length();
    return distance <= positionTolerance &&
           angleError <= angleToleranceDeg &&
           speed <= Math.Max(0.18, maxSpeed * 0.30);
}

double ApplyOrientation(IMyTerminalBlock reference, Vector3D targetForward, Vector3D targetUp)
{
    Vector3D currentForward = reference.WorldMatrix.Forward;
    Vector3D currentUp = reference.WorldMatrix.Up;

    targetForward = SafeNormalize(targetForward, currentForward);
    targetUp = SafeNormalize(targetUp, currentUp);

    Vector3D rotationError = Vector3D.Cross(currentForward, targetForward) +
                             Vector3D.Cross(currentUp, targetUp);

    Vector3D angularVelocity = flight.GetShipVelocities().AngularVelocity;
    Vector3D commandWorld = rotationError * ATTITUDE_GAIN - angularVelocity * ANGULAR_DAMPING;

    double commandLength = commandWorld.Length();
    if (commandLength > MAX_GYRO_COMMAND && commandLength > 0.0001)
        commandWorld *= MAX_GYRO_COMMAND / commandLength;

    commandWorld *= GYRO_SIGN;

    for (int i = 0; i < gyros.Count; i++)
    {
        IMyGyro g = gyros[i];
        MatrixD transpose = MatrixD.Transpose(g.WorldMatrix);
        Vector3D local = Vector3D.TransformNormal(commandWorld, transpose);
        g.GyroOverride = true;
        g.Pitch = (float)local.X;
        g.Yaw = (float)local.Y;
        g.Roll = (float)local.Z;
    }

    double f = Math.Acos(Clamp(Vector3D.Dot(currentForward, targetForward), -1, 1));
    double u = Math.Acos(Clamp(Vector3D.Dot(currentUp, targetUp), -1, 1));
    return Math.Max(f, u) * 180.0 / Math.PI;
}

void ApplyVelocity(Vector3D desiredVelocity, double maxAccel)
{
    Vector3D currentVelocity = flight.GetShipVelocities().LinearVelocity;
    Vector3D desiredAcceleration = (desiredVelocity - currentVelocity) * VELOCITY_GAIN;

    double accelLength = desiredAcceleration.Length();
    if (accelLength > maxAccel && accelLength > 0.0001)
        desiredAcceleration *= maxAccel / accelLength;

    double mass = flight.CalculateShipMass().PhysicalMass;
    Vector3D gravity = flight.GetNaturalGravity();
    Vector3D requiredForce = (desiredAcceleration - gravity) * mass;
    ApplyForce(requiredForce);
}

void ApplyForce(Vector3D requiredForce)
{
    Vector3D[] axes = new Vector3D[6]
    {
        flight.WorldMatrix.Forward,
        flight.WorldMatrix.Backward,
        flight.WorldMatrix.Up,
        flight.WorldMatrix.Down,
        flight.WorldMatrix.Left,
        flight.WorldMatrix.Right
    };

    for (int group = 0; group < 6; group++)
    {
        double need = Vector3D.Dot(requiredForce, axes[group]);
        if (need < 0) need = 0;

        double available = 0;
        List<IMyThrust> list = thrustGroups[group];
        for (int i = 0; i < list.Count; i++)
            if (list[i].IsWorking) available += list[i].MaxEffectiveThrust;

        float pct = available > 1 ? (float)Clamp(need / available, 0, 1) : 0f;
        for (int i = 0; i < list.Count; i++)
            list[i].ThrustOverridePercentage = pct;
    }
}

void SafeReleaseDirectOnly()
{
    ZeroThrusters();
    ReleaseGyros();
    if (flight != null) flight.DampenersOverride = true;
}

void SafeRelease()
{
    if (flight != null)
    {
        flight.SetAutoPilotEnabled(false);
        flight.ClearWaypoints();
        flight.DampenersOverride = true;
    }
    ZeroThrusters();
    ReleaseGyros();
    StopDrills();
}

void ZeroThrusters()
{
    for (int i = 0; i < thrusters.Count; i++)
        thrusters[i].ThrustOverridePercentage = 0f;
}

void ReleaseGyros()
{
    for (int i = 0; i < gyros.Count; i++)
    {
        gyros[i].Pitch = 0f;
        gyros[i].Yaw = 0f;
        gyros[i].Roll = 0f;
        gyros[i].GyroOverride = false;
    }
}

// ================================================================
// Ship systems
// ================================================================
void PrepareForFlight()
{
    SetBatteryMode(ChargeMode.Auto);
    SetTankStockpile(false);
    if (dock != null) dock.Enabled = true;
    if (flight != null) flight.DampenersOverride = true;
}

void StartDrills()
{
    for (int i = 0; i < drills.Count; i++) drills[i].Enabled = true;
}

void StopDrills()
{
    for (int i = 0; i < drills.Count; i++) drills[i].Enabled = false;
}

bool AnyWorkingDrill()
{
    for (int i = 0; i < drills.Count; i++)
        if (drills[i].IsWorking) return true;
    return false;
}

void SetBatteryMode(ChargeMode chargeMode)
{
    for (int i = 0; i < batteries.Count; i++)
        batteries[i].ChargeMode = chargeMode;
}

void SetTankStockpile(bool value)
{
    for (int i = 0; i < tanks.Count; i++)
        tanks[i].Stockpile = value;
}

double GetCargoRatio()
{
    double current = 0;
    double maximum = 0;

    for (int i = 0; i < cargos.Count; i++)
        AddInventoryVolume(cargos[i], ref current, ref maximum);
    for (int i = 0; i < drills.Count; i++)
        AddInventoryVolume(drills[i], ref current, ref maximum);

    return maximum > 0 ? current / maximum : 0;
}

void AddInventoryVolume(IMyTerminalBlock block, ref double current, ref double maximum)
{
    if (!block.HasInventory) return;
    for (int i = 0; i < block.InventoryCount; i++)
    {
        IMyInventory inv = block.GetInventory(i);
        current += (double)inv.CurrentVolume;
        maximum += (double)inv.MaxVolume;
    }
}

double GetBatteryRatio()
{
    double current = 0;
    double maximum = 0;
    for (int i = 0; i < batteries.Count; i++)
    {
        current += batteries[i].CurrentStoredPower;
        maximum += batteries[i].MaxStoredPower;
    }
    return maximum > 0 ? current / maximum : 1;
}

double GetHydrogenRatio()
{
    if (tanks.Count == 0) return 1;
    double sum = 0;
    for (int i = 0; i < tanks.Count; i++) sum += tanks[i].FilledRatio;
    return sum / tanks.Count;
}

bool IsPlayerControlling()
{
    for (int i = 0; i < controllers.Count; i++)
        if (controllers[i].IsUnderControl) return true;
    return false;
}

// ================================================================
// Scan and validation
// ================================================================
void ScanBlocks()
{
    ready = false;
    scanError = "";

    flight = FindExact<IMyRemoteControl>(FLIGHT_NAME);
    drillRef = FindExact<IMyTerminalBlock>(DRILL_REF_NAME);
    dock = FindExact<IMyShipConnector>(DOCK_NAME);
    lcd = FindExact<IMyTextPanel>(LCD_NAME);

    thrusters.Clear();
    gyros.Clear();
    drills.Clear();
    cargos.Clear();
    batteries.Clear();
    tanks.Clear();
    controllers.Clear();

    GridTerminalSystem.GetBlocksOfType(thrusters, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(gyros, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(drills, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(cargos, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(batteries, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(tanks, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(controllers, b => b.IsSameConstructAs(Me));

    if (flight == null) scanError += "Missing Remote Control '" + FLIGHT_NAME + "'\n";
    if (drillRef == null) scanError += "Missing drill reference '" + DRILL_REF_NAME + "'\n";
    if (dock == null) scanError += "Missing connector '" + DOCK_NAME + "'\n";
    if (thrusters.Count == 0) scanError += "No thrusters found\n";
    if (gyros.Count == 0) scanError += "No gyros found\n";
    if (drills.Count == 0) scanError += "No drills found\n";

    if (flight != null && thrusters.Count > 0)
    {
        BuildThrustGroups();
        string missing = MissingThrustDirections();
        if (missing.Length > 0) scanError += "Missing thrust directions: " + missing + "\n";
    }

    ready = scanError.Length == 0;
    if (lcd != null)
    {
        lcd.ContentType = ContentType.TEXT_AND_IMAGE;
        lcd.Alignment = TextAlignment.LEFT;
    }
}

void BuildThrustGroups()
{
    for (int i = 0; i < 6; i++) thrustGroups[i].Clear();

    Vector3D[] axes = new Vector3D[6]
    {
        flight.WorldMatrix.Forward,
        flight.WorldMatrix.Backward,
        flight.WorldMatrix.Up,
        flight.WorldMatrix.Down,
        flight.WorldMatrix.Left,
        flight.WorldMatrix.Right
    };

    for (int i = 0; i < thrusters.Count; i++)
    {
        IMyThrust t = thrusters[i];
        Vector3D forceDirection = t.WorldMatrix.Backward;
        int best = 0;
        double bestDot = -2;
        for (int a = 0; a < 6; a++)
        {
            double d = Vector3D.Dot(forceDirection, axes[a]);
            if (d > bestDot)
            {
                bestDot = d;
                best = a;
            }
        }
        thrustGroups[best].Add(t);
    }
}

string MissingThrustDirections()
{
    string[] names = { "forward", "backward", "up", "down", "left", "right" };
    string result = "";
    for (int i = 0; i < 6; i++)
    {
        if (thrustGroups[i].Count == 0)
        {
            if (result.Length > 0) result += ", ";
            result += names[i];
        }
    }
    return result;
}

T FindExact<T>(string name) where T : class, IMyTerminalBlock
{
    List<T> found = new List<T>();
    GridTerminalSystem.GetBlocksOfType(found, b => b.IsSameConstructAs(Me) && b.CustomName == name);
    return found.Count > 0 ? found[0] : null;
}

// ================================================================
// Config and persistence
// ================================================================
void LoadConfig()
{
    MyIni ini = new MyIni();
    MyIniParseResult parse;
    bool parsed = ini.TryParse(Me.CustomData, out parse);
    if (!parsed) ini.Clear();

    bool changed = false;
    changed |= Ensure(ini, "Blocks", "Flight", FLIGHT_NAME);
    changed |= Ensure(ini, "Blocks", "DrillReference", DRILL_REF_NAME);
    changed |= Ensure(ini, "Blocks", "DockConnector", DOCK_NAME);
    changed |= Ensure(ini, "Blocks", "LCD", LCD_NAME);

    changed |= Ensure(ini, "Cruise", "Speed", CRUISE_SPEED);
    changed |= Ensure(ini, "Cruise", "CollisionAvoidance", COLLISION_AVOIDANCE);

    changed |= Ensure(ini, "Mining", "Depth", MINE_DEPTH);
    changed |= Ensure(ini, "Mining", "Speed", MINE_SPEED);
    changed |= Ensure(ini, "Mining", "RetractSpeed", RETRACT_SPEED);
    changed |= Ensure(ini, "Mining", "Clearance", MINE_CLEARANCE);
    changed |= Ensure(ini, "Mining", "StuckSeconds", STUCK_SECONDS);

    changed |= Ensure(ini, "Return", "CargoRatio", RETURN_CARGO);
    changed |= Ensure(ini, "Return", "BatteryRatio", RETURN_BATTERY);
    changed |= Ensure(ini, "Return", "HydrogenRatio", RETURN_HYDROGEN);

    changed |= Ensure(ini, "Docking", "ApproachSpeed", APPROACH_SPEED);
    changed |= Ensure(ini, "Docking", "FinalSpeed", DOCK_SPEED);
    changed |= Ensure(ini, "Docking", "RouteArrival", ROUTE_ARRIVAL);

    changed |= Ensure(ini, "Service", "AutoRepeat", AUTO_REPEAT);
    changed |= Ensure(ini, "Service", "CargoEmptyRatio", SERVICE_CARGO);
    changed |= Ensure(ini, "Service", "BatteryReadyRatio", SERVICE_BATTERY);
    changed |= Ensure(ini, "Service", "HydrogenReadyRatio", SERVICE_HYDROGEN);

    changed |= Ensure(ini, "Control", "GyroSign", GYRO_SIGN);

    FLIGHT_NAME = ini.Get("Blocks", "Flight").ToString(FLIGHT_NAME);
    DRILL_REF_NAME = ini.Get("Blocks", "DrillReference").ToString(DRILL_REF_NAME);
    DOCK_NAME = ini.Get("Blocks", "DockConnector").ToString(DOCK_NAME);
    LCD_NAME = ini.Get("Blocks", "LCD").ToString(LCD_NAME);

    CRUISE_SPEED = ini.Get("Cruise", "Speed").ToDouble(CRUISE_SPEED);
    COLLISION_AVOIDANCE = ini.Get("Cruise", "CollisionAvoidance").ToBoolean(COLLISION_AVOIDANCE);

    MINE_DEPTH = ini.Get("Mining", "Depth").ToDouble(MINE_DEPTH);
    MINE_SPEED = ini.Get("Mining", "Speed").ToDouble(MINE_SPEED);
    RETRACT_SPEED = ini.Get("Mining", "RetractSpeed").ToDouble(RETRACT_SPEED);
    MINE_CLEARANCE = ini.Get("Mining", "Clearance").ToDouble(MINE_CLEARANCE);
    STUCK_SECONDS = ini.Get("Mining", "StuckSeconds").ToDouble(STUCK_SECONDS);

    RETURN_CARGO = ini.Get("Return", "CargoRatio").ToDouble(RETURN_CARGO);
    RETURN_BATTERY = ini.Get("Return", "BatteryRatio").ToDouble(RETURN_BATTERY);
    RETURN_HYDROGEN = ini.Get("Return", "HydrogenRatio").ToDouble(RETURN_HYDROGEN);

    APPROACH_SPEED = ini.Get("Docking", "ApproachSpeed").ToDouble(APPROACH_SPEED);
    DOCK_SPEED = ini.Get("Docking", "FinalSpeed").ToDouble(DOCK_SPEED);
    ROUTE_ARRIVAL = ini.Get("Docking", "RouteArrival").ToDouble(ROUTE_ARRIVAL);

    AUTO_REPEAT = ini.Get("Service", "AutoRepeat").ToBoolean(AUTO_REPEAT);
    SERVICE_CARGO = ini.Get("Service", "CargoEmptyRatio").ToDouble(SERVICE_CARGO);
    SERVICE_BATTERY = ini.Get("Service", "BatteryReadyRatio").ToDouble(SERVICE_BATTERY);
    SERVICE_HYDROGEN = ini.Get("Service", "HydrogenReadyRatio").ToDouble(SERVICE_HYDROGEN);

    GYRO_SIGN = ini.Get("Control", "GyroSign").ToDouble(GYRO_SIGN);

    if (changed || !parsed) Me.CustomData = ini.ToString();
}

bool Ensure(MyIni ini, string section, string key, string value)
{
    if (ini.ContainsKey(section, key)) return false;
    ini.Set(section, key, value);
    return true;
}

bool Ensure(MyIni ini, string section, string key, double value)
{
    if (ini.ContainsKey(section, key)) return false;
    ini.Set(section, key, value);
    return true;
}

bool Ensure(MyIni ini, string section, string key, bool value)
{
    if (ini.ContainsKey(section, key)) return false;
    ini.Set(section, key, value);
    return true;
}

void SavePersistentData()
{
    MyIni ini = new MyIni();
    ini.Set("Meta", "Version", VERSION);

    SavePose(ini, "HomeDock", homeDockPose);
    SavePose(ini, "HomeApproach", homeApproachPose);
    SavePose(ini, "Mine", minePose);

    ini.Set("HomeApproach", "FlightPosition", VecToString(homeApproachFlightPosition));
    ini.Set("Mine", "ApproachFlightPosition", VecToString(mineApproachFlightPosition));

    ini.Set("Route", "Count", route.Count);
    for (int i = 0; i < route.Count; i++)
        ini.Set("Route", "P" + i, VecToString(route[i]));

    Storage = ini.ToString();
}

void LoadPersistentData()
{
    if (string.IsNullOrWhiteSpace(Storage)) return;

    MyIni ini = new MyIni();
    MyIniParseResult result;
    if (!ini.TryParse(Storage, out result))
    {
        lastMessage = "Stored mission data could not be parsed";
        return;
    }

    homeDockPose = LoadPose(ini, "HomeDock");
    homeApproachPose = LoadPose(ini, "HomeApproach");
    minePose = LoadPose(ini, "Mine");

    homeApproachFlightPosition = ParseVector(ini.Get("HomeApproach", "FlightPosition").ToString(""));
    mineApproachFlightPosition = ParseVector(ini.Get("Mine", "ApproachFlightPosition").ToString(""));

    route.Clear();
    int count = ini.Get("Route", "Count").ToInt32(0);
    for (int i = 0; i < count; i++)
        route.Add(ParseVector(ini.Get("Route", "P" + i).ToString("")));
}

void SavePose(MyIni ini, string section, Pose p)
{
    ini.Set(section, "Valid", p.Valid);
    ini.Set(section, "Position", VecToString(p.Position));
    ini.Set(section, "Forward", VecToString(p.Forward));
    ini.Set(section, "Up", VecToString(p.Up));
}

Pose LoadPose(MyIni ini, string section)
{
    Pose p = new Pose();
    p.Valid = ini.Get(section, "Valid").ToBoolean(false);
    p.Position = ParseVector(ini.Get(section, "Position").ToString(""));
    p.Forward = ParseVector(ini.Get(section, "Forward").ToString(""));
    p.Up = ParseVector(ini.Get(section, "Up").ToString(""));
    return p;
}

Pose CapturePose(IMyTerminalBlock block)
{
    Pose p = new Pose();
    p.Valid = true;
    p.Position = block.GetPosition();
    p.Forward = block.WorldMatrix.Forward;
    p.Up = block.WorldMatrix.Up;
    return p;
}

string VecToString(Vector3D v)
{
    return v.X.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "|" +
           v.Y.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "|" +
           v.Z.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
}

Vector3D ParseVector(string text)
{
    if (string.IsNullOrWhiteSpace(text)) return Vector3D.Zero;
    string[] p = text.Split('|');
    if (p.Length != 3) return Vector3D.Zero;

    double x, y, z;
    if (!double.TryParse(p[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x)) return Vector3D.Zero;
    if (!double.TryParse(p[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y)) return Vector3D.Zero;
    if (!double.TryParse(p[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out z)) return Vector3D.Zero;
    return new Vector3D(x, y, z);
}

// ================================================================
// Status, modes and helpers
// ================================================================
void SetMode(Mode newMode, string message)
{
    mode = newMode;
    lastMessage = message;

    if (newMode == Mode.Undock || newMode == Mode.MineApproach || newMode == Mode.MineAlign ||
        newMode == Mode.Mining || newMode == Mode.Retract || newMode == Mode.HomeApproach ||
        newMode == Mode.DockFinal)
        Runtime.UpdateFrequency = UpdateFrequency.Update1;
    else if (newMode == Mode.CruiseOut || newMode == Mode.CruiseHome)
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
    else
        Runtime.UpdateFrequency = UpdateFrequency.Update100;

    SavePersistentData();
    WriteStatus(true);
}

bool IsActiveMode()
{
    return mode == Mode.Undock || mode == Mode.CruiseOut || mode == Mode.MineApproach ||
           mode == Mode.MineAlign || mode == Mode.Mining || mode == Mode.Retract ||
           mode == Mode.CruiseHome || mode == Mode.HomeApproach || mode == Mode.DockFinal;
}

void Fail(string message)
{
    SafeRelease();
    mode = Mode.Error;
    lastMessage = message;
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    SavePersistentData();
    WriteStatus(true);
}

void WriteStatus(bool force)
{
    string text =
        "NOMAD CORE v" + VERSION + "\n" +
        "Mode: " + mode + "\n" +
        "Ready: " + (ready ? "YES" : "NO") + "\n" +
        "Home dock: " + YesNo(homeDockPose.Valid) + "\n" +
        "Home approach: " + YesNo(homeApproachPose.Valid) + "\n" +
        "Mine: " + YesNo(minePose.Valid) + "\n" +
        "Route points: " + route.Count + "\n" +
        "Cargo: " + (GetCargoRatio() * 100).ToString("0.0") + "%\n" +
        "Battery: " + (GetBatteryRatio() * 100).ToString("0.0") + "%\n" +
        "Hydrogen: " + (GetHydrogenRatio() * 100).ToString("0.0") + "%\n" +
        "Mine progress: " + miningProgress.ToString("0.0") + " / " + MINE_DEPTH.ToString("0.0") + " m\n" +
        "Runtime: " + Runtime.LastRunTimeMs.ToString("0.000") + " ms\n" +
        "Message: " + lastMessage + "\n";

    if (!ready && scanError.Length > 0)
        text += "\nSCAN ERROR\n" + scanError;

    Echo(text);
    if (lcd != null) lcd.WriteText(text, false);
}

string YesNo(bool value)
{
    return value ? "YES" : "NO";
}

Vector3D SafeNormalize(Vector3D value, Vector3D fallback)
{
    if (value.LengthSquared() < 1e-8) return fallback;
    return Vector3D.Normalize(value);
}

double Clamp(double value, double min, double max)
{
    if (value < min) return min;
    if (value > max) return max;
    return value;
}