/*
 * R e a d m e
 * -----------
 * 
 * Available Screen:
 * - debuglog			> shows a log of past actions
 * - debugscreen		> only for debugging purposes
 * - power				> overview of your batteries and generators
 * - gas				> overview of your tanks and gas generators
 * - security			> overview of your turrets and damages blocks
 * - cargo				> overview of your designated cargo holds
 * - ore				> overview of your stored ores
 * - ingot				> overview of your stored ingots
 * - component			> overview of your stored components
 * - ammo				> overview of your stored ammos
 * - bottle			> overview of your stored bottles
 * - tool				> overview of your stored tools, weapons and consumeables
 * 
 * Available Sort Options
 * There are 4 default sort matches, that give that container a priority (unless overwritten).
 * - itemsink			> everything goes here			(default priotity -3)
 * - ore				> ored goes here				(default priority -2)
 * - ingot				> ingots go here				(default priority -2)
 * - component			> components go here			(default priority -2)
 * - ammo				> ammo goes here				(default priority -2)
 * - bottle			> bottles go here				(default priority -2)
 * - tool				> tools go here					(default priority -2)
 * - any item name		> item with that name goes here	(default priority -1)
 * - category/itemname > item that matches both goes here	(default priority 0)
 * 
 * priority can be overwritten by adding :pX where X is the priority (higher priority will be filled first).
 */


IEnumerator<bool> _mainRoutine;
List<AbstractBaseRoutine> Routines = new List<AbstractBaseRoutine>();
InstanceHolder Instance = new InstanceHolder();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    _mainRoutine = RunMainRoutine();
}


public IEnumerator<bool> RunMainRoutine()
{
    List<IMyTerminalBlock> NewBlockList = new List<IMyTerminalBlock>();

    Instance.LcdHelper = new LCDHelper();
    Instance.Log = new LogHelper(50);
    Instance.Me = Me;
    Instance.Runtime = Runtime;
    Instance.Config = new ConfigHelper(Me);
    Instance.AllBlocks = new List<IMyTerminalBlock>();
    Instance.Terminal = GridTerminalSystem;
    Instance.Storage = Storage;


    Instance.Config.AddDefault("general", "ExcludeTag", "exclude", "Blocks with this tag in the name will be completly ignored by the script");

    Routines.Add(new PowerRoutine(Instance));
    Routines.Add(new GasRoutine(Instance));
    Routines.Add(new SecurityRoutine(Instance));
    Routines.Add(new ItemRoutine(Instance));
    Routines.Add(new ScreenRoutine(Instance));


    Instance.Log.LogUP("CNF | Booting up...", Runtime.LastRunTimeMs, Runtime.CurrentInstructionCount);
    yield return true;

    Instance.Screens.Add("debuglog", new List<Line>());
    Instance.Screens.Add("debugscreen", new List<Line>());

    Instance.Config.Check();
    Instance.Excludes = Instance.Config.GetString("general", "ExcludeTag") != null ? Instance.Config.GetString("general", "ExcludeTag").Split(';') : new string[] { "exclude" };
    Instance.Log.LogUP("CNF | Config checked", Runtime.LastRunTimeMs, Runtime.CurrentInstructionCount);
    yield return true;

    for (int Run = 0; ; Run++)
    {
        if (Run % 40 == 0)
        {
            if (Instance.Config.Check())
            {
                Instance.Excludes = Instance.Config.GetString("general", "ExcludeTag") != null ? Instance.Config.GetString("general", "ExcludeTag").Split(';') : new string[] { "exclude" };
                Routines.ForEach(r => r.BlockListUpdated());
            }
            Instance.Log.LogUP("CNF | Config checked", Runtime.LastRunTimeMs);
            yield return true;

            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(NewBlockList, block => block.IsSameConstructAs(Me) && !Instance.ContainsAny(block.CustomName, Instance.Excludes));
            var firstNotSecond = NewBlockList.Except(Instance.AllBlocks, new BlockComparator()).ToList();
            var secondNotFirst = Instance.AllBlocks.Except(NewBlockList, new BlockComparator()).ToList();
            Instance.Log.LogUP("CNF | Blocklist checked (" + Instance.AllBlocks.Count + ")", Runtime.LastRunTimeMs, Runtime.CurrentInstructionCount);
            yield return true;

            if (firstNotSecond.Any() || secondNotFirst.Any())
            {
                Instance.AllBlocks = NewBlockList;
                Routines.ForEach(r => r.BlockListUpdated());
                Instance.Log.LogUP("CNF | Blocklist changed (" + Instance.AllBlocks.Count + ")", Runtime.LastRunTimeMs, Runtime.CurrentInstructionCount);
                yield return true;
            }
        }

        Exception error = null;
        foreach(AbstractBaseRoutine routine in Routines)
        {
            if (routine.Enabled)
            {
                for (int i = 0; i < routine.RunsPerCycle; i++)
                {
                    try
                    {
                        routine.MoveNext();
                    }
                    catch (Exception exception)
                    {
                        error = exception;
                    }
                    if (error != null)
                    {
                        string errorMessage = "ERROR IN [" + routine.RoutineName + "]\n" + error.ToString();
                        Instance.Log.LogUP(errorMessage, Runtime.LastRunTimeMs, Runtime.CurrentInstructionCount);
                        Instance.Screens["debugscreen"].Add(new Line(errorMessage));
                        yield return true;
                        routine.BlockListUpdated();
                        error = null;
                    }
                    yield return true;
                }
            }
        }

        EchoLog(Run);
    }
}

public void EchoLog(int run)
{
    string output = "SmartOS\n" +
    "Run: " + run + "\n" +
    "Avg Runtime: " + Instance.Log.AverageLogTime.ToString("0.000") + "ms\n" +
    "Avg Instructions: " + Instance.Log.AverageInstructions.ToString("0") + "\n";

    for (int i = Instance.Log.Logs.Count - 1; i >= 0; i--)
    {
        output += "\n[" + Instance.Log.Logs[i].time.ToString("0.000") + "] " + Instance.Log.Logs[i].message;
    }

    Instance.Screens["debuglog"].Clear();
    Instance.Screens["debuglog"].Add(new Line(output));
    Echo(output);
}


public void Main(string argument, UpdateType updateSource)
{

    _mainRoutine.MoveNext();
}

public abstract class AbstractBaseRoutine
{
    public string RoutineName;
    protected InstanceHolder Inst;
    private IEnumerator<string> Iterator;
    protected float Step = 0;
    protected DateTime CycleStartTime;
    public long CycleTime = 0;

    protected AbstractBaseRoutine(string routineName, InstanceHolder instance)
    {
        RoutineName = routineName;
        Inst = instance;
        AddDefaultConfig("enabled", "true", "Enable or disable this routine");
        AddDefaultConfig("runsPerCycle", "1", "How many times this routine should be called each Cycle");

        Enabled = EnabledByConfig;
        RunsPerCycle = RunsPerCycleByConfig;
    }

    public abstract IEnumerator<string> Run();

    protected void AddDefaultConfig(string key, string value, string description = null)
    {
        Inst.Config.AddDefault(RoutineName, key, value, description);
    }

    public int RunsPerCycle = 1;

    protected int RunsPerCycleByConfig { get { return Inst.Config.GetInt(RoutineName, "runsPerCycle");  } }

    public bool Enabled = true;

    protected bool EnabledByConfig
    {
        get { return Inst.Config.GetBool(RoutineName, "enabled"); }
    }

    public bool MoveNext()
    {
        if(Iterator == null)
        {
            Iterator = Run();
        }
        Iterator.MoveNext();
        Log(Iterator.Current);
        return true;
    }

    public void Event(string name)
    {
        string realName = Inst.Config.GetString(RoutineName, name);
        int timer = 0;
        Inst.Timers.ForEach(block =>
        {
            if( block.DisplayNameText.Contains(realName))
            {
                timer++;
                block.Trigger();
            }
        });
    }

    public void Log(string message)
    {
        Inst.Log.LogUP(RoutineName+ " | " + message, Inst.Runtime.LastRunTimeMs, Inst.Runtime.CurrentInstructionCount);
    }

    public void BlockListUpdated() {
        if (Iterator != null)
        {
            Iterator.Dispose();
            Iterator = null;
        }
    }

    public void AddScreen(string name)
    {
        Inst.Screens.Add(name, new List<Line>());
        AddToScreen(name, new Line("loading " + name + "..."));
        Inst.ReloadScreen = true;
    }

    public void RemoveScreen(string name)
    {
        Inst.Screens.Remove(name);
    }

    public void ResetScreen(string name)
    {
        Inst.Screens[name].Clear();
    }


    public void AddToScreen(string name, params Line[] lines)
    {
        foreach (Line line in lines)
        {
            Inst.Screens[name].Add(line);
        }
    }

    public void AddToScreenFront(string name, params Line[] lines)
    {
        foreach (Line line in lines)
        {
            Inst.Screens[name].Insert(0, line);
        }
    }

    public void ResetStep()
    {
        Step = 0;
    }

    public bool CanContinue(float increasae)
    {
        Step += increasae;
        if(Step > 25)
        {
            Step = 0;
            return false;
        } else
        {
            return true;
        }
    }

    protected void SetDefaultIfNeeded(string block, string value)
    {
        SetDefaultIfNeeded(RoutineName, block, value);
    }

    protected void SetDefaultIfNeeded(string section, string block, string value)
    {
        if (!Inst.Config.Has(section, block))
        {
            Inst.Config.Set(section, block, value);
        }
    }

    protected float GetMaxMeasuredOutput(IMyTerminalBlock producer)
    {
        if (!Inst.Config.Has("PassiveProducers", producer.BlockDefinition.SubtypeName))
        {
            Inst.Config.Set("PassiveProducers", producer.BlockDefinition.SubtypeName, "0");
            Inst.Config.Save();
        }

        return (float)Inst.Config.GetDouble("PassiveProducers", producer.BlockDefinition.SubtypeName);
    }

    protected void StartCycle()
    {
        CycleStartTime = DateTime.Now;
    }

    protected void EndCycle()
    {
        CycleTime = SecondsSinceCycleStart();
        StartCycle();
    }

    protected long SecondsSinceCycleStart()
    {
        return ToUnixTime(DateTime.Now) - ToUnixTime(CycleStartTime);
    }

    protected long ToUnixTime(DateTime date)
    {
        return  (date.Hour * 3600) + (date.Minute * 60) + date.Second;
    }

    protected void Debug(string log, bool attach = false)
    {
        if(attach && Inst.Screens["debugscreen"].Count > 0)
        {
            Inst.Screens["debugscreen"][0].BaseText += " " + log + "\n";
        } else
        {
            Line line = new Line(log + "\n");
            Inst.Screens["debugscreen"].Insert(0, line);
        }
    }

    protected void ResetDebug()
    {
    }
}

public abstract class AbstractStatusList<T> : List<T>
{
    public double MaxCapacity { get; private set; }
    public double CurrentCapacity { get; private set; }
    public double PastCapacity { get; private set; }
    public double CurrentCapacityPercentage { get; private set; }
    public double PastCapacityPercentage { get; private set; }

    public double CapacityChange { get; private set; }

    new public void Clear()
    {
        base.Clear();
        MaxCapacity = 0;
        CurrentCapacity = 0;
        PastCapacity = 0;
        CurrentCapacityPercentage = 0;
        PastCapacityPercentage = 0;
    }

    public void Load(List<IMyTerminalBlock> blocks)
    {
        this.Clear();
        this.AddList(blocks.FindAll(block => Matches(block)).Cast<T>().ToList());
        this.MaxCapacity = GetTotalMaxCapacity();
    }

    protected virtual bool Matches(IMyTerminalBlock block)
    {
        return block is T;
    }

    protected double GetTotalMaxCapacity()
    {
        double result = 0;
        this.ForEach(block => result += GetMaxCapacity(block));
        return result;
    }

