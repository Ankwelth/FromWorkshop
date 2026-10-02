public interface IParseToml
        {
            void setTomlProperty(string key, string value);
            void setTomlProperty(string key, string value, string section);
        }
        public struct ProgramOptions : IParseToml
        {
            public bool Debug;
            public uint ScanInterval;
            public uint SortInterval;
            public uint StockInterval;
            public bool IgnoreReactors;
            public string ProductionBlocks;
            // Collect items of type `key` in inventories of group `value`
            public Dictionary<MyInventoryItemFilter, string> ItemDestimations;
            public Dictionary<MyItemType, MyFixedPoint> KeepStocked;

            public static ProgramOptions Default
            {
                get
                {
                    return new ProgramOptions()
                    {
                        Debug = false,
                        ScanInterval = 240,
                        SortInterval = 120,
                        StockInterval = 180,
                        IgnoreReactors = true,
                        ProductionBlocks = null,
                        ItemDestimations = new Dictionary<MyInventoryItemFilter, string>(),
                        KeepStocked = new Dictionary<MyItemType, MyFixedPoint>(),
                    };
                }
            }

            public ProgramOptions(uint scanInterval, uint sortInterval, uint stockInterval)
            {
                this = Default;
                ScanInterval = scanInterval;
                SortInterval = sortInterval;
                StockInterval = stockInterval;
            }

            public void setTomlProperty(string key, string value)
            {
                switch (key)
                {
                    case "debug":
                        Debug = ParseBool(value);
                        return;
                    case "scan interval":
                        ScanInterval = Convert.ToUInt32(value);
                        return;
                    case "sort interval":
                        SortInterval = Convert.ToUInt32(value);
                        return;
                    case "stock interval":
                        StockInterval = Convert.ToUInt32(value);
                        return;
                    case "ignore reactors":
                        IgnoreReactors = ParseBool(value);
                        return;
                    case "production blocks":
                        ProductionBlocks = value;
                        return;
                    default:
                        return;
                }
            }
            public void setTomlProperty(string key, string value, string section)
            {
                switch (section)
                {
                    case "item destinations":
                        ItemDestimations.Add(ParseFilter(key), value);
                        return;
                    case "keep stocked":
                        var item = ParseItem(key);
                        var stock = MyFixedPoint.DeserializeStringSafe(value);
                        if (!item.GetItemInfo().UsesFractions) MyFixedPoint.Floor(stock);
                        KeepStocked.Add(item, stock);
                        return;
                    default:
                        return;
                }
            }
        }

        public struct CollectionLink
        {
            public MyInventoryItemFilter Filter;
            public IMyInventory Source;
            public List<IMyInventory> Targets;

            public CollectionLink(MyInventoryItemFilter filter, IMyInventory source, List<IMyInventory> targets)
            {
                Filter = filter;
                Source = source;
                Targets = targets;
            }

            public void Collect()
            {
                for (int i = Source.ItemCount - 1; i >= 0; i--)
                {
                    var item = Source.GetItemAt(i).Value;
                    if (!ApplyFilter(Filter, item.Type)) continue;

                    var info = item.Type.GetItemInfo();
                    var amount = item.Amount;
                    var multiplier = (MyFixedPoint)(1d / info.Volume);
                    foreach (var target in Targets)
                    {
                        if (target.IsFull) continue;
                        var free = target.MaxVolume - target.CurrentVolume;
                        var maxTransfer = free * multiplier;
                        if (!info.UsesFractions) MyFixedPoint.Floor(maxTransfer);
                        if (maxTransfer >= amount)
                        {
                            if (Source.TransferItemTo(target, item))
                                break;
                        }
                        else
                        {
                            if (Source.TransferItemTo(target, item, maxTransfer))
                                amount -= maxTransfer;
                        }
                    }
                }
            }
        }

        public static readonly Dictionary<string, MyInventoryItemFilter> ItemGroups = new Dictionary<string, MyInventoryItemFilter>()
        {
            { "$ingot", new MyInventoryItemFilter("MyObjectBuilder_Ingot/(null)", true) },
            { "$ore", new MyInventoryItemFilter("MyObjectBuilder_Ore/(null)", true) },
            { "$ammo", new MyInventoryItemFilter("MyObjectBuilder_AmmoMagazine/(null)", true) },
            { "$component", new MyInventoryItemFilter("MyObjectBuilder_Component/(null)", true) },
            { "$tool", new MyInventoryItemFilter("MyObjectBuilder_ToolbarItem/(null)", true) },
            { "$consumable", new MyInventoryItemFilter("MyObjectBuilder_ConsumableItem/(null)", true) },
        };
        public static readonly Dictionary<MyItemType, MyDefinitionId> ItemBlueprints = new Dictionary<MyItemType, MyDefinitionId>();
        public static readonly Dictionary<MyDefinitionId, MyItemType> BlueprintItems = new Dictionary<MyDefinitionId, MyItemType>();

        static Program()
        {
            Action<string, string> add = (item, bp) => {
                var itemid = MyItemType.Parse($"MyObjectBuilder_{item}");
                var bpid = MyDefinitionId.Parse($"MyObjectBuilder_BlueprintDefinition/{bp}");
                ItemBlueprints.Add(itemid, bpid);
                BlueprintItems.Add(bpid, itemid);
            };
            // TODO
        }

        private int OptionsHash = 0;
        private DateTime LastScan = DateTime.MinValue, LastSort = DateTime.MinValue, LastStock = DateTime.MinValue;
        ProgramOptions Options = ProgramOptions.Default;
        readonly IMyTextSurface Console;
        private readonly List<CollectionLink> CollectionLinks = new List<CollectionLink>();
        private readonly Dictionary<MyItemType, List<IMyProductionBlock>> ItemProducers = new Dictionary<MyItemType, List<IMyProductionBlock>>();
        string OptionsParseError = null;
        readonly List<IMyTerminalBlock> BlockBuffer = new List<IMyTerminalBlock>(0);

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            Console = Me.GetSurface(0);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            int opthash = Me.CustomData.GetHashCode();
            if (OptionsHash != opthash || updateSource == UpdateType.Terminal)
            {
                Console.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                OptionsParseError = null;
                Options = ProgramOptions.Default;
                try
                {
                    Options = ParseToml(Me.CustomData, ProgramOptions.Default);
                }
                catch (Exception ex)
                {
                    OptionsParseError = ex.Message;
                }
                OptionsHash = opthash;
            }
            var now = DateTime.UtcNow;
            bool performScan = false, performSort = false, performStock = false;
            bool canSort = Options.ItemDestimations.Count > 0;
            bool canStock = false; // Options.ProductionBlocks != null && Options.KeepStocked.Count > 0;
            if ((canSort || canStock) && LastScan.AddSeconds(Options.ScanInterval) < now)
            {
                LastScan = now;
                performScan = true;
            }
            if (canSort && LastSort.AddSeconds(Options.SortInterval) < now)
            {
                LastSort = now;
                performSort = true;
            }
            if (canStock && LastStock.AddSeconds(Options.StockInterval) < now)
            {
                LastStock = now;
                performStock = true;
            }
            if (performScan || performSort || performStock || Options.Debug)
            {
                Func<DateTime, string> formatTime = time => time == DateTime.MinValue ? "--:--:--" : $"{time.ToLocalTime():HH:mm:ss}";
                Console.WriteText($"Last Scan: {formatTime(LastScan)}\nLast Sort: {formatTime(LastSort)}\nLast Stock: {formatTime(LastStock)}\n");
            }
            if (OptionsParseError != null)
            {
                Console.WriteText($"Parse Error: {OptionsParseError}\n", true);
            }
            if (Options.Debug)
            {
                Func<MyInventoryItemFilter, string> formatFilter = filter =>
                {
                    var sb = new StringBuilder();
                    var typeId = filter.ItemType.TypeId;
                    if (typeId.StartsWith("MyObjectBuilder_")) typeId = typeId.Substring(16);
                    sb.Append(typeId);
                    if (!filter.AllSubTypes)
                        sb.Append('/').Append(filter.ItemType.SubtypeId);
                    return sb.ToString();
                };
                Console.WriteText($"[DEBUG] Connections:\n{String.Join("\n", Options.ItemDestimations.ToArray().Select(e => $"- {formatFilter(e.Key)} => {e.Value}"))}\n", true);
            }
            if (performScan) Scan();
            if (performSort) CollectAll();
            if (performStock) Stock();
        }

        public static T ParseToml<T>(string toml, T obj) where T : IParseToml
        {
            string section = null;
            foreach (var entry in toml.Split('\n'))
            {
                var trimmedEntry = entry.Trim();
                if (trimmedEntry.Length == 0) continue;
                if (trimmedEntry.StartsWith("#")) continue;
                if (trimmedEntry.StartsWith("[") && trimmedEntry.EndsWith("]"))
                {
                    section = trimmedEntry.Substring(1, trimmedEntry.Length - 2).Trim();
                    continue;
                }
                var sep = trimmedEntry.IndexOf('=');
                if (sep == -1) continue;
                var key = trimmedEntry.Substring(0, sep).TrimEnd();
                var value = trimmedEntry.Substring(sep + 1).TrimStart();
                if (section == null)
                    obj.setTomlProperty(key, value);
                else obj.setTomlProperty(key, value, section);
            }
            return obj;
        }

        public static bool ParseBool(string str)
        {
            switch (str)
            {
                case "true":
                case "yes":
                    return true;
                case "false":
                case "no":
                    return false;
                default:
                    throw new FormatException($"'{str}' is not a boolean");
            }
        }

        void CollectAll()
        {
            CollectionLinks.ForEach(link => link.Collect());
        }

        void Scan()
        {
            // Sorting
            CollectionLinks.Clear();
            BlockBuffer.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyInventoryOwner>(BlockBuffer, b =>
                b.IsSameConstructAs(Me)
                && b.InventoryCount > 0
                && !(Options.IgnoreReactors && b is IMyReactor));
            foreach (var entry in Options.ItemDestimations)
            {
                var filter = entry.Key;
                var targetGroupName = entry.Value;

                var targetGroup = GridTerminalSystem.GetBlockGroupWithName(targetGroupName);
                if (targetGroup == null)
                {
                    Console.WriteText($"Group not found: {targetGroupName}\n", true);
                    continue;
                }
                // Find target inventories
                var targetInvs = new List<IMyInventory>(8);
                var targetIds = new HashSet<long>(8);
                targetGroup.GetBlocks(null, block =>
                {
                    for (int i = block.InventoryCount - 1; i >= 0; i--)
                    {
                        targetIds.Add(block.GetId());
                        var targetInv = block.GetInventory(i);
                        if (AcceptsItemFor(targetInv, filter))
                            targetInvs.Add(targetInv);
                    }
                    return false; // iterate only
                });
                if (targetInvs.Count == 0) continue;
                // Find source inventories
                foreach (var sourceBlock in BlockBuffer)
                {
                    if (targetIds.Contains(sourceBlock.GetId())) continue;
                    for (int i = sourceBlock.InventoryCount - 1; i >= 0; i--)
                    {
                        var sourceInv = sourceBlock.GetInventory(i);
                        if (AcceptsItemFor(sourceInv, filter) && targetInvs.Any(tinv => sourceInv.CanTransferItemTo(tinv, filter.ItemType)))
                            CollectionLinks.Add(new CollectionLink(filter, sourceInv, targetInvs));
                    }
                }
            }

            if (Options.Debug) Console.WriteText($"[DEBUG] Scanned {BlockBuffer.Count} blocks\n[DEBUG] Found {CollectionLinks.Count} links\n", true);

            // Stocking
            if (Options.KeepStocked.Count == 0) return;
            var removeProducers = new HashSet<MyItemType>(ItemProducers.Keys);
            foreach (var i in Options.KeepStocked.Keys) removeProducers.Remove(i);
            foreach (var i in removeProducers) ItemProducers.Remove(i);
            foreach (var l in ItemProducers.Values) l.Clear();
            var productionGroup = GridTerminalSystem.GetBlockGroupWithName(Options.ProductionBlocks);
            if (productionGroup == null)
            {
                Console.WriteText($"Group not found: {Options.ProductionBlocks}\n", true);
                return;
            }
            productionGroup.GetBlocksOfType((List<IMyProductionBlock>)null, b =>
            {
                if (!b.IsSameConstructAs(Me)) return false;
                foreach (var item in Options.KeepStocked.Keys)
                {
                    if (!b.CanUseBlueprint(item)) continue;
                    if (b is IMyAssembler)
                    {
                        var asm = b as IMyAssembler;
                        if (asm.Mode != MyAssemblerMode.Assembly || asm.CooperativeMode) continue;
                    }
                    if (ItemProducers.ContainsKey(item))
                        ItemProducers[item].Add(b);
                    else ItemProducers[item] = new List<IMyProductionBlock>() { b };
                }
                return false;
            });
        }

        void Stock()
        {
            var stocks = new Dictionary<MyItemType, MyFixedPoint>(Options.KeepStocked.Count);
            foreach (var key in Options.KeepStocked.Keys) stocks[key] = 0;
            GetStocks(stocks);

            if (Options.Debug)
            {
                Console.WriteText("[DEBUG] Stocks:\n", true);
                foreach (var stock in stocks)
                {
                    Console.WriteText($" - {stock.Key}: {stock.Value}\n", true);
                }
            }

            foreach (var entry in stocks)
            {
                var produceCount = Options.KeepStocked[entry.Key] - entry.Value;
                var item = entry.Key;
                if (produceCount > 0 && ItemProducers.ContainsKey(item))
                {
                    var producers = ItemProducers[item];
                    var pc = producers.Count;
                    if (pc == 0) continue;
                    foreach (var producer in producers)
                    {
                        var fraction = produceCount * (MyFixedPoint)(1d / pc--);
                        producer.AddQueueItem(item, fraction);
                        produceCount -= fraction;
                    }
                }
            }
        }

        public MyFixedPoint GetStocks(Dictionary<MyItemType, MyFixedPoint> items)
        {
            var result = new MyFixedPoint();
            var productionItems = new List<MyProductionItem>();
            var itemsKeys = items.Keys.ToArray();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(null, b =>
            {
                if (b.IsSameConstructAs(Me))
                {
                    for (int i = b.InventoryCount - 1; i >= 0; i--)
                    {
                        var inv = b.GetInventory(i);
                        foreach (var item in itemsKeys)
                            items[item] += inv.GetItemAmount(item);
                    }
                    if (b is IMyProductionBlock){
                        if (b is IMyAssembler)
                        {
                            var asm = b as IMyAssembler;
                            if (asm.Mode != MyAssemblerMode.Assembly || asm.CooperativeMode) return false;
                        }
                        // (b as IMyProductionBlock).GetQueue(productionItems);
                        // foreach (var item in productionItems)
                        // {
                        //     Console.WriteText($"[DEBUG] Producing {item.BlueprintId.TypeId}: {item.Amount}\n", true);
                        //     if (items.ContainsKey(item.))
                        //         items[item.BlueprintId] += item.Amount;
                        // }
                    }
                }
                return false;
            });
            return result;
        }

        public static MyInventoryItemFilter ParseFilter(string itemName)
        {
            if (itemName.StartsWith("$"))
            {
                MyInventoryItemFilter filter;
                if (ItemGroups.TryGetValue(itemName, out filter))
                    return filter;
                else throw new FormatException($"Invalid group '{itemName.Substring(1)}'");
            }
            bool allSubTypes = false;
            if (itemName.IndexOf('/') == -1)
            {
                allSubTypes = true;
                itemName += "/(null)";
            }

            MyDefinitionId defId;
            if (MyDefinitionId.TryParse($"MyObjectBuilder_{itemName}", out defId))
                return new MyInventoryItemFilter(defId, allSubTypes);
            else throw new FormatException($"Item (sub-)type '{itemName}' does not exist");
        }

        public static MyItemType ParseItem(string itemName)
        {
            MyDefinitionId defId;
            if (MyDefinitionId.TryParse($"MyObjectBuilder_{itemName}", out defId))
                return defId;
            else throw new FormatException($"Item subtype '{itemName}' does not exist");
        }

        public static bool ApplyFilter(MyInventoryItemFilter filter, MyItemType item)
        {
            return item.TypeId == filter.ItemType.TypeId
                && (filter.AllSubTypes || filter.ItemType.SubtypeId == item.SubtypeId);
        }
        public static Func<MyItemType, bool> ApplyFilter(MyInventoryItemFilter filter)
        {
            return i => ApplyFilter(filter, i);
        }

        private static bool AcceptsItemFor(IMyInventory inv, MyInventoryItemFilter filter)
        {
            bool accepts = false;
            inv.GetAcceptedItems(null, item => { accepts |= ApplyFilter(filter, item); return false; });
            return accepts;
        }