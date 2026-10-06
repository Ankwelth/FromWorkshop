/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

const int defaultPriority = 0;

readonly Dictionary<string, string> defaultFilters = new Dictionary<string, string>()
{
    {"Advanced Cargo Container", ""},
    {"Artillery Turret", "p100 q10"},
    {"Assault Cannon Turret", "p100 q10"},
    {"Assembler", ""},
    {"Basic Assembler", ""},
    {"Basic Refinery", "p100 q1000"},
    {"Cockpit", ""},
    {"Cryo Chamber", ""},
    {"Connector", ""},
    {"Drill", ""},
    {"Fighter Cockpit", ""},
    {"Gatling Gun", "p100 q10"},
    {"Gatling Turret", "p100 q10"},
    {"Grinder", ""},
    {"Hydrogen Tank", ""},
    {"Interior Turret", "p100 q10"},
    {"Large Cargo Container", ""},
    {"Large Reactor", "p100 q100"},
    {"Medium Cargo Container", ""},
    {"Missile Turret", "p100 q10"},
    {"O2/H2 Generator", "ice p100"},
    {"Oxygen Tank", ""},
    {"Refinery", "p100 q1000;p100 q10000"},
    {"Reloadable Rocket Launcher", "p100 q10"},
    {"Small Cargo Container", ""},
    {"Small Hydrogen Tank", ""},
    {"Small Reactor", "p100 q50"},
    {"Survival Kit", ""},
    {"Welder", ""},
};

const bool manageAssemblers = true;
const bool manageRefineries = true;
const bool manageGasGenerators = true;
const bool manageGasTanks = true;
const bool manageBatteryBlocks = true;
const bool manageReactors = true;
const bool manageShipConnectors = true;
const bool manageShipWelders = true;

const bool manageReactorsPower = true;
const float enableReactorsAtStoredPower = 0.4f;
const float disableReactorsAtStoredPower = 0.8f;
const float enableReactorsAtOutput = 0.9f;

const bool manageGasGeneratorsGas = true;
const double enableGasGeneratorsAtStoredGas = 0.6d;
const double disableGasGeneratorsAtStoredGas = 0.9d;

readonly bool logToEcho = true;
readonly bool logToSurface = false;
readonly bool debugMode = false;

// Valid update frequencies are Update1, Update10 and Update100. Lower is faster.
// Instruction count is a rough maximum per cycle. Higher means faster operation.
const UpdateFrequency manageUpdateFrequency = UpdateFrequency.Update100;
const int manageTargetInstructionCount = 1000;

const UpdateFrequency transferUpdateFrequency = UpdateFrequency.Update10;
const int transferTargetInstructionCount = 10000;

const UpdateFrequency scanUpdateFrequency = UpdateFrequency.Update100;
const int scanTargetInstructionCount = 1000;

const UpdateFrequency logUpdateFrequency = UpdateFrequency.Update10;

const string ignoreTag = "!";
const string configSectionKey = "os";
const string parseFilterPrefix = "(";
const string parseFilterSuffix = " os)";
const string parseFilterAll = "*";
const string parseFilterSeparators = "\n;";
const string parseFilterArgSeparators = " ";
const char parseFilterPriority = 'p';
const char parseFilterQuota = 'q';
const char parseFilterSubtract = '-';

const string version = "1.0.3";

public static Program Instance { get; private set; }

const string myName = "(os)";

public readonly char[] _parseFilterSeparators = parseFilterSeparators.ToCharArray();
public readonly char[] _parseFilterArgSeparators = parseFilterArgSeparators.ToCharArray();

public readonly MyIni ini = new MyIni();
public readonly ItemTargetComparer itemTargetComparer = new ItemTargetComparer();

public readonly List<IMyTerminalBlock> IMyGridTerminalSystem_GetBlocks_blocks = new List<IMyTerminalBlock>();
public readonly List<MyItemType> IMyInventory_GetAcceptedItems_itemTypes = new List<MyItemType>();
public readonly List<MyInventoryItem> IMyInventory_GetItems_items = new List<MyInventoryItem>();
public readonly Dictionary<MyItemType, MyFixedPoint> IMyInventory_SumItems_items = new Dictionary<MyItemType, MyFixedPoint>();

readonly UpdateType manageUpdateType = manageUpdateFrequency.ToUpdateType();
readonly UpdateType transferUpdateType = transferUpdateFrequency.ToUpdateType();
readonly UpdateType scanUpdateType = scanUpdateFrequency.ToUpdateType();
readonly UpdateType logUpdateType = logUpdateFrequency.ToUpdateType();

readonly string[] logAnim = new string[] {
    "....",
    ":...",
    "::..",
    ":::.",
    "::::",
    ".:::",
    "..::",
    "...:",
    "....",
    "...:",
    "..::",
    ".:::",
    "::::",
    ":::.",
    "::..",
    ":...",
};
int logAnimIndex = -1;

readonly State state;
readonly Worker manager;
readonly Worker transferrer;
readonly Worker scanner;

public Program()
{
    Instance = this;

    Runtime.UpdateFrequency = manageUpdateFrequency | transferUpdateFrequency | scanUpdateFrequency | logUpdateFrequency;

    if (!Me.CustomName.EndsWith(myName)) Me.CustomName += $" {myName}";

    if (logToSurface)
    {
        Me.GetSurface(0).Prepare();
        Me.GetSurface(1).Prepare(Alignment: TextAlignment.CENTER, FontSize: 4f, TextPadding: 34f);
    }

    state = new State();
    manager = new Worker(Runtime, Manage);
    transferrer = new Worker(Runtime, Transfer);
    scanner = new Worker(Runtime, Scan);

    // Load Storage (string)

    // https://github.com/malware-dev/MDK-SE/wiki/Handling-configuration-and-storage
    // Config in CustomData?
}

public void Save()
{
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means.
    //
    // This method is optional and can be removed if not
    // needed.

    // https://github.com/malware-dev/MDK-SE/wiki/The-Storage-String
    // Storage = string
}

public void Main(string argument, UpdateType updateSource)
{
    if ((updateSource & manageUpdateType) != 0) manager.Cycle(manageTargetInstructionCount);
    if ((updateSource & scanUpdateType) != 0) scanner.Cycle(scanTargetInstructionCount);
    if ((updateSource & transferUpdateType) != 0) transferrer.Cycle(transferTargetInstructionCount);
    if ((updateSource & logUpdateType) != 0) Log();
}

