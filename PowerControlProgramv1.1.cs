/*
 * Power Control Program v1.13
 * 
 * ------------------- INSTRUCTIONS -------------------
 * NOTE: If you have built new blocks or you removed blocks controled by the script, you have to recompile the script.
 * 
 * To use a LCD Display for showing information just write
 * @PowerControl
 * into its CustomData field. Do the same at Programmable Blocks or Cockpits, when you want to use their displays.
 * After closing the menu and enter the Custom Data field again you see the configuration for the displays.
 * 
 * If you want to use the control script to activate, switch or deaktivate single blocks, you need a screen showing the
 * mainScreen, because this is your user interface, and a button panel or the G-Menu of a seat to send arguments to the progr. block.
 * Next you need to setup the buttons or G-Menu selecting the programmable block with the script and choose the Run-Argument.
 * Now use the following commands with the Run-Argument.
 * 
 * Use button 1 to switch the screens with the instruction "menu=", following one of the numbers:
 * 1 - instructions menu, 2 - battery menu, 3 - solar menu, 4 - wind turbine menu, 5 - hydrogen egine menu, 6 - reactor menu
 * 
 * Further you have following instructions:
 * selection=1 - select next object
 * selection=-1 - select previous object
 * selection=100 - aktivate selected object
 * selection=-100 - deaktivate selected object
 * selection=0 - switch selected object mode (only for batteries and reactors)
 * changeStatus=BatteryBlock - switch mode of all batteries
 * changeStatus=SolarAlignment - switches solar alignment modus on or off
 * changeStatus=manualControl - activate or deactivate automatic power control
 * changeStatus=switchUseAll - switch between all backup devices are switched on at once and switched on step by step
 * changeStatus=uranium - select reactors as first energy backup device
 * changeStatus=hydrogen - select H2-Engines as first energy backup device
 * changeStatus=lowerBattery- - lower the percentage of batteries charge, when the backup device is switched on
 * changeStatus=lowerBattery+ - raise the percentage of batteries charge, when the backup device is switched on
 * changeStatus=upperBattery- - lower the percentage of batteries charge, when the backup device is switched off
 * changeStatus=upperBattery+ - raise the percentage of batteries charge, when the backup device is switched off
 * 
 * If you want to use the integrated solar panel alignment script, you have to name your rotors and the attached panels in a specific way.
 * All rotors and panels of one solar mast have to have the same name as part of the block names to match them. Enter this name into the
 * parameter section below. The rotors have to be named with its orientation, too.
 * The rotor to turn the solar panels have to be named "Rotation", the rotors for changing the angle have to be named "Angle".
 * If you build two rotors for changing the angle, e.g. for left and right panels of the center pylon, then name on of them
 * "Angle" and the other "Angle Inv". Don't forget to put he grid name into these names too. So a example of such a name could be:
 * "GridName MastName Rotor Rotate" and "GridName MastName Solar Panel".
 * If you are using other solar alignment scripts enter the name of its Programmable Block below.
 */


// ------------------- SET PARAMETERS -------------------

// Language: english: en = true, german: en = false
static readonly bool en = true;

// enter a specific part of the blocks name, only blocks with this token in there names are controlled by the script
// use this, if you want to seperate different grids
// e.g.: const string gridName = "Base";
const string gridName = "";

// if you want to use the included alignment script enter the names of the solar masts below
// this name token have to be part of the name of the rotors and of the connected solar panels
// e.g.: static string[] solarMastNames = { "front", "left", right" };
static string[] solarMastNames = { };

// if you want to use other solar alignment scripts enter the name of the programmable block with your alignment script below
// e.g.: static string extAlignProgBlockName = "Base SolarScript Programmable Block";
static readonly string extAlignProgBlockName = "";

// depth of the output diagram on the screens
const int outputDiagramDepth = 100;

// ----- DON'T CHANGE ANYTHING BELOW THIS LINE ------









const string programToken = "@PowerControl";
static readonly string[] menuTitles = { "ERROR", "Program Control", "Battery Control", "Solar Control", "Wind Turbine Control", "H2 Engine Control", "Reactor Control", "", "", "" };
static readonly string[] tab0 = { "Error:", "no Toolbar selected!", "please use toolbar Button 1", "to show the correct instructions" };
static readonly string[] tab1 = { "show instruction screen", "turn auto. power control on/off", "activate all at once/step by step", "set uranium as first backup", "set hydrogen as first backup", "decrease lower battery limit", "increase lower battery limit", "decrease upper battery limit", "increase upper battery limit" };
static readonly string[] tab2 = { "show instruction screen", "select previous item", "activate selected item", "switch selected item mode", "deactivate selected item", "select next item", "switch all", "activate all", "deactivate all" };
static readonly string[] tab3 = { "show instruction screen", "select previous item", "activate selected item", "-", "deactivate selected item", "select next item", "switch solar alignment on/off", "activate all", "deactivate all" };
static readonly string[] tab4 = { "show instruction screen", "select previous item", "activate selected item", "-", "deactivate selected item", "select next item", "-", "activate all", "deactivate all" };
static readonly string[] tab5 = { "show instruction screen", "select previous item", "activate selected item", "-", "deactivate selected item", "select next item", "-", "activate all", "deactivate all" };
static readonly string[] tab6 = { "show instruction screen", "select previous item", "activate selected item", "switch automatic refill", "deactivate selected item", "select next item", "switch refill for all", "activate all", "deactivate all" };
static Color colorOn = new Color(130, 130, 130);
static readonly string[] screenTypes = { "mainScreen", "overallStatusScreen", "instructionsScreen", "batteryScreen",
  "solarScreen", "windTurbineScreen", "h2EngineScreen", "reactorScreen", "powerControlScreen", "NONE", "programScreen" };
static List<IMyTerminalBlock> screenBlocks = new List<IMyTerminalBlock>();
static Dictionary<int, string[]> menus = new Dictionary<int, string[]>(10);
static Dictionary<IMyPowerProducer, HydrogenEngine> h2Engines = new Dictionary<IMyPowerProducer, HydrogenEngine>();
static List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
static List<IMySolarPanel> solarPanels = new List<IMySolarPanel>();
static List<IMyReactor> reactors = new List<IMyReactor>();
static List<IMyPowerProducer> windTurbines = new List<IMyPowerProducer>();
static long[] selectedblock = new long[9];
static System.Globalization.CultureInfo region = System.Globalization.CultureInfo.GetCultureInfo("en-US");
static int shownMenu = 1;
const float uraniumMassVol = 0.000052f;
static float usedPower = 0f;
static float producedPower = 0f;
static float maxSolarOutput = 0f;
static float maxSolarOutputPerPanel = 0f;
static float maxReactorOutput = 0f;
static float curReactorIngots = 0f;
static float maxReactorIngots = 0f;
static float maxH2EnginesOutput = 0f;
static float curH2GasVolume = 0f;
static float maxH2GasVolume = 0f;
static float maxBatteryOutput = 0f;
static float curStoredPower = 0f;
static float maxStoredPower = 0f;
static float maxWindTurbineOutput = 0f;
static Queue<float> reactorOutputs = new Queue<float>(outputDiagramDepth);
static Queue<float> reactorMaxOutputs = new Queue<float>(outputDiagramDepth);
static Queue<float> solarOutputs = new Queue<float>(outputDiagramDepth);
static Queue<float> solarMaxOutputs = new Queue<float>(outputDiagramDepth);
static Queue<float> h2EngineOutputs = new Queue<float>(outputDiagramDepth);
static Queue<float> h2EngineMaxOutputs = new Queue<float>(outputDiagramDepth);
static Queue<float> windTurbineOutputs = new Queue<float>(outputDiagramDepth);
static Queue<float> windTurbineMaxOutputs = new Queue<float>(outputDiagramDepth);
static Queue<float> batteryPowerFlow = new Queue<float>(outputDiagramDepth);
static bool firstBackUpReactor = false;
static List<IMyPowerProducer>[] backupBlocks = new List<IMyPowerProducer>[2];
static float lowerBatLimit = 0.2f;
static float upperBatLimit = 0.6f;
static bool useAllBackups = false;
static bool manualControl = true;
static bool charging = false;
static bool alignSolar = false;
static List<SolarMast> solarMasts = new List<SolarMast>(solarMastNames.Length);
static bool useExtAlignment = false;
static IMyProgrammableBlock extAlignBlock;
static StringBuilder programInfo = new StringBuilder();
static string savedValues = "";

Func<float, float, double> percentage = (p, m) =>
{
  if (m > 0)
    return Math.Round((p / m), 6);
  else
    return 0;
};
Func<float, string> wattage = w =>
{
  if (w < 0.9f)
  {
    return Math.Round(w * 1000, 1).ToString("#,0.", region) + " kW";
  }
  else
  {
    return Math.Round(w, 1).ToString("#,0.0", region) + " MW";
  }
};

public Program()
{
  if (!en)
    region = System.Globalization.CultureInfo.GetCultureInfo("de-DE");
  ParseSaving(false);
  List<IMyPowerProducer> allPowerProducer = new List<IMyPowerProducer>();
  GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(allPowerProducer, b => b.CustomName.Contains(gridName));
  List<IMyPowerProducer> engines = new List<IMyPowerProducer>();
  engines = allPowerProducer.Where(b => b.BlockDefinition.SubtypeId.Contains("Hydrogen")).ToList<IMyPowerProducer>();
  engines.ForEach(e => h2Engines.Add(e, new HydrogenEngine(e)));
  h2Engines = h2Engines.OrderBy(h => h.Key.CustomName).ToDictionary(h => h.Key, h => h.Value);
  batteries = allPowerProducer.OfType<IMyBatteryBlock>().ToList<IMyBatteryBlock>();
  batteries = batteries.OrderBy(b => b.CustomName).ToList();
  solarPanels = allPowerProducer.OfType<IMySolarPanel>().ToList<IMySolarPanel>();
  solarPanels = solarPanels.OrderBy(s => s.CustomName).ToList();
  reactors = allPowerProducer.OfType<IMyReactor>().ToList<IMyReactor>();
  reactors = reactors.OrderBy(r => r.CustomName).ToList();
  windTurbines = allPowerProducer.Where(b => b.BlockDefinition.SubtypeId.Contains("Wind")).ToList<IMyPowerProducer>();
  windTurbines = windTurbines.OrderBy(w => w.CustomName).ToList();
  if (firstBackUpReactor)
    BackupBlockList(true);
  else
    BackupBlockList(false);
  menus.Add(0, tab0); menus.Add(1, tab1); menus.Add(2, tab2); menus.Add(3, tab3); menus.Add(4, tab4); menus.Add(5, tab5); menus.Add(6, tab6);
  if (batteries.Any())
    selectedblock[2] = batteries.First().EntityId;
  if (solarPanels.Any())
    selectedblock[3] = solarPanels.First().EntityId;
  if (windTurbines.Any())
    selectedblock[4] = windTurbines.First().EntityId;
  if (h2Engines.Any())
    selectedblock[5] = h2Engines.First().Key.EntityId;
  if (reactors.Any())
    selectedblock[6] = reactors.First().EntityId;

  if (extAlignProgBlockName.Length > 1)
    extAlignBlock = GridTerminalSystem.GetBlockWithName(extAlignProgBlockName) as IMyProgrammableBlock;
  foreach (var mast in solarMastNames)
  {
    var panelsFound = solarPanels.Where(p => p.CustomName.Contains(mast)).ToList();
    List<IMyMotorStator> rotorsFound = new List<IMyMotorStator>();
    GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotorsFound, r => r.CustomName.Contains(mast));
    if (panelsFound.Any() && rotorsFound.Any())
      solarMasts.Add(new SolarMast(mast, panelsFound, rotorsFound));
  }
  Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Save()
{
  foreach (var mast in solarMasts)
    mast.StopAllAlignment();
  Storage = ParseSaving(true);
}

string ParseSaving(bool save)
{
  if (save)
    savedValues =
      alignSolar + "\n" +
      lowerBatLimit + "\n" + upperBatLimit + "\n" +
      manualControl + "\n" + useAllBackups + "\n" + firstBackUpReactor;
  else
  {
    try
    {
      string[] values = Storage.Split('\n');
      if (values.Length >= 6)
      {
        alignSolar = bool.Parse(values[0]);
        lowerBatLimit = float.Parse(values[1]); upperBatLimit = float.Parse(values[2]);
        manualControl = bool.Parse(values[3]); useAllBackups = bool.Parse(values[4]); firstBackUpReactor = bool.Parse(values[5]);
      }
    }
    catch (FormatException e)
    {
      Echo(e + "\nNo saved data loaded");
      return "";
    }
  }
  return savedValues;
}

