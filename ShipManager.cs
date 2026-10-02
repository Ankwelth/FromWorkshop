// Generated from ZerothAngel's SEScripts version e2534e33e23e
// Modules: shipmanager, customdata, split, reactormanager, damagecontrol, dockingaction, redundancy, dockinghandler, productionmanager, refinerymanager, airventmanager, oxygenmanager, timerblockutils, complexairlock, simpleairlock, doorautocloser, eventdriver, commons

// MIT licensed. See https://github.com/ZerothAngel/SEScripts for raw code.

// !!! Leading whitespace stripped to save bytes !!!

// Begin configuration

// Module enable/disable

// You can either edit the following to enable/disable modules or add the
// appropriate line to the prog block's Custom Data (see comment preceding
// each option). The Custom Data method is preferred since it makes upgrading
// the script easier.

// "autoCloseDoors no"
const bool AUTO_CLOSE_DOORS_ENABLE = true;
// "simpleAirlock no"
const bool SIMPLE_AIRLOCK_ENABLE = true;
// "complexAirlock no"
const bool COMPLEX_AIRLOCK_ENABLE = true;
// "oxygenManager no"
const bool OXYGEN_MANAGER_ENABLE = true;
// Air vent manager no longer compatible with complex airlocks as of 1.185.
// Only enable one or the other or neither.
// "airVentManager yes"
const bool AIR_VENT_MANAGER_ENABLE = false;
// "refineryManager no"
const bool REFINERY_MANAGER_ENABLE = true;
// "productionManager yes"
const bool PRODUCTION_MANAGER_ENABLE = false;
// "redundancyManager no"
const bool REDUNDANCY_MANAGER_ENABLE = true;
// "dockingAction no"
const bool DOCKING_ACTION_ENABLE = true;
// "damageControl no"
const bool DAMAGE_CONTROL_ENABLE = true;
// "reactorManager no"
const bool REACTOR_MANAGER_ENABLE = true;

// Options

// DockingAction
const string DOCKING_ACTION_PREFIX = "DockingAction";

// RedundancyManager
const string REDUNDANCY_PREFIX = "Redundant";

// ProductionManager
const bool LIMIT_PRODUCTION_MANAGER_SAME_GRID = true;
const bool PRODUCTION_MANAGER_SETUP = false;
// You can, of course, edit item counts after they've
// been set up. But this multiplier is here for convenience.
// With a 1.0 multiplier, the volume of all components
// will be 2,665,600 liters. This is roughly 6.3 large
// grid large cargo containers at the 1X inventory world
// setting.
const float PRODUCTION_MANAGER_SETUP_AMOUNT_MULTIPLIER = 1.0f;

// RefineryManager
const string REFINERY_MANAGER_PREFIX = "Refineries";

// AirVentManager
const float MIN_AIR_VENT_PRESSURE = 0.9f;

// OxygenManager
const double MIN_OXYGEN_TANK_LEVEL = 0.7;
const double MAX_OXYGEN_TANK_LEVEL = 0.9;
const double LOW_OXYGEN_TANK_LEVEL = 0.5;
const string LOW_OXYGEN_NAME = "Low Oxygen";

// SimpleAirlock
const string SIMPLE_AIRLOCK_GROUP_PREFIX = "SimpleAirlock";

// DoorAutoCloser
const double DEFAULT_DOOR_OPEN_DURATION = 3.0; // In seconds
const string DOOR_AUTO_CLOSER_PREFIX = "AutoClose";

// End configuration

// !!! CODE BEGINS HERE, EDIT AT YOUR OWN RISK !!!

private readonly EventDriver eventDriver = new EventDriver();
private readonly DoorAutoCloser doorAutoCloser = new DoorAutoCloser();
private readonly SimpleAirlock simpleAirlock = new SimpleAirlock();
private readonly ComplexAirlock complexAirlock = new ComplexAirlock();
private readonly OxygenManager oxygenManager = new OxygenManager();
private readonly AirVentManager airVentManager = new AirVentManager();
private readonly RefineryManager refineryManager = new RefineryManager();
private readonly ProductionManager productionManager = new ProductionManager();
private readonly RedundancyManager redundancyManager = new RedundancyManager();
private readonly DockingAction dockingAction = new DockingAction();
private readonly DamageControl damageControl = new DamageControl();
private readonly ReactorManager reactorManager = new ReactorManager();
private readonly ZAStorage myStorage = new ZAStorage();
private readonly ZACustomData customData = new ZACustomData();
private bool FirstRun = true;
private bool AutoCloseDoorsEnable, SimpleAirlockEnable, ComplexAirlockEnable;
private bool OxygenManagerEnable, AirVentManagerEnable, RefineryManagerEnable;
private bool ProductionManagerEnable, RedundancyManagerEnable;
private bool DockingActionEnable, DamageControlEnable, ReactorManagerEnable;
Program()
{
Runtime.UpdateFrequency |= UpdateFrequency.Once;
}
void Main(string argument, UpdateType updateType)
{
var commons = new ZACommons(this, updateType,
storage: myStorage);
if (FirstRun)
{
FirstRun = false;
customData.Parse(Me);
AutoCloseDoorsEnable = customData.GetBool("autoCloseDoors", AUTO_CLOSE_DOORS_ENABLE);
SimpleAirlockEnable = customData.GetBool("simpleAirlock", SIMPLE_AIRLOCK_ENABLE);
ComplexAirlockEnable = customData.GetBool("complexAirlock", COMPLEX_AIRLOCK_ENABLE);
OxygenManagerEnable = customData.GetBool("oxygenManager", OXYGEN_MANAGER_ENABLE);
AirVentManagerEnable = customData.GetBool("airVentManager", AIR_VENT_MANAGER_ENABLE);
RefineryManagerEnable = customData.GetBool("refineryManager", REFINERY_MANAGER_ENABLE);
ProductionManagerEnable = customData.GetBool("productionManager", PRODUCTION_MANAGER_ENABLE);
RedundancyManagerEnable = customData.GetBool("redundancyManager", REDUNDANCY_MANAGER_ENABLE);
DockingActionEnable = customData.GetBool("dockingAction", DOCKING_ACTION_ENABLE);
DamageControlEnable = customData.GetBool("damageControl", DAMAGE_CONTROL_ENABLE);
ReactorManagerEnable = customData.GetBool("reactorManager", REACTOR_MANAGER_ENABLE);
myStorage.Decode(Storage);
if (AutoCloseDoorsEnable) doorAutoCloser.Init(commons, eventDriver);
if (SimpleAirlockEnable) simpleAirlock.Init(commons, eventDriver);
if (ComplexAirlockEnable) complexAirlock.Init(commons, eventDriver);
if (OxygenManagerEnable) oxygenManager.Init(commons, eventDriver);
if (AirVentManagerEnable) airVentManager.Init(commons, eventDriver);
if (RefineryManagerEnable) refineryManager.Init(commons, eventDriver);
if (ProductionManagerEnable) productionManager.Init(commons, eventDriver);
if (RedundancyManagerEnable) redundancyManager.Init(commons, eventDriver);
if (DockingActionEnable) dockingAction.Init(commons, eventDriver);
if (DamageControlEnable) damageControl.Init(commons, eventDriver);
if (ReactorManagerEnable) reactorManager.Init(commons, eventDriver);
}
eventDriver.Tick(commons, argAction: () => {
if (ComplexAirlockEnable) complexAirlock.HandleCommand(commons, eventDriver, argument);
if (ProductionManagerEnable) productionManager.HandleCommand(commons, eventDriver, argument);
if (DamageControlEnable) damageControl.HandleCommand(commons, eventDriver, argument);
if (ReactorManagerEnable) reactorManager.HandleCommand(commons, eventDriver, argument);
},
postAction: () => {
if (ProductionManagerEnable) productionManager.Display(commons);
if (DamageControlEnable) damageControl.Display(commons);
});
if (commons.IsDirty) Storage = myStorage.Encode();
}

