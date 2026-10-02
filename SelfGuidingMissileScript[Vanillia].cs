/*
 * Self Guiding Missile Script
 * For use with a weaponcore turret/gun
 * Enter missile group name in custom data and recompile, script will yell at you whats missing
 * Command to fire is fire 
 * Cruise Distance, conserve fuel, Plungedistance for topdown fuckery. DelayTicks, change to lengthen the missiles activation, use if you want funny torpedoes
 */
const double DEF_PD_P_GAIN = 10;
        const double DEF_PD_D_GAIN = 5;
        const double DEF_PD_AIM_LIMIT = 6.3;
        bool missileActive = false;
        bool homingActive = false;
        int delayTicks = 0;
        int currentClock = 0;
        List<IMyThrust> forwardThrust = new List<IMyThrust>();
        IMyShipMergeBlock mergeBlock;
        List<IMyGyro> allGyros = new List<IMyGyro>();
        List<IMyWarhead> warheads = new List<IMyWarhead>();
        List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
        IMySensorBlock proximitySensor;
        IMyLargeTurretBase targetingTurret;
        IMyRemoteControl missileRemote;
        MyDetectedEntityInfo currentTarget;
        double lastSpeed = 0;
        Vector3D lastTargetVelocity;
        Vector3D AccelerationVector;
        bool hasTarget = false;
        long likelyTarget = 0;
        List<MyDetectedEntityInfo> allProxyTargets = new List<MyDetectedEntityInfo>();
        List<IMyGasTank> hydroTanks = new List<IMyGasTank>();
        bool proxArmed = false;
        bool currentlyArmed = false;
        bool plunging = false;
        bool canCruise = true;
        bool cruising = false;
        double CruiseDistSqMin = 100;
        double PlungeDist = 100;
        bool valid = true;
        string blockGroupName = "Missile1";
        PDController yawController = new PDController(DEF_PD_P_GAIN, DEF_PD_D_GAIN, 6);
        PDController pitchController = new PDController(DEF_PD_P_GAIN, DEF_PD_D_GAIN, 6);
        Vector3D startingDir;
        float forwardThrustCapacity = 0f;
        IMyShipConnector connector;
        List<IMyPowerProducer> otherPower = new List<IMyPowerProducer>();

        public Program()
        {
            MyIni ini = new MyIni();
            if (ini.TryParse(Me.CustomData))
            {
                try
                {
                    delayTicks = ini.Get("Config", "DelayTicks").ToInt32();
                    blockGroupName = ini.Get("Config", "GroupName").ToString();
                    CruiseDistSqMin = ini.Get("Config", "CruiseDistSqMin").ToDouble();
                    PlungeDist = ini.Get("Config", "PlungeDist").ToDouble();
                    Echo($"Config Parsed Successfully");
                }
                catch
                {
                    Echo("ERROR Parsing Custom Data\nRecompile Script");
                    Me.CustomData = "";
                    return;
                }
            }
            else
            {
                Me.CustomData = $"[Config]\n;Use this to set your block names\nDelayTicks={delayTicks}\nGroupName={blockGroupName}\nCruiseDistSqMin={CruiseDistSqMin}\nPlungeDist={PlungeDist}";
                Echo("Config not found, adding template to custom data");
            }
            IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName(blockGroupName);
            if (group == null)
            {
                Echo($"ERROR: No group named \"{blockGroupName}\" found");
                return;
            }
            group.GetBlocksOfType<IMyTerminalBlock>(null, GetBlocks);
            if (missileRemote == null)
            {
                Echo($"No remote found in group {blockGroupName}");
                valid = false;
            }
            if (mergeBlock == null)
            {
                Echo($"No merge block found in group {blockGroupName}");
                valid = false;
            }
            if (allGyros.Count == 0)
            {
                Echo($"No gyros found in group {blockGroupName}");
                valid = false;
            }
            if (targetingTurret == null)
            {
                Echo($"No turret found in group {blockGroupName}");
                valid = false;
            }
            if (batteries.Count == 0)
            {
                Echo($"No batteries found in group {blockGroupName}");
                valid = false;
            }
            if (missileRemote != null)
            {
                forwardThrust.RemoveAll(x =>
                {
                    return x.WorldMatrix.Backward != missileRemote.WorldMatrix.Forward;
                });
            }
            if (forwardThrust.Count == 0)
            {
                Echo($"No forward thrust found in group {blockGroupName}\nMake sure thrust is only forwards");
                valid = false;
            }
            if (warheads.Count == 0)
            {
                Echo($"No warheads found in group {blockGroupName} (not an error)");
            }
            if (proximitySensor == null)
            {
                Echo($"No sensor found in group {blockGroupName} (not an error)");
            }
            if (!valid)
            {
                Echo("Critical Errors need to be fixed");
                return;
            }
            foreach(var thrust in forwardThrust)
            {
                forwardThrustCapacity += thrust.MaxEffectiveThrust;
            }
            if (connector != null)
            {
                connector.Connect();
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!valid) return;
            if (argument != "")
            {
                switch (argument)
                {
                    case "fire":
                        missileActive = true;
                        mergeBlock.Enabled = false;
                        if (connector != null)
                        {
                            connector.Enabled = false;
                        }
                        foreach (IMyGyro gyro in allGyros)
                        {
                            gyro.Enabled = true;
                            gyro.GyroOverride = true;
                            gyro.Pitch = 0f;
                            gyro.Yaw = 0f;
                            gyro.Roll = 0f;
                        }
                        foreach (IMyBatteryBlock battery in batteries)
                        {
                            battery.Enabled = true;
                            battery.ChargeMode = ChargeMode.Discharge;
                        }
                        foreach(var p in otherPower)
                        {
                            p.Enabled = true;
                        }
                        startingDir = missileRemote.WorldMatrix.Forward;
                        Runtime.UpdateFrequency = UpdateFrequency.Update1;
                        break;
                    default:
                        break;
                }
                return;
            }
            if (missileActive)
            {
                
                if (homingActive)
                {
                    bool targetUpdated = false;
                    if (proximitySensor != null && likelyTarget != 0)
                    {
                        allProxyTargets.Clear();
                        proximitySensor.DetectedEntities(allProxyTargets);
                        foreach (MyDetectedEntityInfo entity in allProxyTargets)
                        {
                            if (entity.EntityId == likelyTarget)
                            {
                                currentTarget = entity;
                                targetUpdated = true;
                                break;
                            }
                        }
                    }
                    if (!targetUpdated)
                    {
                        currentTarget = targetingTurret.GetTargetedEntity();
                    }
                    var grav = missileRemote.GetNaturalGravity();
                    if (currentTarget.IsEmpty() && grav != Vector3D.Zero)
                    {
                        var stabilizeHeading = forwardThrustCapacity * startingDir - grav;
                        grav.Normalize();
                        AimAtTarget(grav);
                        return;
                    }
                    if (!currentTarget.IsEmpty())
                    {
                        hasTarget = true;
                        likelyTarget = currentTarget.EntityId;
                        Vector3D normalAccelerationVector = RefreshNavigation();

                        canCruise = canCruise && normalAccelerationVector.Dot(Vector3D.Normalize(missileRemote.GetShipVelocities().LinearVelocity)) > .98;
                        if (cruising != canCruise)
                        {
                            cruising = canCruise;
                            foreach (var thruster in forwardThrust) thruster.ThrustOverridePercentage = canCruise ? .25f : 1f;
                        }
                        if (!currentlyArmed && proxArmed)
                        {
                            foreach(var w in warheads)
                            {
                                w.IsArmed = true;
                            }
                        }
                        AimAtTarget(normalAccelerationVector);
                    }
                    else if (hasTarget)
                    {
                        foreach(IMyGyro gyro in allGyros)
                        {
                            gyro.Pitch = 0f;
                            gyro.Yaw = 0f;
                            gyro.Roll = 0f;
                        }
                        hasTarget = false;
                    }
                }
                else
                {
                    currentClock++;
                    if (currentClock > delayTicks)
                    {
                        homingActive = true;
                        foreach (var g in hydroTanks)
                        {
                            g.Stockpile = false;
                        }
                        foreach (IMyThrust thruster in forwardThrust)
                        {
                            thruster.Enabled = true;
                            thruster.ThrustOverridePercentage = 1f;
                        }
                        targetingTurret.Enabled = true;
                        proximitySensor.Enabled = true;
                        Runtime.UpdateFrequency = UpdateFrequency.Update10;
                    }
                }
            }
        }

        bool GetBlocks(IMyTerminalBlock block)
        {
            if (block is IMyGyro) allGyros.Add((IMyGyro)block);
            else if (block is IMyRemoteControl) missileRemote = (IMyRemoteControl)block;
            else if (block is IMyThrust) forwardThrust.Add((IMyThrust)block);
            else if (block is IMyWarhead) warheads.Add((IMyWarhead)block);
            else if (block is IMySensorBlock) proximitySensor = (IMySensorBlock)block;
            else if (block is IMyShipMergeBlock) mergeBlock = (IMyShipMergeBlock)block;
            else if (block is IMyLargeTurretBase) targetingTurret = (IMyLargeTurretBase)block;
            else if (block is IMyBatteryBlock)
            {
                var b = (IMyBatteryBlock)block;
                b.ChargeMode = ChargeMode.Recharge;
                batteries.Add(b);
            }
            else if (block is IMyPowerProducer)
            {
                var p = (IMyPowerProducer)block;
                p.Enabled = false;
                otherPower.Add(p);
                
            }
            else if (block is IMyGasTank)
            {
                var g = (IMyGasTank)block;
                g.Stockpile = true;
                hydroTanks.Add(g);
            }
            else if (block is IMyShipConnector)
            {
                connector = (IMyShipConnector)block;
            }
            string tag = $"[{blockGroupName}]";
            if (!block.CustomName.StartsWith(tag))
            {
                block.CustomName = $"{tag} - {block.CustomName}";
            }
            return false;
        }
        Vector3D RefreshNavigation()
        {
            var targetPosition = currentTarget.Position;
            var rangeVector = targetPosition - missileRemote.WorldMatrix.Translation;
            var waypointVector = rangeVector;
            var distTargetSq = rangeVector.LengthSquared();

            proxArmed = proxArmed ? proxArmed : distTargetSq < 120 * 120;

            var grav = missileRemote.GetNaturalGravity();
            bool inGrav = grav != Vector3D.Zero;
            // plunging makes no sense in space;
            plunging = inGrav && plunging;
            // Can't cruise in gravity or too close to target
            canCruise = !inGrav && (distTargetSq > CruiseDistSqMin);

            // PLUNGING 
            if (plunging)
            {
                var gravDir = grav;
                gravDir.Normalize();

                var targetHeightDiff = rangeVector.Dot(-gravDir); // Positive if target is higher than missile

                if ((rangeVector.LengthSquared() < PlungeDist * PlungeDist)
                   && targetHeightDiff > 0)
                {
                    plunging = false;
                }

                if (plunging)
                {
                    waypointVector -= gravDir * PlungeDist;
                    if (waypointVector.LengthSquared() < 300 * 300)
                        plunging = false;
                }
            }

            var linearVelocity = missileRemote.GetShipVelocities().LinearVelocity;
            Vector3D velocityVector = currentTarget.Velocity - linearVelocity;
            var speed = missileRemote.GetShipSpeed();

            double alignment = linearVelocity.Dot(ref waypointVector);
            if (alignment > 0)
            {
                Vector3D rangeDivSqVector = waypointVector / waypointVector.LengthSquared();
                Vector3D compensateVector = velocityVector - (velocityVector.Dot(ref waypointVector) * rangeDivSqVector);

                Vector3D targetANVector;
                var targetAccel = (lastTargetVelocity - currentTarget.Velocity) * 0.16667;

                targetANVector = targetAccel - grav - (targetAccel.Dot(ref waypointVector) * rangeDivSqVector);

                bool accelerating = speed > lastSpeed + 1;
                if (accelerating)
                {
                    canCruise = false;
                    AccelerationVector = linearVelocity + (3.5 * 1.5 * (compensateVector + (0.5 * targetANVector)));
                }
                else
                {
                    AccelerationVector = linearVelocity + (3.5 * (compensateVector + (0.5 * targetANVector)));
                }
            }
            // going backwards or perpendicular
            else
            {
                AccelerationVector = (waypointVector * 0.1) + velocityVector;
            }

            lastTargetVelocity = currentTarget.Velocity;
            lastSpeed = speed;

            return Vector3D.TransformNormal(AccelerationVector, MatrixD.Transpose(missileRemote.WorldMatrix));
        }
        void AimAtTarget(Vector3D TargetVector)
        {
            //TargetVector.Normalize();
            //TargetVector += Controller.WorldMatrix.Up * 0.1;

            //---------- Activate Gyroscopes To Turn Towards Target ----------

            double absX = Math.Abs(TargetVector.X);
            double absY = Math.Abs(TargetVector.Y);
            double absZ = Math.Abs(TargetVector.Z);

            double yawInput, pitchInput;
            if (absZ < 0.00001)
            {
                yawInput = pitchInput = MathHelperD.PiOver2;
            }
            else
            {
                bool flipYaw = absX > absZ;
                bool flipPitch = absY > absZ;

                yawInput = FastAT(Math.Max(flipYaw ? (absZ / absX) : (absX / absZ), 0.00001));
                pitchInput = FastAT(Math.Max(flipPitch ? (absZ / absY) : (absY / absZ), 0.00001));

                if (flipYaw) yawInput = MathHelperD.PiOver2 - yawInput;
                if (flipPitch) pitchInput = MathHelperD.PiOver2 - pitchInput;

                if (TargetVector.Z > 0)
                {
                    yawInput = (Math.PI - yawInput);
                    pitchInput = (Math.PI - pitchInput);
                }
            }

            //---------- PID Controller Adjustment ----------

            if (double.IsNaN(yawInput)) yawInput = 0;
            if (double.IsNaN(pitchInput)) pitchInput = 0;

            yawInput *= GetSign(TargetVector.X);
            pitchInput *= GetSign(TargetVector.Y);

            yawInput = yawController.Filter(yawInput, 2);
            pitchInput = pitchController.Filter(pitchInput, 2);

            if (Math.Abs(yawInput) + Math.Abs(pitchInput) > DEF_PD_AIM_LIMIT)
            {
                double adjust = DEF_PD_AIM_LIMIT / (Math.Abs(yawInput) + Math.Abs(pitchInput));
                yawInput *= adjust;
                pitchInput *= adjust;
            }

            //---------- Set Gyroscope Parameters ----------
            ApplyGyroOverride(pitchInput, yawInput, 0);
        }
        void ApplyGyroOverride(double pitch_speed, double yaw_speed, double roll_speed)
        {
            var rotationVec = new Vector3D(-pitch_speed, yaw_speed, roll_speed); //because keen does some weird stuff with signs 
            var shipMatrix = missileRemote.WorldMatrix;
            var relativeRotationVec = Vector3D.TransformNormal(rotationVec, shipMatrix);

            foreach (var thisGyro in allGyros)
            {
                var transformedRotationVec = Vector3D.TransformNormal(relativeRotationVec, Matrix.Transpose(thisGyro.WorldMatrix));

                thisGyro.Pitch = (float)transformedRotationVec.X;
                thisGyro.Yaw = (float)transformedRotationVec.Y;
                thisGyro.Roll = (float)transformedRotationVec.Z;
                thisGyro.GyroOverride = true;
            }
        }
        double FastAT(double x)
        {
            return 0.785375 * x - x * (x - 1.0) * (0.2447 + 0.0663 * x);
        }

        double GetSign(double value)
        {
            return value < 0 ? -1 : 1;
        }
        public class PDController
        {
            double lastInput;

            public double gain_p;
            public double gain_d;

            double second;

            public PDController(double pGain, double dGain, float stepsPerSecond = 60f)
            {
                gain_p = pGain;
                gain_d = dGain;
                second = stepsPerSecond;
            }

            public double Filter(double input, int round_d_digits)
            {
                double roundedInput = Math.Round(input, round_d_digits);

                double derivative = (roundedInput - lastInput) * second;
                lastInput = roundedInput;

                return (gain_p * input) + (gain_d * derivative);
            }

            public void Reset()
            {
                lastInput = 0;
            }
        }