
// Rover Script Version 1.0
// ============= Settings ==============
// The id that the ship should listen to.
// All commands not prefixed by this will be ignored.
// Any character is allowed except ; [ ] and :
const string followerSystemId = "System1";
const string followerId = "Drone1";

// The position that the ship will take relative to the main ship by default
// In (X, Y, Z)
// X: +Right -Left
// Y: +Up -Down
// Z: +Backward -Forward
// Important: Use the command 'reset' to make the follower use this value after it has changed.
readonly Vector3D defaultOffset = new Vector3D(50, 0, 0);

// The name of the cockpit in the ship. You may leave this blank, but it is highly recommended
// to set this field to avoid unexpected behavior related to orientation.
// If this cockpit is not found, the script will attempt to find a suitable cockpit on its own.
const string cockpitName = "";

// This allows you to automatically disable the script when the cockpit is in use.
readonly bool autoStop = true;

// When this is true, opon leaving the cockpit, the script will set the offset to the current position instead
// of returning to its designated point. Similar to the starthere command.
// This only applies if autoStop is true.
readonly bool autoStartHere = false;

// This is the frequency that the script is running at. If you are experiencing lag
// because of this script try decreasing this value. Valid values:
// Update1 : Runs the script every tick
// Update10 : Runs the script every 10th tick
// Update100 : Runs the script every 100th tick
// Note: Changing this value may cause unexpected behavior and will make following less accurate.
readonly UpdateFrequency tickSpeed = UpdateFrequency.Update1;

// When the tick speed of the leader is lower than the tick speed of the follower, this workaround can can be activated.
// When this is enabled, the script will "guess" what the leader position should be in missing ticks. If the leader gets
// damaged and stops, the follower will keep going as if the leader is still there until you use the stop command.
readonly bool calculateMissingTicks = true;

// When calculateMissingTicks is enabled, the maximum number of ticks to estimate before
// assuming the leader is no longer active.
// 1 game tick = 1/60 seconds
const int maxMissingScriptTicks = 100;
// =====================================

// =========== Configurations ==========
// You can save multiple offsets for your ship using the save, savehere, and load commands. The offsets for saved configurations
// can be directly edited in the CustomData field of the programmable block. By default, the script will have a single 'default'
// configuration with the default offset saved. When you make changes to the CustomData directly, you should recompile the script to
// make the changes appear. CustomData will only be updated by the script when using the save and savehere commands.
// Warning: Any error in the CustomData will cause the entire script to reset.
// Syntax:
// <name1> <x1> <y1> <z1>
// <name2> <x2> <y2> <z2>
// =====================================

// ============= Commands ==============
// setoffset;x;y;z : sets the offset variable to this value
// addoffset;x;y;z : adds these values to the current offset
// stop : stops the script and releases control to you
// start : starts the script after a stop command
// starthere : starts the script in the current position
// reset : resets and loads the default configuration with the default offset
// clear : forces the script to forget a previous leader when calculateMissingTicks is true
// save(;name) : saves the configuration to the current offset
// savehere(;name) : saves the configuration to the current position
// load;name : loads the offset from the configuration
// =====================================

// You can ignore any unreachable code warnings that appear in this script.

const float P = 0.03f;
const float I = 0;
const float D = 0.08f;
const float P2 = 0.03f;
const float I2 = 0;
const float D2 = 0.05f;

Dictionary<string, Vector3D> configurations = new Dictionary<string, Vector3D>();
string currentConfig = "default";

IMyShipController rc;
MatrixD leaderMatrix = MatrixD.Zero;
Vector3D leaderVelocity;
bool isDisabled = false;
Random r = new Random();
Vector3D offset;
WheelControl wheels;

bool prevControl = false;

IMyBroadcastListener leaderListener;
IMyBroadcastListener commandListener;
const string transmitTag = "FSLeader" + followerSystemId;
const string transmitCommandTag = "FSCommand" + followerSystemId;

readonly int echoFrequency = 100;
int runtime = 0;
int updated = 0;
const bool debug = false;