    protected double GetTotalCapacity()
    {
        double result = 0;
        this.ForEach(block => result += GetCapacity(block));
        return result;
    }

    public void Refresh()
    {
        this.PastCapacity = this.CurrentCapacity;
        this.PastCapacityPercentage = this.CurrentCapacityPercentage;
        this.CurrentCapacity = this.GetTotalCapacity();
        this.CurrentCapacityPercentage = this.CurrentCapacity / this.MaxCapacity;
        this.CapacityChange = this.CurrentCapacity - this.PastCapacity;
        if (double.IsNaN(this.CurrentCapacityPercentage) || double.IsInfinity(this.CurrentCapacityPercentage))
        {
            this.CurrentCapacityPercentage = 0;
        }
    }

    protected abstract double GetMaxCapacity(T block);
    protected abstract double GetCapacity(T block);

}

public class BlockComparator : IEqualityComparer<IMyTerminalBlock>
{

        public bool Equals(IMyTerminalBlock x, IMyTerminalBlock y)
        {
            return x.EntityId.Equals(y.EntityId) && string.Equals(x.DisplayNameText, y.DisplayNameText);
        }

        public int GetHashCode(IMyTerminalBlock obj)
        {
            return obj.EntityId.GetHashCode() + obj.Name.GetHashCode();
        }

}

public class ColorUtils
{
    public static Color FromString(string color)
    {
        if (color == null || !color.Contains(",")) return Color.White;
        string[] colorParts = color.Split(',');
        if (colorParts.Length < 3) return Color.White;
        int[] rgb = new int[] { 0,0,0};
        int.TryParse(colorParts[0], out rgb[0]);
        int.TryParse(colorParts[1], out rgb[1]);
        int.TryParse(colorParts[2], out rgb[2]);
        return new Color(rgb[0], rgb[1], rgb[2]);
    }

    public static string ToString(Color color)
    {
        return string.Format("{0},{1},{2}", color.R, color.G, color.B);
    }

    public static Color PercentColor(double percentage)
    {
        double redPercent = Math.Min(200 - (percentage * 2), 100) / 100f;
        double greenPercent = Math.Min(percentage * 2, 100) / 100f;

        int red = (int)(0 + ((255 - 0) * redPercent));
        int green = (int)(0 + ((255 - 0) * greenPercent));

        return new Color(red, green, 0);
    }
}

public class ConfigHelper
{
    public const string SETTINGS = "settings";

    IMyTerminalBlock Block;
    string OldCustomData = "?";
    readonly MyIni Defaults = new MyIni();
    readonly MyIni Ini = new MyIni();
    List<MyIniKey> KeysDefaults = new List<MyIniKey>();
    List<MyIniKey> KeysIni = new List<MyIniKey>();

    public ConfigHelper(IMyTerminalBlock block)
    {
        this.Block = block;
        Check();
    }

    public void AddDefault(string section, string key, string value, string description = null)
    {
        Defaults.Set(section, key, value);
        if(description != null)
        {
            Defaults.SetComment(section, key, description);
        }
    }

    public void AddDefault(string key, string value)
    {
        Defaults.Set(SETTINGS, key, value);
    }

    public void Set(string section, string key, string value)
    {
        this.Ini.Set(section, key, value);
    }

    public void Set(string section, string key, bool value)
    {
        this.Ini.Set(section, key, value);
    }

    public void Set(string section, string key, int value)
    {
        this.Ini.Set(section, key, value);
    }

    public void Set(string section, string key, double value)
    {
        this.Ini.Set(section, key, value);
    }

    public string GetString(string section, string key)
    {
        return this.Ini.Get(section, key).ToString();
    }

    public bool GetBool(string section, string key)
    {
        return this.Ini.Get(section, key).ToBoolean();
    }

    public int GetInt(string section, string key)
    {
        return this.Ini.Get(section, key).ToInt32();
    }

    public long GetLong(string section, string key)
    {
        return this.Ini.Get(section, key).ToInt64();
    }

    public double GetDouble(string section, string key)
    {
        return this.Ini.Get(section, key).ToDouble();
    }

    public bool Has(string section, string key)
    {
        return this.Ini.ContainsKey(section, key) && !GetString(section, key).Trim().Equals("");
    }

    public List<MyIniKey> getSectionKeys(string section)
    {
        List<MyIniKey> result = new List<MyIniKey>();
        Ini.GetKeys(section, result);
        return result;
    }


    public bool Check()
    {
        if(OldCustomData != Block.CustomData || !Validate())
        {
            OldCustomData = Block.CustomData;
            if (!Ini.TryParse(Block.CustomData) || !Validate())
            {
                this.InitCustomData();
            }
            return true;
        }
        return false;
    }

    public void InitCustomData()
    {
        Block.CustomData = Defaults.ToString();
        Check();
    }

    public void Save()
    {
        Block.CustomData = Ini.ToString();
        this.OldCustomData = Block.CustomData;
    }

    public bool Validate()
    {
        Defaults.GetKeys(KeysDefaults);
        Ini.GetKeys(KeysIni);
        if (KeysDefaults.Count == 0)
        {
            return true;
        } else if (KeysIni.Count < KeysDefaults.Count)
        {
            return false;
        }
        var firstNotSecond = KeysDefaults.Except(KeysIni).ToList();
        return firstNotSecond.Count == 0;
    }

}

public class GasRoutine : AbstractBaseRoutine
{
    MyGasGeneratorList GeneratorList = new MyGasGeneratorList();
    MyGasTankList OxygenTankList = new MyGasTankList(false);
    MyGasTankList HydrogenTankList = new MyGasTankList(true);
    MyOxygenFarmList OxygenFarms = new MyOxygenFarmList(new Dictionary<string, double>());

    List<IMyGasGenerator> EmptyGeneratorList = new List<IMyGasGenerator>();
    int GeneratorsActive = 0;

    double TargetH2Level;
    double TargetO2Level;
    bool EmptyGeneratorsOnHUD;
    int EmptyGeneratorListLength;

    public GasRoutine(InstanceHolder instance) : base("GAS", instance)
    {
        AddDefaultConfig("HydrogenLevel", "80", "Toggle O2 Generators on if Hydrogenlevel is below this percentage");
        AddDefaultConfig("OxygenLevel", "80", "Toggle O2 Generators on if Oxygenlevel is below this percentage");
        AddDefaultConfig("EmptyGeneratorsOnHUD", "false", "Show O2 Generators on HUD, that dont have Ice");
        AddDefaultConfig("EmptyGeneratorListLength", "3", "How many empty O2 Generators should be displayed on the panel");

        AddDefaultConfig("TimerNameGasGeneratorOnline", "GasGeneratorOnline", "Timers, that have this tag in their name will be triggered when O2 generators are turned on");
        AddDefaultConfig("TimerNameGasGeneratorOffline", "GasGeneratorOffline", "Timers, that have this tag in their name will be triggered when O2 generators are turned off");

        AddScreen("gas");
    }

    public override IEnumerator<string> Run()
    {
        AddDefaultPassives();
        Enabled = EnabledByConfig;
        RunsPerCycle = RunsPerCycleByConfig;
        TargetO2Level = Inst.Config.GetInt(RoutineName, "OxygenLevel") / 100f;
        TargetH2Level = Inst.Config.GetInt(RoutineName, "HydrogenLevel") / 100f;
        EmptyGeneratorListLength = Inst.Config.GetInt(RoutineName, "EmptyGeneratorListLength");
        EmptyGeneratorsOnHUD = Inst.Config.GetBool(RoutineName, "EmptyGeneratorsOnHUD");
        OxygenFarms.OutputTable.Clear();
        foreach (MyIniKey key in Inst.Config.getSectionKeys("PassiveProducers"))
        {
            OxygenFarms.OutputTable.Add(key.Name, Inst.Config.GetDouble(key.Section, key.Name));
        }
        yield return "Loaded Routine Config";

        GeneratorList.Load(Inst.AllBlocks);
        yield return "Loaded " + GeneratorList.Count + " O2 Generators";

        OxygenTankList.Load(Inst.AllBlocks);
        yield return "Loaded " + OxygenTankList.Count + " O2 Tanks";

        HydrogenTankList.Load(Inst.AllBlocks);
        yield return "Loaded " + HydrogenTankList.Count + " H2 Tanks";

        OxygenFarms.Load(Inst.AllBlocks);
        yield return "Loaded " + OxygenFarms.Count + " O2 Farms";

        for (int Run = 0; ; Run++)
        {
            if (OxygenFarms.Count > 0)
            {
                OxygenFarms.Refresh();
                yield return "Checked " + OxygenFarms.Count + " Farms";
            }

            if (OxygenTankList.Count > 0)
            {
                OxygenTankList.Refresh();
                yield return "Checked " + OxygenTankList.Count + " O2 Tanks";
            }

            if (HydrogenTankList.Count > 0)
            {
                HydrogenTankList.Refresh();
                yield return "Checked " + HydrogenTankList.Count + " H2 Tanks";
            }

            if (GeneratorList.Count > 0)
            {
                GeneratorList.Refresh();
                EmptyGeneratorList.Clear();
                foreach (IMyGasGenerator generator in GeneratorList)
                {
                    generator.Enabled = (OxygenTankList.CurrentCapacityPercentage < TargetO2Level || HydrogenTankList.CurrentCapacityPercentage < TargetH2Level);
                    if (generator.GetInventory(0).ItemCount == 0)
                    {
                        EmptyGeneratorList.Add(generator);
                        if (EmptyGeneratorsOnHUD) generator.ShowOnHUD = true;
                    }
                    else if (EmptyGeneratorsOnHUD) generator.ShowOnHUD = false;
                }
                yield return "Checked " + GeneratorList.Count + " O2 Generators";

                if (OxygenTankList.CurrentCapacityPercentage < TargetO2Level || HydrogenTankList.CurrentCapacityPercentage < TargetH2Level)
                {
                    if (GeneratorsActive != 1)
                    {
                        Event("TimerNameGasGeneratorOnline");
                        yield return "Turned O2 Gneerator on";
                    }
                    GeneratorsActive = 1;
                }
                else
                {
                    if (GeneratorsActive != -1)
                    {
                        Event("TimerNameGasGeneratorOffline");
                        yield return "Turned O2 Generators off";
                    }
                    GeneratorsActive = -1;
                }
            }
            EndCycle();
            PrintScreen();
            yield return "Gas Cycle complete";
        }
    }

    private void PrintScreen()
    {
        ResetScreen("gas");
        AddToScreen("gas", new Line("$ " + Inst.LcdHelper.SecondsToTime(CycleTime), "Gas"), new Line("-"));
        if (OxygenTankList.Count > 0)
        {
            AddTankLines("O2 Tanks", OxygenTankList);
        }

        if (HydrogenTankList.Count > 0)
        {
            AddTankLines("H2 Tanks", HydrogenTankList);
        }
        if (OxygenFarms.Count > 0)
        {

            AddToScreen("gas", new Line(String.Format(Inst.LcdHelper.GREEN_SQUARE + " {0} O2 Farms", OxygenFarms.Count)));
            AddToScreen("gas", new Line(String.Format(
                Inst.LcdHelper.BLACK_SQUARE + " {0,8} / {1,8} § {2,3:0}%",
                Inst.LcdHelper.GasString(OxygenFarms.CurrentCapacity),
                Inst.LcdHelper.GasString(OxygenFarms.MaxCapacity),
                OxygenFarms.CurrentCapacityPercentage * 100
                ), (OxygenFarms.CurrentCapacityPercentage).ToString()));
        }
        if (GeneratorList.Count > 0)
        {
            AddToScreen("gas", new Line(String.Format(
                (GeneratorsActive == 1 ? Inst.LcdHelper.YELLOW_SQUARE : Inst.LcdHelper.GREEN_SQUARE) + " {0} $",
                GeneratorList.Count
                ), "O2 Gens"));

            for (int i = 0; i < Math.Min(EmptyGeneratorListLength, EmptyGeneratorList.Count); i++)
            {
                AddToScreen("gas", new Line(Inst.LcdHelper.BLACK_SQUARE + " - " + EmptyGeneratorList[i].DisplayNameText + " empty"));
            }
            if (EmptyGeneratorList.Count > EmptyGeneratorListLength)
            {
                AddToScreen("gas", new Line(Inst.LcdHelper.BLACK_SQUARE + " - " + (EmptyGeneratorList.Count - EmptyGeneratorListLength) + " others empty"));
            }
        }
    }

