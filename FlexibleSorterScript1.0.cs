string FromTag = "[ToSort]";

string OreDestinationTag = "[Ore]";
string IngotsDestinationTag = "[Ingots]";
string ComponentsDestinationTag = "[Components]";
string MiscDestinationTag = "[Misc]";

public Program() 
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
} 

void Main() {
   //Les containers de destinations
   List<IMyInventory> inventoryOreList = InitInventory(OreDestinationTag);
   List<IMyInventory> inventoryIngotsList = InitInventory(IngotsDestinationTag);
   List<IMyInventory> inventoryComponentsList = InitInventory(ComponentsDestinationTag);
   List<IMyInventory> inventoryMiscList = InitInventory(MiscDestinationTag);

   //Les containers d'origines
   List<IMyInventory> inventoryToSortList = InitInventory(FromTag);
   foreach(IMyInventory inventory in inventoryToSortList){
       //Récupérer les objets du container
       List<MyInventoryItem> ListItems = new List<MyInventoryItem>();
       inventory.GetItems(ListItems,filter);
       for(int j = 0; j < ListItems.Count ; j++){
           string itemType = getType(ListItems[j]);
           if(itemType == "Component" && hasElem(inventoryComponentsList)){
              transferItem(inventory,j,inventoryComponentsList,ComponentsDestinationTag);
           }
           else if(itemType == "Ingot" && hasElem(inventoryIngotsList)){
              transferItem(inventory,j,inventoryIngotsList,IngotsDestinationTag);
           }
           else if(itemType == "Ore" && hasElem(inventoryOreList)){
              transferItem(inventory,j,inventoryOreList,ComponentsDestinationTag);
           }
           else if(hasElem(inventoryMiscList)){
              transferItem(inventory,j,inventoryMiscList,ComponentsDestinationTag);
           }
       }
   }  

}

List<IMyInventory> InitInventory(string tag){
   //Initialiser les objets
   List<IMyInventory> toReturnInventoryList = new List<IMyInventory>();
   List<IMyTerminalBlock> blockList = new List<IMyTerminalBlock>();
   GridTerminalSystem.SearchBlocksOfName(tag,blockList);
   if(blockList == null || blockList.Count == 0){
       Echo("WARNING : No block found for tag : " + tag);
       return null;
   }
   foreach(IMyTerminalBlock block in blockList){
      if(block == null || !block.HasInventory){
         Echo("WARNING : block has no inventory");
         continue;
      } 
      else{
         toReturnInventoryList.Add(block.GetInventory(block.InventoryCount - 1));
      }
   }
   if(toReturnInventoryList == null || toReturnInventoryList.Count == 0){
       Echo("WARNING : No inventory found for tag : " + tag);
       return null;
   }
   return toReturnInventoryList;
}

void transferItem(IMyInventory from,int index,List<IMyInventory> destinationList,string tag){
    foreach(IMyInventory destination in destinationList){
       if(destination.IsFull){
          continue;
       }
       from.TransferItemTo(destination, index, null, true, null);
       return;
    }
    Echo("WARNING: "+tag+" inventories are full");
}

Boolean hasElem(List<IMyInventory> inventory){
    return inventory != null && inventory.Count != 0;
}

string getType( MyInventoryItem item){

   String[] aSplit = item.Type.TypeId.Split( '_' );
   return aSplit[aSplit.Length -1];
}

Boolean filter( MyInventoryItem item ){
   return true;
}
