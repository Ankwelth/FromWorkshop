private const string REFINERY_PREFIX = "Refinery - PM Pro - ";
        private const string LCD_PREFIX = "LCD Refinery - PM Pro - ";
        private const string ORE_PREFIX = "Cargo Ore - PM Pro - ";
        private const string INGOT_PREFIX = "Cargo Ingot - PM Pro - ";

        private const string ASSEMBLER_PREFIX = "Assembler - PM Pro - ";
        private const string COMPONENT_PREFIX = "Cargo Component - PM Pro - ";

        private const string MENU_LCD_NAME = "LCD - Menu - PM Pro";

        // --- SPEZIFISCHE LCDs ---
        private const string ORE_LCD_NAME = "LCD Ore - PM Pro";
        private const string INGOT_LCD_NAME = "LCD Ingot - PM Pro";

        // --- ÜBERSICHTS LCD TYPEN ---
        private const string REF_INFO_LCD_NAME = "LCD Refineries Info - PM Pro";
        private const string ASM_INFO_LCD_NAME = "LCD Assembler Info - PM Pro";

        // --- KATEGORIE LCDs ---
        private const string COMP_LCD_NAME = "Components LCD - PM Pro";
        private const string EQUIP_LCD_NAME = "Equipment LCD - PM Pro";
        private const string TOOL_LCD_NAME = "Tools LCD - PM Pro";
        private const string WEAPON_LCD_NAME = "Weapons LCD - PM Pro";
        private const string AMMO_LCD_NAME = "Ammo LCD - PM Pro";

        private IMyTextPanel _menuLcd;

        // Listen für Multi-LCD Support
        private List<IMyTextPanel> _oreLcds = new List<IMyTextPanel>();
        private List<IMyTextPanel> _ingotLcds = new List<IMyTextPanel>();
        private List<IMyTextPanel> _refInfoLcds = new List<IMyTextPanel>();
        private List<IMyTextPanel> _asmInfoLcds = new List<IMyTextPanel>();

        private List<IMyTextPanel> _compLcds = new List<IMyTextPanel>();
        private List<IMyTextPanel> _equipLcds = new List<IMyTextPanel>();
        private List<IMyTextPanel> _toolLcds = new List<IMyTextPanel>();
        private List<IMyTextPanel> _weaponLcds = new List<IMyTextPanel>();
        private List<IMyTextPanel> _ammoLcds = new List<IMyTextPanel>();

        private List<RefineryUnit> _refineries = new List<RefineryUnit>();
        private List<IMyCargoContainer> _oreCargos = new List<IMyCargoContainer>();
        private List<IMyCargoContainer> _ingotCargos = new List<IMyCargoContainer>();

        // Assembler Systeme
        private AssemblerManager _assemblerManager;
        private List<IMyCargoContainer> _componentCargos = new List<IMyCargoContainer>();

        private MenuHandler _menu;
        private MenuFolder _assemblerMenuFolder;

        private int _ticks = 0;
        private long _lcdPageTick = 0;

        // --- STATISTIK VARIABLEN ---
        private Dictionary<string, double> _lastIngotAmounts = new Dictionary<string, double>();
        private Dictionary<string, double> _ingotDeltas = new Dictionary<string, double>();

        private Dictionary<string, double> _lastOreAmounts = new Dictionary<string, double>();
        private Dictionary<string, double> _oreDeltas = new Dictionary<string, double>();

        private Dictionary<string, double> _lastItemAmounts = new Dictionary<string, double>();
        private Dictionary<string, double> _itemDeltas = new Dictionary<string, double>();

        private int _statsTickCounter = 0;
        private const int STATS_INTERVAL = 120;

        private Logger _logger;

        // --- ERROR HANDLING STATE ---
        private bool _hasNamingConflict = false;
        private string _conflictName = "";

        public Program()
        {
            var programBlock = GridTerminalSystem.GetBlockWithName(Me.CustomName) as IMyProgrammableBlock;
            _logger = new Logger(programBlock);
            _logger.Clear();
            _logger.LogTrace("Program initialized.");

            _assemblerManager = new AssemblerManager(_logger);

            Refresh();

            // Nur wenn kein Fehler aufgetreten ist, weiter laden
            if (!_hasNamingConflict)
            {
                LoadConfig();
                BuildMenu();
            }

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save()
        {
            if (!_hasNamingConflict)
            {
                SaveConfig();
            }
        }

        public void Main(string arg, UpdateType src)
        {
            if (!string.IsNullOrEmpty(arg)) _menu.HandleInput(arg.ToUpper());
            if ((src & UpdateType.Update1) != 0)
            {
                // Wenn ein Konflikt besteht, führen wir keine Logik aus, nur das Menu (Render)
                if (!_hasNamingConflict)
                {
                    if (++_ticks >= 20)
                    {
                        _ticks = 0;
                        RunLogic();
                        SaveConfig();
                    }

                    if (++_statsTickCounter >= STATS_INTERVAL)
                    {
                        _statsTickCounter = 0;
                        UpdateStatistics();
                    }
                }

                _lcdPageTick++;
                Render();
            }
        }

        private void Refresh()
        {
            // Reset Lists
            _refineries.Clear();
            _oreCargos.Clear();
            _ingotCargos.Clear();
            _componentCargos.Clear();

            _oreLcds.Clear();
            _ingotLcds.Clear();
            _refInfoLcds.Clear();
            _asmInfoLcds.Clear();
            _compLcds.Clear();
            _equipLcds.Clear();
            _toolLcds.Clear();
            _weaponLcds.Clear();
            _ammoLcds.Clear();

            // Reset Error State
            _hasNamingConflict = false;
            _conflictName = "";

            _menuLcd = GridTerminalSystem.GetBlockWithName(MENU_LCD_NAME) as IMyTextPanel;
            if (_menuLcd != null)
            {
                _menuLcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                // Standardfarbe wiederherstellen (falls vorher rot war)
                _menuLcd.FontColor = new Color(255, 255, 255);
            }

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(blocks);

            List<IMyAssembler> assemblers = new List<IMyAssembler>();

            // Set zur Erkennung von Duplikaten innerhalb relevanter Blöcke
            HashSet<string> uniqueNames = new HashSet<string>();

            foreach (var b in blocks)
            {
                // CHANGE: Hier wurde StartWith statt == eingesetzt für die LCDs, 
                // damit auch "Components LCD - PM Pro 2" als relevant erkannt wird.
                bool isRelevant = b.CustomName.StartsWith(REFINERY_PREFIX) ||
                                  b.CustomName.StartsWith(LCD_PREFIX) ||
                                  b.CustomName.StartsWith(ORE_PREFIX) ||
                                  b.CustomName.StartsWith(INGOT_PREFIX) ||
                                  b.CustomName.StartsWith(ASSEMBLER_PREFIX) ||
                                  b.CustomName.StartsWith(COMPONENT_PREFIX) ||
                                  b.CustomName.StartsWith(ORE_LCD_NAME) ||    // FIX: StartsWith
                                  b.CustomName.StartsWith(INGOT_LCD_NAME) ||  // FIX: StartsWith
                                  b.CustomName.StartsWith(REF_INFO_LCD_NAME) || // FIX: StartsWith
                                  b.CustomName.StartsWith(ASM_INFO_LCD_NAME) || // FIX: StartsWith
                                  b.CustomName.StartsWith(COMP_LCD_NAME) ||   // FIX: StartsWith
                                  b.CustomName.StartsWith(EQUIP_LCD_NAME) ||  // FIX: StartsWith
                                  b.CustomName.StartsWith(TOOL_LCD_NAME) ||   // FIX: StartsWith
                                  b.CustomName.StartsWith(WEAPON_LCD_NAME) || // FIX: StartsWith
                                  b.CustomName.StartsWith(AMMO_LCD_NAME);     // FIX: StartsWith

                if (!isRelevant) continue;

                // Duplikat-Check
                if (uniqueNames.Contains(b.CustomName))
                {
                    TriggerNamingConflict(b.CustomName);
                    return; // Sofort abbrechen
                }
                uniqueNames.Add(b.CustomName);

                // --- Zuordnung ---
                if (b.CustomName.StartsWith(REFINERY_PREFIX) && b is IMyRefinery)
                {
                    string suf = b.CustomName.Substring(REFINERY_PREFIX.Length);
                    var lcd = GridTerminalSystem.GetBlockWithName(LCD_PREFIX + suf) as IMyTextPanel;
                    if (lcd != null)
                        lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                    _refineries.Add(new RefineryUnit((IMyRefinery)b, suf, lcd));
                }
                else if (b.CustomName.StartsWith(ORE_PREFIX) && b is IMyCargoContainer) _oreCargos.Add((IMyCargoContainer)b);
                else if (b.CustomName.StartsWith(INGOT_PREFIX) && b is IMyCargoContainer) _ingotCargos.Add((IMyCargoContainer)b);
                else if (b.CustomName.StartsWith(COMPONENT_PREFIX) && b is IMyCargoContainer) _componentCargos.Add((IMyCargoContainer)b);
                else if (b.CustomName.StartsWith(ASSEMBLER_PREFIX) && b is IMyAssembler) assemblers.Add((IMyAssembler)b);

                if (b is IMyTextPanel)
                {
                    var lcd = (IMyTextPanel)b;
                    // Listen-Checks
                    if (CheckAndAddLcd(lcd, ORE_LCD_NAME, _oreLcds)) continue;
                    if (CheckAndAddLcd(lcd, INGOT_LCD_NAME, _ingotLcds)) continue;
                    if (CheckAndAddLcd(lcd, REF_INFO_LCD_NAME, _refInfoLcds)) continue;
                    if (CheckAndAddLcd(lcd, ASM_INFO_LCD_NAME, _asmInfoLcds)) continue;
                    if (CheckAndAddLcd(lcd, COMP_LCD_NAME, _compLcds)) continue;
                    if (CheckAndAddLcd(lcd, EQUIP_LCD_NAME, _equipLcds)) continue;
                    if (CheckAndAddLcd(lcd, TOOL_LCD_NAME, _toolLcds)) continue;
                    if (CheckAndAddLcd(lcd, WEAPON_LCD_NAME, _weaponLcds)) continue;
                    if (CheckAndAddLcd(lcd, AMMO_LCD_NAME, _ammoLcds)) continue;
                }
            }

            _refineries.Sort(RefineryUnit.CompareBySuffix);

            _componentCargos.Sort((a, b) =>
            {
                string suffixA = a.CustomName.Substring(COMPONENT_PREFIX.Length);
                string suffixB = b.CustomName.Substring(COMPONENT_PREFIX.Length);
                int x, y;
                if (int.TryParse(suffixA, out x) && int.TryParse(suffixB, out y)) return x.CompareTo(y);
                return string.Compare(suffixA, suffixB, StringComparison.OrdinalIgnoreCase);
            });

            _assemblerManager.Setup(assemblers, _componentCargos);
            _assemblerManager.SetIngotSources(_ingotCargos);

            UpdateStatistics(true);
        }

        private void TriggerNamingConflict(string name)
        {
            _hasNamingConflict = true;
            _conflictName = name;
            _logger.LogError("Naming Conflict detected", name);
            BuildErrorMenu();
        }

        private void BuildErrorMenu()
        {
            // LCD Rot färben
            if (_menuLcd != null)
            {
                _menuLcd.FontColor = Color.Red;
            }

            MenuFolder root = new MenuFolder("ERROR");
            root.HeaderFunc = () => $"!!! CRITICAL ERROR !!!\nNaming Conflict Detected!\n\nBlock Name:\n'{_conflictName}'\n\nExisting name found twice.\nPlease fix and Retry.";

            root.Add(new MenuAction("Retry", () =>
            {
                Refresh();
                if (!_hasNamingConflict)
                {
                    // Wenn Fehler behoben, Normalzustand laden
                    LoadConfig();
                    BuildMenu();
                    _logger.LogTrace("Retry successful. System restarted.");
                }
            }));

            _menu = new MenuHandler(root);
        }

        private bool CheckAndAddLcd(IMyTextPanel lcd, string prefix, List<IMyTextPanel> list)
        {
            if (lcd.CustomName.StartsWith(prefix))
            {
                lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                list.Add(lcd);
                return true;
            }
            return false;
        }

        private void RunLogic()
        {
            foreach (var rf in _refineries) rf.ProcessTick(_oreCargos, _ingotCargos);
            _assemblerManager.RunLogic();
        }

        private void UpdateStatistics(bool firstRun = false)
        {
            double timeFactor = 30.0;

            foreach (var kvp in TypeHelper.IngotTypes)
            {
                string subtypeId = kvp.Value;
                double currentAmount = GetSystemAmount("MyObjectBuilder_Ingot", subtypeId, _ingotCargos);

                if (!firstRun && _lastIngotAmounts.ContainsKey(subtypeId))
                {
                    double lastAmount = _lastIngotAmounts[subtypeId];
                    _ingotDeltas[subtypeId] = (currentAmount - lastAmount) * timeFactor;
                }
                else _ingotDeltas[subtypeId] = 0;
                _lastIngotAmounts[subtypeId] = currentAmount;
            }

            foreach (var oreType in Enum.GetValues(typeof(RefineryUnit.Ore)).Cast<RefineryUnit.Ore>())
            {
                string oreName = oreType.ToString();
                double currentAmount = GetSystemAmount("MyObjectBuilder_Ore", oreName, _oreCargos);

                if (!firstRun && _lastOreAmounts.ContainsKey(oreName))
                {
                    double lastAmount = _lastOreAmounts[oreName];
                    _oreDeltas[oreName] = (currentAmount - lastAmount) * timeFactor;
                }
                else _oreDeltas[oreName] = 0;
                _lastOreAmounts[oreName] = currentAmount;
            }

            foreach (var bpName in TypeHelper.AssemblerBlueprints)
            {
                double currentAmount = (double)_assemblerManager.GetCurrentAmount(bpName);

                if (!firstRun && _lastItemAmounts.ContainsKey(bpName))
                {
                    double lastAmount = _lastItemAmounts[bpName];
                    _itemDeltas[bpName] = (currentAmount - lastAmount) * timeFactor;
                }
                else _itemDeltas[bpName] = 0;
                _lastItemAmounts[bpName] = currentAmount;
            }
        }

        private void Render()
        {
            // Immer das Menü rendern (auch im Fehlerfall)
            if (_menuLcd != null) _menuLcd.WriteText(_menu.Render());

            // Wenn wir einen Konflikt haben, rendern wir den Rest nicht, 
            // da die Listen unvollständig oder fehlerhaft sein könnten.
            if (_hasNamingConflict) return;

            foreach (var rf in _refineries) rf.UpdateLCD();

            RenderGlobalOreLcds();
            RenderGlobalIngotLcds();

            RenderRefineryInfoLcds();
            RenderAssemblerInfoLcds();

            RenderCategoryLcds(_compLcds, "COMPONENTS", TypeHelper.CategoryComponents);
            RenderCategoryLcds(_equipLcds, "EQUIPMENT", TypeHelper.CategoryEquipment);
            RenderCategoryLcds(_toolLcds, "TOOLS", TypeHelper.CategoryTools);
            RenderCategoryLcds(_weaponLcds, "WEAPONS", TypeHelper.CategoryWeapons);
            RenderCategoryLcds(_ammoLcds, "AMMO", TypeHelper.CategoryAmmo);
        }

        private void RenderAssemblerInfoLcds()
        {
            if (_asmInfoLcds.Count == 0) return;

            List<string> lines = new List<string>();

            var sortedBps = TypeHelper.AssemblerBlueprints
                .Where(bp => _assemblerManager.GetTarget(bp) > _assemblerManager.GetCurrentAmount(bp))
                .OrderByDescending(bp => _assemblerManager.GetTarget(bp) - _assemblerManager.GetCurrentAmount(bp))
                .ToList();

            foreach (var bp in sortedBps)
            {
                int current = _assemblerManager.GetCurrentAmount(bp);
                int target = _assemblerManager.GetTarget(bp);
                int planned = target - current;

                string name = FormatDisplayName(bp).Replace("Component", "C.");
                if (name.Length > 15) name = name.Substring(0, 15);

                string rateStr = "0/m";
                if (_itemDeltas.ContainsKey(bp))
                {
                    double delta = _itemDeltas[bp];
                    string sign = delta >= 0 ? "+" : "";
                    rateStr = $"{sign}{delta:F1}/m";
                }

                bool isStalled = _assemblerManager.StalledBlueprints.Contains(bp);
                string status = isStalled ? "X" : "O";

                lines.Add($"{name}: {FormatAmount(planned)}  |  {rateStr}  |  {status}");
            }

            foreach (var lcd in _asmInfoLcds)
            {
                if (lcd == null) continue;

                StringBuilder sb = new StringBuilder();
                int linesPerPage = 13;
                int totalItems = lines.Count;
                int totalPages = (totalItems + linesPerPage - 1) / linesPerPage;
                if (totalPages == 0) totalPages = 1;

                int pageIndex = 0;
                string customData = lcd.CustomData;
                int forcedPage;
                if (!string.IsNullOrWhiteSpace(customData) && int.TryParse(customData.Trim(), out forcedPage))
                {
                    pageIndex = MathHelper.Clamp(forcedPage - 1, 0, totalPages - 1);
                }
                else
                {
                    pageIndex = (int)(_lcdPageTick / 240) % totalPages;
                }

                sb.AppendLine("### Assembler Info ###");
                sb.AppendLine($"Total: {_assemblerManager.Count}");
                sb.AppendLine("Planned | Producing | Status");
                sb.AppendLine(new string('-', 30));

                var pageItems = lines.Skip(pageIndex * linesPerPage).Take(linesPerPage);
                foreach (var line in pageItems)
                {
                    sb.AppendLine(line);
                }

                int itemsOnPage = pageItems.Count();
                if (itemsOnPage < linesPerPage && totalPages > 1)
                {
                    for (int i = 0; i < (linesPerPage - itemsOnPage); i++) sb.AppendLine("");
                }

                lcd.WriteText(sb.ToString());
            }
        }

        private void RenderRefineryInfoLcds()
        {
            if (_refInfoLcds.Count == 0) return;

            int countTotal = _refineries.Count;
            int countO = 0;
            int countX = 0;
            int countY = 0;

            List<string> lines = new List<string>();

            foreach (var rf in _refineries)
            {
                string targetPrio = rf.Prios.Count > 0 ? rf.Prios[0].ToString() : "None";
                string currentOre = "None";
                string statusChar = "X";

                var inv = rf.Block.GetInventory(0);
                if (inv.ItemCount > 0)
                {
                    var item = inv.GetItemAt(0);
                    if (item.HasValue)
                    {
                        currentOre = item.Value.Type.SubtypeId;
                        if (currentOre == targetPrio)
                        {
                            statusChar = "O";
                            countO++;
                        }
                        else
                        {
                            statusChar = "Y";
                            countY++;
                        }
                    }
                }
                else
                {
                    statusChar = "X";
                    countX++;
                }

                lines.Add($"Unit {rf.Suffix}  |  ({targetPrio})  |  {currentOre}  |  {statusChar}");
            }

            foreach (var lcd in _refInfoLcds)
            {
                if (lcd == null) continue;

                StringBuilder sb = new StringBuilder();
                int linesPerPage = 14;
                int totalItems = lines.Count;
                int totalPages = (totalItems + linesPerPage - 1) / linesPerPage;
                if (totalPages == 0) totalPages = 1;

                int pageIndex = 0;
                string customData = lcd.CustomData;
                int forcedPage;
                if (!string.IsNullOrWhiteSpace(customData) && int.TryParse(customData.Trim(), out forcedPage))
                {
                    pageIndex = MathHelper.Clamp(forcedPage - 1, 0, totalPages - 1);
                }
                else
                {
                    pageIndex = (int)(_lcdPageTick / 240) % totalPages;
                }

                sb.AppendLine("### REFINERIES INFO ###");
                sb.AppendLine($"Total {countTotal} / O {countO} / X {countX} / Y {countY}");
                sb.AppendLine($"Index | Priority | Current | Status");
                sb.AppendLine(new string('-', 30));

                var pageItems = lines.Skip(pageIndex * linesPerPage).Take(linesPerPage);
                foreach (var line in pageItems)
                {
                    sb.AppendLine(line);
                }

                int itemsOnPage = pageItems.Count();
                if (itemsOnPage < linesPerPage && totalPages > 1)
                {
                    for (int i = 0; i < (linesPerPage - itemsOnPage); i++) sb.AppendLine("");
                }

                if (totalPages > 1)
                {
                    sb.AppendLine(new string('-', 30));
                    sb.AppendLine($"Page {pageIndex + 1}/{totalPages}");
                }

                lcd.WriteText(sb.ToString());
            }
        }

        private void RenderCategoryLcds(List<IMyTextPanel> lcds, string title, List<string> blueprints)
        {
            if (lcds == null || lcds.Count == 0) return;

            string header = $"### {title} STORAGE ###";
            string subHeader = "Name | Current | Target";

            List<string> lines = new List<string>();

            var sortedBps = blueprints
                .OrderByDescending(b => _assemblerManager.GetTarget(b))
                .ThenBy(b => FormatDisplayName(b))
                .ToList();

            foreach (var bp in sortedBps)
            {
                int current = _assemblerManager.GetCurrentAmount(bp);
                int target = _assemblerManager.GetTarget(bp);

                if (current == 0 && target == 0) continue;

                string name = FormatDisplayName(bp);
                if (name.Length > 20) name = name.Substring(0, 20);

                string stats = $"{FormatAmount(current)}/{FormatAmount(target)}";

                if (_assemblerManager.StalledBlueprints.Contains(bp))
                {
                    stats += " [X]";
                }

                lines.Add($"{name}: {stats}");
            }

            int linesPerPage = 14;
            int totalItems = lines.Count;
            int totalPages = (totalItems + linesPerPage - 1) / linesPerPage;
            if (totalPages == 0) totalPages = 1;

            foreach (var lcd in lcds)
            {
                if (lcd == null) continue;

                StringBuilder sb = new StringBuilder();
                int pageIndex = 0;

                string customData = lcd.CustomData;
                int forcedPage;
                if (!string.IsNullOrWhiteSpace(customData) && int.TryParse(customData.Trim(), out forcedPage))
                {
                    pageIndex = MathHelper.Clamp(forcedPage - 1, 0, totalPages - 1);
                }
                else
                {
                    pageIndex = (int)(_lcdPageTick / 240) % totalPages;
                }

                sb.AppendLine(header);
                sb.AppendLine(subHeader);
                sb.AppendLine(new string('-', 32));

                var pageItems = lines.Skip(pageIndex * linesPerPage).Take(linesPerPage);
                foreach (var line in pageItems) sb.AppendLine(line);

                int itemsOnPage = pageItems.Count();
                if (itemsOnPage < linesPerPage && totalPages > 1)
                {
                    for (int i = 0; i < (linesPerPage - itemsOnPage); i++) sb.AppendLine("");
                }

                if (totalPages > 1)
                {
                    sb.AppendLine(new string('-', 32));
                    sb.AppendLine($"Page {pageIndex + 1}/{totalPages}");
                }

                lcd.WriteText(sb.ToString());
            }
        }

        private void RenderGlobalOreLcds()
        {
            if (_oreLcds.Count == 0) return;

            var sortedStats = _lastOreAmounts.OrderByDescending(x => x.Value).ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("### ORE STORAGE ###");
            sb.AppendLine("-------------------");

            foreach (var kvp in sortedStats)
            {
                string line = $"{kvp.Key}: {FormatAmount(kvp.Value)}";
                if (_oreDeltas.ContainsKey(kvp.Key))
                {
                    double delta = _oreDeltas[kvp.Key];
                    if (Math.Abs(delta) >= 1.0)
                    {
                        string sign = delta > 0 ? "+" : "";
                        line += $" ({sign}{FormatAmount(delta)}/min)";
                    }
                }
                sb.AppendLine(line);
            }

            string content = sb.ToString();
            foreach (var lcd in _oreLcds) lcd.WriteText(content);
        }

        private void RenderGlobalIngotLcds()
        {
            if (_ingotLcds.Count == 0) return;

            var sortedStats = _lastIngotAmounts.OrderByDescending(x => x.Value).ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("### INGOT STORAGE ###");
            sb.AppendLine("---------------------");

            foreach (var item in sortedStats)
            {
                string displayName = TypeHelper.IngotTypes.FirstOrDefault(x => x.Value == item.Key).Key;
                if (string.IsNullOrEmpty(displayName)) displayName = item.Key;

                string line = $"{displayName}: {FormatAmount(item.Value)}";
                if (_ingotDeltas.ContainsKey(item.Key))
                {
                    double delta = _ingotDeltas[item.Key];
                    if (Math.Abs(delta) >= 1.0)
                    {
                        string sign = delta > 0 ? "+" : "";
                        line += $" ({sign}{FormatAmount(delta)}/min)";
                    }
                }
                sb.AppendLine(line);
            }

            string content = sb.ToString();
            foreach (var lcd in _ingotLcds) lcd.WriteText(content);
        }

        private void BuildMenu()
        {
            MenuFolder root = new MenuFolder("MAIN MENU");
            root.HeaderFunc = () => "### Production Manager Pro ###";

            MenuFolder units = new MenuFolder("REFINERIES", root);
            foreach (var rf in _refineries)
            {
                RefineryUnit currentRf = rf;
                MenuFolder rfFolder = new MenuFolder($"Unit {currentRf.Suffix}", units);

                rfFolder.NameFunc = () =>
                {
                    string topPrio = "None";
                    if (rfFolder.Children.Count > 0)
                    {
                        topPrio = rfFolder.Children[0].Name;
                    }
                    return $"Unit {currentRf.Suffix} ({topPrio})";
                };

                List<MyInventoryItem> items = new List<MyInventoryItem>();
                currentRf.Block.GetInventory(0).GetItems(items);
                rfFolder.HeaderFunc = () => $"### EDIT PRIORITY UNIT: {currentRf.Suffix} ###\nStatus: {(items.Count != 0 ? "Active" : "Idle")}";

                foreach (var ore in currentRf.Prios)
                {
                    rfFolder.Add(new MenuAction(ore.ToString(), () => { _menu.MovingIdx = _menu.SelectedIdx; }));
                }
                units.Add(rfFolder);
            }
            root.Add(units);

            _assemblerMenuFolder = new MenuFolder("ASSEMBLER", root);
            _assemblerMenuFolder.HeaderFunc = () => "### ASSEMBLER CONFIG ###\nChoose Category:";
            RebuildAssemblerMenu();
            root.Add(_assemblerMenuFolder);

            BuildStatisticsMenu(root);

            root.Add(new MenuAction("SYSTEM REFRESH", () =>
            {
                SaveConfig();
                Refresh();
                if (!_hasNamingConflict)
                {
                    LoadConfig();
                    BuildMenu();
                    _logger.LogTrace("System refreshed & reloaded.");
                }
            }));

            _menu = new MenuHandler(root);
        }

        private void BuildStatisticsMenu(MenuFolder root)
        {
            MenuFolder statsRoot = new MenuFolder("STATISTICS", root);
            statsRoot.HeaderFunc = () => "### STATISTICS MENU ###";

            // 1. ORE
            MenuFolder oreStats = new MenuFolder("Ore", statsRoot) { IsStatMode = true };
            oreStats.HeaderFunc = () =>
            {
                float fill = GetFillLevel(_oreCargos);
                return $"### ORE STATISTICS ###\nNum. Containers: {_oreCargos.Count}\nFill: {fill:F1}%";
            };
            foreach (var oreType in Enum.GetValues(typeof(RefineryUnit.Ore)).Cast<RefineryUnit.Ore>())
            {
                string oreName = oreType.ToString();
                oreStats.Add(new StatMenuItem(() =>
                {
                    double amount = GetSystemAmount("MyObjectBuilder_Ore", oreName, _oreCargos);
                    return $"{oreName}: {FormatAmount(amount)}";
                }));
            }
            statsRoot.Add(oreStats);

            // 2. INGOTS
            MenuFolder ingotStats = new MenuFolder("Ingots", statsRoot) { IsStatMode = true };
            ingotStats.HeaderFunc = () =>
            {
                float fill = GetFillLevel(_ingotCargos);
                return $"### INGOT STATISTICS ###\nNum. Containers: {_ingotCargos.Count}\nFill: {fill:F1}%";
            };

            foreach (var kvp in TypeHelper.IngotTypes)
            {
                string displayName = kvp.Key;
                string subtypeId = kvp.Value;

                ingotStats.Add(new StatMenuItem(() =>
                {
                    double amount = GetSystemAmount("MyObjectBuilder_Ingot", subtypeId, _ingotCargos);
                    return $"{displayName}: {FormatAmount(amount)}";
                }));
            }
            statsRoot.Add(ingotStats);

            // 3. COMPONENTS & ITEMS
            AddStatsCategory(statsRoot, "Components", _componentCargos, TypeHelper.CategoryComponents);
            AddStatsCategory(statsRoot, "Equipment", _componentCargos, TypeHelper.CategoryEquipment);
            AddStatsCategory(statsRoot, "Tools", _componentCargos, TypeHelper.CategoryTools);
            AddStatsCategory(statsRoot, "Weapons", _componentCargos, TypeHelper.CategoryWeapons);
            AddStatsCategory(statsRoot, "Ammo", _componentCargos, TypeHelper.CategoryAmmo);

            // 4. HARDWARE
            MenuFolder hwStats = new MenuFolder("Hardware", statsRoot) { IsStatMode = true };
            hwStats.HeaderFunc = () => "### HARDWARE STATS ###";

            hwStats.Add(new StatMenuItem(() => $"Ore Containers: {_oreCargos.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Ingot Containers: {_ingotCargos.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Comp. Containers: {_componentCargos.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Refineries: {_refineries.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Refinery LCDs: {_refineries.Count(r => r.Lcd != null)}"));
            hwStats.Add(new StatMenuItem(() => $"Assemblers: {_assemblerManager.Count}"));

            hwStats.Add(new StatMenuItem(() => $"Ore LCDs: {_oreLcds.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Ingot LCDs: {_ingotLcds.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Ref. Info LCDs: {_refInfoLcds.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Asm. Info LCDs: {_asmInfoLcds.Count}"));

            hwStats.Add(new StatMenuItem(() => $"Comp LCDs: {_compLcds.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Equip LCDs: {_equipLcds.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Tool LCDs: {_toolLcds.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Wep LCDs: {_weaponLcds.Count}"));
            hwStats.Add(new StatMenuItem(() => $"Ammo LCDs: {_ammoLcds.Count}"));

            statsRoot.Add(hwStats);
            root.Add(statsRoot);
        }

        private void AddStatsCategory(MenuFolder parent, string title, List<IMyCargoContainer> cargos, List<string> blueprints)
        {
            MenuFolder folder = new MenuFolder(title, parent) { IsStatMode = true };
            folder.HeaderFunc = () =>
            {
                float fill = GetFillLevel(cargos);
                return $"### {title.ToUpper()} STATS ###\nFill: {fill:F1}%";
            };

            var sortedList = blueprints.OrderBy(x => FormatDisplayName(x)).ToList();

            foreach (var bpName in sortedList)
            {
                folder.Add(new StatMenuItem(() =>
                {
                    string displayName = FormatDisplayName(bpName);
                    int current = _assemblerManager.GetCurrentAmount(bpName);
                    int target = _assemblerManager.GetTarget(bpName);

                    string line = $"{displayName}: {FormatAmount(current)}/{FormatAmount(target)}";

                    if (_assemblerManager.StalledBlueprints.Contains(bpName))
                    {
                        line += " [X]";
                    }
                    return line;
                }));
            }
            parent.Add(folder);
        }

        // --- HELPER FUNCTIONS ---

        private string FormatDisplayName(string rawName)
        {
            if (rawName.StartsWith("Position"))
            {
                int idx = rawName.IndexOf('_');
                if (idx >= 0 && idx < rawName.Length - 1)
                    rawName = rawName.Substring(idx + 1);
            }
            rawName = rawName.Replace('_', ' ');

            StringBuilder pretty = new StringBuilder();
            for (int i = 0; i < rawName.Length; i++)
            {
                char c = rawName[i];
                if (i > 0 && char.IsUpper(c) && char.IsLower(rawName[i - 1]))
                {
                    pretty.Append(' ');
                }
                pretty.Append(c);
            }
            return pretty.ToString();
        }

        private double GetSystemAmount(string typeId, string subtypeId, List<IMyCargoContainer> containers)
        {
            double total = 0;
            MyItemType type = new MyItemType(typeId, subtypeId);

            total += InventoryHelper.CountTotalInCargos(type, containers);

            foreach (var rf in _refineries)
            {
                if (rf.Block == null || !rf.Block.IsFunctional) continue;
                total += (double)rf.Block.GetInventory(0).GetItemAmount(type);
                total += (double)rf.Block.GetInventory(1).GetItemAmount(type);
            }

            if (_assemblerManager != null)
            {
                total += _assemblerManager.CountItem(type);
            }

            return total;
        }

        private float GetFillLevel(List<IMyCargoContainer> containers)
        {
            if (containers.Count == 0) return 0f;
            float totalFill = 0;
            foreach (var c in containers)
            {
                totalFill += (float)c.GetInventory(0).VolumeFillFactor;
            }
            return (totalFill / containers.Count) * 100f;
        }

        private string FormatAmount(double amount)
        {
            if (Math.Abs(amount) >= 1000000) return $"{amount / 1000000:F1}M";
            if (Math.Abs(amount) >= 1000) return $"{amount / 1000:F1}k";
            return amount.ToString("F0");
        }

        private void RebuildAssemblerMenu()
        {
            if (_assemblerMenuFolder == null) return;
            _assemblerMenuFolder.Children.Clear();

            AddCategoryToMenu("COMPONENTS", TypeHelper.CategoryComponents);
            AddCategoryToMenu("EQUIPMENT", TypeHelper.CategoryEquipment);
            AddCategoryToMenu("TOOLS", TypeHelper.CategoryTools);
            AddCategoryToMenu("WEAPONS", TypeHelper.CategoryWeapons);
            AddCategoryToMenu("AMMO", TypeHelper.CategoryAmmo);
        }

        private void AddCategoryToMenu(string title, List<string> blueprints)
        {
            MenuFolder categoryFolder = new MenuFolder(title, _assemblerMenuFolder);
            categoryFolder.HeaderFunc = () => $"### {title} ###\nSorted by: Target Amount";

            var sortedBps = _assemblerManager.GetSortedBlueprintList(blueprints);

            foreach (var bpName in sortedBps)
            {
                var numInput = new MenuNumberInput(bpName,
                    () => _assemblerManager.GetTarget(bpName),
                    (val) =>
                    {
                        _assemblerManager.SetTarget(bpName, val);
                        RebuildAssemblerMenu();
                    },
                    (name) =>
                    {
                        int current = _assemblerManager.GetCurrentAmount(name);
                        int target = _assemblerManager.GetTarget(name);
                        string displayName = FormatDisplayName(name);

                        string cleanName = displayName.Length > 20
                            ? displayName.Substring(0, 20)
                            : displayName;

                        string status = $"{cleanName}: {current}/{target}";
                        if (_assemblerManager.StalledBlueprints.Contains(name)) status += " [X]";
                        return status;
                    }
                );
                categoryFolder.Add(numInput);
            }

            _assemblerMenuFolder.Add(categoryFolder);
        }

        private void SaveConfig()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var rf in _refineries)
            {
                MenuFolder unitsFolder = null;
                if (_menu != null && _menu.Root != null && _menu.Root.Children.Count > 0)
                {
                    var f = _menu.Root.Children.FirstOrDefault(c => c.Name == "REFINERIES") as MenuFolder;
                    if (f != null) unitsFolder = f;
                }

                if (unitsFolder != null)
                {
                    MenuFolder rfConfigFolder = null;
                    foreach (var child in unitsFolder.Children)
                    {
                        if (child.Name.StartsWith($"Unit {rf.Suffix}"))
                        {
                            rfConfigFolder = child as MenuFolder;
                            break;
                        }
                    }

                    if (rfConfigFolder != null)
                    {
                        var newPrios = new List<RefineryUnit.Ore>();
                        foreach (var child in rfConfigFolder.Children)
                        {
                            try { newPrios.Add((RefineryUnit.Ore)Enum.Parse(typeof(RefineryUnit.Ore), child.Name)); } catch { }
                        }
                        if (newPrios.Count > 0) rf.Prios = newPrios;
                    }
                }
                sb.Append($"{rf.Suffix}:{string.Join(",", rf.Prios)};");
            }

            sb.Append("|");
            sb.Append(_assemblerManager.GetSaveString());

            this.Storage = sb.ToString();
        }

        private void LoadConfig()
        {
            string data = this.Storage;

            if (string.IsNullOrEmpty(data))
            {
                _logger.LogWarning("LoadConfig: Speicher (Storage) ist leer.");
                return;
            }

            _logger.LogTrace($"LoadConfig: Lade {data.Length} Bytes Daten...");

            try
            {
                string[] parts = data.Split('|');

                if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
                {
                    string[] entries = parts[0].Split(';');
                    int rfLoaded = 0;
                    foreach (string e in entries)
                    {
                        if (string.IsNullOrWhiteSpace(e)) continue;

                        string[] p = e.Split(':');
                        string suffix = p[0];

                        RefineryUnit rf = _refineries.FirstOrDefault(r => r.Suffix == suffix);
                        if (rf != null && p.Length > 1)
                        {
                            var loadedPrios = p[1].Split(',')
                                .Select(o => (RefineryUnit.Ore)Enum.Parse(typeof(RefineryUnit.Ore), o))
                                .ToList();
                            if (loadedPrios.Count > 0)
                            {
                                rf.Prios = loadedPrios;
                                rfLoaded++;
                            }
                        }
                    }
                    _logger.LogTrace($"Refineries geladen: {rfLoaded}");
                }

                if (parts.Length > 1)
                {
                    _assemblerManager.Load(parts[1]);
                    RebuildAssemblerMenu();
                    _logger.LogTrace("Assembler Daten geladen.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadConfig Crash", ex.Message);
            }
        }
// --- ERWEITERTES MENÜ FRAMEWORK ---
public interface IMenuItem
{
    string Name { get; }
}

public interface IInteractiveElement : IMenuItem
{
    bool IsActive { get; }

    void Enter();
    void HandleInput(string cmd);
    string RenderDisplay();
}

public class AssemblerManager
{
    private List<IMyAssembler> _assemblers = new List<IMyAssembler>();
    private List<IMyCargoContainer> _componentCargos = new List<IMyCargoContainer>();
    private List<IMyCargoContainer> _ingotCargos = new List<IMyCargoContainer>(); // Referenz für Ingot Nachschub
    // Konfiguration und Status
    private Dictionary<string, int> _targets = new Dictionary<string, int>();
    private Dictionary<string, int> _currentCounts = new Dictionary<string, int>();
    private Dictionary<string, int> _queuedCounts = new Dictionary<string, int>();
    // Liste der aktuell blockierten Blueprints (für Statistik/LCDs)
    public HashSet<string> StalledBlueprints { get; private set; } = new HashSet<string>();

    private Logger _logger;
    public int Count => _assemblers.Count;

    public AssemblerManager(Logger logger)
    {
        _logger = logger;
        foreach (var bp in TypeHelper.AssemblerBlueprints)
        {
            _targets[bp] = 0;
            _currentCounts[bp] = 0;
            _queuedCounts[bp] = 0;
        }
    }

    // Setup erweitert um ingotCargos
    public void Setup(List<IMyAssembler> assemblers, List<IMyCargoContainer> componentCargos)
    {
        _assemblers = assemblers;
        _componentCargos = componentCargos;
    // Hinweis: Die IngotCargos müssen separat gesetzt oder übergeben werden, 
    // da sie in der ursprünglichen Signatur nicht drin waren. 
    // Ich füge unten eine Methode hinzu oder wir nutzen eine Property.
    }

    // Neue Methode, um die Ingot Container bekannt zu machen (wird im Program.cs aufgerufen)
    public void SetIngotSources(List<IMyCargoContainer> ingotCargos)
    {
        _ingotCargos = ingotCargos;
        foreach (var asm in _assemblers)
        {
            asm.Mode = MyAssemblerMode.Assembly;
            asm.CooperativeMode = true; // Koop Modus hilft bei manueller Verteilung, kann aber auch false sein hier
            // WICHTIG: Conveyor System aus, wir machen das selbst!
            asm.UseConveyorSystem = false;
        }
    }

    public void SetTarget(string name, int amount)
    {
        if (_targets.ContainsKey(name))
            _targets[name] = amount;
        else
            _targets.Add(name, amount);
    }

    public int GetTarget(string name)
    {
        return _targets.ContainsKey(name) ? _targets[name] : 0;
    }

    public int GetCurrentAmount(string name)
    {
        return _currentCounts.ContainsKey(name) ? _currentCounts[name] : 0;
    }

    public List<string> GetSortedBlueprintList(List<string> blueprintsToSort)
    {
        return blueprintsToSort.OrderByDescending(bp => GetTarget(bp)).ThenBy(bp => bp).ToList();
    }

    public double CountItem(MyItemType type)
    {
        double amount = 0;
        foreach (var asm in _assemblers)
        {
            if (asm == null || !asm.IsFunctional)
                continue;
            amount += (double)asm.GetInventory(0).GetItemAmount(type);
            amount += (double)asm.GetInventory(1).GetItemAmount(type);
        }

        return amount;
    }

    public void RunLogic()
    {
        if (_assemblers.Count == 0)
            return;
        // Stall-Liste zurücksetzen für neuen Zyklus
        StalledBlueprints.Clear();
        // 1. Stall Detection & Output leeren
        foreach (var asm in _assemblers)
        {
            if (!asm.IsFunctional)
                continue;
            // A. Stall Detection (Bevor wir leeren)
            CheckStalls(asm);
            // B. Output in Component Cargo verschieben
            InventoryHelper.TransferAll(asm.GetInventory(1), _componentCargos);
            // C. Queue aufräumen, wenn Assembler idlet
            if (asm.IsQueueEmpty && !asm.IsProducing)
            {
            // Nichts zu tun
            }
        }

        // 2. Ingots nachfüllen (Manuelle Logistik)
        ManageIngotInputs();
        // 3. Daten sammeln (Ist-Stand)
        ScanInventories();
        ScanQueues();
        // 4. Berechnung von Überfluss und Bedarf
        var productionQueue = new List<ProductionItem>();
        var pruningList = new Dictionary<string, int>();
        foreach (var kvp in _targets)
        {
            string bpName = kvp.Key;
            int target = kvp.Value;
            int inventoryAmount = _currentCounts[bpName];
            int queuedAmount = _queuedCounts.ContainsKey(bpName) ? _queuedCounts[bpName] : 0;
            int totalExisting = inventoryAmount + queuedAmount;
            if (totalExisting > target && queuedAmount > 0)
            {
                // Fall A: Zu viel in der Pipeline -> Löschen
                int surplus = totalExisting - target;
                int toRemove = Math.Min(surplus, queuedAmount);
                if (toRemove > 0)
                    pruningList[bpName] = toRemove;
            }
            else if (totalExisting < target)
            {
                // Fall B: Zu wenig -> Produzieren
                // WICHTIG: Wenn der Blueprint als "Stalled" markiert ist, 
                // produzieren wir ihn NICHT neu, um die Queue nicht zu verstopfen.
                if (!StalledBlueprints.Contains(bpName))
                {
                    productionQueue.Add(new ProductionItem { Blueprint = bpName, Missing = target - totalExisting, Target = target });
                }
            }
        }

        // 5. Aufräumen (Pruning)
        if (pruningList.Count > 0)
        {
            PruneQueues(pruningList);
        }

        // 6. Neue Aufträge verteilen
        if (productionQueue.Count > 0)
        {
            productionQueue = productionQueue.OrderByDescending(x => x.Target).ToList();
            DistributeProduction(productionQueue);
        }
    }

    // --- STALL DETECTION LOGIK ---
    private void CheckStalls(IMyAssembler asm)
    {
        List<MyProductionItem> queue = new List<MyProductionItem>();
        asm.GetQueue(queue);
        var outInv = asm.GetInventory(1);
        if (outInv.ItemCount == 0 && !asm.IsProducing)
        {
            if (queue.Count == 0)
                return;
            // Wenn Output leer ist, aber Queue Items hat, sind alle Items in der Queue gestallt
            foreach (var item in queue)
            {
                string stalledBp = item.BlueprintId.SubtypeName;
                if (!StalledBlueprints.Contains(stalledBp))
                {
                    StalledBlueprints.Add(stalledBp);
                // Optional: Logging
                // _logger.LogTrace($"Stall detected: {stalledBp} in {asm.CustomName}");
                }
            }
        }

        // Wir prüfen das erste Item, das fertig wurde
        var outputItem = outInv.GetItemAt(0);
        if (!outputItem.HasValue)
            return;
        string producedSubtype = outputItem.Value.Type.SubtypeId;
        if (queue.Count <= 1)
            return; // Wenn Queue leer oder nur 1 Item, kann nichts davor gestallt sein
        // Wir suchen, an welcher Stelle dieses Item in der Queue steht.
        // Da wir "Unique Queue" erzwingen, ist das eindeutig.
        int foundIndex = -1;
        for (int i = 0; i < queue.Count; i++)
        {
            string queueBpName = queue[i].BlueprintId.SubtypeName;
            // Mapping: Blueprint Name -> Item Name
            string resultItemSubtype = TypeHelper.GetSubtypeId(queueBpName);
            if (resultItemSubtype == producedSubtype)
            {
                foundIndex = i;
                break;
            }
        }

        // Wenn das produzierte Item an Index > 0 gefunden wurde, 
        // bedeutet das, alle Items von 0 bis foundIndex-1 konnten nicht produziert werden (Ressourcenmangel).
        if (foundIndex > 0)
        {
            for (int i = 0; i < foundIndex; i++)
            {
                string stalledBp = queue[i].BlueprintId.SubtypeName;
                if (!StalledBlueprints.Contains(stalledBp))
                {
                    StalledBlueprints.Add(stalledBp);
                // Optional: Logging
                // _logger.LogTrace($"Stall detected: {stalledBp} in {asm.CustomName}");
                }
            }
        }
    }

    // --- INGOT LOGISTIK ---
    private void ManageIngotInputs()
    {
        if (_ingotCargos.Count == 0)
            return;
        // Ziel: 1000 von jedem Ingot Typ
        double targetAmount = 1000.0;
        foreach (var asm in _assemblers)
        {
            if (!asm.IsFunctional)
                continue;
            IMyInventory inputInv = asm.GetInventory(0);
            if (inputInv.IsFull)
                continue;
            foreach (var kvp in TypeHelper.IngotTypes)
            {
                // kvp.Key = "Iron Ingot" (Display), kvp.Value = "Iron" (Subtype)
                string subtype = kvp.Value;
                MyItemType itemType = new MyItemType("MyObjectBuilder_Ingot", subtype);
                double currentAmount = (double)inputInv.GetItemAmount(itemType);
                double missing = targetAmount - currentAmount;
                if (missing > 10.0) // Kleine Toleranz, um Spam zu vermeiden
                {
                    // Versuche 'missing' Menge aus Ingot Cargos zu ziehen
                    InventoryHelper.PullItemAmount(inputInv, itemType, missing, _ingotCargos);
                }
            }
        }
    }

    private void ScanInventories()
    {
        var keys = _currentCounts.Keys.ToList();
        foreach (var k in keys)
            _currentCounts[k] = 0;
        foreach (var cargo in _componentCargos)
        {
            TallyInventory(cargo.GetInventory(0), keys);
        }

        foreach (var asm in _assemblers)
        {
            if (asm == null || !asm.IsFunctional)
                continue;
            TallyInventory(asm.GetInventory(0), keys);
            TallyInventory(asm.GetInventory(1), keys); // Auch Output mitzählen bevor es verschoben wird
        }
    }

    private void ScanQueues()
    {
        var keys = _targets.Keys.ToList();
        foreach (var k in keys)
            _queuedCounts[k] = 0;
        foreach (var asm in _assemblers)
        {
            if (asm == null || !asm.IsFunctional)
                continue;
            List<MyProductionItem> queue = new List<MyProductionItem>();
            asm.GetQueue(queue);
            foreach (var item in queue)
            {
                string bpName = item.BlueprintId.SubtypeName;
                if (_queuedCounts.ContainsKey(bpName))
                {
                    _queuedCounts[bpName] += (int)item.Amount;
                }
            }
        }
    }

    private void PruneQueues(Dictionary<string, int> toRemoveMap)
    {
        foreach (var asm in _assemblers)
        {
            if (toRemoveMap.Count == 0)
                break;
            if (asm == null || !asm.IsFunctional || asm.IsQueueEmpty)
                continue;
            List<MyProductionItem> queue = new List<MyProductionItem>();
            asm.GetQueue(queue);
            for (int i = queue.Count - 1; i >= 0; i--)
            {
                var item = queue[i];
                string bpName = item.BlueprintId.SubtypeName;
                if (toRemoveMap.ContainsKey(bpName))
                {
                    int amountNeededToRemove = toRemoveMap[bpName];
                    if (amountNeededToRemove <= 0)
                        continue;
                    int amountInStack = (int)item.Amount;
                    int removeAmount = Math.Min(amountInStack, amountNeededToRemove);
                    asm.RemoveQueueItem(i, (MyFixedPoint)removeAmount);
                    toRemoveMap[bpName] -= removeAmount;
                    if (toRemoveMap[bpName] <= 0)
                        toRemoveMap.Remove(bpName);
                    if (toRemoveMap.Count == 0)
                        break;
                }
            }
        }
    }

    private void TallyInventory(IMyInventory inv, List<string> trackedBlueprints)
    {
        List<MyInventoryItem> items = new List<MyInventoryItem>();
        inv.GetItems(items);
        if (items.Count == 0)
            return;
        foreach (var item in items)
        {
            string subType = item.Type.SubtypeId;
            foreach (var bpName in trackedBlueprints)
            {
                string targetSubtype = TypeHelper.GetSubtypeId(bpName);
                if (targetSubtype == subType)
                {
                    _currentCounts[bpName] += (int)item.Amount;
                }
            }
        }
    }

    private void DistributeProduction(List<ProductionItem> queue)
    {
        // Batch Size: Wie viele Items wir auf einmal in den Queue schieben.
        // Da wir Eindeutigkeit wollen, ist das hier effektiv das Limit pro Zyklus pro Item.
        int batchSize = 50;
        var availableAssemblers = new List<IMyAssembler>(_assemblers);
        foreach (var item in queue)
        {
            double remainingNeeded = item.Missing;
            foreach (var asm in availableAssemblers)
            {
                if (remainingNeeded <= 0)
                    break;
                List<MyProductionItem> q = new List<MyProductionItem>();
                asm.GetQueue(q);
                // 1. Queue Length Check (wie gehabt)
                if (q.Count > 3)
                    continue;
                // 2. WICHTIG: Unique Check
                // Wir prüfen, ob dieser Blueprint schon im Queue ist.
                bool alreadyInQueue = false;
                foreach (var qItem in q)
                {
                    if (qItem.BlueprintId.SubtypeName == item.Blueprint)
                    {
                        alreadyInQueue = true;
                        break;
                    }
                }

                if (alreadyInQueue)
                    continue; // Überspringen, wenn schon vorhanden
                MyDefinitionId bpId;
                if (MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + item.Blueprint, out bpId))
                {
                    double amountToAdd = Math.Min(remainingNeeded, batchSize);
                    asm.AddQueueItem(bpId, (MyFixedPoint)amountToAdd);
                    remainingNeeded -= amountToAdd;
                }
            }
        }
    }

    public string GetSaveString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var kvp in _targets)
        {
            if (kvp.Value > 0)
            {
                sb.Append($"{kvp.Key}={kvp.Value};");
            }
        }

        return sb.ToString();
    }

    public void Load(string data)
    {
        if (string.IsNullOrEmpty(data))
            return;
        string[] entries = data.Split(';');
        foreach (var e in entries)
        {
            string[] p = e.Split('=');
            if (p.Length == 2)
            {
                int val;
                if (int.TryParse(p[1], out val))
                {
                    if (_targets.ContainsKey(p[0]))
                        _targets[p[0]] = val;
                }
            }
        }
    }

    private struct ProductionItem
    {
        public string Blueprint;
        public int Missing;
        public int Target;
    }
}