public class ZACustomData
{
private Dictionary<string, string> Data = new Dictionary<string, string>();
public void Parse(IMyTerminalBlock block)
{
Data.Clear();
var lines = System.Text.RegularExpressions.Regex.Split(block.CustomData, "\r\n|\r|\n");
foreach (var line in lines)
{
var trimmed = line.Trim();
if (trimmed.Length == 0) continue;
var tokens = ZASplit.Split(trimmed);
if (tokens.Count != 2)
{
throw new Exception(string.Format("Invalid CustomData: {0}", trimmed));
}
Data[tokens[0].ToLower()] = tokens[1];
}
}
public string GetString(string key, string def = "")
{
string value;
if (!Data.TryGetValue(key.ToLower(), out value))
{
value = def;
}
return value;
}
public int GetInt(string key, int def = 0)
{
int value = def;
string str;
if (Data.TryGetValue(key.ToLower(), out str))
{
if (!int.TryParse(str, out value)) throw new Exception(string.Format("Invalid CustomData int: {0}", str));
}
return value;
}
public double GetDouble(string key, double def = 0.0)
{
double value = def;
string str;
if (Data.TryGetValue(key.ToLower(), out str))
{
if (!double.TryParse(str, out value)) throw new Exception(string.Format("Invalid CustomData double: {0}", str));
}
return value;
}
public bool GetBool(string key, bool def = false)
{
bool value = def;
string str;
if (Data.TryGetValue(key.ToLower(), out str))
{
switch (str.ToLower())
{
case "t":
case "true":
case "y":
case "yes":
{
value = true;
break;
}
case "f":
case "false":
case "n":
case "no":
{
value = false;
break;
}
default:
throw new Exception(string.Format("Invalid CustomData bool: {0}", str));
}
}
return value;
}
public Base6Directions.Direction GetDirection(string key, Base6Directions.Direction def = Base6Directions.Direction.Forward)
{
Base6Directions.Direction value = def;
string str;
if (Data.TryGetValue(key.ToLower(), out str))
{
switch (str.ToLower())
{
case "forward":
case "forwards":
{
value = Base6Directions.Direction.Forward;
break;
}
case "backward":
case "backwards":
{
value = Base6Directions.Direction.Backward;
break;
}
case "left":
{
value = Base6Directions.Direction.Left;
break;
}
case "right":
{
value = Base6Directions.Direction.Right;
break;
}
case "up":
{
value = Base6Directions.Direction.Up;
break;
}
case "down":
{
value = Base6Directions.Direction.Down;
break;
}
default:
throw new Exception(string.Format("Invalid CustomData direction: {0}", str));
}
}
return value;
}
}

public static class ZASplit
{
enum States { Normal, Escaped, Quoted, QuotedEscaped };
public static List<string> Split(string input, bool complete = true)
{
var result = new List<string>();
var state = States.Normal;
var current = new StringBuilder();
foreach (var c in input)
{
switch (state)
{
case States.Normal:
if (c == '\\') state = States.Escaped;
else if (c == '"') state = States.Quoted;
else
{
if (current.Length == 0)
{
if (!Char.IsWhiteSpace(c)) current.Append(c);
}
else if (Char.IsWhiteSpace(c))
{
result.Add(current.ToString());
current = new StringBuilder();
}
else current.Append(c);
}
break;
case States.Escaped:
case States.QuotedEscaped:
if (c == '\\') current.Append('\\');
else if (c == '"') current.Append('"');
else
{
current.Append('\\');
current.Append(c);
}
state = state == States.Escaped ? States.Normal : States.Quoted;
break;
case States.Quoted:
if (c == '\\') state = States.QuotedEscaped;
else if (c == '"') state = States.Normal;
else current.Append(c);
break;
}
}
if (complete && (state == States.Quoted || state == States.QuotedEscaped))
{
throw new Exception("Unterminated quote");
}
if (state == States.Escaped) current.Append('\\');
if (current.Length > 0) result.Add(current.ToString());
return result;
}
}

public class ReactorManager
{
private const double RunDelay = 5.0;
private bool? State = null;
private int ConnectorCount = 0;
public void Init(ZACommons commons, EventDriver eventDriver)
{
State = null;
ConnectorCount = 0;
eventDriver.Schedule(0.0, Run);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
var myConnectors = ZACommons.GetBlocksOfType<IMyShipConnector>(commons.Blocks,
block => block.DefinitionDisplayNameText == "Connector" &&
((IMyShipConnector)block).Status == MyShipConnectorStatus.Connected);
var currentConnectorCount = myConnectors.Count;
if (currentConnectorCount > ConnectorCount)
{
State = null;
}
ConnectorCount = currentConnectorCount;
var fueledPower = GetFueledPowerProducers(commons,
block => block.CubeGrid == commons.Me.CubeGrid && block is IMyFunctionalBlock && ((IMyFunctionalBlock)block).IsWorking);
var currentState = fueledPower.Count > 0;
if (State == null || currentState != (bool)State)
{
State = currentState;
if (!(bool)State)
{
ZACommons.EnableBlocks(GetFueledPowerProducers(commons, block => block.CubeGrid != commons.Me.CubeGrid), false);
}
}
eventDriver.Schedule(RunDelay, Run);
}
public void HandleCommand(ZACommons commons, EventDriver eventDriver,
string argument)
{
var command = argument.Trim().ToLower();
switch (command)
{
case "reactors":
{
ZACommons.EnableBlocks(GetFueledPowerProducers(commons), true);
eventDriver.Schedule(1.0, (c,ed) => {
GetBatteries(c).ForEach(block => block.Enabled = false);
});
break;
}
case "batteries":
{
GetBatteries(commons).ForEach(block =>
{
block.Enabled = true;
block.ChargeMode = ChargeMode.Auto;
});
eventDriver.Schedule(1.0, (c,ed) => {
ZACommons.EnableBlocks(GetFueledPowerProducers(c), false);
});
break;
}
}
}
private static List<IMyTerminalBlock> GetFueledPowerProducers(ZACommons commons, Func<IMyTerminalBlock, bool> collect = null)
{
var result = new List<IMyTerminalBlock>();
foreach (var block in commons.AllBlocks)
{
if ((collect == null || collect(block)) &&
(block is IMyReactor || block.DefinitionDisplayNameText == "Hydrogen Engine") &&
((IMyFunctionalBlock)block).IsFunctional &&
block.CustomName.IndexOf("[Excluded]", ZACommons.IGNORE_CASE) < 0)
{
result.Add(block);
}
}
return result;
}
private static List<IMyBatteryBlock> GetBatteries(ZACommons commons)
{
return ZACommons.GetBlocksOfType<IMyBatteryBlock>(commons.Blocks,
battery => battery.IsFunctional &&
battery.CustomName.IndexOf("[Excluded]", ZACommons.IGNORE_CASE) < 0);
}
}

