const string _debugLCDTag="Отладка";const string _IGCAsh_to_AxisTag="XJI_ML_Axis";const string _IGCAshTag="XJI_FCS_Ash";
string _myName="Ash carrier";const int ReInitTime=360;Color _interfaceColor=new Color(0,100,0,255);Color _targetColor=new
Color(0,255,0,255);Color _ballisticColor=new Color(0,255,0,255);Color _weaponColor=new Color(255,0,0);Color _powerColor=new
Color(255,255,0);Color _propulsionColor=new Color(0,0,255);Color _missileColor=new Color(172,0,230,255);Color _allieColor=new
Color(249,124,0,255);static int _unlockTime=180;const int timeToUpdateButtons=5;string _updateInfo,_statusInfo;List<
IMyTerminalBlock>_allBlocks=new List<IMyTerminalBlock>();List<IMyUserControllableGun>_allGuns=new List<IMyUserControllableGun>();List<
IMyRadioAntenna>_antennas=new List<IMyRadioAntenna>();List<IMyUserControllableGun>_myGuns=new List<IMyUserControllableGun>();List<
IMySmallGatlingGun>_gatlings=new List<IMySmallGatlingGun>();List<IMySmallMissileLauncher>_mLaunchers=new List<IMySmallMissileLauncher>();
List<IMyMotorStator>_allRotors=new List<IMyMotorStator>();List<IMyMotorStator>_rotorsE=new List<IMyMotorStator>();List<
IMyCameraBlock>_radarCameras=new List<IMyCameraBlock>();List<IMyCameraBlock>_allActiveCameras=new List<IMyCameraBlock>();List<
IMyCameraBlock>_myCameras=new List<IMyCameraBlock>();List<IMyCameraBlock>_allCameras=new List<IMyCameraBlock>();List<IMyTextPanel>
_textPanels=new List<IMyTextPanel>();List<IMyShipController>_shipControllers=new List<IMyShipController>();List<IMyGyro>_myGyro=new
List<IMyGyro>();List<IMyLargeTurretBase>_turrets=new List<IMyLargeTurretBase>();List<IMyTurretControlBlock>_TCs=new List<
IMyTurretControlBlock>();LinkedList<AllieTarget>_alliesT=new LinkedList<AllieTarget>();LinkedList<UnitInfo>_missiles=new LinkedList<UnitInfo>
();LinkedList<AllieUnitInfo>_allies=new LinkedList<AllieUnitInfo>();IMyTextPanel _debugLCD;IMyBlockGroup _FCSGroup;
IMyMotorStator _rotorA;IMyShipController _myShipController;IMyShipController _activeShipController;IMyTerminalBlock _referenceBlock;
Turret _turret=new Turret();HullGuidance Hull=new HullGuidance();Radar _radar;TurretRadar _turretRadar=new TurretRadar();
BroadcastModule _communicator;static Dictionary<string,CockpitDef>CockpitDefinitions=new Dictionary<string,CockpitDef>(){{
"Cockpit/SmallBlockCockpit",new CockpitDef(){Up=0.46f,Back=0.28f}},{"Cockpit/SmallBlockCapCockpit",new CockpitDef(){Up=0.46f,Back=0.1f}},{
"Cockpit/OpenCockpitSmall",new CockpitDef(){Up=0.5f,Back=0.19f}},{"Cockpit/SmallBlockStandingCockpit",new CockpitDef(){Up=0.707f,Back=-0.2f}},{
"Cockpit/LargeBlockCockpitSeat",new CockpitDef(){Up=0.5f,Back=0.19f}},};static Dictionary<string,WeaponDef>WeaponDefinitions=new Dictionary<string,
WeaponDef>(){{"SmallGatlingGun/",new WeaponDef(){type="Gatling",Range=800,StartSpeed=400,ReloadTime=1/11.67f}},{
"SmallGatlingGun/SmallGatlingGunWarfare2",new WeaponDef(){type="Gatling",Range=800,StartSpeed=400,ReloadTime=1/11.67f}},{"SmallGatlingGun/SmallBlockAutocannon",
new WeaponDef(){type="Autocanon",Range=800,StartSpeed=400,ReloadTime=1/2.5f}},{
"SmallMissileLauncherReload/SmallBlockMediumCalibreGun",new WeaponDef(){type="Assault canon",Range=1400,StartSpeed=500,ReloadTime=6}},{
"SmallMissileLauncherReload/SmallRailgun",new WeaponDef(){type="Small Railgun",Range=1400,StartSpeed=1000,ReloadTime=20}},{
"SmallMissileLauncher/LargeBlockLargeCalibreGun",new WeaponDef(){type="Artillery",Range=2000,StartSpeed=500,ReloadTime=12}},{"SmallMissileLauncherReload/LargeRailgun",
new WeaponDef(){type="Large Railgun",Range=2000,StartSpeed=2000,ReloadTime=60}},};CockpitDef _cockpitInfo=new CockpitDef();
WeaponDef _weaponInfo=new WeaponDef();string _myIniWeapon="",_customWeapon="false";MyIni _myIni=new MyIni();const string
INI_SECTION_NAMES="Names",INI_NAME="My name",INI_GROUP_NAME_TAG="Group name tag",INI_AZ_ROTOR_NAME_TAG="Azimuth Rotor name tag",
INI_EL_ROTOR_NAME_TAG="Elevation Rotor name tag",INI_MAIN_COCKPIT_NAME_TAG="Name tag \"Main\"",INI_SIGHT_NAME_TAG="Sight name tag",
INI_SECTION_DISPLAY="Display",INI_UNLIMITED_FPS="FPS Limit",INI_SHOW_WEAPON_INFO="Show Weapon Info",INI_SHOW_TANK="Show Tank",
INI_COLOR_INTERFACE="Interface c",INI_COLOR_TARGET="Target color",INI_COLOR_BALLISTC="Ballistic point color",INI_COLOR_TARGET_WEAPON=
"Target weapons color",INI_COLOR_TARGET_POWER="Target power color",INI_COLOR_TARGET_PROPULSION="Target propulsions color",INI_COLOR_ALLIE=
"Allie color",INI_SECTION_RADAR="Radar",INI_INITIAL_RANGE="Initial Range",INI_SECTION_CONTROLS="Controls",INI_EL_MULT=
"Elevation Rotor Multiplier",INI_AZ_MULT="Azimuth Rotor Multiplier",INI_YAW_MULT="Yaw Gyro Multiplier",INI_PITCH_MULT="Pitch Gyro Multiplier",
INI_INTERACTIVE_MOD="Interactive Mode",INI_SECTION_WEAPON="Weapon",INI_MY_WEAPON="My weapon",INI_CUSTOM_SETTINGS="Custom settings",
INI_WEAPON_SHOOT_VELOCITY="Projectile velocity",INI_WEAPON_FIRE_RANGE="Shot range",INI_WEAPON_RELOAD_TIME="Reload Time",INI_SECTION_COCKPIT=
"Cockpit",INI_COEF_UP="Observer position - up",INI_COEF_BACK="Observer position - back",INI_SECTION_TARGETS="Targets",INI_ENEMY=
"Enemy",INI_NEUTRAL="Neutral",INI_ALLIE="Allie",INI_DISPLAYED_TARGET="Displayed Target",INI_SECTION_DEFAULTS="Defaults",
INI_AZIMUTH_ANGLE="Azimuth default angle",INI_ELEVATION_ANGLE="Elevation default angle";string _FCSTag="Ash",_azimuthRotorTag="Azimuth",
_elevationRotorTag="Elevation",_mainTag="Main",_sightNameTag="SIGHT";float elevationSpeedMult=0.001f,azimuthSpeedMult=0.001f,yawMult=
0.001f,pitchMult=0.001f,_myWeaponShotVelocity=400,_myWeaponRangeToFire=800,_myWeaponReloadTime=1/2.5f,_defObsCoefUp=0.46f,
_defObsCoefBack=0.28f,_initialRange=2000,_azimuthDefaultAngle=0,_elevationDefaultAngle=0;bool isTurret=false,canAutoTarget=false,
stabilization=true,autotarget=false,aimAssist=false,isVehicle=false,getTarget=false,_showWeaponInfo=true,drawTank=true,interactive=
false,block=false,centering=false,_fpsLimit=false,_axis=false;long Tick=0,_lastAxisTick=-120;bool allie=false,enemy=true,
neutral=true;IMyMotorStator _mainElRotor;float horizont=0,vertical=0,menuMove=0,Y_button;int menuTimer=0,_targetingPoint=0,
_subsystem=1;Program(){_communicator=new BroadcastModule(IGC,_IGCAsh_to_AxisTag,_IGCAshTag);Runtime.UpdateFrequency=
UpdateFrequency.Update1;}void Main(string argument,UpdateType updateSource){if(menuTimer>0)menuTimer--;_statusInfo="\nSystem status:\n"
;if((Tick%ReInitTime)==0){UpdateBlocks(ref _updateInfo);}_activeShipController=null;switch(argument){case"action":Action(
);break;case"switch_lock":if(_radar.Searching){_radar.DropLock();}else _radar.Searching=true;break;case"switch_aimAssist"
:aimAssist=!aimAssist;autotarget=false;break;case"switch_aiMode":if(aimAssist&&autotarget){aimAssist=false;autotarget=
false;}else{if(!aimAssist)aimAssist=true;else autotarget=!autotarget;}break;case"switch_stab":stabilization=!stabilization;
break;case"switch_subsystem":if(_subsystem==3)_subsystem=0;else _subsystem++;break;case"block":Block();break;case"centering":
centering=!centering;break;default:break;}CommandHandler(argument);if(_radar.Update(Tick,_unlockTime,_initialRange)){if(_antennas
.Count>0){_communicator.SendInfo(Tick,Me.EntityId,_IGCAshTag,_radar.lockedtarget);}foreach(var m in _missiles){if(m.tId==
_radar.lockedtarget.EntityId){_communicator.SendInfo(Tick,_IGCAsh_to_AxisTag,_radar.lockedtarget);break;}}}_statusInfo+=
$"Radar - searching: {_radar.Searching}\n";if(_radar.lockedtarget!=null){EnemyTargetedInfo newTarget;newTarget=_turretRadar.Update(Tick,_radar.lockedtarget);
_radar.UpdateTarget(newTarget);_statusInfo+=$"Target locked: {_radar.lockedtarget.Type} \n";}else{_turretRadar.Update(Tick);}
_communicator.GetMessageFromAxis(Tick,ref _lastAxisTick,_IGCAsh_to_AxisTag,_radar.lockedtarget,_referenceBlock,_missiles);if(
_antennas.Count>0){if(_activeShipController!=null){_communicator.SendMyPos(_IGCAshTag,Me.EntityId,_activeShipController.
GetPosition(),_myName);}else _communicator.SendMyPos(_IGCAshTag,Me.EntityId,Me.GetPosition(),_myName);}_communicator.
GetMessageFromAsh(Tick,Me.EntityId,_IGCAshTag,_allies,_alliesT);_axis=false;if((Tick-_lastAxisTick)<120)_axis=true;if(_myShipController!=
null)_activeShipController=_myShipController;else foreach(var cockpit in _shipControllers){if(cockpit.IsUnderControl){
_activeShipController=cockpit;break;}}if(_activeShipController!=null){horizont=_activeShipController.RotationIndicator.Y;vertical=-
_activeShipController.RotationIndicator.X;menuMove=_activeShipController.RollIndicator;Y_button=_activeShipController.MoveIndicator.Y;
_cockpitInfo=GetCockpitInfo(_activeShipController);}if(menuMove!=0||Y_button!=0){if(interactive&&menuTimer==0&&isTurret){if(menuMove
>0){Action();}if(menuMove<0){Block();}if(Y_button<0){centering=!centering;}}menuTimer=timeToUpdateButtons;}Vector3D?obs=
null;Vector3D?obsForward=null;Vector3D?Intersept=null;Vector3D?BallicticPoint=null;Vector3D?ShootDirection=null;Vector3D
MyPos;Drawing.GetObserverPos(ref obs,ref obsForward,_cockpitInfo.Up,_cockpitInfo.Back,_activeShipController,_myCameras);if(
isTurret){ShootDirection=_turret.referenceBlock.WorldMatrix.Forward;MyPos=_turret.referenceBlock.GetPosition();}else if(
isVehicle){if(_referenceBlock!=null)MyPos=_referenceBlock.GetPosition();else if(_activeShipController!=null)MyPos=
_activeShipController.GetPosition();else MyPos=Me.GetPosition();}else{if(obs!=null)MyPos=obs.GetValueOrDefault();else MyPos=Me.GetPosition();
}if(getTarget)if(obs!=null){GetClosedTarget(_turretRadar.GetTargets(),obs.GetValueOrDefault(),obsForward.
GetValueOrDefault(),ref _radar,Tick);getTarget=false;}if(ShootDirection==null){if(_myShipController!=null){ShootDirection=
_myShipController.WorldMatrix.Forward;}else if(_shipControllers.Count>0)ShootDirection=_shipControllers[0].WorldMatrix.Forward;else
ShootDirection=Me.WorldMatrix.Forward;}if(_radar.lockedtarget!=null){if(_shipControllers.Count>0){EnemyTargetedInfo Target=_radar.
lockedtarget;Vector3D MySpeed=_shipControllers[0].GetShipVelocities().LinearVelocity;Vector3D gravity=_shipControllers[0].
GetNaturalGravity();Intersept=MyMath.FindInterceptGVector(MyPos,MySpeed,Target,gravity,_weaponInfo.StartSpeed,_targetingPoint,false);
Vector3D prSpeed=ShootDirection.GetValueOrDefault()*_weaponInfo.StartSpeed;BallicticPoint=MyMath.FindBallisticPoint(MyPos,
MySpeed,Target,gravity,prSpeed,_targetingPoint);}}else if(_radar.pointOfLock!=null)if(_shipControllers.Count>0){Vector3D
MySpeed=_shipControllers[0].GetShipVelocities().LinearVelocity;Vector3D gravity=_shipControllers[0].GetNaturalGravity();
Vector3D prSpeed=ShootDirection.GetValueOrDefault()*_weaponInfo.StartSpeed;BallicticPoint=MyMath.FindBallisticPoint(MyPos,
MySpeed,_radar.pointOfLock.GetValueOrDefault(),gravity,prSpeed);}if(canAutoTarget){if(isTurret){if(aimAssist){if(Intersept!=
null){if(autotarget){_turret.Status(ref _statusInfo,_azimuthRotorTag,_elevationRotorTag);_turret.Update(Intersept.
GetValueOrDefault(),true,0,0,false);}else{_turret.Status(ref _statusInfo,_azimuthRotorTag,_elevationRotorTag);_turret.Update(Intersept.
GetValueOrDefault(),false,azimuthSpeedMult*horizont,elevationSpeedMult*vertical,false);}centering=false;}else{_turret.Status(ref
_statusInfo,_azimuthRotorTag,_elevationRotorTag);_turret.Update(azimuthSpeedMult*horizont,elevationSpeedMult*vertical,ref centering
,_azimuthDefaultAngle,_elevationDefaultAngle,stabilization);};}else{_turret.Status(ref _statusInfo,_azimuthRotorTag,
_elevationRotorTag);_turret.Update(azimuthSpeedMult*horizont,elevationSpeedMult*vertical,ref centering,_azimuthDefaultAngle,
_elevationDefaultAngle,stabilization);}}else if(isVehicle){if(aimAssist){if(Intersept==null){Hull.Drop(_myGyro);}else{if(_myShipController!=
null){if(autotarget)Hull.Control(_myShipController,Intersept.Value,_myShipController.RollIndicator,_myGyro,false);else{Hull.
Control(_myShipController,Intersept.Value,_myShipController.RollIndicator,_myGyro,false,false,yawMult,pitchMult);}}else{
IMyShipController activeSC=null;foreach(var sc in _shipControllers){if(sc.IsUnderControl){activeSC=sc;break;}}if(activeSC!=null){if(
autotarget)Hull.Control(activeSC,Intersept.Value,activeSC.RollIndicator,_myGyro,false);else{Hull.Control(activeSC,Intersept.Value,
activeSC.RollIndicator,_myGyro,false,false,yawMult,pitchMult);}}else Hull.Drop(_myGyro);}}}else Hull.Drop(_myGyro);}}if(obs!=
null&&(!_fpsLimit|(Tick%10)==0)){foreach(var lcd in _textPanels){if(!_fpsLimit)lcd.ContentType=ContentType.TEXT_AND_IMAGE;
Drawing.SetupDrawSurface(lcd);var frame=lcd.DrawFrame();DrawingInfo DI=new DrawingInfo(obsForward.GetValueOrDefault(),frame,lcd
,obs.GetValueOrDefault(),_targetColor){Target=_radar.lockedtarget};double distance=0;float losing=1f;bool searching=
_radar.Searching;if(_radar.lockedtarget!=null){distance=(obs-_radar.lockedtarget.HitPosition).GetValueOrDefault().Length();
losing=(float)(_unlockTime-_radar.counter)/_unlockTime;searching=false;}DI.point=obsForward.GetValueOrDefault();DI.c=
_interfaceColor;TankInfo tankInfo=new TankInfo(0,false,0,block,centering);DWI dWI=new DWI(){weaponDef=_weaponInfo,draw=_showWeaponInfo,
name=_referenceBlock.CustomName};if(isTurret){if(drawTank){tankInfo.turretRotation=(float)MyMath.
CalculateRotorDeviationAngle(obsForward.Value,_turret.turretMatrix);tankInfo.hullRotation=(float)MyMath.CalculateRotorDeviationAngle(obsForward.
Value,_activeShipController.WorldMatrix);tankInfo.drawTank=true;}}Drawing.BattleInterface(DI,tankInfo,dWI,_axis,losing,
isTurret,isVehicle,autotarget,aimAssist);foreach(var target in _turretRadar.GetTargets()){DI.Target=target;DI.c=_targetColor;if(
_radar.lockedtarget!=null){if(target.EntityId!=_radar.lockedtarget.EntityId)Drawing.DrawTurretTarget(DI);}else Drawing.
DrawTurretTarget(DI);}foreach(var m in _missiles){DI.point=m.target;DI.c=_missileColor;Drawing.DrawMissileTarget(DI);DI.point=m.pos;
Drawing.DrawMissilePos(DI);}foreach(var a in _allies){DI.point=a.pos;DI.c=_allieColor;Drawing.DrawAlliePos(DI,a.name,(float)(a.
pos-obs.Value).Length());}foreach(var t in _alliesT){DI.point=t.target.HitPosition.GetValueOrDefault(t.target.Position);DI.
c=_allieColor;Drawing.DrawTarget(DI);}if(_radar.lockedtarget!=null){var t=_radar.lockedtarget;DI.c=_targetColor;Vector3D
p=t.Position;if(_targetingPoint==0)if(t.TargetedPoint!=null)p=t.TargetedPoint.GetValueOrDefault();if(_targetingPoint==2)
if(t.HitPosition!=null)p=t.HitPosition.GetValueOrDefault();DI.point=p;Drawing.DrawTarget(DI);foreach(var subsystem in
_radar.lockedtarget.TargetSubsystems){switch(_subsystem){case 1:if(subsystem.subsystemType=="PowerSystems")Drawing.
DrawSubsystem(DI,subsystem,_powerColor);break;case 2:if(subsystem.subsystemType=="Propulsion")Drawing.DrawSubsystem(DI,subsystem,
_propulsionColor);break;case 3:if(subsystem.subsystemType=="Weapons")Drawing.DrawSubsystem(DI,subsystem,_weaponColor);break;default:
break;}}}if(BallicticPoint!=null){DI.point=BallicticPoint.Value;if(!((DI.point-MyPos).Length()>_weaponInfo.Range)){DI.c=
_ballisticColor;bool b=_radar.lockedtarget==null;Drawing.DrawBallisticPoint(DI,Intersept==null);}}DI.point=obsForward.GetValueOrDefault
();DI.c=_interfaceColor;if(searching){Drawing.DrawSight(DI);}frame.Dispose();}}Echo(
$"Before next update {(ReInitTime-(Tick%ReInitTime))/60} seconds");Echo(_updateInfo+_statusInfo);Tick++;}void GetClosedTarget(List<EnemyTargetedInfo>targets,Vector3D mypos,Vector3D
obsDir,ref Radar radar,long tick){if(targets.Count>0){double minAngle=MathHelper.Pi;foreach(EnemyTargetedInfo target in
targets){Vector3D dir=target.Position-mypos;if(minAngle>MyMath.VectorAngleBetween(dir,obsDir)){minAngle=MyMath.
VectorAngleBetween(dir,obsDir);radar.GetTarget(target,tick);}}}}bool UpdateBlocks(ref string updateInfo){_debugLCD=GridTerminalSystem.
GetBlockWithName(_debugLCDTag)as IMyTextPanel;LoadIniConfig();updateInfo="";updateInfo+=$"Language: English\n";canAutoTarget=false;
_allBlocks.Clear();_allGuns.Clear();_myGuns.Clear();_gatlings.Clear();_mLaunchers.Clear();_myCameras.Clear();_radarCameras.Clear()
;_allActiveCameras.Clear();_turrets.Clear();_TCs.Clear();_shipControllers.Clear();_allRotors.Clear();_rotorsE.Clear();
_myShipController=null;_mainElRotor=null;_rotorA=null;_myGyro.Clear();_antennas.Clear();_textPanels.Clear();GridTerminalSystem.
GetBlocksOfType(_shipControllers);GridTerminalSystem.GetBlocksOfType(_allRotors);GridTerminalSystem.GetBlocksOfType(_gatlings);
GridTerminalSystem.GetBlocksOfType(_mLaunchers);foreach(var weapon in _gatlings){_allGuns.Add(weapon as IMyUserControllableGun);}foreach(
var weapon in _mLaunchers){_allGuns.Add(weapon as IMyUserControllableGun);}if(_referenceBlock==null)isTurret=false;
isVehicle=false;GridTerminalSystem.GetBlocksOfType(_allCameras);foreach(var camera in _allCameras){if(camera.IsSameConstructAs(Me
)){_myCameras.Add(camera);}}_FCSGroup=GridTerminalSystem.GetBlockGroupWithName(_FCSTag);if(_FCSGroup==null){updateInfo+=
$"\nGroup not Found!\n"+$"Group name: {_FCSTag}\n";}else{_FCSGroup.GetBlocks(_allBlocks);foreach(var block in _allBlocks){if(SystemHelper.
AddToListIfType(block,_textPanels))continue;if(SystemHelper.AddToListIfType(block,_antennas))continue;if(block.IsSameConstructAs(Me)){
if(SystemHelper.AddToListIfType(block,_TCs))continue;if(SystemHelper.AddToListIfType(block,_turrets))continue;if(
SystemHelper.AddToListIfType(block,_allActiveCameras))continue;}if(SystemHelper.AddToListIfType(block,_myGyro))continue;}}foreach(
var a in _antennas)a.EnableBroadcasting=true;foreach(var camera in _allActiveCameras)if(!camera.CustomName.Contains(
_sightNameTag))_radarCameras.Add(camera);if(_turrets.Count==0&&_TCs.Count==0){GridTerminalSystem.GetBlocksOfType(_TCs);
GridTerminalSystem.GetBlocksOfType(_turrets);_turretRadar.UpdateBlocks(_turrets,_TCs,false);}else _turretRadar.UpdateBlocks(_turrets,_TCs)
;if(_radar==null){_radar=new Radar(_radarCameras);_radar.SetTargets(allie,neutral,enemy);}else{_radar.radarCameras=
_radarCameras;_radar.countOfCameras=_radarCameras.Count;_radar.SetTargets(allie,neutral,enemy);}if(_radarCameras.Count<2){updateInfo
+=$"\nNot enought cameras in the radar\n";}updateInfo+=$"\nLast update:\n"+$"Cameras in radar - "+_radarCameras.Count+
$"\nText panels - "+_textPanels.Count+"\n";if(_shipControllers.Count==1){_myShipController=_shipControllers[0];}else{foreach(var block in
_allBlocks){SystemHelper.AddBlockIfType(block,out _myShipController);}}if(_myShipController!=null)updateInfo+=
$"\nMain cockpit - \"{_myShipController.CustomName}\"\n";isTurret=false;bool added=false;foreach(var block in _allBlocks){if(block.CustomName.Contains(_azimuthRotorTag))if(
SystemHelper.AddBlockIfType(block,out _rotorA))continue;if(block.CustomName.Contains(_elevationRotorTag))if(SystemHelper.
AddToListIfType(block,_rotorsE))continue;}if(_rotorA!=null&&_rotorsE.Count!=0){foreach(var rotor in _rotorsE){foreach(var gun in
_allGuns){if(rotor.TopGrid==gun.CubeGrid){if(rotor.CustomName.Contains(_mainTag))_mainElRotor=rotor;if(_mainElRotor==null)
_mainElRotor=rotor;if(!_myGuns.Contains(gun))_myGuns.Add(gun);}}foreach(var camera in _allActiveCameras){if(rotor.TopGrid==camera.
CubeGrid){if(rotor.CustomName.Contains(_mainTag))_mainElRotor=rotor;if(_mainElRotor==null)_mainElRotor=rotor;}}}if(_myGuns.Count
==0){updateInfo+=$"\nTrying to create a turret from blocks in a group...\n"+$"Failure\n"+
$"Not found weapons on rotors \"{_elevationRotorTag}\"\n";_rotorsE.Clear();_rotorA=null;_myGuns.Clear();}else{_turret.UpdateBlocks(_rotorA,_rotorsE,_mainElRotor,_myGuns,
_allActiveCameras,_myGyro);updateInfo+=$"Trying to create a turret from blocks in a group...\n"+$"Success\n";isTurret=true;canAutoTarget=
true;}}else{updateInfo+=$"Trying to create a turret from blocks in a group..."+$"Failure\n"+
$"Not enought rotors in the group\n";}if(!isTurret){foreach(var rotor in _allRotors){added=false;if(rotor.TopGrid==Me.CubeGrid){_rotorA=rotor;continue;}else
if(rotor.CubeGrid==Me.CubeGrid){foreach(var gun in _allGuns){if(rotor.TopGrid==gun.CubeGrid){if(rotor.CustomName.Contains(
_mainTag)){_rotorsE.Add(rotor);_mainElRotor=rotor;added=true;}if(_mainElRotor==null&&!added){_rotorsE.Add(rotor);_mainElRotor=
rotor;added=true;}else if(!added){added=true;_rotorsE.Add(rotor);}if(!_myGuns.Contains(gun))_myGuns.Add(gun);}}if(!added){
foreach(var camera in _allActiveCameras){if(rotor.TopGrid==camera.CubeGrid){if(rotor.CustomName.Contains(_mainTag)){_rotorsE.
Add(rotor);_mainElRotor=rotor;added=true;}if(_mainElRotor==null&&!added){_rotorsE.Add(rotor);_mainElRotor=rotor;added=true;
break;}else if(!added){added=true;_rotorsE.Add(rotor);break;}}}}}}if(_rotorA!=null&&_mainElRotor!=null){_turret.UpdateBlocks(
_rotorA,_rotorsE,_mainElRotor,_myGuns,_allActiveCameras,_myGyro);updateInfo+=
$"Successful auto-transition to turret mode, all components found\n";isTurret=true;canAutoTarget=true;}else updateInfo+=$"Auto-transition to turret mode failed\n";}if(!isTurret){if(_myGyro
.Count==0)GridTerminalSystem.GetBlocksOfType(_myGyro);if(_shipControllers.Count==0){updateInfo+=
$"Transition to hull-guided mode failed:\n"+$"No cockpits\n";}else{if(_myGyro.Count==0)updateInfo+=$"Transition to hull-guided mode failed:\n"+$"No gyros\n";else{
canAutoTarget=true;isVehicle=true;updateInfo+=$"Transition to hull-guided mode successful:\n"+$"Gyroscopes: {_myGyro.Count}\n";}}}if(
_referenceBlock==null){FindReferenceBlock();}UpdateWeaponInfo();return true;}void Action(){if(_radar.lockedtarget==null&&_turretRadar.
GetTargets().Count!=0){getTarget=true;aimAssist=true;}else{_radar.DropLock();aimAssist=false;}}void Block(){if(!isTurret)return;
block=!block;_turret.Block(block);}void FindReferenceBlock(){if(!String.IsNullOrEmpty(_myIniWeapon)){if(TryGetWeaponFromName(
_myIniWeapon))return;}if(isTurret){_referenceBlock=_turret.referenceBlock;}if(_allGuns.Count!=0)foreach(var weapon in _allGuns){if(
_referenceBlock==null)_referenceBlock=weapon;if(weapon.CustomName.Contains(_mainTag)){_referenceBlock=weapon;return;}}if(
_allActiveCameras.Count!=0&&_referenceBlock==null)foreach(var camera in _allActiveCameras){if(_referenceBlock==null)_referenceBlock=
camera;if(camera.CustomName.Contains(_mainTag)){_referenceBlock=camera;return;}}}void UpdateWeaponInfo(){FindWeaponInfo();SC()
;}void FindWeaponInfo(){if(_customWeapon!="false"&&_customWeapon!="true"){if(WeaponDefinitions.TryGetValue(_customWeapon,
out _weaponInfo)){return;}}if(_customWeapon=="false"){if(_referenceBlock!=null){if(_referenceBlock as
IMyUserControllableGun!=null){_weaponInfo=GetWeaponInfo(_referenceBlock);return;}}if(isTurret){if(_turret.referenceBlock as
IMyUserControllableGun!=null){_weaponInfo=GetWeaponInfo(_turret.referenceBlock);return;}}if(_allGuns.Count!=0)_weaponInfo=GetWeaponInfo(
_allGuns[0]);else{_weaponInfo=new WeaponDef(){type="CUSTOM",Range=_myWeaponRangeToFire,StartSpeed=_myWeaponShotVelocity,
ReloadTime=_myWeaponReloadTime};}return;}else if(_customWeapon=="true"){_weaponInfo=new WeaponDef(){type="CUSTOM",Range=
_myWeaponRangeToFire,StartSpeed=_myWeaponShotVelocity,ReloadTime=_myWeaponReloadTime};}}bool TryGetWeaponFromName(string name){foreach(var
weapon in _allGuns)if(weapon.CustomName==name){if(isTurret){if(!_turret.TrySetRef(weapon))continue;}_referenceBlock=weapon;
_weaponInfo=GetWeaponInfo(weapon);return true;}foreach(var camera in _allActiveCameras)if(camera.CustomName==name){if(isTurret){if(
!_turret.TrySetRef(camera))continue;}_referenceBlock=camera;_weaponInfo=GetWeaponInfo(camera);return true;}return false;}
CockpitDef GetCockpitInfo(IMyTerminalBlock block){CockpitDef cockpitDef;if(!(block is IMyCockpit))return null;string key=
SystemHelper.GetKey(block);if(CockpitDefinitions.TryGetValue(key,out cockpitDef))return cockpitDef;else return new CockpitDef(){Up=
_defObsCoefUp,Back=_defObsCoefBack};}WeaponDef GetWeaponInfo(IMyTerminalBlock block){WeaponDef weaponInfo;string key=SystemHelper.
GetKey(block);if(WeaponDefinitions.TryGetValue(key,out weaponInfo))return weaponInfo;else if(WeaponDefinitions.TryGetValue(
_customWeapon,out weaponInfo))return weaponInfo;else return new WeaponDef(){type="CUSTOM",Range=_myWeaponRangeToFire,StartSpeed=
_myWeaponShotVelocity,ReloadTime=_myWeaponReloadTime};}void LoadIniConfig(){_myIni.Clear();bool parsed=_myIni.TryParse(Me.CustomData);if(!
parsed){SC();return;}_myName=_myIni.Get(INI_SECTION_NAMES,INI_NAME).ToString(_myName);_FCSTag=_myIni.Get(INI_SECTION_NAMES,
INI_GROUP_NAME_TAG).ToString(_FCSTag);_azimuthRotorTag=_myIni.Get(INI_SECTION_NAMES,INI_AZ_ROTOR_NAME_TAG).ToString(_azimuthRotorTag);
_elevationRotorTag=_myIni.Get(INI_SECTION_NAMES,INI_EL_ROTOR_NAME_TAG).ToString(_elevationRotorTag);_mainTag=_myIni.Get(INI_SECTION_NAMES,
INI_MAIN_COCKPIT_NAME_TAG).ToString(_mainTag);_sightNameTag=_myIni.Get(INI_SECTION_NAMES,INI_SIGHT_NAME_TAG).ToString(_sightNameTag);_fpsLimit=
_myIni.Get(INI_SECTION_DISPLAY,INI_UNLIMITED_FPS).ToBoolean(_fpsLimit);drawTank=_myIni.Get(INI_SECTION_DISPLAY,INI_SHOW_TANK).
ToBoolean(drawTank);_showWeaponInfo=_myIni.Get(INI_SECTION_DISPLAY,INI_SHOW_WEAPON_INFO).ToBoolean(_showWeaponInfo);IniToColor(
_myIni.Get(INI_SECTION_DISPLAY,INI_COLOR_INTERFACE),ref _interfaceColor);IniToColor(_myIni.Get(INI_SECTION_DISPLAY,
INI_COLOR_TARGET),ref _targetColor);IniToColor(_myIni.Get(INI_SECTION_DISPLAY,INI_COLOR_BALLISTC),ref _ballisticColor);IniToColor(_myIni
.Get(INI_SECTION_DISPLAY,INI_COLOR_TARGET_WEAPON),ref _weaponColor);IniToColor(_myIni.Get(INI_SECTION_DISPLAY,
INI_COLOR_TARGET_POWER),ref _powerColor);IniToColor(_myIni.Get(INI_SECTION_DISPLAY,INI_COLOR_TARGET_PROPULSION),ref _propulsionColor);
IniToColor(_myIni.Get(INI_SECTION_DISPLAY,INI_COLOR_ALLIE),ref _allieColor);_initialRange=(float)_myIni.Get(INI_SECTION_RADAR,
INI_INITIAL_RANGE).ToDouble(_initialRange);elevationSpeedMult=(float)_myIni.Get(INI_SECTION_CONTROLS,INI_EL_MULT).ToDouble(
elevationSpeedMult);azimuthSpeedMult=(float)_myIni.Get(INI_SECTION_CONTROLS,INI_AZ_MULT).ToDouble(azimuthSpeedMult);yawMult=(float)_myIni.
Get(INI_SECTION_CONTROLS,INI_YAW_MULT).ToDouble(yawMult);pitchMult=(float)_myIni.Get(INI_SECTION_CONTROLS,INI_PITCH_MULT).
ToDouble(pitchMult);interactive=_myIni.Get(INI_SECTION_CONTROLS,INI_INTERACTIVE_MOD).ToBoolean(interactive);_myWeaponRangeToFire
=(float)_myIni.Get(INI_SECTION_WEAPON,INI_WEAPON_FIRE_RANGE).ToDouble(_myWeaponRangeToFire);_myIniWeapon=_myIni.Get(
INI_SECTION_WEAPON,INI_MY_WEAPON).ToString(_myIniWeapon);_customWeapon=_myIni.Get(INI_SECTION_WEAPON,INI_CUSTOM_SETTINGS).ToString(
_customWeapon);_myWeaponShotVelocity=(float)_myIni.Get(INI_SECTION_WEAPON,INI_WEAPON_SHOOT_VELOCITY).ToDouble(_myWeaponShotVelocity);
_myWeaponReloadTime=(float)_myIni.Get(INI_SECTION_WEAPON,INI_WEAPON_RELOAD_TIME).ToDouble(_myWeaponReloadTime);_defObsCoefUp=(float)_myIni.
Get(INI_SECTION_COCKPIT,INI_COEF_UP).ToDouble(_defObsCoefUp);_defObsCoefBack=(float)_myIni.Get(INI_SECTION_COCKPIT,
INI_COEF_BACK).ToDouble(_defObsCoefBack);_azimuthDefaultAngle=(float)_myIni.Get(INI_SECTION_DEFAULTS,INI_AZIMUTH_ANGLE).ToDouble(
_azimuthDefaultAngle);_elevationDefaultAngle=(float)_myIni.Get(INI_SECTION_DEFAULTS,INI_ELEVATION_ANGLE).ToDouble(_elevationDefaultAngle);
allie=_myIni.Get(INI_SECTION_TARGETS,INI_ALLIE).ToBoolean(allie);neutral=_myIni.Get(INI_SECTION_TARGETS,INI_NEUTRAL).
ToBoolean(neutral);enemy=_myIni.Get(INI_SECTION_TARGETS,INI_ENEMY).ToBoolean(enemy);_targetingPoint=_myIni.Get(
INI_SECTION_TARGETS,INI_DISPLAYED_TARGET).ToInt32(_targetingPoint);}void SC(){_myIni.Clear();_myIni.Set(INI_SECTION_NAMES,INI_NAME,_myName)
;_myIni.Set(INI_SECTION_NAMES,INI_GROUP_NAME_TAG,_FCSTag);_myIni.Set(INI_SECTION_NAMES,INI_AZ_ROTOR_NAME_TAG,
_azimuthRotorTag);_myIni.Set(INI_SECTION_NAMES,INI_EL_ROTOR_NAME_TAG,_elevationRotorTag);_myIni.Set(INI_SECTION_NAMES,
INI_MAIN_COCKPIT_NAME_TAG,_mainTag);_myIni.Set(INI_SECTION_NAMES,INI_SIGHT_NAME_TAG,_sightNameTag);_myIni.Set(INI_SECTION_DISPLAY,
INI_UNLIMITED_FPS,_fpsLimit);_myIni.Set(INI_SECTION_DISPLAY,INI_SHOW_WEAPON_INFO,_showWeaponInfo);_myIni.Set(INI_SECTION_DISPLAY,
INI_SHOW_TANK,drawTank);_myIni.Set(INI_SECTION_DISPLAY,INI_COLOR_INTERFACE,ColorToString(_interfaceColor));_myIni.Set(
INI_SECTION_DISPLAY,INI_COLOR_TARGET,ColorToString(_targetColor));_myIni.Set(INI_SECTION_DISPLAY,INI_COLOR_BALLISTC,ColorToString(
_ballisticColor));_myIni.Set(INI_SECTION_DISPLAY,INI_COLOR_TARGET_WEAPON,ColorToString(_weaponColor));_myIni.Set(INI_SECTION_DISPLAY,
INI_COLOR_TARGET_POWER,ColorToString(_powerColor));_myIni.Set(INI_SECTION_DISPLAY,INI_COLOR_TARGET_PROPULSION,ColorToString(_propulsionColor))
;_myIni.Set(INI_SECTION_DISPLAY,INI_COLOR_ALLIE,ColorToString(_allieColor));_myIni.Set(INI_SECTION_RADAR,
INI_INITIAL_RANGE,_initialRange);_myIni.Set(INI_SECTION_CONTROLS,INI_EL_MULT,elevationSpeedMult);_myIni.Set(INI_SECTION_CONTROLS,
INI_AZ_MULT,azimuthSpeedMult);_myIni.Set(INI_SECTION_CONTROLS,INI_YAW_MULT,yawMult);_myIni.Set(INI_SECTION_CONTROLS,INI_PITCH_MULT,
pitchMult);_myIni.Set(INI_SECTION_CONTROLS,INI_INTERACTIVE_MOD,interactive);_myIni.Set(INI_SECTION_WEAPON,INI_MY_WEAPON,
_myIniWeapon);_myIni.Set(INI_SECTION_WEAPON,INI_CUSTOM_SETTINGS,_customWeapon);_myIni.Set(INI_SECTION_WEAPON,INI_WEAPON_FIRE_RANGE,
_myWeaponRangeToFire);_myIni.Set(INI_SECTION_WEAPON,INI_WEAPON_SHOOT_VELOCITY,_myWeaponShotVelocity);_myIni.Set(INI_SECTION_WEAPON,
INI_WEAPON_RELOAD_TIME,_myWeaponReloadTime);_myIni.Set(INI_SECTION_COCKPIT,INI_COEF_UP,_defObsCoefUp);_myIni.Set(INI_SECTION_COCKPIT,
INI_COEF_BACK,_defObsCoefBack);_myIni.Set(INI_SECTION_DEFAULTS,INI_AZIMUTH_ANGLE,_azimuthDefaultAngle);_myIni.Set(
INI_SECTION_DEFAULTS,INI_ELEVATION_ANGLE,_elevationDefaultAngle);_myIni.Set(INI_SECTION_TARGETS,INI_ALLIE,allie);_myIni.Set(
INI_SECTION_TARGETS,INI_NEUTRAL,neutral);_myIni.Set(INI_SECTION_TARGETS,INI_ENEMY,enemy);_myIni.Set(INI_SECTION_TARGETS,
INI_DISPLAYED_TARGET,_targetingPoint);Me.CustomData=_myIni.ToString();}void CommandHandler(string command){if(String.IsNullOrEmpty(command))
return;List<string>commandSplit=SystemHelper.SplitString(command);if(commandSplit[0]=="use"){for(int i=1;i<commandSplit.Count(
);i++){switch(commandSplit[i]){case"-p":string p=commandSplit.ElementAtOrDefault(i+1);if(!String.IsNullOrEmpty(p)){
DefaultPresets(p);}break;case"-n":string n=commandSplit.ElementAtOrDefault(i+1);if(!String.IsNullOrEmpty(n)){if(TryGetWeaponFromName(n
)){_myIniWeapon=n;SC();}}break;case"-v":string v=commandSplit.ElementAtOrDefault(i+1);if(!String.IsNullOrEmpty(v)){float
vel;if(float.TryParse(v,out vel)){_customWeapon="true";_myWeaponShotVelocity=vel;FindWeaponInfo();SC();}}break;case"-d":
_myIniWeapon="";_customWeapon="false";_referenceBlock=null;FindReferenceBlock();FindWeaponInfo();SC();break;default:break;}}}}void
DefaultPresets(string p){switch(p){case"custom":_customWeapon="true";FindWeaponInfo();SC();break;case"gatling":_customWeapon=
"SmallGatlingGun/";FindWeaponInfo();SC();break;case"autoCanon":_customWeapon="SmallGatlingGun/SmallBlockAutocannon";FindWeaponInfo();SC();
break;case"assaultCanon":_customWeapon="SmallMissileLauncherReload/SmallBlockMediumCalibreGun";FindWeaponInfo();SC();break;
case"artillery":_customWeapon="SmallMissileLauncher/LargeBlockLargeCalibreGun";FindWeaponInfo();SC();break;case"smallRail":
_customWeapon="SmallMissileLauncherReload/SmallRailgun";FindWeaponInfo();SC();break;case"largeRail":_customWeapon=
"SmallMissileLauncherReload/LargeRailgun";FindWeaponInfo();SC();break;case"defaul":_customWeapon="false";UpdateWeaponInfo();SC();break;default:break;}}void
SetWeaponParam(string key){if(WeaponDefinitions.TryGetValue(key,out _weaponInfo)){_myWeaponRangeToFire=(float)_weaponInfo.Range;
_myWeaponShotVelocity=(float)_weaponInfo.StartSpeed;_myWeaponReloadTime=(float)_weaponInfo.ReloadTime;}}bool IniToColor(MyIniValue val,ref
Color c){string rgbString=val.ToString("");string[]rgbSplit=rgbString.Split(',');int r=0,g=0,b=0,a=0;if(rgbSplit.Length!=4||!
int.TryParse(rgbSplit[0].Trim(),out r)||!int.TryParse(rgbSplit[1].Trim(),out g)||!int.TryParse(rgbSplit[2].Trim(),out b)){
return false;}bool hasAlpha=int.TryParse(rgbSplit[3].Trim(),out a);if(!hasAlpha){a=255;}r=MathHelper.Clamp(r,0,255);g=
MathHelper.Clamp(g,0,255);b=MathHelper.Clamp(b,0,255);a=MathHelper.Clamp(a,0,255);c=new Color(r,g,b,a);return true;}string
ColorToString(Color Value){return string.Format("{0}, {1}, {2}, {3}",Value.R,Value.G,Value.B,Value.A);}
}class BroadcastModule{IMyBroadcastListener _mListener,_aListener;IMyIntergridCommunicationSystem _IGC;public
BroadcastModule(IMyIntergridCommunicationSystem IGC,string _IGCAxisTag,string _IGCAshTag){_IGC=IGC;_mListener=_IGC.
RegisterBroadcastListener(_IGCAxisTag);_aListener=_IGC.RegisterBroadcastListener(_IGCAshTag);}public void SendLaunchMessage(long tick,string tag,
string i,EnemyTargetedInfo target,IMyTerminalBlock refBlock){if(target!=null){var container=target.CreateMessage(tick,i);_IGC.
SendBroadcastMessage(tag,container,TransmissionDistance.CurrentConstruct);foreach(var s in target.TargetSubsystems){var info=new MyTuple<
long,string,Vector3D>(target.EntityId,s.subsystemType,s.gridPosition);_IGC.SendBroadcastMessage(tag,info,
TransmissionDistance.CurrentConstruct);}}else if(refBlock!=null)SendLaunchMessage(tag,refBlock.WorldMatrix.Forward);}public void SendInfo(
long tick,string tag,EnemyTargetedInfo target){if(target!=null){var container=target.CreateMessage(tick);_IGC.
SendBroadcastMessage(tag,container,TransmissionDistance.CurrentConstruct);}}public void SendInfo(long tick,long myId,string tag,
EnemyTargetedInfo target){if(target!=null){var c=target.CreateMessage(tick);var c2=new MyTuple<long,long,MatrixD,MatrixD>(myId,c.Item1,c.
Item2,c.Item3);_IGC.SendBroadcastMessage(tag,c2,TransmissionDistance.AntennaRelay);}}public void SendLaunchMessage(string tag
,Vector3D Dir){Vector3D container=Dir;_IGC.SendBroadcastMessage(tag,container,TransmissionDistance.CurrentConstruct);}
public void SendPingAnswer(string tag){_IGC.SendBroadcastMessage(tag,"answer",TransmissionDistance.CurrentConstruct);}public
void SendMyPos(string tag,long id,Vector3D pos,string name){var container=new MyTuple<long,Vector3D,Vector3D,string>(id,pos,
Vector3D.Zero,name);_IGC.SendBroadcastMessage(tag,container,TransmissionDistance.AntennaRelay);}public void GetMessageFromAxis(
long tick,ref long axisTick,string tag,EnemyTargetedInfo target,IMyTerminalBlock refBlock,LinkedList<UnitInfo>missiles){
while(_mListener.HasPendingMessage){MyIGCMessage myIGCMessage=_mListener.AcceptMessage();var Data=myIGCMessage.Data;if(
myIGCMessage.Tag==tag){if(Data is MyTuple<int,string>){}if(Data is string){if((string)Data=="ping"){axisTick=tick;SendPingAnswer(tag
);}}if(Data is MyTuple<string,string>){var m=(MyTuple<string,string>)Data;if(m.Item1=="Launch"){axisTick=tick;
SendLaunchMessage(tick,tag,m.Item2,target,refBlock);}}if(Data is MyTuple<long,long,Vector3D,Vector3D>){var d=(MyTuple<long,long,Vector3D,
Vector3D>)Data;bool b=false;foreach(var m in missiles){if(m.id==d.Item1){m.Update(tick,d.Item2,d.Item3,d.Item4);b=true;break;}}
if(!b)missiles.AddFirst(new UnitInfo{id=d.Item1,tick=tick,tId=d.Item2,pos=d.Item3,target=d.Item4});}}}var s=missiles.First
;while(s!=null){var s1=s.Next;if(tick-s.Value.tick>5)missiles.Remove(s);s=s1;}}public void GetMessageFromAsh(long tick,
long id,string tag,LinkedList<AllieUnitInfo>allies,LinkedList<AllieTarget>alliesT){while(_aListener.HasPendingMessage){
MyIGCMessage myIGCMessage=_aListener.AcceptMessage();var Data=myIGCMessage.Data;if(myIGCMessage.Tag==tag&&myIGCMessage.Source!=id){
if(Data is MyTuple<long,Vector3D,Vector3D,string>){var d=(MyTuple<long,Vector3D,Vector3D,string>)Data;bool b=false;foreach
(var a in allies){if(a.id==d.Item1){a.Update(tick,0,d.Item2,d.Item3);a.name=d.Item4;b=true;break;}}if(!b)allies.AddFirst(
new AllieUnitInfo{id=d.Item1,tick=tick,pos=d.Item2,target=Vector3D.Zero,name=d.Item4,tId=0});}if(Data is MyTuple<long,long,
MatrixD,MatrixD>){var d=(MyTuple<long,long,MatrixD,MatrixD>)Data;bool b=false;foreach(var t in alliesT){if(t.allieID==d.Item1){
t.Update(tick,d);b=true;break;}}if(!b)alliesT.AddFirst(new AllieTarget(tick,d));}}}var s=allies.First;while(s!=null){var
s1=s.Next;if(tick-s.Value.tick>5)allies.Remove(s);s=s1;}var at=alliesT.First;while(at!=null){var at1=at.Next;if(tick-at.
Value.target.LastLockTick>60)alliesT.Remove(at);at=at1;}}}public class UnitInfo{public long id,tick,tId;public Vector3D pos,
target;public void Update(long tick,long tId,Vector3D p,Vector3D t){pos=p;target=t;this.tick=tick;this.tId=tId;}}public class
AllieUnitInfo:UnitInfo{public string name;}public class AllieTarget{public long allieID{get;private set;}public EnemyTargetedInfo
target{get;private set;}public AllieTarget(long tick,MyTuple<long,long,MatrixD,MatrixD>d){allieID=d.Item1;target=new
EnemyTargetedInfo(tick,d);}public void Update(long tick,MyTuple<long,long,MatrixD,MatrixD>d){target=new EnemyTargetedInfo(tick,d);}}
static class Drawing{static Color _BLACK=new Color(0,0,0,255);static float dz=0.25f-0.005f;static DisplayDef displayDef=new
DisplayDef();static Vector3D cord_lcd;static PlaneD plane;static Vector3D point_on_lcd;static Vector3D delta;static MatrixD m;
static MatrixD mTrans;static Vector3D vectorHudLocal;static Vector2 marker;static RectangleF rect;static float mult;public
static Dictionary<string,DisplayDef>DisplayDefinitions=new Dictionary<string,DisplayDef>(){{"TextPanel/TransparentLCDLarge",
new DisplayDef(){Delta=1.25f,Multiplier=240f,canvasOrientation=new Vector3(1,0,0),fullBlock=false}},{
"TextPanel/HoloLCDLarge",new DisplayDef(){Delta=0.75f,Multiplier=240f,canvasOrientation=new Vector3(1,0,0),fullBlock=false}},{
"TextPanel/LargeFullBlockLCDPanel",new DisplayDef(){Delta=1.25f,Multiplier=240f,canvasOrientation=new Vector3(0,0,1),fullBlock=true}},{
"TextPanel/TransparentLCDSmall",new DisplayDef(){Delta=0.25f-0.005f,Multiplier=1080f,canvasOrientation=new Vector3(1,0,0),fullBlock=false}},{
"TextPanel/SmallFullBlockLCDPanel",new DisplayDef(){Delta=0.25f,Multiplier=1080f,canvasOrientation=new Vector3(0,0,1),fullBlock=true}},{
"TextPanel/HoloLCDSmall",new DisplayDef(){Delta=0.03f,Multiplier=1080f,canvasOrientation=new Vector3(1,0,0.151f),fullBlock=false}},};static bool
PrepeareCoords(IMyTextPanel surface,Vector3D obspos,Vector3D viewvector){displayDef.SetDisplayDef(GetDisplayInfo(surface));mult=
displayDef.Multiplier;dz=displayDef.Delta;cord_lcd=surface.GetPosition()+(surface.WorldMatrix.Forward*displayDef.canvasOrientation
.X+surface.WorldMatrix.Up*displayDef.canvasOrientation.Y+surface.WorldMatrix.Right*displayDef.canvasOrientation.Z)*dz;if(
!displayDef.fullBlock)plane=new PlaneD(cord_lcd,surface.WorldMatrix.Forward);else plane=new PlaneD(cord_lcd,surface.
WorldMatrix.Right);point_on_lcd=plane.Intersection(ref obspos,ref viewvector);delta=point_on_lcd-cord_lcd;m=surface.WorldMatrix;
mTrans=MatrixD.Transpose(m);vectorHudLocal=Vector3D.TransformNormal(delta,mTrans);if(!displayDef.fullBlock)marker=new Vector2(
(float)vectorHudLocal.X,-(float)vectorHudLocal.Y);else marker=new Vector2(-(float)vectorHudLocal.Z,-(float)vectorHudLocal
.Y);rect=new RectangleF((surface.TextureSize-surface.SurfaceSize)/2f,surface.SurfaceSize);if(Math.Abs(marker.X*mult)>(
surface.TextureSize.X/2)||Math.Abs(marker.Y*mult)>(surface.TextureSize.Y/2))return false;return true;}public static void
SetupDrawSurface(IMyTextSurface surface){surface.ScriptBackgroundColor=Color.Black;surface.ContentType=ContentType.SCRIPT;surface.Script
="";}public static void DrawTarget(DrawingInfo dI){Vector3D t=dI.point;Vector3D viewvector=t-dI.obspos;if(!PrepeareCoords
(dI.surface,dI.obspos,viewvector))return;Vector3D lcdToTarget=t-cord_lcd;if(viewvector.Length()>lcdToTarget.Length()){
DrawBoxCorners(dI.c,dI.frame,rect.Center+(marker*mult));}Vector2 delta=new Vector2(25,0);DrawDistance(dI.c,dI.frame,rect.Center+(
marker*mult)+delta,viewvector.Length(),0.7f);}public static void DrawSubsystem(DrawingInfo dI,TargetSubsystem subsystem,Color
c){Vector3D target=subsystem.GetPosition(dI.Target);Vector3D viewvector=target-dI.obspos;if(!PrepeareCoords(dI.surface,dI
.obspos,viewvector))return;Vector3D lcdToTarget=target-cord_lcd;if(viewvector.Length()>lcdToTarget.Length()){DrawPoint(c,
dI.frame,rect.Center+(marker*mult));}}public static void DrawTurretTarget(DrawingInfo dI){Vector3D t=dI.Target.Position;
Vector3D viewvector=t-dI.obspos;if(!PrepeareCoords(dI.surface,dI.obspos,viewvector))return;Vector3D lcdToTarget=t-cord_lcd;if(
viewvector.Length()>lcdToTarget.Length()){DrawPoint(dI.c,dI.frame,rect.Center+(marker*mult));}}public static void
DrawMissileTarget(DrawingInfo dI){Vector3D t=dI.point;if(t==Vector3D.Zero)return;Vector3D viewvector=t-dI.obspos;if(!PrepeareCoords(dI.
surface,dI.obspos,viewvector))return;Vector3D lcdToTarget=t-cord_lcd;if(viewvector.Length()>lcdToTarget.Length()){DrawMT(dI.c,
dI.frame,rect.Center+(marker*mult));}}public static void DrawMissilePos(DrawingInfo dI){Vector3D t=dI.point;Vector3D
viewvector=t-dI.obspos;if(!PrepeareCoords(dI.surface,dI.obspos,viewvector))return;Vector3D lcdToTarget=t-cord_lcd;if(viewvector.
Length()>lcdToTarget.Length()){MPoint(dI.c,dI.frame,rect.Center+(marker*mult));}}public static void DrawAlliePos(DrawingInfo
dI,string name,float range){Vector3D t=dI.point;Vector3D viewvector=t-dI.obspos;if(!PrepeareCoords(dI.surface,dI.obspos,
viewvector))return;Vector3D lcdToTarget=t-cord_lcd;if(viewvector.Length()>lcdToTarget.Length()){DrawAP(dI.c,dI.frame,rect.Center+(
marker*mult),name,range);}}public static void DrawBallisticPoint(DrawingInfo dI,bool distanceB=false){Vector3D viewvector=dI.
point-dI.obspos;if(!PrepeareCoords(dI.surface,dI.obspos,viewvector))return;Vector3D lcdToTarget=dI.point-cord_lcd;if(
viewvector.Length()>lcdToTarget.Length()){Point(dI.c,dI.frame,rect.Center+(marker*mult));if(distanceB){double distance=(dI.point-
dI.obspos).Length();var v=new Vector2(15,0);DrawDistance(dI.c,dI.frame,rect.Center+(marker*mult)+v,distance,0.7f);}}}
public static void BattleInterface(DrawingInfo dI,TankInfo tankInfo,DWI dwi,bool axis,float locked=1.0f,bool isTurret=false,
bool isVeachle=false,bool autoAim=false,bool aimAssist=false){Vector3D viewvector=dI.point;if(!PrepeareCoords(dI.surface,dI.
obspos,viewvector))return;DrawInterface(dI,isTurret,isVeachle);Vector2 statusPos=new Vector2(-40,60);Vector2 infoPos=new
Vector2(-33,40);Vector2 wInfoPos=new Vector2(-200,78);if(locked<0.9f)LosingTarget(dI.c,dI.frame,rect.Center+(marker*mult),
locked,0.7f);if(aimAssist)DrawLockedMode(dI.c,dI.frame,rect.Center+statusPos+(marker*mult),autoAim,0.7f);if(axis){DrawAxisInfo
(dI.c,dI.frame,rect.Center+wInfoPos+(marker*mult),0.7f);}if(dwi.draw)DrawWeaponInfo(dI.c,dwi,dI.frame,rect.Center+
wInfoPos+(marker*mult),0.7f);if(tankInfo.drawTank){Vector2 tankPos=new Vector2(150,150);DrawHull(dI.c,dI.frame,tankPos+rect.
Center+(marker*mult),tankInfo.hullRotation);DrawTurret(dI.c,dI.frame,tankPos+rect.Center+(marker*mult),tankInfo.turretRotation
);DrawTurretInfo(dI.c,dI.frame,tankPos+infoPos+rect.Center+(marker*mult),tankInfo.block,tankInfo.centered);}else{if(
isTurret)DrawTurretInfo(dI.c,dI.frame,infoPos+rect.Center+(marker*mult),tankInfo.block,tankInfo.centered);}}static void
DrawInterface(DrawingInfo dI,bool isTurret,bool isVeachle){string aiMode="";Vector2 logoPos=new Vector2(-200,-200);Vector2 modPos=new
Vector2(100,-200);Vector2 centerPos=rect.Center+logoPos+(marker*mult);DrawLogo(dI.c,dI.frame,centerPos,0.7f);if(isTurret){
aiMode="Turret";}else if(isVeachle){aiMode="Hull";}DrawAiMode(dI.c,dI.frame,aiMode,rect.Center+modPos+(marker*mult),0.7f);}
static void LosingTarget(Color c,MySpriteDrawFrame frame,Vector2 centerPos,float percent,float scale=1f){frame.Add(new
MySprite(0,"SquareSimple",new Vector2(120f,0f)*scale+centerPos,new Vector2(16f,100f)*scale,c,null,TextAlignment.CENTER,0f));
frame.Add(new MySprite(0,"SquareSimple",new Vector2(120f,0f)*scale+centerPos,new Vector2(14f,98f)*scale,_BLACK,null,
TextAlignment.CENTER,0f));frame.Add(new MySprite(0,"SquareSimple",new Vector2(120f,0f+(48*(1-percent)))*scale+centerPos,new Vector2(
12f,96f*percent)*scale,c,null,TextAlignment.CENTER,0f));}static void DrawDistance(Color c,MySpriteDrawFrame frame,Vector2
centerPos,double distance,float scale=1f){if(distance<1000)frame.Add(new MySprite(SpriteType.TEXT,$"{Math.Round(distance,0)} м",
new Vector2(0,0)+centerPos,null,c,"DEBUG",TextAlignment.LEFT,1f*scale));else frame.Add(new MySprite(SpriteType.TEXT,
$"{Math.Round(distance/1000,2)} км",new Vector2(-0,0)+centerPos,null,c,"DEBUG",TextAlignment.LEFT,1f*scale));}public static void DrawSight(DrawingInfo dI,
float scale=1f){Vector3D viewvector=dI.point;if(!PrepeareCoords(dI.surface,dI.obspos,viewvector))return;Vector2 centerPos=
rect.Center+(marker*mult);var c=dI.c;var f=dI.frame;f.Add(new MySprite(0,"SquareSimple",new Vector2(20f,20f)*scale+centerPos
,new Vector2(1f,8f)*scale,c,null,TextAlignment.CENTER,-0.7854f));f.Add(new MySprite(0,"SquareSimple",new Vector2(20f,-20f
)*scale+centerPos,new Vector2(1f,8f)*scale,c,null,TextAlignment.CENTER,0.7854f));f.Add(new MySprite(0,"SquareSimple",new
Vector2(-20f,-20f)*scale+centerPos,new Vector2(1f,8f)*scale,c,null,TextAlignment.CENTER,-0.7854f));f.Add(new MySprite(0,
"SquareSimple",new Vector2(-20f,20f)*scale+centerPos,new Vector2(1f,8f)*scale,c,null,TextAlignment.CENTER,0.7854f));return;}static
void DrawLogo(Color c,MySpriteDrawFrame frame,Vector2 centerPos,float scale=1f){string scriptName="FCS \"Ash\"";frame.Add(
new MySprite(0,"Triangle",new Vector2(145f,17.5f)*scale+centerPos,new Vector2(50f,42f)*scale,c,null,TextAlignment.CENTER,
3.1416f));frame.Add(new MySprite(0,"SquareSimple",new Vector2(68f,15f)*scale+centerPos,new Vector2(156f,36f)*scale,c,null,
TextAlignment.CENTER,0f));frame.Add(new MySprite(0,"Triangle",new Vector2(145f,17.5f)*scale+centerPos,new Vector2(46f,39f)*scale,
_BLACK,null,TextAlignment.CENTER,3.1416f));frame.Add(new MySprite(0,"SquareSimple",new Vector2(68f,15f)*scale+centerPos,new
Vector2(154f,34f)*scale,_BLACK,null,TextAlignment.CENTER,0f));frame.Add(new MySprite(SpriteType.TEXT,scriptName,new Vector2(5f,
0f)*scale+centerPos,null,c,"DEBUG",TextAlignment.LEFT,1f*scale));}static void DrawAiMode(Color c,MySpriteDrawFrame frame,
string text,Vector2 centerPos,float scale=1f){frame.Add(new MySprite(0,"Triangle",new Vector2(-10f,17.5f)*scale+centerPos,new
Vector2(50f,42f)*scale,c,null,TextAlignment.CENTER,3.1416f));frame.Add(new MySprite(0,"SquareSimple",new Vector2(68f,15f)*scale
+centerPos,new Vector2(156f,36f)*scale,c,null,TextAlignment.CENTER,0f));frame.Add(new MySprite(0,"Triangle",new Vector2(-
10f,17.5f)*scale+centerPos,new Vector2(46f,39f)*scale,_BLACK,null,TextAlignment.CENTER,3.1416f));frame.Add(new MySprite(0,
"SquareSimple",new Vector2(68f,15f)*scale+centerPos,new Vector2(153f,34f)*scale,_BLACK,null,TextAlignment.CENTER,0f));frame.Add(new
MySprite(SpriteType.TEXT,text,new Vector2(5f,0f)*scale+centerPos,null,c,"DEBUG",TextAlignment.LEFT,1f*scale));}static void
DrawLockedMode(Color c,MySpriteDrawFrame frame,Vector2 centerPos,bool autotarget,float scale=1f){if(autotarget){frame.Add(new MySprite
(SpriteType.TEXT,"Auto Aim",new Vector2(0f,0f)*scale+centerPos,null,c,"DEBUG",TextAlignment.LEFT,1f*scale));frame.Add(new
MySprite(0,"Circle",new Vector2(-11f,15f)*scale+centerPos,new Vector2(7f,7f)*scale,c,null,TextAlignment.CENTER,0f));}else{frame.
Add(new MySprite(SpriteType.TEXT,"Aim Assist",new Vector2(0f,0f)*scale+centerPos,null,c,"DEBUG",TextAlignment.LEFT,1f*scale
));frame.Add(new MySprite(0,"Circle",new Vector2(-11f,15f)*scale+centerPos,new Vector2(7f,7f)*scale,c,null,TextAlignment.
CENTER,0f));}}static void DrawWeaponInfo(Color c,DWI dwi,MySpriteDrawFrame frame,Vector2 centerPos,float scale=1f){string name
=dwi.name;string weapomType=dwi.weaponDef.type;frame.Add(new MySprite(SpriteType.TEXT,"Weapon:",new Vector2(0f,0f)*scale+
centerPos,null,c,"DEBUG",TextAlignment.LEFT,1f*scale));frame.Add(new MySprite(SpriteType.TEXT,name,new Vector2(-11f,25f)*scale+
centerPos,null,c,"DEBUG",TextAlignment.LEFT,1f*scale));frame.Add(new MySprite(SpriteType.TEXT,weapomType,new Vector2(-11f,50f)*
scale+centerPos,null,c,"DEBUG",TextAlignment.LEFT,1f*scale));frame.Add(new MySprite(0,"Circle",new Vector2(-11f,15f)*scale+
centerPos,new Vector2(7f,7f)*scale,c,null,TextAlignment.CENTER,0f));}static void DrawAxisInfo(Color c,MySpriteDrawFrame frame,
Vector2 centerPos,float scale=1f){frame.Add(new MySprite(SpriteType.TEXT,"MGS \"Axis\"",new Vector2(0f,-25f)*scale+centerPos,
null,c,"DEBUG",TextAlignment.LEFT,1f*scale));frame.Add(new MySprite(0,"Circle",new Vector2(-11f,-10f)*scale+centerPos,new
Vector2(7f,7f)*scale,c,null,TextAlignment.CENTER,0f));}static void DrawPoint(Color c,MySpriteDrawFrame frame,Vector2 centerPos,
float scale=1f){frame.Add(new MySprite(0,"Circle",new Vector2(0f,0f)*scale+centerPos,new Vector2(5f,5f)*scale,c,null,
TextAlignment.CENTER,0f));}static void Point(Color c,MySpriteDrawFrame frame,Vector2 centerPos,float scale=1f){frame.Add(new MySprite
(0,"SquareSimple",new Vector2(0f,0f)*scale+centerPos,new Vector2(1f,1f)*scale,c,null,TextAlignment.CENTER,0f));frame.Add(
new MySprite(0,"SquareSimple",new Vector2(5f,0f)*scale+centerPos,new Vector2(3f,1f)*scale,c,null,TextAlignment.CENTER,0f));
frame.Add(new MySprite(0,"SquareSimple",new Vector2(-5f,0f)*scale+centerPos,new Vector2(3f,1f)*scale,c,null,TextAlignment.
CENTER,0f));frame.Add(new MySprite(0,"SquareSimple",new Vector2(0f,5f)*scale+centerPos,new Vector2(1f,3f)*scale,c,null,
TextAlignment.CENTER,0f));frame.Add(new MySprite(0,"SquareSimple",new Vector2(0f,-5f)*scale+centerPos,new Vector2(1f,3f)*scale,c,null
,TextAlignment.CENTER,0f));}static void MPoint(Color c,MySpriteDrawFrame frame,Vector2 centerPos,float scale=1f){frame.
Add(new MySprite(0,"Triangle",new Vector2(0f,0f)*scale+centerPos,new Vector2(5f,5f)*scale,c,null,TextAlignment.CENTER,0f));
}static void DrawBoxCorners(Color c,MySpriteDrawFrame frame,Vector2 centerPos,float scale=1f){frame.Add(new MySprite(0,
"SquareSimple",new Vector2(15f,13f)+centerPos,new Vector2(1f,5f),c,null,TextAlignment.CENTER,0f));frame.Add(new MySprite(0,
"SquareSimple",new Vector2(13f,15f)+centerPos,new Vector2(5f,1f),c,null,TextAlignment.CENTER,0f));frame.Add(new MySprite(0,
"SquareSimple",new Vector2(15f,-13f)+centerPos,new Vector2(1f,5f),c,null,TextAlignment.CENTER,0f));frame.Add(new MySprite(0,
"SquareSimple",new Vector2(13f,-15f)+centerPos,new Vector2(5f,1f),c,null,TextAlignment.CENTER,0f));frame.Add(new MySprite(0,
"SquareSimple",new Vector2(-15f,13f)+centerPos,new Vector2(1f,5f),c,null,TextAlignment.CENTER,0f));frame.Add(new MySprite(0,
"SquareSimple",new Vector2(-13f,15f)+centerPos,new Vector2(5f,1f),c,null,TextAlignment.CENTER,0f));frame.Add(new MySprite(0,
"SquareSimple",new Vector2(-15f,-13f)+centerPos,new Vector2(1f,5f),c,null,TextAlignment.CENTER,0f));frame.Add(new MySprite(0,
"SquareSimple",new Vector2(-13f,-15f)+centerPos,new Vector2(5f,1f),c,null,TextAlignment.CENTER,0f));}public static bool GetObserverPos
(ref Vector3D?k,ref Vector3D?forwardDirection,float up,float backward,IMyShipController cockpit=null,List<IMyCameraBlock>
all_cams=null){IMyCameraBlock cam=null;foreach(var viewCam in all_cams){if(viewCam.IsActive){cam=viewCam;break;}}if(cam!=null){k
=cam.WorldMatrix.Translation+cam.WorldMatrix.Forward*(cam.CubeGrid.GridSize/2f-0.005f);forwardDirection=cam.WorldMatrix.
Forward;return true;}else if(cockpit!=null&&cockpit.IsUnderControl){k=cockpit.GetPosition()+cockpit.WorldMatrix.Up*(up)+cockpit
.WorldMatrix.Backward*(backward);forwardDirection=cockpit.WorldMatrix.Forward;return true;}else return false;}public
static void DrawMT(Color c,MySpriteDrawFrame f,Vector2 v,float scale=1f){f.Add(new MySprite(0,"SquareSimple",new Vector2(0,0)*
scale+v,new Vector2(1f,1f)*scale,c,null,TextAlignment.CENTER));f.Add(new MySprite(0,"SquareSimple",new Vector2(7f,5f)+v,new
Vector2(1f,5f),c,null,TextAlignment.CENTER,0f));f.Add(new MySprite(0,"SquareSimple",new Vector2(5f,7f)+v,new Vector2(5f,1f),c,
null,TextAlignment.CENTER,0f));f.Add(new MySprite(0,"SquareSimple",new Vector2(-7f,5f)+v,new Vector2(1f,5f),c,null,
TextAlignment.CENTER,0f));f.Add(new MySprite(0,"SquareSimple",new Vector2(-5f,7f)+v,new Vector2(5f,1f),c,null,TextAlignment.CENTER,0f
));f.Add(new MySprite(0,"SquareSimple",new Vector2(7f,-5f)+v,new Vector2(1f,5f),c,null,TextAlignment.CENTER,0f));f.Add(
new MySprite(0,"SquareSimple",new Vector2(5f,-7f)+v,new Vector2(5f,1f),c,null,TextAlignment.CENTER,0f));f.Add(new MySprite(
0,"SquareSimple",new Vector2(-7f,-5f)+v,new Vector2(1f,5f),c,null,TextAlignment.CENTER,0f));f.Add(new MySprite(0,
"SquareSimple",new Vector2(-5f,-7f)+v,new Vector2(5f,1f),c,null,TextAlignment.CENTER,0f));}public static void DrawAP(Color c,
MySpriteDrawFrame f,Vector2 v,string name,float range){f.Add(new MySprite(0,"SquareSimple",new Vector2(10f,0f)+v,new Vector2(2f,22),c,
null,TextAlignment.CENTER,0f));f.Add(new MySprite(0,"SquareSimple",new Vector2(0f,10f)+v,new Vector2(22,2f),c,null,
TextAlignment.CENTER,0f));f.Add(new MySprite(0,"SquareSimple",new Vector2(-10f,0f)+v,new Vector2(2f,22),c,null,TextAlignment.CENTER,
0f));f.Add(new MySprite(0,"SquareSimple",new Vector2(0f,-10f)+v,new Vector2(22,2f),c,null,TextAlignment.CENTER,0f));f.Add(
new MySprite(SpriteType.TEXT,name,new Vector2(14f,-21f)+v,null,c,"DEBUG",TextAlignment.LEFT,1f*0.7f));string r;if(range<
1000)r=$"{Math.Round(range,0)} м";else r=$"{Math.Round(range/1000,2)} км";f.Add(new MySprite(SpriteType.TEXT,r,new Vector2(
14f,-2f)+v,null,c,"DEBUG",TextAlignment.LEFT,1f*0.7f));}public static void DrawTurret(Color c,MySpriteDrawFrame frame,
Vector2 centerPos,float rotation=0f){float sin=(float)Math.Sin(rotation);float cos=(float)Math.Cos(rotation);frame.Add(new
MySprite(0,"Circle",new Vector2(cos*0f-sin*0f,sin*0f+cos*0f)+centerPos,new Vector2(25f,25f),c,null,TextAlignment.CENTER,0f+
rotation));frame.Add(new MySprite(0,"Circle",new Vector2(cos*0f-sin*0f,sin*0f+cos*0f)+centerPos,new Vector2(23f,23f),_BLACK,null
,TextAlignment.CENTER,0f+rotation));frame.Add(new MySprite(0,"SquareSimple",new Vector2(cos*0f-sin*-39f,sin*0f+cos*-39f)+
centerPos,new Vector2(5f,51f),_BLACK,null,TextAlignment.CENTER,0f+rotation));frame.Add(new MySprite(0,"SquareSimple",new Vector2(
cos*0f-sin*-39f,sin*0f+cos*-39f)+centerPos,new Vector2(3f,49f),c,null,TextAlignment.CENTER,0f+rotation));}public static
void DrawHull(Color c,MySpriteDrawFrame frame,Vector2 centerPos,float rotation=0f){float sin=(float)Math.Sin(rotation);float
cos=(float)Math.Cos(rotation);frame.Add(new MySprite(0,"SquareSimple",new Vector2(cos*0f-sin*0f,sin*0f+cos*0f)+centerPos,
new Vector2(40f,60f),c,null,TextAlignment.CENTER,0f+rotation));frame.Add(new MySprite(0,"SquareSimple",new Vector2(cos*0f-
sin*0f,sin*0f+cos*0f)+centerPos,new Vector2(38f,58f),_BLACK,null,TextAlignment.CENTER,0f+rotation));frame.Add(new MySprite(
0,"AH_BoreSight",new Vector2(cos*0f-sin*-40f,sin*0f+cos*-40f)+centerPos,new Vector2(40f,40f),c,null,TextAlignment.CENTER,
-1.5708f+rotation));}public static void DrawTurretInfo(Color c,MySpriteDrawFrame frame,Vector2 centerPos,bool block,bool
centered){string locked="◉ Block";string сentering="↻ Centering";if(block)frame.Add(new MySprite(SpriteType.TEXT,locked,
centerPos,null,c,"DEBUG",TextAlignment.LEFT,0.7f));if(centered)frame.Add(new MySprite(SpriteType.TEXT,сentering,centerPos+new
Vector2(-16,19),null,c,"DEBUG",TextAlignment.LEFT,0.7f));}static DisplayDef GetDisplayInfo(IMyTextPanel surface){DisplayDef
displayD;string key=SystemHelper.GetKey(surface);if(DisplayDefinitions.TryGetValue(key,out displayD))return displayD;if(surface.
CubeGrid.GridSizeEnum==MyCubeSize.Large)return new DisplayDef(){Delta=1.25f,Multiplier=240f,canvasOrientation=new Vector3I(1,0,0
),fullBlock=false};else return new DisplayDef(){Delta=0.25f-0.005f,Multiplier=1080f,canvasOrientation=new Vector3I(1,0,0)
,fullBlock=false};}}public class DrawingInfo{public EnemyTargetedInfo Target;public Vector3D point;public
MySpriteDrawFrame frame;public IMyTextPanel surface;public Vector3D obspos;public Color c;public DrawingInfo(Vector3D point,
MySpriteDrawFrame frame,IMyTextPanel surface,Vector3D obspos,Color c){this.point=point;this.frame=frame;this.surface=surface;this.obspos=
obspos;this.c=c;}public DrawingInfo(EnemyTargetedInfo target,MySpriteDrawFrame frame,IMyTextPanel surface,Vector3D obspos,
Color c){this.Target=target;this.frame=frame;this.surface=surface;this.obspos=obspos;this.c=c;}}public class TankInfo{public
bool block,centered;public float turretRotation;public bool drawTank;public float hullRotation;public TankInfo(float
turretRotation,bool drawTank,float hullRotation,bool block,bool centered){this.block=block;this.centered=centered;this.turretRotation=
turretRotation;this.drawTank=drawTank;this.hullRotation=hullRotation;}}public class DWI{public bool draw;public string name;public
WeaponDef weaponDef;}public class DisplayDef{public float Delta,Multiplier;public Vector3 canvasOrientation;public bool fullBlock
;public void SetDisplayDef(DisplayDef displayDef){Delta=displayDef.Delta;Multiplier=displayDef.Multiplier;
canvasOrientation=displayDef.canvasOrientation;fullBlock=displayDef.fullBlock;}}public class EnemyTargetedInfo{public long EntityId{
private set;get;}public MyDetectedEntityType Type;public Vector3D?TargetedPoint;public Vector3D?HitPosition;public Vector3D
Position;public Vector3D?inBodyPointPosition;public Vector3D?DeltaPosition;public Vector3D Velocity;public Vector3D?Acceleration
;public MatrixD Orientation;public long LastLockTick;public List<TargetSubsystem>TargetSubsystems=new List<
TargetSubsystem>();public List<TargetSubsystem>PowerSubsystems=new List<TargetSubsystem>();public List<TargetSubsystem>PropSubsystems=
new List<TargetSubsystem>();public List<TargetSubsystem>WeaponSubsystems=new List<TargetSubsystem>();public
EnemyTargetedInfo(long tick,MyDetectedEntityInfo nI,Vector3D?dir=null){EntityId=nI.EntityId;Type=nI.Type;HitPosition=nI.HitPosition;
Position=nI.Position;if(HitPosition!=null){TargetedPoint=HitPosition;DeltaPosition=HitPosition.GetValueOrDefault()-Position;}
Orientation=nI.Orientation;if(TargetedPoint!=null){Vector3D worldDirection=TargetedPoint.GetValueOrDefault()-Position;
inBodyPointPosition=Vector3D.TransformNormal(worldDirection,MatrixD.Transpose(Orientation));if(dir!=null)TargetedPoint+=dir/dir.
GetValueOrDefault().Length()*0.5;}Velocity=nI.Velocity;Acceleration=null;LastLockTick=tick;}public EnemyTargetedInfo(long tick,MyTuple<
long,long,MatrixD,MatrixD>d){var m=d.Item3;Velocity=m.Backward;EntityId=d.Item2;Position=m.Right;Vector3D worldDirection=
Vector3D.TransformNormal(m.Up,Orientation);HitPosition=worldDirection+Position;Orientation=d.Item4;Acceleration=null;
LastLockTick=tick;}public void UpdateTargetInfo(long tick,MyDetectedEntityInfo newEntityInfo,Vector3D?dir=null){EntityId=
newEntityInfo.EntityId;Type=newEntityInfo.Type;HitPosition=newEntityInfo.HitPosition;Position=newEntityInfo.Position;Orientation=
newEntityInfo.Orientation;if(HitPosition!=null){DeltaPosition=HitPosition.GetValueOrDefault()-Position;if(TargetedPoint==null){
TargetedPoint=HitPosition;if(dir!=null)TargetedPoint+=dir/dir.GetValueOrDefault().Length()*0.5;Vector3D worldDirection=TargetedPoint.
GetValueOrDefault()-Position;inBodyPointPosition=Vector3D.TransformNormal(worldDirection,MatrixD.Transpose(Orientation));}}else
DeltaPosition=null;if(inBodyPointPosition!=null){Vector3D worldDirection=Vector3D.TransformNormal(inBodyPointPosition.
GetValueOrDefault(),Orientation);TargetedPoint=worldDirection+Position;}else TargetedPoint=null;if(LastLockTick-tick>60)Acceleration=null
;else Acceleration=(newEntityInfo.Velocity-Velocity)/(LastLockTick-tick);Velocity=newEntityInfo.Velocity;LastLockTick=
tick;}public bool AddSubsystem(long tick,Vector3D worldPosition,string type){TargetSubsystem newSubsystem=new
TargetSubsystem(tick,worldPosition,this,type);return CheckSubsystem(tick,newSubsystem,type);}public bool AddSubsystem(long tick,
MyDetectedEntityInfo newEntityInfo,string type){TargetSubsystem newSubsystem=new TargetSubsystem(tick,newEntityInfo.HitPosition.
GetValueOrDefault(),this,type);return CheckSubsystem(tick,newSubsystem,type);}bool CheckSubsystem(long tick,TargetSubsystem newSubsystem,
string type){float size;if(Type==MyDetectedEntityType.LargeGrid)size=2.5f;else if(Type==MyDetectedEntityType.SmallGrid)size=
0.5f;else return false;foreach(var subsystem in TargetSubsystems){Vector3D delta=subsystem.gridPosition-newSubsystem.
gridPosition;if(delta.Length()/size<1){if(subsystem.subsystemType==newSubsystem.subsystemType){subsystem.lastUpdateTick=tick;return
true;}else return false;}}TargetSubsystems.Add(newSubsystem);if(type=="Weapons")WeaponSubsystems.Add(newSubsystem);if(type==
"Propulsion")PropSubsystems.Add(newSubsystem);if(type=="PowerSystems")PowerSubsystems.Add(newSubsystem);return true;}public MyTuple<
long,MatrixD,MatrixD>CreateMessage(long tick){long TickPassed=tick-LastLockTick;var pos=Position+Velocity*TickPassed/60;
Vector3D vector=inBodyPointPosition.GetValueOrDefault(Vector3D.Zero);MatrixD positions=default(MatrixD);positions.Right=pos;
positions.Up=vector;positions.Backward=Velocity;var message=new MyTuple<long,MatrixD,MatrixD>(EntityId,positions,Orientation);
return message;}public MyTuple<long,MatrixD,MatrixD,string>CreateMessage(long tick,string i){var m=CreateMessage(tick);return
new MyTuple<long,MatrixD,MatrixD,string>(m.Item1,m.Item2,m.Item3,i);}}public class TargetSubsystem{public long
lastUpdateTick;public string subsystemType{get;}public Vector3D gridPosition{get;}public TargetSubsystem(long tick,Vector3D
worldPosition,EnemyTargetedInfo Target,string type){lastUpdateTick=tick;subsystemType=type;Vector3D worldDirection=worldPosition-
Target.Position;gridPosition=Vector3D.TransformNormal(worldDirection,MatrixD.Transpose(Target.Orientation));}public Vector3D
GetPosition(EnemyTargetedInfo Target){Vector3D worldDirection=Vector3D.TransformNormal(gridPosition,Target.Orientation);Vector3D
worldPosition=worldDirection+Target.Position;return worldPosition;}}public class HullGuidance{bool _firstrun=true;const double
TURNGYROCONST=0.0035;const float SOFTNAVCONST=0.8f;Vector3D lastTargetDir=Vector3D.Zero;MatrixD lastShipMatrix=MatrixD.Identity;
MatrixD shipMatrix;double lastPitch=0;double lastYaw=0;double maneuvrabilityYaw=0;double maneuvrabilityPitch=0;bool
fullDriveYaw=false;bool fullDrivePitch=false;bool b;double yawDeltaInput=0;double yawInputSpeed=0;double pitchDeltaInput=0;double
pitchInputSpeed=0;public void Control(IMyShipController shipController,Vector3D targetDir,float rollIndicator,List<IMyGyro>gyros,bool
dir=true,bool autoAim=true,float yawMult=1,float pitchMult=1){if(!dir)targetDir-=shipController.WorldMatrix.Translation;
shipMatrix=shipController.WorldMatrix;Vector3D WorldAngularVelocity=shipController.GetShipVelocities().AngularVelocity;Vector3D
LocalAngularVelocity=Vector3D.TransformNormal(WorldAngularVelocity,MatrixD.Transpose(shipMatrix));double ownYaw=-LocalAngularVelocity.Y/60;
double ownPitch=-LocalAngularVelocity.X/60;double wantedYaw,wantedPitch;double yawSpeed,pitchSpeed;double targetAngularVelYaw,
targetAngularVelPitch;Vector3D targetVecLoc=Vector3D.TransformNormal(targetDir,MatrixD.Transpose(shipMatrix));wantedYaw=Math.Atan2(
targetVecLoc.X,-targetVecLoc.Z);double xyLenght=new Vector2D(targetVecLoc.X,targetVecLoc.Z).Length();if(targetVecLoc.Z>0){xyLenght*=
-1;}wantedPitch=Math.Atan2(-targetVecLoc.Y,xyLenght);if(_firstrun){lastYaw=0;lastPitch=0;lastShipMatrix=shipMatrix;
_firstrun=false;lastTargetDir=targetDir;yawDeltaInput=0;pitchDeltaInput=0;yawInputSpeed=0;pitchInputSpeed=0;if(wantedYaw>0){
fullDriveYaw=true;yawSpeed=ownYaw+TURNGYROCONST;}else yawSpeed=ownYaw-TURNGYROCONST;if(wantedPitch>0){fullDrivePitch=true;pitchSpeed
=ownPitch+TURNGYROCONST;}else pitchSpeed=ownPitch-TURNGYROCONST;}else{Vector3D lastTargetVecLoc=Vector3D.TransformNormal(
lastTargetDir,MatrixD.Transpose(shipMatrix));double lastTargetDirYaw=Math.Atan2(lastTargetVecLoc.X,-lastTargetVecLoc.Z);xyLenght=new
Vector2D(lastTargetVecLoc.X,lastTargetVecLoc.Z).Length();if(lastTargetVecLoc.Z>0){xyLenght*=-1;}double lastTargetDirPitch=Math.
Atan2(-lastTargetVecLoc.Y,xyLenght);targetAngularVelYaw=wantedYaw-lastTargetDirYaw;targetAngularVelPitch=wantedPitch-
lastTargetDirPitch;if(fullDriveYaw)maneuvrabilityYaw=Math.Abs(lastYaw-ownYaw);if(fullDrivePitch)maneuvrabilityPitch=Math.Abs(lastPitch-
ownPitch);fullDriveYaw=false;fullDrivePitch=false;double yawRotationInput=yawMult*shipController.RotationIndicator.Y;double
pitchRotationInput=pitchMult*shipController.RotationIndicator.X;if(!autoAim){if(Math.Abs(yawInputSpeed-yawRotationInput)>maneuvrabilityYaw
){if(yawInputSpeed-yawRotationInput>0)yawInputSpeed-=maneuvrabilityYaw;else yawInputSpeed+=maneuvrabilityYaw;}else{
yawInputSpeed=yawRotationInput;}if(Math.Abs(pitchInputSpeed-pitchRotationInput)>maneuvrabilityPitch){if(pitchInputSpeed-
pitchRotationInput>0)pitchInputSpeed-=maneuvrabilityPitch;else pitchInputSpeed+=maneuvrabilityPitch;}else{pitchInputSpeed=
pitchRotationInput;}yawDeltaInput+=yawInputSpeed;pitchDeltaInput+=pitchInputSpeed;}else{yawRotationInput=0;pitchRotationInput=0;
yawDeltaInput=0;pitchDeltaInput=0;}double timeToStopYaw=Math.Abs((ownYaw-targetAngularVelYaw-yawRotationInput)/maneuvrabilityYaw);
double avaibleDistanceYaw=wantedYaw+yawDeltaInput-ownYaw+(targetAngularVelYaw+yawRotationInput)*timeToStopYaw;double
optimalAngularVelYaw=SOFTNAVCONST*Math.Sqrt(Math.Abs(2*maneuvrabilityYaw*avaibleDistanceYaw));if(wantedYaw+yawDeltaInput<0)
optimalAngularVelYaw*=-1;optimalAngularVelYaw+=targetAngularVelYaw;b=Math.Abs(wantedYaw+yawDeltaInput)>maneuvrabilityYaw;if(b){if(ownYaw<
optimalAngularVelYaw){yawSpeed=ownYaw+TURNGYROCONST;fullDriveYaw=true;}else{yawSpeed=ownYaw-TURNGYROCONST;fullDriveYaw=true;}}else{yawSpeed=
wantedYaw+yawDeltaInput+targetAngularVelYaw;fullDriveYaw=false;}double timeToStopPitch=Math.Abs((ownPitch-targetAngularVelPitch-
pitchRotationInput)/maneuvrabilityPitch);double avaibleDistancePitch=wantedPitch+pitchDeltaInput-ownPitch+(targetAngularVelPitch+
pitchRotationInput)*timeToStopPitch;double optimalAngularVelPitch=SOFTNAVCONST*Math.Sqrt(Math.Abs(2*maneuvrabilityPitch*
avaibleDistancePitch));if(wantedPitch+pitchDeltaInput<0)optimalAngularVelPitch*=-1;optimalAngularVelPitch+=targetAngularVelPitch;b=Math.Abs(
wantedPitch+pitchDeltaInput)>maneuvrabilityPitch;if(b){if(ownPitch<optimalAngularVelPitch){pitchSpeed=ownPitch+TURNGYROCONST;
fullDrivePitch=true;}else{pitchSpeed=ownPitch-TURNGYROCONST;fullDrivePitch=true;}}else{pitchSpeed=wantedPitch+pitchDeltaInput+
targetAngularVelPitch;fullDrivePitch=false;}}double rollSpeed=rollIndicator;lastShipMatrix=shipMatrix;lastTargetDir=targetDir;lastYaw=ownYaw;
lastPitch=ownPitch;pitchSpeed*=60;yawSpeed*=60;ApplyGyroOverride(pitchSpeed,yawSpeed,rollSpeed,gyros,shipMatrix);}public void
Drop(List<IMyGyro>gyroList){_firstrun=true;DropGyro(gyroList);yawDeltaInput=0;pitchDeltaInput=0;}public static void DropGyro
(List<IMyGyro>gyroList){foreach(var thisGyro in gyroList){thisGyro.GyroOverride=false;}}public static void
ApplyGyroOverride(double pitchSpeed,double yawSpeed,double rollSpeed,List<IMyGyro>gyroList,MatrixD worldMatrix){var rotationVec=new
Vector3D(pitchSpeed,yawSpeed,rollSpeed);var relativeRotationVec=Vector3D.TransformNormal(rotationVec,worldMatrix);foreach(var
thisGyro in gyroList){var transformedRotationVec=Vector3D.TransformNormal(relativeRotationVec,Matrix.Transpose(thisGyro.
WorldMatrix));thisGyro.Pitch=(float)transformedRotationVec.X;thisGyro.Yaw=(float)transformedRotationVec.Y;thisGyro.Roll=(float)
transformedRotationVec.Z;thisGyro.GyroOverride=true;}}}public static class MyMath{public static Vector3D FindInterceptGVector(Vector3D myPos,
Vector3D MySpeed,EnemyTargetedInfo Target,Vector3D gravity,double projectileSpeed,int targetingPoint=0,bool dir=true){Vector3D
target=Target.Position;if(targetingPoint==0)if(Target.TargetedPoint!=null)target=Target.TargetedPoint.GetValueOrDefault();if(
targetingPoint==2)if(Target.HitPosition!=null)target=Target.HitPosition.GetValueOrDefault();Vector3D targetDirection=target-myPos;
double speed=projectileSpeed;double correctedSpeed=speed;Vector3D sumspeed=Target.Velocity-MySpeed;Vector3D InterceptVector=
FindInterceptVector(myPos,correctedSpeed,target,sumspeed);for(int i=0;i<10;i++){FindGravityCorrection_DirectFire(speed,ref correctedSpeed,
InterceptVector,gravity);InterceptVector=FindInterceptVector(myPos,correctedSpeed,target,sumspeed);}double timeToHit=InterceptVector.
Length()/correctedSpeed;Vector3D yVector=-gravity*timeToHit*timeToHit/2;InterceptVector=InterceptVector+yVector;if(dir)return
InterceptVector;else return InterceptVector+myPos;}public static Vector3D FindBallisticPoint(Vector3D myPos,Vector3D mySpeed,
EnemyTargetedInfo Target,Vector3D grav,Vector3D projectileSpeed,int targetingPoint=0){Vector3D target=Target.Position;if(targetingPoint==
0)if(Target.TargetedPoint!=null)target=Target.TargetedPoint.GetValueOrDefault();if(targetingPoint==2)if(Target.
HitPosition!=null)target=Target.HitPosition.GetValueOrDefault();Vector3D sumSpeed=mySpeed+projectileSpeed-Target.Velocity;Vector3D
dirToTarget=target-myPos;double distanceToTarget=dirToTarget.Length();double projectileSumSpeed=sumSpeed.Length();double
timeToImpact=distanceToTarget/projectileSumSpeed;Vector3D BallisticPoint=myPos+sumSpeed*timeToImpact+grav*timeToImpact*timeToImpact/
2;return BallisticPoint;}public static Vector3D FindBallisticPoint(Vector3D myPos,Vector3D mySpeed,Vector3D Target,
Vector3D grav,Vector3D projectileSpeed){Vector3D sumSpeed=mySpeed+projectileSpeed;Vector3D dirToTarget=Target-myPos;double
distanceToTarget=dirToTarget.Length();double projectileSumSpeed=sumSpeed.Length();double timeToImpact=distanceToTarget/
projectileSumSpeed;Vector3D BallisticPoint=myPos+sumSpeed*timeToImpact+grav*timeToImpact*timeToImpact/2;return BallisticPoint;}static void
FindGravityCorrection_DirectFire(double speed,ref double x,Vector3D targetDir,Vector3D grav){double distanceToTarget=targetDir.Length();double angleCos=
grav.Dot(targetDir)/(grav.Length()*targetDir.Length());double l=targetDir.Length();double g=grav.Length();double a=1;double
b=(2*g*l*angleCos)-(speed*speed);double c=Math.Pow((g*l/2),2);double?resoult1;double?resoult2;double?x1=null;double?x2=
null;QuadraticEquation(a,b,c,out resoult1,out resoult2);if(resoult1!=null)if(resoult1>0){x1=Math.Sqrt(resoult1.
GetValueOrDefault());x=x1.GetValueOrDefault();}if(resoult2!=null)if(resoult2>0){x2=Math.Sqrt(resoult2.GetValueOrDefault());if(x1!=null)if
(x2>x1)x=x2.GetValueOrDefault();}}public static MatrixD CreateLookAtForwardDir(Vector3D cameraPosition,Vector3D
cameraForwardVector,Vector3D suggestedUp){Vector3D up=Vector3D.Cross(Vector3D.Cross(cameraForwardVector,suggestedUp),cameraForwardVector);
Vector3D vector3D=Vector3D.Normalize(-cameraForwardVector);Vector3D vector3D2=Vector3D.Normalize(Vector3D.Cross(up,vector3D));
Vector3D vector=Vector3D.Cross(vector3D,vector3D2);MatrixD result=default(MatrixD);result.Up=vector;result.Right=vector3D2;
result.Backward=vector3D;result.Translation=cameraPosition;return result;}public static MatrixD CreateLookAtUpDir(Vector3D
cameraPosition,Vector3D suggestedForward,Vector3D cameraUpVector){Vector3D cameraForwardVector=Vector3D.Cross(Vector3D.Cross(
cameraUpVector,suggestedForward),cameraUpVector);Vector3D vector3D=Vector3D.Normalize(-cameraForwardVector);Vector3D vector3D2=
Vector3D.Normalize(Vector3D.Cross(cameraUpVector,vector3D));Vector3D vector=Vector3D.Cross(vector3D,vector3D2);MatrixD result=
default(MatrixD);result.Up=vector;result.Right=vector3D2;result.Backward=vector3D;result.Translation=cameraPosition;return
result;}public static Vector3D FindInterceptVector(Vector3D shotOrigin,double shotVel,Vector3D targetOrigin,Vector3D targetVel
){Vector3D toTarget=targetOrigin-shotOrigin;Vector3D dirToTarget=Vector3D.Normalize(toTarget);Vector3D targetVelOrth=
Vector3D.Dot(targetVel,dirToTarget)*dirToTarget;Vector3D targetVelTang=targetVel-targetVelOrth;Vector3D shotVelTang=
targetVelTang;double shotVelSpeed=shotVelTang.Length();if(shotVelSpeed>shotVel){return Vector3D.Normalize(targetVel)*shotVel;}else{
double shotSpeedOrth=Math.Sqrt(shotVel*shotVel-shotVelSpeed*shotVelSpeed);Vector3D shotVelOrth=dirToTarget*shotSpeedOrth;
double timeToHit=toTarget.Length()/(targetVelOrth-shotVelOrth).Length();return(shotVelOrth+shotVelTang).Normalized()*(
timeToHit*shotVel);}}public static void QuadraticEquation(double a,double b,double c,out double?x1,out double?x2){var
discriminant=Math.Pow(b,2)-4*a*c;if(discriminant<0){x1=null;x2=null;}else{if(discriminant==0){x1=-b/(2*a);x2=x1;}else{x1=(-b+Math.
Sqrt(discriminant))/(2*a);x2=(-b-Math.Sqrt(discriminant))/(2*a);}}}public static Vector3D VectorTransform(Vector3D Vec,
MatrixD Orientation){return new Vector3D(Vec.Dot(Orientation.Right),Vec.Dot(Orientation.Up),Vec.Dot(Orientation.Backward));}
public static double CosBetween(Vector3D a,Vector3D b){if(Vector3D.IsZero(a)||Vector3D.IsZero(b))return 0;else return
MathHelper.Clamp(a.Dot(b)/Math.Sqrt(a.LengthSquared()*b.LengthSquared()),-1,1);}public static double CalculateRotorDeviationAngle(
Vector3D forwardVector,MatrixD lastOrientation){var flattenedForwardVector=VectorRejection(forwardVector,lastOrientation.Up);
return VectorAngleBetween(flattenedForwardVector,lastOrientation.Forward)*Math.Sign(flattenedForwardVector.Dot(lastOrientation
.Left));}public static void CalculateYawVelocity(MatrixD turretMatrix,MatrixD turretLastMatrix,out double speed){Vector3D
now=turretMatrix.Forward;var flattenedForwardVector=VectorRejection(now,turretLastMatrix.Up);speed=-VectorAngleBetween(
flattenedForwardVector,turretLastMatrix.Forward)*Math.Sign(flattenedForwardVector.Dot(turretLastMatrix.Left));}public static void
CalculatePitchVelocity(MatrixD weaponMatrix,MatrixD weaponLastMatrix,out double speed){Vector3D now=weaponMatrix.Forward;var
flattenedForwardVector=VectorRejection(now,weaponLastMatrix.Right);speed=-VectorAngleBetween(flattenedForwardVector,weaponLastMatrix.Forward)*
Math.Sign(flattenedForwardVector.Dot(weaponLastMatrix.Down));}public static Vector3D VectorProjection(Vector3D a,Vector3D b)
{return a.Dot(b)/b.LengthSquared()*b;}public static Vector3D VectorRejection(Vector3D a,Vector3D b){if(Vector3D.IsZero(b)
)return Vector3D.Zero;return a-a.Dot(b)/b.LengthSquared()*b;}public static double VectorAngleBetween(Vector3D a,Vector3D
b){if(Vector3D.IsZero(a)||Vector3D.IsZero(b))return 0;else return Math.Acos(MathHelper.Clamp(a.Dot(b)/Math.Sqrt(a.
LengthSquared()*b.LengthSquared()),-1,1));}public static double Vector2AngleBetween(Vector2D a,Vector2D b){if(a.Length()==0||b.Length
()==0)return 0;else return Math.Acos(MathHelper.Clamp(Vector2D.Dot(a,b)/Math.Sqrt(a.LengthSquared()*b.LengthSquared()),-1
,1));}public static double Vector2DeviationFromZero(Vector2D a){if(a.Length()==0)return 0;Vector2D b=new Vector2D(1,0);
return Math.Acos(MathHelper.Clamp(Vector2D.Dot(a,b)/Math.Sqrt(a.LengthSquared()*b.LengthSquared()),-1,1));}}class Radar{public
List<IMyCameraBlock>radarCameras=new List<IMyCameraBlock>();public EnemyTargetedInfo lockedtarget{private set;get;}long
lastRadarLockTick=0;public Vector3D?pointOfLock;public int countOfCameras;public bool Searching=false;public int counter=0;const float
STABLELOCK=1.1f;const float NEWTARGET=0.25f;bool enemy=true;bool neutral=false;bool allie=false;public Radar(List<IMyCameraBlock>
radar){radarCameras=radar;foreach(var camera in radarCameras)camera.EnableRaycast=true;lockedtarget=null;countOfCameras=
radarCameras.Count;}public void SetTargets(bool allieIn,bool neutralIn,bool enemyIn){enemy=enemyIn;neutral=neutralIn;allie=allieIn;}
public bool UpdateTarget(EnemyTargetedInfo newInfo){if(lockedtarget!=null)if(lockedtarget.EntityId==newInfo.EntityId&&
lockedtarget.LastLockTick<newInfo.LastLockTick){lockedtarget.Position=newInfo.Position;lockedtarget.LastLockTick=newInfo.
LastLockTick;lockedtarget.TargetSubsystems=newInfo.TargetSubsystems;lockedtarget.PowerSubsystems=newInfo.PowerSubsystems;
lockedtarget.PropSubsystems=newInfo.PropSubsystems;lockedtarget.WeaponSubsystems=newInfo.WeaponSubsystems;return true;}return false;
}public bool TryLock(long tick,double InitialRange=2000){bool b=false;MyDetectedEntityInfo newDetectedInfo;long
TickPassed=tick-lastRadarLockTick;if(TickPassed>InitialRange*0.03/countOfCameras){var lockcam=GetCameraWithMaxRange(radarCameras);
if(lockcam==null)return b;if(lockcam.CanScan(InitialRange)){newDetectedInfo=lockcam.Raycast(InitialRange,0,0);if(!
newDetectedInfo.IsEmpty()){if(newDetectedInfo.Type==MyDetectedEntityType.SmallGrid||newDetectedInfo.Type==MyDetectedEntityType.
LargeGrid){if((newDetectedInfo.Relationship==MyRelationsBetweenPlayerAndBlock.Enemies)&&enemy||(newDetectedInfo.Relationship==
MyRelationsBetweenPlayerAndBlock.Neutral)&&neutral||(newDetectedInfo.Relationship==MyRelationsBetweenPlayerAndBlock.NoOwnership)&&neutral||(
newDetectedInfo.Relationship==MyRelationsBetweenPlayerAndBlock.FactionShare)&&allie||(newDetectedInfo.Relationship==
MyRelationsBetweenPlayerAndBlock.Friends)&&allie||(newDetectedInfo.Relationship==MyRelationsBetweenPlayerAndBlock.Owner)&&allie){lockedtarget=new
EnemyTargetedInfo(tick,newDetectedInfo,lockcam.WorldMatrix.Forward);b=true;lastRadarLockTick=tick;counter=0;pointOfLock=null;}else
pointOfLock=newDetectedInfo.HitPosition;}else pointOfLock=newDetectedInfo.HitPosition;}}}return b;}public bool Update(long tick,int
unlockTime,double initialRange=2000){bool b=false;counter++;if(countOfCameras<2)return b;if(lockedtarget!=null){long TickPassed=
tick-lastRadarLockTick;Vector3D shift=lockedtarget.Velocity*TickPassed/60;if(shift.Length()<0.002)shift=Vector3D.Zero;if(
TickPassed>(lockedtarget.Position+shift-radarCameras[0].GetPosition()).Length()*0.03/radarCameras.Count*STABLELOCK){if(
radarCameras==null)return b;IMyCameraBlock c=GetCameraWithMaxRange(radarCameras);if(c==null)return b;double TargetDistance=(
lockedtarget.Position+shift-c.GetPosition()).Length()+10d;MyDetectedEntityInfo DetectedEntity;if(c.AvailableScanRange>=
TargetDistance){Vector3D point;Vector3D dir;Vector3D locDir;Vector3D camPos=c.GetPosition();if(lockedtarget.DeltaPosition!=null){point
=lockedtarget.Position+shift+lockedtarget.DeltaPosition.GetValueOrDefault();dir=point-camPos;locDir=Vector3D.
TransformNormal(dir,MatrixD.Transpose(c.WorldMatrix));DetectedEntity=c.Raycast(dir.Length()+10,locDir);if(DetectedEntity.EntityId==
lockedtarget.EntityId)goto UpdateInfo;if(counter>unlockTime*NEWTARGET)if(CheckForNewTarget(DetectedEntity,tick,c.WorldMatrix.Forward
))goto UpdateInfo;}if(lockedtarget.TargetedPoint!=null){Vector3D worldDirection=Vector3D.TransformNormal(lockedtarget.
inBodyPointPosition.GetValueOrDefault(),lockedtarget.Orientation);point=worldDirection+lockedtarget.Position+shift;dir=point-camPos;locDir=
Vector3D.TransformNormal(dir,MatrixD.Transpose(c.WorldMatrix));DetectedEntity=c.Raycast(dir.Length()+10,locDir);if(
DetectedEntity.EntityId==lockedtarget.EntityId){goto UpdateInfo;}if(counter>unlockTime*NEWTARGET)if(CheckForNewTarget(DetectedEntity,
tick,c.WorldMatrix.Forward))goto UpdateInfo;}point=lockedtarget.Position+shift;dir=point-camPos;locDir=Vector3D.
TransformNormal(dir,MatrixD.Transpose(c.WorldMatrix));DetectedEntity=c.Raycast(dir.Length()+10,locDir);if(DetectedEntity.EntityId==
lockedtarget.EntityId)goto UpdateInfo;if(counter>unlockTime*NEWTARGET)if(CheckForNewTarget(DetectedEntity,tick,c.WorldMatrix.Forward
))goto UpdateInfo;}else return b;UpdateInfo:if(DetectedEntity.EntityId==lockedtarget.EntityId){b=true;lastRadarLockTick=
tick;lockedtarget.UpdateTargetInfo(tick,DetectedEntity,c.WorldMatrix.Forward);counter=0;}}if(counter>=unlockTime){counter=0;
lockedtarget=null;}}else if(Searching){b=TryLock(tick,initialRange);}return b;}public void GetTarget(EnemyTargetedInfo target,long
tick){Searching=true;lockedtarget=target;lastRadarLockTick=tick;}bool CheckForNewTarget(MyDetectedEntityInfo newEntity,long
tick,Vector3D viewvec){if(newEntity.Type==MyDetectedEntityType.SmallGrid||newEntity.Type==MyDetectedEntityType.LargeGrid)if(
(newEntity.Relationship==MyRelationsBetweenPlayerAndBlock.Enemies)&&enemy||(newEntity.Relationship==
MyRelationsBetweenPlayerAndBlock.Neutral)&&neutral||(newEntity.Relationship==MyRelationsBetweenPlayerAndBlock.NoOwnership)&&neutral||(newEntity.
Relationship==MyRelationsBetweenPlayerAndBlock.FactionShare)&&allie||(newEntity.Relationship==MyRelationsBetweenPlayerAndBlock.
Friends)&&allie||(newEntity.Relationship==MyRelationsBetweenPlayerAndBlock.Owner)&&allie){lockedtarget=new EnemyTargetedInfo(
tick,newEntity,viewvec);lastRadarLockTick=tick;counter=0;return true;}return false;}public void DropLock(){pointOfLock=null;
lockedtarget=null;Searching=false;}IMyCameraBlock GetCameraWithMaxRange(List<IMyCameraBlock>cameras){double maxRange=0;
IMyCameraBlock maxRangeCamera=null;foreach(var c in cameras){if(c.AvailableScanRange>maxRange){maxRangeCamera=c;maxRange=
maxRangeCamera.AvailableScanRange;}}return maxRangeCamera;}}static class SystemHelper{public static bool AddBlockIfType<T>(
IMyTerminalBlock block,out T orig)where T:class,IMyTerminalBlock{T typedBlock=block as T;orig=typedBlock;if(typedBlock==null)return
false;return true;}public static bool AddToListIfType<T>(IMyTerminalBlock block,List<T>list)where T:class,IMyTerminalBlock{T
typedBlock;return AddToListIfType(block,list,out typedBlock);}public static bool AddToListIfType<T>(IMyTerminalBlock block,List<T>
list,out T typedBlock)where T:class,IMyTerminalBlock{typedBlock=block as T;if(typedBlock!=null){list.Add(typedBlock);return
true;}return false;}public static string GetKey(IMyTerminalBlock block){string wType=block.BlockDefinition.TypeIdString;
wType=wType.Substring(wType.IndexOf('_')+1);string key=wType+"/"+block.BlockDefinition.SubtypeName;return key;}public static
List<string>SplitString(string str){List<string>results=new List<string>();var builder=new StringBuilder();bool quotation=
false;for(int i=0;i<str.Length;i++){char c=str[i];if(!quotation){if(c!=' '&&c!='"'){builder.Append(c);}if(c==' '&&builder.
Length!=0){results.Add(builder.ToString());builder=new StringBuilder();}if(c=='"'){quotation=true;}}else{if(c!='"'){builder.
Append(c);}else{quotation=false;results.Add(builder.ToString());builder=new StringBuilder();}}if(i+1==str.Length&&builder.
Length!=0){results.Add(builder.ToString());continue;}}return results;}}class Turret{const double TURNGYROCONST=0.0035;const
float SOFTNAVCONST=0.8f;IMyMotorStator rotorA;List<IMyMotorStator>rotorsE;IMyMotorStator MainElRotor;List<
IMyUserControllableGun>weapons;List<IMyCameraBlock>radarCameras;List<IMyGyro>_turretGyros=new List<IMyGyro>();List<IMyGyro>_weaponGyros=new
List<IMyGyro>();Vector3D turretFrontVec;Vector3D lastInterceptVector;float MultiplierElevation;public IMyTerminalBlock
referenceBlock=null;public MatrixD turretMatrix{get;private set;}MatrixD weaponMatrix;MatrixD lastWeaponMatrix;MatrixD
lastTurretMatrix;MatrixD lastRotorAMatrix;MatrixD lastRotorEMatrix;static MyIni languageIni=new MyIni();bool firstRunAim=true;double
yawDeltaInput=0;double yawInputSpeed=0;double pitchDeltaInput=0;double pitchInputSpeed=0;double?lastSpeedYaw=null,lastSpeedPitch=null
;double?maneuvrabilityYaw=null;double?maneuvrabilityPitch=null;bool _firstUpdate=true;bool fullDriveYaw=false,
fullDrivePitch=false;bool block=false;public Turret(){_turretGyros=new List<IMyGyro>();radarCameras=new List<IMyCameraBlock>();rotorsE
=new List<IMyMotorStator>();weapons=new List<IMyUserControllableGun>();}public bool UpdateBlocks(IMyMotorStator newRotorA
,List<IMyMotorStator>newRotorsE,IMyMotorStator mainElRotor,List<IMyUserControllableGun>newWeapons,List<IMyCameraBlock>
cameras,List<IMyGyro>gyros){radarCameras=cameras;rotorA=newRotorA;MainElRotor=mainElRotor;rotorsE=newRotorsE;rotorsE.Remove(
mainElRotor);weapons=newWeapons;if(!CheckRefBlock())return false;_turretGyros.Clear();_weaponGyros.Clear();foreach(var g in gyros){
if(g.CubeGrid==rotorA.TopGrid){_turretGyros.Add(g);}if(g.CubeGrid==referenceBlock.CubeGrid)_weaponGyros.Add(g);}
turretFrontVec=referenceBlock.WorldMatrix.Forward;if(_firstUpdate){_firstUpdate=false;lastRotorAMatrix=newRotorA.WorldMatrix;
lastRotorEMatrix=MainElRotor.WorldMatrix;lastWeaponMatrix=MyMath.CreateLookAtForwardDir(referenceBlock.GetPosition(),turretFrontVec,
rotorA.WorldMatrix.Up);lastTurretMatrix=MyMath.CreateLookAtUpDir(rotorA.Top.WorldMatrix.Translation,turretFrontVec,rotorA.
WorldMatrix.Up);}MultiplierElevation=1;float deltaAzimuthCos=(float)rotorA.Top.WorldMatrix.Right.Dot(MainElRotor.WorldMatrix.Up);
Vector3D absUpVec=rotorA.WorldMatrix.Up;Vector3D turretSideVec=MainElRotor.WorldMatrix.Up;Vector3D turretFrontCrossSide=
turretFrontVec.Cross(turretSideVec);if(turretFrontCrossSide.Dot(absUpVec)<0){deltaAzimuthCos+=MathHelper.Pi;MultiplierElevation=-1;}if
(deltaAzimuthCos>1)deltaAzimuthCos=1;if(deltaAzimuthCos<-1)deltaAzimuthCos=-1;return true;}public void Update(Vector3D
interceptVector,bool autoAim=true,float az=0,float el=0,bool dir=true){if(!dir)interceptVector-=referenceBlock.WorldMatrix.Translation;
turretFrontVec=referenceBlock.WorldMatrix.Forward;weaponMatrix=MyMath.CreateLookAtForwardDir(referenceBlock.GetPosition(),
turretFrontVec,rotorA.WorldMatrix.Up);turretMatrix=MyMath.CreateLookAtUpDir(referenceBlock.GetPosition(),turretFrontVec,rotorA.
WorldMatrix.Up);if(block)return;float azError=(float)MyMath.CalculateRotorDeviationAngle(rotorA.WorldMatrix.Forward,
lastRotorAMatrix);float elError=(float)MyMath.CalculateRotorDeviationAngle(MainElRotor.WorldMatrix.Forward,lastRotorEMatrix);double
ownYaw,ownPitch;MyMath.CalculateYawVelocity(turretMatrix,lastTurretMatrix,out ownYaw);MyMath.CalculatePitchVelocity(
weaponMatrix,lastWeaponMatrix,out ownPitch);double yawRotationInput=az;double pitchRotationInput=el;if(autoAim){yawRotationInput=0;
pitchRotationInput=0;yawDeltaInput=0;pitchDeltaInput=0;}else{if(maneuvrabilityYaw!=null&&Math.Abs(yawInputSpeed-yawRotationInput)>
maneuvrabilityYaw){if(yawInputSpeed-yawRotationInput>0)yawInputSpeed-=maneuvrabilityYaw.Value;else yawInputSpeed+=maneuvrabilityYaw.Value
;}else{yawInputSpeed=yawRotationInput;}if(maneuvrabilityPitch!=null&&Math.Abs(pitchInputSpeed-pitchRotationInput)>
maneuvrabilityPitch){if(pitchInputSpeed-pitchRotationInput>0)pitchInputSpeed-=maneuvrabilityPitch.Value;else pitchInputSpeed+=
maneuvrabilityPitch.Value;}else{pitchInputSpeed=pitchRotationInput;}yawDeltaInput+=yawInputSpeed;pitchDeltaInput+=pitchInputSpeed;}Vector3D
targetVecLocTurret=Vector3D.TransformNormal(interceptVector,MatrixD.Transpose(turretMatrix));double wantedYaw=Math.Atan2(
targetVecLocTurret.X,-targetVecLocTurret.Z);Vector3D targetVecLocWeapon=Vector3D.TransformNormal(interceptVector,MatrixD.Transpose(
weaponMatrix));double xyLenght=new Vector2D(targetVecLocWeapon.X,targetVecLocWeapon.Z).Length();double wantedPitch=Math.Atan2(
targetVecLocWeapon.Y,xyLenght);if(firstRunAim){firstRunAim=false;lastInterceptVector=interceptVector;}double targetAngularVelYaw,
targetAngularVelPitch;Vector3D lastTargetVecLocTurret=Vector3D.TransformNormal(lastInterceptVector,MatrixD.Transpose(turretMatrix));double
lastTargetDirYaw=Math.Atan2(lastTargetVecLocTurret.X,-lastTargetVecLocTurret.Z);Vector3D lastTargetVecLocWeapon=Vector3D.TransformNormal
(lastInterceptVector,MatrixD.Transpose(weaponMatrix));xyLenght=new Vector2D(lastTargetVecLocWeapon.X,
lastTargetVecLocWeapon.Z).Length();double lastTargetDirPitch=Math.Atan2(lastTargetVecLocWeapon.Y,xyLenght);targetAngularVelYaw=wantedYaw-
lastTargetDirYaw;targetAngularVelPitch=wantedPitch-lastTargetDirPitch;double yawRotorSpeed,pitchRotorSpeed,yawGyroSpeed,pitchGyroSpeed;
if(fullDriveYaw){fullDriveYaw=false;maneuvrabilityYaw=Math.Abs(lastSpeedYaw.GetValueOrDefault()-ownYaw);}if(
maneuvrabilityYaw==null){if(wantedYaw>0){yawRotorSpeed=MathHelper.Pi/60;yawGyroSpeed=ownYaw+TURNGYROCONST;}else{yawRotorSpeed=-MathHelper
.Pi/60;yawGyroSpeed=ownYaw-TURNGYROCONST;}fullDriveYaw=true;}else{double timeToStopYaw=Math.Abs((ownYaw-
targetAngularVelYaw-yawRotationInput-azError)/maneuvrabilityYaw.Value);double avaibleDistanceYaw=wantedYaw+yawDeltaInput-ownYaw+(
targetAngularVelYaw+yawRotationInput+azError)*timeToStopYaw;double optimalAngularVelYaw=SOFTNAVCONST*Math.Sqrt(Math.Abs(2*maneuvrabilityYaw
.Value*avaibleDistanceYaw));if(wantedYaw+yawDeltaInput<0)optimalAngularVelYaw*=-1;optimalAngularVelYaw+=
targetAngularVelYaw+azError;if(Math.Abs(wantedYaw+yawDeltaInput)>maneuvrabilityYaw){yawRotorSpeed=optimalAngularVelYaw;yawGyroSpeed=ownYaw<
optimalAngularVelYaw?ownYaw+TURNGYROCONST:ownYaw-TURNGYROCONST;fullDriveYaw=true;}else{yawRotorSpeed=wantedYaw+yawDeltaInput+
targetAngularVelYaw+azError;yawGyroSpeed=wantedYaw+yawDeltaInput+targetAngularVelYaw;fullDriveYaw=false;}}if(fullDrivePitch){fullDrivePitch
=false;maneuvrabilityPitch=Math.Abs(lastSpeedPitch.GetValueOrDefault()-ownPitch);}if(maneuvrabilityPitch==null){if(
wantedPitch>0){pitchRotorSpeed=MathHelper.Pi/60;pitchGyroSpeed=ownPitch+TURNGYROCONST;}else{pitchRotorSpeed=-MathHelper.Pi/60;
pitchGyroSpeed=ownPitch-TURNGYROCONST;}fullDrivePitch=true;}else{double timeToStopPitch=Math.Abs((ownPitch-targetAngularVelPitch-
pitchRotationInput-elError)/maneuvrabilityPitch.Value);double avaibleDistancePitch=wantedPitch+pitchDeltaInput-ownPitch+(
targetAngularVelPitch+pitchRotationInput+elError)*timeToStopPitch;double optimalAngularVelPitch=SOFTNAVCONST*Math.Sqrt(Math.Abs(2*
maneuvrabilityPitch.Value*avaibleDistancePitch));if(wantedPitch+pitchDeltaInput<0)optimalAngularVelPitch*=-1;optimalAngularVelPitch+=
targetAngularVelPitch+elError;if(Math.Abs(wantedPitch+pitchDeltaInput)>maneuvrabilityPitch){pitchRotorSpeed=optimalAngularVelPitch;
pitchGyroSpeed=ownPitch<optimalAngularVelPitch?ownPitch+TURNGYROCONST:ownPitch-TURNGYROCONST;fullDrivePitch=true;}else{pitchRotorSpeed
=wantedPitch+pitchDeltaInput+targetAngularVelPitch+elError;pitchGyroSpeed=wantedPitch+pitchDeltaInput+
targetAngularVelPitch;fullDrivePitch=false;}}rotorA.TargetVelocityRad=(float)yawRotorSpeed*60;MainElRotor.TargetVelocityRad=(float)
pitchRotorSpeed*60;foreach(var rotor in rotorsE){if(!rotor.Closed)SetSupprotRotor(rotor,turretFrontVec,(float)pitchRotorSpeed);}
HullGuidance.ApplyGyroOverride(0,yawGyroSpeed*60,0,_turretGyros,turretMatrix);HullGuidance.ApplyGyroOverride(-pitchGyroSpeed*60,
yawGyroSpeed*60,0,_weaponGyros,turretMatrix);lastInterceptVector=interceptVector;LastMatrix(ownYaw,ownPitch);}public void Update(
float az,float el,ref bool centering,float azAngle,float elAngle,bool stab=true){weaponMatrix=MyMath.CreateLookAtForwardDir(
referenceBlock.GetPosition(),turretFrontVec,rotorA.WorldMatrix.Up);turretMatrix=MyMath.CreateLookAtUpDir(rotorA.Top.WorldMatrix.
Translation,turretFrontVec,rotorA.WorldMatrix.Up);turretFrontVec=referenceBlock.WorldMatrix.Forward;if(block)return;if(centering){
fullDriveYaw=false;fullDrivePitch=false;HullGuidance.DropGyro(_turretGyros);HullGuidance.DropGyro(_weaponGyros);if(az==0&&el==0){
float elC=SetRotorAngle(MainElRotor,elAngle);SetRotorAngle(rotorA,azAngle);foreach(var rotor in rotorsE){if(!rotor.Closed)
SetSupprotRotor(rotor,turretFrontVec,elC);}return;}else centering=false;}yawDeltaInput=0;pitchDeltaInput=0;firstRunAim=false;float
azError=0;float elError=0;if(stab){azError=(float)MyMath.CalculateRotorDeviationAngle(rotorA.WorldMatrix.Forward,
lastRotorAMatrix);elError=(float)MyMath.CalculateRotorDeviationAngle(MainElRotor.WorldMatrix.Forward,lastRotorEMatrix);}double ownYaw,
ownPitch;MyMath.CalculateYawVelocity(turretMatrix,lastTurretMatrix,out ownYaw);MyMath.CalculatePitchVelocity(weaponMatrix,
lastWeaponMatrix,out ownPitch);double yawRotationInput=az;double pitchRotationInput=el;float elevation=(float)(elError+
pitchRotationInput);float azimuth=(float)(azError+yawRotationInput);MainElRotor.TargetVelocityRad=MultiplierElevation*elevation*60;rotorA.
TargetVelocityRad=azimuth*60;foreach(var rotor in rotorsE){if(!rotor.Closed)SetSupprotRotor(rotor,turretFrontVec,elevation);}double
yawSpeed=0,pitchSpeed=0;azimuth-=azError;elevation-=elError;if(fullDriveYaw){fullDriveYaw=false;maneuvrabilityYaw=Math.Abs(
lastSpeedYaw.GetValueOrDefault()-ownYaw);}if(maneuvrabilityYaw==null){if(azimuth!=0){if(azimuth>0){fullDriveYaw=true;yawSpeed=ownYaw
+TURNGYROCONST;}else yawSpeed=ownYaw-TURNGYROCONST;fullDriveYaw=true;}}else{if(Math.Abs(ownYaw-azimuth)>maneuvrabilityYaw
){if(ownYaw-azimuth>0)yawSpeed=ownYaw-TURNGYROCONST;else yawSpeed=ownYaw+TURNGYROCONST;fullDriveYaw=true;}else{yawSpeed=
azimuth;}}if(fullDrivePitch){fullDrivePitch=false;maneuvrabilityPitch=Math.Abs(lastSpeedPitch.GetValueOrDefault()-ownPitch);}if
(maneuvrabilityPitch==null){if(elevation!=0){if(elevation>0){pitchSpeed=ownPitch+TURNGYROCONST;}else pitchSpeed=ownPitch-
TURNGYROCONST;fullDrivePitch=true;}}else{if(Math.Abs(ownPitch-elevation)>maneuvrabilityPitch){if(ownPitch-elevation>0)pitchSpeed=
ownPitch-TURNGYROCONST;else pitchSpeed=ownPitch+TURNGYROCONST;fullDrivePitch=true;}else{pitchSpeed=elevation;}}HullGuidance.
ApplyGyroOverride(0,yawSpeed*60,0,_turretGyros,turretMatrix);HullGuidance.ApplyGyroOverride(-pitchSpeed*60,yawSpeed*60,0,_weaponGyros,
turretMatrix);LastMatrix(ownYaw,ownPitch);}void LastMatrix(double ownYaw,double ownPitch){lastSpeedYaw=ownYaw;lastSpeedPitch=
ownPitch;lastTurretMatrix=turretMatrix;lastWeaponMatrix=weaponMatrix;lastRotorAMatrix=rotorA.WorldMatrix;lastRotorEMatrix=
MainElRotor.WorldMatrix;}public void Block(bool b){block=b;rotorA.RotorLock=b;MainElRotor.RotorLock=b;foreach(var r in rotorsE){r.
RotorLock=b;if(b)r.TargetVelocityRad=0;}if(b){fullDriveYaw=false;fullDrivePitch=false;HullGuidance.DropGyro(_turretGyros);
HullGuidance.DropGyro(_weaponGyros);MainElRotor.TargetVelocityRad=0;rotorA.TargetVelocityRad=0;maneuvrabilityYaw=null;
maneuvrabilityPitch=null;lastSpeedYaw=null;lastSpeedPitch=null;}else{lastRotorAMatrix=rotorA.WorldMatrix;lastRotorEMatrix=MainElRotor.
WorldMatrix;lastWeaponMatrix=MyMath.CreateLookAtForwardDir(referenceBlock.GetPosition(),turretFrontVec,rotorA.WorldMatrix.Up);
lastTurretMatrix=MyMath.CreateLookAtUpDir(rotorA.Top.WorldMatrix.Translation,turretFrontVec,rotorA.WorldMatrix.Up);}}public void Status(
ref string statusInfo,string azimuthTag,string elevationTag){statusInfo+=$"\nRotor \"{azimuthTag}\": {rotorA.CustomName}\n"
+$"Main elevation rotor \"{elevationTag}\": {MainElRotor.CustomName}\n"+$"Count of elevation rotors: {rotorsE.Count+1}\n"
+$"Count of weapons: {weapons.Count}\n";}bool SetSupprotRotor(IMyMotorStator rotor,Vector3D direction,float
mainRotTurnSpeed){float localMultiplierElevation=MultiplierElevation;if(rotor.WorldMatrix.Up.Dot(MainElRotor.WorldMatrix.Up)<0){
localMultiplierElevation=-localMultiplierElevation;}Vector3D?frontVec=null;foreach(var gun in weapons)if(gun.CubeGrid==rotor.TopGrid){frontVec=
gun.WorldMatrix.Forward;break;}if(frontVec==null){foreach(var camera in radarCameras)if(camera.CubeGrid==rotor.TopGrid){
frontVec=camera.WorldMatrix.Forward;break;}}if(frontVec==null)return false;Vector3D TargetVectorLoc=MyMath.VectorTransform(
direction,rotor.WorldMatrix.GetOrientation());Vector3D GunVectorLoc=MyMath.VectorTransform(frontVec.GetValueOrDefault(),rotor.
WorldMatrix.GetOrientation());double targetAngleLoc=Math.Atan2(-TargetVectorLoc.X,TargetVectorLoc.Z);double myAngleLoc=Math.Atan2(-
GunVectorLoc.X,GunVectorLoc.Z);float Elevation=(float)(0.1*(targetAngleLoc-myAngleLoc)+localMultiplierElevation*mainRotTurnSpeed);
rotor.TargetVelocityRad=Elevation*60;return true;}public bool TrySetRef(IMyTerminalBlock reference){if(weapons.Contains(
reference)){referenceBlock=reference;return true;}if(radarCameras.Contains(reference)){referenceBlock=reference;return true;}
return false;}static float SetRotorAngle(IMyMotorStator rotor,float degreeAngle){if(!rotor.Closed){float radAngle=(float)(
degreeAngle/180*MathHelper.Pi);float currAngle=rotor.Angle;float angleDiff=radAngle-currAngle;angleDiff%=MathHelper.TwoPi;if(
angleDiff>MathHelper.Pi)angleDiff=-MathHelper.TwoPi+angleDiff;else if(angleDiff<-MathHelper.Pi)angleDiff=MathHelper.TwoPi+
angleDiff;float Elevation=(float)(0.1*angleDiff);rotor.TargetVelocityRad=Elevation*30;return Elevation;}return 0;}bool
CheckRefBlock(){if(referenceBlock==null)return FindRefBlock();if(referenceBlock.Closed)return FindRefBlock();return false;}bool
FindRefBlock(){foreach(var weapon in weapons){if(weapon.CubeGrid==MainElRotor.TopGrid){referenceBlock=weapon;break;}}if(
referenceBlock==null){referenceBlock=radarCameras[0];}if(referenceBlock==null)return false;return true;}}class TurretRadar{public List
<IMyLargeTurretBase>_turrets=new List<IMyLargeTurretBase>();public List<IMyTurretControlBlock>_TCs=new List<
IMyTurretControlBlock>();List<EnemyTargetedInfo>_enemyTargetedInfos=new List<EnemyTargetedInfo>();public long?_lastChangeTick=null;bool
_change=false;int cycle=0;public void UpdateBlocks(List<IMyLargeTurretBase>turrets,List<IMyTurretControlBlock>
turretControlBlocks,bool change=true){_change=change;_turrets=turrets;_TCs=turretControlBlocks;if(_change){foreach(var t in _turrets){
switch(cycle){case 0:t.SetTargetingGroup("Weapons");break;case 1:t.SetTargetingGroup("Propulsion");break;default:t.
SetTargetingGroup("PowerSystems");break;}}foreach(var t in _TCs){switch(cycle){case 0:t.SetTargetingGroup("Weapons");break;case 1:t.
SetTargetingGroup("Propulsion");break;default:t.SetTargetingGroup("PowerSystems");break;}}}}public void Update(long tick,bool b=true){if(
b)_lastChangeTick=null;foreach(var t in _turrets){bool weHaveThisTarget=false;if(t.HasTarget&&!t.GetTargetedEntity().
IsEmpty()){MyDetectedEntityInfo NewTarget=t.GetTargetedEntity();for(int i=0;i<_enemyTargetedInfos.Count;i++){if(NewTarget.
EntityId==_enemyTargetedInfos[i].EntityId){_enemyTargetedInfos[i].UpdateTargetInfo(tick,NewTarget);weHaveThisTarget=true;}}if(!
weHaveThisTarget){EnemyTargetedInfo target=new EnemyTargetedInfo(tick,NewTarget);_enemyTargetedInfos.Add(target);}}}foreach(var t in
_TCs){bool weHaveThisTarget=false;if(t.HasTarget&&!t.GetTargetedEntity().IsEmpty()){MyDetectedEntityInfo NewTarget=t.
GetTargetedEntity();for(int i=0;i<_enemyTargetedInfos.Count;i++){if(NewTarget.EntityId==_enemyTargetedInfos[i].EntityId){
_enemyTargetedInfos[i].UpdateTargetInfo(tick,NewTarget);weHaveThisTarget=true;}}if(!weHaveThisTarget){EnemyTargetedInfo target=new
EnemyTargetedInfo(tick,NewTarget);_enemyTargetedInfos.Add(target);}}}DeleteOldTargets(tick);}public EnemyTargetedInfo Update(long tick,
EnemyTargetedInfo target){Update(tick,false);if(_change){foreach(var t in _turrets){if(t.HasTarget&&!t.GetTargetedEntity().IsEmpty()){
MyDetectedEntityInfo NewSubsystem=t.GetTargetedEntity();if(target.EntityId==NewSubsystem.EntityId){target.AddSubsystem(tick,NewSubsystem,t.
GetTargetingGroup());}}}foreach(var t in _TCs){if(t.HasTarget&&!t.GetTargetedEntity().IsEmpty()){MyDetectedEntityInfo NewSubsystem=t.
GetTargetedEntity();if(target.EntityId==NewSubsystem.EntityId){target.AddSubsystem(tick,NewSubsystem,t.GetTargetingGroup());}}}if(
_lastChangeTick==null){_lastChangeTick=tick;foreach(var t in _turrets){switch(cycle){case 0:t.SetTargetingGroup("Weapons");break;case 1
:t.SetTargetingGroup("Propulsion");break;default:t.SetTargetingGroup("PowerSystems");break;}}foreach(var t in _TCs){
switch(cycle){case 0:t.SetTargetingGroup("Weapons");break;case 1:t.SetTargetingGroup("Propulsion");break;default:t.
SetTargetingGroup("PowerSystems");break;}}}else{if((_lastChangeTick-tick)%120==0){switch(cycle){case 2:cycle=0;break;default:cycle++;
break;}if(_change){foreach(var t in _turrets){switch(cycle){case 0:t.SetTargetingGroup("Weapons");break;case 1:t.
SetTargetingGroup("Propulsion");break;default:t.SetTargetingGroup("PowerSystems");break;}}foreach(var t in _TCs){switch(cycle){case 0:t.
SetTargetingGroup("Weapons");break;case 1:t.SetTargetingGroup("Propulsion");break;default:t.SetTargetingGroup("PowerSystems");break;}}}}}
}return target;}void DeleteOldTargets(long tick){foreach(var t in _enemyTargetedInfos)if(tick-t.LastLockTick>300){
_enemyTargetedInfos.Remove(t);return;}}public List<EnemyTargetedInfo>GetTargets(){return _enemyTargetedInfos;}}public class WeaponDef{
public double Range,StartSpeed;public double ReloadTime;public string type;}class CockpitDef{public float Up,Back;