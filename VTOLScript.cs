/*
 *   VTOL Script
 *   -----------
 *   
 *   Credits:
 *      This script uses modified components from existing open source projects. You 
 *      can find the source code of these projects along with license information below.
 *     
 *      Project: Flight Assist (https://github.com/Naosyth/FlightAssist)
 *      Copyright: Copyright (c) 2017 Brandon Worl
 *      Licence: MIT Licence (https://github.com/Naosyth/FlightAssist/blob/master/LICENSE)
 *
 *      Project: VTOL
 *      Copyright: Copyright (c) 2019 Sean Cambell
 *      
 *   Licence:
 *      MIT License
 *
 *      Copyright (c) 2019 Jonathon Owens
 *
 *      Permission is hereby granted, free of charge, to any person obtaining a copy
 *      of this software and associated documentation files (the "Software"), to deal
 *      in the Software without restriction, including without limitation the rights
 *      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *      copies of the Software, and to permit persons to whom the Software is
 *      furnished to do so, subject to the following conditions:
 *
 *      The above copyright notice and this permission notice shall be included in all
 *      copies or substantial portions of the Software.
 *
 *      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *      SOFTWARE.
 *     
 *   Installation:
 *      Create block group containing:
 *          A ship controller (cockpit, flight seat, remote controll).
 *          Gyroscopes.
 *          Thrusters (Make sure you have downward thrusters).
 *          Rotors for thrusters with left/right in the name and the 0 degree mark facing DOWN
 *      Load Script into Programmable block.
 *      (Optional) Edit Custom Data config.
 *  
 *  Usage:
 *      Once installed VTOL will align the craft with natural gravity. Using 
 *      the WASD keys will result in the craft pitching and rolling in the appropriate 
 *      direction as to result in forward or sideward motion, and will regulate thrust 
 *      accordingly to maintain altitude.
 *      
 *      Using the mouse, or up down left and right keys will orientate the craft 
 *      accordingly in addition to any pitch or roll already applied to move.
 *      
 *      Activation of the motion dampeners will result in the craft pitching and rolling 
 *      in the appropriate direction as to slow the craft.
 *      
 *      VTOL provides 4 modes:
 *          flight   - In flight mode, the craft will behave as described above.
 *                     Flight mode can be activated by running the script with 'flight' 
 *                     (without quotes) as the argument, and can be set as the default 
 *                     mode by setting 'start_mode' in Custom Data to 'flight'.
 *      
 *          landing  - In landing mode, the craft will behave as described above, 
 *                     with the exception that the craft will move slower to allow for more
 *                     precise movement and all movement will be dampened by default.
 *                     Landing mode can be activated by running the script with 'landing'
 *                    as the argument, can be toggled off and on by running the script 
 *                     with 'toggle_landing' as the argument and can be set as the default 
 *                     mode by setting 'start_mode' in Custom Data to 'landing'. Toggling 
 *                     back from landing mode will put the craft into flight mode.
 *                    
 *          manual   - In manual mode, the craft will not pitch and roll accordingly to
 *                     achive motion when the WASD keys are pressed, but still will regulate 
 *                     engine power to maintain altitude.
 *                     Manual mode can be activated by running the script with 'manual'
 *                     as the argument, can be toggled off and on by running the script 
 *                     with 'toggle_manual' as the argument and can be set as the default 
 *                     mode by setting 'start_mode' in Custom Data to 'manual'. Toggling 
 *                     back from landing mode will put the craft into flight mode.
 *                    
 *          shutdown - In shutdown mode, all thrusters and gyroscopes associated with
 *                     VTOL will power off.
 *                     Shutdown mode can be activated by running the script with 'shutdown'
 *                     as the argument, can be toggled off and on by running the script 
 *                     with 'toggle_shutdown' as the argument and can be set as the default 
 *                     mode by setting 'start_mode' in Custom Data to 'shutdown'. Toggling 
 *                     back from landing mode will put the craft into flight mode.
 *                     
 *      VTOL can be configured by editing the Custom Data of the programable block. The
 *      config options are as follows:
 *          block_group_name   - This allows for the setting of a custom group name for blocks 
 *                               associated with VTOL. By default it is 'VTOL'
 *                               (without quotes).
 *                             
 *          max_pitch          - This sets the maximum allowed pitch in degrees for both using the 
 *                               WASD keys, and the motion dampeners. By default it is 45.
 *                             
 *          max_roll           - This sets the maximum allowed roll in degrees for both using the 
 *                               WASD keys, and the motion dampeners. By default it is 45.
 *                             
 *          max_landing_pitch - This sets the maximum allowed pitch in degrees for both using the 
 *                              WASD keys, and the motion dampeners while in landing mode. By 
 *                              default it is 10.
 *                             
 *          max_landing_roll  - This sets the maximum allowed roll in degrees for both using the 
 *                              WASD keys, and the motion dampeners while in landing mode. By 
 *                              default it is 10.
 *                               
 *          start_mode        - This sets the mode that VTOL will be in when it is first
 *                              started, as described in VTOL mode description above. By 
 *                              default is 'flight'.
 *                              
 *          remember_mode     - This setting determines whether VTOL should attempt to remember
 *                              the mode it was last in when the script is restarted. By default it
 *                              is 'true'.
 */