public void Main(string argument, UpdateType updateSource)
{
  programInfo.Clear();
  Echo("---- Power Control Program ----");

  GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(screenBlocks, b => b.CustomData.Contains(programToken) && b.CustomName.Contains(gridName));
  Dictionary<string, List<IMyTextSurface>> matchedSurfaces;

  InitScreenCustomData(screenBlocks);
  MatchTypesWithScreens(screenBlocks, out matchedSurfaces);
  if (extAlignBlock != null)
    useExtAlignment = true;
  if (lowerBatLimit < 0.1f) lowerBatLimit = 0.1f;
  if (upperBatLimit > 1f) upperBatLimit = 1f;

  Func<float, Queue<float>, float> refreshQueue = (cur, queue) =>
  {
    if (queue.Count >= outputDiagramDepth)
    {
      queue.Dequeue();
    }
    queue.Enqueue(cur);
    return cur;
  };

  if (argument.Any())
  {
    string actionType;
    string action;
    string[] argParts = argument.Split('=', '\n');
    actionType = argParts[0];
    action = argParts[1];
    if (actionType.Equals("menu"))
      shownMenu = int.Parse(action);
    if (actionType.Equals("changeStatus"))
      ToggleBlocksStatus(action);
    if (actionType.Equals("selection"))
      ParseSelection(action);
  }

  h2Engines.Values.ToList().ForEach(h => h.Refresh());

  usedPower = 0f;
  usedPower += refreshQueue(h2Engines.Keys.Where(h => h.IsWorking).Sum(h => h.CurrentOutput), h2EngineOutputs);
  usedPower += refreshQueue(batteries.Where(b => b.IsWorking).Sum(b => b.CurrentOutput - b.CurrentInput), batteryPowerFlow);
  usedPower += refreshQueue(solarPanels.Where(r => r.IsWorking).Sum(r => r.CurrentOutput), solarOutputs);
  usedPower += refreshQueue(reactors.Where(r => r.IsWorking).Sum(r => r.CurrentOutput), reactorOutputs);
  usedPower += refreshQueue(windTurbines.Where(w => w.IsWorking).Sum(w => w.CurrentOutput), windTurbineOutputs);
  producedPower = 0f;
  producedPower += refreshQueue(h2Engines.Keys.Where(h => h.IsWorking).Sum(h => h.MaxOutput), h2EngineMaxOutputs);
  producedPower += refreshQueue(solarPanels.Where(s => s.IsWorking).Sum(s => s.MaxOutput), solarMaxOutputs);
  producedPower += refreshQueue(reactors.Where(r => r.IsWorking).Sum(r => r.MaxOutput), reactorMaxOutputs);
  producedPower += refreshQueue(windTurbines.Where(w => w.IsWorking).Sum(w => w.MaxOutput), windTurbineMaxOutputs);
  maxStoredPower = batteries.Sum(b => b.MaxStoredPower);
  curStoredPower = batteries.Where(b => b.IsWorking).Sum(B => B.CurrentStoredPower);
  if (solarPanels.Any()) CalculateMaxSolarOutput(solarPanels);
  if (reactors.Any()) CalculateMaxReactorOutput(reactors);
  if (windTurbines.Any()) CalculateMaxWindTurbineOutput(windTurbines);
  if (h2Engines.Any()) CalculateMaxHydrogenEnginesOutput(h2Engines.Keys.ToList());
  if (batteries.Any()) CalculateMaxBatteryOutput(batteries);
  curH2GasVolume = 0f; maxH2GasVolume = 0f;
  maxH2GasVolume = h2Engines.Values.Sum(h => h.Capacity);
  curH2GasVolume = h2Engines.Values.Sum(h => h.Capacity * h.FilledRatio);
  curReactorIngots = reactors.Sum(r => (float)r.GetInventory(0).CurrentMass);
  maxReactorIngots = reactors.Sum(r => (float)r.GetInventory(0).MaxVolume / uraniumMassVol);

  if (manualControl)
    ConsoleEcho("\nmanual power control\n");
  else
    ConsoleEcho("\nautomatic power control\n");

  if (solarMasts.Any())
  {
    if (alignSolar)
      solarMasts.ForEach(sm => ConsoleEcho(sm.MastName + ": " + sm.MoveAlongSun()));
    else
      solarMasts.ForEach(sm => ConsoleEcho(sm.MastName + ": " + sm.StopAllAlignment()));
  }
  else
    ConsoleEcho("no solar panel masts found");
  if (useExtAlignment)
  {
    extAlignBlock.Enabled = alignSolar;
    ConsoleEcho(extAlignBlock.CustomName + " online: " + extAlignBlock.Enabled.ToString());
  }
  ConsoleEcho("");
  List<IMyTextSurface> actSurface;
  if (matchedSurfaces.TryGetValue(screenTypes[0], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[0]);
    GenerateMainScreen(actSurface, shownMenu);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[1], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[1]);
    GenerateOverallStatus(actSurface);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[2], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[2]);
    GenerateInstructions(actSurface);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[3], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[3]);
    GenerateBatteryScreen(actSurface);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[4], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[4]);
    GenerateSolarScreen(actSurface);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[5], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[5]);
    GenerateWindTurbineScreen(actSurface);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[6], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[6]);
    GenerateH2EngineScreen(actSurface);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[7], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[7]);
    GenerateReactorScreen(actSurface);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[8], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[8]);
    GenerateMainScreen(actSurface, 1);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[9], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[9]);
    actSurface.Clear();
  }
  if (matchedSurfaces.TryGetValue(screenTypes[10], out actSurface))
  {
    ConsoleEcho(actSurface.Count + "x " + screenTypes[10]);
    GenerateProgramScreen(actSurface);
    actSurface.Clear();
  }

  if (!manualControl)
    CheckBatteryCharging();
}

void ToggleBlocksStatus(string action)
{
  if (action.Equals("BatteryBlock") && batteries.Any())
  {
    int mode;
    batteries.ForEach(b =>
    {
      mode = (int)b.ChargeMode;
      mode++;
      mode = mode % 3;
      b.ChargeMode = (ChargeMode)mode;
    });
  }
  if (action.Equals("SolarAlignment"))
    alignSolar = !alignSolar;
  if (action.Equals("uranium"))
    BackupBlockList(true);
  if (action.Equals("hydrogen"))
    BackupBlockList(false);
  if (action.Equals("lowerBattery-"))
    if (lowerBatLimit >= 0.15f) lowerBatLimit -= 0.05f;
  if (action.Equals("lowerBattery+"))
    if (upperBatLimit - lowerBatLimit >= 0.1f) lowerBatLimit += 0.05f;
  if (action.Equals("upperBattery-"))
    if (upperBatLimit - lowerBatLimit >= 0.1f) upperBatLimit -= 0.05f;
  if (action.Equals("upperBattery+"))
    if (upperBatLimit <= 0.95f) upperBatLimit += 0.05f;
  if (action.Equals("switchUseAll"))
    useAllBackups = !useAllBackups;
  if (action.Equals("manualControl"))
    manualControl = !manualControl;
}

void ParseSelection(string action)
{
  int input = int.Parse(action);
  if (input == 1)
    selectedblock[shownMenu] = GetNextBlock(shownMenu);
  if (input == 0)
    ToggleSelection(shownMenu);
  if (input == -1)
    selectedblock[shownMenu] = GetPreviousBlock(shownMenu);
  if (input == 100)
    EnableBlock(shownMenu, true);
  if (input == -100)
    EnableBlock(shownMenu, false);
}

long GetNextBlock(int menu)
{
  long newSelection = selectedblock[menu];
  List<IMyFunctionalBlock> blocks = new List<IMyFunctionalBlock>();
  if (menu == 2)
    blocks = batteries.Where(b => b is IMyFunctionalBlock).Select(b => b as IMyFunctionalBlock).ToList();
  if (menu == 3)
    blocks = solarPanels.Where(s => s is IMyFunctionalBlock).Select(s => s as IMyFunctionalBlock).ToList();
  if (menu == 4)
    blocks = windTurbines.Where(w => w is IMyFunctionalBlock).Select(w => w as IMyFunctionalBlock).ToList();
  if (menu == 5)
    blocks = h2Engines.Keys.Where(e => e is IMyFunctionalBlock).Select(e => e as IMyFunctionalBlock).ToList();
  if (menu == 6)
    blocks = reactors.Where(r => r is IMyFunctionalBlock).Select(r => r as IMyFunctionalBlock).ToList();
  if (blocks.Any())
  {
    IMyFunctionalBlock nextBlock = blocks.SkipWhile(b => b.EntityId != selectedblock[menu]).Skip(1).DefaultIfEmpty(blocks[0]).FirstOrDefault();
    newSelection = nextBlock.EntityId;
  }
  return newSelection;
}

long GetPreviousBlock(int menu)
{
  long newSelection = selectedblock[menu];
  List<IMyFunctionalBlock> blocks = new List<IMyFunctionalBlock>();
  if (menu == 2)
    blocks = batteries.Where(b => b is IMyFunctionalBlock).Select(b => b as IMyFunctionalBlock).ToList();
  if (menu == 3)
    blocks = solarPanels.Where(s => s is IMyFunctionalBlock).Select(s => s as IMyFunctionalBlock).ToList();
  if (menu == 4)
    blocks = windTurbines.Where(w => w is IMyFunctionalBlock).Select(w => w as IMyFunctionalBlock).ToList();
  if (menu == 5)
    blocks = h2Engines.Keys.Where(e => e is IMyFunctionalBlock).Select(e => e as IMyFunctionalBlock).ToList();
  if (menu == 6)
    blocks = reactors.Where(r => r is IMyFunctionalBlock).Select(r => r as IMyFunctionalBlock).ToList();
  if (blocks.Any())
  {
    IMyFunctionalBlock prevBlock = blocks.TakeWhile(b => b.EntityId != selectedblock[menu]).DefaultIfEmpty(blocks[blocks.Count - 1]).LastOrDefault();
    newSelection = prevBlock.EntityId;
  }
  return newSelection;
}

void ToggleSelection(int menu)
{
  if (menu == 2)
  {
    if (batteries.Any())
    {
      int mode;
      IMyBatteryBlock battery = batteries.First(b => b.EntityId == selectedblock[menu]);
      mode = (int)battery.ChargeMode;
      mode++;
      mode = mode % 3;
      battery.ChargeMode = (ChargeMode)mode;
    }
  }
  if (menu == 6)
  {
    if (reactors.Any())
    {
      IMyReactor reactor = reactors.First(r => r.EntityId == selectedblock[menu]);
      reactor.UseConveyorSystem = !reactor.UseConveyorSystem;
    }
  }
}

void EnableBlock(int menu, bool status)
{
  List<IMyFunctionalBlock> blocks = new List<IMyFunctionalBlock>();
  if (menu == 2)
    blocks = batteries.Where(b => b is IMyFunctionalBlock).Select(b => b as IMyFunctionalBlock).ToList();
  if (menu == 3)
    blocks = solarPanels.Where(s => s is IMyFunctionalBlock).Select(s => s as IMyFunctionalBlock).ToList();
  if (menu == 4)
    blocks = windTurbines.Where(w => w is IMyFunctionalBlock).Select(w => w as IMyFunctionalBlock).ToList();
  if (menu == 5)
    blocks = h2Engines.Keys.Where(e => e is IMyFunctionalBlock).Select(e => e as IMyFunctionalBlock).ToList();
  if (menu == 6)
    blocks = reactors.Where(r => r is IMyFunctionalBlock).Select(r => r as IMyFunctionalBlock).ToList();
  if (blocks.Any())
  {
    IMyFunctionalBlock block = blocks.First(b => b.EntityId == selectedblock[menu]);
    block.Enabled = status;
  }
}

void CheckBatteryCharging()
{
  float chargeLevel = batteries.Where(b => b.IsWorking).Where(b => b.ChargeMode != ChargeMode.Discharge).Sum(b => b.CurrentStoredPower)
    / (batteries.Where(b => b.IsWorking).Where(b => b.ChargeMode != ChargeMode.Discharge).Sum(b => b.MaxStoredPower) * 0.999f);
  if (charging || chargeLevel <= lowerBatLimit)
    charging = ChargeBattery(true, useAllBackups);
  if (chargeLevel >= upperBatLimit)
    charging = ChargeBattery(false, useAllBackups);
}

bool ChargeBattery(bool charge, bool all)
{
  int blocksCharging = 0;
  if (!charge)
  {
    for (int i = 0; i < backupBlocks.Length; i++)
      backupBlocks[i].ForEach(b => b.Enabled = false);
  }
  else
  {
    float chargingPower = solarOutputs.Last() + windTurbineOutputs.Last();
    if (all)
    {
      int i = 0;
      while (i < backupBlocks.Length && chargingPower < (usedPower * 1.1f))
      {
        backupBlocks[i].ForEach(b => chargingPower += TryStartCharging(b, ref blocksCharging));
        i++;
      }
    }
    else
    {
      for (int i = 0; i < backupBlocks.Length; i++)
      {
        backupBlocks[i].ForEach(b =>
        {
          if (chargingPower < (usedPower * 1.1f))
            chargingPower += TryStartCharging(b, ref blocksCharging);
          else
            b.Enabled = false;
        });
      }
    }
  }
  if (blocksCharging > 0)
    return true;
  return false;
}