public Program ()
{
    // Prioritize the given cockpit name
    rc = GetBlock<IMyShipController>(cockpitName, true);
    if (rc == null) // Second priority cockpit
        rc = GetBlock<IMyCockpit>();
    if (rc == null) // Third priority remote control
        rc = GetBlock<IMyRemoteControl>();
    if (rc == null) // No cockpits found.
        throw new Exception("No cockpit/remote control found. Set the cockpitName field in settings.");

    wheels = new WheelControl(rc, tickSpeed, GetBlocks<IMyMotorSuspension>());

    leaderListener = IGC.RegisterBroadcastListener(transmitTag);
    leaderListener.SetMessageCallback("");
    commandListener = IGC.RegisterBroadcastListener(transmitCommandTag);
    commandListener.SetMessageCallback("");


    configurations["default"] = defaultOffset;
    offset = defaultOffset;
    LoadStorage();

    if (tickSpeed == UpdateFrequency.Update10)
        echoFrequency = 10;
    else if (tickSpeed == UpdateFrequency.Update100)
        echoFrequency = 1;
    Echo("Ready.");
}

void ResetMovement ()
{
    wheels.Reset();
}
public void Save ()
{
    // isDisabled;currentConfig;x;y;z
    StringBuilder sb = new StringBuilder();
    if (isDisabled)
        sb.Append("1;");
    else
        sb.Append("0;");
    sb.Append(currentConfig);
    sb.Append(';');
    sb.Append(offset.X);
    sb.Append(';');
    sb.Append(offset.Y);
    sb.Append(';');
    sb.Append(offset.Z);
    Storage = sb.ToString();
}

void SaveStorage ()
{
    // Save values stored in Storage
    Save();

    // Save values stored in CustomData
    /* name1 x y z
             * name2 x y z
             */
    StringBuilder sb = new StringBuilder();
    foreach (KeyValuePair<string, Vector3D> kv in configurations)
    {
        sb.Append(kv.Key);
        sb.Append(' ');
        sb.Append(kv.Value.X);
        sb.Append(' ');
        sb.Append(kv.Value.Y);
        sb.Append(' ');
        sb.Append(kv.Value.Z);
        sb.Append('\n');
    }
    Me.CustomData = sb.ToString();
}

void LoadStorage ()
{
    if (string.IsNullOrWhiteSpace(Storage) || string.IsNullOrWhiteSpace(Me.CustomData))
    {
        SaveStorage();
        Runtime.UpdateFrequency = tickSpeed;
    }

    try
    {
        // Parse CustomData values
        Dictionary<string, Vector3D> loadedConfig = new Dictionary<string, Vector3D>
        {
            ["default"] = defaultOffset // Ensure that default offset always exists
        };
        string [] config = Me.CustomData.Split('\n');
        foreach (string s in config)
        {
            if (string.IsNullOrWhiteSpace(s))
                continue; // Ignore blank lines

            string [] configValues = s.Split(' ');
            Vector3D value = new Vector3D(
                double.Parse(configValues [1]),
                double.Parse(configValues [2]),
                double.Parse(configValues [3])
                );
            loadedConfig [configValues [0]] = value;
        }

        // Parse Storage values
        string [] args = Storage.Split(';');
        bool loadedIsDisabled = args [0] == "1";
        string loadedCurrentConfig = args [1];
        Vector3D loadedOffset = new Vector3D(
            double.Parse(args [2]),
            double.Parse(args [3]),
            double.Parse(args [4])
            );

        // Parse successful, update the real values.
        configurations = loadedConfig;
        currentConfig = loadedCurrentConfig;
        if (configurations.ContainsKey(currentConfig))
            currentConfig = "default"; // If something went wrong, use the only guaranteed configuration.
        offset = loadedOffset;
        isDisabled = loadedIsDisabled;
        if (!isDisabled)
            Runtime.UpdateFrequency = tickSpeed;
    } catch (Exception)
    {
        SaveStorage();
        Runtime.UpdateFrequency = tickSpeed;
        //throw;
    }
}

