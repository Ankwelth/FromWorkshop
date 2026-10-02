/// Resource Exchanger version 2.6.0 2023-10-02 for SE 1.203.024
/// Made by Sinus32
/// https://steamcommunity.com/sharedfiles/filedetails/546221822
///
/// Warning! This script does not require any timer blocks and will run immediately.
/// If you want to stop it just switch the programmable block off.
///
/// Configuration can be changed in custom data of the programmable block.

/** Default configuration *****************************************************************/

public bool AnyConstruct = false;
public string DisplayLcdGroup = "Resource exchanger output";
public string DrillsPayloadLightsGroup = "Payload indicators";
public bool EnableDrills = true;
public bool EnableGroups = true;
public bool EnableOxygenGenerators = true;
public bool EnableReactors = true;
public bool EnableRefineries = true;
public bool EnableTurrets = true;
public string GroupTagPattern = @"\bGR\d{1,3}\b";
public MyItemType? LowestRefineryPriority = MyItemType.MakeOre("Stone");
public string ManagedBlocksGroup = "";
public MyItemType? TopRefineryPriority = MyItemType.MakeOre("Iron");

/** Implementation ************************************************************************/

internal readonly Dictionary<MyItemType, ItemInfo> _itemInfo;
private const string ConfigSection = "ResourceExchanger";
private const string OreType = "MyObjectBuilder_Ore";
private const double SmallNumber = 0.000005;
private readonly int[] _avgMovements;
private readonly Dictionary<MyDefinitionId, List<MyItemType>> _blockMap;
private readonly Func<IMyTerminalBlock, bool> _hasInventory, _isFunctional;
private BlockStore _blockStore = null;
private int _cycleNumber = 0;
private object _prevConfig;

public Program()
{
    _itemInfo = new Dictionary<MyItemType, ItemInfo>();
    _blockMap = new Dictionary<MyDefinitionId, List<MyItemType>>();
    _avgMovements = new int[0x08];
    _hasInventory = (IMyTerminalBlock block) => block.IsFunctional && block.HasInventory && (AnyConstruct || block.IsSameConstructAs(Me));
    _isFunctional = (IMyTerminalBlock block) => block.IsFunctional && (AnyConstruct || block.IsSameConstructAs(Me));
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    ReadConfig();

    var stat = new Statistics();
    BlockStore bs = CollectTerminals(stat, (_cycleNumber & 0x0f) > 0);
    if (bs == null)
    {
        PrintOnlineStatus(bs, stat);
        return;
    }

    if (Runtime.CurrentInstructionCount < (Runtime.MaxInstructionCount >> 2))
    {
        ProcessBlocks("Balancing reactors", EnableReactors, bs.Reactors, stat, exclude: bs.AllGroupedInventories);
        ProcessBlocks("Balancing refineries", EnableRefineries, bs.Refineries, stat, exclude: bs.AllGroupedInventories);
        var dcn = ProcessBlocks("Balancing drills", EnableDrills, bs.Drills, stat, exclude: bs.AllGroupedInventories);
        stat.NotConnectedDrillsFound = dcn > 1;
        ProcessBlocks("Balancing turrets", EnableTurrets, bs.Turrets, stat, exclude: bs.AllGroupedInventories);
        ProcessBlocks("Balancing oxygen gen.", EnableOxygenGenerators, bs.OxygenGenerators, stat,
            exclude: bs.AllGroupedInventories, filter: item => item.Type.TypeId == OreType);

        if (EnableGroups)
        {
            foreach (var kv in bs.Groups)
                ProcessBlocks("Balancing group " + kv.Key, true, kv.Value, stat);
        }
        else
        {
            stat.Output.AppendLine("Balancing groups: disabled");
        }

        if (EnableRefineries)
            EnforceItemPriority(bs.Refineries, stat, TopRefineryPriority, LowestRefineryPriority);

        if (dcn >= 1)
            ProcessDrillsLights(bs.Drills, bs.DrillsPayloadLights, stat);
    }

    PrintOnlineStatus(bs, stat);
    WriteOutput(bs, stat);
}

public void Save()
{ }