void BackupBlockList(bool reactorFirst)
{
  if (reactorFirst)
  {
    backupBlocks[0] = reactors.Where(r => r is IMyPowerProducer).Select(r => r as IMyPowerProducer).ToList<IMyPowerProducer>();
    backupBlocks[1] = h2Engines.Keys.Where(r => r is IMyPowerProducer).Select(r => r as IMyPowerProducer).ToList<IMyPowerProducer>();
  }
  else
  {
    backupBlocks[0] = h2Engines.Keys.Where(r => r is IMyPowerProducer).Select(r => r as IMyPowerProducer).ToList<IMyPowerProducer>();
    backupBlocks[1] = reactors.Where(r => r is IMyPowerProducer).Select(r => r as IMyPowerProducer).ToList<IMyPowerProducer>();
  }
  firstBackUpReactor = reactorFirst;
}

float TryStartCharging(IMyPowerProducer block, ref int blocksCharging)
{
  bool resources = false;
  if (block is IMyReactor)
  {
    if (block.GetInventory(0).CurrentMass > 0)
      resources = true;
  }
  else
  {
    HydrogenEngine engine;
    float curVolume = 0f;
    if (h2Engines.TryGetValue(block, out engine))
      curVolume = engine.Capacity * engine.FilledRatio;
    if (curVolume > 0)
      resources = true;
  }
  block.Enabled = resources;
  if (block.Enabled)
  {
    blocksCharging++;
    return block.MaxOutput;
  }
  return 0;
}

void GenerateMainScreen(List<IMyTextSurface> surfaces, int menu)
{
  string headline;
  surfaces.ForEach(s =>
  {
    headline = EnDe(menuTitles[menu]);
    SpriteLayout mainLayout = new SpriteLayout(s);
    mainLayout.GenerateHeader(headline);
    List<IMyFunctionalBlock> blocks = new List<IMyFunctionalBlock>();
    if (menu == 2)
      blocks = batteries.Where(b => b is IMyFunctionalBlock).Select(b => b as IMyFunctionalBlock).ToList();
    if (menu == 3)
      blocks = solarPanels.Where(p => p is IMyFunctionalBlock).Select(p => p as IMyFunctionalBlock).ToList();
    if (menu == 4)
      blocks = windTurbines.Where(w => w is IMyFunctionalBlock).Select(w => w as IMyFunctionalBlock).ToList();
    if (menu == 5)
      blocks = h2Engines.Keys.Where(e => e is IMyFunctionalBlock).Select(e => e as IMyFunctionalBlock).ToList();
    if (menu == 6)
      blocks = reactors.Where(r => r is IMyFunctionalBlock).Select(r => r as IMyFunctionalBlock).ToList();
    if (blocks.Any())
      mainLayout.GenerateSymbolsArea(blocks, selectedblock[menu], true);
    if (menu == 1)
      mainLayout.GenerateControlArea(batteries, windTurbines, solarPanels, backupBlocks, lowerBatLimit, upperBatLimit, useAllBackups, !manualControl);
    mainLayout.DrawLayout();
  });
}

void GenerateOverallStatus(List<IMyTextSurface> surfaces)
{
  float maxOutput;
  int active;
  int count;
  float maxPower = maxSolarOutput + maxH2EnginesOutput + maxReactorOutput + maxWindTurbineOutput + maxBatteryOutput;
  surfaces.ForEach(s =>
  {
    SpriteLayout overallScreenLayout = new SpriteLayout(s);
    overallScreenLayout.GenerateHeader("Power Status");
    overallScreenLayout.GeneratePowerOverviewArea(usedPower, producedPower, maxPower);
    count = batteries.Count; active = 0;
    if (count > 0)
    {
      active = batteries.Count(h => h.IsWorking);
      maxOutput = batteries.Where(b => b.ChargeMode == ChargeMode.Discharge || b.ChargeMode == ChargeMode.Auto).Sum(b => b.MaxOutput);
      overallScreenLayout.GenerateOverviewArea(EnDe("Batteries"), active, count, batteryPowerFlow.Last(), maxOutput, maxBatteryOutput);
    }
    count = windTurbines.Count; active = 0;
    if (count > 0)
    {
      active = windTurbines.Count(h => h.IsWorking);
      overallScreenLayout.GenerateOverviewArea(EnDe("Windmills"), active, count, windTurbineOutputs.Last(), windTurbineMaxOutputs.Last(), maxWindTurbineOutput);
    }
    count = solarPanels.Count; active = 0;
    if (count > 0)
    {
      active = solarPanels.Count(p => p.IsWorking);
      overallScreenLayout.GenerateOverviewArea(EnDe("Solar Panel"), active, count, solarOutputs.Last(), solarMaxOutputs.Last(), maxSolarOutput);
    }
    count = h2Engines.Count; active = 0;
    if (count > 0)
    {
      active = h2Engines.Keys.Count(h => h.IsWorking);
      overallScreenLayout.GenerateOverviewArea(EnDe("H2-Engines"), active, count, h2EngineOutputs.Last(), h2EngineMaxOutputs.Last(), maxH2EnginesOutput);
    }
    count = reactors.Count; active = 0;
    if (count > 0)
    {
      active = reactors.Count(h => h.IsWorking);
      overallScreenLayout.GenerateOverviewArea(EnDe("Reactors"), active, count, reactorOutputs.Last(), reactorMaxOutputs.Last(), maxReactorOutput);
    }
    overallScreenLayout.DrawLayout();
  });
}

void GenerateInstructions(List<IMyTextSurface> surfaces)
{
  string headline;
  string[] tabs;
  surfaces.ForEach(s =>
  {
    headline = EnDe(menuTitles[shownMenu]);
    SpriteLayout instructionLayout = new SpriteLayout(s);
    instructionLayout.GenerateHeader(headline);
    if (menus.TryGetValue(shownMenu, out tabs))
    {
      instructionLayout.GenerateInstructionsScreen(tabs);
    }
    instructionLayout.DrawLayout();
  });
}

void GenerateBatteryScreen(List<IMyTextSurface> surfaces)
{
  string headline;
  surfaces.ForEach(s =>
  {
    if (s.SurfaceSize.X < 500f)
    {
      headline = EnDe("Batteries");
    }
    else
    {
      headline = EnDe("Batteries Status");
    }
    List<IMyFunctionalBlock> blocks = batteries.Where(b => b is IMyFunctionalBlock).Select(b => b as IMyFunctionalBlock).ToList();
    SpriteLayout batteryScreenLayout = new SpriteLayout(s);
    batteryScreenLayout.GenerateHeader(headline);
    batteryScreenLayout.GenerateBatteryLoadBar(curStoredPower, maxStoredPower);
    batteryScreenLayout.GenerateSymbolsArea(blocks, 0, false);
    batteryScreenLayout.GenerateDoubleChart(batteryPowerFlow);
    batteryScreenLayout.DrawLayout();
  });
}

void GenerateSolarScreen(List<IMyTextSurface> surfaces)
{
  string headline;
  surfaces.ForEach(s =>
  {
    if (s.SurfaceSize.X < 500f)
    {
      headline = EnDe("Solar Panels");
    }
    else
    {
      headline = EnDe("Solar Panels Status");
    }
    List<IMyFunctionalBlock> blocks = solarPanels.Where(p => p is IMyFunctionalBlock).Select(p => p as IMyFunctionalBlock).ToList();
    SpriteLayout solarScreenLayout = new SpriteLayout(s);
    solarScreenLayout.GenerateHeader(headline);
    solarScreenLayout.GenerateSymbolsArea(blocks, 0, false);
    solarScreenLayout.GenerateSingleChart(solarOutputs, solarMaxOutputs, maxSolarOutput, new Color(29, 60, 85));
    solarScreenLayout.DrawLayout();
  });
}

void GenerateH2EngineScreen(List<IMyTextSurface> surfaces)
{
  string headline;
  surfaces.ForEach(s =>
  {
    if (s.SurfaceSize.X < 500f)
    {
      headline = EnDe("H2-Engines");
    }
    else
    {
      headline = EnDe("Hydrogen Engines Status");
    }
    List<IMyFunctionalBlock> blocks = h2Engines.Keys.Where(h => h is IMyFunctionalBlock).Select(h => h as IMyFunctionalBlock).ToList();
    SpriteLayout h2EngineScreenLayout = new SpriteLayout(s);
    h2EngineScreenLayout.GenerateHeader(headline);
    h2EngineScreenLayout.GenerateH2EngineLoadBar(curH2GasVolume, maxH2GasVolume);
    h2EngineScreenLayout.GenerateSymbolsArea(blocks, 0, false);
    h2EngineScreenLayout.GenerateSingleChart(h2EngineOutputs, h2EngineMaxOutputs, maxH2EnginesOutput, new Color(90, 25, 25));
    h2EngineScreenLayout.DrawLayout();
  });
}

void GenerateWindTurbineScreen(List<IMyTextSurface> surfaces)
{
  string headline;
  surfaces.ForEach(s =>
  {
    if (s.SurfaceSize.X < 500f)
    {
      headline = EnDe("Windmills");
    }
    else
    {
      headline = EnDe("Windmills Status");
    }
    List<IMyFunctionalBlock> blocks = windTurbines.Where(w => w is IMyFunctionalBlock).Select(w => w as IMyFunctionalBlock).ToList();
    SpriteLayout windTurbineScreenLayout = new SpriteLayout(s);
    windTurbineScreenLayout.GenerateHeader(headline);
    windTurbineScreenLayout.GenerateSymbolsArea(blocks, 0, false);
    windTurbineScreenLayout.GenerateSingleChart(windTurbineOutputs, windTurbineMaxOutputs, maxWindTurbineOutput, new Color(0, 65, 25));
    windTurbineScreenLayout.DrawLayout();
  });
}

void GenerateReactorScreen(List<IMyTextSurface> surfaces)
{
  Color reactorColor = new Color(204, 180, 0);
  string headline;
  surfaces.ForEach(s =>
  {
    if (s.SurfaceSize.X < 500f)
    {
      headline = EnDe("Reactors");
    }
    else
    {
      headline = EnDe("Reactors Status");
    }
    List<IMyFunctionalBlock> blocks = reactors.Where(r => r is IMyFunctionalBlock).Select(r => r as IMyFunctionalBlock).ToList();
    SpriteLayout reactorScreenLayout = new SpriteLayout(s);
    reactorScreenLayout.GenerateHeader(headline);
    reactorScreenLayout.GenerateCargoLoadBar(curReactorIngots, maxReactorIngots);
    reactorScreenLayout.GenerateSymbolsArea(blocks, 0, false);
    reactorScreenLayout.GenerateSingleChart(reactorOutputs, reactorMaxOutputs, maxReactorOutput, reactorColor);
    reactorScreenLayout.DrawLayout();
  });
}

void GenerateProgramScreen(List<IMyTextSurface> surfaces)
{
  surfaces.ForEach(s =>
  {
    SpriteLayout programScreenLayout = new SpriteLayout(s);
    programScreenLayout.Font = "Monospace";
    programScreenLayout.FontSize = 0.4f;
    programScreenLayout.GenerateHeader(EnDe("Power Control Program"));
    programScreenLayout.Write(programInfo.ToString(), colorOn, TextAlignment.LEFT);
    programScreenLayout.DrawLayout();
  });
}

void CalculateMaxReactorOutput(List<IMyReactor> reactors)
{
    float maxOutput = 0;
    reactors.ForEach(r => maxOutput += r.MaxOutput);
    if (maxOutput > maxReactorOutput)
maxReactorOutput = maxOutput;
}

void CalculateMaxHydrogenEnginesOutput(List<IMyPowerProducer> engines)
{
    float maxOutput = 0;
    engines.ForEach(e => maxOutput += e.MaxOutput);
    if (maxOutput > maxH2EnginesOutput)
      maxH2EnginesOutput = maxOutput;
}

void CalculateMaxWindTurbineOutput(List<IMyPowerProducer> turbines)
{
    float maxOutput = 0;
    turbines.ForEach(wt => maxOutput += wt.MaxOutput);
    if (maxOutput > maxWindTurbineOutput)
      maxWindTurbineOutput = maxOutput;
}

void CalculateMaxBatteryOutput(List<IMyBatteryBlock> batteries)
{
    float maxOutput = 0;
    batteries.ForEach(b => maxOutput += b.MaxOutput);
    if (maxOutput > maxBatteryOutput)
      maxBatteryOutput = maxOutput;
}

void CalculateMaxSolarOutput(List<IMySolarPanel> panels)
{
    float maxOutput = 0;
    panels.ForEach(p =>
    {
      maxOutput += p.MaxOutput;
      maxSolarOutputPerPanel = Math.Max(p.MaxOutput, maxSolarOutputPerPanel);
    });
    if (maxOutput > maxSolarOutput)
      maxSolarOutput = maxOutput;
}