public void Log()
{
    if (!logToEcho && !logToSurface) return;

    var header = $"OmniScript v{version} [{logAnim[++logAnimIndex % logAnim.Length]}]";
    var content = $"Manager {manager.Log}Scanner {scanner.Log}Transferrer {transferrer.Log}";
    var frame = $"{header}\n\n{content}";
    if (logToEcho) Echo(frame);
    if (logToSurface)
    {
        Me.GetSurface(0).WriteText(content);
        Me.GetSurface(1).WriteText(header);
    }


    // Dump item info in CustomData
    //var ss = new List<string>();
    //foreach (var item in itemTypes.Values)
    //{
    //    ss.Add($"{item.Group}  {item.DisplayName}\n");
    //}
    //ss.Sort();
    //var sb = new StringBuilder();
    //foreach (var s in ss)
    //{
    //    sb.Append(s);
    //}
    //Me.CustomData = sb.ToString();
}

public string GetDefaultFilter(string definition)
{
    string filter;
    return defaultFilters.TryGetValue(definition, out filter) ? filter : "";
}

public class FilterException : Exception
{
    public FilterException(string filterString) : base($"Failed to parse filter '{filterString}'") { }
}

public class Filter
{
    readonly HashSet<MyItemType> types;
    readonly int priority;
    readonly MyFixedPoint quota;

    public Filter(HashSet<MyItemType> types, int priority, MyFixedPoint quota)
    {
        this.types = types;
        this.priority = priority;
        this.quota = quota;
    }

    public HashSet<MyItemType> Types => types;
    public int Priority => priority;
    public MyFixedPoint Quota => quota;
    public bool HasQuota => quota != MyFixedPoint.MaxValue;

    public static bool TryParse(List<MyItemType> from, string s, out Filter result)
    {
        var args = s.ToLower().Split(Instance._parseFilterArgSeparators, StringSplitOptions.RemoveEmptyEntries);

        int priority = 0, quota = 0;
        bool hasPriority = false, hasQuota = false, hasTypes = false;
        var types = new HashSet<MyItemType>();
        foreach (var arg in args)
        {
            if (arg[0] == parseFilterPriority && int.TryParse(arg.SubStr(1), out priority)) hasPriority = true;
            else if (arg[0] == parseFilterQuota && int.TryParse(arg.SubStr(1), out quota)) hasQuota = true;
            else
            {
                var add = arg[0] != parseFilterSubtract;
                if (!Instance.MatchItem(from, ref types, add ? arg : arg.SubStr(1), add))
                {
                    result = null;
                    return false;
                }
                hasTypes = true;
            }
        }
        if (!hasTypes && (hasPriority || hasQuota)) from.ForEach(type => types.Add(type));
        result = new Filter(types, hasPriority ? priority : defaultPriority, hasQuota ? quota : MyFixedPoint.MaxValue);
        return true;
    }
}

public class Filters : List<Filter>
{
    public Filters() : base() { }
    public Filters(IEnumerable<Filter> filter) : base(filter) { }

    public static Filters None = new Filters();

    /// <exception cref="FilterException"></exception>
    public static Filters Parse(IMyInventory inventory, string name, string data, string defaultFilter = "")
    {
        var filterStrings = new List<string>();

        MyIniParseResult result;
        string fromDataString;
        if (Instance.ini.TryParse(data, out result) && Instance.ini.Get(configSectionKey, "filter").TryGetString(out fromDataString)) filterStrings.AddArray(fromDataString.Split(Instance._parseFilterSeparators));

        string fromNameString;
        if (Util.StringSection(name, parseFilterPrefix, parseFilterSuffix, out fromNameString)) filterStrings.AddArray(fromNameString.Split(Instance._parseFilterSeparators));

        if (filterStrings.Count == 0 && defaultFilter.Length > 0) filterStrings.AddArray(defaultFilter.Split(Instance._parseFilterSeparators));

        var from = inventory.GetAcceptedItems();
        var filters = new Filters();
        foreach (string filterString in filterStrings)
        {
            Filter filter;
            if (Filter.TryParse(from, filterString, out filter)) filters.Add(filter);
            else throw new FilterException(filterString);
        }

        return filters;
    }
}

public class Item
{
    readonly string displayName, group, matchName, matchGroup;

    public Item(MyItemType it)
    {
        displayName = MakeDisplayName(it);
        group = MakeGroup(it);
        matchName = displayName.Replace(" ", "").ToLower();
        matchGroup = group.Replace(" ", "").ToLower();
    }

    public string DisplayName => displayName;
    public string Group => group;

    public bool Match(string s)
    {
        if (s == parseFilterAll || matchGroup == s) return true;

        var i = s.IndexOf('/');
        return i >= 0
            ? matchGroup.StartsWith(s.SubStr(0, i)) && matchName.StartsWith(s.SubStr(i + 1))
            : matchName.StartsWith(s);
    }

    private static string MakeDisplayName(MyItemType it)
    {
        switch (it.ToString())
        {
            case "MyObjectBuilder_Ingot/Stone":
                return "Gravel";
        }

        var s = it.SubtypeId;
        var l = s.EndsWith("Item") ? s.Length - 4 : s.Length;
        var sb = new StringBuilder(s[0].ToString());
        for (int i = 1; i < l; ++i)
        {
            if (s[i] == 'G' && i + 8 < l && s.Substring(i + 1, 7) == "un_Mag_")
            {
                sb.Append(" Magazine");
                break;
            }
            if (
                (char.IsNumber(s[i]) && char.IsLetter(s[i - 1]) && (i < 2 || !char.IsNumber(s[i - 2]))) ||
                (char.IsUpper(s[i]) && char.IsLower(s[i - 1]))
            ) sb.Append(' ');
            sb.Append(s[i]);
        }
        return sb.ToString();
    }

    private static string MakeGroup(MyItemType it)
    {
        switch (it.TypeId)
        {
            case "MyObjectBuilder_ConsumableItem":
                return "Consumable";
            case "MyObjectBuilder_GasContainerObject":
            case "MyObjectBuilder_OxygenContainerObject":
                return "Bottle";
            case "MyObjectBuilder_Datapad":
            case "MyObjectBuilder_Package":
            case "MyObjectBuilder_PhysicalObject":
                return "Other";
        }

        var info = it.GetItemInfo();
        return
            info.IsAmmo ? "Ammo" :
            info.IsComponent ? "Component" :
            info.IsIngot ? "Ingot" :
            info.IsOre ? "Ore" :
            info.IsTool ? "Tool" :
            it.TypeId.SubStr(it.TypeId.LastIndexOf("_") + 1);
    }
}

public readonly Dictionary<MyItemType, Item> itemTypes = new Dictionary<MyItemType, Item>();

public Item GetItem(MyItemType it)
{
    Item item;
    if (!itemTypes.TryGetValue(it, out item)) itemTypes.Add(it, item = new Item(it));
    return item;
}

