/*
 * R e a d m e
 * -----------
 * 
 * Simply insert new TargetEntries in the autoMarketCommodities list.
 * 
 * The script will automatically read your grid's inventory (without peeking into connected grids) and publish buy/sell requests accordingly in all the store blocks with the autoMarketTag in their name.
 * 
 * Target entries have the following signature:
 * TargetEntry(String name, String type, int targetQuantity, int pricePerUnit)
 * 
 * Note, the script runs roughly once per minute.
 * 
 * Feel free to modify this script to your liking.
 */

// This tag identifies the automarkets
private String autoMarketTag = "[AUTOMARKET]";

// This list regulates all target values for each commodity
private List<TargetEntry> autoMarketCommodities = new List<TargetEntry>() {
//  new TargetEntry("Item name","Ore/Ingot/Component/Ammo/Tool",target_quantity,price_per_unit),
//  new TargetEntry("Ice","Ore",0,100) // No comma on last element
};

// Below is the rest of the program
// This is a structure to keep track of target entries
private class TargetEntry {
    public String name;
    public String type;
    public int targetQuantity;
    public int pricePerUnit;

    public TargetEntry(String name, String type, int targetQuantity, int pricePerUnit) {
        this.name = name;
        this.type = type;
        this.targetQuantity = targetQuantity;
        this.pricePerUnit = pricePerUnit;
    }
}

// These are structures to store stuff down the line
private List<IMyTerminalBlock> terminalBlockList;
private List<IMyInventory> inventoryList;
private List<IMyStoreBlock> autoMarketList;
private List<MyStoreQueryItem> activeTransactions;
private Dictionary<String, long> itemToStoredQuantity;

// This is a trick to delay the program
int delayer = 100;

public Program() {
    // Get automarket list
    autoMarketList = new List<IMyStoreBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyStoreBlock>(autoMarketList, block => block.IsSameConstructAs(Me));

    foreach (IMyStoreBlock item in autoMarketList.ToImmutableArray<IMyStoreBlock>()) {
        if (!item.CustomName.Contains(autoMarketTag)) {
            autoMarketList.Remove(item);
        }
    }

    // Get inventories, if present
    terminalBlockList = new List<IMyTerminalBlock>();
    inventoryList = new List<IMyInventory>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(terminalBlockList, block => block.IsSameConstructAs(Me));

    foreach (IMyTerminalBlock item in terminalBlockList) {
        int inventoryCount = item.InventoryCount;
        if (inventoryCount == 1) {
            inventoryList.Add(item.GetInventory());
        } else if (inventoryCount > 1) {
            for (int i = 0; i < inventoryCount; i++) {
                inventoryList.Add(item.GetInventory(i));
            }
        }
    }

    Echo("Found " + inventoryList.Count + " inventories");

    // Prepare aux structures
    activeTransactions = new List<MyStoreQueryItem>();
    itemToStoredQuantity = new Dictionary<String, long>();
    foreach (TargetEntry item in autoMarketCommodities) {
        itemToStoredQuantity.Add(item.name+"-"+item.type, 0);
    }

    // Final checks
    if (autoMarketList.Count > 0) {
        Echo("All systems green, automarket ready to go");
    } else {
        Echo("ERROR: Missing automarket block!\nCheck if the block's name has the " + autoMarketTag + " tag");
    }

    // Runtime set
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource) {
    // Check if the wakeup was due to timer
    if ((updateSource & UpdateType.Update100) > 0) {
        // Increment delayer
        delayer = delayer + 1;

        // Check if it's time to wake up
        if (delayer > 50) {
            // Reset the delayer
            delayer = 0;

            // Zero all inventory data
            Echo("Zeroing data");
            foreach (String item in itemToStoredQuantity.Keys.ToImmutableArray()) {
                itemToStoredQuantity[item] = 0;
            }

            // Gather data on linked inventories
            Echo("Recalculating inventories");
            foreach (IMyInventory inventory in inventoryList) {
                foreach (TargetEntry item in autoMarketCommodities) {
                    itemToStoredQuantity[item.name+"-"+item.type] = itemToStoredQuantity[item.name + "-" + item.type] + inventory.GetItemAmount(new MyItemType("MyObjectBuilder_" + item.type, item.name)).ToIntSafe();
                }
            }
            foreach (String item in itemToStoredQuantity.Keys) {
                Echo(" - Found " + item + " : " + itemToStoredQuantity[item]);
            }

            // Manipulate active transactions
            // By default, all automarket blocks are synchronized
            Echo("Clearing market");
            foreach (IMyStoreBlock store in autoMarketList) {
                // Clear all store listings
                // This is necessary since we can't distinguish between offer and requests
                store.GetPlayerStoreItems(activeTransactions);
                foreach (MyStoreQueryItem transaction in activeTransactions) {
                    store.CancelStoreItem(transaction.Id);
                }
                activeTransactions.Clear();
            }

            // For each target, check against all inventories and output a request/offer
            Echo("Manipulating market");
            foreach (TargetEntry item in autoMarketCommodities) {
                if (item.targetQuantity > itemToStoredQuantity[item.name + "-" + item.type]) {
                    // less than expected, buy the difference
                    foreach (IMyStoreBlock store in autoMarketList) {
                        long orderId;
                        MyStoreInsertResults returnValue;
                        returnValue = store.InsertOrder(new MyStoreItemDataSimple(MyDefinitionId.Parse("MyObjectBuilder_" + item.type + "/" + item.name), item.targetQuantity - (int)itemToStoredQuantity[item.name + "-" + item.type], item.pricePerUnit), out orderId);
                        Echo("Created order with id " + orderId + " and status " + returnValue + " for item " + item.name);
                        // If it failed, tell why
                        if (returnValue != MyStoreInsertResults.Success) {
                            Echo(" - Parser got " + MyDefinitionId.Parse("MyObjectBuilder_" + item.type + "/" + item.name).ToString());
                            Echo(" - Amount was " + (item.targetQuantity - (int)itemToStoredQuantity[item.name + "-" + item.type]));
                            Echo(" - Price was " + item.pricePerUnit);
                        }
                    }
                } else if (item.targetQuantity < itemToStoredQuantity[item.name + "-" + item.type]) {
                    // Surplus, sell the difference
                    foreach (IMyStoreBlock store in autoMarketList) {
                        int requestValue = 0;
                        if (itemToStoredQuantity[item.name + "-" + item.type] - item.targetQuantity > int.MaxValue)
                            requestValue = int.MaxValue;
                        else
                            requestValue = (int)itemToStoredQuantity[item.name + "-" + item.type] - item.targetQuantity;
                        long orderId;
                        MyStoreInsertResults returnValue;
                        returnValue = store.InsertOffer(new MyStoreItemDataSimple(MyDefinitionId.Parse("MyObjectBuilder_" + item.type + "/" + item.name), requestValue, item.pricePerUnit), out orderId);
                        Echo("Created offer with id " + orderId + " and status " + returnValue + " for item " + item.name);
                        // If it failed, tell why
                        if (returnValue != MyStoreInsertResults.Success) {
                            Echo(" - Parser got " + MyDefinitionId.Parse("MyObjectBuilder_" + item.type + "/" + item.name).ToString());
                            Echo(" - Amount was " + requestValue);
                            Echo(" - Price was " + item.pricePerUnit);
                        }
                    }
                }
            }
        }
    }
}