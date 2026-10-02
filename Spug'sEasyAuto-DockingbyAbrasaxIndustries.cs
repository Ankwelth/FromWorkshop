// 
// 
// /////////////////////////////////////////////////////
//     Auto Docking 2.0
//     
//     Author:  Spug
//     Please leave credit if the ship is
//     used on the workshop.
//     (Extra thanks to Whip for his PID controller)
// 
//     Spug's Auto Docking by Abrasax Industries
//     (C) irreality.net 2024
// /////////////////////////////////////////////////////
// 
public class AntennaHandler{private const string _responseTag="Spug's position update response";private const string
_outgoingRequestTag="Spug's position update request";private const string _recallRequestTag="Spug's recall request";private readonly
IMyBroadcastListener _myBroadcastListener;private readonly IMyUnicastListener _myUnicastListener;public IMyRadioAntenna antenna;public List<
IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();public Program parent_program;public AntennaHandler(Program _program){
parent_program=_program;_myBroadcastListener=parent_program.IGC.RegisterBroadcastListener(_recallRequestTag);_myUnicastListener=
parent_program.IGC.UnicastListener;_myUnicastListener.SetMessageCallback("UNICAST");_myBroadcastListener.SetMessageCallback(
_recallRequestTag);}public string CheckAntenna(){var antenna_found_success=0;parent_program.GridTerminalSystem.GetBlocks(blocks);foreach(
var block in blocks)if(block is IMyRadioAntenna&&blockIsOnMyGrid(block)){var my_antenna=(IMyRadioAntenna)block;if(
antenna_found_success<1){antenna_found_success=1;antenna=my_antenna;}if(my_antenna.Enabled&&antenna_found_success<2){antenna_found_success=2;
antenna=my_antenna;}if(my_antenna.EnableBroadcasting&&my_antenna.Enabled&&antenna_found_success<3){antenna_found_success=3;
antenna=my_antenna;}}if(antenna_found_success==3)return"Antenna found:\nReady for use with the\noptional home script.";return""
;}public void SendPositionUpdateRequest(long target_platform){var target_connector_id=parent_program.systemsAnalyzer.
currentHomeLocation.stationConnectorID;var target_grid_id=parent_program.systemsAnalyzer.currentHomeLocation.stationGridID;parent_program.
IGC.SendBroadcastMessage(_outgoingRequestTag,target_connector_id+";"+target_grid_id);}public bool blockIsOnMyGrid(
IMyTerminalBlock block){return block.CubeGrid.EntityId==parent_program.Me.CubeGrid.EntityId;}public void HandleMessage(){while(
_myBroadcastListener.HasPendingMessage){var myIGCMessage=_myBroadcastListener.AcceptMessage();if(myIGCMessage.Tag==_recallRequestTag){var
data=myIGCMessage.Data.ToString();var data_parts=data.Split(';');var arg=data_parts[0];long sourceGrid=0;long.TryParse(
data_parts[1],out sourceGrid);parent_program.shipIOHandler.Echo("Broadcast received. This ship has been ordered to dock.");var
arg_test=parent_program.FindHomeLocation(arg);if(arg_test!=null&&sourceGrid!=parent_program.Me.CubeGrid.EntityId)parent_program.
Main(arg,UpdateType.Script);}}var bFoundMessages=false;do{bFoundMessages=false;if(_myUnicastListener.HasPendingMessage){
bFoundMessages=true;var msg=_myUnicastListener.AcceptMessage();if(msg.Tag==_responseTag)ParsePositionalResponse(msg.Data.ToString());}
}while(bFoundMessages);}void ParsePositionalResponse(string data){var data_parts=data.Split(';');if(data_parts.Length==1)
{parent_program.shipIOHandler.Clear();parent_program.shipIOHandler.Echo(data);parent_program.SafelyExit();parent_program.
shipIOHandler.EchoFinish();}else if(data_parts.Length>1){parent_program.hasConnectionToAntenna=true;var result=parent_program.
systemsAnalyzer.currentHomeLocation.UpdateDataFromOptionalHomeScript(data_parts);if(result.Length>0)parent_program.runningIssues=result
;}}}public class HomeLocation{public HashSet<string>arguments=new HashSet<string>();public IMyShipConnector shipConnector
;public string shipConnectorName;public Vector3D stationAcceleration=Vector3D.Zero;public Vector3D stationAngularVelocity
=Vector3D.Zero;public Vector3D stationConnectorForward;public long stationConnectorID;public Vector3D
stationConnectorLeft;public Vector3D stationConnectorPosition;public string stationConnectorName=null;public string stationGridName=null;
public double stationConnectorSize;public Vector3D stationConnectorUpGlobal;public Vector3D stationConnectorUpLocal;public
long stationGridID;public Vector3D stationVelocity=Vector3D.Zero;private const char landing_sequence_delimeter='ç';private
const char waypoint_delimeter='å';private const char waypoint_data_delimeter='ã';public Dictionary<string,List<Waypoint>>
landingSequences=new Dictionary<string,List<Waypoint>>();public static Dictionary<string,List<Waypoint>>LandingSequencesFromString(
string data){Dictionary<string,List<Waypoint>>result=new Dictionary<string,List<Waypoint>>();if(!string.IsNullOrWhiteSpace(
data)){string[]split_landing_sequences=data.Split(landing_sequence_delimeter);foreach(string landingSequenceStr in
split_landing_sequences){if(landingSequenceStr!=""){string[]split_into_waypoints=landingSequenceStr.Split(waypoint_delimeter);if(
split_into_waypoints.Length>1){string current_arg=split_into_waypoints[0];List<Waypoint>currentLandingSequence=new List<Waypoint>();for(int
i=1;i<split_into_waypoints.Length;i++){if(split_into_waypoints[i]!=""){string[]waypoint_data=split_into_waypoints[i].
Split(waypoint_data_delimeter);Vector3D pos=new Vector3D();Vector3D forward=new Vector3D();Vector3D up=new Vector3D();double
waypoint_accuracy=0.2;double top_speed=1;bool require_rotation=true;Vector3D.TryParse(waypoint_data[0],out pos);Vector3D.TryParse(
waypoint_data[1],out forward);Vector3D.TryParse(waypoint_data[2],out up);double.TryParse(waypoint_data[3],out top_speed);bool.
TryParse(waypoint_data[4],out require_rotation);double.TryParse(waypoint_data[5],out waypoint_accuracy);Waypoint newWaypoint=new
Waypoint(pos,forward,up);newWaypoint.WaypointIsLocal=true;newWaypoint.waypoint_completion_accuracy=waypoint_accuracy;newWaypoint
.top_speed=top_speed;newWaypoint.RequireRotation=require_rotation;currentLandingSequence.Add(newWaypoint);}}if(
currentLandingSequence.Count>0){result[current_arg]=currentLandingSequence;}}}}}return result;}public static string LandingSequencesToString(
Dictionary<string,List<Waypoint>>landingSequences){string o_string="";foreach(KeyValuePair<string,List<Waypoint>>
landingSequenceEntry in landingSequences){string current_landing_sequence_string=landingSequenceEntry.Key+waypoint_delimeter;foreach(
Waypoint waypoint in landingSequenceEntry.Value){string waypoint_representation="";waypoint_representation+=waypoint.position.
ToString()+waypoint_data_delimeter;waypoint_representation+=waypoint.forward.ToString()+waypoint_data_delimeter;
waypoint_representation+=waypoint.auxilleryDirection.ToString()+waypoint_data_delimeter;waypoint_representation+=waypoint.top_speed.ToString()+
waypoint_data_delimeter;waypoint_representation+=waypoint.RequireRotation.ToString()+waypoint_data_delimeter;waypoint_representation+=waypoint.
waypoint_completion_accuracy.ToString();current_landing_sequence_string+=waypoint_representation+waypoint_delimeter;}o_string+=
current_landing_sequence_string+landing_sequence_delimeter;}return o_string;}public HomeLocation(string gridName){stationGridName=gridName;}public
HomeLocation(string new_arg,IMyShipConnector my_connector,IMyShipConnector station_connector){arguments.Add(new_arg);UpdateData(
my_connector,station_connector);}public string ProduceUserFriendlyData(){var o_string="";foreach(var arg in arguments){o_string+=
stationGridName+";";o_string+=stationConnectorName+";";o_string+=shipConnector.CustomName+";";o_string+=arg+";";int waypointCount=0;if(
landingSequences.ContainsKey(arg)){waypointCount=landingSequences[arg].Count;}o_string+=waypointCount.ToString();o_string+="\n";}if(
o_string.Length>1){o_string=o_string.Substring(0,o_string.Length-1);}return o_string;}public void UpdateData(IMyShipConnector
my_connector,IMyShipConnector station_connector){shipConnectorName=my_connector.CustomName;shipConnector=my_connector;
stationConnectorID=station_connector.EntityId;stationConnectorPosition=station_connector.GetPosition();stationConnectorForward=
station_connector.WorldMatrix.Forward;stationConnectorLeft=station_connector.WorldMatrix.Left;stationConnectorName=station_connector.
CustomName;stationGridName=station_connector.CubeGrid.CustomName;var normalizedleft=Vector3D.Normalize(PID.ProjectPointOnPlane(
stationConnectorForward,Vector3D.Zero,my_connector.WorldMatrix.Left));var saved_up=normalizedleft.Cross(stationConnectorForward);
stationConnectorUpGlobal=saved_up;stationConnectorUpLocal=worldDirectionToLocalDirection(stationConnectorUpGlobal,station_connector.WorldMatrix)
;stationGridID=station_connector.CubeGrid.EntityId;stationConnectorSize=ShipSystemsAnalyzer.GetRadiusOfConnector(
station_connector);}public static Vector3D worldDirectionToLocalDirection(Vector3D world_direction,MatrixD world_matrix){return Vector3D.
TransformNormal(world_direction,MatrixD.Transpose(world_matrix));}public static Vector3D localDirectionToWorldDirection(Vector3D
local_direction,MatrixD world_matrix){return Vector3D.TransformNormal(local_direction,world_matrix);}public static Vector3D
localDirectionToWorldDirection(Vector3D local_direction,HomeLocation referenceHomeLocation){MatrixD newWorldMatrix=Matrix.CreateWorld(
referenceHomeLocation.stationConnectorPosition,referenceHomeLocation.stationConnectorForward,(-referenceHomeLocation.stationConnectorLeft).
Cross(referenceHomeLocation.stationConnectorForward));return Vector3D.TransformNormal(local_direction,newWorldMatrix);;}
public static Vector3D localPositionToWorldPosition(Vector3D local_position,MatrixD world_matrix){return Vector3D.Transform(
local_position,world_matrix);}public static Vector3D localPositionToWorldPosition(Vector3D local_position,HomeLocation
referenceHomeLocation){MatrixD newWorldMatrix=Matrix.CreateWorld(referenceHomeLocation.stationConnectorPosition,referenceHomeLocation.
stationConnectorForward,(-referenceHomeLocation.stationConnectorLeft).Cross(referenceHomeLocation.stationConnectorForward));return Vector3D.
Transform(local_position,newWorldMatrix);}public static Vector3D worldPositionToLocalPosition(Vector3D world_position,MatrixD
world_matrix){Vector3D worldDirection=world_position-world_matrix.Translation;return Vector3D.TransformNormal(worldDirection,MatrixD
.Transpose(world_matrix));}public void UpdateShipConnectorUsingName(Program parent_program){shipConnector=(
IMyShipConnector)parent_program.GridTerminalSystem.GetBlockWithName(shipConnectorName);}public string UpdateDataFromOptionalHomeScript(
string[]data_parts){var issuestring="";if(data_parts.Length==6){Vector3D.TryParse(data_parts[0],out stationConnectorPosition);
Vector3D.TryParse(data_parts[1],out stationConnectorForward);Vector3D.TryParse(data_parts[2],out stationConnectorLeft);Vector3D.
TryParse(data_parts[3],out stationVelocity);Vector3D.TryParse(data_parts[4],out stationAngularVelocity);Vector3D.TryParse(
data_parts[5],out stationAcceleration);if(stationConnectorUpLocal!=Vector3D.Zero){MatrixD newWorldMatrix=Matrix.CreateWorld(
stationConnectorPosition,stationConnectorForward,(-stationConnectorLeft).Cross(stationConnectorForward));var newConnectorUpGlobal=
localDirectionToWorldDirection(stationConnectorUpLocal,newWorldMatrix);stationConnectorUpGlobal=newConnectorUpGlobal;}}else{issuestring=
"Warning:\nGot a corrupted message back from the optional home script.\nMaybe it is an old version?";}return issuestring;}public override bool Equals(object obj){if(obj==null||GetType()!=obj.GetType())return false;var
test=(HomeLocation)obj;if(shipConnectorName==test.shipConnectorName&&stationConnectorID==test.stationConnectorID)return true
;return false;}public override int GetHashCode(){var hashCode=-48872655;hashCode=hashCode*-1521134295+EqualityComparer<
HashSet<string>>.Default.GetHashCode(arguments);hashCode=hashCode*-1521134295+shipConnectorName.GetHashCode();hashCode=hashCode
*-1521134295+stationConnectorID.GetHashCode();return hashCode;}}public class IOHandler{private readonly string _scriptID;
private readonly Program parent_program;string echoLine="";public List<IMyTextSurface>output_LCDs=new List<IMyTextSurface>();
public List<IMyTimerBlock>output_timers=new List<IMyTimerBlock>();public List<IMyTimerBlock>output_start_timers=new List<
IMyTimerBlock>();public List<IMyTimerBlock>output_undocked_timers=new List<IMyTimerBlock>();public IOHandler(string scriptID,Program
_parent_program){_scriptID=scriptID;parent_program=_parent_program;FindOutputBlocks();}public static double RoundToSignificantDigits(
double d,int digits){if(d==0)return 0;var scale=Math.Pow(10,Math.Floor(Math.Log10(Math.Abs(d)))+1);return scale*Math.Round(d/
scale,digits);}public void FindOutputBlocks(){output_start_timers=new List<IMyTimerBlock>();if(parent_program.
force_timer_search_on_station){List<IMyTerminalBlock>t_search_blocks=new List<IMyTerminalBlock>();parent_program.GridTerminalSystem.GetBlocks(
t_search_blocks);output_timers=new List<IMyTimerBlock>();output_LCDs=new List<IMyTextSurface>();foreach(var block in t_search_blocks){
string blockName=block.CustomName.ToLower();if(block is IMyTimerBlock&&blockName.Contains(parent_program.timer_tag))
output_timers.Add((IMyTimerBlock)block);if(block is IMyTextSurface&&blockName.Contains(parent_program.lcd_tag))output_LCDs.Add((
IMyTextSurface)block);if(block is IMyTimerBlock&&blockName.Contains(parent_program.start_timer_tag)&&blockIsOnMyGrid(block))
output_start_timers.Add((IMyTimerBlock)block);if(block is IMyTimerBlock&&blockName.Contains(parent_program.undocked_timer_tag)){
output_undocked_timers.Add((IMyTimerBlock)block);}}output_LCDs.AddRange(Display.Find(t_search_blocks,parent_program.lcd_tag,_scriptID));}else{
output_timers=new List<IMyTimerBlock>();output_LCDs=new List<IMyTextSurface>();foreach(var block in parent_program.blocks){string
blockName=block.CustomName.ToLower();if(block is IMyTimerBlock&&blockName.Contains(parent_program.timer_tag)&&blockIsOnMyGrid(
block))output_timers.Add((IMyTimerBlock)block);if(block is IMyTextSurface&&blockName.Contains(parent_program.lcd_tag)&&
blockIsOnMyGrid(block))output_LCDs.Add((IMyTextSurface)block);if(block is IMyTimerBlock&&blockName.Contains(parent_program.
start_timer_tag)&&blockIsOnMyGrid(block))output_start_timers.Add((IMyTimerBlock)block);if(block is IMyTimerBlock&&blockName.Contains(
parent_program.undocked_timer_tag)){output_undocked_timers.Add((IMyTimerBlock)block);}}output_LCDs.AddRange(Display.Find(
parent_program.blocks,parent_program.lcd_tag,_scriptID));}}public bool blockIsOnMyGrid(IMyTerminalBlock block){return block.CubeGrid.
EntityId==parent_program.Me.CubeGrid.EntityId;}public void Error(string ErrorString){if(!parent_program.errorState)echoLine="";
Echo("ERROR:\n"+ErrorString);parent_program.errorState=true;parent_program.SafelyExit();EchoFinish();}public void
WritePastableCoords(Vector3D coords,string coord_name="0"){Echo("GPS:"+coord_name+":"+coords.X+":"+coords.Y+":"+coords.Z+":");}public void
Clear(){echoLine="";}public void Echo(object inp){echoLine+=inp+"\n";}public static string ConvertArg(string argument){if(
argument=="")return"no argument";return argument;}public void OutputTimer(){if(parent_program.force_timer_search_on_station){
FindOutputBlocks();}if(output_timers.Count>0)foreach(var timer in output_timers)if(timer!=null)if(timer.IsWorking)timer.Trigger();}
public void OutputStartTimer(){if(output_start_timers.Count>0)foreach(var timer in output_start_timers)if(timer!=null)if(timer
.IsWorking)timer.Trigger();}public void OutputUndockedTimer(){foreach(IMyTimerBlock timer in output_undocked_timers.Where
(timer=>timer?.IsWorking==true)){timer.StartCountdown();}}public void WaypointEcho(string arg,int count,string
extra_output){if(count==0){Echo("RECORDING MODE\nRecording to argument: "+ConvertArg(arg)+
".\nPressing Run will record\nposition and rotation. To finish, press Run when docked.\n\nTo cancel, press Recompile.");}else{Echo("RECORDING MODE\nRecorded "+count+" waypoints to argument: "+ConvertArg(arg)+extra_output+"\nPressing Run will record position and rotation again. To finish, press Run when docked.\n\nTo cancel, press Recompile."
);}}public void EchoFinish(bool OnlyInProgrammingBlock=false,float fontSize=1){if(echoLine!=""){if(parent_program.
runningIssues.Length>0)parent_program.runningIssues+="\n";var echoString=$"{parent_program.runningIssues}{echoLine}";parent_program.
Echo(echoString);if(!OnlyInProgrammingBlock&&output_LCDs.Count>0)foreach(var surface in output_LCDs)if(surface!=null){
surface.ContentType=ContentType.TEXT_AND_IMAGE;surface.WriteText(echoString);}echoLine="";}}public void OutputHomeLocations(){
Echo("Known docking locations:");var count=1;foreach(var currentHomeLocation in parent_program.homeLocations){var argStr=
"- Location "+count+" arguments: ";foreach(var arg in currentHomeLocation.arguments){var arg_r=arg;if(arg=="")arg_r="NO ARG";argStr+=
arg_r+", ";}Echo(argStr.Substring(0,argStr.Length-2));count+=1;}}public string GetHomeLocationArguments(HomeLocation
currentHomeLocation){var argStr="";foreach(var arg in currentHomeLocation.arguments){var arg_r=arg;if(arg=="")arg_r="NO ARG";argStr+=arg_r+
", ";}if(argStr.Length>2)argStr=argStr.Substring(0,argStr.Length-2);return argStr;}public void DockingSequenceStartMessage(
string argument){if(argument=="")Echo("RUNNING\nAttempting docking sequence\nwith no argument.");else Echo(
"RUNNING\nAttempting docking sequence\nwith argument: "+argument);}}public class PID{private readonly bool _integralDecay;private readonly double _integralDecayRatio;private
readonly double _kD;private readonly double _kI;private readonly double _kP;private readonly double _lowerBound;private readonly
double _upperBound;double _errorSum;bool _firstRun=true;double _inverseTimeStep;double _lastError;double _timeStep;public PID(
double kP,double kI,double kD,double lowerBound,double upperBound,double timeStep){_kP=kP;_kI=kI;_kD=kD;_lowerBound=lowerBound
;_upperBound=upperBound;_timeStep=timeStep;_inverseTimeStep=1/_timeStep;_integralDecay=false;}public PID(double kP,double
kI,double kD,double integralDecayRatio,double timeStep){_kP=kP;_kI=kI;_kD=kD;_timeStep=timeStep;_inverseTimeStep=1/
_timeStep;_integralDecayRatio=integralDecayRatio;_integralDecay=true;}public double Value{get;private set;}public static Vector3D
VectorProjection(Vector3D a,Vector3D b){var projection=a.Dot(b)/b.LengthSquared()*b;return projection;}public static int
VectorCompareDirection(Vector3D a,Vector3D b){var check=a.Dot(b);if(check<0)return-1;return 1;}public static double VectorSignedAngleBetween(
Vector3D current,Vector3D target,Vector3D axisOfRotation,bool requireProjection=false){Vector3D current_adjusted=current;if(
requireProjection){current_adjusted=ProjectPointOnPlane(axisOfRotation,Vector3D.Zero,current);}double angle=Math.Acos(MathHelper.Clamp(
target.Dot(current_adjusted),-1,1));Vector3D cross=target.Cross(current_adjusted);if(axisOfRotation.Dot(cross)<0){angle=-angle
;}return angle;}public static double VectorAngleBetween(Vector3D a,Vector3D b){if(a.LengthSquared()==0||b.LengthSquared()
==0)return 0;return Math.Acos(MathHelper.Clamp(a.Dot(b)/a.Length()/b.Length(),-1,1));}public static Vector3D
NearestPointOnLine(Vector3D linePoint,Vector3D lineDirection,Vector3D point){var lineDir=Vector3D.Normalize(lineDirection);var v=point-
linePoint;var d=v.Dot(lineDir);return linePoint+lineDir*d;}public static Vector3D ProjectPointOnPlane(Vector3D planeNormal,
Vector3D planePoint,Vector3D point){double distance;Vector3D translationVector;distance=SignedDistancePlanePoint(planeNormal,
planePoint,point);distance*=-1;translationVector=SetVectorLength(planeNormal,distance);return point+translationVector;}public
static double SignedDistancePlanePoint(Vector3D planeNormal,Vector3D planePoint,Vector3D point){return Vector3D.Dot(
planeNormal,point-planePoint);}public static Vector3D SetVectorLength(Vector3D vector,double size){var vectorNormalized=Vector3D.
Normalize(vector);return vectorNormalized*=size;}public static void ComputeCoefficients(double[,]X,double[]Y){int I,J,K,K1,N;N=Y.
Length;for(K=0;K<N;K++){K1=K+1;for(I=K;I<N;I++)if(X[I,K]!=0){for(J=K1;J<N;J++)X[I,J]/=X[I,K];Y[I]/=X[I,K];}for(I=K1;I<N;I++)if
(X[I,K]!=0){for(J=K1;J<N;J++)X[I,J]-=X[K,J];Y[I]-=Y[K];}}for(I=N-2;I>=0;I--)for(J=N-1;J>=I+1;J--)Y[I]-=X[I,J]*Y[J];}
public static void Invert(ref Matrix3x3 matrix,out Matrix3x3 result){var num=matrix.Determinant();var num2=1f/num;result.M11=(
matrix.M22*matrix.M33-matrix.M32*matrix.M23)*num2;result.M12=(matrix.M13*matrix.M32-matrix.M12*matrix.M33)*num2;result.M13=(
matrix.M12*matrix.M23-matrix.M13*matrix.M22)*num2;result.M21=(matrix.M23*matrix.M31-matrix.M21*matrix.M33)*num2;result.M22=(
matrix.M11*matrix.M33-matrix.M13*matrix.M31)*num2;result.M23=(matrix.M21*matrix.M13-matrix.M11*matrix.M23)*num2;result.M31=(
matrix.M21*matrix.M32-matrix.M31*matrix.M22)*num2;result.M32=(matrix.M31*matrix.M12-matrix.M11*matrix.M32)*num2;result.M33=(
matrix.M11*matrix.M22-matrix.M21*matrix.M12)*num2;}public double Control(double error){var errorDerivative=(error-_lastError)*
_inverseTimeStep;if(_firstRun){errorDerivative=0;_firstRun=false;}if(!_integralDecay){_errorSum+=error*_timeStep;if(_errorSum>
_upperBound)_errorSum=_upperBound;else if(_errorSum<_lowerBound)_errorSum=_lowerBound;}else{_errorSum=_errorSum*(1.0-
_integralDecayRatio)+error*_timeStep;}_lastError=error;Value=_kP*error+_kI*_errorSum+_kD*errorDerivative;return Value;}public double
Control(double error,double timeStep){_timeStep=timeStep;_inverseTimeStep=1/_timeStep;return Control(error);}public void Reset(
){_errorSum=0;_lastError=0;_firstRun=true;}}
    // CHANGEABLE VARIABLES:

    int speedSetting = 2;                           // 1 = Cinematic, 2 = Classic, 3 = Breakneck
                                                                 // Cinematic: Slower but looks cooler, especially for larger ships.
                                                                 // Classic: Lands at the classic pace.
                                                                 // Breakneck: Still safe, but will land pretty much as quick as it can.

    double caution = 0.4;                                             // Between 0 - 0.9. Defines how close to max deceleration the ship will ride.
    bool extra_info = false;                                          // If true, this script will give you more information about what's happening than usual.
    double topSpeed = 100;                                         // The top speed the ship will go in m/s.

    double connector_clearance = 0;                          // If you raise this number (measured in meters), the ship will fly connector_clearance higher before coming down onto the connector.
    double add_acceleration = 0;                                // If your ship is accelerating very slow, or perhaps stopping at a low top speed, try raising this (e.g to 10).

    string lcd_tag = "[dock]";                                       // The text you can add to an LCD block name. The LCD will then output this block's output
    string timer_tag = "[dock]";                                   // The text you can add to a timer block name. The timer will then be triggered on a completed dock.
    bool force_timer_search_on_station = false;       // If enabled, the ship will make sure it always searches the station for [dock] (the timer_tag) in the names of any timer blocks.
    string start_timer_tag = "[start dock]";                 // A timer with this text in the name will be triggered as soon as a docking procedure is started.
    string undocked_timer_tag = "[undocked]";

    bool enable_antenna_function = true;                   //If enabled, the ship will try to search for an optional home script. Disable if the antenna functionality is giving you problems.

    bool allow_connector_on_seperate_grid = false; // WARNING: All connectors on your ship must have [dock] in the name if you set this to true! This option allows your connector to not be on the same grid.

    double high_speed_lead_amount = 1;                  // WARNING: Only change if the ship can't land on high speed ships (over 120 m/s).
                                                                                  //If this number is less than 1, the ship will fly ahead of the connector more when the connector is moving. This can be a negative number, however try 0 first.

    // Waypoint settings:
    double required_waypoint_accuracy = 6;             // how close the ship needs to be to a waypoint to complete it (measured in meters). Do note, closer waypoints are more accurate anyway.
    double waypoints_top_speed = 100;                     // the top speed the ship will go in m/s when it's moving towards waypoints
    bool rotate_during_waypoints = true;                    // if true, the ship will rotate to face each waypoint's direction as it goes along.

    // This code has been minified by Malware's MDK minifier.
    // Find the original source code here:
    // https://github.com/ksqk34/Autodocking-2

    // DO NOT CHANGE BELOW THIS LINE
    // Well you can try...
    private readonly ShipSystemsAnalyzer systemsAnalyzer;
    public const string About=Application+" "+Version+"\r\n(C) irreality.net 2024-2025\r\n(C) Spug 2020";public const
