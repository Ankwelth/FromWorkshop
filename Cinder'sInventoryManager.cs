// Author: Cinder
// Date: 1/2/2024
// Description: Automatically assemble & disassemble items, organize cargo containers, and display on LCDs

// =====================================================
// Block Names
// Note: Manually set all others assemblers except the master to co-op mode. The script will only use the master assembler.
// =====================================================
string SettingsLCD = "Settings LCD";
string IngotLCD = "Ingot LCD";
string OreLCD = "Ore LCD";
string ComponentLCD = "Component LCD";
string EquipmentLCD = "Equipment LCD";
string AmmoLCD = "Ammo LCD";
string AssemblerName = "Assembler";
string IngotCargo = "Ingot";
string OreCargo = "Ore";
string ComponentCargo = "Component";
string EquipmentCargo = "Equipment";
string AmmoCargo = "Ammo";
// =====================================================
// Set true/false to automatically assemble/dissasemble components
// =====================================================
bool AssembleItems = false;
bool DisassembleItems = false;
//======================================================
// Misc Settings
//======================================================
bool OrganizeInventories = true;
bool IgnoreOtherGrids = false;
Color LCDColor = new Color(0, 90, 255);
bool LearnAssemblerItem = false;  // To use this: Set to true, turn off your assembler, put one of the item in the production queue, and one of the item in the assembler inventory

// =====================================================
// =====================================================
// DON'T CHANGE ANYTHING BELOW THIS LINE
// =====================================================
// =====================================================

class Item
{
    public string Name {get; set;}
    public int Actual {get; set;}
    public int Desired {get; set;}
    public string Blueprint {get; set;}
}

Dictionary<string, Item> ingots = new Dictionary<string, Item>();
Dictionary<string, Item> ore = new Dictionary<string, Item>();
Dictionary<string, Item> components = new Dictionary<string, Item>();
Dictionary<string, Item> equipment = new Dictionary<string, Item>();
Dictionary<string, Item> ammo = new Dictionary<string, Item>();

char[] rotatingLine = { '|', '/', '-', '\\' };
int counter = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100; // Repeat every 100 game ticks
}

public void Main(string argument)
{
    // Animation to show program is running
    Echo(rotatingLine[counter % 4].ToString());
    counter = (counter + 1) % 4;
    
    // Get grid ID
    IMyCubeGrid shipGrid = Me.CubeGrid;

    // Setup Custom Data if it's empty
    Setup();
    
    // Learn item in assembler
    if (LearnAssemblerItem == true){Learn_Assembler_Item();return;}
    
    // Create the list of all items to craft & display
    Create_Items();

    // Count up all the current items and append to the dictionaries
    Sum_All_Items();

    // Display to LCDs
    Settings_Output();
    Ingots_Output();
    Ore_Output();
    Components_Output();
    Equipment_Output();
    Ammo_Output();

    // Organize inventories
    if (OrganizeInventories){Organize_Inventories();}

    // Logic to assembler/disassemble items
    if (AssembleItems || DisassembleItems) {Assemble_Disassemble();}
}

