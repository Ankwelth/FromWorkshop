public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    List<IMyCargoContainer> containers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(containers);

    List<IMyInventory> ore = new List<IMyInventory>();
    List<IMyInventory> ice = new List<IMyInventory>();
    List<IMyInventory> ingot = new List<IMyInventory>();
    List<IMyInventory> comp = new List<IMyInventory>();
    List<IMyInventory> ammo = new List<IMyInventory>();
    List<IMyInventory> food = new List<IMyInventory>();
    List<IMyInventory> other = new List<IMyInventory>();

    // Assign inventories (multi-tag safe)
    for (int i = 0; i < containers.Count; i++)
    {
        IMyCargoContainer c = containers[i];
        string name = c.CustomName.ToUpper();

        IMyInventory inv = c.GetInventory();

        if (name.Contains("[ORE]")) ore.Add(inv);
        if (name.Contains("[ICE]")) ice.Add(inv);
        if (name.Contains("[INGOT]")) ingot.Add(inv);
        if (name.Contains("[COMP]")) comp.Add(inv);
        if (name.Contains("[AMMO]")) ammo.Add(inv);
        if (name.Contains("[FOOD]")) food.Add(inv);
        if (name.Contains("[OTHER]")) other.Add(inv);
    }

    List<MyInventoryItem> items = new List<MyInventoryItem>();

    // Process all containers
    for (int c = 0; c < containers.Count; c++)
    {
        IMyInventory inv = containers[c].GetInventory();
        items.Clear();
        inv.GetItems(items);

        for (int i = items.Count - 1; i >= 0; i--)
        {
            MyInventoryItem item = items[i];

            string type = item.Type.TypeId;
            string subtype = item.Type.SubtypeId;

            IMyInventory target = null;

            // ICE
            if (subtype == "Ice")
                target = Pick(ice);

            // ORE
            else if (type == "MyObjectBuilder_Ore")
                target = Pick(ore);

            // INGOT
            else if (type == "MyObjectBuilder_Ingot")
                target = Pick(ingot);

            // COMPONENT
            else if (type == "MyObjectBuilder_Component")
                target = Pick(comp);

            // AMMO
            else if (type == "MyObjectBuilder_AmmoMagazine")
            {
                if (subtype.Contains("NATO_25x184mm") ||
                    subtype.Contains("Missile") ||
                    subtype.Contains("Artillery") ||
                    subtype.Contains("Assault") ||
                    subtype.Contains("Railgun"))
                    target = Pick(ammo);
                else
                    target = Pick(other);
            }

            // FOOD (RAW + COOKED)
            else if (type == "MyObjectBuilder_ConsumableItem" ||
                     type == "MyObjectBuilder_CookedItem")
                target = Pick(food);

            // EVERYTHING ELSE
            else
                target = Pick(other);

            if (target != null && target != inv)
            {
                inv.TransferItemTo(target, i, null, true);
            }
        }
    }
}

// Simple helper (C# 6 safe, NOT local function)
IMyInventory Pick(List<IMyInventory> list)
{
    if (list == null || list.Count == 0)
        return null;

    return list[0];
}