public class DamageControl
{
private const double RunDelay = 3.0;
private const string ModeKey = "DamageConrol_Mode";
enum Modes : int { Idle=0, Active=1, Auto=2 };
private Modes Mode = Modes.Idle;
public void Init(ZACommons commons, EventDriver eventDriver)
{
Mode = Modes.Idle;
var modeString = commons.GetValue(ModeKey);
if (modeString != null)
{
var newMode = int.Parse(modeString);
switch ((Modes)newMode)
{
case Modes.Idle:
break;
case Modes.Active:
Start(commons, eventDriver, false);
break;
case Modes.Auto:
Start(commons, eventDriver, true);
break;
}
}
}
public void HandleCommand(ZACommons commons, EventDriver eventDriver,
string argument)
{
argument = argument.Trim().ToLower();
var parts = argument.Split(new char[] { ' ' }, 2);
if (parts.Length != 2 || parts[0] != "damecon") return;
var command = parts[1];
switch (command)
{
case "reset":
case "stop":
commons.AllBlocks.ForEach(block => block.ShowOnHUD = false);
ResetMode(commons);
break;
case "show":
Show(commons);
ResetMode(commons);
break;
case "start":
Start(commons, eventDriver, false);
break;
case "auto":
Start(commons, eventDriver, true);
break;
}
}
private void Start(ZACommons commons, EventDriver eventDriver, bool auto)
{
Show(commons);
if (Mode == Modes.Idle) eventDriver.Schedule(RunDelay, Run);
Mode = auto ? Modes.Auto : Modes.Active;
SaveMode(commons);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
if (Mode == Modes.Idle) return;
var damaged = Show(commons) > 0;
if (Mode == Modes.Active || damaged)
{
eventDriver.Schedule(RunDelay, Run);
}
else
{
ResetMode(commons);
}
}
public void Display(ZACommons commons)
{
if (Mode != Modes.Idle)
{
commons.Echo("Damage Control: Active");
}
}
private uint Show(ZACommons commons)
{
uint count = 0;
commons.AllBlocks.ForEach(block => {
var cubeGrid = block.CubeGrid;
var damaged = !cubeGrid.GetCubeBlock(block.Position).IsFullIntegrity;
block.ShowOnHUD = damaged;
if (damaged) count++;
});
return count;
}
private void ResetMode(ZACommons commons)
{
Mode = Modes.Idle;
SaveMode(commons);
}
private void SaveMode(ZACommons commons)
{
commons.SetValue(ModeKey, ((int)Mode).ToString());
}
}

public class DockingAction
{
private const double RunDelay = 3.0;
private const char ACTION_DELIMETER = ':';
public void Init(ZACommons commons, EventDriver eventDriver)
{
eventDriver.Schedule(0.0, Run);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
foreach (var group in commons.GetBlockGroupsWithPrefix(DOCKING_ACTION_PREFIX))
{
var parts = group.Name.Split(new char[] { ACTION_DELIMETER }, 2);
string action = "on";
if (parts.Length == 2)
{
action = parts[1];
}
bool connected = false;
var connectors = ZACommons.GetBlocksOfType<IMyShipConnector>(group.Blocks);
if (connectors.Count > 0)
{
var connector = connectors[0];
connected = connector.Status == MyShipConnectorStatus.Connected;
}
if ("on".Equals(action, ZACommons.IGNORE_CASE) ||
"off".Equals(action, ZACommons.IGNORE_CASE))
{
bool enable;
if ("on".Equals(action, ZACommons.IGNORE_CASE))
{
enable = connected;
}
else
{
enable = !connected;
}
group.Blocks.ForEach(block =>
{
if (!(block is IMyShipConnector) && // ignore connectors
block is IMyFunctionalBlock)
{
((IMyFunctionalBlock)block).Enabled = enable;
}
});
}
}
eventDriver.Schedule(RunDelay, Run);
}
}

public class RedundancyManager : DockingHandler
{
private const double RunDelay = 3.0;
private const char COUNT_DELIMITER = ':';
private bool IsDocked = true;
public void PreDock(ZACommons commons, EventDriver eventDriver) { }
public void DockingAction(ZACommons commons, EventDriver eventDriver,
bool docked)
{
if (docked)
{
IsDocked = true;
}
else if (IsDocked)
{
IsDocked = false;
eventDriver.Schedule(RunDelay, DHRun);
}
}
public void DHRun(ZACommons commons, EventDriver eventDriver)
{
if (IsDocked) return;
Run(commons);
eventDriver.Schedule(RunDelay, DHRun);
}
public void Run(ZACommons commons)
{
foreach (var group in commons.GetBlockGroupsWithPrefix(REDUNDANCY_PREFIX))
{
var parts = group.Name.Split(new char[] { COUNT_DELIMITER }, 2);
var count = 1;
if (parts.Length == 2)
{
if (int.TryParse(parts[1], out count))
{
count = Math.Max(count, 1);
}
else
{
count = 1;
}
}
var running = 0;
var spares = new LinkedList<IMyFunctionalBlock>();
foreach (var block in group.Blocks)
{
var fblock = block as IMyFunctionalBlock;
if (fblock != null && fblock.CubeGrid == commons.Me.CubeGrid &&
fblock.IsFunctional)
{
if (fblock.IsWorking && fblock.Enabled)
{
running++;
}
else
{
spares.AddLast(fblock);
}
}
}
while (running < count && spares.First != null)
{
var block = spares.First.Value;
spares.RemoveFirst();
block.Enabled = true;
running++;
}
}
}
public void Init(ZACommons commons, EventDriver eventDriver)
{
eventDriver.Schedule(0.0, Run);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
Run(commons);
eventDriver.Schedule(RunDelay, Run);
}
}

public interface DockingHandler
{
void PreDock(ZACommons commons, EventDriver eventDriver);
void DockingAction(ZACommons commons, EventDriver eventDriver,
bool docked);
}

