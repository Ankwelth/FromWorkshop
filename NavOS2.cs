// NavOS v2.16 brought to you by StarCpt
// 
// Config (In the customdata of the pb - Must recompile when modified!!):
// # PersistStateData: DO NOT TOUCH
// 
// # MaxThrustOverrideRatio: Amount of forward thrust to use. 0 = 0%, 0.5 = 50%, 1 = 100%
// Important: In gravity forward override ratio may exceed this value.
// 
// # IgnoreMaxThrustForSpeedMatch: Ignore the above config if using SpeedMatch if true.
// 
// # ShipControllerTag: Put this tag in the name of the controller you want to use for orientation
// 
// # ThrustGroupName: Only use thrusters in this group for navigation. Uses all thrusters on the ship if the group doesn't exist
// 
// # GyroGroupName: Same as above but for gyros. Uses the first gyro in the group.
// 
// # ConsoleLcdName: Name of the LCD you want some navigation information to be written to
// 
// # CruiseOffsetDist: For GPS and X:Y:Z cruise commands, come to a stop this many meters earlier than the target
// # CruiseOffsetSideDist: For GPS and X:Y:Z cruise commands, offset the target by this many meters to the side (side = a random perpendicular direction to the target from your current position)
// 
// # Ship180TurnTimeSeconds: How long it takes the ship to do a 180 degree turn in seconds.
// You need to set this to equal to or more than the actual 180 turn time. It determines when to turn for deceleration in case it can't get up to full cruise speed.
// This field is not calculated automatically, run CalibrateTurn to calculate this field. Recalibration is required whenever the mass of the ship changes significantly.
// 
// # MaintainDesiredSpeed: Keeps the ship oriented to the target during cruise and maintains the desired speed if possible (resists RTS friction) until deceleration time.
// 
// # [Journey Start] and [Journey End] (Experimental!)
// This is where journey data is input. Waypoint Format: <DesiredSpeed> <StopAtWaypoint> <GPS>
// StopAtWaypoint: Whether to stop at the destination or pass thru the point at speed
// Example:
// [Journey Start]
// 200 true GPS:StarCpt #2:-17787:48220:-29137:
// 550 false GPS:StarCpt #3:-17933:48686:-29105:#FF75C9F1:
// 150 true GPS:StarCpt #4:-17995:48270:-29421:#FF75C9F1:
// [Journey End]
// 
// Commands:
// # Cruise <DesiredSpeed> <ForwardDistanceMeters>
// Automatically travels the set distance in the forward direction of the ship controller (cockpit) at the desired speed.
// Example: Cruise 500 20000
// 
// # Cruise <DesiredSpeed> <GPS> <Optional:UseCruiseOffset>
// Same as above for GPSes
// If UseCruiseOffset is unset, cruise offsets are enabled by default
// Example 1: Cruise 500 GPS:target:15000:70000:17000:
// Example 2: Cruise 500 GPS:target:15000:70000:17000: false - won't use cruise offsets
// 
// # Autopilot <DesiredSpeed> <ForwardDistanceMeters>
// # Autopilot <DesiredSpeed> <GPS> <Optional:UseCruiseOffset>
// Travels to the target gps (plus configured offsets) or specified forward distance using omnidirectional thrust without turning the ship.
// 
// # Approach <DesiredSpeed>
// (WeaponCore only) Approaches the locked target and comes to a stop just outside its bounding box.
// IMPORTANT: This uses the position of the target at the time of the command. Use at your own risk if the target is moving.
// 
// # Retro/Retrograde
// Points the ship in the opposite direction of travel
// 
// # Prograde
// Points the ship in the direction of travel
// 
// # RadialIn
// Orient toward natural gravity
// 
// # RadialOut
// Orient away from natural gravity
// 
// # Retroburn
// Points the ship to retrograde and stops the ship
// 
// # Match
// Matches speed to CURRENT WeaponCore lock, stays on that target even if locked target changes.
// Rerun the command to change speed match target to current WeaponCore lock.
// 
// # Orient <GPS>
// Orients the ship to point in the direction of the gps with no thruster controls applied.
// 
// # Abort
// Aborts any ongoing navigation routines. Disables all gyro and thrust overrides and reloads the config. Does not enable dampeners
// 
// # ThrustRatio <ratio>
// # MaxThrustOverrideRatio <ratio>
// Sets the MaxThrustOverrideRatio config value. Input a value between 0 and 1. This is an alternative to editing the config.
// Using either ThrustRatio or MaxthrustOverrideRatio commands will do the same thing.
// Example: ThrustRatio 0.6
// 
// # Thrust Set <ratio>
// Sets thrust override ratio on forward thrusters. Example command will set forward thrusters on 50% override.
// Example: Thrust Set 0.5
// 
// # CalibrateTurn
// Measure and set the Ship180TurnTimeSeconds variable in the config. Make sure the ship is not moving or rotating when this command is used.
// 
// # Journey Load (Experimental!)
// Loads the journey setup from config
// 
// # Journey Start (Experimental!)
// Starts the journey if waypoints are loaded

//Config is in the CustomData

//lcd for logging
const string debugLcdName = "debugLcd";
const double throttleRt = 0.1;
const int printInterval = 10;

public NavModeEnum NavMode{get{return _navMode;}set{if(_navMode!=value){var oldValue=_navMode;_navMode=value;
NavModeChanged(oldValue,value);}}}ICruiseController CruiseController{get{return _cruiseController;}set{if(_cruiseController!=value){
var oldValue=_cruiseController;var newValue=value;if(oldValue!=null){oldValue.CruiseTerminated-=OnCruiseTerminated;}
_cruiseController=newValue;if(newValue!=null){newValue.CruiseTerminated+=OnCruiseTerminated;}}}}NavModeEnum _navMode=NavModeEnum.Idle;
ICruiseController _cruiseController=null;Dictionary<Direction,List<IMyThrust>>thrusters=new Dictionary<Direction,List<IMyThrust>>{{
Direction.Forward,new List<IMyThrust>()},{Direction.Backward,new List<IMyThrust>()},{Direction.Right,new List<IMyThrust>()},{
Direction.Left,new List<IMyThrust>()},{Direction.Up,new List<IMyThrust>()},{Direction.Down,new List<IMyThrust>()},};List<IMyGyro>
gyros=new List<IMyGyro>();IMyShipController controller;private static readonly StringBuilder debug=new StringBuilder();
IMyTextSurface debugLcd;IMyTextSurface consoleLcd;public static int counter=-1;int idleCounter=0;IAimController aimController;public
static Profiler profiler;WcPbApi wcApi;bool wcApiActive=false;VariableThrustController thrustController;DateTime bootTime;
public const string programName="NavOS";public const string versionStr="2.16";public Config config;public
 Program
(){InitCommands();LoadConfig(false);UpdateBlocks();Runtime.UpdateFrequency=UpdateFrequency.Update1;bootTime=DateTime.
UtcNow;aimController=new JitAim(Me.CubeGrid.GridSizeEnum);profiler=new Profiler(this);wcApi=new WcPbApi();thrustController=new
VariableThrustController(thrusters,controller);try{wcApiActive=wcApi.Activate(Me);}catch{wcApiActive=false;}thrustController.UpdateThrusts();
TryRestoreNavState();}void TryRestoreNavState(){if(String.IsNullOrWhiteSpace(config.PersistStateData))return;string[]args=config.
PersistStateData.Split('|');NavModeEnum mode;if(args.Length==0||!Enum.TryParse<NavModeEnum>(args[0],out mode)||mode==NavModeEnum.Idle)
return;AbortNav(false);try{string stateStr=null;if(mode==NavModeEnum.Cruise&&args.Length>=2){double desiredSpeed;Vector3D
target;RetroCruiseControl.CruiseStage stage=RetroCruiseControl.CruiseStage.None;if(double.TryParse(args[1],out desiredSpeed)&&
Vector3D.TryParse(Storage,out target)&&(args.Length<3||Enum.TryParse(args[2],out stage))){InitRetroCruise(target,desiredSpeed,
stage,false);stateStr=mode+" "+desiredSpeed;}else stateStr=null;}if(mode==NavModeEnum.SpeedMatch&&args.Length>=2){long
targetId;if(long.TryParse(args[1],out targetId)){InitSpeedMatch(targetId);stateStr=mode+" "+targetId;}else stateStr=null;}else
if(mode==NavModeEnum.Retrograde){CommandRetrograde();stateStr=mode.ToString();}else if(mode==NavModeEnum.Retroburn){
CommandRetroburn();stateStr=mode.ToString();}else if(mode==NavModeEnum.Prograde){CommandPrograde();stateStr=mode.ToString();}else if(
mode==NavModeEnum.Orient){Vector3D target;if(Vector3D.TryParse(Storage,out target)){InitOrient(target);stateStr=mode.
ToString();}else stateStr=null;}else if(mode==NavModeEnum.Journey&&args.Length>=2){int step;if(int.TryParse(args[1],out step)){
thrustController.MaxForwardThrustRatio=(float)config.MaxThrustOverrideRatio;NavMode=NavModeEnum.Journey;CruiseController=new Journey(
aimController,controller,gyros,config.Ship180TurnTimeSeconds*1.5,thrustController,this);((Journey)CruiseController).InitStep(step);
stateStr=mode.ToString();}}else if(mode==NavModeEnum.Autopilot&&args.Length>=2){double desiredSpeed;Vector3D target;if(double.
TryParse(args[1],out desiredSpeed)&&Vector3D.TryParse(Storage,out target)){InitAutopilot(target,desiredSpeed,false);stateStr=
mode+" "+desiredSpeed;}else{stateStr=null;}}else if(mode==NavModeEnum.RadialIn){CommandRadialIn();stateStr=mode.ToString();}
else if(mode==NavModeEnum.RadialOut){CommandRadialOut();stateStr=mode.ToString();}if(stateStr==null)optionalInfo=
$"Failed to restore {mode}";else optionalInfo=$"Restored State: {stateStr}";}catch(Exception e){config.PersistStateData="";SaveConfig(false);
optionalInfo=e.ToString();}}void SaveConfig(bool updateblocks=true){Me.CustomData=config.ToString();if(updateblocks){UpdateBlocks();
}}void LoadConfig(bool updateBlocks){if(!Config.TryParse(Me.CustomData,out config)){config=Config.Default;}SaveConfig(
updateBlocks);}public void
 Main
