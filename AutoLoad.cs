// AutoLoad v3.2
// Commands: init, pull, scan, loadout, toggle, group, reset, status, calibrate, continue, up, down, select
// LCD tags: [AutoLoad] [AutoTotals] [AutoLog] [AutoMenu] or AutoLoad:N in Custom Data
// Light tags: [AutoLight] or [AutoLight:SteelPlate] for stock indicators
// Connector: [AutoLoad] Ignore=true in Custom Data stops it counting as a dock
// Debug: AutoDebug in a panel's Custom Data reports per-tick cost
// Works on ships, drones and rovers. Modded items and ammo supported.
// See PB Custom Data for full help.
// 
private const string DISPLAY_TAG="AutoLoad:",LOG_TAG="AutoLog:",LIGHT_TAG="AutoLight",DEBUG_TAG="AutoDebug:",
LOADOUT_PREFIX="Loadout:",INI_SECTION="AutoLoad",DOCK_SECTION="DockActions",MODDED_SECTION="Modded",MODDED_COMMENT=
" Modded items found at the station.\n"+" Discover=false stops new ones being added.\n"+" Key=TypeId/SubtypeId/LitresPerUnit",IGNORE_HINT_VISIBLE=
"; Uncomment to hide this block from AutoLoad:\n;Ignore=true",IGNORE_HINT_HIDDEN=
"; Hidden from AutoLoad: no conveyor path to a connector.\n; Uncomment to force-show this block:\n;Ignore=false",AMMO_TYPE_ID="MyObjectBuilder_AmmoMagazine";private const float MIN_TRANSFER_LITRES=0.01f;private readonly Dictionary<
string,MyItemType>ALL_ITEMS=new Dictionary<string,MyItemType>(StringComparer.OrdinalIgnoreCase);private readonly Dictionary<
string,List<string>>CATEGORIES=new Dictionary<string,List<string>>();private readonly Dictionary<MyItemType,float>VOL_PER_UNIT
=new Dictionary<MyItemType,float>(),_shipHeldUnits=new Dictionary<MyItemType,float>(),_shipTargetL=new Dictionary<
MyItemType,float>(),_blockHeld=new Dictionary<MyItemType,float>();private readonly Dictionary<MyItemType,string>KEY_OF=new
Dictionary<MyItemType,string>();private readonly MyIni _ini=new MyIni();MyIniParseResult _iniResult;private readonly StringBuilder
_sb=new StringBuilder(),_dbgSb=new StringBuilder(),_dbgOut=new StringBuilder(),_blockSb=new StringBuilder();private
readonly List<IMyCargoContainer>_containers=new List<IMyCargoContainer>(),_allContainers=new List<IMyCargoContainer>();private
readonly HashSet<long>_hiddenContainers=new HashSet<long>(),_knownContainerIds=new HashSet<long>(),_mechDockGrids=new HashSet<
long>(),_ignoredConnectors=new HashSet<long>(),_indicatorLights=new HashSet<long>(),_lightOverall=new HashSet<long>(),
_configIgnored=new HashSet<long>();int _knownTurretCount=-1,_scanCountdown=0,_sinceScan=0,_dbgLastInstr=0,_dbgSrc=0,_dbgTick=0,
_dbgWorstInstr=0,_dbgWorstTick=0,_calTicks=0,_calReadings=0,_calCompleteTicks=0;private const int HIDDEN_SCAN_INTERVAL=100,
MIN_RESCAN_GAP=10,RESTAMP_PER_TICK=2;private readonly List<IMyUserControllableGun>_turrets=new List<IMyUserControllableGun>();private
readonly List<IMyShipConnector>_connectors=new List<IMyShipConnector>();private readonly List<IMyBatteryBlock>_batteries=new
List<IMyBatteryBlock>();private readonly List<IMyGasTank>_gasTanks=new List<IMyGasTank>();private readonly List<IMyThrust>
_thrusters=new List<IMyThrust>();private readonly List<IMyMotorSuspension>_suspensions=new List<IMyMotorSuspension>();private
readonly List<IMyMechanicalConnectionBlock>_mechBlocks=new List<IMyMechanicalConnectionBlock>();private readonly List<IMyGyro>
_gyros=new List<IMyGyro>();private readonly List<IMyLightingBlock>_lights=new List<IMyLightingBlock>();private readonly List<
IMyRadioAntenna>_antennas=new List<IMyRadioAntenna>();private readonly List<IMyShipController>_controllers=new List<IMyShipController>(
);private readonly List<IMyTextSurface>_surfaces=new List<IMyTextSurface>(),_logSurfaces=new List<IMyTextSurface>(),
_debugSurfaces=new List<IMyTextSurface>();string _dbgPhase="",_dbgWorstText="",_activeLoadout="",_cachedLoadoutConfig="";double
_dbgWorstMs=0;private readonly List<IMyTextSurface>_menuSurfaces=new List<IMyTextSurface>(),_totalSurfaces=new List<IMyTextSurface>
();private readonly List<float>_menuScales=new List<float>(),_totalScales=new List<float>();float _shipCurL=0f,_shipMaxL=
0f,_calThrustSum=0f,_calBaseMass=0f,_calGravity=0f;private readonly List<IMyLightingBlock>_watchLights=new List<
IMyLightingBlock>();private readonly List<MyItemType>_watchTypes=new List<MyItemType>(),_tempTypes=new List<MyItemType>();private
readonly List<bool>_watchOverall=new List<bool>();private readonly List<string>_badLightItems=new List<string>(),_log=new List<
string>();private readonly Dictionary<long,MyItemType>_lightItem=new Dictionary<long,MyItemType>();bool _cargoPhase=false;bool
_forceDisplays=false,_discoverEnabled=true,_discoveredDirty=false;string _lastLogEcho="";int _menuCursor=0,_calState=0;float[]
_calResults=new float[5];private static readonly string[]CAL_DIR_NAMES={"FORWARD","LEFT","RIGHT","UP","DOWN"};private readonly List
<MyInventoryItem>_tempItems=new List<MyInventoryItem>();private readonly List<IMyTerminalBlock>_tempBlocks=new List<
IMyTerminalBlock>(),_weapons=new List<IMyTerminalBlock>(),_weaponProbeQueue=new List<IMyTerminalBlock>(),_restampQueue=new List<
IMyTerminalBlock>();private readonly List<MyIniKey>_tempIniKeys=new List<MyIniKey>();private readonly List<IMyConveyorSorter>_sorters=
new List<IMyConveyorSorter>();private readonly Dictionary<string,List<MyItemType>>_ammoByDef=new Dictionary<string,List<
MyItemType>>();private readonly Dictionary<string,bool>_sorterIsWeapon=new Dictionary<string,bool>();private readonly Dictionary<
MyItemType,List<IMyInventory>>_ammoSources=new Dictionary<MyItemType,List<IMyInventory>>();private readonly HashSet<string>
_weaponProbeQueued=new HashSet<string>();private readonly HashSet<MyItemType>_discoveredTypes=new HashSet<MyItemType>();bool
InitBudgetSpent(){return Runtime.CurrentInstructionCount>Runtime.MaxInstructionCount/2;}private readonly List<MyItemType>
_volumeProbeQueue=new List<MyItemType>();int _volumesMeasured=0,_tickCounter=0;int _logHoldCycles=0;private const int LOG_HOLD_CYCLES=12;
bool _wasDocked=false;bool _loadoutJustApplied=false;public
 Program
(){BuildItemDatabase();LoadDiscovered();string[]storageParts=(Storage??"").Split('|');_activeLoadout=storageParts.Length>
0?storageParts[0]:"";if(storageParts.Length>1){int cs;if(int.TryParse(storageParts[1],out cs))_calState=cs;}if(
storageParts.Length>2)_cachedLoadoutConfig=storageParts[2];Runtime.UpdateFrequency=UpdateFrequency.Update10;AutoInit();float calBm,
calUt,calGrav;if(!ReadCalibrationData(out calBm,out calUt,out calGrav)&&!IsGroundVehicle())_calState=1;Echo(IsGroundVehicle()
?"AutoLoad v3.2 ready (rover).":"AutoLoad v3.2 ready.");}public void
 Save
(){Storage=_activeLoadout+"|"+_calState+"|"+_cachedLoadoutConfig;}public void
 Main
