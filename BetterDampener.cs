
        /* Clan "Andromeda"
         * vk.com/andromeda_se
         *
         * Better Dampener v1.7
         * Release 17.03.2022 PM
         * 
         * Аргументы/Arguments:
         * OnOff - Enable/Disable script
         * CruiseAdd [X] - Add croise control speed +X m/s
         * Cruise [X] - Set croise control speed X m/s
         * Cruise 0 - Stop croise control
         * RaycastSync - Get velocity sync from camera (once)
         * ResetSync - Stop velocity sync
         * 
         * Settings in CustomData! 
         * 
         * The settings are loaded when the script is compiled. When changing settings, recompile the script
         * Настройки загружаются при компиляции скрипта. При изменении настроек перкомпилируйте скрипт
         * 
         * Please do not change anything below
         * Пожалуйста не изменяйте ничего ниже
         */


        public static readonly string ScriptName = "Dampener";
        public static readonly string ScriptVersion = "1.7";
        static MsgPackage Settings;
        TextPanel Output;
        ShipControls Ship;
        float CroiseControlSpeed = 0;
        Vector3D VelocityZeroPoint = Vector3D.Zero;
        IMyCameraBlock Camera;
        public Program()
        {
            Settings = MsgPackage.FromString(Me.CustomData);
            Output = new TextPanel(GridTerminalSystem, Me);
            List<IMyTerminalBlock> tmpCams = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(Settings.GetSetting("Camera TAG", "[Dampener camera]"), tmpCams, x => x.CubeGrid == Me.CubeGrid && x is IMyCameraBlock);
            if (tmpCams.Count > 0)
            {
                Camera = (IMyCameraBlock)tmpCams.FirstOrDefault();
                if (Camera != null) Camera.EnableRaycast = true;
            }
			Ship = new ShipControls(GridTerminalSystem, Me);
            Me.CustomData = Settings.ToString();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            
        }
        public void Main(string argument, UpdateType updateSource)
        {
            if (!string.IsNullOrEmpty(argument)) Arguments(argument);
            if (Runtime.UpdateFrequency == UpdateFrequency.None) return;
            if (Ship.UpdateController() || CroiseControlSpeed != 0 || !VelocityZeroPoint.IsZero())
            {
                if (MyRuntime.IsWriteTick)
                {
                    Output.AddLine("Better dampener");
                    float Speed = (float)Ship.Controller.GetShipSpeed();
                    Output.AddLine("Speed:@", Speed.ToString("0.00 m/s"));
                    if (CroiseControlSpeed != 0)
                    {
                        Output.AddLine("Croise:@", CroiseControlSpeed.ToString("0.00 m/s"));
                    }
                    else
                    {
                        Output.AddLine("Sync:@", VelocityZeroPoint.IsZero() ? "Off" : VelocityZeroPoint.Length().ToString("0.00 m/s"));
                    }
                    Output.AddLine("Dampener:@", Ship.Controller.DampenersOverride ? "On" : "Off");
                    Output.Flush();
                }

                if (Runtime.UpdateFrequency != UpdateFrequency.Update1) Runtime.UpdateFrequency = UpdateFrequency.Update1;
                Ship.SetThrust(Ship.GetDampenerForce(CroiseControlSpeed != 0 ? Ship.Controller.WorldMatrix.Forward * CroiseControlSpeed : VelocityZeroPoint));
            }
            else
            {
                if (MyRuntime.IsWriteTick)
                {
                    Output.AddLine("Better dampener");
                    Output.AddLine("Cockpit empty");
                    Output.Flush();
                }
                if (Runtime.UpdateFrequency != UpdateFrequency.Update10) Runtime.UpdateFrequency = UpdateFrequency.Update10;
                Ship.Controller.DampenersOverride = true;
                Ship.SetThrust();
            }
            MyRuntime.Update();
        }

        private void Arguments(string argument)
        {
            var cmd = argument.Split(' ');
            switch (cmd[0])
            {
                case "OnOff":
                    if(Runtime.UpdateFrequency == UpdateFrequency.None)
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.Update1;
                    }
                    else
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.None;
                        Ship.SetThrust();
                        Output.AddLine("Better dampener@v", ScriptVersion);
                        Output.AddLine("@Disabled");
                        Output.Flush();
                    }
                    break;
                case "Cruise":
                    if (cmd.Length > 1)
                    {
                        float.TryParse(cmd[1], out CroiseControlSpeed);
                    }
                    break;
                case "CruiseAdd":
                    if (cmd.Length > 1)
                    {
                        float add;
                        float.TryParse(cmd[1], out add);
                        CroiseControlSpeed += add;
                    }
                    break;
                case "ResetSync":
                    VelocityZeroPoint = Vector3D.Zero;
                    break;
                case "RaycastSync":
                    if(Camera != null)
                    {
                        var Object = Camera.Raycast(Camera.AvailableScanRange);
                        if (Object.Type != MyDetectedEntityType.None)
                        {
                            VelocityZeroPoint = Object.Velocity;
                        }
                        else
                        {
                            VelocityZeroPoint = Vector3D.Zero;
                        }
                    }
                    break;
            }
        }

        class TextPanel
        {
            IMyTextSurface Surface;
            StringBuilder Content = new StringBuilder();
            int OldFlushMaxLen = 1;
            int MaxLen = 1;
            public TextPanel(IMyGridTerminalSystem Terminal, IMyProgrammableBlock Me)
            {
                List<IMyTerminalBlock> LCDS = new List<IMyTerminalBlock>();
                Terminal.SearchBlocksOfName(Settings.GetSetting("Text surface TAG", $"[{ScriptName} status]"), LCDS, x => x.CubeGrid == Me.CubeGrid && (x is IMyTextSurface || x is IMyTextSurfaceProvider));
                if (LCDS.Count > 0)
                {
                    if (LCDS[0] is IMyTextSurfaceProvider)
                    {
                        var id = Settings.GetSetting("Text surface ID", 0);
                        var surfOwn = (LCDS[0] as IMyTextSurfaceProvider);
                        if (id < 0)
                        {
                            id = 0;
                            Settings.GetSetting("Text surface ID", id);
                        }
                        else if (id >= surfOwn.SurfaceCount)
                        {
                            id = surfOwn.SurfaceCount - 1;
                            Settings.GetSetting("Text surface ID", id);
                        }
                        Surface = surfOwn.GetSurface(id);
                    }
                    else if (LCDS[0] is IMyTextSurface)
                    {
                        Surface = LCDS[0] as IMyTextSurface;
                    }
                    Me.GetSurface(0).ContentType = ContentType.NONE;
                }
                else
                {
                    Surface = Me.GetSurface(0);
                }
                Surface.ContentType = ContentType.TEXT_AND_IMAGE;
                Surface.FontSize = 1.387f;
                Surface.Font = "Monospace";
                Surface.Alignment = TextAlignment.CENTER;
                Surface.WriteText("Text panel defined");
            }
            public void Reset()
            {
                Surface.FontSize = 1;
                Surface.ContentType = ContentType.NONE;
            }
            public void AddLine(params string[] line)
            {
                var lined = string.Join("", line);
                var len = lined.Length;
                if (OldFlushMaxLen > len)
                    lined = lined.Replace("@", " ".PadRight(OldFlushMaxLen - len + 1));
                else
                    lined = lined.Replace("@"," ");
                Content.AppendLine(lined);
                if (len > MaxLen)
                {
                    MaxLen = len;
                }
            }
            public void AddProgressBar(float Procentage)
            {
                if (Procentage > 1) Procentage = 1;
                if (Procentage < 0) Procentage = 0;
                string lined = "[".PadRight((int)((OldFlushMaxLen-1) * Procentage), '@');
                Content.AppendLine($"{lined.PadRight(OldFlushMaxLen - 1)}]");
            }
            public void Flush()
            {
                Surface.FontSize = 1f / Math.Max(OldFlushMaxLen, MaxLen) * 25f - 0.1f;
                Surface.WriteText(Content.ToString());
                Content.Clear();
                OldFlushMaxLen = MaxLen;
                MaxLen = 1;
            }
        }
        class ShipControls
        {
            Dictionary<Base6Directions.Direction, ThrustArray> Thrusters = new Dictionary<Base6Directions.Direction, ThrustArray>();

            List<IMyShipController> AllControllers = new List<IMyShipController>();
            public IMyShipController Controller;
            public double MaxThrustOfAll = 0;
            public double DampenerMultiplier;
            public ShipControls(IMyGridTerminalSystem Terminal, IMyProgrammableBlock Me)
            {
                List<IMyThrust> tmpThrust = new List<IMyThrust>();
                Terminal.GetBlocksOfType(tmpThrust, x => x.CubeGrid == Me.CubeGrid);
                foreach (var thrust in tmpThrust)
                {
                    if (Thrusters.ContainsKey(thrust.Orientation.Forward))
                    {
                        Thrusters[thrust.Orientation.Forward].Add(thrust);
                    }
                    else
                    {
                        Thrusters.Add(thrust.Orientation.Forward, new ThrustArray(thrust));
                    }
                    MaxThrustOfAll += thrust.MaxThrust;
                }

                Terminal.GetBlocksOfType(AllControllers, x => x.CubeGrid == Me.CubeGrid);
                Controller = AllControllers.FirstOrDefault(x => x.IsMainCockpit);
                if (Controller == null) Controller = AllControllers.FirstOrDefault();
                DampenerMultiplier = Settings.GetSetting("Dampener multiplier", 10d);
            }
            public bool UpdateController()
            {
                if (Controller.IsUnderControl) return true;
                IMyShipController xs = AllControllers.FirstOrDefault(x => x.IsMainCockpit && x.IsUnderControl);
                if (xs == null) xs = AllControllers.FirstOrDefault(x => x.IsUnderControl);
                if (xs != null)
                {
                    Controller = xs;
                    return true;
                }
                return false;
            }

            public Vector3D GetDampenerForce() => GetDampenerForce(Vector3D.Zero, Vector3D.Zero);
            public Vector3D GetDampenerForce(Vector3D zeroPointVelocity) => GetDampenerForce(zeroPointVelocity, Vector3D.Zero);
            public Vector3D GetDampenerForce(Vector3D zeroPointVelocity, Vector3D zeroPointAcceleration)
            {
                var SMass = Controller.CalculateShipMass();
                Vector3D curVel = Controller.GetShipVelocities().LinearVelocity;
                double multipler = Math.Max(Math.Pow(Math.Min(Math.Sqrt(curVel.Length()) / 0.01, 1), 2), 0.1) * DampenerMultiplier;
                Vector3D Forces = ((Controller.GetNaturalGravity() + zeroPointAcceleration) * SMass.PhysicalMass);
                if (Controller.DampenersOverride) Forces += (curVel - zeroPointVelocity) * multipler * SMass.PhysicalMass;
                if (Controller.MoveIndicator != Vector3.Zero)
                {
                    Vector3D MoveVector = (Vector3D.TransformNormal(Controller.MoveIndicator, Controller.WorldMatrix));
                    Forces -= Vector3D.ProjectOnVector(ref Forces, ref MoveVector) + MoveVector * MaxThrustOfAll;
                }
                return Forces;
            }

            public void SetThrust(Vector3D force)
            {
                foreach (var thrusts in Thrusters)
                {
                    var direction = thrusts.Value.Direction;
                    float directionalForce = (float)Vector3D.Dot(force, direction);
                    if (directionalForce > 0)
                    {
                        thrusts.Value.SetThrust(directionalForce);
                    }
                    else
                    {
                        thrusts.Value.SetThrust(0);
                    }
                }
            }
            public void SetThrust()
            {
                foreach (var thrusts in Thrusters)
                {
                    thrusts.Value.SetThrust();
                }
            }
            class ThrustArray
            {
                public Vector3D Direction => Items.First().WorldMatrix.Forward;
                public float CurrentThrust
                {
                    get
                    {
                        float returs = 0;
                        foreach (var thrs in Items)
                        {
                            if (thrs.IsWorking) returs += thrs.CurrentThrust;
                        }
                        return returs;
                    }
                }
                public float CurrentThrustProcent
                {
                    get
                    {
                        var th = Items.FirstOrDefault(x => x.IsWorking && x.MaxEffectiveThrust != 0);
                        if (th == null) return 0;
                        return th.CurrentThrust / th.MaxEffectiveThrust;
                    }
                }
                public float MaxEffectiveThrust = 0;
                public List<IMyThrust> Items = new List<IMyThrust>();
                public ThrustArray(IMyThrust thruster)
                {
                    Items.Add(thruster);
                    MaxEffectiveThrust += thruster.MaxEffectiveThrust;
                }
                public void Add(IMyThrust thruster)
                {
                    Items.Add(thruster);
                    MaxEffectiveThrust += thruster.MaxEffectiveThrust;
                }
                public void SetThrust(float thrust)
                {
                    var maxEffectiveThrust = 0f;
                    foreach (var thrs in Items)
                    {
                        if (thrs.IsWorking)
                        {
                            thrs.ThrustOverride = thrust * (thrs.MaxThrust / MaxEffectiveThrust);
                            maxEffectiveThrust += thrs.MaxEffectiveThrust;
                        }
                    }
                    MaxEffectiveThrust = maxEffectiveThrust;
                }
                public void SetThrust()
                {
                    foreach (var thrs in Items)
                    {
                        thrs.ThrustOverride = 0;
                    }
                }
            }
        }
        class MsgPackage
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            public static MsgPackage FromString(string data)
            {
                MsgPackage pack = new MsgPackage();
                string[] args = data.Split('\n');
                if (args.Length > 0)
                    for (int i = 0; i < args.Length; i++)
                    {
                        var keyvalue = args[i].Split(new char[] { ':' }, 2);
                        if (keyvalue.Length > 1) pack.Params.Add(keyvalue[0], keyvalue[1].Trim());
                    }
                return pack;
            }
            public override string ToString()
            {
                StringBuilder save = new StringBuilder();
                foreach (var keyvalue in Params)
                {
                    save.Append(keyvalue.Key).Append(": ");
                    save.Append(keyvalue.Value).Append("\n");
                }
                return save.ToString();
            }
            public T GetSetting<T>(string key, T @default)
            {
                if (Params.ContainsKey(key))
                {
                    return (T)Convert.ChangeType(Params[key], typeof(T));
                }
                else
                {
                    Params.Add(key, @default.ToString());
                    return @default;
                }
            }
            public T Get<T>(string key)
            {
                if (Params.ContainsKey(key))
                {
                    return (T)Convert.ChangeType(Params[key], typeof(T));
                }
                else
                {
                    return default(T);
                }
            }
            public void Set<T>(string key, T value)
            {
                if (Params.ContainsKey(key))
                {
                    Params[key] = value.ToString();
                }
                else
                {
                    Params.Add(key, value.ToString());
                }
            }
        }
        static class MyRuntime
        {
            public static uint Tick = 0;
            public static bool IsWriteTick => Tick % 10 == 0;
            public static void Update()
            {
                Tick++;
            }
            
        }
