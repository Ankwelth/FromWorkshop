/*
 * R e a d m e
 * -----------
 * RoadPilot Agent script
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
 * 
 * 
 * 
 * 
 */

//Script parameters (editing allowed)
string IGC_TAG = "IntraCar"; //All RoadPilot scripts with the same IGC_TAG will be able to communicate.
string SCREEN_TAG = "[RoadPilot]"; //Tag used to identify screen blocks. Any block with a functional screen will be detected.
float MAX_SPEED = 25; //The maximum speed the agent could drive.
float MIN_SPEED = 3; //The minimum speed the agent could drive.
double MAX_PROPULSION = 0.25; //The maximum propulsion override for the wheels. Note: Value between 0 and 1.
float STEERING_ANGLE = 46; //The maximum steering angle the wheels can turn (default = 46)
float STEERING_MULTIPLICATOR = 1.25f; //Values above 1 result in harsher steering, Values below 1 result in softer steering
float LANE_WIDTH = 0.5f; //Determines the offset to the right perpendicular to the nodes
float CREEP_DISTANCE = 15; //Distance the agent will creep forward before reversing into a parking (default = 15)
float TURNAROUND_PATHFINDING_PENALTY = 2000; //Penality added during pathfinding to avoid routes that require a turnaround to start

//You can rename these if you want, they are case-sensitive
string blockTag = "[RoadPilot]";
string wheelTag = "Wheels";
string stopLightTag = "Stop Lights";
string turnLightTag = "Turn Lights";

//DO NOT TOUCH THIS UNLESS YOU WANT KLANG TO COME
public class Map
{
	public List<Node> nodes = new List<Node>();

	public Map()
	{
		nodes = new List<Node>();
	}
}
public class Node
{
	public bool isSelected;
	public Vector3D position;
	public List<Node> connectedNodes = new List<Node>();

	public Node(Vector3D pos)
	{
		position = pos;
		connectedNodes = new List<Node>();
	}
}
class Message
{
	public DateTime RecievedTime { get; set; }
	public string Content { get; set; }
	public MessageType Type { get; set; }
	public Vector3D Position { get; set; }
	public long Address { get; set; }
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
	public int maxCursorIndex = 3;
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
class Timer
{
	public IMyTimerBlock myTimer;
	public TimerType timerType = TimerType.ArrivalTimer;

	public Timer(IMyTimerBlock myTimer, TimerType timerType)
	{
		this.myTimer = myTimer;
		this.timerType = timerType;
	}

	public enum TimerType
	{
		ArrivalTimer,
		DepartureTimer,
	}
}
class Reservation
{
	public long sendTime;
	public long address;
	public Vector3D intersection;

	public Reservation(long sendTime, long address, Vector3D intersection)
	{
		this.sendTime = sendTime;
		this.address = address;
		this.intersection = intersection;
	}
}
class Notification
{
	public string title;
	public string body;
	public ProcessState processState;
	public Color color;
	public DateTime startProcessing;

	public enum ProcessState
	{
		UnProcessed,
		Processing,
		Processed
	}

	public Notification(string title, string body, Color color)
	{
		this.title = title;
		this.body = body;
		this.processState = ProcessState.UnProcessed;
		this.color = color;
	}
}
class Screen
{
	public IMyTextSurface textSurface;
	public RectangleF viewport;
	public int page;
	public float LerpY;

	public Screen(IMyTextSurface textSurface)
	{
		this.textSurface = textSurface;
		this.page = 0;
		Vector2 size = textSurface.SurfaceSize;
		Vector2 boarderOffset = (textSurface.TextureSize - size) * 0.5f;
		this.viewport = new RectangleF(boarderOffset, size);
		this.LerpY = -100f;
	}
}

public enum DisplayState
{
	AreaMap,
	Destination,
	DestinationSettings,
	Editing
}
public enum EditState
{
	PlaceTool,
	SelectTool,
	DeleteTool
}
public enum EditStatus
{
	Idle,
	UnlinkingNodes,
	DeletingNodes
}
public enum MessageType
{
	DestinationRequest,
	DestinationAddress,
	DestinationConnector,
	DestinationConnectorDisconnect,
	MapRequest,
	MapUnicast,
	MapBroadcast,
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
public enum SaveStatus
{
	UnloadingNodes,
	PopulatingConnectedNodes,
	Unloaded,
	Saving,
	Idle
}
public enum CollisionStatus
{
	Idle,
	StartCalculating,
	Calculating,
	PathFound,
	Blocked
}
public enum AutoDriveState
{
	AwaitingStart,
	Driving,
	Idle,
	Stopping,
	Parking_Stopping,
	Parking_Creep1,
	Parking_Creep2,
	Parking_Reverse1,
	Parking_Reverse2,
	Parking_Correction,
	Parking_Waiting,
	StuckStop,
	Reversing,
	RechargeWait,
	CargoWait,
	ExternalWait,
	StopForTurn,
	StartingTurn,
	PerformingTurn_1,
	PerformingTurn_2,
	PerformingTurn_3
}

public struct AvoidanceResult
{
	public Vector3D Direction;
	public float SpeedFactor;
}
public struct TooltipData
{
	public string SpriteName;
	public Color SpriteColor;
}

public class HitPoint
{
	public DateTime DetectedTime;
	public Vector3D Position;
	public long EntityId;
	public bool isMoving;
	public MyDetectedEntityType type;