(string argument,UpdateType updateSource){profiler.Run();counter++;if(argument.Length>0){HandleArgs(argument);}debugLcd?.
WriteText(debug.ToString());if(_navMode==NavModeEnum.Idle){idleCounter++;}else if(_cruiseController!=null){_cruiseController?.Run
();}if(idleCounter>=600){NavMode=NavModeEnum.Sleep;}if(_navMode==NavModeEnum.Sleep||counter%(profiler.RunningAverageMs>
throttleRt?60:printInterval)==0){WritePbOutput();}}void AbortNav(bool saveconfig=true){CruiseController?.Abort();thrustController.
ResetThrustOverrides();DisableGyroOverrides();NavMode=NavModeEnum.Idle;CruiseController=null;if(saveconfig){config.PersistStateData="";
SaveConfig();}}void OnCruiseTerminated(ICruiseController source,string reason){optionalInfo=
$"{source.Name} Terminated.\nReason: {reason}";NavMode=NavModeEnum.Idle;CruiseController=null;LoadConfig(false);config.PersistStateData="";Storage="";SaveConfig();}
void NavModeChanged(NavModeEnum old,NavModeEnum now){idleCounter=0;if(now==NavModeEnum.Sleep){Runtime.UpdateFrequency=
UpdateFrequency.None;}else if(old==NavModeEnum.Sleep){Runtime.UpdateFrequency=UpdateFrequency.Update1;}}void DisableGyroOverrides(){
foreach(var gyro in gyros){gyro.GyroOverride=false;gyro.Pitch=0;gyro.Yaw=0;gyro.Roll=0;}}void UpdateBlocks(){foreach(var list
in thrusters.Values){list.Clear();}var blocks=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(blocks,i=>i.
CubeGrid==Me.CubeGrid);var controllers=blocks.OfType<IMyShipController>().Where(b=>b.CustomName.Contains(config.
ShipControllerTag)).ToList();if(controllers.Count==0)throw new Exception($"No cockpit with \"{config.ShipControllerTag}\" found!");else
controller=controllers[0];var tempThrusters=new List<IMyThrust>();GridTerminalSystem.GetBlockGroupWithName(config.ThrustGroupName)
?.GetBlocksOfType(tempThrusters,i=>i.CubeGrid==Me.CubeGrid);if(tempThrusters.Count==0)GridTerminalSystem.GetBlocksOfType(
tempThrusters,i=>i.CubeGrid==Me.CubeGrid);if(tempThrusters.Count==0)throw new Exception("bruh, this ship's got no thrusters!!");
foreach(var thruster in tempThrusters){switch(GetBlockDirection(thruster.WorldMatrix.Forward,controller.WorldMatrix)){case
Direction.Backward:thrusters[Direction.Forward].Add(thruster);break;case Direction.Forward:thrusters[Direction.Backward].Add(
thruster);break;case Direction.Left:thrusters[Direction.Right].Add(thruster);break;case Direction.Right:thrusters[Direction.Left
].Add(thruster);break;case Direction.Down:thrusters[Direction.Up].Add(thruster);break;case Direction.Up:thrusters[
Direction.Down].Add(thruster);break;}}GridTerminalSystem.GetBlockGroupWithName(config.GyroGroupName)?.GetBlocksOfType(gyros,i=>i.
CubeGrid==Me.CubeGrid&&i.IsFunctional);if(gyros.Count==0)GridTerminalSystem.GetBlocksOfType(gyros,i=>i.CubeGrid==Me.CubeGrid&&i.
IsFunctional);if(gyros.Count==0)throw new Exception("No gyros");debugLcd=TryGetBlockWithName<IMyTextSurfaceProvider>(debugLcdName)?.
GetSurface(0);consoleLcd=TryGetBlockWithName<IMyTextSurfaceProvider>(config.ConsoleLcdName)?.GetSurface(0);}T TryGetBlockWithName<
T>(string name)where T:class{IMyTerminalBlock block=GridTerminalSystem.GetBlockWithName(name);return block is T?(T)block:
default(T);}public static Direction GetBlockDirection(Vector3D vector,MatrixD refMatrix){if(vector==refMatrix.Forward)return
Direction.Forward;if(vector==refMatrix.Backward)return Direction.Backward;if(vector==refMatrix.Right)return Direction.Right;if(
vector==refMatrix.Left)return Direction.Left;if(vector==refMatrix.Up)return Direction.Up;if(vector==refMatrix.Down)return
Direction.Down;throw new Exception("Unknown direction");}private readonly StringBuilder pbOut=new StringBuilder();public static
string optionalInfo="";void WritePbOutput(){const string programInfoStr=programName+" v"+versionStr+" | ";string avgRtStr=
profiler.RunningAverageMs.ToString("0.0000");pbOut.Append(programInfoStr).Append(avgRtStr);TimeSpan upTime=DateTime.UtcNow-
bootTime;pbOut.Append("\nUptime: ").Append(SecondsToDuration(upTime.TotalSeconds));pbOut.Append("\nMode: ").AppendLine(NavMode.
ToString());if(optionalInfo!=null&&optionalInfo.Length>0){pbOut.AppendLine();pbOut.AppendLine(optionalInfo);}if(
_cruiseController!=null){pbOut.AppendLine();_cruiseController?.AppendStatus(pbOut);}pbOut.Append("\n-- Loaded Config --\n"+nameof(config.
MaxThrustOverrideRatio)+"="+config.MaxThrustOverrideRatio.ToString()+"\n"+nameof(config.IgnoreMaxThrustForSpeedMatch)+"="+config.
IgnoreMaxThrustForSpeedMatch.ToString()+"\n"+nameof(config.ShipControllerTag)+"="+config.ShipControllerTag+"\n"+nameof(config.ThrustGroupName)+"="+
config.ThrustGroupName+"\n"+nameof(config.GyroGroupName)+"="+config.GyroGroupName+"\n"+nameof(config.ConsoleLcdName)+"="+
config.ConsoleLcdName+"\n"+nameof(config.CruiseOffsetDist)+"="+config.CruiseOffsetDist.ToString()+"\n"+nameof(config.
CruiseOffsetSideDist)+"="+config.CruiseOffsetSideDist.ToString()+"\n"+nameof(config.Ship180TurnTimeSeconds)+"="+config.
Ship180TurnTimeSeconds.ToString()+"\n"+nameof(config.MaintainDesiredSpeed)+"="+config.MaintainDesiredSpeed.ToString()+"\n");if(debugLcd!=null)
pbOut.Append("\nDebug: ").Append(debugLcd!=null);pbOut.Append("\n-- Detected Blocks --").Append("\nConsoleLcd: "+(consoleLcd
!=null)).Append("\nDebugLcd: "+(debugLcd!=null)).AppendLine().Append(thrusters[Direction.Forward].Count+
" Forward Thrusters\n").Append(thrusters[Direction.Backward].Count+" Backward Thrusters\n").Append(thrusters[Direction.Right].Count+
" Right Thrusters\n").Append(thrusters[Direction.Left].Count+" Left Thrusters\n").Append(thrusters[Direction.Up].Count+" Up Thrusters\n").
Append(thrusters[Direction.Down].Count+" Down Thrusters\n").Append(gyros.Count+" Gyros").Append(
"\n\n-- Runtime Information --").Append("\nLast: "+Runtime.LastRunTimeMs).Append("\nAverage: "+avgRtStr).Append("\nMax: "+profiler.MaxRuntimeMsFast);
Echo(pbOut.ToString());pbOut.Clear();if(consoleLcd!=null&&_cruiseController!=null){pbOut.AppendLine(
$"{_cruiseController.Name} | NavOS {versionStr} | {profiler.RunningAverageMs:0.000}\nStatus ------------------------");int beforeLength=pbOut.Length;_cruiseController?.AppendStatus(pbOut);if(pbOut.Length==beforeLength){pbOut.AppendLine(
"Config ------------------------");pbOut.AppendLine($"  Max Thrust {thrustController.MaxForwardThrustRatio,18:0 %}");_cruiseController?.AppendStatus(
pbOut);}if(!string.IsNullOrWhiteSpace(optionalInfo)){pbOut.AppendLine("Additional Info ---------------");pbOut.AppendLine(
optionalInfo);}consoleLcd?.WriteText(pbOut);pbOut.Clear();}else if(consoleLcd!=null){pbOut.AppendLine(
$"{NavMode} | NavOS {versionStr} | {profiler.RunningAverageMs:0.000}\nStatus ------------------------");if(!string.IsNullOrWhiteSpace(optionalInfo)){pbOut.AppendLine(optionalInfo);}pbOut.AppendLine(
"Config ------------------------");pbOut.AppendLine($"  Max Thrust {thrustController.MaxForwardThrustRatio,18:0 %}");consoleLcd?.WriteText(pbOut);pbOut.
Clear();}}public static string SecondsToDuration(double seconds,bool fractions=false){if(double.IsNaN(seconds))return"NaN";if
(double.IsInfinity(seconds))return"Infinity";seconds=Math.Abs(seconds);int hours=(int)seconds/3600;seconds%=3600;int
minutes=(int)seconds/60;seconds%=60;if(hours>0)return
$"{hours:00}:{minutes:00}:{seconds:00}{(fractions?(seconds-(int)seconds).ToString(".000"):"")}";else return$"{minutes:00}:{seconds:00}";}public static void Log(string message)=>debug.AppendLine(message);Dictionary<
string,Action<CommandLine>>commands;void InitCommands(){commands=new Dictionary<string,Action<CommandLine>>{{"abort",cmd=>
AbortNav(false)},{"reload",CommandReload},{"maxthrustoverrideratio",CommandSetThrustRatio},{"thrustratio",CommandSetThrustRatio}
,{"cruise",CommandCruise},{"retro",cmd=>CommandRetrograde()},{"retrograde",cmd=>CommandRetrograde()},{"retroburn",cmd=>
CommandRetroburn()},{"prograde",cmd=>CommandPrograde()},{"radialin",_=>CommandRadialIn()},{"radialout",_=>CommandRadialOut()},{"match",
cmd=>CommandSpeedMatch()},{"speedmatch",cmd=>CommandSpeedMatch()},{"orient",CommandOrient},{"calibrateturn",cmd=>
CommandCalibrateTurnTime()},{"thrust",CommandApplyThrust},{"journey",CommandJourney},{"autopilot",CommandAutopilot},{"approach",CommandApproach}
,};}void HandleArgs(string argument){if(String.IsNullOrWhiteSpace(argument)){return;}CommandLine cmd=new CommandLine(
argument);foreach(var kv in commands){if(cmd.Matches(0,kv.Key)){kv.Value.Invoke(cmd);}}}void CommandReload(CommandLine cmd){
AbortNav(false);LoadConfig(true);optionalInfo="Config reloaded";}void CommandSetThrustRatio(CommandLine cmd){if(cmd.Count<2){
optionalInfo="New override ratio argument not found!";return;}double result;if(!double.TryParse(cmd[1],out result)){optionalInfo=
"Could not parse new override ratio";return;}if(result<0||result>1){optionalInfo="Ratio must be between 0.0 and 1.0!";return;}result=MathHelper.Clamp(result
,0,1);config.MaxThrustOverrideRatio=result;SaveConfig();if(CruiseController is SpeedMatch)thrustController.
MaxForwardThrustRatio=config.IgnoreMaxThrustForSpeedMatch?1f:(float)result;else thrustController.MaxForwardThrustRatio=(float)result;
optionalInfo=$"New thrust ratio set to {result:0.##}";}void CommandCruise(CommandLine cmd){AbortNav(false);optionalInfo="";if(cmd.
Count<3){optionalInfo="Cruise arguments missing";return;}double desiredSpeed;Vector3D target;string error;if(
TryParseCruiseCommands(cmd,out desiredSpeed,out target,out error)){InitRetroCruise(target,desiredSpeed);}else{optionalInfo=error;}}void
InitRetroCruise(Vector3D target,double speed,RetroCruiseControl.CruiseStage stage=RetroCruiseControl.CruiseStage.None,bool saveConfig=
true){speed=Math.Min(speed,this.GetWorldMaxSpeed());thrustController.MaxForwardThrustRatio=(float)config.
MaxThrustOverrideRatio;NavMode=NavModeEnum.Cruise;CruiseController=new RetroCruiseControl(target,speed,aimController,controller,gyros,
thrustController,this,stage){ShipFlipTimeInSeconds=config.Ship180TurnTimeSeconds*1.5,};config.PersistStateData=
$"{NavModeEnum.Cruise}|{speed}|{stage}";Storage=target.ToString();if(saveConfig){SaveConfig();}}void CommandRetrograde(){AbortNav(false);optionalInfo="";
NavMode=NavModeEnum.Retrograde;CruiseController=new Retrograde(aimController,controller,gyros);config.PersistStateData=
$"{NavModeEnum.Retrograde}";SaveConfig();}void CommandRetroburn(){AbortNav(false);optionalInfo="";thrustController.MaxForwardThrustRatio=(float)
config.MaxThrustOverrideRatio;NavMode=NavModeEnum.Retroburn;CruiseController=new Retroburn(aimController,controller,gyros,
thrustController);config.PersistStateData=$"{NavModeEnum.Retroburn}";SaveConfig();}void CommandPrograde(){AbortNav(false);optionalInfo=
"";NavMode=NavModeEnum.Prograde;CruiseController=new Prograde(aimController,controller,gyros);config.PersistStateData=
$"{NavModeEnum.Prograde}";SaveConfig();}void CommandRadialIn(){AbortNav(false);optionalInfo="";NavMode=NavModeEnum.RadialIn;CruiseController=new
RadialIn(aimController,controller,gyros);config.PersistStateData=NavModeEnum.RadialIn.ToString();SaveConfig();}void
CommandRadialOut(){AbortNav(false);optionalInfo="";NavMode=NavModeEnum.RadialOut;CruiseController=new RadialOut(aimController,controller
,gyros);config.PersistStateData=NavModeEnum.RadialOut.ToString();SaveConfig();}void CommandSpeedMatch(){AbortNav(false);
optionalInfo="";if(!wcApiActive){try{wcApiActive=wcApi.Activate(Me);}catch{wcApiActive=false;}}if(!wcApiActive){optionalInfo=
"WeaponCore API error";return;}var target=wcApi.GetAiFocus(Me.CubeGrid.EntityId);if((target?.EntityId??0)==0){optionalInfo=
"Locked target not found";return;}InitSpeedMatch(target.Value.EntityId);}void CommandOrient(CommandLine cmd){AbortNav(false);optionalInfo="";if(!
cmd.Gps.HasValue){optionalInfo="Incorrect orient command params, no gps detected";return;}InitOrient(cmd.Gps.Value.Position
);optionalInfo="";}void CommandCalibrateTurnTime(){AbortNav(false);optionalInfo="";NavMode=NavModeEnum.CalibrateTurnTime;
CruiseController=new CalibrateTurnTime(config,aimController,controller,gyros);config.PersistStateData=$"{NavModeEnum.CalibrateTurnTime}"
;SaveConfig();}void CommandApplyThrust(CommandLine cmd){AbortNav(false);optionalInfo="";float ratio;if(cmd.Count>=3&&cmd.
Matches(1,"set")&&float.TryParse(cmd[2],out ratio)){if(ratio<0||ratio>1.01){optionalInfo="Ratio must be between 0.0 and 1.0!";}
else{optionalInfo=$"Forward thrust override set to {ratio*100:0.###}%";foreach(IMyThrust thrust in thrusters[Direction.
Forward]){thrust.ThrustOverridePercentage=ratio;}}}}void CommandJourney(CommandLine cmd){optionalInfo="";if(cmd.Count<2)return;
optionalInfo="";string failReason;if(cmd.Matches(1,"load"))InitJourney();else if(CruiseController is Journey&&!((Journey)
CruiseController).HandleJourneyCommand(cmd,out failReason))optionalInfo=failReason;}void CommandAutopilot(CommandLine cmd){if(cmd.Count<
3){optionalInfo="Autopilot arguments missing";return;}AbortNav(false);optionalInfo="";double desiredSpeed;Vector3D target
;string error;if(TryParseCruiseCommands(cmd,out desiredSpeed,out target,out error)){InitAutopilot(target,desiredSpeed);}
else{optionalInfo=error;}}void CommandApproach(CommandLine cmd){if(cmd.Count<2){optionalInfo=
"Approach speed argument not found";return;}double approachSpeed;if(!double.TryParse(cmd[1],out approachSpeed)){optionalInfo=
"Could not parse speed argument";return;}MyDetectedEntityInfo target=wcApi.GetAiFocus(Me.CubeGrid.EntityId)??default(MyDetectedEntityInfo);if(target.
IsEmpty()){optionalInfo="No locked target";return;}Vector3D targetPos=target.BoundingBox.Center;double targetRadius=
BoundingSphereD.CreateFromBoundingBox(target.BoundingBox).Radius;double minRadius=Me.CubeGrid.WorldVolume.Radius+targetRadius;Vector3D
myPos=Me.CubeGrid.WorldAABB.Center;if(Vector3D.DistanceSquared(targetPos,myPos)<(minRadius*minRadius)){optionalInfo=
"Too close to target";return;}Vector3D targetDir=Vector3D.Normalize(targetPos-myPos);Vector3D offsetDir=Vector3D.CalculatePerpendicularVector
(targetDir);if(target.Velocity.LengthSquared()>1&&Vector3D.Dot(offsetDir,target.Velocity)>0){offsetDir=-offsetDir;}
Vector3D navTarget=targetPos+offsetDir*minRadius;Vector3D navTargetDir=Vector3D.Normalize(navTarget-myPos);double?
closestIntersection=new RayD(myPos,navTargetDir).Intersects(new BoundingSphereD(targetPos,minRadius));if(closestIntersection.HasValue){
navTarget=myPos+navTargetDir*closestIntersection.Value;}AbortNav(false);optionalInfo="";optionalInfo=
$"Approaching target {target.Name}";InitRetroCruise(navTarget,approachSpeed);}void InitAutopilot(Vector3D target,double speed,bool saveConfig=true){speed=
Math.Min(speed,this.GetWorldMaxSpeed());thrustController.MaxForwardThrustRatio=(float)config.MaxThrustOverrideRatio;NavMode=
NavModeEnum.Autopilot;CruiseController=new Autopilot(controller,thrustController){Target=target,MaxSpeed=(float)speed,};config.
PersistStateData=$"{NavMode}|{speed}";Storage=target.ToString();if(saveConfig){SaveConfig();}}bool TryParseCruiseCommands(CommandLine
cmd,out double desiredSpeed,out Vector3D target,out string error){target=Vector3D.Zero;desiredSpeed=0;try{if(!double.
TryParse(cmd[1],out desiredSpeed)){error="Could not parse desired speed";return false;}Vector3D controllerPos=controller.
WorldAABB.Center;double result;bool distanceCruise;if(distanceCruise=double.TryParse(cmd[2],out result)){target=controllerPos+(
controller.WorldMatrix.Forward*result);}else if(cmd.Gps.HasValue){target=cmd.Gps.Value.Position;}else{error=
"Could not parse target";return false;}bool useOffsets=true;if(cmd.Count>=4&&cmd[3].ToLower()=="false"){useOffsets=false;}useOffsets=useOffsets
&&!distanceCruise;if(useOffsets){Vector3D offsetTarget=Vector3D.Zero;if(config.CruiseOffsetDist>0){if(config.
CruiseOffsetSideDist==0){error="Side offset cannot be zero when using offset";return false;}offsetTarget+=(target-controllerPos).
SafeNormalize()*-config.CruiseOffsetDist;}if(config.CruiseOffsetSideDist>0){offsetTarget+=Vector3D.CalculatePerpendicularVector(
target-controllerPos)*config.CruiseOffsetSideDist;}target+=offsetTarget;}error=null;return true;}catch(Exception e){error=e.
ToString();return false;}}void InitOrient(Vector3D target){NavMode=NavModeEnum.Orient;CruiseController=new Orient(aimController,
controller,gyros,target);config.PersistStateData=$"{NavModeEnum.Orient}";Storage=target.ToString();SaveConfig();}void
InitSpeedMatch(long targetId){thrustController.MaxForwardThrustRatio=config.IgnoreMaxThrustForSpeedMatch?1f:(float)config.
MaxThrustOverrideRatio;NavMode=NavModeEnum.SpeedMatch;CruiseController=new SpeedMatch(targetId,wcApi,controller,Me,thrustController);config.
PersistStateData=$"{NavModeEnum.SpeedMatch}|{targetId}";SaveConfig();}void InitJourney(){thrustController.MaxForwardThrustRatio=(float)
config.MaxThrustOverrideRatio;NavMode=NavModeEnum.Journey;CruiseController=new Journey(aimController,controller,gyros,config.
Ship180TurnTimeSeconds*1.5,thrustController,this);}
}
public enum NavModeEnum{Sleep=-1,Idle=0,Cruise=1,Retrograde=2,Prograde=3,SpeedMatch=4,Retroburn=5,Orient=6,
CalibrateTurnTime=7,Journey=8,Autopilot=9,RadialIn=10,RadialOut=11,}public enum Direction:byte{Forward,Backward,Left,Right,Up,Down,
MAX_COUNT,}public class CommandLine{public GPS?Gps{get;private set;}public int Count=>args.Count;List<string>args=new List<string>(
);private static readonly char[]charSpace={' '};public CommandLine(string command){ParseArgs(command);}void ParseArgs(
string command){GPS gps;if(GPS.TryParse(command,out gps)){Gps=gps;int gpsStrIndex=command.IndexOf(Gps.Value.OriginalString);
string[]argsBeforeGps=command.Substring(0,gpsStrIndex).Split(charSpace,StringSplitOptions.RemoveEmptyEntries);string[]
argsAfterGps=command.Substring(gpsStrIndex+Gps.Value.OriginalString.Length).Split(charSpace,StringSplitOptions.RemoveEmptyEntries);
args.AddRange(argsBeforeGps);args.Add(Gps.Value.OriginalString);args.AddRange(argsAfterGps);}else{args.AddRange(command.
Split(charSpace,StringSplitOptions.RemoveEmptyEntries));}}public string this[int index,bool lowerCase=false]=>args.
IsValidIndex(index)?(lowerCase?args[index].ToLower():args[index]):null;public bool Matches(int index,string str,bool ignoreCase=true
){if(!args.IsValidIndex(index)){return false;}return args[index].Equals(str,ignoreCase?StringComparison.
CurrentCultureIgnoreCase:StringComparison.CurrentCulture);}}public class Config{public enum OffsetType{None,Forward,Side,}public static Config
Default{get;}=new Config();public string PersistStateData{get;set;}="";public double MaxThrustOverrideRatio{get;set;}=1.0;
public bool IgnoreMaxThrustForSpeedMatch{get;set;}=false;public string ShipControllerTag{get;set;}="Nav";public string
ThrustGroupName{get;set;}="NavThrust";public string GyroGroupName{get;set;}="NavGyros";public string ConsoleLcdName{get;set;}=
"consoleLcd";public double CruiseOffsetDist{get;set;}=0;public double CruiseOffsetSideDist{get;set;}=500;public double
Ship180TurnTimeSeconds{get;set;}=10.0;public bool MaintainDesiredSpeed{get;set;}=true;public List<string>JourneySetup{get;}=new List<string>()
;Config(){}public static bool TryParse(string str,out Config config){var conf=new Config();if(string.IsNullOrWhiteSpace(
str)||!str.StartsWith("NavConfig")){config=null;return false;}string[]lines=str.Split(Environment.NewLine.ToCharArray());
Dictionary<string,string>confValues=new Dictionary<string,string>();for(int i=1;i<lines.Length;i++){if(String.IsNullOrWhiteSpace(
lines[i])||lines[i].StartsWith("//"))continue;string[]substrings=lines[i].Split('=');if(substrings.Length>=2&&substrings[0]!=
null&&substrings[1]!=null){if(!confValues.ContainsKey(substrings[0])){confValues.Add(substrings[0],substrings[1]);}else{
confValues[substrings[0]]=substrings[1];}}}string result;if(confValues.TryGetValue(nameof(PersistStateData),out result))conf.
PersistStateData=result;if(confValues.TryGetValue(nameof(MaxThrustOverrideRatio),out result)){double val;if(double.TryParse(result,out
val))conf.MaxThrustOverrideRatio=val;}if(confValues.TryGetValue(nameof(IgnoreMaxThrustForSpeedMatch),out result)){bool val;
if(bool.TryParse(result,out val))conf.IgnoreMaxThrustForSpeedMatch=val;}if(confValues.TryGetValue(nameof(ShipControllerTag
),out result))conf.ShipControllerTag=result;if(confValues.TryGetValue(nameof(ThrustGroupName),out result))conf.
ThrustGroupName=result;if(confValues.TryGetValue(nameof(GyroGroupName),out result))conf.GyroGroupName=result;if(confValues.TryGetValue(
nameof(ConsoleLcdName),out result))conf.ConsoleLcdName=result;if(confValues.TryGetValue(nameof(CruiseOffsetDist),out result)){
double val;if(double.TryParse(result,out val))conf.CruiseOffsetDist=val;}if(confValues.TryGetValue(nameof(CruiseOffsetSideDist
),out result)){double val;if(double.TryParse(result,out val))conf.CruiseOffsetSideDist=val;}if(confValues.TryGetValue(
"OffsetDirection",out result)){OffsetType enumResult;double val;if(Enum.TryParse<OffsetType>(result,true,out enumResult)&&enumResult!=
OffsetType.None&&confValues.TryGetValue("CruiseOffset",out result)&&double.TryParse(result,out val)){if(enumResult==OffsetType.
Side){conf.CruiseOffsetSideDist+=val;}else if(enumResult==OffsetType.Forward){conf.CruiseOffsetDist+=val;}}}if(confValues.
TryGetValue(nameof(Ship180TurnTimeSeconds),out result)){double val;if(double.TryParse(result,out val))conf.Ship180TurnTimeSeconds=
val;}List<string>lineList=new List<string>(lines);int journeyStartIndex=lineList.FindIndex(i=>i=="[Journey Start]");int
journeyEndIndex=lineList.FindIndex(i=>i=="[Journey End]");if(journeyStartIndex>=0&&journeyEndIndex>journeyStartIndex+1){for(int i=
journeyStartIndex+1;i<journeyEndIndex;i++){if(!String.IsNullOrWhiteSpace(lineList[i])&&!lines[i].StartsWith("//")){conf.JourneySetup.Add(
lineList[i]);}}}if(confValues.TryGetValue(nameof(MaintainDesiredSpeed),out result)){bool val;if(bool.TryParse(result,out val))
conf.MaintainDesiredSpeed=val;}config=conf;return true;}public override string ToString(){StringBuilder strb=new
StringBuilder();strb.AppendLine($"NavConfig | {Program.versionStr}");strb.AppendLine(
"// Remember to recompile after you change the config!");strb.AppendLine($"{nameof(PersistStateData)}={PersistStateData}");strb.AppendLine();strb.AppendLine(
"// Maximum thrust override. 0 to 1 (Dont use 0)");strb.AppendLine($"{nameof(MaxThrustOverrideRatio)}={MaxThrustOverrideRatio}");strb.AppendLine(
$"{nameof(IgnoreMaxThrustForSpeedMatch)}={IgnoreMaxThrustForSpeedMatch}");strb.AppendLine();strb.AppendLine("// Tag for the controller used for ship orientation");strb.AppendLine(
$"{nameof(ShipControllerTag)}={ShipControllerTag}");strb.AppendLine();strb.AppendLine("// If this group doesn't exist it uses all thrusters");strb.AppendLine(
$"{nameof(ThrustGroupName)}={ThrustGroupName}");strb.AppendLine();strb.AppendLine("// If this group doesn't exist it uses all gyros");strb.AppendLine(
$"{nameof(GyroGroupName)}={GyroGroupName}");strb.AppendLine();strb.AppendLine("// Copies pb output to this lcd is it exists");strb.AppendLine(
$"{nameof(ConsoleLcdName)}={ConsoleLcdName}");strb.AppendLine();strb.AppendLine("// Cruise offset distances in meters");strb.AppendLine(
$"{nameof(CruiseOffsetDist)}={CruiseOffsetDist}");strb.AppendLine($"{nameof(CruiseOffsetSideDist)}={CruiseOffsetSideDist}");strb.AppendLine();strb.AppendLine(
"// Time for the ship to do a 180 degree turn in seconds");strb.AppendLine($"{nameof(Ship180TurnTimeSeconds)}={Ship180TurnTimeSeconds}");strb.AppendLine();strb.AppendLine(
"// Keeps the ship oriented to the target and maintain speed until decel time");strb.AppendLine($"{nameof(MaintainDesiredSpeed)}={MaintainDesiredSpeed}");strb.AppendLine();strb.AppendLine(
"// Format: <speed> <stopAtWaypoint> <GPS>");strb.AppendLine("[Journey Start]");foreach(var line in JourneySetup)strb.AppendLine(line);strb.Append("[Journey End]")
;return strb.ToString();}}public class WcPbApi{Action<IMyTerminalBlock,IDictionary<MyDetectedEntityInfo,float>>
_getSortedThreats;Action<IMyTerminalBlock,ICollection<MyDetectedEntityInfo>>_getObstructions;Func<long,int,MyDetectedEntityInfo>
_getAiFocus;public bool Activate(IMyTerminalBlock pbBlock){var dict=pbBlock.GetProperty("WcPbAPI")?.As<IReadOnlyDictionary<string,
Delegate>>().GetValue(pbBlock);if(dict==null)throw new Exception("WcPbAPI failed to activate");return ApiAssign(dict);}public
bool ApiAssign(IReadOnlyDictionary<string,Delegate>delegates){if(delegates==null)return false;AssignMethod(delegates,
"GetSortedThreats",ref _getSortedThreats);AssignMethod(delegates,"GetObstructions",ref _getObstructions);AssignMethod(delegates,
"GetAiFocus",ref _getAiFocus);return true;}void AssignMethod<T>(IReadOnlyDictionary<string,Delegate>delegates,string name,ref T
field)where T:class{if(delegates==null){field=null;return;}Delegate del;if(!delegates.TryGetValue(name,out del))throw new
Exception($"{GetType().Name} :: Couldn't find {name} delegate of type {typeof(T)}");field=del as T;if(field==null)throw new
Exception($"{GetType().Name} :: Delegate {name} is not type {typeof(T)}, instead it's: {del.GetType()}");}public void
GetSortedThreats(IMyTerminalBlock pBlock,IDictionary<MyDetectedEntityInfo,float>collection)=>_getSortedThreats?.Invoke(pBlock,collection
);public void GetObstructions(IMyTerminalBlock pBlock,ICollection<MyDetectedEntityInfo>collection)=>_getObstructions?.
Invoke(pBlock,collection);public MyDetectedEntityInfo?GetAiFocus(long shooter,int priority=0)=>_getAiFocus?.Invoke(shooter,
priority);}public struct GPS{public string OriginalString{get;}public string Name{get;}public Vector3D Position{get;}public GPS(
string originalString,string name,Vector3D position){OriginalString=originalString;Name=name;Position=position;}public static
bool TryParse(string input,out GPS result){result=default(GPS);int startIndex=input.IndexOf("gps:",StringComparison.
CurrentCultureIgnoreCase);if(startIndex<0){return false;}input=input.Substring(startIndex,input.LastIndexOf(':')+1-startIndex);string[]parts=
input.Split(new char[]{':'},StringSplitOptions.RemoveEmptyEntries);string name;Vector3D pos;for(int i=2;i<parts.Length-2;i++)
{if(double.TryParse(parts[i],out pos.X)&&double.TryParse(parts[i+1],out pos.Y)&&double.TryParse(parts[i+2],out pos.Z)){
name=String.Join(":",parts.Skip(1).Take(i-1));result=new GPS(input,name,pos);return true;}}return false;}}public interface
IAimController{void Orient(Vector3D forward,IMyGyro gyro,MatrixD refMatrix);void Orient(Vector3D forward,Vector3D up,IMyGyro gyro,
MatrixD refMatrix);void Reset();}public class JitAim:IAimController{public static double gyroMaxRPM=3.1415;public JitAim(
MyCubeSize gridSize){double angleMultiplier=gridSize==MyCubeSize.Small?2:1;gyroMaxRPM*=angleMultiplier;lastAngleRoll=
lastAnglePitch=lastAngleYaw=0;lMPTRoll=lMPTPitch=lMPTYaw=0;}double lastAngleRoll,lastAnglePitch,lastAngleYaw;double lMPTRoll,lMPTPitch
,lMPTYaw;double modr,modp,mody=0;bool active=false;public void flush(){if(!active){active=true;lastAngleRoll=
lastAnglePitch=lastAngleYaw=0;lMPTRoll=lMPTPitch=lMPTYaw=0;modr=modp=mody=0;}}void calculateAxisSpecificData(double now,ref double
prior,ref double lastMPT,ref double mod,out bool ontarg,out bool braking){ontarg=false;var radMovedPerTick=Math.Abs(prior-now
);var ticksToTarget=Math.Abs(now)/radMovedPerTick;var initVel=radMovedPerTick;var rateOfDecel=Math.Abs(lastMPT-
radMovedPerTick);if(rateOfDecel>mod)mod=rateOfDecel;var ticksToStop=initVel/rateOfDecel;bool closing=Math.Abs(now)<Math.Abs(prior);if(!
closing){lastMPT=0.0001;mod=MathHelper.EPSILON;}else lastMPT=radMovedPerTick;if(closing){if(ticksToStop>ticksToTarget)braking=
true;else braking=false;}else braking=false;if(Math.Abs(now)<error_threshold){braking=true;if(radMovedPerTick<
minVelThreshold)ontarg=true;}prior=now;}double error_threshold=MathHelperD.ToRadians(0.025);double minVelThreshold=MathHelperD.
ToRadians(0.005);double nobrake_threshold=MathHelperD.ToRadians(45);double amp_threshold=MathHelperD.ToRadians(10);int
minTicksOnTarget=5;int ticksOnTarget=0;public static void GetRotationAnglesSimultaneous(Vector3D desiredForwardVector,MatrixD
worldMatrix,out double yaw,out double pitch,out double roll){desiredForwardVector=Utils.SafeNormalize(desiredForwardVector);MatrixD
transposedWm;MatrixD.Transpose(ref worldMatrix,out transposedWm);Vector3D.Rotate(ref desiredForwardVector,ref transposedWm,out
desiredForwardVector);Vector3D axis=new Vector3D(desiredForwardVector.Y,-desiredForwardVector.X,0);double angle=Math.Acos(MathHelper.Clamp(-
desiredForwardVector.Z,-1.0,1.0));if(Vector3D.IsZero(axis)){angle=desiredForwardVector.Z<0?0:Math.PI;yaw=angle;pitch=0;roll=0;return;}axis=
Utils.SafeNormalize(axis);yaw=-axis.Y*angle;pitch=axis.X*angle;roll=-axis.Z*angle;}public static void ApplyGyroOverride(
double pitch_speed,double yaw_speed,double roll_speed,IMyGyro gyro,MatrixD refMatrix){var rotationVec=new Vector3D(-
pitch_speed,yaw_speed,roll_speed);var shipMatrix=refMatrix;var relativeRotationVec=Vector3D.TransformNormal(rotationVec,shipMatrix)
;var gyroMatrix=gyro.WorldMatrix;var transformedRotationVec=Vector3D.TransformNormal(relativeRotationVec,Matrix.Transpose
(gyroMatrix));gyro.Pitch=(float)transformedRotationVec.X;gyro.Yaw=(float)transformedRotationVec.Y;gyro.Roll=(float)
transformedRotationVec.Z;gyro.GyroOverride=true;}public void Orient(Vector3D forward,IMyGyro gyro,MatrixD refMatrix){flush();double pitch,yaw,
roll;GetRotationAnglesSimultaneous(forward,refMatrix,out yaw,out pitch,out roll);bool yT,pT,rT,yB,pB,rB;
calculateAxisSpecificData(roll,ref lastAngleRoll,ref lMPTRoll,ref modr,out rT,out rB);calculateAxisSpecificData(pitch,ref lastAnglePitch,ref
lMPTPitch,ref modp,out pT,out pB);calculateAxisSpecificData(yaw,ref lastAngleYaw,ref lMPTYaw,ref mody,out yT,out yB);Vector3D
a_impulse=new Vector3D(pB?0:pitch,yB?0:yaw,rB?0:roll);if(a_impulse!=Vector3D.Zero){var m=a_impulse.AbsMax();if(m>amp_threshold)m=
gyroMaxRPM;else m=m/amp_threshold*gyroMaxRPM;a_impulse=a_impulse/a_impulse.AbsMax()*m;}ApplyGyroOverride(a_impulse.X,a_impulse.Y,
a_impulse.Z,gyro,refMatrix);if(yT&&pT&&rT){ticksOnTarget+=1;}else ticksOnTarget=0;if(ticksOnTarget>minTicksOnTarget){gyro.Pitch=0
;gyro.Yaw=0;gyro.Roll=0;}}public void Orient(Vector3D forward,Vector3D up,IMyGyro gyro,MatrixD refMatrix)=>Orient(forward
,gyro,refMatrix);public void Reset(){active=false;}}public class Autopilot:ICruiseController{private const double
TARGET_REACHED_DIST=0.05;private const double TARGET_REACHED_SPEED=0.01;public event CruiseTerminateEventDelegate CruiseTerminated;public
string Name=>nameof(Autopilot);public Vector3D?Target{get{return _target;}set{_target=value;}}public float MaxSpeed{get{return
_maxSpeed;}set{_maxSpeed=value;}}Vector3D?_target;float _maxSpeed;float _shipMass;Vector3D _gravity;private readonly
IMyShipController _shipController;private readonly VariableThrustController _thrustController;public Autopilot(IMyShipController
shipController,VariableThrustController thrustController){_shipController=shipController;_thrustController=thrustController;}public
void AppendStatus(StringBuilder strb){strb.Append("\n-- Autopilot Status --\n");double targetDist=Vector3D.Distance(_target
??Vector3D.Zero,_shipController.WorldAABB.Center);strb.Append($"Max Speed: {_maxSpeed:0.#} m/s\n");strb.Append(
$"Distance: {targetDist:0.0}m\n");}int counter=-1;public void Run(){counter++;if(!_target.HasValue){Terminate("Target is null");return;}if(counter%10==0
){_thrustController.UpdateThrusts();_shipMass=_shipController.CalculateShipMass().PhysicalMass;_gravity=_shipController.
GetNaturalGravity();_thrustController.SetDampenerState(false);}const int UPS=6;if(counter%(60/UPS)!=0){return;}if(RunStateless(
_thrustController,_target.Value,_maxSpeed,UPS,_shipMass,_gravity)){_thrustController.SetDampenerState(true);Terminate("Target reached");
return;}}public static bool RunStateless(VariableThrustController thrustController,Vector3D target,float maxSpeed,float ups,
float shipMass,Vector3D naturalGravity){IMyShipController shipController=thrustController.ShipController;thrustController.
UpdateThrusts();thrustController.SetDampenerState(false);Vector3D currentVelocity=shipController.GetShipVelocities().LinearVelocity;
Vector3D displacement=target-shipController.WorldAABB.Center;bool targetReachedDist=displacement.LengthSquared()<=(
TARGET_REACHED_DIST*TARGET_REACHED_DIST);bool targetReachedSpeed=currentVelocity.LengthSquared()<=(TARGET_REACHED_SPEED*
TARGET_REACHED_SPEED);if(targetReachedDist&&targetReachedSpeed){return true;}else if(targetReachedDist){thrustController.DampenAllDirections
(currentVelocity,naturalGravity,shipMass,ups);return false;}MatrixD transposedShipControllerWorldMatrix=MatrixD.Transpose
(shipController.WorldMatrix);Vector3D localVelocity;Vector3D localGravity;Vector3D localDisplacement;Vector3D.
TransformNormal(ref currentVelocity,ref transposedShipControllerWorldMatrix,out localVelocity);Vector3D.TransformNormal(ref
naturalGravity,ref transposedShipControllerWorldMatrix,out localGravity);Vector3D.TransformNormal(ref displacement,ref
transposedShipControllerWorldMatrix,out localDisplacement);Vector3D maxLocalClosingVelocity=Vector3D.Abs(Vector3D.Normalize(localDisplacement))*maxSpeed;
float maxForwardThrustRatio=thrustController.MaxForwardThrustRatio;Dictionary<Direction,List<IMyThrust>>thrusters=
thrustController.Thrusters;MovePerAxis(localVelocity.X,localDisplacement.X,shipMass,thrusters[Direction.Right],thrusters[Direction.Left]
,thrustController.GetThrustInDirection(Direction.Right),thrustController.GetThrustInDirection(Direction.Left),
localGravity.X,maxLocalClosingVelocity.X,ups,1,1);MovePerAxis(localVelocity.Y,localDisplacement.Y,shipMass,thrusters[Direction.Up],
thrusters[Direction.Down],thrustController.GetThrustInDirection(Direction.Up),thrustController.GetThrustInDirection(Direction.
Down),localGravity.Y,maxLocalClosingVelocity.Y,ups,1,1);MovePerAxis(localVelocity.Z,localDisplacement.Z,shipMass,thrusters[
Direction.Backward],thrusters[Direction.Forward],thrustController.GetThrustInDirection(Direction.Backward),thrustController.
GetThrustInDirection(Direction.Forward),localGravity.Z,maxLocalClosingVelocity.Z,ups,1,maxForwardThrustRatio);return false;}private static
void MovePerAxis(double velocity,double displacement,double shipMass,List<IMyThrust>approachThrusters,List<IMyThrust>
stoppingThrusters,double approachThrust,double stoppingThrust,double gravity,double maxClosingVelocity,double ups,float maxAccelRatio,
float maxDecelRatio){if(displacement<0){velocity=-velocity;displacement=-displacement;gravity=-gravity;var
tempApproachThrusters=approachThrusters;approachThrusters=stoppingThrusters;stoppingThrusters=tempApproachThrusters;var tempApproachAccel=
approachThrust;approachThrust=stoppingThrust;stoppingThrust=tempApproachAccel;float tempMaxAccelRatio=maxAccelRatio;maxAccelRatio=
maxDecelRatio;maxDecelRatio=tempMaxAccelRatio;}double approachAccel=approachThrust/shipMass;double stoppingAccel=stoppingThrust/
shipMass;double accelMulti;double decelInTimeSteps;bool shouldAccel=ComputeMaxAxisAccel(velocity,displacement,approachAccel+
gravity,stoppingAccel-gravity,ups,out accelMulti,out decelInTimeSteps);approachThrust*=maxAccelRatio;stoppingThrust*=
maxDecelRatio;approachAccel*=maxAccelRatio;stoppingAccel*=maxDecelRatio;double timeStep=1.0/ups;double totalAccelRatio=0;double
totalDecelRatio=0;if(shouldAccel){totalAccelRatio+=accelMulti;}else if(velocity<=0){double accelRatio=(-velocity*shipMass/
approachThrust)*ups;totalAccelRatio+=accelRatio;}else{double desiredStopDist=displacement;double desiredStopAccel=(velocity*velocity)/
(desiredStopDist*2);double desiredStopTime=velocity/desiredStopAccel;double decelRatio;double targetDecelTime=
desiredStopTime-timeStep;if(targetDecelTime<=0||desiredStopDist<=0.0001){decelRatio=(velocity*shipMass/stoppingThrust)*ups;}else{double
targetDecel=velocity/targetDecelTime;decelRatio=targetDecel/stoppingAccel;}totalDecelRatio+=decelRatio;}totalAccelRatio=MathHelper.
Max(0,totalAccelRatio);totalDecelRatio=MathHelper.Max(0,totalDecelRatio);{totalAccelRatio+=approachAccel!=0&&gravity<0?(-
gravity/approachAccel):0;totalDecelRatio+=stoppingAccel!=0&&gravity>0?(gravity/stoppingAccel):0;}{double nextVelocity=velocity+
((totalAccelRatio*approachAccel)-(totalDecelRatio*stoppingAccel)+gravity)*timeStep;if(nextVelocity>maxClosingVelocity){
double excessVelocity=nextVelocity-maxClosingVelocity;totalAccelRatio-=(excessVelocity*ups)/approachAccel;totalDecelRatio-=
totalAccelRatio*approachAccel/stoppingAccel;}}totalAccelRatio=MathHelper.Max(0,totalAccelRatio);totalDecelRatio=MathHelper.Max(0,
totalDecelRatio);double minThrust=Math.Min(totalAccelRatio*approachThrust,totalDecelRatio*stoppingThrust);totalAccelRatio-=minThrust/
approachThrust;totalDecelRatio-=minThrust/stoppingThrust;totalAccelRatio=MathHelper.Saturate(totalAccelRatio);totalDecelRatio=
MathHelper.Saturate(totalDecelRatio);totalAccelRatio*=maxAccelRatio;totalDecelRatio*=maxDecelRatio;SetThrustRatio(
approachThrusters,(float)totalAccelRatio);SetThrustRatio(stoppingThrusters,(float)totalDecelRatio);}public static bool
ComputeMaxAxisAccel(double velocity,double displacement,double accel,double decel,double ups,out double bestAccelMulti,out double
bestDecelInTimeSteps){bestDecelInTimeSteps=0;const double lookAhead=2;double lowerBound=0;double upperBound=1;for(int i=0;i<10;i++){double
accelMulti=i==0?upperBound:(lowerBound+upperBound)*0.5;double decelInSeconds=ComputeTimeToDecel(velocity,displacement,accel*
accelMulti,decel);bool valid=decelInSeconds*ups>lookAhead;if(valid){lowerBound=accelMulti;bestDecelInTimeSteps=decelInSeconds;}
else{upperBound=accelMulti;}}bestAccelMulti=lowerBound;bestDecelInTimeSteps*=ups;return bestAccelMulti!=0;}private static
void SetThrustRatio(List<IMyThrust>thrusters,float percentage){percentage=percentage<0?0:(percentage>1?1:percentage);for(int
i=thrusters.Count-1;i>=0;i--){var thruster=thrusters[i];if(thruster.ThrustOverridePercentage!=percentage){thrusters[i].
ThrustOverridePercentage=percentage;}}}public static double ComputeTimeToDecel(double velocity,double displacement,double accel,double decel){if
(decel<=0){return 0;}if(accel<=0){double stopDist=(velocity*velocity)/(decel*2);return(displacement-stopDist)/velocity;}
double initialTimeToStop=0;if(velocity<0){initialTimeToStop=-velocity/accel;velocity=0;displacement+=0.5*accel*
initialTimeToStop*initialTimeToStop;}double vMax=Math.Sqrt((decel*(velocity*velocity+2*accel*displacement))/(accel+decel));double
timeToDecel=(vMax-velocity)/accel;return(timeToDecel+initialTimeToStop);}public void Abort()=>Terminate("Aborted");public void
Terminate(string reason){_thrustController.ResetThrustOverrides();CruiseTerminated.Invoke(this,reason);}}internal class
CalibrateTurnTime:OrientControllerBase,ICruiseController{public event CruiseTerminateEventDelegate CruiseTerminated;public string Name=>
"CalibrateTurn";const double TICK=1.0/60.0;private const double orientToleranceAngleRadians=0.075*(Math.PI/180.0);double elapsedTimeMs;
Vector3D target;Config _config;public CalibrateTurnTime(Config config,IAimController aimControl,IMyShipController controller,
IList<IMyGyro>gyros):base(aimControl,controller,gyros){_config=config;elapsedTimeMs=0;target=controller.WorldMatrix.Backward;
}public void AppendStatus(StringBuilder strb){strb.AppendLine($"  Elapsed: {elapsedTimeMs:0} ms");}public void Run(){
Orient(target);elapsedTimeMs+=TICK*1000;if(Vector3D.Dot(target,ShipController.WorldMatrix.Forward)>0.999999){_config.
Ship180TurnTimeSeconds=Math.Round(elapsedTimeMs/1000.0,2,MidpointRounding.AwayFromZero);Complete();}}public void Abort()=>Terminate("Aborted")
;void Complete()=>Terminate($"Calibration Completed.\nTurn time is {_config.Ship180TurnTimeSeconds} seconds.");public
void Terminate(string reason){ResetGyroOverride();CruiseTerminated.Invoke(this,reason);}protected override void
OnNoFunctionalGyrosLeft()=>Terminate("No functional gyros found, Calibration terminated.");}public enum CruiseTerminateReason{Completed=1,
Aborted=2,Other=3,}public delegate void CruiseTerminateEventDelegate(ICruiseController source,string reason);public interface
ICruiseController{event CruiseTerminateEventDelegate CruiseTerminated;string Name{get;}void AppendStatus(StringBuilder strb);void Run();
void Abort();void Terminate(string reason);}public class Journey:ICruiseController{public event CruiseTerminateEventDelegate
CruiseTerminated=delegate{};public string Name=>nameof(Journey);IMyShipController shipController;IAimController aimControl;IList<IMyGyro
>gyros;double decelStartMarginSeconds;VariableThrustController thrustControl;Program prog;ICruiseController cruiseControl
;List<Waypoint>waypoints;bool started=false;int currentStep=0;public Journey(IAimController aimControl,IMyShipController
controller,IList<IMyGyro>gyros,double decelStartMarginSeconds,VariableThrustController thrustControl,Program program){this.
aimControl=aimControl;this.shipController=controller;this.gyros=gyros;this.decelStartMarginSeconds=decelStartMarginSeconds;this.
thrustControl=thrustControl;this.prog=program;waypoints=ParseJourneySetup();}List<Waypoint>ParseJourneySetup(){List<Waypoint>
waypoints=new List<Waypoint>();List<string>lines=prog.config.JourneySetup;foreach(var line in lines){Waypoint waypoint;if(
TryParseWaypoint(line,out waypoint)){waypoints.Add(waypoint);}}return waypoints;}public void AppendStatus(StringBuilder strb){if(!
started){strb.AppendLine("Awaiting Start Command...");strb.AppendLine("Waypoints:");for(int i=0;i<waypoints.Count;i++){var step
=waypoints[i];strb.AppendLine($"#{i+1}: {step.Name}").AppendLine(
$"  Target: X:{step.Target.X:0} Y:{step.Target.Y:0} Z:{step.Target.Z:0}").AppendLine($"  Speed: {step.DesiredSpeed}").AppendLine($"  StopAtWaypoint: {step.StopAtWaypoint}");}}else{strb.
AppendLine($"Current Step: {currentStep+1}");strb.AppendLine("Waypoints:");for(int i=0;i<waypoints.Count;i++){var step=waypoints[i
];strb.AppendLine($"#{i+1}: {step.Name}").AppendLine(
$"  Target: X:{step.Target.X:0} Y:{step.Target.Y:0} Z:{step.Target.Z:0}").AppendLine($"  Speed: {step.DesiredSpeed}").AppendLine($"  StopAtWaypoint: {step.StopAtWaypoint}");}strb.AppendLine(
"Cruise Status -----------------");cruiseControl.AppendStatus(strb);}}public bool HandleJourneyCommand(CommandLine cmd,out string failReason){failReason=
"Unknown Journey Command!";if(cmd.Count<2){return false;}else if(started){failReason="Cannot run Journey commands once started";return false;}if(
cmd.Matches(1,"start")){if(waypoints.Count==0){failReason="No waypoints set!";return false;}InitStep(0);return true;}return
false;}public void InitStep(int index){if(!waypoints.IsValidIndex(index)){Terminate("Waypoint index is out of range");}var
step=waypoints[index];Vector3D targetOffset=Vector3D.Zero;step.DesiredSpeed=Math.Min(step.DesiredSpeed,prog.GetWorldMaxSpeed
());if(index==waypoints.Count-1){if(prog.config.CruiseOffsetSideDist>0){targetOffset+=Vector3D.
CalculatePerpendicularVector(step.Target-shipController.GetPosition())*prog.config.CruiseOffsetSideDist;}if(prog.config.CruiseOffsetDist>0){
targetOffset+=(step.Target-shipController.GetPosition()).SafeNormalize()*-prog.config.CruiseOffsetDist;}}started=true;currentStep=
index;if(step.StopAtWaypoint){cruiseControl=new RetroCruiseControl(step.Target+targetOffset,step.DesiredSpeed,aimControl,
shipController,gyros,thrustControl,prog,false){ShipFlipTimeInSeconds=this.decelStartMarginSeconds,};}else{cruiseControl=new
OneWayCruise(step.Target+targetOffset,step.DesiredSpeed,aimControl,shipController,gyros,thrustControl);}cruiseControl.
CruiseTerminated+=OnCruiseTerminated;SavePersistantData();}void SavePersistantData(){prog.config.PersistStateData=
$"{NavModeEnum.Journey}|{currentStep}";prog.Me.CustomData=prog.config.ToString();}public void Run(){if(started){cruiseControl.Run();}}void OnCruiseTerminated(
ICruiseController sender,string reason){if(reason=="JourneyTerminated"){return;}if(reason=="No functional gyros found"){Terminate(reason)
;}else if(Vector3D.DistanceSquared(shipController.GetPosition(),waypoints[currentStep].Target)<=100*100){currentStep++;if
(currentStep<waypoints.Count)InitStep(currentStep);else Terminate("Destination Reached");}else{Terminate(
$"Journey terminated unexpectedly."+$"\nRetroCruise terminate reason: {reason}"+$"\nStep: {currentStep+1} of {waypoints.Count}"+
$"\nDistanceToCurrentTarget: {Vector3D.Distance(shipController.GetPosition(),waypoints[currentStep].Target):0.00}"+$"\nCurrent Speed: {shipController.GetShipSpeed():0.00}");}}public void Abort()=>Terminate("Aborted");public void
Terminate(string reason){cruiseControl?.Terminate("JourneyTerminated");CruiseTerminated.Invoke(this,reason);}public static bool
TryParseWaypoint(string line,out Waypoint waypoint){string[]args=line.Split(' ');if(args.Length<3){waypoint=default(Waypoint);return
false;}double speed;bool stopAtWaypoint;GPS gps;if(double.TryParse(args[0],out speed)&&bool.TryParse(args[1],out
stopAtWaypoint)&&GPS.TryParse(line,out gps)){waypoint=new Waypoint(gps.Name,speed,gps.Position,stopAtWaypoint);return true;}waypoint=
default(Waypoint);return false;}public struct Waypoint{public string Name;public double DesiredSpeed;public Vector3D Target;
public bool StopAtWaypoint;public Waypoint(string name,double desiredSpeed,Vector3D target,bool stopAtWaypoint){Name=name;
DesiredSpeed=desiredSpeed;Target=target;StopAtWaypoint=stopAtWaypoint;}}}public class OneWayCruise:OrientControllerBase,
ICruiseController{public enum OneWayCruiseStage:byte{None=0,CancelPerpendicularVelocity=1,OrientAndAccelerate=2,Complete=6,Aborted=7,}
const double DegToRadMulti=Math.PI/180.0;const double RadToDegMulti=180.0/Math.PI;public event CruiseTerminateEventDelegate
CruiseTerminated=delegate{};public string Name=>nameof(RetroCruiseControl);public OneWayCruiseStage Stage{get{return _stage;}private set
{if(_stage!=value){var old=_stage;_stage=value;OnStageChanged();Program.Log($"{old} to {value}");}}}public Vector3D
Target{get;}public double DesiredSpeed{get;}public float MaxThrustRatio{get{return thrustController.MaxForwardThrustRatio;}set
{if(thrustController.MaxForwardThrustRatio!=value){thrustController.MaxForwardThrustRatio=value;UpdateThrustAndAccel();}}
}public double OrientToleranceAngleRadians{get;set;}=0.075*DegToRadMulti;public double maxInitialPerpendicularVelocity=
0.5;VariableThrustController thrustController;OneWayCruiseStage _stage;int counter=-1;bool counter10=false;float gridMass;
float forwardAccel;double?lastAimDirectionAngleRad=null;Vector3D naturalGravity;double estimatedTimeOfArrival;double
lastForwardSpeedDuringAccel;double lastForwardThrustRatioDuringAccel;double accelTime,cruiseTime,distanceToTarget,vmax;bool approachingTarget;
public OneWayCruise(Vector3D target,double desiredSpeed,IAimController aimControl,IMyShipController controller,IList<IMyGyro>
gyros,VariableThrustController thrustController):base(aimControl,controller,gyros){this.Target=target;this.DesiredSpeed=
desiredSpeed;this.thrustController=thrustController;Stage=OneWayCruiseStage.None;gridMass=controller.CalculateShipMass().
PhysicalMass;UpdateThrustAndAccel();}public void AppendStatus(StringBuilder strb){string targetDistStr=distanceToTarget<1000?
distanceToTarget.ToString("0 m"):(distanceToTarget/1000d).ToString("0.0 km");string stageName=Stage==OneWayCruiseStage.None?"None":Stage
==OneWayCruiseStage.CancelPerpendicularVelocity?"Brrake Lateral Speed":Stage==OneWayCruiseStage.OrientAndAccelerate?
"Accelerate":Stage.ToString();strb.AppendLine($"  {stageName}");strb.AppendLine($"  Target Dist {targetDistStr,17}");double eta=
estimatedTimeOfArrival;strb.AppendLine($"  ETA {$"{(eta<0?"-":"")}{(int)eta/60:00}:{Math.Abs(eta)%60:00}",25}");strb.AppendLine(
$"Config ------------------------");strb.AppendLine($"  Max Speed {DesiredSpeed,15:0.0} m/s");strb.AppendLine($"  Max Thrust {MaxThrustRatio,18:0 %}");}
void DampenSidewaysToZero(Vector3D shipVelocity,float ups){Vector3 localVelocity=Vector3D.TransformNormal(shipVelocity,
MatrixD.Transpose(ShipController.WorldMatrix));Vector3 thrustAmount=localVelocity*gridMass*ups;float right=thrustAmount.X<0?-
thrustAmount.X:0;float left=thrustAmount.X>0?thrustAmount.X:0;float up=thrustAmount.Y<0?-thrustAmount.Y:0;float down=thrustAmount.Y>
0?thrustAmount.Y:0;thrustController.SetSideThrusts(left,right,up,down);}public void Run(){counter++;counter10=counter%10
==0;bool counter30=counter%30==0;if(Stage==OneWayCruiseStage.None){ResetGyroOverride();thrustController.
ResetThrustOverrides();UpdateThrustAndAccel();}if(counter10){lastAimDirectionAngleRad=null;naturalGravity=ShipController.GetNaturalGravity()
;SetDampenerState(false);}if(counter30){gridMass=ShipController.CalculateShipMass().PhysicalMass;UpdateThrustAndAccel();}
Vector3D velocity=ShipController.GetShipVelocities().LinearVelocity+naturalGravity;double velocityLength=velocity.Length();
Vector3D displacement=Target-ShipController.WorldAABB.Center;Vector3D targetDirection=Utils.Normalize(ref displacement,out
distanceToTarget);if(Stage==OneWayCruiseStage.None){Vector3D perpVel=Vector3D.ProjectOnPlane(ref velocity,ref targetDirection);if(
perpVel.LengthSquared()>maxInitialPerpendicularVelocity*maxInitialPerpendicularVelocity)Stage=OneWayCruiseStage.
CancelPerpendicularVelocity;else Stage=OneWayCruiseStage.OrientAndAccelerate;}if(Stage==OneWayCruiseStage.CancelPerpendicularVelocity){
CancelPerpendicularVelocity(velocity,targetDirection);}if(Stage==OneWayCruiseStage.OrientAndAccelerate){OrientAndAccelerate(velocity,velocityLength
,targetDirection);}if(Stage==OneWayCruiseStage.Complete){estimatedTimeOfArrival=0;SetDampenerState(true);Terminate(
distanceToTarget<10?"Destination Reached":"Terminated");}if(counter10){if(Stage<=OneWayCruiseStage.OrientAndAccelerate){double
currentAndDesiredSpeedDelta=Math.Abs(DesiredSpeed-velocityLength);accelTime=currentAndDesiredSpeedDelta/(forwardAccel*MaxThrustRatio);double
accelDist=accelTime*((velocityLength+DesiredSpeed)*0.5);double cruiseDist=distanceToTarget-accelDist;cruiseTime=cruiseDist/
DesiredSpeed;vmax=0;estimatedTimeOfArrival=accelTime+cruiseTime;}else{accelTime=0;cruiseTime=distanceToTarget/velocityLength;
estimatedTimeOfArrival=cruiseTime;}}}void UpdateThrustAndAccel(){thrustController.UpdateThrusts();forwardAccel=(float)(thrustController.
GetThrustInDirection(Direction.Forward)/gridMass);}void ResetThrustOverridesExceptFront(){var backwardThrusters=thrustController.Thrusters[
Direction.Backward];for(int i=backwardThrusters.Count-1;i>=0;i--){backwardThrusters[i].ThrustOverridePercentage=0;}
thrustController.SetSideThrusts(0,0,0,0);}void SetDampenerState(bool enabled)=>ShipController.DampenersOverride=enabled;void
OnStageChanged(){thrustController.ResetThrustOverrides();ResetGyroOverride();SetDampenerState(false);lastAimDirectionAngleRad=null;
lastForwardSpeedDuringAccel=0;lastForwardThrustRatioDuringAccel=0;}void CancelPerpendicularVelocity(Vector3D velocity,Vector3D targetDir){Vector3D
aimDirection=-Vector3D.ProjectOnPlane(ref velocity,ref targetDir);double perpSpeed=aimDirection.Length();if(perpSpeed<=
maxInitialPerpendicularVelocity){Stage=OneWayCruiseStage.OrientAndAccelerate;return;}Orient(aimDirection);if(!counter10){return;}if(!
lastAimDirectionAngleRad.HasValue){lastAimDirectionAngleRad=AngleRadiansBetweenVectorAndControllerForward(aimDirection);}const float UPS=6;float
forwardOverrideRatio=lastAimDirectionAngleRad.Value<=OrientToleranceAngleRadians?Math.Min(MaxThrustRatio,(float)(perpSpeed/forwardAccel*UPS)
):0;var forwardThrusters=thrustController.Thrusters[Direction.Forward];for(int i=forwardThrusters.Count-1;i>=0;i--){
forwardThrusters[i].ThrustOverridePercentage=forwardOverrideRatio;}thrustController.SetSideThrusts(0,0,0,0);
ResetThrustOverridesExceptFront();}void OrientAndAccelerate(Vector3D velocity,double velocityLength,Vector3D targetDir){bool closing=Vector3D.Dot(
targetDir,velocity)>0;if(!closing&&approachingTarget){Stage=OneWayCruiseStage.Complete;return;}approachingTarget=closing;Orient(
targetDir);if(!counter10){return;}if(!lastAimDirectionAngleRad.HasValue){lastAimDirectionAngleRad=
AngleRadiansBetweenVectorAndControllerForward(targetDir);}const float UPS=6;if(lastAimDirectionAngleRad.Value<=OrientToleranceAngleRadians){double forwardSpeed=
velocityLength>0?(velocityLength*Vector3D.Dot(velocity/velocityLength,targetDir)):0;double actualAccel=forwardSpeed-
lastForwardSpeedDuringAccel;double expectedAccel=(forwardAccel*lastForwardThrustRatioDuringAccel)/UPS;double speedDelta=DesiredSpeed-forwardSpeed;
double desiredAccel=speedDelta+(expectedAccel-actualAccel);float thrustRatio=Math.Min(MaxThrustRatio,(float)(desiredAccel/
forwardAccel*UPS));var forwardThrusters=thrustController.Thrusters[Direction.Forward];for(int i=forwardThrusters.Count-1;i>=0;i--){
forwardThrusters[i].ThrustOverridePercentage=thrustRatio;}Vector3D velocityPerpendicularToTarget=Vector3D.ProjectOnPlane(ref velocity,
ref targetDir);DampenSidewaysToZero(velocityPerpendicularToTarget,UPS);lastForwardSpeedDuringAccel=forwardSpeed;
lastForwardThrustRatioDuringAccel=thrustRatio;return;}thrustController.ResetThrustOverrides();}double AngleRadiansBetweenVectorAndControllerForward(
Vector3D vec){Vector3D.Normalize(ref vec,out vec);double cos=Vector3D.Dot(ShipController.WorldMatrix.Forward,vec);double angle=
Math.Acos(cos);return double.IsNaN(angle)?0:angle;}public void Terminate(string reason){thrustController.
ResetThrustOverrides();ResetGyroOverride();CruiseTerminated.Invoke(this,reason);}public void Abort(){Stage=OneWayCruiseStage.Aborted;
Terminate("Aborted");}protected override void OnNoFunctionalGyrosLeft()=>Terminate("No functional gyros found");}public class
Orient:OrientControllerBase,ICruiseController{public event CruiseTerminateEventDelegate CruiseTerminated=delegate{};public
virtual string Name=>"Orient";Vector3D _targetPos;public Orient(IAimController aimControl,IMyShipController controller,IList<
IMyGyro>gyros,Vector3D target):base(aimControl,controller,gyros){this._targetPos=target;}protected Orient(IAimController
aimControl,IMyShipController controller,IList<IMyGyro>gyros):base(aimControl,controller,gyros){}public void AppendStatus(
StringBuilder strb){}public virtual void Run(){Orient(_targetPos-ShipController.GetPosition());}public void Terminate(string reason){
ResetGyroOverride();CruiseTerminated.Invoke(this,reason);}public void Abort()=>Terminate("Aborted");protected override void
OnNoFunctionalGyrosLeft()=>Terminate("No functional gyros found");}public abstract class OrientControllerBase{public IAimController AimControl{
get;private set;}public IMyShipController ShipController{get;private set;}public IMyGyro GyroInUse{get;private set;}Queue<
IMyGyro>availableGyros;protected OrientControllerBase(IAimController aimControl,IMyShipController controller,IList<IMyGyro>
gyros){this.AimControl=aimControl;this.ShipController=controller;this.availableGyros=new Queue<IMyGyro>();foreach(var gyro in
gyros){availableGyros.Enqueue(gyro);}EnsureGyroIsValid();}protected void Orient(Vector3D forward){if(!EnsureGyroIsValid()){
return;}AimControl.Orient(forward,GyroInUse,ShipController.WorldMatrix);}bool EnsureGyroIsValid(){if(GyroInUse==null||
GyroInUse.Closed||!GyroInUse.IsFunctional){while(availableGyros.Count>0){GyroInUse=availableGyros.Dequeue();if(GyroInUse!=null&&!
GyroInUse.Closed&&GyroInUse.IsFunctional){break;}}if(GyroInUse==null||GyroInUse.Closed||!GyroInUse.IsFunctional){
OnNoFunctionalGyrosLeft();return false;}GyroInUse.Enabled=true;}return true;}public void ResetGyroOverride(){if(GyroInUse!=null){GyroInUse.
Pitch=0;GyroInUse.Yaw=0;GyroInUse.Roll=0;GyroInUse.GyroOverride=false;}}protected abstract void OnNoFunctionalGyrosLeft();}
public class Prograde:Orient{private const double TERMINATE_SPEED=5;public override string Name=>nameof(Prograde);public
Prograde(IAimController aimControl,IMyShipController controller,IList<IMyGyro>gyros):base(aimControl,controller,gyros){}public
override void Run(){Vector3D shipVelocity=ShipController.GetShipVelocities().LinearVelocity;if(shipVelocity.LengthSquared()<=
TERMINATE_SPEED*TERMINATE_SPEED){Terminate($"Speed is less than {TERMINATE_SPEED:0.#} m/s");return;}Orient(shipVelocity);}}public class
RadialIn:Orient{public override string Name=>nameof(RadialIn);Vector3D _gravity;public RadialIn(IAimController aimControl,
IMyShipController controller,IList<IMyGyro>gyros):base(aimControl,controller,gyros){_gravity=controller.GetNaturalGravity();}public
override void Run(){if((Program.counter%10)==0){_gravity=ShipController.GetNaturalGravity();}if(_gravity==Vector3D.Zero){
Terminate("No gravity detected");return;}Orient(_gravity);}}public class RadialOut:Orient{public override string Name=>nameof(
RadialOut);Vector3D _gravity;public RadialOut(IAimController aimControl,IMyShipController controller,IList<IMyGyro>gyros):base(
aimControl,controller,gyros){_gravity=controller.GetNaturalGravity();}public override void Run(){if((Program.counter%10)==0){
_gravity=ShipController.GetNaturalGravity();}if(_gravity==Vector3D.Zero){Terminate("No gravity detected");return;}Orient(-
_gravity);}}public class Retroburn:Orient,ICruiseController{const double ORIENT_SPEED_THRESHOLD=5;const float DAMPENER_TOLERANCE
=0.005f;public override string Name=>nameof(Retroburn);VariableThrustController thrustController;float gridMass;public
Retroburn(IAimController aimControl,IMyShipController controller,List<IMyGyro>gyros,VariableThrustController thrustController):
base(aimControl,controller,gyros){this.thrustController=thrustController;}public override void Run(){if(Program.counter%30==
0){gridMass=ShipController.CalculateShipMass().PhysicalMass;thrustController.UpdateThrusts();}Vector3D shipVelocity=
ShipController.GetShipVelocities().LinearVelocity;double velocitySq=shipVelocity.LengthSquared();Vector3D gravity=ShipController.
GetNaturalGravity();if(velocitySq>ORIENT_SPEED_THRESHOLD*ORIENT_SPEED_THRESHOLD)Orient(-shipVelocity);else if(gravity!=Vector3D.Zero)
Orient(-gravity);else ResetGyroOverride();if(Program.counter%10==0){ShipController.DampenersOverride=false;const float UPS=6;
if(Vector3D.Dot(-shipVelocity.SafeNormalize(),ShipController.WorldMatrix.Forward)>0.9999||velocitySq<=
ORIENT_SPEED_THRESHOLD*ORIENT_SPEED_THRESHOLD)thrustController.DampenAllDirections(shipVelocity,gravity,gridMass,UPS-1);else thrustController.
ResetThrustOverrides();}if(velocitySq<=DAMPENER_TOLERANCE*DAMPENER_TOLERANCE){thrustController.ResetThrustOverrides();ShipController.
DampenersOverride=true;Terminate($"Speed is less than {DAMPENER_TOLERANCE} m/s");return;}}}public class RetroCruiseControl:
OrientControllerBase,ICruiseController{public enum CruiseStage:byte{None,CancelPerpendicularVelocity,CancelPerpendicularVelocityFullStop,
Accelerate,Decelerate,DecelerateNoOrient,Overshoot,ExcessivePerpVel,Complete,Aborted,Terminated,}public event
CruiseTerminateEventDelegate CruiseTerminated=delegate{};public string Name{get;}="Cruise";public CruiseStage Stage{get{return _stage;}private set{
if(_stage!=value){var old=_stage;_stage=value;OnStageChanged();Program.Log($"{old} to {value}");}}}public Vector3D Target{
get;}public double DesiredSpeed{get;}public float MaxThrustRatio=>_thrustController.MaxForwardThrustRatio;public double
ShipFlipTimeInSeconds{get;set;}=10;const double PERPENDICULAR_SPEED_THRESHOLD=1;const double DECEL_RESERVE_THRUST=0.05;const double
TARGET_REACHED_SPEED=0.05;const double TARGET_REACHED_DISTANCE=5;const double AIM_ONTARGET_ANGLE_COS=0.99999;private readonly
VariableThrustController _thrustController;private readonly Program _program;private readonly Config _config;CruiseStage _stage;int _counter=-1;
string _terminateReason;float _gridMass;float _forwardAccelPremult;float _minSideAccel;Vector3D _naturalGravity;double
_remainingStageTime;double _perpSpeed;double _perpVelStopTime;double _perpVelStopDist;double _accelTime;double _accelDist;double
_lastForwardSpeedDuringAccel;double _lastForwardThrustRatioDuringAccel;double _decelTime;double _decelDist;double _targetDist;Vector3D _targetDir;
Vector3D _prevAimDir;CruiseStage _initialStage=CruiseStage.None;bool savePersistentData;private readonly IMyShipController
_controller;public RetroCruiseControl(Vector3D target,double desiredSpeed,IAimController aimControl,IMyShipController controller,
IList<IMyGyro>gyros,VariableThrustController thrustController,Program program,bool savePersistentData=true):base(aimControl,
controller,gyros){this.Target=target;this.DesiredSpeed=desiredSpeed;this._thrustController=thrustController;this._program=program;
this._config=program.config;this.savePersistentData=savePersistentData;this._controller=controller;Stage=CruiseStage.None;
_gridMass=controller.CalculateShipMass().PhysicalMass;UpdateThrustAndAccel();}public RetroCruiseControl(Vector3D target,double
desiredSpeed,IAimController aimControl,IMyShipController controller,IList<IMyGyro>gyros,VariableThrustController thrustController,
Program program,CruiseStage stage,bool savePersistentData=true):this(target,desiredSpeed,aimControl,controller,gyros,
thrustController,program){_initialStage=stage;this.savePersistentData=savePersistentData;}string[]stageNames={"None",
"Brake Lateral Speed","Pre-accel Full Stop","Accelerate","Decelerate","Decelerate (gyro lock)","Overshoot","Excessive Lateral Speed",
"Completed","Aborted","Terminated",};public void AppendStatus(StringBuilder strb){string targetDistStr=_targetDist<1000?_targetDist
.ToString("0 m"):(_targetDist/1000d).ToString("0.0 km");strb.AppendLine($"  {stageNames[(int)_stage]} {$"{(_remainingStageTime<0?"-":"")}{(int)_remainingStageTime/60:00}:{Math.Abs(_remainingStageTime)%60:00}".PadLeft(31-3-stageNames[(int)_stage].Length)}"
);strb.Append(_stage==CruiseStage.CancelPerpendicularVelocity?$"   Remaining Speed {_perpSpeed,8:0.0} m/s\n   Remaining Dist {_perpVelStopDist,11:0} m\n   Target Dist {targetDistStr,16}\n"
:_stage==CruiseStage.CancelPerpendicularVelocityFullStop?$"   Target Dist {targetDistStr,16}\n":_stage==CruiseStage.
Accelerate?$"   Remaining Dist {(_accelDist<1000?_accelDist.ToString("0 m"):(_accelDist/1000d).ToString("0.0 km")),13}\n   Target Dist {targetDistStr,16}\n"
:_stage==CruiseStage.Decelerate?$"   Stopping Dist {(_decelDist<1000?_decelDist.ToString("0 m"):(_decelDist/1000d).ToString("0.0 km")),14}\n   Target Dist {targetDistStr,16}\n"
:_stage==CruiseStage.DecelerateNoOrient?$"   Target Dist {targetDistStr,16}\n":_stage==CruiseStage.Overshoot?
$"   Target Dist {targetDistStr,16}\n":_stage==CruiseStage.Complete?$"{_terminateReason}":_stage==CruiseStage.Aborted?$"":_stage==CruiseStage.Terminated?
$"{_terminateReason}":"");strb.AppendLine($"Config ------------------------");strb.AppendLine($"  Max Speed {DesiredSpeed,15:0.0} m/s");strb.
AppendLine($"  Max Thrust {MaxThrustRatio,18:0 %}");}public static string GetShortDistance(double meters){return meters>=1000?(
meters/1000).ToString("0.## km"):meters.ToString("0 m");}void DampenSidewaysToZero(Vector3D shipVelocity,float ups){Vector3
localVelocity=Vector3D.TransformNormal(shipVelocity,MatrixD.Transpose(ShipController.WorldMatrix));Vector3 thrustAmount=localVelocity
*_gridMass*ups;const float EPSILON=0.00001f;float right=thrustAmount.X<-EPSILON?-thrustAmount.X:0;float left=thrustAmount
.X>EPSILON?thrustAmount.X:0;float up=thrustAmount.Y<-EPSILON?-thrustAmount.Y:0;float down=thrustAmount.Y>EPSILON?
thrustAmount.Y:0;_thrustController.SetSideThrusts(left,right,up,down);}const int THRUST_UPS=6;const double THRUST_TIME_STEP=1.0/
THRUST_UPS;bool _stageChangedPrev=false;public void Run(){_counter++;bool update10=_counter%10==0;bool update30=_counter%30==0;if(
_stage==CruiseStage.None){ResetGyroOverride();_thrustController.ResetThrustOverrides();}if(update10||_stage==CruiseStage.None)
{_naturalGravity=ShipController.GetNaturalGravity();SetDampenerState(false);}if(update30||_stage==CruiseStage.None){
_gridMass=ShipController.CalculateShipMass().PhysicalMass;UpdateThrustAndAccel();}Vector3D currentPos=ShipController.WorldAABB.
Center;Vector3D currentVelocity=ShipController.GetShipVelocities().LinearVelocity+_naturalGravity;double currentSpeed=
currentVelocity.Length();Vector3D displacement=Target-currentPos;double targetDist;Vector3D targetDir=Utils.Normalize(ref displacement,
out targetDist);bool closing=Vector3D.Dot(currentVelocity,targetDir)>0;_targetDir=targetDir;_targetDist=targetDist;bool
stageChanged=false||_stageChangedPrev;_stageChangedPrev=false;if(_stage==CruiseStage.None){if(_initialStage!=CruiseStage.None){Stage
=_initialStage;}else if(targetDist<=Math.Max(TARGET_REACHED_DISTANCE,_controller.CubeGrid.WorldVolume.Radius)){Stage=
CruiseStage.Terminated;Terminate("Aborted, too close to target.");return;}else{double perpSpeedSq=Vector3D.ProjectOnPlane(ref
currentVelocity,ref _targetDir).LengthSquared();Stage=perpSpeedSq>PERPENDICULAR_SPEED_THRESHOLD*PERPENDICULAR_SPEED_THRESHOLD?
CruiseStage.CancelPerpendicularVelocity:CruiseStage.Accelerate;}}if(_stage==CruiseStage.CancelPerpendicularVelocity){if(!closing){
Stage=CruiseStage.CancelPerpendicularVelocityFullStop;stageChanged=true;}else{Vector3D targetDir2=targetDir;Vector3D perpVel=
Vector3D.ProjectOnPlane(ref currentVelocity,ref targetDir2);double perpSpeed=perpVel.Length();Vector3D velocityInTargetDir=
Vector3D.ProjectOnVector(ref currentVelocity,ref targetDir2);double timeToStopPerpVel=perpSpeed/_forwardAccelPremult;Vector3D
drift=(velocityInTargetDir*timeToStopPerpVel)+(perpVel*0.5*timeToStopPerpVel);for(int i=0;i<10;i++){targetDir2=Vector3D.
Normalize(Target-(currentPos+drift));perpVel=Vector3D.ProjectOnPlane(ref currentVelocity,ref targetDir2);perpSpeed=perpVel.Length
();velocityInTargetDir=Vector3D.ProjectOnVector(ref currentVelocity,ref targetDir2);timeToStopPerpVel=perpSpeed/
_forwardAccelPremult;drift=(velocityInTargetDir*timeToStopPerpVel)+(perpVel*0.5*timeToStopPerpVel);}_perpSpeed=perpSpeed;_perpVelStopTime=
timeToStopPerpVel;_perpVelStopDist=drift.Length();_remainingStageTime=_perpVelStopTime;bool approachingAtEnd=Vector3D.Dot(
velocityInTargetDir,targetDir2)>0;double actualStopTimeAtEnd=velocityInTargetDir.Length()/_forwardAccelPremult;double actualStopDistAtEndSq
=((velocityInTargetDir*(ShipFlipTimeInSeconds*0.5))+(velocityInTargetDir*0.5*actualStopTimeAtEnd)).LengthSquared();double
availableStopDistAtEndSq=Vector3D.DistanceSquared(currentPos+drift,Target);bool canStopAtEnd=actualStopDistAtEndSq<availableStopDistAtEndSq;if(!
approachingAtEnd||!canStopAtEnd){Stage=CruiseStage.CancelPerpendicularVelocityFullStop;stageChanged=true;}else if(perpSpeed<
PERPENDICULAR_SPEED_THRESHOLD){Stage=CruiseStage.Accelerate;stageChanged=true;}else{Vector3D desiredAimDir=-perpVel.Normalized();Orient(desiredAimDir
);bool onTarget=Vector3D.Dot(desiredAimDir,_controller.WorldMatrix.Forward)>AIM_ONTARGET_ANGLE_COS;if(update10||
stageChanged){float forwardThrustRatio=onTarget?(float)(perpSpeed/(_forwardAccelPremult*THRUST_TIME_STEP)):0;forwardThrustRatio=
MathHelper.Saturate(forwardThrustRatio)*MaxThrustRatio;SetForwardThrustAndResetBackThrusts(forwardThrustRatio);_thrustController.
SetSideThrusts(0,0,0,0);}}}}if(_stage==CruiseStage.CancelPerpendicularVelocityFullStop){Vector3D desiredAimDir=-currentVelocity.
Normalized();Orient(desiredAimDir);bool onTarget=Vector3D.Dot(desiredAimDir,_controller.WorldMatrix.Forward)>
AIM_ONTARGET_ANGLE_COS;if(update10||stageChanged){float forwardThrustRatio=onTarget?(float)(currentSpeed/(_forwardAccelPremult*
THRUST_TIME_STEP)):0;forwardThrustRatio=MathHelper.Saturate(forwardThrustRatio)*MaxThrustRatio;SetForwardThrustAndResetBackThrusts(
forwardThrustRatio);Vector3D perpVel=Vector3D.ProjectOnPlane(ref currentVelocity,ref targetDir);DampenSidewaysToZero(perpVel,THRUST_UPS);
if(perpVel.LengthSquared()<PERPENDICULAR_SPEED_THRESHOLD){Stage=CruiseStage.Accelerate;stageChanged=true;}}
_remainingStageTime=currentSpeed/_forwardAccelPremult;}if(_stage==CruiseStage.Accelerate){double stopTime=currentSpeed/_forwardAccelPremult
;double stopDist=(currentSpeed*ShipFlipTimeInSeconds)+(currentSpeed*0.5*stopTime);Vector3D relativePos=Target-currentPos;
Vector3D velocityDir=currentVelocity.Normalized();double availableDistSq=Vector3D.ProjectOnVector(ref relativePos,ref
velocityDir).LengthSquared();if(closing&&(stopDist*stopDist)>=availableDistSq){Stage=CruiseStage.Decelerate;stageChanged=true;}else
{Orient(targetDir);bool onTarget=Vector3D.Dot(targetDir,_controller.WorldMatrix.Forward)>AIM_ONTARGET_ANGLE_COS;double
closingSpeed=currentSpeed>0.00001?(currentSpeed*Vector3D.Dot(currentVelocity/currentSpeed,targetDir)):0;double a2=2*
_forwardAccelPremult;double accelDist=closingSpeed<0?(-(closingSpeed*closingSpeed/a2)+(DesiredSpeed*DesiredSpeed/a2)):closingSpeed<
DesiredSpeed?((DesiredSpeed*DesiredSpeed-closingSpeed*closingSpeed)/a2):0;double decelDist=DesiredSpeed*DesiredSpeed/a2;if(accelDist
+decelDist>targetDist){_accelTime=Autopilot.ComputeTimeToDecel(closingSpeed,targetDist,_forwardAccelPremult,
_forwardAccelPremult)-ShipFlipTimeInSeconds*0.5;_accelDist=(closingSpeed+(closingSpeed+_forwardAccelPremult*_accelTime))*0.5*_accelTime;}
else{double cruiseDist=targetDist-accelDist-decelDist;_accelDist=accelDist+cruiseDist;_accelTime=(closingSpeed<DesiredSpeed?
((DesiredSpeed-closingSpeed)/_forwardAccelPremult):0)+(cruiseDist/DesiredSpeed)-ShipFlipTimeInSeconds;}
_remainingStageTime=_accelTime;if(onTarget&&(update10||stageChanged)){float forwardThrustRatio;if(closingSpeed<=0){double speedDelta=
DesiredSpeed-closingSpeed;double desiredThrustRatio=_forwardAccelPremult>0?(speedDelta/_forwardAccelPremult*THRUST_UPS):0;
forwardThrustRatio=(float)desiredThrustRatio;}else{double expectedAccel=_forwardAccelPremult*_lastForwardThrustRatioDuringAccel*
THRUST_TIME_STEP;double actualAccel=closingSpeed-_lastForwardSpeedDuringAccel;double speedDelta=DesiredSpeed-closingSpeed;double
desiredAccel=speedDelta+(expectedAccel-actualAccel);double desiredThrustRatio=_forwardAccelPremult>0?(desiredAccel/
_forwardAccelPremult*THRUST_UPS):0;forwardThrustRatio=(float)desiredThrustRatio;}forwardThrustRatio=MathHelper.Saturate(forwardThrustRatio)*
MaxThrustRatio;_lastForwardSpeedDuringAccel=MathHelper.IsValid(closingSpeed)?closingSpeed:0;_lastForwardThrustRatioDuringAccel=
MathHelper.IsValid(forwardThrustRatio)?forwardThrustRatio:0;SetForwardThrustAndResetBackThrusts(forwardThrustRatio);Vector3D
perpVel=Vector3D.ProjectOnPlane(ref currentVelocity,ref targetDir);DampenSidewaysToZero(perpVel,THRUST_UPS);}else if(update10||
stageChanged){_thrustController.ResetThrustOverrides();}}}if(_stage==CruiseStage.Decelerate){if(!closing){Stage=CruiseStage.
Overshoot;stageChanged=true;}else if(targetDist<Math.Max(TARGET_REACHED_DISTANCE,_controller.CubeGrid.WorldVolume.Radius)||
targetDist<currentSpeed*THRUST_TIME_STEP||currentSpeed<=TARGET_REACHED_SPEED){Stage=CruiseStage.DecelerateNoOrient;stageChanged=
true;_prevAimDir=_controller.WorldMatrix.Forward;}else{Vector3D aimDir=-currentVelocity.Normalized();Orient(aimDir);bool
onTarget=Vector3D.Dot(aimDir,_controller.WorldMatrix.Forward)>AIM_ONTARGET_ANGLE_COS;double closingSpeed=currentSpeed;Vector3D
relativePos=Target-currentPos;double desiredStopDist=Vector3D.ProjectOnVector(ref relativePos,ref aimDir).Length();double
timeUntilDecel=(desiredStopDist-(closingSpeed*closingSpeed)/(2*_forwardAccelPremult))/closingSpeed;if(timeUntilDecel>2*
ShipFlipTimeInSeconds){Stage=CruiseStage.Accelerate;stageChanged=true;_stageChangedPrev=true;}else if(update10||stageChanged){double
desiredStopTime=desiredStopDist/(closingSpeed*0.5)-THRUST_TIME_STEP;double desiredStopAccel=(closing&&desiredStopTime>0&&closingSpeed>0
)?(1.0/(desiredStopTime/closingSpeed)):_forwardAccelPremult;bool shouldDecel=desiredStopAccel>=_forwardAccelPremult*(1-
DECEL_RESERVE_THRUST);_decelTime=desiredStopTime;_decelDist=(closingSpeed*closingSpeed)/(2*_forwardAccelPremult);_remainingStageTime=
_decelTime;if(!shouldDecel&&_forwardAccelPremult>0){double actualStopTime=currentSpeed/_forwardAccelPremult;double actualStopDist=
currentSpeed*0.5*actualStopTime;shouldDecel|=actualStopDist>=desiredStopDist-(closingSpeed*THRUST_TIME_STEP);}float
forwardThrustRatio=(onTarget&&shouldDecel)?(float)(desiredStopAccel/_forwardAccelPremult):0;forwardThrustRatio=MathHelper.Saturate(
forwardThrustRatio)*MaxThrustRatio;SetForwardThrustAndResetBackThrusts(forwardThrustRatio);Vector3D perpVel=Vector3D.ProjectOnPlane(ref
currentVelocity,ref targetDir);DampenSidewaysToZero(onTarget?perpVel:Vector3D.Zero,THRUST_UPS);if(perpVel.LengthSquared()>Math.Pow(
desiredStopTime*_minSideAccel,2)){Stage=CruiseStage.ExcessivePerpVel;stageChanged=true;}}}}if(_stage==CruiseStage.DecelerateNoOrient){
_prevAimDir=_prevAimDir.IsZero()?_controller.WorldMatrix.Forward:_prevAimDir;Orient(_prevAimDir);if(currentSpeed<
TARGET_REACHED_SPEED){Stage=CruiseStage.Complete;stageChanged=true;}else if(!closing&&(update10||stageChanged)){_thrustController.
DampenAllDirections(currentVelocity,_gridMass,THRUST_UPS);_remainingStageTime=currentSpeed/_forwardAccelPremult;}else if(closing&&(update10
||stageChanged)){bool onTarget=Vector3D.Dot(_prevAimDir,_controller.WorldMatrix.Forward)>AIM_ONTARGET_ANGLE_COS;double
closingSpeed=currentSpeed>0?currentSpeed*Vector3D.Dot(-currentVelocity.Normalized(),_prevAimDir):0;Vector3D relativePos=Target-
currentPos;double desiredStopDist=Vector3D.ProjectOnVector(ref relativePos,ref _prevAimDir).Length();double desiredStopTime=
desiredStopDist/(closingSpeed*0.5)-THRUST_TIME_STEP;double desiredStopAccel=(closing&&desiredStopTime>0&&closingSpeed>0)?(1.0/(
desiredStopTime/closingSpeed)):_forwardAccelPremult;_remainingStageTime=desiredStopTime;float forwardThrustRatio=onTarget?(float)(
desiredStopAccel/_forwardAccelPremult):0;forwardThrustRatio=MathHelper.Saturate(forwardThrustRatio)*MaxThrustRatio;
SetForwardThrustAndResetBackThrusts(forwardThrustRatio);Vector3D perpVel=onTarget?Vector3D.ProjectOnPlane(ref currentVelocity,ref targetDir):Vector3D.Zero;
DampenSidewaysToZero(perpVel,THRUST_UPS);}}if(_stage==CruiseStage.Overshoot||_stage==CruiseStage.ExcessivePerpVel){Vector3D desiredAimDir=-
currentVelocity.Normalized();Orient(desiredAimDir);bool onTarget=Vector3D.Dot(desiredAimDir,_controller.WorldMatrix.Forward)>
AIM_ONTARGET_ANGLE_COS;if(onTarget&&(update10||stageChanged)){float forwardThrustRatio=onTarget?(float)(currentSpeed/(_forwardAccelPremult*
THRUST_TIME_STEP)):0;forwardThrustRatio=MathHelper.Saturate(forwardThrustRatio)*MaxThrustRatio;SetForwardThrustAndResetBackThrusts(
forwardThrustRatio);DampenSidewaysToZero(currentVelocity,THRUST_UPS);}else if(update10||stageChanged){_thrustController.
ResetThrustOverrides();}{double closingSpeed=currentSpeed;Vector3D aimDir=-currentVelocity.Normalized();Vector3D relativePos=Target-
currentPos;double desiredStopDist=Vector3D.ProjectOnVector(ref relativePos,ref aimDir).Length();double desiredStopTime=
desiredStopDist/(closingSpeed*0.5)-THRUST_TIME_STEP;Vector3D perpVel=Vector3D.ProjectOnPlane(ref currentVelocity,ref targetDir);if(
perpVel.LengthSquared()<=Math.Pow(desiredStopTime*_minSideAccel,2)){Stage=CruiseStage.Decelerate;stageChanged=true;
_stageChangedPrev=true;}}if(currentSpeed<0.05){Stage=CruiseStage.Terminated;Terminate("Cruise terminated due to "+(_stage==CruiseStage.
Overshoot?"target overshoot":"excessive lateral speed"));return;}}if(_stage==CruiseStage.Complete){Terminate(_targetDist<
TARGET_REACHED_DISTANCE?"Destination Reached":"Terminated");}}void SetForwardThrustAndResetBackThrusts(float forwardThrustRatio){var
forwardThrusters=_thrustController.Thrusters[Direction.Forward];for(int i=forwardThrusters.Count-1;i>=0;i--){forwardThrusters[i].
ThrustOverridePercentage=forwardThrustRatio;}var backThrusters=_thrustController.Thrusters[Direction.Backward];for(int i=backThrusters.Count-1;i
>=0;i--){backThrusters[i].ThrustOverridePercentage=0;}}void UpdateThrustAndAccel(){_thrustController.UpdateThrusts();
_forwardAccelPremult=(float)(_thrustController.GetThrustInDirection(Direction.Forward)/_gridMass)*MaxThrustRatio;double leftThrust=
_thrustController.GetThrustInDirection(Direction.Left);double rightThrust=_thrustController.GetThrustInDirection(Direction.Right);double
upThrust=_thrustController.GetThrustInDirection(Direction.Up);double downThrust=_thrustController.GetThrustInDirection(Direction
.Down);_minSideAccel=(float)(Math.Min(Math.Min(leftThrust,rightThrust),Math.Min(upThrust,downThrust))/_gridMass);}void
SetDampenerState(bool enabled)=>ShipController.DampenersOverride=enabled;void OnStageChanged(){_thrustController.ResetThrustOverrides();
ResetGyroOverride();SetDampenerState(false);_lastForwardSpeedDuringAccel=0;_lastForwardThrustRatioDuringAccel=0;if(savePersistentData){
_config.PersistStateData=$"{NavModeEnum.Cruise}|{DesiredSpeed}|{Stage}";_program.Me.CustomData=_config.ToString();}}public void
Terminate(string reason){_terminateReason=reason;if(ShipController.GetShipSpeed()<TARGET_REACHED_SPEED){ShipController.
DampenersOverride=true;}_thrustController.ResetThrustOverrides();ResetGyroOverride();CruiseTerminated.Invoke(this,reason);}public void
Abort(){Stage=CruiseStage.Aborted;Terminate("Aborted");}protected override void OnNoFunctionalGyrosLeft()=>Terminate(
"No functional gyros found");}public class Retrograde:Orient{const double TERMINATE_SPEED=5;public override string Name=>nameof(Retrograde);public
Retrograde(IAimController aimControl,IMyShipController controller,IList<IMyGyro>gyros):base(aimControl,controller,gyros){}public
override void Run(){Vector3D shipVelocity=ShipController.GetShipVelocities().LinearVelocity;if(shipVelocity.LengthSquared()<=
TERMINATE_SPEED*TERMINATE_SPEED){Terminate($"Speed is less than {TERMINATE_SPEED:0.#} m/s");return;}Orient(-shipVelocity);}}public
class SpeedMatch:ICruiseController{enum TargetAcquisitionMode{None=0,AiFocus=1,SortedThreat=2,Obstruction=3,}public event
CruiseTerminateEventDelegate CruiseTerminated;public string Name=>nameof(SpeedMatch);public IMyShipController ShipController{get;set;}public double
relativeSpeedThreshold=0.01;long targetEntityId;WcPbApi wcApi;IMyTerminalBlock pb;VariableThrustController thrustController;int counter=0;
Dictionary<MyDetectedEntityInfo,float>threats=new Dictionary<MyDetectedEntityInfo,float>();List<MyDetectedEntityInfo>obstructions=
new List<MyDetectedEntityInfo>();Vector3D relativeVelocity;float gridMass;MyDetectedEntityInfo?target;TargetAcquisitionMode
targetInfoMode;public SpeedMatch(long targetEntityId,WcPbApi wcApi,IMyShipController shipController,IMyTerminalBlock programmableBlock
,VariableThrustController thrustController){this.targetEntityId=targetEntityId;this.wcApi=wcApi;this.ShipController=
shipController;this.pb=programmableBlock;this.thrustController=thrustController;this.gridMass=ShipController.CalculateShipMass().
PhysicalMass;}public void AppendStatus(StringBuilder strb){if(target.HasValue){strb.AppendLine($"{target.Value.Name,24}");strb.
AppendLine($"  Target Velocity {target.Value.Velocity.Length(),13:0.0 m/s}");strb.AppendLine(
$"  Relative Velocity {relativeVelocity.Length(),11:0.0 m/s}");strb.AppendLine($"  Mode {targetInfoMode,24}");}else{strb.AppendLine($"TargetId: {targetEntityId}");strb.AppendLine(
"Error: Target Not Detected");}strb.AppendLine($"Config ------------------------");strb.AppendLine(
$"  Max Thrust {thrustController.MaxForwardThrustRatio,18:0 %}");}bool TryGetTarget(out MyDetectedEntityInfo?target,bool counter30){target=null;try{var aifocus=wcApi.GetAiFocus(pb.
EntityId);if(aifocus?.EntityId==targetEntityId){target=aifocus.Value;targetInfoMode=TargetAcquisitionMode.AiFocus;return true;}
else{MyDetectedEntityInfo?ent=null;wcApi.GetSortedThreats(pb,threats);foreach(var threat in threats.Keys){if(threat.EntityId
==targetEntityId){ent=threat;targetInfoMode=TargetAcquisitionMode.SortedThreat;break;}}threats.Clear();if(ent.HasValue){
target=ent.Value;return true;}}if(counter30){MyDetectedEntityInfo?ent=null;obstructions.Clear();wcApi.GetObstructions(pb,
obstructions);foreach(var obs in obstructions){if(obs.EntityId==targetEntityId){ent=obs;targetInfoMode=TargetAcquisitionMode.
Obstruction;break;}}if(ent.HasValue){target=ent.Value;return true;}}targetInfoMode=TargetAcquisitionMode.None;return false;}catch{
return false;}}public void Run(){counter++;bool counter10=counter%10==0;bool counter30=counter%30==0;if(counter10){
ShipController.DampenersOverride=false;target=null;if(!TryGetTarget(out target,counter30)){thrustController.ResetThrustOverrides();
return;}thrustController.UpdateThrusts();}if(counter30&&counter%60==0){gridMass=ShipController.CalculateShipMass().
PhysicalMass;}if(target.HasValue&&counter%5==0){relativeVelocity=target.Value.Velocity-ShipController.GetShipVelocities().
LinearVelocity;Vector3 relativeVelocityLocal=Vector3D.TransformNormal(relativeVelocity,MatrixD.Transpose(ShipController.WorldMatrix));
Vector3 thrustAmount=-relativeVelocityLocal*10*gridMass;Vector3 input=ShipController.MoveIndicator;thrustAmount=new Vector3D(
Math.Abs(input.X)<=0.01?thrustAmount.X:0,Math.Abs(input.Y)<=0.01?thrustAmount.Y:0,Math.Abs(input.Z)<=0.01?thrustAmount.Z:0);
thrustController.SetThrusts(thrustAmount);}}public void Abort()=>Terminate("Aborted");public void Terminate(string reason){
thrustController.ResetThrustOverrides();CruiseTerminated.Invoke(this,reason);}}public class VariableThrustController{public float
MaxForwardThrustRatio{get{return _maxForwardThrustRatio;}set{value=MathHelper.Saturate(value);if(_maxForwardThrustRatio!=value){
_maxForwardThrustRatio=value;UpdateThrusts();}}}public IMyShipController ShipController=>_shipController;public readonly Dictionary<Direction,
List<IMyThrust>>Thrusters;float _maxForwardThrustRatio=1f;private readonly IMyShipController _shipController;private
readonly List<IMyThrust>[]_thrusters;private readonly double[]_thrusts=new double[6];float rightThrustInv,leftThrustInv,
upThrustInv,downThrustInv,backThrustInv,forwardThrustInv;public VariableThrustController(Dictionary<Direction,List<IMyThrust>>
thrusters,IMyShipController shipController){this.Thrusters=thrusters;this._shipController=shipController;_thrusters=new[]{
thrusters[(Direction)0],thrusters[(Direction)1],thrusters[(Direction)2],thrusters[(Direction)3],thrusters[(Direction)4],thrusters
[(Direction)5],};}public double GetThrustInDirection(Direction direction){return _thrusts[(int)direction];}public void
UpdateThrusts(){for(int dir=5;dir>=0;dir--){var thrusters=_thrusters[dir];double total=0;for(int i=thrusters.Count-1;i>=0;i--){var
thruster=thrusters[i];if(thruster.IsWorking){total+=thruster.MaxEffectiveThrust;}}_thrusts[dir]=total;switch((Direction)dir){
case Direction.Right:rightThrustInv=(float)(1.0/total);break;case Direction.Left:leftThrustInv=(float)(1.0/total);break;case
Direction.Up:upThrustInv=(float)(1.0/total);break;case Direction.Down:downThrustInv=(float)(1.0/total);break;case Direction.
Forward:forwardThrustInv=(float)(1.0/total);break;case Direction.Backward:backThrustInv=(float)(1.0/total);break;}}}public void
DampenAllDirections(Vector3D shipVelocity,float gridMass,float ups){Vector3 localVelocity=Vector3D.TransformNormal(shipVelocity,MatrixD.
Transpose(_shipController.WorldMatrix));SetThrusts(localVelocity*gridMass*ups);}public void DampenAllDirections(Vector3D
shipVelocity,Vector3D gravity,float gridMass,float ups){MatrixD transposedShipMatrix=MatrixD.Transpose(_shipController.WorldMatrix);
Vector3D localVelocity,localGravity;Vector3D.TransformNormal(ref shipVelocity,ref transposedShipMatrix,out localVelocity);
Vector3D.TransformNormal(ref gravity,ref transposedShipMatrix,out localGravity);Vector3 thrustAmount=(localVelocity*ups+
localGravity)*gridMass;float right=thrustAmount.X<0?-thrustAmount.X:0;float left=thrustAmount.X>0?thrustAmount.X:0;float up=
thrustAmount.Y<0?-thrustAmount.Y:0;float down=thrustAmount.Y>0?thrustAmount.Y:0;float backward=thrustAmount.Z<0?-thrustAmount.Z:0;
float forward=thrustAmount.Z>0?thrustAmount.Z:0;SetSideThrusts(left,right,up,down);backward*=backThrustInv;forward=Math.Min(
forward*forwardThrustInv,_maxForwardThrustRatio+Math.Max(0,(float)localGravity.Z*gridMass*forwardThrustInv));backward=
MathHelper.Saturate(backward);forward=MathHelper.Saturate(forward);var backwardThrusters=_thrusters[(int)Direction.Backward];for(
int i=backwardThrusters.Count-1;i>=0;i--){if(backwardThrusters[i].ThrustOverridePercentage!=backward)backwardThrusters[i].
ThrustOverridePercentage=backward;}var forwardThrusters=_thrusters[(int)Direction.Forward];for(int i=forwardThrusters.Count-1;i>=0;i--){if(
forwardThrusters[i].ThrustOverridePercentage!=forward)forwardThrusters[i].ThrustOverridePercentage=forward;}}public void SetThrusts(
Vector3 thrustAmount){float right=thrustAmount.X<0?-thrustAmount.X:0;float left=thrustAmount.X>0?thrustAmount.X:0;float up=
thrustAmount.Y<0?-thrustAmount.Y:0;float down=thrustAmount.Y>0?thrustAmount.Y:0;float backward=thrustAmount.Z<0?-thrustAmount.Z:0;
float forward=thrustAmount.Z>0?thrustAmount.Z:0;SetSideThrusts(left,right,up,down);backward*=backThrustInv;forward=Math.Min(
forward*forwardThrustInv,_maxForwardThrustRatio);backward=MathHelper.Saturate(backward);forward=MathHelper.Saturate(forward);
var backwardThrusters=_thrusters[(int)Direction.Backward];for(int i=backwardThrusters.Count-1;i>=0;i--){if(
backwardThrusters[i].ThrustOverridePercentage!=backward)backwardThrusters[i].ThrustOverridePercentage=backward;}var forwardThrusters=
_thrusters[(int)Direction.Forward];for(int i=forwardThrusters.Count-1;i>=0;i--){if(forwardThrusters[i].ThrustOverridePercentage!=
forward)forwardThrusters[i].ThrustOverridePercentage=forward;}}public void SetSideThrusts(float left,float right,float up,float
down){right=MathHelper.Saturate(right*rightThrustInv);left=MathHelper.Saturate(left*leftThrustInv);up=MathHelper.Saturate(up
*upThrustInv);down=MathHelper.Saturate(down*downThrustInv);var rightThrusters=_thrusters[(int)Direction.Right];for(int i=
rightThrusters.Count-1;i>=0;i--){if(rightThrusters[i].ThrustOverridePercentage!=right)rightThrusters[i].ThrustOverridePercentage=right
;}var leftThrusters=_thrusters[(int)Direction.Left];for(int i=leftThrusters.Count-1;i>=0;i--){if(leftThrusters[i].
ThrustOverridePercentage!=left)leftThrusters[i].ThrustOverridePercentage=left;}var upThrusters=_thrusters[(int)Direction.Up];for(int i=
upThrusters.Count-1;i>=0;i--){if(upThrusters[i].ThrustOverridePercentage!=up)upThrusters[i].ThrustOverridePercentage=up;}var
downThrusters=_thrusters[(int)Direction.Down];for(int i=downThrusters.Count-1;i>=0;i--){if(downThrusters[i].ThrustOverridePercentage
!=down)downThrusters[i].ThrustOverridePercentage=down;}}public void ResetThrustOverrides(){for(int dir=5;dir>=0;dir--){var
thrusters=_thrusters[dir];for(int i=thrusters.Count-1;i>=0;i--){thrusters[i].ThrustOverridePercentage=0;}}}public void
SetDampenerState(bool enabled)=>_shipController.DampenersOverride=enabled;}public sealed class Profiler{public double RunningAverageMs{
get;private set;}double AverageRuntimeMs{get{double sum=runtimeCollection[0];for(int i=1;i<BufferSize;i++){sum+=
runtimeCollection[i];}return sum*bufferSizeInv;}}public double MaxRuntimeMs{get{double max=runtimeCollection[0];for(int i=1;i<BufferSize;
i++){if(runtimeCollection[i]>max){max=runtimeCollection[i];}}return max;}}public double MaxRuntimeMsFast{get;private set;
}public double MinRuntimeMs{get{double min=runtimeCollection[0];for(int i=1;i<BufferSize;i++){if(runtimeCollection[i]<min
){min=runtimeCollection[i];}}return min;}}double bufferSizeInv;IMyGridProgramRuntimeInfo runtimeInfo;double[]
runtimeCollection;int counter=0;public readonly int BufferSize;public Profiler(Program Program,int BufferSize=300){this.runtimeInfo=
Program.Runtime;this.MaxRuntimeMsFast=Program.Runtime.LastRunTimeMs;this.BufferSize=MathHelper.Clamp(BufferSize,1,int.MaxValue)
;this.bufferSizeInv=1.0/this.BufferSize;this.runtimeCollection=new double[this.BufferSize];this.runtimeCollection[counter
]=Program.Runtime.LastRunTimeMs;this.counter++;}public void Run(){RunningAverageMs-=runtimeCollection[counter]*
bufferSizeInv;RunningAverageMs+=runtimeInfo.LastRunTimeMs*bufferSizeInv;runtimeCollection[counter]=runtimeInfo.LastRunTimeMs;if(
runtimeInfo.LastRunTimeMs>MaxRuntimeMsFast){MaxRuntimeMsFast=runtimeInfo.LastRunTimeMs;}counter++;if(counter>=BufferSize){counter=0
;RunningAverageMs=AverageRuntimeMs;MaxRuntimeMsFast=runtimeInfo.LastRunTimeMs;}}}public static class Utils{public static
Vector3D SafeNormalize(this Vector3D a){if(Vector3D.IsZero(a))return Vector3D.Zero;if(Vector3D.IsUnit(ref a))return a;return
Vector3D.Normalize(a);}public static StringBuilder AppendTime(this StringBuilder strb,double totalSeconds){int minutes=(int)
totalSeconds/60;totalSeconds%=60;strb.Append(minutes).Append(":").Append(totalSeconds.ToString("00.0"));return strb;}public static
string MinuteAndSeconds(double totalSeconds){return$"{(int)totalSeconds/60}:{totalSeconds%60:00.0}";}public static Vector3D
Normalize(ref Vector3D vec,out double length){length=vec.Length();return length<0.00001?Vector3D.Zero:(vec/length);}public static
double GetWorldMaxSpeed(this Program program){return program.Me.CubeGrid.GridSizeEnum==MyCubeSize.Large?program.World.
LargeShipMaxSpeed:program.World.SmallShipMaxSpeed;}