    private void AddTankLines(string name, MyGasTankList source)
    {
        double mwh = Math.Abs(source.CapacityChange) / CycleTime * 3600;
        double hours = (source.CurrentCapacity > source.PastCapacity ? (source.MaxCapacity - source.CurrentCapacity) : source.CurrentCapacity) / mwh;

        AddToScreen("gas", new Line(String.Format(
            Inst.LcdHelper.StatusLed(source.CurrentCapacityPercentage * 1.1) + " {0} $ {1,9}/h",
            source.Count,
            (source.PastCapacity <= source.CurrentCapacity ? '+' : '-') + " " + Inst.LcdHelper.GasString(mwh)
            ), name, 1));
        AddToScreen("gas", new Line(String.Format(
            Inst.LcdHelper.StatusLed(source.CurrentCapacityPercentage * 1.1) + " {0} $ {1}",
            source.Count,
            (source.PastCapacity <= source.CurrentCapacity ? '+' : '-') + " " + Inst.LcdHelper.HoursToTime(hours)
            ), name, 2));

        AddToScreen("gas", new Line(String.Format(
            Inst.LcdHelper.BLACK_SQUARE + " {0,8} / {1,8} § {2,3:0}%",
            Inst.LcdHelper.GasString(source.CurrentCapacity),
            Inst.LcdHelper.GasString(source.MaxCapacity),
            source.CurrentCapacityPercentage * 100
            ), (source.CurrentCapacityPercentage).ToString()));
    }

    private void AddDefaultPassives()
    {
        SetDefaultIfNeeded("PassiveProducers", "LargeBlockOxygenFarm", "1.8");
        Inst.Config.Save();
    }
}

public class InstanceHolder
{
    public ConfigHelper Config;
    public IMyProgrammableBlock Me;
    public LCDHelper LcdHelper;
    private List<IMyTerminalBlock> _AllBlocks = new List<IMyTerminalBlock>();
    public List<IMyTimerBlock> Timers = new List<IMyTimerBlock>();
    public LogHelper Log;
    public IMyGridProgramRuntimeInfo Runtime;
    public IMyGridTerminalSystem Terminal;
    public bool ReloadScreen = false;
    public string[] Excludes = new string[] { "exclude" };

    public Dictionary<string, List<Line>> Screens = new Dictionary<string, List<Line>>();
    public string Storage;

    public List<IMyTerminalBlock> AllBlocks
    {
        get { return _AllBlocks; }
        set
        {
            _AllBlocks.Clear();
            _AllBlocks.AddList(value);
            Timers.Clear();
            _AllBlocks.ForEach(b =>
            {
                if (b is IMyTimerBlock)
                {
                    Timers.Add((IMyTimerBlock)b);
                }
            });
        }
    }

    public bool ContainsAny(string input, string[] checks)
    {
        foreach (string ex in checks) if (input.Contains(ex)) return true;
        return false;
    }
}

public class ItemHelper
{
    public const string MYOB = "MyObjectBuilder_";
    public const string BLUEPRINT = "BlueprintDefinition/";
    public const string CATEGORY_COMPONENT = "Component";
    public const string CATEGORY_ORE = "Ore";
    public const string CATEGORY_INGOT = "Ingot";
    public const string CATEGORY_AMMO = "AmmoMagazine";
    public const string CATEGORY_TOOL = "PhysicalGunObject";
    public const string CATEGORY_PHYSICAL= "PhysicalObject";
    public const string CATEGORY_DATAPAD = "Datapad";
    public const string CATEGORY_CONSUME = "ConsumableItem";
    public const string CATEGORY_OBOTTLE = "OxygenContainerObject";
    public const string CATEGORY_HBOTTLE = "GasContainerObject";

    public Dictionary<string, List<ItemDefinition>> ITEMS = new Dictionary<string, List<ItemDefinition>>();

    private void AddItem(string category, string name, string blueprintName = null)
    {
        if (!ITEMS.ContainsKey(category)) ITEMS.Add(category, new List<ItemDefinition> ());
        ITEMS[category].Add(new ItemDefinition(MYOB + category + "/" + name, (blueprintName != null ? (MYOB + BLUEPRINT + blueprintName) : null)));
    }

    public ItemHelper()
    {
        AddItem(CATEGORY_COMPONENT,"Construction", "ConstructionComponent");
        AddItem(CATEGORY_COMPONENT,"Canvas", "Canvas");
        AddItem(CATEGORY_COMPONENT,"MetalGrid", "MetalGrid");
        AddItem(CATEGORY_COMPONENT,"InteriorPlate", "InteriorPlate");
        AddItem(CATEGORY_COMPONENT,"SteelPlate", "SteelPlate");
        AddItem(CATEGORY_COMPONENT,"Girder", "GirderComponent");
        AddItem(CATEGORY_COMPONENT,"SmallTube", "SmallTube");
        AddItem(CATEGORY_COMPONENT,"LargeTube", "LargeTube");
        AddItem(CATEGORY_COMPONENT,"Motor", "MotorComponent");
        AddItem(CATEGORY_COMPONENT,"Display", "Display");
        AddItem(CATEGORY_COMPONENT,"BulletproofGlass", "BulletproofGlass");
        AddItem(CATEGORY_COMPONENT,"Computer", "ComputerComponent");
        AddItem(CATEGORY_COMPONENT,"Reactor", "ReactorComponent");
        AddItem(CATEGORY_COMPONENT,"Thrust", "ThrustComponent");
        AddItem(CATEGORY_COMPONENT,"GravityGenerator", "GravityGeneratorComponent");
        AddItem(CATEGORY_COMPONENT,"Medical", "MedicalComponent");
        AddItem(CATEGORY_COMPONENT,"RadioCommunication", "RadioCommunicationComponent");
        AddItem(CATEGORY_COMPONENT,"Detector", "DetectorComponent");
        AddItem(CATEGORY_COMPONENT,"Explosives", "ExplosivesComponent");
        AddItem(CATEGORY_COMPONENT,"SolarCell", "SolarCell");
        AddItem(CATEGORY_COMPONENT,"PowerCell", "PowerCell");
        AddItem(CATEGORY_COMPONENT,"Superconductor", "Superconductor");
        AddItem(CATEGORY_AMMO, "NATO_5p56x45mm", "NATO_5p56x45mmMagazine");
        AddItem(CATEGORY_AMMO, "NATO_25x184mm", "NATO_25x184mmMagazine");
        AddItem(CATEGORY_AMMO, "Missile200mm", "Missile200mm");
        AddItem(CATEGORY_TOOL, "AngleGrinderItem", "AngleGrinder1");
        AddItem(CATEGORY_TOOL, "AngleGrinder2Item", "AngleGrinder2");
        AddItem(CATEGORY_TOOL, "AngleGrinder3Item", "AngleGrinder3");
        AddItem(CATEGORY_TOOL, "AngleGrinder4Item", "AngleGrinder4");
        AddItem(CATEGORY_TOOL, "HandDrillItem", "HandDrill1");
        AddItem(CATEGORY_TOOL, "HandDrill2Item", "HandDrill2");
        AddItem(CATEGORY_TOOL, "HandDrill3Item", "HandDrill3");
        AddItem(CATEGORY_TOOL, "HandDrill4Item", "HandDrill4");
        AddItem(CATEGORY_TOOL, "WelderItem", "Welder1");
        AddItem(CATEGORY_TOOL, "Welder2Item", "Welder2");
        AddItem(CATEGORY_TOOL, "Welder3Item", "Welder3");
        AddItem(CATEGORY_TOOL, "Welder4Item", "Welder4");
        AddItem(CATEGORY_TOOL, "AutomaticRifleItem", "AutomaticRifle");
        AddItem(CATEGORY_TOOL, "PreciseAutomaticRifleItem", "PreciseAutomaticRifle");
        AddItem(CATEGORY_TOOL, "RapidFireAutomaticRifleItem", "RapidFireAutomaticRifle");
        AddItem(CATEGORY_TOOL, "UltimateAutomaticRifleItem", "UltimateAutomaticRifle");
        AddItem(CATEGORY_INGOT, "Cobalt");
        AddItem(CATEGORY_INGOT, "Gold");
        AddItem(CATEGORY_INGOT, "Iron");
        AddItem(CATEGORY_INGOT, "Magnesium");
        AddItem(CATEGORY_INGOT, "Nickel");
        AddItem(CATEGORY_INGOT, "Platinum");
        AddItem(CATEGORY_INGOT, "Silicon");
        AddItem(CATEGORY_INGOT, "Silver");
        AddItem(CATEGORY_INGOT, "Stone");
        AddItem(CATEGORY_INGOT, "Uranium");
        AddItem(CATEGORY_ORE, "Cobalt");
        AddItem(CATEGORY_ORE, "Gold");
        AddItem(CATEGORY_ORE, "Ice");
        AddItem(CATEGORY_ORE, "Iron");
        AddItem(CATEGORY_ORE, "Magnesium");
        AddItem(CATEGORY_ORE, "Nickel");
        AddItem(CATEGORY_ORE, "Platinum");
        AddItem(CATEGORY_ORE, "Scrap");
        AddItem(CATEGORY_ORE, "Silicon");
        AddItem(CATEGORY_ORE, "Silver");
        AddItem(CATEGORY_ORE, "Stone");
        AddItem(CATEGORY_ORE, "Uranium");
        AddItem(CATEGORY_HBOTTLE, "HydrogenBottle", "HydrogenBottle");
        AddItem(CATEGORY_OBOTTLE, "OxygenBottle", "OxygenBottle");
        AddItem(CATEGORY_DATAPAD, "Datapad");
        AddItem(CATEGORY_CONSUME, "Medkit");
        AddItem(CATEGORY_CONSUME, "Powerkit");
        AddItem(CATEGORY_CONSUME, "CosmicCoffee");
        AddItem(CATEGORY_CONSUME, "ClangCola");
        AddItem(CATEGORY_COMPONENT, "ZoneChip");
    }
}

public class ItemDefinition
{
    public string Category;
    public string Name;
    public string Blueprint;

    public ItemDefinition(string id, string blueprint = null)
    {
        Id = id;
        Blueprint = blueprint;
    }

    public string Id
    {
        get { return ItemHelper.MYOB + Category + "/" + Name; }
        set {
            string[] splits = value.Replace(ItemHelper.MYOB, "").Split('/');
            Name = splits[1];
            Category = splits[0];
        }
    }
}

public class ItemRoutine : AbstractBaseRoutine
{
    public const string BOTTLE = "Bottle";

    private readonly ItemHelper IHelper = new ItemHelper();
    Dictionary<string, InventoryItem> Items = new Dictionary<string, InventoryItem>();
    Dictionary<string, ContainerGroup> Groups = new Dictionary<string, ContainerGroup>();

    List<MyInventoryItem> InventoryCache = new List<MyInventoryItem>();
    List<MyInventoryItem> InventoryCache2 = new List<MyInventoryItem>();
    List<MyProductionItem> QueueCache = new List<MyProductionItem>();
    List<string> SimpleItemsCache = new List<string>();
    List<SimpleItemMatch> MatchCache = new List<SimpleItemMatch>();

    IEnumerator<string> SortRoutine;

    List<IMyAssembler> AssemblerList = new List<IMyAssembler>();
    List<IMyRefinery> RefineryList = new List<IMyRefinery>();
    List<IMyTerminalBlock> ContainerList = new List<IMyTerminalBlock>();

