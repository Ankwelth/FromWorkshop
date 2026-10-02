/* ============================================================================
 * SCRIPT NAME: IK Crane OS
 * AUTHOR: [BATERCOOL]
 * VERSION: 6.1.15 Fixed
 * NOTES:
 * - Safer IK clamping, including zero-distance protection.
 * - State restore from CustomData.
 * - Logical rotor angles for inverted rotors.
 * - Separate parking/ATC speeds for base, QE, and mouse rotors.
 * - Safer ATC plate/attacher checks.
 * - Tool plates are no longer auto-renamed by the lookup helper.
 * - Setup release now raises to SafeY before shutting down.
 * - Detached wrist/hinges no longer block parking completion.
 * - Optional gyro hold keeps the ship steady while crane mode is active.
 * - Separate MouseX auto speed for parking and ATC.
 * - Setup detach is blocked until the tool plate is locked.
 * - ATC timeout now counts only while the crane is stalled.
 * ============================================================================
 */

string cfgTag = "[Crane]";
double cfgMoveSpeed = 0.1;
float cfgBaseRotateSpeed = 2.0f;
float cfgQESpeed = 5.0f;
float cfgMouseSpeed = 0.5f;
float cfgMouseXAutoSpeed = 2.0f;
float cfgMouseYAutoSpeed = 2.0f;
float cfgWristOffset = 0f;
double cfgL1 = 15.0;
double cfgL2 = 15.0;
float cfgMaxSafeSpeed = 2.0f;
bool cfgInvertKinematic = false;
double cfgSafeDistance = 5.0;
bool cfgGyroHold = true;
float cfgGyroHoldPower = 1.0f;
int cfgATCTimeoutTicks = 1200;

double startX = 0.0;
double startY = 5.0;

string nameCockpit = "Cockpit";
string nameBaseRotor = "Base Rotor";
string nameShoulder = "Shoulder";
string nameElbow1 = "Elbow 1";
string nameElbow2 = "Elbow 2";
string nameWrist = "Wrist";
string nameQERotor = "QERotor";
string nameMouseXRotor = "MouseXRotor";
string nameMouseYRotor = "MouseYRotor";
string nameAttacher = "Attacher";

float parkBaseAngle = 0f;   int parkBaseOrder = 1;
float parkQEAngle = 0f;     int parkQEOrder = 2;
float parkMouseXAngle = 0f; int parkMouseXOrder = 3;
float parkMouseYAngle = 0f; int parkMouseYOrder = 3;
int parkArmOrder = 4;

IMyShipController cockpit;
IMyMotorStator baseRotor, shoulder, elbow1, elbow2, wrist;
IMyMotorStator qeBlock, mouseXBlock, mouseYBlock;
IMyMotorStator attacherBlock;
IMyTextSurface lcd;
List<IMyLightingBlock> craneLights = new List<IMyLightingBlock>();
List<IMyGyro> shipGyros = new List<IMyGyro>();

double targetX, targetY;
double destinationX, destinationY;

bool isCraneActive = false;
bool isParking = false;
int currentParkPhase = 0;
int maxParkPhase = 0;

bool savedControlThrusters = true;
bool savedControlGyros = true;
bool wasUnderControl = false;

// ATC Variables
bool isSetupMode = false;
bool isSetupFinishing = false;
int setupTimer = 0;
int setupPhase = 0;
int setupTargetTool = 0;

bool isATCActive = false;
int atcPhase = 0;
int currentTool = 0;
int targetTool = 0;
int queuedTool = 0;
int atcWaitTimer = 0;
int atcPhaseTimer = 0;
double atcSafeY = 0;
double atcWatchX = 0, atcWatchY = 0;
float atcWatchBase = 0f, atcWatchShoulder = 0f, atcWatchElbow1 = 0f, atcWatchElbow2 = 0f;
float atcWatchWrist = 0f, atcWatchQE = 0f, atcWatchMouseX = 0f, atcWatchMouseY = 0f;
bool atcWatchInitialized = false;

Dictionary<int, string> plateNames = new Dictionary<int, string>();
Dictionary<int, ToolPos> toolPosCache = new Dictionary<int, ToolPos>();
List<GyroState> savedGyroStates = new List<GyroState>();
bool gyroStatesSaved = false;

int tickCounter = 0;
int animFrame = 0;
string lastMessage = "";
bool solidRedAlert = false;
string[] animSequence = { "[=      ]", "[ =    ]", "[  =   ]", "[   =  ]", "[    = ]", "[     =]", "[    = ]", "[   =  ]", "[  =   ]", "[ =    ]" };

VRage.Game.ModAPI.Ingame.Utilities.MyIni ini = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();
System.Text.StringBuilder lcdBuffer = new System.Text.StringBuilder();

struct ToolPos {
    public double X, Y, SafeY;
    public float Base, QE, MX, MY;
    public bool IsValid;
}

struct GyroState {
    public IMyGyro Gyro;
    public bool Enabled;
    public bool Override;
    public float Pitch, Yaw, Roll, Power;
}

public Program()
{
    LoadConfig();
    InitBlocks();
    SyncArmPosition();
    ApplyRestoredState();
    SetUpdateFrequency();
}

public void Save()
{
    var saveIni = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();
    saveIni.TryParse(Me.CustomData);
    saveIni.Set("State", "IsCraneActive", isCraneActive);
    saveIni.Set("State", "SavedThrusters", savedControlThrusters);
    saveIni.Set("State", "SavedGyros", savedControlGyros);
    saveIni.Set("ATC", "CurrentTool", currentTool);
    Me.CustomData = saveIni.ToString();
}