void InitScreenCustomData(List<IMyTerminalBlock> blocks)
{
  foreach (IMyTerminalBlock block in blocks)
  {
    bool init = true;
    string[] customData = block.CustomData.Split('\n');
    if (customData[0].Contains(programToken) && customData.Length > 1 && customData[1].Contains('='))
      init = false;
    if (init)
    {
      var provider = block as IMyTextSurfaceProvider;
      int surfaceCount = provider.SurfaceCount;
      StringBuilder newCustomData = new StringBuilder();
      newCustomData.Append(programToken + "\n");
      for (int i = 0; i < surfaceCount; i++)
      {
        newCustomData.Append(provider.GetSurface(i).DisplayName + "=" + screenTypes[i] + "\n");
      }
      newCustomData.Append("\n//********************************** README **********************************//\n" +
                 "copy one of the following commands after the = of the screen-names\n\n");
      foreach (string type in screenTypes)
      {
        newCustomData.Append(type + "\n");
      }
      block.CustomData = newCustomData.ToString();
    }
  }
}

void MatchTypesWithScreens(List<IMyTerminalBlock> blocks, out Dictionary<string, List<IMyTextSurface>> matchedSurfaces)
{
  matchedSurfaces = new Dictionary<string, List<IMyTextSurface>>();
  foreach (var block in blocks)
  {
    string[] customData = block.CustomData.Substring(0, block.CustomData.IndexOf("//**")).Split('\n', '=');
    IMyTextSurfaceProvider provider = block as IMyTextSurfaceProvider;
    Dictionary<string, IMyTextSurface> surfaces = new Dictionary<string, IMyTextSurface>(provider.SurfaceCount);
    IMyTextSurface surface;
    for (int i = 0; i < provider.SurfaceCount; i++)
    {
      surfaces.Add(provider.GetSurface(i).DisplayName, provider.GetSurface(i));
    }
    for (int i = 0; i < customData.Length; i++)
    {
      if (surfaces.TryGetValue(customData[i], out surface))
      {
        List<IMyTextSurface> oldSurfaces;
        try
        {
          if (matchedSurfaces.ContainsKey(customData[i + 1]))
          {
            oldSurfaces = matchedSurfaces[customData[i + 1]];
            oldSurfaces.Add(surface);
          }
          else
          {
            oldSurfaces = new List<IMyTextSurface>();
            oldSurfaces.Add(surface);
          }
          matchedSurfaces[customData[i + 1]] = oldSurfaces;
        }
        catch (Exception e)
        {
          ConsoleEcho("Assignment failed at " + block.CustomName + "\n" + e.ToString());
          surface.ContentType = ContentType.TEXT_AND_IMAGE;
          surface.Alignment = TextAlignment.CENTER;
          surface.FontSize = 2;
          surface.FontColor = Color.Red;
          surface.WriteText("Assignment failed\n" + e.ToString());
        }
      }
    }
  }
}

static string GetShownName(IMyTerminalBlock block)
{
  if (block.CustomData.StartsWith("@") && block.CustomData.Contains(programToken))
  {
    string[] lines = block.CustomData.Split('\n');
    for (int i = 1; i < lines.Length; i++)
    {
      if (lines[i].StartsWith("name"))
      {
        lines[i] = lines[i].Substring(5).Trim();
        if (lines[i].Length >= 1)
          return lines[i];
      }
    }
  }
  return block.CustomName;
}

static string EnDe(string eng)
{
  string ger;
  string engFormattet;
  Dictionary<string, string> translateGer = new Dictionary<string, string>{
    { "Solar Panels", "Solar Panele" }, { "Reactors", "Reaktoren" }, { "H2-Engines", "H2-Motoren" }, { "Windmills", "Windräder" }, { "Batteries", "Batterien" },
    { "Power Control Program", "Power Kontroll-Programm" }, { "Batteries Status", "Batterien Ladungsstand" }, { "Solar Panels Status", "Solar Panele Status" },
    { "Hydrogen Engines Status", "Wasserstoffmotoren Status" }, { "Reactors Status", "Reaktoren Status" }, { "Windmills Status", "Windräder Status" },
    { "Toolbar", "Befehlsleiste" }, { "Instructions Toolbar", "Anweisungen Befehlsleiste" },
    { "of", "von" }, { "used of", "verbraucht von" }, { "of max. possible", "von max. möglichen" }, { "are online", "sind am Netz" },
    { "fully depleted in", "voll entladen in" }, { "fully recharged in", "voll geladen in"},
    { "show instruction screen", "Befehle anzeigen" },
    { "select previous item", "vorheriges Objekt auswählen" }, { "select next item", "nächstes Objekt anzeigen" },
    { "activate selected item", "aktiviere ausgewähltes Objekt" }, { "deactivate selected item", "deaktiviere ausgewähltes Objekt" },
    { "switch selected item mode", "Modus bei ausgew. Objekt umschalten" },
    { "switch all", "alle Objekte umschalten" }, { "activate all", "alle Objekte aktivieren" }, { "deactivate all", "alle Objekte deaktivieren" },
    { "switch automatic refill", "automatisches nachfüllen umschalten" }, { "switch refill for all", "nachfüllen für alle umschalten"},
    { "Program Control", "Programmsteuerung" }, { "Battery Control", "Batteriesteuerung" }, { "Solar Control", "Solarzellensteuerung" },
    { "Wind Turbine Control", "Windrädersteuerung" }, { "H2 Engine Control", "H2-Motorensteuerung" }, { "Reactor Control", "Reaktorensteuerung" },
    { "activate\nall blocks", "aktiviere\nalle Blöcke" }, { "activate blocks\none by one", "aktiviere Blöcke\nnacheinander" },
    { "manual\npower control", "manuelle\nPower-Kontrolle" }, { "automatic\npower control", "automatische\nPower-Kontrolle" },
    { "turn auto. power control on/off", "auto. Ladesteuerung an/aus" }, { "activate all at once/step by step", "aktiviere gemeinsam/einzeln" },
    { "set uranium as first backup", "wähle Uran als erstes Backup" }, { "set hydrogen as first backup", "wähle Wasserstoff als erstes Backup" },
    { "decrease lower battery limit", "reduziere unteres Ladelimit" }, { "increase lower battery limit", "erhöhe unteres Ladelimit" },
    { "decrease upper battery limit", "reduziere oberes Ladelimit" }, { "increase upper battery limit", "erhöhe oberes Ladelimit" },
    { "switch solar alignment on/off", "Solarturmausrichtung an/aus" }, { "Solar Alignment: online", "Solarzellenausrichtung: aktiv" }, { "Solar Alignment: offline", "Solarzellenausrichtung: inaktiv" }
  };

  Dictionary<string, string> translateEng = new Dictionary<string, string>{
    { "OxygenBottle", "O2 Bottle" },
    { "HydrogenBottle", "H2 Bottle" } };
  if (!en)
  {
    if (translateGer.TryGetValue(eng, out ger))
    {
      return ger;
    }
  }
  else
  {
    if (translateEng.TryGetValue(eng, out engFormattet))
    {
      return engFormattet;
    }
  }
  return eng;
}

void ConsoleEcho(string line)
{
  programInfo.Append(line + "\n");
  Echo(line);
}

class HydrogenEngine
{
  public IMyPowerProducer Engine { get; }
  public float Capacity { get; internal set; }
  public float FilledRatio { get; internal set; }

  public HydrogenEngine(IMyPowerProducer engine)
  {
    Engine = engine;
    Refresh();
  }

  public void Refresh()
  {
    string[] lines = Engine.DetailedInfo.Split('\n');
    int startIndex = lines[3].IndexOf('/') + 1;
    int endIndex = lines[3].Length - 2;
    int length = endIndex - startIndex;
    Capacity = float.Parse(lines[3].Substring(startIndex, length));
    startIndex = lines[3].IndexOf(':') + 1;
    endIndex = lines[3].IndexOf('%');
    length = endIndex - startIndex;
    FilledRatio = float.Parse(lines[3].Substring(startIndex, length)) / 100;
  }

}

class SpriteLayout
{
  public IMyTextSurface Surface { get; }
  public MySpriteDrawFrame Frame { get; }
  public RectangleF SurfaceArea { get; }
  public string Font { get; set; }
  public Color MainColor { get; set; }
  Color greenColor = new Color(0, 65, 0);
  Color redColor = new Color(65, 0, 0);
  Color blueColor = new Color(25, 25, 65);
  Color solarColor = new Color(29, 60, 85);
  Color h2Color = new Color(90, 25, 25);
  Color reactorColor = new Color(204, 180, 0);
  Vector2 position = new Vector2(0);
  Vector2 offset = new Vector2(0);
  float spacer = 0;
  float fontSize = 1f;
  MySprite background;
  List<MySprite> allSprites = new List<MySprite>();
  static float maxBatteryLabeling = 5f;
  readonly Color colorOff = Color.Red;

  public float FontSize
  {
    get
    {
      if (SurfaceArea.Height < 200 || SurfaceArea.Width < 200)
        return fontSize / 0.6f;
      else
        return fontSize;
    }
    set
    {
      if (SurfaceArea.Height < 200 || SurfaceArea.Width < 200)
        fontSize = value * 0.6f;
      else
        fontSize = value;
    }
  }

  Func<float, string> wattage = w =>
  {
    string formattedOutput;
    if (Math.Abs(w) < 0.9f)
      formattedOutput = Math.Round(Math.Abs(w) * 1000, 1).ToString("#,0.", region) + " kW";
    else
      formattedOutput = Math.Round(Math.Abs(w), 1).ToString("#,0.0", region) + " MW";
    if (w < 0)
      formattedOutput = "-" + formattedOutput;
    return formattedOutput;
  };

  Func<float, string> mass = m =>
  {
    if (m < 1000)
      return Math.Round(m, 0).ToString("#,0.#", region) + " kg";
    else
      return Math.Round((m / 1000), 1).ToString("#,0.0", region) + " t ";
  };

  Func<float, string> volume = v => (v / 1000).ToString("#,0.", region) + " m³";

  public SpriteLayout(IMyTextSurface surface)
  {
    Surface = surface;
    Surface.WriteText("");
    Surface.ContentType = ContentType.SCRIPT;
    Surface.Script = string.Empty;
    Frame = Surface.DrawFrame();
    SurfaceArea = CalculateSurfaceArea(Surface.TextureSize, Surface.SurfaceSize);
    Font = "Debug";
    FontSize = 1f;
    MainColor = new Color(130, 130, 130);
    SetSpacer(50f);
    position = offset;
    ChangeBackgroundColor(Color.Black);
  }

  RectangleF CalculateSurfaceArea(Vector2 textureSize, Vector2 surfaceSize)
  {
    float width;
    float height;
    if (textureSize.X > surfaceSize.X)
    {
      width = surfaceSize.X;
      offset.X = (textureSize.X - surfaceSize.X) / 2;
    }
    else
    {
      width = textureSize.X;
      offset.X = (surfaceSize.X - textureSize.X) / 2;
    }
    if (textureSize.Y > surfaceSize.Y)
    {
      height = surfaceSize.Y;
      offset.Y = (textureSize.Y - surfaceSize.Y) / 2;
    }
    else
    {
      height = textureSize.Y;
      offset.Y = (surfaceSize.Y - textureSize.Y) / 2;
    }
    return new RectangleF(offset, new Vector2(width, height));
  }

  public void ChangeBackgroundColor(Color backColor) =>
    background = new MySprite(SpriteType.TEXTURE, "SquareSimple", SurfaceArea.Center, SurfaceArea.Size, backColor);

  public void GenerateHeader(string headline)
  {
    Random rnd = new Random();
    int emtpySprites = rnd.Next(5);
    for (int i = 0; i < emtpySprites; i++)
      allSprites.Add(new MySprite());
    float headerFormat = 1.25f;
    fontSize *= headerFormat;
    Vector2 textSize = Surface.MeasureStringInPixels(new StringBuilder(headline), Font, fontSize);
    position = new Vector2(SurfaceArea.Center.X, offset.Y);
    Write(headline, MainColor, TextAlignment.CENTER);
    fontSize = fontSize / headerFormat;
    DrawSeperationLine(position.Y + textSize.Y + spacer);
    position = new Vector2(offset.X + spacer, offset.Y + textSize.Y + (spacer * 2.5f));
    Surface.WriteText(headline);
  }

  public void GenerateInstructionsScreen(string[] instructions)
  {
    position.X = SurfaceArea.Position.X;
    Vector2 textArea = new Vector2(SurfaceArea.Width - (2 * spacer), SurfaceArea.Height - position.Y - spacer + offset.Y);
    StringBuilder text = new StringBuilder();
    for (int i = 0; i < instructions.Length; i++)
    {
      text.Append((i + 1) + ": " + EnDe(instructions[i]) + "\n");
    }
    Vector2 textSize = Surface.MeasureStringInPixels(text, Font, fontSize);
    Vector2 textPos = position;
    float scaleX = 1f;
    float scaleY = 1f;
    float scale = 1f;
    if (textArea.X < textSize.X)
      scaleX = textArea.X / textSize.X;
    if (textArea.Y < textSize.Y)
      scaleY = textArea.Y / textSize.Y;
    if (scaleX <= scaleY)
      scale = scaleX;
    else
      scale = scaleY;
    fontSize *= scale;
    textSize = Surface.MeasureStringInPixels(text, Font, fontSize);
    position.X = SurfaceArea.Position.X + ((SurfaceArea.Width - textSize.X) / 2);
    Write(text.ToString(), MainColor, TextAlignment.LEFT);
    fontSize /= scale;
    position.X = SurfaceArea.Position.X; position.Y += textSize.Y + spacer;
  }

