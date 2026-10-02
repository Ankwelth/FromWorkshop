//Ver 2.4 2022/09/21

//комнды для работы
//"START" - начать (продолжить)
//"RESTART" - обновить настройки и начать все заново
//"STOP" - остановить

//---------Основные настройки-------------------------------------------------

//[1..4] макс количество 'шагов'. 'Шаг' - это кольцо вокруг Нанобура и равен 25м/75м для малой/большой сетки
private static int MaxStepsXZ = 4;

//Бурить только вниз (необходимо, когда находишься на поверхности)
private static bool DownOnly = false;

//[0..4] макс 'шагов' по высоте. 
//Если 0, то какая высоые в данный момент установленна, то на той и будет добывать.
//Если 1..4, то это дополнительные высоты, относительно самого Нанобура вверх и вниз
private static int MaxStepsY = 1;

//время ожидания Нанобура в секундах, пока будут обнаружеры руды
private static int IdleTime = 7;

//часть названия Экрана для отображения информации о состоянии процесса бурения. (данный блок не обязателен)
string LCDName = "[NanoInfo]";

//------------------------------------------------------------------------------------
int MaxDrillsCount = 6;
const string AO = "AreaOffset", X = "LeftRight", Y = "UpDown", Z = "FrontBack";

int TIK = 0;
List<MyDrill> Drills = new List<MyDrill>();  //список нанобуров

public enum DrillStates  // возможные состояния бура
{
Drilling,
Waiting,
Stop,
End,
Error
}

DrillStates SystemState;  //текущее состояние системы
static DateTime tLastTime;

public static Dictionary<string, string> LANG = new Dictionary<string, string>(); //словарь перевода

//чтение установоу из CustomData
void GetSettings()
{
bool SettingsIsChanged = false;
int temp;
foreach (string str in Me.CustomData.Split('\n'))
{
if (str.Contains("="))
{
try
{
string[] data = str.Split('=');
string key = data[0].Trim();
string value = data[1].Trim();

switch (key)
{
case "MaxStepsXZ":
temp = Int32.Parse(value);
if (temp != MaxStepsXZ)
{
	MaxStepsXZ = temp;
	SettingsIsChanged = true;
}
break;
case "MaxStepsY":
temp = Int32.Parse(value);
if (temp != MaxStepsY)
{
	MaxStepsY = temp;
	SettingsIsChanged = true;
}
break;
case "DownOnly":
bool dOnly = bool.Parse(value);
if (dOnly != DownOnly)
{
	DownOnly = dOnly;
	SettingsIsChanged = true;
}
break;
case "IdleTime":
IdleTime = Int32.Parse(value);
break;
case "LCDName":
LCDName = value;
break;
}
}
finally { };
}
}
if (SettingsIsChanged || DrillSteps.Count == 0)
{
DrillSteps.Clear();
GetSteps();
}
}

//процедура перевода. Необходим блок с тегом [LANG] и содержимым CustomData
void Localize()
{
LANG.Clear();



List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
GridTerminalSystem.SearchBlocksOfName("[Lang]", allBlocks);

if (allBlocks.Count > 0)
{
//Echo("Читаем словарь...");
GetLocalize(allBlocks[0]);
}
//else Echo("Перевод не найден");
}
//руссификация. Если найден блок с тегом [LANG] и секцией LANG в CastomData
void GetLocalize(IMyTerminalBlock bl)
{
string[] custData = bl.CustomData.Substring(bl.CustomData.IndexOf("LANG") + 5).Split('\n');

string subType = "";

for (int i = 0; i < custData.Length; i++)
{
//Echo(subType);
if (custData[i] == "") continue;
if (custData[i].Contains(":"))
{
subType = custData[i].Substring(0, custData[i].Length - 1);
continue;
}
else
{
string[] trans = custData[i].Split(';');
if (trans.Length < 2) continue;
switch (subType)
{
case "Ore":
if(!LANG.ContainsKey(trans[0])) LANG.Add(trans[0], trans[1]);
break;
}
}
}
//Echo("Translation is ready.");
}
//------ конец перевода

