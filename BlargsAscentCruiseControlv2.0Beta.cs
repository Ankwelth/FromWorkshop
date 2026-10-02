// R e a d m e
// -----------
// 
// Blargmode's Ascent Cruise Control
// Version 2.0-beta.1 (2020-01-13)
// Version 2.0-beta.2 (2024-09-05)
// Version 2.0-beta.3 (2025-05-30)
// 
// Tired of wasting fuel when leaving a gravity well? What you need is cruise control!
// 
// This script adjusts the thrust of your rear thrusters to the lowest possible without losing speed.
// 
// ------------------------------------------------------------------------------------------
// NOTE FROM CAPTAIN STUBING : THIS IS NOT MY WORK.  
// 
// This is the amazing work of Blargmode, taken verbatim from:
// https://github.com/Blargmode/Ascent-cruise-control/releases/tag/v2.0-beta.1
// 
// ...as referred to by Blarg in the comments of the original version of this script:
// https://steamcommunity.com/sharedfiles/filedetails/?id=1316048908
// 
// As Blargmode seems to have disappeared and isn't responding to Steam comments or Discord
// messages from another commenter, I have taken it upon myself to publish this,
// since several commenters on the original were asking for updates.  
// 
// If Blargmode objects to this, I will take this down.
// ------------------------------------------------------------------------------------------
// 
// 
// ___/ Setup \\__________
// 
// 1. Install script.
// 2. Sit in a flight seat for a few seconds.
//     You can add the programmable block to the toolbar with the argument 'cruise'.
// 
// The script looks for an occupied flight on startup to determine what is forward. 
// 
// Once one is found, it stops looking and saves the seat internally. If the Wrong seat is stored, you can hop into
// the correct one, access the Programmable block, type 'reset' as the argument, and press run.
// 
// 
// 
// ___/ Usage \\__________
// 
// Press the button you set up in step two to turn the cruise control on or off.
// 
// 
// ___/ Usage (detailed) \\__________
// 
// These commands will move your ship, so don't trigger them accidentally.
// 
// 
// You trigger it by sending commands, either via a toolbar or directly in the programmable block. 
// Set up the former by adding the programmable block to a toolbar and selecting Run, then type in your trigger command.
// 
// 
// The trigger commands are 'cruise' and 'align'. To toggle cruise or align on or off. 
// if you add the argument 'on' or 'off' if you don't want the button to toggle. Example: 'cruise off'
// Cruise control also accepts a number, target speed, which can be supplied like this: 'cruise 95'. 
// 
// It can also be set in the settings. 
// You can send several commands at once as well, separate them with a comma: 'cruise on 95, align on'.
// 
// 
// ___/ Optional extras \\__________
// 
// The script can show status on LCDs. 
// 
// 
// Just add the tag #ACC to the name of the LCD.
// 
// If you want it on a specific cockpit screen, write #ACC@3 to put it on the third display.
// 
// You can have more than one LCD. 
// 
// Cruise Control can also be engaged via a button, a sensor, or any other action. 
// 
// 
// 
// ___/ Settings \\__________
// 
// Theres a list with commands and what they do in the sidebar of the programmable block.
// Each command is used with it's prefix and value. Target speed for example is ts. 
// 
// You can set it to 50 by typing 'ts 50'.
// 
// If type the prefix without a value you'll get an error message telling you what values it accepts.
// 
// You can type for example 'mx' (max altitude) and it will tell you:
// 
// 'mx' requires a number (set to -1 for unlimited) or 'this' for current altitude.
// 
// 
// ___/ Align \\__________
// 
// You can turn on align in order for the ship to align with gravity. This only works in the up direction. 
// 
// This is intended for use in space elevators. You can make a type of space elevator with this script.
// Read more below in the section "Controlled descent".
// 
// 
// ___/ Controlled descent \\__________
// 
// In version 1 of this script it was an unintended feature. Now it has been fleshed out. 
// 
// If you set the target speed to a negative value, it will fly in the reverse direction.
// Not only that, you can set a target altitude and it will slow down in order to stop there. 
// This works for both ascending and descending. 
// 
// 
// Together with Align this makes for a pretty nice space elevator. 
// Set the minimum altitude when at the bottom 'mn this', turn on align and fly straight up.
// When you reach the edge of gravity, stop WITHIN it and set the max altitude 'mx this'.
// 
// Now you can run 'cruise -99' for it to go down and 'cruise 99' to go up, and it will slow down and stop
// at each end. 
// You can even build stations there.
// I recommend using connectors to align it at each station. So that it can't drift sideways if you mess
// around up there. I also recommend not using a flight seat that can steer the ship. Use a button panel instead.
// You can leave cruise control and align turned on even when docked. 
// 
// A word of warning though. 
// 
// The controlled descent does not take ship weight and thrust into consideration.
// If your ship is too weak, it might leave a crater.
// 
Color BackgroundColor = new Color(0, 88, 151);
Color ForegroundColor = new Color(255, 255, 255);

class Aligner
{
    public bool Enabled
    {
        get; private set;
    }
    IMyShipController controller;
    List<IMyGyro> gyros;
    bool _gyroOverride = false;
    bool GyroOverride
    {
        get
        {
            return _gyroOverride;
        }
        set
        {
            _gyroOverride = value;
            for (int i = 0; i < gyros.Count; i++)
            {
                gyros[i].GyroOverride = value;
            }
        }
    }
    bool startedInNaturalGravity = false;
    public bool DisableOnNaturalGravityExit { get; private set; } = true;
    public Aligner(IMyShipController controller, List<IMyGyro> gyros)
    {
        this.controller = controller;
        this.gyros = gyros;
    }
    public void Update()
    {
        if (!Enabled)
            return;
        Vector3D down = controller.GetNaturalGravity();
        Vector3D shipDown = controller.WorldMatrix.Down;
        if (Vector3D.IsZero(down))
        {
            if (DisableOnNaturalGravityExit && startedInNaturalGravity)
            {
                Stop();
                return;
            }
            else
            {
                // We've exited gravity but aren't configured to automatically turn off.
                // Just assume down = shipDown so no further alignment adjustments should occur.
                //down = controller.GetShipVelocities().LinearVelocity;
                down = shipDown;
            }
        }
        Vector3D locationInGrid = Vector3D.Transform(controller.GetPosition() + Vector3.Normalize(down), MatrixD.Invert(controller.WorldMatrix)) / controller.CubeGrid.GridSize;
        Vector3D locationInGrid2 = Vector3D.Transform(controller.GetPosition() + Vector3.Normalize(shipDown), MatrixD.Invert(controller.WorldMatrix)) / controller.CubeGrid.GridSize;
        Vector3D striveForZero = locationInGrid - locationInGrid2;
        striveForZero *= 5;
        ApplyGyroOverride(-striveForZero.Z, controller.RotationIndicator.Y, -striveForZero.X, gyros, controller as IMyTerminalBlock);
    }
    void ApplyGyroOverride(double pitch_speed, double yaw_speed, double roll_speed, List<IMyGyro> gyro_list, IMyTerminalBlock reference)
    {
        var rotationVec = new Vector3D(-pitch_speed, yaw_speed, roll_speed);
        var shipMatrix = reference.WorldMatrix;
        var relativeRotationVec = Vector3D.TransformNormal(rotationVec, shipMatrix);
        foreach (var thisGyro in gyro_list)
        {
            var gyroMatrix = thisGyro.WorldMatrix;
            var transformedRotationVec = Vector3D.TransformNormal(relativeRotationVec, Matrix.Transpose(gyroMatrix));
            thisGyro.Pitch = (float)transformedRotationVec.X;
            thisGyro.Yaw = (float)transformedRotationVec.Y;
            thisGyro.Roll = (float)transformedRotationVec.Z;
        }
    }
    public void Start(bool disableOnNaturalGravityExit = true)
    {
        this.DisableOnNaturalGravityExit = disableOnNaturalGravityExit;
        startedInNaturalGravity = !Vector3D.IsZero(controller.GetNaturalGravity());
        Enabled = true;
        GyroOverride = true;
    }
    public void Stop()
    {
        Enabled = false;
        GyroOverride = false;
    }
}
class CruiseControl
{
    public bool Enabled
    {
        get; private set;
    }
    IMyShipController controller; Dictionary<Base6Directions.Direction, List<IMyThrust>> electricThrusters; Dictionary<Base6Directions.Direction, List<IMyThrust>> hydrogenThrusters;
    public Base6Directions.Direction Forward
    {
        get
        {
            return forward;
        }
        private set
        {
            forward = value;
            reverse = Base6Directions.GetOppositeDirection(value);
        }
    }
    Base6Directions.Direction forward = Base6Directions.Direction.Up;
    Base6Directions.Direction reverse = Base6Directions.Direction.Down; Base6Directions.Direction autoForward; public double TargetSpeed
    {
        get; private set;
    }
    public double AdaptiveTargetSpeed
    {
        get; private set;
    }
    public double Speed
    {
        get; private set;
    }
    bool descending = false; float cutoff = 0.05f; double deceleration = 5; public float ThrustOverrideElectric
    {
        get; private set;
    }
    public float ThrustOverrideHydrogen
    {
        get; private set;
    }
    public double TargetAltAcending { get; private set; } = double.MaxValue;
    public double TargetAltDescending
    {
        get; private set;
    }
    MyPlanetElevation elevationType;
    bool startedInNaturalGravity = false;
    public bool DisableOnNaturalGravityExit { get; private set; } = true;

