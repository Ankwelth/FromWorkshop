//================================================//
//------------------------------------ INSTRUCTIONS ------------------------------------//
//================================================//
//This script will automate production of components and ammunition. It will order production of items until their amount in inventory meets the quota.
//1) Install Programmable Block on your ship
//2) Look at the itemsToProduce list in the config section below:
// 2.1) Modify desired amount of items to be auto-assembled
// 2.2) Adjust items priority - higher priority items will be produced first
//3) Go to Control Panel, select Programmable Block, click "Edit" button, paste the script there, click "OK", click "Run"
//================================================//
//-------------------------------- End of INSTRUCTIONS --------------------------------//
//================================================//

//================================================//
//------------------------------------ CONFIG ------------------------------------//
//================================================//

const int productionOrderAmountLimit = 20; //items will be added to production queue in batches not larger than this number
public List<ProductionOrder> ConfigureQuota()
{
    List<ProductionOrder> itemsToProduce = new List<ProductionOrder>();

    //Components
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/SteelPlate"), amount: 500, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/InteriorPlate"), amount: 500, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ConstructionComponent"), amount: 500, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ComputerComponent"), amount: 100, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/MetalGrid"), amount: 100, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Display"), amount: 100, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/GirderComponent"), amount: 100, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/SmallTube"), amount: 200, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/LargeTube"), amount: 100, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/MotorComponent"), amount: 500, priority: 1));

    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/PowerCell"), amount: 50, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/BulletproofGlass"), amount: 50, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/DetectorComponent"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ExplosivesComponent"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/GravityGeneratorComponent"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/MedicalComponent"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/RadioCommunicationComponent"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ReactorComponent"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/SolarCell"), amount: 50, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Superconductor"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ThrustComponent"), amount: 0, priority: 1));

    //Ammo
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0100_Missile200mm"), amount: 0, priority: 1)); //Rocket
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0070_UltimateAutomaticRifleGun_Mag_30rd"), amount: 0, priority: 1)); //MR-30E Rifle mag
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0080_NATO_25x184mmMagazine"), amount: 0, priority: 1)); //Gatling ammo
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0040_AutomaticRifleGun_Mag_20rd"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0030_ElitePistolMagazine"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0020_FullAutoPistolMagazine"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0060_PreciseAutomaticRifleGun_Mag_5rd"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0050_RapidFireAutomaticRifleGun_Mag_50rd"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0010_SemiAutoPistolMagazine"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0120_LargeCalibreAmmo"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0110_MediumCalibreAmmo"), amount: 0, priority: 1)); //assault cannon shell
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0090_AutocannonClip"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0140_LargeRailgunAmmo"), amount: 0, priority: 1));
    itemsToProduce.Add(new ProductionOrder(definitionId: MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Position0130_SmallRailgunAmmo"), amount: 0, priority: 1));

    return itemsToProduce;
}

//================================================//
//-------------------------------- End of CONFIG --------------------------------//
//================================================//

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100; //The number at the end represents how often the script updates. 1 is the fastest, 10 is "medium", 100 is the slowest. Slow updates means lesser impact on game performance
}

public void Main()
{
    List<ProductionOrder> itemsToProduce = ConfigureQuota();

    //get the list of items on the ship
    List<SimplifiedInventoryItem> itemsInGrid = GetAllItemsInGrids();

    //get the list of items that are currently in production queue
    List<MyProductionItem> productionQueue = GetProductionQueue();

    //calculate new production order
    List<ProductionOrder> nextItemsToProduce = DecideNextProductionItems(itemsToProduce, productionQueue, itemsInGrid);
    DisplayNextProductionItems(nextItemsToProduce);

    //add new order to production queue 
    QueueProduction(nextItemsToProduce);
}

void DisplayNextProductionItems(List<ProductionOrder> nextItemsToProduce)
{
    if (nextItemsToProduce == null) return;
    if (nextItemsToProduce.Count == 0) return;

    Echo("Items to produce:");
    foreach (var item in nextItemsToProduce)
    {
        Echo($"{item.DefinitionId.SubtypeName} {item.Amount}");
    }
}