    bool Sorting;
    bool Autocrafting;
    bool HideEmpty;
    bool CraftBasic;
    bool CraftSurvival;
    bool RefineSurvival;
    double AutocraftPercentage;

    public ItemRoutine(InstanceHolder instance) : base("ITM", instance)
    {
        AddDefaultConfig("runsPerCycle", "3", "How many times this routine should be called each Cycle");
        AddDefaultConfig("sorting", "false", "Enable Item sorting. Disable if you use sorters.");
        AddDefaultConfig("autocrafting", "true", "Should the script control assemblers to keep stock");
        AddDefaultConfig("hideEmpty", "false", "Hide missing/empty Items on the panels");
        AddDefaultConfig("craftBasicAssemblers", "false", "Should Basic Assemblers be used for autocrafting");
        AddDefaultConfig("craftSurvivalKit", "false", "Should Survivalkits be used for autocrafting");
        AddDefaultConfig("autocraftPercentage", "10", "When autocrafting queue up this percentage of item quota split between all assemblers");

        Groups.Add(ItemHelper.CATEGORY_COMPONENT, new ContainerGroup(ItemHelper.CATEGORY_COMPONENT, ItemHelper.CATEGORY_COMPONENT.ToLower()));
        Groups.Add(ItemHelper.CATEGORY_ORE, new ContainerGroup(ItemHelper.CATEGORY_ORE, ItemHelper.CATEGORY_ORE.ToLower()));
        Groups.Add(ItemHelper.CATEGORY_INGOT, new ContainerGroup(ItemHelper.CATEGORY_INGOT, ItemHelper.CATEGORY_INGOT.ToLower()));
        Groups.Add(ItemHelper.CATEGORY_AMMO, new ContainerGroup("Ammo", "ammo", ItemHelper.CATEGORY_AMMO.ToLower()));
        Groups.Add(ItemHelper.CATEGORY_TOOL, new ContainerGroup("Tool", "tool", ItemHelper.CATEGORY_TOOL.ToLower(), ItemHelper.CATEGORY_PHYSICAL.ToLower(), ItemHelper.CATEGORY_CONSUME.ToLower(), ItemHelper.CATEGORY_DATAPAD.ToLower()));
        Groups.Add(BOTTLE, new ContainerGroup(BOTTLE, BOTTLE.ToLower(), ItemHelper.CATEGORY_OBOTTLE.ToLower(), ItemHelper.CATEGORY_HBOTTLE.ToLower()));


        AddScreen("cargo");
        foreach (ContainerGroup group in Groups.Values)
        {
            AddScreen(group.SearchTag[0]);
        }
    }


    public override IEnumerator<string> Run()
    {
        Sorting = Inst.Config.GetBool(RoutineName, "sorting");
        Autocrafting = Inst.Config.GetBool(RoutineName, "autocrafting");
        HideEmpty = Inst.Config.GetBool(RoutineName, "hideEmpty");
        CraftBasic = Inst.Config.GetBool(RoutineName, "craftBasicAssemblers");
        CraftSurvival = Inst.Config.GetBool(RoutineName, "craftSurvivalKit");
        AutocraftPercentage = Inst.Config.GetDouble(RoutineName, "autocraftPercentage") / 100f;

        AddVanillaItems();

        AssemblerList.Clear();
        RefineryList.Clear();
        ContainerList.Clear();

        foreach (ContainerGroup group in Groups.Values)
        {
            group.Reset();
        }
        yield return "Loaded Routine Config";



        foreach (MyIniKey item in Inst.Config.getSectionKeys("ItemsQuotas"))
        {
            if (!Items.ContainsKey(item.Name))
            {
                string blueprint = Inst.Config.Has("Blueprints", item.Name) ? (ItemHelper.MYOB + ItemHelper.BLUEPRINT + Inst.Config.GetString("Blueprints", item.Name)) : null;

                addItem(item.Name, blueprint);
            }
            Items[item.Name].MinAmount = Inst.Config.GetLong("ItemsQuotas", item.Name);
        }
        Inst.Config.Save();
        yield return "Loaded Itemquotas";

        foreach (IMyTerminalBlock block in Inst.AllBlocks)
        {
            if (block.HasInventory)
            {
                if (block is IMyRefinery) RefineryList.Add((IMyRefinery)block);
                else if (block is IMyAssembler) AssemblerList.Add((IMyAssembler)block);
                else
                {
                    ContainerList.Add(block);
                    CheckIfSortContainer(block);
                    Step += 2;

                }
            }
            if (!CanContinue(0.5f))
            {
                yield return "Initializing Item Distribution...";
            }
        }
        yield return "Loaded Item Distribution";


        foreach (InventoryItem item in Items.Values)
        {
            item.SortTargetBlocks();
        }

        yield return "Sorted Containers";


        for (int Run = 0; ; Run++)
        {
            foreach (InventoryItem item in Items.Values)
            {
                item.OldAmount = item.CurrentAmount;
                item.CurrentAmount = 0;
            }

            yield return "Started Cargo Index";
            ResetStep();

            foreach (ContainerGroup group in Groups.Values)
            {
                group.OldVolume = group.CurrentVolume;
                group.CurrentVolume = 0;

                foreach (IMyInventory inven in group.Inventories)
                {
                    group.CurrentVolume += (float)inven.CurrentVolume;
                }
            }
            yield return "Checked Sortcontainers";

            EndCycle();
            PrintCargoScreen();
            yield return "Updated Cargo Status";


            foreach (IMyTerminalBlock container in ContainerList)
            {
                for (int i = 0; i < container.InventoryCount; i++)
                {
                    CountInventory(container.GetInventory(i));
                }
                yield return "Scanned: " + container.DisplayNameText;
            }

            foreach (IMyTerminalBlock container in AssemblerList)
            {
                for (int i = 0; i < container.InventoryCount; i++)
                {
                    CountInventory(container.GetInventory(i));
                }
                yield return "Scanned: " + container.DisplayNameText;
            }

            foreach (IMyTerminalBlock container in RefineryList)
            {
                for (int i = 0; i < container.InventoryCount; i++)
                {
                    CountInventory(container.GetInventory(i));
                }
                yield return "Scanned: " + container.DisplayNameText;
            }


            ResetStep();
            foreach (ContainerGroup group in Groups.Values)
            {
                group.DisplayCache.Clear();
            }

            foreach (InventoryItem item in Items.Values)
            {
                if (item.TargetGroup == null || (item.MinAmount == 0 && item.CurrentAmount == 0 && HideEmpty)) continue;
                item.TargetGroup.DisplayCache.Add(GetItemLine(item));
                if (!CanContinue(1))
                {
                    yield return "Updating Inventory List...";
                }
            }

            foreach (ContainerGroup group in Groups.Values)
            {
                ResetScreen(group.SearchTag[0]);
                AddToScreen(group.SearchTag[0], new Line(group.Name), new Line("-"));
                AddToScreen(group.SearchTag[0], group.DisplayCache.ToArray());
            }
            yield return "Updated Cargo Levels";


            if (Autocrafting)
            {
                foreach (IMyAssembler assembler in AssemblerList)
                {
                    if (assembler.Mode != MyAssemblerMode.Assembly) continue;
                    if (!CraftBasic && assembler.BlockDefinition.ToString().Contains("Basic")) continue;
                    if (!CraftSurvival && assembler.BlockDefinition.ToString().Contains("Survival")) continue;
                    SimpleItemsCache.Clear();
                    assembler.GetQueue(QueueCache);
                    QueueCache.ForEach(item => SimpleItemsCache.Add(item.BlueprintId.ToString()));
                    foreach (InventoryItem item in Items.Values)
                    {
                        if (item.Blueprint != null && item.CurrentAmount < item.MinAmount && !SimpleItemsCache.Contains(item.Blueprint))
                        {
                            decimal amount = (decimal)Math.Max(1, Math.Ceiling(item.MinAmount / AssemblerList.Count * AutocraftPercentage));
                            assembler.AddQueueItem(MyDefinitionId.Parse(item.Blueprint), amount);
                        }
                    }
                    yield return "Crafting in " + assembler.DisplayNameText;
                }
            }

            if (Sorting)
            {
                foreach (IMyRefinery refinery in RefineryList)
                {
                    SortRoutine = SortInventory(refinery, 1);
                    while (SortRoutine.MoveNext())
                    {
                        yield return SortRoutine.Current;
                    }
                }

                foreach (IMyAssembler assembler in AssemblerList)
                {
                    SortRoutine = SortInventory(assembler, assembler.Mode == MyAssemblerMode.Assembly ? 1 : 0);
                    while (SortRoutine.MoveNext())
                    {
                        yield return SortRoutine.Current;
                    }

                    if (assembler.IsQueueEmpty)
                    {
                        SortRoutine = SortInventory(assembler, assembler.Mode == MyAssemblerMode.Assembly ? 0 : 1);
                        while (SortRoutine.MoveNext())
                        {
                            yield return SortRoutine.Current;
                        }
                    }
                }

                foreach (IMyTerminalBlock block in ContainerList)
                {
                    if (block is IMyReactor || block is IMyGasGenerator || block is IMyUserControllableGun) continue;
                    for (int j = 0; j < block.InventoryCount; j++)
                    {
                        SortRoutine = SortInventory(block, j);
                        while (SortRoutine.MoveNext())
                        {
                            yield return SortRoutine.Current;
                        }
                    }
                }

                yield return "Item Cycle complete";
            }
        }
    }
    private IEnumerator<string> SortInventory(IMyTerminalBlock block, int inventory, bool resetStep = false)
    {
        return SortInventory(block, block.GetInventory(inventory), resetStep);
    }

    private IEnumerator<string> SortInventory(IMyTerminalBlock sourceBlock, IMyInventory inventory, bool resetStep = true)
    {
        if (resetStep) ResetStep();
        InventoryCache.Clear();
        inventory.GetItems(InventoryCache);
        if (!CanContinue(1)) yield return "Sorting " + sourceBlock.DisplayNameText + "...";
        foreach (MyInventoryItem item in InventoryCache)
        {
            InventoryItem realItem = Items[item.Type.ToString()];
            MySortContainer sortContainer = realItem.SortTargets.Find(st => st.Block.Equals(sourceBlock));
            float itemVolume = item.Type.GetItemInfo().Volume;
            float itemsMoveAmount = sortContainer != null ? 0 : (float)item.Amount;

            if (!CanContinue(1)) yield return "Sorting " + sourceBlock.DisplayNameText + "...";

            if (itemsMoveAmount > 0)
            {
                foreach (MySortContainer targetContainer in realItem.SortTargets)
                {
                    if (sourceBlock.Equals(targetContainer.Block)) continue;
                    else if (targetContainer.MaxAmount == 0) continue;


                    for (int i = 0; i < targetContainer.Block.InventoryCount; i++)
                    {
                        IMyInventory targetInventory = targetContainer.Block.GetInventory(i);

                        if ((float)(targetInventory.MaxVolume - targetInventory.CurrentVolume) > itemsMoveAmount * itemVolume)
                        {
                            if (inventory.TransferItemTo(targetInventory, item, (MyFixedPoint)itemsMoveAmount))
                            {
                                ResetStep();
                                yield return "Moved " + Inst.LcdHelper.AmountString(itemsMoveAmount) + " " + realItem.Name + " from " + sourceBlock.DisplayNameText + " to " + targetContainer.Block.DisplayNameText;
                                itemsMoveAmount = 0;
                            }
                            else
                            {
                                ResetStep();
                                yield return "Could not move " + Inst.LcdHelper.AmountString(itemsMoveAmount) + " " + realItem.Name + " from " + sourceBlock.DisplayNameText + " to " + targetContainer.Block.DisplayNameText;
                            }
                        }
                        else if ((float)(targetInventory.MaxVolume - targetInventory.CurrentVolume) > itemVolume)
                        {
                            MyFixedPoint toMove = (targetInventory.MaxVolume - targetInventory.CurrentVolume) * (MyFixedPoint)(1 / itemVolume);
                            if (inventory.TransferItemTo(targetInventory, item, toMove))
                            {
                                ResetStep();
                                yield return "Moved " + Inst.LcdHelper.AmountString((float)toMove) + " " + realItem.Name + " from " + sourceBlock.DisplayNameText + " to " + targetContainer.Block.DisplayNameText;
                                itemsMoveAmount -= (float)toMove;
                            }
                            else
                            {
                                yield return "Could not move " + Inst.LcdHelper.AmountString((float)toMove) + " " + realItem.Name + " from " + sourceBlock.DisplayNameText + " to " + targetContainer.Block.DisplayNameText;
                            }
                        }
                        if (!CanContinue(2)) yield return "Sorting " + sourceBlock.DisplayNameText + "...";
                        if (itemsMoveAmount <= 0) break;
                    }
                    if (itemsMoveAmount <= 0) break;
                }
            }
        }
        yield return "Sorted " + sourceBlock.DisplayNameText;
    }

