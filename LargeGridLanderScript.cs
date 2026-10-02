// Generated from ZerothAngel's SEScripts version 253984145aea
// Modules: largelander, emergencystop, airventmanager, oxygenmanager, solargyrocontroller, printutils, batterymonitor, reactormanager, damagecontrol, vtvlhelper, customdata, split, cruiser, cruisecontrol, reversethrust, seeker, pid, simpleairlock, doorautocloser, redundancy, safemode, timerblockutils, safemodehandler, dockinghandler, eventdriver, shipcontrol, thrustcontrol, gyrocontrol, shiporientation, commons

// MIT licensed. See https://github.com/ZerothAngel/SEScripts for raw code.

// !!! Leading whitespace stripped to save bytes !!!

const string SHIP_GROUP = "MyLander";

// Most of the following options can be overridden by adding an appropriate
// line to the Custom Data of the prog block. (See comment that precedes each
// option.)

// Custom Data: referenceGroup "My Remote Group Name"
// (Be sure to include the quotes if your group name has spaces.)
const string VTVLHELPER_REMOTE_GROUP = "*MyLander Remote*";

const bool ABANDONMENT_ENABLED = false;

const bool CONTROL_CHECK_ENABLED = false;

// "oxygenManager no"
const bool OXYGEN_MANAGER_ENABLE = true;
// "airVentManager no"
const bool AIR_VENT_MANAGER_ENABLE = true;
// If other ships can dock to the lander
// "landerCarrier no"
const bool LANDER_CARRIER_ENABLE = true;

// EmergencyStop
const double EMERGENCY_STOP_MAX_GYRO_ERROR = 0.00175; // In radians

// AirVentManager
const float MIN_AIR_VENT_PRESSURE = 0.9f;
const float MAX_AIR_VENT_PRESSURE = 0.975f;

// OxygenManager
const double MIN_OXYGEN_TANK_LEVEL = 0.7;
const double MAX_OXYGEN_TANK_LEVEL = 0.9;
const double LOW_OXYGEN_TANK_LEVEL = 0.5;
const string LOW_OXYGEN_NAME = "Low Oxygen";

// SolarGyroController
const float SOLAR_GYRO_VELOCITY = 0.05f; // In radians per second
const double SOLAR_GYRO_AXIS_TIMEOUT = 15.0; // In seconds
const float SOLAR_GYRO_MIN_ERROR = 0.005f; // As a fraction of theoretical max
// If you're using modded solar panels, set these values (both in MW)
const float SOLAR_PANEL_MAX_POWER_LARGE = 0.120f; // Max power of large panels
const float SOLAR_PANEL_MAX_POWER_SMALL = 0.030f; // Max power of small panels

// BatteryMonitor
const float BATTERY_THRESHOLD = 0.2f; // Battery low mark as a fraction, e.g. 0.2 = 20% remaining
const string LOW_BATTERY_NAME = "Low Battery";  // Name of optional timer block

// VTVLHelper
// Drop
const Base6Directions.Direction VTVLHELPER_BURN_DIRECTION = Base6Directions.Direction.Forward;
const double VTVLHELPER_BURN_SPEED = 98.0; // In meters per second
const Base6Directions.Direction VTVLHELPER_BRAKE_DIRECTION = Base6Directions.Direction.Down; // Direction to face toward planet
const double VTVLHELPER_BRAKING_SPEED = 50.0; // In meters per second
// Launch
const Base6Directions.Direction VTVLHELPER_LAUNCH_DIRECTION = Base6Directions.Direction.Up;
const double VTVLHELPER_LAUNCH_SPEED = 98.0; // In meters per second
const string VTVLHELPER_LAUNCH_DONE = "Launch Done";
// Autodrop
const bool VTVLHELPER_USE_BRAKING_THRUSTER_SPEC_FOR_ALIGN = false; // Set to true to also use braking thruster-spec for alignment thrusters
const double VTVLHELPER_APPROACH_GAIN = 0.1; // Multiplied by distance to get approach speed
const double VTVLHELPER_MINIMUM_SPEED = 5.0; // In meters per second
const double VTVLHELPER_MAXIMUM_SPEED = 25.0; // In meters per second
const string VTVLHELPER_DROP_DONE = "Drop Done";
// Orbit
const Base6Directions.Direction VTVLHELPER_ORBIT_DIRECTION = Base6Directions.Direction.Down;

// CruiseControl
const double CRUISE_CONTROL_DEAD_ZONE = 0.02; // i.e. 2%

// Max gyro error when doing reverse thrust
// Increase (i.e. double or multiply by 5 or 10) if your ship
// takes too long to re-engage thrusters.
// The default (0.0035) is about .2 degrees.
const double REVERSE_THRUST_MAX_GYRO_ERROR = 0.0035;

// SimpleAirlock
const string SIMPLE_AIRLOCK_GROUP_PREFIX = "SimpleAirlock";

// DoorAutoCloser
const double DEFAULT_DOOR_OPEN_DURATION = 3.0; // In seconds
const string DOOR_AUTO_CLOSER_PREFIX = "AutoClose";

// RedundancyManager
const string REDUNDANCY_PREFIX = "Redundant";

// SafeMode
const string EMERGENCY_STOP_NAME = "Safe Mode"; // Name of optional timer block
//const string EMERGENCY_STOP_NAME = "Emergency Stop"; // Old name
const string SAFE_MODE_NAME = "Safe Mode"; // Name of optional timer block
const string ABANDONMENT_TIMEOUT = "01:00"; // HH:MM[:SS]

// !!! CODE BEGINS HERE, EDIT AT YOUR OWN RISK !!!

public class MySafeModeHandler : SafeModeHandler
{
public void SafeMode(ZACommons commons, EventDriver eventDriver)
{
eventDriver.Schedule(1.0, (c,ed) =>
{
new EmergencyStop().SafeMode(c, ed);
});
}
}
private readonly EventDriver eventDriver = new EventDriver();
private readonly SafeMode safeMode = new SafeMode(new MySafeModeHandler());
private readonly RedundancyManager redundancyManager = new RedundancyManager();
private readonly DoorAutoCloser doorAutoCloser = new DoorAutoCloser();
private readonly SimpleAirlock simpleAirlock = new SimpleAirlock();
private readonly CruiseControl cruiseControl = new CruiseControl();
private readonly VTVLHelper vtvlHelper = new VTVLHelper();
private readonly DamageControl damageControl = new DamageControl();
private readonly ReactorManager reactorManager = new ReactorManager();
private readonly BatteryMonitor batteryMonitor = new BatteryMonitor();
private readonly SolarGyroController solarGyroController =
new SolarGyroController(
GyroControl.Pitch,
GyroControl.Roll
);
private readonly OxygenManager oxygenManager = new OxygenManager();
private readonly AirVentManager airVentManager = new AirVentManager();
private readonly ZAStorage myStorage = new ZAStorage();
private readonly ShipOrientation shipOrientation = new ShipOrientation();
private readonly ZACustomData customData = new ZACustomData();
private bool FirstRun = true;
private string VTVLHelperRemoteGroup;
private bool OxygenManagerEnable, AirVentManagerEnable, LanderCarrierEnable;
Program()
{
Runtime.UpdateFrequency |= UpdateFrequency.Once;
}
void Main(string argument, UpdateType updateType)
{
var commons = new ShipControlCommons(this, updateType, shipOrientation,
shipGroup: SHIP_GROUP,
storage: myStorage);
if (FirstRun)
{
FirstRun = false;
customData.Parse(Me);
VTVLHelperRemoteGroup = customData.GetString("referenceGroup", VTVLHELPER_REMOTE_GROUP);
OxygenManagerEnable = customData.GetBool("oxygenManager", OXYGEN_MANAGER_ENABLE);
AirVentManagerEnable = customData.GetBool("airVentManager", AIR_VENT_MANAGER_ENABLE);
LanderCarrierEnable = customData.GetBool("landerCarrier", LANDER_CARRIER_ENABLE);
myStorage.Decode(Storage);
shipOrientation.SetShipReference(commons, VTVLHelperRemoteGroup);
safeMode.Init(commons, eventDriver);
redundancyManager.Init(commons, eventDriver);
doorAutoCloser.Init(commons, eventDriver);
simpleAirlock.Init(commons, eventDriver);
if (LanderCarrierEnable) reactorManager.Init(commons, eventDriver);
batteryMonitor.Init(commons, eventDriver);
solarGyroController.ConditionalInit(commons, eventDriver);
if (OxygenManagerEnable) oxygenManager.Init(commons, eventDriver);
if (AirVentManagerEnable) airVentManager.Init(commons, eventDriver);
cruiseControl.Init(commons, eventDriver, LivenessCheck);
vtvlHelper.Init(commons, eventDriver, customData, LivenessCheck);
damageControl.Init(commons, eventDriver);
}
eventDriver.Tick(commons, argAction: () => {
safeMode.HandleCommand(commons, eventDriver, argument);
cruiseControl.HandleCommand(commons, eventDriver, argument);
vtvlHelper.HandleCommand(commons, eventDriver, argument);
damageControl.HandleCommand(commons, eventDriver, argument);
reactorManager.HandleCommand(commons, eventDriver, argument);
solarGyroController.HandleCommand(commons, eventDriver, argument);
},
postAction: () => {
solarGyroController.Display(commons);
damageControl.Display(commons);
cruiseControl.Display(commons);
vtvlHelper.Display(commons);
});
if (commons.IsDirty) Storage = myStorage.Encode();
}
bool LivenessCheck(ZACommons commons, EventDriver eventDriver)
{
if (CONTROL_CHECK_ENABLED) safeMode.TriggerIfUncontrolled(commons, eventDriver);
return !safeMode.Abandoned;
}