public class RefineryUnit
{
    // 1. Das Enum lebt jetzt HIER DRIN (Nested Type)
    // Dadurch zwingen wir den Compiler, es mitzukopieren.
    public enum Ore
    {
        Iron,
        Nickel,
        Cobalt,
        Magnesium,
        Silicon,
        Silver,
        Gold,
        Platinum,
        Uranium,
        Stone
    }

    public IMyRefinery Block { get; private set; }
    public IMyTextPanel Lcd { get; private set; }
    public string Suffix { get; private set; }
    // WICHTIG: Die Verwendung muss angepasst werden (Ore ist jetzt hier lokal bekannt)
    public List<Ore> Prios { get; set; }

    // Statische Standard-Liste
    public static readonly List<Ore> DefaultPrios = Enum.GetValues(typeof(Ore)).Cast<Ore>().ToList();
    public RefineryUnit(IMyRefinery block, string suffix, IMyTextPanel lcd)
    {
        Block = block;
        Suffix = suffix;
        Lcd = lcd;
        Prios = new List<Ore>(DefaultPrios);
        Block.UseConveyorSystem = false;
    }

    public void ProcessTick(List<IMyCargoContainer> oreSources, List<IMyCargoContainer> ingotDestinations)
    {
        if (Block == null || !Block.IsWorking && !Block.IsFunctional)
            return;
        IMyInventory invIn = Block.GetInventory(0);
        IMyInventory invOut = Block.GetInventory(1);
        InventoryHelper.TransferAll(invOut, ingotDestinations);
        string targetSub = GetHighestPriorityAvailableOre(oreSources);
        if (!string.IsNullOrEmpty(targetSub))
        {
            CleanInputInventory(invIn, targetSub, oreSources);
            InventoryHelper.PullItem(invIn, new MyItemType("MyObjectBuilder_Ore", targetSub), oreSources);
        }
    }

