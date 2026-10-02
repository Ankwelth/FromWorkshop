	/*
     * Author: Wanderer
	 
     Version: 2.0
	 - Complete rewrite of the script.
	 - Performance optimizations.
	 - The script now supports modded hydrogen and oxygen tanks.
	 - The script now supports surface LCDs.
	 - Changed how the script searches LCDs. See steam workshop page for more details.
	 - Changed how custom settings set up for vents and gas tanks.

	* Change log:   
	* Script setup: 
     */
	class LifeSupportSystem
	{
		// General settings.
		string SubstructFromVentName = "Air Vent"; // This string will be substructed from vent name for displaying.
		bool AllowOxygenFarmConstantWork = false; // If 'true' O2 farm(s) will always ON and fill tanks over any limits. Otherwise will be turned OFF when oxygen not need.
		int PageSize = 20; // Number of air vents listed per print.

		// Air vents
		double GlobalPressureLimit = 0.85; // If room fill rate below this limit vent will start pressurization sequence.
		double GlobalDepressureLimit = 0; // Vent will not depresssure room below this fill ratio.

		// O2 and H2 tanks
		double GlobalOxygenTankLowerLimit = 0.4; // If fill rate below this limit tank will start refill sequence.
		double GlobalOxygenTankUpperLimit = 0.5; // If fill rate higher this limit tank will stop refill sequence.
		double GlobalHydrogenTankLowerLimit = 0.95; // If fill rate below this limit tank will start refill sequence.
		double GlobalHydrogenTankUpperLimit = 1; // If fill rate higher this limit tank will stop refill sequence.

		//-----------------------------------------------------------------------------------------------
		// Script stuff. Modifying anything below will void your warranty.
		//-----------------------------------------------------------------------------------------------
		class RuntimeProfiler
		{
			Program Parent;
			double RunTimeAvrEMA;
			double EMA_A;
			int CycleNum;
			int Counter;
			StringBuilder Str;
			IMyTextSurface MyLCD;
			IEnumerator<bool> StMachine;
			public int SkipCycles;
			public string Caption;
			public string Memo;
			public RuntimeProfiler(Program parent, int skipCycles = 0, int skipOffset = 0, double emaA = 0.003)
			{
				Parent = parent;
				SkipCycles = skipCycles;
				CycleNum = skipOffset;
				EMA_A = emaA;
				Str = new StringBuilder();
				Caption = "";
				Memo = "";
				if(Parent.Me.CustomData == "")
					MyLCD = (Parent.Me as IMyTextSurfaceProvider).GetSurface(0);
				StMachine = UpdateIterator();
			}
			public void Update()
			{
				if(!StMachine.MoveNext())
					StMachine = UpdateIterator();
			}
			IEnumerator<bool> UpdateIterator()
			{
				for(; CycleNum < SkipCycles; ++CycleNum)
				{
					RunTimeAvrEMA = EMA_A * Parent.Runtime.LastRunTimeMs + (1 - EMA_A) * RunTimeAvrEMA;
					yield return true;
				}
				CycleNum = 0;
				Str.Clear();
				Str.Append(Caption);
				++Counter;
				switch(Counter % 4)
				{
					case 0: Str.Append("--"); break;
					case 1: Str.Append("\\"); break;
					case 2: Str.Append(" |"); break;
					case 3: Str.Append("/"); break;
				}
				Str.AppendFormat("\n{0}\n", Memo);
				RunTimeAvrEMA = EMA_A * Parent.Runtime.LastRunTimeMs + (1 - EMA_A) * RunTimeAvrEMA;
				Str.AppendFormat("Instructions used: {0:G}/{1:G}\nAverage(EMA) run time: {2:F3} ms\nLast run time: {3:F3} ms\n{4}",
				Parent.Runtime.CurrentInstructionCount,
				Parent.Runtime.MaxInstructionCount,
				RunTimeAvrEMA,
				Parent.Runtime.LastRunTimeMs,
				Counter % 2 == 0 ? "_" : "");
				Parent.Echo(Str.ToString());
				MyLCD?.WriteText(Str);
			}
		}

		class DisplayScheduler
		{
			Program Parent;
			List<IMyTextSurface> LCDs;
			int CycleNum;
			int PrintCounter;
			IEnumerator<bool> StMachine;
			string Keyword;
			StringBuilder Str;
			StringBuilder Header;
			CallBackFunk CallBack;
			
			public delegate StringBuilder CallBackFunk(int i = 0, IMyTextSurface lcd = null);
			public bool Ready { get; set; }
			public int SkipCycles;
			public string Caption;

			public DisplayScheduler(Program parent, CallBackFunk callBack, string keyword, int skipCycles = 0, int skipOffset = 0)
			{
				Parent = parent;
				CallBack = callBack;
				Keyword = keyword;
				SkipCycles = skipCycles;
				CycleNum = skipOffset;
				LCDs = new List<IMyTextSurface>();
				Str = new StringBuilder();
				Header = new StringBuilder();
				StMachine = UpdateIterator();
			}
			public void DetectBlocks()
			{
				LCDs.Clear();
				List<IMyTerminalBlock> lcdHosts = new List<IMyTerminalBlock>();
				Parent.GridTerminalSystem.GetBlocksOfType(lcdHosts, block => block as IMyTextSurfaceProvider != null);
				List<string> lines = new List<string>();
				IMyTextSurface lcd;
				foreach(var block in lcdHosts)
				{
					if(block.CustomData.Length > 0)
					{
						lines.Clear();
						new StringSegment(block.CustomData).GetLines(lines);
						foreach(var line in lines)
						{
							if(line.Contains(Keyword))
							{
								if(block as IMyTextSurface != null)
									lcd = block as IMyTextSurface;
								else
								{
									int i = 0;
									int.TryParse(line.Replace(Keyword, ""), out i);
									IMyTextSurfaceProvider t_sp = block as IMyTextSurfaceProvider;
									i = Math.Max(0, Math.Min(i, t_sp.SurfaceCount));
									lcd = t_sp.GetSurface(i);
								}
								lcd.ContentType = ContentType.TEXT_AND_IMAGE;
								LCDs.Add(lcd);
							}
						}
					}
				}
				Ready = LCDs.Count > 0;
			}
			public void ForcePrint(StringBuilder text = null)
			{
				if(Ready)
				{
					Header.Clear();
					Header.Append(Caption);
					++PrintCounter;
					switch(PrintCounter % 4)
					{
						case 0: Header.Append("--"); break;
						case 1: Header.Append("\\"); break;
						case 2: Header.Append("|"); break;
						case 3: Header.Append("/"); break;
					}
					Header.Append("\n_________________________________________________________\n");
					if(text != null)
					{
						Str.Clear();
						Str.Append(Header);
						Str.Append(text);
						foreach(var lcd in LCDs)
							lcd.WriteText(Str);
					}
					else
					{
						foreach(var lcd in LCDs)
						{
							Str.Clear();
							Str.Append(Header);
							Str.Append(CallBack(PrintCounter, lcd));
							lcd.WriteText(Str);
						}
					}
				}
			}
			public void Update()
			{
				if(Ready)
					if(!StMachine.MoveNext())
						StMachine = UpdateIterator();
			}
			IEnumerator<bool> UpdateIterator()
			{
				while(CycleNum < SkipCycles)
				{
					++CycleNum;
					yield return true;
				}
				CycleNum = 0;
				ForcePrint();
			}
		}

		class ConveyorNet
		{
			public class AirVentWrapper
			{
				double UpdatePeriod;
				double TimeStamp;
				double TimeToShutdown;
				double LowerPressureLimit;
				double LowerDepressureLimit;
				Vector3D WorldLastPosition;
				bool InitiateShutdown;

				public IMyAirVent Obj { get; }
				public readonly bool Hide;
				public bool Pressurize { get { return !Obj.Depressurize; } }
				public bool Depressurize { get { return Obj.Depressurize; } }
				public bool IsFunctional { get { return Obj.IsFunctional; } }
				public bool IsInternal { get { return Obj.CanPressurize; } }
				public bool IsExternal { get { return !Obj.CanPressurize; } }
				public bool IsNeedOxygenSupply { get; private set; }
				public double OxygenLevel { get; private set; }
				public double OxygenLevelEnvironmental { get; private set; }
				public VentStatus Status { get { return Obj.Status; } }
				public bool Enabled { get { return Obj.Enabled; } set { Obj.Enabled = value; } }
				public bool OxygenSupplyAvailable { get; set; }

				public AirVentWrapper(IMyAirVent vent, double lowerPresLim, double lowerDepresLim, double updatePeriodSec)
				{
					Obj = vent;
					LowerPressureLimit = lowerPresLim;
					LowerDepressureLimit = lowerDepresLim;
					TimeStamp = UpdatePeriod = updatePeriodSec;
					MyIni config = new MyIni();
					if(config.TryParse(vent.CustomData))
					{
						MyIniKey key = new MyIniKey("Vent", "LowerPressureLimit");
						if(config.ContainsKey(key))
							config.Get(key).TryGetDouble(out LowerPressureLimit);
						key = new MyIniKey("Vent", "LowerDepressureLimit");
						if(config.ContainsKey(key))
							config.Get(key).TryGetDouble(out LowerDepressureLimit);
						key = new MyIniKey("Vent", "Hide");
						if(config.ContainsKey(key))
							config.Get(key).TryGetBoolean(out Hide);
					}
				}
				public void Update(double dT = 0, bool force = false)
				{
					TimeStamp += dT;
					if(IsInternal)
					{
						OxygenLevel = GetOxygenLevelOptimazed(OxygenLevel, force);
						if(Pressurize)
						{
							if(!Enabled)
								IsNeedOxygenSupply = false;
							if(OxygenLevel < LowerPressureLimit)
							{
								IsNeedOxygenSupply = IsFunctional;
								Enabled = IsFunctional && OxygenSupplyAvailable;
							}
							else if(Status == VentStatus.Pressurized || !IsNeedOxygenSupply)
								InitiateShutdown = true;
							if(InitiateShutdown)
							{
								TimeToShutdown += dT;
								if(TimeToShutdown >= 1.5)
								{
									InitiateShutdown = Enabled = IsNeedOxygenSupply = false;
									TimeToShutdown = 0;
								}
							}
						}
						else // Depressurize
						{
							IsNeedOxygenSupply = false;
							if(Status == VentStatus.Depressurized || OxygenLevel - LowerDepressureLimit <= 1e-6)
							{
								InitiateShutdown = Enabled;
								if(InitiateShutdown)
								{
									TimeToShutdown += dT;
									if(TimeToShutdown >= 1.5)
									{
										InitiateShutdown = Enabled = false;
										TimeToShutdown = 0;
									}
								}
							}
							else
								Enabled = true;
						}
					}
					else // vent is External
					{
						IsNeedOxygenSupply = false;
						if(Depressurize)
						{
							Vector3D pos = Obj.GetPosition();
							if((WorldLastPosition - pos).LengthSquared() > 1e6 && Obj.IsFunctional) // 1 km
							{
								OxygenLevelEnvironmental = GetOxygenLevelOptimazed(OxygenLevelEnvironmental, true);
								WorldLastPosition = pos;
							}
						}
						else // Pressurize
							Enabled = false;
					}
					TimeStamp = TimeStamp % UpdatePeriod;
				}
				private double GetOxygenLevelOptimazed(double oldValue, bool force = false)
				{
					double result = oldValue;
					if(Obj.Enabled)
						result = Obj.GetOxygenLevel();
					else if(TimeStamp >= UpdatePeriod || force)
					{
						Obj.Enabled = true;
						result = Obj.GetOxygenLevel();
						Obj.Enabled = false;
					}
					return result;
				}
			}

			class OxygenGeneratorWrapper
			{

				public IMyGasGenerator Obj { get; }
				public bool IsFunctional { get { return Obj.IsFunctional; } }
				public MyFixedPoint IceAmount { get; private set; }
				public bool IsHasIce { get { return IceAmount > 0; } }
				public bool Enabled { get { return Obj.Enabled; } set { Obj.Enabled = value; } }
				public OxygenGeneratorWrapper(IMyGasGenerator generator)
				{
					Obj = generator;
				}
				public void Update()
				{
					IceAmount = 0;
					List<MyInventoryItem> items = new List<MyInventoryItem>();
					for(int ii = 0; ii < Obj.InventoryCount; ++ii)
					{
						items.Clear();
						Obj.GetInventory(ii).GetItems(items);
						foreach(var item in items)
						{
							if(item.Type.SubtypeId == "Ice")
								IceAmount += item.Amount;
						}
					}
				}
			}

			class OxygenFarmWrapper
			{
				public IMyOxygenFarm Obj { get; }
				public bool CanProduce { get { return Obj.CanProduce; } }
				public bool Enabled { get { return Obj.GetValueBool("OnOff"); } set { Obj.SetValueBool("OnOff", value); } }
				public OxygenFarmWrapper(IMyOxygenFarm farm)
				{
					Obj = farm;
				}
			}

			class GasTankWrapper
			{
				double LowerLimit;
				double UpperLimit;

				public IMyGasTank Obj { get; }
				public bool IsNeedGasSupply { get; private set; }
				public bool IsFunctional { get { return Obj.IsFunctional; } }
				public bool Stockpile { get { return Obj.Stockpile; } set { Obj.Stockpile = value; } }
				public double FilledRatio { get { return Obj.FilledRatio; } }
				public bool Enabled { get { return Obj.Enabled; } set { Obj.Enabled = value; } }
				public GasTankWrapper(IMyGasTank tank, double lowerLim, double upperLim)
				{
					Obj = tank;
					LowerLimit = lowerLim;
					UpperLimit = upperLim;
					MyIni config = new MyIni();
					if(config.TryParse(tank.CustomData))
					{
						MyIniKey key = new MyIniKey("GasTank", "LowerLimit");
						if(config.ContainsKey(key))
							config.Get(key).TryGetDouble(out LowerLimit);
						key = new MyIniKey("GasTank", "UpperLimit");
						if(config.ContainsKey(key))
							config.Get(key).TryGetDouble(out UpperLimit);
					}
				}
				public void Update()
				{
					if(Enabled)
					{
						if(FilledRatio < LowerLimit)
							IsNeedGasSupply = IsFunctional;
						else if(FilledRatio >= UpperLimit)
							IsNeedGasSupply = false;
					}
					else
						IsNeedGasSupply = false;
				}
			}

			struct BoolCollection
			{
				public bool Vents;
				public bool Tanks;
				public bool Generators;
				public bool Other;
				public bool Room { get { return Vents; } }
				public bool Block { get { return Tanks || Other; } }
				public bool Total { get { return Generators || Room || Block; } }
			}
			
			public List<AirVentWrapper> Vents { get; }
			List<OxygenGeneratorWrapper> O2Gens;
			List<OxygenFarmWrapper> O2Farms;
			List<GasTankWrapper> O2Tanks;
			List<GasTankWrapper> H2Tanks;
			List<IMyCargoContainer> CargoContainers;
			List<IMyCryoChamber> CryoPods;
			List<IMyCockpit> Cockpits;
			double BlockPressureLimit;

			public IMyInventory RepresentativeInventory { get; private set; }
			public MyFixedPoint IceAmount { get; private set; }
			public bool Busy { get; private set; }
			public bool HasO2Supply { get; private set; }
			public int GeneratorsWorking { get; private set; }
			public int GeneratorsStandBy { get; private set; }
			public int GeneratorsMalfunc { get; private set; }
			public int O2TanksWorking { get; private set; }
			public int O2TanksStandBy { get; private set; }
			public int O2TanksMalfunc { get; private set; }
			public int H2TanksWorking { get; private set; }
			public int H2TanksStandBy { get; private set; }
			public int H2TanksMalfunc { get; private set; }
			public bool KeepFarmsOn { get; set; }
			public ConveyorNet(bool keepFarmsOn = false)
			{
				KeepFarmsOn = keepFarmsOn;
				Vents = new List<AirVentWrapper>();
				O2Gens = new List<OxygenGeneratorWrapper>();
				O2Farms = new List<OxygenFarmWrapper>();
				O2Tanks = new List<GasTankWrapper>();
				H2Tanks = new List<GasTankWrapper>();
				CargoContainers = new List<IMyCargoContainer>();
				CryoPods = new List<IMyCryoChamber>();
				Cockpits = new List<IMyCockpit>();
			}
			public void AddAirVent(IMyAirVent vent, double lowerPresLim, double lowerDepresLim, double timeDisp = 0, double updatePeriodSec = 30)
			{
				vent.Enabled = false;
				AirVentWrapper newVent = new AirVentWrapper(vent, lowerPresLim, lowerDepresLim, updatePeriodSec);
				newVent.Update(timeDisp, true);
				Vents.Add(newVent);
				if(RepresentativeInventory == null && vent.HasInventory)
					RepresentativeInventory = vent.GetInventory();
			}
			public void AddOxygenGenerator(IMyGasGenerator gen)
			{
				O2Gens.Add(new OxygenGeneratorWrapper(gen));
				if(RepresentativeInventory == null && gen.HasInventory)
					RepresentativeInventory = gen.GetInventory();
			}
			public void AddOxygenFarm(IMyOxygenFarm farm)
			{
				O2Farms.Add(new OxygenFarmWrapper(farm));
				if(RepresentativeInventory == null && farm.HasInventory)
					RepresentativeInventory = farm.GetInventory();
			}
			public void AddOxygenTank(IMyGasTank tank, double lowerLim, double upperLim)
			{
				O2Tanks.Add(new GasTankWrapper(tank, lowerLim, upperLim));
				if(RepresentativeInventory == null && tank.HasInventory)
					RepresentativeInventory = tank.GetInventory();
			}
			public void AddHydrogenTank(IMyGasTank tank, double lowerLim, double upperLim)
			{
				H2Tanks.Add(new GasTankWrapper(tank, lowerLim, upperLim));
				if(RepresentativeInventory == null && tank.HasInventory)
					RepresentativeInventory = tank.GetInventory();
			}
			public void AddCargoContainer(IMyCargoContainer container)
			{
				CargoContainers.Add(container);
			}
			public void AddCryoChamber(IMyCryoChamber chamber, double lowerPresLim)
			{
				CryoPods.Add(chamber);
				BlockPressureLimit = lowerPresLim;
			}
			public void AddCockpit(IMyCockpit cockpit, double lowerPresLim)
			{
				Cockpits.Add(cockpit);
				BlockPressureLimit = lowerPresLim;
			}
			public void Update(double dT)
			{
				BoolCollection hasO2Supply = new BoolCollection();
				BoolCollection hasH2Supply = hasO2Supply;
				BoolCollection isNeedOxygen = hasO2Supply;
				BoolCollection isNeedHydrogen = hasO2Supply;
				BoolCollection startProduce = hasO2Supply;
				GeneratorsWorking = GeneratorsStandBy = GeneratorsMalfunc = O2TanksWorking = O2TanksStandBy = O2TanksMalfunc =
					H2TanksWorking = H2TanksStandBy = H2TanksMalfunc = 0;
				Busy = false;
				// collect info
				IceAmount = 0;
				foreach(var cont in CargoContainers)
				{
					List<MyInventoryItem> items = new List<MyInventoryItem>();
					cont.GetInventory().GetItems(items);
					MyFixedPoint ice = 0;
					foreach(var item in items)
					{
						if(item.Type.SubtypeId == "Ice")
							ice += item.Amount;
					}
					IceAmount += ice;
				}
				foreach(var gen in O2Gens)
				{
					gen.Update();
					IceAmount += gen.IceAmount;
					hasO2Supply.Generators |= gen.IsFunctional;
					hasH2Supply.Generators |= gen.IsFunctional;
					if(!gen.IsFunctional || !gen.IsHasIce)
						++GeneratorsMalfunc;
					else if(gen.Enabled)
						++GeneratorsWorking;
					else
						++GeneratorsStandBy;
				}
				hasO2Supply.Generators &= IceAmount > 0;
				hasH2Supply.Generators &= IceAmount > 0;
				foreach(var vent in Vents)
				{
					vent.Update(dT);
					hasO2Supply.Vents |= (vent.IsExternal && vent.Depressurize && vent.IsFunctional && vent.OxygenLevelEnvironmental > 0);
					isNeedOxygen.Vents |= vent.IsNeedOxygenSupply;
				}
				foreach(var tank in O2Tanks)
				{
					tank.Update();
					hasO2Supply.Tanks |= (tank.IsFunctional && !tank.Stockpile && tank.FilledRatio > 0);
					isNeedOxygen.Tanks |= tank.IsNeedGasSupply;
					if(!tank.IsFunctional)
						++O2TanksMalfunc;
					else if(tank.Enabled)
						++O2TanksWorking;
					else
						++O2TanksStandBy;
				}
				foreach(var tank in H2Tanks)
				{
					tank.Update();
					isNeedHydrogen.Tanks |= tank.IsNeedGasSupply;
					if(!tank.IsFunctional)
						++H2TanksMalfunc;
					else if(tank.Enabled)
						++H2TanksWorking;
					else
						++H2TanksStandBy;
				}
				foreach(var pod in CryoPods)
				{
					if(pod.IsFunctional && pod.OxygenCapacity > 0 && pod.OxygenFilledRatio < BlockPressureLimit)
						isNeedOxygen.Other |= true;
				}
				foreach(var pit in Cockpits)
				{
					if(pit.IsFunctional && pit.OxygenCapacity > 0 && pit.OxygenFilledRatio < BlockPressureLimit)
						isNeedOxygen.Other |= true;
				}
				// take actions
				startProduce.Vents = hasO2Supply.Vents && (isNeedOxygen.Tanks || (!hasO2Supply.Tanks && isNeedOxygen.Total));
				foreach(var vent in Vents)
				{
					if(vent.IsExternal && vent.Depressurize && vent.IsFunctional)
						vent.Enabled = startProduce.Vents;
					else
						vent.OxygenSupplyAvailable = hasO2Supply.Total;
					vent.Update();
					Busy |= vent.Enabled;
				}
				startProduce.Generators = (
					isNeedHydrogen.Total || 
					(!hasO2Supply.Vents && isNeedOxygen.Tanks) ||
					(!hasO2Supply.Vents && !hasO2Supply.Tanks && isNeedOxygen.Total)
					) && hasO2Supply.Generators;
				foreach(var gen in O2Gens)
				{
					if(gen.IsFunctional && IceAmount > 0)
						gen.Enabled = startProduce.Generators;
					else
						gen.Enabled = false;
					Busy |= gen.Enabled;
				}
				foreach(var tank in O2Tanks)
				{
					if(!tank.IsNeedGasSupply && startProduce.Total)
						tank.Enabled = false;
					else
						tank.Enabled = true;
				}
				// handle farms
				foreach(var farm in O2Farms)
					farm.Enabled = KeepFarmsOn || isNeedOxygen.Total;
				// is busy?
				HasO2Supply = hasO2Supply.Total;
			}
		}

		List<ConveyorNet> ConveyorNetList;
		DisplayScheduler Displays;
		RuntimeProfiler Profiler;

		IEnumerator<bool> BootStMachine;
		Program Parent;
		bool Booted;
		bool LSSBusy;
		double TimeLine;

		public LifeSupportSystem(Program parent)
		{
			Parent = parent;
			ConveyorNetList = new List<ConveyorNet>();
			Displays = new DisplayScheduler(parent, ComposeTextInfo, "LSS_display");
			Profiler = new RuntimeProfiler(parent, 0);
			Str = new StringBuilder();
			BootStMachine = Boot();
			Parent.Runtime.UpdateFrequency = UpdateFrequency.Update100;
		}

		public void Update(UpdateType updateSource)
		{
			if(Booted)
			{
				double dT = 0;
				if(updateSource.HasFlag(UpdateType.Update1))
					dT = 0.016;
				else if(updateSource.HasFlag(UpdateType.Update10))
					dT = 0.16;
				else if(updateSource.HasFlag(UpdateType.Update100))
					dT = 1.6;
				try
				{
					TimeLine += dT;
					bool busy = false;
					foreach(var net in ConveyorNetList)
					{
						net.Update(dT);
						busy |= net.Busy;
					}
					Displays.Update();

					if(LSSBusy && !busy)
					{
						Parent.Runtime.UpdateFrequency = UpdateFrequency.Update100;
						Displays.SkipCycles = 0;
						Profiler.SkipCycles = 0;
					}
					else if(!LSSBusy && busy)
					{
						Parent.Runtime.UpdateFrequency = UpdateFrequency.Update10;
						Displays.SkipCycles = 10;
						Profiler.SkipCycles = 2;
					}
					LSSBusy = busy;
				}
				catch(Exception ex) // *Pokemon joke here*
				{
					Booted = false;
					Parent.Echo("Exeption: " + ex.ToString());
				}
				Profiler.Update();
			}
			else // Run boot sequence
			{
				if(!BootStMachine.MoveNext())
					BootStMachine = Boot();
			}
		}

		IEnumerator<bool> Boot()
		{
			Parent.Runtime.UpdateFrequency = UpdateFrequency.Update10;
			StringBuilder bootLog = new StringBuilder("\n");
			IMyTextSurface myLCD = null;
			if(Parent.Me.CustomData == "")
				myLCD = (Parent.Me as IMyTextSurfaceProvider).GetSurface(0);
			ConveyorNetList.Clear();
			ConveyorNet net;

			Displays.DetectBlocks();
			Displays.Caption = "Life Support System\\\\Booting... ";
			if(Displays.Ready)
				bootLog.Append("LSS.DisplayScheduler... ok\n");
			else
				bootLog.Append("LSS.DisplayScheduler... failure\n");
			BootPrint(bootLog, myLCD);
			yield return true;
			List<IMyGasGenerator> allGenerators = new List<IMyGasGenerator>();
			Parent.GridTerminalSystem.GetBlocksOfType(allGenerators, block => block.IsSameConstructAs(Parent.Me) && !block.CustomData.Contains("LSS_IGNORE"));
			foreach(var gen in allGenerators)
			{
				net = GetNetByInventory(gen);
				if(net == null)
				{
					net = new ConveyorNet(AllowOxygenFarmConstantWork);
					ConveyorNetList.Add(net);
				}
				net.AddOxygenGenerator(gen);
			}
			bootLog.AppendFormat("Gas generators added... {0}\n", allGenerators.Count);
			BootPrint(bootLog, myLCD);
			yield return true;
			List<IMyOxygenFarm> allFarms = new List<IMyOxygenFarm>();
			Parent.GridTerminalSystem.GetBlocksOfType(allFarms, block => block.IsSameConstructAs(Parent.Me) && !block.CustomData.Contains("LSS_IGNORE"));
			foreach(var farm in allFarms)
			{
				net = GetNetByInventory(farm);
				if(net == null)
				{
					net = new ConveyorNet(AllowOxygenFarmConstantWork);
					ConveyorNetList.Add(net);
				}
				net.AddOxygenFarm(farm);
			}
			bootLog.AppendFormat("Oxygen farms added... {0}\n", allFarms.Count);
			BootPrint(bootLog, myLCD);
			yield return true;
			List<IMyGasTank> allTanks = new List<IMyGasTank>();
			Parent.GridTerminalSystem.GetBlocksOfType(allTanks, block => block.IsSameConstructAs(Parent.Me) && !block.CustomData.Contains("LSS_IGNORE"));
			int O2tanks = 0;
			int H2Tanks = 0;
			foreach(var tank in allTanks)
			{
				net = GetNetByInventory(tank);
				if(net == null)
				{
					net = new ConveyorNet(AllowOxygenFarmConstantWork);
					ConveyorNetList.Add(net);
				}
				MyResourceSinkComponent sinkComponent;
				if(tank.Components.TryGet(out sinkComponent))
				{
					var resources = sinkComponent.AcceptedResources;
					foreach(var res in resources)
					{
						if(res.SubtypeName == "Hydrogen")
						{
							net.AddHydrogenTank(tank, GlobalHydrogenTankLowerLimit, GlobalHydrogenTankUpperLimit);
							++H2Tanks;
						}
						else if(res.SubtypeName == "Oxygen")
						{
							net.AddOxygenTank(tank, GlobalOxygenTankLowerLimit, GlobalOxygenTankUpperLimit);
							++O2tanks;
						}
					}
				}
			}
			bootLog.AppendFormat("Oxygen tanks added... {0}\nHydrogen tanks added... {1}\n", O2tanks, H2Tanks);
			BootPrint(bootLog, myLCD);
			yield return true;
			List<IMyAirVent> allVents = new List<IMyAirVent>();
			Parent.GridTerminalSystem.GetBlocksOfType(allVents, block => block.IsSameConstructAs(Parent.Me) && !block.CustomData.Contains("LSS_IGNORE"));
			for(int i = 0; i < allVents.Count; ++i)
			{
				net = GetNetByInventory(allVents[i]);
				if(net == null)
				{
					net = new ConveyorNet(AllowOxygenFarmConstantWork);
					ConveyorNetList.Add(net);
				}
				net.AddAirVent(allVents[i], GlobalPressureLimit, GlobalDepressureLimit, 30.0 / allVents.Count * i);
			}
			bootLog.AppendFormat("Vents added... {0}\n", allVents.Count);
			BootPrint(bootLog, myLCD);
			yield return true;
			List<IMyCargoContainer> allContainers = new List<IMyCargoContainer>();
			Parent.GridTerminalSystem.GetBlocksOfType(allContainers, block => block.IsSameConstructAs(Parent.Me) && !block.CustomData.Contains("LSS_IGNORE"));
			int added = 0;
			foreach(var con in allContainers)
			{
				net = GetNetByInventory(con);
				if(net != null)
				{
					++added;
					net.AddCargoContainer(con);
				}
			}
			bootLog.AppendFormat("Cargo containers added... {0}\n", added);
			BootPrint(bootLog, myLCD);
			yield return true;
			List<IMyCryoChamber> allCryoPods = new List<IMyCryoChamber>();
			Parent.GridTerminalSystem.GetBlocksOfType(allCryoPods, block => block.IsSameConstructAs(Parent.Me) && !block.CustomData.Contains("LSS_IGNORE"));
			added = 0;
			foreach(var pod in allCryoPods)
			{
				net = GetNetByInventory(pod);
				if(net != null)
				{
					++added;
					net.AddCryoChamber(pod, GlobalPressureLimit);
				}
			}
			bootLog.AppendFormat("Cryo chambers added... {0}\n", added);
			BootPrint(bootLog, myLCD);
			yield return true;
			List<IMyCockpit> allCockpits = new List<IMyCockpit>();
			Parent.GridTerminalSystem.GetBlocksOfType(allCockpits, block => block.IsSameConstructAs(Parent.Me) && !block.CustomData.Contains("LSS_IGNORE"));
			added = 0;
			foreach(var pit in allCockpits)
			{
				net = GetNetByInventory(pit);
				if(net != null)
				{
					++added;
					net.AddCockpit(pit, GlobalPressureLimit);
				}
			}
			bootLog.AppendFormat("Cockpits added... {0}\n", added);
			BootPrint(bootLog, myLCD);
			yield return true;
			bootLog.AppendFormat("Conveyor nets composed: {0}\n", ConveyorNetList.Count);
			BootPrint(bootLog, myLCD);
			yield return true;
			Displays.Caption = "Life Support System\\\\Status ";
			Profiler.Memo = bootLog.ToString();
			LSSBusy = false;
			Parent.Runtime.UpdateFrequency = UpdateFrequency.Update100;
			Booted = true;
		}

		void BootPrint(StringBuilder str, IMyTextSurface pbLCD)
		{
			Parent.Echo(str.ToString());
			pbLCD?.WriteText(str);
		}

		int Previ;
		int NetIndx;
		int PrintPos;
		string PrevFont;
		StringBuilder[] Col1Str = { new StringBuilder("O\u2082 generators: "), new StringBuilder("O\u2082 tanks: "), new StringBuilder("H\u2082 tanks: ") };
		StringBuilder Col2Str = new StringBuilder(" working");
		StringBuilder Col3Str = new StringBuilder(" stand by");
		StringBuilder Col4Str = new StringBuilder(" malfunction");
		StringBuilder Col1VentStr = new StringBuilder(" Ventilation name");
		StringBuilder Col2VentStr = new StringBuilder("        O\u2082");
		StringBuilder Str;
		StringBuilder ComposeTextInfo(int i, IMyTextSurface lcd)
		{
			if(Previ == i && PrevFont == lcd.Font)
				return Str;
			Str.Clear();
			Previ = i;
			PrevFont = lcd.Font;

			float col1Width = 0;
			foreach(var item in Col1Str)
				col1Width = Math.Max(col1Width, lcd.MeasureStringInPixels(item, lcd.Font, 1f).X);
			col1Width *= 1.2f;
			float col2Width = 1.2f * lcd.MeasureStringInPixels(Col2Str, lcd.Font, 1f).X;
			float col3Width = 1.2f * lcd.MeasureStringInPixels(Col3Str, lcd.Font, 1f).X;
			float col4Width = 1.2f * lcd.MeasureStringInPixels(Col4Str, lcd.Font, 1f).X;
			ConveyorNet net = ConveyorNetList[NetIndx];
			//Str.AppendFormat("Conveyor net #: {0}\n", NetIndx);
			if(!net.HasO2Supply)
				Str.Append("Warning: no O\u2082 source, life support failure!\n");
			else if(LSSBusy)
				Str.Append("Working...\n");
			else
				Str.Append("Idling...\n");
			Str.Append(GetSpacedStr("", col1Width, lcd));
			Str.Append(GetSpacedStr(Col2Str, col2Width, lcd));
			Str.Append(GetSpacedStr(Col3Str, col3Width, lcd));
			Str.Append(GetSpacedStr(Col4Str, col4Width, lcd, false, "\n"));
			Str.Append(GetSpacedStr(Col1Str[0], col1Width, lcd, true));
			Str.Append(GetSpacedStr(net.GeneratorsWorking, col2Width, lcd));
			Str.Append(GetSpacedStr(net.GeneratorsStandBy, col3Width, lcd));
			Str.Append(GetSpacedStr(net.GeneratorsMalfunc, col4Width, lcd, false, "\n"));
			Str.Append(GetSpacedStr(Col1Str[1], col1Width, lcd, true));
			Str.Append(GetSpacedStr(net.O2TanksWorking, col2Width, lcd));
			Str.Append(GetSpacedStr(net.O2TanksStandBy, col3Width, lcd));
			Str.Append(GetSpacedStr(net.O2TanksMalfunc, col4Width, lcd, false, "\n"));
			Str.Append(GetSpacedStr(Col1Str[2], col1Width, lcd, true));
			Str.Append(GetSpacedStr(net.H2TanksWorking, col2Width, lcd));
			Str.Append(GetSpacedStr(net.H2TanksStandBy, col3Width, lcd));
			Str.Append(GetSpacedStr(net.H2TanksMalfunc, col4Width, lcd, false, "\n"));
			int ice = net.IceAmount.ToIntSafe();
			if(ice < 1e3)
				Str.AppendFormat("Ice amount: {0,7:F2} kg\n", ice);
			else if(ice < 1e6)
				Str.AppendFormat("Ice amount: {0,7:F2}K kg\n", ice / 1e3);
			else
				Str.AppendFormat("Ice amount: {0,7:F2}M kg\n", ice / 1e6);
			Str.Append("/////////////////////////////////////////////////////////////////////\n");
			float col1VentWidth = lcd.MeasureStringInPixels(Col1VentStr, lcd.Font, 1f).X;
			foreach(var vent in net.Vents)
			{
				Temp.Clear();
				Temp.Append(vent.Obj.CustomName);
				Temp.Replace(SubstructFromVentName, "");
				col1VentWidth = Math.Max(col1VentWidth, lcd.MeasureStringInPixels(Temp, lcd.Font, 1f).X);
			}
			float col2VentWidth = lcd.MeasureStringInPixels(Col2VentStr, lcd.Font, 1f).X;
			Str.Append(GetSpacedStr(Col1VentStr, col1VentWidth, lcd, true));
			Str.Append(GetSpacedStr(Col2VentStr, col1VentWidth, lcd, false, "\n"));
			int printed = 0;
			for(int p = 0; p < PageSize && p + PrintPos < net.Vents.Count; ++p)
			{
				ConveyorNet.AirVentWrapper vent = net.Vents[p + PrintPos];
				if(!vent.Hide)
				{
					Str.Append(GetSpacedStr(vent.Obj.CustomName.Replace(SubstructFromVentName, ""), col1VentWidth, lcd, true));
					if(vent.IsFunctional)
					{
						if(vent.IsInternal)
							Str.Append(GetSpacedStr(Math.Round(vent.OxygenLevel * 100, 2), col1VentWidth, lcd, false, "%\n"));
						else if(vent.Depressurize)
							Str.Append(GetSpacedStr(Math.Round(vent.OxygenLevelEnvironmental * 100, 2), col1VentWidth, lcd, false, "%\n"));
						else
							Str.Append(GetSpacedStr("breach", col1VentWidth, lcd, false, "\n"));
					}
					else
						Str.Append(GetSpacedStr("malf-n", col1VentWidth, lcd, false, "\n"));
					++printed;
				}
			}
			PrintPos += printed;
			Str.AppendFormat("\n[page {0}/{1}]", PrintPos / PageSize + 1, net.Vents.Count / PageSize + 1);
			if(PrintPos >= net.Vents.Count)
			{
				PrintPos = 0;
				NetIndx = (NetIndx + 1) % ConveyorNetList.Count;
			}
			
			return Str;
		}
		StringBuilder Temp = new StringBuilder();
		StringBuilder spacebarStr = new StringBuilder(" ");
		StringBuilder GetSpacedStr<T>(T value, float width, IMyTextSurface lcd, bool alignLeft = false, string terminator = "")
		{
			Temp.Clear();
			Temp.AppendFormat("{0}{1}", value, terminator);
			float spacebarWidth = lcd.MeasureStringInPixels(spacebarStr, lcd.Font, 1f).X;
			float valueWidth = lcd.MeasureStringInPixels(Temp, lcd.Font, 1f).X;
			if(alignLeft)
			{
				Temp.Append(' ', Math.Max((int)((width - valueWidth) / spacebarWidth), 0));
			}
			else
			{
				Temp.Clear();
				Temp.Append(' ', Math.Max((int)((width - valueWidth) / spacebarWidth), 0));
				Temp.AppendFormat("{0}{1}", value, terminator);
			}
			return Temp;
		}

		ConveyorNet GetNetByInventory(IMyEntity block)
		{
			ConveyorNet result = null;
			/*	-- Cut off because air vents doesn't have inventory --
			if(block.HasInventory)
			{
				IMyInventory inv = block.GetInventory();
				foreach(var net in ConveyorNetList)
					if(inv.IsConnectedTo(net.RepresentativeInventory))
					{
						result = net;
						break;
					}
			}
			*/
			if(ConveyorNetList.Count > 0)
				result = ConveyorNetList[0];
			return result;
		}
	}
	//---------------------------------------------------------------------------------------------------

	LifeSupportSystem LSS;

	public Program()
	{
		LSS = new LifeSupportSystem(this);
	}
	
	public void Main(string args, UpdateType updateSource)
	{
		LSS.Update(updateSource);
	}