public void Setup()
{
    string customData = Me.CustomData;
    if (customData != ""){return;}

    // Create Vanilla Component Items
    Me.CustomData += "Component;SteelPlate;SteelPlate;" + Sum_Item("SteelPlate").ToString() + "\n";
    Me.CustomData += "Component;InteriorPlate;InteriorPlate;" + Sum_Item("InteriorPlate").ToString() + "\n";
    Me.CustomData += "Component;Construction;ConstructionComponent;" + Sum_Item("Construction").ToString() + "\n";
    Me.CustomData += "Component;MetalGrid;MetalGrid;" + Sum_Item("MetalGrid").ToString() + "\n";
    Me.CustomData += "Component;Girder;GirderComponent;" + Sum_Item("Girder").ToString() + "\n";
    Me.CustomData += "Component;SmallTube;SmallTube;" + Sum_Item("SmallTube").ToString() + "\n";
    Me.CustomData += "Component;LargeTube;LargeTube;" + Sum_Item("LargeTube").ToString() + "\n";
    Me.CustomData += "Component;Motor;MotorComponent;" + Sum_Item("Motor").ToString() + "\n";
    Me.CustomData += "Component;Display;Display;" + Sum_Item("Display").ToString() + "\n";
    Me.CustomData += "Component;BulletproofGlass;BulletproofGlass;" + Sum_Item("BulletproofGlass").ToString() + "\n";
    Me.CustomData += "Component;Superconductor;Superconductor;" + Sum_Item("Superconductor").ToString() + "\n";
    Me.CustomData += "Component;Computer;ComputerComponent;" + Sum_Item("Computer").ToString() + "\n";
    Me.CustomData += "Component;Reactor;ReactorComponent;" + Sum_Item("Reactor").ToString() + "\n";
    Me.CustomData += "Component;Thrust;ThrustComponent;" + Sum_Item("Thrust").ToString() + "\n";
    Me.CustomData += "Component;GravityGenerator;GravityGeneratorComponent;" + Sum_Item("GravityGenerator").ToString() + "\n";
    Me.CustomData += "Component;Medical;MedicalComponent;" + Sum_Item("Medical").ToString() + "\n";
    Me.CustomData += "Component;RadioCommunication;RadioCommunicationComponent;" + Sum_Item("RadioCommunication").ToString() + "\n";
    Me.CustomData += "Component;Detector;DetectorComponent;" + Sum_Item("Detector").ToString() + "\n";
    Me.CustomData += "Component;Explosives;ExplosivesComponent;" + Sum_Item("Explosives").ToString() + "\n";
    Me.CustomData += "Component;SolarCell;SolarCell;" + Sum_Item("SolarCell").ToString() + "\n";
    Me.CustomData += "Component;PowerCell;PowerCell;" + Sum_Item("PowerCell").ToString() + "\n";

    // Create Vanilla Equipment Items
    Me.CustomData += "OxygenContainerObject;OxygenBottle;Position0010_OxygenBottle;" + Sum_Item("OxygenBottle").ToString() + "\n";
    Me.CustomData += "GasContainerObject;HydrogenBottle;Position0020_HydrogenBottle;" + Sum_Item("HydrogenBottle").ToString() + "\n";
    Me.CustomData += "PhysicalGunObject;AngleGrinder4Item;Position0040_AngleGrinder4;" + Sum_Item("AngleGrinder4Item").ToString() + "\n";
    Me.CustomData += "PhysicalGunObject;HandDrill4Item;Position0080_HandDrill4;" + Sum_Item("HandDrill4Item").ToString() + "\n";
    Me.CustomData += "PhysicalGunObject;Welder4Item;Position0120_Welder4;" + Sum_Item("Welder4Item").ToString() + "\n";
    Me.CustomData += "PhysicalGunObject;AngleGrinderItem;Position0010_AngleGrinder;" + Sum_Item("AngleGrinderItem").ToString() + "\n";
    Me.CustomData += "PhysicalGunObject;HandDrillItem;Position0050_HandDrill;" + Sum_Item("HandDrillItem").ToString() + "\n";
    Me.CustomData += "PhysicalGunObject;WelderItem;Position0090_Welder;" + Sum_Item("WelderItem").ToString() + "\n";

    // Create Vanilla Ammo Items
    Me.CustomData += "AmmoMagazine;Rocket;Position0100_Missile200mm;" + Sum_Item("Missile200mm").ToString() + "\n";

    // Create Vanilla Ingots
    Me.CustomData += "Ingot;Iron\n";
    Me.CustomData += "Ingot;Cobalt\n";
    Me.CustomData += "Ingot;Magnesium\n";
    Me.CustomData += "Ingot;Nickel\n";
    Me.CustomData += "Ingot;Silicon\n";
    Me.CustomData += "Ingot;Gold\n";
    Me.CustomData += "Ingot;Silver\n";
    Me.CustomData += "Ingot;Platinum\n";
    Me.CustomData += "Ingot;Uranium\n";
    Me.CustomData += "Ingot;Stone\n";

    // Create Vanilla Ore
    Me.CustomData += "Ore;Iron\n";
    Me.CustomData += "Ore;Cobalt\n";
    Me.CustomData += "Ore;Magnesium\n";
    Me.CustomData += "Ore;Nickel\n";
    Me.CustomData += "Ore;Silicon\n";
    Me.CustomData += "Ore;Gold\n";
    Me.CustomData += "Ore;Silver\n";
    Me.CustomData += "Ore;Platinum\n";
    Me.CustomData += "Ore;Uranium\n";
    Me.CustomData += "Ore;Ice\n";
    Me.CustomData += "Ore;Stone\n";
    Me.CustomData += "Ore;Scrap\n";
}

public int Sum_Item(string itemName)
{
    var inventories = new List<IMyTerminalBlock>();
    if (IgnoreOtherGrids == true)
    {
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(inventories, block =>
        block.CubeGrid == Me.CubeGrid && block.HasInventory);
    }
    else{GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(inventories, block => block.HasInventory);}

    int count = 0;
    foreach (var inventory in inventories)
    {
        for (int i=0; i < inventory.InventoryCount; i++)
        {
            var items = new List<MyInventoryItem>();
            inventory.GetInventory(i).GetItems(items);
            foreach (var inventoryItem in items)
            {
                string inventoryItemName = inventoryItem.Type.ToString().Substring(inventoryItem.Type.ToString().IndexOf("/") + 1);
                if (inventoryItemName == itemName)
                {
                    count += (int)inventoryItem.Amount;
                }
            }
        }
    }
    return count;
}

