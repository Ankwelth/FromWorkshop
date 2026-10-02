
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts

        bool enabled = false;

        string GyroscopeGroup = "Retro Gyros";

        public Program()
        {
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            List<IMyTerminalBlock> gyros = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlockGroupWithName(GyroscopeGroup).GetBlocks(gyros);
            
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

            foreach (IMyGyro gyro in gyros)
            {
                gyro.GyroOverride = enabled;
            }

            Runtime.UpdateFrequency = enabled ? UpdateFrequency.Update1 : UpdateFrequency.None;

            if (!enabled)
            {
                return;
            }


            List<IMyShipController> shipControllers = new List<IMyShipController>();
            GridTerminalSystem.GetBlocksOfType<IMyShipController>(shipControllers);
            IMyShipController shipController = shipControllers[0];

            Vector3 alignmentVector = shipController.GetShipVelocities().LinearVelocity;
            alignmentVector = Vector3.Negate(alignmentVector);
            alignmentVector = Vector3.Normalize(alignmentVector);

            foreach(IMyGyro gyro in gyros)
            {
                gyro.Pitch = Vector3.Cross(Me.WorldMatrix.Forward, alignmentVector).Length() * Vector3.Dot(Me.WorldMatrix.Up, alignmentVector) * 1.5f;
                gyro.Yaw = Vector3.Cross(Me.WorldMatrix.Forward, alignmentVector).Length() * Vector3.Dot(Me.WorldMatrix.Right, alignmentVector) * 1.5f;
            }            
        }