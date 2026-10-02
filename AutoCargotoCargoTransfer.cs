void Main(string argument)
{
    String Cargo1="Small Cargo Container"; //Cargo Container for Inventory to be taken
    String Cargo2="Small Cargo Container 2"; //Cargo Container for Inventory to be put
    //---------------------------------------------------------------------------------------------------------------------------
    var cargoContainer = GridTerminalSystem.GetBlockWithName(Cargo1) as IMyCargoContainer;
    IMyInventory cargoInventory = cargoContainer.GetInventory(0);

    var cargoContainer2 = GridTerminalSystem.GetBlockWithName(Cargo2) as IMyCargoContainer;
    IMyInventory cargoInventory2 = cargoContainer2.GetInventory(0);

        for (int x = 0; x < 10; x++) {
            cargoInventory.TransferItemTo(cargoInventory2, 0, stackIfPossible: true);  
        }
    }