public void Main (string argument, UpdateType updateSource)
{
    if (updateSource == UpdateType.Update100 || updateSource == UpdateType.Update10 || updateSource == UpdateType.Update1)
    {
        if (debug || runtime % echoFrequency == 0)
            WriteEcho();

        if (rc.GetNaturalGravity() == Vector3.Zero)
        {
            ResetMovement();
            throw new Exception("Panic! No planet detected!");
        }

        // Check to make sure that a message from the leader has been received
        if (leaderMatrix == MatrixD.Zero)
        {
            runtime++;
            return;
        }

        if (autoStop)
        {
            bool control = rc.IsUnderControl;
            if (control != prevControl)
            {
                if (control)
                    ResetMovement();
                else if (autoStartHere)
                    offset = CurrentOffset();

                prevControl = control;
            }

            if (prevControl)
            {
                runtime++;
                return;
            }
        }

        Move();
        runtime++;
    }
    else if (updateSource == UpdateType.IGC)
    {
        if (leaderListener.HasPendingMessage)
        {
            var data = leaderListener.AcceptMessage().Data;
            if (data is MyTuple<MatrixD, Vector3D, long>)
            {
                // Format: leader data, leader velocity, source grid id
                MyTuple<MatrixD, Vector3D, long> msg = (MyTuple<MatrixD, Vector3D, long>)data;
                if (msg.Item3 != Me.CubeGrid.EntityId)
                {
                    leaderMatrix = msg.Item1;
                    leaderVelocity = msg.Item2;
                    updated = runtime;
                }
                else
                {
                    leaderMatrix = MatrixD.Zero;
                }
            }
            else if (data is MyTuple<MatrixD, Vector3D>)
            {
                // Format: leader data, leader velocity
                MyTuple<MatrixD, Vector3D> msg = (MyTuple<MatrixD, Vector3D>)data;
                leaderMatrix = msg.Item1;
                leaderVelocity = msg.Item2;
                updated = runtime;
            }
        }

        if (commandListener.HasPendingMessage)
        {
            var data = commandListener.AcceptMessage().Data;
            if (data is MyTuple<string, string>)
            {
                MyTuple<string, string> msg = (MyTuple<string, string>)data;

                if (msg.Item1.Length > 0)
                {
                    foreach (string s in msg.Item1.Split(';'))
                    {
                        if (s == followerId)
                        {
                            RemoteCommand(msg.Item2);
                            return;
                        }
                    }
                    return;
                }
                else
                {
                    RemoteCommand(msg.Item2);
                    return;
                }
            }
        }
    }
    else
    {
        RemoteCommand(argument);
    }
}
Vector3D CurrentOffset ()
{
    return Vector3D.TransformNormal(rc.GetPosition() - leaderMatrix.Translation, MatrixD.Transpose(rc.WorldMatrix));
}

void WriteEcho ()
{
    Echo("Running.\nConfigs:");
    foreach (string s in configurations.Keys)
    {
        if (s == currentConfig)
            Echo(s + '*');
        else
            Echo(s);
    }
    Echo(offset.ToString("0.00"));
    if (leaderMatrix == MatrixD.Zero)
        Echo("No messages received.");
    else if (calculateMissingTicks && runtime - updated > maxMissingScriptTicks)
        Echo($"Weak signal, message received {runtime - updated} ticks ago.");
    if (autoStop && prevControl)
        Echo("Cockpit is under control.");
}

void Move ()
{
    // Apply translations to find the world position that this follower is supposed to be

    Vector3D targetPosition = Vector3D.Transform(offset, leaderMatrix);

    if (calculateMissingTicks)
    {
        int diff = Math.Min(Math.Abs(runtime - updated), maxMissingScriptTicks);
        if (diff > 0)
        {
            double secPerTick = 1.0 / 60;
            if (tickSpeed == UpdateFrequency.Update10)
                secPerTick = 1.0 / 6;
            else if (tickSpeed == UpdateFrequency.Update100)
                secPerTick = 5.0 / 3;
            double secPassed = diff * secPerTick;
            targetPosition += leaderVelocity * secPassed;
        }
    }

    wheels.Update(targetPosition);
}