(string argument,UpdateType updateSource){_log.Clear();string arg=(argument??"").Trim();if(arg.Length>0)_scanCountdown=0;
RefreshCaches();DebugBegin();if(arg.Length>0){var rawParts=arg.Split(' ');var partsList=new List<string>();for(int i=0;i<rawParts.
Length;i++)if(rawParts[i].Length>0)partsList.Add(rawParts[i]);DebugPhase("cmd:"+(partsList.Count>0?partsList[0]:"?"));
ProcessCommand(arg,partsList.ToArray());DebugPhase("cmdDisp");WriteDisplays(true,_forceDisplays);_forceDisplays=false;DebugEnd();}if((
updateSource&UpdateType.Update10)!=0){bool docked=IsDocked();if(!docked&&_wasDocked)ReverseDockActions();if(docked&&!_wasDocked){Log
("Docked");ApplyGroupConfigs();ApplyBlockTags();ApplyDockActions();int found=DiscoverFromSources(CollectSourceInventories
());if(found>0)Log($"Discovered {found} modded item(s)");}_wasDocked=docked;DebugPhase("probes");ProcessRestampQueue();
ProcessWeaponProbes();ProcessVolumeProbes();if(_calState>=2&&_calState<=6)UpdateCalibration();_tickCounter++;if(_tickCounter>=10){
_tickCounter=0;bool withCargo=true;if(docked){_cargoPhase=!_cargoPhase;withCargo=_cargoPhase;}if(docked&&withCargo){Echo(
_lastLogEcho);}else{Log(docked?"Docked":"Undocked");if(docked){DebugPhase("tags");ApplyBlockTags();DebugPhase("loadout");
SyncLoadoutConfig();BalanceInventory();}}DebugPhase(withCargo?"disp+cargo":"disp");WriteDisplays(false,withCargo);DebugEnd();}return;}}
void ProcessCommand(string arg,string[]parts){string cmd=parts[0].ToLowerInvariant();switch(cmd){case"init":CmdInit();break;
case"pull":CmdPull();break;case"toggle":if(parts.Length<3)Log("Usage: toggle <BlockName> <ItemKey>");else{string itemKey=
parts[parts.Length-1];string blockName=string.Join(" ",parts,1,parts.Length-2);CmdToggle(blockName,itemKey);}break;case
"group":{int sep=arg.IndexOf('>');if(sep<0||parts.Length<3)Log("Usage: group <Grp> > <Item/Wt,...>");else{string groupName=arg.
Substring(parts[0].Length,sep-parts[0].Length).Trim();string gItemSpec=arg.Substring(sep+1).Trim();CmdGroup(groupName,gItemSpec);
}}break;case"loadout":CmdLoadout(parts);break;case"scan":CmdScan();break;case"reset":CmdReset();break;case"status":
_forceDisplays=true;break;case"up":_menuCursor=Math.Max(0,_menuCursor-1);break;case"down":_menuCursor++;break;case"select":{var names=
ListLoadoutNames();names.Insert(0,"Off");names.Add("Configure");if(_menuCursor<names.Count){string picked=names[_menuCursor];if(picked.
Equals("Off",StringComparison.OrdinalIgnoreCase))CmdLoadout(new string[]{"loadout","off"});else if(picked.Equals("Configure",
StringComparison.OrdinalIgnoreCase)){if(IsGroundVehicle()){Log("Rover detected - calibration not needed.");break;}_calState=1;_calTicks=
0;_calThrustSum=0f;_calReadings=0;for(int ci=0;ci<5;ci++)_calResults[ci]=0f;_calBaseMass=0f;}else{Log(
$"=== LOADOUT: {picked} ===");ApplyLoadoutGlobal(picked);}}}break;case"continue":if(_calState==1){_calState=2;_calTicks=0;_calThrustSum=0f;
_calReadings=0;}else if(_calState>=2&&_calState<=6){int skipIdx=_calState-2;_calResults[skipIdx]=_calReadings>0?_calThrustSum/
_calReadings:0f;_calThrustSum=0f;_calReadings=0;_calState++;if(_calState==7&&_controllers.Count>0){var ctrl=_controllers[0];var sm=
ctrl.CalculateShipMass();_calBaseMass=sm.BaseMass;Vector3D gv=ctrl.GetNaturalGravity();_calGravity=(float)gv.Length();
SaveCalibrationData();}}break;case"calibrate":if(IsGroundVehicle()){Log("Rover detected - calibration not needed.");Log(
"SmartLoad is off for ground vehicles.");break;}_calState=1;_calTicks=0;_calThrustSum=0f;_calReadings=0;for(int ci=0;ci<5;ci++)_calResults[ci]=0f;_calBaseMass=
0f;break;default:Log($"Unknown: '{cmd}'");break;}}bool IsMyGrid(IMyTerminalBlock b){return b.CubeGrid.EntityId==Me.
CubeGrid.EntityId;}bool IsMyConstruct(IMyTerminalBlock b){return b.IsSameConstructAs(Me);}bool IsOwnConnector(IMyShipConnector c
){if(_ignoredConnectors.Contains(c.EntityId))return false;return!_mechDockGrids.Contains(c.CubeGrid.EntityId);}public
static bool IsIgnoredData(MyIni ini,string customData){if(string.IsNullOrEmpty(customData))return false;ini.Clear();
MyIniParseResult parseResult;if(!ini.TryParse(customData,out parseResult))return false;return ini.Get(INI_SECTION,"Ignore").ToBoolean(
false);}void RefreshIgnoredConnectors(){_ignoredConnectors.Clear();foreach(var c in _connectors)if(IsIgnoredData(_ini,c.
CustomData))_ignoredConnectors.Add(c.EntityId);}void RefreshCaches(){_containers.Clear();_allContainers.Clear();_turrets.Clear();
_connectors.Clear();_surfaces.Clear();_logSurfaces.Clear();_debugSurfaces.Clear();_menuSurfaces.Clear();_totalSurfaces.Clear();
_menuScales.Clear();_totalScales.Clear();_batteries.Clear();_gasTanks.Clear();_thrusters.Clear();_suspensions.Clear();_gyros.Clear(
);_lights.Clear();_antennas.Clear();_controllers.Clear();GridTerminalSystem.GetBlocksOfType(_connectors,IsMyConstruct);
GridTerminalSystem.GetBlocksOfType(_allContainers,IsMyGrid);GridTerminalSystem.GetBlocksOfType(_turrets,IsMyGrid);GridTerminalSystem.
GetBlocksOfType(_batteries,IsMyGrid);GridTerminalSystem.GetBlocksOfType(_gasTanks,IsMyGrid);GridTerminalSystem.GetBlocksOfType(
_thrusters,IsMyGrid);GridTerminalSystem.GetBlocksOfType(_suspensions,IsMyGrid);GridTerminalSystem.GetBlocksOfType(_gyros,IsMyGrid)
;GridTerminalSystem.GetBlocksOfType(_lights,IsMyGrid);GridTerminalSystem.GetBlocksOfType(_antennas,IsMyGrid);
GridTerminalSystem.GetBlocksOfType(_controllers,IsMyGrid);RefreshMechDock();RefreshHiddenContainers();foreach(var c in _allContainers)if(!
_hiddenContainers.Contains(c.EntityId))_containers.Add(c);_tempBlocks.Clear();GridTerminalSystem.GetBlocksOfType(_tempBlocks,b=>IsMyGrid(
b)&&b is IMyTextSurfaceProvider);foreach(var block in _tempBlocks){if(block.EntityId==Me.EntityId)continue;var provider=(
IMyTextSurfaceProvider)block;if(provider.SurfaceCount==0)continue;string bName=block.CustomName;string bData=block.CustomData;
CollectScaledSurface(bName,bData,"AutoTotals",provider,_totalSurfaces,_totalScales);CollectNameTagSurface(bName,"[AutoLoad",provider,
_surfaces);CollectNameTagSurface(bName,"[AutoLog",provider,_logSurfaces);CollectScaledSurface(bName,bData,"AutoMenu",provider,
_menuSurfaces,_menuScales);CollectDataTagSurface(bData,DISPLAY_TAG,provider,_surfaces);CollectDataTagSurface(bData,LOG_TAG,provider,
_logSurfaces);CollectDataTagSurface(bData,DEBUG_TAG,provider,_debugSurfaces);}}void RefreshHiddenContainers(){bool setChanged=
_allContainers.Count!=_knownContainerIds.Count||_turrets.Count!=_knownTurretCount;if(!setChanged){foreach(var c in _allContainers){if(
!_knownContainerIds.Contains(c.EntityId)){setChanged=true;break;}}}_scanCountdown--;_sinceScan++;bool due=_scanCountdown
<=0||(setChanged&&_sinceScan>=MIN_RESCAN_GAP);if(!due)return;_scanCountdown=HIDDEN_SCAN_INTERVAL;_sinceScan=0;
_knownContainerIds.Clear();foreach(var c in _allContainers)_knownContainerIds.Add(c.EntityId);_knownTurretCount=_turrets.Count;
RefreshWeapons();RefreshIgnoredConnectors();RefreshLightTags();_hiddenContainers.Clear();_configIgnored.Clear();foreach(var c in
_allContainers){_ini.Clear();if(_ini.TryParse(c.CustomData,out _iniResult)&&_ini.ContainsKey(INI_SECTION,"Ignore")){if(_ini.Get(
INI_SECTION,"Ignore").ToBoolean(false)){_hiddenContainers.Add(c.EntityId);_configIgnored.Add(c.EntityId);}continue;}if(!
HasConveyorAccess(c))_hiddenContainers.Add(c.EntityId);}}bool HasConveyorAccess(IMyCargoContainer container){var inv=container.
GetInventory(0);if(inv==null)return true;bool anyConnector=false;bool connectorReached=false;foreach(var conn in _connectors){if(!
conn.HasInventory)continue;if(!IsOwnConnector(conn))continue;anyConnector=true;if(inv.IsConnectedTo(conn.GetInventory(0))){
connectorReached=true;break;}}bool anyOther=false;bool otherReached=false;if(!anyConnector){foreach(var t in _turrets){if(!t.
HasInventory)continue;anyOther=true;if(inv.IsConnectedTo(t.GetInventory(0))){otherReached=true;break;}}if(!otherReached){foreach(var
other in _allContainers){if(other.EntityId==container.EntityId)continue;if(!other.HasInventory)continue;anyOther=true;if(inv.
IsConnectedTo(other.GetInventory(0))){otherReached=true;break;}}}}return IsReachable(anyConnector,connectorReached,anyOther,
otherReached);}public static bool IsReachable(bool anyConnector,bool connectorReached,bool anyOther,bool otherReached){if(
anyConnector)return connectorReached;if(!anyOther)return true;return otherReached;}void RefreshWeapons(){_weapons.Clear();foreach(
var t in _turrets)if(t.HasInventory)_weapons.Add(t);_sorters.Clear();GridTerminalSystem.GetBlocksOfType(_sorters,IsMyGrid);
foreach(var s in _sorters){if(!s.HasInventory)continue;var inv=s.GetInventory(0);if(inv==null)continue;string defKey=s.
BlockDefinition.ToString();bool isWeapon;if(!_sorterIsWeapon.TryGetValue(defKey,out isWeapon)){if(_weaponProbeQueued.Add(defKey))
_weaponProbeQueue.Add(s);continue;}if(isWeapon)_weapons.Add(s);}}List<MyItemType>AcceptedAmmoFor(IMyTerminalBlock block){string defKey=
block.BlockDefinition.ToString();List<MyItemType>ammo;if(_ammoByDef.TryGetValue(defKey,out ammo))return ammo;ammo=new List<
MyItemType>();var inv=block.GetInventory(0);if(inv!=null)inv.GetAcceptedItems(ammo,t=>t.TypeId==AMMO_TYPE_ID);_ammoByDef[defKey]=
ammo;return ammo;}string InjectIgnoreHint(string iniText,string hint){if(iniText.IndexOf(";Ignore=",StringComparison.
OrdinalIgnoreCase)>=0)return iniText;int idx=iniText.IndexOf("["+INI_SECTION+"]",StringComparison.OrdinalIgnoreCase);if(idx<0)return
iniText;int nl=iniText.IndexOf('\n',idx);if(nl<0)return iniText+"\n"+hint+"\n";return iniText.Substring(0,nl+1)+hint+"\n"+
iniText.Substring(nl+1);}int StampHiddenHints(){int stamped=0;foreach(var c in _allContainers){if(!_hiddenContainers.Contains(c
.EntityId))continue;string data=(c.CustomData??"").TrimEnd();if(data.IndexOf("["+INI_SECTION+"]",StringComparison.
OrdinalIgnoreCase)>=0)continue;if(data.IndexOf(";Ignore=",StringComparison.OrdinalIgnoreCase)>=0)continue;string stamp="["+INI_SECTION+
"]\n"+IGNORE_HINT_HIDDEN+"\n";c.CustomData=data.Length==0?stamp:data+"\n\n"+stamp;stamped++;}return stamped;}string
StripIgnoreHintLines(string text){if(text.IndexOf(";Ignore=",StringComparison.OrdinalIgnoreCase)<0&&text.IndexOf("; Hidden from AutoLoad",
StringComparison.OrdinalIgnoreCase)<0&&text.IndexOf("; Uncomment to",StringComparison.OrdinalIgnoreCase)<0)return text;var lines=text.
Split('\n');_sb.Clear();foreach(var raw in lines){string line=raw.TrimEnd('\r');string t=line.TrimStart();if(t.StartsWith(
";Ignore=",StringComparison.OrdinalIgnoreCase))continue;if(t.StartsWith("; Hidden from AutoLoad",StringComparison.
OrdinalIgnoreCase))continue;if(t.StartsWith("; Uncomment to",StringComparison.OrdinalIgnoreCase))continue;_sb.Append(line).Append('\n');}
return _sb.ToString().TrimEnd('\n');}void AutoInit(){RefreshCaches();foreach(var block in AllConfigurableBlocks()){if(
InitBudgetSpent()){_restampQueue.Add(block);continue;}_ini.Clear();if(_ini.TryParse(block.CustomData,out _iniResult)&&_ini.
ContainsSection(INI_SECTION)){_tempIniKeys.Clear();_ini.GetKeys(INI_SECTION,_tempIniKeys);if(_tempIniKeys.Count>0)continue;}InitBlock(
block);}StampHiddenHints();InitDockActions();StampExampleLoadouts();ApplyBlockTags();RefreshLightTags();}void CmdInit(){Log(
"=== INIT ===");int written=0,updated=0,skipped=0;int discovered=DiscoverFromSources(CollectSourceInventories());_restampQueue.Clear()
;int deferred=0;foreach(var block in AllConfigurableBlocks()){if(InitBudgetSpent()){_restampQueue.Add(block);deferred++;
continue;}var result=InitBlock(block);switch(result){case InitResult.Written:written++;break;case InitResult.Updated:updated++;
break;case InitResult.Skipped:skipped++;break;}}Log($"Done: {written} new, {updated} updated, {skipped} unchanged.");if(
deferred>0)Log($"  ({deferred} more finishing in the background)");int hinted=StampHiddenHints();if(_hiddenContainers.Count>0){
int noConv=_hiddenContainers.Count-_configIgnored.Count;if(noConv>0){Log($"{noConv} block(s) no conveyor route:");foreach(
var hc in _allContainers)if(_hiddenContainers.Contains(hc.EntityId)&&!_configIgnored.Contains(hc.EntityId))Log("  - "+Trunc
(hc.CustomName,20));}if(_configIgnored.Count>0){Log($"{_configIgnored.Count} block(s) set Ignore=true:");foreach(var hc
in _allContainers)if(_configIgnored.Contains(hc.EntityId))Log("  - "+Trunc(hc.CustomName,20));}}if(hinted>0)Log(
$"  ({hinted} stamped with ;Ignore= hint)");if(discovered>0)Log($"Registered {discovered} modded item(s).");ApplyGroupConfigs();InitDockActions();RefreshLightTags
();if(_indicatorLights.Count>0)Log($"{_indicatorLights.Count} AutoLight indicator(s).");foreach(string bad in
_badLightItems)Log("  ! AutoLight unknown item: "+bad);if(_ignoredConnectors.Count>0){Log(
$"{_ignoredConnectors.Count} connector(s) ignored:");foreach(var c in _connectors)if(_ignoredConnectors.Contains(c.EntityId))Log("  - "+Trunc(c.CustomName,20));}if(
_activeLoadout.Length>0&&!_activeLoadout.Equals("Off",StringComparison.OrdinalIgnoreCase)){Log(
$"Loadout '{_activeLoadout}' is active.");Log("  Containers follow it. 'loadout off' to stop.");}}enum InitResult{Written,Updated,Skipped}InitResult InitBlock(
IMyTerminalBlock block,bool quiet=false){_ini.Clear();bool hasSec=_ini.TryParse(block.CustomData,out _iniResult)&&_ini.ContainsSection(
INI_SECTION);var existing=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);string ignoreVal=null;if(hasSec){foreach(
var pair in ALL_ITEMS){if(_ini.ContainsKey(INI_SECTION,pair.Key))existing[pair.Key]=_ini.Get(INI_SECTION,pair.Key).ToString
("");}if(_ini.ContainsKey(INI_SECTION,"Ignore"))ignoreVal=_ini.Get(INI_SECTION,"Ignore").ToString("");_ini.DeleteSection(
INI_SECTION);}int newItems=0;foreach(var cat in CATEGORIES){bool firstInCat=true;foreach(string key in cat.Value){string val;if(
existing.TryGetValue(key,out val)){if(val.IndexOf('/')>=0)_ini.Set(INI_SECTION,key,val);else _ini.Set(INI_SECTION,key,val.Equals
("True",StringComparison.OrdinalIgnoreCase));}else{_ini.Set(INI_SECTION,key,false);if(hasSec)newItems++;}if(firstInCat){
_ini.SetComment(INI_SECTION,key,$"\n {cat.Key}");firstInCat=false;}}}if(ignoreVal!=null)_ini.Set(INI_SECTION,"Ignore",
ignoreVal);WriteSectionComment();string stampedData=_ini.ToString();if(ignoreVal==null)stampedData=InjectIgnoreHint(stampedData,
IGNORE_HINT_VISIBLE);block.CustomData=stampedData;if(!hasSec){if(!quiet)Log($"  + {block.CustomName}");return InitResult.Written;}if(
newItems>0){if(!quiet)Log($"  ~ {block.CustomName}  ({newItems} new items added)");return InitResult.Updated;}return InitResult.
Skipped;}void WriteSectionComment(){_ini.SetSectionComment(INI_SECTION," AutoLoad: true/true/50/false");}void CmdToggle(string
blockName,string itemKey){Log($"=== TOGGLE ===");if(!ALL_ITEMS.ContainsKey(itemKey)){Log($"ERROR: Unknown item '{itemKey}'.");Log
("Check spelling or run 'init' to see valid keys.");return;}IMyTerminalBlock target=FindBlock(blockName);if(target==null)
{Log($"ERROR: No container/turret named '{blockName}'.");return;}_ini.Clear();if(!_ini.TryParse(target.CustomData,out
_iniResult)||!_ini.ContainsSection(INI_SECTION)){Log($"Block has no [AutoLoad] section. Run 'init' first.");return;}bool was=_ini.
Get(INI_SECTION,itemKey).ToBoolean(false);_ini.Set(INI_SECTION,itemKey,!was);target.CustomData=_ini.ToString();Log(
$"  Block : {target.CustomName}");Log($"  Item  : {itemKey}");Log($"  State : {(was?"ON":"OFF")} -> {(!was?"ON":"OFF")}");}void CmdGroup(string
groupName,string itemSpec){Log("=== GROUP ===");Log($"  Group: '{groupName}'");Log($"  Items: '{itemSpec}'");var itemEntries=new
List<KeyValuePair<string,string>>();string[]entries=itemSpec.Split(',');foreach(string entry in entries){string trimmed=
entry.Trim();if(trimmed.Length==0)continue;int slash=trimmed.IndexOf('/');string key;string val;if(slash>=0){key=trimmed.
Substring(0,slash).Trim();string weight=trimmed.Substring(slash+1).Trim();val="true/"+weight;}else{key=trimmed;val="true";}if(!
ALL_ITEMS.ContainsKey(key)){Log($"ERROR: Unknown item '{key}'.");return;}itemEntries.Add(new KeyValuePair<string,string>(key,val)
);}if(itemEntries.Count==0){Log("ERROR: No valid items specified.");return;}var group=GridTerminalSystem.
GetBlockGroupWithName(groupName);if(group==null){Log($"ERROR: No group named '{groupName}'.");return;}_tempBlocks.Clear();group.GetBlocks(
_tempBlocks,b=>b.IsSameConstructAs(Me)&&b.HasInventory);Log($"  Found {_tempBlocks.Count} block(s).");if(_tempBlocks.Count==0)
return;int configured=0,noConfig=0;foreach(var block in _tempBlocks){_ini.Clear();if(!_ini.TryParse(block.CustomData,out
_iniResult)||!_ini.ContainsSection(INI_SECTION)){Log($"  SKIP {Trunc(block.CustomName,20)}: no [AutoLoad] config");noConfig++;
continue;}var setKeys=new HashSet<string>(StringComparer.OrdinalIgnoreCase);foreach(var ie in itemEntries)setKeys.Add(ie.Key);
foreach(var pair in ALL_ITEMS){if(setKeys.Contains(pair.Key)){foreach(var ie in itemEntries){if(ie.Key.Equals(pair.Key,
StringComparison.OrdinalIgnoreCase)){_ini.Set(INI_SECTION,pair.Key,ie.Value);break;}}}else{_ini.Set(INI_SECTION,pair.Key,false);}}block.
CustomData=_ini.ToString();configured++;}Log($"Done: {configured} configured, {noConfig} no config.");foreach(var ie in
itemEntries)Log($"  {ie.Key} = {ie.Value}");}bool IsDocked(){foreach(var c in _connectors)if(c.Status==MyShipConnectorStatus.
Connected&&IsOwnConnector(c))return true;return _mechDockGrids.Count>0;}void RefreshMechDock(){_mechDockGrids.Clear();_mechBlocks
.Clear();GridTerminalSystem.GetBlocksOfType(_mechBlocks,b=>b.IsSameConstructAs(Me));long myGridId=Me.CubeGrid.EntityId;
foreach(var mech in _mechBlocks){if(!mech.IsAttached)continue;var baseGrid=mech.CubeGrid;if(baseGrid!=null&&baseGrid.IsStatic&&
baseGrid.EntityId!=myGridId)_mechDockGrids.Add(baseGrid.EntityId);var topGrid=mech.TopGrid;if(topGrid!=null&&topGrid.IsStatic&&
topGrid.EntityId!=myGridId)_mechDockGrids.Add(topGrid.EntityId);}}void ApplyGroupConfigs(){var allGroups=new List<IMyBlockGroup
>();GridTerminalSystem.GetBlockGroups(allGroups);foreach(var group in allGroups){string name=group.Name;int open=name.
IndexOf('[');int close=name.IndexOf(']',open>=0?open:0);if(open<0||close<=open+1)continue;string itemKey=name.Substring(open+1,
close-open-1).Trim();if(!ALL_ITEMS.ContainsKey(itemKey))continue;_tempBlocks.Clear();group.GetBlocks(_tempBlocks,b=>b.
IsSameConstructAs(Me)&&b.HasInventory);Log($"Group '{name}': {_tempBlocks.Count} block(s)");foreach(var block in _tempBlocks){_ini.Clear(
);if(!_ini.TryParse(block.CustomData,out _iniResult)||!_ini.ContainsSection(INI_SECTION)){Log(
$"  SKIP {Trunc(block.CustomName,20)}: no config");continue;}if(_ini.Get(INI_SECTION,itemKey).ToBoolean(false)){Log($"  OK   {Trunc(block.CustomName,20)}: already ON");
continue;}_ini.Set(INI_SECTION,itemKey,true);block.CustomData=_ini.ToString();Log(
$"  SET  {Trunc(block.CustomName,20)}: {itemKey} -> ON");}}}void InitDockActions(){_ini.Clear();_ini.TryParse(Me.CustomData,out _iniResult);string[]keys={"Batteries","H2Tanks"
,"Thrusters","Gyros","Weapons","Lights","Antennas"};foreach(string k in keys){if(!_ini.ContainsKey(DOCK_SECTION,k))_ini.
Set(DOCK_SECTION,k,"Skip");}_ini.SetSectionComment(DOCK_SECTION," Dock actions: Skip or Recharge/Stockpile/Off");if(!_ini.
ContainsSection("SmartLoad")){_ini.Set("SmartLoad","Enabled",true);_ini.Set("SmartLoad","SafetyMargin",80);_ini.Set("SmartLoad",
"Gravity","auto");_ini.SetSectionComment("SmartLoad"," SmartLoad: Enabled, SafetyMargin, Gravity");}if(!_ini.ContainsSection(
MODDED_SECTION)){_ini.Set(MODDED_SECTION,"Discover",true);_ini.SetSectionComment(MODDED_SECTION,MODDED_COMMENT);}_ini.Set(
"AutoLoad Help","version","3.2");_ini.SetSectionComment("AutoLoad Help"," AutoLoad v3.2\n"+" Commands: init pull loadout toggle\n"+
"   group scan reset status calibrate\n"+" LCD: [AutoLoad] [AutoLog] [AutoMenu]\n"+"   [AutoTotals] - whole-ship cargo\n"+
" Lights: [AutoLight] or [AutoLight:Item]\n"+" Connector: [AutoLoad] Ignore=true\n"+"   stops it counting as a dock\n"+" Debug: AutoDebug in a panel's\n"+
"   Custom Data shows tick cost");Me.CustomData=_ini.ToString();Log("  [DockActions] ready.");}void ReadSmartLoadConfig(out bool enabled,out float
safetyMargin,out string gravity){enabled=true;safetyMargin=80f;gravity="auto";_ini.Clear();if(!_ini.TryParse(Me.CustomData,out
_iniResult))return;if(!_ini.ContainsSection("SmartLoad"))return;enabled=_ini.Get("SmartLoad","Enabled").ToBoolean(true);int margin
=_ini.Get("SmartLoad","SafetyMargin").ToInt32(80);if(margin<1)margin=1;if(margin>99)margin=99;safetyMargin=margin;gravity
=_ini.Get("SmartLoad","Gravity").ToString("auto").Trim();}bool ReadCalibrationData(out float baseMass,out float upThrust,
out float gravity){baseMass=0f;upThrust=0f;gravity=0f;_ini.Clear();if(!_ini.TryParse(Me.CustomData,out _iniResult))return
false;if(!_ini.ContainsSection("Calibration"))return false;if(!_ini.Get("Calibration","Calibrated").ToBoolean(false))return
false;baseMass=(float)_ini.Get("Calibration","BaseMass").ToDouble(0);upThrust=(float)_ini.Get("Calibration","Up").ToDouble(0)
;gravity=(float)_ini.Get("Calibration","Gravity").ToDouble(0);return baseMass>0f&&upThrust>0f;}void SaveCalibrationData()
{_ini.Clear();_ini.TryParse(Me.CustomData,out _iniResult);_ini.Set("Calibration","Calibrated",true);_ini.Set(
"Calibration","BaseMass",(int)_calBaseMass);_ini.Set("Calibration","Forward",(int)_calResults[0]);_ini.Set("Calibration","Left",(int)
_calResults[1]);_ini.Set("Calibration","Right",(int)_calResults[2]);_ini.Set("Calibration","Up",(int)_calResults[3]);_ini.Set(
"Calibration","Down",(int)_calResults[4]);_ini.Set("Calibration","Gravity",_calGravity);_ini.SetSectionComment("Calibration",
" Calibration data (run 'calibrate')");Me.CustomData=_ini.ToString();}void UpdateCalibration(){if(_calState<2||_calState>6)return;if(_controllers.Count==0)
return;var ctrl=_controllers[0];int dirIdx=_calState-2;Vector3D targetDir;switch(dirIdx){case 0:targetDir=ctrl.WorldMatrix.
Forward;break;case 1:targetDir=ctrl.WorldMatrix.Left;break;case 2:targetDir=ctrl.WorldMatrix.Right;break;case 3:targetDir=ctrl.
WorldMatrix.Up;break;case 4:targetDir=ctrl.WorldMatrix.Down;break;default:return;}Vector3D velocity=ctrl.GetShipVelocities().
LinearVelocity;double velInDir=Vector3D.Dot(velocity,targetDir);if(velInDir<2.0)return;float dirThrust=0f;foreach(var thruster in
_thrusters){if(!thruster.IsFunctional)continue;Vector3D thrustDir=thruster.WorldMatrix.Backward;float contribution=(float)Vector3D
.Dot(thrustDir,targetDir);if(contribution>0.1f)dirThrust+=thruster.MaxEffectiveThrust*contribution;}if(dirThrust>0f){
_calThrustSum+=dirThrust;_calReadings++;}_calTicks++;if(_calReadings>=30){_calResults[dirIdx]=_calThrustSum/_calReadings;
_calThrustSum=0f;_calReadings=0;_calTicks=0;_calState++;if(_calState==7){var shipMass=ctrl.CalculateShipMass();_calBaseMass=shipMass.
BaseMass;Vector3D calGravVec=ctrl.GetNaturalGravity();_calGravity=(float)calGravVec.Length();SaveCalibrationData();
_calCompleteTicks=0;}}}public static bool IsLiftCapable(float upThrustNewtons,float baseMassKg){if(baseMassKg<=0f)return true;if(
upThrustNewtons<=0f)return false;return upThrustNewtons>=baseMassKg*9.81f*0.99f;}bool IsGroundVehicle(){if(_suspensions.Count==0)return
false;if(_controllers.Count==0)return true;var ctrl=_controllers[0];Vector3D shipUp=ctrl.WorldMatrix.Up;float up=0f;foreach(
var thruster in _thrusters){if(!thruster.IsFunctional)continue;float contribution=(float)Vector3D.Dot(thruster.WorldMatrix.
Backward,shipUp);if(contribution>0f)up+=thruster.MaxThrust*contribution;}return!IsLiftCapable(up,ctrl.CalculateShipMass().
BaseMass);}float CalcMaxCargoMass(out float baseMass,out float currentCargoMass,out float upThrust,out float gravMag,out float
fuelMass){baseMass=0f;currentCargoMass=0f;upThrust=0f;gravMag=0f;fuelMass=0f;fuelMass=0f;float calBm,calUt,calStoredGrav;if(
ReadCalibrationData(out calBm,out calUt,out calStoredGrav)){baseMass=calBm;currentCargoMass=0f;upThrust=calUt;foreach(var cb in
_allContainers){if(!cb.HasInventory)continue;currentCargoMass+=(float)cb.GetInventory(0).CurrentMass;}bool calSlEnabled;float
calMargin;string calGravCfg;ReadSmartLoadConfig(out calSlEnabled,out calMargin,out calGravCfg);Vector3D calGravVec;if(
_controllers.Count>0){var calCtrl=_controllers[0];if(calGravCfg.Equals("auto",StringComparison.OrdinalIgnoreCase)){calGravVec=
calCtrl.GetNaturalGravity();}else{float gVal;if(float.TryParse(calGravCfg,out gVal)&&gVal>0f)calGravVec=calCtrl.WorldMatrix.
Down*gVal;else calGravVec=calCtrl.GetNaturalGravity();}}else calGravVec=Vector3D.Zero;gravMag=(float)calGravVec.Length();if(
gravMag<0.01f)return float.MaxValue;float calMaxLift=calUt/gravMag;float calMaxCargo=calMaxLift*(calMargin/100f)-calBm-fuelMass
;if(calMaxCargo<=0f&&IsGroundVehicle())return float.MaxValue;if(calMaxCargo<0f)calMaxCargo=0f;return calMaxCargo;}if(
_controllers.Count==0){Log("SmartLoad: no controller");return float.MaxValue;}var ctrl=_controllers[0];foreach(var cb in
_allContainers){if(!cb.HasInventory)continue;currentCargoMass+=(float)cb.GetInventory(0).CurrentMass;}baseMass=ctrl.CalculateShipMass(
).BaseMass;bool smartEnabled;float safetyMargin;string gravCfg;ReadSmartLoadConfig(out smartEnabled,out safetyMargin,out
gravCfg);Vector3D gravVec;if(gravCfg.Equals("auto",StringComparison.OrdinalIgnoreCase)){gravVec=ctrl.GetNaturalGravity();}else{
float gVal;if(float.TryParse(gravCfg,out gVal)&&gVal>0f)gravVec=ctrl.WorldMatrix.Down*gVal;else gravVec=ctrl.
GetNaturalGravity();}gravMag=(float)gravVec.Length();if(gravMag<0.01f)return float.MaxValue;Vector3D gravDir=gravVec/gravMag;foreach(var
thruster in _thrusters){if(!thruster.IsFunctional)continue;Vector3D thrustDir=thruster.WorldMatrix.Backward;float contribution=(
float)(-Vector3D.Dot(thrustDir,gravDir));if(contribution>0f)upThrust+=thruster.MaxThrust*contribution;}if(IsGroundVehicle())
return float.MaxValue;if(upThrust<1f){Log("SmartLoad: no upward thrust");return 0f;}float maxLiftMass=upThrust/gravMag;float
maxCargoMass=maxLiftMass*(safetyMargin/100f)-baseMass-fuelMass;if(maxCargoMass<0f)maxCargoMass=0f;return maxCargoMass;}private
readonly string[]VALID_DOCK_VALUES={"Skip","Recharge","Stockpile","Off"};void ReadDockActions(Dictionary<string,string>actions){
actions.Clear();_ini.Clear();if(!_ini.TryParse(Me.CustomData,out _iniResult))return;string[]keys={"Batteries","H2Tanks",
"Thrusters","Gyros","Weapons","Lights","Antennas"};foreach(string k in keys){string val=_ini.Get(DOCK_SECTION,k).ToString("Skip").
Trim();bool valid=false;foreach(string v in VALID_DOCK_VALUES){if(val.Equals(v,StringComparison.OrdinalIgnoreCase)){valid=
true;break;}}if(!valid){Log($"  WARNING: DockActions '{k}={val}' invalid, using Skip");val="Skip";}actions[k]=val;}}bool
DockAction(Dictionary<string,string>actions,string key,string expected){string val;return actions.TryGetValue(key,out val)&&val.
Equals(expected,StringComparison.OrdinalIgnoreCase);}void ApplyDockActions(){var actions=new Dictionary<string,string>(
StringComparer.OrdinalIgnoreCase);ReadDockActions(actions);if(DockAction(actions,"Batteries","Recharge")){for(int i=0;i<_batteries.
Count;i++){if(i==_batteries.Count-1)_batteries[i].ChargeMode=ChargeMode.Auto;else _batteries[i].ChargeMode=ChargeMode.
Recharge;}}if(DockAction(actions,"H2Tanks","Stockpile"))foreach(var tank in _gasTanks)tank.Stockpile=true;if(DockAction(actions,
"Thrusters","Off"))foreach(var t in _thrusters)t.Enabled=false;if(DockAction(actions,"Gyros","Off"))foreach(var g in _gyros)g.
Enabled=false;if(DockAction(actions,"Weapons","Off"))foreach(var w in _turrets)w.Enabled=false;if(DockAction(actions,"Lights",
"Off"))foreach(var l in _lights)if(!_indicatorLights.Contains(l.EntityId))l.Enabled=false;if(DockAction(actions,"Antennas",
"Off"))foreach(var a in _antennas)a.Enabled=false;}void ReverseDockActions(){Log("Undocked");var actions=new Dictionary<
string,string>(StringComparer.OrdinalIgnoreCase);ReadDockActions(actions);if(DockAction(actions,"Batteries","Recharge"))
foreach(var b in _batteries)b.ChargeMode=ChargeMode.Auto;if(DockAction(actions,"H2Tanks","Stockpile"))foreach(var tank in
_gasTanks)tank.Stockpile=false;if(DockAction(actions,"Thrusters","Off"))foreach(var t in _thrusters)t.Enabled=true;if(DockAction(
actions,"Gyros","Off"))foreach(var g in _gyros)g.Enabled=true;if(DockAction(actions,"Weapons","Off"))foreach(var w in _turrets)
w.Enabled=true;if(DockAction(actions,"Lights","Off"))foreach(var l in _lights)if(!_indicatorLights.Contains(l.EntityId))l
.Enabled=true;if(DockAction(actions,"Antennas","Off"))foreach(var a in _antennas)a.Enabled=true;}void CmdReset(){Log(
"=== RESET ===");int cleared=0;foreach(var block in AllBlocksUnfiltered()){_ini.Clear();if(!_ini.TryParse(block.CustomData,out
_iniResult)||!_ini.ContainsSection(INI_SECTION))continue;_ini.DeleteSection(INI_SECTION);block.CustomData=StripIgnoreHintLines(
_ini.ToString());Log($"  - {Trunc(block.CustomName,24)}");cleared++;}Log(
$"Done: cleared {cleared} block(s). Run 'init' to re-stamp.");if(_activeLoadout.Length>0&&!_activeLoadout.Equals("Off",StringComparison.OrdinalIgnoreCase))Log(
$"Loadout '{_activeLoadout}' switched off.");_activeLoadout="Off";_cachedLoadoutConfig="";}IMyTerminalBlock FindBlock(string name){foreach(var c in _containers)if(
c.CustomName.Equals(name,StringComparison.OrdinalIgnoreCase))return c;foreach(var t in _turrets)if(t.CustomName.Equals(
name,StringComparison.OrdinalIgnoreCase))return t;return null;}List<string>ListLoadoutNames(){var names=new List<string>();
_ini.Clear();if(!_ini.TryParse(Me.CustomData,out _iniResult))return names;var sections=new List<string>();_ini.GetSections(
sections);foreach(var sec in sections){if(sec.StartsWith(LOADOUT_PREFIX,StringComparison.OrdinalIgnoreCase))names.Add(sec.
Substring(LOADOUT_PREFIX.Length));}return names;}Dictionary<string,string>ParseLoadout(string name){var items=new Dictionary<
string,string>(StringComparer.OrdinalIgnoreCase);_ini.Clear();if(!_ini.TryParse(Me.CustomData,out _iniResult))return items;
string section=LOADOUT_PREFIX+name;if(!_ini.ContainsSection(section))return items;var keys=new List<MyIniKey>();_ini.GetKeys(
section,keys);foreach(var key in keys)items[key.Name]=_ini.Get(key).ToString("");return items;}void ApplyLoadoutToBlock(
IMyTerminalBlock block,Dictionary<string,string>loadoutItems){_ini.Clear();_ini.TryParse(block.CustomData,out _iniResult);foreach(var
pair in ALL_ITEMS){string val;if(loadoutItems.TryGetValue(pair.Key,out val))_ini.Set(INI_SECTION,pair.Key,val);else _ini.Set
(INI_SECTION,pair.Key,false);}block.CustomData=_ini.ToString();}string FindBlockLoadoutTag(IMyTerminalBlock block){string
bName=block.CustomName;int open=bName.IndexOf('[');while(open>=0){int close=bName.IndexOf(']',open+1);if(close<0)break;string
tag=bName.Substring(open+1,close-open-1).Trim();if(tag.Length>0){var checkIni=new MyIni();MyIniParseResult checkResult;if(
checkIni.TryParse(Me.CustomData,out checkResult)&&checkIni.ContainsSection(LOADOUT_PREFIX+tag))return tag;}open=bName.IndexOf(
'[',close+1);}return null;}void ApplyLoadoutGlobal(string name){var loadoutItems=ParseLoadout(name);if(loadoutItems.Count==
0){Log($"ERROR: Loadout '{name}' not found or empty.");Log("Use 'loadout list' to see available loadouts.");return;}int
applied=0;int skippedTag=0;foreach(var block in AllConfigurableBlocks()){string blockTag=FindBlockLoadoutTag(block);if(blockTag
!=null){skippedTag++;continue;}ApplyLoadoutToBlock(block,loadoutItems);applied++;}_activeLoadout=name;_cachedLoadoutConfig
=BuildConfigString(loadoutItems);_loadoutJustApplied=true;Log($"Loadout '{name}' applied to {applied} block(s).");if(
skippedTag>0)Log($"  ({skippedTag} block(s) skipped — have per-block tag)");}void ApplyBlockTags(){foreach(var block in
AllConfigurableBlocks()){string tag=FindBlockLoadoutTag(block);if(tag==null)continue;var loadoutItems=ParseLoadout(tag);if(loadoutItems.Count
==0)continue;ApplyLoadoutToBlock(block,loadoutItems);}}string BuildConfigString(Dictionary<string,string>items){var keys=
new List<string>(items.Keys);keys.Sort(StringComparer.OrdinalIgnoreCase);_sb.Clear();foreach(var k in keys)_sb.Append(k).
Append('=').Append(items[k]).Append(';');return _sb.ToString();}Dictionary<string,string>ReadBlockConfigRaw(IMyTerminalBlock
block){var result=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);var blockIni=new MyIni();MyIniParseResult
blockResult;if(!blockIni.TryParse(block.CustomData,out blockResult)||!blockIni.ContainsSection(INI_SECTION))return result;foreach(
var pair in ALL_ITEMS){string val=blockIni.Get(INI_SECTION,pair.Key).ToString("");if(val.Length==0)continue;float pct;bool
on=ParseItemValue(val,out pct);if(on)result[pair.Key]=val;}return result;}void SaveToLoadout(string name,Dictionary<string
,string>items){_ini.Clear();_ini.TryParse(Me.CustomData,out _iniResult);string section=LOADOUT_PREFIX+name;var keys=new
List<MyIniKey>();if(_ini.ContainsSection(section))_ini.GetKeys(section,keys);foreach(var k in keys)_ini.Delete(k);foreach(
var kv in items)_ini.Set(section,kv.Key,kv.Value);Me.CustomData=_ini.ToString();}public const int SYNC_NONE=0,SYNC_PUSH=1,
SYNC_ADOPT=2;public static int LoadoutSyncAction(string loadoutStr,string cachedStr){if(string.IsNullOrEmpty(cachedStr))return
SYNC_ADOPT;return loadoutStr!=cachedStr?SYNC_PUSH:SYNC_NONE;}void SyncLoadoutConfig(){if(_activeLoadout.Length==0||_activeLoadout.
Equals("Off",StringComparison.OrdinalIgnoreCase))return;if(_loadoutJustApplied){_loadoutJustApplied=false;return;}var
loadoutItems=ParseLoadout(_activeLoadout);if(loadoutItems.Count==0)return;string loadoutStr=BuildConfigString(loadoutItems);int
action=LoadoutSyncAction(loadoutStr,_cachedLoadoutConfig);if(action==SYNC_ADOPT){_cachedLoadoutConfig=loadoutStr;return;}if(
action==SYNC_PUSH){foreach(var block in AllConfigurableBlocks()){if(FindBlockLoadoutTag(block)!=null)continue;
ApplyLoadoutToBlock(block,loadoutItems);}_cachedLoadoutConfig=loadoutStr;Log("Loadout -> containers");return;}foreach(var block in
AllConfigurableBlocks()){if(FindBlockLoadoutTag(block)!=null)continue;var blockConfig=ReadBlockConfigRaw(block);string blockStr=
BuildConfigString(blockConfig);if(blockStr!=loadoutStr){SaveToLoadout(_activeLoadout,blockConfig);_cachedLoadoutConfig=BuildConfigString(
blockConfig);foreach(var other in AllConfigurableBlocks()){if(FindBlockLoadoutTag(other)!=null)continue;if(other==block)continue;
ApplyLoadoutToBlock(other,blockConfig);}Log("Container -> loadout");return;}}}void CmdLoadout(string[]parts){if(parts.Length<2){Log(
"Usage:");Log("  loadout <name>       Apply a loadout");Log("  loadout list         Show all loadouts");Log(
"  loadout save <name>  Save current config");return;}string sub=parts[1].ToLowerInvariant();if(sub=="off"){Log("=== LOADOUT: OFF ===");var empty=new Dictionary<
string,string>(StringComparer.OrdinalIgnoreCase);int applied=0;foreach(var block in AllConfigurableBlocks()){
ApplyLoadoutToBlock(block,empty);applied++;}_activeLoadout="Off";_cachedLoadoutConfig="";Log(
$"All items set to false on {applied} block(s).");Log("Containers will empty back to base.");return;}if(sub=="list"){var names=ListLoadoutNames();Log("=== LOADOUTS ==="
);if(names.Count==0){Log("  No loadouts defined.");Log("  Add [Loadout:Name] sections to PB Custom Data.");}else{foreach(
var n in names){string marker=n.Equals(_activeLoadout,StringComparison.OrdinalIgnoreCase)?" <<<":"";Log($"  {n}{marker}");}
}return;}if(sub=="save"&&parts.Length>=3){string saveName=string.Join(" ",parts,2,parts.Length-2);CmdLoadoutSave(saveName
);return;}string loadoutName=string.Join(" ",parts,1,parts.Length-1);Log($"=== LOADOUT: {loadoutName} ===");
ApplyLoadoutGlobal(loadoutName);}void CmdLoadoutSave(string name){Log($"=== SAVE LOADOUT: {name} ===");_ini.Clear();_ini.TryParse(Me.
CustomData,out _iniResult);string section=LOADOUT_PREFIX+name;bool found=false;foreach(var block in AllConfigurableBlocks()){var
blockIni=new MyIni();MyIniParseResult blockResult;if(!blockIni.TryParse(block.CustomData,out blockResult)||!blockIni.
ContainsSection(INI_SECTION))continue;foreach(var pair in ALL_ITEMS){string val=blockIni.Get(INI_SECTION,pair.Key).ToString("");if(val.
Length==0)continue;float pct;bool on=ParseItemValue(val,out pct);if(on)_ini.Set(section,pair.Key,val);}found=true;break;}if(!
found){Log("ERROR: No configured blocks found.");return;}Me.CustomData=_ini.ToString();Log(
$"Saved loadout '{name}' to PB Custom Data.");}void StampExampleLoadouts(){_ini.Clear();if(!_ini.TryParse(Me.CustomData,out _iniResult))return;var sections=new List
<string>();_ini.GetSections(sections);foreach(var sec in sections){if(sec.StartsWith(LOADOUT_PREFIX,StringComparison.
OrdinalIgnoreCase))return;}_ini.Set("Loadout:Build","SteelPlate","true/25");_ini.Set("Loadout:Build","InteriorPlate","true/15");_ini.Set(
"Loadout:Build","Construction","true/15");_ini.Set("Loadout:Build","SmallTube","true/10");_ini.Set("Loadout:Build","Motor","true/10");
_ini.Set("Loadout:Build","Computer","true/10");_ini.Set("Loadout:Build","BulletproofGlass","true/5");_ini.Set(
"Loadout:Build","MetalGrid","true/5");_ini.Set("Loadout:Build","LargeTube","true/5");_ini.SetSectionComment("Loadout:Build",
" Build & repair");_ini.Set("Loadout:Combat","NATO_25x184mm","true/30");_ini.Set("Loadout:Combat","Missile200mm","true/20");_ini.Set(
"Loadout:Combat","AutocannonClip","true/15");_ini.Set("Loadout:Combat","SteelPlate","true/20");_ini.Set("Loadout:Combat","Construction",
"true/10");_ini.Set("Loadout:Combat","Computer","true/5");_ini.SetSectionComment("Loadout:Combat"," Combat loadout");Me.
CustomData=_ini.ToString();}bool ReadEnabledItems(IMyTerminalBlock block,Dictionary<MyItemType,float>result){result.Clear();_ini.
Clear();if(!_ini.TryParse(block.CustomData,out _iniResult)||!_ini.ContainsSection(INI_SECTION))return false;_tempIniKeys.
Clear();_ini.GetKeys(INI_SECTION,_tempIniKeys);foreach(var iniKey in _tempIniKeys){string name=iniKey.Name;if(name.Equals(
"Ignore",StringComparison.OrdinalIgnoreCase))continue;MyItemType type;if(!ALL_ITEMS.TryGetValue(name,out type))continue;string
val=_ini.Get(iniKey).ToString("");if(val.Length==0)continue;float weight;bool on=ParseItemValue(val,out weight);if(on)
result[type]=weight;}return result.Count>0;}MyFixedPoint PushItemToAny(IMyInventory source,MyItemType itemType,MyFixedPoint
maxUnits,List<IMyInventory>destinations){MyFixedPoint totalMoved=MyFixedPoint.Zero;MyFixedPoint remaining=maxUnits;foreach(var
dest in destinations){if(remaining<=0)break;if(!source.CanTransferItemTo(dest,itemType))continue;if((float)dest.
CurrentVolume>=(float)dest.MaxVolume-0.001f)continue;_tempItems.Clear();source.GetItems(_tempItems);for(int i=_tempItems.Count-1;i>=0
&&remaining>0;i--){if(_tempItems[i].Type!=itemType)continue;MyFixedPoint take=MyFixedPoint.Min(remaining,_tempItems[i].
Amount);float volBefore=(float)source.CurrentVolume;source.TransferItemTo(dest,_tempItems[i],take);float volAfter=(float)
source.CurrentVolume;if(volBefore-volAfter>0.0001f){totalMoved+=take;remaining-=take;}}}return totalMoved;}MyFixedPoint
PullItemFromAny(IMyInventory destination,MyItemType itemType,MyFixedPoint maxUnits,List<IMyInventory>sources){MyFixedPoint totalMoved=
MyFixedPoint.Zero;MyFixedPoint remaining=maxUnits;foreach(var src in sources){if(remaining<=0)break;if(!src.CanTransferItemTo(
destination,itemType))continue;_tempItems.Clear();src.GetItems(_tempItems);for(int i=_tempItems.Count-1;i>=0&&remaining>0;i--){if(
_tempItems[i].Type!=itemType)continue;MyFixedPoint take=MyFixedPoint.Min(remaining,_tempItems[i].Amount);if(src.TransferItemTo(
destination,_tempItems[i],take)){totalMoved+=take;remaining-=take;}}}return totalMoved;}void CmdPull(bool silent=false){if(!silent)
Log("=== PULL ===");var sources=CollectSourceInventories();if(sources.Count==0){if(!silent)Log(
"ERROR: No connected inventories. Dock first.");return;}var stock=BuildStockSnapshot(sources);if(stock.Count==0)return;var demandCount=new Dictionary<MyItemType,int>(
);var enabledScan=new Dictionary<MyItemType,float>();foreach(var block in AllConfigurableBlocks()){if(!ReadEnabledItems(
block,enabledScan))continue;var dest=block.GetInventory(0);if(dest==null)continue;float maxL=(float)dest.MaxVolume*1000f;
float curL=(float)dest.CurrentVolume*1000f;if(maxL-curL<MIN_TRANSFER_LITRES)continue;foreach(var kv in enabledScan){int count
;demandCount.TryGetValue(kv.Key,out count);demandCount[kv.Key]=count+1;}}int totalXfers=0;float totalLitres=0f;foreach(
var block in AllConfigurableBlocks()){int xfers;float litres;PullIntoBlock(block,sources,stock,demandCount,out xfers,out
litres);totalXfers+=xfers;totalLitres+=litres;}if(totalXfers>0||!silent)Log(
$"Pulled: {totalXfers} transfer(s), ~{totalLitres:F1} L moved.");}void PullIntoBlock(IMyTerminalBlock block,List<IMyInventory>sources,Dictionary<MyItemType,MyFixedPoint>stock,
Dictionary<MyItemType,int>demandCount,out int xferCount,out float movedLitres){xferCount=0;movedLitres=0f;if(!block.HasInventory)
return;var enabled=new Dictionary<MyItemType,float>();if(!ReadEnabledItems(block,enabled))return;var dest=block.GetInventory(0
);if(dest==null)return;float maxL=(float)dest.MaxVolume*1000f;float curL=(float)dest.CurrentVolume*1000f;float freeL=maxL
-curL;if(freeL<MIN_TRANSFER_LITRES){Log($"  {Trunc(block.CustomName,24)}: already full.");return;}var targets=new
Dictionary<MyItemType,float>();ComputeTargets(enabled,maxL,targets);var held=SnapshotInventory(dest);bool blockHeaderWritten=false
;int blockXfers=0;float blockLitres=0f;foreach(var kv in enabled){var itemType=kv.Key;float targetL;if(!targets.
TryGetValue(itemType,out targetL))continue;MyFixedPoint heldAmt;held.TryGetValue(itemType,out heldAmt);float heldL=ToLitres(
itemType,(float)heldAmt);float neededL=targetL-heldL;if(neededL<MIN_TRANSFER_LITRES)continue;MyFixedPoint availAmt;if(!stock.
TryGetValue(itemType,out availAmt)||availAmt<=0)continue;int blocksWanting;demandCount.TryGetValue(itemType,out blocksWanting);if(
blocksWanting<1)blocksWanting=1;float fairAmt=(float)availAmt/blocksWanting;float neededUnits=FromLitres(itemType,neededL);float
transferUnits=neededUnits;if(transferUnits>fairAmt)transferUnits=fairAmt;if(transferUnits>(float)availAmt)transferUnits=(float)
availAmt;if(transferUnits<0.001f)continue;var toTransfer=(MyFixedPoint)transferUnits;MyFixedPoint actualMoved=PullItemFromAny(
dest,itemType,toTransfer,sources);float actualL=ToLitres(itemType,(float)actualMoved);if(actualMoved>0){stock[itemType]=
stock[itemType]-actualMoved;if(stock[itemType]<=0)stock.Remove(itemType);}int dc;demandCount.TryGetValue(itemType,out dc);if(
dc>1)demandCount[itemType]=dc-1;else demandCount.Remove(itemType);blockLitres+=actualL;if(actualL>=MIN_TRANSFER_LITRES){if
(!blockHeaderWritten){Log($"  {Trunc(block.CustomName,24)} ({enabled.Count} slot(s)):");blockHeaderWritten=true;}float
partial=((float)availAmt<neededUnits)?(float)availAmt/neededUnits*100f:100f;string suffix=(partial<99.9f)?
$" [partial {partial:F0}%]":"";Log($"    {KeyOf(itemType)}: +{actualL:F1} L{suffix}");blockXfers++;}}xferCount+=blockXfers;movedLitres+=blockLitres
;if(!blockHeaderWritten&&blockLitres<MIN_TRANSFER_LITRES){Log($"  {Trunc(block.CustomName,24)}: already stocked.");}}
private static bool ParseItemValue(string val,out float pct){pct=100f;if(val.Length==0)return false;int slash=val.IndexOf('/');
if(slash<0)return val.Trim().Equals("true",StringComparison.OrdinalIgnoreCase);string left=val.Substring(0,slash).Trim();
if(!left.Equals("true",StringComparison.OrdinalIgnoreCase))return false;string right=val.Substring(slash+1).Trim();int p;
if(int.TryParse(right,out p)&&p>0)pct=p;return true;}void ComputeTargets(Dictionary<MyItemType,float>enabled,float maxL,
Dictionary<MyItemType,float>targets){targets.Clear();float totalWeight=0f;foreach(var kv in enabled)totalWeight+=kv.Value;if(
totalWeight<=0f)return;float divisor=Math.Max(totalWeight,100f);foreach(var kv in enabled)targets[kv.Key]=maxL*kv.Value/divisor;}
int DiscoverFromSources(List<IMyInventory>sources){if(!_discoverEnabled){if(_discoveredDirty)SaveDiscovered();return 0;}int
added=0;for(int i=0;i<sources.Count;i++){_tempItems.Clear();sources[i].GetItems(_tempItems);foreach(var item in _tempItems)if
(RegisterDiscoveredType(item.Type,0f))added++;}foreach(var weapon in _weapons){var accepted=AcceptedAmmoFor(weapon);for(
int i=0;i<accepted.Count;i++)if(RegisterDiscoveredType(accepted[i],0f))added++;}if(added>0){_restampQueue.Clear();foreach(
var block in AllConfigurableBlocks())_restampQueue.Add(block);SaveDiscovered();}else if(_discoveredDirty){SaveDiscovered();
}return added;}void ProcessRestampQueue(){int done=0;while(_restampQueue.Count>0&&done<RESTAMP_PER_TICK){int last=
_restampQueue.Count-1;var block=_restampQueue[last];_restampQueue.RemoveAt(last);InitBlock(block,true);done++;}}void
ProcessWeaponProbes(){if(_weaponProbeQueue.Count==0)return;var block=_weaponProbeQueue[0];_weaponProbeQueue.RemoveAt(0);string defKey=block
.BlockDefinition.ToString();_weaponProbeQueued.Remove(defKey);bool isWeapon=false;var inv=block.HasInventory?block.
GetInventory(0):null;if(inv!=null){_tempTypes.Clear();inv.GetAcceptedItems(_tempTypes,t=>t.TypeId!=AMMO_TYPE_ID);isWeapon=_tempTypes
.Count==0;if(isWeapon){_tempTypes.Clear();inv.GetAcceptedItems(_tempTypes,t=>t.TypeId==AMMO_TYPE_ID);isWeapon=_tempTypes.
Count>0;}}_sorterIsWeapon[defKey]=isWeapon;if(_weaponProbeQueue.Count==0)_scanCountdown=0;}void ProcessVolumeProbes(){if(
_volumeProbeQueue.Count==0)return;int last=_volumeProbeQueue.Count-1;var type=_volumeProbeQueue[last];_volumeProbeQueue.RemoveAt(last);
EnsureVolumeKnown(type,null);if(VOL_PER_UNIT.ContainsKey(type))_volumesMeasured++;if(_volumeProbeQueue.Count==0){if(_volumesMeasured>0)
Log($"Measured {_volumesMeasured} item volume(s)");_volumesMeasured=0;if(_discoveredDirty)SaveDiscovered();}}void CmdScan()
{Log("=== SCAN ===");if(!_discoverEnabled){Log("Discovery off ([Modded] Discover=false).");return;}var sources=
CollectSourceInventories();if(sources.Count==0){Log("Not docked - nothing to scan.");return;}int found=DiscoverFromSources(sources);if(found>0)
Log($"Added {found} modded item(s) to config.");else Log("No new items found.");}bool StationStocks(MyItemType type){List<
IMyInventory>holders;return _ammoSources.TryGetValue(type,out holders)&&holders.Count>0;}void AutoFillWeapons(List<IMyInventory>
sources){bool anyRoom=false;for(int w=0;w<_weapons.Count&&!anyRoom;w++){if(!_weapons[w].HasInventory)continue;var wInv=_weapons
[w].GetInventory(0);if(wInv==null)continue;if((float)(wInv.MaxVolume-wInv.CurrentVolume)*1000f>=MIN_TRANSFER_LITRES)
anyRoom=true;}if(!anyRoom)return;foreach(var holders in _ammoSources.Values)holders.Clear();for(int si=0;si<sources.Count;si++)
{var src=sources[si];_tempItems.Clear();src.GetItems(_tempItems);for(int i=0;i<_tempItems.Count;i++){var aType=_tempItems
[i].Type;if(aType.TypeId!=AMMO_TYPE_ID)continue;List<IMyInventory>holders;if(!_ammoSources.TryGetValue(aType,out holders)
){holders=new List<IMyInventory>();_ammoSources[aType]=holders;}if(holders.Count==0||holders[holders.Count-1]!=src)
holders.Add(src);}}foreach(var weapon in _weapons){if(!weapon.HasInventory)continue;var inv=weapon.GetInventory(0);if(inv==null
)continue;var accepted=AcceptedAmmoFor(weapon);if(accepted.Count==0)continue;MyItemType chosen=default(MyItemType);bool
haveChoice=false;_tempItems.Clear();inv.GetItems(_tempItems);for(int i=0;i<_tempItems.Count&&!haveChoice;i++){for(int a=0;a<
accepted.Count;a++){if(_tempItems[i].Type!=accepted[a])continue;chosen=accepted[a];haveChoice=true;break;}}for(int i=0;i<
accepted.Count&&!haveChoice;i++){if(!StationStocks(accepted[i]))continue;chosen=accepted[i];haveChoice=true;}if(!haveChoice)
continue;List<IMyInventory>ammoSrc;if(!_ammoSources.TryGetValue(chosen,out ammoSrc)||ammoSrc.Count==0)continue;if(!KEY_OF.
ContainsKey(chosen))RegisterDiscoveredType(chosen,0f);EnsureVolumeKnown(chosen,inv);float freeL=(float)(inv.MaxVolume-inv.
CurrentVolume)*1000f;if(freeL<MIN_TRANSFER_LITRES)continue;float units=FromLitres(chosen,freeL);if(units<1f)continue;float beforeL=(
float)inv.CurrentVolume*1000f;var moved=PullItemFromAny(inv,chosen,(MyFixedPoint)units,ammoSrc);if(moved>0){float afterL=(
float)inv.CurrentVolume*1000f;LearnVolume(chosen,afterL-beforeL,(float)moved);Log(
$" Ammo {Trunc(weapon.CustomName,12)} x{(float)moved:F0}");}}}void BalanceInventory(){DebugPhase("srcScan");var remoteInvs=CollectSourceInventories();if(remoteInvs.Count==0){Log
("No station");return;}Log($"Src: {remoteInvs.Count}");_dbgSrc=remoteInvs.Count;DebugPhase("weapons");AutoFillWeapons(
remoteInvs);DebugPhase("balance");bool slEnabled;float slMargin;string slGrav;ReadSmartLoadConfig(out slEnabled,out slMargin,out
slGrav);float maxCargoMass=float.MaxValue;float slBaseMass=0f;float slCurrentCargo=0f;float slUpThrust=0f;float slGravMag=0f;
bool slAtLimit=false;float slEstimatedCargo=0f;float slStartVolL=0f;float slDensity=10f;float slFuelMass=0f;if(slEnabled&&
_controllers.Count>0){maxCargoMass=CalcMaxCargoMass(out slBaseMass,out slCurrentCargo,out slUpThrust,out slGravMag,out slFuelMass);
float realCargoMass=0f;float realCargoVolL=0f;foreach(var cb in _allContainers){if(!cb.HasInventory)continue;var cbi=cb.
GetInventory(0);realCargoMass+=(float)cbi.CurrentMass;realCargoVolL+=(float)cbi.CurrentVolume*1000f;}slEstimatedCargo=realCargoMass;
slStartVolL=realCargoVolL;slCurrentCargo=realCargoMass;if(maxCargoMass<float.MaxValue&&slEstimatedCargo>=maxCargoMass)slAtLimit=
true;if(realCargoMass>0f&&realCargoVolL>0f)slDensity=realCargoMass/realCargoVolL;}var enabled=new Dictionary<MyItemType,
float>();var targets=new Dictionary<MyItemType,float>();foreach(var block in AllConfigurableBlocks()){if(!block.HasInventory)
continue;if(block is IMyUserControllableGun)continue;_ini.Clear();if(!_ini.TryParse(block.CustomData,out _iniResult)||!_ini.
ContainsSection(INI_SECTION))continue;ReadEnabledItems(block,enabled);var inv=block.GetInventory(0);if(inv==null)continue;float maxL=(
float)inv.MaxVolume*1000f;string bName=Trunc(block.CustomName,20);if(enabled.Count>0){ComputeTargets(enabled,maxL,targets);
var held=SnapshotInventory(inv);foreach(var kv in targets){var itemType=kv.Key;float targetL=kv.Value;MyFixedPoint heldAmt;
held.TryGetValue(itemType,out heldAmt);float heldL=ToLitres(itemType,(float)heldAmt);float diffL=heldL-targetL;if(diffL>
MIN_TRANSFER_LITRES){float excessUnits=FromLitres(itemType,diffL);var moved=PushItemToAny(inv,itemType,(MyFixedPoint)excessUnits,remoteInvs
);if(moved>0){Log($" {bName}");Log($"  PUSH {Trunc(KeyOf(itemType),10)} x{(float)moved:F0}");}}}}_tempItems.Clear();inv.
GetItems(_tempItems);var toPush=new List<MyItemType>();for(int i=_tempItems.Count-1;i>=0;i--){if(!enabled.ContainsKey(_tempItems
[i].Type)&&!toPush.Contains(_tempItems[i].Type))toPush.Add(_tempItems[i].Type);}foreach(var pushType in toPush){var moved
=PushItemToAny(inv,pushType,MyFixedPoint.MaxValue,remoteInvs);if(moved>0){Log($" {bName}");Log(
$"  EJECT {Trunc(pushType.SubtypeId,10)} x{(float)moved:F0}");}}if(enabled.Count>0&&!slAtLimit){var held=SnapshotInventory(inv);foreach(var kv in targets){var itemType=kv.Key;float
targetL=kv.Value;MyFixedPoint heldAmt;held.TryGetValue(itemType,out heldAmt);float heldL=ToLitres(itemType,(float)heldAmt);
float diffL=targetL-heldL;if(diffL>MIN_TRANSFER_LITRES){float neededUnits=FromLitres(itemType,diffL);float pullUnits=
neededUnits;if(slEnabled&&maxCargoMass<float.MaxValue){float remainingBudget=maxCargoMass-slEstimatedCargo;if(remainingBudget<=0f){
slAtLimit=true;break;}float maxPullL=remainingBudget/slDensity;float maxPullUnits=FromLitres(itemType,maxPullL);if(maxPullUnits<
pullUnits)pullUnits=maxPullUnits;if(maxPullUnits<1f){slAtLimit=true;break;}if(pullUnits<1f)continue;}var moved=PullItemFromAny(
inv,itemType,(MyFixedPoint)pullUnits,remoteInvs);if(slEnabled&&maxCargoMass<float.MaxValue&&moved>0)slEstimatedCargo+=
ToLitres(itemType,(float)moved)*slDensity;if(moved>0){Log($" {bName}");Log(
$"  PULL {Trunc(KeyOf(itemType),10)} x{(float)moved:F0}");}else if(neededUnits>1f){bool stationHas=false;foreach(var src in remoteInvs){if(src.GetItemAmount(itemType)>0){
stationHas=true;break;}}if(stationHas)Log($" Conveyor: {Trunc(KeyOf(itemType),12)}");else Log(
$" No stock: {Trunc(KeyOf(itemType),12)}");}}}}float curL=(float)inv.CurrentVolume*1000f;float pct=maxL>0f?curL/maxL*100f:0f;Log($" {bName} {pct:F0}%");}if(
slEnabled&&maxCargoMass<float.MaxValue&&slEstimatedCargo>maxCargoMass){float excessKg=slEstimatedCargo-maxCargoMass;float pushL=
excessKg/slDensity;Log($"SL-FIX {(int)slEstimatedCargo}>{(int)maxCargoMass} push:{(int)pushL}L");foreach(var block in
AllConfigurableBlocks()){if(!block.HasInventory)continue;var inv=block.GetInventory(0);if(inv==null||(float)inv.CurrentVolume<0.001f)continue
;_tempItems.Clear();inv.GetItems(_tempItems);if(_tempItems.Count==0)continue;var item=_tempItems[_tempItems.Count-1];
float pushUnits=FromLitres(item.Type,pushL);if(pushUnits<1f)pushUnits=1f;if(pushUnits>(float)item.Amount)pushUnits=(float)
item.Amount;var slMoved=PushItemToAny(inv,item.Type,(MyFixedPoint)pushUnits,remoteInvs);Log(
$" pushed:{(float)slMoved:F0} of {pushUnits:F0} dests:{remoteInvs.Count}");break;}}}List<IMyInventory>CollectSourceInventories(){var result=new List<IMyInventory>();var seenBlocks=new HashSet<
long>();foreach(var connector in _connectors){if(connector.Status!=MyShipConnectorStatus.Connected)continue;if(!
IsOwnConnector(connector))continue;var partner=connector.OtherConnector;if(partner==null)continue;if(partner.IsSameConstructAs(Me))
continue;_tempBlocks.Clear();GridTerminalSystem.GetBlocksOfType(_tempBlocks,b=>b.HasInventory&&b.IsSameConstructAs(partner)&&!b.
IsSameConstructAs(Me)&&b.CustomData.IndexOf("[AutoLoad]",StringComparison.OrdinalIgnoreCase)<0);AddBlockInventories(result,seenBlocks);}
foreach(var mechGridId in _mechDockGrids){_tempBlocks.Clear();GridTerminalSystem.GetBlocksOfType(_tempBlocks,b=>b.CubeGrid.
EntityId==mechGridId&&b.HasInventory&&b.CustomData.IndexOf("[AutoLoad]",StringComparison.OrdinalIgnoreCase)<0);
AddBlockInventories(result,seenBlocks);}return result;}void AddBlockInventories(List<IMyInventory>result,HashSet<long>seenBlocks){foreach(
var b in _tempBlocks){if(!seenBlocks.Add(b.EntityId))continue;for(int i=0;i<b.InventoryCount;i++){var inv=b.GetInventory(i)
;if(inv!=null)result.Add(inv);}}}private static Dictionary<MyItemType,MyFixedPoint>BuildStockSnapshot(List<IMyInventory>
inventories){var snap=new Dictionary<MyItemType,MyFixedPoint>();var items=new List<MyInventoryItem>();foreach(var inv in
inventories){items.Clear();inv.GetItems(items);foreach(var it in items){MyFixedPoint existing;snap.TryGetValue(it.Type,out existing
);snap[it.Type]=existing+it.Amount;}}return snap;}private static Dictionary<MyItemType,MyFixedPoint>SnapshotInventory(
IMyInventory inv){var snap=new Dictionary<MyItemType,MyFixedPoint>();var items=new List<MyInventoryItem>();inv.GetItems(items);
foreach(var it in items){MyFixedPoint existing;snap.TryGetValue(it.Type,out existing);snap[it.Type]=existing+it.Amount;}return
snap;}float ToLitres(MyItemType type,float amount){float v;return VOL_PER_UNIT.TryGetValue(type,out v)?amount*v:amount;}
float FromLitres(MyItemType type,float litres){float v;return(VOL_PER_UNIT.TryGetValue(type,out v)&&v>0f)?litres/v:litres;}
string KeyOf(MyItemType type){string key;return KEY_OF.TryGetValue(type,out key)?key:type.SubtypeId;}void DebugBegin(){if(
_debugSurfaces.Count==0)return;if(Runtime.LastRunTimeMs>_dbgWorstMs)_dbgWorstMs=Runtime.LastRunTimeMs;_dbgTick++;_dbgSb.Clear();
_dbgPhase="Caches";_dbgLastInstr=0;for(int i=0;i<_debugSurfaces.Count;i++){var surf=_debugSurfaces[i];surf.ContentType=
ContentType.TEXT_AND_IMAGE;surf.Font="Monospace";surf.FontSize=0.5f;}}void DebugPhase(string name){if(_debugSurfaces.Count==0)
return;int now=Runtime.CurrentInstructionCount;if(_dbgPhase.Length>0)_dbgSb.Append(' ').Append(_dbgPhase.PadRight(9)).Append(
now-_dbgLastInstr).Append('\n');_dbgPhase=name;_dbgLastInstr=now;DebugRender(">"+name+" ...");}void DebugEnd(){if(
_debugSurfaces.Count==0)return;int now=Runtime.CurrentInstructionCount;if(_dbgPhase.Length>0)_dbgSb.Append(' ').Append(_dbgPhase.
PadRight(9)).Append(now-_dbgLastInstr).Append('\n');_dbgPhase="";_dbgLastInstr=now;if(now>_dbgWorstInstr){_dbgWorstInstr=now;
_dbgWorstTick=_dbgTick;_dbgWorstText=_dbgSb.ToString();}DebugRender("done "+now);}void DebugRender(string tail){int now=Runtime.
CurrentInstructionCount;_dbgOut.Clear();_dbgOut.Append("AutoLoad DEBUG  #").Append(_dbgTick).Append('\n');_dbgOut.Append("instr ").Append(now).
Append(" / ").Append(Runtime.MaxInstructionCount).Append('\n');_dbgOut.Append("prev run ").Append(Runtime.LastRunTimeMs.
ToString("F1")).Append(" ms\n");_dbgOut.Append("src ").Append(_dbgSrc).Append("  guns ").Append(_weapons.Count).Append(
"  cargo ").Append(_containers.Count).Append('\n');_dbgOut.Append("===================\n");_dbgOut.Append(_dbgSb);_dbgOut.Append(
tail).Append('\n');if(_dbgWorstInstr>0){_dbgOut.Append("=== worst tick #").Append(_dbgWorstTick).Append("  ").Append(
_dbgWorstInstr).Append(" instr").Append("  peak ").Append(_dbgWorstMs.ToString("F1")).Append(" ms ===\n");_dbgOut.Append(_dbgWorstText
);}string text=_dbgOut.ToString();for(int i=0;i<_debugSurfaces.Count;i++)_debugSurfaces[i].WriteText(text);}void Log(
string line){_log.Add(line);Echo(line);}void WriteDisplays(bool fromCommand,bool withCargo){BindIndicatorLights();bool
wantBlocks=_surfaces.Count>0&&withCargo;bool wantShip=(_totalSurfaces.Count>0||_watchLights.Count>0)&&withCargo;if(wantBlocks||
wantShip)CollectShipTotals(wantBlocks?_blockSb:null);if(wantBlocks){_sb.Clear();_sb.AppendLine(IsDocked()?"AutoLoad DOCKED":
"AutoLoad");AppendSmartLoadLine(_sb);if(_gasTanks.Count>0)_sb.AppendLine(BuildFuelLine());_sb.Append(_blockSb.ToString());if(
_hiddenContainers.Count>0)_sb.AppendLine($"({_hiddenContainers.Count} hidden: no conveyor)");float overallPct=_shipMaxL>0f?_shipCurL/
_shipMaxL:0f;_sb.AppendLine($"{FillBar(overallPct,10)} {overallPct*100f:F0}%");string text=_sb.ToString();for(int i=0;i<_surfaces
.Count;i++){var surface=_surfaces[i];surface.ContentType=ContentType.TEXT_AND_IMAGE;surface.Font="Monospace";surface.
WriteText(text,false);}}if(fromCommand)_logHoldCycles=LOG_HOLD_CYCLES;else if(_logHoldCycles>0)_logHoldCycles--;if(_log.Count>0){
_sb.Clear();foreach(var l in _log)_sb.AppendLine(l);_lastLogEcho=_sb.ToString();}if(_logSurfaces.Count>0&&_log.Count>0&&(
fromCommand||_logHoldCycles==0)){_sb.Clear();_sb.AppendLine("== AutoLoad Log ==");_sb.Append(_lastLogEcho);string logText=_sb.
ToString();for(int i=0;i<_logSurfaces.Count;i++){var surface=_logSurfaces[i];surface.ContentType=ContentType.TEXT_AND_IMAGE;
surface.Font="Monospace";surface.WriteText(logText,false);}}WriteMenu();if(_totalSurfaces.Count>0)WriteTotals();if(_watchLights
.Count>0)ApplyIndicatorLights();}void AppendSmartLoadLine(StringBuilder sb){if(_controllers.Count==0)return;bool slEn;
float slMar;string slGr;ReadSmartLoadConfig(out slEn,out slMar,out slGr);float bm,cc,ut,gm,fm;float maxCargo=CalcMaxCargoMass
(out bm,out cc,out ut,out gm,out fm);if(maxCargo>=float.MaxValue)return;if(slEn)sb.AppendLine("SL "+(int)cc+"/"+(int)
maxCargo+"kg");else if(cc>maxCargo)sb.AppendLine("!! OVERWEIGHT !!");}string BuildFuelLine(){float h2Pct=0f;int h2Cnt=0;float
o2Pct=0f;int o2Cnt=0;foreach(var tank in _gasTanks){if(tank.BlockDefinition.SubtypeId.IndexOf("Hydrogen",StringComparison.
OrdinalIgnoreCase)>=0){h2Pct+=(float)tank.FilledRatio;h2Cnt++;}else{o2Pct+=(float)tank.FilledRatio;o2Cnt++;}}string line="";if(h2Cnt>0)
line+="H2:"+(int)(h2Pct/h2Cnt*100)+"%";if(h2Cnt>0&&o2Cnt>0)line+=" ";if(o2Cnt>0)line+="O2:"+(int)(o2Pct/o2Cnt*100)+"%";
return line;}struct StockRow{public string Name;public float Held,Target,Frac;}void CollectShipTotals(StringBuilder perBlock){
_shipHeldUnits.Clear();_shipTargetL.Clear();_shipCurL=0f;_shipMaxL=0f;if(perBlock!=null)perBlock.Clear();var enabled=new Dictionary<
MyItemType,float>();var targets=new Dictionary<MyItemType,float>();foreach(var block in AllConfigurableBlocks()){if(!block.
HasInventory)continue;var inv=block.GetInventory(0);float maxL=(float)inv.MaxVolume*1000f;float curL=(float)inv.CurrentVolume*1000f;
_shipCurL+=curL;_shipMaxL+=maxL;_blockHeld.Clear();_tempItems.Clear();inv.GetItems(_tempItems);for(int i=0;i<_tempItems.Count;i++
){var type=_tempItems[i].Type;float amt=(float)_tempItems[i].Amount;float have;_blockHeld.TryGetValue(type,out have);
_blockHeld[type]=have+amt;_shipHeldUnits.TryGetValue(type,out have);_shipHeldUnits[type]=have+amt;}if(perBlock!=null){float bPct=
maxL>0f?curL/maxL*100f:0f;perBlock.AppendLine($"{Trunc(block.CustomName,14)} {bPct:F0}%");}if(!ReadEnabledItems(block,
enabled))continue;ComputeTargets(enabled,maxL,targets);foreach(var kv in targets){float t;_shipTargetL.TryGetValue(kv.Key,out t
);_shipTargetL[kv.Key]=t+kv.Value;if(perBlock==null)continue;float heldU;_blockHeld.TryGetValue(kv.Key,out heldU);float
heldL=ToLitres(kv.Key,heldU);float iPct=kv.Value>0f?Math.Min(heldL/kv.Value,1f):0f;perBlock.AppendLine(
$" {FillBar(iPct,8)} {Trunc(KeyOf(kv.Key),8)}");}}}void WriteTotals(){bool docked=IsDocked();var colHeader=new Color(30,144,255);var colWhite=new Color(255,255,255);
var colGreen=new Color(50,205,50);var colYellow=new Color(255,200,0);var colRed=new Color(220,50,50);var colGray=new Color(
120,120,120);var colBarBg=new Color(40,40,40);var colRowFull=new Color(30,110,40);var colRowLow=new Color(150,110,0);var
colRowNone=new Color(140,35,35);var stocked=new List<StockRow>();foreach(var kv in _shipTargetL){float heldU;_shipHeldUnits.
TryGetValue(kv.Key,out heldU);float tgtU=FromLitres(kv.Key,kv.Value);var row=new StockRow();row.Name=KeyOf(kv.Key);row.Held=heldU;
row.Target=tgtU;row.Frac=tgtU>0f?Math.Min(heldU/tgtU,1f):0f;stocked.Add(row);}stocked.Sort((a,b)=>a.Frac!=b.Frac?a.Frac.
CompareTo(b.Frac):string.Compare(a.Name,b.Name,StringComparison.OrdinalIgnoreCase));var other=new List<StockRow>();foreach(var kv
in _shipHeldUnits){if(kv.Value<=0f)continue;if(_shipTargetL.ContainsKey(kv.Key))continue;var row=new StockRow();row.Name=
KeyOf(kv.Key);row.Held=kv.Value;other.Add(row);}other.Sort((a,b)=>b.Held.CompareTo(a.Held));float overall=_shipMaxL>0f?
_shipCurL/_shipMaxL:0f;for(int si=0;si<_totalSurfaces.Count;si++){var surface=_totalSurfaces[si];surface.ContentType=ContentType.
SCRIPT;surface.Script="";surface.ScriptBackgroundColor=Color.Black;float scale=si<_totalScales.Count?_totalScales[si]:1f;var
frame=surface.DrawFrame();float vpX=(surface.TextureSize.X-surface.SurfaceSize.X)/2f;float vpY=(surface.TextureSize.Y-surface
.SurfaceSize.Y)/2f;float w=surface.SurfaceSize.X;float pad=10f*scale;float lineH=22f*scale;float fontSize=0.6f*scale;
float y=vpY;float lx=vpX+pad;float cx=vpX+w/2f;float rx=vpX+w-pad;float rowW=w-pad*2f;float maxY=vpY+surface.SurfaceSize.Y-
pad;frame.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",new Vector2(cx,y+lineH/2f),new Vector2(w,lineH),docked?
colGreen:colHeader));frame.Add(new MySprite(SpriteType.TEXT,docked?"SHIP CARGO - DOCKED":"SHIP CARGO",new Vector2(cx,y),null,
colWhite,"Monospace",TextAlignment.CENTER,fontSize));y+=lineH+4f*scale;float barH=14f*scale;frame.Add(new MySprite(SpriteType.
TEXTURE,"SquareSimple",new Vector2(cx,y+barH/2f),new Vector2(rowW,barH),colBarBg));float fillW=rowW*Math.Max(0f,Math.Min(1f,
overall));Color overallCol=overall<0.6f?colGreen:(overall<0.85f?colYellow:colRed);if(fillW>0f)frame.Add(new MySprite(SpriteType
.TEXTURE,"SquareSimple",new Vector2(lx+fillW/2f,y+barH/2f),new Vector2(fillW,barH),overallCol));y+=barH+3f*scale;frame.
Add(new MySprite(SpriteType.TEXT,FormatQty(_shipCurL)+" / "+FormatQty(_shipMaxL)+" L",new Vector2(lx,y),null,colWhite,
"Monospace",TextAlignment.LEFT,fontSize));frame.Add(new MySprite(SpriteType.TEXT,((int)(overall*100f))+"%",new Vector2(rx,y),null,
colWhite,"Monospace",TextAlignment.RIGHT,fontSize));y+=lineH;_sb.Clear();AppendSmartLoadLine(_sb);string slText=_sb.ToString().
Trim();if(slText.Length>0){frame.Add(new MySprite(SpriteType.TEXT,slText,new Vector2(lx,y),null,slText.IndexOf("OVERWEIGHT",
StringComparison.OrdinalIgnoreCase)>=0?colRed:colWhite,"Monospace",TextAlignment.LEFT,fontSize));y+=lineH;}if(_gasTanks.Count>0){string
fuel=BuildFuelLine();if(fuel.Length>0){frame.Add(new MySprite(SpriteType.TEXT,fuel,new Vector2(lx,y),null,colWhite,
"Monospace",TextAlignment.LEFT,fontSize));y+=lineH;}}if(stocked.Count>0&&y+lineH*2f<maxY){y=DrawSectionLabel(frame,"STOCKED",lx,cx,
y,rowW,scale,fontSize,colGray);for(int i=0;i<stocked.Count;i++){if(y+lineH>maxY)break;int left=stocked.Count-i;if(y+lineH
*2f>maxY&&left>1){frame.Add(new MySprite(SpriteType.TEXT,"+"+left+" more",new Vector2(lx,y),null,colGray,"Monospace",
TextAlignment.LEFT,fontSize*0.8f));y+=lineH;break;}var r=stocked[i];int state=StockState(r.Held,r.Target);Color rowCol=state==0?
colRowFull:(state==1?colRowLow:colRowNone);frame.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",new Vector2(cx,y+lineH/2f),new
Vector2(rowW,lineH-2f*scale),colBarBg));float rw=rowW*r.Frac;if(rw>0f)frame.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",
new Vector2(lx+rw/2f,y+lineH/2f),new Vector2(rw,lineH-2f*scale),rowCol));frame.Add(new MySprite(SpriteType.TEXT,Trunc(r.
Name,14),new Vector2(lx+4f*scale,y),null,colWhite,"Monospace",TextAlignment.LEFT,fontSize));frame.Add(new MySprite(
SpriteType.TEXT,FormatQty(r.Held)+"/"+FormatQty(r.Target),new Vector2(rx-4f*scale,y),null,colWhite,"Monospace",TextAlignment.RIGHT
,fontSize));y+=lineH;}}if(other.Count>0&&y+lineH*2f<maxY){y=DrawSectionLabel(frame,"ALSO ABOARD",lx,cx,y,rowW,scale,
fontSize,colGray);for(int i=0;i<other.Count;i++){if(y+lineH>maxY)break;int left=other.Count-i;if(y+lineH*2f>maxY&&left>1){frame.
Add(new MySprite(SpriteType.TEXT,"+"+left+" more",new Vector2(lx,y),null,colGray,"Monospace",TextAlignment.LEFT,fontSize*
0.8f));y+=lineH;break;}frame.Add(new MySprite(SpriteType.TEXT,Trunc(other[i].Name,14),new Vector2(lx,y),null,colGray,
"Monospace",TextAlignment.LEFT,fontSize));frame.Add(new MySprite(SpriteType.TEXT,FormatQty(other[i].Held),new Vector2(rx,y),null,
colGray,"Monospace",TextAlignment.RIGHT,fontSize));y+=lineH;}}if(_hiddenContainers.Count>0&&y+lineH<=maxY)frame.Add(new
MySprite(SpriteType.TEXT,_hiddenContainers.Count+" hidden: no conveyor",new Vector2(lx,y),null,colRed,"Monospace",TextAlignment.
LEFT,fontSize*0.8f));frame.Dispose();}}float DrawSectionLabel(MySpriteDrawFrame frame,string label,float lx,float cx,float y
,float rowW,float scale,float fontSize,Color col){frame.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",new Vector2(cx
,y+2f*scale),new Vector2(rowW,2f*scale),col));y+=5f*scale;frame.Add(new MySprite(SpriteType.TEXT,label,new Vector2(lx,y),
null,col,"Monospace",TextAlignment.LEFT,fontSize*0.8f));return y+18f*scale;}private static readonly Color LIGHT_FULL=new
Color(0,255,0),LIGHT_SHORT=new Color(255,170,0),LIGHT_MISSING=new Color(255,0,0);void RefreshLightTags(){_lightItem.Clear();
_lightOverall.Clear();_indicatorLights.Clear();_badLightItems.Clear();foreach(var light in _lights){string itemKey;if(!ParseLightTag(
light.CustomName,light.CustomData,out itemKey))continue;_indicatorLights.Add(light.EntityId);if(itemKey.Length==0){
_lightOverall.Add(light.EntityId);continue;}MyItemType type;if(!ALL_ITEMS.TryGetValue(itemKey,out type)){_badLightItems.Add(light.
CustomName+" -> "+itemKey);continue;}_lightItem[light.EntityId]=type;}}void BindIndicatorLights(){_watchLights.Clear();_watchTypes
.Clear();_watchOverall.Clear();if(_indicatorLights.Count==0)return;foreach(var light in _lights){long id=light.EntityId;
if(!_indicatorLights.Contains(id))continue;if(_lightOverall.Contains(id)){_watchLights.Add(light);_watchTypes.Add(default(
MyItemType));_watchOverall.Add(true);continue;}MyItemType type;if(!_lightItem.TryGetValue(id,out type))continue;_watchLights.Add(
light);_watchTypes.Add(type);_watchOverall.Add(false);}}void ApplyIndicatorLights(){int overall=-1;for(int i=0;i<_watchLights
.Count;i++){int state;if(_watchOverall[i]){if(overall<0)overall=OverallStockState();state=overall;}else{var type=
_watchTypes[i];float heldU,tgtL;_shipHeldUnits.TryGetValue(type,out heldU);_shipTargetL.TryGetValue(type,out tgtL);state=StockState
(heldU,FromLitres(type,tgtL));}ApplyLightState(_watchLights[i],state);}}int OverallStockState(){int worst=0;foreach(var
kv in _shipTargetL){float heldU;_shipHeldUnits.TryGetValue(kv.Key,out heldU);int s=StockState(heldU,FromLitres(kv.Key,kv.
Value));if(s>worst)worst=s;if(worst==2)break;}return worst;}void ApplyLightState(IMyLightingBlock light,int state){Color want
=state==0?LIGHT_FULL:state==1?LIGHT_SHORT:LIGHT_MISSING;float blink=state==2?1f:0f;if(!light.Enabled)light.Enabled=true;
if(light.Color!=want)light.Color=want;if(Math.Abs(light.BlinkIntervalSeconds-blink)>0.01f)light.BlinkIntervalSeconds=blink
;if(state==2&&Math.Abs(light.BlinkLength-50f)>0.01f)light.BlinkLength=50f;}public static int StockState(float heldUnits,
float targetUnits){if(heldUnits<=0.0001f)return 2;if(targetUnits<=0f)return 0;return heldUnits>=targetUnits*0.99f?0:1;}public
static string FormatQty(float value){float mag=Math.Abs(value);if(mag>=1000000f)return(value/1000000f).ToString("0.#")+"M";if(
mag>=1000f)return(value/1000f).ToString("0.#")+"k";if(mag>=10f)return((int)value).ToString();return value.ToString("0.#");}
public static bool ParseLightTag(string blockName,string customData,out string itemKey){if(TryReadLightTag(customData,
LIGHT_TAG,out itemKey))return true;return TryReadLightTag(blockName,"["+LIGHT_TAG,out itemKey);}private static bool
TryReadLightTag(string src,string tag,out string itemKey){itemKey="";if(string.IsNullOrEmpty(src))return false;int at=0;while(at<src.
Length){int idx=src.IndexOf(tag,at,StringComparison.OrdinalIgnoreCase);if(idx<0)return false;int pos=idx+tag.Length;if(pos<src
.Length&&(char.IsLetterOrDigit(src[pos])||src[pos]=='_')){at=idx+1;continue;}if(pos<src.Length&&src[pos]==':'){int end=
pos+1;while(end<src.Length&&src[end]!=']'&&src[end]!='\n'&&src[end]!='\r')end++;itemKey=src.Substring(pos+1,end-pos-1).Trim
();}return true;}return false;}void GetMenuData(out List<string>names,out string slLine,out string planetLine,out string
fuelLine,out bool docked){names=ListLoadoutNames();names.Insert(0,"Off");names.Add("Configure");slLine="";planetLine="";fuelLine
="";docked=IsDocked();bool slEn;float slMar;string slGr;ReadSmartLoadConfig(out slEn,out slMar,out slGr);if(slEn&&
_controllers.Count>0){float bm,cc,ut,gm,fm;float maxCargo=CalcMaxCargoMass(out bm,out cc,out ut,out gm,out fm);if(maxCargo<float.
MaxValue){slLine=(int)cc+"/"+(int)maxCargo+" kg";float gForce=gm/9.81f;planetLine=GetPlanetName(gForce)+" ("+FormatG(gForce)+")"
;}else{slLine=IsGroundVehicle()?"Rover - no limit":"no limit";float gForceNl=_controllers[0].GetNaturalGravity().Length()
>0.01?(float)_controllers[0].GetNaturalGravity().Length()/9.81f:0f;planetLine=GetPlanetName(gForceNl)+" ("+FormatG(
gForceNl)+")";}}if(_gasTanks.Count>0)fuelLine=BuildFuelLine();}void WriteMenu(){if(_menuSurfaces.Count==0)return;List<string>
names;string slLine,planetLine,fuelLine;bool docked;GetMenuData(out names,out slLine,out planetLine,out fuelLine,out docked);
if(names.Count>0){if(_menuCursor>=names.Count)_menuCursor=names.Count-1;if(_menuCursor<0)_menuCursor=0;}var colHeader=new
Color(30,144,255);var colWhite=new Color(255,255,255);var colGreen=new Color(50,205,50);var colRed=new Color(220,50,50);var
colYellow=new Color(255,200,0);var colGray=new Color(120,120,120);var colBarBg=new Color(40,40,40);var colHighlight=new Color(50,
50,80);for(int si=0;si<_menuSurfaces.Count;si++){var surface=_menuSurfaces[si];surface.ContentType=ContentType.SCRIPT;
surface.Script="";surface.ScriptBackgroundColor=Color.Black;float scale=si<_menuScales.Count?_menuScales[si]:1f;var frame=
surface.DrawFrame();float vpX=(surface.TextureSize.X-surface.SurfaceSize.X)/2f;float vpY=(surface.TextureSize.Y-surface.
SurfaceSize.Y)/2f;float w=surface.SurfaceSize.X;float pad=10f*scale;float lineH=22f*scale;float fontSize=0.6f*scale;float y=vpY;
float lx=vpX+pad;float cx=vpX+w/2f;float rx=vpX+w-pad;if(_calState==1){frame.Add(new MySprite(SpriteType.TEXTURE,
"SquareSimple",new Vector2(cx,y+lineH/2f),new Vector2(w,lineH),colHeader));frame.Add(new MySprite(SpriteType.TEXT,"SMARTLOAD SETUP",
new Vector2(cx,y),null,colWhite,"Monospace",TextAlignment.CENTER,fontSize));y+=lineH+6f*scale;frame.Add(new MySprite(
SpriteType.TEXT,"1. Empty all cargo",new Vector2(lx,y),null,colWhite,"Monospace",TextAlignment.LEFT,fontSize));y+=lineH;frame.Add(
new MySprite(SpriteType.TEXT,"2. Undock ship",new Vector2(lx,y),null,colWhite,"Monospace",TextAlignment.LEFT,fontSize));y+=
lineH;frame.Add(new MySprite(SpriteType.TEXT,"3. Fly to open area",new Vector2(lx,y),null,colWhite,"Monospace",TextAlignment.
LEFT,fontSize));y+=lineH*1.5f;frame.Add(new MySprite(SpriteType.TEXT,"Run 'continue'",new Vector2(lx,y),null,colYellow,
"Monospace",TextAlignment.LEFT,fontSize));}else if(_calState>=2&&_calState<=6){int dirIdx=_calState-2;frame.Add(new MySprite(
SpriteType.TEXTURE,"SquareSimple",new Vector2(cx,y+lineH/2f),new Vector2(w,lineH),colHeader));frame.Add(new MySprite(SpriteType.
TEXT,"CALIBRATING",new Vector2(cx,y),null,colWhite,"Monospace",TextAlignment.CENTER,fontSize));y+=lineH+6f*scale;for(int d=0
;d<dirIdx;d++){frame.Add(new MySprite(SpriteType.TEXT,CAL_DIR_NAMES[d]+" "+((int)_calResults[d])+"N",new Vector2(lx,y),
null,colGreen,"Monospace",TextAlignment.LEFT,fontSize));y+=lineH;}y+=lineH*0.5f;frame.Add(new MySprite(SpriteType.TEXT,
"Fly "+CAL_DIR_NAMES[dirIdx],new Vector2(lx,y),null,colYellow,"Monospace",TextAlignment.LEFT,fontSize));y+=lineH;if(
_calReadings>0)frame.Add(new MySprite(SpriteType.TEXT,"Capturing "+_calReadings+"/30",new Vector2(lx,y),null,colWhite,"Monospace",
TextAlignment.LEFT,fontSize));else frame.Add(new MySprite(SpriteType.TEXT,"Waiting...",new Vector2(lx,y),null,colGray,"Monospace",
TextAlignment.LEFT,fontSize));y+=lineH;frame.Add(new MySprite(SpriteType.TEXT,"[continue] to skip",new Vector2(lx,y),null,colGray,
"Monospace",TextAlignment.LEFT,fontSize*0.8f));}else if(_calState==7){frame.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",new
Vector2(cx,y+lineH/2f),new Vector2(w,lineH),colGreen));frame.Add(new MySprite(SpriteType.TEXT,"CALIBRATION DONE",new Vector2(cx
,y),null,colWhite,"Monospace",TextAlignment.CENTER,fontSize));y+=lineH+6f*scale;float calG=_calGravity/9.81f;frame.Add(
new MySprite(SpriteType.TEXT,GetPlanetName(calG)+" ("+FormatG(calG)+")",new Vector2(lx,y),null,colWhite,"Monospace",
TextAlignment.LEFT,fontSize));y+=lineH*1.5f;for(int d=0;d<5;d++){frame.Add(new MySprite(SpriteType.TEXT,CAL_DIR_NAMES[d]+" "+((int)
_calResults[d])+"N",new Vector2(lx,y),null,colWhite,"Monospace",TextAlignment.LEFT,fontSize));y+=lineH;}frame.Add(new MySprite(
SpriteType.TEXT,"Mass "+(int)_calBaseMass+"kg",new Vector2(lx,y),null,colWhite,"Monospace",TextAlignment.LEFT,fontSize));y+=lineH*
1.5f;frame.Add(new MySprite(SpriteType.TEXT,"Please wait...",new Vector2(lx,y),null,colGray,"Monospace",TextAlignment.LEFT,
fontSize));_calCompleteTicks++;if(_calCompleteTicks>30)_calState=0;}else{frame.Add(new MySprite(SpriteType.TEXTURE,
"SquareSimple",new Vector2(cx,y+lineH/2f),new Vector2(w,lineH),colHeader));frame.Add(new MySprite(SpriteType.TEXT,"LOADOUTS",new
Vector2(cx,y),null,colWhite,"Monospace",TextAlignment.CENTER,fontSize));y+=lineH+4f;for(int idx=0;idx<names.Count;idx++){bool
isCursor=(idx==_menuCursor);bool isActive=names[idx].Equals(_activeLoadout,StringComparison.OrdinalIgnoreCase);if(isCursor)frame
.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",new Vector2(cx,y+lineH/2f),new Vector2(w,lineH),colHighlight));string
label=(isCursor?"> ":"  ")+names[idx];if(isActive)label+=" *";Color textCol=isActive?colGreen:colWhite;frame.Add(new MySprite
(SpriteType.TEXT,label,new Vector2(lx,y),null,textCol,"Monospace",TextAlignment.LEFT,fontSize));if(isActive)frame.Add(new
MySprite(SpriteType.TEXT,"ACTIVE",new Vector2(rx,y),null,colGreen,"Monospace",TextAlignment.RIGHT,fontSize*0.8f));y+=lineH;}y+=
6f*scale;float barW=w-pad*2f;float barH=12f*scale;float cargoFill=0f;if(slLine.Length>0&&slLine!="no limit"){int slash=
slLine.IndexOf('/');if(slash>0){float cur,max;if(float.TryParse(slLine.Substring(0,slash),out cur)){string afterSlash=slLine.
Substring(slash+1).Replace(" kg","").Trim();if(float.TryParse(afterSlash,out max)&&max>0f)cargoFill=cur/max;}}}frame.Add(new
MySprite(SpriteType.TEXTURE,"SquareSimple",new Vector2(cx,y+barH/2f),new Vector2(barW,barH),colBarBg));Color barCol=cargoFill<
0.6f?colGreen:(cargoFill<0.85f?colYellow:colRed);float fillW=barW*Math.Max(0f,Math.Min(1f,cargoFill));if(fillW>0f)frame.Add(
new MySprite(SpriteType.TEXTURE,"SquareSimple",new Vector2(lx+fillW/2f,y+barH/2f),new Vector2(fillW,barH),barCol));y+=barH+
4f*scale;if(slLine.Length>0){frame.Add(new MySprite(SpriteType.TEXT,"SL "+slLine,new Vector2(lx,y),null,colWhite,
"Monospace",TextAlignment.LEFT,fontSize));y+=lineH;}if(planetLine.Length>0){frame.Add(new MySprite(SpriteType.TEXT,planetLine,new
Vector2(lx,y),null,colWhite,"Monospace",TextAlignment.LEFT,fontSize));y+=lineH;}if(fuelLine.Length>0){frame.Add(new MySprite(
SpriteType.TEXT,fuelLine,new Vector2(lx,y),null,colWhite,"Monospace",TextAlignment.LEFT,fontSize));y+=lineH;}Color dockCol=docked?
colGreen:colRed;frame.Add(new MySprite(SpriteType.TEXT,docked?"DOCKED":"UNDOCKED",new Vector2(lx,y),null,dockCol,"Monospace",
TextAlignment.LEFT,fontSize));y+=lineH;frame.Add(new MySprite(SpriteType.TEXT,"[up / dn / select]",new Vector2(lx,y),null,colGray,
"Monospace",TextAlignment.LEFT,fontSize*0.8f));}frame.Dispose();}}private static string FillBar(float fraction,int width){fraction=
Math.Max(0f,Math.Min(1f,fraction));int filled=(int)Math.Round(fraction*width);return"["+new string('|',filled)+new string(
' ',width-filled)+"]";}private static string Trunc(string s,int max){if(s==null)return"";return s.Length<=max?s:s.Substring
(0,max-1)+"~";}private static string FormatG(float gForce){return gForce.ToString("F2")+"g";}private static readonly
string[]PLANET_NAMES={"Earth","Moon","Mars","Europa","Triton","Pertam","Alien","Titan"};private static readonly Vector3D[]
PLANET_POS={new Vector3D(0.5,0.5,0.5),new Vector3D(16384.5,136384.5,-113615.5),new Vector3D(1031072.5,131072.5,1631072.5),new
Vector3D(916384.5,16384.5,1616384.5),new Vector3D(-284463.5,-2434463.5,365536.5),new Vector3D(-3967231.5,-32231.5,-767231.5),new
Vector3D(131072.5,131072.5,5731072.5),new Vector3D(36384.5,226384.5,5796384.5)};string GetPlanetName(float gForce){if(gForce<
0.01f)return"Space";if(_controllers.Count==0)return FormatG(gForce);Vector3D pos=_controllers[0].GetPosition();double
bestDist=double.MaxValue;string bestName=FormatG(gForce);for(int i=0;i<PLANET_NAMES.Length;i++){double dist=Vector3D.Distance(
pos,PLANET_POS[i]);if(dist<bestDist){bestDist=dist;bestName=PLANET_NAMES[i];}}return bestName;}void CollectScaledSurface(
string blockName,string customData,string tag,IMyTextSurfaceProvider provider,List<IMyTextSurface>surfaces,List<float>scales){
float scale=1f;int surfIdx=0;int dataIdx=customData.IndexOf(tag,StringComparison.OrdinalIgnoreCase);int nameIdx=blockName.
IndexOf("["+tag,StringComparison.OrdinalIgnoreCase);if(dataIdx<0&&nameIdx<0)return;string src=dataIdx>=0?customData:blockName;
int pos=(dataIdx>=0?dataIdx:nameIdx)+tag.Length;if(dataIdx<0&&nameIdx>=0)pos++;if(pos<src.Length&&src[pos]==':')pos++;int
numEnd=pos;while(numEnd<src.Length&&(char.IsDigit(src[numEnd])||src[numEnd]=='.'))numEnd++;if(numEnd>pos){string firstVal=src.
Substring(pos,numEnd-pos);if(numEnd<src.Length&&src[numEnd]==':'){int si;if(int.TryParse(firstVal,out si)&&si>=0)surfIdx=si;int
scaleStart=numEnd+1;int scaleEnd=scaleStart;while(scaleEnd<src.Length&&(char.IsDigit(src[scaleEnd])||src[scaleEnd]=='.'))scaleEnd
++;if(scaleEnd>scaleStart){float sv;if(float.TryParse(src.Substring(scaleStart,scaleEnd-scaleStart),out sv)&&sv>0f)scale=
sv;}}else{if(firstVal.IndexOf('.')>=0){float sv;if(float.TryParse(firstVal,out sv)&&sv>0f)scale=sv;}else{int si;if(int.
TryParse(firstVal,out si)&&si>=0)surfIdx=si;}}}if(surfIdx>=provider.SurfaceCount)surfIdx=0;surfaces.Add(provider.GetSurface(
surfIdx));scales.Add(scale);}void CollectNameTagSurface(string blockName,string tagPrefix,IMyTextSurfaceProvider provider,List<
IMyTextSurface>surfaces){int open=blockName.IndexOf(tagPrefix,StringComparison.OrdinalIgnoreCase);if(open<0)return;int close=blockName
.IndexOf(']',open);if(close<0)return;surfaces.Add(provider.GetSurface(0));}void CollectDataTagSurface(string customData,
string tag,IMyTextSurfaceProvider provider,List<IMyTextSurface>surfaces){string baseName=tag.TrimEnd(':');int tagStart=
customData.IndexOf(baseName,StringComparison.OrdinalIgnoreCase);if(tagStart<0)return;int pos=tagStart+baseName.Length;if(pos<
customData.Length&&char.IsLetter(customData[pos]))return;int surfIdx=0;if(pos<customData.Length&&customData[pos]==':')pos++;int
numEnd=pos;while(numEnd<customData.Length&&char.IsDigit(customData[numEnd]))numEnd++;if(numEnd>pos){int idx;if(int.TryParse(
customData.Substring(pos,numEnd-pos),out idx)&&idx>=0)surfIdx=idx;}if(surfIdx>=provider.SurfaceCount)surfIdx=0;surfaces.Add(
provider.GetSurface(surfIdx));}IEnumerable<IMyTerminalBlock>AllConfigurableBlocks(){foreach(var c in _containers)yield return c;
foreach(var t in _turrets)yield return t;}IEnumerable<IMyTerminalBlock>AllBlocksUnfiltered(){foreach(var c in _allContainers)
yield return c;foreach(var t in _turrets)yield return t;}public static string SanitizeItemKey(string subtypeId){if(subtypeId
==null)return"Item";var sb=new StringBuilder();for(int i=0;i<subtypeId.Length;i++){char c=subtypeId[i];bool ok=(c>='a'&&c
<='z')||(c>='A'&&c<='Z')||(c>='0'&&c<='9')||c=='_'||c=='-'||c=='.';if(ok)sb.Append(c);}return sb.Length>0?sb.ToString():
"Item";}public static string ShortTypeTag(string typeId){if(typeId==null)return"Item";if(typeId.EndsWith("AmmoMagazine",
StringComparison.OrdinalIgnoreCase))return"Ammo";if(typeId.EndsWith("_Ore",StringComparison.OrdinalIgnoreCase))return"Ore";if(typeId.
EndsWith("_Ingot",StringComparison.OrdinalIgnoreCase))return"Ingot";if(typeId.EndsWith("_Component",StringComparison.
OrdinalIgnoreCase))return"Comp";return"Item";}public static float ProbeVolumeLitres(float freeLitres,Func<int,bool>canAdd){if(freeLitres
<=0f)return 0f;if(!canAdd(1))return 0f;const int PROBE_MAX=1<<22;int lo=1;int hi=2;while(hi<=PROBE_MAX&&canAdd(hi)){lo=hi;
hi<<=1;}if(hi>PROBE_MAX)return freeLitres/PROBE_MAX;while(lo+1<hi){int mid=lo+(hi-lo)/2;if(canAdd(mid))lo=mid;else hi=mid;
}float n=lo;return freeLitres*(2f*n+1f)/(2f*n*(n+1f));}public static string FormatDiscovered(string typeId,string
subtypeId,float litres){return typeId+"/"+subtypeId+"/"+litres.ToString("0.####");}public static bool ParseDiscovered(string
value,out string typeId,out string subtypeId,out float litres){typeId=null;subtypeId=null;litres=0f;if(string.IsNullOrEmpty(
value))return false;string[]parts=value.Split('/');if(parts.Length<2)return false;typeId=parts[0].Trim();subtypeId=parts[1].
Trim();if(typeId.Length==0||subtypeId.Length==0)return false;if(parts.Length>=3){float v;if(float.TryParse(parts[2].Trim(),
out v)&&v>0f)litres=v;}return true;}bool RegisterDiscoveredType(MyItemType type,float litres){if(KEY_OF.ContainsKey(type))
return false;string baseKey=SanitizeItemKey(type.SubtypeId);string key=baseKey;MyItemType existing;if(ALL_ITEMS.TryGetValue(
key,out existing)&&existing!=type){key=baseKey+"_"+ShortTypeTag(type.TypeId);int n=2;while(ALL_ITEMS.TryGetValue(key,out
existing)&&existing!=type){key=baseKey+"_"+ShortTypeTag(type.TypeId)+n;n++;}}ALL_ITEMS[key]=type;KEY_OF[type]=key;if(litres>0f)
VOL_PER_UNIT[type]=litres;else _volumeProbeQueue.Add(type);_discoveredTypes.Add(type);List<string>cat;if(!CATEGORIES.TryGetValue(
"Modded",out cat)){cat=new List<string>();CATEGORIES["Modded"]=cat;}cat.Add(key);_discoveredDirty=true;return true;}void
LoadDiscovered(){_ini.Clear();if(!_ini.TryParse(Me.CustomData,out _iniResult))return;if(!_ini.ContainsSection(MODDED_SECTION))return;
_discoverEnabled=_ini.Get(MODDED_SECTION,"Discover").ToBoolean(true);_tempIniKeys.Clear();_ini.GetKeys(MODDED_SECTION,_tempIniKeys);
foreach(var iniKey in _tempIniKeys){if(iniKey.Name.Equals("Discover",StringComparison.OrdinalIgnoreCase))continue;string typeId
,subtypeId;float litres;if(!ParseDiscovered(_ini.Get(iniKey).ToString(""),out typeId,out subtypeId,out litres))continue;
RegisterDiscoveredType(new MyItemType(typeId,subtypeId),litres);}_discoveredDirty=false;}void SaveDiscovered(){_ini.Clear();_ini.TryParse(Me.
CustomData,out _iniResult);_ini.Set(MODDED_SECTION,"Discover",_discoverEnabled);foreach(var type in _discoveredTypes){string key;
if(!KEY_OF.TryGetValue(type,out key))continue;float vol;if(!VOL_PER_UNIT.TryGetValue(type,out vol))vol=0f;_ini.Set(
MODDED_SECTION,key,FormatDiscovered(type.TypeId,type.SubtypeId,vol));}_ini.SetSectionComment(MODDED_SECTION,MODDED_COMMENT);Me.
CustomData=_ini.ToString();_discoveredDirty=false;}void LearnVolume(MyItemType type,float deltaLitres,float movedUnits){if(
movedUnits<=0f||deltaLitres<=0f)return;if(!_discoveredTypes.Contains(type))return;float perUnit=deltaLitres/movedUnits;if(perUnit
<=0f)return;float known;if(VOL_PER_UNIT.TryGetValue(type,out known)&&known>0f&&Math.Abs(known-perUnit)<known*0.02f)return;
VOL_PER_UNIT[type]=perUnit;_discoveredDirty=true;}void EnsureVolumeKnown(MyItemType type,IMyInventory probeInv){if(VOL_PER_UNIT.
ContainsKey(type))return;if(!_discoveredTypes.Contains(type))return;var best=probeInv;float bestFree=best==null?0f:(float)(best.
MaxVolume-best.CurrentVolume)*1000f;foreach(var c in _containers){if(!c.HasInventory)continue;var inv=c.GetInventory(0);if(inv==
null)continue;float freeL=(float)(inv.MaxVolume-inv.CurrentVolume)*1000f;if(freeL>bestFree){best=inv;bestFree=freeL;}}if(
best==null||bestFree<=MIN_TRANSFER_LITRES)return;var probeTarget=best;float perUnit=ProbeVolumeLitres(bestFree,n=>
probeTarget.CanItemsBeAdded((MyFixedPoint)n,type));if(perUnit>0f){VOL_PER_UNIT[type]=perUnit;_discoveredDirty=true;}}void
RegisterItem(string category,string key,string typeId,string subtypeId,float litresPerUnit){var type=new MyItemType(typeId,subtypeId
);ALL_ITEMS[key]=type;KEY_OF[type]=key;VOL_PER_UNIT[type]=litresPerUnit;List<string>cat;if(!CATEGORIES.TryGetValue(
category,out cat)){cat=new List<string>();CATEGORIES[category]=cat;}cat.Add(key);}void BuildItemDatabase(){if(ALL_ITEMS.Count>0)
return;RegisterItem("Ores","CobaltOre","MyObjectBuilder_Ore","Cobalt",0.37f);RegisterItem("Ores","GoldOre",
"MyObjectBuilder_Ore","Gold",0.37f);RegisterItem("Ores","IceOre","MyObjectBuilder_Ore","Ice",0.37f);RegisterItem("Ores","IronOre",
"MyObjectBuilder_Ore","Iron",0.37f);RegisterItem("Ores","MagnesiumOre","MyObjectBuilder_Ore","Magnesium",0.37f);RegisterItem("Ores",
"NickelOre","MyObjectBuilder_Ore","Nickel",0.37f);RegisterItem("Ores","PlatinumOre","MyObjectBuilder_Ore","Platinum",0.37f);
RegisterItem("Ores","ScrapOre","MyObjectBuilder_Ore","Scrap",0.37f);RegisterItem("Ores","SiliconOre","MyObjectBuilder_Ore","Silicon"
,0.37f);RegisterItem("Ores","SilverOre","MyObjectBuilder_Ore","Silver",0.37f);RegisterItem("Ores","StoneOre",
"MyObjectBuilder_Ore","Stone",0.37f);RegisterItem("Ores","UraniumOre","MyObjectBuilder_Ore","Uranium",0.37f);RegisterItem("Ingots",
"CobaltIngot","MyObjectBuilder_Ingot","Cobalt",0.112f);RegisterItem("Ingots","GoldIngot","MyObjectBuilder_Ingot","Gold",0.052f);
RegisterItem("Ingots","GravelIngot","MyObjectBuilder_Ingot","Stone",0.37f);RegisterItem("Ingots","IronIngot","MyObjectBuilder_Ingot"
,"Iron",0.127f);RegisterItem("Ingots","MagnesiumIngot","MyObjectBuilder_Ingot","Magnesium",0.575f);RegisterItem("Ingots",
"NickelIngot","MyObjectBuilder_Ingot","Nickel",0.112f);RegisterItem("Ingots","PlatinumIngot","MyObjectBuilder_Ingot","Platinum",
0.047f);RegisterItem("Ingots","SiliconIngot","MyObjectBuilder_Ingot","Silicon",0.429f);RegisterItem("Ingots","SilverIngot",
"MyObjectBuilder_Ingot","Silver",0.095f);RegisterItem("Ingots","UraniumIngot","MyObjectBuilder_Ingot","Uranium",0.052f);RegisterItem(
"Components","BulletproofGlass","MyObjectBuilder_Component","BulletproofGlass",8.0f);RegisterItem("Components","Canvas",
"MyObjectBuilder_Component","Canvas",8.0f);RegisterItem("Components","Computer","MyObjectBuilder_Component","Computer",1.0f);RegisterItem(
"Components","Construction","MyObjectBuilder_Component","Construction",2.0f);RegisterItem("Components","Detector",
"MyObjectBuilder_Component","Detector",6.0f);RegisterItem("Components","Display","MyObjectBuilder_Component","Display",6.0f);RegisterItem(
"Components","Explosives","MyObjectBuilder_Component","Explosives",2.0f);RegisterItem("Components","Girder",
"MyObjectBuilder_Component","Girder",2.0f);RegisterItem("Components","GravityGenerator","MyObjectBuilder_Component","GravityGenerator",200.0f);
RegisterItem("Components","InteriorPlate","MyObjectBuilder_Component","InteriorPlate",5.0f);RegisterItem("Components","LargeTube",
"MyObjectBuilder_Component","LargeTube",38.0f);RegisterItem("Components","Medical","MyObjectBuilder_Component","Medical",160.0f);RegisterItem(
"Components","MetalGrid","MyObjectBuilder_Component","MetalGrid",15.0f);RegisterItem("Components","Motor",
"MyObjectBuilder_Component","Motor",8.0f);RegisterItem("Components","PowerCell","MyObjectBuilder_Component","PowerCell",45.0f);RegisterItem(
"Components","RadioCommunication","MyObjectBuilder_Component","RadioCommunication",70.0f);RegisterItem("Components","Reactor",
"MyObjectBuilder_Component","Reactor",8.0f);RegisterItem("Components","SmallTube","MyObjectBuilder_Component","SmallTube",2.0f);RegisterItem(
"Components","SolarCell","MyObjectBuilder_Component","SolarCell",20.0f);RegisterItem("Components","SteelPlate",
"MyObjectBuilder_Component","SteelPlate",3.0f);RegisterItem("Components","Superconductor","MyObjectBuilder_Component","Superconductor",8.0f);
RegisterItem("Components","Thrust","MyObjectBuilder_Component","Thrust",10.0f);RegisterItem("Components","ZoneChip",
"MyObjectBuilder_Component","ZoneChip",0.2f);RegisterItem("Ammo","AssaultCannonShell","MyObjectBuilder_AmmoMagazine","AssaultCannonShell",35.0f);
RegisterItem("Ammo","AutocannonClip","MyObjectBuilder_AmmoMagazine","AutocannonClip",24.0f);RegisterItem("Ammo","LargeCalibreAmmo",
"MyObjectBuilder_AmmoMagazine","LargeCalibreAmmo",96.0f);RegisterItem("Ammo","LargeRailgunAmmo","MyObjectBuilder_AmmoMagazine","LargeRailgunAmmo",
60.0f);RegisterItem("Ammo","MediumCalibreAmmo","MyObjectBuilder_AmmoMagazine","MediumCalibreAmmo",24.0f);RegisterItem("Ammo",
"Missile200mm","MyObjectBuilder_AmmoMagazine","Missile200mm",60.0f);RegisterItem("Ammo","NATO_25x184mm","MyObjectBuilder_AmmoMagazine"
,"NATO_25x184mm",16.0f);RegisterItem("Ammo","NATO_5p56x45mm","MyObjectBuilder_AmmoMagazine","NATO_5p56x45mm",0.02f);
RegisterItem("Ammo","RocketAmmo","MyObjectBuilder_AmmoMagazine","RocketAmmo",60.0f);RegisterItem("Ammo","SmallRailgunAmmo",
"MyObjectBuilder_AmmoMagazine","SmallRailgunAmmo",10.0f);RegisterItem("Tools","AngleGrinder","MyObjectBuilder_PhysicalGunObject","AngleGrinderItem",
20.0f);RegisterItem("Tools","AngleGrinder2","MyObjectBuilder_PhysicalGunObject","AngleGrinder2Item",20.0f);RegisterItem(
"Tools","AngleGrinder3","MyObjectBuilder_PhysicalGunObject","AngleGrinder3Item",20.0f);RegisterItem("Tools","AngleGrinder4",
"MyObjectBuilder_PhysicalGunObject","AngleGrinder4Item",20.0f);RegisterItem("Tools","HandDrill","MyObjectBuilder_PhysicalGunObject","HandDrillItem",12.0f);
RegisterItem("Tools","HandDrill2","MyObjectBuilder_PhysicalGunObject","HandDrill2Item",12.0f);RegisterItem("Tools","HandDrill3",
"MyObjectBuilder_PhysicalGunObject","HandDrill3Item",12.0f);RegisterItem("Tools","HandDrill4","MyObjectBuilder_PhysicalGunObject","HandDrill4Item",12.0f);
RegisterItem("Tools","Welder","MyObjectBuilder_PhysicalGunObject","WelderItem",20.0f);RegisterItem("Tools","Welder2",
"MyObjectBuilder_PhysicalGunObject","Welder2Item",20.0f);RegisterItem("Tools","Welder3","MyObjectBuilder_PhysicalGunObject","Welder3Item",20.0f);
RegisterItem("Tools","Welder4","MyObjectBuilder_PhysicalGunObject","Welder4Item",20.0f);RegisterItem("Weapons","AutomaticRifle",
"MyObjectBuilder_PhysicalGunObject","AutomaticRifleItem",14.0f);RegisterItem("Weapons","BasicHandHeldLauncher","MyObjectBuilder_PhysicalGunObject",
"BasicHandHeldLauncherItem",33.0f);RegisterItem("Weapons","PreciseRifle","MyObjectBuilder_PhysicalGunObject","PreciseAutomaticRifleItem",14.0f);
RegisterItem("Weapons","RapidRifle","MyObjectBuilder_PhysicalGunObject","RapidFireAutomaticRifleItem",14.0f);RegisterItem("Weapons",
"RocketLauncher","MyObjectBuilder_PhysicalGunObject","RocketLauncherItem",33.0f);RegisterItem("Weapons","UltimateRifle",
"MyObjectBuilder_PhysicalGunObject","UltimateAutomaticRifleItem",14.0f);RegisterItem("Bottles","HydrogenBottle","MyObjectBuilder_GasContainerObject",
"HydrogenBottle",120.0f);RegisterItem("Bottles","OxygenBottle","MyObjectBuilder_OxygenContainerObject","OxygenBottle",120.0f);
RegisterItem("Misc","Datapad","MyObjectBuilder_Datapad","Datapad",0.1f);RegisterItem("Misc","Package","MyObjectBuilder_Package",
"Package",1.0f);RegisterItem("Misc","SpaceCredit","MyObjectBuilder_PhysicalObject","SpaceCredit",0.01f);}
