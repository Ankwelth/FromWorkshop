
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts

        bool enabled = false;

        string thrusterGroup = "Speed Matcher";

        string targetDirection = "Forward";
        float targetSpeed = 100f;
        float featherThreshold = 50f;


        public Program()
        {
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            List<IMyTerminalBlock> thrusters = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlockGroupWithName(thrusterGroup).GetBlocks(thrusters);

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

            if (!enabled)
            {
                foreach (IMyThrust thruster in thrusters)
                {
                    thruster.ThrustOverridePercentage = 0;
                }
                return;
            }

            List<IMyShipController> shipControllers = new List<IMyShipController>();
            GridTerminalSystem.GetBlocksOfType<IMyShipController>(shipControllers);
            IMyShipController shipController = shipControllers[0];

            float speed = (float)shipController.GetShipSpeed();
            float directionSign;

            directionSign = (targetDirection == "Forward") ? 1 : (targetDirection == "Backward") ? -1 : 0;

            float Thrust = directionSign * (targetSpeed - speed) / featherThreshold;
            Thrust = Clamp(Thrust, 0f, 1f);

            float mass = shipController.CalculateShipMass().TotalMass;
            float gravity = (float)shipController.GetTotalGravity().Length();

            foreach (IMyThrust thruster in thrusters)
            {
                thruster.ThrustOverridePercentage = Thrust;
                thruster.ThrustOverride += mass * gravity / thrusters.Count;
            }
        }

        float Clamp(float x, float min, float max)
        {
            return Math.Max(min, Math.Min(max, x));
        }