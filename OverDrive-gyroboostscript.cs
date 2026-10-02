
const int ReInitTime = 360;
string _ODGroupTag = "Overdrive", _mainCockpitTag = "Main";
List<IMyShipController> _shipControllers = new List<IMyShipController>();
List<IMyTerminalBlock> _allBlocks = new List<IMyTerminalBlock>();
List<IMyGyro> _gyros = new List<IMyGyro>();
GuidanceSystem _guidanceSystem = new GuidanceSystem();
string _statusInfo = "";

IMyBlockGroup _overDriveBlockGroup;

IMyShipController _activeShipController, _myShipController;
readonly MyIni _myIni = new MyIni();
const string INI_SECTION_CONTROLS = "Overdrive", INI_YAW_MULT = "Yaw Gyro Multiplier", INI_PITCH_MULT = "Pitch Gyro Multiplier";
float yawMult = 0.02f, pitchMult = 0.02f;
bool _active = false, ready = false;
long _tick = 0;
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
public void Main(string argument, UpdateType updateSource)
{
    _activeShipController = null;
    _statusInfo = "Overdrive script running...\n";
    if ((_tick % ReInitTime) == 0)
    {
        UpdateBlocks();
    }
    if (!ready)
        _statusInfo += "Not found cockpit or gyro!";
    else
    {
        if (_active)
            _statusInfo += "Status: active";
        else
            _statusInfo += "Status: waiting";
    }
    Echo(_statusInfo);
    if (argument == "control")
    {
        _active = !_active;
        if (_active)
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        else
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            _guidanceSystem.Drop(_gyros);
        }
    }
    _tick++;

    if (_myShipController != null)
        _activeShipController = _myShipController;
    else
        foreach (var cocpit in _shipControllers)
        {
            if (cocpit.IsUnderControl)
            {
                _activeShipController = cocpit;
                break;
            }
        }
    if (_activeShipController != null & _active)
    {
        _guidanceSystem.Control( _activeShipController, _gyros, yawMult, pitchMult);
    }
    else if (!_guidanceSystem._firstrun)
        _guidanceSystem.Drop(_gyros);
}