    private void PrintCargoScreen()
    {
        ResetScreen("cargo");
        AddToScreen("cargo", new Line("$ " + Inst.LcdHelper.SecondsToTime(CycleTime), "Cargoholds"), new Line("-"));
        foreach (ContainerGroup group in Groups.Values)
        {
            float percent = group.CurrentVolume / Math.Max(1, group.MaxVolume);
            AddToScreen("cargo", new Line(String.Format("{0} {1,2} {2,-10} § {3,3:0}%",
                Inst.LcdHelper.StatusLed((1 - percent) * 1.1),
                group.Inventories.Count,
                group.Name,
                percent * 100f
                ), percent.ToString()));
        }
    }


    public Line GetItemLine(InventoryItem item)
    {
        if (item.MinAmount > 0)
        {
            float percent = item.CurrentAmount / item.MinAmount;
            return new Line(String.Format(
                "{0} {1,-12} \u2502 {2,6} / {3,6} § {4,3:0}%",
                Inst.LcdHelper.StatusLed(item.CurrentAmount, item.MinAmount),
                MaxLength(item.Name, 12),
                Inst.LcdHelper.AmountString(item.CurrentAmount),
                Inst.LcdHelper.AmountString(item.MinAmount),
                Math.Min(999, (percent * 100f))
                ), percent.ToString());
        }
        else
        {
            return new Line(String.Format(
                "{0} $ {1,6}",
                Inst.LcdHelper.GRAY_SQUARE,
                Inst.LcdHelper.AmountString(item.CurrentAmount)
                ), MaxLength(item.Name, 15));
        }
    }

    public string MaxLength(string input, int length)
    {
        if (input == null) return null;
        return input.Substring(0, Math.Min(length, input.Length));
    }

    public void CountInventory(IMyInventory inventory)
    {
        InventoryCache.Clear();
        inventory.GetItems(InventoryCache);
        foreach (MyInventoryItem item in InventoryCache)
        {
            string name = item.Type.ToString();
            if (!Items.ContainsKey(name))
            {
                addItem(name);
                Inst.Config.Save();
            }
            Items[name].CurrentAmount += (float)item.Amount;
        }
    }

    public void addItem(string id, string blueprint = null)
    {
        if (!Items.ContainsKey(id))
        {
            InventoryItem item = new InventoryItem(id, blueprint);
            foreach (ContainerGroup group in Groups.Values)
            {
                if (group.Matches(item.Category.ToLower(), true))
                {
                    item.TargetGroup = group;
                    break;
                }
            }
            if (item.TargetGroup == null) throw new Exception("UNKNOWN ITEM GROUP " + item.Category);
            this.Items.Add(id, item);
            if (!Inst.Config.Has("ItemsQuotas", id))
            {
                Inst.Config.Set("ItemsQuotas", id, 0);
            }
            if (blueprint == null && !id.Contains(ItemHelper.CATEGORY_ORE) && !id.Contains(ItemHelper.CATEGORY_INGOT) && !Inst.Config.Has("Blueprints", id))
            {
                Inst.Config.Set("Blueprints", id, "");
            }
        }
    }

    public void CheckIfSortContainer(IMyTerminalBlock block)
    {
        foreach (ContainerGroup group in Groups.Values)
        {
            if (group.Matches(block.DisplayNameText.ToLower())) group.AddBlock(block);
        }
        GetMatches(block.DisplayNameText, MatchCache);
        bool found = false;
        int priority = 0;
        foreach (SimpleItemMatch match in MatchCache)
        {
            foreach (InventoryItem item in Items.Values)
            {
                found = false;
                if (item.MatchName.Equals(match.Name))
                {
                    found = true;
                    priority = match.Priority > -1 ? match.Priority : -1;
                }
                else if (item.Name.ToLower().Equals(match.Name))
                {
                    found = true;
                    priority = match.Priority > -1 ? match.Priority : -2;

                }
                else if (item.TargetGroup != null && item.TargetGroup.Matches(match.Name, true))
                {
                    found = true;
                    priority = match.Priority > -1 ? match.Priority : -3;
                }
                else if (match.Name.Equals("itemsink"))
                {
                    found = true;
                    priority = match.Priority > -1 ? match.Priority : -4;
                }

                if (found && CanStore(block, item))
                {
                    if (match.Amount >= 0) priority = 0;
                    MySortContainer container = item.SortTargets.Find(con => con.Block == block);
                    if (container != null)
                    {
                        if (container.Priority < priority) container.Priority = priority;
                        if (container.MaxAmount < match.Amount) container.MaxAmount = match.Amount;
                    }
                    else
                    {
                        item.AddSortContainer(new MySortContainer
                        {
                            MaxAmount = match.Amount,
                            Priority = priority,
                            Block = block
                        });
                    }
                }
            }
        }
    }

    private bool CanStore(IMyTerminalBlock block, InventoryItem item)
    {
        if (block is IMyReactor) return item.MatchName.Equals((ItemHelper.CATEGORY_INGOT + "/Uranium").ToLower());
        if (block is IMyLargeInteriorTurret) return item.MatchName.Equals((ItemHelper.CATEGORY_AMMO + "/NATO_5p56x45mmMagazine").ToLower());
        if (block is IMyLargeMissileTurret || block is IMySmallMissileLauncher || block is IMySmallMissileLauncherReload) return item.MatchName.Equals((ItemHelper.CATEGORY_AMMO + "/Missile200mm").ToLower());
        if (block is IMyLargeGatlingTurret || block is IMySmallGatlingGun) return item.MatchName.Equals((ItemHelper.CATEGORY_AMMO + "/NATO_25x184mmMagazine").ToLower());
        return true;
    }

    private void AddVanillaItems()
    {
        foreach(List<ItemDefinition> list in IHelper.ITEMS.Values)
        {
            list.ForEach(item =>
            {
                addItem(item.Id, item.Blueprint);
            });
        }
    }

    public void GetMatches(string blockName, List<SimpleItemMatch> target)
    {
        target.Clear();
        char[] arr = blockName.ToLower().Where(c => (
                     char.IsLetterOrDigit(c) ||
                     char.IsWhiteSpace(c) ||
                     c == '-' ||
                     c == ':' ||
                     c == '_' ||
                     c == '/')
                     ).ToArray();

        string[] words = (new string(arr)).Split(' ');
        foreach (string word in words)
        {
            if (word.Length < 3) continue;

            string[] parts = word.Split(':');
            SimpleItemMatch match = new SimpleItemMatch
            {
                Name = parts[0]
            };
            for (int i = 1; i < parts.Length; i++)
            {
                string part = parts[i];
                if (part.StartsWith("p"))
                {
                    match.Priority = int.Parse(part.Substring(1));
                }
                else
                {
                    match.Amount = int.Parse(part);
                }
            }
            target.Add(match);
        }
    }
}

public class ContainerGroup
{
    public string Name;
    public string[] SearchTag;
    public List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
    public List<IMyInventory> Inventories = new List<IMyInventory>();
    public float MaxVolume = 0, CurrentVolume = 0, OldVolume = 0;
    public List<Line> DisplayCache = new List<Line>();

    public ContainerGroup(string name, params string[] searchTags)
    {
        Name = name;
        SearchTag = searchTags;
    }

    public bool Matches(string search, bool equals = false)
    {
        return equals ? SearchTag.Any(tag => search.Equals(tag)) : SearchTag.Any(tag => search.Contains(tag));
    }

    public void Reset()
    {
        Blocks.Clear();
        Inventories.Clear();
        MaxVolume = 0;
        CurrentVolume = 0;
    }

    public void AddBlock(IMyTerminalBlock block)
    {
        Blocks.Add(block);
        for (int i = 0; i < block.InventoryCount; i++)
        {
            Inventories.Add(block.GetInventory(i));
            MaxVolume += (float) block.GetInventory(i).MaxVolume;
        }
    }

}

public class SimpleItemMatch
{
    public string Name;
    public int Priority = -1;
    public int Amount = -1;

    public override string ToString()
    {
        return String.Format("{0}x {1} at {2}", Amount, Name, Priority);
    }
}

public class MySortContainer
{
    public IMyTerminalBlock Block;
    public int Priority = -1;
    public int MaxAmount = -1;

    public override string ToString()
    {
        return Block.DisplayNameText + "[P" + Priority + ", M" + MaxAmount + "]";
    }
}

public class InventoryItem : ItemDefinition
{
    public long MinAmount = 0;
    public float CurrentAmount = 0;
    public float OldAmount = 0;
    public string MatchName;
    public List<MySortContainer> SortTargets = new List<MySortContainer>();
    public ContainerGroup TargetGroup;

    public InventoryItem(string id, string blueprint = null) : base(id, blueprint)
    {
        Id = id;
        Blueprint = blueprint;
    }

    new public string Id
    {
        get { return ItemHelper.MYOB + Category + "/" + Name; }
        set
        {
            string[] splits = value.Replace(ItemHelper.MYOB, "").Split('/');
            Name = splits[1];
            Category = splits[0];
            MatchName = value.Replace(ItemHelper.MYOB, "").ToLower();
        }
    }

    public void AddSortContainer(MySortContainer container)
    {
        SortTargets.Add(container);
    }

    public void SortTargetBlocks()
    {
        SortTargets.Sort((o1, o2) =>
        {
            return -o1.Priority.CompareTo(o2.Priority);
        });
    }

    public void Reset()
    {
        SortTargets.Clear();
    }

    public bool In(IMyTerminalBlock block)
    {
        return SortTargets.Any(target => target.Block.EntityId == block.EntityId);
    }
}

public class LCDHelper
{
    public char RED_SQUARE { get; }
    public char GREEN_SQUARE { get; }
    public char YELLOW_SQUARE { get; }
    public char BLUE_SQUARE { get; }
    public char ORANGE_SQUARE { get; }
    public char BLACK_SQUARE { get; }
    public char GRAY_SQUARE { get; }

    public string MONO = "Monospace";


    public char CreateCustomColor(int r, int g, int b)
    {
        return (char)(0xE100 + (MathHelper.Clamp(r, 0, 7) << 6) + (MathHelper.Clamp(g, 0, 7) << 3) + MathHelper.Clamp(b, 0, 7));
    }

    public LCDHelper()
    {
        RED_SQUARE = CreateCustomColor(7, 0, 0);
        ORANGE_SQUARE = CreateCustomColor(7, 3, 0);
        YELLOW_SQUARE = CreateCustomColor(7, 7, 0);
        GREEN_SQUARE = CreateCustomColor(0, 7, 0);
        BLUE_SQUARE = CreateCustomColor(0, 0, 7);
        BLACK_SQUARE = CreateCustomColor(0, 0, 0);
        GRAY_SQUARE = CreateCustomColor(1, 1, 1);
    }