    private string GetHighestPriorityAvailableOre(List<IMyCargoContainer> oreSources)
    {
        foreach (Ore p in Prios)
        {
            MyItemType type = new MyItemType("MyObjectBuilder_Ore", p.ToString());
            double systemTotal = InventoryHelper.CountTotalInCargos(type, oreSources);
            systemTotal += (double)Block.GetInventory(0).GetItemAmount(type);
            if (systemTotal > 0.01)
                return p.ToString();
        }

        return null;
    }

    private void CleanInputInventory(IMyInventory invIn, string targetSub, List<IMyCargoContainer> dumpTargets)
    {
        if (invIn.ItemCount == 0)
            return;
        for (int i = invIn.ItemCount - 1; i >= 0; i--)
        {
            var item = invIn.GetItemAt(i);
            if (item.HasValue && item.Value.Type.SubtypeId != targetSub)
            {
                InventoryHelper.TransferSingleStack(invIn, i, dumpTargets);
            }
        }
    }

    public void UpdateLCD()
    {
        if (Lcd == null)
            return;
        StringBuilder sb = new StringBuilder();
        IMyInventory inv = Block.GetInventory(0);
        List<MyInventoryItem> items = new List<MyInventoryItem>();
        inv.GetItems(items);
        sb.AppendLine($"### REFINERY: {Suffix} ###");
        sb.AppendLine($"Priority #1: {Prios[0]}");
        string cur = items.Count > 0 ? items[0].Type.SubtypeId : "None";
        int pLvl = Prios.FindIndex(o => o.ToString() == cur) + 1;
        sb.AppendLine($"Producing: {cur}");
        sb.AppendLine($"Current Priority: {(pLvl > 0 ? pLvl.ToString() : "N/A")}");
        sb.AppendLine($"\n--- Inventory ---");
        sb.AppendLine("--------------------------------");
        if (items.Count == 0)
        {
            sb.AppendLine("XXX Empty / No Ore Available XXX");
            Lcd.FontColor = Color.Red;
        }
        else
        {
            Lcd.FontColor = (pLvl == 1) ? Color.Green : Color.Yellow;
            foreach (var itm in items)
                sb.AppendLine($"{(float)itm.Amount:F2} {itm.Type.SubtypeId}");
        }

        Lcd.WriteText(sb.ToString());
    }