//returns list of all items in the grid 
public List<SimplifiedInventoryItem> GetAllItemsInGrids()
{
    List<IMyTerminalBlock> allBlocksInGrid = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(allBlocksInGrid, b => b.CubeGrid == Me.CubeGrid);

    //get items in cargo
    List<MyInventoryItem> itemsInCargo = new List<MyInventoryItem>();
    itemsInCargo.AddRange(GetItemsInBlocks(allBlocksInGrid));

    //cast them to SimplifiedInventoryItem
    List<SimplifiedInventoryItem> simplifiedItemsInCargo = ConvertToSimplifiedInventory(itemsInCargo);

    //merge elements of inventory list by type to get single stacks of every item
    simplifiedItemsInCargo = MergeItemsOfSameType(simplifiedItemsInCargo);

    return simplifiedItemsInCargo;

}

//returns list of currently queued production items
public List<MyProductionItem> GetProductionQueue()
{
    //get all assemblers
    List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);

    List<MyProductionItem> queue = new List<MyProductionItem>();
    foreach (IMyAssembler assembler in assemblers)
    {
        if (assembler.Enabled & !assembler.IsQueueEmpty & assembler.Mode == MyAssemblerMode.Assembly)
        {
            List<MyProductionItem> auxQueue = new List<MyProductionItem>();
            assembler.GetQueue(auxQueue);
            queue.AddRange(auxQueue);
        }
    }

    return queue;
}

//returns next production order, calculated based on priority and difference between items in stock and desired production quota
public List<ProductionOrder> DecideNextProductionItems(List<ProductionOrder> itemsToProduce, List<MyProductionItem> productionQueue, List<SimplifiedInventoryItem> itemsInGrid)
{
    //Echo($"Calculating next production item...");
    List<ProductionOrder> nextItemsToProduce = new List<ProductionOrder>();
    itemsToProduce = itemsToProduce.Where(x => x.Amount > 0).ToList();

    //for each item in itemsToProduce calculate how many needs to be produced to reach quota
    foreach (var item in itemsToProduce)
    {

        var aux = itemsInGrid.Where(x => item.DefinitionId.ToString().Contains(x.Type.SubtypeId)).FirstOrDefault();
        uint amountInInventory = aux == null ? 0 : aux.Amount;
        uint amountInProduction = (uint)productionQueue.Where(x => item.DefinitionId == x.BlueprintId).FirstOrDefault().Amount.ToIntSafe();
        decimal amountToProduce = item.Amount - (decimal)amountInInventory - (decimal)amountInProduction;

        if (amountToProduce > 0)
        {
            ProductionOrder nextItem = new ProductionOrder(item.DefinitionId, (uint)amountToProduce);
            nextItem.QuotaFulfillmentPercentage = (float)(amountInInventory + amountInProduction * 100 / item.Amount);
            nextItemsToProduce.Add(nextItem);
        }
    }

    if (nextItemsToProduce.Count > 0)
    {
        nextItemsToProduce = nextItemsToProduce.OrderByDescending(x => x.Priority).ThenBy(x => x.QuotaFulfillmentPercentage).ToList();
    }
    else return null;

    return nextItemsToProduce;
}
public void QueueProduction(List<ProductionOrder> itemsToProduce = null)
{
    if (itemsToProduce == null)
    {
        Echo("Nothing to add to the production queue - quotas met");
        return; // if nothing is ordered - do nothingj
    }

    //let's limit the next order, as to not order too much at once in one assembler
    //var nextOrder = itemsToProduce.Select(order => { order.Amount = Math.Min(order.Amount, productionOrderAmountLimit); return order; }).ToList();

    foreach (var itemToProduce in itemsToProduce)
    {
        var amountToProduce = itemToProduce.Amount;

        //get viable assemblers
        List<IMyAssembler> assemblers = new List<IMyAssembler>();
        GridTerminalSystem.GetBlocksOfType(assemblers, block => block.IsSameConstructAs(Me));
        List<IMyAssembler> availableAssemblers = assemblers.Where(assembler => GetAssemblerQueueLength(assembler) < 2 && GetAssemblerQueuedItemsCount(assembler) < productionOrderAmountLimit && assembler.Enabled && !assembler.CooperativeMode).ToList();

        //order production to found assemblers
        foreach (var availableAssembler in availableAssemblers)
        {
            var amountToProduceInOneAssembler = Math.Min(amountToProduce, productionOrderAmountLimit);
            if (availableAssembler.CanUseBlueprint(itemToProduce.DefinitionId)) availableAssembler.AddQueueItem(itemToProduce.DefinitionId, amountToProduceInOneAssembler);
            amountToProduce -= amountToProduceInOneAssembler;
            if (amountToProduce <= 0) break;
        }



    }
}