public class ProductionManager
{
private const double RunDelay = 1.0;
private const string StateKey = "ProductionManager_State";
private const char DelimiterStart = '{';
private const char DelimiterEnd = '}';
private const char DelimiterAmount = ':';
public struct ItemStock
{
public string SubtypeName;
public float Amount;
public ItemStock(string subtypeName, float amount)
{
SubtypeName = subtypeName;
Amount = amount * PRODUCTION_MANAGER_SETUP_AMOUNT_MULTIPLIER;
}
public override string ToString()
{
return SubtypeName + ":" + Amount;
}
public static bool TryParse(string str, out ItemStock result)
{
int start = str.IndexOf(DelimiterStart);
int end = str.IndexOf(DelimiterEnd);
if (start >= 0 && end >= 0 && end > start)
{
var data = str.Substring(start + 1, end - start - 1);
var parts = data.Split(':');
if (parts.Length == 2)
{
var subtypeName = parts[0].Trim();
var amountString = parts[1].Trim();
float amount;
if (float.TryParse(amountString, out amount))
{
result = new ItemStock(subtypeName, amount);
return true;
}
}
}
result = default(ItemStock);
return false;
}
}
public struct AssemblerTarget
{
public float Amount;
public List<IMyAssembler> Assemblers;
public AssemblerTarget(float amount)
{
Amount = amount;
Assemblers = new List<IMyAssembler>();
}
public void EnableAssemblers(bool enable)
{
Assemblers.ForEach(assembler =>
{
if ((enable && !assembler.Enabled) ||
(!enable && assembler.Enabled))
{
assembler.Enabled = enable;
}
});
}
}
private ItemStock[] defaultItemStocks = new ItemStock[]
{
new ItemStock("BulletproofGlass", 12000),
new ItemStock("Computer", 6500),
new ItemStock("Construction", 50000),
new ItemStock("Detector", 400),
new ItemStock("Display", 500),
new ItemStock("Explosives", 500),
new ItemStock("Girder", 3500),
new ItemStock("GravityGenerator", 250),
new ItemStock("InteriorPlate", 55000),
new ItemStock("LargeTube", 6000),
new ItemStock("Medical", 120),
new ItemStock("MetalGrid", 15500),
new ItemStock("Motor", 16000),
new ItemStock("PowerCell", 2800),
new ItemStock("RadioCommunication", 250),
new ItemStock("Reactor", 10000),
new ItemStock("SmallTube", 26000),
new ItemStock("SolarCell", 2800),
new ItemStock("SteelPlate", 300000),
new ItemStock("Superconductor", 3000),
new ItemStock("Thrust", 16000),
};
enum States : int { Inactivating=-1, Inactive=0, Active=1 };
private States CurrentState = States.Active;
private Dictionary<string, VRage.MyFixedPoint> EnumerateItems(List<IMyTerminalBlock> blocks, HashSet<string> allowedSubtypes)
{
var result = new Dictionary<string, VRage.MyFixedPoint>();
foreach (var owner in blocks)
{
for (int i = 0; i < owner.InventoryCount; i++)
{
var inventory = owner.GetInventory(i);
var items = new List<MyInventoryItem>();
inventory.GetItems(items);
foreach (var item in items)
{
var subtypeName = item.Type.SubtypeId;
if (allowedSubtypes.Contains(subtypeName))
{
VRage.MyFixedPoint current;
if (!result.TryGetValue(subtypeName, out current)) current = (VRage.MyFixedPoint)0.0f;
result[subtypeName] = current + item.Amount;
}
}
}
}
return result;
}
public void Setup(List<IMyTerminalBlock> ship)
{
var defaultItemStocksMap = new Dictionary<string, ItemStock>();
for (int i = 0; i < defaultItemStocks.Length; i++)
{
var itemStock = defaultItemStocks[i];
defaultItemStocksMap.Add(itemStock.SubtypeName, itemStock);
}
var assemblers = ZACommons.GetBlocksOfType<IMyAssembler>(ship);
var candidates = new LinkedList<IMyAssembler>();
foreach (var assembler in assemblers)
{
ItemStock target;
if (ItemStock.TryParse(assembler.CustomName, out target))
{
defaultItemStocksMap.Remove(target.SubtypeName);
}
else
{
candidates.AddLast(assembler);
}
}
for (var e = defaultItemStocksMap.Values.GetEnumerator(); e.MoveNext() && candidates.First != null;)
{
var itemStock = e.Current;
var candidate = candidates.First.Value;
candidates.RemoveFirst();
StringBuilder builder = new StringBuilder();
builder.Append(candidate.CustomName);
builder.Append(' ');
builder.Append(DelimiterStart);
builder.Append(itemStock.SubtypeName);
builder.Append(DelimiterAmount);
builder.Append(itemStock.Amount);
builder.Append(DelimiterEnd);
candidate.CustomName = builder.ToString();
}
}
public void Init(ZACommons commons, EventDriver eventDriver)
{
var stateValue = commons.GetValue(StateKey);
if (stateValue != null)
{
int state;
if (int.TryParse(stateValue, out state))
{
CurrentState = (States)state;
}
}
else
{
CurrentState = States.Active;
}
if (PRODUCTION_MANAGER_SETUP)
{
Setup(LIMIT_PRODUCTION_MANAGER_SAME_GRID ? commons.Blocks : commons.AllBlocks);
}
else if (CurrentState != States.Inactive)
{
eventDriver.Schedule(0.0, Run);
}
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
if (CurrentState == States.Inactive) return;
var ship = LIMIT_PRODUCTION_MANAGER_SAME_GRID ? commons.Blocks : commons.AllBlocks;
var allowedSubtypes = new HashSet<string>();
var assemblerTargets = new Dictionary<string, AssemblerTarget>();
var assemblers = ZACommons.GetBlocksOfType<IMyAssembler>(ship);
foreach (var assembler in assemblers)
{
ItemStock target;
if (ItemStock.TryParse(assembler.CustomName, out target))
{
var subtype = target.SubtypeName;
allowedSubtypes.Add(subtype);
AssemblerTarget assemblerTarget;
if (!assemblerTargets.TryGetValue(subtype, out assemblerTarget))
{
assemblerTarget = new AssemblerTarget(target.Amount);
assemblerTargets.Add(subtype, assemblerTarget);
}
assemblerTarget.Assemblers.Add(assembler);
assemblerTarget.Amount = Math.Max(assemblerTarget.Amount, target.Amount);
}
}
if (CurrentState == States.Active)
{
var stocks = EnumerateItems(ship, allowedSubtypes);
foreach (var kv in assemblerTargets)
{
var subtype = kv.Key;
var target = kv.Value;
VRage.MyFixedPoint currentStock;
if (!stocks.TryGetValue(subtype, out currentStock)) currentStock = (VRage.MyFixedPoint)0.0f;
target.EnableAssemblers((float)currentStock < target.Amount);
}
}
else if (CurrentState == States.Inactivating)
{
foreach (var target in assemblerTargets.Values)
{
target.EnableAssemblers(false);
}
SetState(commons, States.Inactive);
}
eventDriver.Schedule(RunDelay, Run);
}
public void HandleCommand(ZACommons commons, EventDriver eventDriver,
string argument)
{
argument = argument.Trim().ToLower();
switch (argument)
{
case "prodpause":
SetState(commons, States.Inactivating);
break;
case "prodresume":
if (CurrentState != States.Inactive)
{
SetState(commons, States.Active);
if (!PRODUCTION_MANAGER_SETUP) eventDriver.Schedule(RunDelay, Run);
}
break;
}
}
public void Display(ZACommons commons)
{
if (PRODUCTION_MANAGER_SETUP)
{
commons.Echo("Setup complete. Set PRODUCTION_MANAGER_SETUP to false to enable ProductionManager");
}
else
{
commons.Echo("Production Manager: " +
(CurrentState == States.Inactive ? "Paused" : "Active"));
}
}
private void SetState(ZACommons commons, States newState)
{
CurrentState = newState;
commons.SetValue(StateKey, ((int)CurrentState).ToString());
}
}