const string defaultConfigString = "[main]\nblock_group_name=VTOL\nmax_pitch=80\nmax_roll=45\nmax_landing_pitch=10\nmax_landing_roll=10\nprecision=16\nstart_mode=flight\nremember_mode=true";

IMyShipController controller;

GyroController gyroController;
ThrusterController thrusterController;
RotorController rotorController;

TimeSpan timeSinceLastUpdate;
bool updateSuceeded = false;
bool isFirstRun = true;
string configString;

List<IMyShipController> controllers;
List<IMyGyro> gyros;
List<IMyThrust> thrusters;
List<IMyMotorStator> rotors;

bool precisionEnabled = false;
bool lateralDampeners = false;

//Config Options
string blockGroupName = "VTOL";

float maxFlightPitch = 80.0f;
float maxFlightRoll = 45.0f;

float maxLandingPitch = 10.0f;
float maxLandingRoll = 10.0f;

float precisionMultiplier = 16.0f;

string mode = "flight";
bool rememberMode = true;
bool dampening = true;


public Program()
{
    this.controllers = new List<IMyShipController>();
    this.gyros = new List<IMyGyro>();
    this.thrusters = new List<IMyThrust>();
    this.rotors = new List<IMyMotorStator>();

    timeSinceLastUpdate = TimeSpan.FromSeconds(0);
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}

