/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */
const string AMMO_SUPPLY="[WEP]";const string PAYLOAD="[MIN]";const string ORE_DEST="[ORE]";const string COMP_SUPPLY=
"[CMP]";const string COMP_TAG="[CST]";const string FUEL_SUPPLY="[SRC]";const string REACTOR="[PWR]";const string GAS_TAG=
"[H2O]";const string ICE_SUPPLY="[ORE]";const string GEAR_TAG="[LG]";const string DISPLAY_TAG="USAP Display";const string
MENU_TAG="[MENU]";const string COMMS_SCREEN_TAG="[COMMS";const string RECEIVER_TAG="-RCV]";const char COMMS_SEPARATOR=';';const
string SLASHES="///////////////";const string DASHES=" ------------------- ";const double TIME_STEP=1.0/6.0;const double KP=
0.25;const double KI=0;const double KD=0;const double INVERSE_GAIN=3;const string HEADER="UNIVERSAL SHIP ASSISTANT PROGRAM";
const string INI_HEAD="USAP";const string MAG_TAG="[MAG";const string TRIGGER_HEAD="USAP Triggers";const string PROFILE_HEAD=
"USAP Construction Profiles";const string GATLING="GATLING";const string MISSILE="MISSILE";const string ARTILLERY="ARTILLERY";const string ASSAULT=
"ASSAULT";const string AUTO="AUTO";const string RAIL="RAIL";const string MINI_RAIL="MINI-RAIL";const string LOADOUT="Loadout";
const string PROFILE_LIST="Basic,Advanced,Armor";const string DEFAULT_PROFILE="BulletproofGlass:0\n"+"Computer:0\n"+
"Construction:0\n"+"Detector:0\n"+"Display:0\n"+"Explosives:0\n"+"Girder:0\n"+"GravityGenerator:0\n"+"InteriorPlate:0\n"+"LargeTube:0\n"+
"Medical:0\n"+"MetalGrid:0\n"+"Motor:0\n"+"PowerCell:0\n"+"RadioCommunication:0\n"+"Reactor:0\n"+"SmallTube:0\n"+"SolarCell:0\n"+
"SteelPlate:0\n"+"Superconductor:0\n"+"Thrust:0";const string DEFAULT_ARMOR="BulletproofGlass:0\n"+"Computer:0\n"+"Construction:0\n"+
"Detector:0\n"+"Display:0\n"+"Explosives:0\n"+"Girder:0\n"+"GravityGenerator:0\n"+"InteriorPlate:0\n"+"LargeTube:0\n"+"Medical:0\n"+
"MetalGrid:651\n"+"Motor:0\n"+"PowerCell:0\n"+"RadioCommunication:0\n"+"Reactor:0\n"+"SmallTube:0\n"+"SolarCell:0\n"+"SteelPlate:1953\n"+
"Superconductor:0\n"+"Thrust:0";const string DEFAULT_BASIC="BulletproofGlass:0\n"+"Computer:300\n"+"Construction:1250\n"+"Detector:0\n"+
"Display:0\n"+"Explosives:0\n"+"Girder:0\n"+"GravityGenerator:0\n"+"InteriorPlate:250\n"+"LargeTube:75\n"+"Medical:0\n"+
"MetalGrid:150\n"+"Motor:100\n"+"PowerCell:0\n"+"RadioCommunication:0\n"+"Reactor:0\n"+"SmallTube:500\n"+"SolarCell:0\n"+
"SteelPlate:1550\n"+"Superconductor:0\n"+"Thrust:0";const string DEFAULT_ADVANCED="BulletproofGlass:50\n"+"Computer:750\n"+
"Construction:1000\n"+"Detector:25\n"+"Display:25\n"+"Explosives:0\n"+"Girder:25\n"+"GravityGenerator:4\n"+"InteriorPlate:125\n"+
"LargeTube:25\n"+"Medical:0\n"+"MetalGrid:25\n"+"Motor:100\n"+"PowerCell:25\n"+"RadioCommunication:10\n"+"Reactor:100\n"+
"SmallTube:250\n"+"SolarCell:50\n"+"SteelPlate:1000\n"+"Superconductor:150\n"+"Thrust:75";const string AMMO="NATO_25x184mm";const string
MISL="Missile200mm";const string FUEL="Uranium";const int RUN_CAP=10;static string _statusMessage;const float PI=(float)Math
.PI;static IMyProgrammableBlock _Me;static int _loadCount;static string _currentPower;static string _activeProfile;int
_runningNumber;bool _unloaded;bool _hasComponentCargo;static bool _cruiseThrustersOn;IMyShipController _cockpit;float _maxSpeed=100;
PID _pid;double _Kp;bool _gravityDisengage;int _safetyElevation;IMyTerminalBlock _refBlock;List<IMyTerminalBlock>_magazines
;List<IMyTerminalBlock>_reactors;static List<IMyTerminalBlock>_miningCargos;static List<IMyTerminalBlock>
_constructionCargos;List<IMyTerminalBlock>_o2Generators;static List<Display>_displays;static LandingGearAssembly _landingGear;string
_cruiseTag;float _cruiseFactor=1;static List<IMyThrust>_cruiseThrusters;static string[]_breather={"|","/","--","\\","|","/","--",
"\\"};static Byte _breath;static bool _autoCycle;Program(){if(Storage.Length>0){string[]storageData=Storage.Split(';');try{
_loadCount=int.Parse(storageData[0]);}catch{_loadCount=0;}try{_cruiseThrustersOn=ParseBool(storageData[1]);}catch{
_cruiseThrustersOn=false;}try{_targetThrottle=ParseFloat(storageData[2],0);if(_targetThrottle<0)_targetThrottle=0;}catch{_targetThrottle=0
;}}else{_loadCount=0;_cruiseThrustersOn=false;_targetThrottle=0;}Build();}void Save(){string loadCount=_loadCount.
ToString();string cruiseActive=_cruiseThrustersOn.ToString();string targetThrottle=_targetThrottle.ToString();Storage=loadCount+
";"+cruiseActive+";"+targetThrottle;}void Main(string argument,UpdateType updateSource){_unloaded=false;if(_autoCycle)
PrintHeader();RunLast();if(TriggerCall(updateSource)){Echo("Triggering: "+argument);MainSwitch(argument);}else if(_commsEnabled){
ExecuteComms(updateSource);}Echo("STATUS: "+_statusMessage);Echo("Displays: "+_displays.Count);Echo("Turrets: "+_turretString);
ShowBroadcastData();if(_unloaded){_loadCount++;Echo("Load Count: "+_loadCount);}if(_cruiseThrusters.Count>0&&_cockpit!=null){Echo(
"Max Thrust:\n"+_totalThrust.ToString("0.0")+"N\n");Echo("Thrust to Weight Ratio:\n"+_thrustWeightRatio+"\n");Echo("P-Gain:\n"+_Kp+"\n"
);}if(_cruiseThrustersOn){double velocity=GetForwardVelocity();double error=_targetThrottle-velocity;double control=_pid.
Control(error);ThrottleThrusters((float)control);if(_runningNumber>RUN_CAP){_runningNumber=0;SafetyCheck();CheckGravity();}}
PrintDisplays();}void Activate(string trigger){List<IMyTimerBlock>timers=new List<IMyTimerBlock>();GridTerminalSystem.GetBlocksOfType
<IMyTimerBlock>(timers);if(timers.Count<1||_refBlock==null)return;List<IMyTimerBlock>triggerTimers=new List<IMyTimerBlock
>();foreach(IMyTimerBlock timer in timers){if(timer.CustomName.Contains(trigger))triggerTimers.Add(timer);}if(
triggerTimers.Count<1){_statusMessage="No Timers of name "+trigger+" found.";return;}IMyTimerBlock timerToTrigger=triggerTimers[0];
var distance=Vector3D.Distance(_refBlock.GetPosition(),timerToTrigger.GetPosition());foreach(IMyTimerBlock triggerTimer in
triggerTimers){var newDistance=Vector3D.Distance(_refBlock.GetPosition(),triggerTimer.GetPosition());if(newDistance<distance){
distance=newDistance;timerToTrigger=triggerTimer;}}timerToTrigger.GetActionWithName("TriggerNow").Apply(timerToTrigger);
_statusMessage+="Activating: "+timerToTrigger.CustomName+"\n";}void TriggerCall(string arg){if(arg.ToUpper().StartsWith("TRIGGER_")){
string[]args=arg.Split('_');if(args.Length<2){_statusMessage="Invalid Trigger Command: "+arg;return;}string triggerKey=
"Trigger_"+args[1];if(Me.CustomData.Contains(triggerKey)){string timerName=_programIniHandler.GetKey(TRIGGER_HEAD,triggerKey,"");
if(timerName=="")_statusMessage+="No timer name provided for Trigger Command \""+triggerKey+
"\"!\n  Please check Custom Data.\n";else Activate(timerName);}else{_statusMessage+="No defined Trigger \""+triggerKey+
"\" found!\n  Please check case and spelling.\n";}return;}_statusMessage="UNRECOGNIZED COMMAND: "+arg;}void Reload(IMyTerminalBlock destination,List<IMyTerminalBlock>
supplyBlocks){string[][]loadouts=StringTo2DArray(GetKey(destination,INI_HEAD,LOADOUT,""),'\n',':');if(supplyBlocks.Count<1||!
destination.HasInventory||loadouts.Length<1){return;}IMyInventory magInv=destination.GetInventory(0);foreach(IMyTerminalBlock
supply in supplyBlocks){if(supply.HasInventory){IMyInventory supplyInv=supply.GetInventory(0);for(int i=0;i<loadouts.Length;i
++){ensureMinimumAmount(supplyInv,magInv,loadouts[i][0],ParseInt(loadouts[i][1],1));}}}}void Restock(List<IMyTerminalBlock
>destBlocks,string sourceTag){if(destBlocks.Count<1)return;List<IMyTerminalBlock>sourceBlocks=new List<IMyTerminalBlock>(
);GridTerminalSystem.SearchBlocksOfName(sourceTag,sourceBlocks);if(sourceBlocks.Count<1)return;foreach(IMyTerminalBlock
destBlock in destBlocks){Echo("Resupply: "+destBlock.CustomName);Reload(destBlock,sourceBlocks);}}void Unstock(List<
IMyTerminalBlock>sourceBlocks,string destTag,bool includeComponents){if(sourceBlocks.Count<1)return;List<IMyTerminalBlock>destBlocks=new
List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(destTag,destBlocks);foreach(IMyTerminalBlock sourceBlock in
sourceBlocks){Unload(sourceBlock,destBlocks,includeComponents);}}void Unload(IMyTerminalBlock payload,List<IMyTerminalBlock>
destBlocks,bool includeComponents){if(destBlocks.Count<1)return;Echo("Unloading "+payload.CustomName);var sourceInv=payload.
GetInventory(0);foreach(IMyTerminalBlock container in destBlocks){Echo("Destination: "+container.CustomName);if(container.
HasInventory){var destInv=container.GetInventory(0);if(!destInv.IsFull){List<MyInventoryItem>items=new List<MyInventoryItem>();
sourceInv.GetItems(items);if(items.Count>0){foreach(MyInventoryItem item in items){if(item.Type.ToString().Contains(
"MyObjectBuilder_Ore")||(includeComponents&&item.Type.ToString().Contains("MyObjectBuilder_Component"))){sourceInv.TransferItemTo(destInv,0,
null,true,null);_unloaded=true;}}}}}}}void SetLoadCount(string arg){int value=ParseInt(arg,0);if(value<0){Echo(
"INVALID LOAD COUNT VALUE: "+arg);return;}_loadCount=value;}void AssignCockpit(){List<IMyShipController>controllers=new List<IMyShipController>();
GridTerminalSystem.GetBlocksOfType<IMyShipController>(controllers);if(controllers.Count<1){_statusMessage+="NO CONTROLLERS FOUND!\n";
return;}string name=_programIniHandler.GetKey(CRUISE_HEADER,"Cockpit","");foreach(IMyShipController controller in controllers)
{if(controller.CustomName==name){_cockpit=controller;return;}}_cockpit=controllers[0];_programIniHandler.SetKey(
CRUISE_HEADER,"Cockpit",_cockpit.CustomName);}void PrintHeader(){Echo("UNIVERSAL SHIP                 "+_breather[_breath]+
"\nASSISTANT PROGRAM\n"+DASHES);_breath++;if(_breath>=_breather.Length)_breath=0;}void Build(){_Me=Me;_breath=0;_programIniHandler=new
MyIniHandler(Me);_statusMessage="";_gridID=_programIniHandler.GetKey(SHARED,"Grid_ID",Me.CubeGrid.EntityId.ToString());
_hasComponentCargo=false;_runningNumber=0;_currentPower="OFF";string refTag=_programIniHandler.GetKey(INI_HEAD,"Reference",Me.CustomName);
try{_refBlock=GridTerminalSystem.GetBlockWithName(refTag);Echo(_refBlock.CustomName);}catch{_refBlock=Me;}
_programIniHandler.EnsureKey(TRIGGER_HEAD,"Trigger_Door","Door Timer");_magazines=new List<IMyTerminalBlock>();_reactors=new List<
IMyTerminalBlock>();_constructionCargos=new List<IMyTerminalBlock>();_miningCargos=new List<IMyTerminalBlock>();_o2Generators=new List<
IMyTerminalBlock>();_displays=new List<Display>();List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();GridTerminalSystem.
GetBlocksOfType<IMyTerminalBlock>(blocks);foreach(IMyTerminalBlock block in blocks){if(block.HasInventory&&GetKey(block,SHARED,
"Grid_ID",_gridID)==_gridID)AddToInventories(block);}if(_hasComponentCargo){_activeProfile=_programIniHandler.GetKey(INI_HEAD,
"Profiles",PROFILE_LIST).Split(',')[0];EnsureProfiles();}AssignThrusters();AssembleLandingGear();if(_cruiseThrusters.Count>0){
AssignCockpit();_maxSpeed=ParseFloat(_programIniHandler.GetKey(CRUISE_HEADER,"Max Speed","100"),100);SetThrustWeightRatio();SetGain()
;_safetyElevation=ParseInt(_programIniHandler.GetKey(CRUISE_HEADER,"Safety Height","1000"),1000);if(_safetyElevation<0)
_safetyElevation*=-1;_gravityDisengage=ParseBool(_programIniHandler.GetKey(CRUISE_HEADER,"Zero-G Disable","True"));Echo("Max Thrust:\n"+
_totalThrust.ToString("0.0")+"N\n");Echo("Thrust to Weight Ratio:\n"+_thrustWeightRatio+"\n");}if(_cruiseThrustersOn)
CruiseThrustersOn();else CruiseThrustersOff();AssignMultiTimers();AssignDisplayTurrets();AssignDisplayRelays();AssignDisplays();
PrintDisplays();if(_displays.Count<1){_turrets.Clear();_transponders.Clear();}AssignComms();SetUpdateFrequency();}void
SetUpdateFrequency(){if(_cruiseThrusters.Count>0||_landingGear!=null||_commsEnabled||(_turrets.Count>0&&_displays.Count>0)){Runtime.
UpdateFrequency=UpdateFrequency.Update10;_autoCycle=true;return;}_autoCycle=false;}void SetMagAmounts(IMyTerminalBlock block){string
name=block.CustomName;if((!name.Contains(MAG_TAG+"]")&&!name.Contains(":"))||GetKey(block,SHARED,"Grid_ID",_gridID)!=_gridID
)return;string tag=TagFromName(name);string loadout="";switch(tag.ToUpper()){case"GENERAL":loadout="NATO_25x184mm:1\n"+
"Missile200mm:1\n"+"LargeCalibreAmmo:1\n"+"MediumCalibreAmmo:1\n"+"AutocannonClip:1\n"+"LargeRailgunAmmo:1\n"+"SmallRailgunAmmo:1";break;
case GATLING:loadout="NATO_25x184mm:7";break;case MISSILE:loadout="Missile200mm:4";break;case ARTILLERY:loadout=
"LargeCalibreAmmo:3";break;case ASSAULT:loadout="MediumCalibreAmmo:2";break;case AUTO:loadout="AutocannonClip:5";break;case RAIL:loadout=
"LargeRailgunAmmo:1";break;case MINI_RAIL:loadout="SmallRailgunAmmo:6";break;}EnsureKey(block,INI_HEAD,LOADOUT,loadout);}void EnsureProfiles
(){string[]profiles=_programIniHandler.GetKey(PROFILE_HEAD,"Profiles",PROFILE_LIST).Split(',');if(profiles.Length<1)
return;foreach(string profile in profiles){string header="Profile: "+profile.Trim();string loadout;switch(profile.Trim().
ToUpper()){case"BASIC":loadout=DEFAULT_BASIC;break;case"ADVANCED":loadout=DEFAULT_ADVANCED;break;case"ARMOR":loadout=
DEFAULT_ARMOR;break;default:loadout=DEFAULT_PROFILE;break;}EnsureKey(Me,header,LOADOUT,loadout);}UpdateProfiles();}void SelectProfile
(string profileName){if(profileName==""||!Me.CustomData.ToLower().Contains(profileName.ToLower()+"]")){_statusMessage=
"No Profile named \""+profileName+"\" found!";return;}SetActiveProfile(profileName);UpdateProfiles();Unstock(_constructionCargos,COMP_SUPPLY,
true);Restock(_constructionCargos,COMP_SUPPLY);}string[][]GetActiveProfile(){string[]profiles=_programIniHandler.GetKey(
PROFILE_HEAD,"Profiles",PROFILE_LIST).Split(',');if(profiles.Length>0){string activeProfile="Profile: "+profiles[0].Trim();return
StringTo2DArray(_programIniHandler.GetKey(activeProfile,LOADOUT,DEFAULT_BASIC),'\n',':');}else{return StringTo2DArray(DEFAULT_BASIC,
'\n',':');}}void SetActiveProfile(string profileName){string[]profiles=_programIniHandler.GetKey(PROFILE_HEAD,"Profiles","")
.Split(',');if(profiles.Length<2)return;for(int i=1;i<profiles.Length;i++){if(profiles[i].Trim().ToLower()==profileName.
ToLower()){_activeProfile=profileName;profiles[i]=profiles[0];profiles[0]=profileName;string profileList=profiles[0];for(int j=
1;j<profiles.Length;j++){profileList+=","+profiles[j];}SetKey(Me,INI_HEAD,"Profiles",profileList);return;}}}void
UpdateProfiles(){if(_constructionCargos.Count<1||!_hasComponentCargo)return;string[][]profileData=GetActiveProfile();foreach(
IMyTerminalBlock block in _constructionCargos){if(block.CustomName.Contains(COMP_TAG)&&ParseBool(GetKey(block,INI_HEAD,
"Sync_To_Profiles","false"))){float volumeRatio=(float)block.GetInventory(0).MaxVolume/15.625f;string loadout="";for(int c=0;c<profileData
.GetLength(0);c++){double amount=Math.Floor((float)ParseInt(profileData[c][1],0)*volumeRatio);string number=((int)amount)
.ToString();loadout+=profileData[c][0]+":"+number+"\n";}SetKey(block,INI_HEAD,LOADOUT,loadout);}}}bool
ConnectedToComponentSupply(){List<IMyTerminalBlock>compSupplies=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(COMP_SUPPLY,
compSupplies);if(compSupplies.Count>0)return true;else return false;}void AddToInventories(IMyTerminalBlock block){string name=block
.CustomName;if(name.Contains(PAYLOAD)){_miningCargos.Add(block);}else if(name.Contains(MAG_TAG)){SetMagAmounts(block);
_magazines.Add(block);}else if(name.Contains(COMP_TAG)){EnsureKey(block,INI_HEAD,"Sync_To_Profiles","True");EnsureKey(block,
INI_HEAD,LOADOUT,DEFAULT_PROFILE);_hasComponentCargo=true;_constructionCargos.Add(block);}else if(name.Contains(REACTOR)&&block.
BlockDefinition.TypeIdString.ToLower().Contains("reactor")){EnsureKey(block,INI_HEAD,LOADOUT,"Ingot/Uranium:100");_reactors.Add(block);
}else if(name.Contains(GAS_TAG)&&block.BlockDefinition.TypeIdString.ToLower().Contains("oxygengenerator")){EnsureKey(
block,INI_HEAD,LOADOUT,"Ore/Ice:2702");_o2Generators.Add(block);}}string TagFromName(string name){string tag="";if(name.
Contains(MAG_TAG+"]"))return"GENERAL";int start=name.IndexOf(MAG_TAG)+MAG_TAG.Length+1;Echo("Start: "+start);tag=name.Substring(
start);Echo(tag);int length=tag.IndexOf("]");Echo("Length: "+length);tag=tag.Substring(0,length);Echo(tag);return tag;}void
EnsureLoadout(IMyTerminalBlock block,int[]amounts){if(amounts.Length<7)amounts=new int[]{0,0,0,0,0,0,0};EnsureKey(block,INI_HEAD,
"NATO_25x184mm",amounts[0].ToString());EnsureKey(block,INI_HEAD,"Missile200mm",amounts[1].ToString());EnsureKey(block,INI_HEAD,
"LargeCalibreAmmo",amounts[2].ToString());EnsureKey(block,INI_HEAD,"MediumCalibreAmmo",amounts[3].ToString());EnsureKey(block,INI_HEAD,
"AutocannonClip",amounts[4].ToString());EnsureKey(block,INI_HEAD,"LargeRailgunAmmo",amounts[5].ToString());EnsureKey(block,INI_HEAD,
"SmallRailgunAmmo",amounts[6].ToString());}string[][]StringTo2DArray(string source,char separatorOuter,char separatorInner){return source.
Split(separatorOuter).Select(x=>x.Split(separatorInner)).ToArray();}void ensureMinimumAmount(IMyInventory source,IMyInventory
dest,string itemType,int num){if(num<1)return;int initialSupply=numberOfItemInContainer(dest,itemType);while(!
hasEnoughOfItem(dest,itemType,num)){int?index=indexOfItem(source,itemType);if(index==null)return;source.TransferItemTo(dest,(int)index,
null,true,num-numberOfItemInContainer(dest,itemType));if(numberOfItemInContainer(dest,itemType)==initialSupply){
_statusMessage="WARNING: Failed to transfer item of type "+itemType+"!";return;}}}bool hasEnoughOfItem(IMyInventory inventoryToSearch,
string itemName,int minAmount){return numberOfItemInContainer(inventoryToSearch,itemName)>=minAmount;}int
numberOfItemInContainer(IMyInventory inventoryToSearch,string itemName){int total=0;List<MyInventoryItem>items=new List<MyInventoryItem>();
inventoryToSearch.GetItems(items);for(int c=0;c<items.Count;c++){if(items[c].Type.ToString().Contains(itemName)){total+=(int)(items[c].
Amount);}}return total;}Nullable<int>indexOfItem(IMyInventory source,string item){List<MyInventoryItem>items=new List<
MyInventoryItem>();source.GetItems(items);for(int c=0;c<items.Count;c++){if(items[c].Type.ToString().Contains(item)){return c;}}return
null;}const string COMMS_HEADER="USAP Comms";const string DF_LCD_COMTAG="CH.";const string LCD_COMMS_LABEL="LCD Channel";
const string SUB_TAG="SubChannel";const string BROADCAST="BROADCAST";const string LISTEN="LISTEN";const string CONNECT_LABEL=
"Connected Color";const string DISCONNECT_LABEL="Disconnected Color";const string DISCONNECT_MSG="||| DISCONNECTED |||";const string
UNCONNECT_MSG="Loading...";const string DF_CONNECT_COLOR="0,127,0";const string DF_DISCONNECT_COLOR="24,24,24";bool _commsEnabled;
static Dictionary<int,ICommsScreen>_receiverScreens;static Dictionary<string,ICommsScreen>_broadcasterScreens;static List<
string>_broadcasterKeys;static List<int>_receiverKeys;int _currentBcScreen=0;int _currentRcScreen=0;int _broadcastTick=-1;
static int _listenerTimeOut=20;static string _bcID;interface ICommsScreen{IMyTextSurface TextSurface{get;set;}string
ChannelTag{get;set;}}class LcdRecieverScreen:ICommsScreen{public IMyTextSurface TextSurface{get;set;}public DateTime
LastLcdReceipt{get;set;}public string ChannelTag{get;set;}public string SubChannelTag{get;set;}public string LastMessage{get;set;}
public bool IsConnected{get;set;}public Color ConnectedColor{get;set;}public Color DisconnectColor{get;set;}public
CommsScreenBlock Parent{get;set;}public int ReceiverKey{get;set;}public int ScreenIndex{get;set;}public LcdRecieverScreen(IMyTextSurface
textSurface,string channelTag,string subChannelTag,Color connected,Color disconnected,CommsScreenBlock parent,int receiverKey,int
screenIndex){TextSurface=textSurface;TextSurface.ContentType=ContentType.TEXT_AND_IMAGE;ChannelTag=channelTag;SubChannelTag=
subChannelTag;ConnectedColor=connected;DisconnectColor=disconnected;TextSurface.FontColor=DisconnectColor;IsConnected=false;
LastLcdReceipt=DateTime.MinValue;ScreenIndex=screenIndex;LoadLastMessage();MessageToScreen(LastMessage,"",true);Parent=parent;
ReceiverKey=receiverKey;}private void LoadLastMessage(){StringBuilder stringBuilder=new StringBuilder();TextSurface.ReadText(
stringBuilder);string oldMessage=stringBuilder.ToString();string msg="";string[]lines=oldMessage.Split('\n');if(lines.Length>1){for(
int i=1;i<lines.Length;i++){msg+=lines[i]+"\n";}}LastMessage=msg;}public void SetSubChannel(string subChannelTag){
SubChannelTag=subChannelTag;Parent.IniHandler.SetKey(COMMS_HEADER+" Screen "+ScreenIndex,SUB_TAG,subChannelTag);}public void
MessageToScreen(string message,string displayName,bool firstRun=false){string status;if(firstRun)status=UNCONNECT_MSG;else status=
"Connected";string id="";if(_receiverScreens.Count>1)id="("+ReceiverKey+") ";LastMessage=message;TextSurface.WriteText(id+
ChannelTag+" : "+displayName+" - "+status+"\n"+LastMessage);if(!IsConnected){IsConnected=true;TextSurface.FontColor=ConnectedColor
;TextSurface.ClearImagesFromSelection();}}public void Disconnect(string reason){string id="";if(_receiverScreens.Count>1)
id="("+ReceiverKey+") ";IsConnected=false;TextSurface.FontColor=DisconnectColor;TextSurface.ClearImagesFromSelection();
TextSurface.AddImageToSelection("Danger");TextSurface.WriteText(id+ChannelTag+" - "+DISCONNECT_MSG+"\n"+reason+"\n"+LastMessage);}
public void CycleChannel(bool previous=false){string screenLabel="Screen Channel";if((Parent.Block as IMyTextSurfaceProvider).
SurfaceCount>1)screenLabel+=" "+ScreenIndex;ChannelTag=CycleListener(ChannelTag,previous);Parent.IniHandler.SetKey(COMMS_HEADER,
screenLabel,LISTEN+":"+ChannelTag);}public void CycleSubChannel(bool previous=false){if(_channels.Count<1){_statusMessage+=
"No Channels Detected!\n";return;}if(!_channels.Keys.Contains(ChannelTag)){CycleChannel(previous);}Channel channel=_channels[ChannelTag];
SubChannelTag=channel.CycleSubChannel(SubChannelTag,previous);}}class LcdBroadcasterScreen:ICommsScreen{public IMyTextSurface
TextSurface{get;set;}public string ChannelTag{get;set;}public LcdBroadcasterScreen(IMyTextSurface textSurface,string broadcastTag){
TextSurface=textSurface;TextSurface.ContentType=ContentType.TEXT_AND_IMAGE;ChannelTag=broadcastTag;}}class CommsScreenBlock{public
IMyTerminalBlock Block{get;set;}public MyIniHandler IniHandler{get;set;}public int SurfaceCount{get;set;}public CommsScreenBlock(
IMyTerminalBlock block,MyIniHandler iniHandler){Block=block;SurfaceCount=(Block as IMyTextSurfaceProvider).SurfaceCount;IniHandler=
iniHandler;}public void AssignScreens(){if(SurfaceCount==1)AssignSingleScreen();else if(SurfaceCount>1)AssignMultiScreen();}
private void AssignSingleScreen(int index=0,bool multiscreen=false,bool useDefaultValue=true){string screenId;if(multiscreen)
screenId=" "+index.ToString();else screenId="";string defaultValue;if(useDefaultValue){if(Block.CustomName.Contains(RECEIVER_TAG
))defaultValue=LISTEN;else defaultValue=BROADCAST;defaultValue+=":"+DF_LCD_COMTAG+index;}else{defaultValue="";}
IMyTextSurface surface=(Block as IMyTextSurfaceProvider).GetSurface(index);string[]channelData=IniHandler.GetKey(COMMS_HEADER,
"Screen Channel"+screenId,defaultValue).Split(':');if(channelData.Length!=2)return;string cmd=channelData[0].ToUpper();string channel=
channelData[1];try{if(cmd==BROADCAST){_broadcasterScreens.Add(channel,new LcdBroadcasterScreen(surface,channel));}else if(cmd==
LISTEN){string screenHeader=COMMS_HEADER+" Screen "+index;Color connectedColor=ParseColor(IniHandler.GetKey(screenHeader,
CONNECT_LABEL,DF_CONNECT_COLOR));Color disconnectColor=ParseColor(IniHandler.GetKey(screenHeader,DISCONNECT_LABEL,DF_DISCONNECT_COLOR
));string subChannel=IniHandler.GetKey(screenHeader,SUB_TAG,"");int receiverKey=ParseInt(IniHandler.GetKey(screenHeader,
"Screen "+index+" ID",_receiverScreens.Keys.Count().ToString()),_receiverScreens.Keys.Count());if(_receiverScreens.ContainsKey(
receiverKey)){receiverKey=GetUnusedKey(receiverKey,_receiverScreens);}_receiverScreens.Add(receiverKey,new LcdRecieverScreen(
surface,channel,subChannel,connectedColor,disconnectColor,this,receiverKey,index));}}catch(Exception ex){_statusMessage+=
"Error Adding "+Block.CustomName+"\n At: "+channel+"\n"+ex.Message+"\n";}}private void AssignMultiScreen(){bool isFirstScreen=true;for(
int i=0;i<SurfaceCount;i++){string screenId=" "+i.ToString();AssignSingleScreen(i,true,isFirstScreen);isFirstScreen=false;}
}}void AssignComms(){_receiverScreens=new Dictionary<int,ICommsScreen>();_broadcasterScreens=new Dictionary<string,
ICommsScreen>();List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(COMMS_SCREEN_TAG,
blocks);if(blocks.Count>0){foreach(IMyTerminalBlock block in blocks){MyIniHandler ini=new MyIniHandler(block);if(ini.
HasSameGridId()){CommsScreenBlock screenBlock=new CommsScreenBlock(block,ini);screenBlock.AssignScreens();}}}_receiverKeys=
_receiverScreens.Keys.ToList<int>();_commsEnabled=_broadcasterScreens.Count()+_receiverScreens.Count()>0;if(_broadcasterScreens.Count>0)
{_broadcastTick=Math.Abs((int)Me.CubeGrid.EntityId)%_breather.Length;_broadcasterKeys=_broadcasterScreens.Keys.ToList();
AssignBroadCastID();}if(_receiverScreens.Count>0)AssignChannels();}void ExecuteComms(UpdateType updateSource){if((updateSource&UpdateType
.IGC)>0){ReceiveMessages();}else if(_broadcasterScreens.Count>0){BroadcastCurrentScreen();}UpdateCurrentReceiver();}void
ShowBroadcastData(){if(!_commsEnabled)return;Echo("Broadcast Screen: "+_broadcasterScreens.Count());Echo("Receiver Screens: "+
_receiverScreens.Count());Echo("Broadcast Tick:"+_broadcastTick);if(_breath==_broadcastTick)Echo("BROADCASTING");}int
GetCurrentBroadCasterIndex(){_currentBcScreen++;if(_currentBcScreen>=_broadcasterScreens.Count)_currentBcScreen=0;return _currentBcScreen;}int
GetCurrentRecieverKey(){_currentRcScreen++;if(_currentRcScreen>=_receiverKeys.Count)_currentRcScreen=0;return _receiverKeys[_currentRcScreen]
;}void BroadcastCurrentScreen(){try{if(_breath!=_broadcastTick)return;ICommsScreen screen=_broadcasterScreens[
_broadcasterKeys[GetCurrentBroadCasterIndex()]];StringBuilder stringBuilder=new StringBuilder();screen.TextSurface.ReadText(
stringBuilder);string message=stringBuilder.ToString();IGC.SendBroadcastMessage(screen.ChannelTag,_bcID+message);}catch(Exception ex)
{_statusMessage+=ex.Message+"\n";}}void ReceiveMessages(){if(_receiverScreens.Count<1||_channels.Count<1)return;foreach(
Channel Channel in _channels.Values){while(Channel.Listener.HasPendingMessage){MyIGCMessage rawMessage=Channel.Listener.
AcceptMessage();string[]data=rawMessage.Data.ToString().Split(COMMS_SEPARATOR);string key=data[0];string name=data[1];string message=
data[2];if(Channel.SubChannels.ContainsKey(key))Channel.SubChannels[key].ReceiveMessage(message);else Channel.SubChannels.
Add(key,new SubChannel(key,name,message));}}}void UpdateCurrentReceiver(){if(_receiverScreens.Count<1)return;
LcdRecieverScreen screen=_receiverScreens[GetCurrentRecieverKey()]as LcdRecieverScreen;if(!_channels.ContainsKey(screen.ChannelTag)){
screen.Disconnect("Channel Not Found");return;}Channel channel=_channels[screen.ChannelTag];if(channel.SubChannels.Count<1){
screen.Disconnect("Channel Not Detected");return;}if(screen.SubChannelTag==""||!channel.SubChannels.ContainsKey(screen.
SubChannelTag)){List<string>keys=channel.SubChannels.Keys.ToList();screen.SetSubChannel(keys[0]);}SubChannel subChannel=channel.
SubChannels[screen.SubChannelTag];if(subChannel.IsTimedOut()){screen.Disconnect("Signal Timed Out");return;}screen.MessageToScreen(
subChannel.Message,subChannel.DisplayName);}LcdRecieverScreen GetReceiver(int receiverId){if(_receiverScreens.Count<1){
_statusMessage+="No Recievers Detected!\n";return null;}if(receiverId==-1)return _receiverScreens[_receiverScreens.Keys.Min()]as
LcdRecieverScreen;if(_receiverScreens.Keys.Contains(receiverId))return _receiverScreens[receiverId]as LcdRecieverScreen;_statusMessage+=
"Receiver "+receiverId+" Not Found!\n";return null;}LcdRecieverScreen GetRecieverByArg(string receiverId){int id;if(receiverId=="")
id=-1;else id=ParseInt(receiverId,-2);return GetReceiver(id);}void CycleReceiverChannel(string receiverId,bool previous){
LcdRecieverScreen receiverScreen=GetRecieverByArg(receiverId);if(receiverScreen==null)return;receiverScreen.CycleChannel(previous);}void
CycleReceiverSubChannel(string receiverId,bool previous){LcdRecieverScreen receiverScreen=GetRecieverByArg(receiverId);if(receiverScreen==null)
return;receiverScreen.CycleSubChannel(previous);}static int GetUnusedKey(int currentKey,Dictionary<int,ICommsScreen>commsAdded
){do{currentKey++;}while(commsAdded.ContainsKey(currentKey));return currentKey;}void AssignBroadCastID(){string displayID
="";List<IMyRadioAntenna>antennas=new List<IMyRadioAntenna>();GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(
antennas);if(antennas.Count>0){displayID=antennas[0].HudText;}if(displayID.Trim()==""){displayID=Me.CubeGrid.CustomName;}_bcID=
Me.CubeGrid.EntityId.ToString()+COMMS_SEPARATOR+displayID+COMMS_SEPARATOR;}void NewProfile(string argument){if(string.
IsNullOrEmpty(argument)){Echo("NO PROFILE NAME SPECIFIED! Check command and try again!");}string[]args=argument.Split(';');string
profileName=args[0].Trim();string profileHeader="Profile: "+profileName;string inventoryName="";string profile;if(args.Length==2){
inventoryName=args[1].Trim();List<IMyTerminalBlock>cargoBlocks=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(
inventoryName.Trim(),cargoBlocks);if(cargoBlocks.Count>1){Echo("More than one inventory of name \""+inventoryName+"\" found!");return
;}else if(cargoBlocks.Count==1&&cargoBlocks[0].HasInventory){IMyTerminalBlock block=cargoBlocks[0];IMyInventory inventory
=block.GetInventory(0);Echo("PROTOTYPE INVENTORY:\n* "+block.CustomName+"\n* Vol: "+(inventory.MaxVolume*1000).ToString()
+"L");profile=ProfileFromInventory(inventory);}else{Echo("No inventory of name \""+inventoryName+"\" found!");return;}}
else if(args.Length>2){Echo("Too Many Arguments in Command!");return;}else{profile=DEFAULT_PROFILE;}SetKey(Me,profileHeader,
LOADOUT,profile);AddProfileToList(profileName);SelectProfile(profileName);}string ProfileFromInventory(IMyInventory inventory){
string output="";float ratio=15.625f/(float)inventory.MaxVolume;int bpGlass,computer,construction,detector,display,explosives,
girder,gravGen,interiorPlate,lgTube,medical,metalGrid,motor,powerCell,radio,reactor,smTube,solar,steelPlate,superconductor,
thruster;bpGlass=computer=construction=detector=display=explosives=girder=gravGen=interiorPlate=lgTube=0;medical=metalGrid=motor
=powerCell=radio=reactor=smTube=solar=steelPlate=superconductor=thruster=0;List<MyInventoryItem>items=new List<
MyInventoryItem>();inventory.GetItems(items);if(items.Count>0){foreach(MyInventoryItem item in items){string type=item.Type.SubtypeId.
ToString();switch(type){case"BulletproofGlass":bpGlass=item.Amount.ToIntSafe();break;case"Computer":computer=item.Amount.
ToIntSafe();break;case"Construction":construction=item.Amount.ToIntSafe();break;case"Detector":detector=item.Amount.ToIntSafe();
break;case"Display":display=item.Amount.ToIntSafe();break;case"Explosives":explosives=item.Amount.ToIntSafe();break;case
"Girder":girder=item.Amount.ToIntSafe();break;case"GravityGenerator":gravGen=item.Amount.ToIntSafe();break;case"InteriorPlate":
interiorPlate=item.Amount.ToIntSafe();break;case"LargeTube":lgTube=item.Amount.ToIntSafe();break;case"Medical":medical=item.Amount.
ToIntSafe();break;case"MetalGrid":metalGrid=item.Amount.ToIntSafe();break;case"Motor":motor=item.Amount.ToIntSafe();break;case
"PowerCell":powerCell=item.Amount.ToIntSafe();break;case"RadioCommunication":radio=item.Amount.ToIntSafe();break;case"Reactor":
reactor=item.Amount.ToIntSafe();break;case"SmallTube":smTube=item.Amount.ToIntSafe();break;case"SolarCell":solar=item.Amount.
ToIntSafe();break;case"SteelPlate":steelPlate=item.Amount.ToIntSafe();break;case"Superconductor":superconductor=item.Amount.
ToIntSafe();break;case"Thrust":thruster=item.Amount.ToIntSafe();break;}}}output="BulletproofGlass:"+(int)(bpGlass*ratio)+"\n"+
"Computer:"+(int)(computer*ratio)+"\n"+"Construction:"+(int)(construction*ratio)+"\n"+"Detector:"+(int)(detector*ratio)+"\n"+
"Display:"+(int)(display*ratio)+"\n"+"Explosives:"+(int)(explosives*ratio)+"\n"+"Girder:"+(int)(girder*ratio)+"\n"+
"GravityGenerator:"+(int)(gravGen*ratio)+"\n"+"InteriorPlate:"+(int)(interiorPlate*ratio)+"\n"+"LargeTube:"+(int)(lgTube*ratio)+"\n"+
"Medical:"+(int)(medical*ratio)+"\n"+"MetalGrid:"+(int)(metalGrid*ratio)+"\n"+"Motor:"+(int)(motor*ratio)+"\n"+"PowerCell:"+(int)(
powerCell*ratio)+"\n"+"RadioCommunication:"+(int)(radio*ratio)+"\n"+"Reactor:"+(int)(reactor*ratio)+"\n"+"SmallTube:"+(int)(
smTube*ratio)+"\n"+"SolarCell:"+(int)(solar*ratio)+"\n"+"SteelPlate:"+(int)(steelPlate*ratio)+"\n"+"Superconductor:"+(int)(
superconductor*ratio)+"\n"+"Thrust:"+(int)(thruster*ratio);return output;}void AddProfileToList(string profileName){string oldList=
_programIniHandler.GetKey(INI_HEAD,"Profiles",PROFILE_LIST);string[]profileList=oldList.Split(',');foreach(string profile in profileList){
if(profileName.Trim().ToUpper()==profile.Trim().ToUpper()){return;}}_programIniHandler.SetKey(INI_HEAD,"Profiles",
profileName+","+oldList);}const float CRUISE_STEP=10;const string CRUISE_HEADER="USAP Cruise Control";float _totalThrust;static
float _targetThrottle;double _thrustWeightRatio;double _ki=0;double _kd=0;void SetThrustWeightRatio(){_totalThrust=0;
_thrustWeightRatio=0;if(_cruiseThrusters.Count<1||_cockpit==null)return;foreach(IMyThrust thruster in _cruiseThrusters)_totalThrust+=
thruster.MaxThrust;_thrustWeightRatio=_totalThrust/(_cockpit.CalculateShipMass().TotalMass*9.81);}void SetGain(){if(
_thrustWeightRatio<=0)_Kp=_cruiseFactor/INVERSE_GAIN;else _Kp=_cruiseFactor*(2.33/INVERSE_GAIN)/_thrustWeightRatio;Echo("P-Gain:\n"+_Kp+
"\n");}void ThrottleUp(string arg){float value;if(arg=="")value=CRUISE_STEP;else value=ParseFloat(arg,-1);if(value>0){
_targetThrottle+=value;if(_targetThrottle>_maxSpeed)_targetThrottle=_maxSpeed;if(!_cruiseThrustersOn)CruiseThrustersOn();}else
_statusMessage+="INVALID THROTTLE ARGUMENT:\n\""+arg+"\"\n";}void ThrottleDown(string arg){float value;if(arg=="")value=CRUISE_STEP;
else value=ParseFloat(arg,-1);if(value>0){_targetThrottle-=value;if(_targetThrottle<=0){_targetThrottle=0;CruiseThrustersOff
();}}else _statusMessage+="INVALID THROTTLE ARGUMENT:\n\""+arg+"\"\n";}void CruiseThrustersOn(){if(_cruiseThrusters.Count
<1)return;SetThrustWeightRatio();SetGain();_cruiseThrustersOn=true;_pid=new PID(_Kp,_ki,_kd,TIME_STEP);}void
CruiseThrustersOff(){ThrottleThrusters(0);_cruiseThrustersOn=false;_runningNumber=0;UpdateThrustDisplay(0);}void ToggleCruiseThrusters(){
if(!_cruiseThrustersOn){if(_targetThrottle<=0)_targetThrottle=_maxSpeed;CruiseThrustersOn();}else CruiseThrustersOff();}
double GetForwardVelocity(){return Vector3D.Dot(_cockpit.WorldMatrix.Forward,_cockpit.GetShipVelocities().LinearVelocity);}
void ThrottleThrusters(float input){if(_cruiseThrusters.Count<1)return;foreach(IMyThrust thruster in _cruiseThrusters){
thruster.ThrustOverridePercentage=input;}UpdateThrustDisplay(_cruiseThrusters[0].ThrustOverridePercentage);}void CheckGravity(){
if(_gravityDisengage&&_cockpit.GetNaturalGravity().Length()<0.04){CruiseThrustersOff();_statusMessage+=
"GRAVITY WELL VACATED\nThrusters Disengaged\n";}}void SafetyCheck(){double altitude;if(_cockpit.TryGetPlanetElevation(MyPlanetElevation.Surface,out altitude)){if(
altitude<_safetyElevation){double speed=_cockpit.GetShipVelocities().LinearVelocity.Length();if(speed>0){Vector3D gravity=
_cockpit.GetNaturalGravity();double cos=Vector3D.Dot(_cockpit.WorldMatrix.Forward,gravity)/gravity.Length();if(cos>0.707){
CruiseThrustersOff();_statusMessage+="SAFETY THRUSTER DISENGAGE!\n";}}}}}void AssignThrusters(){_cruiseThrusters=new List<IMyThrust>();
_cruiseTag=_programIniHandler.GetKey(CRUISE_HEADER,"Cruise Thrusters","");if(_cruiseTag=="")return;IMyBlockGroup cruiseGroup=
GridTerminalSystem.GetBlockGroupWithName(_cruiseTag);if(cruiseGroup==null){_statusMessage+="NO GROUP WITH NAME \""+_cruiseTag+
"\" FOUND!\n";return;}cruiseGroup.GetBlocksOfType<IMyThrust>(_cruiseThrusters);_statusMessage+="CRUISE THRUSTERS: "+_cruiseTag+
"\nThruster Count: "+_cruiseThrusters.Count+"\n";float[]gains=GainsFromString(_programIniHandler.GetKey(CRUISE_HEADER,"Cruise Gains","1,0,0"
));_cruiseFactor=gains[0];_ki=gains[1];_kd=gains[2];}void UpdateThrustDisplay(float power){if(!_cruiseThrustersOn){
_currentPower="OFF";return;}int value=(int)(power*100);_currentPower=value+"%";}float[]GainsFromString(string gainArray){float[]
output={1,0,0};string[]values=gainArray.Split(',');if(values.Length>2){for(int i=0;i<3;i++){output[i]=ParseFloat(values[i],0);
}}if(output[0]<=0)output[0]=1;return output;}const string DISPLAY_HEAD="USAP Display Screens";class Display{
IMyTextSurfaceProvider SurfaceProvider;IMyTextSurface Surface;bool ShowHeader;bool ShowStatus;bool ShowLoadCount;bool ShowTargetSpeed;bool
ShowCruiseThrust;bool ShowActiveProfile;bool ShowLandingGear;bool ShowParked;bool ShowTurrets;bool ShowActionRelays;public Display(
IMyTextSurfaceProvider surfaceProvider,int surfaceIndex,string iniHeader){SurfaceProvider=surfaceProvider;Surface=SurfaceProvider.GetSurface(
surfaceIndex);Surface.ContentType=ContentType.TEXT_AND_IMAGE;IMyTerminalBlock block=SurfaceProvider as IMyTerminalBlock;string
isProgram;if(block==_Me)isProgram="True";else isProgram="False";ShowHeader=ParseBool(GetKey(block,iniHeader,"Show Header",
isProgram));ShowStatus=ParseBool(GetKey(block,iniHeader,"Show Status",isProgram));if(_miningCargos.Count>0){ShowLoadCount=
ParseBool(GetKey(block,iniHeader,"Show Load Count","True"));}if(_cruiseThrusters.Count>0){ShowTargetSpeed=ParseBool(GetKey(block,
iniHeader,"Show Target Speed","True"));ShowCruiseThrust=ParseBool(GetKey(block,iniHeader,"Show Throttle Level","True"));}if(
_constructionCargos.Count>0){ShowActiveProfile=ParseBool(GetKey(block,iniHeader,"Show Active Profile","True"));}if(_landingGear!=null){
ShowLandingGear=ParseBool(GetKey(block,iniHeader,"Show Landing Gear","True"));if(_landingGear.Connectors.Count>0||_landingGear.
LandingPlates.Count>0)ShowParked=ParseBool(GetKey(block,iniHeader,"Show Parking Status","True"));}if(_turrets.Count>0){ShowTurrets=
ParseBool(GetKey(block,iniHeader,"Show Turrets","True"));}if(_transponders.Count>0){ShowActionRelays=ParseBool(GetKey(block,
iniHeader,"Show Action Relays","True"));}}public void Print(){string output="";int p=0;if(ShowHeader){output+=HEADER+"\n"+SLASHES
+SLASHES+"////////  ";if(_autoCycle)output+=_breather[_breath];output+="\n";p++;}if(ShowStatus){output+=_statusMessage+
"\n";p++;}if(ShowLoadCount){output+="Load Count: "+_loadCount.ToString()+"\n";p++;}if(ShowTargetSpeed){output+=
"Target Speed: "+_targetThrottle.ToString("0.0")+" m/s\n";p++;}if(ShowCruiseThrust){output+="Auto-Throttle: "+_currentPower+"\n";p++;}if
(ShowActiveProfile){output+="Active Profile: "+_activeProfile+"\n";p++;}if(ShowLandingGear&&_landingGear!=null){output+=
"Gear: "+_landingGear.Status+"\n";p++;}if(ShowParked&&_landingGear!=null){if(_landingGear.IsParked())output+="Parking: Locked\n"
;else output+="Parking: Unlocked\n";p++;}if(ShowTurrets){output+=_turretString.Trim()+"\n";p++;}if(ShowActionRelays){
output+=_relayString.Trim()+"\n";p++;}if(p>0)Surface.WriteText(output.Trim());}}void DisplaysFromBlock(IMyTerminalBlock block)
{int surfaceCount=(block as IMyTextSurfaceProvider).SurfaceCount;if(surfaceCount<1){_statusMessage+=
"BLOCK HAS NO TEXT SURFACES:\n"+block.CustomName+"\n";return;}else if(surfaceCount==1){Display screen=new Display(block as IMyTextSurfaceProvider,0,
"USAP Screen 0");_displays.Add(screen);}else{string defaultBool="true";for(int i=0;i<surfaceCount;i++){if(ParseBool(GetKey(block,
DISPLAY_HEAD,"Show on screen "+i,defaultBool))){Display display=new Display(block as IMyTextSurfaceProvider,i,"USAP Screen "+i);
_displays.Add(display);}defaultBool="false";}}}void AssignDisplays(){List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();
GridTerminalSystem.SearchBlocksOfName(DISPLAY_TAG,blocks);if(blocks.Count<1)return;foreach(IMyTerminalBlock block in blocks)if(GetKey(
block,SHARED,"Grid_ID",_gridID)==_gridID)DisplaysFromBlock(block);}static void PrintDisplays(){if(_displays.Count<1)return;
UpdateTurretString();UpdateRelayString();foreach(Display display in _displays)display.Print();}static List<DisplayRelay>_transponders;
static string _relayString;class DisplayRelay{public IMyTransponder Transponder{get;set;}public MyIniHandler IniHandler{get;
set;}public String DisplayName{get;set;}public DisplayRelay(IMyTransponder transponder,MyIniHandler iniHandler,string
displayName){Transponder=transponder;IniHandler=iniHandler;DisplayName=displayName;}}void AssignDisplayRelays(){_transponders=new
List<DisplayRelay>();List<IMyTransponder>transponderBlocks=new List<IMyTransponder>();GridTerminalSystem.GetBlocksOfType<
IMyTransponder>(transponderBlocks);if(transponderBlocks.Count>0){foreach(IMyTransponder transponderBlock in transponderBlocks){
AssignDisplayRelay(transponderBlock);}}if(_transponders.Count>0){_transponders.Sort((x,y)=>x.DisplayName.CompareTo(y.DisplayName));}}void
AssignDisplayRelay(IMyTransponder transponderBlock){MyIniHandler iniHandler=new MyIniHandler(transponderBlock);if(!iniHandler.
HasSameGridId())return;string name=iniHandler.GetKey(INI_HEAD,DISPLAY_KEY,transponderBlock.CustomName);switch(name.Trim().ToUpper()){
case null:case"":case"FALSE":case"OFF":return;default:_transponders.Add(new DisplayRelay(transponderBlock,iniHandler,name));
break;}}static void UpdateRelayString(){if(_transponders.Count<1)return;_relayString="";foreach(DisplayRelay relay in
_transponders){_relayString+=relay.DisplayName+": Channel "+relay.Transponder.Channel+"\n";}}const string DISPLAY_KEY="Display as";
static string _turretString;static List<DisplayTurret>_turrets;class DisplayTurret{public string DisplayName{get;set;}
MyIniHandler IniHandler{get;set;}public string BlockName{get;set;}public IMyLargeTurretBase Turret{get;set;}public
IMyTurretControlBlock TurretController{get;set;}public DisplayTurret(string displayName,MyIniHandler iniHandler,IMyLargeTurretBase turret){
DisplayName=displayName;IniHandler=iniHandler;Turret=turret;TurretController=null;BlockName=Turret.CustomName;}public DisplayTurret
(string displayName,MyIniHandler iniHandler,IMyTurretControlBlock turretController){DisplayName=displayName;IniHandler=
iniHandler;Turret=null;TurretController=turretController;BlockName=TurretController.CustomName;}public string GetStatus(){string
status="";bool IsWorking=(TurretController!=null&&TurretController.IsWorking)||(Turret!=null&&Turret.IsWorking);bool
IsTargeting=(TurretController!=null&&TurretController.HasTarget)||(Turret!=null&&Turret.HasTarget);if(IsWorking&&IsTargeting)status
="ACTIVE";else if(IsWorking)status="Idle";else status="Disabled";return status;}}void AssignDisplayTurrets(){_turrets=new
List<DisplayTurret>();List<IMyLargeTurretBase>turrets=new List<IMyLargeTurretBase>();GridTerminalSystem.GetBlocksOfType<
IMyLargeTurretBase>(turrets);if(turrets.Count>0){foreach(IMyLargeTurretBase turret in turrets){AssignDisplayTurret(turret);}}List<
IMyTurretControlBlock>controllers=new List<IMyTurretControlBlock>();GridTerminalSystem.GetBlocksOfType<IMyTurretControlBlock>(controllers);if
(controllers.Count>0){foreach(IMyTurretControlBlock controller in controllers){AssignDisplayTurret(controller,true);}}if(
_turrets.Count>0)_turrets.Sort((x,y)=>x.DisplayName.CompareTo(y.DisplayName));}void AssignDisplayTurret(IMyTerminalBlock block,
bool isController=false){MyIniHandler iniHandler=new MyIniHandler(block);if(!iniHandler.HasSameGridId())return;string name=
iniHandler.GetKey(INI_HEAD,DISPLAY_KEY,block.CustomName);switch(name.Trim().ToUpper()){case null:case"":case"FALSE":case"OFF":
return;default:if(isController)_turrets.Add(new DisplayTurret(name,iniHandler,block as IMyTurretControlBlock));else _turrets.
Add(new DisplayTurret(name,iniHandler,block as IMyLargeTurretBase));break;}}static void UpdateTurretString(){if(_turrets.
Count<1)return;_turretString="";foreach(DisplayTurret turret in _turrets){_turretString+=turret.DisplayName+": "+turret.
GetStatus()+"\n";}}const string SHARED="Shared Data";const string GRID_KEY="Grid_ID";static string _gridID;static MyIniHandler
_programIniHandler;class MyIniHandler{public IMyTerminalBlock Block{get;private set;}private MyIni Ini{get;set;}public MyIniHandler(
IMyTerminalBlock block){Block=block;Ini=GetIni(Block);}public void SetKey(string header,string key,string arg){Ini.Set(header,key,arg);
Block.CustomData=Ini.ToString();}public string GetKey(string header,string key,string defaultVal){EnsureKey(header,key,
defaultVal);return Ini.Get(header,key).ToString();;}public void EnsureKey(string header,string key,string defaultVal){if(!Ini.
ContainsKey(header,key))SetKey(header,key,defaultVal);}public bool HasSameGridId(){return GetKey(SHARED,GRID_KEY,_gridID)==_gridID;
}}static void EnsureKey(IMyTerminalBlock block,string header,string key,string defaultVal){MyIni ini=GetIni(block);if(!
ini.ContainsKey(header,key))SetKey(block,header,key,defaultVal);}static string GetKey(IMyTerminalBlock block,string header,
string key,string defaultVal){EnsureKey(block,header,key,defaultVal);MyIni blockIni=GetIni(block);return blockIni.Get(header,
key).ToString();}static void SetKey(IMyTerminalBlock block,string header,string key,string arg){MyIni blockIni=GetIni(block
);blockIni.Set(header,key,arg);block.CustomData=blockIni.ToString();}static MyIni GetIni(IMyTerminalBlock block){MyIni
iniOuti=new MyIni();MyIniParseResult result;if(!iniOuti.TryParse(block.CustomData,out result)){block.CustomData="---\n"+block.
CustomData;if(!iniOuti.TryParse(block.CustomData,out result))throw new Exception(result.ToString());}return iniOuti;}void
SetGridID(string arg){string gridID;if(arg!="")gridID=arg;else gridID=Me.CubeGrid.EntityId.ToString();SetKey(Me,SHARED,"Grid_ID",
gridID);_gridID=gridID;List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<
IMyTerminalBlock>(blocks);foreach(IMyTerminalBlock block in blocks){if(block.CustomData.Contains(SHARED))SetKey(block,SHARED,"Grid_ID",
gridID);}Build();}bool SameGridID(IMyTerminalBlock block,bool useDefaultValue=true){if(!useDefaultValue&&!block.CustomData.
Contains(SHARED))return false;if(GetKey(block,SHARED,GRID_KEY,_gridID)==_gridID)return true;else return false;}class
LandingGearAssembly{public List<IMyPistonBase>Pistons;public List<IMyMotorStator>Stators;public List<IMyLandingGear>LandingPlates;public
List<IMyShipConnector>Connectors;public List<IMyShipMergeBlock>MergeBlocks;public List<IMyLightingBlock>Lights;public
IMyTimerBlock Timer;public bool IsExtended;public string Status;public LandingGearAssembly(IMyTimerBlock timer){Pistons=new List<
IMyPistonBase>();Stators=new List<IMyMotorStator>();LandingPlates=new List<IMyLandingGear>();Connectors=new List<IMyShipConnector>();
MergeBlocks=new List<IMyShipMergeBlock>();Lights=new List<IMyLightingBlock>();Timer=timer;IsExtended=ParseBool(GetKey(timer,
INI_HEAD,"Extended","True"));if(IsExtended)Status="Extended";else Status="Retracted";EnsureKey(timer,INI_HEAD,"Extension Delay",
timer.TriggerDelay.ToString());EnsureKey(timer,INI_HEAD,"Retraction Delay",timer.TriggerDelay.ToString());}public void Extend
(){if(IsExtended)return;Activate(true);}public void Retract(){Unlock();if(!IsExtended)return;Activate(false);}public void
Activate(bool extending){if(Timer.IsCountingDown)return;IsExtended=extending;SetKey(Timer,INI_HEAD,"Extended",extending.ToString
());if(LandingPlates.Count>0)foreach(IMyLandingGear landingPlate in LandingPlates)DisengageLandingPlate(landingPlate);if(
MergeBlocks.Count>0)foreach(IMyShipMergeBlock mergeBlock in MergeBlocks)DisengageMergeBlock(mergeBlock,extending);if(Pistons.Count>
0)foreach(IMyPistonBase piston in Pistons)EngagePiston(piston,extending);if(Stators.Count>0)foreach(IMyMotorStator stator
in Stators)EngageStator(stator,extending);if(Lights.Count>0)foreach(IMyLightingBlock light in Lights)ActivateLandingLight(
light,extending);float delay;if(extending){delay=ParseFloat(GetKey(Timer,INI_HEAD,"Extension Delay",Timer.TriggerDelay.
ToString()),Timer.TriggerDelay);Status="Extending...";}else{delay=ParseFloat(GetKey(Timer,INI_HEAD,"Retraction Delay",Timer.
TriggerDelay.ToString()),Timer.TriggerDelay);Status="Retracting...";}Timer.TriggerDelay=delay;Timer.StartCountdown();}public void
Toggle(){if(IsExtended)Retract();else Extend();}public void TimerCall(){if(Status=="Extending..."||Status=="Retracting...")
TimerLock();else Toggle();}public void TimerLock(){if(IsExtended)Status="Extended";else Status="Retracted";if(Pistons.Count>0)
foreach(IMyPistonBase piston in Pistons)DisengagePiston(piston);if(Stators.Count>0)foreach(IMyMotorStator stator in Stators)
DisengageStator(stator);if(LandingPlates.Count>0)foreach(IMyLandingGear landingPlate in LandingPlates)EngageLandingPlate(landingPlate);
if(MergeBlocks.Count>0)foreach(IMyShipMergeBlock mergeBlock in MergeBlocks)EngageMergeBlock(mergeBlock);}public void
SwapDirections(){IsExtended=!IsExtended;SetKey(Timer,INI_HEAD,"Extended",IsExtended.ToString());if(Pistons.Count>0)foreach(
IMyPistonBase piston in Pistons)SwapVelocities(piston);if(Stators.Count>0)foreach(IMyMotorStator stator in Stators)SwapVelocities(
stator);}public void ClearData(){Timer.CustomData="";if(Pistons.Count>0)foreach(IMyPistonBase piston in Pistons)piston.
CustomData="";if(Stators.Count>0)foreach(IMyMotorStator stator in Stators)stator.CustomData="";}public bool IsParked(){if(
_landingGear.Connectors.Count>0)foreach(IMyShipConnector connector in Connectors)if(connector.Status==MyShipConnectorStatus.
Connected)return true;if(_landingGear.LandingPlates.Count>0)foreach(IMyLandingGear landingPlate in LandingPlates)if(landingPlate.
IsLocked)return true;return false;}public void Lock(){if(Status!="Extended")return;if(LandingPlates.Count>0)foreach(
IMyLandingGear landingPlate in LandingPlates)landingPlate.Lock();if(Connectors.Count>0)foreach(IMyShipConnector connector in
Connectors)connector.Connect();}public void Unlock(){if(Status!="Extended")return;if(LandingPlates.Count>0)foreach(IMyLandingGear
landingPlate in LandingPlates)landingPlate.Unlock();if(Connectors.Count>0)foreach(IMyShipConnector connector in Connectors)connector
.Disconnect();}public void SwitchLock(){if(Status!="Extended")return;if(IsParked())Unlock();else Lock();}}void
AssembleLandingGear(){_landingGear=null;List<IMyTimerBlock>timers=new List<IMyTimerBlock>();GridTerminalSystem.GetBlocksOfType<
IMyTimerBlock>(timers);if(timers.Count<1)return;foreach(IMyTimerBlock timer in timers){if(timer.CustomName.Contains(GEAR_TAG)&&GetKey
(timer,SHARED,"Grid_ID",Me.CubeGrid.EntityId.ToString())==_gridID){_landingGear=new LandingGearAssembly(timer);List<
IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(GEAR_TAG,blocks);foreach(IMyTerminalBlock
block in blocks){if(GetKey(block,SHARED,"Grid_ID",Me.CubeGrid.EntityId.ToString())==_gridID){switch(block.
DefinitionDisplayNameText){case"Piston":AssignLandingPiston(block as IMyPistonBase);break;case"Connector":_landingGear.Connectors.Add(block as
IMyShipConnector);break;case"Rotor":case"Advanced Rotor":case"Hinge":case"Hinge 3x3":AssignLandingStator(block as IMyMotorStator);break;
case"Landing Gear":case"Magnetic Plate":case"Large Magnetic Plate":AssignLandingPlate(block as IMyLandingGear);break;case
"Spotlight":case"Searchlight":case"Light Panel":case"Offset Spotlight":case"Offset Light":case"Rotating Light":case
"Corner Light - Double":case"Corner Light":case"Interior Light":AssignLandingLight(block as IMyLightingBlock);break;case"Merge Block":case
"Small Merge Block":AssignMergeBlock(block as IMyShipMergeBlock);break;}}}return;}}}void AssignLandingPlate(IMyLandingGear landingPlate){
EnsureKey(landingPlate,INI_HEAD,"On Retract","AutoLock");EnsureKey(landingPlate,INI_HEAD,"On Extend","AutoLock");_landingGear.
LandingPlates.Add(landingPlate);}void AssignLandingStator(IMyMotorStator stator){string defaultBool;float velocity=stator.
TargetVelocityRPM;if((_landingGear.IsExtended&&velocity>=0)||(!_landingGear.IsExtended&&velocity<0))defaultBool="True";else defaultBool=
"False";bool extendToPositive=ParseBool(GetKey(stator,INI_HEAD,"Extend To Positive",defaultBool));float defaultExtend,
defaultRetract;if((extendToPositive&&velocity>0)||!extendToPositive&&velocity<0){defaultExtend=velocity;defaultRetract=-velocity;}else
{defaultExtend=-velocity;defaultRetract=velocity;}EnsureKey(stator,INI_HEAD,"Extend Velocity",defaultExtend.ToString(
"0.00"));EnsureKey(stator,INI_HEAD,"Retract Velocity",defaultRetract.ToString("0.00"));EnsureKey(stator,INI_HEAD,
"Off When Stationary","false");_landingGear.Stators.Add(stator);}void AssignLandingPiston(IMyPistonBase piston){string defaultBool;float
velocity=piston.Velocity;if((_landingGear.IsExtended&&velocity>=0)||(!_landingGear.IsExtended&&velocity<0))defaultBool="True";
else defaultBool="False";bool extendToPositive=ParseBool(GetKey(piston,INI_HEAD,"Extend To Positive",defaultBool));float
defaultExtend,defaultRetract;if((extendToPositive&&velocity>0)||!extendToPositive&&velocity<0){defaultExtend=velocity;defaultRetract=
-velocity;}else{defaultExtend=-velocity;defaultRetract=velocity;}EnsureKey(piston,INI_HEAD,"Extend Velocity",
defaultExtend.ToString("0.00"));EnsureKey(piston,INI_HEAD,"Retract Velocity",defaultRetract.ToString("0.00"));EnsureKey(piston,
INI_HEAD,"Off When Stationary","false");_landingGear.Pistons.Add(piston);}void AssignLandingLight(IMyLightingBlock light){string
defaultExtended,defaultRetracted;string currentColor=light.Color.R+","+light.Color.G+","+light.Color.B;if((_landingGear.IsExtended&&
light.IsWorking)||(!_landingGear.IsExtended&&!light.IsWorking)){defaultExtended=currentColor;defaultRetracted="0,0,0";}else{
defaultExtended="0,0,0";defaultRetracted=currentColor;}EnsureKey(light,INI_HEAD,"Color on Extend",defaultExtended);EnsureKey(light,
INI_HEAD,"Color on Retract",defaultRetracted);_landingGear.Lights.Add(light);}void AssignMergeBlock(IMyShipMergeBlock mergeBlock
){EnsureKey(mergeBlock,INI_HEAD,"Disable on Extend","False");EnsureKey(mergeBlock,INI_HEAD,"Disable on Retract","True");
EnsureKey(mergeBlock,INI_HEAD,"Enable When Stopped","True");_landingGear.MergeBlocks.Add(mergeBlock);}static void EngageStator(
IMyMotorStator stator,bool extending){stator.GetActionWithName("OnOff_On").Apply(stator);stator.RotorLock=false;if(_landingGear.
IsExtended)stator.TargetVelocityRPM=ParseFloat(GetKey(stator,INI_HEAD,"Extend Velocity","0"),0);else stator.TargetVelocityRPM=
ParseFloat(GetKey(stator,INI_HEAD,"Retract Velocity","0"),0);}static void DisengageStator(IMyMotorStator stator){stator.
TargetVelocityRPM=0;if(ParseBool(GetKey(stator,INI_HEAD,"Off When Stationary","false")))stator.GetActionWithName("OnOff_Off").Apply(
stator);else stator.RotorLock=true;}static void EngagePiston(IMyPistonBase piston,bool extending){piston.GetActionWithName(
"OnOff_On").Apply(piston);if(extending)piston.Velocity=ParseFloat(GetKey(piston,INI_HEAD,"Extend Velocity","0"),0);else piston.
Velocity=ParseFloat(GetKey(piston,INI_HEAD,"Retract Velocity","0"),0);}static void DisengagePiston(IMyPistonBase piston){piston.
Velocity=0;if(ParseBool(GetKey(piston,INI_HEAD,"Off When Stationary","false")))piston.GetActionWithName("OnOff_Off").Apply(
piston);}static void ActivateLandingLight(IMyLightingBlock light,bool extending){if(extending)light.Color=ParseColor(GetKey(
light,INI_HEAD,"Color on Extend","255,0,127"));else light.Color=ParseColor(GetKey(light,INI_HEAD,"Color on Retract","0,0,0"))
;}static void DisengageLandingPlate(IMyLandingGear landingGear){landingGear.GetActionWithName("OnOff_On").Apply(
landingGear);landingGear.Unlock();landingGear.AutoLock=false;}static void EngageLandingPlate(IMyLandingGear landingGear){if(
_landingGear.IsExtended){string onExtend=GetKey(landingGear,INI_HEAD,"On Extend","AutoLock").ToUpper();switch(onExtend){case
"AUTOLOCK":case"AUTO LOCK":case"AUTO-LOCK":case"AUTO_LOCK":landingGear.AutoLock=true;break;}}else if(!_landingGear.IsExtended){
string onRetract=GetKey(landingGear,INI_HEAD,"On Retract","AutoLock").ToUpper();switch(onRetract){case"AUTOLOCK":case
"AUTO LOCK":case"AUTO-LOCK":case"AUTO_LOCK":landingGear.AutoLock=true;break;case"OFF":case"TURNOFF":case"TURN OFF":landingGear.
GetActionWithName("OnOff_Off").Apply(landingGear);break;}}}static void DisengageMergeBlock(IMyShipMergeBlock mergeBlock,bool extending){
bool disableBlock;if(extending)disableBlock=ParseBool(GetKey(mergeBlock,INI_HEAD,"Disable on Extend","False"));else
disableBlock=ParseBool(GetKey(mergeBlock,INI_HEAD,"Disable on Retract","True"));string action;if(disableBlock)action="OnOff_Off";
else action="OnOff_On";mergeBlock.GetActionWithName(action).Apply(mergeBlock);}static void EngageMergeBlock(
IMyShipMergeBlock mergeBlock){if(ParseBool(GetKey(mergeBlock,INI_HEAD,"Enable When Stopped","True")))mergeBlock.GetActionWithName(
"OnOff_On").Apply(mergeBlock);}void SetRetractBehavior(string behavior){if(_landingGear==null||_landingGear.LandingPlates.Count<1)
return;foreach(IMyLandingGear landingPlate in _landingGear.LandingPlates)SetKey(landingPlate,INI_HEAD,"On Retract",behavior);}
void SetExtendBehavior(string behavior){if(_landingGear==null||_landingGear.LandingPlates.Count<1)return;foreach(
IMyLandingGear landingPlate in _landingGear.LandingPlates)SetKey(landingPlate,INI_HEAD,"On Extend",behavior);}static void
SwapVelocities(IMyTerminalBlock block){bool extendToPositive=!(ParseBool(GetKey(block,INI_HEAD,"Extend To Positive","")));SetKey(block
,INI_HEAD,"Extend To Positive",extendToPositive.ToString());string extendVelocity=GetKey(block,INI_HEAD,"Extend Velocity"
,"");string retractVelocity=GetKey(block,INI_HEAD,"Retract Velocity","");if(extendVelocity!="")SetKey(block,INI_HEAD,
"Retract Velocity",extendVelocity);if(retractVelocity!="")SetKey(block,INI_HEAD,"Extend Velocity",retractVelocity);}static string
CycleListener(string currentChannel,bool previous=false){if(_channels.Count<1)return currentChannel;List<string>keys=_channels.Keys.
ToList();if(!_channels.Keys.Contains(currentChannel))return keys[0];for(int i=0;i<_channels.Count;i++){if(keys[i]==
currentChannel){if(previous){i--;if(i<0)i=keys.Count-1;}else{i++;if(i>=keys.Count)i=0;}return keys[i];}}return currentChannel;}void
MainSwitch(string argument){if(!string.IsNullOrEmpty(argument)){Echo("CMD: "+argument);string[]args=argument.Split(' ');string arg
=args[0].ToUpper();string cmdArg="";if(args.Length>1){for(int i=1;i<args.Length;i++){cmdArg+=args[i]+" ";}cmdArg=cmdArg.
Trim();}switch(arg){case"REFRESH":Build();break;case"UNLOAD":Unstock(_miningCargos,ORE_DEST,false);Unstock(
_constructionCargos,COMP_SUPPLY,true);break;case"RELOAD":Restock(_magazines,AMMO_SUPPLY);Restock(_reactors,FUEL_SUPPLY);Restock(
_o2Generators,ICE_SUPPLY);break;case"REFUEL":Restock(_reactors,FUEL_SUPPLY);Restock(_o2Generators,ICE_SUPPLY);break;case"RESUPPLY":
Unstock(_constructionCargos,COMP_SUPPLY,true);Restock(_constructionCargos,COMP_SUPPLY);break;case"CRUISE_ON":CruiseThrustersOn(
);break;case"CRUISE_OFF":CruiseThrustersOff();break;case"TOGGLE_CRUISE":ToggleCruiseThrusters();break;case
"SELECT_PROFILE":SelectProfile(cmdArg);break;case"UPDATE_PROFILES":UpdateProfiles();break;case"NEW_PROFILE":NewProfile(cmdArg);break;
case"SET_GRID_ID":SetGridID(cmdArg);break;case"ADD_PREFIX":AddTags(cmdArg,true);break;case"ADD_SUFFIX":AddTags(cmdArg,false)
;break;case"DELETE_PREFIX":RemoveTags(cmdArg,true);break;case"DELETE_SUFFIX":RemoveTags(cmdArg,false);break;case
"REPLACE_PREFIX":ReplaceTags(args,true);break;case"REPLACE_SUFFIX":ReplaceTags(args,false);break;case"SWAP_TO_PREFIX":SwapTags(cmdArg,
true);break;case"SWAP_TO_SUFFIX":SwapTags(cmdArg,false);break;case"SET_LOAD_COUNT":SetLoadCount(cmdArg);break;case
"RESET_LOAD_COUNT":SetLoadCount("0");break;case"TOGGLE_GEAR":if(_landingGear!=null)_landingGear.Toggle();break;case"GEAR_DOWN":if(
_landingGear!=null)_landingGear.Extend();break;case"GEAR_UP":if(_landingGear!=null)_landingGear.Retract();break;case"GEAR_TIMER":if(
_landingGear!=null)_landingGear.TimerCall();break;case"TIMER_LOCK":if(_landingGear!=null)_landingGear.TimerLock();break;case
"SWAP_GEAR_DIRECTION":case"SWAP_GEAR_DIRECTIONS":if(_landingGear!=null)_landingGear.SwapDirections();break;case"ON_RETRACT":
SetRetractBehavior(cmdArg);break;case"ON_EXTEND":SetExtendBehavior(cmdArg);break;case"CLEAR_GEAR_DATA":if(_landingGear!=null)_landingGear.
ClearData();break;case"LOCK":if(_landingGear!=null)_landingGear.Lock();break;case"UNLOCK":if(_landingGear!=null)_landingGear.
Unlock();break;case"SWITCH_LOCK":if(_landingGear!=null)_landingGear.SwitchLock();break;case"THROTTLE_UP":ThrottleUp(cmdArg);
break;case"THROTTLE_DOWN":ThrottleDown(cmdArg);break;case"MULTITIMER":CallMultiTimer(cmdArg);break;case"CALL_PHASE":CallPhase
(cmdArg);break;case"NEXT_CHANNEL":CycleReceiverChannel(cmdArg,false);break;case"LAST_CHANNEL":case"PREVIOUS_CHANNEL":
CycleReceiverChannel(cmdArg,true);break;case"NEXT_SUBCHANNEL":CycleReceiverSubChannel(cmdArg,false);break;case"LAST_SUBCHANNEL":case
"PREVIOUS_SUBCHANNEL":CycleReceiverSubChannel(cmdArg,true);break;default:TriggerCall(argument);break;}}else if(!_cruiseThrustersOn){Echo(
"NO ARGUMENT");}}const string MULTI_TAG="[MT:";const string MULTI_HEADER=DASHES+" USAP: Multi-Timer "+DASHES;const string
PHASE_HEADER="Timer Phase ";const string PHASE_KEY="Phase Count";const string CURRENT_KEY="Current Phase";const string ERROR="ERROR"
;Dictionary<string,MultiTimer>_multiTimers;static string _nextCommand;class MultiTimer{public IMyTimerBlock Block;public
string Tag;public int PhaseCount;public Dictionary<int,Phase>Phases;public bool IsInterruptible;int currentPhase;public int
CurrentPhase{get{return currentPhase;}set{currentPhase=value;SetKey(MULTI_HEADER,CURRENT_KEY,currentPhase.ToString());}}MyIni Ini;
public MultiTimer(IMyTimerBlock timer){Phases=new Dictionary<int,Phase>();Block=timer;Ini=GetIni(Block as IMyTerminalBlock);
TagFromBlockName();PhaseCount=ParseInt(GetKey(MULTI_HEADER,PHASE_KEY,"2"),2);currentPhase=ParseInt(GetKey(MULTI_HEADER,CURRENT_KEY,"0"),
0);IsInterruptible=ParseBool(GetKey(MULTI_HEADER,"Interrupt with command","False"));if(currentPhase>=PhaseCount)
currentPhase=0;}void EnsureKey(string header,string key,string defaultVal){if(!Ini.ContainsKey(header,key))SetKey(header,key,
defaultVal);}public string GetKey(string header,string key,string defaultVal){EnsureKey(header,key,defaultVal);return Ini.Get(
header,key).ToString();}public void SetKey(string header,string key,string arg){Ini.Set(header,key,arg);UpdateCustomData();}
public void UpdateCustomData(){Block.CustomData=Ini.ToString();}void IncrementPhase(){CurrentPhase++;if(CurrentPhase>=
PhaseCount)CurrentPhase=0;}public void TimerCall(){Phase phase=Phases[CurrentPhase];phase.Activate();bool goToNext=phase.StartNext
;IncrementPhase();if(goToNext){Block.TriggerDelay=Phases[CurrentPhase].Duration;Block.StartCountdown();}}void
TagFromBlockName(){string fragment=MULTI_TAG.Substring(1);string[]firstPass=Block.CustomName.Split('[');{for(int i=1;i<firstPass.Length;
i++){string[]secondPass=firstPass[i].Split(']');for(int c=0;c<secondPass.Length;c++){if(secondPass[c].Contains(fragment))
{string[]thirdPass=secondPass[c].Split(':');if(thirdPass.Length>1)Tag=thirdPass[1].Trim().ToUpper();else Tag=ERROR;}}}}}
public bool CanBeCalled(){if(Block.IsCountingDown&&!IsInterruptible){_statusMessage+="\nMulti-Timer "+Tag+
" cannot be interrupted!";return false;}return true;}}class Phase{public int Number;public int ActionCount;public float Duration;public bool
StartNext;public List<ActionBlock>Actions;public string Header;public string Name;public Phase(MultiTimer timer,int number,bool
defaultNext){Actions=new List<ActionBlock>();Number=number;Header=PHASE_HEADER+Number.ToString();string phaseHeader=DASHES+" "+
Header+" "+DASHES;Name=timer.GetKey(phaseHeader,"Name",Number.ToString());Duration=ParseFloat(timer.GetKey(phaseHeader,
"Duration",timer.Block.TriggerDelay.ToString("0.##")),timer.Block.TriggerDelay);ActionCount=ParseInt(timer.GetKey(phaseHeader,
"Action Count","1"),1);StartNext=ParseBool(timer.GetKey(phaseHeader,"Start Next Phase",defaultNext.ToString()));}public void Activate(
){if(Actions.Count<1)return;foreach(ActionBlock action in Actions)action.Activate();}}class ActionBlock{public List<
IMyTerminalBlock>Blocks;public IMyProgrammableBlock ProgramBlock;public string Action;public bool IsProgramBlock;public ActionBlock(
IMyTerminalBlock block,string action){Init(action);Blocks.Add(block);}public ActionBlock(List<IMyTerminalBlock>blocks,string action){
Init(action);Blocks=blocks;}public ActionBlock(IMyProgrammableBlock programBlock,string action){Init(action,programBlock);}
public void Activate(){if(ProgramBlock!=null){if(ProgramBlock==_Me)RunNext(Action);else ProgramBlock.TryRun(Action);}else if(
Blocks.Count>0){foreach(IMyTerminalBlock block in Blocks){try{block.GetActionWithName(Action).Apply(block);}catch{
_statusMessage+=block.CustomName+" cannot perform action \""+Action+"\"!\n";}}}}void Init(string action,IMyProgrammableBlock
programBlock=null){Blocks=new List<IMyTerminalBlock>();Action=action;ProgramBlock=programBlock;IsProgramBlock=ProgramBlock!=null;}}
void AssignMultiTimers(){_multiTimers=new Dictionary<string,MultiTimer>();List<IMyTerminalBlock>timers=new List<
IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(timers);if(timers.Count<1)return;foreach(IMyTimerBlock timer in
timers){if(timer.CustomName.ToUpper().Contains(MULTI_TAG)&&SameGridID(timer)){MultiTimer multiTimer=new MultiTimer(timer);
string tag=multiTimer.Tag;if(_multiTimers.Keys.Contains<string>(tag)){_statusMessage+=
"\nERROR: Cannot add Multitimer from block:\n  "+multiTimer.Block.CustomName+"\n  => Tag already in use.";}else if(multiTimer.Tag!=ERROR){AssignPhases(multiTimer);
_multiTimers.Add(multiTimer.Tag,multiTimer);}}}}void AssignPhases(MultiTimer timer){int count=timer.PhaseCount;for(int i=0;i<count;i
++){bool defaultNext=i<count-1;Phase phase=new Phase(timer,i,defaultNext);AssignActions(timer,phase);timer.Phases.Add(i,
phase);}}void AssignActions(MultiTimer timer,Phase phase){if(phase.ActionCount<1)return;string header=phase.Header+" Actions"
;for(int i=0;i<phase.ActionCount;i++){string blockString=timer.GetKey(header,"Block "+i,"");string actionString=timer.
GetKey(header,"Action "+i,"");if(blockString!=""&&actionString!=""){ActionBlock actionBlock;if(blockString.ToUpper().
StartsWith("G:"))actionBlock=ActionBlockFromGroupName(blockString,actionString);else if(blockString.ToUpper().StartsWith("P:"))
actionBlock=ActionBlockFromProgramName(blockString,actionString);else actionBlock=ActionBlockFromBlockName(blockString,actionString
);if(actionBlock!=null)phase.Actions.Add(actionBlock);}}}ActionBlock ActionBlockFromGroupName(string groupName,string
action){IMyBlockGroup group=GridTerminalSystem.GetBlockGroupWithName(groupName.Substring(2).Trim());if(group==null)return null
;List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();group.GetBlocks(blocks);return new ActionBlock(blocks,action);
}ActionBlock ActionBlockFromProgramName(string programName,string action){string name=programName.Substring(2).Trim();
IMyTerminalBlock programBlock=GridTerminalSystem.GetBlockWithName(name);if(programBlock==null)return null;string[]data=programBlock.
GetType().ToString().Split('.');string type=data[data.Length-1].Trim();if(type=="MyProgrammableBlock"&&SameGridID(programBlock)
)return new ActionBlock(programBlock as IMyProgrammableBlock,action);return null;}ActionBlock ActionBlockFromBlockName(
string blockName,string action){IMyTerminalBlock block=GridTerminalSystem.GetBlockWithName(blockName);if(block==null){
_statusMessage+="\nBlock Not Found: "+blockName;return null;}else{Echo("\nBlock: ");Echo(block.CustomName);return new ActionBlock(
block,action);}}MultiTimer GetMultiTimer(string tag){string key=tag.Trim().ToUpper();if(_multiTimers.ContainsKey(key)){return
_multiTimers[key];}return null;}Phase PhaseFromName(MultiTimer timer,string name){foreach(int key in timer.Phases.Keys){Phase phase=
timer.Phases[key];if(phase.Name==name)return phase;}return null;}void CallMultiTimer(string tag){MultiTimer timer=
GetMultiTimer(tag);if(timer==null){_statusMessage+="No Timer with tag \""+tag+"\" found!";return;}if(timer.CanBeCalled())timer.
TimerCall();}void CallPhase(string phaseData){string[]data=phaseData.Split(' ');if(data.Length<2){_statusMessage+=
"\nINSUFFICIENT PHASE DATA!";}string phaseName=data[0];string tag="";for(int i=1;i<data.Length;i++){tag+=data[i]+" ";}MultiTimer timer=GetMultiTimer
(tag.Trim());if(timer==null){_statusMessage+="No Timer with tag \""+tag+"\" found!";return;}Phase phase=PhaseFromName(
timer,phaseName);if(phase==null){_statusMessage+="No Phase with name \""+phaseName+"\" found!";return;}if(timer.CanBeCalled()
){timer.CurrentPhase=phase.Number;timer.TimerCall();}}void MultiTimerDebug(){if(_multiTimers.Count<1){Echo(
"No MultiTimers found!");return;}string multiString="MultiTimers "+DASHES;foreach(string tag in _multiTimers.Keys){MultiTimer timer=
_multiTimers[tag];multiString+="\n * "+tag+"\n   - Phases: "+timer.PhaseCount;foreach(int key in timer.Phases.Keys){Phase phase=
timer.Phases[key];multiString+="\n    Phase "+phase.Number;foreach(ActionBlock actionBlock in phase.Actions){if(actionBlock.
Blocks.Count>0)foreach(IMyTerminalBlock block in actionBlock.Blocks)multiString+="\n      "+block.CustomName+":"+actionBlock.
Action;else if(actionBlock.ProgramBlock!=null)multiString+="\n      "+actionBlock.ProgramBlock.CustomName+":"+actionBlock.
Action;}}}Echo(multiString);}static void RunNext(string arg){_nextCommand=arg;}void RunLast(){if(_nextCommand=="")return;
MainSwitch(_nextCommand);_nextCommand="";}class PID{public double Kp{get;set;}=0;public double Ki{get;set;}=0;public double Kd{get
;set;}=0;public double Value{get;private set;}double _timeStep=0;double _inverseTimeStep=0;double _errorSum=0;double
_lastError=0;bool _firstRun=true;public PID(double kp,double ki,double kd,double timeStep){Kp=kp;Ki=ki;Kd=kd;_timeStep=timeStep;
_inverseTimeStep=1/_timeStep;}protected virtual double GetIntegral(double currentError,double errorSum,double timeStep){return errorSum+
currentError*timeStep;}public double Control(double error){double errorDerivative=(error-_lastError)*_inverseTimeStep;if(_firstRun){
errorDerivative=0;_firstRun=false;}_errorSum=GetIntegral(error,_errorSum,_timeStep);_lastError=error;Value=Kp*error+Ki*_errorSum+Kd*
errorDerivative;return Value;}public Double Control(double error,double timeStep){if(timeStep!=_timeStep){_timeStep=timeStep;
_inverseTimeStep=1/_timeStep;}return Control(error);}public virtual void Reset(){_errorSum=0;_lastError=0;_firstRun=true;}}class
DecayingIntegralPID:PID{public double IntegralDecayRatio{get;set;}public DecayingIntegralPID(double kp,double ki,double kd,double timeStep,
double decayRatio):base(kp,ki,kd,timeStep){IntegralDecayRatio=decayRatio;}protected override double GetIntegral(double
currentError,double errorSum,double timeStep){return errorSum*(1.0-IntegralDecayRatio)+currentError*timeStep;}}class
ClampedIntegralPID:PID{public double IntegralUpperBound{get;set;}public double IntegralLowerBound{get;set;}public ClampedIntegralPID(
double kp,double ki,double kd,double timeStep,double lowerBound,double upperBound):base(kp,ki,kd,timeStep){IntegralUpperBound=
upperBound;IntegralLowerBound=lowerBound;}protected override double GetIntegral(double currentError,double errorSum,double
timeStep){errorSum=errorSum+currentError*timeStep;return Math.Min(IntegralUpperBound,Math.Max(errorSum,IntegralLowerBound));}}
class BufferedIntegralPID:PID{Queue<double>_integralBuffer=new Queue<double>();public int IntegralBufferSize{get;set;}=0;
public BufferedIntegralPID(double kp,double ki,double kd,double timeStep,int bufferSize):base(kp,ki,kd,timeStep){
IntegralBufferSize=bufferSize;}protected override double GetIntegral(double currentError,double errorSum,double timeStep){if(
_integralBuffer.Count==IntegralBufferSize)_integralBuffer.Dequeue();_integralBuffer.Enqueue(currentError*timeStep);return
_integralBuffer.Sum();}public override void Reset(){base.Reset();_integralBuffer.Clear();}}void AddPrefix(IMyTerminalBlock block,string
prefix){string temp_name=block.CustomName;if(block.IsSameConstructAs(Me)&&!temp_name.StartsWith(prefix))block.CustomName=
prefix+" "+temp_name;}void AddSuffix(IMyTerminalBlock block,string suffix){string temp_name=block.CustomName;if(block.
IsSameConstructAs(Me)&&!temp_name.EndsWith(suffix))block.CustomName=temp_name+" "+suffix;}void DeletePrefix(IMyTerminalBlock block,string
prefix){if(block.CustomName.StartsWith(prefix)){string[]nameParts=block.CustomName.Split(' ');if(nameParts.Length>1){string
newName="";for(int i=1;i<nameParts.Length;i++){newName+=nameParts[i]+" ";}block.CustomName=newName.Trim();}else{block.
CustomName=block.DefinitionDisplayNameText;}}}void DeleteSuffix(IMyTerminalBlock block,string suffix){if(block.CustomName.EndsWith
(suffix)){string[]nameParts=block.CustomName.Split(' ');if(nameParts.Length>1){string newName="";for(int i=0;i<nameParts.
Length-1;i++){newName+=nameParts[i]+" ";}block.CustomName=newName.Trim();}else{block.CustomName=block.
DefinitionDisplayNameText;}}}void ReplacePrefix(IMyTerminalBlock block,string oldTag,string newTag){DeletePrefix(block,oldTag);AddPrefix(block,
newTag);}void ReplaceSuffix(IMyTerminalBlock block,string oldTag,string newTag){DeleteSuffix(block,oldTag);AddSuffix(block,
newTag);}void SwapTag(IMyTerminalBlock block,string tag,bool swapToPrefix){if(swapToPrefix){DeleteSuffix(block,tag);AddPrefix(
block,tag);}else{DeletePrefix(block,tag);AddSuffix(block,tag);}}void AddTags(string tag,bool toPrefix){List<IMyTerminalBlock>
blocks=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocks(blocks);if(toPrefix){foreach(IMyTerminalBlock block in blocks
)AddPrefix(block,tag);}else{foreach(IMyTerminalBlock block in blocks)AddSuffix(block,tag);}}void RemoveTags(string tag,
bool fromPrefix){List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocks(blocks);if(
fromPrefix){foreach(IMyTerminalBlock block in blocks)DeletePrefix(block,tag);}else{foreach(IMyTerminalBlock block in blocks)
DeleteSuffix(block,tag);}}void SwapTags(string tag,bool toPrefix){List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocks(blocks);if(toPrefix){foreach(IMyTerminalBlock block in blocks){DeleteSuffix(block,tag);AddPrefix(block,tag);}
}else{foreach(IMyTerminalBlock block in blocks){DeletePrefix(block,tag);AddSuffix(block,tag);}}}void ReplaceTags(string[]
tags,bool replacePrefix){if(tags.Length<3){_statusMessage=
"Insufficient arguments for TAG REPLACEMENT.\n * Be sure to include the old tag to be replaced as well as the new tag.";return;}string oldTag=tags[1];string newTag=tags[2];List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocks(blocks);if(replacePrefix){foreach(IMyTerminalBlock block in blocks)ReplacePrefix(block,oldTag,newTag);}else{
foreach(IMyTerminalBlock block in blocks)ReplaceSuffix(block,oldTag,newTag);}}static Dictionary<string,Channel>_channels;const
string LISTENER_KEY="Receiver Channels";static string _defaultChannels=DF_LCD_COMTAG+"0\n"+DF_LCD_COMTAG+"1\n"+DF_LCD_COMTAG+
"2\n"+DF_LCD_COMTAG+"3\n"+DF_LCD_COMTAG+"4";class SubChannel{public string BroadcastID{get;set;}public string DisplayName{get
;set;}public DateTime LastReceived{get;set;}public string Message{get;set;}public IMyBroadcastListener Listener{get;set;}
public SubChannel(string broadcastId,string displayName,string message=""){BroadcastID=broadcastId;DisplayName=displayName;
Message=message;LastReceived=DateTime.MinValue;}public bool IsTimedOut(){return DateTime.Now-LastReceived>TimeSpan.FromSeconds(
_listenerTimeOut);}public void ReceiveMessage(string message){Message=message;LastReceived=DateTime.Now;}}class Channel{public string
Name{get;set;}public IMyBroadcastListener Listener{get;set;}public Dictionary<string,SubChannel>SubChannels;public Channel(
string name){Name=name;SubChannels=new Dictionary<string,SubChannel>();}public string CycleSubChannel(string currentChannelTag
,bool previous=false){if(SubChannels.Count<1)return"";List<string>subChannelTags=SubChannels.Keys.ToList();if(
subChannelTags.Count==1)return subChannelTags[0];for(int i=0;i<subChannelTags.Count;i++){if(subChannelTags[i]==currentChannelTag){if(
previous)i--;else i++;if(i>=subChannelTags.Count)i=0;else if(i<0)i=subChannelTags.Count-1;return subChannelTags[i];}}return
currentChannelTag;}}void RegisterListener(Channel channel){channel.Listener=IGC.RegisterBroadcastListener(channel.Name);channel.Listener.
SetMessageCallback(channel.Name);}void AssignChannels(){_channels=new Dictionary<string,Channel>();string[]channels=_programIniHandler.
GetKey(COMMS_HEADER,LISTENER_KEY,_defaultChannels).Split('\n');_listenerTimeOut=ParseInt(_programIniHandler.GetKey(
COMMS_HEADER,"Listener Time Out",_listenerTimeOut.ToString()),_listenerTimeOut);foreach(string channel in channels){if(channel.Trim(
)=="")continue;Channel listener=new Channel(channel);_channels.Add(channel,listener);RegisterListener(listener);}}static
bool TriggerCall(UpdateType updateSource){if((updateSource&(UpdateType.Trigger|UpdateType.Terminal))>0||(updateSource&(
UpdateType.Mod))>0||(updateSource&(UpdateType.Script))>0){return true;}return false;}static int ParseInt(string arg,int
defaultValue){int number;if(int.TryParse(arg,out number))return number;else return defaultValue;}static float ParseFloat(string arg,
float defaultValue){float number;if(float.TryParse(arg,out number))return number;else return defaultValue;}static bool
ParseBool(string val){string uVal=val.ToUpper();if(uVal=="TRUE"||uVal=="T"||uVal=="1"){return true;}return false;}static Color
ParseColor(string colorString){UInt16 red,green,blue;red=green=blue=0;string[]values=colorString.Split(',');if(values.Length>2){
UInt16.TryParse(values[0],out red);UInt16.TryParse(values[1],out green);UInt16.TryParse(values[2],out blue);}return new Color(
red,green,blue);}static string GetBracedInfo(string arg){string info="";if(arg.Contains("{")&&arg.Contains("}")){int open=
arg.IndexOf('{');int close=arg.IndexOf('}')-1;if(open<close){info=arg.Substring(open+1,close-open).Trim();}}return info;}
static double ToRadians(int angle){double radianValue=(double)angle*Math.PI/180;return radianValue;}static float ToDegrees(
float angle){float degreeValue=angle*180/(float)Math.PI;return degreeValue;}static float ToHalfCircle(float degrees){if(
degrees>=-180&&degrees<=180)return degrees;else if(degrees>180)return degrees-360;else return degrees+360;}