public bool MatchItem(List<MyItemType> from, ref HashSet<MyItemType> types, string s, bool add)
{
    var startCount = types.Count;
    foreach (var it in from)
    {
        if (!GetItem(it).Match(s)) continue;
        if (add) types.Add(it);
        else types.Remove(it);
    }
    return startCount != types.Count;
}

public class ItemTarget
{
    readonly IManagedInventory inventory;
    readonly Filter filter;

    public ItemTarget(IManagedInventory inventory, Filter filter)
    {
        this.inventory = inventory;
        this.filter = filter;
    }

    public IManagedInventory Inventory => inventory;
    public Filter Filter => filter;
    public int Priority => filter.Priority;
    public MyFixedPoint Quota => filter.Quota;
    public bool HasQuota => filter.HasQuota;
}

public class ItemTargetComparer : IComparer<ItemTarget>
{
    public int Compare(ItemTarget a, ItemTarget b) =>
        b.Priority != a.Priority ? b.Priority - a.Priority :
        b.HasQuota != a.HasQuota ? b.HasQuota ? 1 : -1 :
        b.HasQuota && a.HasQuota && b.Quota != a.Quota ? b.Quota < a.Quota ? 1 : -1 :
        b.Inventory.Inventory.MaxVolume != a.Inventory.Inventory.MaxVolume ? b.Inventory.Inventory.MaxVolume < a.Inventory.Inventory.MaxVolume ? 1 : -1 :
        b.Inventory.Block.Block.EntityId != a.Inventory.Block.Block.EntityId ? b.Inventory.Block.Block.EntityId > a.Inventory.Block.Block.EntityId ? 1 : -1 :
        0;
}

public interface IManagedBlock
{
    IMyTerminalBlock Block { get; }
    string Name { get; }
    string Data { get; }
    string Error { get; }
    bool HasError { get; }
    bool Closed { get; }
    bool Enabled { get; set; }
    bool Changed { get; }
}

public abstract class ManagedBlock<TBlock> : IManagedBlock where TBlock : IMyTerminalBlock
{
    protected readonly TBlock block;
    protected readonly string name;
    protected readonly string data;
    protected string error = "";

    protected ManagedBlock(TBlock block)
    {
        this.block = block;
        name = block.CustomName;
        data = block.CustomData;
    }

    public IMyTerminalBlock Block => block;
    public string Name => name;
    public string Data => data;
    public string Error => error;
    public bool HasError => error.Length > 0;
    public bool Closed => block.Closed;
    public bool Enabled
    {
        get { return !(block is IMyFunctionalBlock) || ((IMyFunctionalBlock)block).Enabled; }
        set { if (block is IMyFunctionalBlock) { ((IMyFunctionalBlock)block).Enabled = value; } }
    }
    public virtual bool Changed { get { return !block.CustomName.Equals(name) || !block.CustomData.Equals(data); } }
}

public class WatchedBlock : ManagedBlock<IMyTerminalBlock>
{
    public WatchedBlock(IMyTerminalBlock block) : base(block) { }
}

public class ManagedBlocks : Dictionary<long, IManagedBlock>
{
    public ManagedBlocks() : base() { }

    public ManagedAssembler Assembler(long id) { IManagedBlock result; TryGetValue(id, out result); return result as ManagedAssembler; }
    public ManagedRefinery Refinery(long id) { IManagedBlock result; TryGetValue(id, out result); return result as ManagedRefinery; }
    public ManagedGasGenerator GasGenerator(long id) { IManagedBlock result; TryGetValue(id, out result); return result as ManagedGasGenerator; }
    public ManagedReactor Reactor(long id) { IManagedBlock result; TryGetValue(id, out result); return result as ManagedReactor; }
    public ManagedGasTank GasTank(long id) { IManagedBlock result; TryGetValue(id, out result); return result as ManagedGasTank; }
    public ManagedBatteryBlock BatteryBlock(long id) { IManagedBlock result; TryGetValue(id, out result); return result as ManagedBatteryBlock; }
}

public interface IManagedInventory
{
    IManagedBlock Block { get; }
    IMyInventory Inventory { get; }
    Filters Filters { get; }
    bool ApplyFilters { get; }
    bool Ready { get; }
}

public class ManagedInventory<TBlock> : IManagedInventory where TBlock : IManagedBlock
{
    protected readonly TBlock block;
    protected readonly IMyInventory inventory;
    protected readonly Filters filters;

    public ManagedInventory(TBlock block, IMyInventory inventory, Filters filters)
    {
        this.block = block;
        this.inventory = inventory;
        this.filters = filters;
    }

    public IManagedBlock Block => block;
    public IMyInventory Inventory => inventory;
    public Filters Filters => filters;
    public virtual bool ApplyFilters => true;
    public virtual bool Ready => block.Block.IsFunctional;
}
public class ManagedInventory : ManagedInventory<IManagedBlock>
{
    public ManagedInventory(IManagedBlock block, IMyInventory inventory, Filters filters) : base(block, inventory, filters) { }
}

public abstract class ManagedFilteredBlock<TBlock> : ManagedBlock<TBlock> where TBlock : IMyTerminalBlock
{
    readonly Filters filters;

    protected ManagedFilteredBlock(TBlock block) : base(block)
    {
        try
        {
            filters = Filters.Parse(block.GetInventory(), name, data, Instance.GetDefaultFilter(block.DefinitionDisplayNameText));
        }
        catch (FilterException e)
        {
            filters = Filters.None;
            error = e.Message;
        }
    }

    public Filters Filters => filters;
}

public abstract class ManagedInventoryBlock<TBlock> : ManagedFilteredBlock<TBlock> where TBlock : IMyTerminalBlock
{
    protected IManagedInventory inventory;

    protected ManagedInventoryBlock(TBlock block) : base(block) { }

    public IManagedInventory Inventory
    {
        get { return inventory; }
        protected set { inventory = value; }
    }
}

public class ManagedInventoryBlock : ManagedInventoryBlock<IMyTerminalBlock>
{
    public ManagedInventoryBlock(IMyTerminalBlock block) : base(block)
    {
        Inventory = new ManagedInventory(this, block.GetInventory(), Filters);
    }
}

public abstract class ManagedProductionBlock<TBlock> : ManagedFilteredBlock<TBlock> where TBlock : IMyProductionBlock
{
    protected IManagedInventory input;
    protected IManagedInventory output;

    protected ManagedProductionBlock(TBlock block) : base(block) { }

    public IManagedInventory Input
    {
        get { return input; }
        protected set { input = value; }
    }
    public IManagedInventory Output
    {
        get { return output; }
        protected set { output = value; }
    }
}