public class RefineryManager
{
public struct RefineryWrapper : IComparable<RefineryWrapper>
{
public IMyRefinery Refinery;
public IMyInventory Inventory;
public MyInventoryItem Item;
public float Amount;
public RefineryWrapper(IMyRefinery refinery)
{
Refinery = refinery;
Inventory = refinery.GetInventory(0);
var items = new List<MyInventoryItem>();
Inventory.GetItems(items);
Item = items.Count > 0 ? items[0] : default(MyInventoryItem);
Amount = (float)Item.Amount;
}
public int CompareTo(RefineryWrapper other)
{
return other.Amount.CompareTo(Amount);
}
}
private const double RunDelay = 1.0;
public void Init(ZACommons commons, EventDriver eventDriver)
{
eventDriver.Schedule(0.0, Run);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
var groups = commons.GetBlockGroupsWithPrefix(REFINERY_MANAGER_PREFIX);
if (groups.Count > 0)
{
groups.ForEach(group => Balance(commons, group.Blocks));
}
else
{
Balance(commons, commons.Blocks);
}
eventDriver.Schedule(RunDelay, Run);
}
private void Balance(ZACommons commons, List<IMyTerminalBlock> blocks)
{
var refineries = ZACommons
.GetBlocksOfType<IMyRefinery>(blocks,
block => block.IsFunctional &&
block.IsWorking &&
((IMyRefinery)block).Enabled &&
((IMyRefinery)block).UseConveyorSystem);
var isProducing = false;
var isIdle = false;
var wrappers = new LinkedList<RefineryWrapper>();
foreach (var refinery in refineries)
{
InsertSorted(wrappers, new RefineryWrapper(refinery));
if (refinery.IsProducing)
{
isProducing = true;
}
else
{
isIdle = true;
}
}
if (isProducing && isIdle)
{
var first = wrappers.First.Value;
var last = wrappers.Last.Value;
if (last.Amount == 0.0f)
{
VRage.MyFixedPoint amount = first.Item.Amount * (VRage.MyFixedPoint)0.5f;
first.Inventory.TransferItemTo(last.Inventory, 0, amount: amount);
}
}
}
private void InsertSorted(LinkedList<RefineryWrapper> wrappers,
RefineryWrapper wrapper)
{
for (var current = wrappers.First;
current != null;
current = current.Next)
{
if (wrapper.CompareTo(current.Value) < 0)
{
wrappers.AddBefore(current, wrapper);
return;
}
}
wrappers.AddLast(wrapper);
}
}

public class AirVentManager
{
private const double RunDelay = 10.0;
public void Init(ZACommons commons, EventDriver eventDriver)
{
eventDriver.Schedule(1, Tick);
}
private List<IMyAirVent> GetAirVents(ZACommons commons)
{
return ZACommons.GetBlocksOfType<IMyAirVent>(commons.AllBlocks,
vent => vent.IsFunctional &&
vent.CustomName.IndexOf("[Excluded]", ZACommons.IGNORE_CASE) < 0 &&
vent.CustomName.IndexOf("[Intake]", ZACommons.IGNORE_CASE) < 0);
}
public void Tick(ZACommons commons, EventDriver eventDriver)
{
var vents = GetAirVents(commons);
vents.ForEach(vent =>
{
vent.Enabled = true;
});
eventDriver.Schedule(1, Tock);
}
public void Tock(ZACommons commons, EventDriver eventDriver)
{
var vents = GetAirVents(commons);
vents.ForEach(vent =>
{
var level = vent.GetOxygenLevel();
if (vent.Depressurize)
{
vent.Enabled = level > 0.0f;
}
else
{
vent.Enabled = level < MIN_AIR_VENT_PRESSURE;
}
});
eventDriver.Schedule(RunDelay, Tick);
}
}

public class OxygenManager
{
private const double RunDelay = 1.0;
enum OxygenLevel { Unknown, Low, Buffer, Normal, High };
private OxygenLevel PreviousState = OxygenLevel.Unknown;
private double GetAverageOxygenTankLevel(List<IMyGasTank> tanks)
{
double total = 0.0;
int count = 0;
foreach (var tank in tanks)
{
total += tank.FilledRatio;
count++;
}
return count != 0 ? total / count : 0.0;
}
private OxygenLevel GetOxygenState(List<IMyGasTank> tanks)
{
var level = GetAverageOxygenTankLevel(tanks);
if (level >= MAX_OXYGEN_TANK_LEVEL)
{
return OxygenLevel.High;
}
else if (level >= MIN_OXYGEN_TANK_LEVEL)
{
return OxygenLevel.Normal;
}
else if (level > LOW_OXYGEN_TANK_LEVEL)
{
return OxygenLevel.Buffer;
}
else
{
return OxygenLevel.Low;
}
}
public void Init(ZACommons commons, EventDriver eventDriver)
{
eventDriver.Schedule(0.0, Run);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
var tanks = ZACommons.GetBlocksOfType<IMyGasTank>(commons.AllBlocks,
tank => tank.IsFunctional &&
tank.IsWorking &&
tank.CustomName.IndexOf("[Excluded]", ZACommons.IGNORE_CASE) < 0);
var currentState = GetOxygenState(tanks);
if (PreviousState != currentState)
{
PreviousState = currentState;
bool? generateOxygen = null;
bool? farmOxygen = null;
switch (currentState)
{
case OxygenLevel.High:
generateOxygen = false;
farmOxygen = false;
break;
case OxygenLevel.Normal:
farmOxygen = true;
break;
case OxygenLevel.Buffer:
generateOxygen = true;
farmOxygen = true;
break;
case OxygenLevel.Low:
generateOxygen = true;
farmOxygen = true;
TimerBlockUtils.StartTimerBlockWithName(commons.AllBlocks, LOW_OXYGEN_NAME);
break;
}
if (generateOxygen != null)
{
var generators =
ZACommons.GetBlocksOfType<IMyGasGenerator>(commons.Blocks,
block => block.IsFunctional);
ZACommons.EnableBlocks(generators, (bool)generateOxygen);
}
if (farmOxygen != null)
{
var farms =
ZACommons.GetBlocksOfType<IMyOxygenFarm>(commons.Blocks,
block => block.IsFunctional);
ZACommons.EnableBlocks(farms, (bool)farmOxygen);
var vents =
ZACommons.GetBlocksOfType<IMyAirVent>(commons.Blocks,
vent => vent.IsFunctional &&
vent.Depressurize &&
vent.CustomName.IndexOf("[Intake]", ZACommons.IGNORE_CASE) >= 0);
ZACommons.EnableBlocks(vents, (bool)farmOxygen);
}
}
eventDriver.Schedule(RunDelay, Run);
}
}

public static class TimerBlockUtils
{
public static bool StartTimerBlockWithName(IEnumerable<IMyTerminalBlock> blocks, string name,
Func<IMyTimerBlock, bool> condition = null)
{
var timer = ZACommons.GetBlockWithName<IMyTimerBlock>(blocks, name);
if (timer != null && timer.Enabled && !timer.IsCountingDown &&
(condition == null || condition(timer)))
{
timer.StartCountdown();
return true;
}
return false;
}
}

