public Program() { }

public void Main(string argument, UpdateType updateSource)
{
    string command = argument.Trim().ToLower();

    if (string.IsNullOrEmpty(command))
    {
        Echo("Error: Please provide a command argument!");
        Echo("Valid commands: firstrun, renamedrone, undo");
        return;
    }

    // 1. Extract the current grid number using Regex
    System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Me.CubeGrid.CustomName, @"\d+");
    if (!match.Success)
    {
        Echo("Error: Could not determine grid number from current name!");
        return;
    }
    string gridNumber = match.Value;

    // 2. Route commands
    switch (command)
    {
        case "firstrun":
            ExecuteFirstRun(gridNumber);
            break;

        case "renamedrone":
            ExecuteRenameDrone(gridNumber);
            break;

        case "undo":
            ExecuteUndo(gridNumber);
            break;

        default:
            Echo($"Unknown command: '{argument}'");
            break;
    }
}

// --- COMMAND 1: FIRSTRUN ---
private void ExecuteFirstRun(string gridNumber)
{
    Me.CubeGrid.CustomName = $"Docking Pad {gridNumber}";

    // First run checks a 6m radius cylinder directly around the PB itself
    List<IMyTerminalBlock> localBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(localBlocks, b => IsWithinRange(Me, b, 6.0, 18.0));

    int count = RenameBlocksInList(localBlocks, "####", gridNumber);
    Echo($"[FirstRun] Pad Grid renamed. Updated {count} local pad blocks.");
}

// --- COMMAND 2: RENAMEDRONE ---
private void ExecuteRenameDrone(string gridNumber)
{
    // Find the specific connector on this docking pad setup
    IMyShipConnector localConnector = FindLocalConnector(gridNumber);

    if (localConnector == null)
    {
        Echo("Rename Drone Failed: Could not find a connector on this pad within range.");
        return;
    }

    if (localConnector.Status == MyShipConnectorStatus.Unconnected)
    {
        Echo("Rename Drone Failed: Pad connector is not connected to a drone.");
        return;
    }

    // 6.0m side tolerance to catch the 9x9 corners, extending 18m straight out from the connector face
    List<IMyTerminalBlock> nearbyBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(nearbyBlocks, b => IsWithinRange(localConnector, b, 6.0, 18.0));

    int blockCount = RenameBlocksInList(nearbyBlocks, "####", gridNumber);
    Echo($"[RenameDrone] Isolated 18m bay corridor. Updated {blockCount} blocks on the attached Drone.");
}

// --- COMMAND 3: UNDO ---
private void ExecuteUndo(string gridNumber)
{
    IMyShipConnector localConnector = FindLocalConnector(gridNumber);
    IMyTerminalBlock anchor = localConnector != null ? (IMyTerminalBlock)localConnector : (IMyTerminalBlock)Me;

    // Clean up blocks strictly inside our specific deep bay corridor
    List<IMyTerminalBlock> nearbyBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(nearbyBlocks, b => IsWithinRange(anchor, b, 6.0, 18.0));
    
    int revertedCount = RenameBlocksInList(nearbyBlocks, gridNumber, "####");

    Me.CubeGrid.CustomName = $"Small Grid {gridNumber}";
    Echo($"[Undo] Reset {revertedCount} blocks in this bay zone back to '####'.");
}

// --- DIRECTIONAL PROXIMITY UTILITY (CYLINDER SHAPE) ---
private bool IsWithinRange(IMyTerminalBlock anchor, IMyTerminalBlock target, double maxSideDistance, double maxForwardDistance)
{
    VRageMath.Vector3D targetPos = target.GetPosition();
    VRageMath.Vector3D anchorPos = anchor.GetPosition();
    
    // Vector pointing from the anchor block to the target block
    VRageMath.Vector3D toTarget = targetPos - anchorPos;
    
    // Direction the anchor block is facing in the world space
    VRageMath.Vector3D forwardVector = anchor.WorldMatrix.Forward;
    
    // Calculate how far along that forward line the block sits
    double forwardDistance = VRageMath.Vector3D.Dot(toTarget, forwardVector);
    
    // Calculate how far to the sides (radial deviation) the block sits
    VRageMath.Vector3D sideVector = toTarget - (forwardVector * forwardDistance);
    double sideDistance = sideVector.Length();

    // Accept the block if it's within the side radius, and fits within the 18m length corridor
    return (sideDistance <= maxSideDistance) && (forwardDistance >= 0.0 && forwardDistance <= maxForwardDistance);
}

// Finds the connector belonging to this specific pad setup
private IMyShipConnector FindLocalConnector(string gridNumber)
{
    List<IMyShipConnector> connectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors);

    foreach (var conn in connectors)
    {
        // Finds a connector within a loose 6-meter boundary of the PB
        double dist = VRageMath.Vector3D.Distance(Me.GetPosition(), conn.GetPosition());
        if (dist <= 6.0)
        {
            return conn;
        }
    }
    return null;
}

private int RenameBlocksInList(List<IMyTerminalBlock> blocks, string oldText, string newText)
{
    int changed = 0;
    foreach (var block in blocks)
    {
        if (block.CustomName.Contains(oldText))
        {
            block.CustomName = block.CustomName.Replace(oldText, newText);
            changed++;
        }
    }
    return changed;
}