public class EmergencyStop : SafeModeHandler
{
public void SafeMode(ZACommons commons, EventDriver eventDriver)
{
var shipControl = (ShipControlCommons)commons;
ZACommons.GetBlocksOfType<IMyShipController>(commons.Blocks).ForEach(controller => {
controller.DampenersOverride = true;
});
if (HaveWorkingThrusters(shipControl, Base6Directions.Direction.Forward) &&
HaveWorkingThrusters(shipControl, Base6Directions.Direction.Backward) &&
HaveWorkingThrusters(shipControl, Base6Directions.Direction.Left) &&
HaveWorkingThrusters(shipControl, Base6Directions.Direction.Right) &&
HaveWorkingThrusters(shipControl, Base6Directions.Direction.Up) &&
HaveWorkingThrusters(shipControl, Base6Directions.Direction.Down))
{
return;
}
eventDriver.Schedule(1.0, (c,ed) =>
{
var sc = (ShipControlCommons)c;
var forward = HaveWorkingThrusters2(sc, Base6Directions.Direction.Forward);
var backward = HaveWorkingThrusters2(sc, Base6Directions.Direction.Backward);
var up = HaveWorkingThrusters2(sc, Base6Directions.Direction.Up);
var left = HaveWorkingThrusters2(sc, Base6Directions.Direction.Left);
var right = HaveWorkingThrusters2(sc, Base6Directions.Direction.Right);
var down = HaveWorkingThrusters2(sc, Base6Directions.Direction.Down);
if (forward && backward && up &&
left && right && down)
{
return;
}
Base6Directions.Direction direction;
if (forward)
{
direction = Base6Directions.Direction.Forward;
}
else if (backward)
{
direction = Base6Directions.Direction.Backward;
}
else if (up)
{
direction = Base6Directions.Direction.Up;
}
else if (left)
{
direction = Base6Directions.Direction.Left;
}
else if (right)
{
direction = Base6Directions.Direction.Right;
}
else if (down)
{
direction = Base6Directions.Direction.Down;
}
else
{
return;
}
new ReverseThrust().Init(c, ed,
EMERGENCY_STOP_MAX_GYRO_ERROR,
thrusterDirection: direction);
});
}
private bool HaveWorkingThrusters(ShipControlCommons shipControl,
Base6Directions.Direction direction)
{
var found = false;
var overridden = new List<IMyThrust>();
var thrusters = shipControl.ThrustControl.GetThrusters(direction);
thrusters.ForEach(thruster =>
{
if (thruster.IsWorking)
{
if (thruster.ThrustOverridePercentage > 0f)
{
overridden.Add(thruster);
}
else
{
found = true;
}
}
});
overridden.ForEach(thruster =>
{
if (found)
{
thruster.Enabled = false;
}
else
{
thruster.ThrustOverridePercentage = 0f;
found = true;
}
});
if (!found)
{
thrusters.ForEach(thruster =>
{
thruster.Enabled = true;
thruster.ThrustOverridePercentage = 0f;
});
}
return found;
}
private bool HaveWorkingThrusters2(ShipControlCommons shipControl,
Base6Directions.Direction direction)
{
var found = false;
var thrusters = shipControl.ThrustControl.GetThrusters(direction);
thrusters.ForEach(thruster =>
{
if (thruster.IsWorking)
{
thruster.ThrustOverridePercentage = 0f;
found = true;
}
});
return found;
}
}

