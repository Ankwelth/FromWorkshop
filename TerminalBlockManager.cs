// Terminal Block Manager script by Handagotes
// -------------------------------------------
// Configurable variables:
const string FLAG = "[tbm script]";
const bool STATIC_MODE = false;
const bool LINE_MARKS = false;
const bool DISPLAY_SCRIPTED_BLOCKS = true;
// -------------------------------------------
// -------------Code starts here--------------

const string WHEN_WORD="when";
const string DO_WORD="do";
const string ONCE_WORD="once";
const string OF_WORD="of";
const string ENABLED_WORD="enabled";
const string DISABLED_WORD="disabled";
const string ENABLE_WORD="enable";
const string DISABLE_WORD="disable";
const string AND_WORD="and";
const string OR_WORD="or";
const string SWITCH_WORD="switch";
const string SET_WORD="set";
const string TO_WORD="to";
const string FOR_WORD="for";
const string WAIT_WORD="wait";
const string TRUE_WORD="true";
const string FALSE_WORD="false";

float statusTimer=0;
List<DeferredActionBlock>deferredActions=new List<DeferredActionBlock>();
List<CompletedActionBlockInfo>completedActions=new List<CompletedActionBlockInfo>();
List<ScriptedBlock>scriptedBlocks;

public Program()
{
Runtime.UpdateFrequency=UpdateFrequency.Update1;

if(STATIC_MODE)
{
scriptedBlocks=GetScriptedBlocks();
for(int i=0;i<scriptedBlocks.Count;++i)
{
if(CheckScript(scriptedBlocks[i])!="")
{
scriptedBlocks=null;
break;
}
}
}
}

public void Main()
{
Echo($"Terminal Block Manager {NextStatusString()}");

if(!STATIC_MODE)
{
scriptedBlocks=GetScriptedBlocks();
}
else
{
Echo("Static mode enabled.");

if(scriptedBlocks==null)
{
Echo("One or more scripted blocks have");
Echo("errors. Please, disable static mode");
Echo("and check the scripts for errors.");
return;
}
}

Echo($"{scriptedBlocks.Count} scripted blocks found.");
if(DISPLAY_SCRIPTED_BLOCKS&&scriptedBlocks.Count>0)
{
Echo("List of scripted blocks:");
scriptedBlocks.ForEach(i=>Echo(i.Block.CustomName));
}

if(!STATIC_MODE)
{
foreach(ScriptedBlock sblock in scriptedBlocks)
{
IMyTerminalBlock block=sblock.Block;
int errLogIndex=block.CustomData.IndexOf("\n* Error log:");
if(errLogIndex !=-1)
{
block.CustomData=block.CustomData.Remove(errLogIndex);
}

if(LINE_MARKS)
{
int lineBreaks=0;
string[]script=block.CustomData.Split('\n');
for(int i=1;i<script.Length;++i)
{
script[i]=$"[{i - lineBreaks}] {script[i]}";

if(System.Text.RegularExpressions.Regex.IsMatch(script[i]+'\n',@",[\s]*?\n"))
{
++lineBreaks;
}
}
block.CustomData=string.Join("\n",script);
}

var errLog=new StringBuilder("\n* Error log:\n");
string errs=CheckScript(sblock);
if(errs=="")
{
errLog.AppendLine("* No errors found.");
RunScript(sblock);
}
else
{
errLog.AppendLine(errs);
Echo($"{errs.Count(c => c == '\n')} errors found in '{block.CustomName}' script.");
}

block.CustomData+=errLog.ToString();
}
}
else
{
scriptedBlocks.ForEach(i=>RunScript(i));
}
}

List<ScriptedBlock>GetScriptedBlocks()
{
var blocks=new List<IMyTerminalBlock>();
var result=new List<ScriptedBlock>();

GridTerminalSystem.GetBlocksOfType(blocks,b=>b.CustomData.StartsWith(FLAG,StringComparison.OrdinalIgnoreCase));
blocks.ForEach(b=>result.Add(new ScriptedBlock(b)));

return result;
}

string CheckScript(ScriptedBlock scriptedBlock)
{
var errLog=new StringBuilder();
IMyTerminalBlock block=scriptedBlock.Block;

for(int i=1;i<scriptedBlock.Script.Length;++i)
{
string[]words=scriptedBlock.Script[i];
if(words.Length>0)
{
if(words[0]!=WHEN_WORD)
{
AddErrorInLog(errLog,i,$"Script must start with '{WHEN_WORD}' keyword.");
}
break;
}
}

bool waitAction=false;

for(int i=1;i<scriptedBlock.Script.Length;++i)
{
string[]words=scriptedBlock.Script[i];
if(words.Length==0)
{
continue;
}

if(words[0]==WHEN_WORD)
{
waitAction=false;
if(words.Length==1)
{
AddErrorInLog(errLog,i,"Found end of the line but expected logical expression.");
}
else
{
int doPos=Array.IndexOf(words,DO_WORD,1);
if(doPos==-1)
{
AddErrorInLog(errLog,i,$"Found end of the line but expected '{DO_WORD}'.");
}
else if(doPos<words.Length-1&&words[doPos+1]!=ONCE_WORD)
{
AddErrorInLog(errLog,i,$"Found '{words[doPos + 1]}' but expected end of the line or '{ONCE_WORD}'.");
}
else if(doPos+1<words.Length-1&&words[doPos+1]==ONCE_WORD)
{
AddErrorInLog(errLog,i,$"Found '{words[doPos + 2]}' but expected end of the line.");
}
else
{
string[]exp=GetSubArray(words,1,doPos-1);
var terminalUnit=new TerminalUnit(block);
string error=CheckGettingValueExp(terminalUnit,exp);
if(error !="")
{
AddErrorInLog(errLog,i,error);
}
else if(!(GetValueExp(terminalUnit,exp)is bool))
{
AddErrorInLog(errLog,i,$"Expression {string.Join(" ", exp)} does not return boolean value.");
}
}
}
}
else
{
if(words[0]==WAIT_WORD)
{
if(waitAction)
{
return $"There is only one allowed '{WAIT_WORD}' action in one '{WHEN_WORD}' block.";
}
else
{
waitAction=true;
}
}

string error=CheckScriptActionExp(new TerminalUnit(block),words);
if(error !="")
{
AddErrorInLog(errLog,i,error);
}
}
}

return errLog.ToString();
}

void RunScript(ScriptedBlock scriptedBlock)
{
var terminalUnit=new TerminalUnit(scriptedBlock.Block);
string[][]script=scriptedBlock.Script;
bool executeBlockFlag=false;
bool actionCompleteFlag=false;
int whenBlockIndex=0;
int nextWhenBlockPos=0;

for(int i=1;i<script.Length;++i)
{
string[]words=scriptedBlock.Script[i];
if(words.Length==0)
{
continue;
}
else if(words[0]==WHEN_WORD)
{
++whenBlockIndex;
nextWhenBlockPos=Array.FindIndex(script,i+1,lineWords=>lineWords.Length>0&&lineWords[0]==WHEN_WORD);
int doPos=Array.IndexOf(words,DO_WORD,1);
string[]logicalExp=GetSubArray(words,1,doPos-1);

executeBlockFlag=(bool)GetValueExp(terminalUnit,logicalExp);
actionCompleteFlag=completedActions.Exists(j=>j.TerminalUnit.Equals(terminalUnit)&&j.WhenBlockIndex==whenBlockIndex);

if(words.Last()==ONCE_WORD)
{
if(executeBlockFlag)
{
if(actionCompleteFlag)
{
executeBlockFlag=false;
}
else
{
completedActions.Add(new CompletedActionBlockInfo(terminalUnit,whenBlockIndex));
}
}
else
{
completedActions.RemoveAll(j=>j.TerminalUnit.Equals(terminalUnit)&&j.WhenBlockIndex==whenBlockIndex);
}
}
}
else if(executeBlockFlag)
{
if(words[0]==WAIT_WORD)
{
if(!deferredActions.Exists(item=>item.TerminalUnit.Equals(terminalUnit)&&item.WhenBlockIndex==whenBlockIndex))
{
float seconds=float.Parse(words[1]);
string[][]actions=nextWhenBlockPos>-1 ? GetSubArray(script,i+1,nextWhenBlockPos-1-i):
GetSubArray(script,i+1);

deferredActions.Add(new DeferredActionBlock(seconds,whenBlockIndex,terminalUnit,actions,i+1));
}

i=nextWhenBlockPos>-1 ? nextWhenBlockPos-1:script.Length;
}
else
{
RunScriptAction(terminalUnit,words,scriptedBlock.OriginalLines[i]);
}
}
else
{
i=nextWhenBlockPos>-1 ? nextWhenBlockPos-1:script.Length;
}
}

for(int i=0;i<deferredActions.Count;++i)
{
if(terminalUnit.Equals(deferredActions[i].TerminalUnit))
{
DeferredActionBlock defAct=deferredActions[i];
defAct.AddElapsedTime((float)Runtime.TimeSinceLastRun.TotalSeconds);
if(defAct.Ready)
{
int originalLineIdx=defAct.FirstOriginalLineActionIndex;

foreach(string[]actionExp in defAct.Actions)
{
if(actionExp.Length>0)
{
string originalLine=scriptedBlock.OriginalLines[originalLineIdx++];
RunScriptAction(defAct.TerminalUnit,actionExp,originalLine);
}
}
deferredActions.RemoveAtFast(i--);
}
}
}
}

