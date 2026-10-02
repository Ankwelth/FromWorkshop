/* 
        * R e a d m e 
        * ----------- 
        * This Script is trying to help setting Up your Custom Turret Controler.
        * It will search for Controlers on the same Grid and when the first rotor is already Setup, 
        * the Script will analyse the Subgrid on top of this rotor and will try to set the rest.
        *
        * It can also rename the Blocks to find them easier - if Reaneming is set to true, read this:
        *
        * Please Name CustomTurretControl like this: Grid_Prefix-CTC_Prefix-Blockname 
        * Example: 
        *           Grid Prefix:                    GunShip- 
        *           CustomTurretControl Prefix:       CTC-G-1- 
        *           Name                            GunShip-CTC-G-1-Custom Turret Control 
        */

        
        //SETUP 
        //Set Renaming true or false. If true, the script will rename the blocks as in the Example. If false, the script will only check the configuration and echo the status. 
        bool Renaming = false; 
        //Set Grid Prefix - If Grid_Prefix is empry, renaming is working without it
        string Grid_Prefix = ""; 
        //LCD Panel Tag for status output 
        string LCD_Panel_Tag = "[TURRET_STATUS]"; 
 
 
        //Set CustomTurretControl Prefix length 
        int CTC_Prefix_Length = 7;  

        /*#########################################################
        ############ NO CHANGES BELOW THIS LINE ######################
        #########################################################*/
 
        // Custom class to store rotor names (replaces Tuple which is prohibited in SE ingame scripts) 
        public class RotorPair 
        { 
            public string AzimuthRotorName; 
            public string ElevationRotorName; 
            public string Status; 
 
            public RotorPair(string azimuth, string elevation) 
            { 
                AzimuthRotorName = azimuth; 
                ElevationRotorName = elevation; 
                Status = ""; 
            } 
        } 

        Dictionary<int, string> Dic_CTC_Status = new Dictionary<int, string>{
    {1,"All OK"},
    {2,"Set up - no Ammunition"},
    {3,"No Camera"},
    {4,"No Tools or Guns"},
    {5,"No Elevator Rotor"},
    {6,"No Azimuth Rotor"}
    };

        // Dictionary to store expected controllers and their rotors
        Dictionary<string, RotorPair> ExpectedControllersAndRotors = new Dictionary<string, RotorPair>();

        //Function to Check which Ammunition is needed for the Gun and if there is Ammunition in the Inventory of the Gun
        public bool Check_Ammunition(IMyFunctionalBlock Gun)
        {
            string need_Ammunition = string.Empty
            ; if (Gun.BlockDefinition.ToString().Contains("Autocannon"))
            { need_Ammunition = "AutocannonClip"; }
            else if (Gun.BlockDefinition.ToString().Contains("SmallRailgun"))
            { need_Ammunition = "SmallRailgunAmmo"; }
            else if (Gun.BlockDefinition.ToString().Contains("MediumCalibreGun"))
            { need_Ammunition = "MediumCalibreAmmo"; }
            else if (Gun.BlockDefinition.ToString().Contains("SmallMissileLauncher"))
            { need_Ammunition = "Missile200mm"; }
            else if (Gun.BlockDefinition.ToString().Contains("Gatling"))
            { need_Ammunition = "NATO_25x184mm"; }

            if (Gun.HasInventory)
            {
                IMyInventory GunInventory;
                GunInventory = Gun.GetInventory();

                List<MyInventoryItem> Items = new List<MyInventoryItem>();
                GunInventory.GetItems(Items, A => A.Type.SubtypeId.Contains(need_Ammunition));
                if (Items.Count > 0)
                { return true; }
                else
                {
                    GunInventory.GetItems(Items);
                    if (Items.Count > 0)
                    {
                        //Echo(Items[0].Type.SubtypeId + " is in the Cargo for " + Gun.CustomName);
                    }
                }
            }
            //Echo(need_Ammunition + " is needed for " + Gun.CustomName);
            return false;
        }

        #region Checking Configuration of CustomTurretController
        //Function to Check if the CustomTurretControl is configurated correctly
        public int Check_CTC_Config(IMyTurretControlBlock CTC)
        {
            if (CTC.AzimuthRotor == null) //Check if Azimuth Rotor is set
            { return 6; }

            if (CTC.ElevationRotor == null) // Check if Elevator Rotor is set
            { return 5; }

            //Get Tool Blocks on Grid from Elevation Rotor
            List<IMyShipToolBase> Elevation_Grid_Tools = new List<IMyShipToolBase>();
            GridTerminalSystem.GetBlocksOfType<IMyShipToolBase>(Elevation_Grid_Tools, A => A.CubeGrid == CTC.ElevationRotor.TopGrid);

            //Get Gun Blocks on Grid from Elevation Rotor
            List<IMyUserControllableGun> Elevation_Grid_Guns = new List<IMyUserControllableGun>();
            GridTerminalSystem.GetBlocksOfType<IMyUserControllableGun>(Elevation_Grid_Guns, A => A.CubeGrid == CTC.ElevationRotor.TopGrid);

            //Get configurated Tools from CTC
            List<IMyFunctionalBlock> CTC_Tools = new List<IMyFunctionalBlock>();
            CTC.GetTools(CTC_Tools);

            if (CTC_Tools.Count() != (Elevation_Grid_Guns.Count() + Elevation_Grid_Tools.Count()) || CTC_Tools.Count() == 0) //Check if all Tool Blocks are set
            { return 4; }

            if (CTC.Camera == null) //Check if Camera is set
            { return 3; }

            foreach (IMyFunctionalBlock CTC_Tool in CTC_Tools) //Check if every Gun has Ammunition
            {
                if (!Check_Ammunition(CTC_Tool))
                { return 2; }
            }

            return 1; //All Setup
        }
        #endregion

        #region Renaming
        //Function to rename the Blocks of the CustomTurretController. Please Name the CustomTurretController as in the Example: Grid_Prefix-CTC_Prefix-Blockname. The Script will then rename all connected Blocks with the same Prefix.
        public void Renaming_CTC_Blocks(IMyTurretControlBlock CTC)
        {
            string CTC_Prefix = CTC.CustomName;
            if (Renaming)
            {
                if (!CTC.CustomName.Contains(Grid_Prefix) && Grid_Prefix != "")
                {
                    Echo("Please Name the CustomTurretController as in the Example");
                    return;
                }

                if(Grid_Prefix == "")
                {
                    CTC_Prefix = CTC.CustomName.Substring(1, CTC_Prefix_Length);
                }else
                {
                    CTC_Prefix = CTC.CustomName.Substring(Grid_Prefix.Length).Substring(0, CTC_Prefix_Length);
                }
            }

            if (CTC.AzimuthRotor != null)
            { CTC.AzimuthRotor.CustomName = Grid_Prefix + CTC_Prefix + CTC.AzimuthRotor.DefinitionDisplayNameText; }

            if (CTC.ElevationRotor != null)
            { CTC.ElevationRotor.CustomName = Grid_Prefix + CTC_Prefix + CTC.ElevationRotor.DefinitionDisplayNameText; }

            if (CTC.ElevationRotor != null)
            {
                List<IMyShipToolBase> Elevation_Grid_Tools = new List<IMyShipToolBase>();
                GridTerminalSystem.GetBlocksOfType<IMyShipToolBase>(Elevation_Grid_Tools, A => A.CubeGrid == CTC.ElevationRotor.TopGrid);
                
                List<IMyUserControllableGun> Elevation_Grid_Guns = new List<IMyUserControllableGun>();
                GridTerminalSystem.GetBlocksOfType<IMyUserControllableGun>(Elevation_Grid_Guns, A => A.CubeGrid == CTC.ElevationRotor.TopGrid);

                if (Elevation_Grid_Tools != null)
                {
                    foreach (IMyShipToolBase Tool in Elevation_Grid_Tools)
                    { Tool.CustomName = Grid_Prefix + CTC_Prefix + Tool.DefinitionDisplayNameText; }
                }
                
                if (Elevation_Grid_Guns != null)
                {
                    foreach (IMyUserControllableGun Gun in Elevation_Grid_Guns)
                    { Gun.CustomName = Grid_Prefix + CTC_Prefix + Gun.DefinitionDisplayNameText; }
                }

                if (CTC.Camera != null)
                { CTC.Camera.CustomName = Grid_Prefix + CTC_Prefix + CTC.Camera.DefinitionDisplayNameText; }
            }         
        }
        #endregion

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        {
            List<IMyTurretControlBlock> CTCList = new List<IMyTurretControlBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTurretControlBlock>(CTCList, A => A.CubeGrid == Me.CubeGrid);
            // Save the list of controllers and their rotors
            ExpectedControllersAndRotors.Clear();
            foreach (IMyTurretControlBlock CTC in CTCList)
                {
                    string azimuthRotorName = CTC.AzimuthRotor != null ? CTC.AzimuthRotor.CustomName : "NULL";
                    string elevationRotorName = CTC.ElevationRotor != null ? CTC.ElevationRotor.CustomName : "NULL";
                    ExpectedControllersAndRotors[CTC.CustomName] = new RotorPair(azimuthRotorName, elevationRotorName);
                }
            StringBuilder saveData = new StringBuilder();
            foreach (var kvp in ExpectedControllersAndRotors)
            {
                saveData.AppendLine($"{kvp.Key}|{kvp.Value.AzimuthRotorName}|{kvp.Value.ElevationRotorName}");
            }
            Storage = saveData.ToString();
        }

        #region Main
        public void Main(string argument, UpdateType updateSource)
        {
            // Handle SAVE argument
            if (argument != null && argument.ToUpper() == "SAVE")
            {
                Save();
                Echo("Status saved");
                return;
            }

            // Load expected controllers and rotors from storage
            if (!string.IsNullOrEmpty(Storage))
            {
                ExpectedControllersAndRotors.Clear();
                string[] lines = Storage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        ExpectedControllersAndRotors[parts[0]] = new RotorPair(parts[1], parts[2]);
                    }
                }
            }
            //Get CustomTurretControllers on Grid
            List<IMyTurretControlBlock> CTCList = new List<IMyTurretControlBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTurretControlBlock>(CTCList, A => A.CubeGrid == Me.CubeGrid);

            // Check for missing controllers and rotors
            CheckMissingControllersAndRotors(CTCList);

            if (CTCList.Count() == 0)
            {
                Echo("No CustomTurretController found");
                return;
            }

            foreach (IMyTurretControlBlock CTC in CTCList)
            {
                try
                {
                    string CTC_Prefix = CTC.CustomName;

                    //Renaming if enabled. Please Name the CustomTurretController as in the Example: Grid_Prefix-CTC_Prefix-Blockname.
                    if (Renaming)
                    {
                        Renaming_CTC_Blocks(CTC);
                    }

                    //Check Configuration of CustomTurretController and echo Status
                    int CTC_Status = Check_CTC_Config(CTC);
                    Echo("CTName: " + CTC_Prefix + " : " + Dic_CTC_Status[CTC_Status]);

                    //Work thoug the return Status Code of the Check_CTC_Config function. If the Status is 1, everything is set up correctly.
                    //If Code 6 is return, no Azimuth Rotor is set. The Script cannot find the Azimuth Rotor on the Grid, because it is needed to find the Elevator Rotor and the Tools and Guns on top of it. Please set the Azimuth Rotor and make sure it is connected to the CustomTurretController. The Script will then automatically find the Elevator Rotor and the Tools and Guns on top of it.
                    if (CTC_Status == 6)
                    {
                        Echo("Please set Azimuth Rotor");
                        continue;
                    }
                    
                    //If Code 5 is return, no Elevator Rotor is set. The Script will then try to find the Elevator Rotor on the Grid and set it as Elevator Rotor of the CustomTurretController.
                    if (CTC_Status == 5 && CTC.AzimuthRotor != null)
                    {
                        List<IMyMotorAdvancedStator> ElevationRotors = new List<IMyMotorAdvancedStator>();
                        GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(ElevationRotors, A => A.CubeGrid == CTC.AzimuthRotor.TopGrid);

                        if (ElevationRotors.Count() == 0)
                        {
                            Echo("No Elevation Rotor found");
                            continue;
                        }
                        else if (ElevationRotors.Count() > 1)
                        {
                            Echo("To many Elevation Rotors found");
                            continue;
                        }
                        else if (ElevationRotors.Count() == 1)
                        {
                            if (Renaming)
                            { ElevationRotors[0].CustomName = Grid_Prefix + "-" + CTC_Prefix + "-" + ElevationRotors[0].DefinitionDisplayNameText; }
                            CTC.ElevationRotor = (IMyMotorAdvancedStator)GridTerminalSystem.GetBlockWithId(ElevationRotors[0].EntityId);
                            continue;
                        }
                    }

                    //If Code 4 is return, no Tools or Guns are set. The Script will then try to find the Tools and Guns on top of the Elevator Rotor and set them as Tools of the CustomTurretController.
                    if (CTC_Status == 4 && CTC.ElevationRotor != null)
                    {
                        //Get Tool Blocks on Grid from Elevation Rotor
                        List<IMyShipToolBase> Elevation_Grid_Tools = new List<IMyShipToolBase>();
                        GridTerminalSystem.GetBlocksOfType<IMyShipToolBase>(Elevation_Grid_Tools, A => A.CubeGrid == CTC.ElevationRotor.TopGrid);

                        //Get Gun Blocks on Grid from Elevation Rotor
                        List<IMyUserControllableGun> Elevation_Grid_Guns = new List<IMyUserControllableGun>();
                        GridTerminalSystem.GetBlocksOfType<IMyUserControllableGun>(Elevation_Grid_Guns, A => A.CubeGrid == CTC.ElevationRotor.TopGrid);

                        int found = 0;
                        foreach (IMyShipToolBase Tool in Elevation_Grid_Tools)
                        {
                            CTC.AddTool(Tool);
                            found++;
                        }
                        foreach (IMyUserControllableGun Gun in Elevation_Grid_Guns)
                        {
                            CTC.AddTool(Gun);
                            found++;
                        }
                        if (found == 0)
                        {
                            Echo("No Tools or Guns found");
                            continue;
                        }
                        else if (found > 1)
                        {
                            continue;
                        }

                    }

                    //If Code 3 is return, no Camera is set. The Script will then try to find the Camera on top of the Elevator Rotor and set it as Camera of the CustomTurretController.
                    if (CTC_Status == 3 && CTC.ElevationRotor != null)
                    {
                        List<IMyCameraBlock> CTC_Cameras = new List<IMyCameraBlock>();
                        GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(CTC_Cameras, A => A.CubeGrid == CTC.ElevationRotor.TopGrid);

                        if (CTC_Cameras.Count() == 0)
                        {
                            Echo("No Camera found");
                            continue;
                        }
                        else if (CTC_Cameras.Count() > 1)
                        {
                            Echo("To many Cameras found");
                            continue;
                        }
                        else if (CTC_Cameras.Count() == 1)
                        {
                            CTC.Camera = CTC_Cameras[0];
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    Echo("Error: " + e.Message + " for Controller: " + CTC.CustomName);
                }
            }

            // Write status to LCD panel
            WriteToLCDPanel(CTCList);
        }
        #endregion

        #region  Writing Status to LCD Panel
        // Function to write turret status to LCD panel
        public void WriteToLCDPanel(List<IMyTurretControlBlock> CTCList)
        {
            List<IMyTurretControlBlock> matchedCTCs = new List<IMyTurretControlBlock>();

            StringBuilder statusText = new StringBuilder();
            statusText.AppendLine("=== TURRET STATUS ===");
            statusText.AppendLine("Grid: " + Me.CubeGrid.CustomName);
            statusText.AppendLine("Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            statusText.AppendLine();

            foreach (var kvp in ExpectedControllersAndRotors)
            {
                string controllerName = kvp.Key;
                string expectedAzimuthRotorName = kvp.Value.AzimuthRotorName;
                string expectedElevationRotorName = kvp.Value.ElevationRotorName;
                string expectedStatus = kvp.Value.Status;

                IMyTurretControlBlock controller = CTCList.Find(ctc => ctc.CustomName == controllerName);

                if(controller == null)
                {
                    statusText.AppendLine("TURRET: " + controllerName);
                    statusText.AppendLine("Status: " + expectedStatus);
                    statusText.AppendLine();    
                }
                else if (controller != null)
                {
                    matchedCTCs.Add(controller);
                    int CTC_Status = Check_CTC_Config(controller);
                    statusText.AppendLine("TURRET: " + controller.CustomName);
                    if(controller.IsFunctional)
                    {statusText.AppendLine("Status: " + Dic_CTC_Status[CTC_Status]);}
                    else
                    {statusText.AppendLine("Status: " + expectedStatus);}
                    statusText.AppendLine();
                }
            }    

            foreach (IMyTurretControlBlock controller in CTCList)
            {
                if (!matchedCTCs.Contains(controller))
                {
                    int CTC_Status = Check_CTC_Config(controller);
                    statusText.AppendLine("UNEXPECTED TURRET: " + controller.CustomName);
                    if(controller.IsFunctional)
                    {statusText.AppendLine("Status: " + Dic_CTC_Status[CTC_Status]);}
                    else
                    {statusText.AppendLine("Status: not funktional");}
                    statusText.AppendLine();
                }
            }

            // Find LCD panel with the specified tag
            List<IMyTextPanel> panels = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels, panel => panel.CustomName.Contains(LCD_Panel_Tag));

            if (panels.Count > 0)
            {
                foreach (IMyTextPanel panel in panels)
                {
                    panel.ContentType = ContentType.TEXT_AND_IMAGE;
                    panel.WriteText(statusText.ToString());
                }
            }
        }
        #endregion

        #region Checking for missing controllers and rotors
        // Function to check for missing controllers and rotors
        public void CheckMissingControllersAndRotors(List<IMyTurretControlBlock> currentControllers)
        {
            if (ExpectedControllersAndRotors.Count == 0)
            {
                // If no expected controllers and rotors are stored, store the current ones
                Save();
                return;
            }

            foreach (var kvp in ExpectedControllersAndRotors)
            {
                string controllerName = kvp.Key;
                string expectedAzimuthRotorName = kvp.Value.AzimuthRotorName;
                string expectedElevationRotorName = kvp.Value.ElevationRotorName;

                IMyTurretControlBlock currentController = currentControllers.FirstOrDefault(ctc => ctc.CustomName == controllerName);
                
                if (currentController == null)
                {
                    kvp.Value.Status = "Controller missing: " + controllerName;
                    Echo("Controller missing: " + controllerName);
                    continue;
                }

                if (!currentController.IsFunctional)
                {
                    kvp.Value.Status = "Controller not functional: " + controllerName;
                    Echo("Controller not functional: " + controllerName);
                    continue;
                }

                // Check and reset Azimuth Rotor if missing
                if (currentController.AzimuthRotor == null && expectedAzimuthRotorName != "NULL")
                {
                    List<IMyMotorAdvancedStator> azimuthRotors = new List<IMyMotorAdvancedStator>();
                    GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(azimuthRotors, rotor => rotor.CustomName == expectedAzimuthRotorName);
                    if (azimuthRotors.Count > 0)
                    {
                        currentController.AzimuthRotor = azimuthRotors[0];
                        Echo("Reset Azimuth Rotor for: " + controllerName);
                    }
                    else
                    {
                        kvp.Value.Status = "Azimuth Rotor missing for: " + controllerName;
                        Echo("Azimuth Rotor missing for: " + controllerName);
                    }
                }

                // Check and reset Elevation Rotor if missing
                if (currentController.ElevationRotor == null && expectedElevationRotorName != "NULL")
                {
                    List<IMyMotorAdvancedStator> elevationRotors = new List<IMyMotorAdvancedStator>();
                    GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(elevationRotors, rotor => rotor.CustomName == expectedElevationRotorName);
                    if (elevationRotors.Count > 0)
                    {
                        currentController.ElevationRotor = elevationRotors[0];
                        Echo("Reset Elevation Rotor for: " + controllerName);
                    }
                    else
                    {
                        Echo("Elevation Rotor missing for: " + controllerName);
                    }
                }
            }
        }
        #endregion