void RemoteCommand (string command)
{
    string [] args = command.Split(';');

    switch (args [0])
    {
        case "setoffset": // setoffset;x;y;z
            if (args.Length == 4)
            {
                if (args [1] == "")
                    args [1] = this.offset.X.ToString();
                if (args [2] == "")
                    args [2] = this.offset.Y.ToString();
                if (args [3] == "")
                    args [3] = this.offset.Z.ToString();

                Vector3D offset;
                if (!StringToVector(args [1], args [2], args [3], out offset))
                    return;
                this.offset = offset;
                WriteEcho();
            }
            else
            {
                return;
            }
            break;
        case "addoffset": // addoffset;x;y;z
            if (args.Length == 4)
            {
                Vector3D offset;
                if (!StringToVector(args [1], args [2], args [3], out offset))
                    return;
                this.offset += offset;
                WriteEcho();
            }
            else
            {
                return;
            }
            break;
        case "stop": // stop
            Runtime.UpdateFrequency = UpdateFrequency.None;
            ResetMovement();
            isDisabled = true;
            Echo("Stopped.");
            break;
        case "start": // start
            Runtime.UpdateFrequency = tickSpeed;
            isDisabled = false;
            WriteEcho();
            break;
        case "starthere": // starthere
            Runtime.UpdateFrequency = tickSpeed;
            offset = CurrentOffset();
            isDisabled = false;
            WriteEcho();
            break;
        case "reset": // reset
            offset = defaultOffset;
            configurations ["default"] = defaultOffset;
            currentConfig = "default";
            isDisabled = false;
            SaveStorage();
            WriteEcho();
            break;
        case "save": // save(;name)
            {
                string key = currentConfig;
                if (args.Length > 1)
                    key = args [1];

                if (key.Contains(' '))
                    return;

                configurations [key] = offset;
                SaveStorage();
            }
            break;
        case "savehere": // save(;name)
            {
                string key = currentConfig;
                if (args.Length > 1)
                    key = args [1];

                if (key.Contains(' '))
                    return;

                Vector3D newOffset = CurrentOffset();
                configurations [key] = newOffset;
                SaveStorage();
            }
            break;
        case "load": // load;name
            if (args.Length == 1 || !configurations.ContainsKey(args [1]))
                return;
            // Load the new config
            offset = configurations [args [1]];
            currentConfig = args [1];
            isDisabled = false;
            WriteEcho();
            break;
        case "clear":
            leaderMatrix = MatrixD.Zero;
            break;
    }
}

bool StringToVector (string x, string y, string z, out Vector3D output)
{
    try
    {
        double x2 = double.Parse(x);
        double y2 = double.Parse(y);
        double z2 = double.Parse(z);
        output = new Vector3D(x2, y2, z2);
        return true;
    }
    catch (Exception)
    {
        output = new Vector3D();
        return false;
    }
}

T GetBlock<T> (string name, bool useSubgrids = false) where T : class, IMyTerminalBlock
{
    if (useSubgrids)
    {
        return GridTerminalSystem.GetBlockWithName(name) as T;
    }
    else
    {
        List<T> blocks = GetBlocks<T>(false);
        foreach (T block in blocks)
        {
            if (block.CustomName == name)
                return block;
        }
        return null;
    }
}
T GetBlock<T> (bool useSubgrids = false) where T : class, IMyTerminalBlock
{
    List<T> blocks = GetBlocks<T>(useSubgrids);
    return blocks.FirstOrDefault();
}
List<T> GetBlocks<T> (string groupName, bool useSubgrids = false) where T : class, IMyTerminalBlock
{
    IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName(groupName);
    if (group == null)
        return new List<T>();
    List<T> blocks = new List<T>();
    if (useSubgrids)
        group.GetBlocksOfType(blocks);
    else
        group.GetBlocksOfType(blocks, b => b.CubeGrid.EntityId == Me.CubeGrid.EntityId);
    return blocks;

}
List<T> GetBlocks<T> (bool useSubgrids = false) where T : class, IMyTerminalBlock
{
    List<T> blocks = new List<T>();
    if (useSubgrids)
        GridTerminalSystem.GetBlocksOfType(blocks);
    else
        GridTerminalSystem.GetBlocksOfType(blocks, b => b.CubeGrid.EntityId == Me.CubeGrid.EntityId);
    return blocks;
}

//Whip's PID controller class v6 - 11/22/17
public class PID
{
    double _kP = 0;
    double _kI = 0;
    double _kD = 0;
    double _integralDecayRatio = 0;
    double _lowerBound = 0;
    double _upperBound = 0;
    double _timeStep = 0;
    double _inverseTimeStep = 0;
    double _errorSum = 0;
    double _lastError = 0;
    bool _firstRun = true;
    bool _integralDecay = false;
    public double Value
    {
        get; private set;
    }

