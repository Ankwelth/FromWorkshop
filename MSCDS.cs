// Manual skid control drive system
// v 2.6
// by: Xejijikot
// 
// INSTRUCTIONS:
//   1. Set the friction and suspension height on the wheels
//   2. Place all wheels cockpits that you want to use
//   in group named "MSCDS", then recompile the script.
//   3. Use "Setup" command to re-configure wheels parameters, script status will change to "stand by". Command setup again to return script back in action.
//   4. Configure the custom data.
// 
// HYDROPNEUMATIC:
// - Hold "C" and use A/D for side tilt, W/S for front tilt, Q/E for up-down movement
// - Tap "C" twice to set hydropneumatic to default position
// 
// NOTES:
//   - Increase wheel's steering angle to better performance
// 
const string _debugLCDTag = "Debug";
List<IMyShipController> _controllers = new List<IMyShipController>();
List<IMyGyro> _gyros = new List<IMyGyro>();
IMyShipController _lastController = null;
List<ExtendedWheel> _allWheels = new List<ExtendedWheel>();
IMyTextPanel _debugLCD;
IMyBlockGroup _mscdcGroup;
        
readonly MyIni _myIni = new MyIni();
string _groupName = "MSCDS", _updateInfo = "";
float _turnDefault = 1.5f, _turnMove = 2, _turnSpeed = 0.3f, _minProp = 0.4f, _maxProp = 1f, _speedScale = 5,
    _gyroMult = 8, _maxGyroSpeed = 10,
    _maxX = 0, _maxZ = 0, _maxHeight = 0,
    _hydroOffset = 0.5f, _tiltFront = 0, _tiltSide = 0, _tiltUp = 0, _tiltXMult = 0.01f, _tiltYMult = 0.01f, _tiltZMult = 0.01f;
bool _previosHydroTick = false, _setHydroToDefault = false, _setParkWhenEmpty = true, _lastTickPark = false,
    _active = true;
static bool _gyrosOff = false;
long _tick = 0, _lastHydroTick = 0;
        
            

const string INI_SECTION_DEFAULTS = "Defaults", INI_GROUP_NAME = "Group name", INI_PARK = "Park when exit",
    INI_SECTION_STEERING = "Steering radius", INI_TURN_DEFAULT = "Default", INI_TURN_MOVE = "Accelerating", INI_TURN_SPEED = "Speed",
    INI_SECTION_WHEELS = "Wheels", INI_FRICTION = "Friction", INI_HEIGHT = "Height", INI_ALWAYS_ROTATE = "Steer when not in range",
    INI_SECTION_PROPULSION = "Propulsion", INI_MIN_PROPULSION = "Minimum propulsion", INI_MAX_PROPULSION = "Maximum propulsion", INI_SPEED_SCALE = "Maximum speed for propulsion boost",
    INI_SECTION_GYROS = "Gyros", INI_GYROS_MULT = "Rotation speed", INI_GYROS_SPEED = "Maximum speed when gyros used", INI_GYROS_OFF = "Turn off when not used",
    INI_SECTION_HYDROPNEUMATIC = "Hydropneumatic", INI_HYDRO_OFFSET = "Offset", INI_HYDRO_XMULT = "Side multiplier", INI_HYDRO_ZMULT = "Front multiplier", INI_HYDRO_DEFAULT_PARK = "Set default when park";
const float LOWFLOAT = 0.000001f;
    
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    UpdateBlocks(ref _updateInfo, true);
    foreach (var w in _allWheels)
    {
        w.SetDefaults(true);
    }
}
public void Main(string argument, UpdateType updateSource)
{
    if (argument == "Setup")
    {
        _active = !_active;
        if (!_active)
        {
            _tiltFront = 0;
            _tiltSide = 0;
            _tiltUp = 0;
            _setHydroToDefault = true;
            foreach (var w in _allWheels)
            {
                w.SetDefaults(true);
            }
        }
        else
        {
            UpdateBlocks(ref _updateInfo);
        }
    }
    var controller = GetControlledShipController(_controllers, _lastController);
    if (controller != null)
    {
        var hydroChange = CheckControlHydro(_tick, controller);
        //Skid steering
        if (controller.HandBrake)
        {
            if (!_lastTickPark)
                foreach (var w in _allWheels)
                {
                    w.SetDefaults();
                }
            _lastTickPark = true;
        }
        else
        {
            if (_active)
                SkidSteer(controller, hydroChange);
            _lastTickPark = false;
        }

        //Hydro
        if (_active)
        {
            Hydropneumatic(controller, hydroChange);
        }
        else if (_setHydroToDefault)
        {
            _tiltFront = 0;
            _tiltSide = 0;
            Hydropneumatic(controller);
        }
        _lastController = controller;
    }
    _setHydroToDefault = false;
    string status = _active ? "active": "stand by";
    Echo("Manual Skid Control Drive System\nv 2.6\n"+
    $"\nStatus: {status}\n" +
    _updateInfo);
    _tick++;
}
    