string ScriptID="Autodocking";private const string Application="Spug's Auto Docking\r\nby Abrasax Industries";private const
string Version="1.4";private const double updatesPerSecond=10;private const double proportionalConstant=2;private const double
derivativeConstant=.5;private const double timeLimit=1/updatesPerSecond;private readonly AntennaHandler antennaHandler;private readonly
List<HomeLocation>homeLocations;private readonly double issueDetection=0;private readonly PID pitchPID;private readonly PID
rollPID;private readonly IOHandler shipIOHandler;private readonly ShipSystemsController systemsController;private readonly PID
yawPID;double anglePitch;double angleRoll;double angleYaw;public readonly List<IMyTerminalBlock>blocks;string current_argument
;double DeltaTimeReal;double DeltaTime;bool errorState;bool hasConnectionToAntenna;bool lastUpdateWasApproach;Vector3D
platformVelocity;DateTime previousTime;Vector3D previousVelocity=Vector3D.Zero;string runningIssues="";double safetyAcceleration=1;bool
scriptEnabled;DateTime scriptStartTime;int current_waypoint_number=0;string status="";double timeElapsed;double
timeElapsedSinceAntennaCheck;double topSpeedUsed=100;Display _display;ConfigurationBuilder _configuration;Vector3D?_startUndockingPosition;public
 Program