public class ManagedProductionBlock : ManagedProductionBlock<IMyProductionBlock>
{
    public ManagedProductionBlock(IMyProductionBlock block) : base(block)
    {
        Input = new ManagedInventory(this, block.InputInventory, Filters);
        Output = new ManagedInventory(this, block.OutputInventory, Filters.None);
    }
}

public class ManagedAssemblerInputInventory : ManagedInventory<ManagedAssembler>
{
    public ManagedAssemblerInputInventory(ManagedAssembler block, Filters filters) : base(block, ((IMyAssembler)block.Block).InputInventory, filters) { }

    public override bool Ready => block.IsQueueEmpty && base.Ready;
}
public class ManagedAssembler : ManagedProductionBlock<IMyAssembler>
{
    public ManagedAssembler(IMyAssembler block) : base(block)
    {
        block.UseConveyorSystem = true;
        Input = new ManagedAssemblerInputInventory(this, Filters);
        Output = new ManagedInventory(this, block.OutputInventory, Filters.None);
    }

    public bool DefinedMaster => name.ContainsIgnoreCase("master");
    public bool IsQueueEmpty => block.IsQueueEmpty;
    public void ClearQueue() => block.ClearQueue();
    public bool CooperativeMode
    {
        get { return block.CooperativeMode; }
        set { block.CooperativeMode = value; }
    }
}

public class ManagedRefineryInputInventory : ManagedInventory<ManagedRefinery>
{
    public ManagedRefineryInputInventory(ManagedRefinery block, Filters filters) : base(block, ((IMyRefinery)block.Block).InputInventory, filters) { }

    public override bool ApplyFilters => block.Enabled && base.ApplyFilters;
}
public class ManagedRefinery : ManagedProductionBlock<IMyRefinery>
{
    public ManagedRefinery(IMyRefinery block) : base(block)
    {
        block.UseConveyorSystem = false;
        Input = new ManagedRefineryInputInventory(this, Filters);
        Output = new ManagedInventory(this, block.OutputInventory, Filters.None);
    }
}

public class ManagedGasGenerator : ManagedInventoryBlock<IMyGasGenerator>
{
    public ManagedGasGenerator(IMyGasGenerator block) : base(block)
    {
        block.UseConveyorSystem = false;
        block.AutoRefill = true;
        Inventory = new ManagedInventory(this, block.GetInventory(), Filters);
    }
}

public class ManagedReactor : ManagedInventoryBlock<IMyReactor>
{
    public ManagedReactor(IMyReactor block) : base(block)
    {
        block.UseConveyorSystem = false;
        Inventory = new ManagedInventory(this, block.GetInventory(), Filters);
    }

    public float CurrentOutput => block.CurrentOutput;
    public float MaxOutput => block.MaxOutput;
}

public class ManagedGasTank : ManagedInventoryBlock<IMyGasTank>
{
    public ManagedGasTank(IMyGasTank block) : base(block)
    {
        Inventory = new ManagedInventory(this, block.GetInventory(), Filters);
    }

    public bool IsOxygen => block.IsOxygen();
    public bool IsHydrogen => block.IsHydrogen();
    public float Capacity => block.Capacity;
    public double Stored => Capacity * FilledRatio;
    public double FilledRatio => block.FilledRatio;
}

public class ManagedBatteryBlock : ManagedBlock<IMyBatteryBlock>
{
    public ManagedBatteryBlock(IMyBatteryBlock block) : base(block) { }

    public float CurrentStoredPower => block.CurrentStoredPower;
    public float MaxStoredPower => block.MaxStoredPower;
    public float CurrentOutput => block.CurrentOutput;
    public float MaxOutput => block.MaxOutput;
}

public class ManagedShipConnector : ManagedInventoryBlock<IMyShipConnector>
{
    public ManagedShipConnector(IMyShipConnector block) : base(block)
    {
        Inventory = new ManagedInventory(this, block.GetInventory(), Filters);
    }

    public bool IsConnected => block.Status == MyShipConnectorStatus.Connected;
    public IMyCubeGrid ConnectedCubeGrid => block.OtherConnector.CubeGrid;
}

public class ManagedShipWelderInventory : ManagedInventory<ManagedShipWelder>
{
    public ManagedShipWelderInventory(ManagedShipWelder block, Filters filters) : base(block, ((IMyShipWelder)block.Block).GetInventory(), filters) { }

    public override bool Ready => !block.IsActivated && base.Ready;
}
public class ManagedShipWelder : ManagedInventoryBlock<IMyShipWelder>
{
    public ManagedShipWelder(IMyShipWelder block) : base(block)
    {
        block.UseConveyorSystem = true;
        Inventory = new ManagedShipWelderInventory(this, Filters);
    }

    public bool IsActivated => block.IsActivated;
}