void SkidSteer(IMyShipController controller, bool hydropneumatic)
{
    if (!controller.IsUnderControl)
    {
        DropGyro(_gyros);
        foreach (var w in _allWheels)
        {
            w.SetDefaults();
        }
        if (_setParkWhenEmpty)
        {
            controller.HandBrake = true;
        }
    }
    Vector3 moveVector = controller.MoveIndicator;
    Vector3D velocityWorld = controller.GetShipVelocities().LinearVelocity;
    float forwardSpeed = (float)Vector3D.Dot(controller.WorldMatrix.Forward, velocityWorld);
    bool isTurning = Math.Abs(moveVector.X) > 1e-3;

    float leftPropulsion = 0f;
    float rightPropulsion = 0f;

    float gyroYaw = 0;
    if (!isTurning) // No turning
    {
        leftPropulsion = -Math.Sign(moveVector.Z) * _maxProp;
        rightPropulsion = Math.Sign(moveVector.Z) * _maxProp;
    }
    else
    {
        //float speedScale = Math.Abs(moveVector.Z) > 1e-3 ? 0.1f : _maxProp;
        float speedScale = _minProp;
        if (_speedScale != 0)
        {
            float component = MathHelper.Clamp(Math.Abs(forwardSpeed), 0, _speedScale) / _speedScale;
            speedScale = component * _minProp + (1 - component) * _maxProp;
        }
        if (moveVector.Z < 0) // moving forward
        {
            if (moveVector.X > 0) // Turn right: left full forward, right fraction backwards
            {
                leftPropulsion = _maxProp;
                rightPropulsion = _maxProp;
            }
            else // Turn left: right full forward, left fraction backwards
            {
                leftPropulsion = -_maxProp;
                rightPropulsion = -_maxProp;
            }
        }
        else if (moveVector.Z > 0) // moving backwards
        {
            if (moveVector.X > 0) // Turn right: left full backwards, right fraction forwards
            {
                leftPropulsion = -_maxProp;
                rightPropulsion = -_maxProp;
            }
            else // Turn left: right full backwards, left fraction forwards
            {
                leftPropulsion = _maxProp;
                rightPropulsion = _maxProp;
            }
        }
        else // Spin in place
        {
            if (moveVector.X > 0) // Turn right: left full forward, right full backwards
            {
                leftPropulsion = speedScale;
                rightPropulsion = speedScale;
            }
            else // Turn left: right full forward, left full backwards
            {
                leftPropulsion = -speedScale;
                rightPropulsion = -speedScale;
            }
        }

        //gyroControl
        gyroYaw = moveVector.X > 0 ? _gyroMult : - _gyroMult;
    }
    // +left -> forward left
    // -right -> forward right
    // +gyro -> turn right
    Vector3D avgWheelPos = GetAverageWheelPosition(_allWheels, true);
    double rotationMult = _turnSpeed * Math.Abs(forwardSpeed) + _turnMove * Math.Abs(moveVector.Z) + _turnDefault * Math.Abs(moveVector.X);
    Vector3D rotationPoint = avgWheelPos + moveVector.X * controller.WorldMatrix.Right * rotationMult;
    if (hydropneumatic)
    {
        SetWheelPropulsionAndFriction(
        controller,
        rotationPoint,
        _allWheels,
        LOWFLOAT,
        LOWFLOAT,
        false);
    }
    else
    {
        SetWheelPropulsionAndFriction(
        controller,
        rotationPoint,
        _allWheels,
        leftPropulsion,
        rightPropulsion,
        isTurning);
    }
    gyroYaw = (float)(gyroYaw /60 * 2 * Math.PI);
    if (gyroYaw == 0 || forwardSpeed > _maxGyroSpeed)
    {
        DropGyro(_gyros);
    }
    else
    {
        ApplyGyroOverride(0, gyroYaw, 0, _gyros, controller.WorldMatrix);
    }
}
void Hydropneumatic(IMyShipController controller, bool Control = false)
{
    Vector3D centerPos = GetAverageWheelPosition(_allWheels, false);
    Vector3 moveVector = controller.MoveIndicator;
    var xMove = moveVector.X;
    var yMove = controller.RollIndicator;
    var zMove = moveVector.Z;
    if (Control)
    {
        float tiltSideDelta = _tiltXMult * xMove;
        if (Math.Abs(_tiltSide + tiltSideDelta) < _maxHeight)
            _tiltSide += tiltSideDelta;

        float tiltFrontDelta = _tiltZMult * zMove;
        if (Math.Abs(_tiltFront + tiltFrontDelta) < _maxHeight)
            _tiltFront += tiltFrontDelta;

        float tiltUpDelta = _tiltYMult * yMove;
        if (Math.Abs(_tiltUp + tiltUpDelta) < _maxHeight)
            _tiltUp += tiltUpDelta;
    }

    foreach (var ew in _allWheels)
    {
        var w = ew.w;
        var offset = ew.CalculateOffset(centerPos, controller.WorldMatrix);
        if (_tiltFront < 0)
            w.Height = (float)(ew.defaultHeight + _tiltSide * (offset.X / _maxX) + _tiltFront * ((offset.Z - _hydroOffset * _maxZ) / _maxZ) + _tiltUp);
        else
            w.Height = (float)(ew.defaultHeight + _tiltSide * (offset.X / _maxX) + _tiltFront * ((offset.Z + _hydroOffset * _maxZ) / _maxZ) + _tiltUp);
        if (Control)
            w.SteeringOverride = LOWFLOAT;
    }
}
void SetWheelPropulsionAndFriction(
    IMyShipController reference,
    Vector3D referencePos,
    List<ExtendedWheel> wheels,
    float leftPropulsion,
    float rightPropulsion,
    bool isTurning)
{
            
    foreach(var ew in wheels)
    {
        if (!ew.w.IsAttached)
            continue;
        var w = ew.w;
        Vector3D left = reference.WorldMatrix.Left;
        Vector3D right = reference.WorldMatrix.Right;
        float maxAngle = ew.w.MaxSteerAngle;

        // Determine what propulsion we should use
        Vector3D diff = w.Top.GetPosition() - referencePos;
        bool isLeft = Vector3D.Dot(diff, left) > 0;

        // Determine if the wheel is pointing outwards (+) or inwards (-) and compensate
        float sign = Math.Sign(Vector3D.Dot(w.WorldMatrix.Up, isLeft ? left : right));

        // Set propulsion
        float propulsion = isLeft ? leftPropulsion : rightPropulsion;
        propulsion *= sign;

        // Set steering
        float angle = (float)CalculateAngle2(Vector3D.Normalize(diff * sign), w.WorldMatrix.Up, reference.WorldMatrix.Up);

        // Set friction
        float realAngle = w.SteerAngle * -1;
        float friction = (float)(Math.Cos(realAngle - angle) * Math.Cos(realAngle - angle));
        if (!isTurning)
        {
            w.PropulsionOverride = propulsion;
            w.SteeringOverride = LOWFLOAT;
            w.Friction = ew.defaultFriction;
        }
        else
        {
            if (ew.alwaysRotate)
            {
                w.PropulsionOverride = propulsion;
                w.SteeringOverride = angle / maxAngle;
                w.Friction = ew.defaultFriction * friction;
            }
            else
            {
                w.PropulsionOverride = Math.Abs(angle) < maxAngle ? propulsion: LOWFLOAT;
                w.SteeringOverride = Math.Abs(angle) < maxAngle ? angle / maxAngle : LOWFLOAT;
                w.Friction = Math.Abs(angle) < maxAngle ? ew.defaultFriction * friction : 0;
            }
        }
    }
}
Vector3D GetAverageWheelPosition(List<ExtendedWheel> wheels, bool isSuspensions)
{
    Vector3D sum = Vector3D.Zero;
    int count = 0;
    foreach (var v in wheels)
    {
        if (v.w.IsAttached)
        {
            if (isSuspensions)
                sum += v.w.GetPosition();
            else
                sum += v.w.Top.GetPosition();
            count++;
        }
    }

    if (count == 0)
    {
        return Vector3D.Zero;
    }

    return sum / count;
}