    public static int CompareBySuffix(RefineryUnit a, RefineryUnit b)
    {
        int x, y;
        if (int.TryParse(a.Suffix, out x) && int.TryParse(b.Suffix, out y))
            return x.CompareTo(y);
        return string.Compare(a.Suffix, b.Suffix, StringComparison.OrdinalIgnoreCase);
    }
}

public static class InventoryHelper
{
    public static void TransferAll(IMyInventory src, List<IMyCargoContainer> targets)
    {
        if (src.ItemCount == 0)
            return;
        for (int i = src.ItemCount - 1; i >= 0; i--)
        {
            TransferSingleStack(src, i, targets);
        }
    }

    public static void TransferSingleStack(IMyInventory src, int idx, List<IMyCargoContainer> targets)
    {
        foreach (var t in targets)
        {
            IMyInventory dst = t.GetInventory(0);
            if (dst.IsFull)
                continue;
            src.TransferItemTo(dst, idx, null, true);
            if (src.GetItemAt(idx) == null)
                break;
        }
    }

    public static void PullItem(IMyInventory dest, MyItemType type, List<IMyCargoContainer> sources)
    {
        if (dest.IsFull)
            return;
        foreach (var cargo in sources)
        {
            IMyInventory srcInv = cargo.GetInventory(0);
            var item = srcInv.FindItem(type);
            if (item.HasValue)
            {
                srcInv.TransferItemTo(dest, item.Value, null);
                if (dest.IsFull)
                    break;
            }
        }
    }

