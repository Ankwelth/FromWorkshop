string OreContainerName = "[Storage][Ore] Container";
string IngotsContainerName = "[Storage][Ingots] Container";
string ComponentsContainerName = "[Storage][Components] Container";
string MiscContainerName = "[Storage][Misc] Container";

string FromTag = "[ToSort]";

public Program() 
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
} 

void Main() {
   //Les containers de destinations
   IMyTerminalBlock oreContainer = GridTerminalSystem.GetBlockWithName(OreContainerName) as IMyTerminalBlock;
   IMyInventory inventoryOre = null;
   if(oreContainer == null || !oreContainer.HasInventory){
       Echo("No Ores Container");
       oreContainer = null;
   }
   else{
       inventoryOre = oreContainer.GetInventory(0);
   }
  
   IMyTerminalBlock ingotsContainer = GridTerminalSystem.GetBlockWithName(IngotsContainerName) as IMyTerminalBlock;
   IMyInventory inventoryIngots = null;
   if(ingotsContainer == null || !ingotsContainer.HasInventory){
       Echo("No Ingots Container");
       ingotsContainer = null;
   } 
   else{
       inventoryIngots = ingotsContainer.GetInventory(0);
   }

   IMyTerminalBlock componentsContainer = GridTerminalSystem.GetBlockWithName(ComponentsContainerName) as IMyTerminalBlock;
   IMyInventory inventoryComponents = null;
   if(componentsContainer == null || !componentsContainer.HasInventory){
       Echo("No Components Container");
       componentsContainer = null;
   } 
   else{
       inventoryComponents = componentsContainer.GetInventory(0);
   }

   IMyTerminalBlock miscContainer = GridTerminalSystem.GetBlockWithName(MiscContainerName) as IMyTerminalBlock;
   IMyInventory inventoryMisc = null;
   if(miscContainer == null || !miscContainer.HasInventory){
       Echo("No Misc Container");
       oreContainer = null;
   } 
   else{
       inventoryMisc = miscContainer.GetInventory(0);
   }

   //Les containers d'origines
   List<IMyTerminalBlock> toSortList = new List<IMyTerminalBlock>();
   GridTerminalSystem.SearchBlocksOfName(FromTag,toSortList);
   if(toSortList == null || toSortList.Count == 0){
       Echo("Nothing To Sort");
       return;
   }
   for(int i = 0 ; i < toSortList.Count ; i++){
       IMyInventory inventory = null;
       if(!toSortList[i].HasInventory){
           continue;
       }
       inventory = toSortList[i].GetInventory(toSortList[i].InventoryCount - 1);
       //Récupérer les objets du container
       List<MyInventoryItem> ListItems = new List<MyInventoryItem>();
       inventory.GetItems(ListItems,filter);
       for(int j = 0; j < ListItems.Count ; j++){
           string itemType = getType(ListItems[j]);
           if(itemType == "Component" && inventoryComponents != null){
               if (!inventoryComponents.IsFull) {
                   inventory.TransferItemTo(inventoryComponents, j, null, true, null);
               }
               else{
                   Echo("Components Container is Full !");
               }
           }
           else if(itemType == "Ingot" && inventoryIngots!= null){
               if (!inventoryIngots.IsFull) {
                   inventory.TransferItemTo(inventoryIngots, j, null, true, null);
               }
               else{
                   Echo("Ingots Container is Full !");
               }          
           }
           else if(itemType == "Ore" && inventoryOre != null){
               if (!inventoryOre.IsFull) {
                   inventory.TransferItemTo(inventoryOre, j, null, true, null);
               }
               else{
                   Echo("Ore Container is Full !");
               }                    
           }
           else if(inventoryMisc != null){
               if (!inventoryMisc.IsFull) {
                   inventory.TransferItemTo(inventoryMisc, j, null, true, null);
               }
               else{
                   Echo("Misc Container is Full !");
               }                    
           }
       }
   }  

}

string getType( MyInventoryItem item){
   String[] aSplit = item.Type.TypeId.Split( '_' );
   return aSplit[aSplit.Length -1];
}

Boolean filter( MyInventoryItem item ){
   return true;
}
