


//Targeter block names can be used any terminal block
//group with all rocket stuff should have name like PB they using
// WAR_TOP down etc must by around gyro which terminal is facing WAR TOP they can be any block
// rear engines should stabilize rocket front engine isn necessary
// if you configure everyghink just put gps coords {X:Y:Z:} and watch
string TargeterNameTop = "WAR_TOP";
string TargeterNameDown = "WAR_DOWN";
string TargeterNameRight = "WAR_RIGHT";
string TargeterNameLeft = "WAR_LEFT";
string BackThrusterName = "Thruster_BACK";
 // terminal of gyros must be up... you know what i mean right?
int FlyingUpCounter = 500; // how long rocket will fly up 1=16ms

/////// Dont edit anything below /////

AutomaticTargeter targeter;
bool triggered = false;
IMyBlockGroup mainGroup;

IMyThrust thruster;
List<IMyWarhead> Warheads = new List<IMyWarhead>();
List<IMyShipMergeBlock> mergeBlocks = new List<IMyShipMergeBlock>();
List<IMyGyro> gyros = new List<IMyGyro>();

enum Action { Idle,FlyingUp,Targeting,FinalFlyToTarget };
Action CurrentTask = Action.Idle;

public Program()
{
    mainGroup = GridTerminalSystem.GetBlockGroupWithName(Me.CustomName); if(mainGroup == null) { Echo("Cant find group"); return; }
    mainGroup.GetBlocksOfType(Warheads);
    mainGroup.GetBlocksOfType(mergeBlocks);
    mainGroup.GetBlocksOfType(gyros);

    List<IMyThrust> thrusters = new List<IMyThrust>();
    mainGroup.GetBlocksOfType(thrusters);

    foreach(var thrust in thrusters)
    {
        if(thrust.CustomName == BackThrusterName ) { thruster = thrust; break; }
    }
    if(thruster == null) { Echo("Cannot find back thruster"); return; }

    targeter = new AutomaticTargeter(mainGroup, TargeterNameTop, TargeterNameDown, TargeterNameRight, TargeterNameLeft);
    if (targeter.errorWhileCreating) { Echo("error while creating targeter object"); return; }
}

public void Main(string argument, UpdateType updateSource)
{
    if (argument.Contains("{X:"))
    {
        if (targeter.SetTarget(argument)) { Echo("Gps Error"); return; }

        foreach (var gyro in gyros)
        {
            gyro.GyroOverride = true;
        }

        thruster.ThrustOverridePercentage = 1;

        foreach (var mergeblock in mergeBlocks)
        {
            mergeblock.Enabled = false;
        }

        triggered = true;
        CurrentTask = Action.FlyingUp;
        Runtime.UpdateFrequency = UpdateFrequency.Update1;
    }
    if (triggered == false) { return; }
    if(CurrentTask == Action.FlyingUp)
    {
        FlyingUpCounter--;
        if(FlyingUpCounter<=0)
        {
            thruster.ThrustOverridePercentage = 0;
            targeter.TryToLock();
            CurrentTask = Action.Targeting;
        }

        return;
    }

    if (CurrentTask == Action.Targeting)
    {
        targeter.TryToLock();
        if (!targeter.LockingInProgress) { CurrentTask = Action.FinalFlyToTarget; return; }
    }

    if (CurrentTask == Action.FinalFlyToTarget)
    {
        thruster.ThrustOverridePercentage = 1;
        foreach(var warhead in Warheads)
        {
            warhead.IsArmed = true;
        }
        Runtime.UpdateFrequency = UpdateFrequency.None;
    }



}

//Targeter Class
public class AutomaticTargeter
{
    Vector3D TargetVector;
    float GyroPrecision = (float)0.01; // max precision is 0.0001
    float GpsPrecision = (float)0.01; // max 0.001

    IMyTerminalBlock TargeterTop;
    IMyTerminalBlock TargeterDown;
    IMyTerminalBlock TargeterRight;
    IMyTerminalBlock TargeterLeft;

    List<IMyGyro> TargeterGyros = new List<IMyGyro>();
    IMyGyro gyr; // used to prevent horizontal and vertical reseting
    enum Side { NONE, UP, DOWN, LEFT, RIGHT };

    int ticksLeftToEnsure = 80;

    public bool LockingInProgress { get; private set; } = false;
    public bool GpsError { get; private set; } = true;
    public bool errorWhileCreating { get; private set; } = true;

