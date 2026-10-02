        /// <summary>
        /// Sherberts Vectored thrust script- very basic
        /// 
        /// Detects thrusters off grid and determines the direction they are facing (front,back,left,right,up,down) just basic 6dir
        /// 
        /// Setup:
        /// 1. put the main cockpit name to be equal to controllerName
        /// 2. place a remote control on each subgrid(to toggle inertial dampeners on subgrids)
        /// 3. Recompile and run
        ///
        /// </summary>

        string controllerName = "Cockpit";

        IMyShipController shipControler;

        List<IMyTerminalBlock> remotes = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> thrusters = new List<IMyTerminalBlock>();

        bool slavedInertials = true;

        public Program()
        {         
            Init();           
        }
        void Init()
        {
            shipControler = GridTerminalSystem.GetBlockWithName(controllerName) as IMyShipController;
            GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters, isSubGrid);
            if(slavedInertials)
                GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(remotes, isSubGrid);

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }
                             
        public void Main(string argument, UpdateType updateSource)
        {
            if(updateSource == UpdateType.Update1)
            {

                Echo(shipControler.MoveIndicator.ToString());
                if (slavedInertials && remotes.Count != 0)
                    foreach(IMyTerminalBlock i in remotes)
                    {
                        IMyRemoteControl remote = i as IMyRemoteControl;
                        remote.DampenersOverride = shipControler.DampenersOverride;
                    }
                if(thrusters.Count != 0)
                    foreach(IMyTerminalBlock i in thrusters)
                    {
                        SetThrust(i);
                    }



            }
        }

        bool isSubGrid(IMyTerminalBlock block)
        {
            return !(Me.CubeGrid == block.CubeGrid);
        }

        void SetThrust(IMyTerminalBlock block)
        {
          
            switch (GetDirection(shipControler,block))
            {
                case Base6Directions.Direction.Forward:
                    if (shipControler.MoveIndicator.Z > 0)
                        (block as IMyThrust).ThrustOverridePercentage = shipControler.MoveIndicator.Z;
                    else
                    {
                        if (slavedInertials && shipControler.MoveIndicator.Z != 0)
                            (block as IMyThrust).ThrustOverridePercentage = 0.0001f;
                        else
                            (block as IMyThrust).ThrustOverridePercentage = 0;
                    }
                    break;
                case Base6Directions.Direction.Backward:
                    if (shipControler.MoveIndicator.Z < 0)
                        (block as IMyThrust).ThrustOverridePercentage = -shipControler.MoveIndicator.Z;
                    else
                    {
                        if (slavedInertials && shipControler.MoveIndicator.Z != 0)
                            (block as IMyThrust).ThrustOverridePercentage = 0.0001f;
                        else
                            (block as IMyThrust).ThrustOverridePercentage = 0;
                    }
                    break;
                case Base6Directions.Direction.Left:
                    if (shipControler.MoveIndicator.X > 0)
                        (block as IMyThrust).ThrustOverridePercentage = shipControler.MoveIndicator.X;
                    else
                    {
                        if (slavedInertials && shipControler.MoveIndicator.X != 0)
                            (block as IMyThrust).ThrustOverridePercentage = 0.0001f;
                        else
                            (block as IMyThrust).ThrustOverridePercentage = 0;
                    }
                    break;
                case Base6Directions.Direction.Right:
                    if (shipControler.MoveIndicator.X < 0)
                        (block as IMyThrust).ThrustOverridePercentage = -shipControler.MoveIndicator.X;
                    else
                    {
                        if (slavedInertials && shipControler.MoveIndicator.X != 0)
                            (block as IMyThrust).ThrustOverridePercentage = 0.0001f;
                        else
                            (block as IMyThrust).ThrustOverridePercentage = 0;
                    }
                    break;
                case Base6Directions.Direction.Up:
                    if (shipControler.MoveIndicator.Y < 0)
                        (block as IMyThrust).ThrustOverridePercentage = -shipControler.MoveIndicator.Y;
                    else
                    {
                        if (slavedInertials && shipControler.MoveIndicator.Y != 0)
                            (block as IMyThrust).ThrustOverridePercentage = 0.0001f;
                        else
                            (block as IMyThrust).ThrustOverridePercentage = 0;
                    }
                    break;
                case Base6Directions.Direction.Down:
                    if (shipControler.MoveIndicator.Y > 0)
                        (block as IMyThrust).ThrustOverridePercentage = shipControler.MoveIndicator.Y;
                    else
                    {
                        if (slavedInertials && shipControler.MoveIndicator.Y != 0)
                            (block as IMyThrust).ThrustOverridePercentage = 0.0001f;
                        else
                            (block as IMyThrust).ThrustOverridePercentage = 0;
                    }
                    break;
                default:
                    break;
            }
        }

        Base6Directions.Direction GetDirection(IMyTerminalBlock block)
        {
            return Me.WorldMatrix.GetClosestDirection(block.WorldMatrix.Forward);
        }

        Base6Directions.Direction GetDirection(IMyTerminalBlock reference, IMyTerminalBlock block)
        {
            return reference.WorldMatrix.GetClosestDirection(block.WorldMatrix.Forward);
        }

