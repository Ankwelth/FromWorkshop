const string ACTION_TAG = "MPX_HUMAN_LOGIC_ACTION";
const string LCD_DISPLAY = "MPX DISPLAY";
const string LCD_LOG = "MPX ACTIONS LOG";
const int BLOCK_REFRESH_INTERVAL = 30;
const int ACTION_COOLDOWN_TICKS = 20;
const int PRODUCE_COOLDOWN_TICKS = 600;
const int MAX_LOG_LINES = 40;
const int ACTION_TRACE_MAX = 8;
int _tick = 0;
int _received = 0;
int _decodeErrors = 0;
string _lastActions = "";
string _lastCommand = "";
List<string> _warnings = new List<string>();
List<string> _logLines = new List<string>();
Dictionary<string, int> _actionCooldowns = new Dictionary<string, int>();
Dictionary<string, int> _produceCooldowns = new Dictionary<string, int>();
Dictionary<string, int> _invCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
int _invCacheTick = -1;
Dictionary<long, bool> _tickDesiredEnabled = new Dictionary<long, bool>();
List<string> _actionTrace = new List<string>();
Dictionary<long, int> _soundStopAtTick = new Dictionary<long, int>();
Dictionary<string, int> _ruleFireCount = new Dictionary<string, int>();
Dictionary<string, double> _vars = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
bool _dryRun = false;
IMyBroadcastListener _listener;
IMyTextPanel _lcdDisplay, _lcdLog;
List<IMyReactor> _reactors = new List<IMyReactor>();
List<IMyBatteryBlock> _batteries = new List<IMyBatteryBlock>();
List<IMySolarPanel> _solar = new List<IMySolarPanel>();
List<IMyDoor> _doors = new List<IMyDoor>();
List<IMyLargeTurretBase> _turrets = new List<IMyLargeTurretBase>();
List<IMyLightingBlock> _lights = new List<IMyLightingBlock>();
List<IMySoundBlock> _soundBlocks = new List<IMySoundBlock>();
List<IMyShipConnector> _connectors = new List<IMyShipConnector>();
List<IMyGasGenerator> _gasGens = new List<IMyGasGenerator>();
List<IMyGasTank> _gasTanks = new List<IMyGasTank>();
List<IMyThrust> _thrusters = new List<IMyThrust>();
List<IMyAssembler> _assemblers = new List<IMyAssembler>();
List<IMyRefinery> _refineries = new List<IMyRefinery>();
List<IMyPistonBase> _pistons = new List<IMyPistonBase>();
List<IMyMotorStator> _rotors = new List<IMyMotorStator>();
List<IMyTimerBlock> _timers = new List<IMyTimerBlock>();
List<IMyConveyorSorter> _sorters = new List<IMyConveyorSorter>();
List<IMyRadioAntenna> _antennas = new List<IMyRadioAntenna>();
List<IMyCameraBlock> _cameras = new List<IMyCameraBlock>();
List<IMyTextPanel> _textPanels = new List<IMyTextPanel>();
List<IMyLandingGear> _gears = new List<IMyLandingGear>();
List<IMyBeacon> _beacons = new List<IMyBeacon>();
List<IMyProjector> _projectors = new List<IMyProjector>();
List<IMyJumpDrive> _jumpDrives = new List<IMyJumpDrive>();
List<IMyGyro> _gyros = new List<IMyGyro>();
List<IMyTerminalBlock> _parachutes = new List<IMyTerminalBlock>();
List<IMyMotorSuspension> _wheels = new List<IMyMotorSuspension>();
List<IMyProgrammableBlock> _otherPBs = new List<IMyProgrammableBlock>();
List<IMyCargoContainer> _cargo = new List<IMyCargoContainer>();
List<IMyAirVent> _airVents = new List<IMyAirVent>();
List<IMySensorBlock> _sensors = new List<IMySensorBlock>();
List<IMyShipController> _shipControllers = new List<IMyShipController>();
List<IMyTerminalBlock> _allBlocks = new List<IMyTerminalBlock>();
enum RuleMode { ContinuousIf, RisingEdgeWhen, FallingEdgeWhen }
enum ActType
{
TurnOn, TurnOff, SetBatteriesRecharge, SetBatteriesAuto, CloseDoors, OpenDoors, CloseHangarDoors, OpenHangarDoors, SetLightColor, SetLightIntensity, SetLightBlink, SetLightRadius, PlaySoundBlocks, StopSoundBlocks, LockConnectors, UnlockConnectors, StockpileHydrogen, StockpileOxygen, RefillTurrets, GenerateMore, MoveOresToRefinery, MoveIngotsToAssembler, MoveItemsBetween, ShootOn, ShootOff, ShootOnce, PistonExtend, PistonRetract, PistonSetVelocity, PistonSetMaxLimit, PistonSetMinLimit, RotorRotate, RotorSetVelocity, RotorSetAngle, RotorLock, RotorUnlock, TimerTrigger, TimerStart, TimerStop, SorterDrainOn, SorterDrainOff, AntennaBroadcast, WriteToLCD, Say, GearLock, GearUnlock, BeaconOn, BeaconOff, BeaconSetText, ProjectorOn, ProjectorOff, JumpDriveCharge, JumpDriveJump, LevelShip, ReleaseGyros, SetSoundVolume, ParachuteDeploy, WheelSetSpeed, WheelSetStrength, HandbrakeOn, HandbrakeOff, RunPB, VarCount, VarAdd, VarSubtract, VarSet, VarReset, VarRemember, VarForget, IGCBroadcast, CameraScanOn, CameraScanOff
}
class ActionPart
{
public ActType Action = ActType.TurnOn;
public string ActTarget = "";
public string ActParam = "";
public string ItemName = "";
public double DurationSec = 0;
public double NumParam = 0;
public string SecondTarget = "";
public string RawLower = "";
}
class ParsedRule
{
public string Raw = "";
public string Label = "";
public RuleMode Mode = RuleMode.ContinuousIf;
public double CondValue = 0;
public string ItemName = "";
}
class ActionCommand
{
public ActionPart Part;
public ParsedRule Rule;
public bool CondMet;
public bool PrevMet;
}
public Program()
{
Runtime.UpdateFrequency = UpdateFrequency.Update10;
_listener = IGC.RegisterBroadcastListener(ACTION_TAG);
_listener.SetMessageCallback("");
RefreshBlocks();
Log("MPX Script Actions listening on " + ACTION_TAG);
}
public void Main(string argument, UpdateType updateSource)
{
_tick++;
if (!string.IsNullOrWhiteSpace(argument)) { HandleArgument(argument.Trim()); }
if (_tick % BLOCK_REFRESH_INTERVAL == 1) RefreshBlocks();
var keys = new List<string>(_actionCooldowns.Keys);
foreach (var k in keys) { _actionCooldowns[k]--; if (_actionCooldowns[k] <= 0) _actionCooldowns.Remove(k); }
var pkeys = new List<string>(_produceCooldowns.Keys);
foreach (var k in pkeys) { _produceCooldowns[k]--; if (_produceCooldowns[k] <= 0) _produceCooldowns.Remove(k); }
ProcessScheduledSoundStops();
_warnings.Clear();
_tickDesiredEnabled.Clear();
DrainActionMessages();
DrawStatus();
}
void HandleArgument(string arg)
{
string up = arg.ToUpperInvariant();
if (up == "SCAN") { RefreshBlocks(); Log("Block scan complete."); return; }
if (up == "CLEAR LOG") { _logLines.Clear(); if (_lcdLog != null) _lcdLog.WriteText(""); return; }
if (up == "DRY-RUN") { _dryRun = true; Log("DRY-RUN enabled."); return; }
if (up == "LIVE") { _dryRun = false; Log("LIVE enabled."); return; }
if (up == "HELP") { Echo("MPX Script Actions listens on IGC tag " + ACTION_TAG + ". Use SCAN, STATUS, DRY-RUN, LIVE, CLEAR LOG."); return; }
if (up == "STATUS") { DrawStatus(); return; }
ActionCommand cmd;
if (TryDecodeAction(arg, out cmd)) RunCommand(cmd); else Log("Unknown arg or bad command.");
}
void RefreshBlocks()
{
_lcdDisplay = GridTerminalSystem.GetBlockWithName(LCD_DISPLAY) as IMyTextPanel;
_lcdLog = GridTerminalSystem.GetBlockWithName(LCD_LOG) as IMyTextPanel;
_reactors.Clear(); GridTerminalSystem.GetBlocksOfType(_reactors, b => b.IsSameConstructAs(Me));
_batteries.Clear(); GridTerminalSystem.GetBlocksOfType(_batteries, b => b.IsSameConstructAs(Me));
_solar.Clear(); GridTerminalSystem.GetBlocksOfType(_solar, b => b.IsSameConstructAs(Me));
_doors.Clear(); GridTerminalSystem.GetBlocksOfType(_doors, b => b.IsSameConstructAs(Me));
_turrets.Clear(); GridTerminalSystem.GetBlocksOfType(_turrets, b => b.IsSameConstructAs(Me));
_lights.Clear(); GridTerminalSystem.GetBlocksOfType(_lights, b => b.IsSameConstructAs(Me));
_soundBlocks.Clear(); GridTerminalSystem.GetBlocksOfType(_soundBlocks, b => b.IsSameConstructAs(Me));
_connectors.Clear(); GridTerminalSystem.GetBlocksOfType(_connectors, b => b.IsSameConstructAs(Me));
_gasGens.Clear(); GridTerminalSystem.GetBlocksOfType(_gasGens, b => b.IsSameConstructAs(Me));
_gasTanks.Clear(); GridTerminalSystem.GetBlocksOfType(_gasTanks, b => b.IsSameConstructAs(Me));
_thrusters.Clear(); GridTerminalSystem.GetBlocksOfType(_thrusters, b => b.IsSameConstructAs(Me));
_assemblers.Clear(); GridTerminalSystem.GetBlocksOfType(_assemblers, b => b.IsSameConstructAs(Me));
_refineries.Clear(); GridTerminalSystem.GetBlocksOfType(_refineries, b => b.IsSameConstructAs(Me));
_shipControllers.Clear(); GridTerminalSystem.GetBlocksOfType(_shipControllers, b => b.IsSameConstructAs(Me));
_pistons.Clear(); GridTerminalSystem.GetBlocksOfType(_pistons, b => b.IsSameConstructAs(Me));
_rotors.Clear(); GridTerminalSystem.GetBlocksOfType(_rotors, b => b.IsSameConstructAs(Me));
_timers.Clear(); GridTerminalSystem.GetBlocksOfType(_timers, b => b.IsSameConstructAs(Me));
_sorters.Clear(); GridTerminalSystem.GetBlocksOfType(_sorters, b => b.IsSameConstructAs(Me));
_antennas.Clear(); GridTerminalSystem.GetBlocksOfType(_antennas, b => b.IsSameConstructAs(Me));
_cameras.Clear(); GridTerminalSystem.GetBlocksOfType(_cameras, b => b.IsSameConstructAs(Me));
_textPanels.Clear(); GridTerminalSystem.GetBlocksOfType(_textPanels, b => b.IsSameConstructAs(Me));
_gears.Clear(); GridTerminalSystem.GetBlocksOfType(_gears, b => b.IsSameConstructAs(Me));
_beacons.Clear(); GridTerminalSystem.GetBlocksOfType(_beacons, b => b.IsSameConstructAs(Me));
_projectors.Clear(); GridTerminalSystem.GetBlocksOfType(_projectors, b => b.IsSameConstructAs(Me));
_jumpDrives.Clear(); GridTerminalSystem.GetBlocksOfType(_jumpDrives, b => b.IsSameConstructAs(Me));
_gyros.Clear(); GridTerminalSystem.GetBlocksOfType(_gyros, b => b.IsSameConstructAs(Me));
_parachutes.Clear(); GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_parachutes, b => b.IsSameConstructAs(Me) && IsParachuteBlock(b));
_wheels.Clear(); GridTerminalSystem.GetBlocksOfType(_wheels, b => b.IsSameConstructAs(Me));
_otherPBs.Clear(); GridTerminalSystem.GetBlocksOfType(_otherPBs, b => b.IsSameConstructAs(Me) && b != Me);
_cargo.Clear(); GridTerminalSystem.GetBlocksOfType(_cargo, b => b.IsSameConstructAs(Me));
_airVents.Clear(); GridTerminalSystem.GetBlocksOfType(_airVents, b => b.IsSameConstructAs(Me));
_sensors.Clear(); GridTerminalSystem.GetBlocksOfType(_sensors, b => b.IsSameConstructAs(Me));
_allBlocks.Clear(); GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_allBlocks, b => b.IsSameConstructAs(Me));
if (_lcdDisplay != null) SetupLCD(_lcdDisplay);
if (_lcdLog != null) SetupLCD(_lcdLog);
}
void SetupLCD(IMyTextPanel lcd)
{
lcd.ContentType = ContentType.TEXT_AND_IMAGE;
lcd.Font = "Monospace";
lcd.FontSize = 0.6f;
lcd.TextPadding = 2f;
lcd.BackgroundColor = Color.Black;
lcd.FontColor = new Color(0, 220, 255);
}
void DrainActionMessages()
{
while (_listener.HasPendingMessage)
{
var msg = _listener.AcceptMessage();
string data = msg.Data == null ? "" : msg.Data.ToString();
ActionCommand cmd;
if (TryDecodeAction(data, out cmd)) { _received++; RunCommand(cmd); }
else { _decodeErrors++; Log("Bad action message."); }
}
}
void RunCommand(ActionCommand cmd)
{
_lastCommand = cmd.Part.Action.ToString();
ExecuteActionPart(cmd.Rule, cmd.Part, cmd.CondMet, cmd.PrevMet);
}
bool TryDecodeAction(string data, out ActionCommand cmd)
{
cmd = null;
if (string.IsNullOrEmpty(data)) return false;
var f = data.Split('|');
if (f.Length < 16 || f[0] != "1") return false;
int ai, mi; double dur, num, cv;
if (!int.TryParse(f[1], out ai)) return false;
if (!double.TryParse(f[5], out dur)) dur = 0;
if (!double.TryParse(f[6], out num)) num = 0;
if (!int.TryParse(f[9], out mi)) mi = 0;
if (!double.TryParse(f[10], out cv)) cv = 0;
var p = new ActionPart();
p.Action = (ActType)ai; p.ActTarget = Dec(f[2]); p.ActParam = Dec(f[3]); p.ItemName = Dec(f[4]); p.DurationSec = dur; p.NumParam = num; p.SecondTarget = Dec(f[7]); p.RawLower = Dec(f[8]);
var r = new ParsedRule();
r.Mode = (RuleMode)mi; r.CondValue = cv; r.ItemName = Dec(f[11]); r.Label = Dec(f[12]); r.Raw = Dec(f[13]);
cmd = new ActionCommand(); cmd.Part = p; cmd.Rule = r; cmd.CondMet = f[14] == "1"; cmd.PrevMet = f[15] == "1";
return true;
}
string Dec(string s)
{
if (s == null) return "";
var sb = new StringBuilder(); bool esc = false;
foreach (char c in s)
{
if (esc) { if (c == 'p') sb.Append('|'); else if (c == 'n') sb.Append('\n'); else if (c == 'r') sb.Append('\r'); else sb.Append(c); esc = false; }
else if (c == '\\') esc = true;
else sb.Append(c);
}
if (esc) sb.Append('\\');
return sb.ToString();
}
void DrawStatus()
{
var sb = new StringBuilder();
sb.AppendLine("MPX SCRIPT ACTIONS");
sb.AppendLine("Tag: " + ACTION_TAG);
sb.AppendLine("Received: " + _received + " Decode errors: " + _decodeErrors + " DryRun: " + _dryRun);
sb.AppendLine("Last: " + _lastCommand + " / " + _lastActions);
sb.AppendLine("Blocks: " + _allBlocks.Count + " Doors " + _doors.Count + " Lights " + _lights.Count + " Turrets " + _turrets.Count + " Sounds " + _soundBlocks.Count);
if (_warnings.Count > 0) foreach (var w in _warnings) sb.AppendLine("! " + w);
if (_actionTrace.Count > 0) { sb.AppendLine("Trace:"); for (int i = _actionTrace.Count - 1; i >= 0; i--) sb.AppendLine(_actionTrace[i]); }
Echo(sb.ToString());
}
void Log(string msg)
{
string entry = "[" + _tick + "] " + msg;
_logLines.Add(entry);
while (_logLines.Count > MAX_LOG_LINES) _logLines.RemoveAt(0);
if (_lcdLog != null)
{
var sb = new StringBuilder();
for (int i = _logLines.Count - 1; i >= 0; i--) sb.AppendLine(_logLines[i]);
_lcdLog.WriteText(sb.ToString());
}
Echo(entry);
}
bool Contains(string src, string sub) { return src != null && sub != null && src.IndexOf(sub, StringComparison.OrdinalIgnoreCase) >= 0; }
double GetVar(string name) { if (string.IsNullOrEmpty(name)) return 0; double v; _vars.TryGetValue(name, out v); return v; }
void SetVar(string name, double v) { if (string.IsNullOrEmpty(name)) return; _vars[name] = v; }
static readonly Dictionary<string, string> ItemAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
{"steel plate","Component/SteelPlate"},{"steel plates","Component/SteelPlate"},
{"interior plate","Component/InteriorPlate"},{"interior plates","Component/InteriorPlate"},
{"construction","Component/Construction"},{"construction component","Component/Construction"},{"construction components","Component/Construction"},
{"computer","Component/Computer"},{"computers","Component/Computer"},{"computer component","Component/Computer"},{"computer components","Component/Computer"},
{"motor","Component/Motor"},{"motors","Component/Motor"},{"motor component","Component/Motor"},{"motor components","Component/Motor"},
{"display","Component/Display"},{"displays","Component/Display"},
{"metal grid","Component/MetalGrid"},{"metal grids","Component/MetalGrid"},
{"large steel tube","Component/LargeTube"},{"large steel tubes","Component/LargeTube"},{"large tube","Component/LargeTube"},{"large tubes","Component/LargeTube"},
{"small steel tube","Component/SmallTube"},{"small steel tubes","Component/SmallTube"},{"small tube","Component/SmallTube"},{"small tubes","Component/SmallTube"},
{"girder","Component/Girder"},{"girders","Component/Girder"},
{"bulletproof glass","Component/BulletproofGlass"},
{"reactor component","Component/Reactor"},{"reactor components","Component/Reactor"},
{"thruster component","Component/Thrust"},{"thruster components","Component/Thrust"},
{"medical component","Component/Medical"},{"medical components","Component/Medical"},
{"power cell","Component/PowerCell"},{"power cells","Component/PowerCell"},
{"solar cell","Component/SolarCell"},{"solar cells","Component/SolarCell"},
{"superconductor","Component/Superconductor"},{"superconductors","Component/Superconductor"},
{"radio","Component/RadioCommunication"},{"radio component","Component/RadioCommunication"},
{"detector","Component/Detector"},{"detector component","Component/Detector"},
{"gravity component","Component/GravityGenerator"},{"gravity generator component","Component/GravityGenerator"},
{"gatling ammo","AmmoMagazine/NATO_25x184mm"},{"gatling magazine","AmmoMagazine/NATO_25x184mm"},{"nato","AmmoMagazine/NATO_25x184mm"},
{"missile ammo","AmmoMagazine/Missile200mm"},{"missile","AmmoMagazine/Missile200mm"},{"missiles","AmmoMagazine/Missile200mm"},
{"ammo","AmmoMagazine/NATO_25x184mm"},
{"iron ore","Ore/Iron"},{"nickel ore","Ore/Nickel"},{"cobalt ore","Ore/Cobalt"},
{"magnesium ore","Ore/Magnesium"},{"silicon ore","Ore/Silicon"},{"silver ore","Ore/Silver"},
{"gold ore","Ore/Gold"},{"platinum ore","Ore/Platinum"},{"uranium ore","Ore/Uranium"},
{"stone","Ore/Stone"},{"ice","Ore/Ice"},{"scrap","Ore/Scrap"},
{"iron ingot","Ingot/Iron"},{"iron ingots","Ingot/Iron"},
{"nickel ingot","Ingot/Nickel"},{"nickel ingots","Ingot/Nickel"},
{"cobalt ingot","Ingot/Cobalt"},{"cobalt ingots","Ingot/Cobalt"},
{"magnesium powder","Ingot/Magnesium"},
{"silicon wafer","Ingot/Silicon"},
{"silver ingot","Ingot/Silver"},{"silver ingots","Ingot/Silver"},
{"gold ingot","Ingot/Gold"},{"gold ingots","Ingot/Gold"},
{"platinum ingot","Ingot/Platinum"},{"platinum ingots","Ingot/Platinum"},
{"uranium ingot","Ingot/Uranium"},{"uranium ingots","Ingot/Uranium"},
{"gravel","Ingot/Stone"},
};
static readonly Dictionary<string, string> InvSubtypeToBlueprint = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
{"SteelPlate",       "MyObjectBuilder_BlueprintDefinition/SteelPlate"},
{"InteriorPlate",    "MyObjectBuilder_BlueprintDefinition/InteriorPlate"},
{"Construction",     "MyObjectBuilder_BlueprintDefinition/ConstructionComponent"},
{"Computer",         "MyObjectBuilder_BlueprintDefinition/ComputerComponent"},
{"Motor",            "MyObjectBuilder_BlueprintDefinition/MotorComponent"},
{"Display",          "MyObjectBuilder_BlueprintDefinition/Display"},
{"MetalGrid",        "MyObjectBuilder_BlueprintDefinition/MetalGrid"},
{"LargeTube",        "MyObjectBuilder_BlueprintDefinition/LargeTube"},
{"SmallTube",        "MyObjectBuilder_BlueprintDefinition/SmallTube"},
{"Girder",           "MyObjectBuilder_BlueprintDefinition/GirderComponent"},
{"BulletproofGlass", "MyObjectBuilder_BlueprintDefinition/BulletproofGlass"},
{"Reactor",          "MyObjectBuilder_BlueprintDefinition/ReactorComponent"},
{"Thrust",           "MyObjectBuilder_BlueprintDefinition/ThrustComponent"},
{"Medical",          "MyObjectBuilder_BlueprintDefinition/MedicalComponent"},
{"PowerCell",        "MyObjectBuilder_BlueprintDefinition/PowerCell"},
{"SolarCell",        "MyObjectBuilder_BlueprintDefinition/SolarCell"},
{"Superconductor",   "MyObjectBuilder_BlueprintDefinition/Superconductor"},
{"RadioCommunication","MyObjectBuilder_BlueprintDefinition/RadioCommunicationComponent"},
{"Detector",         "MyObjectBuilder_BlueprintDefinition/DetectorComponent"},
{"GravityGenerator", "MyObjectBuilder_BlueprintDefinition/GravityGeneratorComponent"},
{"NATO_25x184mm",    "MyObjectBuilder_BlueprintDefinition/NATO_25x184mmMagazine"},
{"Missile200mm",     "MyObjectBuilder_BlueprintDefinition/Missile200mm"},
};
void ExecuteAction(ParsedRule rule, List<ActionPart> actions, bool condMet, bool prevMet)
{
foreach (var part in actions)
ExecuteActionPart(rule, part, condMet, prevMet);
}
void ExecuteActionPart(ParsedRule rule, ActionPart part, bool condMet, bool prevMet)
{
if (rule.Mode == RuleMode.ContinuousIf && part.Action == ActType.PlaySoundBlocks && part.DurationSec > 0)
{
if (!(condMet && !prevMet)) return;
}
if (_dryRun)
{
AddTrace("DRY: " + TraceTag(rule) + " ??? " + part.Action + (string.IsNullOrEmpty(part.ActTarget) ? "" : " " + part.ActTarget));
return;
}
string cooldownKey = part.Action + "|" + part.ActTarget + "|" + part.ActParam + "|" + part.ItemName;
if (_actionCooldowns.ContainsKey(cooldownKey)) return;
bool acted = false;
switch (part.Action)
{
case ActType.TurnOn:
acted = SetBlocksEnabled(part.ActTarget, part.RawLower, true, rule, part);
break;
case ActType.TurnOff:
acted = SetBlocksEnabled(part.ActTarget, part.RawLower, false, rule, part);
break;
case ActType.SetBatteriesRecharge:
foreach (var b in GetTargetBatteries(part.ActTarget))
{ b.ChargeMode = ChargeMode.Recharge; acted = true; }
break;
case ActType.SetBatteriesAuto:
foreach (var b in GetTargetBatteries(part.ActTarget))
{ b.ChargeMode = ChargeMode.Auto; acted = true; }
break;
case ActType.CloseDoors:
foreach (var d in GetTargetDoors(part.ActTarget, false))
{ d.CloseDoor(); acted = true; }
break;
case ActType.OpenDoors:
foreach (var d in GetTargetDoors(part.ActTarget, false))
{ d.OpenDoor(); acted = true; }
break;
case ActType.CloseHangarDoors:
acted = CloseHangarDoors(part.ActTarget);
break;
case ActType.OpenHangarDoors:
acted = OpenHangarDoors(part.ActTarget);
break;
case ActType.SetLightColor:
acted = SetLightColor(part.ActTarget, part.ActParam, rule, part);
break;
case ActType.PlaySoundBlocks:
foreach (var s in GetTargetSounds(part.ActTarget))
{
long sid = s.EntityId;
int pending; if (_soundStopAtTick.TryGetValue(sid, out pending) && pending > _tick) continue;
if (part.DurationSec > 0)
{
float dur = (float)Math.Min(part.DurationSec, 1000.0);
if (dur < 0.1f) dur = 0.1f;
s.LoopPeriod = dur;
s.Play();
_soundStopAtTick[sid] = _tick + (int)Math.Ceiling(dur * 6.0);
}
else
{
s.Play();
}
acted = true;
}
if (acted && rule.Mode == RuleMode.ContinuousIf && part.DurationSec > 0)
{
int holdTicks = (int)Math.Ceiling(part.DurationSec * 6.0) + 2;
_actionCooldowns[cooldownKey] = Math.Max(holdTicks, ACTION_COOLDOWN_TICKS);
_lastActions = part.Action + (string.IsNullOrEmpty(part.ActTarget) ? "" : " ??? " + part.ActTarget);
Log($"ACT: {rule.Raw.Substring(0, Math.Min(rule.Raw.Length, 60))}");
return;
}
break;
case ActType.StopSoundBlocks:
foreach (var s in GetTargetSounds(part.ActTarget))
{
s.Stop();
_soundStopAtTick.Remove(s.EntityId);
acted = true;
}
break;
case ActType.LockConnectors:
acted = LockConnectors(part.ActTarget, true);
break;
case ActType.UnlockConnectors:
acted = LockConnectors(part.ActTarget, false);
break;
case ActType.StockpileHydrogen:
foreach (var t in _gasTanks)
{ if (IsHydrogenTank(t)) { t.Stockpile = true; acted = true; } }
break;
case ActType.StockpileOxygen:
foreach (var t in _gasTanks)
{ if (IsOxygenTank(t)) { t.Stockpile = true; acted = true; } }
break;
case ActType.RefillTurrets:
acted = RefillTurrets(rule, part);
break;
case ActType.GenerateMore:
acted = QueueProduction(rule, part);
break;
case ActType.MoveOresToRefinery:
acted = MoveOresToRefinery(part);
break;
case ActType.MoveIngotsToAssembler:
acted = MoveIngotsToAssembler(part);
break;
case ActType.MoveItemsBetween:
acted = MoveItemsBetween(part);
break;
case ActType.SetLightIntensity:
acted = ApplyLightProperty(part, "intensity");
break;
case ActType.SetLightBlink:
acted = ApplyLightProperty(part, "blink");
break;
case ActType.SetLightRadius:
acted = ApplyLightProperty(part, "radius");
break;
case ActType.ShootOn:
acted = SetTurretShoot(part, true, false);
break;
case ActType.ShootOff:
acted = SetTurretShoot(part, false, false);
break;
case ActType.ShootOnce:
acted = SetTurretShoot(part, false, true);
break;
case ActType.PistonExtend:
acted = OperatePiston(part, "extend");
break;
case ActType.PistonRetract:
acted = OperatePiston(part, "retract");
break;
case ActType.PistonSetVelocity:
acted = OperatePiston(part, "velocity");
break;
case ActType.PistonSetMaxLimit:
acted = OperatePiston(part, "max");
break;
case ActType.PistonSetMinLimit:
acted = OperatePiston(part, "min");
break;
case ActType.RotorRotate:
acted = OperateRotor(part, "rotate");
break;
case ActType.RotorSetVelocity:
acted = OperateRotor(part, "velocity");
break;
case ActType.RotorSetAngle:
acted = OperateRotor(part, "angle");
break;
case ActType.RotorLock:
acted = OperateRotor(part, "lock");
break;
case ActType.RotorUnlock:
acted = OperateRotor(part, "unlock");
break;
case ActType.TimerTrigger:
acted = OperateTimer(part, "trigger");
break;
case ActType.TimerStart:
acted = OperateTimer(part, "start");
break;
case ActType.TimerStop:
acted = OperateTimer(part, "stop");
break;
case ActType.SorterDrainOn:
acted = OperateSorter(part, true);
break;
case ActType.SorterDrainOff:
acted = OperateSorter(part, false);
break;
case ActType.AntennaBroadcast:
acted = OperateAntennaBroadcast(part);
break;
case ActType.WriteToLCD:
acted = WriteToLCD(part);
break;
case ActType.Say:
if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
acted = SayMessage(part);
break;
case ActType.GearLock:        acted = OperateGears(part, true); break;
case ActType.GearUnlock:      acted = OperateGears(part, false); break;
case ActType.BeaconOn:        acted = OperateBeacon(part, true); break;
case ActType.BeaconOff:       acted = OperateBeacon(part, false); break;
case ActType.BeaconSetText:   acted = OperateBeacon(part, true); break;
case ActType.ProjectorOn:     acted = OperateProjector(part, true); break;
case ActType.ProjectorOff:    acted = OperateProjector(part, false); break;
case ActType.JumpDriveCharge: acted = OperateJumpDrive(part, false); break;
case ActType.JumpDriveJump:   acted = OperateJumpDrive(part, true); break;
case ActType.LevelShip:       acted = LevelShipToGravity(true); break;
case ActType.ReleaseGyros:    acted = LevelShipToGravity(false); break;
case ActType.SetSoundVolume:  acted = SetSoundVolume(part); break;
case ActType.ParachuteDeploy: acted = DeployParachutes(part); break;
case ActType.WheelSetSpeed:   acted = OperateWheels(part, "speed"); break;
case ActType.WheelSetStrength:acted = OperateWheels(part, "strength"); break;
case ActType.HandbrakeOn:     acted = SetHandbrake(true); break;
case ActType.HandbrakeOff:    acted = SetHandbrake(false); break;
case ActType.RunPB:           acted = RunOtherPB(part); break;
case ActType.VarCount:
if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
SetVar(part.ActParam, GetVar(part.ActParam) + 1); acted = true;
AddTrace(TraceTag(rule) + " count " + part.ActParam + "=" + GetVar(part.ActParam));
break;
case ActType.VarAdd:
if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
SetVar(part.ActParam, GetVar(part.ActParam) + part.NumParam); acted = true;
break;
case ActType.VarSubtract:
if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
SetVar(part.ActParam, GetVar(part.ActParam) - part.NumParam); acted = true;
break;
case ActType.VarSet:
SetVar(part.ActParam, part.NumParam); acted = true;
break;
case ActType.VarReset:
SetVar(part.ActParam, 0); acted = true;
break;
case ActType.VarRemember:
SetVar(part.ActParam, 1); acted = true;
break;
case ActType.VarForget:
SetVar(part.ActParam, 0); acted = true;
break;
case ActType.IGCBroadcast:
if (rule.Mode == RuleMode.ContinuousIf && !(condMet && !prevMet)) return;
acted = IGCBroadcast(part);
break;
case ActType.CameraScanOn:
foreach (var cam in ResolveTargetList(part.ActTarget, _cameras, b => (IMyCameraBlock)b))
{ if (cam != null) { cam.EnableRaycast = true; acted = true; } }
break;
case ActType.CameraScanOff:
foreach (var cam in ResolveTargetList(part.ActTarget, _cameras, b => (IMyCameraBlock)b))
{ if (cam != null) { cam.EnableRaycast = false; acted = true; } }
break;
}
if (acted)
{
_actionCooldowns[cooldownKey] = ACTION_COOLDOWN_TICKS;
_lastActions = part.Action + (string.IsNullOrEmpty(part.ActTarget) ? "" : " ??? " + part.ActTarget);
Log($"ACT: {rule.Raw.Substring(0, Math.Min(rule.Raw.Length, 60))}");
}
}
bool SetTurretShoot(ActionPart part, bool on, bool once)
{
bool acted = false;
List<IMyLargeTurretBase> targets;
if (!string.IsNullOrEmpty(part.ActTarget))
{
targets = new List<IMyLargeTurretBase>();
foreach (var b in ResolveQuoted(part.ActTarget))
{ var t = b as IMyLargeTurretBase; if (t != null) targets.Add(t); }
if (targets.Count == 0) foreach (var t in _turrets) if (NameMatch(t, part.ActTarget)) targets.Add(t);
}
else targets = _turrets;
string action = once ? "ShootOnce" : (on ? "Shoot_On" : "Shoot_Off");
foreach (var t in targets) { t.ApplyAction(action); acted = true; }
return acted;
}
bool OperatePiston(ActionPart part, string op)
{
bool acted = false;
var pistons = ResolveTargetList(part.ActTarget, _pistons, b => (IMyPistonBase)b);
foreach (var p in pistons)
{
if (p == null) continue;
switch (op)
{
case "extend": p.Extend(); acted = true; break;
case "retract": p.Retract(); acted = true; break;
case "velocity": p.Velocity = (float)part.NumParam; acted = true; break;
case "max": p.MaxLimit = (float)part.NumParam; acted = true; break;
case "min": p.MinLimit = (float)part.NumParam; acted = true; break;
}
}
return acted;
}
bool OperateRotor(ActionPart part, string op)
{
bool acted = false;
var rotors = ResolveTargetList(part.ActTarget, _rotors, b => (IMyMotorStator)b);
foreach (var r in rotors)
{
if (r == null) continue;
switch (op)
{
case "rotate":
if (part.NumParam != 0) r.TargetVelocityRPM = (float)part.NumParam;
r.RotorLock = false; acted = true; break;
case "velocity":
r.TargetVelocityRPM = (float)part.NumParam; acted = true; break;
case "angle":
r.UpperLimitDeg = (float)part.NumParam;
r.LowerLimitDeg = (float)part.NumParam;
if (r.TargetVelocityRPM == 0) r.TargetVelocityRPM = 2f;
r.RotorLock = false; acted = true; break;
case "lock": r.RotorLock = true; acted = true; break;
case "unlock": r.RotorLock = false; acted = true; break;
}
}
return acted;
}
bool OperateTimer(ActionPart part, string op)
{
bool acted = false;
var timers = ResolveTargetList(part.ActTarget, _timers, b => (IMyTimerBlock)b);
foreach (var t in timers)
{
if (t == null) continue;
switch (op)
{
case "trigger": t.Trigger(); acted = true; break;
case "start": t.StartCountdown(); acted = true; break;
case "stop": t.StopCountdown(); acted = true; break;
}
}
return acted;
}
bool OperateSorter(ActionPart part, bool drainOn)
{
bool acted = false;
var sorters = ResolveTargetList(part.ActTarget, _sorters, b => (IMyConveyorSorter)b);
foreach (var s in sorters)
{
if (s == null) continue;
s.DrainAll = drainOn; acted = true;
}
return acted;
}
bool OperateAntennaBroadcast(ActionPart part)
{
bool acted = false;
string msg = part.ActTarget;
var antennas = string.IsNullOrEmpty(part.SecondTarget)
? _antennas
: ResolveTargetList(part.SecondTarget, _antennas, b => (IMyRadioAntenna)b);
foreach (var a in antennas)
{
if (a == null) continue;
if (!string.IsNullOrEmpty(msg)) a.HudText = msg;
a.EnableBroadcasting = true;
acted = true;
}
return acted;
}
bool WriteToLCD(ActionPart part)
{
string msg = part.ActTarget;
string lcdName = part.SecondTarget;
IMyTextPanel target = null;
if (!string.IsNullOrEmpty(lcdName))
{
foreach (var p in _textPanels) if (p.CustomName.Equals(lcdName, StringComparison.OrdinalIgnoreCase)) { target = p; break; }
if (target == null) foreach (var p in _textPanels) if (NameMatch(p, lcdName)) { target = p; break; }
}
if (target == null) target = _lcdDisplay;
if (target == null) { _warnings.Add("write target LCD not found"); return false; }
target.ContentType = ContentType.TEXT_AND_IMAGE;
target.WriteText(msg);
AddTrace(TraceTag(null) + " ??? wrote to " + target.CustomName);
return true;
}
bool SayMessage(ActionPart part)
{
if (_lcdDisplay != null)
{
var sb = new StringBuilder();
sb.AppendLine("????????????????????????????????????????????????????????????????????????????????????????????????");
sb.AppendLine("??? SAY: " + part.ActTarget.PadRight(24).Substring(0, Math.Min(24, part.ActTarget.Length)) + " ???");
sb.AppendLine("????????????????????????????????????????????????????????????????????????????????????????????????");
_lcdDisplay.WriteText(sb.ToString());
}
if (_soundBlocks.Count > 0)
{
var s = _soundBlocks[0];
long sid = s.EntityId;
int pending; if (!_soundStopAtTick.TryGetValue(sid, out pending) || pending <= _tick)
{
s.LoopPeriod = 1f;
s.Play();
_soundStopAtTick[sid] = _tick + 6;
}
}
AddTrace(TraceTag(null) + " ??? SAY \"" + part.ActTarget + "\"");
return true;
}
bool MoveItemsBetween(ActionPart part)
{
string itemKey = part.ItemName;
string dstName = part.SecondTarget;
if (string.IsNullOrEmpty(itemKey) || string.IsNullOrEmpty(dstName)) { _warnings.Add("move: need item + destination"); return false; }
IMyInventory dst = null;
foreach (var b in _allBlocks)
{
if (b.CustomName.Equals(dstName, StringComparison.OrdinalIgnoreCase)) { dst = b.HasInventory ? b.GetInventory(0) : null; if (dst != null) break; }
}
if (dst == null)
{
var grp = GridTerminalSystem.GetBlockGroupWithName(dstName);
if (grp != null) { var gb = new List<IMyTerminalBlock>(); grp.GetBlocks(gb); foreach (var b in gb) if (b.HasInventory) { dst = b.GetInventory(0); break; } }
}
if (dst == null) { _warnings.Add("move: destination not found: " + dstName); return false; }
string typeShort = "Component", subId = itemKey;
int slash = itemKey.IndexOf('/');
if (slash >= 0) { typeShort = itemKey.Substring(0, slash); subId = itemKey.Substring(slash + 1); }
bool acted = false;
var items = new List<MyInventoryItem>();
foreach (var b in _allBlocks)
{
for (int i = 0; i < b.InventoryCount; i++)
{
var src = b.GetInventory(i);
if (src == dst) continue;
items.Clear();
src.GetItems(items);
for (int idx = items.Count - 1; idx >= 0; idx--)
{
var it = items[idx];
if (!it.Type.SubtypeId.Equals(subId, StringComparison.OrdinalIgnoreCase)) continue;
if (!it.Type.TypeId.ToString().Contains(typeShort)) continue;
if (!src.CanTransferItemTo(dst, it.Type)) continue;
src.TransferItemTo(dst, idx, null, true, null);
acted = true;
}
}
}
return acted;
}
bool ApplyLightProperty(ActionPart part, string prop)
{
bool acted = false;
List<IMyLightingBlock> lights;
if (!string.IsNullOrEmpty(part.ActTarget))
{
lights = new List<IMyLightingBlock>();
foreach (var b in ResolveQuoted(part.ActTarget))
{ var l = b as IMyLightingBlock; if (l != null) lights.Add(l); }
}
else lights = _lights;
foreach (var l in lights)
{
switch (prop)
{
case "intensity":
l.Intensity = (float)Math.Max(0, part.NumParam);
if (!l.Enabled) l.Enabled = true;
acted = true; break;
case "blink":
if (part.NumParam <= 0)
{ l.BlinkIntervalSeconds = 0; }
else
{
l.BlinkIntervalSeconds = (float)part.NumParam;
if (l.BlinkLength <= 0) l.BlinkLength = 50f;
}
if (!l.Enabled) l.Enabled = true;
acted = true; break;
case "radius":
l.Radius = (float)Math.Max(0.5, part.NumParam);
if (!l.Enabled) l.Enabled = true;
acted = true; break;
}
}
return acted;
}
List<T> ResolveTargetList<T>(string quoted, List<T> all, Func<IMyTerminalBlock, T> cast) where T : class
{
if (string.IsNullOrEmpty(quoted)) return all;
var result = new List<T>();
foreach (var b in ResolveQuoted(quoted))
{ var x = cast(b); if (x != null) result.Add(x); }
if (result.Count == 0)
{
foreach (var b in all)
{
var asTerm = b as IMyTerminalBlock;
if (asTerm != null && NameMatch(asTerm, quoted)) result.Add(b);
}
}
return result;
}
bool OperateGears(ActionPart part, bool lockIt)
{
bool acted = false;
var gears = ResolveTargetList(part.ActTarget, _gears, b => (IMyLandingGear)b);
foreach (var g in gears)
{
if (g == null) continue;
if (lockIt) g.Lock(); else g.Unlock();
acted = true;
}
return acted;
}
bool OperateBeacon(ActionPart part, bool turnOn)
{
bool acted = false;
var bs = ResolveTargetList(part.ActTarget, _beacons, b => (IMyBeacon)b);
foreach (var b in bs)
{
if (b == null) continue;
if (part.Action == ActType.BeaconSetText && !string.IsNullOrEmpty(part.SecondTarget))
b.CustomName = part.SecondTarget;
else if (part.Action == ActType.BeaconSetText && !string.IsNullOrEmpty(part.ActParam))
b.CustomName = part.ActParam;
if (!string.IsNullOrEmpty(part.ActTarget) && part.Action == ActType.BeaconSetText)
b.HudText = part.ActTarget;
b.Enabled = turnOn;
acted = true;
}
return acted;
}
bool OperateProjector(ActionPart part, bool turnOn)
{
bool acted = false;
var ps = ResolveTargetList(part.ActTarget, _projectors, b => (IMyProjector)b);
foreach (var p in ps) { if (p == null) continue; p.Enabled = turnOn; acted = true; }
return acted;
}
bool OperateJumpDrive(ActionPart part, bool jumpNow)
{
bool acted = false;
var jds = ResolveTargetList(part.ActTarget, _jumpDrives, b => (IMyJumpDrive)b);
foreach (var j in jds)
{
if (j == null) continue;
j.Enabled = true;
if (jumpNow) j.ApplyAction("Jump");
acted = true;
}
return acted;
}
bool LevelShipToGravity(bool engage)
{
bool acted = false;
foreach (var g in _gyros)
{
if (g == null) continue;
g.GyroOverride = engage;
if (!engage) { g.Pitch = 0; g.Yaw = 0; g.Roll = 0; }
acted = true;
}
return acted;
}
bool SetSoundVolume(ActionPart part)
{
bool acted = false;
var ss = string.IsNullOrEmpty(part.ActTarget) ? _soundBlocks : GetTargetSounds(part.ActTarget);
float vol = (float)Math.Max(0.0, Math.Min(100.0, part.NumParam)) / 100.0f;
foreach (var s in ss) { if (s == null) continue; s.Volume = vol; acted = true; }
return acted;
}
bool DeployParachutes(ActionPart part)
{
bool acted = false;
var ps = ResolveTargetList(part.ActTarget, _parachutes, b => IsParachuteBlock(b) ? b : null);
foreach (var p in ps)
{
if (p == null) continue;
p.ApplyAction("Open");
p.ApplyAction("Open_On");
acted = true;
}
return acted;
}
bool OperateWheels(ActionPart part, string what)
{
bool acted = false;
var ws = ResolveTargetList(part.ActTarget, _wheels, b => (IMyMotorSuspension)b);
foreach (var w in ws)
{
if (w == null) continue;
if (what == "speed") w.SetValueFloat("Speed Limit", (float)part.NumParam);
else if (what == "strength") w.Strength = (float)Math.Max(0, Math.Min(100, part.NumParam));
acted = true;
}
return acted;
}
bool SetHandbrake(bool on)
{
bool acted = false;
foreach (var sc in _shipControllers) { sc.HandBrake = on; acted = true; }
return acted;
}
bool RunOtherPB(ActionPart part)
{
if (string.IsNullOrEmpty(part.ActTarget)) return false;
IMyProgrammableBlock target = null;
foreach (var pb in _otherPBs)
if (pb.CustomName.Equals(part.ActTarget, StringComparison.OrdinalIgnoreCase) || NameMatch(pb, part.ActTarget))
{ target = pb; break; }
if (target == null) { _warnings.Add("PB not found: " + part.ActTarget); return false; }
target.TryRun(part.SecondTarget ?? "");
return true;
}
bool IGCBroadcast(ActionPart part)
{
if (string.IsNullOrEmpty(part.ActTarget)) return false;
string channel = string.IsNullOrEmpty(part.SecondTarget) ? "MPX" : part.SecondTarget;
IGC.SendBroadcastMessage(channel, part.ActTarget);
AddTrace(TraceTag(null) + " IGC " + channel + ": " + part.ActTarget);
return true;
}
void ProcessScheduledSoundStops()
{
if (_soundStopAtTick.Count == 0) return;
List<long> expired = null;
foreach (var kv in _soundStopAtTick)
{
if (kv.Value <= _tick)
{
if (expired == null) expired = new List<long>();
expired.Add(kv.Key);
}
}
if (expired == null) return;
foreach (var id in expired)
{
foreach (var s in _soundBlocks)
{
if (s.EntityId == id) { s.Stop(); break; }
}
_soundStopAtTick.Remove(id);
}
}
bool SetBlocksEnabled(string quotedTarget, string rawLower, bool enabled, ParsedRule rule, ActionPart part)
{
bool acted = false;
var blocks = FindTargetBlocks(quotedTarget, rawLower);
foreach (var b in blocks)
{
if (b == Me) continue;
var func = b as IMyFunctionalBlock;
if (func != null && ApplyEnabled(func, enabled, rule, part)) acted = true;
}
return acted;
}
bool ApplyEnabled(IMyFunctionalBlock func, bool enabled, ParsedRule rule, ActionPart part)
{
if (func == null || func == Me) return false;
long id = func.EntityId;
bool prevDesired;
if (_tickDesiredEnabled.TryGetValue(id, out prevDesired) && prevDesired != enabled)
{
_warnings.Add("Conflict: '" + func.CustomName + "' ON+OFF same tick");
}
_tickDesiredEnabled[id] = enabled;
if (func.Enabled == enabled) return false;
func.Enabled = enabled;
AddTrace(TraceTag(rule) + " ??? " + func.CustomName + " " + (enabled ? "ON" : "OFF"));
return true;
}
void BumpRuleFireCount(ParsedRule rule)
{
string key = !string.IsNullOrEmpty(rule.Label) ? rule.Label : (rule.Raw.Length > 36 ? rule.Raw.Substring(0, 36) : rule.Raw);
int n; _ruleFireCount.TryGetValue(key, out n);
_ruleFireCount[key] = n + 1;
}
string TraceTag(ParsedRule r)
{
if (r == null) return "?";
if (!string.IsNullOrEmpty(r.Label)) return "[" + r.Label + "]";
return TruncRule(r);
}
void AddTrace(string entry)
{
_actionTrace.Add(entry);
while (_actionTrace.Count > ACTION_TRACE_MAX) _actionTrace.RemoveAt(0);
}
string TruncRule(ParsedRule r)
{
if (r == null) return "?";
string raw = r.Raw ?? "";
return raw.Length > 36 ? raw.Substring(0, 33) + "..." : raw;
}
List<IMyTerminalBlock> FindTargetBlocks(string quotedTarget, string rawLower)
{
var result = new List<IMyTerminalBlock>();
if (!string.IsNullOrEmpty(quotedTarget))
{
var resolved = ResolveQuoted(quotedTarget);
if (resolved.Count > 0) { result.AddRange(resolved); return result; }
_warnings.Add($"Target not found: {quotedTarget}");
return result;
}
if (Contains(rawLower, "reactor")) foreach (var b in _reactors) result.Add(b);
else if (Contains(rawLower, "batter")) foreach (var b in _batteries) result.Add(b);
else if (Contains(rawLower, "turret") || Contains(rawLower, "gun"))
foreach (var b in _turrets) result.Add(b);
else if (Contains(rawLower, "thruster")) foreach (var b in _thrusters) result.Add(b);
else if (Contains(rawLower, "light")) foreach (var b in _lights) result.Add(b);
else if (Contains(rawLower, "sound") || Contains(rawLower, "alarm") || Contains(rawLower, "siren"))
foreach (var b in _soundBlocks) result.Add(b);
else if (Contains(rawLower, "h2 gen") || Contains(rawLower, "hydrogen gen") || Contains(rawLower, "gas gen") || Contains(rawLower, "oxygen gen"))
foreach (var b in _gasGens) result.Add(b);
else if (Contains(rawLower, "assembler")) foreach (var b in _assemblers) result.Add(b);
else if (Contains(rawLower, "refinery") || Contains(rawLower, "refineries")) foreach (var b in _refineries) result.Add(b);
else if (Contains(rawLower, "air vent")) foreach (var b in _airVents) result.Add(b);
else if (Contains(rawLower, "connector")) foreach (var b in _connectors) result.Add(b);
else if (Contains(rawLower, "piston")) foreach (var b in _pistons) result.Add(b);
else if (Contains(rawLower, "rotor") || Contains(rawLower, "hinge")) foreach (var b in _rotors) result.Add(b);
else if (Contains(rawLower, "timer")) foreach (var b in _timers) result.Add(b);
else if (Contains(rawLower, "sorter")) foreach (var b in _sorters) result.Add(b);
else if (Contains(rawLower, "antenna")) foreach (var b in _antennas) result.Add(b);
else if (Contains(rawLower, "camera")) foreach (var b in _cameras) result.Add(b);
else if (Contains(rawLower, "beacon")) foreach (var b in _beacons) result.Add(b);
else if (Contains(rawLower, "projector")) foreach (var b in _projectors) result.Add(b);
else if (Contains(rawLower, "jump drive") || Contains(rawLower, "jumpdrive")) foreach (var b in _jumpDrives) result.Add(b);
else if (Contains(rawLower, "gyro")) foreach (var b in _gyros) result.Add(b);
else if (Contains(rawLower, "gear") || Contains(rawLower, "landing")) foreach (var b in _gears) result.Add(b);
else if (Contains(rawLower, "parachute")) foreach (var b in _parachutes) result.Add(b);
else if (Contains(rawLower, "wheel") || Contains(rawLower, "suspension")) foreach (var b in _wheels) result.Add(b);
else if (Contains(rawLower, "solar")) foreach (var b in _solar) result.Add(b);
return result;
}
List<IMyBatteryBlock> GetTargetBatteries(string quotedTarget)
{
if (!string.IsNullOrEmpty(quotedTarget))
{
var resolved = ResolveQuoted(quotedTarget);
var r2 = new List<IMyBatteryBlock>();
foreach (var b in resolved) { var bat = b as IMyBatteryBlock; if (bat != null) r2.Add(bat); }
if (r2.Count == 0) _warnings.Add($"Target not found: {quotedTarget}");
return r2;
}
return _batteries;
}
List<IMyDoor> GetTargetDoors(string quotedTarget, bool hangarOnly)
{
var result = new List<IMyDoor>();
if (!string.IsNullOrEmpty(quotedTarget))
{
var resolved = ResolveQuoted(quotedTarget);
foreach (var b in resolved) { var d = b as IMyDoor; if (d != null) result.Add(d); }
if (result.Count == 0) _warnings.Add($"Target not found: {quotedTarget}");
return result;
}
foreach (var d in _doors)
if (!hangarOnly || IsHangarDoor(d)) result.Add(d);
return result;
}
bool CloseHangarDoors(string quotedTarget)
{
bool acted = false;
if (!string.IsNullOrEmpty(quotedTarget))
{
foreach (var b in ResolveQuoted(quotedTarget))
{
var d = b as IMyDoor;
if (d != null) { d.CloseDoor(); acted = true; }
}
if (!acted) _warnings.Add($"Target not found: {quotedTarget}");
return acted;
}
foreach (var d in _doors)
if (IsHangarDoor(d)) { d.CloseDoor(); acted = true; }
return acted;
}
bool OpenHangarDoors(string quotedTarget)
{
bool acted = false;
if (!string.IsNullOrEmpty(quotedTarget))
{
foreach (var b in ResolveQuoted(quotedTarget))
{
var d = b as IMyDoor;
if (d != null) { d.OpenDoor(); acted = true; }
}
if (!acted) _warnings.Add($"Target not found: {quotedTarget}");
return acted;
}
foreach (var d in _doors)
if (IsHangarDoor(d)) { d.OpenDoor(); acted = true; }
return acted;
}
bool SetLightColor(string quotedTarget, string colorName, ParsedRule rule, ActionPart part)
{
Color c = ColorFromName(colorName);
bool acted = false;
bool anyMatched = false;
if (!string.IsNullOrEmpty(quotedTarget))
{
foreach (var b in ResolveQuoted(quotedTarget))
{
var l = b as IMyLightingBlock;
if (l == null) continue;
anyMatched = true;
if (ApplyLight(l, colorName, c, rule)) acted = true;
}
if (!anyMatched) _warnings.Add("Target not found: " + quotedTarget);
return acted;
}
foreach (var l in _lights)
if (ApplyLight(l, colorName, c, rule)) acted = true;
return acted;
}
bool ApplyLight(IMyLightingBlock l, string colorName, Color c, ParsedRule rule)
{
if (l == null) return false;
if (colorName == "off")
{
if (!l.Enabled) return false;
l.Enabled = false;
AddTrace(TraceTag(rule) + " ??? " + l.CustomName + " LIGHT OFF");
return true;
}
bool changed = false;
if (!l.Enabled) { l.Enabled = true; changed = true; }
if (l.Color != c) { l.Color = c; changed = true; }
if (changed) AddTrace(TraceTag(rule) + " ??? " + l.CustomName + " " + colorName.ToUpper());
return changed;
}
bool LockConnectors(string quotedTarget, bool connect)
{
bool acted = false;
if (!string.IsNullOrEmpty(quotedTarget))
{
foreach (var b in ResolveQuoted(quotedTarget))
{
var c = b as IMyShipConnector;
if (c == null) continue;
if (connect) c.Connect(); else c.Disconnect();
acted = true;
}
if (!acted) _warnings.Add($"Target not found: {quotedTarget}");
return acted;
}
foreach (var c in _connectors)
{
if (connect) c.Connect(); else c.Disconnect();
acted = true;
}
return acted;
}
Color ColorFromName(string name)
{
switch (name)
{
case "red": return new Color(255, 0, 0);
case "green": return new Color(0, 255, 0);
case "blue": return new Color(0, 0, 255);
case "orange": return new Color(255, 140, 0);
case "white": return new Color(255, 255, 255);
case "yellow": return new Color(255, 255, 0);
case "cyan": return new Color(0, 255, 255);
default: return new Color(255, 255, 255);
}
}
List<IMySoundBlock> GetTargetSounds(string quotedTarget)
{
if (!string.IsNullOrEmpty(quotedTarget))
{
var resolved = ResolveQuoted(quotedTarget);
var r2 = new List<IMySoundBlock>();
foreach (var b in resolved) { var s = b as IMySoundBlock; if (s != null) r2.Add(s); }
if (r2.Count == 0) _warnings.Add($"Target not found: {quotedTarget}");
return r2;
}
return _soundBlocks;
}
string GetSubtype(IMyTerminalBlock b)
{
if (b == null) return "";
string s = null;
try
{
s = b.BlockDefinition.SubtypeId.ToString();
}
catch { s = null; }
if (string.IsNullOrEmpty(s))
{
try { s = b.DefinitionDisplayNameText; } catch { s = null; }
}
return s ?? "";
}
bool IsHangarDoor(IMyDoor d)
{
if (d == null) return false;
string sub = GetSubtype(d);
string disp = "";
try { disp = d.DefinitionDisplayNameText ?? ""; } catch { }
return sub.IndexOf("Hangar", StringComparison.OrdinalIgnoreCase) >= 0
|| disp.IndexOf("hangar", StringComparison.OrdinalIgnoreCase) >= 0
|| d.CustomName.IndexOf("hangar", StringComparison.OrdinalIgnoreCase) >= 0;
}
bool IsParachuteBlock(IMyTerminalBlock b)
{
if (b == null) return false;
string sub = GetSubtype(b);
string disp = "";
try { disp = b.DefinitionDisplayNameText ?? ""; } catch { }
return sub.IndexOf("Parachute", StringComparison.OrdinalIgnoreCase) >= 0
|| disp.IndexOf("parachute", StringComparison.OrdinalIgnoreCase) >= 0
|| b.CustomName.IndexOf("parachute", StringComparison.OrdinalIgnoreCase) >= 0;
}
List<IMyTerminalBlock> ResolveQuoted(string quoted)
{
var result = new List<IMyTerminalBlock>();
if (string.IsNullOrEmpty(quoted)) return result;
var grp = GridTerminalSystem.GetBlockGroupWithName(quoted);
if (grp != null)
{
grp.GetBlocks(result);
if (result.Count > 0) return result;
}
var blk = GridTerminalSystem.GetBlockWithName(quoted);
if (blk != null) result.Add(blk);
return result;
}
bool IsHydrogenTank(IMyGasTank t)
{
if (t == null) return false;
string sub = GetSubtype(t);
string name = t.CustomName ?? "";
return sub.IndexOf("Hydrogen", StringComparison.OrdinalIgnoreCase) >= 0
|| sub.IndexOf("H2", StringComparison.OrdinalIgnoreCase) >= 0
|| name.IndexOf("Hydrogen", StringComparison.OrdinalIgnoreCase) >= 0
|| name.IndexOf("H2", StringComparison.OrdinalIgnoreCase) >= 0;
}
bool IsOxygenTank(IMyGasTank t)
{
if (t == null) return false;
string sub = GetSubtype(t);
string name = t.CustomName ?? "";
if (sub.IndexOf("Hydrogen", StringComparison.OrdinalIgnoreCase) >= 0) return false;
return sub.IndexOf("Oxygen", StringComparison.OrdinalIgnoreCase) >= 0
|| name.IndexOf("Oxygen", StringComparison.OrdinalIgnoreCase) >= 0
|| name.IndexOf("O2", StringComparison.OrdinalIgnoreCase) >= 0;
}
bool NameMatch(IMyTerminalBlock b, string name)
=> b.CustomName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0;
IMyTerminalBlock FindBlock(string name)
{
if (string.IsNullOrEmpty(name)) return null;
foreach (var b in _allBlocks)
if (b.CustomName.Equals(name, StringComparison.OrdinalIgnoreCase)) return b;
return null;
}
void RebuildInvCache()
{
if (_invCacheTick == _tick) return;
_invCacheTick = _tick;
_invCache.Clear();
var items = new List<MyInventoryItem>();
foreach (var b in _allBlocks)
{
for (int i = 0; i < b.InventoryCount; i++)
{
items.Clear();
b.GetInventory(i).GetItems(items);
foreach (var item in items)
{
int cur;
string typeShort = item.Type.TypeId.Replace("MyObjectBuilder_", "");
string key = typeShort + "/" + item.Type.SubtypeId;
_invCache.TryGetValue(key, out cur);
_invCache[key] = cur + (int)(double)item.Amount;
}
}
}
}
int CountInventoryItem(string key)
{
RebuildInvCache();
if (string.IsNullOrEmpty(key)) return 0;
int result;
if (key.IndexOf('/') >= 0)
{
_invCache.TryGetValue(key, out result);
return result;
}
_invCache.TryGetValue("Component/" + key, out result);
return result;
}
bool QueueProduction(ParsedRule rule, ActionPart part)
{
string itemId = !string.IsNullOrEmpty(part.ItemName) ? part.ItemName : rule.ItemName;
if (string.IsNullOrEmpty(itemId))
{
string rawLower = rule.Raw.ToLowerInvariant();
foreach (var alias in ItemAliases.Keys)
if (Contains(rawLower, alias)) { itemId = ItemAliases[alias]; break; }
}
if (string.IsNullOrEmpty(itemId))
{
_warnings.Add("GenerateMore: item not identified in rule: " + rule.Raw.Substring(0, Math.Min(30, rule.Raw.Length)));
return false;
}
string typeShort = "";
string subId = itemId;
int slash = itemId.IndexOf('/');
if (slash >= 0)
{
typeShort = itemId.Substring(0, slash);
subId = itemId.Substring(slash + 1);
}
if (typeShort == "Ore" || typeShort == "Ingot")
{
_warnings.Add("Cannot generate ore/ingot at assembler: " + itemId);
return false;
}
string produceKey = "produce|" + itemId + "|" + part.ActTarget;
if (_produceCooldowns.ContainsKey(produceKey)) return false;
string bpId;
if (!InvSubtypeToBlueprint.TryGetValue(subId, out bpId))
{
_warnings.Add($"No blueprint for: {itemId}");
return false;
}
int have = CountInventoryItem(itemId);
int need = (int)rule.CondValue;
int missing = need - have;
if (missing <= 0) return false;
var targetAssemblers = string.IsNullOrEmpty(part.ActTarget)
? _assemblers
: GetGroupAssemblers(part.ActTarget);
if (targetAssemblers.Count == 0)
{
_warnings.Add("No assembler found.");
return false;
}
MyDefinitionId bp;
if (!MyDefinitionId.TryParse(bpId, out bp))
{
_warnings.Add($"Bad blueprint ID: {bpId}");
return false;
}
bool queued = false;
foreach (var a in targetAssemblers)
{
if (a.CanUseBlueprint(bp))
{
a.AddQueueItem(bp, (MyFixedPoint)missing);
queued = true;
break;
}
}
if (queued)
{
_produceCooldowns[produceKey] = PRODUCE_COOLDOWN_TICKS;
Log($"Queued {missing}x {itemId}");
}
else
_warnings.Add($"Assembler cannot make: {itemId}");
return queued;
}
List<IMyAssembler> GetGroupAssemblers(string groupName)
{
var result = new List<IMyAssembler>();
foreach (var b in ResolveQuoted(groupName))
{
var a = b as IMyAssembler;
if (a != null) result.Add(a);
}
return result;
}
bool MoveOresToRefinery(ActionPart part)
{
if (_refineries.Count == 0) { _warnings.Add("No refinery on construct."); return false; }
bool acted = false;
var items = new List<MyInventoryItem>();
foreach (var r in _refineries)
{
if (!r.IsWorking) continue;
if (!string.IsNullOrEmpty(part.ActTarget) && !NameMatch(r, part.ActTarget)) continue;
var refInv = r.InputInventory;
if (refInv == null) continue;
foreach (var cargo in _cargo)
{
var cargoInv = cargo.GetInventory();
items.Clear();
cargoInv.GetItems(items);
for (int idx = items.Count - 1; idx >= 0; idx--)
{
var item = items[idx];
if (!item.Type.TypeId.ToString().Contains("Ore")) continue;
if (!cargoInv.CanTransferItemTo(refInv, item.Type)) continue;
cargoInv.TransferItemTo(refInv, idx, null, true, null);
acted = true;
}
}
}
return acted;
}
bool MoveIngotsToAssembler(ActionPart part)
{
if (_assemblers.Count == 0) { _warnings.Add("No assembler on construct."); return false; }
bool acted = false;
var items = new List<MyInventoryItem>();
foreach (var a in _assemblers)
{
if (!a.IsWorking) continue;
if (!string.IsNullOrEmpty(part.ActTarget) && !NameMatch(a, part.ActTarget)) continue;
var asmInv = a.InputInventory;
if (asmInv == null) continue;
foreach (var r in _refineries)
{
var src = r.OutputInventory;
if (src == null) continue;
items.Clear();
src.GetItems(items);
for (int idx = items.Count - 1; idx >= 0; idx--)
{
var item = items[idx];
if (!item.Type.TypeId.ToString().Contains("Ingot")) continue;
if (!src.CanTransferItemTo(asmInv, item.Type)) continue;
src.TransferItemTo(asmInv, idx, null, true, null);
acted = true;
}
}
foreach (var cargo in _cargo)
{
var cargoInv = cargo.GetInventory();
items.Clear();
cargoInv.GetItems(items);
for (int idx = items.Count - 1; idx >= 0; idx--)
{
var item = items[idx];
if (!item.Type.TypeId.ToString().Contains("Ingot")) continue;
if (!cargoInv.CanTransferItemTo(asmInv, item.Type)) continue;
cargoInv.TransferItemTo(asmInv, idx, null, true, null);
acted = true;
}
}
}
return acted;
}
bool RefillTurrets(ParsedRule rule, ActionPart part)
{
bool acted = false;
var items = new List<MyInventoryItem>();
foreach (var turret in _turrets)
{
if (!string.IsNullOrEmpty(part.ActTarget) && !NameMatch(turret, part.ActTarget)) continue;
var turretInv = turret.GetInventory();
foreach (var cargo in _cargo)
{
var cargoInv = cargo.GetInventory();
items.Clear();
cargoInv.GetItems(items);
for (int idx = items.Count - 1; idx >= 0; idx--)
{
var item = items[idx];
if (!item.Type.TypeId.ToString().Contains("AmmoMagazine")) continue;
if (!cargoInv.CanTransferItemTo(turretInv, item.Type)) continue;
cargoInv.TransferItemTo(turretInv, idx, null, true, (MyFixedPoint)10);
acted = true;
}
}
}
return acted;
}