//returns list of items in blocks
public List<MyInventoryItem> GetItemsInBlocks(List<IMyTerminalBlock> blocksWithInventory)
{
    List<MyInventoryItem> itemsInBlocks = new List<MyInventoryItem>();
    foreach (var block in blocksWithInventory)
    {
        //try to get all inventories from the block
        int i = 0;
        while (true)
        {
            try
            {
                IMyInventory blockInventory = block.GetInventory(i);
                if (blockInventory == null) break;
                List<MyInventoryItem> itemsInBlock = new List<MyInventoryItem>(); //declare a list of items in block
                blockInventory.GetItems(itemsInBlock); //fill the list
                itemsInBlocks.AddRange(itemsInBlock);    //add list of item in this block to list of items in all input blocks
                i++;
            }
            catch
            {
                break; //ignore exceptions and move on
            }
        }

    }

    return itemsInBlocks;
}

//merges list so that only one element of each type exists within
public List<SimplifiedInventoryItem> MergeItemsOfSameType(List<SimplifiedInventoryItem> listToMerge)
{

    // Group by Subtype and sum Amount
    List<SimplifiedInventoryItem> mergedList = listToMerge
            .GroupBy(item => item.Type.SubtypeId)
            .Select(group => new SimplifiedInventoryItem
            (
                amount: Convert.ToUInt32(group.Sum(item => item.Amount)),
                type: group.First().Type // Use the Type from the first item in the group
            ))
            .ToList();

    return mergedList;
}


//just for conveniance sake I add this method that converts MyInventoryItem list to a list of SimplifiedInventoryItem
public List<SimplifiedInventoryItem> ConvertToSimplifiedInventory(List<MyInventoryItem> listToConvert)
{
    List<SimplifiedInventoryItem> convertedList = new List<SimplifiedInventoryItem>();

    foreach (var listElement in listToConvert)
    {
        convertedList.Add(new SimplifiedInventoryItem(listElement.Type, (uint)listElement.Amount.ToIntSafe()));
    }

    return convertedList;
}

//this class proved to be required to merge all items in inventory list, as Amount property of MyInventoryItem is read only
public class SimplifiedInventoryItem
{
    public uint Amount { get; set; }
    public MyItemType Type { get; set; }

    public SimplifiedInventoryItem(MyItemType type, uint amount)
    {
        Amount = amount;
        Type = type;
    }
    public override string ToString()
    {
        return $"{String.Format("{0:#,##0}", this.Amount)}" + " " + this.Type.SubtypeId.ToString();
    }
}

public class ProductionOrder
{
    public decimal Amount { get; set; }
    public MyDefinitionId DefinitionId { get; set; }
    public int Priority { get; set; }
    public float QuotaFulfillmentPercentage { get; set; } = 0;

    public ProductionOrder(MyDefinitionId definitionId, uint amount, int priority = 1)
    {
        DefinitionId = definitionId;
        Amount = amount;
        Priority = priority;
    }
}

//returns assembler's queue length
public int GetAssemblerQueueLength(IMyAssembler assembler)
{
    List<MyProductionItem> queue = new List<MyProductionItem>();
    assembler.GetQueue(queue);
    return queue.Count;
}

//returns assembler's queue length
public int GetAssemblerQueuedItemsCount(IMyAssembler assembler)
{
    List<MyProductionItem> queue = new List<MyProductionItem>();
    assembler.GetQueue(queue);

    int queuedItemsCount = 0;
    foreach (var item in queue)
    {
        queuedItemsCount += (int)item.Amount;
    }
    return queuedItemsCount;
}