(){_display=new Display(Me.CustomName,Me.GetSurface(0),Me.BlockDefinition.SubtypeId);_display.Clear();_display.WriteLine(
About);if(string.IsNullOrWhiteSpace(Me.CustomData)){_display.GetRecommendedSettings().CopyTo(_display);}_configuration=new
ConfigurationBuilder(ScriptID);LoadConfiguration();errorState=false;Runtime.UpdateFrequency=UpdateFrequency.Once;platformVelocity=Vector3D.
Zero;blocks=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocks(blocks);homeLocations=new List<HomeLocation>();
shipIOHandler=new IOHandler(ScriptID,this);LoadHomeLocations();antennaHandler=new AntennaHandler(this);systemsAnalyzer=new
ShipSystemsAnalyzer(this);systemsController=new ShipSystemsController(this);pitchPID=new PID(proportionalConstant,0,derivativeConstant,-10,
10,timeLimit);rollPID=new PID(proportionalConstant,0,derivativeConstant,-10,10,timeLimit);yawPID=new PID(
proportionalConstant,0,derivativeConstant,-10,10,timeLimit);timeElapsed=0;timeElapsedSinceAntennaCheck=0;SafelyExit();}public string
your_title{get;set;}public bool small_ship_rotate_on_connector{get;set;}bool large_ship_rotate_on_connector{get;set;}bool
rotate_on_approach{get;set;}bool extra_soft_landing_mode{get;set;}int UndockingDistance{get;set;}double AlignWithGravity(Waypoint waypoint
,bool requireYawControl){if(waypoint.RequireRotation){var referenceBlock=systemsAnalyzer.currentHomeLocation.
shipConnector;var referenceOrigin=referenceBlock.GetPosition();var targetDirection=-waypoint.forward;var gravityVecLength=
targetDirection.Length();if(targetDirection.LengthSquared()==0){foreach(var thisGyro in systemsAnalyzer.gyros)thisGyro.SetValue(
"Override",false);return-1;}var block_WorldMatrix=Matrix.CreateWorld(referenceOrigin,referenceBlock.WorldMatrix.Up,-referenceBlock
.WorldMatrix.Forward);var referenceForward=block_WorldMatrix.Forward;var referenceLeft=block_WorldMatrix.Left;var
referenceUp=block_WorldMatrix.Up;anglePitch=Math.Acos(MathHelper.Clamp(targetDirection.Dot(referenceForward)/gravityVecLength,-1,1)
)-Math.PI/2;Vector3D planetRelativeLeftVec=referenceForward.Cross(targetDirection);angleRoll=PID.VectorAngleBetween(
referenceLeft,planetRelativeLeftVec);angleRoll*=PID.VectorCompareDirection(PID.VectorProjection(referenceLeft,targetDirection),
targetDirection);if(requireYawControl)angleYaw=Math.Acos(MathHelper.Clamp(waypoint.auxilleryDirection.Dot(referenceLeft),-1,1))-Math.PI
/2;else angleYaw=0;anglePitch*=-1;angleRoll*=-1;var rawDevAngle=Math.Acos(MathHelper.Clamp(targetDirection.Dot(
referenceForward),-1,1))*180/Math.PI;rawDevAngle-=90;var rollSpeed=rollPID.Control(angleRoll);var pitchSpeed=pitchPID.Control(anglePitch
);double yawSpeed=0;if(requireYawControl)yawSpeed=yawPID.Control(angleYaw);if(!errorState)systemsController.
ApplyGyroOverride(pitchSpeed,yawSpeed,-rollSpeed,systemsAnalyzer.gyros,block_WorldMatrix);return rawDevAngle;}return-1;}double
AlignWithWaypoint(Waypoint waypoint){MatrixD stationConnectorWorldMatrix=Matrix.CreateWorld(systemsAnalyzer.currentHomeLocation.
stationConnectorPosition,systemsAnalyzer.currentHomeLocation.stationConnectorForward,(-systemsAnalyzer.currentHomeLocation.stationConnectorLeft)
.Cross(systemsAnalyzer.currentHomeLocation.stationConnectorForward));var referenceGrid=Me.CubeGrid;Vector3D
waypointForward=HomeLocation.localDirectionToWorldDirection(waypoint.forward,systemsAnalyzer.currentHomeLocation);Vector3D
waypointRight=HomeLocation.localDirectionToWorldDirection(waypoint.auxilleryDirection,systemsAnalyzer.currentHomeLocation);var
targetDirection=waypointForward;var referenceOrigin=referenceGrid.GetPosition();var block_WorldMatrix=Matrix.CreateWorld(
referenceOrigin,referenceGrid.WorldMatrix.Up,-referenceGrid.WorldMatrix.Forward);var referenceForward=block_WorldMatrix.Forward;var
referenceLeft=block_WorldMatrix.Left;var referenceUp=block_WorldMatrix.Up;anglePitch=Math.Acos(MathHelper.Clamp(targetDirection.Dot(
referenceForward),-1,1))-Math.PI/2;Vector3D relativeLeftVec=referenceForward.Cross(targetDirection);angleRoll=PID.VectorAngleBetween(
referenceLeft,relativeLeftVec);angleRoll*=PID.VectorCompareDirection(PID.VectorProjection(referenceLeft,targetDirection),
targetDirection);Vector3D waypointUp=(-waypointRight).Cross(waypointForward);angleYaw=Math.Acos(MathHelper.Clamp((-waypointUp).Dot(
referenceLeft),-1,1))-Math.PI/2;anglePitch*=-1;angleRoll*=-1;var rawDevAngle=Math.Acos(MathHelper.Clamp(targetDirection.Dot(
referenceForward),-1,1))*180/Math.PI;rawDevAngle-=90;var rollSpeed=rollPID.Control(angleRoll)*1;var pitchSpeed=pitchPID.Control(
anglePitch)*1;double yawSpeed=yawPID.Control(angleYaw)*1;if(!errorState)systemsController.ApplyGyroOverride(pitchSpeed,yawSpeed,-
rollSpeed,systemsAnalyzer.gyros,block_WorldMatrix);return rawDevAngle;}public void Begin(string argument,IMyShipConnector
connectorOverride=null){systemsAnalyzer.currentHomeLocation=FindHomeLocation(argument);if(systemsAnalyzer.currentHomeLocation!=null){if(
connectorOverride!=null){systemsAnalyzer.currentHomeLocation.shipConnector=connectorOverride;systemsAnalyzer.currentHomeLocation.
shipConnectorName=connectorOverride.CustomName;}shipIOHandler.OutputStartTimer();systemsAnalyzer.currentHomeLocation.stationVelocity=
Vector3D.Zero;systemsAnalyzer.currentHomeLocation.stationAcceleration=Vector3D.Zero;systemsAnalyzer.currentHomeLocation.
stationAngularVelocity=Vector3D.Zero;current_argument=argument;scriptEnabled=true;hasConnectionToAntenna=false;lastUpdateWasApproach=false;
current_waypoint_number=0;runningIssues="";safetyAcceleration=1;Runtime.UpdateFrequency=UpdateFrequency.Update1;scriptStartTime=DateTime.Now;
previousTime=DateTime.Now;previousVelocity=systemsAnalyzer.cockpit.GetShipVelocities().LinearVelocity;if(enable_antenna_function)
antennaHandler.SendPositionUpdateRequest(1);}else{SafelyExit();}}public string updateHomeLocation(string argument,IMyShipConnector
my_connected_connector){var station_connector=my_connected_connector.OtherConnector;if(station_connector==null){shipIOHandler.Error(
"\nSomething went wrong when finding the connector.\nMaybe you have multiple connectors on the go, "+your_title+"?");return"";}var newHomeLocation=new HomeLocation(argument,my_connected_connector,station_connector);var
HomeLocationIndex=homeLocations.LastIndexOf(newHomeLocation);if(HomeLocationIndex!=-1){if(extra_info)shipIOHandler.Echo(
"- Docking location already Exists!\n- Adding argument.");if(!homeLocations[HomeLocationIndex].arguments.Contains(argument)){if(extra_info)shipIOHandler.Echo(
"Other arguments associated: "+shipIOHandler.GetHomeLocationArguments(homeLocations[HomeLocationIndex]));homeLocations[HomeLocationIndex].arguments.
Add(argument);if(extra_info)shipIOHandler.Echo("- New argument added.");}else if(extra_info){shipIOHandler.Echo(
"- Argument already in!");if(extra_info)shipIOHandler.Echo("All arguments associated: "+shipIOHandler.GetHomeLocationArguments(homeLocations[
HomeLocationIndex]));}homeLocations[HomeLocationIndex].UpdateData(my_connected_connector,station_connector);}else{homeLocations.Add(
newHomeLocation);if(extra_info)shipIOHandler.Echo("- Added new docking location.");}var amountFound=0;var toDelete=new List<
HomeLocation>();foreach(var currentHomeLocation in homeLocations)if(!currentHomeLocation.Equals(newHomeLocation))if(
currentHomeLocation.arguments.Contains(argument)){amountFound+=1;currentHomeLocation.arguments.Remove(argument);if(currentHomeLocation.
arguments.Count==0)toDelete.Add(currentHomeLocation);}while(toDelete.Count>0){homeLocations.Remove(toDelete[0]);toDelete.RemoveAt
(0);}if(extra_info){if(amountFound==1)shipIOHandler.Echo(
"- Found 1 other association with that argument. Removed this other.");else if(amountFound>1)shipIOHandler.Echo("- Found "+amountFound+
" other associations with that argument. Removed these others.");}SaveHomeLocations();if(argument==""){if(!extra_info)return"SAVED\nSaved docking location as no argument, "+your_title
+".";return"Saved docking location as no argument, "+your_title+".";}if(!extra_info)return
"SAVED\nSaved docking location as "+argument+", "+your_title+".";return"Saved docking location as "+argument+", "+your_title+".";}bool recording=false;bool
waiting_for_arg=false;string recording_arg="";List<Vector3D>waypoints_positions;List<Vector3D>waypoints_forwards;List<Vector3D>
waypoints_rights;List<double>waypoints_speeds;List<bool>waypoints_rotates;public void beginRecordingSetup(){recording=true;
waiting_for_arg=true;SafelyExit();shipIOHandler.Echo(
"RECORDING MODE\nPlease enter an argument that\nwill be associated with these waypoints then press Run, "+your_title+".\n\nTo cancel, press Recompile.");}public void beginRecordingWaypoints(string arg){if(arg.ToLower().Trim()
=="record"){shipIOHandler.Echo("RECORDING MODE\nPlease choose an argument\nother than record, then press Run, "+your_title
+".\n\nTo cancel, press Recompile.");}else{recording=true;waiting_for_arg=false;recording_arg=arg;current_waypoint_number
=0;waypoints_positions=new List<Vector3D>();waypoints_forwards=new List<Vector3D>();waypoints_rights=new List<Vector3D>()
;waypoints_speeds=new List<double>();waypoints_rotates=new List<bool>();shipIOHandler.WaypointEcho(recording_arg,
current_waypoint_number,"");}}public double accuracyFromDistance(Vector3D start_pos,Vector3D end_pos){double dist=Vector3D.Distance(start_pos,
end_pos)/4;if(dist<0.2){dist=0.2;}return dist;}public void recordWaypoint(IMyShipConnector connectedConnector,string argument){
if(connectedConnector==null){waypoints_positions.Add(Me.CubeGrid.GetPosition());waypoints_forwards.Add(Me.CubeGrid.
WorldMatrix.Forward);waypoints_rights.Add(Me.CubeGrid.WorldMatrix.Right);double speed=-1;bool waypoints_rotate=true;if(argument.
Trim().Length>1){if(argument.Trim()[0]=='!'){string second_part=argument.Remove(0,1);double speed_num;bool result=double.
TryParse(second_part,out speed_num);if(result){speed=speed_num;}}}if(argument.ToLower().Trim().Contains("no spin")||argument.
ToLower().Trim().Contains("nospin")||argument.ToLower().Trim().Contains("!nospin")){waypoints_rotate=false;}waypoints_speeds.
Add(speed);waypoints_rotates.Add(waypoints_rotate);string extra_output="";if(speed>0){extra_output="\nRecorded speed: "+
speed;}if(!waypoints_rotate){extra_output+="\nRecorded no rotate";}current_waypoint_number+=1;shipIOHandler.WaypointEcho(
recording_arg,current_waypoint_number,extra_output);}else{shipIOHandler.Echo("FINISHED RECORDING");if(current_waypoint_number==0){
shipIOHandler.Echo("No waypoints recorded.");var result=updateHomeLocation(recording_arg,connectedConnector);}else{recording=false;
waiting_for_arg=false;var result=updateHomeLocation(recording_arg,connectedConnector);HomeLocation currentHomeLocation=FindHomeLocation
(recording_arg);List<Waypoint>landing_sequence=new List<Waypoint>();Vector3D stationConnectorPos=currentHomeLocation.
stationConnectorPosition;Vector3D stationConnectorForward=currentHomeLocation.stationConnectorForward;Vector3D stationConnectorLeft=
currentHomeLocation.stationConnectorLeft;MatrixD stationConnectorWorldMatrix=Matrix.CreateWorld(stationConnectorPos,stationConnectorForward
,(-stationConnectorLeft).Cross(stationConnectorForward));Vector3D last_world_pos=Vector3D.Zero;for(int waypointIndex=0;
waypointIndex<current_waypoint_number;waypointIndex++){Vector3D waypointGlobalPosition=waypoints_positions[waypointIndex];Vector3D
waypointLocalPositionToStation=HomeLocation.worldPositionToLocalPosition(waypointGlobalPosition,stationConnectorWorldMatrix);double
calculated_accuracy=required_waypoint_accuracy;if(waypointIndex>0){double last_accuracy=accuracyFromDistance(waypointGlobalPosition,
last_world_pos);calculated_accuracy=Math.Min(last_accuracy,required_waypoint_accuracy);}if(waypointIndex==current_waypoint_number-1){
double last_accuracy=accuracyFromDistance(waypointGlobalPosition,stationConnectorPos)*0.7;calculated_accuracy=Math.Min(
last_accuracy,calculated_accuracy);}Vector3D gridForwardToLocal=HomeLocation.worldDirectionToLocalDirection(waypoints_forwards[
waypointIndex],stationConnectorWorldMatrix);Vector3D gridRightToLocal=HomeLocation.worldDirectionToLocalDirection(waypoints_rights[
waypointIndex],stationConnectorWorldMatrix);double waypoint_speed=waypoints_speeds[waypointIndex];if(waypoint_speed<0){waypoint_speed
=waypoints_top_speed;}Waypoint newWaypoint=new Waypoint(waypointLocalPositionToStation,gridForwardToLocal,
gridRightToLocal){WaypointIsLocal=true,maximumAcceleration=15,RequireRotation=waypoints_rotates[waypointIndex],
waypoint_completion_accuracy=calculated_accuracy,top_speed=waypoint_speed};last_world_pos=waypointGlobalPosition;landing_sequence.Add(newWaypoint);}
if(currentHomeLocation.landingSequences.ContainsKey(recording_arg)){shipIOHandler.Echo("Overwriting existing waypoints.");
}currentHomeLocation.landingSequences[recording_arg]=landing_sequence;shipIOHandler.Echo("Recorded "+(
current_waypoint_number+1).ToString()+" waypoints to argument: "+IOHandler.ConvertArg(recording_arg));current_waypoint_number=0;}}}public
string checkForClear(string argument){if(argument.ToLower()=="clear"){return string.Empty;}string[]split=argument.Split(' ');
if(split.Length>1){if(split[0].ToLower()=="clear"){return argument.Remove(0,6);}else{return null;}}else{return null;}}
public void ClearMemoryLocation(string argument){if(argument.Length>0){bool found_arg=false;var toDelete=new List<HomeLocation
>();foreach(var currentHomeLocation in homeLocations)if(currentHomeLocation.arguments.Contains(argument)){
currentHomeLocation.arguments.Remove(argument);if(currentHomeLocation.landingSequences.ContainsKey(argument)){currentHomeLocation.
landingSequences.Remove(argument);}found_arg=true;if(currentHomeLocation.arguments.Count==0)toDelete.Add(currentHomeLocation);}while(
toDelete.Count>0){homeLocations.Remove(toDelete[0]);toDelete.RemoveAt(0);}if(found_arg){shipIOHandler.Echo(
"CLEARED\nThe argument: "+IOHandler.ConvertArg(argument)+"\nhas been cleared from memory.");}else{shipIOHandler.Echo("WARNING\nThe argument: "+
IOHandler.ConvertArg(argument)+"\nwasn't found in memory.");}}else{shipIOHandler.Echo(
"CLEARED\nAll locations have been cleared from memory.");homeLocations.Clear();}SaveHomeLocations();}public string ProduceDataOutputString(){string o_string="";foreach(var
homeLocation in homeLocations){o_string+=homeLocation.ProduceUserFriendlyData()+"\n";}if(o_string.Length>1){o_string=o_string.
Substring(0,o_string.Length-1);}return o_string;}public IMyShipConnector CheckForConnectorOverride(ref string argument){if(
argument.Contains("!")){string[]arg_split=argument.Trim().Split('!');if(arg_split.Length>1){string afterSeperator=arg_split[1].
TrimEnd();if(afterSeperator.Length>0){long ID_extracted;bool success=long.TryParse(afterSeperator,out ID_extracted);if(success)
{IMyShipConnector new_connector=(IMyShipConnector)GridTerminalSystem.GetBlockWithId(ID_extracted);if(new_connector!=null)
{string resultant_arg="";string[]raw_split=argument.Split('!');for(int i=0;i<raw_split.Length-1;i++){resultant_arg+=
raw_split[i];}resultant_arg=resultant_arg.TrimEnd();argument=resultant_arg;return new_connector;}}}}}return null;}public bool
checkForReadonly(ref string argument){if(argument.Length>1){if(argument[0]=='!'&&argument.Contains(" ")){string[]arg_split=argument.
Split(' ');if(arg_split.Length>1){if(arg_split[0].ToLower()=="!readonly"){argument=argument.Remove(0,10);return true;}}}}
return false;}public void
 Main
