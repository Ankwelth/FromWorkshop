        /* Clan "Andromeda"
         * vk.com/andromeda_se
         *
         * Minership Subsystem
         * Release 17.03.2022 PM
         * 
         * Аргументы/Arguments:
         * OnOff - Enable/Disable script
         * 
         * Settings in CustomData! 
         * 
         * The settings are loaded when the script is compiled. When changing settings, recompile the script
         * Настройки загружаются при компиляции скрипта. При изменении настроек перкомпилируйте скрипт
         * 
         * Please do not change anything below
         * Пожалуйста не изменяйте ничего ниже
         */


        public static readonly string ScriptName = "Miner";
        static MsgPackage Settings;
        TextPanel Output;
        List<IMyThrust> Thrusters = new List<IMyThrust>();
        List<IMyInventory> Inventories = new List<IMyInventory>();
        List<IMyInventory> HidenInventories = new List<IMyInventory>();
        List<IMyBatteryBlock> Battery = new List<IMyBatteryBlock>();
        List<IMyConveyorSorter> Sorters = new List<IMyConveyorSorter>();
        List<IMyGasTank> GasTanks = new List<IMyGasTank>();
        IMyShipController Controller;
        List<IMyShipConnector> Connectors = new List<IMyShipConnector>();
        List<IMyLandingGear> LandGears = new List<IMyLandingGear>();
        List<IMyTerminalBlock> AllBlocks = new List<IMyTerminalBlock>();
        IMySoundBlock WarningSoundBlock;
        float InventorySizeMultipler = 1;
        float MaxWeightNewtons = 0;
        float BaseMass;
        bool OverloadThrowOut = false;
        bool AutoUnload = true;
        bool WeAreInSpace = false;
        IMyTimerBlock ConnectTimer, DisconnectTimer, LandingTimer, TakeoffTimer;
        public Program()
        {
            Settings = MsgPackage.FromString(Me.CustomData);
            Output = new TextPanel(GridTerminalSystem, Me);
            
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            
            GridTerminalSystem.GetBlocksOfType(AllBlocks, x => x.CubeGrid == Me.CubeGrid);
            Dictionary<Base6Directions.Direction, double> thrustDirections = new Dictionary<Base6Directions.Direction, double>();
            var conTimerTag = Settings.GetSetting("Connect timer TAG", "[Connect timer]");
            var disconTimerTag = Settings.GetSetting("Disconnect timer TAG", "[Disconnect timer]");
            var landingTimerTag = Settings.GetSetting("Landing timer TAG", "[Landing timer]");
            var takeoffTimerTag = Settings.GetSetting("Takeoff timer TAG", "[Takeoff timer]");
            foreach (var block in AllBlocks)
            {
                switch (block.BlockDefinition.TypeIdString)
                {
                    case "MyObjectBuilder_RemoteControl":
                        if(Controller == null) Controller = (IMyShipController)block;
                        break;
                    case "MyObjectBuilder_Cockpit":
                        Controller = (IMyShipController)block;
                        if(Controller.HasInventory) InventorySizeMultipler = (float)block.GetInventory(0).MaxVolume / 1.000f;
                        break;
                    case "MyObjectBuilder_ShipConnector":
                        switch (block.BlockDefinition.SubtypeId)
                        {
                            case "ConnectorMedium":
                                var connector = (IMyShipConnector)block;
                                Connectors.Add(connector);
                                InventorySizeMultipler = (float)block.GetInventory(0).MaxVolume / 1.152f;
                                break;
                            case "ConnectorSmall":
                                InventorySizeMultipler = (float)block.GetInventory(0).MaxVolume / 0.064f;
                                HidenInventories.Add(block.GetInventory(0));
                                continue;
                        }
                        break;
                    case "MyObjectBuilder_BatteryBlock":
                        Battery.Add((IMyBatteryBlock)block);
                        break;
                    case "MyObjectBuilder_ConveyorSorter":
                        Sorters.Add((IMyConveyorSorter)block);
                        switch (block.BlockDefinition.SubtypeId)
                        {
                            case "MediumBlockConveyorSorter":
                                InventorySizeMultipler = (float)block.GetInventory(0).MaxVolume / 0.343f;
                                break;
                            case "SmallBlockConveyorSorter":
                                InventorySizeMultipler = (float)block.GetInventory(0).MaxVolume / 0.064f;
                                break;
                        }
                        block.CustomData = block.BlockDefinition.ToString();
                        HidenInventories.Add(block.GetInventory(0));
                        continue;
                    case "MyObjectBuilder_Reactor":
                        HidenInventories.Add(block.GetInventory(0));
                        if(block.BlockDefinition.SubtypeId.Contains("SmallBlockSmallGenerator"))
                            InventorySizeMultipler = (float)block.GetInventory(0).MaxVolume / 0.125f;
                        continue;
                    case "MyObjectBuilder_OxygenGenerator":
                        HidenInventories.Add(block.GetInventory(0));
                        if(block.BlockDefinition.SubtypeId == "OxygenGeneratorSmall")
                            InventorySizeMultipler = (float)block.GetInventory(0).MaxVolume / 1.000f;
                        break;
                    case "MyObjectBuilder_SoundBlock":
                        WarningSoundBlock = (IMySoundBlock)block;
                        WarningSoundBlock.SelectedSound = "Alert 2";
                        break;
                    case "MyObjectBuilder_LandingGear":
                        LandGears.Add((IMyLandingGear)block);
                        break;
                    case "MyObjectBuilder_TimerBlock":
                        var blocs = (IMyTimerBlock)block;
                        if (blocs.CustomName.Contains(conTimerTag))
                        {
                            ConnectTimer = blocs;
                        }
                        else if (blocs.CustomName.Contains(disconTimerTag))
                        {
                            DisconnectTimer = blocs;
                        }
                        if (blocs.CustomName.Contains(landingTimerTag))
                        {
                            LandingTimer = blocs;
                        }
                        else if (blocs.CustomName.Contains(takeoffTimerTag))
                        {
                            TakeoffTimer = blocs;
                        }
                        break;
                    case "MyObjectBuilder_Thrust":
                        var thruster = (IMyThrust)block;
                        Thrusters.Add(thruster);
                        if (thrustDirections.ContainsKey(thruster.Orientation.Forward))
                        {
                            thrustDirections[thruster.Orientation.Forward] += thruster.MaxThrust;
                        }
                        else
                        {
                            thrustDirections.Add(thruster.Orientation.Forward, thruster.MaxThrust);
                        }
                        if (thrustDirections[thruster.Orientation.Forward] > MaxWeightNewtons)
                        {
                            MaxWeightNewtons = (float)thrustDirections[thruster.Orientation.Forward];
                        }
                        break;
                    case "MyObjectBuilder_OxygenTank":
                        if (block.BlockDefinition.SubtypeId.Contains("Hydrogen"))
                        {
                            GasTanks.Add((IMyGasTank)block);
                        }
                        break;
                    default:
                        block.CustomData = (block.BlockDefinition.ToString());
                        break;
                }
                if (block.HasInventory && !(block is IMyGasGenerator || block is IMyReactor))
                {
                    for (int i = 0; i < block.InventoryCount; i++)
                    {
                        Inventories.Add(block.GetInventory(i));
                    }
                }
            }
            if (Settings.GetSetting("Can fly upside down", false))
            {
                foreach (var x in thrustDirections)
                {
                    if (x.Value < MaxWeightNewtons)
                    {
                        MaxWeightNewtons = (float)x.Value;
                    }
                }
            }
            Echo(InventorySizeMultipler.ToString("0.00"));
            OverloadThrowOut = Settings.GetSetting("Emergency cargo dumping",true);
            AutoUnload = Settings.GetSetting("Automatic cargo unloading", true);
            BaseMass += Controller.CalculateShipMass().BaseMass;
            Me.CustomData = Settings.ToString();
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            if (!string.IsNullOrEmpty(argument)) Arguments(argument);
            if (Runtime.UpdateFrequency == UpdateFrequency.None) return;
            if (MyRuntime.IsWriteTick)
            {
                Output.AddLine("Minership subsystem");
                float load = 0;
                if (!WeAreInSpace && Thrusters.Count > 0)
                {
                    load = GetWeightProcentage();
                    Output.AddLine($"Load:@{load:0.0%}");
                    Output.AddProgressBar(load);
                }
                else
                {
                    WeAreInSpace = Controller.GetNaturalGravity().IsZero();
                }
                if (Inventories.Count > 0)
                {
                    var cargo = GetCargoProcentage();
                    Output.AddLine($"Cargo:@{cargo:0.0%}");
                    Output.AddProgressBar(cargo);
                }
                if (Battery.Count > 0)
                {
                    var batt = GetBatteryCharge();
                    Output.AddLine($"Batt:@{batt:0.0%}");
                    Output.AddProgressBar(batt);
                }
                if (GasTanks.Count > 0)
                {
                    var gas = GetGasCharge();
                    Output.AddLine($"Gas:@{gas:0.0%}");
                    Output.AddProgressBar(gas);
                }
                OverloadUpdate(load > 0.98);

                ConnectedUpdate(Connectors.FirstOrDefault(x => x.Status == MyShipConnectorStatus.Connected));
                LandUpdate(LandGears.FirstOrDefault(x => x.IsLocked) != null);
                CheckDamage();
                Output.Flush();
            }
            MyRuntime.Update();
        }
        bool LastConnectionValue = false;
        Random Rnd = new Random();
        bool LastOverloadValue = false;
        bool LastLandUpfate = false;
        void OverloadUpdate(bool overload)
        {
            if (LastOverloadValue != overload)
            {
                LastOverloadValue = overload;
                if (OverloadThrowOut)
                {
                    foreach (var con in Connectors)
                    {
                        con.ThrowOut = overload;
                        con.CollectAll = overload;
                    }
                }
            }
            if (overload)
            {
                if (OverloadThrowOut && Connectors.Count > 0) Output.AddLine("CARGO@DUMPING");
                else Output.AddLine("OVERLOAD@WARNING");
                if(WarningSoundBlock != null) WarningSoundBlock.Play();
            }
        }
        bool DamageWarning = false;
        void CheckDamage()
        {
            if (MyRuntime.IsHardLoadTick)
            {
                var Damaged = GetDamagedBlocks();
                if (Damaged.Count > 0)
                {
                    DamageWarning = true;
                    Echo("Damaged blocks:");
                    foreach (var str in Damaged) Echo(str);
                }
                else DamageWarning = false;
                
            }
            if (DamageWarning)
            {
                Output.AddLine("DAMAGE@WARNING");
                if (WarningSoundBlock != null) WarningSoundBlock.Play();
            }
        }
        void LandUpdate(bool connected)
        {
            if (LastLandUpfate != connected)
            {
                if (connected)
                {
                    if (LandingTimer != null) LandingTimer.Trigger();
                }
                else
                {
                    if (TakeoffTimer != null) TakeoffTimer.Trigger();
                }
                LastLandUpfate = connected;
                foreach (var srt in Thrusters)
                {
                    srt.Enabled = !connected;
                }
            }
        }
        void ConnectedUpdate(IMyShipConnector connectedConnector)
        {
            var connected = connectedConnector != null;
            if (LastConnectionValue != connected)
            {
                if (connected)
                {
                    if (ConnectTimer != null) ConnectTimer.Trigger();
                }
                else
                {
                    if (DisconnectTimer != null) DisconnectTimer.Trigger();
                }
                LastConnectionValue = connected;
                bool safetyContains = false;
                foreach (var btt in Battery.OrderBy(x => Rnd.Next()))
                {
                    if (!safetyContains)
                    {
                        btt.ChargeMode = ChargeMode.Auto;
                        safetyContains = true;
                    }
                    else btt.ChargeMode = connected ? ChargeMode.Recharge : ChargeMode.Auto;
                }
                foreach (var srt in Sorters)
                {
                    srt.Enabled = !connected;
                }
                foreach (var srt in Thrusters)
                {
                    srt.Enabled = !connected;
                }
                foreach (var srt in GasTanks)
                {
                    srt.Stockpile = connected;
                }
            }
            if (AutoUnload && connected && ContainsSomething.Count > 0)
            {
                var grid = connectedConnector.OtherConnector.CubeGrid;
                List<IMyCargoContainer> tmpContainers = new List<IMyCargoContainer>();
                GridTerminalSystem.GetBlocksOfType(tmpContainers, x => x.CubeGrid == grid);
                var notEmpty = GetNotEmptyInventory();

                foreach (var inv in tmpContainers)
                {
                    var inventory = inv.GetInventory();
                    TransferTo(inventory, ref notEmpty);
                    if (notEmpty == null) break;
                }
            }
        }
        void TransferTo(IMyInventory inventory, ref IMyInventory notEmpty)
        {
            List<MyInventoryItem> notEmptyItems = new List<MyInventoryItem>();
            while (!inventory.IsFull)
            {
                if (notEmpty.ItemCount == 0)
                {
                    notEmpty = GetNotEmptyInventory();
                    if (notEmpty == null) return;
                }
                notEmpty.GetItems(notEmptyItems);
                foreach (var item in notEmptyItems)
                {
                    if (!inventory.TransferItemFrom(notEmpty, item)) return;
                }
            }
        }
        List<IMyInventory> ContainsSomething = new List<IMyInventory>();
        IMyInventory GetNotEmptyInventory()
        {
            if (ContainsSomething.Count > 0)
            {
                var inv = ContainsSomething[0];
                ContainsSomething.RemoveAt(0);
                return inv;
            }
            return null;
        }
        float GetGasCharge()
        {
            if (GasTanks.Count == 0) return 0;
            double max = 0, current = 0;
            foreach (var batt in GasTanks)
            {
                max++;
                current += batt.FilledRatio;
            }
            return (float)(current / max);
        }
        float GetBatteryCharge()
        {
            if (Battery.Count == 0) return 0;
            float max = 0, current = 0;
            foreach (var batt in Battery)
            {
                max += batt.MaxStoredPower;
                current += batt.CurrentStoredPower;
            }
            return current / max;
        }
        float GetCargoProcentage()
        {
            if (Inventories.Count == 0) return 0;
            float max = 0, current = 0;
            InventoryMass = 0;
            ContainsSomething.Clear();
            foreach (var inventory in Inventories)
            {
                max += (float)inventory.MaxVolume;
                var mcurrent = (float)inventory.CurrentVolume;
                if (mcurrent > 0) ContainsSomething.Add(inventory);
                current += mcurrent;
                InventoryMass += (float)inventory.CurrentMass;
            }
            foreach (var inv in HidenInventories)
            {
                InventoryMass += (float)inv.CurrentMass;
            }
            return current / max;
        }
        float InventoryMass = 0;
        float GetWeightProcentage()
        {
            var gravvec = Controller.GetNaturalGravity();
            var gravity = gravvec.Length();
            WeAreInSpace = gravvec.IsZero();
            var massNewtons = BaseMass * gravity;
            return (float)((InventoryMass / InventorySizeMultipler) / ((MaxWeightNewtons * GetThrustCoefficient() - massNewtons) / gravity));

        }
        double GetThrustCoefficient()
        {
            var thrs = Thrusters.Find(x => x.IsWorking);
            if (thrs != null) return thrs.MaxEffectiveThrust / thrs.MaxThrust;
            else return 0.1;
        }
        List<string> GetDamagedBlocks()
        {
            List<string> damaged = new List<string>();
            foreach (var block in AllBlocks)
            {
                if (!block.IsFunctional || Me.CubeGrid.GetCubeBlock(block.Position) == null)
                {
                    damaged.Add($"{block.CustomName}");
                }
            }
            return damaged;
        }
        private void Arguments(string argument)
        {
            var cmd = argument.Split(' ');
            switch (cmd[0])
            {
                case "OnOff":
                    if(Runtime.UpdateFrequency == UpdateFrequency.None)
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.Update10;
                    }
                    else
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.None;
                        Output.AddLine("Minership Subsystem");
                        Output.AddLine("@Disabled");
                        Output.Flush();
                    }
                    break;
            }
        }

        class TextPanel
        {
            IMyTextSurface Surface;
            StringBuilder Content = new StringBuilder();
            int OldFlushMaxLen = 5;
            int MaxLen = 4;
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
                if (!float.IsNaN(Procentage))
                {
                    if (Procentage > 1) Procentage = 1;
                    if (Procentage <= 0) Procentage = 0.001f;
                    string lined = "[".PadRight((int)((OldFlushMaxLen - 1) * Procentage), '|');
                    Content.AppendLine($"{lined.PadRight(OldFlushMaxLen - 1)}]");
                }
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
            public static bool IsWriteTick => Tick % 3 == 0;
            public static bool IsHardLoadTick => Tick % 20 == 0;
            public static void Update()
            {
                Tick++;
            } 
        }