string Update()
{
    updateSuceeded = false;
    try
    {
        string configString = Me.CustomData;
        if (configString.Length == 0) Me.CustomData = configString = defaultConfigString;

        if (configString != this.configString)
        {
            this.configString = configString;
            MyIni configIni = new MyIni();
            MyIniParseResult parseResult;

            if (!configIni.TryParse(configString, out parseResult)) throw new Exception("Failed To Read Config: " + parseResult.Error + " on line" + parseResult.LineNo.ToString());

            maxFlightPitch = (float)configIni.Get("main", "max_pitch").ToDouble(maxFlightPitch);
            maxFlightRoll = (float)configIni.Get("main", "max_roll").ToDouble(maxFlightRoll);

            maxLandingPitch = (float)configIni.Get("main", "max_landing_pitch").ToDouble(maxLandingPitch);
            maxLandingRoll = (float)configIni.Get("main", "max_landing_roll").ToDouble(maxLandingRoll);

            precisionMultiplier = (float)configIni.Get("main", "precision").ToDouble(precisionMultiplier);

            blockGroupName = configIni.Get("main", "block_group_name").ToString(blockGroupName);
            rememberMode = configIni.Get("main", "remember_mode").ToBoolean(rememberMode);

            if (isFirstRun)
            {
                mode = configIni.Get("main", "start_mode").ToString(mode);
                if (!isValidMode(mode))
                    throw new Exception("'" + mode + "is not a valid value for start_mode");
            }
        }

        if (isFirstRun && Storage.Length > 0 && rememberMode) mode = Storage;

        var blockGroup = GridTerminalSystem.GetBlockGroupWithName(blockGroupName);
        if (blockGroup == null) throw new Exception("Could not find block group with name '" + blockGroupName + "'");

        controllers.Clear();
        blockGroup.GetBlocksOfType<IMyShipController>(controllers);
        if (!controllers.Any()) throw new Exception("Ship must have atleast one ship controller");
        foreach (var controller in controllers)
        {
            if (controller.IsMainCockpit) this.controller = controller;
        }
        if (this.controller == null) this.controller = controllers.First();

        gyros.Clear();
        blockGroup.GetBlocksOfType<IMyGyro>(gyros);
        if (!gyros.Any()) throw new Exception("Ship must have atleast one gyroscope");
        gyroController = new GyroController(controller, gyros);

        thrusters.Clear();
        blockGroup.GetBlocksOfType<IMyThrust>(thrusters);
        if (!thrusters.Any()) throw new Exception("Ship must have atleast one thruster");
        thrusterController = new ThrusterController(controller, thrusters);

        rotors.Clear();
        blockGroup.GetBlocksOfType<IMyMotorStator>(rotors);
        if(!rotors.Any()) Echo("No rotors found.");
        rotorController = new RotorController(controller, rotors);

        if (isFirstRun) SwitchToMode(mode);

        updateSuceeded = true;
        isFirstRun = false;
    }
    catch (Exception e)
    {
        return "Error: " + e.Message;
    }
    return "";
}

public void Save()
{
    Storage = mode;
}

public void Main(string argument, UpdateType updateSource)
{
    timeSinceLastUpdate += Runtime.TimeSinceLastRun;

    if (isFirstRun || !updateSuceeded || timeSinceLastUpdate > TimeSpan.FromSeconds(10))
    {
        Echo(Update());
        timeSinceLastUpdate = TimeSpan.FromSeconds(0);
        return;
    }

    Echo("Script running, next update: " + (10 - (uint)timeSinceLastUpdate.TotalSeconds).ToString());
    Echo("Current Mode: " + mode);
    Echo("Precision Aim: " + (precisionEnabled ? "enabled": "disabled"));
    Echo("Lateral Dampening: " + (lateralDampeners ? "enabled" : "disabled"));
    Echo("Longitudinal Dampening: " + (dampening ? "enabled" : "disabled"));

    var wasd = controller.MoveIndicator;
    var mouse = new Vector3D(controller.RotationIndicator, controller.RollIndicator * 9);

    if (precisionEnabled) mouse *= 1 / precisionMultiplier;

    if (isValidMode(argument))
        SwitchToMode(argument);
    else if (argument == "toggle_manual") SwitchToMode(mode == "manual" ? "flight" : "manual");
    else if (argument == "toggle_landing") SwitchToMode(mode == "landing" ? "flight" : "landing");
    else if (argument == "toggle_shutdown") SwitchToMode(mode == "shutdown" ? "flight" : "shutdown");
    else if (argument == "toggle_standby") SwitchToMode(mode == "standby" ? "flight" : "standby");
    else if (argument == "toggle_precision") precisionEnabled = !precisionEnabled;
    else if (argument == "toggle_longitudinal_dampening") dampening = !dampening;
    else if (argument == "toggle_lateral_dampening") lateralDampeners = !lateralDampeners;
    else if (argument == "update")
    {
        updateSuceeded = false;
        return;
    }

    switch (mode)
    {
        case "flight":
            {
                var pitch = wasd.Z * maxFlightPitch * radToDeg;
                var roll = wasd.X * maxFlightRoll * radToDeg;

                var pitchRollToStop = gyroController.CalculatePitchRollToStop(maxFlightPitch * radToDeg, maxFlightRoll * radToDeg);

                if (isZero(pitch) && dampening) pitch = (float)pitchRollToStop.Y;
                if (isZero(roll) && lateralDampeners) roll = (float)pitchRollToStop.X;

                gyroController.SetPitchRoll(0f, roll, mouse);
				
				rotorController.SetPitch(pitch);
                
                ControlVerticalThrust(wasd);
                break;
            }
        case "landing":
            {
                var pitch = wasd.Z * maxLandingPitch * radToDeg;
                var roll = wasd.X * maxLandingRoll * radToDeg;

                var pitchRollToStop = gyroController.CalculatePitchRollToStop(maxLandingPitch * radToDeg, maxLandingRoll * radToDeg);

                if (isZero(pitch) && dampening) pitch = (float)pitchRollToStop.Y;
                if (isZero(roll) && lateralDampeners) roll = (float)pitchRollToStop.X;

                gyroController.SetPitchRoll(0f, roll, mouse);
                
                rotorController.SetPitch(pitch);
                
                ControlVerticalThrust(wasd);
                break;
            }
        case "manual":
            {
                ControlVerticalThrust(wasd);
                gyroController.SetVelocity(mouse);
                break;
            }
        case "shutdown":
            break;
        case "standby":
            break;
    }
}