    public string GetDisplayText()
    {
        string text = "Target Distance:" + AvargeDistanceToTarget().ToString() + " m" + '\n'
        + "Targeter status: "; if (LockingInProgress) { text += "Locking"; } else { text += "Idle"; }
        text += '\n' + "Precision: " + GyroPrecision.ToString() + '/' + GpsPrecision.ToString();
        return text;
    }
    //Constructor
    public AutomaticTargeter(IMyBlockGroup terminalgroup, string TargeterNameTop, string TargeterNameDown, string TargeterNameRight, string TargeterNameLeft) //modified
    {
        terminalgroup.GetBlocksOfType(TargeterGyros);
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        terminalgroup.GetBlocksOfType(blocks);
        foreach(var block in blocks)
        {
            if (block.CustomName == TargeterNameTop) { TargeterTop = block;} else
            if (block.CustomName == TargeterNameDown) { TargeterDown = block; } else
                if(block.CustomName == TargeterNameLeft) { TargeterLeft = block; } else
                if(block.CustomName == TargeterNameRight) { TargeterRight = block; }
        }

        if (TargeterTop == null || TargeterDown == null || TargeterRight == null || TargeterLeft == null || TargeterGyros.Count == 0) { return; }
        gyr = TargeterGyros[0];
        errorWhileCreating = false;
        StopLocking();
    }

    public bool SetTarget(string GpsCoords)
    {
        GpsError = !Vector3D.TryParse(GpsCoords, out TargetVector);
        return GpsError;
    }
    public void TryToLock()
    {
        if (GpsError) { return; }
        LockingInProgress = true;
        // modified to work with rocket
       GyroPrecision = (float)1; GpsPrecision = (float)10;
       if (!AimToTarget()) { return; }

       GyroPrecision = (float)1; GpsPrecision = (float)5;
       if (!AimToTarget()) { return; }

        GyroPrecision = (float)0.5; GpsPrecision = (float)1;
        if (!AimToTarget()) { return; }

        GyroPrecision = (float)0.25; GpsPrecision = (float)0.5;
        if (!AimToTarget()) { return; }

        GyroPrecision = (float)0.05; GpsPrecision = (float)0.1;
        if (!AimToTarget()) { return; }

        GyroPrecision = (float)0.025; GpsPrecision = (float)0.05;
        if (!AimToTarget()) { return; }

        GyroPrecision = (float)0.001; GpsPrecision = (float)0.001; // didnt work if under 0.001
        AimToTarget();
        if (ticksLeftToEnsure > 0) { ticksLeftToEnsure--; return; }

        StopLocking();
    }
    public void StopLocking()
    {
        ControlGyros(true, 0, 0, 0);
        ticksLeftToEnsure = 80;
        LockingInProgress = false;
    }
    ////////////
    double AvargeDistanceToTarget()
    {
        return Math.Round((DistanceToTarget(TargeterTop) + DistanceToTarget(TargeterDown) + DistanceToTarget(TargeterLeft) + DistanceToTarget(TargeterRight)) / 4) * 1;
    }
    void GyroTurn(Side side) // gyro must be terminal facing up
    {
        switch (side)
        {
            case Side.UP: ControlGyros(true, -GyroPrecision, gyr.Roll, gyr.Yaw); break;
            case Side.DOWN: ControlGyros(true, GyroPrecision, gyr.Roll, gyr.Yaw); break;
            case Side.LEFT: ControlGyros(true, gyr.Pitch, GyroPrecision, gyr.Yaw); break;
            case Side.RIGHT: ControlGyros(true, gyr.Pitch, -GyroPrecision, gyr.Yaw); break;
            case Side.NONE: ControlGyros(true, 0, 0, 0); break;
        }
    }
    bool AimToTarget()
    {
        TargetHorizontal();
        TargetVertical();
        if (TargetHorizontal() && TargetVertical()) { return true; }
        return false;
    }
    bool TargetHorizontal() //roll return true if targeted;
    {
        if (DistanceToTarget(TargeterLeft) > DistanceToTarget(TargeterRight)) { GyroTurn(Side.RIGHT); }
        if (DistanceToTarget(TargeterLeft) < DistanceToTarget(TargeterRight)) { GyroTurn(Side.LEFT); }
        if (DistanceToTarget(TargeterLeft) == DistanceToTarget(TargeterRight)) { GyroTurn(Side.NONE); return true; }
        return false;
    }
    bool TargetVertical() //pitch return true if targeted;
    {
        if (DistanceToTarget(TargeterTop) > DistanceToTarget(TargeterDown)) { GyroTurn(Side.DOWN); }
        if (DistanceToTarget(TargeterTop) < DistanceToTarget(TargeterDown)) { GyroTurn(Side.UP); }
        if (DistanceToTarget(TargeterTop) == DistanceToTarget(TargeterDown)) { GyroTurn(Side.NONE); return true; }
        return false;
    }

    double DistanceToTarget(IMyTerminalBlock terminalBlock) // get rounded distance to target
    {
        return Math.Round(Vector3D.Distance(terminalBlock.GetPosition(), TargetVector) / GpsPrecision) * GpsPrecision;
    }
    void ControlGyros(bool OverRide, float Pitch, float Roll, float Yaw)
    {
        foreach (IMyGyro gyro in TargeterGyros)
        {
            gyro.GyroOverride = OverRide;
            gyro.Pitch = Pitch;
            gyro.Roll = Roll;
            gyro.Yaw = Yaw;
        }
    }
}