        const string VERSION = "v0.0.4";

        const string NO_TIME_MASK = "--:--";
        const string EMPTY_TIME_MASK = "[----------]";
        const string EMPTY_PERCENT_MASK = "?%";

        const string LCD_PREFIX = "(farm)";
        const string LCD_FONT = "Monospace";
        const float LCD_FONT_SIZE = 0.8f;

        readonly Color LCD_FONT_COLOR = Color.White;
        readonly Color LCD_BACKGROUND_COLOR = Color.Black;

        const int LCD_WIDTH = 32;
        const int MAX_CROPS_TO_SHOW = 4;
        const int MAX_STRINGBUILDER_LENGTH = 1024;

        const double HEALTH_OK_EPS = 99.95;
        const string PLOT_WATER_LEVEL_TEXT = "Water Level";
        const string PLOT_CROP_HEALTH_TEXT = "Crop Health";
        const string PLOT_CROP_WATER_LEVEL_TEXT = "High";
        const string PLOT_CROP_GROWTH_PROGRESS_TEXT = "Growth Progress";
        const string PLOT_CROP_GROW_TIME_TEXT = "Grow Time";

        const string REASON_LOW_WATER_TEXT = "LOW WATER";
        const string REASON_ENVIRONMENT_TEXT = "ENVIRONMENT";

        const string CROP_DATA_CORRUPTED_TEXT = "n/a";

        private readonly IMyCubeGrid currentGrid;

        List<IMyTerminalBlock> farmingPlotBlocks = new List<IMyTerminalBlock>();
        List<IMyFarmPlotLogic> farmingPlots = new List<IMyFarmPlotLogic>();

        List<IMyTextPanel> lcds = new List<IMyTextPanel>();
        int foundLcdCount = 0;

        class CropAgg
        {
            public string Key;
            public string Name;
            public int Plots;
            public int ReadyPlots;
            public int GrowingPlots;
            public int ExpectedHarvest;
            public bool HasYoungest;
            public double YoungestProgress;
            public TimeSpan YoungestElapsed;
        }

        public Program()
        {
            currentGrid = Me.CubeGrid;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            FindLCDs();

            if (IsLcdCountChanged(foundLcdCount))
            {
                foundLcdCount = lcds.Count();
                InitLCDs();
            }

            if (foundLcdCount == 0)
            {
                Echo($"Add lcd monitor and {LCD_PREFIX} to its name");

                return;
            }

            ScanFarmingPlots();

            var sb = new StringBuilder(MAX_STRINGBUILDER_LENGTH);

            sb.AppendLine($"FARM MANAGER {VERSION}");

            string gridName = currentGrid.CustomName ?? "";
            if (gridName.Length > LCD_WIDTH - 2)
            {
                gridName = "(" + gridName.Substring(0, 30) + ")";
            }

            sb.AppendLine($"({gridName})");
            sb.AppendLine();

            if (farmingPlots.Count == 0)
            {
                sb.AppendLine("No farming plots found.");
                WriteToLCDs(FitToWidth(sb.ToString()));

                return;
            }

            int empty = 0;
            int notFunc = 0;
            int dead = 0;
            int lowWater = 0;
            int affected = 0;

            bool reasonLowWater = false;
            bool reasonEnvironment = false;

            int total = farmingPlots.Count;

            var crops = new Dictionary<string, CropAgg>(StringComparer.Ordinal);

            for (int i = 0; i < farmingPlots.Count; i++)
            {
                var block = farmingPlotBlocks[i];
                var plot = farmingPlots[i];

                if (!block.IsFunctional)
                {
                    notFunc++;

                    continue;
                }

                string waterLevel;
                bool hasWaterInfo = TryGetWaterLevel(plot.GetDetailedInfoWithoutRequiredInput(), out waterLevel);
                bool isLowWater = hasWaterInfo && !IsWaterHigh(waterLevel);

                if (isLowWater)
                {
                    lowWater++;
                }

                if (!plot.IsPlantPlanted)
                {
                    empty++;

                    continue;
                }

                if (!plot.IsAlive)
                {
                    dead++;

                    continue;
                }

                double cropHealth;
                if (TryGetCropHealth(plot.GetDetailedInfoWithoutRequiredInput(), out cropHealth) && cropHealth < HEALTH_OK_EPS)
                {
                    affected++;

                    if (isLowWater)
                    {
                        reasonLowWater = true;
                    }
                    else
                    {
                        reasonEnvironment = true;
                    }

                    continue;
                }

                string cropKey = SafeToString(plot.OutputItem);
                if (string.IsNullOrWhiteSpace(cropKey))
                {
                    // planted but no output key, treat as unknown crop bucket
                    cropKey = "Unknown";
                }

                CropAgg agg;

                if (!crops.TryGetValue(cropKey, out agg))
                {
                    agg = new CropAgg();
                    agg.Key = cropKey;
                    agg.Name = ExtractCropName(cropKey);
                    crops.Add(cropKey, agg);
                }

                agg.Plots++;

                if (plot.IsAlive && plot.IsHarvestable)
                {
                    agg.ExpectedHarvest += plot.OutputItemAmount;
                }

                if (plot.IsHarvestable)
                {
                    agg.ReadyPlots++;
                }
                else
                {
                    agg.GrowingPlots++;

                    double progressPct;
                    TimeSpan growTime;

                    string detailed = plot.GetDetailedInfoWithoutRequiredInput();

                    if (TryParseProgressAndGrowTime(detailed, out progressPct, out growTime))
                    {
                        if (!agg.HasYoungest || growTime < agg.YoungestElapsed)
                        {
                            agg.HasYoungest = true;
                            agg.YoungestElapsed = growTime;
                            agg.YoungestProgress = progressPct;
                        }
                    }
                }
            }

            if (affected > 0)
            {
                SetLcdAlert(true);

                sb.Clear();

                sb.AppendLine(FitLine($"FARM MANAGER {VERSION}", LCD_WIDTH));
                sb.AppendLine(FitLine($"({gridName})", LCD_WIDTH));
                sb.AppendLine(FitLine("", LCD_WIDTH));

                sb.AppendLine(CenterLine("DANGER:", LCD_WIDTH));
                sb.AppendLine(CenterLine("PLANTS ARE DYING!", LCD_WIDTH));
                sb.AppendLine(FitLine("", LCD_WIDTH));

                sb.AppendLine(CenterLine($"AFFECTED - {affected} plots", LCD_WIDTH));

                var reasons = new List<string>(2);
                if (reasonLowWater)
                {
                    reasons.Add(REASON_LOW_WATER_TEXT);
                }

                if (reasonEnvironment)
                {
                    reasons.Add(REASON_ENVIRONMENT_TEXT);
                }

                string MultipleReasonsSuffix = reasons.Count > 1 
                    ? "S" 
                    : "";
                sb.AppendLine(CenterLine($"REASON{MultipleReasonsSuffix}: " + string.Join(", ", reasons), LCD_WIDTH));

                WriteToLCDs(FitToWidth(sb.ToString()));

                return;
            }

            SetLcdAlert(false);

            sb.AppendLine($"Plots: {total}");

            var parts = new List<string>(4);

            if (empty > 0) parts.Add($"{empty} - EMPTY");
            if (notFunc > 0) parts.Add($"{notFunc} - NOT FUNC");
            if (dead > 0) parts.Add($"{dead} - DEAD");
            if (lowWater > 0) parts.Add($"{lowWater} - LOW WATER");

            for (int i = 0; i < parts.Count; i += 2)
            {
                // maximum two statuses per line
                if (i + 1 < parts.Count)
                {
                    sb.AppendLine(parts[i] + ", " + parts[i + 1]);
                }
                else
                {
                    sb.AppendLine(parts[i]);
                }
            }

            sb.AppendLine();

            // Show top crops (by planted plots desc, then name)
            var cropList = crops.Values.ToList();

            cropList.Sort((a, b) =>
            {
                int cmp = b.Plots.CompareTo(a.Plots);

                if (cmp != 0)
                {
                    return cmp;
                }

                return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
            });

            int shown = 0;
            for (int i = 0; i < cropList.Count && shown < MAX_CROPS_TO_SHOW; i++)
            {
                var c = cropList[i];

                sb.AppendLine(FitLine($"{c.Name} (plots:{c.Plots})", LCD_WIDTH));

                if (c.GrowingPlots == 0 && c.ReadyPlots > 0)
                {
                    sb.AppendLine("(ready to harvest)");
                }
                else
                {
                    if (!c.HasYoungest)
                    {
                        sb.AppendLine(FitLine($"{EMPTY_TIME_MASK} {EMPTY_PERCENT_MASK} ({NO_TIME_MASK})", LCD_WIDTH));
                    }
                    else
                    {
                        double p = c.YoungestProgress;
                        var elapsed = c.YoungestElapsed;

                        double remainingSec = 0.0;

                        if (p > 0.1 && elapsed.TotalSeconds > 0.0)
                        {
                            double secPerPercentRaw = elapsed.TotalSeconds / p;
                            int secPerPercent = (int)Math.Round(secPerPercentRaw);

                            int totalSec = secPerPercent * 100;

                            int rem = totalSec - (int)elapsed.TotalSeconds;
                            if (rem < 0) rem = 0;
                            if (rem > 24 * 3600) rem = 24 * 3600;

                            remainingSec = rem;
                        }
                        int pctInt = ClampInt((int)Math.Floor(p + 0.0001), 0, 99);
                        var bar = BuildBar10(pctInt);
                        string mmss = (p > 0.1)
                            ? FormatMinutesSeconds(TimeSpan.FromSeconds(remainingSec))
                            : NO_TIME_MASK;

                        sb.AppendLine(FitLine($"{bar} {pctInt,2}% ({mmss})", LCD_WIDTH));
                    }
                }

                shown++;
            }

            // If there are more crops than shown, indicate that
            int hidden = cropList.Count - shown;
            if (hidden > 0)
            {
                sb.AppendLine(FitLine($"Other crops: {hidden}", LCD_WIDTH));
            }

            if (cropList.Count() > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Harvest:");
            }

            cropList.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            for (int i = 0; i < cropList.Count; i++)
            {
                var c = cropList[i];

                sb.AppendLine(FitLine($"{c.Name}: {c.ExpectedHarvest}", LCD_WIDTH));
            }

            WriteToLCDs(FitToWidth(sb.ToString()));
        }

        void ScanFarmingPlots()
        {
            farmingPlots.Clear();
            farmingPlotBlocks.Clear();

            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, b => b.CubeGrid == currentGrid);

            foreach (var block in blocks)
            {
                var functional = block as IMyFunctionalBlock;

                if (functional == null)
                {
                    continue;
                }

                var plot = GetLogic(functional);

                if (plot == null)
                {
                    continue;
                }

                farmingPlotBlocks.Add(block);
                farmingPlots.Add(plot);
            }
        }

        // thanks for "The Klang Is Good" for that method
        IMyFarmPlotLogic GetLogic(IMyFunctionalBlock block)
        {
            IMyFarmPlotLogic logic;
            if (block.Components.TryGet(out logic))
                return logic;
            return null;
        }

        void FindLCDs()
        {
            lcds.Clear();

            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(
                lcds,
                p =>
                    p.CubeGrid == currentGrid &&
                    p.CustomName != null &&
                    p.CustomName.Contains(LCD_PREFIX)
            );
        }

        void InitLCDs()
        {
            foreach (var lcd in lcds)
            {
                lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                lcd.Font = LCD_FONT;
                lcd.FontSize = LCD_FONT_SIZE;
                lcd.FontColor = LCD_FONT_COLOR;
                lcd.BackgroundColor = LCD_BACKGROUND_COLOR;
                lcd.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
            }
        }

        void WriteToLCDs(string text)
        {
            if (lcds.Count == 0)
            {
                return;
            }

            foreach (var lcd in lcds)
            {
                lcd.WriteText(text, false);
            }
        }

        string BuildBar10(int progressPct)
        {
            int filled = ClampInt(progressPct / 10, 0, 9);

            return "[" + new string('#', filled) + new string('-', 10 - filled) + "]";
        }

        bool TryParseProgressAndGrowTime(string detailed, out double progressPct, out TimeSpan growTime)
        {
            progressPct = 0.0;
            growTime = TimeSpan.Zero;

            if (string.IsNullOrWhiteSpace(detailed))
            {
                return false;
            }

            var lines = detailed.Split('\n');
            bool gotP = false;
            bool gotT = false;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (!gotP && line.StartsWith($"{PLOT_CROP_GROWTH_PROGRESS_TEXT}:", StringComparison.OrdinalIgnoreCase))
                {
                    int idx = line.IndexOf(':');
                    if (idx >= 0)
                    {
                        var val = line.Substring(idx + 1).Trim();
                        val = val.Replace("%", "").Trim();

                        // Invariant parse: dot decimal
                        double p;
                        if (double.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out p))
                        {
                            progressPct = p;
                            gotP = true;
                        }
                    }
                }
                else if (!gotT && line.StartsWith($"{PLOT_CROP_GROW_TIME_TEXT}:", StringComparison.OrdinalIgnoreCase))
                {
                    int idx = line.IndexOf(':');

                    if (idx >= 0)
                    {
                        var val = line.Substring(idx + 1).Trim();

                        // Usually HH:MM:SS
                        TimeSpan t;
                        if (TimeSpan.TryParse(val, out t))
                        {
                            growTime = t;
                            gotT = true;
                        }
                    }
                }

                if (gotP && gotT)
                {
                    return true;
                }
            }

            return gotP && gotT;
        }

        string ExtractCropName(string outputItemKey)
        {
            // Example: MyObjectBuilder_ConsumableItem/Mushrooms -> Mushrooms
            if (string.IsNullOrWhiteSpace(outputItemKey))
            {
                return "Unknown";
            }

            int slash = outputItemKey.LastIndexOf('/');

            if (slash >= 0 && slash + 1 < outputItemKey.Length)
            {
                return outputItemKey.Substring(slash + 1);
            }

            return outputItemKey;
        }

        string SafeToString(object o)
        {
            if (o == null)
            {
                return "";
            }

            try
            {
                return o.ToString();
            }
            catch
            {
                return "";
            }
        }

        string FormatMinutesSeconds(TimeSpan t)
        {
            // total minutes:seconds (minutes can exceed 59)
            int totalMinutes = (int)Math.Floor(t.TotalMinutes);
            int seconds = t.Seconds;

            return totalMinutes.ToString("00") + ":" + seconds.ToString("00");
        }

        int ClampInt(int v, int min, int max)
        {
            if (v < min) return min;
            if (v > max) return max;

            return v;
        }

        string FitLine(string s, int width)
        {
            if (s == null) s = "";
            if (s.Length == width) return s;
            if (s.Length > width) return s.Substring(0, width);

            return s.PadRight(width, ' ');
        }

        string FitToWidth(string text)
        {
            // Ensure every line is <= LCD_WIDTH
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            var lines = text.Split('\n');
            var sb = new StringBuilder(text.Length + 64);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Replace("\r", "");
                sb.AppendLine(FitLine(line, LCD_WIDTH));
            }

            return sb.ToString();
        }

        bool IsLcdCountChanged(int foundLcdCount)
        {
            return lcds.Count() != foundLcdCount;
        }

        bool TryGetWaterLevel(string detailed, out string waterLevel)
        {
            waterLevel = null;

            if (string.IsNullOrWhiteSpace(detailed))
            {
                return false;
            }

            var lines = detailed.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith($"{PLOT_WATER_LEVEL_TEXT}:", StringComparison.OrdinalIgnoreCase))
                {
                    waterLevel = line.Substring($"{PLOT_WATER_LEVEL_TEXT}:".Length).Trim();

                    return true;
                }
            }

            return false;
        }

        bool IsWaterHigh(string waterLevel)
        {
            return !string.IsNullOrWhiteSpace(waterLevel) &&
                   waterLevel.Equals($"{PLOT_CROP_WATER_LEVEL_TEXT}", StringComparison.OrdinalIgnoreCase);
        }

        bool TryGetCropHealth(string detailed, out double healthPct)
        {
            healthPct = 100.0;

            if (string.IsNullOrWhiteSpace(detailed))
            {
                return false;
            }

            var lines = detailed.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith($"{PLOT_CROP_HEALTH_TEXT}:", StringComparison.OrdinalIgnoreCase))
                {
                    var value = line.Substring($"{PLOT_CROP_HEALTH_TEXT}:".Length).Trim();

                    if (value.EndsWith("%"))
                    {
                        value = value.Substring(0, value.Length - 1).Trim();
                    }

                    return double.TryParse(
                        value,
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out healthPct
                    );
                }
            }

            return false;
        }

        void SetLcdAlert(bool isAlert)
        {
            var color = isAlert 
                ? Color.Red 
                : LCD_FONT_COLOR;

            foreach (var lcd in lcds)
            {
                lcd.FontColor = color;
            }
        }

        string CenterLine(string s, int width)
        {
            if (s == null)
            {
                s = "";
            }
            
            if (s.Length >= width)
            {
                return s.Substring(0, width);
            }

            int leftPad = (width - s.Length) / 2;
            
            return new string(' ', leftPad) + s;
        }
