#region mdk preserve
#region mdk macros
// Minified script was deployed at $MDK_DATETIME$
#endregion
#endregion



/* Wico Craft DOCKING Control sub-module
 * 
 * Workshop: http://steamcommunity.com/sharedfiles/filedetails/?id=883865519
 * 
 * Uncompressed Source: https://github.com/Wicorel/WicoSpaceEngineers/tree/master/Wico%20Dock
* 
* Handles MODES:
* MODE_DOCKED
* MODE_DOCKING
* MODE_LAUNCH
* MODE_RELAUNCH
* 

3.0h 
Minify to make 100k room
Removed gyronics
Removed rolls and yaws (for sleds)
remember mom (id)
Get location from 'mom' via antenna. 
When docking, if out of antenna range from mom, move into antenna range before asking for dock

3.0i
Use sensors when moving. If hit something, move away from it.
say 'hello' when Orphan (to make mom respond)
minor adjustments to travel movement to reduce default ranges.

3.0j
timeout on requesting dock (5 seconds)
'fix' by force fZOffset in sensor size calc so sensor doens't extend many meters behind ship.
If secondary avoid hits asteroid, go to ATTENTION (punt on drone control)

3.0k
when 'stuck' use cameras to try to find a way out.
calcfoward now uses WorldMatrix instead of deriving it

3.0l
check for shorter hitpoints if none are clear.  Try to escape to furtherest.
move to half-way to hitpoint and then try again
set forward collision detection based on stopping distance and not target distance (needed for in-asteroid operations)

3.0m
use only stoppingdistance for travel modes/speed

3.0N
code reduction pass
NOTE: Can't dock with something attached to our front (mini cargo) because collision detection 'hits' it..
docking with alignment.

3.0O
update getgrids to NOFOLLOW
Fix crash when attached to print head

3.0P
minor opt in GyroMain
dont go FAST when waiting for reply from dock

3.0Q
localGridFilter in BlockInit
leave LIMIT_GYROS to default

3.0R MDK Version (compressed)

3.0S Don't clear GPS Panel
add 'forgetmom' command to make drone forget about a previous 'motherhsip.

3.0T remove MOM and use new 'BASE' system for docking

3.0T2 search order for text panels

3.0U 
Add delays for requests for BASE?
Turn drills and ejectors off when heading to dock. (drills at start and ejectors when arrive at approach).

3.1 Version for SE 1.185 PB Changes
Turn off logging because of text panel writes causing hang.

3.2 Handle docking with connectors in any orientation

delay for motion after going 'home' (added state 169)

improve long-range travel for docking (chooses optimal speed, uses dampeners, etc). (major changes to TravelMovement code)
Handle case where requested to dock, but have not heard from a base yet; request and wait a bit for a response. (added state 109)

3.2a Remove blockApplyActions and connector actions

3.2b Common DoTravelMemovement and collision code.

3.2c Docking messages now include size of ship in meters so base can choose best available connector

3.2d new travelmovement

3.2E 
Increase scan delay time in tm to .225
only use fast when needed for docking.
use .Once for FAST
12232017

3.2F INI Save 12262017
fix bug in serialize wrting z,y z, instead of x,y,z (oops)
MODE_DOCKED tries to fill tanks and batteries.

3.2g 01062018

3.2h

3.3 Multiple Text Panels.
Only write to panels at the end of script.

3.3a defaultorientationblock code moved

3.4A init optimization.  check instructions on each sub-init

3.4B
Current Code Compile Mar 08 2018

3.4C Current Code Compile Mar 28 2018
Redo BASE serilization to be key=value
Use NAV module for long-range travel

3.4D Current Code Apr 22, 2018

3.4E May 05 2018 + May 18 2018 May 27 2018
Added Report text panel output to docking states.
Source published June 01 2018

3.4F June 02 2018
BeamRider for docking
test with off-center connetors (hence beamrider)

3.4G June 20 2018
Current Source
Dont count sorters (nor ejectors) in cargo space
Gyro: Add support for 'right' and 'left' align
NavCommon: default max speed to 999
Added quadrant scan to ScanEscape
Some ScanEscape() improvements for being inside grids (still needs work)
Add ScanTest() routine for raycasts.

3.4H July 18 2018
Add timer triggers to the different docking modes
July 22 2018: SE 1.187  MDK 1.1.16

3.4I Sep 08 2018
Performance Pass

3.5 Jan 25 2019 SE 1.189 

3.7 May 28 2019 SE 1.190  
IGC Support for Intergrid communication

3.71 June 22 2019 SE 1.191
Cargo check for relaunch defaults to 1% max cargo (from 80% !!!!!)

3.8 Dec 26 2019 SE 1.193.100
Removal of old IGC/Antenna


TODO:
forget known bases when new grid (DONE by craft control)
support 'memory' connector; like MK3 did
support multiple 'memmory' connectors (named for ease of use by player)
ability to turn off 'wico' communication docking (no antenna communications)
timeout after requesting dock; remove base from list or mark 'dead?'
docking maneuver sequence (runway) (text panel sequence like other scripts?)
launch maneuver sequence (ditto)
support working in gravity
Need to support cargo transport ship modes.  With pull from one dock and push to another.
Dock request (Get power, give power, ore dump, get ice, give ice, etc)
get launch info (speed, length, etc) from dock, not CustomData..
hangar opening/closing sequence

See: https://steamcommunity.com/sharedfiles/filedetails/?id=1330904900


*/
string щ="Wico Craft";string ш="Dock";string ч="3.8";const string ц="0.00";void х(){}void ф(){Ɩ();}void у(bool т=false){
Ǜ(Ƌ);ͺ();if(ɵ is IMyRemoteControl)((IMyRemoteControl)ɵ).SetAutoPilotEnabled(false);if(ɵ is IMyShipController)((
IMyShipController)ɵ).DampenersOverride=true;}void с(ˣ Ë){ñ(Ë);Í(Ë);Ի(Ë);Ӏ(Ë);}void р(ˣ Ë){ð(Ë);Ì(Ë);Ժ(Ë);Ӕ(Ë);}о п=new о();class о{public
const Base6Directions.Direction н=Base6Directions.Direction.Forward;public const Base6Directions.Direction м=Base6Directions.
Direction.Backward;public const Base6Directions.Direction л=Base6Directions.Direction.Left;public const Base6Directions.Direction
к=Base6Directions.Direction.Right;public const Base6Directions.Direction й=Base6Directions.Direction.Up;public const
Base6Directions.Direction и=Base6Directions.Direction.Down;public float з=30.0f;public List<IMyGyro>ж=new List<IMyGyro>();
Base6Directions.Direction е=й;Base6Directions.Direction д=л;Base6Directions.Direction г=н;Base6Directions.Direction ъ=й;Base6Directions
.Direction ы=л;Base6Directions.Direction ї=н;public void ѕ(List<IMyTerminalBlock>і){ж=і.ConvertAll(ǧ=>(IMyGyro)ǧ);if(ж.
Count>0)з=ж[0].GetMaximum<float>("Yaw");}public void ѕ(List<IMyGyro>і){ж.Clear();if(і==null)return;ж=і;if(ж.Count>0)з=ж[0].
GetMaximum<float>("Yaw");}public void ѕ(IMyProgrammableBlock є,IMyGridTerminalSystem ѓ){ж.Clear();if((ѓ!=null)&&(є!=null))ѓ.
GetBlocksOfType<IMyGyro>(ж,ǧ=>((ǧ.CubeGrid==є.CubeGrid)&&ǧ.IsFunctional));if(ж.Count>0)з=ж[0].GetMaximum<float>("Yaw");}public void ђ(
IMyTerminalBlock ё,Base6Directions.Direction ѐ=н,Base6Directions.Direction я=й){if(Base6Directions.GetAxis(ѐ)==Base6Directions.GetAxis(я
))я=Base6Directions.GetPerpendicular(ѐ);if(ё==null){}else{Vector3 К=Base6Directions.GetVector(ѐ);Vector3.TransformNormal(
ref К,ё.Orientation,out К);ѐ=Base6Directions.GetDirection(ref К);К=Base6Directions.GetVector(я);Vector3.TransformNormal(ref
К,ё.Orientation,out К);я=Base6Directions.GetDirection(ref К);}ъ=я;ї=ѐ;ы=Base6Directions.GetLeft(ъ,ї);}public void ю(bool
ʘ){for(int w=0;w<ж.Count;w++){ж[w].GyroOverride=ʘ;}}public void ю(int О,bool ʘ){if(О<ж.Count){ж[О].GyroOverride=ʘ;}}
public void э(float ь){for(int w=0;w<ж.Count;w++){ж[w].GyroPower=ь;}}public void э(int О,float ь){if(О<ж.Count){ж[О].GyroPower
=ь;}}public void И(bool Ф){for(int w=0;w<ж.Count;w++){ж[w].Enabled=Ф;}}public void И(int О,bool Ф){if(О<ж.Count){ж[О].
Enabled=Ф;}}public void У(bool Т){for(int w=0;w<ж.Count;w++){ж[w].ShowOnHUD=Т;}}public void У(int О,bool Т){if(О<ж.Count){ж[О].
ShowOnHUD=Т;}}void С(Base6Directions.Direction Р,out string М,out float Л){М="Yaw";Л=-1.0f;if(Base6Directions.GetAxis(е)==
Base6Directions.GetAxis(Р)){if(е==Р)Л=1.0f;}if(Base6Directions.GetAxis(д)==Base6Directions.GetAxis(Р)){М="Pitch";if(д==Р)Л=1.0f;}if(
Base6Directions.GetAxis(г)==Base6Directions.GetAxis(Р)){М="Roll";if(г==Р){}else Л=1.0f;}}public void П(IMyGyro О,string М,float ʘ){if(М
=="Yaw"){О.Yaw=ʘ;}else if(М=="Pitch"){О.Pitch=ʘ;}else{О.Roll=ʘ;}}public void Н(float ʴ){for(int w=0;w<ж.Count;w++){string
М;float Л;Vector3 К=Base6Directions.GetVector(й);Vector3.TransformNormal(ref К,ж[w].Orientation,out К);е=Base6Directions.
GetDirection(ref К);С(ъ,out М,out Л);П(ж[w],М,Л*ʴ);}}public void Й(float ʵ){for(int w=0;w<ж.Count;w++){string М;float Л;Vector3 К=
Base6Directions.GetVector(л);Vector3.TransformNormal(ref К,ж[w].Orientation,out К);д=Base6Directions.GetDirection(ref К);С(ы,out М,out
Л);П(ж[w],М,Л*ʵ);}}public void в(float ʳ){for(int w=0;w<ж.Count;w++){string М;float Л;Vector3 К=Base6Directions.GetVector
(н);Vector3.TransformNormal(ref К,ж[w].Orientation,out К);г=Base6Directions.GetDirection(ref К);С(ї,out М,out Л);П(ж[w],М
,Л*ʳ);}}public void б(float ʴ,float ʵ,float ʳ){for(int w=0;w<ж.Count;w++){string М;float Л;Vector3 К=Base6Directions.
GetVector(н);Vector3.TransformNormal(ref К,ж[w].Orientation,out К);г=Base6Directions.GetDirection(ref К);К=Base6Directions.
GetVector(л);Vector3.TransformNormal(ref К,ж[w].Orientation,out К);д=Base6Directions.GetDirection(ref К);К=Base6Directions.
GetVector(й);Vector3.TransformNormal(ref К,ж[w].Orientation,out К);е=Base6Directions.GetDirection(ref К);С(ъ,out М,out Л);П(ж[w],
М,Л*ʴ);С(ы,out М,out Л);П(ж[w],М,Л*ʵ);С(ї,out М,out Л);П(ж[w],М,Л*ʳ);}}}double а(Vector3D Я,IMyTerminalBlock Ю){double Э=
0;bool Ь=false;MatrixD Ы=ҋ(Ю);Vector3D Ъ=Ю.GetPosition();Vector3D ȩ=Ъ+1.0*Vector3D.Normalize(Ы.Backward);Vector3D Щ=Ъ+1.0
*Vector3D.Normalize(Ы.Right);Vector3D Ш=Ъ-1.0*Vector3D.Normalize(Ы.Right);double Ч=Ґ(Щ,Я);double Ц=Ґ(Ш,Я);double Х=Ґ(Щ,Ш)
;double ј=Vector3D.DistanceSquared(Ъ,Я);double ѩ=Vector3D.DistanceSquared(ȩ,Я);Ь=ј<ѩ;Э=(Ц-Ч)/Х;if(!Ь){Э+=(Э<0)?-1:1;}
return Э;}double Ґ(Vector3D G,Vector3D Ē){return Vector3D.Distance(G,Ē);}MatrixD ҏ(IMyCubeGrid ɿ){Vector3D Ҏ=ɿ.
GridIntegerToWorld(new Vector3I(0,0,0));Vector3D ҍ=ɿ.GridIntegerToWorld(new Vector3I(0,1,0))-Ҏ;Vector3D Ҍ=ɿ.GridIntegerToWorld(new
Vector3I(0,0,1))-Ҏ;return MatrixD.CreateScale(ɿ.GridSize)*MatrixD.CreateWorld(Ҏ,-Ҍ,ҍ);}MatrixD ҋ(IMyCubeBlock Ҋ){Matrix ҁ;Ҋ.
Orientation.GetMatrix(out ҁ);return ҁ*MatrixD.CreateTranslation(((Vector3D)new Vector3D(Ҋ.Min+Ҋ.Max))/2.0)*ҏ(Ҋ.CubeGrid);}bool Ҁ(
double ѿ,string Ѿ="Roll",float ѽ=-1,float Ѽ=1f){float ѻ=0;if(п.ж.Count<1)Echo("NO GYROS!!!");float Ѻ=60f;if(ѽ>0)Ѻ=ѽ;if(Math.
Abs(ѿ)>1.0){ѻ=Ѻ*(float)(ѿ)*Ѽ;}else if(Math.Abs(ѿ)>.7){ѻ=Ѻ*(float)(ѿ)/4;}else if(Math.Abs(ѿ)>0.5){ѻ=0.11f*Math.Sign(ѿ);}else
if(Math.Abs(ѿ)>0.1){ѻ=0.11f*Math.Sign(ѿ);}else if(Math.Abs(ѿ)>0.01){ѻ=0.11f*Math.Sign(ѿ);}else if(Math.Abs(ѿ)>0.001){ѻ=
0.09f*Math.Sign(ѿ);}else ѻ=0;п.Н(ѻ);if(Math.Abs(ѿ)<ˉ){Echo("DR():Aimed");п.ю(false);}else{п.ю(true);п.И(true);return false;}
return true;}double ѹ=-1;int Ѹ=-1;List<IMyTerminalBlock>ѷ=new List<IMyTerminalBlock>();bool Ѷ(IMyTerminalBlock Ǘ){if(Ǘ is
IMyBatteryBlock){IMyBatteryBlock ѵ=Ǘ as IMyBatteryBlock;return(ѵ.ChargeMode==ChargeMode.Recharge);}else return false;}bool Ѵ(
IMyTerminalBlock Ǘ){if(Ǘ is IMyBatteryBlock){IMyBatteryBlock ѵ=Ǘ as IMyBatteryBlock;return(ѵ.ChargeMode==ChargeMode.Discharge);}else
return false;}bool ґ(IMyTerminalBlock Ǘ){if(Ǘ is IMyBatteryBlock){IMyBatteryBlock ѵ=Ǘ as IMyBatteryBlock;return ѵ.IsCharging;}
else return false;}void ҡ(){ѷ.Clear();Ѹ=-1;ѹ=-1;GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(ѷ,ʌ);if(ѷ.Count>0)ѹ=0;
foreach(var ҟ in ѷ){IMyBatteryBlock Ҟ=ҟ as IMyBatteryBlock;ѹ+=Ҟ.MaxOutput;}}double Ҡ(){double ż=0;foreach(var ҟ in ѷ){
IMyBatteryBlock Ҟ=ҟ as IMyBatteryBlock;ż+=Ҟ.CurrentOutput;}return ż;}bool ҝ(int Ҝ,bool ѳ=true,bool қ=false){float Қ=0;float ҙ=0;bool Ҙ=
false;float җ;if(ѷ.Count<1)ҡ();if(ѷ.Count<1)return false;Ѹ=0;for(int Җ=0;Җ<ѷ.Count;Җ++){float ҕ=0;float Ά=0;int Ҕ=100;
IMyBatteryBlock Ē;Ē=ѷ[Җ]as IMyBatteryBlock;җ=Ē.MaxStoredPower;Ά+=җ;Қ+=җ;җ=Ē.CurrentStoredPower;ҕ+=җ;ҙ+=җ;if(Ά>0){җ=((ҕ*100)/Ά);җ=(float
)Math.Round(җ,0);Ҕ=(int)җ;}string ũ;ũ="";if(Ѷ(ѷ[Җ]))ũ+="R";else if(Ѵ(ѷ[Җ]))ũ+="D";else ũ+="a";float ǉ;ǉ=Ē.CurrentInput;if
(ǉ>0)ũ+="+";else ũ+=" ";ǉ=Ē.CurrentOutput;if(ǉ>0)ũ+="-";else ũ+=" ";ũ+=Ҕ+"%";ũ+=":"+ѷ[Җ].CustomName;if(ѳ)Echo(ũ);if(Ѷ(ѷ[Җ
])&&Ҝ>0){if(Ҕ<Ҝ)Ҙ=true;else if(Ҕ>99){Ē.ChargeMode=ChargeMode.Recharge;}}if(!(Ē.ChargeMode==ChargeMode.Recharge)&&Ҕ<Ҝ&&!Ҙ)
{Ē.ChargeMode=ChargeMode.Recharge;Ҙ=true;}}if(Қ>0){җ=((ҙ*100)/Қ);җ=(float)Math.Round(җ,0);Ѹ=(int)җ;}else Ѹ=-1;return Ҙ;}
void ғ(){for(int w=0;w<ѷ.Count;w++){IMyBatteryBlock Ē;Ē=ѷ[w]as IMyBatteryBlock;Ē.ChargeMode=ChargeMode.Auto;}}void Ғ(bool ѳ=
false,bool Ѳ=true){if(ѳ)Echo(ѷ.Count+" Batteries");string ũ;for(int w=0;w<ѷ.Count;w++){IMyBatteryBlock Ē;Ē=ѷ[w]as
IMyBatteryBlock;if(Ѳ){Ē.ChargeMode=ChargeMode.Discharge;}else Ē.ChargeMode=ChargeMode.Recharge;ũ=Ē.CustomName+": ";if(Ē.ChargeMode==
ChargeMode.Recharge){ũ+="RECHARGE/";}else ũ+="NOTRECHARGE/";if(Ē.ChargeMode==ChargeMode.Discharge){ũ+="DISCHARGE";}else{ũ+=
"NOTDISCHARGE";}if(ѳ)Echo(ũ);}}void ѧ(List<IMyTerminalBlock>ĉ,bool Ĉ=true){foreach(var Ē in ĉ){IMyFunctionalBlock ѥ=Ē as
IMyFunctionalBlock;if(ѥ==null)continue;ѥ.Enabled=Ĉ;}}void Ѧ(List<IMyTerminalBlock>ĉ){foreach(var Ē in ĉ){IMyFunctionalBlock ѥ=Ē as
IMyFunctionalBlock;if(ѥ==null)continue;ѥ.Enabled=!ѥ.Enabled;}}string Ѥ="[VIEW]";Matrix ѣ=new Matrix(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);List<
IMyTerminalBlock>Ѣ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ѡ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ѡ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>џ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ў=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ѝ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ќ=new List<IMyTerminalBlock>();IMyTerminalBlock ћ=null;
MyDetectedEntityInfo ϑ;string њ="CAMERAS";void љ(ˣ Ë){Ë.ȓ(њ,"CameraViewOnly",ref Ѥ,true);}bool Ѩ(List<IMyTerminalBlock>χ,double ѱ=100,float
ʵ=0,float ʴ=0){double ѯ=0;ћ=null;for(int w=0;w<χ.Count;w++){double Ѯ=((IMyCameraBlock)χ[w]).AvailableScanRange;if(Ѯ>ѯ){ѯ=
Ѯ;ћ=χ[w];}}var ς=ћ as IMyCameraBlock;if(ћ==null){return false;}if(ς.CanScan(ѱ)){ϑ=ς.Raycast(ѱ,ʵ,ʴ);ћ=ς;if(!ϑ.IsEmpty())ϼ(
ϑ);return true;}else{}return false;}bool Ѩ(List<IMyTerminalBlock>χ,Vector3D Ѱ){double ѯ=0;ћ=null;for(int w=0;w<χ.Count;w
++){double Ѯ=((IMyCameraBlock)χ[w]).AvailableScanRange;if(Ѯ>ѯ){ѯ=Ѯ;ћ=χ[w];}}var ς=ћ as IMyCameraBlock;if(ћ==null)return
false;{ϑ=ς.Raycast(Ѱ);ћ=ς;if(!ϑ.IsEmpty())ϼ(ϑ);return true;}}double ѭ(List<IMyTerminalBlock>χ){double Ѭ=0;for(int w=0;w<χ.
Count;w++){IMyCameraBlock ς=χ[w]as IMyCameraBlock;if(Ѭ<ς.AvailableScanRange)Ѭ=ς.AvailableScanRange;}return Ѭ;}string ѫ(
IMyTerminalBlock Ń){Ѣ.Clear();ѡ.Clear();Ѡ.Clear();џ.Clear();ў.Clear();ѝ.Clear();ќ.Clear();if(Ń==null)return
"\nCameras:No OrientationBlock";GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(ќ,(ĺ=>ĺ.CubeGrid==Me.CubeGrid));Matrix ŭ;Ń.Orientation.GetMatrix(out
ŭ);Matrix.Transpose(ref ŭ,out ŭ);for(int w=0;w<ќ.Count;++w){if(ќ[w].CustomName.Contains(Ѥ))continue;IMyCameraBlock ς=ќ[w]
as IMyCameraBlock;ς.EnableRaycast=true;Matrix Ѫ;ς.Orientation.GetMatrix(out Ѫ);Vector3 ū=Vector3.Transform(Ѫ.Forward,ŭ);if
(ū==ѣ.Left){ў.Add(ќ[w]);}else if(ū==ѣ.Right){ѝ.Add(ќ[w]);}else if(ū==ѣ.Backward){ѡ.Add(ќ[w]);}else if(ū==ѣ.Forward){Ѣ.Add
(ќ[w]);}else if(ū==ѣ.Up){џ.Add(ќ[w]);}else if(ū==ѣ.Down){Ѡ.Add(ќ[w]);}}string ũ;ũ="CS:<";ũ+="F"+Ѣ.Count.ToString("00");ũ
+="B"+ѡ.Count.ToString("00");ũ+="D"+Ѡ.Count.ToString("00");ũ+="U"+џ.Count.ToString("00");ũ+="L"+ў.Count.ToString("00");ũ+=
"R"+ѝ.Count.ToString("00");ũ+=">";return ũ;}void З(List<IMyTerminalBlock>χ,string Ϫ){string ĩ;for(int w=0;w<χ.Count;w++){if
(!χ[w].CustomName.Contains(Ϫ)){ĩ="Camera ";if(χ.Count>1)ĩ+=(w+1).ToString()+" ";ĩ+=Ϫ;χ[w].CustomName=ĩ;}}}List<
IMyTerminalBlock>υ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>τ=new List<IMyTerminalBlock>();string σ(IMyTerminalBlock Ń){string
ũ="";if(ќ.Count<1)ũ+=ѫ(Ń);υ.Clear();τ.Clear();foreach(var ς in Ѡ){if(ς.CustomName.ToLower().Contains("fore")||ς.
CustomData.ToLower().Contains("fore"))υ.Add(ς);else if(ς.CustomName.ToLower().Contains("aft")||ς.CustomData.ToLower().Contains(
"aft"))τ.Add(ς);}ũ+="HCS:<";ũ+="F"+υ.Count.ToString("00");ũ+="A"+τ.Count.ToString("00");ũ+=">";return ũ;}class ρ{bool π=false
;bool ο=false;public bool ξ=false;public Vector3D ν;Program ʱ;public double μ=1250;double λ=5000;float κ=25f;float ι=25f;
double θ=5;float η=3;float ζ=0.5f;public float ε=0;public float δ=0;float γ=0;float β=0;List<IMyTerminalBlock>φ=new List<
IMyTerminalBlock>();private int ψ=0;private int ϝ=1;public MyDetectedEntityInfo ϑ;public List<MyDetectedEntityInfo>Ϝ=new List<
MyDetectedEntityInfo>();public ρ(Program ʢ,List<IMyTerminalBlock>ĉ,double ϛ=1250,float Ϛ=45f,float ϙ=45f,float Ϙ=2,float ϗ=1,float ϖ=0.5f,
double ϕ=5000,bool ϔ=false){ʱ=ʢ;π=false;ο=ϔ;ξ=false;φ.Clear();Ϝ.Clear();ϑ=new MyDetectedEntityInfo();foreach(var Ē in ĉ){if(Ē
is IMyCameraBlock){φ.Add(Ē);IMyCameraBlock Ȍ=Ē as IMyCameraBlock;Ȍ.EnableRaycast=true;if(κ>Ȍ.RaycastConeLimit)κ=Ȍ.
RaycastConeLimit;if(ι>Ȍ.RaycastConeLimit)ι=Ȍ.RaycastConeLimit;}}if(ϛ>ϕ)ϕ=ϛ;μ=ϛ;κ=Ϛ;ι=ϙ;θ=Ϙ;η=ϗ;ζ=ϖ;λ=ϕ;ε=0;δ=0;γ=0;β=0;ψ=0;ϝ=φ.Count;}
public bool ϓ(){return π;}void ϒ(MyDetectedEntityInfo ϑ){bool ϐ=true;for(int w=0;w<Ϝ.Count;w++){if(Ϝ[w].EntityId==ϑ.EntityId)ϐ
=false;}if(ϐ){Ϝ.Add(ϑ);}}public bool Ϗ(){if(φ.Count<1)π=true;if(π)return false;bool ώ=false;for(int ύ=0;ύ<ϝ;ύ++){if(ʱ.Ѩ(φ
,μ,β,γ)){ϑ=ʱ.ϑ;if(!ϑ.IsEmpty()){bool ό=true;if((ϑ.Type==MyDetectedEntityType.LargeGrid)||(ϑ.Type==MyDetectedEntityType.
SmallGrid)){if(ʱ.ɻ(ϑ.EntityId)){ό=false;}}if(ό){ϒ(ϑ);ώ=true;}}else if(ο){π=true;Vector3D ϋ;Vector3D.CreateFromAzimuthAndElevation
(MathHelper.ToRadians(δ),MathHelper.ToRadians(ε),out ϋ);ν=Vector3D.TransformNormal(ϋ,ʱ.ћ.WorldMatrix);ξ=true;return false
;}ψ++;if(β==0&&γ==0){ε=ζ;δ=ζ;ψ=0;}if(ψ>3){ψ=0;δ+=Math.Abs(δ/η)+ζ;if(Math.Abs(δ)>κ){ψ=0;δ=0;ε+=Math.Abs(ε/η)+ζ;}if(Math.
Abs(ε)>ι){ε=0;δ=0;ψ=0;{μ*=θ;if(μ>λ){π=true;return false;}}}}switch(ψ){case 0:β=ε;γ=δ;break;case 1:β=-ε;γ=δ;break;case 2:β=ε
;γ=-δ;break;case 3:β=-ε;γ=-δ;break;}}}return ώ;}}int ϊ=5;int ω=-1;double ΰ=-1;string Ι="CARGO";void ί(ˣ Ë){Ë.ȓ(Ι,
"cargopctmin",ref ϊ,true);}List<IMyTerminalBlock>Η=null;bool Ζ=false;double Ε=0.0;void Δ(){var ĉ=new List<IMyTerminalBlock>();if(Η==
null)Η=new List<IMyTerminalBlock>();else Η.Clear();ɨ<IMyCargoContainer>(ref ĉ);Η.AddRange(ĉ);ω=-1;ΰ=-1;}void Γ(){var ĉ=new
List<IMyTerminalBlock>();ɨ<IMyShipConnector>(ref ĉ);foreach(var Ȍ in ĉ){if(Ȍ.CustomName.Contains("Ejector")||Ȍ.CustomData.
Contains("Ejector"))continue;else Η.Add(Ȍ);}}void Β(){var ĉ=new List<IMyTerminalBlock>();ɨ<IMyShipDrill>(ref ĉ);Η.AddRange(ĉ);}
void Θ(){var ĉ=new List<IMyTerminalBlock>();ɨ<IMyShipWelder>(ref ĉ);Η.AddRange(ĉ);}void Α(){var ĉ=new List<IMyTerminalBlock>
();ɨ<IMyShipGrinder>(ref ĉ);Η.AddRange(ĉ);}bool ΐ=true;void Ώ(){var ĉ=new List<IMyTerminalBlock>();if(Η==null)Η=new List<
IMyTerminalBlock>();else Η.Clear();if(!ΐ)GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(ĉ,ʌ);else ɨ<IMyCargoContainer>(ref ĉ);Η.
AddRange(ĉ);ĉ.Clear();if(!ΐ)GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(ĉ,ʌ);else ɨ<IMyShipConnector>(ref ĉ);foreach(
var Ȍ in ĉ){if(Ȍ.CustomName.Contains("Ejector")||Ȍ.CustomData.Contains("Ejector"))continue;else if(Ȍ.CustomName.Contains(
"Sorter")||Ȍ.CustomData.Contains("Sorter"))continue;else Η.Add(Ȍ);}ĉ.Clear();if(!ΐ)GridTerminalSystem.GetBlocksOfType<
IMyShipDrill>(ĉ,ʌ);else ɨ<IMyShipDrill>(ref ĉ);Η.AddRange(ĉ);ĉ.Clear();if(!ΐ)GridTerminalSystem.GetBlocksOfType<IMyShipWelder>(ĉ,ʌ);
else ɨ<IMyShipWelder>(ref ĉ);Η.AddRange(ĉ);ĉ.Clear();if(!ΐ)GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(ĉ,ʌ);else ɨ<
IMyShipGrinder>(ref ĉ);Η.AddRange(ĉ);ω=-1;ΰ=-1;}void Ύ(){if(Η==null)Ώ();if(Η.Count<1){ω=-1;ΰ=-1;return;}Ε=0.0;double Ό=0.0;double Ί=0;
bool Ή=true;bool Έ=false;for(int w=0;w<Η.Count;w++){double Ά=-1;var Κ=Η[w].InventoryCount;for(var έ=0;έ<Κ;έ++){var Ω=Η[w].
GetInventory(έ);if(Ω!=null){Ε+=(double)Ω.CurrentVolume;if((double)Ω.MaxVolume>9223372036854){Ζ=true;}else{Ζ=false;}if(!Ζ){Ά=(double)
Ω.MaxVolume;double ά=Ϋ(Η[w]);if(ά>0)ΰ=Ά/ά;}else{Ά=Ϋ(Η[w])*10;ΰ=9999;}if((double)Ω.CurrentVolume<Ά){if(!(Η[w]is
IMyShipDrill)){Ή=false;}}else{if(Η[w]is IMyShipDrill){Έ=true;}}}Ό+=Ά;}}if(Ό>0){Ί=(Ε/Ό)*100;}else{Ί=100;}ω=(int)Ί;if(Ή&&Έ)ω=101;}
double ή(IMyTerminalBlock Ϊ){double Ά=-1;var Κ=Ϊ.InventoryCount;for(var έ=0;έ<Κ;έ++){var Ω=Ϊ.GetInventory(έ);if(Ω!=null){Ε+=(
double)Ω.CurrentVolume;if((double)Ω.MaxVolume>9223372036854){Ζ=true;}else{Ζ=false;}if(!Ζ){Ά=(double)Ω.MaxVolume;double ά=Ϋ(Ϊ);
if(ά>0)ΰ=Ά/ά;}else{Ά=Ϋ(Ϊ)*10;ΰ=9999;}}}return Ά;}double Ϋ(IMyTerminalBlock Ϊ){var Ω=Ϊ.GetInventory(0);string Ψ=Ϊ.
BlockDefinition.SubtypeId;double Ά=(double)Ω.MaxVolume;if(Ά<999999999)return Ά;if(Ϊ is IMyCargoContainer){if(Ψ.Contains(
"LargeBlockLargeContainer"))Ά=421.875008;else if(Ψ.Contains("LargeBlockSmallContainer"))Ά=15.625;else if(Ψ.Contains("SmallBlockLargeContainer"))Ά=
15.625;else if(Ψ.Contains("SmallBlockMediumContainer"))Ά=3.375;else if(Ψ.Contains("SmallBlockSmallContainer"))Ά=0.125;else if(
Ψ.Contains("Azimuth_LargeContainer"))Ά=7780.8;else if(Ψ.Contains("Azimuth_MediumLargeContainer"))Ά=1945.2;else if(Ψ.
Contains("Azimuth_MediumContainer"))Ά=1878.6;else if(Ψ.Contains("Azimuth_SmallContainer"))Ά=10.125;}else if(Ψ.Contains(
"SmallBlockDrill"))Ά=3.375;else if(Ψ.Contains("LargeBlockDrill"))Ά=23.4375;else if(Ψ.Contains("ConnectorMedium"))Ά=1.152;else if(Ψ.
Contains("ConnectorSmall"))Ά=0.064;else if(Ψ.Contains("Connector"))Ά=8.000;else if(Ψ.Contains("LargeShipWelder"))Ά=15.625;else
if(Ψ.Contains("LargeShipGrinder"))Ά=15.625;else if(Ψ.Contains("SmallShipWelder"))Ά=3.375;else if(Ψ.Contains(
"SmallShipGrinder"))Ά=3.375;else{Echo("Unknown cargo for default Capacity:"+Ϊ.DefinitionDisplayNameText+":"+Ϊ.BlockDefinition.SubtypeId);Ά
=12;}return Ά;}const int Χ=0;const int Φ=2;const int Υ=4;const int Τ=8;const int Σ=16;const int Ρ=32;const int Π=64;const
int Ο=128;const int Ξ=256;const int Ν=512;const int Μ=1024;const int α=2048;const int Λ=0xfff;string Ϟ(){string Ј="FLAGS:";
if((Ӡ&Φ)>0)Ј+="SLED ";if((Ӡ&Ρ)>0)Ј+="ORBITAL ";if((Ӡ&Π)>0)Ј+="ROCKET ";if((Ӡ&Υ)>0)Ј+="ROTOR ";if((Ӡ&Τ)>0)Ј+="WHEEL ";if((Ӡ
&Ο)>0)Ј+="PET ";if((Ӡ&Ξ)>0)Ј+="NAD ";if((Ӡ&Ν)>0)Ј+="NO Gyro ";if((Ӡ&α)>0)Ј+="No Tank ";if((Ӡ&Μ)>0)Ј+="No Power ";return Ј
;}long І=0;MyIni Ѕ=new MyIni();string Є="";string Ѓ="";void Ђ(){if(Ԛ==null){Ѓ=Storage;}else{Ѓ=Ԛ.GetText();}if(ԙ==null)
return;if(Ѓ==Є){Echo("Load Skip");return;}Є=Ѓ;Ѓ=Ѓ.Trim();MyIniParseResult Ё;if(!Ѕ.TryParse(Ѓ,out Ё)){}ԙ.ʡ(Ѓ);ԙ.ȓ(Ԝ,"SaveID",
ref І);if(Ѐ()){ԙ.ʡ("");}р(ԙ);ԙ.ȓ(Ԝ,"Mode",ref ڂ,true);ԙ.ȓ(Ԝ,"current_state",ref Ԃ,true);ԙ.ȓ(Ԝ,"PassedArgument",ref Ӫ,true);
ԙ.ȓ(Ԝ,"AlertStates",ref ө,true);ԙ.ȓ(Ԝ,"craft_operation",ref Ӡ,true);ԙ.ȓ(Ԝ,"PassedArgument",ref Ӫ);ԙ.ȓ(Ԝ,"ReceivedMessage"
,ref Ө);}bool Ѐ(){if(Ԛ==null||Ċ)return false;if(І<=0||І==(long)Ԛ.EntityId)return false;else return true;}bool Ͽ(string Ͼ)
{Ͼ=Ͼ.Trim().ToLower();return(Ͼ=="True"||Ͼ=="true");}Dictionary<long,MyDetectedEntityInfo>Ͻ=new Dictionary<long,
MyDetectedEntityInfo>();void ϼ(MyDetectedEntityInfo ϻ){if(ϻ.EntityId!=0){if(!Ͻ.ContainsKey(ϻ.EntityId)){Ͻ.Add(ϻ.EntityId,ϻ);}else{Ͻ[ϻ.
EntityId]=ϻ;}}else Echo("Not adding: Zero Entity");}string Ϻ(MyDetectedEntityInfo Ϲ){string ũ="";ũ+="ETBV";ũ+=":"+Ϲ.EntityId.
ToString();ũ+=":"+Ϲ.TimeStamp;Vector3D ϸ=Ϲ.BoundingBox.Min;ũ+=":"+Ǡ(ϸ);Vector3D Ϸ=Ϲ.BoundingBox.Max;ũ+=":"+Ǡ(Ϸ);Vector3D ϵ=(
Vector3)Ϲ.Velocity;ũ+=":"+Ǡ(ϵ);return ũ;}struct ϴ{public long Ǿ;public string ĩ;public Vector3D Ĩ;public Vector3D ϳ;}List<ϴ>ϲ=
new List<ϴ>();bool ϱ(out ϴ Ї){ϴ Љ=new ϴ();Љ.Ǿ=0;Љ.ĩ="";Ї=Љ;{Echo("No saved remote connectors available");return false;}}
void Ж(){IMyTextPanel Д;List<IMyTerminalBlock>ĉ=new List<IMyTerminalBlock>();ĉ=ɧ<IMyTextPanel>("[DOCK]");if(ĉ.Count>0)Д=ĉ[0]
as IMyTextPanel;else return;ϲ.Clear();}void Е(){IMyTextPanel Д;List<IMyTerminalBlock>ĉ=new List<IMyTerminalBlock>();ĉ=ɧ<
IMyTextPanel>("[DOCK]");if(ĉ.Count>0)Д=ĉ[0]as IMyTextPanel;else return;StringBuilder ϧ=new StringBuilder();ϧ.Append(ϲ.Count.ToString
()+"\n");for(int w=0;w<ϲ.Count;w++){ϧ.Append(ϲ[w].Ǿ.ToString());ϧ.Append(":");ϧ.Append(ـ("",ϲ[w].ĩ));ϧ.Append(":");ϧ.
Append(Ǡ(ϲ[w].Ĩ));ϧ.Append(":");ϧ.Append(Ǡ(ϲ[w].ϳ));ϧ.Append("\n");}Д.WriteText(ϧ.ToString(),false);}void В(IMyTerminalBlock ʂ
){if(ʂ==null)return;Vector3D Ĩ=ʂ.GetPosition();MatrixD Г=ʂ.WorldMatrix;Vector3D ϫ=Г.Forward;ϫ.Normalize();В(ʂ.EntityId,ʂ.
CustomName,Ĩ,ϫ);}void В(long Ǿ,string ĩ,Vector3D Ĩ,Vector3D ϫ){for(int w=0;w<ϲ.Count;w++){if(ϲ[w].Ǿ==Ǿ||Ǿ==0){Echo(
"location already in list");return;}}ϴ Ї=new ϴ();Ї.Ǿ=Ǿ;Ї.ĩ=ĩ;Ї.Ĩ=Ĩ;Ї.ϳ=ϫ;ϲ.Add(Ї);Е();}ϴ Б=new ϴ();IMyTerminalBlock А;double Џ=-1;List<
IMyTerminalBlock>Ў=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ѝ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ќ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>Ћ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Њ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ϰ=new List<IMyTerminalBlock>();IMyBroadcastListener ϯ;IMyBroadcastListener ϟ;IMyBroadcastListener ϩ;void Ϩ(){Ջ("clear",
դ);Ջ(ш+":DOCKING!",դ);Echo("DOCKING: state="+Ԃ);ű=true;IMySensorBlock ϧ;if(А==null)Ԃ=0;if(Ԃ==0){if(Ā()){ط(ٸ);return;}ϟ=
IGC.RegisterBroadcastListener("COND");ϟ.SetMessageCallback("COND");ϩ=IGC.RegisterBroadcastListener("ACOND");ϩ.
SetMessageCallback("ACOND");ϯ=IGC.RegisterBroadcastListener("CONA");ϯ.SetMessageCallback("CONA");ư("[DOCKING]");А=þ();if(А==null){Echo(
"No local connector for docking");Ջ(ш+":No local Docking Connector Available!",է,true);ŵ+="\nNo local Docking Connector Available!";ط(پ);ų=false;return;
}else{у();ę();ń(А,ref Ѝ,ref Ў,ref ϰ,ref Њ,ref Ќ,ref Ћ);Ԃ=100;}Ý=0;}Vector3D Ŀ=А.GetPosition();if(!Ā()&&ā()){ù();у();ط(ٸ);
Ǜ(Ƌ,Ŋ,true);return;}if(Ԃ==100){float Ϧ=ħ()+100f+(float)ơ*5f;K(false,Ϧ);if(Ȁ.Count>0){ϧ=Ȁ[0];}Ԃ=110;}else if(Ԃ==110){if(ơ<
10){if(Ý<=0)Ý=ġ();ó=DateTime.Now;if(Ý>0){Ҹ(А);Vector3D[]ȭ=new Vector3D[4];ҹ.Ԅ(5,ȭ);double ϥ=(ȭ[0]-ȭ[1]).Length();double Ϥ=
(ȭ[0]-ȭ[2]).Length();ҹ.Ԅ(0,ȭ);double ϣ=(ȭ[0]-ȭ[2]).Length();string Ļ="";string Ϣ="CON?";Ļ+=Ý.ToString()+":";Ļ+=Ϥ.ToString
("0.0")+","+ϥ.ToString("0.0")+","+ϣ.ToString("0.0")+":";Ļ+=ɵ.CubeGrid.CustomName+":";Ļ+=Ԛ.EntityId.ToString()+":";Ļ+=Ǡ(ɵ.
GetPosition());B(Ϣ,Ļ);Ԃ=120;}else{ī(true);Ԃ=130;}}else у();}else if(Ԃ==120){Ջ("Awaiting Response from Base",դ);ų=false;DateTime ϡ=ó
.AddSeconds(5.0f);DateTime Ϡ=DateTime.Now;if(DateTime.Compare(Ϡ,ϡ)>0){ŵ+="\nTime out awaiting CONA";Ԃ=125;return;}if(ϯ.
HasPendingMessage){var Ľ=ϯ.AcceptMessage();string Ļ=(string)Ľ.Data;string[]ķ=Ļ.Trim().Split(':');Echo(ķ.Length+": Length");for(int w=0;w<
ķ.Length;w++)Echo(w+":"+ķ[w]);if(ķ.Length>1){Echo("Approach answer!");int Ķ=0;long ĵ=0;long.TryParse(ķ[Ķ++],out ĵ);if(ĵ==
Ԛ.EntityId){Ө="";long.TryParse(ķ[Ķ++],out ĵ);double ǧ,Ǧ,ǥ;ǧ=Convert.ToDouble(ķ[Ķ++]);Ǧ=Convert.ToDouble(ķ[Ķ++]);ǥ=Convert
.ToDouble(ķ[Ķ++]);Vector3D Ĩ=new Vector3D(ǧ,Ǧ,ǥ);Æ=Ĩ;Û=true;Ԃ=150;}}}else{Echo("Awaiting reply message");}}else if(Ԃ==125
){if(Ý<=0){ŵ+="\nNo Base in range";ط(پ);return;}else if(ħ()<3000){Ý=-1;ī(true);Ԃ=110;}else{Ә(Ĳ(Ý),ڂ,110,3100,
"DOCK Base Proximity");Ԃ=126;}}else if(Ԃ==126){Echo("Waiting for NAV to start");}else if(Ԃ==130){Ջ("Trying to find a base",դ);ų=false;
DateTime ϡ=ó.AddSeconds(2.0f);DateTime Ϡ=DateTime.Now;if(DateTime.Compare(Ϡ,ϡ)>0){ŵ+="\nTimeout finding base";ط(پ);return;}if(ġ(
)>=0)Ԃ=110;}else if(Ԃ==150){Ԃ=175;}else if(Ԃ==175){Ә(Æ,ڂ,200,5,"DOCK Base Approach");Ԃ=176;}else if(Ԃ==176){Echo(
"Waiting for NAV to start");}else if(Ԃ==200){Ջ("Requsting Docking Connector",դ);if(ơ<1){Ҹ(А);Vector3D[]ȭ=new Vector3D[4];ҹ.Ԅ(5,ȭ);double ϥ=(ȭ[0]-ȭ
[1]).Length();double Ϥ=(ȭ[0]-ȭ[2]).Length();ҹ.Ԅ(0,ȭ);double ϣ=(ȭ[0]-ȭ[2]).Length();string Ļ="";string Ϣ="COND?";Ļ+=Ý.
ToString()+":";Ļ+=Ϥ.ToString("0.0")+","+ϥ.ToString("0.0")+","+ϣ.ToString("0.0")+":";Ļ+=ɵ.CubeGrid.CustomName+":";Ļ+=Ԛ.EntityId.
ToString()+":";Ļ+=Ǡ(ɵ.GetPosition());B(Ϣ,Ļ);{ó=DateTime.Now;Ԃ=210;}}else у();}else if(Ԃ==210){Ջ(
"Awaiting reply with Docking Connector",դ);ų=false;DateTime ϡ=ó.AddSeconds(5.0f);DateTime Ϡ=DateTime.Now;if(DateTime.Compare(Ϡ,ϡ)>0){ŵ+=
"\nTime out awaiting COND";Ԃ=100;return;}if(ϱ(out Б)){Ԃ=250;}else{if(ϟ.HasPendingMessage||ϩ.HasPendingMessage){string Ļ="";bool Ϯ=false;if(ϟ.
HasPendingMessage){var Ľ=ϟ.AcceptMessage();Ļ=(string)Ľ.Data;}else{var Ľ=ϩ.AcceptMessage();Ļ=(string)Ľ.Data;Ϯ=true;}string[]ķ=Ļ.Trim().
Split(':');Echo(ķ.Length+": Length");for(int w=0;w<ķ.Length;w++)Echo(w+":"+ķ[w]);int Ķ=0;{Echo("Docking answer!");long ĵ=0;
long.TryParse(ķ[Ķ++],out ĵ);if(ĵ==Ԛ.EntityId){Ө="";long.TryParse(ķ[Ķ++],out ĵ);string ĩ=ķ[Ķ++];double ǧ,Ǧ,ǥ;ǧ=Convert.
ToDouble(ķ[Ķ++]);Ǧ=Convert.ToDouble(ķ[Ķ++]);ǥ=Convert.ToDouble(ķ[Ķ++]);Vector3D Ĩ=new Vector3D(ǧ,Ǧ,ǥ);ǧ=Convert.ToDouble(ķ[Ķ++])
;Ǧ=Convert.ToDouble(ķ[Ķ++]);ǥ=Convert.ToDouble(ķ[Ķ++]);Vector3D ϫ=new Vector3D(ǧ,Ǧ,ǥ);if(Ϯ){ǧ=Convert.ToDouble(ķ[Ķ++]);Ǧ=
Convert.ToDouble(ķ[Ķ++]);ǥ=Convert.ToDouble(ķ[Ķ++]);Ê=new Vector3D(ǧ,Ǧ,ǥ);É=true;}È=Ĩ;Ç=È+ϫ*(ӥ.Ӹ()*1.5);Æ=È+ϫ*(ӥ.Ӹ()*3);Å=true;
Ä=true;Û=true;Ջ("clear",բ);ف("dock",È);ف("launch1",Ç);ف("Home",Æ);Ԃ=300;}}}else{Echo("Awaiting reply message");}}}else if
(Ԃ==250){È=Б.Ĩ;Ç=È+Б.ϳ*(ӥ.Ӹ()*1.5);Æ=È+Б.ϳ*(ӥ.Ӹ()*3);Å=true;Ä=true;Û=true;Ԃ=300;Ջ("clear",բ);ف("dock",È);ف("launch1",Ç);ف
("Home",Æ);Ƽ();ų=true;}else if(Ԃ==300){Ԃ=310;Ƽ();ų=true;}else if(Ԃ==310){Echo("Moving to Home");Ә(Æ,ڂ,340,3,
"DOCK Approach");Ԃ=311;}else if(Ԃ==311){Echo("Waiting for NAV to start");}else if(Ԃ==340){у();Echo("Waiting for ship to stop");ē();Ƽ();
if(ơ<0.1f){ų=true;Ԃ=350;}else{Ų=true;}}else if(Ԃ==350){double ģ=(Æ-((IMyShipController)ɵ).CenterOfMass).LengthSquared();
Echo("DistanceSQ="+ģ.ToString("0.0"));double ϭ=Ʀ(Ś,ơ,0);if(ģ>ӥ.ӳ()*3){Ǆ(3,5,ł,Ś);Ų=true;}else{у();ư("[DOCKING:APPROACH]");Ƽ(
);Ԃ=400;ų=true;}}else if(Ԃ==400){Echo("Moving to Launch1");Ә(Ç,ڂ,410,3,"DOCK Connector Entry");Ԃ=401;}else if(Ԃ==401){
Echo("Waiting for NAV to start");}else if(Ԃ==410){double ģ=(Ç-((IMyShipController)ɵ).CenterOfMass).LengthSquared();Echo(
"DistanceSQ="+ģ.ToString("0.0"));double ϭ=Ʀ(Ś,ơ,0);if(ģ>ӥ.ӳ()*3){Ǆ(3,5,ł,Ś);Ų=true;}else{у();Ƽ();Ԃ=430;ų=true;}}else if(Ԃ==430){ų=
true;Џ=-1;Ԃ=450;}else if(Ԃ==450||Ԃ==452){Ջ("Align Up to Docking Connector",դ);ų=true;if(!É){Ԃ=500;return;}Echo(
"Aligning to dock");bool Ͳ=false;ˉ=0.03f;Ͳ=ˇ("up",Ê,ɵ);ų=true;if(Ԃ==452)Ԃ=500;else if(Ͳ)Ԃ++;}else if(Ԃ==451){Ջ(
"Align to Docking Connector",դ);ų=true;Vector3D Ϭ=È;Vector3D ϫ=Ϭ-А.GetPosition();if(!É)Ԃ=452;Echo("Aligning to dock");bool Ͳ=false;ˉ=0.03f;Ͳ=ˇ(
"forward",ϫ,А);if(Ͳ)Ԃ=452;else ų=true;}else if(Ԃ==500){Ջ("Reversing to Docking Connector",դ);Echo("bDoDockAlign="+É);Echo(
"Reversing to Dock");ɮ=0.75;ˉ=0.01f;Vector3D Ϭ=È;Vector3D ϫ=Ϭ-А.GetPosition();double ɢ=ϫ.Length();Echo("distance="+ڊ(ɢ));Echo("velocity="+ơ
.ToString("0.00"));Ջ("Distance="+ڊ(ɢ),դ);Ջ("Velocity="+ڊ(ơ)+"/s",դ);if(Џ<0)Џ=ɢ;if(Џ<ɢ){Ԃ=590;}if(ɢ>10)ˉ=0.03f;else ˉ=
0.05f;bool Ͳ=false;if(ɢ>15)Ͳ=ͷ(Ç,È,А);else Ͳ=ˇ("forward",ϫ,А);if(Ͳ){Echo("Aimed");if(ɢ>15){Ų=true;Echo(">15");Ǆ(5,10,Ѝ,Ў);}
else{Echo("<=15");ų=true;Ǆ(.5f,1.5f,Ѝ,Ў);}}else{Echo("Aiming");Ǜ(Ƌ);ų=true;}}else if(Ԃ==590){у();Vector3D ϫ=È-А.GetPosition(
);double ɢ=ϫ.Length();if(ɢ>ӥ.Ӹ()*1.25){Ԃ=0;ų=true;return;}bool Ͳ=ˇ("forward",ϫ,А);if(!Ͳ)ų=true;else Ų=true;Ǆ(5,10,Ў,Ѝ);}}
void չ(){Echo("mode="+ڂ.ToString());յ();if(ڂ==ځ&&(Ӡ&Φ)>0)ط(ש);if(ڂ==ټ){Թ();return;}if(ڂ==ٴ){ڃ();return;}if(ڂ==ٹ){Ϩ();return;
}if(ڂ==ٸ){ڄ();return;}if(ڂ==ם){ն();return;}if(ڂ==ٻ){if(ӊ!=""){Echo("Going to "+ӊ);}}}void ո(){Ջ(DateTime.Now.ToString()+
" ACTION: Reset To Idle",է,true);у();ط(ځ);}void շ(){Ջ(ш+" Manual Control",դ);}void ն(){Echo("Launched. Awaiting commands");Ջ("clear",դ);Ջ(
"Launched. Defalt handling",դ);Ջ("Awaiting Commands",դ);ű=true;if(Ԃ==0){ư("[LAUNCHED]");Ԃ=1;}}void յ(){if(ڂ!=پ){float Ϧ=ħ()+100f+(float)ơ*15f;K(
false,Ϧ);}C();մ();if(Ā()&&(ڂ!=ټ)&&ڂ!=ٸ&&ڂ!=ٴ){Echo("Force to DOCKED");ط(ٸ);}ճ();ī();}void մ(){if(Ө!=""){Echo(
"Received Message=\n"+Ө);ŷ+="Received Message=\n"+Ө;if(ļ(Ө)){Ө="";return;}string[]ķ=Ө.Trim().Split(':');if(ķ.Length>1){if(ķ[0]!="WICO"){Echo(
"not wico system message");return;}if(ķ.Length>2){if(ķ[1]=="MOM"){}}}}else Echo("No pending incoming message");}void ճ(){string ũ;string ղ;double
Ը;string ձ=щ;ũ="Home";if(Ä){ղ="GPS:"+ձ+" Docking Entry:"+Ǡ(Ç)+":";Ջ(ղ,բ);}if(Å){ղ="GPS:"+ձ+" Dock:"+Ǡ(È)+":";Ջ(ղ,բ);}if(Û
){Ը=0;if(ɵ!=null)Ը=(ɵ.GetPosition()-Æ).Length();ũ+=": "+Ը.ToString("0")+"m";ղ="GPS:"+ձ+" Home Entry:"+Ǡ(Æ)+":";Ջ(ղ,բ);}
else ũ+=": NOT SET";if(ɵ!=null){ղ="GPS:"+ձ+" Current Position:"+Ǡ(ɵ.GetPosition())+":";Ջ(ղ,բ);}}List<IMyTerminalBlock>հ=new
List<IMyTerminalBlock>();string կ(){հ.Clear();հ=ɨ<IMyGasGenerator>();return"GG"+հ.Count.ToString("00");}void ծ(bool Ĉ=true){
ѧ(հ,Ĉ);}bool խ(){return true;}void լ(){if(ր()>99){ѧ(հ,false);}else{ѧ(հ,true);}}List<IMyTerminalBlock>ի=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>ժ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>պ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ջ=new List<IMyTerminalBlock>();const int א=1;const int ב=2;int և=0;int ֆ=0;double ǔ=-1;double օ=-1;void ք(){ǔ=ր(ב);օ=ր(
א);}bool փ(){return պ.Count>0;}string ւ(){{ի=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyGasTank>(
ի,(ǧ=>ǧ.CubeGrid==Me.CubeGrid));}և=0;ֆ=0;for(int w=0;w<ի.Count;++w){if(ռ(ի[w])==א){if(ի[w].CustomName.ToLower().Contains(
"isolated"))ջ.Add(ի[w]);else ժ.Add(ի[w]);ֆ++;}else if(ռ(ի[w])==ב){պ.Add(ի[w]);և++;}}return"T"+ի.Count.ToString("00");}double ր(
List<IMyTerminalBlock>ի){double ց=0;int վ=0;for(int w=0;w<ի.Count;++w){{IMyGasTank Ճ=ի[w]as IMyGasTank;if(Ճ==null)continue;
float ս=(float)Ճ.FilledRatio;ց+=ս;վ++;}}if(վ>0){return ց*100/վ;}else return 0;}double ր(int ţ=0xff){if(ի.Count<1)ւ();if(ի.
Count<1)return-1;double տ=0;int վ=0;for(int w=0;w<ի.Count;++w){int Մ=ռ(ի[w]);if((Մ&ţ)>0){IMyGasTank Ճ=ի[w]as IMyGasTank;if(Ճ
==null)continue;float ս=(float)Ճ.FilledRatio;տ+=ս;վ++;}}if(վ>0){return տ/վ;}else return-1;}int ռ(IMyTerminalBlock ŧ){if(ŧ
is IMyGasTank){if(ŧ.BlockDefinition.SubtypeId.Contains("Hydro"))return ב;else return א;}return 0;}void թ(bool ը=true,int ţ
=0xff){if(ի.Count<1)ւ();if(ի.Count<1)return;for(int w=0;w<ի.Count;++w){int Մ=ռ(ի[w]);if((Մ&ţ)>0){IMyGasTank Ճ=ի[w]as
IMyGasTank;if(Ճ==null)continue;Ճ.Stockpile=ը;}}}List<IMyTerminalBlock>Ղ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ձ=new
List<IMyTerminalBlock>();List<IMyTerminalBlock>Հ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Կ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>Ծ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Խ=new List<IMyTerminalBlock>();void Լ(ˣ Ë
){}void Ի(ˣ Ë){}void Ժ(ˣ Ë){}void Թ(){Ջ("clear",դ);Ջ(ш+":LAUNCH!",դ);Ų=true;if(Ԃ==0){Ջ(DateTime.Now.ToString()+
" ACTION: StartLaunch",է,true);Ջ(ш+":Start Launch",դ);ư("[LAUNCH]");if(!Ā()){Ջ("Can't perform action unless docked",է,true);у();ط(ځ);return;}
else{IMyTerminalBlock А=ý(true);ń(А,ref Ձ,ref Ղ,ref Խ,ref Ծ,ref Հ,ref Կ);}È=((IMyShipController)ɵ).CenterOfMass;թ(false);ғ()
;ē();Ǜ(Ƌ);float Ϧ=ħ()+100f+(float)ơ*5f;K(false,Ϧ);Ԃ=100;return;}if(ā()||Ā()){Ջ(ш+":Awaiting Disconnect",դ);Echo(
"Awaiting Disconnect");ù(false,false);return;}if(Ԃ==100){Ǌ(Ղ);Ԃ=1;}Vector3D Ŀ=((IMyShipController)ɵ).CenterOfMass;Echo("vDock="+Ǡ(È));Echo(
"vPos="+Ǡ(Ŀ));double Ը=(Ŀ-È).LengthSquared();Ջ(ш+":Distance Launched="+Ը.ToString("0.00")+"m",դ);Echo(ш+":Distance Launched="+Ը
.ToString("0.00")+"m");if(ơ>Ö*0.9){Ǜ(Ձ);Ǜ(Ղ,Ŋ,true);}else if(ơ>2){Ǌ(Ղ,25);}double Է=Ʀ(Ղ,ơ,0);if((Ը+Է)>Õ){ù(true,true);у()
;ط(ם);}}string Զ="LOGGING";void Ե(ˣ Ë){Ë.ȓ(Զ,"TextPanelReport",ref գ,true);Ë.ȓ(Զ,"StatusName",ref զ,true);Ë.ȓ(Զ,
"LongStatus",ref ե,true);Ë.ȓ(Զ,"RangeReport",ref Յ,true);Ë.ȓ(Զ,"SledReport",ref Ֆ,true);Ë.ȓ(Զ,"GPSTag",ref ա,true);}Փ Դ=null;string
Յ="[RANGE]";Փ Շ=null;string զ="Wico Craft Status";Փ է=null;string ե="Wico Craft Log";Փ դ=null;string գ="Craft Report";Փ բ
=null;string ա="[GPS]";Փ ՙ=null;string Ֆ="[SMREPORT]";bool Օ=false;bool Ք=false;class Փ{Program ʱ;string Ւ="";List<
IMyTextPanel>Ց=new List<IMyTextPanel>();string Ր="";string Տ="";bool Վ=false;bool Ս=true;public Փ(Program ʢ,string ĩ,bool Ռ=false){ʱ
=ʢ;Ւ=ĩ;Վ=Ռ;Ս=true;Ր="";Տ="";Ց.Clear();Ց=ʱ.ɹ(Ւ);if(Ց.Count<1)Ց=ʱ.ɯ(Ւ);}public void Ջ(string Պ,bool Չ=false){if(Պ=="clear")
{Ր="";Տ="X";Ս=false;return;}if(Վ&&Ս){Ս=false;if(Ց.Count>0){Ր=Ց[0].GetText();Տ="X";}}if(Չ){Ր=Պ+"\n"+Ր;}else Ր+=Պ+"\n";}
public void Ո(){if(Տ!=Ր){Ս=true;foreach(var Ն in Ց){Ն.WriteText(Ր);}Տ=Ր;}}}void ג(){Շ=ى(true);է=ٯ(ե,true);;դ=ٯ(գ);Դ=ٯ(Յ);բ=ٯ(ա
,Ċ);ՙ=ٯ(Ֆ);Օ=true;}void ؠ(){if(Շ!=null)Ջ("clear",Շ);if(է!=null)Ջ("clear",է);if(դ!=null)Ջ("clear",դ);if(Դ!=null)Ջ("clear",
Դ);if(բ!=null)Ջ("clear",բ);if(ՙ!=null)Ջ("clear",ՙ);}void ٲ(){if(Շ!=null)Շ.Ո();if(է!=null)է.Ո();if(դ!=null)դ.Ո();if(Դ!=
null)Դ.Ո();if(բ!=null)բ.Ո();if(ՙ!=null)ՙ.Ո();}Փ ٯ(string ٮ,bool Ռ=false){Փ ي=new Փ(this,ٮ,Ռ);return ي;}Փ ى(bool و=false){if(
(Շ!=null||Օ)&&!و)return Շ;Շ=ٯ(զ);return Շ;}void Ջ(string Պ,Փ ه,bool Չ=false){if(ه==null)return;ه.Ջ(Պ,Չ);}void ن(string Պ)
{Ջ(Պ,ى());if(Ք&&Պ!="clear")Echo(Պ);}string م(double ل){int ك=75;if(ل<0)ل=0;int ق=(int)(ل*ك)/100;if(ق>ك)ق=ك;string Ј="["+
new String('|',ق)+new String('\'',ك-ق)+"]";return Ј;}void ف(string ĩ,Vector3D Ĩ){string Į;Į="GPS:"+ĩ+":"+Ǡ(Ĩ)+":";Ջ(Į,բ);}
string ـ(string ؿ,string ؾ){string ũ;int ٱ=ؿ.Length;int ٳ=ؾ.Length;if(ٱ+ٳ>32){if(ٳ>31)return"INVALID";ٱ=32-ٳ;}ũ=ؿ.Substring(0,
ٱ)+ؾ;ũ.Replace(":","_");ũ.Replace(";","_");return ũ;}string ڊ(double ڋ){string ډ="";if(ڋ>1000){ډ=ڋ.ToString("N0")+"km";}
else if(ڋ>10){ډ=ڋ.ToString("0.0")+"m";}else{ډ=ڋ.ToString("0.000")+"m";}return ډ;}void ڈ(){}void ڇ(ˣ ƀ){ą(ƀ);Ņ(ƀ);ɱ(ƀ);љ(ƀ);ȁ
(ƀ);Ȇ(ƀ);ί(ƀ);ã(ƀ);Ӂ(ƀ);ò(ƀ);Î(ƀ);Լ(ƀ);}string چ(){do{Echo("Init:"+Ŷ.ToString());switch(Ŷ){case 0:ŷ+=ʅ();break;case 1:if(
!ӭ.ContainsKey("launch"))ӭ.Add("launch",ټ);if(!ӭ.ContainsKey("godock"))ӭ.Add("godock",ٹ);break;case 2:ג();Ջ(DateTime.Now.
ToString()+щ+":"+ш+":INIT",է,true);break;case 3:ŷ+=ӧ();Ђ();break;case 4:ŷ+=ɴ();break;case 5:ŷ+=ń(ɵ);break;case 6:ŷ+=ң();break;
case 7:ŷ+=Ȝ(ɵ);break;case 8:ŷ+=ѫ(ɵ);break;case 9:ŷ+=Ą();break;case 10:ŷ+=ʲ();ʕ=1;ˉ=0.09f;ɮ=0.75;break;case 11:ŷ+=ȏ();break;
case 12:Ӥ(ɵ);break;case 13:l();break;case 14:Ώ();break;case 15:ւ();break;case 16:ŷ+=څ();Ź=true;break;case 17:break;case 18:
break;case 19:break;}Ŷ++;}while(!Ź&&(((float)Runtime.CurrentInstructionCount/(float)Runtime.MaxInstructionCount)<0.5f));if(Ź)
Ŷ=0;ن(ŷ);Echo(ŷ);return ŷ;}string څ(){if(ڂ==ٹ){у();Ԃ=0;}return">";}void ڄ(){Ջ("clear",դ);Ջ(ш+":DOCKED!",դ);Echo("Docked!"
);Echo("Autorelaunch="+Ô.ToString());ų=false;Ų=false;ű=true;if(Ô){Echo("Docked. Checking Relaunch");if(î()){Echo(
"RELAUNCH!");ط(ٴ);ų=true;return;}else{Echo(" Awaiting Relaunch Criteria");Ջ("Awaiting Relaunch Criteria",դ);{Ջ(" Battery "+Ѹ+"% ("+
Ȋ+"%)",դ);Echo(" Battery "+Ѹ+"% ("+Ȋ+"%)");}{Ջ(" Cargo: "+ω+"% ("+ϊ+")",դ);Echo(" Cargo: "+ω+"% ("+ϊ+")");}if(փ()){Ջ(
" Hydro: "+ǔ+"% ("+ϊ+")",դ);Echo(" Hydro: "+ǔ+"% ("+ϊ+")");}}}if(!Ā()){ط(ځ);Ǜ(Ƌ);if((Ӡ&α)==0)թ(false);float Ϧ=ħ()+100f+(float)ơ*5f
;K(false,Ϧ);ғ();}else{Ջ(ш+":Power Saving Mode",դ);Echo("Power Saving Mode");if(Ԃ==0){if((Ӡ&α)==0)թ(true);Ǜ(Ƌ,Ŋ,true);R();
ɑ();ҝ(0,true);ư("[DOCKED]");Ԃ=1;}else if(Ԃ==1){ҝ(0,true);if(Ѹ<0||(Ӡ&Μ)==0)Ԃ=4;else if(!ҝ(30,true))Ԃ=2;}else if(Ԃ==2){if(!
ҝ(80,true))Ԃ=3;}else if(Ԃ==3){if(!ҝ(100,true))Ԃ=1;}else{ҝ(0,true);}{{Ύ();ք();}if(Ѹ>=0)Ջ("Bat:"+م(Ѹ),դ);else Echo(
"No Batteries");if(օ>=0){Ջ("O2:"+م(օ*100),դ);}else Echo("No Oxygen Tanks");if(ǔ>=0){Ջ("Hyd:"+م(ǔ*100),դ);if(ǔ<0.20f)Ջ(
" WARNING: Low Hydrogen Supplies",դ);Echo("H:"+(ǔ*100).ToString("000.0%"));}else Echo("No Hydrogen Tanks");if(Ѹ>=0&&Ѹ<ȉ)Ջ(" WARNING: Low Battery Power",դ
);}}}void ڃ(){Ջ("clear",դ);Ջ(ш+":RELAUNCH!",դ);Echo("Relaunch countdown in progress");if(Ԃ==0){Ջ(DateTime.Now.ToString()+
" ACTION: ReLaunch",է,true);if(!Ā()){Ջ("Can't perform action unless docked",է,true);у();ط(ځ);return;}ư("[RELAUNCH]");Ï=DateTime.Now;Ԃ=1;ԟ()
;return;}DateTime ϡ=Ï.AddSeconds(5.0f);DateTime Ϡ=DateTime.Now;if(DateTime.Compare(Ϡ,ϡ)>0){ط(ټ);}}int ڂ=-1;const int ځ=0;
const int ڀ=1;const int ٿ=2;const int پ=3;const int ٽ=4;const int ټ=5;const int ٻ=7;const int ٺ=8;const int ٹ=9;const int ٸ=
13;const int ٷ=10;const int ٶ=11;const int ٵ=12;const int ٴ=14;const int ؽ=15;const int ؼ=16;const int ד=17;const int ױ=18
;const int װ=19;const int ת=20;const int ש=21;const int ר=22;const int ק=23;const int צ=24;const int ץ=25;const int פ=26;
const int ף=27;const int ע=28;const int ס=29;const int נ=30;const int ן=31;const int מ=33;const int ם=50;const int ל=60;const
int כ=111;const int ך=112;const int י=200;const int ט=210;const int ח=220;const int ז=225;const int ו=290;const int ה=400;
const int ײ=410;const int ء=500;const int غ=510;const int ػ=600;const int ع=610;const int ظ=999;void ط(int ض){if(ڂ==ض)return;
ڂ=ض;Ԃ=0;Ʊ();}const string ص="WICOB_NAVADDTARGET";const string ش="WICOB_NAVSTART";const string س="WICOB_NAVRESET";const
string ز="WICO_NAVLAUNCH";const string ر="WICO_NAVDOCK";const string ذ="WICO_NAVORBITALLAUNCH";const string د="WICO_NAVLAND";
bool خ=false;public enum ح{ج,ث,ت,ة,ب,ا,ئ,إ,ؤ};class أ{public Program ʢ;public ح آ=ح.ج;public Vector3D ӑ;public bool Ӑ=false;
public int ӌ=נ;public int Ӌ=0;public double ӎ=9999;public double Ӎ=50;public string ӊ="";public bool ӓ(){switch(آ){case ح.ث:{ʢ
.ӑ=ӑ;ʢ.Ӑ=true;ʢ.ӌ=ӌ;ʢ.Ӌ=Ӌ;ʢ.Ӎ=Ӎ;ʢ.ӊ=ӊ;ʢ.ӎ=ӎ;ʢ.خ=true;ʢ.ŵ+="Going to:"+ʢ.ӊ;ʢ.ŵ+=" arrivald="+ʢ.Ӎ.ToString();ʢ.ط(ٻ);}break;
case ح.ت:{ʢ.ӑ=ӑ;ʢ.Ӑ=true;ʢ.ӌ=ӌ;ʢ.Ӌ=Ӌ;ʢ.ӊ=ӊ;ʢ.خ=false;ʢ.ط(ٻ);}break;case ح.ب:{ʢ.ӎ=ӎ;ʢ.ط(ع);}break;case ح.ة:{ʢ.Ӎ=Ӎ;ʢ.ط(ع);}
break;case ح.ا:{ʢ.ط(ټ);}break;case ح.ؤ:{ʢ.ط(ע);}break;case ح.إ:{ʢ.ط(ٹ);}break;case ح.ئ:{ʢ.ط(ס);}break;case ح.ج:{ʢ.Echo(
"Unknown Command");ʢ.ط(پ);return true;}}return false;}}List<أ>Ӓ=new List<أ>();Vector3D ӑ;bool Ӑ=false;DateTime ӏ;double ӎ=9999;double Ӎ=
50;int ӌ=נ;int Ӌ=0;string ӊ="";bool Ӊ=true;bool ӈ=true;bool Ӈ=true;bool ӆ=true;bool Ӆ=false;float ӄ=-1;bool Ӄ=false;string
ӂ="NAV";void Ӂ(ˣ Ë){Ë.ȓ(ӂ,"DTMDebug",ref Ӊ,true);Ë.ȓ(ӂ,"CameraCollision",ref ӈ,true);Ë.ȓ(ӂ,"SensorCollision",ref Ӈ,true);
Ë.ȓ(ӂ,"NAVEmulateOld",ref ӆ,true);Ë.ȓ(ӂ,"NAVGravityMinElevation",ref ӄ,true);Ë.ȓ(ӂ,"NavBeaconDebug",ref Ӄ,true);Ë.ȓ(ӂ,
"AllowBlindNav",ref Ӆ,true);if(ӎ>Ɔ)ӎ=Ɔ;}void Ӏ(ˣ Ë){Ë.ǵ(ӂ,"vTarget",ӑ);Ë.ǵ(ӂ,"ValidNavTarget",Ӑ);Ë.ǵ(ӂ,"TargetName",ӊ);Ë.ǵ(ӂ,
"dStartShip",ӏ);Ë.ǵ(ӂ,"shipSpeedMax",ӎ);Ë.ǵ(ӂ,"arrivalDistanceMin",Ӎ);Ë.ǵ(ӂ,"NAVArrivalMode",ӌ);Ë.ǵ(ӂ,"NAVArrivalState",Ӌ);}void Ӕ(ˣ
Ë){Ë.ȓ(ӂ,"vTarget",ref ӑ,true);Ë.ȓ(ӂ,"ValidNavTarget",ref Ӑ,true);Ë.ȓ(ӂ,"TargetName",ref ӊ,true);Ë.ȓ(ӂ,"dStartShip",ref ӏ
,true);Ë.ȓ(ӂ,"shipSpeedMax",ref ӎ,true);Ë.ȓ(ӂ,"arrivalDistanceMin",ref Ӎ,true);Ë.ȓ(ӂ,"NAVArrivalMode",ref ӌ,true);Ë.ȓ(ӂ,
"NAVArrivalState",ref Ӌ,true);}List<IMyBeacon>ӝ=new List<IMyBeacon>();void Ӟ(string Ӝ){if(Ӄ){if(ӝ.Count<1)GridTerminalSystem.
GetBlocksOfType(ӝ);foreach(var ӛ in ӝ){ӛ.CustomName=Ӝ;}}}void Ӛ(){IGC.SendBroadcastMessage(س,"",TransmissionDistance.CurrentConstruct);
}void ә(Vector3D Ү,int ҭ=ع,int Ҭ=0,double ҫ=50,string Ҫ="",double ҩ=9999,bool Ҩ=true){string ӗ=ӕ(Ү,ҭ,Ҭ,ҫ,Ҫ,ҩ,Ҩ);IGC.
SendBroadcastMessage(ص,ӗ,TransmissionDistance.CurrentConstruct);}void Ә(Vector3D Ү,int ҭ=נ,int Ҭ=0,double ҫ=50,string Ҫ="",double ҩ=9999,
bool Ҩ=true){string ӗ=ӕ(Ү,ҭ,Ҭ,ҫ,Ҫ,ҩ,Ҩ);IGC.SendBroadcastMessage(ش,ӗ,TransmissionDistance.CurrentConstruct);}void Ӗ(){IGC.
SendBroadcastMessage(ش,"",TransmissionDistance.CurrentConstruct);}string ӕ(Vector3D Ү,int ҭ=ع,int Ҭ=0,double ҫ=50,string Ҫ="",double ҩ=9999,
bool Ҩ=true){string ү="";ү+=Ǡ(Ү);ү+="\n";ү+=ҭ.ToString();ү+="\n";ү+=Ҭ.ToString();ү+="\n";ү+=ҫ.ToString();ү+="\n";ү+=Ҫ;ү+=
"\n";ү+=ҩ.ToString();ү+="\n";ү+=Ҩ.ToString();ү+="\n";return ү;}void Ұ(string ү,out Vector3D Ү,out int ҭ,out int Ҭ,out double
ҫ,out string Ҫ,out double ҩ,out bool Ҩ){ү=ү.Trim();string[]ұ=ү.Split('\n');string[]Ǥ=ұ[0].Split(',');if(Ǥ.Length<3){Ǥ=ұ[0
].Split(':');}double ǧ,Ǧ,ǥ;int ҧ=0;bool ǣ=double.TryParse(Ǥ[ҧ++].Trim(),out ǧ);bool Ǣ=double.TryParse(Ǥ[ҧ++].Trim(),out Ǧ
);bool ǡ=double.TryParse(Ǥ[ҧ++].Trim(),out ǥ);if(!ǣ||!Ǣ||!ǡ){Echo("Invalid Command:("+ұ[0]+")");}Ү=new Vector3D(ǧ,Ǧ,ǥ);
int.TryParse(ұ[1],out ҭ);int.TryParse(ұ[2],out Ҭ);double.TryParse(ұ[3],out ҫ);Ҫ=ұ[4];double.TryParse(ұ[5],out ҩ);Ҩ=true;if(
ұ.Length>5)bool.TryParse(ұ[6],out Ҩ);}List<IMyTerminalBlock>Ҧ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ҥ=new
List<IMyTerminalBlock>();List<IMyTerminalBlock>Ҥ=new List<IMyTerminalBlock>();string ң(){Ҧ.Clear();ҥ.Clear();Ҥ.Clear();
GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(Ҧ,ʌ);for(int w=0;w<Ҧ.Count;w++){if(Ҧ[w].CustomName.Contains("[LEFT]")||Ҧ[w].CustomData.
Contains("[LEFT]")){ҥ.Add(Ҧ[w]);}else if(Ҧ[w].CustomName.Contains("[RIGHT]")||Ҧ[w].CustomData.Contains("[RIGHT]")){Ҥ.Add(Ҧ[w]);}
}return"NR:L"+ҥ.Count.ToString("0")+"R"+Ҥ.Count.ToString("0");}bool Ң(float ȼ){if(ҥ.Count<1)return false;float Ҿ=ҥ[0].
GetMaximum<float>("Velocity");var ɽ=ҥ[0]as IMyMotorStator;float ҿ=ɽ.TargetVelocityRPM;float ȹ=(ҿ/Ҿ*100);ȹ=Math.Abs(ȹ);if(ȼ>(ȹ+5f))
ȼ=ȹ+5;if(ȼ<(ȹ-5))ȼ=ȹ-5;if(ȼ<0f)ȼ=0f;if(ȼ>100f)ȼ=100f;if(Math.Abs(ȼ)>0){Ң(ҥ,-ȼ);Ң(Ҥ,ȼ);return true;}else return false;}
bool Ң(List<IMyTerminalBlock>һ,float ȼ){for(int w=0;w<һ.Count;w++){var ɽ=һ[w]as IMyMotorStator;float Ҿ=ɽ.GetMaximum<float>(
"Velocity");if(!ɽ.Enabled)ɽ.Enabled=true;float ҽ=Ҿ*(ȼ/100.0f);ɽ.TargetVelocityRPM=ҽ;}return true;}bool Ҽ(){Ҽ(ҥ);Ҽ(Ҥ);return true;}
bool Ҽ(List<IMyTerminalBlock>һ){for(int w=0;w<һ.Count;w++){IMyMotorStator ɽ=һ[w]as IMyMotorStator;ɽ.TargetVelocityRPM=0;}
return true;}bool Һ(double Э){float ȼ;if(Math.Abs(Э)>1.0){ȼ=50;}else if(Math.Abs(Э)>.7){ȼ=50;}else if(Math.Abs(Э)>0.5){ȼ=30;}
else if(Math.Abs(Э)>0.1){ȼ=20;}else if(Math.Abs(Э)>0.01){ȼ=5;}else if(Math.Abs(Э)>0.001){ȼ=0;}else ȼ=0;ȼ/=3;ȼ=ȼ*-Math.Sign(Э
);if(Math.Abs(ȼ)>0){Ң(ҥ,ȼ);}if(Math.Abs(ȼ)>0){Ң(Ҥ,ȼ);}if(Math.Abs(ȼ)>0)return false;else return true;}Ҷ ҹ;void Ҹ(
IMyTerminalBlock ҷ=null){if(ҷ==null)ҷ=ɵ;if(ҷ==null)return;ҹ=new Ҷ(ҷ);}struct Ҷ{public Vector3D[]ҵ;Vector3D Ҵ;Vector3D ҳ;public Vector3D
ı;static int[]Ҳ={1,3,5,7};static int[]ӟ={0,2,4,6};static int[]ԗ={2,3,6,7};static int[]Ԗ={0,1,4,5};static int[]ԕ={4,5,6,7}
;static int[]Ԕ={0,1,2,3};static int[][]ԓ={Ҳ,ӟ,ԗ,Ԗ,ԕ,Ԕ};public const int Ԓ=0;public const int ԑ=1;public const int Ԑ=2;
public const int ԏ=3;public const int Ԏ=4;public const int ԍ=5;public Ҷ(IMyTerminalBlock Ǘ){ҵ=new Vector3D[8];if(Ǘ==null){ı=
new Vector3D();ҳ=new Vector3D();Ҵ=new Vector3D();return;}ҳ=new Vector3D(Ǘ.CubeGrid.Min)-new Vector3D(0.5,0.5,0.5);ҳ*=Ǘ.
CubeGrid.GridSize;Ҵ=new Vector3D(Ǘ.CubeGrid.Max)+new Vector3D(0.5,0.5,0.5);Ҵ*=Ǘ.CubeGrid.GridSize;var Ԍ=Ǘ.WorldMatrix.
GetOrientation();var ԋ=Ǘ.CubeGrid.WorldMatrix.GetOrientation()*MatrixD.Transpose(Ԍ);Vector3D.TransformNormal(ref ҳ,ref ԋ,out ҳ);
Vector3D.TransformNormal(ref Ҵ,ref ԋ,out Ҵ);var Ԋ=Vector3D.Min(ҳ,Ҵ);Ҵ=Vector3D.Max(ҳ,Ҵ);ҳ=Ԋ;var ԉ=Ǘ.CubeGrid.GetPosition();
Vector3D Ԉ;Vector3D ԇ;Ԉ=ҳ;Vector3D.TransformNormal(ref Ԉ,ref Ԍ,out Ԉ);Ԉ+=ԉ;ԇ=Ҵ;Vector3D.TransformNormal(ref ԇ,ref Ԍ,out ԇ);ԇ+=ԉ;
BoundingBox Ԇ=new BoundingBox(Ԉ,ԇ);ı=Ԇ.Center;Vector3D ԅ;for(int w=0;w<8;w++){ԅ.X=((w&1)==0?ҳ:Ҵ).X;ԅ.Y=((w&2)==0?ҳ:Ҵ).Y;ԅ.Z=((w&4)
==0?ҳ:Ҵ).Z;Vector3D.TransformNormal(ref ԅ,ref Ԍ,out ԅ);ԅ+=ԉ;ҵ[w]=ԅ;}}public void Ԅ(int ԃ,Vector3D[]ȭ,int ͻ=0){ԃ%=ԓ.Length;
for(int w=0;w<ԓ[ԃ].Length;w++){ȭ[ͻ++]=ҵ[ԓ[ԃ][w]];}}}bool Ԙ(string Ɲ){if(Ɲ==""||Ɲ=="timer"||Ɲ=="wccs"||Ɲ=="wcct"){if(Ӫ!=""&&
Ӫ!="timer"){Echo("Using Passed Arg="+Ӫ);Ɲ=Ӫ;}}if(Ɲ=="init"){ŷ="";Ź=false;Ŷ=0;چ();return false;}string[]Գ=Ɲ.Trim().Split(
' ');if(Գ[0]=="timer"){}else if(Գ[0]=="wccs"){}else if(Գ[0]=="wcct"){}else if(Գ[0]=="forgetmom"){}else{int Բ;if(ӭ.
TryGetValue(Գ[0].ToLower(),out Բ)){ط(Բ);}else{}}return false;}bool Ա(string Ɲ){return false;}bool ԧ(string Ɲ){ľ();return false;}
double Ԧ=-1;List<IMyTerminalBlock>ԥ=new List<IMyTerminalBlock>();void Ԥ(){ԥ.Clear();ɨ<IMyReactor>(ref ԥ);float ԡ;Ԣ(out ԡ);}
double ԣ(){double ż=0;foreach(var ҟ in ԥ){IMyReactor Ҟ=ҟ as IMyReactor;ż+=Ҟ.CurrentOutput;}return ż;}bool Ԣ(out float ԡ){ԡ=0;Ԧ
=-1;bool Ԡ=false;if(ԥ.Count>0)Ԧ=0;foreach(IMyReactor Ҟ in ԥ){ԡ+=Ҟ.CurrentOutput;Ԧ+=Ҟ.MaxOutput;}return Ԡ;}void ԟ(){if(ԙ==
null)return;с(ԙ);ԙ.ǵ(Ԝ,"Mode",ڂ.ToString());ԙ.ǵ(Ԝ,"current_state",Ԃ.ToString());ԙ.ǵ(Ԝ,"PassedArgument",Ӫ);ԙ.ǵ(Ԝ,
"AlertStates",ө.ToString());ԙ.ǵ(Ԝ,"craft_operation",Ӡ.ToString());ԙ.ǵ(Ԝ,"ReceivedMessage",Ө);long Ԟ=0;if(Ԛ!=null)Ԟ=Ԛ.EntityId;ԙ.ǵ(Ԝ,
"SaveID",(long)Ԟ);if(ԙ.ʣ){if(ԙ.ʣ){string ʠ=ԙ.ǯ();if(Ԛ==null){Echo("WARNING: saving to Storage");Storage=ʠ;}else{Ԛ.WriteText(ʠ,
false);}}}else{Echo("Not saving: Same");}}string ԝ="Wico Craft Save";string Ԝ="WCCM2";void ԛ(ˣ Ë){Ë.ȓ(Ԝ,"SAVE_FILE_NAME",ref
ԝ,true);}IMyTextPanel Ԛ=null;ˣ ԙ=null;int Ԃ=0;long ԁ=0;int Ӡ=Χ;string Ӫ="";int ө=0;string Ө="";string ӧ(){string ŷ="S";Ԛ=
null;List<IMyTerminalBlock>ĉ=new List<IMyTerminalBlock>();ĉ=ɶ<IMyTextPanel>(ԝ);if(ĉ.Count>1){Ŵ=true;ŵ+=
"\nMultiple blocks found:\""+ԝ+"\"";}else if(ĉ.Count==0){ĉ=ɧ<IMyTextPanel>(ԝ);if(ĉ.Count==1)Ԛ=ĉ[0]as IMyTextPanel;else{ĉ=ɷ<IMyTextPanel>(ԝ);if(ĉ.
Count==1)Ԛ=ĉ[0]as IMyTextPanel;}}else Ԛ=ĉ[0]as IMyTextPanel;ԙ=new ˣ(this,"");if(Ԛ==null){ŷ="-";}return ŷ;}bool Ӧ(){return Ԛ!=
null;}string Ǡ(Vector3D ǫ){string ũ;ũ=ǫ.X.ToString("0.00")+":"+ǫ.Y.ToString("0.00")+":"+ǫ.Z.ToString("0.00");return ũ;}bool
ǩ(string Ǩ,out double ǧ,out double Ǧ,out double ǥ){string[]Ǥ=Ǩ.Trim().Split(',');if(Ǥ.Length<3){Ǥ=Ǩ.Trim().Split(':');}ǧ=
0;Ǧ=0;ǥ=0;if(Ǥ.Length<3)return false;bool ǣ=double.TryParse(Ǥ[0].Trim(),out ǧ);bool Ǣ=double.TryParse(Ǥ[1].Trim(),out Ǧ);
bool ǡ=double.TryParse(Ǥ[2].Trim(),out ǥ);if(!ǣ||!Ǣ||!ǡ){return false;}return true;}Ӭ ӥ;void Ӥ(IMyTerminalBlock Ń){if(Ń==
null){Ń=(IMyTerminalBlock)Me;}ӥ=new Ӭ(this,Ń);}const float ӣ=0.5f;const float Ӣ=2.5f;const double ӡ=0.5;const double ӫ=2.5;
class Ӭ{private float ӿ,Ԁ,Ӿ;private double ӽ,Ӽ,ӻ;private double Ӻ;private Program ʱ;private Ҷ ҹ;public Ӭ(Program ʢ,
IMyTerminalBlock Ń){ʱ=ʢ;if(ʱ.Me.CubeGrid.GridSizeEnum.ToString().ToLower().Contains("small"))Ӻ=ӡ;else Ӻ=ӫ;ҹ=new Ҷ(Ń);Vector3D[]ȭ=new
Vector3D[4];ҹ.Ԅ(Ҷ.ԍ,ȭ);Ӽ=(ȭ[0]-ȭ[1]).Length();ӻ=(ȭ[0]-ȭ[2]).Length();ҹ.Ԅ(0,ȭ);ӽ=(ȭ[0]-ȭ[2]).Length();ӿ=(float)(ӽ/Ӻ);Ԁ=(float)(Ӽ/
Ӻ);Ӿ=(float)(ӻ/Ӻ);}public float ӹ(){return ӿ;}public double Ӹ(){return ӽ;}public float ӷ(){return Ԁ;}public double Ӷ(){
return Ӽ;}public float ӵ(){return Ӿ;}public double Ӵ(){return ӻ;}public double ӳ(){return Ӻ;}}List<IMyTerminalBlock>Ӳ=new List
<IMyTerminalBlock>();float ӱ=0;double Ӱ=-1;void ӯ(){Ӳ.Clear();Ӱ=-1;GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(Ӳ,ʌ)
;Ӯ();}void Ӯ(){if(Ӳ.Count>0)Ӱ=0;ӱ=0;foreach(var ҟ in Ӳ){IMySolarPanel Ҟ=ҟ as IMySolarPanel;Ӱ+=Ҟ.MaxOutput;ӱ+=Ҟ.
CurrentOutput;}}Dictionary<string,int>ӭ=new Dictionary<string,int>();string ƕ="";UpdateFrequency ͽ=UpdateFrequency.Once;bool Ċ=true;
bool Ƈ=false;float Ɔ=100;string ƅ="WORLD";void Ƅ(ˣ Ë){Ë.ȓ(ƅ,"MaxWorldMps",ref Ɔ,true);}string ƃ="WICOCRAFT";void Ƃ(bool Ɓ=
false){ˣ ƀ=new ˣ(this,Me.CustomData);ƀ.ȓ(ƃ,"EchoOn",ref ſ,true);ƀ.ȓ(ƃ,"DebugUpdate",ref Ƈ,true);Ƅ(ƀ);Č(ƀ);Ե(ƀ);ƴ(ƀ);ڇ(ƀ);if(ƀ
.ʣ||Ɓ){Me.CustomData=ƀ.ǯ(true);}}bool ſ=true;Action<string>ž;void Ž(string ż){if(ſ)ž(ż);}Program(){ڈ();Ƃ();ž=Echo;Echo=Ž;
ƕ=щ+":"+ш+" V"+ч+" ";ž(ƕ+"Creator");if(!Me.CustomName.Contains(ш))Me.CustomName="PB "+щ+" "+ш;if(!Me.Enabled){Echo(
"I am turned OFF!");}IMyTextSurface Ż=Me.GetSurface(0);IMyTextSurface ź=Me.GetSurface(1);Ż.ContentType=VRage.Game.GUI.TextPanel.
ContentType.TEXT_AND_IMAGE;Ż.WriteText("Wicorel\n"+ш);Ż.FontSize=2;Ż.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.CENTER;ź.
ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;ź.WriteText("Version:"+ч);ź.Alignment=VRage.Game.GUI.TextPanel.
TextAlignment.CENTER;ź.TextPadding=0.25f;ź.FontSize=3.5f;}bool Ź=false;bool Ÿ=false;string ŷ="";int Ŷ=0;string ŵ="";bool Ŵ=false;bool
ų=false;bool Ų=false;bool ű=false;bool ƈ=false;double Ɗ=5;double Ơ=-1;double ơ=-1;double Ɵ=-2;int ƞ=0;void Main(string Ɲ,
UpdateType Ɯ){ƞ++;Echo(ƕ+ƹ());if(Ƈ)Echo(Ɯ.ToString()+":"+ƞ.ToString());ų=false;Ų=false;ű=false;if(Ơ>Ɗ){Echo("Projector Check");Ơ=0
;ƈ=false;var ƛ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyProjector>(ƛ,ʌ);for(int w=0;w<ƛ.Count;w
++){if(ƛ[w].IsWorking){if(ƛ[w].CustomName.Contains("!WCC")||ƛ[w].CustomData.Contains("!WCC"))continue;Echo(
"Working local Projector found!");ƈ=true;}}}else{if(Ơ<0){Ơ=Ɗ+5;}Ơ+=Runtime.TimeSinceLastRun.TotalSeconds;}if(ƈ)Echo("Working local Projector found!");if
(Ɲ!=""&&Ɲ!="timer"&&Ɲ!="wccs")Echo("Arg="+Ɲ);if(Ɲ=="init"){ŷ="";Ź=false;}if(!Ź){if(ƈ){Ջ("clear",ٯ(գ));Ջ(ш+
":Construction in Progress\nTurn off projector to continue",դ);}ų=true;چ();if(Ŵ)ų=false;Ÿ=true;}else{if(Ÿ)Ջ(DateTime.Now.ToString()+" "+щ+":"+ŷ,է,true);if(ŵ!="")Echo(ŵ);Ђ();if(ɵ
is IMyShipController){ơ=((IMyShipController)ɵ).GetShipSpeed();Vector3D ƚ=((IMyShipController)ɵ).GetNaturalGravity();double
ƙ=ƚ.Length();Ɵ=ƙ/9.81;}if((Ɯ&(UpdateType.Trigger|UpdateType.Terminal))>0||(Ɯ&(UpdateType.Mod))>0||(Ɯ&(UpdateType.Script))
>0){if(Ƈ)Echo("Argument="+Ɲ);if(Ɲ.ToLower()=="profilerreset"){ƍ();ų=true;}if(Ԙ(Ɲ)){Ƙ();ԟ();ٲ();return;}}else if((Ɯ&(
UpdateType.IGC))>0){if(!Ա(Ɲ)){µ(Ɲ);}Ƙ();ԟ();ٲ();return;}else if((Ɯ&(UpdateType.IGC))>0){if(!ԧ(Ɲ)){}Ƙ();ԟ();ٲ();}{Ɲ="";if(Ŵ&&!Ÿ){Ź=
false;ŷ="";}}º();C();х();չ();}Ƙ();ф();ԟ();Ÿ=false;ٲ();}void Ƙ(){UpdateFrequency Ɨ=UpdateFrequency.None;if(ų){Echo("FAST!");Ɨ
|=ͽ;}else{}if(Ų){Echo("MEDIUM");Ɨ|=UpdateFrequency.Update10;}else{}if(ű){Echo("SLOW");Ɨ|=UpdateFrequency.Update100;}else{}
Runtime.UpdateFrequency=Ɨ;}void Ɩ(string ƕ=null){float Ɣ=0;Ɣ=Runtime.CurrentInstructionCount/(float)Runtime.MaxInstructionCount
;if(ƕ==null)ƕ="Instructions=";Echo(ƕ+" "+(Ɣ*100).ToString("0.00")+"%");}int Ɠ=1;int ƒ=20;bool Ƒ=false;StringBuilder Ɛ=new
StringBuilder();void Ə(){if(Ɠ<=ƒ){double Ǝ=Runtime.LastRunTimeMs;Echo("Profiler("+Ɠ+"):Add:"+Ǝ.ToString());Ɛ.Append(Ǝ.ToString()).
Append("\n");Ɠ++;}else if(!Ƒ){Echo("Profiler:DISPLAY");var ƌ=GridTerminalSystem.GetBlockWithName("DEBUG")as IMyTextPanel;ƌ?.
WriteText(Ɛ.ToString());Ƒ=true;}}void ƍ(){Ɠ=1;Ɛ=new StringBuilder();var ƌ=GridTerminalSystem.GetBlockWithName("DEBUG")as
IMyTextPanel;ƌ?.WriteText("");Ƒ=false;}List<IMyTerminalBlock>Ƌ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ł=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>Ś=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ŝ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>Ű=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ł=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ř=new List<
IMyTerminalBlock>();double Ř=0;double ŗ=0;double Ŗ=0;double ŕ=0;double Ŕ=0;double œ=0;int Œ=0;int ő=0;int Ő=0;int ŏ=0;const int Ŏ=1;
const int ō=2;const int Ō=4;const int ŋ=8;const int Ŋ=0xff;Matrix ŉ=new Matrix(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);string ň=
"IGNORE";string Ň="cutter";string ņ="THRUSTERS";void Ņ(ˣ Ë){Ë.ȓ(ņ,"IgnoreThruster",ref ň);Ë.ȓ(ņ,"CutterThruster",ref Ň);}void ń(
IMyTerminalBlock Ń,ref List<IMyTerminalBlock>ł,ref List<IMyTerminalBlock>Ś,ref List<IMyTerminalBlock>Ŝ,ref List<IMyTerminalBlock>Ű,ref
List<IMyTerminalBlock>Ł,ref List<IMyTerminalBlock>ř,int ů=Ŋ){ł.Clear();Ś.Clear();Ŝ.Clear();Ű.Clear();Ł.Clear();ř.Clear();Ƌ.
Clear();if(Ń==null)return;var Ů=new List<IMyTerminalBlock>();ɨ<IMyThrust>(ref Ů);for(int w=0;w<Ů.Count;w++){if(Ů[w].
CustomName.ToLower().Contains(Ň)||Ů[w].CustomData.ToLower().Contains(Ň))continue;if(Ů[w].CustomName.ToLower().Contains(ň)||Ů[w].
CustomData.ToLower().Contains(ň))continue;Ƌ.Add(Ů[w]);}Matrix ŭ;Ń.Orientation.GetMatrix(out ŭ);Matrix.Transpose(ref ŭ,out ŭ);Ř=0;ŗ
=0;Ŗ=0;ŕ=0;Ŕ=0;œ=0;for(int w=0;w<Ƌ.Count;++w){var ş=Ƌ[w]as IMyThrust;Matrix Ŭ;ş.Orientation.GetMatrix(out Ŭ);Vector3 ū=
Vector3.Transform(Ŭ.Backward,ŭ);int Ū=Ũ(Ƌ[w]);if(Ū==Ŏ)Ő++;else if(Ū==ō)ő++;else if(Ū==Ō)Œ++;else if(Ū==ŋ)ŏ++;if(ū==ŉ.Left){Ŕ+=Ŧ
((IMyThrust)Ƌ[w]);Ł.Add(Ƌ[w]);}else if(ū==ŉ.Right){œ+=Ŧ((IMyThrust)Ƌ[w]);ř.Add(Ƌ[w]);}else if(ū==ŉ.Backward){ŗ+=Ŧ((
IMyThrust)Ƌ[w]);Ś.Add(Ƌ[w]);}else if(ū==ŉ.Forward){Ř+=Ŧ((IMyThrust)Ƌ[w]);ł.Add(Ƌ[w]);}else if(ū==ŉ.Up){ŕ+=Ŧ((IMyThrust)Ƌ[w]);Ű.
Add(Ƌ[w]);}else if(ū==ŉ.Down){Ŗ+=Ŧ((IMyThrust)Ƌ[w]);Ŝ.Add(Ƌ[w]);}}}string ń(IMyTerminalBlock Ń){ł.Clear();Ś.Clear();Ŝ.Clear
();Ű.Clear();Ł.Clear();ř.Clear();Ƌ.Clear();if(Ń==null)return"No Orientation Block";ń(Ń,ref ł,ref Ś,ref Ŝ,ref Ű,ref Ł,ref
ř);string ũ;ũ=">";ũ+="F"+ł.Count.ToString("00");ũ+="B"+Ś.Count.ToString("00");ũ+="D"+Ŝ.Count.ToString("00");ũ+="U"+Ű.
Count.ToString("00");ũ+="L"+Ł.Count.ToString("00");ũ+="R"+ř.Count.ToString("00");ũ+="<";return ũ;}int Ũ(IMyTerminalBlock ŧ){
if(ŧ is IMyThrust){if(ŧ.BlockDefinition.SubtypeId.Contains("AtmosphericHover"))return ŋ;else if(ŧ.BlockDefinition.
SubtypeId.Contains("Atmo"))return Ŏ;else if(ŧ.BlockDefinition.SubtypeId.Contains("Hydro"))return ō;else if(ŧ.BlockDefinition.
SubtypeId.Contains("SmallBlock_HoverEngine"))return ŋ;else return Ō;}return 0;}double Ŧ(IMyThrust ş){return ş.MaxEffectiveThrust;
}double ť(List<IMyTerminalBlock>Ť,int ţ=Ŋ){double Ţ=0;for(int š=0;š<Ť.Count;š++){int Š=Ũ(Ť[š]);if((Š&ţ)>0){IMyThrust ş=Ť[
š]as IMyThrust;double Ş=ş.MaxEffectiveThrust;Ţ+=Ş;}}return Ţ;}double ś(List<IMyTerminalBlock>Ť,float Ư=5f,float ǚ=2f,
float Ǚ=1f){double ǘ=0;foreach(var Ǘ in Ť){var ş=Ǘ as IMyThrust;if(ş==null)continue;if(Ũ(ş)==Ŏ)ǘ+=ş.MaxEffectiveThrust*Ư;else
if(Ũ(ş)==Ō)ǘ+=ş.MaxEffectiveThrust*ǚ;else if(Ũ(ş)==ō)ǘ+=ş.MaxEffectiveThrust*Ǚ;else ǘ+=ş.MaxEffectiveThrust;}return ǘ;}
bool ǖ(List<IMyTerminalBlock>Ť,out float Ǖ,out float ǔ,out float Ǔ){Ǖ=0;ǔ=0;Ǔ=0;double ǒ=ť(Ť,Ō);double Ǒ=ť(Ť,Ŏ);double ǐ=ť(Ť
,ō);MyShipMass Ƣ;Ƣ=((IMyShipController)ɵ).CalculateShipMass();double Ʈ=0;Ʈ=Ƣ.PhysicalMass*Ɵ*9.810;if(Ǒ>0){if(Ǒ<Ʈ){Ǖ=100;Ʈ
-=Ǒ;}else{Ǖ=(float)(Ʈ/Ǒ*100);if(Ǖ>0)Ʈ-=(Ǒ*Ǖ/100);}}if(ǒ>0&&Ʈ>0){if(ǒ<Ʈ){Ǔ=100;Ʈ-=ǒ;}else{Ǔ=(float)(Ʈ/ǒ*100);if(Ǔ>0)Ʈ-=((ǒ*
Ǔ)/100);}}if(ǐ>0&&Ʈ>0){if(ǐ<Ʈ){ǔ=100;Ʈ-=ǐ;}else{ǔ=(float)(Ʈ/ǐ*100);if(ǔ>0)Ʈ-=((ǐ*ǔ)/100);;}}if(Ʈ>0)return false;return
true;}List<IMyTerminalBlock>Ǐ(string ǎ){var Ǎ=new List<IMyTerminalBlock>();var ǌ=new List<IMyBlockGroup>();
GridTerminalSystem.GetBlockGroups(ǌ);for(int ǋ=0;ǋ<ǌ.Count;ǋ++){if(ǌ[ǋ].Name==ǎ){List<IMyTerminalBlock>Ť=null;ǌ[ǋ].GetBlocks(Ť,ʌ);for(int
š=0;š<Ť.Count;š++){Ǎ.Add(Ť[š]);}break;}}return Ǎ;}int Ǌ(List<IMyTerminalBlock>Ť,float ǉ,int ţ=Ŋ){int g=0;if(ǉ>100)ǉ=100;
if(ǉ<0)ǉ=0;for(int š=0;š<Ť.Count;š++){int Š=Ũ(Ť[š]);if((Š&ţ)>0){IMyThrust ş=Ť[š]as IMyThrust;if(!ş.IsWorking){if(!ş.
Enabled)ş.Enabled=true;}g+=1;ş.ThrustOverridePercentage=ǉ/100f;}}return g;}int Ǌ(List<IMyTerminalBlock>Ť,int Ǟ=100,int ţ=Ŋ){
return Ǌ(Ť,(float)Ǟ,ţ);}bool Ǌ(string ǜ,int Ǟ=100,int ţ=Ŋ){if(Ǟ>100)Ǟ=100;var ǌ=new List<IMyBlockGroup>();GridTerminalSystem.
GetBlockGroups(ǌ);for(int ǋ=0;ǋ<ǌ.Count;ǋ++){if(ǌ[ǋ].Name==ǜ){List<IMyTerminalBlock>Ť=null;ǌ[ǋ].GetBlocks(Ť,ʌ);return(Ǌ(Ť,Ǟ,ţ)>0);}}
return false;}int Ǜ(List<IMyTerminalBlock>Ť,int ţ=Ŋ,bool ǝ=false){int g=0;for(int š=0;š<Ť.Count;š++){int Š=Ũ(Ť[š]);if((Š&ţ)>0)
{g++;IMyThrust ş=Ť[š]as IMyThrust;ş.ThrustOverride=0;if(ş.IsWorking&&ǝ&&ş.Enabled==true)ş.Enabled=false;else if(!ş.
IsWorking&&!ǝ&&ş.Enabled==false)ş.Enabled=true;}}return g;}bool Ǜ(string ǜ){var ǌ=new List<IMyBlockGroup>();GridTerminalSystem.
GetBlockGroups(ǌ);for(int ǋ=0;ǋ<ǌ.Count;ǋ++){if(ǌ[ǋ].Name==ǜ){List<IMyTerminalBlock>Ť=null;ǌ[ǋ].GetBlocks(Ť,ʌ);return(Ǜ(Ť)>0);}}return
false;}bool Ǌ(){return(Ǌ(ł)>0);}bool Ǜ(){return(Ǜ(ł)>0);}double ǈ(List<IMyTerminalBlock>ƫ,int ţ=Ŋ){for(int w=0;w<ƫ.Count;w++)
{int Š=Ũ(ƫ[w]);if((Š&ţ)>0&&ƫ[w].IsWorking){var ş=ƫ[w]as IMyThrust;return ş.ThrustOverride;}}return 0;}bool ƭ(List<
IMyTerminalBlock>ƫ,int ţ=Ŋ){for(int w=0;w<ƫ.Count;w++){int Š=Ũ(ƫ[w]);if((Š&ţ)>0&&ƫ[w].IsWorking){return true;}}return false;}int Ƭ(List<
IMyTerminalBlock>ƫ,int ţ=Ŋ){int g=0;for(int w=0;w<ƫ.Count;w++){int Š=Ũ(ƫ[w]);if((Š&ţ)>0&&ƫ[w].IsWorking){g++;}}return g;}IMyThrust ƪ(
List<IMyTerminalBlock>ƛ,int Ʃ=Ŋ){foreach(var Ţ in Ƌ){if(Ţ is IMyThrust&&(Ũ(Ţ)&Ʃ)>0)return Ţ as IMyThrust;}return null;}
double ƨ(){if(Ő<1)return 0;var Ƨ=ƪ(Ƌ,Ŏ);if(Ƨ==null)return 0;return Ƨ.MaxEffectiveThrust/Ƨ.MaxThrust;}double Ʀ(List<
IMyTerminalBlock>ƥ,double Ƥ,double ƣ){var Ƣ=((IMyShipController)ɵ).CalculateShipMass();double Ʈ=Ƣ.PhysicalMass*ƣ*9.810;double Ŧ=ť(ƥ);
double ƽ=(Ŧ-Ʈ)/Ƣ.PhysicalMass;double Ǉ=Ƥ/ƽ;double ǆ=Ƥ/2*Ǉ;return ǆ;}int ǅ=0;void Ǆ(float ǃ,float ǂ,List<IMyTerminalBlock>ǁ,
List<IMyTerminalBlock>ǀ){if(ǅ<0)ǅ=0;double Ŧ=ť(ǁ);MyShipMass Ƣ;Ƣ=((IMyShipController)ɵ).CalculateShipMass();double ƿ=Ƣ.
PhysicalMass;float ƾ=100f;if(ƿ>0){double ƽ=(Ŧ)/ƿ;if(ƽ>0)ƾ=(float)(ǃ/ƽ);}if(ơ>ǂ){Ǜ(Ƌ);}else if(ơ<(ǃ*0.90)){if(ơ<0.09)ǅ++;if(ơ<ǃ*0.25)
ǅ++;Ǌ(ǁ,ƾ+ǅ/5);}else if(ơ<(ǃ*1.1)){ǅ--;Ǜ(ǀ,Ŋ,true);Ǜ(ǁ);}else{ǅ--;ǅ--;Ǌ(ǁ,1f);}}void Ƽ(){ǅ=0;}string[]ƻ={"-","\\","|","/"
,"-","\\","|","/"};int ƺ=99;string ƹ(){ƺ++;if(ƺ>=ƻ.Length)ƺ=0;return ƻ[ƺ];}string Ƹ="[WCCT]";string Ʒ="[WCCS]";string ƶ=
"[WCCM]";string Ƶ="WICOTIMERS";void ƴ(ˣ Ë){Ë.ȓ(Ƶ,"FastTimer",ref Ƹ,true);Ë.ȓ(Ƶ,"SubModuleTimer",ref Ʒ,true);Ë.ȓ(Ƶ,"MainTimer",
ref ƶ,true);}Dictionary<string,List<IMyTerminalBlock>>Ƴ=new Dictionary<string,List<IMyTerminalBlock>>();void Ʋ(){Ƴ.Clear();
}void Ʊ(){if(!ư(Ƹ))ư(ƶ);}bool ư(string ŀ="[WCCS]"){bool Ü=false;List<IMyTerminalBlock>ĉ=new List<IMyTerminalBlock>();
IMyTimerBlock Ú=null;if(Ƴ.ContainsKey(ŀ)){ĉ=Ƴ[ŀ];}else{ĉ=ɧ<IMyTerminalBlock>(ŀ);Ƴ.Add(ŀ,ĉ);}for(int w=0;w<ĉ.Count;w++){Ú=ĉ[w]as
IMyTimerBlock;if(Ú!=null){if(Ú.Enabled){Ú.Trigger();Ü=true;}else{Echo("Timer:"+Ú.CustomName+" is OFF");}}}return Ü;}string Ù="DOCK";
bool Ø=true;double Ö=20;double Õ=45;bool Ô=false;bool Ó=false;Vector3D Ò;Vector3D Ñ;Vector3D Ð;DateTime Ï;void Î(ˣ Ë){Ë.ȓ(Ù,
"AllowStaticDocking",ref Ø,true);Ë.ȓ(Ù,"LaunchMaxVelocity",ref Ö,true);Ë.ȓ(Ù,"LaunchDistance",ref Õ,true);}void Í(ˣ Ë){Ë.ǵ(Ù,"AutoRelaunch",
Ô);Ë.ǵ(Ù,"ActionStart",Ï);Ë.ǵ(Ù,"StaticValid",Ó);Ë.ǵ(Ù,"StaticDock",Ò);Ë.ǵ(Ù,"StaticLaunch",Ñ);Ë.ǵ(Ù,"StaticHome",Ð);}
void Ì(ˣ Ë){Ë.ȓ(Ù,"AutoRelaunch",ref Ô,true);Ë.ȓ(Ù,"ActionStart",ref Ï);Ë.ȓ(Ù,"StaticValid",ref Ó);Ë.ȓ(Ù,"StaticDock",ref Ò)
;Ë.ȓ(Ù,"StaticLaunch",ref Ñ);Ë.ȓ(Ù,"StaticHome",ref Ð);}Vector3D Ê;bool É=false;Vector3D È;Vector3D Ç;Vector3D Æ;bool Å=
false;bool Ä=false;bool Û=false;long Ý=0;DateTime ó;string ô="DOCKING";void ò(ˣ Ë){}void ñ(ˣ Ë){Ë.ǵ(ô,"vDock",È);Ë.ǵ(ô,
"ValidDock",Å);Ë.ǵ(ô,"vLaunch1",Ç);Ë.ǵ(ô,"bValidLaunch1",Ä);Ë.ǵ(ô,"vHome",Æ);Ë.ǵ(ô,"bValidHome",Û);Ë.ǵ(ô,"TargetBase",Ý);Ë.ǵ(ô,
"ActionStart",ó);}void ð(ˣ Ë){Ë.ȓ(ô,"vDock",ref È,true);Ë.ȓ(ô,"ValidDock",ref Å,true);Ë.ȓ(ô,"vLaunch1",ref Ç,true);Ë.ȓ(ô,
"bValidLaunch1",ref Ä,true);Ë.ȓ(ô,"vHome",ref Æ,true);Ë.ȓ(ô,"bValidHome",ref Û,true);Ë.ȓ(ô,"TargetBase",ref Ý,true);Ë.ȓ(ô,"ActionStart"
,ref ó);}double ï=-1;bool î(bool í=false,bool ì=true,int ë=1){bool ê=true;bool é=true;bool è=true;bool ç=true;if(ï>=0)ï+=
Runtime.TimeSinceLastRun.TotalMilliseconds;bool æ=í;if(ï<0||ï>0.5*1000){ï=0;æ=true;}if(æ)ҝ(0,false);if(ì){if(Ѹ>=0&&Ѹ<Ȋ){ê=false
;}}else{if(Ѹ>=0&&Ѹ<ȉ){ê=false;}}if(æ)Ύ();if(ì){if(ω>ϊ){ç=false;}}else{if(ω>ë){ç=false;}}if(æ)ք();if(ì){if(փ()&&ǔ*100<70)é
=false;}else{if(փ()&&ǔ*100<30)é=false;}if(ê&&é&&è&&ç){return true;}else return false;}bool å=false;string ä=
"COMMUNICATIONS";void ã(ˣ Ë){Ë.ȓ(ä,"CommunicationsStealth",ref å,false);}bool â=false;List<IMyRadioAntenna>á=new List<IMyRadioAntenna>()
;List<IMyLaserAntenna>à=new List<IMyLaserAntenna>();string ß(){á.Clear();à.Clear();ɨ<IMyRadioAntenna>(ref á);ɨ<
IMyLaserAntenna>(ref à);for(int j=0;j<á.Count;++j){if(á[j].CustomName.Contains("unused")||á[j].CustomData.Contains("unused"))continue;
if(!â){щ="Wico "+á[j].CustomName.Split('!')[0].Trim();â=true;}}return"A"+á.Count.ToString("0");}void Þ(){for(int w=0;w<á.
Count;w++){á[w].Enabled=true;}}string W="";void Á(){if(Ө!=""){if(W==Ө){Ө="";}W=Ө;}else W="";}void U(){}bool S(){return true;}
void R(bool H=false){if(á.Count<1)ß();foreach(var G in á){G.Radius=200;}}void Q(float P=200,bool H=false){if(á.Count<1)ß();
foreach(var N in á){{N.Radius=P;N.Enabled=true;}}}Vector3D O(){if(á.Count<1)ß();foreach(var N in á){return N.GetPosition();}
Vector3D V=new Vector3D();return V;}float M=float.MaxValue;void K(bool H=false,float J=float.MaxValue){if(J<200)J=200;M=J;I(H);}
void I(bool H=false){if(á==null||á.Count<1)ß();foreach(var G in á){{float F=G.GetMaximum<float>("Radius");if(M<F)F=M;G.
Radius=F;G.Enabled=true;}}}int E(){if(á.Count<1)ß();return(á.Count);}List<string>D=new List<string>();void C(){}void B(string
L,string A){IGC.SendBroadcastMessage(L,A);}void B(long Â,string L,string A){IGC.SendUnicastMessage(Â,L,A);}List<string>À=
new List<string>();void º(){if(À.Count>0){if(Ө==""){Ө=À[0];À.RemoveAt(0);}else Echo("Waiting for message to be processed");
Ʊ();}if(À.Count>0){}}void µ(string A){Echo("RECEIVE:\n"+A);À.Add(A);º();}void ª(){if(á.Count>0){Echo(À.Count+
" Pending Incoming Messages");for(int w=0;w<À.Count;w++)Echo(w+":"+À[w]);}else Echo("No antennas found");}List<p>u=new List<p>();const string q=
"BASE1.0";class p{public long o;public string n;public Vector3D Z;public bool Y;}IMyBroadcastListener m;void l(){u.Clear();h();m=
IGC.RegisterBroadcastListener("BASE");m.SetMessageCallback(m.Tag);}void k(){if(ԙ==null)return;ԙ.ǵ(q,"count",u.Count);for(
int j=0;j<u.Count;j++){ԙ.ǵ(q,"ID"+j.ToString(),u[j].o);ԙ.ǵ(q,"name"+j.ToString(),u[j].n);ԙ.ǵ(q,"position"+j.ToString(),u[j]
.Z);ԙ.ǵ(q,"Jumpable"+j.ToString(),u[j].Y);}}int h(){if(ԙ==null){return-2;}int g=-1;long e=0;string d="";Vector3D Z=new
Vector3D();bool Y=false;ԙ.ȓ(q,"count",ref g);for(int Ã=0;Ã<g;Ã++){ԙ.ȓ(q,"ID"+Ã.ToString(),ref e);ԙ.ȓ(q,"name"+Ã.ToString(),ref d
);ԙ.ȓ(q,"position"+Ã.ToString(),ref Z);ԙ.ȓ(q,"Jumpable"+Ã.ToString(),ref Y);p X=new p{o=e,n=d,Z=Z,Y=Y};u.Add(X);}return g
;}void õ(long o,string n,Vector3D ı,bool Y=false){if(u.Count<1)l();p İ=new p{o=o,n=n,Z=ı,Y=Y};for(int w=0;w<u.Count;w++){
if(u[w].o==o){u[w].n=n;u[w].Z=ı;u[w].Y=Y;return;}}u.Add(İ);k();}string į(){string Į;if(u.Count==0)return"No Known Bases";
if(u.Count>1)Į=u.Count.ToString()+" Known Bases\n";else Į=u.Count.ToString()+" Known Base\n";for(int w=0;w<u.Count;w++){Į
+=u[w].n+":";Į+=Ǡ(u[w].Z)+":";Į+="\n";}return Į;}double ĭ=25;double Ĭ=-1;void ī(bool Ī=false){string ĩ=Me.CubeGrid.
CustomName;Vector3D Ĩ=Me.GetPosition();if(Ī){u.Clear();k();}if(ɵ!=null){ĩ=ɵ.CubeGrid.CustomName;Ĩ=ɵ.GetPosition();}if(Ĭ>ĭ||Ī){Ĭ=0;
B("BASE?",ĩ+":"+Ԛ.EntityId.ToString()+":"+Ǡ(Ĩ));}else{if(Ĭ<0){Ĭ=Me.EntityId%ĭ;}if(u.Count<1)Ĭ+=Runtime.TimeSinceLastRun.
TotalSeconds;}}float ħ(){double Ħ=double.MaxValue;int Ĥ=Ġ(ĥ());if(Ĥ>=0&&ɵ!=null){Ħ=(ɵ.GetPosition()-u[Ĥ].Z).Length();}return(float)Ħ
;}long ĥ(){int Ĥ=-1;if(ɵ==null)return Ĥ;double ģ=double.MaxValue;for(int w=0;w<u.Count;w++){double Ģ=Vector3D.
DistanceSquared(u[w].Z,ɵ.GetPosition());if(Ģ<ģ){Ĥ=w;ģ=Ģ;}}if(Ĥ<0)return 0;else return u[Ĥ].o;}long ġ(){return ĥ();}int Ġ(long ğ){for(
int j=0;j<u.Count;j++){if(u[j].o==ğ)return j;}return-1;}Vector3D Ĳ(long o){Vector3D Ŀ=new Vector3D();for(int j=0;j<u.Count;
j++)if(u[j].o==o)return u[j].Z;return Ŀ;}bool ľ(){if(!m.HasPendingMessage)return false;Echo("Base Response");var Ľ=m.
AcceptMessage();string Ļ=(string)Ľ.Data;string[]ķ=Ļ.Trim().Split(':');double ĺ,Ĺ,ĸ;int Ķ=0;string ĩ=ķ[Ķ++];long ĵ=0;long.TryParse(ķ[Ķ
++],out ĵ);ĺ=Convert.ToDouble(ķ[Ķ++]);Ĺ=Convert.ToDouble(ķ[Ķ++]);ĸ=Convert.ToDouble(ķ[Ķ++]);Vector3D Ĩ=new Vector3D(ĺ,Ĺ,ĸ)
;bool Y=Ͽ(ķ[Ķ++]);õ(ĵ,ĩ,Ĩ,Y);return false;}bool ļ(string Ļ){double ĺ,Ĺ,ĸ;string[]ķ=Ļ.Trim().Split(':');if(ķ.Length>1){if(
ķ[0]!="WICO"){Echo("not wico system message");return false;}if(ķ.Length>2){if(ķ[1]=="BASE"){int Ķ=2;string ĩ=ķ[Ķ++];long
ĵ=0;long.TryParse(ķ[Ķ++],out ĵ);ĺ=Convert.ToDouble(ķ[Ķ++]);Ĺ=Convert.ToDouble(ķ[Ķ++]);ĸ=Convert.ToDouble(ķ[Ķ++]);Vector3D
Ĩ=new Vector3D(ĺ,Ĺ,ĸ);bool Y=Ͽ(ķ[Ķ++]);õ(ĵ,ĩ,Ĩ,Y);return true;}}}return false;}List<IMyTerminalBlock>Ĵ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>ĳ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ğ=new List<IMyTerminalBlock>();bool ĝ=
false;string ö="[BASE]";string ć="[DOCK]";string Ć="CONNECTORS";void ą(ˣ Ë){Ë.ȓ(Ć,"BaseConnector",ref ö,true);Ë.ȓ(Ć,
"DockConnector",ref ć,true);}string Ą(){ĝ=false;Ĵ.Clear();ĳ.Clear();Ğ.Clear();ă();return"CL"+Ĵ.Count.ToString()+"CD"+ĳ.Count.ToString()
+"CB"+Ğ.Count.ToString();}void ă(){if(Ĵ.Count<1&&!ĝ)Ĵ=ɨ<IMyShipConnector>();if(ĳ.Count<1&&!ĝ)ĳ=ɧ<IMyShipConnector>(ć);if(
ĳ.Count<1&&!ĝ)ĳ=Ĵ;if(Ğ.Count<1&&!ĝ)Ğ=ɧ<IMyShipConnector>(ö);ĝ=true;return;}bool Ă(){return ĳ.Count>1;}bool ā(){ă();for(
int w=0;w<ĳ.Count;w++){var û=ĳ[w]as IMyShipConnector;if(û==null)continue;if(û.Status==MyShipConnectorStatus.Connectable)
return true;}return false;}bool Ā(){ă();for(int w=0;w<ĳ.Count;w++){var û=ĳ[w]as IMyShipConnector;if(û==null)continue;if(û.
Status==MyShipConnectorStatus.Connected){var ú=û.OtherConnector;if(ú.CubeGrid==û.CubeGrid){continue;}else return true;}}return
false;}void ÿ(){for(int w=0;w<ĳ.Count;w++){var û=ĳ[w]as IMyShipConnector;if(û==null)continue;}}IMyTerminalBlock þ(){ă();if(ĳ.
Count>0){return ĳ[0];}return null;}IMyTerminalBlock ý(bool ü=false){ă();for(int w=0;w<ĳ.Count;w++){var û=ĳ[w]as
IMyShipConnector;if(û==null)continue;if(û.Status==MyShipConnectorStatus.Connected){var ú=û.OtherConnector;if(ú.CubeGrid==û.CubeGrid){
continue;}else{if(!ü){return û.OtherConnector;}else{return ĳ[w];}}}}return null;}void ù(bool ø=true,bool Ĉ=true){ă();for(int w=0
;w<ĳ.Count;w++){var û=ĳ[w]as IMyShipConnector;if(û==null)continue;if(û.Status==MyShipConnectorStatus.Connected){var ú=û.
OtherConnector;if(ú.CubeGrid==û.CubeGrid){continue;}}if(ø){if(û.Status==MyShipConnectorStatus.Connectable)û.Connect();}else{if(û.
Status==MyShipConnectorStatus.Connected)û.Disconnect();}û.Enabled=Ĉ;}return;}List<IMyTerminalBlock>Ĝ=new List<IMyTerminalBlock
>();string ě(){List<IMyTerminalBlock>ĕ=new List<IMyTerminalBlock>();Ĝ.Clear();ĕ=ɨ<IMyShipDrill>();foreach(var Ē in ĕ)Ĝ.
Add(Ē as IMyTerminalBlock);return"D"+Ĝ.Count.ToString("00");}void Ě(){foreach(IMyFunctionalBlock Ē in Ĝ){Ē.Enabled=true;}}
void ę(){if(Ĝ.Count<1)ě();foreach(IMyFunctionalBlock Ē in Ĝ){Ē.Enabled=false;}}bool Ę(){if(Ĝ.Count<1)ě();if(Ĝ.Count<1)return
false;return true;}List<IMyTerminalBlock>ė=new List<IMyTerminalBlock>();string Ė(){List<IMyTerminalBlock>ĕ=new List<
IMyTerminalBlock>();ė.Clear();ĕ=ɧ<IMyShipConnector>("Ejector");foreach(var Ē in ĕ)ė.Add(Ē as IMyTerminalBlock);return"E"+ė.Count.
ToString("00");}void Ĕ(){if(ė.Count<1)Ė();foreach(IMyFunctionalBlock Ē in ė){if(!Ē.Enabled)Ē.Enabled=true;}}void ē(){if(ė.Count<
1)Ė();foreach(IMyFunctionalBlock Ē in ė){if(Ē.Enabled)Ē.Enabled=false;}}string đ="NOFOLLOW";string Đ="!WCC";string ď=
"[NAV]";string Ď="Craft Remote Control";string č="GRIDS";void Č(ˣ Ë){Ë.ȓ(č,"NoFollow",ref đ,true);Ë.ȓ(č,"BlockIgnore",ref Đ,
true);Ë.ȓ(č,"OrientationBlockContains",ref ď,true);Ë.ȓ(č,"OrientationBlockNamed",ref Ď,true);}List<IMyTerminalBlock>ċ=new
List<IMyTerminalBlock>();List<IMyTextPanel>ŝ=new List<IMyTextPanel>();List<IMyTextPanel>ǟ=new List<IMyTextPanel>();List<
IMyTerminalBlock>ɰ=new List<IMyTerminalBlock>();List<IMyCubeGrid>ʋ=new List<IMyCubeGrid>();List<IMyCubeGrid>ʊ=new List<IMyCubeGrid>();
List<IMyCubeGrid>ʉ=new List<IMyCubeGrid>();List<IMyCubeGrid>ʈ=new List<IMyCubeGrid>();bool ʇ(){List<IMyTerminalBlock>ʆ=new
List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(ʆ);if(ԁ!=ʆ.Count){return true;}return false;}
string ʅ(){ċ.Clear();ʈ.Clear();ʋ.Clear();ʊ.Clear();ʉ.Clear();ŝ.Clear();ǟ.Clear();ɰ.Clear();GridTerminalSystem.GetBlocksOfType<
IMyTerminalBlock>(ċ);ԁ=ċ.Count;foreach(var Ǘ in ċ){var ɿ=Ǘ.CubeGrid;if(!ʈ.Contains(ɿ)){ʈ.Add(ɿ);}}ʁ(Me.CubeGrid);foreach(var ɿ in ʈ){if(
ʋ.Contains(ɿ))continue;bool ʄ=false;List<IMyShipConnector>ʃ=new List<IMyShipConnector>();GridTerminalSystem.
GetBlocksOfType<IMyShipConnector>(ʃ,(ĺ=>ĺ.CubeGrid==ɿ));foreach(var ʂ in ʃ){if(ʂ.Status==MyShipConnectorStatus.Connected){if(ʋ.Contains
(ʂ.OtherConnector.CubeGrid)||ʊ.Contains(ʂ.OtherConnector.CubeGrid)){continue;}if(ʋ.Contains(ʂ.OtherConnector.CubeGrid))ʄ=
true;else ʄ=false;}}if(ʄ){if(!ʉ.Contains(ɿ)){ʉ.Add(ɿ);}}if(!ʊ.Contains(ɿ)){ʊ.Add(ɿ);}}string ũ="";ũ+="B"+ċ.Count.ToString();
ũ+="G"+ʈ.Count.ToString();ũ+="L"+ʋ.Count.ToString();ũ+="D"+ʉ.Count.ToString();ũ+="R"+ʊ.Count.ToString();Echo("Found "+ʈ.
Count.ToString()+" Grids");Echo("Found "+ʋ.Count.ToString()+" Local Grids");for(int w=0;w<ʋ.Count;w++)Echo("|"+ʋ[w].
CustomName);Echo("Found "+ʉ.Count.ToString()+" Docked Grids");for(int w=0;w<ʉ.Count;w++)Echo("|"+ʉ[w].CustomName);Echo("Found "+ʊ.
Count.ToString()+" Remote Grids");for(int w=0;w<ʊ.Count;w++)Echo("|"+ʊ[w].CustomName);return ũ;}void ʁ(IMyCubeGrid ɿ){if(ɿ==
null)return;if(!ʋ.Contains(ɿ)){ʋ.Add(ɿ);ʀ(ɿ);ʔ(ɿ);ʓ(ɿ);ʒ(ɿ);}}void ʀ(IMyCubeGrid ɿ){List<IMyMotorStator>ɾ=new List<
IMyMotorStator>();GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(ɾ,(ǧ=>ǧ.TopGrid==ɿ));foreach(var ɽ in ɾ){if(ɽ.CustomName.Contains
(đ)||ɽ.CustomData.Contains(đ))continue;ʁ(ɽ.CubeGrid);}List<IMyMotorAdvancedStator>ɼ=new List<IMyMotorAdvancedStator>();
GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(ɼ,(ǧ=>ǧ.TopGrid==ɿ));foreach(var ɽ in ɼ){if(ɽ.CustomName.Contains(đ)||ɽ.
CustomData.Contains(đ))continue;ʁ(ɽ.CubeGrid);}}void ʔ(IMyCubeGrid ɿ){List<IMyPistonBase>ʑ=new List<IMyPistonBase>();
GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(ʑ,(ǧ=>ǧ.TopGrid==ɿ));foreach(var ʐ in ʑ){ʁ(ʐ.CubeGrid);}}void ʓ(IMyCubeGrid ɿ){List<
IMyMotorStator>ɾ=new List<IMyMotorStator>();GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(ɾ,(ĺ=>ĺ.CubeGrid==ɿ));foreach(var ɽ in
ɾ){if(ɽ.CustomName.Contains(đ)||ɽ.CustomData.Contains(đ))continue;IMyCubeGrid ʏ=ɽ.TopGrid;if(ʏ!=null&&ʏ!=ɿ){ʁ(ʏ);}}ɾ.
Clear();List<IMyMotorAdvancedStator>ɼ=new List<IMyMotorAdvancedStator>();GridTerminalSystem.GetBlocksOfType<
IMyMotorAdvancedStator>(ɼ,(ĺ=>ĺ.CubeGrid==ɿ));foreach(var ɽ in ɼ){if(ɽ.CustomName.Contains(đ)||ɽ.CustomData.Contains(đ))continue;IMyCubeGrid ʏ
=ɽ.TopGrid;if(ʏ!=null&&ʏ!=ɿ){ʁ(ʏ);}}}void ʒ(IMyCubeGrid ɿ){List<IMyPistonBase>ʑ=new List<IMyPistonBase>();
GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(ʑ,(ĺ=>ĺ.CubeGrid==ɿ));foreach(var ʐ in ʑ){IMyCubeGrid ʏ=ʐ.TopGrid;if(ʏ!=null&&ʏ!=ɿ){if(!
ʋ.Contains(ʏ)){ʁ(ʏ);}}}}List<IMyCubeGrid>ʎ(){if(ʋ.Count<1){ʅ();}return ʋ;}List<IMyCubeGrid>ʍ(){if(ʋ.Count<1){ʅ();}return
ʉ;}bool ʌ(IMyTerminalBlock Ǘ){return ʎ().Contains(Ǘ.CubeGrid);}bool ɻ(long ɺ){for(int j=0;j<ʋ.Count;j++){if((long)ʋ[j].
EntityId==ɺ)return true;}return false;}bool ɻ(IMyCubeGrid ɺ){return ʎ().Contains(ɺ);}bool ɤ(IMyTerminalBlock Ǘ){var ɭ=ʍ();if(ɭ==
null)return false;return ɭ.Contains(Ǘ.CubeGrid);}void ɬ(){if(ċ.Count<1)ʅ();ɰ.Clear();foreach(var X in ċ){if(ʌ(X)&&!(X.
CustomName.Contains(Đ)))ɰ.Add(X);}}IMyTerminalBlock ɫ(string ɪ){IMyTerminalBlock Ǘ;Ǘ=(IMyTerminalBlock)GridTerminalSystem.
GetBlockWithName(ɪ);if(Ǘ==null)throw new Exception(ɪ+" Not Found");return Ǘ;}List<ɦ>ɨ<ɦ>(ref List<ɦ>ĕ,string ɥ=null)where ɦ:class{if(ĕ==
null)ĕ=new List<ɦ>();else ĕ.Clear();if(ɰ.Count<1)ɬ();for(int ɩ=0;ɩ<ɰ.Count;ɩ++){if(ɰ[ɩ]is ɦ&&((ɥ==null)||(ɥ!=null&&ɰ[ɩ].
CustomName.StartsWith(ɥ)))){ĕ.Add((ɦ)ɰ[ɩ]);}}return ĕ;}List<IMyTerminalBlock>ɨ<ɦ>(ref List<IMyTerminalBlock>ĕ,string ɥ=null)where
ɦ:class{if(ċ.Count<1)ʅ();if(ĕ==null)ĕ=new List<IMyTerminalBlock>();else ĕ.Clear();if(ɰ.Count<1)ɬ();for(int ɩ=0;ɩ<ɰ.Count;
ɩ++){if(ɰ[ɩ]is ɦ&&((ɥ==null)||(ɥ!=null&&ɰ[ɩ].CustomName.StartsWith(ɥ)))){ĕ.Add(ɰ[ɩ]);}}return ĕ;}List<IMyTerminalBlock>ɨ<
ɦ>(string ɥ=null)where ɦ:class{var ĕ=new List<IMyTerminalBlock>();ɨ<ɦ>(ref ĕ,ɥ);return ĕ;}List<IMyTerminalBlock>ɧ<ɦ>(
string ɥ=null)where ɦ:class{var ĕ=new List<IMyTerminalBlock>();if(ɰ.Count<1)ɬ();for(int ɩ=0;ɩ<ɰ.Count;ɩ++){if(ɰ[ɩ]is ɦ&&ɥ!=
null&&(ɰ[ɩ].CustomName.Contains(ɥ)||ɰ[ɩ].CustomData.Contains(ɥ))){ĕ.Add(ɰ[ɩ]);}}return ĕ;}List<IMyTextPanel>ɯ(string ɥ=null)
{if(ċ.Count<1)ʅ();var ĕ=new List<IMyTextPanel>();if(ŝ.Count>1){foreach(var ɸ in ŝ){if(ɥ!=null&&(ɸ.CustomName.Contains(ɥ)
||ɸ.CustomData.Contains(ɥ)))ĕ.Add(ɸ);}}else{foreach(var ɸ in ċ){if(ɸ is IMyTextPanel&&ʌ(ɸ)&&!(ɸ.CustomName.Contains(Đ)||ɸ.
CustomData.Contains(Đ))){if(ɥ!=null&&(ɸ.CustomName.Contains(ɥ)||ɸ.CustomData.Contains(ɥ)))ĕ.Add(ɸ as IMyTextPanel);ŝ.Add(ɸ as
IMyTextPanel);}}}return ĕ;}List<IMyTextPanel>ɹ(string ɥ=null){if(ɰ.Count<1)ɬ();var ĕ=new List<IMyTextPanel>();if(ǟ.Count>1){foreach(
var ɸ in ǟ){if(ɥ!=null&&(ɸ.CustomName.Contains(ɥ)||ɸ.CustomData.Contains(ɥ)))ĕ.Add(ɸ);}}else{foreach(var ɸ in ɰ){if(ɸ is
IMyTextPanel&&Me.CubeGrid==ɸ.CubeGrid){if(ɥ!=null&&(ɸ.CustomName.Contains(ɥ)||ɸ.CustomData.Contains(ɥ)))ĕ.Add(ɸ as IMyTextPanel);ǟ.
Add(ɸ as IMyTextPanel);}}}return ĕ;}List<IMyTerminalBlock>ɷ<ɦ>(string ɥ=null)where ɦ:class{if(ɰ.Count<1)ɬ();var ĕ=new List<
IMyTerminalBlock>();for(int ɩ=0;ɩ<ɰ.Count;ɩ++){if(ɰ[ɩ]is ɦ&&Me.CubeGrid==ɰ[ɩ].CubeGrid&&ɥ!=null&&(ɰ[ɩ].CustomName.Contains(ɥ)||ɰ[ɩ].
CustomData.Contains(ɥ))){ĕ.Add(ɰ[ɩ]);}}return ĕ;}List<IMyTerminalBlock>ɶ<ɦ>(string ɥ=null)where ɦ:class{if(ɰ.Count<1)ɬ();var ĕ=new
List<IMyTerminalBlock>();for(int ɩ=0;ɩ<ɰ.Count;ɩ++){if(ɰ[ɩ]is ɦ&&ɥ!=null&&ɰ[ɩ].CustomName==ɥ){ĕ.Add(ɰ[ɩ]);}}return ĕ;}
IMyTerminalBlock ɵ=null;string ɴ(){string ŷ="";var ɳ=new List<IMyTerminalBlock>();ɨ<IMyTerminalBlock>(ref ɳ,Ď);if(ɳ.Count==0){ɳ=ɧ<
IMyRemoteControl>(ď);if(ɳ.Count==0){ɨ<IMyRemoteControl>(ref ɳ);if(ɳ.Count==0){ɨ<IMyCockpit>(ref ɳ);int w=0;for(;w<ɳ.Count;w++){Echo(
"Checking Controller:"+ɳ[w].CustomName);if(ɳ[w]is IMyCryoChamber)continue;break;}if(w>=ɳ.Count){ŷ+="!!NO valid Controller:"+w+"\n";Echo(
"No Controller found");}else{ŷ+="S";Echo("Using good ship Controller: "+ɳ[w].CustomName);}}else{ŷ+="R";Echo(
"Using First Remote control found: "+ɳ[0].CustomName);}}}else{ŷ+="N";Echo("Using Named: "+ɳ[0].CustomName);}if(ɳ.Count>0)ɵ=ɳ[0];return ŷ;}string ɲ="!NAV";
void ɱ(ˣ Ë){Ë.ȓ(č,"GyroIgnore",ref ɲ,true);Ë.ȓ(č,"LIMIT_GYROS",ref ʕ,true);Ë.ȓ(č,"LEAVE_GYROS",ref ʪ,true);Ë.ȓ(č,
"CTRL_COEFF",ref ɮ,true);}double ɮ=0.9;int ʕ=99;int ʪ=-1;IMyShipController ˋ;List<IMyGyro>ˊ=new List<IMyGyro>();float ˉ=0.01f;bool ˇ
(string ˆ){if(ˋ==null)ʲ();if(ˋ is IMyShipController){Vector3D ˈ=(ˋ as IMyShipController).GetNaturalGravity();return ˇ(ˆ,ˈ
,ɵ);}else{Echo("No Controller for gravity");}return true;}bool ˇ(string ˆ,Vector3D ˁ,IMyTerminalBlock ˀ){bool ʿ=true;if(ˋ
==null)ʲ();Matrix ʾ;ˀ.Orientation.GetMatrix(out ʾ);Vector3D ʽ;ˆ=ˆ.ToLower();if(ˆ.Contains("rocket"))ʽ=ʾ.Backward;else if(ˆ
.Contains("up"))ʽ=ʾ.Up;else if(ˆ.Contains("backward"))ʽ=ʾ.Backward;else if(ˆ.Contains("forward"))ʽ=ʾ.Forward;else if(ˆ.
Contains("right"))ʽ=ʾ.Right;else if(ˆ.Contains("left"))ʽ=ʾ.Left;else ʽ=ʾ.Down;ˁ.Normalize();for(int j=0;j<ˊ.Count;++j){var ɭ=ˊ[j
];ɭ.Orientation.GetMatrix(out ʾ);var ʼ=Vector3D.Transform(ʽ,MatrixD.Transpose(ʾ));var ʻ=Vector3D.Transform(ˁ,MatrixD.
Transpose(ɭ.WorldMatrix.GetOrientation()));var ʺ=Vector3D.Cross(ʼ,ʻ);double ʹ=Vector3D.Dot(ʼ,ʻ);double ʸ=ʺ.Length();ʸ=Math.Atan2(
ʸ,Math.Sqrt(Math.Max(0.0,1.0-ʸ*ʸ)));if(ʹ<0)ʸ=Math.PI-ʸ;if(ʸ<ˉ){ɭ.GyroOverride=false;continue;}float ʷ=(float)(2*Math.PI);
double ʶ=ʷ*(ʸ/Math.PI)*ɮ;ʶ=Math.Min(ʷ,ʶ);ʶ=Math.Max(0.01,ʶ);ʺ.Normalize();ʺ*=ʶ;float ʵ=-(float)ʺ.X;if(Math.Abs(ɭ.Pitch-ʵ)>0.01
)ɭ.Pitch=ʵ;float ʴ=-(float)ʺ.Y;if(Math.Abs(ɭ.Yaw-ʴ)>0.01)ɭ.Yaw=ʴ;float ʳ=-(float)ʺ.Z;if(Math.Abs(ɭ.Roll-ʳ)>0.01)ɭ.Roll=ʳ;
ɭ.GyroOverride=true;ʿ=false;}return ʿ;}string ʲ(){string ũ="";var ˌ=new List<IMyTerminalBlock>();ˋ=ɵ as IMyShipController
;ˊ.Clear();if(ˋ==null){if(ˌ.Count<1)return"No RC!";}ͺ();GridTerminalSystem.GetBlocksOfType<IMyGyro>(ˌ,ǧ=>ǧ.CubeGrid==ɵ.
CubeGrid);int ͼ=0;for(int j=0;j<ˌ.Count;j++){if(ˌ[j].CustomName.Contains(ɲ)||ˌ[j].CustomData.Contains(ɲ)){ͼ++;continue;}ˊ.Add(ˌ[
j]as IMyGyro);}if(ʕ>0){if(ˊ.Count>ʕ){ˊ.RemoveRange(ʕ,ˊ.Count-ʕ);}else{if((ʪ-ͼ)>0){int ͻ=ˊ.Count-(ʪ-ͼ);ˊ.RemoveRange(ͻ,(ʪ-
ͼ));}}}ͺ();ũ+="GYRO#"+ˊ.Count.ToString("00")+"#";return ũ;}void ͺ(){if(ˊ!=null){for(int j=0;j<ˊ.Count;++j){ˊ[j].
GyroOverride=false;ˊ[j].Enabled=true;}}}bool ͷ(Vector3D Ͷ,Vector3D ʹ,IMyTerminalBlock ͳ){bool Ͳ=false;Vector3D ͱ=(ʹ-Ͷ);Vector3D Ĩ;if
(ͳ is IMyShipController){Ĩ=((IMyShipController)ͳ).CenterOfMass;}else{Ĩ=ͳ.GetPosition();}Vector3D Ͱ=(ʹ-Ĩ);Vector3D ˮ=ˤ(ͱ,Ͱ
);Vector3D ˬ=(ʹ-ˮ*2)-Ĩ;Ͳ=ˇ("forward",ˬ,ͳ);return Ͳ;}Vector3D ˤ(Vector3D G,Vector3D Ē){if(Vector3D.IsZero(Ē))return
Vector3D.Zero;return G-G.Dot(Ē)/Ē.LengthSquared()*Ē;}class ˣ{char ˢ='[';char ˡ=']';string ˠ=";";string ˑ="";public bool ː=false;
public string ˏ="";string ˎ="---";char ˍ='|';private MyGridProgram ʱ;private Dictionary<string,string>ʰ;private Dictionary<
string,string[]>ʖ;private Dictionary<string,Dictionary<string,string>>ʨ;private string ʧ="";static string[]ʦ={"true","yes",
"on","1"};const StringComparison ʥ=StringComparison.OrdinalIgnoreCase;const char ʤ='=';public bool ʣ{get;private set;}=false
;public ˣ(MyGridProgram ʢ,string ʠ){ʱ=ʢ;ʰ=new Dictionary<string,string>();ʖ=new Dictionary<string,string[]>();ʨ=new
Dictionary<string,Dictionary<string,string>>();ʡ(ʠ);}public int ʡ(string ʠ){ʠ.TrimEnd();if(ʧ==ʠ){return ʰ.Count;}ʰ.Clear();ʖ.Clear
();ʨ.Clear();ˑ="";ˏ="";ʣ=false;ʧ=ʠ;string[]ʟ=ʠ.Split('\n');for(int ʞ=0;ʞ<ʟ.Count();ʞ++){string ʝ="";ʟ[ʞ].Trim();if(ʟ[ʞ].
StartsWith(ˢ.ToString())){string ĩ="";for(int ʜ=1;ʜ<ʟ[ʞ].Length;ʜ++)if(ʟ[ʞ][ʜ]==ˡ)break;else ĩ+=ʟ[ʞ][ʜ];if(ĩ!=""){ʝ=ĩ.ToUpper();}
else continue;ʞ++;string ǰ="";var ʛ=new string[ʟ.Count()-ʞ];int ʚ=0;var Ț=new Dictionary<string,string>();for(;ʞ<ʟ.Count();ʞ
++){ʟ[ʞ].Trim();if(ʟ[ʞ].StartsWith(ˢ.ToString())||ʟ[ʞ].StartsWith(ˎ)){ʞ--;break;}ǰ+=ʟ[ʞ]+"\n";ʛ[ʚ++]=ʟ[ʞ];if(ʟ[ʞ].Contains
(ʤ)){string[]ʙ=ʟ[ʞ].Split('=');if(ʙ.Count()>1){string Ǵ=ʙ[0];string ʘ="";for(int j=1;j<ʙ.Count();j++){ʘ+=ʙ[j];if(j+1<ʙ.
Count())ʘ+=ʤ;}if(ʘ==""){int ʗ=ʞ+1;for(;ʗ<ʙ.Count();ʗ++){ʟ[ʗ].Trim();if(ʟ[ʗ].Length>1&&ʟ[ʗ][0]==ˍ){ʘ+=ʟ[ʗ].Substring(1).Trim()
+"\n";break;}}ʞ=ʗ;}Ț.Add(Ǵ,ʘ);}}else if(ʟ[ʞ].StartsWith(ˠ)){}}if(!ʨ.ContainsKey(ʝ)){ʨ.Add(ʝ,Ț);if(!ʖ.ContainsKey(ʝ))ʖ.Add
(ʝ,ʛ);}else{}if(!ʰ.ContainsKey(ʝ)){ʰ.Add(ʝ,ǰ);}else{ʣ=true;}}else if(ʟ[ʞ].StartsWith(ˎ)){ʞ++;for(;ʞ<ʟ.Count();ʞ++){ˏ+=ʟ[ʞ
];}}else{ˑ+=ʟ[ʞ]+"\n";}}return ʰ.Count;}public string ʩ(string Ǳ){string ǰ="";if(ʰ.ContainsKey(Ǳ))ǰ=ʰ[Ǳ];return ǰ;}public
string[]ʯ(string Ǳ){string[]ʮ={""};if(ʖ.ContainsKey(Ǳ))ʮ=ʖ[Ǳ];return ʮ;}public bool ȓ(string Ǳ,string Ǵ,ref string ʭ,bool ȑ=
false){Ǳ=Ǳ.ToUpper();if(ʨ.ContainsKey(Ǳ)){var ț=ʨ[Ǳ];if(ț.ContainsKey(Ǵ)){ʭ=ț[Ǵ];return true;}}if(ȑ)ǵ(Ǳ,Ǵ,ʭ);return false;}
public bool ȓ(string Ǳ,string Ǵ,ref long ʬ,bool ȑ=false){string Ȕ="";if(!ȓ(Ǳ,Ǵ,ref Ȕ)){if(ȑ){ǵ(Ǳ,Ǵ,ʬ);}return false;}ʬ=Convert
.ToInt64(Ȕ);return true;}public bool ȓ(string Ǳ,string Ǵ,ref int ʫ,bool ȑ=false){string Ȕ="";if(!ȓ(Ǳ,Ǵ,ref Ȕ)){if(ȑ){ǵ(Ǳ,
Ǵ,ʫ);}return false;}ʫ=Convert.ToInt32(Ȕ);return true;}public bool ȓ(string Ǳ,string Ǵ,ref double ǳ,bool ȑ=false){string Ȕ
="";if(!ȓ(Ǳ,Ǵ,ref Ȕ)){if(ȑ){ǵ(Ǳ,Ǵ,ǳ);}return false;}bool ȗ=double.TryParse(Ȕ,out ǳ);return true;}public bool ȓ(string Ǳ,
string Ǵ,ref float Ȏ,bool ȑ=false){string Ȕ="";if(!ȓ(Ǳ,Ǵ,ref Ȕ)){if(ȑ){ǵ(Ǳ,Ǵ,Ȏ.ToString());}return false;}bool ȗ=float.
TryParse(Ȕ,out Ȏ);return true;}public bool ȓ(string Ǳ,string Ǵ,ref DateTime Ȗ,bool ȑ=false){string Ȕ="";if(!ȓ(Ǳ,Ǵ,ref Ȕ)){if(ȑ){
ǵ(Ǳ,Ǵ,Ȗ);}return false;}Ȗ=DateTime.Parse(Ȕ);return true;}public bool ȓ(string Ǳ,string Ǵ,ref Vector3D ȕ,bool ȑ=false){
string Ȕ="";if(!ȓ(Ǳ,Ǵ,ref Ȕ)){if(ȑ){ǵ(Ǳ,Ǵ,ȕ);}return false;}double ĺ,Ĺ,ĸ;ǩ(Ȕ,out ĺ,out Ĺ,out ĸ);ȕ.X=ĺ;ȕ.Y=Ĺ;ȕ.Z=ĸ;return true;
}public bool ȓ(string Ǳ,string Ǵ,ref bool Ȓ,bool ȑ=false){string Ȕ="";if(!ȓ(Ǳ,Ǵ,ref Ȕ)){if(ȑ){ǵ(Ǳ,Ǵ,Ȓ);}return false;}Ȓ=ʦ
.Any(Ȍ=>string.Equals(Ȕ,Ȍ,ʥ));return true;}public bool ǵ(string Ǳ,string Ǵ,string Ȕ){if(ʰ.ContainsKey(Ǳ)){ʰ[Ǳ]="";}else{ʰ
.Add(Ǳ,"");ʣ=true;}if(ʨ.ContainsKey(Ǳ)){var Ț=new Dictionary<string,string>();var ț=ʨ[Ǳ];if(ț.ContainsKey(Ǵ)){if(ț[Ǵ]==Ȕ)
return false;ț[Ǵ]=Ȕ;}else{ț.Add(Ǵ,Ȕ);}ʣ=true;}else{var Ț=new Dictionary<string,string>();Ț.Add(Ǵ,Ȕ);ʨ.Add(Ǳ,Ț);ʣ=true;}return
true;}public bool ǵ(string Ǳ,string Ǵ,Vector3D ȕ){ǵ(Ǳ,Ǵ,Ǡ(ȕ));return true;}public bool ǵ(string Ǳ,string Ǵ,bool Ȓ){ǵ(Ǳ,Ǵ,Ȓ.
ToString());return true;}public bool ǵ(string Ǳ,string Ǵ,int ș){ǵ(Ǳ,Ǵ,ș.ToString());return true;}public bool ǵ(string Ǳ,string Ǵ
,long Ș){ǵ(Ǳ,Ǵ,Ș.ToString());return true;}public bool ǵ(string Ǳ,string Ǵ,DateTime Ȗ){ǵ(Ǳ,Ǵ,Ȗ.ToString());return true;}
public bool ǵ(string Ǳ,string Ǵ,float Ȏ){ǵ(Ǳ,Ǵ,Ȏ.ToString());return true;}public bool ǵ(string Ǳ,string Ǵ,double ǳ){ǵ(Ǳ,Ǵ,ǳ.
ToString());return true;}public void ǲ(string Ǳ,string ǰ){ǰ.TrimEnd();Ǳ=Ǳ.ToUpper();if(ʰ.ContainsKey(Ǳ)){if(ʰ[Ǳ]!=ǰ){ʰ[Ǳ]=ǰ;ʣ=
true;}}else{ʣ=true;ʰ.Add(Ǳ,ǰ);}}public string ǯ(bool Ǯ=true){string ǭ="";string Į=ˑ.Trim();if(ː&&Į!="")ǭ=Į+"\n";foreach(var
Ƕ in ʰ){ǭ+=ˢ+Ƕ.Key.Trim()+ˡ+"\n";if(Ƕ.Value.TrimEnd()==""){string Ǭ="";if(ʨ.ContainsKey(Ƕ.Key)){foreach(var Ǫ in ʨ[Ƕ.Key]
){Ǭ+=Ǫ.Key+ʤ+Ǫ.Value+"\n";}}Ǭ+="\n";ǭ+=Ǭ;}else{ǭ+=Ƕ.Value.Trim()+"\n\n";}}if(ˏ!=""){ǭ+="\n"+ˎ+"\n";ǭ+=ˏ+"\n";}if(Ǯ){ʣ=
false;ʧ=ǭ;}return ǭ;}bool ǩ(string Ǩ,out double ǧ,out double Ǧ,out double ǥ){string[]Ǥ=Ǩ.Trim().Split(',');if(Ǥ.Length<3){Ǥ=Ǩ
.Trim().Split(':');}ǧ=0;Ǧ=0;ǥ=0;if(Ǥ.Length<3)return false;bool ǣ=double.TryParse(Ǥ[0].Trim(),out ǧ);bool Ǣ=double.
TryParse(Ǥ[1].Trim(),out Ǧ);bool ǡ=double.TryParse(Ǥ[2].Trim(),out ǥ);if(!ǣ||!Ǣ||!ǡ){return false;}return true;}string Ǡ(
Vector3D ǫ){string ũ;ũ=ǫ.X.ToString("0.00")+":"+ǫ.Y.ToString("0.00")+":"+ǫ.Z.ToString("0.00");return ũ;}}List<IMyTerminalBlock>Ƿ
=new List<IMyTerminalBlock>();string ȏ(){Ƿ.Clear();Ƿ=ɨ<IMyLightingBlock>();return"L"+Ƿ.Count.ToString("00");}void ȍ(List<
IMyTerminalBlock>Ƿ,Color Ȍ){for(int w=0;w<Ƿ.Count;w++){var ȋ=Ƿ[w]as IMyLightingBlock;if(ȋ==null)continue;if(ȋ.Color.Equals(Ȍ)&&ȋ.Enabled
){continue;}ȋ.Color=Ȍ;}}int Ȋ=80;int ȉ=20;double Ȉ=0;string ȇ="POWER";void Ȇ(ˣ Ë){Ë.ȓ(ȇ,"batterypcthigh",ref Ȋ,true);Ë.ȓ(
ȇ,"batterypctlow",ref ȉ,true);}void ȅ(){Ȉ=0;Echo("Init Reactors");Ԥ();Echo("Init Solar");ӯ();Echo("Init Batteries");ҡ();
if(Ԧ>0)Ȉ+=Ԧ;if(ѹ>0)Ȉ+=ѹ;}string Ȅ="[WICO]";double ȃ=0.175;const string Ȃ="SENSORS";void ȁ(ˣ Ë){Ë.ȓ(Ȃ,"SensorUse",ref Ȅ,
true);Ë.ȓ(Ȃ,"SensorSettleWaitMS",ref ȃ,true);}List<IMySensorBlock>Ȁ=new List<IMySensorBlock>();struct ǿ{public long Ǿ;public
double ǽ;public double Ǽ;public double ǻ;public double Ǻ;public double ǹ;public double Ȑ;}List<ǿ>Ǹ=new List<ǿ>();string Ȝ(
IMyTerminalBlock ɐ,bool ɏ=false){Ȁ.Clear();Ǹ.Clear();List<IMyTerminalBlock>Ɏ=ɧ<IMySensorBlock>(Ȅ);Ҷ Ɉ=new Ҷ(ɵ);Vector3D ɇ;Vector3D ȝ;
Vector3D ȳ;Vector3D Ȳ;Vector3D ȱ;Vector3D Ȱ;Vector3D ȯ;Vector3D Ȯ;Vector3D[]ȭ=new Vector3D[4];Ɉ.Ԅ(Ҷ.ԍ,ȭ);ȝ=ȭ[0];Ȳ=ȭ[1];ɇ=ȭ[2];ȳ=
ȭ[3];Ɉ.Ԅ(Ҷ.Ԏ,ȭ);Ȱ=ȭ[0];Ȯ=ȭ[1];ȱ=ȭ[2];ȯ=ȭ[3];foreach(var ɍ in Ɏ){Ȁ.Add(ɍ as IMySensorBlock);ǿ Ɍ=new ǿ();Ɍ.Ǿ=ɍ.EntityId;
Vector3D Ŀ=ɍ.GetPosition();double Ȭ=ɣ(Ŀ,ȝ,Ȳ,ȳ);double Ȫ=ɣ(Ŀ,Ȱ,Ȯ,ȯ);double Ȩ=ɣ(Ŀ,ȝ,ɇ,Ȱ);double ȧ=ɣ(Ŀ,Ȳ,ȳ,Ȯ);double Ȧ=ɣ(Ŀ,ɇ,ȳ,ȱ);
double ȥ=ɣ(Ŀ,ȝ,Ȳ,Ȯ);Ɍ.ǽ=Ȭ;Ɍ.Ǽ=Ȫ;Ɍ.ǻ=Ȩ;Ɍ.Ǻ=ȧ;Ɍ.ǹ=Ȧ;Ɍ.Ȑ=ȥ;Ǹ.Add(Ɍ);}if(ɏ)ɑ();return"S"+Ȁ.Count.ToString("00");}List<
IMySensorBlock>ɋ(string Ɋ=null){List<IMySensorBlock>ɉ=new List<IMySensorBlock>();for(int j=0;j<Ȁ.Count;j++){IMySensorBlock ũ=Ȁ[j]as
IMySensorBlock;if(ũ==null)continue;if(ũ.IsActive&&ũ.Enabled&&!ũ.LastDetectedEntity.IsEmpty()){ɉ.Add(Ȁ[j]);}}return ɉ;}void ɑ(){for(int
j=0;j<Ȁ.Count;j++){IMySensorBlock ɍ=Ȁ[j]as IMySensorBlock;if(ɍ==null)continue;ɍ.LeftExtend=ɍ.RightExtend=ɍ.TopExtend=ɍ.
BottomExtend=ɍ.FrontExtend=ɍ.BackExtend=1;ɍ.Enabled=false;}}double ɣ(Vector3D Ŀ,Vector3D ɟ,Vector3D ɞ,Vector3D ɝ){double ɢ=0;
Vector3D ɚ=ɠ(ɟ,ɞ,ɝ);Vector3D ɡ=Ŀ-ɟ;ɢ=Vector3D.Dot(ɡ,ɚ);return ɢ;}Vector3D ɠ(Vector3D ɟ,Vector3D ɞ,Vector3D ɝ){Vector3D ɜ=ɟ-ɞ;ɜ.
Normalize();Vector3D ɛ=ɞ-ɝ;ɛ.Normalize();Vector3D ɚ=Vector3D.Cross(ɜ,ɛ);ɚ.Normalize();return ɚ;}void ə(IMyTerminalBlock ɘ,float ɗ
,float ɖ,float ɕ,float ɔ,float ɓ,float ɒ){IMySensorBlock ɍ=ɘ as IMySensorBlock;int j=0;for(;j<Ǹ.Count;j++){if(Ǹ[j].Ǿ==ɍ.
EntityId)break;}if(j<Ǹ.Count){float Ȥ=0;if(ɗ<0)Ȥ=-ɗ;else Ȥ=(float)Math.Abs(ɗ+Math.Abs(Ǹ[j].ǻ));ɍ.LeftExtend=Math.Max(Ȥ,1.0f);if(
ɖ<0)Ȥ=-ɖ;else Ȥ=(float)Math.Abs(ɖ+Math.Abs(Ǹ[j].Ǻ));ɍ.RightExtend=Math.Max(Ȥ,1.0f);if(ɕ<0)Ȥ=-ɕ;else Ȥ=(float)Math.Abs(ɕ+
Math.Abs(Ǹ[j].ǹ));ɍ.TopExtend=Math.Max(Ȥ,1.0f);if(ɔ<0)Ȥ=-ɔ;else Ȥ=(float)Math.Abs(ɔ+Math.Abs(Ǹ[j].Ȑ));ɍ.BottomExtend=Math.
Max(Ȥ,1.0f);if(ɓ<0)Ȥ=-ɓ;else Ȥ=(float)Math.Abs(ɓ+Math.Abs(Ǹ[j].ǽ));ɍ.FrontExtend=Math.Max(Ȥ,1.0f);if(ɒ<0)Ȥ=-ɒ;else Ȥ=(float
)Math.Abs(ɒ+Math.Abs(Ǹ[j].Ǽ));ɍ.BackExtend=Math.Max(Ȥ,1.0f);}else{Ҷ Ɉ=new Ҷ(ɵ);Vector3D ɇ;Vector3D ȝ;Vector3D ȳ;Vector3D
Ȳ;Vector3D ȱ;Vector3D Ȱ;Vector3D ȯ;Vector3D Ȯ;Vector3D[]ȭ=new Vector3D[4];Ɉ.Ԅ(Ҷ.ԍ,ȭ);ȝ=ȭ[0];Ȳ=ȭ[1];ɇ=ȭ[2];ȳ=ȭ[3];Ɉ.Ԅ(Ҷ.Ԏ,
ȭ);Ȱ=ȭ[0];Ȯ=ȭ[1];ȱ=ȭ[2];ȯ=ȭ[3];ف("FBL",ȝ);ف("FBR",Ȳ);ف("FTL",ɇ);ف("FTR",ȳ);ف("BBL",Ȱ);ف("BBR",Ȯ);ف("BTL",ȱ);ف("BTR",ȯ);if
(ɍ==null)return;Echo(ɍ.CustomName);Vector3D Ŀ=ɍ.GetPosition();double Ȭ=ɣ(Ŀ,ȝ,Ȳ,ȳ);Echo("DistanceFront="+Ȭ.ToString("0.00"
));Vector3D ȫ=Ŀ+ɍ.WorldMatrix.Forward*Ȭ;ف("FRONT",ȫ);double Ȫ=ɣ(Ŀ,Ȱ,Ȯ,ȯ);Echo("DistanceBack="+Ȫ.ToString("0.00"));
Vector3D ȩ=Ŀ+ɍ.WorldMatrix.Forward*Ȫ;ف("BACK",ȩ);double Ȩ=ɣ(Ŀ,ȝ,ɇ,Ȱ);double ȧ=ɣ(Ŀ,Ȳ,ȳ,Ȯ);double Ȧ=ɣ(Ŀ,ɇ,ȳ,ȱ);double ȥ=ɣ(Ŀ,ȝ,Ȳ,Ȯ)
;float Ȥ=0;Ȥ=(float)Math.Abs(ɗ+Math.Abs(Ȩ));ɍ.LeftExtend=Math.Max(Ȥ,1.0f);Ȥ=(float)Math.Abs(ɖ+Math.Abs(ȧ));ɍ.RightExtend=
Math.Max(Ȥ,1.0f);Ȥ=(float)Math.Abs(ɕ+Math.Abs(Ȧ));ɍ.TopExtend=Math.Max(Ȥ,1.0f);Ȥ=(float)Math.Abs(ɔ+Math.Abs(ȥ));ɍ.
BottomExtend=Math.Max(Ȥ,1.0f);Ȥ=(float)Math.Abs(ɓ+Math.Abs(Ȭ));ɍ.FrontExtend=Math.Max(Ȥ,1.0f);Ȥ=(float)Math.Abs(ɒ+Math.Abs(Ȫ));ɍ.
BackExtend=Math.Max(Ȥ,1.0f);}ɍ.Enabled=true;}bool ȣ(IMySensorBlock Į,ref bool Ȣ,ref bool ȡ,ref bool Ƞ){Ȣ=false;ȡ=false;Ƞ=false;if(
Į!=null&&Į.IsActive&&Į.Enabled&&!Į.LastDetectedEntity.IsEmpty()){List<MyDetectedEntityInfo>ȟ=new List<
MyDetectedEntityInfo>();Į.DetectedEntities(ȟ);for(int Ã=0;Ã<ȟ.Count;Ã++){if(ȟ[Ã].Type==MyDetectedEntityType.Asteroid){Ȣ=true;}else if(ȟ[Ã].
Type==MyDetectedEntityType.LargeGrid){ȡ=true;}else if(ȟ[Ã].Type==MyDetectedEntityType.SmallGrid){Ƞ=true;}}}return Ȣ||ȡ||Ƞ;}
List<IMyTerminalBlock>Ȟ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ȴ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ȵ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ʌ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ɇ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>Ʉ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ƀ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ɂ=new List<IMyTerminalBlock>();string Ɂ(IMyTerminalBlock Ń){Ȟ.Clear();ȴ.Clear();ȵ.Clear();Ʌ.Clear();Ɇ.Clear();Ʉ.Clear()
;Ƀ.Clear();ɂ.Clear();ɨ<IMyMotorSuspension>(ref Ȟ);for(int w=0;w<Ȟ.Count;w++){if(Ȟ[w].CustomName.Contains("[SLED]")||Ȟ[w].
CustomData.Contains("[SLED]")){ȴ.Add(Ȟ[w]);if(Ȟ[w].CustomName.Contains("[REAR]")||Ȟ[w].CustomData.Contains("[FRONT]")){ȵ.Add(Ȟ[w])
;}if(Ȟ[w].CustomName.Contains("[FRONT]")||Ȟ[w].CustomData.Contains("[FRONT]")){Ʌ.Add(Ȟ[w]);}}else{if(Ȟ[w].CustomName.
Contains("[LEFT]")||Ȟ[w].CustomData.Contains("[LEFT]")){Ƀ.Add(Ȟ[w]);}else if(Ȟ[w].CustomName.Contains("[RIGHT]")||Ȟ[w].
CustomData.Contains("[RIGHT]")){ɂ.Add(Ȟ[w]);}if(Ȟ[w].CustomName.Contains("[REAR]")||Ȟ[w].CustomData.Contains("[FRONT]")){Ɇ.Add(Ȟ[w
]);}if(Ȟ[w].CustomName.Contains("[FRONT]")||Ȟ[w].CustomData.Contains("[FRONT]")){Ʉ.Add(Ȟ[w]);}}}return"W"+Ȟ.Count.
ToString("0")+"WS"+ȴ.Count.ToString("0")+"SR"+ȵ.Count.ToString("0")+"SF"+Ʌ.Count.ToString("0");}bool ɀ(){if(ȴ.Count>0)return
true;return false;}void ȿ(){foreach(var Ɖ in ȴ){var ȶ=Ɖ as IMyMotorSuspension;ȶ.SetValueFloat("Friction",0);}}bool Ⱦ(){if(Ȟ.
Count>0){return true;}return false;}bool Ƚ(float ȼ,float ȷ=-1){Echo("WPP:"+ȼ.ToString()+":"+ȷ.ToString());bool Ȼ=true;if(ȼ<0f
)ȼ=0f;if(ȼ>100f)ȼ=100f;foreach(var Ɖ in ɂ){var ȶ=Ɖ as IMyMotorSuspension;float Ⱥ=ȶ.GetValueFloat("Propulsion override");
Echo("CPower:"+Ⱥ.ToString("0.00")+"\n"+ȶ.CustomName);float ȹ=(Ⱥ);ȹ=Math.Abs(ȹ);if(ȹ<1)ȹ*=100f;if(ȼ>(ȹ+5f)){Ȼ=false;ȹ+=5;}
else if(ȼ<(ȹ-5)){Ȼ=false;ȹ-=5;}else ȹ=ȼ;if(ȷ>=0)ȶ.SetValueFloat("Friction",ȷ);Echo("Setting override to"+ȹ.ToString("0.000")
);ȶ.SetValueFloat("Propulsion override",-ȹ);}foreach(var Ɖ in Ƀ){var ȶ=Ɖ as IMyMotorSuspension;float Ⱥ=ȶ.GetValueFloat(
"Propulsion override");Echo("CPower:"+Ⱥ.ToString("0.00")+"\n"+ȶ.CustomName);float ȹ=(Ⱥ);ȹ=Math.Abs(ȹ);if(ȹ<1)ȹ*=100f;if(ȼ>(ȹ+5f)){Ȼ=false;ȹ+=
5;}else if(ȼ<(ȹ-5)){Ȼ=false;ȹ-=5;}else ȹ=ȼ;if(ȷ>=0)ȶ.SetValueFloat("Friction",ȷ);Echo("Setting override to"+(ȹ).ToString(
"0.000"));ȶ.SetValueFloat("Propulsion override",ȹ);}return Ȼ;}void ȸ(float ȷ){foreach(var Ɖ in Ȟ){var ȶ=Ɖ as IMyMotorSuspension
;ȶ.SetValueFloat("Friction",ȷ);}}