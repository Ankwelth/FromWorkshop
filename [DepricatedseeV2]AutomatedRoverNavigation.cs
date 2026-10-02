/*
 * R e a d m e
 * -----------
 * Haka's Automated Rovers
 * 
 * All configuration is done in the 'Custom Data' of the programmable block,
 * There is no user configurable settings in here.
 * You will want at least 1 lcd to display status information.
 * 
 * Arguments are:
 * clear
 * 	Clear the status and debug screens
 * reload
 * 	Reload config data, this will stop any active task
 * path %s
 * 	Follow a waypoint path, where %s is the name of the path to follow, paths are configured
 * 	in the Custom Data of the programable block
 * -combat
 * 	enable combat functions while executing the command, this can be added after any command
 * 	the '-' is required
 * 
 * Settings:
 * These are general settings that apply to all actions
 * all positions are based off the remote controll block
 * 
 * waypoint_range=10
 * 	How close does the rover have to get to a way point before it considers it reached
 *     only cares about distance in the plane of the rover, will use distance from the remote controll block
 * max_speed=10  
 * 	 The speed the rover will travel at unless a different speed is specified in a path, in m/s.
 * 	 The speed limit on the wheels should be higher than this (multiply by 3.6 to get speed in km/hr)
 * 	 The script will apply breaks if speed goes above 2x this value. 
 * remote_control_name=Remote Control
 * 	Name of the remote control block to use, must be exact. If it is not found the script will use a random one
 * wheel_group_name=Wheels 
 * 	Name of wheel suspension group to use, must be exact. If not found or not specified, all wheels will be used
 * 
 * debug_lcd=debug  
 * 	Name of the lcd/text panel to output debug messages to. If not found or specified,
 * 	it will use the programable block's screen. This mostly contains information about setup.
 * status_lcd=status  
 * 	Name of the lcd/text panel to output status information to. If not Found or specified,
 * 	no status messages will be written. The status will be updated every execution of the
 * 	script.
 * 
 * The next settings are for tuning speed and steering controllers, you shouldn't need to touch these,
 * but if you are getting understeer, oversteer, steering oscillations, speed oscillations etc, you might need to.
 * speed_kp=0.2     
 * 	Proportional coefficent of speed controller
 * speed_ki=0.001   
 * 	Integral coefficient of speed controller
 * speed_kd=0.00001 
 * 	Derivitive coefficient of speed controller
 * steering_kp=0.2     
 * 	Proportional coefficent of speed controller
 * steering_ki=0.0001   
 * 	Integral coefficient of speed controller
 * steering_kd=0.0 
 * 	Derivitive coefficient of speed controller
 * 	
 * [combat]
 * 	combat controls, these can be removed for civilian operation
 *     will follow the selected path if no target is available.
 *     (combat behaviors can be enabled by passing the '-combat' flag with any run cmd
 * targeting_turret=Gatling Turret  
 * 	use this turret's target for combat actions
 * target_gps=GPS:attack this waypoint:102:324:124:FF0000 
 * 	circle this waypoint, if a targeting_turret is defined, it's target will take priority
 * pattern=circle 
 * 	type of attack pattern (only circle is supported currently)
 * range=500  
 * 	how far to keep from the target. The distance is not closely maintained, so
 * 	moving targes the distance may vary considerably
 * side=right 
 * 	Which side of the vehicle to keep facing the target (right or left)"
 * 
 * Paths are defined in the config by entering waypoints under the '[path %s]' block, where %s is the path name.
 * - Waypoints are given by '%d.gps=%s' where %d is the waypoint number, and %s is the GPS point.
 * 	Waypoints do not have to be entered in order, they will be sorted before hand. If you have more than 10
 * 	waypoints, prefix with 0s so the waypoint number is always the same number of digits.
 * 	The GPS point should be copied from the GPS tab of the terminal. 
 * - A timer block can be added to a waypoint, it will be triggered when the rover gets within waypoint_range of the waypoint
 * 	timer blocks are given by '%d.tb=%s' where %d is the waypoint number, %s is the timer block name. 
 * - The maximum speed of the rover for a section of the path can be given by '%d.max_speed=%f' where %d is the waypoint
 * 	number and %f is the max speed while navagating to this waypoint. If not set for a waypoint the value in the [settings]
 * 	block will be used
 * - The distance from the waypoint to be considered reached can be given by '%d.waypoint_range=%f' where %d is the
 * 	waypoint number, and %f is the range in meters from the waypoint where it will be considered reached, note: small values may
 * 	result in the rover never reaching the waypoint
 */