public class AirVentManager
{
private const double RunDelay = 3.0;
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
if (vent.Depressurize && level > 0.0f)
{
vent.Enabled = true;
}
else if (!vent.Depressurize)
{
if (level < MIN_AIR_VENT_PRESSURE)
{
vent.Enabled = true;
}
else if (level > MAX_AIR_VENT_PRESSURE)
{
vent.Enabled = false;
}
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

public class SolarGyroController
{
private const double RunDelay = 1.0;
private const string ActiveKey = "SolarGyroController_Active";
public struct SolarPanelDetails
{
public float MaxPowerOutput;
public float DefinedPowerOutput;
public SolarPanelDetails(IEnumerable<IMyTerminalBlock> blocks)
{
MaxPowerOutput = 0.0f;
DefinedPowerOutput = 0.0f;
foreach (var panel in ZACommons.GetBlocksOfType<IMySolarPanel>(blocks))
{
if (panel.IsFunctional && panel.IsWorking)
{
MaxPowerOutput += panel.MaxOutput;
DefinedPowerOutput += panel.CubeGrid.GridSize == 2.5f ? SOLAR_PANEL_MAX_POWER_LARGE : SOLAR_PANEL_MAX_POWER_SMALL;
}
}
}
}
private readonly int[] AllowedAxes;
private readonly float[] LastVelocities;
private readonly TimeSpan AxisTimeout = TimeSpan.FromSeconds(SOLAR_GYRO_AXIS_TIMEOUT);
private float? MaxPower = null;
private int AxisIndex = 0;
private bool Active = false;
private TimeSpan TimeOnAxis;
private float CurrentMaxPower;
public SolarGyroController(params int[] allowedAxes)
{
AllowedAxes = (int[])allowedAxes.Clone();
LastVelocities = new float[AllowedAxes.Length];
for (int i = 0; i < LastVelocities.Length; i++)
{
LastVelocities[i] = SOLAR_GYRO_VELOCITY;
}
}
public void Init(ZACommons commons, EventDriver eventDriver)
{
var shipControl = (ShipControlCommons)commons;
var gyroControl = shipControl.GyroControl;
gyroControl.Reset();
gyroControl.EnableOverride(true);
Active = true;
SaveActive(commons);
MaxPower = null; // Use first-run initialization
CurrentMaxPower = 0.0f;
eventDriver.Schedule(0.0, Run);
}
public void ConditionalInit(ZACommons commons, EventDriver eventDriver,
bool defaultActive = false)
{
var activeValue = commons.GetValue(ActiveKey);
if (activeValue != null)
{
bool active;
if (Boolean.TryParse(activeValue, out active))
{
if (active) Init(commons, eventDriver);
return;
}
}
if (defaultActive) Init(commons, eventDriver);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
if (!Active) return;
var shipControl = (ShipControlCommons)commons;
var gyroControl = shipControl.GyroControl;
var currentAxis = AllowedAxes[AxisIndex];
if (MaxPower == null)
{
MaxPower = -100.0f; // Start with something absurdly low to kick things off
gyroControl.Reset();
gyroControl.EnableOverride(true);
gyroControl.SetAxisVelocity(currentAxis, LastVelocities[AxisIndex]);
TimeOnAxis = eventDriver.TimeSinceStart + AxisTimeout;
}
var solarPanelDetails = new SolarPanelDetails(commons.Blocks);
CurrentMaxPower = solarPanelDetails.MaxPowerOutput;
var minError = solarPanelDetails.DefinedPowerOutput * SOLAR_GYRO_MIN_ERROR;
var delta = CurrentMaxPower - MaxPower;
MaxPower = CurrentMaxPower;
if (delta > minError)
{
gyroControl.EnableOverride(true);
}
else if (delta < -minError)
{
gyroControl.EnableOverride(true);
LastVelocities[AxisIndex] = -LastVelocities[AxisIndex];
gyroControl.SetAxisVelocity(currentAxis, LastVelocities[AxisIndex]);
}
else
{
gyroControl.EnableOverride(false);
}
if (TimeOnAxis <= eventDriver.TimeSinceStart && MaxPower < solarPanelDetails.DefinedPowerOutput * (1.0f - SOLAR_GYRO_MIN_ERROR))
{
AxisIndex++;
AxisIndex %= AllowedAxes.Length;
gyroControl.Reset();
gyroControl.EnableOverride(true);
gyroControl.SetAxisVelocity(AllowedAxes[AxisIndex], LastVelocities[AxisIndex]);
TimeOnAxis = eventDriver.TimeSinceStart + AxisTimeout;
}
eventDriver.Schedule(RunDelay, Run);
}
private void Pause(ZACommons commons, EventDriver eventDriver)
{
Active = false;
SaveActive(commons);
var shipControl = (ShipControlCommons)commons;
var gyroControl = shipControl.GyroControl;
gyroControl.Reset();
gyroControl.EnableOverride(false);
}
public void HandleCommand(ZACommons commons, EventDriver eventDriver,
string argument)
{
argument = argument.Trim().ToLower();
if (argument == "pause")
{
Pause(commons, eventDriver);
}
else if (argument == "resume")
{
if (!Active) Init(commons, eventDriver);
}
else if (argument == "togglesolar")
{
if (Active) Pause(commons, eventDriver);
else Init(commons, eventDriver);
}
}
public void Display(ZACommons commons)
{
if (!Active)
{
commons.Echo("Solar Max Power: Paused");
}
else
{
commons.Echo(string.Format("Solar Max Power: {0}", PrintUtils.FormatPower(CurrentMaxPower)));
}
}
private void SaveActive(ZACommons commons)
{
commons.SetValue(ActiveKey, Active.ToString());
}
}

public static class PrintUtils
{
public static string FormatPower(float value)
{
if (value >= 1.0f)
{
return string.Format("{0:F2} MW", value);
}
else if (value >= 0.001)
{
return string.Format("{0:F2} kW", value * 1000f);
}
else
{
return string.Format("{0:F2} W", value * 1000000f);
}
}
}

public class BatteryMonitor : DockingHandler
{
public interface LowBatteryHandler
{
void LowBattery(ZACommons commons, EventDriver eventDriver,
bool started);
}
private const double RunDelay = 5.0;
private readonly LowBatteryHandler lowBatteryHandler;
private bool IsDocked = true;
private bool Triggered = false;
public BatteryMonitor(LowBatteryHandler lowBatteryHandler = null)
{
this.lowBatteryHandler = lowBatteryHandler;
}
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
Triggered = false;
IsDocked = false;
eventDriver.Schedule(RunDelay, Run);
}
}
public void Init(ZACommons commons, EventDriver eventDriver)
{
IsDocked = false;
eventDriver.Schedule(0.0, Run);
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
if (IsDocked) return;
RunInternal(commons, eventDriver);
eventDriver.Schedule(RunDelay, Run);
}
private void RunInternal(ZACommons commons, EventDriver eventDriver)
{
var lowBattery = ZACommons.GetBlockWithName<IMyTimerBlock>(commons.Blocks, LOW_BATTERY_NAME);
if (lowBatteryHandler == null && lowBattery == null) return;
var batteries = ZACommons.GetBlocksOfType<IMyBatteryBlock>(commons.Blocks, battery => battery.IsFunctional && battery.Enabled);
if (batteries.Count == 0) return;
var currentStoredPower = 0.0f;
var maxStoredPower = 0.0f;
foreach (var block in batteries)
{
var battery = block as IMyBatteryBlock;
currentStoredPower += battery.CurrentStoredPower;
maxStoredPower += battery.MaxStoredPower;
}
var batteryPercent = currentStoredPower / maxStoredPower;
if (!Triggered && batteryPercent < BATTERY_THRESHOLD)
{
Triggered = true;
if (lowBatteryHandler != null) lowBatteryHandler.LowBattery(commons, eventDriver, true);
if (lowBattery != null) lowBattery.ApplyAction("Start");
}
else if (Triggered && batteryPercent >= BATTERY_THRESHOLD)
{
Triggered = false;
if (lowBatteryHandler != null) lowBatteryHandler.LowBattery(commons, eventDriver, false);
}
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
var myReactors = ZACommons.GetBlocksOfType<IMyReactor>(commons.Blocks,
block => block.IsWorking);
var currentState = myReactors.Count > 0;
if (State == null || currentState != (bool)State)
{
State = currentState;
if (!(bool)State)
{
var reactors = ZACommons.GetBlocksOfType<IMyReactor>(commons.AllBlocks,
block => block.CubeGrid != commons.Me.CubeGrid);
reactors.ForEach(block => block.Enabled = false);
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
GetAllReactors(commons).ForEach(block => block.Enabled = true);
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
GetAllReactors(c).ForEach(block => block.Enabled = false);
});
break;
}
}
}
private static List<IMyReactor> GetAllReactors(ZACommons commons)
{
return ZACommons.GetBlocksOfType<IMyReactor>(commons.AllBlocks,
reactor => reactor.IsFunctional &&
reactor.CustomName.IndexOf("[Excluded]", ZACommons.IGNORE_CASE) < 0);
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

public class VTVLHelper
{
private const string LastCommandKey = "VTVLHelper_LastCommand";
private const uint FramesPerRun = 2;
private const double RunsPerSecond = 60.0 / FramesPerRun;
private readonly Seeker seeker = new Seeker(1.0 / RunsPerSecond);
private readonly Cruiser cruiser = new Cruiser(1.0 / RunsPerSecond, 0.02);
private readonly Cruiser LongCruiser = new Cruiser(1.0 / RunsPerSecond, 0.02);
private readonly Cruiser LatCruiser = new Cruiser(1.0 / RunsPerSecond, 0.02);
enum Modes { Idle, Burning, Gliding, Braking, Approaching, Launching, Orbiting };
private Modes Mode = Modes.Idle;
private Func<IMyThrust, bool> ThrusterCondition = null;
private bool Autodrop = false;
private double TargetElevation, BrakingElevation;
private Func<IMyThrust, bool> AutoThrusterCondition = null;
private double MinimumSpeed;
private Vector3D? DropTarget = null;
private Func<ZACommons, EventDriver, bool> LivenessCheck = null;
private double Elevation, Distance;
private const uint OrbitOnDelay = 5;
private const uint OrbitOffDelay = 85;
private ulong OrbitTicks = 0;
private Base6Directions.Direction BurnDirection, BrakeDirection, LaunchDirection;
public void Init(ZACommons commons, EventDriver eventDriver,
ZACustomData customData,
Func<ZACommons, EventDriver, bool> livenessCheck = null)
{
BurnDirection = customData.GetDirection("burnDirection", VTVLHELPER_BURN_DIRECTION);
BrakeDirection = customData.GetDirection("brakeDirection", VTVLHELPER_BRAKE_DIRECTION);
LaunchDirection = customData.GetDirection("launchDirection", VTVLHELPER_LAUNCH_DIRECTION);
LivenessCheck = livenessCheck;
var lastCommand = commons.GetValue(LastCommandKey);
if (lastCommand != null)
{
HandleCommand(commons, eventDriver, lastCommand);
}
}
public void HandleCommand(ZACommons commons, EventDriver eventDriver,
string argument)
{
argument = argument.Trim().ToLower();
var parts = argument.Split(new char[] { ' ' }, 10);
if (parts.Length < 2) return;
var command = parts[0];
var subcommand = parts[1];
if (command == "drop")
{
var shipControl = (ShipControlCommons)commons;
if (subcommand == "start")
{
ThrusterCondition = parts.Length > 2 ? ParseThrusterFlags(parts[2]) : null;
shipControl.Reset(gyroOverride: false, thrusterEnable: true,
thrusterCondition: ThrusterCondition);
cruiser.Init(shipControl,
localForward: BurnDirection);
if (Mode != Modes.Burning)
{
Mode = Modes.Burning;
Autodrop = false;
eventDriver.Schedule(0, Burn);
}
SaveLastCommand(commons, argument);
}
else if (subcommand == "brake" || subcommand == "descend")
{
ThrusterCondition = parts.Length > 2 ? ParseThrusterFlags(parts[2]) : null;
shipControl.Reset(gyroOverride: true, thrusterEnable: true,
thrusterCondition: ThrusterCondition);
var down = shipControl.ShipBlockOrientation.TransformDirection(BrakeDirection);
seeker.Init(shipControl,
shipUp: Base6Directions.GetPerpendicular(down),
shipForward: down);
cruiser.Init(shipControl,
localForward: BrakeDirection);
if (Mode != Modes.Braking)
{
Mode = Modes.Braking;
eventDriver.Schedule(FramesPerRun, Brake);
}
SaveLastCommand(commons, argument);
}
else if (subcommand == "abort" || subcommand == "stop" ||
subcommand == "reset")
{
Reset(shipControl);
}
else if (subcommand == "auto")
{
ThrusterCondition = null;
AutoThrusterCondition = null;
TargetElevation = 1000.0;
if (parts.Length > 2)
{
if (double.TryParse(parts[2], out TargetElevation))
{
TargetElevation = Math.Max(0.0, TargetElevation);
}
else
{
TargetElevation = 1000.0;
}
}
if (parts.Length > 3) ThrusterCondition = ParseThrusterFlags(parts[3]);
BrakingElevation = TargetElevation;
if (parts.Length > 4)
{
if (double.TryParse(parts[4], out BrakingElevation))
{
BrakingElevation = Math.Max(TargetElevation, BrakingElevation);
}
else
{
BrakingElevation = TargetElevation;
}
}
if (parts.Length > 5) AutoThrusterCondition = ParseThrusterFlags(parts[5]);
MinimumSpeed = VTVLHELPER_MINIMUM_SPEED;
if (parts.Length > 6)
{
if (double.TryParse(parts[6], out MinimumSpeed))
{
MinimumSpeed = Math.Max(1.0, MinimumSpeed);
}
else
{
MinimumSpeed = VTVLHELPER_MINIMUM_SPEED;
}
}
DropTarget = null;
if (parts.Length > 9)
{
double x, y, z;
if (double.TryParse(parts[7], out x) &&
double.TryParse(parts[8], out y) &&
double.TryParse(parts[9], out z))
{
DropTarget = new Vector3D(x, y, z);
}
}
shipControl.Reset(gyroOverride: false, thrusterEnable: true,
thrusterCondition: ThrusterCondition);
cruiser.Init(shipControl,
localForward: BurnDirection);
if (Mode != Modes.Burning)
{
Mode = Modes.Burning;
Autodrop = true;
eventDriver.Schedule(0, Burn);
}
SaveLastCommand(commons, argument);
}
}
else if (command == "launch")
{
var shipControl = (ShipControlCommons)commons;
if (subcommand == "start")
{
ThrusterCondition = parts.Length > 2 ? ParseThrusterFlags(parts[2]) : null;
shipControl.Reset(gyroOverride: true, thrusterEnable: true,
thrusterCondition: ThrusterCondition);
var forward = shipControl.ShipBlockOrientation.TransformDirection(LaunchDirection);
seeker.Init(shipControl,
shipUp: Base6Directions.GetPerpendicular(forward),
shipForward: forward);
cruiser.Init(shipControl,
localForward: LaunchDirection);
if (Mode != Modes.Launching)
{
Mode = Modes.Launching;
eventDriver.Schedule(FramesPerRun, Launch);
}
SaveLastCommand(commons, argument);
}
else if (subcommand == "abort" || subcommand == "stop" ||
subcommand == "reset")
{
Reset(shipControl);
}
}
else if (command == "orbit")
{
var shipControl = (ShipControlCommons)commons;
if (subcommand == "start")
{
OrbitInit(shipControl);
if (Mode != Modes.Orbiting)
{
OrbitTicks = 0;
Mode = Modes.Orbiting;
eventDriver.Schedule(0, OrbitOn);
}
SaveLastCommand(commons, argument);
}
else if (subcommand == "abort" || subcommand == "stop" ||
subcommand == "reset")
{
ResetOrbit(shipControl);
}
}
}
public void Burn(ZACommons commons, EventDriver eventDriver)
{
if (ShouldAbort(commons, eventDriver, Modes.Burning, false)) return;
var shipControl = (ShipControlCommons)commons;
var controller = GetShipController(shipControl);
if (controller == null) return;
var gravity = controller.GetNaturalGravity();
if (gravity.LengthSquared() > 0.0)
{
shipControl.Reset(gyroOverride: true, thrusterEnable: true,
thrusterCondition: ThrusterCondition);
shipControl.ThrustControl.Enable(Base6Directions.GetFlippedDirection(BrakeDirection), false);
var down = shipControl.ShipBlockOrientation.TransformDirection(BrakeDirection);
seeker.Init(shipControl,
shipUp: Base6Directions.GetPerpendicular(down),
shipForward: down);
if (Autodrop)
{
var forward = Base6Directions.GetPerpendicular(BrakeDirection);
var right = Base6Directions.GetCross(forward, BrakeDirection);
LongCruiser.Init(shipControl, localForward: forward);
LatCruiser.Init(shipControl, localForward: right);
}
Mode = Modes.Gliding;
eventDriver.Schedule(FramesPerRun, Glide);
}
else
{
cruiser.Cruise(shipControl, VTVLHELPER_BURN_SPEED,
condition: ThrusterCondition);
eventDriver.Schedule(FramesPerRun, Burn);
}
}
public void Glide(ZACommons commons, EventDriver eventDriver)
{
if (ShouldAbort(commons, eventDriver, Modes.Gliding, Autodrop)) return;
var shipControl = (ShipControlCommons)commons;
var controller = GetShipController(shipControl);
if (controller == null) return;
var gravity = controller.GetNaturalGravity();
if (gravity.LengthSquared() > 0.0)
{
double yawPitchError;
seeker.Seek(shipControl, gravity, out yawPitchError);
if (Autodrop)
{
if (!controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out Elevation)) Elevation = 0.0;
Alignment(shipControl, controller);
Distance = Elevation - BrakingElevation;
if (Elevation < BrakingElevation)
{
shipControl.Reset(gyroOverride: true, thrusterEnable: true,
thrusterCondition: ThrusterCondition);
ThrusterCondition = AutoThrusterCondition;
cruiser.Init(shipControl,
localForward: BrakeDirection);
Mode = Modes.Approaching;
eventDriver.Schedule(FramesPerRun, Approach);
}
else
{
eventDriver.Schedule(FramesPerRun, Glide);
}
}
else
{
eventDriver.Schedule(FramesPerRun, Glide);
}
}
else
{
Reset(shipControl);
}
}
public void Brake(ZACommons commons, EventDriver eventDriver)
{
if (ShouldAbort(commons, eventDriver, Modes.Braking, Autodrop)) return;
var shipControl = (ShipControlCommons)commons;
var controller = GetShipController(shipControl);
if (controller == null) return;
var gravity = controller.GetNaturalGravity();
if (gravity.LengthSquared() > 0.0)
{
double yawPitchError;
seeker.Seek(shipControl, gravity, out yawPitchError);
cruiser.Cruise(shipControl, VTVLHELPER_BRAKING_SPEED,
condition: ThrusterCondition,
enableForward: false);
eventDriver.Schedule(FramesPerRun, Brake);
}
else
{
Reset(shipControl);
}
}
public void Approach(ZACommons commons, EventDriver eventDriver)
{
if (Mode != Modes.Approaching) return;
var shipControl = (ShipControlCommons)commons;
var controller = GetShipController(shipControl);
if (controller == null) return;
var gravity = controller.GetNaturalGravity();
if (gravity.LengthSquared() > 0.0)
{
double yawPitchError;
seeker.Seek(shipControl, gravity, out yawPitchError);
if (!controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out Elevation)) Elevation = 0.0;
Distance = Elevation - TargetElevation;
if (Elevation <= TargetElevation)
{
Reset(shipControl);
TimerBlockUtils.StartTimerBlockWithName(commons.Blocks, VTVLHELPER_DROP_DONE);
}
else
{
var targetSpeed = Math.Min(Distance * VTVLHELPER_APPROACH_GAIN,
VTVLHELPER_BRAKING_SPEED);
targetSpeed = Math.Max(targetSpeed, MinimumSpeed);
cruiser.Cruise(shipControl, targetSpeed,
condition: ThrusterCondition,
enableForward: false);
Alignment(shipControl, controller);
eventDriver.Schedule(FramesPerRun, Approach);
}
}
else
{
Reset(shipControl);
}
}
public void Launch(ZACommons commons, EventDriver eventDriver)
{
if (Mode != Modes.Launching) return;
var shipControl = (ShipControlCommons)commons;
var controller = GetShipController(shipControl);
if (controller == null) return;
var gravity = controller.GetNaturalGravity();
if (gravity.LengthSquared() > 0.0)
{
double yawPitchError;
seeker.Seek(shipControl, -gravity, out yawPitchError);
cruiser.Cruise(shipControl, VTVLHELPER_LAUNCH_SPEED,
condition: ThrusterCondition,
enableBackward: false);
eventDriver.Schedule(FramesPerRun, Launch);
}
else
{
Reset(shipControl);
TimerBlockUtils.StartTimerBlockWithName(commons.Blocks, VTVLHELPER_LAUNCH_DONE);
}
}
private void OrbitInit(ShipControlCommons shipControl)
{
shipControl.GyroControl.Reset();
shipControl.GyroControl.EnableOverride(true);
var forward = shipControl.ShipBlockOrientation.TransformDirection(VTVLHELPER_ORBIT_DIRECTION);
seeker.Init(shipControl,
shipUp: Base6Directions.GetPerpendicular(forward),
shipForward: forward);
}
public void OrbitOn(ZACommons commons, EventDriver eventDriver)
{
if (Mode != Modes.Orbiting) return;
var shipControl = (ShipControlCommons)commons;
var controller = GetShipController(shipControl);
if (controller == null) return;
var gravity = controller.GetNaturalGravity();
if (gravity.LengthSquared() > 0.0)
{
double yawPitchError;
seeker.Seek(shipControl, gravity, out yawPitchError);
OrbitTicks++;
if (OrbitTicks >= OrbitOnDelay)
{
shipControl.GyroControl.Reset();
shipControl.GyroControl.EnableOverride(false);
eventDriver.Schedule(OrbitOffDelay, OrbitOff);
}
else
{
eventDriver.Schedule(FramesPerRun, OrbitOn);
}
}
else
{
ResetOrbit(shipControl);
}
}
public void OrbitOff(ZACommons commons, EventDriver eventDriver)
{
if (Mode != Modes.Orbiting) return;
var shipControl = (ShipControlCommons)commons;
OrbitInit(shipControl);
OrbitTicks = 0;
eventDriver.Schedule(FramesPerRun, OrbitOn);
}
public void Display(ZACommons commons)
{
switch (Mode)
{
case Modes.Idle:
break;
case Modes.Burning:
commons.Echo("VTVL: Burn phase");
if (Autodrop) commons.Echo("Auto-drop starting");
break;
case Modes.Gliding:
commons.Echo("VTVL: Glide phase");
if (Autodrop)
{
commons.Echo(string.Format("Elevation: {0:F2} m", Elevation));
commons.Echo(string.Format("Brake Distance: {0:F2} m", Distance));
}
break;
case Modes.Braking:
commons.Echo("VTVL: Braking");
break;
case Modes.Approaching:
commons.Echo("VTVL: Approach phase");
commons.Echo(string.Format("Elevation: {0:F2} m", Elevation));
commons.Echo(string.Format("Stop Distance: {0:F2} m", Distance));
break;
case Modes.Launching:
commons.Echo("VTVL: Launching");
break;
case Modes.Orbiting:
commons.Echo("VTVL: Orbiting");
break;
}
}
private void Reset(ShipControlCommons shipControl)
{
shipControl.Reset(gyroOverride: false, thrusterEnable: true,
thrusterCondition: ThrusterCondition);
Mode = Modes.Idle;
SaveLastCommand(shipControl, null);
}
private void ResetOrbit(ShipControlCommons shipControl)
{
var gyroControl = shipControl.GyroControl;
gyroControl.Reset();
gyroControl.EnableOverride(false);
Mode = Modes.Idle;
SaveLastCommand(shipControl, null);
}
private IMyShipController GetShipController(ShipControlCommons shipControl)
{
if (shipControl.ShipController == null)
{
if (Mode == Modes.Orbiting)
{
ResetOrbit(shipControl);
}
else
{
Reset(shipControl);
}
}
return shipControl.ShipController;
}
private Func<IMyThrust, bool> ParseThrusterFlags(string flags)
{
if (flags == null) return null; // Don't do extra work
var useIon = flags.IndexOf('i') >= 0;
var useH = flags.IndexOf('h') >= 0;
var useAtm = flags.IndexOf('a') >= 0;
return thruster =>
{
var defName = thruster.DefinitionDisplayNameText;
var isH = defName.IndexOf("Hydrogen") >= 0;
var isAtm = defName.IndexOf("Atmospheric") >= 0;
return ((isH && useH) ||
(isAtm && useAtm) ||
(!isH && !isAtm && useIon));
};
}
private void SaveLastCommand(ZACommons commons, string argument)
{
commons.SetValue(LastCommandKey, argument);
}
private bool ShouldAbort(ZACommons commons, EventDriver eventDriver,
Modes expectedMode, bool ignoreLiveness)
{
if (!ignoreLiveness && LivenessCheck != null &&
!LivenessCheck(commons, eventDriver))
{
Reset((ShipControlCommons)commons);
}
return Mode != expectedMode;
}
private void Alignment(ShipControlCommons shipControl, IMyShipController controller)
{
Vector3D center;
if (DropTarget == null || !controller.TryGetPlanetPosition(out center)) return;
var targetRayDirection = Vector3D.Normalize((Vector3D)DropTarget - center);
var myRayLength = (shipControl.ReferencePoint - center).Length();
var targetPosition = center + targetRayDirection * myRayLength;
var targetOffset = targetPosition - shipControl.ReferencePoint;
AlignmentThrust(shipControl, targetOffset, LongCruiser);
AlignmentThrust(shipControl, targetOffset, LatCruiser);
}
private void AlignmentThrust(ShipControlCommons shipControl, Vector3D offset, Cruiser cruiser)
{
var velocity = shipControl.LinearVelocity;
if (velocity != null)
{
var referenceDirection = GetReferenceVector(shipControl, cruiser.LocalForward);
var referenceDistance = Vector3D.Dot(offset, referenceDirection);
var targetSpeed = Math.Min(Math.Abs(referenceDistance) * VTVLHELPER_APPROACH_GAIN, VTVLHELPER_MAXIMUM_SPEED);
targetSpeed *= Math.Sign(referenceDistance);
Func<IMyThrust, bool> AlignThrusterCondition = VTVLHELPER_USE_BRAKING_THRUSTER_SPEC_FOR_ALIGN ? ThrusterCondition : null;
cruiser.Cruise(shipControl, targetSpeed, (Vector3D)velocity,
condition: AlignThrusterCondition);
}
}
private Vector3D GetReferenceVector(ShipControlCommons shipControl, Base6Directions.Direction direction)
{
var offset = shipControl.Me.Position + Base6Directions.GetIntVector(shipControl.ShipBlockOrientation.TransformDirection(direction));
return Vector3D.Normalize(shipControl.Me.CubeGrid.GridIntegerToWorld(offset) - shipControl.Me.GetPosition());
}
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

