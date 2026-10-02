/*
===============================
Farm Plot Monitor – English Version
===============================

Instructions:

1. Place a Programmable Block (PB) on your grid and paste this script into it.
2. Place one or more LCD panels where you want to display farm info.
3. For each LCD panel, open its Custom Data and add lines for the plots you want to display:
   
   Example:
   INCLUDE=Plot 1
   INCLUDE=Plot 2

   - Each INCLUDE line can contain the full or partial name of a farm plot.
   - The LCD will display only plots that match one of these INCLUDE filters.
   - If no INCLUDE lines are present, the LCD will be ignored.

4. The script will output all available information from each farm plot's DetailedInfo:
   - Status (e.g., Growing, Ready)
   - Time remaining until harvest
   - Type of plant (grain, mushroom spores, fruit, vegetables)
   - Any other info provided by the game

5. The script automatically updates all configured LCDs every ~1.6 seconds.

6. You can check the status in the Programmable Block's Echo window if needed.

7. Multiple LCDs can be configured independently with different INCLUDE filters.

===============================
*/

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100; // ~1.6s
}

public void Main(string argument, UpdateType updateSource)
{
    var lcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(lcds);

    var farmPlots = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(farmPlots, b => b.BlockDefinition.SubtypeId.ToLower().Contains("farm"));

    if (farmPlots.Count == 0)
    {
        Echo("No farm plots found.");
        return;
    }

    int lcdCount = 0;

    foreach (var lcd in lcds)
    {
        // Read filter from LCD Custom Data
        var include = new List<string>();
        var lines = lcd.CustomData.Split('\n');
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("INCLUDE=", StringComparison.OrdinalIgnoreCase))
            {
                include.Add(trimmed.Substring(8).Trim());
            }
        }

        if (include.Count == 0)
            continue; // skip LCDs without config

        lcdCount++;

        var outputLines = new List<string>();

        foreach (var block in farmPlots)
        {
            if (!include.Any(f => block.CustomName.Contains(f)))
                continue;

            outputLines.Add($"--- {block.CustomName} ---");
            outputLines.Add(block.DetailedInfo); // full info
            outputLines.Add(""); // empty line for readability
        }

        string output;
        if (outputLines.Count == 0)
        {
            output = "No matching farm plots found.";
        }
        else
        {
            output = string.Join("\n", outputLines);
        }

        lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        lcd.WriteText(output);
    }

    Echo($"Farm Monitor active – {lcdCount} LCD(s) updated.");
}
