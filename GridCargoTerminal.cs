// Space Engineers script: Total inventory ore fill % + ore listing
// Author: FairFly35
//  Please do not Reupload this Script, Thanks.
// Scans ALL blocks with inventory, sums ore amounts, shows total fill % + ore list

string lcdName = "Ore LCD";   // name of LCD panel to display info

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100; // run every ~0.6s
}

public void Main(string argument, UpdateType updateSource)
{
    var lcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextPanel;
    if (lcd == null)
    {
        Echo("ERROR: LCD not found!");
        return;
    }

    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(allBlocks);

    MyFixedPoint totalCurrent = 0;
    MyFixedPoint totalMax = 0;

    var oreDict = new Dictionary<string, double>();

    foreach (var block in allBlocks)
    {
        var invCount = block.InventoryCount;
        if (invCount <= 0) continue;

        for (int i = 0; i < invCount; i++)
        {
            var inv = block.GetInventory(i);
            totalCurrent += inv.CurrentVolume;
            totalMax += inv.MaxVolume;

            var items = new List<MyInventoryItem>();
            inv.GetItems(items);

            foreach (var item in items)
            {
                if (item.Type.TypeId.ToString().Contains("Ore")) // filter ores only
                {
                    string oreName = item.Type.SubtypeId.ToString();
                    double amount = (double)item.Amount;

                    if (!oreDict.ContainsKey(oreName))
                        oreDict[oreName] = 0;

                    oreDict[oreName] += amount;
                }
            }
        }
    }

    double fillPercent = totalMax > 0 ? (double)totalCurrent / (double)totalMax * 100.0 : 0;

    // Build output text
    var sb = new StringBuilder();
    sb.AppendLine("All Inventories");
    sb.AppendLine($"Total Fill: {fillPercent:F1}% ({totalCurrent} / {totalMax})");
    sb.AppendLine();
    sb.AppendLine("Ores:");

    if (oreDict.Count == 0)
    {
        sb.AppendLine(" - None -");
    }
    else
    {
        foreach (var kvp in oreDict.OrderBy(x => x.Key))
        {
            sb.AppendLine($"{kvp.Key}: {kvp.Value:N0}");
        }
    }

    lcd.ContentType = ContentType.TEXT_AND_IMAGE;
    lcd.WriteText(sb.ToString(), false);

    // Show status inside the programmable block info panel
    Echo("CODE ACTIVE\nCHECK YOUR LCD");
}