/// <summary>
        ///  TODO:
        ///     use reed-shepp curves for path planning, A to B
        ///     https://github.com/nathanlct/reeds-shepp-curves/
        /// </summary>
// parsers
MyIni _ini = new MyIni();
MyCommandLine _commandLine = new MyCommandLine();

// displays
IMyTextSurface debugOut;
IMyTextPanel statusOut;

// controls
IMyRemoteControl RemoteControl;
List<IMyMotorSuspension> Wheels;

// waypoint mode
Dictionary<string, List<waypoint>> paths = new Dictionary<string, List<waypoint>>();
List<waypoint> path;
int waypointIndex;
string pathName;

// driving settings
double turningRadius;
Dictionary<long, float> steerMult = new Dictionary<long, float>();
Dictionary<long, float> propMult = new Dictionary<long, float>();
double maxSpeed = 10;
double waypointRange = 10;
string RemoteControlName = "Remote Control";
PidController speedPID = new PidController(1, 0.1, 0.001, 1, -1);
PidController steeringPID = new PidController(1, 0.1, 0.001, 1, -1);

// target control
bool offensive = false;
IMyLargeTurretBase targetDesignator;
MyWaypointInfo targetGPS;
double targetRange;
bool evasive;
Base6Directions.Direction side;
// example path
string pathTemplate = @"
[path base]
0.gps=GPS:base far approach:94613.0475387743:149839.993925089:5776198.89649993:#FF75C9F1:
0.max_speed=20
1.gps=GPS:base near approach:94655.313392382:149944.092998252:5776195.24110773:#FF75C9F1:
2.gps=GPS:base unload:94672.9570207523:149983.455598524:5776197.91306437:#FF75C9F1:
2.max_speed=2
2.waypoint_range=0.2
2.tb=TB Start Timeout
";
// default settings
string settingsTemplate = @"
[settings]
waypoint_range=10
max_speed=10
remote_control_name=Remote Control
wheel_group_name=Wheels
debug_lcd=LCD Debug
status_lcd=LCD Status
speed_kp=0.2
speed_ki=0.001
speed_kd=0.00001
steering_kp=0.2
steering_ki=0.0001
steering_kd=0.0
";
//default settings/example
string combatTemplate = @"
[combat]
targeting_turret=Gatling Turret
target_gps=GPS:attack this waypoint:102:324:124:FF0000
pattern=circle
range=500
side=right
";