void AddErrorInLog(StringBuilder errorLog,int lineIndex,string message)
{
errorLog.AppendFormat("* Line {0}: {1}\n",lineIndex,message);
}

string CheckScriptActionExp(TerminalUnit terminalUnit,string[]exp)
{
switch(exp[0])
{
case FOR_WORD:
{
if(exp.Length==1)
{
return "Found end of the line but expected block/block group name.";
}
else if(exp.Length==2)
{
return $"Found end of the line but expected '{DO_WORD}'.";
}
else if(exp[2]!=DO_WORD)
{
return $"Found '{exp[2]}' but expected '{DO_WORD}'.";
}
else
{
var wantedUnit=TerminalUnit.Find(GridTerminalSystem,exp[1]);
if(wantedUnit==null)
{
return $"Terminal has no block/block group '{exp[1]}'.";
}
else
{
string[]actionExp=GetSubArray(exp,3);
if(actionExp.Length==0)
{
return "Found end of the line but expected expression.";
}
else if(actionExp[0]==WAIT_WORD)
{
return $"Using '{WAIT_WORD}' keyword is not allowed in '{FOR_WORD}' construction.";
}
else
{
return CheckScriptActionExp(wantedUnit,actionExp);
}
}
}
}
case SET_WORD:
{
if(exp.Length==1)
{
return "Found end of the line but expected property name.";
}
else if(exp.Length==2)
{
return $"Found end of the line but expected '{TO_WORD}'.";
}
else if(exp[2]!=TO_WORD)
{
return $"Found '{exp[2]}' but expected '{TO_WORD}'.";
}
else if(exp.Length==3)
{
return "Found end of the line but expected value.";
}
else
{
object propertyValue=null;
if(terminalUnit.Type==TerminalUnitType.Block)
{
propertyValue=terminalUnit.GetValue(exp[1]);
if(propertyValue==null)
{
return $"Block '{terminalUnit.Name}' has no property '{exp[1]}'.";
}
}
else
{
var blockList=new List<IMyTerminalBlock>();
terminalUnit.AsGroup().GetBlocks(blockList);
foreach(IMyTerminalBlock block in blockList)
{
propertyValue=new TerminalUnit(block).GetValue(exp[1]);
if(propertyValue !=null)
{
break;
}
}

if(propertyValue==null)
{
return $"None blocks in group '{terminalUnit.Name}' has property '{exp[1]}'.";
}
}

string[]gettingValueExp=GetSubArray(exp,3);
string error=CheckGettingValueExp(terminalUnit,gettingValueExp);
if(error !="")
{
return error;
}
else
{
object value=GetValueExp(terminalUnit,gettingValueExp);
if(propertyValue.GetType()!=value.GetType())
{
return $"Found type mismatch in expression '{string.Join(" ", exp)}'.";
}
else if(terminalUnit.IsPropertyReadOnly(exp[1]))
{
return "Can't set value to read-only property.";
}
else
{
return "";
}
}
}
}
case ENABLE_WORD:
case DISABLE_WORD:
case SWITCH_WORD:
{
if(exp.Length==1)
{
return "Found end of the line but expected block/block group name.";
}
else
{
var wantedUnit=TerminalUnit.Find(GridTerminalSystem,exp[1]);
if(wantedUnit==null)
{
return $"Terminal has no block/block group '{exp[1]}'.";
}
else if(exp.Length>2)
{
return $"Found '{exp[2]}' but expected end of the line.";
}
else if(wantedUnit.Type==TerminalUnitType.Block)
{
if(!(wantedUnit.Unit is IMyFunctionalBlock))
{
return $"Block '{exp[1]}' is not functional.";
}
else
{
return "";
}
}
else
{
var funcBlockList=new List<IMyFunctionalBlock>();
wantedUnit.AsGroup().GetBlocksOfType(funcBlockList);
if(funcBlockList.Count==0)
{
return $"None block in group '{exp[1]}' is functional.";
}
else
{
return "";
}
}
}
}
case WAIT_WORD:
{
if(exp.Length==1)
{
return "Found end of the line but expected time in seconds.";
}
else
{
float seconds;
if(!float.TryParse(exp[1],out seconds))
{
return $"Found '{exp[1]}' but expected number value.";
}
else if(seconds<0||seconds>600)
{
return $"Value must be between 0 and 600 seconds.";
}
else if(exp.Length>2)
{
return $"Found '{exp[2]}' but expected end of the line.";
}
else
{
return "";
}
}
}
default:
{
if(terminalUnit.FindTerminalAction(exp[0])==null)
{
if(terminalUnit.Type==TerminalUnitType.Block)
{
return $"Block '{terminalUnit.Name}' has no action '{exp[0]}'.";
}
else
{
return $"None block in group '{terminalUnit.Name}' has action '{exp[0]}'.";
}
}
else
{
return "";
}
}
}
}

string CheckLogicalExp(TerminalUnit terminalUnit,string[]exp)
{
if(exp.Length==0)
{
return "Found an empty expression.";
}
else if(exp.Count(i=>i=="(")!=exp.Count(i=>i==")"))
{
return "Wrong amount of parentheses.";
}
else if(!Array.Exists(exp,i=>i==AND_WORD||i==OR_WORD))
{
return CheckComparisonExp(terminalUnit,exp);
}
else
{
int prevOpPos=-1;
int opPos=Array.FindIndex(exp,0,i=>i==AND_WORD||i==OR_WORD);
do
{
if(opPos==0||exp[opPos-1]=="(")
{
return "Found an empty expression before logical operator.";
}
else if(opPos==exp.Length-1||exp[opPos+1]==")")
{
return "Found an empty expression after logical operator.";
}
else
{
string error=CheckComparisonExp(terminalUnit,GetSubArray(exp,prevOpPos+1,opPos-(prevOpPos+1)));
if(error !="")
{
return error;
}
else
{
int nextOpPos=Array.FindIndex(exp,opPos+1,i=>i==AND_WORD||i==OR_WORD);
nextOpPos=nextOpPos>-1 ? nextOpPos:exp.Length;

error=CheckComparisonExp(terminalUnit,GetSubArray(exp,opPos+1,nextOpPos-(opPos+1)));
if(error !="")
{
return error;
}
}
}

prevOpPos=opPos;
opPos=Array.FindIndex(exp,opPos+1,i=>i==AND_WORD||i==OR_WORD);
}
while(opPos>-1);

return "";
}
}