public class Cruiser
{
private const double ThrustKp = 1.0;
private const double ThrustTi = 5.0;
private const double ThrustTd = 0.1;
private readonly PIDController thrustPID;
private readonly double ThrustDeadZone;
public Base6Directions.Direction LocalForward { get; private set; }
public Base6Directions.Direction LocalBackward { get; private set; }
public Cruiser(double dt, double thrustDeadZone)
{
thrustPID = new PIDController(dt);
thrustPID.Kp = ThrustKp;
thrustPID.Ti = ThrustTi;
thrustPID.Td = ThrustTd;
ThrustDeadZone = thrustDeadZone;
}
public void Init(ShipControlCommons shipControl,
Base6Directions.Direction localForward = Base6Directions.Direction.Forward)
{
LocalForward = localForward;
LocalBackward = Base6Directions.GetFlippedDirection(LocalForward);
thrustPID.Reset();
}
public bool Cruise(ShipControlCommons shipControl,
double targetSpeed,
Func<IMyThrust, bool> condition = null,
bool enableForward = true,
bool enableBackward = true)
{
var velocity = shipControl.LinearVelocity;
if (velocity != null)
{
Cruise(shipControl, targetSpeed, (Vector3D)velocity, condition,
enableForward, enableBackward);
}
return velocity != null;
}
public void Cruise(ShipControlCommons shipControl, double targetSpeed,
Vector3D velocity,
Func<IMyThrust, bool> condition = null,
bool enableForward = true,
bool enableBackward = true)
{
var forward3I = shipControl.Me.Position + Base6Directions.GetIntVector(shipControl.ShipBlockOrientation.TransformDirection(LocalForward));
var referenceForward = Vector3D.Normalize(shipControl.Me.CubeGrid.GridIntegerToWorld(forward3I) - shipControl.Me.GetPosition());
var speed = Vector3D.Dot(velocity, referenceForward);
var error = targetSpeed - speed;
var force = thrustPID.Compute(error);
var thrustControl = shipControl.ThrustControl;
if (Math.Abs(error) < ThrustDeadZone * targetSpeed)
{
thrustControl.Enable(LocalForward, false, condition);
thrustControl.Enable(LocalBackward, false, condition);
}
else if (force > 0.0)
{
thrustControl.Enable(LocalForward, enableForward, condition);
if (enableForward) thrustControl.SetOverride(LocalForward, force, condition);
thrustControl.Enable(LocalBackward, false, condition);
}
else
{
thrustControl.Enable(LocalForward, false, condition);
thrustControl.Enable(LocalBackward, enableBackward, condition);
if (enableBackward) thrustControl.SetOverride(LocalBackward, -force, condition);
}
}
}