internal void ReadConfig()
{
    const string anyConstructComment = "\n Set to \"true\" to enable exchanging items with connected ships (AC mode)."
        + "\n Keep \"false\" if blocks connected by connectors should not be affected (SC mode).";
    const string managedGroupComment = "\n Optional name of a group of blocks that will be affected"
        + "\n by the script. By default all blocks connected to the grid are processed,"
        + "\n but you can set this to force the script to affect only certain blocks.";
    const string reactorsComment = "\n Enables exchanging uranium between reactors";
    const string refineriesComment = "\n Enables exchanging ore between refineries and arc furnaces";
    const string drillsComment = "\n Enables exchanging ore between drills and"
        + "\n processing lights that indicates how much free space left in drills";
    const string turretsComment = "\n Enables exchanging ammunition between turrets and launchers";
    const string oxygenGeneratorsComment = "\n Enables exchanging ice between oxygen generators";
    const string groupsComment = "\n Enables exchanging items in blocks of custom groups";
    const string drillsPayloadLightsGroupComment = "\n Name of a group of lights that will be used as indicators of space"
        + "\n left in drills. Both Interior Light and Spotlight are supported."
        + "\n The lights will change colors to tell you how much free space left:"
        + "\n White - All drills are connected to each other and they are empty."
        + "\n Yellow - Drills are full in a half."
        + "\n Red - Drills are almost full (95%)."
        + "\n Purple - Less than 5 m³ of free space left."
        + "\n Cyan - Some drills are not connected to each other.";
    const string topRefineryPriorityComment = "\n Top priority item type to process in refineries"
        + "\n and/or arc furnaces. The script will move an item of this type to"
        + "\n the first slot of a refinery or arc furnace if it find that item"
        + "\n in the refinery (or arc furnace) processing queue.";
    const string lowestRefineryPriorityComment = "\n Lowest priority item type to process in refineries"
        + "\n and/or arc furnaces. The script will move an item of this type to"
        + "\n the last slot of a refinery or arc furnace if it find that item"
        + "\n in the refinery (or arc furnace) processing queue.";
    const string groupTagPatternComment = "\n Regular expression used to recognize groups";
    const string displayLcdGroupComment = "\n Group of wide LCD screens that will act as debugger output for"
        + "\n this script. You can name this screens as you wish, but pay attention"
        + "\n that they will be used in alphabetical order according to their names.";

    if (ReferenceEquals(Me.CustomData, _prevConfig))
        return;

    MyIniParseResult result;
    var ini = new MyIni();
    if (!ini.TryParse(Me.CustomData, out result))
    {
        Echo(String.Format("Err: invalid config in line {0}: {1}", result.LineNo, result.Error));
        return;
    }

    ReadConfigBoolean(ini, nameof(AnyConstruct), ref AnyConstruct, anyConstructComment);
    ReadConfigString(ini, nameof(ManagedBlocksGroup), ref ManagedBlocksGroup, managedGroupComment);
    ReadConfigBoolean(ini, nameof(EnableReactors), ref EnableReactors, reactorsComment);
    ReadConfigBoolean(ini, nameof(EnableRefineries), ref EnableRefineries, refineriesComment);
    ReadConfigBoolean(ini, nameof(EnableDrills), ref EnableDrills, drillsComment);
    ReadConfigBoolean(ini, nameof(EnableTurrets), ref EnableTurrets, turretsComment);
    ReadConfigBoolean(ini, nameof(EnableOxygenGenerators), ref EnableOxygenGenerators, oxygenGeneratorsComment);
    ReadConfigBoolean(ini, nameof(EnableGroups), ref EnableGroups, groupsComment);
    ReadConfigString(ini, nameof(DrillsPayloadLightsGroup), ref DrillsPayloadLightsGroup, drillsPayloadLightsGroupComment);
    ReadConfigItemType(ini, nameof(TopRefineryPriority), ref TopRefineryPriority, topRefineryPriorityComment);
    ReadConfigItemType(ini, nameof(LowestRefineryPriority), ref LowestRefineryPriority, lowestRefineryPriorityComment);
    ReadConfigString(ini, nameof(GroupTagPattern), ref GroupTagPattern, groupTagPatternComment);
    ReadConfigString(ini, nameof(DisplayLcdGroup), ref DisplayLcdGroup, displayLcdGroupComment);

    Me.CustomData = ini.ToString();
    Echo("Configuration readed");
    _prevConfig = Me.CustomData;
}

private void ReadConfigBoolean(MyIni ini, string name, ref bool value, string comment)
{
    var key = new MyIniKey(ConfigSection, name);
    MyIniValue val = ini.Get(key);
    bool tmp;
    if (val.TryGetBoolean(out tmp))
        value = tmp;
    else
        ini.Set(key, value);
    ini.SetComment(key, comment);
}

private void ReadConfigString(MyIni ini, string name, ref string value, string comment)
{
    var key = new MyIniKey(ConfigSection, name);
    MyIniValue val = ini.Get(key);
    string tmp;
    if (val.TryGetString(out tmp))
        value = tmp.Trim();
    else
        ini.Set(key, value);
    ini.SetComment(key, comment);
}

private void ReadConfigItemType(MyIni ini, string name, ref MyItemType? value, string comment)
{
    var key = new MyIniKey(ConfigSection, name);
    MyIniValue val = ini.Get(key);
    string tmp;
    if (val.TryGetString(out tmp))
        value = String.IsNullOrEmpty(tmp) ? null : MyItemType.Parse(tmp.Trim());
    else
        ini.Set(key, value.HasValue ? value.Value.ToString() : String.Empty);
    ini.SetComment(key, comment);
}

