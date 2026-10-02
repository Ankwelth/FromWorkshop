/* Wico Craft NAV Control sub-module
 * 
 * Workshop Link: http://steamcommunity.com/sharedfiles/filedetails/?id=797020890
 * 
 * Uncompressed Source: https://github.com/Wicorel/WicoSpaceEngineers/tree/master/MDK%20Nav
 */
// Minified script was deployed at 2019-12-22 22:45
/*
*
* Commands:
* W <waypoint>
*   Go to waypoint
*
* S <max speed>
*   Set max travel speed
*
* D <min arrive distance
*  set min expected arrival distance in meters
*
* C <comment>
*   Any text except ;
*
* O <waypoint> (untestd)
*   Orient to waypoint
*
*
* Handles MODES:
* MODE_GOTARGET
*
*
* History:
*
2.0.4 Upate to new save format
    .04A Camera Scans for Obstacles...!!!one

2.1 Use new blockInint and localgrids

    .1g Add Docked
    copy from SLED PATROL
    .1h fixed yaw only gyromain
    .1i tested in space. Added !NAV to gyro check
    .1j add doroll
    .1k use (and fix/test) IMyGyroControl

2.2: Update for 1.72

2.9 Copy from Sled Dock 2.2

Needs LOTS of updates.

3.0 Move code into 3.0

3.0A Start NAV processing: W and O
3.0B Add D, S, C
3.0C Add arrivedtarget

3.0D 110517  search order for text panels

3.1 Version for PB Updates SE 1.185
o Added support for GPS-formatted nav locations
    Ex:  W GPS:Wicorel #1:53970.01:128270.31:-123354.92:

3.1a
remove blockApplyActions() and make routines for each block type that needs it

3.2 Collision Avoidance from Docking module for thruster travel

Added Rotors

3.2A travelmovement calculating target speeds and distances with more precision

3.2B Sled Testing

3.2C INI Save
    fix bug in serialize wrting z,y z, instead of x,y,z (oops)

3.2D INI WCCM 01062018

3.2E Major INI settings

3.3 Lists of text panels
Only output to textpanels and end of run

3.3A Redo Serialize

3.4 Sled testing
(EFM Update 8 Drones)

3.4a Save NAV settings so nav can properly resume
(EFM Update 9 Drones)

3.4B

3.4C AvionicsGyro fixes (terminal properties changed units)
Fix for bug in SE wheel setter for friction
Add Gyro limits to CustomData
(EFM Update 11 wheeled Drones)

3.4D Air unit NAV changes
terrain auto-follow
alignment in gravity
(EFM Update 11 Air unit drones)

3.4E testing with space craft again.
Mar 08 2018
Fix gyro terminal properties

3.4F Fix vertical velocity calculation
Add HoverEngine detection support to thrusters

3.4G Timer Processing updates
More CustomData options for timer names, etc.

3.4H 03282018
Add desired arrivalmode and state to navcommon
'arrived' now sets desired mode and state

3.4I Apr 03 2018
push to max speed and then coast..

3.4J Apr 21 2018
Show initialization problems and retry if necessary items are missing

3.4K May 06 2018+
Sensor code update
Tested with higher world speeds
May 27 2018

3.4L June 02 2018
Raycast collision also checks half-way distance for size of ship instea of just full scan distance.
June 10 Added target name so it can be shown

3.4M
Set target name from GPS Waypointcommend

3.4N  default speed to world max speed, not 100
June 15, 2018
Renumber modes >150 to make room

3.4O July 23, 2018 SE 1.187 MDK 1.1.16
Start support for docking in gravity.
(hiatus)

3.4P Sep 08: check hollow asteroids

3.5 Jan 21: SE 1.189
IGC for nav commands
Supports waypoint list
added launch, orbital launch and dock commands

3.7 May 28 2019 SE 1.190
Current source.
Don't set gyro if it's already set.

TODO:
    Do quadrant search for escape
    Support 'Loop' (patrol) command
    handle sides without cameras
    TEST: handle no cameras forward (slower min speed)
    Timer triggers on mode/state changes
    get waypoint list from RC
    Test Rocket ships waypoint nav in gravity

3.71 June 21 2019. SE 1.191.
tmDoFormard for HUGE thrust ratios (hydrogen)

3.7b July 17 2019.
EFM envoy staying high altitude

3.8 Dec 22 2019
Removal of old IGC system due to 1.193.100

*/