public class CruiseControl
{
private const string LastCommandKey = "CruiseControl_LastCommand";
private const uint FramesPerRun = 2;
private const double RunsPerSecond = 60.0 / FramesPerRun;
private readonly PIDController thrustPID = new PIDController(1.0 / RunsPerSecond);
private const double ThrustKp = 1.0;
private const double ThrustTi = 5.0;
private const double ThrustTd = 0.1;
private bool Active = false;
private double TargetSpeed, CurrentSpeed;
private Base6Directions.Direction CruiseDirection;
private string CruiseFlags;
private bool FirstStop = false;
public struct ThrusterState
{
public bool Enabled;
public float Override;
public ThrusterState(IMyThrust thruster)
{
Enabled = thruster.Enabled;
Override = thruster.ThrustOverridePercentage;
}
}
private readonly Dictionary<Vector3I, ThrusterState> ThrusterStates = new Dictionary<Vector3I, ThrusterState>();
private Func<ZACommons, EventDriver, bool> LivenessCheck = null;
public CruiseControl()
{
thrustPID.Kp = ThrustKp;
thrustPID.Ti = ThrustTi;
thrustPID.Td = ThrustTd;
}
public void Init(ZACommons commons, EventDriver eventDriver,
Func<ZACommons, EventDriver, bool> livenessCheck = null)
{
LivenessCheck = livenessCheck;
var lastCommand = commons.GetValue(LastCommandKey);
if (lastCommand != null)
{
HandleCommand(commons, eventDriver, lastCommand);
ThrusterStates.Clear();
FirstStop = true;
}
}
private void Reset(ZACommons commons)
{
var thrustControl = ((ShipControlCommons)commons).ThrustControl;
var collect = ParseCruiseFlags();
thrustControl.Enable(true, collect);
thrustControl.Reset(collect);
}
public void HandleCommand(ZACommons commons, EventDriver eventDriver,
string argument)
{
argument = argument.Trim().ToLower();
var parts = argument.Split(new char[] { ' ' }, 4);
if (parts.Length < 2) return;
var command = parts[0];
var speed = parts[1];
if (command == "cruise")
{
if (speed == "reset")
{
CruiseFlags = null;
if (parts.Length >= 3) CruiseFlags = parts[2];
Reset(commons);
ThrusterStates.Clear();
Active = false;
SaveLastCommand(commons, null);
}
else if (speed == "stop" || speed == "reverse")
{
RestoreThrusterStates(commons);
Active = false;
SaveLastCommand(commons, null);
if (speed == "reverse")
{
new ReverseThrust().Init(commons, eventDriver, REVERSE_THRUST_MAX_GYRO_ERROR);
}
}
else
{
CruiseDirection = Base6Directions.Direction.Forward;
if (parts.Length >= 3)
{
switch (parts[2])
{
case "forward":
case "forwards":
default:
break;
case "backward":
case "backwards":
case "reverse":
CruiseDirection = Base6Directions.Direction.Backward;
break;
case "left":
CruiseDirection = Base6Directions.Direction.Left;
break;
case "right":
CruiseDirection = Base6Directions.Direction.Right;
break;
case "up":
CruiseDirection = Base6Directions.Direction.Up;
break;
case "down":
CruiseDirection = Base6Directions.Direction.Down;
break;
}
}
CruiseFlags = null;
if (parts.Length == 4) CruiseFlags = parts[3];
double desiredSpeed;
if (speed == "set" || speed == "current") {
var shipControl = (ShipControlCommons)commons;
var velocity = shipControl.LinearVelocity;
if (velocity != null) {
desiredSpeed = ((Vector3D)velocity).Length();
CruiseStart(commons, eventDriver, desiredSpeed, argument);
}
}
else if (double.TryParse(speed, out desiredSpeed))
{
CruiseStart(commons, eventDriver, desiredSpeed, argument);
}
}
}
}
private void CruiseStart(ZACommons commons, EventDriver eventDriver, double desiredSpeed, string argument)
{
TargetSpeed = Math.Max(desiredSpeed, 0.0);
thrustPID.Reset();
if (!Active)
{
SaveThrusterStates(commons);
Active = true;
eventDriver.Schedule(0, Run);
}
SaveLastCommand(commons, argument);
}
private Func<IMyThrust, bool> ParseCruiseFlags()
{
if (CruiseFlags == null) return null; // Don't do extra work
var useIon = CruiseFlags.IndexOf('i') >= 0;
var useH = CruiseFlags.IndexOf('h') >= 0;
var useAtm = CruiseFlags.IndexOf('a') >= 0;
return thruster =>
{
var defName = thruster.DefinitionDisplayNameText;
var isH = defName.IndexOf("Hydrogen") >= 0;
var isAtm = defName.IndexOf("Atmospheric") >= 0;
return ((isH && useH) ||
(isAtm && useAtm) ||
(!isH && !isAtm && useIon));
};
}
public void Run(ZACommons commons, EventDriver eventDriver)
{
var shipControl = (ShipControlCommons)commons;
ResetIfNotLive(commons, eventDriver);
if (!Active) return;
var velocity = shipControl.LinearVelocity;
if (velocity != null)
{
var cruiseDirectionFlipped = Base6Directions.GetFlippedDirection(CruiseDirection);
var forward3I = shipControl.Me.Position + Base6Directions.GetIntVector(shipControl.ShipBlockOrientation.TransformDirection(CruiseDirection));
var forward = Vector3D.Normalize(shipControl.Me.CubeGrid.GridIntegerToWorld(forward3I) - shipControl.Me.GetPosition());
CurrentSpeed = Vector3D.Dot((Vector3D)velocity, forward);
var error = TargetSpeed - CurrentSpeed;
var force = thrustPID.Compute(error);
var thrustControl = shipControl.ThrustControl;
var collect = ParseCruiseFlags();
if (Math.Abs(error) < CRUISE_CONTROL_DEAD_ZONE * TargetSpeed)
{
thrustControl.Enable(CruiseDirection, false, collect);
thrustControl.Enable(cruiseDirectionFlipped, false, collect);
}
else if (force > 0.0)
{
thrustControl.Enable(CruiseDirection, true, collect);
thrustControl.SetOverride(CruiseDirection, force, collect);
thrustControl.Enable(cruiseDirectionFlipped, false, collect);
}
else
{
thrustControl.Enable(CruiseDirection, false, collect);
thrustControl.Enable(cruiseDirectionFlipped, true, collect);
thrustControl.SetOverride(cruiseDirectionFlipped, -force, collect);
}
}
eventDriver.Schedule(FramesPerRun, Run);
}
public void Display(ZACommons commons)
{
if (Active)
{
commons.Echo("Cruise control active");
commons.Echo(string.Format("Set Speed: {0:F1} m/s", TargetSpeed));
commons.Echo(string.Format("Actual Speed: {0:F1} m/s", CurrentSpeed));
}
}
private void SaveLastCommand(ZACommons commons, string argument)
{
commons.SetValue(LastCommandKey, argument);
}
private void ResetIfNotLive(ZACommons commons, EventDriver eventDriver)
{
if (LivenessCheck != null && !LivenessCheck(commons, eventDriver))
{
RestoreThrusterStates(commons);
Active = false;
SaveLastCommand(commons, null);
}
}
private void SaveThrusterStates(ZACommons commons)
{
ThrusterStates.Clear();
var thrusters = ZACommons.GetBlocksOfType<IMyThrust>(commons.Blocks);
thrusters.ForEach(thruster => {
ThrusterStates.Add(thruster.Position, new ThrusterState(thruster));
});
}
private void RestoreThrusterStates(ZACommons commons)
{
if (ThrusterStates.Count > 0)
{
var thrusters = ZACommons.GetBlocksOfType<IMyThrust>(commons.Blocks);
thrusters.ForEach(thruster => {
ThrusterState oldState;
if (ThrusterStates.TryGetValue(thruster.Position, out oldState))
{
thruster.Enabled = oldState.Enabled;
thruster.ThrustOverridePercentage = oldState.Override;
}
});
ThrusterStates.Clear();
}
else if (FirstStop)
{
Reset(commons);
}
FirstStop = false;
}
}