  public void GeneratePowerOverviewArea(float usedPower, float producedPower, float maxPosPower)
  {
    string infoLine = EnDe("Power Overview");
    string powerLine = wattage(usedPower) + "/" + wattage(producedPower);
    Write(infoLine, MainColor, TextAlignment.LEFT);
    position.X += SurfaceArea.Width - (2 * spacer);
    Write(powerLine, MainColor, TextAlignment.RIGHT);
    Vector2 infoLineSize = Surface.MeasureStringInPixels(new StringBuilder(infoLine), Font, fontSize);
    position.Y += infoLineSize.Y + (spacer / 2);
    string posPowerInfo = wattage(maxPosPower);
    Vector2 posPowerInfoSize = Surface.MeasureStringInPixels(new StringBuilder("000.0 MW"), Font, fontSize);
    Write(posPowerInfo, MainColor, TextAlignment.RIGHT);
    Vector2 barSize = new Vector2(SurfaceArea.Width - (4 * spacer) - posPowerInfoSize.X, posPowerInfoSize.Y);
    float percentageUsed = usedPower / maxPosPower;
    float percentageProduced = producedPower / maxPosPower;
    position.X = SurfaceArea.Position.X + spacer;
    Vector2 pos = position;
    float line = barSize.Y * 0.1f;
    DrawSquareTL(pos, new Vector2(barSize.X, line), MainColor, true);
    DrawSquareTL(pos, new Vector2(line, barSize.Y), MainColor, true);
    DrawSquareTL(pos + new Vector2(barSize.X - line, 0), new Vector2(line, barSize.Y), MainColor, true);
    DrawSquareTL(pos + new Vector2(0, barSize.Y - line), new Vector2(barSize.X, line), MainColor, true);
    pos += new Vector2(line); barSize.X -= 2 * line; barSize.Y -= 2 * line;
    DrawSquareTL(pos, new Vector2(barSize.X * percentageUsed, barSize.Y / 2), redColor, true);
    DrawSquareTL(pos + new Vector2(0, barSize.Y / 2), new Vector2(barSize.X * percentageProduced, barSize.Y / 2), greenColor, true);
    position.Y += barSize.Y + (spacer * 2);
    DrawSeperationLine(position.Y);
    position.Y += spacer;
  }

  public void GenerateOverviewArea(string titel, int active, int count, float curOutput, float maxOutput, float maxPosOutput)
  {
    string infoLine = active + "/" + count + " " + titel;
    string powerLine = wattage(curOutput) + "/" + wattage(maxOutput);
    Write(infoLine, MainColor, TextAlignment.LEFT);
    position.X += SurfaceArea.Width - (2 * spacer);
    Write(powerLine, MainColor, TextAlignment.RIGHT);
    Vector2 infoLineSize = Surface.MeasureStringInPixels(new StringBuilder(infoLine), Font, fontSize);
    position.Y += infoLineSize.Y + (spacer / 2);
    string posPowerInfo = wattage(maxPosOutput);
    Vector2 posPowerInfoSize = Surface.MeasureStringInPixels(new StringBuilder("000.0 MW"), Font, fontSize);
    Write(posPowerInfo, MainColor, TextAlignment.RIGHT);
    Vector2 barSize = new Vector2(SurfaceArea.Width - (4 * spacer) - posPowerInfoSize.X, posPowerInfoSize.Y);
    float percentage1 = curOutput / maxPosOutput;
    float percentage2 = maxOutput / maxPosOutput;
    float[] percentages = { percentage1, percentage2 };
    position.X = SurfaceArea.Position.X + spacer;
    Color statusColor = greenColor;
    if (percentage1 >= 0.9f)
      statusColor = redColor;
    DrawHorizontalBar(position, barSize, statusColor, percentages);
    position.Y += barSize.Y + spacer;
  }

  public void GenerateBatteryLoadBar(float curLoad, float maxLoad)
  {
    DrawLoadBar(curLoad, maxLoad, wattage, "IconEnergy");
  }

  public void GenerateH2EngineLoadBar(float curLoad, float maxLoad)
  {
    DrawLoadBar(curLoad, maxLoad, volume, "IconHydrogen");
  }

  public void GenerateCargoLoadBar(float curLoad, float maxLoad)
  {
    DrawLoadBar(curLoad, maxLoad, mass, "MyObjectBuilder_Ingot/Uranium");
  }