void UpdateBlocks()
{
    LoadIniConfig();
    _shipControllers.Clear();
    _gyros.Clear();
    _allBlocks.Clear();
    _myShipController = null;
    _overDriveBlockGroup = GridTerminalSystem.GetBlockGroupWithName(_ODGroupTag);
    if (_overDriveBlockGroup != null)
    {
        _overDriveBlockGroup.GetBlocks(_allBlocks);
        foreach (var block in _allBlocks)
        {
            SystemHelper.AddToListIfType(block, _shipControllers);
            SystemHelper.AddToListIfType(block, _gyros);
        }
    }
    if (_shipControllers.Count == 0)
        GridTerminalSystem.GetBlocksOfType<IMyShipController>(_shipControllers);
    foreach (var cocpit in _shipControllers)
        if (cocpit.CustomName.Contains(_mainCockpitTag))
            _myShipController = cocpit;
    if (_gyros.Count == 0)
        GridTerminalSystem.GetBlocksOfType<IMyGyro>(_gyros);
    if (_gyros.Count == 0 || _shipControllers.Count == 0)
    {
        ready = false; return;
    }
    else
    {
        ready = true;
    }

}
void LoadIniConfig()
{
    _myIni.Clear();
    bool parsed = _myIni.TryParse(Me.CustomData);
    if (!parsed)
    {
        SaveIniConfig();
        return;
    }
    yawMult = (float)_myIni.Get(INI_SECTION_CONTROLS, INI_YAW_MULT).ToDouble(yawMult);
    pitchMult = (float)_myIni.Get(INI_SECTION_CONTROLS, INI_YAW_MULT).ToDouble(pitchMult);
    SaveIniConfig();
}
void SaveIniConfig()
{
    _myIni.Clear();
    _myIni.Set(INI_SECTION_CONTROLS, INI_YAW_MULT, yawMult);
    _myIni.Set(INI_SECTION_CONTROLS, INI_PITCH_MULT, pitchMult);
    Me.CustomData = _myIni.ToString();
}

}
public class GuidanceSystem
{
    public bool _firstrun { get; private set; }
    const double TURNGYROCONST = 0.0035;
    MatrixD _shipMatrix;
    MatrixD lastShipMatrix = MatrixD.Identity;
    double lastPitch = 0;
    double lastYaw = 0;
    bool b;
    double maneuvrabilityYaw = 0;
    double maneuvrabilityPitch = 0;
    bool fullDriveYaw = false;				//Флаги для замера маневрненности, замеряем маневрненность только при максимальной мощности гироскопа
    bool fullDrivePitch = false;
    public GuidanceSystem()
    {
        _firstrun = true;
    }
    public void Control(IMyShipController shipController, List<IMyGyro> gyros, double yawMult = 0.1, double pitchMult = 0.1)
    {
        double horizont, vertical, roll;
        horizont = shipController.RotationIndicator.X;
        //horizont = 1000;
        vertical = shipController.RotationIndicator.Y;
        roll = 100 * shipController.RollIndicator;
        _shipMatrix = shipController.WorldMatrix;
        Vector3D WorldAngularVelocity = shipController.GetShipVelocities().AngularVelocity;
        Vector3D LocalAngularVelocity = Vector3D.TransformNormal(WorldAngularVelocity, MatrixD.Transpose(_shipMatrix));


        double wantedYaw = vertical * yawMult, wantedPitch = horizont * pitchMult;
        double yawSpeed, pitchSpeed, rollSpeed;
        double ownYaw = -LocalAngularVelocity.Y / 60;               //speed from rad/s to rad/tick
        double ownPitch = -LocalAngularVelocity.X / 60;
        double ownRoll = -LocalAngularVelocity.Z / 60;
        if (_firstrun)
        {
            lastYaw = 0;
            lastPitch = 0;
            lastShipMatrix = _shipMatrix;
            _firstrun = false;
            if (wantedYaw > 0)
            {
                fullDriveYaw = true;
                yawSpeed = ownYaw + TURNGYROCONST;
            }
            else yawSpeed = ownYaw - TURNGYROCONST;
            if (wantedPitch > 0)
            {
                fullDrivePitch = true;
                pitchSpeed = ownPitch + TURNGYROCONST;
            }
            else pitchSpeed = ownPitch - TURNGYROCONST;
        }
        else
        {
            if (fullDriveYaw)
                maneuvrabilityYaw = Math.Abs(lastYaw - ownYaw);
            if (fullDrivePitch)
                maneuvrabilityPitch = Math.Abs(lastPitch - ownPitch);
            b = Math.Abs(wantedYaw - ownYaw) > maneuvrabilityYaw;
            if (b)//rough
            {
                if (ownYaw < wantedYaw)
                {
                    yawSpeed = ownYaw + TURNGYROCONST;
                    fullDriveYaw = true;
                }
                else
                {
                    yawSpeed = ownYaw - TURNGYROCONST;
                    fullDriveYaw = true;
                }
            }
            else//soft
            {
                yawSpeed = wantedYaw - ownYaw;
                fullDriveYaw = false;
            }

            b = Math.Abs(wantedPitch - ownPitch) > maneuvrabilityPitch;
            if (b)//rough
            {
                if (ownPitch < wantedPitch)
                {
                    pitchSpeed = ownPitch + TURNGYROCONST;
                    fullDrivePitch = true;
                }
                else
                {
                    pitchSpeed = ownPitch - TURNGYROCONST;
                    fullDrivePitch = true;
                }
            }
            else//soft
            {
                pitchSpeed = wantedPitch - ownPitch;
                fullDrivePitch = false;
            }

        }

        if (roll > 0)
        {
            rollSpeed = ownRoll + TURNGYROCONST;
        }
        else if (roll < 0)
        {
            rollSpeed = ownRoll - TURNGYROCONST;
        }
        else
        {
            rollSpeed = 0;
        }
        //rollSpeed = roll;
        lastShipMatrix = _shipMatrix;
        lastYaw = ownYaw;
        lastPitch = ownPitch;
        pitchSpeed *= 60;
        yawSpeed *= 60;
        rollSpeed *= 60;
        if (horizont == 0 & vertical == 0 & roll == 0)
            foreach (var thisGyro in gyros)
            {
                thisGyro.GyroOverride = false;
            }
        else
        ApplyGyroOverride(pitchSpeed, yawSpeed, rollSpeed, gyros, _shipMatrix);
    }
    public void Drop(List<IMyGyro> gyroList)
    {
        _firstrun = true;

        foreach (var thisGyro in gyroList)
        {
            thisGyro.GyroOverride = false;
        }
    }
    void ApplyGyroOverride(double pitchSpeed, double yawSpeed, double rollSpeed, List<IMyGyro> gyroList, MatrixD worldMatrix)
    {
        var rotationVec = new Vector3D(pitchSpeed, yawSpeed, rollSpeed);
        var relativeRotationVec = Vector3D.TransformNormal(rotationVec, worldMatrix);
        foreach (var thisGyro in gyroList)
        {
            var transformedRotationVec = Vector3D.TransformNormal(
                relativeRotationVec,
                Matrix.Transpose(thisGyro.WorldMatrix)
            );
            thisGyro.Pitch = (float)transformedRotationVec.X;
            thisGyro.Yaw = (float)transformedRotationVec.Y;
            thisGyro.Roll = (float)transformedRotationVec.Z;
            thisGyro.GyroOverride = true;
        }
    }

}


static class SystemHelper
{
    public static bool AddBlockIfType<T>(IMyTerminalBlock block, out T orig) where T : class, IMyTerminalBlock
    {
        T typedBlock = block as T;
        orig = typedBlock;
        if (typedBlock == null)
            return false;
        return true;
    }
    public static bool AddToListIfType<T>(IMyTerminalBlock block, List<T> list) where T : class, IMyTerminalBlock
    {
        T typedBlock;
        return AddToListIfType(block, list, out typedBlock);
    }
    public static bool AddToListIfType<T>(IMyTerminalBlock block, List<T> list, out T typedBlock) where T : class, IMyTerminalBlock
    {
        typedBlock = block as T;
        if (typedBlock != null)
        {
            list.Add(typedBlock);
            return true;
        }
        return false;
    }