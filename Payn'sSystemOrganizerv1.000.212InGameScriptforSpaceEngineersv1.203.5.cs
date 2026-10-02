void Main(string argument)
{
    // Only proceed if the argument is "NAME"
    if (argument.ToLower() != "name")
    {
        Echo("Invalid command. Use 'NAME' to rename blocks.");
        return;
    }

// Diagnostic code to list all block groups
var allGroups = new List<IMyBlockGroup>();
GridTerminalSystem.GetBlockGroups(allGroups);
Echo($"Found {allGroups.Count} groups:");
foreach (var grp in allGroups)
{
    Echo($"- {grp.Name}");
}

    // Parse the custom data
    var customData = Me.CustomData;
    var lines = customData.Split('\n');
    string groupName = "";
    string customName = "";
    string gridType = "SameGrid"; // Default to same grid
    string specifiedGridName = null; // Grid name for ConnectedGrids, null if not specified

    foreach (var line in lines)
    {
        if (line.StartsWith("Group:"))
        {
            groupName = line.Substring(6).Trim();
        }
        else if (line.StartsWith("CustomizedName:"))
        {
            customName = line.Substring(16).Trim();
        }
        else if (line.StartsWith("GridType:"))
        {
            gridType = line.Substring(9).Trim();
        }
        else if (line.StartsWith("GridName:"))
        {
            var gridNameEntry = line.Substring(9).Trim();
            specifiedGridName = gridNameEntry == "[]" ? null : gridNameEntry;
        }
    }

    // Check if group name and custom name are provided
    if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(customName))
    {
        Echo("Group name or custom name is missing.");
        return;
    }

    // Attempt to find and rename blocks in the group on the same or connected grids
    bool groupFound = false;
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    if (gridType.Equals("ConnectedGrids", StringComparison.OrdinalIgnoreCase))
    {
        var allGrids = new List<IMyCubeGrid>();
        GetConnectedGrids(Me.CubeGrid, allGrids);

        foreach (var grid in allGrids)
        {
            if (specifiedGridName == null || grid.CustomName == specifiedGridName)
            {
                var groups = new List<IMyBlockGroup>();
                GridTerminalSystem.GetBlockGroups(groups);
                foreach (var group in groups)
                {
                    if (group.Name == groupName)
                    {
                        group.GetBlocks(blocks);
                        groupFound = true;
                        break;
                    }
                }
            }
        }
    }
    else
    {
        var group = GridTerminalSystem.GetBlockGroupWithName(groupName);
        if (group != null)
        {
            group.GetBlocks(blocks);
            groupFound = true;
        }
    }

    if (!groupFound)
    {
        Echo($"Group '{groupName}' not found on this grid or any connected grids.");
        return;
    }

    // Rename each block in the group
    int counter = 1;
    foreach (var block in blocks)
    {
        block.CustomName = $"{customName} {counter}";
        counter++;
    }

    Echo($"Renamed {blocks.Count} blocks in group '{groupName}' on {gridType} grid.");
}

void GetConnectedGrids(IMyCubeGrid startingGrid, List<IMyCubeGrid> allGrids)
{
    var gridGroups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(gridGroups);

    foreach (var group in gridGroups)
    {
        var blocks = new List<IMyTerminalBlock>();
        group.GetBlocks(blocks);
        foreach (var block in blocks)
        {
            var grid = block.CubeGrid;
            if (!allGrids.Contains(grid) && grid.IsSameConstructAs(startingGrid))
            {
                allGrids.Add(grid);
                GetConnectedGrids(grid, allGrids);
            }
        }
    }
}

// Required for Space Engineers scripts
public Program() { Runtime.UpdateFrequency = UpdateFrequency.None; }