IEnumerator<bool> Manage(StringBuilder log)
{
    if (!state.Initialized) yield break;

    if (state.masterAssemblers.Count > 0 && state.slaveAssemblers.Count > 0)
    {
        foreach (var id in state.masterAssemblers.ToList())
        {
            yield return true;
            var assembler = state.blocks.Assembler(id);
            if (assembler == null) continue;
            assembler.CooperativeMode = false;
        }
        foreach (var id in state.slaveAssemblers.ToList())
        {
            yield return true;
            var assembler = state.blocks.Assembler(id);
            if (assembler == null) continue;
            assembler.CooperativeMode = true;
        }
        if (debugMode)
        {
            log.Append($"Master Assemblers ({state.masterAssemblers.Count}):\n");
            foreach (var id in state.masterAssemblers.ToList())
            {
                yield return true;
                var assembler = state.blocks.Assembler(id);
                if (assembler == null) continue;
                log.Append($" - {assembler.Name}{(assembler.Enabled ? "" : "*")}\n");
            }
            log.Append($"Slave Assemblers ({state.slaveAssemblers.Count}):\n");
            foreach (var id in state.slaveAssemblers.ToList())
            {
                yield return true;
                var assembler = state.blocks.Assembler(id);
                if (assembler == null) continue;
                log.Append($" - {assembler.Name}{(assembler.Enabled ? "" : "*")}\n");
            }
        }
    }

    if (state.batteryBlocks.Count > 0 && manageReactorsPower && state.reactors.Count > 0)
    {
        var CurrentStoredPower = 0f;
        var MaxStoredPower = 0f;
        var CurrentOutput = 0f;
        var MaxOutput = 0f;
        foreach (var id in state.batteryBlocks.ToList())
        {
            yield return true;
            var battery = state.blocks.BatteryBlock(id);
            if (battery == null) continue;
            CurrentStoredPower += battery.CurrentStoredPower;
            MaxStoredPower += battery.MaxStoredPower;
            CurrentOutput += battery.CurrentOutput;
            MaxOutput += battery.MaxOutput;
        }
        var StoredPower = CurrentStoredPower / MaxStoredPower;
        var Output = CurrentOutput / MaxOutput;

        var EnableReactorsAtStoredPower = Output >= enableReactorsAtOutput || StoredPower <= enableReactorsAtStoredPower;
        if (EnableReactorsAtStoredPower || StoredPower >= disableReactorsAtStoredPower)
        {
            foreach (var id in state.reactors.ToList())
            {
                yield return true;
                var reactor = state.blocks.Reactor(id);
                if(reactor == null) continue;
                reactor.Enabled = EnableReactorsAtStoredPower;
            }
        }

        if (debugMode)
        {
            log.Append($"StoredPower: {StoredPower.ToPercent()} ({CurrentStoredPower}/{MaxStoredPower} MWh):\n");
            log.Append($"Batteries ({state.batteryBlocks.Count}):\n");
            foreach (var id in state.batteryBlocks.ToList())
            {
                yield return true;
                var battery = state.blocks.BatteryBlock(id);
                if (battery == null) continue;
                log.Append($" - {battery.Name}{(battery.Enabled ? "" : "*")} {(battery.CurrentStoredPower / battery.MaxStoredPower).ToPercent()}\n");
            }
            log.Append($"Reactors ({state.reactors.Count}):\n");
            foreach (var id in state.reactors.ToList())
            {
                yield return true;
                var reactor = state.blocks.Reactor(id);
                if (reactor == null) continue;
                log.Append($" - {reactor.Name}{(reactor.Enabled ? "" : "*")}\n");
            }
        }
    }

    if (state.gasGenerators.Count > 0 && manageGasGeneratorsGas && (state.oxygenTanks.Count + state.hydrogenTanks.Count > 0))
    {
        var OxygenCapacity = 0f;
        var OxygenStored = 0d;
        var HydrogenCapacity = 0f;
        var HydrogenStored = 0d;

        foreach (var id in state.oxygenTanks.ToList())
        {
            yield return true;
            var gasTank = state.blocks.GasTank(id);
            if (gasTank == null) continue;
            OxygenCapacity += gasTank.Capacity;
            OxygenStored += gasTank.Stored;
        }
        foreach (var id in state.hydrogenTanks.ToList())
        {
            yield return true;
            var gasTank = state.blocks.GasTank(id);
            if (gasTank == null) continue;
            HydrogenCapacity += gasTank.Capacity;
            HydrogenStored += gasTank.Stored;
        }
        var StoredGas = Math.Min(OxygenStored / OxygenCapacity, HydrogenStored / HydrogenCapacity);

        var EnableGasGeneratorsAtStoredGas = StoredGas <= enableGasGeneratorsAtStoredGas;
        if (EnableGasGeneratorsAtStoredGas || StoredGas >= disableGasGeneratorsAtStoredGas)
        {
            foreach (var id in state.gasGenerators.ToList())
            {
                yield return true;
                var gasGenerator = state.blocks.GasGenerator(id);
                if (gasGenerator == null) continue;
                gasGenerator.Enabled = EnableGasGeneratorsAtStoredGas;
            }
        }

        if (debugMode)
        {
            log.Append($"StoredGas: {StoredGas.ToPercent()} ({OxygenStored}/{OxygenCapacity} L O) ({HydrogenStored}/{HydrogenCapacity} L H):\n");
            log.Append($"Oxygen ({state.oxygenTanks.Count}):\n");
            foreach (var id in state.oxygenTanks.ToList())
            {
                yield return true;
                var gasTank = state.blocks.GasTank(id);
                if (gasTank == null) continue;
                log.Append($" - {gasTank.Name}{(gasTank.Enabled ? "" : "*")} {gasTank.FilledRatio.ToPercent()}\n");
            }
            log.Append($"Hydrogen ({state.hydrogenTanks.Count}):\n");
            foreach (var id in state.hydrogenTanks.ToList())
            {
                yield return true;
                var gasTank = state.blocks.GasTank(id);
                if (gasTank == null) continue;
                log.Append($" - {gasTank.Name}{(gasTank.Enabled ? "" : "*")} {gasTank.FilledRatio.ToPercent()}\n");
            }
            log.Append($"Generators ({state.gasGenerators.Count}):\n");
            foreach (var id in state.gasGenerators.ToList())
            {
                yield return true;
                var gasGenerator = state.blocks.GasGenerator(id);
                if (gasGenerator == null) continue;
                log.Append($" - {gasGenerator.Name}{(gasGenerator.Enabled ? "" : "*")}\n");
            }
        }
    }
}