internal BlockStore CollectTerminals(Statistics stat, bool refreshReferences)
{
    BlockStore bs;
    if (refreshReferences && _blockStore != null)
    {
        bs = _blockStore;
        bs.RefreshReferences(stat);
    }
    else
    {
        bs = new BlockStore(this);
        stat.DiscoveryDone = true;
        var blocks = new List<IMyTerminalBlock>();

        if (String.IsNullOrEmpty(ManagedBlocksGroup))
        {
            GridTerminalSystem.GetBlocksOfType(blocks, _hasInventory);
        }
        else
        {
            var group = GridTerminalSystem.GetBlockGroupWithName(ManagedBlocksGroup);
            if (group == null)
                stat.Output.Append("Error: a group ").Append(ManagedBlocksGroup).AppendLine(" has not been found");
            else
                group.GetBlocksOfType(blocks, _hasInventory);
        }

        var top = Runtime.MaxInstructionCount;
        top -= top >> 2;
        foreach (var dt in blocks)
        {
            if (bs.Collect(dt) && Runtime.CurrentInstructionCount > top)
                return null;
        }

        if (!String.IsNullOrEmpty(DisplayLcdGroup))
        {
            var group = GridTerminalSystem.GetBlockGroupWithName(DisplayLcdGroup);
            if (group != null)
                group.GetBlocksOfType<IMyTextPanel>(bs.DebugScreen, _isFunctional);
        }

        if (!String.IsNullOrEmpty(DrillsPayloadLightsGroup))
        {
            var group = GridTerminalSystem.GetBlockGroupWithName(DrillsPayloadLightsGroup);
            if (group != null)
                group.GetBlocksOfType<IMyLightingBlock>(bs.DrillsPayloadLights, _isFunctional);
        }
        _blockStore = bs;
    }

    stat.Output.Append("Resource exchanger 2.6.0. Blocks managed:");
    CountOrNA(stat.Output, " reactors: ", EnableReactors, bs.Reactors);
    CountOrNA(stat.Output, " refineries: ", EnableRefineries, bs.Refineries).AppendLine(",");
    CountOrNA(stat.Output, "oxygen gen.: ", EnableOxygenGenerators, bs.OxygenGenerators);
    CountOrNA(stat.Output, ", drills: ", EnableDrills, bs.Drills);
    CountOrNA(stat.Output, ", turrets: ", EnableTurrets, bs.Turrets);
    CountOrNA(stat.Output, ", cargo cont.: ", EnableGroups, bs.CargoContainers);
    CountOrNA(stat.Output, ", custom groups: ", EnableGroups, bs.Groups).AppendLine();
    return bs;
}

private static StringBuilder CountOrNA(StringBuilder sb, string lbl, bool enabled, ICollection c)
    => sb.Append(lbl).Append(enabled ? c.Count.ToString() : "n/a");

internal int ProcessBlocks(string msg, bool enable, ICollection<BlockWrapper> blocks, Statistics stat,
    HashSet<BlockWrapper> exclude = null, Func<MyInventoryItem, bool> filter = null)
{
    stat.Output.Append(msg);
    if (enable)
    {
        if (blocks.Count >= 2)
        {
            var conveyorNetworks = DivideBlocks(blocks, exclude);
            stat.NumberOfNetworks += conveyorNetworks.Count;
            stat.Output.Append(": ").Append(conveyorNetworks.Count).AppendLine(" conveyor networks found");

            foreach (var network in conveyorNetworks)
                BalanceInventories(stat, network.Blocks, network.No, network.Name, filter);

            return conveyorNetworks.Count;
        }
        else
        {
            stat.Output.AppendLine(": nothing to do");
            return 0;
        }
    }
    else
    {
        stat.Output.AppendLine(": disabled");
        return -1;
    }
}

private List<InventoryGroup> DivideBlocks(ICollection<BlockWrapper> inventories, HashSet<BlockWrapper> exclude)
{
    const string MY_OBJECT_BUILDER = "MyObjectBuilder_";

    var result = new List<InventoryGroup>();

    foreach (var wrp1 in inventories)
    {
        if (exclude != null && exclude.Contains(wrp1))
            continue;

        bool add = true;
        for (int n = result.Count - 1; n >= 0; --n)
        {
            var network = result[n];
            var wrp2 = network.Blocks[0];
            if (ReferenceEquals(wrp1.AcceptedItems, wrp2.AcceptedItems) && wrp1.GetInventory().IsConnectedTo(wrp2.GetInventory()))
            {
                network.Blocks.Add(wrp1);
                add = false;
                break;
            }
        }

        if (add)
        {
            var name = wrp1.Block.BlockDefinition.ToString();
            if (name.StartsWith(MY_OBJECT_BUILDER))
                name = name.Substring(MY_OBJECT_BUILDER.Length);

            var network = new InventoryGroup(result.Count + 1, name);
            network.Blocks.Add(wrp1);
            result.Add(network);
        }
    }

    return result;
}