  public void GenerateControlArea(List<IMyBatteryBlock> batts, List<IMyPowerProducer> turbines, List<IMySolarPanel> panels, List<IMyPowerProducer>[] backupBlocks, float lowerLimit, float upperLimit, bool switchingAll, bool auto)
  {
    if (!batts.Any())
    {
      Write("--- no batteries available ---\nPlease install at least one battery\nto use the Power Control Program.", Color.Red, TextAlignment.LEFT);
      return;
    }
    RectangleF batteryArea = new RectangleF(position, new Vector2((SurfaceArea.Width - (2 * spacer)) * 0.25f, SurfaceArea.Height - position.Y + offset.Y - spacer));
    RectangleF symbolArea = new RectangleF(position + new Vector2(batteryArea.Width + spacer, 0), new Vector2((SurfaceArea.Width - (2 * spacer)) * 0.75f, SurfaceArea.Height - position.Y + offset.Y - spacer));
    Vector2 battSymbolSize = new Vector2(batteryArea.Width - (2 * spacer));
    battSymbolSize.Y *= 1.1618f;
    if (batteryArea.Width > batteryArea.Height)
    {
      battSymbolSize = new Vector2(batteryArea.Height - (2 * spacer));
      battSymbolSize.X /= 1.1618f;
    }
    float charge = batts.Where(b => b.IsWorking).Where(b => b.ChargeMode != ChargeMode.Discharge).Sum(b => b.CurrentStoredPower) / (batts.Where(b => b.IsWorking).Where(b => b.ChargeMode != ChargeMode.Discharge).Sum(b => b.MaxStoredPower) * 0.99f);
    bool working = batts.Any(b => b.IsWorking);
    bool charging = true;
    if (batts.Sum(b => b.CurrentInput) < batts.Sum(b => b.CurrentOutput)) charging = false;
    string upperText = upperLimit.ToString("P0", region);
    string lowerText = lowerLimit.ToString("P0", region);
    Vector2 upperTextSize = Surface.MeasureStringInPixels(new StringBuilder(upperText), Font, fontSize);
    Vector2 lowerTextSize = Surface.MeasureStringInPixels(new StringBuilder(lowerText), Font, fontSize);
    DrawBatterySymbol(working, charging, charge, position + new Vector2(spacer, (batteryArea.Height - battSymbolSize.Y) / 2), battSymbolSize);
    if (auto)
    {
      position.Y += ((batteryArea.Height - battSymbolSize.Y) / 2) - upperTextSize.Y - spacer;
      position.X += batteryArea.Width / 2;
      if (SurfaceArea.Height < battSymbolSize.Y * 2)
      {
        position.Y += upperTextSize.Y + spacer;
        position.X += battSymbolSize.X / 2;
      }
      Write(upperText, MainColor, TextAlignment.CENTER);
      if (SurfaceArea.Height < battSymbolSize.Y * 2)
        position.Y += battSymbolSize.Y - spacer - lowerTextSize.Y;
      else
        position.Y += battSymbolSize.Y + upperTextSize.Y + (2 * spacer);
      Write(lowerText, MainColor, TextAlignment.CENTER);
    }
    position = symbolArea.Position;
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Arrow", position + new Vector2((symbolArea.Width / 6) + (spacer / 2), symbolArea.Height / 2),
      new Vector2(symbolArea.Height * 0.2f, (symbolArea.Width / 3) + spacer), MainColor, null, TextAlignment.CENTER, (float)(Math.PI / -2)));
    position.X += spacer;
    Vector2 symbolSize;
    if ((symbolArea.Width - (4 * spacer)) / 3f > (symbolArea.Height * 0.8f * 0.5f) - spacer)
      symbolSize = new Vector2((symbolArea.Height * 0.8f * 0.5f) - spacer);
    else
      symbolSize = new Vector2((symbolArea.Width - (4 * spacer)) / 3);
    if (panels.Any())
    {
      float efficiency = panels.Sum(p => p.CurrentOutput) / (panels.Max(p => p.MaxOutput) * panels.Count);
      working = panels.Any(p => p.IsWorking);
      Vector2 solarSize = new Vector2(symbolSize.X / 1.1618f, symbolSize.Y);
      DrawSolarSymbol(working, efficiency, position + new Vector2((((symbolArea.Width - (2 * spacer)) / 3) - solarSize.X) / 2, (symbolArea.Height / 2) - spacer - solarSize.Y - (symbolArea.Height * 0.1f)), solarSize);
    }
    if (turbines.Any())
    {
      float efficiency = turbines.Sum(p => p.CurrentOutput) / (turbines.Max(p => p.MaxOutput) * turbines.Count);
      working = turbines.Any(p => p.IsWorking);
      DrawWindTurbineSymbol(working, efficiency, position + new Vector2((((symbolArea.Width - (2 * spacer)) / 3) - symbolSize.X) / 2, (symbolArea.Height / 2) + spacer + (symbolArea.Height * 0.1f)), symbolSize);
    }
    Vector2 tempPos = position;
    fontSize *= 0.8f;
    string switchText;
    string activeText;
    if (switchingAll)
      switchText = EnDe("activate\nall blocks");
    else
      switchText = EnDe("activate blocks\none by one");
    position += new Vector2(symbolArea.Width / 3 * 2, (symbolArea.Height / 2) - symbolSize.Y * 1.2f);
    if (auto)
      Write(switchText, MainColor, TextAlignment.CENTER);
    if (auto)
      activeText = EnDe("automatic\npower control");
    else
      activeText = EnDe("manual\npower control");
    position.Y += symbolSize.Y * 1.8f;
    Write(activeText, MainColor, TextAlignment.CENTER);
    fontSize /= 0.8f;
    position = tempPos;
    position.X += (symbolArea.Width / 3); position.Y += (symbolArea.Height - symbolSize.Y) / 2;
    if (backupBlocks[0].Any())
    {
      DrawBackupBlockSymbol(backupBlocks[0], position, symbolSize);
    }
    position.X += spacer + symbolSize.X;
    if (backupBlocks[1].Any())
    {
      DrawBackupBlockSymbol(backupBlocks[1], position, symbolSize);
    }
  }

  void DrawBackupBlockSymbol(List<IMyPowerProducer> backupBlocks, Vector2 pos, Vector2 size)
  {
    bool working;
    if (backupBlocks.First() is IMyReactor)
    {
      working = backupBlocks.Any(b => b.IsWorking);
      float percentage = backupBlocks.Where(b => b.IsWorking).Sum(b => (float)b.GetInventory(0).CurrentVolume) / backupBlocks.Where(b => b.IsWorking).Sum(b => (float)b.GetInventory(0).MaxVolume);
      DrawReactorSymbol(working, percentage, pos, size);
    }
    else
    {
      size.Y *= 0.5f; pos.Y += size.X * 0.25f;
      working = backupBlocks.Any(b => b.IsWorking);
      float curH2 = 0f; float maxH2 = 0f;
      backupBlocks.Where(b => b.IsWorking).ToList().ForEach(b =>
      {
        HydrogenEngine engine;
        if (h2Engines.TryGetValue(b, out engine))
        {
          curH2 += engine.Capacity * engine.FilledRatio;
          maxH2 += engine.Capacity;
        }
      });
      float percentage = curH2 / maxH2;
      DrawEngineSymbol(working, percentage, pos, size);
    }
  }

  public void GenerateSymbolsArea(List<IMyFunctionalBlock> blocks, long selected, bool main)
  {
    string selectedName = "name";
    Vector2 nameSize = Surface.MeasureStringInPixels(new StringBuilder(selectedName), Font, FontSize * 0.75f);
    float drawAreaHeight = SurfaceArea.Height / 2;
    if (main)
      drawAreaHeight = SurfaceArea.Height - spacer - nameSize.Y;
    float aspectRatio = 1f;
    if (blocks.All(b => b is IMyBatteryBlock) || blocks.All(b => b is IMySolarPanel))
    {
      aspectRatio = 1.1618f;
      if (main) drawAreaHeight = SurfaceArea.Height * 0.8f;
    }
    if (blocks.All(b => b.BlockDefinition.SubtypeName.Contains("HydrogenEngine")))
      aspectRatio = 0.5f;
    RectangleF symbolsArea = new RectangleF(position, new Vector2(SurfaceArea.Width - (2 * spacer), drawAreaHeight - position.Y + offset.Y - spacer));
    Vector2 symbolSize = new Vector2(symbolsArea.Height / aspectRatio, symbolsArea.Height);
    int symbolsPerLine = CalculateSymbolsAlignment(symbolsArea, ref symbolSize, blocks.Count, out position);
    Vector2 symbolPos = position;
    int counter = 0;
    DrawSquareTL(symbolsArea.Position, symbolsArea.Size, Color.Black, true);
    if (blocks.All(b => b is IMyBatteryBlock))
    {
      blocks.Where(b => b is IMyBatteryBlock).Select(b => b as IMyBatteryBlock).ToList().ForEach(b =>
      {
        counter++;
        if (main)
        {
          if (b.EntityId == selected)
          {
            DrawSquareTL(symbolPos, symbolSize, MainColor, false);
            selectedName = GetShownName(b);
          }
          DrawBatterySymbol(b, symbolPos + new Vector2(symbolSize.X * 0.2f, symbolSize.Y * 0.1f), new Vector2(symbolSize.X * 0.6f, symbolSize.Y * 0.6f));
          Color chargeModeColor = blueColor;
          if (b.ChargeMode == ChargeMode.Discharge)
            chargeModeColor = redColor;
          if (b.ChargeMode == ChargeMode.Recharge)
            chargeModeColor = greenColor;
          DrawSquareTL(symbolPos + new Vector2(symbolSize.X * 0.2f, symbolSize.Y * 0.8f), new Vector2(symbolSize.X * 0.6f, symbolSize.Y * 0.05f), chargeModeColor, true);
        }
        else
          DrawBatterySymbol(b, symbolPos, symbolSize);
        counter = CheckIfLineFull(counter, symbolsPerLine, symbolSize, ref symbolPos);
      });
    }
    if (blocks.All(b => b is IMySolarPanel))
    {
      blocks.Where(b => b is IMySolarPanel).Select(b => b as IMySolarPanel).ToList().ForEach(b =>
      {
        counter++;
        if (main)
        {
          if (b.EntityId == selected)
          {
            DrawSquareTL(symbolPos, symbolSize, MainColor, false);
            selectedName = GetShownName(b);
          }
        DrawSolarSymbol(b, symbolPos + new Vector2(symbolSize.X * 0.1f, symbolSize.Y * 0.1f), new Vector2(symbolSize.X * 0.8f, symbolSize.Y * 0.8f));
        }
        else
          DrawSolarSymbol(b, symbolPos, symbolSize);
        counter = CheckIfLineFull(counter, symbolsPerLine, symbolSize, ref symbolPos);
      });
    }
    if (blocks.All(b => b is IMyReactor))
    {
      blocks.Where(b => b is IMyReactor).Select(b => b as IMyReactor).ToList().ForEach(b =>
      {
        counter++;
        if (main)
        {
          DrawReactorSymbol(b, symbolPos + new Vector2(symbolSize.X * 0.1f, symbolSize.Y * 0.1f), new Vector2(symbolSize.X * 0.8f, symbolSize.Y * 0.8f));
          if (b.UseConveyorSystem)
            DrawSquareCenter(symbolPos + new Vector2(symbolSize.X * 0.9f, symbolSize.Y / 2), new Vector2(symbolSize.X * 0.1f), reactorColor, true);
          if (b.EntityId == selected)
          {
            DrawSquareTL(symbolPos, symbolSize, MainColor, false);
            selectedName = GetShownName(b);
          }
      }
        else
          DrawReactorSymbol(b, symbolPos, symbolSize);
        counter = CheckIfLineFull(counter, symbolsPerLine, symbolSize, ref symbolPos);
      });
    }
    if (blocks.All(b => b.BlockDefinition.SubtypeName.Contains("WindTurbine")))
    {
      blocks.Where(b => b is IMyPowerProducer).Select(b => b as IMyPowerProducer).ToList().ForEach(b =>
      {
        counter++;
        if (main)
        {
          DrawWindTurbineSymbol(b, symbolPos + new Vector2(symbolSize.X * 0.1f, symbolSize.Y * 0.1f), new Vector2(symbolSize.X * 0.8f, symbolSize.Y * 0.8f));
          if (b.EntityId == selected)
          {
            DrawSquareTL(symbolPos, symbolSize, MainColor, false);
            selectedName = GetShownName(b);
          }
        }
        else
          DrawWindTurbineSymbol(b, symbolPos, symbolSize);
        counter = CheckIfLineFull(counter, symbolsPerLine, symbolSize, ref symbolPos);
      });
    }
    if (blocks.All(b => b.BlockDefinition.SubtypeName.Contains("HydrogenEngine")))
    {
      blocks.Where(b => b is IMyPowerProducer).Select(b => b as IMyPowerProducer).ToList().ForEach(b =>
      {
        HydrogenEngine engine;
        if (h2Engines.TryGetValue(b, out engine))
        {
          KeyValuePair<IMyPowerProducer, HydrogenEngine> pairedEngine = new KeyValuePair<IMyPowerProducer, HydrogenEngine>(b, engine);
          counter++;
          if (main)
          {
            DrawEngineSymbol(pairedEngine, symbolPos + new Vector2(symbolSize.X * 0.1f, symbolSize.Y * 0.1f), new Vector2(symbolSize.X * 0.8f, symbolSize.Y * 0.8f));
            if (b.EntityId == selected)
            {
              DrawSquareTL(symbolPos, symbolSize, MainColor, false);
              selectedName = GetShownName(b);
            }
          }
          else
            DrawEngineSymbol(pairedEngine, symbolPos, symbolSize);
          counter = CheckIfLineFull(counter, symbolsPerLine, symbolSize, ref symbolPos);
        }
      });
    }
    if (main && blocks.All(b => b is IMyBatteryBlock))
    {
      fontSize *= 0.5f;
      position.Y = SurfaceArea.Position.Y + drawAreaHeight + spacer;
      position.X = SurfaceArea.Position.X + (SurfaceArea.Width / 2);
      Write("Auto-Mode", blueColor, TextAlignment.CENTER);
      position.X += Surface.MeasureStringInPixels(new StringBuilder("Auto-Mode"), Font, fontSize).X;
      Write("Charge-Mode", greenColor, TextAlignment.LEFT);
      position.X -= Surface.MeasureStringInPixels(new StringBuilder("Auto-Mode"), Font, fontSize).X * 2;
      Write("Discharge-Mode", redColor, TextAlignment.RIGHT);
      fontSize /= 0.5f;
    }
    if (main && blocks.All(b => b is IMySolarPanel))
    {
      string solarText;
      Color statusColor;
      if (alignSolar)
      {
        solarText = EnDe("Solar Alignment: online");
        statusColor = greenColor;
      }
      else
      {
        solarText = EnDe("Solar Alignment: offline");
        statusColor = redColor;
      }
      fontSize *= 0.5f;
      position.Y = SurfaceArea.Position.Y + drawAreaHeight + spacer;
      position.X = SurfaceArea.Position.X + (SurfaceArea.Width / 2);
      Write(solarText, statusColor, TextAlignment.CENTER);
      fontSize /= 0.5f;
    }
    if (main && blocks.Any())
    {
      fontSize *= 0.75f;
      position.Y = SurfaceArea.Position.Y + SurfaceArea.Height - nameSize.Y;
      position.X = SurfaceArea.Position.X + (SurfaceArea.Width / 2);
      Write(selectedName, MainColor, TextAlignment.CENTER);
      fontSize /= 0.75f;
    }
  }

  int CheckIfLineFull(int counter, int symbolsPerLine, Vector2 symbolSize, ref Vector2 symbolPos)
  {
    symbolPos.X += symbolSize.X + spacer;
    if (counter == symbolsPerLine)
    {
      symbolPos.X = position.X;
      symbolPos.Y += symbolSize.Y + spacer;
      counter = 0;
    }
    return counter;
  }

  int CalculateSymbolsAlignment(RectangleF symbolsArea, ref Vector2 symbolSize, int symbolCount, out Vector2 startPos)
  {
    int symbolsPerLine = (int)((symbolsArea.Width - spacer) / (symbolSize.X + spacer));
    int lines = 1;
    float ratioXtoY = symbolSize.X / symbolSize.Y;
    while (symbolCount > symbolsPerLine * lines)
    {
      symbolSize.Y--; symbolSize.X = symbolSize.Y * ratioXtoY;
      symbolsPerLine = (int)((symbolsArea.Width - spacer) / (symbolSize.X + spacer));
      if ((symbolSize.Y * (lines + 1)) + spacer <= symbolsArea.Height)
        lines++;
    }
    symbolsPerLine = symbolCount / lines;
    if (symbolCount % lines != 0)
      symbolsPerLine++;
    float marginLeft = (symbolsArea.Width - (symbolSize.X * symbolsPerLine) - (spacer * (symbolsPerLine - 1))) / 2;
    float marginTop = (symbolsArea.Height - (symbolSize.Y * lines) - (spacer * (lines - 1))) / 2;
    startPos = symbolsArea.Position + new Vector2(marginLeft, marginTop);
    return symbolsPerLine;
  }

  public void GenerateSingleChart(IEnumerable<float> values, IEnumerable<float> maxValues, float max, Color deviceColor)
  {
    float labelingFormat = 0.8f;
    RectangleF chartArea = new RectangleF(
      new Vector2(spacer + offset.X, offset.Y + (SurfaceArea.Height / 2)),
      new Vector2(SurfaceArea.Width - (2 * spacer), (SurfaceArea.Height / 2) - spacer));
    Color statusColor = deviceColor;
    if (maxValues.Last() == 0)
      statusColor = colorOff;
    if (SurfaceArea.Width > 200)
    {
      position = chartArea.Position + new Vector2(chartArea.Width, 0);
      string yHigh = wattage(max);
      string yMax = wattage(maxValues.Last());
      string yCur = wattage(values.Last());
      string yLow = "0 W";
      fontSize *= labelingFormat;
      Vector2 yHighSize = Surface.MeasureStringInPixels(new StringBuilder(yHigh), Font, fontSize);
      Vector2 yMaxSize = Surface.MeasureStringInPixels(new StringBuilder(yMax), Font, fontSize);
      Vector2 yCurSize = Surface.MeasureStringInPixels(new StringBuilder(yCur), Font, fontSize);
      Vector2 yLowSize = Surface.MeasureStringInPixels(new StringBuilder(yLow), Font, fontSize);
      Write(yHigh, MainColor, TextAlignment.RIGHT);
      position.Y += (chartArea.Height / 2) - yMaxSize.Y - spacer;
      Write(yMax, statusColor, TextAlignment.RIGHT);
      position.Y += yMaxSize.Y + spacer - (spacer / 4); position.X -= yMaxSize.X;
      DrawSquareTL(position, new Vector2(yMaxSize.X, spacer / 2), statusColor, true);
      position.Y += (spacer / 2) + spacer; position.X += yMaxSize.X;
      Write(yCur, statusColor, TextAlignment.RIGHT);
      position.Y = chartArea.Position.Y + chartArea.Height - yLowSize.Y;
      Write(yLow, MainColor, TextAlignment.RIGHT);
      fontSize /= labelingFormat;
      chartArea.Width -= yHighSize.X * 1.5f;
    }
    position = chartArea.Position + new Vector2(0, chartArea.Height);
    DrawSquareTL(position - new Vector2(0, spacer / 2), new Vector2(chartArea.Width, spacer / 2), MainColor, true);
    DrawSquareTL(new Vector2(position.X + chartArea.Width - (spacer * 1.5f), position.Y - chartArea.Height), new Vector2(spacer / 2, chartArea.Height), MainColor, true);
    chartArea.Width -= spacer * 1.5f;
    chartArea.Height -= spacer * 0.5f;
    position = chartArea.Position + new Vector2(0, chartArea.Height);
    float barX = chartArea.Width / outputDiagramDepth;
    IEnumerator<float> maxValuePointer = maxValues.GetEnumerator();
    foreach (var v in values)
    {
      try
      {
        maxValuePointer.MoveNext();
        float heightY = chartArea.Height / max * maxValuePointer.Current;
        if (maxValuePointer.Current == 0)
          statusColor = colorOff;
        else
          statusColor = deviceColor;
        if (maxValuePointer.Current < max)
          DrawSquareTL(new Vector2(position.X, position.Y - heightY), new Vector2(barX, spacer / 4), statusColor, true);
      }
      catch { };
      float barY = chartArea.Height / max * v;
      DrawSquareTL(new Vector2(position.X, position.Y - barY), new Vector2(barX, barY), deviceColor, true);
      position.X += barX;
    }
  }

  public void GenerateDoubleChart(IEnumerable<float> values)
  {
    RectangleF chartArea = new RectangleF(
      new Vector2(spacer + offset.X, offset.Y + (SurfaceArea.Height / 2)),
      new Vector2(SurfaceArea.Width - (2 * spacer), (SurfaceArea.Height / 2) - spacer));
    while (maxBatteryLabeling < Math.Abs(values.Last()))
      maxBatteryLabeling += 1f;
    float labelingFormat = 0.8f;
    Color statusColor = redColor;
    if (values.Last() < 0)
      statusColor = greenColor;
    if (values.Last() == 0)
      statusColor = colorOff;
    float posYPos = chartArea.Position.Y + (chartArea.Height / 2) - (spacer / 4);
    float negYPos = chartArea.Position.Y + (chartArea.Height / 2) + (spacer / 4);
    if (SurfaceArea.Width > 200)
    {
      position = chartArea.Position + new Vector2(chartArea.Width, 0);
      string yHigh = wattage(-maxBatteryLabeling);
      string yMax = wattage(maxBatteryOutput);
      string yCur = wattage(-values.Last());
      string yLow = wattage(maxBatteryLabeling);
      fontSize *= labelingFormat;
      Vector2 yHighSize = Surface.MeasureStringInPixels(new StringBuilder(yHigh), Font, fontSize);
      Vector2 yMaxSize = Surface.MeasureStringInPixels(new StringBuilder(yMax), Font, fontSize);
      Vector2 yCurSize = Surface.MeasureStringInPixels(new StringBuilder(yCur), Font, fontSize);
      Vector2 yLowSize = Surface.MeasureStringInPixels(new StringBuilder(yLow), Font, fontSize);
      Write(yHigh, MainColor, TextAlignment.RIGHT);
      position.Y += (chartArea.Height / 2) - yMaxSize.Y - spacer;
      Write(yMax, statusColor, TextAlignment.RIGHT);
      position.Y = posYPos;
      DrawSquareTL(position - new Vector2(yMaxSize.X, 0), new Vector2(yMaxSize.X, spacer / 2), statusColor, true);
      position.Y += (spacer / 2) + spacer;
      Write(yCur, statusColor, TextAlignment.RIGHT);
      position.Y = chartArea.Position.Y + chartArea.Height - yLowSize.Y;
      Write(yLow, MainColor, TextAlignment.RIGHT);
      fontSize /= labelingFormat;
      chartArea.Width -= yMaxSize.X * 1.5f;
    }
    DrawSquareTL(new Vector2(chartArea.Position.X, posYPos), new Vector2(chartArea.Width, spacer / 2), MainColor, true);
    DrawSquareTL(chartArea.Position + new Vector2(chartArea.Width - (spacer * 1.5f), 0), new Vector2(spacer / 2, chartArea.Height), MainColor, true);
    chartArea.Width -= spacer * 1.5f;
    position = chartArea.Position + new Vector2(0, chartArea.Height / 2);
    float barX = chartArea.Width / outputDiagramDepth;
    foreach (var v in values)
    {
      if (v == 0)
        DrawSquareTL(new Vector2(position.X, posYPos), new Vector2(barX, spacer / 4), redColor, true);
      if (v < 0)
      {
        float barY = chartArea.Height / 2 / maxBatteryLabeling * Math.Abs(v);
        DrawSquareTL(new Vector2(position.X, posYPos - barY), new Vector2(barX, barY), greenColor, true);
      }
      else
      {
        float barY = chartArea.Height / 2 / maxBatteryLabeling * v;
        DrawSquareTL(new Vector2(position.X, negYPos), new Vector2(barX, barY), redColor, true);
      }
      position.X += barX;
    }
  }

  public void Write(string text, Color color, TextAlignment alignment) =>
    allSprites.Add(new MySprite(SpriteType.TEXT, text, position, null, color, Font, alignment, fontSize));

  void DrawLoadBar(float curLoad, float maxLoad, Func<float, string> unit, string symbol)
  {
    Color statusColor = greenColor;
    if (curLoad < maxLoad / 3 * 2)
      statusColor = new Color(65, 65, 0);
    if (curLoad < maxLoad / 3)
      statusColor = redColor;
    string max = unit(maxLoad);
    string cur = unit(curLoad);
    Vector2 maxSize = Surface.MeasureStringInPixels(new StringBuilder(max), Font, fontSize);
    Vector2 curSize = Surface.MeasureStringInPixels(new StringBuilder(cur), Font, fontSize);
    Vector2 symbolSize = new Vector2(curSize.Y + spacer);
    DrawSymbol(position, symbolSize.X, symbol, MainColor);
    Vector2 barSize = new Vector2(SurfaceArea.Width - symbolSize.X - maxSize.X - (4 * spacer), symbolSize.Y);
    position.X += symbolSize.X + spacer;
    DrawHorizontalBar(position, barSize, statusColor, curLoad / maxLoad);
    position.X = SurfaceArea.Position.X + SurfaceArea.Width - spacer;
    position.Y += (symbolSize.Y / 2) - (maxSize.Y / 2);
    Write(max, MainColor, TextAlignment.RIGHT);
    position.X = SurfaceArea.Position.X + symbolSize.X + (barSize.X / 2);
    Write(cur, new Color(25, 25, 25), TextAlignment.CENTER);
    position.X = SurfaceArea.Position.X + spacer;
    position.Y += maxSize.Y + (spacer * 1.5f);
  }

  void DrawSolarSymbol(IMySolarPanel panel, Vector2 posTL, Vector2 size) =>
    DrawSolarSymbol(panel.IsWorking, panel.CurrentOutput / maxSolarOutputPerPanel, posTL, size);

  void DrawSolarSymbol(bool working, float efficiency, Vector2 posTL, Vector2 size)
  {
    Color statusColor = MainColor;
    if (working)
      statusColor = greenColor;
    else
      statusColor = redColor;
    float line = size.X * 0.05f;
    DrawSquareTL(posTL, size, statusColor, false);
    Vector2 posCenter = new Vector2(posTL.X + (size.X / 2), posTL.Y + (size.Y / 2));
    size.X *= 0.8f; size.Y *= 0.8f;
    DrawSquareCenter(new Vector2(posCenter.X, posCenter.Y - (size.Y / 2) + (line / 2)), new Vector2(size.X, line), solarColor, true);
    DrawSquareCenter(new Vector2(posCenter.X, posCenter.Y + (size.Y / 2) - (line / 2)), new Vector2(size.X, line), solarColor, true);
    DrawSquareCenter(new Vector2(posCenter.X - (size.X / 2) + (line / 2), posCenter.Y), new Vector2(line, size.Y), solarColor, true);
    DrawSquareCenter(new Vector2(posCenter.X + (size.X / 2) - (line / 2), posCenter.Y), new Vector2(line, size.Y), solarColor, true);
    size.X -= line; size.Y -= line;
    size.X *= efficiency; size.Y *= efficiency;
    DrawSquareCenter(posCenter, size, solarColor, true);
  }

  void DrawEngineSymbol(KeyValuePair<IMyPowerProducer, HydrogenEngine> engine, Vector2 posTL, Vector2 size) =>
    DrawEngineSymbol(engine.Key.IsWorking, engine.Value.FilledRatio, posTL, size);

  void DrawEngineSymbol(bool working, float loadPercentage, Vector2 posTL, Vector2 size)
  {
    Color statusColor = MainColor;
    if (working)
      statusColor = greenColor;
    else
      statusColor = redColor;
    DrawSquareTL(posTL, new Vector2(size.X * 0.1f, size.Y), statusColor, true);
    posTL += new Vector2(size.X * 0.1f, size.Y / 3);
    DrawSquareTL(posTL, new Vector2(size.X * 0.05f, size.Y / 3), statusColor, true);
    posTL.X += size.X * 0.05f; size.X *= 0.85f;
    posTL.Y -= size.Y / 3;
    Vector2 pimpleSize = new Vector2(size.X / 11, size.Y * 0.1f);
    Vector2 pimplePos = posTL + new Vector2(pimpleSize.X, 0);
    for (int i = 0; i < 5; i++)
    {
      DrawSquareTL(pimplePos, pimpleSize, statusColor, true);
      pimplePos.X += pimpleSize.X * 2;
    }
    posTL.Y += pimpleSize.Y; size.Y -= pimpleSize.Y;
    DrawSquareTL(posTL, new Vector2(size.X * 0.05f, size.Y), statusColor, true);
    DrawSquareTL(posTL, new Vector2(size.X, size.Y * 0.1f), statusColor, true);
    DrawSquareTL(posTL + new Vector2(size.X - (size.X * 0.05f), 0), new Vector2(size.X * 0.05f, size.Y), statusColor, true);
    DrawSquareTL(posTL + new Vector2(0, size.Y - (size.Y * 0.1f)), new Vector2(size.X, size.Y * 0.1f), statusColor, true);
    posTL += new Vector2(size.X * 0.1f, size.Y * 0.2f);
    size.X -= size.X * 0.2f; size.Y -= size.Y * 0.4f;
    float line = size.Y * 0.1f;
    DrawSquareTL(posTL, new Vector2(size.X, line), h2Color, true);
    DrawSquareTL(posTL, new Vector2(line, size.Y), h2Color, true);
    DrawSquareTL(posTL + new Vector2(0, size.Y - line), new Vector2(size.X, line), h2Color, true);
    DrawSquareTL(posTL + new Vector2(size.X - line, 0), new Vector2(line, size.Y), h2Color, true);
    posTL += new Vector2(line); size.X -= 2 * line; size.Y -= 2 * line;
    size.X *= loadPercentage;
    DrawSquareTL(posTL, size, h2Color, true);
  }

  void DrawReactorSymbol(IMyReactor reactor, Vector2 posTL, Vector2 size) =>
    DrawReactorSymbol(reactor.IsWorking, (float)reactor.GetInventory(0).CurrentVolume / (float)reactor.GetInventory(0).MaxVolume, posTL, size);

  void DrawReactorSymbol(bool working, float percentrage, Vector2 posTL, Vector2 size)
  {
    Color statusColor = MainColor;
    if (working)
      statusColor = greenColor;
    else
      statusColor = redColor;
    RectangleF logo = new RectangleF(posTL + new Vector2(0, size.Y * 0.25f), new Vector2(size.X * 0.75f, size.Y * 0.75f));
    RectangleF bar = new RectangleF(posTL + new Vector2(size.X * 0.75f, 0), new Vector2(size.X * 0.25f, size.Y));
    Vector2 posCenter = logo.Position + new Vector2(logo.Width / 2, logo.Height / 2);
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", posCenter, logo.Size, statusColor, null, TextAlignment.CENTER));
    DrawSquareTL(logo.Position + new Vector2(0, logo.Height / 2), new Vector2(logo.Width, logo.Height / 2), statusColor, true);
    Vector2 radioactiv = new Vector2(logo.Width * 0.9f, logo.Height * 0.9f);
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(posCenter.X, posCenter.Y - (logo.Height * 0.05f)), radioactiv, reactorColor, null, TextAlignment.CENTER));
    posCenter.Y += logo.Height * 0.15f;
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", posCenter, new Vector2(logo.Width * 0.4f, logo.Height * 0.4f), Color.Black, null, TextAlignment.CENTER));
    float triangleR = logo.Width * 0.1f;
    Vector2 posTopTri = posCenter - new Vector2(0, triangleR);
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Triangle", posTopTri, new Vector2(logo.Width * 0.25f, logo.Height * 0.25f), reactorColor, null, TextAlignment.CENTER, (float)Math.PI));
    Vector2 posLeftTri = new Vector2(posCenter.X - (float)(Math.Cos(Math.PI * 2 * 30 / 360) * triangleR), posCenter.Y + (float)(Math.Sin(Math.PI * 2 * 30 / 360) * triangleR));
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Triangle", posLeftTri, new Vector2(logo.Width * 0.25f, logo.Height * 0.25f), reactorColor, null, TextAlignment.CENTER, (float)Math.PI / 3));
    Vector2 posRightTri = new Vector2(posCenter.X + (float)(Math.Cos(Math.PI * 2 * 30 / 360) * triangleR), posCenter.Y + (float)(Math.Sin(Math.PI * 2 * 120 / 360) * triangleR));
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Triangle", posRightTri, new Vector2(logo.Width * 0.25f, logo.Height * 0.25f), reactorColor, null, TextAlignment.CENTER, (float)Math.PI / -3));
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", posCenter, new Vector2(logo.Width * 0.2f, logo.Height * 0.2f), reactorColor, null, TextAlignment.CENTER));
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", posCenter, new Vector2(logo.Width * 0.15f, logo.Height * 0.15f), Color.Black, null, TextAlignment.CENTER));
    float line = bar.Width * 0.2f;
    DrawSquareTL(bar.Position, new Vector2(line, bar.Height), statusColor, true);
    DrawSquareTL(bar.Position, new Vector2(bar.Width, line), statusColor, true);
    DrawSquareTL(bar.Position + new Vector2(0, bar.Height - line), new Vector2(bar.Width, line), statusColor, true);
    DrawSquareTL(bar.Position + new Vector2(bar.Width - line, 0), new Vector2(line, bar.Height), statusColor, true);
    Vector2 posBarBL = bar.Position + new Vector2(line, bar.Height - line);
    DrawSquareTL(posBarBL, new Vector2(bar.Width - (2 * line), (bar.Height - (2 * line)) * -percentrage), reactorColor, true);
  }

  void DrawWindTurbineSymbol(IMyPowerProducer turbine, Vector2 posTL, Vector2 size) =>
    DrawWindTurbineSymbol(turbine.IsWorking, turbine.CurrentOutput / turbine.MaxOutput, posTL, size);

  void DrawWindTurbineSymbol(bool working, float percentage, Vector2 posTL, Vector2 size)
  {
    Color statusColor = MainColor;
    if (working)
      statusColor = greenColor;
    else
      statusColor = redColor;
    Vector2 posCenter = new Vector2(posTL.X + (size.X / 2), posTL.Y + (size.Y / 2));
    Vector2 bladeSize = new Vector2(size.X * 0.2f * percentage, size.Y * 0.425f * percentage);
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "CircleHollow", posCenter, size, statusColor, null, TextAlignment.CENTER));
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", posCenter, new Vector2(size.X * 0.15f, size.Y * 0.15f), statusColor, null, TextAlignment.CENTER));
    float rBladeRing = size.Y * 0.2125f;
    Vector2 posTopTri = posCenter - new Vector2(0, rBladeRing);
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Triangle", posTopTri, bladeSize, statusColor, null, TextAlignment.CENTER, (float)Math.PI));
    Vector2 posLeftTri = new Vector2(posCenter.X - (float)(Math.Cos(Math.PI * 2 * 30 / 360) * rBladeRing), posCenter.Y + (float)(Math.Sin(Math.PI * 2 * 30 / 360) * rBladeRing));
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Triangle", posLeftTri, bladeSize, statusColor, null, TextAlignment.CENTER, (float)Math.PI / 3));
    Vector2 posRightTri = new Vector2(posCenter.X + (float)(Math.Cos(Math.PI * 2 * 30 / 360) * rBladeRing), posCenter.Y + (float)(Math.Sin(Math.PI * 2 * 30 / 360) * rBladeRing));
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "Triangle", posRightTri, bladeSize, statusColor, null, TextAlignment.CENTER, (float)Math.PI / -3));
  }

  void DrawBatterySymbol(IMyBatteryBlock battery, Vector2 posTL, Vector2 size) =>
    DrawBatterySymbol(battery.IsWorking, battery.IsCharging, battery.CurrentStoredPower / (battery.MaxStoredPower * 0.99f), posTL, size);

  void DrawBatterySymbol(bool working, bool charging, float charge, Vector2 posTL, Vector2 size)
  {
    Color statusColor = MainColor;
    if (working)
      statusColor = greenColor;
    else
      statusColor = redColor;
    float line = size.Y * 0.1f;
    DrawSquareTL(posTL + new Vector2(size.X / 3, 0), new Vector2(size.X / 3, line), statusColor, true);
    posTL.Y += line; size.Y -= line;
    DrawSquareTL(posTL, size, statusColor, false);
    Vector2 barSize = new Vector2(size.X - (4 * line), size.Y / 9);
    posTL.X += 2 * line; posTL.Y += size.Y - (barSize.Y * 2);
    int bars = ((int)(charge * 100)) / 25;
    if (charging)
      statusColor = greenColor;
    else
      statusColor = redColor;
    for (int i = 0; i < bars; i++)
    {
      DrawSquareTL(posTL, barSize, statusColor, true);
      posTL.Y -= 2 * barSize.Y;
    }
  }

  void DrawHorizontalBar(Vector2 pos, Vector2 size, Color fillingColor, params float[] percentages)
  {
    float line = size.Y * 0.1f;
    DrawSquareTL(pos, new Vector2(size.X, line), MainColor, true);
    DrawSquareTL(pos, new Vector2(line, size.Y), MainColor, true);
    DrawSquareTL(pos + new Vector2(size.X - line, 0), new Vector2(line, size.Y), MainColor, true);
    DrawSquareTL(pos + new Vector2(0, size.Y - line), new Vector2(size.X, line), MainColor, true);
    pos += new Vector2(line); size.X -= 2 * line; size.Y -= 2 * line;
    if (percentages.Length > 0)
      if (percentages[0] >= 0)
        DrawSquareTL(pos, new Vector2(size.X * percentages[0], size.Y), fillingColor, true);
      else
        DrawSquareTL(pos + new Vector2(0, size.Y / 3), new Vector2(size.X * -percentages[0], size.Y / 3), fillingColor, true);
    if (percentages.Length > 1)
      DrawSquareTL(pos + new Vector2(size.X * Math.Abs(percentages[1]), 0), new Vector2(line, size.Y), MainColor, true);
  }

  void DrawSquareTL(Vector2 pos, Vector2 size, Color color, bool filled)
  {
    Vector2 center = new Vector2(pos.X + (size.X / 2), pos.Y + (size.Y / 2));
    DrawSquareCenter(center, size, color, filled);
  }

  void DrawSquareCenter(Vector2 pos, Vector2 size, Color color, bool filled)
  {
    if (filled)
      allSprites.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", pos, size, color, null, TextAlignment.CENTER));
    else
      allSprites.Add(new MySprite(SpriteType.TEXTURE, "SquareHollow", pos, size, color, null, TextAlignment.CENTER));
  }

  void DrawSymbol(Vector2 pos, float size, string symbol, Color color) =>
    allSprites.Add(new MySprite(SpriteType.TEXTURE, symbol, new Vector2(pos.X + (size / 2), pos.Y + (size / 2)), new Vector2(size), color, null, TextAlignment.CENTER));

  void DrawSeperationLine(float Y) =>
    allSprites.Add(new MySprite(SpriteType.TEXTURE, "SquareTapered",
      new Vector2(SurfaceArea.Center.X, Y), new Vector2(SurfaceArea.Width - (2 * spacer), spacer / 2), MainColor));

  public void SetSpacer(float spacerDiv)
  {
    if (SurfaceArea.Width <= SurfaceArea.Height)
      spacer = SurfaceArea.Width / spacerDiv;
    else
      spacer = SurfaceArea.Height / spacerDiv;
  }

  public void DrawLayout()
  {
    Frame.Add(background);
    allSprites.ForEach(s => Frame.Add(s));
    Frame.Dispose();
  }
}

