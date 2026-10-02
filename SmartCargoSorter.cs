// Smart Cargo Sorter
// by FairFly35
// Moves items from all valid inventories into containers with "ALL" in name
// Skips turrets, weapons, O2/H2 generators, and reactors
// Ensures only one stack inside cargo container 
// Also pulls output from Refineries and Assemblers (mod-compatible)

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    Echo("Sorter Active — CODE RUNNING PROPERLY");
}

public void Main(string argument, UpdateType updateSource)
{
    // Find all "ALL" containers
    var allContainers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(allContainers, b =>
        b.IsSameConstructAs(Me) && b.CustomName.Contains("ALL")
    );

    if (allContainers.Count == 0)
    {
        Echo("No 'ALL' containers found.");
        return;
    }

    // Find all blocks with inventory except generators, weapons/turrets, reactors
    var invBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(invBlocks, block =>
        block.HasInventory &&
        block.IsSameConstructAs(Me) &&
        !(block is IMyGasGenerator) &&
        !IsWeapon(block) &&
        !IsReactor(block)
    );

    int moved = 0;
    Dictionary<string, int> itemCounts = new Dictionary<string, int>();

    foreach (var block in invBlocks)
    {
        int invCount = block.InventoryCount;
        for (int i = 0; i < invCount; i++)
        {
            // Skip assembler input (inventory 0) if it's an assembler
            if (block is IMyAssembler && i == 0) continue;

            // For refineries, only use output inventory (index 1)
            if (block is IMyRefinery && i != 1) continue;

            var srcInv = block.GetInventory(i);
            if (block.CustomName.Contains("ALL")) continue;

            var items = new List<MyInventoryItem>();
            srcInv.GetItems(items);

            foreach (var item in items)
            {
                IMyInventory destInv = FindDestinationInventory(allContainers, item);
                if (destInv != null && srcInv.CanTransferItemTo(destInv, item.Type))
                {
                    srcInv.TransferItemTo(destInv, item);
                    moved++;
                }

                // Count items for display
                string typeName = item.Type.SubtypeId;
                if (!itemCounts.ContainsKey(typeName))
                    itemCounts[typeName] = 0;
                itemCounts[typeName] += (int)item.Amount;
            }
        }
    }

    Echo($"Active | Items moved: {moved}");

    // Display counts on PB's built-in LCD
    var sb = new System.Text.StringBuilder();
    sb.AppendLine($"Items on grid:");
    foreach (var kvp in itemCounts)
    {
        sb.AppendLine($"{kvp.Key}: {kvp.Value}");
    }
    Me.GetSurface(0).WriteText(sb.ToString(), false);
}

// Determines if a block is a weapon or turret (vanilla or modded)
bool IsWeapon(IMyTerminalBlock block)
{
    return block is IMyUserControllableGun ||
           block is IMyLargeTurretBase ||
           block.BlockDefinition.TypeIdString.Contains("Turret") ||
           block.BlockDefinition.TypeIdString.Contains("Gun") ||
           block.BlockDefinition.SubtypeName.Contains("Turret") ||
           block.BlockDefinition.SubtypeName.Contains("Gun");
}

// Determines if a block is a reactor (vanilla or modded)
bool IsReactor(IMyTerminalBlock block)
{
    if (block is IMyReactor) return true;

    // Check for modded reactors by name/type
    string typeId = block.BlockDefinition.TypeIdString;
    string subtype = block.BlockDefinition.SubtypeName;
    return typeId.Contains("Reactor") || subtype.Contains("Reactor");
}

// Finds a valid ALL container to receive an item
IMyInventory FindDestinationInventory(List<IMyCargoContainer> containers, MyInventoryItem item)
{
    foreach (var c in containers)
    {
        var inv = c.GetInventory();
        var items = new List<MyInventoryItem>();
        inv.GetItems(items);

        bool hasItem = items.Any(x => x.Type.Equals(item.Type));
        if (!hasItem && !inv.IsFull)
            return inv;
    }

    foreach (var c in containers)
    {
        var inv = c.GetInventory();
        if (!inv.IsFull)
            return inv;
    }

    return null;
}
