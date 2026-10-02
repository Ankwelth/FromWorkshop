/*
 * ==========================================
 * Script: NEXUS Auto-Dock & Beacon System
 * Version: 1.6.2 experimental (Predictive Braking + Raycast Safety)
 * ==========================================
 */

Dictionary<string, MatrixD> savedPositions = new Dictionary<string, MatrixD>();
Dictionary<string, MatrixD> savedLocalOffsets = new Dictionary<string, MatrixD>();
Dictionary<string, long> savedHostGridIds = new Dictionary<string, long>();
Dictionary<string, List<MatrixD>> savedRoutes = new Dictionary<string, List<MatrixD>>();
Dictionary<string, List<MatrixD>> savedLocalRoutes = new Dictionary<string, List<MatrixD>>();
List<long> activeBlocksState = new List<long>();

const string WaypointSectionPrefix = "Waypoint:";

string activeDockingTarget = null;
MatrixD currentActiveTarget;
bool isWaitingForBeacon = false;
bool isWaitingForManualConnect = false;

int dockDelayCounter = 0;
int ecoUnparkTimer = -1;

string pendingSaveName = null;
long pendingSaveHostId = 0;
int pingCounter = 0;
int broadcastTimer = 0;
int hostSignalTimeout = -1;
int activeRouteIndex = -1;
int cameraSafetyDelayTimer = -1;
int cameraSafetyErrorTimer = -1;

Vector3D hostVelocity = Vector3D.Zero;
MatrixD currentHostMatrix = MatrixD.Identity;
bool hasCurrentHostMatrix = false;
bool cameraPrimeShotDone = false;
bool cameraCheckShotDone = false;
string lastCameraSafetyError = "";

bool isRecordingRoute = false;
string recordingRouteName = null;
List<MatrixD> recordingRoutePoints = new List<MatrixD>();

string pendingRouteSaveName = null;
long pendingRouteHostId = 0;
MatrixD pendingRouteFinalMatrix = MatrixD.Identity;
List<MatrixD> pendingRouteWorldPoints = new List<MatrixD>();

bool IsMotherShipBeacon = false;
string ConnectorTag = "park";
string ControllerTag = "";
double MaxForwardSpeed = 20.0;
double RotationDistance = 50.0;
double FinalApproachDistance = 20.0;
double FinalApproachSpeed = 5.0;
int BeaconTimeoutFrames = 120;
double RoutePointReachDistance = 5.0;
const double BrakingSafetyFactor = 1.5;
const double BrakingMinDeceleration = 0.25;
const double CameraSafetyMaxDistance = 500.0;
const double CameraSafetyMargin = 10.0;
const double CameraSafetyAlignment = 0.78;
const double CameraSafetyNoseAlignment = 0.92;
const double CameraSafetyPrimeDistance = 1.0;
const int CameraSafetyDelayFrames = 120;
const int CameraSafetyErrorFrames = 180;

bool AutoEcoAfterAutoDock = true;
bool EnableThrusterGyroOff = true;
bool EnableBatteryCharge = true;
bool EnableTankStockpile = true;

IMyShipController shipController;
List<IMyShipConnector> connectors = new List<IMyShipConnector>();
List<IMyGyro> gyros = new List<IMyGyro>();
List<IMyThrust> thrusters = new List<IMyThrust>();
List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyGasTank> gasTanks = new List<IMyGasTank>();
List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
List<IMyRadioAntenna> antennas = new List<IMyRadioAntenna>();
List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
MyIni ini = new MyIni();

IMyBroadcastListener hostWakeListener = null;
IMyBroadcastListener targetDataListener = null;

public Program()
{
    LoadData();
    InitializeBlocks();
    SetupRadio();
    ManageUpdateFrequency();
}

void SetupRadio()
{
    string myWakeChannel = "NEXUS_WAKE_" + Me.CubeGrid.EntityId.ToString();
    hostWakeListener = IGC.RegisterBroadcastListener(myWakeChannel);
    hostWakeListener.SetMessageCallback("IGC_EVENT");
}

void LoadData()
{
    savedPositions.Clear();
    savedLocalOffsets.Clear();
    savedHostGridIds.Clear();
    savedRoutes.Clear();
    savedLocalRoutes.Clear();
    activeBlocksState.Clear();

    ini.Clear();
    if (ini.TryParse(Me.CustomData))
    {
        IsMotherShipBeacon = ini.Get("Settings", "IsMotherShipBeacon").ToBoolean(false);
        ConnectorTag = ini.Get("Settings", "ConnectorTag").ToString("park");
        ControllerTag = ini.Get("Settings", "ControllerTag").ToString("");
        MaxForwardSpeed = ini.Get("Settings", "MaxForwardSpeed").ToDouble(20.0);
        RotationDistance = ini.Get("Settings", "RotationDistance").ToDouble(50.0);
        FinalApproachDistance = ini.Get("Settings", "FinalApproachDistance").ToDouble(20.0);
        FinalApproachSpeed = ini.Get("Settings", "FinalApproachSpeed").ToDouble(5.0);
        BeaconTimeoutFrames = ini.Get("Settings", "BeaconTimeoutFrames").ToInt32(120);
        RoutePointReachDistance = ini.Get("Settings", "RoutePointReachDistance").ToDouble(5.0);

        AutoEcoAfterAutoDock = ini.Get("EcoSettings", "AutoEcoAfterAutoDock").ToBoolean(true);
        EnableThrusterGyroOff = ini.Get("EcoSettings", "EnableThrusterGyroOff").ToBoolean(true);
        EnableBatteryCharge = ini.Get("EcoSettings", "EnableBatteryCharge").ToBoolean(true);
        EnableTankStockpile = ini.Get("EcoSettings", "EnableTankStockpile").ToBoolean(true);

        string rawStates = ini.Get("EcoState", "BlocksWereOn").ToString("");
        if (!string.IsNullOrWhiteSpace(rawStates))
        {
            var ids = rawStates.Split(',');
            foreach (var idStr in ids)
            {
                long id;
                if (long.TryParse(idStr, out id)) activeBlocksState.Add(id);
            }
        }

        LoadLegacyWaypoints();
        LoadCompactWaypoints();
        NormalizeWaypointData();
    }

    SaveData();
}

void LoadLegacyWaypoints()
{
    List<MyIniKey> keys = new List<MyIniKey>();
    ini.GetKeys("Positions", keys);
    foreach (var key in keys)
    {
        MatrixD matrix;
        if (TryDeserializeMatrix(ini.Get(key).ToString(), out matrix))
            savedPositions[key.Name] = matrix;
    }

    List<MyIniKey> localKeys = new List<MyIniKey>();
    ini.GetKeys("LocalOffsets", localKeys);
    foreach (var key in localKeys)
    {
        MatrixD matrix;
        if (TryDeserializeMatrix(ini.Get(key).ToString(), out matrix))
            savedLocalOffsets[key.Name] = matrix;
    }

    List<MyIniKey> hostKeys = new List<MyIniKey>();
    ini.GetKeys("HostIDs", hostKeys);
    foreach (var key in hostKeys)
    {
        long id;
        if (long.TryParse(ini.Get(key).ToString(), out id))
            savedHostGridIds[key.Name] = id;
    }
}