string Ћ="Wico Craft";string Њ="NAV";string Љ="3.8";const string Ј="0.00";void Ї(ʈ À){ք(À);}void І(ʈ À){օ(À);}void Ѕ(){
do{if(ϟ.HasPendingMessage){var Є=ϟ.AcceptMessage();var Ќ=Є.Source;Vector3D È;int δ;int Ϊ;double β;string α;double d;bool ΰ
;Ֆ(Є.Data.ToString(),out È,out δ,out Ϊ,out β,out α,out d,out ΰ);ε(È,δ,Ϊ,β,α,d,ΰ);}}while(ϟ.HasPendingMessage);do{if(Ϡ.
HasPendingMessage){var Є=ϟ.AcceptMessage();Vector3D È;int δ;int Ϊ;double β;string α;double d;bool ΰ;Ֆ(Є.Data.ToString(),out È,out δ,out Ϊ
,out β,out α,out d,out ΰ);ϙ(È,δ,Ϊ,β,α,d,ΰ);}}while(Ϡ.HasPendingMessage);do{if(Ϟ.HasPendingMessage){var Є=Ϟ.AcceptMessage(
);Echo("ResetNav Received Message");}}while(Ϟ.HasPendingMessage);do{if(ϝ.HasPendingMessage){var Є=Ϟ.AcceptMessage();Echo(
"_NavQueueLaunch Received Message");η();}}while(ϝ.HasPendingMessage);do{if(Ϝ.HasPendingMessage){var Є=Ϟ.AcceptMessage();Echo(
"_NavQueueOrbitalLaunch Received Message");ζ();}}while(Ϝ.HasPendingMessage);}void П(){Echo(Ɂ);Ҷ();Echo(ʵ());}void О(bool Н=false){ő(ҿ);ʋ();ҋ(ը);ҋ(է);ȡ(0,75);if(Ƀ
is IMyRemoteControl)((IMyRemoteControl)Ƀ).SetAutoPilotEnabled(false);if(Ƀ is IMyShipController)((IMyShipController)Ƀ).
DampenersOverride=true;}Л М=new Л();class Л{public const Base6Directions.Direction К=Base6Directions.Direction.Forward;public const
Base6Directions.Direction Й=Base6Directions.Direction.Backward;public const Base6Directions.Direction И=Base6Directions.Direction.Left;
public const Base6Directions.Direction З=Base6Directions.Direction.Right;public const Base6Directions.Direction Ж=
Base6Directions.Direction.Up;public const Base6Directions.Direction Е=Base6Directions.Direction.Down;public float Д=30.0f;public List<
IMyGyro>Г=new List<IMyGyro>();Base6Directions.Direction В=Ж;Base6Directions.Direction Б=И;Base6Directions.Direction А=К;
Base6Directions.Direction Џ=Ж;Base6Directions.Direction Ў=И;Base6Directions.Direction Ѝ=К;public void Ϻ(List<IMyTerminalBlock>ϫ){Г=ϫ.
ConvertAll(ċ=>(IMyGyro)ċ);if(Г.Count>0)Д=Г[0].GetMaximum<float>("Yaw");}public void Ϻ(List<IMyGyro>ϫ){Г.Clear();if(ϫ==null)return;
Г=ϫ;if(Г.Count>0)Д=Г[0].GetMaximum<float>("Yaw");}public void Ϻ(IMyProgrammableBlock Ϲ,IMyGridTerminalSystem ϸ){Г.Clear()
;if((ϸ!=null)&&(Ϲ!=null))ϸ.GetBlocksOfType<IMyGyro>(Г,ċ=>((ċ.CubeGrid==Ϲ.CubeGrid)&&ċ.IsFunctional));if(Г.Count>0)Д=Г[0].
GetMaximum<float>("Yaw");}public void Ϸ(IMyTerminalBlock ϵ,Base6Directions.Direction ϴ=К,Base6Directions.Direction ϳ=Ж){if(
Base6Directions.GetAxis(ϴ)==Base6Directions.GetAxis(ϳ))ϳ=Base6Directions.GetPerpendicular(ϴ);if(ϵ==null){}else{Vector3 ϲ=
Base6Directions.GetVector(ϴ);Vector3.TransformNormal(ref ϲ,ϵ.Orientation,out ϲ);ϴ=Base6Directions.GetDirection(ref ϲ);ϲ=Base6Directions
.GetVector(ϳ);Vector3.TransformNormal(ref ϲ,ϵ.Orientation,out ϲ);ϳ=Base6Directions.GetDirection(ref ϲ);}Џ=ϳ;Ѝ=ϴ;Ў=
Base6Directions.GetLeft(Џ,Ѝ);}public void ϰ(bool ɸ){for(int r=0;r<Г.Count;r++){Г[r].GyroOverride=ɸ;}}public void ϰ(int Ϯ,bool ɸ){if(Ϯ<Г
.Count){Г[Ϯ].GyroOverride=ɸ;}}public void ϯ(float ϭ){for(int r=0;r<Г.Count;r++){Г[r].GyroPower=ϭ;}}public void ϯ(int Ϯ,
float ϭ){if(Ϯ<Г.Count){Г[Ϯ].GyroPower=ϭ;}}public void Ϭ(bool ϱ){for(int r=0;r<Г.Count;r++){Г[r].Enabled=ϱ;}}public void Ϭ(int
Ϯ,bool ϱ){if(Ϯ<Г.Count){Г[Ϯ].Enabled=ϱ;}}public void Ѓ(bool Ђ){for(int r=0;r<Г.Count;r++){Г[r].ShowOnHUD=Ђ;}}public void
Ѓ(int Ϯ,bool Ђ){if(Ϯ<Г.Count){Г[Ϯ].ShowOnHUD=Ђ;}}void Ё(Base6Directions.Direction Ѐ,out string Ͻ,out float ϼ){Ͻ="Yaw";ϼ=-
1.0f;if(Base6Directions.GetAxis(В)==Base6Directions.GetAxis(Ѐ)){if(В==Ѐ)ϼ=1.0f;}if(Base6Directions.GetAxis(Б)==
Base6Directions.GetAxis(Ѐ)){Ͻ="Pitch";if(Б==Ѐ)ϼ=1.0f;}if(Base6Directions.GetAxis(А)==Base6Directions.GetAxis(Ѐ)){Ͻ="Roll";if(А==Ѐ){}
else ϼ=1.0f;}}public void Ͽ(IMyGyro Ϯ,string Ͻ,float ɸ){if(Ͻ=="Yaw"){Ϯ.Yaw=ɸ;}else if(Ͻ=="Pitch"){Ϯ.Pitch=ɸ;}else{Ϯ.Roll=ɸ;}
}public void Ͼ(float ɱ){for(int r=0;r<Г.Count;r++){string Ͻ;float ϼ;Vector3 ϲ=Base6Directions.GetVector(Ж);Vector3.
TransformNormal(ref ϲ,Г[r].Orientation,out ϲ);В=Base6Directions.GetDirection(ref ϲ);Ё(Џ,out Ͻ,out ϼ);Ͽ(Г[r],Ͻ,ϼ*ɱ);}}public void ϻ(
float Ɍ){for(int r=0;r<Г.Count;r++){string Ͻ;float ϼ;Vector3 ϲ=Base6Directions.GetVector(И);Vector3.TransformNormal(ref ϲ,Г[r
].Orientation,out ϲ);Б=Base6Directions.GetDirection(ref ϲ);Ё(Ў,out Ͻ,out ϼ);Ͽ(Г[r],Ͻ,ϼ*Ɍ);}}public void с(float ʑ){for(
int r=0;r<Г.Count;r++){string Ͻ;float ϼ;Vector3 ϲ=Base6Directions.GetVector(К);Vector3.TransformNormal(ref ϲ,Г[r].
Orientation,out ϲ);А=Base6Directions.GetDirection(ref ϲ);Ё(Ѝ,out Ͻ,out ϼ);Ͽ(Г[r],Ͻ,ϼ*ʑ);}}public void р(float ɱ,float Ɍ,float ʑ){
for(int r=0;r<Г.Count;r++){string Ͻ;float ϼ;Vector3 ϲ=Base6Directions.GetVector(К);Vector3.TransformNormal(ref ϲ,Г[r].
Orientation,out ϲ);А=Base6Directions.GetDirection(ref ϲ);ϲ=Base6Directions.GetVector(И);Vector3.TransformNormal(ref ϲ,Г[r].
Orientation,out ϲ);Б=Base6Directions.GetDirection(ref ϲ);ϲ=Base6Directions.GetVector(Ж);Vector3.TransformNormal(ref ϲ,Г[r].
Orientation,out ϲ);В=Base6Directions.GetDirection(ref ϲ);Ё(Џ,out Ͻ,out ϼ);Ͽ(Г[r],Ͻ,ϼ*ɱ);Ё(Ў,out Ͻ,out ϼ);Ͽ(Г[r],Ͻ,ϼ*Ɍ);Ё(Ѝ,out Ͻ,
out ϼ);Ͽ(Г[r],Ͻ,ϼ*ʑ);}}}double п(Vector3D о,IMyTerminalBlock н){double м=0;bool л=false;MatrixD к=э(н);Vector3D ä=н.
GetPosition();Vector3D Ȑ=ä+1.0*Vector3D.Normalize(к.Backward);Vector3D й=ä+1.0*Vector3D.Normalize(к.Right);Vector3D т=ä-1.0*
Vector3D.Normalize(к.Right);double ї=ђ(й,о);double і=ђ(т,о);double ѕ=ђ(й,т);double є=Vector3D.DistanceSquared(ä,о);double ѓ=
Vector3D.DistanceSquared(Ȑ,о);л=є<ѓ;м=(і-ї)/ѕ;if(!л){м+=(м<0)?-1:1;}return м;}double ђ(Vector3D k,Vector3D ʀ){return Vector3D.
Distance(k,ʀ);}MatrixD ё(IMyCubeGrid č){Vector3D ѐ=č.GridIntegerToWorld(new Vector3I(0,0,0));Vector3D я=č.GridIntegerToWorld(new
Vector3I(0,1,0))-ѐ;Vector3D ю=č.GridIntegerToWorld(new Vector3I(0,0,1))-ѐ;return MatrixD.CreateScale(č.GridSize)*MatrixD.
CreateWorld(ѐ,-ю,я);}MatrixD э(IMyCubeBlock ь){Matrix ы;ь.Orientation.GetMatrix(out ы);return ы*MatrixD.CreateTranslation(((
Vector3D)new Vector3D(ь.Min+ь.Max))/2.0)*ё(ь.CubeGrid);}bool ъ(double щ,string ш="Roll",float ч=-1,float ц=1f){float х=0;if(М.Г.
Count<1)Echo("NO GYROS!!!");float ф=60f;if(ч>0)ф=ч;if(Math.Abs(щ)>1.0){х=ф*(float)(щ)*ц;}else if(Math.Abs(щ)>.7){х=ф*(float)(
щ)/4;}else if(Math.Abs(щ)>0.5){х=0.11f*Math.Sign(щ);}else if(Math.Abs(щ)>0.1){х=0.11f*Math.Sign(щ);}else if(Math.Abs(щ)>
0.01){х=0.11f*Math.Sign(щ);}else if(Math.Abs(щ)>0.001){х=0.09f*Math.Sign(щ);}else х=0;М.Ͼ(х);if(Math.Abs(щ)<ɛ){Echo(
"DR():Aimed");М.ϰ(false);}else{М.ϰ(true);М.Ϭ(true);return false;}return true;}void у(List<IMyTerminalBlock>Ɖ,bool Ć=true){foreach(
var ʀ in Ɖ){IMyFunctionalBlock Я=ʀ as IMyFunctionalBlock;if(Я==null)continue;Я.Enabled=Ć;}}void а(List<IMyTerminalBlock>Ɖ){
foreach(var ʀ in Ɖ){IMyFunctionalBlock Я=ʀ as IMyFunctionalBlock;if(Я==null)continue;Я.Enabled=!Я.Enabled;}}string Ю="[VIEW]";
Matrix Э=new Matrix(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);List<IMyTerminalBlock>Ь=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>Ы=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ъ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Щ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>б=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ш=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>Ч=new List<IMyTerminalBlock>();IMyTerminalBlock Ц=null;MyDetectedEntityInfo Μ;string Х="CAMERAS";void Ф(ʈ À){À.ǵ(Х,
"CameraViewOnly",ref Ю,true);}bool У(List<IMyTerminalBlock>Т,double С=100,float Ɍ=0,float ɱ=0){double Р=0;Ц=null;for(int r=0;r<Т.Count;r
++){double з=((IMyCameraBlock)Т[r]).AvailableScanRange;if(з>Р){Р=з;Ц=Т[r];}}var Α=Ц as IMyCameraBlock;if(Ц==null){return
false;}if(Α.CanScan(С)){Μ=Α.Raycast(С,Ɍ,ɱ);Ц=Α;if(!Μ.IsEmpty())ͱ(Μ);return true;}else{}return false;}bool У(List<
IMyTerminalBlock>Т,Vector3D и){double Р=0;Ц=null;for(int r=0;r<Т.Count;r++){double з=((IMyCameraBlock)Т[r]).AvailableScanRange;if(з>Р){Р
=з;Ц=Т[r];}}var Α=Ц as IMyCameraBlock;if(Ц==null)return false;{Μ=Α.Raycast(и);Ц=Α;if(!Μ.IsEmpty())ͱ(Μ);return true;}}
double ж(List<IMyTerminalBlock>Т){double е=0;for(int r=0;r<Т.Count;r++){IMyCameraBlock Α=Т[r]as IMyCameraBlock;if(е<Α.
AvailableScanRange)е=Α.AvailableScanRange;}return е;}string д(IMyTerminalBlock Ŝ){Ь.Clear();Ы.Clear();Ъ.Clear();Щ.Clear();б.Clear();Ш.
Clear();Ч.Clear();if(Ŝ==null)return"\nCameras:No OrientationBlock";GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(Ч,(Ĕ=>Ĕ
.CubeGrid==Me.CubeGrid));Matrix ŝ;Ŝ.Orientation.GetMatrix(out ŝ);Matrix.Transpose(ref ŝ,out ŝ);for(int r=0;r<Ч.Count;++r)
{if(Ч[r].CustomName.Contains(Ю))continue;IMyCameraBlock Α=Ч[r]as IMyCameraBlock;Α.EnableRaycast=true;Matrix г;Α.
Orientation.GetMatrix(out г);Vector3 ŵ=Vector3.Transform(г.Forward,ŝ);if(ŵ==Э.Left){б.Add(Ч[r]);}else if(ŵ==Э.Right){Ш.Add(Ч[r]);}
else if(ŵ==Э.Backward){Ы.Add(Ч[r]);}else if(ŵ==Э.Forward){Ь.Add(Ч[r]);}else if(ŵ==Э.Up){Щ.Add(Ч[r]);}else if(ŵ==Э.Down){Ъ.
Add(Ч[r]);}}string Ï;Ï="CS:<";Ï+="F"+Ь.Count.ToString("00");Ï+="B"+Ы.Count.ToString("00");Ï+="D"+Ъ.Count.ToString("00");Ï+=
"U"+Щ.Count.ToString("00");Ï+="L"+б.Count.ToString("00");Ï+="R"+Ш.Count.ToString("00");Ï+=">";return Ï;}void в(List<
IMyTerminalBlock>Т,string Ϫ){string ʔ;for(int r=0;r<Т.Count;r++){if(!Т[r].CustomName.Contains(Ϫ)){ʔ="Camera ";if(Т.Count>1)ʔ+=(r+1).
ToString()+" ";ʔ+=Ϫ;Т[r].CustomName=ʔ;}}}List<IMyTerminalBlock>Δ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Γ=new List<
IMyTerminalBlock>();string Β(IMyTerminalBlock Ŝ){string Ï="";if(Ч.Count<1)Ï+=д(Ŝ);Δ.Clear();Γ.Clear();foreach(var Α in Ъ){if(Α.
CustomName.ToLower().Contains("fore")||Α.CustomData.ToLower().Contains("fore"))Δ.Add(Α);else if(Α.CustomName.ToLower().Contains(
"aft")||Α.CustomData.ToLower().Contains("aft"))Γ.Add(Α);}Ï+="HCS:<";Ï+="F"+Δ.Count.ToString("00");Ï+="A"+Γ.Count.ToString(
"00");Ï+=">";return Ï;}class ΐ{bool Ώ=false;bool Ύ=false;public bool Ό=false;public Vector3D Ε;Program ʣ;public double Ή=
1250;double Έ=5000;float Ά=25f;float ͽ=25f;double ͼ=5;float ͻ=3;float ͺ=0.5f;public float ͷ=0;public float Ͷ=0;float ʹ=0;
float Ί=0;List<IMyTerminalBlock>ͳ=new List<IMyTerminalBlock>();private int Ζ=0;private int Ω=1;public MyDetectedEntityInfo Μ;
public List<MyDetectedEntityInfo>Ψ=new List<MyDetectedEntityInfo>();public ΐ(Program ʚ,List<IMyTerminalBlock>Ɖ,double Χ=1250,
float Φ=45f,float Υ=45f,float Τ=2,float Σ=1,float Ρ=0.5f,double Π=5000,bool Ο=false){ʣ=ʚ;Ώ=false;Ύ=Ο;Ό=false;ͳ.Clear();Ψ.
Clear();Μ=new MyDetectedEntityInfo();foreach(var ʀ in Ɖ){if(ʀ is IMyCameraBlock){ͳ.Add(ʀ);IMyCameraBlock ǁ=ʀ as
IMyCameraBlock;ǁ.EnableRaycast=true;if(Ά>ǁ.RaycastConeLimit)Ά=ǁ.RaycastConeLimit;if(ͽ>ǁ.RaycastConeLimit)ͽ=ǁ.RaycastConeLimit;}}if(Χ>Π
)Π=Χ;Ή=Χ;Ά=Φ;ͽ=Υ;ͼ=Τ;ͻ=Σ;ͺ=Ρ;Έ=Π;ͷ=0;Ͷ=0;ʹ=0;Ί=0;Ζ=0;Ω=ͳ.Count;}public bool Ξ(){return Ώ;}void Ν(MyDetectedEntityInfo Μ){
bool Λ=true;for(int r=0;r<Ψ.Count;r++){if(Ψ[r].EntityId==Μ.EntityId)Λ=false;}if(Λ){Ψ.Add(Μ);}}public bool Κ(){if(ͳ.Count<1)Ώ
=true;if(Ώ)return false;bool Ι=false;for(int Θ=0;Θ<Ω;Θ++){if(ʣ.У(ͳ,Ή,Ί,ʹ)){Μ=ʣ.Μ;if(!Μ.IsEmpty()){bool Η=true;if((Μ.Type
==MyDetectedEntityType.LargeGrid)||(Μ.Type==MyDetectedEntityType.SmallGrid)){if(ʣ.ɢ(Μ.EntityId)){Η=false;}}if(Η){Ν(Μ);Ι=
true;}}else if(Ύ){Ώ=true;Vector3D Ͳ;Vector3D.CreateFromAzimuthAndElevation(MathHelper.ToRadians(Ͷ),MathHelper.ToRadians(ͷ),
out Ͳ);Ε=Vector3D.TransformNormal(Ͳ,ʣ.Ц.WorldMatrix);Ό=true;return false;}Ζ++;if(Ί==0&&ʹ==0){ͷ=ͺ;Ͷ=ͺ;Ζ=0;}if(Ζ>3){Ζ=0;Ͷ+=
Math.Abs(Ͷ/ͻ)+ͺ;if(Math.Abs(Ͷ)>Ά){Ζ=0;Ͷ=0;ͷ+=Math.Abs(ͷ/ͻ)+ͺ;}if(Math.Abs(ͷ)>ͽ){ͷ=0;Ͷ=0;Ζ=0;{Ή*=ͼ;if(Ή>Έ){Ώ=true;return
false;}}}}switch(Ζ){case 0:Ί=ͷ;ʹ=Ͷ;break;case 1:Ί=-ͷ;ʹ=Ͷ;break;case 2:Ί=ͷ;ʹ=-Ͷ;break;case 3:Ί=-ͷ;ʹ=-Ͷ;break;}}}return Ι;}}
const int ˁ=0;const int ʫ=2;const int ʿ=4;const int ʾ=8;const int ʽ=16;const int ʼ=32;const int ʻ=64;const int ʺ=128;const
int ʹ=256;const int ʸ=512;const int ʷ=1024;const int ʶ=2048;const int ˀ=0xfff;string ʵ(){string ʳ="FLAGS:";if((Ѫ&ʫ)>0)ʳ+=
"SLED ";if((Ѫ&ʼ)>0)ʳ+="ORBITAL ";if((Ѫ&ʻ)>0)ʳ+="ROCKET ";if((Ѫ&ʿ)>0)ʳ+="ROTOR ";if((Ѫ&ʾ)>0)ʳ+="WHEEL ";if((Ѫ&ʺ)>0)ʳ+="PET ";if(
(Ѫ&ʹ)>0)ʳ+="NAD ";if((Ѫ&ʸ)>0)ʳ+="NO Gyro ";if((Ѫ&ʶ)>0)ʳ+="No Tank ";if((Ѫ&ʷ)>0)ʳ+="No Power ";return ʳ;}long ʲ=0;MyIni ʱ=
new MyIni();string ʰ="";string ʯ="";void ʮ(){if(ѭ==null){ʯ=Storage;}else{ʯ=ѭ.GetText();}if(Ѭ==null)return;if(ʯ==ʰ){Echo(
"Load Skip");return;}ʰ=ʯ;ʯ=ʯ.Trim();MyIniParseResult ʭ;if(!ʱ.TryParse(ʯ,out ʭ)){Echo("MyIni:Error parsing INI:"+ʭ.ToString());
string[]ʗ=ʯ.Split('\n');for(int ʖ=0;ʖ<ʗ.Count();ʖ++){Echo(ʖ+1+":"+ʗ[ʖ]);}}Ѭ.ʙ(ʯ);Ѭ.ǵ(ѯ,"SaveID",ref ʲ);if(ʬ()){Ѭ.ʙ("");}Ї(Ѭ);Ѭ
.ǵ(ѯ,"Mode",ref Ն,true);Ѭ.ǵ(ѯ,"current_state",ref ѫ,true);Ѭ.ǵ(ѯ,"PassedArgument",ref Ѩ,true);Ѭ.ǵ(ѯ,"AlertStates",ref ѧ,
true);Ѭ.ǵ(ѯ,"craft_operation",ref Ѫ,true);Ѭ.ǵ(ѯ,"PassedArgument",ref Ѩ);Ѭ.ǵ(ѯ,"ReceivedMessage",ref Ѧ);}bool ʬ(){if(ѭ==null
||ӣ)return false;if(ʲ<=0||ʲ==(long)ѭ.EntityId)return false;else return true;}bool ʴ(string ˆ){ˆ=ˆ.Trim().ToLower();return(
ˆ=="True"||ˆ=="true");}Dictionary<long,MyDetectedEntityInfo>ˎ=new Dictionary<long,MyDetectedEntityInfo>();void ͱ(
MyDetectedEntityInfo ˮ){if(ˮ.EntityId!=0){if(!ˎ.ContainsKey(ˮ.EntityId)){ˎ.Add(ˮ.EntityId,ˮ);}else{ˎ[ˮ.EntityId]=ˮ;}}else Echo(
"Not adding: Zero Entity");}string ˬ(MyDetectedEntityInfo ˤ){string Ï="";Ï+="ETBV";Ï+=":"+ˤ.EntityId.ToString();Ï+=":"+ˤ.TimeStamp;Vector3D ˣ=ˤ.
BoundingBox.Min;Ï+=":"+ǆ(ˣ);Vector3D ˢ=ˤ.BoundingBox.Max;Ï+=":"+ǆ(ˢ);Vector3D ˡ=(Vector3)ˤ.Velocity;Ï+=":"+ǆ(ˡ);return Ï;}void ˠ(){
Echo("mode="+Ն.ToString()+" state="+ѫ.ToString());Echo("Commands="+հ.Count.ToString());if(հ.Count>0)Echo("Next="+հ[0].ղ);if(
Ն==Կ){ˊ();return;}if(Ն==Ԕ){ά();return;}if(Ն==Ԟ){Ϋ();return;}if(Ն==ԓ){Ϣ();return;}}void ˑ(){Ա(DateTime.Now.ToString()+
" ACTION: Reset To Idle",ψ,true);О();ԑ(Յ);}void ː(){Ա(Њ+" Manual Control",φ);}bool Ͱ=false;bool ˏ=false;bool ˍ=false;Vector3D ˌ;Vector3D ˋ;void
ˊ(){Ա("clear",φ);Ա(Њ+":Going Target!",φ);Echo("Going Target: state="+ѫ.ToString());if(ղ!="")Echo(ղ);string ˉ="";ˉ+=
"GT:S="+ѫ;if(ѫ==0){ơ();if((Ѫ&ʫ)>0){Ͱ=true;if(լ>45)լ=45;}else Ͱ=false;if((Ѫ&ʿ)>0){ˏ=true;if(լ>15)լ=15;}else ˏ=false;if((Ѫ&ʾ)>0){
ˍ=true;}else ˍ=false;М.Ϸ(Ƀ);double ˈ=0;((IMyShipController)Ƀ).TryGetPlanetElevation(MyPlanetElevation.Surface,out ˈ);if(!
Ͱ&&!ˏ){if(ב<0)ב=75;}if(ծ){if(ˈ>ӕ.ӧ()){ѫ=150;}else ѫ=160;}else ԑ(Ղ);Ү=true;}else if(ѫ==150){Ү=true;if(ҧ>0){double ˈ=0;((
IMyShipController)Ƀ).TryGetPlanetElevation(MyPlanetElevation.Surface,out ˈ);ˉ+=" E="+ˈ.ToString("0.0");float ˇ=ɛ;ɛ=0.1f;Vector3D Ò=(Ƀ as
IMyShipController).GetNaturalGravity();bool ɗ=ɚ("",Ò,Ƀ);ˉ+=" Aligned="+ɗ.ToString();Echo("bAligned="+ɗ.ToString());ɛ=ˇ;if(ɗ||ˈ<ӕ.ӧ()*2){ʋ
();if(ב>0)ѫ=155;else ѫ=160;}}else ѫ=160;}else if(ѫ==151){Ү=true;if(ҧ>0||Ƙ){double ˈ=0;((IMyShipController)Ƀ).
TryGetPlanetElevation(MyPlanetElevation.Surface,out ˈ);ˉ+=" E="+ˈ.ToString("0.0");float ˇ=ɛ;ɛ=0.1f;Vector3D Ò=(Ƀ as IMyShipController).
GetNaturalGravity();bool ɗ=ɚ("",Ò,Ƀ);ˉ+=" Aligned="+ɗ.ToString();Echo("bAligned="+ɗ.ToString());ɛ=ˇ;if(ɗ||ˈ<ӕ.ӧ()*2){ʋ();if(ב>0)ѫ=155;
else ѫ=160;}else ѫ=150;}else ѫ=160;}else if(ѫ==155){Ү=true;if(ˍ){ѫ=160;return;}if(ҧ>0){Vector3D Ò=(Ƀ as IMyShipController).
GetNaturalGravity();bool ɗ=ɚ("",Ò,Ƀ);ˉ+=" Aligned="+ɗ.ToString();double Ñ=-999;Ñ=п(կ,Ƀ);bool Ó=Math.Abs(Ñ)<0.1;Echo("yawangle="+Ñ.
ToString());ˉ+=" Yaw="+Ñ.ToString("0.00");Echo("bAimed="+Ó.ToString()+" bAligned="+ɗ.ToString());if(!Ó){if(ƚ){Echo("Rotor");ҁ(Ñ)
;}else{ъ(Ñ,"Yaw");}}if(ɗ&&Ó){ʋ();ѫ=160;}else if(ɗ&&Math.Abs(Ñ)<0.5){float Ϗ;float Ϙ;float ώ;Ŏ(ś,out Ϗ,out Ϙ,out ώ);Ϗ+=1;Ϙ
+=1;ώ+=1;ŀ(ś,Ϗ,Ũ);ŀ(ś,Ϙ,ŧ);ŀ(ś,ώ,Ŧ);}else ő(ś);}else ѫ=160;}else if(ѫ==156){Ү=true;Vector3D Ò=(Ƀ as IMyShipController).
GetNaturalGravity();bool Ó=ɚ("",Ò,Ƀ);if(Ó){ʋ();ѫ=160;}}else if(ѫ==160){Echo("Moving to Target");Vector3D æ=կ;Vector3D G=æ-Ƀ.GetPosition()
;double D=G.Length();Echo("distance="+Ս(D));Echo("velocity="+Ҩ.ToString("0.00"));Ա("clear",ς);string ϗ="Moving to Target"
;if(ղ!="")ϗ="Moving to "+ղ;Ա(ϗ+"\nD:"+Ս(D)+" V:"+Ҩ.ToString(Ј),ς);Ա(ϗ+"\nDistance: "+Ս(D)+"\nVelocity: "+Ս(Ҩ)+"/s",φ);if(
Ԉ&&(D<ճ)){ѫ=500;Echo("we have arrived");Ү=true;return;}bool ϖ=true;if(ב>0&&ҧ>0){double ˈ=0;MyShipVelocities ϕ=((
IMyShipController)Ƀ).GetShipVelocities();Vector3D ϔ=ϕ.LinearVelocity;var ϓ=((IMyShipController)Ƀ).WorldMatrix.Up;var ϒ=Vector3D.Dot(ϔ,ϓ);
((IMyShipController)Ƀ).TryGetPlanetElevation(MyPlanetElevation.Surface,out ˈ);ˉ+=" E="+ˈ.ToString("0.0");ˉ+=" V="+Ҩ.
ToString("0.00");Echo("Elevation="+ˈ.ToString("0.0"));Echo("MinEle="+ב.ToString("0.0"));double ϑ=0;if(ϒ<0){ϑ=ƥ(Ř,Math.Abs(ϒ),ҧ);
}double ϐ=ƥ(Ř,ӡ,ҧ);float Ϗ;float Ϙ;float ώ;Ŏ(Ř,out Ϗ,out Ϙ,out ώ);if(ב>0){if(ϒ<-0.5&&(ˈ-ϑ*2)<ב){ˉ+=" EM UP!";Vector3D Ò=(
Ƀ as IMyShipController).GetNaturalGravity();bool ɗ=ɚ("",Ò,Ƀ);ŀ(Ř,100);ϖ=false;Ү=true;}else if(ˈ<ב){Ϗ+=Math.Min(5f,(float)
լ);Ϙ+=Math.Min(5f,(float)լ);ώ+=Math.Min(5f,(float)լ);ˉ+=" UP! A"+Ϗ.ToString("0.00");ŀ(Ř,Ϗ,Ũ);ŀ(Ř,Ϙ,ŧ);ŀ(Ř,ώ,Ŧ);}else if(ˈ
>(ϐ+ב*1.25)){ˉ+=" SUPERHIGH";ő(Ř,Ť,true);Vector3D Ò=(Ƀ as IMyShipController).GetNaturalGravity();bool ɗ=ɚ("",Ò,Ƀ);if(!ɗ){
Ү=true;ϖ=false;}}else if(ˈ>ב*2){ˉ+=" HIGH";if(ϒ>2){ˉ+=" ^";ő(Ř,Ť,true);}else if(ϒ<-0.5){ˉ+=" v";if(ϒ>(-Math.Min(15,լ))){Ϗ
-=Math.Max(25f,Math.Min(5f,(float)Ҩ/2));Ϙ-=Math.Max(25f,Math.Min(5f,(float)Ҩ/2));ώ-=Math.Max(25f,Math.Min(5f,(float)Ҩ/2));
ˉ+=" DOWN! A"+Ϗ.ToString("0.00");}else{Ϗ+=Math.Max(100f,Math.Min(5f,(float)Ҩ/2));Ϙ+=Math.Max(100f,Math.Min(5f,(float)Ҩ/2)
);ώ+=Math.Max(100f,Math.Min(5f,(float)Ҩ/2));ˉ+=" 2FAST! A"+Ϗ.ToString("0.00");Vector3D Ò=(Ƀ as IMyShipController).
GetNaturalGravity();bool ɗ=ɚ("",Ò,Ƀ);if(!ɗ){Ү=true;ϖ=false;}}}else{ˉ+=" -";Ϗ-=5;Ϙ-=5;ώ-=5;}ŀ(Ř,Ϗ,Ũ);ŀ(Ř,Ϙ,ŧ);ŀ(Ř,ώ,Ŧ);}else{ő(Ř);}}}if(ϖ)
{Echo("Do Travel");Ɠ(æ,(float)ճ,500,300);}else{ő(ś);}}else if(ѫ==300){Ү=true;Vector3D æ=կ;ơ();ç(æ);ѫ=320;}else if(ѫ==301)
{Ү=false;}else if(ѫ==320){Echo("Primary Collision Avoid");Ա("clear",ς);Ա("Collision Avoid",ς);Ա("Collision Avoid",φ);Ɠ(è,
5.0f,160,340);}else if(ѫ==340){if(Μ.Type==MyDetectedEntityType.LargeGrid||Μ.Type==MyDetectedEntityType.SmallGrid){ѫ=345;}
else if(Μ.Type==MyDetectedEntityType.Asteroid){ѫ=350;}else ѫ=300;Ү=true;}else if(ѫ==345){Vector3D[]ϩ=new Vector3D[
BoundingBoxD.CornerCount];BoundingBoxD Ϩ=Μ.BoundingBox;Ϩ.GetCorners(ϩ);ˌ=ȼ(ϩ[3],ϩ[4],ϩ[7]);ˋ=ȼ(ϩ[0],ϩ[1],ϩ[4]);Ү=true;ѫ=348;}else if
(ѫ==348){Ү=true;if(ɚ("up",ˌ,Ƀ)){ѫ=349;}}else if(ѫ==349){Ү=true;if(ɚ("right",ˋ,Ƀ)){ѫ=350;}}else if(ѫ==350){M(Ƈ);ơ();խ=
DateTime.Now;ѫ=360;Ү=true;}else if(ѫ==360){Ա("Collision Avoid\nScan for escape route",φ);DateTime ϧ=խ.AddSeconds(5.0f);DateTime
Ϧ=DateTime.Now;if(DateTime.Compare(Ϧ,ϧ)>0){ԑ(Ղ);ƍ();return;}if(I()){Echo("ESCAPE!");ѫ=380;}ҭ=true;}else if(ѫ==380){Ա(
"Collision Avoid Travel",φ);Echo("Escape Collision Avoid");Ɠ(è,1f,160,340);}else if(ѫ==500){{Ա("clear",ς);Ա("Arrived at Target",ς);Ա(
"Arrived at Target",φ);ˉ+=" ARRIVED!";О();ծ=false;ĥ(false);ȫ();ԑ(ի);ѫ=մ;ի=Ӱ;մ=0;ղ="";Ԉ=true;if(ד){var ϥ=ɪ<IMyTerminalBlock>("NAV:");for(int
u=0;u<ϥ.Count();u++){if(ϥ[u].CustomName.StartsWith("NAV:")){Echo("Found NAV: command:");ϥ[u].CustomName=
"NAV: C Arrived Target";}}}}Ү=true;ƍ();}ւ(ˉ);}void Ϥ(float B){if(ˏ){դ(B);}else ŀ(ś,B);}void ϣ(){ő(ҿ);ҋ();}void Ϣ(){Echo("ScanTest");Ա("clear",τ
);switch(ѫ){case 0:Echo("Init");M(true);ѫ=100;Ү=true;break;case 100:Echo("Scanning for Escape");ҭ=true;if(!I())ѫ=500;
break;case 500:Echo("Escape Found!");Ծ("EscapeTarget",è);break;}}void ϡ(){if(հ.Count<1){ԑ(ի);ѫ=մ;ի=Ӱ;մ=0;return;}հ[0].ձ();հ.
RemoveAt(0);}IMyBroadcastListener Ϡ;IMyBroadcastListener ϟ;IMyBroadcastListener Ϟ;IMyBroadcastListener ϝ;IMyBroadcastListener Ϝ;
void ϛ(){Ϡ=IGC.RegisterBroadcastListener(ԏ);Ϡ.SetMessageCallback(ԏ);ϟ=IGC.RegisterBroadcastListener(Ԏ);ϟ.SetMessageCallback(
Ԏ);Ϟ=IGC.RegisterBroadcastListener(ԍ);Ϟ.SetMessageCallback(ԍ);ϝ=IGC.RegisterBroadcastListener(Ԍ);ϝ.SetMessageCallback(Ԍ);
Ϝ=IGC.RegisterBroadcastListener(Ԋ);Ϝ.SetMessageCallback(Ԋ);}void Ϛ(){հ.Clear();}void ϙ(Vector3D È,int δ=Ԟ,int Ϊ=0,double
β=50,string α="",double d=9999,bool ΰ=true){if(d>ӡ)d=ӡ;ն ή=new ն{ʚ=this,կ=È,ծ=true,ի=δ,մ=Ϊ,ճ=β,լ=d,ղ=α};if(ΰ)ή.յ=Փ.Ք;else
ή.յ=Փ.ս;հ.Add(ή);}void η(){ն ή=new ն{ʚ=this,յ=Փ.պ};հ.Add(ή);}void ζ(){ն ή=new ն{ʚ=this,յ=Փ.շ};հ.Add(ή);}void ε(Vector3D È
,int δ=Ӱ,int Ϊ=0,double β=50,string α="",double d=9999,bool ΰ=true){ϙ(È,δ,Ϊ,β,α,d,ΰ);έ();}void ί(){ն ή=new ն{ʚ=this,յ=Փ.ո
};հ.Add(ή);}void έ(){if(հ.Count>0){ԑ(Ԕ);}else{ԑ(Ղ);Echo("No Nav to start");}}void ά(){Echo("Start Nav: state="+ѫ.ToString
());ϡ();Ү=true;}void Ϋ(){Echo("Next Nav: state="+ѫ.ToString());ϡ();Ү=true;}string γ="LOGGING";void θ(ʈ À){À.ǵ(γ,
"TextPanelReport",ref υ,true);À.ǵ(γ,"StatusName",ref ω,true);À.ǵ(γ,"LongStatus",ref χ,true);À.ǵ(γ,"RangeReport",ref ϋ,true);À.ǵ(γ,
"SledReport",ref ό,true);À.ǵ(γ,"GPSTag",ref σ,true);}ο ύ=null;string ϋ="[RANGE]";ο ϊ=null;string ω="Wico Craft Status";ο ψ=null;
string χ="Wico Craft Log";ο φ=null;string υ="Craft Report";ο τ=null;string σ="[GPS]";ο ς=null;string ό="[SMREPORT]";bool ρ=
false;bool π=false;class ο{Program ʣ;string ξ="";List<IMyTextPanel>ν=new List<IMyTextPanel>();string μ="";string λ="";bool κ=
false;bool ι=true;public ο(Program ʚ,string ʔ,bool Ե=false){ʣ=ʚ;ξ=ʔ;κ=Ե;ι=true;μ="";λ="";ν.Clear();ν=ʣ.ɋ(ξ);if(ν.Count<1)ν=ʣ.
ɩ(ξ);}public void Ա(string Ԥ,bool Ԧ=false){if(Ԥ=="clear"){μ="";λ="X";ι=false;return;}if(κ&&ι){ι=false;if(ν.Count>0){μ=ν[0
].GetText();λ="X";}}if(Ԧ){μ=Ԥ+"\n"+μ;}else μ+=Ԥ+"\n";}public void Լ(){if(λ!=μ){ι=true;foreach(var Ի in ν){Ի.WriteText(μ);
}λ=μ;}}}void Ժ(){ϊ=Դ(true);ψ=Է(χ,true);;φ=Է(υ);ύ=Է(ϋ);τ=Է(σ,ӣ);ς=Է(ό);ρ=true;}void Թ(){if(ϊ!=null)Ա("clear",ϊ);if(ψ!=null
)Ա("clear",ψ);if(φ!=null)Ա("clear",φ);if(ύ!=null)Ա("clear",ύ);if(τ!=null)Ա("clear",τ);if(ς!=null)Ա("clear",ς);}void Ը(){
if(ϊ!=null)ϊ.Լ();if(ψ!=null)ψ.Լ();if(φ!=null)φ.Լ();if(ύ!=null)ύ.Լ();if(τ!=null)τ.Լ();if(ς!=null)ς.Լ();}ο Է(string Զ,bool Ե
=false){ο Խ=new ο(this,Զ,Ե);return Խ;}ο Դ(bool Բ=false){if((ϊ!=null||ρ)&&!Բ)return ϊ;ϊ=Է(ω);return ϊ;}void Ա(string Ԥ,ο ԧ
,bool Ԧ=false){if(ԧ==null)return;ԧ.Ա(Ԥ,Ԧ);}void ԥ(string Ԥ){Ա(Ԥ,Դ());if(π&&Ԥ!="clear")Echo(Ԥ);}string ԣ(double Ԣ){int ԡ=
75;if(Ԣ<0)Ԣ=0;int Գ=(int)(Ԣ*ԡ)/100;if(Գ>ԡ)Գ=ԡ;string ʳ="["+new String('|',Գ)+new String('\'',ԡ-Գ)+"]";return ʳ;}void Ծ(
string ʔ,Vector3D ʅ){string Ǒ;Ǒ="GPS:"+ʔ+":"+ǆ(ʅ)+":";Ա(Ǒ,τ);}string Ւ(string Ց,string Ր){string Ï;int Տ=Ց.Length;int Վ=Ր.
Length;if(Տ+Վ>32){if(Վ>31)return"INVALID";Տ=32-Վ;}Ï=Ց.Substring(0,Տ)+Ր;Ï.Replace(":","_");Ï.Replace(";","_");return Ï;}string
Ս(double Ռ){string Ջ="";if(Ռ>1000){Ջ=Ռ.ToString("N0")+"km";}else if(Ռ>10){Ջ=Ռ.ToString("0.0")+"m";}else{Ջ=Ռ.ToString(
"0.000")+"m";}return Ջ;}void Պ(){ϛ();}void Չ(ʈ ӛ){ĳ(ӛ);ş(ӛ);Ɉ(ӛ);Ф(ӛ);Á(ӛ);ֆ(ӛ);}string Ո(){Echo("Init:"+ұ);if(ұ==0){Ա(DateTime
.Now.ToString()+Ћ+":"+Њ+":INIT",ψ,true);if(!ӥ.ContainsKey("scantest"))ӥ.Add("scantest",ԓ);Ɂ+=ѥ();ʮ();Ɂ+=þ();Ɂ+=ɂ();Ժ();}
else if(ұ==1){Ɂ+=Ş(Ƀ);Ɂ+=զ();Ɂ+=ȥ(Ƀ);Ɂ+=Ǟ(Ƀ);Ɂ+=д(Ƀ);Ɂ+=Ĳ();}else if(ұ==2){Ɂ+=ʏ();М.Ϻ(ɜ);М.Ϸ(Ƀ);Ɂ+=Ǆ();Ɂ+=д(Ƀ);Ѿ(Ƀ);Ӕ(Ƀ);Ɂ+=
Շ();ҳ=true;}ұ++;if(ҳ){ұ=0;}ԥ(Ɂ);return Ɂ;}string Շ(){return">";}int Ն=-1;const int Յ=0;const int Մ=1;const int Ճ=2;const
int Ղ=3;const int Ձ=4;const int Հ=5;const int Կ=7;const int Ԡ=8;const int ԅ=9;const int ӭ=13;const int ԃ=10;const int Ԃ=11;
const int ԁ=12;const int Ԁ=14;const int ӿ=15;const int Ӿ=16;const int ӽ=17;const int Ӽ=18;const int ӻ=19;const int Ӻ=20;const
int Ԅ=21;const int ӹ=22;const int ӷ=23;const int Ӷ=24;const int ӵ=25;const int Ӵ=26;const int ӳ=27;const int Ӳ=28;const int
ӱ=29;const int Ӱ=30;const int ӯ=31;const int Ӯ=33;const int Ӹ=50;const int Ԇ=60;const int Ԓ=111;const int ԟ=112;const int
ԝ=200;const int Ԝ=210;const int ԛ=220;const int Ԛ=225;const int ԙ=290;const int Ԙ=400;const int ԗ=410;const int Ԗ=500;
const int ԕ=510;const int Ԕ=600;const int Ԟ=610;const int ԓ=999;void ԑ(int Ԑ){if(Ն==Ԑ)return;Ն=Ԑ;ѫ=0;ƍ();}const string ԏ=
"WICOB_NAVADDTARGET";const string Ԏ="WICOB_NAVSTART";const string ԍ="WICOB_NAVRESET";const string Ԍ="WICO_NAVLAUNCH";const string ԋ=
"WICO_NAVDOCK";const string Ԋ="WICO_NAVORBITALLAUNCH";const string ԉ="WICO_NAVLAND";bool Ԉ=false;public enum Փ{ԇ,Ք,ս,ռ,ջ,պ,չ,ո,շ};
class ն{public Program ʚ;public Փ յ=Փ.ԇ;public Vector3D կ;public bool ծ=false;public int ի=Ӱ;public int մ=0;public double լ=
9999;public double ճ=50;public string ղ="";public bool ձ(){switch(յ){case Փ.Ք:{ʚ.կ=կ;ʚ.ծ=true;ʚ.ի=ի;ʚ.մ=մ;ʚ.ճ=ճ;ʚ.ղ=ղ;ʚ.լ=լ;
ʚ.Ԉ=true;ʚ.Ұ+="Going to:"+ʚ.ղ;ʚ.Ұ+=" arrivald="+ʚ.ճ.ToString();ʚ.ԑ(Կ);}break;case Փ.ս:{ʚ.կ=կ;ʚ.ծ=true;ʚ.ի=ի;ʚ.մ=մ;ʚ.ղ=ղ;ʚ
.Ԉ=false;ʚ.ԑ(Կ);}break;case Փ.ջ:{ʚ.լ=լ;ʚ.ԑ(Ԟ);}break;case Փ.ռ:{ʚ.ճ=ճ;ʚ.ԑ(Ԟ);}break;case Փ.պ:{ʚ.ԑ(Հ);}break;case Փ.շ:{ʚ.ԑ(
Ӳ);}break;case Փ.ո:{ʚ.ԑ(ԅ);}break;case Փ.չ:{ʚ.ԑ(ӱ);}break;case Փ.ԇ:{ʚ.Echo("Unknown Command");ʚ.ԑ(Ղ);return true;}}return
false;}}List<ն>հ=new List<ն>();Vector3D կ;bool ծ=false;DateTime խ;double լ=9999;double ճ=50;int ի=Ӱ;int մ=0;string ղ="";bool
ז=true;bool ו=true;bool ה=true;bool ד=false;bool ג=false;float ב=-1;bool א=true;string և="NAV";void ֆ(ʈ À){À.ǵ(և,
"DTMDebug",ref ז,true);À.ǵ(և,"CameraCollision",ref ו,true);À.ǵ(և,"SensorCollision",ref ה,true);À.ǵ(և,"NAVEmulateOld",ref ד,true);À
.ǵ(և,"NAVGravityMinElevation",ref ב,true);À.ǵ(և,"NavBeaconDebug",ref א,true);À.ǵ(և,"AllowBlindNav",ref ג,true);if(լ>ӡ)լ=ӡ
;}void օ(ʈ À){À.ǰ(և,"vTarget",կ);À.ǰ(և,"ValidNavTarget",ծ);À.ǰ(և,"TargetName",ղ);À.ǰ(և,"dStartShip",խ);À.ǰ(և,
"shipSpeedMax",լ);À.ǰ(և,"arrivalDistanceMin",ճ);À.ǰ(և,"NAVArrivalMode",ի);À.ǰ(և,"NAVArrivalState",մ);}void ք(ʈ À){À.ǵ(և,"vTarget",ref
կ,true);À.ǵ(և,"ValidNavTarget",ref ծ,true);À.ǵ(և,"TargetName",ref ղ,true);À.ǵ(և,"dStartShip",ref խ,true);À.ǵ(և,
"shipSpeedMax",ref լ,true);À.ǵ(և,"arrivalDistanceMin",ref ճ,true);À.ǵ(և,"NAVArrivalMode",ref ի,true);À.ǵ(և,"NAVArrivalState",ref մ,
true);}List<IMyBeacon>փ=new List<IMyBeacon>();void ւ(string ց){if(א){if(փ.Count<1)GridTerminalSystem.GetBlocksOfType(փ);
foreach(var ր in փ){ր.CustomName=ց;}}}void տ(){IGC.SendBroadcastMessage(ԍ,"",TransmissionDistance.CurrentConstruct);}void վ(
Vector3D È,int δ=Ԟ,int Ϊ=0,double β=50,string α="",double d=9999,bool ΰ=true){string ա=ՙ(È,δ,Ϊ,β,α,d,ΰ);IGC.SendBroadcastMessage
(ԏ,ա,TransmissionDistance.CurrentConstruct);}void բ(Vector3D È,int δ=Ӱ,int Ϊ=0,double β=50,string α="",double d=9999,bool
ΰ=true){string ա=ՙ(È,δ,Ϊ,β,α,d,ΰ);IGC.SendBroadcastMessage(Ԏ,ա,TransmissionDistance.CurrentConstruct);}void գ(){IGC.
SendBroadcastMessage(Ԏ,"",TransmissionDistance.CurrentConstruct);}string ՙ(Vector3D È,int δ=Ԟ,int Ϊ=0,double β=50,string α="",double d=9999,
bool ΰ=true){string Օ="";Օ+=ǆ(È);Օ+="\n";Օ+=δ.ToString();Օ+="\n";Օ+=Ϊ.ToString();Օ+="\n";Օ+=β.ToString();Օ+="\n";Օ+=α;Օ+=
"\n";Օ+=d.ToString();Օ+="\n";Օ+=ΰ.ToString();Օ+="\n";return Օ;}void Ֆ(string Օ,out Vector3D È,out int δ,out int Ϊ,out double
β,out string α,out double d,out bool ΰ){Օ=Օ.Trim();string[]ժ=Օ.Split('\n');string[]Ǔ=ժ[0].Split(',');if(Ǔ.Length<3){Ǔ=ժ[0
].Split(':');}double ċ,ǋ,Ǌ;int њ=0;bool ǉ=double.TryParse(Ǔ[њ++].Trim(),out ċ);bool ǈ=double.TryParse(Ǔ[њ++].Trim(),out ǋ
);bool Ǉ=double.TryParse(Ǔ[њ++].Trim(),out Ǌ);if(!ǉ||!ǈ||!Ǉ){Echo("Invalid Command:("+ժ[0]+")");}È=new Vector3D(ċ,ǋ,Ǌ);
int.TryParse(ժ[1],out δ);int.TryParse(ժ[2],out Ϊ);double.TryParse(ժ[3],out β);α=ժ[4];double.TryParse(ժ[5],out d);ΰ=true;if(
ժ.Length>5)bool.TryParse(ժ[6],out ΰ);}List<IMyTerminalBlock>թ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ը=new
List<IMyTerminalBlock>();List<IMyTerminalBlock>է=new List<IMyTerminalBlock>();string զ(){թ.Clear();ը.Clear();է.Clear();
GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(թ,ɣ);for(int r=0;r<թ.Count;r++){if(թ[r].CustomName.Contains("[LEFT]")||թ[r].CustomData.
Contains("[LEFT]")){ը.Add(թ[r]);}else if(թ[r].CustomName.Contains("[RIGHT]")||թ[r].CustomData.Contains("[RIGHT]")){է.Add(թ[r]);}
}return"NR:L"+ը.Count.ToString("0")+"R"+է.Count.ToString("0");}bool դ(float Ƞ){if(ը.Count<1)return false;float ҍ=ը[0].
GetMaximum<float>("Velocity");var ď=ը[0]as IMyMotorStator;float ե=ď.TargetVelocityRPM;float ț=(ե/ҍ*100);ț=Math.Abs(ț);if(Ƞ>(ț+5f))
Ƞ=ț+5;if(Ƞ<(ț-5))Ƞ=ț-5;if(Ƞ<0f)Ƞ=0f;if(Ƞ>100f)Ƞ=100f;if(Math.Abs(Ƞ)>0){դ(ը,-Ƞ);դ(է,Ƞ);return true;}else return false;}
bool դ(List<IMyTerminalBlock>Ҋ,float Ƞ){for(int r=0;r<Ҋ.Count;r++){var ď=Ҋ[r]as IMyMotorStator;float ҍ=ď.GetMaximum<float>(
"Velocity");if(!ď.Enabled)ď.Enabled=true;float Ҍ=ҍ*(Ƞ/100.0f);ď.TargetVelocityRPM=Ҍ;}return true;}bool ҋ(){ҋ(ը);ҋ(է);return true;}
bool ҋ(List<IMyTerminalBlock>Ҋ){for(int r=0;r<Ҋ.Count;r++){IMyMotorStator ď=Ҋ[r]as IMyMotorStator;ď.TargetVelocityRPM=0;}
return true;}bool ҁ(double м){float Ƞ;if(Math.Abs(м)>1.0){Ƞ=50;}else if(Math.Abs(м)>.7){Ƞ=50;}else if(Math.Abs(м)>0.5){Ƞ=30;}
else if(Math.Abs(м)>0.1){Ƞ=20;}else if(Math.Abs(м)>0.01){Ƞ=5;}else if(Math.Abs(м)>0.001){Ƞ=0;}else Ƞ=0;Ƞ/=3;Ƞ=Ƞ*-Math.Sign(м
);if(Math.Abs(Ƞ)>0){դ(ը,Ƞ);}if(Math.Abs(Ƞ)>0){դ(է,Ƞ);}if(Math.Abs(Ƞ)>0)return false;else return true;}Ѽ Ҁ;void Ѿ(
IMyTerminalBlock ѽ=null){if(ѽ==null)ѽ=Ƀ;if(ѽ==null)return;Ҁ=new Ѽ(ѽ);}struct Ѽ{public Vector3D[]ѻ;Vector3D Ѻ;Vector3D ѹ;public Vector3D
Ѹ;static int[]ѷ={1,3,5,7};static int[]Ѷ={0,2,4,6};static int[]ѵ={2,3,6,7};static int[]ѿ={0,1,4,5};static int[]Ѵ={4,5,6,7}
;static int[]Ҏ={0,1,2,3};static int[][]ҡ={ѷ,Ѷ,ѵ,ѿ,Ѵ,Ҏ};public const int Ҡ=0;public const int ҟ=1;public const int Ҟ=2;
public const int ҝ=3;public const int Ҝ=4;public const int қ=5;public Ѽ(IMyTerminalBlock ĉ){ѻ=new Vector3D[8];if(ĉ==null){Ѹ=
new Vector3D();ѹ=new Vector3D();Ѻ=new Vector3D();return;}ѹ=new Vector3D(ĉ.CubeGrid.Min)-new Vector3D(0.5,0.5,0.5);ѹ*=ĉ.
CubeGrid.GridSize;Ѻ=new Vector3D(ĉ.CubeGrid.Max)+new Vector3D(0.5,0.5,0.5);Ѻ*=ĉ.CubeGrid.GridSize;var Қ=ĉ.WorldMatrix.
GetOrientation();var ҙ=ĉ.CubeGrid.WorldMatrix.GetOrientation()*MatrixD.Transpose(Қ);Vector3D.TransformNormal(ref ѹ,ref ҙ,out ѹ);
Vector3D.TransformNormal(ref Ѻ,ref ҙ,out Ѻ);var Ҙ=Vector3D.Min(ѹ,Ѻ);Ѻ=Vector3D.Max(ѹ,Ѻ);ѹ=Ҙ;var җ=ĉ.CubeGrid.GetPosition();
Vector3D Җ;Vector3D ҕ;Җ=ѹ;Vector3D.TransformNormal(ref Җ,ref Қ,out Җ);Җ+=җ;ҕ=Ѻ;Vector3D.TransformNormal(ref ҕ,ref Қ,out ҕ);ҕ+=җ;
BoundingBox Ҕ=new BoundingBox(Җ,ҕ);Ѹ=Ҕ.Center;Vector3D ғ;for(int r=0;r<8;r++){ғ.X=((r&1)==0?ѹ:Ѻ).X;ғ.Y=((r&2)==0?ѹ:Ѻ).Y;ғ.Z=((r&4)
==0?ѹ:Ѻ).Z;Vector3D.TransformNormal(ref ғ,ref Қ,out ғ);ғ+=җ;ѻ[r]=ғ;}}public void Ғ(int ґ,Vector3D[]Ê,int ʌ=0){ґ%=ҡ.Length;
for(int r=0;r<ҡ[ґ].Length;r++){Ê[ʌ++]=ѻ[ҡ[ґ][r]];}}}string Ґ="";bool ҏ(string ѣ){Ґ="";if(ѣ==""||ѣ=="timer"||ѣ=="wccs"||ѣ==
"wcct"){if(Ѩ!=""&&Ѩ!="timer"){ѣ=Ѩ;}if(ד){var ϥ=ɪ<IMyTerminalBlock>("NAV:");for(int u=0;u<ϥ.Count();u++){if(ϥ[u].CustomName.
StartsWith("NAV:")){Echo("Found NAV: command:");ѣ=ϥ[u].CustomName.Substring("NAV:".Length);break;}}}}if(ѣ=="init"){Ɂ="";ҳ=false;ұ=
0;Ո();return false;}string[]ѡ=ѣ.Trim().Split(';');bool Ѡ=false;for(int џ=0;џ<ѡ.Length;џ++){string[]ў=ѡ[џ].Trim().Split(
' ');if(ў[0]=="timer"){}else if(ў[0]=="wccs"){}else if(ў[0]=="wcct"){}else if(ў[0]=="W"||ў[0]=="O"){Ѡ=true;Echo("Args:");
for(int ћ=0;ћ<ў.Length;ћ++)Echo(ў[ћ]);if(ў.Length<1){Echo("Invalid Command:("+ѡ[џ]+")");continue;}string ѝ=ў[1].Trim();if(ў
.Length>2){ѝ=ў[1];for(int ќ=2;ќ<ў.Length;ќ++)ѝ+=" "+ў[ќ];ѝ=ѝ.Trim();}string[]Ǔ=ѝ.Split(',');if(Ǔ.Length<3){Ǔ=ѝ.Split(':')
;}for(int ћ=0;ћ<Ǔ.Length;ћ++)Echo(Ǔ[ћ]);if(Ǔ.Length<3){Echo("Invalid Command:("+ѡ[џ]+")");ʋ();return false;}int њ=0;
string Ѣ="Waypoint";if(Ǔ[0]=="GPS"){if(Ǔ.Length>4){Ѣ=Ǔ[1];њ=2;}else{Echo("Invalid Command");ʋ();return false;}}double ċ,ǋ,Ǌ;
bool ǉ=double.TryParse(Ǔ[њ++].Trim(),out ċ);bool ǈ=double.TryParse(Ǔ[њ++].Trim(),out ǋ);bool Ǉ=double.TryParse(Ǔ[њ++].Trim()
,out Ǌ);if(!ǉ||!ǈ||!Ǉ){Echo("Invalid Command:("+ѡ[џ]+")");continue;}if(ў[0]=="W"){ϙ(new Vector3D(ċ,ǋ,Ǌ),Ԟ,0,ճ,Ѣ,լ);}else{
ϙ(new Vector3D(ċ,ǋ,Ǌ),Ԟ,0,ճ,Ѣ,լ,false);}}else if(ў[0]=="S"){if(ў.Length<1){Echo("Invalid Command:("+ѡ[џ]+")");continue;}
double ċ;bool ǉ=double.TryParse(ў[1].Trim(),out ċ);if(ǉ){լ=ċ;}else{Echo("Invalid Command:("+ѡ[џ]+")");continue;}}else if(ў[0]
=="D"){if(ў.Length<1){Echo("Invalid Command:("+ѡ[џ]+")");continue;}double ċ;bool ǉ=double.TryParse(ў[1].Trim(),out ċ);if(ǉ
){ճ=ċ;}else{Echo("Invalid Command:("+ѡ[џ]+")");continue;}}else if(ў[0]=="C"){if(ў.Length<1){Echo("Invalid Command:("+ѡ[џ]
+")");continue;}else{Echo(ѡ[џ]);}}else if(ў[0]=="L"){Ѡ=true;η();}else if(ў[0]=="launch"){Ѡ=true;η();}else if(ў[0]=="OL"){
Ѡ=true;ζ();}else if(ў[0]=="orbitallaunch"){Ѡ=true;ζ();}else if(ў[0]=="dock"){Ѡ=true;ζ();}else{int ј;if(ӥ.TryGetValue(ў[0]
.ToLower(),out ј)){Ґ="mode set to "+ј;ԑ(ј);}else{Ґ="Unknown argument:"+ў[0];}}}if(Ѡ){έ();}return false;}bool љ(string ѣ){
return false;}bool ѩ(string ѣ){return false;}void Ѳ(){if(Ѭ==null)return;І(Ѭ);Ѭ.ǰ(ѯ,"Mode",Ն.ToString());Ѭ.ǰ(ѯ,"current_state",
ѫ.ToString());Ѭ.ǰ(ѯ,"PassedArgument",Ѩ);Ѭ.ǰ(ѯ,"AlertStates",ѧ.ToString());Ѭ.ǰ(ѯ,"craft_operation",Ѫ.ToString());Ѭ.ǰ(ѯ,
"ReceivedMessage",Ѧ);long ѱ=0;if(ѭ!=null)ѱ=ѭ.EntityId;Ѭ.ǰ(ѯ,"SaveID",(long)ѱ);if(Ѭ.ʛ){if(Ѭ.ʛ){string ʘ=Ѭ.ǔ();if(ѭ==null){Echo(
"WARNING: saving to Storage");Storage=ʘ;}else{ѭ.WriteText(ʘ,false);}}}else{Echo("Not saving: Same");}}string Ѱ="Wico Craft Save";string ѯ="WCCM2";
void Ѯ(ʈ À){À.ǵ(ѯ,"SAVE_FILE_NAME",ref Ѱ,true);}IMyTextPanel ѭ=null;ʈ Ѭ=null;int ѫ=0;long ѳ=0;int Ѫ=ˁ;string Ѩ="";int ѧ=0;
string Ѧ="";string ѥ(){string Ɂ="S";ѭ=null;List<IMyTerminalBlock>Ɖ=new List<IMyTerminalBlock>();Ɖ=ɉ<IMyTextPanel>(Ѱ);if(Ɖ.
Count>1){ү=true;Ұ+="\nMultiple blocks found:\""+Ѱ+"\"";}else if(Ɖ.Count==0){Ɖ=ɪ<IMyTextPanel>(Ѱ);if(Ɖ.Count==1)ѭ=Ɖ[0]as
IMyTextPanel;else{Ɖ=Ɋ<IMyTextPanel>(Ѱ);if(Ɖ.Count==1)ѭ=Ɖ[0]as IMyTextPanel;}}else ѭ=Ɖ[0]as IMyTextPanel;Ѭ=new ʈ(this,"");if(ѭ==null)
{Ɂ="-";}return Ɂ;}bool Ѥ(){return ѭ!=null;}string ǆ(Vector3D ǅ){string Ï;Ï=ǅ.X.ToString("0.00")+":"+ǅ.Y.ToString("0.00")+
":"+ǅ.Z.ToString("0.00");return Ï;}bool Ǎ(string ǌ,out double ċ,out double ǋ,out double Ǌ){string[]Ǔ=ǌ.Trim().Split(',');if
(Ǔ.Length<3){Ǔ=ǌ.Trim().Split(':');}ċ=0;ǋ=0;Ǌ=0;if(Ǔ.Length<3)return false;bool ǉ=double.TryParse(Ǔ[0].Trim(),out ċ);bool
ǈ=double.TryParse(Ǔ[1].Trim(),out ǋ);bool Ǉ=double.TryParse(Ǔ[2].Trim(),out Ǌ);if(!ǉ||!ǈ||!Ǉ){return false;}return true;}
Ӑ ӕ;void Ӕ(IMyTerminalBlock Ŝ){if(Ŝ==null){Ŝ=(IMyTerminalBlock)Me;}ӕ=new Ӑ(this,Ŝ);}const float ӓ=0.5f;const float Ӓ=2.5f
;const double Ӗ=0.5;const double ӑ=2.5;class Ӑ{private float ӏ,ӎ,Ӎ;private double ӌ,Ӌ,ӊ;private double Ӊ;private Program
ʣ;private Ѽ Ҁ;public Ӑ(Program ʚ,IMyTerminalBlock Ŝ){ʣ=ʚ;if(ʣ.Me.CubeGrid.GridSizeEnum.ToString().ToLower().Contains(
"small"))Ӊ=Ӗ;else Ӊ=ӑ;Ҁ=new Ѽ(Ŝ);Vector3D[]Ê=new Vector3D[4];Ҁ.Ғ(Ѽ.қ,Ê);Ӌ=(Ê[0]-Ê[1]).Length();ӊ=(Ê[0]-Ê[2]).Length();Ҁ.Ғ(0,Ê);
ӌ=(Ê[0]-Ê[2]).Length();ӏ=(float)(ӌ/Ӊ);ӎ=(float)(Ӌ/Ӊ);Ӎ=(float)(ӊ/Ӊ);}public float Ӭ(){return ӏ;}public double ӫ(){return
ӌ;}public float Ӫ(){return ӎ;}public double ө(){return Ӌ;}public float Ө(){return Ӎ;}public double ӧ(){return ӊ;}public
double Ӧ(){return Ӊ;}}Dictionary<string,int>ӥ=new Dictionary<string,int>();string Ҿ="";UpdateFrequency Ӥ=UpdateFrequency.Once;
bool ӣ=true;bool Ӣ=false;float ӡ=100;string Ӡ="WORLD";void ӟ(ʈ À){À.ǵ(Ӡ,"MaxWorldMps",ref ӡ,true);}string Ӟ="WICOCRAFT";void
ӝ(bool Ӝ=false){ʈ ӛ=new ʈ(this,Me.CustomData);ӛ.ǵ(Ӟ,"EchoOn",ref Ӛ,true);ӛ.ǵ(Ӟ,"DebugUpdate",ref Ӣ,true);ӟ(ӛ);Ĉ(ӛ);θ(ӛ);Ư
(ӛ);Չ(ӛ);if(ӛ.ʛ||Ӝ){Me.CustomData=ӛ.ǔ(true);}}bool Ӛ=true;Action<string>ә;void Ә(string ӗ){if(Ӛ)ә(ӗ);}Program(){Պ();ӝ();ә
=Echo;Echo=Ә;Ҿ=Ћ+":"+Њ+" V"+Љ+" ";ә(Ҿ+"Creator");if(!Me.CustomName.Contains(Њ))Me.CustomName="PB "+Ћ+" "+Њ;if(!Me.Enabled
){Echo("I am turned OFF!");}IMyTextSurface ҵ=Me.GetSurface(0);IMyTextSurface Ң=Me.GetSurface(1);ҵ.ContentType=VRage.Game.
GUI.TextPanel.ContentType.TEXT_AND_IMAGE;ҵ.WriteText("Wicorel\n"+Њ);ҵ.FontSize=2;ҵ.Alignment=VRage.Game.GUI.TextPanel.
TextAlignment.CENTER;Ң.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;Ң.WriteText("Version:"+Љ);Ң.Alignment=VRage.
Game.GUI.TextPanel.TextAlignment.CENTER;Ң.TextPadding=0.25f;Ң.FontSize=3.5f;}bool ҳ=false;bool Ҳ=false;string Ɂ="";int ұ=0;
string Ұ="";bool ү=false;bool Ү=false;bool ҭ=false;bool Ҭ=false;bool ҫ=false;double Ҵ=5;double Ҫ=-1;double Ҩ=-1;double ҧ=-2;
void Main(string ѣ,UpdateType Ҧ){Echo(Ҿ+ƴ());if(Ӣ)Echo(Ҧ.ToString());Ү=false;ҭ=false;Ҭ=false;if(Ҫ>Ҵ){Echo("Projector Check")
;Ҫ=0;ҫ=false;var Ʃ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyProjector>(Ʃ,ɣ);for(int r=0;r<Ʃ.
Count;r++){if(Ʃ[r].IsWorking){if(Ʃ[r].CustomName.Contains("!WCC")||Ʃ[r].CustomData.Contains("!WCC"))continue;Echo(
"Working local Projector found!");ҫ=true;}}}else{if(Ҫ<0){Ҫ=Ҵ+5;}Ҫ+=Runtime.TimeSinceLastRun.TotalSeconds;}if(ҫ)Echo("Working local Projector found!");if
(ѣ!=""&&ѣ!="timer"&&ѣ!="wccs")Echo("Arg="+ѣ);if(ѣ=="init"){Ɂ="";ҳ=false;}if(!ҳ){if(ҫ){Ա("clear",Է(υ));Ա(Њ+
":Construction in Progress\nTurn off projector to continue",φ);}Ү=true;Ո();if(ү)Ү=false;Ҳ=true;}else{if(Ҳ)Ա(DateTime.Now.ToString()+" "+Ћ+":"+Ɂ,ψ,true);if(Ұ!="")Echo(Ұ);ʮ();if(Ƀ
is IMyShipController){Ҩ=((IMyShipController)Ƀ).GetShipSpeed();Vector3D ҥ=((IMyShipController)Ƀ).GetNaturalGravity();double
Ҥ=ҥ.Length();ҧ=Ҥ/9.81;}if((Ҧ&(UpdateType.Trigger|UpdateType.Terminal))>0||(Ҧ&(UpdateType.Mod))>0||(Ҧ&(UpdateType.Script))
>0){if(ѣ.ToLower()=="profilerreset"){Ӂ();Ү=true;}if(ҏ(ѣ)){ң();Ѳ();Ը();return;}}else if((Ҧ&(UpdateType.IGC))>0){if(!љ(ѣ)){
ę(ѣ);}ң();Ѳ();Ը();return;}else if((Ҧ&(UpdateType.IGC))>0){if(!ѩ(ѣ)){}ң();Ѳ();Ը();}{ѣ="";if(ү&&!Ҳ){ҳ=false;Ɂ="";}}Ě();ħ();
Ѕ();ˠ();}ң();П();Ѳ();Ҳ=false;Ը();}void ң(){UpdateFrequency ҩ=UpdateFrequency.None;if(Ү){Echo("FAST!");ҩ|=Ӥ;}else{}if(ҭ){
Echo("MEDIUM");ҩ|=UpdateFrequency.Update10;}else{}if(Ҭ){Echo("SLOW");ҩ|=UpdateFrequency.Update100;}else{}Runtime.
UpdateFrequency=ҩ;}void Ҷ(string Ҿ=null){float ӈ=0;ӈ=Runtime.CurrentInstructionCount/(float)Runtime.MaxInstructionCount;if(Ҿ==null)Ҿ=
"Instructions=";Echo(Ҿ+" "+(ӈ*100).ToString("0.00")+"%");}int Ӈ=1;int ӆ=20;bool Ӆ=false;StringBuilder ӄ=new StringBuilder();void Ӄ(){if
(Ӈ<=ӆ){double ӂ=Runtime.LastRunTimeMs;Echo("Profiler("+Ӈ+"):Add:"+ӂ.ToString());ӄ.Append(ӂ.ToString()).Append("\n");Ӈ++;}
else if(!Ӆ){Echo("Profiler:DISPLAY");var Ӏ=GridTerminalSystem.GetBlockWithName("DEBUG")as IMyTextPanel;Ӏ?.WriteText(ӄ.
ToString());Ӆ=true;}}void Ӂ(){Ӈ=1;ӄ=new StringBuilder();var Ӏ=GridTerminalSystem.GetBlockWithName("DEBUG")as IMyTextPanel;Ӏ?.
WriteText("");Ӆ=false;}List<IMyTerminalBlock>ҿ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ś=new List<IMyTerminalBlock>();
List<IMyTerminalBlock>Ś=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ř=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>Ř=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ŗ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ŗ=new List<
IMyTerminalBlock>();double ҽ=0;double Ҽ=0;double һ=0;double Һ=0;double ҹ=0;double Ҹ=0;int ҷ=0;int ʪ=0;int ũ=0;int ɺ=0;const int Ũ=1;
const int ŧ=2;const int Ŧ=4;const int ť=8;const int Ť=0xff;Matrix ţ=new Matrix(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);string Ţ=
"IGNORE";string š="cutter";string Š="THRUSTERS";void ş(ʈ À){À.ǵ(Š,"IgnoreThruster",ref Ţ);À.ǵ(Š,"CutterThruster",ref š);}void Ş(
IMyTerminalBlock Ŝ,ref List<IMyTerminalBlock>ś,ref List<IMyTerminalBlock>Ś,ref List<IMyTerminalBlock>ř,ref List<IMyTerminalBlock>Ř,ref
List<IMyTerminalBlock>ŗ,ref List<IMyTerminalBlock>Ŗ,int ŕ=Ť){ś.Clear();Ś.Clear();ř.Clear();Ř.Clear();ŗ.Clear();Ŗ.Clear();ҿ.
Clear();if(Ŝ==null)return;var Ŕ=new List<IMyTerminalBlock>();ɫ<IMyThrust>(ref Ŕ);for(int r=0;r<Ŕ.Count;r++){if(Ŕ[r].
CustomName.ToLower().Contains(š)||Ŕ[r].CustomData.ToLower().Contains(š))continue;if(Ŕ[r].CustomName.ToLower().Contains(Ţ)||Ŕ[r].
CustomData.ToLower().Contains(Ţ))continue;ҿ.Add(Ŕ[r]);}Matrix ŝ;Ŝ.Orientation.GetMatrix(out ŝ);Matrix.Transpose(ref ŝ,out ŝ);ҽ=0;Ҽ
=0;һ=0;Һ=0;ҹ=0;Ҹ=0;for(int r=0;r<ҿ.Count;++r){var ł=ҿ[r]as IMyThrust;Matrix Ŷ;ł.Orientation.GetMatrix(out Ŷ);Vector3 ŵ=
Vector3.Transform(Ŷ.Backward,ŝ);int Ŵ=ų(ҿ[r]);if(Ŵ==Ũ)ũ++;else if(Ŵ==ŧ)ʪ++;else if(Ŵ==Ŧ)ҷ++;else if(Ŵ==ť)ɺ++;if(ŵ==ţ.Left){ҹ+=q
((IMyThrust)ҿ[r]);ŗ.Add(ҿ[r]);}else if(ŵ==ţ.Right){Ҹ+=q((IMyThrust)ҿ[r]);Ŗ.Add(ҿ[r]);}else if(ŵ==ţ.Backward){Ҽ+=q((
IMyThrust)ҿ[r]);Ś.Add(ҿ[r]);}else if(ŵ==ţ.Forward){ҽ+=q((IMyThrust)ҿ[r]);ś.Add(ҿ[r]);}else if(ŵ==ţ.Up){Һ+=q((IMyThrust)ҿ[r]);Ř.
Add(ҿ[r]);}else if(ŵ==ţ.Down){һ+=q((IMyThrust)ҿ[r]);ř.Add(ҿ[r]);}}}string Ş(IMyTerminalBlock Ŝ){ś.Clear();Ś.Clear();ř.Clear
();Ř.Clear();ŗ.Clear();Ŗ.Clear();ҿ.Clear();if(Ŝ==null)return"No Orientation Block";Ş(Ŝ,ref ś,ref Ś,ref ř,ref Ř,ref ŗ,ref
Ŗ);string Ï;Ï=">";Ï+="F"+ś.Count.ToString("00");Ï+="B"+Ś.Count.ToString("00");Ï+="D"+ř.Count.ToString("00");Ï+="U"+Ř.
Count.ToString("00");Ï+="L"+ŗ.Count.ToString("00");Ï+="R"+Ŗ.Count.ToString("00");Ï+="<";return Ï;}int ų(IMyTerminalBlock Ų){
if(Ų is IMyThrust){if(Ų.BlockDefinition.SubtypeId.Contains("AtmosphericHover"))return ť;else if(Ų.BlockDefinition.
SubtypeId.Contains("Atmo"))return Ũ;else if(Ų.BlockDefinition.SubtypeId.Contains("Hydro"))return ŧ;else if(Ų.BlockDefinition.
SubtypeId.Contains("SmallBlock_HoverEngine"))return ť;else return Ŧ;}return 0;}double q(IMyThrust ł){return ł.MaxEffectiveThrust;
}double ű(List<IMyTerminalBlock>Ļ,int Ŀ=Ť){double Ű=0;for(int Ľ=0;Ľ<Ļ.Count;Ľ++){int ļ=ų(Ļ[Ľ]);if((ļ&Ŀ)>0){IMyThrust ł=Ļ[
Ľ]as IMyThrust;double ů=ł.MaxEffectiveThrust;Ű+=ů;}}return Ű;}double Ů(List<IMyTerminalBlock>Ļ,float ŭ=5f,float Ŭ=2f,
float ū=1f){double Ū=0;foreach(var ĉ in Ļ){var ł=ĉ as IMyThrust;if(ł==null)continue;if(ų(ł)==Ũ)Ū+=ł.MaxEffectiveThrust*ŭ;else
if(ų(ł)==Ŧ)Ū+=ł.MaxEffectiveThrust*Ŭ;else if(ų(ł)==ŧ)Ū+=ł.MaxEffectiveThrust*ū;else Ū+=ł.MaxEffectiveThrust;}return Ū;}
bool Ŏ(List<IMyTerminalBlock>Ļ,out float Ō,out float ŋ,out float Ŋ){Ō=0;ŋ=0;Ŋ=0;double ŉ=ű(Ļ,Ŧ);double ň=ű(Ļ,Ũ);double Ň=ű(Ļ
,ŧ);MyShipMass î;î=((IMyShipController)Ƀ).CalculateShipMass();double ņ=0;ņ=î.PhysicalMass*ҧ*9.810;if(ň>0){if(ň<ņ){Ō=100;ņ
-=ň;}else{Ō=(float)(ņ/ň*100);if(Ō>0)ņ-=(ň*Ō/100);}}if(ŉ>0&&ņ>0){if(ŉ<ņ){Ŋ=100;ņ-=ŉ;}else{Ŋ=(float)(ņ/ŉ*100);if(Ŋ>0)ņ-=((ŉ*
Ŋ)/100);}}if(Ň>0&&ņ>0){if(Ň<ņ){ŋ=100;ņ-=Ň;}else{ŋ=(float)(ņ/Ň*100);if(ŋ>0)ņ-=((Ň*ŋ)/100);;}}if(ņ>0)return false;return
true;}List<IMyTerminalBlock>Ņ(string ń){var ō=new List<IMyTerminalBlock>();var Ń=new List<IMyBlockGroup>();
GridTerminalSystem.GetBlockGroups(Ń);for(int Ł=0;Ł<Ń.Count;Ł++){if(Ń[Ł].Name==ń){List<IMyTerminalBlock>Ļ=null;Ń[Ł].GetBlocks(Ļ,ɣ);for(int
Ľ=0;Ľ<Ļ.Count;Ľ++){ō.Add(Ļ[Ľ]);}break;}}return ō;}int ŀ(List<IMyTerminalBlock>Ļ,float B,int Ŀ=Ť){int ľ=0;if(B>100)B=100;
if(B<0)B=0;for(int Ľ=0;Ľ<Ļ.Count;Ľ++){int ļ=ų(Ļ[Ľ]);if((ļ&Ŀ)>0){IMyThrust ł=Ļ[Ľ]as IMyThrust;if(!ł.IsWorking){if(!ł.
Enabled)ł.Enabled=true;}ľ+=1;ł.ThrustOverridePercentage=B/100f;}}return ľ;}int ŀ(List<IMyTerminalBlock>Ļ,int œ=100,int Ŀ=Ť){
return ŀ(Ļ,(float)œ,Ŀ);}bool ŀ(string Ő,int œ=100,int Ŀ=Ť){if(œ>100)œ=100;var Ń=new List<IMyBlockGroup>();GridTerminalSystem.
GetBlockGroups(Ń);for(int Ł=0;Ł<Ń.Count;Ł++){if(Ń[Ł].Name==Ő){List<IMyTerminalBlock>Ļ=null;Ń[Ł].GetBlocks(Ļ,ɣ);return(ŀ(Ļ,œ,Ŀ)>0);}}
return false;}int ő(List<IMyTerminalBlock>Ļ,int Ŀ=Ť,bool Œ=false){int ľ=0;for(int Ľ=0;Ľ<Ļ.Count;Ľ++){int ļ=ų(Ļ[Ľ]);if((ļ&Ŀ)>0)
{ľ++;IMyThrust ł=Ļ[Ľ]as IMyThrust;ł.ThrustOverride=0;if(ł.IsWorking&&Œ&&ł.Enabled==true)ł.Enabled=false;else if(!ł.
IsWorking&&!Œ&&ł.Enabled==false)ł.Enabled=true;}}return ľ;}bool ő(string Ő){var Ń=new List<IMyBlockGroup>();GridTerminalSystem.
GetBlockGroups(Ń);for(int Ł=0;Ł<Ń.Count;Ł++){if(Ń[Ł].Name==Ő){List<IMyTerminalBlock>Ļ=null;Ń[Ł].GetBlocks(Ļ,ɣ);return(ő(Ļ)>0);}}return
false;}bool ŀ(){return(ŀ(ś)>0);}bool ő(){return(ő(ś)>0);}double ŷ(List<IMyTerminalBlock>Ƭ,int Ŀ=Ť){for(int r=0;r<Ƭ.Count;r++)
{int ļ=ų(Ƭ[r]);if((ļ&Ŀ)>0&&Ƭ[r].IsWorking){var ł=Ƭ[r]as IMyThrust;return ł.ThrustOverride;}}return 0;}bool Ʈ(List<
IMyTerminalBlock>Ƭ,int Ŀ=Ť){for(int r=0;r<Ƭ.Count;r++){int ļ=ų(Ƭ[r]);if((ļ&Ŀ)>0&&Ƭ[r].IsWorking){return true;}}return false;}int ƭ(List<
IMyTerminalBlock>Ƭ,int Ŀ=Ť){int ľ=0;for(int r=0;r<Ƭ.Count;r++){int ļ=ų(Ƭ[r]);if((ļ&Ŀ)>0&&Ƭ[r].IsWorking){ľ++;}}return ľ;}IMyThrust ƪ(
List<IMyTerminalBlock>Ʃ,int ƨ=Ť){foreach(var Ű in ҿ){if(Ű is IMyThrust&&(ų(Ű)&ƨ)>0)return Ű as IMyThrust;}return null;}
double Ƨ(){if(ũ<1)return 0;var Ʀ=ƪ(ҿ,Ũ);if(Ʀ==null)return 0;return Ʀ.MaxEffectiveThrust/Ʀ.MaxThrust;}double ƥ(List<
IMyTerminalBlock>Ƥ,double ƫ,double ƣ){var î=((IMyShipController)Ƀ).CalculateShipMass();double ņ=î.PhysicalMass*ƣ*9.810;double q=ű(Ƥ);
double í=(q-ņ)/î.PhysicalMass;double ë=ƫ/í;double ê=ƫ/2*ë;return ê;}int ƿ=0;void ƾ(float ƽ,float Ƽ,List<IMyTerminalBlock>ƻ,
List<IMyTerminalBlock>ƺ){if(ƿ<0)ƿ=0;double q=ű(ƻ);MyShipMass î;î=((IMyShipController)Ƀ).CalculateShipMass();double ƹ=î.
PhysicalMass;float Ƹ=100f;if(ƹ>0){double í=(q)/ƹ;if(í>0)Ƹ=(float)(ƽ/í);}if(Ҩ>Ƽ){ő(ҿ);}else if(Ҩ<(ƽ*0.90)){if(Ҩ<0.09)ƿ++;if(Ҩ<ƽ*0.25)
ƿ++;ŀ(ƻ,Ƹ+ƿ/5);}else if(Ҩ<(ƽ*1.1)){ƿ--;ő(ƺ,Ť,true);ő(ƻ);}else{ƿ--;ƿ--;ŀ(ƻ,1f);}}void Ʒ(){ƿ=0;}string[]ƶ={"-","\\","|","/"
,"-","\\","|","/"};int Ƶ=99;string ƴ(){Ƶ++;if(Ƶ>=ƶ.Length)Ƶ=0;return ƶ[Ƶ];}string Ƴ="[WCCT]";string Ʋ="[WCCS]";string Ʊ=
"[WCCM]";string ư="WICOTIMERS";void Ư(ʈ À){À.ǵ(ư,"FastTimer",ref Ƴ,true);À.ǵ(ư,"SubModuleTimer",ref Ʋ,true);À.ǵ(ư,"MainTimer",
ref Ʊ,true);}Dictionary<string,List<IMyTerminalBlock>>Ə=new Dictionary<string,List<IMyTerminalBlock>>();void Ÿ(){Ə.Clear();
}void ƍ(){if(!ƌ(Ƴ))ƌ(Ʊ);}bool ƌ(string Ƌ="[WCCS]"){bool Ɗ=false;List<IMyTerminalBlock>Ɖ=new List<IMyTerminalBlock>();
IMyTimerBlock ƈ=null;if(Ə.ContainsKey(Ƌ)){Ɖ=Ə[Ƌ];}else{Ɖ=ɪ<IMyTerminalBlock>(Ƌ);Ə.Add(Ƌ,Ɖ);}for(int r=0;r<Ɖ.Count;r++){ƈ=Ɖ[r]as
IMyTimerBlock;if(ƈ!=null){if(ƈ.Enabled){ƈ.Trigger();Ɗ=true;}else{Echo("Timer:"+ƈ.CustomName+" is OFF");}}}return Ɗ;}bool Ƈ=false;
double Ɔ=-1;double ƅ=0.25;double Ǝ=-1;IMyShipController Ƅ=null;double Ƃ=85;List<IMyTerminalBlock>Ɓ=new List<IMyTerminalBlock>(
);List<IMyTerminalBlock>ƀ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ſ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ž=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ž=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ż=new List<
IMyTerminalBlock>();IMySensorBlock Ż=null;bool ź=false;bool Ź=false;bool ƃ=false;double Ɛ=100;double ƕ=50;double Ƣ=15;double Ơ=100;
double Ɵ=100*0.5;double ƞ=100*0.25;double Ɲ=5;float Ɯ=50f;int ƛ=0;bool ƚ=false;bool ƙ=false;bool Ƙ=false;bool Ɨ=false;void ơ()
{Ƅ=null;ȫ();ɛ=0.01f;Ǝ=0;Ɔ=-1;ȡ(0,50);}void Ɩ(Vector3D æ,double d,IMyTerminalBlock Ɣ,int Ŵ=Ť){Ƃ=d;if(Ƃ>ӡ)Ƃ=ӡ;if((Ѫ&ʫ)>0){ƙ
=true;Ұ+="\nI am a SLED!";ȣ();}else ƙ=false;if((Ѫ&ʾ)>0){Ƙ=true;if(Ƀ is IMyShipController)((IMyShipController)Ƀ).HandBrake
=false;}else Ƙ=false;if((Ѫ&ʽ)>0){Ɨ=true;}else Ɨ=false;if((Ѫ&ʿ)>0){ƚ=true;}else ƚ=false;Ƅ=Ɣ as IMyShipController;Vector3D
G=æ-Ƅ.CenterOfMass;double D=G.Length();Ş(Ƅ,ref ƀ,ref Ɓ,ref ż,ref Ž,ref ſ,ref ž,Ŵ);ȫ();if(Ǩ.Count>0){Ż=Ǩ[0];if(ƚ||ƙ)Ż.
DetectAsteroids=false;else Ż.DetectAsteroids=true;Ż.DetectEnemy=true;Ż.DetectLargeShips=true;Ż.DetectSmallShips=true;Ż.DetectStations=
true;Ż.DetectPlayers=false;Ɯ=Ż.GetMaximum<float>("Front");if(Ь.Count<1){if(!ג)Ƃ=Ɯ/2;if(ו)Ұ+=
"\nNo Cameras for collision detection";}}else{Ż=null;Ɯ=0;if(Ь.Count<1){if(ו||ה)Ұ+="\nNo Sensor nor cameras\n for collision detection";if(!ג)Ƃ=5;}else{if(ה)Ұ+=
"\nNo Sensor for collision detection";}}ź=false;Ź=false;ƃ=false;double ì=Ƃ;if(!ƙ&&!ƚ)ì=Ú(Ɓ,D,Ƃ);if(ì<Ƃ)Ƃ=ì;if(ז)Ұ+="\nDistance="+Ս(D)+" OptimalV="+Ս(ì);Ơ=Ƃ;Ɵ
=Ƃ*0.50;ƞ=Ƃ*0.25;if(Ɵ<5)Ɵ=5;if(ƞ<5)ƞ=5;if(ƞ>Ɵ)Ɵ=ƞ;if(ƞ>Ơ)Ơ=ƞ;if(!(Ƙ||ƚ)){Ƣ=ƥ(Ɓ,ƞ+(Ɵ-ƞ)/2,0);ƕ=ƥ(Ɓ,Ɵ+(Ơ-Ɵ)/2,0);Ɛ=ƥ(Ɓ,Ơ,0)
;}if(ז)Ұ+="\nFarSpeed=="+Ս(Ơ)+" ASpeed="+Ս(Ɵ);if(ז)Ұ+="\nFar=="+Ս(Ɛ)+" A="+Ս(ƕ)+" P="+Ս(Ƣ);Ƈ=false;Ɔ=-1;Ǝ=0;ƛ=0;ɛ=0.01f;}
void Ɠ(Vector3D æ,float ƒ,int Ƒ,int ŏ,bool ĺ=false){bool e=false;if(ז){Echo("dTM:"+ѫ+"->"+Ƒ+"-C>"+ŏ+" A:"+ƒ);Echo("W="+Ƙ.
ToString()+" S="+ƙ.ToString()+" R="+ƚ.ToString());}if(Ƅ==null){Ɩ(æ,լ,Ƀ);}if(Ɔ>=0)Ɔ+=Runtime.TimeSinceLastRun.TotalMilliseconds;
if(Ǝ>=0)Ǝ+=Runtime.TimeSinceLastRun.TotalMilliseconds;Vector3D G=æ-Ƅ.CenterOfMass;double D=G.Length();if(ב>0&&ҧ>0){if(D<(ƒ
+ב))e=true;}if(ז){Echo("dTM:distance="+Ս(D)+" ("+ƒ.ToString()+")");Echo("dTM:velocity="+Ҩ.ToString("0.00"));Echo(
"dTM:tmMaxSpeed="+Ƃ.ToString("0.00"));}if(D<ƒ)e=true;if(e){О();ѫ=Ƒ;ơ();Ү=true;return;}Ծ("TargetLocation",æ);List<IMySensorBlock>Ö=null;
double Õ=0;if(!(Ƙ||ƚ)){Õ=ƥ(Ɓ,Ҩ,0);}if(Ǩ.Count>0){float Ô=Math.Min(Ɯ,(float)Õ*1.5f);if(!ו)ȹ(Ż,1,1,1,1,Ô,0);else ȹ(Ż,0,0,0,0,Ô,0
);}bool Ó=false;Vector3D Ò=(Ƀ as IMyShipController).GetNaturalGravity();if(ƙ||ƚ){double Ñ=-999;Ñ=п(æ,Ƀ);Echo("yawangle="+
Ñ.ToString());if(ƙ){Echo("Sled");ъ(Ñ,"Yaw");}else if(ƚ){Echo("Rotor");ҁ(Ñ);}Ó=Math.Abs(Ñ)<.05;}else if(Ƙ&&Ɨ){Echo(
"Wheels W/ Gyro");double Ñ=-999;Ñ=п(æ,Ƀ);Echo("yawangle="+Ñ.ToString());Ó=Math.Abs(Ñ)<.05;if(!Ó){ȡ(0,5);ъ(Ñ,"Yaw");}else Ț(50);}else if(
Ƙ){Echo("Wheels with no gyro...");double Ñ=п(æ,Ƀ);Echo("yawangle="+Ñ.ToString());Ó=Math.Abs(Ñ)<.05;if(!Ó){}else Ț(50);}
else{if(Ò.Length()>0){Echo("DTM: In Gravity using:"+Ƀ.CustomName);bool Ø=ɚ("",Ò,Ƀ);{double Ñ=п(æ,Ƀ);ъ(Ñ,"Yaw");Ó=Math.Abs(Ñ)
<.05;}}else{Ó=ɚ("forward",G,Ƀ);}}Ƅ.DampenersOverride=true;if((D-Õ)<ƒ){ɛ=0.005f;Ա("\"Arriving at target.  Slowing",φ);Echo
("Waiting for stop");if(!Ó)Ү=true;О();return;}if(Ó){ҭ=true;Echo("Aimed");ʋ();if(Ƙ)Ț(50);if(ה&&(Ǝ>Ǭ||Ǝ<0)){Ǝ=0;Ö=Ȱ();if(Ö.
Count>0){var Ð=new List<MyDetectedEntityInfo>();string Ï="";for(int u=0;u<Ö.Count;u++){Ö[u].DetectedEntities(Ð);int Î=0;bool
Í=false;if(Ð.Count>0)Í=true;for(;Î<Ð.Count;Î++){Ï="\nSensor TRIGGER!";Ï+="\nName: "+Ð[Î].Name;Ï+="\nType: "+Ð[Î].Type;Ï+=
"\nRelationship: "+Ð[Î].Relationship;Ï+="\n";if(ז){Echo(Ï);Ա(Ï,ψ);}if(Ð[Î].Type==MyDetectedEntityType.Planet){Í=false;}if(Ð[Î].Type==
MyDetectedEntityType.LargeGrid||Ð[Î].Type==MyDetectedEntityType.SmallGrid){if(Ð[Î].BoundingBox.Contains(æ)!=ContainmentType.Disjoint){if(ז)
Echo("Ignoring collision because we want to be INSIDE");Í=false;}}if(Í)break;}if(Í){Μ=Ð[Î];ơ();ѫ=ŏ;Ƈ=true;Ү=true;О();return;
}}}else Μ=new MyDetectedEntityInfo();}double Ì=Õ*2;{if(Ì<100){if(D<500)Ì=D;else Ì=500;}Ì=Math.Min(D,Ì);}if(ו){}if(ו&&(Ɔ>ƅ
||Ɔ<0)&&D>Ɯ){Ѽ Ë=new Ѽ(Ƀ);Vector3D[]Ê=new Vector3D[4];Ë.Ғ(Ѽ.қ,Ê);bool É=false;Vector3D È;switch(ƛ){case 0:if(У(Ь,Ì)){É=
true;}break;case 1:È=Ê[2]+Ƀ.WorldMatrix.Forward*D;if(У(Ь,È)){É=true;}break;case 2:È=Ê[3]+Ƀ.WorldMatrix.Forward*D;if(У(Ь,È)){
É=true;}break;case 3:È=Ê[0]+Ƀ.WorldMatrix.Forward*D;if(У(Ь,È)){É=true;}break;case 4:È=Ê[1]+Ƀ.WorldMatrix.Forward*D;if(У(Ь
,È)){É=true;}break;case 5:if(У(Ь,Ì)){É=true;}break;case 6:È=Ê[2]+Ƀ.WorldMatrix.Forward*D/2;if(У(Ь,È)){É=true;}break;case
7:È=Ê[3]+Ƀ.WorldMatrix.Forward*D/2;if(У(Ь,È)){É=true;}break;case 8:È=Ê[0]+Ƀ.WorldMatrix.Forward*D/2;if(У(Ь,È)){É=true;}
break;case 9:È=Ê[1]+Ƀ.WorldMatrix.Forward*D/2;if(У(Ь,È)){É=true;}break;}if(É){ƛ++;if(ƛ>9)ƛ=0;Ɔ=0;if(!Μ.IsEmpty()){bool Í=true
;{if(Μ.Type==MyDetectedEntityType.Asteroid){if(Μ.BoundingBox.Contains(æ)!=ContainmentType.Disjoint){Í=false;double Ç=((
Vector3D)Μ.HitPosition-Ƀ.GetPosition()).Length();if((Ç-Õ)<ƒ){О();ѫ=Ƒ;ơ();return;}}}else if(Μ.Type==MyDetectedEntityType.Planet){
Í=false;}else{}}if(ז){Echo("raycast hit:"+Μ.Type.ToString());Ա("Camera Trigger collision",φ);}if(Í){ơ();ѫ=ŏ;Ƈ=false;Ү=
true;О();return;}}else{if(ז){Ա("Camera Scan Clear",φ);}}}else{if(ז){Ա("No Scan Available",φ);}}}else Echo("Raycast delay");
if(ז)Echo("dtmFar="+Ս(Ɛ));if(ז)Echo("dtmApproach="+Ս(ƕ));if(ז)Echo("dtmPrecision="+Ս(Ƣ));if(D>Ɛ&&!ź){Echo(
"dtmFar. Target Vel="+Ơ.ToString("N0"));Ա("\"Far\" from target\n Target Speed="+Ơ.ToString("N0")+"m/s",φ);L(Ơ,100f);}else if(D>ƕ&&!Ź){Echo(
"Approach. Target Vel="+Ɵ.ToString("N0"));Ա("\"Approach\" distance from target\n Target Speed="+Ɵ.ToString("N0")+"m/s",φ);ź=true;L(Ɵ,100f);}
else if(D>Ƣ&&!ƃ){Echo("Precision. Target Vel="+ƞ.ToString("N0"));Ա("\"Precision\" distance from target\n Target Speed="+ƞ.
ToString("N0")+"m/s",φ);if(!Ź)ɛ=0.005f;Ź=true;L(ƞ,100f);}else{Echo("Close. Target Speed="+Ɲ.ToString("N0")+"m/s");Ա(
"\"Close\" distance from target\n Target Speed="+Ɲ.ToString("N0")+"m/s",φ);if(!ƃ)ɛ=0.005f;ƃ=true;L(Ɲ,100f);}}else{Ա("Aiming at target",φ);if(ז)Echo("Aiming");Ү=true;Ƅ.
DampenersOverride=true;if(Ҩ<5){ő(ҿ);}else{ő(Ɓ,Ť,true);}}}double Ú(List<IMyTerminalBlock>ð,double D,double d){if(ð.Count<1)return ӡ;
MyShipMass î;î=((IMyShipController)Ƀ).CalculateShipMass();double q=ű(ð);double í=q/î.PhysicalMass;double ì,ë,ê;ì=d;double é=D/2;if
(ז)Ұ+="COS OptimalV="+Ս(ì);do{ë=ì/í;ê=ì/2*ë;if(ê>é){ì*=0.85;}if(ז)Ұ+="stoppingM="+Ս(ê)+" distance="+Ս(D);}while(ê>é);
return ì;}Vector3D è;void ç(Vector3D æ){if(Ɔ>=0)Ɔ+=Runtime.TimeSinceLastRun.TotalMilliseconds;if(Ǝ>=0)Ǝ+=Runtime.
TimeSinceLastRun.TotalMilliseconds;Vector3D å;if(Μ.HitPosition.HasValue){å=(Vector3D)Μ.HitPosition;}else{å=Ƀ.GetPosition();}Vector3D ä=Μ
.Position;Vector3D G=(ä-å);G.Normalize();Vector3D ã=Μ.BoundingBox.Min;Vector3D â=Μ.BoundingBox.Max;double á=(ä-ã).Length(
);double à=á+ӕ.ө()*5;Vector3D ß;ß=Vector3D.Cross(æ,å);ß.Normalize();ß=å+ß*à;è=ß;}bool Þ=true;bool Ý=true;bool Ü=true;bool
Û=true;bool Æ=true;bool Z=true;MyDetectedEntityInfo A=new MyDetectedEntityInfo();MyDetectedEntityInfo X=new
MyDetectedEntityInfo();MyDetectedEntityInfo W=new MyDetectedEntityInfo();MyDetectedEntityInfo V=new MyDetectedEntityInfo();
MyDetectedEntityInfo U=new MyDetectedEntityInfo();MyDetectedEntityInfo S=new MyDetectedEntityInfo();ΐ R;ΐ Q;ΐ P;ΐ O;ΐ N;ΐ Y;void M(bool K=
false,bool J=true){if(Ɔ>=0)Ɔ+=Runtime.TimeSinceLastRun.TotalMilliseconds;if(Ǝ>=0)Ǝ+=Runtime.TimeSinceLastRun.
TotalMilliseconds;Þ=true;Ý=true;Ü=true;Û=true;Æ=K;Z=J;A=new MyDetectedEntityInfo();X=new MyDetectedEntityInfo();W=new
MyDetectedEntityInfo();V=new MyDetectedEntityInfo();U=new MyDetectedEntityInfo();S=new MyDetectedEntityInfo();if(Μ.Type==
MyDetectedEntityType.LargeGrid||Μ.Type==MyDetectedEntityType.SmallGrid){}if(б.Count<1)Þ=false;if(Ш.Count<1)Ý=false;if(Щ.Count<1)Ü=false;if(Ъ
.Count<1)Û=false;if(Ь.Count<1)Z=false;if(Ы.Count<1)Æ=false;R=new ΐ(this,Ь,200,45,45,2,1,5,200,true);Q=new ΐ(this,Ы,200,45
,45,2,1,5,200,true);P=new ΐ(this,б,200,45,45,2,1,5,200,true);O=new ΐ(this,Ш,200,45,45,2,1,5,200,true);N=new ΐ(this,Щ,200,
45,45,2,1,5,200,true);Y=new ΐ(this,Ъ,200,45,45,2,1,5,200,true);}bool I(){if(Ɔ>=0)Ɔ+=Runtime.TimeSinceLastRun.
TotalMilliseconds;if(Ǝ>=0)Ǝ+=Runtime.TimeSinceLastRun.TotalMilliseconds;MatrixD H=Ƀ.WorldMatrix;Vector3D G=H.Forward;Echo("ScanEscape()")
;if(Þ){if(У(б,200)){Þ=false;A=Μ;if(Μ.IsEmpty()){G=H.Left;G.Normalize();è=Ƀ.GetPosition()+G*200;return true;}}Þ=P.Κ();if(P
.Ό){A=Μ;è=Ƀ.GetPosition()+P.Ε*200;return true;}}if(Ý){if(У(Ш,200)){Ý=false;X=Μ;if(Μ.IsEmpty()){G=H.Right;G.Normalize();è=
Ƀ.GetPosition()+G*200;return true;}}Ý=O.Κ();if(O.Ό){X=Μ;è=Ƀ.GetPosition()+O.Ε*200;return true;}}if(Ü){if(У(Щ,200)){Ü=
false;if(Μ.IsEmpty()){Ұ+="\n Straight Camera HIT!";G=H.Up;G.Normalize();è=Ƀ.GetPosition()+G*200;return true;}}Ü=N.Κ();if(N.Ό)
{W=Μ;è=Ƀ.GetPosition()+N.Ε*200;return true;}}if(Û){if(У(Ъ,200)){V=Μ;Û=false;if(Μ.IsEmpty()){G=H.Down;G.Normalize();è=Ƀ.
GetPosition()+G*200;return true;}}Û=Y.Κ();if(Y.Ό){V=Μ;è=Ƀ.GetPosition()+Y.Ε*200;return true;}}if(Æ){if(У(Ы,200)){U=Μ;Æ=false;if(Μ.
IsEmpty()){G=H.Backward;G.Normalize();è=Ƀ.GetPosition()+G*200;return true;}}Æ=Q.Κ();if(Q.Ό){U=Μ;è=Ƀ.GetPosition()+Q.Ε*200;
return true;}}if(Z){if(У(Ь,200)){Z=false;S=Μ;if(Μ.IsEmpty()){G=H.Forward;G.Normalize();è=Ƀ.GetPosition()+G*200;return true;}}Z
=R.Κ();if(R.Ό){S=Μ;è=R.Ε*200;è=Ƀ.GetPosition()+R.Ε*200;return true;}}if(Z||Æ||Ü||Û||Þ||Ý){Echo("More scans");return false
;}Echo("Scans done. Choose longest");MyDetectedEntityInfo F=U;Vector3D E=Ƀ.GetPosition();G=H.Backward;if(F.HitPosition==
null||A.HitPosition!=null&&Vector3D.DistanceSquared(E,(Vector3D)F.HitPosition)<Vector3D.DistanceSquared(E,(Vector3D)A.
HitPosition)){G=H.Left;F=A;}if(F.HitPosition==null||X.HitPosition!=null&&Vector3D.DistanceSquared(E,(Vector3D)F.HitPosition)<
Vector3D.DistanceSquared(E,(Vector3D)X.HitPosition)){G=H.Right;F=X;}if(F.HitPosition==null||W.HitPosition!=null&&Vector3D.
DistanceSquared(E,(Vector3D)F.HitPosition)<Vector3D.DistanceSquared(E,(Vector3D)W.HitPosition)){G=H.Up;F=W;}if(F.HitPosition==null||V.
HitPosition!=null&&Vector3D.DistanceSquared(E,(Vector3D)F.HitPosition)<Vector3D.DistanceSquared(E,(Vector3D)V.HitPosition)){G=H.
Down;F=V;}if(F.HitPosition==null||S.HitPosition!=null&&Vector3D.DistanceSquared(E,(Vector3D)F.HitPosition)<Vector3D.
DistanceSquared(E,(Vector3D)S.HitPosition)){G=H.Forward;F=S;}if(F.HitPosition==null)return false;double D=Vector3D.Distance(E,(Vector3D
)F.HitPosition);Echo("Distance="+Ս(D));G.Normalize();è=Ƀ.GetPosition()+G*D/2;if(D>4){return true;}Echo(
"not FAR enough: ERROR!");return false;}void C(float B){if(ƚ){դ(B);}else ŀ(ƀ,B);}void L(double d,float q){if(Ƙ){if(Ҩ<1){ȡ(q);}else if(Ҩ<d*.75||(
!ź&&Ҩ<d*.98)){float Ä=(float)d/ӡ*q;ȡ(q);}else if(Ҩ<d*.85)ȡ(q);else if(Ҩ<=d*.98){ȡ(q);}else if(Ҩ>=d*1.02){ȡ(0);}else{ȡ(1);
}}else if(!ƚ){ő(Ɓ,Ť,true);if(Ҩ<1){ŀ(ƀ,q);}else if(Ҩ<d*.75||(!ź&&Ҩ<d*.98)){float Ä=(float)d/ӡ*q;ŀ(ƀ,Ä);}else if(Ҩ<d*.85)ŀ(
ƀ,15f);else if(Ҩ<=d*.98){ŀ(ƀ,1f);}else if(Ҩ>=d*1.2){ő(ҿ);}else if(Ҩ>=d*1.1){ŀ(Ɓ,15f);}else if(Ҩ>=d*1.02){ŀ(Ɓ,1f);}else{ő(
ҿ);ő(Ɓ,Ť,true);}}else{C(q);}}bool Ã=false;string Â="COMMUNICATIONS";void Á(ʈ À){À.ǵ(Â,"CommunicationsStealth",ref Ã,false
);}bool º=false;List<IMyRadioAntenna>µ=new List<IMyRadioAntenna>();List<IMyLaserAntenna>ª=new List<IMyLaserAntenna>();
string w(){µ.Clear();ª.Clear();ɫ<IMyRadioAntenna>(ref µ);ɫ<IMyLaserAntenna>(ref ª);for(int u=0;u<µ.Count;++u){if(µ[u].
CustomName.Contains("unused")||µ[u].CustomData.Contains("unused"))continue;if(!º){Ћ="Wico "+µ[u].CustomName.Split('!')[0].Trim();º
=true;}}return"A"+µ.Count.ToString("0");}void Å(){for(int r=0;r<µ.Count;r++){µ[r].Enabled=true;}}string p="";void o(){if(
Ѧ!=""){if(p==Ѧ){Ѧ="";}p=Ѧ;}else p="";}void n(){}bool m(){return true;}void l(bool g=false){if(µ.Count<1)w();foreach(var k
in µ){k.Radius=200;}}void j(float h=200,bool g=false){if(µ.Count<1)w();foreach(var ï in µ){{ï.Radius=h;ï.Enabled=true;}}}
Vector3D ñ(){if(µ.Count<1)w();foreach(var ï in µ){return ï.GetPosition();}Vector3D Ĩ=new Vector3D();return Ĩ;}float Ħ=float.
MaxValue;void ĥ(bool g=false,float Ĥ=float.MaxValue){if(Ĥ<200)Ĥ=200;Ħ=Ĥ;ģ(g);}void ģ(bool g=false){if(µ==null||µ.Count<1)w();
foreach(var k in µ){{float Ģ=k.GetMaximum<float>("Radius");if(Ħ<Ģ)Ģ=Ħ;k.Radius=Ģ;k.Enabled=true;}}}int ġ(){if(µ.Count<1)w();
return(µ.Count);}List<string>Ġ=new List<string>();void ħ(){}void Ğ(string Ĝ,string Ę){IGC.SendBroadcastMessage(Ĝ,Ę);}void Ğ(
long ĝ,string Ĝ,string Ę){IGC.SendUnicastMessage(ĝ,Ĝ,Ę);}List<string>ě=new List<string>();void Ě(){if(ě.Count>0){if(Ѧ==""){Ѧ
=ě[0];ě.RemoveAt(0);}else Echo("Waiting for message to be processed");ƍ();}if(ě.Count>0){}}void ę(string Ę){Echo(
"RECEIVE:\n"+Ę);ě.Add(Ę);Ě();}void ğ(){if(µ.Count>0){Echo(ě.Count+" Pending Incoming Messages");for(int r=0;r<ě.Count;r++)Echo(r+":"
+ě[r]);}else Echo("No antennas found");}List<IMyTerminalBlock>ĩ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ĺ=new
List<IMyTerminalBlock>();List<IMyTerminalBlock>ĸ=new List<IMyTerminalBlock>();bool ķ=false;string Ķ="[BASE]";string ĵ=
"[DOCK]";string Ĵ="CONNECTORS";void ĳ(ʈ À){À.ǵ(Ĵ,"BaseConnector",ref Ķ,true);À.ǵ(Ĵ,"DockConnector",ref ĵ,true);}string Ĳ(){ķ=
false;ĩ.Clear();Ĺ.Clear();ĸ.Clear();ı();return"CL"+ĩ.Count.ToString()+"CD"+Ĺ.Count.ToString()+"CB"+ĸ.Count.ToString();}void ı
(){if(ĩ.Count<1&&!ķ)ĩ=ɫ<IMyShipConnector>();if(Ĺ.Count<1&&!ķ)Ĺ=ɪ<IMyShipConnector>(ĵ);if(Ĺ.Count<1&&!ķ)Ĺ=ĩ;if(ĸ.Count<1&&
!ķ)ĸ=ɪ<IMyShipConnector>(Ķ);ķ=true;return;}bool İ(){return Ĺ.Count>1;}bool į(){ı();for(int r=0;r<Ĺ.Count;r++){var ą=Ĺ[r]
as IMyShipConnector;if(ą==null)continue;if(ą.Status==MyShipConnectorStatus.Connectable)return true;}return false;}bool Į()
{ı();for(int r=0;r<Ĺ.Count;r++){var ą=Ĺ[r]as IMyShipConnector;if(ą==null)continue;if(ą.Status==MyShipConnectorStatus.
Connected){var Ą=ą.OtherConnector;if(Ą.CubeGrid==ą.CubeGrid){continue;}else return true;}}return false;}void ĭ(){for(int r=0;r<Ĺ.
Count;r++){var ą=Ĺ[r]as IMyShipConnector;if(ą==null)continue;}}IMyTerminalBlock Ĭ(){ı();if(Ĺ.Count>0){return Ĺ[0];}return
null;}IMyTerminalBlock ī(bool Ī=false){ı();for(int r=0;r<Ĺ.Count;r++){var ą=Ĺ[r]as IMyShipConnector;if(ą==null)continue;if(ą
.Status==MyShipConnectorStatus.Connected){var Ą=ą.OtherConnector;if(Ą.CubeGrid==ą.CubeGrid){continue;}else{if(!Ī){return
ą.OtherConnector;}else{return Ĺ[r];}}}}return null;}void ò(bool ć=true,bool Ć=true){ı();for(int r=0;r<Ĺ.Count;r++){var ą=
Ĺ[r]as IMyShipConnector;if(ą==null)continue;if(ą.Status==MyShipConnectorStatus.Connected){var Ą=ą.OtherConnector;if(Ą.
CubeGrid==ą.CubeGrid){continue;}}if(ć){if(ą.Status==MyShipConnectorStatus.Connectable)ą.Connect();}else{if(ą.Status==
MyShipConnectorStatus.Connected)ą.Disconnect();}ą.Enabled=Ć;}return;}string ă="NOFOLLOW";string Ă="!WCC";string ā="[NAV]";string Ā=
"Craft Remote Control";string ÿ="GRIDS";void Ĉ(ʈ À){À.ǵ(ÿ,"NoFollow",ref ă,true);À.ǵ(ÿ,"BlockIgnore",ref Ă,true);À.ǵ(ÿ,
"OrientationBlockContains",ref ā,true);À.ǵ(ÿ,"OrientationBlockNamed",ref Ā,true);}List<IMyTerminalBlock>ý=new List<IMyTerminalBlock>();List<
IMyTextPanel>ü=new List<IMyTextPanel>();List<IMyTextPanel>û=new List<IMyTextPanel>();List<IMyTerminalBlock>ú=new List<
IMyTerminalBlock>();List<IMyCubeGrid>ù=new List<IMyCubeGrid>();List<IMyCubeGrid>ø=new List<IMyCubeGrid>();List<IMyCubeGrid>ö=new List<
IMyCubeGrid>();List<IMyCubeGrid>õ=new List<IMyCubeGrid>();bool ô(){List<IMyTerminalBlock>ó=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(ó);if(ѳ!=ó.Count){return true;}return false;}string þ(){ý.Clear();õ.Clear();ù.Clear()
;ø.Clear();ö.Clear();ü.Clear();û.Clear();ú.Clear();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(ý);ѳ=ý.Count;
foreach(var ĉ in ý){var č=ĉ.CubeGrid;if(!õ.Contains(č)){õ.Add(č);}}Ē(Me.CubeGrid);foreach(var č in õ){if(ù.Contains(č))continue
;bool Ė=false;List<IMyShipConnector>ĕ=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(ĕ
,(Ĕ=>Ĕ.CubeGrid==č));foreach(var ē in ĕ){if(ē.Status==MyShipConnectorStatus.Connected){if(ù.Contains(ē.OtherConnector.
CubeGrid)||ø.Contains(ē.OtherConnector.CubeGrid)){continue;}if(ù.Contains(ē.OtherConnector.CubeGrid))Ė=true;else Ė=false;}}if(Ė)
{if(!ö.Contains(č)){ö.Add(č);}}if(!ø.Contains(č)){ø.Add(č);}}string Ï="";Ï+="B"+ý.Count.ToString();Ï+="G"+õ.Count.
ToString();Ï+="L"+ù.Count.ToString();Ï+="D"+ö.Count.ToString();Ï+="R"+ø.Count.ToString();Echo("Found "+õ.Count.ToString()+
" Grids");Echo("Found "+ù.Count.ToString()+" Local Grids");for(int r=0;r<ù.Count;r++)Echo("|"+ù[r].CustomName);Echo("Found "+ö.
Count.ToString()+" Docked Grids");for(int r=0;r<ö.Count;r++)Echo("|"+ö[r].CustomName);Echo("Found "+ø.Count.ToString()+
" Remote Grids");for(int r=0;r<ø.Count;r++)Echo("|"+ø[r].CustomName);return Ï;}void Ē(IMyCubeGrid č){if(č==null)return;if(!ù.Contains(č
)){ù.Add(č);ė(č);Ď(č);Ċ(č);ɨ(č);}}void ė(IMyCubeGrid č){List<IMyMotorStator>đ=new List<IMyMotorStator>();
GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(đ,(ċ=>ċ.TopGrid==č));foreach(var ď in đ){if(ď.CustomName.Contains(ă)||ď.CustomData.
Contains(ă))continue;Ē(ď.CubeGrid);}List<IMyMotorAdvancedStator>Đ=new List<IMyMotorAdvancedStator>();GridTerminalSystem.
GetBlocksOfType<IMyMotorAdvancedStator>(Đ,(ċ=>ċ.TopGrid==č));foreach(var ď in Đ){if(ď.CustomName.Contains(ă)||ď.CustomData.Contains(ă))
continue;Ē(ď.CubeGrid);}}void Ď(IMyCubeGrid č){List<IMyPistonBase>Č=new List<IMyPistonBase>();GridTerminalSystem.GetBlocksOfType
<IMyPistonBase>(Č,(ċ=>ċ.TopGrid==č));foreach(var Ù in Č){Ē(Ù.CubeGrid);}}void Ċ(IMyCubeGrid č){List<IMyMotorStator>đ=new
List<IMyMotorStator>();GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(đ,(Ĕ=>Ĕ.CubeGrid==č));foreach(var ď in đ){if(ď.
CustomName.Contains(ă)||ď.CustomData.Contains(ă))continue;IMyCubeGrid ɦ=ď.TopGrid;if(ɦ!=null&&ɦ!=č){Ē(ɦ);}}đ.Clear();List<
IMyMotorAdvancedStator>Đ=new List<IMyMotorAdvancedStator>();GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(Đ,(Ĕ=>Ĕ.CubeGrid==č));
foreach(var ď in Đ){if(ď.CustomName.Contains(ă)||ď.CustomData.Contains(ă))continue;IMyCubeGrid ɦ=ď.TopGrid;if(ɦ!=null&&ɦ!=č){Ē(
ɦ);}}}void ɨ(IMyCubeGrid č){List<IMyPistonBase>Č=new List<IMyPistonBase>();GridTerminalSystem.GetBlocksOfType<
IMyPistonBase>(Č,(Ĕ=>Ĕ.CubeGrid==č));foreach(var Ù in Č){IMyCubeGrid ɦ=Ù.TopGrid;if(ɦ!=null&&ɦ!=č){if(!ù.Contains(ɦ)){Ē(ɦ);}}}}List<
IMyCubeGrid>ɥ(){if(ù.Count<1){þ();}return ù;}List<IMyCubeGrid>ɤ(){if(ù.Count<1){þ();}return ö;}bool ɣ(IMyTerminalBlock ĉ){return ɥ(
).Contains(ĉ.CubeGrid);}bool ɢ(long ɡ){for(int u=0;u<ù.Count;u++){if((long)ù[u].EntityId==ɡ)return true;}return false;}
bool ɢ(IMyCubeGrid ɡ){return ɥ().Contains(ɡ);}bool ɧ(IMyTerminalBlock ĉ){var ɓ=ɤ();if(ɓ==null)return false;return ɓ.Contains
(ĉ.CubeGrid);}void ɰ(){if(ý.Count<1)þ();ú.Clear();foreach(var ɮ in ý){if(ɣ(ɮ)&&!(ɮ.CustomName.Contains(Ă)))ú.Add(ɮ);}}
IMyTerminalBlock ɭ(string ɬ){IMyTerminalBlock ĉ;ĉ=(IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(ɬ);if(ĉ==null)throw new
Exception(ɬ+" Not Found");return ĉ;}List<ɇ>ɫ<ɇ>(ref List<ɇ>Ʌ,string Ɇ=null)where ɇ:class{if(Ʌ==null)Ʌ=new List<ɇ>();else Ʌ.Clear(
);if(ú.Count<1)ɰ();for(int Ʉ=0;Ʉ<ú.Count;Ʉ++){if(ú[Ʉ]is ɇ&&((Ɇ==null)||(Ɇ!=null&&ú[Ʉ].CustomName.StartsWith(Ɇ)))){Ʌ.Add((
ɇ)ú[Ʉ]);}}return Ʌ;}List<IMyTerminalBlock>ɫ<ɇ>(ref List<IMyTerminalBlock>Ʌ,string Ɇ=null)where ɇ:class{if(ý.Count<1)þ();
if(Ʌ==null)Ʌ=new List<IMyTerminalBlock>();else Ʌ.Clear();if(ú.Count<1)ɰ();for(int Ʉ=0;Ʉ<ú.Count;Ʉ++){if(ú[Ʉ]is ɇ&&((Ɇ==
null)||(Ɇ!=null&&ú[Ʉ].CustomName.StartsWith(Ɇ)))){Ʌ.Add(ú[Ʉ]);}}return Ʌ;}List<IMyTerminalBlock>ɫ<ɇ>(string Ɇ=null)where ɇ:
class{var Ʌ=new List<IMyTerminalBlock>();ɫ<ɇ>(ref Ʌ,Ɇ);return Ʌ;}List<IMyTerminalBlock>ɪ<ɇ>(string Ɇ=null)where ɇ:class{var Ʌ
=new List<IMyTerminalBlock>();if(ú.Count<1)ɰ();for(int Ʉ=0;Ʉ<ú.Count;Ʉ++){if(ú[Ʉ]is ɇ&&Ɇ!=null&&(ú[Ʉ].CustomName.Contains
(Ɇ)||ú[Ʉ].CustomData.Contains(Ɇ))){Ʌ.Add(ú[Ʉ]);}}return Ʌ;}List<IMyTextPanel>ɩ(string Ɇ=null){if(ý.Count<1)þ();var Ʌ=new
List<IMyTextPanel>();if(ü.Count>1){foreach(var Ⱦ in ü){if(Ɇ!=null&&(Ⱦ.CustomName.Contains(Ɇ)||Ⱦ.CustomData.Contains(Ɇ)))Ʌ.
Add(Ⱦ);}}else{foreach(var Ⱦ in ý){if(Ⱦ is IMyTextPanel&&ɣ(Ⱦ)&&!(Ⱦ.CustomName.Contains(Ă)||Ⱦ.CustomData.Contains(Ă))){if(Ɇ!=
null&&(Ⱦ.CustomName.Contains(Ɇ)||Ⱦ.CustomData.Contains(Ɇ)))Ʌ.Add(Ⱦ as IMyTextPanel);ü.Add(Ⱦ as IMyTextPanel);}}}return Ʌ;}
List<IMyTextPanel>ɋ(string Ɇ=null){if(ú.Count<1)ɰ();var Ʌ=new List<IMyTextPanel>();if(û.Count>1){foreach(var Ⱦ in û){if(Ɇ!=
null&&(Ⱦ.CustomName.Contains(Ɇ)||Ⱦ.CustomData.Contains(Ɇ)))Ʌ.Add(Ⱦ);}}else{foreach(var Ⱦ in ú){if(Ⱦ is IMyTextPanel&&Me.
CubeGrid==Ⱦ.CubeGrid){if(Ɇ!=null&&(Ⱦ.CustomName.Contains(Ɇ)||Ⱦ.CustomData.Contains(Ɇ)))Ʌ.Add(Ⱦ as IMyTextPanel);û.Add(Ⱦ as
IMyTextPanel);}}}return Ʌ;}List<IMyTerminalBlock>Ɋ<ɇ>(string Ɇ=null)where ɇ:class{if(ú.Count<1)ɰ();var Ʌ=new List<IMyTerminalBlock>(
);for(int Ʉ=0;Ʉ<ú.Count;Ʉ++){if(ú[Ʉ]is ɇ&&Me.CubeGrid==ú[Ʉ].CubeGrid&&Ɇ!=null&&(ú[Ʉ].CustomName.Contains(Ɇ)||ú[Ʉ].
CustomData.Contains(Ɇ))){Ʌ.Add(ú[Ʉ]);}}return Ʌ;}List<IMyTerminalBlock>ɉ<ɇ>(string Ɇ=null)where ɇ:class{if(ú.Count<1)ɰ();var Ʌ=new
List<IMyTerminalBlock>();for(int Ʉ=0;Ʉ<ú.Count;Ʉ++){if(ú[Ʉ]is ɇ&&Ɇ!=null&&ú[Ʉ].CustomName==Ɇ){Ʌ.Add(ú[Ʉ]);}}return Ʌ;}
IMyTerminalBlock Ƀ=null;string ɂ(){string Ɂ="";var ɀ=new List<IMyTerminalBlock>();ɫ<IMyTerminalBlock>(ref ɀ,Ā);if(ɀ.Count==0){ɀ=ɪ<
IMyRemoteControl>(ā);if(ɀ.Count==0){ɫ<IMyRemoteControl>(ref ɀ);if(ɀ.Count==0){ɫ<IMyCockpit>(ref ɀ);int r=0;for(;r<ɀ.Count;r++){Echo(
"Checking Controller:"+ɀ[r].CustomName);if(ɀ[r]is IMyCryoChamber)continue;break;}if(r>=ɀ.Count){Ɂ+="!!NO valid Controller:"+r+"\n";Echo(
"No Controller found");}else{Ɂ+="S";Echo("Using good ship Controller: "+ɀ[r].CustomName);}}else{Ɂ+="R";Echo(
"Using First Remote control found: "+ɀ[0].CustomName);}}}else{Ɂ+="N";Echo("Using Named: "+ɀ[0].CustomName);}if(ɀ.Count>0)Ƀ=ɀ[0];return Ɂ;}string ȿ="!NAV";
void Ɉ(ʈ À){À.ǵ(ÿ,"GyroIgnore",ref ȿ,true);À.ǵ(ÿ,"LIMIT_GYROS",ref ɠ,true);À.ǵ(ÿ,"LEAVE_GYROS",ref ɞ,true);À.ǵ(ÿ,
"CTRL_COEFF",ref ɖ,true);}double ɖ=0.9;int ɠ=99;int ɞ=-1;IMyShipController ɝ;List<IMyGyro>ɜ=new List<IMyGyro>();float ɛ=0.01f;bool ɚ
(string ə){if(ɝ==null)ʏ();if(ɝ is IMyShipController){Vector3D Ò=(ɝ as IMyShipController).GetNaturalGravity();return ɚ(ə,Ò
,Ƀ);}else{Echo("No Controller for gravity");}return true;}bool ɚ(string ə,Vector3D ɘ,IMyTerminalBlock ɟ){bool ɗ=true;if(ɝ
==null)ʏ();Matrix ɕ;ɟ.Orientation.GetMatrix(out ɕ);Vector3D ɔ;ə=ə.ToLower();if(ə.Contains("rocket"))ɔ=ɕ.Backward;else if(ə
.Contains("up"))ɔ=ɕ.Up;else if(ə.Contains("backward"))ɔ=ɕ.Backward;else if(ə.Contains("forward"))ɔ=ɕ.Forward;else if(ə.
Contains("right"))ɔ=ɕ.Right;else if(ə.Contains("left"))ɔ=ɕ.Left;else ɔ=ɕ.Down;ɘ.Normalize();for(int u=0;u<ɜ.Count;++u){var ɓ=ɜ[u
];ɓ.Orientation.GetMatrix(out ɕ);var ɒ=Vector3D.Transform(ɔ,MatrixD.Transpose(ɕ));var ɑ=Vector3D.Transform(ɘ,MatrixD.
Transpose(ɓ.WorldMatrix.GetOrientation()));var ɐ=Vector3D.Cross(ɒ,ɑ);double ɏ=Vector3D.Dot(ɒ,ɑ);double Ɏ=ɐ.Length();Ɏ=Math.Atan2(
Ɏ,Math.Sqrt(Math.Max(0.0,1.0-Ɏ*Ɏ)));if(ɏ<0)Ɏ=Math.PI-Ɏ;if(Ɏ<ɛ){ɓ.GyroOverride=false;continue;}float ɍ=(float)(2*Math.PI);
double ɯ=ɍ*(Ɏ/Math.PI)*ɖ;ɯ=Math.Min(ɍ,ɯ);ɯ=Math.Max(0.01,ɯ);ɐ.Normalize();ɐ*=ɯ;float Ɍ=-(float)ɐ.X;if(Math.Abs(ɓ.Pitch-Ɍ)>0.01
)ɓ.Pitch=Ɍ;float ɱ=-(float)ɐ.Y;if(Math.Abs(ɓ.Yaw-ɱ)>0.01)ɓ.Yaw=ɱ;float ʑ=-(float)ɐ.Z;if(Math.Abs(ɓ.Roll-ʑ)>0.01)ɓ.Roll=ʑ;
ɓ.GyroOverride=true;ɗ=false;}return ɗ;}string ʏ(){string Ï="";var ʎ=new List<IMyTerminalBlock>();ɝ=Ƀ as IMyShipController
;ɜ.Clear();if(ɝ==null){if(ʎ.Count<1)return"No RC!";}ʋ();GridTerminalSystem.GetBlocksOfType<IMyGyro>(ʎ,ċ=>ċ.CubeGrid==Ƀ.
CubeGrid);int ʍ=0;for(int u=0;u<ʎ.Count;u++){if(ʎ[u].CustomName.Contains(ȿ)||ʎ[u].CustomData.Contains(ȿ)){ʍ++;continue;}ɜ.Add(ʎ[
u]as IMyGyro);}if(ɠ>0){if(ɜ.Count>ɠ){ɜ.RemoveRange(ɠ,ɜ.Count-ɠ);}else{if((ɞ-ʍ)>0){int ʌ=ɜ.Count-(ɞ-ʍ);ɜ.RemoveRange(ʌ,(ɞ-
ʍ));}}}ʋ();Ï+="GYRO#"+ɜ.Count.ToString("00")+"#";return Ï;}void ʋ(){if(ɜ!=null){for(int u=0;u<ɜ.Count;++u){ɜ[u].
GyroOverride=false;ɜ[u].Enabled=true;}}}bool ʊ(Vector3D ʐ,Vector3D ʉ,IMyTerminalBlock ʇ){bool Ó=false;Vector3D ʆ=(ʉ-ʐ);Vector3D ʅ;if
(ʇ is IMyShipController){ʅ=((IMyShipController)ʇ).CenterOfMass;}else{ʅ=ʇ.GetPosition();}Vector3D ʄ=(ʉ-ʅ);Vector3D ʃ=ʁ(ʆ,ʄ
);Vector3D ʂ=(ʉ-ʃ*2)-ʅ;Ó=ɚ("forward",ʂ,ʇ);return Ó;}Vector3D ʁ(Vector3D k,Vector3D ʀ){if(Vector3D.IsZero(ʀ))return
Vector3D.Zero;return k-k.Dot(ʀ)/ʀ.LengthSquared()*ʀ;}class ʈ{char ɿ='[';char ʒ=']';string ʩ=";";string ʨ="";public bool ʧ=false;
public string ʦ="";string ʥ="---";char ʤ='|';private MyGridProgram ʣ;private Dictionary<string,string>ʢ;private Dictionary<
string,string[]>ʡ;private Dictionary<string,Dictionary<string,string>>ʠ;private string ʟ="";static string[]ʞ={"true","yes",
"on","1"};const StringComparison ʝ=StringComparison.OrdinalIgnoreCase;const char ʜ='=';public bool ʛ{get;private set;}=false
;public ʈ(MyGridProgram ʚ,string ʘ){ʣ=ʚ;ʢ=new Dictionary<string,string>();ʡ=new Dictionary<string,string[]>();ʠ=new
Dictionary<string,Dictionary<string,string>>();ʙ(ʘ);}public int ʙ(string ʘ){ʘ.TrimEnd();if(ʟ==ʘ){return ʢ.Count;}ʢ.Clear();ʡ.Clear
();ʠ.Clear();ʨ="";ʦ="";ʛ=false;ʟ=ʘ;string[]ʗ=ʘ.Split('\n');for(int ʖ=0;ʖ<ʗ.Count();ʖ++){string ʕ="";ʗ[ʖ].Trim();if(ʗ[ʖ].
StartsWith(ɿ.ToString())){string ʔ="";for(int ʓ=1;ʓ<ʗ[ʖ].Length;ʓ++)if(ʗ[ʖ][ʓ]==ʒ)break;else ʔ+=ʗ[ʖ][ʓ];if(ʔ!=""){ʕ=ʔ.ToUpper();}
else continue;ʖ++;string ǭ="";var ɾ=new string[ʗ.Count()-ʖ];int ɽ=0;var Ǳ=new Dictionary<string,string>();for(;ʖ<ʗ.Count();ʖ
++){ʗ[ʖ].Trim();if(ʗ[ʖ].StartsWith(ɿ.ToString())||ʗ[ʖ].StartsWith(ʥ)){ʖ--;break;}ǭ+=ʗ[ʖ]+"\n";ɾ[ɽ++]=ʗ[ʖ];if(ʗ[ʖ].Contains
(ʜ)){string[]ɹ=ʗ[ʖ].Split('=');if(ɹ.Count()>1){string ǖ=ɹ[0];string ɸ="";for(int u=1;u<ɹ.Count();u++){ɸ+=ɹ[u];if(u+1<ɹ.
Count())ɸ+=ʜ;}if(ɸ==""){int ɷ=ʖ+1;for(;ɷ<ɹ.Count();ɷ++){ʗ[ɷ].Trim();if(ʗ[ɷ].Length>1&&ʗ[ɷ][0]==ʤ){ɸ+=ʗ[ɷ].Substring(1).Trim()
+"\n";break;}}ʖ=ɷ;}Ǳ.Add(ǖ,ɸ);}}else if(ʗ[ʖ].StartsWith(ʩ)){}}if(!ʠ.ContainsKey(ʕ)){ʠ.Add(ʕ,Ǳ);if(!ʡ.ContainsKey(ʕ))ʡ.Add
(ʕ,ɾ);}else{}if(!ʢ.ContainsKey(ʕ)){ʢ.Add(ʕ,ǭ);}else{ʛ=true;}}else if(ʗ[ʖ].StartsWith(ʥ)){ʖ++;for(;ʖ<ʗ.Count();ʖ++){ʦ+=ʗ[ʖ
];}}else{ʨ+=ʗ[ʖ]+"\n";}}return ʢ.Count;}public string ɶ(string ǯ){string ǭ="";if(ʢ.ContainsKey(ǯ))ǭ=ʢ[ǯ];return ǭ;}public
string[]ɵ(string ǯ){string[]ɴ={""};if(ʡ.ContainsKey(ǯ))ɴ=ʡ[ǯ];return ɴ;}public bool ǵ(string ǯ,string ǖ,ref string ɳ,bool Ǹ=
false){ǯ=ǯ.ToUpper();if(ʠ.ContainsKey(ǯ)){var ǲ=ʠ[ǯ];if(ǲ.ContainsKey(ǖ)){ɳ=ǲ[ǖ];return true;}}if(Ǹ)ǰ(ǯ,ǖ,ɳ);return false;}
public bool ǵ(string ǯ,string ǖ,ref long ɲ,bool Ǹ=false){string ǳ="";if(!ǵ(ǯ,ǖ,ref ǳ)){if(Ǹ){ǰ(ǯ,ǖ,ɲ);}return false;}ɲ=Convert
.ToInt64(ǳ);return true;}public bool ǵ(string ǯ,string ǖ,ref int ɼ,bool Ǹ=false){string ǳ="";if(!ǵ(ǯ,ǖ,ref ǳ)){if(Ǹ){ǰ(ǯ,
ǖ,ɼ);}return false;}ɼ=Convert.ToInt32(ǳ);return true;}public bool ǵ(string ǯ,string ǖ,ref double Ǻ,bool Ǹ=false){string ǳ
="";if(!ǵ(ǯ,ǖ,ref ǳ)){if(Ǹ){ǰ(ǯ,ǖ,Ǻ);}return false;}bool ɻ=double.TryParse(ǳ,out Ǻ);return true;}public bool ǵ(string ǯ,
string ǖ,ref float ǻ,bool Ǹ=false){string ǳ="";if(!ǵ(ǯ,ǖ,ref ǳ)){if(Ǹ){ǰ(ǯ,ǖ,ǻ.ToString());}return false;}bool ɻ=float.
TryParse(ǳ,out ǻ);return true;}public bool ǵ(string ǯ,string ǖ,ref DateTime Ǽ,bool Ǹ=false){string ǳ="";if(!ǵ(ǯ,ǖ,ref ǳ)){if(Ǹ){
ǰ(ǯ,ǖ,Ǽ);}return false;}Ǽ=DateTime.Parse(ǳ);return true;}public bool ǵ(string ǯ,string ǖ,ref Vector3D Ǯ,bool Ǹ=false){
string ǳ="";if(!ǵ(ǯ,ǖ,ref ǳ)){if(Ǹ){ǰ(ǯ,ǖ,Ǯ);}return false;}double Ĕ,Ƿ,Ƕ;Ǎ(ǳ,out Ĕ,out Ƿ,out Ƕ);Ǯ.X=Ĕ;Ǯ.Y=Ƿ;Ǯ.Z=Ƕ;return true;
}public bool ǵ(string ǯ,string ǖ,ref bool Ǵ,bool Ǹ=false){string ǳ="";if(!ǵ(ǯ,ǖ,ref ǳ)){if(Ǹ){ǰ(ǯ,ǖ,Ǵ);}return false;}Ǵ=ʞ
.Any(ǁ=>string.Equals(ǳ,ǁ,ʝ));return true;}public bool ǰ(string ǯ,string ǖ,string ǳ){if(ʢ.ContainsKey(ǯ)){ʢ[ǯ]="";}else{ʢ
.Add(ǯ,"");ʛ=true;}if(ʠ.ContainsKey(ǯ)){var Ǳ=new Dictionary<string,string>();var ǲ=ʠ[ǯ];if(ǲ.ContainsKey(ǖ)){if(ǲ[ǖ]==ǳ)
return false;ǲ[ǖ]=ǳ;}else{ǲ.Add(ǖ,ǳ);}ʛ=true;}else{var Ǳ=new Dictionary<string,string>();Ǳ.Add(ǖ,ǳ);ʠ.Add(ǯ,Ǳ);ʛ=true;}return
true;}public bool ǰ(string ǯ,string ǖ,Vector3D Ǯ){ǰ(ǯ,ǖ,ǆ(Ǯ));return true;}public bool ǰ(string ǯ,string ǖ,bool Ǵ){ǰ(ǯ,ǖ,Ǵ.
ToString());return true;}public bool ǰ(string ǯ,string ǖ,int Ǿ){ǰ(ǯ,ǖ,Ǿ.ToString());return true;}public bool ǰ(string ǯ,string ǖ
,long ǽ){ǰ(ǯ,ǖ,ǽ.ToString());return true;}public bool ǰ(string ǯ,string ǖ,DateTime Ǽ){ǰ(ǯ,ǖ,Ǽ.ToString());return true;}
public bool ǰ(string ǯ,string ǖ,float ǻ){ǰ(ǯ,ǖ,ǻ.ToString());return true;}public bool ǰ(string ǯ,string ǖ,double Ǻ){ǰ(ǯ,ǖ,Ǻ.
ToString());return true;}public void ǹ(string ǯ,string ǭ){ǭ.TrimEnd();ǯ=ǯ.ToUpper();if(ʢ.ContainsKey(ǯ)){if(ʢ[ǯ]!=ǭ){ʢ[ǯ]=ǭ;ʛ=
true;}}else{ʛ=true;ʢ.Add(ǯ,ǭ);}}public string ǔ(bool ǀ=true){string ǒ="";string Ǒ=ʨ.Trim();if(ʧ&&Ǒ!="")ǒ=Ǒ+"\n";foreach(var
ǐ in ʢ){ǒ+=ɿ+ǐ.Key.Trim()+ʒ+"\n";if(ǐ.Value.TrimEnd()==""){string Ǐ="";if(ʠ.ContainsKey(ǐ.Key)){foreach(var ǎ in ʠ[ǐ.Key]
){Ǐ+=ǎ.Key+ʜ+ǎ.Value+"\n";}}Ǐ+="\n";ǒ+=Ǐ;}else{ǒ+=ǐ.Value.Trim()+"\n\n";}}if(ʦ!=""){ǒ+="\n"+ʥ+"\n";ǒ+=ʦ+"\n";}if(ǀ){ʛ=
false;ʟ=ǒ;}return ǒ;}bool Ǎ(string ǌ,out double ċ,out double ǋ,out double Ǌ){string[]Ǔ=ǌ.Trim().Split(',');if(Ǔ.Length<3){Ǔ=ǌ
.Trim().Split(':');}ċ=0;ǋ=0;Ǌ=0;if(Ǔ.Length<3)return false;bool ǉ=double.TryParse(Ǔ[0].Trim(),out ċ);bool ǈ=double.
TryParse(Ǔ[1].Trim(),out ǋ);bool Ǉ=double.TryParse(Ǔ[2].Trim(),out Ǌ);if(!ǉ||!ǈ||!Ǉ){return false;}return true;}string ǆ(
Vector3D ǅ){string Ï;Ï=ǅ.X.ToString("0.00")+":"+ǅ.Y.ToString("0.00")+":"+ǅ.Z.ToString("0.00");return Ï;}}List<IMyTerminalBlock>ǂ
=new List<IMyTerminalBlock>();string Ǆ(){ǂ.Clear();ǂ=ɫ<IMyLightingBlock>();return"L"+ǂ.Count.ToString("00");}void ǃ(List<
IMyTerminalBlock>ǂ,Color ǁ){for(int r=0;r<ǂ.Count;r++){var Ǖ=ǂ[r]as IMyLightingBlock;if(Ǖ==null)continue;if(Ǖ.Color.Equals(ǁ)&&Ǖ.Enabled
){continue;}Ǖ.Color=ǁ;}}string Ǡ="[WICO]";double Ǭ=0.175;const string Ǫ="SENSORS";void ǩ(ʈ À){À.ǵ(Ǫ,"SensorUse",ref Ǡ,
true);À.ǵ(Ǫ,"SensorSettleWaitMS",ref Ǭ,true);}List<IMySensorBlock>Ǩ=new List<IMySensorBlock>();struct ǧ{public long Ǧ;public
double ǥ;public double Ǥ;public double ǣ;public double Ǣ;public double ǫ;public double ǡ;}List<ǧ>ǟ=new List<ǧ>();string Ǟ(
IMyTerminalBlock ǝ,bool ǜ=false){Ǩ.Clear();ǟ.Clear();List<IMyTerminalBlock>Ǜ=ɪ<IMySensorBlock>(Ǡ);Ѽ Ë=new Ѽ(Ƀ);Vector3D ǚ;Vector3D Ǚ;
Vector3D ǘ;Vector3D Ǘ;Vector3D ǿ;Vector3D Ȁ;Vector3D Ȗ;Vector3D ȁ;Vector3D[]Ê=new Vector3D[4];Ë.Ғ(Ѽ.қ,Ê);Ǚ=Ê[0];Ǘ=Ê[1];ǚ=Ê[2];ǘ=
Ê[3];Ë.Ғ(Ѽ.Ҝ,Ê);Ȁ=Ê[0];ȁ=Ê[1];ǿ=Ê[2];Ȗ=Ê[3];foreach(var Ȫ in Ǜ){Ǩ.Add(Ȫ as IMySensorBlock);ǧ ȯ=new ǧ();ȯ.Ǧ=Ȫ.EntityId;
Vector3D Ȕ=Ȫ.GetPosition();double ȓ=ȩ(Ȕ,Ǚ,Ǘ,ǘ);double ȑ=ȩ(Ȕ,Ȁ,ȁ,Ȗ);double ȏ=ȩ(Ȕ,Ǚ,ǚ,Ȁ);double Ȏ=ȩ(Ȕ,Ǘ,ǘ,ȁ);double ȍ=ȩ(Ȕ,ǚ,ǘ,ǿ);
double Ȍ=ȩ(Ȕ,Ǚ,Ǘ,ȁ);ȯ.ǥ=ȓ;ȯ.Ǥ=ȑ;ȯ.ǣ=ȏ;ȯ.Ǣ=Ȏ;ȯ.ǫ=ȍ;ȯ.ǡ=Ȍ;ǟ.Add(ȯ);}if(ǜ)ȫ();return"S"+Ǩ.Count.ToString("00");}List<
IMySensorBlock>Ȱ(string Ȯ=null){List<IMySensorBlock>Ȭ=new List<IMySensorBlock>();for(int u=0;u<Ǩ.Count;u++){IMySensorBlock Ï=Ǩ[u]as
IMySensorBlock;if(Ï==null)continue;if(Ï.IsActive&&Ï.Enabled&&!Ï.LastDetectedEntity.IsEmpty()){Ȭ.Add(Ǩ[u]);}}return Ȭ;}void ȫ(){for(int
u=0;u<Ǩ.Count;u++){IMySensorBlock Ȫ=Ǩ[u]as IMySensorBlock;if(Ȫ==null)continue;Ȫ.LeftExtend=Ȫ.RightExtend=Ȫ.TopExtend=Ȫ.
BottomExtend=Ȫ.FrontExtend=Ȫ.BackExtend=1;Ȫ.Enabled=false;}}double ȩ(Vector3D Ȕ,Vector3D Ȩ,Vector3D ȧ,Vector3D ȭ){double D=0;
Vector3D ȱ=ȼ(Ȩ,ȧ,ȭ);Vector3D Ƚ=Ȕ-Ȩ;D=Vector3D.Dot(Ƚ,ȱ);return D;}Vector3D ȼ(Vector3D Ȩ,Vector3D ȧ,Vector3D ȭ){Vector3D Ȼ=Ȩ-ȧ;Ȼ.
Normalize();Vector3D Ⱥ=ȧ-ȭ;Ⱥ.Normalize();Vector3D ȱ=Vector3D.Cross(Ȼ,Ⱥ);ȱ.Normalize();return ȱ;}void ȹ(IMyTerminalBlock ȸ,float ȷ
,float ȶ,float ȵ,float ȴ,float ȳ,float Ȳ){IMySensorBlock Ȫ=ȸ as IMySensorBlock;int u=0;for(;u<ǟ.Count;u++){if(ǟ[u].Ǧ==Ȫ.
EntityId)break;}if(u<ǟ.Count){float ȕ=0;if(ȷ<0)ȕ=-ȷ;else ȕ=(float)Math.Abs(ȷ+Math.Abs(ǟ[u].ǣ));Ȫ.LeftExtend=Math.Max(ȕ,1.0f);if(
ȶ<0)ȕ=-ȶ;else ȕ=(float)Math.Abs(ȶ+Math.Abs(ǟ[u].Ǣ));Ȫ.RightExtend=Math.Max(ȕ,1.0f);if(ȵ<0)ȕ=-ȵ;else ȕ=(float)Math.Abs(ȵ+
Math.Abs(ǟ[u].ǫ));Ȫ.TopExtend=Math.Max(ȕ,1.0f);if(ȴ<0)ȕ=-ȴ;else ȕ=(float)Math.Abs(ȴ+Math.Abs(ǟ[u].ǡ));Ȫ.BottomExtend=Math.
Max(ȕ,1.0f);if(ȳ<0)ȕ=-ȳ;else ȕ=(float)Math.Abs(ȳ+Math.Abs(ǟ[u].ǥ));Ȫ.FrontExtend=Math.Max(ȕ,1.0f);if(Ȳ<0)ȕ=-Ȳ;else ȕ=(float
)Math.Abs(Ȳ+Math.Abs(ǟ[u].Ǥ));Ȫ.BackExtend=Math.Max(ȕ,1.0f);}else{Ѽ Ë=new Ѽ(Ƀ);Vector3D ǚ;Vector3D Ǚ;Vector3D ǘ;Vector3D
Ǘ;Vector3D ǿ;Vector3D Ȁ;Vector3D Ȗ;Vector3D ȁ;Vector3D[]Ê=new Vector3D[4];Ë.Ғ(Ѽ.қ,Ê);Ǚ=Ê[0];Ǘ=Ê[1];ǚ=Ê[2];ǘ=Ê[3];Ë.Ғ(Ѽ.Ҝ,
Ê);Ȁ=Ê[0];ȁ=Ê[1];ǿ=Ê[2];Ȗ=Ê[3];Ծ("FBL",Ǚ);Ծ("FBR",Ǘ);Ծ("FTL",ǚ);Ծ("FTR",ǘ);Ծ("BBL",Ȁ);Ծ("BBR",ȁ);Ծ("BTL",ǿ);Ծ("BTR",Ȗ);if
(Ȫ==null)return;Echo(Ȫ.CustomName);Vector3D Ȕ=Ȫ.GetPosition();double ȓ=ȩ(Ȕ,Ǚ,Ǘ,ǘ);Echo("DistanceFront="+ȓ.ToString("0.00"
));Vector3D Ȓ=Ȕ+Ȫ.WorldMatrix.Forward*ȓ;Ծ("FRONT",Ȓ);double ȑ=ȩ(Ȕ,Ȁ,ȁ,Ȗ);Echo("DistanceBack="+ȑ.ToString("0.00"));
Vector3D Ȑ=Ȕ+Ȫ.WorldMatrix.Forward*ȑ;Ծ("BACK",Ȑ);double ȏ=ȩ(Ȕ,Ǚ,ǚ,Ȁ);double Ȏ=ȩ(Ȕ,Ǘ,ǘ,ȁ);double ȍ=ȩ(Ȕ,ǚ,ǘ,ǿ);double Ȍ=ȩ(Ȕ,Ǚ,Ǘ,ȁ)
;float ȕ=0;ȕ=(float)Math.Abs(ȷ+Math.Abs(ȏ));Ȫ.LeftExtend=Math.Max(ȕ,1.0f);ȕ=(float)Math.Abs(ȶ+Math.Abs(Ȏ));Ȫ.RightExtend=
Math.Max(ȕ,1.0f);ȕ=(float)Math.Abs(ȵ+Math.Abs(ȍ));Ȫ.TopExtend=Math.Max(ȕ,1.0f);ȕ=(float)Math.Abs(ȴ+Math.Abs(Ȍ));Ȫ.
BottomExtend=Math.Max(ȕ,1.0f);ȕ=(float)Math.Abs(ȳ+Math.Abs(ȓ));Ȫ.FrontExtend=Math.Max(ȕ,1.0f);ȕ=(float)Math.Abs(Ȳ+Math.Abs(ȑ));Ȫ.
BackExtend=Math.Max(ȕ,1.0f);}Ȫ.Enabled=true;}bool ȋ(IMySensorBlock Ǒ,ref bool ȉ,ref bool Ȉ,ref bool ȇ){ȉ=false;Ȉ=false;ȇ=false;if(
Ǒ!=null&&Ǒ.IsActive&&Ǒ.Enabled&&!Ǒ.LastDetectedEntity.IsEmpty()){List<MyDetectedEntityInfo>Ȇ=new List<
MyDetectedEntityInfo>();Ǒ.DetectedEntities(Ȇ);for(int Î=0;Î<Ȇ.Count;Î++){if(Ȇ[Î].Type==MyDetectedEntityType.Asteroid){ȉ=true;}else if(Ȇ[Î].
Type==MyDetectedEntityType.LargeGrid){Ȉ=true;}else if(Ȇ[Î].Type==MyDetectedEntityType.SmallGrid){ȇ=true;}}}return ȉ||Ȉ||ȇ;}
List<IMyTerminalBlock>ȅ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ȅ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ȃ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ȃ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ȋ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>ȗ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ȟ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>Ȧ=new List<IMyTerminalBlock>();string ȥ(IMyTerminalBlock Ŝ){ȅ.Clear();Ȅ.Clear();ȃ.Clear();Ȃ.Clear();Ȋ.Clear();ȗ.Clear()
;Ȟ.Clear();Ȧ.Clear();ɫ<IMyMotorSuspension>(ref ȅ);for(int r=0;r<ȅ.Count;r++){if(ȅ[r].CustomName.Contains("[SLED]")||ȅ[r].
CustomData.Contains("[SLED]")){Ȅ.Add(ȅ[r]);if(ȅ[r].CustomName.Contains("[REAR]")||ȅ[r].CustomData.Contains("[FRONT]")){ȃ.Add(ȅ[r])
;}if(ȅ[r].CustomName.Contains("[FRONT]")||ȅ[r].CustomData.Contains("[FRONT]")){Ȃ.Add(ȅ[r]);}}else{if(ȅ[r].CustomName.
Contains("[LEFT]")||ȅ[r].CustomData.Contains("[LEFT]")){Ȟ.Add(ȅ[r]);}else if(ȅ[r].CustomName.Contains("[RIGHT]")||ȅ[r].
CustomData.Contains("[RIGHT]")){Ȧ.Add(ȅ[r]);}if(ȅ[r].CustomName.Contains("[REAR]")||ȅ[r].CustomData.Contains("[FRONT]")){Ȋ.Add(ȅ[r
]);}if(ȅ[r].CustomName.Contains("[FRONT]")||ȅ[r].CustomData.Contains("[FRONT]")){ȗ.Add(ȅ[r]);}}}return"W"+ȅ.Count.
ToString("0")+"WS"+Ȅ.Count.ToString("0")+"SR"+ȃ.Count.ToString("0")+"SF"+Ȃ.Count.ToString("0");}bool Ȥ(){if(Ȅ.Count>0)return
true;return false;}void ȣ(){foreach(var ȝ in Ȅ){var Ș=ȝ as IMyMotorSuspension;Ș.SetValueFloat("Friction",0);}}bool Ȣ(){if(ȅ.
Count>0){return true;}return false;}bool ȡ(float Ƞ,float ș=-1){Echo("WPP:"+Ƞ.ToString()+":"+ș.ToString());bool ȟ=true;if(Ƞ<0f
)Ƞ=0f;if(Ƞ>100f)Ƞ=100f;foreach(var ȝ in Ȧ){var Ș=ȝ as IMyMotorSuspension;float Ȝ=Ș.GetValueFloat("Propulsion override");
Echo("CPower:"+Ȝ.ToString("0.00")+"\n"+Ș.CustomName);float ț=(Ȝ);ț=Math.Abs(ț);if(ț<1)ț*=100f;if(Ƞ>(ț+5f)){ȟ=false;ț+=5;}
else if(Ƞ<(ț-5)){ȟ=false;ț-=5;}else ț=Ƞ;if(ș>=0)Ș.SetValueFloat("Friction",ș);Echo("Setting override to"+ț.ToString("0.000")
);Ș.SetValueFloat("Propulsion override",-ț);}foreach(var ȝ in Ȟ){var Ș=ȝ as IMyMotorSuspension;float Ȝ=Ș.GetValueFloat(
"Propulsion override");Echo("CPower:"+Ȝ.ToString("0.00")+"\n"+Ș.CustomName);float ț=(Ȝ);ț=Math.Abs(ț);if(ț<1)ț*=100f;if(Ƞ>(ț+5f)){ȟ=false;ț+=
5;}else if(Ƞ<(ț-5)){ȟ=false;ț-=5;}else ț=Ƞ;if(ș>=0)Ș.SetValueFloat("Friction",ș);Echo("Setting override to"+(ț).ToString(
"0.000"));Ș.SetValueFloat("Propulsion override",ț);}return ȟ;}void Ț(float ș){foreach(var ȝ in ȅ){var Ș=ȝ as IMyMotorSuspension
;Ș.SetValueFloat("Friction",ș);}}