private void BalanceInventories(
    Statistics stat, List<BlockWrapper> group, int networkNumber,
    string groupName, Func<MyInventoryItem, bool> filter)
{
    if (group.Count < 2)
    {
        stat.Output.Append("Cannot balance conveyor network ").Append(networkNumber)
            .Append(" group \"").Append(groupName).AppendLine("\"")
            .AppendLine("  because there is only one inventory.");
        return; // nothing to do
    }

    foreach (var wrp in group)
        wrp.LoadVolume(this, stat, filter);

    BlockWrapper min = group[0], max = group[0];
    for (int i = 1; i < group.Count; ++i)
    {
        var dt = group[i];
        if (min.Percent > dt.Percent)
            min = dt;
        if (max.Percent < dt.Percent)
            max = dt;
    }

    if (max.CurrentVolume < SmallNumber)
    {
        stat.Output.Append("Cannot balance conveyor network ").Append(networkNumber)
            .Append(" group \"").Append(groupName)
            .AppendLine("\"")
            .AppendLine("  because of lack of items in it.");
        return; // nothing to do
    }

    stat.Output.Append("Balancing conveyor network ").Append(networkNumber)
        .Append(" group \"").Append(groupName).AppendLine("\"...");

    if (min == max)
    {
        stat.Output.AppendLine("  nothing to do");
        return;
    }

    double toMove;
    if (min.MaxVolume == max.MaxVolume)
    {
        toMove = (max.CurrentVolume - min.CurrentVolume) / 2.0;
    }
    else
    {
        toMove = (max.CurrentVolume * min.MaxVolume - min.CurrentVolume * max.MaxVolume)
            / (min.MaxVolume + max.MaxVolume);
    }

    stat.Output.Append("Inv. 1 vol: ").Append(min.CurrentVolume.ToString("F6")).Append("; ");
    stat.Output.Append("Inv. 2 vol: ").Append(max.CurrentVolume.ToString("F6")).Append("; ");
    stat.Output.Append("To move: ").Append(toMove.ToString("F6")).AppendLine();

    if (toMove < 0.0)
        throw new InvalidOperationException("Something went wrong with calculations: volumeDiff is " + toMove);

    if (toMove < SmallNumber)
        return;

    MoveVolume(stat, max, min, (VRage.MyFixedPoint)toMove, filter);
}

private VRage.MyFixedPoint MoveVolume(Statistics stat, BlockWrapper from, BlockWrapper to,
    VRage.MyFixedPoint volumeAmountToMove, Func<MyInventoryItem, bool> filter)
{
    if (volumeAmountToMove == 0)
        return volumeAmountToMove;

    if (volumeAmountToMove < 0)
        throw new ArgumentException("Invalid volume amount", "volumeAmount");

    stat.Output.Append("Move ").Append(volumeAmountToMove).Append(" l. from ")
        .Append(from.Block.CustomName).Append(" to ").AppendLine(to.Block.CustomName);

    var itemsFrom = stat.EmptyTempItemList();
    var fromInv = from.GetInventory();
    var destInv = to.GetInventory();
    fromInv.GetItems(itemsFrom, filter);

    ItemInfo data;
    for (int i = itemsFrom.Count - 1; i >= 0; --i)
    {
        MyInventoryItem item = itemsFrom[i];
        if (!TryGetItemInfo(stat, item.Type, out data))
            continue;

        var amountToMoveRaw = (double)volumeAmountToMove;
        amountToMoveRaw /= data.Volume;
        if (!data.UsesFractions)
            amountToMoveRaw = Math.Floor(amountToMoveRaw + 0.1);
        var amountToMove = (VRage.MyFixedPoint)amountToMoveRaw;
        if (amountToMove > item.Amount)
            amountToMove = item.Amount;
        if (amountToMove <= VRage.MyFixedPoint.Zero)
            continue;

        var itemVolume = (double)amountToMove * data.Volume;
        bool success = fromInv.TransferItemTo(destInv, item, amountToMove);
        stat.MovementsDone += 1;
        stat.Output.Append("Move ").Append(amountToMove).Append(" -> ").AppendLine(success ? "success" : "failure");

        if (success)
            volumeAmountToMove -= (VRage.MyFixedPoint)itemVolume;
        if (volumeAmountToMove < (VRage.MyFixedPoint)SmallNumber)
            return volumeAmountToMove;
    }

    stat.Output.Append("Cannot move ").Append(volumeAmountToMove).AppendLine(" l.");
    return volumeAmountToMove;
}

private bool TryGetItemInfo(Statistics stat, MyItemType key, out ItemInfo data)
{
    if (!_itemInfo.TryGetValue(key, out data))
    {
        data = new ItemInfo(key.GetItemInfo());
        _itemInfo.Add(key, data);
    }

    if (!data.IsValid)
    {
        stat.MissingInfo.Add(key.ToString());
        stat.Output.Append("Volume to amount ratio for ").Append(key).AppendLine(" is not known.");
        return false;
    }
    return true;
}

private void EnforceItemPriority(List<BlockWrapper> group, Statistics stat, MyItemType? topPriority, MyItemType? lowestPriority)
{
    if (topPriority == null && lowestPriority == null)
        return;

    foreach (var wrp in group)
    {
        var inv = wrp.GetInventory();
        if (inv.ItemCount < 2)
            continue;

        var items = stat.EmptyTempItemList();
        inv.GetItems(items, null);

        if (topPriority.HasValue && !items[0].Type.Equals(topPriority.Value))
        {
            for (int i = 1; i < items.Count; ++i)
            {
                var item = items[i];
                if (item.Type.Equals(topPriority.Value))
                {
                    stat.Output.Append("Moving ").Append(topPriority.Value.SubtypeId).Append(" from ")
                        .Append(i + 1).Append(" slot to first slot of ").AppendLine(wrp.Block.CustomName);
                    inv.TransferItemTo(inv, i, 0, false, null);
                    stat.MovementsDone += 1;
                    break;
                }
            }
        }

        if (lowestPriority.HasValue && !items[items.Count - 1].Type.Equals(lowestPriority.Value))
        {
            for (int i = items.Count - 2; i >= 0; --i)
            {
                var item = items[i];
                if (item.Type.Equals(lowestPriority.Value))
                {
                    stat.Output.Append("Moving ").Append(lowestPriority.Value.SubtypeId).Append(" from ")
                        .Append(i + 1).Append(" slot to last slot of ").AppendLine(wrp.Block.CustomName);
                    inv.TransferItemTo(inv, i, items.Count, false, null);
                    stat.MovementsDone += 1;
                    break;
                }
            }
        }
    }
}