class SolarRotor
{
  public IMyMotorStator Rotor { get; }
  public float SunrisePos { get; set; }
  bool inSunrisePos = true;
  float sunsetPos;
  public int MoveFromSunrise { get; set; }
  public float Position
  {
    get
    {
      return (float)(Rotor.Angle * 180 / Math.PI);
    }
  }
  public float LowerLimit { get; }
  public float UpperLimit { get; }

  public SolarRotor(IMyMotorStator rotor)
  {
    Rotor = rotor;
    sunsetPos = SunrisePos = (float)(rotor.Angle * 180 / Math.PI);
    MoveFromSunrise = 1;
    if (rotor.CustomName.Contains("Inv"))
      MoveFromSunrise *= -1;
    LowerLimit = rotor.LowerLimitDeg;
    UpperLimit = rotor.UpperLimitDeg;
    rotor.Torque = 33599988;
    rotor.BrakingTorque = 33599988;
    rotor.TargetVelocityRPM = 0;
    rotor.RotorLock = false;
  }

  public void RotateToSunrise(float speedRPM)
  {
    speedRPM = Math.Abs(speedRPM);
    Rotor.TargetVelocityRPM = 0;
    if (!inSunrisePos)
    {
      if (SunrisePos <= sunsetPos)
      {
        if (SunrisePos > LowerLimit)
          Rotor.LowerLimitDeg = SunrisePos;
        Rotor.TargetVelocityRPM = -speedRPM;
      }
      if (SunrisePos > sunsetPos)
      {
        if (SunrisePos > UpperLimit)
          Rotor.UpperLimitDeg = SunrisePos;
        Rotor.TargetVelocityRPM = speedRPM;
      }
      if (SunrisePos == Position)
      {
        Rotor.TargetVelocityRPM = 0f;
        Rotor.LowerLimitDeg = LowerLimit;
        Rotor.UpperLimitDeg = UpperLimit;
        inSunrisePos = true;
      }
    }
  }

