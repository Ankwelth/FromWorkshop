        const string VERSION = "BASE MANAGER v0.0.15";

        const int DEBUG_MAX_CHARS = 25000;

        const string LCD_MENU_PREFIX = "(menu)";
        const string LCD_INFO_PREFIX = "(info)";

        const string LCD_MENU_FONT = "Monospace";
        const string LCD_INFO_FONT = "Monospace";

        const string MENU_NO_ORE_MESSAGE = "No ore available";

        const float LCD_MENU_FONT_SIZE = 0.8f;
        const float LCD_INFO_FONT_SIZE = 0.7f;

        readonly Color LCD_MENU_FONT_COLOR = Color.White;
        readonly Color LCD_MENU_BACKGROUND_COLOR = Color.Black;
        readonly Color LCD_INFO_FONT_COLOR = Color.White;
        readonly Color LCD_INFO_BACKGROUND_COLOR = Color.Black;

        const string CONTAINER_INGOTS_PREFIX = "(ingots)";
        const string CONTAINER_ORES_PREFIX = "(ores)";
        const string CONTAINER_COMPONENTS_PREFIX = "(components)";
        const string CONTAINER_EQUIPMENT_PREFIX = "(equipments)";
        const string CONTAINER_AMMO_PREFIX = "(ammo)";
        const string CONTAINER_STUFF_PREFIX = "(stuff)";
        const string CONTAINER_FOOD_PREFIX = "(food)";
        const string CONTAINER_SEEDS_PREFIX = "(seeds)";

        const string CONTAINER_IGNORE_PREFIX = "(ignore)"; // keep containers with such label as is without sorting

        private readonly IMyCubeGrid currentGrid;
        private readonly List<IMyRefinery> refineries;
        private readonly List<IMyCargoContainer> containers;

        private readonly List<IMyCargoContainer> containersOres;
        private readonly List<IMyCargoContainer> containersIngots;
        private readonly List<IMyCargoContainer> containersComponents;
        private readonly List<IMyCargoContainer> containersEquipments;
        private readonly List<IMyCargoContainer> containersAmmo;
        private readonly List<IMyCargoContainer> containersStuff;
        private readonly List<IMyCargoContainer> containersFood;
        private readonly List<IMyCargoContainer> containersSeeds;

        private readonly List<IMyTextPanel> lcdMenuPanels;
        private readonly List<IMyTextPanel> lcdInfoPanels;

        private int currentMenuIndex = 0;

        private string gridName;

        private bool isSubMenuActive = false;
        private List<string> availableOres = new List<string>();
        private int currentOreIndex = 0;
        private string selectedOre = null;

        private readonly List<KeyValuePair<string, string>> menuItems =
            new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("", "[ SELECT ACTION ]"),
            new KeyValuePair<string, string>("start", "Start all refineries"),
            new KeyValuePair<string, string>("stop", "Stop and unload refineries"),
            new KeyValuePair<string, string>("sort", "Sort everything"),
            new KeyValuePair<string, string>("force_refine", "Force ore to refine"),
        };

        static readonly string[] FOOD_SUBTYPE_INDEX = new string[] {
            "Algae",
            "Grain",
            "Mushrooms",
            "Fruit",
            "Vegetables",
            "Meat",
            "MealPack"
        };

        private Dictionary<string, float> previousOres = new Dictionary<string, float>();
        private Dictionary<string, int> blockCounts = new Dictionary<string, int>();

        public Program()
        {
            currentGrid = Me.CubeGrid;

            containers = new List<IMyCargoContainer>();
            refineries = new List<IMyRefinery>();

            lcdMenuPanels = new List<IMyTextPanel>();
            lcdInfoPanels = new List<IMyTextPanel>();

            containersOres = new List<IMyCargoContainer>();
            containersIngots = new List<IMyCargoContainer>();
            containersComponents = new List<IMyCargoContainer>();
            containersEquipments = new List<IMyCargoContainer>();
            containersAmmo = new List<IMyCargoContainer>();
            containersStuff = new List<IMyCargoContainer>();
            containersFood = new List<IMyCargoContainer>();
            containersSeeds = new List<IMyCargoContainer>();

            ClearErrorLabelOnProgrammableBlock();
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            try
            {
                if (HasStructureChanged() && !InitializeBase())
                {
                    return;
                }

                bool isPeriodicTick = (updateSource & (UpdateType.Update1 | UpdateType.Update10 | UpdateType.Update100)) != 0;

                if (isSubMenuActive)
                {
                    HandleSubMenu(argument, isPeriodicTick);
                }
                else
                {
                    HandleMainMenu(argument, isPeriodicTick);
                }
            }
            catch (Exception e)
            {
                ShowErrorLabelOnProgrammableBlock();
                DebugWriteException($"Main(arg='{argument}', src={updateSource})", e);
                Echo("ERROR! See ProgrammableBlock.CustomData");

                return;
            }
        }

        private bool InitializeBase()
        {
            lcdMenuPanels.Clear();
            lcdInfoPanels.Clear();

            GetBlocksInCurrentGrid(lcdMenuPanels, LCD_MENU_PREFIX);
            GetBlocksInCurrentGrid(lcdInfoPanels, LCD_INFO_PREFIX);

            if (lcdMenuPanels.Count == 0)
            {
                ShowErrorLabelOnProgrammableBlock();
                Echo($"Step 1: Create LCD panel and add label \"{LCD_MENU_PREFIX}\" to its name");

                return false;
            }

            if (lcdInfoPanels.Count == 0)
            {
                ShowErrorLabelOnProgrammableBlock();
                Echo($"Step 2: Now create another LCD panel and add label \"{LCD_INFO_PREFIX}\" to its name");

                return false;
            }

            SetupLcdMenuPanels(lcdMenuPanels);
            SetupLcdInfoPanels(lcdInfoPanels);

            refineries.Clear();
            containers.Clear();
            containersOres.Clear();
            containersIngots.Clear();
            containersComponents.Clear();
            containersEquipments.Clear();
            containersAmmo.Clear();
            containersStuff.Clear();
            containersFood.Clear();
            containersSeeds.Clear();

            GetBlocks(containers);

            GetBlocksInCurrentGrid(refineries);
            GetBlocksInCurrentGrid(containersOres, CONTAINER_ORES_PREFIX);
            GetBlocksInCurrentGrid(containersIngots, CONTAINER_INGOTS_PREFIX);
            GetBlocksInCurrentGrid(containersComponents, CONTAINER_COMPONENTS_PREFIX);
            GetBlocksInCurrentGrid(containersEquipments, CONTAINER_EQUIPMENT_PREFIX);
            GetBlocksInCurrentGrid(containersAmmo, CONTAINER_AMMO_PREFIX);
            GetBlocksInCurrentGrid(containersStuff, CONTAINER_STUFF_PREFIX);
            GetBlocksInCurrentGrid(containersFood, CONTAINER_FOOD_PREFIX);
            GetBlocksInCurrentGrid(containersSeeds, CONTAINER_SEEDS_PREFIX);

            var checks = new Dictionary<List<IMyTerminalBlock>, string>
            {
                { containersOres.Cast<IMyTerminalBlock>().ToList(), CONTAINER_ORES_PREFIX },
                { containersIngots.Cast<IMyTerminalBlock>().ToList(), CONTAINER_INGOTS_PREFIX },
                { containersComponents.Cast<IMyTerminalBlock>().ToList(), CONTAINER_COMPONENTS_PREFIX },
                { containersEquipments.Cast<IMyTerminalBlock>().ToList(), CONTAINER_EQUIPMENT_PREFIX },
                { containersAmmo.Cast<IMyTerminalBlock>().ToList(), CONTAINER_AMMO_PREFIX },
                { containersStuff.Cast<IMyTerminalBlock>().ToList(), CONTAINER_STUFF_PREFIX },
                { containersFood.Cast<IMyTerminalBlock>().ToList(), CONTAINER_FOOD_PREFIX },
                { containersSeeds.Cast<IMyTerminalBlock>().ToList(), CONTAINER_SEEDS_PREFIX },
            };

            string errorMessage = CheckLabeledContainers(checks);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                WriteError(errorMessage);
                DebugAppend(errorMessage);

                return false;
            }

            gridName = currentGrid.CustomName;

            CacheBlocksCount();
            ClearErrorLabelOnProgrammableBlock();

            return true;
        }

        private void CacheBlocksCount()
        {
            blockCounts["refineries"] = refineries.Count;
            blockCounts["containers"] = containers.Count;
            blockCounts["lcdMenuPanels"] = lcdMenuPanels.Count;
            blockCounts["lcdInfoPanels"] = lcdInfoPanels.Count;
        }

        private bool HasStructureChanged()
        {
            if (blockCounts.Count == 0)
            {
                return true;
            }

            var currentLcdMenuPanels = new List<IMyTextPanel>();
            var currentLcdInfoPanels = new List<IMyTextPanel>();
            var currentRefineries = new List<IMyRefinery>();
            var currentContainers = new List<IMyCargoContainer>();

            GetBlocksInCurrentGrid(currentLcdMenuPanels, LCD_MENU_PREFIX);
            GetBlocksInCurrentGrid(currentLcdInfoPanels, LCD_INFO_PREFIX);
            GetBlocksInCurrentGrid(currentRefineries);
            GetBlocks(currentContainers);

            return currentLcdMenuPanels.Count != blockCounts["lcdMenuPanels"]
                || currentLcdInfoPanels.Count != blockCounts["lcdInfoPanels"]
                || currentRefineries.Count != blockCounts["refineries"]
                || currentContainers.Count != blockCounts["containers"];
        }

        private string CheckLabeledContainers(Dictionary<List<IMyTerminalBlock>, string> checks)
        {
            string errorMessage = "";

            foreach (var check in checks)
            {
                var list = check.Key;
                var prefix = check.Value;

                if (list.Count == 0)
                {
                    errorMessage += errorMessage == "" ? "Add next labels to any container(s):\n\n" : "\n";
                    errorMessage += $"\"{prefix}\"";
                }
            }

            return errorMessage.TrimEnd();
        }

        private void HandleMainMenu(string argument, bool isPeriodicTick)
        {
            bool needStatusRefresh = isPeriodicTick;

            if (!string.IsNullOrEmpty(argument))
            {
                switch (argument.ToLower())
                {
                    case "up":
                        currentMenuIndex = (currentMenuIndex - 1 + menuItems.Count) % menuItems.Count;
                        break;

                    case "down":
                        currentMenuIndex = (currentMenuIndex + 1) % menuItems.Count;
                        break;

                    case "execute":
                        ExecuteCurrentMenuItem(currentMenuIndex);
                        needStatusRefresh = true;
                        break;

                    case "sort":
                        SortEverything();
                        needStatusRefresh = true;
                        break;

                    default:
                        break;
                }
            }

            if (needStatusRefresh)
            {
                ShowStatus(updateBaseline: isPeriodicTick);
            }
                
            UpdateMenu(lcdMenuPanels, currentMenuIndex);
        }

        private void HandleSubMenu(string argument, bool isPeriodicTick)
        {
            bool needStatusRefresh = isPeriodicTick;

            if (!string.IsNullOrEmpty(argument))
            {
                switch (argument.ToLower())
                {
                    case "up":
                        currentOreIndex = (currentOreIndex - 1 + availableOres.Count) % availableOres.Count;
                        break;

                    case "down":
                        currentOreIndex = (currentOreIndex + 1) % availableOres.Count;
                        break;

                    case "execute":
                        selectedOre = availableOres[currentOreIndex];

                        if (selectedOre != MENU_NO_ORE_MESSAGE)
                        {
                            ForceRefine(selectedOre);
                        }

                        isSubMenuActive = false;
                        needStatusRefresh = true;
                        break;

                    default:
                        break;
                }
            }

            if (needStatusRefresh)
            {
                ShowStatus(updateBaseline: isPeriodicTick);
            }

            UpdateSubMenu();
        }

        private void UpdateSubMenu()
        {
            var menuText = $"{VERSION}\n({gridName})\n\nSelect Ore to Refine:\n";

            for (int i = 0; i < availableOres.Count; i++)
            {
                var prefix = i == currentOreIndex ? ">> " : "   ";
                menuText += prefix + availableOres[i] + "\n";
            }

            WriteToAllLcdPanels(lcdMenuPanels, menuText, LCD_MENU_FONT_COLOR, LCD_MENU_BACKGROUND_COLOR, LCD_MENU_FONT_SIZE);
        }

        public void SetupLcdMenuPanels(List<IMyTextPanel> lcdInfoPanels)
        {
            foreach (var lcdInfoPanel in lcdInfoPanels)
            {
                lcdInfoPanel.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                lcdInfoPanel.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
                lcdInfoPanel.Font = LCD_MENU_FONT;
                lcdInfoPanel.FontColor = LCD_MENU_FONT_COLOR;
                lcdInfoPanel.BackgroundColor = LCD_MENU_BACKGROUND_COLOR;
                lcdInfoPanel.FontSize = LCD_MENU_FONT_SIZE;
            }
        }

        public void SetupLcdInfoPanels(List<IMyTextPanel> lcdInfoPanels)
        {
            foreach (var lcdInfoPanel in lcdInfoPanels)
            {
                lcdInfoPanel.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                lcdInfoPanel.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
                lcdInfoPanel.Font = LCD_INFO_FONT;
                lcdInfoPanel.FontColor = LCD_INFO_FONT_COLOR;
                lcdInfoPanel.BackgroundColor = LCD_INFO_BACKGROUND_COLOR;
                lcdInfoPanel.FontSize = LCD_INFO_FONT_SIZE;
            }
        }

        private void UpdateMenu(List<IMyTextPanel> lcdPanels, int currentMenuIndex)
        {
            var maxLength = menuItems.Max(x => x.Value.Length);
            var menuText = $"{VERSION}\n({gridName})\n\n";

            for (int i = 0; i < menuItems.Count; i++)
            {
                var prefix = i == currentMenuIndex ? ">> " : "   ";
                menuText += prefix + menuItems[i].Value.PadRight(maxLength) + "\n";
            }

            WriteToAllLcdPanels(
                lcdPanels,
                menuText,
                LCD_MENU_FONT_COLOR,
                LCD_MENU_BACKGROUND_COLOR,
                LCD_MENU_FONT_SIZE
            );
        }

        private void EnterSubMenuForOres()
        {
            var refiningOres = GetRefiningOres(refineries, containers);
            availableOres = refiningOres.Keys.Where(ore => ore != "Ice").ToList();

            if (availableOres.Count == 0)
            {
                availableOres.Add(MENU_NO_ORE_MESSAGE);
            }

            isSubMenuActive = true;
            currentOreIndex = 0;
            UpdateSubMenu();
        }

        private void ExecuteCommand(string argument)
        {
            switch (argument)
            {
                case "start":
                    StartAllRefineries();
                    break;

                case "stop":
                    StopAndUnloadRefineries();
                    break;

                case "sort":
                    SortEverything();
                    break;

                case "force_refine":
                    EnterSubMenuForOres();
                    break;

                default:
                    Echo("Unknown command");
                    break;
            }
        }

        private void ExecuteCurrentMenuItem(int currentMenuIndex)
        {
            ExecuteCommand(menuItems[currentMenuIndex].Key);
        }

        private void ForceRefine(string oreType)
        {
            var refineriesToFill = refineries.Where(r => r.IsFunctional).ToList();
            int refineryCount = refineriesToFill.Count;

            if (refineryCount == 0)
            {
                Echo("No functional refineries available.");
                return;
            }

            foreach (var refinery in refineriesToFill)
            {
                refinery.Enabled = false;
                MoveAllItemsToContainers(refinery.GetInventory(0), containersOres);
                MoveAllItemsToContainers(refinery.GetInventory(1), containersIngots);
            }

            foreach (var container in containers)
            {
                var containerInventory = container.GetInventory();
                var items = new List<MyInventoryItem>();
                containerInventory.GetItems(items);

                foreach (var item in items)
                {
                    if (item.Type.SubtypeId == oreType)
                    {
                        var remainingAmount = item.Amount;

                        foreach (var refinery in refineriesToFill)
                        {
                            if (remainingAmount <= 0)
                            {
                                break;
                            }

                            var oreInventory = refinery.GetInventory(0);
                            containerInventory.TransferItemTo(oreInventory, item, remainingAmount);

                            remainingAmount = GetRemainingAmountById(containerInventory, item.ItemId);
                        }
                    }
                }
            }

            foreach (var refinery in refineriesToFill)
            {
                refinery.Enabled = true;
            }

            Echo($"Refineries now processing {oreType}.");
        }

        private Dictionary<string, float> GetRefiningOres(List<IMyRefinery> refineries, List<IMyCargoContainer> cargoContainers)
        {
            var refiningOres = new Dictionary<string, float>();

            foreach (var refinery in refineries)
            {
                var inputInventory = refinery.GetInventory(0);
                var items = new List<MyInventoryItem>();
                inputInventory.GetItems(items);

                foreach (var item in items)
                {
                    if (item.Type.TypeId == "MyObjectBuilder_Ore")
                    {
                        if (!refiningOres.ContainsKey(item.Type.SubtypeId))
                        {
                            refiningOres[item.Type.SubtypeId] = 0;
                        }

                        refiningOres[item.Type.SubtypeId] += (float)item.Amount;
                    }
                }
            }

            foreach (var container in cargoContainers)
            {
                var containerInventory = container.GetInventory();
                var items = new List<MyInventoryItem>();
                containerInventory.GetItems(items);

                foreach (var item in items)
                {
                    if (item.Type.TypeId == "MyObjectBuilder_Ore")
                    {
                        if (!refiningOres.ContainsKey(item.Type.SubtypeId))
                        {
                            refiningOres[item.Type.SubtypeId] = 0;
                        }

                        refiningOres[item.Type.SubtypeId] += (float)item.Amount;
                    }
                }
            }

            return refiningOres;
        }

        private void ShowStatus(bool updateBaseline)
        {
            var statusText = "";

            var uraniumAmount = GetReactorFuel();
            statusText += $"Reactors fuel: {uraniumAmount:F2} kg\n";

            float currentHydrogen, maxHydrogen;
            GetHydrogenLevel(out currentHydrogen, out maxHydrogen);
            statusText += $"Hydrogen fuel: {currentHydrogen:F1}ML (max {maxHydrogen:F1}ML)\n";

            double irrigationIceKg;
            if (TryGetIrrigationIceLoaded(out irrigationIceKg))
            {
                statusText += $"Irrigation ice: {irrigationIceKg:N0} kg\n";
            }

            statusText += "\nRefineries: " + RefineryStatusOnOff(refineries) + "\n";
            statusText += "Working now: " + RefineryFunctionality(refineries) + "\n";

            var refiningOres = GetRefiningOres(refineries, containers);
            statusText += "Ores available:\n";

            foreach (var ore in refiningOres)
            {
                var oreName = ore.Key;
                var currentAmount = ore.Value;
                float previousAmount;

                if (!previousOres.TryGetValue(oreName, out previousAmount))
                {
                    previousAmount = currentAmount;
                    if (updateBaseline)
                    {
                        previousOres[oreName] = currentAmount;
                    }
                }

                float delta = currentAmount - previousAmount;

                string deltaText = delta != 0
                    ? $" ({(delta > 0 ? "+" : "")}{delta:F2} kg)"
                    : "";

                statusText += $"- {oreName}: {FormatNumber(currentAmount)} kg{deltaText}\n";

                if (updateBaseline)
                {
                    previousOres[oreName] = currentAmount;
                }
            }

            if (updateBaseline)
            {
                var toRemove = new List<string>();
                foreach (var k in previousOres.Keys)
                {
                    if (!refiningOres.ContainsKey(k))
                    {
                        toRemove.Add(k);
                    }
                }

                foreach (var k in toRemove)
                {
                    previousOres.Remove(k);
                }
            }

            var spaceForOreLeft = GetSpaceForOreLeft();
            statusText += $"\nSpace for ore left: {FormatNumber(spaceForOreLeft)}\n";

            WriteToAllLcdPanels(lcdInfoPanels, statusText, LCD_INFO_FONT_COLOR, LCD_INFO_BACKGROUND_COLOR, LCD_INFO_FONT_SIZE);
        }

        // This function calculating free space only for containers with "(ores)" label
        private float GetSpaceForOreLeft()
        {
            float totalSpaceLeft = 0f;

            foreach (var container in containersOres)
            {
                var inventory = container.GetInventory();
                float availableSpace = (float)inventory.MaxVolume - (float)inventory.CurrentVolume;
                totalSpaceLeft += availableSpace * 1000;
            }

            return totalSpaceLeft;
        }

        private string FormatNumber(float number)
        {
            var formatted = number.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);

            return formatted.Replace(',', ' ');
        }

        private float GetReactorFuel()
        {
            var reactors = new List<IMyReactor>();
            GetBlocksInCurrentGrid(reactors);

            float totalUranium = 0f;

            foreach (var reactor in reactors)
            {
                var inventory = reactor.GetInventory();
                var items = new List<MyInventoryItem>();
                inventory.GetItems(items);

                foreach (var item in items)
                {
                    if (item.Type.SubtypeId == "Uranium")
                    {
                        totalUranium += (float)item.Amount;
                    }
                }
            }

            return totalUranium;
        }

        private void GetHydrogenLevel(out float currentHydrogen, out float maxHydrogen)
        {
            var hydrogenTanks = new List<IMyGasTank>();

            GetBlocksInCurrentGrid(hydrogenTanks);
            hydrogenTanks.RemoveAll(tank => !tank.BlockDefinition.SubtypeName.Contains("Hydrogen"));

            currentHydrogen = 0f;
            maxHydrogen = 0f;

            foreach (var tank in hydrogenTanks)
            {
                currentHydrogen += (float)tank.FilledRatio * (float)tank.Capacity / 1000000;
                maxHydrogen += (float)tank.Capacity / 1000000;
            }
        }

        private void StartAllRefineries()
        {
            foreach (var refinery in refineries)
            {
                refinery.Enabled = true;
            }
        }

        private void StopAndUnloadRefineries()
        {
            foreach (var refinery in refineries)
            {
                refinery.Enabled = false;
                MoveAllItemsToContainers(refinery.GetInventory(0), containersOres);
                MoveAllItemsToContainers(refinery.GetInventory(1), containersIngots);
            }
        }

        private string RefineryStatusOnOff(List<IMyRefinery> blocks)
        {
            if (blocks.Count == 0)
            {
                return "NONE";
            }

            int blocksOn = 0;
            int blocksOff = 0;
            int blocksNotFunc = 0;

            foreach (IMyRefinery block in blocks)
            {
                if (!block.IsFunctional)
                    blocksNotFunc++;
                else if (block.Enabled)
                    blocksOn++;
                else
                    blocksOff++;

            }

            string statusText;
            if (blocksNotFunc == blocks.Count)
            {
                statusText = $"{blocks.Count} (All NOT FUNC)";
            }
            else if (blocksOn == blocks.Count)
            {
                statusText = $"{blocks.Count} (All ON)";
            }
            else if (blocksOff == blocks.Count)
            {
                statusText = $"{blocks.Count} (All OFF)";
            }
            else
            {
                if (blocksNotFunc > 0)
                {
                    statusText = $"{blocks.Count}\n({blocksOn} - ON, {blocksOff} - OFF, {blocksNotFunc} - NOT FUNC)";
                }
                else
                {
                    statusText = $"{blocks.Count} ({blocksOn} - ON, {blocksOff} - OFF)";
                }
            }

            return statusText;
        }

        private string RefineryFunctionality(List<IMyRefinery> blocks)
        {
            if (blocks.Count == 0)
            {
                return "NONE";
            }

            int blocksWork = 0;
            int blocksIdle = 0;

            foreach (IMyRefinery block in blocks)
            {
                if (IsRefineryWorking(block))
                    blocksWork++;
                else
                    blocksIdle++;
            }

            string statusText = $"{(blocksWork == blocks.Count ? "All WORKING" : (blocksIdle == blocks.Count ? "All IDLE" : $"{blocksWork} - WORK, {blocksIdle} - IDLE"))}";

            return statusText;
        }

        private void SortEverything()
        {
            var allCargoContainers = new List<IMyCargoContainer>();
            GetBlocks(allCargoContainers);

            var allAssemblers = new List<IMyAssembler>();
            GetBlocks(allAssemblers);

            var allConnectors = new List<IMyShipConnector>();
            GetBlocks(allConnectors);

            var allCockpits = new List<IMyCockpit>();
            GetBlocks(allCockpits);

            var allCryoChambers = new List<IMyCryoChamber>();
            GetBlocks(allCryoChambers);

            var allAlgaeFarms = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(
                allAlgaeFarms,
                b =>
                    !IsIgnored(b) &&
                    b.HasInventory &&
                    (
                        b.BlockDefinition.SubtypeName == "LargeBlockAlgaeFarm" ||
                        b.BlockDefinition.SubtypeName == "LargeBlockAlgaeFarmHalf" ||
                        b.BlockDefinition.SubtypeName == "SmallBlockAlgaeFarm" ||
                        b.BlockDefinition.SubtypeName == "SmallBlockAlgaeFarmHalf"
                    )
            );

            var allWelders = new List<IMyShipWelder>();
            GetBlocks(allWelders);

            var allGrinders = new List<IMyShipGrinder>();
            GetBlocks(allGrinders);

            var oreContainers = new List<IMyCargoContainer>();
            var ingotContainers = new List<IMyCargoContainer>();
            var componentContainers = new List<IMyCargoContainer>();
            var equipmentContainers = new List<IMyCargoContainer>();
            var ammoContainers = new List<IMyCargoContainer>();
            var stuffContainers = new List<IMyCargoContainer>();
            var foodContainers = new List<IMyCargoContainer>();
            var seedsContainers = new List<IMyCargoContainer>();

            foreach (var container in allCargoContainers)
            {
                if (container.CustomName.Contains(CONTAINER_ORES_PREFIX))
                {
                    oreContainers.Add(container);
                }

                if (container.CustomName.Contains(CONTAINER_INGOTS_PREFIX))
                {
                    ingotContainers.Add(container);
                }

                if (container.CustomName.Contains(CONTAINER_COMPONENTS_PREFIX))
                {
                    componentContainers.Add(container);
                }

                if (container.CustomName.Contains(CONTAINER_EQUIPMENT_PREFIX))
                {
                    equipmentContainers.Add(container);
                }

                if (container.CustomName.Contains(CONTAINER_AMMO_PREFIX))
                {
                    ammoContainers.Add(container);
                }

                if (container.CustomName.Contains(CONTAINER_STUFF_PREFIX))
                {
                    stuffContainers.Add(container);
                }

                if (container.CustomName.Contains(CONTAINER_FOOD_PREFIX))
                {
                    foodContainers.Add(container);
                }

                if (container.CustomName.Contains(CONTAINER_SEEDS_PREFIX))
                {
                    seedsContainers.Add(container);
                }
            }

            foreach (var container in allCargoContainers)
            {
                SortInventory(container.GetInventory(),
                    oreContainers,
                    ingotContainers,
                    componentContainers,
                    equipmentContainers,
                    ammoContainers,
                    stuffContainers,
                    foodContainers,
                    seedsContainers
                );
            }

            foreach (var connector in allConnectors)
            {
                SortInventory(connector.GetInventory(),
                    oreContainers,
                    ingotContainers,
                    componentContainers,
                    equipmentContainers,
                    ammoContainers,
                    stuffContainers,
                    foodContainers,
                    seedsContainers
                );
            }

            foreach (var assembler in allAssemblers)
            {
                if (!assembler.IsProducing)
                {
                    MoveAllItemsToContainers(assembler.GetInventory(0), ingotContainers);
                }

                MoveAllItemsToContainers(assembler.GetInventory(1), componentContainers);
            }

            foreach (var refinery in refineries)
            {
                MoveAllItemsToContainers(refinery.GetInventory(1), containersIngots);
            }

            foreach (var cockpit in allCockpits)
            {
                SortInventory(cockpit.GetInventory(),
                    oreContainers,
                    ingotContainers,
                    componentContainers,
                    equipmentContainers,
                    ammoContainers,
                    stuffContainers,
                    foodContainers,
                    seedsContainers
                );
            }

            foreach (var farm in allAlgaeFarms)
            {
                SortInventory(farm.GetInventory(),
                    oreContainers,
                    ingotContainers,
                    componentContainers,
                    equipmentContainers,
                    ammoContainers,
                    stuffContainers,
                    foodContainers,
                    seedsContainers
                );
            }

            foreach (var cryoChamber in allCryoChambers)
            {
                SortInventory(cryoChamber.GetInventory(),
                    oreContainers,
                    ingotContainers,
                    componentContainers,
                    equipmentContainers,
                    ammoContainers,
                    stuffContainers,
                    foodContainers,
                    seedsContainers
                );
            }

            foreach (var welder in allWelders)
            {
                SortInventory(welder.GetInventory(),
                    oreContainers,
                    ingotContainers,
                    componentContainers,
                    equipmentContainers,
                    ammoContainers,
                    stuffContainers,
                    foodContainers,
                    seedsContainers
                );
            }

            foreach (var grinder in allGrinders)
            {
                SortInventory(grinder.GetInventory(),
                    oreContainers,
                    ingotContainers,
                    componentContainers,
                    equipmentContainers,
                    ammoContainers,
                    stuffContainers,
                    foodContainers,
                    seedsContainers
                );
            }
        }

        private void SortInventory(
            IMyInventory inventory,
            List<IMyCargoContainer> oreContainers,
            List<IMyCargoContainer> ingotContainers,
            List<IMyCargoContainer> componentContainers,
            List<IMyCargoContainer> equipmentContainers,
            List<IMyCargoContainer> ammoContainers,
            List<IMyCargoContainer> stuffContainers,
            List<IMyCargoContainer> foodContainers,
            List<IMyCargoContainer> seedsContainers
            )
        {
            if (inventory == null)
            {
                return;
            }

            var items = new List<MyInventoryItem>();
            inventory.GetItems(items);

            foreach (var item in items)
            {
                var typeId = item.Type.TypeId;
                var subTypeId = item.Type.SubtypeId;

                if (typeId == "MyObjectBuilder_Ore")
                {
                    MoveItemToCategory(inventory, item, oreContainers);
                }
                else if (typeId == "MyObjectBuilder_Ingot")
                {
                    MoveItemToCategory(inventory, item, ingotContainers);
                }
                else if (typeId == "MyObjectBuilder_Component")
                {
                    MoveItemToCategory(inventory, item, componentContainers);
                }
                else if (
                    typeId == "MyObjectBuilder_PhysicalGunObject"
                    || typeId == "MyObjectBuilder_Tool"
                    || typeId == "MyObjectBuilder_OxygenContainerObject"
                    || typeId == "MyObjectBuilder_GasContainerObject"
                )
                {
                    MoveItemToCategory(inventory, item, equipmentContainers);
                }
                else if (typeId == "MyObjectBuilder_AmmoMagazine")
                {
                    MoveItemToCategory(inventory, item, ammoContainers);
                }
                else if (typeId == "MyObjectBuilder_PhysicalObject"
                    || typeId == "MyObjectBuilder_Credit"
                    || typeId == "MyObjectBuilder_Datapad"
                    || typeId == "MyObjectBuilder_ConsumableItem"
                )
                {
                    if (IsFoodItem(item))
                    {
                        MoveItemToCategory(inventory, item, foodContainers);
                    }
                    else
                    {
                        MoveItemToCategory(inventory, item, stuffContainers);
                    }
                }
                else if (typeId == "MyObjectBuilder_SeedItem")
                {
                    MoveItemToCategory(inventory, item, seedsContainers);
                }
                else
                {
                    Echo($"(Not error, just warning) Unknown item for sorting: TypeId = {typeId}, SubtypeId = {subTypeId}");
                }
            }
        }

        private void MoveItemToCategory(IMyInventory sourceInventory, MyInventoryItem item, List<IMyCargoContainer> targetContainers)
        {
            VRage.MyFixedPoint remainingAmount = item.Amount;
            long itemId = item.ItemId;

            foreach (var container in targetContainers)
            {
                if (remainingAmount <= 0)
                {
                    return;
                }

                var targetInventory = container.GetInventory();
                if (sourceInventory == targetInventory)
                {
                    // item already in proper container
                    return;
                }

                // Previously I checked the status of TransferItemTo function.
                // It returns very unpredictable result. It can return "true" while not all the inventory items transferred yet.
                // So, now I made a compromise. I transferring items to all containers and checking only remainingAmount. All other algorithms are not effecient.
                sourceInventory.TransferItemTo(targetInventory, item, remainingAmount);

                remainingAmount = GetRemainingAmountById(sourceInventory, itemId);
            }

            if (remainingAmount > 0)
            {
                Echo($"Warning: Unable to move item {item.Type.SubtypeId}. Remaining: {remainingAmount}.");
            }
        }

        private void MoveAllItemsToContainers(IMyInventory sourceInventory, List<IMyCargoContainer> targetContainers)
        {
            var items = new List<MyInventoryItem>();
            sourceInventory.GetItems(items);

            foreach (var item in items)
            {
                MoveItemToCategory(sourceInventory, item, targetContainers);
            }
        }

        private void WriteToAllLcdPanels(List<IMyTextPanel> lcdPanels, String message, Color textColor, Color bgColor, float fontSize, bool isKeepPreviousText = false)
        {
            foreach (IMyTextPanel lcdPanel in lcdPanels)
            {
                lcdPanel.SetValue("BackgroundColor", bgColor);
                lcdPanel.SetValue("FontColor", textColor);
                lcdPanel.SetValue("FontSize", fontSize);

                lcdPanel.WriteText(message, isKeepPreviousText);
            }
        }

        public bool IsFull(IMyInventory container)
        {
            return ((float)container.CurrentVolume / (float)container.MaxVolume > 0.99F);
        }

        public bool IsRefineryWorking(IMyRefinery refinery)
        {
            if (!refinery.Enabled)
            {
                return false;
            }

            return refinery.IsWorking && refinery.IsProducing;
        }

        void GetBlocks<T>(List<T> blockList, string prefix = null) where T : class, IMyTerminalBlock
        {
            if (string.IsNullOrEmpty(prefix))
            {
                GridTerminalSystem.GetBlocksOfType<T>(blockList, block => !IsIgnored(block));
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType<T>(blockList, block => block.CustomName.Contains(prefix) && !IsIgnored(block));
            }
        }

        void GetBlocksInCurrentGrid<T>(List<T> blockList, string prefix = null) where T : class, IMyTerminalBlock
        {
            if (string.IsNullOrEmpty(prefix))
            {
                GridTerminalSystem.GetBlocksOfType<T>(blockList, block => block.CubeGrid == currentGrid && !IsIgnored(block));
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType<T>(blockList, block => block.CubeGrid == currentGrid && block.CustomName.Contains(prefix) && !IsIgnored(block));
            }
        }

        private bool IsIgnored(IMyTerminalBlock block)
        {
            return block.CustomName.Contains(CONTAINER_IGNORE_PREFIX);
        }

        private void WriteError(string message)
        {
            if (lcdInfoPanels.Count > 0)
            {
                foreach (var lcdInfoPanel in lcdInfoPanels)
                {
                    lcdInfoPanel.WriteText(message);
                }
            }
        }

        private void ShowErrorLabelOnProgrammableBlock()
        {
            var surface = Me.GetSurface(0);
            surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
            surface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
            surface.FontSize = 2.5f;
            surface.FontColor = new Color(255, 0, 0);
            surface.BackgroundColor = new Color(0, 0, 0);
            surface.WriteText("ERROR\n\nCheck logs");
        }

        private void ClearErrorLabelOnProgrammableBlock()
        {
            var surface = Me.GetSurface(0);
            surface.WriteText("");
        }

        private bool IsFoodItem(MyInventoryItem item)
        {
            string subtype = item.Type.SubtypeId;

            for (int i = 0; i < FOOD_SUBTYPE_INDEX.Length; i++)
            {
                if (subtype.IndexOf(FOOD_SUBTYPE_INDEX[i], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private VRage.MyFixedPoint GetRemainingAmountById(IMyInventory inventory, long itemId)
        {
            var items = new List<MyInventoryItem>();
            inventory.GetItems(items);

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ItemId == itemId)
                {
                    return items[i].Amount;
                }
            }

            return 0;
        }

        private bool TryGetIrrigationIceLoaded(out double iceKg)
        {
            iceKg = 0;
            var irrigation = new List<IMyGasGenerator>();

            GridTerminalSystem.GetBlocksOfType(irrigation, b =>
                b != null
                && !IsIgnored(b)
                && b.CubeGrid == Me.CubeGrid
                && b.BlockDefinition.SubtypeName == "IrrigationSystem"
                && b.HasInventory
            );

            if (irrigation.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < irrigation.Count; i++)
            {
                var inv = irrigation[i].GetInventory(0);
                
                if (inv == null)
                {
                    continue;
                }

                var items = new List<MyInventoryItem>();
                inv.GetItems(items);

                for (int k = 0; k < items.Count; k++)
                {
                    var it = items[k];
                    if (it.Type.TypeId == "MyObjectBuilder_Ore" && it.Type.SubtypeId == "Ice")
                    {
                        iceKg += (double)it.Amount;
                    }
                }
            }

            return true;
        }

        private void DebugAppend(string msg)
        {
            var line = msg + "\n";
            if (Me.CustomData.Length + line.Length > DEBUG_MAX_CHARS)
                Me.CustomData = Me.CustomData.Substring(Math.Max(0, Me.CustomData.Length - DEBUG_MAX_CHARS / 2));

            Me.CustomData += line;
        }

        private void DebugWriteException(string context, Exception e)
        {
            DebugAppend("=== EXCEPTION ===");
            DebugAppend("Context: " + context);
            DebugAppend("Type: " + e.GetType().Name);
            DebugAppend("Message: " + e.Message);
            DebugAppend("Stack: " + e.StackTrace);
            DebugAppend("=================");
        }
