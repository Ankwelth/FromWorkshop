/*
Wico craft Antenna Receive sub-module

Full Source available here: https://github.com/Wicorel/SpaceEngineers/tree/master/MDK%20Ant%20Receive
Workshop here: http://steamcommunity.com/sharedfiles/filedetails/?id=883864500
*
Handles:
Receiving messages from antennas
Passing messages on to other sub-modules
queueing of received mesages

Dock Manager.  Handles managing connectors and requests for docking
 
Modes:
none

Commands:

Need:

Want:

3.0 Match control code
3.0c Performance Optimizations
3.0D rotor NOFOLLOW
ignore projectors with !WCC in name or customdata

3.0E MDK Version

3.0F Combined DockMgr into code

3.0G New BASE communications.  remove MOM

3.0G2  search order for text panels

3.0H
Increase time before active BASE send.  Randomize time before starting to send to avoid burst transmission at world start.

3.1 Version for SE 1.185

3.1A 
Now using MDK Minify (size was >80k without)
Moved a number of utility routines into WicoAntenna
Check antenna for this PB set as attached.  Attach this PB to the antenna with maximum range

*3.2 Drone height, width and length in docking requests

3.2A 12262017 INI Save
01062018

3.2B 01132018
Getting settings from CustomData
All ignore for !WCC
SenorUse
For docking, only use lights with [BASE]

3.3 0119 Lists of panels.
Output to panels at END of script.

3.3a redo blockinit

3.4a optimizations for text panel init.

3.4B Current Code Compile. Mar 08 2018

3.4C Current Code Compile Mar 29 2018
Added MODE_DOSCANS
Add asteroids and ore message processing.

3.4D  May 05 2018
text panel reports during scans
May 27, 2018
published June 1, 2018

3.4E June 02, 2018
June 08,2018 FAST doscan mode current 0->410 FAST

3.4F June 19, 2018
Add PATROL antenna message processing

3.4G Add PATROL supprot for planepilot RC block

3.5 SE 1.189

3.7IGC Use IGC for intergrid communication

3.8 SE 1.193.100 
Remove last old IGC

TODO: Choose closest connector as 'best'
TODO: Process size info to select connectors
TODO: drones may take up more than one connector depending on height, width.

TODO: Hangar door control: open/close by antenna request

TODO: send/receive 'timer' messages. Like https://steamcommunity.com/sharedfiles/filedetails/?id=1287452364


*/
string ʖ="Wico Craft";string ʕ="AntReceive";string ʔ="3.8";void ʓ(ǲ W){ƫ(W);ʜ();ů();}void ʒ(ǲ W){ƫ(W);ʝ();Ū();}void ʑ(){
ą();}void ʐ(){Echo(ʍ.Count.ToString()+" Known Asteroids");Echo(ƒ.Count.ToString()+" Known Ores");Echo(Ě);ĩ();}void ʏ(bool
ʎ=false){if(ǵ is IMyRemoteControl)((IMyRemoteControl)ǵ).SetAutoPilotEnabled(false);if(ǵ is IMyShipController)((
IMyShipController)ǵ).DampenersOverride=true;}List<ʋ>ʍ=new List<ʋ>();const string ʌ="ASTEROIDS";class ʋ{public long ʊ;public BoundingBoxD
ʗ;public Vector3D Ĺ{get{return ʗ.Center;}}}void ʝ(){if(υ==null)return;var ũ=ʍ.Count;υ.Ɗ(ʌ,"count",ũ);for(int Ó=0;Ó<ʍ.
Count;Ó++){υ.Ɗ(ʌ,"EntityId"+Ó.ToString(),ʍ[Ó].ʊ.ToString());υ.Ɗ(ʌ,"BBMin"+Ó.ToString(),Ɩ(ʍ[Ó].ʗ.Min));υ.Ɗ(ʌ,"BBMax"+Ó.
ToString(),Ɩ(ʍ[Ó].ʗ.Max));}}void ʜ(){if(υ==null)return;int ŏ=0;υ.Ǚ(ʌ,"count",ref ŏ);ʍ.Clear();for(int Ľ=0;Ľ<ŏ;Ľ++){long Ŏ=0;υ.Ǚ(
ʌ,"EntityId"+Ľ.ToString(),ref Ŏ);BoundingBoxD ʄ=new BoundingBoxD();υ.Ǚ(ʌ,"BBMin"+Ľ.ToString(),ref ʄ.Min);υ.Ǚ(ʌ,"BBMax"+Ľ.
ToString(),ref ʄ.Max);ʋ ɸ=new ʋ();ɸ.ʊ=Ŏ;ɸ.ʗ=ʄ;ʍ.Add(ɸ);}}void ʛ(){ʍ.Clear();ʜ();}void ʘ(long ʚ,BoundingBoxD ʄ,bool ʀ=true){bool
Ÿ=false;for(int F=0;F<ʍ.Count;F++){if(ʍ[F].ʊ==ʚ){Ÿ=true;break;}}if(!Ÿ){ʋ ʙ=new ʋ();ʙ.ʊ=ʚ;ʙ.ʗ=ʄ;ʍ.Add(ʙ);ʝ();if(ʀ){Ŝ("AST"
,ν.EntityId.ToString()+":"+ʚ.ToString()+":"+Ɩ(ʄ.Min)+":"+Ɩ(ʄ.Max));}}}void ʘ(MyDetectedEntityInfo Ɉ,bool ʀ=true){if(Ɉ.
IsEmpty()||Ɉ.Type!=MyDetectedEntityType.Asteroid)return;ʘ((long)Ɉ.EntityId,Ɉ.BoundingBox,ʀ);}bool ʈ(List<MyDetectedEntityInfo>ɾ
){bool ɻ=false;for(int ɽ=0;ɽ<ɾ.Count;ɽ++){if(ɾ[ɽ].Type==MyDetectedEntityType.Asteroid){if(ɼ(ɾ[ɽ]))ɻ=true;}}return ɻ;}bool
ɼ(MyDetectedEntityInfo Ɇ){ɉ(Ɇ);bool ɻ=false;if(Ɇ.Type==MyDetectedEntityType.Asteroid){ʘ(Ɇ);ɻ=true;}return ɻ;}long ɿ(bool
ɺ=false){long ɶ=-1;if(ǵ==null)return ɶ;if(ʍ.Count<1)ʜ();double ň=double.MaxValue;foreach(var ɸ in ʍ){if(ɺ){if(ɸ.ʗ.
Contains(ǵ.GetPosition())==ContainmentType.Contains){ɶ=ɸ.ʊ;}}else{double Ň=Vector3D.DistanceSquared(ɸ.Ĺ,ǵ.GetPosition());if(Ň<ň)
{ɶ=ɸ.ʊ;ň=Ň;}}}return ɶ;}Vector3D ɷ(long ɶ){if(ʍ.Count<1)ʜ();Vector3D ɹ=new Vector3D(0,0,0);for(int F=0;F<ʍ.Count;F++){if(
ʍ[F].ʊ==ɶ)ɹ=ʍ[F].Ĺ;}return ɹ;}BoundingBoxD ʁ(long ɶ){if(ʍ.Count<1)ʜ();BoundingBoxD ʄ=new BoundingBoxD();for(int F=0;F<ʍ.
Count;F++){if(ʍ[F].ʊ==ɶ){ʄ=ʍ[F].ʗ;break;}}return ʄ;}bool ʇ(string o){double n,m,k;string[]µ=o.Trim().Split(':');if(µ.Length>1
){if(µ[0]!="WICO"){Echo("not wico system message");return false;}if(µ.Length>2){if(µ[1]=="AST"){int ª=2;long u=0;long.
TryParse(µ[ª++],out u);long Ʊ=0;long.TryParse(µ[ª++],out Ʊ);n=Convert.ToDouble(µ[ª++]);m=Convert.ToDouble(µ[ª++]);k=Convert.
ToDouble(µ[ª++]);Vector3D ʆ=new Vector3D(n,m,k);n=Convert.ToDouble(µ[ª++]);m=Convert.ToDouble(µ[ª++]);k=Convert.ToDouble(µ[ª++])
;Vector3D ʅ=new Vector3D(n,m,k);BoundingBoxD ʄ=new BoundingBoxD(ʆ,ʅ);ʘ(Ʊ,ʄ,false);return true;}else if(µ[1]=="AST?"){}}}
return false;}double ʃ=55;double ʉ=-1;string ʂ="BASE";void ʞ(ǲ W){W.Ǚ(ʂ,"BaseTransmitWait",ref ʃ,true);}void ʻ(bool ʺ=false){
if(ɂ.Count>0){if(ʉ>ʃ||ʺ){ʉ=0;bool q=false;string ʹ=Me.CubeGrid.CustomName;Vector3D r=Ř();if(ǵ!=null){ʹ=ǵ.CubeGrid.
CustomName;}Ŝ("BASE",Φ("",ʹ)+":"+ν.EntityId.ToString()+":"+Ɩ(r)+":"+q.ToString());}else{if(ʉ<0){ʉ=Me.EntityId%ʃ;}ʉ+=Runtime.
TimeSinceLastRun.TotalSeconds;Echo("BASE: Last Transmit="+ʉ.ToString());}}}bool ʸ(string ʷ){string[]µ=ʷ.Trim().Split(':');if(µ.Length>1)
{if(µ[0]!="WICO"){Echo("not wico system message");return false;}if(µ.Length>2){if(µ[1]=="HELLO"){Echo("HELLO");ʉ=ʃ+5;Ė=
true;}return false;}}return false;}void ʶ(List<IMyTerminalBlock>ď,bool B=true){foreach(var Ȱ in ď){IMyFunctionalBlock ʵ=Ȱ as
IMyFunctionalBlock;if(ʵ==null)continue;ʵ.Enabled=B;}}void ʴ(List<IMyTerminalBlock>ď){foreach(var Ȱ in ď){IMyFunctionalBlock ʵ=Ȱ as
IMyFunctionalBlock;if(ʵ==null)continue;ʵ.Enabled=!ʵ.Enabled;}}string ˌ="[VIEW]";Matrix ˍ=new Matrix(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);List<
IMyTerminalBlock>ˋ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ˊ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ˉ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>ˈ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ˇ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ˆ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ˁ=new List<IMyTerminalBlock>();IMyTerminalBlock ˀ=null;
MyDetectedEntityInfo Ȭ;string ʿ="CAMERAS";void ʾ(ǲ W){W.Ǚ(ʿ,"CameraViewOnly",ref ˌ,true);}bool ʧ(List<IMyTerminalBlock>ʢ,double ʽ=100,float
ʼ=0,float ʳ=0){double ʥ=0;ˀ=null;for(int F=0;F<ʢ.Count;F++){double ʤ=((IMyCameraBlock)ʢ[F]).AvailableScanRange;if(ʤ>ʥ){ʥ=
ʤ;ˀ=ʢ[F];}}var ʠ=ˀ as IMyCameraBlock;if(ˀ==null){return false;}if(ʠ.CanScan(ʽ)){Ȭ=ʠ.Raycast(ʽ,ʼ,ʳ);ˀ=ʠ;if(!Ȭ.IsEmpty())ɉ(
Ȭ);return true;}else{}return false;}bool ʧ(List<IMyTerminalBlock>ʢ,Vector3D ʦ){double ʥ=0;ˀ=null;for(int F=0;F<ʢ.Count;F
++){double ʤ=((IMyCameraBlock)ʢ[F]).AvailableScanRange;if(ʤ>ʥ){ʥ=ʤ;ˀ=ʢ[F];}}var ʠ=ˀ as IMyCameraBlock;if(ˀ==null)return
false;{Ȭ=ʠ.Raycast(ʦ);ˀ=ʠ;if(!Ȭ.IsEmpty())ɉ(Ȭ);return true;}}double ʣ(List<IMyTerminalBlock>ʢ){double ʡ=0;for(int F=0;F<ʢ.
Count;F++){IMyCameraBlock ʠ=ʢ[F]as IMyCameraBlock;if(ʡ<ʠ.AvailableScanRange)ʡ=ʠ.AvailableScanRange;}return ʡ;}string ʟ(
IMyTerminalBlock ʨ){ˋ.Clear();ˊ.Clear();ˉ.Clear();ˈ.Clear();ˇ.Clear();ˆ.Clear();ˁ.Clear();if(ʨ==null)return
"\nCameras:No OrientationBlock";GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(ˁ,(n=>n.CubeGrid==Me.CubeGrid));Matrix ʲ;ʨ.Orientation.GetMatrix(out
ʲ);Matrix.Transpose(ref ʲ,out ʲ);for(int F=0;F<ˁ.Count;++F){if(ˁ[F].CustomName.Contains(ˌ))continue;IMyCameraBlock ʠ=ˁ[F]
as IMyCameraBlock;ʠ.EnableRaycast=true;Matrix ʱ;ʠ.Orientation.GetMatrix(out ʱ);Vector3 ʰ=Vector3.Transform(ʱ.Forward,ʲ);if
(ʰ==ˍ.Left){ˇ.Add(ˁ[F]);}else if(ʰ==ˍ.Right){ˆ.Add(ˁ[F]);}else if(ʰ==ˍ.Backward){ˊ.Add(ˁ[F]);}else if(ʰ==ˍ.Forward){ˋ.Add
(ˁ[F]);}else if(ʰ==ˍ.Up){ˈ.Add(ˁ[F]);}else if(ʰ==ˍ.Down){ˉ.Add(ˁ[F]);}}string è;è="CS:<";è+="F"+ˋ.Count.ToString("00");è
+="B"+ˊ.Count.ToString("00");è+="D"+ˉ.Count.ToString("00");è+="U"+ˈ.Count.ToString("00");è+="L"+ˇ.Count.ToString("00");è+=
"R"+ˆ.Count.ToString("00");è+=">";return è;}void ʯ(List<IMyTerminalBlock>ʢ,string ʮ){string w;for(int F=0;F<ʢ.Count;F++){if
(!ʢ[F].CustomName.Contains(ʮ)){w="Camera ";if(ʢ.Count>1)w+=(F+1).ToString()+" ";w+=ʮ;ʢ[F].CustomName=w;}}}List<
IMyTerminalBlock>ʭ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ʬ=new List<IMyTerminalBlock>();string ʫ(IMyTerminalBlock ʨ){string
è="";if(ˁ.Count<1)è+=ʟ(ʨ);ʭ.Clear();ʬ.Clear();foreach(var ʠ in ˉ){if(ʠ.CustomName.ToLower().Contains("fore")||ʠ.
CustomData.ToLower().Contains("fore"))ʭ.Add(ʠ);else if(ʠ.CustomName.ToLower().Contains("aft")||ʠ.CustomData.ToLower().Contains(
"aft"))ʬ.Add(ʠ);}è+="HCS:<";è+="F"+ʭ.Count.ToString("00");è+="A"+ʬ.Count.ToString("00");è+=">";return è;}class ʪ{bool ʩ=false
;bool ɵ=false;public bool ɋ=false;public Vector3D ȩ;Program Ǥ;public double Ȩ=1250;double ȧ=5000;float Ȧ=25f;float ȥ=25f;
double Ȥ=5;float ȣ=3;float Ȣ=0.5f;public float ȡ=0;public float Ƞ=0;float ȟ=0;float Ȟ=0;List<IMyTerminalBlock>ȝ=new List<
IMyTerminalBlock>();private int Ȝ=0;private int Ȫ=1;public MyDetectedEntityInfo Ȭ;public List<MyDetectedEntityInfo>ȹ=new List<
MyDetectedEntityInfo>();public ʪ(Program ǐ,List<IMyTerminalBlock>ď,double ȸ=1250,float ȷ=45f,float ȶ=45f,float ȵ=2,float ȴ=1,float ȳ=0.5f,
double Ȳ=5000,bool ȱ=false){Ǥ=ǐ;ʩ=false;ɵ=ȱ;ɋ=false;ȝ.Clear();ȹ.Clear();Ȭ=new MyDetectedEntityInfo();foreach(var Ȱ in ď){if(Ȱ
is IMyCameraBlock){ȝ.Add(Ȱ);IMyCameraBlock Ǹ=Ȱ as IMyCameraBlock;Ǹ.EnableRaycast=true;if(Ȧ>Ǹ.RaycastConeLimit)Ȧ=Ǹ.
RaycastConeLimit;if(ȥ>Ǹ.RaycastConeLimit)ȥ=Ǹ.RaycastConeLimit;}}if(ȸ>Ȳ)Ȳ=ȸ;Ȩ=ȸ;Ȧ=ȷ;ȥ=ȶ;Ȥ=ȵ;ȣ=ȴ;Ȣ=ȳ;ȧ=Ȳ;ȡ=0;Ƞ=0;ȟ=0;Ȟ=0;Ȝ=0;Ȫ=ȝ.Count;}
public bool ȯ(){return ʩ;}void Ȯ(MyDetectedEntityInfo Ȭ){bool ȭ=true;for(int F=0;F<ȹ.Count;F++){if(ȹ[F].EntityId==Ȭ.EntityId)ȭ
=false;}if(ȭ){ȹ.Add(Ȭ);}}public bool ȏ(){if(ȝ.Count<1)ʩ=true;if(ʩ)return false;bool ș=false;for(int ȍ=0;ȍ<Ȫ;ȍ++){if(Ǥ.ʧ(ȝ
,Ȩ,Ȟ,ȟ)){Ȭ=Ǥ.Ȭ;if(!Ȭ.IsEmpty()){bool Ȍ=true;if((Ȭ.Type==MyDetectedEntityType.LargeGrid)||(Ȭ.Type==MyDetectedEntityType.
SmallGrid)){if(Ǥ.Ò(Ȭ.EntityId)){Ȍ=false;}}if(Ȍ){Ȯ(Ȭ);ș=true;}}else if(ɵ){ʩ=true;Vector3D ȋ;Vector3D.CreateFromAzimuthAndElevation
(MathHelper.ToRadians(Ƞ),MathHelper.ToRadians(ȡ),out ȋ);ȩ=Vector3D.TransformNormal(ȋ,Ǥ.ˀ.WorldMatrix);ɋ=true;return false
;}Ȝ++;if(Ȟ==0&&ȟ==0){ȡ=Ȣ;Ƞ=Ȣ;Ȝ=0;}if(Ȝ>3){Ȝ=0;Ƞ+=Math.Abs(Ƞ/ȣ)+Ȣ;if(Math.Abs(Ƞ)>Ȧ){Ȝ=0;Ƞ=0;ȡ+=Math.Abs(ȡ/ȣ)+Ȣ;}if(Math.
Abs(ȡ)>ȥ){ȡ=0;Ƞ=0;Ȝ=0;{Ȩ*=Ȥ;if(Ȩ>ȧ){ʩ=true;return false;}}}}switch(Ȝ){case 0:Ȟ=ȡ;ȟ=Ƞ;break;case 1:Ȟ=-ȡ;ȟ=Ƞ;break;case 2:Ȟ=ȡ
;ȟ=-Ƞ;break;case 3:Ȟ=-ȡ;ȟ=-Ƞ;break;}}}return ș;}}int Ȋ=5;int ȉ=-1;double Ȉ=-1;string Ȏ="CARGO";void ȇ(ǲ W){W.Ǚ(Ȏ,
"cargopctmin",ref Ȋ,true);}List<IMyTerminalBlock>ȅ=null;bool Ȅ=false;double ȃ=0.0;void Ȃ(){var ď=new List<IMyTerminalBlock>();if(ȅ==
null)ȅ=new List<IMyTerminalBlock>();else ȅ.Clear();à<IMyCargoContainer>(ref ď);ȅ.AddRange(ď);ȉ=-1;Ȉ=-1;}void Ȇ(){var ď=new
List<IMyTerminalBlock>();à<IMyShipConnector>(ref ď);foreach(var Ǹ in ď){if(Ǹ.CustomName.Contains("Ejector")||Ǹ.CustomData.
Contains("Ejector"))continue;else ȅ.Add(Ǹ);}}void Ț(){var ď=new List<IMyTerminalBlock>();à<IMyShipDrill>(ref ď);ȅ.AddRange(ď);}
void Ș(){var ď=new List<IMyTerminalBlock>();à<IMyShipWelder>(ref ď);ȅ.AddRange(ď);}void ȗ(){var ď=new List<IMyTerminalBlock>
();à<IMyShipGrinder>(ref ď);ȅ.AddRange(ď);}bool Ȗ=true;void ȕ(){var ď=new List<IMyTerminalBlock>();if(ȅ==null)ȅ=new List<
IMyTerminalBlock>();else ȅ.Clear();if(!Ȗ)GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(ď,Õ);else à<IMyCargoContainer>(ref ď);ȅ.
AddRange(ď);ď.Clear();if(!Ȗ)GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(ď,Õ);else à<IMyShipConnector>(ref ď);foreach(
var Ǹ in ď){if(Ǹ.CustomName.Contains("Ejector")||Ǹ.CustomData.Contains("Ejector"))continue;else if(Ǹ.CustomName.Contains(
"Sorter")||Ǹ.CustomData.Contains("Sorter"))continue;else ȅ.Add(Ǹ);}ď.Clear();if(!Ȗ)GridTerminalSystem.GetBlocksOfType<
IMyShipDrill>(ď,Õ);else à<IMyShipDrill>(ref ď);ȅ.AddRange(ď);ď.Clear();if(!Ȗ)GridTerminalSystem.GetBlocksOfType<IMyShipWelder>(ď,Õ);
else à<IMyShipWelder>(ref ď);ȅ.AddRange(ď);ď.Clear();if(!Ȗ)GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(ď,Õ);else à<
IMyShipGrinder>(ref ď);ȅ.AddRange(ď);ȉ=-1;Ȉ=-1;}void Ȕ(){if(ȅ==null)ȕ();if(ȅ.Count<1){ȉ=-1;Ȉ=-1;return;}ȃ=0.0;double ȓ=0.0;double Ȓ=0;
bool ȑ=true;bool Ȑ=false;for(int F=0;F<ȅ.Count;F++){double ț=-1;var ũ=ȅ[F].InventoryCount;for(var Ⱥ=0;Ⱥ<ũ;Ⱥ++){var ƣ=ȅ[F].
GetInventory(Ⱥ);if(ƣ!=null){ȃ+=(double)ƣ.CurrentVolume;if((double)ƣ.MaxVolume>9223372036854){Ȅ=true;}else{Ȅ=false;}if(!Ȅ){ț=(double)
ƣ.MaxVolume;double ɟ=ɞ(ȅ[F]);if(ɟ>0)Ȉ=ț/ɟ;}else{ț=ɞ(ȅ[F])*10;Ȉ=9999;}if((double)ƣ.CurrentVolume<ț){if(!(ȅ[F]is
IMyShipDrill)){ȑ=false;}}else{if(ȅ[F]is IMyShipDrill){Ȑ=true;}}}ȓ+=ț;}}if(ȓ>0){Ȓ=(ȃ/ȓ)*100;}else{Ȓ=100;}ȉ=(int)Ȓ;if(ȑ&&Ȑ)ȉ=101;}
double ɠ(IMyTerminalBlock ɝ){double ț=-1;var ũ=ɝ.InventoryCount;for(var Ⱥ=0;Ⱥ<ũ;Ⱥ++){var ƣ=ɝ.GetInventory(Ⱥ);if(ƣ!=null){ȃ+=(
double)ƣ.CurrentVolume;if((double)ƣ.MaxVolume>9223372036854){Ȅ=true;}else{Ȅ=false;}if(!Ȅ){ț=(double)ƣ.MaxVolume;double ɟ=ɞ(ɝ);
if(ɟ>0)Ȉ=ț/ɟ;}else{ț=ɞ(ɝ)*10;Ȉ=9999;}}}return ț;}double ɞ(IMyTerminalBlock ɝ){var ƣ=ɝ.GetInventory(0);string ɜ=ɝ.
BlockDefinition.SubtypeId;double ț=(double)ƣ.MaxVolume;if(ț<999999999)return ț;if(ɝ is IMyCargoContainer){if(ɜ.Contains(
"LargeBlockLargeContainer"))ț=421.875008;else if(ɜ.Contains("LargeBlockSmallContainer"))ț=15.625;else if(ɜ.Contains("SmallBlockLargeContainer"))ț=
15.625;else if(ɜ.Contains("SmallBlockMediumContainer"))ț=3.375;else if(ɜ.Contains("SmallBlockSmallContainer"))ț=0.125;else if(
ɜ.Contains("Azimuth_LargeContainer"))ț=7780.8;else if(ɜ.Contains("Azimuth_MediumLargeContainer"))ț=1945.2;else if(ɜ.
Contains("Azimuth_MediumContainer"))ț=1878.6;else if(ɜ.Contains("Azimuth_SmallContainer"))ț=10.125;}else if(ɜ.Contains(
"SmallBlockDrill"))ț=3.375;else if(ɜ.Contains("LargeBlockDrill"))ț=23.4375;else if(ɜ.Contains("ConnectorMedium"))ț=1.152;else if(ɜ.
Contains("ConnectorSmall"))ț=0.064;else if(ɜ.Contains("Connector"))ț=8.000;else if(ɜ.Contains("LargeShipWelder"))ț=15.625;else
if(ɜ.Contains("LargeShipGrinder"))ț=15.625;else if(ɜ.Contains("SmallShipWelder"))ț=3.375;else if(ɜ.Contains(
"SmallShipGrinder"))ț=3.375;else{Echo("Unknown cargo for default Capacity:"+ɝ.DefinitionDisplayNameText+":"+ɝ.BlockDefinition.SubtypeId);ț
=12;}return ț;}const int ɛ=0;const int ɚ=2;const int ɡ=4;const int ɢ=8;const int ɳ=16;const int ɴ=32;const int ɲ=64;const
int ɱ=128;const int ɰ=256;const int ɯ=512;const int ɮ=1024;const int ɭ=2048;const int ɬ=0xfff;string ɫ(){string ɪ="FLAGS:";
if((ς&ɚ)>0)ɪ+="SLED ";if((ς&ɴ)>0)ɪ+="ORBITAL ";if((ς&ɲ)>0)ɪ+="ROCKET ";if((ς&ɡ)>0)ɪ+="ROTOR ";if((ς&ɢ)>0)ɪ+="WHEEL ";if((ς
&ɱ)>0)ɪ+="PET ";if((ς&ɰ)>0)ɪ+="NAD ";if((ς&ɯ)>0)ɪ+="NO Gyro ";if((ς&ɭ)>0)ɪ+="No Tank ";if((ς&ɮ)>0)ɪ+="No Power ";return ɪ
;}long ɩ=0;MyIni ɨ=new MyIni();string ɧ="";string ɦ="";void ɥ(){if(ν==null){ɦ=Storage;}else{ɦ=ν.GetText();}if(υ==null)
return;if(ɦ==ɧ){Echo("Load Skip");return;}ɧ=ɦ;ɦ=ɦ.Trim();MyIniParseResult ɤ;if(!ɨ.TryParse(ɦ,out ɤ)){}υ.Ǐ(ɦ);υ.Ǚ(ϛ,"SaveID",
ref ɩ);if(ɣ()){υ.Ǐ("");}ʓ(υ);υ.Ǚ(ϛ,"Mode",ref κ,true);υ.Ǚ(ϛ,"current_state",ref τ,true);υ.Ǚ(ϛ,"PassedArgument",ref ρ,true);
υ.Ǚ(ϛ,"AlertStates",ref π,true);υ.Ǚ(ϛ,"craft_operation",ref ς,true);}bool ɣ(){if(ν==null||ύ)return false;if(ɩ<=0||ɩ==(
long)ν.EntityId)return false;else return true;}bool ə(string ɘ){ɘ=ɘ.Trim().ToLower();return(ɘ=="True"||ɘ=="true");}
Dictionary<long,MyDetectedEntityInfo>Ȼ=new Dictionary<long,MyDetectedEntityInfo>();void ɉ(MyDetectedEntityInfo Ɉ){if(Ɉ.EntityId!=0
){if(!Ȼ.ContainsKey(Ɉ.EntityId)){Ȼ.Add(Ɉ.EntityId,Ɉ);}else{Ȼ[Ɉ.EntityId]=Ɉ;}}else Echo("Not adding: Zero Entity");}string
ɇ(MyDetectedEntityInfo Ɇ){string è="";è+="ETBV";è+=":"+Ɇ.EntityId.ToString();è+=":"+Ɇ.TimeStamp;Vector3D Ʌ=Ɇ.BoundingBox.
Min;è+=":"+Ɩ(Ʌ);Vector3D Ʉ=Ɇ.BoundingBox.Max;è+=":"+Ɩ(Ʉ);Vector3D Ƀ=(Vector3)Ɇ.Velocity;è+=":"+Ɩ(Ƀ);return è;}List<Ɂ>ɂ=new
List<Ɂ>();class Ɂ{public IMyTerminalBlock ɀ;public long ȿ;public long Ⱦ;public List<IMyTerminalBlock>Ƚ;public long ȼ;public
string Ɋ;}List<IMyTerminalBlock>Ɍ=new List<IMyTerminalBlock>();IMyBroadcastListener ɖ;IMyBroadcastListener ɗ;
IMyBroadcastListener ɕ;string ɔ(){string è="";ɕ=IGC.RegisterBroadcastListener("BASE?");ɕ.SetMessageCallback(ɕ.Tag);ɖ=IGC.
RegisterBroadcastListener("CON?");ɖ.SetMessageCallback(ɖ.Tag);ɗ=IGC.RegisterBroadcastListener("COND?");ɗ.SetMessageCallback(ɗ.Tag);ɂ.Clear();Ɍ=ŀ<
IMyLightingBlock>(Ç);if(É.Count<1)è+=Ã();Echo(É.Count+" Base Connectors");for(int F=0;F<É.Count;F++){ɓ(É[F]);}è+="DI:"+ɂ.Count;return è;
}void ɓ(IMyTerminalBlock ɀ,long ȿ=0){List<IMyTerminalBlock>Ƚ=new List<IMyTerminalBlock>();Ɂ ɒ=new Ɂ();ɒ.ɀ=ɀ;ɒ.ȿ=ȿ;ɒ.Ⱦ=0;ɒ
.ȼ=-1;for(int F=0;F<Ɍ.Count;F++){double ɑ=(Ɍ[F].GetPosition()-ɒ.ɀ.GetPosition()).Length();if(ɑ<3){Ƚ.Add(Ɍ[F]);}}ɒ.Ƚ=Ƚ;
string ɐ=ɀ.CustomData;string[]ɏ=ɐ.Trim().Split('\n');Echo(ɏ.Length+" Lines");for(int F=0;F<ɏ.Length;F++){Echo("|"+ɏ[F].Trim())
;string[]Ɏ=ɏ[F].Trim().Split('=');if(ɏ[F].ToLower().Contains("align")){if(Ɏ.Length>1){long ȫ;if(long.TryParse(Ɏ[1],out ȫ)
)ɒ.ȼ=ȫ;else Echo("Error Converting"+Ɏ[1]);}else Echo("Error parsing");}}ɂ.Add(ɒ);}Color ˎ=new Color(0f,1.0f,0.0f);Color Ψ
=new Color(0f,0f,1.0f);Color ϳ=new Color(1f,0f,0f);void ϱ(){if(ɂ.Count==0){return;}string ğ="Docking Info:\n";foreach(var
ɒ in ɂ){Color ϰ=ˎ;float ϯ=0;ğ+=Φ("",ɒ.ɀ.CustomName)+":";switch(ɒ.ȿ){case 0:if(ɒ.ɀ is IMyShipConnector){IMyShipConnector Ϯ
=ɒ.ɀ as IMyShipConnector;if(Ϯ.Status==MyShipConnectorStatus.Connectable){ϰ=Ψ;ğ+="In Range";}else if(Ϯ.Status==
MyShipConnectorStatus.Connected){ϰ=Ψ;ğ+="CONNECTED";}else{ğ+="Available";}}break;case 1:ğ+="Player Reserved";ϰ=Ψ;break;case 2:ğ+=
"Incoming Ship";ϰ=ϳ;ϯ=0.5f;if(ɒ.ɀ is IMyShipConnector){IMyShipConnector é=ɒ.ɀ as IMyShipConnector;if(é.Status==MyShipConnectorStatus.
Connected)ɒ.ȿ=0;if(é.Status==MyShipConnectorStatus.Connectable)ɒ.ȿ=0;}break;default:Echo("unkonwn docking state");break;}foreach(
var ɀ in ɒ.Ƚ){if(ɀ is IMyLightingBlock){IMyLightingBlock ϭ=ɀ as IMyLightingBlock;ϭ.Color=ϰ;ϭ.BlinkIntervalSeconds=ϯ;}}ğ+=
"\n";}Echo(ğ);}int Ϭ(long ϥ,string Ϫ,double ϩ,double Ϩ,double ϧ){int ϲ=-1;for(int F=0;F<ɂ.Count;F++){if(ɂ[F].ɀ is
IMyShipConnector){IMyShipConnector é=ɂ[F].ɀ as IMyShipConnector;if(ɂ[F].Ⱦ==ϥ){if(é.Status==MyShipConnectorStatus.Connected){ɂ[F].Ⱦ=0;
continue;}if(é.Status==MyShipConnectorStatus.Connectable){ɂ[F].Ⱦ=0;continue;}ϲ=F;break;}}}if(ϲ<0){for(int F=0;F<ɂ.Count;F++){if(
ɂ[F].ɀ is IMyShipConnector){IMyShipConnector é=ɂ[F].ɀ as IMyShipConnector;if(ɂ[F].ȿ==0){if(é.Status==
MyShipConnectorStatus.Connected)continue;if(é.Status==MyShipConnectorStatus.Connectable)continue;ϲ=F;break;}else if(ɂ[F].ȿ==1)continue;else
if(ɂ[F].ȿ==2)continue;}}}return ϲ;}void ϻ(int ϲ,long ϥ,string w,bool Ϻ=false){if(ϲ<0||ϲ>=ɂ.Count)return;IMyTerminalBlock é
=ɂ[ϲ].ɀ;Vector3D r=é.GetPosition();MatrixD Ϲ=é.WorldMatrix;Vector3D ſ=Ϲ.Forward;ſ.Normalize();ɂ[ϲ].ȿ=2;ɂ[ϲ].Ⱦ=ϥ;ɂ[ϲ].Ɋ=w;
Vector3D ϸ;MatrixD Ϸ;if(ǵ!=null)Ϸ=ǵ.WorldMatrix;else Ϸ=Me.WorldMatrix;ϸ=Ϸ.Forward;switch(ɂ[ϲ].ȼ){case 0:break;case 1:ϸ=Ϸ.Up;
break;case 2:ϸ=Ϸ.Down;break;case 3:ϸ=Ϸ.Left;break;case 4:ϸ=Ϸ.Right;break;case 5:ϸ=Ϸ.Backward;break;}ϸ.Normalize();if(Ϻ){
Vector3D ϵ=r+ſ*30;Ŝ("CONA",ϥ+":"+é.EntityId+":"+Ɩ(ϵ));}else{if(ɂ[ϲ].ȼ<0){Ŝ("COND",ϥ+":"+é.EntityId+":"+Φ("",é.CustomName)+":"+Ɩ(
r)+":"+Ɩ(ſ));}else{Ŝ("ACOND",ϥ+":"+é.EntityId+":"+Φ("",é.CustomName)+":"+Ɩ(r)+":"+Ɩ(ſ)+":"+Ɩ(ϸ));}}}bool ϴ(){if(ɕ.
HasPendingMessage){Echo("Base Request");var L=ɕ.AcceptMessage();string o=(string)L.Data;string[]µ=o.Trim().Split(':');long ϥ=0;bool Ȁ=
false;Ȁ=long.TryParse(µ[0],out ϥ);ʻ(true);}if(ɖ.HasPendingMessage){Echo("Connector Approach Request!");var L=ɖ.AcceptMessage(
);string o=(string)L.Data;string[]µ=o.Trim().Split(':');int ª=0;bool Ȁ=false;long ń=0;Ȁ=long.TryParse(µ[ª++],out ń);if(ń
!=ν.EntityId){Echo("Not our approach request");return false;}string Ϫ=µ[ª++];double ϩ=-1;double Ϩ=-1;double ϧ=-1;string[]Ϧ
=Ϫ.Trim().Split(',');if(Ϧ.Length>2){Ȁ=double.TryParse(Ϧ[0],out ϩ);Ȁ=double.TryParse(Ϧ[1],out Ϩ);Ȁ=double.TryParse(Ϧ[2],
out ϧ);}string Ɋ=µ[ª++];int F=-1;long ϥ=0;Ȁ=long.TryParse(µ[ª++],out ϥ);F=Ϭ(ϥ,Ϫ,ϩ,Ϩ,ϧ);if(F>=0&&Ȁ){Echo("Sending Dock Info"
);ϻ(F,ϥ,Ɋ,true);}else{Echo("Sending Dock Fail");Ŝ("CONF",ϥ+":"+Me.CubeGrid.CustomName+":"+ν.EntityId.ToString()+":"+Ɩ(ǵ.
GetPosition()));}}if(ɗ.HasPendingMessage){Echo("Connector Dock Request!");var L=ɗ.AcceptMessage();string o=(string)L.Data;string[]µ
=o.Trim().Split(':');int ª=0;bool Ȁ=false;long ń=0;Ȁ=long.TryParse(µ[ª++],out ń);if(ń!=ν.EntityId){return false;}string Ϫ
=µ[ª++];double ϩ=-1;double Ϩ=-1;double ϧ=-1;string[]Ϧ=Ϫ.Trim().Split(',');if(Ϧ.Length>2){Ȁ=double.TryParse(Ϧ[0],out ϩ);Ȁ=
double.TryParse(Ϧ[1],out Ϩ);Ȁ=double.TryParse(Ϧ[2],out ϧ);}string Ɋ=µ[ª++];int F=-1;long ϥ=0;Ȁ=long.TryParse(µ[ª++],out ϥ);F=Ϭ
(ϥ,Ϫ,ϩ,Ϩ,ϧ);if(F>=0&&Ȁ){ϻ(F,ϥ,Ɋ);}else{Ŝ("CONF",ϥ+":"+Me.CubeGrid.CustomName+":"+ν.EntityId.ToString()+":"+Ɩ(Ř()));}}
return false;}bool ϫ(string ʷ){string[]µ=ʷ.Trim().Split(':');if(µ.Length>1){if(µ[0]!="WICO"){Echo("not wico system message");
return false;}if(µ.Length>2){if(µ[1]=="DOCK?"){Echo("[OBSOLETE] Docking Request!");}if(µ[1]=="CON?"){Echo(
"Connector Approach Request!");bool Ȁ=false;long ń=0;Ȁ=long.TryParse(µ[2],out ń);if(ń!=ν.EntityId){return false;}string Ϫ=µ[3];double ϩ=-1;double Ϩ=-
1;double ϧ=-1;string[]Ϧ=Ϫ.Trim().Split(',');if(Ϧ.Length>2){Ȁ=double.TryParse(Ϧ[0],out ϩ);Ȁ=double.TryParse(Ϧ[1],out Ϩ);Ȁ=
double.TryParse(Ϧ[2],out ϧ);}string Ɋ=µ[4];ʷ="";int F=-1;long ϥ=0;Ȁ=long.TryParse(µ[5],out ϥ);F=Ϭ(ϥ,Ϫ,ϩ,Ϩ,ϧ);if(F>=0&&Ȁ){ϻ(F,ϥ
,Ɋ,true);}else{Ŝ("CONF",ϥ+":"+Me.CubeGrid.CustomName+":"+ν.EntityId.ToString()+":"+Ɩ(ǵ.GetPosition()));}return true;}if(µ
[1]=="COND?"){Echo("Connector Dock Request!");bool Ȁ=false;long ń=0;Ȁ=long.TryParse(µ[2],out ń);if(ń!=ν.EntityId){return
false;}ʷ="";string Ϫ=µ[3];double ϩ=-1;double Ϩ=-1;double ϧ=-1;string[]Ϧ=Ϫ.Trim().Split(',');if(Ϧ.Length>2){Ȁ=double.TryParse(
Ϧ[0],out ϩ);Ȁ=double.TryParse(Ϧ[1],out Ϩ);Ȁ=double.TryParse(Ϧ[2],out ϧ);}string Ɋ=µ[4];int F=-1;long ϥ=0;Ȁ=long.TryParse(
µ[5],out ϥ);F=Ϭ(ϥ,Ϫ,ϩ,Ϩ,ϧ);if(F>=0&&Ȁ){ϻ(F,ϥ,Ɋ);}else{Ŝ("CONF",ϥ+":"+Me.CubeGrid.CustomName+":"+ν.EntityId.ToString()+":"
+Ɩ(Ř()));}return true;}if(µ[1]=="BASE?"){Echo("Base Request!");ʷ="";long ϥ=0;bool Ȁ=false;Ȁ=long.TryParse(µ[3],out ϥ);ʻ(
true);return true;}}}return false;}void Й(){Echo("mode="+κ.ToString());Щ();if(κ==Β)Ϙ();}void Щ(){ϱ();ʻ();Ą();}void Ъ(){Ѐ(
DateTime.Now.ToString()+" ACTION: Reset To Idle",С,true);ʏ();Ή(λ);}void Ш(){Ѐ(ʕ+" Manual Control",П);}string Ч="LOGGING";void Ц(
ǲ W){W.Ǚ(Ч,"TextPanelReport",ref О,true);W.Ǚ(Ч,"StatusName",ref Т,true);W.Ǚ(Ч,"LongStatus",ref Р,true);W.Ǚ(Ч,
"RangeReport",ref Ф,true);W.Ǚ(Ч,"SledReport",ref К,true);W.Ǚ(Ч,"GPSTag",ref М,true);}ϼ Х=null;string Ф="[RANGE]";ϼ У=null;string Т=
"Wico Craft Status";ϼ С=null;string Р="Wico Craft Log";ϼ П=null;string О="Craft Report";ϼ Н=null;string М="[GPS]";ϼ Л=null;string К=
"[SMREPORT]";bool И=false;bool З=false;class ϼ{Program Ǥ;string Ї="";List<IMyTextPanel>І=new List<IMyTextPanel>();string Ѕ="";string
Є="";bool Ѓ=false;bool Ђ=true;public ϼ(Program ǐ,string w,bool Ё=false){Ǥ=ǐ;Ї=w;Ѓ=Ё;Ђ=true;Ѕ="";Є="";І.Clear();І=Ǥ.ǧ(Ї);
if(І.Count<1)І=Ǥ.Ǩ(Ї);}public void Ѐ(string Ͽ,bool Ͼ=false){if(Ͽ=="clear"){Ѕ="";Є="X";Ђ=false;return;}if(Ѓ&&Ђ){Ђ=false;if(
І.Count>0){Ѕ=І[0].GetText();Є="X";}}if(Ͼ){Ѕ=Ͽ+"\n"+Ѕ;}else Ѕ+=Ͽ+"\n";}public void Ͻ(){if(Є!=Ѕ){Ђ=true;foreach(var Ј in І)
{Ј.WriteText(Ѕ);}Є=Ѕ;}}}void Љ(){У=Б(true);С=Д(Р,true);;П=Д(О);Х=Д(Ф);Н=Д(М,ύ);Л=Д(К);И=true;}void Е(){if(У!=null)Ѐ(
"clear",У);if(С!=null)Ѐ("clear",С);if(П!=null)Ѐ("clear",П);if(Х!=null)Ѐ("clear",Х);if(Н!=null)Ѐ("clear",Н);if(Л!=null)Ѐ("clear"
,Л);}void Ж(){if(У!=null)У.Ͻ();if(С!=null)С.Ͻ();if(П!=null)П.Ͻ();if(Х!=null)Х.Ͻ();if(Н!=null)Н.Ͻ();if(Л!=null)Л.Ͻ();}ϼ Д(
string Г,bool Ё=false){ϼ В=new ϼ(this,Г,Ё);return В;}ϼ Б(bool А=false){if((У!=null||И)&&!А)return У;У=Д(Т);return У;}void Ѐ(
string Ͽ,ϼ Џ,bool Ͼ=false){if(Џ==null)return;Џ.Ѐ(Ͽ,Ͼ);}void Ў(string Ͽ){Ѐ(Ͽ,Б());if(З&&Ͽ!="clear")Echo(Ͽ);}string Ѝ(double Ќ){
int Ћ=75;if(Ќ<0)Ќ=0;int Њ=(int)(Ќ*Ћ)/100;if(Њ>Ћ)Њ=Ћ;string ɪ="["+new String('|',Њ)+new String('\'',Ћ-Њ)+"]";return ɪ;}void
Ϥ(string w,Vector3D r){string Ķ;Ķ="GPS:"+w+":"+Ɩ(r)+":";Ѐ(Ķ,Н);}string Φ(string Υ,string Τ){string è;int Σ=Υ.Length;int Ρ
=Τ.Length;if(Σ+Ρ>32){if(Ρ>31)return"INVALID";Σ=32-Ρ;}è=Υ.Substring(0,Σ)+Τ;è.Replace(":","_");è.Replace(";","_");return è;
}string Π(double Ο){string Ξ="";if(Ο>1000){Ξ=Ο.ToString("N0")+"km";}else if(Ο>10){Ξ=Ο.ToString("0.0")+"m";}else{Ξ=Ο.
ToString("0.000")+"m";}return Ξ;}void Ν(){}bool Μ=false;void Λ(ǲ Κ){Ä(Κ);ʞ(Κ);ʾ(Κ);Κ.Ǚ("PATROL","AllowPatrol",ref Μ,true);}
string Χ(){Echo(ʕ+" Init:"+ę);if(ę==0){Ѐ(DateTime.Now.ToString()+ʖ+":"+ʕ+":INIT",С,true);if(!Ϗ.ContainsKey("doscan"))Ϗ.Add(
"doscan",Β);Ě+=Ì();Љ();ó();Ě+=ο();ɥ();Ě+=Ǵ();Ě+=ć();if(!đ()){ė=true;Ę+="\nNo Antenna Available";}}else if(ę==1){ʛ();Ű();Ě+=ʟ(ǵ);
Ě+=Ã();Ě+=ɔ();if(É.Count<1)Ę+="\nNo [BASE] Connectors found";Ě+=Ω();Ĝ=true;}ę++;if(Ĝ)ę=0;Ў(Ě);return Ě;}string Ω(){return
">";}int κ=-1;const int λ=0;const int ι=1;const int θ=2;const int η=3;const int ζ=4;const int ε=5;const int δ=7;const int γ
=8;const int β=9;const int α=13;const int ΰ=10;const int ί=11;const int ή=12;const int έ=14;const int ά=15;const int Ϋ=16
;const int Ϊ=17;const int Θ=18;const int ͷ=19;const int Ζ=20;const int ʹ=21;const int ͳ=22;const int Ͳ=23;const int ͱ=24;
const int Ͱ=25;const int ˮ=26;const int Ͷ=27;const int ˬ=28;const int ˣ=29;const int ˢ=30;const int ˡ=31;const int ˠ=33;const
int ˑ=50;const int ː=60;const int ˤ=111;const int ˏ=112;const int ͺ=200;const int Η=210;const int Ε=220;const int Δ=225;
const int Γ=290;const int Β=400;const int Α=410;const int ΐ=500;const int Ώ=510;const int Ύ=600;const int Ό=610;const int Ί=
999;void Ή(int Έ){if(κ==Έ)return;κ=Έ;τ=0;ò();}ʪ Ά;ʪ ͽ;ʪ ͼ;ʪ Ι;ʪ ͻ;ʪ μ;double Ϛ=0;void Ϙ(){Ѐ("clear",П);Ѐ(ʕ+":SCAN!",П);Echo
("Scan:current_state="+τ.ToString());switch(τ){case 0:{ʏ();Ϛ=0;Ά=new ʪ(this,ˋ,5000);ͽ=new ʪ(this,ˊ,5000);ͼ=new ʪ(this,ˇ,
5000);Ι=new ʪ(this,ˆ,5000);ͻ=new ʪ(this,ˈ,5000);μ=new ʪ(this,ˉ,5000);τ=410;Ė=true;break;}case 410:{Ѐ("Long Range Scan",П);if
(Ά==null){Ė=true;τ=0;return;}ĕ=true;Ϛ+=Runtime.TimeSinceLastRun.TotalMilliseconds;if(Ά.ȏ()){ʈ(Ά.ȹ);}if(ͽ.ȏ()){ʈ(ͽ.ȹ);}if(
ͼ.ȏ()){ʈ(ͼ.ȹ);}if(Ι.ȏ()){ʈ(Ι.ȹ);}if(ͻ.ȏ()){ʈ(ͻ.ȹ);}if(μ.ȏ()){ʈ(μ.ȹ);}string è="";è+="Front: ";if(Ά.ȯ())è+="DONE!";else{è
+=Ά.Ȩ.ToString("0")+" meters";}è+=" "+Ά.ȹ.Count+" objects";è+="\n";è+="Back: ";if(ͽ.ȯ())è+="DONE!";else{è+=ͽ.Ȩ.ToString(
"0")+" meters";}è+=" "+ͽ.ȹ.Count+" objects";è+="\n";è+="Left: ";if(ͼ.ȯ())è+="DONE!";else{è+=ͼ.Ȩ.ToString("0")+" meters";}è
+=" "+ͼ.ȹ.Count+" objects";è+="\n";è+="Right: ";if(Ι.ȯ())è+="DONE!";else{è+=Ι.Ȩ.ToString("0")+" meters";}è+=" "+Ι.ȹ.Count+
" objects";è+="\n";è+="Top: ";if(ͻ.ȯ())è+="DONE!";else{è+=ͻ.Ȩ.ToString("0")+" meters";}è+=" "+ͻ.ȹ.Count+" objects";è+="\n";è+=
"Bottom: ";if(μ.ȯ())è+="DONE!";else{è+=μ.Ȩ.ToString("0")+" meters";}è+=" "+μ.ȹ.Count+" objects";è+="\n";if(ɿ()<0)è+=
"No Known Asteroid";else è+="FOUND at least one asteroid!";Ѐ(è,П);Echo(è);if(Ά.ȯ()&&ͽ.ȯ()&&ͼ.ȯ()&&Ι.ȯ()&&ͻ.ȯ()&&μ.ȯ()){Ή(ư);τ=Ư;ư=Α;Ư=0;}
break;}}}bool ϗ(string İ){if(İ==""||İ=="timer"||İ=="wccs"||İ=="wcct"){if(ρ!=""&&ρ!="timer"){Echo("Using Passed Arg="+ρ);İ=ρ;}
}if(İ=="init"){Ě="";Ĝ=false;ę=0;Χ();return false;}string[]ϖ=İ.Trim().Split(' ');if(ϖ[0]=="trigger"){Echo("trigger");if(ϖ.
Length>1){string ϕ="WICO:TRIGGER:";ϕ+=ϖ[1].Trim();Ŝ("TRIGGER",ϖ[1].Trim());}else Echo("Incomplete command");}else if(ϖ[0]==
"namecameras"){ʯ(ˋ,"Front");ʯ(ˊ,"Back");ʯ(ˉ,"Down");ʯ(ˈ,"Up");ʯ(ˇ,"Left");ʯ(ˆ,"Right");}else{int ϔ;if(Ϗ.TryGetValue(ϖ[0].ToLower(),
out ϔ)){Ή(ϔ);}else{}}return false;}bool ϓ(string İ){if(ϫ(İ))return true;if(ʇ(İ))return true;if(Ơ(İ))return true;if(ϙ(İ))
return true;if(ϑ(İ))return true;return false;}bool ϒ(string İ){Ę+="IGC:"+İ;ϴ();return false;}bool ϑ(string ʷ){ʷ=ʷ.Trim();
string[]µ=ʷ.Trim().Split(':');if(µ.Length>0){if(µ[0]!="WICO"){Echo("not wico system message");return false;}if(µ.Length>1){if(
µ[1]=="TRIGGER"&&µ.Length>2){bool Ă=ø(µ[2]);return Ă;}else{}}}return false;}bool ϙ(string ʷ){ʷ=ʷ.Trim();string[]µ=ʷ.Trim(
).Split(':');Echo(µ.Length+" Parts");if(µ.Length>1){if(µ[0]!="WICO"){Echo("not wico system message");return false;}if(µ.
Length>2){if(µ[1]=="PATROL"){Echo("Patrol waypoint Request!");if(!Μ){Echo("Patrol is turned off");return false;}
IMyRemoteControl ϣ;ϣ=ǵ as IMyRemoteControl;if(ϣ==null){return false;}ϣ.ClearWaypoints();ϣ.FlightMode=FlightMode.Circle;ϣ.SetDockingMode(
false);ϣ.SetCollisionAvoidance(false);bool Ȁ=false;int Ϣ=0;Ȁ=int.TryParse(µ[2],out Ϣ);int ϡ=3;for(int Ϡ=0;Ϡ<Ϣ;Ϡ++){{double ä,
Ɯ,ƛ;try{bool ƙ=double.TryParse(µ[ϡ++].Trim(),out ä);bool Ƙ=double.TryParse(µ[ϡ++].Trim(),out Ɯ);bool Ɨ=double.TryParse(µ[
ϡ++].Trim(),out ƛ);if(!ƙ||!Ƙ||!Ɨ){Echo("Invalid Command:("+µ+")");continue;}ϣ.AddWaypoint(new Vector3D(ä,Ɯ,ƛ),"Patrol"+Ϡ)
;}catch{Echo("Invalid message");return false;}}}ITerminalAction ϟ;ϟ=ϣ.GetActionWithName("planepilot_onoff_on_Action");if(
ϟ!=null){ϟ.Apply(ϣ);}else{ϣ.SetAutoPilotEnabled(true);}return true;}}}return false;}void Ϟ(){if(υ==null)return;ʒ(υ);υ.Ɗ(ϛ
,"Mode",κ.ToString());υ.Ɗ(ϛ,"current_state",τ.ToString());υ.Ɗ(ϛ,"PassedArgument",ρ);υ.Ɗ(ϛ,"AlertStates",π.ToString());υ.Ɗ
(ϛ,"craft_operation",ς.ToString());long ϝ=0;if(ν!=null)ϝ=ν.EntityId;υ.Ɗ(ϛ,"SaveID",(long)ϝ);if(υ.Ǒ){if(υ.Ǒ){string ǎ=υ.Ƅ(
);if(ν==null){Echo("WARNING: saving to Storage");Storage=ǎ;}else{ν.WriteText(ǎ,false);}}}else{Echo("Not saving: Same");}}
string Ϝ="Wico Craft Save";string ϛ="WCCM2";void ϐ(ǲ W){W.Ǚ(ϛ,"SAVE_FILE_NAME",ref Ϝ,true);}IMyTextPanel ν=null;ǲ υ=null;int τ
=0;long σ=0;int ς=ɛ;string ρ="";int π=0;string ο(){string Ě="S";ν=null;List<IMyTerminalBlock>ď=new List<IMyTerminalBlock>
();ď=ǩ<IMyTextPanel>(Ϝ);if(ď.Count>1){ė=true;Ę+="\nMultiple blocks found:\""+Ϝ+"\"";}else if(ď.Count==0){ď=ŀ<IMyTextPanel
>(Ϝ);if(ď.Count==1)ν=ď[0]as IMyTextPanel;else{ď=ǥ<IMyTextPanel>(Ϝ);if(ď.Count==1)ν=ď[0]as IMyTextPanel;}}else ν=ď[0]as
IMyTextPanel;υ=new ǲ(this,"");if(ν==null){Ě="-";}return Ě;}bool ξ(){return ν!=null;}string Ɩ(Vector3D ƕ){string è;è=ƕ.X.ToString(
"0.00")+":"+ƕ.Y.ToString("0.00")+":"+ƕ.Z.ToString("0.00");return è;}bool Ɲ(string ƞ,out double ä,out double Ɯ,out double ƛ){
string[]ƚ=ƞ.Trim().Split(',');if(ƚ.Length<3){ƚ=ƞ.Trim().Split(':');}ä=0;Ɯ=0;ƛ=0;if(ƚ.Length<3)return false;bool ƙ=double.
TryParse(ƚ[0].Trim(),out ä);bool Ƙ=double.TryParse(ƚ[1].Trim(),out Ɯ);bool Ɨ=double.TryParse(ƚ[2].Trim(),out ƛ);if(!ƙ||!Ƙ||!Ɨ){
return false;}return true;}Dictionary<string,int>Ϗ=new Dictionary<string,int>();string Ĩ="";UpdateFrequency ώ=UpdateFrequency.
Once;bool ύ=true;bool ό=false;float ϋ=100;string ϊ="WORLD";void ω(ǲ W){W.Ǚ(ϊ,"MaxWorldMps",ref ϋ,true);}string ψ="WICOCRAFT"
;void χ(bool φ=false){ǲ Κ=new ǲ(this,Me.CustomData);Κ.Ǚ(ψ,"EchoOn",ref ȁ,true);Κ.Ǚ(ψ,"DebugUpdate",ref ό,true);ω(Κ);X(Κ);
Ц(Κ);õ(Κ);Λ(Κ);if(Κ.Ǒ||φ){Me.CustomData=Κ.Ƅ(true);}}bool ȁ=true;Action<string>ġ;void ƍ(string ğ){if(ȁ)ġ(ğ);}Program(){Ν()
;χ();ġ=Echo;Echo=ƍ;Ĩ=ʖ+":"+ʕ+" V"+ʔ+" ";ġ(Ĩ+"Creator");if(!Me.CustomName.Contains(ʕ))Me.CustomName="PB "+ʖ+" "+ʕ;if(!Me.
Enabled){Echo("I am turned OFF!");}IMyTextSurface Ğ=Me.GetSurface(0);IMyTextSurface ĝ=Me.GetSurface(1);Ğ.ContentType=VRage.Game
.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;Ğ.WriteText("Wicorel\n"+ʕ);Ğ.FontSize=2;Ğ.Alignment=VRage.Game.GUI.TextPanel.
TextAlignment.CENTER;ĝ.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;ĝ.WriteText("Version:"+ʔ);ĝ.Alignment=VRage.
Game.GUI.TextPanel.TextAlignment.CENTER;ĝ.TextPadding=0.25f;ĝ.FontSize=3.5f;}bool Ĝ=false;bool ě=false;string Ě="";int ę=0;
string Ę="";bool ė=false;bool Ė=false;bool ĕ=false;bool Ĕ=false;bool ē=false;double Ē=5;double Ġ=-1;double Ģ=-1;double ı=-2;
int Ĳ=0;void Main(string İ,UpdateType į){Ĳ++;Echo(Ĩ+û());if(ό)Echo(į.ToString()+":"+Ĳ.ToString());Ė=false;ĕ=false;Ĕ=false;
if(Ġ>Ē){Echo("Projector Check");Ġ=0;ē=false;var Į=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<
IMyProjector>(Į,Õ);for(int F=0;F<Į.Count;F++){if(Į[F].IsWorking){if(Į[F].CustomName.Contains("!WCC")||Į[F].CustomData.Contains(
"!WCC"))continue;Echo("Working local Projector found!");ē=true;}}}else{if(Ġ<0){Ġ=Ē+5;}Ġ+=Runtime.TimeSinceLastRun.TotalSeconds
;}if(ē)Echo("Working local Projector found!");if(İ!=""&&İ!="timer"&&İ!="wccs")Echo("Arg="+İ);if(İ=="init"){Ě="";Ĝ=false;}
if(!Ĝ){if(ē){Ѐ("clear",Д(О));Ѐ(ʕ+":Construction in Progress\nTurn off projector to continue",П);}Ė=true;Χ();if(ė)Ė=false;ě
=true;}else{if(ě)Ѐ(DateTime.Now.ToString()+" "+ʖ+":"+Ě,С,true);if(Ę!="")Echo(Ę);ɥ();if(ǵ is IMyShipController){Ģ=((
IMyShipController)ǵ).GetShipSpeed();Vector3D ĭ=((IMyShipController)ǵ).GetNaturalGravity();double Ĭ=ĭ.Length();ı=Ĭ/9.81;}if((į&(UpdateType
.Trigger|UpdateType.Terminal))>0||(į&(UpdateType.Mod))>0||(į&(UpdateType.Script))>0){if(ό)Echo("Argument="+İ);if(İ.
ToLower()=="profilerreset"){ÿ();Ė=true;}if(ϗ(İ)){ī();Ϟ();Ж();return;}}else if((į&(UpdateType.IGC))>0){if(!ϒ(İ)){}ī();Ϟ();Ж();}{
İ="";if(ė&&!ě){Ĝ=false;Ě="";}}ʑ();Й();}ī();ʐ();Ϟ();ě=false;Ж();}void ī(){UpdateFrequency Ī=UpdateFrequency.None;if(Ė){
Echo("FAST!");Ī|=ώ;}else{}if(ĕ){Echo("MEDIUM");Ī|=UpdateFrequency.Update10;}else{}if(Ĕ){Echo("SLOW");Ī|=UpdateFrequency.
Update100;}else{}Runtime.UpdateFrequency=Ī;}void ĩ(string Ĩ=null){float ħ=0;ħ=Runtime.CurrentInstructionCount/(float)Runtime.
MaxInstructionCount;if(Ĩ==null)Ĩ="Instructions=";Echo(Ĩ+" "+(ħ*100).ToString("0.00")+"%");}int Ħ=1;int ĥ=20;bool Ĥ=false;StringBuilder ģ=
new StringBuilder();void Đ(){if(Ħ<=ĥ){double ā=Runtime.LastRunTimeMs;Echo("Profiler("+Ħ+"):Add:"+ā.ToString());ģ.Append(ā.
ToString()).Append("\n");Ħ++;}else if(!Ĥ){Echo("Profiler:DISPLAY");var þ=GridTerminalSystem.GetBlockWithName("DEBUG")as
IMyTextPanel;þ?.WriteText(ģ.ToString());Ĥ=true;}}void ÿ(){Ħ=1;ģ=new StringBuilder();var þ=GridTerminalSystem.GetBlockWithName(
"DEBUG")as IMyTextPanel;þ?.WriteText("");Ĥ=false;}string[]ý={"-","\\","|","/","-","\\","|","/"};int ü=99;string û(){ü++;if(ü>=ý
.Length)ü=0;return ý[ü];}string ú="[WCCT]";string Ā="[WCCS]";string ù="[WCCM]";string ö="WICOTIMERS";void õ(ǲ W){W.Ǚ(ö,
"FastTimer",ref ú,true);W.Ǚ(ö,"SubModuleTimer",ref Ā,true);W.Ǚ(ö,"MainTimer",ref ù,true);}Dictionary<string,List<IMyTerminalBlock>>
ô=new Dictionary<string,List<IMyTerminalBlock>>();void ó(){ô.Clear();}void ò(){if(!ø(ú))ø(ù);}bool ø(string ñ="[WCCS]"){
bool Ă=false;List<IMyTerminalBlock>ď=new List<IMyTerminalBlock>();IMyTimerBlock Ď=null;if(ô.ContainsKey(ñ)){ď=ô[ñ];}else{ď=ŀ
<IMyTerminalBlock>(ñ);ô.Add(ñ,ď);}for(int F=0;F<ď.Count;F++){Ď=ď[F]as IMyTimerBlock;if(Ď!=null){if(Ď.Enabled){Ď.Trigger()
;Ă=true;}else{Echo("Timer:"+Ď.CustomName+" is OFF");}}}return Ă;}bool č=false;string Č="COMMUNICATIONS";void ċ(ǲ W){W.Ǚ(Č
,"CommunicationsStealth",ref č,false);}bool Ċ=false;List<IMyRadioAntenna>ĉ=new List<IMyRadioAntenna>();List<
IMyLaserAntenna>Ĉ=new List<IMyLaserAntenna>();string ć(){ĉ.Clear();Ĉ.Clear();à<IMyRadioAntenna>(ref ĉ);à<IMyLaserAntenna>(ref Ĉ);for(
int Ó=0;Ó<ĉ.Count;++Ó){if(ĉ[Ó].CustomName.Contains("unused")||ĉ[Ó].CustomData.Contains("unused"))continue;if(!Ċ){ʖ="Wico "+
ĉ[Ó].CustomName.Split('!')[0].Trim();Ċ=true;}}return"A"+ĉ.Count.ToString("0");}void Ć(){for(int F=0;F<ĉ.Count;F++){ĉ[F].
Enabled=true;}}void ą(){}void Ą(){}bool đ(){return true;}void ă(bool ĳ=false){if(ĉ.Count<1)ć();foreach(var ő in ĉ){ő.Radius=200
;}}void Ś(float ř=200,bool ĳ=false){if(ĉ.Count<1)ć();foreach(var ŗ in ĉ){{ŗ.Radius=ř;ŗ.Enabled=true;}}}Vector3D Ř(){if(ĉ.
Count<1)ć();foreach(var ŗ in ĉ){return ŗ.GetPosition();}Vector3D Ŗ=new Vector3D();return Ŗ;}float ŕ=float.MaxValue;void Ŕ(
bool ĳ=false,float œ=float.MaxValue){if(œ<200)œ=200;ŕ=œ;Œ(ĳ);}void Œ(bool ĳ=false){if(ĉ==null||ĉ.Count<1)ć();foreach(var ő
in ĉ){{float Ő=ő.GetMaximum<float>("Radius");if(ŕ<Ő)Ő=ŕ;ő.Radius=Ő;ő.Enabled=true;}}}int ś(){if(ĉ.Count<1)ć();return(ĉ.
Count);}void Ŝ(string ť,string Ť){Echo("AntSend:"+Ť);IGC.SendBroadcastMessage(ť,Ť);}void Ŝ(long Ŧ,string ť,string Ť){IGC.
SendUnicastMessage(Ŧ,ť,Ť);}List<š>ţ=new List<š>();const string Ţ="BASE1.0";class š{public long Ļ;public string ĺ;public Vector3D ľ;public
bool q;}IMyBroadcastListener Š;void ş(){ţ.Clear();ŝ();Š=IGC.RegisterBroadcastListener("BASE");Š.SetMessageCallback(Š.Tag);}
void Ş(){if(υ==null)return;υ.Ɗ(Ţ,"count",ţ.Count);for(int Ó=0;Ó<ţ.Count;Ó++){υ.Ɗ(Ţ,"ID"+Ó.ToString(),ţ[Ó].Ļ);υ.Ɗ(Ţ,"name"+Ó.
ToString(),ţ[Ó].ĺ);υ.Ɗ(Ţ,"position"+Ó.ToString(),ţ[Ó].ľ);υ.Ɗ(Ţ,"Jumpable"+Ó.ToString(),ţ[Ó].q);}}int ŝ(){if(υ==null){return-2;}
int ŏ=-1;long Ŏ=0;string Ĵ="";Vector3D ľ=new Vector3D();bool q=false;υ.Ǚ(Ţ,"count",ref ŏ);for(int Ľ=0;Ľ<ŏ;Ľ++){υ.Ǚ(Ţ,"ID"+Ľ
.ToString(),ref Ŏ);υ.Ǚ(Ţ,"name"+Ľ.ToString(),ref Ĵ);υ.Ǚ(Ţ,"position"+Ľ.ToString(),ref ľ);υ.Ǚ(Ţ,"Jumpable"+Ľ.ToString(),
ref q);š Í=new š{Ļ=Ŏ,ĺ=Ĵ,ľ=ľ,q=q};ţ.Add(Í);}return ŏ;}void ļ(long Ļ,string ĺ,Vector3D Ĺ,bool q=false){if(ţ.Count<1)ş();š ĸ=
new š{Ļ=Ļ,ĺ=ĺ,ľ=Ĺ,q=q};for(int F=0;F<ţ.Count;F++){if(ţ[F].Ļ==Ļ){ţ[F].ĺ=ĺ;ţ[F].ľ=Ĺ;ţ[F].q=q;return;}}ţ.Add(ĸ);Ş();}string ķ(
){string Ķ;if(ţ.Count==0)return"No Known Bases";if(ţ.Count>1)Ķ=ţ.Count.ToString()+" Known Bases\n";else Ķ=ţ.Count.
ToString()+" Known Base\n";for(int F=0;F<ţ.Count;F++){Ķ+=ţ[F].ĺ+":";Ķ+=Ɩ(ţ[F].ľ)+":";Ķ+="\n";}return Ķ;}double ĵ=25;double Ŀ=-1;
void Ł(bool ō=false){string w=Me.CubeGrid.CustomName;Vector3D r=Me.GetPosition();if(ō){ţ.Clear();Ş();}if(ǵ!=null){w=ǵ.
CubeGrid.CustomName;r=ǵ.GetPosition();}if(Ŀ>ĵ||ō){Ŀ=0;Ŝ("BASE?",w+":"+ν.EntityId.ToString()+":"+Ɩ(r));}else{if(Ŀ<0){Ŀ=Me.
EntityId%ĵ;}if(ţ.Count<1)Ŀ+=Runtime.TimeSinceLastRun.TotalSeconds;}}float Ō(){double ŋ=double.MaxValue;int ŉ=Ņ(Ŋ());if(ŉ>=0&&ǵ!=
null){ŋ=(ǵ.GetPosition()-ţ[ŉ].ľ).Length();}return(float)ŋ;}long Ŋ(){int ŉ=-1;if(ǵ==null)return ŉ;double ň=double.MaxValue;
for(int F=0;F<ţ.Count;F++){double Ň=Vector3D.DistanceSquared(ţ[F].ľ,ǵ.GetPosition());if(Ň<ň){ŉ=F;ň=Ň;}}if(ŉ<0)return 0;else
return ţ[ŉ].Ļ;}long ņ(){return Ŋ();}int Ņ(long ń){for(int Ó=0;Ó<ţ.Count;Ó++){if(ţ[Ó].Ļ==ń)return Ó;}return-1;}Vector3D Ń(long
Ļ){Vector3D ł=new Vector3D();for(int Ó=0;Ó<ţ.Count;Ó++)if(ţ[Ó].Ļ==Ļ)return ţ[Ó].ľ;return ł;}bool º(){if(!Š.
HasPendingMessage)return false;Echo("Base Response");var L=Š.AcceptMessage();string o=(string)L.Data;string[]µ=o.Trim().Split(':');double
n,m,k;int ª=0;string w=µ[ª++];long u=0;long.TryParse(µ[ª++],out u);n=Convert.ToDouble(µ[ª++]);m=Convert.ToDouble(µ[ª++]);
k=Convert.ToDouble(µ[ª++]);Vector3D r=new Vector3D(n,m,k);bool q=ə(µ[ª++]);ļ(u,w,r,q);return false;}bool p(string o){
double n,m,k;string[]µ=o.Trim().Split(':');if(µ.Length>1){if(µ[0]!="WICO"){Echo("not wico system message");return false;}if(µ.
Length>2){if(µ[1]=="BASE"){int ª=2;string w=µ[ª++];long u=0;long.TryParse(µ[ª++],out u);n=Convert.ToDouble(µ[ª++]);m=Convert.
ToDouble(µ[ª++]);k=Convert.ToDouble(µ[ª++]);Vector3D r=new Vector3D(n,m,k);bool q=ə(µ[ª++]);ļ(u,w,r,q);return true;}}}return
false;}List<IMyTerminalBlock>Ë=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ê=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>É=new List<IMyTerminalBlock>();bool È=false;string Ç="[BASE]";string Æ="[DOCK]";string Å="CONNECTORS";void Ä(ǲ W){W.Ǚ(Å
,"BaseConnector",ref Ç,true);W.Ǚ(Å,"DockConnector",ref Æ,true);}string Ã(){È=false;Ë.Clear();Ê.Clear();É.Clear();Â();
return"CL"+Ë.Count.ToString()+"CD"+Ê.Count.ToString()+"CB"+É.Count.ToString();}void Â(){if(Ë.Count<1&&!È)Ë=à<IMyShipConnector>
();if(Ê.Count<1&&!È)Ê=ŀ<IMyShipConnector>(Æ);if(Ê.Count<1&&!È)Ê=Ë;if(É.Count<1&&!È)É=ŀ<IMyShipConnector>(Ç);È=true;return
;}bool Á(){return Ê.Count>1;}bool À(){Â();for(int F=0;F<Ê.Count;F++){var A=Ê[F]as IMyShipConnector;if(A==null)continue;if
(A.Status==MyShipConnectorStatus.Connectable)return true;}return false;}bool K(){Â();for(int F=0;F<Ê.Count;F++){var A=Ê[F
]as IMyShipConnector;if(A==null)continue;if(A.Status==MyShipConnectorStatus.Connected){var E=A.OtherConnector;if(E.
CubeGrid==A.CubeGrid){continue;}else return true;}}return false;}void I(){for(int F=0;F<Ê.Count;F++){var A=Ê[F]as
IMyShipConnector;if(A==null)continue;}}IMyTerminalBlock H(){Â();if(Ê.Count>0){return Ê[0];}return null;}IMyTerminalBlock J(bool G=false)
{Â();for(int F=0;F<Ê.Count;F++){var A=Ê[F]as IMyShipConnector;if(A==null)continue;if(A.Status==MyShipConnectorStatus.
Connected){var E=A.OtherConnector;if(E.CubeGrid==A.CubeGrid){continue;}else{if(!G){return A.OtherConnector;}else{return Ê[F];}}}}
return null;}void D(bool C=true,bool B=true){Â();for(int F=0;F<Ê.Count;F++){var A=Ê[F]as IMyShipConnector;if(A==null)continue;
if(A.Status==MyShipConnectorStatus.Connected){var E=A.OtherConnector;if(E.CubeGrid==A.CubeGrid){continue;}}if(C){if(A.
Status==MyShipConnectorStatus.Connectable)A.Connect();}else{if(A.Status==MyShipConnectorStatus.Connected)A.Disconnect();}A.
Enabled=B;}return;}string g="NOFOLLOW";string e="!WCC";string d="[NAV]";string Z="Craft Remote Control";string Y="GRIDS";void X
(ǲ W){W.Ǚ(Y,"NoFollow",ref g,true);W.Ǚ(Y,"BlockIgnore",ref e,true);W.Ǚ(Y,"OrientationBlockContains",ref d,true);W.Ǚ(Y,
"OrientationBlockNamed",ref Z,true);}List<IMyTerminalBlock>V=new List<IMyTerminalBlock>();List<IMyTextPanel>U=new List<IMyTextPanel>();List<
IMyTextPanel>S=new List<IMyTextPanel>();List<IMyTerminalBlock>R=new List<IMyTerminalBlock>();List<IMyCubeGrid>Q=new List<IMyCubeGrid
>();List<IMyCubeGrid>P=new List<IMyCubeGrid>();List<IMyCubeGrid>O=new List<IMyCubeGrid>();List<IMyCubeGrid>N=new List<
IMyCubeGrid>();bool M(){List<IMyTerminalBlock>h=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(h
);if(σ!=h.Count){return true;}return false;}string Ì(){V.Clear();N.Clear();Q.Clear();P.Clear();O.Clear();U.Clear();S.
Clear();R.Clear();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(V);σ=V.Count;foreach(var Ï in V){var ã=Ï.CubeGrid;if(!
N.Contains(ã)){N.Add(ã);}}ç(Me.CubeGrid);foreach(var ã in N){if(Q.Contains(ã))continue;bool ë=false;List<IMyShipConnector
>ê=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(ê,(n=>n.CubeGrid==ã));foreach(var é
in ê){if(é.Status==MyShipConnectorStatus.Connected){if(Q.Contains(é.OtherConnector.CubeGrid)||P.Contains(é.OtherConnector.
CubeGrid)){continue;}if(Q.Contains(é.OtherConnector.CubeGrid))ë=true;else ë=false;}}if(ë){if(!O.Contains(ã)){O.Add(ã);}}if(!P.
Contains(ã)){P.Add(ã);}}string è="";è+="B"+V.Count.ToString();è+="G"+N.Count.ToString();è+="L"+Q.Count.ToString();è+="D"+O.Count
.ToString();è+="R"+P.Count.ToString();Echo("Found "+N.Count.ToString()+" Grids");Echo("Found "+Q.Count.ToString()+
" Local Grids");for(int F=0;F<Q.Count;F++)Echo("|"+Q[F].CustomName);Echo("Found "+O.Count.ToString()+" Docked Grids");for(int F=0;F<O.
Count;F++)Echo("|"+O[F].CustomName);Echo("Found "+P.Count.ToString()+" Remote Grids");for(int F=0;F<P.Count;F++)Echo("|"+P[F]
.CustomName);return è;}void ç(IMyCubeGrid ã){if(ã==null)return;if(!Q.Contains(ã)){Q.Add(ã);æ(ã);ð(ã);ï(ã);î(ã);}}void æ(
IMyCubeGrid ã){List<IMyMotorStator>å=new List<IMyMotorStator>();GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(å,(ä=>ä.TopGrid
==ã));foreach(var ì in å){if(ì.CustomName.Contains(g)||ì.CustomData.Contains(g))continue;ç(ì.CubeGrid);}List<
IMyMotorAdvancedStator>í=new List<IMyMotorAdvancedStator>();GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(í,(ä=>ä.TopGrid==ã));
foreach(var ì in í){if(ì.CustomName.Contains(g)||ì.CustomData.Contains(g))continue;ç(ì.CubeGrid);}}void ð(IMyCubeGrid ã){List<
IMyPistonBase>Û=new List<IMyPistonBase>();GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(Û,(ä=>ä.TopGrid==ã));foreach(var Ú in Û){
ç(Ú.CubeGrid);}}void ï(IMyCubeGrid ã){List<IMyMotorStator>å=new List<IMyMotorStator>();GridTerminalSystem.GetBlocksOfType
<IMyMotorStator>(å,(n=>n.CubeGrid==ã));foreach(var ì in å){if(ì.CustomName.Contains(g)||ì.CustomData.Contains(g))continue
;IMyCubeGrid Ù=ì.TopGrid;if(Ù!=null&&Ù!=ã){ç(Ù);}}å.Clear();List<IMyMotorAdvancedStator>í=new List<IMyMotorAdvancedStator
>();GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(í,(n=>n.CubeGrid==ã));foreach(var ì in í){if(ì.CustomName.
Contains(g)||ì.CustomData.Contains(g))continue;IMyCubeGrid Ù=ì.TopGrid;if(Ù!=null&&Ù!=ã){ç(Ù);}}}void î(IMyCubeGrid ã){List<
IMyPistonBase>Û=new List<IMyPistonBase>();GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(Û,(n=>n.CubeGrid==ã));foreach(var Ú in Û)
{IMyCubeGrid Ù=Ú.TopGrid;if(Ù!=null&&Ù!=ã){if(!Q.Contains(Ù)){ç(Ù);}}}}List<IMyCubeGrid>Ø(){if(Q.Count<1){Ì();}return Q;}
List<IMyCubeGrid>Ö(){if(Q.Count<1){Ì();}return O;}bool Õ(IMyTerminalBlock Ï){return Ø().Contains(Ï.CubeGrid);}bool Ò(long Ñ)
{for(int Ó=0;Ó<Q.Count;Ó++){if((long)Q[Ó].EntityId==Ñ)return true;}return false;}bool Ò(IMyCubeGrid Ñ){return Ø().
Contains(Ñ);}bool Ð(IMyTerminalBlock Ï){var Î=Ö();if(Î==null)return false;return Î.Contains(Ï.CubeGrid);}void Ô(){if(V.Count<1)Ì
();R.Clear();foreach(var Í in V){if(Õ(Í)&&!(Í.CustomName.Contains(e)))R.Add(Í);}}IMyTerminalBlock Ü(string â){
IMyTerminalBlock Ï;Ï=(IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(â);if(Ï==null)throw new Exception(â+" Not Found");return Ï;}
List<Ý>à<Ý>(ref List<Ý>Þ,string ß=null)where Ý:class{if(Þ==null)Þ=new List<Ý>();else Þ.Clear();if(R.Count<1)Ô();for(int á=0;
á<R.Count;á++){if(R[á]is Ý&&((ß==null)||(ß!=null&&R[á].CustomName.StartsWith(ß)))){Þ.Add((Ý)R[á]);}}return Þ;}List<
IMyTerminalBlock>à<Ý>(ref List<IMyTerminalBlock>Þ,string ß=null)where Ý:class{if(V.Count<1)Ì();if(Þ==null)Þ=new List<IMyTerminalBlock>()
;else Þ.Clear();if(R.Count<1)Ô();for(int á=0;á<R.Count;á++){if(R[á]is Ý&&((ß==null)||(ß!=null&&R[á].CustomName.StartsWith
(ß)))){Þ.Add(R[á]);}}return Þ;}List<IMyTerminalBlock>à<Ý>(string ß=null)where Ý:class{var Þ=new List<IMyTerminalBlock>();
à<Ý>(ref Þ,ß);return Þ;}List<IMyTerminalBlock>ŀ<Ý>(string ß=null)where Ý:class{var Þ=new List<IMyTerminalBlock>();if(R.
Count<1)Ô();for(int á=0;á<R.Count;á++){if(R[á]is Ý&&ß!=null&&(R[á].CustomName.Contains(ß)||R[á].CustomData.Contains(ß))){Þ.
Add(R[á]);}}return Þ;}List<IMyTextPanel>Ǩ(string ß=null){if(V.Count<1)Ì();var Þ=new List<IMyTextPanel>();if(U.Count>1){
foreach(var Ǧ in U){if(ß!=null&&(Ǧ.CustomName.Contains(ß)||Ǧ.CustomData.Contains(ß)))Þ.Add(Ǧ);}}else{foreach(var Ǧ in V){if(Ǧ
is IMyTextPanel&&Õ(Ǧ)&&!(Ǧ.CustomName.Contains(e)||Ǧ.CustomData.Contains(e))){if(ß!=null&&(Ǧ.CustomName.Contains(ß)||Ǧ.
CustomData.Contains(ß)))Þ.Add(Ǧ as IMyTextPanel);U.Add(Ǧ as IMyTextPanel);}}}return Þ;}List<IMyTextPanel>ǧ(string ß=null){if(R.
Count<1)Ô();var Þ=new List<IMyTextPanel>();if(S.Count>1){foreach(var Ǧ in S){if(ß!=null&&(Ǧ.CustomName.Contains(ß)||Ǧ.
CustomData.Contains(ß)))Þ.Add(Ǧ);}}else{foreach(var Ǧ in R){if(Ǧ is IMyTextPanel&&Me.CubeGrid==Ǧ.CubeGrid){if(ß!=null&&(Ǧ.
CustomName.Contains(ß)||Ǧ.CustomData.Contains(ß)))Þ.Add(Ǧ as IMyTextPanel);S.Add(Ǧ as IMyTextPanel);}}}return Þ;}List<
IMyTerminalBlock>ǥ<Ý>(string ß=null)where Ý:class{if(R.Count<1)Ô();var Þ=new List<IMyTerminalBlock>();for(int á=0;á<R.Count;á++){if(R[á]
is Ý&&Me.CubeGrid==R[á].CubeGrid&&ß!=null&&(R[á].CustomName.Contains(ß)||R[á].CustomData.Contains(ß))){Þ.Add(R[á]);}}
return Þ;}List<IMyTerminalBlock>ǩ<Ý>(string ß=null)where Ý:class{if(R.Count<1)Ô();var Þ=new List<IMyTerminalBlock>();for(int á
=0;á<R.Count;á++){if(R[á]is Ý&&ß!=null&&R[á].CustomName==ß){Þ.Add(R[á]);}}return Þ;}IMyTerminalBlock ǵ=null;string Ǵ(){
string Ě="";var ǳ=new List<IMyTerminalBlock>();à<IMyTerminalBlock>(ref ǳ,Z);if(ǳ.Count==0){ǳ=ŀ<IMyRemoteControl>(d);if(ǳ.Count
==0){à<IMyRemoteControl>(ref ǳ);if(ǳ.Count==0){à<IMyCockpit>(ref ǳ);int F=0;for(;F<ǳ.Count;F++){Echo(
"Checking Controller:"+ǳ[F].CustomName);if(ǳ[F]is IMyCryoChamber)continue;break;}if(F>=ǳ.Count){Ě+="!!NO valid Controller:"+F+"\n";Echo(
"No Controller found");}else{Ě+="S";Echo("Using good ship Controller: "+ǳ[F].CustomName);}}else{Ě+="R";Echo(
"Using First Remote control found: "+ǳ[0].CustomName);}}}else{Ě+="N";Echo("Using Named: "+ǳ[0].CustomName);}if(ǳ.Count>0)ǵ=ǳ[0];return Ě;}class ǲ{char Ǳ='['
;char ǰ=']';string ǯ=";";string Ǯ="";public bool ǭ=false;public string Ǭ="";string ǫ="---";char Ǫ='|';private
MyGridProgram Ǥ;private Dictionary<string,string>Ǘ;private Dictionary<string,string[]>Ǣ;private Dictionary<string,Dictionary<string,
string>>ǖ;private string Ǖ="";static string[]ǔ={"true","yes","on","1"};const StringComparison Ǔ=StringComparison.
OrdinalIgnoreCase;const char ǒ='=';public bool Ǒ{get;private set;}=false;public ǲ(MyGridProgram ǐ,string ǎ){Ǥ=ǐ;Ǘ=new Dictionary<string,
string>();Ǣ=new Dictionary<string,string[]>();ǖ=new Dictionary<string,Dictionary<string,string>>();Ǐ(ǎ);}public int Ǐ(string ǎ
){ǎ.TrimEnd();if(Ǖ==ǎ){return Ǘ.Count;}Ǘ.Clear();Ǣ.Clear();ǖ.Clear();Ǯ="";Ǭ="";Ǒ=false;Ǖ=ǎ;string[]Ǎ=ǎ.Split('\n');for(
int ǌ=0;ǌ<Ǎ.Count();ǌ++){string ǋ="";Ǎ[ǌ].Trim();if(Ǎ[ǌ].StartsWith(Ǳ.ToString())){string w="";for(int Ǌ=1;Ǌ<Ǎ[ǌ].Length;Ǌ
++)if(Ǎ[ǌ][Ǌ]==ǰ)break;else w+=Ǎ[ǌ][Ǌ];if(w!=""){ǋ=w.ToUpper();}else continue;ǌ++;string ƅ="";var ǣ=new string[Ǎ.Count()-ǌ
];int ǡ=0;var Ǡ=new Dictionary<string,string>();for(;ǌ<Ǎ.Count();ǌ++){Ǎ[ǌ].Trim();if(Ǎ[ǌ].StartsWith(Ǳ.ToString())||Ǎ[ǌ].
StartsWith(ǫ)){ǌ--;break;}ƅ+=Ǎ[ǌ]+"\n";ǣ[ǡ++]=Ǎ[ǌ];if(Ǎ[ǌ].Contains(ǒ)){string[]ǟ=Ǎ[ǌ].Split('=');if(ǟ.Count()>1){string Ɖ=ǟ[0];
string Ǟ="";for(int Ó=1;Ó<ǟ.Count();Ó++){Ǟ+=ǟ[Ó];if(Ó+1<ǟ.Count())Ǟ+=ǒ;}if(Ǟ==""){int ǝ=ǌ+1;for(;ǝ<ǟ.Count();ǝ++){Ǎ[ǝ].Trim();
if(Ǎ[ǝ].Length>1&&Ǎ[ǝ][0]==Ǫ){Ǟ+=Ǎ[ǝ].Substring(1).Trim()+"\n";break;}}ǌ=ǝ;}Ǡ.Add(Ɖ,Ǟ);}}else if(Ǎ[ǌ].StartsWith(ǯ)){}}if(
!ǖ.ContainsKey(ǋ)){ǖ.Add(ǋ,Ǡ);if(!Ǣ.ContainsKey(ǋ))Ǣ.Add(ǋ,ǣ);}else{}if(!Ǘ.ContainsKey(ǋ)){Ǘ.Add(ǋ,ƅ);}else{Ǒ=true;}}else
if(Ǎ[ǌ].StartsWith(ǫ)){ǌ++;for(;ǌ<Ǎ.Count();ǌ++){Ǭ+=Ǎ[ǌ];}}else{Ǯ+=Ǎ[ǌ]+"\n";}}return Ǘ.Count;}public string ǜ(string Ɔ){
string ƅ="";if(Ǘ.ContainsKey(Ɔ))ƅ=Ǘ[Ɔ];return ƅ;}public string[]Ǜ(string Ɔ){string[]ǚ={""};if(Ǣ.ContainsKey(Ɔ))ǚ=Ǣ[Ɔ];return ǚ
;}public bool Ǚ(string Ɔ,string Ɖ,ref string ǘ,bool Ƕ=false){Ɔ=Ɔ.ToUpper();if(ǖ.ContainsKey(Ɔ)){var Ǻ=ǖ[Ɔ];if(Ǻ.
ContainsKey(Ɖ)){ǘ=Ǻ[Ɖ];return true;}}if(Ƕ)Ɗ(Ɔ,Ɖ,ǘ);return false;}public bool Ǚ(string Ɔ,string Ɖ,ref long ǿ,bool Ƕ=false){string Ƿ=
"";if(!Ǚ(Ɔ,Ɖ,ref Ƿ)){if(Ƕ){Ɗ(Ɔ,Ɖ,ǿ);}return false;}ǿ=Convert.ToInt64(Ƿ);return true;}public bool Ǚ(string Ɔ,string Ɖ,ref
int Ǿ,bool Ƕ=false){string Ƿ="";if(!Ǚ(Ɔ,Ɖ,ref Ƿ)){if(Ƕ){Ɗ(Ɔ,Ɖ,Ǿ);}return false;}Ǿ=Convert.ToInt32(Ƿ);return true;}public
bool Ǚ(string Ɔ,string Ɖ,ref double ƈ,bool Ƕ=false){string Ƿ="";if(!Ǚ(Ɔ,Ɖ,ref Ƿ)){if(Ƕ){Ɗ(Ɔ,Ɖ,ƈ);}return false;}bool Ȁ=
double.TryParse(Ƿ,out ƈ);return true;}public bool Ǚ(string Ɔ,string Ɖ,ref float Ƌ,bool Ƕ=false){string Ƿ="";if(!Ǚ(Ɔ,Ɖ,ref Ƿ)){
if(Ƕ){Ɗ(Ɔ,Ɖ,Ƌ.ToString());}return false;}bool Ȁ=float.TryParse(Ƿ,out Ƌ);return true;}public bool Ǚ(string Ɔ,string Ɖ,ref
DateTime ǉ,bool Ƕ=false){string Ƿ="";if(!Ǚ(Ɔ,Ɖ,ref Ƿ)){if(Ƕ){Ɗ(Ɔ,Ɖ,ǉ);}return false;}ǉ=DateTime.Parse(Ƿ);return true;}public
bool Ǚ(string Ɔ,string Ɖ,ref Vector3D ǽ,bool Ƕ=false){string Ƿ="";if(!Ǚ(Ɔ,Ɖ,ref Ƿ)){if(Ƕ){Ɗ(Ɔ,Ɖ,ǽ);}return false;}double n,m
,k;Ɲ(Ƿ,out n,out m,out k);ǽ.X=n;ǽ.Y=m;ǽ.Z=k;return true;}public bool Ǚ(string Ɔ,string Ɖ,ref bool ǹ,bool Ƕ=false){string
Ƿ="";if(!Ǚ(Ɔ,Ɖ,ref Ƿ)){if(Ƕ){Ɗ(Ɔ,Ɖ,ǹ);}return false;}ǹ=ǔ.Any(Ǹ=>string.Equals(Ƿ,Ǹ,Ǔ));return true;}public bool Ɗ(string Ɔ
,string Ɖ,string Ƿ){if(Ǘ.ContainsKey(Ɔ)){Ǘ[Ɔ]="";}else{Ǘ.Add(Ɔ,"");Ǒ=true;}if(ǖ.ContainsKey(Ɔ)){var Ǡ=new Dictionary<
string,string>();var Ǻ=ǖ[Ɔ];if(Ǻ.ContainsKey(Ɖ)){if(Ǻ[Ɖ]==Ƿ)return false;Ǻ[Ɖ]=Ƿ;}else{Ǻ.Add(Ɖ,Ƿ);}Ǒ=true;}else{var Ǡ=new
Dictionary<string,string>();Ǡ.Add(Ɖ,Ƿ);ǖ.Add(Ɔ,Ǡ);Ǒ=true;}return true;}public bool Ɗ(string Ɔ,string Ɖ,Vector3D ǽ){Ɗ(Ɔ,Ɖ,Ɩ(ǽ));
return true;}public bool Ɗ(string Ɔ,string Ɖ,bool ǹ){Ɗ(Ɔ,Ɖ,ǹ.ToString());return true;}public bool Ɗ(string Ɔ,string Ɖ,int Ǽ){Ɗ
(Ɔ,Ɖ,Ǽ.ToString());return true;}public bool Ɗ(string Ɔ,string Ɖ,long ǻ){Ɗ(Ɔ,Ɖ,ǻ.ToString());return true;}public bool Ɗ(
string Ɔ,string Ɖ,DateTime ǉ){Ɗ(Ɔ,Ɖ,ǉ.ToString());return true;}public bool Ɗ(string Ɔ,string Ɖ,float Ƌ){Ɗ(Ɔ,Ɖ,Ƌ.ToString());
return true;}public bool Ɗ(string Ɔ,string Ɖ,double ƈ){Ɗ(Ɔ,Ɖ,ƈ.ToString());return true;}public void Ƈ(string Ɔ,string ƅ){ƅ.
TrimEnd();Ɔ=Ɔ.ToUpper();if(Ǘ.ContainsKey(Ɔ)){if(Ǘ[Ɔ]!=ƅ){Ǘ[Ɔ]=ƅ;Ǒ=true;}}else{Ǒ=true;Ǘ.Add(Ɔ,ƅ);}}public string Ƅ(bool ƃ=true){
string Ƃ="";string Ķ=Ǯ.Trim();if(ǭ&&Ķ!="")Ƃ=Ķ+"\n";foreach(var Ɓ in Ǘ){Ƃ+=Ǳ+Ɓ.Key.Trim()+ǰ+"\n";if(Ɓ.Value.TrimEnd()==""){
string ƌ="";if(ǖ.ContainsKey(Ɓ.Key)){foreach(var Ǝ in ǖ[Ɓ.Key]){ƌ+=Ǝ.Key+ǒ+Ǝ.Value+"\n";}}ƌ+="\n";Ƃ+=ƌ;}else{Ƃ+=Ɓ.Value.Trim()
+"\n\n";}}if(Ǭ!=""){Ƃ+="\n"+ǫ+"\n";Ƃ+=Ǭ+"\n";}if(ƃ){Ǒ=false;Ǖ=Ƃ;}return Ƃ;}bool Ɲ(string ƞ,out double ä,out double Ɯ,out
double ƛ){string[]ƚ=ƞ.Trim().Split(',');if(ƚ.Length<3){ƚ=ƞ.Trim().Split(':');}ä=0;Ɯ=0;ƛ=0;if(ƚ.Length<3)return false;bool ƙ=
double.TryParse(ƚ[0].Trim(),out ä);bool Ƙ=double.TryParse(ƚ[1].Trim(),out Ɯ);bool Ɨ=double.TryParse(ƚ[2].Trim(),out ƛ);if(!ƙ||
!Ƙ||!Ɨ){return false;}return true;}string Ɩ(Vector3D ƕ){string è;è=ƕ.X.ToString("0.00")+":"+ƕ.Y.ToString("0.00")+":"+ƕ.Z.
ToString("0.00");return è;}}const string Ɣ="ORE";const string Ɠ="OREDESIREABILITY";List<Ƒ>ƒ=new List<Ƒ>();class Ƒ{public long Ɛ;
public int Ə;public Vector3D ľ;public Vector3D ű;public long ŭ;}void Ű(){ƒ.Clear();ƶ();ů();}int ů(){if(υ==null)return-1;int ŏ=
0;υ.Ǚ(Ɣ,"count",ref ŏ);ƒ.Clear();long Ŏ=0;int Ů=0;Vector3D ľ=new Vector3D(0,0,0);Vector3D ű=new Vector3D(0,0,0);long ŭ=0;
for(int Ľ=0;Ľ<ŏ;Ľ++){υ.Ǚ(Ɣ,"AsteroidId"+Ľ.ToString(),ref Ŏ);υ.Ǚ(Ɣ,"oreId"+Ľ.ToString(),ref Ů);υ.Ǚ(Ɣ,"position"+Ľ.ToString()
,ref ľ);υ.Ǚ(Ɣ,"vector"+Ľ.ToString(),ref ű);υ.Ǚ(Ɣ,"detectiontype"+Ľ.ToString(),ref ŭ);Ƒ ū=new Ƒ{Ɛ=Ŏ,Ə=Ů,ľ=ľ,ű=ű,ŭ=ŭ};ƒ.Add
(ū);}return ŏ;}void Ū(){if(υ==null)return;var ũ=ƒ.Count;υ.Ɗ(Ɣ,"count",ũ);for(int Ó=0;Ó<ƒ.Count;Ó++){υ.Ɗ(Ɣ,"AsteroidId"+Ó.
ToString(),ƒ[Ó].Ɛ);υ.Ɗ(Ɣ,"oreId"+Ó.ToString(),ƒ[Ó].Ə);υ.Ɗ(Ɣ,"position"+Ó.ToString(),ƒ[Ó].ľ);υ.Ɗ(Ɣ,"vector"+Ó.ToString(),ƒ[Ó].ű);
υ.Ɗ(Ɣ,"detectiontype"+Ó.ToString(),ƒ[Ó].ŭ);}}void Ũ(long Ŭ,int ŧ,Vector3D Ĺ,Vector3D ſ,long ŭ){Ƒ ž=new Ƒ();ž.Ə=ŧ;ž.Ɛ=Ŭ;ž.
ľ=Ĺ;ž.ű=ſ;ž.ŭ=ŭ;ƒ.Add(ž);Ŝ("ORE",Me.CubeGrid.EntityId.ToString()+":"+Ŭ+":"+ŧ+":"+Ɩ(Ĺ)+":"+Ɩ(ſ)+":"+ŭ.ToString());}void Ž(
){for(int F=0;F<ƒ.Count;F++){Echo(Ǉ(ƒ[F].Ə)+":"+ƒ[F].ľ.ToString()+":"+ƒ[F].ŭ.ToString());}}List<Ż>ż=new List<Ż>();class Ż
{public int Ů;public string ź;public long Ź;public bool Ÿ;public double ŷ;}const string Ŷ="Uranium";const string ŵ=
"Platinum";const string Ŵ="Ice";const string ų="Cobalt";const string ƀ="Gold";const string Ų="Magnesium";const string Ɵ="Nickel";
const string ƽ="Silicon";const string Ƽ="Silver";const string ƻ="Iron";const string ƺ="Scrap";const string ƹ="Stone";string[]
Ƹ={"Unknown",Ŷ,ŵ,Ŵ,ų,ƀ,Ų,Ɵ,ƽ,Ƽ,ƻ,ƹ};long[]Ʒ={0,100,95,75,55,45,45,45,45,45,15,-1};void ƶ(){ż.Clear();bool Ƶ=false;bool ƴ=
false;if(υ!=null){int ŏ=0;υ.Ǚ(Ɠ,"count",ref ŏ);if(ŏ>=Ʒ.Length){ƴ=true;for(int Ľ=0;Ľ<ŏ;Ľ++){int Ů=0;string ź="";long Ź=-1;bool
Ÿ=false;double ŷ=0;υ.Ǚ(Ɠ,"oreId"+Ľ.ToString(),ref Ů);υ.Ǚ(Ɠ,"oreName"+Ľ.ToString(),ref ź);υ.Ǚ(Ɠ,"desireability"+Ľ.ToString
(),ref Ź);υ.Ǚ(Ɠ,"bFound"+Ľ.ToString(),ref Ÿ);υ.Ǚ(Ɠ,"localAmount"+Ľ.ToString(),ref ŷ);Ż ƾ=new Ż();ƾ.Ů=Ů;ƾ.ź=ź;ƾ.Ź=Ź;ƾ.Ÿ=Ÿ;
ƾ.ŷ=ŷ;ż.Add(ƾ);}}}if(!ƴ){Ƶ=true;for(int ǈ=0;ǈ<Ƹ.Length;ǈ++){Ż ƾ=new Ż();ƾ.Ů=ǈ;ƾ.ź=Ƹ[ǈ];ƾ.Ź=Ʒ[ǈ];ƾ.Ÿ=false;ƾ.ŷ=0;ż.Add(ƾ);
}}if(Ƶ){if(υ==null){Ę+="\nNo INI for saving on OreInit()";return;}var ũ=ż.Count;υ.Ɗ(Ɠ,"count",ũ);for(int Ó=0;Ó<ż.Count;Ó
++){υ.Ɗ(Ɠ,"oreId"+Ó.ToString(),ż[Ó].Ů);υ.Ɗ(Ɠ,"oreName"+Ó.ToString(),ż[Ó].ź);υ.Ɗ(Ɠ,"desireability"+Ó.ToString(),ż[Ó].Ź);υ.Ɗ
(Ɠ,"bFound"+Ó.ToString(),ż[Ó].Ÿ);υ.Ɗ(Ɠ,"localAmount"+Ó.ToString(),ż[Ó].ŷ);}}}string Ǉ(int Ə){for(int F=0;F<ż.Count;F++)if
(ż[F].Ů==Ə)return ż[F].ź;return"INVALID ID:"+Ə;}void ǆ(){if(ż.Count<1)ƶ();for(int F=0;F<ż.Count;F++){ż[F].ŷ=0;}}void ǅ(
string Ǆ,double ǃ,bool ǂ=false){if(ż.Count<1)ƶ();for(int F=0;F<ż.Count;F++){if(ż[F].ź==Ǆ){ż[F].ŷ+=ǃ;if(ǃ>0)ż[F].Ÿ=true;if(!ǂ&&
ǃ>0&&!ż[F].Ÿ){ƿ(ż[F].Ů);}return;}}Ę+="\nOre :'"+Ǆ+"' Not found";}double ǁ(){double ǀ=0;for(int F=0;F<ż.Count;F++){if(ż[F]
.Ź<0){ǀ+=ż[F].ŷ;}}return ǀ;}void ƿ(int Ƴ){if(ż[Ƴ].Ź>0){MatrixD Ʋ=new MatrixD();Ʋ=ǵ.WorldMatrix;Vector3D ſ=Vector3D.
Normalize(Ʋ.Forward);long ƨ=ɿ(true);if(ƨ>0){Ũ(ƨ,Ƴ,ǵ.GetPosition(),ſ,69);}}}void Ƨ(){Echo("Ore Contents:");for(int F=0;F<ż.Count;F
++){if(ż[F].ŷ>0||ż[F].Ÿ)Echo(ż[F].ź+" "+ż[F].ŷ.ToString("N0"));}}void Ʀ(bool ƥ=false){if(ȅ==null||ȅ.Count<1)ȕ();if(ȅ==null
||ȅ.Count<1){return;}ǆ();var Ƥ=new List<MyInventoryItem>();for(int F=0;F<ȅ.Count;F++){var ƣ=ȅ[F].GetInventory(0);if(ƣ==
null)continue;ƣ.GetItems(Ƥ);for(int Ƣ=0;Ƣ<Ƥ.Count;Ƣ++){var ơ=Ƥ[Ƣ];if(ơ.Type.ToString().Contains("Ore")){ǅ(ơ.Type.SubtypeId.
ToString(),(double)ơ.Amount,ƥ);}}}}bool Ơ(string o){double n,m,k;string[]µ=o.Trim().Split(':');if(µ.Length>1){if(µ[0]!="WICO"){
Echo("not wico system message");return false;}if(µ.Length>2){if(µ[1]=="ORE"){int ª=2;long u=0;long.TryParse(µ[ª++],out u);
long Ʊ=0;long.TryParse(µ[ª++],out Ʊ);int Ů=0;int.TryParse(µ[ª++],out Ů);n=Convert.ToDouble(µ[ª++]);m=Convert.ToDouble(µ[ª++]
);k=Convert.ToDouble(µ[ª++]);Vector3D ľ=new Vector3D(n,m,k);n=Convert.ToDouble(µ[ª++]);m=Convert.ToDouble(µ[ª++]);k=
Convert.ToDouble(µ[ª++]);Vector3D ű=new Vector3D(n,m,k);long ŭ=0;long.TryParse(µ[ª++],out ŭ);Ƒ ū=new Ƒ{Ɛ=Ʊ,Ə=Ů,ľ=ľ,ű=ű,ŭ=ŭ};ƒ.
Add(ū);return true;}else if(µ[1]=="ORE?"){}}}return false;}int ư=Α;int Ư=0;string Ʈ="SCANS";void ƭ(ǲ W){}void Ƭ(ǲ W){W.Ɗ(Ʈ,
"DoneMode",ư);W.Ɗ(Ʈ,"DoneState",Ư);}void ƫ(ǲ W){W.Ǚ(Ʈ,"DoneMode",ref ư,true);W.Ǚ(Ʈ,"DoneState",ref Ư,true);}void ƪ(int ɍ=ˢ,int Ʃ=0
){ư=ɍ;Ư=Ʃ;τ=0;Ή(Β);}