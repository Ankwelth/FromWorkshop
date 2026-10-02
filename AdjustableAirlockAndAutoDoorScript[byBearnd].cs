/*
###########################################################
                                   --- Adjustable Airlock & Auto Door Script ---
                                   - by Bearnd | Version 1.1.106 | 03/18/2024 -

This is the Adjustable Airlock & Auto Door Script created by Bearnd. The script allows you to create the most thinkable
configurations for airlocks and auto doors by supporting door blocks, light blocks, screen block, sound blocks, timer
blocks and airvent blocks.


Please read the full guide for more detailed information about all options and features of this script.
LINK TO THE GUIDE: https://steamcommunity.com/sharedfiles/filedetails/?id=2852149535

Please DO NOT publish this script or its derivations without my permission!
Feel free to use it in your awesome builds and blueprints!

If you have any questions, remarks or if you notice a bug, please leave a comment on the official workshop page of this script!

Please give a thumbs up, if you like this script!!! This motivates me to continuously improve this script and keep it alive and up to date!


Credits:
Special thanks to Nidaz (for your support, review and for testing and translation!)

###########################################################
*/

/////-- CUSTOMIZABLE SCRIPT TEXT OUTPUT --/////
/*
Here you can change the text output from the script for manual changes or translation purposes.
Please note: the default texts are optimized to the screens to fit properly as good as possible.
Making custom changes might lead to texts out of screen range (if text is too long).
Changes are therefore out of my support!
*/

///-- Airlock Status --///
const string TXT_AIRLOCK_CLEAR = "Airlock clear!";
const string TXT_AIRLOCK_OPEN = "Airlock open!";
const string TXT_AIRLOCK_OPENING = "Airlock is opening";
const string TXT_AIRLOCK_CLOSING = "Airlock is closing";
const string TXT_INT_DOOR_OPEN = "Internal door(s) open";
const string TXT_INT_DOOR_LOCKED = "Internal doors locked!";
const string TXT_INT_DOOR_OPENING = "Internal door is opening";
const string TXT_INT_DOOR_CLOSING = "Internal door is closing";
const string TXT_EXT_DOOR_OPEN = "External door(s) open";
const string TXT_EXT_DOOR_LOCKED = "External doors locked!";
const string TXT_EXT_DOOR_OPENING = "External door is opening";
const string TXT_EXT_DOOR_CLOSING = "External door is closing";
const string TXT_AIRLOCK_EQUAL = "Airlock pressure equalized";
const string TXT_AIRLOCK_PRESSURIZED = "Airlock pressurized!";
const string TXT_AIRLOCK_PRESSURIZING = "Airlock is pressurizing";
const string TXT_AIRLOCK_DEPRESSURIZED = "Airlock depressurized!";
const string TXT_AIRLOCK_DEPRESSURIZING = "Airlock is depressurizing";
const string TXT_AIRLOCK_OXYGENFULL = "Oxygen tanks full !";
const string TXT_AIRLOCK_OXYGENEMPTY = "Oxygen tanks empty !";

///-- Errors --///
const string TXT_ERR_ONE_DOOR = "# ERROR: Only one door assigned to group";
const string TXT_SOL_ONE_DOOR = "-> SOLUTION: Assign another door to the same group.";
const string TXT_ERR_NO_DOOR_AL = "(i) NOTE: There is no door assigned to an airlock.";
const string TXT_SOL_NO_DOOR_AL = "-> SOLUTION: Add a door with proper script and group tag.";
const string TXT_ERR_NO_DOOR_GR = "(i) NOTE: There are no door blocks found on this grid!";
const string TXT_SOL_NO_DOOR_GR = "-> SOLUTION: Add one or more doors and assign propper script tag to the doors name.";
const string TXT_ERR_EXT_DOOR_MISSING = "# ERROR: External door missing for group";
const string TXT_SOL_EXT_DOOR_MISSING = "-> SOLUTION: Add a door with proper tag and group for an external door.";
const string TXT_ERR_INT_DOOR_MISSING = "# ERROR: Internal door missing for group";
const string TXT_SOL_INT_DOOR_MISSING = "-> SOLUTION: Add a door with proper tag and group for an internal door.";

///-- PB-Screen --///
const string TXT_PBSCREEN_AUTODOOR = "Number of autodoors detected";
const string TXT_PBSCREEN_AIRLOCK = "Number of airlocks detected";
const string TXT_SCREEN_BLOCK_UPDATE = "Next block update in";
const string TXT_SCREEN_BLOCK_UPDATE_UNIT = "seconds";

///-- Details-Screen --///
const string TXT_DETAILS_SCRIPTTAG = "Script Tag";
const string TXT_DETAILS_STEALTH = "Stealth Mode";
const string TXT_DETAILS_SCRIPT_INFO = "Script Performance Info";
const string TXT_DETAILS_AVGRT = "Avg running time";
const string TXT_DETAILS_AVGF = "Avg Frequence";
const string TXT_DETAILS_RPS = "Runs per Second";
const string TXT_DETAILS_CPUT = "Block CPU Temp";
const string TXT_DETAILS_INSTR = "Avg Instructions";

///-- UNITS --///
const string TXT_UNIT_MS = "ms";
const string TXT_UNIT_RPS = "r/s";
const string TXT_UNIT_CEL = "°C";







/*
###########################################################

                       !!! DO NOT CHANGE ANYTHING BELOW THIS TEXT !!!

###########################################################
*/