void LoadCompactWaypoints()
{
    List<string> sections = new List<string>();
    ini.GetSections(sections);

    foreach (var section in sections)
    {
        if (!section.ToLower().StartsWith(WaypointSectionPrefix.ToLower()))
            continue;

        string name = section.Substring(WaypointSectionPrefix.Length).Trim();
        if (string.IsNullOrWhiteSpace(name))
            continue;

        MatrixD worldMatrix;
        if (!TryDeserializeMatrix(ini.Get(section, "World").ToString(""), out worldMatrix))
            continue;

        savedPositions[name] = worldMatrix;

        MatrixD localMatrix;
        long hostId;
        bool hasLocal = TryDeserializeMatrix(ini.Get(section, "Local").ToString(""), out localMatrix);
        bool hasHost = long.TryParse(ini.Get(section, "HostId").ToString(""), out hostId);

        if (hasLocal && hasHost)
        {
            savedLocalOffsets[name] = localMatrix;
            savedHostGridIds[name] = hostId;
        }
        else
        {
            savedLocalOffsets.Remove(name);
            savedHostGridIds.Remove(name);
        }

        List<MatrixD> route;
        if (TryDeserializeRoute(ini.Get(section, "Route").ToString(""), out route))
            savedRoutes[name] = route;

        if (TryDeserializeRoute(ini.Get(section, "LocalRoute").ToString(""), out route))
            savedLocalRoutes[name] = route;
    }
}

void NormalizeWaypointData()
{
    List<string> removeHosts = new List<string>();
    foreach (var host in savedHostGridIds)
    {
        if (!savedPositions.ContainsKey(host.Key) || !savedLocalOffsets.ContainsKey(host.Key))
            removeHosts.Add(host.Key);
    }
    foreach (var name in removeHosts)
        savedHostGridIds.Remove(name);

    List<string> removeLocals = new List<string>();
    foreach (var local in savedLocalOffsets)
    {
        if (!savedPositions.ContainsKey(local.Key) || !savedHostGridIds.ContainsKey(local.Key))
            removeLocals.Add(local.Key);
    }
    foreach (var name in removeLocals)
        savedLocalOffsets.Remove(name);

    List<string> removeRoutes = new List<string>();
    foreach (var route in savedRoutes)
    {
        if (!savedPositions.ContainsKey(route.Key) || route.Value == null || route.Value.Count == 0)
            removeRoutes.Add(route.Key);
    }
    foreach (var name in removeRoutes)
        savedRoutes.Remove(name);

    List<string> removeLocalRoutes = new List<string>();
    foreach (var route in savedLocalRoutes)
    {
        if (!savedPositions.ContainsKey(route.Key) || !savedHostGridIds.ContainsKey(route.Key) || route.Value == null || route.Value.Count == 0)
            removeLocalRoutes.Add(route.Key);
    }
    foreach (var name in removeLocalRoutes)
        savedLocalRoutes.Remove(name);
}

void SaveData()
{
    ini.Clear();

    ini.Set("Settings", "IsMotherShipBeacon", IsMotherShipBeacon);
    ini.Set("Settings", "ConnectorTag", ConnectorTag);
    ini.Set("Settings", "ControllerTag", ControllerTag);
    ini.Set("Settings", "MaxForwardSpeed", MaxForwardSpeed);
    ini.Set("Settings", "RotationDistance", RotationDistance);
    ini.Set("Settings", "FinalApproachDistance", FinalApproachDistance);
    ini.Set("Settings", "FinalApproachSpeed", FinalApproachSpeed);
    ini.Set("Settings", "BeaconTimeoutFrames", BeaconTimeoutFrames);
    ini.Set("Settings", "RoutePointReachDistance", RoutePointReachDistance);

    ini.Set("EcoSettings", "AutoEcoAfterAutoDock", AutoEcoAfterAutoDock);
    ini.Set("EcoSettings", "EnableThrusterGyroOff", EnableThrusterGyroOff);
    ini.Set("EcoSettings", "EnableBatteryCharge", EnableBatteryCharge);
    ini.Set("EcoSettings", "EnableTankStockpile", EnableTankStockpile);

    string rawStates = string.Join(",", activeBlocksState);
    ini.Set("EcoState", "BlocksWereOn", rawStates);

    foreach (var pos in savedPositions)
    {
        string section = GetWaypointSection(pos.Key);
        ini.Set(section, "World", SerializeMatrix(pos.Value));

        MatrixD localMatrix;
        long hostId;
        if (savedLocalOffsets.TryGetValue(pos.Key, out localMatrix) &&
            savedHostGridIds.TryGetValue(pos.Key, out hostId))
        {
            ini.Set(section, "Local", SerializeMatrix(localMatrix));
            ini.Set(section, "HostId", hostId.ToString());
        }

        List<MatrixD> route;
        if (savedRoutes.TryGetValue(pos.Key, out route) && route.Count > 0)
            ini.Set(section, "Route", SerializeRoute(route));

        if (savedLocalRoutes.TryGetValue(pos.Key, out route) && route.Count > 0)
            ini.Set(section, "LocalRoute", SerializeRoute(route));
    }

    Me.CustomData = ini.ToString();
}

string GetWaypointSection(string name)
{
    return WaypointSectionPrefix + name;
}

string SerializeMatrix(MatrixD m)
{
    return SerializeDouble(m.Forward.X) + "|" +
           SerializeDouble(m.Forward.Y) + "|" +
           SerializeDouble(m.Forward.Z) + "|" +
           SerializeDouble(m.Up.X) + "|" +
           SerializeDouble(m.Up.Y) + "|" +
           SerializeDouble(m.Up.Z) + "|" +
           SerializeDouble(m.Translation.X) + "|" +
           SerializeDouble(m.Translation.Y) + "|" +
           SerializeDouble(m.Translation.Z);
}

MatrixD DeserializeMatrix(string s)
{
    MatrixD matrix;
    if (TryDeserializeMatrix(s, out matrix))
        return matrix;
    return MatrixD.Identity;
}

string SerializeRoute(List<MatrixD> route)
{
    List<string> parts = new List<string>();
    foreach (var matrix in route)
        parts.Add(SerializeMatrix(matrix));

    return string.Join(";", parts);
}

