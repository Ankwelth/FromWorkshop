        string cockpitName = "IndustrialCockpitGW";
        int lcdPanelNumber = 2;
        List<IMyCargoContainer> Cargo;
        List<IMyFunctionalBlock> Tools;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            Cargo = new List<IMyCargoContainer>();
            Echo("Initializing Cargo");
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(Cargo, block => block.IsSameConstructAs(Me));
            DetectShipTools();
        }

        public void DetectShipTools()
        {
            List<IMyShipDrill> drills = new List<IMyShipDrill>();
            List<IMyShipWelder> welders = new List<IMyShipWelder>();
            List<IMyShipGrinder> grinders = new List<IMyShipGrinder>();
            Tools = new List<IMyFunctionalBlock>();
            Echo("Initializing Tools");
            GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drills, block => block.IsSameConstructAs(Me));
            if (drills != null && drills.Count() > 0)
            {
               Echo($"Found {drills.Count()} drills");
               for (int i = 0; i < drills.Count(); i++)
               {
                   Tools.Add((IMyFunctionalBlock)drills[i]);
               }
            }
            GridTerminalSystem.GetBlocksOfType<IMyShipWelder>(welders, block => block.IsSameConstructAs(Me));
            if (welders != null && welders.Count() > 0)
            {
               Echo($"Found {welders.Count()} welders");
               for (int i = 0; i < welders.Count(); i++)
               {
                   Tools.Add((IMyFunctionalBlock)welders[i]);
               }
            }
            GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(grinders, block => block.IsSameConstructAs(Me));
            if (grinders != null && grinders.Count() > 0)
            {
               Echo($"Found {grinders.Count()} grinders");
               for (int i = 0; i < grinders.Count(); i++)
               {
                   Tools.Add((IMyFunctionalBlock)grinders[i]);
               }
            }
        }

        public void Main(string argument)
        {
            int barLength = 30;
            string barFill = "|";
            string barEmpty = ".";
            string bar = "[";
            float Capacity = 0.0f;
            float Current = 0.0f;

            Echo($"Cargo container count: {Cargo.Count}");
            if (Cargo == null || Cargo.Count == 0 || argument == "update")
            {
                Echo("Initializing Tools");
                GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(Cargo, block => block.IsSameConstructAs(Me));
                Echo($"Cargo container count: {Cargo.Count}");
            }

            Echo($"Tools count: {Tools.Count}");
            if (Tools == null || Tools.Count == 0 || argument == "update")
            {
                DetectShipTools();
            }

            for (int i = 0; i < Cargo.Count; i++)
            {
                var inventory = Cargo[i].GetInventory(0);
                Echo($"CurrentVolume[{i}] = {inventory.CurrentVolume.ToString()}");
                Capacity += (float)inventory.MaxVolume;
                Current += (float)inventory.CurrentVolume;
            }

            for (int i = 0; i < Tools.Count; i++)
            {
               var inventory = Tools[i].GetInventory(0);
               Capacity += (float)inventory.MaxVolume;
                Current += (float)inventory.CurrentVolume;
            }

            Echo($"Capacity: {Capacity}");
            Echo($"Current: {Current}");
            var amount = Current / Capacity;
            var filledPercentage = amount * 100;
            var filledBars = (int)(barLength * amount);

            for (int i = filledBars; i > 0; i--)
            {
                bar += barFill;
            }

            for (int i = barLength - filledBars; i > 0; i--)
            {
                bar += barEmpty;
            }
            string TextOutput = bar + "]" + "\n" + String.Format("Cargo Load: {0:0.00} %", filledPercentage);

            Echo(TextOutput);
            
            var cockpit = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName( cockpitName, cockpit, block => block.IsSameConstructAs(Me));
            for(int i = 0; i < cockpit.Count(); i++)
            {
               var outputScreens =  new List<IMyTextSurfaceProvider>();
              Echo($"Lcd Screens in cockpit: {  ((IMyTextSurfaceProvider)cockpit[i]).SurfaceCount}");
              IMyTextSurface textSurface =  ((IMyTextSurfaceProvider)cockpit[i]).GetSurface(lcdPanelNumber);
              if (textSurface != null)
                   textSurface.WriteText(TextOutput);
            }
        }