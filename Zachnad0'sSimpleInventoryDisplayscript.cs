/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

private readonly IMyTextPanel inventoryStockLcd;
private readonly IMyBlockGroup containerGroup;

public Program()
{
    inventoryStockLcd = GridTerminalSystem.GetBlockWithName("InventoryStockLcd") as IMyTextPanel;
    containerGroup = GridTerminalSystem.GetBlockGroupWithName("InventoryStockContainers");
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
    inventoryStockLcd.ContentType = ContentType.TEXT_AND_IMAGE;
    var containers = new List<IMyTerminalBlock>();
    containerGroup.GetBlocks(containers, b => true);
    var contents = "";
    foreach (IMyTerminalBlock container in containers)
    {
        for (int c = 0; c < container.GetInventory(0).ItemCount; c++)
        {
            contents += container.GetInventory(0).GetItemAt(c).Value.Type.SubtypeId + " - " + container.GetInventory(0).GetItemAt(c).Value.Amount + " \n";
        }
    }
    inventoryStockLcd.WriteText(contents);
}