public void Create_Items()
{
    string customData = Me.CustomData;

    string[] lines = customData.Split('\n');
    foreach (string line in lines)
    {
        if (line == ""){return;}

        string[] data = line.Split(';');
        if (data[0] == "Ingot")
        {
            ingots[data[1]] = new Item{Name=data[1]};
        }
        else if (data[0] == "Ore")
        {
            ore[data[1]] = new Item{Name=data[1]};
        }
        else if (data[0] == "Component")
        {
            components[data[1]] = new Item{Name=data[1], Desired=int.Parse(data[3]), Blueprint=data[2]};
        }
        else if (data[0] == "AmmoMagazine")
        {
            ammo[data[1]] = new Item{Name=data[1], Desired=int.Parse(data[3]), Blueprint=data[2]};
        }
        else  // Default to equipment
        {
            equipment[data[1]] = new Item{Name=data[1], Desired=int.Parse(data[3]), Blueprint=data[2]};
        }
    }
}

public void Learn_Assembler_Item()
{
    Echo("Learn setting enabled\n");

    List<IMyAssembler> masterAssemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType(masterAssemblers, block => block.CustomName == AssemblerName && block.CubeGrid == Me.CubeGrid);
    IMyAssembler assembler = masterAssemblers.FirstOrDefault();
    string customData = Me.CustomData;

    if (assembler == null && assembler.CubeGrid == Me.CubeGrid){Echo("Assembler not found.");return;}
    assembler.Enabled = false;

    // Get the production queue of the assembler
    List<MyProductionItem> productionQueue = new List<MyProductionItem>();
    assembler.GetQueue(productionQueue);

    if (productionQueue.Count == 0)
    {
        Echo($"{AssemblerName} must have an item in queue to learn");
        return;
    }
    else if (productionQueue.Count > 1)
    {
        Echo($"{AssemblerName} must have only ONE item in queue to learn");
        return;
    }

    // Get the assembler's inventory
    IMyInventory assemblerInventory = assembler.GetInventory(1);

    // Get items in the assembler's inventory
    List<MyInventoryItem> items = new List<MyInventoryItem>();
    assemblerInventory.GetItems(items);

    if (items.Count == 0)
    {
        Echo($"{AssemblerName} must have an item in its inventory to learn");
        return;
    }
    else if (items.Count > 1)
    {
        Echo($"{AssemblerName} must have only ONE item in its inventory to learn");
        return;
    }

    // Retrieve item type
    string newData = "";
    foreach (var item in items)
    {
        MyItemType itemType = item.Type;
        string text = itemType.TypeId.ToString().Substring(itemType.TypeId.ToString().LastIndexOf('_') + 1);
        newData += text + ";";
        Echo($"Item Type: {text}");
    }
    
    // Retrieve item name
    string itemName = "";
    foreach (var item in items)
    {
        itemName = item.Type.ToString().Substring(item.Type.ToString().IndexOf("/") + 1);
        newData += itemName + ";";
        Echo($"Item Name: {itemName}");
    }
    
    // Retrieve blueprint
    foreach (var productionItem in productionQueue)
    {
        MyDefinitionId blueprintId = productionItem.BlueprintId;
        string text = blueprintId.SubtypeId.ToString();
        newData += text + ";";
        Echo($"Blueprint ID: {text}");
    }

    // Count existing items
    int count = Sum_Item(itemName);

    newData += count.ToString() + "\n";

    if (customData.Contains(newData)){Echo("Item learned");}
    else{Me.CustomData += newData;}
}

