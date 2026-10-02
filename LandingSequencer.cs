
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts

        string retrogradeBlockName = "Retro Thruster";
        string speedMatchBlockName = "Speed Match";
        string thrusterGroupName = "Landing Thrusters";
        float desiredHeight = 50f;
        float initialSpeed = 100f;
        float finalSpeed = 5f;

        bool enabled = false;

        public Program()
        {
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {

            switch (argument)
            {
                default:
                    break;

                case "toggle":
                    enabled = !enabled;
                    break;

                case "on":
                    enabled = true;
                    break;

                case "off":
                    enabled = false;
                    break;
            }

            Runtime.UpdateFrequency = enabled ? UpdateFrequency.Update1 : UpdateFrequency.None;


            List<IMyTerminalBlock> shipControllers = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(shipControllers);
            IMyShipController shipController = shipControllers[0] as IMyShipController;

            IMyBlockGroup thrusterGroup = GridTerminalSystem.GetBlockGroupWithName(thrusterGroupName);
            List<IMyTerminalBlock> thrusters = new List<IMyTerminalBlock>();
            thrusterGroup.GetBlocks(thrusters);

            IMyProgrammableBlock retrogradeBlock = GridTerminalSystem.GetBlockWithName(retrogradeBlockName) as IMyProgrammableBlock;
            IMyProgrammableBlock speedMatchBlock = GridTerminalSystem.GetBlockWithName(speedMatchBlockName) as IMyProgrammableBlock;


            double elevation;
            if (!shipController.TryGetPlanetElevation(MyPlanetElevation.Surface, out elevation))
            {
                return;
            }
            

            float maxThrust = 0;
            foreach (IMyThrust thruster in thrusters)
            {
                maxThrust += thruster.MaxEffectiveThrust;
            }

            float mass = shipController.CalculateShipMass().TotalMass;

            float gravity = (float)shipController.GetNaturalGravity().Length();

            float maxAcceleration = gravity - maxThrust / mass;

            float decelerationHeight = desiredHeight - (float)(
                (Math.Pow(finalSpeed, 2) - (Math.Pow(initialSpeed, 2))) / (2 * maxAcceleration)
                );

            
            retrogradeBlock.TryRun("on");

            if (elevation <= desiredHeight)
            {
                speedMatchBlock.TryRun("on");
            }

            Echo (elevation.ToString());
        }