public Program()
{
    if (Me.CustomData == "")
    {
        Me.CustomData = settingsTemplate + pathTemplate + combatTemplate;
    }

    Wheels = new List<IMyMotorSuspension>();
    Load();
    refreshConfig();
}
public void Load()
{
    _ini.TryParse(Storage);

    pathName = _ini.Get("waypoints", "pathID").ToString("");
    waypointIndex = _ini.Get("waypoints", "waypointIndex").ToInt32(0);
}
public void refreshConfig()
{
    // Call the TryParse method on the custom data. This method will
    // return false if the source wasn't compatible with the parser.
    MyIniParseResult result;
    if (!_ini.TryParse(Me.CustomData, out result))
    {
        debugOut = Me.GetSurface(0);
        debugOut.ContentType = ContentType.TEXT_AND_IMAGE;
        debugOut.Font = "DEBUG";
        debugl("error parsing config: " + result.ToString());
    }

    /*
                    [settings]
                    waypoint_range=10 ; how close to a waypoint do we need to get to count as being reached
                    max_speed=5 ; apply breaks above this speed
                    remote_control_name=remote control ; name of the remote control
                    wheel_group_name=Wheels ; wheels group must be exact
                    kp=1     ; Proportional coefficent of speed controller
                    ki=0.1   ; integral coefficient of speed controller
                    kd=0.001 ; derivitive coefficient of speed controller
                    */
    waypointRange = _ini.Get("settings", "waypoint_range").ToDouble();
    if (waypointRange < 5) { waypointRange = 5; }
    maxSpeed = _ini.Get("settings", "max_speed").ToDouble();
    if (maxSpeed < 1) { maxSpeed = 10; }
    double kp = _ini.Get("settings", "speed_kp").ToDouble();
    double ki = _ini.Get("settings", "speed_ki").ToDouble();
    double kd = _ini.Get("settings", "speed_kd").ToDouble();
    speedPID.kp = kp;
    speedPID.ki = ki;
    speedPID.kd = kd;
    speedPID.ClearAccumulator();
    kp = _ini.Get("settings", "steering_kp").ToDouble();
    ki = _ini.Get("settings", "steering_ki").ToDouble();
    kd = _ini.Get("settings", "steering_kd").ToDouble();
    steeringPID.kp = kp;
    steeringPID.ki = ki;
    steeringPID.kd = kd;
    steeringPID.ClearAccumulator();

    RemoteControlName = _ini.Get("settings", "remote_control_name").ToString();

    string wheelsGroupName = _ini.Get("settings", "wheel_group_name").ToString();

    string debugLCDName = _ini.Get("settings", "debug_lcd").ToString();
    List<IMyTextPanel> lcds = new List<IMyTextPanel>();
    IMyTextPanel lcd = GridTerminalSystem.GetBlockWithName(debugLCDName) as IMyTextPanel;
    if (lcd != null)
    {
        debugOut = lcd;
        debugOut.ContentType = ContentType.TEXT_AND_IMAGE;
        debugOut.Font = "DEBUG";
    }
    else
    {
        debugOut = Me.GetSurface(0);
        debugOut.ContentType = ContentType.TEXT_AND_IMAGE;
        debugOut.Font = "DEBUG";
    }
    string statusLCDName = _ini.Get("settings", "status_lcd").ToString();
    lcd = GridTerminalSystem.GetBlockWithName(statusLCDName) as IMyTextPanel;
    if (lcd != null)
    {
        statusOut = lcd;
        statusOut.ContentType = ContentType.TEXT_AND_IMAGE;
        statusOut.Font = "DEBUG";
    }
    RemoteControl = GridTerminalSystem.GetBlockWithName(RemoteControlName) as IMyRemoteControl;
    if (RemoteControl == null)
    {
        List<IMyRemoteControl> remotes = new List<IMyRemoteControl>();
        GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(remotes, r => !r.CustomName.Contains("IGNORE"));
        if (remotes.Count > 0) RemoteControl = remotes[0];
        if (RemoteControl == null)
        {
            throw new Exception("no RemoteControls found");
        }
        debugl("could not find Remote Control named: '" + RemoteControlName + "'; using remote '" + RemoteControl.CustomName + "' instead");
    }

    IMyBlockGroup wheelGroup = GridTerminalSystem.GetBlockGroupWithName(wheelsGroupName);
    if (wheelGroup != null)
    {
        wheelGroup.GetBlocksOfType(Wheels);
    }
    else
    {
        GridTerminalSystem.GetBlocksOfType<IMyMotorSuspension>(Wheels);
        debugl("could not find group '" + wheelsGroupName + "'; using all wheels");
    }
    /*
                     * [path 1]
                     * 1.gps=GPS:102:324:124: // location
                     * 1.tb=Timer Block 1
                     * 2.gps=GPS:102:324:124:
                     * 3.gps=GPS:108:34:1114:
                     * 3.waypoint_range=1
                     * 4.gps=GPS:120:324:184:
                     * 4.tb=Timer Block 4
                     */
    paths.Clear();
    List<string> sections = new List<string>();
    _ini.GetSections(sections);
    foreach (string section in sections)
    {
        // skip anything that isn't a path
        if (!section.StartsWith("path")) { continue; }
        string pathName = section.Substring("path ".Length);
        List<waypoint> waypoints = new List<waypoint>();

        List<MyIniKey> keys = new List<MyIniKey>(); _ini.GetKeys(section, keys);
        keys.Sort((MyIniKey a, MyIniKey b) => a.Name.CompareTo(b.Name));
        int index = -1;
        string n = "";
        MyWaypointInfo gps = new MyWaypointInfo();
        IMyTimerBlock tb = null;
        double wpMaxSpeed = 0;
        double wpRange = 0;
        foreach (var item in keys)
        {
            if (!item.Name.StartsWith(n))
            {
                if (!gps.IsEmpty())
                {
                    debugl(pathName + " adding waypoint: " + gps.Name + ", path length: " + waypoints.Count());
                    if (tb != null) { debugl("tb: " + tb.CustomName); }
                    waypoints.Add(new waypoint(gps, tb, wpMaxSpeed, wpRange));
                    index++;
                }
                tb = null;
                wpMaxSpeed = 0;
                wpRange = 0;
                gps = new MyWaypointInfo();
            }
            var parts = item.Name.Split('.');
            n = parts[0];
            switch (parts[1])
            {
                case "gps":
                    gps = ParseGPS(_ini.Get(section, item.Name).ToString());
                    break;
                case "tb":
                    tb = GridTerminalSystem.GetBlockWithName(_ini.Get(section, item.Name).ToString()) as IMyTimerBlock;
                    break;
                case "max_speed":
                    wpMaxSpeed = _ini.Get(section, item.Name).ToDouble();
                    break;
                case "waypoint_range":
                    wpRange = _ini.Get(section, item.Name).ToDouble();
                    break;
            }
        }
        if (!gps.IsEmpty())
        {
            {
                debugl(pathName + " adding waypoint: " + gps.Name + ", path length: " + waypoints.Count());
                if (tb != null) { debugl("tb: " + tb.CustomName); }
                waypoints.Add(new waypoint(gps, tb, wpMaxSpeed, wpRange));
            }
        }
        if (waypoints.Count() == 0)
        {
            break;
        }
        debugl("adding path");
        paths.Add(pathName, waypoints);
    }
    /*
            * combat controls:
                [combat]
                targeting_turret=Gatling Turret ; what block is giving us targets (must be a turret)
                target_gps=GPS:102:324:124: ; GPS position to attack, can be used instead of target designator
                pattern=circle ; circles the target at range
                range=500      ; range to keep from target
                evasive=true   ; wobble in and out to dodge
                side=right     ; what side to keep to the enemy, left or right, this is also the break off side
            */
    string gpsString = _ini.Get("combat", "target_gps").ToString();
    if (gpsString != "")
    {
        targetGPS = ParseGPS(gpsString);
    }
    targetRange = _ini.Get("combat", "range").ToDouble();
    evasive = _ini.Get("combat", "evasive").ToBoolean();
    string sides = _ini.Get("combat", "side").ToString();
    switch (sides)
    {
        case "left":
            side = Base6Directions.Direction.Left;
            break;
        case "right":
            side = Base6Directions.Direction.Right;
            break;
    }

    string targetDesignatorName = _ini.Get("combat", "targeting_turret").ToString();
    IMyLargeTurretBase turret = GridTerminalSystem.GetBlockWithName(targetDesignatorName) as IMyLargeTurretBase;
    if (turret != null)
    {
        targetDesignator = turret;
        debugl("got target designator: " + targetDesignator.CustomName);
    }
    else
    {
        debugl("could not find turret named: '" + targetDesignatorName + "'");
    }

    double wheelbase, frontPos=0, BackPos = 0;
    double maxTurnAngel = 0;
    Vector3D wheelPositionSum = Vector3D.Zero;
    Vector3D Center = RemoteControl.CenterOfMass;
    debugl("center: "+vectorToString(Center)+ "; remote: "+vectorToString(RemoteControl.WorldMatrix.Translation));
    foreach (IMyMotorSuspension w in Wheels)
    {
        if (!GridTerminalSystem.CanAccess(w)) continue;
        wheelPositionSum += w.GetPosition();
        if (w.MaxSteerAngle > maxTurnAngel)
        {
            maxTurnAngel = w.MaxSteerAngle;
        }
        // get the "forward" coordinate relative to the Remote control
        //Math.Sign(Vector3D.Dot(wheel.GetPosition() - avgWheelPosition, referenceController.WorldMatrix.Forward))
        double d = Vector3D.Dot(w.GetPosition() - Center,RemoteControl.WorldMatrix.Forward);
        if (d > frontPos)
        {
            frontPos = d;
        }
        else if ((d) < BackPos)
        {
            BackPos = d ;
        }
        float end = -Math.Sign(d);
        if (d == 0) d = 1;
        debugl(w.CustomName + " d: " + d);
        // rear wheels need to have their steering flipped still doesn't work :/
        steerMult[w.EntityId] = Math.Sign(Vector3D.Dot(w.WorldMatrix.Forward, RemoteControl.WorldMatrix.Up)) * end;
        // right wheels need propultion flipped
        propMult[w.EntityId] = Math.Sign(Vector3D.Dot(w.WorldMatrix.Up, RemoteControl.WorldMatrix.Left));
        debugl("    steerMult: " + steerMult[w.EntityId]);
        debugl("    propMult: " + propMult[w.EntityId]);
    }
    wheelbase = frontPos - BackPos;
    turningRadius = wheelbase / Math.Tan(maxTurnAngel);
    debugl("Turning Radius: " + turningRadius);
}