IEnumerator<bool> Scan(StringBuilder log)
{
    var blocks = state.blocks;
    var scanned = state.scanned;

    var targetsUpdated = false;
    var sources = new List<IManagedInventory>(state.sources);
    var targets = new List<IManagedInventory>(state.targets);
    var masterAssemblers = new HashSet<long>(state.masterAssemblers);
    var slaveAssemblers = new HashSet<long>(state.slaveAssemblers);
    var refineries = new HashSet<long>(state.refineries);
    var gasGenerators = new HashSet<long>(state.gasGenerators);
    var oxygenTanks = new HashSet<long>(state.oxygenTanks);
    var hydrogenTanks = new HashSet<long>(state.hydrogenTanks);
    var batteryBlocks = new HashSet<long>(state.batteryBlocks);
    var reactors = new HashSet<long>(state.reactors);
    var shipConnectors = new HashSet<long>(state.shipConnectors);

    Action<long> remove = (id) =>
    {
        blocks.Remove(id);
        scanned.Remove(id);
        sources.RemoveAll(source => source.Block.Block.EntityId == id);
        targets.RemoveAll(target => target.Block.Block.EntityId == id);
        masterAssemblers.Remove(id);
        slaveAssemblers.Remove(id);
        refineries.Remove(id);
        gasGenerators.Remove(id);
        oxygenTanks.Remove(id);
        hydrogenTanks.Remove(id);
        batteryBlocks.Remove(id);
        reactors.Remove(id);
        shipConnectors.Remove(id);
    };

    var targetsCount = targets.Count;
    foreach (var e in blocks.ToList())
    {
        yield return true;
        if (e.Value.Closed || e.Value.Changed) remove(e.Key);
    }
    if (targetsCount != targets.Count) targetsUpdated = true;

    bool foundOtherMe = false;
    foreach (var block in GridTerminalSystem.GetBlocks())
    {
        yield return true;
        if (block == Me) continue;

        if (
            block is IMyProgrammableBlock &&
            (block.CubeGrid.EntityId < Me.CubeGrid.EntityId || block.EntityId < Me.EntityId) &&
            ((IMyProgrammableBlock)block).Enabled &&
            block.CustomName.EndsWith(myName)
            )
        {
            foundOtherMe = true;
            continue;
        }

        var id = block.EntityId;
        var local = block.CubeGrid == Me.CubeGrid || block.CubeGrid.CustomName.StartsWith(Me.CubeGrid.CustomName);
        if (scanned.Contains(id)) continue;
        scanned.Add(id);

        if (block.CustomName.IndexOf(ignoreTag) >= 0)
        {
            blocks.Add(id, new WatchedBlock(block));
            continue;
        }

        if (manageAssemblers && block is IMyAssembler)
        {
            var assembler = new ManagedAssembler((IMyAssembler)block);
            blocks.Add(id, assembler);
            sources.Add(assembler.Input);
            sources.Add(assembler.Output);
            if (assembler.Input.Filters.Count > 0) targets.Add(assembler.Input);
            if (local) (assembler.DefinedMaster ? masterAssemblers : slaveAssemblers).Add(id);
        }
        else if (manageRefineries && block is IMyRefinery)
        {
            var refinery = new ManagedRefinery((IMyRefinery)block);
            blocks.Add(id, refinery);
            sources.Add(refinery.Input);
            if (refinery.Input.Filters.Count > 0) targets.Add(refinery.Input);
            sources.Add(refinery.Output);
            if (local) refineries.Add(id);
        }
        else if (manageGasGenerators && block is IMyGasGenerator)
        {
            var gasGenerator = new ManagedGasGenerator((IMyGasGenerator)block);
            blocks.Add(id, gasGenerator);
            sources.Add(gasGenerator.Inventory);
            if (gasGenerator.Inventory.Filters.Count > 0) targets.Add(gasGenerator.Inventory);
            if (local) gasGenerators.Add(id);
        }
        else if (manageGasTanks && block is IMyGasTank)
        {
            var gasTank = new ManagedGasTank((IMyGasTank)block);
            blocks.Add(id, gasTank);
            sources.Add(gasTank.Inventory);
            if (gasTank.Inventory.Filters.Count > 0) targets.Add(gasTank.Inventory);
            if (local)
            {
                if (gasTank.IsOxygen) oxygenTanks.Add(id);
                else if (gasTank.IsHydrogen) hydrogenTanks.Add(id);
            }
        }
        else if (manageBatteryBlocks && block is IMyBatteryBlock)
        {
            var batteryBlock = new ManagedBatteryBlock((IMyBatteryBlock)block);
            blocks.Add(id, batteryBlock);
            if (local) batteryBlocks.Add(id);
        }
        else if (manageReactors && block is IMyReactor)
        {
            var reactor = new ManagedReactor((IMyReactor)block);
            blocks.Add(id, reactor);
            sources.Add(reactor.Inventory);
            if (reactor.Inventory.Filters.Count > 0) targets.Add(reactor.Inventory);
            if (local) reactors.Add(id);
        }
        else if (manageShipConnectors && block is IMyShipConnector)
        {
            var shipConnector = new ManagedShipConnector((IMyShipConnector)block);
            blocks.Add(id, shipConnector);
            sources.Add(shipConnector.Inventory);
            if (shipConnector.Inventory.Filters.Count > 0) targets.Add(shipConnector.Inventory);
            if (local) shipConnectors.Add(id);
        }
        else if (manageShipWelders && block is IMyShipWelder)
        {
            var shipWelder = new ManagedShipWelder((IMyShipWelder)block);
            blocks.Add(id, shipWelder);
            sources.Add(shipWelder.Inventory);
            if (shipWelder.Inventory.Filters.Count > 0) targets.Add(shipWelder.Inventory);
        }
        else if (block.HasInventory && defaultFilters.ContainsKey(block.DefinitionDisplayNameText))
        {
            if (block is IMyProductionBlock)
            {
                var productionBlock = new ManagedProductionBlock((IMyProductionBlock)block);
                blocks.Add(id, productionBlock);
                sources.Add(productionBlock.Output);
                if (productionBlock.Input.Filters.Count > 0) targets.Add(productionBlock.Input);
            }
            else
            {
                var inventoryBlock = new ManagedInventoryBlock(block);
                blocks.Add(id, inventoryBlock);
                sources.Add(inventoryBlock.Inventory);
                if (inventoryBlock.Inventory.Filters.Count > 0) targets.Add(inventoryBlock.Inventory);
            }
        }
        else if (block.HasInventory)
        {
            blocks.Add(id, new WatchedBlock(block));
            // scanned.Remove(id);
            // log.Append($"{block.DefinitionDisplayNameText} ({block.CustomName})\n");
        }
    }
    if (state.foundOtherMe = foundOtherMe)
    {
        log.Append("Found other script block\n");
    }
    if (targetsCount != targets.Count) targetsUpdated = true;

    var errorBlocks = blocks.Where(e => e.Value.HasError).ToList();
    if (errorBlocks.Count > 0)
    {
        log.Append($"Errors ({errorBlocks.Count}):\n");
        foreach (var e in errorBlocks)
        {
            yield return true;
            log.Append($" - {e.Value.Name}: {e.Value.Error}\n");
        }
    }

    state.Update(
        targetsUpdated,
        sources,
        targets,
        masterAssemblers,
        slaveAssemblers,
        refineries,
        gasGenerators,
        oxygenTanks,
        hydrogenTanks,
        batteryBlocks,
        reactors,
        shipConnectors
    );
}

public class State
{
    bool initialized = false;
    public bool Initialized => initialized;

    public bool foundOtherMe = false;

    public ManagedBlocks blocks = new ManagedBlocks();
    public HashSet<long> scanned = new HashSet<long>();
    public Dictionary<MyItemType, List<ItemTarget>> itemTargets = new Dictionary<MyItemType, List<ItemTarget>>();

    public bool targetsUpdated = false;
    public List<IManagedInventory> sources = new List<IManagedInventory>();
    public List<IManagedInventory> targets = new List<IManagedInventory>();
    public HashSet<long> masterAssemblers = new HashSet<long>();
    public HashSet<long> slaveAssemblers = new HashSet<long>();
    public HashSet<long> refineries = new HashSet<long>();
    public HashSet<long> gasGenerators = new HashSet<long>();
    public HashSet<long> oxygenTanks = new HashSet<long>();
    public HashSet<long> hydrogenTanks = new HashSet<long>();
    public HashSet<long> batteryBlocks = new HashSet<long>();
    public HashSet<long> reactors = new HashSet<long>();
    public HashSet<long> shipConnectors = new HashSet<long>();

