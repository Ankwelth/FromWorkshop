/*
 * R e a d m e
 * -----------
 * 
 * Version 1.3.2
 * 
 * New Features:
 * - Easy Waypoint Batch Import from Text Panels
 * - Cycle Marker Type of Active Waypoint
 * - Duplicate Waypoint Entries now update location of Waypoint
 * - Custom Data Reading/Writing optimization for Star Maps
 */
const int SHIP_RED=127;const int SHIP_GREEN=127;const int SHIP_BLUE=192;const int SHIP_SCALE=24;const float DIAMETER_MIN
=6;const int HASH_LIMIT=125;const float JUMP_RATIO=2;const int MARKER_WIDTH=8;const int FOCAL_MOD=250;const int DV_RADIUS
=262144;const int DV_FOCAL=256;const int DV_ALTITUDE=-15;const int BRIGHTNESS_LIMIT=4;const int BAR_HEIGHT=20;const int
TOP_MARGIN=8;const int SIDE_MARGIN=15;const int MAX_VALUE=1073741824;const string SLASHES=
" //////////////////////////////////////////////////////////////";const string PROGRAM_HEAD="Map Settings";string[]_cycleSpinner={"--"," / "," | "," \\ "};string _mapTag;string
_previousCommand;bool _lightOn;static bool _planets;bool _planetToLog;bool _slowMode=false;const int CYCLE_LENGTH=5;static int
_cycleStep;static int _cycleOffset;int _sortCounter=0;string _activePlanet="";static string _clipboard="";Vector3 _myPos;List<
IMyTerminalBlock>_mapBlocks;static List<string>_messages;const int MESSAGE_LIMIT=20;Planet _nearestPlanet;IMyTerminalBlock _refBlock;
Program(){_cycleOffset=Math.Abs((int)Me.CubeGrid.EntityId%CYCLE_LENGTH);_planetToLog=false;Build();_previousCommand=
"NEWLY LOADED";}void Save(){}void Main(string argument){_planets=_planetList.Count>0;_myPos=_refBlock.GetPosition();Echo(
"////// PLANET MAP 3D ////// "+_cycleSpinner[_cycleStep%_cycleSpinner.Length]);Echo("Cmd: "+_previousCommand+"\n");EchoMessages();DisplayScanData();
Echo("\nMAPS: "+_mapList.Count+"\nMENUS: "+_mapMenus.Count+"\nDATA DISPLAYS: "+_dataDisplays.Count);if(_planets){Echo(
"Planet Count: "+_planetList.Count);}else{Echo("No Planets Logged!");}if(_waypointList.Count>0){Echo("GPS Count: "+_waypointList.Count+
"\n");}else{Echo("No Waypoints Logged!");}ShowMenuData();if(_mapList.Count>0){CycleExecute();ButtonTimer();if(argument!=""){
MainSwitch(argument);}if(_planets){if(_cycleStep==CYCLE_LENGTH||_previousCommand=="NEWLY LOADED"){SortByNearest(_planetList);}
_nearestPlanet=_planetList[0];}DrawMaps();}else{SetGridID("");if(_mapList.Count<1)AddMessage("NO MAP DISPLAY FOUND!\nPlease add tag "+
_mapTag+" to desired block.");}UpdateDisplays();}void Show(List<StarMap>maps,string attribute,int state){if(NoMaps(maps))return
;foreach(StarMap map in maps){switch(attribute){case"GPS":if(state==3){cycleGPSForList(maps);}else{map.GpsState=state;}
break;case"NAMES":map.ShowNames=setState(map.ShowNames,state);break;case"SHIP":map.ShowShip=setState(map.ShowShip,state);
break;case"INFO":map.ShowInfo=setState(map.ShowInfo,state);break;default:AddMessage("INVALID DISPLAY COMMAND");break;}}}void
cycleGPS(StarMap map){map.GpsState++;if(map.GpsState>2)map.GpsState=0;map.Mode=map.GpsStateToMode();map.SetMapKey(GPS_KEY,map.
Mode);}void cycleGPSForList(List<StarMap>maps){if(NoMaps(maps))return;foreach(StarMap map in maps){cycleGPS(map);}}void
DataToLog(){MyIni mapIni=DataToIni(Me);if(_waypointList.Count>0){String waypointData="";foreach(Waypoint waypoint in
_waypointList){waypointData+=WaypointToString(waypoint)+"\n";}mapIni.Set(PROGRAM_HEAD,"Waypoint_List",waypointData);}String
planetData="";if(_planetList.Count>0){foreach(Planet planet in _planetList){planetData+=planet.ToString()+"\n";}}if(_unchartedList
.Count>0){foreach(Planet uncharted in _unchartedList){planetData+=uncharted.ToString()+"\n";}}if(planetData!=""){mapIni.
Set(PROGRAM_HEAD,"Planet_List",planetData);}Me.CustomData=mapIni.ToString();}void ClipboardToLog(string markerType,string
clipboard){string[]waypointData=clipboard.Split(':');if(waypointData.Length<6){return;}Vector3 position=new Vector3(float.Parse(
waypointData[2]),float.Parse(waypointData[3]),float.Parse(waypointData[4]));LogWaypoint(waypointData[1],position,markerType,
waypointData[5]);}string LogToClipboard(string waypointName){Waypoint waypoint=GetWaypoint(waypointName);if(waypoint==null){
AddMessage("No waypoint "+waypointName+" found!");return _messages[_messages.Count-1];}Vector3 location=waypoint.position;string
output="GPS:"+waypoint.name+":"+location.X+":"+location.Y+":"+location.Z+":#FF75C9F1:";return output;}void LogWaypoint(String
waypointName,Vector3 position,String markerType,String waypointColor){if(waypointName==""){AddMessage(
"No Waypoint Name Provided! Please Try Again.");return;}Waypoint waypoint=GetWaypoint(waypointName);bool newWaypoint=waypoint==null;if(newWaypoint){waypoint=new
Waypoint();waypoint.name=waypointName;}else{AddMessage("Waypoint "+waypointName+" updated.");}waypoint.position=position;
waypoint.marker=markerType;waypoint.isActive=true;waypoint.color=waypointColor;if(newWaypoint)_waypointList.Add(waypoint);
DataToLog();foreach(StarMap map in _mapList){UpdateMap(map);}}void SetWaypointState(String waypointName,int state){Waypoint
waypoint=GetWaypoint(waypointName);if(waypoint==null){WaypointError(waypointName);return;}switch(state){case 0:waypoint.isActive
=false;break;case 1:waypoint.isActive=true;break;case 2:waypoint.isActive=!waypoint.isActive;break;case 3:_waypointList.
Remove(waypoint);AddMessage("Waypoint deleted: "+waypointName);break;default:AddMessage("Invalid waypoint state int!");break;}
DataToLog();}void PlotJumpPoint(string planetName){Planet planet=GetPlanet(planetName);{if(planet==null){PlanetError(planetName);
}}int designation=1;string name=planet.name+" Orbit ";Waypoint jumpPoint=GetWaypoint(name+designation);while(jumpPoint!=
null){designation++;jumpPoint=GetWaypoint(name+designation);}Vector3 position=planet.position+(_myPos-planet.position)/
Vector3.Distance(_myPos,planet.position)*planet.radius*JUMP_RATIO;LogWaypoint(name+designation,position,"WAYPOINT","WHITE");}
void ProjectPoint(string marker,string arg){string[]args=arg.Split(' ');if(args.Length<2){AddMessage(
"INSUFFICIENT ARGUMENT!\nPlease include arguments <DISTANCE(in meters)> <WAYPOINT NAME>");return;}int distance;if(int.TryParse(args[0],out distance)){string name="";for(int i=1;i<args.Length;i++){name+=args[i
]+" ";}Vector3 location=_myPos+_refBlock.WorldMatrix.Forward*distance;LogWaypoint(name.Trim(),location,marker,"WHITE");
return;}AddMessage("DISTANCE ARGEMENT FAILED!\nPlease include Distance in meters. Do not include unit.");}void PlanetError(
string name){AddMessage("No planet "+name+" found!");}void WaypointError(string name){AddMessage("No waypoint "+name+" found!"
);}void DeletePlanet(String planetName){Planet alderaan=GetPlanet(planetName);if(alderaan==null){PlanetError(planetName);
return;}_planetList.Remove(alderaan);DataToLog();AddMessage("PLANET DELETED: "+planetName+"\n\nDon't be too proud of this TECHNOLOGICAL TERROR you have constructed. The ability to DESTROY a PLANET is insignificant next to the POWER of the FORCE."
);_planets=_planetList.Count>0;if(_planets)_nearestPlanet=_planetList[0];}void SetPlanetColor(String argument){String[]
args=argument.Split(' ');String planetColor=args[0];if(args.Length<2){AddMessage(
"Insufficient Argument.  COLOR_PLANET requires COLOR and PLANET NAME.");}else{String planetName="";for(int p=1;p<args.Length;p++){planetName+=args[p]+" ";}planetName=planetName.Trim(' ').
ToUpper();Planet planet=GetPlanet(planetName);if(planet!=null){planet.color=planetColor;AddMessage(planetName+
" color changed to "+planetColor);DataToLog();return;}PlanetError(planetName);}}void SetWaypointColor(String argument){String[]args=argument
.Split(' ');String waypointColor=args[0];if(args.Length<2){AddMessage(
"Insufficient Argument.  COLOR_WAYPOINT requires COLOR and WAYPOINT NAME.");}else{String waypointName="";for(int w=1;w<args.Length;w++){waypointName+=args[w]+" ";}waypointName=waypointName.Trim(
' ').ToUpper();Waypoint waypoint=GetWaypoint(waypointName);if(waypoint!=null){waypoint.color=waypointColor;AddMessage(
waypointName+" color changed to "+waypointColor);DataToLog();return;}WaypointError(waypointName);}}void SetWaypointType(string arg,
string waypointName){Waypoint waypoint=GetWaypoint(waypointName);if(waypoint==null){WaypointError(waypointName);return;}
waypoint.marker=arg;DataToLog();}bool onGrid(IMyTerminalBlock mapTerminal){if(!mapTerminal.IsSameConstructAs(Me))return false;
string iniGrid=GetKey(mapTerminal,SHARED,"Grid_ID",Me.CubeGrid.EntityId.ToString());if(iniGrid==_gridID)return true;else
return false;}MyIni DataToIni(IMyTerminalBlock block){MyIni iniOuti=new MyIni();MyIniParseResult result;if(!iniOuti.TryParse(
block.CustomData,out result))throw new Exception(result.ToString());return iniOuti;}Color ColorSwitch(string colorString,bool
isWaypoint){colorString=colorString.ToUpper();if(colorString.StartsWith("#"))return HexToColor(colorString);Color colorOut=new
Color(8,8,8);if(isWaypoint)colorOut=Color.White;switch(colorString){case"RED":colorOut=new Color(32,0,0);break;case"GREEN":
colorOut=new Color(0,32,0);break;case"BLUE":colorOut=new Color(0,0,32);break;case"YELLOW":colorOut=new Color(127,127,26);break;
case"MAGENTA":colorOut=new Color(64,0,64);break;case"PURPLE":colorOut=new Color(24,0,48);break;case"CYAN":colorOut=new Color
(0,32,32);break;case"LIGHTBLUE":colorOut=new Color(32,32,96);break;case"ORANGE":colorOut=new Color(32,16,0);break;case
"TAN":colorOut=new Color(153,100,48);break;case"BROWN":colorOut=new Color(38,25,12);break;case"RUST":colorOut=new Color(64,20
,16);break;case"GRAY":colorOut=new Color(16,16,16);break;case"GREY":colorOut=new Color(16,16,16);break;case"WHITE":
colorOut=new Color(64,64,64);if(isWaypoint)colorOut=Color.White;break;default:colorOut=new Color(8,8,8);break;}return colorOut;}
Color HexToColor(string hexString){if(hexString.Length!=9&&hexString.Length!=7)return Color.White;int i=3;if(hexString.Length
==7)i=1;int r,g,b=0;r=Convert.ToUInt16(hexString.Substring(i,2),16);g=Convert.ToUInt16(hexString.Substring(i+2,2),16);b=
Convert.ToUInt16(hexString.Substring(i+4,2),16);return new Color(r,g,b);}bool setState(bool attribute,int state){switch(state){
case 0:attribute=false;break;case 1:attribute=true;break;case 3:attribute=!attribute;break;}return attribute;}Waypoint
StringToWaypoint(String argument){Waypoint waypoint=new Waypoint();String[]wayPointData=argument.Split(';');if(wayPointData.Length>3){
waypoint.name=wayPointData[0];waypoint.position=StringToVector3(wayPointData[1]);waypoint.marker=wayPointData[2];waypoint.
isActive=wayPointData[3].ToUpper()=="ACTIVE";}if(wayPointData.Length<5){waypoint.color="WHITE";}else{waypoint.color=wayPointData
[4];}return waypoint;}String WaypointToString(Waypoint waypoint){String output=waypoint.name+";"+Vector3ToString(waypoint
.position)+";"+waypoint.marker;String activity="INACTIVE";if(waypoint.isActive){activity="ACTIVE";}output+=";"+activity+
";"+waypoint.color;return output;}static string Vector3ToString(Vector3 vec3){String newData="("+vec3.X+","+vec3.Y+","+vec3
.Z+")";return newData;}static Vector3 StringToVector3(string sVector){try{if(sVector.StartsWith("(")&&sVector.EndsWith(
")")){sVector=sVector.Substring(1,sVector.Length-2);}string[]sArray=sVector.Split(',');Vector3 result=new Vector3(
ParseFloat(sArray[0],0),ParseFloat(sArray[1],0),ParseFloat(sArray[2],0));return result;}catch{return Vector3.Zero;}}string
abbreviateValue(float valueIn){string abbreviatedValue;if(valueIn<=-1000000000||valueIn>=1000000000){valueIn=valueIn/1000000000;
abbreviatedValue=valueIn.ToString("0.0")+"G";}else if(valueIn<=-1000000||valueIn>=1000000){valueIn=valueIn/1000000;abbreviatedValue=
valueIn.ToString("0.0")+"M";}else if(valueIn<=-1000||valueIn>=1000){valueIn=valueIn/1000;abbreviatedValue=valueIn.ToString(
"0.0")+"k";}else{abbreviatedValue=valueIn.ToString("F0");}return abbreviatedValue;}void PlanetSort(List<Planet>planets,
StarMap map){int length=planets.Count;for(int i=0;i<planets.Count-1;i++){for(int p=1;p<length;p++){Planet planetA=planets[p-1];
Planet planetB=planets[p];if(planetA.transformedCoords[map.Number].Z<planetB.transformedCoords[map.Number].Z){planets[p-1]=
planetB;planets[p]=planetA;}}length--;if(length<2){return;}}}Vector3 transformVector(Vector3 vectorIn,StarMap map){double xS=
vectorIn.X-map.Center.X;double yS=vectorIn.Y-map.Center.Y;double zS=vectorIn.Z-map.Center.Z;double r=map.RotationalRadius;double
cosAz=Math.Cos(ToRadians(map.Azimuth));double sinAz=Math.Sin(ToRadians(map.Azimuth));double cosAlt=Math.Cos(ToRadians(map.
Altitude));double sinAlt=Math.Sin(ToRadians(map.Altitude));double xT=cosAz*xS+sinAz*zS;double yT=sinAz*sinAlt*xS+cosAlt*yS-
sinAlt*cosAz*zS;double zT=-sinAz*cosAlt*xS+sinAlt*yS+cosAz*cosAlt*zS+r;Vector3 vectorOut=new Vector3(xT,yT,zT);return
vectorOut;}Vector3 rotateVector(Vector3 vecIn,StarMap map){float x=vecIn.X;float y=vecIn.Y;float z=vecIn.Z;float cosAz=(float)
Math.Cos(ToRadians(map.Azimuth));float sinAz=(float)Math.Sin(ToRadians(map.Azimuth));float cosAlt=(float)Math.Cos(ToRadians(
map.Altitude));float sinAlt=(float)Math.Sin(ToRadians(map.Altitude));float xT=cosAz*x+sinAz*z;float yT=sinAz*sinAlt*x+
cosAlt*y-sinAlt*cosAz*z;float zT=-sinAz*cosAlt*x+sinAlt*y+cosAz*cosAlt*z;Vector3 vecOut=new Vector3(xT,yT,zT);return vecOut;}
static Vector3 rotateMovement(Vector3 vecIn,StarMap map){float x=vecIn.X;float y=vecIn.Y;float z=vecIn.Z;float cosAz=(float)
Math.Cos(ToRadians(-map.Azimuth));float sinAz=(float)Math.Sin(ToRadians(-map.Azimuth));float cosAlt=(float)Math.Cos(
ToRadians(-map.Altitude));float sinAlt=(float)Math.Sin(ToRadians(-map.Altitude));float xT=cosAz*x+sinAz*sinAlt*y+sinAz*cosAlt*z;
float yT=cosAlt*y-sinAlt*z;float zT=-sinAz*x+cosAz*sinAlt*y+cosAz*cosAlt*z;Vector3 vecOut=new Vector3(xT,yT,zT);return vecOut
;}void CycleExecute(){_cycleStep--;int stage=(_cycleStep-_cycleOffset+CYCLE_LENGTH)%CYCLE_LENGTH;switch(stage){case 4:
UpdateDistances();break;case 3:SortGlobalWaypoints();break;case 2:SortPlanetsForMaps();break;case 1:if(_planetToLog){DataToLog();
_planetToLog=false;}break;case 0:UpdatePageData();break;}if(_cycleStep<1){_cycleStep=CYCLE_LENGTH;_lightOn=!_lightOn;}}void
UpdateMap(StarMap map){if(_mapList.Count==0)return;if(_planets){foreach(Planet planet in _planetList){Vector3 newCenter=
transformVector(planet.position,map);if(planet.transformedCoords.Count<_mapList.Count){planet.transformedCoords.Add(newCenter);}else{
planet.transformedCoords[map.Number]=newCenter;}}}if(_waypointList.Count>0){foreach(Waypoint waypoint in _waypointList){
Vector3 newPos=transformVector(waypoint.position,map);if(waypoint.transformedCoords.Count<_mapList.Count){waypoint.
transformedCoords.Add(newPos);}else{waypoint.transformedCoords[map.Number]=newPos;}}}}void activateMap(StarMap map){
IMyTextSurfaceProvider mapBlock=map.Block as IMyTextSurfaceProvider;map.DrawingSurface=mapBlock.GetSurface(map.Index);
PrepareTextSurfaceForSprites(map.DrawingSurface);map.Viewport=new RectangleF((map.DrawingSurface.TextureSize-map.DrawingSurface.SurfaceSize)/2f,map.
DrawingSurface.SurfaceSize);map.Number=_mapList.Count;_mapList.Add(map);UpdateMap(map);}static void AddMessage(string message){
_messages.Add(message);if(_messages.Count>=MESSAGE_LIMIT)_messages.RemoveAt(0);}void EchoMessages(){if(_messages.Count<1)return;
Echo("-- MESSAGES --");for(int i=_messages.Count-1;i>-1;i--)Echo("* "+_messages[i]);}void Build(){_messages=new List<string>
();_cycleStep=CYCLE_LENGTH;_lightOn=true;_planetList=new List<Planet>();_unchartedList=new List<Planet>();_waypointList=
new List<Waypoint>();_mapList=new List<StarMap>();_mapBlocks=new List<IMyTerminalBlock>();_mapMenus=new List<MapMenu>();
_gridID=GetKey(Me,SHARED,"Grid_ID",Me.CubeGrid.EntityId.ToString());AssignRefBlock();AssignMaps();LoadPlanetData();
LoadWaypointData();UpdateMapDataPage();AssignDataDisplays();AssignMenus();SetScanCamera();SetRefreshRate();}void AssignRefBlock(){string
refName=GetKey(Me,PROGRAM_HEAD,"Reference_Name","[Reference]");if(refName==""){AddMessage(
"WARNING: No Reference Block Name Specified!\nMay result in false orientation!");_refBlock=Me as IMyTerminalBlock;}else{List<IMyTerminalBlock>refBlocks=new List<IMyTerminalBlock>();GridTerminalSystem
.SearchBlocksOfName(refName,refBlocks);if(refBlocks.Count>0){_refBlock=refBlocks[0]as IMyTerminalBlock;Echo("Reference: "
+_refBlock.CustomName);}else{AddMessage("WARNING: No Block containing "+refName+
" found.\nMay result in false orientation!");_refBlock=Me as IMyTerminalBlock;}}_myPos=_refBlock.GetPosition();}void AssignMaps(){_mapTag=GetKey(Me,PROGRAM_HEAD,
"MAP_TAG","[MAP]");if(_mapTag==""){Echo("No LCD specified!!!");}else{GridTerminalSystem.SearchBlocksOfName(_mapTag,_mapBlocks);if
(_mapBlocks.Count<1){AddMessage("No screens with tag \""+_mapTag+"\" found!");return;}foreach(IMyTerminalBlock mapBlock
in _mapBlocks){Echo(mapBlock.CustomName);if(onGrid(mapBlock)&&GetSurfaceCount(mapBlock)>0){List<StarMap>maps=
ParametersToMaps(mapBlock);if(maps.Count>0){foreach(StarMap map in maps)activateMap(map);}}}}}void LoadPlanetData(){string planetData=
GetKey(Me,PROGRAM_HEAD,"Planet_List","");string[]mapEntries=planetData.Split('\n');foreach(string planetString in mapEntries){
string newPlanetString=ConvertOldPlanetData(planetString);Planet planet=new Planet(newPlanetString);if(planet.radius>0)
_planetList.Add(planet);}DataToLog();_planets=_planetList.Count>0;}void LoadWaypointData(){string waypointData=GetKey(Me,
PROGRAM_HEAD,"Waypoint_List","");string[]gpsEntries=waypointData.Split('\n');foreach(string waypointString in gpsEntries){if(
waypointString.Contains(";")){Waypoint waypoint=StringToWaypoint(waypointString);_waypointList.Add(waypoint);}}}void SetRefreshRate(){
_slowMode=ParseBool(GetKey(Me,PROGRAM_HEAD,"Slow_Mode","false"));if(_slowMode)Runtime.UpdateFrequency=UpdateFrequency.Update100;
else Runtime.UpdateFrequency=UpdateFrequency.Update10;}void ButtonPress(string buttonIndex,string menuIndex){MapMenu menu=
GetMenu(menuIndex);if(menu==null){AddMessage("Invalid Button Call: "+buttonIndex+","+menuIndex);return;}StarMap map=GetMap(menu
.CurrentMapIndex);if(map==null){AddMessage("Invalid Index for Menu "+menu.IDNumber+"!\n- Index:"+menu.CurrentMapIndex);
return;}switch(buttonIndex){case"1":Action1(menu,map);break;case"2":Action2(menu,map);break;case"3":Action3(menu,map);break;
case"4":Action4(menu,map);break;case"5":Action5(menu,map);break;case"6":Action6(menu,map);break;case"7":Action7(menu,map);
break;default:AddMessage("No Such Button \""+buttonIndex+"\"!");break;}}void Action1(MapMenu menu,StarMap map){menu.
PressButton(1);switch(menu.CurrentPage){case 1:CyclePlanets(map,false);break;case 2:map.Zoom(false);break;case 3:map.Rotate("LEFT")
;break;case 4:map.Move("LEFT");break;case 5:map.Track("LEFT");break;case 6:menu.PreviousDataPage();break;}DrawMenu(menu);
}void Action2(MapMenu menu,StarMap map){menu.PressButton(2);switch(menu.CurrentPage){case 1:CyclePlanets(map,true);break;
case 2:map.Zoom(true);break;case 3:map.Rotate("RIGHT");break;case 4:map.Move("RIGHT");break;case 5:map.Track("RIGHT");break;
case 6:menu.NextDataPage();break;}DrawMenu(menu);}void Action3(MapMenu menu,StarMap map){menu.PressButton(3);switch(menu.
CurrentPage){case 1:CycleWaypoints(map,false);break;case 2:map.AdjustRadius(true);break;case 3:map.Rotate("DOWN");break;case 4:map.
Move("DOWN");break;case 5:map.Track("DOWN");break;case 6:menu.ScrollUp();break;}DrawMenu(menu);}void Action4(MapMenu menu,
StarMap map){menu.PressButton(4);switch(menu.CurrentPage){case 1:CycleWaypoints(map,true);break;case 2:map.AdjustRadius(false);
break;case 3:map.Rotate("UP");break;case 4:map.Move("UP");break;case 5:map.Track("UP");break;case 6:menu.ScrollDown();break;}
DrawMenu(menu);}void Action5(MapMenu menu,StarMap map){menu.PressButton(5);switch(menu.CurrentPage){case 1:menu.PreviousMap();
break;case 2:CycleMode(map,false);break;case 3:map.Spin("LEFT");break;case 4:map.Move("BACKWARD");break;case 5:map.Track(
"BACKWARD");break;case 6:menu.PreviousDataDisplay();break;}DrawMenu(menu);}void Action6(MapMenu menu,StarMap map){menu.PressButton
(6);switch(menu.CurrentPage){case 1:menu.NextMap();break;case 2:CycleMode(map,true);break;case 3:map.Spin("RIGHT");break;
case 4:map.Move("FORWARD");break;case 5:map.Track("FORWARD");break;case 6:menu.NextDataDisplay();break;}DrawMenu(menu);}void
Action7(MapMenu menu,StarMap map){menu.PressButton(7);switch(menu.CurrentPage){case 1:cycleGPS(map);break;case 2:map.ShowInfo=
setState(map.ShowInfo,3);break;case 3:map.ShowInfo=setState(map.ShowInfo,3);break;case 4:map.ShowInfo=setState(map.ShowInfo,3);
break;case 5:map.Stop();break;case 6:break;}DrawMenu(menu);}const string DATA_TAG="[Map Data]";const string DATA_HEADER=
"Data Display";const string DATA_SCREENS="Data Display Screens";const string DATA_PAGE="Current Page";const string DATA_SCROLL=
"Scroll Level";const string SCREEN_KEY="Screen Index";const string SYSTEM_TITLE="SYSTEM DATA";const string PLANET_TITLE="PLANETS";
const string WAYPOINT_TITLE="WAYPOINTS";const string GPS_INPUT="GPS INPUT";const string BELOW_LINE=
"~Input Terminal Coords Below This Line~";const int PAGE_LIMIT=5;const int SYSTEM_PAGE=0;const int PLANET_PAGE=1;const int WAYPOINT_PAGE=2;const int INPUT_PAGE=3
;const int CLIPBOARD_PAGE=4;const int MAP_PAGE=5;static List<string>_systemDataPage;static List<string>_planetDataPage;
static List<string>_waypointDataPage;static List<string>_mapDataPage;static List<DataDisplay>_dataDisplays;class DataDisplay{
public int CurrentPage;public int ScrollIndex;int ScreenIndex;public int IDNumber;public IMyTerminalBlock Owner;public
IMyTextSurface Surface;public string Header;public DataDisplay(IMyTerminalBlock block,int screenNumber){Owner=block;ScreenIndex=
screenNumber;Header=DATA_HEADER;if((block as IMyTextSurfaceProvider).SurfaceCount>1)Header+=" - Screen "+ScreenIndex;CurrentPage=
ParseInt(GetKey(block,Header,DATA_PAGE,"0"),0);ScrollIndex=ParseInt(GetKey(block,Header,DATA_SCROLL,"0"),0);try{Surface=(Owner
as IMyTextSurfaceProvider).GetSurface(ScreenIndex);}catch{Surface=null;AddMessage(
"Screen Index Error for Data Display on\n\""+Owner.CustomName+"\"");}}public void ScrollDown(){ScrollIndex++;DisplayPage(true);}public void ScrollUp(){if(
ScrollIndex>0)ScrollIndex--;DisplayPage(true);}public void NextPage(){CurrentPage++;if(CurrentPage>PAGE_LIMIT)CurrentPage=0;SetKey(
Owner,Header,DATA_PAGE,CurrentPage.ToString());ScrollIndex=0;SetKey(Owner,Header,DATA_SCROLL,"0");DisplayPage(true);}public
void PreviousPage(){CurrentPage--;if(CurrentPage<0)CurrentPage=PAGE_LIMIT;SetKey(Owner,Header,DATA_PAGE,CurrentPage.ToString
());ScrollIndex=0;SetKey(Owner,Header,DATA_SCROLL,"0");DisplayPage(true);}public string BuildPageHeader(string pageTitle)
{string displayNumber="";if(_dataDisplays.Count>1)displayNumber="["+IDNumber+"] ";return"// "+displayNumber+pageTitle+
SLASHES+"\n";}string GetScrolledPage(List<string>entries){string output="";if(entries.Count>0){if(ScrollIndex>=entries.Count){
ScrollIndex=entries.Count-1;SetKey(Owner,Header,DATA_SCROLL,ScrollIndex.ToString());}for(int i=ScrollIndex;i<entries.Count;i++)
output+=entries[i]+"\n\n";}return output;}void DisplaySystemData(){Surface.WriteText(BuildPageHeader(SYSTEM_TITLE)+
GetScrolledPage(_systemDataPage));}void DisplayPlanetData(){Surface.WriteText(BuildPageHeader(PLANET_TITLE)+GetScrolledPage(
_planetDataPage));}void DisplayWaypointData(){Surface.WriteText(BuildPageHeader(WAYPOINT_TITLE)+GetScrolledPage(_waypointDataPage));}
public void DisplayGPSInput(){StringBuilder menuText=new StringBuilder();Surface.ReadText(menuText,false);string output=
menuText.ToString();string title=BuildPageHeader(GPS_INPUT);if(!(output.StartsWith(title)))output=title+BELOW_LINE;Surface.
WriteText(output);}void DisplayClipboard(){Surface.WriteText(BuildPageHeader("CLIPBOARD")+"\n"+_clipboard);}void DisplayMapData()
{Surface.WriteText(BuildPageHeader("MAP SCREENS")+GetScrolledPage(_mapDataPage));}public void DisplayPage(bool
fromCommand){switch(CurrentPage){case SYSTEM_PAGE:DisplaySystemData();break;case PLANET_PAGE:DisplayPlanetData();break;case
WAYPOINT_PAGE:DisplayWaypointData();break;case INPUT_PAGE:if(fromCommand)DisplayGPSInput();break;case CLIPBOARD_PAGE:if(fromCommand)
DisplayClipboard();break;case MAP_PAGE:if(fromCommand)DisplayMapData();break;}}}void AssignDataDisplays(){_dataDisplays=new List<
DataDisplay>();List<IMyTerminalBlock>displayBlocks=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(DATA_TAG,
displayBlocks);if(displayBlocks.Count<1)return;UpdatePageData();foreach(IMyTerminalBlock block in displayBlocks){if(OnGrid(block)){
AddScreensFromData(block);}}}void AddScreensFromData(IMyTerminalBlock block){int screenCount=(block as IMyTextSurfaceProvider).
SurfaceCount;if(screenCount<1){AddMessage("Block \""+block.CustomName+"\" contains no surfaces for Data Display!");return;}else if(
screenCount==1){AddDataDisplay(block,0);}else{string defaultBool="True";for(int i=0;i<screenCount;i++){if(ParseBool(GetKey(block,
DATA_SCREENS,"Show on Screens "+i,defaultBool)))AddDataDisplay(block,i);defaultBool="False";}}}void AddDataDisplay(IMyTerminalBlock
block,int screenNumber){DataDisplay display=new DataDisplay(block,screenNumber);if(display.Surface!=null){display.IDNumber=
_dataDisplays.Count;display.Surface.ContentType=ContentType.TEXT_AND_IMAGE;_dataDisplays.Add(display);}}void UpdatePageData(){
UpdateSystemDataPage();UpdatePlanetDataPage();UpdateWaypointDataPage();}static void UpdateMapDataPage(){_mapDataPage=new List<string>();if(
_mapList.Count<1)return;foreach(StarMap map in _mapList){_mapDataPage.Add("Map "+map.Number+" --- "+map.Viewport.Width+" x "+map
.Viewport.Height+"\n * Block: "+map.Block.CustomName+"\n * Screen: "+map.Index);}}void UpdateSystemDataPage(){
_systemDataPage=new List<string>();_systemDataPage.Add("  Command: "+_previousCommand);_systemDataPage.Add("  Maps: "+_mapList.Count+
" -- Data Screens: "+_dataDisplays.Count+" -- Menus: "+_mapMenus.Count);_systemDataPage.Add("  Planets: "+_planetList.Count+
" -- Waypoints: "+_waypointList.Count);_systemDataPage.Add("Messages:");if(_messages.Count<0)return;for(int i=_messages.Count-1;i>-1;i--)
_systemDataPage.Add(_messages[i]);}void UpdatePlanetDataPage(){if(!_planets)return;_planetDataPage=new List<string>();foreach(Planet
planet in _planetList){_planetDataPage.Add(planet.name+"\n * Radius: "+(planet.radius/1000).ToString("N1")+"km -- Dist: "+(
planet.Distance/1000).ToString("N1")+" km");}}void UpdateWaypointDataPage(){_waypointDataPage=new List<string>();if(
_waypointList.Count<1)return;foreach(Waypoint waypoint in _waypointList){_waypointDataPage.Add(waypoint.name+" --- "+waypoint.marker+
"\n * Dist: "+(waypoint.Distance/1000).ToString("N1")+" km");}}static DataDisplay GetDataDisplay(int displayID){if(displayID>=
_dataDisplays.Count||displayID<0)return null;return _dataDisplays[displayID];}DataDisplay DataDisplayFromString(string dataID){int
index;if(dataID=="")index=0;else index=ParseInt(dataID,int.MaxValue);return GetDataDisplay(index);}void NextDataPage(string
dataID,bool next=true){DataDisplay display=DataDisplayFromString(dataID);if(display==null)return;if(next)display.NextPage();
else display.PreviousPage();}void ScrollData(string direction,string dataID){DataDisplay display=DataDisplayFromString(
dataID);if(display==null)return;if(direction.ToUpper()=="UP")display.ScrollUp();else display.ScrollDown();}void UpdateDisplays
(bool fromCommand=false){if(_dataDisplays.Count<1)return;foreach(DataDisplay display in _dataDisplays)display.DisplayPage
(fromCommand);}MySpriteDrawFrame _frame;void DrawTexture(string shape,Vector2 position,Vector2 size,float rotation,Color
color){var sprite=new MySprite(){Type=SpriteType.TEXTURE,Data=shape,Position=position,Size=size,RotationOrScale=rotation,
Color=color};_frame.Add(sprite);}void DrawText(string text,Vector2 position,float scale,TextAlignment alignment,Color color){
var sprite=new MySprite(){Type=SpriteType.TEXT,Data=text,Position=position,RotationOrScale=scale,Color=color,Alignment=
alignment,FontId="White"};_frame.Add(sprite);}void DrawMaps(){foreach(StarMap map in _mapList){if(map.Mode=="CHASE"){AlignShip(
map);}else if(map.Mode=="PLANET"&&_planetList.Count>0){ShipToPlanet(map);}else if(map.Mode=="ORBIT"){AlignOrbit(map);}else{
map.Azimuth=DegreeAdd(map.Azimuth,map.dAz);if(map.Mode!="SHIP"){Vector3 deltaC=new Vector3(map.dX,map.dY,map.dZ);map.Center
+=rotateMovement(deltaC,map);}else{map.Center=_myPos;}UpdateMap(map);}_frame=map.DrawingSurface.DrawFrame();DrawMap(map);
_frame.Dispose();}}void DrawShip(StarMap map,List<Planet>displayPlanets){Color bodyColor=new Color(SHIP_RED,SHIP_GREEN,
SHIP_BLUE);Color aftColor=new Color(180,60,0);Color plumeColor=Color.Yellow;Color canopyColor=Color.DarkBlue;Vector3
transformedShip=transformVector(_myPos,map);Vector2 shipPos=PlotObject(transformedShip,map);float shipX=shipPos.X;float shipY=shipPos.Y
;int vertMod=0;if(map.ShowInfo){vertMod=BAR_HEIGHT;if(map.Viewport.Width>500){vertMod*=2;}}bool offZ=transformedShip.Z<
map.FocalLength;bool leftX=shipX<-map.Viewport.Width/2||(offZ&&shipX<0);bool rightX=shipX>map.Viewport.Width/2||(offZ&&
shipX>=0);bool aboveY=shipY<-map.Viewport.Height/2+vertMod||(offZ&&shipY<0);bool belowY=shipY>map.Viewport.Height/2-vertMod||
(offZ&&shipX>=0);bool offX=leftX||rightX;bool offY=aboveY||belowY;if(offZ||offX||offY){float posX;float posY;float
rotation=0;int pointerScale=SHIP_SCALE/2;if(offZ){bodyColor=Color.DarkRed;}else{bodyColor=Color.DodgerBlue;}if(leftX){posX=0;
rotation=(float)Math.PI*3/2;}else if(rightX){posX=map.Viewport.Width-pointerScale;rotation=(float)Math.PI/2;}else{posX=map.
Viewport.Width/2+shipX-pointerScale/2;}if(aboveY){posY=vertMod+TOP_MARGIN+map.Viewport.Center.Y-map.Viewport.Height/2;rotation=0
;}else if(belowY){posY=map.Viewport.Center.Y+map.Viewport.Height/2-vertMod-TOP_MARGIN;rotation=(float)Math.PI;}else{posY=
map.Viewport.Height/2+shipY+(map.Viewport.Width-map.Viewport.Height)/2;}if(offX&&offY){rotation=(float)Math.Atan2(shipY,
shipX);}DrawTexture("Triangle",new Vector2(posX-2,posY),new Vector2(pointerScale+4,pointerScale+4),rotation,Color.Black);
DrawTexture("Triangle",new Vector2(posX,posY),new Vector2(pointerScale,pointerScale),rotation,bodyColor);}else{Vector2 position=
shipPos;Vector3 heading=rotateVector(_refBlock.WorldMatrix.Forward,map);if(displayPlanets.Count>0){String planetColor=
obscureShip(position,displayPlanets,map);if(planetColor!="NONE"){bodyColor=ColorSwitch(planetColor,false)*2*map.BrightnessMod;
aftColor=bodyColor*0.75f;plumeColor=aftColor;canopyColor=aftColor;}}float headingX=heading.X;float headingY=heading.Y;float
headingZ=heading.Z;float aftScale=SHIP_SCALE*(1+0.125f*headingZ);float shipLength=(float)1.33*SHIP_SCALE*(float)Math.Sqrt(
headingX*headingX+headingY*headingY)/(float)Math.Sqrt(headingX*headingX+headingY*headingY+headingZ*headingZ);float shipAngle=(
float)Math.Atan2(headingX,headingY)*-1;position+=map.Viewport.Center;Vector2 offset=new Vector2((float)Math.Sin(shipAngle),(
float)Math.Cos(shipAngle)*-1);position+=offset*shipLength/4;position-=new Vector2(aftScale/2,0);Vector2 startPosition=
position;position-=new Vector2(2,0);DrawTexture("Triangle",position,new Vector2(aftScale+4,shipLength+4),shipAngle,Color.Black);
float aftHeight=aftScale-shipLength/(float)1.33;position=startPosition;position-=offset*shipLength/2;position-=new Vector2(2,
0);DrawTexture("Circle",position,new Vector2(aftScale+4,aftHeight+4),shipAngle,Color.Black);position=startPosition;
DrawTexture("Triangle",position,new Vector2(aftScale,shipLength),shipAngle,bodyColor);if(headingZ<0){position=startPosition;
position-=offset*shipLength/2;DrawTexture("Circle",position,new Vector2(aftScale,aftHeight),shipAngle,bodyColor);}position=
startPosition;position+=offset*shipLength/8;position+=new Vector2(aftScale/4,0);DrawTexture("Triangle",position,new Vector2(aftScale*
0.5f,shipLength*0.5f),shipAngle,canopyColor);Vector3 shipUp=rotateVector(_refBlock.WorldMatrix.Up,map);Vector3 shipRight=
rotateVector(_refBlock.WorldMatrix.Right,map);float rollInput=(float)Math.Atan2(shipRight.Z,shipUp.Z)+(float)Math.PI;float rollAngle
=(float)Math.Cos(rollInput/2)*(float)Math.Atan2(SHIP_SCALE,2*shipLength)*0.9f;float rollScale=(float)Math.Sin(rollInput/2
)*0.7f*shipLength/SHIP_SCALE;Vector2 rollOffset=new Vector2((float)Math.Sin(shipAngle-rollAngle),(float)Math.Cos(
shipAngle-rollAngle)*-1);float maskMod=0.95f;if(rollInput<0.35f||rollInput>5.93f)maskMod=0.6f;var edgeColor=bodyColor;int edgeMod
=1;if(headingZ<0){edgeColor=canopyColor;edgeMod=-1;}position=startPosition;position-=offset*shipLength/8;position+=new
Vector2(aftScale/4,0);DrawTexture("SemiCircle",position,new Vector2(aftScale*0.5f,aftHeight*0.5f*edgeMod),shipAngle,edgeColor);
position=startPosition;position+=new Vector2((1-rollScale)*aftScale/2,0);position+=offset*shipLength*0.25f;position-=rollOffset*
0.7f*shipLength/3;DrawTexture("Triangle",position,new Vector2(aftScale*rollScale,shipLength*maskMod),shipAngle-rollAngle,
bodyColor);if(headingZ>=0){position=startPosition;position-=offset*shipLength/2;DrawTexture("Circle",position,new Vector2(
aftScale,aftHeight),shipAngle,aftColor);position-=offset*shipLength/16;position+=new Vector2(aftScale/6,0);DrawTexture("Circle",
position,new Vector2(aftScale*0.67f,aftHeight*0.67f),shipAngle,plumeColor);}}}Vector2 PlotObject(Vector3 pos,StarMap map){float
zFactor=map.FocalLength/pos.Z;float plotX=pos.X*zFactor;float plotY=pos.Y*zFactor;Vector2 mapPos=new Vector2(-plotX,-plotY);
return mapPos;}void DrawPlanets(List<Planet>displayPlanets,StarMap map){PlanetSort(displayPlanets,map);string drawnPlanets=
"Displayed Planets:";foreach(Planet planet in displayPlanets){drawnPlanets+=" "+planet.name+",";Vector2 planetPosition=PlotObject(planet.
transformedCoords[map.Number],map);planet.mapPos=planetPosition;Color surfaceColor=ColorSwitch(planet.color,false)*map.BrightnessMod;
Color lineColor=surfaceColor*2;Vector2 startPosition=map.Viewport.Center+planetPosition;float diameter=ProjectDiameter(planet
,map);Vector2 position;Vector2 size;if(map.Mode=="ORBIT"&&planet==map.ActivePlanet){float radMod=0.83f;size=new Vector2(
diameter*radMod*2,diameter*radMod*2);position=startPosition-new Vector2(diameter*radMod,0);DrawTexture("CircleHollow",position,
size,0,Color.Yellow);radMod*=0.99f;position=startPosition-new Vector2(diameter*radMod,0);size=new Vector2(diameter*radMod*2,
diameter*radMod*2);DrawTexture("CircleHollow",position,size,0,Color.Black);}position=startPosition-new Vector2(diameter/2,0);
DrawTexture("Circle",position,new Vector2(diameter,diameter),0,surfaceColor);double radAngle=(float)map.Altitude*Math.PI/180;float
pitchMod=(float)Math.Sin(radAngle)*diameter;DrawTexture("CircleHollow",position,new Vector2(diameter,pitchMod),0,lineColor);int
scaleMod=-1;if(map.Altitude<0){scaleMod*=-1;}DrawTexture("SemiCircle",position,new Vector2(diameter,diameter*scaleMod),0,
surfaceColor);DrawTexture("CircleHollow",position,new Vector2(diameter,diameter),0,lineColor);if(diameter>HASH_LIMIT&&map.Mode!=
"CHASE"){DrawHashMarks(planet,diameter,lineColor,map);}if(map.ShowNames&&map.GpsState>0){float fontMod=1;if(diameter<50){
fontMod=(float)0.5;}position=startPosition;DrawText(planet.name,position,fontMod*0.8f,TextAlignment.CENTER,Color.Black);
position+=new Vector2(-2,2);DrawText(planet.name,position,fontMod*0.8f,TextAlignment.CENTER,Color.Yellow*map.BrightnessMod);}}
Echo(drawnPlanets.Trim(',')+"\n");}void DrawHashMarks(Planet planet,float diameter,Color lineColor,StarMap map){List<
Waypoint>hashMarks=new List<Waypoint>();float planetDepth=planet.transformedCoords[map.Number].Z;Waypoint north=new Waypoint();
north.name="N -";north.position=planet.position+new Vector3(0,(float)planet.radius,0);north.transformedCoords.Add(
transformVector(north.position,map));if(north.transformedCoords[0].Z<planetDepth){hashMarks.Add(north);}Waypoint south=new Waypoint();
south.name="S -";south.position=planet.position-new Vector3(0,(float)planet.radius,0);south.transformedCoords.Add(
transformVector(south.position,map));if(south.transformedCoords[0].Z<planetDepth){hashMarks.Add(south);}float r1=planet.radius*0.95f;
float r2=(float)Math.Sqrt(2)/2*r1;float r3=r1/2;String[]latitudes=new String[]{"+","|","+"};String[]longitudes=new String[]{
"135°E","90°E","45°E","0°","45°W","90°W","135°W","180°"};float[]yCoords=new float[]{-r2,0,r2};float[,]xCoords=new float[,]{{-r3
,-r2,-r3,0,r3,r2,r3,0},{-r2,-r1,-r2,0,r2,r1,r2,0},{-r3,-r2,-r3,0,r3,r2,r3,0}};float[,]zCoords=new float[,]{{r3,0,-r3,-r2,
-r3,0,r3,r2},{r2,0,-r2,-r1,-r2,0,r2,r1},{r3,0,-r3,-r2,-r3,0,r3,r2}};for(int m=0;m<3;m++){String latitude=latitudes[m];
float yCoord=yCoords[m];for(int n=0;n<8;n++){Waypoint hashMark=new Waypoint();hashMark.name=latitude+" "+longitudes[n];float
xCoord=xCoords[m,n];float zCoord=zCoords[m,n];hashMark.position=planet.position+new Vector3(xCoord,yCoord,zCoord);hashMark.
transformedCoords.Add(transformVector(hashMark.position,map));if(hashMark.transformedCoords[0].Z<planetDepth){hashMarks.Add(hashMark);}}}
foreach(Waypoint hash in hashMarks){Vector2 position=map.Viewport.Center+PlotObject(hash.transformedCoords[0],map);if(diameter>
2*HASH_LIMIT){String[]hashLabels=hash.name.Split(' ');float textMod=1;int pitchMod=1;if(map.Altitude>0){pitchMod=-1;}if(
diameter>3*HASH_LIMIT){textMod=1.5f;}Vector2 hashOffset=new Vector2(0,10*textMod*pitchMod);position-=hashOffset;DrawText(
hashLabels[0],position,0.5f*textMod,TextAlignment.CENTER,lineColor);position+=hashOffset;DrawText(hashLabels[1],position,0.4f*
textMod,TextAlignment.CENTER,lineColor);}else{position+=new Vector2(-2,2);DrawTexture("Circle",position,new Vector2(4,4),0,
lineColor);}}}void DrawWaypoints(StarMap map){float fontSize=0.5f;float markerSize=MARKER_WIDTH;int focalRadius=map.
RotationalRadius-map.FocalLength;if(map.Viewport.Width>500){fontSize*=1.5f;markerSize*=2;}foreach(Waypoint waypoint in _waypointList){if
(waypoint.isActive){float rotationMod=0;Color markerColor=ColorSwitch(waypoint.color,true);float gpsScale=1;float coordZ;
try{coordZ=waypoint.transformedCoords[map.Number].Z;}catch{return;}bool activePoint=(map.ActiveWaypointName!="")&&(waypoint
.name==map.ActiveWaypointName);if(map.GpsState==1&&!activePoint)gpsScale=FOCAL_MOD*map.FocalLength/coordZ;float iconSize=
markerSize*gpsScale;Vector2 markerScale=new Vector2(iconSize,iconSize);Vector2 waypointPosition=PlotObject(waypoint.
transformedCoords[map.Number],map);Vector2 startPosition=map.Viewport.Center+waypointPosition;String markerShape="";switch(waypoint.
marker.ToUpper()){case STATION:markerShape="CircleHollow";break;case BASE:markerShape="SemiCircle";markerScale*=1.25f;break;
case LANDMARK:markerShape="Triangle";markerColor=new Color(48,48,48);break;case HAZARD:markerShape="SquareTapered";
markerColor=Color.Red;rotationMod=(float)Math.PI/4;break;case ASTEROID:markerShape="SquareTapered";markerColor=new Color(48,32,32);
markerScale*=0.9f;rotationMod=(float)Math.PI/4;break;default:markerShape="SquareHollow";break;}if(coordZ>map.FocalLength){Vector2
position=startPosition-new Vector2(iconSize/2,0);markerColor*=map.BrightnessMod;DrawTexture(markerShape,position,markerScale,
rotationMod,Color.Black);position+=new Vector2(1,0);DrawTexture(markerShape,position,markerScale*1.2f,rotationMod,markerColor);
position+=new Vector2(1,0);DrawTexture(markerShape,position,markerScale,rotationMod,markerColor);switch(waypoint.marker.ToUpper(
)){case STATION:position+=new Vector2(iconSize/2-iconSize/20,0);DrawTexture("SquareSimple",position,new Vector2(iconSize/
10,iconSize),rotationMod,markerColor);break;case HAZARD:position+=new Vector2(iconSize/2-iconSize/20,-iconSize*0.85f);
DrawText("!",position,fontSize*1.2f*gpsScale,TextAlignment.CENTER,Color.White);break;case BASE:position+=new Vector2(iconSize/6,
-iconSize/12);DrawTexture("SemiCircle",position,new Vector2(iconSize*1.15f,iconSize*1.15f),rotationMod,new Color(0,64,64)
*map.BrightnessMod);startPosition-=new Vector2(0,iconSize*0.4f);break;case ASTEROID:position+=new Vector2(iconSize/2-
iconSize/20,0);DrawTexture("SquareTapered",position,markerScale,rotationMod,new Color(32,32,32)*map.BrightnessMod);position-=new
Vector2(iconSize-iconSize/10,0);DrawTexture("SquareTapered",position,markerScale,rotationMod,new Color(32,32,32)*map.
BrightnessMod);break;default:break;}if(activePoint){position=startPosition-new Vector2(0.9f*markerSize,0.33f*markerSize);DrawText(
"|________",position,fontSize,TextAlignment.LEFT,Color.White);}if(map.ShowNames){position=startPosition+new Vector2(1.33f*iconSize,
-0.75f*iconSize);DrawText(waypoint.name,position,fontSize*gpsScale,TextAlignment.LEFT,markerColor*map.BrightnessMod);}}}}
}float ProjectDiameter(Planet planet,StarMap map){float viewAngle=(float)Math.Asin(planet.radius/planet.transformedCoords
[map.Number].Z);float diameter=(float)Math.Tan(Math.Abs(viewAngle))*2*map.FocalLength;if(diameter<DIAMETER_MIN){diameter=
DIAMETER_MIN;}return diameter;}String obscureShip(Vector2 shipPos,List<Planet>planets,StarMap map){Planet closest=planets[0];foreach
(Planet planet in planets){if(Vector2.Distance(shipPos,planet.mapPos)<Vector2.Distance(shipPos,closest.mapPos)){closest=
planet;}}String color="NONE";float distance=Vector2.Distance(shipPos,closest.mapPos);float radius=0.95f*closest.radius*map.
FocalLength/closest.transformedCoords[map.Number].Z;if(distance<radius&&closest.transformedCoords[map.Number].Z<transformVector(
_myPos,map).Z){color=closest.color;}return color;}static void PrepareTextSurfaceForSprites(IMyTextSurface textSurface){
textSurface.ContentType=ContentType.SCRIPT;textSurface.ScriptBackgroundColor=new Color(0,0,0);textSurface.Script="";}void DrawMap(
StarMap map){Echo("[MAP "+map.Number+"]");Vector3 mapCenter=map.Center;Color gridColor=new Color(0,64,0);Vector2 position=map.
Viewport.Center-new Vector2(map.Viewport.Width/2,0);DrawTexture("Grid",position,map.Viewport.Size,0,gridColor);List<Planet>
displayPlanets=new List<Planet>();if(_planetList.Count>0){foreach(Planet planet in _planetList){if(planet.transformedCoords.Count==
_mapList.Count&&planet.transformedCoords[map.Number].Z>map.FocalLength){displayPlanets.Add(planet);}}}DrawPlanets(displayPlanets
,map);if(map.GpsState>0){DrawWaypoints(map);}if(map.ShowShip){DrawShip(map,displayPlanets);}if(map.ShowInfo){DrawMapInfo(
map);}}void DrawMapInfo(StarMap map){float fontSize=0.6f;int barHeight=BAR_HEIGHT;String angleReading=map.Altitude*-1+"° "+
map.Azimuth+"°";String shipMode="S";String planetMode="P";String freeMode="F";String worldMode="W";String chaseMode="C";
String orbitMode="O";if(map.Viewport.Width>500){fontSize*=1.5f;barHeight*=2;angleReading="Alt:"+map.Altitude*-1+"°  Az:"+map.
Azimuth+"°";shipMode="SHIP";planetMode="PLANET";freeMode="FREE";worldMode="WORLD";chaseMode="CHASE";orbitMode="ORBIT";}var
position=map.Viewport.Center;position-=new Vector2(map.Viewport.Width/2,map.Viewport.Height/2-barHeight/2);DrawTexture(
"SquareSimple",position,new Vector2(map.Viewport.Width,barHeight),0,Color.Black);position+=new Vector2(SIDE_MARGIN,-TOP_MARGIN);string
modeReading="";switch(map.Mode){case"SHIP":modeReading=shipMode;break;case"PLANET":modeReading=planetMode;break;case"WORLD":
modeReading=worldMode;break;case"CHASE":modeReading=chaseMode;break;case"ORBIT":modeReading=orbitMode;break;default:modeReading=
freeMode;break;}DrawText(modeReading,position,fontSize,TextAlignment.LEFT,Color.White);string xCenter=abbreviateValue(map.Center
.X);string yCenter=abbreviateValue(map.Center.Y);string zCenter=abbreviateValue(map.Center.Z);string centerReading="["+
xCenter+", "+yCenter+", "+zCenter+"]";position+=new Vector2(map.Viewport.Width/2-SIDE_MARGIN,0);DrawText(centerReading,position
,fontSize,TextAlignment.CENTER,Color.White);position+=new Vector2(map.Viewport.Width/2-SIDE_MARGIN,TOP_MARGIN);Color
lightColor=new Color(0,8,0);if(_lightOn){DrawTexture("Circle",position,new Vector2(7,7),0,lightColor);}position-=new Vector2(5,7);
string mapID="["+map.Number+"]";DrawText(mapID,position,fontSize,TextAlignment.RIGHT,Color.White);position=map.Viewport.Center
;position-=new Vector2(map.Viewport.Width/2,barHeight/2-map.Viewport.Height/2);if(map.Viewport.Width==1024){position=new
Vector2(0,map.Viewport.Height-barHeight/2);}DrawTexture("SquareSimple",position,new Vector2(map.Viewport.Width,barHeight),0,
Color.Black);position+=new Vector2(SIDE_MARGIN,-TOP_MARGIN);string dofReading="FL:"+abbreviateValue((float)map.FocalLength);
DrawText(dofReading,position,fontSize,TextAlignment.LEFT,Color.White);position+=new Vector2(map.Viewport.Width/2-SIDE_MARGIN,0);
DrawText(angleReading,position,fontSize,TextAlignment.CENTER,Color.White);string radius="R:"+abbreviateValue((float)map.
RotationalRadius);position+=new Vector2(map.Viewport.Width/2-SIDE_MARGIN,0);DrawText(radius,position,fontSize,TextAlignment.RIGHT,Color.
White);}void DrawMenu(MapMenu menu){_frame=menu.Surface.DrawFrame();Vector2 center=menu.Viewport.Center;float height=menu.
Viewport.Height;float width=menu.Viewport.Width;float fontSize=0.5f;bool bigScreen=menu.Viewport.Width>500;if(bigScreen)fontSize
*=1.5f;Color bgColor=menu.BackgroundColor;Color titleColor=menu.TitleColor;Color labelColor=menu.LabelColor;Color
buttonColor=menu.ButtonColor;int page=menu.CurrentPage;float cellWidth=(width/7);float buttonHeight=(height/2);if(buttonHeight>
cellWidth)buttonHeight=cellWidth-4;Vector2 position=center-new Vector2(width/2,0);DrawTexture("SquareSimple",position,new Vector2
(width,height),0,bgColor);Vector2 topLeft;switch(menu.Alignment.ToUpper()){case"TOP":topLeft=center-new Vector2(width/2,
height/2);break;case"BOTTOM":topLeft=center-new Vector2(width/2,height/-2+buttonHeight*2);break;case"CENTER":default:topLeft=
center-new Vector2(width/2,buttonHeight);break;}position=topLeft+new Vector2((cellWidth-buttonHeight)/2,buttonHeight*1.5f);
Vector2 buttonScale=new Vector2(buttonHeight,buttonHeight);for(int i=1;i<8;i++){Color color;if(i==menu.ActiveButton)color=
buttonColor*2;else color=buttonColor*0.5f;if(ShouldBeVisible(menu,i))DrawTexture("SquareSimple",position,buttonScale,0,color);
position+=new Vector2(cellWidth,0);}position=topLeft+new Vector2(10,0);DrawText("MENU "+page+": "+_menuTitle[page],position,
fontSize,TextAlignment.LEFT,titleColor);position=topLeft+new Vector2(width*0.67f,0);if(_mapMenus.Count>1)DrawText("ID: "+menu.
IDNumber,position,fontSize,TextAlignment.CENTER,labelColor);position=topLeft+new Vector2(width-10,0);if(_mapList.Count>1)
DrawText("MAP: "+menu.CurrentMapIndex,position,fontSize,TextAlignment.RIGHT,titleColor);position=topLeft+new Vector2(cellWidth,
buttonHeight*0.6f);DrawText(_labelA[page],position,fontSize*0.9f,TextAlignment.CENTER,labelColor);position+=new Vector2(cellWidth*2,
0);DrawText(_labelB[page],position,fontSize*0.9f,TextAlignment.CENTER,labelColor);position+=new Vector2(cellWidth*2,0);
string cLabel=_labelC[page];if((menu.CurrentPage==1&&_mapList.Count<2)||(menu.CurrentPage==6&&_dataDisplays.Count<2))cLabel=""
;else if(menu.CurrentPage==6&&_dataDisplays.Count>1&&menu.DataDisplay!=null)cLabel+=" "+menu.DataDisplay.IDNumber;
DrawText(cLabel,position,fontSize*0.9f,TextAlignment.CENTER,labelColor);position+=new Vector2(cellWidth*1.5f,0);DrawText(_labelD
[page],position,fontSize*0.75f,TextAlignment.CENTER,labelColor);Vector2 iconScale=buttonScale*0.33f;position=topLeft+new
Vector2(cellWidth/2,buttonHeight*1.27f);StringToIcon(_cmd1[page],position,iconScale,bgColor,bigScreen);position+=new Vector2(
cellWidth,0);StringToIcon(_cmd2[page],position,iconScale,bgColor,bigScreen);position+=new Vector2(cellWidth,0);StringToIcon(_cmd3
[page],position,iconScale,bgColor,bigScreen);position+=new Vector2(cellWidth,0);StringToIcon(_cmd4[page],position,
iconScale,bgColor,bigScreen);position+=new Vector2(cellWidth,0);StringToIcon(_cmd5[page],position,iconScale,bgColor,bigScreen);
position+=new Vector2(cellWidth,0);StringToIcon(_cmd6[page],position,iconScale,bgColor,bigScreen);position+=new Vector2(
cellWidth,0);StringToIcon(_cmd7[page],position,iconScale,bgColor,bigScreen);fontSize*=1.5f;position=topLeft+new Vector2(cellWidth
/2,buttonHeight*1.35f);DrawText("1",position,fontSize,TextAlignment.CENTER,bgColor);position+=new Vector2(cellWidth,0);
DrawText("2",position,fontSize,TextAlignment.CENTER,bgColor);position+=new Vector2(cellWidth,0);DrawText("3",position,fontSize,
TextAlignment.CENTER,bgColor);position+=new Vector2(cellWidth,0);DrawText("4",position,fontSize,TextAlignment.CENTER,bgColor);
position+=new Vector2(cellWidth,0);DrawText("5",position,fontSize,TextAlignment.CENTER,bgColor);position+=new Vector2(cellWidth,
0);DrawText("6",position,fontSize,TextAlignment.CENTER,bgColor);position+=new Vector2(cellWidth,0);DrawText("7",position,
fontSize,TextAlignment.CENTER,bgColor);if(menu.Decals!=""){DrawDecals(menu,topLeft,buttonHeight,menu.Alignment,menu.Decals);}
_frame.Dispose();}void StringToIcon(string arg,Vector2 position,Vector2 scale,Color color,bool bigScreen){switch(arg){case"<":
DrawTriangle(position,scale,color,"left");break;case">":DrawTriangle(position,scale,color,"right");break;case"<<":DrawDoubleTriangle
(position,scale,color,"left");break;case">>":DrawDoubleTriangle(position,scale,color,"right");break;case"^":DrawTriangle(
position,scale,color,"up");break;case"v":DrawTriangle(position,scale,color,"down");break;case"^^":DrawDoubleTriangle(position,
scale,color,"up");break;case"vv":DrawDoubleTriangle(position,scale,color,"down");break;case"-/o":DrawToggle(position,scale,
color);break;case"cycle":DrawCycle(position,scale,color);break;default:DrawCharacters(arg,position,scale,color,bigScreen);
break;}}void DrawTriangle(Vector2 position,Vector2 scale,Color color,string direction){float rotation;Vector2 offset;switch(
direction){case"right":rotation=0.5f;offset=new Vector2(scale.Y*0.33f,0);break;case"down":rotation=1;offset=new Vector2(scale.X/2
,scale.Y*-0.1f);break;case"left":rotation=1.5f;offset=new Vector2(scale.Y*0.67f,0);break;case"up":default:rotation=0;
offset=new Vector2(scale.X/2,scale.Y*0.1f);break;}DrawTexture("Triangle",position-offset,scale,(float)Math.PI*rotation,color);
}void DrawDoubleTriangle(Vector2 position,Vector2 scale,Color color,string direction){float rotation;float length=scale.Y
*0.33f;Vector2 offset1;Vector2 offset2;switch(direction){case"right":rotation=0.5f;offset1=new Vector2(scale.Y*0.33f,0);
offset2=new Vector2(length,0);break;case"down":rotation=1;offset1=new Vector2(scale.X/4,scale.Y*-0.25f);offset2=new Vector2(0,-
length);break;case"left":rotation=1.5f;offset1=new Vector2(scale.Y*0.167f,0);offset2=new Vector2(-length,0);break;case"up":
default:rotation=0;offset1=new Vector2(scale.X/4,scale.Y*0.125f);offset2=new Vector2(0,length);break;}rotation*=(float)Math.PI;
position-=offset1;DrawTexture("Triangle",position,scale*0.5f,rotation,color);position+=offset2;DrawTexture("Triangle",position,
scale*0.5f,rotation,color);}void DrawToggle(Vector2 position,Vector2 scale,Color color){position-=new Vector2(scale.Y/2,0);
DrawTexture("SemiCircle",position,scale,(float)Math.PI*1.5f,color);DrawTexture("CircleHollow",position,scale,0,color);DrawTexture(
"CircleHollow",position+new Vector2(scale.X*0.05f,0),scale*0.9f,0,color);DrawTexture("CircleHollow",position+new Vector2(scale.X*
0.075f,0),scale*0.85f,0,color);}void DrawCycle(Vector2 position,Vector2 scale,Color color){position-=new Vector2(scale.Y/2,0);
DrawTexture("CircleHollow",position,scale,0,color);DrawTexture("CircleHollow",position+new Vector2(scale.X*0.05f,0),scale*0.9f,0,
color);DrawTexture("CircleHollow",position+new Vector2(scale.X*0.075f,0),scale*0.85f,0,color);DrawTexture("Triangle",position
+new Vector2(scale.X*0.67f,-scale.Y*0.25f),scale*0.5f,(float)Math.PI*0.75f,color);}void DrawCharacters(string characters,
Vector2 position,Vector2 scale,Color color,bool bigScreen){Vector2 offset;float fontSize;float screenMod;if(bigScreen)screenMod
=1;else screenMod=1.5f;if(characters.Length>3){offset=new Vector2(0,scale.Y*0.67f*screenMod);fontSize=0.75f/screenMod;}
else{offset=new Vector2(0,scale.Y*1.25f*screenMod);fontSize=1.25f;}DrawText(characters,position-offset,fontSize,
TextAlignment.CENTER,color);}void DrawMenus(){if(_mapMenus.Count<1)return;foreach(MapMenu menu in _mapMenus){DrawMenu(menu);}}bool
ShouldBeVisible(MapMenu menu,int buttonNumber){switch(buttonNumber){case 5:case 6:if((menu.CurrentPage==1&&_mapList.Count<2)||(menu.
CurrentPage==6&&_dataDisplays.Count<2))return false;break;case 7:if(menu.CurrentPage==6)return false;break;}return true;}void
DrawDecals(MapMenu menu,Vector2 startPosition,float heightScale,string alignment,string decalType){Vector2 position;if(alignment==
"TOP")position=startPosition+new Vector2(0,menu.Viewport.Height/2+heightScale);else position=startPosition-new Vector2(0,menu
.Viewport.Height/2-heightScale);string texture;switch(menu.Decals){case"BLUEPRINT":case"BLUEPRINTS":texture=
GetBlueprintDecal(menu);break;case"GRAPH":case"GRAPHS":texture=GetGraphDecal(menu);break;default:return;}float widthMod;if(menu.Viewport.
Width<=500)widthMod=1.5f;else widthMod=1;DrawTexture(texture,position,new Vector2(menu.Viewport.Height*widthMod,menu.Viewport
.Height-heightScale*2),0,Color.White);}string GetGraphDecal(MapMenu menu){string output;switch(menu.CurrentPage){case 1:
case 4:output="LCD_Economy_Graph_2";break;case 2:case 5:output="LCD_Economy_Graph_3";break;case 3:case 6:output=
"LCD_Economy_Graph_5";break;default:output="OutOfOrder";break;}return output;}string GetBlueprintDecal(MapMenu menu){string output;switch(
menu.CurrentPage){case 1:case 4:output="LCD_Economy_SC_Blueprint";break;case 2:case 5:output="LCD_Economy_Blueprint_2";break
;case 3:case 6:output="LCD_Economy_Blueprint_3";break;default:output="OutOfOrder";break;}return output;}const string
SHARED="Shared Data";const char SEPARATOR=';';static string _gridID;const string GRID_KEY="Grid_ID";static void EnsureKey(
IMyTerminalBlock block,string header,string key,string defaultVal){MyIni ini=GetIni(block);if(!ini.ContainsKey(header,key))SetKey(block,
header,key,defaultVal);}static string GetKey(IMyTerminalBlock block,string header,string key,string defaultVal){EnsureKey(
block,header,key,defaultVal);MyIni blockIni=GetIni(block);return blockIni.Get(header,key).ToString();}static void SetKey(
IMyTerminalBlock block,string header,string key,string arg){MyIni blockIni=GetIni(block);blockIni.Set(header,key,arg);block.CustomData=
blockIni.ToString();}static MyIni GetIni(IMyTerminalBlock block){MyIni iniOuti=new MyIni();MyIniParseResult result;if(!iniOuti.
TryParse(block.CustomData,out result)){block.CustomData="---\n"+block.CustomData;if(!iniOuti.TryParse(block.CustomData,out
result))throw new Exception(result.ToString());}return iniOuti;}void SetGridID(string arg){string gridID;if(arg!=""&&arg!="0")
gridID=arg;else gridID=Me.CubeGrid.EntityId.ToString();SetKey(Me,SHARED,"Grid_ID",gridID);_gridID=gridID;List<IMyTerminalBlock
>blocks=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks);foreach(
IMyTerminalBlock block in blocks){if(block.IsSameConstructAs(Me)&&block.CustomData.Contains(SHARED))SetKey(block,SHARED,"Grid_ID",gridID
);}Build();}string InsertEntry(string entry,string oldString,char separator,int index,string placeHolder){List<string>
entries=StringToEntries(oldString,separator);if(index==entries.Count){entries.Add(entry);}else if(index>entries.Count){while(
index>entries.Count)entries.Add(placeHolder);entries.Add(entry);}else{entries[index]=entry;}string newString=entries[0];if(
entries.Count>1){for(int n=1;n<entries.Count;n++){newString+=separator+entries[n];}}return newString;}string InsertEntry(string
entry,string oldString,int index,int length,string placeHolder){string newString;List<string>entries=StringToEntries(
oldString,length,placeHolder);if(entries.Count==1&&length==0){return entry;}entries[index]=entry;newString=entries[0];for(int n=1
;n<entries.Count;n++){newString+=SEPARATOR+entries[n];}return newString;}List<string>StringToEntries(string arg,int
length,string placeHolder){List<string>entries=new List<string>();string[]args=arg.Split(SEPARATOR);foreach(string argument in
args){entries.Add(argument);}while(entries.Count<length){entries.Add(placeHolder);}return entries;}List<string>
StringToEntries(string arg,char separator){List<string>entries=new List<string>();string[]args=arg.Split(separator);foreach(string
argument in args){entries.Add(argument);}return entries;}static bool OnGrid(IMyTerminalBlock block){return GetKey(block,SHARED,
GRID_KEY,_gridID)==_gridID;}const string EARTH="EARTHLIKE;(0,0,0);60000;GREEN;1";const string MOON=
"MOON;(16388,136375,-113547);9394;GRAY;1";const string MARS="MARS;(1032762,134086,1632659);64606;RUST;1";const string EUROPA=
"EUROPA;(916410,16373.72,1616441);9600;LIGHTBLUE;1";const string ALIEN="ALIEN;(131110.8,131220.6,5731113);60894.06;MAGENTA;1";const string TITAN=
"TITAN;(36385.04,226384,5796385);9238.224;CYAN;1";const string TRITON="TRITON;(-284463.6,-2434464,365536.2);38128.81;WHITE;1";const string PERTAM=
"PERTAM;(-3967231.50,-32231.50,-767231.50);30066.50;BROWN;1";const int STRING_LENGTH=5;const string WAYPOINT="WAYPOINT";const string BASE="BASE";const string STATION="STATION";
const string HAZARD="HAZARD";const string LANDMARK="LANDMARK";const string ASTEROID="ASTEROID";static List<Planet>_planetList
;static List<Planet>_unchartedList;static List<Waypoint>_waypointList;class Location{public String name;public Vector3
position;public List<Vector3>transformedCoords;public String color;public float Distance;public Location(){}}class Waypoint:
Location{public String marker;public bool isActive;public Waypoint(){transformedCoords=new List<Vector3>();}public void
CycleType(){switch(marker){case WAYPOINT:marker=ASTEROID;break;case ASTEROID:marker=BASE;break;case BASE:marker=STATION;break;
case STATION:marker=HAZARD;break;case HAZARD:marker=LANDMARK;break;case LANDMARK:default:marker=WAYPOINT;break;}}}class
Planet:Location{public float radius;public Vector2 mapPos;public int SampleCount;public Planet(String planetString){string[]
planetData=planetString.Split(';');name=planetData[0];transformedCoords=new List<Vector3>();if(planetData.Length<STRING_LENGTH){
radius=0;return;}if(planetData[1]!=""){position=StringToVector3(planetData[1]);}if(planetData[2]!=""){radius=float.Parse(
planetData[2]);}else{radius=0;}color=planetData[3];SampleCount=ParseInt(planetData[4],1);}public override String ToString(){String
[]planetData=new String[9];planetData[0]=name;planetData[1]=Vector3ToString(position);float radius=this.radius;if(radius>
0){planetData[2]=radius.ToString();}else{planetData[2]="";}planetData[3]=color;planetData[4]=SampleCount.ToString();
String planetString=planetData[0];for(int i=1;i<STRING_LENGTH;i++){planetString=planetString+";"+planetData[i];}return
planetString;}}void CyclePlanetsForList(List<StarMap>maps,bool next){if(NoMaps(maps))return;foreach(StarMap map in maps){
CyclePlanets(map,next);}}void CyclePlanets(StarMap map,bool next){if(!_planets){AddMessage("No Planets Logged!");return;}map.
DefaultView();if(next){map.PlanetIndex++;}else{map.PlanetIndex--;}if(map.PlanetIndex<0){map.PlanetIndex=_planetList.Count-1;}else
if(map.PlanetIndex>=_planetList.Count){map.PlanetIndex=0;}map.SetActivePlanet(_planetList[map.PlanetIndex]);}static Planet
GetPlanet(string planetName){if(planetName==""||planetName=="[null]")return null;if(_unchartedList.Count>0){foreach(Planet
uncharted in _unchartedList){if(uncharted.name.ToUpper()==planetName.ToUpper()){return uncharted;}}}if(_planets){foreach(Planet
planet in _planetList){if(planet.name.ToUpper()==planetName.ToUpper()){return planet;}}}return null;}static Waypoint
GetWaypoint(string waypointName){if(_waypointList.Count>0){foreach(Waypoint waypoint in _waypointList){if(waypoint.name.ToUpper()==
waypointName.ToUpper()){return waypoint;}}}return null;}void CycleWaypoints(StarMap map,bool next){int gpsCount=_waypointList.Count;
if(gpsCount<1){AddMessage("No Waypoints Logged!");return;}map.DefaultView();if(next){map.WaypointIndex++;}else{map.
WaypointIndex--;}if(map.WaypointIndex==-1){map.ActiveWaypoint=null;map.ActiveWaypointName="";return;}else if(map.WaypointIndex<-1){
map.WaypointIndex=gpsCount-1;}else if(map.WaypointIndex>=gpsCount){map.WaypointIndex=-1;map.ActiveWaypoint=null;map.
ActiveWaypointName="";return;}Waypoint waypoint=_waypointList[map.WaypointIndex];map.SetActiveWaypoint(waypoint);}void
CycleWaypointsForList(List<StarMap>maps,bool next){if(NoMaps(maps))return;foreach(StarMap map in maps){CycleWaypoints(map,next);}}float
GetDistance(Location location){return Vector3.Distance(location.position,_myPos);}void UpdateDistances(){if(_waypointList.Count>0){
foreach(Waypoint waypoint in _waypointList){waypoint.Distance=GetDistance(waypoint);}}if(_planetList.Count>0){foreach(Planet
planet in _planetList){planet.Distance=GetDistance(planet)-planet.radius;}}}void SortByNearest(List<Planet>planets){int length
=planets.Count;if(length>1){for(int i=0;i<length-1;i++){for(int p=1;p<length;p++){Planet planetA=planets[p-1];Planet
planetB=planets[p];float distA=planetA.Distance;float distB=planetB.Distance;if(distB<distA){planets[p-1]=planetB;planets[p]=
planetA;}}length--;if(length<2){return;}}}}void SortWaypoints(List<Waypoint>waypoints){int length=waypoints.Count;for(int i=0;i
<length-1;i++){for(int w=1;w<length;w++){Waypoint pointA=waypoints[w-1];Waypoint pointB=waypoints[w];float distA=pointA.
Distance;float distB=pointB.Distance;if(distB<distA){waypoints[w-1]=pointB;waypoints[w]=pointA;}}length--;if(length<2){return;}}
}void SortGlobalWaypoints(){if(_waypointList.Count<1)return;_sortCounter++;if(_sortCounter>=10){SortWaypoints(
_waypointList);_sortCounter=0;}}void SortPlanetsForMaps(){if(_planets){SortByNearest(_planetList);_nearestPlanet=_planetList[0];if(
_mapList.Count<1)return;foreach(StarMap map in _mapList){if(map.Mode=="PLANET"||map.Mode=="CHASE"||map.Mode=="ORBIT"){UpdateMap(
map);}}}}void LoadVanillaPlanets(){List<string>planetData=new List<string>{EARTH,MOON,MARS,EUROPA,ALIEN,TITAN,TRITON,PERTAM
};foreach(string entry in planetData){string planetName=entry.Split(';')[0];Planet planet=GetPlanet(planetName);if(planet
==null)_planetList.Add(new Planet(entry));else AddMessage("Planet of name \""+planetName+"\" already logged.\n");}
DataToLog();}string ConvertOldPlanetData(string dataToCheck){string[]data=dataToCheck.Split(';');int length=data.Length;if(length
==5)return dataToCheck;string dataOut="";if(dataToCheck!="")dataOut+=data[0]+";";else dataOut+="ERROR;";if(length>1&&data[
1]!="")dataOut+=data[1]+";";else dataOut+="(0,0,0);";if(length>2&&data[2]!="")dataOut+=data[2]+";";else dataOut+="0";if(
length>3)dataOut+=data[3];dataOut+=";1";return dataOut;}void MainSwitch(string argument){string[]args=argument.Split(' ');
string[]cmds=args[0].ToUpper().Split('_');string command=cmds[0];string cmdArg="";if(cmds.Length>1)cmdArg=cmds[1];string
argData="";_previousCommand="Command: "+argument;if(args.Length==1){argData="0";}else if(args.Length>1){argData=args[1];if(args
.Length>2){for(int q=2;q<args.Length;q++){argData+=" "+args[q];}}}List<StarMap>maps=new List<StarMap>();if(!(cmdArg.
Contains("SCAN")))maps=ArgToMaps(argData);switch(command){case"ZOOM":ZoomMaps(maps,cmdArg);break;case"MOVE":MoveMaps(maps,cmdArg
);break;case"DEFAULT":MapsToDefault(maps);break;case"ROTATE":RotateMaps(maps,cmdArg);break;case"SPIN":SpinMaps(maps,
cmdArg);break;case"TRACK":TrackMaps(maps,cmdArg);break;case"STOP":StopMaps(maps);break;case"GPS":if(cmdArg=="ON"){Show(maps,
"GPS",1);}else{Show(maps,"GPS",0);}break;case"HIDE":if(cmdArg=="WAYPOINT"){SetWaypointState(argData,0);}else{Show(maps,cmdArg
,0);}break;case"SHOW":if(cmdArg=="WAYPOINT"){SetWaypointState(argData,1);}else{Show(maps,cmdArg,1);}break;case"TOGGLE":
AddMessage("TOGGLING");if(cmdArg=="WAYPOINT"){SetWaypointState(argData,2);}else{Show(maps,cmdArg,3);}break;case"CYCLE":if(cmdArg==
"GPS")cycleGPSForList(maps);else if(cmdArg.Contains("TYPE"))maps[0].CycleActiveWaypointType();break;case"NEXT":nextLast(maps,
cmdArg,argData,true);break;case"PREVIOUS":nextLast(maps,cmdArg,argData,false);break;case"WORLD":ChangeMode("WORLD",maps);break
;case"SHIP":ChangeMode("SHIP",maps);break;case"CHASE":ChangeMode("CHASE",maps);break;case"PLANET":ChangeMode("PLANET",
maps);break;case"FREE":ChangeMode("FREE",maps);break;case"ORBIT":ChangeMode("ORBIT",maps);break;case"DECREASE":AdjustRadii(
maps,false);break;case"INCREASE":AdjustRadii(maps,true);break;case"CENTER":MapsToShip(maps);break;case"WAYPOINT":
waypointCommand(cmdArg,argData);break;case"PASTE":ClipboardToLog(cmdArg,argData);break;case"EXPORT":_clipboard=LogToClipboard(argData);
UpdateDisplays(true);AddMessage("Export: "+_clipboard);break;case"PROJECT":ProjectPoint(cmdArg,argData);break;case"LOG":if(cmdArg==
"BATCH"){LogBatch(argData);}else{LogWaypoint(argData,_myPos,cmdArg,"WHITE");}break;case"COLOR":if(cmdArg=="PLANET"){
SetPlanetColor(argData);}else{SetWaypointColor(argData);}break;case"MAKE":SetWaypointType(cmdArg,argData);break;case"PLOT":
PlotJumpPoint(argData);break;case"BRIGHTEN":BrightenMaps(maps,true);break;case"DARKEN":BrightenMaps(maps,false);break;case"DELETE":if
(cmdArg=="PLANET"){DeletePlanet(argData);}else{SetWaypointState(argData,3);}break;case"SYNC":sync(cmdArg,argData);break;
case"REFRESH":Build();break;case"UPDATE":case"SET":if(cmdArg.Contains("GRID")||cmdArg.Contains("TAGS"))SetGridID(argData);
else if(cmdArg.Contains("SCAN"))SetScanRange(argData);break;case"BUTTON":ButtonPress(cmdArg,argData);break;case"SCAN":
ScanPlanet(argData);break;case"RESCAN":case"RE-SCAN":ScanPlanet(argData,true);break;case"LOAD":if(cmdArg.Contains("VANILLA")||
cmdArg.Contains("PLANETS"))LoadVanillaPlanets();break;case"SCROLL":ScrollData(cmdArg,argData);break;case"IMPORT":ImportCommand
(cmdArg);break;case"CANCEL":CancelScan();break;case"CLEAR":_messages.Clear();break;default:AddMessage(
"UNRECOGNIZED COMMAND!");break;}if(maps.Count>0){foreach(StarMap cmdMap in maps){UpdateMap(cmdMap);}}}void nextLast(List<StarMap>maps,string
arg,string data,bool state){switch(arg){case"PLANET":CyclePlanetsForList(maps,state);break;case"WAYPOINT":
CycleWaypointsForList(maps,state);break;case"MODE":CycleModeForList(maps,state);break;case"PAGE":NextDataPage(data,state);break;case"MENU":
NextMenu(data,state);break;}}void ImportCommand(string type){string typeSingle;if(type=="")typeSingle="WAYPOINT";else if(type.
ToUpper().EndsWith("S"))typeSingle=type.ToUpper().Substring(0,type.Length-1);else typeSingle=type.ToUpper();AddMessage(
"Importing Coordinates of type: "+typeSingle);ImportFromLCDs(typeSingle);}void waypointCommand(string arg,string waypointName){int state=0;if(arg=="ON")
state=1;SetWaypointState(waypointName,state);}const string MENU_HEAD="Map Menu";const string MENU_TAG="[Map Menu]";const
string MENU_ID="Menu ID";const string MENU_COLOR="Menu Color Settings";const string MAP_KEY="Current Map";const string
PAGE_KEY="Current Page";const string ALIGNMENT_KEY="Alignment";const string DECAL_KEY="Decals";const string BG_KEY=
"Background Color";const string TITLE_KEY="Title Color";const string BUTTON_KEY="Button Color";const string LABEL_KEY="Label Color";const
string DATA_KEY="Current Data Display";List<MapMenu>_mapMenus;const int MENU_PAGES=6;static int _menuPageLimit;static int
_buttonCountDown;const int BUTTON_TIME=3;class MapMenu{public IMyTerminalBlock Block;public DataDisplay DataDisplay;public
IMyTextSurface Surface;public int CurrentMapIndex;public int CurrentPage;public int IDNumber;public int ActiveButton;public RectangleF
Viewport;public Color BackgroundColor;public Color TitleColor;public Color LabelColor;public Color ButtonColor;public string
Alignment;public string Decals;public MapMenu(IMyTerminalBlock block){EnsureKey(block,MENU_HEAD,MENU_ID,"");Block=block;
ActiveButton=0;int surfaceCount=GetSurfaceCount(block);int index;if(surfaceCount>1)index=ParseInt(GetKey(block,MENU_HEAD,
"Screen Index","0"),0);else index=0;Alignment=GetMenuKey(ALIGNMENT_KEY,"TOP").ToUpper();Decals=GetMenuKey(DECAL_KEY,"").ToUpper();
CurrentPage=ParseInt(GetKey(block,MENU_HEAD,PAGE_KEY,"1"),1);BackgroundColor=ParseColor(GetKey(block,MENU_COLOR,BG_KEY,"0,0,0"));
TitleColor=ParseColor(GetKey(block,MENU_COLOR,TITLE_KEY,"160,160,0"));LabelColor=ParseColor(GetKey(block,MENU_COLOR,LABEL_KEY,
"160,160,160"));ButtonColor=ParseColor(GetKey(block,MENU_COLOR,BUTTON_KEY,"0,160,160"));int mapIndex;if(_mapList.Count>1){mapIndex=
ParseInt(GetKey(block,MENU_HEAD,MAP_KEY,"0"),0);if(mapIndex>=_mapList.Count)mapIndex=0;}else mapIndex=0;CurrentMapIndex=mapIndex
;if(surfaceCount>0&&index<surfaceCount){Surface=(block as IMyTextSurfaceProvider).GetSurface(index);}else{AddMessage(
"Menu Surface could not be retrieved from block "+block.CustomName);Surface=null;}SetDataDisplay();}public void SetID(int idNumber){IDNumber=idNumber;SetKey(Block,
MENU_HEAD,MENU_ID,idNumber.ToString());}void SetDataDisplay(){if(_dataDisplays.Count<1)return;string dataKey=GetMenuKey(DATA_KEY,
"0");int dataIndex=ParseInt(dataKey,0);if(dataKey!=""&&dataIndex<_dataDisplays.Count)DataDisplay=GetDataDisplay(dataIndex);
else DataDisplay=null;}bool NewDisplayAssignment(){if(DataDisplay==null&&_dataDisplays.Count>0)return true;return false;}
public void NextDataDisplay(){bool newDisplay=NewDisplayAssignment();if(_dataDisplays.Count<2)return;int index;if(newDisplay)
index=0;else{index=DataDisplay.IDNumber+1;if(index>=_dataDisplays.Count)index=0;}DataDisplay=GetDataDisplay(index);SetKey(
Block,MENU_HEAD,DATA_KEY,index.ToString());}public void PreviousDataDisplay(){bool newDisplay=NewDisplayAssignment();if(
_dataDisplays.Count<2)return;int index;if(newDisplay){index=0;}else{index=DataDisplay.IDNumber-1;if(index<0)index=_dataDisplays.Count
-1;}DataDisplay=GetDataDisplay(index);SetKey(Block,MENU_HEAD,DATA_KEY,index.ToString());}public void InitializeSurface(){
PrepareTextSurfaceForSprites(Surface);Viewport=new RectangleF((Surface.TextureSize-Surface.SurfaceSize)/2f,Surface.SurfaceSize);}public void
PressButton(int button){ActiveButton=button;_buttonCountDown=BUTTON_TIME;}public void NextMap(){if(_mapList.Count<2)return;
CurrentMapIndex++;if(CurrentMapIndex>=_mapList.Count)CurrentMapIndex=0;SetKey(Block,MENU_HEAD,MAP_KEY,CurrentMapIndex.ToString());}
public void PreviousMap(){if(_mapList.Count<2)return;CurrentMapIndex--;if(CurrentMapIndex<0)CurrentMapIndex=_mapList.Count-1;
SetKey(Block,MENU_HEAD,MAP_KEY,CurrentMapIndex.ToString());}string GetMenuKey(string key,string defaultValue){return GetKey(
Block,MENU_HEAD,key,defaultValue);}public void NextDataPage(){if(DataDisplay==null)return;DataDisplay.NextPage();}public void
PreviousDataPage(){if(DataDisplay==null)return;DataDisplay.PreviousPage();}public void ScrollDown(){if(DataDisplay==null)return;
DataDisplay.ScrollDown();}public void ScrollUp(){if(DataDisplay==null)return;DataDisplay.ScrollUp();}}void AssignMenus(){
_buttonCountDown=0;_menuPageLimit=MENU_PAGES;if(_dataDisplays.Count<1)_menuPageLimit--;List<IMyTerminalBlock>menuBlocks=new List<
IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(MENU_TAG,menuBlocks);if(menuBlocks.Count>0){foreach(IMyTerminalBlock menuBlock
in menuBlocks){if(GetKey(menuBlock,SHARED,"Grid_ID",_gridID)==_gridID){MapMenu menu=new MapMenu(menuBlock);if(menu!=null&&
menu.Surface!=null){SetMenuID(menu);menu.InitializeSurface();_mapMenus.Add(menu);}else{AddMessage(
"MENU SURFACE ERROR! - Could not add Menu for controller \n\""+menuBlock.CustomName+"\"\n* Please check LCD Index in Custom Data for Controller.");}}}}DrawMenus();}MapMenu GetMenu(
string arg){if(_mapMenus.Count<1)return null;int menuID;try{if(arg=="")menuID=0;else menuID=ParseInt(arg.Split(' ')[0],0);}
catch{return null;}if(menuID==0)return _mapMenus[0];foreach(MapMenu menu in _mapMenus){if(menu.IDNumber==menuID)return menu;}
return null;}void NextMenu(string arg,bool next){MapMenu menu=GetMenu(arg);if(menu==null){AddMessage("No Menu "+arg+" found!")
;return;}menu.ActiveButton=0;if(next)menu.CurrentPage++;else menu.CurrentPage--;if(menu.CurrentPage>6)menu.CurrentPage=1;
else if(menu.CurrentPage<1)menu.CurrentPage=6;SetKey(menu.Block,MENU_HEAD,PAGE_KEY,menu.CurrentPage.ToString());DrawMenu(
menu);}void ButtonTimer(){if(_buttonCountDown<1)return;else if(_buttonCountDown==1)ClearButtons();_buttonCountDown--;}void
ClearButtons(){if(_mapMenus.Count<1)return;foreach(MapMenu menu in _mapMenus){if(menu.ActiveButton>0){menu.ActiveButton=0;DrawMenu(
menu);}}}void ShowMenuData(){Echo("Menus: "+_mapMenus.Count);if(_mapMenus.Count<1)return;foreach(MapMenu menu in _mapMenus){
string display;if(menu.DataDisplay!=null)display=menu.DataDisplay.IDNumber.ToString();else display="null";Echo(menu.Block.
CustomName+"\n * Data Display: "+display+"\n");}}void SetMenuID(MapMenu menu){string idString=GetKey(menu.Block,MENU_HEAD,MENU_ID,
"");int menuID;if(idString==""){menuID=_mapMenus.Count+1;menu.SetID(menuID);while(DuplicatedIDInData(menu)){menuID++;menu.
SetID(menuID);}}else{menuID=ParseInt(idString,1);menu.SetID(menuID);while(DuplicateIDInList(menu)){menuID++;menu.SetID(menuID
);}}}bool DuplicateIDInList(MapMenu menu){if(_mapMenus.Count<1)return false;foreach(MapMenu assignedMenu in _mapMenus){if
(menu.IDNumber==assignedMenu.IDNumber)return true;}return false;}bool DuplicatedIDInData(MapMenu menu){List<
IMyTerminalBlock>menuBlocks=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(MENU_TAG,menuBlocks);if(menuBlocks.Count<
1)return false;foreach(IMyTerminalBlock block in menuBlocks){string blockData=block.CustomData;if(blockData.Contains(
MENU_HEAD)&&blockData.Contains(MENU_ID)&&block!=menu.Block){MyIni blockIni=GetIni(block);string idValue=blockIni.Get(MENU_HEAD,
MENU_ID).ToString();if(idValue==menu.IDNumber.ToString())return true;}}return false;}const string SYNC_TAG="[Map Sync]";const
string IMPORT_TAG="[IMPORT]";void sync(string cmdArg,string argData){bool syncTo=argData=="OVERWRITE";if(cmdArg=="MASTER"){
syncMaster(syncTo);}else if(cmdArg=="NEAREST"){syncNearest(syncTo);}else{AddMessage("Invalid Sync Command!");}}void syncMaster(
bool syncTo){if(Me.CustomName.Contains(SYNC_TAG)){AddMessage("SYNC Requests cannot be made from SYNC terminal!");return;}
List<IMyTerminalBlock>syncBlocks=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(SYNC_TAG,syncBlocks);if(
syncBlocks.Count<1){AddMessage("NO MAP MASTER FOUND.\nPlease add tag '"+SYNC_TAG+
"' to the map computer's name on your station or capital ship.");return;}if(syncBlocks.Count>1){AddMessage("Multiple blocks found with tag '"+SYNC_TAG+
"'! Please resolve conflict before syncing.");return;}syncWith(syncBlocks[0]as IMyProgrammableBlock,syncTo);}void syncNearest(bool syncTo){List<IMyProgrammableBlock
>computers=new List<IMyProgrammableBlock>();GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(computers);
IMyProgrammableBlock syncBlock=null;float nearest=float.MaxValue;foreach(IMyProgrammableBlock computer in computers){if(computer.CustomData.
Contains("[Map Settings]")&&!(computer==Me)){float distance=Vector3.Distance(_myPos,computer.GetPosition());if(distance<nearest)
{nearest=distance;syncBlock=computer;}}}if(!(syncBlock==null)){syncWith(syncBlock,syncTo);return;}AddMessage(
"No other mapping computers available to sync!");}void syncWith(IMyProgrammableBlock syncBlock,bool syncTo){IMyTerminalBlock blockA=syncBlock as IMyTerminalBlock;
IMyTerminalBlock blockB=Me;if(syncTo){blockA=Me;blockB=syncBlock as IMyTerminalBlock;}int[]pSync=mapSync(blockA,blockB,"Planet_List");
int[]wSync=mapSync(blockA,blockB,"Waypoint_List");if(syncTo){pSync=syncReverse(pSync);wSync=syncReverse(wSync);}syncBlock.
TryRun("SYNC_ALERT "+Me.CustomName);Build();AddMessage("MAP DATA SYNCED\n-- Planets --\nDownloaded: "+pSync[0]+"\nUploaded: "+
pSync[1]+"\n\n--Waypoints--\nDownloaded: "+wSync[0]+"\nUploaded: "+wSync[1]);}int[]syncReverse(int[]input){int[]output=new
int[]{input[1],input[0]};return output;}int[]mapSync(IMyTerminalBlock mapA,IMyTerminalBlock mapB,string listName){int[]
downUp=new int[]{0,0};if(syncBlockError(mapA))return downUp;if(syncBlockError(mapB))return downUp;MyIni iniA=DataToIni(mapA);
MyIni iniB=DataToIni(mapB);string dataA=iniA.Get("Map Settings",listName).ToString();string dataB=iniB.Get("Map Settings",
listName).ToString();string newData="";if(dataA==""){newData=dataB;downUp[1]=dataB.Split('\n').Length;}else if(dataB==""){
newData=dataA;downUp[0]=dataA.Split('\n').Length;}else{List<string>outputs=dataA.Split('\n').ToList();List<string>inputs=dataB.
Split('\n').ToList();int startCount=outputs.Count;int matchCount=0;foreach(string input in inputs){string name=input.Split(
';')[0];bool matched=false;foreach(string output in outputs){if(output.StartsWith(name)){matched=true;}}if(matched)
matchCount++;else outputs.Add(input);}downUp[0]=startCount-matchCount;downUp[1]=inputs.Count-matchCount;foreach(string entry in
outputs){newData+=entry+"\n";}}iniA.Set("Map Settings",listName,newData.Trim());mapA.CustomData=iniA.ToString();iniB.Set(
"Map Settings",listName,newData.Trim());mapB.CustomData=iniB.ToString();return downUp;}bool syncBlockError(IMyTerminalBlock sync){if(
sync.CustomData.Contains("[Map Settings]")){return false;}AddMessage("SYNC Block '"+sync.CustomName+
"' contains no map settings! Please ensure that SYNC Block is also running this script!");return true;}void syncAlert(string name){Build();string senderData;IMyTerminalBlock sender;try{sender=
GridTerminalSystem.GetBlockWithName(name);senderData=GetKey(sender,SHARED,"Grid_ID",sender.CubeGrid.EntityId.ToString());}catch{senderData
="UNKNOWN";}AddMessage("Origin Grid ID: "+senderData);}void LogBatch(string arg){int number=ParseInt(arg,0);DataDisplay
display=GetDataDisplay(number);if(display.Surface==null){AddMessage("No DATA DISPLAY Screen Designated!");return;}if(display.
CurrentPage!=INPUT_PAGE){AddMessage("Please navigate to GPS INPUT page before running LOG_BATCH command.");return;}
ImportCoordinates(display.Surface);display.Surface.WriteText(display.BuildPageHeader(GPS_INPUT)+BELOW_LINE);}void ImportFromLCDs(string
type){List<IMyTextPanel>lcds=new List<IMyTextPanel>();GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds);if(lcds.Count<1
)return;foreach(IMyTextPanel lcd in lcds)if(lcd.CustomName.ToUpper().Contains(IMPORT_TAG))ImportCoordinates(lcd,type);}
void ImportCoordinates(IMyTextSurface surface,string type="WAYPOINT"){StringBuilder inputText=new StringBuilder();surface.
ReadText(inputText,false);string[]inputs=inputText.ToString().Split('\n');List<string>outputs=new List<string>();foreach(string
entry in inputs)if(entry.Contains("GPS:"))ClipboardToLog(type,entry);}Dictionary<int,string>_menuTitle=new Dictionary<int,
string>(){{1,"Systems"},{2,"Zoom"},{3,"Rotation"},{4,"Translation"},{5,"Tracking"},{6,"Data Display"}};Dictionary<int,string>
_labelA=new Dictionary<int,string>(){{1,"PLANET"},{2,"ZOOM"},{3,""},{4,""},{5,""},{6,"PAGE"}};Dictionary<int,string>_labelB=new
Dictionary<int,string>(){{1,"WAYPOINT"},{2,"RADIUS"},{3,""},{4,""},{5,""},{6,"SCROLL"}};Dictionary<int,string>_labelC=new
Dictionary<int,string>(){{1,"MAP"},{2,"MAP MODE"},{3,""},{4,""},{5,""},{6,"DATA"}};Dictionary<int,string>_labelD=new Dictionary<
int,string>(){{1,"DISPLAY"},{2,"INFO"},{3,"INFO"},{4,"INFO"},{5,""},{6,""}};Dictionary<int,string>_cmd1=new Dictionary<int,
string>(){{1,"<"},{2,"-"},{3,"<"},{4,"<"},{5,"<<"},{6,"<"}};Dictionary<int,string>_cmd2=new Dictionary<int,string>(){{1,">"},{
2,"+"},{3,">"},{4,">"},{5,">>"},{6,">"}};Dictionary<int,string>_cmd3=new Dictionary<int,string>(){{1,"<"},{2,"+"},{3,"v"}
,{4,"v"},{5,"vv"},{6,"^"}};Dictionary<int,string>_cmd4=new Dictionary<int,string>(){{1,">"},{2,"-"},{3,"^"},{4,"^"},{5,
"^^"},{6,"v"}};Dictionary<int,string>_cmd5=new Dictionary<int,string>(){{1,"<"},{2,"<"},{3,"<<"},{4,"-"},{5,"--"},{6,"<"}};
Dictionary<int,string>_cmd6=new Dictionary<int,string>(){{1,">"},{2,">"},{3,">>"},{4,"+"},{5,"++"},{6,">"}};Dictionary<int,string>
_cmd7=new Dictionary<int,string>(){{1,"cycle"},{2,"-/o"},{3,"-/o"},{4,"-/o"},{5,"STOP"},{6,""}};const string SCAN_TAG=
"[Scan Cam]";const float DV_SCAN=10;const string RANGE_KEY="Scan Range";const string BORDER_LINE=
"----------------------------------------------";IMyCameraBlock _scanCamera;static bool _scannerActive;float _scanRange;string _scanPlanet;void SetScanCamera(){
_scannerActive=false;List<IMyCameraBlock>cameras=new List<IMyCameraBlock>();GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(cameras
);if(cameras.Count<1){_scanCamera=null;return;}foreach(IMyCameraBlock camera in cameras){if(camera.CustomName.Contains(
SCAN_TAG)&&GetKey(camera,SHARED,GRID_KEY,_gridID)==_gridID){_scanCamera=camera;_scanRange=ParseFloat(GetKey(camera,PROGRAM_HEAD,
RANGE_KEY,DV_SCAN.ToString()),DV_SCAN)*1000;return;}}_scanRange=0;_scanCamera=null;}void DisplayScanData(){if(!_scannerActive)
return;string flasher;if(_lightOn)flasher=" - SCANNING -";else flasher="";Echo(BORDER_LINE+"\nPLANET SCANNER"+flasher);if(
_scanCamera==null){Echo("  Inoperable: No Scan Camera Specified.\n");}else{string planetToScan;if(_scanPlanet==_activePlanet)
planetToScan="* Rescanning Planet:\n    ";else planetToScan="* Scanning New Planet:\n    ";Echo(planetToScan+_scanPlanet);Echo("* "+
_scanCamera.CustomName+"\n  - Range: "+(_scanRange/1000).ToString("N1")+"km");string countdown;int milliseconds=_scanCamera.
TimeUntilScan(_scanRange);TimeSpan time=TimeSpan.FromMilliseconds(milliseconds);if(milliseconds<1){if(_lightOn)countdown="  -";else
countdown="  - READY";}else countdown="  - Ready in: "+time.ToString(@"hh\:mm\:ss");Echo(countdown+"\n"+BORDER_LINE+"\n");}}void
ScanPlanet(string planetName,bool resetPlanet=false){if(_scanCamera==null){AddMessage("No Scan Camera Specified!");return;}else if
(planetName==""||planetName=="0"){AddMessage("No PLANET NAME specified for SCAN!");}else if(_scannerActive){CastRay(
resetPlanet);}else{_scannerActive=true;_scanPlanet=planetName;_scanCamera.EnableRaycast=true;Planet planet=GetPlanet(planetName);if
(planet!=null)_activePlanet=planet.name;}}void CancelScan(){if(!_scannerActive)return;AddMessage("PLANET SCAN CANCELLED."
);_scannerActive=false;_scanPlanet="";_scanCamera.EnableRaycast=false;_activePlanet="";}void CastRay(bool resetPlanet){
MyDetectedEntityInfo planetInfo=_scanCamera.Raycast(_scanRange,0,0);if(planetInfo.IsEmpty()||planetInfo.Type.ToString().ToUpper()!="PLANET")
{AddMessage("Scan Missed");return;}Vector3D?contact=planetInfo.HitPosition;if(contact==null){AddMessage(
"Hit Position Error");return;}Vector3D samplePoint=(Vector3D)contact;Vector3D center=planetInfo.Position;double radius=Vector3D.Distance(
samplePoint,center);if(_scanPlanet==_activePlanet)UpdatePlanetFromCast(planetInfo,center,(float)radius,resetPlanet);else
NewPlanetFromCast(planetInfo,center,(float)radius);}void NewPlanetFromCast(MyDetectedEntityInfo planetInfo,Vector3D center,float radius){
string planetString=_scanPlanet+";"+Vector3ToString(center)+";"+radius.ToString("0.#")+";GRAY;;;;;1";_planetList.Add(new
Planet(planetString));DisplayScannedPlanet(planetInfo,true);DisableScanner();DataToLog();}void UpdatePlanetFromCast(
MyDetectedEntityInfo planetInfo,Vector3D center,float radius,bool resetPlanet){Planet planet=GetPlanet(_activePlanet);if(planet==null){
AddMessage("Rescanning Error for planet: "+_activePlanet);return;}if(planet.SampleCount==0)planet.SampleCount=1;float oldRadius=
planet.radius;float newRadius;if(resetPlanet){newRadius=radius;planet.SampleCount=1;}else{newRadius=((oldRadius*planet.
SampleCount)+radius)/(planet.SampleCount+1);planet.SampleCount++;}float difference=(newRadius-oldRadius)/1000;DisplayScannedPlanet(
planetInfo,false);AddMessage("Radius change of "+difference.ToString("0.#")+"km");DataToLog();DisableScanner();}void
DisplayScannedPlanet(MyDetectedEntityInfo planetInfo,bool newPlanet){if(newPlanet)AddMessage("New Planet Logged:\n  "+_scanPlanet);else
AddMessage("Planet Updated:\n  "+_scanPlanet);}void DisableScanner(){_scannerActive=false;_scanPlanet="";_scanCamera.EnableRaycast
=false;}void SetScanRange(string arg){if(arg==""||arg.ToUpper()=="DEFAULT")_scanRange=DV_SCAN*1000;else _scanRange=
ParseFloat(arg,DV_SCAN)*1000;string range=(_scanRange/1000).ToString("0.###");AddMessage("Scan Range set to "+range+"km");SetKey(
_scanCamera,PROGRAM_HEAD,RANGE_KEY,range);if(_scanRange<0)_scanRange*=-1;}const string MAP_HEADER="MAP DISPLAY";const string
MODE_KEY="Mode";const string CENTER_KEY="Center";const string AZ_KEY="Azimuth";const string ALT_KEY="Altitude";const string
ZOOM_KEY="Focal Length";const string RADIUS_KEY="Rotational Radius";const string MOTION_KEY="Motion Vector";const string
INFO_KEY="Show Info";const string GPS_KEY="GPS Mode";const string SHIP_KEY="Show Ship";const string PLANET_KEY="Selected Planet"
;const string WAYPOINT_KEY="Selected Waypoint";const string BRIGHTNESS_KEY="Brightness";const string FOCAL_KEY=
"Focal Length";const string ORIGIN="(0,0,0)";const string DV_MOTION="0,0,0,0";static List<StarMap>_mapList;class StarMap{public
IMyTerminalBlock Block;public string Header;public IMyTextSurface DrawingSurface;public RectangleF Viewport;public Vector3 Center;public
string Mode;public int Altitude;public int Azimuth;public int RotationalRadius;public int FocalLength;public int Number;public
int Index;public int dX;public int dY;public int dZ;public int dAz;public int GpsState;public float BrightnessMod;public
bool ShowNames;public bool ShowShip;public bool ShowInfo;public int PlanetIndex;public int WaypointIndex;public string
ActivePlanetName;public string ActiveWaypointName;public string GpsMode;public Planet ActivePlanet;public Waypoint ActiveWaypoint;public
float ZoomMod;MyIni Ini;public StarMap(IMyTerminalBlock block,int screenIndex,string header){Block=block;Ini=GetIni(Block);
Header=header;PlanetIndex=0;WaypointIndex=-1;Index=screenIndex;Mode=GetMapKey(MODE_KEY,"FREE");Center=StringToVector3(
GetMapKey(CENTER_KEY,ORIGIN));Azimuth=ParseInt(GetMapKey(AZ_KEY,"0"),0);Altitude=ParseInt(GetMapKey(ALT_KEY,DV_ALTITUDE.ToString(
)),DV_ALTITUDE);FocalLength=ParseInt(GetMapKey(ZOOM_KEY,DV_FOCAL.ToString()),DV_FOCAL);RotationalRadius=ParseInt(
GetMapKey(RADIUS_KEY,DV_RADIUS.ToString()),DV_RADIUS);ZoomMod=1;string[]movements=GetMapKey(MOTION_KEY,DV_MOTION).Split(',');if(
movements.Length<4)movements=new string[]{"0","0","0","0"};dX=ParseInt(movements[0],0);dY=ParseInt(movements[1],0);dZ=ParseInt(
movements[2],0);dAz=ParseInt(movements[3],0);ShowInfo=ParseBool(GetMapKey(INFO_KEY,"True"));GpsMode=GetMapKey(GPS_KEY,"NORMAL").
ToUpper();GpsModeToState();ShowShip=ParseBool(GetMapKey(SHIP_KEY,"True"));ShowNames=true;ActivePlanetName=GetMapKey(PLANET_KEY,
"");ActivePlanet=GetPlanet(ActivePlanetName);ActiveWaypointName=GetMapKey(WAYPOINT_KEY,"");ActiveWaypoint=GetWaypoint(
ActiveWaypointName);BrightnessMod=ParseFloat(GetMapKey(BRIGHTNESS_KEY,"1"),1);}void Yaw(int angle){if(Mode.ToUpper()=="PLANET"||Mode.
ToUpper()=="CHASE"||Mode.ToUpper()=="ORBIT"){AddMessage("Yaw controls locked in PLANET, CHASE & ORBIT modes.");return;}Azimuth=
DegreeAdd(Azimuth,angle);SetMapKey(AZ_KEY,Azimuth.ToString());}void Pitch(int angle){if(Mode.ToUpper()!="PLANET"||Mode.ToUpper()
=="ORBIT"){int newAngle=DegreeAdd(Altitude,angle);if(newAngle>MAX_PITCH){newAngle=MAX_PITCH;}else if(newAngle<-MAX_PITCH){
newAngle=-MAX_PITCH;}Altitude=newAngle;}else{AddMessage("Pitch controls locked in PLANET & ORBIT modes.");}SetMapKey(ALT_KEY,
Altitude.ToString());}public void Rotate(string direction){switch(direction){case"LEFT":Yaw(ANGLE_STEP);break;case"RIGHT":Yaw(-
ANGLE_STEP);break;case"UP":Pitch(-ANGLE_STEP);break;case"DOWN":Pitch(ANGLE_STEP);break;}}public void Move(string direction){float
step=(float)MOVE_STEP;float x=0;float y=0;float z=0;switch(direction){case"LEFT":x=step;break;case"RIGHT":x=-step;break;case
"UP":y=step;break;case"DOWN":y=-step;break;case"FORWARD":case"FORWARDS":z=step;break;case"BACKWARD":case"BACKWARDS":z=-step;
break;}Vector3 moveVector=new Vector3(x,y,z);if(Mode=="FREE"||Mode=="WORLD"){Center+=rotateMovement(moveVector,this);}else{
AddMessage("Translation controls only available in FREE & WORLD modes.");}SetMapKey(CENTER_KEY,Vector3ToString(Center));}public
void Track(string direction){switch(direction){case"LEFT":dX+=MOVE_STEP;break;case"RIGHT":dX-=MOVE_STEP;break;case"UP":dY+=
MOVE_STEP;break;case"DOWN":dY-=MOVE_STEP;break;case"FORWARD":case"FORWARDS":dZ+=MOVE_STEP;break;case"BACKWARD":case"BACKWARDS":dZ
-=MOVE_STEP;break;default:AddMessage("Error with Track Command");break;}UpdateData();}public void Spin(string direction){
int deltaAz=ANGLE_STEP/2;if(direction=="RIGHT")deltaAz*=-1;dAz+=deltaAz;UpdateData();}public void Zoom(bool zoomIn){if(Mode
=="ORBIT"){if(zoomIn)ZoomMod/=ZOOM_STEP;else ZoomMod*=ZOOM_STEP;return;}int doF=FocalLength;float newScale;if(zoomIn){
newScale=doF*ZOOM_STEP;}else{newScale=doF/ZOOM_STEP;}if(newScale>ZOOM_MAX){doF=ZOOM_MAX;}else if(newScale<1){doF=1;}else{doF=(
int)newScale;}FocalLength=doF;SetMapKey(ZOOM_KEY,FocalLength.ToString());}public void AdjustRadius(bool increase){int
radius=RotationalRadius;if(increase){radius*=2;}else{radius/=2;}if(radius<FocalLength){radius=FocalLength;}else if(radius>
MAX_VALUE){radius=MAX_VALUE;}RotationalRadius=radius;SetMapKey(RADIUS_KEY,RotationalRadius.ToString());}public void Stop(){dX=0;
dY=0;dZ=0;dAz=0;UpdateData();}public string GpsStateToMode(){switch(GpsState){case 0:GpsMode="OFF";break;case 1:GpsMode=
"NORMAL";break;case 2:GpsMode="SHOW_ACTIVE";break;default:GpsMode="ERROR";break;}return GpsMode;}public void GpsModeToState(){
switch(GpsMode){case"OFF":case"FALSE":GpsState=0;break;case"SHOW_ACTIVE":GpsState=2;break;default:GpsState=1;break;}}public
void DefaultView(){Mode="FREE";Center=new Vector3(0,0,0);FocalLength=DV_FOCAL;if(Viewport.Width>500){FocalLength*=3;}
RotationalRadius=DV_RADIUS;Azimuth=0;Altitude=DV_ALTITUDE;UpdateData();}public void Brighten(bool brighten){if(brighten){BrightnessMod+=
BRIGHTNESS_STEP;if(BrightnessMod>BRIGHTNESS_LIMIT)BrightnessMod=BRIGHTNESS_LIMIT;}else{BrightnessMod-=BRIGHTNESS_STEP;if(BrightnessMod<
BRIGHTNESS_STEP)BrightnessMod=BRIGHTNESS_STEP;}SetMapKey(BRIGHTNESS_KEY,BrightnessMod.ToString());}public void CycleActiveWaypointType(
){if(ActiveWaypoint==null||Center!=ActiveWaypoint.position){AddMessage("Please Align Map "+Number+
" to desired waypoint before cycling waypoint type.");return;}ActiveWaypoint.CycleType();}public void UpdateBasicParameters(){Ini.Set(Header,MODE_KEY,Mode);Ini.Set(Header,
CENTER_KEY,Vector3ToString(Center));Ini.Set(Header,AZ_KEY,Azimuth.ToString());Ini.Set(Header,ALT_KEY,Altitude.ToString());Ini.Set(
Header,ZOOM_KEY,FocalLength.ToString());Ini.Set(Header,RADIUS_KEY,RotationalRadius.ToString());Ini.Set(Header,INFO_KEY,
ShowInfo.ToString());Ini.Set(Header,GPS_KEY,GpsMode);Ini.Set(Header,SHIP_KEY,ShowShip.ToString());Ini.Set(Header,PLANET_KEY,
ActivePlanetName);Ini.Set(Header,WAYPOINT_KEY,ActiveWaypointName);Ini.Set(Header,BRIGHTNESS_KEY,BrightnessMod.ToString());Ini.Set(Header
,FOCAL_KEY,FocalLength.ToString());Block.CustomData=Ini.ToString();}public void UpdateData(){Block.CustomData=Ini.
ToString();}public void SetMapKey(string key,string value){Ini.Set(Header,key,value);UpdateData();}public string GetMapKey(
string key,string defaultValue){EnsureMapKey(key,defaultValue);return Ini.Get(Header,key).ToString();}void EnsureMapKey(string
key,string defaultVal){if(!Ini.ContainsKey(Header,key))SetMapKey(key,defaultVal);}public void SetActivePlanet(Planet planet
){ActivePlanet=planet;ActivePlanetName=planet.name;Ini.Set(Header,PLANET_KEY,ActivePlanetName);Center=planet.position;if(
planet.radius<27000){FocalLength*=4;}else if(planet.radius<40000){FocalLength*=3;FocalLength/=2;}UpdateBasicParameters();}
public void SetActiveWaypoint(Waypoint waypoint){ActiveWaypoint=waypoint;ActiveWaypointName=waypoint.name;Ini.Set(Header,
WAYPOINT_KEY,ActiveWaypointName);Center=waypoint.position;ActiveWaypoint=waypoint;ActiveWaypointName=waypoint.name;
UpdateBasicParameters();}}List<StarMap>ParametersToMaps(IMyTerminalBlock mapBlock){List<StarMap>mapsOut=new List<StarMap>();List<string>
headers=new List<string>();List<int>indexes=new List<int>();int surfaceCount=(mapBlock as IMyTextSurfaceProvider).SurfaceCount;
if(surfaceCount<1){AddMessage("Block \""+mapBlock.CustomName+"\" has no screens!");return mapsOut;}else if(surfaceCount==1
){headers.Add(MAP_HEADER);indexes.Add(0);}else{string defaultBool="True";for(int i=0;i<surfaceCount;i++){if(ParseBool(
GetKey(mapBlock,"MAP DISPLAYS","Show On Screen "+i,defaultBool))){headers.Add(MAP_HEADER+" "+i);indexes.Add(i);}defaultBool=
"False";}}for(int j=0;j<headers.Count;j++){mapsOut.Add(new StarMap(mapBlock,indexes[j],headers[j]));}return mapsOut;}List<
StarMap>ArgToMaps(string arg){if(arg.ToUpper()=="ALL"){return _mapList;}List<StarMap>mapsToEdit=new List<StarMap>();string[]
args=arg.Split(',');foreach(string argValue in args){int number;bool success=Int32.TryParse(argValue,out number);if(success)
{if(number<_mapList.Count){mapsToEdit.Add(_mapList[number]);}}}return mapsToEdit;}bool NoMaps(List<StarMap>maps){if(maps.
Count<1){AddMessage("No relevant maps found! Check arguments!");return true;}return false;}StarMap GetMap(int mapNumber){if(
mapNumber>-1&&mapNumber<_mapList.Count)return _mapList[mapNumber];else return null;}const float PI=(float)Math.PI;static int
GetSurfaceCount(IMyTerminalBlock block){try{return(block as IMyTextSurfaceProvider).SurfaceCount;}catch{return 0;}}static int ParseInt(
string arg,int defaultValue){int number;if(int.TryParse(arg,out number))return number;else return defaultValue;}static float
ParseFloat(string arg,float defaultValue){float number;if(float.TryParse(arg,out number))return number;else return defaultValue;}
static bool ParseBool(string val){string uVal=val.ToUpper();if(uVal=="TRUE"||uVal=="T"||uVal=="1"){return true;}return false;}
static Color ParseColor(string colorString){UInt16 red,green,blue;red=green=blue=0;string[]values=colorString.Split(',');if(
values.Length>2){UInt16.TryParse(values[0],out red);UInt16.TryParse(values[1],out green);UInt16.TryParse(values[2],out blue);}
return new Color(red,green,blue);}static double ToRadians(int angle){double radianValue=(double)angle*Math.PI/180;return
radianValue;}float ToDegrees(float angle){float degreeValue=angle*180/(float)Math.PI;return degreeValue;}static int DegreeAdd(int
angle_A,int angle_B){int angleOut=angle_A+angle_B;if(angleOut>180){angleOut-=360;}else if(angleOut<-179){angleOut+=360;}return
angleOut;}const int ANGLE_STEP=5;const int MAX_PITCH=90;const int MOVE_STEP=5000;const float ZOOM_STEP=1.5f;const int ZOOM_MAX=
1000000000;const float BRIGHTNESS_STEP=0.25f;void ZoomMaps(List<StarMap>maps,string direction){if(NoMaps(maps))return;bool zoomIn=
direction=="IN";foreach(StarMap map in maps)map.Zoom(zoomIn);}void AdjustRadii(List<StarMap>maps,bool increase){if(NoMaps(maps))
return;foreach(StarMap map in maps){map.AdjustRadius(increase);}}void MoveMaps(List<StarMap>maps,string direction){if(NoMaps(
maps))return;foreach(StarMap map in maps){map.Move(direction);}}void TrackMaps(List<StarMap>maps,string direction){if(NoMaps
(maps))return;foreach(StarMap map in maps){map.Track(direction);}}void RotateMaps(List<StarMap>maps,string direction){if(
NoMaps(maps))return;foreach(StarMap map in maps){map.Rotate(direction);}}void SpinMaps(List<StarMap>maps,string direction){if(
NoMaps(maps))return;foreach(StarMap map in maps){map.Spin(direction);}}void StopMaps(List<StarMap>maps){if(NoMaps(maps))return
;foreach(StarMap map in maps){map.Stop();}}void MapsToShip(List<StarMap>maps){if(NoMaps(maps))return;foreach(StarMap map
in maps){map.Center=_myPos;}}void CenterWorld(StarMap map){map.Altitude=-15;map.Azimuth=45;map.FocalLength=256;map.
RotationalRadius=4194304;Vector3 worldCenter=new Vector3(0,0,0);if(_planets){foreach(Planet planet in _planetList){worldCenter+=planet.
position;}worldCenter/=_planetList.Count;}map.Center=worldCenter;}void CenterShip(StarMap map){map.DefaultView();map.Center=
_myPos;map.SetMapKey(CENTER_KEY,Vector3ToString(map.Center));}void AlignShip(StarMap map){Vector3 heading=_refBlock.
WorldMatrix.Forward;int newAz=DegreeAdd((int)ToDegrees((float)Math.Atan2(heading.Z,heading.X)),-90);int newAlt=(int)ToDegrees((
float)Math.Asin(heading.Y));if(newAlt<-90){newAlt=DegreeAdd(newAlt,180);newAz=DegreeAdd(newAz,180);}map.Altitude=newAlt;map.
Azimuth=newAz;map.Center=_myPos;}void AlignOrbit(StarMap map){if(_planetList.Count<1){return;}if(map.ActivePlanet==null){if(
_nearestPlanet==null){Echo("No Nearest Planet Set!");return;}map.SetActivePlanet(_nearestPlanet);}Vector3 planetPos=map.ActivePlanet.
position;map.Center=(_myPos+planetPos)*0.75f;map.Altitude=0;Vector3 orbit=_myPos-planetPos;map.Azimuth=(int)ToDegrees((float)
Math.Abs(Math.Atan2(orbit.Z,orbit.X)+PI*0.75f));float span=orbit.Length();map.FocalLength=DV_FOCAL;double newRadius=1.25f*
map.FocalLength*span/map.Viewport.Height*map.ZoomMod;if(newRadius>MAX_VALUE||newRadius<0){newRadius=MAX_VALUE;double
newZoom=0.8f*map.Viewport.Height*(MAX_VALUE/span);map.FocalLength=(int)newZoom;}map.RotationalRadius=(int)newRadius;}void
PlanetMode(StarMap map){map.Mode="PLANET";map.RotationalRadius=DV_RADIUS;map.FocalLength=DV_FOCAL;if(map.Viewport.Width>500){map.
FocalLength*=4;}if(_planets){SortByNearest(_planetList);map.ActivePlanet=_planetList[0];ShipToPlanet(map);if(map.ActivePlanet.
radius<30000){map.FocalLength*=4;}}map.Stop();SetKey(map.Block,map.Header,ZOOM_KEY,map.FocalLength.ToString());SetKey(map.
Block,map.Header,RADIUS_KEY,map.RotationalRadius.ToString());SetKey(map.Block,map.Header,PLANET_KEY,map.ActivePlanetName);}
void SetMapMode(StarMap map,string mapMode){if(mapMode=="WORLD"){CenterWorld(map);}else{CenterShip(map);}if(mapMode==
"PLANET"){PlanetMode(map);}else if(mapMode=="ORBIT"){AlignOrbit(map);}else if(mapMode=="CHASE"){AlignShip(map);}map.Mode=mapMode
;map.SetMapKey(MODE_KEY,mapMode);}void ChangeMode(string mapMode,List<StarMap>maps){if(NoMaps(maps))return;foreach(
StarMap map in maps){SetMapMode(map,mapMode);}}void CycleMode(StarMap map,bool cycleUp){_activePlanet="";string[]modes={"FREE",
"SHIP","CHASE","PLANET","ORBIT","WORLD"};int length=modes.Length;int modeIndex=0;for(int i=0;i<length;i++){if(map.Mode.ToUpper
()==modes[i]){modeIndex=i;}}if(cycleUp){modeIndex++;}else{modeIndex--;}if(modeIndex>=length){modeIndex=0;}else if(
modeIndex<0){modeIndex=length-1;}SetMapMode(map,modes[modeIndex]);}void CycleModeForList(List<StarMap>maps,bool cycleUp){if(
NoMaps(maps))return;foreach(StarMap map in maps){CycleMode(map,cycleUp);}}void ShipToPlanet(StarMap map){if(_planets){Planet
planet=_nearestPlanet;Vector3 shipVector=_myPos-planet.position;float magnitude=Vector3.Distance(_myPos,planet.position);float
azAngle=(float)Math.Atan2(shipVector.Z,shipVector.X);float altAngle=(float)Math.Asin(shipVector.Y/magnitude);map.Center=planet.
position;map.Azimuth=DegreeAdd((int)ToDegrees(azAngle),90);map.Altitude=(int)ToDegrees(-altAngle);}}void MapsToDefault(List<
StarMap>maps){if(maps.Count<1)return;foreach(StarMap map in maps){map.DefaultView();}}void BrightenMaps(List<StarMap>maps,bool
increaseBrightness){if(NoMaps(maps))return;foreach(StarMap map in maps)map.Brighten(increaseBrightness);}