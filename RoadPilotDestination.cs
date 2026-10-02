/*
 * R e a d m e
 * -----------
 * RoadPilot Destination script
 * --- Verion 1.01 ---
 * Ingame script by Pluscezz
 * 
 * Full steam guide on how to get started: https://steamcommunity.com/sharedfiles/filedetails/?id=3628237056
 * 
 * Fleet Management Update - V1.01
 * New features:
 * - Added a management user interface to the Destination script
 * - Added support for multiple screens and built-in screens (e.g. cockpits)
 * - Added new WaitTypes: Recharge, Cargo and External
 * - Added advanced navigation commands
 * - Added support for multiple routes via custom data
 * - Added support for blinkers
 * - Added support for rear-wheel steering
 * - Added new script parameters for agents
 * - Added estimated time of arrival information
 * - Added connector size dependent reservation
 * 
 * Bug Fixes:
 * - Drive settings map rotation is aligned correctly
 * - Anti-stuck stops completely before reversing
 * - Steering in harsh angles is improved
 * - Parking creep logic is fixed
 * 
 * Improvements:
 * - Map performance
 * - Raycast performance
 */

//Script parameters (editing allowed)
string IGC_TAG = "IntraCar"; //All RoadPilot scripts with the same IGC_TAG will be able to communicate.
string SCREEN_TAG = "[RoadPilot]"; //Tag used to identify screen blocks. Any block with a functional screen will be detected.


//DO NOT TOUCH THIS UNLESS YOU WANT KLANG TO COME
string destinationName;
string BLOCK_TAG = "connectors";
int pingFrequency = 10; //seconds

public enum MessageType
{
	DestinationRequest,
	DestinationAddress,
	DestinationConnector,
	DestinationConnectorDisconnect,
	MapRequest,
	MapUnicast,
	ReserveIntersection,
	RequestReservations,
	RemoveIntersection,
	ParkingBlock,
	ParkingUnblock,
	ParkingPing,
	StatusUpdate,
	StatusRequest,
	Command,
	CommandConfirmed
}
class Message
{
	public DateTime RecievedTime { get; set; }
	public string Content { get; set; }
	public MessageType Type { get; set; }
	public Vector3D Position { get; set; }
	public long Address { get; set; }
}
class Connector
{
	public Vector3D position;
	public bool available;
	public IMyShipConnector connector;
	public bool previousState = false;
	public long reservedAddres;
	public DateTime lastPing;
	public DateTime lastPingSent;
	public bool isSmallPort;

	public Connector(Vector3D position, bool available, IMyShipConnector connector, bool isSmall)
	{
		this.position = position;
		this.available = available;
		this.connector = connector;
		this.isSmallPort = isSmall;
	}
}
class AgentStatus
{
	public string agentName;
	public string status;

	public float batteryPercentage;
	public float cargoPercentage;
	public double speed;

	public int choosenDestinationCount;
	public int choosenDestinationIndex;

	public double etaDistance;
	public double etaTotalDistance;
	public double etaSeconds;
	public double etaTotalSeconds;

	public string navigationType;

	public long agentAdress;

	public DateTime lastPing;

	public AgentStatus prevStatus;
	public bool isSelected;
	public CommandStatus commandStatus;

	public enum CommandStatus
	{
		Waiting,
		Confirmed,
		Rejected
	}

	public AgentStatus(string agentName, string status, float batteryPercentage, float cargoPercentage,double speed, int choosenDestinationCount, int choosenDestinationIndex,double etaDist, double etaSec,double etaTotalDist, double etaTotalSec,string navigationtype,long address, DateTime lastPing, CommandStatus commandConfirmed)
	{
		this.agentName = agentName;
		this.status = status;
		this.batteryPercentage = batteryPercentage;
		this.cargoPercentage = cargoPercentage;
		this.speed = speed;
		this.choosenDestinationCount = choosenDestinationCount;
		this.choosenDestinationIndex = choosenDestinationIndex;
		this.etaDistance = etaDist;
		this.etaTotalDistance = etaTotalDist;
		this.etaTotalSeconds = etaTotalSec;
		this.etaSeconds = etaSec;
		this.agentAdress = address;
		this.lastPing = lastPing;
		this.commandStatus = commandConfirmed;
		this.navigationType = navigationtype;
	}
}
class Destination
{
	public string name;
	public Vector3D position;
	public long address;
	public DateTime lastPing;
	public bool isChoosen;

	public Destination(string Name, Vector3D Position, long Address, DateTime time)
	{
		name = Name;
		position = Position;
		address = Address;
		lastPing = time;
	}
	public string ToString(List<Destination> destinations)
	{
		string output = "";
		List<string> destStrings = new List<string>();
		foreach (var dest in destinations)
		{
			List<string> strings = new List<string>();
			strings.Add(dest.name);
			strings.Add(dest.position.ToString());
			strings.Add(dest.address.ToString());
			destStrings.Add(String.Join("*", strings));
		}

		output = String.Join("/", destStrings);
		return output;
	}
}
class DriveSettings
{
	public NavigationType navigationType = NavigationType.OneWay;
	public CollisionType collisionType = CollisionType.None;
	public WaitType waitType = WaitType.None;
	public ParkingType parkingType = ParkingType.Street;
	public InterruptType interruptType = InterruptType.ReplaceAll;

	public enum InterruptType
	{
		ReplaceAll,
		InsertLast,
		InsertFirst,
		InsertNext
	}

	public enum CollisionType
	{
		None,
		SensorDriven
	}
	public enum NavigationType
	{
		Circle,
		OneWay,
		Patrol
	}
	public enum WaitType
	{
		None,
		Timerblock,
		Recharge,
		Cargo,
		External
	}
	public enum ParkingType
	{
		Street,
		Connector
	}