    public PID (double kP, double kI, double kD, double lowerBound, double upperBound, double timeStep)
    {
        _kP = kP;
        _kI = kI;
        _kD = kD;
        _lowerBound = lowerBound;
        _upperBound = upperBound;
        _timeStep = timeStep;
        _inverseTimeStep = 1 / _timeStep;
        _integralDecay = false;
    }

    public PID (double kP, double kI, double kD, double integralDecayRatio, double timeStep)
    {
        _kP = kP;
        _kI = kI;
        _kD = kD;
        _timeStep = timeStep;
        _inverseTimeStep = 1 / _timeStep;
        _integralDecayRatio = integralDecayRatio;
        _integralDecay = true;
    }

    public double Control (double error)
    {
        //Compute derivative term
        var errorDerivative = (error - _lastError) * _inverseTimeStep;

        if (_firstRun)
        {
            errorDerivative = 0;
            _firstRun = false;
        }

        //Compute integral term
        if (!_integralDecay)
        {
            _errorSum += error * _timeStep;

            //Clamp integral term
            if (_errorSum > _upperBound)
                _errorSum = _upperBound;
            else if (_errorSum < _lowerBound)
                _errorSum = _lowerBound;
        }
        else
        {
            _errorSum = _errorSum * (1.0 - _integralDecayRatio) + error * _timeStep;
        }

        //Store this error as last error
        _lastError = error;

        //Construct output
        this.Value = _kP * error + _kI * _errorSum + _kD * errorDerivative;
        return this.Value;
    }

    public double Control (double error, double timeStep)
    {
        _timeStep = timeStep;
        _inverseTimeStep = 1 / _timeStep;
        return Control(error);
    }

    public void Reset ()
    {
        _errorSum = 0;
        _lastError = 0;
        _firstRun = true;
    }
}

public class VectorPID
{
    private PID X;
    private PID Y;
    private PID Z;

    public VectorPID (double kP, double kI, double kD, double lowerBound, double upperBound, double timeStep)
    {
        X = new PID(kP, kI, kD, lowerBound, upperBound, timeStep);
        Y = new PID(kP, kI, kD, lowerBound, upperBound, timeStep);
        Z = new PID(kP, kI, kD, lowerBound, upperBound, timeStep);
    }

    public VectorPID (double kP, double kI, double kD, double integralDecayRatio, double timeStep)
    {
        X = new PID(kP, kI, kD, integralDecayRatio, timeStep);
        Y = new PID(kP, kI, kD, integralDecayRatio, timeStep);
        Z = new PID(kP, kI, kD, integralDecayRatio, timeStep);
    }

    public Vector3D Control (Vector3D error)
    {
        return new Vector3D(X.Control(error.X), Y.Control(error.Y), Z.Control(error.Z));
    }

    public void Reset ()
    {
        X.Reset();
        Y.Reset();
        Z.Reset();
    }
}

// MotorStator, LargeStator: 8000
// MotorSuspension, Suspension1x1: 20000
// MotorSuspension, Suspension3x3: 60000
// MotorSuspension, Suspension5x5: 100000
// MotorSuspension, Suspension1x1mirrored: 20000
// MotorSuspension, Suspension3x3mirrored: 60000
// MotorSuspension, Suspension5x5mirrored: 100000
// MotorSuspension, SmallSuspension1x1: 120
// MotorSuspension, SmallSuspension3x3: 1920
// MotorSuspension, SmallSuspension5x5: 4800
// MotorSuspension, SmallSuspension1x1mirrored: 120
// MotorSuspension, SmallSuspension3x3mirrored: 1920
// MotorSuspension, SmallSuspension5x5mirrored: 4800

public class WheelControl
{
    PID anglePID;
    PID forwardPID;
    IMyShipController rc;
    bool first = true;
    readonly List<Wheel> wheels = new List<Wheel>();

    struct Wheel
    {
        public IMyMotorSuspension block;
        public float maxForce;

        public Wheel (IMyMotorSuspension block, float maxForce)
        {
            this.block = block;
            this.maxForce = maxForce;
        }
    }

