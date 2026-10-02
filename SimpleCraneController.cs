string masterTag = "[Crane]";
IMyShipController remoteControl;

float headPistonSpeed = 0f;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

void FindRemoteControl()
{
    List<IMyShipController> controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, c => c.CustomName.Contains(masterTag) && c is IMyRemoteControl);
    if (controllers.Count > 0) { remoteControl = controllers[0]; return; }
    
    GridTerminalSystem.GetBlocksOfType(controllers, c => c.CustomName.Contains(masterTag));
    if (controllers.Count > 0) { remoteControl = controllers[0]; return; }
}

public void Main(string argument, UpdateType updateSource)
{
    FindRemoteControl();

    if (!string.IsNullOrEmpty(argument))
    {
        string arg = argument.ToLower();
        if (arg == "headpiston_extend") headPistonSpeed = 0.3f;
        else if (arg == "headpiston_retract") headPistonSpeed = -0.3f;
    }

    float rotSpeed = 0f;
    float liftSpeed = 0f;
    float middleSpeed = 0f;
    float headHingeSpeed = 0f;
    float mainPistonSpeed = 0f;
    float headPistonSpeedInput = headPistonSpeed;

    if (remoteControl != null)
    {
        // A / D -> Base Rotation
        float moveX = (float)remoteControl.MoveIndicator.X;
        if (moveX > 0f) rotSpeed = 1.0f;
        else if (moveX < 0f) rotSpeed = -1.0f;

        // W / S -> Main Lift Boom
        float moveZ = (float)remoteControl.MoveIndicator.Z;
        if (moveZ > 0f) liftSpeed = 1.0f;
        else if (moveZ < 0f) liftSpeed = -1.0f;

        // Space / C -> Main Pistons
        float moveY = (float)remoteControl.MoveIndicator.Y;
        if (moveY > 0f) mainPistonSpeed = 0.2f;
        else if (moveY < 0f) mainPistonSpeed = -0.2f;

        // Q / E -> Head Hinge Tilt
        float roll = (float)remoteControl.RollIndicator;
        if (roll > 0f) headHingeSpeed = 1.0f;
        else if (roll < 0f) headHingeSpeed = -1.0f;

        // Mouse Look Left / Right -> Middle Hinge
        float yaw = (float)remoteControl.RotationIndicator.Y;
        if (yaw > 0f) middleSpeed = 1.0f;
        else if (yaw < 0f) middleSpeed = -1.0f;

        // Mouse Look Up / Down -> Head Pistons
        float pitch = (float)remoteControl.RotationIndicator.X;
        if (pitch > 0f) headPistonSpeed = 0.2f;
        else if (pitch < 0f) headPistonSpeed = -0.2f;
    }
    else
    {
        headPistonSpeed = headPistonSpeedInput;
    }

    // Apply to all blocks containing [Crane]
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, b => b.CustomName.Contains(masterTag));

    foreach (var block in blocks)
    {
        string name = block.CustomName.ToLower();

        var motor = block as IMyMotorStator;
        if (motor != null)
        {
            motor.Enabled = true;
            motor.BrakingTorque = 100000000f; 
            motor.Torque = 100000000f;

            if (name.Contains("base") || name.Contains("rotate"))
                motor.TargetVelocityRPM = rotSpeed;
            else if (name.Contains("lift") || name.Contains("boom"))
                motor.TargetVelocityRPM = liftSpeed;
            else if (name.Contains("middle"))
                motor.TargetVelocityRPM = middleSpeed;
            else if (name.Contains("head") && (name.Contains("hinge") || name.Contains("tilt")))
                motor.TargetVelocityRPM = headHingeSpeed;
        }

        var piston = block as IMyPistonBase;
        if (piston != null)
        {
            piston.Enabled = true;
            
            if (name.Contains("head") && name.Contains("piston"))
            {
                piston.Velocity = headPistonSpeed;
            }
            else if (name.Contains("piston"))
            {
                piston.Velocity = mainPistonSpeed;
            }
        }
    }
}