const string VERSION = "1.1.106";
string SCRIPT_TAG;
double SCRIPT_SPEED;
bool SHOW_PB_SCREEN;
bool SHOW_SCREEN_INIT_TRIGGER;
bool ENABLE_AUTODOOR;
bool ENABLE_AUTOOPEN;
bool HANDLE_ALL_DOORS;
double DOOR_CLOSING_TIME;
bool EXCLUDE_HANGAR_DOORS;
double HANGAR_CLOSING_TIME;
bool IGNORE_PRESSURE;
bool IGNORE_AUTOLOCK;
bool IGNORE_AUTOCLOSE;
bool USE_COLORS_FROM_LIGHT_BLOCK;
bool USE_STANDARD_LIGHT_COLORS;
Color LIGHT_COLOR_POSITIVE;
Color LIGHT_COLOR_NEUTRAL;
Color LIGHT_COLOR_NEGATIVE;
bool DO_BLINK_POSITIVE;
bool DO_BLINK_NEUTRAL;
bool DO_BLINK_NEGATIVE;
double BLINK_INTERVAL;
double BLINK_LENGTH;
double BLINK_OFFSET;
bool USE_ON_OFF;
bool SHOW_DYNAMIC_TEXT;
bool SHOW_STATUS_TEXT;
bool SHOW_O2_BAR;
bool SHOW_O2_PCT;
bool USE_DEFAULT_FONTSIZE;
bool USE_DEFAULT_ALIGNMENT;
bool USE_COLORS_FROM_SCREEN_BLOCK;
bool USE_STANDARD_SCREEN_COLORS;
Color FONT_COLOR_POSITIVE;
Color FONT_COLOR_NEUTRAL;
Color FONT_COLOR_NEGATIVE;
Color BG_COLOR_POSITIVE;
Color BG_COLOR_NEUTRAL;
Color BG_COLOR_NEGATIVE;
bool INVERT_COLOR_SETTINGS;
double PLAYBACK_TIME;
string TOTALSCRIPTTAG;
string TOTALSCRIPTTAG_INT;
string TOTALSCRIPTTAG_EXT;
string TOTALSCRIPTTAG_OPT;
string OldGlobals;
string NewGlobals;
string RunnerString;
int GRP_DOORS_OPEN;
int GRP_INT_DOORS_OPEN;
int GRP_EXT_DOORS_OPEN;
int GRP_DOORS_OPENING;
int GRP_DOORS_CLOSING;
int GRP_DOORS_OPENED;
int GRP_DOORS_CLOSED;
int GRP_INT_DOORS_OPENING;
int GRP_INT_DOORS_CLOSING;
int GRP_INT_DOORS_OPENED;
int GRP_INT_DOORS_CLOSED;
int GRP_EXT_DOORS_OPENING;
int GRP_EXT_DOORS_CLOSING;
int GRP_EXT_DOORS_OPENED;
int GRP_EXT_DOORS_CLOSED;
int intRunner = 0;
int RatioCounter = 0;
int InstrCounter = 0;
List<IMyDoor> lstDoors = new List<IMyDoor>();
List<IMyLightingBlock> lstLights = new List<IMyLightingBlock>();
List<IMyTextPanel> lstScreens = new List<IMyTextPanel>();
List<IMySoundBlock> lstSounds = new List<IMySoundBlock>();
List<IMyTimerBlock> lstTimer = new List<IMyTimerBlock>();
List<IMyAirVent> lstAirVents = new List<IMyAirVent>();
List<IMyGasTank> lstGasTanks = new List<IMyGasTank>();
List<IMyDoor> lstTAGDoors = new List<IMyDoor>();
List<IMyLightingBlock> lstTAGLights = new List<IMyLightingBlock>();
List<IMyTextPanel> lstTAGScreens = new List<IMyTextPanel>();
List<SOUNDLIST> lstTAGSounds = new List<SOUNDLIST>();
List<TIMERLIST> lstTAGTimers = new List<TIMERLIST>();
List<IMyAirVent> lstTAGAirVents = new List<IMyAirVent>();
List<IMyDoor> lstGRPDoors = new List<IMyDoor>();
List<IMyDoor> lstGRPIntDoors = new List<IMyDoor>();
List<IMyDoor> lstGRPExtDoors = new List<IMyDoor>();
List<IMyLightingBlock> lstGRPLights = new List<IMyLightingBlock>();
List<IMyLightingBlock> lstGRPIntLights = new List<IMyLightingBlock>();
List<IMyLightingBlock> lstGRPExtLights = new List<IMyLightingBlock>();
List<IMyLightingBlock> lstGRPOptLights = new List<IMyLightingBlock>();
List<IMyTextPanel> lstGRPScreens = new List<IMyTextPanel>();
List<IMyTextPanel> lstGRPIntScreens = new List<IMyTextPanel>();
List<IMyTextPanel> lstGRPExtScreens = new List<IMyTextPanel>();
List<IMyTextPanel> lstGRPOptScreens = new List<IMyTextPanel>();
List<SOUNDLIST> lstGRPSounds = new List<SOUNDLIST>();
List<TIMERLIST> lstGRPTimers = new List<TIMERLIST>();
List<IMyAirVent> lstGRPAirVents = new List<IMyAirVent>();
List<GROUPLIST> lstGroup = new List<GROUPLIST>();
List<DOORLIST> lstDoorOcc = new List<DOORLIST>();
List<DOORLIST> lstDoorCloser = new List<DOORLIST>();
List<PERFORMANCELIST> lstPerformance = new List<PERFORMANCELIST>();
DateTime RunnerTimer = System.DateTime.UtcNow;
DateTime PerformanceTimer = DateTime.MinValue;
DateTime RunGatherer = System.DateTime.UtcNow;
DateTime RunTimerStart = System.DateTime.UtcNow;
DateTime RunTimerEnd = System.DateTime.UtcNow;
DateTime NewRunTimer;
DateTime SpriteTimer = System.DateTime.UtcNow;
bool initialRun = true;
bool STEALTH_MODE;
IEnumerator<bool> _stateMachine;
StringBuilder RunnerSB = new StringBuilder();
StringBuilder GathererInfo = new StringBuilder();
StringBuilder PerformanceInfo = new StringBuilder();
StringBuilder ErrorInfo = new StringBuilder();
StringBuilder ScriptInfo = new StringBuilder();
public Program()
{
_stateMachine = RunCodeInTime();
Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
public void Main(string argument, UpdateType updateType)
{
if(string.Equals(argument, "STEALTH_ON", StringComparison.OrdinalIgnoreCase)) {STEALTH_MODE = true;}
if(string.Equals(argument, "STEALTH_OFF", StringComparison.OrdinalIgnoreCase)) {STEALTH_MODE = false;}
if ((updateType & UpdateType.Update10) == UpdateType.Update10) {Runtime.UpdateFrequency |= UpdateFrequency.Once;}
if ((updateType & UpdateType.Once) == UpdateType.Once)
{
RunStateMachine();
}
}
public void RunStateMachine()
{
if (_stateMachine != null)
{
bool hasMoreSteps = _stateMachine.MoveNext();
if (hasMoreSteps)
{
Runtime.UpdateFrequency |= UpdateFrequency.Once;
}
else
{
_stateMachine.Dispose();
_stateMachine = RunCodeInTime();
}
}
}
public IEnumerator<bool> RunCodeInTime()
{
while (true)
{
NewRunTimer = System.DateTime.UtcNow;
ErrorInfo.Clear();
_RuntimeCollector();
InstrCounter = 0;
_SetRunnerString();
if(_InitGlobalSettings()) {yield break;}
if(_BlockGatherer()) {yield break;}
_AUTODOORCloser();
if(lstGroup.Count > 0)
{
double MainLoopTime = 0;
foreach(var GROUP in lstGroup)
{
MainLoopTime += _MainLoop(GROUP);
InstrCounter += Runtime.CurrentInstructionCount;
if(Runtime.CurrentInstructionCount>SCRIPT_SPEED*1000 || MainLoopTime>SCRIPT_SPEED)
{
MainLoopTime = 0;
yield return true;
}
}
}
else
{
if(lstDoors.Count > 0)
{
ErrorInfo.AppendLine(TXT_ERR_NO_DOOR_AL);
ErrorInfo.AppendLine(TXT_SOL_NO_DOOR_AL);
}
else
{
ErrorInfo.AppendLine(TXT_ERR_NO_DOOR_GR);
ErrorInfo.AppendLine(TXT_SOL_NO_DOOR_GR);
}
InstrCounter += Runtime.CurrentInstructionCount;
}
int RunZS = Runtime.CurrentInstructionCount;
_PBScreenManager();
_InfoReporter();
InstrCounter += (Runtime.CurrentInstructionCount-RunZS);
RunTimerStart = NewRunTimer;
RunTimerEnd = System.DateTime.UtcNow;
yield return true;
}
}
public void _RuntimeCollector()
{
double sumRunningTime = 0;
double sumRunningFrequence = 0;
double sumCurrentInstructions = 0;
double AvgRunningTime = 0;
double AvgFrequence = 0;
double RunsPerSec = 0;
double InstructionCount = 0;
if(!initialRun)
{
double RunningTime = (RunTimerEnd-RunTimerStart).TotalMilliseconds;
double RunningFrequence = (NewRunTimer-RunTimerStart).TotalMilliseconds;
double Instructions = Convert.ToDouble(InstrCounter);
lstPerformance.Add(new PERFORMANCELIST {RunningTime = RunningTime,RunningFrequence = RunningFrequence, SumInstructions = Instructions});
if(lstPerformance.Count > 100) {lstPerformance.Remove(lstPerformance[0]);}
}
if(lstPerformance.Count > 0)
{
DateTime PerformanceTimerNow = System.DateTime.UtcNow;
if (PerformanceTimer.Equals(DateTime.MinValue) || (PerformanceTimerNow-PerformanceTimer).TotalSeconds > 0.15)
{
PerformanceTimer = System.DateTime.UtcNow;
foreach(var ENTRY in lstPerformance)
{
sumRunningTime+=ENTRY.RunningTime;
sumRunningFrequence+=ENTRY.RunningFrequence;
sumCurrentInstructions+=ENTRY.SumInstructions;
}
AvgRunningTime = sumRunningTime/lstPerformance.Count;
AvgFrequence = sumRunningFrequence/lstPerformance.Count;
RunsPerSec = (1/(sumRunningFrequence/lstPerformance.Count)*1000);
InstructionCount = sumCurrentInstructions/lstPerformance.Count;
PerformanceInfo.Clear();
PerformanceInfo.AppendLine(TXT_DETAILS_SCRIPT_INFO);
PerformanceInfo.AppendLine(" - "+TXT_DETAILS_AVGRT+": "+string.Format("{0:N4} "+TXT_UNIT_MS, AvgRunningTime));
PerformanceInfo.AppendLine(" - "+TXT_DETAILS_AVGF+": "+string.Format("{0:N4} "+TXT_UNIT_MS, AvgFrequence));
PerformanceInfo.AppendLine(" - "+TXT_DETAILS_RPS+": "+string.Format("{0:N2} "+TXT_UNIT_RPS, RunsPerSec));
PerformanceInfo.AppendLine(" - "+TXT_DETAILS_INSTR+": "+string.Format("{0:N0} "+"", (InstructionCount)) );
PerformanceInfo.AppendLine(" - "+TXT_DETAILS_CPUT+": "+string.Format("{0:N1} "+TXT_UNIT_CEL, (((InstructionCount/Math.Max(1,lstGroup.Count)*2)/470.1)+25)   ) );
}
}
}
public bool _InitGlobalSettings()
{
bool bResult = false;
if(OldGlobals != Me.CustomData)
{
bResult = true;
initialRun = true;
USE_STANDARD_LIGHT_COLORS=true;
USE_STANDARD_SCREEN_COLORS=true;
string EndString = "//////< GLOBAL SCRIPT SETTINGS >//////\n";
string GL_101 = "Script tag = ";
string GL_102 = "Script speed reg. = ";
string GL_103 = "Enable PB screen = ";
string GL_201 = "Enable autodoor = ";
string GL_202 = "Handle all doors = ";
string GL_203 = "Door closing time (s) = ";
string GL_204 = "Exclude hangar doors = ";
string GL_205 = "Hangar closing time (s) = ";
string GL_301 = "Enable auto open door = ";
string GL_302 = "Ignore pressure = ";
string GL_303 = "Ignore autolock = ";
string GL_304 = "Ignore autoclose = ";
string GL_401 = "Use colors from light block settings = ";
string GL_402 = "Light color positive = {";
string GL_403 = "Light color neutral = {";
string GL_404 = "Light color negative = {";
string GL_405 = "Enable blink for positive = ";
string GL_406 = "Enable blink for neutral = ";
string GL_407 = "Enable blink for negative = ";
string GL_408 = "Blink interval (s) = ";
string GL_409 = "Blink length (%) = ";
string GL_410 = "Blink offset (%) = ";
string GL_501 = "Use ON/OFF-mode = ";
string GL_502 = "Show dynamic text content = ";
string GL_503 = "Show status on screens = ";
string GL_504 = "Show O2-bar on screens = ";
string GL_505 = "Show O2-percentage on screens = ";
string GL_506 = "Use default font size = ";
string GL_507 = "Use default Alignment = ";
string GL_508 = "Use colors from screen block settings = ";
string GL_509 = "Font color positive = {";
string GL_510 = "Font color neutral = {";
string GL_511 = "Font color negative = {";
string GL_512 = "Background color positive = {";
string GL_513 = "Background color neutral = {";
string GL_514 = "Background color negative = {";
string GL_515 = "Invert color settings = ";
string GL_601 = "Max play time (s) = ";
string EndMarker = " ------- \n";
NewGlobals = Me.CustomData;
EndString+="\n////-- Script settings --////\n";
bool BOOL_GL_101 = NewGlobals.Contains(GL_101);
if(!BOOL_GL_101)
{
SCRIPT_TAG="AIRLOCK";
EndString+=GL_101+SCRIPT_TAG+"\n";
}
else
{
int intStringFirst = NewGlobals.IndexOf(GL_101, StringComparison.OrdinalIgnoreCase)+GL_101.Length;
int intStringLast = NewGlobals.IndexOf("\n", intStringFirst);
string FoundValue = NewGlobals.Substring(intStringFirst, intStringLast-intStringFirst);
string FullValue = GL_101+FoundValue+"\n";
SCRIPT_TAG = FoundValue.Trim().ToUpper();
for (int i = 0; i < SCRIPT_TAG.Length; i++)
{
if (!(char.IsLetter(SCRIPT_TAG[i])) && (!(char.IsNumber(SCRIPT_TAG[i]))))
{
SCRIPT_TAG = SCRIPT_TAG.Remove(i, 1);
i = i-1;
}
}
if(String.IsNullOrWhiteSpace(SCRIPT_TAG) || SCRIPT_TAG.Length < 1) {SCRIPT_TAG="AIRLOCK";}
string ValueLine = GL_101+SCRIPT_TAG.ToUpper()+"\n";
NewGlobals = NewGlobals.Replace(FullValue,ValueLine);
EndString+=ValueLine;
}
EndString+=InitGlobalDouble(Me, GL_102, "\n", ref SCRIPT_SPEED, 10f, 0, 100);
EndString+=InitGlobalBool(Me, GL_103, ref SHOW_PB_SCREEN, true);
EndString+="\n////-- Autodoor settings --////\n";
EndString+=InitGlobalBool(Me, GL_201, ref ENABLE_AUTODOOR, true);
EndString+=InitGlobalBool(Me, GL_202, ref HANDLE_ALL_DOORS, true);
EndString+=InitGlobalDouble(Me, GL_203, "\n", ref DOOR_CLOSING_TIME, 2.5f, 0, 600);
EndString+=InitGlobalBool(Me, GL_204, ref EXCLUDE_HANGAR_DOORS, false);
EndString+=InitGlobalDouble(Me, GL_205, "\n", ref HANGAR_CLOSING_TIME, 10, 0, 600);
EndString+="\n////-- Door settings --////\n";
EndString+=InitGlobalBool(Me, GL_301, ref ENABLE_AUTOOPEN, false);
EndString+=InitGlobalBool(Me, GL_302, ref IGNORE_PRESSURE, false);
EndString+=InitGlobalBool(Me, GL_303, ref IGNORE_AUTOLOCK, false);
EndString+=InitGlobalBool(Me, GL_304, ref IGNORE_AUTOCLOSE, false);
EndString+="\n////-- Light settings --////\n";
EndString+=InitGlobalBool(Me, GL_401, ref USE_COLORS_FROM_LIGHT_BLOCK, false);
EndString+=InitGlobalColor(Me, GL_402, "}\n", ref LIGHT_COLOR_POSITIVE, new Color(100, 255, 70), ref USE_STANDARD_LIGHT_COLORS);
EndString+=InitGlobalColor(Me, GL_403, "}\n", ref LIGHT_COLOR_NEUTRAL, new Color(255, 200, 0), ref USE_STANDARD_LIGHT_COLORS);
EndString+=InitGlobalColor(Me, GL_404, "}\n", ref LIGHT_COLOR_NEGATIVE, new Color(255, 5, 5), ref USE_STANDARD_LIGHT_COLORS);
EndString+=InitGlobalBool(Me, GL_405, ref DO_BLINK_POSITIVE, false);
EndString+=InitGlobalBool(Me, GL_406, ref DO_BLINK_NEUTRAL, true);
EndString+=InitGlobalBool(Me, GL_407, ref DO_BLINK_NEGATIVE, false);
EndString+=InitGlobalDouble(Me, GL_408, "\n", ref BLINK_INTERVAL, 0.5f, 0, 30);
EndString+=InitGlobalDouble(Me, GL_409, "\n", ref BLINK_LENGTH, 50, 0, 100);
EndString+=InitGlobalDouble(Me, GL_410, "\n", ref BLINK_OFFSET, 0, 0, 100);
EndString+="\n////-- Screen settings --////\n";
EndString+=InitGlobalBool(Me, GL_501, ref USE_ON_OFF, false);
EndString+=InitGlobalBool(Me, GL_502, ref SHOW_DYNAMIC_TEXT, true);
EndString+=InitGlobalBool(Me, GL_503, ref SHOW_STATUS_TEXT, true);
EndString+=InitGlobalBool(Me, GL_504, ref SHOW_O2_BAR, true);
EndString+=InitGlobalBool(Me, GL_505, ref SHOW_O2_PCT, true);
EndString+=InitGlobalBool(Me, GL_506, ref USE_DEFAULT_FONTSIZE, true);
EndString+=InitGlobalBool(Me, GL_507, ref USE_DEFAULT_ALIGNMENT, true);
EndString+=InitGlobalBool(Me, GL_508, ref USE_COLORS_FROM_SCREEN_BLOCK, false);
EndString+=InitGlobalColor(Me, GL_509, "}\n", ref FONT_COLOR_POSITIVE, new Color(100, 255, 70), ref USE_STANDARD_SCREEN_COLORS);
EndString+=InitGlobalColor(Me, GL_510, "}\n", ref FONT_COLOR_NEUTRAL, new Color(255, 200, 0), ref USE_STANDARD_SCREEN_COLORS);
EndString+=InitGlobalColor(Me, GL_511, "}\n", ref FONT_COLOR_NEGATIVE, new Color(255, 5, 5), ref USE_STANDARD_SCREEN_COLORS);
EndString+=InitGlobalColor(Me, GL_512, "}\n", ref BG_COLOR_POSITIVE, new Color(0, 0, 0), ref USE_STANDARD_SCREEN_COLORS);
EndString+=InitGlobalColor(Me, GL_513, "}\n", ref BG_COLOR_NEUTRAL, new Color(0, 0, 0), ref USE_STANDARD_SCREEN_COLORS);
EndString+=InitGlobalColor(Me, GL_514, "}\n", ref BG_COLOR_NEGATIVE, new Color(0, 0, 0), ref USE_STANDARD_SCREEN_COLORS);
EndString+=InitGlobalBool(Me, GL_515, ref INVERT_COLOR_SETTINGS, false);
EndString+="\n////-- Sound settings --/////\n";
EndString+=InitGlobalDouble(Me, GL_601, "\n", ref PLAYBACK_TIME, 30, 1, 1800);
EndString+=EndMarker;
int intGSLast = Me.CustomData.IndexOf(EndMarker, 0);
if(intGSLast > 0)
{
intGSLast = intGSLast+EndMarker.Length;
Me.CustomData = Me.CustomData.Remove(0, intGSLast);
}
Me.CustomData = Me.CustomData.Insert(0, EndString);
OldGlobals = Me.CustomData;
TOTALSCRIPTTAG_INT = "["+SCRIPT_TAG+"-"+"I"+"-";
TOTALSCRIPTTAG_EXT = "["+SCRIPT_TAG+"-"+"E"+"-";
TOTALSCRIPTTAG_OPT = "["+SCRIPT_TAG+"-"+"X"+"-";
TOTALSCRIPTTAG = "["+SCRIPT_TAG+"-";
lstPerformance.Clear();
}
return bResult;
}
public bool _BlockGatherer()
{
bool bResult = false;
GathererInfo.Clear();
DateTime RunGathererNow = System.DateTime.UtcNow;
double runtimer = 9.99;
if ((RunGathererNow-RunGatherer).TotalSeconds > runtimer || initialRun)
{
bResult = true;
lstGroup.Clear();
lstTAGDoors.Clear();
lstTAGLights.Clear();
lstTAGScreens.Clear();
lstTAGAirVents.Clear();
GridTerminalSystem.GetBlocksOfType<IMyDoor>(lstDoors, filterOnlyGrid);
GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(lstLights, filterOnlyGrid);
GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lstScreens, filterOnlyGrid);
GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(lstSounds, filterOnlyGrid);
GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(lstTimer, filterOnlyGrid);
GridTerminalSystem.GetBlocksOfType<IMyAirVent>(lstAirVents, filterOnlyGrid);
GridTerminalSystem.GetBlocksOfType<IMyGasTank>(lstGasTanks, filterOnlyGridOxy);
if(lstDoors.Count > 0)
{
for(int i = 0; i < lstDoors.Count; i++)
{
if(lstDoors[i].CustomName.IndexOf(TOTALSCRIPTTAG, StringComparison.OrdinalIgnoreCase) > -1)
{
bool AdvancedAirlock = false;
string strBlock = lstDoors[i].CustomName;
int ndx = strBlock.IndexOf(TOTALSCRIPTTAG, StringComparison.OrdinalIgnoreCase);
while (ndx > -1)
{
int intStringFirst = ndx+TOTALSCRIPTTAG.Length;
int intStringLast = strBlock.IndexOf("]", intStringFirst);
if(intStringFirst > 0 && intStringLast > 0)
{
string FoundTAG = strBlock.Substring(intStringFirst, intStringLast-intStringFirst);
if(FoundTAG != "" && !(string.Equals(FoundTAG, "DETAILS", StringComparison.OrdinalIgnoreCase)))
{
if((FoundTAG.IndexOf("I-", StringComparison.OrdinalIgnoreCase)>-1) || (FoundTAG.IndexOf("E-", StringComparison.OrdinalIgnoreCase)>-1) || (FoundTAG.IndexOf("X-", StringComparison.OrdinalIgnoreCase)>-1))
{
AdvancedAirlock = true;
FoundTAG= FoundTAG.Substring(2,(FoundTAG.Length-2));
}
else
{
AdvancedAirlock = false;
}
lstGroup.Add(new GROUPLIST {GroupTag = FoundTAG.ToUpper(),isAdvanced = AdvancedAirlock});
}
}
ndx = strBlock.IndexOf(TOTALSCRIPTTAG, ndx + TOTALSCRIPTTAG.Length, StringComparison.OrdinalIgnoreCase);
}
lstTAGDoors.Add(lstDoors[i]);
lstDoorOcc.Add(new DOORLIST {Door = lstDoors[i]});
}
}
lstGroup = lstGroup.GroupBy(p => new {p.GroupTag, p.isAdvanced} ).Select(g => g.First()).ToList();
lstDoorOcc = lstDoorOcc.GroupBy(p => new {p.Door} ).Select(g => g.First()).ToList();
for(int i = 0; i < lstDoorOcc.Count; i++)
{
if(lstDoorOcc[i].Door.Closed) {lstDoorOcc.Remove(lstDoorOcc[i]);i--;}
}
foreach(var DOOR in lstDoors) {lstDoorCloser.Add(new DOORLIST {Door = DOOR});}
lstDoorCloser = lstDoorCloser.GroupBy(p => new {p.Door} ).Select(g => g.First()).ToList();
for(int i = 0; i < lstDoorCloser.Count; i++)
{
bool IGNORE_AUTODOOR = (lstDoorCloser[i].Door.CustomData.IndexOf("IGNORE_AUTODOOR", StringComparison.OrdinalIgnoreCase)>-1);
bool IS_AIRLOCK_DOOR = false;
if(lstDoorCloser[i].Door.CustomName.IndexOf(TOTALSCRIPTTAG, StringComparison.OrdinalIgnoreCase) > -1) {IS_AIRLOCK_DOOR = true;}
if(lstDoorCloser[i].Door.Closed || !ENABLE_AUTODOOR) {lstDoorCloser.Remove(lstDoorCloser[i]);i--;}
else if((IGNORE_AUTODOOR || (!IS_AIRLOCK_DOOR && !HANDLE_ALL_DOORS))) {lstDoorCloser.Remove(lstDoorCloser[i]);i--;}
else if(EXCLUDE_HANGAR_DOORS && ((lstDoorCloser[i].Door.BlockDefinition.ToString().IndexOf("HANGAR", StringComparison.OrdinalIgnoreCase)>-1) || (lstDoorCloser[i].Door.CustomData.IndexOf("IS_HANGAR", StringComparison.OrdinalIgnoreCase) > -1))) {lstDoorCloser.Remove(lstDoorCloser[i]);i--;}
}
foreach(var LIGHT in lstLights)
{
if(LIGHT.CustomName.IndexOf(TOTALSCRIPTTAG, StringComparison.OrdinalIgnoreCase) > -1) {lstTAGLights.Add(LIGHT);}
}
foreach(var SCREEN in lstScreens)
{
if(SCREEN.CustomName.IndexOf(TOTALSCRIPTTAG, StringComparison.OrdinalIgnoreCase) > -1) {lstTAGScreens.Add(SCREEN);}
}
foreach(var SOUND in lstSounds)
{
if(SOUND.CustomName.IndexOf(TOTALSCRIPTTAG, StringComparison.OrdinalIgnoreCase) > -1) {lstTAGSounds.Add(new SOUNDLIST {Soundblock = SOUND, SoundIndex = 0});}
}
lstTAGSounds = lstTAGSounds.GroupBy(p => new {p.Soundblock} ).Select(g => g.First()).ToList();
for(int i = 0; i < lstTAGSounds.Count; i++) {if(lstTAGSounds[i].Soundblock.Closed) {lstTAGSounds.Remove(lstTAGSounds[i]);i--;}}
foreach(var TIMER in lstTimer)
{
if(TIMER.CustomName.IndexOf(TOTALSCRIPTTAG, StringComparison.OrdinalIgnoreCase) > -1) {lstTAGTimers.Add(new TIMERLIST {Timerblock = TIMER, TimerIndex = 0});}
}
lstTAGTimers = lstTAGTimers.GroupBy(p => new {p.Timerblock} ).Select(g => g.First()).ToList();
for(int i = 0; i < lstTAGTimers.Count; i++) {if(lstTAGTimers[i].Timerblock.Closed) {lstTAGTimers.Remove(lstTAGTimers[i]);i--;}}
foreach(var AIRVENT in lstAirVents)
{
if(AIRVENT.CustomName.IndexOf(TOTALSCRIPTTAG, StringComparison.OrdinalIgnoreCase) > -1) {lstTAGAirVents.Add(AIRVENT);}
}
}
RunGatherer = System.DateTime.UtcNow;
initialRun = false;
}
GathererInfo.AppendLine(TXT_PBSCREEN_AUTODOOR+": "+lstDoorCloser.Count);
GathererInfo.AppendLine(TXT_PBSCREEN_AIRLOCK+": "+lstGroup.Count);
GathererInfo.AppendLine(TXT_SCREEN_BLOCK_UPDATE+" "+Math.Ceiling(runtimer-((RunGathererNow-RunGatherer).TotalSeconds))+" "+TXT_SCREEN_BLOCK_UPDATE_UNIT+RunnerString+"");
return bResult;
}
public void _AUTODOORCloser()
{
foreach(var AUTODOOR in lstDoorCloser)
{
if((AUTODOOR.Door.BlockDefinition.ToString().IndexOf("HANGAR", StringComparison.OrdinalIgnoreCase)>-1) || (AUTODOOR.Door.CustomData.IndexOf("IS_HANGAR", StringComparison.OrdinalIgnoreCase)>-1))
{
if(AUTODOOR.Door.Status != DoorStatus.Open) {AUTODOOR.OpeningTime = System.DateTime.MinValue;}
else if(AUTODOOR.Door.Status == DoorStatus.Open && AUTODOOR.OpeningTime == System.DateTime.MinValue) {AUTODOOR.OpeningTime = System.DateTime.UtcNow;}
else if(AUTODOOR.Door.Status == DoorStatus.Open && AUTODOOR.OpeningTime != System.DateTime.MinValue)
{
if((System.DateTime.UtcNow-AUTODOOR.OpeningTime).TotalSeconds > (float)InitDouble(AUTODOOR.Door, "CLOSING_TIME{", "}", ref HANGAR_CLOSING_TIME, HANGAR_CLOSING_TIME, 0, 600)) {_setCloseDoor(AUTODOOR.Door);}
}
}
else
{
if(AUTODOOR.Door.Status != DoorStatus.Open) {AUTODOOR.OpeningTime = System.DateTime.MinValue;}
else if(AUTODOOR.Door.Status == DoorStatus.Open && AUTODOOR.OpeningTime == System.DateTime.MinValue) {AUTODOOR.OpeningTime = System.DateTime.UtcNow;}
else if(AUTODOOR.Door.Status == DoorStatus.Open && AUTODOOR.OpeningTime != System.DateTime.MinValue)
{
if((System.DateTime.UtcNow-AUTODOOR.OpeningTime).TotalSeconds > (float)InitDouble(AUTODOOR.Door, "CLOSING_TIME{", "}", ref DOOR_CLOSING_TIME, DOOR_CLOSING_TIME, 0, 600)) {_setCloseDoor(AUTODOOR.Door);}
}
}
}
}
public double _MainLoop(GROUPLIST GROUP)
{
double MainLoopTime = 0;
DateTime MainLoopStart = System.DateTime.UtcNow;
bool ERR_INT_DOOR_MISSING = false;
bool ERR_EXT_DOOR_MISSING = false;
bool ERR_DOOR_MISSING = false;
GRP_DOORS_OPEN = 0;
GRP_INT_DOORS_OPEN = 0;
GRP_EXT_DOORS_OPEN = 0;
GRP_DOORS_OPENING = 0;
GRP_DOORS_CLOSING = 0;
GRP_DOORS_OPENED = 0;
GRP_DOORS_CLOSED = 0;
GRP_INT_DOORS_OPENING = 0;
GRP_INT_DOORS_CLOSING = 0;
GRP_INT_DOORS_OPENED = 0;
GRP_INT_DOORS_CLOSED = 0;
GRP_EXT_DOORS_OPENING = 0;
GRP_EXT_DOORS_CLOSING = 0;
GRP_EXT_DOORS_OPENED = 0;
GRP_EXT_DOORS_CLOSED = 0;
lstGRPDoors.Clear();
lstGRPIntDoors.Clear();
lstGRPExtDoors.Clear();
lstGRPLights.Clear();
lstGRPOptLights.Clear();
lstGRPIntLights.Clear();
lstGRPExtLights.Clear();
lstGRPScreens.Clear();
lstGRPOptScreens.Clear();
lstGRPIntScreens.Clear();
lstGRPExtScreens.Clear();
lstGRPSounds.Clear();
lstGRPTimers.Clear();
lstGRPAirVents.Clear();
if(GROUP.isAdvanced)
{
string TOTALTAG_INT = TOTALSCRIPTTAG_INT+GROUP.GroupTag+"]";
string TOTALTAG_EXT =  TOTALSCRIPTTAG_EXT+GROUP.GroupTag+"]";
string TOTALTAG_OPT =  TOTALSCRIPTTAG_OPT+GROUP.GroupTag+"]";
foreach(var DOOR in lstTAGDoors)
{
if(DOOR.CustomName.IndexOf(TOTALTAG_INT, StringComparison.OrdinalIgnoreCase) > -1)
{
lstGRPIntDoors.Add(DOOR);
lstGRPDoors.Add(DOOR);
if(DOOR.Status == DoorStatus.Open || DOOR.Status == DoorStatus.Opening || DOOR.Status == DoorStatus.Closing){GRP_INT_DOORS_OPEN++;}
if(DOOR.Status == DoorStatus.Open){GRP_DOORS_OPENED++; GRP_INT_DOORS_OPENED++;}
else if(DOOR.Status == DoorStatus.Closed){GRP_DOORS_CLOSED++; GRP_INT_DOORS_CLOSED++;}
else if(DOOR.Status == DoorStatus.Opening){GRP_DOORS_OPENING++; GRP_INT_DOORS_OPENING++;}
else if(DOOR.Status == DoorStatus.Closing){GRP_DOORS_CLOSING++; GRP_INT_DOORS_CLOSING++;}
if(lstTAGDoors.Count(p => p.CustomName.IndexOf(TOTALTAG_EXT, StringComparison.OrdinalIgnoreCase)>-1) < 1)
{
ERR_EXT_DOOR_MISSING = true;
}
}
else if(DOOR.CustomName.IndexOf(TOTALTAG_EXT, StringComparison.OrdinalIgnoreCase) > -1)
{
lstGRPExtDoors.Add(DOOR);
lstGRPDoors.Add(DOOR);
if(DOOR.Status == DoorStatus.Open || DOOR.Status == DoorStatus.Opening || DOOR.Status == DoorStatus.Closing){GRP_EXT_DOORS_OPEN++;}
if(DOOR.Status == DoorStatus.Open){GRP_DOORS_OPENED++; GRP_EXT_DOORS_OPENED++;}
else if(DOOR.Status == DoorStatus.Closed){GRP_DOORS_CLOSED++; GRP_EXT_DOORS_CLOSED++;}
else if(DOOR.Status == DoorStatus.Opening){GRP_DOORS_OPENING++; GRP_EXT_DOORS_OPENING++;}
else if(DOOR.Status == DoorStatus.Closing){GRP_DOORS_CLOSING++; GRP_EXT_DOORS_CLOSING++;}
if(lstTAGDoors.Count(p => p.CustomName.IndexOf(TOTALTAG_INT, StringComparison.OrdinalIgnoreCase)>-1) < 1)
{
ERR_INT_DOOR_MISSING = true;
}
}
}
if(ERR_EXT_DOOR_MISSING)
{
ErrorInfo.AppendLine(TXT_ERR_EXT_DOOR_MISSING+" '"+GROUP.GroupTag+"'!");
ErrorInfo.AppendLine(TXT_SOL_EXT_DOOR_MISSING);
}
else if(ERR_INT_DOOR_MISSING)
{
ErrorInfo.AppendLine(TXT_ERR_INT_DOOR_MISSING+" '"+GROUP.GroupTag+"'!");
ErrorInfo.AppendLine(TXT_SOL_INT_DOOR_MISSING);
}
else
{
foreach(var LIGHT in lstTAGLights)
{
if(LIGHT.CustomName.IndexOf(TOTALTAG_INT, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPIntLights.Add(LIGHT); lstGRPLights.Add(LIGHT);}
else if(LIGHT.CustomName.IndexOf(TOTALTAG_EXT, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPExtLights.Add(LIGHT); lstGRPLights.Add(LIGHT);}
else if(LIGHT.CustomName.IndexOf(TOTALTAG_OPT, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPOptLights.Add(LIGHT); lstGRPLights.Add(LIGHT);}
}
foreach(var SCREEN in lstTAGScreens)
{
if(SCREEN.CustomName.IndexOf(TOTALTAG_INT, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPIntScreens.Add(SCREEN); lstGRPScreens.Add(SCREEN);}
else if(SCREEN.CustomName.IndexOf(TOTALTAG_EXT, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPExtScreens.Add(SCREEN); lstGRPScreens.Add(SCREEN);}
else if(SCREEN.CustomName.IndexOf(TOTALTAG_OPT, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPOptScreens.Add(SCREEN); lstGRPScreens.Add(SCREEN);}
}
foreach(var SOUND in lstTAGSounds)
{
if((SOUND.Soundblock.CustomName.IndexOf(TOTALTAG_INT, StringComparison.OrdinalIgnoreCase) > -1) || (SOUND.Soundblock.CustomName.IndexOf(TOTALTAG_EXT, StringComparison.OrdinalIgnoreCase) > -1) || (SOUND.Soundblock.CustomName.IndexOf(TOTALTAG_OPT, StringComparison.OrdinalIgnoreCase) > -1))
{
lstGRPSounds.Add(new SOUNDLIST {Soundblock = SOUND.Soundblock, SoundIndex = SOUND.SoundIndex});
}
}
foreach(var TIMER in lstTAGTimers)
{
if((TIMER.Timerblock.CustomName.IndexOf(TOTALTAG_INT, StringComparison.OrdinalIgnoreCase) > -1) || (TIMER.Timerblock.CustomName.IndexOf(TOTALTAG_EXT, StringComparison.OrdinalIgnoreCase) > -1) || (TIMER.Timerblock.CustomName.IndexOf(TOTALTAG_OPT, StringComparison.OrdinalIgnoreCase) > -1))
{
lstGRPTimers.Add(new TIMERLIST {Timerblock = TIMER.Timerblock, TimerIndex = TIMER.TimerIndex});
}
}
foreach(var AIRVENT in lstTAGAirVents)
{
if((AIRVENT.CustomName.IndexOf(TOTALTAG_INT, StringComparison.OrdinalIgnoreCase) > -1) || (AIRVENT.CustomName.IndexOf(TOTALTAG_EXT, StringComparison.OrdinalIgnoreCase) > -1) || (AIRVENT.CustomName.IndexOf(TOTALTAG_OPT, StringComparison.OrdinalIgnoreCase) > -1))
{
lstGRPAirVents.Add(AIRVENT);
}
}
if(lstGRPAirVents.Count > 0)
{
bool GROUP_HAS_AIRVENT = true;
float CALCULATED_PRESSURE = 0.0f;
float PRESSURE_INT = (float) 0.0f;
float PRESSURE_EXT = (float) 0.0f;
bool ENOUGH_PRESSURE_EXT = false;
bool ENOUGH_PRESSURE_INT = false;
IMyAirVent REF_AIRVENT_INT = null;
IMyAirVent REF_AIRVENT_EXT = null;
IMyAirVent REF_AIRVENT_AIRLOCK = null;
string AirVentStatus = "";
if(lstGRPAirVents.Count > 1)
{
foreach(var OBJ_GRP_AIRVENT in lstGRPAirVents)
{
if((OBJ_GRP_AIRVENT.CustomData.IndexOf("REFERENCE_AIRVENT", StringComparison.OrdinalIgnoreCase) > -1) && REF_AIRVENT_AIRLOCK == null)
{
REF_AIRVENT_AIRLOCK = OBJ_GRP_AIRVENT;
break;
}
}
}
if(REF_AIRVENT_AIRLOCK == null)
{
REF_AIRVENT_AIRLOCK = lstGRPAirVents[0];
}
CALCULATED_PRESSURE = (float)REF_AIRVENT_AIRLOCK.GetOxygenLevel()*100;
foreach(var OBJ_GRP_AIRVENT in lstAirVents)
{
if(REF_AIRVENT_INT != null && REF_AIRVENT_EXT != null) {break;}
if((OBJ_GRP_AIRVENT.CustomData.IndexOf("REFERENCE_"+GROUP.GroupTag+"_INT", StringComparison.OrdinalIgnoreCase) > -1) && REF_AIRVENT_INT == null) {REF_AIRVENT_INT = OBJ_GRP_AIRVENT;}
if((OBJ_GRP_AIRVENT.CustomData.IndexOf("REFERENCE_"+GROUP.GroupTag+"_EXT", StringComparison.OrdinalIgnoreCase) > -1) && REF_AIRVENT_EXT == null) {REF_AIRVENT_EXT = OBJ_GRP_AIRVENT;}
}
foreach(var OBJ_GRP_AIRVENT in lstAirVents)
{
if(REF_AIRVENT_INT != null && REF_AIRVENT_EXT != null) {break;}
if((OBJ_GRP_AIRVENT.CustomData.IndexOf("REFERENCE_INT", StringComparison.OrdinalIgnoreCase) > -1) && REF_AIRVENT_INT == null) {REF_AIRVENT_INT = OBJ_GRP_AIRVENT;}
if((OBJ_GRP_AIRVENT.CustomData.IndexOf("REFERENCE_EXT", StringComparison.OrdinalIgnoreCase) > -1) && REF_AIRVENT_EXT == null) {REF_AIRVENT_EXT = OBJ_GRP_AIRVENT;}
}
if(REF_AIRVENT_INT != null)
{
PRESSURE_INT = (float)REF_AIRVENT_INT.GetOxygenLevel()*100;
if(CALCULATED_PRESSURE >= PRESSURE_INT) {ENOUGH_PRESSURE_INT = true;}
}
if(REF_AIRVENT_EXT != null)
{
PRESSURE_EXT = (float)REF_AIRVENT_EXT.GetOxygenLevel()*100;
if(CALCULATED_PRESSURE <= PRESSURE_EXT) {ENOUGH_PRESSURE_EXT = true;}
}
if(IGNORE_PRESSURE)
{
if(GRP_INT_DOORS_OPEN > 0 && GRP_EXT_DOORS_OPEN > 0)
{
foreach(var OBJ_GRP_DOOR in lstGRPDoors) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
}
else if(GRP_INT_DOORS_OPEN > 0 && GRP_EXT_DOORS_OPEN < 1)
{
foreach(var OBJ_GRP_DOOR in lstGRPIntDoors) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
foreach(var OBJ_GRP_DOOR in lstGRPExtDoors) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, false);}
}
else if(GRP_INT_DOORS_OPEN < 1 && GRP_EXT_DOORS_OPEN > 0)
{
foreach(var OBJ_GRP_DOOR in lstGRPIntDoors) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, false);}
foreach(var OBJ_GRP_DOOR in lstGRPExtDoors) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
}
else if(GRP_INT_DOORS_OPEN < 1 && GRP_EXT_DOORS_OPEN < 1)
{
foreach(var OBJ_GRP_DOOR in lstGRPDoors) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
}
if(GRP_INT_DOORS_OPEN > 0 && GRP_EXT_DOORS_OPEN > 0)
{
AirVentStatus = "Depressurized";
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OPEN, false);}
}
else if(GRP_INT_DOORS_OPENING > 0)
{
AirVentStatus = "Pressurized";
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_OPENING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_OPENING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_LOCKED, false);}
}
else if(GRP_INT_DOORS_CLOSING > 0)
{
AirVentStatus = "Pressurized";
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_CLOSING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_CLOSING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_LOCKED, false);}
}
else if(GRP_INT_DOORS_OPENED > 0 && GRP_INT_DOORS_OPENING < 1 && GRP_INT_DOORS_CLOSING < 1)
{
AirVentStatus = "Pressurized";
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_OPEN, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_OPEN, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_LOCKED, false);}
}
else if(GRP_EXT_DOORS_OPENING > 0)
{
AirVentStatus = "Depressurized";
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_OPENING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_LOCKED, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_OPENING, true);}
}
else if(GRP_EXT_DOORS_CLOSING > 0)
{
AirVentStatus = "Depressurized";
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_CLOSING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_LOCKED, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_CLOSING, true);}
}
else if(GRP_EXT_DOORS_OPENED > 0 && GRP_EXT_DOORS_OPENING < 1 && GRP_EXT_DOORS_CLOSING < 1)
{
AirVentStatus = "Depressurized";
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_OPEN, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_LOCKED, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_OPEN, false);}
}
else if(GRP_INT_DOORS_CLOSED + GRP_EXT_DOORS_CLOSED == lstGRPDoors.Count())
{
AirVentStatus = REF_AIRVENT_AIRLOCK.Status.ToString();
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_CLEAR, false);}
}
foreach(var OBJ_GRP_SOUND in lstGRPSounds) {lstTAGSounds[lstTAGSounds.FindIndex(x => x.Soundblock == OBJ_GRP_SOUND.Soundblock)].SoundIndex = _setSoundOnOff(OBJ_GRP_SOUND.Soundblock, OBJ_GRP_SOUND.SoundIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_TIMER in lstGRPTimers) {lstTAGTimers[lstTAGTimers.FindIndex(x => x.Timerblock == OBJ_GRP_TIMER.Timerblock)].TimerIndex = _setRunTimer(OBJ_GRP_TIMER.Timerblock, OBJ_GRP_TIMER.TimerIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
}
else
{
if(REF_AIRVENT_INT != null && REF_AIRVENT_EXT != null && PRESSURE_EXT >= PRESSURE_INT && ENOUGH_PRESSURE_EXT)
{
AirVentStatus = "Pressurized";
foreach(var OBJ_GRP_DOOR in lstGRPDoors) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_EQUAL, false);}
foreach(var OBJ_GRP_SOUND in lstGRPSounds) {lstTAGSounds[lstTAGSounds.FindIndex(x => x.Soundblock == OBJ_GRP_SOUND.Soundblock)].SoundIndex = _setSoundOnOff(OBJ_GRP_SOUND.Soundblock, OBJ_GRP_SOUND.SoundIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_TIMER in lstGRPTimers) {lstTAGTimers[lstTAGTimers.FindIndex(x => x.Timerblock == OBJ_GRP_TIMER.Timerblock)].TimerIndex = _setRunTimer(OBJ_GRP_TIMER.Timerblock, OBJ_GRP_TIMER.TimerIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
}
else if(!REF_AIRVENT_AIRLOCK.Depressurize)
{
foreach(var OBJ_GRP_DOOR in lstGRPExtDoors)
{
if(GRP_INT_DOORS_OPEN < 1 && (OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_PRESSURE", StringComparison.OrdinalIgnoreCase) > -1)) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
else if(GRP_INT_DOORS_OPEN > 0) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
else {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, false);}
}
if(CALCULATED_PRESSURE > 98 || ENOUGH_PRESSURE_INT || CheckOxygen("Empty"))
{
AirVentStatus = "Pressurized";
foreach(var OBJ_GRP_DOOR in lstGRPIntDoors)
{
if(GRP_EXT_DOORS_OPEN > 0) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
else{_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
}
if(GRP_INT_DOORS_OPEN > 0 && GRP_EXT_DOORS_OPEN > 0)
{
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OPEN, false);}
}
else
{
if(CheckOxygen("Empty") && CALCULATED_PRESSURE < 100)
{
AirVentStatus = "OxygenEmpty";
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OXYGENEMPTY, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OXYGENEMPTY, false);}
}
else
{
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_PRESSURIZED, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_PRESSURIZED, false);}
}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_LOCKED, false);}
}
foreach(var OBJ_GRP_SOUND in lstGRPSounds) {lstTAGSounds[lstTAGSounds.FindIndex(x => x.Soundblock == OBJ_GRP_SOUND.Soundblock)].SoundIndex = _setSoundOnOff(OBJ_GRP_SOUND.Soundblock, OBJ_GRP_SOUND.SoundIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_TIMER in lstGRPTimers) {lstTAGTimers[lstTAGTimers.FindIndex(x => x.Timerblock == OBJ_GRP_TIMER.Timerblock)].TimerIndex = _setRunTimer(OBJ_GRP_TIMER.Timerblock, OBJ_GRP_TIMER.TimerIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
}
else
{
AirVentStatus = "Pressurizing";
foreach(var OBJ_GRP_DOOR in lstGRPExtDoors)
{
if(OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_PRESSURE", StringComparison.OrdinalIgnoreCase) == -1) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
_setOpenDoor(OBJ_GRP_DOOR, false);
}
foreach(var OBJ_GRP_DOOR in lstGRPIntDoors)
{
_setOpenDoor(OBJ_GRP_DOOR, true);
if(GRP_EXT_DOORS_OPEN < 1 && (OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_PRESSURE", StringComparison.OrdinalIgnoreCase) > -1)) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
else if (GRP_EXT_DOORS_OPEN > 0 || OBJ_GRP_DOOR.Status == DoorStatus.Closed) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
}
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_PRESSURIZING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_PRESSURIZING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_LOCKED, false);}
foreach(var OBJ_GRP_SOUND in lstGRPSounds) {lstTAGSounds[lstTAGSounds.FindIndex(x => x.Soundblock == OBJ_GRP_SOUND.Soundblock)].SoundIndex = _setSoundOnOff(OBJ_GRP_SOUND.Soundblock, OBJ_GRP_SOUND.SoundIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_TIMER in lstGRPTimers) {lstTAGTimers[lstTAGTimers.FindIndex(x => x.Timerblock == OBJ_GRP_TIMER.Timerblock)].TimerIndex = _setRunTimer(OBJ_GRP_TIMER.Timerblock, OBJ_GRP_TIMER.TimerIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
}
}
else if(REF_AIRVENT_AIRLOCK.Depressurize)
{
foreach(var OBJ_GRP_DOOR in lstGRPIntDoors)
{
if(GRP_EXT_DOORS_OPEN < 1 && (OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_PRESSURE", StringComparison.OrdinalIgnoreCase) > -1)) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
else if(GRP_EXT_DOORS_OPEN > 0) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
else {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, false);}
}
if(CALCULATED_PRESSURE < 0.001 || ENOUGH_PRESSURE_EXT || CheckOxygen("Full"))
{
AirVentStatus = "Depressurized";
foreach(var OBJ_GRP_DOOR in lstGRPExtDoors)
{
if(GRP_INT_DOORS_OPEN > 0) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
else{_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
}
if(GRP_INT_DOORS_OPEN > 0 && GRP_EXT_DOORS_OPEN > 0)
{
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OPEN, false);}
}
else
{
if(CheckOxygen("Full") && CALCULATED_PRESSURE > 0.001)
{
AirVentStatus = "OxygenFull";
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OXYGENFULL, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OXYGENFULL, false);}
}
else
{
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_DEPRESSURIZED, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_DEPRESSURIZED, false);}
}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_LOCKED, false);}
}
foreach(var OBJ_GRP_SOUND in lstGRPSounds) {lstTAGSounds[lstTAGSounds.FindIndex(x => x.Soundblock == OBJ_GRP_SOUND.Soundblock)].SoundIndex = _setSoundOnOff(OBJ_GRP_SOUND.Soundblock, OBJ_GRP_SOUND.SoundIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_TIMER in lstGRPTimers) {lstTAGTimers[lstTAGTimers.FindIndex(x => x.Timerblock == OBJ_GRP_TIMER.Timerblock)].TimerIndex = _setRunTimer(OBJ_GRP_TIMER.Timerblock, OBJ_GRP_TIMER.TimerIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
}
else
{
AirVentStatus = "Depressurizing";
foreach(var OBJ_GRP_DOOR in lstGRPIntDoors)
{
if(OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_PRESSURE", StringComparison.OrdinalIgnoreCase) == -1) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
_setOpenDoor(OBJ_GRP_DOOR, false);
}
foreach(var OBJ_GRP_DOOR in lstGRPExtDoors)
{
_setOpenDoor(OBJ_GRP_DOOR, true);
if(GRP_INT_DOORS_OPEN < 1 && (OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_PRESSURE", StringComparison.OrdinalIgnoreCase) > -1)) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
else if (GRP_INT_DOORS_OPEN > 0 || OBJ_GRP_DOOR.Status == DoorStatus.Closed)  {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
}
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_DEPRESSURIZING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_LOCKED, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_DEPRESSURIZING, true);}
foreach(var OBJ_GRP_SOUND in lstGRPSounds) {lstTAGSounds[lstTAGSounds.FindIndex(x => x.Soundblock == OBJ_GRP_SOUND.Soundblock)].SoundIndex = _setSoundOnOff(OBJ_GRP_SOUND.Soundblock, OBJ_GRP_SOUND.SoundIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_TIMER in lstGRPTimers) {lstTAGTimers[lstTAGTimers.FindIndex(x => x.Timerblock == OBJ_GRP_TIMER.Timerblock)].TimerIndex = _setRunTimer(OBJ_GRP_TIMER.Timerblock, OBJ_GRP_TIMER.TimerIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
}
}
}
}
else
{
bool GROUP_HAS_AIRVENT = false;
float CALCULATED_PRESSURE = 0.0f;
string AirVentStatus = "";
if(GRP_INT_DOORS_OPEN > 0 && GRP_EXT_DOORS_OPEN > 0)
{
foreach(var OBJ_GRP_DOOR in lstGRPDoors) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
}
else if(GRP_INT_DOORS_OPEN > 0 && GRP_EXT_DOORS_OPEN < 1)
{
foreach(var OBJ_GRP_DOOR in lstGRPIntDoors) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
foreach(var OBJ_GRP_DOOR in lstGRPExtDoors) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, false);}
}
else if(GRP_INT_DOORS_OPEN < 1 && GRP_EXT_DOORS_OPEN > 0)
{
foreach(var OBJ_GRP_DOOR in lstGRPIntDoors) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, false);}
foreach(var OBJ_GRP_DOOR in lstGRPExtDoors) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
}
else if(GRP_INT_DOORS_OPEN < 1 && GRP_EXT_DOORS_OPEN < 1)
{
foreach(var OBJ_GRP_DOOR in lstGRPDoors) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
}
if(GRP_INT_DOORS_OPEN > 0 && GRP_EXT_DOORS_OPEN > 0)
{
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OPEN, false);}
}
else if(GRP_INT_DOORS_OPENING > 0)
{
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_OPENING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_OPENING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_LOCKED, false);}
}
else if(GRP_INT_DOORS_CLOSING > 0)
{
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_CLOSING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_CLOSING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_LOCKED, false);}
}
else if(GRP_INT_DOORS_OPENED > 0 && GRP_INT_DOORS_OPENING < 1 && GRP_INT_DOORS_CLOSING < 1)
{
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_OPEN, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_OPEN, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_LOCKED, false);}
}
else if(GRP_EXT_DOORS_OPENING > 0)
{
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_OPENING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_LOCKED, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_OPENING, true);}
}
else if(GRP_EXT_DOORS_CLOSING > 0)
{
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_CLOSING, true);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_LOCKED, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_CLOSING, true);}
}
else if(GRP_EXT_DOORS_OPENED > 0 && GRP_EXT_DOORS_OPENING < 1 && GRP_EXT_DOORS_CLOSING < 1)
{
foreach(var OBJ_GRP_LIGHT in lstGRPOptLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPIntLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_LIGHT in lstGRPExtLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPOptScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_OPEN, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPIntScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_INT_DOOR_LOCKED, false);}
foreach(var OBJ_GRP_SCREEN in lstGRPExtScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_EXT_DOOR_OPEN, false);}
}
else if(GRP_INT_DOORS_CLOSED + GRP_EXT_DOORS_CLOSED == lstGRPDoors.Count())
{
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_CLEAR, false);}
}
foreach(var OBJ_GRP_SOUND in lstGRPSounds) {lstTAGSounds[lstTAGSounds.FindIndex(x => x.Soundblock == OBJ_GRP_SOUND.Soundblock)].SoundIndex = _setSoundOnOff(OBJ_GRP_SOUND.Soundblock, OBJ_GRP_SOUND.SoundIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_TIMER in lstGRPTimers) {lstTAGTimers[lstTAGTimers.FindIndex(x => x.Timerblock == OBJ_GRP_TIMER.Timerblock)].TimerIndex = _setRunTimer(OBJ_GRP_TIMER.Timerblock, OBJ_GRP_TIMER.TimerIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
}
}
}
else
{
string TOTALTAG = TOTALSCRIPTTAG+GROUP.GroupTag+"]";
foreach(var DOOR in lstTAGDoors)
{
if(DOOR.CustomName.IndexOf(TOTALTAG, StringComparison.OrdinalIgnoreCase) > -1)
{
lstGRPDoors.Add(DOOR);
if(DOOR.Status == DoorStatus.Open || DOOR.Status == DoorStatus.Opening || DOOR.Status == DoorStatus.Closing){GRP_DOORS_OPEN++;}
if(DOOR.Status == DoorStatus.Open){GRP_DOORS_OPENED++;}
else if(DOOR.Status == DoorStatus.Closed){GRP_DOORS_CLOSED++;}
else if(DOOR.Status == DoorStatus.Opening){GRP_DOORS_OPENING++;}
else if(DOOR.Status == DoorStatus.Closing){GRP_DOORS_CLOSING++;}
if(lstTAGDoors.Count(p => p.CustomName.IndexOf(TOTALTAG, StringComparison.OrdinalIgnoreCase)>-1) < 2)
{
ERR_DOOR_MISSING = true;
}
}
}
if(ERR_DOOR_MISSING)
{
ErrorInfo.AppendLine(TXT_ERR_ONE_DOOR+" '"+GROUP.GroupTag+"'!");
ErrorInfo.AppendLine(TXT_SOL_ONE_DOOR);
}
else
{
foreach(var LIGHT in lstTAGLights)
{
if(LIGHT.CustomName.IndexOf(TOTALTAG, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPLights.Add(LIGHT);}
}
foreach(var SCREEN in lstTAGScreens)
{
if(SCREEN.CustomName.IndexOf(TOTALTAG, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPScreens.Add(SCREEN);}
}
foreach(var SOUND in lstTAGSounds)
{
if(SOUND.Soundblock.CustomName.IndexOf(TOTALTAG, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPSounds.Add(new SOUNDLIST {Soundblock = SOUND.Soundblock, SoundIndex = SOUND.SoundIndex});}
}
foreach(var TIMER in lstTAGTimers)
{
if(TIMER.Timerblock.CustomName.IndexOf(TOTALTAG, StringComparison.OrdinalIgnoreCase) > -1) {lstGRPTimers.Add(new TIMERLIST {Timerblock = TIMER.Timerblock, TimerIndex = TIMER.TimerIndex});}
}
bool GROUP_HAS_AIRVENT = false;
float CALCULATED_PRESSURE = 0.0f;
string AirVentStatus = "";
if(GRP_DOORS_OPEN > 1)
{
foreach(var OBJ_GRP_DOOR in lstGRPDoors) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, true);}
}
else if(GRP_DOORS_OPEN == 1)
{
foreach(var OBJ_GRP_DOOR in lstGRPDoors) {_setLockDoor(OBJ_GRP_DOOR, GROUP.GroupTag, false);}
}
else if(GRP_DOORS_OPEN < 1)
{
foreach(var OBJ_GRP_DOOR in lstGRPDoors) {_setUnlockDoor(OBJ_GRP_DOOR, GROUP.GroupTag);}
}
if(GRP_DOORS_OPENING > 0)
{
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", true, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OPENING, true);}
}
else if(GRP_DOORS_CLOSING > 0)
{
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "NEUTRAL", DO_BLINK_NEUTRAL, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "NEUTRAL", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_CLOSING, true);}
}
else if(GRP_DOORS_OPENED > 0 && GRP_DOORS_OPENING < 1 && GRP_DOORS_CLOSING < 1)
{
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "NEGATIVE", DO_BLINK_NEGATIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "NEGATIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_OPEN, false);}
}
else if(GRP_DOORS_CLOSED == lstGRPDoors.Count())
{
foreach(var OBJ_GRP_LIGHT in lstGRPLights) {_setLight(OBJ_GRP_LIGHT, "POSITIVE", DO_BLINK_POSITIVE, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_SCREEN in lstGRPScreens) {_setScreen(OBJ_GRP_SCREEN, "POSITIVE", GROUP_HAS_AIRVENT, AirVentStatus, CALCULATED_PRESSURE, TXT_AIRLOCK_CLEAR, false);}
}
foreach(var OBJ_GRP_SOUND in lstGRPSounds) {lstTAGSounds[lstTAGSounds.FindIndex(x => x.Soundblock == OBJ_GRP_SOUND.Soundblock)].SoundIndex = _setSoundOnOff(OBJ_GRP_SOUND.Soundblock, OBJ_GRP_SOUND.SoundIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
foreach(var OBJ_GRP_TIMER in lstGRPTimers) {lstTAGTimers[lstTAGTimers.FindIndex(x => x.Timerblock == OBJ_GRP_TIMER.Timerblock)].TimerIndex = _setRunTimer(OBJ_GRP_TIMER.Timerblock, OBJ_GRP_TIMER.TimerIndex, GROUP_HAS_AIRVENT, AirVentStatus);}
}
}
DateTime MainLoopEnd = System.DateTime.UtcNow;
MainLoopTime = (MainLoopEnd-MainLoopStart).TotalMilliseconds;
return MainLoopTime;
}
public void _InfoReporter()
{
ScriptInfo.Clear();
ScriptInfo.AppendLine("Adjustable Airlock & Auto Door");
ScriptInfo.AppendLine("- by Bearnd | Version "+VERSION+" -\n");
ScriptInfo.AppendLine(TXT_DETAILS_SCRIPTTAG+": "+SCRIPT_TAG);
ScriptInfo.AppendLine(TXT_DETAILS_STEALTH+": "+StealthConverter());
ScriptInfo.AppendLine(GathererInfo.ToString());
ScriptInfo.AppendLine(PerformanceInfo.ToString());
ScriptInfo.AppendLine(ErrorInfo.ToString());
Echo(ScriptInfo.ToString());
foreach(var SCREEN in lstScreens)
{
if(SCREEN.CustomName.IndexOf(TOTALSCRIPTTAG+"DETAILS]", StringComparison.OrdinalIgnoreCase) > -1)
{
SCREEN.ContentType = ContentType.TEXT_AND_IMAGE;
SCREEN.WriteText(ScriptInfo.ToString(),false);
}
}
}
public void _PBScreenManager()
{
if(SHOW_PB_SCREEN)
{
SHOW_SCREEN_INIT_TRIGGER = true;
IMyTextSurface PB_SCREEN_SURFACE;
RectangleF PB_VIEWPORT;
PB_SCREEN_SURFACE = Me.GetSurface(0);
PB_SCREEN_SURFACE.ContentType = ContentType.SCRIPT;
PB_SCREEN_SURFACE.Script = "";
PB_SCREEN_SURFACE.ScriptForegroundColor = new Color(255, 255, 255);
PB_SCREEN_SURFACE.ScriptBackgroundColor = new Color(0, 0, 0);
PB_VIEWPORT = new RectangleF((PB_SCREEN_SURFACE.TextureSize - PB_SCREEN_SURFACE.SurfaceSize) / 2f, PB_SCREEN_SURFACE.SurfaceSize);
var frame = PB_SCREEN_SURFACE.DrawFrame();
float ScaleSizeX = ((PB_VIEWPORT.Size[0]/512)+(512/PB_VIEWPORT.Size[0]*0.1f)-0.1f);
float ScalePosX = ((PB_VIEWPORT.Size[0]/512));
float ScalePosY = ((PB_VIEWPORT.Size[1]/320));
float[] DoorRatioL = new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.75f, 0.50f, 0.25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.25f, 0.50f, 0.75f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f};
float[] DoorRatioR = new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.75f, 0.50f, 0.25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.25f, 0.50f, 0.75f, 1f};
DateTime SpriteTimerNow = System.DateTime.UtcNow;
if ((SpriteTimerNow-SpriteTimer).TotalSeconds > 0.16)
{
RatioCounter++;
SpriteTimer = System.DateTime.UtcNow;
}
if(RatioCounter >= DoorRatioL.Count()) {RatioCounter = 0;}
Color DoorStatusL = new Color(255, 0, 0);
Color DoorStatusR = new Color(255, 0, 0);
Color LampStatusL = new Color(255, 0, 0);
Color LampStatusR = new Color(255, 0, 0);
if(DoorRatioR[RatioCounter] == 1f) {DoorStatusL = new Color(0, 255, 0);}
if(DoorRatioL[RatioCounter] == 1f) {DoorStatusR = new Color(0, 255, 0);}
if(DoorRatioL[RatioCounter] == 0f) {LampStatusL = new Color(0, 255, 0);}
if(DoorRatioR[RatioCounter] == 0f) {LampStatusR = new Color(0, 255, 0);}
if(DoorRatioL[RatioCounter] > 0f && DoorRatioL[RatioCounter] < 1f) {LampStatusL = new Color(255, 255, 0);}
if(DoorRatioR[RatioCounter] > 0f && DoorRatioR[RatioCounter] < 1f) {LampStatusR = new Color(255, 255, 0);}
//Text #1
var sprite = new MySprite()
{
Type = SpriteType.TEXT,
Data = "Adjustable Airlock & Auto Door",
Position = new Vector2(PB_VIEWPORT.Center[0], PB_VIEWPORT.Position[1]+(5f*ScalePosY)),
RotationOrScale = 1f * ScaleSizeX,
Color = new Color(0, 255, 175),
Alignment = TextAlignment.CENTER,
FontId = "White"
};
frame.Add(sprite);
//Text #2
sprite = new MySprite()
{
Type = SpriteType.TEXT,
Data = "- by Bearnd | Version "+VERSION+" -",
Position = new Vector2(PB_VIEWPORT.Center[0], PB_VIEWPORT.Position[1]+(30f*ScalePosY)),
RotationOrScale = 0.7f * ScaleSizeX,
Color = new Color(0, 255, 175),
Alignment = TextAlignment.CENTER,
FontId = "White"
};
frame.Add(sprite);
//Text #3
sprite = new MySprite()
{
Type = SpriteType.TEXT,
Data = GathererInfo.ToString(),
Position = new Vector2(PB_VIEWPORT.Center[0], PB_VIEWPORT.Position[1]+(80f*ScalePosY)),
RotationOrScale = 0.55f * ScaleSizeX,
Color = new Color(0, 255, 175),
Alignment = TextAlignment.CENTER,
FontId = "Monospace"
};
frame.Add(sprite);
//Left Door
float PosX = 50f*ScalePosX;
float PosY = 150f*ScalePosY;
float SizX = Math.Min(80f,100f*ScalePosX);
float SizY = SizX*1.75f;
float SizXL = (SizX-14)*DoorRatioL[RatioCounter];
//Left Door - Frame
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX, (PosY+(SizY/2))) + PB_VIEWPORT.Position,
Size = new Vector2(SizX, SizY),
Color = new Color(255, 255, 255),
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Left Door - Background
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX+5, PosY+(SizY/2)+4) + PB_VIEWPORT.Position,
Size = new Vector2(SizX-10, SizY-8),
Color = new Color(0, 0, 0),
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Left Door - Ratio
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX+7, PosY+(SizY/2)+5) + PB_VIEWPORT.Position,
Size = new Vector2(SizXL, SizY-10),
Color = new Color(255, 255, 255),
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Left Door - Lamp
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "Circle",
Position = new Vector2((PosX+SizX)+(50f*ScalePosX), PosY+(20f*ScalePosX)) + PB_VIEWPORT.Position,
Size = new Vector2(40, 40)*ScalePosX,
Color = LampStatusL,
Alignment = TextAlignment.CENTER
};
frame.Add(sprite);
//Left Door - Lock Status Left
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX+5-(2.5f), (PosY+(SizY/2))) + PB_VIEWPORT.Position,
Size = new Vector2(2.5f, 50f),
Color = DoorStatusL,
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX+SizX-4.5f, (PosY+(SizY/2))) + PB_VIEWPORT.Position,
Size = new Vector2(2.5f, 50f),
Color = DoorStatusL,
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Left Door - Lock Status Top
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2((PosX+(SizX/2)-((50f*ScalePosX)/2f)), PosY+4+(ScalePosY/2)+2.5f) + PB_VIEWPORT.Position,
Size = new Vector2(50f*ScalePosX, 2.5f),
Color = DoorStatusL,
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Right Door
PosX = 362f*ScalePosX;
PosY = 150f*ScalePosY;
SizX = Math.Min(80f,100f*ScalePosX);
SizY = SizX*1.75f;
float SizXR = (SizX-14) * DoorRatioR[RatioCounter];
//Right Door - Frame
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX, (PosY+(SizY/2))) + PB_VIEWPORT.Position,
Size = new Vector2(SizX, SizY),
Color = new Color(255, 255, 255),
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Right Door - Background
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX+5, PosY+(SizY/2)+4) + PB_VIEWPORT.Position,
Size = new Vector2(SizX-10, SizY-8),
Color = new Color(0, 0, 0),
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Right Door - Ratio
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX+7, PosY+(SizY/2)+5) + PB_VIEWPORT.Position,
Size = new Vector2(SizXR, SizY-10),
Color = new Color(255, 255, 255),
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Right Door - Lamp
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "Circle",
Position = new Vector2(PosX-(50f*ScalePosX), PosY+(20f*ScalePosX)) + PB_VIEWPORT.Position,
Size = new Vector2(40, 40)*ScalePosX,
Color = LampStatusR,
Alignment = TextAlignment.CENTER
};
frame.Add(sprite);
//Right Door - Lock Status Left
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX+SizX-4.5f, (PosY+(SizY/2))) + PB_VIEWPORT.Position,
Size = new Vector2(2.5f, 50f),
Color = DoorStatusR,
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Right Door - Lock Status Right
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX+2.5f, (PosY+(SizY/2))) + PB_VIEWPORT.Position,
Size = new Vector2(2.5f, 50f),
Color = DoorStatusR,
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Right Door - Lock Status Top
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PosX+(SizX/4), PosY+4+(ScalePosY/2)+2.5f) + PB_VIEWPORT.Position,
Size = new Vector2(50f*ScalePosX, 2.5f),
Color = DoorStatusR,
Alignment = TextAlignment.LEFT
};
frame.Add(sprite);
//Horizontal Bar
PosX = 0f*ScalePosX;
PosY = 50f*ScalePosY;
SizX = 520f*ScalePosX;
SizY = 3f*ScalePosY;
sprite = new MySprite()
{
Type = SpriteType.TEXTURE,
Data = "SquareSimple",
Position = new Vector2(PB_VIEWPORT.Center[0], PB_VIEWPORT.Position[1]+(60f*ScalePosY)),
Size = new Vector2(SizX, SizY),
Color = new Color(255, 255, 255),
Alignment = TextAlignment.CENTER
};
frame.Add(sprite);
frame.Dispose();
}
else
{
if(SHOW_SCREEN_INIT_TRIGGER)
{
IMyTextSurface PB_SCREEN_SURFACE;
PB_SCREEN_SURFACE = Me.GetSurface(0);
PB_SCREEN_SURFACE.ContentType = ContentType.SCRIPT;
PB_SCREEN_SURFACE.Script = "TSS_ClockAnalog";
PB_SCREEN_SURFACE.Script = "";
PB_SCREEN_SURFACE.ContentType = ContentType.NONE;
SHOW_SCREEN_INIT_TRIGGER=false;
}
}
}
public void _setOpenDoor(IMyDoor OBJ_GRP_DOOR, bool OPEN)
{
if(OBJ_GRP_DOOR.Status == DoorStatus.Closed)
{
if(OPEN) {lstDoorOcc[lstDoorOcc.FindIndex(x => x.Door == OBJ_GRP_DOOR)].AutoOpen = 1;}
else {lstDoorOcc[lstDoorOcc.FindIndex(x => x.Door == OBJ_GRP_DOOR)].AutoOpen = 0;}
}
}
public void _setCloseDoor(IMyDoor OBJ_GRP_DOOR)
{
OBJ_GRP_DOOR.Enabled = true;
OBJ_GRP_DOOR.CloseDoor();
}
public void _setLockDoor(IMyDoor OBJ_GRP_DOOR, string GROUP, bool ForceClose)
{
bool IGNORE_CLOSE = (OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_AUTOCLOSE", StringComparison.OrdinalIgnoreCase) > -1);
bool IGNORE_O2LVL = (OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_PRESSURE", StringComparison.OrdinalIgnoreCase) > -1);
bool IGNORE_LOCK = (OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_AUTOLOCK", StringComparison.OrdinalIgnoreCase) > -1);
if(OBJ_GRP_DOOR.Status == DoorStatus.Open)
{
if(!IGNORE_CLOSE && !IGNORE_AUTOCLOSE)
{
if(ForceClose)
{
_setCloseDoor(OBJ_GRP_DOOR);
}
}
}
if(!IGNORE_LOCK && !IGNORE_AUTOLOCK)
{
if(OBJ_GRP_DOOR.Status == DoorStatus.Closed)
{
OBJ_GRP_DOOR.Enabled = false;
int ndx = lstDoorOcc.FindIndex(x => x.Door == OBJ_GRP_DOOR && x.Occupied == null);
if(ndx > -1) {lstDoorOcc[ndx].Occupied = GROUP;}
}
}
else
{
lstDoorOcc[lstDoorOcc.FindIndex(x => x.Door == OBJ_GRP_DOOR)].Occupied = null;
}
}
internal void _setUnlockDoor(IMyDoor OBJ_GRP_DOOR, string GROUP)
{
bool IGNORE_LOCK = (OBJ_GRP_DOOR.CustomData.IndexOf("IGNORE_AUTOLOCK", StringComparison.OrdinalIgnoreCase) > -1);
bool IsSameGroup = false;
int ndx = lstDoorOcc.FindIndex(x => x.Door == OBJ_GRP_DOOR && (x.Occupied == null || string.Equals(x.Occupied, GROUP, StringComparison.OrdinalIgnoreCase)));
bool OPEN_ONCE = (lstDoorOcc[lstDoorOcc.FindIndex(x => x.Door == OBJ_GRP_DOOR)].AutoOpen == 1);
bool AUTO_OPEN = (OBJ_GRP_DOOR.CustomData.IndexOf("AUTO_OPEN", StringComparison.OrdinalIgnoreCase) > -1);
if(ndx > -1) {IsSameGroup = true;}
if(!IGNORE_LOCK && !IGNORE_AUTOLOCK && IsSameGroup)
{
OBJ_GRP_DOOR.Enabled = true;
lstDoorOcc[lstDoorOcc.FindIndex(x => x.Door == OBJ_GRP_DOOR)].Occupied = null;
if(OPEN_ONCE && (ENABLE_AUTOOPEN || AUTO_OPEN))
{
OBJ_GRP_DOOR.OpenDoor();
}
lstDoorOcc[lstDoorOcc.FindIndex(x => x.Door == OBJ_GRP_DOOR)].AutoOpen = 0;
}
}
internal void _setLight(IMyLightingBlock OBJ_GRP_LIGHT, string LightType, bool DoBlink, bool HasAirVent, string AirVentStatus)
{
int iActIndex = 0;
bool IGNORE_STEALTH = OBJ_GRP_LIGHT.CustomData.ToUpper().Contains("IGNORE_STEALTH");
if(!STEALTH_MODE || IGNORE_STEALTH) {iActIndex = checkObjectActivation(OBJ_GRP_LIGHT, HasAirVent, AirVentStatus);}
bool CUSTOM_COLOR = OBJ_GRP_LIGHT.CustomData.ToUpper().Contains("CUSTOM_COLOR");
if(OBJ_GRP_LIGHT.BlockDefinition.ToString().Contains("RotatingLight")) {DoBlink = false;}
if(!CUSTOM_COLOR)
{
if(LightType == "POSITIVE")
{
OBJ_GRP_LIGHT.Color=InitColor(OBJ_GRP_LIGHT, "LIGHT_COLOR_POSITIVE{", "}", ref LIGHT_COLOR_POSITIVE, LIGHT_COLOR_POSITIVE, ref USE_STANDARD_LIGHT_COLORS);
}
else if(LightType == "NEUTRAL")
{
OBJ_GRP_LIGHT.Color=InitColor(OBJ_GRP_LIGHT, "LIGHT_COLOR_NEUTRAL{", "}", ref LIGHT_COLOR_NEUTRAL, LIGHT_COLOR_NEUTRAL, ref USE_STANDARD_LIGHT_COLORS);
}
else if(LightType == "NEGATIVE")
{
OBJ_GRP_LIGHT.Color=InitColor(OBJ_GRP_LIGHT, "LIGHT_COLOR_NEGATIVE{", "}", ref LIGHT_COLOR_NEGATIVE, LIGHT_COLOR_NEGATIVE, ref USE_STANDARD_LIGHT_COLORS);
}
}
if(DoBlink)
{
OBJ_GRP_LIGHT.BlinkIntervalSeconds = (float)InitDouble(OBJ_GRP_LIGHT, "BLINK_INTERVAL{", "}", ref BLINK_INTERVAL, BLINK_INTERVAL, 0, 30);
OBJ_GRP_LIGHT.BlinkLength = (float)InitDouble(OBJ_GRP_LIGHT, "BLINK_LENGTH{", "}", ref BLINK_LENGTH, BLINK_LENGTH, 0, 100);
OBJ_GRP_LIGHT.BlinkOffset = (float)InitDouble(OBJ_GRP_LIGHT, "BLINK_OFFSET{", "}", ref BLINK_OFFSET, BLINK_OFFSET, 0, 100);
}
else
{
OBJ_GRP_LIGHT.BlinkIntervalSeconds = (float)0;
OBJ_GRP_LIGHT.BlinkLength = (float)0;
OBJ_GRP_LIGHT.BlinkOffset = (float)0;
}
if(iActIndex > 0)
{
OBJ_GRP_LIGHT.Enabled = true;
}
else
{
OBJ_GRP_LIGHT.Enabled = false;
}
}
internal void _setScreen(IMyTextPanel OBJ_GRP_SCREEN, string ScreenType, bool HasAirVent, string AirVentStatus, float OxygenLevel, string StatusText, bool HasRunner)
{
int iActIndex = 0;
bool IGNORE_STEALTH = OBJ_GRP_SCREEN.CustomData.ToUpper().Contains("IGNORE_STEALTH");
if(!STEALTH_MODE || IGNORE_STEALTH) {iActIndex = checkObjectActivation(OBJ_GRP_SCREEN, HasAirVent, AirVentStatus);}
string ScreenContent = "";
string USED_FONT = OBJ_GRP_SCREEN.Font;
string SCREEN_TYP = OBJ_GRP_SCREEN.BlockDefinition.SubtypeId.ToString();
bool ON_OFF_MODE = (OBJ_GRP_SCREEN.CustomData.IndexOf("ON_OFF_MODE", StringComparison.OrdinalIgnoreCase) > -1) || USE_ON_OFF;
bool STATIC_TEXT = (OBJ_GRP_SCREEN.CustomData.IndexOf("STATIC_TEXT", StringComparison.OrdinalIgnoreCase) > -1) || !SHOW_DYNAMIC_TEXT;
bool ADD_TEXT = (OBJ_GRP_SCREEN.CustomData.IndexOf("ADD_TEXT{", StringComparison.OrdinalIgnoreCase) > -1);
bool HIDE_STATUS_TEXT = (OBJ_GRP_SCREEN.CustomData.IndexOf("HIDE_STATUSTEXT", StringComparison.OrdinalIgnoreCase) > -1) || !SHOW_STATUS_TEXT;
bool HIDE_PRESSURE_LEVEL = (OBJ_GRP_SCREEN.CustomData.IndexOf("HIDE_O2_LVL", StringComparison.OrdinalIgnoreCase) > -1) || (!SHOW_O2_BAR && !SHOW_O2_PCT);
bool HIDE_PRESSURE_BAR = (OBJ_GRP_SCREEN.CustomData.IndexOf("HIDE_O2_BAR", StringComparison.OrdinalIgnoreCase) > -1) || !SHOW_O2_BAR;
bool HIDE_PRESSURE_PERCENT = (OBJ_GRP_SCREEN.CustomData.IndexOf("HIDE_O2_PCT", StringComparison.OrdinalIgnoreCase) > -1) || !SHOW_O2_PCT;
bool CUSTOM_SIZE = (OBJ_GRP_SCREEN.CustomData.IndexOf("CUSTOM_SIZE", StringComparison.OrdinalIgnoreCase) > -1) || !USE_DEFAULT_FONTSIZE;
bool CUSTOM_ALIGNMENT = (OBJ_GRP_SCREEN.CustomData.IndexOf("CUSTOM_ALIGNMENT", StringComparison.OrdinalIgnoreCase) > -1) || !USE_DEFAULT_ALIGNMENT;
bool CUSTOM_COLOR = (OBJ_GRP_SCREEN.CustomData.IndexOf("CUSTOM_COLOR", StringComparison.OrdinalIgnoreCase) > -1) || USE_COLORS_FROM_SCREEN_BLOCK;
bool INVERTED = (OBJ_GRP_SCREEN.CustomData.IndexOf("INVERTED", StringComparison.OrdinalIgnoreCase) > -1) || INVERT_COLOR_SETTINGS;
if(!ON_OFF_MODE)
{
OBJ_GRP_SCREEN.ContentType = ContentType.TEXT_AND_IMAGE;
if(iActIndex > 0)
{
if(!CUSTOM_ALIGNMENT) {OBJ_GRP_SCREEN.Alignment = TextAlignment.CENTER;}
if(!STATIC_TEXT)
{
if(!CUSTOM_SIZE)
{
if(SCREEN_TYP.Contains("SmallLCDPanel") || SCREEN_TYP.Contains("SmallTextPanel") || SCREEN_TYP.Contains("TransparentLCDSmall") || SCREEN_TYP.Contains("TransparentLCDLarge") || SCREEN_TYP.Contains("LargeLCDPanel") || SCREEN_TYP.Contains("LargeTextPanel")) {if(USED_FONT=="Monospace"){OBJ_GRP_SCREEN.FontSize = 0.85f;}else{OBJ_GRP_SCREEN.FontSize = 1.5f;}}
if(SCREEN_TYP.Contains("SmallBlockCorner") || SCREEN_TYP.Contains("LCDPanelWide")) {if(USED_FONT=="Monospace"){OBJ_GRP_SCREEN.FontSize = 1.8f;}else{OBJ_GRP_SCREEN.FontSize = 3.2f;}}
if(SCREEN_TYP.Contains("LargeBlockCorner")) {if(USED_FONT=="Monospace"){OBJ_GRP_SCREEN.FontSize = 3.5f;}else{OBJ_GRP_SCREEN.FontSize = 4.5f;}}
}
if(ADD_TEXT)
{
ScreenContent = "";
string strCustomText = OBJ_GRP_SCREEN.CustomData;
int intStringFirst = strCustomText.IndexOf("ADD_TEXT{", StringComparison.OrdinalIgnoreCase)+"ADD_TEXT{".Length;
int intStringLast = strCustomText.IndexOf("}", intStringFirst);
if(intStringFirst > 0 && intStringLast > 0)
{
string strStatusText = strCustomText.Substring(intStringFirst, intStringLast-intStringFirst);
strStatusText= strStatusText.Replace("\n", "");
strStatusText= strStatusText.Replace("#ECHO#", "\n");
ScreenContent+= strStatusText;
}
ScreenContent+="\n";
}
if(!HIDE_STATUS_TEXT)
{
ScreenContent+= StatusText;
if(HasRunner)
{
ScreenContent+= RunnerString;
}
ScreenContent+= "\n";
}
if(HasAirVent)
{
if(!HIDE_PRESSURE_LEVEL)
{
double dOxygenLevel = Math.Max(0, Math.Min(100, Convert.ToDouble(OxygenLevel)));
double tOxygenLevel = (Math.Truncate((dOxygenLevel) * 100) / 100);
string sOxygenLevel = string.Format("{0:N1}%", Math.Min(100,tOxygenLevel));
if(!HIDE_PRESSURE_BAR)
{
double FSize = OBJ_GRP_SCREEN.FontSize;
double FSizeRatio = 100;
if(SCREEN_TYP.Contains("SmallLCDPanel") || SCREEN_TYP.Contains("SmallTextPanel") || SCREEN_TYP.Contains("TransparentLCDSmall") || SCREEN_TYP.Contains("TransparentLCDLarge") || SCREEN_TYP.Contains("LargeLCDPanel") || SCREEN_TYP.Contains("LargeTextPanel")) {if(USED_FONT=="Monospace"){FSizeRatio = 25;}else{FSizeRatio = 80;}}
if(SCREEN_TYP.Contains("SmallBlockCorner") || SCREEN_TYP.Contains("LCDPanelWide")) {if(USED_FONT=="Monospace"){FSizeRatio = 50;}else{FSizeRatio = 170;}}
if(SCREEN_TYP.Contains("LargeBlockCorner")) {if(USED_FONT=="Monospace"){FSizeRatio = 100;}else{FSizeRatio = 340;}}
double douPartFull = Math.Round((FSizeRatio/100*dOxygenLevel/FSize), MidpointRounding.ToEven);
double douPartTotal = Math.Round((FSizeRatio/100*100/FSize), MidpointRounding.ToEven);
double douPartEmpty = douPartTotal-douPartFull;
int intPartFull = Convert.ToInt32(douPartFull);
int intPartEmpty = Convert.ToInt32(douPartEmpty);
int pctCounter = 0;
if(USED_FONT=="Monospace"){pctCounter= 4;}
if(!HIDE_PRESSURE_PERCENT)
{
pctCounter= 19;
if(USED_FONT=="Monospace"){pctCounter= 12;}
}
int pctPartFullCounter = Convert.ToInt32(pctCounter*(douPartFull/douPartTotal));
int pctPartEmptyCounter = Convert.ToInt32(pctCounter*(douPartEmpty/douPartTotal));
intPartFull = intPartFull - pctPartFullCounter;
intPartEmpty = intPartEmpty - pctPartEmptyCounter;
if(intPartFull<=0 && intPartEmpty<=0){intPartFull=0;intPartEmpty=0;}
string PartFull = "";
for(int i = 0; i < intPartFull; i=i+10) {PartFull = PartFull+"||||||||||";}
string PartEmpty = "";
for(int i = 0; i < intPartEmpty; i=i+10) {PartEmpty = PartEmpty+"''''''''''";}
string strBar = PartFull.Substring(0, intPartFull)+PartEmpty.Substring(0, intPartEmpty);
ScreenContent+="["+strBar+"]";
}
if(!HIDE_PRESSURE_BAR && !HIDE_PRESSURE_PERCENT)
{
string strSpace = "";
if(USED_FONT=="Monospace")
{
int intSpaceCounter = 8-sOxygenLevel.Length;
for(int i = 0; i < intSpaceCounter; i++) {strSpace=strSpace+" ";}
}
else
{
double counter = 13;
foreach(char charPCT in sOxygenLevel)
{
if(charPCT=='1') {counter = counter - 1.125;}
else if(charPCT=='3' || charPCT=='7') {counter = counter - 2;}
else if(charPCT=='4') {counter = counter - 2.2;}
else {counter = counter - 2.25;}
}
int intSpaceCounter = Convert.ToInt32(Math.Round((counter+2), MidpointRounding.ToEven));
for(int i = 0; i < intSpaceCounter; i++) {strSpace=strSpace+" ";}
}
ScreenContent+=strSpace+sOxygenLevel;
}
else if(HIDE_PRESSURE_BAR && !HIDE_PRESSURE_PERCENT)
{
ScreenContent+="[ "+sOxygenLevel+" ]";
}
}
OBJ_GRP_SCREEN.WriteText(ScreenContent, false);
}
else
{
OBJ_GRP_SCREEN.WriteText(ScreenContent, false);
}
}
}
}
else
{
if(iActIndex > 0)
{
OBJ_GRP_SCREEN.Enabled = true;
}
else
{
OBJ_GRP_SCREEN.Enabled = false;
}
}
if(!CUSTOM_COLOR)
{
if(iActIndex > 0)
{
OBJ_GRP_SCREEN.Enabled = true;
if(ScreenType == "POSITIVE")
{
OBJ_GRP_SCREEN.FontColor=InitColor(OBJ_GRP_SCREEN, "FONT_COLOR_POSITIVE{", "}", ref FONT_COLOR_POSITIVE, FONT_COLOR_POSITIVE, ref USE_STANDARD_SCREEN_COLORS);
OBJ_GRP_SCREEN.BackgroundColor=InitColor(OBJ_GRP_SCREEN, "BG_COLOR_POSITIVE{", "}", ref BG_COLOR_POSITIVE, BG_COLOR_POSITIVE, ref USE_STANDARD_SCREEN_COLORS);
if(USE_STANDARD_SCREEN_COLORS && INVERTED)
{
OBJ_GRP_SCREEN.FontColor = new Color(255, 255, 255);
OBJ_GRP_SCREEN.BackgroundColor = new Color(10, 50, 10);
}
}
else if(ScreenType == "NEUTRAL")
{
OBJ_GRP_SCREEN.FontColor=InitColor(OBJ_GRP_SCREEN, "FONT_COLOR_NEUTRAL{", "}", ref FONT_COLOR_NEUTRAL, FONT_COLOR_NEUTRAL, ref USE_STANDARD_SCREEN_COLORS);
OBJ_GRP_SCREEN.BackgroundColor=InitColor(OBJ_GRP_SCREEN, "BG_COLOR_NEUTRAL{", "}", ref BG_COLOR_NEUTRAL, BG_COLOR_NEUTRAL, ref USE_STANDARD_SCREEN_COLORS);
if(USE_STANDARD_SCREEN_COLORS && INVERTED)
{
OBJ_GRP_SCREEN.FontColor = new Color(0, 0, 0);
OBJ_GRP_SCREEN.BackgroundColor = new Color(100, 80, 0);
}
}
else if(ScreenType == "NEGATIVE")
{
OBJ_GRP_SCREEN.FontColor=InitColor(OBJ_GRP_SCREEN, "FONT_COLOR_NEGATIVE{", "}", ref FONT_COLOR_NEGATIVE, FONT_COLOR_NEGATIVE, ref USE_STANDARD_SCREEN_COLORS);
OBJ_GRP_SCREEN.BackgroundColor=InitColor(OBJ_GRP_SCREEN, "BG_COLOR_NEGATIVE{", "}", ref BG_COLOR_NEGATIVE, BG_COLOR_NEGATIVE, ref USE_STANDARD_SCREEN_COLORS);
if(USE_STANDARD_SCREEN_COLORS && INVERTED)
{
OBJ_GRP_SCREEN.FontColor = new Color(255, 255, 255);
OBJ_GRP_SCREEN.BackgroundColor = new Color(100, 2, 2);
}
}
}
else
{
OBJ_GRP_SCREEN.FontColor = new Color(1, 1, 1, 0);
OBJ_GRP_SCREEN.BackgroundColor = new Color(0, 0, 0, 0);
}
}
else
{
if(iActIndex > 0)
{
OBJ_GRP_SCREEN.Enabled = true;
}
else
{
OBJ_GRP_SCREEN.Enabled = false;
}
}
}
public int _setSoundOnOff(IMySoundBlock OBJ_GRP_SOUND, int SOUND_ID, bool HasAirVent, string AirVentStatus)
{
int iActIndex = 0;
bool IGNORE_STEALTH = (OBJ_GRP_SOUND.CustomData.IndexOf("IGNORE_STEALTH", StringComparison.OrdinalIgnoreCase) > -1);
if(!STEALTH_MODE || IGNORE_STEALTH) {iActIndex = checkObjectActivation(OBJ_GRP_SOUND, HasAirVent, AirVentStatus);}
if(SOUND_ID != iActIndex)
{
if(iActIndex == 0)
{
OBJ_GRP_SOUND.Stop();
SOUND_ID = 0;
}
else
{
OBJ_GRP_SOUND.LoopPeriod = (float)PLAYBACK_TIME;
OBJ_GRP_SOUND.Play();
SOUND_ID = iActIndex;
}
}
else if(iActIndex == 0)
{
OBJ_GRP_SOUND.Stop();
SOUND_ID = 0;
}
return SOUND_ID;
}
public int _setRunTimer(IMyTimerBlock OBJ_GRP_TIMER, int TIMER_ID, bool HasAirVent, string AirVentStatus)
{
int iActIndex = checkObjectActivation(OBJ_GRP_TIMER, HasAirVent, AirVentStatus);
bool TRIGGERNOW = (OBJ_GRP_TIMER.CustomData.IndexOf("TRIGGERNOW", StringComparison.OrdinalIgnoreCase) > -1);
bool START = (OBJ_GRP_TIMER.CustomData.IndexOf("START", StringComparison.OrdinalIgnoreCase) > -1);
if(TIMER_ID != iActIndex)
{
if(iActIndex == 0)
{
TIMER_ID = 0;
}
else
{
if(TRIGGERNOW)
{
OBJ_GRP_TIMER.Trigger();
}
else if(START)
{
OBJ_GRP_TIMER.StartCountdown();
}
TIMER_ID = iActIndex;
}
}
else if(iActIndex == 0)
{
OBJ_GRP_TIMER.StopCountdown();
TIMER_ID = 0;
}
return TIMER_ID;
}
public int checkObjectActivation(IMyTerminalBlock GRP_OBJ, bool HasAirVent, string AirVentStatus)
{
int iResultID = 0;
bool USE_DEFAULT_SETTING = true;
bool PRESSURIZING = (GRP_OBJ.CustomData.IndexOf("IS_PRESSURIZING", StringComparison.OrdinalIgnoreCase) > -1);
bool DEPRESSURIZING = (GRP_OBJ.CustomData.IndexOf("IS_DEPRESSURIZING", StringComparison.OrdinalIgnoreCase) > -1);
bool PRESSURIZED = (GRP_OBJ.CustomData.IndexOf("IS_PRESSURIZED", StringComparison.OrdinalIgnoreCase) > -1);
bool DEPRESSURIZED = (GRP_OBJ.CustomData.IndexOf("IS_DEPRESSURIZED", StringComparison.OrdinalIgnoreCase) > -1);
bool OXYGEN_FULL = (GRP_OBJ.CustomData.IndexOf("OXYGEN_FULL", StringComparison.OrdinalIgnoreCase) > -1);
bool OXYGEN_EMPTY = (GRP_OBJ.CustomData.IndexOf("OXYGEN_EMPTY", StringComparison.OrdinalIgnoreCase) > -1);
bool OPENING_ANY = (GRP_OBJ.CustomData.IndexOf("OPENING_ANY", StringComparison.OrdinalIgnoreCase) > -1);
bool CLOSING_ANY = (GRP_OBJ.CustomData.IndexOf("CLOSING_ANY", StringComparison.OrdinalIgnoreCase) > -1);
bool OPENED_ANY = (GRP_OBJ.CustomData.IndexOf("OPENED_ANY", StringComparison.OrdinalIgnoreCase) > -1);
bool CLOSED_ANY = (GRP_OBJ.CustomData.IndexOf("CLOSED_ANY", StringComparison.OrdinalIgnoreCase) > -1);
bool OPENING_INT = (GRP_OBJ.CustomData.IndexOf("OPENING_INT", StringComparison.OrdinalIgnoreCase) > -1);
bool CLOSING_INT = (GRP_OBJ.CustomData.IndexOf("CLOSING_INT", StringComparison.OrdinalIgnoreCase) > -1);
bool OPENED_INT = (GRP_OBJ.CustomData.IndexOf("OPENED_INT", StringComparison.OrdinalIgnoreCase) > -1);
bool CLOSED_INT = (GRP_OBJ.CustomData.IndexOf("CLOSED_INT", StringComparison.OrdinalIgnoreCase) > -1);
bool OPENING_EXT = (GRP_OBJ.CustomData.IndexOf("OPENING_EXT", StringComparison.OrdinalIgnoreCase) > -1);
bool CLOSING_EXT = (GRP_OBJ.CustomData.IndexOf("CLOSING_EXT", StringComparison.OrdinalIgnoreCase) > -1);
bool OPENED_EXT = (GRP_OBJ.CustomData.IndexOf("OPENED_EXT", StringComparison.OrdinalIgnoreCase) > -1);
bool CLOSED_EXT = (GRP_OBJ.CustomData.IndexOf("CLOSED_EXT", StringComparison.OrdinalIgnoreCase) > -1);
if(PRESSURIZING || DEPRESSURIZING || PRESSURIZED || DEPRESSURIZED || OXYGEN_FULL || OXYGEN_EMPTY || OPENING_ANY || CLOSING_ANY || OPENED_ANY || CLOSED_ANY	|| OPENING_INT || CLOSING_INT || OPENED_INT || CLOSED_INT || OPENING_EXT || CLOSING_EXT ||  OPENED_EXT || CLOSED_EXT)
{
USE_DEFAULT_SETTING = false;
}
if(USE_DEFAULT_SETTING)
{
if(GRP_OBJ.BlockDefinition.ToString().IndexOf("LIGHT", StringComparison.OrdinalIgnoreCase)>-1)
{
if(HasAirVent) {iResultID = 1;}
else if(GRP_DOORS_OPENING > 0) {iResultID = 7;}
else if(GRP_DOORS_CLOSING > 0) {iResultID = 8;}
else if(GRP_DOORS_OPENED > 0) {iResultID = 9;}
else {iResultID = 0;}
}
if(GRP_OBJ.BlockDefinition.ToString().IndexOf("SOUNDBLOCK", StringComparison.OrdinalIgnoreCase)>-1)
{
if(GRP_DOORS_OPENING > 0) {iResultID = 7;}
else if(GRP_DOORS_CLOSING > 0) {iResultID = 8;}
else if(GRP_DOORS_OPENED > 0) {iResultID = 9;}
else if(CheckOxygen("Empty")) {iResultID = 6;}
else {iResultID = 0;}
}
if(GRP_OBJ.BlockDefinition.ToString().IndexOf("TEXTPANEL", StringComparison.OrdinalIgnoreCase)>-1) {iResultID = 1;}
if(GRP_OBJ.BlockDefinition.ToString().IndexOf("TIMERBLOCK", StringComparison.OrdinalIgnoreCase)>-1) {iResultID = 0;}
}
else
{
if(PRESSURIZING && AirVentStatus == "Pressurizing")	{iResultID =  1;}
else if(DEPRESSURIZING && AirVentStatus == "Depressurizing") {iResultID = 2;}
else if(PRESSURIZED && AirVentStatus == "Pressurized") {iResultID = 3;}
else if(DEPRESSURIZED && AirVentStatus == "Depressurized") {iResultID = 4;}
else if(OXYGEN_FULL && AirVentStatus == "OxygenFull") {iResultID = 5;}
else if(OXYGEN_EMPTY && AirVentStatus == "OxygenEmpty") {iResultID = 6;}
else if(OPENING_ANY && GRP_DOORS_OPENING > 0) {iResultID = 7;}
else if(CLOSING_ANY && GRP_DOORS_CLOSING > 0) {iResultID = 8;}
else if(OPENED_ANY && GRP_DOORS_OPENED > 0) {iResultID = 9;}
else if(CLOSED_ANY && GRP_DOORS_CLOSED == lstGRPDoors.Count()) {iResultID = 10;}
else if(OPENING_INT && GRP_INT_DOORS_OPENING > 0) {iResultID = 11;}
else if(CLOSING_INT && GRP_INT_DOORS_CLOSING > 0) {iResultID = 12;}
else if(OPENED_INT && GRP_INT_DOORS_OPENED > 0) {iResultID = 13;}
else if(CLOSED_INT && GRP_INT_DOORS_CLOSED > 0) {iResultID = 14;}
else if(OPENING_EXT && GRP_EXT_DOORS_OPENING > 0) {iResultID = 15;}
else if(CLOSING_EXT && GRP_EXT_DOORS_CLOSING > 0) {iResultID = 16;}
else if(OPENED_EXT && GRP_EXT_DOORS_OPENED > 0) {iResultID = 17;}
else if(CLOSED_EXT && GRP_EXT_DOORS_CLOSED > 0) {iResultID = 18;}
else {iResultID = 0;}
}
return iResultID;
}
public void _SetRunnerString()
{
if ((System.DateTime.UtcNow-RunnerTimer).TotalSeconds > 0.32)
{
RunnerSB.Clear();
if(intRunner > 3) {intRunner = 0;}
for (int i = 0; i < intRunner; i++) {RunnerSB.Append(".");}
for (int i = 0; i < (3 - intRunner); i++) {RunnerSB.Append(" ");}
intRunner++;
RunnerTimer = System.DateTime.UtcNow;
}
RunnerString = RunnerSB.ToString();
}
public string StealthConverter()
{
string strStealthMode = "OFF";
if (STEALTH_MODE){strStealthMode = "ON";}
return strStealthMode;
}
public bool CheckOxygen(string OxygenState)
{
bool bResult = true;
if (lstGasTanks.Count == 0)
{
bResult = true;
}
else
{
foreach (var TANK in lstGasTanks)
{
if(OxygenState == "Empty")
{
if(TANK.FilledRatio > 0)
{
bResult = false;
break;
}
}
else if(OxygenState == "Full")
{
if (TANK.FilledRatio < 1)
{
bResult = false;
break;
}
}
}
}
return bResult;
}
public string InitGlobalBool(IMyTerminalBlock Block_OBJ, string ScriptSetting, ref bool ScriptVar, bool BaseDefault)
{
bool ResultValue = ScriptVar;
string FoundValue = "";
string FoundLine = "";
string strCustomText = NewGlobals;
if(!strCustomText.Contains(ScriptSetting))
{
ResultValue = BaseDefault;
FoundLine = ScriptSetting+BaseDefault.ToString()+"\n";
}
else
{
int intStringFirst = strCustomText.IndexOf(ScriptSetting, StringComparison.OrdinalIgnoreCase)+ScriptSetting.Length;
int intStringLast = strCustomText.IndexOf("\n", intStringFirst);
FoundValue = strCustomText.Substring(intStringFirst, intStringLast-intStringFirst);
FoundLine = ScriptSetting+FoundValue+"\n";
if(FoundValue.Length < 1) {ResultValue=BaseDefault;}
if((FoundValue.IndexOf((!BaseDefault).ToString(), 0, StringComparison.OrdinalIgnoreCase) != -1))
{
ResultValue=!BaseDefault;
}
else
{
ResultValue=BaseDefault;
}
}
ScriptVar = ResultValue;
string ValueLine = ScriptSetting+ScriptVar.ToString()+"\n";
NewGlobals = strCustomText.Replace(FoundLine,ValueLine);
return ValueLine;
}
public string InitGlobalDouble(IMyTerminalBlock Block_OBJ, string Command, string EndCommand, ref double ScriptVar, double BaseDefault, double minVal, double maxVal)
{
string ValueLine = "";
ValueLine = Command+InitDouble(Block_OBJ, Command, EndCommand, ref ScriptVar, BaseDefault, minVal, maxVal).ToString()+EndCommand;
return ValueLine;
}
public double InitDouble(IMyTerminalBlock Block_OBJ, string Command, string EndCommand, ref double ScriptVar, double BaseDefault, double minVal, double maxVal)
{
bool bool_Setting = false;
double ResultValue = ScriptVar;
int intStringFirst = -1;
int intStringLast = -1;
string ValueLine = "";
string strCustomText = "";
string FoundValue = "";
string FoundLine = "";
if(Block_OBJ == Me) {strCustomText = NewGlobals;}
else {strCustomText = Block_OBJ.CustomData;}
intStringFirst = strCustomText.IndexOf(Command, StringComparison.OrdinalIgnoreCase);
if(intStringFirst>-1)
{
intStringFirst = intStringFirst+Command.Length;
intStringLast = strCustomText.IndexOf(EndCommand, intStringFirst);
if(intStringLast>-1)
{
bool_Setting = true;
}
}
if(!bool_Setting)
{
double.TryParse(BaseDefault.ToString(), out ResultValue);
ValueLine = Command+BaseDefault+EndCommand;
FoundLine = ValueLine;
}
else
{
FoundValue = strCustomText.Substring(intStringFirst, intStringLast-intStringFirst);
FoundLine = strCustomText.Substring(strCustomText.IndexOf(Command, StringComparison.OrdinalIgnoreCase),(Command+FoundValue+EndCommand).Length);
if(String.IsNullOrWhiteSpace(FoundValue) || FoundValue.Length < 1) {FoundValue = BaseDefault.ToString();}
double.TryParse(FoundValue, out ResultValue);
if(maxVal==1800)
{
ResultValue = Math.Round(MathHelper.Clamp(ResultValue, minVal, maxVal),3);
}
else
{
ResultValue = Math.Round(MathHelper.Clamp(ResultValue, minVal, maxVal),1);
}
ValueLine = Command+" "+ResultValue.ToString()+" "+EndCommand;
}
if(Block_OBJ == Me)
{
ScriptVar = ResultValue;
NewGlobals = strCustomText.Replace(FoundLine,ValueLine);
}
else
{
if(bool_Setting) {Block_OBJ.CustomData = strCustomText.Replace(FoundLine,ValueLine);}
}
return ResultValue;
}
public string InitGlobalColor(IMyTerminalBlock Block_OBJ, string Command, string EndCommand, ref Color ScriptVar, Color BaseDefault, ref bool DefaultSwitch)
{
string ValueLine = "";
Color ColorValue = InitColor(Block_OBJ, Command, EndCommand, ref ScriptVar, BaseDefault, ref DefaultSwitch);
ValueLine = Command+string.Format(" {0} , {1} , {2} ", ColorValue.R, ColorValue.G, ColorValue.B)+EndCommand;
return ValueLine;
}
public Color InitColor(IMyTerminalBlock Block_OBJ, string Command, string EndCommand, ref Color ScriptVar, Color BaseDefault, ref bool DefaultSwitch)
{
bool bool_Setting = false;
Color ResultValue = ScriptVar;
int intStringFirst = -1;
int intStringLast = -1;
string ValueLine = "";
string strCustomText = "";
string FoundValue = "";
string FoundLine = "";
if(Block_OBJ == Me) {strCustomText = NewGlobals;}
else {strCustomText = Block_OBJ.CustomData;}
intStringFirst = strCustomText.IndexOf(Command, StringComparison.OrdinalIgnoreCase);
if(intStringFirst>-1)
{
intStringFirst = intStringFirst+Command.Length;
intStringLast = strCustomText.IndexOf(EndCommand, intStringFirst);
if(intStringLast>-1)
{
bool_Setting = true;
}
}
if(!bool_Setting)
{
ResultValue = BaseDefault;
ValueLine = Command+string.Format(" {0} , {1} , {2} ", BaseDefault.R, BaseDefault.G, BaseDefault.B)+EndCommand;
FoundLine = ValueLine;
}
else
{
FoundValue = strCustomText.Substring(intStringFirst, intStringLast-intStringFirst);
FoundLine = strCustomText.Substring(strCustomText.IndexOf(Command, StringComparison.OrdinalIgnoreCase),(Command+FoundValue+EndCommand).Length);
if(String.IsNullOrWhiteSpace(FoundValue) || FoundValue.Length < 1) {FoundValue = string.Format(" {0} , {1} , {2} ", BaseDefault.R, BaseDefault.G, BaseDefault.B);}
int[] RGBValue = new int[] {0,0,0};
string[] FoundSplit = FoundValue.Split(',');
for (int i = 0; i < Math.Min(3,FoundSplit.Count()); i++)
{
int.TryParse(FoundSplit[i].Trim(), out RGBValue[i]);
RGBValue[i] = MathHelper.Clamp(RGBValue[i], 0, 255);
}
ResultValue = new Color(RGBValue[0],RGBValue[1],RGBValue[2]);
ValueLine = Command+string.Format(" {0} , {1} , {2} ", ResultValue.R, ResultValue.G, ResultValue.B)+EndCommand;
}
if(Block_OBJ == Me)
{
ScriptVar = ResultValue;
NewGlobals = strCustomText.Replace(FoundLine,ValueLine);
if(ResultValue!=BaseDefault)
{
DefaultSwitch=false;
}
}
else
{
if(bool_Setting) {Block_OBJ.CustomData = strCustomText.Replace(FoundLine,ValueLine);}
}
return ResultValue;
}
public class PERFORMANCELIST
{
public double RunningTime { get; set; }
public double RunningFrequence { get; set; }
public double SumInstructions { get; set; }
}
public class GROUPLIST
{
public string GroupTag { get; set; }
public bool isAdvanced { get; set; }
}
public class DOORLIST
{
public IMyDoor Door { get; set; }
public DateTime OpeningTime { get; set; }
public string Occupied { get; set; }
public int AutoOpen { get; set; }
}
public class TIMERLIST
{
public IMyTimerBlock Timerblock { get; set; }
public int TimerIndex { get; set; }
}
public class SOUNDLIST
{
public IMySoundBlock Soundblock { get; set; }
public int SoundIndex { get; set; }
}
public bool filterOnlyGrid(IMyTerminalBlock block)
{
return block.CubeGrid == Me.CubeGrid;
}
public bool filterOnlyGridOxy(IMyTerminalBlock block)
{
return (block.CubeGrid == Me.CubeGrid && ((block.BlockDefinition.ToString().Contains("Oxygen") && !block.BlockDefinition.SubtypeId.Contains("Hydrogen")) || block.DefinitionDisplayNameText.Contains("Oxygen")));
}