string CheckComparisonExp(TerminalUnit terminalUnit,string[]exp)
{
exp=Array.FindAll(exp,i=>i !="("&&i !=")");
TerminalUnit checkingUnit=terminalUnit;

int opPos=Array.FindIndex(exp,i=>i=="<="||
i==">="||
i=="<"||
i==">"||
i=="!="||
i=="=");
if(opPos==-1)
{
string error=CheckGettingValueExp(terminalUnit,exp);
if(error !="")
{
return error;
}
else if(!(GetValueExp(terminalUnit,exp)is bool))
{
return "Found end of the expression but expected comparison symbol.";
}
else
{
return "";
}
}
else if(opPos==0)
{
return $"Found an empty expression before '{exp[opPos]}' comparison operator.";
}
else if(opPos==exp.Length-1)
{
return $"Found an empty expression after '{exp[opPos]}' comparison operator.";
}
else
{
string[]leftExp=GetSubArray(exp,0,opPos);
string[]rightExp=GetSubArray(exp,opPos+1);

string error=CheckGettingValueExp(terminalUnit,leftExp);
if(error !="")
{
return error;
}
else
{
if(leftExp.Length==3&&leftExp[1]==OF_WORD)
{
checkingUnit=TerminalUnit.Find(GridTerminalSystem,leftExp[2])?? terminalUnit;
}

error=CheckGettingValueExp(checkingUnit,rightExp);
if(error !="")
{
return error;
}
}

object leftValue=GetValueExp(terminalUnit,leftExp);
object rightValue=GetValueExp(checkingUnit,rightExp);
if(leftValue.GetType()!=rightValue.GetType())
{
return $"Found a type mismatch in expression '{string.Join(" ", exp)}'.";
}
else if(exp[opPos]!="="&&exp[opPos]!="!=")
{
if(leftValue is bool)
{
return $"Can't compare bool values with '{exp[opPos]}' comparison symbol.";
}
else if(leftValue is Color)
{
return $"Can't compare color values with '{exp[opPos]}' comparison symbol.";
}
else if(leftValue is Enum)
{
return $"Can't compare enum values with '{exp[opPos]}' comparison symbol.";
}
else
{
return "";
}
}
else
{
return "";
}
}
}

string CheckGettingValueExp(TerminalUnit terminalUnit,string[]exp)
{
if(IsLogicalExp(exp))
{
return CheckLogicalExp(terminalUnit,exp);
}
else if(exp[0]==ENABLED_WORD||exp[0]==DISABLED_WORD)
{
if(exp.Length==1)
{
return "Found end of the expression but expected block name.";
}
else
{
var checkingUnit=TerminalUnit.Find(GridTerminalSystem,exp[1]);
if(checkingUnit==null||checkingUnit.Type==TerminalUnitType.Group)
{
return $"Terminal has no block '{exp[1]}'.";
}
else if(exp.Length>2)
{
return $"Found '{exp[2]}' but expected end of the expression.";
}
else if(!(checkingUnit.Unit is IMyFunctionalBlock))
{
return $"Block '{exp[1]}' is not functional.";
}
else
{
return "";
}
}
}
else if(exp.Length==1)
{
decimal decimalResult;
if(decimal.TryParse(exp[0],out decimalResult))
{
return "";
}
else if(exp[0]==TRUE_WORD||
exp[0]==FALSE_WORD)
{
return "";
}
else if(GetEnumValue(terminalUnit,exp[0])!=null)
{
return "";
}
else
{
object value;
try
{
value=terminalUnit.GetValue(exp[0]);
}
catch(Exception e)
{
return e.Message;
}

if(value==null)
{
return $"Block '{terminalUnit.Name}' has no property '{exp[0]}'.";
}
else
{
return "";
}
}
}
else
{
int r,g,b;
if(exp.Length==3&&
int.TryParse(exp[0],out r)&&
int.TryParse(exp[1],out g)&&
int.TryParse(exp[2],out b))
{
return "";
}
else if(exp[1]!=OF_WORD)
{
return $"Found '{exp[1]}' but expected end of the expression.";
}
else if(exp.Length==2)
{
return $"Found end of the expression but expected name of block.";
}
else
{
var wantedBlock=TerminalUnit.Find(GridTerminalSystem,exp[2]);
if(wantedBlock==null||wantedBlock.Type==TerminalUnitType.Group)
{
return $"Terminal has no block with name '{exp[2]}'.";
}
else if(exp.Length>3)
{
return $"Found '{exp[3]}' but expected end of the expression.";
}
else
{
return CheckGettingValueExp(wantedBlock,GetSubArray(exp,0,1));
}
}
}
}

void RunScriptAction(TerminalUnit terminalUnit,string[]exp,string originalLine)
{
switch(exp[0])
{
case FOR_WORD:
{
var wantedUnit=TerminalUnit.Find(GridTerminalSystem,exp[1]);
string[]actionExp=GetSubArray(exp,3);
RunScriptAction(wantedUnit,actionExp,originalLine);
break;
}
case SET_WORD:
{
string[]gettingValueExp=GetSubArray(exp,3);
terminalUnit.SetValue(exp[1],GetValueExp(terminalUnit,gettingValueExp));
break;
}
case ENABLE_WORD:
{
var wantedUnit=TerminalUnit.Find(GridTerminalSystem,exp[1]);
wantedUnit.ApplyAction("onoff_on");
break;
}
case DISABLE_WORD:
{
var wantedUnit=TerminalUnit.Find(GridTerminalSystem,exp[1]);
wantedUnit.ApplyAction("onoff_off");
break;
}
case SWITCH_WORD:
{
var wantedUnit=TerminalUnit.Find(GridTerminalSystem,exp[1]);
wantedUnit.ApplyAction("onoff");
break;
}
case WAIT_WORD:
{
break;
}
default:
{
if(exp.Length==1)
{
terminalUnit.ApplyAction(exp[0]);
break;
}

var parameters=new List<TerminalActionParameter>();
var paramsMatches=System.Text.RegularExpressions.Regex.Matches(originalLine,"`.+?`");

foreach(System.Text.RegularExpressions.Match match in paramsMatches)
{
string parameter=match.Value.Trim('`');
parameters.Add(TerminalActionParameter.Get(parameter));
}

terminalUnit.ApplyAction(exp[0],parameters);

break;
}
}
}

bool ParseLogicalExp(TerminalUnit terminalUnit,string[]exp)
{
if(exp[0]=="(")
{
int len=ExternalSearch(exp,")",1);
if(len==exp.Length-1)
{
return ParseLogicalExp(terminalUnit,GetSubArray(exp,1,len-1));
}
}

int opPos=ExternalSearch(exp,OR_WORD);
opPos=opPos>-1 ? opPos:ExternalSearch(exp,AND_WORD);

if(opPos>-1)
{
string[]left=GetSubArray(exp,0,opPos);
string[]right=GetSubArray(exp,opPos+1);

if(exp[opPos]==AND_WORD)
{
return ParseLogicalExp(terminalUnit,left)&&ParseLogicalExp(terminalUnit,right);
}
else
{
return ParseLogicalExp(terminalUnit,left)||ParseLogicalExp(terminalUnit,right);
}
}
else
{
return ParseComparisonExp(terminalUnit,exp);
}
}

bool ParseComparisonExp(TerminalUnit terminalUnit,string[]exp)
{
int opPos=Array.FindIndex(exp,i=>i=="<="||
i==">="||
i=="<"||
i==">"||
i=="!="||
i=="=");
if(opPos==-1)
{
return(bool)GetValueExp(terminalUnit,exp);
}
else
{
TerminalUnit checkingUnit=terminalUnit;
{
string[]leftExp=GetSubArray(exp,0,opPos);
if(leftExp.Length==3&&leftExp[1]==OF_WORD)
{
checkingUnit=TerminalUnit.Find(GridTerminalSystem,leftExp[2])?? terminalUnit;
}
}

object leftValue=GetValueExp(terminalUnit,GetSubArray(exp,0,opPos));
object rightValue=GetValueExp(checkingUnit,GetSubArray(exp,opPos+1));

if(leftValue is float)
{
decimal left=FloatToDecimal((float)leftValue);
decimal right=FloatToDecimal((float)rightValue);
int decimals=Math.Min(GetDecimalPlaces(left),GetDecimalPlaces(right));
decimals=decimals>15 ? 15:decimals;
left=Math.Round(left,decimals);
right=Math.Round(right,decimals);

switch(exp[opPos])
{
case "<=":return left<=right;
case ">=":return left>=right;
case "<":return left<right;
case ">":return left>right;
case "=":return left==right;
case "!=":return left !=right;
}
}
else
{
if(leftValue is bool)
{
bool left=(bool)leftValue;
bool right=(bool)rightValue;

switch(exp[opPos])
{
case "=":return left==right;
case "!=":return left !=right;
}
}
else if(leftValue is Color)
{
var left=(Color)leftValue;
var right=(Color)rightValue;

switch(exp[opPos])
{
case "=":return left==right;
case "!=":return left !=right;
}
}
else if(leftValue is Enum)
{
var left=(Enum)leftValue;
var right=(Enum)rightValue;

switch(exp[opPos])
{
case "=":return left.Equals(right);
case "!=":return !left.Equals(right);
}
}
}
}

throw new Exception("Unknown error.");
}