    public WheelControl (IMyShipController rc, UpdateFrequency tickSpeed, List<IMyMotorSuspension> wheels)
    {
        if (rc == null)
            throw new Exception("Ship controller null.");

        this.rc = rc;

        this.wheels = wheels.Select(x => new Wheel(x, GetForce(x))).ToList();

        double factor = 1;
        if (tickSpeed == UpdateFrequency.Update10)
            factor = 10;
        else if (tickSpeed == UpdateFrequency.Update100)
            factor = 100;
        double secondsPerTick = (1.0 / 60) * factor;

        anglePID = new PID(P2 / factor, I2 / factor, D2 / factor, 0.2 / factor, secondsPerTick);
        forwardPID = new PID(P / factor, I / factor, D / factor, 0.2 / factor, secondsPerTick);
        Reset();
    }

    public void Update (Vector3D target)
    {
        MatrixD transpose = MatrixD.Transpose(rc.WorldMatrix);

        Vector3D meToTarget = rc.WorldMatrix.Translation - target;
        Vector3D localError = Vector3D.TransformNormal(meToTarget, transpose);

        localError.Y = 0;
        if (localError.X > -0.5 && localError.X < 0.5)
            localError.X = 0;
        if (localError.Z > -0.5 && localError.Z < 0.5)
            localError.Z = 0;

        float correction = (float)forwardPID.Control(localError.Z);
        float force = correction * rc.CalculateShipMass().TotalMass;

        float rightLeft = (float)anglePID.Control(-localError.X);
        Vector3D localVelocity = Vector3D.TransformNormal(rc.GetShipVelocities().LinearVelocity, transpose);
        float angle = -rightLeft;
        if (localVelocity.Z < 0)
            angle *= -1;

        foreach (Wheel w in wheels)
        {
            IMyMotorSuspension wheel = w.block;
            if (first)
            {
                Vector3D center = Vector3D.TransformNormal(rc.CenterOfMass - rc.GetPosition(), transpose);
                Vector3D local = Vector3D.TransformNormal(wheel.GetPosition() - rc.GetPosition(), transpose);
                wheel.InvertSteer = (local.Z > center.Z);
                wheel.InvertPropulsion = (wheel.Orientation.Left != rc.Orientation.Forward);
                wheel.Brake = false;
            }

            if (wheel.Steering)
                wheel.SetValueFloat("Steer override", angle);

            if (wheel.Propulsion)
            {
                float maxForce = w.maxForce;
                if (maxForce <= 0)
                    continue;
                float percent = MathHelper.Clamp(force / maxForce, -1, 1);
                force -= percent * maxForce;
                wheel.SetValueFloat("Propulsion override", percent);
            }
        }
        first = false;
    }

    float GetForce (IMyMotorSuspension wheel)
    {
        switch (wheel.BlockDefinition.SubtypeId)
        {
            case "Suspension1x1mirrored":
            case "Suspension1x1":
            case "OffroadSuspension1x1":
            case "OffroadSuspension1x1mirrored":
                return 20000;
            case "Suspension3x3mirrored":
            case "Suspension3x3":
            case "OffroadSuspension3x3":
            case "OffroadSuspension3x3mirrored":
                return 60000;
            case "Suspension5x5mirrored":
            case "Suspension5x5":
            case "OffroadSuspension5x5":
            case "OffroadSuspension5x5mirrored":
                return 100000;
            case "SmallSuspension1x1mirrored":
            case "SmallSuspension1x1":
            case "OffroadSmallSuspension1x1":
            case "OffroadSmallSuspension1x1mirrored":
                return 120;
            case "SmallSuspension3x3mirrored":
            case "SmallSuspension3x3":
            case "OffroadSmallSuspension3x3":
            case "OffroadSmallSuspension3x3mirrored":
                return 1920;
            case "SmallSuspension5x5mirrored":
            case "SmallSuspension5x5":
            case "OffroadSmallSuspension5x5":
            case "OffroadSmallSuspension5x5mirrored":
                return 4800;
        }
        throw new Exception("Unknown wheel type: " + wheel.BlockDefinition.SubtypeId.ToString());
    }

    public void Reset ()
    {
        foreach (Wheel w in wheels)
        {
            IMyMotorSuspension wheel = w.block;
            wheel.SetValueFloat("Propulsion override", 0);
            wheel.SetValueFloat("Steer override", 0);
            wheel.InvertPropulsion = false;
            wheel.InvertSteer = false;
            wheel.Brake = true;
        }
        first = true;
    }
}