  public void RotateAlongSun(float speedRPM)
  {
    inSunrisePos = false;
    Rotor.TargetVelocityRPM = speedRPM * MoveFromSunrise;
  }

  public void StopRotation() => Rotor.TargetVelocityRPM = 0f;

  public void ChangeDirection() => MoveFromSunrise *= -1;

  public void Sunset() => sunsetPos = (float)(Rotor.Angle * 180 / Math.PI);

}

class SolarMast
{
  public string MastName { get; }
  List<IMySolarPanel> panels;
  List<SolarRotor> angleRotors;
  SolarRotor rotationRotor = null;
  float maxOutput;
  float lastOutputMoving;
  float lastOutputFixed;
  float curOutput;
  bool changeAngle = false;
  bool rotate = false;
  const float sunMovementSpeed = 0.1f;
  int runsAfterDirectionChange = 0;
  bool changedDirection = false;
  string statusInfo;
  bool switchedToSunrise = true;
  bool allRotorsStoped = true;
  int rotorIndex = 0;

  public SolarMast(string mastName, List<IMySolarPanel> panels, List<IMyMotorStator> rotors)
  {
    MastName = mastName;
    this.panels = panels;
    maxOutput = panels.Max(p => p.MaxOutput) * panels.Count;
    curOutput = panels.Sum(p => p.MaxOutput);
    lastOutputMoving = lastOutputFixed = curOutput;
    if (rotors.Any())
    {
      angleRotors = rotors.Where(r => r.CustomName.Contains("Angle")).Select(r => new SolarRotor(r)).ToList<SolarRotor>();
      if (rotors.Any(r => r.CustomName.Contains("Rotat")))
        rotationRotor = new SolarRotor(rotors.First(r => r.CustomName.Contains("Rotat")));
    }
  }

  public string MoveAlongSun()
  {
    if (rotationRotor == null && !angleRotors.Any())
    {
      return "no rotors found";
    }
    statusInfo = "no alignment needed";
    curOutput = panels.Sum(p => p.MaxOutput);
    float curMaxOutput = panels.Max(p => p.MaxOutput) * panels.Count;
    if (curMaxOutput > maxOutput)
      maxOutput = curMaxOutput;
    if (curOutput > (maxOutput * 0.1f))
    {
      if (curOutput < (lastOutputFixed * 0.998f) && !rotate && !changeAngle)
      {
        lastOutputMoving = curOutput;
        statusInfo = "starting alignment...";
        runsAfterDirectionChange = 0;
        changedDirection = false;
        if (rotationRotor != null)
          rotate = true;
        else
          changeAngle = true;
        rotorIndex = 0;
      }
      if (rotate)
      {
        statusInfo = "rotating solar panels...";
        if (rotorIndex < 1)
          RotateRotorAlongSun(rotationRotor);
        else
        {
          rotate = false;
          changeAngle = true;
          rotorIndex = 0;
          lastOutputMoving = curOutput;
          changedDirection = false;
          runsAfterDirectionChange = 0;
        }
      }
      if (changeAngle)
      {
        statusInfo = "align solar panels angle...";
        if (rotorIndex < angleRotors.Count)
          RotateRotorAlongSun(angleRotors[rotorIndex]);
        else
        {
          changeAngle = false;
          rotorIndex = 0;
          lastOutputMoving = curOutput;
          changedDirection = false;
          runsAfterDirectionChange = 0;
        }
      }
      if (curOutput >= lastOutputFixed && !rotate && !changeAngle)
      {
        if (rotationRotor != null) rotationRotor.StopRotation();
        angleRotors.ForEach(r => r.StopRotation());
        rotate = changeAngle = false;
        lastOutputFixed = lastOutputMoving = curOutput;
        statusInfo = "solar panels aligned to the sun";
      }
    }
    else
    {
      rotate = changeAngle = false;
      angleRotors.ForEach(r => r.StopRotation());
      if (rotationRotor != null)
      {
        rotationRotor.StopRotation();
        RotateToSunrise();
      }
      statusInfo = "night mode - aligned to sunrise";
    }
    return statusInfo;
  }

  public string StopAllAlignment()
  {
    if (!allRotorsStoped)
    {
      if (rotationRotor != null) rotationRotor.StopRotation();
      angleRotors.ForEach(r => r.StopRotation());
      runsAfterDirectionChange = 0;
      allRotorsStoped = true;
    }
    return "solar alignment offline";
  }

  void RotateRotorAlongSun(SolarRotor sRotor)
  {
    switchedToSunrise = false;
    allRotorsStoped = false;
    sRotor.RotateAlongSun(sunMovementSpeed);
    if (runsAfterDirectionChange >= 2 && curOutput < lastOutputMoving && !changedDirection)
    {
      sRotor.ChangeDirection();
      changedDirection = true;
      runsAfterDirectionChange = -1;
      lastOutputMoving = curOutput;
    }
    if (runsAfterDirectionChange >= 2 && curOutput >= lastOutputMoving)
    {
      lastOutputMoving = curOutput;
    }
    if ( (curOutput < lastOutputMoving && changedDirection) || (sRotor.Position == sRotor.LowerLimit || sRotor.Position == sRotor.UpperLimit) )
    {
      sRotor.StopRotation();
      rotorIndex++;
      lastOutputMoving = curOutput;
      runsAfterDirectionChange = -1;
      changedDirection = false;
    }
    runsAfterDirectionChange++;
  }

  void RotateToSunrise()
  {
    if (!switchedToSunrise)
    {
      switchedToSunrise = true;
      rotationRotor.Sunset();
      if (rotationRotor.Position > 0)
        rotationRotor.SunrisePos = rotationRotor.Position - 180f;
      else
        rotationRotor.SunrisePos = rotationRotor.Position + 180f;
    }
    rotationRotor.RotateToSunrise(1.0f);
  }
}