    // Neue Methode für präzises Auffüllen von Mengen
    public static void PullItemAmount(IMyInventory dest, MyItemType type, double amount, List<IMyCargoContainer> sources)
    {
        if (dest.IsFull || amount <= 0)
            return;
        foreach (var cargo in sources)
        {
            IMyInventory srcInv = cargo.GetInventory(0);
            var item = srcInv.FindItem(type);
            if (item.HasValue)
            {
                MyFixedPoint amountToTransfer = (MyFixedPoint)amount;
                MyFixedPoint available = item.Value.Amount;
                if (available < amountToTransfer)
                    amountToTransfer = available;
                if (amountToTransfer > 0)
                {
                    srcInv.TransferItemTo(dest, item.Value, amountToTransfer);
                    amount -= (double)amountToTransfer;
                }

                if (amount <= 0 || dest.IsFull)
                    break;
            }
        }
    }

    public static double CountTotalInCargos(MyItemType type, List<IMyCargoContainer> cargos)
    {
        return cargos.Sum(c => (double)c.GetInventory(0).GetItemAmount(type));
    }
}

public class Logger
{
    IMyTerminalBlock _logBlock;
    public Logger(IMyTerminalBlock logBlock)
    {
        _logBlock = logBlock;
    }

    public void LogTrace(string message)
    {
        string currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        if (_logBlock != null)
            _logBlock.CustomData = _logBlock.CustomData + $"{currentTimestamp}: {message} \n";
    }

