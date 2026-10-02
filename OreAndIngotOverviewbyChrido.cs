IMyTextPanel _lcdPanelOre = null;
IMyTextPanel _lcdPanelIngots = null;
Dictionary<string, VRage.MyFixedPoint> _ore = new Dictionary<string, VRage.MyFixedPoint>();
Dictionary<string, VRage.MyFixedPoint> _ingots = new Dictionary<string, VRage.MyFixedPoint>();

List<IMyInventory> _inventories = new List<IMyInventory>();

StringBuilder _statsSB = new StringBuilder();
StringBuilder _outputText = new StringBuilder();

IEnumerator<bool> _init;
IEnumerator<bool> _work;

bool _firstRun = true;
string _currentStep = "";
string _lastStep = "";

string formatAmount(VRage.MyFixedPoint value)
{
    if (value >= 10000000) // 10.0M  
    {
        value = value * (VRage.MyFixedPoint)0.00001;
        value = VRage.MyFixedPoint.Floor(value);
        value = value * (VRage.MyFixedPoint)0.1;
        return value.ToString() + "M";
    }
    else if (value >= 1000000) // 1.00M 
    {
        value = value * (VRage.MyFixedPoint)0.0001;
        value = VRage.MyFixedPoint.Floor(value);
        value = value * (VRage.MyFixedPoint)0.01;
        return value.ToString() + "M";
    }
    else if (value >= 100000) // 100k  
    {
        value = value * (VRage.MyFixedPoint)0.001;
        value = VRage.MyFixedPoint.Floor(value);
        return value.ToString() + "k";
    }
    else if (value >= 10000) // 10.0k 
    {
        value = value * (VRage.MyFixedPoint)0.01;
        value = VRage.MyFixedPoint.Floor(value);
        value = value * (VRage.MyFixedPoint)0.1;
        return value.ToString() + "k";
    }
    else if (value >= 1000) // 1.00k 
    {
        value = value * (VRage.MyFixedPoint)0.1;
        value = VRage.MyFixedPoint.Floor(value);
        value = value * (VRage.MyFixedPoint)0.01;
        return value.ToString() + "k";
    }
    else if (value >= 100)  // 100 
    {
        return VRage.MyFixedPoint.Floor(value).ToString();
    }
    else if (value >= 10) // 10.0  
    {
        value = value * (VRage.MyFixedPoint)10.0;
        value = VRage.MyFixedPoint.Floor(value);
        value = value * (VRage.MyFixedPoint)0.1;
        return value.ToString();
    }
    else // 1.00 
    {
        value = value * (VRage.MyFixedPoint)100.0;
        value = VRage.MyFixedPoint.Floor(value);
        value = value * (VRage.MyFixedPoint)0.01;
        return value.ToString();
    }
}

public void initOreCount()
{
    _ore["Cobalt"] = 0;
    _ore["Gold"] = 0;
    _ore["Iron"] = 0;
    _ore["Magnesium"] = 0;
    _ore["Nickel"] = 0;
    _ore["Platinum"] = 0;
    _ore["Silicon"] = 0;
    _ore["Silver"] = 0;
    _ore["Stone"] = 0;
    _ore["Uranium"] = 0;
    _ingots["Cobalt"] = 0;
    _ingots["Gold"] = 0;
    _ingots["Iron"] = 0;
    _ingots["Magnesium"] = 0;
    _ingots["Nickel"] = 0;
    _ingots["Platinum"] = 0;
    _ingots["Silicon"] = 0;
    _ingots["Silver"] = 0;
    _ingots["Stone"] = 0;
    _ingots["Uranium"] = 0;
}

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Once;
    Echo("Initializing...");
    _init = Setup();
}

void checkInventory(List<MyInventoryItem> inventoryItems)
{
    foreach (MyInventoryItem item in inventoryItems)
    {
        if (item.Type.TypeId =="MyObjectBuilder_Ore")
        {
            string oreType = item.Type.SubtypeId;
            if (_ore.ContainsKey(oreType))
            {
                _ore[oreType] += item.Amount;
            }

        }
        else if (item.Type.TypeId  == "MyObjectBuilder_Ingot")
        {
            string ingotType = item.Type.SubtypeId;
            if (_ingots.ContainsKey(ingotType))
            {
                _ingots[ingotType] += item.Amount;
            }
        }
    }
}