public class ComplexAirlock
{
public class OpenQueueEntry
{
public int DesiredState { get; private set; }
public HashSet<IMyDoor> Doors { get; private set; }
public OpenQueueEntry(int desiredState, HashSet<IMyDoor> doors)
{
DesiredState = desiredState;
Doors = doors;
}
}
private const double RunDelay = 1.0;
private const int AIRLOCK_STATE_VACUUM = 0;
private const int AIRLOCK_STATE_PRESSURIZED = 1;
private const int AIRLOCK_STATE_UNKNOWN = -1;
private readonly List<ZACommons.BlockGroup> rooms = new List<ZACommons.BlockGroup>();
private readonly Dictionary<string, ZACommons.BlockGroup> roomsMap = new Dictionary<string, ZACommons.BlockGroup>();
private readonly HashSet<IMyDoor> innerDoors = new HashSet<IMyDoor>();
private readonly HashSet<IMyDoor> spaceDoors = new HashSet<IMyDoor>();
private readonly Dictionary<string, ZACommons.BlockGroup> doorVentGroups = new Dictionary<string, ZACommons.BlockGroup>();
private readonly Dictionary<IMyDoor, ZACommons.BlockGroup> doorVentRooms = new Dictionary<IMyDoor, ZACommons.BlockGroup>(); // Reverse mapping of rooms
private readonly Dictionary<IMyDoor, List<IMyAirVent>> doorVentMap = new Dictionary<IMyDoor, List<IMyAirVent>>();
private readonly Dictionary<string, OpenQueueEntry> openQueue = new Dictionary<string, OpenQueueEntry>();
private void Init(ZACommons commons)
{
var groups = commons.GetBlockGroupsWithPrefix("Airlock");
foreach (var group in groups)
{
if (string.Equals("AirlockDoorInner", group.Name, ZACommons.IGNORE_CASE))
{
innerDoors.UnionWith(ZACommons.GetBlocksOfType<IMyDoor>(group.Blocks));
}
else if (string.Equals("AirlockDoorSpace", group.Name, ZACommons.IGNORE_CASE))
{
spaceDoors.UnionWith(ZACommons.GetBlocksOfType<IMyDoor>(group.Blocks));
}
else if (group.Name.StartsWith("AirlockDoor", ZACommons.IGNORE_CASE))
{
doorVentGroups.Add(group.Name, group);
var vents = ZACommons.GetBlocksOfType<IMyAirVent>(group.Blocks);
var doors = ZACommons.GetBlocksOfType<IMyDoor>(group.Blocks);
foreach (var door in doors)
{
doorVentMap.Add(door, vents);
}
}
else
{
rooms.Add(group);
roomsMap.Add(group.Name, group);
var doors = ZACommons.GetBlocksOfType<IMyDoor>(group.Blocks);
foreach (var door in doors)
{
doorVentRooms.Add(door, group);
}
}
}
}
private void Clear()
{
rooms.Clear();
roomsMap.Clear();
innerDoors.Clear();
spaceDoors.Clear();
doorVentGroups.Clear();
doorVentRooms.Clear();
doorVentMap.Clear();
}
private int GetAirlockState(List<IMyAirVent> vents)
{
foreach (var vent in vents)
{
if (vent.IsFunctional && vent.Enabled)
{
var level = vent.GetOxygenLevel();
if (level < .01f) return AIRLOCK_STATE_VACUUM;
else if (level >= 0.99f) return AIRLOCK_STATE_PRESSURIZED;
else return AIRLOCK_STATE_UNKNOWN;
}
}
return AIRLOCK_STATE_UNKNOWN;
}
private void DepressurizeVents(IEnumerable<IMyAirVent> vents, bool depressurize)
{
foreach (var vent in vents)
{
vent.Depressurize = depressurize;
}
}
private void OpenCloseDoors(IEnumerable<IMyDoor> doors, bool open)
{
foreach (var door in doors)
{
if (open) door.OpenDoor();
else door.CloseDoor();
}
}
private void ChangeRoomState(string roomName,
List<IMyAirVent> vents, List<IMyDoor> doors,
int current, int target,
IEnumerable<IMyDoor> targetDoors = null)
{
if (target != current && target != AIRLOCK_STATE_UNKNOWN)
{
OpenCloseDoors(doors, false);
DepressurizeVents(vents, target == AIRLOCK_STATE_VACUUM);
}
var entry = new OpenQueueEntry(target,
targetDoors != null ?
new HashSet<IMyDoor>(targetDoors) :
new HashSet<IMyDoor>());
openQueue[roomName] = entry;
}
private void HandleCommandInternal(string argument)
{
var parts = argument.Split(new char[] { ' ' }, 2);
if (parts.Length != 2) return;
var command = parts[0];
argument = parts[1].Trim();
if (command == "inner" || command == "space" || command == "toggle")
{
ZACommons.BlockGroup room;
if (roomsMap.TryGetValue(argument, out room))
{
var vents = ZACommons.GetBlocksOfType<IMyAirVent>(room.Blocks);
var current = GetAirlockState(vents);
int target = AIRLOCK_STATE_UNKNOWN;
switch (command)
{
case "space":
target = AIRLOCK_STATE_VACUUM;
break;
case "inner":
target = AIRLOCK_STATE_PRESSURIZED;
break;
case "toggle":
target = current == AIRLOCK_STATE_PRESSURIZED ?
AIRLOCK_STATE_VACUUM : AIRLOCK_STATE_PRESSURIZED;
break;
}
ChangeRoomState(room.Name,
vents, ZACommons.GetBlocksOfType<IMyDoor>(room.Blocks),
current, target, null);
}
}
else if (command == "open")
{
ZACommons.BlockGroup group;
if (doorVentGroups.TryGetValue(argument, out group))
{
var doors = ZACommons.GetBlocksOfType<IMyDoor>(group.Blocks);
if (doors.Count > 0)
{
var door = doors[0];
ZACommons.BlockGroup room;
if (doorVentRooms.TryGetValue(door, out room))
{
var otherVents = ZACommons.GetBlocksOfType<IMyAirVent>(group.Blocks);
var roomVents = ZACommons.GetBlocksOfType<IMyAirVent>(room.Blocks);
var target = GetAirlockState(otherVents);
var current = GetAirlockState(roomVents);
ChangeRoomState(room.Name,
roomVents,
ZACommons.GetBlocksOfType<IMyDoor>(room.Blocks),
current, target, doors);
}
}
}
}
}
private void CloseDoorsAsNeeded(EventDriver eventDriver,
ZACommons.BlockGroup room, List<IMyDoor> doors,
HashSet<IMyDoor> targetDoors,
int checkState)
{
var openDoors = new HashSet<IMyDoor>();
OpenQueueEntry entry;
if (openQueue.TryGetValue(room.Name, out entry))
{
if (entry.DesiredState == checkState)
{
openQueue.Remove(room.Name);
openDoors = entry.Doors;
if (openDoors.Count == 0)
{
openDoors = new HashSet<IMyDoor>(targetDoors); // NB copy
}
openDoors.IntersectWith(doors);
}
}
foreach (var door in doors)
{
int otherState;
List<IMyAirVent> otherVents;
if (doorVentMap.TryGetValue(door, out otherVents))
{
otherState = GetAirlockState(otherVents);
}
else { otherState = AIRLOCK_STATE_UNKNOWN; }
if (targetDoors.Contains(door) || otherState == checkState)
{
door.Enabled = true;
}
else
{
if (door.Status == DoorStatus.Open)
{
door.CloseDoor();
}
else if (door.OpenRatio == 0.0f && door.Enabled)
{
door.Enabled = false;
}
}
}
if (openDoors.Count > 0)
{
eventDriver.Schedule(2.5, (p, e) =>
{
foreach (var door in openDoors)
{
door.OpenDoor();
}
});
}
}
private void OpenCloseDoorsAsNeeded(EventDriver eventDriver)
{
foreach (var room in rooms)
{
var vents = ZACommons.GetBlocksOfType<IMyAirVent>(room.Blocks);
if (vents.Count == 0) continue;
var doors = ZACommons.GetBlocksOfType<IMyDoor>(room.Blocks);
if (doors.Count == 0) continue;
var state = GetAirlockState(vents);
switch (state)
{
case AIRLOCK_STATE_VACUUM:
CloseDoorsAsNeeded(eventDriver, room, doors, spaceDoors,
AIRLOCK_STATE_VACUUM);
break;
case AIRLOCK_STATE_PRESSURIZED:
CloseDoorsAsNeeded(eventDriver, room, doors, innerDoors,
AIRLOCK_STATE_PRESSURIZED);
break;
case AIRLOCK_STATE_UNKNOWN:
foreach (var door in doors)
{
door.CloseDoor();
if (door.Status == DoorStatus.Closed && door.Enabled)
{
door.Enabled = false;
}
}
break;
}
}
}
private void RunInternal(ZACommons commons, EventDriver eventDriver, string argument)
{
Init(commons);
if (!string.IsNullOrWhiteSpace(argument))
{
HandleCommandInternal(argument);
}
OpenCloseDoorsAsNeeded(eventDriver);
Clear();
}
public void Init(ZACommons commons, EventDriver eventDriver)
{
eventDriver.Schedule(0.0, Run);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
RunInternal(commons, eventDriver, ""); // Why...
eventDriver.Schedule(RunDelay, Run);
}
public void HandleCommand(ZACommons commons, EventDriver eventDriver, string argument)
{
RunInternal(commons, eventDriver, argument); // Why...
}
}