    public void Update(
        bool targetsUpdated,
        List<IManagedInventory> sources,
        List<IManagedInventory> targets,
        HashSet<long> masterAssemblers,
        HashSet<long> slaveAssemblers,
        HashSet<long> refineries,
        HashSet<long> gasGenerators,
        HashSet<long> oxygenTanks,
        HashSet<long> hydrogenTanks,
        HashSet<long> batteryBlocks,
        HashSet<long> reactors,
        HashSet<long> shipConnectors
    )
    {
        this.targetsUpdated = targetsUpdated;
        Util.Swap(ref this.sources, ref sources);
        Util.Swap(ref this.targets, ref targets);
        Util.Swap(ref this.masterAssemblers, ref masterAssemblers);
        Util.Swap(ref this.slaveAssemblers, ref slaveAssemblers);
        Util.Swap(ref this.refineries, ref refineries);
        Util.Swap(ref this.gasGenerators, ref gasGenerators);
        Util.Swap(ref this.oxygenTanks, ref oxygenTanks);
        Util.Swap(ref this.hydrogenTanks, ref hydrogenTanks);
        Util.Swap(ref this.batteryBlocks, ref batteryBlocks);
        Util.Swap(ref this.reactors, ref reactors);
        Util.Swap(ref this.shipConnectors, ref shipConnectors);
        initialized = true;
    }
}

IEnumerator<bool> Transfer(StringBuilder log)
{
    if (!state.Initialized || state.foundOtherMe) yield break;

    if (state.targetsUpdated)
    {
        foreach (var target in state.itemTargets) target.Value.Clear();

        {
            List<ItemTarget> itemTargets;
            foreach (var target in state.targets.ToList())
            {
                foreach (var filter in target.Filters)
                {
                    foreach (var type in filter.Types)
                    {
                        yield return true;
                        if (!state.itemTargets.TryGetValue(type, out itemTargets)) state.itemTargets.Add(type, itemTargets = new List<ItemTarget>());
                        itemTargets.Add(new ItemTarget(target, filter));
                    }
                }
            }
        }

        foreach (var itemTargets in state.itemTargets.Values)
        {
            yield return true;
            itemTargets.Sort(itemTargetComparer);
        }
    }

    foreach (var source in state.sources.ToList())
    {
        yield return true;
        if (!source.Ready) continue;

        foreach (var e in source.Inventory.SumItems().ToList())
        {
            yield return true;

            var type = e.Key;
            List<ItemTarget> targets;
            if (!state.itemTargets.TryGetValue(type, out targets)) continue;

            var sourceAmount = e.Value;
            var sourceFilters = source.ApplyFilters ? source.Filters.Where(filter => filter.Types.Contains(type)) : Filters.None;

            foreach (var target in targets.ToList())
            {
                yield return true;
                if (!target.Inventory.Ready || !target.Inventory.ApplyFilters || source.Inventory == target.Inventory.Inventory || target.Inventory.Inventory.IsFull) continue;

                var targetAmount = target.Inventory.Inventory.GetItemAmount(type);

                if (target.HasQuota && targetAmount >= target.Quota) continue;

                var toTransfer = sourceAmount;
                if (target.HasQuota) toTransfer = MyFixedPoint.Min(toTransfer, target.Quota - targetAmount);

                foreach (var sourceFilter in sourceFilters)
                {
                    if (sourceFilter.Priority < target.Filter.Priority) continue;
                    else if (sourceFilter.Priority == target.Filter.Priority && (sourceFilter.HasQuota || target.Filter.HasQuota))
                    {
                        if (sourceFilter.HasQuota && (!target.Filter.HasQuota || sourceFilter.Quota <= target.Filter.Quota))
                        {
                            toTransfer = MyFixedPoint.Min(toTransfer, sourceAmount - sourceFilter.Quota);
                        }
                    }
                    else
                    {
                        toTransfer = MyFixedPoint.Min(toTransfer, sourceFilter.HasQuota ? sourceAmount - sourceFilter.Quota : MyFixedPoint.Zero);
                    }

                    if (toTransfer <= MyFixedPoint.Zero) break;
                }
                if (toTransfer <= MyFixedPoint.Zero) continue;

                sourceAmount -= source.Inventory.TransferItemTypeTo(target.Inventory.Inventory, type, toTransfer);
                if (sourceAmount <= MyFixedPoint.Zero) break;
            }
        }
    }

    if (debugMode)
    {
        log.Append($"Sources ({state.sources.Count}):\n");
        foreach (var source in state.sources.ToList())
        {
            yield return true;
            log.Append($" - {source.Block.Name}{(source.Ready ? "" : "*")}\n");
        }
        log.Append($"Targets ({state.targets.Count}):\n");
        foreach (var e in state.itemTargets)
        {
            yield return true;
            if (e.Value.Count == 0) continue;
            log.Append($" - {e.Key.DisplayName()} ({e.Key.Group()}):\n");
            foreach (var itemTarget in e.Value)
            {
                log.Append($"   - {parseFilterPriority}{itemTarget.Priority}{(itemTarget.HasQuota ? $" {parseFilterQuota}{itemTarget.Quota}" : "")} {itemTarget.Inventory.Block.Name}\n");
            }
        }
    }
}

public static class Util
{
    public static void Swap<T>(ref T a, ref T b)
    {
        var c = a;
        a = b;
        b = c;
    }

    public static bool StringSection(string value, string prefix, string suffix, out string result)
    {
        var end = value.LastIndexOf(suffix);
        if (end < 0) goto Fail;

        var begin = value.LastIndexOf(prefix, end - prefix.Length);
        if (begin < 0) goto Fail;

        result = value.SubStr(begin + prefix.Length, end);
        return true;

    Fail:
        result = null;
        return false;
    }
}

public class Worker
{
    readonly IMyGridProgramRuntimeInfo runtime;
    readonly Func<StringBuilder, IEnumerator<bool>> taskFn;
    IEnumerator<bool> task = null;
    bool? current = null;
    readonly StringBuilder logBuilder = new StringBuilder();
    string log = "pending...\n";
    int cycleCountInProgress, cycleCount = -1;
    int instructionCountInProgress, instructionCount = -1;
    int totalRuns = 0;
    int maxInstructionCount = -1;

    const string idle = "idle...\n";

    public string Log => log;
    public int CycleCount => cycleCount;
    public int InstructionCount => instructionCount;
    public int TotalRuns => totalRuns;
    public int MaxInstructionCount => maxInstructionCount;

    public Worker(IMyGridProgramRuntimeInfo runtime, Func<StringBuilder, IEnumerator<bool>> taskFn)
    {
        this.runtime = runtime;
        this.taskFn = taskFn;
    }