	public int cursorIndex = 0;
	public int maxCursorIndex = 4;
	public List<Enum> GetEnums()
	{
		List<Enum> enums = new List<Enum>();
		enums.Add(navigationType);
		enums.Add(collisionType);
		enums.Add(waitType);
		enums.Add(parkingType);
		return enums;
	}
}
public enum DisplayState
{
	StatusOverview,
	CommandScreen,
	NavigateScreen,
	DriveSettings,
}
struct Ģ{public string ģ;public Color Ī;}public enum Ĥ{ĥ,Ħ,ħ,Ĩ,ĩ}class ġ{public IMyTextSurface ī;public RectangleF T;
public int ţ;public ġ(IMyTextSurface ī){this.ī=ī;this.ţ=0;Vector2 ì=ī.SurfaceSize;Vector2 Ť=(ī.TextureSize-ì)*0.5f;this.T=new
RectangleF(Ť,ì);}}class ť{public Ĥ Õ;public Ũ ŧ;public string Ţ;public string Ŧ;public bool P;public enum Ũ{ũ,Ū}public ť(Ĥ Õ,
string Ŧ,string Ţ){this.Õ=Õ;this.Ŧ=Ŧ;this.Ţ=Ţ;this.ŧ=Ũ.Ū;}}bool Ř=false;IMyRadioAntenna Œ;List<IMyBroadcastListener>œ=new List
<IMyBroadcastListener>();List<string>Ŕ=new List<string>();List<Connector>ŕ=new List<Connector>();int Ŗ=0;string[]ŗ={"-",
"\\","|","/"};DisplayState ř;List<ġ>Š=new List<ġ>();List<AgentStatus>Ś=new List<AgentStatus>();int ś=-1;int Ŝ=0;int ŝ=0;int
Ş=0;bool[]ş={false,false,false,false,false};float ő=0.1f;DateTime š;DisplayState ū;Vector2 Ų;Vector2 ų;List<ť>Ŵ=new List<
ť>();int ŵ=10;DateTime Ŷ;Destination ŷ;float Ÿ=0.1f;DateTime Ź;List<Destination>ź=new List<Destination>();List<
Destination>Ż=new List<Destination>();DriveSettings ż;Program(){Runtime.UpdateFrequency=UpdateFrequency.Update10;IGC.
RegisterBroadcastListener(IGC_TAG);ż=new DriveSettings();}void Main(string Ž,UpdateType ž){if(!Ř){ķ();return;}destinationName=Me.CubeGrid.
CustomName;ŉ();Echo($"RoadPilot Destination Script V1.01 {ŗ[Ŗ]}\n\nDestination: {destinationName}\nConnectors {ŕ.Count} | Available {ŕ.FindAll(M=>M.available).Count}\nAddress: {IGC.Me}\nInstructions: {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount}\nStatus: {Ś.Count}"
);Ŗ++;if(Ŗ>ŗ.Length-1)Ŗ=0;foreach(var ſ in Ŕ){Echo(ſ);}if(Ŕ.Count>5)Ŕ.Clear();int ű=DateTime.Now.Second/5;if(ű!=ś){Ö();}
if(Ŵ[ŝ].ŧ!=ť.Ũ.ũ){switch(Ž){case"1":if((DateTime.UtcNow-š).TotalSeconds>ő){ş[0]=true;š=DateTime.UtcNow;}else{return;}if(ř
==DisplayState.StatusOverview){if(Ś.Count>0){Ś[Ŝ].isSelected=!Ś[Ŝ].isSelected;}}else if(ř==DisplayState.CommandScreen){if(
Ś.Count>0){Ŵ.ForEach(M=>M.P=false);Ŵ[ŝ].P=!Ŵ[ŝ].P;}if(ŝ==0&&Ŵ[0].P){ř=DisplayState.NavigateScreen;ź.ForEach(M=>M.
isChoosen=false);Ż.Clear();}else if(ŝ==1&&Ŵ[1].P){ř=DisplayState.DriveSettings;ż.interruptType=DriveSettings.InterruptType.
ReplaceAll;ż.waitType=DriveSettings.WaitType.None;ż.parkingType=DriveSettings.ParkingType.Connector;ż.collisionType=DriveSettings.
CollisionType.SensorDriven;Ż.Clear();Ż.Add(ź.Find(M=>M.address==IGC.Me));ź.Find(M=>M.address==IGC.Me).isChoosen=true;}else{Ô(Ŵ[ŝ].Õ);
Ŵ[ŝ].ŧ=ť.Ũ.ũ;Ŷ=DateTime.Now;}}else if(ř==DisplayState.NavigateScreen){if(ź.Count-1>=Ş){ź[Ş].isChoosen=!ź[Ş].isChoosen;if(
ź[Ş].isChoosen&&!Ż.Any(M=>M.address==ź[Ş].address)){Ż.Add(ź[Ş]);}else if(!ź[Ş].isChoosen&&Ż.Any(M=>M.address==ź[Ş].
address)){Ż.Remove(Ż.FindLast(M=>M.address==ź[Ş].address));ź[Ş].isChoosen=false;}}}else if(ř==DisplayState.DriveSettings){if(ż.
cursorIndex==0&&Ż.Count>1){if(((int)ż.navigationType)<Enum.GetValues(typeof(DriveSettings.NavigationType)).Length-1){ż.
navigationType++;}else{ż.navigationType=0;}}else if(ż.cursorIndex==1){if(((int)ż.collisionType)<Enum.GetValues(typeof(DriveSettings.
CollisionType)).Length-1){ż.collisionType++;}else{ż.collisionType=0;}}else if(ż.cursorIndex==2){if(((int)ż.parkingType)<Enum.
GetValues(typeof(DriveSettings.ParkingType)).Length-1){ż.parkingType++;}else{ż.parkingType=0;}if(ż.parkingType==DriveSettings.
ParkingType.Street){if(ż.waitType==DriveSettings.WaitType.Recharge||ż.waitType==DriveSettings.WaitType.Cargo){ż.waitType=
DriveSettings.WaitType.None;}}}else if(ż.cursorIndex==3){if(Ż.Count<=1){ż.waitType=DriveSettings.WaitType.None;return;}int Ŭ=Enum.
GetValues(typeof(DriveSettings.WaitType)).Length-1;bool ŭ=(ż.parkingType==DriveSettings.ParkingType.Connector);int Ű=0;do{int Ů=(
int)ż.waitType+1;if(Ů>Ŭ)Ů=0;ż.waitType=(DriveSettings.WaitType)Ů;bool ů=(!ŭ&&(ż.waitType==DriveSettings.WaitType.Recharge||
ż.waitType==DriveSettings.WaitType.Cargo));if(!ů){break;}Ű++;}while(Ű<=Ŭ);}else if(ż.cursorIndex==4){if(((int)ż.
interruptType)<Enum.GetValues(typeof(DriveSettings.InterruptType)).Length-1){ż.interruptType++;}else{ż.interruptType=0;}}}Ö();break;
case"2":if((DateTime.UtcNow-š).TotalSeconds>ő){ş[1]=true;š=DateTime.UtcNow;}else{return;}if(ř==DisplayState.StatusOverview){
if(Ŝ!=0){Ŝ--;}}else if(ř==DisplayState.CommandScreen){if(ŝ!=0){ŝ--;}}else if(ř==DisplayState.NavigateScreen){if(Ş!=0){Ş--;
}}else if(ř==DisplayState.DriveSettings){if(ż.cursorIndex!=0){ż.cursorIndex--;}}Ö();break;case"3":if((DateTime.UtcNow-š).
TotalSeconds>ő){ş[2]=true;š=DateTime.UtcNow;}else{return;}if(ř==DisplayState.StatusOverview){if(Ś.Count-1!=Ŝ){Ŝ++;}}else if(ř==
DisplayState.CommandScreen){if(Ŵ.Count-1!=ŝ){ŝ++;}}else if(ř==DisplayState.NavigateScreen){if(ź.Count-1!=Ş){Ş++;}}else if(ř==
DisplayState.DriveSettings){if(ż.maxCursorIndex!=ż.cursorIndex){ż.cursorIndex++;}}Ö();break;case"4":if((DateTime.UtcNow-š).
TotalSeconds>ő){ş[3]=true;š=DateTime.UtcNow;}else{return;}if(ř==DisplayState.CommandScreen&&Ś.Exists(M=>M.isSelected)){ū=
DisplayState.StatusOverview;Ż.Clear();ź.ForEach(M=>M.isChoosen=false);}if(ř==DisplayState.NavigateScreen){ū=DisplayState.
CommandScreen;Ż.Clear();ź.ForEach(M=>M.isChoosen=false);}if(ř==DisplayState.DriveSettings){ū=DisplayState.NavigateScreen;}Ö();break;
case"5":if((DateTime.UtcNow-š).TotalSeconds>ő){ş[4]=true;š=DateTime.UtcNow;}else{return;}if(ř==DisplayState.StatusOverview&&
Ś.Exists(M=>M.isSelected)){ū=DisplayState.CommandScreen;}else if(ř==DisplayState.CommandScreen){if(ŝ==0){ū=DisplayState.
NavigateScreen;ź.ForEach(M=>M.isChoosen=false);Ż.Clear();}else if(ŝ==1){ū=DisplayState.DriveSettings;ż.interruptType=DriveSettings.
InterruptType.ReplaceAll;Ż.Clear();Ż.Add(ź.Find(M=>M.address==IGC.Me));ź.Find(M=>M.address==IGC.Me).isChoosen=true;}else{Ô(Ŵ[ŝ].Õ);Ŵ[
ŝ].ŧ=ť.Ũ.ũ;Ŷ=DateTime.Now;}}else if(ř==DisplayState.NavigateScreen){if(Ż.Count>0){ū=DisplayState.DriveSettings;ż.
interruptType=DriveSettings.InterruptType.ReplaceAll;}}else if(ř==DisplayState.DriveSettings){µ();Ŵ[ŝ].ŧ=ť.Ũ.ũ;Ŷ=DateTime.Now;}Ö();
break;default:break;}}for(int F=0;F<ş.Length;F++){if((DateTime.UtcNow-š).TotalSeconds>ő){if(F==3&&ş[3]){ř=ū;}else if(F==4&&ş[
4]){ř=ū;}ş[F]=false;}}}void ķ(){List<IMyRadioAntenna>ĸ=new List<IMyRadioAntenna>();GridTerminalSystem.GetBlocksOfType<
IMyRadioAntenna>(ĸ);Š.Clear();List<IMyTextSurfaceProvider>Ĺ=new List<IMyTextSurfaceProvider>();GridTerminalSystem.GetBlocksOfType<
IMyTextSurfaceProvider>(Ĺ);foreach(var ĺ in Ĺ){IMyTerminalBlock Ļ=ĺ as IMyTerminalBlock;if(Ļ!=null&&Ļ.CustomName.Contains(SCREEN_TAG)&&Ļ.
CubeGrid==Me.CubeGrid){MyIni ļ=new MyIni();if(!ļ.TryParse(Ļ.CustomData))ļ.Clear();bool ľ=false;List<string>Ń=new List<string>();
for(int F=0;F<ĺ.SurfaceCount;F++){string Ŀ=$"{ĺ.GetSurface(F).DisplayName}_{F}";Ń.Add(Ŀ);string þ=ļ.Get("Screens",Ŀ).
ToString("");if(!ļ.ContainsKey("Screens",Ŀ)){þ=(F==0)?"X":"";ļ.Set("Screens",Ŀ,þ);ľ=true;}IMyTextSurface ŀ=ĺ.GetSurface(F);if(þ.
Trim().Equals("X",StringComparison.OrdinalIgnoreCase)){ŀ.ContentType=ContentType.SCRIPT;ŀ.Script="";ŀ.ScriptBackgroundColor=
Color.Black;Š.Add(new ġ(ŀ));}else if(ŀ.ContentType==ContentType.SCRIPT){ŀ.ContentType=ContentType.NONE;ŀ.BackgroundColor=
Color.Cyan;ŀ.WriteText("");}}List<MyIniKey>Ł=new List<MyIniKey>();ļ.GetKeys("Screens",Ł);foreach(var ł in Ł){if(!Ń.Contains(ł
.Name)){ļ.Delete(ł.Section,ł.Name);ľ=true;}}if(ľ){Ļ.CustomData=ļ.ToString();}}}if(ĸ.Count!=0){Œ=ĸ[0];œ.Add(IGC.
RegisterBroadcastListener(IGC_TAG));Ř=true;}else{Echo("Add antenna");}ŕ.Clear();List<IMyShipConnector>ń=new List<IMyShipConnector>();var Ľ=
GridTerminalSystem.GetBlockGroupWithName(BLOCK_TAG);if(Ľ!=null){Ľ.GetBlocksOfType<IMyShipConnector>(ń);ń.ForEach(M=>{ŕ.Add(new Connector(M
.GetPosition(),!M.IsConnected,M,ô(M)));});}Ŋ("",MessageType.StatusRequest,IGC_TAG,TransmissionDistance.
TransmissionDistanceMax);destinationName=Me.CubeGrid.CustomName;ŷ=new Destination(destinationName,Me.GetPosition(),IGC.Me,DateTime.Now);ź.Add(ŷ
);ı();}void ı(){Ŵ.Clear();Ŵ.Add(new ť(Ĥ.ĥ,"","Plan a route with one or more destinations."));Ŵ.Add(new ť(Ĥ.Ħ,"",
$"Summon to current location:\n{destinationName}"));Ŵ.Add(new ť(Ĥ.Ĩ,"","Resume current route if agent is paused."));Ŵ.Add(new ť(Ĥ.ħ,"",
"Stop agent at current location\nwithout clearing its route."));Ŵ.Add(new ť(Ĥ.ĩ,"","Cancels a route and stops the agent."));}List<Message>ĭ(){List<Message>Į=new List<Message>();for(
int F=0;F<œ.Count;F++){var į=œ[F];while(į.HasPendingMessage){var İ=į.AcceptMessage();var Ĭ=İ.Data as string;if(string.
IsNullOrEmpty(Ĭ))continue;string[]Ĳ=Ĭ.Split('^');if(Ĳ.Length<4)continue;Vector3D I;Vector3D.TryParse(Ĳ[2],out I);MessageType ĳ;if(!
Enum.TryParse(Ĳ[1],out ĳ))continue;long Ĵ;long.TryParse(Ĳ[3],out Ĵ);Į.Add(new Message{Type=ĳ,Content=Ĳ[0],Address=Ĵ,Position
=I,RecievedTime=DateTime.UtcNow});}}return Į;}List<Message>ĵ(){var Ķ=IGC.UnicastListener;List<Message>Į=new List<Message>
();while(Ķ.HasPendingMessage){var İ=Ķ.AcceptMessage();var Ĭ=İ.Data as string;if(string.IsNullOrEmpty(Ĭ))continue;string[]
Ĳ=Ĭ.Split('^');if(Ĳ.Length<4)continue;Vector3D I;Vector3D.TryParse(Ĳ[2],out I);MessageType ĳ;if(!Enum.TryParse(Ĳ[1],out ĳ
))continue;long Ĵ;long.TryParse(Ĳ[3],out Ĵ);Į.Add(new Message{Type=ĳ,Content=Ĳ[0],Address=Ĵ,Position=I,RecievedTime=
DateTime.UtcNow});}return Į;}void Ŋ(string º,MessageType ĳ,string ŋ,TransmissionDistance Ō){List<string>ō=new List<string>();ō.
Add(º);ō.Add(ĳ.ToString());ō.Add(Me.GetPosition().ToString());ō.Add(IGC.Me.ToString());string Ŏ=string.Join("^",ō);IGC.
SendBroadcastMessage(ŋ,Ŏ,Ō);}void ŏ(string º,MessageType ĳ,string ŋ,long Ő,Vector3D Ą){List<string>ō=new List<string>();ō.Add(º);ō.Add(ĳ.
ToString());ō.Add(Ą.ToString());ō.Add(IGC.Me.ToString());string Ŏ=string.Join("^",ō);IGC.SendUnicastMessage(Ő,ŋ,Ŏ);}void ŉ(){try
{List<Message>Į=ĭ();if(Į.Any()){foreach(var É in Į){if(É.Type==MessageType.DestinationRequest){ŏ(destinationName,
MessageType.DestinationAddress,IGC_TAG,É.Address,Me.GetPosition());Ŕ.Add($"Message sent to: {É.Address}");}else if(É.Type==
MessageType.StatusUpdate){Ï(É);}}}List<Message>Ņ=ĵ();if(Ņ.Any()){foreach(var É in Ņ){if(É.Type==MessageType.DestinationConnector){
bool ņ=true;bool.TryParse(É.Content,out ņ);if(ŕ.Count>0&&ŕ.Exists(M=>M.available&&M.isSmallPort==ņ)){Connector Æ=ŕ.Find(M=>M
.available&&M.isSmallPort==ņ);Æ.available=false;Æ.reservedAddres=É.Address;Æ.lastPing=DateTime.UtcNow;ŏ(
$"Available|{Æ.connector.WorldMatrix.Forward}",MessageType.DestinationConnector,IGC_TAG,É.Address,Æ.position);Ŕ.Add(
$"Message sent to [DestinationConnector]: {É.Address}");}else{ŏ("Unavailable",MessageType.DestinationConnector,IGC_TAG,É.Address,Me.GetPosition());Ŕ.Add(
$"Message sent to [DestinationConnector]: {É.Address}");}}else if(É.Type==MessageType.DestinationAddress){Destination Ň=new Destination(É.Content,É.Position,É.Address,É.
RecievedTime);if(!ź.Exists(M=>M.address==Ň.address)){ź.Add(Ň);}else{ź.Find(M=>M.address==Ň.address).lastPing=Ň.lastPing;ź.Find(M=>M.
address==Ň.address).name=Ň.name;if(Ż.Exists(M=>M.address==Ň.address))Ż.Find(M=>M.address==Ň.address).name=Ň.name;}}else if(É.
Type==MessageType.DestinationConnectorDisconnect){Vector3D I=new Vector3D();Vector3D.TryParse(É.Content,out I);double ň=
double.MaxValue;int z=0;int F=0;foreach(var Æ in ŕ){if(Æ.connector.IsConnected)Æ.available=false;double f=Vector3D.Distance(Æ.
connector.GetPosition(),I);if(f<ň){ň=f;z=F;}F++;}ŕ[z].available=true;Ŕ.Add($"Connector freed");}else if(É.Type==MessageType.
ParkingPing){ŕ.Find(M=>M.reservedAddres==É.Address).lastPing=DateTime.UtcNow;}else if(É.Type==MessageType.StatusUpdate){Ï(É);}else
if(É.Type==MessageType.CommandConfirmed){if(É.Content=="Y")Ś.Find(M=>M.agentAdress==É.Address).commandStatus=AgentStatus.
CommandStatus.Confirmed;else if(É.Content=="N")Ś.Find(M=>M.agentAdress==É.Address).commandStatus=AgentStatus.CommandStatus.Rejected;}
}}foreach(var Æ in ŕ){if((DateTime.UtcNow-Æ.lastPing).TotalSeconds>pingFrequency+5&&!Æ.available&&!Æ.connector.
IsConnected){Æ.available=true;ŏ("Unavailable",MessageType.DestinationConnector,IGC_TAG,Æ.reservedAddres,Me.GetPosition());Ŕ.Add(
$"Unavailable [Ping]: {Æ.reservedAddres}");Æ.reservedAddres=0;}else if((DateTime.UtcNow-Æ.lastPingSent).TotalSeconds>pingFrequency/2&&!Æ.available&&!Æ.connector.
IsConnected){ŏ("Ping",MessageType.ParkingPing,IGC_TAG,Æ.reservedAddres,Me.GetPosition());Æ.lastPingSent=DateTime.UtcNow;Ŕ.Add(
$"[Ping]: {Æ.reservedAddres}");}}}catch(Exception ex){Echo(ex.Message);}if((DateTime.UtcNow-Ź).TotalMinutes>Ÿ){Ŋ("v",MessageType.DestinationRequest,
IGC_TAG,TransmissionDistance.TransmissionDistanceMax);Ź=DateTime.UtcNow;}List<Destination>Ç=new List<Destination>();foreach(var
S in ź){if((DateTime.UtcNow-S.lastPing).TotalMinutes>Ÿ*2&&S.address!=IGC.Me){Ç.Add(S);}}foreach(var S in Ç){ź.Remove(S);}
}void Ï(Message É){string[]Ê=É.Content.Split(';');if(Ê.Length<7)return;string Ë=Ê[0];string Ì=Ê[1];float Í=float.Parse(Ê[
2]);float Î=float.Parse(Ê[3]);double Ð=double.Parse(Ê[4]);int È=int.Parse(Ê[5]);int Å=int.Parse(Ê[6]);string ª=Ê[7];
double f=0;double u=0;double v=0;double w=0;if(Ê.Length>8)double.TryParse(Ê[8],out f);if(Ê.Length>9)double.TryParse(Ê[9],out u
);if(Ê.Length>10)double.TryParse(Ê[10],out v);if(Ê.Length>11){double.TryParse(Ê[11],out w);}AgentStatus y=new AgentStatus
(Ë,Ì,Í,Î,Ð,È,Å,f,u,v,w,ª,É.Address,DateTime.Now,AgentStatus.CommandStatus.Waiting);int z=Ś.FindIndex(M=>M.agentAdress==É.
Address);if(z!=-1){AgentStatus Ä=Ś[z];y.prevStatus=Ä;y.commandStatus=Ś[z].commandStatus;y.isSelected=Ś[z].isSelected;Ś[z]=y;}
else{Ś.Add(y);}}void µ(){string º=Á(Ż,ż);foreach(var À in Ś){if(À.isSelected){ŏ(º,MessageType.Command,IGC_TAG,À.agentAdress,
Me.GetPosition());Echo(º);Me.CustomData=º;}}}string Á(List<Destination>Â,DriveSettings Ã){if(Â==null||Â.Count==0)return"";
string t=string.Join(";",Â.ConvertAll(M=>M.name));List<string>Ñ=new List<string>();Ñ.Add($"Collision={Ã.collisionType}");Ñ.Add
($"Parking={Ã.parkingType}");Ñ.Add($"Wait={Ã.waitType}");Ñ.Add($"Navigation={Ã.navigationType}");Ñ.Add(
$"Interrupt={Ã.interruptType}");string Ó=string.Join(";",Ñ);return$"Navigate:{t}:{Ó}";}void Ô(Ĥ Õ){string º="";switch(Õ){case Ĥ.ĥ:break;case Ĥ.Ħ:break
;case Ĥ.ħ:º="stop";break;case Ĥ.Ĩ:º="resume";break;case Ĥ.ĩ:º="cancel";break;default:break;}foreach(var À in Ś.FindAll(M
=>M.isSelected)){ŏ(º,MessageType.Command,IGC_TAG,À.agentAdress,Me.GetPosition());}}void Ö(){foreach(var A in Š){var O=A.ī.
DrawFrame();switch(ř){case DisplayState.StatusOverview:Ø(O,A);break;case DisplayState.CommandScreen:Ò(O,A);break;case
DisplayState.NavigateScreen:Ù(O,A);break;case DisplayState.DriveSettings:X(O,A);break;default:break;}}}void Ø(MySpriteDrawFrame O,ġ
A){RectangleF T=A.T;Vector2 U=new Vector2(T.Width*0.25f,110);Ś.RemoveAll(M=>(DateTime.Now-M.lastPing).TotalSeconds>
pingFrequency*2);if(Ś.Count>0){int H=Math.Max(1,(int)Math.Round((T.Height-110)/100));A.ţ=Ŝ/H;int B=(Ś.Count+H-1)/H;A.ţ=Math.Max(0,
Math.Min(A.ţ,B-1));int C=A.ţ*H;int D=Math.Min(C+H,Ś.Count);for(int F=C;F<D;F++){bool P=(Ŝ==F);bool W=(F==D-1);N(O,A,U,Ś[F],P
,Ś[F].isSelected,W);U.Y+=100;}ā(O,A,new Vector2(T.Width/2,40),0.7f,$"{A.ţ+1}/{B}",Color.Yellow,TextAlignment.CENTER);}
else{ā(O,A,T.Center,1,"No agents found",Color.Yellow);ā(O,A,new Vector2(T.Center.X,T.Center.Y+15),0.8f,
"Add at least one RoadPilot\nAgent script",Color.White);}Ü(O,A,"Status overview");Ē(O,A,"Select",$"Up","Down","Map","Action");if(ŕ.Count>0)ĝ(O,A);O.Dispose();}
void Ò(MySpriteDrawFrame O,ġ A){RectangleF T=A.T;Vector2 U=new Vector2(T.Width*0.25f,130);ā(O,A,new Vector2(T.Width/2,T.
Height*0.75f),0.6f,$"{Ś.FindAll(M=>M.isSelected).Count} agent(s) selected",Color.Yellow,TextAlignment.CENTER);int H=Math.Max(1
,(int)Math.Round((T.Height-110)/60));A.ţ=ŝ/H;int B=(Ŵ.Count+H-1)/H;A.ţ=Math.Max(0,Math.Min(A.ţ,B-1));int C=A.ţ*H;int D=
Math.Min(C+H,Ŵ.Count);for(int F=C;F<D;F++){bool P=(ŝ==F);bool W=(F==D-1);c(O,A,U,Ŵ[F],P,Ŵ[F].P,W);U+=60;}ā(O,A,new Vector2(T
.Width/2,40),0.7f,$"{A.ţ+1}/{B}",Color.Yellow,TextAlignment.CENTER);Ü(O,A,"Actions");Ē(O,A,"Select",$"Up","Down","Status"
,"Next");if(Ŵ[ŝ].ŧ==ť.Ũ.ũ){ć(O,A,T.Center);}O.Dispose();}void Ù(MySpriteDrawFrame O,ġ A){RectangleF T=A.T;Vector2 U=new
Vector2(T.Width*0.25f,130);ā(O,A,new Vector2(T.Width/2,T.Height*0.75f),0.6f,
$"{Ś.FindAll(M=>M.isSelected).Count} agent(s) selected",Color.Yellow,TextAlignment.CENTER);int H=Math.Max(1,(int)Math.Round((T.Height-110)/60));A.ţ=Ş/H;int B=(ź.Count+H-1)/H;A
.ţ=Math.Max(0,Math.Min(A.ţ,B-1));int C=A.ţ*H;int D=Math.Min(C+H,ź.Count);for(int F=C;F<D;F++){bool P=(Ş==F);bool W=(F==D-
1);e(O,A,U,ź[F],P,ź[F].isChoosen,W);U+=60;}ā(O,A,new Vector2(T.Width/2,40),0.7f,$"{A.ţ+1}/{B}",Color.Yellow,TextAlignment
.CENTER);Ü(O,A,"Navigate");Ē(O,A,"Select",$"Up","Down","Actions","Next");O.Dispose();}void X(MySpriteDrawFrame O,ġ A){
RectangleF T=A.T;float Y=80f;float V=130f;int R=5;int H=Math.Max(1,(int)Math.Round((T.Height-V-40)/Y));A.ţ=ż.cursorIndex/H;int B=(
R+H-1)/H;A.ţ=Math.Max(0,Math.Min(A.ţ,B-1));int C=A.ţ*H;int D=Math.Min(C+H,R);int E=0;for(int F=C;F<D;F++){float G=V+(E*Y)
-50;Vector2 I=new Vector2(T.Width*0.05f,G+50);bool P=(ż.cursorIndex==F);if(P){float J=G-25;O.Add(new MySprite(){Type=
SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="AH_BoreSight",Position=new Vector2(T.X+15f,T.Y+I.Y),Size=new Vector2(25,25
),Color=Color.White,});}bool K=true;if(Ż.Count<2)K=false;switch(F){case 0:à<DriveSettings.NavigationType>(O,A,I,T.Width*
0.9f,10,(int)ż.navigationType);break;case 1:à<DriveSettings.CollisionType>(O,A,I,T.Width*0.9f,10,(int)ż.collisionType);break
;case 2:à<DriveSettings.ParkingType>(O,A,I,T.Width*0.9f,10,(int)ż.parkingType);break;case 3:List<int>L=new List<int>();if
(Ż.Count<=1||ż.parkingType!=DriveSettings.ParkingType.Connector){if(!L.Contains((int)DriveSettings.WaitType.Recharge))L.
Add((int)DriveSettings.WaitType.Recharge);if(!L.Contains((int)DriveSettings.WaitType.Cargo))L.Add((int)DriveSettings.
WaitType.Cargo);}if(Ż.Count<=1){if(!L.Contains((int)DriveSettings.WaitType.Timerblock))L.Add((int)DriveSettings.WaitType.
Timerblock);if(!L.Contains((int)DriveSettings.WaitType.External))L.Add((int)DriveSettings.WaitType.External);}à<DriveSettings.
WaitType>(O,A,I,T.Width*0.9f,10,(int)ż.waitType,L);break;case 4:à<DriveSettings.InterruptType>(O,A,I,T.Width*0.9f,10,(int)ż.
interruptType);break;}E++;}ā(O,A,new Vector2(T.Width/2,T.Height*0.75f),0.6f,$"{Ś.FindAll(M=>M.isSelected).Count} agent(s) selected",
Color.Yellow,TextAlignment.CENTER);ā(O,A,new Vector2(T.Width/2,40),0.7f,$"{A.ţ+1}/{B}",Color.Yellow,TextAlignment.CENTER);Ü(O
,A,"Settings");Ē(O,A,"Select","Up","Down","Navigate","Send");if(Ŵ[ŝ].ŧ==ť.Ũ.ũ){ć(O,A,T.Center);}O.Dispose();}void N(
MySpriteDrawFrame O,ġ A,Vector2 Q,AgentStatus Z,bool P,bool b,bool W){RectangleF T=A.T;float J=T.Y+Q.Y;Color d=P?Color.Yellow:Color.White
;if(P){O.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="AH_BoreSight",Position=new
Vector2(T.X+15f,J),Size=new Vector2(25,25),Color=Color.White,});}if(!W)O.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=
TextAlignment.CENTER,Data="SquareSimple",Position=new Vector2(T.Center.X,J+35f),Size=new Vector2(T.Width,2.5f),Color=new Color(255,
255,255,50),});ā(O,A,new Vector2(T.X+T.Width*0.05f,Q.Y-50),0.7f,Z.agentName,d,TextAlignment.LEFT);ā(O,A,new Vector2(T.X+T.
Width*0.4f,Q.Y-50),0.7f,Z.status,d,TextAlignment.LEFT);float h=T.X+(T.Width*0.05f);float j=50f;float k=Q.Y;float l=Q.Y+15f;
Vector2 m=new Vector2(h+15f,k);ñ(O,A,"IconEnergy",m,Color.White,new Vector2(20,20),TextAlignment.CENTER);ñ(O,A,"CircleHollow",m
,Color.White,new Vector2(30,30),TextAlignment.CENTER);ā(O,A,new Vector2(m.X,l),0.5f,$"{Z.batteryPercentage:F0}%",Color.
White,TextAlignment.CENTER);Vector2 n=new Vector2(h+15f+j,k);ñ(O,A,"Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds",n,
Color.White,new Vector2(20,20),TextAlignment.CENTER);ñ(O,A,"CircleHollow",n,Color.White,new Vector2(30,30),TextAlignment.
CENTER);ā(O,A,new Vector2(n.X,l),0.5f,$"{Z.cargoPercentage:F0}%",Color.White,TextAlignment.CENTER);Vector2 o=new Vector2(h+15f
+(j*2),k);ñ(O,A,"Textures\\FactionLogo\\Others\\OtherIcon_22.dds",o,Color.White,new Vector2(20,20),TextAlignment.CENTER);
ñ(O,A,"CircleHollow",o,Color.White,new Vector2(30,30),TextAlignment.CENTER);ā(O,A,new Vector2(o.X,l),0.5f,
$"{Z.speed:F0}km/h",Color.White,TextAlignment.CENTER);if(b){ñ(O,A,"AH_BoreSight",new Vector2(T.Width-50,m.Y),Color.Yellow,new Vector2(25,25
),TextAlignment.CENTER);ñ(O,A,"CircleHollow",new Vector2(T.Width-50,m.Y),Color.White,new Vector2(30,30),TextAlignment.
CENTER);}if(Z.prevStatus!=null){if((Z.status.Contains("Driving")||Z.status.Contains("Getting"))&&Z.speed<1&&Z.prevStatus.speed
<1){ñ(O,A,"Danger",new Vector2(o.X+15,o.Y-15),Color.White,new Vector2(15,15));}}TimeSpan p=TimeSpan.FromSeconds(Z.
etaSeconds);string q;string r;if(p.TotalHours>=1){q=$"{p.Hours} Hr {p.Minutes} Mins";}else if(p.Minutes>0){q=$"{p.Minutes} Mins";}
else{q=$"1 Min";}if(Z.etaDistance/1000<1000&&Z.etaDistance>0){r=$"{Math.Max(Math.Round(Z.etaDistance/50)*50,50)} m";}else if
(Z.etaDistance>0){r=$"{Math.Abs(Z.etaDistance).ToString("F2")} km";}else{r=$"50 m";}if(Z.choosenDestinationCount-1==Z.
choosenDestinationIndex){h=T.X+(T.Width*0.4f)*1.5f;ë(O,A,new Vector2(h,k),new Vector2(T.Width*0.4f,10),Color.Cyan,Color.White,(float)Z.
etaTotalDistance,(float)Z.etaTotalDistance-(float)Z.etaDistance);ñ(O,A,"SquareSimple",new Vector2(T.X+(T.Width*0.4f),k),Color.White,new
Vector2(10,22),TextAlignment.LEFT);ñ(O,A,"SquareSimple",new Vector2(T.X+(T.Width*0.4f)*2,k),Color.Yellow,new Vector2(10,22),
TextAlignment.CENTER);ā(O,A,new Vector2(T.X+(T.Width*0.6f),l),0.5f,$"{q} | {r} | {Z.navigationType}",Color.White);}else if(Z.
choosenDestinationCount-1>Z.choosenDestinationIndex){h=T.X+(T.Width*0.4f)*1.5f;ë(O,A,new Vector2(h,k),new Vector2(T.Width*0.4f,10),Color.Cyan,
Color.White,1,0);h=T.X+(T.Width*0.4f)*1.25f;ë(O,A,new Vector2(h,k),new Vector2(T.Width*0.2f,10),Color.Cyan,Color.White,(float
)Z.etaTotalDistance,(float)Z.etaTotalDistance-(float)Z.etaDistance);ñ(O,A,"SquareSimple",new Vector2(T.X+(T.Width*0.4f),k
),Color.White,new Vector2(10,22),TextAlignment.LEFT);ñ(O,A,"SquareSimple",new Vector2(T.X+(T.Width*0.4f)*2,k),Color.
Yellow,new Vector2(10,22),TextAlignment.CENTER);Vector2 s=new Vector2(T.X+(T.Width*0.6f),k);double g=(T.Width*0.15)/(Z.
choosenDestinationCount-1-Z.choosenDestinationIndex);for(int F=Z.choosenDestinationIndex;F<Z.choosenDestinationCount-1;F++){ñ(O,A,
"SquareSimple",s,Color.Yellow,new Vector2(10,22));s.X+=(float)g;}ā(O,A,new Vector2(T.X+(T.Width*0.6f),l),0.5f,
$"{q} | {r} | {Z.navigationType}",Color.White);}}void c(MySpriteDrawFrame O,ġ A,Vector2 Q,ť a,bool P,bool b,bool W){RectangleF T=A.T;float J=T.Y+Q.Y;
Color d=P?Color.Yellow:Color.White;if(P){O.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data=
"AH_BoreSight",Position=new Vector2(T.X+15f,J-40),Size=new Vector2(25,25),Color=Color.White,});}if(!W)O.Add(new MySprite(){Type=
SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=new Vector2(T.Center.X,J-10),Size=new Vector2(T.
Width,2.5f),Color=new Color(255,255,255,50),});ā(O,A,new Vector2(T.X+T.Width*0.05f,Q.Y-50),0.7f,$"{a.Õ}",d,TextAlignment.LEFT
);ā(O,A,new Vector2(T.X+T.Width*0.3f,Q.Y-50),0.5f,$"{a.Ţ}",d,TextAlignment.LEFT);}void e(MySpriteDrawFrame O,ġ A,Vector2
Q,Destination S,bool P,bool b,bool W){RectangleF T=A.T;float J=T.Y+Q.Y;Color d=P?Color.Yellow:Color.White;if(P){O.Add(new
MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="AH_BoreSight",Position=new Vector2(T.X+15f,J-40),Size=
new Vector2(25,25),Color=Color.White,});}if(!W)O.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,
Data="SquareSimple",Position=new Vector2(T.Center.X,J-10),Size=new Vector2(T.Width,2.5f),Color=new Color(255,255,255,50),});
ā(O,A,new Vector2(T.X+T.Width*0.05f,Q.Y-50),0.7f,$"{S.name}",d,TextAlignment.LEFT);if(Ż.Exists(M=>M.address==S.address)){
Destination Ď=Ż.Find(M=>M.address==S.address);ā(O,A,new Vector2(T.Width-50,Q.Y-45-5),0.7f,$"{Ż.IndexOf(Ď)+1}",Color.Yellow,
TextAlignment.CENTER);ñ(O,A,"CircleHollow",new Vector2(T.Width-50,Q.Y-50+10),Color.White,new Vector2(30,30),TextAlignment.CENTER);}}
void ď(MySpriteDrawFrame O,ġ A,Vector2 Q,DriveSettings Đ,bool P,bool b,bool W){RectangleF T=A.T;float J=T.Y+Q.Y;Color d=P?
Color.Yellow:Color.White;if(P){O.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data=
"AH_BoreSight",Position=new Vector2(T.X+15f,J-40),Size=new Vector2(25,25),Color=Color.White,});}if(!W)O.Add(new MySprite(){Type=
SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=new Vector2(T.Center.X,J-10),Size=new Vector2(T.
Width,2.5f),Color=new Color(255,255,255,50),});}void ć(MySpriteDrawFrame O,ġ A,Vector2 Q){RectangleF T=A.T;float Ć=T.Width*
0.58f;float ú=T.Height*0.78f;float Ĉ=Q.Y-T.Y;float č=Ĉ-(ú*0.35f);float ĉ=(T.Width/2f)-(Ć/2f)+15f;float Ċ=(T.Width/2f)+(Ć/2f)-
15f;ñ(O,A,"SquareSimple",new Vector2(T.Center.X,Ĉ),Color.White,new Vector2(T.Width*0.6f,T.Height*0.8f));ñ(O,A,
"SquareSimple",new Vector2(T.Center.X,Ĉ),Color.Black,new Vector2(Ć,ú));float ċ=Ś.FindAll(M=>M.isSelected).Count;float Č=Ś.FindAll(M=>M
.isSelected&&M.commandStatus==AgentStatus.CommandStatus.Confirmed).Count;float ą=č-20f;ā(O,A,new Vector2(ĉ,ą-10),0.6f,
$"Confirmed: {Č}/{ċ}",Color.Yellow,TextAlignment.LEFT);ë(O,A,new Vector2(T.Center.X,č),new Vector2(Ć*0.8f,10f),Color.Yellow,Color.White,Č,ċ);
float Y=25f;float đ=č+25f;var ě=Ś.FindAll(M=>M.isSelected);foreach(var À in ě){if(đ>Ĉ+(ú/2f)-15f)break;Color Ĝ=Color.White;
string Ì="WAITING";switch(À.commandStatus){case AgentStatus.CommandStatus.Waiting:break;case AgentStatus.CommandStatus.
Confirmed:Ĝ=Color.Yellow;Ì="CONFIRMED";break;case AgentStatus.CommandStatus.Rejected:Ĝ=Color.Red;Ì="REJECTED";break;}string Ë=À.
agentName.Length>15?À.agentName.Substring(0,12)+"...":À.agentName;ā(O,A,new Vector2(ĉ,đ),0.6f,Ë,Color.White,TextAlignment.LEFT);ā
(O,A,new Vector2(Ċ,đ),0.6f,Ì,Ĝ,TextAlignment.RIGHT);đ+=Y;}if((DateTime.Now-Ŷ).TotalSeconds>ŵ||(ċ==Č&&(DateTime.Now-Ŷ).
TotalSeconds>1.5f)||(Ś.FindAll(M=>M.commandStatus==AgentStatus.CommandStatus.Rejected).Count==ċ&&(DateTime.Now-Ŷ).TotalSeconds>1.5f)
){ř=DisplayState.StatusOverview;Ś.ForEach(M=>{M.isSelected=false;M.commandStatus=AgentStatus.CommandStatus.Waiting;});Ŵ[ŝ
].ŧ=ť.Ũ.Ū;ź.ForEach(M=>M.isChoosen=false);}}void ĝ(MySpriteDrawFrame O,ġ A){RectangleF T=A.T;int Ğ=ŕ.Count;if(Ğ==0)return
;float ğ=10f;float j=15f;float Ġ=T.Width*0.05f+T.X;float Ě=32;ā(O,A,new Vector2(Ġ-3,10),0.5f,"Connectors",Color.White,
TextAlignment.LEFT);foreach(var Æ in ŕ){Color d=Color.White;if(Æ.connector.IsConnected){d=Color.Green;}else if(!Æ.available&&!Æ.
connector.IsConnected){d=Color.Yellow;}ñ(O,A,"Circle",new Vector2(Ġ,Ě),d,new Vector2(ğ,ğ));Ġ+=j;}}void Ē(MySpriteDrawFrame O,ġ A,
string ē="",string Ĕ="",string ĕ="",string Ė="",string ę=""){RectangleF T=A.T;Ģ[]ė=new Ģ[5]{new Ģ{ģ="SquareHollow",Ī=Color.
Black},new Ģ{ģ="SquareHollow",Ī=Color.White},new Ģ{ģ="SquareHollow",Ī=Color.White},new Ģ{ģ="SquareHollow",Ī=Color.White},new
Ģ{ģ="SquareHollow",Ī=Color.White}};for(int F=0;F<ė.Length;F++){if(ş[F]){ė[F].ģ="SquareSimple";ė[F].Ī=Color.White;}}Ę(O,A,
new Vector2(T.Width*0.2f,T.Height*0.92f),new Vector2(T.Width*0.2f,40),Color.White,ė[0].Ī,$"1: {ē}",ş[0]);Ę(O,A,new Vector2(
(T.Width*0.2f)*2+(T.Width*0.1f),T.Height*0.92f),new Vector2(T.Width*0.2f,40),Color.White,ė[1].Ī,$"2: {Ĕ}",ş[1]);Ę(O,A,new
Vector2((T.Width*0.2f)*3.5f+(T.Width*0.1f),T.Height*0.92f),new Vector2(T.Width*0.2f,40),Color.White,ė[2].Ī,$"3: {ĕ}",ş[2]);if(ř
!=DisplayState.StatusOverview){Ę(O,A,new Vector2(-2,32),new Vector2(T.Width*0.5f,40),ė[3].Ī,Color.White,$"",ş[3]);ñ(O,A,
"AH_BoreSight",new Vector2(T.Width*0.05f,32),ė[3].Ī,new Vector2(40,40),TextAlignment.CENTER,(float)Math.PI);ā(O,A,new Vector2(T.Width*
0.09f,20),0.7f,$"4: {Ė}",ė[3].Ī,TextAlignment.LEFT);}Color d=Color.Gray;if((ř==DisplayState.StatusOverview&&Ś.Exists(M=>M.
isSelected))||ř==DisplayState.CommandScreen||(ř==DisplayState.NavigateScreen&&Ż.Count>0)||ř==DisplayState.DriveSettings){d=Color.
Yellow;}Ę(O,A,new Vector2(T.Width+2,32),new Vector2(T.Width*0.5f,40),d,Color.White,$"",ş[4]);ñ(O,A,"AH_BoreSight",new Vector2(
T.Width*0.95f,32),d,new Vector2(40,40),TextAlignment.CENTER);ā(O,A,new Vector2(T.Width*0.72f+30+T.X,20),0.7f,$"5: {ę}",ė[
4].Ī,TextAlignment.LEFT);}void Ę(MySpriteDrawFrame O,ġ A,Vector2 Ą,Vector2 ì,Color ä,Color å,string æ,bool ç=false,
TextAlignment è=TextAlignment.CENTER){RectangleF T=A.T;Color ê=ä;Color í=Color.Black;if(ç){ê=Color.Black;í=å;}ñ(O,A,"SemiCircle",new
Vector2(Ą.X-ì.X/2+5,Ą.Y),ä,new Vector2(ì.Y,ì.Y),TextAlignment.CENTER,-(float)Math.PI/2);ñ(O,A,"SemiCircle",new Vector2(Ą.X+ì.X/
2-5,Ą.Y),ä,new Vector2(ì.Y,ì.Y),TextAlignment.CENTER,(float)Math.PI/2);ñ(O,A,"SquareSimple",Ą,ä,ì);ñ(O,A,"SemiCircle",new
Vector2(Ą.X-ì.X/2+5,Ą.Y),í,new Vector2(ì.Y-3,ì.Y-3),TextAlignment.CENTER,-(float)Math.PI/2);ñ(O,A,"SemiCircle",new Vector2(Ą.X+
ì.X/2-5,Ą.Y),í,new Vector2(ì.Y-3,ì.Y-3),TextAlignment.CENTER,(float)Math.PI/2);ñ(O,A,"SquareSimple",Ą,í,new Vector2(ì.X-3
,ì.Y-3));ā(O,A,new Vector2(Ą.X,Ą.Y-15),0.8f,æ,ê,è);}void ë(MySpriteDrawFrame O,ġ A,Vector2 I,Vector2 ì,Color å,Color î,
float é,float ã,float Þ=0){ã=Math.Min(ã,é);RectangleF T=A.T;Vector2 Û=new Vector2((ã/é)*ì.X,ì.Y);ñ(O,A,"SquareSimple",I,î,ì,
TextAlignment.CENTER,Þ);ñ(O,A,"SquareSimple",new Vector2(I.X-(ì.X/2),I.Y),å,Û,TextAlignment.LEFT,Þ);}void Ü(MySpriteDrawFrame O,ġ A,
string Ý){RectangleF T=A.T;Vector2 â=new Vector2(T.Width/2f,10f);ā(O,A,â,0.9f,Ý,Color.White,TextAlignment.CENTER);Vector2 ß=
new Vector2(T.Center.X,T.Y+37f);O.Add(new MySprite(){Type=SpriteType.TEXTURE,Data="SquareSimple",Position=ß,Size=new
Vector2(50,2.5f),Color=Color.LightGray,Alignment=TextAlignment.CENTER});}void à<á>(MySpriteDrawFrame O,ġ A,Vector2 I,float Ú,
float g,int ï,List<int>ö=null){var ø=Enum.GetValues(typeof(á)).Cast<á>().ToList();if(ø.Count==0||ø.Count>6)return;bool ù=ö!=
null&&ö.Count>=ø.Count-1;float ú=10f;float û=30f;float ü=55f;Vector2 ì=new Vector2((Ú-((ø.Count-1)*g))/ø.Count,ú);Vector2 ý=
new Vector2(I.X,I.Y);for(int F=0;F<ø.Count;F++){var þ=ø[F];bool ÿ=ö!=null&&ö.Contains(F);Color Ā=Color.White;if(ï==F){if(ù)
Ā=Color.DarkGray;else if(ÿ)Ā=Color.Red;else Ā=Color.Yellow;ā(O,A,new Vector2(I.X,I.Y-û),0.7f,þ.ToString(),Ā,TextAlignment
.LEFT);}else{if(ÿ)Ā=Color.DarkGray;else Ā=Color.White;}ñ(O,A,"SquareSimple",ý,Ā,ì,TextAlignment.LEFT);ý.X+=ì.X+g;}ā(O,A,
new Vector2(I.X,I.Y-ü),0.8f,typeof(á).Name,Color.White,TextAlignment.LEFT);}void ā(MySpriteDrawFrame O,ġ A,Vector2 Ă,float
ă,string æ,Color d,TextAlignment ó=TextAlignment.CENTER){Vector2 I=new Vector2(A.T.X+Ă.X,A.T.Y+Ă.Y);var ð=new MySprite{
Type=SpriteType.TEXT,Data=æ,Position=I,RotationOrScale=ă,Color=d,Alignment=ó,FontId="White"};O.Add(ð);}void ñ(
MySpriteDrawFrame O,ġ A,string ò,Vector2 I,Color d,Vector2 ì,TextAlignment ó=TextAlignment.CENTER,float Þ=0){RectangleF T=A.T;var ð=new
MySprite(){Type=SpriteType.TEXTURE,Data=ò,Position=I+T.Position,Size=ì,Color=d,RotationOrScale=Þ,Alignment=ó};O.Add(ð);}bool ô(
IMyShipConnector Æ){string õ=Æ.BlockDefinition.SubtypeId;return õ=="LargeBlockInsetConnectorSmall";}