public static IMyShipController GetControlledShipController(List<IMyShipController> controllers, IMyShipController lastController = null)
{
    IMyShipController currentlyControlled = null;
    foreach (IMyShipController ctrl in controllers)
    {
        if (ctrl.IsMainCockpit)
        {
            return ctrl;
        }

        // Grab the first seat that has a player sitting in it
        // and save it away in-case we don't have a main contoller
        if (currentlyControlled == null && ctrl != lastController && ctrl.IsUnderControl && ctrl.CanControlShip)
        {
            currentlyControlled = ctrl;
        }
    }

    // We did not find a main controller, so if the first controlled controller
    // from last cycle if it is still controlled
    if (lastController != null && lastController.IsUnderControl)
    {
        return lastController;
    }

    // Otherwise we return the first ship controller that we
    // found that was controlled.
    if (currentlyControlled != null)
    {
        return currentlyControlled;
    }
    if (lastController == null && controllers.Count != 0)
        lastController = controllers[0];
    // Nothing is under control, return the controller from last cycle.
    return lastController;
}
public bool CheckControlHydro(long tick, IMyShipController controller)
{
    if (controller.MoveIndicator.Y < -1e-3)
    {
        if (tick - _lastHydroTick < 30 && tick - _lastHydroTick > 5 && !_previosHydroTick)
        {
            _setHydroToDefault = true;
            _tiltFront = 0;
            _tiltSide = 0;
            _tiltUp = 0;
        }

        if (_setHydroToDefault)
            return false;
                
        if (!_previosHydroTick)
            _lastHydroTick = tick;
        _previosHydroTick = true;

        return true;
    }
    else
    {
        _setHydroToDefault = false;
        _previosHydroTick = false;
        return false;
    }
}
public static void DropGyro(List<IMyGyro> gyroList)
{
    foreach (var thisGyro in gyroList)
    {
        thisGyro.GyroOverride = false;
        if (_gyrosOff)
            thisGyro.Enabled = false;

    }
}
public static void ApplyGyroOverride(double pitchSpeed, double yawSpeed, double rollSpeed, List<IMyGyro> gyroList, MatrixD worldMatrix)
{
    var rotationVec = new Vector3D(pitchSpeed, yawSpeed, rollSpeed);
    var relativeRotationVec = Vector3D.TransformNormal(rotationVec, worldMatrix);
    foreach (var thisGyro in gyroList)
    {
        if (_gyrosOff)
            thisGyro.Enabled = true;
        if (!thisGyro.IsWorking)
        {
            thisGyro.GyroOverride = false;
            continue;
        }
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
class ExtendedWheel
{
    public IMyMotorSuspension w;
    public bool alwaysRotate {get; private set;}
    public float defaultFriction {get; private set;}
    public float defaultHeight {get; private set;}
    public ExtendedWheel(IMyMotorSuspension m, bool getConfs = false)
    {
        w = m;
        float f = w.Friction, h = w.Height;
        bool r = false;
        if (getConfs)
            LoadWheelConf(w, ref f, ref h, ref r);
        defaultFriction = f;
        defaultHeight = h;
        alwaysRotate = r;
    }
    public void SetDefaults(bool setH = false)
    {
        float f = defaultFriction, h = defaultHeight;
        bool r = alwaysRotate;
        LoadWheelConf(w, ref f, ref h, ref r);
        if (setH)
            w.Height = h;
        w.Friction = f;
        w.SteeringOverride = 0;
        alwaysRotate = r;
        SaveDefaults();
    }
    public void SaveDefaults()
    {
        defaultFriction = w.Friction;
        defaultHeight = w.Height;
        SaveWheelConf(w, defaultFriction, defaultHeight, alwaysRotate);
    }
    public Vector3D CalculateOffset(Vector3D centralPos, MatrixD orientation)
    {
        if (!w.IsAttached)
            return Vector3D.Zero;
        Vector3D diff = w.Top.GetPosition() - centralPos;
        Vector3D localCord = Vector3D.TransformNormal(diff, MatrixD.Transpose(orientation));
        return localCord;
    }
}
public void UpdateBlocks(ref string updateInfo, bool firstrun = false)
{
    updateInfo = "";
    _debugLCD = GridTerminalSystem.GetBlockWithName(_debugLCDTag) as IMyTextPanel;
    LoadIniConfig();
    _controllers.Clear();
    _allWheels.Clear();
    _gyros.Clear();
    _mscdcGroup = GridTerminalSystem.GetBlockGroupWithName(_groupName);
    if (_mscdcGroup == null)
    {
        updateInfo += $"\nGroup not Found!\n" +
            $"Group name: {_groupName}\n";
    }
    else
    {
        //controllers
        _mscdcGroup.GetBlocksOfType(_controllers);
        if (_controllers.Count() == 0)
        {    
            updateInfo += $"\nError!\n"
            + $"- Ship controllers: Not found!\n"
            + $"  Group name: {_groupName}\n";
            SC();
            return;
        }
        _lastController = GetControlledShipController(_controllers);

        //wheels
        List<IMyMotorSuspension> allWheels = new List<IMyMotorSuspension>();
        _mscdcGroup.GetBlocksOfType(allWheels);
        foreach (var w in allWheels)
        {
            ExtendedWheel ew = new ExtendedWheel(w, firstrun);
            _allWheels.Add(ew);
            if (firstrun)
                ew.SetDefaults();
            else
            {
                ew.SaveDefaults();
            }
        }
        //Calculate max X and Z offset for hydropneumatic
        Vector3D avgWheelPos = GetAverageWheelPosition(_allWheels, false);
        _maxX = 0; _maxZ = 0; _maxHeight = 0;
        foreach(var w in _allWheels)
        {
            var offset = w.CalculateOffset(avgWheelPos, _lastController.WorldMatrix);
            _maxX = (float)(_maxX < offset.X ? offset.X : _maxX);
            _maxZ = (float)(_maxZ < offset.Z ? offset.Z : _maxZ);
            _maxHeight = w.w.CubeGrid.GridSizeEnum == MyCubeSize.Large ? 1.5f : 0.5f;
        }

        //gyros
        _mscdcGroup.GetBlocksOfType(_gyros);

        updateInfo += $"\nLast update info:\n"
        + $"- Ship controllers: {_controllers.Count()}\n"
        + (allWheels.Count() == 0 ? "Warning! No wheels found": $"- Wheels: {allWheels.Count()}\n")
        + $"- Gyros: {_gyros.Count()}\n";
    }
    SC();
}
void LoadIniConfig()
{
    _myIni.Clear();
    bool parsed = _myIni.TryParse(Me.CustomData);
    if (!parsed)
    {
        SC();
        return;
    }
    _groupName = _myIni.Get(INI_SECTION_DEFAULTS, INI_GROUP_NAME).ToString(_groupName);
    _setParkWhenEmpty = _myIni.Get(INI_SECTION_DEFAULTS, INI_PARK).ToBoolean(_setParkWhenEmpty);
    _turnDefault = (float)_myIni.Get(INI_SECTION_STEERING, INI_TURN_DEFAULT).ToDouble(_turnDefault);
    _turnMove = (float)_myIni.Get(INI_SECTION_STEERING, INI_TURN_MOVE).ToDouble(_turnMove);
    _turnSpeed = (float)_myIni.Get(INI_SECTION_STEERING, INI_TURN_SPEED).ToDouble(_turnSpeed);
    _minProp = (float)_myIni.Get(INI_SECTION_PROPULSION, INI_MIN_PROPULSION).ToDouble(_minProp);
    _maxProp = (float)_myIni.Get(INI_SECTION_PROPULSION, INI_MAX_PROPULSION).ToDouble(_maxProp);
    _speedScale = (float)_myIni.Get(INI_SECTION_PROPULSION, INI_SPEED_SCALE).ToDouble(_speedScale);
    _gyroMult = (float)_myIni.Get(INI_SECTION_GYROS, INI_GYROS_MULT).ToDouble(_gyroMult);
    _maxGyroSpeed = (float)_myIni.Get(INI_SECTION_GYROS, INI_GYROS_SPEED).ToDouble(_maxGyroSpeed);
    _gyrosOff = _myIni.Get(INI_SECTION_GYROS, INI_GYROS_OFF).ToBoolean(_gyrosOff);
    _hydroOffset = (float)_myIni.Get(INI_SECTION_HYDROPNEUMATIC, INI_HYDRO_OFFSET).ToDouble(_hydroOffset);
    _tiltXMult = (float)_myIni.Get(INI_SECTION_HYDROPNEUMATIC, INI_HYDRO_XMULT).ToDouble(_tiltXMult);
    _tiltZMult = (float)_myIni.Get(INI_SECTION_HYDROPNEUMATIC, INI_HYDRO_ZMULT).ToDouble(_tiltZMult);
}
void SC()
{
    _myIni.Clear();
    _myIni.Set(INI_SECTION_DEFAULTS, INI_GROUP_NAME, _groupName);
    _myIni.Set(INI_SECTION_DEFAULTS, INI_PARK, _setParkWhenEmpty);
    _myIni.Set(INI_SECTION_STEERING, INI_TURN_DEFAULT, _turnDefault);
    _myIni.Set(INI_SECTION_STEERING, INI_TURN_MOVE, _turnMove);
    _myIni.Set(INI_SECTION_STEERING, INI_TURN_SPEED, _turnSpeed);
    _myIni.Set(INI_SECTION_PROPULSION, INI_MIN_PROPULSION, _minProp);
    _myIni.Set(INI_SECTION_PROPULSION, INI_MAX_PROPULSION, _maxProp);
    _myIni.Set(INI_SECTION_PROPULSION, INI_SPEED_SCALE, _speedScale);
    _myIni.Set(INI_SECTION_GYROS, INI_GYROS_MULT, _gyroMult);
    _myIni.Set(INI_SECTION_GYROS, INI_GYROS_SPEED, _maxGyroSpeed);
    _myIni.Set(INI_SECTION_GYROS, INI_GYROS_OFF, _gyrosOff);
    _myIni.Set(INI_SECTION_HYDROPNEUMATIC, INI_HYDRO_OFFSET, _hydroOffset);
    _myIni.Set(INI_SECTION_HYDROPNEUMATIC, INI_HYDRO_XMULT, _tiltXMult);
    _myIni.Set(INI_SECTION_HYDROPNEUMATIC, INI_HYDRO_ZMULT, _tiltZMult);
    Me.CustomData = _myIni.ToString();
}
static void LoadWheelConf(IMyTerminalBlock refBlock, ref float friction, ref float height, ref bool alwaysRotate)
{
    MyIni ini = new MyIni();
    bool parsed = ini.TryParse(refBlock.CustomData);
    if (!parsed)
        return;
    friction = (float)ini.Get(INI_SECTION_WHEELS, INI_FRICTION).ToDouble(friction);
    height = (float)ini.Get(INI_SECTION_WHEELS, INI_HEIGHT).ToDouble(height);
    alwaysRotate = ini.Get(INI_SECTION_WHEELS, INI_ALWAYS_ROTATE).ToBoolean(alwaysRotate);
}
static void SaveWheelConf(IMyTerminalBlock refBlock, float friction, float height, bool alwaysRotate)
{
    MyIni ini = new MyIni();
    ini.Set(INI_SECTION_WHEELS, INI_FRICTION, friction);
    ini.Set(INI_SECTION_WHEELS, INI_HEIGHT, height);
    ini.Set(INI_SECTION_WHEELS, INI_ALWAYS_ROTATE, alwaysRotate);
    refBlock.CustomData = ini.ToString();
}
public static double CalculateAngle2(Vector3D a, Vector3D b, Vector3D up)
{
    Vector3D left = Vector3D.Normalize(Vector3D.Cross(up, a));
    return Vector3D.Angle(a, b) * Math.Sign(b.Dot(left));
}
