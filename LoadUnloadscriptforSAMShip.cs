 // Version 1.0  SAM LOADER_UNLAODER script
        // You MUST have a Program block with SAM on the grid to use this script.  Note that loading/unloading is not done by this script.  If needed, I have another script that runs on the station for this. 
        // This scripts waits until all containers tagged with the tag "to" in custom data are FULL or EMPTY.   When this is reached, it calls the SAM program block to fly to the next destination. 
        // It will change the action from loading to unloading each time.  The ship will thus fly to A and load, wait untill full, then to B and unload, wait until empty.  
        // Command possible are RUN, WAIT, SKIP, LOAD
        // RUN =  default program: loading, waiting, fly, unload, wait, fly in a loop
        // WAIT = stay where you are, do nothing for now. (pauze).  Note that loading and unloading will continue, but no flying. 
        // SKIP = ignore whether full or empty, fly off to the next stop.  It will perform the next action, being loading or unloading
        // LOAD = run this the first time to get your display filled with info.  It then goes into waiting mode.


        string LCD_Name = "Load3";  //You must have an LCD with this name. Can be changed here.  Case sensitive !
        string samsblock = "[SAM]"; //The program block that runs SAM should have this in the CUSTOM DATA.  You can change it here. 
        // The containers you want to have filled and emptied should have "to" in their CUSTOM DATA.   These will be checked if they are full, other ones are ignored.  If you want another tag, I indicated in the script below where to change it.
        string samcommand = "START NEXT";  //The SAM command that is given when full or empty.  Default START NEXT just goes to the next waypoint.  You should have 2 set of course

        // Changes from here are on your own risk but could be exciting :)

        List<IMyCargoContainer> FROMContainers = new List<IMyCargoContainer>();
        List<IMyCargoContainer> TOContainers = new List<IMyCargoContainer>();
        List<IMyProgrammableBlock> Programblocks = new List<IMyProgrammableBlock>();
        IMyTextSurfaceProvider MyLCD;
        IMyProgrammableBlock PBsam;
        string lcdtxt = "";
        float capacity = 0;
        float usedcapacity = 0;
        string waitingfor = "full";
        string state = "loading";


        public Program()
        {
            MyLCD = GridTerminalSystem.GetBlockWithName(LCD_Name) as IMyTextSurfaceProvider;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            switch (argument.ToLower())
            {
                case "load":
                    state = "waiting";
                    LOAD();
                    break;
                case "wait":
                    state = "waiting";
                    break;
                case "run":
                    state = "loading";
                    break;
                case "skip":  //we skip the current phase and go to the next destination
                    if(waitingfor == "full")
                    { waitingfor = "empty"; }
                    else
                    { waitingfor = "full"; }
                    dosamscreen();
                    break;
            }

            switch (state)
            {
                case "waiting":
                 waitplease(); 
                 break;
                case "loading":
                   LOAD(); 
                break;
            }
        }

        public void LOAD()
        {
            UpdateContainers();
            CalcStorage();

        }

        public void waitplease()
        {
            lcdtxt = "We are in waiting mode,  previous mode was: " + waitingfor;

            lcdtxt += "\nTO containers : " + TOContainers.Count + "\n FROM containers : " + FROMContainers.Count;
            MyLCD.GetSurface(0).WriteText(lcdtxt);

        }
        public void UpdateContainers()
        {
            List<IMyCargoContainer> AllContainers = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(AllContainers, block => block.IsSameConstructAs(Me));
            FROMContainers.Clear();
            TOContainers.Clear();

            for (int i = 0; i < AllContainers.Count; i++)

            {
                switch (AllContainers[i].CustomData.ToLower())
                {
                    case "to" :  // Change this if you want another TAG for your containers
                        TOContainers.Add(AllContainers[i]);
                        break;

                    default:
                        FROMContainers.Add(AllContainers[i]);
                        break;
                }
            }
            lcdtxt = "We are waiting for containers to be: " + waitingfor;

            lcdtxt += "\nTO containers : " + TOContainers.Count + "\n FROM containers : " + FROMContainers.Count;
            MyLCD.GetSurface(0).WriteText(lcdtxt);

        }


        public void CalcStorage()
        {
            capacity = 0;
            usedcapacity = 0;

            for (int i = 0; i < TOContainers.Count; i++)
            {
                capacity += (float)TOContainers[i].GetInventory(0).MaxVolume;
                usedcapacity += (float)TOContainers[i].GetInventory(0).CurrentVolume;
            }
            lcdtxt += "\nCapacity: " + capacity + "\n Used: " + usedcapacity;

            if ((capacity - usedcapacity) < 100)  //We are full ! 
            { lcdtxt += "\nWe are FULL";
                MyLCD.GetSurface(0).WriteText(lcdtxt);
                if(waitingfor == "full")  //are we waiting to be full ? 
                {
                    waitingfor = "empty";  //Now we wait to be empty to trigger again to fly
                    dosamscreen();  //Fly to the next point
                }

            }
            else //We are not full
            { if(usedcapacity == 0)   //Are we empty ? 
                {
                    if (waitingfor == "empty")  //are we waiting to be empty ?
                    {
                        waitingfor = "full";  //Now we wait to be full to trigger again to fly
                        dosamscreen();  //Fly to the next point
                    }
                } 
              else //We are not empty
                { 
                lcdtxt += "\nWe have " + (capacity - usedcapacity) +" left, so we wait";
                MyLCD.GetSurface(0).WriteText(lcdtxt);
                }
            }
           
        }

        public void callsam()
        {

            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(Programblocks, block => block.IsSameConstructAs(Me));
            //block.ApplyAction("Run", argumentsList)
            for (int i = 0; i < Programblocks.Count; i++)

            {
                if (Programblocks[i].CustomName.Contains(samsblock))
                {
                    lcdtxt += "\nPB " + Programblocks[i].CustomName + "is the one";
                    PBsam = Programblocks[i];
                    break;
                }
                else
                {
                    lcdtxt += "\nNothing found !";
                }
            }
            MyLCD.GetSurface(0).WriteText(lcdtxt);

        }

        public void dosamscreen()
        {
            callsam();
            PBsam.TryRun(samcommand);   


        }