	public HitPoint(DateTime time, Vector3D pos, long Id, bool moving, MyDetectedEntityType entityType)
	{
		DetectedTime = time;
		Position = pos;
		EntityId = Id;
		isMoving = moving;
		type = entityType;
	}
}

float Е=0.1f;float Д=10;float Г=10;float В=0.5f;float ƹ=200;float Ƹ=-200;float Ʒ=1000;float Ι=0.25f;float Θ=1f;float Η=
3.0f;float Ζ=150.0f;float Ε=25;float Δ=10;double Γ=10.0;float Β=1f;float Α=3.0f;double ΐ=10.0;double Ώ=3.0;double Ύ=3.0;
double Ό=0.5;const double Ί=15;const double Ή=(Ί/2.0)*(Ί/2.0);const double Έ=7.5;const float Ά=1.15f;const double ͽ=0.5;const
double ͼ=-0.2;const double ͻ=3.0;const float Κ=0.1f;const double ͺ=60.0;const double Λ=0.05;double ε=20.0;double γ=5.0;float β
=180;float α=4;float ΰ=60;float ί=100;float ή=2f;int έ=8;DateTime ά;float Ϋ=5;float Ϊ=5;float δ=2;bool Ω=false;
DriveSettings Χ;IMyShipController Φ;IMySensorBlock Υ;IMyTextPanel Τ;List<IMyTextPanel>Σ=new List<IMyTextPanel>();List<
IMyMotorSuspension>Ρ=new List<IMyMotorSuspension>();List<IMyLightingBlock>Π=new List<IMyLightingBlock>();DisplayState Ο=DisplayState.
AreaMap;EditState Ξ=EditState.PlaceTool;EditStatus Ν=EditStatus.Idle;int Μ=0;List<Node>Ψ=new List<Node>();SaveStatus ͷ=
SaveStatus.Idle;string[]ˑ;int ˀ=0;int ʿ=0;int ʾ=0;int ʽ=0;int Ȃ=20;Map ć=new Map();Map ʼ=new Map();RectangleF ʻ;bool ʺ=false;bool
ʹ=false;Dictionary<Vector3D,Node>ʸ=new Dictionary<Vector3D,Node>();float ʷ=0f;о ʶ;AutoDriveState ʵ=AutoDriveState.Idle;
bool ʴ=false;bool ʳ=false;float ʲ;List<Node>ʱ=new List<Node>();List<MyDetectedEntityInfo>ʰ=new List<MyDetectedEntityInfo>();
CollisionStatus ʯ=CollisionStatus.Idle;MyDetectedEntityInfo?ʮ=null;float ˁ=0f;DateTime ʭ;DateTime ˇ;int Ͷ=0;const int ͳ=5;const double
Ͳ=5.0;DateTime ͱ=DateTime.MinValue;Vector3D Ͱ,ˮ,ˬ=new Vector3D();IMySlimBlock ˤ,ˣ;int ˢ=0;int ˡ=0;int ʹ=0;float ˠ=0;List<
Node>ː=new List<Node>();List<Node>ˏ=new List<Node>();List<Node>ˎ=new List<Node>();Node Ƥ=null;Node ƣ=null;ȋ ˍ=new ȋ();
DateTime ˌ;List<Destination>ˋ=new List<Destination>();List<Destination>ˊ=new List<Destination>();int ˉ=0;bool ˆ;IMyRadioAntenna
ˈ;List<IMyBroadcastListener>ζ=new List<IMyBroadcastListener>();double Ϸ=20f;double ϴ=20f;double ϳ=10f;float ϲ=75;Vector3D
å;float ϱ=0;Node Ʀ=new Node(Vector3D.Zero);IMyCameraBlock ϰ;List<MyDetectedEntityInfo>ϯ=new List<MyDetectedEntityInfo>();
List<HitPoint>Ϯ=new List<HitPoint>();Vector3D ϭ=new Vector3D();int ϵ=1;Vector3D Ϭ;bool Ϫ;Vector3D ϩ;DateTime Ϩ;
IMyShipConnector ϧ;List<Timer>Ϧ=new List<Timer>();Vector3D ϥ;long Ϥ;bool ϣ=false;int Ϣ=0;DateTime ϡ;Vector3D ϫ;float ϸ=-1f;DateTime Є=
DateTime.MinValue;float Б=-1f;float Џ=-1f;int Ў=0;int Ѝ=0;List<Node>Ќ=new List<Node>();Reservation Ћ;List<Reservation>Њ=new List
<Reservation>();List<IMyLightingBlock>Љ=new List<IMyLightingBlock>();List<IMyLightingBlock>Ј=new List<IMyLightingBlock>()
;bool Ї;DateTime І;long А;DateTime Ѕ;Message Ѓ;bool[]Ђ={false,false,false,false,false};float Ё=0.1f;DateTime Ѐ;
DisplayState Ͽ;Vector2 Ͼ;Vector2 Ͻ;bool ϼ;float ϻ=0f;List<Notification>Ϻ=new List<Notification>();float Ϲ;int Ϡ=0;string[]ύ={"-",
"\\","|","/"};float ϔ;float ϋ=400000;List<Screen>ϊ=new List<Screen>();List<IMyShipController>ω=new List<IMyShipController>()
;bool ψ;const float χ=512f;float φ=1f;double υ=50.0;float τ=0.05f;MyIni σ=new MyIni();bool ς;List<IMyBatteryBlock>ρ=new
List<IMyBatteryBlock>();List<IMyCargoContainer>π=new List<IMyCargoContainer>();int ο=-1;Dictionary<IMyMotorSuspension,float>
ξ=new Dictionary<IMyMotorSuspension,float>();Dictionary<IMyMotorSuspension,float>ν=new Dictionary<IMyMotorSuspension,
float>();void μ(){if(!σ.TryParse(Me.CustomData))σ.Clear();if(!σ.ContainsSection("Routes")){σ.Set("Routes","Default","None");
string λ=" Format: RouteName=Navigate:Dest1;Dest2:Setting=Value;Setting2=Value\n\n"+" [SETTINGS & OPTIONS]\n"+
" Navigation: Circle, OneWay, Patrol\n"+" Collision:  None, SensorDriven\n"+" Wait:       None, Timerblock, Recharge, Cargo, External\n"+
" Parking:    Street, Connector\n"+" Interrupt:  ReplaceAll, InsertLast, InsertFirst, InsertNext\n\n"+" [EXECUTION]\n"+
" Run PB with argument 'Route' for Default, or 'Route:RouteName'";σ.SetComment("Routes","Default",λ);Me.CustomData=σ.ToString();}ϊ.Clear();List<IMyTextSurfaceProvider>κ=new List<
IMyTextSurfaceProvider>();GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(κ);foreach(var ι in κ){IMyTerminalBlock Ǻ=ι as
IMyTerminalBlock;if(Ǻ!=null&&Ǻ.CustomName.Contains(SCREEN_TAG)&&Ǻ.CubeGrid==Me.CubeGrid){MyIni θ=new MyIni();if(!θ.TryParse(Ǻ.CustomData
))θ.Clear();bool ό=false;List<string>η=new List<string>();for(int V=0;V<ι.SurfaceCount;V++){string ϟ=
$"{ι.GetSurface(V).DisplayName}_{V}";η.Add(ϟ);string ʀ=θ.Get("Screens",ϟ).ToString("");if(!θ.ContainsKey("Screens",ϟ)){ʀ=(V==0)?"X":"";θ.Set("Screens",ϟ,ʀ);
ό=true;}IMyTextSurface ϝ=ι.GetSurface(V);if(ʀ.Trim().Equals("X",StringComparison.OrdinalIgnoreCase)){ϝ.ContentType=
ContentType.SCRIPT;ϝ.Script="";ϝ.ScriptBackgroundColor=Color.Black;ϊ.Add(new Screen(ϝ));}else if(ϝ.ContentType==ContentType.SCRIPT)
{ϝ.ContentType=ContentType.NONE;ϝ.BackgroundColor=Color.Cyan;ϝ.WriteText("");}}List<MyIniKey>Ϝ=new List<MyIniKey>();θ.
GetKeys("Screens",Ϝ);foreach(var Ĥ in Ϝ){if(!η.Contains(Ĥ.Name)){θ.Delete(Ĥ.Section,Ĥ.Name);ό=true;}}if(ό){Ǻ.CustomData=θ.
ToString();}}}if(ϊ.Count<1){Echo("Add screen");return;}Υ=ң<IMySensorBlock>(blockTag);if(Υ==null){Echo("Add sensor");ĕ(à,ʻ.Center
,1.2f,$"Add sensor with tag {blockTag}",Color.White);ɷ(à,"Setup");ɽ(à,new Vector2(ʻ.Width/2,ʻ.Height*0.7f),new Vector2(ʻ.
Width*0.5f,ʻ.Height*0.05f),Color.Green,Color.White,5,1);à.Dispose();return;}Υ.BackExtend=50;Υ.FrontExtend=50;Υ.LeftExtend=50;
Υ.RightExtend=50;Υ.TopExtend=50;Υ.BottomExtend=50;Υ.DetectAsteroids=false;Υ.DetectFloatingObjects=false;Υ.
DetectSmallShips=true;Υ.DetectPlayers=true;Υ.DetectStations=false;Υ.DetectSubgrids=false;ϰ=ң<IMyCameraBlock>(blockTag);if(ϰ!=null){ϰ.
EnableRaycast=true;}else{Echo("Add Front facing camera");ĕ(à,ʻ.Center,1.2f,$"Add camera with tag {blockTag}",Color.White);ɷ(à,"Setup"
);ɽ(à,new Vector2(ʻ.Width/2,ʻ.Height*0.7f),new Vector2(ʻ.Width*0.5f,ʻ.Height*0.05f),Color.Green,Color.White,5,2);à.
Dispose();return;}Φ=ң<IMyShipController>(blockTag);if(Φ==null){Echo("Add controller block");ĕ(à,ʻ.Center,1.2f,
$"Add controller block with tag {blockTag}",Color.White);ɷ(à,"Setup");ɽ(à,new Vector2(ʻ.Width/2,ʻ.Height*0.7f),new Vector2(ʻ.Width*0.5f,ʻ.Height*0.05f),Color.Green
,Color.White,5,3);à.Dispose();return;}ˈ=ң<IMyRadioAntenna>(null);if(ˈ==null){Echo("Add antenna");ĕ(à,ʻ.Center,1.2f,
"Add antenna",Color.White);ɷ(à,"Setup");ɽ(à,new Vector2(ʻ.Width/2,ʻ.Height*0.7f),new Vector2(ʻ.Width*0.5f,ʻ.Height*0.05f),Color.Green
,Color.White,5,4);à.Dispose();return;}ˈ.Radius=5000;ζ.Add(IGC.RegisterBroadcastListener(IGC_TAG));Æ("v",MessageType.
DestinationRequest,IGC_TAG,TransmissionDistance.AntennaRelay);ˌ=DateTime.UtcNow;ϧ=ң<IMyShipConnector>(blockTag);if(ϧ?.CubeGrid!=Me.
CubeGrid)ϧ=null;List<IMyTimerBlock>ϛ=new List<IMyTimerBlock>();GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(ϛ);foreach(var
Ϛ in ϛ){if(Ϛ.CustomName.Contains(blockTag)){if(Ϛ.CustomData.Contains("Arrival")){Ϧ.Add(new Timer(Ϛ,Timer.TimerType.
ArrivalTimer));}if(Ϛ.CustomData.Contains("Departure")){Ϧ.Add(new Timer(Ϛ,Timer.TimerType.DepartureTimer));}}}var ϙ=
GridTerminalSystem.GetBlockGroupWithName(wheelTag);if(ϙ!=null){ϙ.GetBlocksOfType<IMyMotorSuspension>(Ρ);if(Ρ.Count>0){if(ξ==null)ξ=new
Dictionary<IMyMotorSuspension,float>();if(ν==null)ν=new Dictionary<IMyMotorSuspension,float>();ν.Clear();ξ.Clear();Vector3D Ϙ=Φ.
WorldMatrix.Forward;Vector3D ϗ=Φ.WorldMatrix.Right;Vector3D ϖ=Φ.CenterOfMass;foreach(var Ϟ in Ρ){Vector3D ϕ=Ϟ.GetPosition()-ϖ;
double ϓ=Vector3D.Dot(ϕ,ϗ);float ϒ=ϓ>0.0f?-1.0f:1.0f;ξ.Add(Ϟ,ϒ);double ϑ=Vector3D.Dot(ϕ,Ϙ);float ϐ=ϑ<0.0f?-1.0f:1.0f;ν.Add(Ϟ,ϐ
);}var Ϗ=Ρ.Find(K=>K.Steering);var ώ=Ρ.Find(K=>!K.Steering);if(Ϗ!=null&&ώ!=null){ʲ=(float)Vector3D.Distance(Ϗ.GetPosition
(),ώ.GetPosition());}else{Echo("Warning: Could not determine wheelbase (missing steering or non-steering wheel).");ʲ=0f;}
Ρ.ForEach(K=>K.MaxSteerAngle=(float)(Math.PI/180*STEERING_ANGLE));Ρ.ForEach(K=>K.PropulsionOverride=0);Ρ.ForEach(K=>K.
SteeringOverride=0);Echo($"Found {Ρ.Count} wheels");}else{Echo("No wheels found in wheelgroup");ĕ(à,ʻ.Center,1.2f,
$"Add wheels to wheelsgroup",Color.White);ɷ(à,"Setup");ɽ(à,new Vector2(ʻ.Width/2,ʻ.Height*0.7f),new Vector2(ʻ.Width*0.5f,ʻ.Height*0.05f),Color.Green
,Color.White,4,3.5f);à.Dispose();return;}}else{Echo("No wheel group found");ĕ(à,ʻ.Center,1.2f,
$"Add wheelgroup called {wheelTag}",Color.White);ɷ(à,"Setup");ɽ(à,new Vector2(ʻ.Width/2,ʻ.Height*0.7f),new Vector2(ʻ.Width*0.5f,ʻ.Height*0.05f),Color.Green
,Color.White,5,4.5f);à.Dispose();return;}Ρ.RemoveAll(K=>K.CubeGrid!=Me.CubeGrid);var Ҏ=GridTerminalSystem.
GetBlockGroupWithName(stopLightTag);if(Ҏ!=null){Ҏ.GetBlocksOfType<IMyLightingBlock>(Π);Π.ForEach(K=>{K.Enabled=false;K.Color=Color.Red;});}Π.
RemoveAll(K=>K.CubeGrid!=Me.CubeGrid);var ҧ=GridTerminalSystem.GetBlockGroupWithName(turnLightTag);if(ҧ!=null){List<
IMyLightingBlock>Ҧ=new List<IMyLightingBlock>();ҧ.GetBlocksOfType<IMyLightingBlock>(Ҧ);Ҧ.ForEach(K=>{K.Enabled=false;K.
BlinkIntervalSeconds=0.9f;K.BlinkLength=50;});foreach(var ҥ in Ҧ){Vector3D Ҥ=ҥ.GetPosition()-Φ.CenterOfMass;double ϓ=Vector3D.Dot(Ҥ,Φ.
WorldMatrix.Right);if(ϓ>=0)Ј.Add(ҥ);else Љ.Add(ҥ);}}Ј.RemoveAll(K=>K.CubeGrid!=Me.CubeGrid);Љ.RemoveAll(K=>K.CubeGrid!=Me.CubeGrid)
;ǐ();Ͼ=new Vector2(0,ʻ.Height*1.25f);Ͻ=new Vector2(0,ʻ.Height*1.25f);Ϲ=ʻ.Height*0.2f;GridTerminalSystem.GetBlocksOfType<
IMyShipController>(ω);GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(ρ);GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(π);Ω=
true;ά=DateTime.UtcNow;}Ȣ ң<Ȣ>(string À)where Ȣ:class,IMyTerminalBlock{var ȕ=new List<Ȣ>();GridTerminalSystem.
GetBlocksOfType<Ȣ>(ȕ);if(ȕ.Count==0)return null;if(string.IsNullOrEmpty(À))return ȕ[0];for(int V=0;V<ȕ.Count;V++){var Ț=ȕ[V];if(Ț.
CustomName.Contains(À))return Ț;}return null;}Program(){try{Runtime.UpdateFrequency=UpdateFrequency.Update10;Χ=new DriveSettings()
;if(Storage.Length>0){Echo("Storage: "+Storage.Length);ϔ=Storage.Length;string[]Ң=Storage.Split('%');ˑ=Ң[0].Split(';');if
(Ң[1].Length>0){string[]ҡ=Ң[1].Split('/');foreach(var ҍ in ҡ){Echo(ҍ);string[]Ê=ҍ.Split('*');Vector3D È=new Vector3D();
Vector3D.TryParse(Ê[1],out È);Destination Q=new Destination(Ê[0],È,long.Parse(Ê[2]),DateTime.UtcNow);Q.isChoosen=true;ˊ.Add(Q);ˋ
.Add(Q);}ˉ=int.Parse(Ң[2]);if(ˊ.Count>0&&ˉ<ˊ.Count){ˊ=ˊ.Skip(ˉ).Concat(ˊ.Take(ˉ)).ToList();ˉ=0;}Enum.TryParse<
DriveSettings.NavigationType>(Ң[3],out Χ.navigationType);Enum.TryParse<DriveSettings.CollisionType>(Ң[4],out Χ.collisionType);Enum.
TryParse<DriveSettings.ParkingType>(Ң[5],out Χ.parkingType);Enum.TryParse<DriveSettings.WaitType>(Ң[6],out Χ.waitType);bool.
TryParse(Ң[7],out ʺ);}ͷ=SaveStatus.UnloadingNodes;}}catch(Exception){}}MySpriteDrawFrame à;void Main(string Ҋ,UpdateType Ҳ){if(!
Ω){μ();return;}if(ͷ==SaveStatus.UnloadingNodes){if(Runtime.CurrentInstructionCount<Runtime.MaxInstructionCount*0.75f){ª(ˑ
);if(ʿ>=ˑ.Length){ͷ=SaveStatus.PopulatingConnectedNodes;}Echo($"Unloading nodes {ʿ}/{ˑ.Length}\nInstruction: {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount}\nRuntime: {Runtime.LastRunTimeMs}"
);foreach(var ұ in ϊ){à=ұ.textSurface.DrawFrame();ʻ=ұ.viewport;φ=Math.Min(ʻ.Width,ʻ.Height)/χ;ĕ(à,ʻ.Center,1,
$"Unloading map {ʿ}/{ˑ.Length}",Color.Yellow);ɽ(à,new Vector2(ʻ.Center.X,ʻ.Center.Y+40),new Vector2(ʻ.Width*0.8f,10),Color.Green,Color.White,ˑ.Length,ʿ
,0);à.Dispose();}}return;}else if(ͷ==SaveStatus.PopulatingConnectedNodes){if(Runtime.CurrentInstructionCount<Runtime.
MaxInstructionCount*0.75f){s(ˑ);Echo($"Unloading subnodes {ʾ}/{ć.nodes.Count}\nInstruction: {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount}\nRuntime: {Runtime.LastRunTimeMs}"
);if(ʾ>=ć.nodes.Count){ͷ=SaveStatus.Unloaded;Echo("Map loaded");}foreach(var ұ in ϊ){à=ұ.textSurface.DrawFrame();ʻ=ұ.
viewport;φ=Math.Min(ʻ.Width,ʻ.Height)/χ;ĕ(à,ʻ.Center,1,$"Unloading subnodes {ʾ}/{ć.nodes.Count}",Color.Yellow);ɽ(à,new Vector2(ʻ
.Center.X,ʻ.Center.Y+40),new Vector2(ʻ.Width*0.8f,10),Color.Green,Color.White,ć.nodes.Count,ʾ,0);à.Dispose();}ˆ=false;}
return;}if((ʵ==AutoDriveState.Idle||ʵ==AutoDriveState.Parking_Waiting)&&ˆ){ć=new Map();Storage="";ͷ=SaveStatus.UnloadingNodes;
}if(Ҋ.Equals("setup",StringComparison.OrdinalIgnoreCase)){μ();return;}Ó();Vector3D Ұ=new Vector3D();var ү=Ρ.FindAll(K=>K.
Steering);foreach(var Ϟ in ү){Ұ+=Ϟ.GetPosition();}å=Ұ/ү.Count;if(ʺ&&ˊ.Count>0&&ˋ.Count>0){ʺ=false;ˏ.Clear();ʹ=0;double Я=double.
MaxValue;double Ю=double.MaxValue;if(ϧ!=null)ϧ.Disconnect();Φ.HandBrake=false;foreach(var Z in ć.nodes){double Э=Vector3D.
Distance(å,Z.position);double Ь=Vector3D.Distance(ˊ[0].position,Z.position);if(Э<Я){Я=Э;Ƥ=Z;}if(Ь<Ю){Ю=Ь;ƣ=Z;}}if(Ƥ!=null&&ƣ!=
null){Echo($"Calculating route to {ˋ[ˡ].name}");ˍ.Ǿ(Ƥ,ƣ,Φ.WorldMatrix.Forward,ȏ,TURNAROUND_PATHFINDING_PENALTY);}ʵ=
AutoDriveState.Driving;ʴ=!ʴ;}if(Ν==EditStatus.Idle){switch(Ҋ){case"4":if((DateTime.UtcNow-Ѐ).TotalSeconds>Ё){Ђ[3]=true;Ѐ=DateTime.
UtcNow;}else{return;}if(Ο==DisplayState.Editing||Ο==DisplayState.Destination){Ͽ=DisplayState.AreaMap;if(ʹ){Æ(Ĉ(ć),MessageType.
MapBroadcast,IGC_TAG,TransmissionDistance.TransmissionDistanceMax);Storage=Ĉ(ć);}ʹ=false;}else if(Ο==DisplayState.
DestinationSettings){if(ʵ==AutoDriveState.AwaitingStart&&!ˍ.ȅ){ʵ=AutoDriveState.Idle;ˏ.Clear();Ͽ=DisplayState.Destination;}}break;case"1":
if((DateTime.UtcNow-Ѐ).TotalSeconds>Ё){Ђ[0]=true;Ѐ=DateTime.UtcNow;}else{return;}if(Ο==DisplayState.Editing&&Ξ==EditState.
PlaceTool&&ϔ<ϋ){ӏ();Ӓ();ʹ=true;ϔ=Ĉ(ć).Length;}else if(Ο==DisplayState.Editing&&Ξ==EditState.SelectTool){Ʀ.isSelected=!Ʀ.
isSelected;ʹ=true;ϔ=Ĉ(ć).Length;}else if(Ο==DisplayState.Editing&&Ξ==EditState.DeleteTool){if(ć.nodes.Any(K=>K.isSelected)){Ψ=ć.
nodes.ToList();Μ=0;Ν=EditStatus.UnlinkingNodes;}}else if(Ο==DisplayState.AreaMap){if(ʵ==AutoDriveState.Driving||ˏ.Count>0){ʴ=
!ʴ;ʵ=AutoDriveState.Driving;}else{Ο=DisplayState.Destination;}}else if(Ο==DisplayState.Destination&&ˋ.Count>0){if(ˋ.Count
-1>=ˡ){ˋ[ˡ].isChoosen=!ˋ[ˡ].isChoosen;if(ˋ[ˡ].isChoosen&&!ˊ.Any(K=>K.address==ˋ[ˡ].address)){ˊ.Add(ˋ[ˡ]);}else if(!ˋ[ˡ].
isChoosen&&ˊ.Any(K=>K.address==ˋ[ˡ].address)){ˊ.Remove(ˊ.FindLast(K=>K.address==ˋ[ˡ].address));ˋ[ˡ].isChoosen=false;}if(ˊ.Count==
0){ˏ.Clear();ʴ=false;ʵ=AutoDriveState.Idle;}if(ˊ.Count<=1){Χ.navigationType=DriveSettings.NavigationType.OneWay;Χ.
waitType=DriveSettings.WaitType.None;}}}else if(Ο==DisplayState.DestinationSettings){if(Χ.cursorIndex==0&&ˊ.Count>1){if(((int)Χ.
navigationType)<Enum.GetValues(typeof(DriveSettings.NavigationType)).Length-1){Χ.navigationType++;}else{Χ.navigationType=0;}}else if(Χ
.cursorIndex==1&&ϰ!=null&&Υ!=null){if(((int)Χ.collisionType)<Enum.GetValues(typeof(DriveSettings.CollisionType)).Length-1
){Χ.collisionType++;}else{Χ.collisionType=0;}}else if(Χ.cursorIndex==2&&ϧ!=null){if(((int)Χ.parkingType)<Enum.GetValues(
typeof(DriveSettings.ParkingType)).Length-1){Χ.parkingType++;}else{Χ.parkingType=0;}if(Χ.parkingType==DriveSettings.
ParkingType.Street){if(Χ.waitType==DriveSettings.WaitType.Recharge||Χ.waitType==DriveSettings.WaitType.Cargo){Χ.waitType=
DriveSettings.WaitType.None;}}}else if(Χ.cursorIndex==3){if(ˊ.Count<=1){Χ.waitType=DriveSettings.WaitType.None;return;}int Ү=Enum.
GetValues(typeof(DriveSettings.WaitType)).Length-1;bool ҭ=Ϧ.Exists(K=>K.timerType==Timer.TimerType.DepartureTimer);bool Ҭ=(Χ.
parkingType==DriveSettings.ParkingType.Connector);int ҫ=0;do{int Ҫ=(int)Χ.waitType+1;if(Ҫ>Ү)Ҫ=0;Χ.waitType=(DriveSettings.WaitType)
Ҫ;bool ҩ=(!ҭ&&Χ.waitType==DriveSettings.WaitType.Timerblock);bool Ҩ=(!Ҭ&&(Χ.waitType==DriveSettings.WaitType.Recharge||Χ.
waitType==DriveSettings.WaitType.Cargo));if(!ҩ&&!Ҩ){break;}ҫ++;}while(ҫ<=Ү);}}break;case"2":if((DateTime.UtcNow-Ѐ).TotalSeconds>
Ё){Ђ[1]=true;Ѐ=DateTime.UtcNow;}else{return;}if(Ο==DisplayState.Editing){if(Ξ!=0){Ξ--;}}else if(Ο==DisplayState.
Destination){if(ˡ!=0){ˡ--;}}else if(Ο==DisplayState.DestinationSettings){if(Χ.cursorIndex!=0){Χ.cursorIndex--;}}else if(Ο==
DisplayState.AreaMap&&ʴ){ϼ=!ϼ;}break;case"3":if((DateTime.UtcNow-Ѐ).TotalSeconds>Ё){Ђ[2]=true;Ѐ=DateTime.UtcNow;}else{return;}if(Ο==
DisplayState.AreaMap){if(ʵ==AutoDriveState.Driving||ˏ.Count>0){ʵ=AutoDriveState.Stopping;ʴ=false;ˏ.Clear();ˊ.Clear();ˉ=0;ˋ.ForEach(K
=>K.isChoosen=false);}else Ο=DisplayState.Editing;}else if(Ο==DisplayState.Editing){if((int)Ξ<2){Ξ++;}}else if(Ο==
DisplayState.Destination){if(ˋ.Count()-1!=ˡ){ˡ++;}}else if(Ο==DisplayState.DestinationSettings){if(Χ.maxCursorIndex!=Χ.cursorIndex){
Χ.cursorIndex++;}}break;case"scaleUp":if(Ϸ!=ϴ){Ϸ+=0.25f;}break;case"scaleDown":if(Ϸ!=ϳ){Ϸ-=0.25f;}break;case"ClearMap":
break;case"5":if((DateTime.UtcNow-Ѐ).TotalSeconds>Ё){Ђ[4]=true;Ѐ=DateTime.UtcNow;}else{return;}if(Ο==DisplayState.Destination
&&ˊ.Count>0){ˏ.Clear();ʹ=0;double Я=double.MaxValue;double Ю=double.MaxValue;foreach(var Z in ć.nodes){double Э=Vector3D.
Distance(å,Z.position);double Ь=Vector3D.Distance(ˊ[0].position,Z.position);if(Э<Я){Я=Э;Ƥ=Z;}if(Ь<Ю){Ю=Ь;ƣ=Z;}}if(Ƥ!=null&&ƣ!=
null){Echo($"Calculating route to {ˋ[ˡ].name}");ˍ.Ǿ(Ƥ,ƣ,Φ.WorldMatrix.Forward,ȏ,TURNAROUND_PATHFINDING_PENALTY);Χ.
navigationType=DriveSettings.NavigationType.OneWay;Ͽ=DisplayState.DestinationSettings;ʵ=AutoDriveState.AwaitingStart;}else{č(
"Navigation","Unable to calculate path",Color.Red);}ϼ=false;}else if(Ο==DisplayState.Destination&&ˊ.Count==0){Ͽ=DisplayState.
Destination;}else if(Ο==DisplayState.DestinationSettings){Φ.HandBrake=false;if(ϧ!=null)ϧ.Disconnect();if(Ϫ)Ã(Ϭ.ToString(),
MessageType.DestinationConnectorDisconnect,IGC_TAG,Ϥ);Ϫ=false;Ͽ=DisplayState.AreaMap;ʵ=AutoDriveState.Driving;ʴ=true;Ϸ=ϴ;}break;
default:break;}ҋ(Ҋ);}else{g();}for(int V=0;V<Ђ.Length;V++){if((DateTime.UtcNow-Ѐ).TotalSeconds>Ё){if(V==3&&Ђ[3]&&!ˍ.ȅ){if(Ο==
DisplayState.DestinationSettings)Ϸ=ϴ;Ο=Ͽ;}else if(V==4&&Ђ[4]){Ο=Ͽ;}Ђ[V]=false;}}ˍ.Ȏ();ʰ.Clear();Υ.DetectedEntities(ʰ);List<
MyDetectedEntityInfo>Ҍ=new List<MyDetectedEntityInfo>();Ҍ=ʰ;ӊ();if(ʰ.Exists(K=>K.Type==MyDetectedEntityType.CharacterHuman)||ω.Exists(K=>K.
IsUnderControl)){ψ=false;Ӝ();}else if(!ψ){ĕ(à,ʻ.Center,1,"Screen in Sleep Mode",Color.Yellow);à.Dispose();ψ=true;}}void Save(){String
ˋ="";if(ˊ.Count>0){ˋ=ˊ[0].ToString(ˊ);}if(ć.nodes.Count>0){Storage=
$"{Ĉ(ć)}%{ˋ}%{ˉ}%{Χ.navigationType}%{Χ.collisionType}%{Χ.parkingType}%{Χ.waitType}%{ʴ}";}}void ҋ(string Ҋ,long Ï=0){if(string.IsNullOrWhiteSpace(Ҋ))return;string Ѳ=Ҋ.Trim();string[]É=Ѳ.Split(':');string ҁ=É[
0].ToLower();if(ҁ.Equals("route")){MyIni Ҁ=new MyIni();if(Ҁ.TryParse(Me.CustomData)){string ѿ=(É.Length>1)?É[1]:"Default"
;string Ѿ=Ҁ.Get("Routes",ѿ).ToString("");if(!string.IsNullOrWhiteSpace(Ѿ)&&!Ѿ.Equals("None",StringComparison.
OrdinalIgnoreCase)){č("AutoDrive",$"Loading Route: {ѿ}",Color.Cyan);ҋ(Ѿ,Ï);return;}else{č("Config Error",$"Route '{ѿ}' not found",Color.
Red);}}return;}if(ҁ.StartsWith("navigate")){List<string>ѽ=new List<string>();if(ҁ.Contains(".")){ѽ.Add(ҁ.Split('.')[1]);}
else if(É.Length>1){ѽ.AddRange(É[1].Split(';'));}List<Destination>Ѽ=new List<Destination>();foreach(var ѻ in ѽ){var ҍ=ˋ.Find
(K=>K.name.Equals(ѻ.Trim(),StringComparison.OrdinalIgnoreCase));if(ҍ!=null)Ѽ.Add(ҍ);}if(Ѽ.Count>0){if(É.Length>2){ҙ(É[2])
;}Ҟ(Ѽ,Ï);}else{č("Navigation","No valid destinations found.",Color.Red);}}else if(ҁ.Contains("exportmap")){if(!σ.TryParse
(Me.CustomData))σ.Clear();σ.Set("Map","Data",Ĉ(ć));Me.CustomData=σ.ToString();č("Map Update","Exported to [Map] section",
Color.Green);}else if(ҁ.Contains("importmap")){MyIni Ҁ=new MyIni();if(Ҁ.TryParse(Me.CustomData)&&Ҁ.ContainsKey("Map","Data"))
{string Ҡ=Ҁ.Get("Map","Data").ToString();ˑ=Ҡ.Split(';');ʿ=0;ʾ=0;ʽ=0;ˆ=true;č("Map Update","Importing map...",Color.Yellow
);}else{č("Map Error","No [Map] Data found!",Color.Red);}}else if(ҁ.Contains("clearmap")){ć=new Map();Storage="";ҏ();}
else if(ҁ.Contains("cancel")){ʵ=AutoDriveState.Stopping;ʴ=false;ҏ();if(Ï!=0)Ã("Y",MessageType.CommandConfirmed,IGC_TAG,Ï);}
else if(ҁ.Contains("resume")){if(ʵ==AutoDriveState.Idle&&ˏ.Count>0){Φ.HandBrake=false;ʵ=AutoDriveState.Driving;ʴ=true;if(Ï!=
0)Ã("Y",MessageType.CommandConfirmed,IGC_TAG,Ï);}else if(ʵ==AutoDriveState.ExternalWait){ς=true;if(Ï!=0)Ã("Y",MessageType
.CommandConfirmed,IGC_TAG,Ï);}else{if(Ï!=0)Ã("N",MessageType.CommandConfirmed,IGC_TAG,Ï);}}else if(ҁ.Contains("stop")){if
(ʵ==AutoDriveState.Driving&&ˏ.Count>0){ʵ=AutoDriveState.Stopping;ʴ=false;if(Ï!=0)Ã("Y",MessageType.CommandConfirmed,
IGC_TAG,Ï);}else{if(Ï!=0)Ã("N",MessageType.CommandConfirmed,IGC_TAG,Ï);}}}void Ҟ(List<Destination>ҝ,long Ï=0){bool Ҝ=false;
switch(Χ.interruptType){case DriveSettings.InterruptType.ReplaceAll:ˊ.Clear();ˊ.AddRange(ҝ);ˉ=0;Ҝ=true;break;case
DriveSettings.InterruptType.InsertLast:ˊ.AddRange(ҝ);if(ʵ!=AutoDriveState.Driving)Ҝ=true;break;case DriveSettings.InterruptType.
InsertFirst:ˊ.InsertRange(ˉ,ҝ);Ҝ=true;break;case DriveSettings.InterruptType.InsertNext:int қ=ˉ+1;if(қ>ˊ.Count)қ=ˊ.Count;ˊ.
InsertRange(қ,ҝ);if(ʵ==AutoDriveState.Idle)Ҝ=true;break;}ҳ();ˋ.ForEach(K=>K.isChoosen=false);foreach(var Қ in ˊ)Қ.isChoosen=true;if
(Ҝ){ˏ.Clear();Φ.HandBrake=false;if(ϧ!=null)ϧ.Disconnect();if(Ϫ){Ã(Ϭ.ToString(),MessageType.DestinationConnectorDisconnect
,IGC_TAG,Ϥ);}Ϭ=new Vector3D();Ϫ=false;Node Ƥ=new Node(Vector3D.Zero);Node ƣ=new Node(Vector3D.Zero);Ж(ref Ƥ,ref ƣ);if(Ƥ!=
null&&ƣ!=null){ˍ=new ȋ();ˍ.Ǿ(Ƥ,ƣ,Φ.WorldMatrix.Forward,ȏ,TURNAROUND_PATHFINDING_PENALTY);ʴ=true;ʵ=AutoDriveState.Driving;}č(
"Navigation",$"Route Updated. Total Stops: {ˊ.Count}",Color.Green);}else{č("Navigation",$"Queue Updated (+{ҝ.Count} stops)",Color.
Green);}if(ˊ.Count==1){Χ.navigationType=DriveSettings.NavigationType.OneWay;}if(Ï!=0)Ã("Y",MessageType.CommandConfirmed,
IGC_TAG,Ï);}void ҙ(string ҟ){string[]Ҙ=ҟ.Split(';');foreach(var җ in Ҙ){string[]Җ=җ.Split('=');if(Җ.Length!=2)continue;string Ĥ
=Җ[0].Trim().ToLower();string ҕ=Җ[1].Trim();try{switch(Ĥ){case"collision":DriveSettings.CollisionType Ҕ;if(Enum.TryParse(
ҕ,true,out Ҕ))Χ.collisionType=Ҕ;break;case"parking":if(ϧ!=null){DriveSettings.ParkingType ғ;if(Enum.TryParse(ҕ,true,out ғ
))Χ.parkingType=ғ;}break;case"wait":DriveSettings.WaitType Ғ;if(Enum.TryParse(ҕ,true,out Ғ))Χ.waitType=Ғ;if(Χ.waitType==
DriveSettings.WaitType.Timerblock&&Ϧ.Count==0)Χ.waitType=DriveSettings.WaitType.None;break;case"navigation":DriveSettings.
NavigationType ґ;if(Enum.TryParse(ҕ,true,out ґ))Χ.navigationType=ґ;break;case"interrupt":DriveSettings.InterruptType Ґ;if(Enum.
TryParse(ҕ,true,out Ґ))Χ.interruptType=Ґ;break;}}catch{č("Settings",$"Failed to parse setting: {Ĥ}",Color.Red);}}}void ҏ(){ˊ.
Clear();ˉ=0;ˋ.ForEach(K=>K.isChoosen=false);ˏ.Clear();}void ҳ(){if(ˊ.Count<2)return;for(int V=ˊ.Count-1;V>0;V--){if(ˊ[V].name
==ˊ[V-1].name){ˊ.RemoveAt(V);}}}void Ӝ(){foreach(var ұ in ϊ){ʻ=ұ.viewport;var à=ұ.textSurface.DrawFrame();φ=Math.Min(ʻ.
Width,ʻ.Height)/χ;switch(Ο){case DisplayState.AreaMap:ӛ(à);break;case DisplayState.Editing:Ӗ(à);break;case DisplayState.
Destination:Ӧ(à);break;case DisplayState.DestinationSettings:ӟ(à);break;default:break;}Ô(à,ұ);à.Dispose();}if(ϼ){ϻ=Ȥ(ϻ,1f,Ι);if(ˏ.
Count>0){ʶ=ћ(ˏ.GetRange(0,ˏ.Count-1));}}else{ϻ=Ȥ(ϻ,0f,Ι);}}void ӛ(MySpriteDrawFrame à){Color Ӛ=Color.DeepSkyBlue;Color ә=
Color.Orange;if(ć.nodes.Count>0){ʖ(à,ć,new Vector2(ʻ.Width/2,ʻ.Height/2));ϲ=Ȥ(ϲ,75,Ι);}if(ʴ){Ӛ=Color.DeepSkyBlue;ә=Color.
CadetBlue;}else{Ӛ=Color.Green;}if(ʵ!=AutoDriveState.Idle){Ɉ(à,new Vector2(ʻ.Width*0.95f,ʻ.Height*0.60f),Ӛ);á(à,"Pause",$"Info",
"Cancel");}else{ʚ(à,new Vector2(ʻ.Width*0.95f,60),Color.Green);string Ә="Drive";if(ˏ.Count>0)á(à,"Resume","Info","Cancel");else
á(à,"Navigate","","Edit");}if(Ƥ!=null&&ƣ!=null){if(ʵ==AutoDriveState.Driving||ʵ==AutoDriveState.Reversing)foreach(var ӗ
in Ϯ){if(ӗ.type==MyDetectedEntityType.Planet||ӗ.type==MyDetectedEntityType.LargeGrid)ă(à,ƻ(ӗ.Position,å,ϲ),Color.Gray);
else ă(à,ƻ(ӗ.Position,å,ϲ),Color.Orange);}}foreach(var Q in ˋ){ɟ(à,ƻ(Q.position,å,ϲ),Q.name);}ĕ(à,new Vector2(ʻ.Width/2,ʻ.
Height*0.12f),0.8f,$"{(int)(с(Φ)*3.6f)} km/h",ә,TextAlignment.CENTER);é(à,ә);ɷ(à,"Map");}void Ӗ(MySpriteDrawFrame à){Ʀ=Ǹ(ć.
nodes,å);ϱ+=0.25f;if(ć.nodes.Count>0&&Ʀ!=null){if(Ξ==EditState.PlaceTool){float ǳ=Math.Max((float)Vector3D.Distance(å,Ʀ.
position),25);float ʫ=Math.Min(ʻ.Width/2,ʻ.Height);float ʤ=(float)(ʫ/ǳ);Ϸ=Ȥ((float)Ϸ,ʤ*0.5f,Ι);}else{Ϸ=Ȥ((float)Ϸ,(float)ϳ,Ι);}}
if(ć.nodes.Count==0){ĕ(à,new Vector2D(ʻ.Width/2,ʻ.Height/2+(25*φ)),1,"Place first node",Color.Yellow);ɧ(à,ʻ.Center);}else{
foreach(var Z in ć.nodes){Vector2D ӕ=ƻ(Z.position,å,ϲ);if(Z==Ʀ&&ć.nodes.Find(K=>K.isSelected==true)==null){ă(à,ӕ,Color.Red);}
else{ă(à,ӕ,Color.Yellow);}foreach(var Y in Z.connectedNodes){Vector2D Ӕ=ƻ(Y.position,å,ϲ);ĝ(à,ӕ,Ӕ,Color.White,5);}}switch(Ξ)
{case EditState.PlaceTool:if(ć.nodes.Find(K=>K.isSelected==true)==null){ĝ(à,ƻ(Ʀ.position,å,ϲ),new Vector2(ʻ.Width/2,ʻ.
Height/2),Color.Green,5);}else{List<Node>Ӎ=ć.nodes.FindAll(K=>K.isSelected);foreach(var ӌ in Ӎ){ĝ(à,ƻ(ӌ.position,å,ϲ),new
Vector2(ʻ.Width/2,ʻ.Height/2),Color.Green,5);}}ɧ(à,ʻ.Center);if(Ʀ.isSelected){ɢ(à,ƻ(Ʀ.position,å,ϲ),Color.YellowGreen,1,ϱ);}
break;case EditState.SelectTool:if(!Ʀ.isSelected){ɢ(à,ƻ(Ʀ.position,å,ϲ),Color.White);}else{ɢ(à,ƻ(Ʀ.position,å,ϲ),Color.Green,
1,ϱ);}é(à,Color.Orange);break;case EditState.DeleteTool:é(à,Color.Orange);break;default:break;}}if(ć.nodes.Find(K=>K.
isSelected==true)!=null){foreach(var Z in ć.nodes){if(Z.isSelected&&Ʀ!=Z&&Ξ!=EditState.DeleteTool){ɢ(à,ƻ(Z.position,å,ϲ),Color.
YellowGreen,1,ϱ);}else if(Ξ==EditState.DeleteTool&&Z.isSelected){ɢ(à,ƻ(Z.position,å,ϲ),Color.Red,1,ϱ);}}}string Ө=Ξ.ToString().
Substring(0,Ξ.ToString().IndexOf('T'));á(à,Ө,"Prev. Tool","Next Tool","Back");ɷ(à,"EditMap");ĕ(à,new Vector2D(ʻ.Width/2+(10*φ),65
*φ),0.8f,$"x: {(int)å.X} | y: {(int)å.Y} | z: {(int)å.Z}",Color.White);string[]ӧ=Ξ.ToString().Split('T');ĕ(à,new Vector2(
ʻ.Width/2,ʻ.Height*0.78f),0.85f,$"{ӧ[0]} Tool",Color.Yellow);if(Ν!=EditStatus.Idle){ˠ+=0.25f;Ɋ(à,new Vector2(ʻ.Width*
0.15f,16f*φ),"",ˠ,Color.Yellow);}ĕ(à,new Vector2(ʻ.Width*0.8f,ʻ.Height*0.75f),0.78f,$"{(int)(ϔ/1000)} k / 400 k",Color.White)
;ɽ(à,new Vector2(ʻ.Width*0.8f,ʻ.Height*0.82f),new Vector2(ʻ.Width*0.15f,ʻ.Height*0.02f),Color.Yellow,Color.White,ϋ,ϔ);ϲ=Ȥ
(ϲ,0,Ι);}void Ӧ(MySpriteDrawFrame à){Vector2 ӥ=new Vector2(ʻ.Width*0.1f,60f*φ);float Ӥ=40f*φ;int ӣ=8;int Ӣ=(ˋ.Count+ӣ-1)/
ӣ;int ӡ=ˢ*ӣ;int Ӡ=Math.Min(ӡ+ӣ,ˋ.Count);ˋ=ˋ.OrderBy(K=>K.name).ToList();for(int V=ӡ;V<Ӡ;V++){bool ɶ=(ˡ==V);ɱ(à,ӥ,ˋ[V].
name,ɶ,ˊ.Any(K=>K.name==ˋ[V].name));ӥ=ӥ+new Vector2(0,Ӥ);}ɷ(à,"Destinations");if(ˋ.Count>0){ĕ(à,new Vector2D(ʻ.Width/2,ʻ.
Height*0.78f),1,$"{ˢ+1}/{Ӣ}",Color.Yellow,TextAlignment.CENTER);}else{ĕ(à,new Vector2D(ʻ.Width/2,ʻ.Height*0.45f),1.25f,
$"No destinations",Color.Yellow,TextAlignment.CENTER);ĕ(à,new Vector2D(ʻ.Width/2,ʻ.Height*0.52f),1f,
$"Add the [DestinationScript] \nto your destinations.",Color.White,TextAlignment.CENTER);}if(ˡ>=Ӡ&&ˡ<ˋ.Count){ˢ++;}else if(ˡ<ӡ&&ˡ>=0){ˢ--;}ˢ=Math.Max(0,Math.Min(ˢ,Ӣ-1));á(à,
"Select","Up","Down","Back","Next");}void ӟ(MySpriteDrawFrame à){Color Ӛ=Color.DeepSkyBlue;Color ә=Color.Orange;if(ˍ.ȅ){ˠ+=0.25f
;float Ӟ=16f*φ;Ɋ(à,new Vector2(ʻ.Width*0.15f,Ӟ),"",ˠ,Color.Yellow);ĕ(à,new Vector2(ʻ.Width/2f,ʻ.Height/2f),1.5f,
"Calculating route...",Color.White);}else if(ˏ.Count>0){if(ć.nodes.Count>0){ϲ=Ȥ(ϲ,0,Ι);Vector2 Ƶ=new Vector2(ʻ.Width/4,ʻ.Height*0.75f);
Vector3D ǲ=ˊ[ˉ].position-å;ǲ.Normalize();Vector3D ƴ=Φ.WorldMatrix.Forward;Vector3D ȟ=Φ.WorldMatrix.Right;double ӝ=Vector3D.Dot(ǲ
,ƴ);double ӓ=Vector3D.Dot(ǲ,ȟ);double ӆ=Math.Atan2(ӓ,ӝ);double Ӌ=ӆ;double ӄ=Ӌ-ʷ;ӄ=Math.IEEERemainder(ӄ,Math.PI*2);ʷ=Ȥ(ʷ,(
float)(ʷ+ӄ),Ι);ʷ=(float)Math.IEEERemainder(ʷ,Math.PI*2);double ǳ=50;foreach(var Z in ˏ){double ī=Vector3D.Distance(å,Z.
position);if(ī>ǳ)ǳ=ī;}υ=Ȥ((float)υ,(float)ǳ,τ);float ʫ=Math.Min(ʻ.Width/2,ʻ.Height);Ϸ=(float)(ʫ/υ)*0.85f;ʖ(à,ć,Ƶ,false,υ*1.5,ʷ);
é(à,Color.Orange,true,Ƶ);Vector3D Ӄ=ȩ(ˊ[ˉ].position,å,ʷ);Vector2D ӂ=ƻ(Ӄ,å,(float)ϲ,true,Ƶ,ƹ,Ƹ,Ʒ,false);ɟ(à,ӂ,ˊ[ˉ].name);}
float Ӂ=150f*φ;float Ӏ=10f*φ;float ҿ=Ӂ+(60f*φ);float ʄ=350f*φ;float Ҿ=6f*φ;float ҽ=20f*φ;float Ҽ=ʻ.Width-ҽ-(ҿ/2f);float һ=
245f*φ;Ğ(à,new Vector2(Ҽ,һ),Color.White,new Vector2(ҿ,ʄ));Ğ(à,new Vector2(Ҽ,һ),Color.Black,new Vector2(ҿ-Ҿ,ʄ-Ҿ));float Һ=Ҽ-(
Ӂ/2f);float ҹ=150f*φ;float Ҹ=225f*φ;float ҷ=300f*φ;float Ҷ=375f*φ;List<int>ҵ=new List<int>();if(ˊ.Count<=1){ҵ.Add((int)
DriveSettings.NavigationType.Circle);ҵ.Add((int)DriveSettings.NavigationType.Patrol);}ʪ<DriveSettings.NavigationType>(à,new Vector2(Һ
,ҹ),Ӂ,Ӏ,(int)Χ.navigationType,ҵ);List<int>Ӆ=new List<int>();if(ϰ==null||Υ==null){Ӆ.Add((int)DriveSettings.CollisionType.
SensorDriven);}ʪ<DriveSettings.CollisionType>(à,new Vector2(Һ,Ҹ),Ӂ,Ӏ,(int)Χ.collisionType,Ӆ);List<int>Ҵ=new List<int>();if(ϧ==null){
Ҵ.Add((int)DriveSettings.ParkingType.Connector);}ʪ<DriveSettings.ParkingType>(à,new Vector2(Һ,ҷ),Ӂ,Ӏ,(int)Χ.parkingType,Ҵ
);List<int>Ӈ=new List<int>();if(Ϧ.Count==0||!Ϧ.Exists(K=>K.timerType==Timer.TimerType.DepartureTimer)){Ӈ.Add((int)
DriveSettings.WaitType.Timerblock);}if(ˊ.Count<=1||Χ.parkingType!=DriveSettings.ParkingType.Connector){if(!Ӈ.Contains((int)
DriveSettings.WaitType.Recharge))Ӈ.Add((int)DriveSettings.WaitType.Recharge);if(!Ӈ.Contains((int)DriveSettings.WaitType.Cargo))Ӈ.Add(
(int)DriveSettings.WaitType.Cargo);}if(ˊ.Count<=1){if(!Ӈ.Contains((int)DriveSettings.WaitType.Timerblock))Ӈ.Add((int)
DriveSettings.WaitType.Timerblock);if(!Ӈ.Contains((int)DriveSettings.WaitType.External))Ӈ.Add((int)DriveSettings.WaitType.External);}
ʪ<DriveSettings.WaitType>(à,new Vector2(Һ,Ҷ),Ӂ,Ӏ,(int)Χ.waitType,Ӈ);Vector2 ӑ=new Vector2(Һ,ҹ);if(Χ.cursorIndex==1)ӑ.Y=Ҹ;
else if(Χ.cursorIndex==2)ӑ.Y=ҷ;else if(Χ.cursorIndex==3)ӑ.Y=Ҷ;float Ӑ=25f*φ;à.Add(new MySprite(){Type=SpriteType.TEXTURE,
Alignment=TextAlignment.RIGHT,Data="AH_BoreSight",Position=new Vector2(ӑ.X,ӑ.Y)+ʻ.Position,Size=new Vector2(Ӑ,Ӑ),Color=new Color(
255,255,255,255),});ɷ(à,"Drive Settings");á(à,"Toggle","Up","Down","Back","Drive");}}void ӏ(){Node u=new Node(å);if(ć.nodes
.Count==0){ć.nodes.Add(u);}else{if(ć.nodes.Find(K=>K.isSelected==true)==null){Node ӎ=Ǹ(ć.nodes,å);ć.nodes.Find(K=>K.
position==ӎ.position).connectedNodes.Add(u);u.connectedNodes.Add(ӎ);}else{List<Node>Ӎ=ć.nodes.FindAll(K=>K.isSelected);foreach(
var ӌ in Ӎ){ӌ.connectedNodes.Add(u);u.connectedNodes.Add(ӌ);}}ć.nodes.Add(u);}}void Ӓ(){foreach(var Z in ć.nodes){Z.
isSelected=false;}}void ӊ(){if(ʴ&&ˍ.ȅ){Ц(0);}else if(ʴ&&!ˍ.ȅ){var Ӊ=Ϧ.Find(K=>K.timerType==Timer.TimerType.DepartureTimer);var ӈ=Ϧ
.Find(K=>K.timerType==Timer.TimerType.ArrivalTimer);if(ʵ==AutoDriveState.Driving){if(Њ.Count>1&&Ќ.Count>0){List<
Reservation>з=Њ.FindAll(K=>K.intersection==Ќ[0].position).OrderBy(K=>K.sendTime).ThenBy(K=>K.address).ToList();if(Vector3D.Distance
(å,Ќ[0].position)<ΰ&&з.IndexOf(Ћ)>0){Ц(0);Ї=true;}else{ж();Ї=false;}}else{ж();}if(Ѷ()){ʵ=AutoDriveState.StuckStop;ˇ=
DateTime.UtcNow;}Ѹ();}else if(ʵ==AutoDriveState.Parking_Stopping){Ц(0);if(с(Φ)<1){Node Ʀ=Ǹ(ć.nodes,Ϭ);if(Math.Abs(є(Ů(Ʀ,å,0)))>
100){ʵ=AutoDriveState.Parking_Creep2;}else{ʵ=AutoDriveState.Parking_Creep1;}č("Parking","Starting parking manoeuvre",Color.
Green);}}else if(ʵ==AutoDriveState.Parking_Creep1){var ɏ=Ō(Vector3D.Zero);if(ʰ.Exists(K=>K.Velocity.LengthSquared()>2&&K.Type
==MyDetectedEntityType.SmallGrid&&Vector3D.Distance(K.Position,å)<20)&&Ϯ.Exists(K=>Vector3D.Distance(å,K.Position)<10)){Ц(
0);}else{Node Ʀ=Ǹ(ć.nodes,Ϭ);ϭ=ǹ(å,Ů(Ʀ,Ϭ,0),ѵ.Ѵ(с(Φ),5,15));Ы(Ů(Ʀ,Ϭ,0));Ц(MIN_SPEED);if(Vector3D.Distance(Ů(Ʀ,Ϭ,0),å)<5){
ϥ=new Vector3D(å);ʵ=AutoDriveState.Parking_Creep2;}}}else if(ʵ==AutoDriveState.Parking_Creep2){var ɏ=Ō(Vector3D.Zero);if(
ʰ.Exists(K=>K.Velocity.LengthSquared()>2&&K.Type==MyDetectedEntityType.SmallGrid&&Vector3D.Distance(K.Position,å)<20)&&Ϯ.
Exists(K=>Vector3D.Distance(å,K.Position)<10)){Ц(0);}else{Node Ʀ=Ǹ(ć.nodes,Ϭ);ϭ=ǹ(å,Ů(Ʀ,Ϭ,0),ѵ.Ѵ(с(Φ),5,15));Ы(Ů(Ʀ,å,0));Ц(
MIN_SPEED);if(Math.Abs(є(ˬ,false,ϧ.GetPosition()))-β<25&&Vector3D.Distance(ϥ,ϧ.GetPosition())>CREEP_DISTANCE){ʳ=true;ʵ=
AutoDriveState.Parking_Reverse1;}Echo($"ConnectorAngle: {Math.Abs(є(ˬ,false,ϧ.GetPosition()))}");}}else if(ʵ==AutoDriveState.
Parking_Reverse1){var ɏ=Ō(Vector3D.Zero);if(ʰ.Exists(K=>K.Velocity.LengthSquared()>2&&K.Type==MyDetectedEntityType.SmallGrid&&Vector3D.
Distance(K.Position,å)<20)&&Ϯ.Exists(K=>Vector3D.Distance(å,K.Position)<10)){Ц(0);}else{Vector3D Й=Ż(Ϭ,Ϭ+ϩ*20,ϧ.GetPosition(),0)
+-ϩ*2.5f;Ц(MIN_SPEED);Ы(Й,true,ϧ.GetPosition());ϭ=Й;if(Vector3D.Distance(ϧ.GetPosition(),Ϭ)<α){ʵ=AutoDriveState.
Parking_Correction;ʳ=false;}}}else if(ʵ==AutoDriveState.Parking_Correction){var ɏ=Ō(Vector3D.Zero);if(ʰ.Exists(K=>K.Velocity.LengthSquared
()>2&&K.Type==MyDetectedEntityType.SmallGrid&&Vector3D.Distance(K.Position,å)<20)&&Ϯ.Exists(K=>Vector3D.Distance(å,K.
Position)<10)){Ц(0);}else{Ц(MIN_SPEED);Vector3D Й=Ϭ+ϩ*20;ϭ=Й;Ы(Й);if(Vector3D.Distance(å,Й)<4){ʳ=true;ʵ=AutoDriveState.
Parking_Reverse2;}}}else if(ʵ==AutoDriveState.Parking_Reverse2){var ɏ=Ō(Vector3D.Zero);if(ʰ.Exists(K=>K.Velocity.LengthSquared()>2&&K.
Type==MyDetectedEntityType.SmallGrid&&Vector3D.Distance(K.Position,å)<20)&&Ϯ.Exists(K=>Vector3D.Distance(å,K.Position)<10)){
Ц(0);}else{Vector3D Й=Ż(Ϭ,Ϭ+ϩ*20,ϧ.GetPosition(),0)+-ϩ*2.5f;Ц(MIN_SPEED);Ы(Й,true,ϧ.GetPosition());ϭ=Й;if(Vector3D.
Distance(ϧ.GetPosition(),Ϭ)>1){ϧ.Connect();if(ϧ.IsConnected){Π.ForEach(K=>{K.Enabled=false;K.Color=Color.Red;});if(Χ.waitType==
DriveSettings.WaitType.Timerblock){ʵ=AutoDriveState.Parking_Waiting;if(Ӊ!=null){Ӊ.myTimer.StartCountdown();}if(ӈ!=null){ӈ.myTimer.
Trigger();}ϡ=DateTime.UtcNow;}else if(Χ.waitType==DriveSettings.WaitType.Recharge){ʵ=AutoDriveState.RechargeWait;}else if(Χ.
waitType==DriveSettings.WaitType.Cargo){ϸ=-1f;ʵ=AutoDriveState.CargoWait;}else if(Χ.waitType==DriveSettings.WaitType.External){ʵ
=AutoDriveState.ExternalWait;}else{ʵ=AutoDriveState.Driving;ƥ();}}}else{ʵ=AutoDriveState.Parking_Correction;ʳ=false;}}}
else if(ʵ==AutoDriveState.Parking_Waiting){Ц(0);Φ.HandBrake=true;if(!Ӊ.myTimer.IsCountingDown){Φ.HandBrake=false;ʵ=
AutoDriveState.Driving;ƥ();}}else if(ʵ==AutoDriveState.Stopping){Ц(0);if(с(Φ)<1){Φ.HandBrake=true;if(Χ.waitType==DriveSettings.
WaitType.Timerblock){if(Ӊ!=null){Ӊ.myTimer.StartCountdown();}if(ӈ!=null){ӈ.myTimer.Trigger();}ʵ=AutoDriveState.Parking_Waiting;ϡ
=DateTime.UtcNow;}else if(Χ.waitType==DriveSettings.WaitType.External){ʵ=AutoDriveState.ExternalWait;}else{ʵ=
AutoDriveState.Driving;ƥ();}}}else if(ʵ==AutoDriveState.AwaitingStart){Ц(0);}else if(ʵ==AutoDriveState.StuckStop){Ц(0);if(с(Φ)<1){ʵ=
AutoDriveState.Reversing;}Ј.ForEach(K=>K.Enabled=true);Љ.ForEach(K=>K.Enabled=true);}else if(ʵ==AutoDriveState.Reversing){if(Ͷ>=ͳ){if(
ͱ==DateTime.MinValue){ͱ=DateTime.UtcNow;Ц(0);Echo("⚠️ Too many recovery attempts. Cooling down...");}if((DateTime.UtcNow-
ͱ).TotalSeconds>Ͳ){Ͷ=0;ͱ=DateTime.MinValue;ʵ=AutoDriveState.Driving;ʳ=false;ʭ=DateTime.MinValue;}else{Ц(0);return;}}else{
ʳ=true;long д=Φ.CubeGrid.EntityId;bool г=ʰ.Exists(K=>K.EntityId!=д&&K.Velocity.LengthSquared()>2&&K.Type!=
MyDetectedEntityType.Planet&&Vector3D.Distance(K.Position,å)<20);bool в=ʰ.Exists(K=>{if(K.EntityId==д)return false;if(K.Type==
MyDetectedEntityType.Planet)return false;if(K.Type==MyDetectedEntityType.FloatingObject)return false;double ī=Vector3D.Distance(K.Position,å
);bool л=Vector3D.Dot(Vector3D.Normalize(K.Position-å),Φ.WorldMatrix.Backward)>0.8;if(!л)return false;if(K.Type==
MyDetectedEntityType.LargeGrid)return ī<3.0;return ī<8.0;});if(г||в){Ц(0);ˇ=DateTime.UtcNow;Echo(в?"🛑 Obstacle Behind! Waiting...":
"🛑 Traffic! Waiting...");}else{foreach(var й in Ρ)й.SteeringOverride=0f;ϭ=å+Φ.WorldMatrix.Backward*10;Ц(MIN_SPEED);if((DateTime.UtcNow-ˇ).
TotalSeconds>Ύ){ʵ=AutoDriveState.Driving;ʳ=false;ʭ=DateTime.MinValue;Ͷ++;}}}Ј.ForEach(K=>K.Enabled=true);Љ.ForEach(K=>K.Enabled=true
);}else if(ʵ==AutoDriveState.RechargeWait){Φ.HandBrake=true;Ц(0);float I=0;float H=0;bool к=false;foreach(var G in ρ){I+=
G.CurrentStoredPower;H+=G.MaxStoredPower;if(G.ChargeMode==ChargeMode.Recharge){к=true;}}float A=(H>0)?(I/H)*100:0;if(A>95
){Φ.HandBrake=false;ʵ=AutoDriveState.Driving;ƥ();}else{Echo($"RechargeWait: Battery at {A:F1}%");}}else if(ʵ==
AutoDriveState.CargoWait){Φ.HandBrake=true;Ц(0);float D=0;foreach(var B in π){var R=B.GetInventory(0);if(R!=null){D+=(float)R.
CurrentVolume;}}if(ϸ<0){ϸ=D;Є=DateTime.UtcNow;return;}if(Math.Abs(D-ϸ)>0.01f){ϸ=D;Є=DateTime.UtcNow;Echo(
"CargoWait: Transfer in progress...");}else{if((DateTime.UtcNow-Є).TotalSeconds>5.0){Φ.HandBrake=false;ʵ=AutoDriveState.Driving;ϸ=-1f;ƥ();}else{Echo(
$"CargoWait: Waiting for transfer... ({(DateTime.UtcNow-Є).TotalSeconds:F1}s)");}}}else if(ʵ==AutoDriveState.ExternalWait){if(ς){Φ.HandBrake=false;ʵ=AutoDriveState.Driving;ƥ();ς=false;}else{Ц(0);}}}
else{if(ʵ==AutoDriveState.Stopping){Ц(0);if(с(Φ)<1){Φ.HandBrake=true;ʵ=AutoDriveState.Idle;Π.ForEach(K=>{K.Enabled=false;K.
Color=Color.Red;});if(Ϫ)Ã(Ϭ.ToString(),MessageType.DestinationConnectorDisconnect,IGC_TAG,Ϥ);Ϫ=false;}}else if(ʵ!=
AutoDriveState.AwaitingStart){ʵ=AutoDriveState.Idle;Ρ.ForEach(й=>{й.PropulsionOverride=0;й.SteeringOverride=0;й.Friction=50;});}else
if(ʵ==AutoDriveState.AwaitingStart){Ц(0);}}и();}void и(){Echo($"RoadPilot Agent Script V1.01 {ύ[Ϡ]}");Echo("DriveState: "+
ʵ);Echo($"Speed: "+(int)с(Φ)+" m/s");Echo("ChoosenDestCount: "+ˊ.Count);Echo($"Address: {IGC.Me.ToString()}");Echo(
$"Last runtime: {Runtime.LastRunTimeMs}");Echo($"Instructions: {Runtime.CurrentInstructionCount}");int V=0;if(Ќ.Count>0){List<Reservation>з=Њ.FindAll(K=>K.
intersection==Ќ[0].position).OrderBy(K=>K.sendTime).ThenBy(K=>K.address).ToList();foreach(var Ð in з){V++;}}Ϡ++;if(Ϡ>ύ.Length-1)Ϡ=0;
}void ж(){е();З();}void е(){int ơ=Math.Min(5,ˏ.Count);Vector3D б=Ƣ(ơ);double Ę=є(б);ˬ=new Vector3D(Ų(ˏ[0],0));var О=
Vector3D.Cross(ˬ,ˏ[0].position);О.Normalize();Ͱ=new Vector3D(-О*Ϋ/2+Ų(ˏ[0],0));ˮ=new Vector3D(О*Ϋ/2+Ų(ˏ[0],0));if(ʱ.Count>1&&(
DateTime.UtcNow-ά).Seconds>5){Echo("Point: "+ƻ(ʱ[1].position,å,ϲ).Y);Echo("Center: "+ʻ.Center.Y);if(ƻ(ʱ[1].position,å,ϲ).Y>ʻ.
Center.Y&&(ʵ!=AutoDriveState.PerformingTurn_1||ʵ!=AutoDriveState.PerformingTurn_2||ʵ!=AutoDriveState.StopForTurn)&&ʱ.Count>2){
ʵ=AutoDriveState.StopForTurn;Ц(0);}}if(ʵ==AutoDriveState.StopForTurn){if(с(Φ)<5){ʵ=AutoDriveState.PerformingTurn_1;}}if(ʯ
!=CollisionStatus.Idle){ʳ=false;Ц(0);if(с(Φ)<5&&ʯ==CollisionStatus.PathFound){ʯ=CollisionStatus.StartCalculating;}}else if
(ʵ==AutoDriveState.StartingTurn||ʵ==AutoDriveState.PerformingTurn_1||ʵ==AutoDriveState.PerformingTurn_2||ˎ.Count>0){Ц(
MIN_SPEED);Н(MIN_SPEED);}else if(ʵ==AutoDriveState.Driving){ʳ=false;double М=п(Ę);Ц((float)М);Н((float)М);}}void Н(float М){
Vector3D ǽ;if(Χ.collisionType==DriveSettings.CollisionType.SensorDriven){Υ.Enabled=true;ϰ.Enabled=true;}else{Υ.Enabled=false;ϰ.
Enabled=false;}try{if(ˏ.Count>1){if(Vector3D.Distance(å,Ż(ˏ[0],ˏ[1],Ɖ(å,Φ.WorldMatrix.Forward,Д),ʲ))>Г){Vector3D Л=Ż(ˏ[0],ˏ[1],
Ɖ(å,Φ.WorldMatrix.Forward,Д),ʲ);ǽ=Ǚ(ˏ[0].position,ˏ[1].position,Л,LANE_WIDTH);}else{Vector3D Л=Ż(ˏ[0],ˏ[1],Ɖ(å,Φ.
WorldMatrix.Forward,Д),ʲ);ǽ=Ǚ(ˏ[0].position,ˏ[1].position,Л,LANE_WIDTH);}var К=Ō(ǽ);var Й=К.Direction;if(Й==Vector3D.Zero){Ц(0);}if
(Vector3D.Distance(å,ˊ[ˉ].position)<100)М=10;Ц(М*К.SpeedFactor);Vector3D И=Φ.CenterOfMass;Й=И+Й*ѵ.Ѵ(((М*К.SpeedFactor)/
MAX_SPEED)*Δ,8,Δ);Ы(Й);ϭ=Й;}else if(ˏ.Count==1&&ː.Count>1){Vector3D Л=Ż(ˏ[0],ː[ː.Count-2],å,ʲ);ǽ=Ů(ˏ[0],Ɖ(ˏ[0].position,Φ.
WorldMatrix.Forward,10),0);var К=Ō(ǽ);var Й=К.Direction;Ц(М*К.SpeedFactor);Vector3D И=Φ.CenterOfMass;Й=И+Й*ѵ.Ѵ(((М*К.SpeedFactor)/
MAX_SPEED)*Δ,8,Δ);Ы(Й);ϭ=Й;}else if(ˏ.Count==1&&ː.Count==1){var К=Ō(ˏ[0].position);var Й=К.Direction;if(Vector3D.Distance(å,ˊ[ˉ].
position)<100)М=10;Ц(М*К.SpeedFactor);Vector3D И=Φ.CenterOfMass;Й=И+Й*ѵ.Ѵ(((М*К.SpeedFactor)/MAX_SPEED)*Δ,8,Δ);Ы(Й);ϭ=Й;}}catch(
Exception ex){}}void З(){if(ˏ.Count==1&&Vector3D.Distance(å,ˏ[0].position)<Д+ʲ){ʶ=ћ(ˏ.GetRange(0,ˏ.Count-1));if(Χ.parkingType==
DriveSettings.ParkingType.Connector&&Ϫ){ƨ();ʵ=AutoDriveState.Parking_Stopping;}else{ʵ=AutoDriveState.Stopping;}switch(Χ.
navigationType){case DriveSettings.NavigationType.Circle:if(ˊ.Count<=1){ϣ=false;ʵ=AutoDriveState.Stopping;ʴ=false;return;}Ϣ=(ˉ+1)%ˊ.
Count;ϣ=true;break;case DriveSettings.NavigationType.OneWay:ϣ=true;Ϣ=-1;break;case DriveSettings.NavigationType.Patrol:if(ˊ.
Count<=1){ϣ=false;ʵ=AutoDriveState.Stopping;ʴ=false;return;}if(ˉ+1>=ˊ.Count){Ϣ=0;Ϣ=-2;}else{Ϣ=ˉ+1;}ϣ=true;break;}if(Ќ.Count>0
){if(ˏ[0]==Ќ[0]){Æ($"{Ћ.intersection}",MessageType.RemoveIntersection,IGC_TAG,TransmissionDistance.
TransmissionDistanceMax);Ћ=null;Њ.RemoveAll(K=>K.intersection==Ќ[0].position);Ќ.RemoveAt(0);}}ʹ=0;}try{if(ˏ==null||ˏ.Count==0)return;else if(ˏ.
Count>1){if(Vector3D.Distance(å,ˏ[1].position)<Д+ʲ){ʶ=ћ(ˏ.GetRange(0,ˏ.Count-1));if(ˏ.Count>0){if(Ќ.Count>0){if(ˏ[0]==Ќ[0]){Æ
($"{Ћ.intersection}",MessageType.RemoveIntersection,IGC_TAG,TransmissionDistance.TransmissionDistanceMax);Ћ=null;Њ.
RemoveAll(K=>K.intersection==Ќ[0].position);Ќ.RemoveAt(0);}}ˏ.RemoveAt(0);}ʹ++;}}if(ʹ>=ˏ.Count)ʹ=Math.Max(0,ˏ.Count-1);}catch(
Exception e){Echo(e.Message);Echo(e.StackTrace);}}void Ж(ref Node Ƥ,ref Node ƣ,bool Р=true){Vector3D Й=ˊ[ˉ].position;var а=ć;if(!
Р){Й=ˏ[1].position;а=ʼ;}double Я=double.MaxValue;double Ю=double.MaxValue;foreach(var Z in а.nodes){double Э=Vector3D.
Distance(å,Z.position);double Ь=Vector3D.Distance(Й,Z.position);if(Э<Я){Я=Э;Ƥ=Z;}if(Ь<Ю){Ю=Ь;ƣ=Z;}}}void Ы(Vector3D Й,bool ʳ=
false,Vector3D Ъ=new Vector3D()){int Т=1;if(ʳ){Т=-1;}double Ę=0;if(Ъ.Equals(new Vector3D())){Ę=є(Й,ʳ);}else{Ę=є(Й,ʳ,Ъ);}
double Щ=0.01;double Ш=(Ę*Щ)*1.5f*STEERING_MULTIPLICATOR;Ш=ѵ.Ѵ(Ш,-1,1);ˁ=(float)Ш;foreach(var Ϟ in Ρ){float Ч=1.0f;if(ν.
ContainsKey(Ϟ)){Ч=ν[Ϟ];}Ϟ.SteeringOverride=(float)Ш*Т*Ч;}}void Ц(float М){float Х=50f;float Ф=15f;float У=100f;int Т=ʳ?-1:1;double
П=с(Φ)*Т;double С=М-П;const double м=0.2;double Ѳ=С*м;Ѳ=ѵ.Ѵ(Ѳ,-1,MAX_PROPULSION);Vector3D ϖ=Φ.CenterOfMass;Vector3D ѱ=Φ.
WorldMatrix.Forward;Vector3D Ѱ=Φ.WorldMatrix.Right;double ѯ=0;double Ѯ=0;foreach(var й in Ρ){double ī=Vector3D.Dot(й.GetPosition()-
ϖ,ѱ);if(ī>ѯ)ѯ=ī;if(ī<Ѯ)Ѯ=ī;}double ѭ=ѯ-Ѯ;double Ѭ=ѭ*0.25;float ѫ=Math.Abs(ˁ);foreach(var Ϟ in Ρ){float Ѫ=ξ.ContainsKey(Ϟ)
?ξ[Ϟ]:1f;float Ѩ=Т*Ѫ;double ѧ=Ѳ;float Ѧ=Х;bool ѥ=(П>0.5&&Ѳ<0)||(П<-0.5&&Ѳ>0);if(!ѥ){Vector3D Ѥ=Ϟ.GetPosition()-ϖ;double ѣ
=Vector3D.Dot(Ѥ,ѱ);double Ѣ=Vector3D.Dot(Ѥ,Ѱ);bool ѡ=Math.Abs(ѣ)<Ѭ;bool Ѡ=Ѣ>0;bool џ=false;if(ˁ>0.05f&&Ѡ)џ=true;else if(ˁ
<-0.05f&&!Ѡ)џ=true;if(ѫ>0.1f){if(ѡ){Ѧ=Ȥ(Х,Ф,ѫ*2.0f);ѧ*=(1.0-ѫ);}else if(џ){Ѧ=Ȥ(Х,Ф,ѫ);ѧ*=(1.0-ѫ);}else{Ѧ=Ȥ(Х,У,ѫ);ѧ*=(1.0
+(ѫ*0.5));}}}else{Ѧ=100f;if(Math.Abs(П)>0.5){double ѩ=Vector3D.Dot(Ϟ.GetPosition()-ϖ,ѱ);bool ѳ=(П>0&&ѩ>0)||(П<0&&ѩ<0);if(
ѳ)ѧ*=0.3;}}ѧ=ѵ.Ѵ(ѧ,-1.0,1.0);Ϟ.PropulsionOverride=(float)(ѧ*Ѩ);Ϟ.Friction=(float)ѵ.Ѵ(Ѧ,0f,100f);}ѷ(С);}void Ѹ(){if(Ќ.
Count==0||Vector3D.Distance(å,Ќ[0].position)>100){Ј.ForEach(K=>K.Enabled=false);Љ.ForEach(K=>K.Enabled=false);return;}int Ѻ=ˏ
.IndexOf(Ќ[0]);if(Ѻ<0||Ѻ+1>=ˏ.Count){Ј.ForEach(K=>K.Enabled=false);Љ.ForEach(K=>K.Enabled=false);return;}Vector3D ѹ=
Vector3D.Normalize(ˏ[Ѻ+1].position-Ќ[0].position);double э=Vector3D.Dot(ѹ,Φ.WorldMatrix.Forward);double у=Vector3D.Dot(Vector3D.
Cross(Φ.WorldMatrix.Forward,ѹ),Φ.WorldMatrix.Up);if(э>0.7){Ј.ForEach(K=>K.Enabled=false);Љ.ForEach(K=>K.Enabled=false);}else
if(у>0){Љ.ForEach(K=>K.Enabled=true);Ј.ForEach(K=>K.Enabled=false);}else{Ј.ForEach(K=>K.Enabled=true);Љ.ForEach(K=>K.
Enabled=false);}}void ѷ(double С){if(!ʳ&&С<0){Π.ForEach(K=>{K.Enabled=true;K.Color=Color.Red;});}else if(!ʳ&&С>0){Π.ForEach(K=>
{K.Enabled=false;K.Color=Color.Red;});}else if(ʳ){Π.ForEach(K=>{K.Enabled=true;K.Color=Color.White;});}}bool Ѷ(){if(ʵ!=
AutoDriveState.Driving||Ї){ʭ=DateTime.MinValue;return false;}double П=Math.Abs(с(Φ));if(П<Ό){if(ʭ==DateTime.MinValue){ʭ=DateTime.
UtcNow;}else if((DateTime.UtcNow-ʭ).TotalSeconds>Ώ){return true;}}else{ʭ=DateTime.MinValue;}return false;}static class ѵ{
public static double Ѵ(double ʀ,double ў,double ы){if(ʀ<ў)return ў;if(ʀ>ы)return ы;return ʀ;}}double є(Vector3D щ,bool ʳ=false
,Vector3D Ъ=new Vector3D()){double Ę;Vector3D ш;Vector3D ч;if(Ъ.Equals(new Vector3D())){ч=Φ.CenterOfMass;}else ч=Ъ;
Vector3D ц=щ-ч;if(!ʳ){ш=Φ.WorldMatrix.Forward;}else{ш=Φ.WorldMatrix.Backward;}Vector3D ȴ=Φ.WorldMatrix.Up;Vector3D х=(Vector3D.
Dot(ц,ȴ)/ȴ.LengthSquared())*ȴ;Vector3D ф=ц-х;if(ф.LengthSquared()==0||ш.LengthSquared()==0){return 0;}var у=Vector3D.Dot(ф,
ш);var ɤ=у/(ф.Length()*ш.Length());if(double.IsNaN(ɤ)||ɤ>1||ɤ<-1){ɤ=ѵ.Ѵ(ɤ,-1,1);}Ę=Math.Acos(ɤ);Ę=Ę*180/Math.PI;ф.
Normalize();var т=Vector3D.Cross(ф,ш);if(Vector3D.Dot(ȴ,т)<0){Ę=-Ę;}if(double.IsNaN(Ę)){return 0;}return Ę;}double с(
IMyShipController Φ){Vector3D р=Φ.GetShipVelocities().LinearVelocity;return р.Dot(Φ.WorldMatrix.GetDirectionVector(Base6Directions.
Direction.Forward));}double п(double Ę){double Ƨ=ѵ.Ѵ(1-Math.Abs(Ę)/90,0,1);double М=MIN_SPEED+(MAX_SPEED-MIN_SPEED)*Ƨ;return М;}
struct о{public double ъ;public double н;public double ɇ;public double ѝ;}о ћ(List<Node>њ){о љ=new о();if(ː!=null&&ː.Count>1){
for(int V=0;V<ː.Count-1;V++){љ.ɇ+=Vector3D.Distance(ː[V].position,ː[V+1].position);}}if(њ==null||њ.Count<2)return љ;
Vector3D ј=Ż(њ[0].position,њ[1].position,å);double ї=Vector3D.Distance(ј,њ[1].position);љ.ъ+=ї;double і=с(Φ);љ.н+=ї/Math.Max(і,
1.0);for(int V=1;V<њ.Count-1;V++){Vector3D ƃ=њ[V].position;Vector3D ѕ=њ[V+1].position;double ќ=Vector3D.Distance(ƃ,ѕ);љ.ъ+=
ќ;int ơ=Math.Min(5,њ.Count-V-2);Vector3D б=Ƣ(ơ);double ѓ=п(є(б,false,ƃ));љ.н+=ќ/Math.Max(ѓ,1.0);}return љ;}Vector3D Ż(
Vector3D ђ,Vector3D ё,Vector3D ē){Vector3D ѐ=ё-ђ;double я=ѐ.Length();if(я==0)return ђ;ѐ/=я;Vector3D ю=ē-ђ;double э=Vector3D.Dot(
ю,ѐ);э=ѵ.Ѵ(э,0,я);return ђ+ѐ*э;}float ь(float Ŀ){double ʬ=Φ.GetNaturalGravity().Length();if(ʬ<0.1)return(Ŀ*Ŀ)/(2*1.0f);
float ì=0.8f;float Ŗ=(float)(ʬ*ì);if(Ŗ<0.1f)Ŗ=0.1f;float ŕ=(Ŀ*Ŀ)/(2*Ŗ);return ŕ;}void Ŕ(){Vector3D œ=ˤ.CubeGrid.
GridIntegerToWorld(ˤ.Position);Vector3D Œ=ˣ.CubeGrid.GridIntegerToWorld(ˣ.Position);if(ʵ==AutoDriveState.PerformingTurn_1){ʳ=false;
Vector3D Ő=Ż(Ͱ,ˬ,œ,0);Vector3D ő=Ż(ˮ,ˬ,œ,0);Ρ.ForEach(K=>K.SteeringOverride=-1);if(Vector3D.Distance(Ő,Ͱ)<1||Vector3D.Distance(ő
,ˮ)<1){ʵ=AutoDriveState.PerformingTurn_2;}}if(ʵ==AutoDriveState.PerformingTurn_2){ʳ=true;Ρ.ForEach(K=>K.SteeringOverride=
1);Vector3D Ő=Ż(Ͱ,ˬ,Œ,0);Vector3D ő=Ż(ˮ,ˬ,Œ,0);if(Vector3D.Distance(Ő,Ͱ)<1||Vector3D.Distance(ő,ˮ)<1){ʵ=AutoDriveState.
PerformingTurn_1;}}if(ʱ.Count>1){if(ƻ(ʱ[1].position,å,ϲ).Y<ʻ.Center.Y&&(ʵ==AutoDriveState.PerformingTurn_1||ʵ==AutoDriveState.
PerformingTurn_2)){ʵ=AutoDriveState.Driving;}}}float[]ŏ={-75f,-45f,-15f,-5f,0f,5f,15f,45f,75f};int Ŏ=0;int ō=20;AvoidanceResult Ō(
Vector3D ŋ){const double Ŋ=4.0;const double ŉ=(Ί/2.0)+Ŋ;const double ň=ŉ*ŉ;float[]Ň=ŏ;ϯ.Clear();Ϯ.RemoveAll(K=>K.DetectedTime<
DateTime.UtcNow.AddSeconds(-1f));Ϯ.RemoveAll(K=>K.isMoving&&K.DetectedTime<DateTime.UtcNow.AddSeconds(-0.5f));ʰ.RemoveAll(K=>K.
Type==MyDetectedEntityType.CharacterHuman);double Ř=Ζ;if(ʰ.Count>0){Ř=50;int Ţ=Math.Min(ō,Ň.Length);for(int V=0;V<Ţ;V++){if(
Runtime.CurrentInstructionCount+5000>Runtime.MaxInstructionCount)break;float Ę=Ň[Ŏ];Ŏ++;if(Ŏ>=Ň.Length)Ŏ=0;var ĭ=ϰ.Raycast(Ř,0,
Ę);ϯ.Add(ĭ);if(Runtime.CurrentInstructionCount+5000<=Runtime.MaxInstructionCount){var Ū=ϰ.Raycast(Ř,10f,Ę);ϯ.Add(Ū);}}}
Vector3D ũ=Φ.GetNaturalGravity();Vector3D Ũ=Vector3D.Up;bool ŧ=false;if(ũ.LengthSquared()>0.001){ŧ=true;Ũ=-Vector3D.Normalize(ũ)
;}const double Ŧ=0.35;foreach(var ĭ in ϯ){if(Runtime.CurrentInstructionCount+5000>Runtime.MaxInstructionCount){break;}if(
!ĭ.IsEmpty()){Vector3D ť=ϰ.GetPosition();Vector3D Ť=(Vector3D)ĭ.HitPosition;Vector3D ū=Ť-ť;bool ţ=false;if(ĭ.Type==
MyDetectedEntityType.Planet&&ŧ){double š=Vector3D.Dot(ū,Ũ);double Š=ū.LengthSquared();double ş=Math.Sqrt(Math.Max(0,Š-(š*š)));if(ş>0.1){
double Ş=š/ş;if(Ş<Ŧ){ţ=true;}}}if(!ţ){HitPoint Ĭ=new HitPoint(DateTime.UtcNow,Ť,ĭ.EntityId,false,ĭ.Type);if(ĭ.Velocity.
LengthSquared()>2)Ĭ.isMoving=true;if(ū.LengthSquared()<0.0001)continue;double ė=Vector3D.Dot(Vector3D.Normalize(ū),Φ.WorldMatrix.
Forward);if(ė>0.1){Ϯ.Add(Ĭ);if(Ϯ.Count>Ε)Ϯ.RemoveAt(0);}}}}double ŝ=double.MaxValue;ʮ=null;var Ŝ=Φ.WorldMatrix.Forward;var ś=Φ.
GetShipVelocities().LinearVelocity;foreach(var Ś in ʰ){if(Ś.EntityId==Φ.EntityId)continue;Vector3D ř=Ś.Position-Φ.GetPosition();double Ņ=
Vector3D.Dot(Vector3D.Normalize(ř),Ŝ);if(Ņ<0.8){continue;};if(Ś.Velocity.LengthSquared()<1){if(Ќ.Count>0){if(Ќ[0]==ˏ[0]){
continue;}else if(Ћ==null){continue;}else if(Њ.FindAll(K=>K.intersection==Ќ[0].position).OrderBy(K=>K.sendTime).ThenBy(K=>K.
address).ToList().IndexOf(Ћ)==0){continue;}}else{continue;}}Vector3D į=Vector3D.Normalize(Ś.Velocity);double Į=Vector3D.Dot(į,Ŝ
);if(Į<0.5){if(Runtime.CurrentInstructionCount+5000>Runtime.MaxInstructionCount){break;}if(Ś.Velocity.LengthSquared()<2){
continue;}var ĭ=ϰ.Raycast(Ś.Position);if(!ĭ.IsEmpty()&&Ś.Type==MyDetectedEntityType.SmallGrid){if(Vector3D.Distance((Vector3D)ĭ.
HitPosition,Ś.Position)<5){HitPoint Ĭ=new HitPoint(DateTime.UtcNow,(Vector3D)ĭ.HitPosition,ĭ.EntityId,false,ĭ.Type);Ϯ.Add(Ĭ);}
continue;}}double ī=ř.Length();if(Į>0.8){if(ī<ŝ){ŝ=ī;ʮ=Ś;}}}if(ʮ.HasValue){Ϯ.RemoveAll(ĥ=>ʮ.Value.BoundingBox.Contains(ĥ.
Position)!=ContainmentType.Disjoint);double Ī=(ʮ.Value.Position-Φ.GetPosition()).Length();Ϯ.RemoveAll(ĥ=>Math.Abs((ĥ.Position-Φ.
GetPosition()).Length()-Ī)<5.0&&Vector3D.Dot(Vector3D.Normalize(ĥ.Position-Φ.GetPosition()),Φ.WorldMatrix.Forward)>0.7);}Vector3D ĩ
=ŋ==Φ.GetPosition()?Φ.WorldMatrix.Forward:Vector3D.Normalize(ŋ-Φ.GetPosition());double Ĩ=Vector3D.Dot(ĩ,Φ.WorldMatrix.
Right);if(Math.Abs(Ĩ)>0.1){ϵ=Ĩ>0?1:-1;}var ħ=new Dictionary<long,List<HitPoint>>();var Ħ=new Dictionary<string,List<HitPoint>
>();foreach(var ĥ in Ϯ){if(Runtime.CurrentInstructionCount+5000>Runtime.MaxInstructionCount){break;}if(ĥ.EntityId!=0){
List<HitPoint>ģ;if(!ħ.TryGetValue(ĥ.EntityId,out ģ)){ģ=new List<HitPoint>();ħ[ĥ.EntityId]=ģ;}ģ.Add(ĥ);}else{string Ĥ=Math.
Round(ĥ.Position.X,1).ToString("F1")+"_"+Math.Round(ĥ.Position.Y,1).ToString("F1")+"_"+Math.Round(ĥ.Position.Z,1).ToString(
"F1");List<HitPoint>ģ;if(!Ħ.TryGetValue(Ĥ,out ģ)){ģ=new List<HitPoint>();Ħ[Ĥ]=ģ;}ģ.Add(ĥ);}}var Ģ=new List<List<HitPoint>>()
;foreach(var ġ in ħ)Ģ.Add(ġ.Value);foreach(var ġ in Ħ)Ģ.Add(ġ.Value);bool Ġ=false;Vector3D İ=Vector3D.Zero;foreach(var ğ
in Ģ){if(Runtime.CurrentInstructionCount+5000>Runtime.MaxInstructionCount){break;}if(ğ.Count<2)continue;var ı=ğ.OrderBy(ĥ
=>ĥ.DetectedTime).ToList();var Ń=ı.First();var ł=ı.Last();double Ł=(ł.DetectedTime-Ń.DetectedTime).TotalSeconds;if(Ł<Λ)
continue;Vector3D ŀ=(ł.Position-Ń.Position)/Ł;double Ŀ=ŀ.Length();if(Ŀ<ͽ)continue;Vector3D ľ=ŀ-ś;Vector3D Ľ=ł.Position-Φ.
GetPosition();if(Ľ.LengthSquared()<0.0001)continue;double ļ=Vector3D.Dot(Vector3D.Normalize(Ľ),Vector3D.Normalize(ľ));if(ļ<ͼ){Ġ=
true;İ=ľ;break;}}float Ļ=float.MinValue;Vector3D ĺ=Φ.WorldMatrix.Forward;double ń=double.MaxValue;int Ĺ=-75;int ĸ=75;int ķ=-
1;double Ķ=double.MaxValue;if(Ġ&&İ.Length()>0.001){double ĵ=Ϯ.Count>0?Ϯ.Min(ĥ=>(ĥ.Position-Φ.GetPosition()).Length()):
double.MaxValue;if(ĵ<double.MaxValue){Vector3D Ĵ=Vector3D.Normalize((Ϯ.OrderBy(ĥ=>(ĥ.Position-Φ.GetPosition()).Length()).First
()).Position-Φ.GetPosition());double ĳ=Math.Abs(Vector3D.Dot(İ,Ĵ));if(ĳ>0.001)Ķ=ĵ/ĳ;}}Ϯ.RemoveAll(K=>Vector3D.Distance(K.
Position,ϰ.GetPosition())>Έ&&K.type!=MyDetectedEntityType.SmallGrid);for(int V=Ĺ;V<=ĸ;V++){if(Runtime.CurrentInstructionCount+
5000>Runtime.MaxInstructionCount){break;}Vector3D ņ=Vector3D.TransformNormal(Vector3D.Rotate(Vector3D.Forward,MatrixD.
CreateFromAxisAngle(Vector3D.Up,MathHelperD.ToRadians(V))),Φ.WorldMatrix);Vector3D Ŭ=Vector3D.Normalize(ņ);double ƈ=Ζ;bool Ɵ=false;float Ɲ=
0f;foreach(var Ɯ in Ϯ){Vector3D ū=Ɯ.Position-Φ.GetPosition();double ƛ=ū.Length();if(ƛ<1.0)continue;double ƚ=Vector3D.Dot(ū
,Ŭ);if(ƚ<=0)continue;double ƙ=ū.LengthSquared()-(ƚ*ƚ);if(ƙ<Ή){double Ƙ=ƚ-Γ;if(Ƙ<0)Ƙ=0;if(Ƙ<ƈ){Ɵ=(Ɯ.type==
MyDetectedEntityType.Planet||Ɯ.type==MyDetectedEntityType.LargeGrid);ƈ=Ƙ;}}else if(ƙ<ň){double Ɨ=Math.Sqrt(ƙ);double Ɩ=Ί/2.0;double ƕ=(Ɨ-Ɩ)/
Ŋ;float ƞ=(float)((1.0-ƕ)*0.5);if(ƞ>Ɲ)Ɲ=ƞ;}double ė=Vector3D.Dot(Ŭ,ū/ƛ);if(ė>0.8&&ƛ<ń)ń=ƛ;}float Ɠ=(float)(ƈ/Ζ);if(ƈ<Έ){Ɠ
-=2000f;}else if(Ɵ&&ƈ<Έ){Ɠ-=1000f;}Ɠ-=Ɲ;float ƒ=(float)Vector3D.Dot(Ŭ,Vector3D.Normalize(ĩ));if(Vector3D.Dot(Vector3D.
Normalize(ĩ),Φ.WorldMatrix.Forward)<0.2){double Ƒ=Vector3D.Dot(Ŭ,Φ.WorldMatrix.Right);int Ɛ=Ƒ>0.05?1:(Ƒ<-0.05?-1:0);if(Ɛ!=0&&Ɛ!=ϵ
){ƒ-=0.3f;}}float Ə=(float)(1.0-(ƈ/Ζ));double Ǝ=Math.Max(-1.0,Math.Min(1.0,Vector3D.Dot(Vector3D.Normalize(Φ.WorldMatrix.
Forward),Ŭ)));float ƍ=(float)((Math.Acos(Ǝ)*(180.0/Math.PI))/90.0);float ƌ=(Ɠ*Θ)+(ƒ*Η)-(Ə*Β)-(ƍ*Ά);double Ƌ=Math.Acos(Ǝ)*(180.0
/Math.PI);double Ɗ=Math.Sign(Vector3D.Dot(Vector3D.Cross(Φ.WorldMatrix.Forward,ņ),Φ.WorldMatrix.Up));double Ɣ=Ƌ*Ɗ;if(Ġ&&
Math.Abs(Ɣ)<=ͺ){int Ɛ=(Ɣ>0)?1:(Ɣ<0)?-1:0;if(Ɛ==ķ)ƌ+=Κ*3;else if(Ɛ!=0)ƌ-=(Κ*0.25f);}if(ƌ>Ļ){Ļ=ƌ;ĺ=ņ;}}float Ƨ=1.0f;if(Ļ<-500f
){Ƨ=0.0f;}else if(ń<Α){Ƨ=0.0f;}else if(ń<ΐ){Ƨ=(float)((ń-Α)/(ΐ-Α));Ƨ=(float)ѵ.Ѵ(Ƨ,0,1);}float Ʋ=1.0f;if(ʮ.HasValue){
double ŕ=ŝ;if(ŕ<γ){Ʋ=0f;}else if(ŕ<ε){Ʋ=(float)((ŕ-γ)/(ε-γ));Ʋ=(float)ѵ.Ѵ(Ʋ,0f,1f);}double ư=с(Φ);double Ư=ʮ.Value.Velocity.
Dot(Φ.WorldMatrix.Forward);if(Ư<ư){double Ʈ=(Ư<=0)?0:(Ư/ư);Ʋ=Math.Min(Ʋ,(float)Ʈ);}}Vector3D ƭ=Φ.WorldMatrix.Forward;double
Ƭ=Vector3D.Dot(ƭ,ĺ);float ƫ=1.0f;double ƪ=MathHelperD.ToDegrees(Math.Acos(ѵ.Ѵ(Ƭ,-1.0,1.0)));if(ƪ>5.0){double ŵ=(ƪ-5.0)/(
45.0-5.0);ŵ=ѵ.Ѵ(ŵ,0f,1f);ƫ=Ȥ(1.0f,0.3f,(float)ŵ);}float Ʃ=Math.Min(Ƨ,Ʋ);Ʃ=Math.Min(Ʃ,ƫ);double Ʊ=Vector3D.Dot(ĺ,Φ.
WorldMatrix.Right);if(Math.Abs(Ʊ)>0.1){ϵ=Ʊ>0?1:-1;}return new AvoidanceResult{Direction=ĺ,SpeedFactor=Ʃ};}void ƨ(){Node Ʀ=new Node(
Vector3D.Zero);float ƈ=float.MaxValue;foreach(var Z in ć.nodes){float ŕ=(float)Vector3D.Distance(Ϭ,Z.position);if(ŕ<ƈ){ƈ=ŕ;Ʀ=Z;}
}ϥ=Ų(Ʀ,0);ʵ=AutoDriveState.Parking_Stopping;}void ƥ(){if(ϣ){if(Ϣ==-1){if(ˊ.Count>0)ˊ.RemoveAt(0);ˉ=0;}else if(Ϣ==-2){ˊ.
Reverse();if(ˊ.Count>1)ˉ=1;else ˉ=0;}else{if(Ϣ>=0&&Ϣ<ˊ.Count)ˉ=Ϣ;}ϣ=false;}if(ˊ.Count==0){ʴ=false;ˋ.ForEach(K=>K.isChoosen=
false);ʵ=AutoDriveState.Idle;ˏ.Clear();}else if(ˊ.Count>0){Φ.HandBrake=false;if(ϧ!=null)ϧ.Disconnect();if(Ϫ)Ã(Ϭ.ToString(),
MessageType.DestinationConnectorDisconnect,IGC_TAG,Ϥ);Ϭ=new Vector3D();Ϫ=false;Node Ƥ=new Node(Vector3D.Zero);Node ƣ=new Node(
Vector3D.Zero);Ж(ref Ƥ,ref ƣ);if(Ƥ!=null&&ƣ!=null){ˍ=new ȋ();ˍ.Ǿ(Ƥ,ƣ,Φ.WorldMatrix.Forward,ȏ,TURNAROUND_PATHFINDING_PENALTY);}č(
"Navigation",$"Traveling to {ˊ[ˉ].name}",Color.Green);}}Vector3D Ƣ(int ơ){Vector3D Ơ=Vector3D.Zero;for(int V=0;V<ơ;V++){Ơ+=ˏ[V].
position;}return Ơ/ơ;}Vector3D Ż(Node Ƃ,Node ż,Vector3D Ÿ,double Ű){return Ż(Ƃ.position,ż.position,Ÿ,Ű);}Vector3D Ż(Vector3D ź,
Vector3D Ź,Vector3D Ÿ,double Ű){Vector3D ŷ=Ź-ź;Vector3D Ŷ=Ÿ-ź;double ŵ=Vector3D.Dot(Ŷ,ŷ)/Vector3D.Dot(ŷ,ŷ);double Ŵ=ŷ.Length();
double ų=Ű/Ŵ;ŵ=Math.Max(ų,Math.Min(1,ŵ));Vector3D ů=ź+ŵ*ŷ;return ů;}Vector3D Ų(Node ű,double Ű){var ů=ű.position;float ŕ=float
.MaxValue;foreach(var Z in ű.connectedNodes){if(Vector3D.Distance(Ż(ű,Z,å,Ű),å)<ŕ){ŕ=(float)Vector3D.Distance(Ż(ű,Z,å,Ű),
å);ů=Ż(ű,Z,å,Ű);}}return ů;}Vector3D Ů(Node ű,Vector3D ŭ,double Ű){Vector3D ů=ű.position;double ƈ=double.MaxValue;
Vector3D Ƈ=Φ.WorldMatrix.Forward;Vector3D Ɔ=Φ.CenterOfMass;foreach(var Z in ű.connectedNodes){Vector3D Ƅ=Ż(ű,Z,ŭ,Ű);double ī=
Vector3D.Distance(Ƅ,ŭ);Vector3D ƅ=Ƅ-Ɔ;if(Vector3D.Dot(ƅ,Ƈ)<0)continue;if(ī<ƈ){ƈ=ī;ů=Ƅ;}}if(ƈ==double.MaxValue){foreach(var Z in
ű.connectedNodes){Vector3D Ƅ=Ż(ű,Z,ŭ,Ű);double ī=Vector3D.Distance(Ƅ,ŭ);if(ī<ƈ){ƈ=ī;ů=Ƅ;}}}return ů;}Vector3D Ɖ(Vector3D
ƃ,Vector3D ŷ,double ŕ){Vector3D Ɓ=Vector3D.Normalize(ŷ);Vector3D ƀ=Ɓ*ŕ;Vector3D ſ=ƃ+ƀ;return ſ;}List<Message>ž(){List<
Message>Ì=new List<Message>();for(int V=0;V<ζ.Count;V++){var Ž=ζ[V];while(Ž.HasPendingMessage){var Ë=Ž.AcceptMessage();var Ê=Ë.
Data as string;if(string.IsNullOrEmpty(Ê))continue;string[]É=Ê.Split('^');if(É.Length<4)continue;Vector3D È;Vector3D.
TryParse(É[2],out È);MessageType Á;if(!Enum.TryParse(É[1],out Á))continue;long Ç;long.TryParse(É[3],out Ç);Ì.Add(new Message{
Type=Á,Content=É[0],Address=Ç,Position=È,RecievedTime=DateTime.UtcNow});}}return Ì;}List<Message>Î(){var Í=IGC.
UnicastListener;List<Message>Ì=new List<Message>();while(Í.HasPendingMessage){var Ë=Í.AcceptMessage();var Ê=Ë.Data as string;if(string.
IsNullOrEmpty(Ê))continue;string[]É=Ê.Split('^');if(É.Length<4)continue;Vector3D È;Vector3D.TryParse(É[2],out È);MessageType Á;if(!
Enum.TryParse(É[1],out Á))continue;long Ç;long.TryParse(É[3],out Ç);Ì.Add(new Message{Type=Á,Content=É[0],Address=Ç,Position
=È,RecievedTime=DateTime.UtcNow});}return Ì;}void Æ(string Â,MessageType Á,string À,TransmissionDistance Å){List<string>º
=new List<string>();º.Add(Â);º.Add(Á.ToString());º.Add(å.ToString());º.Add(IGC.Me.ToString());string Ä=string.Join("^",º)
;IGC.SendBroadcastMessage(À,Ä,Å);}void Ã(string Â,MessageType Á,string À,long Ï){List<string>º=new List<string>();º.Add(Â
);º.Add(Á.ToString());º.Add(Me.GetPosition().ToString());º.Add(IGC.Me.ToString());string Ä=string.Join("^",º);IGC.
SendUnicastMessage(Ï,À,Ä);}void Ó(){List<Message>Ì=ž();if(Ì.Any()){foreach(var Ò in Ì){if(Ò.Type==MessageType.MapRequest){Ã(Ĉ(ć),
MessageType.MapUnicast,IGC_TAG,Ò.Address);}if(Ò.Type==MessageType.ReserveIntersection&&Ќ.Count>0&&Ћ!=null){string[]Ê=Ò.Content.
Split(';');Vector3D È=new Vector3D();Vector3D.TryParse(Ê[1],out È);Reservation Ð=new Reservation(long.Parse(Ê[0]),Ò.Address,È
);if(!Њ.Exists(K=>K.address==Ð.address)&&Ð.intersection==Ћ.intersection){Њ.Add(Ð);Њ=Њ.OrderBy(K=>K.sendTime).ThenBy(K=>K.
address).ToList();}}if(Ò.Type==MessageType.RequestReservations&&Ћ!=null){Ã($"{Ћ.sendTime};{Ћ.intersection};",MessageType.
ReserveIntersection,IGC_TAG,Ò.Address);}if(Ò.Type==MessageType.RemoveIntersection&&Ћ!=null){Vector3D È=new Vector3D();Vector3D.TryParse(Ò.
Content,out È);if(Њ.IndexOf(Ћ)>1&&È==Ћ.intersection){Њ.RemoveAll(K=>K.address==Ò.Address);}else if(È==Ћ.intersection){Ѓ=Ò;Ѕ=
DateTime.UtcNow;}}if(Ò.Type==MessageType.MapBroadcast&&ͷ!=SaveStatus.UnloadingNodes){ˑ=Ò.Content.Split(';');ʿ=0;ʾ=0;ʽ=0;ˆ=true;č
("Map Update","New map available",Color.Green);}if(Ò.Type==MessageType.StatusRequest){Ã(O(),MessageType.StatusUpdate,
IGC_TAG,Ò.Address);}}}Ì.Clear();Ì=Î();if(Ì.Any()){foreach(var Ò in Ì){if(Ò.Type==MessageType.DestinationAddress){Destination Ñ=
new Destination(Ò.Content,Ò.Position,Ò.Address,Ò.RecievedTime);if(!ˋ.Exists(K=>K.address==Ñ.address)){ˋ.Add(Ñ);}else{ˋ.Find
(K=>K.address==Ñ.address).lastPing=Ñ.lastPing;ˋ.Find(K=>K.address==Ñ.address).name=Ñ.name;if(ˊ.Exists(K=>K.address==Ñ.
address))ˊ.Find(K=>K.address==Ñ.address).name=Ñ.name;}}if(Ò.Type==MessageType.MapUnicast&&ͷ!=SaveStatus.UnloadingNodes&&ͷ!=
SaveStatus.Unloaded){ˑ=Ò.Content.Split(';');ʿ=0;ʾ=0;ʽ=0;ˆ=true;č("Map Update","New map available",Color.Yellow);}if(Ò.Type==
MessageType.DestinationConnector&&ˊ.Count>0){if(Ò.Content.Contains("Available")&&Ò.Address==ˊ[ˉ].address){Ϫ=true;Ϭ=Ò.Position;Ϥ=Ò.
Address;var Ê=Ò.Content.Split('|');Vector3D.TryParse(Ê[1],out ϩ);č("Parking",$"Connector reserved at {ˊ[ˉ].name}",Color.Green);
}else if(Ò.Content.Equals("Unavailable")&&Ò.Address==ˊ[ˉ].address){if(Ϫ){č("Parking",$"Connector timed out {ˊ[ˉ].name}",
Color.Red);}Ϫ=false;}}if(Ò.Type==MessageType.ReserveIntersection&&Ќ.Count>0&&Ћ!=null){string[]Ê=Ò.Content.Split(';');Vector3D
È=new Vector3D();Vector3D.TryParse(Ê[1],out È);Reservation Ð=new Reservation(long.Parse(Ê[0]),Ò.Address,È);if(!Њ.Exists(K
=>K.address==Ð.address)&&Ð.intersection==Ћ.intersection){Њ.Add(Ð);Њ=Њ.OrderBy(K=>K.sendTime).ThenBy(K=>K.address).ToList()
;}}if(Ò.Type==MessageType.ParkingPing&&Ϫ){Ã("Pong",MessageType.ParkingPing,IGC_TAG,Ò.Address);}if(Ò.Type==MessageType.
Command){ҋ(Ò.Content,Ò.Address);}}}if((DateTime.UtcNow-ˌ).TotalMinutes>Е){Æ("v",MessageType.DestinationRequest,IGC_TAG,
TransmissionDistance.TransmissionDistanceMax);ˌ=DateTime.UtcNow;}List<Destination>µ=new List<Destination>();foreach(var Q in ˋ){if((DateTime
.UtcNow-Q.lastPing).TotalMinutes>Е*2){µ.Add(Q);}}foreach(var Q in µ){ˋ.Remove(Q);}if(ć.nodes.Count==0&&ͷ!=SaveStatus.
UnloadingNodes){Æ(" ",MessageType.MapRequest,IGC_TAG,TransmissionDistance.AntennaRelay);ͷ=SaveStatus.Idle;Echo("Requesting map");}if(ʵ
==AutoDriveState.Driving&&Χ.parkingType==DriveSettings.ParkingType.Connector&&Ϫ==false&&(DateTime.UtcNow-Ϩ).TotalSeconds>
10&&ˊ.Count>0){Ã($"{Ȭ(ϧ)}",MessageType.DestinationConnector,IGC_TAG,ˊ[ˉ].address);Echo(
$"Requesting connector at: {ˊ[ˉ].address}");Ϩ=DateTime.UtcNow;}if(Ќ.Count>0){if(ʵ==AutoDriveState.Driving&&Vector3D.Distance(å,Ќ[0].position)<ί&&Ћ==null){Ћ=new
Reservation(DateTime.UtcNow.ToBinary(),IGC.Me,Ќ[0].position);Æ($"{Ћ.sendTime};{Ћ.intersection};",MessageType.ReserveIntersection,
IGC_TAG,TransmissionDistance.TransmissionDistanceMax);Æ("",MessageType.RequestReservations,IGC_TAG,TransmissionDistance.
TransmissionDistanceMax);Њ.Add(Ћ);}Њ.RemoveAll(K=>(DateTime.UtcNow-DateTime.FromBinary(K.sendTime)).TotalSeconds>30);if((DateTime.UtcNow-Ѕ).
TotalSeconds>ή&&Ѓ!=null){Њ.RemoveAll(K=>K.address==Ѓ.Address);Ѓ=null;}}int P=DateTime.Now.Second/5;if(P!=ο){ο=P;Æ(O(),MessageType.
StatusUpdate,IGC_TAG,TransmissionDistance.TransmissionDistanceMax);}}string O(){string M=
$"{Me.CubeGrid.CustomName};{N()};{J()};{E()};{с(Φ)};{ˊ.Count};{ˉ};{Χ.navigationType};{ʶ.ъ};{ʶ.н};{ʶ.ɇ};{ʶ.ѝ}";return M;}string N(){string M="Idle";switch(ʵ){case AutoDriveState.AwaitingStart:break;case AutoDriveState.Driving:if(ˊ
.Count>0){if(Ї&&Ќ.Count>0&&Vector3D.Distance(Ќ[0].position,å)<50){M=$"Waiting at intersection";}else{M=
$"Driving to {ˊ[ˉ].name}";}}break;case AutoDriveState.Idle:if(ϧ!=null){if(ϧ.IsConnected&&ˋ.Count>0){M=
$"Parked at {ˋ.OrderBy(K=>Vector3D.DistanceSquared(å,K.position)).FirstOrDefault().name}";}}else if(ˋ.Count>0){var L=ˋ.OrderBy(K=>Vector3D.DistanceSquared(å,K.position)).FirstOrDefault();if(Vector3D.Distance(L
.position,å)<200)M=$"Idle near {L.name}";}break;case AutoDriveState.Stopping:M=$"Stopping";break;case AutoDriveState.
Parking_Stopping:M=$"Parking at {ˊ[ˉ].name}";break;case AutoDriveState.Parking_Creep1:M=$"Parking at {ˊ[ˉ].name}";break;case
AutoDriveState.Parking_Creep2:M=$"Parking at {ˊ[ˉ].name}";break;case AutoDriveState.Parking_Reverse1:M=$"Parking at {ˊ[ˉ].name}";break
;case AutoDriveState.Parking_Reverse2:M=$"Parking at {ˊ[ˉ].name}";break;case AutoDriveState.Parking_Correction:M=
$"Parking at {ˊ[ˉ].name}";break;case AutoDriveState.Parking_Waiting:M=$"Parking at {ˊ[ˉ].name}";break;case AutoDriveState.StuckStop:M=
$"Getting unstuck";break;case AutoDriveState.Reversing:M=$"Getting unstuck";break;case AutoDriveState.StopForTurn:break;case
AutoDriveState.StartingTurn:break;case AutoDriveState.CargoWait:M=$"(Un)loading at {ˊ[ˉ].name}";break;case AutoDriveState.RechargeWait
:M=$"Recharging at {ˊ[ˉ].name}";break;case AutoDriveState.ExternalWait:M=$"Awaiting trigger";break;default:break;}return
M;}string J(){float I=0;float H=0;foreach(var G in ρ){I+=G.CurrentStoredPower;H+=G.MaxStoredPower;}float A=(H>0)?(I/H)*
100:0;string F=$"{A}";return F;}string E(){float D=0;float C=0;foreach(var B in π){var R=B.GetInventory(0);if(R==null)
continue;D+=(float)R.CurrentVolume;C+=(float)R.MaxVolume;}float A=(C>0)?(D/C)*100:0;string U=$"{A}";return U;}void ª(string[]q){
if(ʿ==0){ʸ.Clear();ć.nodes.Clear();}for(int V=0;V<Ȃ&&ʿ<q.Length;V++,ʿ++){Node u=new Node(Vector3D.Zero);string[]o=q[ʿ].
Split('/');Vector3D.TryParse(o[0],out u.position);ć.nodes.Add(u);if(!ʸ.ContainsKey(u.position)){ʸ.Add(u.position,u);}}}void s
(string[]q){for(int V=0;V<Ȃ&&ʾ<ć.nodes.Count;V++){Node p=ć.nodes[ʾ];string[]o=q[ʾ].Split('/');if(o.Length<2){ʾ++;continue
;}string[]m=o[1].Split('|');for(int v=0;v<Ȃ&&ʽ<m.Length;v++,ʽ++){Vector3D l;if(Vector3D.TryParse(m[ʽ],out l)){Node k;if(ʸ
.TryGetValue(l,out k)){if(!p.connectedNodes.Contains(k)){p.connectedNodes.Add(k);}}}}if(ʽ>=m.Length){ʾ++;ʽ=0;}else{break;
}}}void g(){if(Runtime.CurrentInstructionCount/Runtime.MaxInstructionCount>0.75f){Echo(
$"Instruction Limit Reached. Pausing deletion.");}else{if(Ν==EditStatus.UnlinkingNodes){int X=Ψ.Count;int c=Math.Min(Μ+Ȃ,X);for(;Μ<c;Μ++){var Z=Ψ[Μ];if(Z.isSelected){
foreach(var Y in Z.connectedNodes){Y.connectedNodes.RemoveAll(K=>K.position==Z.position);}Echo($"Unlinked node {Μ}/{X}");}}if(Μ
>=X){Ν=EditStatus.DeletingNodes;Μ=0;Ψ=ć.nodes.Where(K=>K.isSelected).ToList();Echo(
$"Phase 1 complete. {Ψ.Count} nodes marked for deletion.");}ɷ(à,"Deleting Nodes");ĕ(à,ʻ.Center,1,$"Unlinking nodes...",Color.Yellow);ɽ(à,new Vector2(ʻ.Center.X,ʻ.Center.Y+40),
new Vector2(ʻ.Width*0.8f,10),Color.Red,Color.White,X,Μ,0);}else if(Ν==EditStatus.DeletingNodes){int X=Ψ.Count;int W=Math.
Min(Ȃ,Ψ.Count-Μ);if(W>0){List<Node>S=new List<Node>();for(int V=0;V<ć.nodes.Count;V++){if(!ć.nodes[V].isSelected){S.Add(ć.
nodes[V]);}}ć.nodes=S;Μ=X;}if(Μ>=X){Ν=EditStatus.Idle;Ӓ();Echo("Node deletion complete.");ʹ=true;ϔ=Ĉ(ć).Length;}}}}void Ô(
MySpriteDrawFrame à,Screen đ){float Đ=-ʻ.Height*0.2f;float ď=ʻ.Height*0.07f;if(Ϻ.Count>0){var Ď=Ϻ[0];if(Ď.processState==Notification.
ProcessState.UnProcessed){Ď.processState=Notification.ProcessState.Processing;Ď.startProcessing=DateTime.UtcNow;}if(Ď.processState==
Notification.ProcessState.Processing){if((DateTime.UtcNow-Ď.startProcessing).TotalSeconds>έ){Ď.processState=Notification.
ProcessState.Processed;}đ.LerpY=Ȥ(đ.LerpY,ď,Ι);}else if(Ď.processState==Notification.ProcessState.Processed){đ.LerpY=Ȥ(đ.LerpY,Đ,Ι);
if(Math.Abs(đ.LerpY-Đ)<2f){Ϻ.RemoveAt(0);}}}else{đ.LerpY=Ȥ(đ.LerpY,Đ,Ι);}Vector2 È=new Vector2(0,đ.LerpY);ǚ(à,
"SquareSimple",È,Color.Black,new Vector2(ʻ.Width,ʻ.Height*0.17f),TextAlignment.LEFT);ǚ(à,"SquareSimple",new Vector2(È.X,È.Y+ʻ.Height*
0.08f),Color.White,new Vector2(ʻ.Width,3.5f*φ),TextAlignment.LEFT);if(Ϻ.Count>0){ĕ(à,new Vector2(ʻ.Width/2,È.Y-ʻ.Height*0.05f
),1f,$"{Ϻ[0].title}",Color.Yellow);ĕ(à,new Vector2(ʻ.Width/2,È.Y),0.8f,$"{Ϻ[0].body}",Color.White);ǚ(à,"Circle",new
Vector2(ʻ.Width*0.9f,È.Y-ʻ.Height*0.01f),Ϻ[0].color,new Vector2(10*φ,10*φ),TextAlignment.CENTER);}}void č(string Č,string ċ,
Color è){var Ċ=new Notification(Č,ċ,è);bool Ē=Ϻ.Any(ĉ=>ĉ.title==Ċ.title&&ĉ.body==Ċ.body);if(!Ē){Ϻ.Add(Ċ);}}string Ĉ(Map ć){
System.Text.StringBuilder ą=new System.Text.StringBuilder(ć.nodes.Count*50);for(int V=0;V<ć.nodes.Count;V++){var Z=ć.nodes[V];
Ć(ą,Z.position);ą.Append('/');for(int v=0;v<Z.connectedNodes.Count;v++){Ć(ą,Z.connectedNodes[v].position);if(v<Z.
connectedNodes.Count-1){ą.Append('|');}}if(V<ć.nodes.Count-1){ą.Append(';');}}return ą.ToString();}void Ć(System.Text.StringBuilder ą,
Vector3D Ą){ą.Append(Ą.X.ToString("F1")).Append(' ').Append(Ą.Y.ToString("F1")).Append(' ').Append(Ą.Z.ToString("F1"));}void ă(
MySpriteDrawFrame à,Vector2D ē,Color è){var â=new MySprite(){Type=SpriteType.TEXTURE,Data="Circle",Position=new Vector2((float)ē.X-5,(
float)ē.Y)+ʻ.Position,Size=new Vector2(10,10),Color=è};à.Add(â);}void ĝ(MySpriteDrawFrame à,Vector2D Ĝ,Vector2D c,Color è,
float ě=1){Vector2D Ě=(Ĝ+c)/2;float ę=(float)Vector2D.Distance(Ĝ,c);float Ę=(float)Math.Atan2(c.Y-Ĝ.Y,c.X-Ĝ.X);var â=new
MySprite(){Type=SpriteType.TEXTURE,Data="SquareSimple",Position=new Vector2((float)Ě.X,(float)Ě.Y)+ʻ.Position,Size=new Vector2(ę
,ě*φ),Color=è,RotationOrScale=Ę,Alignment=TextAlignment.CENTER};à.Add(â);}void Ğ(MySpriteDrawFrame à,Vector2 È,Color è,
Vector2 ò,TextAlignment ė=TextAlignment.CENTER,float Ė=0){var â=new MySprite(){Type=SpriteType.TEXTURE,Data="SquareSimple",
Position=È+ʻ.Position,Size=ò,Color=è,RotationOrScale=Ė,Alignment=ė};à.Add(â);}void ĕ(MySpriteDrawFrame à,Vector2D ē,float Ĕ,
string ï,Color è,TextAlignment ë=TextAlignment.CENTER){var â=new MySprite{Type=SpriteType.TEXT,Data=ï,Position=new Vector2((
float)ē.X,(float)ē.Y)+ʻ.Position,RotationOrScale=Ĕ*φ,Color=è,Alignment=ë,FontId="White"};à.Add(â);}void é(MySpriteDrawFrame à
,Color è,bool ç=false,Vector2 æ=default(Vector2)){Vector2 å=new Vector2(ʻ.Width/2,ʻ.Height/2);if(ç){å=æ;}float ä=25f;
float ã=15f;var â=new MySprite(){Type=SpriteType.TEXTURE,Data="CircleHollow",Position=å+ʻ.Position,Size=new Vector2(ä,ä)*φ,
Color=Color.White,Alignment=TextAlignment.CENTER,RotationOrScale=0};à.Add(â);â=new MySprite(){Type=SpriteType.TEXTURE,Data=
"Circle",Position=å+ʻ.Position,Size=new Vector2(ã,ã)*φ,Color=è,Alignment=TextAlignment.CENTER,RotationOrScale=0};à.Add(â);}void
á(MySpriteDrawFrame à,string ß="",string Þ="",string Ý="",string Ü="",string Û=""){TooltipData[]Ú=new TooltipData[5]{new
TooltipData{SpriteName="SquareHollow",SpriteColor=Color.Black},new TooltipData{SpriteName="SquareHollow",SpriteColor=Color.White},
new TooltipData{SpriteName="SquareHollow",SpriteColor=Color.White},new TooltipData{SpriteName="SquareHollow",SpriteColor=
Color.White},new TooltipData{SpriteName="SquareHollow",SpriteColor=Color.White}};for(int V=0;V<Ú.Length;V++){if(Ђ[V]){Ú[V].
SpriteName="SquareSimple";Ú[V].SpriteColor=Color.White;}}float Ù=40f*φ;float Ø=ʻ.Height-(30f*φ);float Ö=32f*φ;float ê=20f*φ;float
Õ=40f*φ;ó(à,new Vector2(ʻ.Width*0.2f,Ø),new Vector2(ʻ.Width*0.2f,Ù),Color.White,Ú[0].SpriteColor,$"1: {ß}",Ђ[0]);if(Ο!=
DisplayState.AreaMap||ʴ)ó(à,new Vector2((ʻ.Width*0.2f)*2+(ʻ.Width*0.1f),Ø),new Vector2(ʻ.Width*0.2f,Ù),Color.White,Ú[1].SpriteColor,
$"2: {Þ}",Ђ[1]);ó(à,new Vector2((ʻ.Width*0.2f)*3.5f+(ʻ.Width*0.1f),Ø),new Vector2(ʻ.Width*0.2f,Ù),Color.White,Ú[2].SpriteColor,
$"3: {Ý}",Ђ[2]);if(Ο!=DisplayState.AreaMap){if(ˍ.ȅ)Ú[3].SpriteColor=Color.Gray;ó(à,new Vector2(-2*φ,Ö),new Vector2(ʻ.Width*0.5f,Ù
),Ú[3].SpriteColor,Color.White,$"",Ђ[3]);ǚ(à,"AH_BoreSight",new Vector2(ʻ.Width*0.05f,Ö),Ú[3].SpriteColor,new Vector2(Õ,Õ
),TextAlignment.CENTER,(float)Math.PI);ĕ(à,new Vector2(ʻ.Width*0.09f,ê),0.8f,$"4: {Ü}",Ú[3].SpriteColor,TextAlignment.
LEFT);}if(Ο==DisplayState.Destination||Ο==DisplayState.DestinationSettings&&!ˍ.ȅ){Color è=Color.Gray;if(ˊ.Count>0)è=Color.
Cyan;float Ă=ʻ.Width*0.5f;Vector2 Ā=new Vector2(ʻ.Width+(2*φ),Ö);ó(à,Ā,new Vector2(Ă,Ù),è,Color.White,$"",Ђ[4]);ǚ(à,
"AH_BoreSight",new Vector2(ʻ.Width*0.95f,Ö),è,new Vector2(Õ,Õ),TextAlignment.CENTER);float ÿ=Ā.X-(Ă/2f);float þ=ÿ+(15f*φ);ĕ(à,new
Vector2(þ,ê),0.8f,$"5: {Û}",Ú[4].SpriteColor,TextAlignment.LEFT);}if(ʴ&&ˊ.Count>0&&ˏ.Count>0){Vector2 ý=new Vector2(0,ʻ.Height-
(35f*φ));Vector2 ü=new Vector2(0,ʻ.Height-(80f*φ));Vector2 û=new Vector2(0,ʻ.Height*1.1f);Vector2 ú=new Vector2(0,ʻ.
Height*1.02f);Vector2 ù=Vector2.Lerp(û,ý,ϻ);Vector2 ø=Vector2.Lerp(ú,ü,ϻ);TimeSpan ö=TimeSpan.FromSeconds(ʶ.н);string ā;string
õ;if(ö.TotalHours>=1)ā=$"{ö.Hours} Hr {ö.Minutes} Mins";else if(ö.Minutes>0)ā=$"{ö.Minutes} Mins";else ā=$"1 Min";if(ʶ.ъ/
1000<1000&&ʶ.ъ>0)õ=$"{Math.Max(Math.Round(ʶ.ъ/50)*50,50)} m";else if(ʶ.ъ>0)õ=$"{Math.Abs(ʶ.ъ).ToString("F2")} km";else õ=
$"50 m";ǚ(à,"SquareSimple",ù,Color.Black,new Vector2(ʻ.Width,100f*φ),TextAlignment.LEFT);ǚ(à,"SquareSimple",ø,Color.White,new
Vector2(ʻ.Width,3.5f*φ),TextAlignment.LEFT);ĕ(à,new Vector2(ʻ.Width*0.15f,ù.Y-(40*φ)),0.8f,$"ETA:\nDestination:\nMode:",Color.
White,TextAlignment.LEFT);ĕ(à,new Vector2(ʻ.Width*0.40f,ù.Y-(40*φ)),0.8f,
$"{ā}  {(DateTime.Now+ö).ToString("HH:mm")}  |  {õ}\n{ˊ[ˉ].name}\n{Χ.navigationType}",Color.Yellow,TextAlignment.LEFT);Color ô=Color.DarkGray;if(ϧ!=null&&Χ.parkingType==DriveSettings.ParkingType.Connector
&&!Ϫ)ô=Color.Orange;else if(Ϫ)ô=Color.Green;ǚ(à,"CircleHollow",new Vector2(ʻ.Width*0.9f,ù.Y-(2*φ)),Color.White,new Vector2
(30*φ,30*φ));ĕ(à,new Vector2(ʻ.Width*0.9f,ù.Y-(13*φ)),0.8f,"P",ô,TextAlignment.CENTER);}}void ó(MySpriteDrawFrame à,
Vector2 å,Vector2 ò,Color ñ,Color ð,string ï,bool î=false){float í=5f*φ;float Ĳ=3f*φ;float Ƴ=15f*φ;Color ɍ=ñ;Color ɩ=Color.
Black;if(î){ɍ=Color.Black;ɩ=ð;}ǚ(à,"SemiCircle",new Vector2(å.X-ò.X/2+í,å.Y),ñ,new Vector2(ò.Y,ò.Y),TextAlignment.CENTER,-(
float)Math.PI/2);ǚ(à,"SemiCircle",new Vector2(å.X+ò.X/2-í,å.Y),ñ,new Vector2(ò.Y,ò.Y),TextAlignment.CENTER,(float)Math.PI/2);
ǚ(à,"SquareSimple",å,ñ,ò);ǚ(à,"SemiCircle",new Vector2(å.X-ò.X/2+í,å.Y),ɩ,new Vector2(ò.Y-Ĳ,ò.Y-Ĳ),TextAlignment.CENTER,-
(float)Math.PI/2);ǚ(à,"SemiCircle",new Vector2(å.X+ò.X/2-í,å.Y),ɩ,new Vector2(ò.Y-Ĳ,ò.Y-Ĳ),TextAlignment.CENTER,(float)
Math.PI/2);ǚ(à,"SquareSimple",å,ɩ,new Vector2(ò.X-Ĳ,ò.Y-Ĳ));ĕ(à,new Vector2(å.X,å.Y-Ƴ),0.8f,ï,ɍ);}void ɧ(MySpriteDrawFrame à
,Vector2 ɦ,float Ĕ=1f,float Ė=0f){float ɥ=(float)Math.Sin(Ė);float ɤ=(float)Math.Cos(Ė);float ɡ=1f*φ;float ɠ=Ĕ*ɡ;float ɣ=
13f*ɠ;float ɨ=28f*ɠ;à.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="CircleHollow",
Position=new Vector2(-ɥ*-ɣ,ɤ*-ɣ)+ɦ,Size=new Vector2(50f,50f)*ɠ,Color=new Color(255,255,255,255),RotationOrScale=Ė});à.Add(new
MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=new Vector2(-ɥ*-ɨ,ɤ*-ɨ)+ɦ,Size=
new Vector2(15f,30f)*ɠ,Color=new Color(255,255,0,255),RotationOrScale=Ė});à.Add(new MySprite(){Type=SpriteType.TEXTURE,
Alignment=TextAlignment.CENTER,Data="Triangle",Position=new Vector2(0f,0f)+ɦ,Size=new Vector2(25f,30f)*ɠ,Color=new Color(255,255,
0,255),RotationOrScale=3.1416f+Ė});}void ɢ(MySpriteDrawFrame à,Vector2D å,Color è,float Ĕ=1f,float Ė=0f){Vector2 È=(
Vector2)(å+ʻ.Position);float ɡ=1f*φ;float ɠ=Ĕ*ɡ;à.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,
Data="CircleHollow",Position=È,Size=new Vector2(50f,50f)*ɠ,Color=è,RotationOrScale=Ė});à.Add(new MySprite(){Type=SpriteType.
TEXTURE,Alignment=TextAlignment.CENTER,Data="Screen_LoadingBar",Position=È,Size=new Vector2(30f,30f)*ɠ,Color=è,RotationOrScale=
Ė});}void ɟ(MySpriteDrawFrame à,Vector2D ē,string ɪ){float Õ=35f*φ;à.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment
=TextAlignment.CENTER,Data="AH_BoreSight",Position=(Vector2)ē,Size=new Vector2(Õ,Õ),Color=new Color(255,255,255,255),
RotationOrScale=1.5708f});ĕ(à,new Vector2((float)ē.X,(float)ē.Y-Õ),1,ɪ,Color.Yellow);}void ɷ(MySpriteDrawFrame à,string ɵ){float ɴ=50f*
φ;float ɭ=2.5f*φ;float ɳ=16f*φ;Ğ(à,new Vector2(ʻ.Center.X,ɴ),Color.LightGray,new Vector2(ʻ.Width*0.1f,ɭ));Vector2 ɲ=new
Vector2(ʻ.Center.X,ɳ);ĕ(à,ɲ,1,ɵ,Color.White);}void ɱ(MySpriteDrawFrame à,Vector2 È,string ï,bool ɶ,bool ɰ){Color è=Color.White;
if(ɶ){è=Color.Yellow;}float ɯ=15f*φ;float ɮ=35f*φ;float ɭ=2.5f*φ;float Ƴ=2.5f*φ;float Õ=25f*φ;float ɬ=1f*φ;if(ɶ){à.Add(new
MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="AH_BoreSight",Position=new Vector2(ʻ.Width*0.05f,È.Y+ɯ)+
ʻ.Position,Size=new Vector2(Õ,Õ),Color=new Color(255,255,255,255),});}à.Add(new MySprite(){Type=SpriteType.TEXTURE,
Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=new Vector2(ʻ.Center.X,È.Y+ɮ)+ʻ.Position,Size=new Vector2(ʻ.Width*
0.85f,ɭ),Color=new Color(255,255,255,255),});string ɫ="";if(ˊ.Any(K=>K.name==ï)){if(ˊ.FindAll(K=>K.name==ï).Count>1){foreach(
var Ȧ in ˊ.FindAll(K=>K.name==ï)){ɫ=$"{ɫ} / {ˊ.IndexOf(Ȧ)+1}";}}else{ɫ=(ˊ.FindIndex(K=>K.name==ï)+1).ToString();à.Add(new
MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="CircleHollow",Position=new Vector2(ʻ.Width*0.8f,È.Y+ɯ)+ʻ
.Position,Size=new Vector2(Õ,Õ),Color=new Color(255,255,255,255),});ĕ(à,new Vector2(ʻ.Width*0.8f,È.Y+ɬ),0.8f,ɫ,è,
TextAlignment.CENTER);}}string ɗ=ï;if(ɰ)ɗ=$"[{ɫ}] {ï}";ĕ(à,È,1,ï,è,TextAlignment.LEFT);ĕ(à,new Vector2(ʻ.Width*0.925f,È.Y+Ƴ),0.8f,(
Vector3D.Distance(ˋ.Find(K=>K.name==ï).position,å)/1000).ToString("F2"),è,TextAlignment.RIGHT);}void Ɋ(MySpriteDrawFrame à,
Vector2 È,string ï,float Ė,Color è){Vector2 ɉ=new Vector2(ʻ.Width*0.9f,È.Y*1.8f);float ä=30f*φ;float ã=22f*φ;à.Add(new MySprite
(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="CircleHollow",Position=ɉ+ʻ.Position,Size=new Vector2(ä,ä)
,Color=Color.White,});à.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data=
"Screen_LoadingBar",Position=ɉ+ʻ.Position,RotationOrScale=Ė,Size=new Vector2(ã,ã),Color=è,});ĕ(à,È,0.8f,ï,Color.White);}void Ɉ(
MySpriteDrawFrame à,Vector2 È,Color è){double ɇ=0;string M=ʵ.ToString();Vector2 Ɇ=new Vector2(ʻ.Width/2f,ʻ.Height/2f);if(Ќ.Count>0){var Ǡ
=ƻ(Ќ[0].position,å,(float)ϲ,true,Ɇ,ƹ,Ƹ,Ʒ,true);Ǡ.Y=Ǡ.Y+(20f*φ);Color Ʌ=Color.Green;Color Ʉ=Color.White;if(Њ.IndexOf(Ћ)>0)
{Ʌ=Color.White;Ʉ=Color.Red;}float Ƀ=35f*φ;float ɋ=32f*φ;float ɂ=20f*φ;float Ɍ=42f*φ;float ɞ=10f*φ;float ɜ=30f*φ;float ɛ=
5f*φ;float ɚ=25f*φ;ǚ(à,"SemiCircle",new Vector2((float)Ǡ.X+ɛ,(float)Ǡ.Y-ɞ),Color.White,new Vector2(Ƀ,Ƀ));ǚ(à,"SemiCircle",
new Vector2((float)Ǡ.X+ɛ,(float)Ǡ.Y+ɜ),Color.White,new Vector2(Ƀ,Ƀ),TextAlignment.CENTER,(float)Math.PI);ǚ(à,"SquareSimple"
,new Vector2((float)Ǡ.X+ɛ,(float)Ǡ.Y+ɞ),Color.White,new Vector2(Ƀ,Ɍ));ǚ(à,"SemiCircle",new Vector2((float)Ǡ.X+ɛ,(float)Ǡ.
Y-ɞ),Color.Black,new Vector2(ɋ,ɋ));ǚ(à,"SemiCircle",new Vector2((float)Ǡ.X+ɛ,(float)Ǡ.Y+ɜ),Color.Black,new Vector2(ɋ,ɋ),
TextAlignment.CENTER,(float)Math.PI);ǚ(à,"SquareSimple",new Vector2((float)Ǡ.X+ɛ,(float)Ǡ.Y+ɞ),Color.Black,new Vector2(ɋ,Ɍ));ǚ(à,
"Circle",new Vector2((float)Ǡ.X+ɛ,(float)Ǡ.Y-ɛ),Ʉ,new Vector2(ɂ,ɂ));ǚ(à,"Circle",new Vector2((float)Ǡ.X+ɛ,(float)Ǡ.Y+ɚ),Ʌ,new
Vector2(ɂ,ɂ));}if(ʵ==AutoDriveState.Parking_Creep1||ʵ==AutoDriveState.Parking_Creep2||ʵ==AutoDriveState.Parking_Correction||ʵ==
AutoDriveState.Parking_Reverse1||ʵ==AutoDriveState.Parking_Reverse2||ʵ==AutoDriveState.Parking_Waiting){Node Ʀ=Ǹ(ć.nodes,Ϭ);Vector3D ů
=Ů(Ʀ,Ϭ,0);Vector3D ƴ=Vector3D.Normalize(ϩ);Vector3D ə=Ϭ;Vector3D ǫ=Ϭ+ƴ*10;Vector3D ǈ=Vector3D.Up;Vector3D ȟ=Vector3D.
Normalize(Vector3D.Cross(ƴ,ǈ));double ɝ=5;Vector3D ɘ=ə-ȟ*ɝ;Vector3D ɖ=ə+ȟ*ɝ;Vector3D ɕ=ǫ-ȟ*ɝ;Vector3D ɔ=ǫ+ȟ*ɝ;Vector2D ɓ=ƻ(ɘ,å,(
float)ϲ,true,Ɇ,ƹ,Ƹ,Ʒ,true);Vector2D ɒ=ƻ(ɖ,å,(float)ϲ,true,Ɇ,ƹ,Ƹ,Ʒ,true);Vector2D ɑ=ƻ(ɕ,å,(float)ϲ,true,Ɇ,ƹ,Ƹ,Ʒ,true);Vector2D
ɐ=ƻ(ɔ,å,(float)ϲ,true,Ɇ,ƹ,Ƹ,Ʒ,true);Color ɏ=Color.Blue;float ě=0.5f*(float)Ϸ;ĝ(à,ɓ,ɒ,ɏ,ě);ĝ(à,ɑ,ɐ,ɏ,ě);ĝ(à,ɓ,ɑ,ɏ,ě);ĝ(à,ɒ
,ɐ,ɏ,ě);Vector2D Ɏ=(ɓ+ɒ+ɑ+ɐ)/4;ĕ(à,Ɏ,0.1f*(float)Ϸ,"P",Color.White);ĝ(à,ƻ(å,å,(float)ϲ,true,Ɇ,ƹ,Ƹ,Ʒ,true),ƻ(ϭ,å,(float)ϲ,
true,Ɇ,ƹ,Ƹ,Ʒ,true),Color.Cyan,(float)(В*Ϸ));if(ʵ==AutoDriveState.Parking_Waiting){ɽ(à,new Vector2(ʻ.Width*0.5f,ʻ.Height*
0.76f),new Vector2(ʻ.Width*0.7f,ʻ.Height*0.02f),Color.Yellow,Color.White,1,(float)ѵ.Ѵ((double)(DateTime.UtcNow-ϡ).
TotalSeconds/Ϧ.Find(K=>K.timerType==Timer.TimerType.DepartureTimer).myTimer.TriggerDelay,0,1));TimeSpan ɸ=DateTime.UtcNow-ϡ;double ʙ
=Ϧ.Find(K=>K.timerType==Timer.TimerType.DepartureTimer).myTimer.TriggerDelay-ɸ.TotalSeconds;TimeSpan ʥ=TimeSpan.
FromSeconds(ʙ);ĕ(à,new Vector2(ʻ.Width/2,(ʻ.Height*0.77f)),0.7f,$"Time remaining: {ʥ.ToString(@"mm\:ss")}",Color.White,
TextAlignment.CENTER);}}else if(ˏ.Count>0&&ˊ.Count>0&&ʵ!=AutoDriveState.RechargeWait&&ʵ!=AutoDriveState.CargoWait&&ʵ!=AutoDriveState.
ExternalWait){ĝ(à,ƻ(å,å,(float)ϲ,true,Ɇ,ƹ,Ƹ,Ʒ,true),ƻ(ϭ,å,(float)ϲ,true,Ɇ,ƹ,Ƹ,Ʒ,true),Color.Yellow,(float)(В*Ϸ));}if(M.Contains(
"Parking"))M="Parking";if(M.Contains("Recharge"))M="Recharging";if(M.Contains("Cargo"))M="(Un)loading";if(M.Contains("External"))
M="Standby";ˠ+=0.25f;float ɳ=16f*φ;Ɋ(à,new Vector2(ʻ.Width*0.15f,ɳ),"AutoDrive",ˠ,è);ĕ(à,new Vector2(ʻ.Width*0.78f,ɳ),
0.8f,M,Color.Yellow);if(с(Φ)>5&&ʵ==AutoDriveState.Driving){Ϸ=Ȥ((float)Ϸ,(float)Math.Abs(ѵ.Ѵ(MAX_SPEED/с(Φ),ϳ,ϴ)),Ι);}else if
(с(Φ)<5&&ʵ==AutoDriveState.Driving){Ϸ=Ȥ((float)Ϸ,(float)ϴ,Ι);}if(ʵ==AutoDriveState.Parking_Creep1||ʵ==AutoDriveState.
Parking_Creep2||ʵ==AutoDriveState.Parking_Correction||ʵ==AutoDriveState.Parking_Reverse1||ʵ==AutoDriveState.Parking_Reverse2||ʵ==
AutoDriveState.Parking_Waiting){if(Ϫ){float ʤ=(float)(250f/(Vector3D.Distance(å,Ϭ)));Ϸ=Ȥ((float)Ϸ,ʤ*0.85f,Ι);ϲ=Ȥ(ϲ,0,Ι);}}Vector2 ò=
new Vector2(25*φ,7*φ);Vector2 ʣ=new Vector2(ʻ.Width/2-60,ʻ.Height*0.15f);Vector2 ʢ=new Vector2(ʻ.Width/2+60,ʻ.Height*0.15f)
;Color ʡ=Color.Green;bool ʠ=(DateTime.UtcNow.TimeOfDay.TotalSeconds%0.9)<0.45;if(Љ.TrueForAll(K=>K.Enabled)&&Љ.Count>0&&ʠ
){ǚ(à,"SquareSimple",ʣ,ʡ,ò);ǚ(à,"AH_BoreSight",new Vector2(ʣ.X-ò.X/2,ʣ.Y),ʡ,new Vector2(ò.X,ò.X),TextAlignment.CENTER,(
float)Math.PI/180*180);}if(Ј.TrueForAll(K=>K.Enabled)&&Ј.Count>0&&ʠ){ǚ(à,"SquareSimple",ʢ,ʡ,ò);ǚ(à,"AH_BoreSight",new Vector2
(ʢ.X+ò.X/2,ʢ.Y),ʡ,new Vector2(ò.X,ò.X));}ʦ(à);}void ʦ(MySpriteDrawFrame à){Vector2 ʟ=new Vector2(ʻ.Width*0.5f,ʻ.Height*
0.76f);ʟ.X-=70;if(ϧ!=null){if(ϧ.IsConnected){float ʞ=float.Parse(J());if(Б>=0){if(ʞ-Б>0.05f)Ў=1;else if(Б-ʞ>0.05f)Ў=-1;}Б=ʞ;ǚ
(à,"Circle",ʟ,Color.Black,new Vector2(60,60),TextAlignment.CENTER);ǚ(à,"IconEnergy",ʟ,Color.White,new Vector2(50,50),
TextAlignment.CENTER);ǚ(à,"CircleHollow",ʟ,Color.White,new Vector2(60,60),TextAlignment.CENTER);ĕ(à,new Vector2(ʟ.X,ʟ.Y+30f),0.8f,
$"{ʞ:F0}%",Color.White,TextAlignment.CENTER);if(Ў!=0){float ʜ=Ў==1?0f:(float)Math.PI;Color ʛ=Ў==1?Color.Green:Color.Red;ǚ(à,
"Triangle",new Vector2(ʟ.X+45f,ʟ.Y+15),ʛ,new Vector2(15,15),TextAlignment.CENTER,ʜ);}ʟ.X+=140;float ʝ=float.Parse(E());if(Џ>=0){if
(ʝ-Џ>0.05f)Ѝ=1;else if(Џ-ʝ>0.05f)Ѝ=-1;}Џ=ʝ;ǚ(à,"Circle",ʟ,Color.Black,new Vector2(60,60),TextAlignment.CENTER);ǚ(à,
"Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds",ʟ,Color.White,new Vector2(50,50),TextAlignment.CENTER);ǚ(à,"CircleHollow",ʟ,Color.White,new Vector2(60,60),
TextAlignment.CENTER);ĕ(à,new Vector2(ʟ.X,ʟ.Y+30f),0.8f,$"{ʝ:F0}%",Color.White,TextAlignment.CENTER);if(Ѝ!=0){float ʜ=Ѝ==1?0f:(float)
Math.PI;Color ʛ=Ѝ==1?Color.Green:Color.Red;ǚ(à,"Triangle",new Vector2(ʟ.X+45f,ʟ.Y+15),ʛ,new Vector2(15,15),TextAlignment.
CENTER,ʜ);}}else{Б=-1f;Џ=-1f;Ў=0;Ѝ=0;}}}void ʚ(MySpriteDrawFrame à,Vector2 È,Color è){Ϸ=Ȥ((float)Ϸ,(float)ϴ,Ι);if(ϧ!=null){if(
ϧ.IsConnected){Vector3D ʧ=ϧ.GetPosition();float ʫ=Math.Min(ʻ.Width/2,ʻ.Height);float ʤ=(float)(ʫ/(Vector3D.Distance(å,ʧ))
);Ϸ=Ȥ((float)Ϸ,ʤ*0.85f,Ι);ϲ=Ȥ(ϲ,0,Ι);Node Ʀ=Ǹ(ć.nodes,ʧ);Vector3D ů=Ů(Ʀ,ʧ,0);Vector3D ƴ=Vector3D.Normalize(Φ.WorldMatrix.
Forward);Vector3D ə=ʧ;Vector3D ǫ=ʧ+ƴ*10;Vector3D ǈ=Vector3D.Up;Vector3D ȟ=Vector3D.Normalize(Vector3D.Cross(ƴ,ǈ));double ɝ=5;
Vector3D ɘ=ə-ȟ*ɝ;Vector3D ɖ=ə+ȟ*ɝ;Vector3D ɕ=ǫ-ȟ*ɝ;Vector3D ɔ=ǫ+ȟ*ɝ;Vector2D ɓ=ƻ(ɘ,å,ϲ);Vector2D ɒ=ƻ(ɖ,å,ϲ);Vector2D ɑ=ƻ(ɕ,å,ϲ);
Vector2D ɐ=ƻ(ɔ,å,ϲ);float ě=0.5f*(float)Ϸ;ĝ(à,ɓ,ɒ,è,ě);ĝ(à,ɑ,ɐ,è,ě);ĝ(à,ɓ,ɑ,è,ě);ĝ(à,ɒ,ɐ,è,ě);Vector2D Ɏ=(ɓ+ɒ+ɑ+ɐ)/4;ĕ(à,Ɏ,0.1f*
(float)Ϸ,"P",Color.White);}}ʦ(à);}void ʪ<ʩ>(MySpriteDrawFrame à,Vector2 È,float ʨ,float ʘ,int ʈ,List<int>ʏ=null){var ʆ=
Enum.GetValues(typeof(ʩ)).Cast<ʩ>().ToList();if(ʆ.Count==0)return;bool ʅ=ʏ!=null&&ʏ.Count>=ʆ.Count-1;float ʄ=10f*φ;float ʃ=
30f*φ;float ʂ=55f*φ;Vector2 ò=new Vector2((ʨ-((ʆ.Count-1)*ʘ))/ʆ.Count,ʄ);Vector2 ʁ=new Vector2(È.X,È.Y);for(int V=0;V<ʆ.
Count;V++){var ʀ=ʆ[V];bool ɿ=ʏ!=null&&ʏ.Contains(V);Color ɾ=Color.White;if(ʈ==V){if(ʅ)ɾ=new Color(130,130,125);else if(ɿ)ɾ=
Color.Red;else ɾ=Color.Yellow;ĕ(à,new Vector2(È.X+(ʨ/2),È.Y-ʃ),0.7f,ʀ.ToString(),ɾ);}else{if(ɿ)ɾ=new Color(130,130,125);else
ɾ=Color.White;}Ğ(à,ʁ,ɾ,ò,TextAlignment.LEFT);ʁ.X+=ò.X+ʘ;}ĕ(à,new Vector2(È.X+(ʨ/2),È.Y-ʂ),0.8f,typeof(ʩ).Name,Color.White
);}void ɽ(MySpriteDrawFrame à,Vector2 È,Vector2 ò,Color ð,Color ɼ,float ɻ,float ɺ,float Ė=0){Vector2 ɹ=new Vector2((ɺ/ɻ)*
ò.X,ò.Y);Ğ(à,È,ɼ,ò,TextAlignment.CENTER,Ė);Ğ(à,new Vector2(È.X-(ò.X/2),È.Y),ð,ɹ,TextAlignment.LEFT,Ė);}bool ʇ(Vector2D ț,
Vector2D Ț,float ʗ,float ʐ){if(ț.X<0&&Ț.X<0)return true;if(ț.X>ʗ&&Ț.X>ʗ)return true;if(ț.Y<0&&Ț.Y<0)return true;if(ț.Y>ʐ&&Ț.Y>ʐ)
return true;return false;}void ʖ(MySpriteDrawFrame à,Map ć,Vector2 ʕ,bool ƶ=true,double?ʔ=null,float ʓ=0){try{double ʒ=ʔ.
HasValue?ʔ.Value:400;double ʑ=ʒ*ʒ;float ʗ=ʻ.Width;float ʐ=ʻ.Height;Color ʎ=(ʵ==AutoDriveState.Driving||ʵ==AutoDriveState.
Reversing)?Color.DeepSkyBlue:Color.Green;foreach(var Z in ć.nodes){if(Vector3D.DistanceSquared(Z.position,å)>ʑ)continue;Vector3D
ʍ=ȩ(Z.position,å,ʓ);Vector2D ǫ=ƻ(ʍ,å,(float)ϲ,true,ʕ,ƹ,Ƹ,Ʒ,ƶ);if(double.IsNaN(ǫ.X))continue;foreach(var Y in Z.
connectedNodes){if(Z.GetHashCode()>Y.GetHashCode())continue;Vector3D ʌ=ȩ(Y.position,å,ʓ);Vector2D ǰ=ƻ(ʌ,å,(float)ϲ,true,ʕ,ƹ,Ƹ,Ʒ,ƶ);if(
double.IsNaN(ǰ.X))continue;if(ʇ(ǫ,ǰ,ʗ,ʐ))continue;ĝ(à,ǫ,ǰ,Color.White,2);}}if(ˏ.Count>1){int ʋ=ʔ.HasValue?ˏ.Count:20;int ʊ=
Math.Min(ˏ.Count-1,ʋ);for(int V=1;V<ʊ;V++){Vector3D ʉ=ȩ(ˏ[V].position,å,ʓ);Vector3D Ɂ=ȩ(ˏ[V+1].position,å,ʓ);Vector2D ǫ=ƻ(ʉ,
å,(float)ϲ,true,ʕ,ƹ,Ƹ,Ʒ,ƶ);Vector2D ǰ=ƻ(Ɂ,å,(float)ϲ,true,ʕ,ƹ,Ƹ,Ʒ,ƶ);if(!double.IsNaN(ǫ.X)&&!double.IsNaN(ǰ.X)&&!ʇ(ǫ,ǰ,ʗ,
ʐ)){ĝ(à,ǫ,ǰ,ʎ,5);}}Vector3D ǩ=Ż(ˏ[0],ˏ[1],å,0);Vector3D Ǩ=ȩ(ǩ,å,ʓ);Vector3D ǧ=ȩ(ˏ[1].position,å,ʓ);Vector3D Ǧ=ȩ(å,å,ʓ);
Vector2D ǥ=ƻ(Ǩ,å,(float)ϲ,true,ʕ,ƹ,Ƹ,Ʒ,ƶ);Vector2D Ǥ=ƻ(ǧ,å,(float)ϲ,true,ʕ,ƹ,Ƹ,Ʒ,ƶ);Vector2D ǣ=ƻ(Ǧ,å,(float)ϲ,true,ʕ,ƹ,Ƹ,Ʒ,ƶ);if
(!double.IsNaN(ǥ.X)&&!double.IsNaN(Ǥ.X)){float Ǣ=5;ĝ(à,ǥ,Ǥ,ʎ,Ǣ);if(Vector2D.Distance(ǥ,ǣ)>2.0){ĝ(à,ǥ,ǣ,ʎ,Ǣ);}}}}catch(
Exception){}}void ǡ(MySpriteDrawFrame à,Vector2 Ǡ,Color ǟ,Color Ǟ){Ğ(à,new Vector2(ʻ.Width*Ǡ.X,Ǡ.Y),ǟ,new Vector2(40,10));ǚ(à,
"Circle",new Vector2(ʻ.Width*(Ǡ.X-0.02f),Ǡ.Y+3),Ǟ,new Vector2(12,12),TextAlignment.CENTER,0);ǚ(à,"Circle",new Vector2(ʻ.Width*(Ǡ
.X+0.02f),Ǡ.Y+3),Ǟ,new Vector2(12,12),TextAlignment.CENTER,0);ǚ(à,"Circle",new Vector2(ʻ.Width*(Ǡ.X-0.02f),Ǡ.Y+3),ǟ,new
Vector2(9,9),TextAlignment.CENTER,0);ǚ(à,"Circle",new Vector2(ʻ.Width*(Ǡ.X+0.02f),Ǡ.Y+3),ǟ,new Vector2(9,9),TextAlignment.
CENTER,0);ǚ(à,"Circle",new Vector2(ʻ.Width*Ǡ.X,Ǡ.Y-5),ǟ,new Vector2(20,12),TextAlignment.CENTER,0);}void ǝ(MySpriteDrawFrame à
){Color è=Color.Green;float ǜ=0.79f;float Ǜ=0.70f;if(ʮ.HasValue){Vector2D È;if(ˏ.Count>1)È=ƻ(Ż(ˏ[0],ˏ[1],ʮ.Value.Position
,0),å,ϲ);else È=ƻ(ʮ.Value.Position,å,ϲ);è=Color.DeepSkyBlue;ǡ(à,new Vector2(0.7f,ʻ.Height*0.93f),è,Color.Black);}ǡ(à,new
Vector2(0.85f,ʻ.Height*0.93f),è,Color.Black);ǚ(à,"Triangle",new Vector2(ʻ.Width*ǜ,ʻ.Height*Ǜ),è,new Vector2(10,10),
TextAlignment.CENTER,(float)(Math.PI/2));ǚ(à,"Triangle",new Vector2(ʻ.Width*(ǜ-0.01f),ʻ.Height*Ǜ),è,new Vector2(13,13),TextAlignment.
CENTER,(float)(Math.PI/2));ǚ(à,"Triangle",new Vector2(ʻ.Width*(ǜ-0.01f),ʻ.Height*Ǜ),è,new Vector2(12,12),TextAlignment.CENTER,
(float)(Math.PI/2));ǚ(à,"Triangle",new Vector2(ʻ.Width*(ǜ-0.02f),ʻ.Height*Ǜ),è,new Vector2(15f,15),TextAlignment.CENTER,(
float)(Math.PI/2));ǚ(à,"Triangle",new Vector2(ʻ.Width*(ǜ-0.01f),ʻ.Height*Ǜ),è,new Vector2(14,14),TextAlignment.CENTER,(float)
(Math.PI/2));}void ǚ(MySpriteDrawFrame à,string Ǫ,Vector2 È,Color è,Vector2 ò,TextAlignment ė=TextAlignment.CENTER,float
Ė=0){var â=new MySprite(){Type=SpriteType.TEXTURE,Data=Ǫ,Position=È+ʻ.Position,Size=ò,Color=è,RotationOrScale=Ė,Alignment
=ė};à.Add(â);}Node Ǹ(List<Node>q,Vector3D Ƿ){Node Ʀ=new Node(Vector3D.Zero);double Ƕ=double.MaxValue;foreach(Node Z in q)
{double ŕ=Vector3D.Distance(Z.position,Ƿ);if(ŕ<Ƕ){Ƕ=ŕ;Ʀ=Z;}}return Ʀ;}Vector3D ǹ(Vector3D Ɔ,Vector3D Ǵ,double ǳ){Vector3D
ǲ=Ǵ-Ɔ;double ī=ǲ.Length();if(ī>ǳ){ǲ=Vector3D.Normalize(ǲ)*ǳ;}return Ɔ+ǲ;}Vector3D Ǳ(Vector3D ǫ,Vector3D ǰ,double ǯ){
Vector3D Ǯ=Vector3D.Normalize(ǰ-ǫ);Vector3D ǭ=Φ.WorldMatrix.Right;Vector3D Ǭ=Vector3D.Cross(Ǯ,ǭ);Ǭ=Vector3D.Normalize(Ǭ);
Vector3D ǵ=Ǭ*ǯ;return ǵ;}Vector3D Ǚ(Vector3D ǒ,Vector3D ǅ,Vector3D Ǆ,float ǃ){Vector3D ǂ=ǅ-ǒ;Vector3D ǁ=Vector3D.Normalize(ǂ);
Vector3D ǀ=Φ.WorldMatrix.Up;Vector3D ƿ=Vector3D.Cross(ǁ,ǀ);Vector3D ƾ=Vector3D.Normalize(ƿ);Vector3D ƽ=ƾ*ǃ;Vector3D Ƽ=Ǆ+ƽ;return
Ƽ;}Vector2D ƻ(Vector3D ē,Vector3D ƺ,float Ę,bool ç=false,Vector2 æ=default(Vector2),double ƹ=200.0,double Ƹ=-200,double Ʒ
=1000.0,bool ƶ=true){Vector2 Ƶ=new Vector2(ʻ.Width/2,ʻ.Height/2);if(ç){Ƶ=æ;}Vector3D ǆ=Φ.WorldMatrix.Right;Vector3D ƴ=Φ.
WorldMatrix.Forward;Vector3D ǈ=Φ.WorldMatrix.Up;double ǘ=Math.PI*Ę/180.0;Vector3D Ǘ=Vector3D.Normalize(ƴ*Math.Cos(ǘ)+ǈ*Math.Sin(ǘ))
;Vector3D ǖ=Φ.WorldMatrix.Up;Vector3D Ǖ=ē-ƺ;double ŕ=Vector3D.Dot(Ǖ,ǖ);Vector3D ǔ=ē-ŕ*ǖ;if(ƶ){double Ǉ=Vector3D.Dot(ǔ-ƺ,ƴ
);if(Ǉ<Ƹ||Ǉ>Ʒ){return new Vector2D(double.NaN,double.NaN);}double Ǔ=ƹ/(ƹ+Ǉ);double K=Vector3D.Dot(ǔ-ƺ,ǆ)*Ǔ;double Ǌ=
Vector3D.Dot(ǔ-ƺ,Ǘ)*Ǔ;K*=Ϸ;Ǌ*=Ϸ;Vector2D Ǒ=new Vector2D(Ƶ.X+K,Ƶ.Y-Ǌ);return Ǒ;}else{double K=Vector3D.Dot(ǔ-ƺ,ǆ);double Ǌ=
Vector3D.Dot(ǔ-ƺ,Ǘ);K*=Ϸ;Ǌ*=Ϸ;Vector2D Ǒ=new Vector2D(Ƶ.X+K,Ƶ.Y-Ǌ);return Ǒ;}}void ǐ(){IMyCubeGrid Ǐ=Me.CubeGrid;IMySlimBlock ǎ=
null;IMySlimBlock Ǎ=null;double ǌ=double.MinValue;double ǋ=double.MinValue;int V=0;for(int K=Ǐ.Min.X;K<=Ǐ.Max.X;K++){for(int
Ǌ=Ǐ.Min.Y;Ǌ<=Ǐ.Max.Y;Ǌ++){for(int Ǉ=Ǐ.Min.Z;Ǉ<=Ǐ.Max.Z;Ǉ++){Vector3I ǉ=new Vector3I(K,Ǌ,Ǉ);if(Ǐ.CubeExists(ǉ)){
IMySlimBlock Ǻ=Ǐ.GetCubeBlock(ǉ);if(Ǻ!=null){Vector3D ȱ=Ǻ.CubeGrid.GridIntegerToWorld(Ǻ.Position);Vector3D ȯ=Vector3D.
TransformNormal(ȱ-Me.CubeGrid.WorldMatrix.Translation,MatrixD.Transpose(Me.CubeGrid.WorldMatrix));double Ȯ=Vector3D.Dot(ȯ,Φ.WorldMatrix
.Forward);double ȭ=Vector3D.Dot(ȯ,Φ.WorldMatrix.Backward);if(Ȯ>ǌ){ǌ=Ȯ;ǎ=Ǻ;}if(ȭ>ǋ){ǋ=ȭ;Ǎ=Ǻ;}V++;}}}}}if(Math.Abs(є(ǎ.
CubeGrid.GridIntegerToWorld(ǎ.Position)))>90){ˤ=Ǎ;ˣ=ǎ;}else{ˤ=ǎ;ˣ=Ǎ;}Echo("Angle: "+є(ǎ.CubeGrid.GridIntegerToWorld(ǎ.Position))
.ToString("F2"));Echo(ˤ.Position.ToString());}bool Ȭ(IMyShipConnector ȫ){string Ȫ=ȫ.BlockDefinition.SubtypeId;return Ȫ==
"ConnectorSmall"||Ȫ=="SmallBlockInsetConnector"||Ȫ=="SmallBlockInsetConnectorMedium";}Vector3D ȩ(Vector3D ē,Vector3D Ě,double Ȩ){if(Ȩ==0
)return ē;Vector3D Ȱ=ē-Ě;MatrixD ȧ=MatrixD.CreateFromAxisAngle(Φ.WorldMatrix.Up,Ȩ);Vector3D ȥ=Vector3D.TransformNormal(Ȱ,
ȧ);return Ě+ȥ;}float Ȥ(float Ĝ,float c,float ŵ){return Ĝ+(c-Ĝ)*ŵ;}class ȣ<Ȣ>{public Ȣ ȡ{get;set;}public float Ƞ{get;set;}
public ȣ(Ȣ Ȧ,float Ȳ){ȡ=Ȧ;Ƞ=Ȳ;}}class ȹ<Ȣ>{List<ȣ<Ȣ>>ɀ=new List<ȣ<Ȣ>>();public int ȿ=>ɀ.Count;public void Ⱦ(Ȣ Ȧ,float Ȳ){ɀ.Add
(new ȣ<Ȣ>(Ȧ,Ȳ));ȸ(ɀ.Count-1);}public Ȣ Ƚ(){var ȼ=ɀ[0];var ł=ɀ[ɀ.Count-1];ɀ.RemoveAt(ɀ.Count-1);if(ɀ.Count>0){ɀ[0]=ł;ȶ(0);
}return ȼ.ȡ;}public bool Ȼ(Ȣ Ȧ){for(int V=0;V<ɀ.Count;V++)if(ɀ[V].ȡ.Equals(Ȧ))return true;return false;}public void Ⱥ(Ȣ Ȧ
){for(int V=0;V<ɀ.Count;V++){if(ɀ[V].ȡ.Equals(Ȧ)){int ł=ɀ.Count-1;ɀ[V]=ɀ[ł];ɀ.RemoveAt(ł);if(V<ɀ.Count){ȸ(V);ȶ(V);}break;
}}}void ȸ(int ȵ){while(ȵ>0){int ȷ=(ȵ-1)>>1;if(ɀ[ȵ].Ƞ<ɀ[ȷ].Ƞ){var ȕ=ɀ[ȷ];ɀ[ȷ]=ɀ[ȵ];ɀ[ȵ]=ȕ;ȵ=ȷ;}else break;}}void ȶ(int ȵ){
int ȴ=ɀ.Count;while(true){int ȳ=ȵ*2+1;int ȟ=ȳ+1;int ȍ=ȵ;if(ȳ<ȴ&&ɀ[ȳ].Ƞ<ɀ[ȍ].Ƞ)ȍ=ȳ;if(ȟ<ȴ&&ɀ[ȟ].Ƞ<ɀ[ȍ].Ƞ)ȍ=ȟ;if(ȍ==ȵ)break;
var ȕ=ɀ[ȵ];ɀ[ȵ]=ɀ[ȍ];ɀ[ȍ]=ȕ;ȵ=ȍ;}}}class ȋ{public ȹ<Node>Ȋ;private Dictionary<Node,Node>ȉ;private Dictionary<Node,float>Ȉ;
private Dictionary<Node,float>ȇ;private Node Ƥ;private Node Ȇ;public bool ȅ;private Action<List<Node>>Ȅ;public List<string>ȃ=
new List<string>();public int Ȃ=10;private Vector3D Ǽ;private bool ȁ;private float Ȁ;public event Action<Node,Node>ǿ;public
ȋ(){Ȋ=new ȹ<Node>();ȉ=new Dictionary<Node,Node>();Ȉ=new Dictionary<Node,float>();ȇ=new Dictionary<Node,float>();ȅ=false;ȁ
=false;}public void Ǿ(Node Ĝ,Node ǽ,Vector3D Ǽ,Action<List<Node>>ǻ,float Ȍ){this.Ƥ=Ĝ;this.Ȇ=ǽ;this.Ǽ=Ǽ;this.Ȅ=ǻ;this.ȁ=
false;Ȁ=Ȍ;Ȋ.Ⱦ(Ĝ,0);ȉ.Clear();Ȉ.Clear();ȇ.Clear();foreach(var Z in Ȓ(Ĝ)){Ȉ[Z]=float.MaxValue;ȇ[Z]=float.MaxValue;}Ȉ[Ĝ]=0;ȇ[Ĝ]=
Ȝ(Ĝ.position,ǽ.position);ȅ=true;ȃ.Add("Pathfinding started.");}public void Ȏ(){if(!ȅ)return;if(Ȋ.ȿ>0){for(int V=0;V<Ȃ&&Ȋ.
ȿ>0;V++){var Ȕ=Ȋ.Ƚ();ȃ.Add($"Checking node at position: {Ȕ.position}");if(Ȕ==Ȇ){ȅ=false;Ȅ(Ȗ(ȉ,Ȕ));return;}foreach(var k
in Ȕ.connectedNodes){float ȝ=Ȉ[Ȕ]+(float)Vector3D.Distance(Ȕ.position,k.position);if(!ȁ){ȝ+=ș(Ȕ.position,k.position);}if(ȝ
<Ȉ[k]){ȉ[k]=Ȕ;Ȉ[k]=ȝ;ȇ[k]=Ȉ[k]+Ȝ(k.position,Ȇ.position);if(Ȋ.Ȼ(k)){Ȋ.Ⱥ(k);}Ȋ.Ⱦ(k,ȇ[k]);ȃ.Add(
$"Enqueued neighbor at position: {k.position}");}}ȁ=true;}}else{ȅ=false;Ȅ(null);ȃ.Add("No path found.");}}private float Ȝ(Vector3D ț,Vector3D Ț){return(float)Vector3D
.Distance(ț,Ț);}private float ș(Vector3D Ș,Vector3D ȗ){Vector3D ŷ=ȗ-Ș;ŷ.Normalize();double Ȟ=Vector3D.Dot(Ǽ,ŷ);return(
float)((1-Ȟ)*Ȁ);}private List<Node>Ȗ(Dictionary<Node,Node>ȉ,Node Ȕ){var ȓ=new List<Node>{Ȕ};while(ȉ.ContainsKey(Ȕ)){Ȕ=ȉ[Ȕ];ȓ.
Add(Ȕ);}ȓ.Reverse();return ȓ;}private IEnumerable<Node>Ȓ(Node Ĝ){var ȑ=new HashSet<Node>();var Ȑ=new Queue<Node>();Ȑ.
Enqueue(Ĝ);while(Ȑ.Count>0){var Z=Ȑ.Dequeue();if(!ȑ.Contains(Z)){ȑ.Add(Z);foreach(var k in Z.connectedNodes){if(!ȑ.Contains(k))
{Ȑ.Enqueue(k);}}}}return ȑ;}}void ȏ(List<Node>ŗ){if(ŗ!=null){ˏ.Clear();ː.Clear();Ќ.Clear();foreach(var Z in ŗ){ˏ.Add(Z);ː
.Add(Z);if(Z.connectedNodes.Count>2){Ќ.Add(Z);}}ʯ=CollisionStatus.Idle;}else{č("Navigation","Unable to calculate path",
Color.Red);ʴ=false;ʵ=AutoDriveState.Stopping;}}