void ControlVerticalThrust(Vector3D control) 
{
    if(control.Y == 1) 
    {
        thrusterController.SetYAxisThrust(float.MaxValue);
        Echo("Status: Ascending");
    } 
    else if(control.Y == -1)
    {
        thrusterController.SetYAxisThrust(1);
        Echo("Status: Descending");
    }
    else
    {
        thrusterController.SetYAxisThrust(thrusterController.CalculateThrustToHover());
        Echo("Status: Hovering");
    }
}

void SwitchToMode(string mode)
{
    switch (mode)
    {
        case "flight":
            gyroController.SetEnabled(true);
            thrusterController.SetEnabled(true);
            gyroController.SetGyroOverride(true);
            break;
        case "landing":
            gyroController.SetEnabled(true);
            thrusterController.SetEnabled(true);
            gyroController.SetGyroOverride(true);
            controller.DampenersOverride = true;
            break;
        case "manual":
            gyroController.SetEnabled(true);
            thrusterController.SetEnabled(true);
            gyroController.SetGyroOverride(true);
            break;
        case "shutdown":
            gyroController.SetEnabled(false);
            thrusterController.SetEnabled(false);
            break;
        case "standby":
            gyroController.SetEnabled(true);
            thrusterController.SetEnabled(true);
            gyroController.SetGyroOverride(false);
            thrusterController.SetYAxisThrust(0);
            break;
    }
    if (isValidMode(mode)) this.mode = mode;
    precisionEnabled = false;
    lateralDampeners = false;
}

bool isValidMode(string mode)
{
    return mode == "flight" || mode == "landing" || mode == "manual" || mode == "shutdown" || mode == "standby";
}

//The GyroController module is based on Flight Assist's GyroController and HoverModule, sharing code in places.
public class GyroController
{
    const double minGyroRpmScale = 0.001 / 9.87;
    const double gyroVelocityScale = 0.2 / 9.87;

    private readonly List<IMyGyro> gyros;
    private readonly IMyShipController controller;


    public GyroController(IMyShipController controller, List<IMyGyro> gyros)
    {
        this.gyros = gyros;
        this.controller = controller;

        foreach(var gyro in gyros)
        {
            gyro.Pitch = 0f;
            gyro.Roll = 0f;
            gyro.Yaw = 0f;
            gyro.GyroPower = 1f;
        }
    }

    public void SetEnabled(bool enabled)
    {
        foreach (var gyro in gyros)
        {
            gyro.Enabled = enabled;
        }
    }