IMyTextSurface LCD;
void LCDPrepare()
{
LCD.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
//Vector2 SurfaceSize = LCD.SurfaceSize;

LCD.Font = "Monospace";
//LCD.FontSize = 0.6f;
}
class oreDic
{
public float max = 0, cur = 0;
public string LName = "";
public oreDic(float temp, string tName)
{
this.cur = temp;
LName = System.Text.RegularExpressions.Regex.Replace(tName, "(_)([0-9]).", "");
LName = System.Text.RegularExpressions.Regex.Replace(LName, "Ore", "");
LName = System.Text.RegularExpressions.Regex.Replace(LName, "_CMM", "");
LName = System.Text.RegularExpressions.Regex.Replace(LName, "_", " ");

if (LANG.ContainsKey(LName)) LName = LANG[LName];
}
public void Add(float temp)
{
cur += temp;
if (cur > max) max = cur;
}

}
//------------------------------------------------------------------------------------------------------
class Position
{
public int X, Y, Z;
public Position(int X = 0, int Y = 0, int Z = 0)
{
this.X = X;
this.Y = Y;
this.Z = Z;
}
}
static List<Position> DrillSteps = new List<Position>();
void GetSteps()
{
//Echo("Генерация шагов...");
DrillSteps.Clear();

DrillSteps.Add(new Position());
int maximum = (MaxStepsXZ > 4) ? 4 : MaxStepsXZ;
for (int i = 1; i <= maximum; i++)
{

int x = 0;
int z = i;
DrillSteps.Add(new Position(x++, 0, z));
bool sX = true;
int step = 1;

while (x != 0 || z != i)
{
DrillSteps.Add(new Position(x, 0, z));
if (Math.Abs(x) == Math.Abs(z))
{
sX = !sX;
if (x > 0 && z > 0) step = -1;
if (x < 0 && z < 0) step = 1;
}
if (sX) x += step;
else z += step;
}
}
int ringCount = DrillSteps.Count;
maximum = (MaxStepsY > 4) ? 4 : MaxStepsY;
for (int y = 1; y <= maximum; y++)
{
for (int i = 0; i < ringCount; i++)
{
DrillSteps.Add(new Position(DrillSteps[i].X, -y, DrillSteps[i].Z));
}
if (!DownOnly) //UNREACH
{
for (int i = 0; i < ringCount; i++)
{
DrillSteps.Add(new Position(DrillSteps[i].X, y, DrillSteps[i].Z));
}
}
}
//Echo("Генерация шагов шагов завершена (" + TotalSteps.ToString() + ")");
}
class MyDrill
{
public IMyShipDrill drill;
public int DrIndex;
public DrillStates State = DrillStates.Stop; // текущее состояние бура
public int CurrentStep = 0; //текущий шаг
public int TotalSteps { get { return DrillSteps.Count; } }			
Dictionary<string, oreDic> Ores = new Dictionary<string, oreDic>(); // список руд для копания
public int OresCount { get { return Ores.Count; } }
public float OresAmount
{
get
{
float amount = 0;
foreach (KeyValuePair<string, oreDic> ore in Ores) amount += ore.Value.cur;
return amount;
}
}
public float OresAmountMax
{
get
{
float amount = 0;
foreach (KeyValuePair<string, oreDic> ore in Ores) amount += ore.Value.max;
return amount;
}
}
public float Copmleted
{
get
{
return (OresAmountMax - OresAmount) * 100 / OresAmountMax;
}
}
void updPossibleDrillOres()
{
//Ores.Clear();
foreach (KeyValuePair<string, oreDic> ore in Ores) ore.Value.cur = 0;
List<List<object>> PossibleDrillTargets = drill.GetValue<List<List<object>>>("Drill.PossibleDrillTargets");

foreach (List<object> DrillTarget in PossibleDrillTargets)
{
//List<object> {
//0  TargetVoxelData entityData,
//1  IMyEntity Entity,
//2  double Distance,
//3  MyVoxelMaterialDefinition MaterialDef,
//4  float Amount }
string[] _MaterialDef = DrillTarget[3].ToString().Split('/'); //MyDx11VoxelMaterialDefinition
if (_MaterialDef.Length > 1)
{
String tOre = _MaterialDef[1];
float tAmount = (float)DrillTarget[4];

if (!Ores.ContainsKey(tOre)) Ores.Add(tOre, new oreDic(tAmount, tOre));
else Ores[tOre].Add(tAmount);
}
}
}
float AreaSize = 75f;   // размер зоны бурения
float MaxOffset = 250f; // Маскимальное смещение
System.DateTime LastTime;
public int idling
{
get
{
TimeSpan interval = tLastTime - LastTime;
return interval.Seconds;
}
}
public MyDrill(IMyShipDrill tdrill, int num)
{
drill = tdrill;
DrIndex = num;

if (drill.CubeGrid.GridSizeEnum == MyCubeSize.Small)
{
AreaSize = 25f;
MaxOffset = 84f;
}
Init();
}
float PosToFloat(int pos)
{
float offset = Convert.ToSingle(pos) * AreaSize;
if (pos >= 4) offset = MaxOffset;
if (pos <= -4) offset = -MaxOffset;

return offset;
}
float getOffset(string Direction) { return drill.GetValueFloat("Drill." + AO + Direction); }
void setOffset(Position pos)
{
setOffset(X, PosToFloat(pos.X));
setOffset(Z, PosToFloat(pos.Z));

if (MaxStepsY > 0) setOffset(Y, PosToFloat(pos.Y));
}
void setOffset(string Direction, float toOffset)
{
while (getOffset(Direction) != toOffset)
moveOffset(AO + Direction + ((getOffset(Direction) > toOffset) ? "_Decrease" : "_Increase"));
}
void moveOffset(string Action) { drill.ApplyAction(Action); }
public void ON() { drill.GetActionWithName("OnOff_On").Apply(drill); }
public void OFF() { drill.GetActionWithName("OnOff_Off").Apply(drill); }
public void Init()
{
OFF();
setOffset(new Position(4, 4, 4));
setOffset(new Position(0, 0, 0));
CurrentStep = 0;
State = DrillStates.Stop;
}
public void Start()
{
Ores.Clear();
ON();
State = DrillStates.Waiting;
LastTime = tLastTime;
}
public void Next()
{
CurrentStep++;
Ores.Clear();
if (CurrentStep >= TotalSteps)
{
State = DrillStates.End;
OFF();
return;
}
setOffset(DrillSteps[CurrentStep]);
LastTime = tLastTime;
State = DrillStates.Waiting;
}
public void Stop()
{
State = DrillStates.Stop;
OFF();
}
public bool Check()
{
if (State == DrillStates.End || State == DrillStates.Stop) return false;
updPossibleDrillOres();

if (OresAmount > 0)
{
State = DrillStates.Drilling;
return true;
}

if (State == DrillStates.Drilling)
{
LastTime = tLastTime;
State = DrillStates.Waiting;
}
if (idling > IdleTime) Next();

return true;
}
public string ToString(int ScreenWidth)
{				
int ChartWidth = 0;
if (ScreenWidth >= 30) ChartWidth = ScreenWidth - 25;
if (ChartWidth > 10) ChartWidth = 10;

if (State == DrillStates.Stop || State == DrillStates.End)
return makeStr($"{DrIndex}.{State.ToString()}", "", ScreenWidth); 


if (State == DrillStates.Drilling)
{
string result = makeStr(
$"{DrIndex}.{State.ToString()}",
$"{CurrentStep + 1}/{TotalSteps}" + getChart(CurrentStep, TotalSteps, ChartWidth),
ScreenWidth);
int ind = 1;
foreach (KeyValuePair<string, oreDic> ore in Ores)
{
string oreName = ore.Value.LName;
string digits = Math.Round(ore.Value.cur, 1).ToString();

result += makeStr(
" " + (ind == Ores.Count ? "└" : "├") + oreName,
digits + getChart(ore.Value.cur, ore.Value.max, ChartWidth),
ScreenWidth);
ind++;
}
return result;
}
else if (State == DrillStates.Waiting)
{
return makeStr(
$"{DrIndex}.{State.ToString()} {IdleTime - idling}",
$"{CurrentStep + 1}/{TotalSteps}" + getChart(CurrentStep, TotalSteps, ChartWidth),
ScreenWidth);
}
return "";

}
}
//------------------------------------------------------------------------------------------------------
void SystemInit()
{
Runtime.UpdateFrequency = UpdateFrequency.Update100;
GetSettings();		
Localize();
if (Drills.Count > 0)
{
Drills.Clear();
}


List<IMyShipDrill> allDrils = new List<IMyShipDrill>();
GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(allDrils, b => b.CubeGrid == Me.CubeGrid);

if (allDrils.Count > 0)
{
//Echo("Found: " + allDrils.Count.ToString() + " drills");
foreach (IMyShipDrill drill in allDrils)
{
if (drill.BlockDefinition.SubtypeId.Contains("NanobotDrillSystem"))
Drills.Add(new MyDrill(drill, Drills.Count + 1));
if (Drills.Count >= MaxDrillsCount) break;
}
//Echo("  " + Drills.Count.ToString() + " of them is Nanodrills");
}
else return;

if (Drills.Count > 0)
{
//Runtime.UpdateFrequency = UpdateFrequency.Update10;
SystemState = DrillStates.Stop;
}
else
{
//Runtime.UpdateFrequency = UpdateFrequency.None;
//Echo("NanoDrills not found...");
//SystemState = DrillStates.Error;
return;
}

// экран для вывода инфы про раскопки
//Echo("Поиск LCD ...");
List<IMyTerminalBlock> lcdList = new List<IMyTerminalBlock>();
GridTerminalSystem.SearchBlocksOfName(LCDName, lcdList, b => b.CubeGrid == Me.CubeGrid);
if (lcdList.Count > 0)
{
LCD = lcdList[0] as IMyTextSurface;
LCDPrepare();
//Echo("LCD: " + lcdList[0].CustomName);
}
else
{
//if (Me.CubeGrid.GridSizeEnum == MyCubeSize.Large) LCD = Me.GetSurface(0);
//else
LCD = null;
//Echo("LCD not found. Must have subtitle: " + LCDName);
}
//int SurfaceCount { get; }
}
public Program()
{
Runtime.UpdateFrequency = UpdateFrequency.Update100;
}
void Main(string args)
{            
if (Drills.Count == 0)
{
try
{
SystemInit();
}
finally{
Echo("Script is initializing...");
}
return;
}

tLastTime = System.DateTime.UtcNow;
string cmd = args.ToUpper();
if (cmd.StartsWith("START"))
{
Start();
}
else if (cmd.StartsWith("RESTART"))
{
SystemInit();
Start();
return;
}
else if (cmd.StartsWith("STOP"))
{
Stop();
}
MyEcho();
if (TIK++ < 5) return;            
TIK = 0;
if (SystemState == DrillStates.Drilling)
{
int work = 0;
foreach (MyDrill drill in Drills)
{
drill.Check();
if (drill.State == DrillStates.Drilling || drill.State == DrillStates.Waiting) work++;
}
if (work == 0) SystemState = DrillStates.End;
}			
TextOut();

}
void Start()
{
Runtime.UpdateFrequency = UpdateFrequency.Update10; 
foreach (MyDrill drill in Drills)
drill.Start();
SystemState = DrillStates.Drilling;
}
void Stop()
{
Runtime.UpdateFrequency = UpdateFrequency.Update100; 
foreach (MyDrill drill in Drills)
drill.Stop();
SystemState = DrillStates.Stop;
}