    public void Clear()
    {
        if (_logBlock != null)
            _logBlock.CustomData = "";
    }

    public void LogWarning(string message)
    {
        if (_logBlock != null)
            _logBlock.CustomData = _logBlock.CustomData + $"Warning: {message} \n";
    }

    public void LogError(string message, string errorMessage)
    {
        string currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        if (_logBlock != null)
            _logBlock.CustomData = _logBlock.CustomData + $"{currentTimestamp} - Error: {message}. Details: {errorMessage} \n";
    }
}

public class MenuAction : IMenuItem
{
    private string _name;
    public string Name
    {
        get
        {
            return NameFunc != null ? NameFunc() : _name;
        }

        set
        {
            _name = value;
        }
    }

    public Func<string> NameFunc;
    public Action OnExecute;
    public MenuAction(string name, Action action)
    {
        Name = name;
        OnExecute = action;
    }
}

public class StatMenuItem : IMenuItem
{
    private Func<string> _textProvider;
    public string Name => _textProvider();

    public StatMenuItem(Func<string> textProvider)
    {
        _textProvider = textProvider;
    }
}

public class MenuFolder : IMenuItem
{
    private string _name;
    // Änderung: Der Name kann nun dynamisch via NameFunc abgerufen werden
    public string Name
    {
        get
        {
            return NameFunc != null ? NameFunc() : _name;
        }

        set
        {
            _name = value;
        }
    }