private void ProcessDrillsLights(List<BlockWrapper> drills, List<IMyLightingBlock> lights, Statistics stat)
{
    VRage.MyFixedPoint warningLevelInCubicMetersLeft = 5;

    if (lights.Count == 0)
    {
        stat.Output.AppendLine("Setting color of drills payload indicators. Not enough lights found. Nothing to do.");
        return;
    }

    stat.Output.AppendLine("Setting color of drills payload indicators.");

    Color color;
    if (stat.NotConnectedDrillsFound)
    {
        stat.Output.AppendLine("Not all drills are connected.");
        color = step4();
    }
    else
    {
        var drillsMaxVolume = VRage.MyFixedPoint.Zero;
        var drillsCurrentVolume = VRage.MyFixedPoint.Zero;
        foreach (var drill in drills)
        {
            var inv = drill.GetInventory();
            drillsMaxVolume += inv.MaxVolume;
            drillsCurrentVolume += inv.CurrentVolume;
        }

        if (drillsMaxVolume > 0)
        {
            var p = (float)drillsCurrentVolume * 1000.0f;
            p /= (float)drillsMaxVolume;

            stat.DrillsPayloadStr = (p / 10.0f).ToString("F1");

            stat.Output.Append("Drills space usage: ");
            stat.Output.Append(stat.DrillsPayloadStr);
            stat.Output.AppendLine("%");

            stat.DrillsVolumeWarning = (drillsMaxVolume - drillsCurrentVolume) < warningLevelInCubicMetersLeft;
            if (stat.DrillsVolumeWarning)
            {
                color = step3();
            }
            else
            {
                Color c1, c2;
                float m1, m2;

                if (p < 500.0f)
                {
                    c1 = step0();
                    c2 = step1();
                    m2 = p / 500.0f;
                    m1 = 1.0f - m2;
                }
                else
                {
                    c1 = step2();
                    c2 = step1();
                    m1 = (p - 500.0f) / 450.0f;
                    if (m1 > 1.0f)
                        m1 = 1.0f;
                    m2 = 1.0f - m1;
                }

                float r = c1.R * m1 + c2.R * m2;
                float g = c1.G * m1 + c2.G * m2;
                float b = c1.B * m1 + c2.B * m2;

                if (r > 255.0f)
                    r = 255.0f;
                else if (r < 0.0f)
                    r = 0.0f;

                if (g > 255.0f)
                    g = 255.0f;
                else if (g < 0.0f)
                    g = 0.0f;

                if (b > 255.0f)
                    b = 255.0f;
                else if (b < 0.0f)
                    b = 0.0f;

                color = new Color((int)r, (int)g, (int)b);
            }
        }
        else
        {
            color = step0();
        }
    }

    stat.Output.Append("Drills payload indicators lights color: ").Append(color).AppendLine();

    foreach (IMyLightingBlock light in lights)
    {
        var currentColor = light.GetValue<Color>("Color");
        if (currentColor != color)
            light.SetValue<Color>("Color", color);
    }

    stat.Output.Append("Color of ").Append(lights.Count).AppendLine(" drills payload indicators has been set.");
}

private Color step0() => new Color(255, 255, 255);

private Color step1() => new Color(255, 255, 0);

private Color step2() => new Color(255, 0, 0);

private Color step3() => (_cycleNumber & 0x1) == 0 ? new Color(128, 0, 128) : new Color(128, 0, 64);

private Color step4() => (_cycleNumber & 0x1) == 0 ? new Color(0, 128, 128) : new Color(0, 64, 128);