public class ReverseThrust
{
private const uint FramesPerRun = 1;
private const double RunsPerSecond = 60.0 / FramesPerRun;
private readonly Seeker seeker = new Seeker(1.0 / RunsPerSecond);
private const uint SampleDelay = 60;
private Vector3D LastPosition;
private double MaxError;
private Base6Directions.Direction ThrusterDirection;
private bool Enabled;
private Vector3D TargetVector;
public void Init(ZACommons commons, EventDriver eventDriver,
double maxError,
Base6Directions.Direction thrusterDirection = Base6Directions.Direction.Forward)
{
MaxError = maxError;
ThrusterDirection = thrusterDirection;
var shipControl = (ShipControlCommons)commons;
var forward = shipControl.ShipBlockOrientation.TransformDirection(ThrusterDirection);
seeker.Init(shipControl,
shipUp: Base6Directions.GetPerpendicular(forward),
shipForward: forward);
var gyroControl = shipControl.GyroControl;
gyroControl.Reset();
gyroControl.EnableOverride(true);
LastPosition = shipControl.ReferencePoint;
Enabled = true;
shipControl.ThrustControl.Enable(false);
eventDriver.Schedule(SampleDelay, DetermineVelocity);
}
public void DetermineVelocity(ZACommons commons, EventDriver eventDriver)
{
if (!Enabled) return;
var shipControl = (ShipControlCommons)commons;
var velocity = (shipControl.ReferencePoint - LastPosition) /
((double)SampleDelay / 60.0);
TargetVector = -velocity;
var speed = TargetVector.Normalize();
if (speed > 0.1)
{
eventDriver.Schedule(FramesPerRun, Reorient);
}
else
{
var gyroControl = shipControl.GyroControl;
gyroControl.Reset();
gyroControl.EnableOverride(false);
shipControl.ThrustControl.Enable(true);
}
}
public void Reorient(ZACommons commons, EventDriver eventDriver)
{
if (!Enabled) return;
var shipControl = (ShipControlCommons)commons;
double yawPitchError;
var gyroControl = seeker.Seek(shipControl, TargetVector,
out yawPitchError);
if (yawPitchError < MaxError)
{
gyroControl.Reset();
shipControl.ThrustControl.Enable(true);
gyroControl.Reset();
gyroControl.EnableOverride(false);
}
else
{
eventDriver.Schedule(FramesPerRun, Reorient);
}
}
public void Reset(ZACommons commons)
{
var shipControl = (ShipControlCommons)commons;
var gyroControl = shipControl.GyroControl;
gyroControl.Reset();
gyroControl.EnableOverride(false);
shipControl.ThrustControl.Enable(true);
Enabled = false;
}
}

public class Seeker
{
private const double AngleKp = 5.0;
private const double AngleTi = 0.0;
private const double AngleTd = 0.08;
private const double VelKp = 1.0;
private const double VelTi = 0.0;
private const double VelTd = 0.08;
private readonly PIDController yawPID, pitchPID, rollPID;
private readonly PIDController yawVPID, pitchVPID, rollVPID;
private Base6Directions.Direction ShipForward, ShipUp, ShipLeft;
public double ControlThreshold { get; set; }
public Seeker(double dt)
{
yawPID = new PIDController(dt);
pitchPID = new PIDController(dt);
rollPID = new PIDController(dt);
yawVPID = new PIDController(dt);
pitchVPID = new PIDController(dt);
rollVPID = new PIDController(dt);
ControlThreshold = 0.01;
}
public void Init(ShipControlCommons shipControl,
Base6Directions.Direction shipUp = Base6Directions.Direction.Up,
Base6Directions.Direction shipForward = Base6Directions.Direction.Forward)
{
ShipForward = shipForward;
ShipUp = shipUp;
ShipLeft = Base6Directions.GetLeft(ShipUp, ShipForward);
double maxVel = Math.PI / 4.0;
yawPID.Kp = AngleKp;
yawPID.Ti = AngleTi;
yawPID.Td = AngleTd;
yawPID.min = -maxVel;
yawPID.max = maxVel;
pitchPID.Kp = AngleKp;
pitchPID.Ti = AngleTi;
pitchPID.Td = AngleTd;
pitchPID.min = -maxVel;
pitchPID.max = maxVel;
rollPID.Kp = AngleKp / 2.0; // Don't ask
rollPID.Ti = AngleTi;
rollPID.Td = AngleTd;
rollPID.min = -maxVel;
rollPID.max = maxVel;
yawVPID.Kp = VelKp;
yawVPID.Ti = VelTi;
yawVPID.Td = VelTd;
yawVPID.min = -Math.PI;
yawVPID.max = Math.PI;
pitchVPID.Kp = VelKp;
pitchVPID.Ti = VelTi;
pitchVPID.Td = VelTd;
pitchVPID.min = -Math.PI;
pitchVPID.max = Math.PI;
rollVPID.Kp = VelKp / 2.0; // Don't ask
rollVPID.Ti = VelTi;
rollVPID.Td = VelTd;
rollVPID.min = -Math.PI;
rollVPID.max = Math.PI;
yawPID.Reset();
pitchPID.Reset();
rollPID.Reset();
yawVPID.Reset();
pitchVPID.Reset();
rollVPID.Reset();
}
public GyroControl Seek(ShipControlCommons shipControl,
Vector3D targetVector,
out double yawPitchError)
{
double rollError;
return _Seek(shipControl, targetVector, null,
out yawPitchError, out rollError);
}
public GyroControl Seek(ShipControlCommons shipControl,
Vector3D targetVector, Vector3D targetUp,
out double yawPitchError, out double rollError)
{
return _Seek(shipControl, targetVector, targetUp,
out yawPitchError, out rollError);
}
private GyroControl _Seek(ShipControlCommons shipControl,
Vector3D targetVector, Vector3D? targetUp,
out double yawPitchError, out double rollError)
{
var angularVelocity = shipControl.AngularVelocity;
if (angularVelocity == null)
{
yawPitchError = rollError = Math.PI;
return shipControl.GyroControl;
}
Vector3D referenceForward;
Vector3D referenceLeft;
Vector3D referenceUp;
GyroControl gyroControl;
if (shipControl.ShipUp == ShipUp && shipControl.ShipForward == ShipForward)
{
referenceForward = shipControl.ReferenceForward;
referenceLeft = shipControl.ReferenceLeft;
referenceUp = shipControl.ReferenceUp;
gyroControl = shipControl.GyroControl;
}
else
{
referenceForward = GetReferenceVector(shipControl, ShipForward);
referenceLeft = GetReferenceVector(shipControl, ShipLeft);
referenceUp = GetReferenceVector(shipControl, ShipUp);
gyroControl = new GyroControl();
gyroControl.Init(shipControl.Blocks,
shipUp: ShipUp,
shipForward: ShipForward);
}
targetVector = Vector3D.Normalize(targetVector);
var toLocal = MatrixD.Transpose(MatrixD.CreateWorld(Vector3D.Zero, referenceForward, referenceUp));
var localTarget = Vector3D.TransformNormal(-targetVector, toLocal);
var localVel = Vector3D.TransformNormal((Vector3D)angularVelocity, toLocal);
var yawError = Math.Atan2(localTarget.X, localTarget.Z);
var pitchError = Math.Atan2(localTarget.Y, localTarget.Z);
var desiredYawVel = yawPID.Compute(yawError);
var desiredPitchVel = pitchPID.Compute(pitchError);
double gyroYaw = 0.0;
if (Math.Abs(desiredYawVel) >= ControlThreshold)
{
gyroYaw = yawVPID.Compute(desiredYawVel - localVel.X);
}
double gyroPitch = 0.0;
if (Math.Abs(desiredPitchVel) >= ControlThreshold)
{
gyroPitch = pitchVPID.Compute(desiredPitchVel - localVel.Y);
}
gyroControl.SetAxisVelocity(GyroControl.Yaw, (float)gyroYaw);
gyroControl.SetAxisVelocity(GyroControl.Pitch, (float)gyroPitch);
yawPitchError = Math.Acos(MathHelperD.Clamp(Vector3D.Dot(targetVector, referenceForward), -1.0, 1.0));
if (targetUp != null)
{
localTarget = Vector3D.TransformNormal((Vector3D)targetUp, toLocal);
rollError = Math.Atan2(localTarget.X, localTarget.Y);
var desiredRollVel = rollPID.Compute(rollError);
double gyroRoll = 0.0;
if (Math.Abs(desiredRollVel) >= ControlThreshold)
{
gyroRoll = rollVPID.Compute(desiredRollVel - localVel.Z);
}
gyroControl.SetAxisVelocity(GyroControl.Roll, (float)gyroRoll);
rollError = Math.Abs(rollError);
}
else
{
rollError = 0.0;
}
return gyroControl;
}
private Vector3D GetReferenceVector(ShipControlCommons shipControl,
Base6Directions.Direction direction)
{
var offset = shipControl.Me.Position + Base6Directions.GetIntVector(direction);
return Vector3D.Normalize(shipControl.Me.CubeGrid.GridIntegerToWorld(offset) - shipControl.Me.GetPosition());
}
}