    public void InitPanel(IMyTextSurface surface, double fontSize = 0)
    {
        surface.ContentType = ContentType.TEXT_AND_IMAGE;
        surface.Font = MONO;
        if(fontSize > 0)
        {
            surface.FontSize = (float)fontSize;
        }
    }

    public void InitPanel(List<IMyTextPanel> panels)
    {
        panels.ForEach(p => InitPanel(p));
    }

    public string AmountString(double amount)
    {
        if (amount < 10) return amount.ToString("0.0");
        else if (amount < 1000) return amount.ToString("0");
        else if (amount < 10000) return (amount / 1000f).ToString("0.0") + " K";
        else if (amount < 1000000) return (amount / 1000f).ToString("0") + " K";
        else if (amount < 10000000) return (amount / 1000000f).ToString("0.0") + " M";
        else return (amount / 1000000f).ToString("0") + " M";
    }

    public string PowerString(double amount)
    {
        if (amount < 0.001) return (amount * 1000000f).ToString("0") + " W";
        else if (amount < 1) return (amount * 1000f).ToString("0") + " KW" ;
        else if (amount < 10f) return amount.ToString("0.0") + " MW";
        else if (amount < 1000f) return amount.ToString("0") + " MW";
        else if (amount < 10000f) return (amount / 1000f).ToString("0.0") + " GW";
        else if (amount < 1000000f) return (amount / 1000f).ToString("0") + " GW";
        else if (amount < 10000000f) return (amount / 1000000f).ToString("0.0") + " RW";
        else return (amount / 1000000f).ToString("0") + " RW";
    }

    public string GasString(double amount)
    {
        if (amount < 10) return amount.ToString("0.0") + " L";
        else if (amount < 1000) return amount.ToString("0") + " L";
        else if (amount < 100000) return (amount / 1000).ToString("0.0") + " KL";
        else if (amount < 10000000) return (amount / 1000).ToString("0") + " KL";
        else if (amount < 1000000000) return (amount / 1000000).ToString("0.0") + " ML";
        else if (amount < 100000000000) return (amount / 1000000).ToString("0") + " ML";
        else return (amount / 1000000000).ToString("0.0") + " GL";
    }

    public string HoursToTime(double hours)
    {
        if (double.IsNaN(hours) || double.IsInfinity(hours)) return "forever";
        return SecondsToTime((long)(hours * 3600));
    }

    public string SecondsToTime(long seconds)
    {
        int days = (int)(seconds / 86400);
        seconds -= (days * 86400);
        int hours = (int)(seconds / 3600);
        seconds -= (hours * 3600);
        int minutes = (int)(seconds / 60);
        seconds -= (minutes * 60);

        string result = String.Format("{0:00}:{1:00}", minutes, seconds);
        if (hours > 0) result = String.Format("{0:00}:{1}", hours, result);
        if (days > 365) result = String.Format("> 1y", days, result);
        else if (days > 9) result = String.Format("{0}d", days, result);
        else if (days > 0) result = String.Format("{0}d {1}", days, result);
        return result;
    }

    public int PanelCharWidth(IMyTextSurface surface)
    {
        return (int)(surface.SurfaceSize.X * (0.05f / surface.FontSize));
    }

    public string Filer(IMyTextSurface surface)
    {
        return new string('\u2500', PanelCharWidth(surface));
    }

    public char StatusLed(double percent)
    {
        percent = MathHelper.Clamp(percent, 0, 1);
        return CreateCustomColor((int)Math.Min(7, Math.Ceiling((1f - percent) / 5.71f * 100f)),(int)Math.Max(0, Math.Min(7, Math.Ceiling((percent - 0.25f) / 5.71f * 100f))), 0);
    }

    public char StatusLed(double current, double total)
    {
        return StatusLed(current / total);
    }

    public char StatusLed(bool status)
    {
        return status ? GREEN_SQUARE : RED_SQUARE;
    }

    public string Progressbar(int length, float percent)
    {
        if(length <= 0) return "";
        else if (length < 4) return new string(' ', length);
        percent = MathHelper.Clamp(percent, 0, 1);
        length -= 2;
        int lower = (int)Math.Floor(length * percent);
        int upper = (int)Math.Ceiling(length * (1 - percent));
        string result = "\u2502";
        if (lower > 0) result += new string('\u2588', lower);
        if (upper > 0) result += new string('-', upper);
        return result + "\u2502";
    }

    public string Progressbar(int length, float current, float total)
    {
        return this.Progressbar(length, current / total);
    }

    public string Format(IMyTextSurface surface, List<Line> lines)
    {
        string result = "";
        foreach (Line line in lines) result += Format(surface, line) + "\n";
        return result;
    }

    public string Format(IMyTextSurface surface, Line line)
    {
        if(line.Filler != null)
        {
            int length = Math.Max(1,PanelCharWidth(surface) - line.BaseText.Length + 1);
            if(line.BaseText.Contains("$"))
            {
                return String.Format(line.BaseText.Replace("$", "{0,-" + length + "}"), line.Filler);
            } else if(line.BaseText.Contains("§"))
            {
                return line.BaseText.Replace("§", Progressbar(length, float.Parse(line.Filler)));
            }
        }
        else if(line.BaseText.Equals("-"))
        {
            return Filer(surface);
        }
        return line.BaseText;
    }
}

public class Line
{
    public string BaseText;
    public string Filler;
    public int Blink = 0;

    public Line(string baseText, string filler)
    {
        BaseText = baseText;
        Filler = filler;
    }

    public Line(string baseText)
    {
        BaseText = baseText;
    }

    public Line(string baseText, string filler, int blink) : this(baseText, filler)
    {
        Blink = blink;
    }
}

public class LogHelper
{
    public int LogsToKeep;
    public List<LogMessage> Logs = new List<LogMessage>();

    public LogHelper(int logsToKeep)
    {
        this.LogsToKeep = logsToKeep;
    }

    public void LogUP(string message, double runtime, int instructions = 0)
    {
        if (Logs.Count >= LogsToKeep)
        {
            Logs.RemoveAt(0);
        }
        if (Logs.Count > 0)
        {
            Logs[Logs.Count - 1].time = runtime;
            Logs[Logs.Count - 1].instruction = instructions;
        }
        Logs.Add(new LogMessage(message));
    }

    public double AverageLogTime
    {
        get
        {
            double averageTime = 0;
            Logs.ForEach(t => averageTime += t.time);
            averageTime /= Math.Max(1, Logs.Count);
            return averageTime;
        }

    }

    public double AverageInstructions
    {
        get
        {
            double averageInstructions = 0;
            Logs.ForEach(t => averageInstructions += t.instruction);
            averageInstructions /= Math.Max(1, Logs.Count);
            return averageInstructions;
        }

    }
}

public class LogMessage
{
    public double time = 0;
    public string message;
    public int instruction = 0;

    public LogMessage(double time, string message, int instruction)
    {
        this.time = time;
        this.message = message;
        this.instruction = instruction;
    }

    public LogMessage(string message, double time)
    {
        this.message = message;
        this.time = time;
    }

    public LogMessage(string message)
    {
        this.message = message;
    }
}

public class MyBatteryList : AbstractStatusList<IMyBatteryBlock>
{
    protected override double GetCapacity(IMyBatteryBlock block)
    {
        return block.CurrentStoredPower;
    }

    protected override double GetMaxCapacity(IMyBatteryBlock block)
    {
        return block.MaxStoredPower;
    }

    protected override bool Matches(IMyTerminalBlock block)
    {
        return block is IMyBatteryBlock && block.BlockDefinition.TypeIdString.Contains("Bat");
    }
}

public class MyEngineList : AbstractStatusList<IMyPowerProducer>
{
    protected override double GetCapacity(IMyPowerProducer block)
    {
        return block.CurrentOutput;
    }

    protected override double GetMaxCapacity(IMyPowerProducer block)
    {
        return block.MaxOutput;
    }

    protected override bool Matches(IMyTerminalBlock block)
    {
        return block is IMyPowerProducer && block.BlockDefinition.TypeIdString.Contains("Hydro");
    }
}

public class MyGasGeneratorList : AbstractStatusList<IMyGasGenerator>
{
    protected override double GetCapacity(IMyGasGenerator block)
    {
        return block.GetInventory(0).ItemCount > 0 ? 1 : 0;
    }

    protected override double GetMaxCapacity(IMyGasGenerator block)
    {
        return 1;
    }
}

public class MyGasTankList : AbstractStatusList<IMyGasTank>
{
    public bool IsHydrogen = false;

    public MyGasTankList(bool isHydrogen)
    {
        IsHydrogen = isHydrogen;
    }

    protected override double GetCapacity(IMyGasTank block)
    {
        return block.FilledRatio * block.Capacity;
    }

    protected override double GetMaxCapacity(IMyGasTank block)
    {
        return block.Capacity;
    }

    protected override bool Matches(IMyTerminalBlock block)
    {
        return block is IMyGasTank && ((IsHydrogen && block.BlockDefinition.SubtypeId.Contains("Hydro")) || (!IsHydrogen && !block.BlockDefinition.SubtypeId.Contains("Hydro")));
    }
}

public class MyJumpDriveList : AbstractStatusList<IMyJumpDrive>
{
    protected override double GetCapacity(IMyJumpDrive block)
    {
        return block.CurrentStoredPower;
    }

    protected override double GetMaxCapacity(IMyJumpDrive block)
    {
        return block.MaxStoredPower;
    }

    protected override bool Matches(IMyTerminalBlock block)
    {
        return block is IMyJumpDrive;
    }
}

public class MyOxygenFarmList : AbstractStatusList<IMyOxygenFarm>
{
    public Dictionary<string, double> OutputTable;

    public MyOxygenFarmList(Dictionary<string, double> outputTable)
    {
        OutputTable = outputTable;
    }

    protected override double GetCapacity(IMyOxygenFarm block)
    {
        return block.GetOutput() * GetMaxCapacity(block);
    }

    protected override double GetMaxCapacity(IMyOxygenFarm block)
    {
        return OutputTable.GetValueOrDefault(block.BlockDefinition.SubtypeName, 0);
    }
}

public class MyReactorList : AbstractStatusList<IMyReactor>
{
    protected override double GetCapacity(IMyReactor block)
    {
        return block.CurrentOutput;
    }

    protected override double GetMaxCapacity(IMyReactor block)
    {
        return block.MaxOutput;
    }
}

public class MySolarPanelList : AbstractStatusList<IMySolarPanel>
{
    public Dictionary<string, double> OutputTable;

    public MySolarPanelList(Dictionary<string, double> outputTable)
    {
        OutputTable = outputTable;
    }

    protected override double GetCapacity(IMySolarPanel block)
    {
        return block.MaxOutput;
    }

    protected override double GetMaxCapacity(IMySolarPanel block)
    {
        return OutputTable.GetValueOrDefault(block.BlockDefinition.SubtypeName, 0);
    }
}

public class MyWindTurbineList : AbstractStatusList<IMyPowerProducer>
{
    public Dictionary<string, double> OutputTable;

    public MyWindTurbineList(Dictionary<string, double> outputTable)
    {
        OutputTable = outputTable;
    }

    protected override double GetCapacity(IMyPowerProducer block)
    {
        return block.MaxOutput;
    }

    protected override double GetMaxCapacity(IMyPowerProducer block)
    {
        return OutputTable.GetValueOrDefault(block.BlockDefinition.SubtypeName, 0);
    }

    protected override bool Matches(IMyTerminalBlock block)
    {
        return block is IMyPowerProducer && block.BlockDefinition.TypeIdString.Contains("Wind");
    }
}

public class PowerRoutine : AbstractBaseRoutine
{
    MyBatteryList BatteryList = new MyBatteryList();
    MyJumpDriveList JumpdriveList = new MyJumpDriveList();
    MyReactorList ReactorList = new MyReactorList();
    MyEngineList EngineList = new MyEngineList();
    MySolarPanelList SolarList = new MySolarPanelList(new Dictionary<string, double>());
    MyWindTurbineList TurbineList = new MyWindTurbineList(new Dictionary<string, double>());

