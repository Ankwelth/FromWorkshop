        // to set a specific landing velocity, use command "land_{speed}"
        private const string COMMAND_LAND = "land";
        private const float LANDING_SPEED_DEFAULT = 3f;

        private const string COMMAND_SYSTEM_ON = "sys_on";
        private const string COMMAND_SYSTEM_OFF = "sys_off";

        private const string COMMAND_LEVELIZING_ON = "levelize_on";
        private const string COMMAND_LEVELIZING_OFF = "levelize_off";

        private const string NAME_LCD_FORLOG = "LCD_ForDebug";

        private const string NAMETAG_VECTOR_THRUSTS = "vector";
        private const string NAMETAG_ROTOR_LEFT = "vector_thrust_l";
        private const string NAMETAG_ROTOR_RIGHT = "vector_thrust_r";
        private const string NAMETAG_GYRO = "Gyro";

        private const float ANGLE_THRESHOLD = 0.0087f; // 0.5 degree
        private const float MAX_SPEED = 100f;
        private const float RAD_SPEED_MAX = 3.14f;
        private const float DAMPENER_FACTOR = 1f; // how fast we stop

        private const float GYRO_ANGLE_THRESHOLD = 0.0087f; // 0.5 degree
        /// <summary>
        /// GYRO:How much power to use 0 to 1.0
        /// </summary>
        private const float CTRL_COEFF = 0.5f;

        // only when current gravity is less than this threshold, the thrust will push down when pressing 'C'
        private const float GRAVITY_THRESHOLD_THRUST_DOWN = 9.81f * 0.3f; // 0.3G
        // if current gravity is larger than this threshold, we will turn gyros to levelize ship
        private const float GRAVITY_THRESHOLD_LEVELIZING = 9.81f * 0.3f; // 0.3G

        private IMyTextSurface m_lcd_log = null;

        private List<IMyShipController> m_myShipControllers = new List<IMyShipController>();
        private List<IMyThrust> m_thrusts = new List<IMyThrust>();
        private List<IMyMotorStator> m_rotor_left = new List<IMyMotorStator>();
        private List<IMyMotorStator> m_rotor_right = new List<IMyMotorStator>();
        private List<IMyGyro> m_gyros = new List<IMyGyro>();

        private bool m_hasBeenSetup = false;

        private float m_landingSpeed = LANDING_SPEED_DEFAULT;

        private enum FlyMode
        {
            Floating,
            Landing
        }

        private enum LevelizingMode
        {
            On,
            Off
        }

        private enum SystemStatus
        {
            On,
            Off
        }

        private FlyMode m_flyMode = FlyMode.Floating;

        private LevelizingMode m_levelizingMode = LevelizingMode.On;

        private SystemStatus m_systemStatus = SystemStatus.On;

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.

            Echo($"[{DateTime.Now}]");
            PrintOnLCD($"[{DateTime.Now}]\r\n", false);

            if (!SetupIfNecessary())
            {
                return;
            }

            if (argument.StartsWith(COMMAND_LAND))
            {
                InitializeLanding(argument);
                m_flyMode = FlyMode.Landing;
            }
            else if (argument.Equals(COMMAND_LEVELIZING_ON))
            {
                m_levelizingMode = LevelizingMode.On;
            }
            else if (argument.Equals(COMMAND_LEVELIZING_OFF))
            {
                m_levelizingMode = LevelizingMode.Off;
            }
            else if (argument.Equals(COMMAND_SYSTEM_ON))
            {
                m_systemStatus = SystemStatus.On;
                // only enable thrusters when system is on. so that we can turn off them when landed
                EnableThrusters(m_thrusts, true);
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
            else if (argument.Equals(COMMAND_SYSTEM_OFF))
            {
                m_systemStatus = SystemStatus.Off;
                Runtime.UpdateFrequency = UpdateFrequency.Update100;
            }

            if (m_systemStatus == SystemStatus.Off)
            {
                EnableThrusters(m_thrusts, false);
                TurningGyros(m_gyros, false, 0, 0, 0);
                return;
            }

            IMyShipController myshipController = m_myShipControllers[0];
            EngageThrusters(myshipController);

            if (m_levelizingMode == LevelizingMode.On)
            {
                EngageGyro(myshipController);
            }
            else
            {
                TurningGyros(m_gyros, false, 0, 0, 0);
            }
        }

        private bool EngageGyro(IMyShipController myShipController)
        {
            float rollIndicator = myShipController.RollIndicator;
            if (rollIndicator != 0)
            {
                TurningGyros(m_gyros, false, 0, 0, 0);
                return false;
            }

            Vector3D natualGravityVectorWorld = myShipController.GetNaturalGravity();
            if (natualGravityVectorWorld.Length() <= GRAVITY_THRESHOLD_LEVELIZING)
            {
                // low gravity, no levelizing
                return false;
            }
            Vector3D naturalGravityVector = Vector3D.TransformNormal(natualGravityVectorWorld, MatrixD.Transpose(myShipController.WorldMatrix));
            // we don't care about pitch or yaw.
            naturalGravityVector.Y = 0f;
            naturalGravityVector.Z = 0f;

            Matrix upOrientation;
            myShipController.Orientation.GetMatrix(out upOrientation);

            Vector3D localUp = Vector3D.TransformNormal(upOrientation.Up, MatrixD.Transpose(upOrientation));
            Vector3D negGravity = Vector3D.Negate(naturalGravityVector);

            PrintOnLCD($"localUp = {localUp}\r\n", true);
            PrintOnLCD($"negGravity = {negGravity}\r\n", true);

            Vector3D rot = Vector3D.Cross(localUp, negGravity);
            double ang = Math.Atan2(
                rot.Length(),
                Math.Sqrt(Math.Max(0.0, 1.0 - rot.LengthSquared())));

            if (Vector3D.Dot(localUp, negGravity) < 0)
            {
                ang = Math.PI - ang;
            }

            if (ang <= GYRO_ANGLE_THRESHOLD)
            {
                TurningGyros(m_gyros, false, 0, 0, 0);
                return true;
            }

            float maxVel = (float)(Math.PI * 2);
            // if ang is between -1 and 1, we square it so that it will be less sensitive
            if (0 < ang && ang < 1)
            {
                ang *= ang;
            }
            else if (-1 < ang && ang < 0)
            {
                ang *= -ang;
            }
            float ctrl_vel = (float)(Math.PI * ang * CTRL_COEFF);
            ctrl_vel = Math.Min(maxVel, ctrl_vel);
            ctrl_vel = Math.Max(0f, ctrl_vel);

            rot.Normalize();
            rot *= ctrl_vel;

            float roll = -(float)rot.Z;

            Vector2 rotationIndicator = myShipController.RotationIndicator;
            TurningGyros(m_gyros, true, rotationIndicator.Y, rotationIndicator.X, roll);
            return false;
        }

        private void EngageThrusters(IMyShipController myShipController)
        {
            Vector3D shipVelocityWorld = myShipController.GetShipVelocities().LinearVelocity;
            Vector3D shipVelocity = Vector3D.TransformNormal(shipVelocityWorld, MatrixD.Transpose(myShipController.WorldMatrix));

            shipVelocity = Vector3D.Multiply(shipVelocity, DAMPENER_FACTOR);

            Vector3D natualGravityVectorWorld = myShipController.GetNaturalGravity();
            Vector3D naturalGravityVector = Vector3D.TransformNormal(natualGravityVectorWorld, MatrixD.Transpose(myShipController.WorldMatrix));

            PrintOnLCD($"Natural gravity length = {naturalGravityVector.Length()}\r\n", true);

            Vector3D moveIndicator = myShipController.MoveIndicator;

            if (!myShipController.DampenersOverride)
            {
                // if dampener is off, no override
                OverrideThrusters(myShipController, Vector3D.Zero);
                return;
            }

            if (naturalGravityVector.Length() == 0)
            {
                // no gravity, reset from landing mode
                m_flyMode = FlyMode.Floating;
            }

            Vector3D totalVector;
            if (moveIndicator.Z == 0 && moveIndicator.Y == 0) // if no pilot input, we float still, or land
            {
                if (m_flyMode == FlyMode.Landing)
                {
                    // if during landing, fake gravity vector by reducing its length by LANDING_SPEED_DEFAULT
                    naturalGravityVector = Vector3D.Multiply(
                        naturalGravityVector,
                        1 - m_landingSpeed / naturalGravityVector.Length());
                }
                totalVector = Vector3D.Add(shipVelocity, naturalGravityVector);
            }
            else
            {
                // having input, set cancel landing mode
                m_flyMode = FlyMode.Floating;

                // fake ship velocity to make thrusters go forward
                if (moveIndicator.Z < 0)
                {
                    shipVelocity.Z = MAX_SPEED;
                }
                else if (moveIndicator.Z > 0)
                {
                    shipVelocity.Z = -MAX_SPEED;
                }

                if (moveIndicator.Y > 0)
                {
                    shipVelocity.Y = -MAX_SPEED;
                }
                else if (moveIndicator.Y < 0) // pilot pressing 'C'
                {
                    if (naturalGravityVector.Length() > GRAVITY_THRESHOLD_THRUST_DOWN) // if current gravity > a threshold
                    {
                        // fake the vertical velocity to cancel the gravity, so we can let the gravity do the work
                        shipVelocity.Y = -naturalGravityVector.Y;
                    }
                    else
                    {
                        shipVelocity.Y = MAX_SPEED;
                    }
                }

                PrintOnLCD($"moveIndicator = {moveIndicator}\r\n", true);

                totalVector = Vector3D.Add(shipVelocity, naturalGravityVector);
            }

            AlignThrusters(Vector3D.Negate(totalVector), m_rotor_left, m_rotor_right);
            OverrideThrusters(myShipController, totalVector);
        }

        private void OverrideThrusters(IMyShipController myShipController, Vector3D vector)
        {
            float mass = myShipController.CalculateShipMass().PhysicalMass;
            float force = (float)(mass * vector.Length() / m_thrusts.Count);

            if (force > m_thrusts[0].MaxEffectiveThrust)
            {
                InsufficientThrustWarning(force, m_thrusts[0].MaxEffectiveThrust);
            }

            SetOverride(m_thrusts, force);
        }

        private bool AlignThrusters(Vector3D vector, List<IMyMotorStator> m_rotor_left, List<IMyMotorStator> m_rotor_right)
        {
            double atan2_r = Math.Atan2(vector.Y, vector.Z) + Math.PI;
            double atan2_l = Math.Atan2(-vector.Y, vector.Z) + Math.PI;

            bool aligned = true;

            foreach (var r in m_rotor_left)
            {
                aligned &= MoveToAngle(r, (float)atan2_l, RAD_SPEED_MAX);
            }
            foreach (var r in m_rotor_right)
            {
                aligned &= MoveToAngle(r, (float)atan2_r, RAD_SPEED_MAX);
            }

            return aligned;
        }

        private void InsufficientThrustWarning(float thrustNeeded, float possibleThrust)
        {
            PrintOnLCD($"[WARNING]\r\nthrustNeeded = {thrustNeeded}\r\n", true);
            PrintOnLCD($"possibleThrust = {possibleThrust}\r\n", true);
        }

        private void SetOverride(List<IMyThrust> thrusts, float force)
        {
            foreach (var t in thrusts)
            {
                t.ThrustOverridePercentage = Math.Min(1f, force / t.MaxEffectiveThrust);
            }
        }

        private bool SetupIfNecessary()
        {
            if (m_hasBeenSetup)
            {
                return true;
            }

            var GS = GridTerminalSystem;
            GS.GetBlocksOfType(m_myShipControllers);
            GS.GetBlocksOfType(m_thrusts, t => t.DisplayNameText.Contains(NAMETAG_VECTOR_THRUSTS));
            GS.GetBlocksOfType(m_rotor_left, r => r.DisplayNameText.Contains(NAMETAG_ROTOR_LEFT));
            GS.GetBlocksOfType(m_rotor_right, r => r.DisplayNameText.Contains(NAMETAG_ROTOR_RIGHT));
            GS.GetBlocksOfType(m_gyros, g => g.DisplayNameText.Contains(NAMETAG_GYRO));

            m_lcd_log = (IMyTextSurface)GS.GetBlockWithName(NAME_LCD_FORLOG);

            if (m_myShipControllers.Count == 0)
            {
                Echo("No ship controller!");
                return false;
            }

            if (m_thrusts.Count == 0)
            {
                Echo("No vector thrusts!");
                return false;
            }

            m_hasBeenSetup = true;
            return true;
        }

        private void InitializeLanding(string argument)
        {
            int i = argument.LastIndexOf('_');
            if (i > 0 && i < argument.Length - 1)
            {
                try
                {
                    m_landingSpeed = float.Parse(argument.Substring(i + 1));
                }
                catch
                {
                    // ignore
                }
            }
        }

        #region Utility

        private void EnableThrusters(List<IMyThrust> thrusters, bool enable)
        {
            foreach (var t in thrusters)
            {
                t.Enabled = enable;
                if (!enable)
                {
                    t.ThrustOverridePercentage = 0f;
                }
            }
        }

        private void TurningGyros(List<IMyGyro> gyros, bool turning, float yaw, float pitch, float roll)
        {
            foreach (var g in gyros)
            {
                g.GyroOverride = turning;
                g.Yaw = yaw;
                g.Pitch = pitch;
                g.Roll = roll;
            }
        }

        private void PrintOnLCD(string text, bool append)
        {
            if (m_lcd_log != null)
            {
                m_lcd_log.WriteText(text, append);
            }
        }

        private bool MoveToAngle(IMyMotorStator motor, float targetAngle, float radSpeed)
        {
            if (StopMotorIfReachesAngle(motor, targetAngle))
            {
                motor.RotorLock = true;
                ClearLimits(motor);
                return true;
            }

            motor.RotorLock = false;
            float delta = targetAngle - motor.Angle;
            if (delta > 0)
            {
                if (delta < Math.PI)
                {
                    motor.TargetVelocityRad = radSpeed;
                    motor.LowerLimitRad = float.MinValue;
                    motor.UpperLimitRad = targetAngle;
                }
                else
                {
                    motor.TargetVelocityRad = -radSpeed;
                    motor.UpperLimitRad = float.MaxValue;
                    motor.LowerLimitRad = targetAngle;
                }
            }
            else
            {
                if (Math.Abs(delta) < Math.PI)
                {
                    motor.TargetVelocityRad = -radSpeed;
                    motor.UpperLimitRad = float.MaxValue;
                    motor.LowerLimitRad = targetAngle;
                }
                else
                {
                    motor.TargetVelocityRad = radSpeed;
                    motor.LowerLimitRad = float.MinValue;
                    motor.UpperLimitRad = targetAngle;
                }
            }

            return false;
        }

        private bool StopMotorIfReachesAngle(IMyMotorStator motor, float targetAngle)
        {
            if (HasMotorReachedAngle(motor, targetAngle))
            {
                motor.TargetVelocityRad = 0;
                return true;
            }

            return false;
        }

        private void ClearLimits(IMyMotorStator motor)
        {
            motor.LowerLimitRad = float.MinValue;
            motor.UpperLimitRad = float.MaxValue;
        }

        private bool HasMotorReachedAngle(IMyMotorStator motor, float targetAngle)
        {
            return Math.Abs(motor.Angle - targetAngle) <= ANGLE_THRESHOLD;
        }

        #endregion