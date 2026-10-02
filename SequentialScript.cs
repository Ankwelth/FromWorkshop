// R e a d m e
// -----------
// 
// V1.3-alpha
// See more information in the following link:
// https://github.com/space-engineers-hf/SequentialScript

    static readonly bool DEBUG_IN_SCREEN = false;
    static readonly UpdateFrequency UPDATE_FREQUENCY = UpdateFrequency.Update10; // Update1, Update10, Update100
    static readonly int UPDATE_TICKS = 0; // Very slow mode multiplier (for debug)


    /* ----------------------------------------------------------------------------------- */
    /* --- ¡¡¡IMPORTANT!!! Do not change anything below this line. --- */
    /* ----------------------------------------------------------------------------------- */

    DateTime _momento;IList<IMyTerminalBlock>_terminalBlocks;IList<IMyBlockGroup>_terminalGroups;IDictionary<string,
IEnumerable<IMyTerminalBlock>>_blocksDictionary;IDictionary<string,ICommandInstruction>_commands;InstructionCommand _command;
IEnumerable<Task>_tasks;ConditionCommandInstruction _conditionCommand;int _checkIndex;int ticks;public
 Program
(){Runtime.UpdateFrequency=UpdateFrequency.None;}public void
 Main
(string argument,UpdateType updateSource){AdvancedEchoReset();if(CommonHelper.IsCycle(updateSource)){var debug=new
StringBuilder();var now=DateTime.UtcNow;try{if(UPDATE_TICKS==0||ticks%UPDATE_TICKS==0){if(_terminalBlocks==null||_terminalGroups==
null){AdvancedEcho($"Getting terminal blocks",append:true);_terminalBlocks=GridTerminalSystem.GetBlocks();AdvancedEcho(
$" - OK",append:true);AdvancedEcho($"Getting terminal groups",append:true);_terminalGroups=GridTerminalSystem.GetBlockGroups();
AdvancedEcho($" - OK",append:true);}else if(_blocksDictionary==null){var blockNames=_commands.Values.OfType<InstructionCommand>().
SelectMany(cmd=>cmd.Body).SelectMany(body=>body.Instructions).Where(instruction=>instruction.BlockName!=null).Select(instruction=>
instruction.BlockName).Distinct();AdvancedEcho($"Building dictionary",append:true);_blocksDictionary=Helper.CreateBlockDictionary(
blockNames,_terminalBlocks,_terminalGroups);AdvancedEcho($" - OK",append:true);}else if(_command!=null){IEnumerable<Task>thenTasks
;AdvancedEcho($"Running {_command.CommandName}",append:true);thenTasks=Tasks.CreateTasks(_command.Body,_blocksDictionary)
;StartTasks(thenTasks,$"{debug}\nStarted.",appendMessage:true);}else if(_tasks!=null){IEnumerable<Task>tasksRunning;
tasksRunning=_tasks.Run(debug);if(tasksRunning.Any()){AdvancedEcho($"{debug}",append:true);AdvancedEcho(
$"Running tasks: {(DateTime.UtcNow-now).TotalMilliseconds:N0}",append:true);}else{EndCycle();AdvancedEcho("Done.");}}else if(_conditionCommand!=null){CheckNextCondition();
AdvancedEcho($"Condition command: {(DateTime.UtcNow-now).TotalMilliseconds:N0}",append:true);}else{throw new Exception(
"Invalid state.");}ticks=0;}ticks++;}catch(Exception ex){EndCycle();AdvancedEcho($"ERROR: {ex.Message}",append:true);}}else{var debug=
new StringBuilder();ICommandInstruction command;try{_tasks?.Cancel();_commands=InstructionParser.Parse(Me.CustomData);if(
string.IsNullOrWhiteSpace(argument)){}else if(_commands.TryGetValue(argument,out command)){_terminalBlocks=null;
_terminalGroups=null;_blocksDictionary=null;if(command is InstructionCommand){StartCommand((InstructionCommand)command,
$"Command '{command.CommandName}' started.");}else if(command is ConditionCommandInstruction){StartCheck((ConditionCommandInstruction)command,
$"Checking condition '{command.CommandName}'...");}}else{AdvancedEcho($"ERROR: Command not found: '{argument}'");}}catch(Exception ex){debug.AppendLine(
$"ERROR: {ex.Message}");}finally{AdvancedEcho($"{debug}");}}}void StartCommand(InstructionCommand command,string message="Command started.",
bool appendMessage=false){_command=command;_tasks=null;_conditionCommand=null;_checkIndex=0;Runtime.UpdateFrequency=
UPDATE_FREQUENCY;ticks=1;AdvancedEcho(message,appendMessage);}void StartTasks(IEnumerable<Task>tasks,string message="Task started.",bool
appendMessage=false){_command=null;_tasks=tasks;_conditionCommand=null;Runtime.UpdateFrequency=UPDATE_FREQUENCY;ticks=1;AdvancedEcho(
message,appendMessage);}void StartCheck(ConditionCommandInstruction conditionCommand,string message="Checking condition...",
bool appendMessage=false){_command=null;_tasks=null;_conditionCommand=conditionCommand;_checkIndex=0;Runtime.UpdateFrequency
=UPDATE_FREQUENCY;ticks=1;AdvancedEcho(message,appendMessage);}void CheckNextCondition(){var debug=new StringBuilder();if
(_conditionCommand!=null&&_checkIndex<_commands.Count){bool positive=false;var condition=_conditionCommand.Body[
_checkIndex];debug.AppendLine($"Index condition: {_checkIndex}");if(string.IsNullOrEmpty(condition.When)){debug.AppendLine(
$"Condition name: ELSE");positive=true;}else{InstructionCommand whenCommand;IEnumerable<Task>whenTasks;whenCommand=(InstructionCommand)
_commands[condition.When];debug.AppendLine($"Condition name: {whenCommand.CommandName}");whenTasks=Tasks.CreateTasks(whenCommand.
Body,_blocksDictionary);if(Tasks.IsCompleted(whenTasks,debug)){positive=true;}}AdvancedEcho(debug.ToString(),true);if(
positive){InstructionCommand thenCommand;thenCommand=(InstructionCommand)_commands[condition.Then.Single()];StartCommand(
thenCommand,$"Command started: {thenCommand.CommandName}.",appendMessage:true);}else{_checkIndex++;}}else{_conditionCommand=null;
_checkIndex=0;}}void EndCycle(){_tasks=null;Runtime.UpdateFrequency=UpdateFrequency.None;}void AdvancedEchoReset(){_momento=
DateTime.UtcNow;}void AdvancedEcho(string message,bool append=false){string value=message;if(append){var builder=new
StringBuilder(Me.CustomInfo);builder.Append(value);message=builder.ToString();}message=
$"| Elapsed {(DateTime.UtcNow-_momento).TotalMilliseconds:00}ms |\n{message}";Echo(message);if(DEBUG_IN_SCREEN){var displays=DisplayHelper.GetTextSurfaces(new[]{Me});var display=displays.First().
TextSurface;display.ContentType=ContentType.TEXT_AND_IMAGE;display.WriteText(message);}}
}
abstract class ActionProfile<TMyTerminalBlock>:IActionProfile where TMyTerminalBlock:class,IMyTerminalBlock{public
abstract IEnumerable<string>ActionNames{get;}public virtual string GroupName=>ActionNames.First();public abstract Action<
TMyTerminalBlock,IDictionary<string,string>>OnActionCallback{get;}public abstract Func<TMyTerminalBlock,IDictionary<string,string>,bool>
IsCompleteCallback{get;}protected virtual string GetCompletionDetails(TMyTerminalBlock block,IDictionary<string,string>arguments)=>null;
Action<IMyTerminalBlock,IDictionary<string,string>>IActionProfile.OnActionCallback=>(block,args)=>OnActionCallback(
GetMyTerminalBlock(block),args);Func<IMyTerminalBlock,IDictionary<string,string>,bool>IActionProfile.IsCompleteCallback=>(block,args)=>
IsCompleteCallback(GetMyTerminalBlock(block),args);string IActionProfile.GetCompletionDetails(IMyTerminalBlock block,IDictionary<string,
string>arguments)=>GetCompletionDetails(GetMyTerminalBlock(block),arguments);bool IActionProfile.IsAssignableFrom(
IMyTerminalBlock block)=>(block is TMyTerminalBlock);TMyTerminalBlock GetMyTerminalBlock(IMyTerminalBlock block){if(block==null){throw
new NullReferenceException("Cannot run any action in a null block.");}else{var castBlock=block as TMyTerminalBlock;if(
castBlock==null){throw new InvalidCastException(
$"Cannot block type '{block.GetType().Name}' does not support the action profile '{this.GetType().Name}'.");}else{return castBlock;}}}}sealed class ActionProfileAirVentDepressurize:ActionProfile<IMyAirVent>{public override
IEnumerable<string>ActionNames=>new[]{"Depressurize"};public override string GroupName=>"Pressurization";public override Action<
IMyAirVent,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.Depressurize=true;public override Func<IMyAirVent,
IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block.GetOxygenLevel()==0;}sealed class
ActionProfileAirVentPressurize:ActionProfile<IMyAirVent>{public override IEnumerable<string>ActionNames=>new[]{"Pressurize"};public override string
GroupName=>"Pressurization";public override Action<IMyAirVent,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.
Depressurize=false;public override Func<IMyAirVent,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block.Status==
VentStatus.Pressurized;}sealed class ActionProfileBatteryAuto:ActionProfile<IMyBatteryBlock>{public override IEnumerable<string>
ActionNames=>new[]{"Auto"};public override string GroupName=>"ChargeMode";public override Action<IMyBatteryBlock,IDictionary<string
,string>>OnActionCallback=>(block,args)=>block.ChargeMode=ChargeMode.Auto;public override Func<IMyBatteryBlock,
IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block.ChargeMode==ChargeMode.Auto;}sealed class
ActionProfileBatteryDischarge:ActionProfile<IMyBatteryBlock>{public override IEnumerable<string>ActionNames=>new[]{"Discharge"};public override
string GroupName=>"ChargeMode";public override Action<IMyBatteryBlock,IDictionary<string,string>>OnActionCallback=>(block,args
)=>block.ChargeMode=ChargeMode.Discharge;public override Func<IMyBatteryBlock,IDictionary<string,string>,bool>
IsCompleteCallback=>(block,args)=>(block.ChargeMode==ChargeMode.Discharge&&block.CurrentStoredPower==0);}sealed class
ActionProfileBatteryRecharge:ActionProfile<IMyBatteryBlock>{public override IEnumerable<string>ActionNames=>new[]{"Recharge"};public override string
GroupName=>"ChargeMode";public override Action<IMyBatteryBlock,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.
ChargeMode=ChargeMode.Recharge;public override Func<IMyBatteryBlock,IDictionary<string,string>,bool>IsCompleteCallback=>(block,
args)=>(block.ChargeMode==ChargeMode.Recharge&&block.CurrentStoredPower==block.MaxStoredPower);}sealed class
ActionProfileConnectorLock:ActionProfile<IMyShipConnector>{public override IEnumerable<string>ActionNames=>new[]{"Lock","Connect"};public override
string GroupName=>"Status";public override Action<IMyShipConnector,IDictionary<string,string>>OnActionCallback=>(block,args)=>
block.Connect();public override Func<IMyShipConnector,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block
.Status==MyShipConnectorStatus.Connected;}sealed class ActionProfileConnectorUnlock:ActionProfile<IMyShipConnector>{
public override IEnumerable<string>ActionNames=>new[]{"Unlock","Disconnect"};public override string GroupName=>"Status";public
override Action<IMyShipConnector,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.Disconnect();public override
Func<IMyShipConnector,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>{bool result=true;string value;
result=(block.Status!=MyShipConnectorStatus.Connected);if(args.TryGetValue("FULL",out value)){result&=(block.Status==
MyShipConnectorStatus.Unconnected);}return result;};}sealed class ActionProfileDoorClose:ActionProfile<IMyDoor>{public override IEnumerable<
string>ActionNames=>new string[]{"Close"};public override string GroupName=>"Status";public override Action<IMyDoor,
IDictionary<string,string>>OnActionCallback=>(block,args)=>block.CloseDoor();public override Func<IMyDoor,IDictionary<string,string
>,bool>IsCompleteCallback=>(block,args)=>block.Status==DoorStatus.Closed;}sealed class ActionProfileDoorOpen:
ActionProfile<IMyDoor>{public override IEnumerable<string>ActionNames=>new[]{"Open"};public override string GroupName=>"Status";
public override Action<IMyDoor,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.OpenDoor();public override
Func<IMyDoor,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block.Status==DoorStatus.Open;}sealed class
ActionProfileDoorToggle:ActionProfile<IMyDoor>{public override IEnumerable<string>ActionNames=>new string[]{"Toggle"};public override string
GroupName=>"Status";public override Action<IMyDoor,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.ToggleDoor();
public override Func<IMyDoor,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block.Status==DoorStatus.Open||
block.Status==DoorStatus.Closed;}sealed class ActionProfileFunctionalBlockDisable:ActionProfile<IMyFunctionalBlock>{public
override IEnumerable<string>ActionNames=>new[]{"Disable","Off"};public override string GroupName=>"OnOff";public override Action
<IMyFunctionalBlock,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.Enabled=false;public override Func<
IMyFunctionalBlock,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>!block.Enabled;}sealed class
ActionProfileFunctionalBlockEnable:ActionProfile<IMyFunctionalBlock>{public override IEnumerable<string>ActionNames=>new[]{"Enable","On"};public override
string GroupName=>"OnOff";public override Action<IMyFunctionalBlock,IDictionary<string,string>>OnActionCallback=>(block,args)
=>block.Enabled=true;public override Func<IMyFunctionalBlock,IDictionary<string,string>,bool>IsCompleteCallback=>(block,
args)=>block.Enabled;}sealed class ActionProfileGasTankAuto:ActionProfile<IMyGasTank>{public override IEnumerable<string>
ActionNames=>new[]{"Auto"};public override string GroupName=>"Stockpile";public override Action<IMyGasTank,IDictionary<string,
string>>OnActionCallback=>(block,args)=>block.Stockpile=false;public override Func<IMyGasTank,IDictionary<string,string>,bool>
IsCompleteCallback=>(block,args)=>!block.Stockpile;}sealed class ActionProfileGasTankStockpile:ActionProfile<IMyGasTank>{public override
IEnumerable<string>ActionNames=>new[]{"Stockpile"};public override string GroupName=>"Stockpile";public override Action<IMyGasTank,
IDictionary<string,string>>OnActionCallback=>(block,args)=>block.Stockpile=true;public override Func<IMyGasTank,IDictionary<string,
string>,bool>IsCompleteCallback=>(block,args)=>block.Stockpile&&block.FilledRatio==1;}sealed class ActionProfileLcdDisplay:
ActionProfile<IMyTerminalBlock>{public override IEnumerable<string>ActionNames=>new[]{"Display"};public override Action<
IMyTerminalBlock,IDictionary<string,string>>OnActionCallback=>(block,args)=>{if(block is IMyTextSurfaceProvider){var index=
GetTextSurfaceIndex(args);var textSurface=GetTextSurface(block,index);string value;if(args.TryGetValue("BACKGROUND",out value)){textSurface
.BackgroundColor=Helper.ParseColor(value);}if(args.TryGetValue("COLOR",out value)){textSurface.FontColor=Helper.
ParseColor(value);}if(args.TryGetValue("TEXT",out value)){textSurface.WriteText(value.Replace("\\n","\n"));}}else{throw new
NotSupportedException($"Block '{block.DisplayNameText}' not supports displays.");}};public override Func<IMyTerminalBlock,IDictionary<string,
string>,bool>IsCompleteCallback=>(block,args)=>{if(block is IMyTextSurfaceProvider){bool result=true;var index=
GetTextSurfaceIndex(args);var textSurface=GetTextSurface(block,index);string value;if(args.TryGetValue("BACKGROUND",out value)){result&=(
textSurface.BackgroundColor==Helper.ParseColor(value));}if(args.TryGetValue("COLOR",out value)){result&=(textSurface.FontColor==
Helper.ParseColor(value));}if(args.TryGetValue("TEXT",out value)){result&=(textSurface.GetText()==value.Replace("\\n","\n"));}
return result;}else{throw new NotSupportedException($"Block '{block.DisplayNameText}' not supports displays.");}};static int?
GetTextSurfaceIndex(IDictionary<string,string>args){int?result=null;string stringIndex;if(args.TryGetValue("INDEX",out stringIndex)){int
index;if(int.TryParse(stringIndex,out index)){result=index;}else{throw new FormatException(
$"Invalid format for '/INDEX:<number>. Value '{stringIndex}' is not numeric.");}}return result;}static IMyTextSurface GetTextSurface(IMyTerminalBlock block,int?index){IMyTextSurface result;var
textSurfaces=GetTextSurfaces(block).Select((val,i)=>new{Index=i,TextSurface=val});switch(textSurfaces.Count()){case 0:throw new
NotSupportedException($"Block '{block.DisplayNameText}' is not a supported display block.");case 1:var first=textSurfaces.First();if(index!=
null&&first.Index!=index){throw new ArgumentException(
$"Display index '{index}' is not available for block '{block.DisplayNameText}'.");}else{result=first.TextSurface;}break;default:if(index==null){throw new ArgumentException(
"Missing parameter '/INDEX:<number>'");}else{var selected=textSurfaces.SingleOrDefault(x=>x.Index==index.Value);if(selected==null){throw new
ArgumentException($"Display index '{index}' is not available for block '{block.DisplayNameText}'.");}else{result=selected.TextSurface;}}
break;}return result;}static IEnumerable<IMyTextSurface>GetTextSurfaces(IMyTerminalBlock block){var provider=(
IMyTextSurfaceProvider)block;var textSurfaces=new List<IMyTextSurface>();for(int i=0;i<provider.SurfaceCount;i++){textSurfaces.Add(provider.
GetSurface(i));}return textSurfaces;}}sealed class ActionProfileLightSet:ActionProfile<IMyLightingBlock>{public override
IEnumerable<string>ActionNames=>new[]{"Set"};public override Action<IMyLightingBlock,IDictionary<string,string>>OnActionCallback=>(
block,args)=>{string value;if(args.TryGetValue("COLOR",out value)){block.Color=Helper.ParseColor(value);}};public override
Func<IMyLightingBlock,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>{bool result=true;string value;if(
args.TryGetValue("COLOR",out value)){result&=(block.Color==Helper.ParseColor(value));}return result;};}sealed class
ActionProfileMergeBlockDisable:ActionProfile<IMyShipMergeBlock>{public override IEnumerable<string>ActionNames=>new[]{"Disable","Off","Unlock"};public
override string GroupName=>"State";public override Action<IMyShipMergeBlock,IDictionary<string,string>>OnActionCallback=>(block,
args)=>block.Enabled=false;public override Func<IMyShipMergeBlock,IDictionary<string,string>,bool>IsCompleteCallback=>(block
,args)=>block.State!=MergeState.Locked;}sealed class ActionProfileMergeBlockEnable:ActionProfile<IMyShipMergeBlock>{
public override IEnumerable<string>ActionNames=>new[]{"Enable","On","Lock"};public override string GroupName=>"State";public
override Action<IMyShipMergeBlock,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.Enabled=true;public override
Func<IMyShipMergeBlock,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block.State==MergeState.Locked;}
sealed class ActionProfileMotorStatorBack:ActionProfile<IMyMotorStator>{public override IEnumerable<string>ActionNames=>new[]{
"Back"};public override string GroupName=>"Direction";public override Action<IMyMotorStator,IDictionary<string,string>>
OnActionCallback=>(block,args)=>block.TargetVelocityRPM=Math.Abs(block.TargetVelocityRPM)*-1;public override Func<IMyMotorStator,
IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block.TargetVelocityRad<0&&(block.LowerLimitRad==float.MinValue||
block.Angle<=block.LowerLimitRad);}sealed class ActionProfileMotorStatorDetach:ActionProfile<IMyMotorStator>{public override
IEnumerable<string>ActionNames=>new[]{"Detach"};public override Action<IMyMotorStator,IDictionary<string,string>>OnActionCallback=>
(block,args)=>block.Detach();public override Func<IMyMotorStator,IDictionary<string,string>,bool>IsCompleteCallback=>(
block,args)=>!block.IsAttached;}sealed class ActionProfileMotorStatorForward:ActionProfile<IMyMotorStator>{public override
IEnumerable<string>ActionNames=>new[]{"Forward"};public override string GroupName=>"Direction";public override Action<
IMyMotorStator,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.TargetVelocityRPM=Math.Abs(block.TargetVelocityRPM);
public override Func<IMyMotorStator,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block.TargetVelocityRad>
0&&(block.UpperLimitRad==float.MaxValue||block.Angle>=block.UpperLimitRad);}sealed class ActionProfileMotorStatorSet:
ActionProfile<IMyMotorStator>{public override IEnumerable<string>ActionNames=>new[]{"Set"};public override Action<IMyMotorStator,
IDictionary<string,string>>OnActionCallback=>(block,args)=>{string value;if(args.TryGetValue("MAX",out value)){block.UpperLimitDeg=
ParseValue(value)??float.MaxValue;}if(args.TryGetValue("MIN",out value)){block.LowerLimitDeg=ParseValue(value)??float.MinValue;}};
public override Func<IMyMotorStator,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>{bool result=true;string
value;if(args.TryGetValue("MAX",out value)){result&=(block.UpperLimitDeg==(ParseValue(value)??float.MaxValue));}if(args.
TryGetValue("MIN",out value)){result&=(block.LowerLimitDeg==(ParseValue(value)??float.MinValue));}return result;};static float?
ParseValue(string value){float?result;if(value.Equals("null",StringComparison.OrdinalIgnoreCase)){result=null;}else{result=float.
Parse(value);}return result;}}sealed class ActionProfilePistonBaseExtend:ActionProfile<IMyPistonBase>{public override
IEnumerable<string>ActionNames=>new[]{"Extend"};public override string GroupName=>"Direction";public override Action<IMyPistonBase,
IDictionary<string,string>>OnActionCallback=>(block,args)=>block.Extend();public override Func<IMyPistonBase,IDictionary<string,
string>,bool>IsCompleteCallback=>(block,args)=>block.Status==PistonStatus.Extended||(block.MaxLimit-block.CurrentPosition)<
0.05;protected override string GetCompletionDetails(IMyPistonBase block,IDictionary<string,string>arguments)=>
$"Position: {block.CurrentPosition:N2} ({(block.MaxLimit-block.CurrentPosition):N2}); Status: {block.Status}";}sealed class ActionProfilePistonBaseRetract:ActionProfile<IMyPistonBase>{public override IEnumerable<string>
ActionNames=>new[]{"Retract"};public override string GroupName=>"Direction";public override Action<IMyPistonBase,IDictionary<string
,string>>OnActionCallback=>(block,args)=>block.Retract();public override Func<IMyPistonBase,IDictionary<string,string>,
bool>IsCompleteCallback=>(block,args)=>block.Status==PistonStatus.Retracted||(block.CurrentPosition-block.MinLimit)<0.05;
protected override string GetCompletionDetails(IMyPistonBase block,IDictionary<string,string>arguments)=>
$"Position: {block.CurrentPosition:N2} ({(block.CurrentPosition-block.MinLimit):N2}); Status: {block.Status}";}sealed class ActionProfilePistonBaseReverse:ActionProfile<IMyPistonBase>{public override IEnumerable<string>
ActionNames=>new[]{"Reverse"};public override string GroupName=>"Direction";public override Action<IMyPistonBase,IDictionary<string
,string>>OnActionCallback=>(block,args)=>block.Reverse();public override Func<IMyPistonBase,IDictionary<string,string>,
bool>IsCompleteCallback=>(block,args)=>(block.Velocity>=0)?(block.Status==PistonStatus.Extended||(block.MaxLimit-block.
CurrentPosition)<0.05):(block.Status==PistonStatus.Retracted||(block.CurrentPosition-block.MinLimit)<0.05);protected override string
GetCompletionDetails(IMyPistonBase block,IDictionary<string,string>arguments)=>(block.Velocity>=0)?
$"Position: {block.CurrentPosition:N2} ({(block.MaxLimit-block.CurrentPosition):N2}); Status: {block.Status}":$"Position: {block.CurrentPosition:N2} ({(block.CurrentPosition-block.MinLimit):N2}); Status: {block.Status}";}sealed
class ActionProfilePistonBaseSet:ActionProfile<IMyPistonBase>{public override IEnumerable<string>ActionNames=>new[]{"Set"};
public override Action<IMyPistonBase,IDictionary<string,string>>OnActionCallback=>(block,args)=>{string value;if(args.
TryGetValue("MAX",out value)){block.MaxLimit=float.Parse(value);}if(args.TryGetValue("MIN",out value)){block.MinLimit=float.Parse(
value);}};public override Func<IMyPistonBase,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>{bool result=
true;string value;if(args.TryGetValue("MAX",out value)){result&=(block.MaxLimit==float.Parse(value));}if(args.TryGetValue(
"MIN",out value)){result&=(block.MinLimit==float.Parse(value));}return result;};}sealed class
ActionProfileProgrammableBlockRun:ActionProfile<IMyProgrammableBlock>{public override IEnumerable<string>ActionNames=>new[]{"Run"};public override Action
<IMyProgrammableBlock,IDictionary<string,string>>OnActionCallback=>(block,args)=>{string argument;if(args.TryGetValue(
"ARG",out argument)){block.TryRun(argument);}else{block.TryRun(block.TerminalRunArgument);}};public override Func<
IMyProgrammableBlock,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>block.IsRunning;}static class ActionProfiles{static
readonly IList<IActionProfile>_profiles=new List<IActionProfile>(){new ActionProfileFunctionalBlockEnable(),new
ActionProfileFunctionalBlockDisable(),new ActionProfilePistonBaseExtend(),new ActionProfilePistonBaseRetract(),new ActionProfilePistonBaseReverse(),new
ActionProfilePistonBaseSet(),new ActionProfileMergeBlockEnable(),new ActionProfileMergeBlockDisable(),new ActionProfileDoorOpen(),new
ActionProfileDoorClose(),new ActionProfileDoorToggle(),new ActionProfileAirVentPressurize(),new ActionProfileAirVentDepressurize(),new
ActionProfileTimerStart(),new ActionProfileTimerStop(),new ActionProfileTimerTrigger(),new ActionProfileSoundPlay(),new ActionProfileSoundStop(
),new ActionProfileMotorStatorForward(),new ActionProfileMotorStatorBack(),new ActionProfileMotorStatorDetach(),new
ActionProfileMotorStatorSet(),new ActionProfileProgrammableBlockRun(),new ActionProfileBatteryAuto(),new ActionProfileBatteryRecharge(),new
ActionProfileBatteryDischarge(),new ActionProfileConnectorLock(),new ActionProfileConnectorUnlock(),new ActionProfileLightSet(),new
ActionProfileGasTankStockpile(),new ActionProfileGasTankAuto(),new ActionProfileLcdDisplay(),new ActionProfileThrusterSet(),new
ActionProfileWarheadArm(),new ActionProfileWarheadDisarm(),new ActionProfileWarheadDetonate(),new ActionProfileWarheadStart(),new
ActionProfileWarheadStop(),};public static IActionProfile GetActionProfile(IMyTerminalBlock block,string action){IActionProfile actionProfile=
_profiles.LastOrDefault(profile=>profile.IsAssignableFrom(block)&&profile.ActionNames.Contains(action,StringComparer.
OrdinalIgnoreCase));if(actionProfile==null){throw new NullReferenceException(
$"There is no implementation for type '{block.DefinitionDisplayNameText}' -> '{action}' ({block.DisplayNameText}).");}return actionProfile;}}sealed class ActionProfileSoundPlay:ActionProfile<IMySoundBlock>{public override IEnumerable<
string>ActionNames=>new[]{"Play"};public override string GroupName=>"Play";public override Action<IMySoundBlock,IDictionary<
string,string>>OnActionCallback=>(block,args)=>block.Play();public override Func<IMySoundBlock,IDictionary<string,string>,bool
>IsCompleteCallback=>(block,args)=>true;}sealed class ActionProfileSoundStop:ActionProfile<IMySoundBlock>{public override
IEnumerable<string>ActionNames=>new[]{"Stop"};public override string GroupName=>"Play";public override Action<IMySoundBlock,
IDictionary<string,string>>OnActionCallback=>(block,args)=>block.Stop();public override Func<IMySoundBlock,IDictionary<string,
string>,bool>IsCompleteCallback=>(block,args)=>true;}sealed class ActionProfileThrusterSet:ActionProfile<IMyThrust>{public
override IEnumerable<string>ActionNames=>new[]{"Set"};public override Action<IMyThrust,IDictionary<string,string>>
OnActionCallback=>(block,args)=>{string value;if(args.TryGetValue("OVERRIDE",out value)){block.ThrustOverridePercentage=float.Parse(
value);}};public override Func<IMyThrust,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>{bool result=true;
string value;if(args.TryGetValue("OVERRIDE",out value)){result&=(block.ThrustOverridePercentage==float.Parse(value));}return
result;};}sealed class ActionProfileTimerStart:ActionProfile<IMyTimerBlock>{public override IEnumerable<string>ActionNames=>
new[]{"Start"};public override string GroupName=>"Status";public override Action<IMyTimerBlock,IDictionary<string,string>>
OnActionCallback=>(block,args)=>block.StartCountdown();public override Func<IMyTimerBlock,IDictionary<string,string>,bool>
IsCompleteCallback=>(block,args)=>!block.IsCountingDown;protected override string GetCompletionDetails(IMyTimerBlock block,IDictionary<
string,string>arguments)=>$"Time: {TimeSpan.FromSeconds(block.TriggerDelay).ToString(@"hh\:mm\:ss")}";}sealed class
ActionProfileTimerStop:ActionProfile<IMyTimerBlock>{public override IEnumerable<string>ActionNames=>new[]{"Stop"};public override string
GroupName=>"Status";public override Action<IMyTimerBlock,IDictionary<string,string>>OnActionCallback=>(block,args)=>block.
StopCountdown();public override Func<IMyTimerBlock,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>!block.
IsCountingDown;}sealed class ActionProfileTimerTrigger:ActionProfile<IMyTimerBlock>{public override IEnumerable<string>ActionNames=>
new[]{"Trigger"};public override string GroupName=>"Status";public override Action<IMyTimerBlock,IDictionary<string,string>
>OnActionCallback=>(block,args)=>block.Trigger();public override Func<IMyTimerBlock,IDictionary<string,string>,bool>
IsCompleteCallback=>(block,args)=>true;}sealed class ActionProfileWarheadArm:ActionProfile<IMyWarhead>{public override IEnumerable<string>
ActionNames=>new[]{"Arm"};public override string GroupName=>"Arm";public override Action<IMyWarhead,IDictionary<string,string>>
OnActionCallback=>(block,args)=>block.IsArmed=true;public override Func<IMyWarhead,IDictionary<string,string>,bool>IsCompleteCallback=>(
block,args)=>block.IsArmed;}sealed class ActionProfileWarheadDetonate:ActionProfile<IMyWarhead>{public override IEnumerable<
string>ActionNames=>new[]{"Detonate"};public override Action<IMyWarhead,IDictionary<string,string>>OnActionCallback=>(block,
args)=>block.Detonate();public override Func<IMyWarhead,IDictionary<string,string>,bool>IsCompleteCallback=>(block,args)=>
true;}sealed class ActionProfileWarheadDisarm:ActionProfile<IMyWarhead>{public override IEnumerable<string>ActionNames=>new[
]{"Disarm"};public override string GroupName=>"Arm";public override Action<IMyWarhead,IDictionary<string,string>>
OnActionCallback=>(block,args)=>block.IsArmed=true;public override Func<IMyWarhead,IDictionary<string,string>,bool>IsCompleteCallback=>(
block,args)=>!block.IsArmed;}sealed class ActionProfileWarheadStart:ActionProfile<IMyWarhead>{public override IEnumerable<
string>ActionNames=>new[]{"Start"};public override string GroupName=>"Status";public override Action<IMyWarhead,IDictionary<
string,string>>OnActionCallback=>(block,args)=>block.StartCountdown();public override Func<IMyWarhead,IDictionary<string,
string>,bool>IsCompleteCallback=>(block,args)=>block.Closed||!block.IsCountingDown;protected override string
GetCompletionDetails(IMyWarhead block,IDictionary<string,string>arguments)=>
$"Time: {TimeSpan.FromSeconds(block.DetonationTime).ToString(@"hh\:mm\:ss")}";}sealed class ActionProfileWarheadStop:ActionProfile<IMyWarhead>{public override IEnumerable<string>ActionNames=>new[]{
"Stop"};public override string GroupName=>"Status";public override Action<IMyWarhead,IDictionary<string,string>>
OnActionCallback=>(block,args)=>block.StopCountdown();public override Func<IMyWarhead,IDictionary<string,string>,bool>IsCompleteCallback
=>(block,args)=>!block.IsCountingDown;}interface IActionProfile{IEnumerable<string>ActionNames{get;}string GroupName{get;}
Action<IMyTerminalBlock,IDictionary<string,string>>OnActionCallback{get;}Func<IMyTerminalBlock,IDictionary<string,string>,bool
>IsCompleteCallback{get;}string GetCompletionDetails(IMyTerminalBlock block,IDictionary<string,string>arguments);bool
IsAssignableFrom(IMyTerminalBlock block);}static class Helper{public static Color ParseColor(string value){Color?color;if(string.
IsNullOrWhiteSpace(value)){throw new FormatException("Color argument must have some value.");}else if(value.StartsWith("#")){color=
ColorHelper.FromHtml(value.Trim());if(color==null){throw new FormatException($"Color argument is not a valid HTML value: '{value}'"
);}}else{var rgb=value.Trim().Split(new[]{';',','},StringSplitOptions.None);int a,r,g,b;if(rgb.Length==1){color=
ColorHelper.FromName(rgb[0]);if(color==null){throw new FormatException($"Color argument is not a valid color name: '{value}'");}}
else if(rgb.Length==3&&int.TryParse(rgb[0],out r)&&int.TryParse(rgb[1],out g)&&int.TryParse(rgb[2],out b)){color=ColorHelper
.FromRGB(r,g,b);}else if(rgb.Length==4&&int.TryParse(rgb[0],out a)&&int.TryParse(rgb[1],out r)&&int.TryParse(rgb[2],out g
)&&int.TryParse(rgb[3],out b)){color=ColorHelper.FromARGB(a,r,g,b);}else{throw new FormatException(
$"Color argument is not a valid RGB value: '{value}'.");}}return color.Value;}public static IDictionary<string,IEnumerable<IMyTerminalBlock>>CreateBlockDictionary(IEnumerable
<string>blockNames,IEnumerable<IMyTerminalBlock>blockList,IEnumerable<IMyBlockGroup>blockGroup){var result=new Dictionary
<string,IEnumerable<IMyTerminalBlock>>(StringComparer.OrdinalIgnoreCase);var blockDictionary=Enumerable.GroupBy(blockList
,x=>x.DisplayNameText,StringComparer.OrdinalIgnoreCase).ToDictionarySafe(x=>x.Key,x=>x.AsEnumerable(),StringComparer.
OrdinalIgnoreCase);IEnumerable<IMyTerminalBlock>blocks;foreach(var blockName in blockNames){if(!result.TryGetValue(blockName,out blocks))
{if(blockName.StartsWith("*")&&blockName.EndsWith("*")){var realName=blockName.Substring(1,blockName.Length-2);var groups
=blockGroup.Where(x=>x.Name.Equals(realName,StringComparison.OrdinalIgnoreCase));if(groups.Any()){blocks=groups.
SelectMany(group=>group.GetBlocks());}}if(blocks==null){blockDictionary.TryGetValue(blockName,out blocks);}if(blocks?.Any()==true)
{result.Add(blockName,blocks.ToArray());}else{throw new KeyNotFoundException($"No blocks found with name '{blockName}'.")
;}}}return result;}}class ConditionBlockInstruction{public string When{get;set;}public IEnumerable<string>Then{get;set;}}
class ConditionCommandInstruction:ICommandInstruction{public string CommandName{get;set;}public IList<
ConditionBlockInstruction>Body{get;set;}}public interface ICommandInstruction{string CommandName{get;set;}}public class Instruction{public string
BlockName{get;set;}public string ActionName{get;set;}public IDictionary<string,string>Arguments{get;set;}public bool IsValid{get;
set;}}public class InstructionBlock{public string Alias{get;set;}public IEnumerable<string>PreviousAlias{get;set;}public
IEnumerable<Instruction>Instructions{get;set;}}public class InstructionCommand:ICommandInstruction{public string CommandName{get;
set;}public IEnumerable<InstructionBlock>Body{get;set;}}public class InstructionParser{const string commandPattern=
@"\[(?<command>.*?)\](?:\r?\n)+(?<body>[\s\S]*?)(?=\r?\n\[(.*?)\]|$)";const string bodyPattern=
@"(?:when\s+(?:(?<when>[\w@,| ]*)\s+)(?:\/\/[ \w]+\s+)?)?run(?<run>\s?.*?)\s+as\s+(?<var>[\w@]+)(?:\r?|;)";const string ifPattern=@"(?:(?<clausule>if\s+|else\s?if\s+|else\s?)(?<condition>#\w+)?[\r\n]+(?<body>.*?)(?=[\r\n]+(?<close>if|else if|else|end)))"
;static readonly System.Text.RegularExpressions.Regex commandRegex=new System.Text.RegularExpressions.Regex(
commandPattern,System.Text.RegularExpressions.RegexOptions.Singleline);static readonly System.Text.RegularExpressions.Regex bodyRegex=
new System.Text.RegularExpressions.Regex(bodyPattern,System.Text.RegularExpressions.RegexOptions.Singleline);static
readonly System.Text.RegularExpressions.Regex ifRegex=new System.Text.RegularExpressions.Regex(ifPattern,System.Text.
RegularExpressions.RegexOptions.Singleline);public static IDictionary<string,ICommandInstruction>Parse(string text){var result=new
Dictionary<string,ICommandInstruction>(StringComparer.OrdinalIgnoreCase);var commandMatches=commandRegex.Matches(text);foreach(
System.Text.RegularExpressions.Match commandMatch in commandMatches){ICommandInstruction instructionCommand;instructionCommand
=CreateCommand(commandMatch,text);if(instructionCommand==null){instructionCommand=CreateConditionCommand(commandMatch);}
if(instructionCommand==null){throw new FormatException($"Unknown command body format.");}result.Add(instructionCommand.
CommandName,instructionCommand);}ValidateCommandDependences(result);return result;}private static InstructionCommand CreateCommand(
System.Text.RegularExpressions.Match commandMatch,string text){InstructionCommand result=null;var instructionBlocks=new
Dictionary<string,InstructionBlock>(StringComparer.OrdinalIgnoreCase);var commandName=commandMatch.Groups["command"].Value.Trim();
var bodyGroup=commandMatch.Groups["body"];var bodyContent=bodyGroup.Value.Trim();var bodyMatches=bodyRegex.Matches(
bodyContent);if(bodyMatches.OfType<System.Text.RegularExpressions.Match>().Any()){var previousIndex=-1;var lineCount=1;lineCount+=
GetLineCount(text,0,bodyGroup.Index);foreach(System.Text.RegularExpressions.Match bodyMatch in bodyMatches){int lineStart;try{
lineCount+=CheckSyntax(bodyContent,previousIndex+1,bodyMatch.Index);}catch(SyntaxException ex){throw new SyntaxException(ex.
OriginalMessage,lineCount+ex.Line,ex.Pos,ex.InnerException);}lineStart=lineCount;lineCount+=GetLineCount(bodyContent,bodyMatch.Index,
bodyMatch.Length);var whenGroup=bodyMatch.Groups["when"]?.Value;var runGroup=bodyMatch.Groups["run"];var run=runGroup.Value;var
aliasGroup=bodyMatch.Groups["var"];var alias=aliasGroup.Value.Trim();if(string.IsNullOrEmpty(alias)||!(alias=="none"||alias.
StartsWith("@"))){throw new SyntaxException("Invalid text after 'as' clausule.",lineStart+GetLineCount(bodyContent,bodyMatch.Index
,aliasGroup.Index-bodyMatch.Index));}else if(instructionBlocks.ContainsKey(alias)){throw new SyntaxException(
$"'{alias}' has been declared several times.",lineStart+GetLineCount(bodyContent,bodyMatch.Index,aliasGroup.Index-bodyMatch.Index));}else{var actions=run.Split(new[]
{'\n','\r'},StringSplitOptions.RemoveEmptyEntries).Select(line=>line.Trim()).Where(line=>!string.IsNullOrWhiteSpace(line)
&&!line.StartsWith("//")).Select((line,index)=>{Instruction instruction=null;var lineComments=line.Split(new[]{"//"},
StringSplitOptions.None);var lineWithoutComments=lineComments.First().Trim();if(lineWithoutComments.Contains("->")){instruction=
CreateInstruction(lineWithoutComments);}else{var lineItems=lineWithoutComments.Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);
var command=lineItems.FirstOrDefault();if(command==null){instruction=null;}else if(command.Equals("DELAY",StringComparison.
OrdinalIgnoreCase)){instruction=CreateInstructionDelay(lineItems);}if(instruction==null){instruction=new Instruction{BlockName=null,
ActionName=null,Arguments=null,IsValid=false};}}return new{Line=index+1,Instruction=instruction,};});var invalid=actions.Where(x=>
!x.Instruction.IsValid);if(invalid.Any()){throw new SyntaxException(
$"Invalid sentence in '{alias}' for lines: {string.Join(",",invalid.Select(x=>x.Line))}",lineStart+GetLineCount(bodyContent,bodyMatch.Index,runGroup.Index-bodyMatch.Index));}else{var previousActionsMatch=
whenGroup.Split(new[]{","},StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).GroupJoin(instructionBlocks.Keys,x=>x,y=>
$"@{y}",(x,y)=>new{Alias=x,Exists=y.Any(),IsValid=x.StartsWith("@")||x.Equals("none",StringComparison.OrdinalIgnoreCase)});if(
previousActionsMatch.Any(x=>!x.IsValid)){throw new FormatException(
$"Invalid syntax near to 'when' clausule in '{alias}' (line {lineStart}): "+$"{string.Join(",",previousActionsMatch.Where(x=>!x.IsValid).Select(x=>$"{x.Alias}"))} is not valid.\n"+
$"Maybe you forgot the '@'.");}else{if(alias.Equals("none",StringComparison.OrdinalIgnoreCase)){alias=$"Unnamed_{instructionBlocks.Count:00}";}
instructionBlocks.Add(alias,new InstructionBlock{Alias=alias,PreviousAlias=previousActionsMatch.Select(x=>x.Alias),Instructions=actions.
Select(x=>x.Instruction)});}}}previousIndex=bodyMatch.Index+bodyMatch.Length;}try{CheckSyntax(bodyContent,previousIndex,
bodyContent.Length);}catch(SyntaxException ex){throw new SyntaxException(ex.OriginalMessage,lineCount+ex.Line,ex.Pos,ex.
InnerException);}foreach(var item in instructionBlocks.Values){CheckDependences(instructionBlocks,item);}result=new InstructionCommand
{CommandName=commandName,Body=instructionBlocks.Values};}return result;}private static Instruction CreateInstruction(
string lineWithoutComments){var lineItems=lineWithoutComments.Split(new[]{"->"},StringSplitOptions.RemoveEmptyEntries);string
blockName=null,actionName=null;bool isValid;if(lineItems.Length>0){blockName=lineItems[0].Trim();}if(lineItems.Length>1){
actionName=lineItems[1].Trim();}isValid=lineItems.Length==2;var arguments=GetArguments(actionName);return new Instruction{
BlockName=blockName,ActionName=arguments[""].Trim(),Arguments=arguments.Where(x=>!string.IsNullOrEmpty(x.Key)).ToDictionarySafe(x
=>x.Key,x=>x.Value,StringComparer.OrdinalIgnoreCase),IsValid=isValid};}private static Instruction CreateInstructionDelay(
string[]lineItems){string timeString=null;if(lineItems.Length>=1){timeString=lineItems[1];}return new Instruction{BlockName=
null,ActionName="DELAY",Arguments=new Dictionary<string,string>(){{"TIME",timeString}},IsValid=true};}private static
ConditionCommandInstruction CreateConditionCommand(System.Text.RegularExpressions.Match commandMatch){ConditionCommandInstruction result=null;var
instructionBlocks=new List<ConditionBlockInstruction>();var commandName=commandMatch.Groups["command"].Value.Trim();var bodyGroup=
commandMatch.Groups["body"];var bodyContent=bodyGroup.Value.Trim();var bodyMatches=ifRegex.Matches(bodyContent);if(bodyMatches.
OfType<System.Text.RegularExpressions.Match>().Any()){bool ifCondition=false,elseCondition=false;foreach(System.Text.
RegularExpressions.Match match in bodyMatches){var clausule=match.Groups["clausule"].Value.Trim();var close=match.Groups["close"].Value.
Trim();var condition=match.Groups["condition"].Value?.Trim();var body=match.Groups["body"].Value.Trim();if(clausule.Equals(
"if",StringComparison.OrdinalIgnoreCase)){if(!ifCondition){ifCondition=true;}else{throw new Exception(
$"Syntact incorrect near '{clausule}'.");}}if(!ifCondition){throw new Exception($"Syntact incorrect near '{clausule}': 'if' condition not found.");}else if(
clausule.Equals("else",StringComparison.OrdinalIgnoreCase)){if(!elseCondition){if(close.Equals("end",StringComparison.
OrdinalIgnoreCase)){elseCondition=true;}else{throw new Exception($"Syntact incorrect near '{clausule}': 'end' clausule not found.");}}
else{throw new Exception($"Syntact incorrect near '{clausule}': there are already other 'else' clausule.");}}else if(!
condition.StartsWith("#")){throw new Exception(
$"'{condition}' is not a valid command name becaue it does not start with '#' character.");}else{condition=condition.Substring(1);}instructionBlocks.Add(new ConditionBlockInstruction{When=condition,Then=body.
Split(new[]{'\n','\r'},StringSplitOptions.RemoveEmptyEntries).Select(line=>line.Trim().Split(new[]{"//"},StringSplitOptions.
RemoveEmptyEntries).First().Trim()).Where(line=>{if(string.IsNullOrWhiteSpace(line)||line.StartsWith("//")){return false;}else if(line.
StartsWith("#")){return true;}else{throw new Exception(
$"'{condition}' is not a valid command name becaue it does not start with '#' character.");}}).Select(value=>value.Substring(1))});}result=new ConditionCommandInstruction{CommandName=commandName,Body=
instructionBlocks};}return result;}private static IDictionary<string,string>GetArguments(string value){var result=new Dictionary<string,
string>(StringComparer.OrdinalIgnoreCase);int i;char c;int stage=0;bool instring=false;var keyBuilder=new System.Text.
StringBuilder();var valueBuilder=new System.Text.StringBuilder();for(i=0;i<value.Length;i++){c=value[i];if(c=='/'&&!instring){result.
Add(keyBuilder.ToString(),valueBuilder.ToString());keyBuilder.Clear();valueBuilder.Clear();stage=1;}else if(c==':'&&!
instring){stage=2;}else if(c==' '&&!instring&&stage>0){stage=3;}else if(c=='\"'){instring=!instring;}else if(stage==1){
keyBuilder.Append(c);}else if(stage==2||stage==0){valueBuilder.Append(c);}}if(stage<3){if(instring){throw new FormatException(
"String not closed.");}else{result.Add(keyBuilder.ToString(),valueBuilder.ToString());}}return result;}private static int GetLineCount(
string content,int startIndex,int length){int i=0;char previous=char.MinValue;var substring=content.Substring(startIndex,
length);foreach(var chr in substring){if(chr=='\r'){i++;}else if(chr=='\n'&&previous!='\r'){i++;}previous=chr;}return i;}
private static int CheckSyntax(string content,int startIndex,int endIndex){var substring=content.Substring(startIndex,endIndex-
startIndex);var lines=substring.Split(new[]{"\r\n","\n","\r"},StringSplitOptions.None).Select(x=>x.Trim());var buffer=new System.
Text.StringBuilder();char previous=char.MinValue;bool validate=false;var i=0;foreach(var chr in substring){if(chr=='\r'){
validate=true;}else if(chr=='\n'){if(previous!='\r'){validate=true;}}else{buffer.Append(chr);}if(validate){var value=buffer.
ToString().Trim();if(string.IsNullOrEmpty(value)){}else if(value.StartsWith("//")){}else{throw new SyntaxException(
$"Syntaxt exception near '{value}'.",i);}buffer.Clear();i++;validate=false;}previous=chr;}return i;}private static void CheckDependences(IDictionary<string,
InstructionBlock>collection,InstructionBlock item,IEnumerable<string>path=null){string stringPath;if(path==null){path=new List<string>()
;}stringPath=string.Join("/",path.Append(item.Alias).Reverse());if(path.Contains(item.Alias,StringComparer.
OrdinalIgnoreCase)){throw new StackOverflowException(
$"'{item.Alias}' calls itself in the following path: '{stringPath}'. Please check 'when' clausules.");}else{var newpath=path.Append(item.Alias);foreach(var dependence in item.PreviousAlias){InstructionBlock previous;if(
collection.TryGetValue(dependence,out previous)){CheckDependences(collection,previous,newpath);}else{throw new
NullReferenceException($"Unknown dependence '{dependence}' found in 'when' clausule.");}}}}private static void ValidateCommandDependences(
IDictionary<string,ICommandInstruction>result){foreach(var instructionCommand in result.Values.OfType<ConditionCommandInstruction>(
)){foreach(var conditions in instructionCommand.Body){if(!string.IsNullOrEmpty(conditions.When)){ICommandInstruction
command;string whenName=conditions.When;if(whenName.Equals(instructionCommand.CommandName,StringComparison.OrdinalIgnoreCase)){
throw new ArgumentException($"A command cannot call itself ({whenName}).");}if(!result.TryGetValue(whenName,out command)){
throw new NullReferenceException($"Command with name '{whenName}' not found.");}else if(!(command is InstructionCommand)){
throw new ArgumentException($"Conditional commands not allowed '({whenName})' in conditional commands.");}}foreach(var then
in conditions.Then){ICommandInstruction command;string thenName=then;if(thenName.Equals(instructionCommand.CommandName,
StringComparison.OrdinalIgnoreCase)){throw new ArgumentException($"A command cannot call itself ({thenName}).");}else if(!result.
TryGetValue(thenName,out command)){throw new NullReferenceException($"Command with name '{thenName}' not found.");}else if(!(
command is InstructionCommand)){throw new ArgumentException(
$"Conditional commands not allowed '({thenName})' in conditional commands.");}}}}}}public sealed class SyntaxException:FormatException{public string OriginalMessage{get;set;}public int Line{get;}
public int Pos{get;}public SyntaxException(string message):this(message,-1,-1){}public SyntaxException(string message,int line
):this(message,line,-1){}public SyntaxException(string message,int line,int pos):this(message,line,pos,null){}public
SyntaxException(string message,int line,int pos,Exception innerException):base(GetMessage(message,line,pos),innerException){this.
OriginalMessage=message;this.Line=line;this.Pos=pos;}static string GetMessage(string message,int line,int pos){string result;if(line==-
1){result=message;}else{var builder=new System.Text.StringBuilder(message);builder.Append($" Line {line}");if(pos==-1){
builder.Append(".");}else{builder.Append($"; pos: {pos}.");}result=builder.ToString();}return result;}}interface ITaskAction{
string ActionKey{get;}bool IsCommandCondition{get;set;}DateTime?StartTime{get;set;}void Execute();bool Check(TaskStatusMode
mode,DateTime?momento=null,StringBuilder debug=null);}class Task{public string Alias{get;set;}public IEnumerable<Task>
PreviousTasks{get;set;}public IEnumerable<ITaskAction>Actions{get;set;}public bool IsRunning{get;set;}public bool IsDone{get;set;}}
class TaskAction:ITaskAction{public string ActionKey{get{return$"{this.Block.EntityId}\t{this.ActionProfile.GroupName}";}}
public IMyTerminalBlock Block{get;set;}public IActionProfile ActionProfile{get;set;}public IDictionary<string,string>Arguments
{get;set;}public DateTime?StartTime{get;set;}public bool IsCommandCondition{get;set;}public int Wait{get;set;}public void
Execute(){this.ActionProfile.OnActionCallback(this.Block,this.Arguments);}public bool Check(TaskStatusMode mode,DateTime?
momento=null,StringBuilder debug=null){var action=this;bool isCompleted=false;bool printDebug=false;string sufixDebug=null;
switch(mode){case TaskStatusMode.Condition:if(action.Wait>-1){sufixDebug=$"(wait:{action.Wait})";isCompleted=true;printDebug=
false;}break;case TaskStatusMode.Run:if(action.Wait==0){isCompleted=true;}else if(action.Wait>-1){isCompleted=(action.
StartTime!=null&&(((momento??DateTime.UtcNow)-action.StartTime.Value).TotalMilliseconds>=action.Wait));}sufixDebug=
$"(wait:{action.Wait})";printDebug=true;break;default:break;}isCompleted|=action.ActionProfile.IsCompleteCallback(action.Block,action.Arguments
);if(printDebug&&!isCompleted){var isCompletedText=(isCompleted?"Done":"Pending");var actionDebug=action.ActionProfile.
GetCompletionDetails(action.Block,action.Arguments);if(!string.IsNullOrWhiteSpace(actionDebug)){sufixDebug+=$" ({actionDebug})";}debug?.
AppendLine($"  - {action.Block.DisplayNameText}.{action.ActionProfile.ActionNames.First()} ({isCompletedText})");debug?.AppendLine
($"    {sufixDebug?.Trim()} {action.StartTime?.ToString("HH:mm:ss")}");}return isCompleted;}}class TaskActionDelay:
ITaskAction{string _name=>$"delay_{DateTime.Now.Ticks}";DateTime?_internalStartTime;public int Delay{get;set;}public string
ActionKey=>_name;public DateTime?StartTime{get;set;}public bool IsCommandCondition{get{return false;}set{throw new
NotImplementedException();}}public void Execute(){_internalStartTime=DateTime.UtcNow;}public bool Check(TaskStatusMode mode,DateTime?momento=
null,StringBuilder debug=null){bool isCompleted;string isCompletedText;if(_internalStartTime.HasValue){var elapsedTimeSpan=(
momento??DateTime.UtcNow)-_internalStartTime.Value;var remainingTimeSpan=TimeSpan.FromMilliseconds(this.Delay)-elapsedTimeSpan;
debug?.Append($"Delay: {remainingTimeSpan:hh\\:mm\\:ss}");isCompleted=(remainingTimeSpan<=TimeSpan.Zero);}else{debug?.Append(
$"Delay: not started");isCompleted=false;}isCompletedText=(isCompleted?"Done":"Pending");debug?.Append($" ({isCompletedText})");return
isCompleted;}}enum TaskStatus{Pending,Running,Completed}enum TaskStatusMode{Condition,Run}static class Tasks{public static
IEnumerable<Task>CreateTasks(IEnumerable<InstructionBlock>instructions,IDictionary<string,IEnumerable<IMyTerminalBlock>>
blockDictionary){var result=new List<Task>();foreach(var instructionBlock in instructions){var actions=new List<ITaskAction>();foreach(
var instruction in instructionBlock.Instructions){if(instruction.BlockName==null){switch(instruction.ActionName){case
"DELAY":actions.Add(CreateTaskDelay(instruction));break;default:throw new Exception($"Action name: '{instruction.ActionName}'."
);}}else{actions.AddRange(CreateTaskAction(instruction,blockDictionary));}}result.Add(new Task{Alias=instructionBlock.
Alias,Actions=actions,PreviousTasks=result.Join(instructionBlock.PreviousAlias,x=>x.Alias,y=>y,(x,y)=>x,StringComparer.
OrdinalIgnoreCase),});}foreach(var task in result){task.PreviousTasks=task.PreviousTasks.ToArray();}return result;}private static
IEnumerable<TaskAction>CreateTaskAction(Instruction instruction,IDictionary<string,IEnumerable<IMyTerminalBlock>>blockDictionary){
var list=new List<TaskAction>();IEnumerable<IMyTerminalBlock>blocks=null;if(blockDictionary.TryGetValue(instruction.
BlockName,out blocks)){string argumentValue;bool check;int wait;check=instruction.Arguments.TryGetValue("CHECK",out argumentValue
)&&(string.IsNullOrWhiteSpace(argumentValue)||argumentValue.Equals("true",StringComparison.OrdinalIgnoreCase));if(
instruction.Arguments.TryGetValue("MAXWAIT",out argumentValue)){if(string.IsNullOrWhiteSpace(argumentValue)||!int.TryParse(
argumentValue,out wait)){throw new FormatException($"'/MAXWAIT' must have a numeric value.");}}else if(instruction.Arguments.
TryGetValue("WAIT",out argumentValue)){if(string.IsNullOrWhiteSpace(argumentValue)||!int.TryParse(argumentValue,out wait)){throw
new FormatException($"'/WAIT' must have a numeric value.");}}else if(instruction.Arguments.TryGetValue("NOWAIT",out
argumentValue)&&(string.IsNullOrWhiteSpace(argumentValue)||argumentValue.Equals("true",StringComparison.OrdinalIgnoreCase))){wait=0;}
else{wait=-1;}foreach(var block in blocks){list.Add(new TaskAction{Block=block,ActionProfile=ActionProfiles.GetActionProfile
(block,instruction.ActionName),Arguments=instruction.Arguments,IsCommandCondition=check,Wait=wait});}}else{throw new
KeyNotFoundException($"No blocks found with name '{instruction.BlockName}'.");}return list;}private static TaskActionDelay CreateTaskDelay(
Instruction instruction){string argumentValue;int milliseconds;if(instruction.Arguments.TryGetValue("TIME",out argumentValue)){if(
string.IsNullOrWhiteSpace(argumentValue)||!int.TryParse(argumentValue,out milliseconds)){throw new FormatException(
$"Delay time must be numeric.");}}else{throw new FormatException($"No time defined for delay.");}return new TaskActionDelay{Delay=milliseconds};}
public static IEnumerable<Task>Run(this IEnumerable<Task>tasks,StringBuilder debug=null){var momento=DateTime.UtcNow;debug?.
AppendLine("Running:");foreach(var task in tasks){var status=GetStatus(task,TaskStatusMode.Run,momento,debug);if(status==
TaskStatus.Completed&&task.IsRunning){task.IsRunning=false;task.IsDone=true;}else if(status==TaskStatus.Pending){var
previousCompleted=task.PreviousTasks.All(previous=>previous.IsDone);debug?.AppendLine(
$"{task.Alias} previous completed: {previousCompleted} ({string.Join(", ",task.PreviousTasks.Select(x=>x.Alias))})");if(previousCompleted){foreach(var action in task.Actions){action.Execute();action.StartTime=momento;}task.IsRunning=
true;}}}return tasks.Where(task=>task.IsRunning==true);}public static void Cancel(this IEnumerable<Task>tasks){tasks.ForEach
(task=>{task.IsRunning=false;task.Actions.ForEach(action=>action.StartTime=null);});}public static bool IsCompleted(this
IEnumerable<Task>tasks,StringBuilder debug=null){bool result;var checkActions=tasks.SelectMany(x=>x.Actions).Where(x=>x.
IsCommandCondition);debug?.AppendLine("Checking:");if(!checkActions.Any()){checkActions=GetLastActions(tasks);}result=IsDone(checkActions,
TaskStatusMode.Condition,null,debug);debug?.AppendLine($"Result: {result}");return result;}private static TaskStatus GetStatus(Task
task,TaskStatusMode mode,DateTime?momento=null,StringBuilder debug=null){TaskStatus status;debug?.AppendLine(
$" -> {task.Alias} ({(task.IsRunning?"Running":task.IsDone?"Done":"Pending")})");if(task.IsDone){status=TaskStatus.Completed;}else if(task.IsRunning){if(IsDone(task,mode,momento,debug)){status=
TaskStatus.Completed;}else{status=TaskStatus.Running;}}else{status=TaskStatus.Pending;}return status;}private static bool IsDone(
Task task,TaskStatusMode mode,DateTime?momento=null,StringBuilder debug=null){return IsDone(task.Actions,mode,momento,debug)
;}private static bool IsDone(IEnumerable<ITaskAction>actions,TaskStatusMode mode,DateTime?momento=null,StringBuilder
debug=null){return actions.All(action=>action.Check(mode,momento,debug));}public static IEnumerable<ITaskAction>
GetLastActions(this IEnumerable<Task>tasks){var actionDictionary=new Dictionary<string,ITaskAction>(StringComparer.OrdinalIgnoreCase);
var validatedTasks=new List<Task>();IEnumerable<Task>validationTasks;var i=0;validationTasks=tasks.Where(x=>!x.
PreviousTasks.Any());while(validationTasks.Any()){foreach(var task in validationTasks){foreach(var action in task.Actions){
actionDictionary[action.ActionKey]=action;}validatedTasks.Add(task);}validationTasks=tasks.Except(validatedTasks).Where(t=>t.
PreviousTasks.All(x=>validatedTasks.Contains(x)));i++;}return actionDictionary.Values;}}public static class ColorHelper{static
IDictionary<string,Color?>Colors=new Dictionary<string,Color?>(StringComparer.OrdinalIgnoreCase){{"Transparent",Color.Transparent},
{"AliceBlue",Color.AliceBlue},{"AntiqueWhite",Color.AntiqueWhite},{"Aqua",Color.Aqua},{"Aquamarine",Color.Aquamarine},{
"Azure",Color.Azure},{"Beige",Color.Beige},{"Bisque",Color.Bisque},{"Black",Color.Black},{"BlanchedAlmond",Color.BlanchedAlmond
},{"Blue",Color.Blue},{"BlueViolet",Color.BlueViolet},{"Brown",Color.Brown},{"BurlyWood",Color.BurlyWood},{"CadetBlue",
Color.CadetBlue},{"Chartreuse",Color.Chartreuse},{"Chocolate",Color.Chocolate},{"Coral",Color.Coral},{"CornflowerBlue",Color.
CornflowerBlue},{"Cornsilk",Color.Cornsilk},{"Crimson",Color.Crimson},{"Cyan",Color.Cyan},{"DarkBlue",Color.DarkBlue},{"DarkCyan",
Color.DarkCyan},{"DarkGoldenrod",Color.DarkGoldenrod},{"DarkGray",Color.DarkGray},{"DarkGreen",Color.DarkGreen},{"DarkKhaki",
Color.DarkKhaki},{"DarkMagenta",Color.DarkMagenta},{"DarkOliveGreen",Color.DarkOliveGreen},{"DarkOrange",Color.DarkOrange},{
"DarkOrchid",Color.DarkOrchid},{"DarkRed",Color.DarkRed},{"DarkSalmon",Color.DarkSalmon},{"DarkSeaGreen",Color.DarkSeaGreen},{
"DarkSlateBlue",Color.DarkSlateBlue},{"DarkSlateGray",Color.DarkSlateGray},{"DarkTurquoise",Color.DarkTurquoise},{"DarkViolet",Color.
DarkViolet},{"DeepPink",Color.DeepPink},{"DeepSkyBlue",Color.DeepSkyBlue},{"DimGray",Color.DimGray},{"DodgerBlue",Color.DodgerBlue
},{"Firebrick",Color.Firebrick},{"FloralWhite",Color.FloralWhite},{"ForestGreen",Color.ForestGreen},{"Fuchsia",Color.
Fuchsia},{"Gainsboro",Color.Gainsboro},{"GhostWhite",Color.GhostWhite},{"Gold",Color.Gold},{"Goldenrod",Color.Goldenrod},{
"Gray",Color.Gray},{"Green",Color.Green},{"GreenYellow",Color.GreenYellow},{"Honeydew",Color.Honeydew},{"HotPink",Color.
HotPink},{"IndianRed",Color.IndianRed},{"Indigo",Color.Indigo},{"Ivory",Color.Ivory},{"Khaki",Color.Khaki},{"Lavender",Color.
Lavender},{"LavenderBlush",Color.LavenderBlush},{"LawnGreen",Color.LawnGreen},{"LemonChiffon",Color.LemonChiffon},{"LightBlue",
Color.LightBlue},{"LightCoral",Color.LightCoral},{"LightCyan",Color.LightCyan},{"LightGoldenrodYellow",Color.
LightGoldenrodYellow},{"LightGray",Color.LightGray},{"LightGreen",Color.LightGreen},{"LightPink",Color.LightPink},{"LightSalmon",Color.
LightSalmon},{"LightSeaGreen",Color.LightSeaGreen},{"LightSkyBlue",Color.LightSkyBlue},{"LightSlateGray",Color.LightSlateGray},{
"LightSteelBlue",Color.LightSteelBlue},{"LightYellow",Color.LightYellow},{"Lime",Color.Lime},{"LimeGreen",Color.LimeGreen},{"Linen",
Color.Linen},{"Magenta",Color.Magenta},{"Maroon",Color.Maroon},{"MediumAquamarine",Color.MediumAquamarine},{"MediumBlue",
Color.MediumBlue},{"MediumOrchid",Color.MediumOrchid},{"MediumPurple",Color.MediumPurple},{"MediumSeaGreen",Color.
MediumSeaGreen},{"MediumSlateBlue",Color.MediumSlateBlue},{"MediumSpringGreen",Color.MediumSpringGreen},{"MediumTurquoise",Color.
MediumTurquoise},{"MediumVioletRed",Color.MediumVioletRed},{"MidnightBlue",Color.MidnightBlue},{"MintCream",Color.MintCream},{
"MistyRose",Color.MistyRose},{"Moccasin",Color.Moccasin},{"NavajoWhite",Color.NavajoWhite},{"Navy",Color.Navy},{"OldLace",Color.
OldLace},{"Olive",Color.Olive},{"OliveDrab",Color.OliveDrab},{"Orange",Color.Orange},{"OrangeRed",Color.OrangeRed},{"Orchid",
Color.Orchid},{"PaleGoldenrod",Color.PaleGoldenrod},{"PaleGreen",Color.PaleGreen},{"PaleTurquoise",Color.PaleTurquoise},{
"PaleVioletRed",Color.PaleVioletRed},{"PapayaWhip",Color.PapayaWhip},{"PeachPuff",Color.PeachPuff},{"Peru",Color.Peru},{"Pink",Color.
Pink},{"Plum",Color.Plum},{"PowderBlue",Color.PowderBlue},{"Purple",Color.Purple},{"Red",Color.Red},{"RosyBrown",Color.
RosyBrown},{"RoyalBlue",Color.RoyalBlue},{"SaddleBrown",Color.SaddleBrown},{"Salmon",Color.Salmon},{"SandyBrown",Color.SandyBrown
},{"SeaGreen",Color.SeaGreen},{"SeaShell",Color.SeaShell},{"Sienna",Color.Sienna},{"Silver",Color.Silver},{"SkyBlue",
Color.SkyBlue},{"SlateBlue",Color.SlateBlue},{"SlateGray",Color.SlateGray},{"Snow",Color.Snow},{"SpringGreen",Color.
SpringGreen},{"SteelBlue",Color.SteelBlue},{"Tan",Color.Tan},{"Teal",Color.Teal},{"Thistle",Color.Thistle},{"Tomato",Color.Tomato},
{"Turquoise",Color.Turquoise},{"Violet",Color.Violet},{"Wheat",Color.Wheat},{"White",Color.White},{"WhiteSmoke",Color.
WhiteSmoke},{"Yellow",Color.Yellow},{"YellowGreen",Color.YellowGreen}};public static Color?FromName(string name){Color?color;
Colors.TryGetValue(name,out color);return color;}public static Color?FromHtml(string htmlColor){return ColorExtensions.
FromHtml(htmlColor);}public static Color?FromRGB(int r,int g,int b){return new Color(r,g,b);}public static Color?FromARGB(int a,
int r,int g,int b){return new Color(r,g,b,a);}}static class CommonExtensions{public static IList<IMyTerminalBlock>GetBlocks
(this IMyBlockGroup blockGroup,Func<IMyTerminalBlock,bool>collect=null){var blocks=new List<IMyTerminalBlock>();
blockGroup.GetBlocks(blocks,collect);return blocks;}public static IList<T>GetBlocksOfType<T>(this IMyBlockGroup blockGroup,Func<T,
bool>collect=null)where T:class{var blocks=new List<T>();blockGroup.GetBlocksOfType(blocks,collect);return blocks;}public
static IList<IMyTerminalBlock>GetBlocks(this IMyGridTerminalSystem gridTerminalSystem){var blocks=new List<IMyTerminalBlock>()
;gridTerminalSystem.GetBlocks(blocks);return blocks;}public static IList<T>GetBlocksOfType<T>(this IMyGridTerminalSystem
gridTerminalSystem,Func<T,bool>collect=null)where T:class{var blocks=new List<T>();gridTerminalSystem.GetBlocksOfType(blocks,collect);
return blocks;}public static IList<IMyBlockGroup>GetBlockGroups(this IMyGridTerminalSystem gridTerminalSystem,Func<
IMyBlockGroup,bool>collect=null){var blockGroups=new List<IMyBlockGroup>();gridTerminalSystem.GetBlockGroups(blockGroups,collect);
return blockGroups;}public static IList<IMyBlockGroup>GetBlockGroups(this IMyGridTerminalSystem gridTerminalSystem,
IMyTerminalBlock block,Func<IMyBlockGroup,bool>collect=null){Func<IMyBlockGroup,bool>basicPredicate=x=>x.GetBlocks().Any(y=>y==block);
Func<IMyBlockGroup,bool>predicate;if(collect==null){predicate=basicPredicate;}else{predicate=x=>basicPredicate(x)&&collect(x
);}return GetBlockGroups(gridTerminalSystem,predicate);}public static IList<ITerminalAction>GetActions(this
IMyTerminalBlock block,Func<ITerminalAction,bool>collect=null){var actions=new List<ITerminalAction>();block.GetActions(actions,collect)
;return actions;}public static void ForEach<T>(this IEnumerable<T>collections,Action<T>action)where T:class{foreach(var
item in collections){action(item);}}public static Dictionary<TKey,TElement>ToDictionarySafe<TSource,TKey,TElement>(this
IEnumerable<TSource>source,Func<TSource,TKey>keySelector,Func<TSource,TElement>elementSelector,IEqualityComparer<TKey>comparer){var
result=new Dictionary<TKey,TElement>(comparer);foreach(var item in source){result.Add(keySelector(item),elementSelector(item))
;}return result;}}public static class CommonHelper{public static bool IsCycle(UpdateType updateSource){return(
updateSource==UpdateType.Update1||updateSource==UpdateType.Update10||updateSource==UpdateType.Update100||updateSource==UpdateType.
Once);}public static bool ArrayEquals<TValue>(IEnumerable<TValue>value1,IEnumerable<TValue>value2){if(value1==null&&value2==
null){return true;}else if(value1==null||value2==null){return false;}else{var etor1=value1.GetEnumerator();var etor2=value2.
GetEnumerator();var pend1=true;var pend2=true;var equals=true;while(equals&&pend1&&pend2){pend1=etor1.MoveNext();pend2=etor2.MoveNext
();if(pend1&&pend2){equals=object.Equals(etor1.Current,etor2.Current);}}return!pend1&&!pend2&&equals;}}}public static
class DisplayHelper{public static IList<MyTextSurfaceInfo>GetTextSurfaces(IEnumerable<IMyTerminalBlock>blocks){var result=new
List<MyTextSurfaceInfo>();var panels=blocks.OfType<IMyTextPanel>();var surfaceProviders=blocks.OfType<IMyTextSurfaceProvider
>();foreach(var panel in panels){result.Add(new MyTextSurfaceInfo{Index=-1,Owner=panel,TextSurface=panel,CustomData=panel
.CustomData});}foreach(var provider in surfaceProviders){for(int i=0;i<provider.SurfaceCount;i++){var textSurface=
provider.GetSurface(i);var block=provider as IMyTerminalBlock;result.Add(new MyTextSurfaceInfo{Index=i,Owner=block,TextSurface=
textSurface,CustomData=GetTextSurfaceCustomData(block.CustomData,i)});}}return result;}public static IList<MyTextSurfaceInfo>
GetTextSurfaces(params IMyTerminalBlock[]blocks)=>GetTextSurfaces((IEnumerable<IMyTerminalBlock>)blocks);public static string
GetTextSurfaceCustomData(string customData,int index){var result=string.Empty;var regex=new System.Text.RegularExpressions.Regex(
"@((?<index>[0-9]+) (?<script>\\w+))(?<content>[^@]*)");var matches=regex.Matches(customData);var match=matches.OfType<System.Text.RegularExpressions.Match>().FirstOrDefault(
x=>x.Success&&x.Groups["index"].Value==index.ToString());if(match!=null){result=match.Groups["content"].Value.Trim();}
return result;}public static string FormatMinutes(float minutes){return FormatTime(TimeSpan.FromMinutes(minutes));}public
static string FormatTime(TimeSpan timeSpan){var text=new List<string>();if(timeSpan.Days>0){text.Add(
$"{timeSpan.Days} day{(timeSpan.Days>0?"s":"")}");}if(timeSpan.Hours>0){text.Add($"{timeSpan.Hours}h");}if(timeSpan.Minutes>0){if(timeSpan.Days>0||timeSpan.Hours>0||
timeSpan.Seconds>0){text.Add($"{timeSpan.Minutes}m");}else{text.Add($"{timeSpan.Minutes} minute{(timeSpan.Minutes>0?"s":"")}");}
}if(timeSpan.Seconds>0){if(timeSpan.Days>0||timeSpan.Hours>0||timeSpan.Minutes>0){text.Add($"{timeSpan.Seconds}s");}else{
text.Add($"{timeSpan.Seconds} second{(timeSpan.Seconds>0?"s":"")}");}}return string.Join(" ",text);}}public sealed class
NotImplementedException:Exception{}public sealed class StackOverflowException:Exception{public StackOverflowException(string message):base(
message){}}public class MyTextSurfaceInfo{public int Index{get;set;}public IMyTextSurface TextSurface{get;set;}public
IMyTerminalBlock Owner{get;set;}public string CustomData{get;set;}