void LoadConfig()
{
    var parsedIni = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();
    parsedIni.TryParse(Me.CustomData);

    cfgTag = parsedIni.Get("Settings", "Tag").ToString(cfgTag);
    cfgMoveSpeed = parsedIni.Get("Settings", "MoveSpeed").ToDouble(cfgMoveSpeed);
    cfgBaseRotateSpeed = parsedIni.Get("Settings", "BaseRotateSpeed").ToSingle(cfgBaseRotateSpeed);
    cfgQESpeed = parsedIni.Get("Settings", "QESpeed").ToSingle(cfgQESpeed);
    cfgMouseSpeed = parsedIni.Get("Settings", "MouseSpeed").ToSingle(cfgMouseSpeed);
    cfgMouseXAutoSpeed = parsedIni.Get("Settings", "MouseXAutoSpeed").ToSingle(cfgMouseXAutoSpeed);
    cfgMouseYAutoSpeed = parsedIni.Get("Settings", "MouseYAutoSpeed").ToSingle(cfgMouseYAutoSpeed);
    cfgWristOffset = parsedIni.Get("Settings", "WristOffsetDeg").ToSingle(cfgWristOffset);
    cfgMaxSafeSpeed = parsedIni.Get("Settings", "MaxSafeSpeed").ToSingle(cfgMaxSafeSpeed);
    cfgInvertKinematic = parsedIni.Get("Settings", "InvertKinematic").ToBoolean(cfgInvertKinematic);
    cfgSafeDistance = parsedIni.Get("Settings", "SafeDistance").ToDouble(cfgSafeDistance);
    cfgGyroHold = parsedIni.Get("Settings", "GyroHold").ToBoolean(cfgGyroHold);
    cfgGyroHoldPower = parsedIni.Get("Settings", "GyroHoldPower").ToSingle(cfgGyroHoldPower);
    cfgATCTimeoutTicks = parsedIni.Get("Settings", "ATCTimeoutTicks").ToInt32(cfgATCTimeoutTicks);
    if (cfgMouseXAutoSpeed < 0.05f) cfgMouseXAutoSpeed = 0.05f;
    if (cfgMouseYAutoSpeed < 0.05f) cfgMouseYAutoSpeed = 0.05f;
    cfgGyroHoldPower = MathHelper.Clamp(cfgGyroHoldPower, 0f, 1f);
    if (cfgATCTimeoutTicks < 120) cfgATCTimeoutTicks = 120;

    cfgL1 = parsedIni.Get("Geometry", "ArmLength_L1").ToDouble(cfgL1);
    cfgL2 = parsedIni.Get("Geometry", "ArmLength_L2").ToDouble(cfgL2);
    if (cfgL1 < 0.1) cfgL1 = 0.1;
    if (cfgL2 < 0.1) cfgL2 = 0.1;

    startX = parsedIni.Get("Position", "Starting_X").ToDouble(startX);
    startY = parsedIni.Get("Position", "Starting_Y").ToDouble(startY);

    nameCockpit = parsedIni.Get("BlockNames", "Cockpit").ToString(nameCockpit);
    nameBaseRotor = parsedIni.Get("BlockNames", "BaseRotor").ToString(nameBaseRotor);
    nameShoulder = parsedIni.Get("BlockNames", "Shoulder").ToString(nameShoulder);
    nameElbow1 = parsedIni.Get("BlockNames", "Elbow1").ToString(nameElbow1);
    nameElbow2 = parsedIni.Get("BlockNames", "Elbow2").ToString(nameElbow2);
    nameWrist = parsedIni.Get("BlockNames", "Wrist").ToString(nameWrist);
    nameQERotor = parsedIni.Get("BlockNames", "QERotor").ToString(nameQERotor);
    nameMouseXRotor = parsedIni.Get("BlockNames", "MouseXRotor").ToString(nameMouseXRotor);
    nameMouseYRotor = parsedIni.Get("BlockNames", "MouseYRotor").ToString(nameMouseYRotor);

    parkBaseAngle = parsedIni.Get("Park", "BaseRotor_Angle").ToSingle(parkBaseAngle);
    parkBaseOrder = parsedIni.Get("Park", "BaseRotor_Order").ToInt32(parkBaseOrder);
    parkQEAngle = parsedIni.Get("Park", "QERotor_Angle").ToSingle(parkQEAngle);
    parkQEOrder = parsedIni.Get("Park", "QERotor_Order").ToInt32(parkQEOrder);
    parkMouseXAngle = parsedIni.Get("Park", "MouseXRotor_Angle").ToSingle(parkMouseXAngle);
    parkMouseXOrder = parsedIni.Get("Park", "MouseXRotor_Order").ToInt32(parkMouseXOrder);
    parkMouseYAngle = parsedIni.Get("Park", "MouseYRotor_Angle").ToSingle(parkMouseYAngle);
    parkMouseYOrder = parsedIni.Get("Park", "MouseYRotor_Order").ToInt32(parkMouseYOrder);
    parkArmOrder = parsedIni.Get("Park", "Arm_Order").ToInt32(parkArmOrder);

    nameAttacher = parsedIni.Get("ATC", "Attacher").ToString(nameAttacher);
    currentTool = parsedIni.Get("ATC", "CurrentTool").ToInt32(currentTool);

    isCraneActive = parsedIni.Get("State", "IsCraneActive").ToBoolean(isCraneActive);
    savedControlThrusters = parsedIni.Get("State", "SavedThrusters").ToBoolean(savedControlThrusters);
    savedControlGyros = parsedIni.Get("State", "SavedGyros").ToBoolean(savedControlGyros);

    ini.Clear();

    ini.Set("Settings", "Tag", cfgTag);
    ini.Set("Settings", "MoveSpeed", cfgMoveSpeed);
    ini.Set("Settings", "BaseRotateSpeed", cfgBaseRotateSpeed);
    ini.Set("Settings", "QESpeed", cfgQESpeed);
    ini.Set("Settings", "MouseSpeed", cfgMouseSpeed);
    ini.Set("Settings", "MouseXAutoSpeed", cfgMouseXAutoSpeed);
    ini.Set("Settings", "MouseYAutoSpeed", cfgMouseYAutoSpeed);
    ini.Set("Settings", "WristOffsetDeg", cfgWristOffset);
    ini.Set("Settings", "MaxSafeSpeed", cfgMaxSafeSpeed);
    ini.Set("Settings", "InvertKinematic", cfgInvertKinematic);
    ini.Set("Settings", "SafeDistance", cfgSafeDistance);
    ini.Set("Settings", "GyroHold", cfgGyroHold);
    ini.Set("Settings", "GyroHoldPower", cfgGyroHoldPower);
    ini.Set("Settings", "ATCTimeoutTicks", cfgATCTimeoutTicks);
    ini.Set("Position", "Starting_X", startX);
    ini.Set("Position", "Starting_Y", startY);
    ini.Set("Geometry", "ArmLength_L1", cfgL1);
    ini.Set("Geometry", "ArmLength_L2", cfgL2);

    ini.Set("BlockNames", "Cockpit", nameCockpit);
    ini.Set("BlockNames", "BaseRotor", nameBaseRotor);
    ini.Set("BlockNames", "Shoulder", nameShoulder);
    ini.Set("BlockNames", "Elbow1", nameElbow1);
    ini.Set("BlockNames", "Elbow2", nameElbow2);
    ini.Set("BlockNames", "Wrist", nameWrist);
    ini.Set("BlockNames", "QERotor", nameQERotor);
    ini.Set("BlockNames", "MouseXRotor", nameMouseXRotor);
    ini.Set("BlockNames", "MouseYRotor", nameMouseYRotor);

    ini.Set("Park", "BaseRotor_Angle", parkBaseAngle);
    ini.Set("Park", "BaseRotor_Order", parkBaseOrder);
    ini.Set("Park", "QERotor_Angle", parkQEAngle);
    ini.Set("Park", "QERotor_Order", parkQEOrder);
    ini.Set("Park", "MouseXRotor_Angle", parkMouseXAngle);
    ini.Set("Park", "MouseXRotor_Order", parkMouseXOrder);
    ini.Set("Park", "MouseYRotor_Angle", parkMouseYAngle);
    ini.Set("Park", "MouseYRotor_Order", parkMouseYOrder);
    ini.Set("Park", "Arm_Order", parkArmOrder);

    ini.Set("ATC", "Attacher", nameAttacher);
    ini.Set("ATC", "CurrentTool", currentTool);

    plateNames.Clear();
    toolPosCache.Clear();
    List<VRage.Game.ModAPI.Ingame.Utilities.MyIniKey> atcKeys = new List<VRage.Game.ModAPI.Ingame.Utilities.MyIniKey>();
    parsedIni.GetKeys("ATC", atcKeys);

    for (int i = 1; i <= 3; i++) {
        string val = parsedIni.Get("ATC", "Tool_" + i).ToString("Plate " + i);
        plateNames[i] = val;
        ini.Set("ATC", "Tool_" + i, val);
    }

    foreach (var key in atcKeys) {
        if (key.Name.StartsWith("Tool_")) {
            string nStr = key.Name.Substring(5);
            int id;
            if (int.TryParse(nStr, out id) && id > 3) {
                string val = parsedIni.Get("ATC", key.Name).ToString();
                plateNames[id] = val;
                ini.Set("ATC", key.Name, val);
            }
        }
    }

    ini.Set("State", "IsCraneActive", isCraneActive);
    ini.Set("State", "SavedThrusters", savedControlThrusters);
    ini.Set("State", "SavedGyros", savedControlGyros);

    int gyroSnapshotCount = parsedIni.Get("GyroHoldState", "Count").ToInt32(0);
    ini.Set("GyroHoldState", "Count", gyroSnapshotCount);
    for (int i = 0; i < gyroSnapshotCount; i++) {
        string sec = "GyroHoldState_" + i;
        ini.Set(sec, "Name", parsedIni.Get(sec, "Name").ToString());
        ini.Set(sec, "Enabled", parsedIni.Get(sec, "Enabled").ToBoolean());
        ini.Set(sec, "Override", parsedIni.Get(sec, "Override").ToBoolean());
        ini.Set(sec, "Pitch", parsedIni.Get(sec, "Pitch").ToSingle());
        ini.Set(sec, "Yaw", parsedIni.Get(sec, "Yaw").ToSingle());
        ini.Set(sec, "Roll", parsedIni.Get(sec, "Roll").ToSingle());
        ini.Set(sec, "Power", parsedIni.Get(sec, "Power").ToSingle());
    }

    List<string> sections = new List<string>();
    parsedIni.GetSections(sections);
    foreach (var sec in sections) {
        if (sec.StartsWith("ATC_Tool_")) {
            ini.Set(sec, "X", parsedIni.Get(sec, "X").ToDouble());
            ini.Set(sec, "Y", parsedIni.Get(sec, "Y").ToDouble());
            ini.Set(sec, "SafeY", parsedIni.Get(sec, "SafeY").ToDouble());
            ini.Set(sec, "Base", parsedIni.Get(sec, "Base").ToSingle());
            ini.Set(sec, "QE", parsedIni.Get(sec, "QE").ToSingle());
            ini.Set(sec, "MX", parsedIni.Get(sec, "MX").ToSingle());
            ini.Set(sec, "MY", parsedIni.Get(sec, "MY").ToSingle());
            ini.Set(sec, "IsValid", parsedIni.Get(sec, "IsValid").ToBoolean());
        }
    }

    Me.CustomData = ini.ToString();
}

