   // This code is for STATIONS.    It will search for all containers (and only containers) that have the tag "to" in their custom data and consider them as targets. 
        // All the other containers are consider to be FROM.   The code will move all items from the targets into the TO containers until they are full. 
        // This code considers ALL containers on the grid, which means the station and all connected grids as well.   
        // This version does not provide any filtering or tagging of containers that should be skipped, so be aware that the script will move all it can find. 
        // I will provide some tagging to exclude containers in a future version.  
        // Note that if you have 2 ships docked and they have containers without the "to", also these will be emptied if possible....   Room for improvement.
        // USe this together with my other script for ships that checks loading and will make them fly away when full to make a full automated transport system
        // Big credit to this guys youtube. Most of this script is based on his scripting tutorial.  https://www.youtube.com/channel/UCFgBLdj1Oms0suCRfhSztGg

        // You must have an LCD on the station with this name.  You can change the name. 
        string LCD_Name = "Load";


        // Changes below are on your own risk. 

        List<IMyCargoContainer> FROMContainers = new List<IMyCargoContainer>();
        List<IMyCargoContainer> TOContainers = new List<IMyCargoContainer>();
        IMyTextSurfaceProvider MyLCD;
        string lcdtxt = "";

        public Program()
        {
            MyLCD = GridTerminalSystem.GetBlockWithName(LCD_Name) as IMyTextSurfaceProvider;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            LOAD();
        }

        public void LOAD()
        {
            UpdateContainers();
            SortStuff();
        }
        public void UpdateContainers()
        {
            List<IMyCargoContainer> AllContainers = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(AllContainers);
            FROMContainers.Clear();
            TOContainers.Clear();

            for (int i = 0; i < AllContainers.Count; i++)

            {
                switch (AllContainers[i].CustomData.ToLower())
                {
                    case "to":
                        TOContainers.Add(AllContainers[i]);
                        break;

                    default:
                        FROMContainers.Add(AllContainers[i]);
                        break;
                }
            }
            lcdtxt = "TO containers : " + TOContainers.Count + "\n FROM containers : " + FROMContainers.Count;
            MyLCD.GetSurface(0).WriteText(lcdtxt);

        }
        public void SortStuff()
        {
            MyInventoryItem Item;
            for (int i = 0; i < FROMContainers.Count; i++)
            {
                for (int x = FROMContainers[i].GetInventory(0).ItemCount - 1; x >= 0; x--)
                {
                    Item = FROMContainers[i].GetInventory(0).GetItemAt(x).Value;

                    for (int y = 0; y < TOContainers.Count; y++)
                    {
                        if ((TOContainers[y].GetInventory(0).MaxVolume - TOContainers[y].GetInventory(0).CurrentVolume) > Item.Amount * Item.Type.GetItemInfo().Volume)
                        {
                            FROMContainers[i].GetInventory(0).TransferItemTo(TOContainers[y].GetInventory(0), Item);
                            break;
                        }
                        else
                        {
                            FROMContainers[i].GetInventory(0).TransferItemTo(TOContainers[y].GetInventory(0), Item, (TOContainers[y].GetInventory(0).MaxVolume - TOContainers[y].GetInventory(0).CurrentVolume) * (MyFixedPoint)(1 / Item.Type.GetItemInfo().Volume));
                        }
                    }

                }
            }
        }
