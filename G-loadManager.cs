        const string VERSION = "OVERLOAD SHIP GRAVITY MANAGER v0.0.6";

        const string IGNORE_LABEL = "(ignore)"; // if you have cockpits (or thrusters) you want to be ignored, add this label to it's names

        const int MAX_TRIES_CALCULATE_SHIP_MASS = 10;

        const double G_MOONS = 0.25; // Moon, Europa, Titan, Triton 
        const double G_MARS = 0.9; // Mars 
        const double G_EARTHLIKE = 1.0; // EarthLike 
        const double G_ALIEN = 1.1; // Alien Planet 
        const double G_PERTAM = 1.2; // Pertam

        Dictionary<string, double> PLANET_G = new Dictionary<string, double>()
        {
            { "0.25g", G_MOONS },
            { " 0.9g", G_MARS },
            { " 1.0g", G_EARTHLIKE },
            { " 1.1g", G_ALIEN },
            { " 1.2g", G_PERTAM }
        };

        const string SCARY_MESSAGE = "DANGER: FALL!";
        const string SCARY_MESSAGE_SHORT = "FALL!";
        const int MESSAGE_LINE_WILL_BE_PADDED_TO = 13;

        const string NO_GRAVITY_MESSAGE = "No natural gravity";
        const string STOCKPILE_ON_DANGER_MESSAGE = "Stockpile ON!";
        const string SHIP_NO_POWERED_MESSAGE = "NO POWER!";

        const float MINIMUM_VALUE_OF_POWER_FOR_THRUSTERS = 0.001f; // minimum energy ship should produce to make hydrogen thrusters work

        const int SURFACE_INFO_ID = 0;
        float defaultFontSize = 1.8f;
        string defaultFont = "Monospace";
        Color defaultColor = Color.White;
        Color defaultBackgroundColor = Color.Black;

        const int SURFACE_STAT_ID = 3;
        float defaultStatFontSize = 1.7f;
        string defaultStatFont = "Monospace";
        Color defaultStatColor = Color.White;
        Color defaultStatBackgroundColor = Color.Black;

        IMyShipController controller;
        private readonly IMyCubeGrid currentGrid;
        List<IMyThrust> thrusters = new List<IMyThrust>();
        List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();
        private readonly List<IMyCockpit> cockpits;

        List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
        List<IMyReactor> reactors = new List<IMyReactor>();
        List<IMySolarPanel> solarPanels = new List<IMySolarPanel>();
        List<IMyPowerProducer> hydrogenEngines = new List<IMyPowerProducer>();
        List<IMyTerminalBlock> gridBlocks = new List<IMyTerminalBlock>();

        double inventoryMultiplier;
        int massNotFoundCount;
        double massAllShipBlockWithoutInventories;
        List<IMyShipConnector> connectorsToRestore;
        List<IMyLandingGear> landingGearsToRestore;
        Dictionary<IMyLandingGear, bool> landingGearAutoLockToRestore;

        public Program()
        {
            currentGrid = Me.CubeGrid;
            controller = null;
            inventoryMultiplier = World.InventoryMultiplier;

            var controllers = new List<IMyShipController>();

            GridTerminalSystem.GetBlocksOfType(controllers, c => c.CubeGrid == Me.CubeGrid && (c.IsMainCockpit || c.CanControlShip));

            if (controllers.Count > 0)
            {
                controller = controllers.FirstOrDefault(c => c.IsMainCockpit) ?? controllers[0];
            }

            cockpits = new List<IMyCockpit>();
            GridTerminalSystem.GetBlocksOfType(cockpits, cockpit => !cockpit.CustomName.Contains(IGNORE_LABEL)
                && cockpit.SurfaceCount > 0
                && cockpit.CubeGrid == currentGrid
            );

            if (cockpits.Count > 0)
            {
                SetupLCDs(cockpits);
            }

            massNotFoundCount = 0; // will prevent to infinite cycle
            connectorsToRestore = new List<IMyShipConnector>();
            landingGearsToRestore = new List<IMyLandingGear>();
            landingGearAutoLockToRestore = new Dictionary<IMyLandingGear, bool>();
            massAllShipBlockWithoutInventories = -1;

            Runtime.UpdateFrequency = UpdateFrequency.Update1; // will be changed to UpdateFrequency.Update10 after mass check;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (massNotFoundCount > MAX_TRIES_CALCULATE_SHIP_MASS)
            {
                Echo("Script interrupted because ship mass not calculated after " + MAX_TRIES_CALCULATE_SHIP_MASS + " iterations");
                Runtime.UpdateFrequency = UpdateFrequency.None;

                return;
            }

            if (controller == null)
            {
                Echo("No controller found. Add it and recompile script");

                return;
            }

            if (cockpits.Count == 0)
            {
                Echo("No cockpit found. Add it and recompile script.");

                return;
            }

            massAllShipBlockWithoutInventories = MyCalculateShipMassKg();

            if (massAllShipBlockWithoutInventories <= 0)
            {
                massNotFoundCount++;

                return;
            }

            string statusText = "";
            string statisticText = "";
            double mass = (double)massAllShipBlockWithoutInventories + GetAllGridInventoriesMassKg(inventoryMultiplier);

            Vector3D gravity = controller.GetNaturalGravity();
            double g = gravity.Length();

            statisticText = "\n" + $"Mass: {FormatMass(mass)}\n\n" + $"g: {g:0.00} m/s²";
            WriteToSurfaces(cockpits, SURFACE_STAT_ID, statisticText, defaultStatColor);

            // Check for this values each iteration (do not move to constructor)
            GridTerminalSystem.GetBlocksOfType(thrusters, t => t.CubeGrid == Me.CubeGrid && !t.CustomName.Contains(IGNORE_LABEL));

            if (g < 0.05)
            {
                // only up directional thrusters counting
                StringBuilder sb = new StringBuilder();
                Vector3D antiGravDirArtificial = controller.WorldMatrix.Up;

                double upThrustArtificial = CalculateUpThrust(thrusters, antiGravDirArtificial);
                double aMaxArtificial = upThrustArtificial / mass;

                bool isScary = false;
                foreach (var kv in PLANET_G)
                {
                    double g_val = kv.Value * 9.81;

                    if (aMaxArtificial > 0)
                    {
                        double loadPercentArtificial = (g_val / aMaxArtificial) * 100.0;
                        sb.AppendLine(PadStringTo($"{kv.Key}: {loadPercentArtificial:0.0}%"));
                    }
                    else
                    {
                        // no engines enabled or no engines for UP thrust
                        isScary = true;
                        sb.AppendLine(PadStringTo($"{kv.Key}: " + SCARY_MESSAGE_SHORT));
                    }
                }

                statusText = sb.ToString();
                statisticText = "\n" + $"Mass: {FormatMass(mass)}\n\n" + $"g: {g:0.00} m/s²";

                Echo(statusText + "\n" + statisticText);
                WriteToSurfaces(cockpits, SURFACE_INFO_ID, statusText, isScary ? Color.Red : Color.LightBlue);
                WriteToSurfaces(cockpits, SURFACE_STAT_ID, statisticText, defaultStatColor);

                return;
            }

            GridTerminalSystem.GetBlocksOfType(hydrogenTanks, t => t.CubeGrid == Me.CubeGrid
                && t.BlockDefinition.SubtypeName.Contains("Hydrogen")
                && !t.CustomName.Contains(IGNORE_LABEL)
            );

            if (hydrogenTanks.Count > 0 && hydrogenTanks.TrueForAll(t => t.Stockpile))
            {
                statusText = "\n" + STOCKPILE_ON_DANGER_MESSAGE + "\n\n" + SCARY_MESSAGE;

                WriteToSurfaces(cockpits, SURFACE_INFO_ID, statusText, Color.Red);

                return;
            }

            /** Checking all power sources **/

            GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(
                hydrogenEngines,
                e =>
                    e.CubeGrid == Me.CubeGrid &&
                    !e.CustomName.Contains(IGNORE_LABEL) &&
                    (e.BlockDefinition.SubtypeName.Contains("HydrogenEngine") ||
                     e.BlockDefinition.SubtypeName.Contains("HydrogenEngineSmall") ||
                     e.BlockDefinition.SubtypeName.Contains("LargeHydrogenEngine"))
            );

            GridTerminalSystem.GetBlocksOfType(reactors, t => t.CubeGrid == Me.CubeGrid && !t.CustomName.Contains(IGNORE_LABEL));
            GridTerminalSystem.GetBlocksOfType(solarPanels, t => t.CubeGrid == Me.CubeGrid && !t.CustomName.Contains(IGNORE_LABEL));
            GridTerminalSystem.GetBlocksOfType(batteries, t => t.CubeGrid == Me.CubeGrid && !t.CustomName.Contains(IGNORE_LABEL));

            if (!HasLocalGeneration())
            {
                statusText = "\n" + SHIP_NO_POWERED_MESSAGE + "\n\n" + SCARY_MESSAGE;

                WriteToSurfaces(cockpits, SURFACE_INFO_ID, statusText, Color.Red);

                return;
            }

            /** End checking all power sources **/

            Vector3D antiGravDir = -gravity / g;
            double upThrust = CalculateUpThrust(thrusters, antiGravDir);

            if (upThrust <= 0)
            {
                statusText = "\n\n\n" + SCARY_MESSAGE;

                WriteToSurfaces(cockpits, SURFACE_INFO_ID, statusText, Color.Red);

                return;
            }

            double aMax = upThrust / mass;    // max acceleration up
            double load = g / aMax;           // load proportion
            double loadPercent = load * 100;         // load percentage

            var statusColor = CalculateColor(loadPercent);
            var statusMessage = CalculateStatus(loadPercent);

            statusText = $"G-load: {loadPercent:0.#}%\n\n" + statusMessage;

            Echo(statusText + "\n\n" + statisticText);
            WriteToSurfaces(cockpits, SURFACE_INFO_ID, statusText, statusColor);

            Echo($"Ship mass: {massAllShipBlockWithoutInventories:0.##}");
        }

        private void SetupLCDs(List<IMyCockpit> cockpits)
        {
            foreach (IMyCockpit cockpit in cockpits)
            {
                var surface = cockpit.GetSurface(SURFACE_INFO_ID);

                surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                surface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;

                surface.Font = defaultFont;
                surface.FontSize = defaultFontSize;
                surface.FontColor = defaultColor;
                surface.BackgroundColor = defaultBackgroundColor;

                surface.WriteText("");

                var surfaceStat = cockpit.GetSurface(SURFACE_STAT_ID);

                surfaceStat.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                surfaceStat.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;

                surfaceStat.Font = defaultStatFont;
                surfaceStat.FontSize = defaultStatFontSize;
                surfaceStat.FontColor = defaultStatColor;
                surfaceStat.BackgroundColor = defaultStatBackgroundColor;

                surfaceStat.WriteText("");
            }
        }

        private void WriteToSurfaces(List<IMyCockpit> cockpits, int surfaceId, string statusText, Color statusColor)
        {
            foreach (IMyCockpit cockpit in cockpits)
            {
                var surface = cockpit.GetSurface(surfaceId);

                surface.FontColor = statusColor;
                surface.WriteText(statusText);
            }
        }

        private Color CalculateColor(double digit)
        {
            if (digit < 50) return new Color(0, 255, 0);
            if (digit < 70) return new Color(255, 255, 0);
            if (digit < 100) return new Color(255, 0, 0);

            return new Color(255, 0, 0);
        }

        private string CalculateStatus(double digit)
        {
            if (digit < 70) return "OK";
            if (digit < 100) return "WARNING";

            return SCARY_MESSAGE;
        }

        bool HasLocalGeneration()
        {
            double power = 0;

            foreach (var r in reactors)
            {
                if (r.IsFunctional && r.Enabled)
                {
                    power += r.CurrentOutput;
                }
            }

            foreach (var e in hydrogenEngines)
            {
                if (e.IsFunctional && e.Enabled)
                {
                    power += e.CurrentOutput;
                }
            }

            foreach (var b in batteries)
            {
                if (b.IsFunctional && b.Enabled && b.ChargeMode != ChargeMode.Recharge)
                {
                    power += b.CurrentOutput;
                }
            }

            foreach (var s in solarPanels)
            {
                if (s.IsFunctional)
                {
                    power += s.CurrentOutput;
                }
            }

            return power >= MINIMUM_VALUE_OF_POWER_FOR_THRUSTERS;
        }

        string FormatMass(double massKg)
        {
            if (massKg < 1000)
            {
                return $"{massKg:0} kg";
            }

            if (massKg < 1000000)
            {
                return $"{massKg / 1000:0.0} t";
            }

            if (massKg < 1000000000)
            {
                return $"{massKg / 1000000:0.0} Mt";
            }

            return $"{massKg / 1000000000:0.0} Gt";
        }

        void WriteToSurface(IMyCockpit cockpit, string text, Color textColor)
        {
            var surf = cockpit.GetSurface(SURFACE_INFO_ID);
            surf.FontColor = textColor;
            surf.WriteText(text);
        }

        double GetAllGridInventoriesMassKg(double inventoryMultiplier)
        {
            gridBlocks.Clear();

            GridTerminalSystem.GetBlocksOfType(
                gridBlocks,
                b => b.CubeGrid == Me.CubeGrid
            );

            double cargoMass = 0;

            foreach (var tb in gridBlocks)
            {
                for (int i = 0; i < tb.InventoryCount; i++)
                {
                    var inv = tb.GetInventory(i);

                    if (inv != null)
                    {
                        cargoMass += (double)inv.CurrentMass;
                    }
                }
            }

            return cargoMass / inventoryMultiplier;
        }

        double MyCalculateShipMassKg()
        {
            if (massAllShipBlockWithoutInventories > 0)
            {
                return massAllShipBlockWithoutInventories;
            }

            bool anyDocked = false;
            var connectorsCheck = new List<IMyShipConnector>();
            GridTerminalSystem.GetBlocksOfType(connectorsCheck, c => c.CubeGrid == Me.CubeGrid);

            var connectorsList = new List<IMyShipConnector>();
            GridTerminalSystem.GetBlocksOfType(connectorsList);

            foreach (var connectorCheck in connectorsList)
            {
                if (connectorCheck.Status == MyShipConnectorStatus.Connected)
                {
                    anyDocked = true;
                    connectorsToRestore.Add(connectorCheck);
                }
            }

            bool anyLandingGearLocked = false;

            landingGearsToRestore.Clear();
            landingGearAutoLockToRestore.Clear();

            var landingGears = new List<IMyLandingGear>();
            GridTerminalSystem.GetBlocksOfType(landingGears, g => g.CubeGrid == Me.CubeGrid);

            foreach (var gear in landingGears)
            {
                if (gear.IsLocked)
                {
                    anyLandingGearLocked = true;
                    landingGearsToRestore.Add(gear);
                    landingGearAutoLockToRestore[gear] = gear.AutoLock;
                }
            }

            if (anyDocked || anyLandingGearLocked)
            {
                foreach (var connectorCheck in connectorsToRestore)
                {
                    connectorCheck.Disconnect();
                }

                foreach (var gear in landingGearsToRestore)
                {
                    gear.AutoLock = false;
                    gear.Unlock();
                }

                return -1;
            }

            massAllShipBlockWithoutInventories = controller.CalculateShipMass().BaseMass;

            foreach (var connectorToRestore in connectorsToRestore)
            {
                connectorToRestore.Connect();
            }

            foreach (var gear in landingGearsToRestore)
            {
                gear.Lock();

                if (landingGearAutoLockToRestore.ContainsKey(gear))
                {
                    gear.AutoLock = landingGearAutoLockToRestore[gear];
                }
            }

            connectorsToRestore.Clear();
            landingGearsToRestore.Clear();
            landingGearAutoLockToRestore.Clear();

            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            return massAllShipBlockWithoutInventories;
        }

        double CalculateUpThrust(List<IMyThrust> thrusters, Vector3D antiGravDir)
        {
            double total = 0;

            foreach (var t in thrusters)
            {
                if (!t.IsFunctional || !t.Enabled)
                {
                    continue;
                }

                Vector3D thrustDir = t.WorldMatrix.Backward;
                double dot = Vector3D.Dot(thrustDir, antiGravDir);

                if (dot <= 0)
                {
                    continue;
                }

                double effective = t.MaxEffectiveThrust * dot;
                total += effective;
            }

            return total;
        }

        string PadStringTo(string s)
        {
            if (s.Length >= MESSAGE_LINE_WILL_BE_PADDED_TO)
            {
                return s;
            }

            return s.PadRight(MESSAGE_LINE_WILL_BE_PADDED_TO, ' ');
        }