    public void SetGyroOverride(bool gyroOverride)
    {
        foreach (var gyro in gyros)
        {
            gyro.GyroOverride = gyroOverride;
        }
    }

    public Vector2 CalculatePitchRollToStop(float maxPitch, float maxRoll)
    {
        Vector2 localVelocity;

        Vector3 worldVelocity = Vector3.Normalize(controller.GetShipVelocities().LinearVelocity);
        Vector3 gravity = -Vector3D.Normalize(controller.GetNaturalGravity());

        localVelocity.Y = Vector3.Dot(worldVelocity, Vector3.Cross(gravity, controller.WorldMatrix.Right)) * (float)controller.GetShipSpeed();
        localVelocity.X = Vector3.Dot(worldVelocity, Vector3.Cross(gravity, controller.WorldMatrix.Forward)) * (float)controller.GetShipSpeed();

        localVelocity.X = double.IsNaN(localVelocity.X) ? 0 : localVelocity.X;
        localVelocity.Y = double.IsNaN(localVelocity.Y) ? 0 : localVelocity.Y;

        localVelocity *= (float)gyroVelocityScale * 4;

        localVelocity.X = Math.Abs(localVelocity.X) < maxRoll ? localVelocity.X : localVelocity.X > 0 ? maxRoll : -maxRoll;
        localVelocity.Y = Math.Abs(localVelocity.Y) < maxPitch ? localVelocity.Y : localVelocity.Y > 0 ? maxPitch : -maxPitch;

        return localVelocity;
    }

    public void SetPitchRoll(float pitch, float roll, Vector3 velocity)
    {
        Matrix matrix; controller.Orientation.GetMatrix(out matrix);
        Vector3 reference = Vector3D.Transform(matrix.Down, Quaternion.CreateFromAxisAngle(matrix.Left, pitch) * Quaternion.CreateFromAxisAngle(matrix.Backward, roll));
        Vector3 target = controller.GetNaturalGravity();

        foreach (var gyro in gyros)
        {
            gyro.Orientation.GetMatrix(out matrix);
            matrix = Matrix.Transpose(matrix);

            var localReference = Vector3D.Transform(reference, (MatrixD)matrix);
            var localVelocity = Vector3D.Transform(velocity, (MatrixD)matrix);
            var localTarget = Vector3D.Transform(target, MatrixD.Transpose(gyro.WorldMatrix.GetOrientation()));

            var axis = Vector3D.Cross(localReference, localTarget);
            var angle = axis.Length();

            angle = Math.Atan2(angle, Math.Sqrt(Math.Max(0.0, 1.0 - angle * angle)));
            if (Vector3D.Dot(localReference, localTarget) < 0) angle = Math.PI;

            axis.Normalize();
            axis *= Math.Max(minGyroRpmScale, gyro.GetMaximum<float>("Roll") * (angle / Math.PI) * gyroVelocityScale);

            gyro.Pitch = (float)(-axis.X + localVelocity.X);
            gyro.Roll = (float)(-axis.Z + localVelocity.Z);
            gyro.Yaw = (float)(-axis.Y + localVelocity.Y);
        }
    }

    public void SetVelocity(Vector3D velocity)
    {
        Matrix matrix;

        foreach (var gyro in gyros)
        {
            gyro.Orientation.GetMatrix(out matrix);
            matrix = Matrix.Transpose(matrix);
            var localVelocity = Vector3D.Transform(velocity, (MatrixD)matrix);

            gyro.Pitch = (float)localVelocity.X;
            gyro.Roll = (float)localVelocity.Z;
            gyro.Yaw = (float)localVelocity.Y;
        }
    }
}

public class RotorController
{
    private IMyShipController controller;
    private List<IMyMotorStator> rotors;
    
    private List<IMyMotorStator> leftRotors;
    private List<IMyMotorStator> rightRotors;