    List<IMyPowerProducer> ReactorEmptyList = new List<IMyPowerProducer>();
    int EnginesActive = 0;
    int ReactorsActive = 0;

    double TurnReactorsOn;
    double TurnEnginesOn;
    bool EmptyReactorOnHUD;
    int EmptyReactorListLength;

    public PowerRoutine(InstanceHolder instance) : base("PWR", instance)
    {
        AddDefaultConfig("TurnReactorsOn", "30", "If the fillpercentage of all baterries is below this point turn on reactors");
        AddDefaultConfig("TurnEnginesOn", "50", "If the fillpercentage of all baterries is below this point turn on engines");
        AddDefaultConfig("EmptyReactorOnHUD", "false", "Show empty reactors on your HUD");
        AddDefaultConfig("EmptyReactorListLength", "3", "How many empty reactors should be visible on the panel");
        AddDefaultConfig("TimerNameReactorsOnline", "ReactorsOnline", "Timers, that have this tag in their name will be triggered when reactors are turned on");
        AddDefaultConfig("TimerNameReactorsOffline", "ReactorsOffline", "Timers, that have this tag in their name will be triggered when reactors are turned off");
        AddDefaultConfig("TimerNameEnginesOnline", "EnginesOnline", "Timers, that have this tag in their name will be triggered when engines are turned on");
        AddDefaultConfig("TimerNameEnginesOffline", "EnginesOffline", "Timers, that have this tag in their name will be triggered when engines are turned off");

        AddScreen("power");
    }

    public override IEnumerator<string> Run()
    {
        ReactorEmptyList.Clear();
        AddDefaultPassives();
        SolarList.OutputTable.Clear();
        TurbineList.OutputTable.Clear();
        foreach (MyIniKey key in Inst.Config.getSectionKeys("PassiveProducers"))
        {
            SolarList.OutputTable.Add(key.Name, Inst.Config.GetDouble(key.Section, key.Name));
            TurbineList.OutputTable.Add(key.Name, Inst.Config.GetDouble(key.Section, key.Name));
        }
        TurnReactorsOn = Inst.Config.GetDouble(RoutineName, "TurnReactorsOn") / 100f;
        TurnEnginesOn = Inst.Config.GetDouble(RoutineName, "TurnEnginesOn") / 100f;
        EmptyReactorOnHUD = Inst.Config.GetBool(RoutineName, "EmptyReactorOnHUD");
        EmptyReactorListLength = Inst.Config.GetInt(RoutineName, "EmptyReactorListLength");

        yield return "Loaded Routine Config";

        BatteryList.Load(Inst.AllBlocks);
        yield return "Loaded " + BatteryList.Count + " Batteries";

        ReactorList.Load(Inst.AllBlocks);
        yield return "Loaded " + ReactorList.Count + " Reactors";

        EngineList.Load(Inst.AllBlocks);
        yield return "Loaded " + EngineList.Count + " Engines";

        SolarList.Load(Inst.AllBlocks);
        yield return "Loaded " + SolarList.Count + " Solars";

        TurbineList.Load(Inst.AllBlocks);
        yield return "Loaded " + TurbineList.Count + " Turbines";

        JumpdriveList.Load(Inst.AllBlocks);
        yield return "Loaded " + JumpdriveList.Count + " Jumpdrives";



        for (int Run = 0; ; Run++)
        {
            if (SolarList.Count > 0)
            {
                SolarList.Refresh();
                yield return "Checked " + SolarList.Count + " Solars";
            }

            if (TurbineList.Count > 0)
            {
                TurbineList.Refresh();
                yield return "Checked " + TurbineList.Count + " Turbines";
            }

            if (BatteryList.Count > 0)
            {
                BatteryList.Refresh();
                yield return "Checked " + BatteryList.Count + " Batteries";
            }

            if (JumpdriveList.Count > 0)
            {
                JumpdriveList.Refresh();
                yield return "Checked " + JumpdriveList.Count + " Jumpdrives";
            }

            if (EngineList.Count > 0)
            {
                EngineList.Refresh();
                yield return "Checked " + EngineList.Count + " Engines";

                if (BatteryList.CurrentCapacityPercentage < TurnEnginesOn)
                {
                    if (EnginesActive != 1)
                    {
                        Event("TimerNameEnginesOnline");
                        EngineList.ForEach(e => e.Enabled = true);
                        yield return "Turned Engines on";
                    }
                    EnginesActive = 1;
                }
                else
                {
                    if (EnginesActive != -1)
                    {
                        Event("TimerNameEnginesOffline");
                        EngineList.ForEach(e => e.Enabled = false);
                        yield return "Turned Engines off";
                    }
                    EnginesActive = -1;
                }
            }


            if (ReactorList.Count > 0)
            {
                ReactorList.Refresh();
                ReactorEmptyList.Clear();
                foreach (IMyReactor generator in ReactorList)
                {
                    if (generator.GetInventory(0).ItemCount == 0)
                    {
                        ReactorEmptyList.Add(generator);
                        if (EmptyReactorOnHUD) generator.ShowOnHUD = true;
                    }
                    else if (EmptyReactorOnHUD) generator.ShowOnHUD = false;
                }
                yield return "Checked " + ReactorList.Count + " Reactors";

                if (BatteryList.CurrentCapacityPercentage < TurnReactorsOn)
                {
                    if (ReactorsActive != 1)
                    {
                        Event("TimerNameReactorsOnline");
                        ReactorList.ForEach(r => r.Enabled = true);
                        yield return "Turned Reactors on";
                    }
                    ReactorsActive = 1;
                }
                else
                {
                    if (ReactorsActive != -1)
                    {
                        Event("TimerNameReactorsOffline");
                        ReactorList.ForEach(r => r.Enabled = false);
                        yield return "Turned Reactors off";
                    }
                    ReactorsActive = -1;
                }
            }

            EndCycle();
            PrintPowerScreen();
            yield return "Power Cycle complete";
        }
    }

    private void PrintPowerScreen()
    {
        ResetScreen("power");
        AddToScreen("power", new Line("$ " + Inst.LcdHelper.SecondsToTime(CycleTime), "Power"), new Line("-"));
        if (BatteryList.Count > 0)
        {
            double mwh = Math.Abs(BatteryList.CapacityChange) / CycleTime * 3600;
            double hours = (BatteryList.CurrentCapacity > BatteryList.PastCapacity ? (BatteryList.MaxCapacity - BatteryList.CurrentCapacity) : BatteryList.CurrentCapacity) / mwh;

            AddToScreen("power", new Line(String.Format(
                Inst.LcdHelper.StatusLed(BatteryList.CurrentCapacityPercentage * 1.1) + " {0} $ {1,9}/h",
                BatteryList.Count,
                (BatteryList.CapacityChange >= 0 ? '+' : '-') + " " + Inst.LcdHelper.PowerString(mwh)
                ), "Batteries", 2));
            AddToScreen("power", new Line(String.Format(
                Inst.LcdHelper.StatusLed(BatteryList.CurrentCapacityPercentage * 1.1) + " {0} $ {1} {2}",
                BatteryList.Count,
                (BatteryList.CapacityChange >= 0 ? '+' : '-'),
                Inst.LcdHelper.HoursToTime(hours)
                ), "Batteries", 1));

            AddToScreen("power", new Line(String.Format(
                Inst.LcdHelper.BLACK_SQUARE + " {0,8} / {1,8} § {2,3:0}%",
                Inst.LcdHelper.PowerString(BatteryList.CurrentCapacity),
                Inst.LcdHelper.PowerString(BatteryList.MaxCapacity),
                BatteryList.CurrentCapacityPercentage * 100
                ), (BatteryList.CurrentCapacityPercentage).ToString()));
        }
        if (SolarList.Count > 0)
        {
            AddProducerLines("Solars", SolarList, Inst.LcdHelper.GREEN_SQUARE);
        }
        if (TurbineList.Count > 0)
        {
            AddProducerLines("Turbines", TurbineList, Inst.LcdHelper.GREEN_SQUARE);
        }
        if (EngineList.Count > 0)
        {
            AddProducerLines("Engines", EngineList, (EnginesActive == 1 ? Inst.LcdHelper.YELLOW_SQUARE : Inst.LcdHelper.GREEN_SQUARE));
        }
        if (ReactorList.Count > 0)
        {
            AddProducerLines("Reactors", ReactorList, (ReactorsActive == 1 ? Inst.LcdHelper.YELLOW_SQUARE : Inst.LcdHelper.GREEN_SQUARE));

            for (int i = 0; i < Math.Min(EmptyReactorListLength, ReactorEmptyList.Count); i++)
            {
                AddToScreen("power", new Line(Inst.LcdHelper.BLACK_SQUARE + " - " + ReactorEmptyList[i].DisplayNameText + " empty"));
            }
            if (ReactorEmptyList.Count > EmptyReactorListLength)
            {
                AddToScreen("power", new Line(Inst.LcdHelper.BLACK_SQUARE + " - " + (ReactorEmptyList.Count - EmptyReactorListLength) + " others empty"));
            }
        }

        if (JumpdriveList.Count > 0)
        {
            double mwh = Math.Abs(JumpdriveList.CapacityChange) / CycleTime * 3600;
            double hours = (JumpdriveList.CurrentCapacity > JumpdriveList.PastCapacity ? (JumpdriveList.MaxCapacity - JumpdriveList.CurrentCapacity) : JumpdriveList.CurrentCapacity) / mwh;
            AddToScreen("power", new Line(""));
            AddToScreen("power", new Line(String.Format(
                Inst.LcdHelper.StatusLed(JumpdriveList.CurrentCapacityPercentage * 1.1) + " {0} $ {1,9}/h",
                JumpdriveList.Count,
                (JumpdriveList.CapacityChange >= 0 ? '+' : '-') + " " + Inst.LcdHelper.PowerString(mwh)
                ), "Jumpdrives", 2));
            AddToScreen("power", new Line(String.Format(
                Inst.LcdHelper.StatusLed(JumpdriveList.CurrentCapacityPercentage * 1.1) + " {0} $ {1} {2}",
                JumpdriveList.Count,
                (JumpdriveList.CapacityChange >= 0 ? '+' : '-'),
                Inst.LcdHelper.HoursToTime(hours)
                ), "Jumpdrives", 1));

            AddToScreen("power", new Line(String.Format(
                Inst.LcdHelper.BLACK_SQUARE + " {0,8} / {1,8} § {2,3:0}%",
                Inst.LcdHelper.PowerString(JumpdriveList.CurrentCapacity),
                Inst.LcdHelper.PowerString(JumpdriveList.MaxCapacity),
                JumpdriveList.CurrentCapacityPercentage * 100
                ), (JumpdriveList.CurrentCapacityPercentage).ToString()));
        }
    }

    private void AddProducerLines<T>(string name, AbstractStatusList<T> source, char icon)
    {
        AddToScreen("power", new Line(String.Format(
            icon + " {0} $",
            source.Count
            ), name));
        AddToScreen("power", new Line(String.Format(
            Inst.LcdHelper.BLACK_SQUARE + " {0,8} / {1,8} § {2,3:0}%",
            Inst.LcdHelper.PowerString(source.CurrentCapacity),
            Inst.LcdHelper.PowerString(source.MaxCapacity),
            source.CurrentCapacityPercentage * 100
            ), (source.CurrentCapacityPercentage).ToString()));
    }

    private void AddDefaultPassives()
    {
        SetDefaultIfNeeded("PassiveProducers", "LargeBlockSolarPanel", "0.12");
        SetDefaultIfNeeded("PassiveProducers", "LargeBlockWindTurbine", "0.4");
        SetDefaultIfNeeded("PassiveProducers", "SmallBlockSolarPanel", "0.04");
        Inst.Config.Save();
    }
}

