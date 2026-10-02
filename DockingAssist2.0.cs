// Docking Assist 2.0 - install in the SHIP programmable block. C# 6. See USER_GUIDE.md.
// NEXT -> SELECT (no name required) -> DOCK | STOP | SCAN | CONNECT | RELOAD
const string NETWORK = "DOCKING_HOME_V2";
const string CONNECTOR_NAME = "Docking Connector";
const string CAMERA_NAME = "Docking Camera";
const string CONTROLLER_NAME = "Docking Cockpit";
const double MAX_SPEED = 10.0, FINAL_SPEED = 0.20, FINAL_GAP = 0.08;
const double SLOW_ZONE = 2.5;
const double DATA_TIMEOUT = 2.0, LEASE_TIMEOUT = 2.0;
const double MAX_RANGE = 150;
// Circular connectors do not require matching rotation around the docking axis.
const bool MATCH_PORT_ROLL = false;
IMyShipConnector connector;
IMyShipController cockpit;
IMyCameraBlock camera;
string cameraError = "";
IMyBroadcastListener listener;
List<IMyThrust> thrusters = new List<IMyThrust>();
List<IMyGyro> gyros = new List<IMyGyro>();
List<IMyTextSurface> screens = new List<IMyTextSurface>();
Dictionary<long, Port> ports = new Dictionary<long, Port>();
List<long> listing = new List<long>();
long selected;
bool selectionConfirmed;
double now, nextRequest, grantedAt = -100, startedAt;
double approachDistance;
string token = "", status = "Ready: NEXT -> SELECT -> DOCK", setupError = "";
int serial;
bool controlling, scanPending;
bool closeStart;
Vector3D holdPoint;
Vector3D? fallbackPoint;
Vector3D fallbackForward, fallbackUp;
double fallbackAt;
float previousPull;
double axial, lateral, angle;
enum Phase { Idle, Reserve, Align, Approach, Final }
Phase phase = Phase.Idle;
class Port
{
    public long Id, Grid, Source, Sequence, Owner;
    public string Name, Station;
    public Vector3D Face, Forward, Up;
    public double Seen;
    public bool Available;
    public int Status;
}

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    listener = IGC.RegisterBroadcastListener(NETWORK);
    Reload();
    // A sequence never resumes automatically after loading or recompiling.
    // Recover only the actuators managed by this PB (same grid).
    if (Storage.StartsWith("ACTIVE|"))
    {
        foreach (var t in thrusters) t.ThrustOverridePercentage = 0;
        foreach (var g in gyros) { g.GyroOverride = false; g.Pitch = g.Yaw = g.Roll = 0; }
        float pull;
        if (connector != null && float.TryParse(Storage.Substring(7), System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out pull)) connector.PullStrength = pull;
        if (cockpit != null) cockpit.DampenersOverride = true;
        status = "Sequence stopped on reload. Use DOCK to start again.";
    }
    Storage = "";
}
public void Save() { }
public void Main(string argument, UpdateType updateSource)
{
    double dt = Math.Max(0, Runtime.TimeSinceLastRun.TotalSeconds);
    now += dt;
    try
    {
        Receive();
        if (!string.IsNullOrWhiteSpace(argument))
        {
            Command(argument.Trim());
            // Toolbar commands provide immediate feedback on the display.
            if ((updateSource & UpdateType.Update10) == 0) Draw();
        }
        if ((updateSource & UpdateType.Update10) != 0)
        {
            if (phase != Phase.Idle && dt > 0.75) Stop("Aborted: script updates are too slow");
            if (phase != Phase.Idle) AutoStep();
            if (scanPending) Scan();
            Draw();
        }
    }
    catch (Exception e) { Stop("ERROR: " + e.Message); Echo(status); }
}
void Reload()
{
    setupError = "";
    var cons = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(cons, b => b.CubeGrid == Me.CubeGrid
        && (b.CustomName == CONNECTOR_NAME || b.CustomName.IndexOf("[Docking]", StringComparison.OrdinalIgnoreCase) >= 0));
    connector = cons.Count == 1 ? cons[0] : null;
    if (connector == null) setupError = cons.Count == 0
        ? "One [Docking] connector is required on the PB's grid."
        : "Multiple docking connectors: tag/name only one ship connector.";
    var controls = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controls, b => b.CubeGrid == Me.CubeGrid);
    cockpit = controls.Find(b => b.CustomName == CONTROLLER_NAME);
    if (cockpit == null) cockpit = controls.Find(b => b.IsUnderControl);
    if (cockpit == null && controls.Count == 1) cockpit = controls[0];
    if (cockpit == null) setupError += " Name the controller '" + CONTROLLER_NAME + "'.";
    var cameras = new List<IMyCameraBlock>();
    GridTerminalSystem.GetBlocksOfType(cameras, b => b.IsSameConstructAs(Me)
        && (b.CustomName == CAMERA_NAME || b.CustomName.IndexOf("[Docking]", StringComparison.OrdinalIgnoreCase) >= 0));
    camera = cameras.Count == 1 ? cameras[0] : null;
    cameraError = cameras.Count > 1
        ? "Multiple Docking cameras: keep the tag/name on only one camera."
        : "A [Docking] camera is required on the ship. Run RELOAD after renaming.";
    if (camera != null) camera.EnableRaycast = true;
    GridTerminalSystem.GetBlocksOfType(thrusters, b => b.CubeGrid == Me.CubeGrid);
    GridTerminalSystem.GetBlocksOfType(gyros, b => b.CubeGrid == Me.CubeGrid);
    screens.Clear();
    var panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, b => b.IsSameConstructAs(Me) && (b.CustomName == "Docking LCD" || b.CustomName.Contains("[Docking]")));
    foreach (var p in panels) screens.Add(p);
    if (screens.Count == 0) screens.Add(Me.GetSurface(0));
    foreach (var s in screens)
    {
        s.ContentType = ContentType.SCRIPT;
        s.Script = "";
        s.Font = "Monospace"; s.FontSize = 0.65f; s.TextPadding = 2;
        s.FontColor = Color.White; s.BackgroundColor = new Color(5, 10, 18);
    }
}
void Command(string command)
{
    int space = command.IndexOf(' ');
    string cmd = (space < 0 ? command : command.Substring(0, space)).ToUpperInvariant();
    string arg = space < 0 ? "" : command.Substring(space + 1).Trim();
    if (cmd == "STOP" || cmd == "ABORT") { Stop("Aborted by pilot"); return; }
    if (cmd == "RELOAD") { Stop("Block configuration refreshed"); Reload(); return; }
    if (phase != Phase.Idle) { status = "Sequence active: use STOP to abort"; return; }
    if (cmd == "NEXT")
    {
        RefreshList();
        selectionConfirmed = false;
        if (listing.Count > 0) selected = listing[(listing.IndexOf(selected) + 1) % listing.Count];
        else selected = 0;
        status = selected == 0 ? "No IGC ports received" : "Port preview: press SELECT to confirm";
        fallbackPoint = null; return;
    }
    if (cmd == "SELECT" || cmd == "DOCK" || cmd == "ATTRACCO")
    {
        if (arg.Length > 0 && !SelectPort(arg)) return;
        if (cmd == "SELECT") ConfirmSelection();
        else { ConfirmSelection(); Start(); }
        return;
    }
    if (cmd == "SCAN")
    {
        fallbackPoint = null; selected = 0; selectionConfirmed = false; scanPending = true;
        status = "Aim the camera at the center of the target connector"; return;
    }
    if (cmd == "CONNECT")
    {
        if (connector != null && connector.Status == MyShipConnectorStatus.Connectable) connector.Connect();
        status = "Single manual connection attempt"; return;
    }
    if (cmd == "LIST") { RefreshList(); return; }
    status = "Commands: NEXT, SELECT, DOCK, STOP, SCAN";
}
void ConfirmSelection()
{
    Port p;
    if (!ports.TryGetValue(selected, out p) || now - p.Seen > DATA_TIMEOUT)
    {
        selectionConfirmed = false;
        status = "No valid port: use NEXT, then SELECT";
        return;
    }
    selectionConfirmed = true;
    status = "PORT SELECTED: " + p.Station + " / " + p.Name + " - press DOCK to start";
}
void RefreshList()
{
    listing.Clear();
    foreach (var p in ports.Values)
        if (now - p.Seen <= DATA_TIMEOUT && p.Status == (int)MyShipConnectorStatus.Unconnected && (p.Owner == 0 || p.Owner == IGC.Me)) listing.Add(p.Id);
    listing.Sort();
}
bool SelectPort(string arg)
{
    RefreshList();
    long id;
    if (long.TryParse(arg, out id) && listing.Contains(id)) { selected = id; selectionConfirmed = false; fallbackPoint = null; return true; }
    var matches = new List<long>();
    foreach (long key in listing)
    {
        Port p = ports[key];
        if (p.Name.Equals(arg, StringComparison.OrdinalIgnoreCase)
            || (p.Station + "/" + p.Name).Equals(arg, StringComparison.OrdinalIgnoreCase)) matches.Add(key);
    }
    if (matches.Count != 1) { status = "Port missing or ambiguous: use its ID from LIST"; return false; }
    selected = matches[0]; selectionConfirmed = false; fallbackPoint = null; return true;
}
void Start()
{
    if (setupError.Length > 0) { status = setupError; return; }
    if (!connector.IsWorking || !cockpit.IsWorking) { status = "Connector/controller is not operational"; return; }
    if (connector.Status == MyShipConnectorStatus.Connected) { status = "Already connected"; return; }
    Port p;
    if (!ports.TryGetValue(selected, out p) || now - p.Seen > DATA_TIMEOUT)
    { status = "Select an IGC port. Without IGC: SCAN for manual guidance."; return; }
    if (!p.Available || p.Status == (int)MyShipConnectorStatus.Connected || (p.Owner != 0 && p.Owner != IGC.Me))
    { status = "Port occupied, reserved, or not on a static grid"; return; }
    Vector3D delta = connector.GetPosition() - p.Face;
    double along = Vector3D.Dot(delta, p.Forward);
    double distance = (connector.GetPosition() + connector.WorldMatrix.Forward * FaceOffset(connector) - p.Face).Length();
    double turn = RotationError(connector.WorldMatrix, -p.Forward, DockUp(p)).Length();
    if (distance > MAX_RANGE)
    { status = "DOCK NOT STARTED: port is more than 150 m away"; return; }
    if (along < FaceOffset(connector) + FINAL_GAP)
    { status = "DOCK NOT STARTED: behind the port or too close to its face"; return; }
    closeStart = distance < StandOff();
    if (closeStart && turn > 0.025)
    {
        status = "DOCK NOT STARTED: align orientation only.\nLateral centering will be automatic.\nDistance " + distance.ToString("F1") + " m; rotation clearance threshold " + StandOff().ToString("F1") + " m.";
        return;
    }
    if (gyros.Find(g => g.IsWorking) == null || cockpit.GetShipSpeed() > 2
        || cockpit.GetShipVelocities().AngularVelocity.Length() > 0.1)
    { status = "Working gyros and initial speed <= 2 m/s are required"; return; }
    foreach (var t in thrusters) if (t.ThrustOverridePercentage > 0) { status = "Clear existing thruster overrides"; return; }
    foreach (var g in gyros) if (g.GyroOverride) { status = "Disable other gyro overrides"; return; }
    if (HasPilotInput()) { status = "Release movement and rotation controls"; return; }
    double capacity = AccelerationMargin();
    if (capacity < 0.3) { status = "Insufficient thrust in all six directions / gravity margin"; return; }
    previousPull = connector.PullStrength;
    // At close range, center at the current depth (at least 2.5 m) without
    // returning to the distant approach point needed for a large rotation.
    approachDistance = closeStart ? Math.Max(SLOW_ZONE, along - FaceOffset(connector)) : StandOff();
    Storage = "ACTIVE|" + previousPull.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
    connector.PullStrength = 0; // Reduce magnetic pull until the controlled approach is complete.
    controlling = true;
    cockpit.DampenersOverride = false;
    holdPoint = connector.GetPosition();
    token = IGC.Me + ":" + (++serial) + ":" + DateTime.UtcNow.Ticks;
    phase = Phase.Reserve; startedAt = now; grantedAt = -100; nextRequest = now;
    fallbackPoint = null;
    status = "Requesting port reservation";
}
void Receive()
{
    int budget = 60;
    while (listener.HasPendingMessage && budget-- > 0)
    {
        var msg = listener.AcceptMessage();
        if (!(msg.Data is string)) continue;
        string data = (string)msg.Data;
        long hidden;
        Port hiddenPort;
        if (data.StartsWith("HIDE|") && long.TryParse(data.Substring(5), out hidden))
        {
            if (ports.TryGetValue(hidden, out hiddenPort) && hiddenPort.Source == msg.Source && !(selected == hidden && phase != Phase.Idle))
            { ports.Remove(hidden); if (selected == hidden) selectionConfirmed = false; }
            continue;
        }
        Port p;
        if (!ParsePort((string)msg.Data, out p)) continue;
        if (p.Grid == Me.CubeGrid.EntityId || msg.Source == IGC.Me) continue;
        Port old;
        if (ports.TryGetValue(p.Id, out old))
        {
            if (old.Source != msg.Source && now - old.Seen < DATA_TIMEOUT) continue;
            if (selected == p.Id && phase != Phase.Idle && old.Source != msg.Source)
                Stop("Port transmitter changed");
            // Stations are static: abort if the port pose changes during docking.
            if (selected == p.Id && phase != Phase.Idle &&
                ((old.Face - p.Face).Length() > 0.02 || Vector3D.Dot(old.Forward, p.Forward) < 0.9999
                 || Vector3D.Dot(old.Up, p.Up) < 0.9999)) Stop("Port moved: docking aborted");
        }
        if (ports.Count >= 256 && !ports.ContainsKey(p.Id)) continue;
        p.Source = msg.Source; p.Seen = now; ports[p.Id] = p;
    }
    budget = 40;
    while (IGC.UnicastListener.HasPendingMessage && budget-- > 0)
    {
        var msg = IGC.UnicastListener.AcceptMessage();
        if (msg.Tag != NETWORK || !(msg.Data is string) || phase == Phase.Idle) continue;
        string[] f = ((string)msg.Data).Split('|');
        Port p;
        Port update;
        if (ParsePort((string)msg.Data, out update))
        {
            if (update.Id == selected && ports.TryGetValue(selected, out p) && msg.Source == p.Source && update.Owner == IGC.Me)
            {
                if ((p.Face - update.Face).Length() > 0.02 || Vector3D.Dot(p.Forward, update.Forward) < 0.9999 || Vector3D.Dot(p.Up, update.Up) < 0.9999)
                    Stop("Port moved: docking aborted");
                else { update.Source = msg.Source; update.Seen = now; ports[selected] = update; }
            }
            continue;
        }
        if (f.Length != 3 || !ports.TryGetValue(selected, out p) || msg.Source != p.Source || f[1] != selected.ToString() || f[2] != token) continue;
        if (f[0] == "GRANT") grantedAt = now;
        else if (f[0] == "DENY") Stop("Port reservation denied");
    }
    var expired = new List<long>();
    foreach (var p in ports.Values) if (now - p.Seen > 10 && p.Id != selected) expired.Add(p.Id);
    foreach (long id in expired) ports.Remove(id);
}
bool ParsePort(string packet, out Port p)
{
    p = new Port();
    if (packet.Length > 2000) return false;
    string[] f = packet.Split('|');
    if (f.Length != 12 || f[0] != "PORT" || !long.TryParse(f[1], out p.Id) || !long.TryParse(f[2], out p.Grid)
        || !long.TryParse(f[3], out p.Sequence) || !ReadVec(f[6], out p.Face) || !ReadVec(f[7], out p.Forward)
        || !ReadVec(f[8], out p.Up) || !int.TryParse(f[10], out p.Status) || !long.TryParse(f[11], out p.Owner)) return false;
    if (p.Id <= 0 || p.Face.Length() > 1e9 || Math.Abs(p.Forward.Length() - 1) > 0.01
        || Math.Abs(p.Up.Length() - 1) > 0.01 || Math.Abs(Vector3D.Dot(p.Up, p.Forward)) > 0.01
        || p.Status < 0 || p.Status > 2 || (f[9] != "0" && f[9] != "1")) return false;
    p.Forward = Vector3D.Normalize(p.Forward);
    p.Up = Vector3D.Normalize(p.Up - p.Forward * Vector3D.Dot(p.Up, p.Forward));
    p.Name = f[5]; p.Station = f[4]; p.Available = f[9] == "1";
    return true;
}
bool ReadVec(string s, out Vector3D v)
{
    v = Vector3D.Zero; string[] f = s.Split(';'); double x, y, z;
    if (f.Length != 3 || !ReadNum(f[0], out x) || !ReadNum(f[1], out y) || !ReadNum(f[2], out z)) return false;
    v = new Vector3D(x, y, z); return true;
}
bool ReadNum(string s, out double n)
{
    return double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out n)
        && !double.IsNaN(n) && !double.IsInfinity(n);
}
void AutoStep()
{
    if (connector == null || connector.Closed || cockpit == null || cockpit.Closed || !connector.IsWorking || !cockpit.IsWorking)
    { Stop("A required block is not operational"); return; }
    if (connector.Status == MyShipConnectorStatus.Connected)
    {
        bool correct = connector.OtherConnector != null && connector.OtherConnector.EntityId == selected;
        Stop(correct ? "CONNECTED: sequence complete" : "Connected to a different port: sequence ended"); return;
    }
    if (HasPilotInput()) { Stop("Aborted: manual pilot input"); return; }
    Port p;
    if (!ports.TryGetValue(selected, out p) || now - p.Seen > DATA_TIMEOUT)
    { Stop("IGC signal lost: inertial dampeners enabled"); return; }
    if (!p.Available || p.Status == (int)MyShipConnectorStatus.Connected || (p.Owner != 0 && p.Owner != IGC.Me))
    { Stop("Port unavailable"); return; }
    if (now - startedAt > 300) { Stop("Docking sequence timed out"); return; }
    if (now >= nextRequest)
    {
        nextRequest = now + 0.5;
        IGC.SendUnicastMessage(p.Source, NETWORK, "RESERVE|" + selected + "|" + token);
    }
    if (phase != Phase.Reserve && now - grantedAt > LEASE_TIMEOUT) { Stop("Port reservation expired"); return; }
    double accel = Math.Min(0.8, AccelerationMargin() * 0.4);
    if (accel < 0.12 || gyros.Find(g => !g.Closed && g.IsWorking) == null)
    { Stop("Insufficient thrust or working gyros"); return; }
    Vector3D rot = RotationError(connector.WorldMatrix, -p.Forward, DockUp(p));
    angle = rot.Length();
    Vector3D faceDelta = connector.GetPosition() + connector.WorldMatrix.Forward * FaceOffset(connector) - p.Face;
    axial = Vector3D.Dot(faceDelta, p.Forward);
    lateral = (faceDelta - p.Forward * axial).Length();
    if (phase == Phase.Reserve)
    {
        ApplyTranslation(holdPoint, 0.5, accel);
        if (now - grantedAt <= LEASE_TIMEOUT)
        {
            phase = closeStart ? Phase.Approach : Phase.Align;
            status = closeStart ? "Automatic close-range centering" : "Aligning ship orientation";
        }
        else if (now - startedAt > 5) Stop("No acknowledgement from station");
        return;
    }
    ApplyRotation(rot);
    Vector3D goal = p.Face + p.Forward * (FaceOffset(connector) + StandOff());
    if (phase == Phase.Align)
    {
        ApplyTranslation(holdPoint, 0.5, accel);
        if (angle < 0.025 && cockpit.GetShipVelocities().AngularVelocity.Length() < 0.03)
        { phase = Phase.Approach; status = "Moving to the approach point in front of the port"; }
    }
    else if (phase == Phase.Approach)
    {
        double planeGap = Math.Max(0, axial);
        double approachSpeed = Math.Min(TravelSpeed((goal - connector.GetPosition()).Length(), accel), TravelSpeed(planeGap, accel));
        ApplyTranslation(goal, angle < 0.08 ? approachSpeed : 0.15, accel);
        if ((goal - connector.GetPosition()).Length() < 0.25 && cockpit.GetShipSpeed() < 0.15 && angle < 0.02)
        { phase = Phase.Final; status = "Approaching: speed adjusted to distance"; }
    }
    else if (phase == Phase.Final)
    {
        if (angle > 0.08 || lateral > 0.5 || axial < -0.02) { Stop("Alignment lost: approach aborted"); return; }
        goal = p.Face + p.Forward * (FaceOffset(connector) + FINAL_GAP);
        ApplyTranslation(goal, TravelSpeed(axial, accel), accel);
        if (connector.Status == MyShipConnectorStatus.Connectable && cockpit.GetShipSpeed() < 0.12
            && cockpit.GetShipVelocities().AngularVelocity.Length() < 0.02 && angle < 0.025 && lateral < 0.15)
        {
            // Release flight controls before joining the two grids' power networks.
            // After Connect, do not change thrust, gyros, dampeners, or connector pull.
            Stop("Flight controls released: connecting once");
            connector.Connect();
            if (connector.Status == MyShipConnectorStatus.Connected)
            {
                bool correct = connector.OtherConnector != null && connector.OtherConnector.EntityId == selected;
                status = correct ? "CONNECTED: sequence complete" : "Different port: sequence ended";
            }
            else status = "Connection not confirmed: manual control, no automatic retry";
        }
        else if ((goal - connector.GetPosition()).Length() < 0.03)
            status = "Holding at the final gap: waiting for Connectable";
    }
}
bool HasPilotInput()
{
    return cockpit != null && (cockpit.MoveIndicator.LengthSquared() > 0.01
        || cockpit.RotationIndicator.LengthSquared() > 0.01 || Math.Abs(cockpit.RollIndicator) > 0.01);
}
double StandOff()
{
    // Allow rotation clearance for a ship whose connector is away from its center.
    return controlling ? approachDistance : Math.Max(10, Me.CubeGrid.WorldAABB.HalfExtents.Length() * 2 + 5);
}
Vector3D DockUp(Port p)
{
    if (MATCH_PORT_ROLL) return p.Up;
    Vector3D up = connector.WorldMatrix.Up - p.Forward * Vector3D.Dot(connector.WorldMatrix.Up, p.Forward);
    return up.LengthSquared() > 0.0001 ? Vector3D.Normalize(up) : p.Up;
}
void Stop(string reason)
{
    Port p;
    long releaseTo = phase != Phase.Idle && ports.TryGetValue(selected, out p) ? p.Source : 0;
    phase = Phase.Idle;
    if (controlling)
    {
        foreach (var t in thrusters) if (!t.Closed) t.ThrustOverridePercentage = 0;
        foreach (var g in gyros) if (!g.Closed) { g.Pitch = g.Yaw = g.Roll = 0; g.GyroOverride = false; }
        if (cockpit != null && !cockpit.Closed) cockpit.DampenersOverride = true;
        if (connector != null && !connector.Closed) connector.PullStrength = previousPull;
    }
    controlling = false; Storage = ""; scanPending = false; status = reason;
    // Release local controls before sending any radio message.
    if (releaseTo != 0) IGC.SendUnicastMessage(releaseTo, NETWORK, "RELEASE|" + selected + "|" + token);
}
Vector3D[] Axes()
{
    return new Vector3D[] { cockpit.WorldMatrix.Right, cockpit.WorldMatrix.Left, cockpit.WorldMatrix.Up,
        cockpit.WorldMatrix.Down, cockpit.WorldMatrix.Backward, cockpit.WorldMatrix.Forward };
}
double[] Capacities(Vector3D[] axes)
{
    double[] result = new double[6];
    foreach (var t in thrusters)
    {
        if (t.Closed || !t.IsWorking) continue;
        for (int i = 0; i < 6; i++) if (Vector3D.Dot(t.WorldMatrix.Backward, axes[i]) > 0.99) result[i] += t.MaxEffectiveThrust;
    }
    return result;
}
double AccelerationMargin()
{
    var axes = Axes(); var cap = Capacities(axes);
    double mass = cockpit.CalculateShipMass().PhysicalMass;
    if (mass <= 0) return 0;
    double min = double.MaxValue;
    Vector3D gravity = cockpit.GetNaturalGravity();
    // Keep sufficient thrust margin while rotating in gravity.
    for (int i = 0; i < 6; i++) min = Math.Min(min, cap[i] / mass - gravity.Length());
    return Math.Max(0, min);
}
void ApplyTranslation(Vector3D goal, double speedLimit, double accel)
{
    Vector3D error = goal - connector.GetPosition();
    double distance = error.Length();
    double speed = Math.Min(speedLimit, Math.Min(distance * 0.7, Math.Sqrt(2 * accel * distance)));
    Vector3D wanted = distance > 0.0001 ? error / distance * speed : Vector3D.Zero;
    var velocities = cockpit.GetShipVelocities();
    Vector3D pointVelocity = velocities.LinearVelocity + Vector3D.Cross(velocities.AngularVelocity, connector.GetPosition() - cockpit.CenterOfMass);
    Vector3D acceleration = Limit((wanted - pointVelocity) * 1.8, accel);
    Vector3D force = (acceleration - cockpit.GetNaturalGravity()) * cockpit.CalculateShipMass().PhysicalMass;
    var axes = Axes(); var cap = Capacities(axes);
    // All thrust is managed here: do not run other flight-control scripts concurrently.
    foreach (var t in thrusters)
    {
        if (t.Closed) continue;
        float fraction = 0;
        if (t.IsWorking)
            for (int i = 0; i < 6; i++)
                if (cap[i] > 0 && Vector3D.Dot(t.WorldMatrix.Backward, axes[i]) > 0.99)
                    fraction = (float)MathHelper.Clamp(Vector3D.Dot(force, axes[i]) / cap[i], 0, 1);
        t.ThrustOverridePercentage = fraction;
    }
}
Vector3D Limit(Vector3D v, double max)
{
    double len = v.Length(); return len > max && len > 0 ? v * (max / len) : v;
}
double TravelSpeed(double distance, double accel)
{
    if (distance < 1) return 0.10;
    if (distance <= SLOW_ZONE) return FINAL_SPEED;
    double remaining = distance - SLOW_ZONE;
    double proportional = FINAL_SPEED + 0.1 * remaining;
    // Reserve half the deceleration and allow 0.75 s of control response time:
    // v*t + (v^2-vfinal^2)/(2*a) <= distance to the slow zone.
    double braking = Math.Max(0.01, accel * 0.5);
    double reaction = braking * 0.75;
    double safeSpeed = Math.Sqrt(reaction * reaction + FINAL_SPEED * FINAL_SPEED + 2 * braking * remaining) - reaction;
    return Math.Min(MAX_SPEED, Math.Min(proportional, Math.Max(FINAL_SPEED, safeSpeed)));
}
void ApplyRotation(Vector3D error)
{
    Vector3D rate = Limit(error * 0.7, 0.18);
    foreach (var g in gyros)
    {
        if (g.Closed || !g.IsWorking) continue;
        // The gyro API vector has the opposite sign to a mathematical right-hand rotation.
        Vector3D local = Vector3D.TransformNormal(-rate, MatrixD.Transpose(g.WorldMatrix));
        g.Pitch = (float)local.X; g.Yaw = (float)local.Y; g.Roll = (float)local.Z;
        g.GyroOverride = true;
    }
}
// World-space axis-angle error, including 180 degrees (a cross product alone fails).
Vector3D RotationError(MatrixD current, Vector3D forward, Vector3D up)
{
    Vector3D back = -forward, right = Vector3D.Normalize(Vector3D.Cross(up, back));
    up = Vector3D.Cross(back, right);
    Vector3D x = right * current.Right.X + up * current.Up.X + back * current.Backward.X;
    Vector3D y = right * current.Right.Y + up * current.Up.Y + back * current.Backward.Y;
    Vector3D z = right * current.Right.Z + up * current.Up.Z + back * current.Backward.Z;
    double qw, qx, qy, qz, s;
    double trace = x.X + y.Y + z.Z;
    if (trace > 0)
    {
        s = Math.Sqrt(trace + 1) * 2; qw = s / 4;
        qx = (y.Z - z.Y) / s; qy = (z.X - x.Z) / s; qz = (x.Y - y.X) / s;
    }
    else if (x.X > y.Y && x.X > z.Z)
    {
        s = Math.Sqrt(Math.Max(0, 1 + x.X - y.Y - z.Z)) * 2;
        qw = (y.Z - z.Y) / s; qx = s / 4; qy = (y.X + x.Y) / s; qz = (z.X + x.Z) / s;
    }
    else if (y.Y > z.Z)
    {
        s = Math.Sqrt(Math.Max(0, 1 + y.Y - x.X - z.Z)) * 2;
        qw = (z.X - x.Z) / s; qx = (y.X + x.Y) / s; qy = s / 4; qz = (z.Y + y.Z) / s;
    }
    else
    {
        s = Math.Sqrt(Math.Max(0, 1 + z.Z - x.X - y.Y)) * 2;
        qw = (x.Y - y.X) / s; qx = (z.X + x.Z) / s; qy = (z.Y + y.Z) / s; qz = s / 4;
    }
    Vector3D q = new Vector3D(qx, qy, qz);
    if (qw < 0) { qw = -qw; q = -q; }
    double len = q.Length();
    return len < 1e-9 ? Vector3D.Zero : q / len * (2 * Math.Atan2(len, qw));
}
void Scan()
{
    if (camera == null) { status = cameraError; scanPending = false; return; }
    if (camera == null || camera.Closed || !camera.IsWorking || connector == null) { status = "Camera/connector missing or unavailable"; scanPending = false; return; }
    if (!camera.CanScan(MAX_RANGE)) { status = "Waiting for camera raycast charge"; return; }
    var hit = camera.Raycast(MAX_RANGE);
    scanPending = false;
    if (hit.IsEmpty() || !hit.HitPosition.HasValue || (hit.Type != MyDetectedEntityType.LargeGrid && hit.Type != MyDetectedEntityType.SmallGrid)
        || hit.EntityId == Me.CubeGrid.EntityId || hit.Velocity.Length() > 0.05)
    { status = "SCAN: no valid stationary grid surface"; return; }
    fallbackPoint = hit.HitPosition.Value;
    fallbackForward = connector.WorldMatrix.Forward; fallbackUp = connector.WorldMatrix.Up; fallbackAt = now;
    status = "Manual fallback: target point selected by pilot";
}
double FaceOffset(IMyShipConnector p)
{
    foreach (string line in p.CustomData.Split('\n'))
    {
        double value;
        if (line.Trim().StartsWith("DockFaceOffset=", StringComparison.OrdinalIgnoreCase)
            && ReadNum(line.Trim().Substring(15), out value) && value >= 0 && value <= 10) return value;
    }
    Vector3D size = (Vector3D)(p.Max - p.Min + Vector3I.One) * p.CubeGrid.GridSize;
    Vector3D f = Vector3D.TransformNormal(p.WorldMatrix.Forward, MatrixD.Transpose(p.CubeGrid.WorldMatrix));
    return (Math.Abs(f.X) * size.X + Math.Abs(f.Y) * size.Y + Math.Abs(f.Z) * size.Z) * 0.5;
}
string Move(double v, string positive, string negative)
{ return Math.Abs(v) < 0.08 ? "OK" : (v > 0 ? positive : negative) + " " + Math.Abs(v).ToString("F2") + "m"; }
void Draw()
{
    var b = new System.Text.StringBuilder();
    b.AppendLine("DOCKING 2.0 | " + (phase == Phase.Idle ? "AUTOPILOT IDLE" : "AUTO: " + phase));
    b.AppendLine("Ship connector: " + (connector == null ? "MISSING" : connector.CustomName));
    bool ready = connector != null && connector.Status == MyShipConnectorStatus.Connectable;
    bool connected = connector != null && connector.Status == MyShipConnectorStatus.Connected;
    b.AppendLine(connected ? "CONNECTED - sequence complete" : ready ? "READY TO CONNECT" : status);
    if (setupError.Length > 0) b.AppendLine(setupError);
    Port p;
    if (phase == Phase.Idle && selected != 0)
    {
        if (ports.TryGetValue(selected, out p) && now - p.Seen <= DATA_TIMEOUT)
        {
            b.AppendLine(selectionConfirmed ? "PORT SELECTED" : "PREVIEW - press SELECT");
            b.AppendLine(p.Station + " / " + p.Name);
        }
        else b.AppendLine("PORT SIGNAL LOST - use NEXT");
    }
    if (!connected && ports.TryGetValue(selected, out p) && now - p.Seen <= DATA_TIMEOUT && connector != null && cockpit != null)
    {
        b.AppendLine(p.Station + " / " + p.Name);
        b.AppendLine("ID: " + p.Id);
        Vector3D ownFace = connector.GetPosition() + connector.WorldMatrix.Forward * FaceOffset(connector);
        Vector3D delta = p.Face + p.Forward * FINAL_GAP - ownFace;
        Vector3D pilot = Vector3D.TransformNormal(delta, MatrixD.Transpose(cockpit.WorldMatrix));
        Vector3D rotation = RotationError(connector.WorldMatrix, -p.Forward, DockUp(p));
        Vector3D r = Vector3D.TransformNormal(rotation, MatrixD.Transpose(cockpit.WorldMatrix)) * (180 / Math.PI);
        if (!ready)
        {
            b.AppendLine("Port distance: " + (p.Face - ownFace).Length().ToString("F2") + " m");
            b.AppendLine("Ship speed: " + cockpit.GetShipSpeed().ToString("F2") + " m/s");
            b.AppendLine(phase == Phase.Idle ? "GUIDANCE ONLY - autopilot is NOT running" : "Corrections handled by autopilot:");
            b.AppendLine("Reference controller: " + cockpit.CustomName);
            b.AppendLine("Offset: " + Move(pilot.X, "RIGHT", "LEFT"));
            b.AppendLine("        " + Move(pilot.Y, "UP", "DOWN"));
            b.AppendLine("        " + Move(-pilot.Z, "FORWARD", "BACKWARD"));
            b.AppendLine("Angular error: " + (rotation.Length() * 180 / Math.PI).ToString("F1") + " degrees");
            b.AppendLine("Nose " + Turn(r.X, "UP", "DOWN") + " / " + Turn(r.Y, "LEFT", "RIGHT"));
            b.AppendLine("Roll " + Turn(r.Z, "LEFT SIDE DOWN", "RIGHT SIDE DOWN"));
        }
    }
    else if (!connected && !ready && fallbackPoint.HasValue && connector != null && cockpit != null)
    {
        if (now - fallbackAt > 120 || Vector3D.Dot(connector.WorldMatrix.Forward, fallbackForward) < 0.996
            || Vector3D.Dot(connector.WorldMatrix.Up, fallbackUp) < 0.996)
        { fallbackPoint = null; b.AppendLine("Fallback expired/orientation changed: align and SCAN again"); }
        else
        {
            Vector3D delta = fallbackPoint.Value - connector.GetPosition();
            Vector3D side = delta - fallbackForward * Vector3D.Dot(delta, fallbackForward);
            Vector3D pilot = Vector3D.TransformNormal(side, MatrixD.Transpose(cockpit.WorldMatrix));
            b.AppendLine("MANUAL - port center selected with SCAN");
            b.AppendLine(Move(pilot.X, "RIGHT", "LEFT") + " | " + Move(pilot.Y, "UP", "DOWN"));
            b.AppendLine(Move(-pilot.Z, "FORWARD", "BACKWARD"));
            b.AppendLine("Approach slowly along the connector axis.");
            b.AppendLine("No automatic flight control in fallback mode.");
        }
    }
    b.AppendLine("DOCK = start | STOP = abort");
    b.AppendLine("Undock: connector toolbar / P | SCAN = manual");
    if (phase == Phase.Idle)
    {
        RefreshList(); b.AppendLine("IGC ports: NEXT -> SELECT -> DOCK");
        for (int i = 0; i < listing.Count; i++)
        {
            p = ports[listing[i]];
            b.AppendLine((p.Id == selected ? "> " : "  ") + p.Station + "/" + p.Name + " [" + p.Id + "]"
                + (!p.Available ? " NO AUTO" : p.Status == (int)MyShipConnectorStatus.Connected ? " OCCUPIED" : ""));
        }
    }
    string text = b.ToString(); Echo(text);
    foreach (var s in screens) Dashboard(s, ready, connected);
}
void Dashboard(IMyTextSurface surface, bool ready, bool connected)
{
    Color muted = new Color(151, 171, 195), accent = new Color(70, 208, 235);
    Color signal = connected || ready ? new Color(104, 228, 157) : setupError.Length > 0 ? new Color(255, 122, 111) : accent;
    float scale = Math.Min(surface.SurfaceSize.X / 768f, surface.SurfaceSize.Y / 768f);
    Vector2 origin = (surface.TextureSize - new Vector2(768, 768) * scale) / 2;
    Port port;
    bool live = ports.TryGetValue(selected, out port) && now - port.Seen <= DATA_TIMEOUT;
    bool guidance = live && connector != null && cockpit != null && !ready && !connected;
    Vector3D pilot = Vector3D.Zero, turn = Vector3D.Zero;
    double distance = 0;
    if (guidance)
    {
        Vector3D face = connector.GetPosition() + connector.WorldMatrix.Forward * FaceOffset(connector);
        pilot = Vector3D.TransformNormal(port.Face + port.Forward * FINAL_GAP - face, MatrixD.Transpose(cockpit.WorldMatrix));
        turn = Vector3D.TransformNormal(RotationError(connector.WorldMatrix, -port.Forward, DockUp(port)), MatrixD.Transpose(cockpit.WorldMatrix)) * (180 / Math.PI);
        distance = (port.Face - face).Length();
    }
    bool manual = !live && fallbackPoint.HasValue && connector != null && cockpit != null && !ready && !connected;
    if (manual)
    {
        Vector3D delta = fallbackPoint.Value - connector.GetPosition();
        pilot = Vector3D.TransformNormal(delta - fallbackForward * Vector3D.Dot(delta, fallbackForward), MatrixD.Transpose(cockpit.WorldMatrix));
    }
    using (var frame = surface.DrawFrame())
    {
        frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", surface.TextureSize / 2, surface.TextureSize, new Color(8, 14, 24)));
        Box(frame, origin, scale, 24, 24, 720, 5, signal);
        Label(frame, surface, origin, scale, "DOCKING / 2.0", 24, 42, 430, 1.15f, Color.White);
        Label(frame, surface, origin, scale, connected ? "CONNECTED" : ready ? "READY" : phase == Phase.Idle ? "IDLE" : "AUTO / " + phase, 450, 47, 294, .75f, signal);
        Label(frame, surface, origin, scale, connector == null ? "Ship connector: MISSING" : connector.CustomName, 24, 91, 720, .57f, muted);
        Box(frame, origin, scale, 24, 125, 720, 93, new Color(18, 30, 46));
        Label(frame, surface, origin, scale, live ? selectionConfirmed ? "PORT SELECTED" : "PREVIEW - press SELECT" : manual ? "MANUAL GUIDANCE / SCAN" : "NO PORT SIGNAL", 40, 138, 688, .56f, accent);
        Label(frame, surface, origin, scale, live ? port.Station + " / " + port.Name : "NEXT > SELECT > DOCK", 40, 168, 688, .76f, Color.White);
        Label(frame, surface, origin, scale, "PORT DISTANCE", 24, 239, 330, .53f, muted);
        Label(frame, surface, origin, scale, guidance ? distance.ToString("F2") + " m" : ready ? "READY TO CONNECT" : "--", 24, 265, 350, 1.12f, signal);
        Label(frame, surface, origin, scale, "SHIP SPEED", 400, 239, 344, .53f, muted);
        Label(frame, surface, origin, scale, cockpit == null ? "--" : cockpit.GetShipSpeed().ToString("F2") + " m/s", 400, 265, 344, 1.12f, Color.White);
        Box(frame, origin, scale, 24, 322, 350, 181, new Color(18, 30, 46));
        Box(frame, origin, scale, 390, 322, 354, 181, new Color(18, 30, 46));
        Label(frame, surface, origin, scale, "TRANSLATION", 40, 337, 318, .62f, accent);
        Label(frame, surface, origin, scale, "ROTATION / NOSE", 406, 337, 322, .62f, accent);
        string[] moves = { Move(pilot.X, "RIGHT", "LEFT"), Move(pilot.Y, "UP", "DOWN"), Move(-pilot.Z, "FORWARD", "BACKWARD") };
        string[] turns = { Turn(turn.X, "UP", "DOWN"), Turn(turn.Y, "LEFT", "RIGHT"), "Roll " + Turn(turn.Z, "LEFT SIDE DOWN", "RIGHT SIDE DOWN") };
        for (int i = 0; i < 3; i++)
        {
            Label(frame, surface, origin, scale, guidance || manual ? moves[i] : "--", 40, 379 + 36 * i, 318, .7f, Color.White);
            Label(frame, surface, origin, scale, guidance ? turns[i] : manual ? "MANUAL" : "--", 406, 379 + 36 * i, 322, .7f, Color.White);
        }
        Label(frame, surface, origin, scale, cockpit == null ? "Reference: MISSING" : "Reference controller: " + cockpit.CustomName, 24, 516, 720, .5f, muted);
        Box(frame, origin, scale, 24, 547, 720, 143, new Color(18, 30, 46));
        string message = setupError.Length > 0 ? setupError : connected ? "CONNECTED - sequence complete" : ready ? "READY TO CONNECT" : status;
        Wrapped(frame, surface, origin, scale, message, 40, 560, 688, signal);
        Label(frame, surface, origin, scale, phase == Phase.Idle ? "DOCK = start | STOP = abort" : "AUTO ACTIVE / STOP = cancel", 24, 711, 720, .6f, accent);
        Label(frame, surface, origin, scale, "Undock: connector toolbar / P | SCAN = manual", 24, 741, 720, .43f, muted);
    }
}
void Box(MySpriteDrawFrame frame, Vector2 origin, float scale, float x, float y, float w, float h, Color color)
{ frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", origin + new Vector2(x + w / 2, y + h / 2) * scale, new Vector2(w, h) * scale, color)); }
void Label(MySpriteDrawFrame frame, IMyTextSurface surface, Vector2 origin, float scale, string value, float x, float y, float width, float font, Color color)
{
    float measured = surface.MeasureStringInPixels(new System.Text.StringBuilder(value), "Debug", font).X;
    float fit = Math.Min(1, width / Math.Max(1, measured));
    var sprite = MySprite.CreateText(value, "Debug", color, font * fit * scale, TextAlignment.LEFT);
    sprite.Position = origin + new Vector2(x, y) * scale; frame.Add(sprite);
}
void Wrapped(MySpriteDrawFrame frame, IMyTextSurface surface, Vector2 origin, float scale, string value, float x, float y, float width, Color color)
{
    var lines = new List<string>(); string line = "";
    foreach (string word in value.Replace('\n', ' ').Split(' '))
    {
        string next = line.Length == 0 ? word : line + " " + word;
        if (line.Length > 0 && surface.MeasureStringInPixels(new System.Text.StringBuilder(next), "Debug", .6f).X > width)
        { lines.Add(line); line = word; }
        else line = next;
    }
    if (line.Length > 0) lines.Add(line);
    float row = Math.Min(28, 115f / Math.Max(1, lines.Count));
    for (int i = 0; i < lines.Count; i++) Label(frame, surface, origin, scale, lines[i], x, y + row * i, width, Math.Min(.6f, row / 30), color);
}
string Turn(double degrees, string positive, string negative)
{ return Math.Abs(degrees) < 1 ? "OK" : (degrees > 0 ? positive : negative) + " " + Math.Abs(degrees).ToString("F1") + "deg"; }
