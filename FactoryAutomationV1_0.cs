public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    IMyTextPanel lcd = GridTerminalSystem.GetBlockWithName("LCD Production") as IMyTextPanel;
    
    // --- MAPPING: ITEM NAME -> BLUEPRINT NAME ---
    // Some items have different names in the inventory vs the assembler queue.
    Dictionary<string, string> blueprintMap = new Dictionary<string, string>()
    {
        { "SteelPlate", "SteelPlate" },
        { "InteriorPlate", "InteriorPlate" },
        { "Construction", "ConstructionComponent" }, // Corrected!
        { "Girder", "GirderComponent" },             // Corrected!
        { "SmallTube", "SmallTube" },
        { "LargeTube", "LargeTube" }
    };

    Dictionary<string, int> targets = new Dictionary<string, int>()
    {
        { "SteelPlate", 2000 }, { "InteriorPlate", 1000 }, { "Construction", 1000 },
        { "Girder", 500 }, { "SmallTube", 500 }, { "LargeTube", 200 }
    };

    Dictionary<string, int> stock = new Dictionary<string, int>();
    foreach(var key in targets.Keys) stock[key] = 0;

    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks);
    
    foreach (var b in blocks)
    {
        if (!b.HasInventory || b == Me) continue;
        for (int i = 0; i < b.InventoryCount; i++)
        {
            var inv = b.GetInventory(i);
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            foreach (var item in items)
            {
                string sub = item.Type.SubtypeId;
                if (stock.ContainsKey(sub)) stock[sub] += (int)item.Amount;
            }
        }
    }

    var activeAssemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(activeAssemblers, a => a.Mode == MyAssemblerMode.Assembly);

    int idx = 0;
    foreach (var kvp in targets)
    {
        int needed = kvp.Value - stock[kvp.Key];
        if (needed > 0 && activeAssemblers.Count > 0)
        {
            var asm = activeAssemblers[idx % activeAssemblers.Count];
            string bpName = blueprintMap[kvp.Key];
            
            MyDefinitionId def;
            if (!MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + bpName, out def))
            {
                Echo("Error parsing: " + bpName);
                continue;
            }

            var queue = new List<MyProductionItem>();
            asm.GetQueue(queue);
            bool alreadyQueued = false;
            foreach(var item in queue) if(item.BlueprintId == def) alreadyQueued = true;

            if (!alreadyQueued) asm.AddQueueItem(def, (MyFixedPoint)needed);
            idx++;
        }
    }

    if (lcd != null)
    {
        lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        string output = "--- PRODUCTION STATUS ---\n\n";
        foreach (var kvp in targets)
        {
            int current = stock[kvp.Key];
            int needed = kvp.Value - current;
            output += $"{kvp.Key}: {current}/{kvp.Value} [{(needed <= 0 ? "STOCKED" : "QUEUED: " + needed)}]\n";
        }
        lcd.WriteText(output);
    }
}