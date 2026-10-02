/*
 * ComeAndSee's Combat Drone Deployment System v0.4
 * 
 * Description:
 * This script reads the status from an AI offensive block and uses a signal relay to deploy docked drones when enemies are detected. 
 * When no enemies are detected, it signals the drones to return. Only the ship deploying the drones needs this script.
 * Additionally, optional timer blocks can be triggered when the drones are signaled to return or attack, such as opening hangar doors.
 * Note: The AI offensive block doesn't need a move block to function.
 */

//Configuration
//By default the script will look for the first action relay and AI offensive block on the grid.
//You only need to set the names if you want to specify a specific one.

// Set the names of the blocks here
string AIOffensiveBlockName = "";  // (Optional) The name of the AI offensive block to monitor
string actionRelayName = "";  // (Optional) The name of the transponder block to send signals through
string lcdPanelName = "";  // (Optional) The name of the LCD panel to display the status information on
string attackTimerBlockName = "";  // (Optional) The name of the timer block to trigger when drones are signaled to attack
string returnTimerBlockName = "";  // (Optional) The name of the timer block to trigger when drones are signaled to return

// Set the channel numbers here
int returnChannel = 36;  // The channel number for the transponder signal when drones should return
int attackChannel = 35;  // The channel number for the transponder signal when drones should attack

IMyOffensiveCombatBlock offensiveBlock;
IMyTransponder transponder;
IMyTextPanel lcdPanel;
IMyProgrammableBlock programmableBlock;
IMyTimerBlock attackTimerBlock;
IMyTimerBlock returnTimerBlock;
string previousState = "";

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    InitializeBlocks();
}

void InitializeBlocks()
{
    offensiveBlock = !string.IsNullOrEmpty(AIOffensiveBlockName) 
        ? GridTerminalSystem.GetBlockWithName(AIOffensiveBlockName) as IMyOffensiveCombatBlock
        : GetFirstBlockOfType<IMyOffensiveCombatBlock>();

    transponder = !string.IsNullOrEmpty(actionRelayName) 
        ? GridTerminalSystem.GetBlockWithName(actionRelayName) as IMyTransponder
        : GetFirstBlockOfType<IMyTransponder>();

    lcdPanel = !string.IsNullOrEmpty(lcdPanelName) 
        ? GridTerminalSystem.GetBlockWithName(lcdPanelName) as IMyTextPanel 
        : null;

    attackTimerBlock = !string.IsNullOrEmpty(attackTimerBlockName) 
        ? GridTerminalSystem.GetBlockWithName(attackTimerBlockName) as IMyTimerBlock 
        : null;

    returnTimerBlock = !string.IsNullOrEmpty(returnTimerBlockName) 
        ? GridTerminalSystem.GetBlockWithName(returnTimerBlockName) as IMyTimerBlock 
        : null;

    programmableBlock = Me as IMyProgrammableBlock;

    if (offensiveBlock == null)
    {
        Echo($"{AIOffensiveBlockName} block not found.");
    }
    if (transponder == null)
    {
        Echo($"{actionRelayName} not found.");
    }
    if (lcdPanel != null)
    {
        lcdPanel.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    }

    if (programmableBlock != null)
    {
        var surface = programmableBlock.GetSurface(0);
        surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        surface.WriteText("Combat Drone - " + previousState);
    }

    if (offensiveBlock != null)
    {
        previousState = offensiveBlock.CustomData;
    }
}

void Main(string argument, UpdateType updateSource)
{
    // Ensure blocks are initialized
    if (offensiveBlock == null || transponder == null || programmableBlock == null)
    {
        InitializeBlocks();
        if (offensiveBlock == null || transponder == null)
        {
            return;
        }
    }

    // Retrieve the status information
    int lastScan = offensiveBlock.LastScan;
    OffensiveCombatTargetPriority targetPriority = offensiveBlock.TargetPriority;
    bool isWorking = offensiveBlock.IsWorking;
    string detailedInfo = offensiveBlock.DetailedInfo;

    detailedInfo = System.Text.RegularExpressions.Regex.Replace(detailedInfo, @"\[Color=#FFFF0000\]ERROR: Grid has no active Move type block! Please add and/or enable a Move type block\.\[\/Color\]", "");

    string currentState = detailedInfo.Contains("Status: Searching for enemies") ? "Searching" : "Attacking";

    if (currentState != previousState)
    {
        if (currentState == "Searching")
        {
            transponder.Channel = returnChannel;
            transponder.SendSignal();
            if (returnTimerBlock != null)
            {
                returnTimerBlock.Trigger();
            }
        }
        else
        {
            transponder.Channel = attackChannel;
            transponder.SendSignal();
            if (attackTimerBlock != null)
            {
                attackTimerBlock.Trigger();
            }
        }
        previousState = currentState;
        offensiveBlock.CustomData = currentState;

        if (programmableBlock != null)
        {
            var surface = programmableBlock.GetSurface(0);
            surface.WriteText("Combat Drone - " + previousState);
        }
    }

    StringBuilder status = new StringBuilder();

    // Current Drone Deployment Status
    status.AppendLine("Current Drone Deployment Status:");
    status.AppendLine($"  * {(currentState == "Searching" ? "Return" : currentState)}");
    status.AppendLine();

    // Previous Drone Deployment Status
    status.AppendLine("Previous Drone Deployment Status:");
    status.AppendLine($"  * {(previousState == "Searching" ? "Return" : previousState)}");
    status.AppendLine();

    // Offensive Combat Block Status
    status.AppendLine("Offensive Combat Block Status:");
    status.AppendLine($"   * Last Scan: {lastScan} ms ago");
    status.AppendLine($"   * Target Priority: {targetPriority}");
    status.AppendLine($"   * Is Working: {isWorking}");
    status.AppendLine($"   * Detailed Info:");

    // Process and append detailed info
    var lines = detailedInfo.Split('\n');
    foreach (var line in lines)
    {
        if (line.Contains("Searching") || line.Contains("Attacking"))
        {
            status.AppendLine($"   * {line.Trim()}");
        }
        else if (!string.IsNullOrWhiteSpace(line))
        {
            status.AppendLine($"      {line.Trim()}");
        }
    }

    // Main Display
    Echo(status.ToString());
    if (lcdPanel != null)
    {
        lcdPanel.WriteText(status.ToString());
    }
}

T GetFirstBlockOfType<T>() where T : class, IMyTerminalBlock
{
    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<T>(blocks, b => b.CubeGrid == Me.CubeGrid);
    return blocks.FirstOrDefault() as T;
}

void Save()
{
    if (offensiveBlock != null)
    {
        offensiveBlock.CustomData = previousState;
    }
}