void MyEcho()
{
Echo("STATUS: " + SystemState.ToString().ToUpper());
int tSteps = 0;
int cSteps = 0;
foreach (MyDrill drill in Drills)
{
tSteps += drill.TotalSteps;
cSteps += drill.CurrentStep;
}
Echo($"{Drills.Count} NanobotDrill(s) {Math.Round(cSteps * 100f / tSteps, 1)}%");

switch (SystemState)
{
case DrillStates.Stop:
Echo($"System is ready for drilling.");
Echo("Run Scrip with cmd 'START' or 'RESTART'");
break;
case DrillStates.End:
Echo("Drilling is over.");
Echo("Run Scrip with cmd 'RESTART'");
break;
}
}
void TextOut()
{
if (LCD != null)
{
LCD.WriteText("STATUS: " + SystemState.ToString().ToUpper() + "\n");
int ScreenWidth = (int)Math.Round(25.2f / LCD.FontSize);

foreach (MyDrill drill in Drills)
LCD.WriteText(drill.ToString(ScreenWidth), true);
}
}
//--------------------------------------------------------------------------------------------------
public static string getChart(double k1, double k2, int size, bool inv = false)
{
if (size < 4) return "";
if (k2 == 0) return "".PadRight(size, '░');

int val = Convert.ToInt32((inv ? k2 - k1 : k1) * Convert.ToDouble(size) / k2);
if (val > size) val = size;

string res = "".PadRight(val, '▓');
res = res.PadRight(size, '░');
res = " " + res;
return res;
}
public static string getChart(int k1, int k2, int size, bool inv = false)
{
return getChart(Convert.ToDouble(k1), Convert.ToDouble(k2), size, inv);
}
public static string makeStr(string str1, string str2, int chars, char symb = '─')
{
int str1Length = str1.Length;
int str2Length = str2.Length;

if (chars < str1Length + str2Length)
{
str1Length = chars - str2Length - 2;
if (str1Length < 0) str1Length = 0;
str1 = str1.Substring(0, str1Length - 2) + "..";
}
int padding = chars - str2Length;
if (str2Length > 0) padding--;

return (str1 + " ").PadRight(padding, symb) + " " + str2 + "\n";
}
//--------------------------------------------------------------------------------------------------
