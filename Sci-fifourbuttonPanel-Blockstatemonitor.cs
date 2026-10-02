bool isRunning = false;

// Configuration Parameters
string buttonPanelName = "Sci-Fi Four-Button Panel";
string[] groupNames = { "BuildAndRepairGroup1", "[MC-RS] Turrets", "[MC-RS] Artillery Turrets", "[MC-RS] Assault Cannons" };
string[] customTexts = { "Piston", "Turrets", "Artillery\nTurrets", "Assault\nCannons" };
string[] onTexts = { "Up", "ON", "ON", "ON" };
string[] offTexts = { "Down", "Off", "Off", "Off" };

Color onColor = new Color(0, 255, 0); // Default to Green
Color offColor = new Color(255, 0, 0); // Default to Red

public Program()
{
    // Initialization
    if (!isRunning)
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update10; // Set the script to run every 10 game ticks
        isRunning = true;
    }
}

void Main(string argument)
{
    // Get the Sci-Fi Four Button Panel
    var buttonPanel = GridTerminalSystem.GetBlockWithName(buttonPanelName) as IMyTextSurfaceProvider;

    // Check if the panel exists
    if (buttonPanel == null)
    {
        Echo($"Error: Panel not found. Make sure you entered the correct name '{buttonPanelName}'.");
        return;
    }

    // Specify the screen indices for each group (0 to 3)
    int targetScreenIndex1 = 0;
    int targetScreenIndex2 = 1;
    int targetScreenIndex3 = 2;
    int targetScreenIndex4 = 3;

    // Get the groups of blocks
    var blockGroup1 = GridTerminalSystem.GetBlockGroupWithName(groupNames[0]) as IMyBlockGroup;
    var blockGroup2 = GridTerminalSystem.GetBlockGroupWithName(groupNames[1]) as IMyBlockGroup;
    var blockGroup3 = GridTerminalSystem.GetBlockGroupWithName(groupNames[2]) as IMyBlockGroup;
    var blockGroup4 = GridTerminalSystem.GetBlockGroupWithName(groupNames[3]) as IMyBlockGroup;

    // Check if the groups exist
    if (blockGroup1 == null || blockGroup2 == null || blockGroup3 == null || blockGroup4 == null)
    {
        Echo("Error: Groups not found. Make sure you entered the correct names.");
        return;
    }

    // Check the state of blocks for each group
    bool areBlocksWorking1 = AreBlocksWorking(blockGroup1);
    bool areBlocksWorking2 = AreBlocksWorking(blockGroup2);
    bool areBlocksWorking3 = AreBlocksWorking(blockGroup3);
    bool areBlocksWorking4 = AreBlocksWorking(blockGroup4);

    // Display custom text on/off status on the specified screens of the Sci-Fi Four Button Panel
    DisplayOnButtonPanel(buttonPanel, targetScreenIndex1, areBlocksWorking1, customTexts[0], onTexts[0], offTexts[0], onColor, offColor);
    DisplayOnButtonPanel(buttonPanel, targetScreenIndex2, areBlocksWorking2, customTexts[1], onTexts[1], offTexts[1], onColor, offColor);
    DisplayOnButtonPanel(buttonPanel, targetScreenIndex3, areBlocksWorking3, customTexts[2], onTexts[2], offTexts[2], onColor, offColor);
    DisplayOnButtonPanel(buttonPanel, targetScreenIndex4, areBlocksWorking4, customTexts[3], onTexts[3], offTexts[3], onColor, offColor);
}

bool AreBlocksWorking(IMyBlockGroup blockGroup)
{
    List<IMyFunctionalBlock> blocks = new List<IMyFunctionalBlock>();
    blockGroup.GetBlocksOfType(blocks);

    foreach (var block in blocks)
    {
        if (block.IsWorking)
        {
            return true;
        }
    }

    return false;
}

void DisplayOnButtonPanel(IMyTextSurfaceProvider buttonPanel, int screenIndex, bool areBlocksWorking, string customText, string onText, string offText, Color onColor, Color offColor)
{
    if (screenIndex < 0 || screenIndex >= buttonPanel.SurfaceCount)
    {
        Echo("Error: Invalid screen index.");
        return;
    }

    var surface = buttonPanel.GetSurface(screenIndex);
    surface.ContentType = ContentType.TEXT_AND_IMAGE;

    if (areBlocksWorking)
    {
        surface.WriteText($"{customText}\n{onText}", false);
        surface.FontColor = onColor;
    }
    else
    {
        surface.WriteText($"{customText}\n{offText}", false);
        surface.FontColor = offColor;
    }
}
