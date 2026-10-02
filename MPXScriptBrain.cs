const string LCD_COMMAND="MPX COMMAND";
const string LCD_CORE="MPX CORE";
const string LCD_DISPLAY="MPX DISPLAY";
const string LCD_LOG="MPX LOG";
const string ACTION_TAG="MPX_HUMAN_LOGIC_ACTION";
const TransmissionDistance ACTION_DISTANCE=TransmissionDistance.AntennaRelay;
const string SENSOR_GROUP="MPX ENEMY SENSOR";
const string DETECT_GROUP="MPX DETECTOR";
const string DAYLIGHT_SENSOR_GROUP="MPX DAYLIGHT SENSOR";
const bool USE_DAYLIGHT_SENSOR_GROUP=false;
const double SOLAR_DARK_THRESHOLD=0.05;
const double SOLAR_LIGHT_THRESHOLD=0.20;
const int SOLAR_HYSTERESIS_TICKS=60;
const int BLOCK_REFRESH_INTERVAL=30;
const int ACTION_COOLDOWN_TICKS=20;
const int PRODUCE_COOLDOWN_TICKS=600;
const int MAX_LOG_LINES=60;
const int MAX_RULES=80;
const int ACTION_TRACE_MAX=6;
bool _paused=false;
int _tick=0;
SolarState _solarState=SolarState.Unknown;
int _solarDarkTicks=0;
int _solarLightTicks=0;
bool _solarAvailable=false;
int _solarPanelCount=0;
string _solarSource="none";
string _solarWarning="";
int _lastSolarChangeTick=0;
bool _enemyDetected=false;
string _enemySource="none";
double _powerPct=0;
double _hydrogenPct=0;
double _oxygenPct=0;
double _cargoPct=0;
double _solarPct=0;
double _solarBestPct=0;
double _solarAvgPct=0;
bool _connectorDocked=false;
int _spinnerIdx=0;
int _scanDotIdx=0;
string _lastUnderstood="";
string _lastTriggered="";
string _lastActions="";
int _sentActions=0;
string _lastDispatch="";
List<string>_warnings=new List<string>();
List<string>_logLines=new List<string>();
List<ParsedRule>_rules=new List<ParsedRule>();
Dictionary<string,int>_actionCooldowns=new Dictionary<string,int>();
Dictionary<string,int>_produceCooldowns=new Dictionary<string,int>();
Dictionary<string,int>_invCache=new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);
int _invCacheTick=-1;
string _lastCommandText=null;
Dictionary<long,bool>_tickDesiredEnabled=new Dictionary<long,bool>();
List<string>_actionTrace=new List<string>();
Dictionary<long,int>_soundStopAtTick=new Dictionary<long,int>();
HashSet<string>_mutedLabels=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
Dictionary<string,int>_simulated=new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);
string _pendingArgument="";
string _activeMode="";
Dictionary<string,double>_vars=new Dictionary<string,double>(StringComparer.OrdinalIgnoreCase);
string _varsSnapshot="";
Dictionary<string,IMyBroadcastListener>_igcListeners=new Dictionary<string,IMyBroadcastListener>(StringComparer.OrdinalIgnoreCase);
Dictionary<string,string>_igcReceived=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
Dictionary<string,int>_ruleFireCount=new Dictionary<string,int>();
double _gameHour=12.0;
bool _inGravity=false;
bool _dryRun=false;
IMyTextPanel _lcdCommand,_lcdCore,_lcdDisplay,_lcdLog;
List<IMyReactor>_reactors=new List<IMyReactor>();
List<IMyBatteryBlock>_batteries=new List<IMyBatteryBlock>();
List<IMySolarPanel>_solar=new List<IMySolarPanel>();
List<IMySolarPanel>_solarSensorPanels=new List<IMySolarPanel>();
List<IMyDoor>_doors=new List<IMyDoor>();
List<IMyLargeTurretBase>_turrets=new List<IMyLargeTurretBase>();
List<IMyLightingBlock>_lights=new List<IMyLightingBlock>();
List<IMySoundBlock>_soundBlocks=new List<IMySoundBlock>();
List<IMyShipConnector>_connectors=new List<IMyShipConnector>();
List<IMyGasGenerator>_gasGens=new List<IMyGasGenerator>();
List<IMyGasTank>_gasTanks=new List<IMyGasTank>();
List<IMyThrust>_thrusters=new List<IMyThrust>();
List<IMyAssembler>_assemblers=new List<IMyAssembler>();
List<IMyRefinery>_refineries=new List<IMyRefinery>();
List<IMyPistonBase>_pistons=new List<IMyPistonBase>();
List<IMyMotorStator>_rotors=new List<IMyMotorStator>();
List<IMyTimerBlock>_timers=new List<IMyTimerBlock>();
List<IMyConveyorSorter>_sorters=new List<IMyConveyorSorter>();
List<IMyRadioAntenna>_antennas=new List<IMyRadioAntenna>();
List<IMyCameraBlock>_cameras=new List<IMyCameraBlock>();
List<IMyTextPanel>_textPanels=new List<IMyTextPanel>();
List<IMyUserControllableGun>_allGuns=new List<IMyUserControllableGun>();
List<IMyLandingGear>_gears=new List<IMyLandingGear>();
List<IMyBeacon>_beacons=new List<IMyBeacon>();
List<IMyProjector>_projectors=new List<IMyProjector>();
List<IMyJumpDrive>_jumpDrives=new List<IMyJumpDrive>();
List<IMyGyro>_gyros=new List<IMyGyro>();
List<IMyParachute>_parachutes=new List<IMyParachute>();
List<IMyMotorSuspension>_wheels=new List<IMyMotorSuspension>();
List<IMyProgrammableBlock>_otherPBs=new List<IMyProgrammableBlock>();
List<IMyCargoContainer>_cargo=new List<IMyCargoContainer>();
List<IMyAirVent>_airVents=new List<IMyAirVent>();
List<IMySensorBlock>_sensors=new List<IMySensorBlock>();
List<IMyTerminalBlock>_allBlocks=new List<IMyTerminalBlock>();
List<IMyShipController>_shipControllers=new List<IMyShipController>();
double _shipSpeed=0;
double _shipAltitude=0;
bool _pilotPresent=false;
enum SolarState{
Unknown,Daylight,Dark}
enum RuleMode{
ContinuousIf,RisingEdgeWhen,FallingEdgeWhen}
enum CondJoin{
None,And,Or}
enum CondType{
EnemyDetected,EnemyNotDetected,PowerBelow,PowerAbove,PowerFull,BatteryBelow,NamedBatteryBelow,NamedBatteryAbove,Dark,Daylight,SolarBelow,SolarAbove,ConnectorDocked,ConnectorUndocked,NamedConnectorDocked,NamedConnectorUndocked,OxygenBelow,OxygenLow,HydrogenBelow,HydrogenLow,CargoAbove,CargoFull,NamedCargoAbove,ItemBelow,BlockIsOff,BlockIsOn,BlockDamaged,BlockNotDamaged,AirVentOff,AirVentOn,RefineryIdle,RefineryWorking,AssemblerIdle,AssemblerWorking,SpeedAbove,SpeedBelow,AltitudeAbove,AltitudeBelow,PilotPresent,PilotAbsent,GunShooting,GunNotShooting,ArgumentMatches,GameHourIs,GameHourBefore,GameHourAfter,NightTime,DayTime,BlockCountBelow,BlockCountAbove,BlockCountIs,GunAmmoBelow,InGravity,InSpace,GyroOverriding,SensorTriggered,VarAbove,VarBelow,VarIs,Remembered,Forgotten,IGCReceived,CameraSees,CameraSeesEnemy,Always}
enum ActType{
TurnOn,TurnOff,SetBatteriesRecharge,SetBatteriesAuto,CloseDoors,OpenDoors,CloseHangarDoors,OpenHangarDoors,SetLightColor,SetLightIntensity,SetLightBlink,SetLightRadius,PlaySoundBlocks,StopSoundBlocks,LockConnectors,UnlockConnectors,StockpileHydrogen,StockpileOxygen,RefillTurrets,GenerateMore,MoveOresToRefinery,MoveIngotsToAssembler,MoveItemsBetween,ShootOn,ShootOff,ShootOnce,PistonExtend,PistonRetract,PistonSetVelocity,PistonSetMaxLimit,PistonSetMinLimit,RotorRotate,RotorSetVelocity,RotorSetAngle,RotorLock,RotorUnlock,TimerTrigger,TimerStart,TimerStop,SorterDrainOn,SorterDrainOff,AntennaBroadcast,WriteToLCD,Say,GearLock,GearUnlock,BeaconOn,BeaconOff,BeaconSetText,ProjectorOn,ProjectorOff,JumpDriveCharge,JumpDriveJump,LevelShip,ReleaseGyros,SetSoundVolume,ParachuteDeploy,WheelSetSpeed,WheelSetStrength,HandbrakeOn,HandbrakeOff,RunPB,VarCount,VarAdd,VarSubtract,VarSet,VarReset,VarRemember,VarForget,IGCBroadcast,CameraScanOn,CameraScanOff}
class ActionPart{
public ActType Action=ActType.TurnOn;
public string ActTarget="";
public string ActParam="";
public string ItemName="";
public double DurationSec=0;
public double NumParam=0;
public string SecondTarget="";
public string RawLower="";
}
class ParsedRule{
public string Raw="";
public string Label="";
public List<string>ModeTags=new List<string>();
public int Priority=0;
public RuleMode Mode=RuleMode.ContinuousIf;
public CondType Condition=CondType.Always;
public double CondValue=0;
public string CondTarget="";
public string ItemName="";
public CondJoin Join=CondJoin.None;
public CondType Cond2=CondType.Always;
public double Cond2Value=0;
public string Cond2Target="";
public string Cond2Item="";
public List<ActionPart>Actions=new List<ActionPart>();
public List<ActionPart>OtherwiseActions=new List<ActionPart>();
public int EveryNTicks=0;
public int LastFireTick=-100000;
public bool PrevCondMet=false;
public bool Valid=false;
public string ParseError="";
}
static readonly Dictionary<string,string>ItemAliases=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase){
{
"steel plate","Component/SteelPlate"}
,{
"steel plates","Component/SteelPlate"}
,{
"interior plate","Component/InteriorPlate"}
,{
"interior plates","Component/InteriorPlate"}
,{
"construction","Component/Construction"}
,{
"construction component","Component/Construction"}
,{
"construction components","Component/Construction"}
,{
"computer","Component/Computer"}
,{
"computers","Component/Computer"}
,{
"computer component","Component/Computer"}
,{
"computer components","Component/Computer"}
,{
"motor","Component/Motor"}
,{
"motors","Component/Motor"}
,{
"motor component","Component/Motor"}
,{
"motor components","Component/Motor"}
,{
"display","Component/Display"}
,{
"displays","Component/Display"}
,{
"metal grid","Component/MetalGrid"}
,{
"metal grids","Component/MetalGrid"}
,{
"large steel tube","Component/LargeTube"}
,{
"large steel tubes","Component/LargeTube"}
,{
"large tube","Component/LargeTube"}
,{
"large tubes","Component/LargeTube"}
,{
"small steel tube","Component/SmallTube"}
,{
"small steel tubes","Component/SmallTube"}
,{
"small tube","Component/SmallTube"}
,{
"small tubes","Component/SmallTube"}
,{
"girder","Component/Girder"}
,{
"girders","Component/Girder"}
,{
"bulletproof glass","Component/BulletproofGlass"}
,{
"reactor component","Component/Reactor"}
,{
"reactor components","Component/Reactor"}
,{
"thruster component","Component/Thrust"}
,{
"thruster components","Component/Thrust"}
,{
"medical component","Component/Medical"}
,{
"medical components","Component/Medical"}
,{
"power cell","Component/PowerCell"}
,{
"power cells","Component/PowerCell"}
,{
"solar cell","Component/SolarCell"}
,{
"solar cells","Component/SolarCell"}
,{
"superconductor","Component/Superconductor"}
,{
"superconductors","Component/Superconductor"}
,{
"radio","Component/RadioCommunication"}
,{
"radio component","Component/RadioCommunication"}
,{
"detector","Component/Detector"}
,{
"detector component","Component/Detector"}
,{
"gravity component","Component/GravityGenerator"}
,{
"gravity generator component","Component/GravityGenerator"}
,{
"gatling ammo","AmmoMagazine/NATO_25x184mm"}
,{
"gatling magazine","AmmoMagazine/NATO_25x184mm"}
,{
"nato","AmmoMagazine/NATO_25x184mm"}
,{
"missile ammo","AmmoMagazine/Missile200mm"}
,{
"missile","AmmoMagazine/Missile200mm"}
,{
"missiles","AmmoMagazine/Missile200mm"}
,{
"ammo","AmmoMagazine/NATO_25x184mm"}
,{
"iron ore","Ore/Iron"}
,{
"nickel ore","Ore/Nickel"}
,{
"cobalt ore","Ore/Cobalt"}
,{
"magnesium ore","Ore/Magnesium"}
,{
"silicon ore","Ore/Silicon"}
,{
"silver ore","Ore/Silver"}
,{
"gold ore","Ore/Gold"}
,{
"platinum ore","Ore/Platinum"}
,{
"uranium ore","Ore/Uranium"}
,{
"stone","Ore/Stone"}
,{
"ice","Ore/Ice"}
,{
"scrap","Ore/Scrap"}
,{
"iron ingot","Ingot/Iron"}
,{
"iron ingots","Ingot/Iron"}
,{
"nickel ingot","Ingot/Nickel"}
,{
"nickel ingots","Ingot/Nickel"}
,{
"cobalt ingot","Ingot/Cobalt"}
,{
"cobalt ingots","Ingot/Cobalt"}
,{
"magnesium powder","Ingot/Magnesium"}
,{
"silicon wafer","Ingot/Silicon"}
,{
"silver ingot","Ingot/Silver"}
,{
"silver ingots","Ingot/Silver"}
,{
"gold ingot","Ingot/Gold"}
,{
"gold ingots","Ingot/Gold"}
,{
"platinum ingot","Ingot/Platinum"}
,{
"platinum ingots","Ingot/Platinum"}
,{
"uranium ingot","Ingot/Uranium"}
,{
"uranium ingots","Ingot/Uranium"}
,{
"gravel","Ingot/Stone"}
,}
;
public Program(){
Runtime.UpdateFrequency=UpdateFrequency.Update10;
LoadVars();
RefreshBlocks();
ParseRules();
Log("MPX Script Brain started. Action tag "+ACTION_TAG);
}
public void Main(string argument,UpdateType updateSource){
_tick++;
if(!string.IsNullOrEmpty(argument)){
string up=argument.Trim().ToUpperInvariant();
bool isPassthroughArg=up.StartsWith("RUN ")||up.StartsWith("TRIGGER ");
HandleArgument(up,argument.Trim());
if(!isPassthroughArg)return;
}
if(_simulated.Count>0){
var dead=new List<string>();
foreach(var kv in _simulated)if(kv.Value<=_tick)dead.Add(kv.Key);
foreach(var k in dead)_simulated.Remove(k);
}
if(_paused){
DrawCoreLCD();
return;
}
if(_tick%BLOCK_REFRESH_INTERVAL==1)RefreshBlocks();
CheckCommandReload();
if(_tick%300==2)ParseRules();
if(_tick%4==0)_spinnerIdx=(_spinnerIdx+1)%4;
if(_tick%6==0)_scanDotIdx=(_scanDotIdx+1)%3;
var keys=new List<string>(_actionCooldowns.Keys);
foreach(var k in keys){
_actionCooldowns[k]--;
if(_actionCooldowns[k]<=0)_actionCooldowns.Remove(k);
}
var pkeys=new List<string>(_produceCooldowns.Keys);
foreach(var k in pkeys){
_produceCooldowns[k]--;
if(_produceCooldowns[k]<=0)_produceCooldowns.Remove(k);
}
GatherTelemetry();
DrainIGCMessages();
_warnings.Clear();
_tickDesiredEnabled.Clear();
EvaluateRules();
SaveVarsIfDirty();
_pendingArgument="";
DrawCoreLCD();
DrawDisplayLCD();
}
void HandleArgument(string arg,string argOriginal){
if(arg.StartsWith("MUTE ")){
_mutedLabels.Add(argOriginal.Substring(5).Trim());
Log("MUTE: "+argOriginal.Substring(5).Trim());
return;
}
if(arg.StartsWith("UNMUTE ")){
_mutedLabels.Remove(argOriginal.Substring(7).Trim());
Log("UNMUTE: "+argOriginal.Substring(7).Trim());
return;
}
if(arg.StartsWith("MODE ")){
_activeMode=argOriginal.Substring(5).Trim().ToLowerInvariant();
Log("MODE: "+_activeMode);
return;
}
if(arg.StartsWith("SIMULATE ")){
string what=arg.Substring(9).Trim().ToLowerInvariant();
if(what=="clear"||what=="off"){
_simulated.Clear();
Log("SIMULATE cleared.");
return;
}
_simulated[what]=_tick+60;
Log("SIMULATE: "+what+" (10s)");
return;
}
if(arg.StartsWith("RUN ")||arg.StartsWith("TRIGGER ")){
int sp=argOriginal.IndexOf(' ');
_pendingArgument=argOriginal.Substring(sp+1).Trim();
Log("ARG: "+_pendingArgument);
return;
}
if(arg.StartsWith("TUTORIAL ")){
string flavor=argOriginal.Substring(9).Trim().ToLowerInvariant();
WriteTutorialToCommand(flavor);
return;
}
switch(arg){
case"HELP":string help=BuildHelpText();
Echo(help);
if(_lcdCore!=null){
_lcdCore.WriteText(help);
}
break;
case"RELOAD":RefreshBlocks();
ParseRules();
Log("Manual reload.");
break;
case"TUTORIAL":WriteTutorialToCommand("");
break;
case"STATS":EchoRuleStats();
break;
case"DIAGNOSE":EchoDiagnose();
break;
case"DRY-RUN":case"DRYRUN":_dryRun=true;
Log("DRY-RUN: rules will evaluate but no blocks will be touched.");
break;
case"LIVE":_dryRun=false;
Log("LIVE: actions will fire normally.");
break;
case"SCAN":RefreshBlocks();
Log("Block scan complete.");
break;
case"STATUS":Echo($"Rules: {_rules.Count} | Paused: {_paused} | Enemy: {_enemyDetected} | Solar: {_solarState}");
break;
case"CLEARLOG":_logLines.Clear();
if(_lcdLog!=null)_lcdLog.WriteText("");
Log("Log cleared.");
break;
case"PAUSE":_paused=true;
Log("System PAUSED.");
break;
case"RESUME":_paused=false;
Log("System RESUMED.");
break;
default:Echo($"Unknown argument: {arg}. Try HELP.");
break;
}
}
string BuildHelpText(){
return"MPX SCRIPT BRAIN\nRules go on MPX COMMAND. Examples:\nif enemy detected turn turrets on\nif power below 25 set batteries recharge otherwise set batteries auto\nCommands: RELOAD DIAGNOSE STATS DRY-RUN LIVE PAUSE RESUME SCAN TUTORIAL\nAction IGC tag: "+ACTION_TAG+"\nSeparate grids need powered enabled antennas in range and Actions PB running.";
}
void WriteTutorialToCommand(string flavor){
if(_lcdCommand==null){
Echo("MPX COMMAND LCD not found.");
return;
}
if((_lcdCommand.GetText()??"").Trim().Length>0){
Echo("MPX COMMAND already has rules; clear it first.");
return;
}
_lcdCommand.WriteText("# MPX split starter rules\nif enemy detected turn turrets on and play \"Alarm\" for 5 seconds\nwhen enemy gone turn turrets off and stop \"Alarm\"\nif power below 25 set batteries recharge otherwise set batteries auto\nif dark turn lights red otherwise turn lights off\n");
ParseRules();
Log("TUTORIAL written.");
}
void EchoRuleStats(){
var sb=new StringBuilder();
sb.AppendLine("Rules: "+_rules.Count+" Sent: "+_sentActions);
var list=new List<KeyValuePair<string,int>>(_ruleFireCount);
list.Sort((a,b)=>b.Value.CompareTo(a.Value));
foreach(var kv in list)sb.AppendLine(kv.Value+"x "+kv.Key);
if(list.Count==0)sb.AppendLine("no rules fired");
Echo(sb.ToString());
if(_lcdLog!=null)_lcdLog.WriteText(sb.ToString());
}
void EchoDiagnose(){
var sb=new StringBuilder();
int valid=0,bad=0;
foreach(var r in _rules){
if(r.Valid)valid++;
else bad++;
}
sb.AppendLine("Valid rules: "+valid+" Errors: "+bad+" Sent: "+_sentActions);
sb.AppendLine("Blocks "+_allBlocks.Count+" Batteries "+_batteries.Count+" Turrets "+_turrets.Count+" Lights "+_lights.Count);
sb.AppendLine("PWR "+_powerPct.ToString("0")+"% H2 "+_hydrogenPct.ToString("0")+"% O2 "+_oxygenPct.ToString("0")+"% Cargo "+_cargoPct.ToString("0")+"%");
foreach(var r in _rules)if(!r.Valid)sb.AppendLine("ERR "+TruncRule(r)+": "+r.ParseError);
Echo(sb.ToString());
if(_lcdLog!=null)_lcdLog.WriteText(sb.ToString());
}
bool UseDaylightSensorGroup(){
return USE_DAYLIGHT_SENSOR_GROUP;
}
void RefreshBlocks(){
_lcdCommand=GridTerminalSystem.GetBlockWithName(LCD_COMMAND)as IMyTextPanel;
_lcdCore=GridTerminalSystem.GetBlockWithName(LCD_CORE)as IMyTextPanel;
_lcdDisplay=GridTerminalSystem.GetBlockWithName(LCD_DISPLAY)as IMyTextPanel;
_lcdLog=GridTerminalSystem.GetBlockWithName(LCD_LOG)as IMyTextPanel;
_reactors.Clear();
GridTerminalSystem.GetBlocksOfType(_reactors,b=>b.IsSameConstructAs(Me));
_batteries.Clear();
GridTerminalSystem.GetBlocksOfType(_batteries,b=>b.IsSameConstructAs(Me));
_solar.Clear();
GridTerminalSystem.GetBlocksOfType(_solar,b=>b.IsSameConstructAs(Me));
_doors.Clear();
GridTerminalSystem.GetBlocksOfType(_doors,b=>b.IsSameConstructAs(Me));
_turrets.Clear();
GridTerminalSystem.GetBlocksOfType(_turrets,b=>b.IsSameConstructAs(Me));
_lights.Clear();
GridTerminalSystem.GetBlocksOfType(_lights,b=>b.IsSameConstructAs(Me));
_soundBlocks.Clear();
GridTerminalSystem.GetBlocksOfType(_soundBlocks,b=>b.IsSameConstructAs(Me));
_connectors.Clear();
GridTerminalSystem.GetBlocksOfType(_connectors,b=>b.IsSameConstructAs(Me));
_gasGens.Clear();
GridTerminalSystem.GetBlocksOfType(_gasGens,b=>b.IsSameConstructAs(Me));
_gasTanks.Clear();
GridTerminalSystem.GetBlocksOfType(_gasTanks,b=>b.IsSameConstructAs(Me));
_thrusters.Clear();
GridTerminalSystem.GetBlocksOfType(_thrusters,b=>b.IsSameConstructAs(Me));
_assemblers.Clear();
GridTerminalSystem.GetBlocksOfType(_assemblers,b=>b.IsSameConstructAs(Me));
_refineries.Clear();
GridTerminalSystem.GetBlocksOfType(_refineries,b=>b.IsSameConstructAs(Me));
_shipControllers.Clear();
GridTerminalSystem.GetBlocksOfType(_shipControllers,b=>b.IsSameConstructAs(Me));
_pistons.Clear();
GridTerminalSystem.GetBlocksOfType(_pistons,b=>b.IsSameConstructAs(Me));
_rotors.Clear();
GridTerminalSystem.GetBlocksOfType(_rotors,b=>b.IsSameConstructAs(Me));
_timers.Clear();
GridTerminalSystem.GetBlocksOfType(_timers,b=>b.IsSameConstructAs(Me));
_sorters.Clear();
GridTerminalSystem.GetBlocksOfType(_sorters,b=>b.IsSameConstructAs(Me));
_antennas.Clear();
GridTerminalSystem.GetBlocksOfType(_antennas,b=>b.IsSameConstructAs(Me));
_cameras.Clear();
GridTerminalSystem.GetBlocksOfType(_cameras,b=>b.IsSameConstructAs(Me));
_textPanels.Clear();
GridTerminalSystem.GetBlocksOfType(_textPanels,b=>b.IsSameConstructAs(Me));
_allGuns.Clear();
GridTerminalSystem.GetBlocksOfType(_allGuns,b=>b.IsSameConstructAs(Me));
_gears.Clear();
GridTerminalSystem.GetBlocksOfType(_gears,b=>b.IsSameConstructAs(Me));
_beacons.Clear();
GridTerminalSystem.GetBlocksOfType(_beacons,b=>b.IsSameConstructAs(Me));
_projectors.Clear();
GridTerminalSystem.GetBlocksOfType(_projectors,b=>b.IsSameConstructAs(Me));
_jumpDrives.Clear();
GridTerminalSystem.GetBlocksOfType(_jumpDrives,b=>b.IsSameConstructAs(Me));
_gyros.Clear();
GridTerminalSystem.GetBlocksOfType(_gyros,b=>b.IsSameConstructAs(Me));
_parachutes.Clear();
GridTerminalSystem.GetBlocksOfType(_parachutes,b=>b.IsSameConstructAs(Me));
_wheels.Clear();
GridTerminalSystem.GetBlocksOfType(_wheels,b=>b.IsSameConstructAs(Me));
_otherPBs.Clear();
GridTerminalSystem.GetBlocksOfType(_otherPBs,b=>b.IsSameConstructAs(Me)&&b!=Me);
_cargo.Clear();
GridTerminalSystem.GetBlocksOfType(_cargo,b=>b.IsSameConstructAs(Me));
_airVents.Clear();
GridTerminalSystem.GetBlocksOfType(_airVents,b=>b.IsSameConstructAs(Me));
_sensors.Clear();
GridTerminalSystem.GetBlocksOfType(_sensors,b=>b.IsSameConstructAs(Me));
_allBlocks.Clear();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_allBlocks,b=>b.IsSameConstructAs(Me));
_solarSensorPanels.Clear();
if(UseDaylightSensorGroup()){
var dlGrp=GridTerminalSystem.GetBlockGroupWithName(DAYLIGHT_SENSOR_GROUP);
if(dlGrp!=null){
var tmp=new List<IMyTerminalBlock>();
dlGrp.GetBlocks(tmp);
foreach(var b in tmp){
var sp=b as IMySolarPanel;
if(sp!=null)_solarSensorPanels.Add(sp);
}
}
}
if(_lcdCore!=null)SetupLCD(_lcdCore);
if(_lcdDisplay!=null)SetupLCD(_lcdDisplay);
if(_lcdLog!=null)SetupLCD(_lcdLog);
}
void SetupLCD(IMyTextPanel lcd){
lcd.ContentType=ContentType.TEXT_AND_IMAGE;
lcd.Font="Monospace";
lcd.FontSize=0.6f;
lcd.TextPadding=2f;
lcd.BackgroundColor=Color.Black;
lcd.FontColor=new Color(0,220,255);
}
void GatherTelemetry(){
double stored=0,maxStored=0;
foreach(var b in _batteries){
stored+=b.CurrentStoredPower;
maxStored+=b.MaxStoredPower;
}
_powerPct=maxStored>0?stored/maxStored*100.0:0;
double hStored=0,hMax=0;
foreach(var t in _gasTanks){
if(IsHydrogenTank(t)){
hStored+=t.FilledRatio;
hMax+=1.0;
}
}
_hydrogenPct=hMax>0?hStored/hMax*100.0:0;
double oStored=0,oMax=0;
foreach(var t in _gasTanks){
if(IsOxygenTank(t)){
oStored+=t.FilledRatio;
oMax+=1.0;
}
}
_oxygenPct=oMax>0?oStored/oMax*100.0:0;
double cFill=0,cMax=0;
foreach(var c in _cargo){
cFill+=(double)c.GetInventory().CurrentVolume;
cMax+=(double)c.GetInventory().MaxVolume;
}
_cargoPct=cMax>0?cFill/cMax*100.0:0;
List<IMySolarPanel>solarSrc;
if(UseDaylightSensorGroup()&&_solarSensorPanels.Count>0){
solarSrc=_solarSensorPanels;
_solarSource="MPX DAYLIGHT SENSOR";
}
else if(_solar.Count>0){
solarSrc=_solar;
_solarSource="all solar panels";
}
else{
solarSrc=null;
_solarSource="none";
}
_solarPanelCount=solarSrc!=null?solarSrc.Count:0;
double bestPct=0,sumPct=0;
int usableCount=0;
if(solarSrc!=null){
foreach(var s in solarSrc){
if(!s.IsWorking)continue;
double defMax=PanelDefMaxMW(s);
if(defMax<=0)continue;
double pct=(s.MaxOutput/defMax)*100.0;
if(pct>100.0)pct=100.0;
if(pct>bestPct)bestPct=pct;
sumPct+=pct;
usableCount++;
}
}
_solarBestPct=bestPct;
_solarAvgPct=usableCount>0?sumPct/usableCount:0;
_solarPct=_solarBestPct;
_solarAvailable=usableCount>0;
if(!_solarAvailable){
_solarState=SolarState.Unknown;
_solarDarkTicks=0;
_solarLightTicks=0;
_solarWarning=(solarSrc==null||solarSrc.Count==0)?"No solar panels found ??? dark/daylight rules disabled.":"Solar panels offline/damaged ??? dark/daylight rules disabled.";
}
else{
_solarWarning="";
if(_solarBestPct<SOLAR_DARK_THRESHOLD*100.0){
_solarDarkTicks++;
_solarLightTicks=0;
if(_solarDarkTicks>=SOLAR_HYSTERESIS_TICKS&&_solarState!=SolarState.Dark){
_solarState=SolarState.Dark;
_lastSolarChangeTick=_tick;
}
}
else if(_solarBestPct>SOLAR_LIGHT_THRESHOLD*100.0){
_solarLightTicks++;
_solarDarkTicks=0;
if(_solarLightTicks>=SOLAR_HYSTERESIS_TICKS&&_solarState!=SolarState.Daylight){
_solarState=SolarState.Daylight;
_lastSolarChangeTick=_tick;
}
}
else{
if(_solarDarkTicks>0)_solarDarkTicks--;
if(_solarLightTicks>0)_solarLightTicks--;
}
}
_connectorDocked=false;
foreach(var c in _connectors)if(c.Status==MyShipConnectorStatus.Connected){
_connectorDocked=true;
break;
}
_shipSpeed=0;
_shipAltitude=0;
_pilotPresent=false;
IMyShipController best=null;
foreach(var sc in _shipControllers){
if(sc.IsUnderControl){
_pilotPresent=true;
best=sc;
break;
}
if(best==null&&sc.IsMainCockpit)best=sc;
}
if(best==null&&_shipControllers.Count>0)best=_shipControllers[0];
if(best!=null){
var v=best.GetShipVelocities().LinearVelocity;
_shipSpeed=v.Length();
double elev;
if(best.TryGetPlanetElevation(MyPlanetElevation.Surface,out elev))_shipAltitude=elev;
var grav=best.GetNaturalGravity();
_inGravity=grav.LengthSquared()>0.01;
}
else{
_inGravity=false;
}
if(_solarAvailable&&_solarBestPct>0){
_gameHour=6.0+(_solarBestPct/100.0)*12.0;
if(_solarState==SolarState.Dark)_gameHour=(_tick/600.0)%6.0;
}
DetectEnemy();
}
double PanelDefMaxMW(IMySolarPanel panel){
if(panel==null)return 0;
return panel.CubeGrid.GridSizeEnum==MyCubeSize.Large?0.16:0.04;
}
void DetectEnemy(){
_enemyDetected=false;
_enemySource="none";
var sensorGroup=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlockGroupWithName(SENSOR_GROUP)?.GetBlocks(sensorGroup);
GridTerminalSystem.GetBlockGroupWithName(DETECT_GROUP)?.GetBlocks(sensorGroup);
foreach(var b in sensorGroup){
var s=b as IMySensorBlock;
if(s!=null&&s.IsActive){
_enemyDetected=true;
_enemySource="Sensor:"+s.CustomName;
return;
}
}
foreach(var s in _sensors)if(s.IsActive){
_enemyDetected=true;
_enemySource="Sensor:"+s.CustomName;
return;
}
foreach(var t in _turrets){
if(t.HasTarget){
_enemyDetected=true;
_enemySource="Turret:"+t.CustomName;
return;
}
}
}
string ReadCommandText(){
if(_lcdCommand!=null){
string t=_lcdCommand.GetText();
return t??"";
}
return Me.CustomData??"";
}
void CheckCommandReload(){
string current=ReadCommandText();
if(current==_lastCommandText)return;
_lastCommandText=current;
ParseRules();
Log("MPX COMMAND changed ??? rules reloaded ("+_rules.Count+")");
}
void ParseRules(){
_rules.Clear();
string rawText=ReadCommandText();
_lastCommandText=rawText;
if(string.IsNullOrWhiteSpace(rawText)){
_lastUnderstood="No rules found.";
return;
}
rawText=StripBlockComments(rawText);
var lines=rawText.Split(new char[]{
'\n','\r'}
,StringSplitOptions.RemoveEmptyEntries);
foreach(var rawLine in lines){
string line=rawLine.Trim();
if(string.IsNullOrEmpty(line))continue;
if(line.StartsWith("#")||line.StartsWith("//"))continue;
if(_rules.Count>=MAX_RULES)break;
var rule=ParseLine(line);
rule.Raw=line;
_rules.Add(rule);
}
int valid=0;
foreach(var r in _rules)if(r.Valid)valid++;
_lastUnderstood=valid+"/"+_rules.Count+" rules parsed OK";
foreach(var r in _rules){
if(!r.Valid){
_lastUnderstood+=" | ERR: "+(r.Raw.Length>26?r.Raw.Substring(0,26)+"...":r.Raw)+" ??? "+SuggestParseFix(r);
break;
}
}
}
string StripBlockComments(string s){
if(s.IndexOf("/*",StringComparison.Ordinal)<0)return s;
var sb=new StringBuilder(s.Length);
int i=0;
while(i<s.Length){
if(i+1<s.Length&&s[i]=='/'&&s[i+1]=='*'){
int end=s.IndexOf("*/",i+2,StringComparison.Ordinal);
if(end<0)break;
i=end+2;
continue;
}
sb.Append(s[i]);
i++;
}
return sb.ToString();
}
string SuggestParseFix(ParsedRule r){
string raw=(r.Raw??"").ToLowerInvariant();
if(raw.Contains("shoo turret"))return"did you mean 'shoot turret'?";
if(raw.Contains("turn lite")||raw.Contains("turn lights "))return"name colors after 'lights' (red/green/blue/...)";
if(raw.Contains(" of "))return"try '<verb> <name> <value>' without 'of'";
if(raw.Contains("shoting"))return"did you mean 'shooting'?";
if(raw.Contains("recharg")&&!raw.Contains("recharge"))return"try 'set batteries recharge'";
if(!raw.Contains(" "))return"rules need a verb ??? try 'if <cond> <action>'";
if(!raw.Contains("if ")&&!raw.Contains("when ")&&!raw.Contains("every "))return"rules start with 'if' or 'when' (or 'every N seconds if ...')";
return r.ParseError??"syntax";
}
ParsedRule ParseLine(string line){
var rule=new ParsedRule();
string working=line.Trim();
while(working.StartsWith("[")){
int closeBracket=working.IndexOf(']');
if(closeBracket<=0||closeBracket>60)break;
string tag=working.Substring(1,closeBracket-1).Trim();
string tagLower=tag.ToLowerInvariant();
if(tagLower.StartsWith("mode ")){
foreach(var m in tag.Substring(5).Split(new[]{
' ',','}
,StringSplitOptions.RemoveEmptyEntries))rule.ModeTags.Add(m.Trim().ToLowerInvariant());
}
else if(tagLower.StartsWith("priority")){
string rest=tag.Substring(8).Replace("="," ").Trim();
int p;
if(int.TryParse(rest,out p))rule.Priority=p;
}
else if(tagLower.StartsWith("cooldown")){
string rest=tag.Substring(8).Replace("="," ").Replace("s"," ").Trim();
double n;
if(double.TryParse(rest,out n))rule.EveryNTicks=(int)(n*6.0);
}
else if(string.IsNullOrEmpty(rule.Label))rule.Label=tag;
else rule.ModeTags.Add(tagLower);
working=working.Substring(closeBracket+1).TrimStart();
}
if(working.StartsWith("every ",StringComparison.OrdinalIgnoreCase)){
int consumed;
double seconds;
if(TryParseEveryPrefix(working,out consumed,out seconds)){
rule.EveryNTicks=(int)(seconds*6.0);
working=working.Substring(consumed).TrimStart();
}
}
if(working.StartsWith("if ",StringComparison.OrdinalIgnoreCase)){
working=working.Substring(3).TrimStart();
rule.Mode=RuleMode.ContinuousIf;
}
else if(working.StartsWith("when ",StringComparison.OrdinalIgnoreCase)){
working=working.Substring(5).TrimStart();
rule.Mode=RuleMode.RisingEdgeWhen;
}
string lower=NormalizeLower(working);
int actionStart=FindActionStart(lower);
int actionStartOrig=FindActionStartInOriginal(working);
if(actionStart<0||actionStartOrig<0){
rule.ParseError="No action found";
return rule;
}
string condPart=lower.Substring(0,actionStart).Trim();
string actionPart=lower.Substring(actionStart).Trim();
string actionPartOrig=working.Substring(actionStartOrig).Trim();
if(rule.Mode==RuleMode.RisingEdgeWhen){
if(EndsWithWord(condPart,"ends")||EndsWithWord(condPart,"stops")||EndsWithWord(condPart,"gone")||EndsWithWord(condPart,"clears")||EndsWithWord(condPart,"clear")||EndsWithWord(condPart,"ended")){
rule.Mode=RuleMode.FallingEdgeWhen;
condPart=StripTrailingWord(condPart);
}
else if(EndsWithWord(condPart,"starts")||EndsWithWord(condPart,"begins")||EndsWithWord(condPart,"started")||EndsWithWord(condPart,"becomes")){
condPart=StripTrailingWord(condPart);
}
string cPlain=condPart.Trim();
if(cPlain=="enemy"||cPlain=="enemies")condPart="enemy detected";
else if(cPlain=="damage"||cPlain=="damaged")condPart="block damaged";
}
ParseCondition(rule,condPart);
string thenLower=actionPart,thenOrig=actionPartOrig;
string elseLower="",elseOrig="";
int otherIdx=actionPart.IndexOf(" otherwise ",StringComparison.OrdinalIgnoreCase);
int otherLen=11;
if(otherIdx<0){
otherIdx=actionPart.IndexOf(" else ",StringComparison.OrdinalIgnoreCase);
otherLen=6;
}
if(otherIdx>=0){
thenLower=actionPart.Substring(0,otherIdx).Trim();
elseLower=actionPart.Substring(otherIdx+otherLen).Trim();
int otherIdxOrig=actionPartOrig.IndexOf(" otherwise ",StringComparison.OrdinalIgnoreCase);
int otherLenOrig=11;
if(otherIdxOrig<0){
otherIdxOrig=actionPartOrig.IndexOf(" else ",StringComparison.OrdinalIgnoreCase);
otherLenOrig=6;
}
if(otherIdxOrig>=0){
thenOrig=actionPartOrig.Substring(0,otherIdxOrig).Trim();
elseOrig=actionPartOrig.Substring(otherIdxOrig+otherLenOrig).Trim();
}
}
ParseAction(rule,thenLower,thenOrig);
if(elseLower.Length>0)ParseElseAction(rule,elseLower,elseOrig);
rule.Valid=string.IsNullOrEmpty(rule.ParseError)&&rule.Actions.Count>0;
return rule;
}
string NormalizeLower(string s){
return s.ToLowerInvariant().Replace("less then","less than").Replace("more then","more than").Replace("airvent","air vent").Replace("day light","daylight").Replace("day time","daytime").Replace("night time","nighttime");
}
bool TryParseEveryPrefix(string s,out int consumed,out double seconds){
consumed=0;
seconds=0;
if(!s.StartsWith("every ",StringComparison.OrdinalIgnoreCase))return false;
int i=6;
var sb=new StringBuilder();
bool dot=false;
while(i<s.Length&&(char.IsDigit(s[i])||(s[i]=='.'&&!dot))){
if(s[i]=='.')dot=true;
sb.Append(s[i]);
i++;
}
double n;
if(sb.Length==0||!double.TryParse(sb.ToString(),out n))return false;
while(i<s.Length&&s[i]==' ')i++;
int unitStart=i;
while(i<s.Length&&char.IsLetter(s[i]))i++;
string unit=s.Substring(unitStart,i-unitStart).ToLowerInvariant();
if(unit.StartsWith("min"))n*=60.0;
else if(unit.StartsWith("hour"))n*=3600.0;
seconds=n;
consumed=i;
return true;
}
bool EndsWithWord(string s,string word){
string t=s.TrimEnd();
if(string.IsNullOrEmpty(t)||string.IsNullOrEmpty(word))return false;
if(!t.EndsWith(word,StringComparison.OrdinalIgnoreCase))return false;
int boundary=t.Length-word.Length;
return boundary==0||!char.IsLetter(t[boundary-1]);
}
string StripTrailingWord(string s){
string t=s.TrimEnd();
int i=t.Length-1;
while(i>=0&&char.IsLetter(t[i]))i--;
return t.Substring(0,i+1).TrimEnd();
}
void ParseElseAction(ParsedRule rule,string actLower,string actOrig){
var lowerSegs=SplitOnAndQuoteAware(actLower);
var origSegs=SplitOnAndQuoteAware(actOrig);
for(int i=0;
i<lowerSegs.Count&&rule.OtherwiseActions.Count<MAX_ACTIONS_PER_RULE;
i++){
string lp=lowerSegs[i];
string op=i<origSegs.Count?origSegs[i]:lp;
var part=ParseSingleActionSegment(lp,op);
if(part!=null)rule.OtherwiseActions.Add(part);
}
}
const int MAX_ACTIONS_PER_RULE=5;
List<string>SplitOnAndQuoteAware(string s){
var parts=new List<string>();
if(string.IsNullOrEmpty(s))return parts;
var cur=new StringBuilder();
bool inQuote=false;
int i=0;
while(i<s.Length){
char c=s[i];
if(c=='"'){
inQuote=!inQuote;
cur.Append(c);
i++;
continue;
}
if(!inQuote&&i+5<=s.Length&&string.Compare(s,i," and ",0,5,StringComparison.OrdinalIgnoreCase)==0){
parts.Add(cur.ToString().Trim());
cur.Length=0;
i+=5;
continue;
}
cur.Append(c);
i++;
}
string tail=cur.ToString().Trim();
if(tail.Length>0)parts.Add(tail);
return parts;
}
void ParseDuration(ActionPart part,string actLower){
int idx=actLower.IndexOf(" for ",StringComparison.Ordinal);
if(idx<0)return;
string rest=actLower.Substring(idx+5).TrimStart();
var sb=new StringBuilder();
bool hasDot=false;
foreach(char c in rest){
if(char.IsDigit(c))sb.Append(c);
else if(c=='.'&&!hasDot){
sb.Append(c);
hasDot=true;
}
else break;
}
if(sb.Length==0)return;
double dur;
if(double.TryParse(sb.ToString(),out dur)&&dur>0)part.DurationSec=dur;
}
void ParseCondition(ParsedRule rule,string cond){
int orIdx=FindCondBoundary(cond," or ");
int andIdx=FindCondBoundary(cond," and ");
int splitIdx=-1;
CondJoin join=CondJoin.None;
int joinLen=0;
if(orIdx>=0&&(andIdx<0||orIdx<andIdx)){
splitIdx=orIdx;
join=CondJoin.Or;
joinLen=4;
}
else if(andIdx>=0){
splitIdx=andIdx;
join=CondJoin.And;
joinLen=5;
}
if(splitIdx>0){
string first=cond.Substring(0,splitIdx).Trim();
string second=cond.Substring(splitIdx+joinLen).Trim();
ParsePrimaryCondition(rule,first);
var temp=new ParsedRule();
ParsePrimaryCondition(temp,second);
rule.Join=join;
rule.Cond2=temp.Condition;
rule.Cond2Value=temp.CondValue;
rule.Cond2Target=temp.CondTarget;
rule.Cond2Item=temp.ItemName;
}
else{
ParsePrimaryCondition(rule,cond);
}
}
int FindCondBoundary(string s,string needle){
bool inQuote=false;
for(int i=0;
i+needle.Length<=s.Length;
i++){
if(s[i]=='"')inQuote=!inQuote;
if(!inQuote&&string.Compare(s,i,needle,0,needle.Length,StringComparison.OrdinalIgnoreCase)==0)return i;
}
return-1;
}
void ParsePrimaryCondition(ParsedRule rule,string cond){
if(string.IsNullOrWhiteSpace(cond)||cond=="always"){
rule.Condition=CondType.Always;
return;
}
if(Contains(cond,"argument")||Contains(cond,"command")){
string aq=ExtractQuoted(cond);
if(!string.IsNullOrEmpty(aq)){
rule.Condition=CondType.ArgumentMatches;
rule.CondTarget=aq;
return;
}
}
if((Contains(cond,"received")||Contains(cond,"got message")||Contains(cond,"signal arrives"))&&!string.IsNullOrEmpty(ExtractQuoted(cond))){
rule.Condition=CondType.IGCReceived;
rule.CondTarget=ExtractQuoted(cond);
EnsureIGCListener(rule.CondTarget);
return;
}
if(Contains(cond,"camera")&&(Contains(cond,"sees")||Contains(cond,"detect")||Contains(cond,"spots"))){
rule.Condition=Contains(cond,"enemy")||Contains(cond,"hostile")?CondType.CameraSeesEnemy:CondType.CameraSees;
rule.CondTarget=ExtractQuoted(cond);
rule.CondValue=ExtractNumber(cond);
if(rule.CondValue<=0)rule.CondValue=200;
return;
}
if(Contains(cond,"night time")||Contains(cond,"nighttime")){
rule.Condition=CondType.NightTime;
return;
}
if(Contains(cond,"day time")||Contains(cond,"daytime")){
rule.Condition=CondType.DayTime;
return;
}
if(Contains(cond,"hour")||Contains(cond,"clock")){
if(Contains(cond,"after")||Contains(cond,"past")){
rule.Condition=CondType.GameHourAfter;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond,"before")){
rule.Condition=CondType.GameHourBefore;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond," is ")||Contains(cond,"equals")){
rule.Condition=CondType.GameHourIs;
rule.CondValue=ExtractNumber(cond);
return;
}
}
if(Contains(cond,"in gravity")||Contains(cond,"on planet")||Contains(cond,"in atmosphere")){
rule.Condition=CondType.InGravity;
return;
}
if(Contains(cond,"in space")||Contains(cond,"no gravity")){
rule.Condition=CondType.InSpace;
return;
}
if(Contains(cond,"leveling")||Contains(cond,"levelling")||(Contains(cond,"gyro")&&(Contains(cond,"overrid")||Contains(cond,"active")))){
rule.Condition=CondType.GyroOverriding;
return;
}
if((Contains(cond,"triggered")||Contains(cond,"sensor"))&&!string.IsNullOrEmpty(ExtractQuoted(cond))&&!Contains(cond,"damaged")){
string sq=ExtractQuoted(cond);
if(Contains(cond,"triggered")||Contains(cond,"active")){
rule.Condition=CondType.SensorTriggered;
rule.CondTarget=sq;
return;
}
}
if(Contains(cond,"count")&&!Contains(cond,"count of")&&!Contains(cond,"remember")){
string blockType=ExtractBlockTypeKeyword(cond);
if(!string.IsNullOrEmpty(blockType)){
if(Contains(cond,"below")||Contains(cond,"less than")||Contains(cond,"under")){
rule.Condition=CondType.BlockCountBelow;
rule.CondTarget=blockType;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond,"above")||Contains(cond,"more than")||Contains(cond,"over")){
rule.Condition=CondType.BlockCountAbove;
rule.CondTarget=blockType;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond," is ")||Contains(cond,"equals")){
rule.Condition=CondType.BlockCountIs;
rule.CondTarget=blockType;
rule.CondValue=ExtractNumber(cond);
return;
}
}
}
if(Contains(cond,"ammo")&&!string.IsNullOrEmpty(ExtractQuoted(cond))&&(Contains(cond,"below")||Contains(cond,"less than"))){
rule.Condition=CondType.GunAmmoBelow;
rule.CondTarget=ExtractQuoted(cond);
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond,"count ")||cond.StartsWith("count ")){
string vname=ExtractQuoted(cond);
if(string.IsNullOrEmpty(vname))vname=ExtractVarName(cond,"count");
if(!string.IsNullOrEmpty(vname)){
if(Contains(cond,"above")||Contains(cond,"more than")||Contains(cond,"over")){
rule.Condition=CondType.VarAbove;
rule.CondTarget=vname;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond,"below")||Contains(cond,"less than")||Contains(cond,"under")){
rule.Condition=CondType.VarBelow;
rule.CondTarget=vname;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond," is ")||Contains(cond,"equals")){
rule.Condition=CondType.VarIs;
rule.CondTarget=vname;
rule.CondValue=ExtractNumber(cond);
return;
}
}
}
if(Contains(cond,"remember")||Contains(cond,"remembers")||Contains(cond,"is remembered")||Contains(cond,"forgot")||Contains(cond,"forgotten")){
string vname=ExtractQuoted(cond);
if(string.IsNullOrEmpty(vname))vname=ExtractVarName(cond,"remember");
if(string.IsNullOrEmpty(vname))vname=ExtractVarName(cond,"forgot");
if(!string.IsNullOrEmpty(vname)){
bool wantOn=!Contains(cond,"forgot")&&!Contains(cond,"forgotten")&&!Contains(cond,"not remember");
rule.Condition=wantOn?CondType.Remembered:CondType.Forgotten;
rule.CondTarget=vname;
return;
}
}
if(Contains(cond,"speed")||Contains(cond,"velocity")){
if(Contains(cond,"above")||Contains(cond,"more than")||Contains(cond,"over")||Contains(cond,"greater")){
rule.Condition=CondType.SpeedAbove;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond,"below")||Contains(cond,"less than")||Contains(cond,"under")){
rule.Condition=CondType.SpeedBelow;
rule.CondValue=ExtractNumber(cond);
return;
}
}
if(Contains(cond,"altitude")||Contains(cond,"elevation")){
if(Contains(cond,"above")||Contains(cond,"more than")||Contains(cond,"over")){
rule.Condition=CondType.AltitudeAbove;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond,"below")||Contains(cond,"less than")||Contains(cond,"under")){
rule.Condition=CondType.AltitudeBelow;
rule.CondValue=ExtractNumber(cond);
return;
}
}
if(Contains(cond,"pilot")||Contains(cond,"cockpit")){
if(Contains(cond,"no ")||Contains(cond,"not ")||Contains(cond,"empty")||Contains(cond,"absent")){
rule.Condition=CondType.PilotAbsent;
return;
}
rule.Condition=CondType.PilotPresent;
return;
}
if(Contains(cond,"damaged")||Contains(cond,"damage")||Contains(cond,"hull damage")||Contains(cond,"hull damaged")||Contains(cond,"is broken")){
string dq=ExtractQuoted(cond);
bool wantNot=Contains(cond,"not damaged")||Contains(cond,"no damage")||Contains(cond,"undamaged");
rule.Condition=wantNot?CondType.BlockNotDamaged:CondType.BlockDamaged;
rule.CondTarget=dq;
return;
}
if(Contains(cond,"shooting")||Contains(cond,"firing")){
string sq=ExtractQuoted(cond);
bool wantNot=Contains(cond,"not shooting")||Contains(cond,"not firing")||Contains(cond,"stop shooting")||Contains(cond,"stops shooting")||Contains(cond,"stop firing")||Contains(cond,"stops firing")||Contains(cond,"no shooting")||Contains(cond,"not fire");
rule.Condition=wantNot?CondType.GunNotShooting:CondType.GunShooting;
rule.CondTarget=sq;
return;
}
if(Contains(cond,"enemy detected")||Contains(cond,"enemy near")||Contains(cond,"under attack")){
rule.Condition=CondType.EnemyDetected;
return;
}
if(Contains(cond,"enemy not detected")||Contains(cond,"no enemy")||Contains(cond,"enemy clear")){
rule.Condition=CondType.EnemyNotDetected;
return;
}
string powerQ=ExtractQuoted(cond);
bool hasPowerKeyword=Contains(cond,"battery")||Contains(cond,"batteries")||Contains(cond,"charge")||Contains(cond,"%")||Contains(cond,"percent");
if(!string.IsNullOrEmpty(powerQ)&&hasPowerKeyword){
if(Contains(cond,"less than")||Contains(cond,"below")||Contains(cond,"under")){
rule.Condition=CondType.NamedBatteryBelow;
rule.CondValue=ExtractNumber(cond);
rule.CondTarget=powerQ;
return;
}
if(Contains(cond,"more than")||Contains(cond,"above")||Contains(cond,"over")){
rule.Condition=CondType.NamedBatteryAbove;
rule.CondValue=ExtractNumber(cond);
rule.CondTarget=powerQ;
return;
}
}
if(Contains(cond,"power is full")||Contains(cond,"power full")||Contains(cond,"batteries full")){
rule.Condition=CondType.PowerFull;
return;
}
if((Contains(cond,"power")||Contains(cond,"batteries")||Contains(cond,"battery"))&&(Contains(cond,"less than")||Contains(cond,"below")||Contains(cond,"under"))){
rule.Condition=CondType.PowerBelow;
rule.CondValue=ExtractNumber(cond);
return;
}
if((Contains(cond,"power")||Contains(cond,"batteries")||Contains(cond,"battery"))&&(Contains(cond,"more than")||Contains(cond,"above")||Contains(cond,"over"))){
rule.Condition=CondType.PowerAbove;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond,"battery")&&(Contains(cond,"less than")||Contains(cond,"below"))){
rule.Condition=CondType.BatteryBelow;
rule.CondValue=ExtractNumber(cond);
return;
}
bool isDarkPhrase=Contains(cond,"dark")||Contains(cond,"night")||Contains(cond,"nighttime")||Contains(cond,"no sun")||Contains(cond,"sun down")||Contains(cond,"solar panels are not detecting sun")||Contains(cond,"solar not detecting sun")||Contains(cond,"solar panels are not")||Contains(cond,"solar output is less than")||Contains(cond,"solar output below");
if(isDarkPhrase){
if(Contains(cond,"less than")||Contains(cond,"below")){
rule.Condition=CondType.SolarBelow;
rule.CondValue=ExtractNumber(cond);
}
else{
rule.Condition=CondType.Dark;
}
return;
}
bool isDayPhrase=Contains(cond,"daylight")||Contains(cond,"daytime")||Contains(cond,"sun up")||Contains(cond,"sun is up")||Contains(cond,"sun detected")||Contains(cond,"solar panels detecting sun")||Contains(cond,"solar detecting sun")||Contains(cond,"solar output is more than")||Contains(cond,"solar output above");
if(isDayPhrase){
if(Contains(cond,"more than")||Contains(cond,"above")){
rule.Condition=CondType.SolarAbove;
rule.CondValue=ExtractNumber(cond);
}
else{
rule.Condition=CondType.Daylight;
}
return;
}
string connQ=ExtractQuoted(cond);
if(!string.IsNullOrEmpty(connQ)&&(Contains(cond,"docked")||Contains(cond,"connected")||Contains(cond,"undocked")||Contains(cond,"disconnected"))){
if(Contains(cond,"undocked")||Contains(cond,"disconnected")||Contains(cond,"not connected")||Contains(cond,"not docked")){
rule.Condition=CondType.NamedConnectorUndocked;
rule.CondTarget=connQ;
return;
}
rule.Condition=CondType.NamedConnectorDocked;
rule.CondTarget=connQ;
return;
}
if(Contains(cond,"undocked")||Contains(cond,"connector disconnected")||Contains(cond,"not connected")){
rule.Condition=CondType.ConnectorUndocked;
return;
}
if(Contains(cond,"connector connected")||Contains(cond,"docked")||Contains(cond,"connector is connected")){
rule.Condition=CondType.ConnectorDocked;
return;
}
if(Contains(cond,"oxygen is low")||Contains(cond,"o2 is low")){
rule.Condition=CondType.OxygenLow;
return;
}
if((Contains(cond,"oxygen")||Contains(cond,"o2"))&&(Contains(cond,"less than")||Contains(cond,"below"))){
rule.Condition=CondType.OxygenBelow;
rule.CondValue=ExtractNumber(cond);
return;
}
if(Contains(cond,"hydrogen is low")||Contains(cond,"h2 is low")){
rule.Condition=CondType.HydrogenLow;
return;
}
if((Contains(cond,"hydrogen")||Contains(cond,"h2"))&&(Contains(cond,"less than")||Contains(cond,"below"))){
rule.Condition=CondType.HydrogenBelow;
rule.CondValue=ExtractNumber(cond);
return;
}
string cargoQ=ExtractQuoted(cond);
if(!string.IsNullOrEmpty(cargoQ)&&Contains(cond,"cargo")&&(Contains(cond,"more than")||Contains(cond,"above")||Contains(cond,"over")||Contains(cond,"is full")||Contains(cond,"full"))){
rule.Condition=CondType.NamedCargoAbove;
rule.CondTarget=cargoQ;
rule.CondValue=Contains(cond,"full")?95.0:ExtractNumber(cond);
return;
}
if(Contains(cond,"cargo is full")||Contains(cond,"cargo full")){
rule.Condition=CondType.CargoFull;
return;
}
if(Contains(cond,"cargo")&&(Contains(cond,"more than")||Contains(cond,"above")||Contains(cond,"over"))){
rule.Condition=CondType.CargoAbove;
rule.CondValue=ExtractNumber(cond);
return;
}
foreach(var alias in ItemAliases.Keys){
if(Contains(cond,alias)&&(Contains(cond,"less than")||Contains(cond,"below")||Contains(cond,"are less")||Contains(cond,"ammo is low"))){
rule.Condition=CondType.ItemBelow;
rule.ItemName=ItemAliases[alias];
rule.CondValue=ExtractNumber(cond);
if(Contains(cond,"ammo is low"))rule.CondValue=100;
return;
}
}
if(Contains(cond,"refinery")){
string rq=ExtractQuoted(cond);
bool wantIdle=Contains(cond,"idle")||Contains(cond,"not working")||Contains(cond,"not producing")||Contains(cond,"queue empty")||Contains(cond,"queue is empty");
bool wantWorking=Contains(cond,"is working")||Contains(cond,"is producing")||Contains(cond,"is active")||Contains(cond,"producing");
if(wantIdle){
rule.Condition=CondType.RefineryIdle;
rule.CondTarget=rq;
return;
}
if(wantWorking){
rule.Condition=CondType.RefineryWorking;
rule.CondTarget=rq;
return;
}
}
if(Contains(cond,"assembler")){
string aq=ExtractQuoted(cond);
bool wantIdle=Contains(cond,"idle")||Contains(cond,"queue empty")||Contains(cond,"queue is empty")||Contains(cond,"not working")||Contains(cond,"not producing");
bool wantWorking=Contains(cond,"is working")||Contains(cond,"is producing")||Contains(cond,"queue not empty")||Contains(cond,"queue is not empty");
if(wantIdle){
rule.Condition=CondType.AssemblerIdle;
rule.CondTarget=aq;
return;
}
if(wantWorking){
rule.Condition=CondType.AssemblerWorking;
rule.CondTarget=aq;
return;
}
}
string quotedBlock=ExtractQuoted(cond);
if(!string.IsNullOrEmpty(quotedBlock)){
if(Contains(cond,"is off")||Contains(cond,"is disabled")){
rule.Condition=CondType.BlockIsOff;
rule.CondTarget=quotedBlock;
return;
}
if(Contains(cond,"is on")||Contains(cond,"is enabled")){
rule.Condition=CondType.BlockIsOn;
rule.CondTarget=quotedBlock;
return;
}
}
if(Contains(cond,"air vent")){
string vq=ExtractQuoted(cond);
bool wantOff=Contains(cond,"off")||Contains(cond,"disabled")||Contains(cond,"broken")||Contains(cond,"not working");
rule.Condition=wantOff?CondType.AirVentOff:CondType.AirVentOn;
rule.CondTarget=vq;
return;
}
rule.ParseError=$"Unknown condition: {cond}";
}
void ParseAction(ParsedRule rule,string actLower,string actOrig){
var lowerSegs=SplitOnAndQuoteAware(actLower);
var origSegs=SplitOnAndQuoteAware(actOrig);
if(lowerSegs.Count==0){
rule.ParseError=(rule.ParseError??"")+"|No action found";
return;
}
for(int i=0;
i<lowerSegs.Count&&rule.Actions.Count<MAX_ACTIONS_PER_RULE;
i++){
string lp=lowerSegs[i];
string op=i<origSegs.Count?origSegs[i]:lp;
var part=ParseSingleActionSegment(lp,op);
if(part!=null)rule.Actions.Add(part);
else rule.ParseError=(rule.ParseError??"")+$"|Unknown action: {lp}";
}
}
ActionPart ParseSingleActionSegment(string actLower,string actOrig){
var part=new ActionPart();
string quoted=ExtractQuoted(actOrig);
string quoted2=ExtractSecondQuoted(actOrig);
part.ActTarget=quoted;
part.SecondTarget=quoted2;
part.RawLower=actLower;
if(actLower.StartsWith("say ")&&!string.IsNullOrEmpty(quoted)){
part.Action=ActType.Say;
ParseDuration(part,actLower);
return part;
}
if((actLower.StartsWith("write ")||actLower.StartsWith("print ")||actLower.StartsWith("show "))&&!string.IsNullOrEmpty(quoted)){
part.Action=ActType.WriteToLCD;
return part;
}
bool turretWord=Contains(actLower,"turret")||Contains(actLower,"gun");
bool shootWord=Contains(actLower,"shoot")||Contains(actLower,"fire ");
if(shootWord&&turretWord){
if(Contains(actLower,"once")){
part.Action=ActType.ShootOnce;
return part;
}
if(Contains(actLower," off")||Contains(actLower,"stop")){
part.Action=ActType.ShootOff;
return part;
}
part.Action=ActType.ShootOn;
return part;
}
if(shootWord&&Contains(actLower,"once")){
part.Action=ActType.ShootOnce;
return part;
}
if(Contains(actLower,"stop shoot")||Contains(actLower,"cease fire")){
part.Action=ActType.ShootOff;
return part;
}
if(Contains(actLower,"piston")){
ParseNumParam(part,actLower);
if(Contains(actLower,"extend")||Contains(actLower,"expand")||Contains(actLower," out")){
part.Action=ActType.PistonExtend;
return part;
}
if(Contains(actLower,"retract")||Contains(actLower," in")){
part.Action=ActType.PistonRetract;
return part;
}
if(Contains(actLower,"velocity")||Contains(actLower,"speed")){
part.Action=ActType.PistonSetVelocity;
return part;
}
if(Contains(actLower,"max limit")||Contains(actLower,"max length")){
part.Action=ActType.PistonSetMaxLimit;
return part;
}
if(Contains(actLower,"min limit")||Contains(actLower,"min length")){
part.Action=ActType.PistonSetMinLimit;
return part;
}
}
if(Contains(actLower,"rotor")||Contains(actLower,"hinge")){
ParseNumParam(part,actLower);
if(Contains(actLower,"lock")){
part.Action=ActType.RotorLock;
return part;
}
if(Contains(actLower,"unlock")){
part.Action=ActType.RotorUnlock;
return part;
}
if(Contains(actLower,"angle")||Contains(actLower,"degree")){
part.Action=ActType.RotorSetAngle;
return part;
}
if(Contains(actLower,"velocity")||Contains(actLower,"rpm")){
part.Action=ActType.RotorSetVelocity;
return part;
}
if(Contains(actLower,"rotate")||Contains(actLower,"spin")||Contains(actLower,"turn ")){
part.Action=ActType.RotorRotate;
return part;
}
}
if(Contains(actLower,"timer")){
if(Contains(actLower,"trigger")){
part.Action=ActType.TimerTrigger;
return part;
}
if(Contains(actLower,"stop")){
part.Action=ActType.TimerStop;
return part;
}
if(Contains(actLower,"start")||Contains(actLower,"run ")){
part.Action=ActType.TimerStart;
return part;
}
}
if(Contains(actLower,"sorter")){
if(Contains(actLower,"stop drain")||Contains(actLower,"drain off")){
part.Action=ActType.SorterDrainOff;
return part;
}
if(Contains(actLower,"drain")){
part.Action=ActType.SorterDrainOn;
return part;
}
}
if((actLower.StartsWith("broadcast ")||Contains(actLower,"send signal")||Contains(actLower,"antenna"))&&!string.IsNullOrEmpty(quoted)){
part.Action=ActType.AntennaBroadcast;
return part;
}
if((actLower.StartsWith("move ")||actLower.StartsWith("send "))&&!string.IsNullOrEmpty(quoted)&&!string.IsNullOrEmpty(quoted2)&&Contains(actLower," to ")){
part.Action=ActType.MoveItemsBetween;
foreach(var alias in ItemAliases.Keys)if(Contains(quoted.ToLowerInvariant(),alias)){
part.ItemName=ItemAliases[alias];
break;
}
return part;
}
if(actLower.StartsWith("count ")||actLower.StartsWith("increment ")){
part.Action=ActType.VarCount;
part.ActParam=ExtractVarNameForAction(actLower,actOrig,actLower.StartsWith("count ")?"count":"increment");
return part;
}
if(actLower.StartsWith("reset ")||actLower.StartsWith("clear ")){
string keyword=actLower.StartsWith("reset ")?"reset":"clear";
string nm=ExtractVarNameForAction(actLower,actOrig,keyword);
if(!string.IsNullOrEmpty(nm)&&!nm.Equals("log",StringComparison.OrdinalIgnoreCase)){
part.Action=ActType.VarReset;
part.ActParam=nm;
return part;
}
}
if(actLower.StartsWith("remember ")){
part.Action=ActType.VarRemember;
part.ActParam=ExtractVarNameForAction(actLower,actOrig,"remember");
return part;
}
if(actLower.StartsWith("forget ")){
part.Action=ActType.VarForget;
part.ActParam=ExtractVarNameForAction(actLower,actOrig,"forget");
return part;
}
if(actLower.StartsWith("add ")&&Contains(actLower," to ")){
ParseNumParam(part,actLower);
int toIdx=actLower.IndexOf(" to ",StringComparison.Ordinal);
part.ActParam=ExtractVarTail(actOrig,toIdx+4);
part.Action=ActType.VarAdd;
return part;
}
if(actLower.StartsWith("subtract ")&&Contains(actLower," from ")){
ParseNumParam(part,actLower);
int fromIdx=actLower.IndexOf(" from ",StringComparison.Ordinal);
part.ActParam=ExtractVarTail(actOrig,fromIdx+6);
part.Action=ActType.VarSubtract;
return part;
}
if(actLower.StartsWith("set ")&&Contains(actLower," to ")&&!HasBlockKeyword(actLower)){
ParseNumParam(part,actLower);
int toIdx=actLower.IndexOf(" to ",StringComparison.Ordinal);
string nm=actOrig.Substring(4,toIdx-4).Trim().Trim('"');
if(!string.IsNullOrEmpty(nm)){
part.Action=ActType.VarSet;
part.ActParam=nm;
return part;
}
}
if(actLower.StartsWith("broadcast ")||actLower.StartsWith("send signal ")||actLower.StartsWith("send ")){
if(!string.IsNullOrEmpty(quoted)){
part.Action=ActType.IGCBroadcast;
return part;
}
}
if(Contains(actLower,"camera")){
if(Contains(actLower,"scan on")||Contains(actLower,"enable scan")||Contains(actLower,"enable raycast")){
part.Action=ActType.CameraScanOn;
return part;
}
if(Contains(actLower,"scan off")||Contains(actLower,"disable scan")||Contains(actLower,"disable raycast")){
part.Action=ActType.CameraScanOff;
return part;
}
}
if(Contains(actLower,"gear")||Contains(actLower,"landing")){
if(Contains(actLower,"lock")){
part.Action=ActType.GearLock;
return part;
}
if(Contains(actLower,"unlock")||Contains(actLower,"release")){
part.Action=ActType.GearUnlock;
return part;
}
}
if(Contains(actLower,"beacon")){
if(Contains(actLower,"name")||Contains(actLower,"text")||(!string.IsNullOrEmpty(quoted)&&(Contains(actLower,"set ")))){
part.Action=ActType.BeaconSetText;
return part;
}
if(Contains(actLower," on")||Contains(actLower,"enable")||actLower.StartsWith("set beacon")){
part.Action=ActType.BeaconOn;
return part;
}
if(Contains(actLower," off")||Contains(actLower,"disable")){
part.Action=ActType.BeaconOff;
return part;
}
}
if(Contains(actLower,"projector")){
if(Contains(actLower,"enable")||Contains(actLower," on")){
part.Action=ActType.ProjectorOn;
return part;
}
if(Contains(actLower,"disable")||Contains(actLower," off")){
part.Action=ActType.ProjectorOff;
return part;
}
}
if(Contains(actLower,"jump drive")||Contains(actLower,"jumpdrive")){
if(Contains(actLower,"charge")||Contains(actLower," on")){
part.Action=ActType.JumpDriveCharge;
return part;
}
}
if(actLower.StartsWith("jump ")||actLower=="jump"){
part.Action=ActType.JumpDriveJump;
return part;
}
if(Contains(actLower,"level ship")||(Contains(actLower,"level")&&Contains(actLower,"gyro"))){
part.Action=ActType.LevelShip;
return part;
}
if(Contains(actLower,"release gyros")||Contains(actLower,"release gyro")||Contains(actLower,"free gyros")){
part.Action=ActType.ReleaseGyros;
return part;
}
if(Contains(actLower,"volume")&&Contains(actLower,"sound")==false&&Contains(actLower,"alarm")==false&&Contains(actLower,"block")==false){
ParseNumParam(part,actLower);
part.Action=ActType.SetSoundVolume;
return part;
}
if(Contains(actLower,"volume")&&(Contains(actLower,"sound")||Contains(actLower,"alarm"))){
ParseNumParam(part,actLower);
part.Action=ActType.SetSoundVolume;
return part;
}
if(Contains(actLower,"parachute")||actLower.StartsWith("deploy ")){
part.Action=ActType.ParachuteDeploy;
return part;
}
if(Contains(actLower,"wheels")||Contains(actLower,"suspension")){
ParseNumParam(part,actLower);
if(Contains(actLower,"strength")||Contains(actLower,"stiffness")){
part.Action=ActType.WheelSetStrength;
return part;
}
if(Contains(actLower,"speed")||Contains(actLower,"velocity")){
part.Action=ActType.WheelSetSpeed;
return part;
}
}
if(Contains(actLower,"handbrake")||Contains(actLower,"hand brake")||Contains(actLower,"parking brake")){
if(Contains(actLower," on")||Contains(actLower,"enable")||Contains(actLower,"engage")){
part.Action=ActType.HandbrakeOn;
return part;
}
if(Contains(actLower," off")||Contains(actLower,"disable")||Contains(actLower,"release")){
part.Action=ActType.HandbrakeOff;
return part;
}
}
if((actLower.StartsWith("run ")||actLower.StartsWith("invoke "))&&!string.IsNullOrEmpty(quoted)){
part.Action=ActType.RunPB;
return part;
}
if(Contains(actLower,"light")||Contains(actLower,"lights")){
ParseNumParam(part,actLower);
if(Contains(actLower,"intensity")){
part.Action=ActType.SetLightIntensity;
return part;
}
if(Contains(actLower,"blink")){
part.Action=ActType.SetLightBlink;
return part;
}
if(Contains(actLower,"radius")){
part.Action=ActType.SetLightRadius;
return part;
}
}
bool startsWithPlay=actLower.StartsWith("play ")||actLower=="play";
bool startsWithStop=actLower.StartsWith("stop ")||actLower=="stop";
if(startsWithPlay&&!string.IsNullOrEmpty(quoted)){
part.Action=ActType.PlaySoundBlocks;
ParseDuration(part,actLower);
return part;
}
if(Contains(actLower,"play sound")||Contains(actLower,"run sound")||Contains(actLower,"sound block")||Contains(actLower,"alarm on")||Contains(actLower,"play alarm")){
part.Action=ActType.PlaySoundBlocks;
ParseDuration(part,actLower);
return part;
}
if(Contains(actLower,"stop sound")||Contains(actLower,"alarm off")){
part.Action=ActType.StopSoundBlocks;
return part;
}
if(startsWithStop&&!string.IsNullOrEmpty(quoted)){
part.Action=ActType.StopSoundBlocks;
return part;
}
if(Contains(actLower,"close hangar")||(Contains(actLower,"close")&&Contains(actLower,"hangar"))){
part.Action=ActType.CloseHangarDoors;
return part;
}
if(Contains(actLower,"open hangar")||(Contains(actLower,"open")&&Contains(actLower,"hangar"))){
part.Action=ActType.OpenHangarDoors;
return part;
}
if(Contains(actLower,"close door")||(Contains(actLower,"close")&&Contains(actLower,"door"))){
part.Action=ActType.CloseDoors;
return part;
}
if(Contains(actLower,"open door")||(Contains(actLower,"open")&&Contains(actLower,"door"))){
part.Action=ActType.OpenDoors;
return part;
}
if((Contains(actLower,"batteries")||Contains(actLower,"battery"))&&Contains(actLower,"recharge")){
part.Action=ActType.SetBatteriesRecharge;
return part;
}
if((Contains(actLower,"batteries")||Contains(actLower,"battery"))&&Contains(actLower,"auto")){
part.Action=ActType.SetBatteriesAuto;
return part;
}
if(Contains(actLower,"lock connector")||Contains(actLower,"lock connect")){
part.Action=ActType.LockConnectors;
return part;
}
if(Contains(actLower,"unlock connector")||Contains(actLower,"unlock connect")){
part.Action=ActType.UnlockConnectors;
return part;
}
if(Contains(actLower,"stockpile hydrogen")||(Contains(actLower,"stockpile")&&Contains(actLower,"h2"))){
part.Action=ActType.StockpileHydrogen;
return part;
}
if(Contains(actLower,"stockpile oxygen")||(Contains(actLower,"stockpile")&&Contains(actLower,"o2"))){
part.Action=ActType.StockpileOxygen;
return part;
}
if(Contains(actLower,"move ores to refinery")||Contains(actLower,"move ore to refinery")||Contains(actLower,"feed refinery")||Contains(actLower,"send ores to refinery")||Contains(actLower,"send ore to refinery")){
part.Action=ActType.MoveOresToRefinery;
return part;
}
if(Contains(actLower,"move ingots to assembler")||Contains(actLower,"move ingot to assembler")||Contains(actLower,"feed assembler")||Contains(actLower,"send ingots to assembler")||Contains(actLower,"send ingot to assembler")){
part.Action=ActType.MoveIngotsToAssembler;
return part;
}
if(Contains(actLower,"generate more")||Contains(actLower,"queue")||Contains(actLower,"produce")){
part.Action=ActType.GenerateMore;
foreach(var alias in ItemAliases.Keys)if(Contains(actLower,alias)){
part.ItemName=ItemAliases[alias];
break;
}
return part;
}
if(Contains(actLower,"refill turret")||Contains(actLower,"rearm")){
part.Action=ActType.RefillTurrets;
return part;
}
if(Contains(actLower,"light")||Contains(actLower,"lights")){
string color=ExtractColor(actLower);
if(!string.IsNullOrEmpty(color)){
part.Action=ActType.SetLightColor;
part.ActParam=color;
return part;
}
}
bool isTurnOn=Contains(actLower,"turn on")||Contains(actLower," on")||Contains(actLower,"enable")||Contains(actLower,"activate");
bool isTurnOff=Contains(actLower,"turn off")||Contains(actLower," off")||Contains(actLower,"disable")||Contains(actLower,"deactivate");
if(isTurnOff){
part.Action=ActType.TurnOff;
return part;
}
if(isTurnOn){
part.Action=ActType.TurnOn;
return part;
}
if(Contains(actLower,"run ")||Contains(actLower,"start ")){
part.Action=ActType.TurnOn;
return part;
}
if(Contains(actLower,"turn ")&&HasBlockKeyword(actLower)){
part.Action=ActType.TurnOn;
return part;
}
return null;
}
bool HasBlockKeyword(string actLower){
return Contains(actLower,"reactor")||Contains(actLower,"batter")||Contains(actLower,"turret")||Contains(actLower,"gun")||Contains(actLower,"thruster")||Contains(actLower,"light")||Contains(actLower,"sound")||Contains(actLower,"alarm")||Contains(actLower,"siren")||Contains(actLower,"h2 gen")||Contains(actLower,"hydrogen gen")||Contains(actLower,"gas gen")||Contains(actLower,"oxygen gen")||Contains(actLower,"assembler")||Contains(actLower,"refinery")||Contains(actLower,"air vent")||Contains(actLower,"connector")||Contains(actLower,"solar")||Contains(actLower,"piston")||Contains(actLower,"rotor")||Contains(actLower,"hinge")||Contains(actLower,"timer")||Contains(actLower,"sorter")||Contains(actLower,"antenna")||Contains(actLower,"beacon")||Contains(actLower,"camera")||Contains(actLower,"projector")||Contains(actLower,"gear")||Contains(actLower,"landing")||Contains(actLower,"jump drive")||Contains(actLower,"jumpdrive")||Contains(actLower,"gyro")||Contains(actLower,"parachute")||Contains(actLower,"wheel")||Contains(actLower,"suspension")||Contains(actLower,"drill")||Contains(actLower,"welder")||Contains(actLower,"grinder");
}
int FindActionStart(string lower){
string[]actionVerbs={
"close hangar","open hangar","close all hangar","close door","open door","close all","open all","turn on ","turn off ","turn all ","turn ","set ","lock ","unlock ","play ","stop ","run ","generate","queue","produce","refill","rearm","stockpile","write ","print ","show ","enable ","disable ","activate ","deactivate ","move ","feed ","send ","shoot ","fire ","cease ","extend ","retract ","rotate ","spin ","trigger ","start ","drain ","broadcast ","say ","blink ","count ","increment ","reset ","clear ","remember ","forget ","add ","subtract ","level ","deploy ","charge ","jump ","engage ","invoke "}
;
foreach(var v in actionVerbs){
int idx=lower.IndexOf(v,StringComparison.Ordinal);
if(idx>=0)return idx;
}
return-1;
}
int FindActionStartInOriginal(string original){
return FindActionStart(original.ToLowerInvariant());
}
bool Contains(string src,string sub)=>src.IndexOf(sub,StringComparison.OrdinalIgnoreCase)>=0;
double ExtractNumber(string s){
var sb=new StringBuilder();
bool hasDot=false;
foreach(char c in s){
if(char.IsDigit(c))sb.Append(c);
else if(c=='.'&&!hasDot){
sb.Append(c);
hasDot=true;
}
else if(sb.Length>0&&!char.IsDigit(c)&&c!='.')break;
}
double result;
double.TryParse(sb.ToString(),out result);
return result;
}
string ExtractQuoted(string s){
int start=s.IndexOf('"');
if(start<0)return"";
int end=s.IndexOf('"',start+1);
if(end<0)return"";
return s.Substring(start+1,end-start-1);
}
string ExtractSecondQuoted(string s){
int first=s.IndexOf('"');
if(first<0)return"";
int firstClose=s.IndexOf('"',first+1);
if(firstClose<0)return"";
int second=s.IndexOf('"',firstClose+1);
if(second<0)return"";
int secondClose=s.IndexOf('"',second+1);
if(secondClose<0)return"";
return s.Substring(second+1,secondClose-second-1);
}
string ExtractVarNameForAction(string actLower,string actOrig,string keyword){
int idx=actLower.IndexOf(keyword,StringComparison.OrdinalIgnoreCase);
if(idx<0)return"";
int start=idx+keyword.Length;
while(start<actLower.Length&&actLower[start]==' ')start++;
int stop=actLower.Length;
int andIdx=actLower.IndexOf(" and ",start,StringComparison.OrdinalIgnoreCase);
if(andIdx>=0)stop=andIdx;
int byIdx=actLower.IndexOf(" by ",start,StringComparison.OrdinalIgnoreCase);
if(byIdx>=0&&byIdx<stop)stop=byIdx;
string name=actOrig.Substring(Math.Min(start,actOrig.Length),Math.Min(stop,actOrig.Length)-Math.Min(start,actOrig.Length)).Trim();
if(name.Length>=2&&name[0]=='"'&&name[name.Length-1]=='"')name=name.Substring(1,name.Length-2);
return name;
}
string ExtractVarTail(string actOrig,int startIdx){
if(startIdx<0||startIdx>=actOrig.Length)return"";
string tail=actOrig.Substring(startIdx).Trim();
if(tail.Length>=2&&tail[0]=='"'&&tail[tail.Length-1]=='"')tail=tail.Substring(1,tail.Length-2);
return tail;
}
void ParseNumParam(ActionPart part,string actLower){
var sb=new StringBuilder();
bool dot=false;
bool seenDigit=false;
foreach(char c in actLower){
if(char.IsDigit(c)){
sb.Append(c);
seenDigit=true;
}
else if(c=='.'&&!dot&&seenDigit){
sb.Append(c);
dot=true;
}
else if(c=='-'&&!seenDigit){
sb.Append(c);
}
else if(seenDigit)break;
}
double n;
if(sb.Length>0&&double.TryParse(sb.ToString(),out n))part.NumParam=n;
}
string ExtractColor(string s){
if(Contains(s," red"))return"red";
if(Contains(s," green"))return"green";
if(Contains(s," blue"))return"blue";
if(Contains(s," orange"))return"orange";
if(Contains(s," white"))return"white";
if(Contains(s," yellow"))return"yellow";
if(Contains(s," cyan"))return"cyan";
if(Contains(s," off"))return"off";
return"";
}
void EvaluateRules(){
var ordered=new List<ParsedRule>(_rules);
ordered.Sort((a,b)=>b.Priority.CompareTo(a.Priority));
foreach(var rule in ordered){
if(!rule.Valid)continue;
if(!string.IsNullOrEmpty(rule.Label)&&_mutedLabels.Contains(rule.Label))continue;
if(rule.ModeTags.Count>0&&!rule.ModeTags.Contains(_activeMode))continue;
bool condMet=EvaluateCondition(rule);
bool prevMet=rule.PrevCondMet;
rule.PrevCondMet=condMet;
bool shouldFireThen=false,shouldFireElse=false;
switch(rule.Mode){
case RuleMode.ContinuousIf:shouldFireThen=condMet;
shouldFireElse=!condMet&&rule.OtherwiseActions.Count>0;
break;
case RuleMode.RisingEdgeWhen:shouldFireThen=condMet&&!prevMet;
break;
case RuleMode.FallingEdgeWhen:shouldFireThen=!condMet&&prevMet;
break;
}
if(rule.EveryNTicks>0&&(_tick-rule.LastFireTick)<rule.EveryNTicks){
shouldFireThen=false;
shouldFireElse=false;
}
if(shouldFireThen){
_lastTriggered=TraceTag(rule);
rule.LastFireTick=_tick;
BumpRuleFireCount(rule);
ExecuteAction(rule,rule.Actions,condMet,prevMet);
}
else if(shouldFireElse){
_lastTriggered=TraceTag(rule)+" (else)";
rule.LastFireTick=_tick;
BumpRuleFireCount(rule);
ExecuteAction(rule,rule.OtherwiseActions,condMet,prevMet);
}
}
}
bool EvaluateCondition(ParsedRule rule){
bool a=EvalSingleCondition(rule.Condition,rule.CondValue,rule.CondTarget,rule.ItemName);
if(rule.Join==CondJoin.None)return a;
bool b=EvalSingleCondition(rule.Cond2,rule.Cond2Value,rule.Cond2Target,rule.Cond2Item);
return rule.Join==CondJoin.And?(a&&b):(a||b);
}
bool EvalSingleCondition(CondType cond,double value,string target,string item){
if(_simulated.Count>0){
if((cond==CondType.EnemyDetected)&&(_simulated.ContainsKey("enemy detected")||_simulated.ContainsKey("enemy")))return true;
if((cond==CondType.Dark)&&_simulated.ContainsKey("dark"))return true;
if((cond==CondType.Daylight)&&_simulated.ContainsKey("daylight"))return true;
if((cond==CondType.ConnectorDocked)&&_simulated.ContainsKey("docked"))return true;
}
switch(cond){
case CondType.EnemyDetected:return _enemyDetected;
case CondType.EnemyNotDetected:return!_enemyDetected;
case CondType.PowerBelow:return _powerPct<value;
case CondType.PowerAbove:return _powerPct>value;
case CondType.PowerFull:return _powerPct>=98.0;
case CondType.BatteryBelow:return _powerPct<value;
case CondType.NamedBatteryBelow:return NamedBatteryPct(target)<value;
case CondType.NamedBatteryAbove:return NamedBatteryPct(target)>value;
case CondType.Dark:return _solarAvailable&&_solarState==SolarState.Dark;
case CondType.Daylight:return _solarAvailable&&_solarState==SolarState.Daylight;
case CondType.SolarBelow:return _solarAvailable&&_solarPct<value;
case CondType.SolarAbove:return _solarAvailable&&_solarPct>value;
case CondType.ConnectorDocked:return _connectorDocked;
case CondType.ConnectorUndocked:return!_connectorDocked;
case CondType.NamedConnectorDocked:return NamedConnectorIsDocked(target,true);
case CondType.NamedConnectorUndocked:return NamedConnectorIsDocked(target,false);
case CondType.OxygenBelow:return _oxygenPct<value;
case CondType.OxygenLow:return _oxygenPct<25.0;
case CondType.HydrogenBelow:return _hydrogenPct<value;
case CondType.HydrogenLow:return _hydrogenPct<25.0;
case CondType.CargoAbove:return _cargoPct>value;
case CondType.CargoFull:return _cargoPct>=95.0;
case CondType.NamedCargoAbove:return NamedCargoPct(target)>value;
case CondType.ItemBelow:return CountInventoryItem(item)<(int)value;
case CondType.BlockIsOff:{
var blk=FindBlock(target);
return blk!=null&&!blk.IsWorking;
}
case CondType.BlockIsOn:{
var blk=FindBlock(target);
return blk!=null&&blk.IsWorking;
}
case CondType.BlockDamaged:return AnyBlockDamaged(target);
case CondType.BlockNotDamaged:return!AnyBlockDamaged(target);
case CondType.AirVentOff:return AnyAirVentMatches(target,false);
case CondType.AirVentOn:return AnyAirVentMatches(target,true);
case CondType.RefineryIdle:return AnyRefineryMatches(target,false);
case CondType.RefineryWorking:return AnyRefineryMatches(target,true);
case CondType.AssemblerIdle:return AnyAssemblerMatches(target,false);
case CondType.AssemblerWorking:return AnyAssemblerMatches(target,true);
case CondType.SpeedAbove:return _shipSpeed>value;
case CondType.SpeedBelow:return _shipSpeed<value;
case CondType.AltitudeAbove:return _shipAltitude>value;
case CondType.AltitudeBelow:return _shipAltitude<value&&_shipAltitude>0;
case CondType.PilotPresent:return _pilotPresent;
case CondType.PilotAbsent:return!_pilotPresent;
case CondType.GunShooting:return AnyGunShooting(target,true);
case CondType.GunNotShooting:return AnyGunShooting(target,false);
case CondType.ArgumentMatches:return!string.IsNullOrEmpty(_pendingArgument)&&_pendingArgument.Equals(target,StringComparison.OrdinalIgnoreCase);
case CondType.GameHourIs:return Math.Abs(_gameHour-value)<0.5;
case CondType.GameHourBefore:return _gameHour<value;
case CondType.GameHourAfter:return _gameHour>=value;
case CondType.NightTime:return _gameHour<6.0||_gameHour>=18.0;
case CondType.DayTime:return _gameHour>=6.0&&_gameHour<18.0;
case CondType.BlockCountBelow:return CountBlocksOfType(target)<value;
case CondType.BlockCountAbove:return CountBlocksOfType(target)>value;
case CondType.BlockCountIs:return Math.Abs(CountBlocksOfType(target)-value)<0.5;
case CondType.GunAmmoBelow:return GunAmmoCount(target)<value;
case CondType.InGravity:return _inGravity;
case CondType.InSpace:return!_inGravity;
case CondType.GyroOverriding:foreach(var g in _gyros)if(g.GyroOverride)return true;
return false;
case CondType.SensorTriggered:foreach(var s in _sensors)if(s!=null&&(string.IsNullOrEmpty(target)||NameMatch(s,target))&&s.IsActive)return true;
return false;
case CondType.VarAbove:return GetVar(target)>value;
case CondType.VarBelow:return GetVar(target)<value;
case CondType.VarIs:return Math.Abs(GetVar(target)-value)<0.5;
case CondType.Remembered:return GetVar(target)>0;
case CondType.Forgotten:return GetVar(target)<=0;
case CondType.IGCReceived:return _igcReceived.ContainsKey(target??"");
case CondType.CameraSees:return DoCameraScan(target,value,false);
case CondType.CameraSeesEnemy:return DoCameraScan(target,value,true);
case CondType.Always:return true;
}
return false;
}
double NamedBatteryPct(string quoted){
if(string.IsNullOrEmpty(quoted))return _powerPct;
var blocks=ResolveQuoted(quoted);
if(blocks.Count==0){
foreach(var b in _batteries)if(NameMatch(b,quoted)){
blocks.Add(b);
}
if(blocks.Count==0)return 0;
}
double stored=0,max=0;
foreach(var b in blocks){
var bat=b as IMyBatteryBlock;
if(bat==null)continue;
stored+=bat.CurrentStoredPower;
max+=bat.MaxStoredPower;
}
return max>0?stored/max*100.0:0;
}
bool NamedConnectorIsDocked(string quoted,bool wantDocked){
if(string.IsNullOrEmpty(quoted))return wantDocked==_connectorDocked;
var blocks=ResolveQuoted(quoted);
if(blocks.Count==0){
foreach(var c in _connectors)if(NameMatch(c,quoted))blocks.Add(c);
}
foreach(var b in blocks){
var c=b as IMyShipConnector;
if(c==null)continue;
bool docked=c.Status==MyShipConnectorStatus.Connected;
if(docked==wantDocked)return true;
}
return false;
}
double NamedCargoPct(string quoted){
if(string.IsNullOrEmpty(quoted))return _cargoPct;
var blocks=ResolveQuoted(quoted);
if(blocks.Count==0)foreach(var c in _cargo)if(NameMatch(c,quoted))blocks.Add(c);
double cur=0,max=0;
foreach(var b in blocks){
var c=b as IMyCargoContainer;
if(c==null)continue;
var inv=c.GetInventory();
cur+=(double)inv.CurrentVolume;
max+=(double)inv.MaxVolume;
}
return max>0?cur/max*100.0:0;
}
string ExtractBlockTypeKeyword(string s){
string[][]map={
new[]{
"reactor","reactor"}
,new[]{
"batter","battery"}
,new[]{
"turret","turret"}
,new[]{
"thruster","thruster"}
,new[]{
"light","light"}
,new[]{
"assembler","assembler"}
,new[]{
"refinery","refinery"}
,new[]{
"air vent","airvent"}
,new[]{
"connector","connector"}
,new[]{
"solar","solar"}
,new[]{
"piston","piston"}
,new[]{
"rotor","rotor"}
,new[]{
"hinge","rotor"}
,new[]{
"timer","timer"}
,new[]{
"sorter","sorter"}
,new[]{
"antenna","antenna"}
,new[]{
"beacon","beacon"}
,new[]{
"camera","camera"}
,new[]{
"gear","gear"}
,new[]{
"door","door"}
,new[]{
"gun","turret"}
,}
;
foreach(var pair in map)if(Contains(s,pair[0]))return pair[1];
return"";
}
int CountBlocksOfType(string type){
if(string.IsNullOrEmpty(type))return 0;
int n=0;
switch(type){
case"reactor":foreach(var b in _reactors)if(b.IsWorking)n++;
break;
case"battery":foreach(var b in _batteries)if(b.IsWorking)n++;
break;
case"turret":foreach(var b in _turrets)if(b.IsWorking)n++;
break;
case"thruster":foreach(var b in _thrusters)if(b.IsWorking)n++;
break;
case"light":foreach(var b in _lights)if(b.IsWorking)n++;
break;
case"assembler":foreach(var b in _assemblers)if(b.IsWorking)n++;
break;
case"refinery":foreach(var b in _refineries)if(b.IsWorking)n++;
break;
case"airvent":foreach(var b in _airVents)if(b.IsWorking)n++;
break;
case"connector":foreach(var b in _connectors)if(b.IsWorking)n++;
break;
case"solar":foreach(var b in _solar)if(b.IsWorking)n++;
break;
case"piston":foreach(var b in _pistons)if(b.IsWorking)n++;
break;
case"rotor":foreach(var b in _rotors)if(b.IsWorking)n++;
break;
case"timer":foreach(var b in _timers)if(b.IsWorking)n++;
break;
case"sorter":foreach(var b in _sorters)if(b.IsWorking)n++;
break;
case"antenna":foreach(var b in _antennas)if(b.IsWorking)n++;
break;
case"beacon":foreach(var b in _beacons)if(b.IsWorking)n++;
break;
case"camera":foreach(var b in _cameras)if(b.IsWorking)n++;
break;
case"gear":foreach(var b in _gears)if(b.IsWorking)n++;
break;
case"door":foreach(var b in _doors)if(b.IsWorking)n++;
break;
}
return n;
}
int GunAmmoCount(string quoted){
var items=new List<MyInventoryItem>();
int total=0;
foreach(var g in _allGuns){
var tb=g as IMyTerminalBlock;
if(tb==null)continue;
if(!string.IsNullOrEmpty(quoted)&&!NameMatch(tb,quoted))continue;
for(int i=0;
i<tb.InventoryCount;
i++){
items.Clear();
tb.GetInventory(i).GetItems(items);
foreach(var it in items)if(it.Type.TypeId.ToString().Contains("AmmoMagazine"))total+=(int)(double)it.Amount;
}
}
return total;
}
string ExtractVarName(string s,string keyword){
int idx=s.IndexOf(keyword,StringComparison.OrdinalIgnoreCase);
if(idx<0)return"";
int start=idx+keyword.Length;
while(start<s.Length&&s[start]==' ')start++;
string[]stops={
" above "," below "," is "," more than "," less than "," under "," over "," equals "}
;
int stop=s.Length;
foreach(var w in stops){
int k=s.IndexOf(w,start,StringComparison.OrdinalIgnoreCase);
if(k>=0&&k<stop)stop=k;
}
string name=s.Substring(start,stop-start).Trim();
if(name.Length>=2&&name[0]=='"'&&name[name.Length-1]=='"')name=name.Substring(1,name.Length-2);
return name;
}
double GetVar(string name){
if(string.IsNullOrEmpty(name))return 0;
double v;
_vars.TryGetValue(name,out v);
return v;
}
void SetVar(string name,double v){
if(string.IsNullOrEmpty(name))return;
_vars[name]=v;
}
void EnsureIGCListener(string tag){
if(string.IsNullOrEmpty(tag)||_igcListeners.ContainsKey(tag))return;
var listener=IGC.RegisterBroadcastListener(tag);
listener.SetMessageCallback("");
_igcListeners[tag]=listener;
}
void DrainIGCMessages(){
if(_igcListeners.Count==0){
if(_igcReceived.Count>0)_igcReceived.Clear();
return;
}
_igcReceived.Clear();
foreach(var kv in _igcListeners){
var listener=kv.Value;
while(listener.HasPendingMessage){
var msg=listener.AcceptMessage();
_igcReceived[kv.Key]=msg.Data==null?"":msg.Data.ToString();
}
}
}
bool DoCameraScan(string quoted,double distance,bool wantEnemyOnly){
IMyCameraBlock cam=null;
if(!string.IsNullOrEmpty(quoted)){
foreach(var c in _cameras)if(c.CustomName.Equals(quoted,StringComparison.OrdinalIgnoreCase)||NameMatch(c,quoted)){
cam=c;
break;
}
}
else if(_cameras.Count>0)cam=_cameras[0];
if(cam==null)return false;
cam.EnableRaycast=true;
if(!cam.CanScan(distance))return false;
var hit=cam.Raycast(distance);
if(hit.IsEmpty())return false;
if(!wantEnemyOnly)return true;
var rel=hit.Relationship;
return rel==MyRelationsBetweenPlayerAndBlock.Enemies||rel==MyRelationsBetweenPlayerAndBlock.Neutral||rel==MyRelationsBetweenPlayerAndBlock.NoOwnership;
}
const string VAR_HEADER="# MPX VARS (auto-saved)";
void LoadVars(){
_vars.Clear();
string data=Me.CustomData??"";
if(!data.StartsWith(VAR_HEADER)){
_varsSnapshot=BuildVarText();
return;
}
foreach(var rawLine in data.Split('\n')){
var line=rawLine.Trim();
if(line.Length==0||line.StartsWith("#"))continue;
int eq=line.IndexOf('=');
if(eq<=0)continue;
string k=line.Substring(0,eq).Trim();
double v;
if(double.TryParse(line.Substring(eq+1).Trim(),out v))_vars[k]=v;
}
_varsSnapshot=BuildVarText();
}
string BuildVarText(){
if(_vars.Count==0)return"";
var sb=new StringBuilder();
sb.AppendLine(VAR_HEADER);
foreach(var kv in _vars)sb.AppendLine(kv.Key+"="+kv.Value);
return sb.ToString();
}
void SaveVarsIfDirty(){
string text=BuildVarText();
if(text==_varsSnapshot)return;
_varsSnapshot=text;
Me.CustomData=text;
}
bool AnyGunShooting(string quoted,bool wantShooting){
List<IMyTerminalBlock>blocks;
if(!string.IsNullOrEmpty(quoted)){
blocks=ResolveQuoted(quoted);
if(blocks.Count==0){
blocks=new List<IMyTerminalBlock>();
foreach(var g in _allGuns){
var tb=g as IMyTerminalBlock;
if(tb!=null&&NameMatch(tb,quoted))blocks.Add(tb);
}
}
foreach(var b in blocks){
var g=b as IMyUserControllableGun;
if(g==null)continue;
bool firing=g.IsShooting||g.Shoot;
if(firing==wantShooting)return true;
}
return false;
}
foreach(var g in _allGuns){
bool firing=g.IsShooting||g.Shoot;
if(firing==wantShooting)return true;
}
return false;
}
bool AnyBlockDamaged(string quoted){
List<IMyTerminalBlock>blocks;
if(!string.IsNullOrEmpty(quoted)){
blocks=ResolveQuoted(quoted);
if(blocks.Count==0){
blocks=new List<IMyTerminalBlock>();
foreach(var b in _allBlocks)if(NameMatch(b,quoted))blocks.Add(b);
}
}
else blocks=_allBlocks;
foreach(var b in blocks){
var slim=b.CubeGrid.GetCubeBlock(b.Position);
if(slim==null)continue;
if(slim.BuildIntegrity<slim.MaxIntegrity)return true;
}
return false;
}
bool AnyRefineryMatches(string quoted,bool wantProducing){
if(!string.IsNullOrEmpty(quoted)){
var blocks=ResolveQuoted(quoted);
if(blocks.Count==0)return false;
foreach(var b in blocks){
var r=b as IMyRefinery;
if(r==null)continue;
if(r.IsProducing==wantProducing)return true;
}
return false;
}
if(_refineries.Count==0)return false;
foreach(var r in _refineries)if(r.IsProducing==wantProducing)return true;
return false;
}
bool AnyAssemblerMatches(string quoted,bool wantProducing){
if(!string.IsNullOrEmpty(quoted)){
var blocks=ResolveQuoted(quoted);
if(blocks.Count==0)return false;
foreach(var b in blocks){
var a=b as IMyAssembler;
if(a==null)continue;
if(a.IsProducing==wantProducing)return true;
}
return false;
}
if(_assemblers.Count==0)return false;
foreach(var a in _assemblers)if(a.IsProducing==wantProducing)return true;
return false;
}
bool AnyAirVentMatches(string quoted,bool wantOn){
if(!string.IsNullOrEmpty(quoted)){
var blocks=ResolveQuoted(quoted);
if(blocks.Count==0)return false;
foreach(var b in blocks){
var v=b as IMyAirVent;
if(v==null)continue;
bool isOn=v.Enabled&&v.IsWorking;
if(isOn==wantOn)return true;
}
return false;
}
foreach(var v in _airVents){
bool isOn=v.Enabled&&v.IsWorking;
if(isOn==wantOn)return true;
}
return false;
}
void ExecuteAction(ParsedRule rule,List<ActionPart>actions,bool condMet,bool prevMet){
foreach(var part in actions)ExecuteActionPart(rule,part,condMet,prevMet);
}
void ExecuteActionPart(ParsedRule rule,ActionPart part,bool condMet,bool prevMet){
if(rule.Mode==RuleMode.ContinuousIf&&part.Action==ActType.PlaySoundBlocks&&part.DurationSec>0)if(!(condMet&&!prevMet))return;
if(_dryRun){
AddTrace("DRY: "+TraceTag(rule)+" -> "+part.Action+(string.IsNullOrEmpty(part.ActTarget)?"":" "+part.ActTarget));
return;
}
string cooldownKey=part.Action+"|"+part.ActTarget+"|"+part.ActParam+"|"+part.ItemName;
if(_actionCooldowns.ContainsKey(cooldownKey))return;
bool acted=false;
switch(part.Action){
case ActType.VarCount:if(rule.Mode==RuleMode.ContinuousIf&&!(condMet&&!prevMet))return;
SetVar(part.ActParam,GetVar(part.ActParam)+1);
acted=true;
AddTrace(TraceTag(rule)+" count "+part.ActParam+"="+GetVar(part.ActParam));
break;
case ActType.VarAdd:if(rule.Mode==RuleMode.ContinuousIf&&!(condMet&&!prevMet))return;
SetVar(part.ActParam,GetVar(part.ActParam)+part.NumParam);
acted=true;
break;
case ActType.VarSubtract:if(rule.Mode==RuleMode.ContinuousIf&&!(condMet&&!prevMet))return;
SetVar(part.ActParam,GetVar(part.ActParam)-part.NumParam);
acted=true;
break;
case ActType.VarSet:SetVar(part.ActParam,part.NumParam);
acted=true;
break;
case ActType.VarReset:SetVar(part.ActParam,0);
acted=true;
break;
case ActType.VarRemember:SetVar(part.ActParam,1);
acted=true;
break;
case ActType.VarForget:SetVar(part.ActParam,0);
acted=true;
break;
case ActType.IGCBroadcast:if(rule.Mode==RuleMode.ContinuousIf&&!(condMet&&!prevMet))return;
if(!string.IsNullOrEmpty(part.ActTarget)){
string channel=string.IsNullOrEmpty(part.SecondTarget)?"MPX":part.SecondTarget;
IGC.SendBroadcastMessage(channel,part.ActTarget);
AddTrace(TraceTag(rule)+" IGC "+channel);
acted=true;
}
break;
case ActType.CameraScanOn:foreach(var cam in ResolveTargetList(part.ActTarget,_cameras,b=>(IMyCameraBlock)b)){
if(cam!=null){
cam.EnableRaycast=true;
acted=true;
}
}
break;
case ActType.CameraScanOff:foreach(var cam in ResolveTargetList(part.ActTarget,_cameras,b=>(IMyCameraBlock)b)){
if(cam!=null){
cam.EnableRaycast=false;
acted=true;
}
}
break;
default:DispatchAction(rule,part,condMet,prevMet);
acted=true;
break;
}
if(acted){
_actionCooldowns[cooldownKey]=ACTION_COOLDOWN_TICKS;
_lastActions=part.Action+(string.IsNullOrEmpty(part.ActTarget)?"":" -> "+part.ActTarget);
}
}
void DispatchAction(ParsedRule rule,ActionPart part,bool condMet,bool prevMet){
string raw=rule.Raw??"";
if(raw.Length>160)raw=raw.Substring(0,160);
string data="1|"+((int)part.Action)+"|"+Enc(part.ActTarget)+"|"+Enc(part.ActParam)+"|"+Enc(part.ItemName)+"|"+part.DurationSec+"|"+part.NumParam+"|"+Enc(part.SecondTarget)+"|"+Enc(part.RawLower)+"|"+((int)rule.Mode)+"|"+rule.CondValue+"|"+Enc(rule.ItemName)+"|"+Enc(rule.Label)+"|"+Enc(raw)+"|"+(condMet?"1":"0")+"|"+(prevMet?"1":"0");
IGC.SendBroadcastMessage(ACTION_TAG,data,ACTION_DISTANCE);
_sentActions++;
_lastDispatch=part.Action.ToString();
AddTrace("TX "+TraceTag(rule)+" -> "+part.Action);
Log("TX: "+part.Action+(string.IsNullOrEmpty(part.ActTarget)?"":" -> "+part.ActTarget));
}
string Enc(string s){
if(s==null)return"";
return s.Replace("\\","\\\\").Replace("|","\\p").Replace("\r","\\r").Replace("\n","\\n");
}
List<T>ResolveTargetList<T>(string quoted,List<T>all,Func<IMyTerminalBlock,T>cast)where T:class{
if(string.IsNullOrEmpty(quoted))return all;
var result=new List<T>();
foreach(var b in ResolveQuoted(quoted)){
var x=cast(b);
if(x!=null)result.Add(x);
}
if(result.Count==0)foreach(var b in all){
var asTerm=b as IMyTerminalBlock;
if(asTerm!=null&&NameMatch(asTerm,quoted))result.Add(b);
}
return result;
}
void BumpRuleFireCount(ParsedRule rule){
string key=!string.IsNullOrEmpty(rule.Label)?rule.Label:(rule.Raw.Length>36?rule.Raw.Substring(0,36):rule.Raw);
int n;
_ruleFireCount.TryGetValue(key,out n);
_ruleFireCount[key]=n+1;
}
string TraceTag(ParsedRule r){
if(r==null)return"?";
if(!string.IsNullOrEmpty(r.Label))return"["+r.Label+"]";
return TruncRule(r);
}
void AddTrace(string entry){
_actionTrace.Add(entry);
while(_actionTrace.Count>ACTION_TRACE_MAX)_actionTrace.RemoveAt(0);
}
string TruncRule(ParsedRule r){
if(r==null)return"?";
string raw=r.Raw??"";
return raw.Length>36?raw.Substring(0,33)+"...":raw;
}
string GetSubtype(IMyTerminalBlock b){
if(b==null)return"";
string s=null;
try{
s=b.BlockDefinition.SubtypeId.ToString();
}
catch{
s=null;
}
if(string.IsNullOrEmpty(s)){
try{
s=b.DefinitionDisplayNameText;
}
catch{
s=null;
}
}
return s??"";
}
bool IsHydrogenTank(IMyGasTank t){
if(t==null)return false;
string sub=GetSubtype(t);
string name=t.CustomName??"";
return sub.IndexOf("Hydrogen",StringComparison.OrdinalIgnoreCase)>=0||sub.IndexOf("H2",StringComparison.OrdinalIgnoreCase)>=0||name.IndexOf("Hydrogen",StringComparison.OrdinalIgnoreCase)>=0||name.IndexOf("H2",StringComparison.OrdinalIgnoreCase)>=0;
}
bool IsOxygenTank(IMyGasTank t){
if(t==null)return false;
string sub=GetSubtype(t);
string name=t.CustomName??"";
if(sub.IndexOf("Hydrogen",StringComparison.OrdinalIgnoreCase)>=0)return false;
return sub.IndexOf("Oxygen",StringComparison.OrdinalIgnoreCase)>=0||name.IndexOf("Oxygen",StringComparison.OrdinalIgnoreCase)>=0||name.IndexOf("O2",StringComparison.OrdinalIgnoreCase)>=0;
}
List<IMyTerminalBlock>ResolveQuoted(string quoted){
var result=new List<IMyTerminalBlock>();
if(string.IsNullOrEmpty(quoted))return result;
var grp=GridTerminalSystem.GetBlockGroupWithName(quoted);
if(grp!=null){
grp.GetBlocks(result);
if(result.Count>0)return result;
}
var blk=GridTerminalSystem.GetBlockWithName(quoted);
if(blk!=null)result.Add(blk);
return result;
}
bool NameMatch(IMyTerminalBlock b,string name){
return b!=null&&!string.IsNullOrEmpty(name)&&b.CustomName.IndexOf(name,StringComparison.OrdinalIgnoreCase)>=0;
}
IMyTerminalBlock FindBlock(string name){
if(string.IsNullOrEmpty(name))return null;
foreach(var b in _allBlocks)if(b.CustomName.Equals(name,StringComparison.OrdinalIgnoreCase))return b;
return null;
}
void RebuildInvCache(){
if(_invCacheTick==_tick)return;
_invCacheTick=_tick;
_invCache.Clear();
var items=new List<MyInventoryItem>();
foreach(var b in _allBlocks){
for(int i=0;
i<b.InventoryCount;
i++){
items.Clear();
b.GetInventory(i).GetItems(items);
foreach(var item in items){
int cur;
string typeShort=item.Type.TypeId.Replace("MyObjectBuilder_","");
string key=typeShort+"/"+item.Type.SubtypeId;
_invCache.TryGetValue(key,out cur);
_invCache[key]=cur+(int)(double)item.Amount;
}
}
}
}
int CountInventoryItem(string key){
RebuildInvCache();
if(string.IsNullOrEmpty(key))return 0;
int result;
if(key.IndexOf('/')>=0){
_invCache.TryGetValue(key,out result);
return result;
}
_invCache.TryGetValue("Component/"+key,out result);
return result;
}
static readonly char[]SPINNER={
'|','/','-','\\'}
;
void DrawCoreLCD(){
if(_lcdCore==null)return;
var sb=new StringBuilder();
sb.AppendLine("MPX SCRIPT BRAIN "+(_paused?"PAUSED":"ONLINE")+" "+SPINNER[_spinnerIdx]);
sb.AppendLine("Mode: "+(string.IsNullOrEmpty(_activeMode)?"default":_activeMode)+" Rules: "+_rules.Count+" Sent: "+_sentActions);
sb.AppendLine("Enemy: "+(_enemyDetected?"YES "+_enemySource:"NO")+" Docked: "+(_connectorDocked?"YES":"NO"));
sb.AppendLine("Power "+_powerPct.ToString("0")+"% H2 "+_hydrogenPct.ToString("0")+"% O2 "+_oxygenPct.ToString("0")+"% Cargo "+_cargoPct.ToString("0")+"%");
sb.AppendLine("Solar "+_solarState+" best "+_solarBestPct.ToString("0")+"% avg "+_solarAvgPct.ToString("0")+"% source "+_solarSource);
sb.AppendLine("Last rule: "+_lastTriggered);
sb.AppendLine("Last action: "+_lastActions);
if(!string.IsNullOrEmpty(_lastUnderstood))sb.AppendLine("Parsed: "+_lastUnderstood);
if(_warnings.Count>0){
sb.AppendLine("Warnings:");
int n=0;
foreach(var w in _warnings){
if(n++>=4)break;
sb.AppendLine("! "+w);
}
}
if(_actionTrace.Count>0){
sb.AppendLine("Trace:");
for(int i=_actionTrace.Count-1;
i>=0;
i--)sb.AppendLine(_actionTrace[i]);
}
if(_vars.Count>0){
sb.AppendLine("Vars:");
int n=0;
foreach(var kv in _vars){
if(n++>=4)break;
sb.AppendLine(kv.Key+"="+kv.Value);
}
}
_lcdCore.WriteText(sb.ToString());
}
void DrawDisplayLCD(){
if(_lcdDisplay==null)return;
var sb=new StringBuilder();
sb.AppendLine("MPX TELEMETRY "+SPINNER[(_spinnerIdx+2)%4]);
sb.AppendLine(BuildBar("PWR",_powerPct));
sb.AppendLine(BuildBar("H2 ",_hydrogenPct));
sb.AppendLine(BuildBar("O2 ",_oxygenPct));
sb.AppendLine(BuildBar("CGO",_cargoPct));
sb.AppendLine(BuildBar("SOL",_solarPct));
sb.AppendLine("Enemy: "+(_enemyDetected?"YES":"NO")+" Light: "+(_solarAvailable?_solarState.ToString():"Unknown")+" Docked: "+(_connectorDocked?"YES":"NO"));
sb.AppendLine("Speed: "+_shipSpeed.ToString("0.0")+" m/s Alt: "+(_shipAltitude>0?_shipAltitude.ToString("0")+" m":"n/a")+" Pilot: "+(_pilotPresent?"YES":"NO"));
sb.AppendLine("Blocks R"+_reactors.Count+" B"+_batteries.Count+" T"+_turrets.Count+" L"+_lights.Count+" D"+_doors.Count+" A"+_assemblers.Count);
_lcdDisplay.WriteText(sb.ToString());
}
string BuildBar(string label,double pct){
pct=Math.Max(0,Math.Min(100,pct));
int filled=(int)(pct/5.0);
var bar=new StringBuilder();
bar.Append("  "+label+" [");
for(int i=0;
i<20;
i++)bar.Append(i<filled?'#':'.');
bar.Append("] "+pct.ToString("0.0")+"%");
return bar.ToString();
}
void Log(string msg){
string entry=$"[{_tick,6}] {msg}";
_logLines.Add(entry);
if(_logLines.Count>MAX_LOG_LINES)_logLines.RemoveAt(0);
if(_lcdLog!=null){
var sb=new StringBuilder();
sb.AppendLine("=== MPX LOG ===");
for(int i=_logLines.Count-1;
i>=0;
i--)sb.AppendLine(_logLines[i]);
_lcdLog.WriteText(sb.ToString());
}
Echo(entry);
}