public class PIDController
{
public readonly double dt; // i.e. 1.0 / ticks per second
public double min { get; set; }
public double max { get; set; }
public double Kp { get; set; }
public double Ki
{
get { return m_Ki; }
set { m_Ki = value; }
}
private double m_Ki;
public double Ti
{
get { return m_Ki != 0.0 ? Kp / m_Ki : 0.0; }
set
{
if (value != 0.0)
{
Ki = Kp / value;
}
else Ki = 0.0;
}
}
public double Kd
{
get { return m_Kd; }
set { m_Kd = value; m_Kddt = m_Kd / dt; }
}
private double m_Kd, m_Kddt;
public double Td
{
get { return Kd / Kp; }
set { Kd = Kp * value; }
}
private double integral = 0.0;
private double lastError = 0.0;
public PIDController(double dt)
{
this.dt = dt;
min = -1.0;
max = 1.0;
}
public void Reset()
{
integral = 0.0;
lastError = 0.0;
}
public double Compute(double error)
{
var newIntegral = integral + error * dt;
var derivative = error - lastError;
lastError = error;
var CV = ((Kp * error) +
(m_Ki * newIntegral) +
(m_Kddt * derivative));
if (CV > max)
{
if (newIntegral <= integral) integral = newIntegral;
return max;
}
else if (CV < min)
{
if (newIntegral >= integral) integral = newIntegral;
return min;
}
integral = newIntegral;
return CV;
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

public class SafeMode : DockingHandler
{
private const double FastRunDelay = 1.0;
private const double SlowRunDelay = 5.0;
private readonly SafeModeHandler[] SafeModeHandlers;
private readonly TimeSpan AbandonmentTimeout = TimeSpan.Parse(ABANDONMENT_TIMEOUT);
private bool? IsControlled = null;
private bool IsDocked = true;
private TimeSpan AbandonedTime;
public bool Abandoned { get; private set; }
public SafeMode(params SafeModeHandler[] safeModeHandlers)
{
SafeModeHandlers = safeModeHandlers;
AbandonedTime = AbandonmentTimeout;
Abandoned = false;
}
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
IsControlled = null;
ResetAbandonment(eventDriver);
IsDocked = false;
eventDriver.Schedule(FastRunDelay, Fast);
eventDriver.Schedule(SlowRunDelay, Slow);
}
}
public void Init(ZACommons commons, EventDriver eventDriver)
{
IsControlled = null;
ResetAbandonment(eventDriver);
IsDocked = false;
eventDriver.Schedule(0.0, Fast);
eventDriver.Schedule(0.0, Slow);
}
public void HandleCommand(ZACommons commons, EventDriver eventDriver,
string argument)
{
var command = argument.Trim().ToLower();
if (command == "safemode")
{
TriggerSafeMode(commons, eventDriver);
}
}
public void Fast(ZACommons commons, EventDriver eventDriver)
{
if (IsDocked) return; // Don't bother if we're docked
var controllers = ZACommons.GetBlocksOfType<IMyShipController>(commons.Blocks, controller => IsValidController(controller));
var currentState = IsShipControlled(controllers);
if (IsControlled == null || (bool)IsControlled != currentState)
{
IsControlled = currentState;
if (!(bool)IsControlled)
{
var dampenersChanged = false;
foreach (var block in controllers)
{
var controller = (IMyShipController)block;
if (!controller.DampenersOverride)
{
controller.DampenersOverride = true;
dampenersChanged = true;
}
}
if (dampenersChanged)
{
TriggerSafeMode(commons, eventDriver, EMERGENCY_STOP_NAME);
}
}
}
if (currentState) ResetAbandonment(eventDriver);
if (ABANDONMENT_ENABLED)
{
if (!Abandoned && !currentState)
{
if (AbandonedTime <= eventDriver.TimeSinceStart)
{
TriggerSafeMode(commons, eventDriver);
}
}
}
eventDriver.Schedule(FastRunDelay, Fast);
}
public void Slow(ZACommons commons, EventDriver eventDriver)
{
if (IsDocked) return; // Don't bother if we're docked
var controllers = ZACommons.GetBlocksOfType<IMyShipController>(commons.Blocks, controller => IsValidController(controller));
if (!Abandoned)
{
if (controllers.Count == 0)
{
TriggerSafeMode(commons, eventDriver);
}
else
{
var nonRemote = false;
foreach (var block in controllers)
{
var remote = block as IMyRemoteControl;
if (remote == null)
{
nonRemote = true;
break;
}
}
if (!nonRemote)
{
TriggerIfNoAntenna(commons, eventDriver);
}
}
}
eventDriver.Schedule(SlowRunDelay, Slow);
}
private bool IsValidController(IMyShipController controller)
{
if (!controller.IsFunctional) return false;
if (controller is IMyRemoteControl) return true;
switch (controller.DefinitionDisplayNameText)
{
case "Flight Seat":
return true;
case "Control Station":
return true;
case "Cockpit":
return true;
case "Fighter Cockpit":
return true;
}
return false;
}
private bool IsShipControlled(IEnumerable<IMyShipController> controllers)
{
foreach (var controller in controllers)
{
if (controller.IsUnderControl)
{
return true;
}
}
return false;
}
private void ResetAbandonment(EventDriver eventDriver)
{
AbandonedTime = eventDriver.TimeSinceStart + AbandonmentTimeout;
Abandoned = false;
}
private void TriggerSafeMode(ZACommons commons, EventDriver eventDriver,
string timerBlockName = SAFE_MODE_NAME)
{
Abandoned = true; // No need to trigger any other condition until reset
for (var i = 0; i < SafeModeHandlers.Length; i++)
{
SafeModeHandlers[i].SafeMode(commons, eventDriver);
}
TimerBlockUtils.StartTimerBlockWithName(commons.Blocks, timerBlockName);
}
private void TriggerIfNoAntenna(ZACommons commons, EventDriver eventDriver)
{
var antennaFound = false;
foreach (var block in commons.Blocks)
{
var antenna = block as IMyRadioAntenna;
if (antenna != null && antenna.IsWorking && antenna.Enabled) // && antenna.IsBroadcasting)
{
antennaFound = true;
break;
}
var lantenna = block as IMyLaserAntenna;
if (lantenna != null && lantenna.IsWorking && lantenna.Enabled && lantenna.Status == MyLaserAntennaStatus.Connected)
{
antennaFound = true;
break;
}
}
if (!antennaFound) TriggerSafeMode(commons, eventDriver); // We're deaf...
}
public void TriggerIfUncontrolled(ZACommons commons, EventDriver eventDriver)
{
if (!Abandoned && IsControlled != null && !(bool)IsControlled)
{
TriggerSafeMode(commons, eventDriver);
}
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

public interface SafeModeHandler
{
void SafeMode(ZACommons commons, EventDriver eventDriver);
}

public interface DockingHandler
{
void PreDock(ZACommons commons, EventDriver eventDriver);
void DockingAction(ZACommons commons, EventDriver eventDriver,
bool docked);
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

public class ShipControlCommons : ZACommons
{
private readonly ShipOrientation shipOrientation;
public Base6Directions.Direction ShipUp
{
get { return shipOrientation.ShipUp; }
}
public Base6Directions.Direction ShipForward
{
get { return shipOrientation.ShipForward; }
}
public MyBlockOrientation ShipBlockOrientation
{
get { return shipOrientation.BlockOrientation; }
}
public ShipControlCommons(MyGridProgram program, UpdateType updateType,
ShipOrientation shipOrientation,
string shipGroup = null,
ZAStorage storage = null)
: base(program, updateType, shipGroup: shipGroup, storage: storage)
{
this.shipOrientation = shipOrientation;
}
public GyroControl GyroControl
{
get
{
if (m_gyroControl == null)
{
m_gyroControl = new GyroControl();
m_gyroControl.Init(Blocks,
shipUp: shipOrientation.ShipUp,
shipForward: shipOrientation.ShipForward);
}
return m_gyroControl;
}
}
private GyroControl m_gyroControl = null;
public ThrustControl ThrustControl
{
get
{
if (m_thrustControl == null)
{
m_thrustControl = new ThrustControl();
m_thrustControl.Init(Blocks,
shipUp: shipOrientation.ShipUp,
shipForward: shipOrientation.ShipForward);
}
return m_thrustControl;
}
}
private ThrustControl m_thrustControl = null;
public void Reset(bool gyroOverride = false,
bool? thrusterEnable = true,
Func<IMyThrust, bool> thrusterCondition = null)
{
GyroControl.Reset();
GyroControl.EnableOverride(gyroOverride);
ThrustControl.Reset(thrusterCondition);
if (thrusterEnable != null) ThrustControl.Enable((bool)thrusterEnable, thrusterCondition);
}
public Vector3D ReferencePoint
{
get
{
if (m_referencePoint == null)
{
m_referencePoint = ShipController != null ? ShipController.CenterOfMass : Me.GetPosition();
}
return (Vector3D)m_referencePoint;
}
}
private Vector3D? m_referencePoint = null;
public Vector3D ReferenceUp
{
get
{
if (m_referenceUp == null)
{
m_referenceUp = GetReferenceVector(shipOrientation.ShipUp);
}
return (Vector3D)m_referenceUp;
}
}
private Vector3D? m_referenceUp = null;
public Vector3D ReferenceForward
{
get
{
if (m_referenceForward == null)
{
m_referenceForward = GetReferenceVector(shipOrientation.ShipForward);
}
return (Vector3D)m_referenceForward;
}
}
private Vector3D? m_referenceForward = null;
public Vector3D ReferenceLeft
{
get
{
if (m_referenceLeft == null)
{
m_referenceLeft = GetReferenceVector(Base6Directions.GetLeft(shipOrientation.ShipUp, shipOrientation.ShipForward));
}
return (Vector3D)m_referenceLeft;
}
}
private Vector3D? m_referenceLeft = null;
private Vector3D GetReferenceVector(Base6Directions.Direction direction)
{
var offset = Me.Position + Base6Directions.GetIntVector(direction);
return Vector3D.Normalize(Me.CubeGrid.GridIntegerToWorld(offset) - Me.GetPosition());
}
public IMyShipController ShipController
{
get
{
if (m_shipController == null)
{
foreach (var block in Blocks)
{
var controller = block as IMyShipController;
if (controller != null && controller.IsFunctional)
{
m_shipController = controller;
break;
}
}
}
return m_shipController;
}
}
private IMyShipController m_shipController = null;
public Vector3D? LinearVelocity
{
get
{
return ShipController != null ?
ShipController.GetShipVelocities().LinearVelocity : (Vector3D?)null;
}
}
public Vector3D? AngularVelocity
{
get
{
return ShipController != null ?
ShipController.GetShipVelocities().AngularVelocity : (Vector3D?)null;
}
}
}

public class ThrustControl
{
private readonly Dictionary<Base6Directions.Direction, List<IMyThrust>> thrusters = new Dictionary<Base6Directions.Direction, List<IMyThrust>>();
private void AddThruster(Base6Directions.Direction direction, IMyThrust thruster)
{
var thrusterList = GetThrusters(direction); // collect must be null to modify original list
thrusterList.Add(thruster);
}
public void Init(IEnumerable<IMyTerminalBlock> blocks,
Func<IMyThrust, bool> collect = null,
Base6Directions.Direction shipUp = Base6Directions.Direction.Up,
Base6Directions.Direction shipForward = Base6Directions.Direction.Forward)
{
MyBlockOrientation shipOrientation = new MyBlockOrientation(shipForward, shipUp);
thrusters.Clear();
foreach (var block in blocks)
{
var thruster = block as IMyThrust;
if (thruster != null && thruster.IsFunctional &&
(collect == null || collect(thruster)))
{
var facing = thruster.Orientation.TransformDirection(Base6Directions.Direction.Forward); // Exhaust goes this way
var thrustDirection = Base6Directions.GetFlippedDirection(facing);
var shipDirection = shipOrientation.TransformDirectionInverse(thrustDirection);
AddThruster(shipDirection, thruster);
}
}
}
public List<IMyThrust> GetThrusters(Base6Directions.Direction direction,
Func<IMyThrust, bool> collect = null,
bool disable = false)
{
List<IMyThrust> thrusterList;
if (!thrusters.TryGetValue(direction, out thrusterList))
{
thrusterList = new List<IMyThrust>();
thrusters.Add(direction, thrusterList);
}
if (collect == null)
{
return thrusterList;
}
else
{
var result = new List<IMyThrust>();
foreach (var thruster in thrusterList)
{
if (collect(thruster))
{
result.Add(thruster);
}
else if (disable)
{
thruster.Enabled = false;
}
}
return result;
}
}
public void SetOverride(Base6Directions.Direction direction, bool enable = true,
Func<IMyThrust, bool> collect = null)
{
var thrusterList = GetThrusters(direction, collect, true);
thrusterList.ForEach(thruster => thruster.ThrustOverridePercentage = enable ? 1f : 0f);
}
public void SetOverride(Base6Directions.Direction direction, double percent,
Func<IMyThrust, bool> collect = null)
{
var thrusterList = GetThrusters(direction, collect, true);
thrusterList.ForEach(thruster => thruster.ThrustOverridePercentage = (float)percent);
}
public void Enable(Base6Directions.Direction direction, bool enable,
Func<IMyThrust, bool> collect = null)
{
var thrusterList = GetThrusters(direction, collect, true);
thrusterList.ForEach(thruster => thruster.Enabled = enable);
}
public void Enable(bool enable,
Func<IMyThrust, bool> collect = null)
{
foreach (var thrusterList in thrusters.Values)
{
thrusterList.ForEach(thruster =>
{
if (collect == null || collect(thruster)) thruster.Enabled = enable;
});
}
}
public void Reset(Func<IMyThrust, bool> collect = null)
{
foreach (var thrusterList in thrusters.Values)
{
thrusterList.ForEach(thruster =>
{
if (collect == null || collect(thruster)) thruster.ThrustOverridePercentage = 0f;
});
}
}
}

public class GyroControl
{
public const int Yaw = 0;
public const int Pitch = 1;
public const int Roll = 2;
private readonly string[] AxisNames = new string[] { "Yaw", "Pitch", "Roll" };
public struct GyroAxisDetails
{
public int LocalAxis;
public int Sign;
public GyroAxisDetails(int localAxis, int sign)
{
LocalAxis = localAxis;
Sign = sign;
}
}
public struct GyroDetails
{
public IMyGyro Gyro;
public GyroAxisDetails[] AxisDetails;
public GyroDetails(IMyGyro gyro, Base6Directions.Direction shipUp,
Base6Directions.Direction shipForward)
{
Gyro = gyro;
AxisDetails = new GyroAxisDetails[3];
var shipLeft = Base6Directions.GetLeft(shipUp, shipForward);
SetAxisDetails(gyro, Yaw, shipUp);
SetAxisDetails(gyro, Pitch, shipLeft);
SetAxisDetails(gyro, Roll, shipForward);
}
private void SetAxisDetails(IMyGyro gyro, int axis,
Base6Directions.Direction axisDirection)
{
switch (gyro.Orientation.TransformDirectionInverse(axisDirection))
{
case Base6Directions.Direction.Up:
AxisDetails[axis] = new GyroAxisDetails(Yaw, -1);
break;
case Base6Directions.Direction.Down:
AxisDetails[axis] = new GyroAxisDetails(Yaw, 1);
break;
case Base6Directions.Direction.Left:
AxisDetails[axis] = new GyroAxisDetails(Pitch, -1);
break;
case Base6Directions.Direction.Right:
AxisDetails[axis] = new GyroAxisDetails(Pitch, 1);
break;
case Base6Directions.Direction.Forward:
AxisDetails[axis] = new GyroAxisDetails(Roll, 1);
break;
case Base6Directions.Direction.Backward:
AxisDetails[axis] = new GyroAxisDetails(Roll, -1);
break;
}
}
}
private readonly List<GyroDetails> gyros = new List<GyroDetails>();
public void Init(IEnumerable<IMyTerminalBlock> blocks,
Func<IMyGyro, bool> collect = null,
Base6Directions.Direction shipUp = Base6Directions.Direction.Up,
Base6Directions.Direction shipForward = Base6Directions.Direction.Forward)
{
gyros.Clear();
foreach (var block in blocks)
{
var gyro = block as IMyGyro;
if (gyro != null &&
gyro.IsFunctional && gyro.IsWorking && gyro.Enabled &&
(collect == null || collect(gyro)))
{
var details = new GyroDetails(gyro, shipUp, shipForward);
gyros.Add(details);
}
}
}
public void EnableOverride(bool enable)
{
gyros.ForEach(gyro => gyro.Gyro.GyroOverride = enable);
}
public void SetAxisVelocity(int axis, float velocity)
{
gyros.ForEach(gyro => gyro.Gyro.SetValue<float>(AxisNames[gyro.AxisDetails[axis].LocalAxis], gyro.AxisDetails[axis].Sign * velocity * MathHelper.RadiansPerSecondToRPM));
}
public void Reset()
{
gyros.ForEach(gyro => {
gyro.Gyro.SetValue<float>("Yaw", 0.0f);
gyro.Gyro.SetValue<float>("Pitch", 0.0f);
gyro.Gyro.SetValue<float>("Roll", 0.0f);
});
}
}

public class ShipOrientation
{
public Base6Directions.Direction ShipUp { get; private set; }
public Base6Directions.Direction ShipForward { get; private set; }
public MyBlockOrientation BlockOrientation
{
get
{
return new MyBlockOrientation(ShipForward, ShipUp);
}
}
public ShipOrientation()
{
ShipUp = Base6Directions.Direction.Up;
ShipForward = Base6Directions.Direction.Forward;
}
public void SetShipReference(IMyCubeBlock reference)
{
ShipUp = reference.Orientation.TransformDirection(Base6Directions.Direction.Up);
ShipForward = reference.Orientation.TransformDirection(Base6Directions.Direction.Forward);
}
public void SetShipReference(ZACommons commons, string groupName,
Func<IMyTerminalBlock, bool> condition = null)
{
var group = commons.GetBlockGroupWithName(groupName);
if (group != null)
{
foreach (var block in group.Blocks)
{
if (block.CubeGrid == commons.Me.CubeGrid &&
(condition == null || condition(block)))
{
SetShipReference(block);
return;
}
}
}
ShipUp = Base6Directions.Direction.Up;
ShipForward = Base6Directions.Direction.Forward;
}
public void SetShipReference<T>(IEnumerable<IMyTerminalBlock> blocks,
Func<T, bool> condition = null)
where T : IMyCubeBlock
{
var references = ZACommons.GetBlocksOfType<T>(blocks, condition);
if (references.Count > 0)
{
SetShipReference(references[0]);
}
else
{
ShipUp = Base6Directions.Direction.Up;
ShipForward = Base6Directions.Direction.Forward;
}
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
