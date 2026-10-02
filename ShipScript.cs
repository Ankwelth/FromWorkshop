        // Either put desired Cockpits name below or set Cockpit as Main Cockpit in Control Panel

        string cockpitName = "";    // For example --> string cockpitName = "Cockpit 3";

        //############# no touchy below #############
        int screen = 0;
        int menu = 0;
        int choice = 0;
        int parking = 0;
        int setup;
        int counter;
        float batMax;
        float reactorMax;
        float gasMax;
        float oxyMax;
        float cargoMax;
        
        IMyCockpit cockpit;
        IMyOreDetector detector;
        List<int> _menu0 = new List<int>();
        List<int> _menu1 = new List<int>();
        List<IMyLightingBlock> _light = new List<IMyLightingBlock>();
        List<IMyCameraBlock> _cam = new List<IMyCameraBlock>();
        List<IMySensorBlock> _sensor = new List<IMySensorBlock>();
        List<IMyShipConnector> _connector = new List<IMyShipConnector>();
        List<IMyGyro> _gyro = new List<IMyGyro>();
        List<IMyRadioAntenna> _radio = new List<IMyRadioAntenna>();
        List<IMyLandingGear> _gear = new List<IMyLandingGear>();
        List<IMyThrust> _thruster = new List<IMyThrust>();
        List<IMyGasTank> _gas = new List<IMyGasTank>();
        List<IMyGasTank> _oxy = new List<IMyGasTank>();
        List<IMyGasGenerator> _generator = new List<IMyGasGenerator>();
        List<IMyBatteryBlock> _battery = new List<IMyBatteryBlock>();
        List<IMyReactor> _reactor = new List<IMyReactor>();
        List<IMyRemoteControl> _remote = new List<IMyRemoteControl>();
        List<IMyCockpit> _ship = new List<IMyCockpit>();
        List<IMyCargoContainer> _cargo  = new List<IMyCargoContainer>();
        List<IMyInventory> _inventory = new List<IMyInventory>();
        List<MyInventoryItem> _buffer = new List<MyInventoryItem>();
        Dictionary<MyItemType, Double> _invClean = new Dictionary<MyItemType, double>();
        List<MyItemType> _items = new List<MyItemType>();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            Fetch();
            Help();
        }
        public void Help()
        {
            Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
            Me.GetSurface(0).WriteText("", false);
            Me.GetSurface(1).ContentType = ContentType.TEXT_AND_IMAGE;
            Me.GetSurface(1).WriteText("", false);
            if(Me.CubeGrid.ToString().Contains("Large"))
            {
                Me.GetSurface(0).FontSize = 1f;
                Me.GetSurface(1).FontSize = 3f;
            }
            else
            {
                Me.GetSurface(0).FontSize = 1.6f;
                Me.GetSurface(1).FontSize = 3.35f;
            }
            if (_ship.Count > 1 && cockpit == null)
            {
                if (Me.CubeGrid.ToString().Contains("Small"))
                    Me.GetSurface(0).FontSize = 1.4f;
                Me.GetSurface(0).WriteText("  ###### WARNING ######" + "\n" + " MULTIPLE COCKPITS FOUND!" + "\n" + "\n", true);
                Me.GetSurface(0).WriteText(" Apply desired Cockpits" + "\n" + " name in top line of Script" + "\n" + " or set one Cockpit as" + "\n" + " Main Cockpit in ControlPanel" + "\n" + "\n", true);
                Me.GetSurface(0).WriteText(" Then hit 'recompile' or" + "\n" + " run Argument: refresh" + "\n" + "\n", true);
            }
            if (_ship.Count >= 1 && cockpit != null)
            {
                Me.GetSurface(0).WriteText(" Drag PB in Toolbar" + "\n" + " of Cockpit, choose run" + "\n" + " and type in: lcd" + "\n" + "\n", true);
                Me.GetSurface(0).WriteText(" If docked or have" + "\n" + " landing gears engaged" + "\n" + " Run 'lcd' to start/stop edit" + "\n" + "\n", true);
                Me.GetSurface(0).WriteText(" Navigate with: WASD" + "\n" + " hit SPACE to enter", true);
                Me.GetSurface(1).WriteText(" Run Argument: refresh" + "\n" + " or hit recompile" + "\n" + " to update blocks", true);
            }
        }
        public void Main(string argument, UpdateType updateSource)
        {
            if (argument.Equals("refresh", StringComparison.OrdinalIgnoreCase))
            {
                Fetch();
                Help();
            }
            if (cockpit != null)
                if (cockpit.IsUnderControl == true)
                {
                    foreach (var connector in _connector)
                        if (connector != null)
                        {
                            if (connector.CustomData.Equals("", StringComparison.OrdinalIgnoreCase) && !connector.CustomName.Contains("#x#") && (connector.Status == MyShipConnectorStatus.Connectable || connector.Status == MyShipConnectorStatus.Connected))
                            {
                                if (cockpit.MoveIndicator.Normalize() == 0 && connector.Status != MyShipConnectorStatus.Connected)
                                {
                                    Dock(1);
                                }
                                else if (Base6Directions.GetIntVector(connector.Orientation.Forward) != cockpit.MoveIndicator && cockpit.MoveIndicator.Normalize() != 0 && setup == 0)
                                    Undock();
                            }

                        }
                    if (cockpit.MoveIndicator.Normalize() != 0 && cockpit.GetShipSpeed() < 2 && setup == 0)
                        foreach (var gear in _gear)
                            if (gear?.IsLocked == true)
                            {
                                Undock();
                                break;
                            }
                    if (cockpit.MoveIndicator.Normalize() == 0 && parking == 0)
                        foreach (var gear in _gear)
                            if (gear?.IsLocked == true)
                            {
                                Dock(0);
                                break;
                            }
                    if (!cockpit.BlockDefinition.SubtypeId.Contains("Fighter"))
                        if (argument.Equals("lcd", StringComparison.OrdinalIgnoreCase) && parking == 1)
                        {
                            if (setup == 0)
                                setup = 1;
                            else
                                setup = 0;
                        }
                    if (setup == 1)
                    {
                        if (counter < 10)
                            counter++;
                        if (counter == 10)
                        {
                            if (cockpit.MoveIndicator.Z > 0 || cockpit.MoveIndicator.Z < 0)
                            {
                                if (choice == 0)
                                    choice++;
                                else
                                    choice--;
                                counter = 0;
                            }
                            if (cockpit.MoveIndicator.X > 0 && choice == 0)
                            {
                                if (screen < cockpit.SurfaceCount - 1 && screen < 3)
                                    screen++;
                                else if (screen == cockpit.SurfaceCount - 1 || screen == 3)
                                    screen = 0;
                                counter = 0;
                            }
                            if (cockpit.MoveIndicator.X < 0 && choice == 0)
                            {
                                if (screen >= 0)
                                    screen--;
                                if (screen < 0)
                                    if (cockpit.SurfaceCount - 1 <= 3)
                                        screen = cockpit.SurfaceCount - 1;
                                    else
                                        screen = 3;
                                counter = 0;
                            }
                            if (cockpit.MoveIndicator.X > 0 && choice == 1)
                            {
                                if (menu < 6)
                                    menu++;
                                else
                                    menu = 0;
                                counter = 0;
                            }
                            if (cockpit.MoveIndicator.X < 0 && choice == 1)
                            {
                                if (menu > 0)
                                    menu--;
                                else
                                    menu = 6;
                                counter = 0;
                            }
                            if (cockpit.MoveIndicator.Y > 0)
                            {
                                if (menu == 0)
                                {
                                    if (_menu1.Contains(screen))
                                        _menu1.Remove(screen);
                                    if (!_menu0.Contains(screen))
                                        _menu0.Add(screen);
                                    ScreenSetup(screen);
                                    counter = 0;
                                }
                                if (menu == 1)
                                {
                                    if (_menu0.Contains(screen))
                                        _menu0.Remove(screen);
                                    if (!_menu1.Contains(screen))
                                        _menu1.Add(screen);
                                    ScreenSetup(screen);
                                    counter = 0;
                                }
                                if (menu == 2 && screen != 0)
                                {
                                    cockpit.GetSurface(screen).ContentType = ContentType.SCRIPT;
                                    cockpit.GetSurface(screen).Script = "TSS_ClockAnalog";
                                }
                                if (menu == 3 && screen != 0)
                                {
                                    cockpit.GetSurface(screen).ContentType = ContentType.SCRIPT;
                                    cockpit.GetSurface(screen).Script = "TSS_ClockDigital";
                                }
                                if (menu == 4 && screen != 0)
                                {
                                    cockpit.GetSurface(screen).ContentType = ContentType.SCRIPT;
                                    cockpit.GetSurface(screen).Script = "TSS_TargetingInfo";
                                }
                                if (menu == 5 && screen != 0)
                                {
                                    cockpit.GetSurface(screen).ContentType = ContentType.SCRIPT;
                                    cockpit.GetSurface(screen).Script = "TSS_Velocity";
                                }
                                if (menu == 6 && screen != 0)
                                {
                                    cockpit.GetSurface(screen).ContentType = ContentType.SCRIPT;
                                    cockpit.GetSurface(screen).Script = "TSS_EnergyHydrogen";
                                }
                            }
                        }
                    }
                    if (setup == 1)
                        Print(screen, menu);
                    else
                        cockpit.GetSurface(0).WriteText("", false);
                    foreach (var show in _menu0)
                        Status(show);
                    foreach (var show in _menu1)
                        Cargo(show);
                }
                else
                {
                    setup = 0;
                    cockpit.ControlThrusters = true;
                    foreach (var gyro in _gyro)
                        if (gyro != null)
                            gyro.Enabled = true;
                }

            foreach (var remote in _remote)
            {
                if (remote != null)
                    if (remote.IsUnderControl == true)
                    {
                        foreach (var connector in _connector)
                            if (connector != null)
                            {
                                if (connector.CustomData.Equals("", StringComparison.OrdinalIgnoreCase) && (connector.Status == MyShipConnectorStatus.Connectable || connector.Status == MyShipConnectorStatus.Connected))
                                {
                                    if (remote.MoveIndicator.Normalize() == 0 && connector.Status != MyShipConnectorStatus.Connected)
                                    {
                                        Dock(1);
                                    }
                                    else if (Base6Directions.GetIntVector(connector.Orientation.Forward) != remote.MoveIndicator && remote.MoveIndicator.Normalize() != 0)
                                        Undock();
                                }
                            }
                        if (remote.MoveIndicator.Normalize() != 0 && remote.GetShipSpeed() < 2)
                            foreach (var gear in _gear)
                                if (gear?.IsLocked == true)
                                {
                                    Undock();
                                    break;
                                }
                        if (remote.MoveIndicator.Normalize() == 0 && parking == 0)
                            foreach (var gear in _gear)
                                if (gear?.IsLocked == true)
                                {
                                    Dock(0);
                                    break;
                                }
                    }
            }
        }
        public void Print(int s, int m)
        {
            string menu = "";
            switch (m)
            {
                case 0:
                    menu = "Status";
                    break;
                case 1:
                    menu = "Cargo";
                    break;
                case 2:
                    menu = "aClock";
                    break;
                case 3:
                    menu = "dClock";
                    break;
                case 4:
                    menu = "Target";
                    break;
                case 5:
                    menu = "Speed";
                    break;
                case 6:
                    menu = "Energy";
                    break;
            }
            var surface = cockpit.GetSurface(0);

            surface.WriteText("", false);
                if (choice == 0)
                {
                    surface.WriteText("> Screen : " + s.ToString() + "\n", true);
                    surface.WriteText("  Display: " + menu + "\n" + "\n", true);
                }
                if (choice == 1)
                {
                    surface.WriteText("  Screen : " + s.ToString() + "\n", true);
                    surface.WriteText("> Display: " + menu + "\n" + "\n", true);
                }
        }
        public void Status(int surface)
        {
            if (surface != 0)
                cockpit.GetSurface(surface).WriteText("", false);
            double per;
            string percent;
            string label;

            if (_battery.Count > 0)
            {
                label = "Battery:";
                double batCur = 0;
                foreach (var battery in _battery)
                    batCur += battery.CurrentStoredPower;
                per = batCur * 100 / batMax;
                percent = per.ToString("0") + "%";
                PrintStatus(surface, label, per);
            }
            if (_reactor.Count > 0)
            {
                label = "Reactor:";
                double reactorCur = 0;
                foreach (var reactor in _reactor)
                    reactorCur += reactor.CurrentOutput;
                per = reactorCur * 100 / reactorMax;
                percent = per.ToString("0") + "%";
                PrintStatus(surface, label, per);
            }
            if (_gas.Count > 0)
            {
                label = "Hydrogen:";
                double gasCur = 0;
                foreach (var gas in _gas)
                    gasCur += gas.Capacity * gas.FilledRatio;
                per = gasCur * 100 / gasMax;
                percent = per.ToString("0") + "%";
                PrintStatus(surface, label, per);
            }
            if (_oxy.Count > 0)
            {
                label = "Oxygen:";
                double oxyCur = 0;
                foreach (var oxy in _oxy)
                    oxyCur += oxy.Capacity * oxy.FilledRatio;
                per = oxyCur * 100 / oxyMax;
                percent = per.ToString("0") + "%";
                PrintStatus(surface, label, per);
            }
        }
        public void Cargo(int s)
        {
            var surface = cockpit.GetSurface(s);
            if (s != 0)
                surface.WriteText("", false);
            double cargoCur = 0;
            foreach (var cargo in _cargo)
                cargoCur += (double)cargo.GetInventory().CurrentVolume;
            double cPer = cargoCur * 100 / cargoMax;

            _invClean.Clear();
            foreach (var inventory in _inventory)
            {
                _buffer.Clear();
                inventory.GetItems(_buffer);
                foreach (var item in _buffer)
                {
                    double n;
                    _invClean.TryGetValue(item.Type, out n);
                    _invClean[item.Type] = n + (double)item.Amount;
                }
            }
            string bar = "----------";
            for (int i = 0; i < Math.Round(cPer / 10); i++)
            {
                StringBuilder sb = new StringBuilder(bar);
                sb[i] = '/';
                bar = sb.ToString();
            }
            int width;
            if (surface.SurfaceSize.X > 512)
                width = (int)Math.Floor(512 / (10 * surface.FontSize));
            else if (cockpit.BlockDefinition.SubtypeId.Contains("Industrial") && s == 3)
                width = (int)Math.Floor(512 / (10 * surface.FontSize));
            else
                width = (int)Math.Floor(surface.SurfaceSize.X / (10 * surface.FontSize));
            string percent = cPer.ToString("0") + "%";
            int tempSpace = width - (percent.Length + 12);

            surface.WriteText("Cargo:" + "\n", true);
            surface.WriteText("[" + bar + "]" + string.Empty.PadRight(tempSpace, (char)32) + percent + "\n" + "\n", true);
            foreach (var iAmount in _invClean)
            {
                string amount = (iAmount.Value / 1000).ToString("0") + "k";
                string type = iAmount.Key.SubtypeId.Substring(0);
                if (type.Length > 10)
                {
                    type = type.Substring(0, 10);
                }
                tempSpace = width - (type.Length + amount.Length);
                surface.WriteText(type + string.Empty.PadRight(tempSpace, (char)32) + amount + "\n", true);
            }
        }
        public void PrintStatus(int s, string label, double per)
        {
            var surface = cockpit.GetSurface(s);
            int width;
            if (surface.SurfaceSize.X > 512)
                width = (int)Math.Floor(512 / (10 * surface.FontSize));
            else if (cockpit.BlockDefinition.SubtypeId.Contains("Industrial") && s == 3)
                width = (int)Math.Floor(512 / (10 * surface.FontSize));
            else
                width = (int)Math.Floor(surface.SurfaceSize.X / (10 * surface.FontSize));
            string percent = per.ToString("0") + "%";
            int tempSpace;

            string bar = "----------";
            for (int i = 0; i < Math.Round(per / 10); i++)
            {                
                StringBuilder sb = new StringBuilder(bar);
                sb[i] = '/';
                bar = sb.ToString();
            }
            tempSpace = width - (percent.Length + 12);
                surface.WriteText(label + "\n", true);
                surface.WriteText("[" + bar + "]" + string.Empty.PadRight(tempSpace, (char)32) + percent + "\n", true);
        }
        public void Dock(int i)
        {
            foreach (var connector in _connector)
                connector?.Connect();
            foreach (var gear in _gear)
                gear?.Lock();
            foreach (var cam in _cam)
                if (cam != null)
                    if (cam.CustomData.Equals(""))
                        cam.Enabled = false;
            foreach (var sens in _sensor)
                if (sens != null)
                    if (sens.CustomData.Equals(""))
                        sens.Enabled = false;
            if (detector != null)
                detector.Enabled = false;
            foreach (var thrust in _thruster)
                if (thrust != null)
                    thrust.Enabled = false;
            foreach (var gyro in _gyro)
                if (gyro != null)
                    gyro.Enabled = false;
            foreach (var antenna in _radio)
                if (antenna != null)
                    antenna.Enabled = false;
            foreach (var light in _light)
                if (light != null)
                    light.Enabled = false;
            if (i == 1)
            {
                foreach (var gas in _gas)
                    if (gas != null)
                        gas.Stockpile = true;
                foreach (var oxy in _oxy)
                    if (oxy != null)
                        oxy.Stockpile = true;
                foreach (var battery in _battery)
                    if (battery != null)
                        battery.ChargeMode = ChargeMode.Recharge;
                foreach (var reactor in _reactor)
                    if (reactor != null)
                        reactor.Enabled = false;
            }
            parking = 1;
        }
        public void Undock()
        {
            foreach (var gas in _gas)
                if (gas != null)
                    gas.Stockpile = false;
            foreach (var oxy in _oxy)
                if (oxy != null)
                    oxy.Stockpile = false;
            foreach (var battery in _battery)
                if (battery != null)
                    battery.ChargeMode = ChargeMode.Auto;
            foreach (var thrust in _thruster)
                if (thrust != null)
                    thrust.Enabled = true;
            foreach (var gyro in _gyro)
                if (gyro != null)
                    gyro.Enabled = true;
            foreach (var reactor in _reactor)
                if (reactor != null)
                    reactor.Enabled = true;
            foreach (var antenna in _radio)
                if (antenna != null)
                    antenna.Enabled = true;
            foreach (var light in _light)
                if (light != null)
                    light.Enabled = true;
            foreach (var cam in _cam)
                if (cam != null)
                    cam.Enabled = true;
            foreach (var sens in _sensor)
                if (sens != null)
                    sens.Enabled = true;
            if (detector != null)
                detector.Enabled = true;
            foreach (var connector in _connector)
                connector?.Disconnect();
            foreach (var gear in _gear)
                gear?.Unlock();
            parking = 0;
        }
        public void ScreenSetup(int i)
        {
            cockpit.GetSurface(i).ContentType = ContentType.TEXT_AND_IMAGE;
            cockpit.GetSurface(i).Font = "Monospace";
            cockpit.GetSurface(i).FontSize = 1.5f;
            cockpit.GetSurface(i).FontColor = Color.DarkGreen;
            cockpit.GetSurface(i).Alignment = TextAlignment.LEFT;
            cockpit.GetSurface(i).WriteText("", false);
        }
        public void Fetch()
        {
            _light.Clear();
            _cam.Clear();
            _sensor.Clear();
            _connector.Clear();
            _gyro.Clear();
            _radio.Clear();
            _gear.Clear();
            _thruster.Clear();
            _gas.Clear();
            _oxy.Clear();
            _generator.Clear();
            _battery.Clear();
            _reactor.Clear();
            _remote.Clear();
            _ship.Clear();
            _cargo.Clear();
            _inventory.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(null, FilteringMethod);
            if (_ship.Count == 1 && cockpit == null)
                foreach (var ship in _ship)
                    cockpit = GridTerminalSystem.GetBlockWithName(ship.CustomName) as IMyCockpit;
            if (cockpit != null)
            {
                if (!cockpit.BlockDefinition.SubtypeId.Contains("Fighter"))
                {
                    cockpit.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
                    cockpit.GetSurface(0).Font = "Monospace";
                    cockpit.GetSurface(0).FontSize = 1.5f;
                    cockpit.GetSurface(0).FontColor = Color.DarkGreen;
                    cockpit.GetSurface(0).Alignment = TextAlignment.LEFT;
                }
                for (int i = 0; i < cockpit.SurfaceCount; i++)
                {
                    cockpit.GetSurface(i).WriteText("", false);
                }
            }
            foreach (var battery in _battery)
                batMax += battery.MaxStoredPower;
            foreach (var reactor in _reactor)
                reactorMax += reactor.MaxOutput;
            foreach (var gas in _gas)
                gasMax += gas.Capacity;
            foreach (var oxy in _oxy)
                oxyMax += oxy.Capacity;
            foreach (var cargo in _cargo)
                cargoMax += (float)cargo.GetInventory().MaxVolume;
        }
        bool FilteringMethod(IMyTerminalBlock block)
        {
            if (block.IsSameConstructAs(Me))
            {
                var drill = block as IMyShipDrill;
                if (drill != null)
                    drill.ShowInTerminal = false;

                var welder = block as IMyShipWelder;
                if (welder != null)
                    welder.ShowInTerminal = false;

                var grinder = block as IMyShipGrinder;
                if (grinder != null)
                    grinder.ShowInTerminal = false;

                var camera = block as IMyCameraBlock;
                if (camera != null)
                {
                    camera.ShowInTerminal = false;
                    _cam.Add(camera);
                }
                var sensor = block as IMySensorBlock;
                if (sensor != null)
                {
                    sensor.ShowInTerminal = false;
                    _sensor.Add(sensor);
                }
                var oreDetector = block as IMyOreDetector;
                if (oreDetector != null && detector == null)
                    detector = GridTerminalSystem.GetBlockWithName(oreDetector.CustomName) as IMyOreDetector;

                var light = block as IMyLightingBlock;
                if (light != null)
                {
                    light.ShowInTerminal = false;
                    _light.Add(light);
                }
                var connector = block as IMyShipConnector;
                if (connector != null)
                {
                    connector.ShowInTerminal = false;
                    connector.IsParkingEnabled = false;
                    _connector.Add(connector);
                }
                var gyro = block as IMyGyro;
                if (gyro != null)
                {
                    gyro.ShowInTerminal = false;
                    _gyro.Add(gyro);
                }
                var antenna = block as IMyRadioAntenna;
                if (antenna != null)
                    _radio.Add(antenna);

                var gear = block as IMyLandingGear;
                if (gear != null)
                {
                    gear.ShowInTerminal = false;
                    _gear.Add(gear);
                }
                var thruster = block as IMyThrust;
                if (thruster != null)
                {
                    thruster.ShowInTerminal = false;
                    _thruster.Add(thruster);
                }
                var gastank = block as IMyGasTank;
                if (gastank != null)
                {
                    gastank.ShowInInventory = false;
                    gastank.ShowInTerminal = false;
                    if (gastank.BlockDefinition.SubtypeId.Contains("Hydrogen"))
                        _gas.Add(gastank);
                    if (gastank.BlockDefinition.SubtypeId.Contains("Oxy"))
                        _oxy.Add(gastank);
                }
                var generator = block as IMyGasGenerator;
                if (generator != null)
                {
                    generator.ShowInTerminal = false;
                    _generator.Add(generator);
                }
                var battery = block as IMyBatteryBlock;
                if (battery != null)
                {
                    battery.ShowInTerminal = false;
                    _battery.Add(battery);
                }
                var reactor = block as IMyReactor;
                if (reactor != null)
                {
                    reactor.ShowInTerminal = false;
                    _reactor.Add(reactor);
                }
                var ship = block as IMyCockpit;
                if (ship != null)
                {
                    if (cockpit == null)
                    {
                        if (ship.IsMainCockpit == true && cockpitName == "")
                            cockpit = GridTerminalSystem.GetBlockWithName(ship.CustomName) as IMyCockpit;
                        if (cockpitName != "")
                            cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit;
                    }
                    _ship.Add(ship);
                }
                var remote = block as IMyRemoteControl;
                if (remote != null)
                    _remote.Add(remote);

                var cargo = block as IMyCargoContainer;
                if (cargo != null)
                {
                    cargo.ShowInTerminal = false;
                    _inventory.Add(cargo.GetInventory(0));
                    _cargo.Add(cargo);
                }
            }
            return false;
        }