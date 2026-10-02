#region script

        // standard large grid square LCDs
        private const float HANGAR_FONT_SIZE = 8.0f;
        private const string FONT = "Monospace";
        private const float MIN_PRESSURE_LEVEL = 0.75f;

        // global timer
        private TimeSpan Uptime;

        public Program()
        {
            // Update every 10 and 100 ticks
            Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;

            Uptime = TimeSpan.Zero;

            LoadProgrammSettings();

            // Initialize all sub systems
            InitAll();
        }

        #region programm_settings
        private enum ProgramMode { Main, Miner, Fighter };

        private ProgramMode SelectedProgramMode = ProgramMode.Main;
        private double ShipMaxSpeed = 100;
        private int AirlockAutoClosingDelay = 20; // ticks

        private void LoadProgrammSettings()
        {
            var properties = GetKeyValuePairsFromString(Me.CustomData);

            if (properties.ContainsKey("mode"))
                Enum.TryParse<ProgramMode>(properties["mode"], out SelectedProgramMode);

            Echo("Programm runs in " + SelectedProgramMode.ToString() + " mode");

            if (properties.ContainsKey("speed"))
                double.TryParse(properties["speed"], out ShipMaxSpeed);

            if (properties.ContainsKey("airlock_delay"))
                int.TryParse(properties["airlock_delay"], out AirlockAutoClosingDelay);

        }
        #endregion

        #region entrypoints
        public void Main(string argument, UpdateType updateType)
        {
            this.Uptime += Runtime.TimeSinceLastRun;

            // Update every 10 ticks
            if ((updateType & UpdateType.Update10) != 0)
            {
                Run10();
            }

            // Update every 100 ticks
            if ((updateType & UpdateType.Update100) != 0)
            {
                Run100();
            }

            // Called by button, timer, sensor or other trigger
            if ((updateType & UpdateType.Trigger) != 0)
            {
                ExecuteCommand(argument);
            }

            // Called by an antenna
            if ((updateType & UpdateType.IGC) != 0)
            {
                Echo("IGC not supported yet");
            }

            // Called from another programmable block
            if ((updateType & UpdateType.Script) != 0)
            {
                Echo("Script not supported yet");
            }

            // Called manually through the Terminal
            if ((updateType & UpdateType.Terminal) != 0)
            {
                ExecuteCommand(argument);
            }
        }

        public void Save()
        {
            SaveSettings();
        }

        private void ExecuteCommand(string argument)
        {
            if (argument == "init")
            {
                InitAll();
            }
            else if (argument == "init displays")
            {
                InitDisplays();
            }
            else if (argument.StartsWith("hangar"))
            {
                CommandHangar(argument.Substring(6).TrimStart());
            }
            else if (argument.StartsWith("lights"))
            {
                CommandLights(argument.Substring(6).TrimStart());
            }
            else if (argument.StartsWith("autopilot"))
            {
                CommandAutopilot(argument.Substring(9).TrimStart());
            }
            else
            {
                Echo("Command not supported");
            }
        }

        private void InitAll()
        {
            LoadSettings();

            InitShipReference();
            InitDisplays();
            InitBatteries();
            InitEnvironment();

            if (SelectedProgramMode != ProgramMode.Fighter)
            {
                InitInventories();
                InitReactor();
                InitEnergy();
                InitThrust();
                InitConnectors();
                InitAutopilot();
            }

            if (SelectedProgramMode == ProgramMode.Main)
            {
                InitHydrogen();
                InitOxygen();
                InitSolar();
                InitRooms();
                InitLights();
                InitAirlocks();
                InitHangars();
            }
        }

        /**
         * Runs every 10 ticks
         */
        private void Run10()
        {
            UpdateEnvironment10();

            if (SelectedProgramMode != ProgramMode.Fighter)
            {
                UpdateAutopilot();
            }

            if (SelectedProgramMode == ProgramMode.Main)
            {
                UpdateAirlocks();
            }

            UpdateDisplays10();
        }

        /**
         * Runs every 100 ticks
         */
        private void Run100()
        {
            UpdateBatteries();
            UpdateInventories();
            UpdateEnvironment100();

            if (SelectedProgramMode != ProgramMode.Fighter)
            {
                UpdateReactor();
                UpdateThrust();
                UpdateThrustWeight();
            }

            if (SelectedProgramMode == ProgramMode.Main)
            {
                UpdateHydrogen();
                UpdateOxygen();
                UpdateSolar();
                UpdateRooms();
                UpdateLights();
                UpdateHangars();
            }

            if (SelectedProgramMode != ProgramMode.Fighter)
            {
                UpdateEnergy(); // depends on batteries, reactor and solar
            }

            // Displays should be last (for obvious reasons)
            UpdateDisplays100();
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region displays
        private enum DisplayMode { hydrogen, oxygen, batteries, solar, energy, thrust, thrustweight, environment, airlocks, rooms, hangars, inventory, ores, ingots, autopilot, connectors, settings, reactor, debug };
        private Dictionary<DisplayWidths, Dictionary<DisplayMode, List<IMyTextSurface>>> displays = new Dictionary<DisplayWidths, Dictionary<DisplayMode, List<IMyTextSurface>>>();

        // S = square LCDs, M = cockpit and block displays, L = Wide LCD
        private enum DisplayWidths { S = 20, M = 30, L = 40 };
        private DisplayWidths CurrentDisplayWidth = DisplayWidths.S;
        private int DisplayWidth = 20;
        private int DisplayHeight = 13;

        private Color COLOR_ORANGE = new Color(255, 161, 20);
        private Color COLOR_GREEN = new Color(42, 255, 36);
        private Color COLOR_RED = new Color(255, 22, 27);

        private Color CockpitDisplaysBackgroundColor = new Color(0, 70, 130);

        private void InitDisplays()
        {
            this.displays.Clear();

            this.displays.Add(DisplayWidths.S, new Dictionary<DisplayMode, List<IMyTextSurface>>());
            this.displays.Add(DisplayWidths.M, new Dictionary<DisplayMode, List<IMyTextSurface>>());
            this.displays.Add(DisplayWidths.L, new Dictionary<DisplayMode, List<IMyTextSurface>>());

            // LCDs
            List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(textPanels, CurrentGridOnly);
            foreach (var display in textPanels)
            {
                var properties = GetKeyValuePairsFromString(display.CustomData);
                if (!properties.ContainsKey("display"))
                    continue;

                bool isTransparent = ((IMyTextSurface)display).Name.StartsWith("Transparent");
                float fontSize = isTransparent ? 1.32f : 1.25f;
                float padding = isTransparent ? 0f : 2f;

                bool isWide = ((IMyTextSurface)display).SurfaceSize.X == 1024;

                AddTextSurface(properties["display"], isWide ? DisplayWidths.L : DisplayWidths.S, display, fontSize, padding);
            }

            // Cockpits
            List<IMyCockpit> cockpits = new List<IMyCockpit>();
            GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits, CurrentGridOnly);
            foreach (var cockpit in cockpits)
            {
                var properties = GetKeyValuePairsFromString(cockpit.CustomData);

                for (int i = 0; i < cockpit.SurfaceCount; i++)
                {
                    string key = "display" + (cockpit.SurfaceCount > 1 ? i.ToString() : "");

                    if (!properties.ContainsKey(key))
                        continue;

                    // TODO differentiate between cockpits and other displays and set font size and padding accordingly
                    // bool isLargeGrid = cockpit.CubeGrid.GridSizeEnum == VRage.Game.MyCubeSize.Large;
                    bool isActualCockpit = cockpit.SurfaceCount > 2; // all cockpits have more than 2 text surfaces

                    float fontSize = isActualCockpit ? 0.84f : 0.865f;
                    float padding = isActualCockpit ? 2f : 0.5f;

                    AddTextSurface(properties[key], DisplayWidths.M, cockpit.GetSurface(i), fontSize, padding);

                    if (isActualCockpit)
                    {
                        cockpit.GetSurface(i).BackgroundColor = CockpitDisplaysBackgroundColor;
                    }
                }
            }

            // Programmable block itself
            AddTextSurface(DisplayMode.settings.ToString(), DisplayWidths.M, Me.GetSurface(0), 0.86f, 0.5f);

        }

        private void AddTextSurface(string modeString, DisplayWidths width, IMyTextSurface surface, float fontSize, float padding)
        {
            DisplayMode mode;
            if (!DisplayMode.TryParse(modeString, out mode))
                return;

            if (!this.displays[width].ContainsKey(mode))
            {
                this.displays[width].Add(mode, new List<IMyTextSurface>());
            }

            this.displays[width][mode].Add(surface);

            surface.Font = FONT;
            surface.ContentType = ContentType.TEXT_AND_IMAGE;
            surface.FontSize = fontSize;
            surface.TextPadding = padding;
        }

        private bool HasDisplaysForMode(DisplayMode mode, int page = 1)
        {
            return displays[CurrentDisplayWidth].ContainsKey(mode);
        }

        private List<IMyTextSurface> GetDisplaysForMode(DisplayMode mode)
        {
            return displays[CurrentDisplayWidth][mode];
        }

        private void SetCurrentDisplayWidth(DisplayWidths width)
        {
            CurrentDisplayWidth = width;
            DisplayWidth = (int)width;
        }

        private void UpdateDisplays10()
        {
            foreach (DisplayWidths width in Enum.GetValues(typeof(DisplayWidths)))
            {
                SetCurrentDisplayWidth(width);
                DisplayEnvironment();
            }
        }

        private void UpdateDisplays100()
        {
            foreach (DisplayWidths width in Enum.GetValues(typeof(DisplayWidths)))
            {
                SetCurrentDisplayWidth(width);

                // all
                DisplayBatteries();
                DisplaySettings();
                DisplayDebug();

                if (SelectedProgramMode != ProgramMode.Fighter)
                {
                    DisplayInventory();
                    DisplayReactor();
                    DisplayEnergy();
                    DisplayThrust();
                    DisplayThrustWeight();
                    DisplayAutopilot();
                }

                if (SelectedProgramMode == ProgramMode.Main)
                {
                    DisplayHydrogen();
                    DisplayOxygen();
                    DisplaySolar();
                    DisplayAirlocks();
                    DisplayRooms();
                    DisplayHangars();
                    DisplayConnectors();
                }
            }
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region settings
        private Dictionary<string, bool> Settings = new Dictionary<string, bool>();

        private void SaveSettings()
        {
            var builder = new StringBuilder();
            foreach (var el in Settings)
            {
                builder.Append(el.Key);
                builder.Append(':');
                builder.Append(el.Value ? "1" : "0");
                builder.Append(';');
            }

            if (builder.Length > 0)
                builder.Length--; // remove trailing ;

            Storage = builder.ToString();
        }

        private void LoadSettings()
        {
            string[] storedData = Storage.Split(';');
            foreach (var element in storedData)
            {
                string[] keyValuePair = element.Split(':');
                if (keyValuePair.Length == 2)
                {
                    Settings.Add(keyValuePair[0], keyValuePair[1] == "1");
                }
            }
        }

        private bool HasSetting(string key)
        {
            return Settings.ContainsKey(key);
        }

        private void SetSetting(string key, bool value)
        {
            if (HasSetting(key))
            {
                Settings[key] = value;
            }
            else
            {
                Settings.Add(key, value);
            }
        }

        private void ClearSetting(string key)
        {
            if (HasSetting(key))
                Settings.Remove(key);
        }

        private bool GetSetting(string key, bool fallback = false)
        {
            if (HasSetting(key))
                return Settings[key];
            return fallback;
        }

        private void DisplaySettings()
        {
            if (!HasDisplaysForMode(DisplayMode.settings)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Settings");

            CreateTwoColumnText(text, "Uptime", Uptime.Hours.ToString("00") + ":" + Uptime.Minutes.ToString("00") + ":" + Uptime.Seconds.ToString("00"));

            text.Append('\n');

            CreateTwoColumnText(text, "Mode", SelectedProgramMode.ToString());
            CreateTwoColumnText(text, "Max Speed", ShipMaxSpeed.ToString(ThousandSeparatorNumberFormat));
            CreateTwoColumnText(text, "Airlock Delay", AirlockAutoClosingDelay.ToString());

            text.Append('\n');

            text.Append("Stored data:\n");
            foreach (var element in Settings)
            {
                CreateTwoColumnText(text, element.Key, element.Value ? "yes" : "no");
            }

            WriteRenderedTextToScreens(DisplayMode.settings, text);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region helpers
        private const string ThousandSeparatorNumberFormat = "#,##0";
        private const string ThousandSeparatorNumberFormatWithSingleDecimalPlace = "#,##0.0";
        private const string DoubleDecimalPlacesFormat = "0.00";
        private const string PercentageNumberFormat = "P0";

        #region characters
        public const char LINE_VERT = '\u2502';
        public const char LINE_HOR = '\u2500';

        public const char CORNER_TOP_LEFT = '\u250c';
        public const char CORNER_TOP_RIGHT = '\u2510';
        public const char CORNER_BOTTOM_LEFT = '\u2514';
        public const char CORNER_BOTTOM_RIGHT = '\u2518';

        public const char HALF_BLOCK_TOP = '\u2580';
        public const char HALF_BLOCK_BOTTOM = '\u2584';
        public const char HALF_BLOCK_LEFT = '\u258c';
        public const char HALF_BLOCK_RIGHT = '\u2590';

        public const char QUARTER_BLOCK_BOTTOM = '\u2581';

        public const char BLOCK_FULL = '\u2588';
        public const char BLOCK_LIGHT = '\u2591';
        public const char BLOCK_MEDIUM = '\u2592';
        public const char BLOCK_DENSE = '\u2593';
        #endregion

        /// <summary>
        /// Parses a string of one key:value pair per line 
        /// </summary>
        /// <param name="customData"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetKeyValuePairsFromString(string customData)
        {
            var lines = new List<string>(customData.Split('\n'));
            var properties = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var pair = line.Split(':');
                if (pair.Length == 2)
                {
                    properties.Add(pair[0], pair[1]);
                }
            }
            return properties;
        }

        private bool CurrentGridOnly(IMyCubeBlock block)
        {
            return block.CubeGrid == Me.CubeGrid;
        }

        #region Screen
        private StringBuilder GetStringBuilderForScreen()
        {
            // + 1 to account for line breaks
            return new StringBuilder((DisplayWidth + 1) * DisplayHeight);
        }

        #region Header
        private void CreateHeader(StringBuilder text, string title)
        {
            text.Append(title);
            text.Append('\n');
            text.Append(LINE_HOR, DisplayWidth);
            text.Append('\n');
        }
        private void CreateTwoColumnHeader(StringBuilder text, string title, string right)
        {
            CreateTwoColumnText(text, title, right);
            text.Append(LINE_HOR, DisplayWidth);
            text.Append('\n');
        }
        #endregion

        #region TextAlignment
        private void CreateCenteredText(StringBuilder text, string textCentered, char spacer = ' ')
        {
            int textLength = textCentered.Length;
            int spaceLeft = (int)Math.Max(0, Math.Floor((DisplayWidth - textLength) / 2d));
            text.Append(spacer, spaceLeft);
            text.Append(textCentered);
            text.Append('\n');
        }
        #endregion

        #region MulticolumnText
        private void CreateTwoColumnText(StringBuilder text, string textLeft, string textRight, char spacer = ' ')
        {
            int lenLeft = textLeft.Length;
            int lenRight = textRight.Length;
            int spaceBetween = DisplayWidth - lenLeft - lenRight;

            text.Append(textLeft);
            if (spaceBetween > 0)
            {
                text.Append(spacer, spaceBetween);
            }
            text.Append(textRight);
            text.Append('\n');
        }
        #endregion

        #region TextBar
        private void CreateBar(StringBuilder text, double percentage, bool border = true)
        {
            CreateBar(text, percentage, 0, border);
        }

        private void CreateBar(StringBuilder text, double percentage1, double percentage2, bool border = true, bool upDownMode = false)
        {
            int barWidth = DisplayWidth - 2;

            int width1 = Math.Max(0, (int)(barWidth * percentage1));
            int width2 = Math.Max(0, (int)(barWidth * percentage2));


            if (border)
            {
                text.Append(CORNER_TOP_LEFT);
                text.Append(LINE_HOR, barWidth);
                text.Append(CORNER_TOP_RIGHT);
                text.Append('\n');
            }

            text.Append(LINE_VERT);

            if (upDownMode)
            {
                int remainingWidth = Math.Max(0, barWidth - Math.Max(width1, width2));

                text.Append(BLOCK_FULL, Math.Min(width1, width2));
                text.Append(width1 > width2 ? HALF_BLOCK_TOP : HALF_BLOCK_BOTTOM, Math.Max(width1, width2));
                text.Append(' ', remainingWidth);
            }
            else
            {
                width2 = Math.Max(0, width2 - width1);
                int remainingWidth = Math.Max(0, barWidth - width1 - width2);

                text.Append(BLOCK_FULL, width1);
                text.Append(HALF_BLOCK_BOTTOM, width2);
                text.Append(border ? ' ' : QUARTER_BLOCK_BOTTOM, remainingWidth);
            }

            text.Append(LINE_VERT);

            if (border)
            {
                text.Append('\n');
                text.Append(CORNER_BOTTOM_LEFT);
                text.Append(LINE_HOR, barWidth);
                text.Append(CORNER_BOTTOM_RIGHT);
            }

            text.Append('\n');
        }
        #endregion

        private void CreateDoubleBar(StringBuilder text, double leftPercentage, double rightPercentage)
        {
            int barWidth = DisplayWidth - 2;

            int leftWidth = Math.Min(barWidth, Math.Max(0, (int)(barWidth * leftPercentage)));
            int rightWidth = Math.Min(barWidth, Math.Max(0, (int)(barWidth * rightPercentage)));

            int inBetweenWidth = barWidth - (leftWidth + rightWidth);

            text.Append(LINE_VERT);

            // overlapping
            if (inBetweenWidth < 0)
            {
                text.Append(HALF_BLOCK_BOTTOM, barWidth - rightWidth);
                text.Append(BLOCK_FULL, Math.Abs(inBetweenWidth));
                text.Append(HALF_BLOCK_TOP, barWidth - leftWidth);
            }
            else
            {
                text.Append(HALF_BLOCK_BOTTOM, leftWidth);
                text.Append(' ', inBetweenWidth);
                text.Append(HALF_BLOCK_TOP, rightWidth);
            }

            text.Append(LINE_VERT);
            text.Append('\n');
        }

        private void WriteRenderedTextToScreens(DisplayMode mode, StringBuilder text)
        {
            var panels = GetDisplaysForMode(mode);

            string renderedText = text.ToString();

            foreach (var panel in panels)
            {
                panel.WriteText(renderedText);
            }
        }

        #endregion

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region hydrogen_storage
        // cache
        private List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();

        // static
        private double hydrogenTotalCapacityInQm;

        // dynamic
        private double hydrogenTotalFillPercentage = 0;
        private double hydrogenAvailableFillPercentage = 0;
        private double hydrogenAvailableTotalCapacityInQm = 0;
        private double hydrogenNumInStockpileMode = 0;

        private void InitHydrogen()
        {
            hydrogenTanks.Clear();
            hydrogenTotalCapacityInQm = 0;

            List<IMyGasTank> tanks = new List<IMyGasTank>();
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, CurrentGridOnly);

            double hydrogenTotalCapacity = 0;

            foreach (var tank in tanks)
            {
                if (tank.BlockDefinition.SubtypeId.Contains("Hydro"))
                {
                    hydrogenTanks.Add(tank);
                    hydrogenTotalCapacity += tank.Capacity;
                }
            }

            hydrogenTotalCapacityInQm = hydrogenTotalCapacity / 1000;
        }

        private void UpdateHydrogen()
        {
            hydrogenNumInStockpileMode = 0;

            double fillRatio = 0;
            double availableFillRatio = 0;
            double availableTotalCapacity = 0;

            foreach (var tank in hydrogenTanks)
            {
                fillRatio += tank.FilledRatio;

                if (tank.Stockpile)
                {
                    hydrogenNumInStockpileMode++;
                }
                else
                {
                    availableFillRatio += tank.FilledRatio;
                    availableTotalCapacity += tank.Capacity;
                }
            }

            hydrogenTotalFillPercentage = fillRatio / hydrogenTanks.Count;
            hydrogenAvailableFillPercentage = availableFillRatio / hydrogenTanks.Count;
            hydrogenAvailableTotalCapacityInQm = availableTotalCapacity / 1000;
        }

        private void DisplayHydrogen()
        {
            if (!HasDisplaysForMode(DisplayMode.hydrogen)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Hydrogen");

            if (hydrogenNumInStockpileMode > 0)
            {
                // Available hydrogen in percent
                CreateTwoColumnText(text, "Available:", hydrogenAvailableFillPercentage.ToString(PercentageNumberFormat));

                // Available hydrogen in m³
                StringBuilder availableCapacity = new StringBuilder(DisplayWidth);
                availableCapacity.Append((hydrogenAvailableFillPercentage * hydrogenTotalCapacityInQm).ToString(ThousandSeparatorNumberFormat));
                availableCapacity.Append('/');
                availableCapacity.Append(hydrogenTotalCapacityInQm.ToString(ThousandSeparatorNumberFormat));
                availableCapacity.Append("m³");

                CreateTwoColumnText(text, string.Empty, availableCapacity.ToString());
                text.Append('\n');

                // Show stockpile warning
                CreateCenteredText(text, "Stockpile warning!");

                if (hydrogenNumInStockpileMode == hydrogenTanks.Count)
                {
                    CreateCenteredText(text, "All stockpiling!");
                }
                else
                {
                    CreateCenteredText(text, hydrogenNumInStockpileMode.ToString() + "/" + hydrogenTanks.Count.ToString() + " tanks");
                }

                text.Append("\n");
            }

            // Total hydrogen in percent
            CreateTwoColumnText(text, "Total:", hydrogenTotalFillPercentage.ToString(PercentageNumberFormat));

            // Total hydrogen in m³
            StringBuilder capacity = new StringBuilder(DisplayWidth);
            capacity.Append((hydrogenTotalFillPercentage * hydrogenTotalCapacityInQm).ToString(ThousandSeparatorNumberFormat));
            capacity.Append('/');
            capacity.Append(hydrogenTotalCapacityInQm.ToString(ThousandSeparatorNumberFormat));
            capacity.Append("m³");

            CreateTwoColumnText(text, string.Empty, capacity.ToString());

            WriteRenderedTextToScreens(DisplayMode.hydrogen, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region oxygen_storage
        // cache
        private List<IMyGasTank> oxygenTanks = new List<IMyGasTank>();

        // static
        private double oxygenTotalCapacityInQm;

        // dynamic
        private double oxygenTotalFillPercentage = 0;
        private double oxygenAvailableFillPercentage = 0;
        private double oxygenAvailableTotalCapacityInQm = 0;
        private double oxygenNumInStockpileMode = 0;

        private void InitOxygen()
        {
            oxygenTanks.Clear();
            oxygenTotalCapacityInQm = 0;

            List<IMyGasTank> tanks = new List<IMyGasTank>();
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, CurrentGridOnly);

            double oxygenTotalCapacity = 0;

            foreach (var tank in tanks)
            {
                if (!tank.BlockDefinition.SubtypeId.Contains("Hydro"))
                {
                    oxygenTanks.Add(tank);
                    oxygenTotalCapacity += tank.Capacity;
                }
            }

            oxygenTotalCapacityInQm = oxygenTotalCapacity / 1000;
        }

        private void UpdateOxygen()
        {
            oxygenNumInStockpileMode = 0;

            double fillRatio = 0;
            double availableFillRatio = 0;
            double availableTotalCapacity = 0;

            foreach (var tank in oxygenTanks)
            {
                fillRatio += tank.FilledRatio;

                if (tank.Stockpile)
                {
                    oxygenNumInStockpileMode++;
                }
                else
                {
                    availableFillRatio += tank.FilledRatio;
                    availableTotalCapacity += tank.Capacity;
                }
            }

            oxygenTotalFillPercentage = fillRatio / oxygenTanks.Count;
            oxygenAvailableFillPercentage = availableFillRatio / oxygenTanks.Count;
            oxygenAvailableTotalCapacityInQm = availableTotalCapacity / 1000;
        }

        private void DisplayOxygen()
        {
            if (!HasDisplaysForMode(DisplayMode.oxygen)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Oxygen");

            if (oxygenNumInStockpileMode > 0)
            {
                // Available oxygen in percent
                CreateTwoColumnText(text, "Available:", oxygenAvailableFillPercentage.ToString(PercentageNumberFormat));

                // Available oxygen in m³
                StringBuilder availableCapacity = new StringBuilder(DisplayWidth);
                availableCapacity.Append((oxygenAvailableFillPercentage * oxygenTotalCapacityInQm).ToString(ThousandSeparatorNumberFormat));
                availableCapacity.Append('/');
                availableCapacity.Append(oxygenTotalCapacityInQm.ToString(ThousandSeparatorNumberFormat));
                availableCapacity.Append("m³");

                CreateTwoColumnText(text, string.Empty, availableCapacity.ToString());
                text.Append('\n');

                // Show stockpile warning
                CreateCenteredText(text, "Stockpile warning!");

                if (oxygenNumInStockpileMode == oxygenTanks.Count)
                {
                    CreateCenteredText(text, "All stockpiling!");
                }
                else
                {
                    CreateCenteredText(text, oxygenNumInStockpileMode.ToString() + "/" + oxygenTanks.Count.ToString() + " tanks");
                }

                text.Append("\n");
            }

            // Total oxygen in percent
            CreateTwoColumnText(text, "Total:", oxygenTotalFillPercentage.ToString(PercentageNumberFormat));

            // Total oxygen in m³
            StringBuilder capacity = new StringBuilder(DisplayWidth);
            capacity.Append((oxygenTotalFillPercentage * oxygenTotalCapacityInQm).ToString(ThousandSeparatorNumberFormat));
            capacity.Append('/');
            capacity.Append(oxygenTotalCapacityInQm.ToString(ThousandSeparatorNumberFormat));
            capacity.Append("m³");

            CreateTwoColumnText(text, string.Empty, capacity.ToString());

            WriteRenderedTextToScreens(DisplayMode.oxygen, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region batteries
        // cache
        private List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();

        // static
        private double batteriesTotalCapacity;
        private double batteriesMaxOutput;
        private double batteriesMaxInput;

        // dynamic
        private double batteriesCharge = 0;
        private double batteriesChargePercentage = 0;

        private double batteriesOutput = 0;
        private double batteriesOutputPercentage = 0;

        private double batteriesInput = 0;
        private double batteriesInputPercentage = 0;

        private int batteriesNumModeRecharging = 0;
        private int batteriesNumModeDischarging = 0;
        private int batteriesNumModeAuto = 0;


        private void InitBatteries()
        {
            batteries.Clear();
            batteriesTotalCapacity = 0;
            batteriesMaxOutput = 0;
            batteriesMaxInput = 0;

            GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, b => b.CubeGrid == Me.CubeGrid);

            foreach (var battery in batteries)
            {
                batteriesTotalCapacity += battery.MaxStoredPower;
                batteriesMaxOutput += battery.MaxOutput;
                batteriesMaxInput += battery.MaxInput;
            }
        }

        private void UpdateBatteries()
        {
            batteriesCharge = 0;
            batteriesOutput = 0;
            batteriesInput = 0;

            batteriesNumModeRecharging = 0;
            batteriesNumModeDischarging = 0;
            batteriesNumModeAuto = 0;

            foreach (var battery in batteries)
            {

                batteriesCharge += battery.CurrentStoredPower;
                batteriesOutput += battery.CurrentOutput;
                batteriesInput += battery.CurrentInput;

                switch (battery.ChargeMode)
                {
                    case ChargeMode.Auto: batteriesNumModeAuto++; break;
                    case ChargeMode.Discharge: batteriesNumModeDischarging++; break;
                    case ChargeMode.Recharge: batteriesNumModeRecharging++; break;
                }

            }

            batteriesChargePercentage = batteriesCharge / batteriesTotalCapacity;
            batteriesInputPercentage = batteriesInput / batteriesMaxInput;
            batteriesOutputPercentage = batteriesOutput / batteriesMaxOutput;
        }

        private void DisplayBatteries()
        {
            if (!HasDisplaysForMode(DisplayMode.batteries)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Batteries");

            CreateTwoColumnText(text, "Charge:", batteriesChargePercentage.ToString(PercentageNumberFormat));

            StringBuilder capacity = new StringBuilder(DisplayWidth);
            capacity.Append(batteriesCharge.ToString(ThousandSeparatorNumberFormat));
            capacity.Append('/');
            capacity.Append(batteriesTotalCapacity.ToString(ThousandSeparatorNumberFormat));
            capacity.Append(" MWh");
            CreateTwoColumnText(text, string.Empty, capacity.ToString());

            CreateBar(text, batteriesChargePercentage, false);
            text.Append('\n');

            StringBuilder input = new StringBuilder(DisplayWidth);
            input.Append(batteriesInput.ToString(ThousandSeparatorNumberFormat));
            input.Append('/');
            input.Append(batteriesMaxInput.ToString(ThousandSeparatorNumberFormat));
            input.Append(" MWh");
            CreateTwoColumnText(text, "Input", input.ToString());

            CreateBar(text, batteriesInputPercentage, batteriesOutputPercentage, false, true);

            StringBuilder output = new StringBuilder(DisplayWidth);
            output.Append(batteriesOutput.ToString(ThousandSeparatorNumberFormat));
            output.Append('/');
            output.Append(batteriesMaxOutput.ToString(ThousandSeparatorNumberFormat));
            output.Append(" MWh");
            CreateTwoColumnText(text, "Output", output.ToString());
            text.Append('\n');

            if (batteriesNumModeRecharging == 0 && batteriesNumModeDischarging == 0)
            {
                CreateCenteredText(text, "All in auto mode!");
            }
            else
            {
                text.Append(batteriesNumModeRecharging);
                text.Append(" recharging\n");
                text.Append(batteriesNumModeDischarging);
                text.Append(" discharging");
            }

            WriteRenderedTextToScreens(DisplayMode.batteries, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region solar
        // cache
        private List<IMySolarPanel> solarPanels = new List<IMySolarPanel>();

        // static
        private double solarMaxOutput = 0;

        // dynamic
        double solarCurrentMaxOutput = 0;
        double solarCurrentMaxOutputPercentage = 0;
        double solarCurrentOutput = 0;
        double solarCurrentOutputPercentage = 0;

        private void InitSolar()
        {
            solarPanels.Clear();
            solarMaxOutput = 0;

            GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(solarPanels, CurrentGridOnly);
            foreach (var solarPanel in solarPanels)
            {
                solarMaxOutput += 0.160d; // large solar panels can output up to 160 kW
            }
        }

        private void UpdateSolar()
        {
            double currentMaxOutput = 0;
            double currentOutput = 0;

            foreach (var solarPanel in solarPanels)
            {
                currentMaxOutput += solarPanel.MaxOutput;
                currentOutput += solarPanel.CurrentOutput;
            }

            solarCurrentMaxOutput = currentMaxOutput;
            solarCurrentOutput = currentOutput;

            solarCurrentMaxOutputPercentage = solarCurrentMaxOutput / solarMaxOutput;
            solarCurrentOutputPercentage = solarCurrentOutput / solarMaxOutput;
        }

        private void DisplaySolar()
        {
            if (!HasDisplaysForMode(DisplayMode.solar)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Solar Panels");

            CreateTwoColumnText(text, "Current:", solarCurrentOutputPercentage.ToString(PercentageNumberFormat));

            StringBuilder output = new StringBuilder(DisplayWidth);
            output.Append(solarCurrentOutput.ToString(ThousandSeparatorNumberFormatWithSingleDecimalPlace));
            output.Append('/');
            output.Append(solarCurrentMaxOutput.ToString(ThousandSeparatorNumberFormatWithSingleDecimalPlace));
            output.Append('/');
            output.Append(solarMaxOutput.ToString(ThousandSeparatorNumberFormatWithSingleDecimalPlace));
            output.Append(" MWh");
            CreateTwoColumnText(text, string.Empty, output.ToString());
            text.Append('\n');

            CreateBar(text, solarCurrentOutputPercentage, solarCurrentMaxOutputPercentage, false);

            WriteRenderedTextToScreens(DisplayMode.solar, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region reactor
        private List<IMyReactor> reactors = new List<IMyReactor>();

        private double reactorMaxOutput = 0;
        private double reactorCurrentOutput = 0;
        private double reactorCurrentOutputPercentage = 0;

        private void InitReactor() { }

        private void UpdateReactor()
        {
            reactors.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors, CurrentGridOnly);
            UpdateEnergyProducer(reactors, out reactorMaxOutput, out reactorCurrentOutput, out reactorCurrentOutputPercentage);
        }

        private void DisplayReactor()
        {
            if (!HasDisplaysForMode(DisplayMode.reactor)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Reactors");

            CreateTwoColumnText(text, "Current:", reactorCurrentOutputPercentage.ToString(PercentageNumberFormat));

            StringBuilder output = new StringBuilder(DisplayWidth);
            output.Append(reactorCurrentOutput.ToString(ThousandSeparatorNumberFormatWithSingleDecimalPlace));
            output.Append('/');
            output.Append(reactorMaxOutput.ToString(ThousandSeparatorNumberFormatWithSingleDecimalPlace));
            output.Append(" MWh");
            CreateTwoColumnText(text, string.Empty, output.ToString());
            text.Append('\n');

            CreateBar(text, reactorCurrentOutputPercentage, false);
            text.Append('\n');

            var amountUranium = GetAmountOfResource(InventoryResourceType.Ingot, InventoryResourceName.Uranium);
            CreateTwoColumnText(text, "Uranium", amountUranium.ToString(ThousandSeparatorNumberFormat) + "kg");

            WriteRenderedTextToScreens(DisplayMode.reactor, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region energy
        private List<IMyPowerProducer> powerProducers = new List<IMyPowerProducer>();

        private double powerProducersMaxOutput = 0;
        private double powerProducersCurrentOutput = 0;
        private double powerProducersCurrentOutputPercentage = 0;

        private double otherPowerProducersMaxOutput = 0;
        private double otherPowerProducersCurrentOutput = 0;
        private double otherPowerProducersCurrentOutputPercentage = 0;

        private void InitEnergy()
        {
            powerProducers.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(powerProducers, CurrentGridOnly);
        }

        private void UpdateEnergy()
        {
            UpdateEnergyProducer(powerProducers, out powerProducersMaxOutput, out powerProducersCurrentOutput, out powerProducersCurrentOutputPercentage);
            otherPowerProducersMaxOutput = powerProducersMaxOutput - batteriesMaxOutput - solarMaxOutput - reactorMaxOutput;
            otherPowerProducersCurrentOutput = powerProducersCurrentOutput - batteriesOutput - solarCurrentOutput - reactorCurrentOutput;
            otherPowerProducersCurrentOutputPercentage = otherPowerProducersCurrentOutput / otherPowerProducersMaxOutput;
        }

        private void UpdateEnergyProducer(IReadOnlyList<IMyPowerProducer> list, out double maxOutput, out double currentOutput, out double currentOutputPercentage)
        {
            maxOutput = 0;
            currentOutput = 0;

            foreach (var item in list)
            {
                maxOutput += item.MaxOutput;
                currentOutput += item.CurrentOutput;
            }

            currentOutputPercentage = currentOutput / maxOutput;
        }

        private void DisplayEnergy()
        {
            if (!HasDisplaysForMode(DisplayMode.energy)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Energy Output");

            CreateTwoColumnText(text, "Batteries", batteriesOutputPercentage.ToString(PercentageNumberFormat));
            CreateBar(text, batteriesOutputPercentage, false);
            text.Append('\n');

            CreateTwoColumnText(text, "Solar", solarCurrentOutputPercentage.ToString(PercentageNumberFormat));
            CreateBar(text, solarCurrentOutputPercentage, false);
            text.Append('\n');

            CreateTwoColumnText(text, "Reactors", reactorCurrentOutputPercentage.ToString(PercentageNumberFormat));
            CreateBar(text, reactorCurrentOutputPercentage, false);
            text.Append('\n');

            CreateTwoColumnText(text, "Other", otherPowerProducersCurrentOutputPercentage.ToString(PercentageNumberFormat));
            CreateBar(text, otherPowerProducersCurrentOutputPercentage, false);
            text.Append('\n');

            var amountUranium = GetAmountOfResource(InventoryResourceType.Ingot, InventoryResourceName.Uranium);
            if (amountUranium < 15)
            {
                CreateCenteredText(text, "Uranium low!");
            }

            WriteRenderedTextToScreens(DisplayMode.energy, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region shipReference
        // static
        private IMyShipController mainShipController = null;
        private const string requiresMainShipController = "requires main cockpit";

        private void InitShipReference()
        {
            this.mainShipController = null;

            var shipControllers = new List<IMyShipController>();
            GridTerminalSystem.GetBlocksOfType<IMyShipController>(shipControllers);

            foreach (var shipController in shipControllers)
            {
                if (shipController.IsMainCockpit)
                {
                    this.mainShipController = shipController;
                    return;
                }
            }
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region thrust
        #region thrust_helpers
        private class ThrustInformation
        {
            public float currentThrust = 0; // in N
            public float maxThrust = 0; // in N
            public float maxEffectiveThrust = 0; // in N

            public void Add(IMyThrust thruster)
            {
                currentThrust += thruster.CurrentThrust;
                maxThrust += thruster.MaxThrust;
                maxEffectiveThrust += thruster.MaxEffectiveThrust;
            }

            public void Clear()
            {
                currentThrust = 0;
                maxThrust = 0;
                maxEffectiveThrust = 0;
            }

        }

        private string getStringForThrusterDirection(Base6Directions.Direction direction)
        {
            switch (direction)
            {
                case Base6Directions.Direction.Up: return "Up";
                case Base6Directions.Direction.Down: return "Down";
                case Base6Directions.Direction.Left: return "Left";
                case Base6Directions.Direction.Right: return "Right";
                case Base6Directions.Direction.Forward: return "Forwd";
                case Base6Directions.Direction.Backward: return "Backwd";
                default: return "???";
            }
        }
        #endregion

        private Dictionary<Base6Directions.Direction, List<IMyThrust>> thrusterDirections = new Dictionary<Base6Directions.Direction, List<IMyThrust>>();
        private Dictionary<Base6Directions.Direction, ThrustInformation> thrusterInformation = new Dictionary<Base6Directions.Direction, ThrustInformation>();

        private void InitThrust()
        {
            if (mainShipController == null)
                return;

            thrusterInformation.Clear();
            thrusterDirections.Clear();

            List<IMyThrust> thrusters = new List<IMyThrust>();
            GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters, CurrentGridOnly);

            var directions = Enum.GetValues(typeof(Base6Directions.Direction));
            foreach (Base6Directions.Direction direction in directions)
            {
                thrusterDirections.Add(direction, new List<IMyThrust>());
                thrusterInformation.Add(direction, new ThrustInformation());
            }

            foreach (var thruster in thrusters)
            {
                // stolen from https://gist.github.com/ZerothAngel/da177f8a02347ac252b9
                var facing = thruster.Orientation.TransformDirection(Base6Directions.Direction.Forward); // Exhaust goes this way
                var thrustDirection = Base6Directions.GetFlippedDirection(facing);
                var shipDirection = mainShipController.Orientation.TransformDirectionInverse(thrustDirection);

                thrusterDirections[shipDirection].Add(thruster);
            }
        }

        private void UpdateThrust()
        {
            if (mainShipController == null)
                return;

            foreach (var direction in thrusterInformation.Keys)
            {
                thrusterInformation[direction].Clear();

                foreach (var thruster in thrusterDirections[direction])
                {
                    thrusterInformation[direction].Add(thruster);
                }
            }
        }

        private void DisplayThrust()
        {
            if (!HasDisplaysForMode(DisplayMode.thrust)) return;

            var text = GetStringBuilderForScreen();
            CreateTwoColumnHeader(text, "Thrust", "(eff/max)");

            if (mainShipController == null)
            {
                text.Append('\n');
                CreateCenteredText(text, requiresMainShipController);
            }
            else
            {
                PrintThrustForPair(text, Base6Directions.Direction.Up, Base6Directions.Direction.Down);
                PrintThrustForPair(text, Base6Directions.Direction.Left, Base6Directions.Direction.Right);
                PrintThrustForPair(text, Base6Directions.Direction.Forward, Base6Directions.Direction.Backward);
            }

            WriteRenderedTextToScreens(DisplayMode.thrust, text);
        }

        private void PrintThrustForPair(StringBuilder text, Base6Directions.Direction left, Base6Directions.Direction right)
        {
            var leftThrustPercentage = GetThrustPercentageFor(left);
            var rightThrustPercentage = GetThrustPercentageFor(right);

            PrintThrustInfoLine(text, left);
            CreateBar(text, leftThrustPercentage, rightThrustPercentage, false, true);
            PrintThrustInfoLine(text, right);

            //CreateDoubleBar(text, leftThrustPercentage, rightThrustPercentage);
            text.Append('\n');
        }

        private void PrintThrustInfoLine(StringBuilder text, Base6Directions.Direction direction)
        {
            var info = thrusterInformation[direction];
            var title = getStringForThrusterDirection(direction);

            const string noThrust = "0";
            string currToMaxEffectivePercentage = info.maxEffectiveThrust == 0 ? noThrust : (info.currentThrust / info.maxEffectiveThrust).ToString(PercentageNumberFormat);
            string currToMaxPercentage = info.maxThrust == 0 ? noThrust : (info.currentThrust / info.maxThrust).ToString(PercentageNumberFormat);

            CreateTwoColumnText(text, title, currToMaxEffectivePercentage + " / " + currToMaxPercentage);
        }

        private double GetThrustPercentageFor(Base6Directions.Direction direction)
        {
            var info = thrusterInformation[direction];
            return info.currentThrust / info.maxThrust;
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region thrust_weight
        private double thrustWeightDownwardForceInN = 0;

        private void UpdateThrustWeight()
        {
            if (mainShipController == null)
                return;

            thrustWeightDownwardForceInN = envIsInPlanetReach ? envShipTotalMassInKg * envCurrentGravity : 0;
        }

        private void DisplayThrustWeight()
        {
            if (!HasDisplaysForMode(DisplayMode.thrustweight)) return;

            var text = GetStringBuilderForScreen();

            CreateTwoColumnHeader(text, envIsInPlanetReach ? "Thrust/Weight" : "Thrust", envIsInPlanetReach ? "(eff/max)" : "(kN)");

            if (mainShipController == null)
            {
                text.Append('\n');
                CreateCenteredText(text, requiresMainShipController);
            }
            else
            {
                foreach (var entry in thrusterInformation)
                {
                    var title = getStringForThrusterDirection(entry.Key);
                    string maxThrustkN = (entry.Value.maxThrust / 1000).ToString(ThousandSeparatorNumberFormat);

                    if (envIsInPlanetReach)
                    {
                        string downforceToMaxThrustRatio = (entry.Value.maxThrust / thrustWeightDownwardForceInN).ToString(DoubleDecimalPlacesFormat);
                        string downforceToMaxEffectiveThrusttRatio = (entry.Value.maxEffectiveThrust / thrustWeightDownwardForceInN).ToString(DoubleDecimalPlacesFormat);

                        CreateTwoColumnText(text, title, downforceToMaxThrustRatio + " / " + downforceToMaxEffectiveThrusttRatio);
                    }
                    else
                    {
                        CreateTwoColumnText(text, title, maxThrustkN + " kN");
                    }
                }
            }

            WriteRenderedTextToScreens(DisplayMode.thrustweight, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region environment
        // dynamic
        private double envCurrentGravity = 0;
        private double environmentCurrentSpeed = 0;

        private bool envIsInPlanetReach = false;
        private double envSeaLevelElevation = 0;
        private double envSurfaceLevelElevation = 0;

        private float envShipTotalMassInKg = 0f;

        private IMyAirVent envOutsideVent = null;
        private float envOutsideAirPressure = 0f;

        private void InitEnvironment()
        {
            envCurrentGravity = 0;
            environmentCurrentSpeed = 0;

            envOutsideVent = null;
            List<IMyAirVent> airVents = new List<IMyAirVent>();
            GridTerminalSystem.GetBlocksOfType<IMyAirVent>(airVents, CurrentGridOnly);
            foreach (var vent in airVents)
            {
                var properties = GetKeyValuePairsFromString(vent.CustomData);
                if (properties.ContainsKey("vent") && properties["vent"] == "environment")
                {
                    envOutsideVent = vent;
                    break;
                }
            }
        }

        private void UpdateEnvironment10()
        {
            envOutsideAirPressure = envOutsideVent != null ? envOutsideVent.GetOxygenLevel() : 0f;

            if (mainShipController == null) return;
            environmentCurrentSpeed = mainShipController.GetShipSpeed();
        }

        private void UpdateEnvironment100()
        {
            // auto fill oxygen tanks when in atmosphere
            if (envOutsideVent != null && envOutsideAirPressure > 0)
                envOutsideVent.Depressurize = oxygenTotalFillPercentage < 1;

            if (mainShipController == null) return;

            envCurrentGravity = mainShipController.GetNaturalGravity().Length();

            envIsInPlanetReach = (
                mainShipController.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out envSeaLevelElevation)
                && mainShipController.TryGetPlanetElevation(MyPlanetElevation.Surface, out envSurfaceLevelElevation)
            );

            var massInformation = mainShipController.CalculateShipMass();
            envShipTotalMassInKg = massInformation.TotalMass;
        }

        private void DisplayEnvironment()
        {
            if (!HasDisplaysForMode(DisplayMode.environment)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Environment");

            if (mainShipController == null)
            {
                text.Append('\n');
                CreateCenteredText(text, requiresMainShipController);
                text.Append("\n");
            }
            else
            {

                CreateTwoColumnText(text, "Gravity (m/s²)", envCurrentGravity.ToString(DoubleDecimalPlacesFormat));

                CreateTwoColumnText(text, "Speed (m/s)", environmentCurrentSpeed.ToString(DoubleDecimalPlacesFormat));
                text.Append('\n');
                CreateBar(text, environmentCurrentSpeed / ShipMaxSpeed, false);
                text.Append("\n");

                CreateTwoColumnText(text, "Mass", envShipTotalMassInKg.ToString(ThousandSeparatorNumberFormat));
                text.Append("\n");

                if (envIsInPlanetReach)
                {
                    CreateTwoColumnText(text, "Surface", envSurfaceLevelElevation.ToString(ThousandSeparatorNumberFormat));
                    CreateTwoColumnText(text, "Sea Level", envSeaLevelElevation.ToString(ThousandSeparatorNumberFormat));
                    text.Append("\n");
                }
            }

            if (envOutsideVent != null)
            {
                CreateTwoColumnText(text, "Air pressure", envOutsideAirPressure.ToString(PercentageNumberFormat));
            }

            WriteRenderedTextToScreens(DisplayMode.environment, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region warnings

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region checklist

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region inventories
        // cache
        private List<IMyInventory> inventories = new List<IMyInventory>();

        // static
        private double inventoriesMaxVolume = 0;

        // dynamic
        private double inventoriesCurrentVolume = 0;
        private double inventoriesCurrentMass = 0;
        private double inventoriesFillPercentage = 0;

        private enum InventoryResourceType { Ore, Ingot };
        private enum InventoryResourceName { Iron, Nickel, Cobald, Gold, Silver, Silicon, Uranium, Platinum, Stone, Magnesium };

        private Dictionary<InventoryResourceType, Dictionary<InventoryResourceName, double>> inventoryResources = new Dictionary<InventoryResourceType, Dictionary<InventoryResourceName, double>>();

        List<MyInventoryItem> inventoryTmpItemsList = new List<MyInventoryItem>(); // tmp list to avoid reallocation every update

        private void InitInventories()
        {
            inventories.Clear();
            inventoryResources.Clear();

            inventoryResources.Add(InventoryResourceType.Ore, new Dictionary<InventoryResourceName, double>());
            inventoryResources.Add(InventoryResourceType.Ingot, new Dictionary<InventoryResourceName, double>());

            this.inventoriesMaxVolume = 0;
            long inventoriesMaxVolume = 0;

            List<IMyTerminalBlock> potentialInventoryHolders = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(potentialInventoryHolders, CurrentGridOnly);

            foreach (var potentialInventoryHolder in potentialInventoryHolders)
            {
                if (!potentialInventoryHolder.HasInventory)
                    continue;

                for (int i = 0; i < potentialInventoryHolder.InventoryCount; i++)
                {
                    IMyInventory inventory = potentialInventoryHolder.GetInventory(i);
                    inventories.Add(inventory);
                    inventoriesMaxVolume += inventory.MaxVolume.RawValue;
                }
            }

            this.inventoriesMaxVolume = inventoriesMaxVolume / 1000000d;
        }

        private void UpdateInventories()
        {
            inventoryResources[InventoryResourceType.Ore].Clear();
            inventoryResources[InventoryResourceType.Ingot].Clear();

            long inventoriesCurrentVolume = 0;
            long inventoriesCurrentMass = 0;

            foreach (var inventory in inventories)
            {
                inventoriesCurrentVolume += inventory.CurrentVolume.RawValue;
                inventoriesCurrentMass += inventory.CurrentMass.RawValue;

                inventoryTmpItemsList.Clear();
                inventory.GetItems(inventoryTmpItemsList);
                InventoryAddItems(inventoryTmpItemsList);
            }

            this.inventoriesCurrentVolume = inventoriesCurrentVolume / 1000000d;
            this.inventoriesCurrentMass = inventoriesCurrentMass / 1000000d;
            inventoriesFillPercentage = this.inventoriesMaxVolume > 0 ? this.inventoriesCurrentVolume / this.inventoriesMaxVolume : 0;
        }

        private void InventoryAddItems(List<MyInventoryItem> items)
        {
            foreach (var item in items)
            {
                InventoryResourceName resource;
                if (!InventoryResourceName.TryParse(item.Type.SubtypeId, out resource))
                    continue;

                InventoryResourceType type;
                if (item.Type.TypeId.EndsWith("Ore"))
                {
                    type = InventoryResourceType.Ore;
                }
                else if (item.Type.TypeId.EndsWith("Ingot"))
                {
                    type = InventoryResourceType.Ingot;
                }
                else
                {
                    continue;
                }

                var list = inventoryResources[type];

                if (!list.ContainsKey(resource))
                    list.Add(resource, 0);

                list[resource] += item.Amount.RawValue / 1000000d;
            }
        }

        private void DisplayInventory()
        {
            DisplayInventoryOverview();

            DisplayInventoryResource(DisplayMode.ingots, InventoryResourceType.Ingot, 2);
            DisplayInventoryResource(DisplayMode.ores, InventoryResourceType.Ore, 3);
        }

        private void DisplayInventoryOverview()
        {
            if (!HasDisplaysForMode(DisplayMode.inventory)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Inventory");

            CreateTwoColumnText(text, "Total", inventoriesFillPercentage.ToString(PercentageNumberFormat));

            StringBuilder volume = new StringBuilder(DisplayWidth);
            volume.Append(inventoriesCurrentVolume.ToString(ThousandSeparatorNumberFormatWithSingleDecimalPlace));
            volume.Append("/");
            volume.Append(inventoriesMaxVolume.ToString(ThousandSeparatorNumberFormatWithSingleDecimalPlace));
            volume.Append(" hm³");
            CreateTwoColumnText(text, string.Empty, volume.ToString());
            text.Append('\n');

            CreateBar(text, inventoriesFillPercentage, false);

            // Show list of ores in miner mode
            if (SelectedProgramMode == ProgramMode.Miner && inventoryResources.ContainsKey(InventoryResourceType.Ore))
            {
                text.Append('\n');
                foreach (var resource in inventoryResources[InventoryResourceType.Ore])
                {
                    CreateTwoColumnText(text, resource.Key.ToString(), resource.Value.ToString(ThousandSeparatorNumberFormatWithSingleDecimalPlace));
                }
            }

            WriteRenderedTextToScreens(DisplayMode.inventory, text);
        }

        private void DisplayInventoryResource(DisplayMode mode, InventoryResourceType resourceType, int page)
        {
            if (!HasDisplaysForMode(mode)) return;

            var text = GetStringBuilderForScreen();
            CreateTwoColumnHeader(text, resourceType.ToString(), "kg");

            foreach (var resource in inventoryResources[resourceType])
            {
                CreateTwoColumnText(text, resource.Key.ToString(), resource.Value.ToString(ThousandSeparatorNumberFormatWithSingleDecimalPlace));
            }

            WriteRenderedTextToScreens(mode, text);
        }

        private bool HasResource(InventoryResourceType type, InventoryResourceName resource)
        {
            return inventoryResources[type].ContainsKey(resource);
        }

        private double GetAmountOfResource(InventoryResourceType type, InventoryResourceName resource)
        {
            if (!HasResource(type, resource))
                return 0;
            return inventoryResources[type][resource];
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region rooms
        // cache
        private Dictionary<string, List<IMyAirVent>> roomVents = new Dictionary<string, List<IMyAirVent>>();
        private Dictionary<string, float> roomPressureLevels = new Dictionary<string, float>();

        private void InitRooms()
        {
            roomVents.Clear();

            List<IMyAirVent> vents = new List<IMyAirVent>();
            GridTerminalSystem.GetBlocksOfType<IMyAirVent>(vents, CurrentGridOnly);

            foreach (var vent in vents)
            {
                var properties = GetKeyValuePairsFromString(vent.CustomData);

                if (!properties.ContainsKey("room"))
                    continue;

                string roomName = properties["room"];

                if (!roomVents.ContainsKey(roomName))
                {
                    roomVents[roomName] = new List<IMyAirVent>();
                }

                roomVents[roomName].Add(vent);
            }

        }

        private void UpdateRooms()
        {

        }

        private float getPressureLevelForRoom(string roomName)
        {
            if (!roomVents.ContainsKey(roomName))
                return 0f;

            return roomVents[roomName][0].GetOxygenLevel();
        }

        private void DisplayRooms()
        {
            if (!HasDisplaysForMode(DisplayMode.rooms)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Rooms");

            CreateTwoColumnText(text, "Room", "Pressure");

            foreach (var roomName in roomVents.Keys)
            {
                string status = getPressureLevelForRoom(roomName).ToString(PercentageNumberFormat);
                CreateTwoColumnText(text, roomName, status);
            }

            WriteRenderedTextToScreens(DisplayMode.rooms, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region lights
        private Dictionary<string, List<IMyLightingBlock>> lights = new Dictionary<string, List<IMyLightingBlock>>();

        private void InitLights()
        {
            lights.Clear();
            List<IMyLightingBlock> allLights = new List<IMyLightingBlock>();

            GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(allLights, CurrentGridOnly);

            foreach (var light in allLights)
            {
                var properties = GetKeyValuePairsFromString(light.CustomData);

                if (!properties.ContainsKey("room"))
                    continue;

                string roomName = properties["room"];

                if (!lights.ContainsKey(roomName))
                {
                    lights[roomName] = new List<IMyLightingBlock>();
                }

                lights[roomName].Add(light);
            }

        }

        private void UpdateLights()
        {
            // Nothing to do
        }

        private void CommandLights(string argument)
        {
            string[] arguments = argument.Split(' ');

            if (arguments.Length != 2)
                return;

            string roomName = arguments[1];

            if (!lights.ContainsKey(roomName))
                return;

            var roomLights = lights[roomName];

            switch (arguments[0])
            {
                case "on":
                    roomLights.ForEach(l => l.Enabled = true);
                    break;

                case "off":
                    roomLights.ForEach(l => l.Enabled = false);
                    break;

                case "toggle":
                    roomLights.ForEach(l => l.Enabled = !l.Enabled);
                    break;
            }
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region airlocks
        // cache
        private Dictionary<string, List<IMyDoor>> airlockDoors = new Dictionary<string, List<IMyDoor>>();
        private Dictionary<IMyDoor, int> airlockOpenDoorDelays = new Dictionary<IMyDoor, int>();
        private Dictionary<IMyDoor, string> airlockDoorToRoom = new Dictionary<IMyDoor, string>();

        private void InitAirlocks()
        {
            airlockDoors.Clear();
            airlockOpenDoorDelays.Clear();
            airlockDoorToRoom.Clear();

            List<IMyDoor> doors = new List<IMyDoor>();
            GridTerminalSystem.GetBlocksOfType<IMyDoor>(doors);

            foreach (var door in doors)
            {
                var properties = GetKeyValuePairsFromString(door.CustomData);

                if (!properties.ContainsKey("airlock"))
                    continue;

                string groupName = properties["airlock"];

                door.CloseDoor();

                if (!this.airlockDoors.ContainsKey(groupName))
                {
                    this.airlockDoors.Add(groupName, new List<IMyDoor>());
                }

                this.airlockDoors[groupName].Add(door);

                if (properties.ContainsKey("room"))
                {
                    string roomName = properties["room"];
                    this.airlockDoorToRoom.Add(door, roomName);
                }
            }
        }

        private void UpdateAirlocks()
        {
            // update delay left
            var doors = new List<IMyDoor>(airlockOpenDoorDelays.Keys);
            foreach (var door in doors)
            {
                airlockOpenDoorDelays[door] = Math.Max(0, airlockOpenDoorDelays[door] - 10);
            }

            // update all door groups (aka airlocks)
            foreach (string group in airlockDoors.Keys)
            {
                bool allDoorsClosed = true;

                foreach (var door in airlockDoors[group])
                {
                    if (door.Status != DoorStatus.Closed)
                    {
                        allDoorsClosed = false;
                        break;
                    }
                }

                bool openDoorsInGroupAreSafe = true;
                foreach (IMyDoor door in airlockDoors[group])
                {
                    if (door.Status == DoorStatus.Closed)
                    {
                        continue;
                    }

                    // if an open airlock door does not have a room defined,
                    // it is treated as a door to outside.
                    if (!airlockDoorToRoom.ContainsKey(door))
                    {
                        if (envOutsideAirPressure < MIN_PRESSURE_LEVEL)
                        {
                            openDoorsInGroupAreSafe = false;
                            break;
                        }

                        continue;
                    }

                    float pressure = getPressureLevelForRoom(airlockDoorToRoom[door]);
                    if (pressure < MIN_PRESSURE_LEVEL)
                    {
                        openDoorsInGroupAreSafe = false;
                        break;
                    }
                }

                foreach (IMyDoor door in airlockDoors[group])
                {
                    // only send command to open doors
                    if (door.Status == DoorStatus.Open)
                    {
                        // if the door is not within the ticks left open dict,
                        // the door is open for the first iteration.
                        // put it in with the value of the set delay
                        if (!airlockOpenDoorDelays.ContainsKey(door))
                        {
                            airlockOpenDoorDelays.Add(door, AirlockAutoClosingDelay);
                            continue;
                        }

                        // if the ticks left open are larger than 0,
                        // wait until the ticks are reached
                        if (airlockOpenDoorDelays[door] > 0)
                        {
                            continue;
                        }

                        // when ticks are reached, close the door shut
                        airlockOpenDoorDelays.Remove(door);
                        door.CloseDoor();
                    }

                    // Only disable closed doors
                    if (door.Status == DoorStatus.Closed)
                    {
                        // if all open doors are save, the airlock is set up to be connected to a room and the pressure level in that room is safe,
                        // it does not need to be disabled.
                        bool otherSideIsSafe = airlockDoorToRoom.ContainsKey(door) ? getPressureLevelForRoom(airlockDoorToRoom[door]) >= MIN_PRESSURE_LEVEL : envOutsideAirPressure >= MIN_PRESSURE_LEVEL;
                        if (openDoorsInGroupAreSafe && otherSideIsSafe)
                        {
                            door.Enabled = true;
                        }
                        else
                        {
                            door.Enabled = allDoorsClosed;
                        }
                    }

                }
            }
        }

        private void DisplayAirlocks()
        {
            if (!HasDisplaysForMode(DisplayMode.airlocks)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Airlocks");

            foreach (var groupName in airlockDoors.Keys)
            {
                text.Append("- ");
                text.Append(groupName);
                text.Append('\n');
            }

            WriteRenderedTextToScreens(DisplayMode.airlocks, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region hangars
        private Dictionary<string, List<IMyAirtightHangarDoor>> hangarDoors = new Dictionary<string, List<IMyAirtightHangarDoor>>();
        private Dictionary<string, List<IMyTextPanel>> hangarWarningDisplays = new Dictionary<string, List<IMyTextPanel>>();
        private Dictionary<string, List<IMyLightingBlock>> hangarWarningLights = new Dictionary<string, List<IMyLightingBlock>>();
        private Dictionary<string, DoorStatus> hangarStatus = new Dictionary<string, DoorStatus>();

        private void InitHangars()
        {
            hangarDoors.Clear();
            hangarStatus.Clear();
            hangarWarningDisplays.Clear();
            hangarWarningLights.Clear();

            List<IMyAirtightHangarDoor> doors = new List<IMyAirtightHangarDoor>();
            GridTerminalSystem.GetBlocksOfType<IMyAirtightHangarDoor>(doors, CurrentGridOnly);
            foreach (var door in doors)
            {
                var properties = GetKeyValuePairsFromString(door.CustomData);

                if (!properties.ContainsKey("hangar"))
                    continue;

                var roomName = properties["hangar"];

                bool isOpen = GetSetting("hangar_" + roomName, false);

                if (!hangarDoors.ContainsKey(roomName))
                {
                    hangarDoors.Add(roomName, new List<IMyAirtightHangarDoor>());
                    hangarStatus.Add(roomName, isOpen ? DoorStatus.Opening : DoorStatus.Closing);
                }

                hangarDoors[roomName].Add(door);
                if (isOpen)
                    door.OpenDoor();
                else
                    door.CloseDoor();
            }

            List<IMyTextPanel> hangarDisplays = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(hangarDisplays, CurrentGridOnly);
            foreach (var display in hangarDisplays)
            {
                var properties = GetKeyValuePairsFromString(display.CustomData);

                if (!properties.ContainsKey("hangar"))
                    continue;

                var roomName = properties["hangar"];

                if (!hangarWarningDisplays.ContainsKey(roomName))
                {
                    hangarWarningDisplays.Add(roomName, new List<IMyTextPanel>());
                }

                hangarWarningDisplays[roomName].Add(display);
            }

            List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(lights, CurrentGridOnly);
            foreach (var light in lights)
            {
                var properties = GetKeyValuePairsFromString(light.CustomData);

                if (!properties.ContainsKey("hangar"))
                    continue;

                var roomName = properties["hangar"];

                if (!hangarWarningLights.ContainsKey(roomName))
                {
                    hangarWarningLights.Add(roomName, new List<IMyLightingBlock>());
                }

                hangarWarningLights[roomName].Add(light);
                light.Enabled = false;
            }

        }

        private void SetHangarWarnLights(string roomName, bool enabled)
        {
            if (hangarWarningLights.ContainsKey(roomName))
            {
                foreach (var light in hangarWarningLights[roomName])
                {
                    light.Enabled = enabled;
                }
            }
        }

        private void SetHangarDisplays(string roomName, DoorStatus status)
        {
            if (hangarWarningDisplays.ContainsKey(roomName))
            {
                Color color = Color.White;
                string text = string.Empty;

                switch (status)
                {
                    case DoorStatus.Open:
                        color = COLOR_RED;
                        text = "Hangar open";
                        break;

                    case DoorStatus.Opening:
                        color = COLOR_ORANGE;
                        text = "Hangar opening";
                        break;

                    case DoorStatus.Closing:
                        color = COLOR_ORANGE;
                        text = "Hangar closing";
                        break;

                    case DoorStatus.Closed:
                        color = COLOR_GREEN;
                        text = "Hangar closed";
                        break;
                }

                foreach (var display in hangarWarningDisplays[roomName])
                {
                    display.FontColor = color;
                    display.FontSize = HANGAR_FONT_SIZE;
                    display.WriteText(text);
                }
            }
        }

        private void ChangeHangarState(string roomName, DoorStatus oldState, DoorStatus newState)
        {
            hangarStatus[roomName] = newState;
            SetHangarDisplays(roomName, newState);

            if (newState == DoorStatus.Closed)
            {
                SetHangarWarnLights(roomName, false);
            }
            else if (newState == DoorStatus.Open)
            {
                SetHangarWarnLights(roomName, false);
            }
            else if (newState == DoorStatus.Closing)
            {
                SetHangarWarnLights(roomName, true);
            }
            else if (newState == DoorStatus.Opening)
            {
                SetHangarWarnLights(roomName, true);
            }
        }

        private void UpdateHangars()
        {
            foreach (var roomName in hangarDoors.Keys)
            {
                switch (hangarStatus[roomName])
                {
                    case DoorStatus.Closing:

                        // First, check if all doors are closed
                        bool allDoorsClosed = true;
                        foreach (var door in hangarDoors[roomName])
                        {
                            if (door.Status != DoorStatus.Closed)
                            {
                                door.CloseDoor();
                                allDoorsClosed = false;
                            }
                        }

                        if (!allDoorsClosed)
                            break;

                        // Next check if the room is pressurized
                        // (only if the room has vents)
                        bool roomIsPressurized = true;
                        if (roomVents.ContainsKey(roomName))
                        {
                            foreach (var vent in roomVents[roomName])
                            {
                                // start pressurization
                                if (vent.Depressurize)
                                {
                                    vent.Depressurize = false;
                                }

                                if (vent.GetOxygenLevel() != 1f)
                                {
                                    roomIsPressurized = false;
                                }

                            }
                        }

                        if (!roomIsPressurized)
                            break;

                        // if doors are closed and room is pressurized, we change to close state
                        ChangeHangarState(roomName, hangarStatus[roomName], DoorStatus.Closed);

                        break;

                    case DoorStatus.Closed:
                        // idle
                        break;

                    case DoorStatus.Opening:

                        // Next check if the room is pressurized
                        // (only if the room has vents)
                        bool roomIsDepressurized = true;
                        if (roomVents.ContainsKey(roomName))
                        {
                            foreach (var vent in roomVents[roomName])
                            {
                                // start depressurization
                                if (!vent.Depressurize)
                                {
                                    vent.Depressurize = true;
                                }

                                if (vent.GetOxygenLevel() != 0)
                                {
                                    roomIsDepressurized = false;
                                }

                            }

                            // check if oxygen tanks are 100% full, if so skip complete depressurization
                            if (oxygenAvailableFillPercentage == 1)
                            {
                                roomIsDepressurized = true;
                            }
                        }

                        if (!roomIsDepressurized)
                            break;

                        bool allDoorsOpen = true;
                        foreach (var door in hangarDoors[roomName])
                        {
                            if (door.Status != DoorStatus.Open)
                            {
                                door.OpenDoor();
                                allDoorsOpen = false;
                            }
                        }

                        if (!allDoorsOpen)
                            break;

                        // Disable pressurize before changing to open state
                        // otherweise they would keep depressurizing while the hangar is open,
                        // which doesn't do anything except filling the oxygen tanks in atmospheres,
                        // but the constant depressurize fog animation can be annoying
                        // and the outside vent can fill the oxygen tanks within atmospheres too.
                        foreach (var vent in roomVents[roomName])
                            vent.Depressurize = false;

                        // if doors are open and room is depressurized, we change to open state
                        ChangeHangarState(roomName, hangarStatus[roomName], DoorStatus.Open);

                        break;

                    case DoorStatus.Open:
                        // nothing to do
                        break;
                }
            }
        }

        private void CommandHangar(string argument)
        {
            var argumentList = argument.Split(' ');

            if (argumentList.Length < 2) return;

            switch (argumentList[0])
            {
                case "open":
                    OpenCloseHangar(argumentList[1], true);
                    break;

                case "close":
                    OpenCloseHangar(argumentList[1], false);
                    break;
            }

        }

        private void OpenCloseHangar(string roomName, bool open = true)
        {
            if (!hangarDoors.ContainsKey(roomName)) return;
            ChangeHangarState(roomName, hangarStatus[roomName], open ? DoorStatus.Opening : DoorStatus.Closing);
            SetSetting("hangar_" + roomName, open);
        }

        private void DisplayHangars()
        {
            if (!HasDisplaysForMode(DisplayMode.hangars)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Hangars");

            foreach (var roomName in hangarDoors.Keys)
            {
                string status = hangarStatus[roomName].ToString();

                if (roomVents.ContainsKey(roomName))
                {
                    string oxygenLevel = roomVents[roomName][0].GetOxygenLevel().ToString(PercentageNumberFormat);
                    status += " (" + oxygenLevel + ")";
                }

                CreateTwoColumnText(text, roomName, status);
            }

            WriteRenderedTextToScreens(DisplayMode.hangars, text);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region autopilot
        private bool apEnabled = true;
        private bool apAutoOffEnabled = true;

        private bool apDampenersEnabled = false;
        private bool apShipIsControlled = false;
        private bool apAutoDockingEnabled = true;
        private bool apAutoOff = false;

        private const string apEnabledKey = "ap_enabled";
        private const string apAutoOffKey = "ap_auto_off";

        private void InitAutopilot()
        {
            apEnabled = GetSetting(apEnabledKey, true);
            apAutoOffEnabled = GetSetting(apAutoOffKey, true);
        }

        private void UpdateAutopilot()
        {
            if (mainShipController == null) return;

            apDampenersEnabled = mainShipController.DampenersOverride;
            apShipIsControlled = mainShipController.IsUnderControl;

            if (!apEnabled) return;

            if (envIsInPlanetReach && !apShipIsControlled)
            {
                mainShipController.DampenersOverride = true;
            }

            // auto on/off ships
            if (apAutoOffEnabled && IsShipDocked() && SelectedProgramMode == ProgramMode.Miner)
            {
                if (apAutoOff) // ship is "offline"
                {
                    if (apShipIsControlled)
                    {
                        AutpilotAutodockSetBatteriesAndThrusters(true);
                        apAutoOff = false;
                    }
                }
                else // ship is "online"
                {
                    if (!apShipIsControlled)
                    {
                        AutpilotAutodockSetBatteriesAndThrusters(false);
                        apAutoOff = true;
                    }
                }
            }
        }

        private bool IsShipDocked()
        {
            foreach (var connector in connectors)
                if (connector.Status == MyShipConnectorStatus.Connected)
                    return true;

            return false;
        }

        private void AutopilotDock()
        {
            bool hasConnectableConnector = false;

            foreach (var connector in connectors)
            {
                if (connector.Status == MyShipConnectorStatus.Connectable)
                {
                    connector.Connect();
                    hasConnectableConnector = true;
                }
            }

            if (hasConnectableConnector)
                AutpilotAutodockSetBatteriesAndThrusters(false);
        }

        private void AutopilotUndock()
        {
            AutpilotAutodockSetBatteriesAndThrusters(true);

            foreach (var connector in connectors)
                connector.Disconnect();
        }

        private void AutpilotAutodockSetBatteriesAndThrusters(bool on)
        {
            foreach (var battery in batteries)
                battery.ChargeMode = on ? ChargeMode.Auto : ChargeMode.Recharge;

            foreach (var dirThrustMap in thrusterDirections)
                foreach (var thruster in dirThrustMap.Value)
                    thruster.Enabled = on;
        }

        private const string enableString = "enable";
        private const string disableString = "disable";

        private void CommandAutopilot(string argument)
        {
            string[] arguments = argument.Split(' ');

            if (arguments.Length == 0)
                return;

            if (arguments[0] == enableString)
            {
                apEnabled = true;
                SetSetting(apEnabledKey, true);
            }
            else if (arguments[0] == disableString)
            {
                apEnabled = false;
                SetSetting(apEnabledKey, false);
            }
            else if (arguments[0] == "auto_off" && arguments.Length == 2)
            {
                if (arguments[1] == enableString)
                {
                    apAutoOffEnabled = true;
                    SetSetting(apAutoOffKey, true);
                }
                else if (arguments[1] == disableString)
                {
                    apAutoOffEnabled = false;
                    SetSetting(apAutoOffKey, false);
                    apAutoOff = false;
                }
            }
            else if (arguments[0] == "dock")
            {
                AutopilotDock();
            }
            else if (arguments[0] == "undock")
            {
                AutopilotUndock();
            }
            else
            {
                Echo("Unknown autopilot command");
            }

        }

        private void DisplayAutopilot()
        {
            const string yes = "yes";
            const string no = "no";

            if (!HasDisplaysForMode(DisplayMode.autopilot)) return;

            var text = GetStringBuilderForScreen();
            CreateHeader(text, "Autopilot");

            if (mainShipController == null)
            {
                text.Append('\n');
                CreateCenteredText(text, requiresMainShipController);
                text.Append('\n');
            }
            else
            {
                CreateTwoColumnText(text, "Enabled", apEnabled ? yes : no);
                text.Append('\n');
                CreateTwoColumnText(text, "Dampeners", apDampenersEnabled ? yes : no);
                CreateTwoColumnText(text, "Piloted", apShipIsControlled ? yes : no);
                text.Append('\n');

                if (SelectedProgramMode == ProgramMode.Miner)
                {
                    CreateTwoColumnText(text, "Auto Off", apAutoOffEnabled ? yes : no);
                    text.Append('\n');
                }

            }

            WriteRenderedTextToScreens(DisplayMode.autopilot, text);
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region connectors
        private List<IMyShipConnector> connectors = new List<IMyShipConnector>();
        private List<IMyLandingGear> landingGears = new List<IMyLandingGear>();

        private void InitConnectors()
        {
            connectors.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, CurrentGridOnly);
            GridTerminalSystem.GetBlocksOfType<IMyLandingGear>(landingGears, CurrentGridOnly);
        }

        private void UpdateConnectors()
        {
            // nothing to do?
        }

        private void DisplayConnectors()
        {
            if (!HasDisplaysForMode(DisplayMode.connectors)) return;

            var text = GetStringBuilderForScreen();

            CreateHeader(text, "Connectors");

            foreach (var connector in connectors)
            {
                text.Append(connector.CustomName);
                text.Append('\n');

                var status = connector.Status;
                string statusString = status.ToString();

                if (status == MyShipConnectorStatus.Connected)
                {
                    var otherConnector = connector.OtherConnector;
                    statusString = otherConnector.CubeGrid.CustomName;
                }

                CreateTwoColumnText(text, "", statusString);
            }

            WriteRenderedTextToScreens(DisplayMode.connectors, text);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region debug
        private void DisplayDebug()
        {
            if (!HasDisplaysForMode(DisplayMode.debug)) return;

            var textSurfaces = GetDisplaysForMode(DisplayMode.debug);

            foreach (var textSurface in textSurfaces)
            {
                var text = GetStringBuilderForScreen();

                CreateHeader(text, "Debug");
                CreateTwoColumnText(text, "Width", DisplayWidth.ToString());
                CreateTwoColumnText(text, "Height", DisplayHeight.ToString());

                CreateTwoColumnText(text, "FontSize", textSurface.FontSize.ToString());
                CreateTwoColumnText(text, "Padding", textSurface.TextPadding.ToString());

                textSurface.WriteText(text.ToString());
            }
        }
        #endregion

        #endregion