public void Sum_All_Items()
{
    // Get all the inventories on the ship
    var inventories = new List<IMyTerminalBlock>();
    if (IgnoreOtherGrids == true)
    {
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(inventories, block =>
        block.CubeGrid == Me.CubeGrid && block.HasInventory);
    }
    else{GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(inventories, block => block.HasInventory);}

    // Sum up the quantities of each item type
    foreach (var inventory in inventories)
    {
        for (int i=0; i < inventory.InventoryCount; i++)
        {
            var items = new List<MyInventoryItem>();
            inventory.GetInventory(i).GetItems(items);
            foreach (var item in items)
            {
                string itemName = item.Type.ToString().Substring(item.Type.ToString().IndexOf("/") + 1);

                // Ingots
                if (ingots.ContainsKey(itemName) && item.Type.ToString().Contains("Ingot"))
                {
                    ingots[itemName].Actual += (int)item.Amount;
                }

                // Ore
                else if (ore.ContainsKey(itemName) && item.Type.ToString().Contains("Ore"))
                {
                    ore[itemName].Actual += (int)item.Amount;
                }
                
                // Components
                else if (components.ContainsKey(itemName))
                {
                    components[itemName].Actual += (int)item.Amount;
                }

                // Ammo
                else if (ammo.ContainsKey(itemName))
                {
                    ammo[itemName].Actual += (int)item.Amount;
                }

                // Equipment
                else if (equipment.ContainsKey(itemName))
                {
                    equipment[itemName].Actual += (int)item.Amount;
                }

                // New Item
                else
                {
                    string customData = Me.CustomData;
                    string newData = "";
                    string itemType = item.Type.ToString();
                    if (itemType.Contains("Ingot"))
                    {
                        Echo($"Adding ingot; Name: {itemName}; Type:{itemType}");
                        newData = "Ingot;" + itemName + "\n";
                        ingots[itemName] = new Item{Name=itemName};
                    }
                    else if (itemType.Contains("Ore"))
                    {
                        Echo($"Adding ore; Name: {itemName}; Type:{itemType}");
                        newData = "Ore;" + itemName + "\n";
                        ore[itemName] = new Item{Name=itemName};
                    }
                    Me.CustomData += newData;
                    //if (!customData.Contains(newData)){Me.CustomData += newData;}
                }
            }
        }
    }
}