object GetValueExp(TerminalUnit terminalUnit,string[]exp)
{
if(IsLogicalExp(exp))
{
return ParseLogicalExp(terminalUnit,exp);
}
else if(exp[0]==ENABLED_WORD||exp[0]==DISABLED_WORD)
{
var checkingBlock=TerminalUnit.Find(GridTerminalSystem,exp[1]).Unit as IMyFunctionalBlock;
if(exp[0]==ENABLED_WORD)
{
return checkingBlock.Enabled;
}
else
{
return !checkingBlock.Enabled;
}
}
else if(exp.Length==1)
{
float floatResult;
if(float.TryParse(exp[0],out floatResult))
{
return floatResult;
}
else if(exp[0]==TRUE_WORD)
{
return true;
}
else if(exp[0]==FALSE_WORD)
{
return false;
}
else
{
return GetEnumValue(terminalUnit,exp[0])?? terminalUnit.GetValue(exp[0]);
}
}
else
{
int r,g,b;
if(exp.Length==3&&
int.TryParse(exp[0],out r)&&
int.TryParse(exp[1],out g)&&
int.TryParse(exp[2],out b))
{
return new Color(r,g,b);
}
else
{
var wantedBlock=TerminalUnit.Find(GridTerminalSystem,exp[2]);
return GetValueExp(wantedBlock,GetSubArray(exp,0,1));
}
}
}

object GetEnumValue(TerminalUnit terminalUnit,string value)
{
var block=terminalUnit.AsBlock();
if(block is IMyDoor||block is IMyParachute)
{
DoorStatus result;
if(Enum.TryParse(value,true,out result))
{
return result;
}
}
else if(block is IMyAirVent)
{
VentStatus result;
if(Enum.TryParse(value,true,out result))
{
return result;
}
}
else if(block is IMyJumpDrive)
{
MyJumpDriveStatus result;
if(Enum.TryParse(value,true,out result))
{
return result;
}
}
else if(block is IMyLaserAntenna)
{
MyLaserAntennaStatus result;
if(Enum.TryParse(value,true,out result))
{
return result;
}
}
else if(block is IMyShipConnector)
{
MyShipConnectorStatus result;
if(Enum.TryParse(value,true,out result))
{
return result;
}
}
else if(block is IMyPistonBase)
{
PistonStatus result;
if(Enum.TryParse(value,true,out result))
{
return result;
}
}
else if(block is IMyLargeTurretBase||block is IMyCameraBlock)
{
MyDetectedEntityType result;
if(Enum.TryParse(value,true,out result))
{
return result;
}
}

return null;
}

bool IsLogicalExp(string[]exp)
{
return Array.Exists(exp,i=>i=="<"||
i==">"||
i=="<="||
i==">="||
i=="!="||
i=="="||
i==OR_WORD||
i==AND_WORD);
}

decimal FloatToDecimal(float n)
{
try
{
return Convert.ToDecimal(n);
}
catch
{
if(n>0)
{
return decimal.MaxValue;
}
else
{
return decimal.MinValue;
}
}
}

int GetDecimalPlaces(decimal n)
{
n=Math.Abs(n);
n-=Math.Truncate(n);

int decimalPlaces=0;
while(n>0)
{
++decimalPlaces;
n *=10;
n-=Math.Truncate(n);
}
return decimalPlaces;
}

int ExternalSearch(string[]array,string match,int start=0)
{
int i=start;
int skips=0;
while(i<array.Length&&(array[i]!=match||skips>0))
{
if(array[i]=="(")
{
++skips;
}
else if(array[i]==")")
{
--skips;
}
++i;
}
if(i==array.Length)
{
return-1;
}
else
{
return i;
}
}

T[]GetSubArray<T>(T[]array,int start,int length=-1)
{
if(length<0)
{
length=array.Length-start;
}

T[]result=new T[length];
for(int i=0;i<length;i++)
{
result[i]=array[start+i];
}
return result;
}

string NextStatusString()
{
statusTimer+=Runtime.TimeSinceLastRun.Milliseconds * 0.06f;
if(statusTimer<10)
{
return ">";
}
else if(statusTimer<20)
{
return ">>";
}
else if(statusTimer<30)
{
return ">>>";
}
else if(statusTimer<40)
{
return "  >>";
}
else if(statusTimer<50)
{
return "    >";
}
else if(statusTimer<60)
{
return "";
}
else
{
statusTimer=0;
return "";
}
}

class ScriptedBlock
{
public IMyTerminalBlock Block{get;}
public string[][]Script{get;private set;}
public string[]OriginalLines{get;private set;}

public ScriptedBlock(IMyTerminalBlock block)
{
Block=block;
SetupScript();
}

private void SetupScript()
{
Block.CustomData=System.Text.RegularExpressions.Regex.Replace(Block.CustomData,@"\[\d+\] ","");
string script=System.Text.RegularExpressions.Regex.Replace(Block.CustomData,@",[\s]*?\n"," ");
string[]lines=script.Split('\n');
string[][]result=new string[lines.Length][];

for(int i=0;i<lines.Length;++i)
{
result[i]=FormatLine(lines[i]);
}

Script=result;
OriginalLines=lines;
}

private string[]FormatLine(string line)
{
line=line.ToLower();
line=System.Text.RegularExpressions.Regex.Replace(line,@"\*[^\n]*","");
line=System.Text.RegularExpressions.Regex.Replace(line,@"(\d),(\d)","$1.$2");
line=System.Text.RegularExpressions.Regex.Replace(line,@"(<=)|(>=)|(!=)|[<>=();]"," $& ");
return line.Split(default(string[]),StringSplitOptions.RemoveEmptyEntries);
}
}

enum TerminalUnitType{Block,Group}