private void PrintOnlineStatus(BlockStore bs, Statistics stat)
{
    var sb = new StringBuilder(4096);
    if (bs == null)
    {
        sb.AppendLine("Initialization in progress...");
    }
    else
    {
        var blocksAffected = bs.Reactors.Count
            + bs.Refineries.Count
            + bs.Drills.Count
            + bs.Turrets.Count
            + bs.OxygenGenerators.Count
            + bs.CargoContainers.Count;

        sb.Append("Grids connected: ").Append(bs.AllGrids.Count)
            .Append(AnyConstruct ? " (AC" : " (SC").AppendLine(stat.DiscoveryDone ? "+)" : ")");
        sb.Append("Conveyor networks: ").Append(stat.NumberOfNetworks).AppendLine();
        sb.Append("Blocks affected: ").Append(blocksAffected).AppendLine();

        sb.Append("reactors: ");
        if (bs.Reactors.Count != 0)
            sb.Append(bs.Reactors.Count);
        else
            sb.Append(EnableReactors ? "0" : "OFF");

        sb.Append(", refineries: ");
        if (bs.Refineries.Count != 0)
            sb.Append(bs.Refineries.Count);
        else
            sb.Append(EnableRefineries ? "0" : "OFF");

        sb.AppendLine().Append("drills: ");
        if (bs.Drills.Count != 0)
            sb.Append(bs.Drills.Count);
        else
            sb.Append(EnableDrills ? "0" : "OFF");

        sb.Append(", turrets: ");
        if (bs.Turrets.Count != 0)
            sb.Append(bs.Turrets.Count);
        else
            sb.Append(EnableTurrets ? "0" : "OFF");

        sb.Append(", o. gen.: ");
        if (bs.OxygenGenerators.Count != 0)
            sb.Append(bs.OxygenGenerators.Count);
        else
            sb.Append(EnableOxygenGenerators ? "0" : "OFF");

        sb.AppendLine().Append("cargo cont.: ");
        if (bs.CargoContainers.Count != 0)
            sb.Append(bs.CargoContainers.Count);
        else
            sb.Append(EnableGroups ? "0" : "OFF");

        sb.Append(", groups: ");
        if (bs.Groups.Count != 0)
            sb.Append(bs.Groups.Count);
        else
            sb.Append(EnableGroups ? "0" : "OFF");

        sb.AppendLine();

        if (bs.Drills.Count != 0)
        {
            if (stat.NotConnectedDrillsFound)
                sb.AppendLine("Warn: Some drills are not connected");

            sb.Append("Drills payload: ").Append(stat.DrillsPayloadStr ?? "N/A");
            if (stat.DrillsVolumeWarning)
                sb.AppendLine((_cycleNumber & 0x01) == 0 ? "%  !" : "% ! !");
            else
                sb.AppendLine("%");
        }

        if (stat.MissingInfo.Count > 0)
            sb.Append("Err: missing volume information for ").AppendLine(String.Join(", ", stat.MissingInfo));

        _avgMovements[_cycleNumber & 0x07] = stat.MovementsDone;
        var samples = Math.Min(_cycleNumber + 1, 0x08);
        double avg = 0;
        for (int i = 0; i < samples; ++i)
            avg += _avgMovements[i];
        avg /= samples;

        sb.Append("Avg. movements: ").Append(avg.ToString("F2")).Append(" (last ").Append(samples).AppendLine(" runs)");
    }

    float cpu = Runtime.CurrentInstructionCount * 100;
    cpu /= Runtime.MaxInstructionCount;
    sb.Append("Complexity limit usage: ").Append(cpu.ToString("F2")).AppendLine("%");

    sb.Append("Prev. run time: ").Append(Runtime.LastRunTimeMs.ToString("F3")).AppendLine(" ms");

    const string bar = "··|· ···· ···· ··|· ···· ···· ";
    var pos = _cycleNumber++ % bar.Length;
    sb.Append(bar.Substring(pos)).AppendLine(bar.Remove(pos));
    Echo(sb.ToString());
}

private void WriteOutput(BlockStore bs, Statistics stat)
{
    const int linesPerDebugScreen = 17;

    if (bs.DebugScreen.Count == 0)
        return;

    bs.DebugScreen.Sort(new MyTextPanelNameComparer());
    string[] lines = stat.Output.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

    for (int i = 0; i < bs.DebugScreen.Count; ++i)
    {
        var screen = (IMyTextSurface)bs.DebugScreen[i];
        var sb = new StringBuilder();
        int firstLine = i * linesPerDebugScreen;
        for (int j = 0; j < linesPerDebugScreen && firstLine + j < lines.Length; ++j)
            sb.AppendLine(lines[firstLine + j].Trim());
        screen.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        screen.WriteText(sb);
    }
}

internal sealed class BlockStore
{
    public readonly Program _program;
    public readonly HashSet<IMyCubeGrid> AllGrids;
    public readonly HashSet<BlockWrapper> AllGroupedInventories;
    public readonly List<BlockWrapper> CargoContainers;
    public readonly List<IMyTextPanel> DebugScreen;
    public readonly List<BlockWrapper> Drills;
    public readonly List<IMyLightingBlock> DrillsPayloadLights;
    public readonly Dictionary<string, HashSet<BlockWrapper>> Groups;
    public readonly List<BlockWrapper> OxygenGenerators;
    public readonly List<BlockWrapper> Reactors;
    public readonly List<BlockWrapper> Refineries;
    public readonly List<BlockWrapper> Turrets;
    private System.Text.RegularExpressions.Regex _groupTagPattern;

    public BlockStore(Program program)
    {
        _program = program;
        DebugScreen = new List<IMyTextPanel>();
        Reactors = new List<BlockWrapper>();
        OxygenGenerators = new List<BlockWrapper>();
        Refineries = new List<BlockWrapper>();
        Drills = new List<BlockWrapper>();
        Turrets = new List<BlockWrapper>();
        CargoContainers = new List<BlockWrapper>();
        Groups = new Dictionary<string, HashSet<BlockWrapper>>();
        AllGroupedInventories = new HashSet<BlockWrapper>();
        AllGrids = new HashSet<IMyCubeGrid>();
        DrillsPayloadLights = new List<IMyLightingBlock>();
    }

    public bool Collect(IMyTerminalBlock block)
    {
        return CollectContainer(block as IMyCargoContainer)
            || CollectRefinery(block as IMyRefinery)
            || CollectReactor(block as IMyReactor)
            || CollectDrill(block as IMyShipDrill)
            || CollectTurret(block as IMyUserControllableGun)
            || CollectOxygenGenerator(block as IMyGasGenerator);
    }

