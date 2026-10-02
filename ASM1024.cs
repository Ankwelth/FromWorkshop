/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */
const UpdateType CommandUpdate=UpdateType.Trigger|UpdateType.Terminal;MyCommandLine commandLine=new MyCommandLine();
IMyTextSurface drawingSurface;Instructions instructions;string version="0.1";Program(){Runtime.UpdateFrequency=UpdateFrequency.
Update10;drawingSurface=Me.GetSurface(0);drawingSurface.ContentType=ContentType.TEXT_AND_IMAGE;instructions=new Instructions(
this);Init();}void Init(){}void Save(){}void Main(string argument,UpdateType updateType){if((updateType&CommandUpdate)!=0){
RunCommand(argument);}if((updateType&UpdateType.Update10)!=0){RunContinuousLogic();}}void RunCommand(string argument){if(argument
!=null){commandLine.TryParse(argument);var command=commandLine.Argument(0);switch(command){case"infos":GetInformation(
commandLine.Argument(1));break;case"execute":instructions.Init();instructions.ExecuteLabel(commandLine.Argument(1));break;case
"reset":Init();break;case"prefix":RenamePrefix(commandLine.Argument(1));break;case"unprefix":UnRenamePrefix(commandLine.
Argument(1));break;default:drawingSurface.WriteText("Program started",false);instructions.Init();instructions.Start();break;}}}
void RenamePrefix(string tag){BlockSystem<IMyTerminalBlock>blocks=BlockSystem<IMyTerminalBlock>.SearchBlocks(this);blocks.
ForEach(delegate(IMyTerminalBlock block){if(!block.CustomName.StartsWith(tag)){block.CustomName=tag+" "+block.CustomName;}});}
void UnRenamePrefix(string tag){BlockSystem<IMyTerminalBlock>blocks=BlockSystem<IMyTerminalBlock>.SearchBlocks(this);blocks.
ForEach(delegate(IMyTerminalBlock block){if(block.CustomName.StartsWith(tag)){block.CustomName=block.CustomName.Replace(tag+" "
,"");}});}void GetInformation(string filter){BlockFilter<IMyTerminalBlock>block_filter=BlockFilter<IMyTerminalBlock>.
Create(Me,filter);var items=BlockSystem<IMyTerminalBlock>.SearchByFilter(this,block_filter);if(items.IsEmpty==false){foreach(
var item in items.List){var infos=new StringBuilder();var type=item.GetType();infos.AppendLine($"# {type.Name}");List<
ITerminalProperty>properties=new List<ITerminalProperty>();item.GetProperties(properties);properties.Sort(new TerminalPropertyComparer())
;infos.AppendLine();infos.AppendLine("## Properties:");foreach(var property in properties){infos.AppendLine(
$"* [RW] {property.Id}: {property.TypeName}");}List<IReflectionProperty>reflectionProperties=new List<IReflectionProperty>();item.GetReflectionProperties(
reflectionProperties);if(reflectionProperties.Count>0){properties.Sort(new TerminalPropertyComparer());infos.AppendLine();infos.AppendLine(
"## Reflection Properties:");foreach(var property in reflectionProperties){var bindingFlags="[R ]";if(property.BindingFlags==ReflectionBindingFlags
.ReadWrite){bindingFlags="[RW]";}infos.AppendLine(
$"* {bindingFlags} {property.Id}: {property.TypeName} {property.Description}");}}List<ITerminalAction>actions=new List<ITerminalAction>();item.GetActions(actions);actions.Sort(new
TerminalActionComparer());infos.AppendLine();infos.AppendLine("## Actions:");foreach(var action in actions){infos.AppendLine(
$"* [W] {action.Id}: {action.Name}");}item.CustomData=infos.ToString();}}}void RunContinuousLogic(){if(instructions.FirstStarted==false){Echo(
$"Version {version}");instructions.Init();instructions.Start();}instructions.Execute();}public enum StateMachine{Stopped,Traking,Running,
Waitting}class BlockDevice{private Instruction instruction;public BlockDevice(Instruction instruction){this.instruction=
instruction;}public void SetColor(BlockSystem<IMyTerminalBlock>devices,string name,Color color){if(devices.IsEmpty)return;foreach(
IMyTerminalBlock block in devices.List){block.SetValueColor(name,color);}}public void ApplyAction(BlockSystem<IMyTerminalBlock>devices,
string name){if(devices.IsEmpty)return;foreach(IMyTerminalBlock block in devices.List){block.ApplyAction(name);}}public void
SetProperty(BlockSystem<IMyTerminalBlock>devices,string name,double value){if(devices.IsEmpty)return;foreach(IMyTerminalBlock block
in devices.List){var property=block.GetProperty(name);if(property!=null){switch(property.TypeName){case"Boolean":block.
SetValueBool(property.Id,value>=1);break;case"Single":block.SetValueFloat(property.Id,(float)value);break;case"Color":throw new
Exception("Use Color instruction");}}else{throw new Exception("Wrong property name");}}}public double GetProperty(BlockSystem<
IMyTerminalBlock>devices,string name,AggregationType aggregation){if(devices.IsEmpty)return 0;var values=new List<double>();foreach(
IMyTerminalBlock block in devices.List){var property=block.GetProperty(name);if(property!=null){switch(property.TypeName){case"Boolean":
{var valueBool=block.GetValueBool(property.Id);var value=valueBool?1d:0d;values.Add(value);}break;case"Single":{var
valueFloat=block.GetValueFloat(property.Id);var value=(double)valueFloat;values.Add(value);}break;case"Color":{var valueColor=
block.GetValueColor(property.Id);throw new Exception("Color Not Implemented");}break;}}else{var reflectionProperty=block.
GetReflectionProperty(name);if(reflectionProperty!=null){switch(reflectionProperty.TypeName){case"Boolean":{var valueBool=block.
GetReflectionValue<bool>(reflectionProperty.Id);var value=valueBool?1d:0d;values.Add(value);}break;case"Single":{var valueFloat=block.
GetReflectionValue<double>(reflectionProperty.Id);var value=(double)valueFloat;values.Add(value);}break;case"Color":{var valueColor=block.
GetReflectionValue<Color>(reflectionProperty.Id);throw new Exception("Color Not Implemented");}}}else{throw new Exception(
"Wrong property name");}}}switch(aggregation){case AggregationType.Average:return values.Average();case AggregationType.Sum:return values.Sum
();case AggregationType.Maximum:return values.Max();case AggregationType.Minimum:return values.Min();}return 0d;}public
double InventoryDevice(BlockSystem<IMyTerminalBlock>devices,int index,string property,AggregationType aggregation){if(devices.
IsEmpty)return 0;var values=new List<double>();foreach(IMyTerminalBlock block in devices.List){double count=0;IMyInventory
inventory=block.GetInventory(index);switch(property){case"Amount":List<MyInventoryItem>items=new List<MyInventoryItem>();
inventory.GetItems(items);foreach(MyInventoryItem item in items){switch(property){case"Amount":double amount=0;Double.TryParse(
item.Amount.ToString(),out amount);count+=amount;break;}}values.Add(count);break;default:var reflectionProperty=inventory.
GetReflectionProperty(property);if(reflectionProperty!=null){switch(reflectionProperty.TypeName){case"Boolean":{var valueBool=inventory.
GetReflectionValue<bool>(reflectionProperty.Id);var value=valueBool?1d:0d;values.Add(value);}break;case"Single":{var valueFloat=inventory.
GetReflectionValue<double>(reflectionProperty.Id);var value=(double)valueFloat;values.Add(value);}break;case"Color":{var valueColor=
inventory.GetReflectionValue<Color>(reflectionProperty.Id);throw new Exception("Color Not Implemented");}}}else{throw new
Exception("Wrong property name");}break;}}switch(aggregation){case AggregationType.Average:return values.Average();case
AggregationType.Sum:return values.Sum();case AggregationType.Maximum:return values.Max();case AggregationType.Minimum:return values.Min
();}return 0d;}public double LoadDevice(BlockSystem<IMyTerminalBlock>devices,string property,AggregationType aggregation)
{switch(property){default:return GetProperty(devices,property,aggregation);}}public void StoreDevice(BlockSystem<
IMyTerminalBlock>devices,string property,double value){switch(property){default:SetProperty(devices,property,value);break;}}public void
ActionDevice(BlockSystem<IMyTerminalBlock>devices,string property){switch(property){default:{var isAction=devices.First.HasAction(
property);if(isAction){ApplyAction(devices,property);}else{throw new Exception("Wrong action name");}}break;}}}public enum
AggregationType{Average,Sum,Minimum,Maximum}class BlockSystem<T>where T:class{protected Program program;public List<T>List=new List<T>(
);public BlockSystem(){List=new List<T>();}public BlockSystem(Program program){this.program=program;this.List=new List<T>
();}public static BlockSystem<T>SearchBlocks(Program program,Func<T,bool>collect=null,string info=null){List<T>list=new
List<T>();try{program.GridTerminalSystem.GetBlocksOfType<T>(list,collect);}catch{}if(info==null)program.Echo(String.Format(
"List <{0}> count: {1}",typeof(T).Name,list.Count));else program.Echo(String.Format("List <{0}> count: {1}",info,list.Count));return new
BlockSystem<T>(){program=program,List=list};}public static BlockSystem<T>SearchByTag(Program program,string tag){return BlockSystem
<T>.SearchBlocks(program,block=>((IMyTerminalBlock)block).CustomName.Contains(tag),tag);}public static BlockSystem<T>
SearchByName(Program program,string name){return BlockSystem<T>.SearchBlocks(program,block=>((IMyTerminalBlock)block).CustomName.
Equals(name),name);}public static List<IMyBlockGroup>SearchGroups(Program program,Func<IMyBlockGroup,bool>collect=null){List<
IMyBlockGroup>list=new List<IMyBlockGroup>();try{program.GridTerminalSystem.GetBlockGroups(list,collect);}catch{}program.Echo(String.
Format("List <IMyBlockGroup> count: {0}",list.Count));return list;}public static BlockSystem<T>SearchByGroup(Program program,
string name){List<T>list=new List<T>();IMyBlockGroup group=null;try{group=program.GridTerminalSystem.GetBlockGroupWithName(
name);}catch{}if(group!=null)group.GetBlocksOfType<T>(list);program.Echo(String.Format("List <{0}> count: {1}",name,list.
Count));return new BlockSystem<T>(){program=program,List=list};}public static BlockSystem<T>SearchByGrid(Program program,
IMyCubeGrid cubeGrid){return BlockSystem<T>.SearchBlocks(program,block=>((IMyCubeBlock)block).CubeGrid==cubeGrid);}public static
BlockSystem<T>SearchByFilter(Program program,BlockFilter<T>filter){List<T>list=new List<T>();try{if(filter.ByGroup){List<
IMyBlockGroup>groups=new List<IMyBlockGroup>();program.GridTerminalSystem.GetBlockGroups(groups,filter.GroupVisitor());List<T>
group_list=new List<T>();groups.ForEach(delegate(IMyBlockGroup group){group_list.Clear();group.GetBlocksOfType<T>(list,filter.
BlockVisitor());list.AddList(group_list);});}else{program.GridTerminalSystem.GetBlocksOfType<T>(list,filter.BlockVisitor());}}catch{
}program.Echo(String.Format("List<{0}>({1}):{2}",typeof(T).Name,filter.Value,list.Count));return new BlockSystem<T>(){
program=program,List=list};}public static List<IMyBlockGroup>SearchGroupFilter(Program program,BlockFilter<T>filter){List<
IMyBlockGroup>groups=new List<IMyBlockGroup>();try{if(filter.ByGroup){program.GridTerminalSystem.GetBlockGroups(groups,filter.
GroupVisitor());}}catch{}program.Echo(String.Format("List <{0}> count: {1}",filter.Value,groups.Count));return groups;}public void
ForEach(Action<T>action){if(!IsEmpty){List.ForEach(action);}}public bool IsPosition(float position,float epsilon=0.1f){bool
isState=true;if(!IsEmpty){if(List is List<IMyPistonBase>){foreach(IMyPistonBase block in List){float value=block.
CurrentPosition-position;if(Math.Abs(value)>epsilon)isState=false;}}if(List is List<IMyMotorStator>){foreach(IMyMotorStator block in
List){float value=block.Angle-float.Parse(Util.DegToRad(position).ToString());if(Math.Abs(value)>epsilon)isState=false;}}}
return isState;}public bool IsMorePosition(float position){bool isState=true;if(!IsEmpty){if(List is List<IMyPistonBase>){
foreach(IMyPistonBase block in List){if(block.CurrentPosition<position)isState=false;}}if(List is List<IMyMotorStator>){foreach
(IMyMotorStator block in List){if(block.Angle<float.Parse(Util.DegToRad(position).ToString()))isState=false;}}}return
isState;}public bool IsLessPosition(float position){bool isState=true;if(!IsEmpty){if(List is List<IMyPistonBase>){foreach(
IMyPistonBase block in List){if(block.CurrentPosition>position)isState=false;}}if(List is List<IMyMotorStator>){foreach(
IMyMotorStator block in List){if(block.Angle>float.Parse(Util.DegToRad(position).ToString()))isState=false;}}}return isState;}public
bool IsPositionMax(float epsilon=0.1f){bool isState=true;if(!IsEmpty){if(List is List<IMyPistonBase>){foreach(IMyPistonBase
block in List){float value=block.CurrentPosition-block.MaxLimit;if(Math.Abs(value)>epsilon)isState=false;}}if(List is List<
IMyMotorStator>){foreach(IMyMotorStator block in List){float value=block.Angle-block.UpperLimitRad;if(Math.Abs(value)>epsilon/100)
isState=false;}}}return isState;}public bool IsPositionMin(float epsilon=0.1f){bool isState=true;if(!IsEmpty){if(List is List<
IMyPistonBase>){foreach(IMyPistonBase block in List){float value=block.CurrentPosition-block.MinLimit;if(Math.Abs(value)>epsilon)
isState=false;}}if(List is List<IMyMotorStator>){foreach(IMyMotorStator block in List){float value=block.Angle-block.
LowerLimitRad;if(Math.Abs(value)>epsilon/100)isState=false;}}}return isState;}public void Velocity(float velocity){if(!IsEmpty){if(
List is List<IMyPistonBase>){foreach(IMyPistonBase block in List){block.Velocity=velocity;}}if(List is List<IMyMotorStator>)
{foreach(IMyMotorStator block in List){block.TargetVelocityRPM=velocity;}}}}public void ApplyAction(string action){if(!
IsEmpty){foreach(IMyTerminalBlock block in List){block.ApplyAction(action);}}}public void On(){ApplyAction("OnOff_On");}public
bool IsOn(){bool isState=true;if(!IsEmpty){foreach(IMyTerminalBlock block in List){if(!block.GetValueBool("OnOff"))isState=
false;}}return isState;}public void Off(){ApplyAction("OnOff_Off");}public bool IsOff(){return!IsOn();}public void Lock(){if(
!IsEmpty){if(List is List<IMyMotorStator>){foreach(IMyMotorStator block in List){block.RotorLock=true;}}else{ApplyAction(
"Lock");}}}public void Unlock(){if(!IsEmpty){if(List is List<IMyMotorStator>){foreach(IMyMotorStator block in List){block.
RotorLock=false;}}else{ApplyAction("Unlock");}}}public void Merge(BlockSystem<T>blockSystem){List.AddList(blockSystem.List);}
public bool IsEmpty{get{if(List!=null&&List.Count>0){return false;}return true;}}public T First{get{if(!IsEmpty){return List.
First();}return null;}}}class BlockFilter<T>where T:class{public string Value;public string Filter;public IMyCubeGrid
CubeGrid;public bool ByContains=false;public bool ByGroup=false;public bool MultiGrid=false;public bool HasInventory=false;
public static BlockFilter<T>Create(IMyTerminalBlock parent,string filter){BlockFilter<T>blockFilter=new BlockFilter<T>{Value=
filter,CubeGrid=parent.CubeGrid};if(filter.Contains(":")){string[]values=filter.Split(':');if(values[0].Contains("C"))
blockFilter.ByContains=true;if(values[0].Contains("G"))blockFilter.ByGroup=true;if(values[0].Contains("M"))blockFilter.MultiGrid=
true;if(!values[1].Equals("*"))blockFilter.Filter=values[1];}else{if(!filter.Equals("*"))blockFilter.Filter=filter;}return
blockFilter;}public Func<T,bool>BlockVisitor(){return delegate(T block){IMyTerminalBlock tBlock=(IMyTerminalBlock)block;bool state=
true;if(Filter!=null&&!ByGroup){if(ByContains){if(!tBlock.CustomName.Contains(Filter))state=false;}else{if(!tBlock.
CustomName.Equals(Filter))state=false;}}if(!MultiGrid){if(tBlock.CubeGrid!=CubeGrid)state=false;}if(HasInventory){if(!tBlock.
HasInventory)state=false;}return state;};}public Func<IMyBlockGroup,bool>GroupVisitor(){return delegate(IMyBlockGroup group){bool
state=true;if(Filter!=null&&ByGroup){if(ByContains){if(!group.Name.Contains(Filter))state=false;}else{if(!group.Name.Equals(
Filter))state=false;}}return state;};}}public enum SpriteForm{SquareSimple,SquareHollow,Circle,Triangle}public enum
SpriteOrientation{Horizontal,Vertical}class Instruction{public Instructions Parent{get;set;}public string Command{get;set;}public string
Name{get;set;}public InstructionType Type{get;set;}public int Index{get;set;}public int NextIndex{get;set;}public string[]
Args{get;set;}public float GetArgumentFloat(int index){if(index>0&&index<Args.Length){var arg=Args[index];if(Parent.Vars.
ContainsKey(arg)){return(float)Parent.Vars[arg];}else{return float.Parse(arg);}}return 0f;}public double GetArgumentDouble(int
index){if(index>0&&index<Args.Length){var arg=Args[index];if(Parent.Vars.ContainsKey(arg)){return(double)Parent.Vars[arg];}
else{return double.Parse(arg);}}return 0;}public int GetArgumentInt(int index){if(index>0&&index<Args.Length){var arg=Args[
index];if(Parent.Vars.ContainsKey(arg)){return(int)Parent.Vars[arg];}else{return int.Parse(arg);}}return 0;}public string
GetArgumentString(int index){if(index>0&&index<Args.Length){var arg=Args[index];return arg;}return null;}public int GetArgumentAdress(int
index){if(index>0&&index<Args.Length){var arg=Args[index];if(Parent.Labels.ContainsKey(arg)){return Parent.Labels[arg];}else
if(Parent.Vars.ContainsKey(arg)){return(int)Parent.Vars[arg];}else{return int.Parse(arg);}}return-1;}public BlockSystem<T>
GetArgumentDevice<T>(int index)where T:class{if(index>0&&index<Args.Length){var arg=Args[index];if(Parent.Devices.ContainsKey(arg)){
return(BlockSystem<T>)Parent.Devices[arg];}}return null;}public void SetVar(string name,object value){Parent.SetVar(name,value
);}public void SetDevice(string name,object value){Parent.SetDevice(name,value);}public void ParseFieldString(string
field,out string value){object var;if(Parent.Vars.TryGetValue(field,out var)){value=var.ToString();}else{value=field;}}public
void ParseFieldFloat(string field,out float value){if(!float.TryParse(field,out value)){object var;if(Parent.Vars.
TryGetValue(field,out var)){value=(float)var;}}}public void ParseFieldInt(string field,out int value){if(!int.TryParse(field,out
value)){object var;if(Parent.Vars.TryGetValue(field,out var)){value=(int)var;}}}public void ParseFieldBool(string field,out
bool value){if(!bool.TryParse(field,out value)){object var;if(Parent.Vars.TryGetValue(field,out var)){value=(bool)var;}}}
public override string ToString(){return$"{Name}:{Index}";}}class Instructions{public Program myProgram;private List<string>
logger=new List<string>();private MyCommandLine commandLine=new MyCommandLine();private char separator=' ';public List<
Instruction>Items=new List<Instruction>();public Dictionary<string,object>Devices=new Dictionary<string,object>();public Dictionary
<string,object>Vars=new Dictionary<string,object>();public Dictionary<string,int>Labels=new Dictionary<string,int>();
public Dictionary<string,InstructionType>Words=new Dictionary<string,InstructionType>();public StateBasic State=StateBasic.
None;public int Index=0;public bool FirstStarted=false;public Instructions(Program program){myProgram=program;}public void
Init(){logger.Clear();Index=0;Vars.Clear();Items.Clear();Words.Clear();Labels.Clear();SetVar("PY",Math.PI);InterpretBranch.
AppendWords(Words);InterpretDevice.AppendWords(Words);InterpretJump.AppendWords(Words);InterpretLogic.AppendWords(Words);
InterpretMath.AppendWords(Words);InterpretMisk.AppendWords(Words);InterpretSelection.AppendWords(Words);Log($"Words {Words.Count}");
Parse(myProgram.Me.CustomData);}public void Execute(){switch(State){case StateBasic.Completing:State=StateBasic.Completed;Log
("Program completed");break;case StateBasic.Running:ExecuteProgram();break;}myProgram.drawingSurface.WriteText(String.
Join("\n",logger));}public void ExecuteLabel(string name){try{if(Labels.ContainsKey(name)){if(Items.Count==0||Index>=Items.
Count){State=StateBasic.Completing;}else{ExecuteInstructions(true);}var adress=Labels[name];Index=adress;State=StateBasic.
Running;}else{Log($"Execution error: The label does not exist");}}catch(Exception ex){Log(
$"Execution Label error {Index+1}: {ex.Message}");Log(ex.StackTrace);State=StateBasic.Completing;}}public void Start(){State=StateBasic.Running;FirstStarted=true;}
public void ExecuteProgram(){try{if(Items.Count==0||Index>=Items.Count){State=StateBasic.Completing;}else{ExecuteInstructions(
);}}catch(Exception ex){Log($"Program error {Index+1}: {ex.Message}");Log(ex.StackTrace);State=StateBasic.Completing;}}
public void ExecuteInstructions(bool isSettings=false){try{var loop=0;var wait=true;while(wait){var instruction=Items[Index];
if(instruction==null){Index++;if(Index>=Items.Count){wait=false;}}else{if(instruction.Type==InstructionType.Label){Log(
$"== {instruction.Command} at {Index+1}");}else{Log($"-> {instruction.Command} at {Index+1}");}switch(instruction.Type){case InstructionType.Branch:
InterpretBranch.Interpret(instruction);break;case InstructionType.Device:InterpretDevice.Interpret(instruction);break;case
InstructionType.Jump:InterpretJump.Interpret(instruction);break;case InstructionType.Logic:InterpretLogic.Interpret(instruction);break;
case InstructionType.Math:InterpretMath.Interpret(instruction);break;case InstructionType.Misc:InterpretMisk.Interpret(
instruction);break;case InstructionType.Selection:InterpretSelection.Interpret(instruction);break;}Index=instruction.NextIndex;if(
instruction.Name==MiskWords.yield.ToString()||Index>=Items.Count){wait=false;}else{loop++;wait=loop<1024;}if(isSettings){if(
instruction.Type==InstructionType.Label){wait=false;}}}}}catch(InstructionException ex){Log(
$"Instruction error {Index+1}: {ex.Message}");State=StateBasic.Completing;}catch(Exception ex){Log($"Instruction error {Index+1}: {ex.Message}");Log(ex.StackTrace);
State=StateBasic.Completing;}}public void Log(string message){if(logger.Count>30)logger.RemoveAt(0);logger.Add(message);}
public void SetVar(string name,object value){if(Vars.ContainsKey(name)){Vars[name]=value;}else{Vars.Add(name,value);}}public
void SetDevice(string name,object value){if(Devices.ContainsKey(name)){Devices[name]=value;}else{Devices.Add(name,value);}}
private void Parse(string script){if(script==null)return;string[]lines=script.Split('\n');if(lines.Length>0){for(int i=0;i<
lines.Length;i++){string aline=lines[i];var instruction=ParseLine(aline,i);Items.Add(instruction);}}Log(
$"Parsed lines {Items.Count}");}private Instruction ParseLine(string line,int index){if(line.Trim()!=null&&!line.Trim().Equals("")){if(line.
StartsWith("#")){Instruction instanciated=new Instruction();instanciated.Command=line;instanciated.Index=index;instanciated.
NextIndex=index+1;instanciated.Parent=this;instanciated.Type=InstructionType.None;}else{if(line.Contains("#")){var splited=line.
Split('#');line=splited[0];}commandLine.TryParse(line);string[]values=commandLine.Items.ToArray();string name=values[0];if(
string.IsNullOrEmpty(name)==false){name=name.ToLower();}Log($"try instanciate {line} at {index+1}");var instanciated=
InstanciateInstruction(line,name,index,values);return instanciated;}}return null;}private Instruction InstanciateInstruction(string command,
string name,int index,string[]Args){Instruction instanciated=new Instruction();instanciated.Command=command;instanciated.Name=
name;instanciated.Index=index;instanciated.NextIndex=index+1;instanciated.Parent=this;instanciated.Args=Args;if(name.
LastIndexOf(':')!=-1){instanciated.Type=InstructionType.Label;var label=name.Substring(0,name.LastIndexOf(":"));Labels.Add(label,
index);}else{if(Words.ContainsKey(name)){instanciated.Type=Words[name];}else{throw new InstructionException(
$"Instruction '{name}' not found at {index}");}}return instanciated;}}public enum StateBasic{None,Stopped,Completing,Completed,Running,Waitting,Sleeped}public enum
InstructionType{None,Label,Device,Branch,Jump,Selection,Math,Logic,Stack,Misc,Import}class InterpretBranch{public static void
AppendWords(Dictionary<string,InstructionType>words){foreach(BranchWords item in Enum.GetValues(typeof(BranchWords))){words.Add(
item.ToString(),InstructionType.Branch);}}public static void Interpret(Instruction instruction){BranchWords word;Enum.
TryParse(instruction.Name,out word);var a=instruction.GetArgumentDouble(1);var b=0d;var c=0d;int address;if(instruction.Args.
Length==5){b=instruction.GetArgumentDouble(2);c=instruction.GetArgumentDouble(3);address=instruction.GetArgumentAdress(4);}
else if(instruction.Args.Length==4){b=instruction.GetArgumentDouble(2);address=instruction.GetArgumentAdress(3);}else{
address=instruction.GetArgumentAdress(3);}var isRedirect=false;var isRelative=false;var valid=false;switch(word){case
BranchWords.bap:valid=Math.Abs(a-b)<=Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon*8);break;case BranchWords.bapal:
isRedirect=true;valid=Math.Abs(a-b)<=Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon*8);break;case BranchWords.bapz:
valid=Math.Abs(a-b)<=Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon*8);break;case BranchWords.bapzal:isRedirect=
true;valid=Math.Abs(a-b)<=Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon*8);break;case BranchWords.beq:valid=a
==b;break;case BranchWords.beqal:isRedirect=true;valid=a==b;break;case BranchWords.beqz:valid=a==b;break;case BranchWords.
beqzal:isRedirect=true;valid=a==b;break;case BranchWords.bge:valid=a>=b;break;case BranchWords.bgeal:isRedirect=true;valid=a>=
b;break;case BranchWords.bgez:valid=a>=b;break;case BranchWords.bgezal:isRedirect=true;valid=a>=b;break;case BranchWords.
bgt:valid=a>b;break;case BranchWords.bgtal:isRedirect=true;valid=a>b;break;case BranchWords.bgtz:valid=a>b;break;case
BranchWords.bgtzal:isRedirect=true;valid=a>b;break;case BranchWords.ble:valid=a<=b;break;case BranchWords.bleal:isRedirect=true;
valid=a<=b;break;case BranchWords.blez:valid=a<=b;break;case BranchWords.blezal:isRedirect=true;valid=a<=b;break;case
BranchWords.blt:valid=a<b;break;case BranchWords.bltal:isRedirect=true;valid=a<b;break;case BranchWords.bltz:valid=a<b;break;case
BranchWords.bltzal:isRedirect=true;valid=a<b;break;case BranchWords.bna:valid=Math.Abs(a-b)>Math.Max(c*Math.Max(Math.Abs(a),Math.
Abs(b)),double.Epsilon*8);break;case BranchWords.bnaal:isRedirect=true;valid=Math.Abs(a-b)>Math.Max(c*Math.Max(Math.Abs(a),
Math.Abs(b)),double.Epsilon*8);break;case BranchWords.bnaz:valid=Math.Abs(a-b)>Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b)),
double.Epsilon*8);break;case BranchWords.bnazal:isRedirect=true;valid=Math.Abs(a-b)>Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b
)),double.Epsilon*8);break;case BranchWords.bne:valid=a!=b;break;case BranchWords.bneal:isRedirect=true;valid=a!=b;break;
case BranchWords.bnez:valid=a!=b;break;case BranchWords.bnezal:isRedirect=true;valid=a!=b;break;case BranchWords.brap:
isRelative=true;valid=Math.Abs(a-b)<=Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon*8);break;case BranchWords.brapz:
isRelative=true;valid=Math.Abs(a-b)<=Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon*8);break;case BranchWords.breq:
isRelative=true;valid=a==b;break;case BranchWords.breqz:isRelative=true;valid=a==b;break;case BranchWords.brge:isRelative=true;
valid=a>=b;break;case BranchWords.brgez:isRelative=true;valid=a>=b;break;case BranchWords.brgt:isRelative=true;valid=a>b;
break;case BranchWords.brgtz:isRelative=true;valid=a>b;break;case BranchWords.brle:isRelative=true;valid=a<=b;break;case
BranchWords.brlez:isRelative=true;valid=a<=b;break;case BranchWords.brlt:isRelative=true;valid=a<b;break;case BranchWords.brltz:
isRelative=true;valid=a<=b;break;case BranchWords.brna:isRelative=true;valid=Math.Abs(a-b)>Math.Max(c*Math.Max(Math.Abs(a),Math.
Abs(b)),double.Epsilon*8);break;case BranchWords.brnaz:isRelative=true;valid=Math.Abs(a-b)>Math.Max(c*Math.Max(Math.Abs(a),
Math.Abs(b)),double.Epsilon*8);break;case BranchWords.brne:isRelative=true;valid=a!=b;break;case BranchWords.brnez:
isRelative=true;valid=a!=b;break;}if(valid){if(isRelative){instruction.NextIndex=instruction.Index+address;}else{instruction.
NextIndex=address;}if(isRedirect){instruction.SetVar("ra",instruction.Index+1);}}else{instruction.NextIndex=instruction.Index+1;}
}}enum BranchWords{bap,bapal,bapz,bapzal,beq,beqal,beqz,beqzal,bge,bgeal,bgez,bgezal,bgt,bgtal,bgtz,bgtzal,ble,bleal,blez
,blezal,blt,bltal,bltz,bltzal,bna,bnaal,bnaz,bnazal,bne,bneal,bnez,bnezal,brap,brapz,breq,breqz,brge,brgez,brgt,brgtz,
brle,brlez,brlt,brltz,brna,brnaz,brne,brnez}class InterpretDevice{public static void AppendWords(Dictionary<string,
InstructionType>words){foreach(DeviceWords item in Enum.GetValues(typeof(DeviceWords))){words.Add(item.ToString(),InstructionType.
Device);}}public static void Interpret(Instruction instruction){var blockDevice=new BlockDevice(instruction);DeviceWords word;
Enum.TryParse(instruction.Name,out word);switch(word){case DeviceWords.device:{var device=instruction.GetArgumentString(1);
var filter=instruction.GetArgumentString(2);var prog=instruction.Parent.myProgram;BlockFilter<IMyTerminalBlock>block_filter
=BlockFilter<IMyTerminalBlock>.Create(prog.Me,filter);var items=BlockSystem<IMyTerminalBlock>.SearchByFilter(prog,
block_filter);instruction.SetDevice(device,items);}break;case DeviceWords.get:{var r=instruction.GetArgumentString(1);var device=
instruction.GetArgumentDevice<IMyTerminalBlock>(2);var a=instruction.GetArgumentString(3);var b=instruction.GetArgumentInt(4);
AggregationType aggregation=(AggregationType)b;var result=blockDevice.LoadDevice(device,a,aggregation);instruction.SetVar(r,result);}
break;case DeviceWords.inventory:{var r=instruction.GetArgumentString(1);var device=instruction.GetArgumentDevice<
IMyTerminalBlock>(2);var a=instruction.GetArgumentInt(3);var b=instruction.GetArgumentString(4);var c=instruction.GetArgumentInt(5);
AggregationType aggregation=(AggregationType)c;var result=blockDevice.InventoryDevice(device,a,b,aggregation);instruction.SetVar(r,
result);}break;case DeviceWords.set:{var device=instruction.GetArgumentDevice<IMyTerminalBlock>(1);var a=instruction.
GetArgumentString(2);var b=instruction.GetArgumentDouble(3);blockDevice.StoreDevice(device,a,b);}break;case DeviceWords.action:{var
device=instruction.GetArgumentDevice<IMyTerminalBlock>(1);var a=instruction.GetArgumentString(2);blockDevice.ActionDevice(
device,a);}break;case DeviceWords.color:{var device=instruction.GetArgumentDevice<IMyTerminalBlock>(1);var name=instruction.
GetArgumentString(2);var r=instruction.GetArgumentInt(3);var g=instruction.GetArgumentInt(4);var b=instruction.GetArgumentInt(5);var a=
instruction.GetArgumentInt(6);var color=new Color(r,g,b,a);blockDevice.SetColor(device,name,color);}break;case DeviceWords.colorhsv
:{var device=instruction.GetArgumentDevice<IMyTerminalBlock>(1);var name=instruction.GetArgumentString(2);var h=
instruction.GetArgumentDouble(3);var s=instruction.GetArgumentDouble(4);var v=instruction.GetArgumentDouble(5);var hsv=new Vector3(
h/360,s,v);var color=hsv.HSVtoColor();blockDevice.SetColor(device,name,color);}break;case DeviceWords.colorhex:{var
device=instruction.GetArgumentDevice<IMyTerminalBlock>(1);var name=instruction.GetArgumentString(2);var hex=instruction.
GetArgumentString(3);var color=ColorExtensions.HexToColor(hex);blockDevice.SetColor(device,name,color);}break;}}}enum DeviceWords{device,
get,set,action,inventory,color,colorhsv,colorhex}class InterpretJump{public static void AppendWords(Dictionary<string,
InstructionType>words){foreach(JumpWords item in Enum.GetValues(typeof(JumpWords))){words.Add(item.ToString(),InstructionType.Jump);}}
public static void Interpret(Instruction instruction){JumpWords word;Enum.TryParse(instruction.Name,out word);switch(word){
case JumpWords.j:case JumpWords.jal:{var address=instruction.GetArgumentAdress(1);instruction.NextIndex=address;if(word==
JumpWords.jal){instruction.SetVar("ra",instruction.Index+1);}}break;case JumpWords.jr:{int address=instruction.GetArgumentInt(1);
instruction.NextIndex=address;}break;}}}enum JumpWords{j,jal,jr}class InterpretLogic{public static void AppendWords(Dictionary<
string,InstructionType>words){foreach(LogicWords item in Enum.GetValues(typeof(LogicWords))){words.Add(item.ToString(),
InstructionType.Logic);}}public static void Interpret(Instruction instruction){LogicWords word;Enum.TryParse(instruction.Name,out word)
;var r=instruction.GetArgumentString(1);var a=instruction.GetArgumentDouble(2)>=1;var b=instruction.GetArgumentDouble(3)
>=1;bool value=false;switch(word){case LogicWords.and:value=a&b;break;case LogicWords.nor:value=!a&!b;break;case
LogicWords.or:value=a|b;break;case LogicWords.xor:value=a^b;break;case LogicWords.not:value=!a;break;}if(value){instruction.SetVar
(r,1d);}else{instruction.SetVar(r,0d);}}}enum LogicWords{and,nor,or,xor,not}class InterpretMath{public static void
AppendWords(Dictionary<string,InstructionType>words){foreach(MathWords item in Enum.GetValues(typeof(MathWords))){words.Add(item.
ToString(),InstructionType.Math);}}public static void Interpret(Instruction instruction){MathWords word;Enum.TryParse(
instruction.Name,out word);var r=instruction.GetArgumentString(1);var a=instruction.GetArgumentDouble(2);var b=instruction.
GetArgumentDouble(3);double value=0;switch(word){case MathWords.abs:value=Math.Abs(a);break;case MathWords.acos:value=Math.Acos(a);break;
case MathWords.add:value=a+b;break;case MathWords.asin:value=Math.Asin(a);break;case MathWords.atan:value=Math.Atan(a);break
;case MathWords.ceil:value=Math.Ceiling(a);break;case MathWords.cos:value=Math.Cos(a);break;case MathWords.div:value=a/b;
break;case MathWords.exp:value=Math.Exp(a);break;case MathWords.floor:value=Math.Floor(a);break;case MathWords.log:value=Math
.Log(a);break;case MathWords.max:value=Math.Max(a,b);break;case MathWords.min:value=Math.Min(a,b);break;case MathWords.
mod:value=a%b;break;case MathWords.mul:value=a*b;break;case MathWords.rand:var rand=new Random();value=rand.NextDouble();
break;case MathWords.round:value=Math.Round(a);break;case MathWords.sin:value=Math.Sin(a);break;case MathWords.sqrt:value=
Math.Sqrt(a);break;case MathWords.sub:value=a-b;break;case MathWords.tan:value=Math.Tan(a);break;case MathWords.trunc:value=
Math.Truncate(a);break;}instruction.SetVar(r,value);}}enum MathWords{abs,acos,add,asin,atan,ceil,cos,div,exp,floor,log,max,
min,mod,mul,rand,round,sin,sqrt,sub,tan,trunc}class InterpretMisk{public static void AppendWords(Dictionary<string,
InstructionType>words){foreach(MiskWords item in Enum.GetValues(typeof(MiskWords))){if(item!=MiskWords.label){words.Add(item.ToString()
,InstructionType.Misc);}}}public static void Interpret(Instruction instruction){MiskWords word;Enum.TryParse(instruction.
Name,out word);var r=instruction.GetArgumentString(1);var a=instruction.GetArgumentDouble(2);switch(word){case MiskWords.
define:{instruction.Parent.SetVar(r,a);}break;case MiskWords.yield:break;case MiskWords.move:{instruction.Parent.SetVar(r,a);}
break;case MiskWords.print:{a=instruction.GetArgumentDouble(1);instruction.Parent.Log($"{r}: {a}");}break;}}}enum MiskWords{
define,yield,move,print,label}class InterpretSelection{public static void AppendWords(Dictionary<string,InstructionType>words)
{foreach(SelectionWords item in Enum.GetValues(typeof(SelectionWords))){words.Add(item.ToString(),InstructionType.
Selection);}}public static void Interpret(Instruction instruction){SelectionWords word;Enum.TryParse(instruction.Name,out word);
var r=instruction.GetArgumentString(1);var a=instruction.GetArgumentDouble(2);var b=instruction.GetArgumentDouble(3);var c=
instruction.GetArgumentDouble(4);double value=0;var valid=false;switch(word){case SelectionWords.sap:valid=Math.Abs(a-b)<=Math.Max(
c*Math.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon*8);break;case SelectionWords.sapz:valid=Math.Abs(a-b)<=Math.Max(c*Math
.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon*8);break;case SelectionWords.select:valid=a!=0;break;case SelectionWords.seq
:valid=a==b;break;case SelectionWords.seqz:valid=a==b;break;case SelectionWords.sge:valid=a>=b;break;case SelectionWords.
sgez:valid=a>=b;break;case SelectionWords.sgt:valid=a>b;break;case SelectionWords.sgtz:valid=a>b;break;case SelectionWords.
sle:valid=a<=b;break;case SelectionWords.slez:valid=a<=b;break;case SelectionWords.slt:valid=a<b;break;case SelectionWords.
sltz:valid=a<b;break;case SelectionWords.sna:valid=Math.Abs(a-b)>Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon
*8);break;case SelectionWords.snaz:valid=Math.Abs(a-b)>Math.Max(c*Math.Max(Math.Abs(a),Math.Abs(b)),double.Epsilon*8);
break;case SelectionWords.sne:valid=a!=b;break;case SelectionWords.snez:valid=a!=b;break;}if(word==SelectionWords.select){
value=valid?b:c;}else{value=valid?1d:0d;}instruction.SetVar(r,value);}}enum SelectionWords{sap,sapz,select,seq,seqz,sge,sgez,
sgt,sgtz,sle,slez,slt,sltz,sna,snaz,sne,snez}enum StackWords{peek,pop,push}class TerminalActionComparer:IComparer<
ITerminalAction>{public int Compare(ITerminalAction x,ITerminalAction y){if(x!=null&&y!=null){return x.Id.CompareTo(y.Id);}else{return
0;}}}class TerminalPropertyComparer:IComparer<ITerminalProperty>{public int Compare(ITerminalProperty x,ITerminalProperty
y){if(x!=null&&y!=null){return x.Id.CompareTo(y.Id);}else{return 0;}}}class Util{static public string GetKiloFormat(
double value){double pow=1.0;string suffix="";if(value>1000.0){int y=int.Parse(Math.Floor(Math.Log10(value)/3).ToString());
suffix="KMGTPEZY".Substring(y-1,1);pow=Math.Pow(10,y*3);}return String.Format("{0:0.0}{1}",(value/pow),suffix);}static public
double RadToDeg(float angle){return angle*180/Math.PI;}static public double DegToRad(float angle){return angle*Math.PI/180;}
static public string GetType(MyInventoryItem inventory_item){return inventory_item.Type.TypeId;}static public string GetName(
MyInventoryItem inventory_item){return inventory_item.Type.SubtypeId;}static public string GetType(MyProductionItem production_item){
MyDefinitionId itemDefinitionId;string subtypeName=production_item.BlueprintId.SubtypeName;string typeName=Util.GetName(
production_item);if((subtypeName.EndsWith("Rifle")||subtypeName.StartsWith("Welder")||subtypeName.StartsWith("HandDrill")||subtypeName.
StartsWith("AngleGrinder"))&&MyDefinitionId.TryParse("MyObjectBuilder_PhysicalGunObject",typeName,out itemDefinitionId))return
itemDefinitionId.TypeId.ToString();if(subtypeName.StartsWith("Hydrogen")&&MyDefinitionId.TryParse("MyObjectBuilder_GasContainerObject",
typeName,out itemDefinitionId))return itemDefinitionId.TypeId.ToString();if(subtypeName.StartsWith("Oxygen")&&MyDefinitionId.
TryParse("MyObjectBuilder_OxygenContainerObject",typeName,out itemDefinitionId))return itemDefinitionId.TypeId.ToString();if((
subtypeName.Contains("Missile")||subtypeName.EndsWith("Magazine"))&&MyDefinitionId.TryParse("MyObjectBuilder_AmmoMagazine",typeName
,out itemDefinitionId))return itemDefinitionId.TypeId.ToString();if(MyDefinitionId.TryParse("MyObjectBuilder_Component",
typeName,out itemDefinitionId))return itemDefinitionId.TypeId.ToString();return production_item.BlueprintId.TypeId.ToString();}
static public string GetName(MyProductionItem production_item){string subtypeName=production_item.BlueprintId.SubtypeName;if(
subtypeName.EndsWith("Component"))subtypeName=subtypeName.Replace("Component","");if(subtypeName.EndsWith("Rifle")||subtypeName.
StartsWith("Welder")||subtypeName.StartsWith("HandDrill")||subtypeName.StartsWith("AngleGrinder"))subtypeName=subtypeName+"Item";
if(subtypeName.EndsWith("Magazine"))subtypeName=subtypeName.Replace("Magazine","");return subtypeName;}static public
string CutString(string value,int limit){if(value.Length>limit){int len=(limit-3)/2;return value.Substring(0,len)+"..."+value.
Substring(value.Length-len,len);}return value;}}
}internal static class IMyInventoryExtensions{public static void GetReflectionProperties(this IMyInventory inventory,List
<IReflectionProperty>resultList){resultList.Add(new ReflectionProperty<IMyInventory>("IsFull","Boolean",x=>(bool)x.IsFull
,"Use inventory command"));resultList.Add(new ReflectionProperty<IMyInventory>("CurrentMass","Single",x=>{double amount=0
;Double.TryParse(x.CurrentMass.ToString(),out amount);return amount;},"Use inventory command"));resultList.Add(new
ReflectionProperty<IMyInventory>("MaxVolume","Single",x=>{double amount=0;Double.TryParse(x.MaxVolume.ToString(),out amount);return amount
;},"Use inventory command"));resultList.Add(new ReflectionProperty<IMyInventory>("CurrentVolume","Single",x=>{double
amount=0;Double.TryParse(x.CurrentVolume.ToString(),out amount);return amount;},"Use inventory command"));resultList.Add(new
ReflectionProperty<IMyInventory>("ItemCount","Single",x=>(double)x.ItemCount,"Use inventory command"));resultList.Add(new
ReflectionProperty<IMyInventory>("VolumeFillFactor","Single",x=>(double)x.VolumeFillFactor,"Use inventory command"));}public static
IReflectionProperty GetReflectionProperty(this IMyInventory inventory,string id){List<IReflectionProperty>reflectionProperties=new List<
IReflectionProperty>();inventory.GetReflectionProperties(reflectionProperties);if(reflectionProperties.Count>0){return reflectionProperties
.First(x=>x.Id==id);}return null;}public static object GetReflectionValue(this IMyInventory inventory,string id){var
property=inventory.GetReflectionProperty(id);if(property==null)return null;return property.GetValue(inventory);}public static
TValue GetReflectionValue<TValue>(this IMyInventory inventory,string id){var property=inventory.GetReflectionProperty(id);if(
property==null)return default(TValue);return property.GetValue<TValue>(inventory);}}internal static class
IMyTerminalBlockExtensions{public static void GetReflectionProperties(this IMyTerminalBlock block,List<IReflectionProperty>resultList){if(block is
IMyMotorStator){resultList.Add(new ReflectionProperty<IMyMotorStator>("Angle","Single",x=>(double)x.Angle*180/Math.PI,
"Value in degres"));}if(block is IMyPistonBase){resultList.Add(new ReflectionProperty<IMyPistonBase>("CurrentPosition","Single",x=>(
double)x.CurrentPosition));}if(block is IMyShipMergeBlock){resultList.Add(new ReflectionProperty<IMyShipMergeBlock>(
"IsConnected","Boolean",x=>(bool)x.IsConnected));}if(block is IMySensorBlock){resultList.Add(new ReflectionProperty<IMySensorBlock>(
"IsActive","Boolean",x=>(bool)x.IsActive));}if(block is IMyPowerProducer){resultList.Add(new ReflectionProperty<IMyPowerProducer>(
"CurrentOutput","Single",x=>(double)x.CurrentOutput));resultList.Add(new ReflectionProperty<IMyPowerProducer>("MaxOutput","Single",x=>(
double)x.MaxOutput));resultList.Add(new ReflectionProperty<IMyPowerProducer>("CurrentOutputRatio","Single",x=>(double)x.
CurrentOutputRatio));}if(block is IMyBatteryBlock){resultList.Add(new ReflectionProperty<IMyBatteryBlock>("HasCapacityRemaining","Boolean"
,x=>(bool)x.HasCapacityRemaining));resultList.Add(new ReflectionProperty<IMyBatteryBlock>("CurrentStoredPower","Single",x
=>(double)x.CurrentStoredPower));resultList.Add(new ReflectionProperty<IMyBatteryBlock>("MaxStoredPower","Single",x=>(
double)x.MaxStoredPower));resultList.Add(new ReflectionProperty<IMyBatteryBlock>("CurrentInput","Single",x=>(double)x.
CurrentInput));resultList.Add(new ReflectionProperty<IMyBatteryBlock>("MaxInput","Single",x=>(double)x.MaxInput));resultList.Add(new
ReflectionProperty<IMyBatteryBlock>("IsCharging","Boolean",x=>(bool)x.IsCharging));}if(block is IMyLandingGear){resultList.Add(new
ReflectionProperty<IMyLandingGear>("IsLocked","Boolean",x=>(bool)x.IsLocked));resultList.Add(new ReflectionProperty<IMyLandingGear>(
"LockMode","Single",x=>(double)x.LockMode,"0:Unlocked 1:ReadyToLock 2:Locked"));resultList.Add(new ReflectionProperty<
IMyLandingGear>("IsParkingEnabled",ReflectionBindingFlags.ReadWrite,"Boolean",x=>(bool)x.IsParkingEnabled));}if(block is IMyAssembler)
{resultList.Add(new ReflectionProperty<IMyAssembler>("CurrentProgress","Single",x=>(double)x.CurrentProgress));resultList
.Add(new ReflectionProperty<IMyAssembler>("Mode",ReflectionBindingFlags.ReadWrite,"Single",x=>(double)x.Mode,
"0:Assembly 1:Disassembly"));resultList.Add(new ReflectionProperty<IMyAssembler>("CooperativeMode",ReflectionBindingFlags.ReadWrite,"Boolean",x=>(
bool)x.CooperativeMode));resultList.Add(new ReflectionProperty<IMyAssembler>("Repeating",ReflectionBindingFlags.ReadWrite,
"Boolean",x=>(bool)x.Repeating));}}public static IReflectionProperty GetReflectionProperty(this IMyTerminalBlock block,string id)
{List<IReflectionProperty>reflectionProperties=new List<IReflectionProperty>();block.GetReflectionProperties(
reflectionProperties);if(reflectionProperties.Count>0){return reflectionProperties.First(x=>x.Id==id);}return null;}public static object
GetReflectionValue(this IMyTerminalBlock block,string id){var property=block.GetReflectionProperty(id);if(property==null)return null;
return property.GetValue(block);}public static TValue GetReflectionValue<TValue>(this IMyTerminalBlock block,string id){var
property=block.GetReflectionProperty(id);if(property==null)return default(TValue);return property.GetValue<TValue>(block);}}
class InstructionException:Exception{public InstructionException():base(){}public InstructionException(string message):base(
message){}public InstructionException(string message,Exception innerException):base(message,innerException){}}public enum
ReflectionBindingFlags{Read,Write,ReadWrite}public interface IReflectionProperty:ITerminalProperty{ReflectionBindingFlags BindingFlags{get;}
string Description{get;}object GetValue(object block);TValue GetValue<TValue>(object block);}public class ReflectionProperty<T
>:IReflectionProperty where T:class{public ReflectionProperty(string id,string typeName,Func<T,object>lambdaGet,string
description=""){this.Id=id;this.BindingFlags=ReflectionBindingFlags.Read;this.TypeName=typeName;this.Description=description;this.
lambdaGet=lambdaGet;}public ReflectionProperty(string id,ReflectionBindingFlags bindingFlags,string typeName,Func<T,object>
lambdaGet,string description=""){this.Id=id;this.BindingFlags=bindingFlags;this.TypeName=typeName;this.Description=description;
this.lambdaGet=lambdaGet;}public string Id{get;}public ReflectionBindingFlags BindingFlags{get;}public string TypeName{get;}
public string Description{get;}protected Func<T,object>lambdaGet;public object GetValue(object block){return lambdaGet(block
as T);}public TValue GetValue<TValue>(object block){return(TValue)lambdaGet(block as T);}