class TerminalUnit:IEquatable<TerminalUnit>
{
public object Unit{get;}
public string Name{get;}
public TerminalUnitType Type{get;}

public TerminalUnit(IMyTerminalBlock block)
{
if(block==null)
{
throw new Exception("'block' argument is null.");
}
Unit=block;
Name=block.CustomName;
Type=TerminalUnitType.Block;
}

public TerminalUnit(IMyBlockGroup group)
{
if(group==null)
{
throw new Exception("'group' argument is null.");
}
Unit=group;
Name=group.Name;
Type=TerminalUnitType.Group;
}

public ITerminalAction FindTerminalAction(string actionName)
{
string actionNameWithSpaces=actionName.Replace("_"," ");

if(Type==TerminalUnitType.Block)
{
var block=this.AsBlock();
var actionList=new List<ITerminalAction>();

block.GetActions(actionList,action=>string.Equals(action.Id,actionName,StringComparison.OrdinalIgnoreCase)||
string.Equals(action.Id,actionNameWithSpaces,StringComparison.OrdinalIgnoreCase));
if(actionList.Count>0)
{
return actionList[0];
}
else
{
return null;
}
}
else
{
var blockList=new List<IMyTerminalBlock>();

this.AsGroup().GetBlocks(blockList);
foreach(IMyTerminalBlock block in blockList)
{
var actionList=new List<ITerminalAction>();

block.GetActions(actionList,action=>string.Equals(action.Id,actionName,StringComparison.OrdinalIgnoreCase)||
string.Equals(action.Id,actionNameWithSpaces,StringComparison.OrdinalIgnoreCase));
if(actionList.Count>0)
{
return actionList[0];
}
}
return null;
}
}

public ITerminalProperty FindTerminalProperty(string propertyName)
{
string propertyNameWithSpaces=propertyName.Replace("_"," ");

if(Type==TerminalUnitType.Block)
{
var resultList=new List<ITerminalProperty>();

this.AsBlock().GetProperties(resultList,i=>string.Equals(i.Id,propertyName,StringComparison.OrdinalIgnoreCase)||
string.Equals(i.Id,propertyNameWithSpaces,StringComparison.OrdinalIgnoreCase));
if(resultList.Count>0)
{
return resultList[0];
}
else
{
return null;
}
}
else
{
var blockList=new List<IMyTerminalBlock>();

this.AsGroup().GetBlocks(blockList);
foreach(IMyTerminalBlock block in blockList)
{
var resultList=new List<ITerminalProperty>();

block.GetProperties(resultList,i=>string.Equals(i.Id,propertyName,StringComparison.OrdinalIgnoreCase)||
string.Equals(i.Id,propertyNameWithSpaces,StringComparison.OrdinalIgnoreCase));
if(resultList.Count>0)
{
return resultList[0];
}
}
return null;
}
}

public void ApplyAction(string actionName,List<TerminalActionParameter>parameters=null)
{
ITerminalAction action=FindTerminalAction(actionName);

if(action==null)
{
throw new Exception($"Unknown action '{actionName}'.");
}
else
{
if(Type==TerminalUnitType.Block)
{
action.Apply(this.AsBlock(),parameters);
}
else
{
var blockList=new List<IMyTerminalBlock>();

this.AsGroup().GetBlocks(blockList);
foreach(IMyTerminalBlock block in blockList)
{
action.Apply(block,parameters);
}
}
}
}

public void SetValue(string propertyName,object value)
{
ITerminalProperty property=FindTerminalProperty(propertyName);

if(property !=null)
{
if(Type==TerminalUnitType.Block)
{
SetTerminalPropertyValue(this.AsBlock(),property,value);
}
else
{
var blockList=new List<IMyTerminalBlock>();

this.AsGroup().GetBlocks(blockList);
foreach(IMyTerminalBlock block in blockList)
{
if(block.GetProperty(property.Id)!=null)
{
SetTerminalPropertyValue(block,property,value);
}
}
}
}
else
{
if(Type==TerminalUnitType.Block)
{
new AdditionalProperty(this.AsBlock(),propertyName).SetValue(value);
}
else
{
var blockList=new List<IMyTerminalBlock>();

this.AsGroup().GetBlocks(blockList);
foreach(IMyTerminalBlock block in blockList)
{
try
{
new AdditionalProperty(block,propertyName).SetValue(value);
}
catch
{
}
}
}
}
}

public object GetValue(string propertyName)
{
if(Type==TerminalUnitType.Block)
{
var block=this.AsBlock();
ITerminalProperty property=FindTerminalProperty(propertyName);

if(property !=null)
{
switch(property.TypeName)
{
case "Single":
case "Double":
case "Float":return block.GetValueFloat(property.Id);
case "Boolean":return block.GetValueBool(property.Id);
case "Color":return block.GetValueColor(property.Id);
default:throw new Exception($"Property type '{property.TypeName}' is not supported.");
}
}
else
{
try
{
return new AdditionalProperty(block,propertyName).GetValue();
}
catch
{
return null;
}
}
}
else
{
throw new Exception("Can't get value from group.");
}
}

public bool IsPropertyReadOnly(string propertyName)
{
if(FindTerminalProperty(propertyName)!=null)
{
return false;
}
else
{
if(Type==TerminalUnitType.Block)
{
var block=this.AsBlock();

try
{
return new AdditionalProperty(block,propertyName).ReadOnly;
}
catch
{
throw new Exception($"Block '{block.CustomName}' has no property '{propertyName}'.");
}
}
else
{
var blockList=new List<IMyTerminalBlock>();
this.AsGroup().GetBlocks(blockList);
foreach(IMyTerminalBlock block in blockList)
{
try
{
return new AdditionalProperty(block,propertyName).ReadOnly;
}
catch
{
}
}

throw new Exception($"None block in group '{Name}' has no property '{propertyName}'.");
}
}
}

public IMyTerminalBlock AsBlock()
{
if(Type==TerminalUnitType.Block)
{
return Unit as IMyTerminalBlock;
}
else
{
throw new Exception($"Terminal unit '{Name}' is not a block.");
}
}

public IMyBlockGroup AsGroup()
{
if(Type==TerminalUnitType.Group)
{
return Unit as IMyBlockGroup;
}
else
{
throw new Exception($"Terminal unit '{Name}' is not a group.");
}
}

public bool Equals(TerminalUnit other)
{
return Unit==other.Unit;
}

public static TerminalUnit Find(IMyGridTerminalSystem gridTerminalSystem,string name)
{
name=name.Replace("_"," ");

var blockList=new List<IMyTerminalBlock>();
gridTerminalSystem.GetBlocks(blockList);
foreach(IMyTerminalBlock block in blockList)
{
string blockName=block.CustomName.Replace("_"," ");
if(string.Equals(blockName,name,StringComparison.OrdinalIgnoreCase))
{
return new TerminalUnit(block);
}
}

var groupList=new List<IMyBlockGroup>();
gridTerminalSystem.GetBlockGroups(groupList);
foreach(IMyBlockGroup group in groupList)
{
string groupName=group.Name.Replace("_"," ");
if(string.Equals(groupName,name,StringComparison.OrdinalIgnoreCase))
{
return new TerminalUnit(group);
}
}

return null;
}

private static void SetTerminalPropertyValue(IMyTerminalBlock block,ITerminalProperty property,object value)
{
switch(property.TypeName)
{
case "Boolean":
if(value is bool)
{
block.SetValueBool(property.Id,(bool)value);
return;
}
break;
case "Float":
case "Double":
case "Single":
if(value is float||
value is double||
value is int)
{
block.SetValueFloat(property.Id,Convert.ToSingle(value));
return;
}
break;
case "Color":
if(value is Color)
{
block.SetValueColor(property.Id,(Color)value);
return;
}
break;
}
throw new Exception($"Can't set {property.TypeName} property '{property.Id}' to '{value}'.");
}
}

