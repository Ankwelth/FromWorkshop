
		static class Variables
		{
			static Dictionary<string, object> v = new Dictionary<string, object> {
				{ "control-assemblers", new Variable<bool> { value = true, parser = s => s == "true" } },
				{ "restrict-scope", new Variable<bool> { value = false, parser = s => s == "true" } },
				{ "refill-docked-ship", new Variable<bool> { value = false, parser = s => s == "true" } },
				{ "control-refineries", new Variable<bool> { value = true, parser = s => s == "true" } }
			};
			public static void Set(string key, string value) { (v[key] as ISettable).Set(value); }
			public static void Set<T>(string key, T value) { (v[key] as ISettable).Set(value); }
			public static T Get<T>(string key) { return (v[key] as ISettable).Get<T>(); }
			public interface ISettable
			{
				void Set(string v);
				T1 Get<T1>();
				void Set<T1>(T1 v);
			}
			public class Variable<T> : ISettable
			{
				public T value;
				public Func<string, T> parser;
				public void Set(string v) { value = parser(v); }
				public void Set<T1>(T1 v) { value = (T)(object)v; }
				public T1 Get<T1>() { return (T1)(object)value; }
			}
		}

		bool pendingInitSequence;
		CommandRegistry commandRegistry;
		public class CommandRegistry
		{
			Dictionary<string, Action<string[]>> commands;
			public CommandRegistry(Dictionary<string, Action<string[]>> commands)
			{
				this.commands = commands;
			}
			public void RunCommand(string id, string[] cmdParts)
			{
				this.commands[id].Invoke(cmdParts);
			}
		}

		int runCount;
		void StartOfTick(string arg)
		{
			runCount++;

			if (pendingInitSequence && string.IsNullOrEmpty(arg))
			{
				pendingInitSequence = false;

				string cData = Me.CustomData;
				var parts = cData.Trim('\n').Split(new string[] { "CDATA Format=CSV" }, StringSplitOptions.None);
				if (parts.Length == 2)
					cData = parts[0].Trim('\n');

				arg = string.Join(",", cData.Trim('\n').Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(s => !s.StartsWith("//")).Select(s => "[" + s + "]"));
				SendFeedback(DateTime.Now.ToString("hh:mm:ss") + ": " + Me.CustomName + ": hello there, I've just got initialized. Have a great day and may the profitsssss be with you!", "", true);
			}

			if (!string.IsNullOrEmpty(arg) && arg.Contains(":"))
			{
				var commands = arg.Split(new[] { "],[" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim('[', ']')).ToList();
				foreach (var c in commands)
				{
					string[] cmdParts = c.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
					if (cmdParts[0] == "command")
					{
						this.commandRegistry.RunCommand(cmdParts[1], cmdParts);
					}
				}
			}
		}

		void EndOfTick()
		{
			FlushFeedbackBuffer();
		}

		void Ctor()
		{
			if (!string.IsNullOrEmpty(Me.CustomData))
				pendingInitSequence = true;

			E.Init(Echo, GridTerminalSystem);

			Action<IMyTextPanel> styleConfigurator1 =
										x =>
										{
											x.ContentType = ContentType.TEXT_AND_IMAGE;
											x.FontColor = new Color(r: 255, g: 195, b: 110);
											x.FontSize = 1f;
											x.Font = "Monospace"; // 51 maxCharsPerLine
										};

			logger = new RollingAppender(maxLinesCount: 12, maxCharsPerLine: 65);
			mWriter = new MultiscreenWriter(maxLinesCount: 18, maxCharsPerLine: 51, configurator: styleConfigurator1);

			this.commandRegistry = new CommandRegistry(
				new Dictionary<string, Action<string[]>>
					{
						{
							"set-value", (parts) => Variables.Set(parts[2], parts[3])
						},
						{
							"add-panel", (parts) => AddUniqueItem(GridTerminalSystem.GetBlockWithName(parts[2]) as IMyTextPanel, rawPanels)
						},
						{
							"add-multipanel", (parts) => mWriter.AddPanel(GridTerminalSystem.GetBlockWithName(parts[2]) as IMyTextPanel)
						},
						{
							"add-logger", (parts) =>
							{
								logger.AddPanel(GridTerminalSystem.GetBlockWithName(parts[2]) as IMyTextPanel);
								SendFeedback("Added new logger '" + parts[2] + "'", "", true);
							}
						},
						{
							"cycle-feedback-tag", (parts) =>
							{
								CycleFeedbackTag();
							}
						}
					}
				);
		}

		static void AddUniqueItem<T>(T item, IList<T> c) where T : class
		{
			if ((item != null) && !c.Contains(item))
				c.Add(item);
		}

		///////////////

		public static class Config
		{
			public const string Title = "==== RobCo Ахошник v2.1 ====\n";
			public const string PBOutputName = "PB AXO Multipanel";
			public const string DefaultMesssageCategoryName = "inventories";
			public const string PwrCategoryName = "grid power";
		}

		public class DesiredAmountDefinition
		{
			public string TypeIdentity { get; set; }
			public int Amount { get; set; }
			public MyDefinitionId HardcodedDefinition { get; set; }
			public float VolumeOfUnit { get; set; }
		}

		public class ComponentAvailability
		{
			public string ContentName { get; set; }
			public string DisplayName { get; set; }
			public MyFixedPoint CurrentAmount { get; set; }
			public int DesiredAmount { get; set; }
			public MyDefinitionId HardcodedDefinition { get; set; }
			public float VolumeOfUnit { get; set; }
			public ComponentAvailability Clone()
			{
				return (ComponentAvailability)MemberwiseClone();
			}
		}

		public List<ComponentAvailability> ComponentAvailabilities = new List<ComponentAvailability>();
		public void CountInventoryItem(MyInventoryItem item, List<ComponentAvailability> caList)
		{
			for (int i = 0; i < caList.Count; i++)
			{
				if (caList[i].ContentName == item.Type.TypeId + "/" + item.Type.SubtypeId)
				{
					caList[i].CurrentAmount += item.Amount;
					return;
				}
			}
			DesiredAmountDefinition des = DesiredComponentAmounts.FirstOrDefault(d => item.Type.SubtypeId.Contains(d.TypeIdentity));
			// https://forum.keenswh.com/threads/ingame-programming-missing-api-and-functions-and-known-issues.7358476/page-9#post-1287039973
			// var inferredDef = MyDefinitionId.FromContent(item.Content);
			var ca = new ComponentAvailability
			{
				// item.Content = VRage_blabla_MyObjectBuilder_BlueprintDefinition_Ore
				// item.Content.SubtypeName = Iron
				ContentName = item.Type.TypeId + "/" + item.Type.SubtypeId,
				DisplayName = AddTagToSubtypeNameHack(item),
				CurrentAmount = item.Amount
			};
			if (des != null)
			{
				ca.DesiredAmount = des.Amount;
				ca.HardcodedDefinition = des.HardcodedDefinition;
				ca.VolumeOfUnit = des.VolumeOfUnit;
			}
			caList.Add(ca);
		}


		public List<ComponentAvailability> accumulatedComponentAvailabilities = new List<ComponentAvailability>();
		public List<ComponentAvailability> prevComponentAvailabilities = new List<ComponentAvailability>();
		public double? CheckIfRising(string key, MyFixedPoint count)
		{
			foreach (var prevValue in prevComponentAvailabilities)
			{
				if (prevValue.ContentName == key)
				{
					if (prevValue.CurrentAmount == count)
						return null;
					return (double)(count - prevValue.CurrentAmount);
				}
			}
			return null;
		}

		Queue<List<ComponentAvailability>> historicalStats = new Queue<List<ComponentAvailability>>();
		DateTime lastPushStamp;
		public void PushPerMiniuteStat(List<ComponentAvailability> entry)
		{
			if ((lastPushStamp == default(DateTime) || (DateTime.Now - lastPushStamp).Minutes > 1))
			{
				lastPushStamp = DateTime.Now;
				historicalStats.Enqueue(new List<ComponentAvailability>(entry));
				if (historicalStats.Count == 60)
				{
					historicalStats.Dequeue();
				}
			}
		}

		List<DesiredAmountDefinition> DesiredComponentAmounts = new List<DesiredAmountDefinition>();

		public Program()
		{
			Runtime.UpdateFrequency = UpdateFrequency.Update10;
			gridBlocksCache = new GridBlocksCache(GridTerminalSystem, Me);

			Ctor();

			if (Variables.Get<bool>("control-assemblers"))
			{
				// https://forum.keenswh.com/threads/adding-needed-projector-bp-components-to-assembler.7396730/
				var parts = Me.CustomData.Trim('\n').Split(new string[] { "CDATA Format=CSV" }, StringSplitOptions.None);
				if (parts.Length == 2)
				{
					string[] defList = parts[1].Trim('\n').Split('\n');
					foreach (var def in defList)
					{
						var fields = def.Split(',');

						var dad = new DesiredAmountDefinition
						{
							TypeIdentity = fields[0].Trim(),
							HardcodedDefinition = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/" + fields[1].Trim()),
							Amount = int.Parse(fields[2].Trim()),
							VolumeOfUnit = float.Parse(fields[3].Trim())
						};
						DesiredComponentAmounts.Add(dad);
					}
				}
			}
		}

		private int blocksCacheExpiredIn = 0;
		GridBlocksCache gridBlocksCache;
		class GridBlocksCache
		{
			IMyGridTerminalSystem gts;
			IMyProgrammableBlock me;

			public Dictionary<string, List<IMyTerminalBlock>> GridGroupedBlocks = new Dictionary<string, List<IMyTerminalBlock>>();
			public List<IMyAssembler> Assemblers = new List<IMyAssembler>();
			public List<IMyRefinery> Refineries = new List<IMyRefinery>();
			public List<RefineryProxy> ManagedRefineries = new List<RefineryProxy>();
			public List<IMyTerminalBlock> AllContainers = new List<IMyTerminalBlock>();
			public Dictionary<string, List<IMyTerminalBlock>> TypedStorages = new Dictionary<string, List<IMyTerminalBlock>>();

			public GridBlocksCache(IMyGridTerminalSystem gts, IMyProgrammableBlock me)
			{
				this.gts = gts;
				this.me = me;
			}
			public void RePopulateBlocks(Func<IMyTerminalBlock, bool> selector = null)
			{
				GridGroupedBlocks.Clear();
				List<IMyTerminalBlock> allB = new List<IMyTerminalBlock>();
				gts.GetBlocksOfType(allB, selector);
				foreach (var myTerminalBlock in allB)
				{
					if (!GridGroupedBlocks.ContainsKey(myTerminalBlock.CubeGrid.CustomName))
					{
						GridGroupedBlocks.Add(myTerminalBlock.CubeGrid.CustomName, new List<IMyTerminalBlock> { myTerminalBlock });
					}
					else
					{
						GridGroupedBlocks[myTerminalBlock.CubeGrid.CustomName].Add(myTerminalBlock);
					}

					if (myTerminalBlock.CustomName.Contains("[axo"))
					{
						string cname = myTerminalBlock.CustomName;
						if (cname.Contains("drain"))
						{
							// tags form many-to-one relationship with container
							var typeTags = cname.Split('=')[1].TrimEnd(']').Split(',');
							foreach (var typeTag in typeTags)
							{
								if (!TypedStorages.ContainsKey(typeTag))
									TypedStorages.Add(typeTag, new List<IMyTerminalBlock> { myTerminalBlock });
								else if (!TypedStorages[typeTag].Contains(myTerminalBlock))
									TypedStorages[typeTag].Add(myTerminalBlock);
							}
						}
					}
				}

				gts.GetBlocksOfType(Assemblers, b => b.IsSameConstructAs(me));
				gts.GetBlocksOfType(AllContainers, b => b.IsSameConstructAs(me) && b.HasInventory);

				if (Variables.Get<bool>("control-refineries"))
				{
					gts.GetBlocksOfType(Refineries, b => b.IsSameConstructAs(me));
					Refineries.ForEach(r => {
						r.UseConveyorSystem = !Variables.Get<bool>("control-refineries");
						if (!ManagedRefineries.Any(x => x.Refinery == r))
						{
							ManagedRefineries.Add(new RefineryProxy(r, this));
						}
					});
				}
			}
		}

		
		class RefineryProxy
		{
			public IMyRefinery Refinery { get; set; }
			List<string> tags = new List<string>();
			public bool IsBusy { get; private set; }
			int currentConsideredTag = 0;
			int? currentRefiningTag;
			GridBlocksCache gbc;
			public RefineryProxy(IMyRefinery r, GridBlocksCache gbc)
			{
				string cname = r.CustomName;
				Refinery = r;
				this.gbc = gbc;
				if (cname.Contains("priority"))
				{
					tags = cname.Split('=')[1].TrimEnd(']').Split(',').ToList();
					var oreInv = Refinery.GetInventory(0);
					if (oreInv.ItemCount > 0)
					{
						var items = new List<MyInventoryItem>();
						oreInv.GetItems(items);
						foreach (var item in items)
						{
							PushItem(item, oreInv);
						}
					}
				}
			}
			public void ConsiderItem(MyInventoryItem item, IMyInventory src)
			{
				if ((item.Type.ToString().Contains("Ore")) && (!item.Type.ToString().Contains("Ice")))
				{
					if (IsBusy)
					{
						for (int n = 0; n < tags.Count; n++)
						{
							// push the low priority Ore out
							if (item.Type.ToString().Contains(tags[n]) && (currentRefiningTag.HasValue) && (n < currentRefiningTag.Value))
							{
								var ore = Refinery.GetInventory(0).GetItemAt(0);
								if (ore.HasValue)
								{
									PushItem(ore.Value, Refinery.GetInventory(0));
								}
							}
						}
					}
					else
					{
						bool result = TryMoveItem(item, src);

						/*
							// True => Large Cargo Container 3 [axo.drain=Ore,Ingot] => MyObjectBuilder_Ore/Ice
							// Fuck you Keen, just fuck you
									if (item.Type.ToString().Contains("Ice"))
										E.Log(result + " => " + (src.Owner as IMyTerminalBlock)?.CustomName + " => " + item.Type.ToString());
						*/
						if (item.Type.ToString().Contains("Ice"))
							result = false;

						IsBusy = result;
						if (!result)
						{
							rejectedAtLeastOnce = true;
							rejectedDescr = "Tried movin " + item.Amount + " " + item.Type.SubtypeId + " from " + (src.Owner as IMyTerminalBlock)?.CustomName;
						}
						else
						{
							if (tags.Count > 0)
							{
								currentRefiningTag = currentConsideredTag;
								currentConsideredTag = 0;
							}
						}
					}
				}
			}
			bool TryMoveItem(MyInventoryItem item, IMyInventory src)
			{
				if (tags.Count == 0)
					return src.TransferItemTo(Refinery.GetInventory(0), item);
				else if (item.Type.ToString().Contains(tags[currentConsideredTag]))
					return src.TransferItemTo(Refinery.GetInventory(0), item);
				return false;
			}

			public void UpdateBeforeIterator()
			{
				//IsBusy = Refinery.GetInventory(0).ItemCount > 0;
				rejectedAtLeastOnce = false;
				rejectedDescr = null;
				if (!IsBusy)
					currentRefiningTag = null;
			}
			bool rejectedAtLeastOnce;
			string rejectedDescr;
			public void UpdateAfterIterator()
			{
				IsBusy = Refinery.GetInventory(0).ItemCount > 0;
				if (!IsBusy && rejectedAtLeastOnce && (tags.Count() > 0))
				{
					currentConsideredTag++;
					if (currentConsideredTag == tags.Count)
						currentConsideredTag = 0;
				}
				var outputInv = Refinery.GetInventory(1);
				if (outputInv.ItemCount > 0)
				{
					var items = new List<MyInventoryItem>();
					outputInv.GetItems(items);
					foreach (var item in items)
					{
						PushItem(item, outputInv);
					}
				}
			}
			public void PushItem(MyInventoryItem item, IMyInventory outputInv)
			{
				// send type to the least occupied container from the list tied to this type

				if (gbc.TypedStorages.Any(ts => item.Type.ToString().Contains(ts.Key)))
				{
					var dest = gbc.TypedStorages.First(ts => item.Type.ToString().Contains(ts.Key)).Value
						.OrderBy(s => (float)s.GetInventory(0).CurrentVolume).First();
					var inv = dest.GetInventory(0);
					if ((inv != null) && inv.CanItemsBeAdded(item.Amount, item.Type) && inv.IsConnectedTo(outputInv))
						outputInv.TransferItemTo(inv, item); // laggy af, up to 40ms for 30 refineries
				}
				else
				{
					E.Echo(gbc.AllContainers.Where(c => c is IMyCargoContainer && c.IsSameConstructAs(Refinery)).Cast<IMyCargoContainer>()
						.Select(c => c.GetInventory(0)).Where(c => c.IsConnectedTo(outputInv) && c.CanItemsBeAdded(item.Amount, item.Type)).Count().ToString());
					var destInv = gbc.AllContainers.Where(c => c is IMyCargoContainer).Cast<IMyCargoContainer>()
						.Select(c => c.GetInventory(0)).Where(c => c.IsConnectedTo(outputInv) && c.CanItemsBeAdded(item.Amount, item.Type))
						.OrderByDescending(c => (float)c.MaxVolume).OrderBy(c => (float)c.CurrentVolume).FirstOrDefault();
					if (destInv != null)
					{
						//E.Echo((destInv.Owner as IMyTerminalBlock).CustomName + ", " + item.Type.TypeId);
						//E.Echo(destInv.CurrentVolume + "/" + destInv.MaxVolume + ", " + ((float)item.Amount).ToString());
						outputInv.TransferItemTo(destInv, item); // extremely fucking laggy af, up to 240ms for 30 refineries
					}
				}
			}
			public override string ToString()
			{
				string res = Refinery.CustomName + (IsBusy? " is busy" : " is chillin") + "\n";
				if (Refinery.GetInventory(0).ItemCount > 0)
				{
					List<MyInventoryItem> items = new List<MyInventoryItem>();
					Refinery.GetInventory(0).GetItems(items);
					foreach (var i in items)
					{
						res += "....Item: " + i.Type.SubtypeId + "\n";
					}
				}
				res += "....Has just rejected an item: " + rejectedAtLeastOnce + "\n";
				if (!string.IsNullOrEmpty(rejectedDescr))
					res += "...." + rejectedDescr + "\n";
				res += "....Tags: " + string.Join(", ", tags) + "\n";
				if (tags.Count > 0)
				{
					res += "....currentConsideredTag: " + tags[currentConsideredTag] + "\n";
					res += "....currentRefiningTag: " + (currentRefiningTag.HasValue ? tags[currentRefiningTag.Value] : "-") + "\n";
				}
				return res;
			}
		}

		MyFixedPoint spaceNominal = new MyFixedPoint();
		MyFixedPoint spaceOccupied = new MyFixedPoint();
		MyFixedPoint cargoMass = new MyFixedPoint();
		MyFixedPoint spaceNominalLast = new MyFixedPoint();
		MyFixedPoint spaceOccupiedLast = new MyFixedPoint();
		MyFixedPoint cargoMassLast = new MyFixedPoint();

		void Main(string param, UpdateType updateType)
		{
			var _start = DateTime.Now;
			total = 0;

			StartOfTick(param);

			if (this.blocksCacheExpiredIn == 0)
			{
				this.blocksCacheExpiredIn = 10;
				if (Variables.Get<bool>("restrict-scope"))
					gridBlocksCache.RePopulateBlocks(b => b.IsSameConstructAs(Me));
				else
					gridBlocksCache.RePopulateBlocks();
			}
			Echo("Block cache expired in " + this.blocksCacheExpiredIn--);

			SendFeedback(Config.Title + " PWR STAT " + this.RandomSpinner(), Config.PwrCategoryName);
			if (tags[currentTagIndex] == Config.PwrCategoryName)
			{
				foreach (var gridGroup in gridBlocksCache.GridGroupedBlocks)
				{
					var sb = new StringBuilder();
					sb.AppendLine(string.Format("GRID \"{0}\"", gridGroup.Key));

					var pwrProducers = gridGroup.Value.Where(b => b is IMyPowerProducer).Cast<IMyPowerProducer>();
					float currR = 0;
					float maxR = 0;
					Dictionary<string, float> groupedPowerProducers = new Dictionary<string, float>();
					foreach (var p in pwrProducers)
					{
						var type = p.GetType().Name;
						if (!groupedPowerProducers.ContainsKey(type))
							groupedPowerProducers.Add(type, 0);
						groupedPowerProducers[type] += p.CurrentOutput;

						currR += p.CurrentOutput;
						maxR += p.MaxOutput;
					}

					sb.AppendLine("..Producers breakdown: ");
					foreach (var gp in groupedPowerProducers)
						sb.AppendLine(string.Format("....{0}: {1:F1} MW", gp.Key, gp.Value));

					float storedPower = 0;
					float maxPower = 0;
					var batteries = gridGroup.Value.Where(b => b is IMyBatteryBlock).Cast<IMyBatteryBlock>();
					foreach (var b in batteries)
					{
						currR += b.CurrentOutput;
						maxR += b.MaxOutput;
						maxPower += b.MaxStoredPower;
						storedPower += b.CurrentStoredPower;
					}

					if (maxR > 0)
						sb.AppendLine(string.Format("..Output capability: {0:F1}/{1:F1}", currR, maxR));

					if (maxPower > 0)
						sb.AppendLine(string.Format("..Stored power: {0:F1}/{1:F1}", storedPower, maxPower));

					var totals = this.GetPwrTotalsForBlocks(gridGroup.Value, "Required Input");
					foreach (var n in this.GetPwrTotalsForBlocks(gridGroup.Value, "Current Input"))
					{
						if (!totals.ContainsKey(n.Key))
							totals.Add(n.Key, n.Value);
						else
							totals[n.Key] += n.Value;
					}
					sb.AppendLine("..Consumers breakdown: ");
					float gridTotal = 0;
					foreach (var f in totals)
					{
						sb.AppendLine("...." + f.Key + ": " + FormatNumberToNeatString(f.Value, "W"));
						gridTotal += f.Value;
					}
					if (gridTotal > 0)
					{
						sb.AppendLine("..Total required input: " + (gridTotal / 1000000f).ToString("F3") + " MW\n");
						SendFeedback(sb.ToString(), Config.PwrCategoryName);
					}
				}
			}
			//////////////////////////
			SendFeedback(Config.Title + "INV STAT " + this.RandomSpinner(), Config.DefaultMesssageCategoryName);
			SendFeedback(Config.Title + " INV STAT C.I.O." + this.RandomSpinner(), "inv cio");

			Echo("Found " + gridBlocksCache.AllContainers.Count + " inventories");

			TimeStampProfile(ref _start, "Grid power stats");
			
			if (Variables.Get<bool>("control-refineries"))
			{
				gridBlocksCache.ManagedRefineries.ForEach(r => r.UpdateBeforeIterator());
			}
			
			//IterateInventories(ref spaceNominal, ref spaceOccupied, ref cargoMass, ref _start);
			bool updateComplete = IterateInventoriesDelayed(_start);

			TimeStampProfile(ref _start, "CountInventoryItems");

			if (Variables.Get<bool>("control-refineries"))
			{
				//gridBlocksCache.ManagedRefineries.ForEach(r => r.UpdateAfterIterator());
				int index = runCount % gridBlocksCache.ManagedRefineries.Count;
				gridBlocksCache.ManagedRefineries[index].UpdateAfterIterator();
				Echo($"Flushing {gridBlocksCache.ManagedRefineries[index].Refinery.CustomName} [{index}]");
				/*
				for (int i = 0; i < gridBlocksCache.ManagedRefineries.Count; i++)
				{
					if (runCount % gridBlocksCache.ManagedRefineries.Count == i)
					{
						gridBlocksCache.ManagedRefineries[i].UpdateAfterIterator();
						Echo($"Flushing {gridBlocksCache.ManagedRefineries[i].Refinery.CustomName} [{i}]");
					}
				}
				*/
			}
			
			if (Variables.Get<bool>("control-refineries"))
			{
				foreach (var mr in gridBlocksCache.ManagedRefineries)
				{
					SendFeedback(mr.ToString(), "managed refineries");
				}
			}

			TimeStampProfile(ref _start, "ManagedRefineries");

			List<string> statEntries = new List<string>();
			//var workingSet = updateComplete ? 
			for (int i = 0; i < accumulatedComponentAvailabilities.Count; i++)
			{
				var currentEntry = accumulatedComponentAvailabilities[i];

				//if (currentEntry.ContentName.Contains("Ingot"))
				if (true)
				{
					string msg = String.Format(
						"{0}: {1}",
						currentEntry.DisplayName,
						FormatNumberToNeatString((float)currentEntry.CurrentAmount));
					if (currentEntry.DesiredAmount > 0)
						msg = OverwriteAtStringPos(msg, " (" + FormatNumberToNeatString(currentEntry.DesiredAmount) + ")", 27);
					double? diff = CheckIfRising(currentEntry.ContentName, currentEntry.CurrentAmount);
					if (diff.HasValue)
					{
						msg += (diff > 0) ? " ^" : " v";
						msg = OverwriteAtStringPos(msg, FormatNumberToNeatString((float)diff.Value), 35);
					}
					if (historicalStats.Count > 0)
					{
						var h = historicalStats.Peek().FirstOrDefault(hs => hs.ContentName == currentEntry.ContentName);
						if (h != null)
						{
							var d = (float)(currentEntry.CurrentAmount - h.CurrentAmount);
							if (d != 0)
								msg = OverwriteAtStringPos(msg, FormatNumberToNeatString(d), 43);
						}
					}
					statEntries.Add(msg);
				}
				

				if (Variables.Get<bool>("control-assemblers"))
				{
					var ass = gridBlocksCache.Assemblers.FirstOrDefault(a => a.IsQueueEmpty);
					if (ass != null)
					{
						if (currentEntry.CurrentAmount < currentEntry.DesiredAmount)
						{
							// check if it's already being produced
							bool alreadyHandled = false;
							Echo("checking if it's already being produced - " + currentEntry.ContentName);
							foreach (var assB in gridBlocksCache.Assemblers.Where(a => !a.IsQueueEmpty))
							{
								List<MyProductionItem> producing = new List<MyProductionItem>();
								assB.GetQueue(producing);
								if (producing.Any(item => item.BlueprintId == currentEntry.HardcodedDefinition))
									alreadyHandled = true;
							}

							// NRE here, what the fuck, Keen?
							// https://forum.keenswh.com/threads/ingame-programming-missing-api-and-functions-and-known-issues.7358476/page-9#post-1287039973
							if (!alreadyHandled)
							{
								try
								{
									ass.AddQueueItem(currentEntry.HardcodedDefinition, currentEntry.DesiredAmount - currentEntry.CurrentAmount);
									SendFeedback(DateTime.Now.ToString("hh:mm:ss") + ": " + Me.CustomName + ": " + ass.CustomName + " queued " + currentEntry.DisplayName + " for production", "", true);
								}
								catch (Exception ex)
								{
									SendFeedback(DateTime.Now.ToString("hh:mm:ss") + ": " + Me.CustomName + ": "
										+ ass.CustomName + " has failed at understanding current items' definition: "
										+ currentEntry.HardcodedDefinition.ToString(), "", true);
								}
							}
						}
					}
					// flush every manufactured stack (PUSH or whatever)
					foreach (var myAssembler in gridBlocksCache.Assemblers)
					{
						IMyInventory assOutput = myAssembler.GetInventory(1);
						for (int s = 0; s < assOutput.ItemCount; s++)
						{
							// trying to push to every storage container until success
							for (int t = 0; t < gridBlocksCache.AllContainers.Count; t++)
							{
								if (assOutput.TransferItemTo(gridBlocksCache.AllContainers[t].GetInventory(0), s, stackIfPossible: true))
									break;
							}
						}
					}
				}

				
			}

			if (updateComplete)
			{
				prevComponentAvailabilities.Clear();
				prevComponentAvailabilities = accumulatedComponentAvailabilities.Select(ca => ca.Clone()).ToList();

				accumulatedComponentAvailabilities.Clear();
				accumulatedComponentAvailabilities = ComponentAvailabilities.Select(ca => ca.Clone()).ToList();
				PushPerMiniuteStat(accumulatedComponentAvailabilities);
				ComponentAvailabilities.ForEach(ca => ca.CurrentAmount = 0);

				cargoMassLast = cargoMass;
				spaceNominalLast = spaceNominal;
				spaceOccupiedLast = spaceOccupied;
			}

			Echo("Stats history: " + historicalStats.Count());

			TimeStampProfile(ref _start, "Assembler controls");

			if (Variables.Get<bool>("refill-docked-ship"))
				SendFeedback(CheckDockedShip() + "\n", Config.DefaultMesssageCategoryName);

			TimeStampProfile(ref _start, "CheckDockedShip");

			SendFeedback($"Refresh stats:\n {GetProgressBarString(gridBlocksCache.AllContainers.Count, currentIndex)} (inventory {currentIndex}/{gridBlocksCache.AllContainers.Count})",
						Config.DefaultMesssageCategoryName + ",inv cio");

			SendFeedback(string.Format("Current volume:\n {0} {1} of {2}",
					this.GetProgressBarString((float)spaceNominalLast, (float)spaceOccupiedLast),
					((float)spaceOccupiedLast).ToString("F1"),
					((float)spaceNominalLast).ToString("F1")),
						Config.DefaultMesssageCategoryName + ",inv cio");

			SendFeedback(string.Format("Current cargo mass: {0}", ((float)cargoMassLast).ToString("F1")),
					Config.DefaultMesssageCategoryName + ",inv cio");

			foreach (var assembler in gridBlocksCache.Assemblers.Where(ass => !ass.IsQueueEmpty))
			{
				List<MyProductionItem> producing = new List<MyProductionItem>();
				assembler.GetQueue(producing);
				// are we producing any of that configured items?
				var first = producing.First();
				if (accumulatedComponentAvailabilities.Any(c => c.HardcodedDefinition == first.BlueprintId))
				{
					SendFeedback(assembler.CustomName + " is producing " 
						+ accumulatedComponentAvailabilities.First(c => c.HardcodedDefinition == first.BlueprintId).DisplayName, Config.DefaultMesssageCategoryName + ",inv cio");
				}
			}
			SendFeedback("=================================", Config.DefaultMesssageCategoryName + ",inv cio");
			SendFeedback(OverwriteAtStringPos(OverwriteAtStringPos(OverwriteAtStringPos("type", "expect", 27), "diff", 35), "hour", 43), Config.DefaultMesssageCategoryName + ",inv cio");
			SendFeedback("=================================", Config.DefaultMesssageCategoryName + ",inv cio");

			if (tags[currentTagIndex] == "inv cio")
			{
				SendFeedback("____Ores & Ingots________________", "inv cio");
				SendFeedback("=================================", "inv cio");
				foreach (string statEntry in statEntries.Where(s => s.Contains("Ore") || s.Contains("Ingot")).OrderBy(s => s))
				{
					SendFeedback(statEntry, "inv cio");
				}

				SendFeedback("____Components___________________", "inv cio");
				SendFeedback("=================================", "inv cio");
				foreach (string statEntry in statEntries.Where(s => accumulatedComponentAvailabilities.Any(ca => s.Contains(ca.DisplayName) && (ca.ContentName).Contains("Component"))).OrderBy(s => s))
				{
					SendFeedback(statEntry, "inv cio");
				}
			}

			if (tags[currentTagIndex] == Config.DefaultMesssageCategoryName)
			{
				foreach (string statEntry in statEntries.OrderBy(s => s))
				{
					SendFeedback(statEntry, Config.DefaultMesssageCategoryName);
				}
			}



			EndOfTick();

			TimeStampProfile(ref _start, "Feedback handling");

			Echo("Total: " + total.ToString("F5") + " ms");
			Echo("Runtime.LastRunTimeMs: " + Runtime.LastRunTimeMs.ToString("F5") + " ms");
		}

		IEnumerator<bool> invIterState;
		DateTime invIterStamp;
		int currentIndex;
		bool IterateInventoriesDelayed(DateTime stamp)
		{
			invIterStamp = stamp;
			if (invIterState != null)
			{
				if (!invIterState.MoveNext())
				{
					invIterState.Dispose();
					invIterState = null;
					return true;
				}
			}
			else
			{
				spaceNominal = 0;
				spaceOccupied = 0;
				cargoMass = 0;
				currentIndex = 0;
				invIterState = IterateInventoriesEnumerator();
			}
			return false;
		}

		double GetElapsedMs()
		{
			return (DateTime.Now - invIterStamp).TotalMilliseconds;
		}

		IEnumerator<bool> IterateInventoriesEnumerator()
		{
			for (int i = 0; i < gridBlocksCache.AllContainers.Count; i++)
			{
				currentIndex = i;
				if (GetElapsedMs() > 0.05)
					yield return true;

				var inv = gridBlocksCache.AllContainers[i].GetInventory(0);
				if (inv == null)
					continue;
				var items = new List<MyInventoryItem>();
				inv.GetItems(items);
				spaceNominal += inv.MaxVolume;
				spaceOccupied += inv.CurrentVolume;
				cargoMass += inv.CurrentMass;
				if (gridBlocksCache.AllContainers[i] is IMyProductionBlock)
				{
					inv = gridBlocksCache.AllContainers[i].GetInventory(1);
					var items2 = new List<MyInventoryItem>();
					inv.GetItems(items2);
					items.AddRange(items2);
					spaceNominal += inv.MaxVolume;
					spaceOccupied += inv.CurrentVolume;
					cargoMass += inv.CurrentMass;
				}
				for (int n = 0; n < items.Count; n++)
				{
					CountInventoryItem(items[n], ComponentAvailabilities);

					Func<IMyTerminalBlock, bool> invTypeIsAllowed = b => (!(b is IMyProductionBlock) && !(b is IMyGasGenerator) && !(b is IMyLargeTurretBase)
							&& !(b is IMySmallGatlingGun) && !(b is IMySmallMissileLauncherReload) && !(b is IMySmallMissileLauncher) && !(b is IMyShipWelder));

					if (invTypeIsAllowed(gridBlocksCache.AllContainers[i]) && !(gridBlocksCache.AllContainers[i].CustomName.Contains("[axo.ignore]")))
					{
						if (Variables.Get<bool>("control-refineries"))
						{
							gridBlocksCache.ManagedRefineries.ForEach(r => r.ConsiderItem(items[n], inv));
						}
						if (!gridBlocksCache.AllContainers[i].CustomName.Contains("axo.drain"))
						{
							// send type to the least occupied container from the list tied to this type
							if (gridBlocksCache.TypedStorages.Any(ts => items[n].Type.ToString().Contains(ts.Key)))
								inv.TransferItemTo(gridBlocksCache.TypedStorages.First(ts => items[n].Type.ToString().Contains(ts.Key)).Value
									.OrderBy(s => (float)s.GetInventory(0).CurrentVolume).First().GetInventory(0), items[n]);
						}
						
						if (GetElapsedMs() > 0.05)
							yield return true;
					}

					//gridBlocksCache.typedStorages["Ingot"].First().CustomData += items[n].Type.ToString() + "///TypeId////" + items[n].Type.TypeId + "//////SubtypeId//////" + items[n].Type.SubtypeId + "\n";
				}
			}
			yield return true;
		}

		void IterateInventories(ref MyFixedPoint spaceNominal, ref MyFixedPoint spaceOccupied, ref MyFixedPoint cargoMass, ref DateTime stamp)
		{
			spaceNominal = 0;
			spaceOccupied = 0;
			cargoMass = 0;
			for (int i = 0; i < gridBlocksCache.AllContainers.Count; i++)
			{
				var inv = gridBlocksCache.AllContainers[i].GetInventory(0);
				if (inv == null)
					continue;
				var items = new List<MyInventoryItem>();
				inv.GetItems(items);
				spaceNominal += inv.MaxVolume;
				spaceOccupied += inv.CurrentVolume;
				cargoMass += inv.CurrentMass;
				if (gridBlocksCache.AllContainers[i] is IMyProductionBlock)
				{
					inv = gridBlocksCache.AllContainers[i].GetInventory(1);
					var items2 = new List<MyInventoryItem>();
					inv.GetItems(items2);
					items.AddRange(items2);
					spaceNominal += inv.MaxVolume;
					spaceOccupied += inv.CurrentVolume;
					cargoMass += inv.CurrentMass;
				}
				for (int n = 0; n < items.Count; n++)
				{
					CountInventoryItem(items[n], ComponentAvailabilities);

					Func<IMyTerminalBlock, bool> invTypeIsAllowed = b => (!(b is IMyProductionBlock) && !(b is IMyGasGenerator) && !(b is IMyLargeTurretBase)
							&& !(b is IMySmallGatlingGun) && !(b is IMySmallMissileLauncherReload) && !(b is IMySmallMissileLauncher) && !(b is IMyShipWelder));

					if (invTypeIsAllowed(gridBlocksCache.AllContainers[i]) && !(gridBlocksCache.AllContainers[i].CustomName.Contains("[axo.ignore]")))
					{
						if (Variables.Get<bool>("control-refineries"))
						{
							gridBlocksCache.ManagedRefineries.ForEach(r => r.ConsiderItem(items[n], inv));
						}

						if (!gridBlocksCache.AllContainers[i].CustomName.Contains("axo.drain"))
						{
							// send type to the least occupied container from the list tied to this type
							if (gridBlocksCache.TypedStorages.Any(ts => items[n].Type.ToString().Contains(ts.Key)))
								inv.TransferItemTo(gridBlocksCache.TypedStorages.First(ts => items[n].Type.ToString().Contains(ts.Key)).Value
									.OrderBy(s => (float)s.GetInventory(0).CurrentVolume).First().GetInventory(0), items[n]);
						}
					}

					//gridBlocksCache.typedStorages["Ingot"].First().CustomData += items[n].Type.ToString() + "///TypeId////" + items[n].Type.TypeId + "//////SubtypeId//////" + items[n].Type.SubtypeId + "\n";
				}
			}
		}

		private Dictionary<string, float> GetPwrTotalsForBlocks(List<IMyTerminalBlock> allBlocks, string parName)
		{
			Dictionary<string, float> pwrConsumerTotals = new Dictionary<string, float>();
			foreach (var b in allBlocks)
			{
				if (b.DetailedInfo.Contains(parName))
				{
					/*
                    float val;
                    if (float.TryParse(
                        System.Text.RegularExpressions.Regex.Match(b.DetailedInfo, @"(?<=" + parName + @"\s?:\s?)\d+(\.\d{1,2})?").Value.Replace('.', ','),
                        out val))
                    {
                        string cat = b.GetType().ToString().Split('.').Last();
                        if (!pwrConsumerTotals.ContainsKey(cat))
                            pwrConsumerTotals.Add(cat, val);
                        else
                            pwrConsumerTotals[cat] += val;
                    }*/

					string _pat = @"(?<=\n" + parName + @"\s?:\s?)(?<dig>\d+(\.\d{1,2})?)(?<w>\s.{2})?";
					System.Text.RegularExpressions.Match res = System.Text.RegularExpressions.Regex.Match(b.DetailedInfo, _pat);
					if (res.Success)
					{
						if (res.Groups["dig"].Success)
						{
							float value;
							var separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
							string num = res.Groups["dig"].Value.Replace(".", separator);
							//Echo(b.CustomName + " - " + res.Value + " str- " + num);
							if (float.TryParse(num,
									System.Globalization.NumberStyles.AllowDecimalPoint,
									System.Globalization.CultureInfo.InvariantCulture, out value))
							{
								//Echo(b.CustomName + " - " + res.Value + " val- " + value.ToString("F1"));

								if (res.Groups["w"] != null)
								{
									string kinda = res.Groups["w"].Value.Trim();
									if (string.Equals(kinda, "kw", StringComparison.OrdinalIgnoreCase))
									{
										value *= 1000;
									}
									else if (string.Equals(kinda, "mw", StringComparison.OrdinalIgnoreCase))
									{
										value *= 1000000;
									}
								}
								string cat = b.GetType().Name;
								if (!pwrConsumerTotals.ContainsKey(cat))
								{
									pwrConsumerTotals.Add(cat, value);
								}
								else
								{
									pwrConsumerTotals[cat] += value;
								}
							}
						}
					}
				}
			}
			return pwrConsumerTotals;
		}

		private string AddTagToSubtypeNameHack(MyInventoryItem item)
		{
			//var stn = MyDefinitionId.FromContent(item.Content).SubtypeName;
			var stn = item.Type.SubtypeId;
			
			if (item.Type.ToString().Contains("Ore"))
			{
				return stn + " (Ore)";
			}
			if (item.Type.ToString().Contains("Ingot"))
			{
				return stn + " (Ingot)";
			}

			return stn;
		}

		string FormatNumberToNeatString(float value, string measure = "")
		{
			string valueString;
			if (Math.Abs(value) >= 1000000)
			{
				if (!string.IsNullOrEmpty(measure))
					valueString = string.Format("{0:0.##} M{1}", value / 1000000, measure);
				else
					valueString = string.Format("{0:0.##}M", value / 1000000);
			}
			else if (Math.Abs(value) >= 1000)
			{
				if (!string.IsNullOrEmpty(measure))
					valueString = string.Format("{0:0.##} k{1}", value / 1000, measure);
				else
					valueString = string.Format("{0:0.##}k", value / 1000);
			}
			else
			{
				if (!string.IsNullOrEmpty(measure))
					valueString = string.Format("{0:0.##} {1}", value, measure);
				else
					valueString = string.Format("{0:0.##}", value);
			}
			return valueString;
		}

		public string CheckDockedShip()
		{
			Func<IMyTerminalBlock, bool> checkGrid = b => Me.CubeGrid == b.CubeGrid;
			List<IMyProgrammableBlock> pebes = new List<IMyProgrammableBlock>();
			GridTerminalSystem.GetBlocksOfType(pebes, b => !checkGrid(b) && (b.CustomName.Contains("drone")));
			var pbMain = pebes.FirstOrDefault();
			if (pbMain != null)
			{
				List<IMyTerminalBlock> shipContainers = new List<IMyTerminalBlock>();
				GridTerminalSystem.GetBlocksOfType<IMyInventoryOwner>(shipContainers, b => b.CubeGrid == pbMain.CubeGrid && b is IMyCargoContainer);
				var spaceNominal = new VRage.MyFixedPoint();
				var spaceOccupied = new VRage.MyFixedPoint();
				var cargoMass = new VRage.MyFixedPoint();
				for (int i = 0; i < shipContainers.Count; i++)
				{
					var inv = shipContainers[i].GetInventory(0);
					var items = new List<MyInventoryItem>();
					inv.GetItems(items);
					spaceNominal += inv.MaxVolume;
					spaceOccupied += inv.CurrentVolume;
					cargoMass += inv.CurrentMass;
				}
				var freeVol = (spaceNominal - spaceOccupied) * 1000;
				float neededSpace = 0;
				float debuffer = 1f;
				if (freeVol > 10)
				{
					var shipCA = ComponentAvailabilities.Select(ca => ca.Clone()).ToList();
					shipCA.ForEach(ca => ca.CurrentAmount = 0);
					for (int i = 0; i < shipContainers.Count; i++)
					{
						var inv = shipContainers[i].GetInventory(0);
						List<MyInventoryItem> items = new List<MyInventoryItem>();
						inv.GetItems(items);
						for (int n = 0; n < items.Count; n++)
						{
							CountInventoryItem(items[n], shipCA);
						}
					}
					foreach (var ca in shipCA)
					{
						/*
						var diff = ca.DesiredAmount - (int)ca.CurrentAmount;
						if (diff > 0)
							neededSpace += (ca.DesiredAmount - (int)ca.CurrentAmount) * ca.VolumeOfUnit;*/
						neededSpace += ca.DesiredAmount * ca.VolumeOfUnit;
						freeVol += ca.CurrentAmount * ca.VolumeOfUnit;
					}
					if (neededSpace > 0)
					{
						debuffer = (float)freeVol / neededSpace;
						List<IMyTerminalBlock> allContainers = new List<IMyTerminalBlock>();
						GridTerminalSystem.GetBlocksOfType<IMyInventoryOwner>(allContainers, b => b.CubeGrid == Me.CubeGrid);
						StringBuilder sb = new StringBuilder();
						sb.AppendLine("debuffer: " + debuffer);
						foreach (var ca in shipCA)
						{
							sb.AppendLine(ca.DisplayName + ": " + ca.CurrentAmount + " DesiredAmount: " + ca.DesiredAmount + " correctedAmount: " + ((int)(ca.DesiredAmount * debuffer) - (int)ca.CurrentAmount));
							for (int i = 0; i < allContainers.Count; i++)
							{
								var inv = allContainers[i].GetInventory(0);
								List<MyInventoryItem> items = new List<MyInventoryItem>();
								inv.GetItems(items);
								for (int n = 0; n < items.Count; n++)
								{
									//if (ca.ContentName == MyDefinitionId.FromContent(items[n].Content).SubtypeName + "/" + (items[n].Content))
									if (ca.ContentName == (items[n].Type + "/" + items[n].Type.SubtypeId))
									{
										int correctedAmount = (int)(ca.DesiredAmount * debuffer) - (int)ca.CurrentAmount;
										if (correctedAmount > 0)
										{
											var amountToSend = correctedAmount > items[n].Amount ? items[n].Amount : correctedAmount;
											bool result = inv.TransferItemTo(shipContainers[0].GetInventory(), n, stackIfPossible: true, amount: amountToSend);
											if (result)
											{
												SendFeedback(ca.DisplayName + " transferred " + ((int)amountToSend).ToString("F1"), "", true);
												ca.CurrentAmount += (int)amountToSend;
											}
										}
									}
								}
							}
						}
						// command:set-vectors-docking:1060538.34354488:93219.342056705:1592400.99327744:0:0:0:-0.870105987700538:-0.428052845506658:-0.244307862381308:0:0.117772537183434:-0.66190498075107:0.74027793830581:0:-0.478586668904439:0.615347509875987:0.626340356704224:0:1060538.34354488:93219.342056705:1592400.99327744:1

						List<IMyShipConnector> docks = new List<IMyShipConnector>();
						GridTerminalSystem.GetBlocksOfType(docks, d => d.Status == MyShipConnectorStatus.Connected && (d.CubeGrid != pbMain.CubeGrid));
						var docka = docks.OrderBy(d => (d.GetPosition() - pbMain.GetPosition()).Length()).FirstOrDefault();
						if (docka != null)
						{
							SendFeedback("Dock pushes vectors to ship", "", true);
							var msg1 = "[command:recycle]";
							var msg2 = string.Format("[command:set-vectors-docking:{0}:{1}:{2}]", VectorOpsHelper.V3DtoBroadcastString(docka.GetPosition(), new Vector3D()),
								VectorOpsHelper.MtrDtoBroadcastString(docka.WorldMatrix),
								VectorOpsHelper.V3DtoBroadcastString(Me.WorldAABB.Min, Me.WorldAABB.Max));
							var msg3 = "[command:rwp-exec]";
							var cmd = string.Join(",", msg1, msg2, msg3);
							if (pbMain.TryRun(cmd))
							{
								SendFeedback("Dock pushes vectors to ship", "", true);
								docka.Disconnect();
							}
						}
						
					}
				}
				string res = pbMain.CubeGrid.CustomName + " is docked, cargo space: " + ((float)spaceOccupied).ToString("F1") + "/" + ((float)spaceNominal).ToString("F1");
				res += "\nNeeded space: " + neededSpace.ToString("F1") + " debuffer: " + debuffer.ToString("F5");
				return res;
			}
			return "No docked ships";
		}

		string[] frames = new[] { "\\", "|", "/", "--" };
		int frameIndex;
		private string RandomSpinner()
		{
			this.frameIndex = (++frameIndex >= frames.Length) ? 0 : frameIndex++;
			return this.frames[frameIndex];
		}

		private string GetProgressBarString(float max, float current)
		{
			StringBuilder sb = new StringBuilder();
			if (current <= float.Epsilon)
				current = 1;
			float pecentage = current / max;
			const int TotalCharsCount = 20;
			sb.Append('[');
			for (int i = 0; i < TotalCharsCount; i++)
			{
				sb.Append((i < TotalCharsCount * pecentage) ? 'l' : ' ');
			}
			sb.Append(']');
			return sb.ToString();
		}

		private string OverwriteAtStringPos(string src, string val, int pos)
		{
			return src.PadRight(pos) + val;
		}

		double total;
		private double TimeStampProfile(ref DateTime d, string tag = null)
		{
			var elapsed = (DateTime.Now - d).TotalMilliseconds;
			d = DateTime.Now;
			Echo((tag ?? "") + " " + elapsed.ToString("F5") + " ms");
			total += elapsed;
			return elapsed;
		}

		public static class VectorOpsHelper
		{
			public static string V3DtoBroadcastString(params Vector3D[] vectors)
			{
				return string.Join(":", vectors.Select(v => string.Format("{0}:{1}:{2}", v.X, v.Y, v.Z)));
			}

			public static string MtrDtoBroadcastString(MatrixD mat)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						sb.Append(mat[i, j] + ":");
					}
				}
				return sb.ToString().TrimEnd(':');
			}
		}




		/// ///////////////////////////////
		/// 
		public static class E
		{
			static string debugTag = "";
			static Action<string> e;
			static IMyTextPanel p;
			public static void Init(Action<string> echo, IMyGridTerminalSystem g)
			{
				e = echo;
				p = g.GetBlockWithName("LCD Debug") as IMyTextPanel;
			}
			public static void Echo(string s) { if ((debugTag == "") || (s.Contains(debugTag))) e(s); }

			static string buff = "";
			public static void DebugToPanel(string s)
			{
				if (p != null)
					buff += s + "\n";
			}

			public static void MatToPanel(MatrixD m)
			{
				DebugToPanel(m.Col0.ToString("F1"));
				DebugToPanel(m.Col1.ToString("F1"));
				DebugToPanel(m.Col2.ToString("F1"));
				//DebugToPanel(m.Translation.ToString("F1"));
			}

			public static void FlushDebugPanel()
			{
				if (!string.IsNullOrEmpty(buff))
				{
					p.WriteText(buff);
					buff = "";
				}
			}

			static IMyCameraBlock debCam;
			public static void I(IMyCameraBlock c) { debCam = c; }
			public static void R(Vector3D v)
			{
				if (debCam != null)
					debCam.Raycast(v);
			}
		}

		List<KeyValuePair<string, string>> taggedMsgBuffer = new List<KeyValuePair<string, string>>();
		public void SendFeedback(string message, string tag, bool sendToLogger = false)
		{
			if (sendToLogger)
			{
				message = DateTime.Now.ToString("hh:mm:ss <") + Me.CustomName + ">: " + message;
				if (logger != null)
				{
					this.logger.WritePublicText(message);
				}
			}
			else
			{
				if ((rawPanels.Count > 0) || mWriter.HasPanels())
				{
					if (string.IsNullOrEmpty(tag))
						tag = "cross-cat";
					var tags = tag.Split(',');
					foreach (var t in tags)
					{
						taggedMsgBuffer.Add(new KeyValuePair<string, string>(t, message));
					}
				}
			}
		}

		List<IMyTextPanel> rawPanels = new List<IMyTextPanel>();
		IFeedbackWriter logger;
		IFeedbackWriter mWriter;

		int currentTagIndex;
		List<string> tags = new List<string>() { "" };
		public void CycleFeedbackTag()
		{
			for (int i = 0; i < tags.Count; i++)
			{
				if (i == currentTagIndex)
				{
					var newInd = i + 1;
					if (newInd == tags.Count)
						newInd = 0;
					currentTagIndex = newInd;
					break;
				}
			}
		}

		public void FlushFeedbackBuffer()
		{
			// load (2 rdc mounts) 0.227-0.280
			// idle 0.051ms

			// idle 0.051ms
			// load 0.200ms
			if (taggedMsgBuffer.Count > 0)
			{
				foreach (var tag in taggedMsgBuffer.GroupBy(t => t.Key).Select(g => g.First()))
				{
					if ((tag.Key != "cross-cat") && !tags.Contains(tag.Key))
						tags.Add(tag.Key);
				}
				string a = "[" + ((currentTagIndex == 0) ? "*" : tags[currentTagIndex]) + "]\n";
				string xc = string.Join("\n", taggedMsgBuffer.Where(i => i.Key == "cross-cat").Select(i => i.Value));
				a = xc + "\n" + a;
				a += string.Join("\n", taggedMsgBuffer.Where(i => (i.Key != "cross-cat") && (currentTagIndex == 0 || i.Key == tags[currentTagIndex])).Select(i => i.Value));
				foreach (var panel in rawPanels)
				{
					panel.WriteText(a);
				}
				mWriter.WritePublicText(a);
				taggedMsgBuffer.Clear();
			}
		}

		public interface IFeedbackWriter
		{
			void WritePublicText(string message);
			void AddPanel(IMyTextPanel panel);
			void ClearAllText();
			bool HasPanels();
		}

		public abstract class WriterBase : IFeedbackWriter
		{
			protected List<IMyTextPanel> panels = new List<IMyTextPanel>();
			Action<IMyTextPanel> conf;
			protected int maxLinesCount;
			protected int maxCharsPerLine;
			public WriterBase(int maxLinesCount, int maxCharsPerLine, Action<IMyTextPanel> configurator = null)
			{
				this.maxLinesCount = maxLinesCount;
				this.maxCharsPerLine = maxCharsPerLine;
				conf = configurator;
			}
			public void AddPanel(IMyTextPanel panel)
			{
				if ((panel != null) && !panels.Contains(panel))
				{
					panels.Add(panel);
					conf?.Invoke(panel);
				}
			}
			public void ClearAllText()
			{
				panels.ForEach(p => p.WriteText(""));
			}
			public bool HasPanels()
			{
				return panels.Count > 0;
			}
			public abstract void WritePublicText(string message);
		}

		public class RollingAppender : WriterBase
		{
			public RollingAppender(int maxLinesCount, int maxCharsPerLine, Action<IMyTextPanel> configurator = null) : base(maxLinesCount, maxCharsPerLine, configurator) { }

			void Push(string message)
			{
				foreach (IMyTextPanel outputPanel in panels)
				{
					var str = outputPanel.GetText().TrimEnd('\n');
					string[] currentlySavedLines = str.Split('\n');
					int availableLinesCount = this.maxLinesCount - currentlySavedLines.Length;
					string[] messageLines = message.TrimEnd('\n').Split('\n');

					List<string> csLines = currentlySavedLines.ToList();
					List<string> linesTW = messageLines.ToList();
					csLines.AddRange(linesTW);
					availableLinesCount -= messageLines.Length;
					StringBuilder sb = new StringBuilder();
					for (int n = 0; n < csLines.Count; n++)
					{
						if (availableLinesCount < 0)
						{
							availableLinesCount++;
							continue;
						}
						sb.Append(csLines[n] + "\n");
					}
					outputPanel.WriteText(sb.ToString());
				}
			}

			public override void WritePublicText(string message)
			{
				Push(string.Join("\n", Wrap(message.Trim('\n'), maxCharsPerLine)));
			}
		}

		public class MultiscreenWriter : WriterBase
		{
			public MultiscreenWriter(int maxLinesCount, int maxCharsPerLine, Action<IMyTextPanel> configurator = null) : base(maxLinesCount, maxCharsPerLine, configurator) { }

			public override void WritePublicText(string message)
			{
				WriteToMulti(message, false);
			}

			private void WriteToMulti(string message, bool append)
			{
				if (!append)
				{
					ClearAllText();
				}

				string msg = message;
				// if we see \n in the bulk message then we have to split 'em up 
				if (msg.Contains("\n"))
				{
					string[] linesToProcess = msg.Split('\n');
					foreach (string line in linesToProcess)
					{
						ProcessLine(line, append);
					}
				}
				else
				{
					ProcessLine(msg, append);
				}
			}

			private void ProcessLine(string line, bool append)
			{
				List<String> wrappedLinesToWrite = Wrap(line, maxCharsPerLine);
				wrappedLinesToWrite.Reverse();
				var msgStack = new Stack<string>(wrappedLinesToWrite);
				for (int n = 0; n < panels.Count; n++)
				{
					var currentPanel = panels[n];
					var str = currentPanel.GetText();
					int availableLines = maxLinesCount - str.Split('\n').Length;
					if (availableLines < 1)
						continue;
					int msgStackCountCapture = msgStack.Count;
					for (int i = 0; i < msgStackCountCapture; i++)
					{
						currentPanel.WriteText(msgStack.Pop() + "\n", true);
						if (--availableLines <= 1)
							break;
					}
				}
			}
		}

		// string wrapping by Bryan Reynolds
		public static List<String> Wrap(string text, int maxLength)
		{
			if (text.Length == 0) return new List<string>();
			var words = text.Split(' ');
			var lines = new List<string>();
			var currentLine = "";
			foreach (var currentWord in words)
			{
				if ((currentLine.Length > maxLength) || ((currentLine.Length + currentWord.Length) > maxLength))
				{
					lines.Add(currentLine);
					currentLine = "";
				}
				if (currentLine.Length > 0)
					currentLine += " " + currentWord;
				else
					currentLine += currentWord;
			}
			if (currentLine.Length > 0)
				lines.Add(currentLine);
			return lines;
		}