void ShowStats()
{
    _statsSB.Clear()
      .Append("--- Ore & Ingots Overview ---\n")
      .Append("                   by Chrido\n\n")
      .Append($"Last Step: {_lastStep}\n")
      .Append($"Last Runtime: {Runtime.LastRunTimeMs.ToString("0.0000")} ms\n");
    Echo(_statsSB.ToString());
    _lastStep = _currentStep;
}

public IEnumerator<bool> Setup()
{
    _currentStep = "Setup";
    // Runtime.UpdateFrequency = UpdateFrequency.Update1 | UpdateFrequency.Update10 | UpdateFrequency.Update100;
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    _firstRun = false;

    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(blocks);

    foreach (IMyTerminalBlock block in blocks)
    {
        for (int i = 0; i < block.InventoryCount; i++)
        {
            _inventories.Add(block.GetInventory(i));
        }
    }

    _lcdPanelOre = GridTerminalSystem.GetBlockWithName("LCD Ore") as IMyTextPanel;
    _lcdPanelIngots = GridTerminalSystem.GetBlockWithName("LCD Ingots") as IMyTextPanel;

    _work = Work();

    yield return false;
}

public IEnumerator<bool> Work()
{
    _currentStep = "ClearCount";
    initOreCount();
    yield return true;

    _currentStep = "FetchItems";
    List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();
    foreach (var inventory in _inventories)
    {
        inventory.GetItems(inventoryItems);
    }
    yield return true;

    _currentStep = "CountItems";
    checkInventory(inventoryItems);
    yield return true;

    _currentStep = "UpdateLCDs";
    _outputText.Clear()
      .Append($" Cobalt Ore             { formatAmount(_ore["Cobalt"]) }\n")
      .Append($" Gold Ore                { formatAmount(_ore["Gold"]) }\n")
      .Append($" Iron Ore                 { formatAmount(_ore["Iron"]) }\n")
      .Append($" Magnesium Ore    { formatAmount(_ore["Magnesium"]) }\n")
      .Append($" Nickel Ore             { formatAmount(_ore["Nickel"]) }\n")
      .Append($" Platinum Ore         { formatAmount(_ore["Platinum"]) }\n")
      .Append($" Silicon Ore            { formatAmount(_ore["Silicon"]) }\n")
      .Append($" Silver Ore              { formatAmount(_ore["Silver"]) }\n")
      .Append($" Stone                     { formatAmount(_ore["Stone"]) }\n")
      .Append($" Uranium Ore          { formatAmount(_ore["Uranium"]) }\n");
    _lcdPanelOre.WriteText(_outputText.ToString());

    _outputText.Clear()
      .Append($" Cobalt              { formatAmount(_ingots["Cobalt"]) }\n")
      .Append($" Gold                 { formatAmount(_ingots["Gold"]) }\n")
      .Append($" Iron                  { formatAmount(_ingots["Iron"]) }\n")
      .Append($" Magnesium     { formatAmount(_ingots["Magnesium"]) }\n")
      .Append($" Nickel              { formatAmount(_ingots["Nickel"]) }\n")
      .Append($" Platinum          { formatAmount(_ingots["Platinum"]) }\n")
      .Append($" Silicon             { formatAmount(_ingots["Silicon"]) }\n")
      .Append($" Silver               { formatAmount(_ingots["Silver"]) }\n")
      .Append($" Gravel              { formatAmount(_ingots["Stone"]) }\n")
      .Append($" Uranium           { formatAmount(_ingots["Uranium"]) }\n");
    _lcdPanelIngots.WriteText(_outputText.ToString());
}

public void SubMain(string argument, UpdateType updateSource)
{
    _currentStep = "Restart";

    if (_init != null)
    {
        if (!_init.MoveNext())
        {
            _init.Dispose();
            _init = null;
        }
        return;
    }

    if (_work.MoveNext() == false)
    {
        _work.Dispose();
        _work = Work();
    }

    ShowStats();
}

public void Main(string argument, UpdateType updateSource)
{
    if ((!_firstRun && Runtime.TimeSinceLastRun.TotalSeconds == 0) || updateSource == UpdateType.Mod)
        return;
    try
    {
        SubMain(argument, updateSource);
    }
    catch (Exception e)
    {
        StringBuilder dbg = new StringBuilder();
        dbg.Append("Exception Message:\n")
          .Append($"{e.Message}\n\n")
          .Append("Stack trace:\n")
          .Append($"{e.StackTrace}\n");
        Echo(dbg.ToString());
        throw;
    }
}