public class SimpleAirlock
{
private const double RunDelay = 1.0;
private bool IsAnyDoorOpen(List<IMyDoor> doors)
{
foreach (var door in doors)
{
if (door.OpenRatio > 0.0f) return true;
}
return false;
}
public void Init(ZACommons commons, EventDriver eventDriver)
{
eventDriver.Schedule(0.0, Run);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
var groups = commons.GetBlockGroupsWithPrefix(SIMPLE_AIRLOCK_GROUP_PREFIX);
foreach (var group in groups)
{
var doors = ZACommons.GetBlocksOfType<IMyDoor>(group.Blocks,
door => door.CubeGrid == commons.Me.CubeGrid &&
door.IsFunctional);
var opened = IsAnyDoorOpen(doors);
foreach (var door in doors)
{
if (door.OpenRatio == 0.0f && opened)
{
if (door.Enabled) door.Enabled = false;
}
else
{
if (!door.Enabled) door.Enabled = true;
}
}
}
eventDriver.Schedule(RunDelay, Run);
}
}

public class DoorAutoCloser
{
private const double RunDelay = 1.0;
private const char DURATION_DELIMITER = ':';
private readonly Dictionary<IMyDoor, TimeSpan> opened = new Dictionary<IMyDoor, TimeSpan>();
public void Init(ZACommons commons, EventDriver eventDriver)
{
eventDriver.Schedule(0.0, Run);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
var groups = commons.GetBlockGroupsWithPrefix(DOOR_AUTO_CLOSER_PREFIX);
if (groups.Count > 0)
{
groups.ForEach(group => {
var parts = group.Name.Split(new char[] { DURATION_DELIMITER }, 2);
var duration = DEFAULT_DOOR_OPEN_DURATION;
if (parts.Length == 2)
{
if (!double.TryParse(parts[1], out duration))
{
duration = DEFAULT_DOOR_OPEN_DURATION;
}
}
var doors = ZACommons.GetBlocksOfType<IMyDoor>(group.Blocks,
block => block.IsFunctional);
CloseDoors(commons, eventDriver, doors, duration);
});
}
else
{
var doors = ZACommons
.GetBlocksOfType<IMyDoor>(commons.Blocks,
block => block.IsFunctional &&
block.CustomName.IndexOf("[Excluded]", ZACommons.IGNORE_CASE) < 0 &&
block.DefinitionDisplayNameText != "Airtight Hangar Door");
CloseDoors(commons, eventDriver, doors, DEFAULT_DOOR_OPEN_DURATION);
}
eventDriver.Schedule(RunDelay, Run);
}
private void CloseDoors(ZACommons commons, EventDriver eventDriver, List<IMyDoor> doors,
double openDurationSeconds)
{
var openDuration = TimeSpan.FromSeconds(openDurationSeconds);
doors.ForEach(door => {
if (door.Status == DoorStatus.Open)
{
TimeSpan closeTime;
if (opened.TryGetValue(door, out closeTime))
{
if (closeTime <= eventDriver.TimeSinceStart)
{
door.CloseDoor();
opened.Remove(door);
}
}
else
{
opened.Add(door, eventDriver.TimeSinceStart + openDuration);
}
}
else
{
opened.Remove(door);
}
});
}
}

