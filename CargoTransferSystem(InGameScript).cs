static readonly string VERSION = "Version: 1.71";
#region "Program"
static Program program;
class ControlReader
{
IMyShipController _controller = null;
bool _pressedQ = false;
bool _pressedE = false;
bool _pressedA = false;
bool _pressedD = false;
// bool _pressedS = false;
// bool _pressedW = false;
bool _pressedC = false;
bool _pressedSpace = false;
public event InputChangedHandler inputChangedArgs;
public ControlReader(IMyShipController controller)
{ _controller = controller; }
public void ReadInput()
{
if (!_controller.IsUnderControl)
return;
InputChangedEventArgs arg = new InputChangedEventArgs();
if (!_pressedE && _controller.RollIndicator > 0)
{ _pressedE = true; arg.Key = PressedKey.E; inputChangedArgs?.Invoke(this, arg); return; }
if (!_pressedQ && _controller.RollIndicator < 0)
{ _pressedQ = true; arg.Key = PressedKey.Q; inputChangedArgs?.Invoke(this, arg); return; }
if (Math.Abs(_controller.RollIndicator) < 0.01)
{ _pressedE = false; _pressedQ = false; }
if (!_pressedD && _controller.MoveIndicator.X > 0)
{ _pressedD = true; arg.Key = PressedKey.D; inputChangedArgs?.Invoke(this, arg); return; }
if (!_pressedA && _controller.MoveIndicator.X < 0)
{ _pressedA = true; arg.Key = PressedKey.A; inputChangedArgs?.Invoke(this, arg); return; }
if (Math.Abs(_controller.MoveIndicator.X) < 0.01)
{ _pressedD = false; _pressedA = false; }
if (_controller.MoveIndicator.Z < 0) // !_pressedW &&
{ arg.Key = PressedKey.W; inputChangedArgs?.Invoke(this, arg); return; } // _pressedW = true;
if (_controller.MoveIndicator.Z > 0)//!_pressedS &&
{ arg.Key = PressedKey.S; inputChangedArgs?.Invoke(this, arg); return; }//_pressedS = true;
//if (Math.Abs(_controller.MoveIndicator.Z) < 0.01)
//{ _pressedS = false; _pressedW = false; }
if (!_pressedC && _controller.MoveIndicator.Y < 0)
{ _pressedC = true; arg.Key = PressedKey.C; inputChangedArgs?.Invoke(this, arg); return; }
if (!_pressedSpace && _controller.MoveIndicator.Y > 0)
{ _pressedSpace = true; arg.Key = PressedKey.Space; inputChangedArgs?.Invoke(this, arg); return; }
if (Math.Abs(_controller.MoveIndicator.Y) < 0.01)
{ _pressedC = false; _pressedSpace = false; }
}
}
delegate void InputChangedHandler(object sender, InputChangedEventArgs e);
enum PressedKey { Q, E, A, D, S, W, C, Space };
class InputChangedEventArgs : EventArgs { public PressedKey Key; }
enum ConveyorSystemSize { Small, Big };
#endregion
class Cargo
{
public ConveyorSystemSize Size;
public IMyTerminalBlock Container;
public Cargo(ConveyorSystemSize size, IMyTerminalBlock container)
{ Size = size; Container = container; }
}
static MyFixedPoint GetSpareSpace(IMyTerminalBlock container)
{ return container.GetInventory().MaxVolume - container.GetInventory().CurrentVolume; }
class OrderPosition
{
public string[] NameWords;
public int Amount, Moved;
public int Left { get { return Amount - Moved; } }
public bool Done { get { return Amount - Moved <= 0; } }
public OrderPosition(string[] name, int amount)
{ NameWords = name; Amount = amount; Moved = 0; }
public override string ToString()
{
StringBuilder sb = new StringBuilder();
foreach (string name in NameWords)
sb.Append(name + " ");
sb.Append("- " + Moved.ToString() + '/' + Amount.ToString());
return sb.ToString();
}
}
enum TypeFilter
{ All, Ore, Ingots, Components, Tools, Ammo };
class Order
{
public long CustomerID { get; private set; }
private string
customerName = "Operator",
orderArgument;
public long ConnectorID { get; private set; }
int connecotrNum = 0;
public int ConnectorNum { get { return connecotrNum; } }
public bool Unload { get; private set; } = false;
public bool CheckNames { get; private set; } = false;
public bool GetConnectorByID { get; private set; } = false;
public List<OrderPosition>
PositionsList = new List<OrderPosition>(),
CompletedList = new List<OrderPosition>();
TypeFilter Filter = TypeFilter.All;
// 1734867183641, gather 7127124942 "steel socks, slime rocks"
// 1734867183641, gather 7127124942
// 1734867183641, gather 7127124942 components
//by IGI
public bool TryParseOrder(long customerID, string argument)
{
customerName = customerID + " ID";
CustomerID = customerID;
GetConnectorByID = true;
orderArgument = argument;
return ParseArgument(argument);
}
//by operator
public bool TryParseOrder(string argument)
{
CustomerID = 0;
GetConnectorByID = false;
orderArgument = argument;
if (ParseArgument(argument))
{
connecotrNum--;
return true;
}
return false;
}
public Order() { }
public Order(int connetorNum, TypeFilter filter, bool unload)
{
connecotrNum = connetorNum;
Filter = filter;
Unload = unload;
orderArgument = unload ? "UNLOAD " : "GATHER " + connetorNum + " " + filter.ToString();
}
bool ParseArgument(string argument)
{
string[] words = argument.Split(' ');
switch (words[0].Trim().ToUpper())
{
case "GATHER": Unload = false; break;
case "UNLOAD": Unload = true; break;
default: return false;
}
if (GetConnectorByID)
{
long connectorID;
if (!long.TryParse(words[1], out connectorID))
return false;
ConnectorID = connectorID;
if (!cargoTransferSystem.GetNumFromIdDictionary(connectorID, out connecotrNum))
return false;
}
else
{
int connectorNum;
if (!int.TryParse(words[1], out connectorNum))
return false;
connecotrNum = connectorNum;
}
string[] NamesData = argument.Split('"');
if (NamesData.Length > 1)
{
CheckNames = true;
ParseSubTypes(NamesData[1]);
}
else
{
CheckNames = false;
if (words.Length > 2)
TryParseFilter(words[2]);
else
Filter = TypeFilter.All;
}
return true;
}
void ParseSubTypes(string argument)
{
PositionsList.Clear();
string[] namePairs = argument.Split(',');
for (int i = 0; i < namePairs.Length; i++)
{
string[] orderItem = namePairs[i].Trim().Split('-');
string[] nameSubString = orderItem[0].Split(' ');
for (int p = 0; p < nameSubString.Length; p++)
nameSubString[p] = nameSubString[p].Trim().ToUpper();
int amount = int.MaxValue;
if (orderItem.Length > 1)
if (!int.TryParse(orderItem[1], out amount))
amount = int.MaxValue;
PositionsList.Add(new OrderPosition(nameSubString, amount));
}
}
public int CheckItemType(MyItemType itemType, ref StringBuilder message)
{
var itemInfo = itemType.GetItemInfo();
switch (Filter)
{
case TypeFilter.All:
return 0;
case TypeFilter.Ammo:
if (itemInfo.IsAmmo)
return 0;
break;
case TypeFilter.Components:
if (itemInfo.IsComponent)
return 0;
break;
case TypeFilter.Ingots:
if (itemInfo.IsIngot)
{
message.Append(" ingot");
return 0;
}
break;
case TypeFilter.Ore:
if (itemInfo.IsOre)
{
message.Append(" ore");
return 0;
}
break;
case TypeFilter.Tools:
if (itemInfo.IsTool)
return 0;
break;
}
return -1;
}
public int CheckItemSubtypeID(MyItemType itemType, ref StringBuilder message)
{
for (int i = 0; i < PositionsList.Count; i++)
{
OrderPosition item = PositionsList[i];
string subtypeid = itemType.SubtypeId.ToUpper();
bool success = subtypeid.Contains(item.NameWords[0]);
for (int n = 1; n < item.NameWords.Length && success; n++)
success &= subtypeid.Contains(item.NameWords[n]);
if (success)
return i;
}
return -1;
}
bool TryParseFilter(string str)
{
switch (str.Trim().ToUpper())
{
case "ALL":
Filter = TypeFilter.All;
break;
case "AMMO":
Filter = TypeFilter.Ammo;
break;
case "COMPONENTS":
Filter = TypeFilter.Components;
break;
case "INGOTS":
Filter = TypeFilter.Ingots;
break;
case "ORE":
Filter = TypeFilter.Ore;
break;
case "TOOLS":
Filter = TypeFilter.Tools;
break;
default:
return false;
}
return true;
}
public void AddToListAsStrings(ref List<string> strList)
{
strList.Add("   Customer: " + customerName);
strList.Add(orderArgument);
}
}
class CargoTransferSystem
{
#region "Fields"
enum SystemState
{
Idle,
Running,
Stop,
Startup,
Transfer,
ShutdownLogo,
Error
};
SystemState state = SystemState.Stop;
enum StartupState
{
GatherContainers,
RemoveDecor,
RemoveConetrollersTrash,
GatherOtherContainers,
GatherConnectors,
RemoveEjectors,
Ready,
ReadData,
MakeSprites
};
StartupState startupState = StartupState.ReadData;
enum TransferState
{
Prepear,
UnloadByType,
UnloadByName,
GatherByType,
GatherByName,
Report,
GetOpContainers,
GetOpAllContainers,
AfterGatherCheck,
Idle,
SortConveyorSystems,
Table
};
TransferState transferState = TransferState.Idle;
enum MessageBoxResult { IterruptTransfer, RemoveOrder };
MessageBoxResult messageBoxResult = MessageBoxResult.IterruptTransfer;
IMyUnicastListener USI = program.IGC.UnicastListener;
const string FONT = "Debug";
const float
FONT_SIZE = 0.45f,
HEADER_SIZE = 0.65f,
BORDER_SIZE = 2f;
int
orderPosition,
mySContainer = 0,
myLContainer = 0,
opSContainer = 0,
opLContainer = 0,
item = 0,
itemCounter = 0,
menu = 0,
connectorNum = 0,
surfaceNumber = 0,
filterInt = 0,
filterSlider = 0,
logSlider = 0,
orderListSlider = 0,
maxContentlines = 0,
block = 0,
gather = 6,
MaxCharsOnSurface = 0;
float
GUIScale = 1.0f,
fontSize = FONT_SIZE,
strHalf = 0,
headerFontSize = HEADER_SIZE,
borderSize = BORDER_SIZE,
progressBarLength = 0,
pBLH = 0;
bool
passBulky = false,
unload = false,
showMessageBox = false;
IMyShipController controller;
public IMyShipController Controller { get { return controller; } }
IMyTextSurface surface;
string
controllerName = "Cockpit",
surfaceProviderName = "Cockpit";
string[] filterNames = { "1 All", "2 Ore", "3 Ingots", "4 Components", "5 Tools", "6 Ammo" };
Vector2
surfaceAdjust = new Vector2(),
surfaceHalf = new Vector2(),
headerPosition = new Vector2();
MySprite[]
headerSprites = new MySprite[3],
statusBorderSprites = new MySprite[2],
contentBorderSprites = new MySprite[2],
standbySprites = new MySprite[4],
connectorMenuSprites = new MySprite[10],
commandsMenuSprites = new MySprite[4],
commandsButtonSprites = new MySprite[3],
connectorsButtonSprites = new MySprite[3],
logButtonSprites = new MySprite[3],
ordersButtonSprites = new MySprite[3],
progressBarSprites = new MySprite[3],
headerProgressBarSprites = new MySprite[3],
filterSprites = new MySprite[1],
contentSprites = new MySprite[1],
messageBox = new MySprite[10];
List<string>
logLines = new List<string>(),
ordersList = new List<string>();
List<Color> logColors = new List<Color>();
StringBuilder message = new StringBuilder();
string lastMessage = "";
MySprite patternLeft, patternRight, patternCenter;
List<IMyShipConnector> connectors = new List<IMyShipConnector>();
Dictionary<long, int> connectorsNumsFomId = new Dictionary<long, int>();
IMyInventory myInventory = null;
List<IMyTerminalBlock>
myContainers = new List<IMyTerminalBlock>(),
opContainers = new List<IMyTerminalBlock>(),
unsortedBlocks = new List<IMyTerminalBlock>();
List<Order> orders = new List<Order>();
Order currentOrder { get { return orders[0]; } }
List<Cargo>
mySCS = new List<Cargo>(),
myLCS = new List<Cargo>(),
opSCS = new List<Cargo>(),
opLCS = new List<Cargo>();
IMyInventory opInventory = null;
Color[] buttonsColor;
Color
colorBlack = new Color(0, 0, 0, 255),
colorBorders = new Color(110, 114, 120, 255),
colorBackground = new Color(37, 46, 53, 255),
colorText = new Color(203, 226, 233, 255),
colorHighlighted = new Color(146, 205, 218, 255),
colorButton = new Color(66, 75, 82, 255),
colorButtonText = new Color(203, 226, 233, 255),
colorInactive = new Color(33, 40, 45, 255),
colorSuccess = new Color(75, 215, 25, 255),
colorWarning = new Color(255, 255, 25, 255),
colorError = new Color(215, 65, 25, 255),
colorShadow = new Color(0, 0, 0, 125);
#endregion
#region "Initializers"
public CargoTransferSystem()
{
ReadData();
InitializeSystems();
program.Me.GetSurface(0).WriteText("", false);
}
void GetFirstWithName<T>(ref T block, string name) where T : class, IMyTerminalBlock
{
List<T> units = new List<T>();
program.GridTerminalSystem.GetBlocksOfType(units);
foreach (T unit in units)
if (unit.CubeGrid == program.Me.CubeGrid && unit.CustomName.Contains(name))
{
block = unit;
return;
}
}
bool GetContainersFromUnsorted(ref List<IMyTerminalBlock> containers, IMyTerminalBlock target)
{
for (int i = 0; i < 5 && block < unsortedBlocks.Count; i++, block++)
if (unsortedBlocks[block].IsSameConstructAs(target) && unsortedBlocks[block].HasInventory)
containers.Add(unsortedBlocks[block]);
if (block < unsortedBlocks.Count)
return false;
block = 0;
return true;
}
bool GetContainersFromUnsorted(ref List<IMyTerminalBlock> containers, IMyTerminalBlock target, string line)
{
for (int i = 0; i < 5 && block < unsortedBlocks.Count; i++, block++)
if (unsortedBlocks[block].IsSameConstructAs(target) &&
unsortedBlocks[block].HasInventory &&
unsortedBlocks[block].BlockDefinition.SubtypeName.ToLower().Contains(line))
containers.Add(unsortedBlocks[block]);
if (block < unsortedBlocks.Count)
return false;
block = 0;
return true;
}
void InitDictionary()
{
for (int i = 0; i < connectors.Count; i++)
connectorsNumsFomId.Add(connectors[i].GetId(), i);
}
bool GetConnectorsFromUnsorted(ref List<IMyTerminalBlock> containers, ref List<IMyShipConnector> connectors, IMyTerminalBlock target)
{
for (int i = 0; i < 5 && block < unsortedBlocks.Count; i++, block++)
if (unsortedBlocks[block].IsSameConstructAs(target) &&
unsortedBlocks[block].HasInventory &&
!unsortedBlocks[block].BlockDefinition.SubtypeName.ToLower().Contains("connectorsmall"))
{
connectors.Add(unsortedBlocks[block] as IMyShipConnector);
containers.Add(unsortedBlocks[block]);
}
if (block < unsortedBlocks.Count)
return false;
block = 0;
return true;
}
bool GetOpponentContainersFromUnsorted()
{
for (int i = 0; i < 5 && block < unsortedBlocks.Count; i++, block++)
if (unsortedBlocks[block].IsSameConstructAs(connectors[currentOrder.ConnectorNum].OtherConnector)
&& unsortedBlocks[block].HasInventory)
opContainers.Add(unsortedBlocks[block]);
if (block < unsortedBlocks.Count)
return false;
block = 0;
return true;
}
bool GatherContainers(ref List<IMyTerminalBlock> containers, ref List<IMyShipConnector> connectors, IMyTerminalBlock target)
{
float A = unsortedBlocks.Count == 0 ? 0 : (float)block / unsortedBlocks.Count;
switch (gather)
{
case 1:
UppdateProgressBar(5f + A, 6);
if (GetContainersFromUnsorted(ref containers, target, "container"))
return true;
break;
case 2:
UppdateProgressBar(4f + A, 6);
if (GetConnectorsFromUnsorted(ref containers, ref connectors, target))
FillUnsortedWith<IMyCargoContainer>();
break;
case 3:
UppdateProgressBar(3f + A, 6);
if (GetContainersFromUnsorted(ref containers, target, "cockpit"))
FillUnsortedWith<IMyShipConnector>();
break;
case 4:
UppdateProgressBar(2f + A, 6);
if (GetContainersFromUnsorted(ref containers, target))
FillUnsortedWith<IMyCockpit>();
break;
case 5:
UppdateProgressBar(1f + A, 6);
if (GetContainersFromUnsorted(ref containers, target))
FillUnsortedWith<IMyShipDrill>();
break;
case 6:
UppdateProgressBar(0f + A, 6);
if (GetContainersFromUnsorted(ref containers, target))
FillUnsortedWith<IMyShipGrinder>();
break;
}
return false;
}
Color ColorFromWords(string[] words)
{
Color returnValue = new Vector3();
try
{ returnValue = new Color(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4])); }
catch { return new Color(); }
return returnValue;
}
string ColorToString(Color color)
{
return color.R.ToString() + ": " + color.G.ToString() + ": " + color.B.ToString() + ": " + color.A.ToString();
}
void ReadData()
{
if (string.IsNullOrWhiteSpace(program.Me.CustomData))
SaveData();
string[] Lines = program.Me.CustomData.Split('\n');
string[] Words;
try
{
Words = Lines[0].Split(':'); controllerName = Words[1].Trim();
Words = Lines[1].Split(':'); surfaceProviderName = Words[1].Trim();
Words = Lines[2].Split(':'); surfaceNumber = int.Parse(Words[1]);
if (surfaceNumber != 0) surfaceNumber--;
Words = Lines[3].Split(':'); GUIScale = float.Parse(Words[1]);
Words = Lines[4].Split(':'); colorBorders = ColorFromWords(Words);
Words = Lines[5].Split(':'); colorBackground = ColorFromWords(Words);
Words = Lines[6].Split(':'); colorText = ColorFromWords(Words);
Words = Lines[7].Split(':'); colorHighlighted = ColorFromWords(Words);
Words = Lines[8].Split(':'); colorButton = ColorFromWords(Words);
Words = Lines[9].Split(':'); colorButtonText = ColorFromWords(Words);
Words = Lines[10].Split(':'); colorInactive = ColorFromWords(Words);
Words = Lines[11].Split(':'); colorSuccess = ColorFromWords(Words);
Words = Lines[12].Split(':'); colorWarning = ColorFromWords(Words);
Words = Lines[13].Split(':'); colorError = ColorFromWords(Words);
}
catch { SaveData(); }
}
void SaveData()
{
StringBuilder sb = new StringBuilder("");
sb.Append("Controller name: " + controllerName + "\n");
sb.Append("Surface provider name: " + surfaceProviderName + "\n");
sb.Append("Surface number: " + surfaceNumber + "\n");
sb.Append("GIU Scale: " + GUIScale + "\n");
sb.Append("Borders Color: " + ColorToString(colorBorders) + "\n");
sb.Append("Background Color: " + ColorToString(colorBackground) + "\n");
sb.Append("Text Color: " + ColorToString(colorText) + "\n");
sb.Append("Highlighted Color: " + ColorToString(colorHighlighted) + "\n");
sb.Append("Button Color: " + ColorToString(colorButton) + "\n");
sb.Append("Button Text Color: " + ColorToString(colorButtonText) + "\n");
sb.Append("Inactive Color: " + ColorToString(colorInactive) + "\n");
sb.Append("Success Color: " + ColorToString(colorSuccess) + "\n");
sb.Append("Issue Color: " + ColorToString(colorWarning) + "\n");
sb.Append("Error Color: " + ColorToString(colorError) + "\n");
program.Me.CustomData = sb.ToString();
}
void InitializeSystems()
{
GetFirstWithName(ref controller, controllerName);
if (controller == null)
throw new Exception("Controller not found.");
IMyTerminalBlock surfaceProvider = null;
GetFirstWithName(ref surfaceProvider, surfaceProviderName);
if (surfaceProvider == null)
throw new Exception("Surface provider not found.");
IMyTextSurfaceProvider TSP = surfaceProvider as IMyTextSurfaceProvider;
if (TSP.SurfaceCount > 0)
{
surfaceNumber %= TSP.SurfaceCount;
surface = TSP.GetSurface(surfaceNumber);
surfaceAdjust = (surface.TextureSize - surface.SurfaceSize) * 0.5f;
surfaceHalf = surface.SurfaceSize * 0.5f;
surface.ContentType = ContentType.SCRIPT;
surface.Script = "";
surface.ScriptBackgroundColor = colorBlack;
}
else throw new Exception("Surface provider have no surfaces.");
}
#endregion
#region "Make sprites"
void MakeStandBySprites()
{
Vector2 position = surfaceAdjust;
position.Y += surfaceHalf.Y;
position.X += surfaceHalf.X;
Vector2 size = surface.SurfaceSize - 10 * GUIScale;
standbySprites[0] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, size, colorBorders);
size -= borderSize;
standbySprites[1] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, size, colorBackground);
StringBuilder sb = new StringBuilder("Cargo Transfer System");
Vector2 strSize = surface.MeasureStringInPixels(sb, FONT, headerFontSize);
standbySprites[2] = MySprite.CreateText("Cargo Transfer System", FONT, colorText, headerFontSize, TextAlignment.CENTER);
standbySprites[2].Position = new Vector2(position.X, position.Y - surfaceHalf.Y + 32 * GUIScale - strSize.Y * 0.5f);
standbySprites[3] = MySprite.CreateText("", FONT, colorText, headerFontSize, TextAlignment.CENTER);
standbySprites[3].Position = new Vector2(position.X, position.Y + surfaceHalf.Y - 32 * GUIScale - strSize.Y * 0.5f);
}
void MakeHeaderSprites()
{
headerPosition = surfaceAdjust;
headerPosition.Y += 20 * GUIScale;
headerPosition.X += surfaceHalf.X;
Vector2 size = surface.SurfaceSize - 10 * GUIScale;
headerSprites[0] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
headerPosition, new Vector2(size.X, 30 * GUIScale), colorBorders);
headerSprites[1] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
headerPosition, new Vector2(size.X - borderSize, 30 * GUIScale - borderSize), colorBackground);
StringBuilder sb = new StringBuilder(program.Me.CubeGrid.CustomName + " [" + program.Me.GetOwnerFactionTag() + "]");
Vector2 strSize = surface.MeasureStringInPixels(sb, FONT, headerFontSize);
headerSprites[2] = MySprite.CreateText(sb.ToString(), FONT, colorText, headerFontSize, TextAlignment.CENTER);
headerSprites[2].Position = new Vector2(headerPosition.X, headerPosition.Y - strSize.Y * 0.5f);
}
void MakeStatusBorderSprites()
{
Vector2 position = surfaceAdjust;
position.Y += 52 * GUIScale;
position.X += surfaceHalf.X;
Vector2 size = surface.SurfaceSize - 10 * GUIScale;
statusBorderSprites[0] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(size.X, 24 * GUIScale), colorBorders);
statusBorderSprites[1] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(size.X - borderSize, 24 * GUIScale - borderSize), colorBackground);
}
void MakeContentBorderSprites()
{
Vector2 position = surfaceAdjust;
float freespace = (surface.SurfaceSize.Y - 64f * GUIScale);
position.Y += surface.SurfaceSize.Y - freespace * 0.5f;
position.X += surfaceHalf.X;
Vector2 size = surface.SurfaceSize - 10 * GUIScale;
size.Y = freespace - 10 * GUIScale;
contentBorderSprites[0] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, size, colorBorders);
size -= borderSize;
contentBorderSprites[1] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, size, colorBackground);
StringBuilder sb = new StringBuilder("Test");
Vector2 strSize = surface.MeasureStringInPixels(sb, FONT, fontSize);
maxContentlines = (int)Math.Floor((freespace - 14) / (strSize.Y + 2));
}
void MakeButtonsSprites()
{
StringBuilder sb = new StringBuilder("connectors");
Vector2
position = surfaceAdjust,
strSize = surface.MeasureStringInPixels(sb, FONT, fontSize),
size = new Vector2(strSize.X + 4, 16 * GUIScale),
borders = size - borderSize;
position.Y += 52 * GUIScale;
float
textHeightPos = position.Y - strHalf,
Xoffset = (surface.SurfaceSize.X - surfaceAdjust.X - 10 * GUIScale - borderSize * 2) * 0.25f;
MySprite
buttonTextPattern = MySprite.CreateText("null", FONT, colorButtonText, fontSize, TextAlignment.CENTER),
buttonBordersPattern = new MySprite(SpriteType.TEXTURE, "SquareSimple", position, size, colorBorders),
buttonBackgroundPattern = new MySprite(SpriteType.TEXTURE, "SquareSimple", position, borders, colorInactive);
// connectors button
position.X += Xoffset * 0.5f + 5 * GUIScale + borderSize;
connectorsButtonSprites[0] = buttonBordersPattern;
connectorsButtonSprites[0].Position = position;
connectorsButtonSprites[1] = buttonBackgroundPattern;
connectorsButtonSprites[1].Position = position;
connectorsButtonSprites[2] = buttonTextPattern;
connectorsButtonSprites[2].Data = sb.ToString();
connectorsButtonSprites[2].Position = new Vector2(position.X, textHeightPos);
// commands button
sb.Clear(); sb.Append("commands");
position.X += Xoffset;
commandsButtonSprites[0] = buttonBordersPattern;
commandsButtonSprites[0].Position = position;
commandsButtonSprites[1] = buttonBackgroundPattern;
commandsButtonSprites[1].Position = position;
commandsButtonSprites[2] = buttonTextPattern;
commandsButtonSprites[2].Data = sb.ToString();
commandsButtonSprites[2].Position = new Vector2(position.X, textHeightPos);
// log button
sb.Clear(); sb.Append("log");
position.X += Xoffset;
logButtonSprites[0] = buttonBordersPattern;
logButtonSprites[0].Position = position;
logButtonSprites[1] = buttonBackgroundPattern;
logButtonSprites[1].Position = position;
logButtonSprites[2] = buttonTextPattern;
logButtonSprites[2].Data = sb.ToString();
logButtonSprites[2].Position = new Vector2(position.X, textHeightPos);
// order list button
sb.Clear(); sb.Append("orders");
position.X += Xoffset;
ordersButtonSprites[0] = buttonBordersPattern;
ordersButtonSprites[0].Position = position;
ordersButtonSprites[1] = buttonBackgroundPattern;
ordersButtonSprites[1].Position = position;
ordersButtonSprites[2] = buttonTextPattern;
ordersButtonSprites[2].Data = sb.ToString();
ordersButtonSprites[2].Position = new Vector2(position.X, textHeightPos);
}
void MakeFilterListSprites()
{
Vector2 position = new Vector2(10, 72);
position *= GUIScale;
position += surfaceAdjust;
StringBuilder sb = new StringBuilder("Test");
Vector2 strSize = surface.MeasureStringInPixels(sb, FONT, fontSize);
float step = strSize.Y + 2;
position.Y += step * 2;
int maxLines = maxContentlines - 2 >= filterNames.Length ? filterNames.Length : maxContentlines - 2;
filterSprites = new MySprite[maxLines];
for (int i = 0; i < maxLines; i++)
{
MySprite line = patternLeft;
line.Position = new Vector2(position.X, position.Y);
filterSprites[i] = line;
position.Y += step;
}
UppdateFilterList();
}
void MakeCommandsMenuSprites()
{
Vector2 position = new Vector2(10, 72);
position *= GUIScale;
position += surfaceAdjust;
Vector2 positionRight = position;
positionRight.X = surfaceAdjust.X + surface.SurfaceSize.X - 10 * GUIScale;
StringBuilder sb = new StringBuilder("Connector: ");
Vector2 strSize = surface.MeasureStringInPixels(sb, FONT, fontSize);
float line = strSize.Y + 2;
commandsMenuSprites[0] = patternLeft;
commandsMenuSprites[0].Data = sb.ToString();
commandsMenuSprites[0].Position = new Vector2(position.X, position.Y);
sb.Clear(); sb.Append(connectors[connectorNum].CustomName);
commandsMenuSprites[1] = patternRight;
commandsMenuSprites[1].Data = sb.ToString();
commandsMenuSprites[1].Position = new Vector2(positionRight.X, positionRight.Y);
positionRight.Y = position.Y += line;
sb.Clear(); sb.Append("Transfer state: ");
commandsMenuSprites[2] = patternLeft;
commandsMenuSprites[2].Data = sb.ToString();
commandsMenuSprites[2].Position = new Vector2(position.X, position.Y);
sb.Clear(); sb.Append("Unload");
commandsMenuSprites[3] = patternRight;
commandsMenuSprites[3].Data = sb.ToString();
commandsMenuSprites[3].Color = colorWarning;
commandsMenuSprites[3].Position = new Vector2(positionRight.X, positionRight.Y);
}
void MakeConnectorInfoSprites()
{
Vector2 position = new Vector2(10, 72);
position *= GUIScale;
position += surfaceAdjust;
Vector2 positionRight = position;
positionRight.X = surfaceAdjust.X + surface.SurfaceSize.X - 10 * GUIScale;
StringBuilder sb = new StringBuilder("Connector:");
Vector2 strSize = surface.MeasureStringInPixels(sb, FONT, fontSize);
float line = strSize.Y + 2;
connectorMenuSprites[0] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.LEFT);
connectorMenuSprites[0].Position = new Vector2(position.X, position.Y);
sb.Clear(); sb.Append("");
connectorMenuSprites[1] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.RIGHT);
connectorMenuSprites[1].Position = new Vector2(positionRight.X, positionRight.Y);
positionRight.Y = position.Y += line;
sb.Clear(); sb.Append("Status: ");
connectorMenuSprites[2] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.LEFT);
connectorMenuSprites[2].Position = new Vector2(position.X, position.Y);
sb.Clear(); sb.Append("");
connectorMenuSprites[3] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.RIGHT);
connectorMenuSprites[3].Position = new Vector2(positionRight.X, positionRight.Y);
//
sb.Clear(); sb.Append("Connected with: ");
positionRight.Y = position.Y += line;
connectorMenuSprites[4] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.LEFT);
connectorMenuSprites[4].Position = new Vector2(position.X, position.Y);
sb.Clear(); sb.Append("");
connectorMenuSprites[5] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.RIGHT);
connectorMenuSprites[5].Position = new Vector2(positionRight.X, positionRight.Y);
sb.Clear(); sb.Append("Name: ");
positionRight.Y = position.Y += line;
connectorMenuSprites[6] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.LEFT);
connectorMenuSprites[6].Position = new Vector2(position.X, position.Y);
sb.Clear(); sb.Append("");
connectorMenuSprites[7] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.RIGHT);
connectorMenuSprites[7].Position = new Vector2(positionRight.X, positionRight.Y);
sb.Clear(); sb.Append("Faction: ");
positionRight.Y = position.Y += line;
connectorMenuSprites[8] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.LEFT);
connectorMenuSprites[8].Position = new Vector2(position.X, position.Y);
sb.Clear(); sb.Append("");
connectorMenuSprites[9] = MySprite.CreateText(sb.ToString(), FONT, colorText, fontSize, TextAlignment.RIGHT);
connectorMenuSprites[9].Position = new Vector2(positionRight.X, positionRight.Y);
}
void MakeProgressBarSprites()
{
Vector2 position = surfaceAdjust + surface.SurfaceSize * 0.5f;
progressBarLength = surface.SurfaceSize.X - 36 * GUIScale;
pBLH = progressBarLength * 0.5f;
Vector2 size = new Vector2(surface.SurfaceSize.X - 32 * GUIScale, 26 * GUIScale);
progressBarSprites[0] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, size, colorBorders);
size -= borderSize;
progressBarSprites[1] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, size, colorBackground);
size -= borderSize;
progressBarSprites[2] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(progressBarLength, size.Y), colorButton);
}
void MakeHeaderProgressBarSprites()
{
Vector2 position = surfaceAdjust + surface.SurfaceSize * 0.5f;
progressBarLength = surface.SurfaceSize.X - 36 * GUIScale;
pBLH = progressBarLength * 0.5f;
position.Y = headerPosition.Y;
Vector2 size = new Vector2(surface.SurfaceSize.X - 32 * GUIScale, 26 * GUIScale);
headerProgressBarSprites[0] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, size, colorBorders);
size -= borderSize;
headerProgressBarSprites[1] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, size, colorBackground);
size -= borderSize;
headerProgressBarSprites[2] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(progressBarLength, size.Y), colorButton);
}
void MakeLogListSprites()
{
Vector2 position = new Vector2(10, 72);
position *= GUIScale;
position += surfaceAdjust;
StringBuilder sb = new StringBuilder("Test");
Vector2 strSize = surface.MeasureStringInPixels(sb, FONT, fontSize);
float step = strSize.Y + 2;
contentSprites = new MySprite[maxContentlines];
for (int i = 0; i < maxContentlines; i++)
{
MySprite line = patternLeft;
line.Position = new Vector2(position.X, position.Y);
contentSprites[i] = line;
position.Y += step;
}
}
void MakeMessageBoxSprites()
{
StringBuilder sb = new StringBuilder("Test");
//borders
Vector2 position = surfaceAdjust + surfaceHalf;
messageBox[1] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(192 * GUIScale, 36 * GUIScale), colorBorders);
messageBox[2] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(188 * GUIScale, 32 * GUIScale), colorBackground);
position.Y -= 7 * GUIScale;
//Message text
messageBox[3] = patternCenter;
messageBox[3].Data = "Test";
messageBox[3].Position = new Vector2(position.X, position.Y - strHalf * GUIScale);
//left button
position.Y += 13 * GUIScale;
position.X -= 68 * GUIScale;
messageBox[4] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(48 * GUIScale, 14 * GUIScale), colorBorders);
messageBox[4].Position = position;
messageBox[5] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(46 * GUIScale, 12 * GUIScale), colorBackground);
messageBox[5].Position = position;
messageBox[6] = patternCenter;
messageBox[6].Data = "No (Q)";
messageBox[6].Position = new Vector2(position.X, position.Y - strHalf);
//Right button
position.X += 136 * GUIScale;
messageBox[7] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(48 * GUIScale, 14 * GUIScale), colorBorders);
messageBox[7].Position = position;
messageBox[8] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(46 * GUIScale, 12 * GUIScale), colorBackground);
messageBox[8].Position = position;
messageBox[9] = patternCenter;
messageBox[9].Data = "Yes (E)";
messageBox[9].Position = new Vector2(position.X, position.Y - strHalf);
//shadow
position = surfaceAdjust + surfaceHalf;
position += 7 * GUIScale;
messageBox[0] = new MySprite(SpriteType.TEXTURE, "SquareSimple",
position, new Vector2(192 * GUIScale, 34 * GUIScale), colorShadow);
}
#endregion
#region "Draw"
void DrawStartUp()
{
using (MySpriteDrawFrame frame = surface.DrawFrame())
DrawProgressBar(frame);
}
void DrawPanel()
{
using (MySpriteDrawFrame frame = surface.DrawFrame())
{
DrawHeader(frame);
DrawStatusBorders(frame);
DrawContentBorders(frame);
DrawContent(frame);
DrawButtons(frame);
if(showMessageBox)
DrawMessageBox(frame);
}
}
void DrawStandby()
{
using (MySpriteDrawFrame frame = surface.DrawFrame())
{
DrawLogo(frame);
DrawProgressBar(frame);
}
}
void DrawButtons(MySpriteDrawFrame frame)
{
frame.Add(logButtonSprites[0]);
frame.Add(logButtonSprites[1]);
frame.Add(logButtonSprites[2]);
frame.Add(connectorsButtonSprites[0]);
frame.Add(connectorsButtonSprites[1]);
frame.Add(connectorsButtonSprites[2]);
frame.Add(commandsButtonSprites[0]);
frame.Add(commandsButtonSprites[1]);
frame.Add(commandsButtonSprites[2]);
frame.Add(ordersButtonSprites[0]);
frame.Add(ordersButtonSprites[1]);
frame.Add(ordersButtonSprites[2]);
}
void DrawProgressBar(MySpriteDrawFrame frame)
{
frame.Add(progressBarSprites[0]);
frame.Add(progressBarSprites[1]);
frame.Add(progressBarSprites[2]);
}
void DrawLogo(MySpriteDrawFrame frame)
{
frame.Add(standbySprites[0]);
frame.Add(standbySprites[1]);
frame.Add(standbySprites[2]);
frame.Add(standbySprites[3]);
}
void DrawContent(MySpriteDrawFrame frame)
{
switch (menu)
{
case 0:
UppdateConnectorMenuInfo();
DrawConnectorInfo(frame);
break;
case 1:
UppdateCommandsMenuInfo();
DrawCommands(frame);
break;
case 2:
UppdateLogMenu(frame);
break;
case 3:
UppdateOrdersListMenu(frame);
break;
default:
menu = 0;
break;
}
}
void DrawConnectorInfo(MySpriteDrawFrame frame)
{
frame.Add(connectorMenuSprites[0]);
frame.Add(connectorMenuSprites[1]);
frame.Add(connectorMenuSprites[2]);
frame.Add(connectorMenuSprites[3]);
frame.Add(connectorMenuSprites[4]);
frame.Add(connectorMenuSprites[5]);
frame.Add(connectorMenuSprites[6]);
frame.Add(connectorMenuSprites[7]);
frame.Add(connectorMenuSprites[8]);
frame.Add(connectorMenuSprites[9]);
}
void DrawCommands(MySpriteDrawFrame frame)
{
frame.Add(commandsMenuSprites[0]);
frame.Add(commandsMenuSprites[1]);
frame.Add(commandsMenuSprites[2]);
frame.Add(commandsMenuSprites[3]);
if (connectors[connectorNum].Status == MyShipConnectorStatus.Connected)
for (int i = 0; i < filterSprites.Length; i++)
frame.Add(filterSprites[i]);
}
void DrawHeader(MySpriteDrawFrame frame)
{
frame.Add(headerSprites[0]);
frame.Add(headerSprites[1]);
if (transferState != TransferState.Idle)
DrawHeaderProgressBar(frame);
else frame.Add(headerSprites[2]);
}
void DrawHeaderProgressBar(MySpriteDrawFrame frame)
{
frame.Add(headerProgressBarSprites[0]);
frame.Add(headerProgressBarSprites[1]);
frame.Add(headerProgressBarSprites[2]);
}
void DrawStatusBorders(MySpriteDrawFrame frame)
{
frame.Add(statusBorderSprites[0]);
frame.Add(statusBorderSprites[1]);
}
void DrawContentBorders(MySpriteDrawFrame frame)
{
frame.Add(contentBorderSprites[0]);
frame.Add(contentBorderSprites[1]);
}
private void DrawMessageBox(MySpriteDrawFrame frame)
{
frame.Add(messageBox[0]);
frame.Add(messageBox[1]);
frame.Add(messageBox[2]);
frame.Add(messageBox[3]);
frame.Add(messageBox[4]);
frame.Add(messageBox[5]);
frame.Add(messageBox[6]);
frame.Add(messageBox[7]);
frame.Add(messageBox[8]);
frame.Add(messageBox[9]);
}
#endregion
#region "Uppdate stuff"
void ResizeString(ref string str)
{
int pos = MaxCharsOnSurface;
while (pos < str.Length)
{
pos = str.LastIndexOf(' ', pos);
str = str.Insert(pos, "\n");
pos += MaxCharsOnSurface;
}
/* pos -= pos - str.Length;
pos = str.LastIndexOf(' ', pos);
str.Insert(pos, "\n");*/
str.Trim();
}
void ShowMessageBox(string Message)
{
ResizeString(ref Message);
StringBuilder sb = new StringBuilder(Message);
Vector2 textSize = surface.MeasureStringInPixels(sb, FONT, FONT_SIZE);
Vector2 windowSize = new Vector2(192 * GUIScale, 36 * GUIScale);
//resize window
if (textSize.X > 184 * GUIScale)
windowSize.X = textSize.X + 8 * GUIScale;
if (textSize.Y > 10 * GUIScale)
windowSize.Y = textSize.Y + 24 * GUIScale;
messageBox[0].Size = windowSize;
messageBox[1].Size = windowSize;
messageBox[2].Size = windowSize - 4;
//message
Vector2 position = messageBox[2].Position.Value;
position.Y = position.Y - textSize.Y * 0.5f - 8 * GUIScale;
messageBox[3].Position = position;
messageBox[3].Data = Message;
//move buttons
position = messageBox[2].Position.Value;
position.Y = position.Y + textSize.Y * 0.5f + 1;
position.X -= 68 * GUIScale;
messageBox[4].Position = position;
messageBox[5].Position = position;
messageBox[6].Position = new Vector2(position.X, position.Y - strHalf);
position.X += 136 * GUIScale;
messageBox[7].Position = position;
messageBox[8].Position = position;
messageBox[9].Position = new Vector2(position.X, position.Y - strHalf);
showMessageBox = true;
}
void UppdateProgressBar(float value, float max)
{
Vector2 position = surfaceAdjust + surfaceHalf;
position.X -= pBLH;
position.X += pBLH * value / max;
Vector2 size = new Vector2(1, 22 * GUIScale);
size.X = progressBarLength * value / max;
progressBarSprites[2].Position = position;
progressBarSprites[2].Size = size;
position.Y = headerPosition.Y;
headerProgressBarSprites[2].Position = position;
headerProgressBarSprites[2].Size = size;
}
void UppdateConnectorMenuInfo()
{
StringBuilder sb = new StringBuilder("");
sb = new StringBuilder("Connector: N" + (connectorNum + 1) + "|" + connectors.Count);
connectorMenuSprites[0].Data = sb.ToString();
sb.Clear();
sb.Append(connectors[connectorNum].CustomName);
connectorMenuSprites[1].Data = sb.ToString();
Color color = colorText;
sb.Clear();
if (connectors[connectorNum].IsFunctional)
{
if (!connectors[connectorNum].Enabled)
{
color = colorWarning;
sb.Append("Disabled");
}
else
sb.Append(connectors[connectorNum].Status.ToString());
}
else
{
sb.Append("Damaged!");
color = colorError;
}
connectorMenuSprites[3].Data = sb.ToString();
connectorMenuSprites[3].Color = color;
if (connectors[connectorNum].Status == MyShipConnectorStatus.Connected)
{
string type = "Ship";
if (connectors[connectorNum].OtherConnector.CubeGrid.IsStatic)
type = "Station";
else if (connectors[connectorNum].OtherConnector.CubeGrid.GridSizeEnum == MyCubeSize.Small)
type = "Small " + type;
sb.Clear(); sb.Append(type);
connectorMenuSprites[5].Data = sb.ToString();
sb.Clear(); sb.Append(connectors[connectorNum].OtherConnector.CubeGrid.CustomName);
connectorMenuSprites[7].Data = sb.ToString();
sb.Clear(); sb.Append(connectors[connectorNum].OtherConnector.GetOwnerFactionTag());
connectorMenuSprites[9].Data = sb.ToString();
}
else
{
connectorMenuSprites[5].Data = "";
connectorMenuSprites[7].Data = "";
connectorMenuSprites[9].Data = "";
}
}
void UppdateCommandsMenuInfo()
{
StringBuilder sb;
sb = new StringBuilder("Connector: N" + (connectorNum + 1) + "|" + connectors.Count);
commandsMenuSprites[0].Data = sb.ToString();
sb.Clear(); sb.Append(connectors[connectorNum].CustomName);
commandsMenuSprites[1].Data = sb.ToString();
}
void UppdateLogMenu(MySpriteDrawFrame frame)
{
int i = logLines.Count > maxContentlines ? logLines.Count - maxContentlines - logSlider : 0;
for (int l = 0; l < maxContentlines && i < logLines.Count; i++, l++)
{
contentSprites[l].Data = logLines[i];
contentSprites[l].Color = logColors[i];
frame.Add(contentSprites[l]);
}
}
void UppdateOrdersListMenu(MySpriteDrawFrame frame)
{
ordersList.Clear();
foreach (Order order in orders)
order.AddToListAsStrings(ref ordersList);
for (int i = 0; i < maxContentlines && i < ordersList.Count; i++)
{
contentSprites[i].Data = ordersList[i];
contentSprites[i].Color = colorText;
frame.Add(contentSprites[i]);
}
}
void SetButtonsColor()
{
buttonsColor = new Color[] { colorInactive, colorInactive, colorInactive, colorInactive };
buttonsColor[menu] = colorButton;
connectorsButtonSprites[1].Color = buttonsColor[0];
commandsButtonSprites[1].Color = buttonsColor[1];
logButtonSprites[1].Color = buttonsColor[2];
ordersButtonSprites[1].Color = buttonsColor[3];
}
void UppdateTransferStateLine()
{
if (unload) commandsMenuSprites[3].Data = "Unload";
else commandsMenuSprites[3].Data = "Gather";
}
void UppdateFilterList()
{
for (int i = 0; i < filterSprites.Length; i++)
{
filterSprites[i].Color = colorText;
filterSprites[i].Data = filterNames[i + filterSlider];
}
filterSprites[filterInt - filterSlider].Color = colorHighlighted;
}
#endregion
#region "Input"
public bool GetNumFromIdDictionary(long id, out int num)
{
if (connectorsNumsFomId.ContainsKey(id))
{
num = connectorsNumsFomId[id];
return true;
}
num = 0;
return false;
}
void ConnectorsMenuInput(PressedKey Key)
{
switch (Key)
{
case PressedKey.D:
connectorNum++;
connectorNum %= connectors.Count;
break;
case PressedKey.A:
connectorNum--;
if (connectorNum < 0) connectorNum = connectors.Count - 1;
break;
case PressedKey.C:
if (orders.Count > 0 && currentOrder.ConnectorNum == connectorNum)
{
ShowMessageBox("Are you sure you want to switch lock on this connector?This will interrupt current transfer.");
messageBoxResult = MessageBoxResult.IterruptTransfer;
}
else
connectors[connectorNum].ToggleConnect();
break;
case PressedKey.Space:
if (orders.Count > 0 && currentOrder.ConnectorNum == connectorNum)
{
ShowMessageBox("Are you sure you want to switch state on this connector?This will interrupt current transfer.");
messageBoxResult = MessageBoxResult.IterruptTransfer;
}
else
connectors[connectorNum].Enabled = !connectors[connectorNum].Enabled;
break;
}
}
void CommandsMenuInput(PressedKey Key)
{
switch (Key)
{
case PressedKey.W:
if (filterInt > 0)
{
if (filterSlider == filterInt)
filterSlider--;
filterInt--;
}
UppdateFilterList();
break;
case PressedKey.S:
if (filterInt < filterNames.Length - 1)
{
filterInt++;
if (filterInt - filterSlider >= filterSprites.Length)
filterSlider++;
}
UppdateFilterList();
break;
case PressedKey.C:
unload = !unload;
UppdateTransferStateLine();
break;
case PressedKey.Space:
orders.Add(new Order(connectorNum, (TypeFilter)filterInt, unload));
program.Storage += "0 " + (unload ? "unload " : "gather ") + (connectorNum - 1) + " " + ((TypeFilter)filterInt).ToString() + '\n';
break;
}
}
void LogMenuInput(PressedKey Key)
{
switch (Key)
{
case PressedKey.C:
if (orders.Count > 0)
{
ShowMessageBox("Are you sure you want to interrupt this transfer!?");
messageBoxResult = MessageBoxResult.IterruptTransfer;
}
break;
case PressedKey.W:
if (logSlider < logLines.Count - maxContentlines)
{
logSlider += maxContentlines;
if (logSlider > logLines.Count - maxContentlines)
logSlider = logLines.Count - maxContentlines;
}
break;
case PressedKey.S:
if (logSlider > 0)
{
logSlider -= maxContentlines;
if (logSlider < 0)
logSlider = 0;
}
break;
}
}
void OrdersListMenuInput(PressedKey Key)
{
switch (Key)
{
case PressedKey.W:
if (orderListSlider < ordersList.Count - maxContentlines)
{
orderListSlider += maxContentlines;
if (orderListSlider > ordersList.Count - maxContentlines)
orderListSlider = ordersList.Count - maxContentlines;
}
break;
case PressedKey.S:
if (orderListSlider > 0)
{
orderListSlider -= maxContentlines;
if (orderListSlider < 0)
orderListSlider = 0;
}
break;
}
}
void Input(PressedKey Key)
{
if (showMessageBox)
{
if (Key == PressedKey.Q)
showMessageBox = false;
if (Key == PressedKey.E)
{
switch (messageBoxResult)
{
case MessageBoxResult.IterruptTransfer:
ReportAndRemoveOrder("Interrupted", "Operator request");
break;
case MessageBoxResult.RemoveOrder:
break;
}
showMessageBox = false;
}
return;
}
if (Key == PressedKey.E)
{
menu++;
menu %= 4;
SetButtonsColor();
return;
}
if (Key == PressedKey.Q)
{
menu--;
if (menu < 0)
menu = 3;
SetButtonsColor();
return;
}
switch (menu)
{
case 0:
ConnectorsMenuInput(Key);
break;
case 1:
CommandsMenuInput(Key);
break;
case 2:
LogMenuInput(Key);
break;
case 3:
OrdersListMenuInput(Key);
break;
default:
menu = 0;
break;
}
}
public void InterGridInput()
{
if (USI.HasPendingMessage)
{
MyIGCMessage message = USI.AcceptMessage();
//unload 13400530330569 "glass boots, golden potato"
string data = message.Data.ToString();
string[] words = data.Split(' ');
switch (words[0].Trim().ToUpper())
{
case "GATHER":
case "UNLOAD":
Order order = new Order();
if (order.TryParseOrder(message.Source, data))
{
orders.Add(order);
program.Storage += message.Source + " " + data + '\n';
}
break;
}
}
}
public void ManualInput(string argument)
{
if (string.IsNullOrWhiteSpace(argument))
return;
string[] words = argument.Split(' ');
//unload 1 "steel plates, copper ore, radiator"
//gather 2 "steel tubes, reinforced glass, ammo box"
//gather 2 ore
//unload 2 tools
switch (words[0].Trim().ToUpper())
{
case "E": Input(PressedKey.E); break;
case "Q": Input(PressedKey.Q); break;
case "A": Input(PressedKey.A); break;
case "D": Input(PressedKey.D); break;
case "W": Input(PressedKey.W); break;
case "S": Input(PressedKey.S); break;
case "SP": Input(PressedKey.Space); break;
case "C": Input(PressedKey.C); break;
case "GATHER":
case "UNLOAD":
Order order = new Order();
if (order.TryParseOrder(argument))
{
orders.Add(order);
program.Storage += "0 " + argument + '\n';
}
break;
}
}
public void MovementInput(object sender, InputChangedEventArgs e)
{ Input(e.Key); }
#endregion
#region "Transfer"
void NewLogEntry(string message, Color color)
{
lastMessage = message;
logLines.Add(message);
logColors.Add(color);
}
void SortConveyorSystems(int maxIterations, ref int number, List<IMyTerminalBlock> containers, ref List<Cargo> SmallConveyorSystem, ref List<Cargo> LargeConveyorSystem)
{
for (int i = 0; i < maxIterations && number < containers.Count; i++, number++)
{
IMyInventory inventory = containers[number].GetInventory();
if (!inventory.CanTransferItemTo(connectors[currentOrder.ConnectorNum].GetInventory(), MyItemType.MakeTool("WelderItem")))
{
NewLogEntry("Line is damaged, blocked or doesn't exist.", colorWarning);
NewLogEntry(containers[number].CustomName + " -> " + connectors[currentOrder.ConnectorNum].CustomName, colorWarning);
continue;
}
if (inventory.CanTransferItemTo(connectors[currentOrder.ConnectorNum].GetInventory(), MyItemType.MakeComponent("PowerCell")))
{
LargeConveyorSystem.Add(new Cargo(ConveyorSystemSize.Big, containers[number]));
SmallConveyorSystem.Add(new Cargo(ConveyorSystemSize.Big, containers[number]));
}
else
SmallConveyorSystem.Add(new Cargo(ConveyorSystemSize.Small, containers[number]));
}
}
void GetFromListOnlySubtypeId(string SubtypeId, int maxIterations, ref int number, ref List<IMyTerminalBlock> containers)
{
for (int i = 0; i < maxIterations && number < containers.Count; i++)
{
if (!containers[number].BlockDefinition.SubtypeId.Contains(SubtypeId))
containers.RemoveAt(number);
else
number++;
}
}
List<IMyShipConnector> opConnectors = new List<IMyShipConnector>();
bool PreRunChecks()
{
if (currentOrder.ConnectorNum < 0 || currentOrder.ConnectorNum >= connectors.Count)
{
NewLogEntry("Connector number out of bounds.", colorError);
transferState = TransferState.Report;
return false;
}
return true;
}
void ReportAndRemoveOrder(string status, string message)
{
if (currentOrder.CustomerID != 0)
{
NewLogEntry("Sending report to customer: " + currentOrder.CustomerID, colorText);
if (program.IGC.SendUnicastMessage(currentOrder.CustomerID, "CarTraSys", status.ToUpper() + " " + currentOrder.ConnectorID + " " + message))
NewLogEntry("Success", colorSuccess);
else
NewLogEntry("Failed", colorError);
}
if (currentOrder.CheckNames)
{
NewLogEntry("Order list:", colorText);
foreach (OrderPosition oItem in currentOrder.PositionsList)
NewLogEntry(oItem.ToString(), colorWarning);
foreach (OrderPosition oItem in currentOrder.CompletedList)
NewLogEntry(oItem.ToString(), colorSuccess);
}
NewLogEntry("Transfer: " + status, colorText);
if (!string.IsNullOrWhiteSpace(message))
NewLogEntry(message, colorText);
orders.RemoveAt(0);
program.Storage = program.Storage.Remove(0, program.Storage.IndexOf('\n') + 1);
if (showMessageBox && messageBoxResult == MessageBoxResult.IterruptTransfer)
showMessageBox = false;
transferState = TransferState.Idle;
}
int Transfer()
{
program.Echo("Transfer: " + transferState.ToString());
switch (transferState)
{
case TransferState.Prepear:
logLines.Clear();
logColors.Clear();
UppdateProgressBar(0, 1);
if (!PreRunChecks())
break;
SetButtonsColor();
if (connectors[currentOrder.ConnectorNum].Status != MyShipConnectorStatus.Connected)
{
ReportAndRemoveOrder("Failed", "Chosen connector isn't connected.");
break;
}
opContainers = new List<IMyTerminalBlock>();
FillUnsortedWith<IMyCargoContainer>();
NewLogEntry("Gathering containers data.", colorText);
if (currentOrder.Unload)
transferState = TransferState.GetOpContainers;
else
{
transferState = TransferState.GetOpAllContainers; gather = 6;
};
break;
// End Prepear
case TransferState.GetOpContainers:
if (GetOpponentContainersFromUnsorted())
transferState = TransferState.AfterGatherCheck;
break;
case TransferState.GetOpAllContainers:
if (GatherContainers(ref opContainers, ref opConnectors, connectors[currentOrder.ConnectorNum].OtherConnector))
transferState = TransferState.AfterGatherCheck;
break;
case TransferState.AfterGatherCheck:
if (opContainers.Count == 0)
{
ReportAndRemoveOrder("Failed", "Cant initialize opponents containers.");
break;
}
NewLogEntry("Separating conveyor lines.", colorText);
opConnectors.Clear();
mySContainer = opSContainer = item = 0;
itemCounter = 1;
mySCS.Clear(); myLCS.Clear();
opSCS.Clear(); opLCS.Clear();
passBulky = false;
transferState = TransferState.SortConveyorSystems;
break;
case TransferState.SortConveyorSystems:
UppdateProgressBar(mySContainer + opSContainer, mySCS.Count + opSCS.Count);
SortConveyorSystems(5, ref mySContainer, myContainers, ref mySCS, ref myLCS);
CheckShipsConnection();
SortConveyorSystems(5, ref opSContainer, opContainers, ref opSCS, ref opLCS);
if (opSContainer >= opContainers.Count && mySContainer >= myContainers.Count)
{
if (opLCS.Count == 0 || myLCS.Count == 0)
{
NewLogEntry("Can't retrieve or pass large items.", colorWarning);
passBulky = true;
}
mySContainer = myLContainer = opSContainer = opLContainer = item = 0;
NewLogEntry("Transfer: begun.", colorText);
if (currentOrder.Unload)
transferState = currentOrder.CheckNames ? TransferState.UnloadByName : TransferState.UnloadByType;
else
transferState = currentOrder.CheckNames ? TransferState.GatherByName : TransferState.GatherByType;
}
break;
case TransferState.UnloadByName:
UppdateProgressBar(mySContainer + opSContainer, mySCS.Count + opSCS.Count);
Unload(currentOrder.CheckItemSubtypeID);
if (currentOrder.PositionsList.Count == 0)
transferState = TransferState.Report;
break;
case TransferState.UnloadByType:
UppdateProgressBar(mySContainer + opSContainer, mySCS.Count + opSCS.Count);
Unload(currentOrder.CheckItemType);
break;
case TransferState.GatherByName:
UppdateProgressBar(mySContainer + opSContainer, mySCS.Count + opSCS.Count);
Gather(currentOrder.CheckItemSubtypeID);
if (currentOrder.PositionsList.Count == 0)
transferState = TransferState.Report;
break;
case TransferState.GatherByType:
UppdateProgressBar(mySContainer + opSContainer, mySCS.Count + opSCS.Count);
Gather(currentOrder.CheckItemType);
break;
// transfer end
case TransferState.Report:
UppdateProgressBar(1, 1);
ReportAndRemoveOrder("Over", "");
break;
case TransferState.Idle:
break;
}
return 0;
}
void Gather(DelegateCheckItem delegateCheckItem)
{
if (CheckOpponentSystems() != 0)
{ NextOpContainer(); return; }
if (PreTransferChecks() != 0) return;
opInventory = opSCS[opSContainer].Container.GetInventory();
if (opInventory == null || opInventory.ItemCount == 0 || item >= opInventory.ItemCount)
{
NextOpContainer();
return;
}
var itemType = opInventory.GetItemAt(item).Value.Type;
message.Clear();
message.Append(itemCounter + "-" + itemType.SubtypeId);
var itemInfo = itemType.GetItemInfo();
orderPosition = delegateCheckItem.Invoke(itemType, ref message);
if (orderPosition < 0)
{
OpNextItem();
return;
}
if (CheckOpponentSystems() != 0)
{ NextOpContainer(); return; }
bool bulky = !(itemInfo.IsIngot || itemInfo.IsOre || itemInfo.IsTool) && itemInfo.Size.Length() >= 0.4d;
if (bulky)
{
if (passBulky)
{
OpNextItem();
return;
}
if (opSCS[opSContainer].Size == ConveyorSystemSize.Small)
{
NewLogEntry(message.ToString(), colorWarning);
NewLogEntry("To huge to get from.", colorWarning);
NewLogEntry(opSCS[opSContainer].Container.CustomName, colorWarning);
OpNextItem();
itemCounter++;
return;
}
if (myLContainer >= myLCS.Count)
{
NewLogEntry("No more space for large items.", colorWarning);
passBulky = true;
itemCounter++;
return;
}
// Big
if (CheckContainer(myLCS, myLContainer) != 0)
return;
MoveItem(opSCS, opSContainer, myLCS, myLContainer, NextMyBigContainer, OpNextItem);
}
else
MoveItem(opSCS, opSContainer, mySCS, mySContainer, NextMyContainer, OpNextItem);
}
void Unload(DelegateCheckItem delegateCheckItem)
{
if (PreTransferChecks() != 0) return;
myInventory = mySCS[mySContainer].Container.GetInventory();
if (myInventory == null || myInventory.ItemCount == 0 || item >= myInventory.ItemCount)
{
NextMyContainer();
return;
}
var itemType = myInventory.GetItemAt(item).Value.Type;
var itemInfo = itemType.GetItemInfo();
message.Clear();
message.Append(itemCounter + "-" + itemType.SubtypeId);
orderPosition = delegateCheckItem.Invoke(itemType, ref message);
if (orderPosition < 0)
{
MyNextItem();
return;
}
bool bulky = !(itemInfo.IsIngot || itemInfo.IsOre || itemInfo.IsTool) && itemInfo.Size.Length() >= 0.4d;
if (bulky)
{
if (passBulky)
{
MyNextItem();
return;
}
if (mySCS[mySContainer].Size == ConveyorSystemSize.Small)
{
NewLogEntry(message.ToString() + ". To large.", colorWarning);
//NewLogEntry("To huge to get from.", WarningColor);
//NewLogEntry(_mySCS[_mySContainer].Container.CustomName, WarningColor);
MyNextItem();
itemCounter++;
return;
}
if (opLContainer >= opLCS.Count)
{
NewLogEntry(message.ToString(), colorWarning);
NewLogEntry("No more space for large items.", colorWarning);
passBulky = true;
itemCounter++;
return;
}
if (CheckContainer(opLCS, opLContainer) != 0) return;
// Big
MoveItem(mySCS, mySContainer, opLCS, opLContainer, NextOpBigContainer, MyNextItem);
}
else
MoveItem(mySCS, mySContainer, opSCS, opSContainer, NextOpContainer, MyNextItem);
}
delegate int DelegateCheckItem(MyItemType itemType, ref StringBuilder message);
delegate void DelegateNextContainer();
delegate void DelegateNextItem();
int CheckOpponentSystems()
{
if (opSCS[opSContainer].Container == null)
{
NewLogEntry("Opponent's container was destroyed!", colorError);
return 202;
}
if (!opSCS[opSContainer].Container.IsFunctional)
{
NewLogEntry("Opponent's container is damaged!", colorWarning);
return 203;
}
if (!connectors[currentOrder.ConnectorNum].OtherConnector.GetInventory().IsConnectedTo(opSCS[opSContainer].Container.GetInventory()))
{
NewLogEntry("Line is damaged, blocked or doesn't exist.", colorWarning);
NewLogEntry(connectors[currentOrder.ConnectorNum].OtherConnector.CustomName + " -> " + opSCS[opSContainer].Container.CustomName, colorWarning);
return 205;
}
return 0;
}
int CheckContainer(List<Cargo> fromConveyor, int fromContainer)
{
if (fromConveyor[fromContainer].Container == null)
{
NewLogEntry("Recipient container was destroyed!", colorError);
return 200;
}
if (!fromConveyor[fromContainer].Container.IsFunctional)
{
NewLogEntry("Recipient container is unfunctional!", colorError);
NewLogEntry(fromConveyor[fromContainer].Container.CustomName, colorWarning);
return 200;
}
return 0;
}
void MoveItem(List<Cargo> fromConveyor, int fromContainer, List<Cargo> toConveyor, int toContainer, DelegateNextContainer delNextContainer, DelegateNextItem delNextItem)
{
IMyInventory
fromInventory = fromConveyor[fromContainer].Container.GetInventory(),
toInventory = toConveyor[toContainer].Container.GetInventory();
MyFixedPoint opSpareSpace = GetSpareSpace(toConveyor[toContainer].Container);
var itemType = fromInventory.GetItemAt(item).Value.Type;
var itemInfo = itemType.GetItemInfo();
// free space of 1 liter volume
if (opSpareSpace < (MyFixedPoint)0.001)
{
delNextContainer.Invoke();
return;
}
MyFixedPoint
itemAmount,
itemStackVolume = fromInventory.GetItemAmount(itemType) * itemInfo.Volume;
if (opSpareSpace > itemStackVolume)
itemAmount = fromInventory.GetItemAmount(itemType);
else
itemAmount = (MyFixedPoint)((float)(itemStackVolume - (itemStackVolume - opSpareSpace)) / itemInfo.Volume);
if (!itemInfo.UsesFractions && itemAmount < 1)
{
delNextContainer.Invoke();
return;
}
if (currentOrder.CheckNames)
{
if (currentOrder.PositionsList[orderPosition].Left < itemAmount)
itemAmount = currentOrder.PositionsList[orderPosition].Left;
currentOrder.PositionsList[orderPosition].Moved += (int)Math.Floor((double)itemAmount);
if (currentOrder.PositionsList[orderPosition].Done)
{
currentOrder.CompletedList.Add(currentOrder.PositionsList[orderPosition]);
currentOrder.PositionsList.RemoveAt(orderPosition);
}
}
message.Append(":" + Math.Round((double)itemAmount, 2) + " -> ");
if (fromInventory.TransferItemTo(toInventory, item, null, true, itemAmount))
{
NewLogEntry(message.ToString() + "done.", colorSuccess);
}
else
{
NewLogEntry(message.ToString() + "error.", colorError);
NewLogEntry("Cannot transfer this item to opponent's ship.", colorWarning);
delNextItem.Invoke();
}
itemCounter++;
}
int PreTransferChecks()
{
if (connectors[currentOrder.ConnectorNum] == null)
{
ReportAndRemoveOrder("Failed", "Connector is destroyed!");
UppdateProgressBar(1, 1);
return 100;
}
if (!connectors[currentOrder.ConnectorNum].IsFunctional)
{
ReportAndRemoveOrder("Failed", "Connector is unfunctional!");
UppdateProgressBar(1, 1);
return 101;
}
if (CheckShipsConnection() != 0) return 102;
if (mySCS[mySContainer].Container == null)
{
NewLogEntry("Container was destroyed!", colorError);
NextMyContainer();
return 200;
}
if (!mySCS[mySContainer].Container.IsFunctional)
{
NewLogEntry("Container is unfunctional!", colorWarning);
NewLogEntry(mySCS[mySContainer].Container.CustomName, colorWarning);
NextMyContainer();
return 201;
}
if (!mySCS[mySContainer].Container.GetInventory().IsConnectedTo(connectors[currentOrder.ConnectorNum].GetInventory()))
{
NewLogEntry("Line is damaged, blocked or doesn't exist.", colorWarning);
NewLogEntry(mySCS[mySContainer].Container.CustomName + " -> " + connectors[currentOrder.ConnectorNum].CustomName, colorWarning);
NextMyContainer();
return 204;
}
return 0;
}
void MyNextItem()
{
item++;
if (item >= myInventory.ItemCount)
NextMyContainer();
}
void OpNextItem()
{
item++;
if (item >= opInventory.ItemCount)
NextOpContainer();
}
int CheckShipsConnection()
{
if (connectors[currentOrder.ConnectorNum].Status != MyShipConnectorStatus.Connected)
{
UppdateProgressBar(1, 1);
ReportAndRemoveOrder("Failed", "Connection lost!");
return 1;
}
return 0;
}
void NextMyContainer()
{
mySContainer++;
item = 0;
if (mySContainer >= mySCS.Count)
transferState = TransferState.Report;
}
void NextMyBigContainer()
{
myLContainer++;
}
void NextOpContainer()
{
opSContainer++;
if (opSContainer >= opSCS.Count)
transferState = TransferState.Report;
}
void NextOpBigContainer()
{
opLContainer++;
}
#endregion
void FillUnsortedWith<T>() where T : class, IMyTerminalBlock
{
unsortedBlocks.Clear();
program.GridTerminalSystem.GetBlocksOfType<T>(unsortedBlocks);
gather--;
}
public void Startup()
{
switch (startupState)
{
case StartupState.ReadData:
ReadData();
fontSize = FONT_SIZE * GUIScale;
headerFontSize = HEADER_SIZE * GUIScale;
borderSize = BORDER_SIZE * GUIScale;
StringBuilder sb = new StringBuilder("a");
strHalf = surface.MeasureStringInPixels(sb, FONT, fontSize).Y * 0.5f;
float charWidth = surface.MeasureStringInPixels(sb, FONT, fontSize).X;
MaxCharsOnSurface = (int)(surface.SurfaceSize.X / charWidth) - 2;
MakeStandBySprites();
MakeProgressBarSprites();
myContainers.Clear();
FillUnsortedWith<IMyShipWelder>();
gather = 6;
startupState = StartupState.GatherContainers;
UppdateProgressBar(0, 1);
connectors.Clear();
connectorsNumsFomId.Clear();
break;
case StartupState.GatherContainers:
if (GatherContainers(ref myContainers, ref connectors, program.Me))
startupState = StartupState.MakeSprites;
break;
case StartupState.MakeSprites:
UppdateProgressBar(1, 1);
if (connectors.Count == 0)
{
state = SystemState.Error;
standbySprites[3].Data = "Error:\nThis construction lacks of connectors.";
program.Runtime.UpdateFrequency = UpdateFrequency.Update100;
break;
}
InitDictionary();
patternLeft = MySprite.CreateText("", FONT, colorText, fontSize, TextAlignment.LEFT);
patternRight = MySprite.CreateText("", FONT, colorText, fontSize, TextAlignment.RIGHT);
patternCenter = MySprite.CreateText("", FONT, colorText, fontSize, TextAlignment.CENTER);
MakeHeaderSprites();
MakeStatusBorderSprites();
MakeContentBorderSprites();
MakeConnectorInfoSprites();
MakeCommandsMenuSprites();
MakeFilterListSprites();
MakeLogListSprites();
MakeButtonsSprites();
SetButtonsColor();
UppdateTransferStateLine();
MakeHeaderProgressBarSprites();
MakeMessageBoxSprites();
startupState = StartupState.Ready;
break;
case StartupState.Ready:
UppdateProgressBar(1, 1);
ReadStorageForOrders();
state = SystemState.Running;
break;
}
}
bool ReadStorageForOrders()
{
program.Storage = program.Storage.Trim();
if (!string.IsNullOrWhiteSpace(program.Storage))
{
string[] data = program.Storage.Split('\n');
for (int i = 0; i < data.Length - 1; i++)
{
string strID = data[i].Split(' ')[0].Trim();
long ID = long.Parse(strID);
Order order = new Order();
if (order.TryParseOrder(ID, data[i].Remove(0, strID.Length + 1)))
orders.Add(order);
}
return true;
}
return false;
}
void Wakeup()
{
surface.ContentType = ContentType.SCRIPT;
surface.Script = "";
state = SystemState.Startup;
startupState = StartupState.ReadData;
program.Runtime.UpdateFrequency = UpdateFrequency.Update10;
UppdateProgressBar(0, 8);
DrawStartUp();
}
public int Main()
{
program.Echo(VERSION);
program.Echo("Last log: " + lastMessage);
program.Echo("State: " + state.ToString());
try
{
switch (state)
{
case SystemState.Startup:
program.Echo("Startup: " + startupState.ToString());
Startup();
DrawStartUp();
break;
case SystemState.Idle:
if (controller.IsUnderControl || orders.Count > 0)
Wakeup();
break;
case SystemState.Running:
if (orders.Count > 0)
{
state = SystemState.Transfer;
transferState = TransferState.Prepear;
break;
}
if (!controller.IsUnderControl && transferState == TransferState.Idle)
{
state = SystemState.ShutdownLogo;
standbySprites[3].Data = "Now you can turn off the power.";
using (MySpriteDrawFrame frame = surface.DrawFrame())
DrawLogo(frame);
program.Runtime.UpdateFrequency = UpdateFrequency.Update100;
break;
}
DrawPanel();
break;
case SystemState.Error:
if (!controller.IsUnderControl)
{
state = SystemState.ShutdownLogo;
standbySprites[3].Data = "Now you can turn off the power.";
program.Runtime.UpdateFrequency = UpdateFrequency.Update100;
}
using (MySpriteDrawFrame frame = surface.DrawFrame())
DrawLogo(frame);
break;
case SystemState.Transfer:
if (transferState == TransferState.Idle)
{
if (orders.Count > 0)
{
transferState = TransferState.Prepear;
break;
}
if (controller.IsUnderControl)
{
state = SystemState.Running;
break;
}
state = SystemState.ShutdownLogo;
standbySprites[3].Data = "Now you can turn off the power.";
using (MySpriteDrawFrame frame = surface.DrawFrame())
DrawLogo(frame);
program.Runtime.UpdateFrequency = UpdateFrequency.Update100;
break;
}
Transfer();
DrawPanel();
break;
case SystemState.ShutdownLogo:
// I'm to lazy to make a timer.
state = SystemState.Stop;
break;
case SystemState.Stop:
standbySprites[3].Data = "";
surface.ContentType = ContentType.TEXT_AND_IMAGE;
state = SystemState.Idle;
break;
}
}
catch (Exception e)
{
program.Me.GetSurface(0).WriteText(DateTime.Now.TimeOfDay.ToString(), false);
program.Me.GetSurface(0).WriteText('\n' + e.Message, true);
program.Me.GetSurface(0).WriteText('\n' + e.StackTrace, true);
program.Runtime.UpdateFrequency = UpdateFrequency.None;
program.Echo(e.Message);
}
return 0;
}
}
public Program()
{
program = this;
program.Me.CustomName = "PB CarTraSys";
Runtime.UpdateFrequency = UpdateFrequency.Update100;
cargoTransferSystem = new CargoTransferSystem();
controlReader = new ControlReader(cargoTransferSystem.Controller);
controlReader.inputChangedArgs += cargoTransferSystem.MovementInput;
}
static CargoTransferSystem cargoTransferSystem;
static ControlReader controlReader;
void Main(string argument, UpdateType updateSource)
{
switch (updateSource)
{
case UpdateType.Terminal:
case UpdateType.Trigger:
cargoTransferSystem.ManualInput(argument);
break;
default:
controlReader.ReadInput();
cargoTransferSystem.InterGridInput();
cargoTransferSystem.Main();
break;
}
//Echo("CIC: " + Runtime.CurrentInstructionCount);
}