string GetPlateName(int id) {
    if (plateNames.ContainsKey(id)) return plateNames[id];
    return "Plate " + id;
}

void InitBlocks()
{
    cockpit = FindAndTagBlock<IMyShipController>(nameCockpit);
    baseRotor = FindAndTagBlock<IMyMotorStator>(nameBaseRotor);
    shoulder = FindAndTagBlock<IMyMotorStator>(nameShoulder);
    elbow1 = FindAndTagBlock<IMyMotorStator>(nameElbow1);
    elbow2 = FindAndTagBlock<IMyMotorStator>(nameElbow2);
    wrist = FindAndTagBlock<IMyMotorStator>(nameWrist);
    qeBlock = FindAndTagBlock<IMyMotorStator>(nameQERotor);
    mouseXBlock = FindAndTagBlock<IMyMotorStator>(nameMouseXRotor);
    mouseYBlock = FindAndTagBlock<IMyMotorStator>(nameMouseYRotor);
    attacherBlock = FindAndTagBlock<IMyMotorStator>(nameAttacher);

    craneLights.Clear();
    GridTerminalSystem.GetBlocksOfType(craneLights, b => b.CustomName.Contains(cfgTag));
    RefreshShipGyros();

    lcd = Me.GetSurface(0);
    if (lcd != null)
    {
        lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        lcd.FontSize = 1.05f;
        lcd.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
    }
}