    public void RefreshReferences(Statistics stat)
    {
        AllGrids.Clear();
        AllGroupedInventories.Clear();

        Refresh(Reactors);
        Refresh(Refineries);
        Refresh(Turrets);
        Refresh(Drills);
        Refresh(OxygenGenerators);
        Refresh(CargoContainers);
        Refresh(DrillsPayloadLights);
        Refresh(DebugScreen);

        foreach (var gr in Groups.Values)
        {
            var list = gr.ToList();
            Refresh(list);
            gr.Clear();
            foreach (var dt in list)
            {
                gr.Add(dt);
                AllGroupedInventories.Add(dt);
            }
        }
    }

    private void AddToGroup(BlockWrapper inv)
    {
        const string MatchNothingExpr = @"a^";

        if (_groupTagPattern == null)
        {
            var expr = String.IsNullOrEmpty(_program.GroupTagPattern) ? MatchNothingExpr : _program.GroupTagPattern;
            _groupTagPattern = new System.Text.RegularExpressions.Regex(expr,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        foreach (System.Text.RegularExpressions.Match dt in _groupTagPattern.Matches(inv.Block.CustomName))
        {
            HashSet<BlockWrapper> tmp;
            if (!Groups.TryGetValue(dt.Value, out tmp))
            {
                tmp = new HashSet<BlockWrapper>();
                Groups.Add(dt.Value, tmp);
            }
            tmp.Add(inv);
            AllGroupedInventories.Add(inv);
        }
    }

    private bool CollectContainer(IMyCargoContainer myCargoContainer)
    {
        if (myCargoContainer == null)
            return false;

        if (!_program.EnableGroups)
            return true;

        var wrp = InvWrp(myCargoContainer, myCargoContainer.GetInventory());
        if (wrp != null)
        {
            CargoContainers.Add(wrp);
            AllGrids.Add(myCargoContainer.CubeGrid);
            AddToGroup(wrp);
        }
        return true;
    }

    private bool CollectDrill(IMyShipDrill myDrill)
    {
        if (myDrill == null)
            return false;

        if (!_program.EnableDrills || !myDrill.UseConveyorSystem)
            return true;

        var wrp = InvWrp(myDrill, myDrill.GetInventory());
        if (wrp != null)
        {
            Drills.Add(wrp);
            AllGrids.Add(myDrill.CubeGrid);
            if (_program.EnableGroups)
                AddToGroup(wrp);
        }
        return true;
    }

    private bool CollectOxygenGenerator(IMyGasGenerator myOxygenGenerator)
    {
        if (myOxygenGenerator == null)
            return false;

        if (!_program.EnableOxygenGenerators)
            return true;

        var wrp = InvWrp(myOxygenGenerator, myOxygenGenerator.GetInventory());
        if (wrp != null)
        {
            OxygenGenerators.Add(wrp);
            AllGrids.Add(myOxygenGenerator.CubeGrid);
            if (_program.EnableGroups)
                AddToGroup(wrp);
        }
        return true;
    }

    private bool CollectReactor(IMyReactor myReactor)
    {
        if (myReactor == null)
            return false;

        if (!_program.EnableReactors || !myReactor.UseConveyorSystem)
            return true;

        var wrp = InvWrp(myReactor, myReactor.GetInventory());
        if (wrp != null)
        {
            Reactors.Add(wrp);
            AllGrids.Add(myReactor.CubeGrid);
            if (_program.EnableGroups)
                AddToGroup(wrp);
        }
        return true;
    }

    private bool CollectRefinery(IMyRefinery myRefinery)
    {
        if (myRefinery == null)
            return false;

        if (!_program.EnableRefineries || !myRefinery.UseConveyorSystem)
            return true;

        var wrp = InvWrp(myRefinery, myRefinery.InputInventory);
        if (wrp != null)
        {
            Refineries.Add(wrp);
            AllGrids.Add(myRefinery.CubeGrid);
            if (_program.EnableGroups)
                AddToGroup(wrp);
        }
        return true;
    }

    private bool CollectTurret(IMyUserControllableGun myTurret)
    {
        if (myTurret == null)
            return false;

        if (!_program.EnableTurrets || myTurret is IMyLargeInteriorTurret)
            return true;

        var wrp = InvWrp(myTurret, myTurret.GetInventory());
        if (wrp != null)
        {
            Turrets.Add(wrp);
            AllGrids.Add(myTurret.CubeGrid);
            if (_program.EnableGroups)
                AddToGroup(wrp);
        }
        return true;
    }

    private BlockWrapper InvWrp(IMyTerminalBlock block, IMyInventory inv)
    {
        if (inv != null && inv.MaxVolume > 0)
        {
            var accepted = FindAcceptedItems(block.BlockDefinition, inv);
            if (accepted.Count > 0)
                return new BlockWrapper(block, accepted);
        }
        return null;
    }

    private BlockWrapper InvWrp(IMyProductionBlock block, IMyInventory inv)
    {
        if (inv != null && inv.MaxVolume > 0)
        {
            var accepted = FindAcceptedItems(block.BlockDefinition, inv);
            if (accepted.Count > 0)
                return new BlockWrapperProd(block, accepted);
        }
        return null;
    }

    private List<MyItemType> FindAcceptedItems(MyDefinitionId def, IMyInventory inv)
    {
        var blockMap = _program._blockMap;
        List<MyItemType> result;
        if (blockMap.TryGetValue(def, out result))
            return result;

        result = new List<MyItemType>();
        inv.GetAcceptedItems(result);
        result.Sort((a, b) => String.Compare(a.ToString(), b.ToString(), StringComparison.Ordinal));

        foreach (var list in blockMap.Values)
        {
            if (Enumerable.SequenceEqual(result, list))
            {
                blockMap[def] = list;
                return list;
            }
        }

        blockMap[def] = result;
        return result;
    }

    private void Refresh(List<BlockWrapper> list)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            var wrp = list[i];
            var block = _program.GridTerminalSystem.GetBlockWithId(wrp.Block.EntityId);
            if (block == null)
            {
                list.RemoveAt(i--);
            }
            else
            {
                AllGrids.Add(block.CubeGrid);
                list[i] = wrp.CopyFor(block);
            }
        }
    }

