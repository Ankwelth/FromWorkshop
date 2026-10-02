//  Bongo's Rover Light Control and Utilities Script
     //
     //  After deploying the script modify settings in the custom data of this block to your liking.
     //  For further instructions check workshop page
     //
     //  Leave feedback on the workshop
     //
     //  DO NOT MODIFY ANYTHING BEYOND THIS POINT. Actually feel free, idc, but dont complain if it breaks

        string version = "v1.0";

        string cockpitGroupName = "Cockpit";
        string brakelightGroupName = "Brakelights";
        string leftIndicatorGroupName = "Left Indicators";
        string rightIndicatorGroupName = "Right Indicators";
        //string wheels = "[Wheels]";  // Used for tank steering, TCS and SCS

        bool autohandbrake = false;
        bool cTogglesHandbrake = false;


        List<IMyLightingBlock> brakelights;
        List<IMyLightingBlock> indicatorsR;
        List<IMyLightingBlock> indicatorsL;

        List<IMyShipController> controllers;

        IMyShipController controller;

        Vector3 PlayerInput = Vector3.Zero;
        Vector3 LastFrameInput = Vector3.Zero;

        int frameCount = 0;

        enum Setup
        {
            Pre_Setup,
            Setup,
            Post_Setup
        }

        public void set_light_list_power(List<IMyLightingBlock> terminalBlocks, bool state) {
            foreach(IMyLightingBlock terminalBlock in terminalBlocks) {
                terminalBlock.Enabled = state;
            }
        }

        public void update_blocks_used()
        {
            IMyBlockGroup tempGroup;

            controllers = new List<IMyShipController>();
            try
            {
                tempGroup = GridTerminalSystem.GetBlockGroupWithName(cockpitGroupName);
                tempGroup.GetBlocksOfType(controllers);
            }
            catch { }

            if (controllers.Count != 0) {
                bool mainCockpitExists;
                bool cockpitOccupied;
                int cockpitIndex = 0;

                mainCockpitExists = false;
                cockpitOccupied = false;

                for (int i = 0; i < controllers.Count; i++) {
                    controller = controllers[i];
                    if (controller.IsMainCockpit) {
                        mainCockpitExists = true;
                        cockpitIndex = i;
                        cockpitOccupied = controller.IsUnderControl;
                    }
                    if (controller.IsUnderControl && !mainCockpitExists) {
                        cockpitIndex = i;
                        cockpitOccupied = true;
                    }
                }
                controller = controllers[cockpitIndex];  // Sets controller to an occupied cockpit, this is overidden if a main cockpit exists
            }
            else {
                controller = null;
            }

            brakelights = new List<IMyLightingBlock>();
            try
            {
                tempGroup = GridTerminalSystem.GetBlockGroupWithName(brakelightGroupName);
                tempGroup.GetBlocksOfType(brakelights);
            }
            catch { }

            indicatorsR = new List<IMyLightingBlock>();
            try
            {
                tempGroup = GridTerminalSystem.GetBlockGroupWithName(rightIndicatorGroupName);
                tempGroup.GetBlocksOfType(indicatorsR);
            }
            catch { }

            indicatorsL = new List<IMyLightingBlock>();
            try
            {
                tempGroup = GridTerminalSystem.GetBlockGroupWithName(leftIndicatorGroupName);
                tempGroup.GetBlocksOfType(indicatorsL);
            }
            catch { }
        }
        public Program()
        {
            // Read settings from custom data
            string data = Me.CustomData;
            string[] lines = data.Split('\n');
            Setup setup = Setup.Pre_Setup;
            string pre_setup = "";
            string post_setup = "";
            foreach (string line in lines)
            {
                if (line.StartsWith("[Bongo's Rover Light Control and Utilities Script"))
                {
                    setup = Setup.Setup;
                }
                else if (setup == Setup.Setup && line.StartsWith("["))
                {
                    setup = Setup.Post_Setup;
                }
                if (line.StartsWith("Cockpit Group:") && setup == Setup.Setup)
                {
                    cockpitGroupName = line.Substring(line.IndexOf(":") + 1);
                }
                if (line.StartsWith("Breaklight Group:") && setup == Setup.Setup)
                {
                    brakelightGroupName = line.Substring(line.IndexOf(":") + 1);
                }
                if (line.StartsWith("Right Indicator Group:") && setup == Setup.Setup)
                {
                    rightIndicatorGroupName = line.Substring(line.IndexOf(":") + 1);
                }
                if (line.StartsWith("Left Indicator Group:") && setup == Setup.Setup)
                {
                    leftIndicatorGroupName = line.Substring(line.IndexOf(":") + 1);
                }
                if (line.StartsWith("Autohandbrake:") && setup == Setup.Setup)
                {
                    autohandbrake = line.Substring(line.IndexOf(":") + 1).ToLower().Contains("true");
                }
                if (line.StartsWith("C Toggles Handbrake:") && setup == Setup.Setup)
                {
                    cTogglesHandbrake = line.Substring(line.IndexOf(":") + 1).ToLower().Contains("true");
                }
                if (setup == Setup.Pre_Setup) { pre_setup += line+"\n"; }
                if (setup == Setup.Post_Setup) { post_setup += line+"\n"; }
            }
            if (pre_setup == "") { pre_setup = "Make sure to recompile the script to apply setup changes!\n"; }
            // Write in setup data, will use provided or default values
            Me.CustomData = pre_setup
                + $"[Bongo's Rover Light Control and Utilities Script {version}]\n"
                + $"Cockpit Group:{cockpitGroupName}\n"
                + $"Breaklight Group:{brakelightGroupName}\n"
                + $"Right Indicator Group:{rightIndicatorGroupName}\n"
                + $"Left Indicator Group:{leftIndicatorGroupName}\n"
                + $"Autohandbrake:{autohandbrake}\n"
                + $"C Toggles Handbrake:{cTogglesHandbrake}\n"
                + post_setup;

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // Check for new blocks every 100 game ticks including first tick
            frameCount %= 100;
            if (frameCount == 0)
            {
                update_blocks_used();
            }
            frameCount += 1;
            if (controller != null) {
                if (controller.IsUnderControl)
                {
                    PlayerInput = controller.MoveIndicator;

                    float xInput = PlayerInput.GetDim(0);  // AD
                    float yInput = PlayerInput.GetDim(1);  // Space C
                    float zInput = PlayerInput.GetDim(2);  // WS

                    float yInputMem = LastFrameInput.GetDim(1);

                    if (yInput < 0 && yInputMem >= 0 && cTogglesHandbrake) { controller.HandBrake = !controller.HandBrake; }
                    set_light_list_power(indicatorsL, xInput == -1);
                    set_light_list_power(indicatorsR, xInput == 1);
                    set_light_list_power(brakelights, zInput == 1 || yInput == 1);

                    LastFrameInput = PlayerInput;
                }
                else {
                    
                    set_light_list_power(brakelights, false);
                    set_light_list_power(indicatorsL, false);
                    set_light_list_power(indicatorsR, false);
                }

                controller.HandBrake = controller.HandBrake || autohandbrake && !controller.IsUnderControl;  // Autohandbrake

            }

            Echo(
                $"[Bongo's Rover Light Control and Utilities Script {version}]\n"
                + $"Cockpits:\n  {cockpitGroupName} -> {controllers.Count}\n"
                + $"Brakelights:\n  {brakelightGroupName} -> {brakelights.Count}\n"
                + $"Left Indicators:\n  {leftIndicatorGroupName} -> {indicatorsL.Count}\n"
                + $"Right Indicators:\n  {rightIndicatorGroupName} -> {indicatorsR.Count}\n"
            );
        }