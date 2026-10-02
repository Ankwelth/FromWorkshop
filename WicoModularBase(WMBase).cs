/*
 * Wico Modular
 * Base Connectors
 * 
 * March 21, 2020
 * May 24, 2020
 * October 11, 2020
 * 
 * Handles Connectors on a base.
 * 
 * Connectors to mananage must have [BASE] in the name or customdata
 * 
 * https://steamcommunity.com/sharedfiles/filedetails/?id=2035493398
 * 
 * 
 */
BaseConnectors baseConnectors;Asteroids _asteroids;OreInfoLocs _oreInfoLocs;Displays _displays;WicoControl _wicoControl;
Timers _timers;PowerProduction _power;GasTanks _tanks;PowerManagement _powerManagement;void ModuleControlInit(){_wicoControl=
new WicoControl(this,wicoIGC);}void ModuleProgramInit(){_timers=new Timers(this,wicoBlockMaster);baseConnectors=new
BaseConnectors(this,wicoBlockMaster,wicoIGC,wicoElapsedTime,_timers);_displays=new Displays(this,wicoBlockMaster,wicoElapsedTime);
_asteroids=new Asteroids(this,_wicoControl,wicoIGC,_displays);_oreInfoLocs=new OreInfoLocs(this,wicoBlockMaster,wicoIGC,_asteroids
,_displays);_power=new PowerProduction(this,wicoBlockMaster);_tanks=new GasTanks(this,wicoBlockMaster);_powerManagement=
new PowerManagement(this,_wicoControl,_power,_tanks,wicoElapsedTime,wicoIGC,_displays);}void ModulePreMain(string argument,
UpdateType updateSource){}void ModulePostMain(){if(bInitDone){_displays.EchoInfo();_wicoControl.WantSlow();}}class Asteroids{
Program _program;WicoControl _wicoControl;WicoIGC _wicoIGC;Displays _displays;public Asteroids(Program program,WicoControl
wicoControl,WicoIGC wicoIGC,Displays displays){_program=program;_wicoControl=wicoControl;_wicoIGC=wicoIGC;_displays=displays;
_program.AddLoadHandler(LoadHandler);_program.AddSaveHandler(SaveHandler);_wicoIGC.AddPublicHandler(sAsteroidTag,
BroadcastHandler);_displays.AddSurfaceHandler("ASTEROIDS",SurfaceHandler);}StringBuilder sbNotices=new StringBuilder(300);StringBuilder
sbModeInfo=new StringBuilder(100);public void SurfaceHandler(string tag,IMyTextSurface tsurface,int ActionType){if(tag==
"ASTEROIDS"){if(ActionType==Displays.DODRAW){sbNotices.Clear();sbModeInfo.Clear();sbModeInfo.AppendLine(asteroidsInfo.Count+
" Known Asteroids");foreach(var ai in asteroidsInfo){sbNotices.AppendLine(" "+(_program.Me.GetPosition()-ai.Position).Length().ToString(
"N0")+" Meters");}tsurface.WriteText(sbModeInfo);if(tsurface.SurfaceSize.Y<512){}else{tsurface.WriteText(sbNotices,true);}}
else if(ActionType==Displays.SETUPDRAW){tsurface.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;tsurface.
WriteText("");if(tsurface.SurfaceSize.Y<512){tsurface.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.CENTER;tsurface.FontSize=2
;}else{tsurface.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.LEFT;tsurface.FontSize=1.5f;}}else if(ActionType==
Displays.CLEARDISPLAY){tsurface.WriteText("");}}}void LoadHandler(MyIni Ini){int iCount=0;iCount=Ini.Get(sAsteroidSection,
"count").ToInt32(0);asteroidsInfo.Clear();Vector3D v3D;long eId=0;for(int j1=0;j1<iCount;j1++){eId=Ini.Get(sAsteroidSection,
"EntityId"+j1.ToString()).ToInt32(0);if(eId<=0)continue;BoundingBoxD box=new BoundingBoxD();Vector3D.TryParse(Ini.Get(
sAsteroidSection,"BBMin"+j1.ToString()).ToString(),out v3D);box.Min=v3D;Vector3D.TryParse(Ini.Get(sAsteroidSection,"BBMax"+j1.ToString()
).ToString(),out v3D);box.Max=v3D;AsteroidInfo ast=new AsteroidInfo{EntityId=eId,BoundingBox=box};asteroidsInfo.Add(ast);
}}void SaveHandler(MyIni Ini){var count=asteroidsInfo.Count;Ini.Set(sAsteroidSection,"count",count);for(int i1=0;i1<
asteroidsInfo.Count;i1++){Ini.Set(sAsteroidSection,"EntityId"+i1.ToString(),asteroidsInfo[i1].EntityId.ToString());Ini.Set(
sAsteroidSection,"BBMin"+i1.ToString(),_program.Vector3DToString(asteroidsInfo[i1].BoundingBox.Min));Ini.Set(sAsteroidSection,"BBMax"+i1
.ToString(),_program.Vector3DToString(asteroidsInfo[i1].BoundingBox.Max));}}void BroadcastHandler(MyIGCMessage msg){if(
msg.Tag==sAsteroidTag){if(msg.Data is string){string[]aMessage=((string)msg.Data).Trim().Split(':');double x1,y1,z1;int
iOffset=0;long id=0;long.TryParse(aMessage[iOffset++],out id);long asteroidID=0;long.TryParse(aMessage[iOffset++],out
asteroidID);x1=Convert.ToDouble(aMessage[iOffset++]);y1=Convert.ToDouble(aMessage[iOffset++]);z1=Convert.ToDouble(aMessage[iOffset
++]);Vector3D vMin=new Vector3D(x1,y1,z1);x1=Convert.ToDouble(aMessage[iOffset++]);y1=Convert.ToDouble(aMessage[iOffset++]
);z1=Convert.ToDouble(aMessage[iOffset++]);Vector3D vMax=new Vector3D(x1,y1,z1);BoundingBoxD box=new BoundingBoxD(vMin,
vMax);AsteroidAdd(asteroidID,box,false);}}}List<AsteroidInfo>asteroidsInfo=new List<AsteroidInfo>();const string
sAsteroidSection="ASTEROIDS";string sAsteroidTag="WICOAST";public class AsteroidInfo{public long EntityId;public BoundingBoxD
BoundingBox;public Vector3D Position{get{return BoundingBox.Center;}}}void AsteroidAdd(long entityid,BoundingBoxD box,bool
bTransmitAsteroid=true){bool bFound=false;if(entityid<=0)return;for(int i=0;i<asteroidsInfo.Count;i++){if(asteroidsInfo[i].EntityId==
entityid){bFound=true;break;}}if(!bFound){AsteroidInfo ai=new AsteroidInfo();ai.EntityId=entityid;ai.BoundingBox=box;
asteroidsInfo.Add(ai);if(bTransmitAsteroid){_program.IGC.SendBroadcastMessage(sAsteroidTag,_program.Me.EntityId.ToString()+":"+
entityid.ToString()+":"+_program.Vector3DToString(box.Min)+":"+_program.Vector3DToString(box.Max));}}}public void AsteroidAdd(
MyDetectedEntityInfo thisDetectedInfo,bool bTransmitAsteroid=true){if(thisDetectedInfo.IsEmpty()||thisDetectedInfo.Type!=
MyDetectedEntityType.Asteroid)return;AsteroidAdd((long)thisDetectedInfo.EntityId,thisDetectedInfo.BoundingBox,bTransmitAsteroid);}public
bool AsteroidProcessLDEI(List<MyDetectedEntityInfo>lmyDEI){bool bFoundAsteroid=false;for(int j=0;j<lmyDEI.Count;j++){if(
lmyDEI[j].Type==MyDetectedEntityType.Asteroid){if(AsteroidProcessDEI(lmyDEI[j]))bFoundAsteroid=true;}}return bFoundAsteroid;}
public bool AsteroidProcessDEI(MyDetectedEntityInfo dei){bool bFoundAsteroid=false;if(dei.Type==MyDetectedEntityType.Asteroid)
{AsteroidAdd(dei);bFoundAsteroid=true;}return bFoundAsteroid;}public long AsteroidFindNearest(bool bInsideOnly=false){
long AsteroidID=-1;double distanceSQ=double.MaxValue;foreach(var ast in asteroidsInfo){if(ast.EntityId<=0)continue;if(
bInsideOnly){if(ast.BoundingBox.Contains(_program.Me.GetPosition())==ContainmentType.Contains){AsteroidID=ast.EntityId;}}else{
double curDistanceSQ=Vector3D.DistanceSquared(ast.Position,_program.Me.GetPosition());if(curDistanceSQ<distanceSQ){AsteroidID=
ast.EntityId;distanceSQ=curDistanceSQ;}}}return AsteroidID;}public Vector3D AsteroidGetPosition(long AsteroidID){Vector3D
pos=new Vector3D(0,0,0);for(int i=0;i<asteroidsInfo.Count;i++){if(asteroidsInfo[i].EntityId==AsteroidID)pos=asteroidsInfo[i
].Position;}return pos;}public BoundingBoxD AsteroidGetBB(long AsteroidID){BoundingBoxD box=new BoundingBoxD();for(int i=
0;i<asteroidsInfo.Count;i++){if(asteroidsInfo[i].EntityId==AsteroidID){box=asteroidsInfo[i].BoundingBox;break;}}return
box;}}class BaseConnectors{Program _program;WicoBlockMaster _wicoBlockMaster;WicoIGC _wicoIGC;WicoElapsedTime
_wicoElapsedTime;Timers _timers;string _BaseTransmit="BaseTransmit";string DockStartTag="Dock Start";string DockEndTag="Dock End";bool
bHaveIncoming=false;public BaseConnectors(Program program,WicoBlockMaster wbm,WicoIGC wicoIGC,WicoElapsedTime wicoElapsedTime,Timers
timers){_program=program;_wicoBlockMaster=wbm;_wicoIGC=wicoIGC;_wicoElapsedTime=wicoElapsedTime;_timers=timers;_program.
moduleName+=" Base Connectors";_program.moduleList+="\nBase Connectors V4.2c";_program.AddUpdateHandler(UpdateHandler);_program.
AddTriggerHandler(ProcessTrigger);_program.AddLoadHandler(LoadHandler);_program.AddSaveHandler(SaveHandler);_program.AddPostInitHandler(
PostInitHandler());_wicoBlockMaster.AddLocalBlockChangedHandler(LocalGridChangedHandler);_wicoBlockMaster.AddLocalBlockHandler(
BlockParseHandler);_wicoIGC.AddPublicHandler("BASE?",BroadcastHandler);_wicoIGC.AddPublicHandler("CON?",BroadcastHandler);_wicoIGC.
AddPublicHandler("COND?",BroadcastHandler);_wicoElapsedTime.AddTimer(_BaseTransmit,55,BaseTransmitTimerHandler);_wicoElapsedTime.
StartTimer(_BaseTransmit);}void ResetMotionHandler(bool bNoDrills=false){}void LoadHandler(MyIni Ini){}void SaveHandler(MyIni Ini)
{}public IEnumerator<bool>PostInitHandler(){yield return true;}public void ModeChangeHandler(int fromMode,int fromState,
int toMode,int toState){}void ModeInitHandler(){}List<IMyTerminalBlock>_localConnectors=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>_localDockConnectors=new List<IMyTerminalBlock>();List<IMyTerminalBlock>_localBaseConnectors=new List<IMyTerminalBlock>
();List<IMyTerminalBlock>_localEjectors=new List<IMyTerminalBlock>();List<IMyTerminalBlock>_allLights=new List<
IMyTerminalBlock>();public void BlockParseHandler(IMyTerminalBlock tb){if(tb is IMyShipConnector){if(tb.BlockDefinition.SubtypeName==
"ConnectorSmall)"){_localEjectors.Add(tb);}else{_localConnectors.Add(tb);if(tb.CustomName.Contains("[DOCK]")||tb.CustomData.Contains(
"[DOCK]"))_localDockConnectors.Add(tb);if(tb.CustomName.Contains("[BASE]")||tb.CustomData.Contains("[BASE]"))
_localBaseConnectors.Add(tb);}}else if(tb is IMyLightingBlock){_allLights.Add(tb);}}void LocalGridChangedHandler(){_localEjectors.Clear();
_localConnectors.Clear();_localDockConnectors.Clear();_localBaseConnectors.Clear();_allLights.Clear();dockingInfo.Clear();}public void
ProcessTrigger(string sArgument,MyCommandLine myCommandLine,UpdateType updateSource){}void UpdateHandler(UpdateType updateSource){
_program.Echo("Base Connectors="+_localBaseConnectors.Count.ToString());if(dockingInfo.Count<1&&_localBaseConnectors.Count>0){
foreach(var tb in _localBaseConnectors){addDockingInfo(tb);}}processDockingStates();}void BroadcastHandler(MyIGCMessage msg){if
(msg.Tag=="BASE?"){_program.Echo("Base Request");string sMessage=(string)msg.Data;string[]aMessage=sMessage.Trim().Split(
':');long incomingID=0;bool pOK=false;pOK=long.TryParse(aMessage[0],out incomingID);doBaseAnnounce(true);}if(msg.Tag==
"CON?"){_program.Echo("Connector Approach Request!");string sMessage=(string)msg.Data;string[]aMessage=sMessage.Trim().Split(
':');int iOffset=0;bool pOK=false;long baseID=0;pOK=long.TryParse(aMessage[iOffset++],out baseID);if(baseID!=_program.Me.
EntityId){_program.Echo("Not our approach request");return;}string sType=aMessage[iOffset++];double height=-1;double width=-1;
double length=-1;string[]aSize=sType.Trim().Split(',');if(aSize.Length>2){pOK=double.TryParse(aSize[0],out height);pOK=double.
TryParse(aSize[1],out width);pOK=double.TryParse(aSize[2],out length);}string sDroneName=aMessage[iOffset++];int i=-1;long
incomingID=0;pOK=long.TryParse(aMessage[iOffset++],out incomingID);i=getAvailableDock(incomingID,sType,height,width,length);if(i>=
0&&pOK){_program.Echo("Sending Dock Info");sendDockInfo(i,incomingID,sDroneName,true);_timers.TimerTriggers(DockStartTag)
;bHaveIncoming=true;}else{_program.Echo("Sending Dock Fail");_program.IGC.SendBroadcastMessage("CONF",incomingID+":"+
_wicoBlockMaster.GetShipName().ToString()+":"+_program.Me.EntityId.ToString()+":"+_program.Vector3DToString(_program.Me.GetPosition()));
}}if(msg.Tag=="COND?"){_program.Echo("Connector Dock Request!");string sMessage=(string)msg.Data;string[]aMessage=
sMessage.Trim().Split(':');int iOffset=0;bool pOK=false;long baseID=0;pOK=long.TryParse(aMessage[iOffset++],out baseID);if(
baseID!=_program.Me.EntityId){return;}string sType=aMessage[iOffset++];double height=-1;double width=-1;double length=-1;
string[]aSize=sType.Trim().Split(',');if(aSize.Length>2){pOK=double.TryParse(aSize[0],out height);pOK=double.TryParse(aSize[1]
,out width);pOK=double.TryParse(aSize[2],out length);}string sDroneName=aMessage[iOffset++];int i=-1;long incomingID=0;
pOK=long.TryParse(aMessage[iOffset++],out incomingID);i=getAvailableDock(incomingID,sType,height,width,length);if(i>=0&&pOK
){sendDockInfo(i,incomingID,sDroneName);}else{_program.IGC.SendBroadcastMessage("CONF",incomingID+":"+_wicoBlockMaster.
GetShipName().ToString()+":"+_program.Me.EntityId.ToString()+":"+_program.Vector3DToString(_program.Me.GetPosition()));}}}void
BaseTransmitTimerHandler(string sName){BaseAnnounce();}void BaseAnnounce(){if(dockingInfo.Count>0){bool bJumpCapable=false;string sname=
_wicoBlockMaster.GetShipName().ToString();Vector3D vPosition=_program.Me.CubeGrid.GetPosition();_program.IGC.SendBroadcastMessage("BASE"
,_program.toGpsName("",sname)+":"+_program.Me.EntityId.ToString()+":"+_program.Vector3DToString(vPosition)+":"+
bJumpCapable.ToString());}}public void doBaseAnnounce(bool bForceAnnounce=false){if(bForceAnnounce){BaseAnnounce();_wicoElapsedTime.
ResetTimer(_BaseTransmit);}}List<DockingInfo>dockingInfo=new List<DockingInfo>();public class DockingInfo{public IMyTerminalBlock
tb;public long State;public long assignedEntity;public List<IMyTerminalBlock>subBlocks;public long lAlign;public string
sDroneName;}void addDockingInfo(IMyTerminalBlock tb,long State=0){List<IMyTerminalBlock>subBlocks=new List<IMyTerminalBlock>();
DockingInfo di=new DockingInfo();di.tb=tb;di.State=State;di.assignedEntity=0;di.lAlign=-1;for(int i=0;i<_allLights.Count;i++){
double distance=(_allLights[i].GetPosition()-di.tb.GetPosition()).Length();if(distance<3){subBlocks.Add(_allLights[i]);}}di.
subBlocks=subBlocks;string sData=tb.CustomData;string[]lines=sData.Trim().Split('\n');_program.Echo(lines.Length+" Lines");for(
int i=0;i<lines.Length;i++){_program.Echo("|"+lines[i].Trim());string[]keys=lines[i].Trim().Split('=');if(lines[i].ToLower(
).Contains("align")){if(keys.Length>1){long l;if(long.TryParse(keys[1],out l))di.lAlign=l;else _program.Echo(
"Error Converting"+keys[1]);}else _program.Echo("Error parsing");}}dockingInfo.Add(di);}void sendDockInfo(int iDock,long incomingID,string
sName,bool bApproach=false){if(iDock<0||iDock>=dockingInfo.Count)return;IMyTerminalBlock connector=dockingInfo[iDock].tb;
Vector3D vPosition=connector.GetPosition();MatrixD worldConnectortb=connector.WorldMatrix;Vector3D vVec=worldConnectortb.Forward
;vVec.Normalize();dockingInfo[iDock].State=2;dockingInfo[iDock].assignedEntity=incomingID;dockingInfo[iDock].sDroneName=
sName;Vector3D vAlign;MatrixD worldtb;worldtb=_program.Me.WorldMatrix;vAlign=worldtb.Forward;switch(dockingInfo[iDock].lAlign
){case 0:break;case 1:vAlign=worldtb.Up;break;case 2:vAlign=worldtb.Down;break;case 3:vAlign=worldtb.Left;break;case 4:
vAlign=worldtb.Right;break;case 5:vAlign=worldtb.Backward;break;}vAlign.Normalize();if(bApproach){double size=_wicoBlockMaster
.HeightInMeters()+_wicoBlockMaster.WidthInMeters()+_wicoBlockMaster.LengthInMeters();size=Math.Max(size,30);Vector3D
vApproach=vPosition+vVec*size;_program.IGC.SendBroadcastMessage("CONA",incomingID+":"+_program.Me.EntityId.ToString()+":"+
_program.Vector3DToString(vApproach));}else{if(dockingInfo[iDock].lAlign<0){_program.IGC.SendBroadcastMessage("COND",incomingID+
":"+_program.Me.EntityId.ToString()+":"+_program.toGpsName("",connector.CustomName)+":"+_program.Vector3DToString(vPosition
)+":"+_program.Vector3DToString(vVec));}else{_program.IGC.SendBroadcastMessage("ACOND",incomingID+":"+_program.Me.
EntityId.ToString()+":"+_program.toGpsName("",connector.CustomName)+":"+_program.Vector3DToString(vPosition)+":"+_program.
Vector3DToString(vVec)+":"+_program.Vector3DToString(vAlign));}}}int getAvailableDock(long incomingID,string sType,double height,double
width,double length){int iDock=-1;for(int i=0;i<dockingInfo.Count;i++){if(dockingInfo[i].tb is IMyShipConnector){
IMyShipConnector connector=dockingInfo[i].tb as IMyShipConnector;if(dockingInfo[i].assignedEntity==incomingID){if(connector.Status==
MyShipConnectorStatus.Connected){dockingInfo[i].assignedEntity=0;continue;}if(connector.Status==MyShipConnectorStatus.Connectable){
dockingInfo[i].assignedEntity=0;continue;}iDock=i;break;}}}if(iDock<0){for(int i=0;i<dockingInfo.Count;i++){if(dockingInfo[i].tb is
IMyShipConnector){IMyShipConnector connector=dockingInfo[i].tb as IMyShipConnector;if(dockingInfo[i].State==0){if(connector.Status==
MyShipConnectorStatus.Connected)continue;if(connector.Status==MyShipConnectorStatus.Connectable)continue;iDock=i;break;}else if(dockingInfo[i
].State==1)continue;else if(dockingInfo[i].State==2)continue;}}}return iDock;}Color AVAILABLE_COLOR=new Color(0f,1.0f,
0.0f);Color INUSE_COLOR=new Color(0f,0f,1.0f);Color ASSIGNED_COLOR=new Color(1f,0f,0f);void processDockingStates(){bool
bWasIncoming=bHaveIncoming;if(dockingInfo.Count==0){return;}string output="Docking Info:\n";foreach(var di in dockingInfo){Color
toSet=AVAILABLE_COLOR;float blinkInterval=0;output+=_program.toGpsName("",di.tb.CustomName)+":";switch(di.State){case 0:if(di
.tb is IMyShipConnector){IMyShipConnector sc=di.tb as IMyShipConnector;if(sc.Status==MyShipConnectorStatus.Connectable){
toSet=INUSE_COLOR;output+="In Range";}else if(sc.Status==MyShipConnectorStatus.Connected){toSet=INUSE_COLOR;output+=
"CONNECTED";}else{output+="Available";}}break;case 1:output+="Player Reserved";toSet=INUSE_COLOR;break;case 2:output+=
"Incoming Ship";toSet=ASSIGNED_COLOR;blinkInterval=0.5f;if(di.tb is IMyShipConnector){bHaveIncoming=true;IMyShipConnector connector=di.
tb as IMyShipConnector;if(connector.Status==MyShipConnectorStatus.Connected)di.State=0;if(connector.Status==
MyShipConnectorStatus.Connectable)di.State=0;}break;default:_program.Echo("unkonwn docking state");break;}foreach(var tb in di.subBlocks){if(
tb is IMyLightingBlock){IMyLightingBlock lb=tb as IMyLightingBlock;lb.Color=toSet;lb.BlinkIntervalSeconds=blinkInterval;}}
output+="\n";}_program.Echo(output);if(bWasIncoming&&!bHaveIncoming){_timers.TimerTriggers(DockEndTag);}}}class CargoCheck{
public int cargopctmin=5;public int cargopcent=-1;public int cargohighwater=95;double cargoMult=-1;double totalCurrentVolume=
0.0;bool bCreative=false;public List<IMyTerminalBlock>lContainers=new List<IMyTerminalBlock>();public List<IMyTerminalBlock
>localEjectors=new List<IMyTerminalBlock>();Program _program;WicoBlockMaster _wicoBlockMaster;Displays _displays;string
sCargoSection="CARGO";string DisplayCargoCheck="CARGOCHECK";string KeyCargoPcent="cargopctmin";string KeyCargoHighwater=
"cargohighwater";public CargoCheck(Program program,WicoBlockMaster wbm,Displays displays){_program=program;_wicoBlockMaster=wbm;
_displays=displays;_wicoBlockMaster.AddLocalBlockHandler(BlockParseHandler);_wicoBlockMaster.AddLocalBlockChangedHandler(
LocalGridChangedHandler);_program.AddLoadHandler(LoadHandler);_program.AddSaveHandler(SaveHandler);cargopctmin=_program._CustomDataIni.Get(
sCargoSection,KeyCargoPcent).ToInt32(cargopctmin);_program._CustomDataIni.Set(sCargoSection,KeyCargoPcent,cargopctmin);cargohighwater
=_program._CustomDataIni.Get(sCargoSection,KeyCargoHighwater).ToInt32(cargohighwater);_program._CustomDataIni.Set(
sCargoSection,KeyCargoHighwater,cargohighwater);if(_displays!=null)_displays.AddSurfaceHandler(DisplayCargoCheck,SurfaceHandler);}
StringBuilder sbNotices=new StringBuilder(300);StringBuilder sbModeInfo=new StringBuilder(100);public void SurfaceHandler(string tag,
IMyTextSurface tsurface,int ActionType){if(tag==DisplayCargoCheck){if(ActionType==Displays.DODRAW){tsurface.WriteText(sbModeInfo);if(
tsurface.SurfaceSize.Y<256){}else{tsurface.WriteText(sbNotices,true);}}else if(ActionType==Displays.SETUPDRAW){tsurface.
ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;tsurface.WriteText("");if(tsurface.SurfaceSize.Y<256){tsurface.
Alignment=VRage.Game.GUI.TextPanel.TextAlignment.CENTER;tsurface.FontSize=2;}else{tsurface.Alignment=VRage.Game.GUI.TextPanel.
TextAlignment.LEFT;tsurface.FontSize=1.5f;}}else if(ActionType==Displays.CLEARDISPLAY){tsurface.WriteText("");}}}public void
BlockParseHandler(IMyTerminalBlock tb){if(tb is IMyCargoContainer)lContainers.Add(tb);else if(tb is IMyShipDrill)lContainers.Add(tb);else
if(tb is IMyShipWelder)lContainers.Add(tb);else if(tb is IMyShipGrinder)lContainers.Add(tb);else if(tb is IMyShipConnector
){if(tb.BlockDefinition.SubtypeName.Contains("ConnectorSmall")){localEjectors.Add(tb);}else{lContainers.Add(tb);}}}void
LocalGridChangedHandler(){lContainers.Clear();cargopcent=-1;cargoMult=-1;}void LoadHandler(MyIni Ini){}void SaveHandler(MyIni Ini){}public bool
bHasDrills=false;public bool bCargoFull=true;public bool bDrillFull=false;public void doCargoCheck(){sbNotices.Clear();sbModeInfo.
Clear();if(lContainers.Count<1){sbModeInfo.AppendLine("No Cargo Containers Found");cargopcent=-1;cargoMult=-1;return;}
totalCurrentVolume=0.0;double totalMax=0.0;double ratio=0;bCargoFull=true;bDrillFull=false;bHasDrills=false;sbNotices.AppendLine(
lContainers.Count+" Cargo Containers");for(int i=0;i<lContainers.Count;i++){if((lContainers[i]is IMyShipDrill)){bHasDrills=true;}
double capacity=-1;var count=lContainers[i].InventoryCount;for(var invcount=0;invcount<count;invcount++){var inv=lContainers[i
].GetInventory(invcount);if(inv!=null){totalCurrentVolume+=(double)inv.CurrentVolume;if((double)inv.MaxVolume>
9223372036854){bCreative=true;}else{bCreative=false;}if(!bCreative){capacity=(double)inv.MaxVolume;double dCapacity=defaultCapacity(
lContainers[i]);if(dCapacity>0)cargoMult=capacity/dCapacity;}else{capacity=defaultCapacity(lContainers[i])*10;cargoMult=9999;}if((
double)inv.CurrentVolume<(capacity-1)){if(!(lContainers[i]is IMyShipDrill)){bCargoFull=false;}}else{if(lContainers[i]is
IMyShipDrill){bDrillFull=true;}}}totalMax+=capacity;}}if(totalMax>0){ratio=(totalCurrentVolume/totalMax)*100;}else{ratio=100;}
cargopcent=(int)ratio;sbModeInfo.AppendLine(cargopcent+"% full");sbNotices.AppendLine("Cargo Full="+bCargoFull);if(bHasDrills)
sbNotices.AppendLine("Drills Full="+bDrillFull);if(bCargoFull&&bDrillFull)cargopcent=101;}double defaultCapacity(IMyTerminalBlock
theContainer){var inv=theContainer.GetInventory(0);string subtype=theContainer.BlockDefinition.SubtypeId;double capacity=(double)inv
.MaxVolume;if(capacity<999999999)return capacity;if(theContainer is IMyCargoContainer){if(subtype.Contains(
"LargeBlockLargeContainer"))capacity=421.875008;else if(subtype.Contains("LargeBlockSmallContainer"))capacity=15.625;else if(subtype.Contains(
"SmallBlockLargeContainer"))capacity=15.625;else if(subtype.Contains("SmallBlockMediumContainer"))capacity=3.375;else if(subtype.Contains(
"SmallBlockSmallContainer"))capacity=0.125;else if(subtype.Contains("Azimuth_LargeContainer"))capacity=7780.8;else if(subtype.Contains(
"Azimuth_MediumLargeContainer"))capacity=1945.2;else if(subtype.Contains("Azimuth_MediumContainer"))capacity=1878.6;else if(subtype.Contains(
"Azimuth_SmallContainer"))capacity=10.125;}else if(subtype.Contains("SmallBlockDrill"))capacity=3.375;else if(subtype.Contains("LargeBlockDrill"
))capacity=23.4375;else if(subtype.Contains("ConnectorMedium"))capacity=1.152;else if(subtype.Contains("ConnectorSmall"))
capacity=0.064;else if(subtype.Contains("Connector"))capacity=8.000;else if(subtype.Contains("LargeShipWelder"))capacity=15.625;
else if(subtype.Contains("LargeShipGrinder"))capacity=15.625;else if(subtype.Contains("SmallShipWelder"))capacity=3.375;else
if(subtype.Contains("SmallShipGrinder"))capacity=3.375;else{capacity=12;}return capacity;}}class Displays{const string
DisplayCheckTimer="DisplayCheck";const double DisplayInterval=0.5;public const int CLEARDISPLAY=0;public const int DODRAW=1;public const
int SETUPDRAW=99;bool _Debug=false;List<IMyTerminalBlock>_SurfaceProviders=new List<IMyTerminalBlock>();List<WicoDisplay>
_wicoDisplays=new List<WicoDisplay>();public class WicoDisplay{public string tag;List<IMyTextSurface>_surfaces;List<Action<string,
IMyTextSurface,int>>SurfaceDrawHandlers=new List<Action<string,IMyTextSurface,int>>();public WicoDisplay(List<IMyTerminalBlock>lsp,
string Tag){tag=Tag;_surfaces=new List<IMyTextSurface>();SurfaceDrawHandlers=new List<Action<string,IMyTextSurface,int>>();}
public void ResetSurfaces(){_surfaces.Clear();}public void OfferSurface(IMyTerminalBlock tb){if(tb.CustomName.Contains(tag)){
var tsp=tb as IMyTextSurfaceProvider;var x=tsp.SurfaceCount;var tsurface=tsp.GetSurface(0);if(tsurface!=null)_surfaces.Add(
tsurface);}}public void OfferHandler(Action<string,IMyTextSurface,int>handler){if(!SurfaceDrawHandlers.Contains(handler))
SurfaceDrawHandlers.Add(handler);}public void CallHandlers(int ActionType){foreach(var handler in SurfaceDrawHandlers){foreach(var surface
in _surfaces){handler(tag,surface,ActionType);}}}}Program _program;WicoBlockMaster _wicoBlockMaster;WicoElapsedTime
_wicoElapsedTime;string WicoDisplaySection="WicoDisplay";public Displays(Program program,WicoBlockMaster wicoBlockMaster,WicoElapsedTime
wicoElapsedTime){_program=program;_wicoBlockMaster=wicoBlockMaster;_wicoElapsedTime=wicoElapsedTime;_wicoBlockMaster.
AddLocalBlockHandler(BlockParseHandler);_wicoBlockMaster.AddLocalBlockChangedHandler(LocalGridChangedHandler);_program.AddPostInitHandler(
PostInitHandler());_Debug=_program._CustomDataIni.Get(WicoDisplaySection,"Debug").ToBoolean(_Debug);_program._CustomDataIni.Set(
WicoDisplaySection,"Debug",_Debug);_wicoElapsedTime.AddTimer(DisplayCheckTimer,DisplayInterval,ElapsedTimerHandler);_wicoElapsedTime.
StartTimer(DisplayCheckTimer);}public IEnumerator<bool>PostInitHandler(){foreach(var surface in _wicoDisplays){foreach(var tb in
_SurfaceProviders){surface.OfferSurface(tb);}}foreach(var surface in _wicoDisplays){surface.CallHandlers(SETUPDRAW);}yield return false;}
void BlockParseHandler(IMyTerminalBlock tb){if(tb is IMyTextPanel){_SurfaceProviders.Add(tb);}}void LocalGridChangedHandler(
){_SurfaceProviders.Clear();foreach(var display in _wicoDisplays){display.ResetSurfaces();}}void ElapsedTimerHandler(
string timerName){foreach(var display in _wicoDisplays){if(_Debug)_program.Echo("Display:"+display.tag);display.CallHandlers(
DODRAW);}}public bool AddSurfaceHandler(string tag,Action<string,IMyTextSurface,int>handler){if(handler==null)_program.Echo(
"handler is NULL!");bool bFound=false;WicoDisplay FoundDisplay=null;foreach(var display in _wicoDisplays){if(display.tag==tag){
FoundDisplay=display;bFound=true;break;}}if(!bFound){FoundDisplay=new WicoDisplay(_SurfaceProviders,tag);FoundDisplay.OfferHandler(
handler);_wicoDisplays.Add(FoundDisplay);}else{FoundDisplay.OfferHandler(handler);}return true;}public void ClearDisplays(
string tag){foreach(var display in _wicoDisplays){if(display.tag==tag){display.CallHandlers(CLEARDISPLAY);}}}public void
EchoInfo(){_program.Echo("Displays:");_program.Echo(" "+_wicoDisplays.Count+" DisplayTypes");foreach(var display in
_wicoDisplays){_program.Echo(" "+display.tag);}_program.Echo(" "+_SurfaceProviders.Count+" Surface Providers");}}class
WicoElapsedTime{Program _program;WicoUpdates _wicoUpdates;bool _bDebug=false;string wicoETString="WicoET";public WicoElapsedTime(
Program program,WicoUpdates wicoUpdates){_program=program;_wicoUpdates=wicoUpdates;_program.AddMainHandler(CheckTimers);_bDebug
=_program._CustomDataIni.Get(wicoETString,"Debug").ToBoolean(_bDebug);_program._CustomDataIni.Set(wicoETString,"Debug",
_bDebug);}List<ElapsedTimers>TimerList=new List<ElapsedTimers>();class ElapsedTimers{public string sName;public double
dWaitSeconds;public double dElapsedSeconds;public bool bActive;public bool AutoRestart;public Action<string>handler;}public bool
AddTimer(string sName,double dDefaultWaitSeconds=1,Action<string>handler=null,bool AutoRestart=true){ElapsedTimers et=new
ElapsedTimers{sName=sName,dWaitSeconds=dDefaultWaitSeconds,dElapsedSeconds=-1,bActive=false,AutoRestart=AutoRestart,handler=handler};
foreach(var et1 in TimerList){if(et1.sName==sName){et1.dWaitSeconds=dDefaultWaitSeconds;et1.dElapsedSeconds=-1;et1.AutoRestart=
AutoRestart;et1.handler=handler;return false;}}if(bCheckingTimers)_program.Echo("ERROR: Adding while checking");TimerList.Add(et);
return true;}public bool StartTimer(string sName){foreach(var et in TimerList){if(et.sName==sName){et.bActive=true;return true
;}}return false;}public bool StopTimer(string sName){foreach(var et in TimerList){if(et.sName==sName){et.bActive=false;
return true;}}return false;}public bool ResetTimer(string sName){foreach(var et in TimerList){if(et.sName==sName){et.
dElapsedSeconds=-1;et.bActive=false;return true;}}return false;}public bool RestartTimer(string sName){foreach(var et in TimerList){if(
et.sName==sName){et.dElapsedSeconds=0;et.bActive=true;return true;}}return false;}public bool IsExpired(string sName){
foreach(var et in TimerList){if(et.sName==sName){if(!et.bActive)return false;if(et.dElapsedSeconds<0)return true;if(et.
dElapsedSeconds>et.dWaitSeconds)return true;else return false;}}return true;}public bool IsInActiveOrExpired(string sName){foreach(var
et in TimerList){if(et.sName==sName){if(!et.bActive)return true;if(et.dElapsedSeconds<0)return true;if(et.dElapsedSeconds>
et.dWaitSeconds)return true;else return false;}}return true;}public bool IsActive(string sName){foreach(var et in
TimerList){if(et.sName==sName){if(et.bActive)return true;}}return false;}public bool GetTime(string sName,out double Elapsed,out
double Wait){Elapsed=-1;Wait=-1;foreach(var et in TimerList){if(et.sName==sName){Elapsed=et.dElapsedSeconds;Wait=et.
dWaitSeconds;return true;}}return false;}public double GetElapsed(string sName){foreach(var et in TimerList){if(et.sName==sName){
return et.dElapsedSeconds;}}return-1;}bool bCheckingTimers=false;public void CheckTimers(UpdateType updateSource){
bCheckingTimers=true;foreach(var et in TimerList){if(_bDebug)_program.Echo("Timer:"+et.sName+" Active="+et.bActive+" "+et.
dElapsedSeconds.ToString("0.00")+"/"+et.dWaitSeconds.ToString("0.00"));if(et.bActive){if(et.dElapsedSeconds>=0){et.dElapsedSeconds+=
_program.Runtime.TimeSinceLastRun.TotalMilliseconds/1000;}if(et.dElapsedSeconds>et.dWaitSeconds){if(et.handler!=null){et.handler
(et.sName);if(et.AutoRestart)et.dElapsedSeconds=0;}}if(et.dElapsedSeconds<0){if(et.AutoRestart)et.dElapsedSeconds=0;}
_wicoUpdates.WantSlow();}}bCheckingTimers=false;}public void SetDebug(bool bDebug=false){_bDebug=bDebug;}}class WicoIGC{
IMyUnicastListener _unicastListener;List<Action<MyIGCMessage>>_unicastMessageHandlers=new List<Action<MyIGCMessage>>();List<Action<
MyIGCMessage>>_broadcastMessageHandlers=new List<Action<MyIGCMessage>>();List<IMyBroadcastListener>_broadcastChannels=new List<
IMyBroadcastListener>();Program _program;bool _debug=false;IMyTextPanel _debugTextPanel;string WicoIGCSection="WicoIGC";public WicoIGC(
Program myProgram){_program=myProgram;_debug=_program._CustomDataIni.Get(WicoIGCSection,"Debug").ToBoolean(_debug);_program.
_CustomDataIni.Set(WicoIGCSection,"Debug",_debug);_debugTextPanel=_program.GridTerminalSystem.GetBlockWithName("IGC Report")as
IMyTextPanel;if(_debug)_debugTextPanel?.WriteText("");}public bool AddPublicHandler(string channelTag,Action<MyIGCMessage>handler,
bool setCallback=true){IMyBroadcastListener publicChannel;publicChannel=_program.IGC.RegisterBroadcastListener(channelTag);
if(setCallback)publicChannel.SetMessageCallback(channelTag);if(!_broadcastMessageHandlers.Contains(handler))
_broadcastMessageHandlers.Add(handler);if(!_broadcastChannels.Contains(publicChannel))_broadcastChannels.Add(publicChannel);return true;}public
bool AddUnicastHandler(Action<MyIGCMessage>handler){_unicastListener=_program.IGC.UnicastListener;_unicastListener.
SetMessageCallback("UNICAST");if(!_unicastMessageHandlers.Contains(handler))_unicastMessageHandlers.Add(handler);return true;}public void
ProcessIGCMessages(){bool bFoundMessages=false;if(_debug)_program.Echo(_broadcastChannels.Count.ToString()+" broadcast channels");if(
_debug)_program.Echo(_broadcastMessageHandlers.Count.ToString()+" broadcast message handlers");if(_debug)_program.Echo(
_unicastMessageHandlers.Count.ToString()+" unicast message handlers");do{bFoundMessages=false;foreach(var channel in _broadcastChannels){if(
channel.HasPendingMessage){bFoundMessages=true;var msg=channel.AcceptMessage();if(_debug){_program.Echo(
"Broadcast received. TAG:"+msg.Tag);_debugTextPanel?.WriteText("IGC:"+msg.Tag+" SRC:"+msg.Source.ToString("X")+"\n",true);}foreach(var handler in
_broadcastMessageHandlers){if(_debug)_program.Echo("Calling handler");handler(msg);}if(_debug)_program.Echo("Broadcast Handlers completed");}}}
while(bFoundMessages);if(_unicastListener!=null){if(_debug)_program.Echo("Unicast check");do{bFoundMessages=false;if(
_unicastListener.HasPendingMessage){bFoundMessages=true;var msg=_unicastListener.AcceptMessage();if(_debug)_program.Echo(
"Unicast received. TAG:"+msg.Tag);foreach(var handler in _unicastMessageHandlers){if(_debug)_program.Echo(" Unicast Handler");handler(msg);}if(
_debug)_program.Echo("Broadcast Handlers completed");}}while(bFoundMessages);if(_debug)_program.Echo("Unicast check completed"
);}}public void SetDebug(bool debug){_debug=debug;if(_debug)_debugTextPanel?.WriteText("");}}class IFF{const string
IGCIFFMessage="IGC_IFF_MSG";const string IGCIFFTimer="IGCIFFTIMER";WicoIGC _wicoIGC;WicoElapsedTime _wicoElapsedTime;long _EntityId;
Program _program;double AnnounceSeconds=1;public IFF(Program program,WicoIGC wicoIGC,WicoElapsedTime wicoElapsedTime){_wicoIGC=
wicoIGC;_wicoElapsedTime=wicoElapsedTime;_EntityId=program.Me.CubeGrid.EntityId;_program=program;AnnounceSeconds=_program.
_CustomDataIni.Get(_program.OurName,"IFFAnnounceSeconds").ToDouble(AnnounceSeconds);_program._CustomDataIni.Set(_program.OurName,
"IFFAnnounceSeconds",AnnounceSeconds);if(AnnounceSeconds>0){wicoElapsedTime.AddTimer(IGCIFFTimer,AnnounceSeconds,ElapsedTimehandler);
wicoElapsedTime.StartTimer(IGCIFFTimer);}}public void ElapsedTimehandler(string s){if(s==IGCIFFTimer){Announce();}}public void
AnnounceEnemy(long EntityID,Vector3D position,Double radius){MyTuple<byte,long,Vector3D,double>msg;msg.Item1=1;msg.Item2=EntityID;msg
.Item3=position;msg.Item4=radius;_program.IGC.SendBroadcastMessage(IGCIFFMessage,msg);}public void Announce(){MyTuple<
byte,long,Vector3D,double>msg;msg.Item1=2;msg.Item2=_EntityId;msg.Item3=_program.Me.CubeGrid.GetPosition();msg.Item4=
_program.Me.CubeGrid.WorldVolume.Radius;_program.IGC.SendBroadcastMessage(IGCIFFMessage,msg);}}WicoIGC wicoIGC;WicoBlockMaster
wicoBlockMaster;WicoElapsedTime wicoElapsedTime;List<Action<string,MyCommandLine,UpdateType>>UpdateTriggerHandlers=new List<Action<
string,MyCommandLine,UpdateType>>();List<Action<UpdateType>>UpdateUpdateHandlers=new List<Action<UpdateType>>();MyCommandLine
myCommandLine=new MyCommandLine();List<Action<MyIni>>SaveHandlers=new List<Action<MyIni>>();List<Action<MyIni>>LoadHandlers=new List<
Action<MyIni>>();List<Action<bool>>ResetMotionHandlers=new List<Action<bool>>();List<IEnumerator<bool>>PostInitHandlers=new
List<IEnumerator<bool>>();List<Action<UpdateType>>MainHandlers=new List<Action<UpdateType>>();MyIni _SaveIni=new MyIni();
MyIni _CustomDataIni=new MyIni();UpdateType utTriggers=UpdateType.Terminal|UpdateType.Trigger|UpdateType.Mod|UpdateType.
Script;UpdateType utUpdates=UpdateType.Update1|UpdateType.Update10|UpdateType.Update100|UpdateType.Once;bool bUsePBSurfaces=
true;IMyTextSurface mesurface0;IMyTextSurface mesurface1;bool bAllowPBRename=true;double tmGridCheckElapsedMs=0;string
OurName="Wico Modular";string moduleName="";string moduleList="";string sVersion=" 4.2b";string sMasterReporting="";Program(){
if(Me.TerminalRunArgument=="--clear"){Me.CustomData="";Storage="";}MyIniParseResult result;if(!_CustomDataIni.TryParse(Me.
CustomData,out result)){Me.CustomData="";_CustomDataIni.Clear();Echo(result.ToString());}if(!_SaveIni.TryParse(Storage,out result)
){Storage="";_SaveIni.Clear();Echo(result.ToString());}long meentityid=0;_SaveIni.Get(OurName+sVersion,"MEENITYID").
TryGetInt64(out meentityid);if(meentityid!=Me.EntityId){ErrorLog("New instance:Resetting Storage");Storage="";_SaveIni.Clear();}
_SaveIni.Set(OurName+sVersion,"MEENITYID",Me.EntityId);bAddDate=_CustomDataIni.Get(OurName,"DebugAddDate").ToBoolean(bAddDate);
_CustomDataIni.Set(OurName,"DebugAddDate",bAddDate);bAddLogCount=_CustomDataIni.Get(OurName,"DebugAddLogCount").ToBoolean(bAddLogCount
);_CustomDataIni.Set(OurName,"DebugAddLogCount",bAddLogCount);bAddRunCount=_CustomDataIni.Get(OurName,"DebugAddRunCount")
.ToBoolean(bAddRunCount);_CustomDataIni.Set(OurName,"DebugAddRunCount",bAddRunCount);bAllowPBRename=_CustomDataIni.Get(
OurName,"AllowPBRename").ToBoolean(bAllowPBRename);_CustomDataIni.Set(OurName,"AllowPBRename",bAllowPBRename);bUsePBSurfaces=
_CustomDataIni.Get(OurName,"UsePBSurfaces").ToBoolean(bUsePBSurfaces);_CustomDataIni.Set(OurName,"UsePBSurfaces",bUsePBSurfaces);
bEchoOn=_CustomDataIni.Get(OurName,"EchoOn").ToBoolean(bEchoOn);_CustomDataIni.Set(OurName,"EchoOn",bEchoOn);wicoIGC=new
WicoIGC(this);wicoBlockMaster=new WicoBlockMaster(this);wicoBlockMaster.LoadLocalGrid();ModuleControlInit();wicoElapsedTime=new
WicoElapsedTime(this,_wicoControl);ModuleProgramInit();Runtime.UpdateFrequency|=UpdateFrequency.Once;_oldEcho=Echo;Echo=MyEcho;if(
bUsePBSurfaces){if(Me.SurfaceCount>0){mesurface0=Me.GetSurface(0);mesurface0.ContentType=ContentType.TEXT_AND_IMAGE;mesurface0.
WriteText(OurName+sVersion+"\n"+moduleList);mesurface0.FontSize=1.3f;mesurface0.Alignment=TextAlignment.CENTER;}if(Me.
SurfaceCount>1){mesurface1=Me.GetSurface(1);mesurface1.ContentType=ContentType.TEXT_AND_IMAGE;mesurface1.WriteText("Version: "+
sVersion);mesurface0.Alignment=TextAlignment.CENTER;mesurface1.TextPadding=0.25f;mesurface1.FontSize=3.5f;}}if(bAllowPBRename&&!
Me.CustomName.Contains(moduleName))Me.CustomName="PB"+moduleName;if(!Me.Enabled){_oldEcho("I am turned OFF!");}}bool
bEchoOn=true;Action<string>_oldEcho;void MyEcho(string output){if(bEchoOn)_oldEcho(output);}void Save(){foreach(var handler in
SaveHandlers){handler(_SaveIni);}Storage=_SaveIni.ToString();}void AddSaveHandler(Action<MyIni>handler){if(!SaveHandlers.Contains(
handler))SaveHandlers.Add(handler);}void AddLoadHandler(Action<MyIni>handler){if(!LoadHandlers.Contains(handler))LoadHandlers.
Add(handler);}bool HandleLoad(MyIni theIni){foreach(var handler in LoadHandlers){handler(_SaveIni);}return false;}void
HandleMain(UpdateType updateSource){foreach(var handler in MainHandlers){handler(updateSource);}}void AddUpdateHandler(Action<
UpdateType>handler){if(!UpdateUpdateHandlers.Contains(handler))UpdateUpdateHandlers.Add(handler);}void AddTriggerHandler(Action<
string,MyCommandLine,UpdateType>handler){if(!UpdateTriggerHandlers.Contains(handler))UpdateTriggerHandlers.Add(handler);}void
AddResetMotionHandler(Action<bool>handler){if(!ResetMotionHandlers.Contains(handler))ResetMotionHandlers.Add(handler);}void ResetMotion(bool
bNoDrills=false){foreach(var handler in ResetMotionHandlers){handler(bNoDrills);}}void AddPostInitHandler(IEnumerator<bool>
handler){if(!PostInitHandlers.Contains(handler))PostInitHandlers.Add(handler);}void AddMainHandler(Action<UpdateType>handler){
if(!MainHandlers.Contains(handler))MainHandlers.Add(handler);}int postInitIterator=0;bool PostInit(){for(;postInitIterator
<PostInitHandlers.Count;postInitIterator++){if(PostInitHandlers[postInitIterator].MoveNext()){return true;}else{
PostInitHandlers[postInitIterator].Dispose();}}return false;}bool bCustomDataNeedsSave=false;double LastRunMs=0;double MaxRunMs=0;long
runCount=0;void Main(string argument,UpdateType updateSource){runCount++;LastRunMs=Runtime.LastRunTimeMs;if(bInitDone){if(
LastRunMs>MaxRunMs)MaxRunMs=LastRunMs;}if(moduleList!=""){Echo(OurName+sVersion+moduleList);}if(_wicoControl!=null&&_wicoControl.
_bUpdateDebug)Echo("Update="+updateSource.ToString());ModulePreMain(argument,updateSource);if(tmGridCheckElapsedMs>=0)
tmGridCheckElapsedMs+=Runtime.TimeSinceLastRun.TotalMilliseconds;if(!bInitDone){if(!WicoLocalInit()){Echo("Init Incomplete.  Trying again");
Runtime.UpdateFrequency=UpdateFrequency.Once;return;}}else{if(_wicoControl!=null)_wicoControl.AnnounceState();}{wicoIGC.
ProcessIGCMessages();}if((updateSource&(utTriggers))>0){MyCommandLine useCommandLine=null;if(myCommandLine.TryParse(argument)){
useCommandLine=myCommandLine;}bool bProcessed=false;{if(argument=="save"){Save();ErrorLog("After Save storage=");ErrorLog(Storage);
bProcessed=true;}}if(!bProcessed){foreach(var handler in UpdateTriggerHandlers){handler(argument,useCommandLine,updateSource);}}}
if((updateSource&(utUpdates))>0){_wicoControl.ResetUpdates();foreach(var handler in UpdateUpdateHandlers){handler(
updateSource);}}if(sMasterReporting!="")Echo("Reporting:\n"+sMasterReporting);if(sMasterReporting.Length>1024*2){sMasterReporting=""
;}ModulePostMain();HandleMain(updateSource);if(bCustomDataNeedsSave){bCustomDataNeedsSave=false;Me.CustomData=
_CustomDataIni.ToString();}Runtime.UpdateFrequency=_wicoControl.GenerateUpdate();}long logcount=0;bool bAddDate=false;bool
bAddLogCount=false;bool bAddRunCount=false;void ErrorLog(string str){if(bAddDate)str=System.DateTime.Now.ToLongTimeString()+":"+str;
;if(bAddLogCount)str=logcount++.ToString()+":"+str;if(bAddRunCount)str=runCount.ToString()+":"+str;sMasterReporting+="\n"
+str;}bool bInitDone=false;int InitStage=0;bool WicoLocalInit(){if(bInitDone)return true;if(InitStage<1){HandleLoad(
_SaveIni);InitStage++;}if(InitStage<2){if(PostInit())return false;InitStage++;}bInitDone=true;if(InitStage<3){if(_wicoControl!=
null)_wicoControl.ModeAfterInit(_SaveIni);InitStage++;}Me.CustomData=_CustomDataIni.ToString();return bInitDone;}void
WicoInitReset(){bInitDone=false;InitStage=0;}void CustomDataChanged(){bCustomDataNeedsSave=true;}string niceDoubleMeters(double thed)
{string nice="";if(thed>1000){nice=thed.ToString("N0")+"km";}else if(thed>100){nice=thed.ToString("0")+"m";}else if(thed>
10){nice=thed.ToString("0.0")+"m";}else{nice=thed.ToString("0.000")+"m";}return nice;}string Vector3DToString(Vector3D v){
string s;s=v.X.ToString("0.00")+":"+v.Y.ToString("0.00")+":"+v.Z.ToString("0.00");return s;}Vector3D StringToVector3D(string s
){string[]coordinates=s.Split(',');if(coordinates.Length<3){coordinates=s.Split(':');}double x,y,z;int iCoordinate=0;bool
xOk=double.TryParse(coordinates[iCoordinate++].Trim(),out x);bool yOk=double.TryParse(coordinates[iCoordinate++].Trim(),out
y);bool zOk=double.TryParse(coordinates[iCoordinate++].Trim(),out z);return new Vector3D(x,y,z);}bool stringToBool(string
txt){txt=txt.Trim().ToLower();return(txt=="on"||txt=="true");}string toGpsName(string ShipName,string sQual){string s;int
iName=ShipName.Length;int iQual=sQual.Length;if(iName+iQual>32){if(iQual>31)return"INVALID";iName=32-iQual;}s=ShipName.
Substring(0,iName)+sQual;s.Replace(":","_");s.Replace(";","_");return s;}void EchoInstructions(string sBanner=null){float fper=0;
fper=Runtime.CurrentInstructionCount/(float)Runtime.MaxInstructionCount;if(sBanner==null)sBanner="Instructions=";Echo(
sBanner+" "+(fper*100).ToString("0.00")+"%");}class OresLocal:CargoCheck{Program _program;WicoBlockMaster _wicoBlockMaster;
WicoControl _wicoControl;WicoIGC _wicoIGC;Asteroids _asteroids;Displays _displays;OreInfoLocs _oreInfoLocs;public OresLocal(Program
program,WicoBlockMaster wbm,WicoControl wicoControl,WicoIGC wicoIGC,Asteroids asteroids,OreInfoLocs orelocs,Displays displays):
base(program,wbm,null){_program=program;_wicoBlockMaster=wbm;_wicoControl=wicoControl;_wicoIGC=wicoIGC;_asteroids=asteroids;
_displays=displays;_oreInfoLocs=orelocs;if(_oreInfoLocs==null){_oreInfoLocs=new OreInfoLocs(program,wbm,wicoIGC,asteroids,
displays);}}void BroadcastHandler(MyIGCMessage msg){}List<MyInventoryItem>itemsL=new List<MyInventoryItem>();public void
OreDoCargoCheck(bool bInit=false){_oreInfoLocs.OreClearAmounts();for(int i=0;i<lContainers.Count;i++){var inv=lContainers[i].
GetInventory(0);if(inv==null)continue;itemsL.Clear();inv.GetItems(itemsL);for(int i2=0;i2<itemsL.Count;i2++){var item=itemsL[i2];if(
item.Type.ToString().Contains("Ore")){_oreInfoLocs.OreAddAmount(item.Type.SubtypeId.ToString(),(double)item.Amount,bInit);}}
}for(int i=0;i<localEjectors.Count;i++){var inv=localEjectors[i].GetInventory(0);if(inv==null)continue;itemsL.Clear();inv
.GetItems(itemsL);for(int i2=0;i2<itemsL.Count;i2++){var item=itemsL[i2];if(item.Type.ToString().Contains("Ore")){
_oreInfoLocs.OreAddAmount(item.Type.SubtypeId.ToString(),(double)item.Amount,bInit);}}}}}class OreInfoLocs{Program _program;
WicoBlockMaster _wicoBlockMaster;WicoIGC _wicoIGC;Asteroids _asteroids;Displays _displays;protected List<OreLocInfo>_oreLocs=new List<
OreLocInfo>();const string sOreSection="ORE";const string sOreDesirabilitySection="OREDESIREABILITY";protected const string
sOreTag="WICOORE";public OreInfoLocs(Program program,WicoBlockMaster wbm,WicoIGC igc,Asteroids asteroids,Displays displays){
_program=program;_wicoBlockMaster=wbm;_wicoIGC=igc;_asteroids=asteroids;_displays=displays;_program.AddLoadHandler(LoadHandler);
_program.AddSaveHandler(SaveHandler);OreInitInfo(_program._CustomDataIni);if(_displays!=null)_displays.AddSurfaceHandler(
"ORELOCS",SurfaceHandler);}public class OreLocInfo{public long AstEntityId;public int oreId;public Vector3D position;public
Vector3D vector;public long detectionType;}void LoadHandler(MyIni ini){int iCount;iCount=ini.Get(sOreSection,"count").ToInt32();
_oreLocs.Clear();long eId=0;int oreID=0;Vector3D position=new Vector3D(0,0,0);Vector3D vector=new Vector3D(0,0,0);long
detectionType=0;for(int j1=0;j1<iCount;j1++){eId=ini.Get(sOreSection,"AsteroidId"+j1.ToString()).ToInt32(0);oreID=ini.Get(sOreSection
,"oreId"+j1.ToString()).ToInt32(0);Vector3D.TryParse(ini.Get(sOreSection,"position"+j1.ToString()).ToString(),out
position);Vector3D.TryParse(ini.Get(sOreSection,"vector"+j1.ToString()).ToString(),out vector);detectionType=ini.Get(sOreSection
,"detectiontype"+j1.ToString()).ToInt32();OreLocInfo ore=new OreLocInfo{AstEntityId=eId,oreId=oreID,position=position,
vector=vector,detectionType=detectionType};_oreLocs.Add(ore);}}void SaveHandler(MyIni ini){var count=_oreLocs.Count;ini.Set(
sOreSection,"count",count);for(int i1=0;i1<_oreLocs.Count;i1++){ini.Set(sOreSection,"AsteroidId"+i1.ToString(),_oreLocs[i1].
AstEntityId);ini.Set(sOreSection,"oreId"+i1.ToString(),_oreLocs[i1].oreId);ini.Set(sOreSection,"position"+i1.ToString(),_program.
Vector3DToString(_oreLocs[i1].position));ini.Set(sOreSection,"vector"+i1.ToString(),_program.Vector3DToString(_oreLocs[i1].vector));ini.
Set(sOreSection,"detectiontype"+i1.ToString(),_oreLocs[i1].detectionType);}}StringBuilder sbNotices=new StringBuilder(300);
StringBuilder sbModeInfo=new StringBuilder(100);public void SurfaceHandler(string tag,IMyTextSurface tsurface,int ActionType){if(tag
=="ORELOCS"){if(ActionType==Displays.DODRAW){sbNotices.Clear();sbModeInfo.Clear();sbNotices.AppendLine(_oreLocs.Count+
" Ore Locations");{foreach(var oreloc in _oreLocs){sbNotices.AppendLine(oreloc.AstEntityId.ToString("N0")+" "+(_program.Me.GetPosition()
-oreloc.position).Length().ToString("N0")+"Meters");}tsurface.WriteText(sbModeInfo);if(tsurface.SurfaceSize.Y<512){}else{
tsurface.WriteText(sbNotices,true);}}}else if(ActionType==Displays.SETUPDRAW){tsurface.ContentType=VRage.Game.GUI.TextPanel.
ContentType.TEXT_AND_IMAGE;tsurface.WriteText("");if(tsurface.SurfaceSize.Y<512){tsurface.Alignment=VRage.Game.GUI.TextPanel.
TextAlignment.CENTER;tsurface.FontSize=2;}else{tsurface.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.LEFT;tsurface.FontSize=1.5f;
}}else if(ActionType==Displays.CLEARDISPLAY){tsurface.WriteText("");}}}public void OreAddLoc(long asteroidId,int OreID,
Vector3D Position,Vector3D vVec,long detectionType){OreLocInfo oli=new OreLocInfo();oli.oreId=OreID;oli.AstEntityId=asteroidId;
oli.position=Position;oli.vector=vVec;oli.detectionType=detectionType;_oreLocs.Add(oli);_program.IGC.SendBroadcastMessage(
sOreTag,asteroidId+":"+OreID+":"+_program.Vector3DToString(Position)+":"+_program.Vector3DToString(vVec)+":"+detectionType.
ToString());}public void OreDumpLocs(){for(int i=0;i<_oreLocs.Count;i++){_program.Echo(OreName(_oreLocs[i].oreId)+":"+_oreLocs[i
].position.ToString()+":"+_oreLocs[i].detectionType.ToString());}}List<OreInfo>oreInfos=new List<OreInfo>();public class
OreInfo{public int oreID;public string oreName;public long desireability;public bool bFound;public double localAmount;}const
string URANIUM="Uranium";const string PLATINUM="Platinum";const string ICE="Ice";const string COBALT="Cobalt";const string
GOLD="Gold";const string MAGNESIUM="Magnesium";const string NICKEL="Nickel";const string SILICON="Silicon";const string
SILVER="Silver";const string IRON="Iron";const string SCRAP="Scrap";const string STONE="Stone";string[]aOres={"Unknown",
URANIUM,PLATINUM,ICE,COBALT,GOLD,MAGNESIUM,NICKEL,SILICON,SILVER,IRON,STONE};long[]lOreDesirability={0,100,95,75,55,45,45,45,45
,45,15,-1};void OreInitInfo(MyIni ini){oreInfos.Clear();if(ini!=null){int iCount=aOres.Length;{for(int j1=0;j1<iCount;j1
++){int oreID=0;string oreName="";long desireability=-1;bool bFound=false;double localAmount=0;oreID=j1;oreName=aOres[j1];
desireability=ini.Get(sOreDesirabilitySection,aOres[j1]+"Desireability").ToInt64(lOreDesirability[j1]);OreInfo oi=new OreInfo();oi.
oreID=oreID;oi.oreName=oreName;oi.desireability=desireability;oi.bFound=bFound;oi.localAmount=localAmount;oreInfos.Add(oi);}}
}{_program.CustomDataChanged();for(int i1=1;i1<oreInfos.Count;i1++){ini.Set(sOreDesirabilitySection,oreInfos[i1].oreName+
"Desireability",oreInfos[i1].desireability);}}}public string OreName(int oreId){for(int i=0;i<oreInfos.Count;i++)if(oreInfos[i].oreID==
oreId)return oreInfos[i].oreName;return"INVALID ID:"+oreId;}public void OreClearAmounts(){if(oreInfos.Count<1)OreInitInfo(
_program._CustomDataIni);for(int i=0;i<oreInfos.Count;i++){oreInfos[i].localAmount=0;}}public void OreAddAmount(string sOre,
double lAmount,bool bNoFind=false){if(oreInfos.Count<1)OreInitInfo(_program._CustomDataIni);for(int i=0;i<oreInfos.Count;i++){
if(oreInfos[i].oreName==sOre){oreInfos[i].localAmount+=lAmount;if(lAmount>0)oreInfos[i].bFound=true;if(!bNoFind&&lAmount>0
&&!oreInfos[i].bFound){OreFound(oreInfos[i].oreID);}return;}}_program.ErrorLog("Ore :'"+sOre+"' Not found");}public double
CurrentUndesireableAmount(){double undesireableAmount=0;for(int i=0;i<oreInfos.Count;i++){if(oreInfos[i].desireability<0){undesireableAmount+=
oreInfos[i].localAmount;}}return undesireableAmount;}void OreFound(int oreIndex){if(oreInfos[oreIndex].desireability>0){MatrixD
refOrientation=new MatrixD();refOrientation=_wicoBlockMaster.GetMainController().WorldMatrix;Vector3D vVec=Vector3D.Normalize(
refOrientation.Forward);long astEntity=_asteroids.AsteroidFindNearest(true);if(astEntity>0){OreAddLoc(astEntity,oreIndex,
_wicoBlockMaster.GetMainController().GetPosition(),vVec,69);}}}StringBuilder sbFound=new StringBuilder(100);public StringBuilder
OreFoundInfo(){sbFound.Clear();sbFound.AppendLine("Ore Content:");for(int i=0;i<oreInfos.Count;i++){if(oreInfos[i].localAmount>0||
oreInfos[i].bFound){sbFound.AppendLine(oreInfos[i].oreName+" "+oreInfos[i].localAmount.ToString("N0"));}}return sbFound;}}class
PowerManagement{bool _ControlEngines=true;Program _program;WicoControl _wicoControl;PowerProduction _power;GasTanks _tanks;
WicoElapsedTime _elapsedTime;WicoIGC _igc;Displays _displays;string PowerManagementSection="PowerManagement";const string ScreenTag=
"POWERMANAGEMENT";double PowerManagementCheckSeconds=1;bool _bDebug=false;bool PowerManagementEnable=true;const string
PowerManagementName="PowerManagement";const string PowerManagementTimer=PowerManagementName+"Check";const string ControlEngines=
"ControlEngines";const string PowerManagementeDebug=PowerManagementName+"Debug";const string PowerManagementeEnabled=PowerManagementName
+"Enabled";public PowerManagement(Program program,WicoControl wicoControl,PowerProduction powerProduction,GasTanks tanks,
WicoElapsedTime wicoElapsedTime,WicoIGC wicoIGC,Displays displays){_program=program;_wicoControl=wicoControl;_power=powerProduction;
_tanks=tanks;_elapsedTime=wicoElapsedTime;_igc=wicoIGC;_displays=displays;_program.moduleName+=" PowerMgmt";_program.
moduleList+="\nPower Management V4.2b";_program.AddLoadHandler(LoadHandler);_program.AddSaveHandler(SaveHandler);_ControlEngines=
_program._CustomDataIni.Get(PowerManagementSection,ControlEngines).ToBoolean(_ControlEngines);_program._CustomDataIni.Set(
PowerManagementSection,ControlEngines,_ControlEngines);PowerManagementCheckSeconds=_program._CustomDataIni.Get(_program.OurName,
PowerManagementTimer).ToDouble(PowerManagementCheckSeconds);_program._CustomDataIni.Set(_program.OurName,PowerManagementTimer,
PowerManagementCheckSeconds);_elapsedTime.AddTimer(PowerManagementTimer,PowerManagementCheckSeconds,ElapsedTimeHandler);_elapsedTime.StartTimer(
PowerManagementTimer);_displays.AddSurfaceHandler(ScreenTag,SurfaceHandler);_bDebug=_program._CustomDataIni.Get(_program.OurName,
PowerManagementeDebug).ToBoolean(_bDebug);_program._CustomDataIni.Set(_program.OurName,PowerManagementeDebug,_bDebug);PowerManagementEnable=
_program._CustomDataIni.Get(_program.OurName,PowerManagementeEnabled).ToBoolean(PowerManagementEnable);_program._CustomDataIni.
Set(_program.OurName,PowerManagementeEnabled,PowerManagementEnable);if(!PowerManagementEnable){_elapsedTime.StopTimer(
PowerManagementTimer);}}void LoadHandler(MyIni Ini){}void SaveHandler(MyIni Ini){}StringBuilder sbNotices=new StringBuilder(300);
StringBuilder sbModeInfo=new StringBuilder(100);public void SurfaceHandler(string tag,IMyTextSurface tsurface,int ActionType){if(!
PowerManagementEnable)return;if(tag==ScreenTag){if(ActionType==Displays.DODRAW){sbNotices.Clear();sbModeInfo.Clear();if(_power.HasBatteries()
)sbModeInfo.AppendLine("Batteries="+_power.batteryPercentage+" ("+_power.batterypctlow+")");if(_tanks.HasHydroTanks())
sbModeInfo.AppendLine("H Tanks="+_tanks.hydroPercent.ToString("0")+"%");if(_power.EnginesCount()>0)sbModeInfo.AppendLine(
"   Engines="+(_power.EnginesAreOff()?"Off":"ON"));if(_power.maxTotalPower>0){if(_power.HasBatteries())sbNotices.AppendLine(
"Batteries="+(_power.batteryTotalOutput/_power.maxTotalPower*100).ToString("0")+"%");if(_power.maxReactorPower>0)sbNotices.
AppendLine("Reactors="+(_power.currentReactorOutput/_power.maxTotalPower*100).ToString("0")+"%");if(_power.maxSolarPower>0)
sbNotices.AppendLine("Solar="+(_power.currentSolarOutput/_power.maxTotalPower*100).ToString("0")+"%");if(_power.
currentTurbineOutput>0)sbNotices.AppendLine("Turbines="+(_power.currentTurbineOutput/_power.maxTotalPower*100).ToString("0")+"%");if(_power.
currentEngineOutput>0)sbNotices.AppendLine("Engines="+(_power.currentEngineOutput/_power.maxTotalPower*100).ToString("0")+"%");}tsurface.
WriteText(sbModeInfo);if(tsurface.SurfaceSize.Y<512){}else{tsurface.WriteText(sbNotices,true);}}else if(ActionType==Displays.
SETUPDRAW){tsurface.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;tsurface.WriteText("");if(tsurface.
SurfaceSize.Y<512){tsurface.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.CENTER;tsurface.FontSize=2;}else{tsurface.Alignment=
VRage.Game.GUI.TextPanel.TextAlignment.LEFT;tsurface.FontSize=1.5f;}}else if(ActionType==Displays.CLEARDISPLAY){tsurface.
WriteText("");}}}void ElapsedTimeHandler(string timer){if(!PowerManagementEnable)return;_power.CalcPower();_tanks.TanksCalculate(
);if(_ControlEngines&&_power.EnginesCount()>0){if(_power.batteryPercentage<_power.batterypctlow){_power.EngineControl(
true);_program.Echo("PWR:Batteries LOW!");}else if(_power.batteryTotalOutput>(_power.maxBatteryPower*.75)&&_tanks.
hydroPercent>=_tanks.tankspctlow&&_power.batteryPercentage<_power.batterypcthigh){_power.EngineControl(true);_program.Echo(
"PWR:Batteries need help on output");}else if(_power.HasBatteries()&&_power.batteryPercentage<_power.batterypcthigh&&_tanks.hydroPercent>=_tanks.
tankspcthigh){_power.EngineControl(true);_program.Echo("PWR:Extra hydro fuel; using for charging batteries to max");}else{if(!_power
.EnginesAreOff()&&_tanks.hydroPercent>=_tanks.tankspcthigh){if(_power.batteryPercentage<(_power.batterypcthigh*1.1)){
_program.Echo("PWR:Keep running for a bit with extra hydro");return;}}_power.EngineControl(false);_program.Echo(
"No need to have engines on.");}}}public void ProcessTrigger(string sArgument,MyCommandLine myCommandLine,UpdateType updateSource){}void
UpdateHandler(UpdateType updateSource){}}class PowerProduction{List<IMyTerminalBlock>lHydrogenEngines=new List<IMyTerminalBlock>();
public double maxHydrogenPower=-1;public float currentEngineOutput=-1;List<IMyTerminalBlock>reactorList=new List<
IMyTerminalBlock>();public double maxReactorPower=-1;public float currentReactorOutput=-1;List<IMyTerminalBlock>solarList=new List<
IMyTerminalBlock>();public double maxSolarPower=-1;public float currentSolarOutput=0;List<IMyTerminalBlock>batteryList=new List<
IMyTerminalBlock>();public double maxBatteryPower=-1;public int batteryPercentage=-1;public double batteryTotalInput=0;public double
batteryTotalOutput=0;public int batterypcthigh=80;public int batterypctlow=20;List<IMyTerminalBlock>turbineList=new List<IMyTerminalBlock>
();public double maxTurbinePower=-1;public float currentTurbineOutput=0;public double maxTotalPower=-1;public double
currentTotalOutput=-1;string sPowerSection="POWER";Program _program;WicoBlockMaster _wicoBlockMaster;public PowerProduction(Program
program,WicoBlockMaster wbm){_program=program;_wicoBlockMaster=wbm;_wicoBlockMaster.AddLocalBlockHandler(BlockParseHandler);
_wicoBlockMaster.AddLocalBlockChangedHandler(LocalGridChangedHandler);_program.AddLoadHandler(LoadHandler);_program.AddSaveHandler(
SaveHandler);batterypcthigh=_program._CustomDataIni.Get(sPowerSection,"batterypcthigh").ToInt32(batterypcthigh);_program.
_CustomDataIni.Set(sPowerSection,"batterypcthigh",batterypcthigh);batterypctlow=_program._CustomDataIni.Get(sPowerSection,
"batterypctlow").ToInt32(batterypctlow);_program._CustomDataIni.Set(sPowerSection,"batterypctlow",batterypctlow);}public void
BlockParseHandler(IMyTerminalBlock tb){if(tb.BlockDefinition.TypeIdString=="MyObjectBuilder_HydrogenEngine"){lHydrogenEngines.Add(tb);}if
(tb is IMyReactor){reactorList.Add(tb);}if(tb is IMySolarPanel){solarList.Add(tb);}if(tb is IMyBatteryBlock){batteryList.
Add(tb);}if(tb.BlockDefinition.TypeIdString=="MyObjectBuilder_WindTurbine"){turbineList.Add(tb);}}void
LocalGridChangedHandler(){lHydrogenEngines.Clear();maxHydrogenPower=-1;reactorList.Clear();maxReactorPower=-1;solarList.Clear();
currentSolarOutput=0;maxSolarPower=-1;batteryList.Clear();maxBatteryPower=-1;batteryPercentage=-1;}void LoadHandler(MyIni Ini){}void
SaveHandler(MyIni Ini){}public int CalcCurrentEngine(){currentEngineOutput=0;maxHydrogenPower=0;int count=0;foreach(var tb in
lHydrogenEngines){if(tb is IMyPowerProducer){count++;var pp=tb as IMyPowerProducer;var fb=tb as IMyFunctionalBlock;if(fb.Enabled){
currentEngineOutput+=pp.CurrentOutput;}maxHydrogenPower+=pp.MaxOutput;}}return count;}public int EnginesCount(){return lHydrogenEngines.
Count;}public double EnginesTanksFill(){double totalLevel=0;int iTanksCount=0;foreach(var tb in lHydrogenEngines){if(tb.
BlockDefinition.TypeIdString=="MyObjectBuilder_HydrogenEngine"){double tankLevel=0;string[]lines=tb.DetailedInfo.Trim().Split('\n');if(
lines.Length<3)continue;string[]aParams=lines[3].Split(' ');if(aParams.Length<2)continue;string sPercent=aParams[1].Replace(
'%',' ');bool bOK=double.TryParse(sPercent.Trim(),out tankLevel);tankLevel/=100.0;totalLevel+=tankLevel;iTanksCount++;}}if(
iTanksCount>0){return totalLevel/iTanksCount;}else return-1;}public bool EnginesAreOff(){foreach(var tb in lHydrogenEngines){if(tb
is IMyFunctionalBlock){var fb=tb as IMyFunctionalBlock;if(fb.Enabled)return false;}}return true;}public void EngineControl
(bool bOn=true){foreach(var tb in lHydrogenEngines){if(tb is IMyFunctionalBlock){var fb=tb as IMyFunctionalBlock;if(fb.
Enabled!=bOn)fb.Enabled=bOn;}}}public bool reactorCheck(out float currentOutput){currentOutput=0;maxReactorPower=-1;bool
bNeedyReactor=false;if(reactorList.Count>0)maxReactorPower=0;foreach(IMyReactor r in reactorList){currentOutput+=r.CurrentOutput;
maxReactorPower+=r.MaxOutput;}return bNeedyReactor;}void calcCurrentSolar(){if(solarList.Count>0)maxSolarPower=0;currentSolarOutput=0;
foreach(var tb in solarList){IMySolarPanel r=tb as IMySolarPanel;maxSolarPower+=r.MaxOutput;currentSolarOutput+=r.CurrentOutput
;}}void calcCurrentTurbine(){if(turbineList.Count>0)maxTurbinePower=0;currentTurbineOutput=0;foreach(var tb in
turbineList){var pp=tb as IMyPowerProducer;maxTurbinePower+=pp.MaxOutput;currentTurbineOutput+=pp.CurrentOutput;}}bool
isRechargeSet(IMyTerminalBlock block){if(block is IMyBatteryBlock){IMyBatteryBlock myb=block as IMyBatteryBlock;return(myb.ChargeMode
==ChargeMode.Recharge);}else return false;}bool isDischargeSet(IMyTerminalBlock block){if(block is IMyBatteryBlock){
IMyBatteryBlock myb=block as IMyBatteryBlock;return(myb.ChargeMode==ChargeMode.Discharge);}else return false;}public bool HasBatteries(
){return batteryList.Count>0;}public bool BatteryCheck(int targetMax,bool bEcho=true,bool bProgress=false){float
totalCapacity=0;float totalCharge=0;bool bFoundRecharging=false;float f1;if(batteryList.Count<1)return false;batteryPercentage=0;
batteryTotalInput=0;batteryTotalOutput=0;maxBatteryPower=-1;for(int ib=0;ib<batteryList.Count;ib++){float charge=0;float capacity=0;int
percentthisbattery=100;IMyBatteryBlock b;b=batteryList[ib]as IMyBatteryBlock;if(maxBatteryPower<0)maxBatteryPower=0;maxBatteryPower+=b.
MaxOutput;f1=b.MaxStoredPower;capacity+=f1;totalCapacity+=f1;f1=b.CurrentStoredPower;charge+=f1;totalCharge+=f1;if(capacity>0){f1
=((charge*100)/capacity);f1=(float)Math.Round(f1,0);percentthisbattery=(int)f1;}string s;s="";if(isRechargeSet(
batteryList[ib]))s+="R";else if(isDischargeSet(batteryList[ib]))s+="D";else s+="a";float fPower;fPower=b.CurrentInput;
batteryTotalInput+=fPower;if(fPower>0)s+="+";else s+=" ";fPower=b.CurrentOutput;batteryTotalOutput+=fPower;if(fPower>0)s+="-";else s+=" "
;s+=percentthisbattery+"%";s+=":"+batteryList[ib].CustomName;if(batteryList.Count<10&&bEcho)_program.Echo(s);if(targetMax
>0){if(b.ChargeMode==ChargeMode.Recharge){if(percentthisbattery>Math.Min(targetMax,99)){b.ChargeMode=ChargeMode.Auto;}
else bFoundRecharging=true;}else{if(percentthisbattery<targetMax&&!bFoundRecharging){b.ChargeMode=ChargeMode.Recharge;
bFoundRecharging=true;}else{b.ChargeMode=ChargeMode.Auto;}}}}if(totalCapacity>0){f1=((totalCharge*100)/totalCapacity);f1=(float)Math.
Round(f1,0);batteryPercentage=(int)f1;if(bEcho)_program.Echo("Batteries: "+f1.ToString("0.0")+"%");}else batteryPercentage=-1
;return bFoundRecharging;}public void BatterySetNormal(){for(int i=0;i<batteryList.Count;i++){IMyBatteryBlock b;b=
batteryList[i]as IMyBatteryBlock;b.ChargeMode=ChargeMode.Auto;}}public void BatteryDischargeSet(bool bEcho=false,bool bDischarge=
true){if(bEcho)_program.Echo(batteryList.Count+" Batteries");string s;for(int i=0;i<batteryList.Count;i++){IMyBatteryBlock b
;b=batteryList[i]as IMyBatteryBlock;if(bDischarge){b.ChargeMode=ChargeMode.Discharge;}else b.ChargeMode=ChargeMode.
Recharge;s=b.CustomName+": ";if(b.ChargeMode==ChargeMode.Recharge){s+="RECHARGE/";}else s+="NOTRECHARGE/";if(b.ChargeMode==
ChargeMode.Discharge){s+="DISCHARGE";}else{s+="NOTDISCHARGE";}if(bEcho)_program.Echo(s);}}public void CalcPower(){calcCurrentSolar
();reactorCheck(out currentReactorOutput);CalcCurrentEngine();BatteryCheck(0,false);calcCurrentTurbine();maxTotalPower=
maxBatteryPower+maxHydrogenPower+maxReactorPower+maxSolarPower+maxTurbinePower;currentTotalOutput=batteryTotalOutput-batteryTotalInput+
currentEngineOutput+currentReactorOutput+currentSolarOutput+currentTurbineOutput;}}class GasTanks{List<IMyTerminalBlock>tankList=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>oxytankList=new List<IMyTerminalBlock>();List<IMyTerminalBlock>hydrotankList=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>isolatedoxytankList=new List<IMyTerminalBlock>();string _tanksSection="TANKS";Program _program
;WicoBlockMaster wbm;public GasTanks(Program program,WicoBlockMaster wicoBlockMaster){_program=program;wbm=
wicoBlockMaster;wbm.AddLocalBlockHandler(BlockParseHandler);wbm.AddLocalBlockChangedHandler(LocalGridChangedHandler);tankspcthigh=
_program._CustomDataIni.Get(_tanksSection,"tankspcthigh").ToInt32(tankspcthigh);_program._CustomDataIni.Set(_tanksSection,
"tankspcthigh",tankspcthigh);tankspctlow=_program._CustomDataIni.Get(_tanksSection,"tankspctlow").ToInt32(tankspctlow);_program.
_CustomDataIni.Set(_tanksSection,"tankspctlow",tankspctlow);}public void BlockParseHandler(IMyTerminalBlock tb){if(tb is IMyGasTank){
tankList.Add(tb);if(TankType(tb)==iTankOxygen){if(tb.CustomName.ToLower().Contains("isolated"))isolatedoxytankList.Add(tb);else
oxytankList.Add(tb);}else if(TankType(tb)==iTankHydro){hydrotankList.Add(tb);}}}void LocalGridChangedHandler(){tankList.Clear();
isolatedoxytankList.Clear();oxytankList.Clear();hydrotankList.Clear();}public int tankspcthigh=99;public int tankspctlow=25;public double
hydroPercent=-1;public double oxyPercent=-1;public void TanksCalculate(){hydroPercent=tanksFill(iTankHydro)*100;oxyPercent=tanksFill
(iTankOxygen)*100;}public double TanksFill(List<IMyTerminalBlock>tankList){double totalPercent=0;int iTanksCount=0;for(
int i=0;i<tankList.Count;++i){{IMyGasTank tank=tankList[i]as IMyGasTank;if(tank==null)continue;float tankLevel=(float)tank.
FilledRatio;totalPercent+=tankLevel;iTanksCount++;}}if(iTanksCount>0){return totalPercent*100/iTanksCount;}else return 0;}public
double tanksFill(int iTypes=0xff){if(tankList.Count<1)return-1;double totalLevel=0;int iTanksCount=0;for(int i=0;i<tankList.
Count;++i){int iTankType=TankType(tankList[i]);if((iTankType&iTypes)>0){IMyGasTank tank=tankList[i]as IMyGasTank;if(tank==
null)continue;var tankLevel=tank.FilledRatio;totalLevel+=tankLevel;iTanksCount++;}}if(iTanksCount>0){return totalLevel/
iTanksCount;}else return-1;}const int iTankOxygen=1;const int iTankHydro=2;int TankType(IMyTerminalBlock theBlock){if(theBlock is
IMyGasTank){if(theBlock.BlockDefinition.SubtypeId.Contains("Hydro"))return iTankHydro;else return iTankOxygen;}return 0;}public
void TanksStockpile(bool bStockPile=true,int iTypes=0xff){if(tankList.Count<1)return;for(int i=0;i<tankList.Count;++i){int
iTankType=TankType(tankList[i]);if((iTankType&iTypes)>0){IMyGasTank tank=tankList[i]as IMyGasTank;if(tank==null)continue;tank.
Stockpile=bStockPile;}}}public bool HasHydroTanks(){for(int i=0;i<tankList.Count;++i){int iTankType=TankType(tankList[i]);if(
iTankType==iTankHydro){return true;}}return false;}}class Timers{List<IMyTerminalBlock>localTimerList=new List<IMyTerminalBlock>(
);Dictionary<string,List<IMyTerminalBlock>>dTimers=new Dictionary<string,List<IMyTerminalBlock>>();Program thisProgram;
WicoBlockMaster wicoBlockMaster;public Timers(Program program,WicoBlockMaster wbm){thisProgram=program;wicoBlockMaster=wbm;
wicoBlockMaster.AddLocalBlockHandler(BlockParseHandler);wicoBlockMaster.AddLocalBlockChangedHandler(LocalGridChangedHandler);}public
void BlockParseHandler(IMyTerminalBlock tb){if(tb is IMyTimerBlock){localTimerList.Add(tb);}}void LocalGridChangedHandler(){
localTimerList.Clear();dTimers.Clear();}public void initTimers(){dTimers.Clear();}public bool TimerTriggers(string sKeyword){bool
bTriggered=false;List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();IMyTimerBlock theTriggerTimer=null;if(dTimers.
ContainsKey(sKeyword)){blocks=dTimers[sKeyword];}else{blocks=wicoBlockMaster.GetBlocksContains<IMyTimerBlock>(sKeyword);dTimers.Add
(sKeyword,blocks);}for(int i=0;i<blocks.Count;i++){theTriggerTimer=blocks[i]as IMyTimerBlock;if(theTriggerTimer!=null){if
(theTriggerTimer.Enabled){theTriggerTimer.Trigger();bTriggered=true;}else{thisProgram.Echo("Timer:"+theTriggerTimer.
CustomName+" is OFF");}}}return bTriggered;}}class WicoBlockMaster{Program _program;IMyGridTerminalSystem GridTerminalSystem;
public WicoBlockMaster(Program program){_program=program;GridTerminalSystem=_program.GridTerminalSystem;AddLocalBlockHandler(
BlockParseHandler);AddLocalBlockChangedHandler(LocalGridChangedHandler);_program.AddLoadHandler(LoadHandler);_program.AddSaveHandler(
SaveHandler);_program.AddPostInitHandler(LocalBlocksInit());_program.AddPostInitHandler(RemoteBlocksInit());
DesiredMinTravelElevation=(float)_program._CustomDataIni.Get(_program.OurName,"MinTravelElevation").ToDouble(DesiredMinTravelElevation);_program.
_CustomDataIni.Set(_program.OurName,"MinTravelElevation",DesiredMinTravelElevation);}void LoadHandler(MyIni theINI){}void SaveHandler(
MyIni theINI){}List<IMyShipController>shipControllers=new List<IMyShipController>();private IMyShipController
MainShipController;public void BlockParseHandler(IMyTerminalBlock tb){if(tb is IMyShipController){shipControllers.Add(tb as
IMyShipController);}}public void LocalGridChangedHandler(){shipdimController=null;MainShipController=null;shipControllers.Clear();}public
bool IsClosed(IMyTerminalBlock block){if(block==null||block.WorldMatrix==MatrixD.Identity)return true;return!(
GridTerminalSystem.GetBlockWithId(block.EntityId)==block);}public IMyRemoteControl GetRemoteControl(){foreach(var tb in shipControllers){
if(tb is IMyRemoteControl&&tb.IsUnderControl){return tb as IMyRemoteControl;}}foreach(var tb in shipControllers){if(tb is
IMyRemoteControl){return tb as IMyRemoteControl;}}return null;}public IMyShipController GetMainController(){foreach(var tb in
shipControllers){if(tb.IsUnderControl&&tb.CanControlShip){MainShipController=tb;break;}}if(MainShipController==null){foreach(var tb in
shipControllers){if(tb is IMyRemoteControl&&tb.CanControlShip){MainShipController=tb;break;}}if(MainShipController==null){foreach(var
tb in shipControllers){if(tb is IMyCockpit&&tb.CanControlShip){MainShipController=tb;break;}}}if(MainShipController==null)
{foreach(var tb in shipControllers){if(tb is IMyShipController&&tb.CanControlShip){MainShipController=tb;break;}}}if(
MainShipController!=null){ShipDimensions(MainShipController);}else{}}return MainShipController;}public Vector3D CenterOfMass(){Vector3D
com=_emptyV3D;var shipcontroller=GetMainController();if(shipcontroller!=null)com=shipcontroller.CenterOfMass;else{com=
_program.Me.CubeGrid.GetPosition();}return com;}public double GetShipSpeed(){double shipspeed=-1;var shipcontroller=
GetMainController();if(shipcontroller!=null)shipspeed=shipcontroller.GetShipSpeed();return shipspeed;}public Vector3D GetShipVelocity(){
Vector3D velocity=_emptyV3D;var shipcontroller=GetMainController();if(shipcontroller!=null){MyShipVelocities velocities=
shipcontroller.GetShipVelocities();velocity=velocities.LinearVelocity;}return velocity;}StringBuilder ShipName=new StringBuilder(42);
public StringBuilder GetShipName(){ShipName.Clear();if(GetMainController()!=null)ShipName.Append(GetMainController().CubeGrid.
CustomName);else ShipName.Append(_program.Me.CubeGrid.CustomName);return ShipName;}public Vector3D GetNaturalGravity(){Vector3D
vNG=_emptyV3D;var shipcontroller=GetMainController();if(shipcontroller!=null)vNG=shipcontroller.GetNaturalGravity();return
vNG;}public double GetAllPhysicalMass(){double effectiveMass=-1;effectiveMass=GetPhysicalMass();foreach(var grid in
remoteCubeGrids){bool bGridDone=false;foreach(var tb in gtsRemoteBlocks){if(tb is IMyShipController&&tb.CubeGrid==grid){var sc=tb as
IMyShipController;MyShipMass myMass;myMass=sc.CalculateShipMass();effectiveMass+=myMass.PhysicalMass;bGridDone=true;break;}}if(bGridDone)
break;}return effectiveMass;}public double GetPhysicalMass(){double effectiveMass=-1;var shipcontroller=GetMainController();
if(shipcontroller!=null){MyShipMass myMass;myMass=shipcontroller.CalculateShipMass();effectiveMass=myMass.PhysicalMass;}
return effectiveMass;}public void DisplayInfo(){_program.Echo("LBlocks ="+localBlocksCount+" grids="+localCubeGrids.Count);
_program.Echo("RBlocks ="+remoteBlocksCount+" grids="+remoteCubeGrids.Count);_program.Echo("PM="+GetPhysicalMass().ToString("N2"
));_program.Echo("APM="+GetAllPhysicalMass().ToString("N2"));}List<IMyTerminalBlock>gtsLocalBlocks=new List<
IMyTerminalBlock>();public long localBlocksCount=0;bool CollectRemote=false;List<IMyTerminalBlock>gtsRemoteBlocks=new List<
IMyTerminalBlock>();long remoteBlocksCount=0;List<IMyCubeGrid>localCubeGrids=new List<IMyCubeGrid>();List<IMyCubeGrid>remoteCubeGrids=
new List<IMyCubeGrid>();List<Action<IMyTerminalBlock>>WicoLocalBlockParseHandlers=new List<Action<IMyTerminalBlock>>();List
<Action<IMyTerminalBlock>>WicoRemoteBlockParseHandlers=new List<Action<IMyTerminalBlock>>();List<Action>
WicoLocalBlockChangedHandlers=new List<Action>();List<Action>WicoRemoteBlockChangedHandlers=new List<Action>();public bool AddLocalBlockHandler(
Action<IMyTerminalBlock>handler){if(!WicoLocalBlockParseHandlers.Contains(handler))WicoLocalBlockParseHandlers.Add(handler);
return true;}public void AddLocalBlockChangedHandler(Action handler){if(!WicoLocalBlockChangedHandlers.Contains(handler))
WicoLocalBlockChangedHandlers.Add(handler);}public bool AddRemoteBlockHandler(Action<IMyTerminalBlock>handler){if(!WicoRemoteBlockParseHandlers.
Contains(handler))WicoRemoteBlockParseHandlers.Add(handler);return true;}public void AddRemoteBlocChangedHandler(Action handler)
{if(!WicoRemoteBlockChangedHandlers.Contains(handler))WicoRemoteBlockChangedHandlers.Add(handler);}public void
LoadLocalGrid(){localCubeGrids.Clear();gtsLocalBlocks.Clear();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(gtsLocalBlocks,(x1
=>x1.IsSameConstructAs(_program.Me)&&ValidBlock(x1)));foreach(var tb in gtsLocalBlocks){if(!localCubeGrids.Contains(tb.
CubeGrid)){localCubeGrids.Add(tb.CubeGrid);}}}public IEnumerator<bool>LocalBlocksInit(){yield return true;float fper=0;if(
gtsLocalBlocks.Count<1){LoadLocalGrid();}fper=_program.Runtime.CurrentInstructionCount/(float)_program.Runtime.MaxInstructionCount;if(
fper>0.75f)yield return true;localBlocksCount=gtsLocalBlocks.Count;foreach(var tb in gtsLocalBlocks){fper=_program.Runtime.
CurrentInstructionCount/(float)_program.Runtime.MaxInstructionCount;if(fper>0.75f){yield return true;}foreach(var handler in
WicoLocalBlockParseHandlers){fper=_program.Runtime.CurrentInstructionCount/(float)_program.Runtime.MaxInstructionCount;if(fper>0.75f){yield return
true;}handler(tb);}}}void LocalBlocksChanged(){foreach(var handler in WicoLocalBlockChangedHandlers){handler();}}public
IEnumerator<bool>RemoteBlocksInit(){yield return true;float fper=0;gtsRemoteBlocks.Clear();remoteCubeGrids.Clear();if(CollectRemote
){GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(gtsRemoteBlocks,(x1=>!x1.IsSameConstructAs(_program.Me)&&
ValidBlock(x1)));fper=_program.Runtime.CurrentInstructionCount/(float)_program.Runtime.MaxInstructionCount;if(fper>0.75f)yield
return true;remoteBlocksCount=gtsRemoteBlocks.Count;foreach(var tb in gtsRemoteBlocks){if(!remoteCubeGrids.Contains(tb.
CubeGrid)){remoteCubeGrids.Add(tb.CubeGrid);}fper=_program.Runtime.CurrentInstructionCount/(float)_program.Runtime.
MaxInstructionCount;if(fper>0.75f){yield return true;}foreach(var handler in WicoRemoteBlockParseHandlers){fper=_program.Runtime.
CurrentInstructionCount/(float)_program.Runtime.MaxInstructionCount;if(fper>0.75f){yield return true;}handler(tb);}}}}void RemoteBlocksChanged(
){foreach(var handler in WicoLocalBlockChangedHandlers){handler();}}public void SetCollectRemote(bool bUse=true){
CollectRemote=bUse;}public List<IMyTerminalBlock>GetBlocksContains<T>(string Keyword=null)where T:class{var Output=new List<
IMyTerminalBlock>();if(gtsLocalBlocks.Count<1)LocalBlocksInit();for(int e1=0;e1<gtsLocalBlocks.Count;e1++){if(gtsLocalBlocks[e1]is T&&
Keyword!=null&&(gtsLocalBlocks[e1].CustomName.Contains(Keyword)||gtsLocalBlocks[e1].CustomData.Contains(Keyword))){Output.Add(
gtsLocalBlocks[e1]);}}return Output;}List<IMyTerminalBlock>gtsTestBlocks=new List<IMyTerminalBlock>();public bool CalcLocalGridChange(
bool bForceUpdate=false){gtsTestBlocks.Clear();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(gtsTestBlocks,(x1=>x1.
IsSameConstructAs(_program.Me)&&ValidBlock(x1)));if(localBlocksCount!=gtsTestBlocks.Count||bForceUpdate){_program.Echo(
"WBM:CGC:CHANGE DETECTED! New="+gtsTestBlocks.Count+" Old="+localBlocksCount);LocalBlocksChanged();localBlocksCount=gtsTestBlocks.Count;gtsLocalBlocks=
gtsTestBlocks;foreach(var tb in gtsLocalBlocks){foreach(var handler in WicoLocalBlockParseHandlers){handler(tb);}}return true;}return
false;}Vector3D _emptyV3D=new Vector3D();bool ValidBlock(IMyTerminalBlock tb){if(tb.GetPosition()==_emptyV3D){return false;}
else return true;}public bool CalcRemoteGridChange(){List<IMyTerminalBlock>gtsTestBlocks=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(gtsTestBlocks,(x1=>!x1.IsSameConstructAs(_program.Me)));if(remoteBlocksCount!=
gtsTestBlocks.Count){RemoteBlocksChanged();remoteBlocksCount=gtsTestBlocks.Count;gtsRemoteBlocks=gtsTestBlocks;foreach(var tb in
gtsRemoteBlocks){foreach(var handler in WicoRemoteBlockParseHandlers){handler(tb);}}return true;}return false;}const float
SMALL_BLOCK_VOLUME=0.5f;const float LARGE_BLOCK_VOLUME=2.5f;const float SMALL_BLOCK_LENGTH=0.5f;const float LARGE_BLOCK_LENGTH=2.5f;
private float _length_blocks,_width_blocks,_height_blocks;private double _length,_width,_height;public float gridsize;private
OrientedBoundingBoxFaces _obbf;IMyShipController shipdimController;void ShipDimensions(IMyShipController orientationBlock){shipdimController=
orientationBlock;if(_program.Me.CubeGrid.GridSizeEnum.ToString().ToLower().Contains("small"))gridsize=SMALL_BLOCK_LENGTH;else gridsize=
LARGE_BLOCK_LENGTH;_obbf=new OrientedBoundingBoxFaces(orientationBlock);Vector3D[]points=new Vector3D[4];_obbf.GetFaceCorners(
OrientedBoundingBoxFaces.LookupFront,points);_width=(points[0]-points[1]).Length();_height=(points[0]-points[2]).Length();_obbf.GetFaceCorners(0
,points);_length=(points[0]-points[2]).Length();_length_blocks=(float)(_length/gridsize);_width_blocks=(float)(_width/
gridsize);_height_blocks=(float)(_height/gridsize);}public float LengthInBlocks(){if(shipdimController==null)ShipDimensions(
GetMainController());return _length_blocks;}public double LengthInMeters(){if(shipdimController==null)ShipDimensions(GetMainController())
;return _length;}public float WidthInBlocks(){if(shipdimController==null)ShipDimensions(GetMainController());return
_width_blocks;}public double WidthInMeters(){if(shipdimController==null)ShipDimensions(GetMainController());return _width;}public
float HeightInBlocks(){if(shipdimController==null)ShipDimensions(GetMainController());return _height_blocks;}public double
HeightInMeters(){if(shipdimController==null)ShipDimensions(GetMainController());return _height;}public double LargestSideInMeters(){if
(shipdimController==null)ShipDimensions(GetMainController());double largest=_height;if(_length>largest)largest=_length;if
(_width>largest)largest=_width;return largest;}public double BlockMultiplier(){if(shipdimController==null)ShipDimensions(
GetMainController());return gridsize;}public void BlocksOnOff(List<IMyTerminalBlock>blocks,bool bOn=true){foreach(var b in blocks){
IMyFunctionalBlock f=b as IMyFunctionalBlock;if(f==null)continue;f.Enabled=bOn;}}public float DesiredMinTravelElevation=-1;}struct
OrientedBoundingBoxFaces{public Vector3D[]Corners;Vector3D localMax;Vector3D localMin;public Vector3D Position;static int[]PointsLookupRight={1,
3,5,7};static int[]PointsLookupLeft={0,2,4,6};static int[]PointsLookupTop={2,3,6,7};static int[]PointsLookupBottom={0,1,4
,5};static int[]PointsLookupBack={4,5,6,7};static int[]PointsLookupFront={0,1,2,3};static int[][]PointsLookup={
PointsLookupRight,PointsLookupLeft,PointsLookupTop,PointsLookupBottom,PointsLookupBack,PointsLookupFront};public const int LookupRight=0;
public const int LookupLeft=1;public const int LookupTop=2;public const int LookupBottom=3;public const int LookupBack=4;
public const int LookupFront=5;public OrientedBoundingBoxFaces(IMyTerminalBlock block){Corners=new Vector3D[8];if(block==null)
{Position=new Vector3D();localMin=new Vector3D();localMax=new Vector3D();return;}localMin=new Vector3D(block.CubeGrid.Min
)-new Vector3D(0.5,0.5,0.5);localMin*=block.CubeGrid.GridSize;localMax=new Vector3D(block.CubeGrid.Max)+new Vector3D(0.5,
0.5,0.5);localMax*=block.CubeGrid.GridSize;var blockOrient=block.WorldMatrix.GetOrientation();var matrix=block.CubeGrid.
WorldMatrix.GetOrientation()*MatrixD.Transpose(blockOrient);Vector3D.TransformNormal(ref localMin,ref matrix,out localMin);Vector3D
.TransformNormal(ref localMax,ref matrix,out localMax);var tmpMin=Vector3D.Min(localMin,localMax);localMax=Vector3D.Max(
localMin,localMax);localMin=tmpMin;var center=block.CubeGrid.GetPosition();Vector3D tmp2;Vector3D tmp3;tmp2=localMin;Vector3D.
TransformNormal(ref tmp2,ref blockOrient,out tmp2);tmp2+=center;tmp3=localMax;Vector3D.TransformNormal(ref tmp3,ref blockOrient,out
tmp3);tmp3+=center;BoundingBox bb=new BoundingBox(tmp2,tmp3);Position=bb.Center;Vector3D tmp;for(int i=0;i<8;i++){tmp.X=((i&
1)==0?localMin:localMax).X;tmp.Y=((i&2)==0?localMin:localMax).Y;tmp.Z=((i&4)==0?localMin:localMax).Z;Vector3D.
TransformNormal(ref tmp,ref blockOrient,out tmp);tmp+=center;Corners[i]=tmp;}}public void GetFaceCorners(int face,Vector3D[]points,int
index=0){face%=PointsLookup.Length;for(int i=0;i<PointsLookup[face].Length;i++){points[index++]=Corners[PointsLookup[face][i]
];}}}double CalculateYaw(Vector3D destination,IMyTerminalBlock Origin){double yawAngle=0;bool facingTarget=false;MatrixD
refOrientation=GetBlock2WorldTransform(Origin);Vector3D vCenter=Origin.GetPosition();Vector3D vBack=vCenter+1.0*Vector3D.Normalize(
refOrientation.Backward);Vector3D vRight=vCenter+1.0*Vector3D.Normalize(refOrientation.Right);Vector3D vLeft=vCenter-1.0*Vector3D.
Normalize(refOrientation.Right);double rightTargetDistance=calculateDistance(vRight,destination);double leftTargetDistance=
calculateDistance(vLeft,destination);double yawLocalDistance=calculateDistance(vRight,vLeft);double centerTargetDistance=Vector3D.
DistanceSquared(vCenter,destination);double backTargetDistance=Vector3D.DistanceSquared(vBack,destination);facingTarget=
centerTargetDistance<backTargetDistance;yawAngle=(leftTargetDistance-rightTargetDistance)/yawLocalDistance;if(!facingTarget){yawAngle+=(
yawAngle<0)?-1:1;}return yawAngle;}double calculateDistance(Vector3D a,Vector3D b){return Vector3D.Distance(a,b);}MatrixD
GetGrid2WorldTransform(IMyCubeGrid grid){Vector3D origin=grid.GridIntegerToWorld(new Vector3I(0,0,0));Vector3D plusY=grid.GridIntegerToWorld(
new Vector3I(0,1,0))-origin;Vector3D plusZ=grid.GridIntegerToWorld(new Vector3I(0,0,1))-origin;return MatrixD.CreateScale(
grid.GridSize)*MatrixD.CreateWorld(origin,-plusZ,plusY);}MatrixD GetBlock2WorldTransform(IMyCubeBlock blk){Matrix blk2grid;
blk.Orientation.GetMatrix(out blk2grid);return blk2grid*MatrixD.CreateTranslation(((Vector3D)new Vector3D(blk.Min+blk.Max))
/2.0)*GetGrid2WorldTransform(blk.CubeGrid);}class WicoControl:WicoUpdates{bool _bControlDebug=false;const string
MODECHANGETAG="[WICOMODECHANGE]";int _iMode=-1;int _iState=-1;string ControlSection="WicoControl";public int IMode{get{return _iMode;
}set{SetMode(value);}}public int IState{get{return _iState;}set{SetState(value);}}List<Action<int,int,int,int>>
ControlChangeHandlers=new List<Action<int,int,int,int>>();List<Action>ModeAfterInitHandlers=new List<Action>();public const int MODE_IDLE=0;
public const int MODE_DOCKING=30;public const int MODE_DOCKED=40;public const int MODE_LAUNCH=50;public const int
MODE_LAUNCHPREP=100;public const int MODE_ORBITALLAUNCH=120;public const int MODE_DESCENT=150;public const int MODE_ORBITALLAND=151;
public const int MODE_HOVER=170;public const int MODE_LANDED=180;public const int MODE_MINE=500;public const int MODE_GOTOORE=
510;public const int MODE_BORESINGLE=520;public const int MODE_EXITINGASTEROID=590;public const int MODE_STARTNAV=600;
public const int MODE_GOINGTARGET=650;public const int MODE_NAVNEXTTARGET=670;public const int MODE_ARRIVEDTARGET=699;public
const int MODE_DOSCANS=900;public const int MODE_ATTENTION=9999;StringBuilder sbData=new StringBuilder(100);public void
SetMode(int theNewMode,int theNewState=0){if(_iMode==theNewMode)return;sbData.Clear();sbData.AppendLine(_iMode.ToString());
sbData.AppendLine(_iState.ToString());sbData.AppendLine(theNewMode.ToString());sbData.AppendLine(theNewState.ToString());
SendToAllSubscribers(MODECHANGETAG,sbData.ToString());HandleModeChange(_iMode,_iState,theNewMode,theNewState);_iMode=theNewMode;_iState=
theNewState;WantOnce();}public void SetState(int theNewState){_iState=theNewState;}public bool AddControlChangeHandler(Action<int,
int,int,int>handler){if(!ControlChangeHandlers.Contains(handler))ControlChangeHandlers.Add(handler);return true;}void
HandleModeChange(int fromMode,int fromState,int toMode,int toState){foreach(var handler in ControlChangeHandlers){handler(fromMode,
fromState,toMode,toState);}}public bool AddModeInitHandler(Action handler){if(!ModeAfterInitHandlers.Contains(handler))
ModeAfterInitHandlers.Add(handler);return true;}public void ModeAfterInit(MyIni theIni){_iState=theIni.Get(ControlSection,"State").ToInt32(
_iState);_iMode=theIni.Get(ControlSection,"Mode").ToInt32(_iMode);foreach(var handler in ModeAfterInitHandlers){handler();}}
void SaveHandler(MyIni theIni){theIni.Set(ControlSection,"Mode",_iMode);theIni.Set(ControlSection,"State",_iState);}WicoIGC
_wicoIGC;TransmissionDistance localConstructs=TransmissionDistance.CurrentConstruct;public WicoControl(Program program,WicoIGC
wicoIGC):base(program){_program=program;_wicoIGC=wicoIGC;WicoControlInit();}List<long>_WicoMainSubscribers=new List<long>();
bool bIAmMain=true;string WicoMainTag="WicoTagMain";string YouAreSub="YOUARESUB";string UnicastTagTrigger="TRIGGER";string
UnicastAnnounce="IAMWICO";public void WicoControlInit(){_WicoMainSubscribers.Clear();_program.IGC.SendBroadcastMessage(WicoMainTag,
"Configure",localConstructs);_wicoIGC.AddPublicHandler(WicoMainTag,WicoControlMessagehandler,true);_wicoIGC.AddUnicastHandler(
WicoConfigUnicastListener);_program.AddTriggerHandler(ProcessTrigger);_bControlDebug=_program._CustomDataIni.Get(_program.OurName,"ControlDebug")
.ToBoolean(_bControlDebug);_program._CustomDataIni.Set(_program.OurName,"ControlDebug",_bControlDebug);_program.
AddSaveHandler(SaveHandler);}public bool IamMain(){return bIAmMain;}public void ProcessTrigger(string sArgument,MyCommandLine
myCommandLine,UpdateType updateSource){if(myCommandLine!=null&&myCommandLine.ArgumentCount>1){if(myCommandLine.Argument(0)=="setmode"
){int toMode=0;bool bOK=int.TryParse(myCommandLine.Argument(1),out toMode);if(bOK){SetMode(toMode);WantOnce();}}}}public
void SendToAllSubscribers(string tag,string argument){foreach(var submodule in _WicoMainSubscribers){if(submodule==_program.
Me.EntityId)continue;_program.IGC.SendUnicastMessage(submodule,tag,argument);}}public void WicoControlMessagehandler(
MyIGCMessage msg){var tag=msg.Tag;var src=msg.Source;if(tag==WicoMainTag){if(msg.Data is string){string data=(string)msg.Data;if(
data=="Configure"){_program.IGC.SendUnicastMessage(src,UnicastAnnounce,"");}}}}public void WicoConfigUnicastListener(
MyIGCMessage msg){var tag=msg.Tag;var src=msg.Source;if(tag==YouAreSub){bIAmMain=false;}else if(tag==UnicastAnnounce){if(
_WicoMainSubscribers.Contains(src)){}else{_WicoMainSubscribers.Add(src);}bIAmMain=true;foreach(var other in _WicoMainSubscribers){if(other<
_program.Me.EntityId){bIAmMain=false;}}}else if(tag==UnicastTagTrigger){}else if(tag==MODECHANGETAG){string[]aLines=((string)msg
.Data).Split('\n');int theNewMode=Convert.ToInt32(aLines[2]);int theNewState=Convert.ToInt32(aLines[3]);if(_iMode!=
theNewMode)HandleModeChange(_iMode,_iState,theNewMode,theNewState);_iMode=theNewMode;_iState=theNewState;}}public new void
AnnounceState(){if(_bControlDebug){}if(bIAmMain)_program.Echo("MAIN. Mode="+IMode.ToString()+" S="+IState.ToString());else _program.
Echo("SUB. Mode="+IMode.ToString()+" S="+IState.ToString());}}class WicoUpdates{protected Program _program;public bool
_bUpdateDebug=false;public WicoUpdates(Program program){_program=program;_bUpdateDebug=_program._CustomDataIni.Get(_program.OurName,
"UpdateDebug").ToBoolean(_bUpdateDebug);_program._CustomDataIni.Set(_program.OurName,"UpdateDebug",_bUpdateDebug);}public float
fMaxWorldMps=100f;bool bWantOnce=false;bool bWantFast=false;bool bWantMedium=false;bool bWantSlow=false;public void ResetUpdates(){
bWantOnce=false;bWantFast=false;bWantMedium=false;bWantSlow=false;}public void WantOnce(){bWantOnce=true;}public void WantFast(){
bWantFast=true;}public void WantMedium(){bWantMedium=true;}public void WantSlow(){bWantSlow=true;}public UpdateFrequency
GenerateUpdate(){UpdateFrequency desired=0;if(bWantOnce)desired|=UpdateFrequency.Once;if(bWantFast)desired|=UpdateFrequency.Update1;if
(bWantMedium)desired|=UpdateFrequency.Update10;if(bWantSlow)desired|=UpdateFrequency.Update100;return desired;}public
void AnnounceState(){_program.Echo("Standalone Control");}}