public class EventDriver
{
public struct FutureTickAction : IComparable<FutureTickAction>
{
public ulong When;
public Action<ZACommons, EventDriver> Action;
public FutureTickAction(ulong when, Action<ZACommons, EventDriver> action = null)
{
When = when;
Action = action;
}
public int CompareTo(FutureTickAction other)
{
return When.CompareTo(other.When);
}
}
public struct FutureTimeAction : IComparable<FutureTimeAction>
{
public TimeSpan When;
public Action<ZACommons, EventDriver> Action;
public FutureTimeAction(TimeSpan when, Action<ZACommons, EventDriver> action = null)
{
When = when;
Action = action;
}
public int CompareTo(FutureTimeAction other)
{
return When.CompareTo(other.When);
}
}
private const float TicksPerSecond = 60.0f;
private readonly LinkedList<FutureTickAction> TickQueue = new LinkedList<FutureTickAction>();
private readonly LinkedList<FutureTimeAction> TimeQueue = new LinkedList<FutureTimeAction>();
private ulong Ticks; // Not a reliable measure of time because of variable update frequency.
public TimeSpan TimeSinceStart { get; private set; }
public EventDriver()
{
TimeSinceStart = TimeSpan.FromSeconds(0);
}
private void KickTimer(ZACommons commons)
{
if (TickQueue.First != null)
{
commons.Program.Runtime.UpdateFrequency = UpdateFrequency.Update1;
}
else if (TimeQueue.First != null)
{
var next = (float)(TimeQueue.First.Value.When.TotalSeconds - TimeSinceStart.TotalSeconds);
if (next < (10.0f / TicksPerSecond))
{
commons.Program.Runtime.UpdateFrequency = UpdateFrequency.Update1;
}
else if (next < (100.0f / TicksPerSecond))
{
commons.Program.Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
else
{
commons.Program.Runtime.UpdateFrequency = UpdateFrequency.Update100;
}
}
else
{
commons.Program.Runtime.UpdateFrequency = UpdateFrequency.None;
}
}
public void Tick(ZACommons commons, Action mainAction = null,
Action preAction = null,
Action argAction = null,
Action postAction = null)
{
Ticks++;
TimeSinceStart += commons.Program.Runtime.TimeSinceLastRun;
bool runMain = false;
if (preAction != null) preAction();
if (argAction != null && (commons.UpdateType & ~(UpdateType.Update1|UpdateType.Update10|UpdateType.Update100|UpdateType.Once)) != 0) argAction();
while (TickQueue.First != null &&
TickQueue.First.Value.When <= Ticks)
{
var action = TickQueue.First.Value.Action;
TickQueue.RemoveFirst();
if (action != null)
{
action(commons, this);
}
else
{
runMain = true;
}
}
while (TimeQueue.First != null &&
TimeQueue.First.Value.When <= TimeSinceStart)
{
var action = TimeQueue.First.Value.Action;
TimeQueue.RemoveFirst();
if (action != null)
{
action(commons, this);
}
else
{
runMain = true;
}
}
if (runMain && mainAction != null) mainAction();
if (postAction != null) postAction();
KickTimer(commons);
}
public void Schedule(ulong delay, Action<ZACommons, EventDriver> action = null)
{
var future = new FutureTickAction(Ticks + delay, action);
for (var current = TickQueue.First;
current != null;
current = current.Next)
{
if (future.CompareTo(current.Value) < 0)
{
TickQueue.AddBefore(current, future);
return;
}
}
TickQueue.AddLast(future);
}
public void Schedule(double seconds, Action<ZACommons, EventDriver> action = null)
{
var delay = Math.Max(seconds, 0.0);
var future = new FutureTimeAction(TimeSinceStart + TimeSpan.FromSeconds(delay), action);
for (var current = TimeQueue.First;
current != null;
current = current.Next)
{
if (future.CompareTo(current.Value) < 0)
{
TimeQueue.AddBefore(current, future);
return;
}
}
TimeQueue.AddLast(future);
}
}

public class ZACommons
{
public const StringComparison IGNORE_CASE = StringComparison.CurrentCultureIgnoreCase;
public readonly MyGridProgram Program;
public readonly UpdateType UpdateType;
private readonly string ShipGroupName;
private readonly ZAStorage Storage;
public bool IsDirty { get; private set; }
public List<IMyTerminalBlock> AllBlocks
{
get
{
if (m_allBlocks == null)
{
m_allBlocks = new List<IMyTerminalBlock>();
Program.GridTerminalSystem.GetBlocks(m_allBlocks);
}
return m_allBlocks;
}
}
private List<IMyTerminalBlock> m_allBlocks = null;
public List<IMyTerminalBlock> Blocks
{
get
{
if (m_blocks == null)
{
if (ShipGroupName != null)
{
var group = GetBlockGroupWithName(ShipGroupName);
if (group != null) m_blocks = group.Blocks;
}
if (m_blocks == null)
{
m_blocks = new List<IMyTerminalBlock>();
foreach (var block in AllBlocks)
{
if (block.CubeGrid == Program.Me.CubeGrid) m_blocks.Add(block);
}
}
}
return m_blocks;
}
}
private List<IMyTerminalBlock> m_blocks = null;
public class BlockGroup
{
private readonly IMyBlockGroup MyBlockGroup;
public BlockGroup(IMyBlockGroup myBlockGroup)
{
MyBlockGroup = myBlockGroup;
}
public String Name
{
get { return MyBlockGroup.Name; }
}
public List<IMyTerminalBlock> Blocks
{
get
{
if (m_blocks == null)
{
m_blocks = new List<IMyTerminalBlock>();
MyBlockGroup.GetBlocks(m_blocks);
}
return m_blocks;
}
}
private List<IMyTerminalBlock> m_blocks = null;
}
public List<BlockGroup> Groups
{
get
{
if (m_groups == null)
{
var groups = new List<IMyBlockGroup>();
Program.GridTerminalSystem.GetBlockGroups(groups);
m_groups = new List<BlockGroup>();
groups.ForEach(group => m_groups.Add(new BlockGroup(group)));
}
return m_groups;
}
}
private List<BlockGroup> m_groups = null;
public Dictionary<string, BlockGroup> GroupsByName
{
get
{
if (m_groupsByName == null)
{
m_groupsByName = new Dictionary<string, BlockGroup>();
foreach (var group in Groups)
{
m_groupsByName.Add(group.Name.ToLower(), group);
}
}
return m_groupsByName;
}
}
private Dictionary<string, BlockGroup> m_groupsByName = null;
public ZACommons(MyGridProgram program, UpdateType updateType,
string shipGroup = null, ZAStorage storage = null)
{
Program = program;
UpdateType = updateType;
ShipGroupName = shipGroup;
Storage = storage;
IsDirty = false;
}
public BlockGroup GetBlockGroupWithName(string name)
{
BlockGroup group;
if (GroupsByName.TryGetValue(name.ToLower(), out group))
{
return group;
}
return null;
}
public List<BlockGroup> GetBlockGroupsWithPrefix(string prefix)
{
var result = new List<BlockGroup>();
foreach (var group in Groups)
{
if (group.Name.StartsWith(prefix, IGNORE_CASE)) result.Add(group);
}
return result;
}
public static List<T> GetBlocksOfType<T>(IEnumerable<IMyTerminalBlock> blocks,
Func<T, bool> collect = null)
{
var list = new List<T>();
foreach (var block in blocks)
{
if (block is T && (collect == null || collect((T)block))) list.Add((T)block);
}
return list;
}
public static T GetBlockWithName<T>(IEnumerable<IMyTerminalBlock> blocks, string name)
where T : IMyTerminalBlock
{
foreach (var block in blocks)
{
if(block is T && block.CustomName.Equals(name, IGNORE_CASE)) return (T)block;
}
return default(T);
}
public static List<IMyTerminalBlock> SearchBlocksOfName(IEnumerable<IMyTerminalBlock> blocks, string name, Func<IMyTerminalBlock, bool> collect = null)
{
var result = new List<IMyTerminalBlock>();
foreach (var block in blocks)
{
if (block.CustomName.IndexOf(name, IGNORE_CASE) >= 0 &&
(collect == null || collect(block)))
{
result.Add(block);
}
}
return result;
}
public static void ForEachBlockOfType<T>(IEnumerable<IMyTerminalBlock> blocks, Action<T> action)
{
foreach (var block in blocks)
{
if (block is T)
{
action((T)block);
}
}
}
public static void EnableBlocks(IEnumerable<IMyTerminalBlock> blocks, bool enabled)
{
foreach (var block in blocks)
{
block.SetValue<bool>("OnOff", enabled);
}
}
public IMyProgrammableBlock Me
{
get { return Program.Me; }
}
public Action<string> Echo
{
get { return Program.Echo; }
}
public void SetValue(string key, string value)
{
if (Storage != null)
{
if (!string.IsNullOrWhiteSpace(value))
{
Storage.Data[key] = value;
}
else
{
Storage.Data.Remove(key);
}
IsDirty = true;
}
}
public string GetValue(string key)
{
string value;
if (Storage != null && Storage.Data.TryGetValue(key, out value))
{
return value;
}
return null;
}
}
public class ZAStorage
{
private const char KEY_DELIM = '\\';
private const char PAIR_DELIM = '$';
private readonly string PAIR_DELIM_STR = new string(PAIR_DELIM, 1);
public readonly Dictionary<string, string> Data = new Dictionary<string, string>();
public string Encode()
{
var encoded = new List<string>();
foreach (var kv in Data)
{
ValidityCheck(kv.Key);
ValidityCheck(kv.Value);
var pair = new StringBuilder();
pair.Append(kv.Key);
pair.Append(KEY_DELIM);
pair.Append(kv.Value);
encoded.Add(pair.ToString());
}
return string.Join(PAIR_DELIM_STR, encoded);
}
public void Decode(string data)
{
Data.Clear();
var pairs = data.Split(PAIR_DELIM);
for (int i = 0; i < pairs.Length; i++)
{
var parts = pairs[i].Split(new char[] { KEY_DELIM }, 2);
if (parts.Length == 2)
{
Data[parts[0]] = parts[1];
}
}
}
private void ValidityCheck(string value)
{
if (value.IndexOf(KEY_DELIM) >= 0 ||
value.IndexOf(PAIR_DELIM) >= 0)
{
throw new Exception(string.Format("String '{0}' cannot be used by ZAStorage!", value));
}
}
}