class AdditionalProperty
{
public string Name{get;}
public string TypeName{get;}
public bool ReadOnly{get;}
public IMyTerminalBlock Block{get;}
private object value;

public object GetValue()
{
return value;
}

public void SetValue(object value)
{
if(ReadOnly)
{
throw new Exception("Can't set value to read-only property.");
}
else
{
if(Block is IMyAirVent)
{
var specifiedBlock=Block as IMyAirVent;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyAssembler)
{
var specifiedBlock=Block as IMyAssembler;
switch(Name)
{
case "cooperativemode":specifiedBlock.CooperativeMode=(bool)value;this.value=value;break;
case "repeating":specifiedBlock.Repeating=(bool)value;this.value=value;break;
}
}
else if(Block is IMyBatteryBlock)
{
var specifiedBlock=Block as IMyBatteryBlock;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyBeacon)
{
var specifiedBlock=Block as IMyBeacon;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyButtonPanel)
{
var specifiedBlock=Block as IMyButtonPanel;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyCameraBlock)
{
var specifiedBlock=Block as IMyCameraBlock;
switch(Name)
{
case "enableraycast":specifiedBlock.EnableRaycast=(bool)value;this.value=value;break;
}
}
else if(Block is IMyCargoContainer)
{
var specifiedBlock=Block as IMyCargoContainer;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyCockpit)
{
var specifiedBlock=Block as IMyCockpit;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyCollector)
{
var specifiedBlock=Block as IMyCollector;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyControlPanel)
{
var specifiedBlock=Block as IMyControlPanel;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyConveyorSorter)
{
var specifiedBlock=Block as IMyConveyorSorter;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyCryoChamber)
{
var specifiedBlock=Block as IMyCryoChamber;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyDecoy)
{
var specifiedBlock=Block as IMyDecoy;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyDoor)
{
var specifiedBlock=Block as IMyDoor;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyExtendedPistonBase)
{
var specifiedBlock=Block as IMyExtendedPistonBase;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyGasGenerator)
{
var specifiedBlock=Block as IMyGasGenerator;
switch(Name)
{
case "autorefill":specifiedBlock.AutoRefill=(bool)value;this.value=value;break;
}
}
else if(Block is IMyGasTank)
{
var specifiedBlock=Block as IMyGasTank;
switch(Name)
{
case "autorefillbottles":specifiedBlock.AutoRefillBottles=(bool)value;this.value=value;break;
}
}
else if(Block is IMyGravityGenerator)
{
var specifiedBlock=Block as IMyGravityGenerator;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyGravityGeneratorSphere)
{
var specifiedBlock=Block as IMyGravityGeneratorSphere;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyGyro)
{
var specifiedBlock=Block as IMyGyro;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyInteriorLight)
{
var specifiedBlock=Block as IMyInteriorLight;
switch(Name)
{
case "blinkintervalseconds":specifiedBlock.BlinkIntervalSeconds=Convert.ToSingle(value);break;
case "blinklength":specifiedBlock.BlinkLength=Convert.ToSingle(value);break;
case "blinkoffset":specifiedBlock.BlinkOffset=Convert.ToSingle(value);break;
case "falloff":specifiedBlock.Falloff=Convert.ToSingle(value);break;
}
}
else if(Block is IMyJumpDrive)
{
var specifiedBlock=Block as IMyJumpDrive;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyLandingGear)
{
var specifiedBlock=Block as IMyLandingGear;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyLargeTurretBase)
{
var specifiedBlock=Block as IMyLargeTurretBase;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyLaserAntenna)
{
var specifiedBlock=Block as IMyLaserAntenna;
switch(Name)
{
case "range":specifiedBlock.Range=Convert.ToSingle(value);break;
}
}
else if(Block is IMyMedicalRoom)
{
var specifiedBlock=Block as IMyMedicalRoom;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyMotorAdvancedStator)
{
var specifiedBlock=Block as IMyMotorAdvancedStator;
switch(Name)
{
case "targetvelocityrad":specifiedBlock.TargetVelocityRad=Convert.ToSingle(value);break;
case "targetvelocityrpm":specifiedBlock.TargetVelocityRPM=Convert.ToSingle(value);break;
}
}
else if(Block is IMyMotorStator)
{
var specifiedBlock=Block as IMyMotorStator;
switch(Name)
{
case "targetvelocityrad":specifiedBlock.TargetVelocityRad=Convert.ToSingle(value);break;
case "targetvelocityrpm":specifiedBlock.TargetVelocityRPM=Convert.ToSingle(value);break;
}
}
else if(Block is IMyMotorSuspension)
{
var specifiedBlock=Block as IMyMotorSuspension;
switch(Name)
{
case "brake":specifiedBlock.Brake=(bool)value;this.value=value;break;
}
}
else if(Block is IMyOreDetector)
{
var specifiedBlock=Block as IMyOreDetector;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyOxygenFarm)
{
var specifiedBlock=Block as IMyOxygenFarm;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyParachute)
{
var specifiedBlock=Block as IMyParachute;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyProgrammableBlock)
{
var specifiedBlock=Block as IMyProgrammableBlock;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyProjector)
{
var specifiedBlock=Block as IMyProjector;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyRadioAntenna)
{
var specifiedBlock=Block as IMyRadioAntenna;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyReactor)
{
var specifiedBlock=Block as IMyReactor;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyRefinery)
{
var specifiedBlock=Block as IMyRefinery;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyReflectorLight)
{
var specifiedBlock=Block as IMyReflectorLight;
switch(Name)
{
case "blinkintervalseconds":specifiedBlock.BlinkIntervalSeconds=Convert.ToSingle(value);break;
case "blinklength":specifiedBlock.BlinkLength=Convert.ToSingle(value);break;
case "blinkoffset":specifiedBlock.BlinkOffset=Convert.ToSingle(value);break;
case "falloff":specifiedBlock.Falloff=Convert.ToSingle(value);break;
}
}
else if(Block is IMyRemoteControl)
{
var specifiedBlock=Block as IMyRemoteControl;
switch(Name)
{
case "":break;
}
}
else if(Block is IMySensorBlock)
{
var specifiedBlock=Block as IMySensorBlock;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyShipConnector)
{
var specifiedBlock=Block as IMyShipConnector;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyShipDrill)
{
var specifiedBlock=Block as IMyShipDrill;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyShipGrinder)
{
var specifiedBlock=Block as IMyShipGrinder;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyShipMergeBlock)
{
var specifiedBlock=Block as IMyShipMergeBlock;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyShipWelder)
{
var specifiedBlock=Block as IMyShipWelder;
switch(Name)
{
case "":break;
}
}
else if(Block is IMySmallGatlingGun)
{
var specifiedBlock=Block as IMySmallGatlingGun;
switch(Name)
{
case "":break;
}
}
else if(Block is IMySmallMissileLauncher)
{
var specifiedBlock=Block as IMySmallMissileLauncher;
switch(Name)
{
case "":break;
}
}
else if(Block is IMySmallMissileLauncherReload)
{
var specifiedBlock=Block as IMySmallMissileLauncherReload;
switch(Name)
{
case "":break;
}
}
else if(Block is IMySolarPanel)
{
var specifiedBlock=Block as IMySolarPanel;
switch(Name)
{
case "":break;
}
}
else if(Block is IMySoundBlock)
{
var specifiedBlock=Block as IMySoundBlock;
switch(Name)
{
case "":break;
}
}
else if(Block is IMySpaceBall)
{
var specifiedBlock=Block as IMySpaceBall;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyTextPanel)
{
var specifiedBlock=Block as IMyTextPanel;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyThrust)
{
var specifiedBlock=Block as IMyThrust;
switch(Name)
{
case "thrustoverridepercentage":specifiedBlock.ThrustOverridePercentage=Convert.ToSingle(value);break;
}
}
else if(Block is IMyTimerBlock)
{
var specifiedBlock=Block as IMyTimerBlock;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyVirtualMass)
{
var specifiedBlock=Block as IMyVirtualMass;
switch(Name)
{
case "":break;
}
}
else if(Block is IMyWarhead)
{
var specifiedBlock=Block as IMyWarhead;
switch(Name)
{
case "":break;
}
}
}
}

public AdditionalProperty(IMyTerminalBlock block,string propertyName)
{
Block=block;
Name=propertyName.ToLower();
if(Block is IMyAirVent)
{
var specifiedBlock=Block as IMyAirVent;
switch(Name)
{
case "status":
{
value=specifiedBlock.Status;
TypeName="enum";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyAssembler)
{
var specifiedBlock=Block as IMyAssembler;
switch(Name)
{
case "cooperativemode":
{
value=specifiedBlock.CooperativeMode;
TypeName="bool";
ReadOnly=false;
break;
}
case "currentprogress":
{
value=specifiedBlock.CurrentProgress;
TypeName="float";
ReadOnly=true;
break;
}
case "isproducing":
{
value=specifiedBlock.IsProducing;
TypeName="bool";
ReadOnly=true;
break;
}
case "isqueueempty":
{
value=specifiedBlock.IsQueueEmpty;
TypeName="bool";
ReadOnly=true;
break;
}
case "repeating":
{
value=specifiedBlock.Repeating;
TypeName="bool";
ReadOnly=false;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyBatteryBlock)
{
var specifiedBlock=Block as IMyBatteryBlock;
switch(Name)
{
case "currentinput":
{
value=specifiedBlock.CurrentInput;
TypeName="float";
ReadOnly=true;
break;
}
case "currentoutput":
{
value=specifiedBlock.CurrentOutput;
TypeName="float";
ReadOnly=true;
break;
}
case "currentstoredpower":
{
value=specifiedBlock.CurrentStoredPower;
TypeName="float";
ReadOnly=true;
break;
}
case "ischarging":
{
value=specifiedBlock.IsCharging;
TypeName="bool";
ReadOnly=true;
break;
}
case "maxinput":
{
value=specifiedBlock.MaxInput;
TypeName="float";
ReadOnly=true;
break;
}
case "maxoutput":
{
value=specifiedBlock.MaxOutput;
TypeName="float";
ReadOnly=true;
break;
}
case "maxstoredpower":
{
value=specifiedBlock.MaxStoredPower;
TypeName="float";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyBeacon)
{
var specifiedBlock=Block as IMyBeacon;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyButtonPanel)
{
var specifiedBlock=Block as IMyButtonPanel;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyCameraBlock)
{
var specifiedBlock=Block as IMyCameraBlock;
switch(Name)
{
case "availablescanrange":
{
value=Convert.ToSingle(specifiedBlock.AvailableScanRange);
TypeName="float";
ReadOnly=true;
break;
}
case "enableraycast":
{
value=specifiedBlock.EnableRaycast;
TypeName="bool";
ReadOnly=false;
break;
}
case "isactive":
{
value=specifiedBlock.IsActive;
TypeName="bool";
ReadOnly=true;
break;
}
case "raycastconelimit":
{
value=specifiedBlock.RaycastConeLimit;
TypeName="float";
ReadOnly=true;
break;
}
case "raycastdistancelimit":
{
value=Convert.ToSingle(specifiedBlock.RaycastDistanceLimit);
TypeName="float";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyCargoContainer)
{
var specifiedBlock=Block as IMyCargoContainer;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyCockpit)
{
var specifiedBlock=Block as IMyCockpit;
switch(Name)
{
case "cancontrolship":
{
value=specifiedBlock.CanControlShip;
TypeName="bool";
ReadOnly=true;
break;
}
case "haswheels":
{
value=specifiedBlock.HasWheels;
TypeName="bool";
ReadOnly=true;
break;
}
case "isundercontrol":
{
value=specifiedBlock.IsUnderControl;
TypeName="bool";
ReadOnly=true;
break;
}
case "oxygencapacity":
{
value=specifiedBlock.OxygenCapacity;
TypeName="float";
ReadOnly=true;
break;
}
case "oxygenfilledratio":
{
value=specifiedBlock.OxygenFilledRatio;
TypeName="float";
ReadOnly=true;
break;
}
case "shipspeed":
{
value=(float)specifiedBlock.GetShipSpeed();
TypeName="float";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyCollector)
{
var specifiedBlock=Block as IMyCollector;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyControlPanel)
{
var specifiedBlock=Block as IMyControlPanel;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyConveyorSorter)
{
var specifiedBlock=Block as IMyConveyorSorter;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyDecoy)
{
var specifiedBlock=Block as IMyDecoy;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyDoor)
{
var specifiedBlock=Block as IMyDoor;
switch(Name)
{
case "status":
{
value=specifiedBlock.Status;
TypeName="enum";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyExtendedPistonBase)
{
var specifiedBlock=Block as IMyExtendedPistonBase;
switch(Name)
{
case "status":
{
value=specifiedBlock.Status;
TypeName="enum";
ReadOnly=true;
break;
}
case "currentposition":
{
value=specifiedBlock.CurrentPosition;
TypeName="float";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyGasGenerator)
{
var specifiedBlock=Block as IMyGasGenerator;
switch(Name)
{
case "autorefill":
{
value=specifiedBlock.AutoRefill;
TypeName="bool";
ReadOnly=false;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyGasTank)
{
var specifiedBlock=Block as IMyGasTank;
switch(Name)
{
case "autorefillbottles":
{
value=specifiedBlock.AutoRefillBottles;
TypeName="bool";
ReadOnly=false;
break;
}
case "capacity":
{
value=specifiedBlock.Capacity;
TypeName="bool";
ReadOnly=true;
break;
}
case "filledratio":
{
value=Convert.ToSingle(specifiedBlock.FilledRatio);
TypeName="float";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyGravityGenerator)
{
var specifiedBlock=Block as IMyGravityGenerator;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyGravityGeneratorSphere)
{
var specifiedBlock=Block as IMyGravityGeneratorSphere;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyGyro)
{
var specifiedBlock=Block as IMyGyro;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyInteriorLight)
{
var specifiedBlock=Block as IMyInteriorLight;
switch(Name)
{
case "blinkintervalseconds":
{
value=specifiedBlock.BlinkIntervalSeconds;
TypeName="float";
ReadOnly=false;
break;
}
case "blinklength":
{
value=specifiedBlock.BlinkLength;
TypeName="float";
ReadOnly=false;
break;
}
case "blinkoffset":
{
value=specifiedBlock.BlinkOffset;
TypeName="float";
ReadOnly=false;
break;
}
case "falloff":
{
value=specifiedBlock.Falloff;
TypeName="float";
ReadOnly=false;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyJumpDrive)
{
var specifiedBlock=Block as IMyJumpDrive;
switch(Name)
{
case "currentstoredpower":
{
value=specifiedBlock.CurrentStoredPower;
TypeName="float";
ReadOnly=true;
break;
}
case "maxstoredpower":
{
value=specifiedBlock.MaxStoredPower;
TypeName="float";
ReadOnly=true;
break;
}
case "status":
{
value=specifiedBlock.Status;
TypeName="enum";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyLandingGear)
{
var specifiedBlock=Block as IMyLandingGear;
switch(Name)
{
case "islocked":
{
value=specifiedBlock.IsLocked;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyLargeTurretBase)
{
var specifiedBlock=Block as IMyLargeTurretBase;
switch(Name)
{
case "aienabled":
{
value=specifiedBlock.AIEnabled;
TypeName="bool";
ReadOnly=true;
break;
}
case "hastarget":
{
value=specifiedBlock.HasTarget;
TypeName="bool";
ReadOnly=true;
break;
}
case "isaimed":
{
value=specifiedBlock.IsAimed;
TypeName="bool";
ReadOnly=true;
break;
}
case "isshooting":
{
value=specifiedBlock.IsShooting;
TypeName="bool";
ReadOnly=true;
break;
}
case "isundercontrol":
{
value=specifiedBlock.IsUnderControl;
TypeName="bool";
ReadOnly=true;
break;
}
case "targettype":
{
value=specifiedBlock.GetTargetedEntity().Type;
TypeName="enum";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyLaserAntenna)
{
var specifiedBlock=Block as IMyLaserAntenna;
switch(Name)
{
case "range":
{
value=specifiedBlock.Range;
TypeName="float";
ReadOnly=false;
break;
}
case "status":
{
value=specifiedBlock.Status;
TypeName="enum";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyMedicalRoom)
{
var specifiedBlock=Block as IMyMedicalRoom;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyMotorAdvancedStator)
{
var specifiedBlock=Block as IMyMotorAdvancedStator;
switch(Name)
{
case "angle":
{
value=(float)(180 / Math.PI * specifiedBlock.Angle);
TypeName="float";
ReadOnly=true;
break;
}
case "pendingattachment":
{
value=specifiedBlock.PendingAttachment;
TypeName="bool";
ReadOnly=true;
break;
}
case "targetvelocityrad":
{
value=specifiedBlock.TargetVelocityRad;
TypeName="float";
ReadOnly=false;
break;
}
case "targetvelocityrpm":
{
value=specifiedBlock.TargetVelocityRPM;
TypeName="float";
ReadOnly=false;
break;
}
case "isattached":
{
value=specifiedBlock.IsAttached;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyMotorStator)
{
var specifiedBlock=Block as IMyMotorStator;
switch(Name)
{
case "angle":
{
value=(float)(180 / Math.PI * specifiedBlock.Angle);
TypeName="float";
ReadOnly=true;
break;
}
case "pendingattachment":
{
value=specifiedBlock.PendingAttachment;
TypeName="bool";
ReadOnly=true;
break;
}
case "targetvelocityrad":
{
value=specifiedBlock.TargetVelocityRad;
TypeName="float";
ReadOnly=false;
break;
}
case "targetvelocityrpm":
{
value=specifiedBlock.TargetVelocityRPM;
TypeName="float";
ReadOnly=false;
break;
}
case "isattached":
{
value=specifiedBlock.IsAttached;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyMotorSuspension)
{
var specifiedBlock=Block as IMyMotorSuspension;
switch(Name)
{
case "brake":
{
value=specifiedBlock.Brake;
TypeName="bool";
ReadOnly=false;
break;
}
case "isattached":
{
value=specifiedBlock.IsAttached;
TypeName="bool";
ReadOnly=true;
break;
}
case "pendingattachment":
{
value=specifiedBlock.PendingAttachment;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyOreDetector)
{
var specifiedBlock=Block as IMyOreDetector;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyOxygenFarm)
{
var specifiedBlock=Block as IMyOxygenFarm;
switch(Name)
{
case "canproduce":
{
value=specifiedBlock.CanProduce;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyParachute)
{
var specifiedBlock=Block as IMyParachute;
switch(Name)
{
case "atmosphere":
{
value=specifiedBlock.Atmosphere;
TypeName="float";
ReadOnly=true;
break;
}
case "status":
{
value=specifiedBlock.Status;
TypeName="enum";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyProgrammableBlock)
{
var specifiedBlock=Block as IMyProgrammableBlock;
switch(Name)
{
case "isrunning":
{
value=specifiedBlock.IsRunning;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyProjector)
{
var specifiedBlock=Block as IMyProjector;
switch(Name)
{
case "buildableblockscount":
{
value=(float)specifiedBlock.BuildableBlocksCount;
TypeName="float";
ReadOnly=true;
break;
}
case "remainingarmorblocks":
{
value=(float)specifiedBlock.RemainingArmorBlocks;
TypeName="float";
ReadOnly=true;
break;
}
case "remainingblocks":
{
value=(float)specifiedBlock.RemainingBlocks;
TypeName="float";
ReadOnly=true;
break;
}
case "isprojecting":
{
value=specifiedBlock.IsProjecting;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyRadioAntenna)
{
var specifiedBlock=Block as IMyRadioAntenna;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyReactor)
{
var specifiedBlock=Block as IMyReactor;
switch(Name)
{
case "currentoutput":
{
value=specifiedBlock.CurrentOutput;
TypeName="float";
ReadOnly=true;
break;
}
case "maxoutput":
{
value=specifiedBlock.MaxOutput;
TypeName="float";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyRefinery)
{
var specifiedBlock=Block as IMyRefinery;
switch(Name)
{
case "isproducing":
{
value=specifiedBlock.IsProducing;
TypeName="bool";
ReadOnly=true;
break;
}
case "isqueueempty":
{
value=specifiedBlock.IsQueueEmpty;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyReflectorLight)
{
var specifiedBlock=Block as IMyReflectorLight;
switch(Name)
{
case "blinkintervalseconds":
{
value=specifiedBlock.BlinkIntervalSeconds;
TypeName="float";
ReadOnly=false;
break;
}
case "blinklength":
{
value=specifiedBlock.BlinkLength;
TypeName="float";
ReadOnly=false;
break;
}
case "blinkoffset":
{
value=specifiedBlock.BlinkOffset;
TypeName="float";
ReadOnly=false;
break;
}
case "falloff":
{
value=specifiedBlock.Falloff;
TypeName="float";
ReadOnly=false;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyRemoteControl)
{
var specifiedBlock=Block as IMyRemoteControl;
switch(Name)
{
case "cancontrolship":
{
value=specifiedBlock.CanControlShip;
TypeName="bool";
ReadOnly=true;
break;
}
case "haswheels":
{
value=specifiedBlock.HasWheels;
TypeName="bool";
ReadOnly=true;
break;
}
case "isundercontrol":
{
value=specifiedBlock.IsUnderControl;
TypeName="bool";
ReadOnly=true;
break;
}
case "shipspeed":
{
value=(float)specifiedBlock.GetShipSpeed();
TypeName="float";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMySensorBlock)
{
var specifiedBlock=Block as IMySensorBlock;
switch(Name)
{
case "isactive":
{
value=specifiedBlock.IsActive;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyShipConnector)
{
var specifiedBlock=Block as IMyShipConnector;
switch(Name)
{
case "status":
{
value=specifiedBlock.Status;
TypeName="enum";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyShipDrill)
{
var specifiedBlock=Block as IMyShipDrill;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyShipGrinder)
{
var specifiedBlock=Block as IMyShipGrinder;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyShipMergeBlock)
{
var specifiedBlock=Block as IMyShipMergeBlock;
switch(Name)
{
case "isconnected":
{
value=specifiedBlock.IsConnected;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyShipWelder)
{
var specifiedBlock=Block as IMyShipWelder;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMySmallGatlingGun)
{
var specifiedBlock=Block as IMySmallGatlingGun;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMySmallMissileLauncher)
{
var specifiedBlock=Block as IMySmallMissileLauncher;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMySmallMissileLauncherReload)
{
var specifiedBlock=Block as IMySmallMissileLauncherReload;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMySolarPanel)
{
var specifiedBlock=Block as IMySolarPanel;
switch(Name)
{
case "currentoutput":
{
value=specifiedBlock.CurrentOutput;
TypeName="float";
ReadOnly=true;
break;
}
case "maxoutput":
{
value=specifiedBlock.MaxOutput;
TypeName="float";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMySoundBlock)
{
var specifiedBlock=Block as IMySoundBlock;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMySpaceBall)
{
var specifiedBlock=Block as IMySpaceBall;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyTextPanel)
{
var specifiedBlock=Block as IMyTextPanel;
switch(Name)
{
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyThrust)
{
var specifiedBlock=Block as IMyThrust;

switch(Name)
{
case "currentthrust":
{
value=specifiedBlock.CurrentThrust;
TypeName="float";
ReadOnly=true;
break;
}
case "maxeffectivethrust":
{
value=specifiedBlock.MaxEffectiveThrust;
TypeName="float";
ReadOnly=true;
break;
}
case "maxthrust":
{
value=specifiedBlock.MaxThrust;
TypeName="float";
ReadOnly=true;
break;
}
case "thrustoverridepercentage":
{
value=specifiedBlock.ThrustOverridePercentage;
TypeName="float";
ReadOnly=false;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyTimerBlock)
{
var specifiedBlock=Block as IMyTimerBlock;
switch(Name)
{
case "iscountingdown":
{
value=specifiedBlock.IsCountingDown;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyVirtualMass)
{
var specifiedBlock=Block as IMyVirtualMass;
switch(Name)
{
case "virtualmass":
{
value=specifiedBlock.VirtualMass;
TypeName="float";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else if(Block is IMyWarhead)
{
var specifiedBlock=Block as IMyWarhead;
switch(Name)
{
case "iscountingdown":
{
value=specifiedBlock.IsCountingDown;
TypeName="bool";
ReadOnly=true;
break;
}
default:throw new Exception($"Block '{block.CustomName}' has no additional property '{propertyName}'.");
}
}
else
{
throw new Exception($"Can't get additional property from block '{block.CustomName}'.");
}
}
}

class DeferredActionBlock
{
readonly float seconds;
public float ElapsedTime{get;private set;}
public int WhenBlockIndex{get;}
public bool Ready{get;private set;}
public TerminalUnit TerminalUnit{get;}
public string[][]Actions{get;}
public int FirstOriginalLineActionIndex{get;}

public DeferredActionBlock(float seconds,int whenBlockIndex,TerminalUnit terminalUnit,string[][]actions,int firstOriginalLineActionIndex)
{
ElapsedTime=0;
this.seconds=seconds;
WhenBlockIndex=whenBlockIndex;
Ready=false;
TerminalUnit=terminalUnit;
Actions=actions;
FirstOriginalLineActionIndex=firstOriginalLineActionIndex;
}

public void AddElapsedTime(float elapsedSeconds)
{
ElapsedTime+=elapsedSeconds;
if(ElapsedTime>=seconds)
{
Ready=true;
}
}
}

class CompletedActionBlockInfo
{
public TerminalUnit TerminalUnit{get;}
public int WhenBlockIndex{get;}

public CompletedActionBlockInfo(TerminalUnit terminalUnit,int whenBlockIndex)
{
TerminalUnit=terminalUnit;
WhenBlockIndex=whenBlockIndex;
}
}