    // Optionale Funktion für dynamische Ordnernamen
    public Func<string> NameFunc;
    public List<IMenuItem> Children { get; set; }

    public MenuFolder Parent;
    public Func<string> HeaderFunc;
    public bool IsStatMode { get; set; } = false;

    public MenuFolder(string name, MenuFolder parent = null)
    {
        Name = name;
        Parent = parent;
        Children = new List<IMenuItem>();
    }

    public void Add(IMenuItem item)
    {
        Children.Add(item);
    }
}

public class MenuNumberInput : IInteractiveElement
{
    public string Name { get; private set; }

    private Func<int> _getter;
    private Action<int> _setter;
    private Func<string, string> _labelFormatter;
    public bool IsActive { get; private set; } = false;
    public bool IsEditing { get; private set; } = false;

    private int _cursorPos = 0;
    private int[] _digits = new int[6];
    public MenuNumberInput(string name, Func<int> getter, Action<int> setter, Func<string, string> labelFormatter = null)
    {
        Name = name;
        _getter = getter;
        _setter = setter;
        _labelFormatter = labelFormatter;
    }

    public void Enter()
    {
        IsActive = true;
        IsEditing = false;
        _cursorPos = 0;
        LoadValue();
    }

    private void LoadValue()
    {
        int val = _getter();
        if (val > 999999)
            val = 999999;
        string s = val.ToString("D6");
        for (int i = 0; i < 6; i++)
            _digits[i] = int.Parse(s.Substring(i, 1));
    }

    private void SaveValue()
    {
        int val = 0;
        int multiplier = 100000;
        for (int i = 0; i < 6; i++)
        {
            val += _digits[i] * multiplier;
            multiplier /= 10;
        }

        _setter(val);
    }

    public void HandleInput(string cmd)
    {
        switch (cmd)
        {
            case "SELECT":
                IsEditing = !IsEditing;
                break;
            case "BACK":
                if (IsEditing)
                    IsEditing = false;
                else
                {
                    SaveValue();
                    IsActive = false;
                }

                break;
            case "UP":
                if (IsEditing)
                {
                    _digits[_cursorPos]++;
                    if (_digits[_cursorPos] > 9)
                        _digits[_cursorPos] = 0;
                }
                else
                {
                    _cursorPos--;
                    if (_cursorPos < 0)
                        _cursorPos = 0;
                }

                break;
            case "DOWN":
                if (IsEditing)
                {
                    _digits[_cursorPos]--;
                    if (_digits[_cursorPos] < 0)
                        _digits[_cursorPos] = 9;
                }
                else
                {
                    _cursorPos++;
                    if (_cursorPos > 5)
                        _cursorPos = 5;
                }

                break;
        }
    }

    public string RenderDisplay()
    {
        if (!IsActive)
            return _labelFormatter != null ? _labelFormatter(Name) : $"{Name}: {_getter()}";
        StringBuilder sb = new StringBuilder();
        sb.Append("SET: ");
        for (int i = 0; i < 6; i++)
        {
            if (i == _cursorPos)
            {
                sb.Append(IsEditing ? "{" : "[");
                sb.Append(_digits[i]);
                sb.Append(IsEditing ? "}" : "]");
            }
            else
                sb.Append(_digits[i]);
        }

        return sb.ToString();
    }
}

public class MenuHandler
{
    public MenuFolder Root;
    public MenuFolder Current;
    public int SelectedIdx = 0;
    public int MovingIdx = -1;
    private const int PAGE_SIZE = 10;
    public MenuHandler(MenuFolder root)
    {
        Root = root;
        Current = root;
    }

    public void HandleInput(string cmd)
    {
        if (Current.Children.Count > SelectedIdx && !Current.IsStatMode)
        {
            var activeItem = Current.Children[SelectedIdx] as IInteractiveElement;
            if (activeItem != null && activeItem.IsActive)
            {
                activeItem.HandleInput(cmd);
                return;
            }
        }

        int count = Current.Children.Count;
        if (count == 0 && cmd != "BACK")
            return;
        switch (cmd)
        {
            case "UP":
                if (Current.IsStatMode)
                {
                    if (SelectedIdx == 0)
                    {
                        if (count > 0)
                            SelectedIdx = ((count - 1) / PAGE_SIZE) * PAGE_SIZE;
                    }
                    else
                    {
                        SelectedIdx = Math.Max(0, SelectedIdx - PAGE_SIZE);
                    }
                }
                else
                {
                    int nextUp = (SelectedIdx > 0) ? SelectedIdx - 1 : count - 1;
                    if (MovingIdx != -1)
                        Swap(SelectedIdx, nextUp);
                    SelectedIdx = nextUp;
                }

                break;
            case "DOWN":
                if (Current.IsStatMode)
                {
                    if (SelectedIdx + PAGE_SIZE >= count)
                    {
                        SelectedIdx = 0;
                    }
                    else
                    {
                        SelectedIdx += PAGE_SIZE;
                    }
                }
                else
                {
                    int nextDown = (SelectedIdx < count - 1) ? SelectedIdx + 1 : 0;
                    if (MovingIdx != -1)
                        Swap(SelectedIdx, nextDown);
                    SelectedIdx = nextDown;
                }

                break;
            case "SELECT":
                if (Current.IsStatMode)
                    return;
                if (MovingIdx != -1)
                {
                    MovingIdx = -1;
                }
                else
                {
                    IMenuItem item = Current.Children[SelectedIdx];
                    if (item is MenuFolder)
                    {
                        Current = (MenuFolder)item;
                        SelectedIdx = 0;
                    }
                    else if (item is IInteractiveElement)
                        ((IInteractiveElement)item).Enter();
                    else if (item is MenuAction)
                        ((MenuAction)item).OnExecute?.Invoke();
                }

                break;
            case "BACK":
                if (MovingIdx != -1)
                    MovingIdx = -1;
                else if (Current.Parent != null)
                {
                    MenuFolder old = Current;
                    Current = Current.Parent;
                    SelectedIdx = Current.Children.IndexOf(old);
                    if (SelectedIdx == -1)
                        SelectedIdx = 0;
                }

                break;
        }
    }

    private void Swap(int a, int b)
    {
        IMenuItem temp = Current.Children[a];
        Current.Children[a] = Current.Children[b];
        Current.Children[b] = temp;
    }

    public string Render()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(Current.HeaderFunc?.Invoke() ?? string.Format("### {0} ###", Current.Name));
        sb.AppendLine(new string ('-', 24));
        int totalItems = Current.Children.Count;
        int page = SelectedIdx / PAGE_SIZE;
        int totalPages = (totalItems - 1) / PAGE_SIZE + 1;
        if (totalPages == 0)
            totalPages = 1;
        int startIdx = page * PAGE_SIZE;
        int endIdx = Math.Min(startIdx + PAGE_SIZE, totalItems);
        for (int i = startIdx; i < endIdx; i++)
        {
            bool isSelected = (i == SelectedIdx);
            IMenuItem item = Current.Children[i];
            string nameDisplay = item.Name;
            if (item is IInteractiveElement)
            {
                nameDisplay = ((IInteractiveElement)item).RenderDisplay();
            }

            string ptr;
            if (Current.IsStatMode)
            {
                ptr = "   ";
            }
            else
            {
                ptr = isSelected ? (MovingIdx != -1 ? " >> " : "  > ") : "    ";
            }

            sb.AppendLine(ptr + nameDisplay + (isSelected && !Current.IsStatMode ? " <" : ""));
        }