    public RotorController(IMyShipController controller, List<IMyMotorStator> rotors)
    {
        this.controller = controller;
        this.rotors = rotors;
        
        leftRotors = new List<IMyMotorStator>();
        rightRotors = new List<IMyMotorStator>();
        
        foreach(var rotor in rotors)
        {
            if(rotor.CustomName.ToLower().Contains("right")) {
                rightRotors.Add(rotor);
            } 
            else if(rotor.CustomName.ToLower().Contains("left"))
            {
                leftRotors.Add(rotor);
            }
        }
    }

    public void SetPitch(float pitch)
    {   
        foreach(var rotor in leftRotors) {
            SetRotorVelocity(rotor, pitch);
        }
        foreach(var rotor in rightRotors) {
            SetRotorVelocity(rotor, -pitch);
        }
    }
    
    public void SetRotorVelocity(IMyMotorStator rotor, float pitch) {
        float angle = rotor.Angle;
        float dA = pitch - angle;
        dA = (float) Math.Atan2((double) Math.Sin((double) dA), Math.Cos((double) dA));
        float vel = dA;
        rotor.TargetVelocityRPM = vel * 100;
    }
}

public class ThrusterController
{
    private IMyShipController controller;
    private List<IMyThrust> allThrusters;


    public ThrusterController(IMyShipController controller, List<IMyThrust> thrusters)
    {
        this.controller = controller;
        this.allThrusters = thrusters;

        foreach (var thruster in thrusters)
        {
            thruster.ThrustOverride = 1;
        }
    }

    public void SetEnabled(bool enabled)
    {
        foreach (var thruster in allThrusters)
        {
            thruster.Enabled = enabled;
        }
    }

    public float SetYAxisThrust(float thrust)
    {
        return setAxisThrust(thrust, ref this.allThrusters);
    }

    public float CalculateMaxEffectiveThrust()
    {
        return calculateMaxAxisThrust(ref this.allThrusters);
    }

    public float CalculateThrustToHover()
    {
        var gravityDir = controller.GetNaturalGravity();
        var weight = controller.CalculateShipMass().TotalMass * gravityDir.Length();
        var velocity = controller.GetShipVelocities().LinearVelocity;

        gravityDir.Normalize();
        var gravityMatrix = Matrix.Invert(Matrix.CreateFromDir(gravityDir));
        velocity = Vector3D.Transform(velocity, gravityMatrix);

        var thrust = weight + weight * -velocity.Z;
        if(thrust < 1) thrust = 1;

        if (Vector3.Transform(controller.WorldMatrix.GetOrientation().Down, gravityMatrix).Z < 0)
            return (float)(thrust);
        else
            return -(float)(thrust);
    }

    private float calculateMaxAxisThrust(ref List<IMyThrust> thrusters)
    {
        float thrust = 0;
        foreach(var thruster in thrusters)
        {
            thrust += thruster.MaxEffectiveThrust;
        }
        return thrust;
    }

    private float calculateEffectiveThustRatio(IMyThrust thruster)
    {
        return thruster.MaxThrust / thruster.MaxEffectiveThrust;
    }

    private float setAxisThrust(float thrust, ref List<IMyThrust> thrustersPos)
    {
        if(thrust == 0) thrust = 1;
        
        List<IMyThrust> thrusters;

        if (thrust >= 0)
        {
            thrusters = thrustersPos;
        }
		else
		{
			return 0.0f;
		}

        thrust = Math.Abs(thrust);

        foreach (var thruster in thrusters)
        {
            //TODO: replace with smart thruster thrust allocation code.
            var localThrust = (thrust / thrusters.Count) * calculateEffectiveThustRatio(thruster);
            thruster.ThrustOverride = (float.IsNaN(localThrust) || float.IsInfinity(localThrust)) ? 0 : localThrust;
        }
        return 0.0f;
    }
}

const float radToDeg = (float)Math.PI / 180;

public static bool isZero(double value, double epsilon = 0.0001)
{
    return Math.Abs(value) <= epsilon;
}