    public void Cycle(int targetInstructionCount)
    {
        if (task == null)
        {
            task = taskFn(logBuilder);
            current = null;
            logBuilder.Clear();
            cycleCountInProgress = 0;
            instructionCountInProgress = 0;
        }

        var startInstructionCount = runtime.CurrentInstructionCount;
        bool hasNext;

        while (hasNext = task.MoveNext())
        {
            current = task.Current;
            if (runtime.CurrentInstructionCount >= targetInstructionCount) break;
        }
        ++cycleCountInProgress;
        var cycleInstructionCount = runtime.CurrentInstructionCount - startInstructionCount;
        if (cycleInstructionCount > maxInstructionCount) maxInstructionCount = cycleInstructionCount;
        instructionCountInProgress += cycleInstructionCount;

        if (!hasNext)
        {
            task.Dispose();
            task = null;
            if (current == true)
            {
                cycleCount = cycleCountInProgress;
                instructionCount = instructionCountInProgress;
                log = $"({maxInstructionCount}/{instructionCount}/{cycleCount}) [{++totalRuns}]{(logBuilder.Length > 0 ? ":" : "")}\n{logBuilder}";
            }
            else
            {
                log = idle;
            }
        }
    }
}

}
static class Extensions
{
    public static bool ContainsIgnoreCase(this string s, string value) => s.ToLower().Contains(value.ToLower());
    public static string SubStr(this string s, int begin, int end) => s.Substring(begin, end - begin);
    public static string SubStr(this string s, int begin = 0) => s.SubStr(begin, s.Length);

    public static string ToPercent(this float f) => f.ToString("0.00%");
    public static string ToPercent(this double d) => d.ToString("0.00%");

    public static List<IMyTerminalBlock> GetBlocks(this IMyGridTerminalSystem gts)
    {
        Program.Instance.IMyGridTerminalSystem_GetBlocks_blocks.Clear();
        gts.GetBlocksOfType<IMyTerminalBlock>(Program.Instance.IMyGridTerminalSystem_GetBlocks_blocks);
        return Program.Instance.IMyGridTerminalSystem_GetBlocks_blocks;
    }

    public static MyFixedPoint FreeVolume(this IMyInventory i) => i.MaxVolume - i.CurrentVolume;
    public static MyFixedPoint FitItems(this IMyInventory i, MyItemType type) => MyFixedPoint.MultiplySafe(i.FreeVolume(), 1 / type.GetItemInfo().Volume);
    public static MyFixedPoint TransferItemToSafe(this IMyInventory i, IMyInventory dstInventory, MyInventoryItem item, MyFixedPoint amount)
    {
        MyFixedPoint safeAmount = MyFixedPoint.Min(dstInventory.FitItems(item.Type), amount);
        return safeAmount > MyFixedPoint.Zero && i.TransferItemTo(dstInventory, item, safeAmount) ? safeAmount : MyFixedPoint.Zero;
    }
    public static MyFixedPoint TransferItemFromSafe(this IMyInventory i, IMyInventory sourceInventory, MyInventoryItem item, MyFixedPoint amount)
    {
        return sourceInventory.TransferItemToSafe(i, item, amount);
    }
    public static MyFixedPoint TransferItemTypeTo(this IMyInventory i, IMyInventory dstInventory, MyItemType type, MyFixedPoint amount)
    {
        if (!i.CanTransferItemTo(dstInventory, type)) return MyFixedPoint.Zero;

        var transferred = MyFixedPoint.Zero;
        var safeAmount = MyFixedPoint.Min(dstInventory.FitItems(type), amount);
        var items = i.GetItems().Where(item => item.Type == type).ToList();
        for (var ii = 0; ii < items.Count && transferred < safeAmount; ++ii)
        {
            var item = items[ii];
            var toTransfer = MyFixedPoint.Min(item.Amount, safeAmount - transferred);
            if (!i.TransferItemTo(dstInventory, item, toTransfer)) break;
            transferred += toTransfer;
        }
        return transferred;
    }
    public static MyFixedPoint TransferItemTypeFrom(this IMyInventory i, IMyInventory sourceInventory, MyItemType type, MyFixedPoint amount)
    {
        return sourceInventory.TransferItemTypeTo(i, type, amount);
    }
    public static List<MyInventoryItem> GetItems(this IMyInventory i)
    {
        Program.Instance.IMyInventory_GetItems_items.Clear();
        i.GetItems(Program.Instance.IMyInventory_GetItems_items);
        return Program.Instance.IMyInventory_GetItems_items;
    }
    public static void SumItems(this IMyInventory i, Dictionary<MyItemType, MyFixedPoint> dict)
    {
        foreach (var item in i.GetItems())
        {
            MyFixedPoint amount;
            if (!dict.TryGetValue(item.Type, out amount)) amount = MyFixedPoint.Zero;
            dict[item.Type] = amount + item.Amount;
        }
    }
    public static Dictionary<MyItemType, MyFixedPoint> SumItems(this IMyInventory i)
    {
        Program.Instance.IMyInventory_SumItems_items.Clear();
        i.SumItems(Program.Instance.IMyInventory_SumItems_items);
        return Program.Instance.IMyInventory_SumItems_items;
    }
    public static List<MyItemType> GetAcceptedItems(this IMyInventory i)
    {
        Program.Instance.IMyInventory_GetAcceptedItems_itemTypes.Clear();
        i.GetAcceptedItems(Program.Instance.IMyInventory_GetAcceptedItems_itemTypes);
        return Program.Instance.IMyInventory_GetAcceptedItems_itemTypes;
    }

    public static string DisplayName(this MyItemType it) => Program.Instance.GetItem(it).DisplayName;
    public static string Group(this MyItemType it) => Program.Instance.GetItem(it).Group;

    public static bool IsOxygen(this IMyGasTank gt) => gt.BlockDefinition.SubtypeId.Length == 0 || gt.BlockDefinition.SubtypeId.Contains("Oxygen");
    public static bool IsHydrogen(this IMyGasTank gt) => gt.BlockDefinition.SubtypeId.Contains("Hydrogen");

    public static UpdateType ToUpdateType(this UpdateFrequency ut)
    {
        switch (ut)
        {
            case UpdateFrequency.Once: return UpdateType.Once;
            case UpdateFrequency.Update1: return UpdateType.Update1;
            case UpdateFrequency.Update10: return UpdateType.Update10;
            case UpdateFrequency.Update100: return UpdateType.Update100;
            default: return UpdateType.None;
        }
    }

    public static void Prepare(this IMyTextSurface ts,
        ContentType ContentType = ContentType.TEXT_AND_IMAGE,
        TextAlignment Alignment = TextAlignment.LEFT,
        string Font = "Debug",
        float FontSize = 1f,
        float TextPadding = 1f
    )
    {
        ts.ContentType = ContentType;
        ts.Alignment = Alignment;
        ts.Font = Font;
        ts.FontSize = FontSize;
        ts.TextPadding = TextPadding;
    }