    public CruiseControl(IMyShipController controller, List<IMyThrust> allThrusters)
    {
        this.controller = controller;
        electricThrusters = new Dictionary<Base6Directions.Direction, List<IMyThrust>>()
        {
            { Base6Directions.Direction.Forward,    new List<IMyThrust>() },
            { Base6Directions.Direction.Backward,   new List<IMyThrust>() },
            { Base6Directions.Direction.Up,         new List<IMyThrust>() },
            { Base6Directions.Direction.Down,       new List<IMyThrust>() },
        };
        hydrogenThrusters = new Dictionary<Base6Directions.Direction, List<IMyThrust>>()
        {
            { Base6Directions.Direction.Forward,    new List<IMyThrust>() },
            { Base6Directions.Direction.Backward,   new List<IMyThrust>() },
            { Base6Directions.Direction.Up,         new List<IMyThrust>() },
            { Base6Directions.Direction.Down,       new List<IMyThrust>() },
        };
        var forward = controller.Orientation.Forward;
        var back = Base6Directions.GetOppositeDirection(forward);
        var up = controller.Orientation.Up;
        var down = Base6Directions.GetOppositeDirection(up);
        var maxThrust = new Dictionary<Base6Directions.Direction, float>()
        {
            { Base6Directions.Direction.Forward,    0f },
            { Base6Directions.Direction.Backward,   0f },
            { Base6Directions.Direction.Up,         0f },
            { Base6Directions.Direction.Down,       0f }
        };
        foreach (var item in allThrusters)
        {
            if (item.Orientation.Forward == forward)
            {
                if (item.BlockDefinition.SubtypeId.Contains("Hydrogen"))
                    hydrogenThrusters[Base6Directions.Direction.Backward].Add(item);
                else
                    electricThrusters[Base6Directions.Direction.Backward].Add(item);
                maxThrust[Base6Directions.Direction.Backward] += item.MaxThrust;
            }
            else if (item.Orientation.Forward == back)
            {
                if (item.BlockDefinition.SubtypeId.Contains("Hydrogen"))
                    hydrogenThrusters[Base6Directions.Direction.Forward].Add(item);
                else
                    electricThrusters[Base6Directions.Direction.Forward].Add(item);
                maxThrust[Base6Directions.Direction.Forward] += item.MaxThrust;
            }
            if (item.Orientation.Forward == up)
            {
                if (item.BlockDefinition.SubtypeId.Contains("Hydrogen"))
                    hydrogenThrusters[Base6Directions.Direction.Down].Add(item);
                else
                    electricThrusters[Base6Directions.Direction.Down].Add(item);
                maxThrust[Base6Directions.Direction.Down] += item.MaxThrust;
            }
            else if (item.Orientation.Forward == down)
            {
                if (item.BlockDefinition.SubtypeId.Contains("Hydrogen"))
                    hydrogenThrusters[Base6Directions.Direction.Up].Add(item);
                else
                    electricThrusters[Base6Directions.Direction.Up].Add(item);
                maxThrust[Base6Directions.Direction.Up] += item.MaxThrust;
            }
        }
        Forward = maxThrust.Aggregate((left, right) => left.Value > right.Value ? left : right).Key;
        autoForward = Forward;
    }
    public void Update(double deltaTime)
    {
        if (!Enabled)
            return;
        double height;
        double newTarget;
        if (descending)
        {
            if (controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out height))
            {
                if (height > TargetAltDescending)
                {
                    newTarget = -((height - TargetAltDescending) / deceleration);
                }
                else
                {
                    newTarget = ((TargetAltDescending - height) / deceleration);
                }
                if (newTarget < TargetSpeed)
                    AdaptiveTargetSpeed = TargetSpeed;
                else
                    AdaptiveTargetSpeed = newTarget;
            }
        }
        else
        {
            if (controller.TryGetPlanetElevation(elevationType, out height))
            {
                if (height > TargetAltAcending)
                {
                    newTarget = -((height - TargetAltAcending) / deceleration);
                }
                else
                {
                    newTarget = ((TargetAltAcending - height) / deceleration);
                }
                if (newTarget > TargetSpeed)
                    AdaptiveTargetSpeed = TargetSpeed;
                else
                    AdaptiveTargetSpeed = newTarget;
            }
            else
            {
                if (DisableOnNaturalGravityExit && startedInNaturalGravity)
                {
                    Stop();
                    return;
                }
            }
        }
        double mass = controller.CalculateShipMass().PhysicalMass;
        double gravity = Vector3D.Dot(controller.GetNaturalGravity(), controller.WorldMatrix.GetOrientation().Down);
        double weight = mass * gravity;
        var vel = controller.GetShipVelocities().LinearVelocity;
        if (forward == Base6Directions.Direction.Up)
        {
            Speed = (Vector3D.TransformNormal(vel, MatrixD.Transpose(controller.WorldMatrix))).Y;
        }
        else if (forward == Base6Directions.Direction.Down)
        {
            Speed = -(Vector3D.TransformNormal(vel, MatrixD.Transpose(controller.WorldMatrix))).Y;
        }
        else if (forward == Base6Directions.Direction.Forward)
        {
            Speed = -(Vector3D.TransformNormal(vel, MatrixD.Transpose(controller.WorldMatrix))).Z;
        }
        else if (forward == Base6Directions.Direction.Backward)
        {
            Speed = (Vector3D.TransformNormal(vel, MatrixD.Transpose(controller.WorldMatrix))).Z;
        }
        double difference = AdaptiveTargetSpeed - Speed;
        double errorMagnitude = MathHelper.Clamp(difference / 5, -1, 1);
        float electricThrust = GetElectricThrust(forward);
        float electricThrustRev = GetElectricThrust(reverse);
        float hydroThrust = GetHydrogenThrust(forward);
        float hydroThrustRev = GetHydrogenThrust(reverse);
        float thrustOverride = MathHelper.Clamp((float)weight / electricThrust, 0, 1);
        float hydroThrustOverride = MathHelper.Clamp(((float)weight - electricThrust) / hydroThrust, 0, 1);
        float thrustOverrideRev = 0;
        float hydroThrustOverrideRev = 0;
        float thrustExcess = 0;
        float hydroThrustExcess = 0;
        thrustExcess = 1 - thrustOverride;
        hydroThrustExcess = 1 - hydroThrustOverride;
        if (Speed < AdaptiveTargetSpeed)
        {
            thrustOverride += thrustExcess * (float)errorMagnitude;
            hydroThrustOverride += hydroThrustExcess * (float)errorMagnitude;
        }
        else
        {
            thrustOverride *= 1 + (float)errorMagnitude;
            hydroThrustOverride *= 1 + (float)errorMagnitude;
        }
        thrustOverrideRev = -MathHelper.Clamp((electricThrustRev * (float)errorMagnitude) / electricThrustRev, -1, 0);
        hydroThrustOverrideRev = -MathHelper.Clamp((hydroThrustRev * (float)errorMagnitude) / hydroThrustRev, -1, 0);
        if (thrustOverride <= 0 || double.IsNaN(thrustOverride))
            thrustOverride = 0.000001f;
        if (hydroThrustOverride <= 0 || double.IsNaN(hydroThrustOverride))
            hydroThrustOverride = 0.000001f;
        if (thrustOverrideRev <= 0 || double.IsNaN(thrustOverrideRev))
            thrustOverrideRev = 0.000001f;
        if (hydroThrustOverrideRev <= 0 || double.IsNaN(hydroThrustOverrideRev))
            hydroThrustOverrideRev = 0.000001f;
        ThrustOverrideElectric = 0;
        for (int i = 0; i < electricThrusters[forward].Count; i++)
        {
            if (electricThrusters[forward][i].MaxEffectiveThrust / electricThrusters[forward][i].MaxThrust <= cutoff)
                electricThrusters[forward][i].ThrustOverridePercentage = 0.000001f;
            else
            {
                electricThrusters[forward][i].ThrustOverridePercentage = thrustOverride;
                ThrustOverrideElectric += thrustOverride;
            }
        }
        for (int i = 0; i < hydrogenThrusters[forward].Count; i++)
        {
            hydrogenThrusters[forward][i].ThrustOverridePercentage = hydroThrustOverride;
        }
        for (int i = 0; i < electricThrusters[reverse].Count; i++)
        {
            if (electricThrusters[reverse][i].MaxEffectiveThrust / electricThrusters[reverse][i].MaxThrust <= cutoff)
                electricThrusters[reverse][i].ThrustOverridePercentage = 0.000001f;
            else
            {
                electricThrusters[reverse][i].ThrustOverridePercentage = thrustOverrideRev;
                ThrustOverrideElectric += thrustOverrideRev;
            }
        }
        for (int i = 0; i < hydrogenThrusters[reverse].Count; i++)
        {
            hydrogenThrusters[reverse][i].ThrustOverridePercentage = hydroThrustOverrideRev;
        }
        ThrustOverrideElectric = ThrustOverrideElectric / (electricThrusters[forward].Count + electricThrusters[reverse].Count);
        ThrustOverrideHydrogen = hydroThrustOverride + hydroThrustOverrideRev;
    }
    float GetElectricThrust(Base6Directions.Direction direction)
    {
        float electricThrust = 0;
        for (int i = 0; i < electricThrusters[direction].Count; i++)
        {
            if (electricThrusters[direction][i].MaxEffectiveThrust / electricThrusters[direction][i].MaxThrust > cutoff)
            {
                electricThrust += electricThrusters[direction][i].MaxEffectiveThrust;
            }
        }
        return electricThrust;
    }
    float GetHydrogenThrust(Base6Directions.Direction direction)
    {
        float hydroThrust = 0;
        for (int i = 0; i < hydrogenThrusters[direction].Count; i++)
        {
            hydroThrust += hydrogenThrusters[direction][i].MaxEffectiveThrust;
        }
        return hydroThrust;
    }
    public void UpdateCopy()
    {
        if (!Enabled)
            return;
        double height;
        double newTarget;
        if (descending)
        {
            if (controller.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out height))
            {
                if (height > TargetAltDescending)
                {
                    newTarget = -((height - TargetAltDescending) / deceleration);
                }
                else
                {
                    newTarget = ((TargetAltDescending - height) / deceleration);
                }
                if (newTarget < TargetSpeed)
                    AdaptiveTargetSpeed = TargetSpeed;
                else
                    AdaptiveTargetSpeed = newTarget;
            }
        }
        else
        {
            if (controller.TryGetPlanetElevation(elevationType, out height))
            {
                if (height > TargetAltAcending)
                {
                    newTarget = -((height - TargetAltAcending) / deceleration);
                }
                else
                {
                    newTarget = ((TargetAltAcending - height) / deceleration);
                }
                if (newTarget > TargetSpeed)
                    AdaptiveTargetSpeed = TargetSpeed;
                else
                    AdaptiveTargetSpeed = newTarget;
            }
            else
            {
                if (DisableOnNaturalGravityExit && startedInNaturalGravity)
                {
                    Stop();
                    return;
                }
            }
        }
        double mass = controller.CalculateShipMass().PhysicalMass;
        double gravity = Vector3D.Dot(controller.GetNaturalGravity(), controller.WorldMatrix.GetOrientation().Down);
        var vel = controller.GetShipVelocities().LinearVelocity;
        double speed = 0;
        if (forward == Base6Directions.Direction.Up)
        {
            speed = (Vector3D.TransformNormal(vel, MatrixD.Transpose(controller.WorldMatrix))).Y;
        }
        else
        {
            speed = -(Vector3D.TransformNormal(vel, MatrixD.Transpose(controller.WorldMatrix))).Z;
        }
        double difference = AdaptiveTargetSpeed - speed;
        double errorMagnitude = MathHelper.Clamp(Math.Abs(difference / 5), 0, 1);
        float electricThrust = 0;
        for (int i = 0; i < electricThrusters[forward].Count; i++)
        {
            if (electricThrusters[forward][i].MaxEffectiveThrust / electricThrusters[forward][i].MaxThrust > cutoff)
            {
                electricThrust += electricThrusters[forward][i].MaxEffectiveThrust;
            }
        }
        float hydroThrust = 0;
        for (int i = 0; i < hydrogenThrusters[forward].Count; i++)
        {
            hydroThrust += hydrogenThrusters[forward][i].MaxEffectiveThrust;
        }
        mass *= gravity;
        float thrustOverride = MathHelper.Clamp((float)mass / electricThrust, 0, 1);
        float hydroThrustOverride = MathHelper.Clamp(((float)mass - electricThrust) / hydroThrust, 0, 1);
        float thrustExcess = 0;
        float hydroThrustExcess = 0;
        thrustExcess = 1 - thrustOverride;
        hydroThrustExcess = 1 - hydroThrustOverride;
        if (AdaptiveTargetSpeed > speed)
        {
            thrustOverride += thrustExcess * (float)errorMagnitude;
            hydroThrustOverride += hydroThrustExcess * (float)errorMagnitude;
        }
        else
        {
            thrustOverride *= 1 - (float)errorMagnitude;
            hydroThrustOverride *= 1 - (float)errorMagnitude;
        }
        if (thrustOverride <= 0 || double.IsNaN(thrustOverride))
            thrustOverride = 0.000001f;
        if (hydroThrustOverride <= 0 || double.IsNaN(thrustOverride))
            hydroThrustOverride = 0.000001f;
        for (int i = 0; i < electricThrusters[forward].Count; i++)
        {
            if (electricThrusters[forward][i].MaxEffectiveThrust / electricThrusters[forward][i].MaxThrust <= cutoff)
                electricThrusters[forward][i].ThrustOverridePercentage = 0.000001f;
            else
                electricThrusters[forward][i].ThrustOverridePercentage = thrustOverride;
        }
        for (int i = 0; i < hydrogenThrusters[forward].Count; i++)
        {
            hydrogenThrusters[forward][i].ThrustOverridePercentage = hydroThrustOverride;
        }
    }
    public void Start(float targetSpeed, Base6Directions.Direction forward, double targetAltDescending = 0, double targetAltAcending = double.MaxValue, bool useSealevel = false, bool disableOnNaturalGravityExit = true)
    {
        Enabled = true;
        TargetSpeed = targetSpeed;
        AdaptiveTargetSpeed = targetSpeed;
        if (forward == Base6Directions.Direction.Left)
            Forward = autoForward;
        else
            Forward = forward;
        if (targetSpeed < 0)
            descending = true;
        else
            descending = false;
        if (useSealevel)
            elevationType = MyPlanetElevation.Sealevel;
        else
            elevationType = MyPlanetElevation.Surface;
        TargetAltAcending = targetAltAcending;
        TargetAltDescending = targetAltDescending;
        DisableOnNaturalGravityExit = disableOnNaturalGravityExit;
        startedInNaturalGravity = controller.GetNaturalGravity().Length() > 0;
    }
    public void Stop()
    {
        Enabled = false;
        EnableThrusters(reverse, true, true);
        EnableThrusters(forward, true, true);
    }
    void EnableThrusters(Base6Directions.Direction direction, bool enable, bool disableThrustOverride = false)
    {
        for (int i = 0; i < electricThrusters[direction].Count; i++)
        {
            electricThrusters[direction][i].Enabled = enable;
            if (disableThrustOverride)
                electricThrusters[direction][i].ThrustOverride = 0;
        }
        for (int i = 0; i < hydrogenThrusters[direction].Count; i++)
        {
            hydrogenThrusters[direction][i].Enabled = enable;
            if (disableThrustOverride)
                hydrogenThrusters[direction][i].ThrustOverride = 0;
        }
    }
    void SetThrustOverride(Base6Directions.Direction direction, float amount)
    {
        for (int i = 0; i < electricThrusters[direction].Count; i++)
        {
            electricThrusters[direction][i].ThrustOverride = amount;
        }
        for (int i = 0; i < hydrogenThrusters[direction].Count; i++)
        {
            hydrogenThrusters[direction][i].ThrustOverride = amount;
        }
    }
}
IMyShipController MainController;
bool Initialized = false;
Aligner Align;
CruiseControl Cruise;
Screens LCDs;
MyIni Settings;
const string SettingsHeader = "ACC Settings";
const string SettingsController = "controller position";
const string SettingsTargetSpeed = "target speed";
const string SettingsSelectedThrusters = "selected thrusters";
const string SettingsTargetAltAscending = "target altitude ascending";
const string SettingsTargetAltDescending = "target altitude descending";
const string SettingsDisableCruiseExitingGravity = "disable cruise exiting gravity";
const string SettingsDisableAlignExitingGravity = "disable align exiting gravity";
const string SettingsUseSeaLevel = "use sealevel";
const string SettingsCruiseEnabled = "cruise enabled";
const string SettingsAlignEnabled = "align enabled";
const string SettingsWorldTopSpeed = "world top speed";
float targetSpeed = 95;
Base6Directions.Direction thrustDirection = Base6Directions.Direction.Left;
double targetAltAscending = double.MaxValue;
double targetAltDescending = 0;
bool disableCruiseExitingGravity = true;
bool disableAlignExitingGravity = true;
bool useSeaLevel = false;
float worldTopSpeed = 100;
Dictionary<string, string> Errors = new Dictionary<string, string>();
List<string> InputErrors = new List<string>();
TimeSpan InputErrorTimeout = TimeSpan.MinValue;
TimeSpan time;
int detailedInfoTextWith = 40;
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
public void Main(string argument, UpdateType updateType)
{
    time += Runtime.TimeSinceLastRun;
    if (!Initialized && time.Seconds % 5 == 0)
    {
        Initialized = Init();
    }
    if (Initialized)
    {
        if ((updateType & (UpdateType.Terminal | UpdateType.Trigger | UpdateType.Script)) != 0)
        {
            Input(argument);
        }
        if ((updateType & UpdateType.Update10) != 0)
        {
            if (Align != null && Align.Enabled)
            {
                Align.Update();
            }
            if (Cruise != null && Cruise.Enabled)
            {
                Cruise.Update(Runtime.TimeSinceLastRun.TotalSeconds);
            }
            if (LCDs != null)
            {
                ScreenData data = new ScreenData();
                data.maxAlt = targetAltAscending;
                data.minAlt = targetAltDescending;
                data.maxSpeed = worldTopSpeed;
                if (Cruise != null)
                {
                    if (Cruise.Enabled)
                    {
                        data.speed = Cruise.Speed;
                        data.targetSpeed = Cruise.AdaptiveTargetSpeed;
                    }
                    else if (MainController != null)
                    {
                        data.speed = MainController.GetShipSpeed();
                        data.targetSpeed = targetSpeed;
                    }
                    data.thrustOverrideH2 = Cruise.ThrustOverrideHydrogen;
                    data.thrustOverridePW = Cruise.ThrustOverrideElectric;
                    data.cruiseEnabled = Cruise.Enabled;
                }
                else
                {
                    if (MainController != null)
                    {
                        data.speed = MainController.GetShipSpeed();
                    }
                    data.targetSpeed = targetSpeed;
                }
                if (Align != null)
                {
                    data.alignEnabled = Align.Enabled;
                }
                LCDs.Update(data);
            }
        }
    }
    DetailedInfo();
    BroadcastStatus();
}
void ResetController()
{
    Settings.Set(SettingsHeader, SettingsController, "");
    SaveSettings();
    Initialized = false;
    MainController = null;
}
void ToggleCruiseControl()
{
    if (Cruise != null)
    {
        if (Cruise.Enabled)
        {
            StartCruiseControl(false);
        }
        else
        {
            StartCruiseControl(true);
        }
    }
}
void StartCruiseControl(bool enable)
{
    if (Initialized && Cruise != null)
    {
        if (enable)
        {
            Cruise.Start(targetSpeed, thrustDirection, targetAltDescending, targetAltAscending, useSeaLevel, disableCruiseExitingGravity);
        }
        else
        {
            Cruise.Stop();
        }
    }
}
void ToggleAlign()
{
    if (Initialized && Align != null)
    {
        if (Align.Enabled)
        {
            StartAlign(false);
        }
        else
        {
            StartAlign(true);
        }
    }
}
void StartAlign(bool enable)
{
    if (Align != null)
    {
        if (enable)
        {
            Align.Start(disableAlignExitingGravity);
        }
        else
        {
            Align.Stop();
        }
    }
}
void Input(string argument)
{
    InputErrors.Clear();
    argument = argument.ToLower().Trim();
    if (argument.Length == 0)
    {
        ToggleCruiseControl();
    }
    else if (argument == "on")
    {
        StartCruiseControl(true);
    }
    else if (argument == "off")
    {
        StartCruiseControl(false);
    }
    else if (argument == "swap")
    {
        if (thrustDirection == Base6Directions.Direction.Forward)
            thrustDirection = Base6Directions.Direction.Up;
        else if (thrustDirection == Base6Directions.Direction.Up)
            thrustDirection = Base6Directions.Direction.Forward;
        if (Cruise != null && Cruise.Enabled)
        {
            StartCruiseControl(false);
            StartCruiseControl(true);
        }
    }
    else if (argument == "reset")
    {
        ResetController();
        return;
    }
    else
    {
        float newSpeed = 0;
        if (float.TryParse(argument, out newSpeed))
        {
            targetSpeed = newSpeed;
            StartCruiseControl(true);
        }
        else
        {
            string[] commands = argument.ToLower().Split(',');
            foreach (var command in commands)
            {
                string[] parts = command.Trim(new char[] { ' ', '\'' }).Split(' ');
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = parts[i].Trim();
                }
                if (parts.Length > 0)
                {
                    if (parts[0] == "?")
                    {
                        InputErrors.Add("Errors shows up here.");
                    }
                    else if (parts[0] == "cruise" || parts[0] == "cc")
                    {
                        if (parts.Length > 1)
                        {
                            float num = 0;
                            bool on = true;
                            for (int i = 1; i < parts.Length; i++)
                            {
                                if (float.TryParse(parts[i], out num))
                                {
                                    if (targetSpeed == num && Cruise != null && Cruise.Enabled)
                                    {
                                        on = false;
                                    }
                                    targetSpeed = num;
                                }
                                else if (parts[i] == "on")
                                {
                                    on = true;
                                }
                                else if (parts[i] == "off")
                                {
                                    on = false;
                                }
                                else
                                {
                                    InputErrors.Add("'" + parts[1] + "' not recognozed for 'cruise' use: on|off (or omitt for toggle).");
                                }
                            }
                            StartCruiseControl(on);
                        }
                        else
                        {
                            ToggleCruiseControl();
                        }
                    }
                    else if (parts[0] == "align" || parts[0] == "al")
                    {
                        if (parts.Length > 1)
                        {
                            bool on = true;
                            for (int i = 1; i < parts.Length; i++)
                            {
                                if (parts[i] == "on")
                                {
                                    on = true;
                                }
                                else if (parts[i] == "off")
                                {
                                    on = false;
                                }
                                else
                                {
                                    InputErrors.Add("'" + parts[1] + "' not recognozed for 'align' use: on|off (or omitt for toggle).");
                                }
                            }
                            StartAlign(on);
                        }
                        else
                        {
                            ToggleAlign();
                        }
                    }
                    else if (parts[0] == "ts")
                    {
                        if (parts.Length > 1)
                        {
                            float num = 0;
                            if (float.TryParse(parts[1], out num))
                            {
                                targetSpeed = num;
                                if (Cruise != null && Cruise.Enabled)
                                    StartCruiseControl(true);
                            }
                            else
                            {
                                InputErrors.Add("'" + parts[1] + "' not recognized as a number. 'ts' requires one (negative for descent).");
                            }
                        }
                        else
                        {
                            InputErrors.Add("'ts' requires a number (negative for descent).");
                        }
                    }
                    else if (parts[0] == "td")
                    {
                        if (parts.Length > 1)
                        {
                            switch (parts[1])
                            {
                                case "up":
                                    thrustDirection = Base6Directions.Direction.Up;
                                    break;
                                case "down":
                                    thrustDirection = Base6Directions.Direction.Down;
                                    break;
                                case "forward":
                                    thrustDirection = Base6Directions.Direction.Forward;
                                    break;
                                case "backward":
                                    thrustDirection = Base6Directions.Direction.Backward;
                                    break;
                                case "auto":
                                    thrustDirection = Base6Directions.Direction.Left;
                                    break;
                                default:
                                    InputErrors.Add("'" + parts[1] + "' not recognized for 'td', use: up | down | forward | backward | auto.");
                                    break;
                            }
                            if (Cruise != null && Cruise.Enabled)
                            {
                                StartCruiseControl(false);
                                StartCruiseControl(true);
                            }
                            if (Align != null && Align.Enabled)
                            {
                                StartAlign(true);
                            }
                        }
                        else
                        {
                            InputErrors.Add("'td' requires a direction: up | down | forward | backward | auto.");
                        }
                    }
                    else if (parts[0] == "mx")
                    {
                        if (parts.Length > 1)
                        {
                            double num = 0;
                            if (double.TryParse(parts[1], out num))
                            {
                                if (num <= 0)
                                {
                                    targetAltAscending = double.MaxValue;
                                }
                                else
                                {
                                    targetAltAscending = num;
                                }
                                if (Cruise != null && Cruise.Enabled)
                                    StartCruiseControl(true);
                            }
                            else if (parts[1] == "this")
                            {
                                if (MainController != null)
                                {
                                    double elev = 0;
                                    if (MainController.TryGetPlanetElevation((useSeaLevel ? MyPlanetElevation.Sealevel : MyPlanetElevation.Surface), out elev))
                                    {
                                        targetAltAscending = elev;
                                    }
                                }
                                else
                                {
                                    InputErrors.Add("No controller, can set 'mx' to 'this'.");
                                }
                            }
                            else
                            {
                                InputErrors.Add("'mx' requires a number or 'this', couldn't parse '" + parts[1] + "'.");
                            }
                        }
                        else
                        {
                            InputErrors.Add("'mx' requires a number (set to -1 for unlimited) or 'this' for current altitude.");
                        }
                    }
                    else if (parts[0] == "mn")
                    {
                        if (parts.Length > 1)
                        {
                            double num = 0;
                            if (double.TryParse(parts[1], out num))
                            {
                                if (num < 0)
                                {
                                    targetAltDescending = 0;
                                }
                                else
                                {
                                    targetAltDescending = num;
                                }
                                if (Cruise != null && Cruise.Enabled)
                                    StartCruiseControl(true);
                            }
                            else if (parts[1] == "this")
                            {
                                if (MainController != null)
                                {
                                    double elev = 0;
                                    if (MainController.TryGetPlanetElevation(MyPlanetElevation.Surface, out elev))
                                    {
                                        targetAltDescending = elev;
                                    }
                                }
                                else
                                {
                                    InputErrors.Add("No controller, can set 'mn' to 'this'.");
                                }
                            }
                            else
                            {
                                InputErrors.Add("'mn' requires a number or 'this', couldn't parse '" + parts[1] + "'.");
                            }
                        }
                        else
                        {
                            InputErrors.Add("'mn' requires a number or 'this' for current altitude.");
                        }
                    }
                    else if (parts[0] == "rf")
                    {
                        if (parts.Length > 1)
                        {
                            if (parts[1] == "sea" || parts[1] == "sealevel")
                            {
                                useSeaLevel = true;
                            }
                            else
                            {
                                useSeaLevel = false;
                            }
                            if (Cruise != null && Cruise.Enabled)
                                StartCruiseControl(true);
                        }
                        else
                        {
                            InputErrors.Add("'rf' requires a reference point: sea | ground");
                        }
                    }
                    else if (parts[0] == "ag")
                    {
                        if (parts.Length > 1)
                        {
                            if (parts[1] == "yes")
                            {
                                disableAlignExitingGravity = true;
                            }
                            else
                            {
                                disableAlignExitingGravity = false;
                            }
                            if (Cruise != null && Cruise.Enabled)
                                StartCruiseControl(true);
                        }
                        else
                        {
                            InputErrors.Add("'ag' requires a state: on | off.");
                        }
                    }
                    else if (parts[0] == "cg")
                    {
                        if (parts.Length > 1)
                        {
                            if (parts[1] == "yes")
                            {
                                disableCruiseExitingGravity = true;
                            }
                            else
                            {
                                disableCruiseExitingGravity = false;
                            }
                            if (Cruise != null && Cruise.Enabled)
                                StartCruiseControl(true);
                        }
                        else
                        {
                            InputErrors.Add("'cg' requires a state: on | off.");
                        }
                    }
                    else if (parts[0] == "ws")
                    {
                        if (parts.Length > 1)
                        {
                            float num = 0;
                            if (float.TryParse(parts[1], out num))
                            {
                                worldTopSpeed = num;
                            }
                            else
                            {
                                InputErrors.Add("'" + parts[1] + "' not recognized as a number. 'ws' requires one.");
                            }
                        }
                        else
                        {
                            InputErrors.Add("'ws' requires a number.");
                        }
                    }
                    else
                    {
                        InputErrors.Add("Command not recognized '" + parts[0] + "'");
                    }
                }
                else
                {
                    InputErrors.Add("Input not recognized '" + command + "'");
                }
            }
        }
    }
    if (InputErrors.Count > 0)
    {
        InputErrorTimeout = TimeSpan.FromSeconds(60) + time;
    }
    else
    {
        InputErrorTimeout = TimeSpan.MinValue;
    }
    SaveSettings();
}
void DetailedInfo()
{
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Blarg's Ascent Cruise Control");
    sb.AppendLine($"Cruise: {(Cruise != null && Cruise.Enabled ? "on" : "off")}   |   Align: {(Align != null && Align.Enabled ? "on" : "off")} ");
    if (Errors.Count > 0)
    {
        sb.AppendLine();
        sb.AppendLine("____Errors_________");
        foreach (var item in Errors)
        {
            sb.AppendLine(AdjustTextToWidth("- " + item.Value, detailedInfoTextWith));
        }
    }
    if (InputErrorTimeout > time && InputErrors.Count > 0)
    {
        sb.AppendLine();
        sb.AppendLine("____Input Errors_________");
        foreach (var item in InputErrors)
        {
            sb.AppendLine(AdjustTextToWidth("- " + item, detailedInfoTextWith));
        }
    }
    sb.AppendLine();
    sb.AppendLine("____Start/stop_________");
    sb.AppendLine("Commands:");
    sb.AppendLine("- cruise [on|off][#]");
    sb.AppendLine("- align [on|off]");
    sb.AppendLine("[optional], will toggle if");
    sb.AppendLine("omitted. # is target speed.");
    sb.AppendLine();
    sb.AppendLine("____Settings_________");
    sb.AppendLine("Type 'prefix value' and press");
    sb.AppendLine("run to set, for example 'ts 100'");
    sb.AppendLine("Run just the prefix for details.");
    if (Cruise != null)
    {
        sb.AppendLine();
        sb.AppendLine("____Cruise Control_________");
        sb.AppendLine($"ts    Target speed: {targetSpeed.ToString("n0")} ");
        sb.AppendLine($"cg   Disable at zero-G: {(disableCruiseExitingGravity ? "Yes" : "No")}");
        sb.AppendLine($"td    Travel direction: {(thrustDirection == Base6Directions.Direction.Left ? "Auto(" + Cruise.Forward.ToString() + ")" : thrustDirection.ToString())}");
        sb.AppendLine($"mx  Target altitude max: {(targetAltAscending == double.MaxValue ? "••" : targetAltAscending.ToString("n0") + "m")}");
        sb.AppendLine($"mn  Target altitude min: {targetAltDescending.ToString("n0") + "m"}");
        sb.AppendLine($"rf     Max altitude reference: {(useSeaLevel ? "Sealevel" : "Ground")}");
        sb.AppendLine($"ws  World top speed: {worldTopSpeed.ToString("n0")}");
    }
    if (Align != null)
    {
        sb.AppendLine();
        sb.AppendLine("____Aligner_________");
        sb.AppendLine($"ag   Disable at zero-G: {(disableAlignExitingGravity ? "yes" : "no")}");
    }
    Echo(sb.ToString());
}
void BroadcastStatus()
{
    if (Initialized)
    {
        IGC.SendBroadcastMessage("ACC", Cruise.AdaptiveTargetSpeed, TransmissionDistance.CurrentConstruct);
    }
}
bool GetController(List<IMyShipController> controllers, Vector3I position)
{
    foreach (var item in controllers)
    {
        if (item.Position == position)
        {
            MainController = item;
            return true;
        }
    }
    return false;
}
bool GetController(List<IMyShipController> controllers)
{
    foreach (var item in controllers)
    {
        if (item.CanControlShip)
        {
            if (item.IsUnderControl)
            {
                MainController = item;
                return true;
            }
        }
    }
    return false;
}
bool Init()
{
    var controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, x => x.IsSameConstructAs(Me));
    bool cruiseEnabledInSettings = false;
    bool alignEnabledInSettings = false;
    Settings = new MyIni();
    if (Settings.TryParse(Storage))
    {
        if (Settings.ContainsSection(SettingsHeader))
        {
            targetSpeed = Settings.Get(SettingsHeader, SettingsTargetSpeed).ToSingle(targetSpeed);
            thrustDirection = (Base6Directions.Direction)Settings.Get(SettingsHeader, SettingsSelectedThrusters).ToInt32((int)thrustDirection);
            targetAltAscending = Settings.Get(SettingsHeader, SettingsTargetAltAscending).ToDouble(targetAltAscending);
            targetAltDescending = Settings.Get(SettingsHeader, SettingsTargetAltDescending).ToDouble(targetAltDescending);
            disableCruiseExitingGravity = Settings.Get(SettingsHeader, SettingsDisableCruiseExitingGravity).ToBoolean(disableCruiseExitingGravity);
            disableAlignExitingGravity = Settings.Get(SettingsHeader, SettingsDisableAlignExitingGravity).ToBoolean(disableAlignExitingGravity);
            useSeaLevel = Settings.Get(SettingsHeader, SettingsUseSeaLevel).ToBoolean(useSeaLevel);
            worldTopSpeed = Settings.Get(SettingsHeader, SettingsWorldTopSpeed).ToSingle(worldTopSpeed);
            cruiseEnabledInSettings = Settings.Get(SettingsHeader, SettingsCruiseEnabled).ToBoolean();
            alignEnabledInSettings = Settings.Get(SettingsHeader, SettingsCruiseEnabled).ToBoolean();
            var parts = Settings.Get(SettingsHeader, SettingsController).ToString().Split(';');
            if (parts.Length == 3)
            {
                GetController(controllers, new Vector3I(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
            }
        }
    }
    if (MainController == null)
    {
        if (!GetController(controllers))
        {
            Errors["no controller"] = "No ship controller found. Can't resume. Sit in one for a few seconds.";
            return false;
        }
    }
    if (MainController != null)
    {
        Errors.Remove("no controller");
    }
    Settings.Set(SettingsHeader, SettingsController, $"{MainController.Position.X};{MainController.Position.Y};{MainController.Position.Z}");
    SaveSettings();
    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(allBlocks, x => x.IsSameConstructAs(Me));
    List<IMyGyro> gyros = new List<IMyGyro>();
    List<IMyThrust> thrusters = new List<IMyThrust>();
    List<IMyTextSurface> screens = new List<IMyTextSurface>();
    foreach (var item in allBlocks)
    {
        if (item is IMyGyro)
        {
            gyros.Add(item as IMyGyro);
        }
        if (item is IMyThrust)
        {
            thrusters.Add(item as IMyThrust);
        }
        if (item is IMyTextPanel)
        {
            if (item.CustomName.Contains("#ACC"))
            {
                screens.Add(item as IMyTextSurface);
                screens[screens.Count - 1].ContentType = ContentType.SCRIPT;
                screens[screens.Count - 1].Script = "";
                screens[screens.Count - 1].ScriptBackgroundColor = BackgroundColor;
                screens[screens.Count - 1].ScriptForegroundColor = ForegroundColor;
            }
        }
        if (item is IMyCockpit)
        {
            if (item.CustomName.Contains("#ACC"))
            {
                int screennr = 0;
                var parts = item.CustomName.Split('@');
                if (parts.Length > 1)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i].EndsWith("#ACC") && parts.Length > i + 1)
                        {
                            int.TryParse(new string(parts[i + 1].TakeWhile(char.IsDigit).ToArray()), out screennr);
                        }
                    }
                }
                screens.Add((item as IMyTextSurfaceProvider).GetSurface(screennr));
                screens[screens.Count - 1].ContentType = ContentType.SCRIPT;
                screens[screens.Count - 1].Script = "";
                screens[screens.Count - 1].ScriptBackgroundColor = BackgroundColor;
                screens[screens.Count - 1].ScriptForegroundColor = ForegroundColor;
            }
        }
    }
    if (screens.Count > 0)
    {
        LCDs = new Screens(screens);
    }
    if (gyros.Count > 0)
    {
        Align = new Aligner(MainController, gyros);
        if (alignEnabledInSettings)
        {
            StartAlign(true);
        }
    }
    if (thrusters.Count > 0)
    {
        Cruise = new CruiseControl(MainController, thrusters);
        if (cruiseEnabledInSettings)
        {
            StartCruiseControl(true);
        }
    }
    return true;
}
void SaveSettings()
{
    if (Settings == null)
        Settings = new MyIni();
    Settings.Set(SettingsHeader, SettingsTargetSpeed, targetSpeed);
    Settings.Set(SettingsHeader, SettingsSelectedThrusters, (int)thrustDirection);
    Settings.Set(SettingsHeader, SettingsTargetAltAscending, targetAltAscending);
    Settings.Set(SettingsHeader, SettingsTargetAltDescending, targetAltDescending);
    Settings.Set(SettingsHeader, SettingsDisableCruiseExitingGravity, disableCruiseExitingGravity);
    Settings.Set(SettingsHeader, SettingsDisableAlignExitingGravity, disableAlignExitingGravity);
    Settings.Set(SettingsHeader, SettingsUseSeaLevel, useSeaLevel);
    Settings.Set(SettingsHeader, SettingsCruiseEnabled, (Cruise == null ? false : Cruise.Enabled));
    Settings.Set(SettingsHeader, SettingsAlignEnabled, (Align == null ? false : Align.Enabled));
    Settings.Set(SettingsHeader, SettingsWorldTopSpeed, worldTopSpeed);
    Storage = Settings.ToString();
}
public static string AdjustTextToWidth(string text, int width)
{
    string rest = text;
    string output = "";
    if (rest.Length > width)
    {
        while (rest.Length > width)
        {
            string part = rest.Substring(0, width);
            rest = rest.Substring(width);
            for (int i = part.Length - 1; i > 0; i--)
            {
                if (part[i] == ' ')
                {
                    output += part.Substring(0, i) + "\n";
                    rest = part.Substring(i + 1) + rest;
                    break;
                }
            }
        }
    }
    output += rest;
    return output;
}
struct ScreenData
{
    public double maxAlt;
    public double minAlt;
    public double speed;
    public double maxSpeed;
    public double targetSpeed;
    public float thrustOverrideH2;
    public float thrustOverridePW;
    public bool cruiseEnabled;
    public bool alignEnabled;
}
class Screens
{
    List<IMyTextSurface> surfaces;
    public Screens(List<IMyTextSurface> surfaces)
    {
        this.surfaces = surfaces;
    }
    public void Update(ScreenData data)
    {
        foreach (var surface in surfaces)
        {
            if (surface.SurfaceSize.Y / surface.SurfaceSize.X >= 0.5f)
            {
                DrawRegular(surface, data);
            }
            else
            {
                DrawSlim(surface);
            }
        }
    }
    void DrawRegular(IMyTextSurface surface, ScreenData data)
    {
        Color background = surface.ScriptBackgroundColor;
        Color foreground = surface.ScriptForegroundColor;
        Color white = new Color(230, 230, 230);
        Color gray1 = new Color(130, 130, 130);
        Color gray2 = new Color(15, 15, 15);
        Color darken = new Color(50, 50, 50, 200);
        Color darken2 = new Color(0, 0, 0, 150);
        white = foreground;
        var hsv = ColorExtensions.ColorToHSV(foreground);
        hsv.Y *= 0.6f;
        hsv.Z *= 0.6f;
        gray1 = ColorExtensions.HSVtoColor(hsv);
        Canvas canvas = new Canvas((surface.TextureSize - Vec(surface.SurfaceSize.Y * 1f)) * 0.5f, Vec(surface.SurfaceSize.Y * 1f));
        using (var frame = surface.DrawFrame())
        {
            frame.Add(canvas.Circle(Vec(0, 0), Vec(0.95f), darken));
            frame.Add(canvas.Rect(Vec(0, -0.136f), Vec(0.95f, 0.9f), background));
            frame.Add(canvas.CircleHollow(Vec(0, 0), Vec(1f), gray2));
            frame.Add(canvas.CircleHollow(Vec(0, 0), Vec(0.95f), gray1));

            // Hydrogen thrust override bar
            frame.Add(canvas.Rect(Vec(0.3f, 0), Vec(0.055f, 0.3f), darken));
            frame.Add(canvas.Progress(Vec(0.3f, 0), Vec(0.3f, 0.055f), Color.Orange, data.thrustOverrideH2, rotation: 4.71239f));
            frame.Add(canvas.TriangleRight(Vec(0.3f, -0.13f), Vec(0.06f, 0.04f), background, rotation: MathHelper.ToRadians(180)));
            frame.Add(canvas.TriangleRight(Vec(0.3f, 0.13f), Vec(0.04f, 0.06f), background, rotation: MathHelper.ToRadians(-90)));
            frame.Add(canvas.Rect(Vec(0.3f, 0), Vec(0.06f, 0.008f), background));

            // Electrics thrust override bar
            frame.Add(canvas.Rect(Vec(-0.3f, 0), Vec(0.055f, 0.3f), darken));
            frame.Add(canvas.Progress(Vec(-0.3f, 0), Vec(0.3f, 0.055f), Color.Blue, data.thrustOverridePW, rotation: 4.71239f));
            frame.Add(canvas.TriangleRight(Vec(-0.3f, -0.13f), Vec(0.04f, 0.06f), background, rotation: MathHelper.ToRadians(90)));
            frame.Add(canvas.TriangleRight(Vec(-0.3f, 0.13f), Vec(0.06f, 0.04f), background));
            frame.Add(canvas.Rect(Vec(0 - .3f, 0), Vec(0.06f, 0.008f), background));

            // H2 and Electrics icons
            frame.Add(canvas.Sprite("IconHydrogen", Vec(0.3f, -0.2f), Vec(0.08f, 0.08f), white, Anchor.Center));
            frame.Add(canvas.Sprite("IconEnergy", Vec(-0.3f, -0.2f), Vec(0.1f, 0.1f), white, Anchor.Center));

            // H2 and Electrics override percent, below progress bars
            frame.Add(canvas.Text((data.cruiseEnabled ? string.Format("{0:0%}", data.thrustOverrideH2) : ""), Vec(0.30f, 0.185f), 0.08f, white));
            frame.Add(canvas.Text((data.cruiseEnabled ? string.Format("{0:0%}", data.thrustOverridePW) : ""), Vec(-0.30f, 0.185f), 0.08f, white));

            // Target Speed marker - Dark triangle on outer circle
            double speedAngle = ((Math.Abs(data.targetSpeed) / data.maxSpeed) * 4.71238898) + 0.785398163;
            frame.Add(canvas.Triangle(Vec(0, 0), Vec(0.04f), gray2, rotation: (float)speedAngle, offset: Vec(0, 0.46f)));

            // Current Speed marker - White rectangle on outer circle
            speedAngle = ((Math.Abs(data.speed) / data.maxSpeed) * 4.71238898) + 0.785398163;
            frame.Add(canvas.Rect(Vec(0, 0), Vec(0.01f, 0.1f), white, rotation: (float)speedAngle, offset: Vec(0, 0.435f)));

            // Speed intermediate marks on outer circle (white triangles)
            frame.Add(canvas.Triangle(Vec(0, 0), Vec(0.01f, 0.06f), white, rotation: (float)Math.PI, offset: Vec(0, 0.468f)));
            frame.Add(canvas.Triangle(Vec(0, 0), Vec(0.01f, 0.06f), white, rotation: (float)Math.PI * 0.625f, offset: Vec(0, 0.468f)));
            frame.Add(canvas.Triangle(Vec(0, 0), Vec(0.01f, 0.06f), white, rotation: (float)Math.PI * 0.25f, offset: Vec(0, 0.468f)));
            frame.Add(canvas.Triangle(Vec(0, 0), Vec(0.01f, 0.06f), white, rotation: (float)Math.PI * -0.625f, offset: Vec(0, 0.468f)));
            frame.Add(canvas.Triangle(Vec(0, -0), Vec(0.01f, 0.06f), white, rotation: (float)Math.PI * -0.25f, offset: Vec(0, 0.468f)));

            // Current and target speed text, followed by units label m/s
            frame.Add(canvas.Text(data.speed.ToString("n0"), Vec(0, -0.20f), 0.16f, white));
            frame.Add(canvas.Text($"[{data.targetSpeed.ToString("n0")}]", Vec(0, -0.09f), 0.14f, white));
            frame.Add(canvas.Text("m/s", Vec(0, 0), 0.1f, gray1));

            // Cruise and align enable state text labels
            frame.Add(canvas.Text("Cruise", Vec(0, 0.15f), 0.1f, gray1));
            frame.Add(canvas.Text("Align", Vec(0, 0.25f), 0.1f, gray1));

            // Cruise state indicator
            if (data.cruiseEnabled)
            {
                frame.Add(canvas.Rect(Vec(-0.15f, 0.15f), Vec(0.02f, 0.06f), Color.Green));
            }
            else
            {
                frame.Add(canvas.Rect(Vec(-0.15f, 0.15f), Vec(0.02f, 0.06f), Color.Red));
            }

            // Align state indicator
            if (data.alignEnabled)
            {
                frame.Add(canvas.Rect(Vec(-0.15f, 0.25f), Vec(0.02f, 0.06f), Color.Green));
            }
            else
            {
                frame.Add(canvas.Rect(Vec(-0.15f, 0.25f), Vec(0.02f, 0.06f), Color.Red));
            }

            // (Unsure what these are - they appear to be overwriting some areas with background color for no obvious reason)
            frame.Add(canvas.Triangle(Vec(-0.13f, 0.10f), Vec(0.08f), background, rotation: 1.57079633f));
            frame.Add(canvas.Triangle(Vec(-0.13f, 0.20f), Vec(0.08f), background, rotation: 1.57079633f));
            frame.Add(canvas.TriangleRight(Vec(-0.13f, 0.284f), Vec(0.08f, 0.04f), background));

            // Ascent cutoff altitude icon (light gray triangle below rectangle)
            frame.Add(canvas.Triangle(Vec(0, -0.41f), Vec(0.04f, 0.06f), gray1, Anchor.Center));
            frame.Add(canvas.Rect(Vec(0, -0.426f), Vec(0.04f, 0.01f), gray1, Anchor.Center));

            // Descent cutoff altitude icon (inverted light gray triangle over rectangle)
            frame.Add(canvas.Triangle(Vec(0, 0.41f), Vec(0.04f, 0.06f), gray1, Anchor.Center, rotation: (float)Math.PI));
            frame.Add(canvas.Rect(Vec(0, 0.426f), Vec(0.04f, 0.01f), gray1, Anchor.Center));

            // Show configured max altitude cutoff (ascent), if any configured
            if (data.maxAlt == double.MaxValue)
            {
                frame.Add(canvas.Text("--", Vec(0, -0.35f), 0.07f, gray1));
            }
            else
            {
                frame.Add(canvas.Text(data.maxAlt.ToString("n0") + "m", Vec(0, -0.35f), 0.07f, gray1));
            }

            // Show configured min altitude cutoff (descent) (if unconfigured this will be zero)
            frame.Add(canvas.Text(data.minAlt.ToString("n1") + "m", Vec(0, 0.35f), 0.07f, gray1));
        }
    }
    void DrawRegular_Copy(IMyTextSurface surface)
    {
        Color background = surface.ScriptBackgroundColor;
        Color foreground = surface.ScriptForegroundColor;
        Color white = new Color(230, 230, 230);
        Color gray1 = new Color(50, 50, 50);
        Color gray2 = new Color(25, 25, 25);
        Color darken = new Color(0, 0, 0, 100);
        Color darken2 = new Color(0, 0, 0, 150);
        Canvas canvas = new Canvas(Vec(0, surface.TextureSize.Y - surface.SurfaceSize.Y), Vec(surface.SurfaceSize.Y));
        if (background.ColorToHSV().Z < 0.4)
        {
            white = new Color(230, 230, 230);
            darken = new Color(255, 255, 255, 5);
            darken2 = new Color(255, 255, 255, 15);
        }
        using (var frame = surface.DrawFrame())
        {
            frame.Add(canvas.Rect(Vec(0.325f, 0f), Vec(0.3f, 0.38f), darken));
            frame.Add(canvas.Rect(Vec(0.325f, 0.19f), Vec(0.3f, 0.01f), Color.Orange, offset: Vec(0, -(0.38f * 0.25f))));
            frame.Add(canvas.Rect(Vec(-0.325f, 0f), Vec(0.3f, 0.38f), darken, Anchor.Center));
            frame.Add(canvas.Rect(Vec(-0.325f, 0.19f), Vec(0.3f, 0.01f), Color.Blue, offset: Vec(0, -(0.38f * 0.71f))));
            frame.Add(canvas.Rect(Vec(0, 0.2325f), Vec(0.86f, 0.085f), darken2));
            frame.Add(canvas.CircleHollow(Vector2.Zero, Vector2.One, gray1));
            frame.Add(canvas.CircleHollow(Vector2.Zero, Vector2.One * 1.03f, gray2));
            frame.Add(canvas.Rect(Vec(0, -0.19f - 0.25f), Vec(1.1f, 0.5f), background));
            frame.Add(canvas.Circle(Vec(0, -0.05f), Vec(0.62f), background));
            frame.Add(canvas.CircleHollow(Vec(0, -0.05f), Vec(0.62f), gray1));
            frame.Add(canvas.CircleHollow(Vec(0, -0.05f), Vec(0.65f), gray2));
            frame.Add(canvas.Rect(Vec(0, 0.525f), Vec(1.1f, 0.5f), background));
            frame.Add(canvas.Triangle(Vec(0, -0.05f), Vec(0.01f, 0.06f), gray1, rotation: (float)Math.PI, offset: Vec(0, 0.26f)));
            frame.Add(canvas.Triangle(Vec(0, -0.05f), Vec(0.01f, 0.06f), gray1, rotation: (float)Math.PI * 0.625f, offset: Vec(0, 0.26f)));
            frame.Add(canvas.Triangle(Vec(0, -0.05f), Vec(0.01f, 0.06f), gray1, rotation: (float)Math.PI * 0.25f, offset: Vec(0, 0.26f)));
            frame.Add(canvas.Triangle(Vec(0, -0.05f), Vec(0.01f, 0.06f), gray1, rotation: (float)Math.PI * -0.625f, offset: Vec(0, 0.26f)));
            frame.Add(canvas.Triangle(Vec(0, -0.05f), Vec(0.01f, 0.06f), gray1, rotation: (float)Math.PI * -0.25f, offset: Vec(0, 0.26f)));
            frame.Add(canvas.Rect(Vec(0, -0.05f), Vec(0.01f, 0.1f), white, rotation: (float)Math.PI * 0.785f, offset: Vec(0, 0.23f)));
            frame.Add(canvas.Sprite("IconHydrogen", Vec(-0.35f, -0.25f), Vec(0.08f, 0.08f), white, Anchor.Center));
            frame.Add(canvas.Sprite("IconEnergy", Vec(0.35f, -0.25f), Vec(0.1f, 0.1f), white, Anchor.Center));
            frame.Add(canvas.Text("25%", Vec(0.30f, 0.235f), 0.08f, white));
            frame.Add(canvas.Text("71%", Vec(-0.30f, 0.235f), 0.08f, white));
            frame.Add(canvas.Text("46", Vec(0, -0.1f), 0.16f, white));
            frame.Add(canvas.Text("[99]", Vec(0, 0.02f), 0.14f, white));
            frame.Add(canvas.Text("m/s", Vec(0, 0.15f), 0.1f, gray1));
        }
    }
    void DrawSlim(IMyTextSurface surface)
    {
    }
    Vector2 Vec(float x, float y)
    {
        return new Vector2(x, y);
    }
    Vector2 Vec(float xy)
    {
        return new Vector2(xy, xy);
    }
}
class Canvas
{
    Vector2 canvasPosition;
    Vector2 canvasSize;
    Vector2 center;
    public Canvas(Vector2 position, Vector2 size)
    {
        canvasPosition = position;
        canvasSize = size;
        center = Vector2.One * 0.5f;
    }
    public MySprite Rect(Vector2 position, Vector2 size, Color color, Anchor anchor = Anchor.Center, float rotation = 0, Vector2? offset = null)
    {
        if (offset == null)
            offset = Vector2.Zero;
        return Sprite("SquareSimple", position, size, color, anchor, rotation, offset);
    }
    public MySprite Circle(Vector2 position, Vector2 size, Color color, Anchor anchor = Anchor.Center, float rotation = 0)
    {
        return Sprite("Circle", position, size, color, anchor, rotation);
    }
    public MySprite CircleHollow(Vector2 position, Vector2 size, Color color, Anchor anchor = Anchor.Center, float rotation = 0)
    {
        return Sprite("CircleHollow", position, size, color, anchor, rotation);
    }
    public MySprite Triangle(Vector2 position, Vector2 size, Color color, Anchor anchor = Anchor.Center, float rotation = 0, Vector2? offset = null)
    {
        if (offset == null)
            offset = Vector2.Zero;
        return Sprite("Triangle", position, size, color, anchor, rotation, offset);
    }
    public MySprite TriangleRight(Vector2 position, Vector2 size, Color color, Anchor anchor = Anchor.Center, float rotation = 0, Vector2? offset = null)
    {
        if (offset == null)
            offset = Vector2.Zero;
        return Sprite("RightTriangle", position, size, color, anchor, rotation, offset);
    }
    public MySprite SemiCircle(Vector2 position, Vector2 size, Color color, Anchor anchor = Anchor.Center, float rotation = 0, Vector2? offset = null)
    {
        if (offset == null)
            offset = Vector2.Zero;
        return Sprite("SemiCircle", position, size, color, anchor, rotation, offset);
    }
    public MySprite Progress(Vector2 position, Vector2 size, Color color, float percent, Anchor anchor = Anchor.Center, float rotation = 0)
    {
        size = size * canvasSize;
        Vector2 spriteSize = size;
        spriteSize.X = MathHelper.Clamp(percent, 0, 1) * size.X;
        Vector2 rotateAround = ((center + position) * canvasSize) + canvasPosition;
        position = rotateAround - (Vector2.UnitX * size.X * 0.5f) + (Vector2.UnitX * spriteSize.X * 0.5f);
        return new MySprite(SpriteType.TEXTURE, "SquareSimple", position: AdjustToRotation(AdjustToAnchor(anchor, position, size), rotateAround, rotation), size: spriteSize, color: color, rotation: rotation);
    }
    public MySprite Sprite(string sprite, Vector2 position, Vector2 size, Color color, Anchor anchor, float rotation = 0, Vector2? offset = null)
    {
        if (offset == null)
            offset = Vector2.Zero;
        size = size * canvasSize;
        Vector2 rotateAround = ((center + position) * canvasSize) + canvasPosition;
        position = rotateAround + (Vector2)offset * canvasSize;
        return new MySprite(SpriteType.TEXTURE, sprite, position: AdjustToRotation(AdjustToAnchor(anchor, position, size), rotateAround, rotation), size: size, color: color, rotation: rotation);
    }
    public MySprite Text(string text, Vector2 position, float size, Color color, TextAlignment align = TextAlignment.CENTER)
    {
        position.Y -= size * 0.5f;
        size = canvasSize.Y / 30.6f * size;
        position = (center + position) * canvasSize + canvasPosition;
        return new MySprite(SpriteType.TEXT, text, position, rotation: size, color: color, alignment: align);
    }
    public float TextHeight(float scale, int lines = 1)
    {
        return lines * scale * 30.6f;
    }
    public Vector2 AdjustToAnchor(Anchor anchor, Vector2 position, Vector2 size)
    {
        switch (anchor)
        {
            case Anchor.Left:
                position.X = position.X + size.X * 0.5f;
                break;
            case Anchor.Right:
                position.X = position.X - size.X * 0.5f;
                break;
        }
        return position;
    }
    public Vector2 AdjustToRotation(Vector2 position, Vector2 rotateAround, float rotation)
    {
        var vec = Vector2.Zero;
        vec.X = (float)(Math.Cos(rotation) * (position.X - rotateAround.X) - Math.Sin(rotation) * (position.Y - rotateAround.Y) + rotateAround.X);
        vec.Y = (float)(Math.Sin(rotation) * (position.X - rotateAround.X) + Math.Cos(rotation) * (position.Y - rotateAround.Y) + rotateAround.Y);
        return vec;
    }
}
enum Anchor
{
    Center, Left, Right
}