bool TryDeserializeRoute(string s, out List<MatrixD> route)
{
    route = new List<MatrixD>();
    if (string.IsNullOrWhiteSpace(s))
        return false;

    var parts = s.Split(';');
    foreach (var part in parts)
    {
        MatrixD matrix;
        if (TryDeserializeMatrix(part, out matrix))
            route.Add(matrix);
    }

    return route.Count > 0;
}

bool TryDeserializeMatrix(string s, out MatrixD matrix)
{
    matrix = MatrixD.Identity;
    if (string.IsNullOrWhiteSpace(s))
        return false;

    var p = s.Split('|');
    return TryDeserializeMatrixParts(p, 0, out matrix);
}

bool TryDeserializeMatrixParts(string[] p, int startIndex, out MatrixD matrix)
{
    matrix = MatrixD.Identity;
    if (p == null || p.Length < startIndex + 9)
        return false;

    double fx, fy, fz, ux, uy, uz, tx, ty, tz;
    if (!TryReadDouble(p[startIndex], out fx)) return false;
    if (!TryReadDouble(p[startIndex + 1], out fy)) return false;
    if (!TryReadDouble(p[startIndex + 2], out fz)) return false;
    if (!TryReadDouble(p[startIndex + 3], out ux)) return false;
    if (!TryReadDouble(p[startIndex + 4], out uy)) return false;
    if (!TryReadDouble(p[startIndex + 5], out uz)) return false;
    if (!TryReadDouble(p[startIndex + 6], out tx)) return false;
    if (!TryReadDouble(p[startIndex + 7], out ty)) return false;
    if (!TryReadDouble(p[startIndex + 8], out tz)) return false;

    Vector3D f = new Vector3D(fx, fy, fz);
    Vector3D u = new Vector3D(ux, uy, uz);
    Vector3D t = new Vector3D(tx, ty, tz);
    matrix = MatrixD.CreateWorld(t, f, u);
    return true;
}

string SerializeVector(Vector3D v)
{
    return SerializeDouble(v.X) + ";" + SerializeDouble(v.Y) + ";" + SerializeDouble(v.Z);
}

bool TryDeserializeVector(string s, out Vector3D v)
{
    v = Vector3D.Zero;
    if (string.IsNullOrWhiteSpace(s))
        return false;

    var p = s.Split(';');
    if (p.Length != 3)
        p = s.Split(',');

    if (p.Length == 3)
    {
        double x, y, z;
        if (TryReadDouble(p[0], out x) && TryReadDouble(p[1], out y) && TryReadDouble(p[2], out z))
        {
            v = new Vector3D(x, y, z);
            return true;
        }
    }

    return Vector3D.TryParse(s, out v);
}

string SerializeDouble(double value)
{
    return value.ToString("R");
}

bool TryReadDouble(string raw, out double value)
{
    if (double.TryParse(raw, out value))
        return true;

    string swapped = raw.IndexOf('.') >= 0 ? raw.Replace('.', ',') : raw.Replace(',', '.');
    return double.TryParse(swapped, out value);
}

bool TryParseHostPayload(string hostData, out long hostId, out MatrixD hostMatrix, out Vector3D reportedVelocity)
{
    hostId = 0;
    hostMatrix = MatrixD.Identity;
    reportedVelocity = Vector3D.Zero;

    if (string.IsNullOrWhiteSpace(hostData))
        return false;

    var parts = hostData.Split('|');
    if (parts.Length < 10)
        return false;

    if (!long.TryParse(parts[0], out hostId))
        return false;

    if (!TryDeserializeMatrixParts(parts, 1, out hostMatrix))
        return false;

    if (parts.Length > 10)
        TryDeserializeVector(parts[10], out reportedVelocity);

    return true;
}