public void Organize_Inventories()
{
    // Get all the blocks on the ship
    List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
    if (IgnoreOtherGrids == true)
    {
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(allBlocks, block =>
        block.CubeGrid == Me.CubeGrid && block.HasInventory);
    }
    else{GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(allBlocks, block => block.HasInventory);}

    // Iterate through all blocks and add their inventories to inventory lists
    List<IMyInventory> allInventories = new List<IMyInventory>();
    List<IMyInventory> cargoInventories = new List<IMyInventory>();
    List<IMyInventory> ingotInventories = new List<IMyInventory>();
    List<IMyInventory> oreInventories = new List<IMyInventory>();
    List<IMyInventory> componentInventories = new List<IMyInventory>();
    List<IMyInventory> equipmentInventories = new List<IMyInventory>();
    List<IMyInventory> ammoInventories = new List<IMyInventory>();
    
    foreach (IMyTerminalBlock block in allBlocks)
    {
        for (int i = 0; i < block.InventoryCount; i++)
        {
            IMyInventory inventory = block.GetInventory(i);
            allInventories.Add(inventory);

            if (block is IMyCargoContainer)
            {
                double currentVolume = (double)inventory.CurrentVolume;
                double maxVolume = (double)inventory.MaxVolume;
                double percentageFull = (currentVolume / maxVolume) * 100;
                cargoInventories.Add(inventory);
            }
            if (inventory.Owner.ToString().Contains(IngotCargo))
            {
                double currentVolume = (double)inventory.CurrentVolume;
                double maxVolume = (double)inventory.MaxVolume;
                double percentageFull = (currentVolume / maxVolume) * 100;
                if (percentageFull < 98.0){ingotInventories.Add(inventory);}
            }
            else if (inventory.Owner.ToString().Contains(OreCargo))
            {
                double currentVolume = (double)inventory.CurrentVolume;
                double maxVolume = (double)inventory.MaxVolume;
                double percentageFull = (currentVolume / maxVolume) * 100;
                if (percentageFull < 98.0){oreInventories.Add(inventory);}
            }
            else if (inventory.Owner.ToString().Contains(ComponentCargo))
            {
                double currentVolume = (double)inventory.CurrentVolume;
                double maxVolume = (double)inventory.MaxVolume;
                double percentageFull = (currentVolume / maxVolume) * 100;
                if (percentageFull < 98.0){componentInventories.Add(inventory);}
            }
            else if (inventory.Owner.ToString().Contains(EquipmentCargo))
            {
                double currentVolume = (double)inventory.CurrentVolume;
                double maxVolume = (double)inventory.MaxVolume;
                double percentageFull = (currentVolume / maxVolume) * 100;
                if (percentageFull < 98.0){equipmentInventories.Add(inventory);}
            }
            else if (inventory.Owner.ToString().Contains(AmmoCargo))
            {
                double currentVolume = (double)inventory.CurrentVolume;
                double maxVolume = (double)inventory.MaxVolume;
                double percentageFull = (currentVolume / maxVolume) * 100;
                if (percentageFull < 98.0){ammoInventories.Add(inventory);}
            }
        }
    }
    if (cargoInventories.Count == 0){Echo("All cargo containers full or none available");return;}
    
    // Move items to correct containers
    foreach (IMyInventory inventory in allInventories)
    {
        // Check if inventory should be skipped
		
        IMyEntity ownerEntity = inventory.Owner;
        IMyTerminalBlock ownerBlock = (IMyTerminalBlock)ownerEntity;
        string blockName = ownerBlock.CustomName;
		
		// Skip assemblers with items queued
        if (ownerBlock is IMyAssembler)
        {
            IMyAssembler assembler = (IMyAssembler)ownerBlock;
            if (!assembler.IsQueueEmpty){continue;}
        }
		
		// Skip refinery first inventory
        if (ownerBlock is IMyRefinery)
        {
            IMyRefinery refinery = (IMyRefinery)ownerBlock;
            if (inventory == refinery.GetInventory(0)){continue;}
        }
		
		// Check if the ownerBlock matches any turret type
        if (ownerBlock is IMyLargeGatlingTurret ||
            ownerBlock is IMyLargeMissileTurret ||
            ownerBlock is IMyLargeInteriorTurret ||
            ownerBlock is IMySmallGatlingGun ||
            ownerBlock is IMySmallMissileLauncher)
		{continue;}
		
        else if (ownerBlock is IMyGasGenerator){continue;}  // Skip all H2/O2 generators
        else if (ownerBlock is IMyReactor){continue;}  // Skip all reactors

        List<MyInventoryItem> items = new List<MyInventoryItem>();
        inventory.GetItems(items);

        foreach (MyInventoryItem item in items)
        {
            // Check if the item is already in a matching container
            string itemType = item.Type.ToString();
            string[] parts = itemType.Split('_', '/');
            itemType = parts.Length > 1 ? parts[1] : parts[0];

            if (itemType.Contains("Ingot") && blockName.Contains(IngotCargo)){continue;}
            else if (itemType.Contains("Ore") && blockName.Contains(OreCargo)){continue;}
            else if (itemType.Contains("Component") && blockName.Contains(ComponentCargo)){continue;}
            else if (itemType.Contains("Ammo") && blockName.Contains(AmmoCargo)){continue;}
            else if (blockName == EquipmentCargo){continue;}

            bool transferred = false;

            // Ingot
            if (itemType.Contains("Ingot"))
            {
                if (ingotInventories.Count == 0)
                {
                    if(inventory.Owner is IMyCargoContainer){continue;}  // Item is already in a cargo container
                    else  // Move item to cargo container 
                    {
                        for (int i=0; i < cargoInventories.Count; i++)
                        {
							transferred = inventory.TransferItemTo(cargoInventories[i], item);
                            if (transferred){break;}
                        }
                        if (!transferred)
							{Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                    }
                }
                else
                {
                    for (int i=0; i < ingotInventories.Count; i++)
                    {
						var inventoryOwner = ingotInventories[i].Owner as IMyTerminalBlock;
						if (!inventory.CanTransferItemTo(ingotInventories[i], item.Type))
							{Echo("No working conveyor connection to transfer " + item.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
						if (!ingotInventories[i].CanItemsBeAdded(item.Amount, item.Type))
							{Echo("Not enough cargo space to move " + itemType.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
                        transferred = inventory.TransferItemTo(ingotInventories[i], item);
                        if (transferred){break;}
                    }
                    if (!transferred){Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                }
            }

            // Ore
            else if (itemType.Contains("Ore"))
            {
                if (oreInventories.Count == 0)
                {
                    if(inventory.Owner is IMyCargoContainer){continue;}  // Item is already in a cargo container
                    else  // Move item to cargo container 
                    {
                        for (int i=0; i < cargoInventories.Count; i++)
                        {
							transferred = inventory.TransferItemTo(cargoInventories[i], item);
                            if (transferred){break;}
                        }
                        if (!transferred)
							{Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                    }
                }
                else
                {
                    for (int i=0; i < oreInventories.Count; i++)
                    {
						var inventoryOwner = oreInventories[i].Owner as IMyTerminalBlock;
						if (!inventory.CanTransferItemTo(oreInventories[i], item.Type))
							{Echo("No working conveyor connection to transfer " + item.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
						if (!oreInventories[i].CanItemsBeAdded(item.Amount, item.Type))
							{Echo("Not enough cargo space to move " + itemType.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
                        transferred = inventory.TransferItemTo(oreInventories[i], item);
                        if (transferred){break;}
                    }
                    if (!transferred){Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                }
            }

            // Component
            else if (itemType.Contains("Component"))
            {
                if (componentInventories.Count == 0)
				{
                    if(inventory.Owner is IMyCargoContainer){continue;}  // Item is already in a cargo container
                    else  // Move item to cargo container 
                    {
                        for (int i=0; i < cargoInventories.Count; i++)
                        {
							transferred = inventory.TransferItemTo(cargoInventories[i], item);
                            if (transferred){break;}
                        }
                        if (!transferred)
							{Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                    }
                }
                else
                {
                    for (int i=0; i < componentInventories.Count; i++)
                    {
						var inventoryOwner = componentInventories[i].Owner as IMyTerminalBlock;
						if (!inventory.CanTransferItemTo(componentInventories[i], item.Type))
							{Echo("No working conveyor connection to transfer " + item.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
						if (!componentInventories[i].CanItemsBeAdded(item.Amount, item.Type))
							{Echo("Not enough cargo space to move " + itemType.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
                        transferred = inventory.TransferItemTo(componentInventories[i], item);
                        if (transferred){break;}
                    }
                    if (!transferred){Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                }
            }

            // Ammo
            else if (itemType.Contains("Ammo"))
            {
                if (ammoInventories.Count == 0)
                {
                    if(inventory.Owner is IMyCargoContainer){continue;}  // Item is already in a cargo container
                    else  // Move item to cargo container 
                    {
                        for (int i=0; i < cargoInventories.Count; i++)
                        {
                            transferred = inventory.TransferItemTo(cargoInventories[i], item);
                            if (transferred){break;}
                        }
                        if (!transferred){Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                    }
                }
                else
                {
                    for (int i=0; i < ammoInventories.Count; i++)
                    {
						var inventoryOwner = ammoInventories[i].Owner as IMyTerminalBlock;
						if (!inventory.CanTransferItemTo(ammoInventories[i], item.Type))
							{Echo("No working conveyor connection to transfer " + item.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
						if (!ammoInventories[i].CanItemsBeAdded(item.Amount, item.Type))
							{Echo("Not enough cargo space to move " + itemType.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
                        transferred = inventory.TransferItemTo(ammoInventories[i], item);
                        if (transferred){break;}
                    }
                    if (!transferred){Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                }
            }

            // Equipment
            else
            {
                if (equipmentInventories.Count == 0)
                {
                    if(inventory.Owner is IMyCargoContainer){continue;}  // Item is already in a cargo container
                    else  // Move item to cargo container 
                    {
                        for (int i=0; i < cargoInventories.Count; i++)
                        {
							transferred = inventory.TransferItemTo(cargoInventories[i], item);
                            if (transferred){break;}
                        }
                        if (!transferred)
							{Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                    }
                }
                else
                {
                    for (int i=0; i < equipmentInventories.Count; i++)
                    {
						var inventoryOwner = equipmentInventories[i].Owner as IMyTerminalBlock;
						if (!inventory.CanTransferItemTo(equipmentInventories[i], item.Type))
							{Echo("No working conveyor connection to transfer " + item.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
						if (!equipmentInventories[i].CanItemsBeAdded(item.Amount, item.Type))
							{Echo("Not enough cargo space to move " + itemType.ToString() + " from " + ownerBlock.CustomName + " to " + inventoryOwner.CustomName);return;}
                        transferred = inventory.TransferItemTo(equipmentInventories[i], item);
                        if (transferred){break;}
                    }
                    if (!transferred){Echo("Unable to transfer " + item.ToString() + " from " + ownerBlock.CustomName);return;}
                }
            }
        }
    }
}

public void Assemble_Disassemble()
{
    // Verify master assembler is on
    List<IMyAssembler> masterAssemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType(masterAssemblers, block => block.CustomName == AssemblerName && block.CubeGrid == Me.CubeGrid);
    IMyAssembler assembler = masterAssemblers.FirstOrDefault();
    if (assembler == null)
    {
        Echo($"Assembler named {AssemblerName} not found");
        return;
    }
    if (assembler.Enabled == false)
    {
        Echo($"Error: {AssemblerName} is off");
        return;
    }
    
    // Check if any assembler has an item in its queue and return
    List<IMyAssembler> assemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
    foreach (IMyAssembler currentAssembler in assemblers)
    {
        if (!currentAssembler.IsQueueEmpty)
        {
            Echo($"{currentAssembler.CustomName} has an item in its queue");
            return;
        }
    }

    // Components
    foreach (var kvp in components)
    {
        if (kvp.Value.Actual < kvp.Value.Desired && AssembleItems == true)
        {
            Assemble_Item(kvp.Value.Name, kvp.Value.Actual, kvp.Value.Desired, kvp.Value.Blueprint);
        }
        else if (kvp.Value.Actual > kvp.Value.Desired && DisassembleItems == true)
        {
            Disassemble_Item(kvp.Value.Name, kvp.Value.Actual, kvp.Value.Desired, kvp.Value.Blueprint);
        }
    }
    // Equipment
    foreach (var kvp in equipment)
    {
        if (kvp.Value.Actual < kvp.Value.Desired && AssembleItems == true)
        {
            Assemble_Item(kvp.Value.Name, kvp.Value.Actual, kvp.Value.Desired, kvp.Value.Blueprint);
        }
        else if (kvp.Value.Actual > kvp.Value.Desired && DisassembleItems == true)
        {
            Disassemble_Item(kvp.Value.Name, kvp.Value.Actual, kvp.Value.Desired, kvp.Value.Blueprint);
        }
    }
    // Ammo
    foreach (var kvp in ammo)
    {
        if (kvp.Value.Actual < kvp.Value.Desired && AssembleItems == true)
        {
            Assemble_Item(kvp.Value.Name, kvp.Value.Actual, kvp.Value.Desired, kvp.Value.Blueprint);
        }
        else if (kvp.Value.Actual > kvp.Value.Desired && DisassembleItems == true)
        {
            Disassemble_Item(kvp.Value.Name, kvp.Value.Actual, kvp.Value.Desired, kvp.Value.Blueprint);
        }
    }
}

public bool Assemble_Item(string name, int actual, int desired, string bp)
{
    // Get ID for assembler queue
    MyDefinitionId id = MyDefinitionId.Parse($"MyObjectBuilder_BlueprintDefinition/{bp}");

    // Get amount to assemble
    double amount = desired - actual;

    List<IMyAssembler> assemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);

    foreach (IMyAssembler currentAssembler in assemblers){currentAssembler.Mode = MyAssemblerMode.Assembly;}

    // If the item is not found in any assembler's queue, add it to the queue of the master assembler
    List<IMyAssembler> masterAssemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType(masterAssemblers, block => block.CustomName == AssemblerName && block.CubeGrid == Me.CubeGrid);
    IMyAssembler assembler = masterAssemblers.FirstOrDefault();
    if (assembler != null)
    {
        try
        {
            assembler.AddQueueItem(id, amount);
            Echo($"Production queued for {amount}x {name} in {assembler.CustomName}");
        }
        catch (Exception e)
        {
            Echo($"Can't assemble {name}");
            return false;
        }
    }
    else
    {
        Echo($"No assemblers found. {name} cannot be queued.");
        return false;
    }
    return true;
}

public bool Disassemble_Item(string name, int actual, int desired, string bp)
{
    // Get ID for assembler queue
    MyDefinitionId id = MyDefinitionId.Parse($"MyObjectBuilder_BlueprintDefinition/{bp}");

    // Get amount to disassemble
    double amount = actual - desired;

    // If the item is not found in any assembler's queue, add it to the queue of the master assembler
    List<IMyAssembler> masterAssemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType(masterAssemblers, block => block.CustomName == AssemblerName && block.CubeGrid == Me.CubeGrid);
    IMyAssembler assembler = masterAssemblers.FirstOrDefault();
    if (assembler != null)
    {
        try
        {
            assembler.Mode = MyAssemblerMode.Disassembly;
            assembler.AddQueueItem(id, amount);
            Echo($"Production queued for {amount}x {name} in {assembler.CustomName}");
        }
        catch (Exception e1)
        {
            Echo($"Can't disassemble {name}");
            return false;
        }
    }
    else
    {
        Echo($"No assemblers found. {name} cannot be queued.");
        return false;
    }
    return true;
}

public void Settings_Output()
{
    // Format the results as a string and output to an LCD
    var output = new StringBuilder();

    // Headers
    output.AppendLine("Cinder's Inventory Manager Settings");
    output.AppendLine("===================================");
    output.AppendLine($"{"Setting",-25} {"Value"}\n");

    output.AppendLine($"{"Assemble Components:",-25} {AssembleItems}");
    output.AppendLine($"{"Disassemble Components:",-25} {DisassembleItems}");
    output.AppendLine($"{"Organize Inventories:",-25} {OrganizeInventories}");
    output.AppendLine($"{"Ignore Other Grids:",-25} {IgnoreOtherGrids}");
    output.AppendLine($"{"Learn Assembler Item:",-25} {LearnAssemblerItem}");
    
    var lcds = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, block =>
        block.CubeGrid == Me.CubeGrid && block.CustomName == SettingsLCD);
    foreach (var lcd in lcds)
    {
        var textPanel = lcd as IMyTextPanel;
        textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
        textPanel.Font = "Monospace";
        textPanel.FontColor = new Color(0, 90, 255);
        textPanel.WriteText(output.ToString());
    }
}

public void Ingots_Output()
{
    // Format the results as a string and output to an LCD
    var output = new StringBuilder();

    // Headers
    output.AppendLine("Cinder's Ingot Display");
    output.AppendLine("======================");
    output.AppendLine($"{"Ingot",-15} {"Actual"}\n");

    foreach (var kvp in ingots)
    {
        Item value = kvp.Value;
        string name = value.Name;
        if (name.Contains("Stone")){name = name.Replace("Stone", "Gravel");}
        if (name.Contains("Ingot")){name = name.Replace("Ingot", "");}
        output.AppendLine($"{name,-15} {value.Actual.ToString("N0")}");
    }
    var lcds = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, block =>
        block.CubeGrid == Me.CubeGrid && block.CustomName == IngotLCD);
    foreach (var lcd in lcds)
    {
        var textPanel = lcd as IMyTextPanel;
        textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
        textPanel.Font = "Monospace";
        textPanel.FontColor = new Color(0, 90, 255);
        textPanel.WriteText(output.ToString());
    }
}

public void Ore_Output()
{
    // Format the results as a string and output to an LCD
    var output = new StringBuilder();

    // Headers
    output.AppendLine("Cinder's Ore Display");
    output.AppendLine("====================");
    output.AppendLine($"{"Ore",-15} {"Actual"}\n");

    foreach (var kvp in ore)
    {
        Item value = kvp.Value;
        output.AppendLine($"{value.Name,-15} {value.Actual.ToString("N0")}");
    }
    var lcds = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, block =>
        block.CubeGrid == Me.CubeGrid && block.CustomName == OreLCD);
    foreach (var lcd in lcds)
    {
        var textPanel = lcd as IMyTextPanel;
        textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
        textPanel.Font = "Monospace";
        textPanel.FontColor = new Color(0, 90, 255);
        textPanel.WriteText(output.ToString());
    }
}

public void Components_Output()
{
    // Format the results as a string and output to an LCD
    var output = new StringBuilder();

    // Headers
    output.AppendLine("Cinder's Component Display");
    output.AppendLine("==========================");
    output.AppendLine($"{"Component",-30} {"Actual",-10} {"Desired"}\n");

    foreach (var kvp in components)
    {
        Item value = kvp.Value;
        output.AppendLine($"{value.Name,-30} {value.Actual.ToString("N0"),-10} {value.Desired.ToString("N0")}");
    }
    var lcds = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, block =>
        block.CubeGrid == Me.CubeGrid && block.CustomName == ComponentLCD);
    foreach (var lcd in lcds)
    {
        var textPanel = lcd as IMyTextPanel;
        textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
        textPanel.Font = "Monospace";
        textPanel.FontColor = new Color(0, 90, 255);
        textPanel.WriteText(output.ToString());
    }
}

public void Equipment_Output()
{
    // Format the results as a string and output to an LCD
    var output = new StringBuilder();

    // Headers
    output.AppendLine("Cinder's Equipment Display");
    output.AppendLine("==========================");
    output.AppendLine($"{"Item",-20} {"Actual",-10} {"Desired"}\n");

    foreach (var kvp in equipment)
    {
        Item value = kvp.Value;
        string name = value.Name;
        if (name.Contains("Item")){name = name.Replace("Item", "");}
        if (name.Contains("4")){name = name.Replace("4", "Elite");}
        output.AppendLine($"{name,-20} {value.Actual.ToString("N0"),-10} {value.Desired.ToString("N0")}");
    }
    var lcds = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, block =>
        block.CubeGrid == Me.CubeGrid && block.CustomName == EquipmentLCD);
    foreach (var lcd in lcds)
    {
        var textPanel = lcd as IMyTextPanel;
        textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
        textPanel.Font = "Monospace";
        textPanel.FontColor = LCDColor;
        textPanel.WriteText(output.ToString());
    }
}

public void Ammo_Output()
{
    // Format the results as a string and output to an LCD
    var output = new StringBuilder();

    // Headers
    output.AppendLine("Cinder's Ammo Display");
    output.AppendLine("=====================");
    output.AppendLine($"{"Item",-30} {"Actual",-10} {"Desired"}\n");

    foreach (var kvp in ammo)
    {
        Item value = kvp.Value;
        string name = value.Name;
        if (name.Contains("Magazine")){name = name.Replace("Magazine", "");}
        output.AppendLine($"{name,-30} {value.Actual.ToString("N0"),-10} {value.Desired.ToString("N0")}");
    }
    var lcds = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, block =>
        block.CubeGrid == Me.CubeGrid && block.CustomName == AmmoLCD);
    foreach (var lcd in lcds)
    {
        var textPanel = lcd as IMyTextPanel;
        textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
        textPanel.Font = "Monospace";
        textPanel.FontColor = LCDColor;
        textPanel.WriteText(output.ToString());
    }
}