public void Save()
{
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means.
    //
    // This method is optional and can be removed if not
    // needed.

    _ini.TryParse(Storage);
    _ini.Set("waypoints", "pathID", pathName);
    _ini.Set("waypoints", "waypointIndex", waypointIndex);
}

public void Main(string argument, UpdateType updateSource)
{
    // The main entry point of the script, invoked every time
    // one of the programmable block's Run actions are invoked,
    // or the script updates itself. The updateSource argument
    // describes where the update came from. Be aware that the
    // updateSource is a  bitfield  and might contain more than
    // one update type.
    //
    // The method itself is required, but the arguments above
    // can be removed if not needed.
    if (_commandLine.TryParse(argument))
    {

        if (_commandLine.Switch("Reload") || _commandLine.Switch("reload") || _commandLine.Argument(0).ToLower() == "reload")
        {
            refreshConfig();
            Runtime.UpdateFrequency = UpdateFrequency.None;
            path = null;
            RemoteControl.HandBrake = true;
            speedPID.ClearAccumulator();
            steeringPID.ClearAccumulator();
            waypointIndex = 0;
            SetWheels(0, 0);
            return;

        }
        else if (_commandLine.Switch("Clear") || _commandLine.Switch("clear") || _commandLine.Argument(0).ToLower() == "clear")
        {
            debug("", false);
            writeStatus("", false);
            return;
        }
        else if (_commandLine.Switch("Stop") || _commandLine.Switch("stop") || _commandLine.Argument(0).ToLower() == "stop")
        {
            Runtime.UpdateFrequency = UpdateFrequency.None;
            path = null;
            speedPID.ClearAccumulator();
            steeringPID.ClearAccumulator();
            waypointIndex = 0;
            SetWheels(0, 0);
            return;
        }
        else if (_commandLine.Argument(0).ToLower() == "path")
        {
            // starting a waypoint list
            pathName = _commandLine.Argument(1);
            try
            {
                path = paths[pathName];
            }
            catch
            {
                writeStatus("Path " + pathName + " not defined", false);
                SetWheels(0, 0);
                RemoteControl.HandBrake = true;
                speedPID.ClearAccumulator();
                steeringPID.ClearAccumulator();
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;

            }
            if (path == null || path.Count() == 0)
            {
                writeStatus("Path " + pathName + " not defined", false);
                SetWheels(0, 0);
                RemoteControl.HandBrake = true;
                speedPID.ClearAccumulator();
                steeringPID.ClearAccumulator();
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }
        }
    }
    //debugl("running: total Distance: "+distance+"m");
    switch (updateSource)
    {
        case UpdateType.Update100:
        case UpdateType.Update10:
        case UpdateType.Update1:
            MatrixD m = RemoteControl.WorldMatrix;
            Vector2D v;
            double speedLimit = 0;
            if (offensive)
            {
                if (targetDesignator != null && !targetDesignator.Closed && targetDesignator.IsWorking && targetDesignator.HasTarget)
                {
                    var target = targetDesignator.GetTargetedEntity();
                    CircleTarget(target, m, out v);
                }
                else if (!targetGPS.IsEmpty())
                {
                    CircleGPS(m, out v);
                }
                else
                {
                    WaypointMission(m, out v, out speedLimit);
                }
            }
            else
            {
                WaypointMission(m, out v, out speedLimit);
            }
            if (v.Equals(Vector2D.Zero))
            {
                RemoteControl.HandBrake = true;
                Runtime.UpdateFrequency = UpdateFrequency.None;
                SetWheels(0, 0);
                return;
            }
            else
            {
                RemoteControl.HandBrake = false;
            }
            if (speedLimit == 0) speedLimit = maxSpeed;

            Vector3D velocity = RemoteControl.GetShipVelocities().LinearVelocity;
            Vector3D vel = Vector3D.TransformNormal(velocity, MatrixD.Transpose(m));
            driveToPoint(v, speedLimit, -vel);
            if (velocity.Length() > 2 * speedLimit)
            {
                RemoteControl.HandBrake = true;
            }
            break;
        default:
            offensive = _commandLine.Switch("combat");
            waypointIndex = 0;
            break;
    }
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
public void CircleTarget(MyDetectedEntityInfo target, MatrixD m, out Vector2D v)
{
    statusl("Targeting object: " + target.Name, false);
    Vector3D d = (m.Translation - target.Position).Normalized();
    var b = d * targetRange + target.Position;
    switch (side)
    {
        case Base6Directions.Direction.Left:
            b -= d.Cross(m.Up) * targetRange;
            break;
        case Base6Directions.Direction.Right:
            b += d.Normalized().Cross(m.Up) * targetRange;
            break;
    }
    v = WorldToLocal(b, m);
    statusl("Goal point: <" + v.X.ToString("0.00") + "," + v.Y.ToString("0.00") + ">");
}
public void CircleGPS(MatrixD m, out Vector2D v)
{
    statusl("Targeting GPS: " + targetGPS.Name, false);
    Vector3D d = (m.Translation - targetGPS.Coords).Normalized();
    var b = d * targetRange + targetGPS.Coords;
    switch (side)
    {
        case Base6Directions.Direction.Left:
            b -= d.Normalized().Cross(m.Up) * targetRange;
            break;
        case Base6Directions.Direction.Right:
            b += d.Normalized().Cross(m.Up) * targetRange;
            break;
    }
    v = WorldToLocal(b, m);
    statusl("Goal point: <" + v.X.ToString("0.00") + "," + v.Y.ToString("0.00") + ">");
}
public void WaypointMission(MatrixD m, out Vector2D v, out double speedLimit)
{
    if (path == null || waypointIndex >= path.Count())
    {
        debugl("done");
        writeStatus("Path complete", false);
        path = null;
        speedPID.ClearAccumulator();
        steeringPID.ClearAccumulator();
        waypointIndex = 0;
        speedLimit = 0;
        v = Vector2D.Zero;
        return;
    }
    statusl("Path: " + pathName,false);
    waypoint w = path[waypointIndex];
    v = WorldToLocal(w.info.Coords, m);
    statusl("Going to waypoint:" + path[waypointIndex].info.Name);
    statusl("Range: " + v.Length().ToString("0.00") + "m");
    double range = w.distance;
    speedLimit = w.maxSpeed;
    if (range <= 0)
    {
        range = waypointRange * waypointRange;
    }
    if (v.LengthSquared() <= range)
    {
        debugl("reached Waypoint " + w.info.Name);
        speedPID.ClearAccumulator();
        steeringPID.ClearAccumulator();
        if (w.tb != null)
        {
            debugl("triggered tb: " + w.tb.CustomName);
            w.tb.Trigger();
        }
        waypointIndex++;
        return;
    }
}
public void driveToPoint(Vector2D v, double maxSpeed, Vector3D velocity)
{
    double speed = velocity.Length();
    statusl("Velocity: " + vectorToString(velocity));
    // we negate v.Y since positive steer is in the -Y direction
    float steer = Math.Sign(v.Y);
    float prop = (float)Clamp(-1, 1, v.X);
    statusl("Waypoint relative position: <" + v.X.ToString("0.00") + "," + v.Y.ToString("0.00") + ">");
    double dist2 = v.LengthSquared();
    double tr2 = turningRadius * turningRadius;
    if (v.Y * v.Y > v.X * v.X + velocity.Z)
    {
        if (dist2 > tr2)
        {
            // goal is perpendicular to our movement direction
            // if we move it will change from infront to behind quickly
            // and we will shift back and fourth often.
            // just choose a side and steer for it
            statusl("Waypoint beside rover, outdside steering radius");
            prop = 1;
        }
        else
        {
            // we are too close to steer directly into the goal, so back off
            statusl("Waypoint beside rover, inside steering radius");
            prop = -1;
        }
    }
    else if (v.X < 0)
    {
        // goal is behind steering should be maxed
        statusl("Waypoint behind rover");
        prop = -1;
    }
    else
    {
        var l = Math.Max(1, (dist2) / tr2);
        statusl("Waypoint ahead of rover");
        steer = (float)steeringPID.eval(v.Y / l);
    }
    prop = (float)speedPID.eval((maxSpeed * prop) - (speed * Math.Sign(velocity.Z)));
    statusl("Propulsion: " + prop.ToString("0.00") + "%");
    steer *= (float)Math.Sign(velocity.Z);
    statusl("Steering: " + steer.ToString("0.00")+"%");
    SetWheels(prop, steer);
}
public static double Clamp(double min, double max, double value)
{
    return Math.Min(Math.Max(min, value), max);
}
public void SetWheels(float propulsion, float steering)
{
    foreach (var w in Wheels)
    {
        if (!GridTerminalSystem.CanAccess(w)) { continue; }
        if (w.Steering)
        {
            w.SteeringOverride = steering * steerMult[w.EntityId];
        }
        w.PropulsionOverride = propulsion * propMult[w.EntityId];
    }
}
public static Vector2D WorldToLocal(Vector3D worldPos, MatrixD WorldMatrix)
{
    Vector3D d = (worldPos - WorldMatrix.Translation);
    return new Vector2D(d.Dot(WorldMatrix.Forward), d.Dot(WorldMatrix.Left));

}
public MyWaypointInfo ParseGPS(string gps)
{
    debugl("parsing gps " + gps);
    MyWaypointInfo info = new MyWaypointInfo();
    // extracts only the GPS data
    var parts = gps.Split(':');
    if (parts.Length < 6)
    {
        debugl("invalid gps " + gps);
        return info;
    }
    int startofGPS = 0;
    for (int i = 0; i < parts.Length; i++)
    {
        if (parts[i].EndsWith("GPS"))
        {
            startofGPS = i;
            break;
        }
    }
    //GPS format for MyWaypointInfo.TryParse: 'GPS:foo:-253371.035481119:-2426168.99301961:385974.271284248:'
    debugl("start of GPS:" + startofGPS);
    debugl("number of parts: " + parts.Count());
    gps = String.Format("GPS:{0}:{1}:{2}:{3}:", parts[startofGPS + 1], parts[startofGPS + 2], parts[startofGPS + 3], parts[startofGPS + 4]);
    if (!MyWaypointInfo.TryParse(gps, out info))
    {
        debugl("could not parse GPS coordinate: '" + gps + "'");
        return new MyWaypointInfo();
    };
    return info;
}
public static string vectorToString(Vector3D v)
{
    return "<"+v.X.ToString("0.00") + ", " + v.Y.ToString("0.00") + ", " + v.Z.ToString("0.00") +">";
}
public void debug(string s, bool append = true)
{
    if (debugOut != null)
    {
        debugOut.WriteText(s, append);
    }
    else { Echo("debug lcd not found"); }
}
public void debugl(string s, bool append = true)
{
    debug(s + "\r\n", append);
}
public void writeStatus(string s, bool append = true)
{
    if (statusOut != null)
    {
        statusOut.WriteText(s, append);
    }
    else { Echo("status lcd not found"); }
}
public void statusl(string s, bool append = true)
{
    writeStatus(s + "\r\n", append);
}
public class waypoint
{
    public MyWaypointInfo info;
    public IMyTimerBlock tb;
    public double maxSpeed;
    public double distance;
    public waypoint(MyWaypointInfo i, IMyTimerBlock tb)
    {
        this.info = i;
        this.tb = tb;
    }
    public waypoint(MyWaypointInfo i, IMyTimerBlock tb, double maxSpeed)
    {
        this.info = i;
        this.tb = tb;
        this.maxSpeed = maxSpeed;
    }
    public waypoint(MyWaypointInfo i, IMyTimerBlock tb, double maxSpeed, double waypointDistance)
    {
        this.info = i;
        this.tb = tb;
        this.maxSpeed = maxSpeed;
        this.distance = waypointDistance;
    }
}
public class PidController
{
    public double kp;
    public double kd;
    public double ki;
    public double max;
    public double min;
    double i;
    double last;
    public PidController(double kp, double kd, double ki, double maxOutput, double minOutput)
    {
        this.kp = kp;
        this.kd = kd;
        this.ki = ki;
        this.max = maxOutput;
        this.min = minOutput;
    }
    public double eval(double err)
    {
        double d = err - last;
        last = err;
        i += err * kd;
        return Clamp(min, max, kp * err + i + kd * d);
    }
    public void ClearAccumulator()
    {
        i = 0;
    }
}