    private void Refresh<TBlock>(List<TBlock> list)
        where TBlock : class, IMyTerminalBlock
    {
        for (int i = 0; i < list.Count; ++i)
        {
            var old = list[i];
            var block = _program.GridTerminalSystem.GetBlockWithId(old.EntityId) as TBlock;
            if (block == null)
            {
                list.RemoveAt(i--);
            }
            else
            {
                AllGrids.Add(block.CubeGrid);
                list[i] = block;
            }
        }
    }
}

internal class BlockWrapper
{
    public readonly List<MyItemType> AcceptedItems;
    public readonly IMyTerminalBlock Block;
    public double CurrentVolume;
    public double MaxVolume;
    public double Percent;

    public BlockWrapper(IMyTerminalBlock block, List<MyItemType> acceptedItems)
    {
        Block = block;
        AcceptedItems = acceptedItems;
    }

    public virtual BlockWrapper CopyFor(IMyTerminalBlock block)
    {
        return new BlockWrapper(block, this.AcceptedItems);
    }

    public virtual IMyInventory GetInventory()
    {
        return Block.GetInventory();
    }

    public void LoadVolume(Program prog, Statistics stat, Func<MyInventoryItem, bool> filter)
    {
        var inv = GetInventory();
        CurrentVolume = (double)inv.CurrentVolume;
        MaxVolume = (double)inv.MaxVolume;

        if (filter != null)
        {
            double volumeBlocked = 0.0;
            inv.GetItems(null, item =>
            {
                if (!filter(item))
                {
                    ItemInfo data;
                    if (prog.TryGetItemInfo(stat, item.Type, out data))
                        volumeBlocked += (double)item.Amount * data.Volume;
                }
                return false;
            });

            if (volumeBlocked > 0.0)
            {
                CurrentVolume -= volumeBlocked;
                MaxVolume -= volumeBlocked;
                stat.Output.Append("volumeBlocked ").AppendLine(volumeBlocked.ToString("N6"));
            }
        }

        Percent = CurrentVolume / MaxVolume;
    }

    public bool MoveItem(int sourceItemIndex, int targetItemIndex)
    {
        var inv = GetInventory();
        return inv.TransferItemTo(inv, sourceItemIndex, targetItemIndex, false, null);
    }
}

internal sealed class BlockWrapperProd : BlockWrapper
{
    public BlockWrapperProd(IMyProductionBlock block, List<MyItemType> acceptedItems)
        : base(block, acceptedItems)
    { }

    public override BlockWrapper CopyFor(IMyTerminalBlock block)
    {
        return new BlockWrapperProd((IMyProductionBlock)block, this.AcceptedItems);
    }

    public override IMyInventory GetInventory()
    {
        return ((IMyProductionBlock)Block).InputInventory;
    }
}

internal sealed class InventoryGroup
{
    public List<BlockWrapper> Blocks;
    public string Name;
    public int No;

    public InventoryGroup(int no, string name)
    {
        No = no;
        Name = name;
        Blocks = new List<BlockWrapper>();
    }
}

internal sealed class ItemInfo
{
    public readonly double Volume;
    public readonly bool IsValid, UsesFractions, CanStack;

    public ItemInfo(MyItemInfo info)
    {
        Volume = info.Volume;
        IsValid = Volume >= SmallNumber;
        UsesFractions = info.UsesFractions;
        CanStack = info.IsOre || info.IsIngot || info.IsComponent || info.IsAmmo;
    }
}

internal sealed class MyTextPanelNameComparer : IComparer<IMyTextPanel>
{
    public int Compare(IMyTextPanel x, IMyTextPanel y)
    {
        return String.Compare(x.CustomName, y.CustomName, true);
    }
}

internal sealed class Statistics
{
    public readonly List<MyInventoryItem> _tmpItems;
    public readonly HashSet<string> MissingInfo;
    public readonly StringBuilder Output;
    public string DrillsPayloadStr;
    public bool NotConnectedDrillsFound, DrillsVolumeWarning, DiscoveryDone;
    public int NumberOfNetworks, MovementsDone;

    public Statistics()
    {
        Output = new StringBuilder();
        MissingInfo = new HashSet<string>();
        _tmpItems = new List<MyInventoryItem>();
    }

    public List<MyInventoryItem> EmptyTempItemList()
    {
        _tmpItems.Clear();
        return _tmpItems;
    }
}
