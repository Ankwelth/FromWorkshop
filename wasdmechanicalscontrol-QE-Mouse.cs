/* * CONTROL INSTRUCTIONS:
 * 1. Create block groups or rename individual blocks as named in the strings below.
 * 2. Add the word "invert" to a block's "Custom Data" to flip its direction.
 * 3. Use the argument "refresh" in the programmable block to reload the block lists.
 */

// --- GROUP & KEY BINDINGS SETTINGS ---
string cockpitName              = "Main Cockpit"; 

string forward_WS_Group   = "Move Forward";   // Keys: W / S
string side_AD_Group          = "Move Side";          // Keys: A / D
string up_SpaceC_Group     = "Move Up";             // Keys: Space / C
string roll_QE_Group            = "Move Roll";           // Keys: Q / E
string pitch_MouseUpDn     = "Move Pitch";         // Mouse: Up / Down
string yaw_MouseLftRt       = "Move Yaw";          // Mouse: Left / Right

// Speed Coefficients
float pistonSpeed = 3.0f; 
float rotorSpeed  = 2.0f; 
float mouseSensitivity = 0.5f; 

// Block Caching
List<IMyTerminalBlock> forwardBlocks = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> sideBlocks    = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> upBlocks      = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> rollBlocks    = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> pitchBlocks   = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> yawBlocks     = new List<IMyTerminalBlock>();
IMyShipController cockpit;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    RefreshBlocks();
}

void RefreshBlocks()
{
    cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyShipController;
    GetBlocksFromGroupOrName(forward_WS_Group, forwardBlocks);
    GetBlocksFromGroupOrName(side_AD_Group, sideBlocks);
    GetBlocksFromGroupOrName(up_SpaceC_Group, upBlocks);
    GetBlocksFromGroupOrName(roll_QE_Group, rollBlocks);
    GetBlocksFromGroupOrName(pitch_MouseUpDn, pitchBlocks);
    GetBlocksFromGroupOrName(yaw_MouseLftRt, yawBlocks);
}

public void Main(string argument, UpdateType updateSource)
{
    // Reload blocks if "refresh" argument is sent
    if (argument.ToLower() == "refresh") { RefreshBlocks(); return; }
    
    if (cockpit == null) return;

    Vector3 moveVec = cockpit.MoveIndicator;
    float rollVec = cockpit.RollIndicator;
    Vector2 mouseVec = cockpit.RotationIndicator;

    // Stop all movement if no input is detected
    if (moveVec.LengthSquared() < 0.0001f && Math.Abs(rollVec) < 0.0001f && mouseVec.LengthSquared() < 0.0001f)
    {
        StopAll();
        return;
    }

    // Apply velocities based on input
    UpdateGroupVelocity(forwardBlocks, -moveVec.Z, pistonSpeed, rotorSpeed); 
    UpdateGroupVelocity(sideBlocks, moveVec.X, pistonSpeed, rotorSpeed);
    UpdateGroupVelocity(upBlocks, moveVec.Y, pistonSpeed, rotorSpeed);
    UpdateGroupVelocity(rollBlocks, rollVec, pistonSpeed, rotorSpeed);
    UpdateGroupVelocity(pitchBlocks, mouseVec.X, 0, rotorSpeed * mouseSensitivity);
    UpdateGroupVelocity(yawBlocks, mouseVec.Y, 0, rotorSpeed * mouseSensitivity);
}

void StopAll()
{
    UpdateGroupVelocity(forwardBlocks, 0, 0, 0);
    UpdateGroupVelocity(sideBlocks, 0, 0, 0);
    UpdateGroupVelocity(upBlocks, 0, 0, 0);
    UpdateGroupVelocity(rollBlocks, 0, 0, 0);
    UpdateGroupVelocity(pitchBlocks, 0, 0, 0);
    UpdateGroupVelocity(yawBlocks, 0, 0, 0);
}

void UpdateGroupVelocity(List<IMyTerminalBlock> blocks, float direction, float pSpeed, float rSpeed)
{
    if (blocks == null || blocks.Count == 0) return;
    foreach (var block in blocks)
    {
        if (block == null) continue;
        float multiplier = block.CustomData.ToLower().Contains("invert") ? -1.0f : 1.0f;
        float finalDir = direction * multiplier;

        if (block is IMyPistonBase)
            ((IMyPistonBase)block).Velocity = finalDir * pSpeed;
        else if (block is IMyMotorStator)
            ((IMyMotorStator)block).TargetVelocityRPM = finalDir * rSpeed;
    }
}

void GetBlocksFromGroupOrName(string name, List<IMyTerminalBlock> list)
{
    list.Clear();
    var group = GridTerminalSystem.GetBlockGroupWithName(name);
    if (group != null) group.GetBlocks(list);
    else
    {
        var block = GridTerminalSystem.GetBlockWithName(name);
        if (block != null) list.Add(block);
    }
}