(string argument,UpdateType updateSource){if(argument.StartsWith("GracefulShutDown::")){return;}if((updateSource&(
UpdateType.Update1|UpdateType.Once|UpdateType.IGC))==0){if(ExecuteCommand(argument)){return;}if(errorState){errorState=false;
systemsAnalyzer.GatherBasicData();}if(!errorState){if(!recording){var my_connected_connector=systemsAnalyzer.FindMyConnectedConnector()
;var clear_command=checkForClear(argument);if(argument.ToLower().Trim()=="record"){if(my_connected_connector==null){
beginRecordingSetup();}else{shipIOHandler.Echo("WARNING\nPlease make sure you are not connected\nto a home connector before recording, "+
your_title+".");}}else if(clear_command!=null){ClearMemoryLocation(clear_command);}else{var connectorOverride=
CheckForConnectorOverride(ref argument);bool user_wants_readonly=checkForReadonly(ref argument);if(user_wants_readonly&&my_connected_connector!=
null){if(my_connected_connector.Status==MyShipConnectorStatus.Connectable){my_connected_connector=null;}}if(
my_connected_connector==null){if(scriptEnabled&&argument==current_argument){shipIOHandler.Echo("STOPPED\nAwaiting orders, "+your_title+".");
SafelyExit();}else{if(connectorOverride!=null){Begin(argument,connectorOverride);}else{Begin(argument);}}}else{if(!
user_wants_readonly){if(connectorOverride!=null){my_connected_connector=connectorOverride;}var result=updateHomeLocation(argument,
my_connected_connector);shipIOHandler.Echo(result);SafelyExit();}else{shipIOHandler.Echo(
"OVERRIDDEN\nShip save has been overriden\ndue to the !readonly command.");}}}}else{if(waiting_for_arg){beginRecordingWaypoints(argument);}else{var connectorOverride=CheckForConnectorOverride(
ref argument);var my_connected_connector=systemsAnalyzer.FindMyConnectedConnector();if(my_connected_connector!=null&&
connectorOverride!=null){my_connected_connector=connectorOverride;}recordWaypoint(my_connected_connector,argument);}}}shipIOHandler.
EchoFinish();}if(VerifyUndocking()){return;}if(scriptEnabled&&!errorState){timeElapsed+=Runtime.TimeSinceLastRun.TotalSeconds;if(
timeElapsed>=timeLimit){systemsAnalyzer.CheckForMassChange();if(hasConnectionToAntenna&&enable_antenna_function)antennaHandler.
SendPositionUpdateRequest(1);DockingSequenceFrameUpdate();timeElapsed=0;}}if(enable_antenna_function&&scriptEnabled&&!errorState&&!
hasConnectionToAntenna){timeElapsedSinceAntennaCheck+=Runtime.TimeSinceLastRun.TotalSeconds;if(timeElapsedSinceAntennaCheck>=1){antennaHandler
.SendPositionUpdateRequest(1);timeElapsedSinceAntennaCheck=0;}}if((updateSource&UpdateType.IGC)!=0&&
enable_antenna_function)antennaHandler.HandleMessage();}void LoadConfiguration(){MyIniParseResult customDataParseResult;if(_configuration.
TryLoad(Me.CustomData,out customDataParseResult)){your_title=_configuration.YourTitle;small_ship_rotate_on_connector=
_configuration.RotateSmallShipOnConnector;large_ship_rotate_on_connector=_configuration.RotateLargeShipOnConnector;rotate_on_approach=
_configuration.RotateOnApproach;extra_soft_landing_mode=_configuration.ExtraSoftLandingMode;UndockingDistance=_configuration.
UndockingDistance;SaveConfiguration();}else{your_title=ConfigurationBuilder.DefaultYourTitle;small_ship_rotate_on_connector=
ConfigurationBuilder.DefaultRotateSmallShipOnConnector;large_ship_rotate_on_connector=ConfigurationBuilder.DefaultRotateLargeShipOnConnector
;rotate_on_approach=ConfigurationBuilder.DefaultRotateOnApproach;extra_soft_landing_mode=ConfigurationBuilder.
DefaultExtraSoftLandingMode;UndockingDistance=ConfigurationBuilder.DefaultUndockingDistance;_display.WriteLine("Error while loading configuration."
);_display.WriteLine(customDataParseResult.ToString());}}void SaveConfiguration(){Me.CustomData=_configuration.ToString()
;}void SaveHomeLocations(){ISet<string>toRemove=_configuration.GridNames;foreach(HomeLocation homeLocation in
homeLocations){DockConfigurationBuilder dock=_configuration.Dock(homeLocation.stationGridName);dock.GridID=homeLocation.stationGridID
;dock.ConnectorName=homeLocation.stationConnectorName;dock.ConnectorID=homeLocation.stationConnectorID;dock.
ConnectorPosition=homeLocation.stationConnectorPosition;dock.ConnectorForward=homeLocation.stationConnectorForward;dock.ConnectorUpGlobal
=homeLocation.stationConnectorUpGlobal;dock.ConnectorUpLocal=homeLocation.stationConnectorUpLocal;dock.ConnectorLeft=
homeLocation.stationConnectorLeft;dock.ConnectorSize=homeLocation.stationConnectorSize;dock.MyConnectorName=homeLocation.
shipConnectorName;dock.LandingSequences=HomeLocation.LandingSequencesToString(homeLocation.landingSequences);dock.Arguments=homeLocation.
arguments;toRemove.Remove(dock.GridName);}foreach(string gridName in toRemove){_configuration.DeleteDock(gridName);}
SaveConfiguration();}void LoadHomeLocations(){homeLocations.Clear();foreach(string gridName in _configuration.GridNames){
DockConfigurationBuilder dock=_configuration.Dock(gridName);HomeLocation homeLocation=new HomeLocation(gridName){stationGridID=dock.GridID,
stationConnectorName=dock.ConnectorName,stationConnectorID=dock.ConnectorID,stationConnectorPosition=dock.ConnectorPosition,
stationConnectorForward=dock.ConnectorForward,stationConnectorUpGlobal=dock.ConnectorUpGlobal,stationConnectorUpLocal=dock.ConnectorUpLocal,
stationConnectorLeft=dock.ConnectorLeft,stationConnectorSize=dock.ConnectorSize,shipConnectorName=dock.MyConnectorName,landingSequences=
HomeLocation.LandingSequencesFromString(dock.LandingSequences)};foreach(string argument in dock.Arguments){homeLocation.arguments.
Add(argument);}homeLocation.UpdateShipConnectorUsingName(this);homeLocations.Add(homeLocation);}}bool ExecuteCommand(string
command){command=command.Trim();if(command.Length==0){return true;}switch(command.ToLower()){case"about":_display.WriteLine(
About);return true;case"cls":_display.Clear();return true;case"list":shipIOHandler.OutputHomeLocations();shipIOHandler.
EchoFinish();return true;case"write":SaveHomeLocations();return true;case"undock":StartUndocking();return true;}return false;}
HomeLocation FindHomeLocation(string argument){var amountFound=0;HomeLocation resultantHomeLocation=null;foreach(var
currentHomeLocation in homeLocations)if(currentHomeLocation.arguments.Contains(argument)){amountFound+=1;if(resultantHomeLocation==null)
resultantHomeLocation=currentHomeLocation;}if(amountFound>1)shipIOHandler.Echo("Minor Warning:\nThere are "+amountFound+
" places\nthat argument is associated with!\nPicking first one found, "+your_title+".");else if(amountFound==0)shipIOHandler.Echo("WARNING:\nNo docking location found with that argument, "+
your_title+".\nPlease dock to a connector and press 'Run' with your argument\nto save it as a docking location.");return
resultantHomeLocation;}void DockingSequenceFrameUpdate(){if(systemsAnalyzer.currentHomeLocation.landingSequences.ContainsKey(current_argument
)){List<Waypoint>landing_sequence=systemsAnalyzer.currentHomeLocation.landingSequences[current_argument];if(
current_waypoint_number>=landing_sequence.Count){AutoLandToConnector(true);}else{Waypoint currentWaypoint=landing_sequence[
current_waypoint_number];Waypoint nextWaypoint=null;if(current_waypoint_number<landing_sequence.Count-1){nextWaypoint=landing_sequence[
current_waypoint_number+1];}double dist_to_waypoint=AutoFollowWaypoint(currentWaypoint,nextWaypoint);double accuracy=Math.Min(
required_waypoint_accuracy,currentWaypoint.waypoint_completion_accuracy);if(dist_to_waypoint<accuracy){current_waypoint_number+=1;}}}else{
AutoLandToConnector(false);}}void AutoLandToConnector(bool only_last_landing){var dontRotateOnConnector=!small_ship_rotate_on_connector&&!
systemsAnalyzer.isLargeShip||!large_ship_rotate_on_connector&&systemsAnalyzer.isLargeShip;if(systemsAnalyzer.currentHomeLocation.
shipConnector.Status==MyShipConnectorStatus.Connected||systemsAnalyzer.currentHomeLocation.shipConnector.Status==
MyShipConnectorStatus.Connectable&&dontRotateOnConnector){ConnectAndDock();}else{shipIOHandler.DockingSequenceStartMessage(current_argument);
if(systemsAnalyzer.basicDataGatherRequired){systemsAnalyzer.GatherBasicData();systemsAnalyzer.basicDataGatherRequired=
false;}if(errorState==false){double sideways_dist_needed_to_land=3;var rotate_on_connector_accuracy=0.015;double
height_needed_for_connector=5;if(speedSetting==1){height_needed_for_connector=7;topSpeedUsed=10;}else if(speedSetting==3){
height_needed_for_connector=4;topSpeedUsed=topSpeed;rotate_on_connector_accuracy=0.03;}else{height_needed_for_connector=6;topSpeedUsed=topSpeed;}if
(extra_soft_landing_mode)height_needed_for_connector=8;height_needed_for_connector+=connector_clearance;if(
only_last_landing)height_needed_for_connector=0;if(topSpeedUsed>topSpeed)topSpeedUsed=topSpeed;if(systemsAnalyzer.currentHomeLocation.
stationVelocity.Length()>5){rotate_on_connector_accuracy=0.035;sideways_dist_needed_to_land=5;}var speedDampener=1-((systemsAnalyzer.
currentHomeLocation.stationVelocity.Length()/100)*0.2*high_speed_lead_amount);var ConnectorLocation=systemsAnalyzer.currentHomeLocation.
stationConnectorPosition+DeltaTimeReal*systemsAnalyzer.currentHomeLocation.stationVelocity*speedDampener;var ConnectorDirection=systemsAnalyzer.
currentHomeLocation.stationConnectorForward;var ConnectorUp=systemsAnalyzer.currentHomeLocation.stationConnectorUpGlobal;var
target_position=ConnectorLocation+ConnectorDirection*height_needed_for_connector;var current_position=systemsAnalyzer.
currentHomeLocation.shipConnector.GetPosition();var point_in_sequence="Starting...";var aboveConnectorWaypoint=new Waypoint(target_position
,ConnectorDirection,ConnectorUp);double direction_accuracy;var connectedLate=false;if(!dontRotateOnConnector&&
systemsAnalyzer.currentHomeLocation.shipConnector.Status==MyShipConnectorStatus.Connectable){direction_accuracy=AlignWithGravity(
aboveConnectorWaypoint,true);if(Math.Abs(angleYaw)<rotate_on_connector_accuracy){ConnectAndDock();connectedLate=true;}}else{var yaw_rotate=
false;if(rotate_on_approach&&lastUpdateWasApproach)yaw_rotate=true;direction_accuracy=AlignWithGravity(aboveConnectorWaypoint
,yaw_rotate);}lastUpdateWasApproach=false;if(!connectedLate){if(Math.Abs(direction_accuracy)<15){var pointOnConnectorAxis
=PID.NearestPointOnLine(ConnectorLocation,ConnectorDirection,current_position);var heightDifference=pointOnConnectorAxis-
ConnectorLocation;var signedHeightDistanceToConnector=ConnectorDirection.Dot(Vector3D.Normalize(heightDifference))*heightDifference.
Length();var sidewaysDistance=(current_position-pointOnConnectorAxis).Length();if(sidewaysDistance>
sideways_dist_needed_to_land&&signedHeightDistanceToConnector<height_needed_for_connector*0.9&&!only_last_landing){const double overshoot=2;var
SomewhereOnCorrectSide=new Waypoint(current_position+ConnectorDirection*(-signedHeightDistanceToConnector+overshoot+
height_needed_for_connector),ConnectorDirection,ConnectorUp);SomewhereOnCorrectSide.maximumAcceleration=20;SomewhereOnCorrectSide.required_accuracy
=0.8;if(speedSetting==1)aboveConnectorWaypoint.maximumAcceleration=8;else if(speedSetting==3)aboveConnectorWaypoint.
maximumAcceleration=20;else aboveConnectorWaypoint.maximumAcceleration=10;MoveToWaypoint(SomewhereOnCorrectSide);point_in_sequence=
"Behind target, moving to be in front";}else if(sidewaysDistance>sideways_dist_needed_to_land&&!only_last_landing){if(speedSetting==1)aboveConnectorWaypoint.
maximumAcceleration=5;else if(speedSetting==3)aboveConnectorWaypoint.maximumAcceleration=15;else aboveConnectorWaypoint.maximumAcceleration
=5;MoveToWaypoint(aboveConnectorWaypoint);point_in_sequence="Moving toward connector";}else{var connectorHeight=
systemsAnalyzer.currentHomeLocation.stationConnectorSize+ShipSystemsAnalyzer.GetRadiusOfConnector(systemsAnalyzer.currentHomeLocation.
shipConnector);var DockedToConnector=new Waypoint(ConnectorLocation+ConnectorDirection*connectorHeight,ConnectorDirection,ConnectorUp
);DockedToConnector.maximumAcceleration=3;if(speedSetting==1)aboveConnectorWaypoint.maximumAcceleration=1;else if(
speedSetting==3)aboveConnectorWaypoint.maximumAcceleration=3;else aboveConnectorWaypoint.maximumAcceleration=1;if(
extra_soft_landing_mode)topSpeedUsed=2;var acc=MoveToWaypoint(DockedToConnector);point_in_sequence="landing on connector";lastUpdateWasApproach
=true;}}else{status="Rotating";point_in_sequence="Rotating to connector";}if(extra_info){shipIOHandler.Echo("Status: "+
status);shipIOHandler.Echo("Place in sequence: "+point_in_sequence);}var elapsed=DateTime.Now-scriptStartTime;shipIOHandler.
Echo("\nTime elapsed: "+elapsed.Seconds+"."+elapsed.Milliseconds.ToString().Substring(0,1));shipIOHandler.EchoFinish();}}}}
double AutoFollowWaypoint(Waypoint currentWaypoint,Waypoint nextWaypoint){shipIOHandler.DockingSequenceStartMessage(
current_argument);if(systemsAnalyzer.basicDataGatherRequired){systemsAnalyzer.GatherBasicData();systemsAnalyzer.basicDataGatherRequired=
false;}if(errorState==true)return 0;if(speedSetting==1){topSpeedUsed=10;}else if(speedSetting==3){topSpeedUsed=
currentWaypoint.top_speed;}else{topSpeedUsed=currentWaypoint.top_speed;}if(topSpeedUsed>currentWaypoint.top_speed)topSpeedUsed=
currentWaypoint.top_speed;if(rotate_during_waypoints&&currentWaypoint.RequireRotation){AlignWithWaypoint(currentWaypoint);}if(
speedSetting==1)currentWaypoint.maximumAcceleration=15;else if(speedSetting==3)currentWaypoint.maximumAcceleration=15;else
currentWaypoint.maximumAcceleration=20;double dist_left=MoveToWaypoint(currentWaypoint);if(extra_info){shipIOHandler.Echo("Status: "+
status);shipIOHandler.Echo("Moving to waypoint: "+(current_waypoint_number+1).ToString()+".");}else{shipIOHandler.Echo(
"Moving to waypoint: "+(current_waypoint_number+1).ToString()+".");}var elapsed=DateTime.Now-scriptStartTime;shipIOHandler.Echo(
"\nTime elapsed: "+elapsed.Seconds+"."+elapsed.Milliseconds.ToString().Substring(0,1));shipIOHandler.EchoFinish();return dist_left;}double
MoveToWaypoint(Waypoint waypoint){DeltaTime=Runtime.TimeSinceLastRun.TotalSeconds*10;DeltaTimeReal=(DateTime.Now-previousTime).
TotalSeconds;var CurrentVelocity=systemsAnalyzer.cockpit.GetShipVelocities().LinearVelocity;var VelocityChange=CurrentVelocity-
previousVelocity;var ActualAcceleration=Vector3D.Zero;if(DeltaTimeReal>0)ActualAcceleration=VelocityChange/DeltaTimeReal;systemsAnalyzer
.UpdateThrusterGroupsWorldDirections();ThrusterGroup forceThrusterGroup=null;status="ERROR";var UnknownAcceleration=-
systemsAnalyzer.currentHomeLocation.stationAcceleration*safetyAcceleration;var Gravity_And_Unknown_Forces=(systemsAnalyzer.cockpit.
GetNaturalGravity()+UnknownAcceleration)*systemsAnalyzer.shipMass;Vector3D waypointPos=waypoint.position;if(waypoint.WaypointIsLocal){
waypointPos=HomeLocation.localPositionToWorldPosition(waypointPos,systemsAnalyzer.currentHomeLocation);}var TargetRoute=waypointPos
-systemsAnalyzer.currentHomeLocation.shipConnector.GetPosition();if(waypoint.WaypointIsLocal){TargetRoute=waypointPos-Me.
CubeGrid.GetPosition();}var TargetDirection=Vector3D.Normalize(TargetRoute);var totalDistanceLeft=TargetRoute.Length();var
LeadVelocity=(CurrentVelocity-systemsAnalyzer.currentHomeLocation.stationVelocity).Length()+DeltaTime*(waypoint.maximumAcceleration+
issueDetection+add_acceleration);if(LeadVelocity>topSpeedUsed)LeadVelocity=topSpeedUsed;var TargetVelocity=TargetDirection*
LeadVelocity+systemsAnalyzer.currentHomeLocation.stationVelocity;var velocityDifference=CurrentVelocity-TargetVelocity;double
max_forward_acceleration;if(velocityDifference.Length()==0){max_forward_acceleration=0;}else{var forward_thrust_direction=Vector3D.Normalize(
TargetRoute);forward_thrust_direction=-Vector3D.Normalize(velocityDifference);forceThrusterGroup=systemsAnalyzer.SolveMaxThrust(
Gravity_And_Unknown_Forces,forward_thrust_direction);if(forceThrusterGroup==null){shipIOHandler.Echo("Not enough thrust!");previousVelocity=
CurrentVelocity;safetyAcceleration=0;previousTime=DateTime.Now;return totalDistanceLeft;}max_forward_acceleration=forceThrusterGroup.
lambdaResult/systemsAnalyzer.shipMass;}var reverse_target_velocity=systemsAnalyzer.currentHomeLocation.stationVelocity;var
reverse_velocity_difference=CurrentVelocity-reverse_target_velocity;double max_reverse_acceleration;if(reverse_velocity_difference.Length()==0){
max_reverse_acceleration=0;}else{var reverse_thrust_direction=-Vector3D.Normalize(TargetRoute);forceThrusterGroup=systemsAnalyzer.SolveMaxThrust
(Gravity_And_Unknown_Forces,reverse_thrust_direction);if(forceThrusterGroup==null){shipIOHandler.Echo(
"Not enough thrust!");safetyAcceleration=0;previousVelocity=CurrentVelocity;previousTime=DateTime.Now;return totalDistanceLeft;}
max_reverse_acceleration=forceThrusterGroup.lambdaResult/systemsAnalyzer.shipMass;}double distanceToGetToZero=0;var Accelerating=false;if(
max_reverse_acceleration!=0){double timeToGetToZero=0;timeToGetToZero=reverse_velocity_difference.Length()/(max_reverse_acceleration*(1-caution)
*waypoint.PercentageOfMaxAcceleration);timeToGetToZero+=DeltaTime;distanceToGetToZero=reverse_velocity_difference.Length(
)*timeToGetToZero/2;}if(distanceToGetToZero+waypoint.required_accuracy<totalDistanceLeft)Accelerating=true;if(
Accelerating){var target_acceleration=-velocityDifference/DeltaTime;var target_thrust=target_acceleration*systemsAnalyzer.shipMass;
var target_acceleration_amount=target_acceleration.Length();if(target_acceleration_amount>max_forward_acceleration){
forceThrusterGroup=systemsAnalyzer.SolveMaxThrust(Gravity_And_Unknown_Forces,Vector3.Normalize(target_acceleration));status="Speeding up";
}else{forceThrusterGroup=systemsAnalyzer.SolvePartialThrust(Gravity_And_Unknown_Forces,target_thrust);status="Drifting";}
}else{var target_acceleration2=-reverse_velocity_difference/DeltaTime;var target_thrust2=target_acceleration2*
systemsAnalyzer.shipMass;var target_acceleration_amount=target_acceleration2.Length();if(target_acceleration_amount>
max_reverse_acceleration){forceThrusterGroup=systemsAnalyzer.SolveMaxThrust(Gravity_And_Unknown_Forces,Vector3.Normalize(target_acceleration2));
status="Slowing down";}else{forceThrusterGroup=systemsAnalyzer.SolvePartialThrust(Gravity_And_Unknown_Forces,target_thrust2);
status="Finished";}}SetResultantForces(forceThrusterGroup);previousVelocity=CurrentVelocity;previousTime=DateTime.Now;return
totalDistanceLeft;}void SetResultantAcceleration(Vector3D Gravity_And_Unknown_Forces,Vector3D TargetForceDirection,double
proportionOfThrustToUse){var maxForceThrusterGroup=systemsAnalyzer.SolveMaxThrust(-Gravity_And_Unknown_Forces,TargetForceDirection,
proportionOfThrustToUse);systemsController.SetThrusterForces(maxForceThrusterGroup.finalThrusterGroups[3],0);systemsController.
SetThrusterForces(maxForceThrusterGroup.finalThrusterGroups[4],0);systemsController.SetThrusterForces(maxForceThrusterGroup.
finalThrusterGroups[5],0);systemsController.SetThrusterForces(maxForceThrusterGroup.finalThrusterGroups[0],maxForceThrusterGroup.
finalThrustForces.X);systemsController.SetThrusterForces(maxForceThrusterGroup.finalThrusterGroups[1],maxForceThrusterGroup.
finalThrustForces.Y);systemsController.SetThrusterForces(maxForceThrusterGroup.finalThrusterGroups[2],maxForceThrusterGroup.
finalThrustForces.Z);}void SetResultantForces(ThrusterGroup maxForceThrusterGroup){systemsController.SetThrusterForces(
maxForceThrusterGroup.finalThrusterGroups[3],0);systemsController.SetThrusterForces(maxForceThrusterGroup.finalThrusterGroups[4],0);
systemsController.SetThrusterForces(maxForceThrusterGroup.finalThrusterGroups[5],0);systemsController.SetThrusterForces(
maxForceThrusterGroup.finalThrusterGroups[0],maxForceThrusterGroup.finalThrustForces.X);systemsController.SetThrusterForces(
maxForceThrusterGroup.finalThrusterGroups[1],maxForceThrusterGroup.finalThrustForces.Y);systemsController.SetThrusterForces(
maxForceThrusterGroup.finalThrusterGroups[2],maxForceThrusterGroup.finalThrustForces.Z);}void ConnectAndDock(){SafelyExit();systemsAnalyzer.
currentHomeLocation.shipConnector.Connect();shipIOHandler.Clear();shipIOHandler.Echo("DOCKED\nThe ship has docked "+your_title+
"!\nI will patiently await.");shipIOHandler.EchoFinish();shipIOHandler.OutputTimer();}void SafelyExit(){Runtime.UpdateFrequency|=UpdateFrequency.
Update1;scriptEnabled=false;runningIssues="";_startUndockingPosition=null;if(systemsAnalyzer!=null){foreach(var thisGyro in
systemsAnalyzer.gyros)if(thisGyro!=null)if(thisGyro.IsWorking)thisGyro.SetValue("Override",false);foreach(var thisThruster in
systemsAnalyzer.thrusters)if(thisThruster!=null)if(thisThruster.IsWorking)thisThruster.SetValue("Override",0f);}}void StartUndocking(){
if(_startUndockingPosition==null){shipIOHandler.Clear();List<IMyThrust>allThrusters=blocks.OfType<IMyThrust>().Where(block
=>block.CubeGrid.IsSameConstructAs(Me.CubeGrid)).ToList();foreach(IMyThrust thruster in allThrusters){thruster.
ThrustOverridePercentage=0;thruster.Enabled=true;}List<IMyGyro>allGyroscopes=blocks.OfType<IMyGyro>().Where(block=>block.CubeGrid.
IsSameConstructAs(Me.CubeGrid)).ToList();foreach(IMyGyro gyroscope in allGyroscopes){gyroscope.GyroOverride=false;}systemsAnalyzer.
GatherBasicData();shipIOHandler.Echo($"UNDOCKING");foreach(IMyThrust thruster in systemsAnalyzer.UpThrust.thrusters){thruster.
ThrustOverridePercentage=1;}_startUndockingPosition=Me.GetPosition();Runtime.UpdateFrequency=UpdateFrequency.Update1;systemsAnalyzer.
FindMyConnectedConnector()?.Disconnect();shipIOHandler.EchoFinish();}else{SafelyExit();}}bool VerifyUndocking(){if(_startUndockingPosition.
HasValue){double distance=Vector3D.Distance(Me.GetPosition(),_startUndockingPosition.Value);shipIOHandler.Clear();shipIOHandler.
Echo($"UNDOCKING\n{distance:F1} m");if(distance>=UndockingDistance){FinishUndocking();}shipIOHandler.EchoFinish();return
true;}else{return false;}}void FinishUndocking(){shipIOHandler.Clear();shipIOHandler.Echo("UNDOCKED");SafelyExit();
shipIOHandler.OutputUndockedTimer();}public class ShipSystemsAnalyzer{public ThrusterGroup BackwardThrust;public bool
basicDataGatherRequired=false;public IMyShipController cockpit;public HomeLocation currentHomeLocation;public ThrusterGroup DownThrust;bool
firstTime=true;public ThrusterGroup ForwardThrust;public List<IMyGyro>gyros=new List<IMyGyro>();public bool isLargeShip;public
ThrusterGroup LeftThrust;public Program parent_program;public float previousShipMass=9999;public IMyRemoteControl remote_control;
public ThrusterGroup RightThrust;public float shipMass=9999;public Dictionary<Base6Directions.Direction,ThrusterGroup>
thrusterGroups;public List<IMyThrust>thrusters=new List<IMyThrust>();double off_thrusters_count=0;public ThrusterGroup UpThrust;public
ShipSystemsAnalyzer(Program in_parent_program){parent_program=in_parent_program;parent_program.shipIOHandler.Echo("INITIALIZED");
GatherBasicData();}public ThrusterGroup SolveMaxThrust(Vector3D minus_g,Vector3D targetDirection,double maxPercentageThrustToUse=1){
Base6Directions.Direction actual2Di;Base6Directions.Direction actual3Di;double t2c;double t3c;double Lambda;ThrusterGroup t1;
ThrusterGroup t2;ThrusterGroup t3;if(maxPercentageThrustToUse>1)maxPercentageThrustToUse=1;else if(maxPercentageThrustToUse<0)
maxPercentageThrustToUse=0;foreach(var entry in thrusterGroups){t1=entry.Value;if(t1.LocalThrustDirection==Base6Directions.Direction.Up||t1.
LocalThrustDirection==Base6Directions.Direction.Down){t2=thrusterGroups[Base6Directions.Direction.Left];t3=thrusterGroups[Base6Directions.
Direction.Forward];}else if(t1.LocalThrustDirection==Base6Directions.Direction.Left||t1.LocalThrustDirection==Base6Directions.
Direction.Right){t2=thrusterGroups[Base6Directions.Direction.Up];t3=thrusterGroups[Base6Directions.Direction.Forward];}else if(t1
.LocalThrustDirection==Base6Directions.Direction.Forward||t1.LocalThrustDirection==Base6Directions.Direction.Backward){t2
=thrusterGroups[Base6Directions.Direction.Up];t3=thrusterGroups[Base6Directions.Direction.Left];}else{parent_program.
shipIOHandler.Error("Encountered unusual thruster direction.\nIf you've gotten this error in particular,\nplease report it to the script owner, Spug."
);t2=thrusterGroups[Base6Directions.Direction.Up];t3=thrusterGroups[Base6Directions.Direction.Left];}t1.matrixM[0,0]=t2.
WorldThrustDirection.X;t1.matrixM[0,1]=t3.WorldThrustDirection.X;t1.matrixM[0,2]=-targetDirection.X;t1.matrixM[1,0]=t2.WorldThrustDirection.
Y;t1.matrixM[1,1]=t3.WorldThrustDirection.Y;t1.matrixM[1,2]=-targetDirection.Y;t1.matrixM[2,0]=t2.WorldThrustDirection.Z;
t1.matrixM[2,1]=t3.WorldThrustDirection.Z;t1.matrixM[2,2]=-targetDirection.Z;t1.ANS[0]=-t1.MaxThrust*t1.
WorldThrustDirection.X*maxPercentageThrustToUse-minus_g.X;t1.ANS[1]=-t1.MaxThrust*t1.WorldThrustDirection.Y*maxPercentageThrustToUse-minus_g
.Y;t1.ANS[2]=-t1.MaxThrust*t1.WorldThrustDirection.Z*maxPercentageThrustToUse-minus_g.Z;PID.ComputeCoefficients(t1.
matrixM,t1.ANS);t2c=t1.ANS[0];t3c=t1.ANS[1];Lambda=t1.ANS[2];actual2Di=t2.LocalThrustDirection;actual3Di=t3.
LocalThrustDirection;if(t2c<0){actual2Di=Base6Directions.GetOppositeDirection(t2.LocalThrustDirection);t2c*=-1;}if(t3c<0){actual3Di=
Base6Directions.GetOppositeDirection(t3.LocalThrustDirection);t3c*=-1;}t1.finalThrustForces.X=t1.MaxThrust*maxPercentageThrustToUse;t1.
finalThrustForces.Y=t2c;t1.finalThrustForces.Z=t3c;t1.lambdaResult=Lambda;t1.finalThrusterGroups[0]=t1;t1.finalThrusterGroups[1]=
thrusterGroups[actual2Di];t1.finalThrusterGroups[2]=thrusterGroups[actual3Di];t1.finalThrusterGroups[3]=thrusterGroups[Base6Directions
.GetOppositeDirection(t1.LocalThrustDirection)];t1.finalThrusterGroups[4]=thrusterGroups[Base6Directions.
GetOppositeDirection(t2.LocalThrustDirection)];t1.finalThrusterGroups[5]=thrusterGroups[Base6Directions.GetOppositeDirection(t3.
LocalThrustDirection)];}ThrusterGroup bestCandidate=null;double bestCandidateLambda=-999999999;foreach(var entry in thrusterGroups)if(entry.
Value.lambdaResult>bestCandidateLambda)if(entry.Value.finalThrustForces.Y<=entry.Value.finalThrusterGroups[1].MaxThrust+1&&
entry.Value.finalThrustForces.Z<=entry.Value.finalThrusterGroups[2].MaxThrust+1){bestCandidate=entry.Value;
bestCandidateLambda=entry.Value.lambdaResult;}return bestCandidate;}public ThrusterGroup SolvePartialThrust(Vector3D g,Vector3D
targetThrust){var resultantForce=-g+targetThrust;var t1=FindThrusterGroupsInDirection(resultantForce);t1.matrixM[0,0]=t1.
finalThrusterGroups[0].WorldThrustDirection.X;t1.matrixM[0,1]=t1.finalThrusterGroups[1].WorldThrustDirection.X;t1.matrixM[0,2]=t1.
finalThrusterGroups[2].WorldThrustDirection.X;t1.matrixM[1,0]=t1.finalThrusterGroups[0].WorldThrustDirection.Y;t1.matrixM[1,1]=t1.
finalThrusterGroups[1].WorldThrustDirection.Y;t1.matrixM[1,2]=t1.finalThrusterGroups[2].WorldThrustDirection.Y;t1.matrixM[2,0]=t1.
finalThrusterGroups[0].WorldThrustDirection.Z;t1.matrixM[2,1]=t1.finalThrusterGroups[1].WorldThrustDirection.Z;t1.matrixM[2,2]=t1.
finalThrusterGroups[2].WorldThrustDirection.Z;t1.ANS[0]=resultantForce.X;t1.ANS[1]=resultantForce.Y;t1.ANS[2]=resultantForce.Z;PID.
ComputeCoefficients(t1.matrixM,t1.ANS);t1.finalThrustForces.X=t1.ANS[0];t1.finalThrustForces.Y=t1.ANS[1];t1.finalThrustForces.Z=t1.ANS[2];
return t1;}public ThrusterGroup FindThrusterGroupsInDirection(Vector3D _WorldDirection){var AccumulatorGroup=new List<
ThrusterGroup>();var OtherGroup=new List<ThrusterGroup>();var WorldDirection=Vector3D.Normalize(_WorldDirection);foreach(var entry in
thrusterGroups){var currentThrusterGroup=entry.Value;if(currentThrusterGroup.WorldThrustDirection.Dot(WorldDirection)>0)
AccumulatorGroup.Add(currentThrusterGroup);else OtherGroup.Add(currentThrusterGroup);}if(AccumulatorGroup.Count>3){AccumulatorGroup.Sort
(delegate(ThrusterGroup c1,ThrusterGroup c2){if(c1.WorldThrustDirection.Dot(WorldDirection)>c2.WorldThrustDirection.Dot(
WorldDirection))return-1;return 1;});for(var i=3;i<AccumulatorGroup.Count;i++)OtherGroup.Add(AccumulatorGroup[i]);parent_program.
shipIOHandler.Echo("Warning, more than 3 viable thruster groups found! removing extra ones");}if(AccumulatorGroup.Count<3)
parent_program.shipIOHandler.Error(
"Only two viable thruster groups found!\nPlease rotate the ship slightly and recompile, that could fix this.");var t1=AccumulatorGroup[0];t1.finalThrusterGroups[0]=AccumulatorGroup[0];t1.finalThrusterGroups[1]=AccumulatorGroup[1]
;t1.finalThrusterGroups[2]=AccumulatorGroup[2];t1.finalThrusterGroups[3]=OtherGroup[0];t1.finalThrusterGroups[4]=
OtherGroup[1];t1.finalThrusterGroups[5]=OtherGroup[2];return t1;}public void GatherBasicData(){if(!firstTime&&!parent_program.
scriptEnabled)parent_program.shipIOHandler.Echo("RE-INITIALIZED\nSome change was detected\nso I have re-checked ship data, "+
parent_program.your_title+".");cockpit=FindCockpit();if(cockpit!=null){var Masses=cockpit.CalculateShipMass();shipMass=Masses.
PhysicalMass;previousShipMass=shipMass;}else{parent_program.shipIOHandler.Error(
"The ship systems analyzer couldn't find some sort of cockpit or remote control.\nPlease check you have one of these, "+parent_program.your_title+".");}if(parent_program.Me.CubeGrid.GridSize==0.5)isLargeShip=false;else isLargeShip=true;
thrusters=FindThrusters();gyros=FindGyros();var antenna_result="";if(parent_program.enable_antenna_function)antenna_result=
parent_program.antennaHandler.CheckAntenna();if(!parent_program.errorState)if(firstTime){parent_program.shipIOHandler.Echo(
"Waiting for orders, "+parent_program.your_title+".\n");if(!parent_program.extra_info){parent_program.shipIOHandler.Echo(
"Ready for Waypoints.\n");}if(antenna_result!="")parent_program.shipIOHandler.Echo(antenna_result);if(parent_program.extra_info){if(shipMass!=0)
parent_program.shipIOHandler.Echo("Mass: "+shipMass);parent_program.shipIOHandler.Echo("Thruster count: "+thrusters.Count);
parent_program.shipIOHandler.Echo("Gyro count: "+gyros.Count);parent_program.shipIOHandler.Echo("Main control: "+cockpit.CustomName);
parent_program.shipIOHandler.Echo("Is large ship: "+isLargeShip);parent_program.shipIOHandler.OutputHomeLocations();}}
Populate6ThrusterGroups();parent_program.shipIOHandler.EchoFinish();firstTime=false;}public bool blockIsOnMyGrid(IMyTerminalBlock block){return
block.CubeGrid.EntityId==parent_program.Me.CubeGrid.EntityId;}public static double GetRadiusOfConnector(IMyShipConnector con)
{if(con.CubeGrid.GridSize==0.5)return con.CubeGrid.GridSize;return con.CubeGrid.GridSize*0.5;}List<IMyThrust>
FindThrusters(){var o_thrusters=new List<IMyThrust>();foreach(var block in parent_program.blocks)if(block is IMyThrust&&block.
IsWorking&&blockIsOnMyGrid(block))o_thrusters.Add((IMyThrust)block);else if(block is IMyThrust){if(!((IMyThrust)block).Enabled&&
blockIsOnMyGrid(block)){off_thrusters_count+=1;}}return o_thrusters;}List<IMyGyro>FindGyros(){var o_gyros=new List<IMyGyro>();foreach(
var block in parent_program.blocks)if(block is IMyGyro&&block.IsWorking&&blockIsOnMyGrid(block))o_gyros.Add((IMyGyro)block)
;return o_gyros;}public IMyShipConnector FindMyConnectedConnector(){IMyShipConnector output=null;var Connectors=new List<
IMyShipConnector>();parent_program.GridTerminalSystem.GetBlocksOfType(Connectors);var found_connected_connector=false;var
found_connectable_connector=false;foreach(var connector in Connectors)if((cockpit.CubeGrid.ToString()==connector.CubeGrid.ToString()&&!connector.
CustomName.ToLower().Contains("[recall dock]")&&!parent_program.allow_connector_on_seperate_grid)||(parent_program.
allow_connector_on_seperate_grid&&connector.CustomName.ToLower().Contains("[dock]"))){if(connector.Status==MyShipConnectorStatus.Connected){if(!
found_connected_connector){found_connected_connector=true;output=connector;}}else if(connector.Status==MyShipConnectorStatus.Connectable){if(
found_connected_connector==false)if(!found_connectable_connector){found_connectable_connector=true;output=connector;}}}return output;}public void
CheckForMassChange(){var Masses=cockpit.CalculateShipMass();shipMass=Masses.PhysicalMass;if(previousShipMass!=shipMass)GatherBasicData();
previousShipMass=shipMass;}IMyShipController FindCockpit(){var cockpits=new List<IMyShipController>();IMyShipController foundCockpit=
null;var foundMainCockpit=false;foreach(var block in parent_program.blocks)if(block is IMyShipController&&blockIsOnMyGrid(
block)){if(foundCockpit==null)foundCockpit=(IMyShipController)block;if(block is IMyCockpit){var c_cockpit=(IMyCockpit)block;
if(foundMainCockpit==false)foundCockpit=(IMyShipController)block;if(c_cockpit.IsMainCockpit){foundMainCockpit=true;
foundCockpit=(IMyShipController)block;}}if(block is IMyRemoteControl&&foundMainCockpit==false)foundCockpit=(IMyShipController)block;
if(block is IMyRemoteControl&&remote_control==null)remote_control=(IMyRemoteControl)block;}return foundCockpit;}public
void Populate6ThrusterGroups(){thrusterGroups=new Dictionary<Base6Directions.Direction,ThrusterGroup>();ForwardThrust=new
ThrusterGroup(parent_program,Base6Directions.Direction.Forward,cockpit);UpThrust=new ThrusterGroup(parent_program,Base6Directions.
Direction.Up,cockpit);LeftThrust=new ThrusterGroup(parent_program,Base6Directions.Direction.Left,cockpit);BackwardThrust=new
ThrusterGroup(parent_program,Base6Directions.Direction.Backward,cockpit);DownThrust=new ThrusterGroup(parent_program,Base6Directions.
Direction.Down,cockpit);RightThrust=new ThrusterGroup(parent_program,Base6Directions.Direction.Right,cockpit);thrusterGroups.Add(
Base6Directions.Direction.Forward,ForwardThrust);thrusterGroups.Add(Base6Directions.Direction.Up,UpThrust);thrusterGroups.Add(
Base6Directions.Direction.Left,LeftThrust);thrusterGroups.Add(Base6Directions.Direction.Backward,BackwardThrust);thrusterGroups.Add(
Base6Directions.Direction.Down,DownThrust);thrusterGroups.Add(Base6Directions.Direction.Right,RightThrust);Vector3D thrusterDirection;
double forwardDot=0;double upDot=0;double leftDot=0;var unusedDirections=new List<Base6Directions.Direction>();
unusedDirections.Add(Base6Directions.Direction.Forward);unusedDirections.Add(Base6Directions.Direction.Up);unusedDirections.Add(
Base6Directions.Direction.Left);unusedDirections.Add(Base6Directions.Direction.Backward);unusedDirections.Add(Base6Directions.Direction
.Down);unusedDirections.Add(Base6Directions.Direction.Right);foreach(var thisThruster in thrusters)if(thisThruster.
IsWorking&&blockIsOnMyGrid(thisThruster)){thrusterDirection=-thisThruster.WorldMatrix.Forward;forwardDot=Vector3D.Dot(
thrusterDirection,cockpit.WorldMatrix.Forward);upDot=Vector3D.Dot(thrusterDirection,cockpit.WorldMatrix.Up);leftDot=Vector3D.Dot(
thrusterDirection,cockpit.WorldMatrix.Left);var foundDirection=Base6Directions.Direction.Forward;var unset=true;if(forwardDot>=0.97){
foundDirection=Base6Directions.Direction.Forward;unset=false;}else if(leftDot>=0.97){foundDirection=Base6Directions.Direction.Left;
unset=false;}else if(upDot>=0.97){foundDirection=Base6Directions.Direction.Up;unset=false;}else if(forwardDot<=-0.97){
foundDirection=Base6Directions.Direction.Backward;unset=false;}else if(leftDot<=-0.97){foundDirection=Base6Directions.Direction.Right;
unset=false;}else if(upDot<=-0.97){foundDirection=Base6Directions.Direction.Down;unset=false;}if(!unset){thrusterGroups[
foundDirection].AddThruster(thisThruster);if(unusedDirections.Contains(foundDirection))unusedDirections.Remove(foundDirection);}}
string err_string="";if(unusedDirections.Count==6){err_string="Sorry "+parent_program.your_title+
", I couldn't seem to find any thrusters on your ship.";if(off_thrusters_count>0){err_string+="\nI have detected that some thrusters\nare disabled. Could this be the problem?"
;}parent_program.shipIOHandler.Error(err_string);}else if(unusedDirections.Count==1){err_string="Sorry "+parent_program.
your_title+", it's required that all 6 directions have at least one thruster.\nThe missing direction might be "+unusedDirections[0
]+".";if(off_thrusters_count>0){err_string+=
"\nI have detected that some thrusters\nare disabled. Could this be the problem?";}parent_program.shipIOHandler.Error(err_string);}else if(unusedDirections.Count>1){var total_string="";foreach(var di
in unusedDirections)total_string+=di+"-";err_string="Sorry "+parent_program.your_title+
", it's required that all 6 directions have at least one thruster.\nIt seems the missing directions might be: "+total_string;if(off_thrusters_count>0){err_string+=
"\nI have detected that some thrusters\nare disabled. Could this be the problem?";}parent_program.shipIOHandler.Error(err_string);}}public void UpdateThrusterGroupsWorldDirections(){foreach(var entry
in thrusterGroups)entry.Value.UpdateWorldDirection();}void Echo(object inp){parent_program.shipIOHandler.Echo(inp);}void
EchoFinish(bool OnlyInProgrammingBlock=false){parent_program.shipIOHandler.EchoFinish(OnlyInProgrammingBlock);}void Error(string
str){parent_program.shipIOHandler.Error(str);}}public class ThrusterGroup{private readonly Program parent_program;public
double[]ANS;public IMyTerminalBlock directionReferenceBlock;public int directionSign=1;public ThrusterGroup[]
finalThrusterGroups;public Vector3D finalThrustForces;public double lambdaResult;public Base6Directions.Direction LocalThrustDirection;
public double[,]matrixM;public double MaxThrust;public List<IMyThrust>thrusters;public Vector3D WorldThrustDirection;public
ThrusterGroup(Program _parent_program,Base6Directions.Direction direction,IMyTerminalBlock _directionReferenceBlock){parent_program=
_parent_program;directionReferenceBlock=_directionReferenceBlock;thrusters=new List<IMyThrust>();MaxThrust=0;LocalThrustDirection=
direction;UpdateWorldDirection();ANS=new double[3];finalThrustForces=new Vector3D();finalThrusterGroups=new ThrusterGroup[6];
matrixM=new double[3,3];if(LocalThrustDirection==Base6Directions.Direction.Down||LocalThrustDirection==Base6Directions.
Direction.Right||LocalThrustDirection==Base6Directions.Direction.Backward)directionSign=-1;else directionSign=1;}public void
AddThruster(IMyThrust thruster){thrusters.Add(thruster);MaxThrust+=thruster.MaxEffectiveThrust;}public void UpdateWorldDirection(){
WorldThrustDirection=directionReferenceBlock.WorldMatrix.GetDirectionVector(LocalThrustDirection);}}public class ShipSystemsController{
private readonly Program parent_program;public ShipSystemsController(Program _parent_program){parent_program=_parent_program;}
public void ApplyGyroOverride(double pitch_speed,double yaw_speed,double roll_speed,List<IMyGyro>gyro_list,MatrixD
b_WorldMatrix){var rotationVec=new Vector3D(-pitch_speed,yaw_speed,roll_speed);var relativeRotationVec=Vector3D.TransformNormal(
rotationVec,b_WorldMatrix);var hasDetected=false;foreach(var thisGyro in gyro_list)if(thisGyro.IsWorking){var gyroMatrix=thisGyro.
WorldMatrix;var transformedRotationVec=Vector3D.TransformNormal(relativeRotationVec,Matrix.Transpose(gyroMatrix));thisGyro.Pitch=(
float)transformedRotationVec.X;thisGyro.Yaw=(float)transformedRotationVec.Y;thisGyro.Roll=(float)transformedRotationVec.Z;
thisGyro.GyroOverride=true;}else if(!hasDetected){parent_program.systemsAnalyzer.basicDataGatherRequired=true;parent_program.
shipIOHandler.Echo("Warning:\nGyro damage detected, recomputing.");hasDetected=true;}}public void SetThrusterForces(ThrusterGroup
thrusterGroup,double thrustToApply){var thrustProportion=thrustToApply/thrusterGroup.MaxThrust;var hasDetected=false;foreach(var
thisThruster in thrusterGroup.thrusters)if(thisThruster.IsWorking){thisThruster.ThrustOverride=(float)(thisThruster.MaxThrust*
thrustProportion);}else if(!hasDetected){parent_program.systemsAnalyzer.basicDataGatherRequired=true;parent_program.shipIOHandler.Echo(
"Warning:\nThruster damage detected, recomputing.");hasDetected=true;}}}public class Waypoint{public Vector3D forward;public double maximumAcceleration=5;public double
PercentageOfMaxAcceleration=1;public double waypoint_completion_accuracy=5;public double top_speed=1;public Vector3D position;public double
required_accuracy=0.1;public bool RequireRotation=true;public bool WaypointIsLocal=false;public Vector3D auxilleryDirection;public
Waypoint(Vector3D _pos,Vector3D _forward,Vector3D _auxilleryDirection){position=_pos;forward=_forward;auxilleryDirection=
_auxilleryDirection;}}
}
internal class ConfigurationBuilder{public const string DefaultYourTitle="Captain";public const bool
DefaultRotateSmallShipOnConnector=true;public const bool DefaultRotateLargeShipOnConnector=false;public const bool DefaultRotateOnApproach=false;public
const bool DefaultExtraSoftLandingMode=false;public const int DefaultUndockingDistance=300;private const string
YourTitleKeyName="Your title";private const string RotateSmallShipOnConnectorKeyName="Rotate small ship on connector";private const
string RotateLargeShipOnConnectorKeyName="Rotate large ship on connector";private const string RotateOnApproachKeyName=
"Rotate on approach";private const string ExtraSoftLandingModeKeyName="Extra soft landing mode";private const string
UndockingDistanceKeyName="Undocking distance";private readonly string _sectionName;private readonly MyIni _ini;public ConfigurationBuilder(
string sectionName){_sectionName=sectionName;_ini=new MyIni();}public string YourTitle{get{return _ini.Get(_sectionName,
YourTitleKeyName).ToString(DefaultYourTitle);}set{_ini.Set(_sectionName,YourTitleKeyName,value);}}public bool RotateSmallShipOnConnector
{get{return _ini.Get(_sectionName,RotateSmallShipOnConnectorKeyName).ToBoolean(DefaultRotateSmallShipOnConnector);}set{
_ini.Set(_sectionName,RotateSmallShipOnConnectorKeyName,value);}}public bool RotateLargeShipOnConnector{get{return _ini.Get(
_sectionName,RotateLargeShipOnConnectorKeyName).ToBoolean(DefaultRotateLargeShipOnConnector);}set{_ini.Set(_sectionName,
RotateLargeShipOnConnectorKeyName,value);}}public bool RotateOnApproach{get{return _ini.Get(_sectionName,RotateOnApproachKeyName).ToBoolean(
DefaultRotateOnApproach);}set{_ini.Set(_sectionName,RotateOnApproachKeyName,value);}}public bool ExtraSoftLandingMode{get{return _ini.Get(
_sectionName,ExtraSoftLandingModeKeyName).ToBoolean(DefaultExtraSoftLandingMode);}set{_ini.Set(_sectionName,
ExtraSoftLandingModeKeyName,value);}}public int UndockingDistance{get{return _ini.Get(_sectionName,UndockingDistanceKeyName).ToInt32(
DefaultUndockingDistance);}set{_ini.Set(_sectionName,UndockingDistanceKeyName,value);}}public ISet<string>GridNames{get{List<string>result=new
List<string>();_ini.GetSections(result);return new HashSet<string>(result.Where(x=>x!=_sectionName));}}public bool TryLoad(
string data,out MyIniParseResult result){return _ini.TryParse(data,out result);}public DockConfigurationBuilder Dock(string
gridName){return new DockConfigurationBuilder(gridName,_ini);}public void DeleteDock(string gridName){_ini.DeleteSection(
gridName);}public override string ToString(){YourTitle=YourTitle;RotateSmallShipOnConnector=RotateSmallShipOnConnector;
RotateLargeShipOnConnector=RotateLargeShipOnConnector;RotateOnApproach=RotateOnApproach;ExtraSoftLandingMode=ExtraSoftLandingMode;
UndockingDistance=UndockingDistance;return _ini.ToString();}}internal class DockConfigurationBuilder{private const string
MyConnectorNameKey="My connector name";private const string ConnectorNameKey="Connector name";private const string ConnectorIDKey=
"Connector ID";private const string ConnectorPositionKey="Connector position";private const string ConnectorForwardKey=
"Connector forward";private const string ConnectorUpGlobalKey="Connector up global";private const string ConnectorUpLocalKey=
"Connector up local";private const string ConnectorLeftKey="Connector left";private const string GridIDKey="Grid ID";private const string
ConnectorSizeKey="Connector size";private const string LandingSequencesKey="Landing sequences";private const string ArgumentsKey=
"Arguments";private readonly MyIni _ini;public DockConfigurationBuilder(string gridName,MyIni ini){_ini=ini;GridName=gridName;}
public string GridName{get;private set;}public string MyConnectorName{get{return _ini.Get(GridName,MyConnectorNameKey).
ToString();}set{_ini.Set(GridName,MyConnectorNameKey,value);}}public string ConnectorName{get{return _ini.Get(GridName,
ConnectorNameKey).ToString();}set{_ini.Set(GridName,ConnectorNameKey,value);}}public long ConnectorID{get{return _ini.Get(GridName,
ConnectorIDKey).ToInt64();}set{_ini.Set(GridName,ConnectorIDKey,value);}}public Vector3D ConnectorPosition{get{Vector3D result;
Vector3D.TryParse(_ini.Get(GridName,ConnectorPositionKey).ToString(),out result);return result;}set{_ini.Set(GridName,
ConnectorPositionKey,value.ToString());}}public Vector3D ConnectorForward{get{Vector3D result;Vector3D.TryParse(_ini.Get(GridName,
ConnectorForwardKey).ToString(),out result);return result;}set{_ini.Set(GridName,ConnectorForwardKey,value.ToString());}}public Vector3D
ConnectorUpGlobal{get{Vector3D result;Vector3D.TryParse(_ini.Get(GridName,ConnectorUpGlobalKey).ToString(),out result);return result;}set
{_ini.Set(GridName,ConnectorUpGlobalKey,value.ToString());}}public Vector3D ConnectorUpLocal{get{Vector3D result;Vector3D
.TryParse(_ini.Get(GridName,ConnectorUpLocalKey).ToString(),out result);return result;}set{_ini.Set(GridName,
ConnectorUpLocalKey,value.ToString());}}public Vector3D ConnectorLeft{get{Vector3D result;Vector3D.TryParse(_ini.Get(GridName,
ConnectorLeftKey).ToString(),out result);return result;}set{_ini.Set(GridName,ConnectorLeftKey,value.ToString());}}public long GridID{
get{return _ini.Get(GridName,GridIDKey).ToInt64();}set{_ini.Set(GridName,GridIDKey,value);}}public double ConnectorSize{get
{return _ini.Get(GridName,ConnectorSizeKey).ToDouble();}set{_ini.Set(GridName,ConnectorSizeKey,value);}}public string
LandingSequences{get{return _ini.Get(GridName,LandingSequencesKey).ToString();}set{_ini.Set(GridName,LandingSequencesKey,value);}}public
IEnumerable<string>Arguments{get{int argumentsCount=_ini.Get(GridName,ArgumentsKey).ToInt32();for(int argumentNumber=1;
argumentNumber<=argumentsCount;argumentNumber++){yield return _ini.Get(GridName,ArgumentKey(argumentNumber)).ToString();}}set{int
argumentNumber=1;foreach(string argument in value){_ini.Set(GridName,ArgumentKey(argumentNumber),argument);}_ini.Set(GridName,
ArgumentsKey,argumentNumber);}}private static string ArgumentKey(int number){return$"Argument{number}";}}internal class Blinker:
IAnimation{public Blinker(string whenVisible,string whenNotVisible=null){WhenVisible=whenVisible;WhenNotVisible=whenNotVisible??
new string(' ',whenVisible.Length);}public string CurrentFrame{get{return IsVisible?WhenVisible:WhenNotVisible;}}public
string WhenVisible{get;set;}public string WhenNotVisible{get;set;}bool IsVisible{get;set;}public void NextFrame(){IsVisible=!
IsVisible;}}public class ConsoleBlock:IMyTextSurfaceProvider{IMyTerminalBlock _block;IMyTextSurfaceProvider _provider;public
ConsoleBlock(IMyTerminalBlock block,ConsoleConfigurationBuilder configuration){_block=block;_provider=(IMyTextSurfaceProvider)block;
Configuration=configuration;}public string CustomName{get{return _block.CustomName;}}public ConsoleConfigurationBuilder Configuration
{get;private set;}public bool UseGenericLcd{get{return _provider.UseGenericLcd;}}public int SurfaceCount{get{return
_provider.SurfaceCount;}}public IMyTextSurface GetSurface(int index){return _provider.GetSurface(index);}public static
IEnumerable<ConsoleBlock>Find(IEnumerable<IMyTerminalBlock>blocks,string scriptID){foreach(IMyTerminalBlock block in blocks.Where(x
=>x is IMyTextSurfaceProvider)){ConsoleConfigurationBuilder configuration=new ConsoleConfigurationBuilder();if(
configuration.TryLoad(block.CustomData)&&configuration.Displays.Any(display=>display.ScriptID==scriptID)){yield return new
ConsoleBlock(block,configuration);}}}}public class ConsoleConfigurationBuilder{private readonly string _defaultScriptID;public
ConsoleConfigurationBuilder(string defaultScriptID="AutoLCD"){Displays=new List<DisplayConfigurationBuilder>();_defaultScriptID=defaultScriptID;}
public List<DisplayConfigurationBuilder>Displays{get;set;}public bool TryLoad(string data){List<string>sections=data.Split(new
char[]{'@'},StringSplitOptions.RemoveEmptyEntries).Select(section=>section.Trim()).ToList();List<DisplayConfigurationBuilder
>displays=new List<DisplayConfigurationBuilder>();foreach(string section in sections){IEnumerable<string>lines=section.
Split(new char[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).Select(line=>line.Trim());string firstLine=lines.
FirstOrDefault();if(!string.IsNullOrWhiteSpace(firstLine)){List<string>segments=firstLine.Split(new char[]{' '},StringSplitOptions.
RemoveEmptyEntries).Select(segment=>segment.Trim()).ToList();int index;if(segments.Count>=2&&int.TryParse(segments.First(),out index)){
displays.Add(new DisplayConfigurationBuilder(){Index=index,ScriptID=string.Join(" ",segments.Skip(1)),Lines=lines.Skip(1).ToList
()});}else{if(sections.Count==1){displays.Add(new DisplayConfigurationBuilder(){Index=0,ScriptID=_defaultScriptID,Lines=
data.Split(new char[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).Select(line=>line.Trim()).ToList()});}}}}Displays=
displays;return true;}}internal class Cursor:Blinker{public Cursor():base("_"){}}internal class Display:ITextSurfaceSettings{
public const string DefaultPrompt="> ";private readonly IMyTextSurface _surface;private readonly string _subtypeId;private
readonly Vector2 _surfaceSize;public Display(string name,IMyTextSurface surface,string subtypeId){Name=name;_surface=surface;
_subtypeId=subtypeId;switch(_subtypeId){case"LargeBlockCockpitIndustrial":case"LargeBlockCockpitSeat":case
"LargeBlockInsetButtonPanel":case"SmallProgrammableBlock":case"SmallProgrammableBlockReskin":float scale=512/_surface.SurfaceSize.X;_surfaceSize=new
Vector2(_surface.SurfaceSize.X*scale,_surface.SurfaceSize.Y*scale);break;default:_surfaceSize=_surface.SurfaceSize;break;}Lines
=new List<string>();IsLastLineFinished=true;Cursor=new Cursor();MeasureSize();}public string Name{get;}public string Font
{get{return _surface.Font;}set{_surface.Font=value;MeasureSize();}}public float FontSize{get{return _surface.FontSize;}
set{_surface.FontSize=value;MeasureSize();}}public float TextPadding{get{return _surface.TextPadding;}set{_surface.
TextPadding=value;MeasureSize();}}public ContentType ContentType{get{return _surface.ContentType;}set{_surface.ContentType=value;}}
public Color FontColor{get{return _surface.FontColor;}set{_surface.FontColor=value;}}public Color BackgroundColor{get{return
_surface.BackgroundColor;}set{_surface.BackgroundColor=value;}}public TextAlignment Alignment{get{return _surface.Alignment;}set
{_surface.Alignment=value;}}public int MaxColumnCount{get;private set;}public int MaxLineCount{get;private set;}public
Blinker Cursor{get;set;}List<string>Lines{get;set;}bool IsLastLineFinished{get;set;}public void Clear(){Lines.Clear();}public
void Write(string text){AddLines(text.Split(new string[]{"\r\n"},StringSplitOptions.None));IsLastLineFinished=false;
WriteLines();}public void WriteLine(string text){AddLines(text.Split(new string[]{"\r\n"},StringSplitOptions.None));
IsLastLineFinished=true;WriteLines();}public void WriteCommandLine(string text,string prompt=DefaultPrompt,bool isNewLineRequired=false){
string line=string.Concat(prompt,text,text.Length>0?string.Empty:Cursor.CurrentFrame);string whenVisible=string.Concat(prompt,
Cursor.WhenVisible);string whenNotVisible=string.Concat(prompt,Cursor.WhenNotVisible);int lastLineIndex=Lines.Count-1;if(!
isNewLineRequired&&(lastLineIndex>=0)&&((Lines[lastLineIndex]==whenVisible)||(Lines[lastLineIndex]==whenNotVisible))){Lines[lastLineIndex
]=line;WriteLines();}else{WriteLine(line);}}public void RemoveCommandLine(string prompt=DefaultPrompt){string whenVisible
=string.Concat(prompt,Cursor.WhenVisible);string whenNotVisible=string.Concat(prompt,Cursor.WhenNotVisible);int
lastLineIndex=Lines.Count-1;if((lastLineIndex>=0)&&((Lines[lastLineIndex]==whenVisible)||(Lines[lastLineIndex]==whenNotVisible))){
Lines.RemoveAt(lastLineIndex);WriteLines();}}public void Mirror(Action<string>echo,int maxLineCount=12){foreach(string line
in GetLines(maxLineCount)){echo(line.Replace("[","[[").Replace("]","]]"));}}public string GetText(){return _surface.
GetText();}public IEnumerable<string>GetLines(int maxCount){int skip=Math.Max(0,Lines.Count-maxCount);return Lines.Skip(skip).
Take(maxCount);}public void AddImage(string id){_surface.PreserveAspectRatio=true;_surface.ChangeInterval=1;_surface.
AddImageToSelection(id,true);}public void RemoveImage(string id){_surface.RemoveImageFromSelection(id,true);}public TextSurfaceSettings
GetRecommendedSettings(){TextSurfaceSettings result=new TextSurfaceSettings(){ContentType=ContentType.TEXT_AND_IMAGE,Font="DEBUG",FontSize=
0.8f,FontColor=new Color(192,192,192),BackgroundColor=new Color(0,0,0),Alignment=TextAlignment.LEFT,TextPadding=0.2f};switch
(_subtypeId){case"LargeBlockCockpit":case"LargeBlockCockpitIndustrial":case"LargeBlockInsetButtonPanel":case
"LargeProgrammableBlock":case"OpenCockpitLarge":result.FontSize=0.6f;break;case"LargeBlockCockpitSeat":result.FontSize=0.75f;break;};return
result;}public void MeasureSize(){Vector2 letterSize=_surface.MeasureStringInPixels(new StringBuilder("W"),_surface.Font,
_surface.FontSize);Vector2 padding=new Vector2(_surfaceSize.X*_surface.TextPadding/100,_surfaceSize.Y*_surface.TextPadding/100);
MaxColumnCount=(int)Math.Floor((_surfaceSize.X-padding.X)/letterSize.X);MaxLineCount=(int)Math.Floor((_surfaceSize.Y-padding.Y)/
letterSize.Y);}void AddLines(IEnumerable<string>newLines){if(!newLines.Any()){return;}if(IsLastLineFinished){Lines.AddRange(
newLines);}else{if(Lines.Count==0){Lines.Add(string.Empty);}Lines[Lines.Count-1]=string.Concat(Lines.Last(),newLines.First());
Lines.AddRange(newLines.Skip(1));}int skip=Math.Max(0,Lines.Count-MaxLineCount);Lines=Lines.Skip(skip).Take(MaxLineCount).
ToList();}void WriteLines(){_surface.WriteText(string.Join("\r\n",Lines),false);}public static IEnumerable<IMyTextSurface>Find
(IEnumerable<IMyTerminalBlock>blocks,string tag,string scriptID){foreach(ConsoleBlock consoleBlock in ConsoleBlock.Find(
blocks.Where(x=>x.CustomName.Contains(tag)),scriptID)){foreach(DisplayConfigurationBuilder display in consoleBlock.
Configuration.Displays.Where(x=>x.ScriptID==scriptID)){yield return consoleBlock.GetSurface(display.Index);}}}}public class
DisplayConfigurationBuilder{public int Index{get;set;}public string ScriptID{get;set;}public List<string>Lines{get;set;}}internal interface
IAnimation{string CurrentFrame{get;}void NextFrame();}internal interface ITextSurfaceSettings{ContentType ContentType{get;set;}
string Font{get;set;}float FontSize{get;set;}Color FontColor{get;set;}Color BackgroundColor{get;set;}TextAlignment Alignment{
get;set;}float TextPadding{get;set;}}internal class TextSurfaceSettings:ITextSurfaceSettings{public ContentType ContentType
{get;set;}public string Font{get;set;}public float FontSize{get;set;}public Color FontColor{get;set;}public Color
BackgroundColor{get;set;}public TextAlignment Alignment{get;set;}public float TextPadding{get;set;}public void CopyTo(
ITextSurfaceSettings other){other.ContentType=ContentType;other.Font=Font;other.FontSize=FontSize;other.FontColor=FontColor;other.
BackgroundColor=BackgroundColor;other.Alignment=Alignment;other.TextPadding=TextPadding;}