        for (int k = 0; k < (PAGE_SIZE - (endIdx - startIdx)); k++)
        {
            sb.AppendLine("");
        }

        sb.AppendLine(new string ('-', 24));
        if (MovingIdx != -1)
        {
            sb.AppendLine("[*] SORTIER-MODUS");
        }
        else
        {
            if (totalItems > 0 && !Current.IsStatMode && Current.Children[SelectedIdx] is IInteractiveElement && ((IInteractiveElement)Current.Children[SelectedIdx]).IsActive)
            {
                sb.AppendLine("[SEL] Edit [BACK] Save");
            }
            else
            {
                sb.AppendLine(string.Format("Page {0} / {1}", page + 1, totalPages));
            }
        }

        return sb.ToString();
    }
}

public static class TypeHelper
{
    public static readonly string BlueprintTypePrefix = "MyObjectBuilder_BlueprintDefinition/";
    public static readonly Dictionary<string, string> BlueprintToItemMap = new Dictionary<string, string>
    {
        {
            "BulletproofGlass",
            "BulletproofGlass"
        },
        {
            "ComputerComponent",
            "Computer"
        },
        {
            "ConstructionComponent",
            "Construction"
        },
        {
            "DetectorComponent",
            "Detector"
        },
        {
            "Display",
            "Display"
        },
        {
            "EngineerPlushie",
            "EngineerPlushie"
        },
        {
            "ExplosivesComponent",
            "Explosives"
        },
        {
            "GirderComponent",
            "Girder"
        },
        {
            "GravityGeneratorComponent",
            "GravityGenerator"
        },
        {
            "InteriorPlate",
            "InteriorPlate"
        },
        {
            "LargeTube",
            "LargeTube"
        },
        {
            "MedicalComponent",
            "Medical"
        },
        {
            "MetalGrid",
            "MetalGrid"
        },
        {
            "MotorComponent",
            "Motor"
        },
        {
            "Position0030_Canvas",
            "Canvas"
        },
        {
            "PowerCell",
            "PowerCell"
        },
        {
            "RadioCommunicationComponent",
            "RadioCommunication"
        },
        {
            "ReactorComponent",
            "Reactor"
        },
        {
            "SabiroidPlushie",
            "SabiroidPlushie"
        },
        {
            "SmallTube",
            "SmallTube"
        },
        {
            "SolarCell",
            "SolarCell"
        },
        {
            "SteelPlate",
            "SteelPlate"
        },
        {
            "Superconductor",
            "Superconductor"
        },
        {
            "ThrustComponent",
            "Thrust"
        },
        {
            "ZoneChip",
            "ZoneChip"
        },
        // Tools Mapping
        {
            "Position0010_AngleGrinder",
            "AngleGrinderItem"
        },
        {
            "Position0010_OxygenBottle",
            "OxygenBottle"
        },
        {
            "Position0010_SemiAutoPistol",
            "SemiAutoPistolItem"
        },
        {
            "Position0020_AngleGrinder2",
            "AngleGrinder2Item"
        },
        {
            "Position0020_FullAutoPistol",
            "FullAutoPistolItem"
        },
        {
            "Position0020_HydrogenBottle",
            "HydrogenBottle"
        },
        {
            "Position0030_AngleGrinder3",
            "AngleGrinder3Item"
        },
        {
            "Position0030_EliteAutoPistol",
            "ElitePistolItem"
        },
        {
            "Position0040_AngleGrinder4",
            "AngleGrinder4Item"
        },
        {
            "Position0040_AutomaticRifle",
            "AutomaticRifleItem"
        },
        {
            "Position0050_FlareGun",
            "FlareGunItem"
        },
        {
            "Position0050_HandDrill",
            "HandDrillItem"
        },
        {
            "Position0050_RapidFireAutomaticRifle",
            "RapidFireAutomaticRifleItem"
        },
        {
            "Position0060_HandDrill2",
            "HandDrill2Item"
        },
        {
            "Position0060_PreciseAutomaticRifle",
            "PreciseAutomaticRifleItem"
        },
        {
            "Position0070_HandDrill3",
            "HandDrill3Item"
        },
        {
            "Position0070_UltimateAutomaticRifle",
            "UltimateAutomaticRifleItem"
        },
        {
            "Position0080_BasicHandHeldLauncher",
            "BasicHandHeldLauncherItem"
        },
        {
            "Position0080_HandDrill4",
            "HandDrill4Item"
        },
        {
            "Position0090_AdvancedHandHeldLauncher",
            "AdvancedHandHeldLauncherItem"
        },
        {
            "Position0090_Welder",
            "WelderItem"
        },
        {
            "Position0100_Welder2",
            "Welder2Item"
        },
        {
            "Position0110_Welder3",
            "Welder3Item"
        },
        {
            "Position0120_Welder4",
            "Welder4Item"
        }
    };
    public static string GetSubtypeId(string blueprintName)
    {
        if (BlueprintToItemMap.ContainsKey(blueprintName))
            return BlueprintToItemMap[blueprintName];
        return blueprintName;
    }

    // --- KATEGORIEN ---
    public static readonly List<string> CategoryComponents = new List<string>()
    {
        "PowerCell",
        "RadioCommunicationComponent",
        "ReactorComponent",
        "SmallTube",
        "SolarCell",
        "SteelPlate",
        "Superconductor",
        "ThrustComponent",
        "BulletproofGlass",
        "ComputerComponent",
        "ConstructionComponent",
        "DetectorComponent",
        "Display",
        "ExplosivesComponent",
        "GirderComponent",
        "GravityGeneratorComponent",
        "InteriorPlate",
        "LargeTube",
        "MedicalComponent",
        "MetalGrid",
        "MotorComponent"
    };
    public static readonly List<string> CategoryEquipment = new List<string>()
    {
        "Position0021_Medkit",
        "Position0010_OxygenBottle",
        "Position0020_HydrogenBottle",
        "Position0022_Powerkit",
        "Position0030_Canvas",
        "Position0050_FlareGun",
        "Position0051_FlareGunMagazine",
        "Position0061_FireworksBoxGreen",
        "Position0062_FireworksBoxRed",
        "Position0063_FireworksBoxYellow",
        "Position0064_FireworksBoxPink",
        "Position0065_FireworksBoxRainbow",
        "Position0040_Datapad",
        "Position0060_FireworksBoxBlue"
    };
    public static readonly List<string> CategoryTools = new List<string>()
    {
        "Position0010_AngleGrinder",
        "Position0020_AngleGrinder2",
        "Position0030_AngleGrinder3",
        "Position0040_AngleGrinder4",
        "Position0050_HandDrill",
        "Position0060_HandDrill2",
        "Position0070_HandDrill3",
        "Position0080_HandDrill4",
        "Position0090_Welder",
        "Position0100_Welder2",
        "Position0110_Welder3",
        "Position0120_Welder4"
    };
    public static readonly List<string> CategoryWeapons = new List<string>()
    {
        "Position0010_SemiAutoPistol",
        "Position0020_FullAutoPistol",
        "Position0030_EliteAutoPistol",
        "Position0040_AutomaticRifle",
        "Position0050_RapidFireAutomaticRifle",
        "Position0060_PreciseAutomaticRifle",
        "Position0070_UltimateAutomaticRifle",
        "Position0080_BasicHandHeldLauncher",
        "Position0090_AdvancedHandHeldLauncher"
    };
    public static readonly List<string> CategoryAmmo = new List<string>()
    {
        "Position0010_SemiAutoPistolMagazine",
        "Position0020_FullAutoPistolMagazine",
        "Position0030_ElitePistolMagazine",
        "Position0040_AutomaticRifleGun_Mag_20rd",
        "Position0050_RapidFireAutomaticRifleGun_Mag_50rd",
        "Position0060_PreciseAutomaticRifleGun_Mag_5rd",
        "Position0070_UltimateAutomaticRifleGun_Mag_30rd",
        "Position0080_NATO_25x184mmMagazine",
        "Position0090_AutocannonClip",
        "Position0100_Missile200mm",
        "Position0110_MediumCalibreAmmo",
        "Position0120_LargeCalibreAmmo",
        "Position0130_SmallRailgunAmmo",
        "Position0140_LargeRailgunAmmo"
    };
    public static readonly List<string> AssemblerBlueprints = CategoryComponents.Concat(CategoryEquipment).Concat(CategoryTools).Concat(CategoryWeapons).Concat(CategoryAmmo).ToList();
    // NEU: Dictionary für Ingots (DisplayName -> SubtypeId)
    public static readonly Dictionary<string, string> IngotTypes = new Dictionary<string, string>()
    {
        {
            "Cobalt Ingot",
            "Cobalt"
        },
        {
            "Gold Ingot",
            "Gold"
        },
        {
            "Gravel",
            "Stone"
        },
        {
            "Iron Ingot",
            "Iron"
        },
        {
            "Magnesium Powder",
            "Magnesium"
        },
        {
            "Nickel Ingot",
            "Nickel"
        },
        {
            "Platinum Ingot",
            "Platinum"
        },
        {
            "Prototech Scrap",
            "PrototechScrap"
        },
        {
            "Silicon Wafer",
            "Silicon"
        },
        {
            "Silver Ingot",
            "Silver"
        },
        {
            "Uranium Ingot",
            "Uranium"
        }
    };
}
