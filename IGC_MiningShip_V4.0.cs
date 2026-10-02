/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */
const float angleTolerance=0.07f;int counter=0,counter_TargetCargoContainer_Int=0,counter_LocalCargoContainer_Int=0;bool
fromStage2;Vector3D MeAngle;double angle_Pitch_Double,angle_Roll_Double;double powerLevel_Double=0,cargoLevel_Double=0,
fuelLevel_Double=0;bool drillSwitch_Bool=false;MyIni _ini=new MyIni();MyIni DrillPointList=new MyIni();IMyRemoteControl remoteControl=
null;List<IMyShipConnector>connectors=new List<IMyShipConnector>();List<IMyGyro>gyros=new List<IMyGyro>();List<IMyGasTank>
hydrogenTanks=new List<IMyGasTank>();List<IMyGasTank>tanks=new List<IMyGasTank>();List<IMyCargoContainer>cargoContainers=new List<
IMyCargoContainer>();List<IMyShipDrill>drills=new List<IMyShipDrill>();List<IMyThrust>thrusters=new List<IMyThrust>();List<
IMyBatteryBlock>batteries=new List<IMyBatteryBlock>();List<IMyTextPanel>panels=new List<IMyTextPanel>();List<IMyLightingBlock>lights=
new List<IMyLightingBlock>();List<string>spritesList=new List<string>();List<IMyCargoContainer>shipCargoContainers=new List
<IMyCargoContainer>();List<IMyCargoContainer>baseCargoContainers=new List<IMyCargoContainer>();const string
shipInformation_Section="ShipInformation",shipName_Key="ShipName",autoPilotName_Key="AutoPilotName",infoPanel_Key="Info_Panel",
hydrogenThreshold_Key="HydrogenThreshold",batteriesThreshold_Key="batteriesThreshold";const string mission_Section="Mission",iGC_Key="IGC",
online_String="Online",offline_String="Offline",stage_Key="Stage",speedLimit_Key="SpeedLimit",height_Key="Height",height2_Key=
"Height2",depth_Key="Depth",drillPointNum_Key="DrillPointNum",drillPointTotal_Key="DrillPointTotal",yNum_Key="yNum",yNumTemp_Key=
"yNumTemp",drillSpeed_Key="DrillSpeed",interval_Key="Interval",dockingPoint_Section="DockingPoint",
dockingPoint_Reference_to_MainShip_Section="DockingPoint_Reference_to_MainShip",dockingPointDirection_Section="DockingPointDirection",
dockingPointDirection_Reference_to_MainShip_Section="DockingPointDirection_Reference_to_MainShip",approachPoint1_Section="ApproachPoint1",
approachPoint1_Reference_to_MainShip_Section="ApproachPoint1_Reference_to_MainShip",approachPoint2_Section="ApproachPoint2",
approachPoint2_Reference_to_MainShip_Section="ApproachPoint2_Reference_to_MainShip",mineField1_Section="MineField1",mineField2_Section="MineField2",
mineField3_Section="MineField3",mineField_Direction_Section="MineFieldDirection",referenceCoordinate_Forward_Section=
"ReferenceCoordinate_Forward",drillApproachPoint_Section="DrillApproachPoint",drillPoint_Section="DrillPoint",endPoint_Section="EndPoint",ze_Section=
"Ze",x_Key="X",y_Key="Y",z_Key="Z",stage_0="0",stage_1="1",stage_2="2",stage_3="3",stage_3_1="3-1",stage_4="4",stage_5="5",
stage_6="6",stage_7="7",stage_8="8",mode_Key="Mode",mode_stop="stop",mode_depart="depart",mode_back="back",mode_backandstop=
"back and stop",currentPoint_Section="CurrentPoint",isFinished_Key="IsFinished",true_String="True",false_String="False";const string
finalTarget_Section="Final_Target",targetName_Key="TargetName",distance_Key="Distance";const string currentTarget_Section="CurrentTarget";
const string mainShip_RemoteControl_String="MainShip_RemoteControl",mainShip_RemoteControl_Position_Section=
"MainShip_RemoteControl_Position",mainShip_RemoteControl_Forward_Section="MainShip_RemoteControl_Forward",mainShip_RemoteControl_Up_Section=
"MainShip_RemoteControl_Up",modeCommand_String="ModeCommand";MovementControl upDownControl=new MovementControl();MovementControl
forwardBackwardControl=new MovementControl();MovementControl leftRightControl=new MovementControl();Color card_Background_Color_Overall=new
Color(10,20,40);Color font_Color_Overall=new Color(230,255,255);Program(){Runtime.UpdateFrequency=UpdateFrequency.Once|
UpdateFrequency.Update10;DrillPointList.TryParse(Storage);BuildBlockList();SetDefultCustomData();GetRemoteControl();GetThrusters();
Build_SpriteList();}void LightControl(bool enabled){if(lights.Count<1)return;foreach(var light in lights){if(enabled&&!light.Enabled)
light.Enabled=true;else if(!enabled&&light.Enabled)light.Enabled=false;}}void GetRemoteControl(){string autoPilotName_Value=
GetValue_from_CustomData(shipInformation_Section,autoPilotName_Key);List<IMyRemoteControl>remoteControls=new List<IMyRemoteControl>();
GridTerminalSystem.GetBlocksOfType(remoteControls,block=>block.IsSameConstructAs(Me)&&block.CustomName==autoPilotName_Value);if(
remoteControls.Count==0){Echo("Error: Can't find remotecontrol block !!!");}else{foreach(var rc in remoteControls){remoteControl=rc;
upDownControl.remoteControl=remoteControl;forwardBackwardControl.remoteControl=remoteControl;leftRightControl.remoteControl=
remoteControl;}}}void GetThrusters(){List<IMyThrust>thrusters_Temp=new List<IMyThrust>();foreach(var thruster in thrusters){if(
remoteControl.WorldMatrix.Down==thruster.WorldMatrix.Forward)thrusters_Temp.Add(thruster);}upDownControl.thrusters_A=thrusters_Temp;
thrusters_Temp=new List<IMyThrust>();foreach(var thruster in thrusters){if(remoteControl.WorldMatrix.Up==thruster.WorldMatrix.Forward)
thrusters_Temp.Add(thruster);}upDownControl.thrusters_B=thrusters_Temp;thrusters_Temp=new List<IMyThrust>();foreach(var thruster in
thrusters){if(remoteControl.WorldMatrix.Left==thruster.WorldMatrix.Forward)thrusters_Temp.Add(thruster);}leftRightControl.
thrusters_A=thrusters_Temp;thrusters_Temp=new List<IMyThrust>();foreach(var thruster in thrusters){if(remoteControl.WorldMatrix.
Right==thruster.WorldMatrix.Forward)thrusters_Temp.Add(thruster);}leftRightControl.thrusters_B=thrusters_Temp;thrusters_Temp=
new List<IMyThrust>();foreach(var thruster in thrusters){if(remoteControl.WorldMatrix.Forward==thruster.WorldMatrix.Forward
)thrusters_Temp.Add(thruster);}forwardBackwardControl.thrusters_A=thrusters_Temp;thrusters_Temp=new List<IMyThrust>();
foreach(var thruster in thrusters){if(remoteControl.WorldMatrix.Backward==thruster.WorldMatrix.Forward)thrusters_Temp.Add(
thruster);}forwardBackwardControl.thrusters_B=thrusters_Temp;}void BuildBlockList(){GridTerminalSystem.GetBlocksOfType(
connectors,b=>b.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(gyros,b=>b.IsSameConstructAs(Me));GridTerminalSystem.
GetBlocksOfType(hydrogenTanks,b=>b.IsSameConstructAs(Me)&&!b.DefinitionDisplayNameText.ToString().Contains("Oxygen")&&!b.
DefinitionDisplayNameText.ToString().Contains("氧气"));GridTerminalSystem.GetBlocksOfType(tanks,b=>b.IsSameConstructAs(Me));GridTerminalSystem.
GetBlocksOfType(cargoContainers,b=>b.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(drills,b=>b.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(thrusters,b=>b.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(batteries,b=>b.
IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(panels,b=>b.IsSameConstructAs(Me)&&b.CustomName==GetValue_from_CustomData(
shipInformation_Section,infoPanel_Key));GridTerminalSystem.GetBlocksOfType(lights,b=>b.IsSameConstructAs(Me));GridTerminalSystem.
GetBlocksOfType(shipCargoContainers,b=>b.IsSameConstructAs(Me));}void SetDefultCustomData(){WriteDefultItem(shipInformation_Section,
shipName_Key,"MiningShip");WriteDefultItem(shipInformation_Section,autoPilotName_Key,"Remote Control 2");WriteDefultItem(
shipInformation_Section,infoPanel_Key,"Info_Panel");WriteDefultItem(shipInformation_Section,hydrogenThreshold_Key,"30");WriteDefultItem(
shipInformation_Section,batteriesThreshold_Key,"30");WriteDefultItem(mission_Section,iGC_Key,offline_String);WriteDefultItem(mission_Section,
mode_Key,mode_stop);WriteDefultItem(mission_Section,stage_Key,stage_0);WriteDefultItem(mission_Section,speedLimit_Key,"20");
WriteDefultItem(mission_Section,height_Key,"30");WriteDefultItem(mission_Section,height2_Key,"15");WriteDefultItem(mission_Section,
drillSpeed_Key,"0.05");WriteDefultItem(mission_Section,interval_Key,"5");WriteDefultItem(mission_Section,depth_Key,"10");
WriteDefultItem(mission_Section,drillPointNum_Key,"0");WriteDefultItem(mission_Section,drillPointTotal_Key,"0");WriteDefultItem(
dockingPoint_Section,x_Key,"0");WriteDefultItem(dockingPoint_Section,y_Key,"0");WriteDefultItem(dockingPoint_Section,z_Key,"0");
WriteDefultItem(dockingPoint_Reference_to_MainShip_Section,x_Key,"0");WriteDefultItem(dockingPoint_Reference_to_MainShip_Section,y_Key,
"0");WriteDefultItem(dockingPoint_Reference_to_MainShip_Section,z_Key,"0");WriteDefultItem(dockingPointDirection_Section,
x_Key,"0");WriteDefultItem(dockingPointDirection_Section,y_Key,"0");WriteDefultItem(dockingPointDirection_Section,z_Key,"0");
WriteDefultItem(dockingPointDirection_Reference_to_MainShip_Section,x_Key,"0");WriteDefultItem(
dockingPointDirection_Reference_to_MainShip_Section,y_Key,"0");WriteDefultItem(dockingPointDirection_Reference_to_MainShip_Section,z_Key,"0");WriteDefultItem(
approachPoint1_Reference_to_MainShip_Section,x_Key,"0");WriteDefultItem(approachPoint1_Reference_to_MainShip_Section,y_Key,"0");WriteDefultItem(
approachPoint1_Reference_to_MainShip_Section,z_Key,"0");WriteDefultItem(approachPoint1_Section,x_Key,"0");WriteDefultItem(approachPoint1_Section,y_Key,"0");
WriteDefultItem(approachPoint1_Section,z_Key,"0");WriteDefultItem(approachPoint2_Reference_to_MainShip_Section,x_Key,"0");
WriteDefultItem(approachPoint2_Reference_to_MainShip_Section,y_Key,"0");WriteDefultItem(approachPoint2_Reference_to_MainShip_Section,
z_Key,"0");WriteDefultItem(approachPoint2_Section,x_Key,"0");WriteDefultItem(approachPoint2_Section,y_Key,"0");
WriteDefultItem(approachPoint2_Section,z_Key,"0");WriteDefultItem(mineField1_Section,x_Key,"0");WriteDefultItem(mineField1_Section,
y_Key,"0");WriteDefultItem(mineField1_Section,z_Key,"0");WriteDefultItem(mineField2_Section,x_Key,"0");WriteDefultItem(
mineField2_Section,y_Key,"0");WriteDefultItem(mineField2_Section,z_Key,"0");WriteDefultItem(mineField3_Section,x_Key,"0");WriteDefultItem(
mineField3_Section,y_Key,"0");WriteDefultItem(mineField3_Section,z_Key,"0");WriteDefultItem(currentPoint_Section,isFinished_Key,"True");
WriteDefultItem(ze_Section,x_Key,"0");WriteDefultItem(ze_Section,y_Key,"0");WriteDefultItem(ze_Section,z_Key,"0");WriteDefultItem(
finalTarget_Section,targetName_Key,"");WriteDefultItem(finalTarget_Section,x_Key,"0");WriteDefultItem(finalTarget_Section,y_Key,"0");
WriteDefultItem(finalTarget_Section,z_Key,"0");WriteDefultItem(currentTarget_Section,targetName_Key,"");WriteDefultItem(
currentTarget_Section,x_Key,"0");WriteDefultItem(currentTarget_Section,y_Key,"0");WriteDefultItem(currentTarget_Section,z_Key,"0");}void
Build_SpriteList(){if(panels.Count<1){if(Me.SurfaceCount>0){Me.GetSurface(0).GetSprites(spritesList);}}else{panels[0].GetSprites(
spritesList);}}void WriteDefultItem(string section,string key,string value){string valueTemp_String=GetValue_from_CustomData(
section,key);if(valueTemp_String==""){WriteValue_to_CustomData(section,key,value);}}string GetValue_from_CustomData(string
section,string key){_ini=new MyIni();MyIniParseResult result;if(!_ini.TryParse(Me.CustomData,out result))throw new Exception(
result.ToString());string DefaultValue="";return _ini.Get(section,key).ToString(DefaultValue);}string GetValue_from_Storage(
string section,string key){MyIniParseResult result;if(!DrillPointList.TryParse(Storage,out result))throw new Exception(result.
ToString());string DefaultValue="";return DrillPointList.Get(section,key).ToString(DefaultValue);}void WriteValue_to_CustomData(
string section,string key,string value){_ini.Set(section,key,value);Me.CustomData=_ini.ToString();}Vector3D
GetPoint_from_CustomData(string pName){Vector3D coordTemp;Vector3D p1;string x=GetValue_from_CustomData(pName,x_Key);string y=
GetValue_from_CustomData(pName,y_Key);string z=GetValue_from_CustomData(pName,z_Key);double.TryParse(x,out coordTemp.X);double.TryParse(y,out
coordTemp.Y);double.TryParse(z,out coordTemp.Z);p1.X=coordTemp.X;p1.Y=coordTemp.Y;p1.Z=coordTemp.Z;return p1;}Vector3D
GetPoint_from_Storage(string pName){Vector3D coordTemp,p1;string x=GetValue_from_Storage(pName,x_Key);string y=GetValue_from_Storage(pName,
y_Key);string z=GetValue_from_Storage(pName,z_Key);double.TryParse(x,out coordTemp.X);double.TryParse(y,out coordTemp.Y);
double.TryParse(z,out coordTemp.Z);p1.X=coordTemp.X;p1.Y=coordTemp.Y;p1.Z=coordTemp.Z;return p1;}void WritePoint_to_CustomData
(string pName,Vector3D p1){WriteValue_to_CustomData(pName,x_Key,p1.X.ToString());WriteValue_to_CustomData(pName,y_Key,p1.
Y.ToString());WriteValue_to_CustomData(pName,z_Key,p1.Z.ToString());}void ButtonOnShip(string argument){Echo(
$"ButtonOnShip = {argument}");switch(argument){case"1":WriteValue_to_CustomData(mission_Section,mode_Key,mode_stop);GyrosOnOff(true);DrillSwitch(
false);ThrustOverrideRecover();counter_TargetCargoContainer_Int=0;counter_LocalCargoContainer_Int=0;break;case"2":
WriteValue_to_CustomData(mission_Section,mode_Key,mode_depart);counter_LocalCargoContainer_Int=0;break;case"3":WriteValue_to_CustomData(
mission_Section,mode_Key,mode_back);counter_LocalCargoContainer_Int=0;break;case"4":WriteValue_to_CustomData(mission_Section,mode_Key,
mode_backandstop);counter_LocalCargoContainer_Int=0;break;case"6":DrillSwitch();break;case"OKR":WriteValue_to_CustomData(mission_Section
,mode_Key,mode_back);WriteValue_to_CustomData(mission_Section,stage_Key,stage_3);counter_TargetCargoContainer_Int=0;
counter_LocalCargoContainer_Int=0;break;case"L":LockShip();break;case"UL":WriteValue_to_CustomData(mission_Section,stage_Key,stage_1);BatteryCharge(
false);ThrustEnabled(true);TankCharge(false);LightControl(true);GyrosOnOff(true);counter_LocalCargoContainer_Int=0;break;case
"D1":SetD1();break;case"D2":SetD2();break;case"D3":SetD3();break;case"D4":IGCSwitch();break;case"M1":
WritePoint_to_CustomData(mineField1_Section,remoteControl.GetPosition());WritePoint_to_CustomData(referenceCoordinate_Forward_Section,
remoteControl.WorldMatrix.Forward);break;case"M2":WritePoint_to_CustomData(mineField2_Section,remoteControl.GetPosition());break;case
"M3":WritePoint_to_CustomData(mineField3_Section,remoteControl.GetPosition());BuildMineFieldGrid();
CalculateMineFieldDirection();break;case"M4":BuildMineFieldGrid();CalculateMineFieldDirection();break;case"DEFAULT":ResetDefaultCustomData();break;
}}void CheckBlock(){if(remoteControl==null)Echo("Error: Can't find remotecontrol block !!!");}void SetD1(){
WritePoint_to_CustomData(dockingPoint_Section,remoteControl.GetPosition());WritePoint_to_CustomData(dockingPointDirection_Section,remoteControl.
WorldMatrix.Forward);Vector3D mainShip_RemoteControl_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Position_Section);
Vector3D mainShip_RemoteControl_Forward_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Forward_Section);Vector3D
mainShip_RemoteControl_Up_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Up_Section);MatrixD refLookAtMatrix=MatrixD.CreateLookAt(
mainShip_RemoteControl_Vector3D,mainShip_RemoteControl_Forward_Vector3D,mainShip_RemoteControl_Up_Vector3D);Vector3D currentPosition_Vector3D=
remoteControl.GetPosition();Vector3D dockingPoint_Reference_to_MainShip_Vector3D=Vector3D.Transform(currentPosition_Vector3D,
refLookAtMatrix);;Vector3D dockingPointDirection_Reference_to_MainShip_Vector3D=remoteControl.WorldMatrix.Forward;
dockingPointDirection_Reference_to_MainShip_Vector3D=Vector3D.TransformNormal(dockingPointDirection_Reference_to_MainShip_Vector3D,refLookAtMatrix);WritePoint_to_CustomData
(dockingPoint_Reference_to_MainShip_Section,dockingPoint_Reference_to_MainShip_Vector3D);WritePoint_to_CustomData(
dockingPointDirection_Reference_to_MainShip_Section,dockingPointDirection_Reference_to_MainShip_Vector3D);}void SetD2(){WritePoint_to_CustomData(approachPoint1_Section,
remoteControl.GetPosition());Vector3D mainShip_RemoteControl_Vector3D=GetPoint_from_CustomData(
mainShip_RemoteControl_Position_Section);Vector3D mainShip_RemoteControl_Forward_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Forward_Section);
Vector3D mainShip_RemoteControl_Up_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Up_Section);MatrixD refLookAtMatrix=
MatrixD.CreateLookAt(mainShip_RemoteControl_Vector3D,mainShip_RemoteControl_Forward_Vector3D,mainShip_RemoteControl_Up_Vector3D
);Vector3D currentPosition_Vector3D=remoteControl.GetPosition();Vector3D approachPoint1_Reference_to_MainShip_Vector3D=
Vector3D.Transform(currentPosition_Vector3D,refLookAtMatrix);WritePoint_to_CustomData(
approachPoint1_Reference_to_MainShip_Section,approachPoint1_Reference_to_MainShip_Vector3D);}void SetD3(){WritePoint_to_CustomData(approachPoint2_Section,
remoteControl.GetPosition());Vector3D mainShip_RemoteControl_Vector3D=GetPoint_from_CustomData(
mainShip_RemoteControl_Position_Section);Vector3D mainShip_RemoteControl_Forward_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Forward_Section);
Vector3D mainShip_RemoteControl_Up_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Up_Section);MatrixD refLookAtMatrix=
MatrixD.CreateLookAt(mainShip_RemoteControl_Vector3D,mainShip_RemoteControl_Forward_Vector3D,mainShip_RemoteControl_Up_Vector3D
);Vector3D currentPosition_Vector3D=remoteControl.GetPosition();Vector3D approachPoint2_Reference_to_MainShip_Vector3D=
Vector3D.Transform(currentPosition_Vector3D,refLookAtMatrix);WritePoint_to_CustomData(
approachPoint2_Reference_to_MainShip_Section,approachPoint2_Reference_to_MainShip_Vector3D);}void BuildMineFieldGrid(){Vector3D mineField1_Vector3D=
GetPoint_from_CustomData(mineField1_Section);Vector3D mineField2_Vector3D=GetPoint_from_CustomData(mineField2_Section);Vector3D
mineField3_Vector3D=GetPoint_from_CustomData(mineField3_Section);Vector3D xLength_Vector3D=mineField2_Vector3D-mineField1_Vector3D;Vector3D
yLength_Vector3D=mineField3_Vector3D-mineField2_Vector3D;if(yLength_Vector3D.Length()<0.1){mineField3_Vector3D=mineField2_Vector3D+0.1*
yLength_Vector3D;}Vector3D PGravity_Vector3D;remoteControl.TryGetPlanetPosition(out PGravity_Vector3D);Vector3D xe_Vector3D=Vector3D.
Normalize(mineField2_Vector3D-mineField1_Vector3D);Vector3D ye_Vector3D=Vector3D.Normalize(mineField3_Vector3D-
mineField2_Vector3D);Vector3D ze_Vector3D=Vector3D.Normalize(mineField1_Vector3D-PGravity_Vector3D);WritePoint_to_CustomData(ze_Section,
ze_Vector3D);string interval_Value=GetValue_from_CustomData(mission_Section,interval_Key);double itv=double.Parse(interval_Value);
double xNumTemp=Vector3D.Distance(mineField2_Vector3D,mineField1_Vector3D)/itv;double yNumTemp=Vector3D.Distance(
mineField3_Vector3D,mineField2_Vector3D)/itv;int xNum=Convert.ToInt32(Math.Floor(xNumTemp));int yNum=Convert.ToInt32(Math.Floor(yNumTemp));
if(xNumTemp>xNum){if(xNum!=0){xNum=xNum+2;}else{xNum=1;}}if(yNumTemp>yNum){if(yNum!=0){yNum=yNum+2;}else{yNum=1;}}
WriteValue_to_CustomData(mission_Section,yNum_Key,yNum.ToString());WriteValue_to_CustomData(mission_Section,yNumTemp_Key,yNumTemp.ToString());
int x,y,k=0;DrillPointList.Clear();for(y=1;y<=yNum;y++){for(x=1;x<=xNum;x++){k++;Vector3D p_Vector3D;p_Vector3D=
mineField1_Vector3D+(x-1)*itv*xe_Vector3D+(y-1)*itv*ye_Vector3D;DrillPointList.Set(k.ToString(),x_Key,p_Vector3D.X);DrillPointList.Set(k.
ToString(),y_Key,p_Vector3D.Y);DrillPointList.Set(k.ToString(),z_Key,p_Vector3D.Z);}}WriteValue_to_CustomData(mission_Section,
drillPointTotal_Key,k.ToString());WriteValue_to_CustomData(mission_Section,drillPointNum_Key,"1");WriteValue_to_CustomData(
currentPoint_Section,isFinished_Key,true_String);Storage=DrillPointList.ToString();}void CalculateMineFieldDirection(){Vector3D
referenceCoordinate_Forward_Vector3D=GetPoint_from_CustomData(referenceCoordinate_Forward_Section);Vector3D ze_Vector3D=GetPoint_from_CustomData(ze_Section)
;Vector3D mineField1_Vector3D=GetPoint_from_CustomData(mineField1_Section);Vector3D mineField2_Vector3D=
GetPoint_from_CustomData(mineField2_Section);Vector3D mineField_Direction_Vector3D=mineField2_Vector3D-mineField1_Vector3D;MatrixD
refLookAtMatrix=MatrixD.CreateLookAt(new Vector3D(),referenceCoordinate_Forward_Vector3D,ze_Vector3D);mineField_Direction_Vector3D=
Vector3D.TransformNormal(mineField_Direction_Vector3D,refLookAtMatrix);mineField_Direction_Vector3D.Y=0;
mineField_Direction_Vector3D=Vector3D.TransformNormal(mineField_Direction_Vector3D,MatrixD.Transpose(refLookAtMatrix));mineField_Direction_Vector3D=
Vector3D.Normalize(mineField_Direction_Vector3D);WritePoint_to_CustomData(mineField_Direction_Section,
mineField_Direction_Vector3D);}void CalculateDrillApproachPoint(string dpn,string pName,string height_Value){Vector3D p1_Vector3D=
GetPoint_from_Storage(dpn);Vector3D unitVector_Vector3D=GetPoint_from_CustomData(ze_Section);p1_Vector3D+=Convert.ToDouble(height_Value)*
unitVector_Vector3D;WritePoint_to_CustomData(pName,p1_Vector3D);}void CalculateEndPoint(string dpn,string pName){string depth_Value=
GetValue_from_CustomData(mission_Section,depth_Key);double depth_Double=Convert.ToDouble(depth_Value);Vector3D p1_Vector3D=GetPoint_from_Storage
(dpn);Vector3D unitVector_Vector3D=GetPoint_from_CustomData(ze_Section);p1_Vector3D-=depth_Double*unitVector_Vector3D;
WritePoint_to_CustomData(pName,p1_Vector3D);}void DrillSwitch(bool truefalse){foreach(var drill in drills)drill.Enabled=truefalse;}void
DrillSwitch(){if(drillSwitch_Bool)drillSwitch_Bool=false;else drillSwitch_Bool=true;switch(drillSwitch_Bool){case true:DrillSwitch(
true);break;case false:DrillSwitch(false);break;}}void ResetDefaultCustomData(){Me.CustomData="";SetDefultCustomData();}void
LockShip(){foreach(var connector in connectors){connector.Connect();if(connector.Status==MyShipConnectorStatus.Connected){
BatteryCharge(true);ThrustEnabled(false);TankCharge(true);LightControl(false);GyrosOnOff(false);counter_LocalCargoContainer_Int=0;}}}
void BatteryCharge(bool charge){if(batteries.Count==0)return;if(charge){foreach(var battery in batteries){battery.ChargeMode
=ChargeMode.Recharge;}}else{foreach(var battery in batteries){battery.ChargeMode=ChargeMode.Auto;}}}void TankCharge(bool
charge){if(tanks.Count>0){if(charge){foreach(IMyGasTank tank in tanks){if(tank.Stockpile==false){tank.Stockpile=true;}}}else{
foreach(IMyGasTank tank in tanks){if(tank.Stockpile){tank.Stockpile=false;}}}}}void BatteriesStatus(){if(batteries.Count<1){
powerLevel_Double=0;return;}float maxPower=0;float currentPower=0;foreach(var battery in batteries){maxPower+=battery.MaxStoredPower;
currentPower+=battery.CurrentStoredPower;}powerLevel_Double=Math.Round(currentPower/maxPower,2)*100;string stage_Value=
GetValue_from_CustomData(mission_Section,stage_Key);string mode_Value=GetValue_from_CustomData(mission_Section,mode_Key);string
batteriesThreshold_String=GetValue_from_CustomData(shipInformation_Section,batteriesThreshold_Key);double batteriesThreshold_Double=Convert.
ToDouble(batteriesThreshold_String);if(powerLevel_Double<batteriesThreshold_Double&&stage_Value!=stage_0&&mode_Value==
mode_depart){RecordCurrentPoint();WriteValue_to_CustomData(mission_Section,mode_Key,mode_back);}}void CargoStatus(){MyFixedPoint
maxVolume=0;MyFixedPoint currentVolume=0;foreach(var cargoContainer in cargoContainers){maxVolume+=cargoContainer.GetInventory().
MaxVolume;currentVolume+=cargoContainer.GetInventory().CurrentVolume;}cargoLevel_Double=Math.Round((double)currentVolume/(double)
maxVolume,4)*100;string stage_Value=GetValue_from_CustomData(mission_Section,stage_Key);string mode_Value=
GetValue_from_CustomData(mission_Section,mode_Key);if(cargoLevel_Double==100&&stage_Value!=stage_0&&mode_Value==mode_depart){RecordCurrentPoint(
);WriteValue_to_CustomData(mission_Section,mode_Key,mode_back);}}void HydrogenStatus(){fuelLevel_Double=0;if(
hydrogenTanks.Count<1){return;}foreach(var tank in hydrogenTanks)fuelLevel_Double+=tank.FilledRatio;fuelLevel_Double=Math.Round(
fuelLevel_Double/hydrogenTanks.Count*100);string stage_Value=GetValue_from_CustomData(mission_Section,stage_Key);string mode_Value=
GetValue_from_CustomData(mission_Section,mode_Key);string hydrogenThreshold_String=GetValue_from_CustomData(shipInformation_Section,
hydrogenThreshold_Key);double hydrogenThreshold_Double=Convert.ToDouble(hydrogenThreshold_String);if(fuelLevel_Double<
hydrogenThreshold_Double&&stage_Value!=stage_0&&mode_Value==mode_depart){RecordCurrentPoint();WriteValue_to_CustomData(mission_Section,mode_Key,
mode_back);}}bool CheckFull(double level_Double,float threshold_Float){if(level_Double>threshold_Float)return true;else return
false;}bool CheckEmpty(double level_Double,float threshold_Float){if(level_Double<threshold_Float)return true;else return
false;}void IGCSwitch(){string iGCStatus_String=GetValue_from_CustomData(mission_Section,iGC_Key);switch(iGCStatus_String){
case online_String:WriteValue_to_CustomData(mission_Section,iGC_Key,offline_String);break;case offline_String:
WriteValue_to_CustomData(mission_Section,iGC_Key,online_String);break;}}void RecordCurrentPoint(){string stage_String=GetValue_from_CustomData(
mission_Section,stage_Key);string mode_String=GetValue_from_CustomData(mission_Section,mode_Key);if(stage_String==stage_5&&mode_String
==mode_depart){WriteValue_to_CustomData(currentPoint_Section,isFinished_Key,"False");WritePoint_to_CustomData(
currentPoint_Section,remoteControl.GetPosition());}}void ThrustEnabled(bool true_or_false){if(thrusters.Count==0)return;foreach(var thrust
in thrusters){thrust.ThrustOverridePercentage=0;if(true_or_false){thrust.Enabled=true;}else{thrust.Enabled=false;}}}void
ThrustOverrideRecover(){if(thrusters.Count==0)return;foreach(var thrust in thrusters)thrust.ThrustOverridePercentage=0;}void TransferOreIce()
{if(baseCargoContainers==null||baseCargoContainers.Count<1){GridTerminalSystem.GetBlocksOfType(baseCargoContainers,b=>!b.
IsSameConstructAs(Me));counter_TargetCargoContainer_Int=0;counter_LocalCargoContainer_Int=0;}else if(string.IsNullOrEmpty(
baseCargoContainers[0].DisplayNameText)){GridTerminalSystem.GetBlocksOfType(baseCargoContainers,b=>!b.IsSameConstructAs(Me));
counter_TargetCargoContainer_Int=0;counter_LocalCargoContainer_Int=0;}for(int i=1;i<=5;i++){var shipCargoContainer=shipCargoContainers[
counter_LocalCargoContainer_Int];var baseCargoContainer=baseCargoContainers[counter_TargetCargoContainer_Int];List<MyInventoryItem>items=new List<
MyInventoryItem>();shipCargoContainer.GetInventory().GetItems(items);foreach(MyInventoryItem item in items){shipCargoContainer.
GetInventory().TransferItemTo(baseCargoContainer.GetInventory(),item);}counter_LocalCargoContainer_Int++;if(
counter_LocalCargoContainer_Int>=shipCargoContainers.Count){counter_LocalCargoContainer_Int=0;counter_TargetCargoContainer_Int++;if(
counter_TargetCargoContainer_Int>=baseCargoContainers.Count)counter_TargetCargoContainer_Int=0;}}}void AttitudeMeasurement(){Vector3D
planetPosition_Vector3D;remoteControl.TryGetPlanetPosition(out planetPosition_Vector3D);MatrixD refLookAtMatrix=MatrixD.CreateLookAt(new
Vector3D(),remoteControl.WorldMatrix.Forward,remoteControl.WorldMatrix.Up);Vector3D currentPosition_Vector3D=remoteControl.
GetPosition();Vector3D upVector_Vector3D=currentPosition_Vector3D-planetPosition_Vector3D;upVector_Vector3D=Vector3D.Normalize(
upVector_Vector3D);MeAngle=Vector3D.TransformNormal(upVector_Vector3D,refLookAtMatrix);Vector3D up_Vecotr3D=new Vector3D(0,1,0),
pitch_Vecotr3D=new Vector3D(0,MeAngle.Y,MeAngle.Z),roll_Vecotr3D=new Vector3D(MeAngle.X,MeAngle.Y,0);angle_Pitch_Double=-
AngleBetweenVectorsDegrees(up_Vecotr3D,pitch_Vecotr3D)*MeAngle.Z/Math.Abs(MeAngle.Z);angle_Roll_Double=-AngleBetweenVectorsDegrees(up_Vecotr3D,
roll_Vecotr3D)*MeAngle.X/Math.Abs(MeAngle.X);Echo($"Pitch:{Math.Round(angle_Pitch_Double,2)}°");Echo(
$"Roll:{Math.Round(angle_Roll_Double,2)}°");}bool GravityAlignment(float angleTolerance_Float){if(gyros.Count<1)return false;int gyroOverrideCounter_Int=0;float
pitch_Float=0,roll_Float=0;if(Math.Abs(angle_Pitch_Double)>angleTolerance_Float){pitch_Float=Convert.ToSingle(0.5*
angle_Pitch_Double)/180f*Convert.ToSingle(Math.PI);gyroOverrideCounter_Int++;}if(Math.Abs(angle_Roll_Double)>angleTolerance_Float){
roll_Float=Convert.ToSingle(-0.5*angle_Roll_Double)/180f*Convert.ToSingle(Math.PI);gyroOverrideCounter_Int++;}foreach(var gyro in
gyros){string mode_Value=GetValue_from_CustomData(mission_Section,mode_Key);if(mode_Value!=mode_stop){if(pitch_Float!=0){gyro
.GyroOverride=true;gyro.Pitch=pitch_Float;}if(roll_Float!=0){gyro.GyroOverride=true;gyro.Roll=roll_Float;}}else{gyro.Yaw=
0;gyro.Pitch=0;gyro.Roll=0;gyro.GyroOverride=false;}}if(gyroOverrideCounter_Int>0)return true;else return false;}bool
GravityAlignment(float angleTolerance_Float,Vector3D direction_Vector3D){if(gyros.Count<1)return false;int gyroOverrideCounter_Int=0;
MatrixD shipCoordinate_MatrixD=MatrixD.CreateLookAt(new Vector3D(),remoteControl.WorldMatrix.Forward,remoteControl.WorldMatrix.
Up);Vector3D forwardAngle_Vector3D=Vector3D.TransformNormal(direction_Vector3D,shipCoordinate_MatrixD);Vector3D
forward_Vecotr3D=new Vector3D(0,0,-1),yaw_Vecotr3D=new Vector3D(forwardAngle_Vector3D.X,0,forwardAngle_Vector3D.Z);double
angle_Yaw_Double=AngleBetweenVectorsDegrees(forward_Vecotr3D,yaw_Vecotr3D)*forwardAngle_Vector3D.X/Math.Abs(forwardAngle_Vector3D.X);
Echo($"Yaw:{Math.Round(angle_Yaw_Double,2)}°");float pitch_Float=0,roll_Float=0,yaw_Float=0;if(Math.Abs(angle_Pitch_Double)>
angleTolerance_Float){pitch_Float=Convert.ToSingle(0.5*angle_Pitch_Double)/180f*Convert.ToSingle(Math.PI);gyroOverrideCounter_Int++;}if(Math
.Abs(angle_Roll_Double)>angleTolerance_Float){roll_Float=Convert.ToSingle(-0.5*angle_Roll_Double)/180f*Convert.ToSingle(
Math.PI);gyroOverrideCounter_Int++;}if(Math.Abs(angle_Yaw_Double)>angleTolerance_Float){yaw_Float=Convert.ToSingle(0.5*
angle_Yaw_Double)/180f*Convert.ToSingle(Math.PI);gyroOverrideCounter_Int++;}foreach(var gyro in gyros){string mode_Value=
GetValue_from_CustomData(mission_Section,mode_Key);if(mode_Value!=mode_stop){if(pitch_Float!=0){gyro.GyroOverride=true;gyro.Pitch=pitch_Float;}
if(roll_Float!=0){gyro.GyroOverride=true;gyro.Roll=roll_Float;}if(yaw_Float!=0){gyro.GyroOverride=true;gyro.Yaw=yaw_Float;
}}else{gyro.Yaw=0;gyro.Pitch=0;gyro.Roll=0;gyro.GyroOverride=false;}}if(gyroOverrideCounter_Int>0)return true;else return
false;}double AngleBetweenVectors(Vector3D a,Vector3D b){double dotProduct=a.X*b.X+a.Y*b.Y+a.Z*b.Z;double magnitudeA=Math.
Sqrt(a.X*a.X+a.Y*a.Y+a.Z*a.Z);double magnitudeB=Math.Sqrt(b.X*b.X+b.Y*b.Y+b.Z*b.Z);if(magnitudeA==0||magnitudeB==0)return 0;
double cosTheta=dotProduct/(magnitudeA*magnitudeB);cosTheta=Math.Max(-1.0,Math.Min(1.0,cosTheta));return Math.Acos(cosTheta);}
double AngleBetweenVectorsDegrees(Vector3D a,Vector3D b){double radians=AngleBetweenVectors(a,b);return radians*(180.0/Math.PI
);}void GyrosOnOff(bool trueFalse_bool){foreach(var gyro in gyros){if(gyro.Enabled!=trueFalse_bool)gyro.Enabled=
trueFalse_bool;if(trueFalse_bool&&gyro.GyroOverride!=false)gyro.GyroOverride=false;}}void Receive_IGC_Information(){string iGC_String=
GetValue_from_CustomData(mission_Section,iGC_Key);if(iGC_String==offline_String)return;string shipName_String=GetValue_from_CustomData(
shipInformation_Section,shipName_Key);string iGC_Tag_String=shipName_String+mainShip_RemoteControl_String;IMyBroadcastListener
iGC_MainShip_RemoteControl_BroadcastListener=IGC.RegisterBroadcastListener(iGC_Tag_String);while(iGC_MainShip_RemoteControl_BroadcastListener.HasPendingMessage){
MyIGCMessage message=iGC_MainShip_RemoteControl_BroadcastListener.AcceptMessage();TransformMainShipRemoteControlData(message.Data.
ToString());}iGC_Tag_String=shipName_String+modeCommand_String;IMyBroadcastListener iGC_Mode_Command_BroadcastListener=IGC.
RegisterBroadcastListener(shipName_String+modeCommand_String);while(iGC_Mode_Command_BroadcastListener.HasPendingMessage){MyIGCMessage message=
iGC_Mode_Command_BroadcastListener.AcceptMessage();WriteValue_to_CustomData(mission_Section,mode_Key,message.Data.ToString());}}void SendShipStatus(){
string shipName_String=GetValue_from_CustomData(shipInformation_Section,shipName_Key);string mode_String=
GetValue_from_CustomData(mission_Section,mode_Key);string stage_String=GetValue_from_CustomData(mission_Section,stage_Key);string battery_String
=powerLevel_Double.ToString();string fuel_String=fuelLevel_Double.ToString();string cargo_String=cargoLevel_Double.
ToString();string drillPointNum_String=GetValue_from_CustomData(mission_Section,drillPointNum_Key);string drillPointTotal_String
=GetValue_from_CustomData(mission_Section,drillPointTotal_Key);StringBuilder str=new StringBuilder();str.Append(
mode_String);str.Append(":");str.Append(stage_String);str.Append(":");str.Append(battery_String);str.Append(":");str.Append(
fuel_String);str.Append(":");str.Append(cargo_String);str.Append(":");str.Append(drillPointNum_String);str.Append("/");str.Append(
drillPointTotal_String);string iGC_Tag_String=shipName_String+mode_Key;IMyBroadcastListener _myBroadcastListener=IGC.RegisterBroadcastListener
(iGC_Tag_String);IGC.SendBroadcastMessage(iGC_Tag_String,str.ToString());}void TransformMainShipRemoteControlData(string
data_String){string[]array_String=data_String.Split(':');WriteValue_to_CustomData(mainShip_RemoteControl_Position_Section,x_Key,
array_String[0]);WriteValue_to_CustomData(mainShip_RemoteControl_Position_Section,y_Key,array_String[1]);WriteValue_to_CustomData(
mainShip_RemoteControl_Position_Section,z_Key,array_String[2]);WriteValue_to_CustomData(mainShip_RemoteControl_Forward_Section,x_Key,array_String[3]);
WriteValue_to_CustomData(mainShip_RemoteControl_Forward_Section,y_Key,array_String[4]);WriteValue_to_CustomData(
mainShip_RemoteControl_Forward_Section,z_Key,array_String[5]);WriteValue_to_CustomData(mainShip_RemoteControl_Up_Section,x_Key,array_String[6]);
WriteValue_to_CustomData(mainShip_RemoteControl_Up_Section,y_Key,array_String[7]);WriteValue_to_CustomData(mainShip_RemoteControl_Up_Section,
z_Key,array_String[8]);}void CoreLogic(){string mode_Value=GetValue_from_CustomData(mission_Section,mode_Key);switch(
mode_Value){case"depart":DepartSwitch();break;case"back":BackSwitch();break;case"stop":remoteControl.SetAutoPilotEnabled(false);
remoteControl.SetCollisionAvoidance(false);break;case"back and stop":BackSwitch();break;}}void DepartSwitch(){int drillPointNumTemp;
int drillPointTotalTemp;string drillPointNum_Value;string drillPointTotal_Value;string stage_value=GetValue_from_CustomData
(mission_Section,stage_Key);Echo($"Stage {stage_value}");switch(stage_value){case stage_0:Stage0();break;case stage_1:if(
connectors.Count>0){foreach(var connector in connectors){if(connector.Status==MyShipConnectorStatus.Connected)connector.Disconnect
();}}counter_LocalCargoContainer_Int=0;Back_to_ApproachPoint();break;case stage_2:fromStage2=true;Fly_to_ApproachPoint(
approachPoint2_Section,approachPoint2_Reference_to_MainShip_Section,Base6Directions.Direction.Forward,false,5,2,stage_3);break;case stage_3:
LevelFlight(stage_3_1);break;case stage_3_1:Fly_to_DrillApproachPoint(stage_4);break;case stage_4:Down_to_DrillPoint(stage_5);break
;case stage_5:Down_to_EndPoint(stage_6);break;case stage_6:Up_to_DrillPoint(stage_7);break;case stage_7:
Up_to_DrillApproachPoint(stage_8,height2_Key);break;case stage_8:int dpn,dpt;drillPointNum_Value=GetValue_from_CustomData(mission_Section,
drillPointNum_Key);dpn=Convert.ToInt32(drillPointNum_Value);drillPointNumTemp=Convert.ToInt32(drillPointNum_Value)+1;drillPointNum_Value=
drillPointNumTemp.ToString();WriteValue_to_CustomData(mission_Section,drillPointNum_Key,drillPointNum_Value);WriteValue_to_CustomData(
mission_Section,stage_Key,stage_3);drillPointTotal_Value=GetValue_from_CustomData(mission_Section,drillPointTotal_Key);
drillPointTotalTemp=Convert.ToInt32(drillPointTotal_Value);dpt=drillPointTotalTemp;if(drillPointNumTemp>drillPointTotalTemp){
WriteValue_to_CustomData(mission_Section,mode_Key,mode_back);WriteValue_to_CustomData(mission_Section,drillPointNum_Key,"0");}ClearTargetValue()
;break;}}void BackSwitch(){string stage_value=GetValue_from_CustomData(mission_Section,stage_Key);switch(stage_value){
case stage_0:TransferOreIce();ClearTargetValue();string mode_Value=GetValue_from_CustomData(mission_Section,mode_Key);if(
mode_Value==mode_back){WriteValue_to_CustomData(mission_Section,mode_Key,mode_depart);}else if(mode_Value==mode_backandstop){
WriteValue_to_CustomData(mission_Section,mode_Key,mode_stop);BatteryCharge(true);ThrustEnabled(false);LightControl(false);TankCharge(true);}
break;case stage_1:Back_to_DockingPoint();break;case stage_2:Fly_to_ApproachPoint(approachPoint1_Section,
approachPoint1_Reference_to_MainShip_Section,Base6Directions.Direction.Forward,false,5f,2,stage_1);break;case stage_3:string speedLimit_Value=
GetValue_from_CustomData(mission_Section,speedLimit_Key);Fly_to_ApproachPoint(approachPoint2_Section,
approachPoint2_Reference_to_MainShip_Section,Base6Directions.Direction.Forward,false,Convert.ToSingle(speedLimit_Value),10,stage_2);break;case stage_3_1:
WriteValue_to_CustomData(mission_Section,stage_Key,stage_3);break;case stage_4:Up_to_DrillApproachPoint(stage_3,height_Key);break;case stage_5:
Up_to_DrillPoint(stage_4);break;case stage_6:WriteValue_to_CustomData(mission_Section,stage_Key,stage_5);break;case stage_7:
WriteValue_to_CustomData(mission_Section,stage_Key,stage_4);break;case stage_8:WriteValue_to_CustomData(mission_Section,stage_Key,stage_4);break
;}}void StageGo(string targetName,float speedLimit,FlightMode flightMode,Base6Directions.Direction direction,bool
dockingMode,double distanceLimit,string nextStage){Echo($"nextstage = {nextStage}");Vector3D target_Vector3D=
GetPoint_from_CustomData(targetName),relativePosition_Vector3D=target_Vector3D-remoteControl.GetPosition();relativePosition_Vector3D=Vector3D.
TransformNormal(relativePosition_Vector3D,MatrixD.Transpose(remoteControl.WorldMatrix));double distance_Double=Vector3D.Distance(
remoteControl.GetPosition(),target_Vector3D);if(Vector3D.Distance(remoteControl.CurrentWaypoint.Coords,target_Vector3D)>distanceLimit
){remoteControl.ClearWaypoints();remoteControl.AddWaypoint(target_Vector3D,targetName);remoteControl.FlightMode=
flightMode;remoteControl.Direction=direction;remoteControl.SetDockingMode(dockingMode);}if(remoteControl.SpeedLimit!=speedLimit)
remoteControl.SpeedLimit=speedLimit;if(!remoteControl.IsAutoPilotEnabled)remoteControl.SetAutoPilotEnabled(true);WriteTargetValue(
target_Vector3D,targetName,relativePosition_Vector3D,targetName+"_Relative");if(distance_Double<=distanceLimit){
WriteValue_to_CustomData(mission_Section,stage_Key,nextStage);remoteControl.SetAutoPilotEnabled(false);return;}}void WriteTargetValue(Vector3D
target_Vector3D,string targetName_String,Vector3D target_Relative_Vector3D,string target_Relative_Name_String){Vector3D
currentPosition_Vector3D=remoteControl.GetPosition();double targetDistance_Double=Vector3D.Distance(currentPosition_Vector3D,target_Vector3D);
targetDistance_Double=Math.Round(targetDistance_Double,1);double targetRelativeDistance_Double=target_Relative_Vector3D.Length();
targetRelativeDistance_Double=Math.Round(targetRelativeDistance_Double,1);WriteValue_to_CustomData(finalTarget_Section,targetName_Key,
targetName_String);WriteValue_to_CustomData(finalTarget_Section,distance_Key,targetDistance_Double.ToString());WritePoint_to_CustomData(
finalTarget_Section,target_Vector3D);WriteValue_to_CustomData(currentTarget_Section,targetName_Key,target_Relative_Name_String);
WriteValue_to_CustomData(currentTarget_Section,distance_Key,targetRelativeDistance_Double.ToString());WritePoint_to_CustomData(
currentTarget_Section,target_Relative_Vector3D);}void ClearTargetValue(){Vector3D vector_Zero_Vector3D=Vector3D.Zero;WriteValue_to_CustomData
(finalTarget_Section,targetName_Key,"");WriteValue_to_CustomData(finalTarget_Section,distance_Key,"0");
WritePoint_to_CustomData(finalTarget_Section,vector_Zero_Vector3D);WriteValue_to_CustomData(currentTarget_Section,targetName_Key,"");
WriteValue_to_CustomData(currentTarget_Section,distance_Key,"0");WritePoint_to_CustomData(currentTarget_Section,vector_Zero_Vector3D);}void
Stage0(){string stage_value=GetValue_from_CustomData(mission_Section,stage_Key);string drillPointNum_Value=
GetValue_from_CustomData(mission_Section,drillPointNum_Key);BatteryCharge(true);TankCharge(true);ThrustEnabled(false);LightControl(false);
ClearTargetValue();TransferOreIce();remoteControl.SetDockingMode(false);bool powerLevel_Bool=false,fuelLevel_Bool=false,cargoLevel_Bool=
false;if(batteries.Count<1)powerLevel_Bool=true;else if(CheckFull(powerLevel_Double,99))powerLevel_Bool=true;if(hydrogenTanks
.Count<1)fuelLevel_Bool=true;else if(CheckFull(fuelLevel_Double,99))fuelLevel_Bool=true;if(cargoContainers.Count<1)
cargoLevel_Bool=true;else if(CheckEmpty(cargoLevel_Double,1))cargoLevel_Bool=true;if(powerLevel_Bool&&fuelLevel_Bool&&cargoLevel_Bool&&
drillPointNum_Value!="0"){BatteryCharge(false);TankCharge(false);ThrustEnabled(true);LightControl(true);WriteValue_to_CustomData(
mission_Section,stage_Key,stage_1);foreach(var connector in connectors)connector.Disconnect();}else if(drillPointNum_Value=="0"){
WriteValue_to_CustomData(mission_Section,mode_Key,mode_backandstop);}}void LevelFlight(string nextStage){Echo("LevelFlight");string
drillPointNum_Value=GetValue_from_CustomData(mission_Section,drillPointNum_Key);string drillPointTotal_Value=GetValue_from_CustomData(
mission_Section,drillPointTotal_Key);if(Convert.ToInt32(drillPointNum_Value)>Convert.ToInt32(drillPointTotal_Value)||
drillPointNum_Value=="0"){WriteValue_to_CustomData(mission_Section,drillPointNum_Key,"1");}if(fromStage2){string height_Value=
GetValue_from_CustomData(mission_Section,height_Key);CalculateDrillApproachPoint(drillPointNum_Value,drillApproachPoint_Section,height_Value);}
else{string height2_String=GetValue_from_CustomData(mission_Section,height2_Key);CalculateDrillApproachPoint(
drillPointNum_Value,drillApproachPoint_Section,height2_String);}if(GetPoint_from_CustomData(drillApproachPoint_Section).Length()==0||
drillPointNum_Value=="0"){WriteValue_to_CustomData(mission_Section,mode_Key,mode_stop);}string speedLimit_Value=GetValue_from_CustomData(
mission_Section,speedLimit_Key);float speedLimit_Float=Convert.ToSingle(speedLimit_Value);speedLimit_Float=CalculateShipSpeed(
speedLimit_Float);StageGo(drillApproachPoint_Section,speedLimit_Float,FlightMode.OneWay,Base6Directions.Direction.Forward,false,5,
nextStage);}void Down_to_DrillPoint(string nextStage){Echo("Down_to_DrillPoint");DrillSwitch(true);fromStage2=false;string
isFinished_String=GetValue_from_CustomData(currentPoint_Section,isFinished_Key);Vector3D direction_Vector3D=GetPoint_from_CustomData(
mineField_Direction_Section);GravityAlignment(0.17f,direction_Vector3D);if(isFinished_String==true_String){string drillPointNum_Value=
GetValue_from_CustomData(mission_Section,drillPointNum_Key);WritePoint_to_CustomData(drillPoint_Section,GetPoint_from_Storage(
drillPointNum_Value));DrillCycle(drillPoint_Section,10f,1,nextStage,true,1f,1f);}else{DrillCycle(currentPoint_Section,10f,1,nextStage,false
,1f,1f);}}void Down_to_EndPoint(string nextStage){Echo("Down_to_EndPoint");DrillSwitch(true);WriteValue_to_CustomData(
currentPoint_Section,isFinished_Key,"True");Vector3D direction_Vector3D=GetPoint_from_CustomData(mineField_Direction_Section);
GravityAlignment(0.17f,direction_Vector3D);string drillPointNum_Value=GetValue_from_CustomData(mission_Section,drillPointNum_Key);
CalculateEndPoint(drillPointNum_Value,endPoint_Section);string drillSpeed_Value=GetValue_from_CustomData(mission_Section,drillSpeed_Key);
float drillSpeed_Float=Convert.ToSingle(drillSpeed_Value);DrillCycle(endPoint_Section,drillSpeed_Float,1,nextStage,true,0.5f,
0.5f);}void Up_to_DrillPoint(string nextStage){Echo("Up_to_DrillPoint");Vector3D direction_Vector3D=GetPoint_from_CustomData
(mineField_Direction_Section);GravityAlignment(0.17f,direction_Vector3D);string drillPointNum_Value=
GetValue_from_CustomData(mission_Section,drillPointNum_Key);int drillPointNum_Int=Convert.ToInt16(drillPointNum_Value);Echo(
$"drillPointNum_Int={drillPointNum_Int}");WritePoint_to_CustomData(drillPoint_Section,GetPoint_from_Storage(drillPointNum_Int.ToString()));DrillCycle(
drillPoint_Section,4f,1,nextStage,true,0.7f,0.7f);}void Up_to_DrillApproachPoint(string nextStage,string height_String){Echo(
"Up_to_DrillApproachPoint");DrillSwitch(false);Vector3D direction_Vector3D=GetPoint_from_CustomData(mineField_Direction_Section);GravityAlignment(
0.17f,direction_Vector3D);string drillPointNum_Value=GetValue_from_CustomData(mission_Section,drillPointNum_Key);string
height_Value=GetValue_from_CustomData(mission_Section,height_String);CalculateDrillApproachPoint(drillPointNum_Value,
drillApproachPoint_Section,height_Value);DrillCycle(drillApproachPoint_Section,8f,1,nextStage,true,1.5f,1.5f);}void DrillCycle(string targetName,
float speedLimit,double distanceLimit,string nextStage,bool isFinished_Bool,float error_Left_Right_Float=0.2f,float
error_Forward_Backward_Float=0.2f){Echo($"nextstage = {nextStage}");float depth_Float=Convert.ToSingle(GetValue_from_CustomData(mission_Section,
depth_Key));Vector3D ze_Vector3D=GetPoint_from_CustomData(ze_Section),EndPoint_Vector3D=GetPoint_from_CustomData(targetName),
CurrentPosition_Vector3D=remoteControl.GetPosition(),distance_Vector3D=EndPoint_Vector3D-CurrentPosition_Vector3D,relativePosition_Vector3D=
Vector3D.TransformNormal(distance_Vector3D,MatrixD.Transpose(remoteControl.WorldMatrix));if(distance_Vector3D.Length()<2f*
speedLimit||distance_Vector3D.Length()<2f||distance_Vector3D.Length()<depth_Float){speedLimit=0.3f*speedLimit;}Movement_UpDown(
EndPoint_Vector3D,speedLimit,error_Left_Right_Float,error_Forward_Backward_Float);WriteTargetValue(EndPoint_Vector3D,targetName,
relativePosition_Vector3D,"Target_Relative");if(distance_Vector3D.Length()<distanceLimit){WriteValue_to_CustomData(mission_Section,stage_Key,
nextStage);ThrustEnabled(true);GyrosOnOff(true);return;}}void Back_to_DockingPoint(){Echo("Back_to_DockingPoint");string
iGC_Status=GetValue_from_CustomData(mission_Section,iGC_Key);if(iGC_Status==online_String){Vector3D
mainShip_RemoteControl_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Position_Section),mainShip_RemoteControl_Forward_Vector3D=
GetPoint_from_CustomData(mainShip_RemoteControl_Forward_Section),mainShip_RemoteControl_Up_Vector3D=GetPoint_from_CustomData(
mainShip_RemoteControl_Up_Section);MatrixD refLookAtMatrix=MatrixD.CreateLookAt(mainShip_RemoteControl_Vector3D,mainShip_RemoteControl_Forward_Vector3D,
mainShip_RemoteControl_Up_Vector3D);Vector3D dockingPoint_Reference_to_MainShip_Vector3D=GetPoint_from_CustomData(
dockingPoint_Reference_to_MainShip_Section);dockingPoint_Reference_to_MainShip_Vector3D=Vector3D.TransformNormal(dockingPoint_Reference_to_MainShip_Vector3D,
MatrixD.Transpose(refLookAtMatrix));dockingPoint_Reference_to_MainShip_Vector3D+=mainShip_RemoteControl_Vector3D;
WritePoint_to_CustomData(dockingPoint_Section,dockingPoint_Reference_to_MainShip_Vector3D);Vector3D dockingPointDirection_Reference_Vector3D=
GetPoint_from_CustomData(dockingPointDirection_Reference_to_MainShip_Section);dockingPointDirection_Reference_Vector3D=Vector3D.TransformNormal(
dockingPointDirection_Reference_Vector3D,MatrixD.Transpose(refLookAtMatrix));WritePoint_to_CustomData(dockingPointDirection_Section,
dockingPointDirection_Reference_Vector3D);}Vector3D target_Vector3D=GetPoint_from_CustomData(dockingPoint_Section),forward_Vector3D=GetPoint_from_CustomData(
dockingPointDirection_Section),overShootTarget_Vector3D=target_Vector3D+forward_Vector3D,target_Relative_Vector3D=overShootTarget_Vector3D-
remoteControl.GetPosition();target_Relative_Vector3D=Vector3D.TransformNormal(target_Relative_Vector3D,MatrixD.Transpose(
remoteControl.WorldMatrix));GravityAlignment(0.17f,forward_Vector3D);if(Vector3D.Distance(remoteControl.GetPosition(),
overShootTarget_Vector3D)>=4){Movement_ForwardBackward(overShootTarget_Vector3D,3f);}else{Movement_ForwardBackward(overShootTarget_Vector3D,0.2f
);foreach(var connector in connectors){connector.Connect();switch(connector.Status){case MyShipConnectorStatus.
Unconnected:break;case MyShipConnectorStatus.Connectable:break;case MyShipConnectorStatus.Connected:WriteValue_to_CustomData(
mission_Section,stage_Key,stage_0);ThrustEnabled(true);GyrosOnOff(false);break;}}}WriteTargetValue(overShootTarget_Vector3D,"Target",
target_Relative_Vector3D,"Target_Relative");}void Back_to_ApproachPoint(){Echo("Back_to_ApproachPoint");string iGC_Status=
GetValue_from_CustomData(mission_Section,iGC_Key);if(iGC_Status==online_String){Vector3D mainShip_RemoteControl_Vector3D=
GetPoint_from_CustomData(mainShip_RemoteControl_Position_Section),mainShip_RemoteControl_Forward_Vector3D=GetPoint_from_CustomData(
mainShip_RemoteControl_Forward_Section),mainShip_RemoteControl_Up_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Up_Section);MatrixD refLookAtMatrix
=MatrixD.CreateLookAt(mainShip_RemoteControl_Vector3D,mainShip_RemoteControl_Forward_Vector3D,
mainShip_RemoteControl_Up_Vector3D);Vector3D approachPoint_Reference_to_MainShip_Vector3D=GetPoint_from_CustomData(
approachPoint1_Reference_to_MainShip_Section);approachPoint_Reference_to_MainShip_Vector3D=Vector3D.TransformNormal(approachPoint_Reference_to_MainShip_Vector3D,
MatrixD.Transpose(refLookAtMatrix));approachPoint_Reference_to_MainShip_Vector3D+=mainShip_RemoteControl_Vector3D;
WritePoint_to_CustomData(approachPoint1_Section,approachPoint_Reference_to_MainShip_Vector3D);Vector3D dockingPointDirection_Reference_Vector3D=
GetPoint_from_CustomData(dockingPointDirection_Reference_to_MainShip_Section);dockingPointDirection_Reference_Vector3D=Vector3D.TransformNormal(
dockingPointDirection_Reference_Vector3D,MatrixD.Transpose(refLookAtMatrix));WritePoint_to_CustomData(dockingPointDirection_Section,
dockingPointDirection_Reference_Vector3D);}Vector3D target_Vector3D=GetPoint_from_CustomData(approachPoint1_Section),forward_Vector3D=GetPoint_from_CustomData(
dockingPointDirection_Section),target_Relative_Vector3D=target_Vector3D-remoteControl.GetPosition();target_Relative_Vector3D=Vector3D.TransformNormal
(target_Relative_Vector3D,MatrixD.Transpose(remoteControl.WorldMatrix));GravityAlignment(0.17f,forward_Vector3D);double
distance_Double=Vector3D.Distance(remoteControl.GetPosition(),target_Vector3D);if(distance_Double>=3){Movement_ForwardBackward(
target_Vector3D,3f);}else{Movement_ForwardBackward(target_Vector3D,0.5f);if(distance_Double<1){WriteValue_to_CustomData(mission_Section
,stage_Key,stage_2);ThrustEnabled(true);GyrosOnOff(true);}}WriteTargetValue(target_Vector3D,"Target",
target_Relative_Vector3D,"Target_Relative");}void Fly_to_ApproachPoint(string target_String,string target_Reference_to_MainShip_String,
Base6Directions.Direction direction,bool dockingMode_Bool,float speedLimit_Float,double targetDistance_Double,string nextStage_String){
DrillSwitch(false);GyrosOnOff(true);remoteControl.SetCollisionAvoidance(false);String iGC_Status=GetValue_from_CustomData(
mission_Section,iGC_Key);if(iGC_Status==online_String){Vector3D mainShip_RemoteControl_Vector3D=GetPoint_from_CustomData(
mainShip_RemoteControl_Position_Section);Vector3D mainShip_RemoteControl_Forward_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Forward_Section);
Vector3D mainShip_RemoteControl_Up_Vector3D=GetPoint_from_CustomData(mainShip_RemoteControl_Up_Section);MatrixD refLookAtMatrix=
MatrixD.CreateLookAt(mainShip_RemoteControl_Vector3D,mainShip_RemoteControl_Forward_Vector3D,mainShip_RemoteControl_Up_Vector3D
);Vector3D target_Reference_to_MainShip_Vector3D=GetPoint_from_CustomData(target_Reference_to_MainShip_String);
target_Reference_to_MainShip_Vector3D=Vector3D.TransformNormal(target_Reference_to_MainShip_Vector3D,MatrixD.Transpose(refLookAtMatrix));
target_Reference_to_MainShip_Vector3D+=mainShip_RemoteControl_Vector3D;WritePoint_to_CustomData(target_String,target_Reference_to_MainShip_Vector3D);}
speedLimit_Float=CalculateShipSpeed(speedLimit_Float);StageGo(target_String,speedLimit_Float,FlightMode.OneWay,direction,
dockingMode_Bool,targetDistance_Double,nextStage_String);}void Fly_to_DrillApproachPoint(string neststage){Echo(
"Fly_to_DrillApproachPoint");Vector3D target_Vector3D=GetPoint_from_CustomData(drillApproachPoint_Section),forward_Vector3D=
GetPoint_from_CustomData(mineField_Direction_Section),target_Relative_Vector3D=target_Vector3D-remoteControl.GetPosition();
target_Relative_Vector3D=Vector3D.TransformNormal(target_Relative_Vector3D,MatrixD.Transpose(remoteControl.WorldMatrix));GravityAlignment(0.17f,
forward_Vector3D);double distance_Double=Vector3D.Distance(remoteControl.GetPosition(),target_Vector3D);if(distance_Double>=4){
Movement_ForwardBackward(target_Vector3D,3f);}else{Movement_ForwardBackward(target_Vector3D,0.2f);if(distance_Double<1){WriteValue_to_CustomData
(mission_Section,stage_Key,neststage);ThrustEnabled(true);GyrosOnOff(true);}}WriteTargetValue(target_Vector3D,"Target",
target_Relative_Vector3D,"Target_Relative");}void Movement_UpDown(Vector3D target_Vector3D,float speedLimit_Float,float error_Left_Right_Float,
float error_Forward_Backward_Float){upDownControl.target_Vector3D=target_Vector3D;leftRightControl.target_Vector3D=
target_Vector3D;forwardBackwardControl.target_Vector3D=target_Vector3D;upDownControl.speedLimit_Float=speedLimit_Float;leftRightControl
.speedLimit_Float=0.5f;forwardBackwardControl.speedLimit_Float=0.5f;upDownControl.Data_Preprocessing();leftRightControl.
Data_Preprocessing();forwardBackwardControl.Data_Preprocessing();upDownControl.Direction_Y_Data();leftRightControl.Direction_X_Data();
forwardBackwardControl.Direction_Z_Data();leftRightControl.Control();forwardBackwardControl.Control();if(Math.Abs(leftRightControl.GetDistance
())<error_Left_Right_Float&&Math.Abs(forwardBackwardControl.GetDistance())<error_Forward_Backward_Float){upDownControl.
Control(false);}else{upDownControl.Control();}}void Movement_ForwardBackward(Vector3D target_Vector3D,float speedLimit_Float){
upDownControl.target_Vector3D=target_Vector3D;leftRightControl.target_Vector3D=target_Vector3D;forwardBackwardControl.target_Vector3D
=target_Vector3D;upDownControl.speedLimit_Float=0.5f;leftRightControl.speedLimit_Float=0.5f;forwardBackwardControl.
speedLimit_Float=speedLimit_Float;upDownControl.Data_Preprocessing();leftRightControl.Data_Preprocessing();forwardBackwardControl.
Data_Preprocessing();upDownControl.Direction_Y_Data();leftRightControl.Direction_X_Data();forwardBackwardControl.Direction_Z_Data();
leftRightControl.Control();upDownControl.Control();if(Math.Abs(leftRightControl.GetDistance())<0.2&&Math.Abs(upDownControl.GetDistance()
)<2){forwardBackwardControl.Control(false);}else{foreach(var thruster in forwardBackwardControl.thrusters_A)thruster.
ThrustOverride=0f;foreach(var thruster in forwardBackwardControl.thrusters_B)thruster.ThrustOverride=0f;}}void
UpdateFrequency_Adjustment(){string stage_String=GetValue_from_CustomData(mission_Section,stage_Key);if(stage_String==stage_0&&Runtime.
UpdateFrequency!=UpdateFrequency.Update100)Runtime.UpdateFrequency=UpdateFrequency.Update100;else if(stage_String!=stage_0&&Runtime.
UpdateFrequency!=UpdateFrequency.Update10)Runtime.UpdateFrequency=UpdateFrequency.Update10;}double CalculateBrakingDistance(double
initialVelocity_Double,double brakingForce_Double,double mass_Double){double deceleration_Double=brakingForce_Double/mass_Double;double
brakingDistance_Double=(initialVelocity_Double*initialVelocity_Double)/(2*deceleration_Double);return brakingDistance_Double;}double
CalculateFinalVelocity(double initialVelocity_Double,double force_Double,double mass_Double,double distance_Double){double acceleration_Double
=force_Double/mass_Double;double valueInsideSquareRoot=initialVelocity_Double*initialVelocity_Double+2*
acceleration_Double*distance_Double;double finalVelocity_Double=Math.Sqrt(valueInsideSquareRoot);return finalVelocity_Double;}float
CalculateShipSpeed(float speedLimit_Float){Vector3D worldDirection_Vector3D=remoteControl.CurrentWaypoint.Coords-remoteControl.GetPosition
(),bodyDirection_Vector3D=Vector3D.TransformNormal(worldDirection_Vector3D,remoteControl.WorldMatrix);double
shipMass_Double=remoteControl.CalculateShipMass().PhysicalMass,currentSpeed_Double=remoteControl.GetShipVelocities().LinearVelocity.
Length(),distance_Double=Math.Abs(bodyDirection_Vector3D.Z),distance_for_Calculate_Double=distance_Double;double
totalThrust_Double=0;foreach(var thruster in forwardBackwardControl.thrusters_A)totalThrust_Double+=thruster.MaxEffectiveThrust;double
distance_Required_Double=CalculateBrakingDistance(currentSpeed_Double,totalThrust_Double,shipMass_Double);float speedLimit_Calculate_Float=
Convert.ToSingle(CalculateFinalVelocity(0,totalThrust_Double,shipMass_Double,distance_Double));if(speedLimit_Float>
speedLimit_Calculate_Float)speedLimit_Float=speedLimit_Calculate_Float;return speedLimit_Float;}void PB_ShowInfo(){counter++;Me.GetSurface(0).
ContentType=ContentType.TEXT_AND_IMAGE;Color co=new Color();co.R=0;co.G=130;co.B=255;co.A=255;Me.GetSurface(0).FontColor=co;Me.
GetSurface(0).FontSize=2.2f;switch(counter){case 1:Me.GetSurface(0).WriteText(" \n",false);break;case 2:Me.GetSurface(0).WriteText
("*************************************************************************************\n",false);counter=0;break;}string
mode_Value=GetValue_from_CustomData(mission_Section,mode_Key);string stage_Value=GetValue_from_CustomData(mission_Section,
stage_Key);string drillPointNum_Value=GetValue_from_CustomData(mission_Section,drillPointNum_Key);string drillPointTotal_Value=
GetValue_from_CustomData(mission_Section,drillPointTotal_Key);string height_Value=GetValue_from_CustomData(mission_Section,height_Key);string
height2_Value=GetValue_from_CustomData(mission_Section,height2_Key);string depth_Value=GetValue_from_CustomData(mission_Section,
depth_Key);Me.GetSurface(0).WriteText($"Power:{powerLevel_Double}%\n",true);Me.GetSurface(0).WriteText(
$"Cargo:{cargoLevel_Double}%\n",true);Me.GetSurface(0).WriteText($"Fuel:{fuelLevel_Double}%\n",true);Me.GetSurface(0).WriteText(
$"Mode = {mode_Value}\n",true);Me.GetSurface(0).WriteText($"Stage = {stage_Value}\n",true);Me.GetSurface(0).WriteText(
$"DrillPoint = {drillPointNum_Value} / {drillPointTotal_Value}\n",true);}void LCD_ShowInfo(){foreach(var panel in panels){if(panel.ContentType!=ContentType.SCRIPT)panel.ContentType=
ContentType.SCRIPT;if(panel.ScriptBackgroundColor!=card_Background_Color_Overall)panel.ScriptBackgroundColor=
card_Background_Color_Overall;panel.ScriptBackgroundColor=card_Background_Color_Overall;DrawEachScreen(panel);}}void DrawEachScreen(IMyTextPanel
panel){MySpriteDrawFrame frame=panel.DrawFrame();ShipSheet(frame);ShowDistance(frame);FlowChartFrame(frame);FlowChartCore(
frame);frame.Dispose();}void ShipSheet(MySpriteDrawFrame frame){float width_Basic_Float=500f,height_Basic_Float=90f;float
y_Basic_Float=70f+6f+Convert.ToSingle(counter)/100f;float x_Basic_Float=512f/2f;float border_Float=3f;float y_NotifyLight_Float=15f+
Convert.ToSingle(counter)/100f;float height_NotifyLight_Float=20;float x_MiddleBar_Float=x_Basic_Float+10f;float
y_MiddleBar_Float=y_Basic_Float+height_Basic_Float/4f;float x_IGCSignifier_Float=x_Basic_Float-227.5f,y_IGCSignifier_Float=y_Basic_Float-
height_Basic_Float/4f;float width_IGCSignifier_Float=height_Basic_Float/2f-5f;float x_VerticalBar1_Float=x_IGCSignifier_Float+
height_Basic_Float/4f;float x_ShipName_Float=x_Basic_Float-200f,y_ShipName_Float=y_Basic_Float-40f;float y_ModeSignifier_Float=
y_Basic_Float+height_Basic_Float/4f;float width_ModeSignifier_Float=height_Basic_Float/2f*0.75f;float x_Mode_Float=x_Basic_Float-240f
,y_Mode_Float=y_Basic_Float+5f;float x_Stage_Float=x_Mode_Float+height_Basic_Float/2;float height_MiddleBar_Float=
height_Basic_Float/2f;float x_VerticalBar2_Float=x_VerticalBar1_Float+170f;float height_VerticalBar2_Float=height_Basic_Float/2f;float
x_PowerIcon_Float=x_VerticalBar2_Float+height_Basic_Float/4f-7f;float x_PowerText_Float=x_VerticalBar2_Float+height_Basic_Float/2f-15f;
float x_VerticalBar3_Float=x_VerticalBar2_Float+130f;float x_FuelIcon_Float=x_VerticalBar3_Float+height_Basic_Float/4f;float
x_FuelText_Float=x_VerticalBar3_Float+height_Basic_Float/2f;float x_VerticalBar4_Float=x_VerticalBar1_Float+width_IGCSignifier_Float;
float x_DrillPoint_Float=x_VerticalBar4_Float+5f;float fontSize_Float=1.15f;if(counter==0){DrawBox(frame,x_Basic_Float,
y_NotifyLight_Float,width_Basic_Float,height_NotifyLight_Float,card_Background_Color_Overall);}else{DrawBox(frame,x_Basic_Float,
y_NotifyLight_Float,width_Basic_Float,height_NotifyLight_Float,Color.Green);}DrawBox(frame,x_Basic_Float,y_Basic_Float,width_Basic_Float,
height_Basic_Float,border_Float,font_Color_Overall,card_Background_Color_Overall);DrawBox(frame,x_Basic_Float,y_Basic_Float,
width_Basic_Float,border_Float,font_Color_Overall);IGCSignifier(frame,x_IGCSignifier_Float,y_IGCSignifier_Float,width_IGCSignifier_Float,
font_Color_Overall);DrawBox(frame,x_VerticalBar1_Float,y_Basic_Float,border_Float,height_Basic_Float,font_Color_Overall);string iGC_String
=GetValue_from_CustomData(mission_Section,iGC_Key);if(iGC_String==offline_String)DrawIcon(frame,"Danger",
x_IGCSignifier_Float,y_IGCSignifier_Float,width_IGCSignifier_Float,width_IGCSignifier_Float,font_Color_Overall);string shipName_String=
GetValue_from_CustomData(shipInformation_Section,shipName_Key);PanelWriteText(frame,shipName_String,x_ShipName_Float,y_ShipName_Float,
fontSize_Float,TextAlignment.LEFT);string mode_String=GetValue_from_CustomData(mission_Section,mode_Key);switch(mode_String){case
mode_stop:DrawBox(frame,x_IGCSignifier_Float,y_ModeSignifier_Float,width_ModeSignifier_Float,width_ModeSignifier_Float,
font_Color_Overall);break;case mode_depart:DrawIcon(frame,"Triangle",x_IGCSignifier_Float,y_ModeSignifier_Float,width_ModeSignifier_Float,
width_ModeSignifier_Float,font_Color_Overall,90f);break;case mode_back:DrawIcon(frame,"Triangle",x_IGCSignifier_Float,y_ModeSignifier_Float,
width_ModeSignifier_Float,width_ModeSignifier_Float,font_Color_Overall,270f);break;case mode_backandstop:BackandStopSignifier(frame,
x_IGCSignifier_Float,y_ModeSignifier_Float,width_ModeSignifier_Float,font_Color_Overall,card_Background_Color_Overall);break;}string
stage_String=GetValue_from_CustomData(mission_Section,stage_Key);PanelWriteText(frame,stage_String,x_ShipName_Float,y_Mode_Float,
fontSize_Float,TextAlignment.LEFT);DrawBox(frame,x_VerticalBar4_Float,y_ModeSignifier_Float,border_Float,height_VerticalBar2_Float,
font_Color_Overall);string drillPointNum_String=GetValue_from_CustomData(mission_Section,drillPointNum_Key);string drillPointTotal_String=
GetValue_from_CustomData(mission_Section,drillPointTotal_Key);string drillPoint_String=drillPointNum_String+"/"+drillPointTotal_String;
PanelWriteText(frame,drillPoint_String,x_DrillPoint_Float,y_Mode_Float,fontSize_Float,TextAlignment.LEFT);DrawBox(frame,
x_VerticalBar2_Float,y_ModeSignifier_Float,border_Float,height_VerticalBar2_Float,font_Color_Overall);DrawIcon(frame,"IconEnergy",
x_PowerIcon_Float,y_ModeSignifier_Float,width_ModeSignifier_Float,width_ModeSignifier_Float,font_Color_Overall);PanelWriteText(frame,
powerLevel_Double.ToString()+"%",x_PowerText_Float,y_Mode_Float,fontSize_Float,TextAlignment.LEFT);DrawBox(frame,x_VerticalBar3_Float,
y_Basic_Float,border_Float,height_Basic_Float,font_Color_Overall);DrawIcon(frame,"IconHydrogen",x_FuelIcon_Float,
y_ModeSignifier_Float,width_ModeSignifier_Float,width_ModeSignifier_Float,font_Color_Overall);PanelWriteText(frame,fuelLevel_Double.ToString(
)+"%",x_FuelText_Float,y_Mode_Float,fontSize_Float,TextAlignment.LEFT);DrawIcon(frame,
"Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds",x_FuelIcon_Float,y_IGCSignifier_Float,width_ModeSignifier_Float,width_ModeSignifier_Float,font_Color_Overall);
PanelWriteText(frame,cargoLevel_Double.ToString()+"%",x_FuelText_Float,y_ShipName_Float,fontSize_Float,TextAlignment.LEFT);}void
ShowDistance(MySpriteDrawFrame frame){float width_Basic_Float=500f,height_Basic_Float=25f;float x_Basic_Float=512f/2f,y_Basic_Float=
140f+Convert.ToSingle(counter)/100f;float fontSize_Float=0.8f,border_Float=3f;float height_Header_Float=7f;float
y_HorizontalLine1_Float=y_Basic_Float+height_Header_Float/2f+height_Basic_Float;float y_Header2_Float=y_HorizontalLine1_Float+
height_Basic_Float+height_Header_Float/2f;float y_HorizontalLine2_Float=y_Header2_Float+height_Header_Float/2f+height_Basic_Float;float
y_HorizontalLine3_Float=y_HorizontalLine2_Float+height_Basic_Float;float x_VerticalLine1_Float=x_Basic_Float-width_Basic_Float/2f+border_Float/
2f;float x_VerticalLine2_Float=x_VerticalLine1_Float+width_Basic_Float/3f;float x_VerticalLine3_Float=
x_VerticalLine2_Float+width_Basic_Float/3f;float x_VerticalLine4_Float=x_Basic_Float+width_Basic_Float/2f-border_Float/2f;float
height_VerticalLine1_Float=height_Basic_Float*4f+height_Header_Float*2f;float y_VerticalLine1_Float=y_Basic_Float+height_Header_Float/2f+
height_Basic_Float*2f;float y_VerticalLine2_Float=y_HorizontalLine1_Float+height_Basic_Float/2f;float y_VerticalLine5_Float=
y_HorizontalLine2_Float+height_Basic_Float/2f;float x_Title1_Float=x_VerticalLine1_Float+5f;float x_Distance1_Float=x_VerticalLine4_Float-5f;
float x_CoordX1_Float=x_VerticalLine2_Float-5f;float x_CoordY1_Float=x_VerticalLine3_Float-5f;float y_Title1_Float=
y_HorizontalLine1_Float-24f;float y_CoordX1_Float=y_HorizontalLine1_Float+2f;float y_Title2_Float=y_HorizontalLine2_Float-24f;float
y_CoordX2_Float=y_HorizontalLine2_Float+2f;float x_YSign_Float=x_VerticalLine2_Float+5f;float x_ZSign_Float=x_VerticalLine3_Float+5f;
DrawBox(frame,x_Basic_Float,y_Basic_Float,width_Basic_Float,height_Header_Float,font_Color_Overall);DrawBox(frame,x_Basic_Float
,y_Header2_Float,width_Basic_Float,height_Header_Float,font_Color_Overall);DrawBox(frame,x_Basic_Float,
y_HorizontalLine1_Float,width_Basic_Float,border_Float,font_Color_Overall);DrawBox(frame,x_Basic_Float,y_HorizontalLine2_Float,
width_Basic_Float,border_Float,font_Color_Overall);DrawBox(frame,x_Basic_Float,y_HorizontalLine3_Float,width_Basic_Float,border_Float,
font_Color_Overall);DrawBox(frame,x_VerticalLine1_Float,y_VerticalLine1_Float,border_Float,height_VerticalLine1_Float,font_Color_Overall);
DrawBox(frame,x_VerticalLine2_Float,y_VerticalLine2_Float,border_Float,height_Basic_Float,font_Color_Overall);DrawBox(frame,
x_VerticalLine3_Float,y_VerticalLine1_Float,border_Float,height_VerticalLine1_Float,font_Color_Overall);DrawBox(frame,x_VerticalLine4_Float,
y_VerticalLine1_Float,border_Float,height_VerticalLine1_Float,font_Color_Overall);DrawBox(frame,x_VerticalLine2_Float,y_VerticalLine5_Float,
border_Float,height_Basic_Float,font_Color_Overall);string finalTarget_String=GetValue_from_CustomData(finalTarget_Section,
targetName_Key);PanelWriteText(frame,finalTarget_String,x_Title1_Float,y_Title1_Float,fontSize_Float,TextAlignment.LEFT);string
finalTargetDistance_String=GetValue_from_CustomData(finalTarget_Section,distance_Key);PanelWriteText(frame,finalTargetDistance_String,
x_Distance1_Float,y_Title1_Float,fontSize_Float,TextAlignment.RIGHT);PanelWriteText(frame,"d=",x_ZSign_Float,y_Title1_Float,
fontSize_Float,TextAlignment.LEFT);Vector3D finalTarget_Vector3D=GetPoint_from_CustomData(finalTarget_Section);PanelWriteText(frame,
Math.Round(finalTarget_Vector3D.X,1).ToString(),x_CoordX1_Float,y_CoordX1_Float,fontSize_Float,TextAlignment.RIGHT);
PanelWriteText(frame,Math.Round(finalTarget_Vector3D.Y,1).ToString(),x_CoordY1_Float,y_CoordX1_Float,fontSize_Float,TextAlignment.
RIGHT);PanelWriteText(frame,Math.Round(finalTarget_Vector3D.Z,1).ToString(),x_Distance1_Float,y_CoordX1_Float,fontSize_Float,
TextAlignment.RIGHT);PanelWriteText(frame,"X=",x_Title1_Float,y_CoordX1_Float,fontSize_Float,TextAlignment.LEFT);PanelWriteText(frame
,"Y=",x_YSign_Float,y_CoordX1_Float,fontSize_Float,TextAlignment.LEFT);PanelWriteText(frame,"Z=",x_ZSign_Float,
y_CoordX1_Float,fontSize_Float,TextAlignment.LEFT);string currentTarget_String=GetValue_from_CustomData(currentTarget_Section,
targetName_Key);PanelWriteText(frame,currentTarget_String,x_Title1_Float,y_Title2_Float,fontSize_Float,TextAlignment.LEFT);string
currentTargetDistance_String=GetValue_from_CustomData(currentTarget_Section,distance_Key);PanelWriteText(frame,currentTargetDistance_String,
x_Distance1_Float,y_Title2_Float,fontSize_Float,TextAlignment.RIGHT);PanelWriteText(frame,"d=",x_ZSign_Float,y_Title2_Float,
fontSize_Float,TextAlignment.LEFT);Vector3D currentTarget_Vector3D=GetPoint_from_CustomData(currentTarget_Section);PanelWriteText(
frame,Math.Round(currentTarget_Vector3D.X,1).ToString(),x_CoordX1_Float,y_CoordX2_Float,fontSize_Float,TextAlignment.RIGHT);
PanelWriteText(frame,Math.Round(currentTarget_Vector3D.Y,1).ToString(),x_CoordY1_Float,y_CoordX2_Float,fontSize_Float,TextAlignment.
RIGHT);PanelWriteText(frame,Math.Round(currentTarget_Vector3D.Z,1).ToString(),x_Distance1_Float,y_CoordX2_Float,
fontSize_Float,TextAlignment.RIGHT);PanelWriteText(frame,"X=",x_Title1_Float,y_CoordX2_Float,fontSize_Float,TextAlignment.LEFT);
PanelWriteText(frame,"Y=",x_YSign_Float,y_CoordX2_Float,fontSize_Float,TextAlignment.LEFT);PanelWriteText(frame,"Z=",x_ZSign_Float,
y_CoordX2_Float,fontSize_Float,TextAlignment.LEFT);}void FlowChartFrame(MySpriteDrawFrame frame){float width_Basic_Float=500f,
height_Basic_Float=240f,height_Box_Float=25f;float x_Basic_Float=512f/2f,y_Basic_Float=385f+Convert.ToSingle(counter)/100f;float
border_Float=3f;float fontSize_Float=0.8f;float width_FlyIcon_Float=height_Box_Float*2f;float width_InfoBox_Float=width_Basic_Float*
0.4f;float width_HorizontalLine1_Float=width_InfoBox_Float-width_FlyIcon_Float;float x_HorizontalLine1_Float=x_Basic_Float-
width_Basic_Float/2f+width_FlyIcon_Float+width_HorizontalLine1_Float/2f;float x_HorizontalLine3_Float=x_Basic_Float-width_Basic_Float/2f+
width_InfoBox_Float/2f;float y_HorizontalLine1_Float=y_Basic_Float-height_Basic_Float/2f+height_Box_Float;float y_HorizontalLine2_Float=
y_HorizontalLine1_Float+height_Box_Float;float y_HorizontalLine3_Float=y_HorizontalLine2_Float+height_Box_Float;float y_HorizontalLine4_Float=
y_Basic_Float+height_Basic_Float/2f-height_Box_Float*3f;float y_HorizontalLine5_Float=y_HorizontalLine4_Float+height_Box_Float;float
y_HorizontalLine6_Float=y_HorizontalLine5_Float+height_Box_Float;float height_VerticalLine1_Float=height_Box_Float*3f;float x_FlyIcon_Float=
x_Basic_Float-width_Basic_Float/2f+width_FlyIcon_Float/2f;float x_VerticalLine1_Float=x_Basic_Float-width_Basic_Float/2f+
width_FlyIcon_Float;float x_VerticalLine2_Float=x_Basic_Float-width_Basic_Float/2f+width_InfoBox_Float-border_Float/2;float
y_VerticalLine1_Float=y_HorizontalLine1_Float+height_Box_Float/2f;float y_VerticalLine3_Float=y_HorizontalLine5_Float+height_Box_Float/2f;
float x_SpeedLimitSign_Float=x_VerticalLine1_Float+5f;float x_SpeedLimit_Float=x_VerticalLine2_Float-5f;float
y_SpeedLimitSign_Float=y_HorizontalLine1_Float-24f;float y_HeightSign1_Float=y_HorizontalLine2_Float-24f;float y_HeightSign2_Float=
y_HorizontalLine3_Float-24f;float y_DrillSpeed_Float=y_HorizontalLine4_Float+2f;float y_Interval_Float=y_HorizontalLine5_Float+2f;float
y_DepthSign_Float=y_HorizontalLine6_Float+1f;DrawBox(frame,x_Basic_Float,y_Basic_Float,width_Basic_Float,height_Basic_Float,border_Float,
font_Color_Overall,card_Background_Color_Overall);DrawBox(frame,x_HorizontalLine1_Float,y_HorizontalLine1_Float,
width_HorizontalLine1_Float,border_Float,font_Color_Overall);DrawBox(frame,x_HorizontalLine1_Float,y_HorizontalLine2_Float,
width_HorizontalLine1_Float,border_Float,font_Color_Overall);DrawBox(frame,x_HorizontalLine3_Float,y_HorizontalLine3_Float,width_InfoBox_Float,
border_Float,font_Color_Overall);DrawBox(frame,x_HorizontalLine3_Float,y_HorizontalLine4_Float,width_InfoBox_Float,border_Float,
font_Color_Overall);DrawBox(frame,x_HorizontalLine1_Float,y_HorizontalLine5_Float,width_HorizontalLine1_Float,border_Float,
font_Color_Overall);DrawBox(frame,x_HorizontalLine1_Float,y_HorizontalLine6_Float,width_HorizontalLine1_Float,border_Float,
font_Color_Overall);DrawBox(frame,x_VerticalLine1_Float,y_VerticalLine1_Float,border_Float,height_VerticalLine1_Float,font_Color_Overall);
DrawBox(frame,x_VerticalLine2_Float,y_VerticalLine1_Float,border_Float,height_VerticalLine1_Float,font_Color_Overall);DrawBox(
frame,x_VerticalLine1_Float,y_VerticalLine3_Float,border_Float,height_VerticalLine1_Float,font_Color_Overall);DrawBox(frame,
x_VerticalLine2_Float,y_VerticalLine3_Float,border_Float,height_VerticalLine1_Float,font_Color_Overall);DrawIcon(frame,
"Textures\\FactionLogo\\Builders\\BuilderIcon_14.dds",x_FlyIcon_Float,y_VerticalLine1_Float,width_FlyIcon_Float,width_FlyIcon_Float,font_Color_Overall);DrawIcon(frame,
"Textures\\FactionLogo\\Miners\\MinerIcon_3.dds",x_FlyIcon_Float,y_VerticalLine3_Float,width_FlyIcon_Float,width_FlyIcon_Float,font_Color_Overall);string
speedLimit_String=GetValue_from_CustomData(mission_Section,speedLimit_Key);speedLimit_String=speedLimit_String+" m/s";PanelWriteText(
frame,"V=",x_SpeedLimitSign_Float,y_SpeedLimitSign_Float,fontSize_Float,TextAlignment.LEFT);PanelWriteText(frame,
speedLimit_String,x_SpeedLimit_Float,y_SpeedLimitSign_Float,fontSize_Float,TextAlignment.RIGHT);string height_String=
GetValue_from_CustomData(mission_Section,height_Key);height_String=height_String+" m";PanelWriteText(frame,"H=",x_SpeedLimitSign_Float,
y_HeightSign1_Float,fontSize_Float,TextAlignment.LEFT);PanelWriteText(frame,height_String,x_SpeedLimit_Float,y_HeightSign1_Float,
fontSize_Float,TextAlignment.RIGHT);string height2_String=GetValue_from_CustomData(mission_Section,height2_Key);height2_String=
height2_String+" m";PanelWriteText(frame,"H2=",x_SpeedLimitSign_Float,y_HeightSign2_Float,fontSize_Float,TextAlignment.LEFT);
PanelWriteText(frame,height2_String,x_SpeedLimit_Float,y_HeightSign2_Float,fontSize_Float,TextAlignment.RIGHT);string
drillSpeed_String=GetValue_from_CustomData(mission_Section,drillSpeed_Key);drillSpeed_String=drillSpeed_String+" m/s";PanelWriteText(
frame,"V=",x_SpeedLimitSign_Float,y_DrillSpeed_Float,fontSize_Float,TextAlignment.LEFT);PanelWriteText(frame,
drillSpeed_String,x_SpeedLimit_Float,y_DrillSpeed_Float,fontSize_Float,TextAlignment.RIGHT);string interval_String=
GetValue_from_CustomData(mission_Section,interval_Key);interval_String=interval_String+" m";PanelWriteText(frame,"I->II=",x_SpeedLimitSign_Float
,y_Interval_Float,fontSize_Float,TextAlignment.LEFT);PanelWriteText(frame,interval_String,x_SpeedLimit_Float,
y_Interval_Float,fontSize_Float,TextAlignment.RIGHT);string depth_String=GetValue_from_CustomData(mission_Section,depth_Key);
depth_String=depth_String+" m";PanelWriteText(frame,"D=",x_SpeedLimitSign_Float,y_DepthSign_Float,fontSize_Float,TextAlignment.LEFT)
;PanelWriteText(frame,depth_String,x_SpeedLimit_Float,y_DepthSign_Float,fontSize_Float,TextAlignment.RIGHT);}void
FlowChartCore(MySpriteDrawFrame frame){float width_Basic_Float=500f,width_Box_Float=25f;float x_Basic_Float=512f/2f,y_Basic_Float=
385f+Convert.ToSingle(counter)/100f;float border_Float=3f,interval_Float=7f;float width_Stage0_Float=width_Box_Float*2f;
float x_Stage0_Float=x_Basic_Float-width_Basic_Float*0.5f+40f+width_Stage0_Float*0.5f;float width_Stage1_Float=150f;float
x_Stage1_Float=x_Stage0_Float+width_Stage0_Float*0.5f+interval_Float+width_Stage1_Float*0.5f;float height_Stage2_Float=90f;float
x_Stage2_Float=x_Stage1_Float+width_Stage1_Float/2f+interval_Float+width_Box_Float*0.5f;float y_Stage2_Float=y_Basic_Float-
height_Stage2_Float*0.5f+width_Box_Float*0.5f;float width_Stage3_Float=150f;float x_Stage3_Float=x_Stage2_Float+width_Stage3_Float*0.5f-
width_Box_Float*0.5f;float y_Stage3_Float=y_Stage2_Float-height_Stage2_Float*0.5f-interval_Float-width_Box_Float*0.5f;float
height_Stage4_Float=90f;float x_Stage4_Float=x_Stage3_Float+width_Stage3_Float*0.5f-width_Box_Float*0.5f;float y_Stage4_Float=
y_Stage3_Float+width_Box_Float*0.5f+interval_Float+height_Stage4_Float*0.5f;float y_Stage5_Float=y_Stage4_Float+height_Stage4_Float+
interval_Float;float x_Stage6_Float=x_Stage4_Float+50f;string stage_String=GetValue_from_CustomData(mission_Section,stage_Key);if(
stage_String==stage_0)DrawBox(frame,x_Stage0_Float,y_Basic_Float,width_Stage0_Float,width_Stage0_Float,border_Float,
font_Color_Overall,Color.Green);else DrawBox(frame,x_Stage0_Float,y_Basic_Float,width_Stage0_Float,width_Stage0_Float,border_Float,
font_Color_Overall,card_Background_Color_Overall);if(stage_String==stage_1)DrawBox(frame,x_Stage1_Float,y_Basic_Float,width_Stage1_Float,
width_Box_Float,border_Float,font_Color_Overall,Color.Green);else DrawBox(frame,x_Stage1_Float,y_Basic_Float,width_Stage1_Float,
width_Box_Float,border_Float,font_Color_Overall,card_Background_Color_Overall);if(stage_String==stage_2)DrawBox(frame,x_Stage2_Float,
y_Stage2_Float,width_Box_Float,height_Stage2_Float,border_Float,font_Color_Overall,Color.Green);else DrawBox(frame,x_Stage2_Float,
y_Stage2_Float,width_Box_Float,height_Stage2_Float,border_Float,font_Color_Overall,card_Background_Color_Overall);if(stage_String==
stage_3)DrawBox(frame,x_Stage3_Float,y_Stage3_Float,width_Stage3_Float,width_Box_Float,border_Float,font_Color_Overall,Color.
Green);else DrawBox(frame,x_Stage3_Float,y_Stage3_Float,width_Stage3_Float,width_Box_Float,border_Float,font_Color_Overall,
card_Background_Color_Overall);if(stage_String==stage_4)DrawBox(frame,x_Stage4_Float,y_Stage4_Float,width_Box_Float,height_Stage4_Float,border_Float,
font_Color_Overall,Color.Green);else DrawBox(frame,x_Stage4_Float,y_Stage4_Float,width_Box_Float,height_Stage4_Float,border_Float,
font_Color_Overall,card_Background_Color_Overall);if(stage_String==stage_5)DrawBox(frame,x_Stage4_Float,y_Stage5_Float,width_Box_Float,
height_Stage4_Float,border_Float,font_Color_Overall,Color.Green);else DrawBox(frame,x_Stage4_Float,y_Stage5_Float,width_Box_Float,
height_Stage4_Float,border_Float,font_Color_Overall,card_Background_Color_Overall);if(stage_String==stage_6)DrawBox(frame,x_Stage6_Float,
y_Stage5_Float,width_Box_Float,height_Stage4_Float,border_Float,font_Color_Overall,Color.Green);else DrawBox(frame,x_Stage6_Float,
y_Stage5_Float,width_Box_Float,height_Stage4_Float,border_Float,font_Color_Overall,card_Background_Color_Overall);if(stage_String==
stage_7)DrawBox(frame,x_Stage6_Float,y_Stage4_Float,width_Box_Float,height_Stage4_Float,border_Float,font_Color_Overall,Color.
Green);else DrawBox(frame,x_Stage6_Float,y_Stage4_Float,width_Box_Float,height_Stage4_Float,border_Float,font_Color_Overall,
card_Background_Color_Overall);if(stage_String==stage_8)DrawBox(frame,x_Stage6_Float,y_Stage3_Float,width_Box_Float,width_Box_Float,border_Float,
font_Color_Overall,Color.Green);else DrawBox(frame,x_Stage6_Float,y_Stage3_Float,width_Box_Float,width_Box_Float,border_Float,
font_Color_Overall,card_Background_Color_Overall);}void IGCSignifier(MySpriteDrawFrame frame,float x,float y,float width,Color co){float
x1_Float=x-width/4f;float y_Triangle_Float=y-width*0.2f;float width_Triangle_Float=width*0.4f;float border_Float=4f;float
interval_Float=8f,heightInterval_Float=10f;float height1_Float=width*0.7f,height2_Float=height1_Float-heightInterval_Float,
height3_Float=height2_Float-heightInterval_Float;float y_Bar1_Float=y+heightInterval_Float/2f,y_Bar2_Float=y+heightInterval_Float;
DrawIcon(frame,"Triangle",x1_Float,y_Triangle_Float,width_Triangle_Float,width_Triangle_Float,co,180f);DrawIcon(frame,
"SquareSimple",x1_Float,y,border_Float,height1_Float,co);DrawIcon(frame,"SquareSimple",x1_Float+interval_Float*1f,y_Bar2_Float,
border_Float,height3_Float,co);DrawIcon(frame,"SquareSimple",x1_Float+interval_Float*2f,y_Bar1_Float,border_Float,height2_Float,co);
DrawIcon(frame,"SquareSimple",x1_Float+interval_Float*3f,y,border_Float,height1_Float,co);}void BackandStopSignifier(
MySpriteDrawFrame frame,float x,float y,float width,Color iconColor,Color backgroundColor){float width_Triangle_Float=width-6f;DrawBox(
frame,x,y,width,width,iconColor);DrawIcon(frame,"Triangle",x,y,width_Triangle_Float,width_Triangle_Float,backgroundColor,270f
);}void DrawIcon(MySpriteDrawFrame frame,string icon,float x,float y,float width,float height,Color picture_Color){var
sprite=new MySprite{Type=SpriteType.TEXTURE,Data=icon,Position=new Vector2(x,y),RotationOrScale=0,Size=new Vector2(width,
height),Color=picture_Color,Alignment=TextAlignment.CENTER};frame.Add(sprite);}void DrawIcon(MySpriteDrawFrame frame,string
icon,float x,float y,float width,float height,Color picture_Color,float rotation){var sprite=new MySprite{Type=SpriteType.
TEXTURE,Data=icon,Position=new Vector2(x,y),RotationOrScale=Convert.ToSingle(rotation/360f*2f*Math.PI),Size=new Vector2(width,
height),Color=picture_Color,Alignment=TextAlignment.CENTER};frame.Add(sprite);}void DrawBox(MySpriteDrawFrame frame,float x,
float y,float width,float height,float border_Width,Color border_Color,Color background_Color){MySprite sprite;sprite=
MySprite.CreateSprite("SquareSimple",new Vector2(x,y),new Vector2(width,height));sprite.Color=border_Color;frame.Add(sprite);
sprite=MySprite.CreateSprite("SquareSimple",new Vector2(x,y),new Vector2(width-border_Width*2f,height-border_Width*2f));sprite
.Color=background_Color;frame.Add(sprite);}void DrawBox(MySpriteDrawFrame frame,float x,float y,float width,float height,
Color background_Color){MySprite sprite;sprite=MySprite.CreateSprite("SquareSimple",new Vector2(x,y),new Vector2(width,height
));sprite.Color=background_Color;frame.Add(sprite);}void PanelWriteText(MySpriteDrawFrame frame,string text,float x,float
y,float fontSize,TextAlignment alignment){MySprite sprite=new MySprite(){Type=SpriteType.TEXT,Data=text,Position=new
Vector2(x,y),RotationOrScale=fontSize,Color=font_Color_Overall,Alignment=alignment,FontId="LoadingScreen"};frame.Add(sprite);}
void Main(string argument,UpdateType updateSource){if(argument!=""){ButtonOnShip(argument);}CheckBlock();BatteriesStatus();
CargoStatus();HydrogenStatus();AttitudeMeasurement();Receive_IGC_Information();CoreLogic();UpdateFrequency_Adjustment();
SendShipStatus();PB_ShowInfo();LCD_ShowInfo();}class MovementControl{public IMyRemoteControl remoteControl;public List<IMyThrust>
thrusters_A=new List<IMyThrust>();public List<IMyThrust>thrusters_B=new List<IMyThrust>();public Vector3D target_Vector3D=new
Vector3D();public float speedLimit_Float=0;const double acceleration_Min_Double=0.05,acceleration_Max_Double=1;const float
lowSpeed_Float=0.03f;Vector3D velocity_Vector3D,target_Relative_Vector3D,gravity_Vector3D;double distance_Double,gravity_Double,
targetForce_Double,activeForce_Double,velocity_Double;public void Data_Preprocessing(){velocity_Vector3D=Vector3D.TransformNormal(
remoteControl.GetShipVelocities().LinearVelocity,MatrixD.Transpose(remoteControl.WorldMatrix));target_Relative_Vector3D=
target_Vector3D-remoteControl.GetPosition();target_Relative_Vector3D=Vector3D.TransformNormal(target_Relative_Vector3D,MatrixD.
Transpose(remoteControl.WorldMatrix));gravity_Vector3D=Vector3D.TransformNormal(remoteControl.GetTotalGravity(),MatrixD.Transpose
(remoteControl.WorldMatrix));targetForce_Double=acceleration_Min_Double*remoteControl.CalculateShipMass().PhysicalMass;
activeForce_Double=acceleration_Max_Double*remoteControl.CalculateShipMass().PhysicalMass;}public void Direction_X_Data(){velocity_Double=
velocity_Vector3D.X;distance_Double=target_Relative_Vector3D.X;gravity_Double=gravity_Vector3D.X*remoteControl.CalculateShipMass().
PhysicalMass;}public void Direction_Y_Data(){velocity_Double=velocity_Vector3D.Y;distance_Double=target_Relative_Vector3D.Y;
gravity_Double=gravity_Vector3D.Y*remoteControl.CalculateShipMass().PhysicalMass;}public void Direction_Z_Data(){velocity_Double=
velocity_Vector3D.Z;distance_Double=target_Relative_Vector3D.Z;gravity_Double=gravity_Vector3D.Z*remoteControl.CalculateShipMass().
PhysicalMass;}public void Control(bool steady_Bool=true){if(!steady_Bool&&Math.Abs(distance_Double)>10){targetForce_Double=
speedLimit_Float*0.5f*remoteControl.CalculateShipMass().PhysicalMass;}double resultantForce_Double=targetForce_Double+Math.Abs(
gravity_Double);if(Math.Abs(velocity_Double)<lowSpeed_Float){resultantForce_Double+=activeForce_Double;targetForce_Double+=
activeForce_Double;}if(distance_Double>0){if(velocity_Double<-0.05||Math.Abs(velocity_Double)>speedLimit_Float){SetThrust(thrusters_A,0f);
SetThrust(thrusters_B,0f);}else{if(gravity_Double<=0){SetThrust(thrusters_A,resultantForce_Double);SetThrust(thrusters_B,0f);}
else{SetThrust(thrusters_A,targetForce_Double);SetThrust(thrusters_B,Math.Abs(gravity_Double));}}}else{if(velocity_Double>
0.05||Math.Abs(velocity_Double)>speedLimit_Float){SetThrust(thrusters_A,0f);SetThrust(thrusters_B,0f);}else{if(
gravity_Double<=0){SetThrust(thrusters_A,Math.Abs(gravity_Double));SetThrust(thrusters_B,targetForce_Double);}else{SetThrust(
thrusters_A,0f);SetThrust(thrusters_B,resultantForce_Double);}}}}public void SetThrust(List<IMyThrust>thrusters,double force_Double
){double totalForce_Double=0,ratio_Double=0;foreach(var thruster in thrusters)totalForce_Double+=thruster.
MaxEffectiveThrust;if(totalForce_Double!=0)ratio_Double=force_Double/totalForce_Double;foreach(var thruster in thrusters){if(thruster.
ThrustOverride!=thruster.MaxEffectiveThrust*Convert.ToSingle(ratio_Double)){thruster.ThrustOverride=thruster.MaxEffectiveThrust*
Convert.ToSingle(ratio_Double);}}}public double GetDistance(){return distance_Double;}}