void SetUpdateFrequency()
{
    if (isCraneActive || isParking || isATCActive || isSetupMode || isSetupFinishing)
        Runtime.UpdateFrequency = UpdateFrequency.Update1;
    else
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

void ApplyRestoredState()
{
    if (isCraneActive) {
        SetRotorLocks(false);
        if (!LoadGyroSnapshot()) CaptureGyroStates();
        ApplyGyroHold();
        if (cockpit != null) {
            cockpit.ControlThrusters = false;
            cockpit.SetValueBool("ControlGyros", false);
        }
    } else {
        SetRotorLocks(true);
    }
}

void RefreshShipGyros()
{
    shipGyros.Clear();
    var controlGrid = cockpit != null ? cockpit.CubeGrid : Me.CubeGrid;
    GridTerminalSystem.GetBlocksOfType(shipGyros, g => g.CubeGrid == controlGrid);
}

void CaptureGyroStates()
{
    if (!cfgGyroHold || gyroStatesSaved) return;

    RefreshShipGyros();
    savedGyroStates.Clear();

    foreach (var g in shipGyros) {
        if (g == null) continue;

        GyroState st = new GyroState();
        st.Gyro = g;
        st.Enabled = g.Enabled;
        st.Override = g.GyroOverride;
        st.Pitch = g.Pitch;
        st.Yaw = g.Yaw;
        st.Roll = g.Roll;
        st.Power = g.GyroPower;
        savedGyroStates.Add(st);
    }

    gyroStatesSaved = true;
    SaveGyroSnapshot();
}

void ApplyGyroHold()
{
    if (!cfgGyroHold) return;

    if (!gyroStatesSaved) CaptureGyroStates();
    if (shipGyros.Count == 0) RefreshShipGyros();

    foreach (var g in shipGyros) {
        if (g == null) continue;
        g.Enabled = true;
        g.GyroPower = cfgGyroHoldPower;
        g.Pitch = 0f;
        g.Yaw = 0f;
        g.Roll = 0f;
        g.GyroOverride = true;
    }
}

void RestoreGyroStates()
{
    if (!gyroStatesSaved && !LoadGyroSnapshot()) return;

    foreach (var st in savedGyroStates) {
        if (st.Gyro == null) continue;
        st.Gyro.Pitch = st.Pitch;
        st.Gyro.Yaw = st.Yaw;
        st.Gyro.Roll = st.Roll;
        st.Gyro.GyroPower = st.Power;
        st.Gyro.GyroOverride = st.Override;
        st.Gyro.Enabled = st.Enabled;
    }

    savedGyroStates.Clear();
    gyroStatesSaved = false;
    ClearGyroSnapshot();
}

void SaveGyroSnapshot()
{
    var saveIni = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();
    saveIni.TryParse(Me.CustomData);
    saveIni.Set("GyroHoldState", "Count", savedGyroStates.Count);

    for (int i = 0; i < savedGyroStates.Count; i++) {
        GyroState st = savedGyroStates[i];
        string sec = "GyroHoldState_" + i;
        saveIni.Set(sec, "Name", st.Gyro != null ? st.Gyro.CustomName : "");
        saveIni.Set(sec, "Enabled", st.Enabled);
        saveIni.Set(sec, "Override", st.Override);
        saveIni.Set(sec, "Pitch", st.Pitch);
        saveIni.Set(sec, "Yaw", st.Yaw);
        saveIni.Set(sec, "Roll", st.Roll);
        saveIni.Set(sec, "Power", st.Power);
    }

    Me.CustomData = saveIni.ToString();
}

bool LoadGyroSnapshot()
{
    var loadIni = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();
    loadIni.TryParse(Me.CustomData);
    int count = loadIni.Get("GyroHoldState", "Count").ToInt32(0);
    if (count <= 0) return false;

    RefreshShipGyros();
    savedGyroStates.Clear();

    for (int i = 0; i < count; i++) {
        string sec = "GyroHoldState_" + i;
        string name = loadIni.Get(sec, "Name").ToString();
        IMyGyro gyro = FindGyroByName(name);
        if (gyro == null) continue;

        GyroState st = new GyroState();
        st.Gyro = gyro;
        st.Enabled = loadIni.Get(sec, "Enabled").ToBoolean(gyro.Enabled);
        st.Override = loadIni.Get(sec, "Override").ToBoolean(gyro.GyroOverride);
        st.Pitch = loadIni.Get(sec, "Pitch").ToSingle(gyro.Pitch);
        st.Yaw = loadIni.Get(sec, "Yaw").ToSingle(gyro.Yaw);
        st.Roll = loadIni.Get(sec, "Roll").ToSingle(gyro.Roll);
        st.Power = loadIni.Get(sec, "Power").ToSingle(gyro.GyroPower);
        savedGyroStates.Add(st);
    }

    gyroStatesSaved = savedGyroStates.Count > 0;
    return gyroStatesSaved;
}

IMyGyro FindGyroByName(string name)
{
    if (string.IsNullOrWhiteSpace(name)) return null;
    foreach (var g in shipGyros) {
        if (g != null && g.CustomName == name) return g;
    }
    return null;
}

void ClearGyroSnapshot()
{
    var saveIni = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();
    saveIni.TryParse(Me.CustomData);
    saveIni.Set("GyroHoldState", "Count", 0);
    Me.CustomData = saveIni.ToString();
}

void SyncArmPosition()
{
    if (shoulder != null && elbow1 != null)
    {
        double rawShoulder = GetLogicalRotorAngleRad(shoulder);
        double rawElbow1 = GetLogicalRotorAngleRad(elbow1);

        double sAngle = rawShoulder + Math.PI / 2;
        double eAngle = -rawElbow1 * 2.0;
        targetX = cfgL1 * Math.Cos(sAngle) + cfgL2 * Math.Cos(sAngle - eAngle);
        targetY = cfgL1 * Math.Sin(sAngle) + cfgL2 * Math.Sin(sAngle - eAngle);

        ClampIKTarget();
        destinationX = targetX;
        destinationY = targetY;
    }
    else {
        targetX = startX;
        targetY = startY;
        destinationX = startX;
        destinationY = startY;
    }
}

void ClampIKTarget()
{
    double distSq = targetX * targetX + targetY * targetY;
    double dist = Math.Sqrt(distSq);

    double maxDist = (cfgL1 + cfgL2) * 0.98;
    double minDist = Math.Abs(cfgL1 - cfgL2) + 0.05;
    if (minDist < 0.5) minDist = 0.5;
    if (minDist > maxDist) minDist = maxDist * 0.5;

    if (dist > maxDist) {
        targetX *= maxDist / dist;
        targetY *= maxDist / dist;
        return;
    }

    if (dist < minDist) {
        if (dist < 0.001) {
            targetX = 0;
            targetY = minDist;
        } else {
            targetX *= minDist / dist;
            targetY *= minDist / dist;
        }
    }
}

void SetCraneState(bool activate)
{
    if (isCraneActive == activate) return;

    if (activate)
    {
        if (cockpit != null && cockpit.GetShipSpeed() > cfgMaxSafeSpeed) {
            SetStatusMessage("Cannot activate: ship speed is above MaxSafeSpeed.");
            return;
        }

        isCraneActive = true;
        isParking = false;
        isATCActive = false;
        isSetupFinishing = false;
        SetRotorLocks(false);
        CaptureGyroStates();
        ApplyGyroHold();
        SyncArmPosition();
        ClearStatusMessage();

        if (cockpit != null)
        {
            bool hasShipControl = cockpit.ControlThrusters || cockpit.GetValueBool("ControlGyros");
            if (hasShipControl)
            {
                savedControlThrusters = cockpit.ControlThrusters;
                savedControlGyros = cockpit.GetValueBool("ControlGyros");
                cockpit.ControlThrusters = false;
                cockpit.SetValueBool("ControlGyros", false);
            }
            else
            {
                savedControlThrusters = false;
                savedControlGyros = false;
            }
        }
    }
    else
    {
        isCraneActive = false;
        isATCActive = false;
        isSetupMode = false;
        isSetupFinishing = false;
        ClearSolidRedAlert();
        StopAuxRotors();
        RestoreGyroStates();
        if (cockpit != null)
        {
            cockpit.ControlThrusters = savedControlThrusters;
            cockpit.SetValueBool("ControlGyros", savedControlGyros);
        }
        SetRotorLocks(true);
    }

    SetUpdateFrequency();
    Save();
}

void StartParking()
{
    if (!isCraneActive) SetCraneState(true);
    if (!isCraneActive) return;

    isParking = true;
    isATCActive = false;
    isSetupMode = false;
    currentParkPhase = 1;
    maxParkPhase = Math.Max(Math.Max(parkBaseOrder, parkQEOrder), Math.Max(Math.Max(parkMouseXOrder, parkMouseYOrder), parkArmOrder));
    SetUpdateFrequency();
}

T FindAndTagBlock<T>(string baseName) where T : class, IMyTerminalBlock
{
    if (string.IsNullOrWhiteSpace(baseName)) return null;
    string taggedName = cfgTag + " " + baseName;
    var block = GridTerminalSystem.GetBlockWithName(taggedName) as T;
    if (block == null)
    {
        block = GridTerminalSystem.GetBlockWithName(baseName) as T;
        if (block != null && !block.CustomName.Contains(cfgTag)) block.CustomName = taggedName;
    }
    return block;
}

IMyLandingGear FindToolPlate(int id)
{
    string plateName = GetPlateName(id);
    if (string.IsNullOrWhiteSpace(plateName)) return null;

    var plate = GridTerminalSystem.GetBlockWithName(plateName) as IMyLandingGear;
    if (plate != null) return plate;

    if (!plateName.Contains(cfgTag)) {
        plate = GridTerminalSystem.GetBlockWithName(cfgTag + " " + plateName) as IMyLandingGear;
    }

    return plate;
}

public void Main(string argument, UpdateType updateSource)
{
    tickCounter++;

    if (cockpit == null || baseRotor == null)
    {
        if (tickCounter % 300 == 0) InitBlocks();
        if (cockpit == null) return;
    }

    bool currentlyUnderControl = cockpit.IsUnderControl;

    if (!currentlyUnderControl && wasUnderControl)
    {
        if (!isParking && !isATCActive && !isSetupFinishing && !isSetupMode)
        {
            SetCraneState(false);
        }
    }
    wasUnderControl = currentlyUnderControl;

    if (!string.IsNullOrWhiteSpace(argument))
    {
        string arg = argument.ToLower().Trim();
        if (arg == "on") SetCraneState(true);
        else if (arg == "off") SetCraneState(false);
        else if (arg == "toggle" || arg == "switch") SetCraneState(!isCraneActive);
        else if (arg == "reset")
        {
            destinationX = startX;
            destinationY = startY;
            isParking = false;
            isATCActive = false;
            isSetupMode = false;
            isSetupFinishing = false;
            setupPhase = 0;
            ClearStatusMessage();
            InitBlocks();
            SetUpdateFrequency();
        }
        else if (arg == "park")
        {
            if (isParking) {
                isParking = false;
                SyncArmPosition();
                SetCraneState(true);
            }
            else StartParking();
        }
        else if (arg == "setup")
        {
            if (isATCActive) CancelATC("ATC cancelled for setup.");
            else {
                if (!isCraneActive) SetCraneState(true);
                if (isCraneActive) {
                    isSetupMode = !isSetupMode;
                    if (isSetupMode) SetStatusMessage("Setup mode: move crane, then run 'tool N'.");
                    else ClearStatusMessage();
                }
            }
            SetUpdateFrequency();
        }
        else if (arg.StartsWith("tool"))
        {
            string idText = arg.Length > 4 ? arg.Substring(4).Trim() : "";
            int tId;
            if (int.TryParse(idText, out tId) && tId > 0) {
                if (isATCActive) {
                    CancelATC("ATC cancelled.");
                } else if (isSetupMode) {
                    SaveToolPos(tId);

                    IMyLandingGear magPlate = FindToolPlate(tId);
                    if (magPlate != null) magPlate.Lock();
                    else SetStatusMessage("Warning: tool plate not found: " + GetPlateName(tId));

                    setupTargetTool = tId;
                    isSetupFinishing = true;
                    setupTimer = 0;
                    setupPhase = 1;
                    isSetupMode = false;
                    SetUpdateFrequency();
                } else {
                    StartATC(tId);
                }
            }
        }
    }

    if (tickCounter % (isCraneActive ? 10 : 3) == 0)
    {
        animFrame = (animFrame + 1) % animSequence.Length;
        UpdateScreen();
        UpdateLights();
    }

    if (!isCraneActive) return;

    ApplyGyroHold();

    if (isSetupFinishing)
    {
        SetRotorLocks(false);
        RunSetupFinishSequence();
        if (!isCraneActive) return;
    }
    else if (isATCActive)
    {
        SetRotorLocks(false);
        RunATCSequence();
    }
    else if (isParking)
    {
        SetRotorLocks(false);
        RunParkingSequence();
    }
    else if (cockpit != null)
    {
        if (!cockpit.IsUnderControl)
        {
            SetRotorLocks(true);
            return;
        }
        SetRotorLocks(false);

        Vector3 moveIndicator = cockpit.MoveIndicator;
        Vector2 mouseIndicator = cockpit.RotationIndicator;
        float rollIndicator = cockpit.RollIndicator;

        if (Math.Abs(moveIndicator.X) > 0.05f && baseRotor != null)
            baseRotor.TargetVelocityRPM = moveIndicator.X * cfgBaseRotateSpeed * (IsInverted(baseRotor) ? -1f : 1f);
        else if (baseRotor != null) baseRotor.TargetVelocityRPM = 0;

        if (Math.Abs(rollIndicator) > 0.05f && qeBlock != null)
            qeBlock.TargetVelocityRPM = rollIndicator * cfgQESpeed * (IsInverted(qeBlock) ? -1f : 1f);
        else if (qeBlock != null) qeBlock.TargetVelocityRPM = 0;

        if (mouseXBlock != null) mouseXBlock.TargetVelocityRPM = (Math.Abs(mouseIndicator.Y) > 0.1f) ? mouseIndicator.Y * cfgMouseSpeed * (IsInverted(mouseXBlock) ? 1f : -1f) : 0;
        if (mouseYBlock != null) mouseYBlock.TargetVelocityRPM = (Math.Abs(mouseIndicator.X) > 0.1f) ? mouseIndicator.X * cfgMouseSpeed * (IsInverted(mouseYBlock) ? -1f : 1f) : 0;

        if (Math.Abs(moveIndicator.Z) > 0.05f) destinationX += -moveIndicator.Z * cfgMoveSpeed;

        float inputY = moveIndicator.Y;
        if (cfgInvertKinematic) inputY = -inputY;
        if (Math.Abs(inputY) > 0.05f) destinationY += inputY * cfgMoveSpeed;
    }

    double dX = destinationX - targetX;
    double dY = destinationY - targetY;
    double distToDest = Math.Sqrt(dX * dX + dY * dY);

    if (distToDest > 0.001)
    {
        double step = Math.Min(distToDest, cfgMoveSpeed);
        targetX += (dX / distToDest) * step;
        targetY += (dY / distToDest) * step;
    }

    ClampIKTarget();
    destinationX = targetX;
    destinationY = targetY;

    ApplyIKToArm();
}

void ApplyIKToArm()
{
    double distSq = targetX * targetX + targetY * targetY;
    double cosAlpha = (distSq - cfgL1 * cfgL1 - cfgL2 * cfgL2) / (2 * cfgL1 * cfgL2);
    double elbowAngleRad = Math.Acos(MathHelper.Clamp((float)cosAlpha, -1f, 1f));
    double shoulderAngleRad = Math.Atan2(targetY, targetX) + Math.Atan2(cfgL2 * Math.Sin(elbowAngleRad), cfgL1 + cfgL2 * Math.Cos(elbowAngleRad));

    float wristOffsetRad = MathHelper.ToRadians(cfgWristOffset);

    ApplyAngle(shoulder, (float)(shoulderAngleRad - Math.PI / 2));
    ApplyAngle(elbow1, (float)(-elbowAngleRad / 2.0));
    ApplyAngle(elbow2, (float)(-elbowAngleRad / 2.0));
    ApplyAngle(wrist, (float)(-shoulderAngleRad + elbowAngleRad) + wristOffsetRad);
}

// ================= LIGHTS LOGIC =================
void UpdateLights()
{
    foreach (var l in craneLights)
    {
        if (solidRedAlert)
        {
            l.Enabled = true;
            l.Radius = 20f;
            l.Color = Color.Red;
            l.BlinkIntervalSeconds = 0f;
            l.BlinkLength = 0f;
            continue;
        }

        if (!isCraneActive && !isParking && !isATCActive && !isSetupMode && !isSetupFinishing)
        {
            l.Enabled = false;
            continue;
        }

        l.Enabled = true;
        l.Radius = 20f;

        if (isSetupMode || isSetupFinishing)
        {
            l.Color = Color.Red;
            l.BlinkIntervalSeconds = 0.5f;
            l.BlinkLength = 50f;
        }
        else if (isParking || isATCActive)
        {
            l.Color = Color.Red;
            l.BlinkIntervalSeconds = 1.5f;
            l.BlinkLength = 50f;
        }
        else
        {
            l.Color = Color.White;
            l.BlinkIntervalSeconds = 0f;
        }
    }
}

// ================= ATC LOGIC =================

void SaveToolPos(int id) {
    var saveIni = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();
    saveIni.TryParse(Me.CustomData);
    string section = "ATC_Tool_" + id;

    ToolPos tp = new ToolPos();
    tp.X = targetX;
    tp.Y = targetY;
    tp.SafeY = targetY + cfgSafeDistance;
    tp.Base = baseRotor != null ? GetLogicalRotorAngleDeg(baseRotor) : 0f;
    tp.QE = qeBlock != null ? GetLogicalRotorAngleDeg(qeBlock) : 0f;
    tp.MX = mouseXBlock != null ? GetLogicalRotorAngleDeg(mouseXBlock) : 0f;
    tp.MY = mouseYBlock != null ? GetLogicalRotorAngleDeg(mouseYBlock) : 0f;
    tp.IsValid = true;

    saveIni.Set(section, "X", tp.X);
    saveIni.Set(section, "Y", tp.Y);
    saveIni.Set(section, "SafeY", tp.SafeY);
    saveIni.Set(section, "Base", tp.Base);
    saveIni.Set(section, "QE", tp.QE);
    saveIni.Set(section, "MX", tp.MX);
    saveIni.Set(section, "MY", tp.MY);
    saveIni.Set(section, "IsValid", true);

    Me.CustomData = saveIni.ToString();
    toolPosCache[id] = tp;

    SetStatusMessage("Saved ATC position for " + GetPlateName(id) + ".");
}

void RunSetupFinishSequence()
{
    ToolPos tp = GetToolPos(setupTargetTool);
    if (!tp.IsValid) {
        isSetupFinishing = false;
        setupPhase = 0;
        SetStatusMessage("Setup cancelled: saved tool position is invalid.");
        SetUpdateFrequency();
        return;
    }

    if (setupPhase <= 0) setupPhase = 1;

    switch (setupPhase) {
        case 1:
            destinationX = targetX;
            destinationY = targetY;

            IMyLandingGear magPlate = FindToolPlate(setupTargetTool);
            if (magPlate == null) {
                SetSolidRedAlert("SETUP BLOCKED: tool plate not found: " + GetPlateName(setupTargetTool));
                return;
            }

            if (!IsPlateLocked(magPlate)) {
                magPlate.Lock();
                SetSolidRedAlert("SETUP BLOCKED: plate is not locked. Detach disabled.");
                return;
            }

            ClearSolidRedAlert();

            if (attacherBlock == null) {
                SetSolidRedAlert("SETUP BLOCKED: attacher rotor not found.");
                return;
            }

            if (attacherBlock.IsAttached) {
                SetStatusMessage("Plate locked. Detaching tool.");
                attacherBlock.Detach();
                return;
            }

            currentTool = 0;
            Save();

            destinationX = tp.X;
            destinationY = tp.SafeY;
            setupPhase = 2;
            setupTimer = 0;
            SetStatusMessage("Setup saved. Raising to safe height.");
            break;

        case 2:
            destinationX = tp.X;
            destinationY = tp.SafeY;

            if (Math.Abs(targetX - destinationX) < 0.05 && Math.Abs(targetY - destinationY) < 0.05 && IsArmAtRest()) {
                setupTimer++;
                if (setupTimer > 10) {
                    isSetupFinishing = false;
                    setupPhase = 0;
                    SetUpdateFrequency();

                    if (cockpit == null || !cockpit.IsUnderControl) {
                        SetCraneState(false);
                    }
                }
            } else {
                setupTimer = 0;
            }
            break;
    }
}

ToolPos GetToolPos(int id) {
    ToolPos cached;
    if (toolPosCache.TryGetValue(id, out cached)) return cached;

    var tIni = new VRage.Game.ModAPI.Ingame.Utilities.MyIni();
    tIni.TryParse(Me.CustomData);
    string section = "ATC_Tool_" + id;
    ToolPos tp = new ToolPos();
    tp.IsValid = tIni.Get(section, "IsValid").ToBoolean(false);
    if (tp.IsValid) {
        tp.X = tIni.Get(section, "X").ToDouble();
        tp.Y = tIni.Get(section, "Y").ToDouble();
        tp.SafeY = tIni.Get(section, "SafeY").ToDouble();
        tp.Base = tIni.Get(section, "Base").ToSingle();
        tp.QE = tIni.Get(section, "QE").ToSingle();
        tp.MX = tIni.Get(section, "MX").ToSingle();
        tp.MY = tIni.Get(section, "MY").ToSingle();
    }
    toolPosCache[id] = tp;
    return tp;
}

bool IsArmAtRest() {
    float tolerance = 0.05f;
    if (IsRotorMoving(shoulder, tolerance)) return false;
    if (IsRotorMoving(elbow1, tolerance)) return false;
    if (IsRotorMoving(elbow2, tolerance)) return false;
    if (IsRotorMoving(wrist, tolerance)) return false;
    if (IsRotorMoving(baseRotor, tolerance)) return false;
    if (IsRotorMoving(qeBlock, tolerance)) return false;
    if (IsRotorMoving(mouseXBlock, tolerance)) return false;
    if (IsRotorMoving(mouseYBlock, tolerance)) return false;
    return IsArmNearIKTarget(0.04f);
}

bool IsRotorUsable(IMyMotorStator rotor)
{
    return rotor != null && rotor.IsAttached;
}

bool IsRotorMoving(IMyMotorStator rotor, float tolerance)
{
    return IsRotorUsable(rotor) && Math.Abs(rotor.TargetVelocityRPM) > tolerance;
}

bool IsArmNearIKTarget(float toleranceRad)
{
    double distSq = targetX * targetX + targetY * targetY;
    double cosAlpha = (distSq - cfgL1 * cfgL1 - cfgL2 * cfgL2) / (2 * cfgL1 * cfgL2);
    double elbowAngleRad = Math.Acos(MathHelper.Clamp((float)cosAlpha, -1f, 1f));
    double shoulderAngleRad = Math.Atan2(targetY, targetX) + Math.Atan2(cfgL2 * Math.Sin(elbowAngleRad), cfgL1 + cfgL2 * Math.Cos(elbowAngleRad));
    float wristOffsetRad = MathHelper.ToRadians(cfgWristOffset);

    float shoulderTarget = (float)(shoulderAngleRad - Math.PI / 2);
    float elbowTarget = (float)(-elbowAngleRad / 2.0);
    float wristTarget = (float)(-shoulderAngleRad + elbowAngleRad) + wristOffsetRad;

    if (IsRotorUsable(shoulder) && AbsAngleDiff(shoulderTarget, GetLogicalRotorAngleRad(shoulder)) > toleranceRad) return false;
    if (IsRotorUsable(elbow1) && AbsAngleDiff(elbowTarget, GetLogicalRotorAngleRad(elbow1)) > toleranceRad) return false;
    if (IsRotorUsable(elbow2) && AbsAngleDiff(elbowTarget, GetLogicalRotorAngleRad(elbow2)) > toleranceRad) return false;
    if (IsRotorUsable(wrist) && AbsAngleDiff(wristTarget, GetLogicalRotorAngleRad(wrist)) > toleranceRad) return false;
    return true;
}

void StartATC(int tId) {
    if (!isCraneActive) SetCraneState(true);
    if (!isCraneActive) return;

    ToolPos requestedPos = GetToolPos(tId);
    if (!requestedPos.IsValid) {
        SetStatusMessage("Tool " + tId + " has no saved ATC position.");
        return;
    }

    if (currentTool != 0 && currentTool != tId) {
        ToolPos currentPos = GetToolPos(currentTool);
        if (!currentPos.IsValid) {
            SetStatusMessage("Current tool has no saved ATC position.");
            return;
        }
        targetTool = currentTool;
        queuedTool = tId;
    } else {
        targetTool = tId;
        queuedTool = 0;
    }

    isATCActive = true;
    isParking = false;
    isSetupMode = false;
    atcPhase = 1;
    atcWaitTimer = 0;
    ResetATCWatchdog();

    ToolPos tpTarget = GetToolPos(targetTool);
    atcSafeY = tpTarget.IsValid ? tpTarget.SafeY : targetY + cfgSafeDistance;

    destinationX = targetX;
    StopAuxRotors();
    ClearStatusMessage();
    SetUpdateFrequency();
}

void CancelATC(string reason) {
    isATCActive = false;
    queuedTool = 0;
    atcPhase = 0;
    ResetATCWatchdog();
    destinationX = targetX;
    destinationY = targetY;
    StopAuxRotors();
    if (!string.IsNullOrWhiteSpace(reason)) SetStatusMessage(reason);
    SetUpdateFrequency();
}

void RunATCSequence() {
    ToolPos tp = GetToolPos(targetTool);
    if (!tp.IsValid) {
        CancelATC("ATC cancelled: target tool position is invalid.");
        return;
    }

    if (HasATCStalledOut()) {
        CancelATC("ATC timeout in phase " + atcPhase + ".");
        return;
    }

    bool phaseComplete = false;

    switch (atcPhase) {
        case 1:
            destinationX = targetX;
            destinationY = atcSafeY;
            if (Math.Abs(targetY - destinationY) < 0.05 && IsArmAtRest()) phaseComplete = true;
            break;

        case 2:
            destinationX = tp.X;
            destinationY = atcSafeY;

            bool anglesDone = true;
            if (baseRotor != null && !ParkRotor(baseRotor, tp.Base, cfgBaseRotateSpeed)) anglesDone = false;
            if (qeBlock != null && !ParkRotor(qeBlock, tp.QE, cfgQESpeed)) anglesDone = false;
            if (mouseXBlock != null && !ParkRotor(mouseXBlock, tp.MX, cfgMouseXAutoSpeed)) anglesDone = false;
            if (mouseYBlock != null && !ParkRotor(mouseYBlock, tp.MY, cfgMouseYAutoSpeed)) anglesDone = false;

            if (anglesDone && Math.Abs(targetX - destinationX) < 0.1 && Math.Abs(targetY - destinationY) < 0.1 && IsArmAtRest()) {
                StopAuxRotors();
                phaseComplete = true;
            }
            break;

        case 3:
            destinationX = tp.X;
            destinationY = tp.Y;

            if (Math.Abs(targetY - destinationY) < 0.01 && Math.Abs(targetX - destinationX) < 0.01 && IsArmAtRest()) {
                atcWaitTimer++;
                if (atcWaitTimer > 30) phaseComplete = true;
            }
            break;

        case 4:
            if (targetTool == currentTool) {
                IMyLandingGear magPlate = FindToolPlate(targetTool);
                if (magPlate == null) {
                    CancelATC("ATC cancelled: plate not found for " + GetPlateName(targetTool) + ".");
                    return;
                }
                magPlate.Lock();
                if (IsPlateLocked(magPlate)) phaseComplete = true;
            } else {
                if (attacherBlock == null) {
                    CancelATC("ATC cancelled: attacher rotor not found.");
                    return;
                }
                if (!attacherBlock.IsAttached) attacherBlock.Attach();
                if (attacherBlock.IsAttached) phaseComplete = true;
            }
            break;

        case 5:
            atcWaitTimer++;
            if (atcWaitTimer > 15) phaseComplete = true;
            break;

        case 6:
            if (targetTool == currentTool) {
                if (attacherBlock == null) {
                    CancelATC("ATC cancelled: attacher rotor not found.");
                    return;
                }
                if (attacherBlock.IsAttached) {
                    attacherBlock.Detach();
                    break;
                }
                currentTool = 0;
                Save();
                phaseComplete = true;
            } else {
                IMyLandingGear magPlate2 = FindToolPlate(targetTool);
                if (magPlate2 == null) {
                    CancelATC("ATC cancelled: plate not found for " + GetPlateName(targetTool) + ".");
                    return;
                }
                if (IsPlateLocked(magPlate2)) {
                    magPlate2.Unlock();
                    break;
                }
                currentTool = targetTool;
                Save();
                phaseComplete = true;
            }
            break;

        case 7:
            atcWaitTimer++;
            if (atcWaitTimer > 15) phaseComplete = true;
            break;

        case 8:
            destinationX = tp.X;
            destinationY = tp.SafeY;
            if (Math.Abs(targetY - destinationY) < 0.05 && Math.Abs(targetX - destinationX) < 0.05 && IsArmAtRest()) phaseComplete = true;
            break;

        case 9:
            if (queuedTool != 0) {
                targetTool = queuedTool;
                queuedTool = 0;
                atcPhase = 1;
                atcWaitTimer = 0;
                ResetATCWatchdog();
                ToolPos nextTp = GetToolPos(targetTool);
                atcSafeY = nextTp.IsValid ? nextTp.SafeY : targetY + cfgSafeDistance;
            } else {
                isATCActive = false;
                StopAuxRotors();
                SetUpdateFrequency();

                if (cockpit == null || !cockpit.IsUnderControl) {
                    SetCraneState(false);
                }
            }
            break;
    }

    if (phaseComplete && atcPhase < 9) {
        atcPhase++;
        atcWaitTimer = 0;
        ResetATCWatchdog();
    }
}

bool HasATCStalledOut()
{
    if (cfgATCTimeoutTicks <= 0 || atcPhase <= 0 || atcPhase >= 9) return false;

    if (!atcWatchInitialized) {
        SaveATCWatchdogSnapshot();
        return false;
    }

    if (HasATCProgress()) {
        atcPhaseTimer = 0;
        SaveATCWatchdogSnapshot();
        return false;
    }

    atcPhaseTimer++;
    return atcPhaseTimer > cfgATCTimeoutTicks;
}

bool HasATCProgress()
{
    if (Math.Abs(targetX - atcWatchX) > 0.002 || Math.Abs(targetY - atcWatchY) > 0.002) return true;
    if (RotorAngleChanged(baseRotor, atcWatchBase)) return true;
    if (RotorAngleChanged(shoulder, atcWatchShoulder)) return true;
    if (RotorAngleChanged(elbow1, atcWatchElbow1)) return true;
    if (RotorAngleChanged(elbow2, atcWatchElbow2)) return true;
    if (RotorAngleChanged(wrist, atcWatchWrist)) return true;
    if (RotorAngleChanged(qeBlock, atcWatchQE)) return true;
    if (RotorAngleChanged(mouseXBlock, atcWatchMouseX)) return true;
    if (RotorAngleChanged(mouseYBlock, atcWatchMouseY)) return true;
    return false;
}

bool RotorAngleChanged(IMyMotorStator rotor, float lastAngle)
{
    return IsRotorUsable(rotor) && AbsAngleDiff(GetLogicalRotorAngleRad(rotor), lastAngle) > 0.001f;
}

void ResetATCWatchdog()
{
    atcPhaseTimer = 0;
    atcWatchInitialized = false;
}

void SaveATCWatchdogSnapshot()
{
    atcWatchX = targetX;
    atcWatchY = targetY;
    atcWatchBase = GetLogicalRotorAngleRad(baseRotor);
    atcWatchShoulder = GetLogicalRotorAngleRad(shoulder);
    atcWatchElbow1 = GetLogicalRotorAngleRad(elbow1);
    atcWatchElbow2 = GetLogicalRotorAngleRad(elbow2);
    atcWatchWrist = GetLogicalRotorAngleRad(wrist);
    atcWatchQE = GetLogicalRotorAngleRad(qeBlock);
    atcWatchMouseX = GetLogicalRotorAngleRad(mouseXBlock);
    atcWatchMouseY = GetLogicalRotorAngleRad(mouseYBlock);
    atcWatchInitialized = true;
}

// ================= PARKING & MOVEMENT =================

void RunParkingSequence()
{
    bool phaseComplete = true;

    if (parkBaseOrder == currentParkPhase && baseRotor != null)
        if (!ParkRotor(baseRotor, parkBaseAngle, cfgBaseRotateSpeed)) phaseComplete = false;

    if (parkQEOrder == currentParkPhase && qeBlock != null)
        if (!ParkRotor(qeBlock, parkQEAngle, cfgQESpeed)) phaseComplete = false;

    if (parkMouseXOrder == currentParkPhase && mouseXBlock != null)
        if (!ParkRotor(mouseXBlock, parkMouseXAngle, cfgMouseXAutoSpeed)) phaseComplete = false;

    if (parkMouseYOrder == currentParkPhase && mouseYBlock != null)
        if (!ParkRotor(mouseYBlock, parkMouseYAngle, cfgMouseYAutoSpeed)) phaseComplete = false;

    if (parkArmOrder == currentParkPhase)
    {
        destinationX = startX;
        destinationY = startY;

        if (Math.Abs(targetX - startX) > 0.05 || Math.Abs(targetY - startY) > 0.05)
        {
            phaseComplete = false;
        }
        else
        {
            if (!IsArmAtRest()) phaseComplete = false;
        }
    }

    if (phaseComplete)
    {
        currentParkPhase++;
        if (currentParkPhase > maxParkPhase)
        {
            isParking = false;
            SetCraneState(false);
        }
    }
}

bool ParkRotor(IMyMotorStator rotor, float targetDeg, float maxSpeedRPM)
{
    if (!IsRotorUsable(rotor)) {
        if (rotor != null) rotor.TargetVelocityRPM = 0;
        return true;
    }

    float targetRad = MathHelper.ToRadians(targetDeg);
    float currentRad = GetLogicalRotorAngleRad(rotor);
    float diff = NormalizeRadians(targetRad - currentRad);

    if (Math.Abs(diff) < 0.002f)
    {
        rotor.TargetVelocityRPM = 0;
        return true;
    }

    float speed = diff * 5f;
    float minSpeed = 0.02f;

    if (Math.Abs(speed) < minSpeed) speed = Math.Sign(speed) * minSpeed;

    float limit = Math.Abs(maxSpeedRPM);
    if (limit < minSpeed) limit = minSpeed;

    speed = MathHelper.Clamp(speed, -limit, limit);
    if (IsInverted(rotor)) speed = -speed;

    rotor.TargetVelocityRPM = speed;
    return false;
}

bool IsInverted(IMyTerminalBlock block) {
    return block != null && block.CustomData.IndexOf("invert", StringComparison.OrdinalIgnoreCase) >= 0;
}

float GetLogicalRotorAngleRad(IMyMotorStator rotor)
{
    if (rotor == null) return 0f;
    return IsInverted(rotor) ? -rotor.Angle : rotor.Angle;
}

float GetLogicalRotorAngleDeg(IMyMotorStator rotor)
{
    return MathHelper.ToDegrees(GetLogicalRotorAngleRad(rotor));
}

float NormalizeRadians(float angle)
{
    while (angle > Math.PI) angle -= (float)Math.PI * 2f;
    while (angle < -Math.PI) angle += (float)Math.PI * 2f;
    return angle;
}

float AbsAngleDiff(float a, float b)
{
    return Math.Abs(NormalizeRadians(a - b));
}

void ApplyAngle(IMyMotorStator hinge, float targetRad)
{
    if (!IsRotorUsable(hinge)) {
        if (hinge != null) hinge.TargetVelocityRPM = 0;
        return;
    }

    float currentRad = GetLogicalRotorAngleRad(hinge);
    float diff = NormalizeRadians(targetRad - currentRad);

    if (Math.Abs(diff) < 0.001f)
    {
        hinge.TargetVelocityRPM = 0;
        return;
    }

    float speed = MathHelper.Clamp(diff * 20f, -20f, 20f);
    if (IsInverted(hinge)) speed = -speed;
    hinge.TargetVelocityRPM = speed;
}

void SetRotorLocks(bool isLocked)
{
    LockSingle(baseRotor, isLocked);
    LockSingle(shoulder, isLocked);
    LockSingle(elbow1, isLocked);
    LockSingle(elbow2, isLocked);
    LockSingle(wrist, isLocked);
    LockSingle(qeBlock, isLocked);
    LockSingle(mouseXBlock, isLocked);
    LockSingle(mouseYBlock, isLocked);
}

void LockSingle(IMyMotorStator r, bool l)
{
    if (r != null)
    {
        if (l) r.TargetVelocityRPM = 0;
        r.RotorLock = l;
    }
}

void StopAuxRotors()
{
    if (baseRotor != null) baseRotor.TargetVelocityRPM = 0;
    if (qeBlock != null) qeBlock.TargetVelocityRPM = 0;
    if (mouseXBlock != null) mouseXBlock.TargetVelocityRPM = 0;
    if (mouseYBlock != null) mouseYBlock.TargetVelocityRPM = 0;
}

bool IsPlateLocked(IMyLandingGear plate)
{
    return plate != null && plate.LockMode.ToString() == "Locked";
}

void SetStatusMessage(string message)
{
    string newMessage = message ?? "";
    bool changed = newMessage != lastMessage;
    lastMessage = newMessage;
    if (!string.IsNullOrWhiteSpace(lastMessage) && changed) Echo(lastMessage);
}

void SetSolidRedAlert(string message)
{
    solidRedAlert = true;
    SetStatusMessage(message);
}

void ClearSolidRedAlert()
{
    solidRedAlert = false;
}

void ClearStatusMessage()
{
    lastMessage = "";
    solidRedAlert = false;
}

void UpdateScreen()
{
    if (lcd == null) return;

    string mode = isCraneActive ? "CRANE" : "SHIP";
    string status = "STANDBY";

    if (solidRedAlert) status = "ALERT";
    else if (isSetupMode || isSetupFinishing) status = "SETUP MODE";
    else if (isATCActive) {
        if (targetTool == currentTool) status = "ATC: DROPPING " + GetPlateName(targetTool);
        else status = "ATC: PICKING " + GetPlateName(targetTool);
    }
    else if (isParking) status = "PARKING...";
    else if (cockpit != null && cockpit.IsUnderControl) status = "ACTIVE";
    else if (isCraneActive) status = "AUTO-STANDBY";

    bool currentGyroStatus = cockpit != null ? cockpit.GetValueBool("ControlGyros") : false;
    string shipCtrl = currentGyroStatus ? "ON" : "OFF";
    string toolName = currentTool == 0 ? "NONE" : GetPlateName(currentTool);

    lcdBuffer.Clear();
    lcdBuffer.Append("=== IK CRANE OS v6.1.15 ===\n\n");
    lcdBuffer.Append("Mode: ").Append(mode).Append(" | Tool: ").Append(toolName).Append("\n");
    lcdBuffer.Append("Cockpit Control: ").Append(shipCtrl).Append("\n");
    lcdBuffer.Append("Status: ").Append(status).Append(" ").Append(animSequence[animFrame]).Append("\n\n");
    lcdBuffer.Append("Target X: ").Append(targetX.ToString("F1")).Append(" m\n");
    lcdBuffer.Append("Target Y: ").Append(targetY.ToString("F1")).Append(" m");

    if (!string.IsNullOrWhiteSpace(lastMessage)) {
        lcdBuffer.Append("\n\n").Append(lastMessage);
    }

    lcd.WriteText(lcdBuffer.ToString());
}
