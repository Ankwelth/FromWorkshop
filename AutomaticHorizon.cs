float GyroSense = 5, YawSense = 0.5f;
        bool YawFix = false, PitchFix = true, RollFix = true;

        IMyCockpit cockPit;
        List<IMyGyro> gyros;
        bool SuccessfullyCompiled, IsRunning;

        public Program()
        {
            GetAutoHorizonSystem();
            if (SuccessfullyCompiled) Echo("Script is ready to be called");
            Runtime.UpdateFrequency = UpdateFrequency.None;
        }
        string GetValueOfKey(string Key)
        {
            int i0 = Me.CustomData.IndexOf(Key);
            int i1 = Me.CustomData.IndexOf(";", i0);
            return Me.CustomData.Substring(i0 + Key.Length + 1, i1 - i0 - Key.Length - 1);
        }
        void ClearAutoHorizonSystem()
        {
            cockPit = null;
            if (gyros != null) gyros.Clear();
            gyros = null;
        }
        void GetAutoHorizonSystem()
        {
            ClearAutoHorizonSystem();
            SuccessfullyCompiled = true;

            // Getting Cockpit
            try
            {
                List<IMyCockpit> allCockpits = new List<IMyCockpit>();
                List<IMyCockpit> cockpits = new List<IMyCockpit>();
                GridTerminalSystem.GetBlocksOfType<IMyCockpit>(allCockpits);
                foreach (IMyCockpit cockpit in allCockpits) if (cockpit.CubeGrid == Me.CubeGrid) cockpits.Add(cockpit);
                if (cockpits.Count == 0) { SuccessfullyCompiled = false; Echo("No cockpits in this Grid"); }
                else
                {
                    foreach (IMyCockpit cockpit in cockpits) if (cockpit.IsMainCockpit) cockPit = cockpit;
                    if (cockPit == null) { SuccessfullyCompiled = false; Echo("Main cockpit not found"); }
                }
            }
            catch { SuccessfullyCompiled = false; Echo("Exception while getting Cockpit (don't know why)"); }

            // Getting Gyros
            try
            {
                List<IMyGyro> allGyros = new List<IMyGyro>();
                gyros = new List<IMyGyro>();
                GridTerminalSystem.GetBlocksOfType<IMyGyro>(allGyros);
                foreach (IMyGyro gyro in allGyros) if (gyro.CubeGrid == Me.CubeGrid) gyros.Add(gyro);
                if (gyros.Count == 0) { SuccessfullyCompiled = false; Echo("No gyros in this Grid"); }
            }
            catch (Exception ex) { SuccessfullyCompiled = false; Echo(ex.Message);/*Echo("Exception while getting Gyros (don't know why)"); */}

            // Getting Gyro Sense
            try { GyroSense = Convert.ToSingle(GetValueOfKey("GYRO_SENSE")); }
            catch { SuccessfullyCompiled = false; Echo("Key GYRO_SENSE not found"); }

            // Getting Yaw Sense
            try { YawSense = Convert.ToSingle(GetValueOfKey("YAW_SENSE")); }
            catch { SuccessfullyCompiled = false; Echo("Key YAW_SENSE not found"); }

            // Getting Yaw Fix
            try
            {
                string tmp = Convert.ToString(GetValueOfKey("YAW_FIX"));
                tmp = tmp.ToLower();
                if (tmp == "true") YawFix = true;
                else if (tmp == "false") YawFix = false;
            }
            catch { SuccessfullyCompiled = false; Echo("Key YAW_FIX not found"); }

            // Getting Pitch Fix
            try
            {
                string tmp = Convert.ToString(GetValueOfKey("PITCH_FIX"));
                tmp = tmp.ToLower();
                if (tmp == "true") PitchFix = true;
                else if (tmp == "false") PitchFix = false;
            }
            catch { SuccessfullyCompiled = false; Echo("Key PITCH_FIX not found"); }

            // Getting Roll Fix
            try
            {
                string tmp = Convert.ToString(GetValueOfKey("ROLL_FIX"));
                tmp = tmp.ToLower();
                if (tmp == "true") RollFix = true;
                else if (tmp == "false") RollFix = false;
            }
            catch { SuccessfullyCompiled = false; Echo("Key ROLL_FIX not found"); }
        }

        #region Formulas
        float Yaw() { return (YawFix) ? 0 : (YawSense * cockPit.RotationIndicator.Y); }
        float Pitch() { return (PitchFix) ? (-GyroSense * (float)Vector3D.Dot(Vector3D.Normalize(cockPit.GetNaturalGravity()), cockPit.WorldMatrix.Forward)) : (YawSense * cockPit.RotationIndicator.X); }
        float Roll() { return (RollFix) ? (GyroSense * (float)Vector3D.Dot(Vector3D.Normalize(cockPit.GetNaturalGravity()), cockPit.WorldMatrix.Left)) : (YawSense * cockPit.RollIndicator); }

        struct AxisOrientation
        {
            static string GetOrientation(char a, int sign)
            {
                if (a == 'F')
                {
                    if (sign == 1) return "Forward";
                    else return "Backward";
                }
                else if (a == 'U')
                {
                    if (sign == 1) return "Up";
                    else return "Down";
                }
                else if (a == 'L')
                {
                    if (sign == 1) return "Left";
                    else return "Right";
                }
                else return "Unknown";
            }
            public char F, U, L;
            public int signF, signU, signL;

            public override string ToString()
            {
                return (GetOrientation(F, signF) + "; " + GetOrientation(U, signU) + "; " + GetOrientation(L, signL));
            }
        }
        char Orientation(string a)
        {
            if (a == "Forward" || a == "Backward") return 'F';
            if (a == "Up" || a == "Down") return 'U';
            if (a == "Left" || a == "Right") return 'L';
            return 'N';
        }
        int OrientationSign(string a)
        {
            if (a == "Backward" || a == "Down" || a == "Right") return -1;
            else return 1;
        }
        AxisOrientation Orientations(IMyCubeBlock a)
        {
            AxisOrientation result = new AxisOrientation();
            result.F = Orientation(a.Orientation.Forward.ToString());
            result.U = Orientation(a.Orientation.Up.ToString());
            result.L = Orientation(a.Orientation.Left.ToString());
            result.signF = OrientationSign(a.Orientation.Forward.ToString());
            result.signU = OrientationSign(a.Orientation.Up.ToString());
            result.signL = OrientationSign(a.Orientation.Left.ToString());
            return result;
        }
        AxisOrientation GyroToCockpitPosition(ref AxisOrientation gyro, ref AxisOrientation cockpit)
        {
            AxisOrientation result = new AxisOrientation();
            result.signF = result.signU = result.signL = 0;
            {//Forward
                if (cockpit.F == gyro.F)
                {
                    result.F = 'F';
                    result.signF = gyro.signF * cockpit.signF;
                }
                else if (cockpit.F == gyro.U)
                {
                    result.U = 'F';
                    result.signU = gyro.signU * cockpit.signF;
                }
                else if (cockpit.F == gyro.L)
                {
                    result.L = 'F';
                    result.signL = gyro.signL * cockpit.signF;
                }
            }
            {//Up
                if (cockpit.U == gyro.F)
                {
                    result.F = 'U';
                    result.signF = gyro.signF * cockpit.signU;
                }
                else if (cockpit.U == gyro.U)
                {
                    result.U = 'U';
                    result.signU = gyro.signU * cockpit.signU;
                }
                else if (cockpit.U == gyro.L)
                {
                    result.L = 'U';
                    result.signL = gyro.signL * cockpit.signU;
                }
            }
            {//Left
                if (cockpit.L == gyro.F)
                {
                    result.F = 'L';
                    result.signF = gyro.signF * cockpit.signL;
                }
                else if (cockpit.L == gyro.U)
                {
                    result.U = 'L';
                    result.signU = gyro.signU * cockpit.signL;
                }
                else if (cockpit.L == gyro.L)
                {
                    result.L = 'L';
                    result.signL = gyro.signL * cockpit.signL;
                }
            }
            return result;
        }
        #endregion

        void SetRotations(IMyGyro gyro, ref AxisOrientation gyroOri)
        {
            if (gyroOri.F == 'F' && gyroOri.U == 'U' && gyroOri.L == 'L')  //FUL n FDR n BUR n BDL
            {
                if (gyroOri.signF == 1 && gyroOri.signU == 1 && gyroOri.signL == 1) //FUL
                {
                    gyro.Yaw = Yaw();
                    gyro.Pitch = Pitch();
                    gyro.Roll = Roll();
                }
                else if (gyroOri.signF == 1 && gyroOri.signU == -1 && gyroOri.signL == -1) //FDR
                {
                    gyro.Yaw = -Yaw();
                    gyro.Pitch = -Pitch();
                    gyro.Roll = Roll();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == 1 && gyroOri.signL == -1) //BUR
                {
                    gyro.Yaw = Yaw();
                    gyro.Pitch = -Pitch();
                    gyro.Roll = -Roll();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == -1 && gyroOri.signL == 1) //BDL
                {
                    gyro.Yaw = -Yaw();
                    gyro.Pitch = Pitch();
                    gyro.Roll = -Roll();
                }
            }
            else if (gyroOri.F == 'F' && gyroOri.U == 'L' && gyroOri.L == 'U') // FLD n FRU n BRD n BLU
            {
                if (gyroOri.signF == 1 && gyroOri.signU == 1 && gyroOri.signL == -1) //FLD
                {
                    gyro.Yaw = -Pitch();
                    gyro.Pitch = Yaw();
                    gyro.Roll = Roll();
                }
                else if (gyroOri.signF == 1 && gyroOri.signU == -1 && gyroOri.signL == 1) //FRU
                {
                    gyro.Yaw = Pitch();
                    gyro.Pitch = -Yaw();
                    gyro.Roll = Roll();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == -1 && gyroOri.signL == -1) //BRD
                {
                    gyro.Yaw = Pitch();
                    gyro.Pitch = Yaw();
                    gyro.Roll = -Roll();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == 1 && gyroOri.signL == 1) //BLU
                {
                    gyro.Yaw = -Pitch();
                    gyro.Pitch = -Yaw();
                    gyro.Roll = -Roll();
                }
            }
            else if (gyroOri.F == 'U' && gyroOri.U == 'F' && gyroOri.L == 'L') // UFR n UBL n DFL n DBR
            {
                if (gyroOri.signF == 1 && gyroOri.signU == 1 && gyroOri.signL == -1) //UFR
                {
                    gyro.Yaw = -Roll();
                    gyro.Pitch = -Pitch();
                    gyro.Roll = -Yaw();
                }
                else if (gyroOri.signF == 1 && gyroOri.signU == -1 && gyroOri.signL == 1) //UBL
                {
                    gyro.Yaw = Roll();
                    gyro.Pitch = Pitch();
                    gyro.Roll = -Yaw();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == 1 && gyroOri.signL == 1) //DFL
                {
                    gyro.Yaw = -Roll();
                    gyro.Pitch = Pitch();
                    gyro.Roll = Yaw();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == -1 && gyroOri.signL == -1) //DBR
                {
                    gyro.Yaw = Roll();
                    gyro.Pitch = -Pitch();
                    gyro.Roll = Yaw();
                }
            }
            else if (gyroOri.F == 'U' && gyroOri.U == 'L' && gyroOri.L == 'F') // ULF n URB n DLB n DRF
            {
                if (gyroOri.signF == 1 && gyroOri.signU == 1 && gyroOri.signL == 1) //ULF
                {
                    gyro.Yaw = -Pitch();
                    gyro.Pitch = Roll();
                    gyro.Roll = -Yaw();
                }
                else if (gyroOri.signF == 1 && gyroOri.signU == -1 && gyroOri.signL == -1) //URB
                {
                    gyro.Yaw = Pitch();
                    gyro.Pitch = -Roll();
                    gyro.Roll = -Yaw();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == 1 && gyroOri.signL == -1) //DLB
                {
                    gyro.Yaw = -Pitch();
                    gyro.Pitch = -Roll();
                    gyro.Roll = Yaw();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == -1 && gyroOri.signL == 1) //DRF
                {
                    gyro.Yaw = Pitch();
                    gyro.Pitch = Roll();
                    gyro.Roll = Yaw();
                }
            }
            else if (gyroOri.F == 'L' && gyroOri.U == 'U' && gyroOri.L == 'F') // LUB n LDF n RUF n RDB
            {
                if (gyroOri.signF == 1 && gyroOri.signU == 1 && gyroOri.signL == -1) //LUB
                {
                    gyro.Yaw = Yaw();
                    gyro.Pitch = -Roll();
                    gyro.Roll = Pitch();
                }
                else if (gyroOri.signF == 1 && gyroOri.signU == -1 && gyroOri.signL == 1) //LDF
                {
                    gyro.Yaw = -Yaw();
                    gyro.Pitch = Roll();
                    gyro.Roll = Pitch();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == 1 && gyroOri.signL == 1) //RUF
                {
                    gyro.Yaw = Yaw();
                    gyro.Pitch = Roll();
                    gyro.Roll = -Pitch();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == -1 && gyroOri.signL == -1) //RDB
                {
                    gyro.Yaw = -Yaw();
                    gyro.Pitch = -Roll();
                    gyro.Roll = -Pitch();
                }
            }
            else if (gyroOri.F == 'L' && gyroOri.U == 'F' && gyroOri.L == 'U') // LFU n LBD n RBU n RFD
            {
                if (gyroOri.signF == 1 && gyroOri.signU == 1 && gyroOri.signL == 1) //LFU
                {
                    gyro.Yaw = -Roll();
                    gyro.Pitch = -Yaw();
                    gyro.Roll = Pitch();
                }
                else if (gyroOri.signF == 1 && gyroOri.signU == -1 && gyroOri.signL == -1) //LBD
                {
                    gyro.Yaw = Roll();
                    gyro.Pitch = Yaw();
                    gyro.Roll = Pitch();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == -1 && gyroOri.signL == 1) //RBU
                {
                    gyro.Yaw = Roll();
                    gyro.Pitch = -Yaw();
                    gyro.Roll = -Pitch();
                }
                else if (gyroOri.signF == -1 && gyroOri.signU == 1 && gyroOri.signL == -1) //RFD
                {
                    gyro.Yaw = -Roll();
                    gyro.Pitch = Yaw();
                    gyro.Roll = -Pitch();
                }
            }
        }
        void StopGyros()
        {
            if (gyros != null) if (gyros.Count != 0) foreach (IMyGyro gyro in gyros)
                    {
                        gyro.Yaw = gyro.Pitch = gyro.Roll = 0;
                        gyro.GyroOverride = false;
                    }
        }
        void StartGyros()
        {
            if (!SuccessfullyCompiled) { Echo("AutoHorizonSystem is broken"); return; }
            else if (cockPit.GetNaturalGravity().IsZero()) foreach (IMyGyro gyro in gyros) gyro.GyroOverride = false;
            else
            {

                AxisOrientation gyroOrientation, cockPitOrientation;
                cockPitOrientation = Orientations(cockPit);
                foreach (IMyGyro gyro in gyros)
                {
                    gyroOrientation = Orientations(gyro);
                    gyroOrientation = GyroToCockpitPosition(ref gyroOrientation, ref cockPitOrientation);
                    SetRotations(gyro, ref gyroOrientation);
                    gyro.GyroOverride = true;
                }
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (SuccessfullyCompiled) Echo("Script started");
            else
            {
                Echo("Script is no ready because of error through constructing");
                updateSource = UpdateType.None;
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
            GetAutoHorizonSystem();
            switch (updateSource)
            {
                case UpdateType.Trigger:
                    try
                    {
                        switch (argument.ToUpper())
                        {
                            case "START":
                                if (IsRunning)
                                {
                                    IsRunning = false;
                                    StopGyros();
                                    updateSource = UpdateType.None;
                                    Runtime.UpdateFrequency = UpdateFrequency.None;
                                    Echo("Script stopped");
                                }
                                else
                                {
                                    IsRunning = true;
                                    StartGyros();
                                    updateSource = UpdateType.Update1;
                                    Runtime.UpdateFrequency = UpdateFrequency.Update1;
                                    Echo("Script started");
                                }
                                break;
                            default:
                                Echo("Unknown argument");
                                break;
                        }
                    }
                    catch
                    {
                        IsRunning = false;
                        StopGyros();
                        updateSource = UpdateType.None;
                        Runtime.UpdateFrequency = UpdateFrequency.None;
                        switch (argument.ToUpper())
                        {
                            case "START":
                                if (IsRunning) Echo("Exception while starting script");
                                else Echo("Exception while stopping script");
                                break;
                            default:
                                Echo("Exception while calling script");
                                break;
                        }
                    }
                    break;
                case UpdateType.Update1:
                    StartGyros();
                    break;
            }
        }