public class ScreenRoutine : AbstractBaseRoutine
{
    bool Blink = false;
    private List<PrintBuffer> Buffer = new List<PrintBuffer>();
    List<IMyTextPanel> Panels = new List<IMyTextPanel>();

    double PreferedFrontSize;

    public ScreenRoutine(InstanceHolder instance) : base("DSP", instance)
    {
        AddDefaultConfig("PreferedFrontSize", "0.5", "Update every Panel, that displays text to this font size. Set to 0 to disable.");
    }

    public override IEnumerator<string> Run()
    {
        Panels.Clear();

        PreferedFrontSize = Inst.Config.GetDouble(RoutineName, "PreferedFrontSize");
        yield return "Loaded Routine Config";

        Panels = Inst.AllBlocks.FindAll(b => b is IMyTextPanel && Inst.ContainsAny(b.CustomName, Inst.Screens.Keys.ToArray())).Cast<IMyTextPanel>().ToList();
        yield return "Loaded "+Panels.Count+" Panels";

        Panels.ForEach(p => Inst.LcdHelper.InitPanel(p, PreferedFrontSize));
        yield return "Initialized Panels";

        if (Panels.Count > 0)
        {
            for (int Run = 0; ; Run++)
            {
                StartCycle();
                Blink = !Blink;
                foreach (IMyTextPanel panel in Panels)
                {
                    Buffer.Clear();

                    foreach (string key in Inst.Screens.Keys)
                    {
                        if (panel.CustomName.Contains(key))
                        {
                            Buffer.Add(new PrintBuffer(WriteScreen(panel, Inst.Screens[key]), panel.CustomName.IndexOf(key)));
                        }
                    }
                    Buffer.Sort((pb1, pb2) => pb1.Index.CompareTo(pb2.Index));
                    string text = "";
                    Buffer.ForEach(pb => text += pb.Text + "\n");
                    panel.WriteText(text);
                    yield return "Updated " + panel.DisplayNameText;
                }
                if (Inst.ReloadScreen)
                {
                    Inst.ReloadScreen = false;
                    BlockListUpdated();
                }
                EndCycle();
            }
        } else
        {
            yield return "No Panels found";
        }
    }

    public string WriteScreen(IMyTextSurface surface, List<Line> lines)
    {
        string text = "";
        lines.ForEach(line => {
            if(line.Blink == 0 || (line.Blink == 1 && Blink) || (line.Blink == 2 && !Blink)) text += Inst.LcdHelper.Format(surface, line) + "\n";
        });
        return text;
    }
}

public class PrintBuffer
{
    public string Text;
    public int Index;

    public PrintBuffer(string text, int index)
    {
        Text = text;
        Index = index;
    }
}

public class SecurityRoutine : AbstractBaseRoutine
{

    bool triggerAlarm = false;
    bool triggerDamage = false;
    int TurretsActiveChange;
    List<IMyLargeTurretBase> Turrets = new List<IMyLargeTurretBase>();
    List<IMyLargeTurretBase> TurretsWithTarget = new List<IMyLargeTurretBase>();
    List<IMyLargeTurretBase> TurretsOutOfAmmo = new List<IMyLargeTurretBase>();
    List<IMyTerminalBlock> DamagedBlocks = new List<IMyTerminalBlock>();

    List<string> Targets = new List<string>();

    int ScanForDamage;
    bool EmptyTurretsOnHUD;
    int EmptyTurretListLength;
    bool DamagedBlocksOnHUD;
    int DamagedBlockListLength;

    public SecurityRoutine(InstanceHolder instance) : base("SEC", instance)
    {
        AddDefaultConfig("ScanForDamage", "5", "How often should be scanned for damaged blocks. Lower = faster, but slower enemy detection");
        AddDefaultConfig("EmptyTurretsOnHUD", "true", "Show turrets on HUD that are out of ammo");
        AddDefaultConfig("EmptyTurretListLength", "3", "How many empty turrets should be listed on the security panel");
        AddDefaultConfig("DamagedBlocksOnHUD", "true", "Show damaged blocks on HUD");
        AddDefaultConfig("DamagedBlockListLength", "5", "How many damaged blocks should be listed on the security panel");

        AddDefaultConfig("TimerNameAttackStarted", "AttackStarted", "Timers with this tag in the name will be triggered, when turrets detect an enemy");
        AddDefaultConfig("TimerNameAttackStopped", "AttackStopped", "Timers with this tag in the name will be triggered, when turrets detect no more enemies");
        AddDefaultConfig("TimerNameDamageDetected", "DamageDetected", "Timers with this tag in the name will be triggered, when damage is detected");
        AddDefaultConfig("TimerNameNoDamageDetected", "NoMoreDamageDetected", "Timers with this tag in the name will be triggered, when no more damage is detected");

        AddScreen("security");
        AddScreen("defenselog");
    }


    public override IEnumerator<string> Run()
    {
        TurretsWithTarget.Clear();
        TurretsOutOfAmmo.Clear();
        DamagedBlocks.Clear();
        Targets.Clear();

        ScanForDamage = Inst.Config.GetInt(RoutineName, "ScanForDamage");
        EmptyTurretsOnHUD = Inst.Config.GetBool(RoutineName, "EmptyTurretsOnHUD");
        EmptyTurretListLength = Inst.Config.GetInt(RoutineName, "EmptyTurretListLength");
        DamagedBlocksOnHUD = Inst.Config.GetBool(RoutineName, "DamagedBlocksOnHUD");
        DamagedBlockListLength = Inst.Config.GetInt(RoutineName, "DamagedBlockListLength");
        yield return "Loaded Routine Config";

        Turrets = Inst.AllBlocks.FindAll(b => b is IMyLargeTurretBase).Cast<IMyLargeTurretBase>().ToList();
        yield return "Loaded " + Turrets.Count + " Turrets";


        for (int Run = 0; ; Run++)
        {
            if (Run % ScanForDamage == 0)
            {
                DamagedBlocks.Clear();
                ResetStep();
                foreach (IMyTerminalBlock block in Inst.AllBlocks)
                {
                    if (!block.IsFunctional)
                    {
                        DamagedBlocks.Add(block);
                        if (DamagedBlocksOnHUD) block.ShowOnHUD = true;
                    }
                    else if (DamagedBlocksOnHUD) block.ShowOnHUD = false;

                    if (!CanContinue(.5f))
                    {
                        yield return "Scanning for Damage...";
                    }
                }
                yield return "Found " + DamagedBlocks.Count + " damaged Blocks";

                if (DamagedBlocks.Count == 0 && triggerDamage)
                {
                    Event("TimerNameNoDamageDetected");
                    triggerDamage = false;
                    yield return "Acessed Damage";
                }
                else if (DamagedBlocks.Count > 0 && !triggerDamage)
                {
                    Event("TimerNameDamageDetected");
                    triggerDamage = true;
                    yield return "Acessed Damage";
                }
            }

            if (Turrets.Count > 0)
            {
                TurretsActiveChange = TurretsWithTarget.Count;
                TurretsWithTarget.Clear();
                TurretsOutOfAmmo.Clear();

                ResetStep();
                foreach (IMyLargeTurretBase turret in Turrets)
                {
                    if (turret.HasTarget)
                    {
                        if(!Targets.Contains(turret.GetTargetedEntity().Name))
                        {
                            Targets.Add(turret.GetTargetedEntity().Name);
                        }
                        TurretsWithTarget.Add(turret);
                    }
                    if (turret.GetInventory(0).ItemCount == 0)
                    {
                        TurretsOutOfAmmo.Add(turret);
                        if (EmptyTurretsOnHUD) turret.ShowOnHUD = true;
                    }
                    else if (EmptyTurretsOnHUD) turret.ShowOnHUD = false;
                }
                TurretsActiveChange = TurretsWithTarget.Count - TurretsActiveChange;
                yield return "Scanned Turrets";

                if (TurretsWithTarget.Count > 0 && !triggerAlarm)
                {
                    triggerAlarm = true;
                    Event("TimerNameAttackStarted");
                    yield return "Accessed Thread";
                }
                else if (TurretsWithTarget.Count == 0 && triggerAlarm)
                {
                    AddToScreenFront("defenselog", new Line(string.Format("[{0}] {1}", DateTime.Now.ToString("dd.MM HH:mm"), String.Join(", ", Targets))));
                    Targets.Clear();
                    triggerAlarm = false;
                    Event("TimerNameAttackStopped");
                    yield return "Accessed Thread";
                }
            }

            EndCycle();
            PrintScreen();
            yield return "Security Cycle Complete";
        }
    }

    private void AddDefenseProtocol(bool append = false)
    {

    }

    private void PrintScreen()
    {
        char turretSign = Inst.LcdHelper.GREEN_SQUARE;
        if (TurretsOutOfAmmo.Count > 0) turretSign = Inst.LcdHelper.ORANGE_SQUARE;
        if (TurretsWithTarget.Count > 0) turretSign = Inst.LcdHelper.RED_SQUARE;
        ResetScreen("security");
        AddToScreen("security", new Line("$ " + Inst.LcdHelper.SecondsToTime(CycleTime), "Security"), new Line("-"));
        AddToScreen("security", new Line(String.Format(turretSign + " {0,2} Turrets", Turrets.Count)));
        if (Turrets.Count > 0)
        {
            AddToScreen("security", new Line(String.Format(
                Inst.LcdHelper.BLACK_SQUARE + " - Engaged   {0,4} / {1,4} § {2,3:0}%",
                TurretsWithTarget.Count,
                Turrets.Count,
                (float)TurretsWithTarget.Count / (float)Turrets.Count * 100f
                ), ((float)TurretsWithTarget.Count / (float)Turrets.Count).ToString()));
            AddToScreen("security", new Line(String.Format(
                Inst.LcdHelper.BLACK_SQUARE + " - Armed     {0,4} / {1,4} § {2,3:0}%",
                (Turrets.Count - TurretsOutOfAmmo.Count),
                Turrets.Count,
                (float)(Turrets.Count - TurretsOutOfAmmo.Count) / (float)Turrets.Count * 100f
                ), ((float)(Turrets.Count - TurretsOutOfAmmo.Count) / (float)Turrets.Count).ToString()));


            for (int i = 0; i < Math.Min(EmptyTurretListLength, TurretsOutOfAmmo.Count); i++)
            {
                AddToScreen("security", new Line(Inst.LcdHelper.BLACK_SQUARE + " - " + TurretsOutOfAmmo[i].DisplayNameText + " empty"));
            }
            if (TurretsOutOfAmmo.Count > EmptyTurretListLength)
            {
                AddToScreen("security", new Line(Inst.LcdHelper.BLACK_SQUARE + " - " + (TurretsOutOfAmmo.Count - EmptyTurretListLength) + " others empty"));
            }
        }

        AddToScreen("security", new Line(""), new Line(String.Format(
            Inst.LcdHelper.StatusLed(Inst.AllBlocks.Count - DamagedBlocks.Count, Inst.AllBlocks.Count) + " Integrity {0,4} / {1,4} § {2,3:0}%",
            Inst.AllBlocks.Count - DamagedBlocks.Count,
            Inst.AllBlocks.Count,
            (float)(Inst.AllBlocks.Count - DamagedBlocks.Count) / (float)Inst.AllBlocks.Count * 100f
            ), ((float)(Inst.AllBlocks.Count - DamagedBlocks.Count) / (float)Inst.AllBlocks.Count).ToString()));

        for (int i = 0; i < Math.Min(DamagedBlockListLength, DamagedBlocks.Count); i++)
        {
            AddToScreen("security", new Line(Inst.LcdHelper.BLACK_SQUARE + " - " + DamagedBlocks[i].DisplayNameText));
        }
        if (DamagedBlocks.Count > DamagedBlockListLength)
        {
            AddToScreen("security", new Line(Inst.LcdHelper.BLACK_SQUARE + " - " + (DamagedBlocks.Count - DamagedBlockListLength) + " others"));
        }
    }
}