void InitializeBlocks()
{
    GridTerminalSystem.GetBlocksOfType(connectors, b => b.IsSameConstructAs(Me) && b.CustomName.ToLower().Contains(ConnectorTag.ToLower()));
    GridTerminalSystem.GetBlocksOfType(gyros, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(thrusters, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(batteries, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(gasTanks, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(lights, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(antennas, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(cameras, b => b.IsSameConstructAs(Me));

    foreach (var camera in cameras)
        camera.EnableRaycast = true;

    List<IMyShipController> controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, b => b.IsSameConstructAs(Me));

    shipController = null;

    if (!string.IsNullOrWhiteSpace(ControllerTag))
        shipController = controllers.Find(c => c.CustomName.ToLower().Contains(ControllerTag.ToLower()));

    if (shipController == null)
        shipController = controllers.Find(c => c.IsMainCockpit);

    if (shipController == null)
        shipController = controllers.Find(c => c.IsUnderControl);

    if (shipController == null && controllers.Count > 0)
        shipController = controllers[0];
}

void ManageUpdateFrequency()
{
    if (activeDockingTarget != null || pendingSaveName != null || pendingRouteSaveName != null || ecoUnparkTimer >= 0 || isWaitingForManualConnect || cameraSafetyErrorTimer >= 0 || (IsMotherShipBeacon && broadcastTimer > 0))
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update1;
    }
    else
    {
        Runtime.UpdateFrequency = UpdateFrequency.None;
    }
}

public void Main(string argument, UpdateType updateSource)
{
    if ((updateSource & (UpdateType.Trigger | UpdateType.Terminal)) != 0)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            if (isRecordingRoute && !string.IsNullOrWhiteSpace(recordingRouteName))
            {
                HandleRouteRecording(recordingRouteName);
                return;
            }

            Echo("[ERROR]\nArgument cannot be empty.");
            return;
        }

        string cmd = argument.Trim().ToLower();

        if (cmd == "record")
        {
            ToggleRouteRecording();
            return;
        }

        if (isRecordingRoute)
        {
            HandleRouteRecording(argument.Trim());
            return;
        }

        if (cmd == "park")
        {
            InitializeBlocks();
            bool isConnected = false;
            IMyShipConnector readyConnector = null;

            foreach (var c in connectors)
            {
                if (c.Status == MyShipConnectorStatus.Connected) isConnected = true;
                if (c.Status == MyShipConnectorStatus.Connectable) readyConnector = c;
            }

            if (isConnected)
            {
                Echo("[MOVING]\nInitializing undocking...\nEnabling ship systems.");
                SetEcoBlocksState(false);
                ecoUnparkTimer = 60;
                ManageUpdateFrequency();
            }
            else if (readyConnector != null)
            {
                readyConnector.Connect();
                isWaitingForManualConnect = true;
                Echo("[MOVING]\nLocking connector... Waiting for connection.");
                ManageUpdateFrequency();
            }
            else
            {
                Echo("[ERROR]\n'park' command rejected.\nShip must be docked or connector must be yellow.");
            }
            return;
        }

        pendingSaveName = null;

        if (!savedPositions.ContainsKey(argument))
        {
            InitializeBlocks();

            IMyShipConnector activeConnector = null;
            foreach (var connector in connectors)
            {
                if (connector.Status == MyShipConnectorStatus.Connectable || connector.Status == MyShipConnectorStatus.Connected)
                {
                    activeConnector = connector;
                    break;
                }
            }

            if (activeConnector != null && shipController != null)
            {
                if (IsMotherShipBeacon)
                {
                    Echo("[ERROR]\nHost beacon cannot save waypoints!");
                    return;
                }

                if (activeConnector.Status == MyShipConnectorStatus.Connectable) activeConnector.Connect();

                if (activeConnector.Status == MyShipConnectorStatus.Connected && activeConnector.OtherConnector != null)
                {
                    pendingSaveName = argument;
                    pendingSaveHostId = activeConnector.OtherConnector.CubeGrid.EntityId;
                    pingCounter = 0;

                    targetDataListener = IGC.RegisterBroadcastListener("NEXUS_REPLY_" + Me.CubeGrid.EntityId.ToString());
                    IGC.SendBroadcastMessage("NEXUS_WAKE_" + pendingSaveHostId.ToString(), Me.CubeGrid.EntityId.ToString());

                    Echo($"[SAVING]\nChecking interaction type for [{argument}]...");
                    ManageUpdateFrequency();
                }
                else
                {
                    Echo("[ERROR]\nConnector lost contact during saving.");
                }
            }
            else
            {
                Echo($"[ERROR]\nConnector '{ConnectorTag}' not ready or Controller not found!\nFly closer to make connector yellow.");
            }
        }
        else
        {
            if (activeDockingTarget == argument)
            {
                StopShip();
                activeDockingTarget = null;
                isWaitingForBeacon = false;
                hostSignalTimeout = -1;
                activeRouteIndex = -1;
                hasCurrentHostMatrix = false;
                ResetCameraSafety();
                Echo($"[SAVED]\nParking to [{argument}] canceled.");
            }
            else
            {
                InitializeBlocks();
                activeDockingTarget = argument;
                pingCounter = 0;
                hostSignalTimeout = -1;
                activeRouteIndex = 0;
                hostVelocity = Vector3D.Zero;
                hasCurrentHostMatrix = false;
                cameraSafetyErrorTimer = -1;
                lastCameraSafetyError = "";
                ResetCameraSafety();
                PrimeCameraSafetyRaycast();
                currentActiveTarget = savedPositions[argument];

                long hostId;
                MatrixD localOffset;
                if (TryGetDynamicTarget(argument, out hostId, out localOffset))
                {
                    isWaitingForBeacon = true;
                    hostSignalTimeout = 0;
                    targetDataListener = IGC.RegisterBroadcastListener("NEXUS_REPLY_" + Me.CubeGrid.EntityId.ToString());
                    IGC.SendBroadcastMessage("NEXUS_WAKE_" + hostId.ToString(), Me.CubeGrid.EntityId.ToString());
                }
                else
                {
                    isWaitingForBeacon = false;
                    hostVelocity = Vector3D.Zero;
                }

                Echo($"[MOVING]\nInitializing parking to [{argument}]");
            }
        }
        ManageUpdateFrequency();
    }

    if ((updateSource & UpdateType.IGC) != 0)
    {
        if (IsMotherShipBeacon && hostWakeListener != null && hostWakeListener.HasPendingMessage)
        {
            while (hostWakeListener.HasPendingMessage)
            {
                var msg = hostWakeListener.AcceptMessage();
                if (msg.Data is string)
                {
                    string targetShipId = (string)msg.Data;
                    broadcastTimer = 120;
                    string hostVelStr = shipController != null ? SerializeVector(shipController.GetShipVelocities().LinearVelocity) : SerializeVector(Vector3D.Zero);
                    string payload = Me.CubeGrid.EntityId.ToString() + "|" + SerializeMatrix(Me.CubeGrid.WorldMatrix) + "|" + hostVelStr;

                    IGC.SendBroadcastMessage("NEXUS_REPLY_" + targetShipId, payload);
                }
            }
            ManageUpdateFrequency();
        }
    }

    if ((updateSource & UpdateType.Update1) != 0)
    {
        if (isWaitingForManualConnect)
        {
            bool connectedNow = false;
            foreach (var c in connectors) if (c.Status == MyShipConnectorStatus.Connected) connectedNow = true;

            if (connectedNow)
            {
                isWaitingForManualConnect = false;
                SetEcoBlocksState(true);
                Echo("[SAVED]\nManual lock successful!\nShip switched to eco mode.");
                ManageUpdateFrequency();
                return;
            }
            return;
        }

        if (ecoUnparkTimer >= 0)
        {
            ecoUnparkTimer--;
            if (ecoUnparkTimer == 0)
            {
                ecoUnparkTimer = -1;
                foreach (var c in connectors) if (c.Status == MyShipConnectorStatus.Connected) c.Disconnect();
                Echo("[SAVED]\nShip successfully undocked!");
                ManageUpdateFrequency();
            }
            else
            {
                Echo($"[MOVING]\nPower grid stabilization... {ecoUnparkTimer} frames to disconnect.");
            }
            return;
        }

        if (IsMotherShipBeacon && broadcastTimer > 0)
        {
            broadcastTimer--;
            if (broadcastTimer <= 0) ManageUpdateFrequency();
            return;
        }

        if (cameraSafetyErrorTimer >= 0)
        {
            cameraSafetyErrorTimer--;
            Echo(lastCameraSafetyError);
            WriteProgramLcd(lastCameraSafetyError);
            if (cameraSafetyErrorTimer < 0) ManageUpdateFrequency();
            return;
        }

        if (pendingRouteSaveName != null)
        {
            pingCounter++;
            if (pingCounter % 10 == 0)
                IGC.SendBroadcastMessage("NEXUS_WAKE_" + pendingRouteHostId.ToString(), Me.CubeGrid.EntityId.ToString());

            if (targetDataListener != null && targetDataListener.HasPendingMessage)
            {
                string hostData = null;
                while (targetDataListener.HasPendingMessage)
                {
                    var msg = targetDataListener.AcceptMessage();
                    if (msg.Data is string) hostData = (string)msg.Data;
                }

                long verifiedHostId;
                MatrixD hostMatrix;
                Vector3D ignoredVelocity;
                if (TryParseHostPayload(hostData, out verifiedHostId, out hostMatrix, out ignoredVelocity) &&
                    verifiedHostId == pendingRouteHostId)
                {
                    string savedName = pendingRouteSaveName;
                    int routeCount = pendingRouteWorldPoints.Count;
                    SaveDynamicRoute(savedName, pendingRouteFinalMatrix, pendingRouteWorldPoints, verifiedHostId, hostMatrix);
                    ClearPendingRouteSave();
                    bool ecoEnabled = EnableEcoAfterWaypointSave();
                    Echo($"[SAVED]\nDynamic route [{savedName}] synchronized with host.\nPoints: {routeCount}\n{EcoSaveLine(ecoEnabled)}");
                    ManageUpdateFrequency();
                    return;
                }
            }

            if (pingCounter >= 45)
            {
                string savedName = pendingRouteSaveName;
                int routeCount = pendingRouteWorldPoints.Count;
                SaveStaticRoute(savedName, pendingRouteFinalMatrix, pendingRouteWorldPoints);
                ClearPendingRouteSave();
                bool ecoEnabled = EnableEcoAfterWaypointSave();
                Echo($"[SAVED]\nStatic route [{savedName}] saved.\nHost beacon not found.\nPoints: {routeCount}\n{EcoSaveLine(ecoEnabled)}");
                ManageUpdateFrequency();
                return;
            }

            return;
        }

        if (pendingSaveName != null)
        {
            pingCounter++;
            if (pingCounter % 10 == 0) IGC.SendBroadcastMessage("NEXUS_WAKE_" + pendingSaveHostId.ToString(), Me.CubeGrid.EntityId.ToString());

            if (targetDataListener != null && targetDataListener.HasPendingMessage)
            {
                string hostData = null;
                while (targetDataListener.HasPendingMessage)
                {
                    var msg = targetDataListener.AcceptMessage();
                    if (msg.Data is string) hostData = (string)msg.Data;
                }

                long verifiedHostId;
                MatrixD hostMatrix;
                Vector3D ignoredVelocity;
                if (TryParseHostPayload(hostData, out verifiedHostId, out hostMatrix, out ignoredVelocity) &&
                    verifiedHostId == pendingSaveHostId &&
                    shipController != null)
                {
                    string savedName = pendingSaveName;
                    MatrixD localMatrix = shipController.WorldMatrix * MatrixD.Invert(hostMatrix);
                    savedPositions[savedName] = shipController.WorldMatrix;
                    savedLocalOffsets[savedName] = localMatrix;
                    savedHostGridIds[savedName] = verifiedHostId;

                    pendingSaveName = null;
                    bool ecoEnabled = EnableEcoAfterWaypointSave();
                    Echo($"[SAVED]\nDynamic waypoint [{savedName}]\nsynchronized with host!\n{EcoSaveLine(ecoEnabled)}");
                    ManageUpdateFrequency();
                    return;
                }
            }

            if (pingCounter >= 45)
            {
                if (shipController != null)
                {
                    string savedName = pendingSaveName;
                    savedPositions[savedName] = shipController.WorldMatrix;
                    savedLocalOffsets.Remove(savedName);
                    savedHostGridIds.Remove(savedName);

                    pendingSaveName = null;
                    bool ecoEnabled = EnableEcoAfterWaypointSave();
                    Echo($"[SAVED]\nStatic waypoint [{savedName}]\nsaved.\n{EcoSaveLine(ecoEnabled)}");
                }
                else
                {
                    pendingSaveName = null;
                }
                ManageUpdateFrequency();
            }
            return;
        }

        if (activeDockingTarget != null)
        {
            string dockType = "Static (in space)";

            long hostId;
            MatrixD localOffset;
            if (TryGetDynamicTarget(activeDockingTarget, out hostId, out localOffset))
            {
                dockType = "Searching for host signal...";

                pingCounter++;
                hostSignalTimeout++;
                if (pingCounter % 15 == 0) IGC.SendBroadcastMessage("NEXUS_WAKE_" + hostId.ToString(), Me.CubeGrid.EntityId.ToString());

                if (targetDataListener != null && targetDataListener.HasPendingMessage)
                {
                    string hostData = null;
                    while (targetDataListener.HasPendingMessage)
                    {
                        var msg = targetDataListener.AcceptMessage();
                        if (msg.Data is string) hostData = (string)msg.Data;
                    }

                    long reportedHostId;
                    MatrixD hostMatrix;
                    Vector3D reportedVelocity;
                    if (TryParseHostPayload(hostData, out reportedHostId, out hostMatrix, out reportedVelocity) &&
                        reportedHostId == hostId)
                    {
                        currentHostMatrix = hostMatrix;
                        hasCurrentHostMatrix = true;
                        currentActiveTarget = localOffset * hostMatrix;
                        hostVelocity = reportedVelocity;
                        hostSignalTimeout = 0;
                        isWaitingForBeacon = false;
                    }
                }

                if (isWaitingForBeacon)
                {
                    StopShip();
                    Echo($"[MOVING]\nMode: {dockType}\nTarget: {activeDockingTarget}");
                    return;
                }

                if (hostSignalTimeout > BeaconTimeoutFrames)
                {
                    isWaitingForBeacon = true;
                    StopShip();
                    Echo($"[MOVING]\nHost signal lost.\nHolding position for [{activeDockingTarget}]...");
                    return;
                }

                dockType = "DYNAMIC (Synchronized)";
            }
            else
            {
                hostVelocity = Vector3D.Zero;
                hostSignalTimeout = -1;
            }

            if (shipController == null)
            {
                Echo("[ERROR]\nController not found during parking.");
                return;
            }

            MatrixD routeTarget;
            int routeTotal;
            if (TryGetActiveRouteTarget(out routeTarget, out routeTotal))
            {
                double routeDistance = (routeTarget.Translation - shipController.WorldMatrix.Translation).Length();
                if (routeDistance <= RoutePointReachDistance)
                {
                    activeRouteIndex++;
                    DisableCameraSafetyAfterFirstRouteLeg();
                    Echo($"[MOVING]\nRoute point reached: {activeRouteIndex}/{routeTotal}");
                    return;
                }

                PerformParking(routeTarget, dockType + $" | Route {activeRouteIndex + 1}/{routeTotal}", false);
            }
            else
            {
                PerformParking(currentActiveTarget, dockType, true);
            }
        }
    }
}

bool TryGetDynamicTarget(string name, out long hostId, out MatrixD localOffset)
{
    hostId = 0;
    localOffset = MatrixD.Identity;

    return savedHostGridIds.TryGetValue(name, out hostId) &&
           savedLocalOffsets.TryGetValue(name, out localOffset);
}

bool TryGetActiveRouteTarget(out MatrixD routeTarget, out int routeTotal)
{
    routeTarget = MatrixD.Identity;
    routeTotal = 0;

    if (activeDockingTarget == null || activeRouteIndex < 0)
        return false;

    List<MatrixD> route;
    if (savedLocalRoutes.TryGetValue(activeDockingTarget, out route) && hasCurrentHostMatrix)
    {
        routeTotal = route.Count;
        if (activeRouteIndex >= 0 && activeRouteIndex < route.Count)
        {
            routeTarget = route[activeRouteIndex] * currentHostMatrix;
            return true;
        }
    }

    if (savedRoutes.TryGetValue(activeDockingTarget, out route))
    {
        routeTotal = route.Count;
        if (activeRouteIndex >= 0 && activeRouteIndex < route.Count)
        {
            routeTarget = route[activeRouteIndex];
            return true;
        }
    }

    return false;
}

bool ActiveTargetHasRoute()
{
    if (activeDockingTarget == null)
        return false;

    List<MatrixD> route;
    if (savedRoutes.TryGetValue(activeDockingTarget, out route) && route != null && route.Count > 0)
        return true;

    if (savedLocalRoutes.TryGetValue(activeDockingTarget, out route) && route != null && route.Count > 0)
        return true;

    return false;
}

void ToggleRouteRecording()
{
    if (isRecordingRoute)
    {
        ClearRouteRecording();
        Echo("[SAVED]\nRoute recording canceled.");
        return;
    }

    if (IsMotherShipBeacon)
    {
        Echo("[ERROR]\nHost beacon cannot record docking routes.");
        return;
    }

    InitializeBlocks();
    if (shipController == null)
    {
        Echo("[ERROR]\nController not found. Route recording cannot start.");
        return;
    }

    StopShip();
    activeDockingTarget = null;
    pendingSaveName = null;
    ClearPendingRouteSave();
    isWaitingForBeacon = false;
    hostSignalTimeout = -1;
    activeRouteIndex = -1;
    hasCurrentHostMatrix = false;
    ResetCameraSafety();

    isRecordingRoute = true;
    recordingRouteName = null;
    recordingRoutePoints.Clear();

    Echo("[RECORDING]\nRoute recording started.\nNext argument sets route name and saves point #1.");
}

void HandleRouteRecording(string argument)
{
    InitializeBlocks();

    if (shipController == null)
    {
        Echo("[ERROR]\nController not found. Route point was not saved.");
        return;
    }

    if (string.IsNullOrWhiteSpace(recordingRouteName))
        recordingRouteName = argument;

    IMyShipConnector activeConnector = GetReadyOrConnectedConnector();
    bool readyToFinish = activeConnector != null && recordingRoutePoints.Count > 0;

    if (readyToFinish)
    {
        BeginRouteFinalize(activeConnector);
        return;
    }

    recordingRoutePoints.Add(shipController.WorldMatrix);
    Echo($"[RECORDING]\nRoute: {recordingRouteName}\nPoint #{recordingRoutePoints.Count} saved.");
}

IMyShipConnector GetReadyOrConnectedConnector()
{
    foreach (var connector in connectors)
    {
        if (connector.Status == MyShipConnectorStatus.Connected ||
            connector.Status == MyShipConnectorStatus.Connectable)
            return connector;
    }

    return null;
}

void BeginRouteFinalize(IMyShipConnector activeConnector)
{
    if (activeConnector.Status == MyShipConnectorStatus.Connectable)
        activeConnector.Connect();

    string saveName = recordingRouteName;
    MatrixD finalMatrix = shipController.WorldMatrix;
    List<MatrixD> routePoints = new List<MatrixD>(recordingRoutePoints);

    bool canAskHost = activeConnector.Status == MyShipConnectorStatus.Connected &&
                      activeConnector.OtherConnector != null;

    ClearRouteRecording();

    if (canAskHost)
    {
        pendingRouteSaveName = saveName;
        pendingRouteHostId = activeConnector.OtherConnector.CubeGrid.EntityId;
        pendingRouteFinalMatrix = finalMatrix;
        pendingRouteWorldPoints = routePoints;
        pingCounter = 0;

        targetDataListener = IGC.RegisterBroadcastListener("NEXUS_REPLY_" + Me.CubeGrid.EntityId.ToString());
        IGC.SendBroadcastMessage("NEXUS_WAKE_" + pendingRouteHostId.ToString(), Me.CubeGrid.EntityId.ToString());

        Echo($"[SAVING]\nFinalizing route [{saveName}]...\nChecking for host beacon.");
        ManageUpdateFrequency();
        return;
    }

    SaveStaticRoute(saveName, finalMatrix, routePoints);
    bool ecoEnabled = EnableEcoAfterWaypointSave();
    Echo($"[SAVED]\nStatic route [{saveName}] saved.\nPoints: {routePoints.Count}\n{EcoSaveLine(ecoEnabled)}");
    ManageUpdateFrequency();
}

void SaveStaticRoute(string name, MatrixD finalMatrix, List<MatrixD> routePoints)
{
    savedPositions[name] = finalMatrix;
    savedLocalOffsets.Remove(name);
    savedHostGridIds.Remove(name);
    savedLocalRoutes.Remove(name);

    if (routePoints != null && routePoints.Count > 0)
        savedRoutes[name] = new List<MatrixD>(routePoints);
    else
        savedRoutes.Remove(name);
}

void SaveDynamicRoute(string name, MatrixD finalMatrix, List<MatrixD> routePoints, long hostId, MatrixD hostMatrix)
{
    MatrixD invHost = MatrixD.Invert(hostMatrix);

    savedPositions[name] = finalMatrix;
    savedLocalOffsets[name] = finalMatrix * invHost;
    savedHostGridIds[name] = hostId;

    if (routePoints != null && routePoints.Count > 0)
    {
        savedRoutes[name] = new List<MatrixD>(routePoints);

        List<MatrixD> localRoute = new List<MatrixD>();
        foreach (var point in routePoints)
            localRoute.Add(point * invHost);

        savedLocalRoutes[name] = localRoute;
    }
    else
    {
        savedRoutes.Remove(name);
        savedLocalRoutes.Remove(name);
    }
}

void ClearRouteRecording()
{
    isRecordingRoute = false;
    recordingRouteName = null;
    recordingRoutePoints.Clear();
}

void ClearPendingRouteSave()
{
    pendingRouteSaveName = null;
    pendingRouteHostId = 0;
    pendingRouteFinalMatrix = MatrixD.Identity;
    pendingRouteWorldPoints.Clear();
}

bool EnableEcoAfterWaypointSave()
{
    InitializeBlocks();
    if (!HasConnectedConnector())
    {
        SaveData();
        return false;
    }

    SetEcoBlocksState(true);
    return true;
}

bool HasConnectedConnector()
{
    foreach (var c in connectors)
        if (c.Status == MyShipConnectorStatus.Connected)
            return true;

    return false;
}

string EcoSaveLine(bool ecoEnabled)
{
    return ecoEnabled ? "Eco mode enabled." : "Eco mode skipped: connector is not locked.";
}

void ResetCameraSafety()
{
    cameraPrimeShotDone = false;
    cameraCheckShotDone = false;
    cameraSafetyDelayTimer = -1;
}

void DisableCameraSafetyAfterFirstRouteLeg()
{
    if (ActiveTargetHasRoute() && activeRouteIndex > 0)
    {
        cameraPrimeShotDone = true;
        cameraCheckShotDone = true;
        cameraSafetyDelayTimer = -1;
    }
}

void PrimeCameraSafetyRaycast()
{
    if (cameraPrimeShotDone || cameras.Count == 0)
        return;

    foreach (var camera in cameras)
    {
        if (!camera.Enabled)
            continue;

        camera.EnableRaycast = true;
        if (!camera.CanScan(CameraSafetyPrimeDistance))
            continue;

        camera.Raycast(CameraSafetyPrimeDistance);
        cameraPrimeShotDone = true;
        cameraSafetyDelayTimer = CameraSafetyDelayFrames;
        return;
    }
}

void UpdateCameraSafetyDelay()
{
    if (cameraSafetyDelayTimer > 0)
        cameraSafetyDelayTimer--;
}

bool ShouldRunCameraSafetyScan(double distance, Vector3D directionToTarget, MatrixD currentMatrix)
{
    if (activeDockingTarget == null || isRecordingRoute || cameras.Count == 0)
        return false;

    if (ActiveTargetHasRoute() && activeRouteIndex > 0)
        return false;

    if (!cameraPrimeShotDone)
    {
        PrimeCameraSafetyRaycast();
        return false;
    }

    if (cameraCheckShotDone || cameraSafetyDelayTimer > 0)
        return false;

    if (distance <= CameraSafetyMargin || distance > CameraSafetyMaxDistance)
        return false;

    double noseAlignment = currentMatrix.Forward.Dot(directionToTarget);
    return noseAlignment >= CameraSafetyNoseAlignment;
}

bool RunCameraSafetyScan(Vector3D targetPosition, Vector3D directionToTarget)
{
    bool ranScan = false;
    double closestHit = double.MaxValue;
    double closestTargetDistance = double.MaxValue;
    string closestName = "";

    foreach (var camera in cameras)
    {
        if (!camera.Enabled)
            continue;

        double alignment = camera.WorldMatrix.Forward.Dot(directionToTarget);
        if (alignment < CameraSafetyAlignment)
            continue;

        double cameraTargetDistance = (targetPosition - camera.GetPosition()).Length();
        if (cameraTargetDistance <= CameraSafetyMargin || cameraTargetDistance > CameraSafetyMaxDistance)
            continue;

        double scanDistance = Math.Min(cameraTargetDistance, CameraSafetyMaxDistance);
        if (!camera.CanScan(scanDistance))
            continue;

        ranScan = true;
        var hit = camera.Raycast(scanDistance);
        if (hit.IsEmpty())
            continue;

        Vector3D hitPos = hit.HitPosition.HasValue ? hit.HitPosition.Value : hit.Position;
        double hitDistance = (hitPos - camera.GetPosition()).Length();
        if (hitDistance <= CameraSafetyMargin)
            continue;

        if (hitDistance < closestHit)
        {
            closestHit = hitDistance;
            closestTargetDistance = cameraTargetDistance;
            closestName = hit.Name;
        }
    }

    if (ranScan)
        cameraCheckShotDone = true;

    if (closestHit < closestTargetDistance - CameraSafetyMargin)
    {
        TriggerCameraSafetyError(closestHit, closestTargetDistance, closestName);
        return false;
    }

    return true;
}

void TriggerCameraSafetyError(double hitDistance, double distanceToTarget, string hitName)
{
    StopShip();
    activeDockingTarget = null;
    isWaitingForBeacon = false;
    hostSignalTimeout = -1;
    activeRouteIndex = -1;
    hasCurrentHostMatrix = false;
    ResetCameraSafety();

    if (string.IsNullOrWhiteSpace(hitName))
        hitName = "unknown object";

    lastCameraSafetyError =
        "[ERROR]\nObstacle detected on docking path.\n" +
        "Object: " + hitName + "\n" +
        "Hit: " + hitDistance.ToString("F1") + " m\n" +
        "Target: " + distanceToTarget.ToString("F1") + " m\n" +
        "Safety margin: " + CameraSafetyMargin.ToString("F0") + " m";

    cameraSafetyErrorTimer = CameraSafetyErrorFrames;
    Echo(lastCameraSafetyError);
    WriteProgramLcd(lastCameraSafetyError);
    ManageUpdateFrequency();
}

void WriteProgramLcd(string text)
{
    var surface = Me.GetSurface(0);
    surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    surface.WriteText(text, false);
}

void PerformParking(MatrixD targetMatrix, string dockType, bool allowConnectorLock)
{
    if (shipController == null) return;

    bool isConnectable = false;
    bool isConnected = false;

    foreach (var connector in connectors)
    {
        if (connector.Status == MyShipConnectorStatus.Connectable) isConnectable = true;
        if (connector.Status == MyShipConnectorStatus.Connected) isConnected = true;
    }

    if (isConnected && allowConnectorLock)
    {
        StopShip();
        activeDockingTarget = null;
        isWaitingForBeacon = false;
        hostSignalTimeout = -1;
        activeRouteIndex = -1;
        hasCurrentHostMatrix = false;
        ResetCameraSafety();

        if (AutoEcoAfterAutoDock) SetEcoBlocksState(true);

        ManageUpdateFrequency();
        Echo("[SAVED]\nParking successfully completed!");
        return;
    }

    if (isConnectable && allowConnectorLock)
    {
        dockDelayCounter++;
        if (dockDelayCounter >= 120)
        {
            foreach (var connector in connectors)
            {
                if (connector.Status == MyShipConnectorStatus.Connectable) connector.Connect();
            }
        }
    }
    else
    {
        dockDelayCounter = 0;
    }

    shipController.DampenersOverride = false;

    MatrixD currentMatrix = shipController.WorldMatrix;
    Vector3D targetPosition = targetMatrix.Translation;
    Vector3D currentPosition = currentMatrix.Translation;

    Vector3D movementVector = targetPosition - currentPosition;
    double distance = movementVector.Length();
    Vector3D currentVelocity = shipController.GetShipVelocities().LinearVelocity;

    Echo("[MOVING]");
    Echo($"Mode: {dockType}");
    Echo($"Target: {activeDockingTarget}");
    Echo($"Distance: {distance:F2} m");
    Echo($"Speed: {currentVelocity.Length():F2} m/s");

    Vector3D directionToTarget = distance > 0.001 ? Vector3D.Normalize(movementVector) : targetMatrix.Forward;
    double brakingAcceleration = EstimateBrakingAcceleration(directionToTarget);
    double desiredSpeed = CalculateSafeApproachSpeed(distance, brakingAcceleration);

    Echo($"Cmd Speed: {desiredSpeed:F2} m/s");
    Echo($"Brake Accel: {brakingAcceleration:F2} m/s2");

    Vector3D desiredVelocity = hostVelocity + (directionToTarget * desiredSpeed);
    Vector3D velocityError = desiredVelocity - currentVelocity;

    UpdateCameraSafetyDelay();
    if (ShouldRunCameraSafetyScan(distance, directionToTarget, currentMatrix))
    {
        if (!RunCameraSafetyScan(targetPosition, directionToTarget))
            return;
    }

    if (distance > RotationDistance)
    {
        Vector3D tempUp = currentMatrix.Up;
        if (Math.Abs(directionToTarget.Dot(tempUp)) > 0.9) tempUp = currentMatrix.Right;
        MatrixD approachMatrix = MatrixD.CreateWorld(currentPosition, directionToTarget, tempUp);

        ApplyGyroOverride(approachMatrix, currentMatrix);
        ApplyThrust(velocityError);
    }
    else
    {
        ApplyGyroOverride(targetMatrix, currentMatrix);
        ApplyThrust(velocityError);
    }
}

void SetEcoBlocksState(bool enableEco)
{
    InitializeBlocks();

    if (enableEco)
    {
        activeBlocksState.Clear();
        foreach (var l in lights) if (l.Enabled) activeBlocksState.Add(l.EntityId);
        foreach (var a in antennas) if (a.Enabled) activeBlocksState.Add(a.EntityId);

        SaveData();

        if (EnableBatteryCharge) foreach (var bat in batteries) bat.ChargeMode = ChargeMode.Recharge;
        if (EnableTankStockpile) foreach (var tank in gasTanks) tank.Stockpile = true;

        foreach (var l in lights) l.Enabled = false;
        foreach (var a in antennas) a.Enabled = false;
        if (EnableThrusterGyroOff)
        {
            foreach (var thruster in thrusters) thruster.Enabled = false;
            foreach (var gyro in gyros) gyro.Enabled = false;
        }
    }
    else
    {
        LoadData();

        if (EnableBatteryCharge) foreach (var bat in batteries) bat.ChargeMode = ChargeMode.Auto;
        if (EnableTankStockpile) foreach (var tank in gasTanks) tank.Stockpile = false;
        if (EnableThrusterGyroOff)
        {
            foreach (var thruster in thrusters) thruster.Enabled = true;
            foreach (var gyro in gyros) gyro.Enabled = true;
        }

        foreach (var l in lights) l.Enabled = activeBlocksState.Contains(l.EntityId);
        foreach (var a in antennas) a.Enabled = activeBlocksState.Contains(a.EntityId);

        activeBlocksState.Clear();
        SaveData();
    }
}

double CalculateSafeApproachSpeed(double distance, double brakingAcceleration)
{
    double oldDesiredSpeed;
    if (distance > FinalApproachDistance)
    {
        oldDesiredSpeed = FinalApproachSpeed + (distance - FinalApproachDistance) * 0.5;
        oldDesiredSpeed = Math.Min(oldDesiredSpeed, MaxForwardSpeed);

        double brakingDistance = Math.Max(0, distance - FinalApproachDistance);
        double safeEntrySpeedSq = FinalApproachSpeed * FinalApproachSpeed + 2 * brakingAcceleration * brakingDistance;
        double safeEntrySpeed = Math.Sqrt(Math.Max(0, safeEntrySpeedSq));

        return Math.Min(oldDesiredSpeed, safeEntrySpeed);
    }

    oldDesiredSpeed = Math.Min(distance * 0.5, FinalApproachSpeed);
    if (distance < 0.05) oldDesiredSpeed = 0;

    double finalSafeSpeedSq = 2 * brakingAcceleration * Math.Max(0, distance);
    double finalSafeSpeed = Math.Sqrt(Math.Max(0, finalSafeSpeedSq));
    return Math.Min(oldDesiredSpeed, finalSafeSpeed);
}

double EstimateBrakingAcceleration(Vector3D directionToTarget)
{
    double mass = shipController.CalculateShipMass().PhysicalMass;
    if (mass <= 0)
        return BrakingMinDeceleration;

    double brakingForce = 0;
    foreach (var thruster in thrusters)
    {
        if (!thruster.Enabled)
            continue;

        double alignment = thruster.WorldMatrix.Forward.Dot(directionToTarget);
        if (alignment > 0)
            brakingForce += thruster.MaxEffectiveThrust * alignment;
    }

    double acceleration = brakingForce / mass / BrakingSafetyFactor;
    return Math.Max(acceleration, BrakingMinDeceleration);
}

void ApplyThrust(Vector3D velocityError)
{
    float mass = shipController.CalculateShipMass().PhysicalMass;
    foreach (var thruster in thrusters)
    {
        if (!thruster.Enabled) continue;
        double thrustAmount = velocityError.Dot(thruster.WorldMatrix.Forward);

        if (thrustAmount < 0)
        {
            float forceNeeded = (float)Math.Abs(thrustAmount) * mass * 2f;
            thruster.ThrustOverride = Math.Min(forceNeeded, thruster.MaxEffectiveThrust);
        }
        else thruster.ThrustOverride = 0;
    }
}

void ApplyGyroOverride(MatrixD targetMatrix, MatrixD currentMatrix)
{
    Vector3D forwardCross = Vector3D.Cross(currentMatrix.Forward, targetMatrix.Forward);
    Vector3D upCross = Vector3D.Cross(currentMatrix.Up, targetMatrix.Up);
    Vector3D rotationAxis = forwardCross + upCross;

    double p = 1.5;
    Vector3D controlSignal = rotationAxis * p;
    double maxAngularVelocity = 0.3;
    if (controlSignal.LengthSquared() > maxAngularVelocity * maxAngularVelocity)
    {
        controlSignal = Vector3D.Normalize(controlSignal) * maxAngularVelocity;
    }

    foreach (var gyro in gyros)
    {
        if (!gyro.Enabled) continue;
        gyro.GyroOverride = true;
        Vector3D localRotation = Vector3D.TransformNormal(controlSignal, MatrixD.Transpose(gyro.WorldMatrix));
        gyro.Pitch = -(float)localRotation.X;
        gyro.Yaw = -(float)localRotation.Y;
        gyro.Roll = -(float)localRotation.Z;
    }
}

void StopShip()
{
    dockDelayCounter = 0;
    isWaitingForManualConnect = false;
    hostVelocity = Vector3D.Zero;
    foreach (var thruster in thrusters) thruster.ThrustOverride = 0;
    foreach (var gyro in gyros) gyro.GyroOverride = false;
    if (shipController != null) shipController.DampenersOverride = true;
}
