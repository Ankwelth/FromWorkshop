/*
 * R e a d m e
 * -----------
 * 
 * This is an Autopilot script with some advance feature.
 * 
 * This is a very early alpha version i'm still working on the script.
 * You can already use it normally the following features are already stable.
 * 
 * Prerequisite :
 * One remote controller named Remote (it's better if the forward is align with the forward of your ship).
 * One remote controller named Recorder.
 * One Connector named Connector.
 * 
 * Lists of Commands :
 * 
 * 1 - Adding command, this are used to save conectors etc...
 * 
 * AddConnector:Name
 * Add a connector, save it with the name that you want.
 * Use it one time when you are connected, turn off the connector of your ship,
 * thrust in a direction and redo it.
 * 
 * AddPlanet:Name
 * If your ship have camera, Forward your ship, the camera will raycast.
 * And the planet will be save.
 * 
 * AddMiningPoint:200
 * Add a mining point, the number is the deepness.
 * 
 * AddPath:Name
 * Create a path, if path is created ad new position, position of the ship.
 * 
 * Remove:Name
 * Remove connector or path or planet.
 * 
 * 2 - Align/Look
 * 
 * AlignGravity:Forward :Backward :Up :Down
 * The :Forward :Backward :Up :Down of your ship will line up on gravity
 * 
 * LookPlanet:Name
 * The ship will line up, with the planet withe saved name,
 * and give info like distance from the planet.
 * 
 * 3 - Goto
 * 
 * GotoConnector:Name
 * The ship will go automatically to the connector and connect it.
 * 
 * Leave
 * The ship will leave the connector. (Go to the second point see AddConnector)
 * 
 * GotoOrbit
 * The ship will find the shortest way to the 0g, and go to 0g.
 * 
 * LandPlanet
 * The ship will land to nearest planet.
 * Ship detect the planet, blast until be capture to the gravity.
 * It will align on the gravity and make a free fall to economize hydrogen.
 * Ship will stop at 500m of the ground of the planet. (Need camera on backward cameras don't need to have special name)
 * 
 * FollowPath:Name
 * Will follow the saved with the given name.
 * The ship will choose the sense of the path regarding the nearest point beetween first and last.
 * 
 * 4 - Role
 * 
 * Loader
 * If you have saved two connector with name (Base and Target).
 * The ship will take items from Base ans transport it to the Target and restart.
 * 
 * Unloader
 * If you have saved two connector with name (Base and Target).
 * The ship will take items from Target and transport it to the Base and restart.
 * 
 * Miner
 * If you have a connector name base, and if you added a mining point.
 * The ship will mine until the deepness, and restart forward on the same deepness, and right etc...
 * When ship is full it go to unload to base ans restart.
 * 
 * 5 - Stop
 * 
 * Stop
 * Stop current procedure.
 * 
 * Clear
 * Clear all connector, path, planet etc...
 * 
 * Write reset in Data of programmable block, will erase the saving data when you recompile.
 * 
 * 6 - Screen / Debugging
 * 
 * Put a LCDScreen named ScreenAction you will see what actions your ship is doing.
 * Put a LCDScreen named ScreenPoint you will see the list of Connector and planet registered.
 * Put a LCDScreen named ScreenInfo you will see lot informations and detected block at the compilation.
 * 
 * 7 - Config variables
 * 
 * public static int MiningHeight = 5; Number of the restart in height
 * public static int MiningWidth = 5; Number of the restart in width
 * public static int MiningGapWidth = 8; meters beetwen width restart
 * public static int MiningGapHeight = 8; meters beetwen height restart
 * 
 */

//Config
public static class Config
{
    //Land
     public static double GroundDistance = 65000;
     public static double GroundBrake = 400;
     public static double StableDistance = 200;
     public static double OrbitDistance = 80000;

    //Goto
    public static double ToleranceAngle = 0.01;
    public static double ToleranceDistance = 0.5;
    public static  int MaximumTick = 1000000;
    public static int AlignementWaiting = 20;

    //Volume
    public static double ToleranceVolume = 1;
    public static double ToleranceBattery = 1;

    //Mining
    public static double MiningDistance = 1;
    public static double MiningDeepness = 100;
    public static double ControlMiningTolerance = 0.01;
    public static double RetryDistance = 5;
    public static int MiningHeight = 5;
    public static int MiningWidth = 5;
    public static int MiningGapWidth = 20;
    public static int MiningGapHeight = 20;
    public static int ControlMiningFrequency = 20;

    public static Dictionary<string, float> Speed = new Dictionary<string, float>() {{"Cruising", 30F },
                                                                                     {"Max", 100F },
                                                                                     {"Approach",10F },
                                                                                     {"Docking",1F },
                                                                                     {"Leaving",5F },
                                                                                     {"Mining",0.5F }};
}
static class Ŵ{public static IMyGridTerminalSystem ų;public static IMyProgrammableBlock Ų;public static Program ű;public
static Ư Ű=null;public static Ư ů=null;public static Ư Ů=null;public static int ŭ=0;public static void Ú(IMyGridTerminalSystem
D,IMyProgrammableBlock Ã,Program X){ų=D;Ų=Ã;ű=X;if(string.IsNullOrEmpty(ű.Storage))ű.Storage="init|Grid";Ű=(Ţ(
"ScreenAction"))?new Ư("ScreenAction"):null;ů=(Ţ("ScreenPoint"))?new Ư("ScreenPoint"):null;Ů=(Ţ("ScreenInfo"))?new Ư("ScreenInfo"):
null;}public static List<IMyTerminalBlock>ž(string e,bool ż=true,bool Ż=false){List<IMyTerminalBlock>Ũ=new List<
IMyTerminalBlock>();ų.GetBlocks(Ũ);if(Ż)Ũ.RemoveAll(delegate(IMyTerminalBlock A){return!A.CustomName.Contains(e);});else Ũ.RemoveAll(
delegate(IMyTerminalBlock A){return A.CustomName!=e;});if(ż)Ź(Ũ);return Ũ;}public static IMyTerminalBlock š(string e,bool ż=true
,bool Ż=false){return Ţ(e,ż,Ż)?ž(e,ż,Ż)[0]:default(IMyTerminalBlock);}public static List<IMyTerminalBlock>Ž(bool ż=true){
List<IMyTerminalBlock>Ũ=new List<IMyTerminalBlock>();ų.GetBlocks(Ũ);if(ż)Ź(Ũ);return Ũ;}public static bool Ţ(string e,bool ż
=true,bool Ż=false){return ž(e,ż,Ż).Count()>0;}public static bool ź(IMyTerminalBlock A){return(A.CubeGrid==Ų.CubeGrid);}
public static void Ź<Ÿ>(List<Ÿ>Ũ){Ũ.RemoveAll(delegate(Ÿ A){return!ź((IMyTerminalBlock)A);});}public static List<
IMyTerminalBlock>ŷ(List<IMyTerminalBlock>ĸ){return ĸ.FindAll(delegate(IMyTerminalBlock A){return A.HasInventory;});}public static void Ŷ
(string e,object ŵ){string[]ř=ű.Storage.Split('|');List<string>Ŭ=ř[0].Split('_').ToList();List<string>ň=ř[1].Split('_').
ToList();int Õ=Array.IndexOf(ř[0].Split('_'),e);if(Õ>-1)ň[Õ]=ŵ.ToString();else{Ŭ.Add(e);ň.Add(ŵ.ToString());}ű.Storage=string.
Join("_",Ŭ)+"|"+string.Join("_",ň);}public static string Ś(string e){string[]ř=ű.Storage.Split('|');int Õ=Array.IndexOf(ř[0]
.Split('_'),e);return(Õ>-1)?ř[1].Split('_')[Õ]:null;}public static void Ř(){ű.Storage="";}public static void ŗ(){if(Ű!=
null)Ű.ŗ(Ƨ.Ƥ);}public static void Ŗ(){if(ů!=null)ů.ƃ();}public static void ŕ(string Ĵ,bool æ=true){if(Ů==null)return;Ů.Ƭ(Ĵ,æ
);}public static void Ŕ(){if(Ů==null)return;Ů.Ɖ();}}static class œ{public class Œ{public string e;public long ő;public
long Ő;public long ŏ;public Œ(string æ="",long Ŏ=0){e=æ;ő=Ŏ;}public long ō(int Ç=1000000){return ő/Ç;}public long Ō(int Ç=
1000000){return Ő/Ç;}public long ŋ(int Ç=1000000){return Ő/Ç;}public bool Ŋ(){return Ő>(ŏ-Config.ToleranceVolume);}public bool
ŉ(){return Ő==0;}}public static List<Œ>ś(List<IMyTerminalBlock>ĸ){List<IMyTerminalBlock>Ũ=Ŵ.ŷ(ĸ);List<Œ>ū=new List<Œ>();
foreach(var ť in Ũ){foreach(var Î in Ť(ť)){int Ū=ū.FindIndex(delegate(Œ A){return A.e==Î.Type.SubtypeId;});if(Ū>-1){ū[Ū].ő+=1;}
else{ū.Add(new Œ(Î.Type.SubtypeId,Î.Amount.RawValue));}}}return ū;}public static Œ ũ(List<IMyTerminalBlock>ĸ){List<
IMyTerminalBlock>Ũ=Ŵ.ŷ(ĸ);List<IMyInventory>ŧ=new List<IMyInventory>();List<MyInventoryItem>Ŧ=new List<MyInventoryItem>();Œ Õ=new Œ();
foreach(var ť in Ũ){Õ.Ő+=ť.GetInventory().CurrentVolume.RawValue;Õ.ŏ+=ť.GetInventory().MaxVolume.RawValue;}return Õ;}public
static List<MyInventoryItem>Ť(IMyTerminalBlock ĸ){List<MyInventoryItem>Š=new List<MyInventoryItem>();ĸ.GetInventory().GetItems
(Š);return Š;}}static class ţ{public static bool Ţ(string e){return Ž().Exists(delegate(MyWaypointInfo A){return A.Name==
e;});}public static MyWaypointInfo š(string e){List<MyWaypointInfo>Š=Ž();return Š[Š.FindIndex(delegate(MyWaypointInfo A){
return A.Name==e;})];}public static MyWaypointInfo ş(Vector3D X){foreach(var A in Ž()){if(Ʒ.Ƴ(X,A.Coords,2))return A;}return
new MyWaypointInfo("Zero",Vector3D.Zero);}public static MyWaypointInfo Ş(){foreach(var A in Ž()){if(A.Name.Contains(
"Planet")&&Ʒ.Ƴ(Ə.Ǝ.GetPosition(),A.Coords,200000))return A;}return new MyWaypointInfo("Zero",Vector3D.Zero);}public static void
ŝ(string e,Vector3D X){if(Ţ(e))ſ(e);Ə.ƍ.AddWaypoint(X,e);}public static void ſ(string e){ƨ(ƫ(Ž(),e));}public static List<
MyWaypointInfo>ƫ(List<MyWaypointInfo>ƪ,string Ʃ=""){List<MyWaypointInfo>Š=new List<MyWaypointInfo>();ƪ.ForEach(delegate(MyWaypointInfo
Ĝ){if(Ĝ.Name!=Ʃ)Š.Add(new MyWaypointInfo(Ĝ.Name,Ĝ.Coords));});return Š;}public static void ƨ(List<MyWaypointInfo>Š){Ə.ƍ.
ClearWaypoints();Š.ForEach(delegate(MyWaypointInfo A){Ə.ƍ.AddWaypoint(A.Coords,A.Name);});}public static List<MyWaypointInfo>Ž(){List<
MyWaypointInfo>Š=new List<MyWaypointInfo>();Ə.ƍ.GetWaypointInfo(Š);return Š;}}static class Ƨ{public struct Ʀ{public string ì{get;set;}
public string e{get;set;}public string ć{get;set;}public Vector3D Å{get;set;}public Vector3D Ä{get;set;}}public static List<Ʀ>
ƥ=new List<Ʀ>();public static int Ƥ=-2;public static Ʀ ƣ{get{return ƥ[Ƥ];}}public static void Ƣ(string Ĵ,string æ,string
O="",Vector3D Å=new Vector3D(),Vector3D Ä=new Vector3D()){Ʀ Z=new Ʀ();Z.ì=Ĵ;Z.e=æ;Z.ć=O;Z.Å=Å;Z.Ä=Ä;ƥ.Add(Z);}public
static void ơ(){Ʀ Z=ƥ[Ƥ];Z.Å=ţ.š(ƥ[Ƥ].e).Coords;ƥ[Ƥ]=Z;}public static Ʀ Ơ(string e){Ʀ Z=ƥ[Ƥ];Matrix ƺ=new Matrix();Ə.Ǝ.
Orientation.GetMatrix(out ƺ);Vector3D ƻ=Vector3D.Normalize(Ə.Ǝ.GetNaturalGravity());Vector3D D=Ə.Ǝ.GetNaturalGravity();Z.Ä=Vector3D
.Zero;if(e=="Forward")Z.Å=ƻ;if(e=="Backward")Z.Å=-ƻ;if(e=="Up"){Z.Å=(Vector3D)ƺ.Forward*ƻ;Z.Ä=ƻ;}if(e=="Down"){Z.Å=(
Vector3D)ƺ.Forward*ƻ;Z.Ä=-ƻ;}ƥ[Ƥ]=Z;return Z;}public static void é(int æ){for(int Õ=0;Õ<=æ;Õ++)ƥ.RemoveAt(Ƥ);}public static void
ƹ(){ƥ.Clear();}public static int ê{get{return ƥ.Count();}}public static void Ƹ(){Ƥ=(Ƥ==-2)?0:(Ƥ+1)%ê;Ŵ.ŭ=0;Ŵ.Ŷ("Cursor",Ƥ
);}}static class Ʒ{public static Vector3D ƶ(Vector3D X,Vector3D Ç,double Ƶ){return X+(Ç*Ƶ);}public static double ƴ(
Vector3D Z,Vector3D A){return Math.Acos(MathHelper.Clamp(Z.Dot(A),-1,1));}public static bool Ƴ(Vector3D Å,Vector3D Ä,double Ʊ){
return Vector3D.Distance(Å,Ä)<Ʊ;}public static bool Ʋ(Vector3D Å,Vector3D Ä,double Ʊ=0.1){return Math.Abs(ƴ(Å,Ä))<Ʊ;}public
static Vector3D ư(Vector3D Z){if(Vector3D.IsZero(Z))return Vector3D.Zero;if(Vector3D.IsUnit(ref Z))return Z;return Vector3D.
Normalize(Z);}}class Ư{private IMyTextSurface Ʈ;public Ư(string ƭ){Ʈ=(IMyTextSurface)Ŵ.š(ƭ);}public void Ƭ(string Ƌ,bool Ɵ=true){
if(Ɵ)Ƌ="\n"+Ƌ;Ʈ.WriteText(Ƌ,true);}public void ƀ(string Ƌ){Ʈ.WriteText(Ƌ,false);}public void Ɗ(){Ʈ.WriteText("\n",true);}
public void Ɖ(){ƀ("");}public void ƈ(){foreach(var Î in œ.ś(Ŵ.Ž())){Ƭ(Î.e,true);Ƭ("  "+Î.ō().ToString(),false);}}public void Ƈ
(string e,bool Ɔ=true){List<IMyTerminalBlock>Ũ=Ŵ.ž(e,Ɔ);foreach(var Î in œ.ś(Ũ)){Ƭ(Î.e,true);Ƭ("  "+Î.ō().ToString(),
false);}}public void ƅ(int æ){Ɖ();if(æ<0){Ƭ("Inactive "+æ,true);}else{for(int Õ=æ;Õ<Ƨ.ê;Õ++){string O=(Õ==æ)?">":" ";Ƭ(O+" "+
Ƨ.ƥ[Õ].ì+" "+Ƨ.ƥ[Õ].e,true);}for(int Õ=0;Õ<æ;Õ++){string O=(Õ==æ)?">":" ";Ƭ(O+" "+Ƨ.ƥ[Õ].ì+" "+Ƨ.ƥ[Õ].e,true);}}}public
void ŗ(int æ){int Ƅ=8;ƀ("Cursor : "+æ.ToString()+" Tick : "+Ŵ.ŭ.ToString()+" Energy : "+Ə.ß.ToString()+" %");Ɗ();Ƭ(
"Actions :");if(æ>-1){for(int Õ=æ;Õ<Ƨ.ê;Õ++){if(Õ<æ+Ƅ){string O=(Õ==æ)?" >":Õ.ToString();Ƭ(O+" "+Ƨ.ƥ[Õ].ì+" "+Ƨ.ƥ[Õ].e,true);}}}
else{Ƭ("Inactive ",true);}Ɗ();Ƭ("Storage :");Ƭ("Process: "+Ŵ.Ś("Process"));Ƭ("Saved Cursor: "+Ŵ.Ś("Cursor")+" MiningCount: "
+Ŵ.Ś("MiningCount"));}public void ƃ(){var Ƃ=ţ.Ž();var O=new List<String>();var X=new List<String>();var æ=new List<String
>();var Ŭ=new List<String>();foreach(var è in Ƃ){int Ɓ=0;Ɓ+=(ţ.Ţ(è.Name+"_Access"))?1:0;Ɓ+=(ţ.Ţ(è.Name+"_Access_Up"))?1:0
;Ɓ+=(ţ.Ţ(è.Name+"_Access_Forward"))?1:0;if(Ɓ==3)O.Add(è.Name);}foreach(var è in Ƃ)if(è.Name.Contains("Planet_"))X.Add(è.
Name.Split('_')[1]);foreach(var è in Ƃ){if(è.Name.Contains("Path")){string e=è.Name.Split('_')[1];æ.Add(e);if(!Ŭ.Contains(e)
){Ŭ.Add(e);}}}ƀ("Number of points : "+Ƃ.Count());Ƭ("");Ƭ("Connectors :");O.ForEach(è=>Ƭ("-"+è));Ƭ("");Ƭ("Planets    :");X
.ForEach(è=>Ƭ("-"+è));Ƭ("");Ƭ("Path       :");Ŭ.ForEach(è=>Ƭ("-"+è+"[Points : "+æ.FindAll(Ĝ=>Ĝ==è).Count()+" ]"));}public
void ƞ(){}}class Ɲ{public Ɲ(IMyCameraBlock O,string Ç="",string Ɯ=""){ƛ=O;ù=Ç;Ƙ=Ɯ;ƚ=default(MyDetectedEntityInfo);ƙ=default(
MyDetectedEntityInfo);}public IMyCameraBlock ƛ;public MyDetectedEntityInfo ƚ=default(MyDetectedEntityInfo);public MyDetectedEntityInfo ƙ=
default(MyDetectedEntityInfo);public string ù;public string Ƙ;public bool Ɨ{get{return ƚ.Type==MyDetectedEntityType.Planet;}}
public bool Ɩ{get{return!ƚ.IsEmpty();}}public bool ƕ{get{return!ƙ.IsEmpty();}}public Vector3D Ɣ{get{return(Vector3D)ƚ.
HitPosition;}}public Vector3D Ɠ{get{return(Vector3D)ƙ.HitPosition;}}public double F=0;public double ê(){return(Ɩ)?Vector3D.Distance
(ƛ.GetPosition(),Ɣ):-1;}public double ƒ(){return(ƕ)?Vector3D.Distance(ƛ.GetPosition(),Ɠ):-1;}public void Ƒ(double H){ƛ.
EnableRaycast=true;if(ƛ.AvailableScanRange>H){ƚ=ƛ.Raycast(H);F=(ƕ)?Vector3D.Distance(Ɣ,Ɠ):-1;ƙ=ƚ;ƛ.EnableRaycast=false;}else{Ŵ.ŕ(
"Charging raycast : "+Math.Round((ƛ.AvailableScanRange/H)*100)+"%");Ŵ.ŕ("Current Capacity : "+Math.Round(ƛ.AvailableScanRange));ƚ=default(
MyDetectedEntityInfo);}}public void Ɛ(){}}static class Ə{public static IMyRemoteControl Ǝ;public static IMyRemoteControl ƍ;public static
List<IMyGyro>č;public static List<IMyThrust>ƌ;public static List<IMyCameraBlock>Ŝ;public static List<Ɲ>Ň;public static List<
IMyBatteryBlock>z;public static List<IMyShipDrill>ã;public static List<IMyGasTank>â;public static List<IMyTerminalBlock>á;public static
IMyShipConnector à;public static double ß{get{float Þ=0F;float Ý=0F;z.ForEach(delegate(IMyBatteryBlock A){Þ+=A.MaxStoredPower;Ý+=A.
CurrentStoredPower;});return(Þ+Ý==0F)?100:Math.Round((Ý/Þ)*100);}}public static bool Ü=false;public static bool Û=false;public static void
Ú(){č=new List<IMyGyro>();ã=new List<IMyShipDrill>();á=new List<IMyTerminalBlock>();ƌ=new List<IMyThrust>();z=new List<
IMyBatteryBlock>();â=new List<IMyGasTank>();Ŝ=new List<IMyCameraBlock>();Ň=new List<Ɲ>();Ǝ=(IMyRemoteControl)Ŵ.š("Remote");ƍ=(
IMyRemoteControl)Ŵ.š("Recorder");à=(IMyShipConnector)Ŵ.š("Connector");Ŵ.ų.GetBlocksOfType<IMyGyro>(č,D=>Ŵ.ź(D));Ŵ.ų.GetBlocksOfType<
IMyBatteryBlock>(z,A=>Ŵ.ź(A));Ŵ.ų.GetBlocksOfType<IMyShipDrill>(ã,Ç=>Ŵ.ź(Ç));Ŵ.ų.GetBlocksOfType<IMyThrust>(ƌ,Ç=>Ŵ.ź(Ç));Ŵ.ų.
GetBlocksOfType<IMyCargoContainer>(á,O=>Ŵ.ź(O));Ŵ.ų.GetBlocksOfType<IMyGasTank>(â,O=>Ŵ.ź(O));Ŵ.ų.GetBlocksOfType<IMyCameraBlock>(Ŝ,O=>Ŵ
.ź(O));Ø();Ü=true;if(Ǝ!=default(IMyRemoteControl))Ǝ.FlightMode=FlightMode.OneWay;}public static List<Vector3D>Ù(){return(
Ǝ==default(IMyRemoteControl))?new List<Vector3D>():new List<Vector3D>(){Ǝ.WorldMatrix.Forward,Ǝ.WorldMatrix.Backward,Ǝ.
WorldMatrix.Left,Ǝ.WorldMatrix.Right,Ǝ.WorldMatrix.Up,Ǝ.WorldMatrix.Down};}public static void Ø(){List<Vector3D>Ö=Ù();for(var Õ=0;Õ
<Ö.Count();Õ++){for(var O=0;O<Ŝ.Count();O++){if(Ö[Õ]==Ŝ[O].WorldMatrix.Forward){Ň.Add(new Ɲ(Ŝ[O],Base6Directions.
EnumDirections[Õ].ToString()));}}}}public static void Ô(bool Ò=true){č.ForEach(delegate(IMyGyro A){A.ApplyAction("OnOff_"+((Ò)?"On":
"Off"));});}public static void Ó(bool Ò=false){ã.ForEach(delegate(IMyShipDrill A){A.ApplyAction("OnOff_"+((Ò)?"On":"Off"));})
;}public static void ä(ChargeMode æ){z.ForEach(A=>A.ChargeMode=æ);}public static double ü(double H,bool û=true){double Ç=
(H/Ə.Ǝ.GetShipVelocities().LinearVelocity.Length());return(û)?Math.Round(Ç):Ç;}public static Ɲ ú(string ù){return Ň.Find(
O=>O.ù==ù);}public static void ø(){Ŵ.ŕ("Required : ");Ŵ.ŕ("-Remote : "+(Ə.Ǝ!=default(IMyRemoteControl)));Ŵ.ŕ(
"-Recorder : "+(Ə.ƍ!=default(IMyRemoteControl)));Ŵ.ŕ("-Connector : "+(Ə.à!=default(IMyShipConnector)));Ŵ.ŕ("");Ŵ.ŕ("Options :");Ŵ.ŕ(
"-Battery : "+Ə.z.Count());Ŵ.ŕ("-Gryros : "+Ə.č.Count());Ŵ.ŕ("-Thruster : "+Ə.ƌ.Count());Ŵ.ŕ("-Cargo : "+Ə.á.Count());Ŵ.ŕ(
"-Drills : "+Ə.ã.Count());Ŵ.ŕ("-Cameras : "+Ə.Ŝ.Count());}}static class ö{public static string õ="None";public static int ô=0;public
static double ó=0;public static void Ú(){if(Ŵ.Ś("Cursor")==null)Ŵ.Ŷ("Cursor",-2);if(Ŵ.Ś("MiningCount")==null)Ŵ.Ŷ("MiningCount"
,0);if(!Ə.Ü)Ə.Ú();}public static void ò(string ì){ţ.ŝ(ì,Ə.Ǝ.GetPosition());}public static void ñ(string ì){ţ.ŝ(ì+
"_Access",Ə.Ǝ.GetPosition());ţ.ŝ(ì+"_Access_Up",Ə.Ǝ.WorldMatrix.Up);ţ.ŝ(ì+"_Access_Forward",Ə.Ǝ.WorldMatrix.Forward);}public
static void ð(){ţ.ŝ("Stable_Point",Ʒ.ƶ(Ə.Ǝ.GetPosition(),-Vector3D.Normalize(Ə.Ǝ.GetNaturalGravity()),Config.StableDistance));
ţ.ŝ("Orbit_Point",Vector3D.Zero);}public static void ï(){ţ.ŝ("Orbit_Point",Ʒ.ƶ(Ə.Ǝ.GetPosition(),-Vector3D.Normalize(Ə.Ǝ.
GetNaturalGravity()),Config.OrbitDistance));}public static void î(string ì){if(!ţ.Ţ(ì))ò(ì);else if(!ţ.Ţ(ì+"_Access"))ñ(ì);}public static
void í(string ì,string ë){ò(ì);double ê=(!String.IsNullOrEmpty(ë))?double.Parse(ë):10;Vector3D Ñ=Ʒ.ƶ(Ə.Ǝ.GetPosition(),-Ə.à.
WorldMatrix.Forward,ê);ţ.ŝ(ì+"_Access",Ñ);ţ.ŝ(ì+"_Access_Up",Ə.Ǝ.WorldMatrix.Up);ţ.ŝ(ì+"_Access_Forward",Ə.Ǝ.WorldMatrix.Forward);}
public static void é(string A){ţ.Ž().ForEach(delegate(MyWaypointInfo è){if(è.Name.Contains(A))ţ.ſ(è.Name);});}public static
void ç(string e,Vector3D Ñ){int Z=0;ţ.Ž().ForEach(delegate(MyWaypointInfo A){if(A.Name.Contains("Path_"+e))Z++;});ţ.ŝ(
"Path_"+e+"_"+Z.ToString(),Ñ);}public static MyWaypointInfo h(string e){return ţ.š("Path_"+e+"_"+"0");}public static
MyWaypointInfo f(string e){int Z=-1;ţ.Ž().ForEach(delegate(MyWaypointInfo A){if(A.Name.Contains("Path_"+e))Z++;});return ţ.š("Path_"+e
+"_"+Z.ToString());}public static void Y(string X){double V=double.Parse(X);ñ("Mining");ò("Mining_Memory");ţ.ŝ(
"Mining_End",Ʒ.ƶ(Ə.Ǝ.GetPosition(),Ə.ã[0].WorldMatrix.Forward,V));ţ.ŝ("Mining_End_Start",new Vector3D(ţ.š("Mining_End").Coords));ţ.ŝ
("Mining_Access_Start",new Vector3D(ţ.š("Mining_Access").Coords));Ŵ.Ŷ("MiningCount",0);}public static void W(double V=0,
int U=0){int R=int.Parse(Ŵ.Ś("MiningCount"));Vector3D Q=new Vector3D(ţ.š("Mining_Access_Start").Coords);double P=Vector3D.
Distance(Q,ţ.š("Mining_End_Start").Coords);double O=R/Config.MiningHeight;int N=(int)Math.Floor((double)R/Config.MiningHeight);
int M=R%Config.MiningWidth;Vector3D L=Ʒ.ƶ(Q,Ə.ã[0].WorldMatrix.Left,M*Config.MiningGapWidth);L=Ʒ.ƶ(L,Ə.ã[0].WorldMatrix.Up,
N*Config.MiningGapHeight);Vector3D K=Ʒ.ƶ(L,Ə.ã[0].WorldMatrix.Forward,P);Vector3D J=new Vector3D(L);ţ.ŝ("Mining_Access",L
);ţ.ŝ("Mining_End",K);ţ.ŝ("Mining_Memory",J);}public static void I(){double H=Vector3D.Distance(Ə.Ǝ.GetPosition(),Ƨ.ƣ.Å);
bool G=Ə.Û&&H<Config.ToleranceDistance;if(!G&&Ƨ.ƣ.ć=="ForceAlign"){if(!º(Ƨ.ƣ.e))return;}if(!G&&Ƨ.ƣ.ć=="Access"){if(H<100)ª(
"Approach");}if(!G&&Ƨ.ƣ.ć=="Gravity"){if(!Ə.Û)ï();G=(Ƨ.ƣ.Ä.X==0&&Ə.Ǝ.GetNaturalGravity().Length()==Ƨ.ƣ.Ä.X)||(Ƨ.ƣ.Ä.X>0&&Ə.Ǝ.
GetNaturalGravity().Length()>Ƨ.ƣ.Ä.X);double F=Vector3D.Distance(Ə.Ǝ.GetPosition(),ţ.š("Stable_Point").Coords);Ŵ.ŕ("Current Gravity : "+Ƨ
.ƣ.ć);Ŵ.ŕ("Distance made : "+Math.Round(F));}else{Ŵ.ŕ("Distance : "+Math.Round(H)+"m");Ŵ.ŕ("Approximate Duration : "+Ə.ü(
H));}Ŵ.ŕ("Arrived : "+G);Ŵ.ŕ("Going : "+Ə.Û);Ŵ.ŕ("Checker : "+Ƨ.ƣ.ć);if(G&&Ə.Û){Ə.Û=false;Ə.Ǝ.ClearWaypoints();Ə.Ǝ.
SetAutoPilotEnabled(false);Ƨ.Ƹ();}else{if(!Ə.Û){Ə.Û=true;Ƨ.ơ();Ə.Ǝ.AddWaypoint(new MyWaypointInfo(Ƨ.ƣ.e,Ƨ.ƣ.Å));Ə.Ǝ.SetAutoPilotEnabled(
true);}}}public static void E(){if(Ƨ.ƣ.e=="Gravity"&&Ŵ.ŭ==1){Ƨ.Ơ(Ƨ.ƣ.ć);}if(Æ(Ƨ.ƣ.Å,Ƨ.ƣ.Ä,Ə.Ǝ.WorldMatrix)){Ə.č.ForEach(
delegate(IMyGyro D){D.GyroOverride=false;});Ƨ.Ƹ();}}public static void C(){int B=int.Parse(Ƨ.ƣ.e);if(Ŵ.ŭ==B)Ƨ.Ƹ();else Ŵ.ŕ(
"Waiting : "+((Ŵ.ŭ/B)*100)+" % ");}public static void j(){if(Ə.à.Status==MyShipConnectorStatus.Connected){if(Ƨ.ƣ.ć=="Battery")Ə.ä(
ChargeMode.Recharge);Ƨ.Ƹ();}else{Ə.à.ApplyAction("OnOff_On");Ə.à.ApplyAction("SwitchLock");}}public static void u(){œ.Œ Î=œ.ũ(Ə.á)
;bool Ð=Ƨ.ƣ.ć!="Battery"||(Ə.ß>(100-Config.ToleranceBattery));if(Ƨ.ƣ.e=="full"){if(Î.Ŋ()&&Ð){Ə.à.CollectAll=false;Ə.à.
ApplyAction("OnOff_Off");Ə.ä(ChargeMode.Auto);Ƨ.Ƹ();}}if(Ƨ.ƣ.e=="empty"){Ə.à.CollectAll=true;foreach(var O in Ə.á){if(œ.Ť(O).Count(
)>0){O.GetInventory().TransferItemTo(Ə.à.GetInventory(),œ.Ť(O)[0]);}}if(Î.ŉ()&&Ð){Ə.à.CollectAll=false;Ə.à.ApplyAction(
"OnOff_Off");Ə.ä(ChargeMode.Auto);Ƨ.Ƹ();}}if(Ƨ.ƣ.e==""&&Ð){Ə.à.ApplyAction("OnOff_Off");Ə.ä(ChargeMode.Auto);Ƨ.Ƹ();}}public static
void Ï(){bool µ=º("Mining_Access");ó=(Ŵ.ŭ%Config.ControlMiningFrequency==0)?0:(ó+Ə.Ǝ.GetShipVelocities().LinearVelocity.
Length());œ.Œ Î=œ.ũ(Ə.á);if((Ŵ.ŭ%Config.ControlMiningFrequency)==Config.ControlMiningFrequency-1&&ó<(Config.
ControlMiningTolerance*Config.ControlMiningFrequency)&&õ!="Retry"){õ="Retry";Ə.Ǝ.ClearWaypoints();Vector3D Í=Ʒ.ƶ(Ə.Ǝ.GetPosition(),-Ə.ã[0].
WorldMatrix.Forward,Config.RetryDistance);ţ.ŝ("Mining_Retry",Í);Ə.Ǝ.AddWaypoint(Í,"Mining_Retry");Ġ("Drills");Ə.Ǝ.
SetAutoPilotEnabled(true);}if(õ=="Retry"){if(Ʒ.Ƴ(Ə.Ǝ.GetPosition(),ţ.š("Mining_Retry").Coords,Config.MiningDistance)){Ə.Ǝ.ClearWaypoints();
ţ.ſ("Mining_Retry");Ə.Ǝ.SetAutoPilotEnabled(false);õ="None";}}if(Î.Ŋ()||õ=="Process"){if(Î.Ŋ()){ţ.ŝ("Mining_Memory",Ə.Ǝ.
GetPosition());ª("Docking");Ə.Ǝ.ClearWaypoints();Ə.Ó(false);Ə.Ǝ.SetAutoPilotEnabled(false);õ="None";Ƨ.Ƹ();}if(Ʒ.Ƴ(Ə.Ǝ.GetPosition()
,ţ.š("Mining_End").Coords,Config.MiningDistance)){õ="End";Ə.Ǝ.ClearWaypoints();Ə.Ǝ.SetAutoPilotEnabled(false);Ə.Ó(false);
Ƨ.Ƹ();}}else{õ="Process";Ə.Ǝ.SetValueBool("ControlGyros",false);Ə.Ǝ.AddWaypoint(ţ.š("Mining_End"));Ə.Ǝ.
SetAutoPilotEnabled(true);Ġ("Drills");ª("Mining");Ə.Ó(true);}}public static void Ì(){if(õ=="End"){W();Ŵ.Ŷ("MiningCount",int.Parse(Ŵ.Ś(
"MiningCount"))+1);õ="None";}Ƨ.Ƹ();}public static void Ë(){Ɲ È=Ə.ú(Ƨ.ƣ.e);È.Ƒ(Config.GroundDistance);if(È.Ɨ){Ŵ.ŕ("Target Reached: "+È
.ƚ.Name);Ŵ.ŕ("Impact Point at : "+È.ê()+" m ");Ŵ.ŕ("Approximate Duration : "+Ə.ü(È.ê())+" s ");ţ.ŝ("Planet_"+Ƨ.ƣ.ć,È.ƚ.
Position);Ƨ.Ƹ();}}public static void Ê(){if(Ƨ.ƣ.e=="Planet"){double Ç=Vector3D.Distance(Ə.Ǝ.GetPosition(),ţ.š("Planet_"+Ƨ.ƣ.ć).
Coords);Ŵ.ŕ("Planet "+Ƨ.ƣ.ć+":");Ŵ.ŕ("Distance Approximation : "+(Math.Round((Ç/1000),2)-50)+" km ");Ŵ.ŕ("");Ƨ.Ƹ();}}public
static void É(){Ə.ƌ.ForEach(X=>X.Enabled=false);Ɲ È=Ə.ú(Ƨ.ƣ.e);if(Ŵ.ŭ==1)Ƨ.Ơ(Ƨ.ƣ.e);bool µ=Æ(Ƨ.ƣ.Å,Ƨ.ƣ.Ä,Ə.Ǝ.WorldMatrix);
double Ç=(ţ.Ţ("PL_ImpactPoint"))?Math.Round(Vector3D.Distance(È.ƛ.GetPosition(),ţ.š("PL_ImpactPoint").Coords)):Config.
GroundDistance;È.Ƒ(Ç+2000);Ŵ.ŕ("Impact Point at : "+Math.Round(È.ƒ()/1000,2)+" km ");Ŵ.ŕ("Aligned With Gavity : "+µ);Ŵ.ŕ(
"Approximate Duration : "+Ə.ü(È.ƒ())+" s ");Ŵ.ŕ("Delta : "+Math.Round(È.F,2));if(È.Ɨ){ţ.ŝ("PL_ImpactPoint",È.Ɣ);if(È.ê()<Config.GroundBrake){Ə.ƌ.
ForEach(X=>X.Enabled=true);ý();}Ŵ.ŕ("Thruster Off");}}public static bool Æ(Vector3D Å,Vector3D Ä,MatrixD Ã){Ə.Ô(true);double Â,
Á,À;ī(Å,Ä,Ã,out Â,out Á,out À);double F=Math.Abs(Â)+Math.Abs(Á)+Math.Abs(À);Ŵ.ŕ("Delta : "+F);ĳ(Â,Á,À);return F<Config.
ToleranceAngle;}public static bool º(string X){string e=X.Split('_')[0]+"_Access";bool µ=Æ(ţ.š(e+"_Forward").Coords,ţ.š(e+"_Up").
Coords,Ə.Ǝ.WorldMatrix);Ə.Ô(!µ);Ə.Ǝ.SetAutoPilotEnabled(µ);return µ;}public static void ª(string k=""){Ə.Ǝ.SpeedLimit=(k=="")?
Config.Speed[Ƨ.ƣ.e]:Config.Speed[k];}public static void ý(bool ď=true){Ƨ.Ƥ=-1;if(ď){Ŵ.Ŷ("Cursor",-1);Ŵ.Ŷ("Process","Stop");}õ=
"None";Ə.Û=false;if(Ə.Ǝ!=default(IMyRemoteControl)){Ə.Ǝ.ClearWaypoints();Ə.Ǝ.Direction=0;}Ə.Ó(false);Ə.Ô(true);foreach(var ĭ
in Ə.č)ĭ.GyroOverride=false;Ŵ.ű.Runtime.UpdateFrequency=UpdateFrequency.None;Ƨ.ƹ();}public static void ķ(double k=0.005){
Vector3D Ķ=Ə.Ǝ.GetShipVelocities().LinearVelocity;double ĵ=Ķ.Length();Vector3D ù=Ə.ã[0].WorldMatrix.Forward;if(Ķ==Vector3D.Zero
||Ķ.Normalize().Equals(ù)){if(ĵ<k){foreach(var Ĵ in Ə.ƌ){if(ù==Ĵ.WorldMatrix.Backward)Ĵ.ThrustOverride+=0.001F;}}if(ĵ>k){
foreach(var Ĵ in Ə.ƌ){if(ù==Ĵ.WorldMatrix.Backward)Ĵ.ThrustOverride-=0.001F;}}}}public static void ĳ(double Ĳ,double ı,double İ
){var į=new Vector3D(Ĳ,ı,İ);var Į=Vector3D.TransformNormal(į,Ə.Ǝ.WorldMatrix);foreach(var ĭ in Ə.č){var Ĭ=Vector3D.
TransformNormal(Į,Matrix.Transpose(ĭ.WorldMatrix));ĭ.Pitch=(float)Ĭ.X;ĭ.Yaw=(float)Ĭ.Y;ĭ.Roll=(float)Ĭ.Z;ĭ.GyroOverride=true;}}public
static void ī(Vector3D Ī,Vector3D ĩ,MatrixD Ĩ,out double Â,out double Á,out double À){Ī=Ʒ.ư(Ī);MatrixD ħ;MatrixD.Transpose(ref
Ĩ,out ħ);Vector3D.Rotate(ref Ī,ref ħ,out Ī);Vector3D.Rotate(ref ĩ,ref ħ,out ĩ);Vector3D Ħ=Vector3D.Cross(ĩ,Ī);Vector3D ĥ;
double Ĥ;if(Vector3D.IsZero(ĩ)||Vector3D.IsZero(Ħ)){ĥ=new Vector3D(Ī.Y,-Ī.X,0);Ĥ=Math.Acos(MathHelper.Clamp(-Ī.Z,-1.0,1.0));}
else{Ħ=Ʒ.ư(Ħ);Vector3D ģ=Vector3D.Cross(Ī,Ħ);MatrixD Ģ=MatrixD.Zero;Ģ.Forward=Ī;Ģ.Left=Ħ;Ģ.Up=ģ;ĥ=new Vector3D(Ģ.M23-Ģ.M32,Ģ
.M31-Ģ.M13,Ģ.M12-Ģ.M21);double ġ=Ģ.M11+Ģ.M22+Ģ.M33;Ĥ=Math.Acos(MathHelper.Clamp((ġ-1)*0.5,-1,1));}if(Vector3D.IsZero(ĥ)){
Ĥ=Ī.Z<0?0:Math.PI;Á=Ĥ;Â=0;À=0;return;}ĥ=Ʒ.ư(ĥ);Á=-ĥ.Y*Ĥ;Â=-ĥ.X*Ĥ;À=-ĥ.Z*Ĥ;}public static void Ġ(string ĸ){List<Vector3D>Ö
=Ə.Ù();Vector3D ł=new Vector3D();int Ç=ĸ.Contains("-")?-1:1;if(ĸ.Contains("Gravity"))ł=Ç*Vector3D.Normalize(Ə.Ǝ.
GetNaturalGravity());if(ĸ.Contains("Connector"))ł=Ç*Ə.à.WorldMatrix.Forward;if(ĸ.Contains("Drills"))ł=Ç*Ə.ã[0].WorldMatrix.Forward;if(ĸ==
"Reset"){Ə.Ǝ.Direction=Base6Directions.EnumDirections[0];Ŵ.ŕ("Ship Remote Orientation Default");return;}if(ł!=default(Vector3D)
){for(int Õ=0;Õ<Ö.Count();Õ++){if(Ʒ.Ʋ(ł,Ö[Õ])){Ə.Ǝ.Direction=Base6Directions.EnumDirections[Õ];Ŵ.ŕ("Ship Remote Base "+
Base6Directions.EnumDirections[Õ].ToString());}}}}public static void ņ(){IMySensorBlock Ņ=(IMySensorBlock)Ŵ.ž("Follow",true);List<
MyDetectedEntityInfo>ń=new List<MyDetectedEntityInfo>();Ņ.DetectedEntities(ń);var Ń=ń.Find(delegate(MyDetectedEntityInfo A){return A.Type==
MyDetectedEntityType.CharacterHuman;});if(!Ń.IsEmpty()){Ŵ.ŕ("Human detected");if(!ţ.Ţ("Player_Pos")||Vector3D.Distance(Ń.Position,ţ.š(
"Player_Pos").Coords)>10){Vector3D Ĵ=Ʒ.ƶ(Ń.Position,Ə.Ǝ.WorldMatrix.Up,2);Vector3D ł=Ʒ.ƶ(Ń.Position,Vector3D.Normalize(Ĵ-Ə.Ǝ.
GetPosition()),2);ţ.ŝ("Player_Pos",ł);Ə.Ǝ.ClearWaypoints();Ə.Ǝ.AddWaypoint(ţ.š("Player_Pos").Coords,"Player");Ə.Ǝ.
SetAutoPilotEnabled(true);}}else{Ŵ.ŕ("Human not detected");}}public static void Ł(){if(Ə.ß>100-(Config.ToleranceBattery))Ƨ.Ƹ();}public
static void ŀ(){if(!(Ƨ.ê>Ƨ.Ƥ))ý();if(!(Ƨ.Ƥ>-1))return;if(Ƨ.ƣ.ì!="Stop")Ŵ.Ŕ();Ŵ.ŕ(Ƨ.ƣ.ì+" "+Ƨ.ƣ.e);switch(Ƨ.ƣ.ì){case"Goto":I()
;break;case"Lock":j();break;case"UnLock":u();break;case"Align":E();break;case"Mining":Ï();break;case
"AutopilotOrientation":Ġ(Ƨ.ƣ.e);Ƨ.Ƹ();break;case"Gyros":Ə.Ô(Ƨ.ƣ.e=="On");Ƨ.Ƹ();break;case"Remove":Ƨ.é(0);break;case"FreeFall":É();break;case
"CheckBattery":Ł();break;case"AddPlanet":Ë();break;case"CheckMining":Ì();break;case"ChangeSpeed":ª();Ƨ.Ƹ();break;case"Stop":ý();break;
case"Folower":ņ();break;case"Wait":C();break;case"Info":Ê();break;}}}static class Ŀ{public static string[]ľ={"Stop",
"FollowPath","AlignGravity","LandPlanet","GotoOrbit","GotoConnector","Goto","Unloader","Leave","AddPlanet","Loader","Miner",
"LookPlanet"};public static string[]Ľ(string ļ){string[]ĸ=ļ.Split(':');string Ē=ĸ[0];string đ=(ĸ.Length>1)?ĸ[1]:"";string ë=(ĸ.
Length>2)?ĸ[2]:"";return new string[]{Ē,đ,ë};}public static void I(string æ,string O=""){Ƨ.Ƣ("Goto",æ,O);}public static void Ļ
(string æ){Ƨ.Ƣ("Align","Access","",ţ.š(æ+"_Access_Forward").Coords,ţ.š(æ+"_Access_Up").Coords);}public static void ĺ(
string O="",bool Ą=false){if(Ą)Ƨ.ƹ();Ƨ.Ƣ("Align","Gravity",O);if(Ą)ý();}public static void Ĺ(string æ,string O){Ƨ.Ƣ(
"ScanPlanet",æ,O);}public static void Ġ(string æ){Ƨ.Ƣ("AutopilotOrientation",æ);}public static void j(string æ=""){Ƨ.Ƣ("Lock","",æ);
}public static void č(bool æ){Ƨ.Ƣ("Gyros",(æ)?"On":"Off");}public static void Č(string æ){Ƨ.Ƣ("ChangeSpeed",æ);}public
static void ý(){Ƨ.Ƣ("Stop","");}public static void ċ(string ÿ,bool Ċ=false,bool Ą=false){if(Ą)Ƨ.ƹ();I(ÿ+"_Access","Access");Ļ(
ÿ);Ƨ.Ƣ("Wait",Config.AlignementWaiting.ToString());Ļ(ÿ);č(false);I(ÿ+"_Access");if(Ċ)č(true);if(Ą)ý();}public static void
ĉ(string ÿ,bool Ĉ=false,string ć="",bool Ą=false){if(Ą)Ƨ.ƹ();Č("Cruising");if(Ą)Ğ(ÿ,true);ċ(ÿ);Ġ("Connector");Č("Docking"
);I(ÿ);if(Ĉ)j(ć);Ġ("Reset");č(true);Č("Cruising");if(Ą)ý();}public static void Ć(bool Ą=true){if(Ą)Ƨ.ƹ();ö.ð();Č(
"Cruising");I("Stable_Point","Access");Č("Max");č(true);ĺ("Backward");Ƨ.Ƣ("Wait","20");č(false);Ġ("Gravity");Ƨ.Ƣ("Goto",
"Orbit_Point","Gravity",Vector3D.Zero,Vector3D.Zero);if(Ą)ý();}public static void ą(bool Ą=true){if(Ą)Ƨ.ƹ();MyWaypointInfo X=ţ.Ş();if
(X.Coords!=Vector3D.Zero){if(Ə.Ǝ.GetNaturalGravity().Length()==0){Ƨ.Ƣ("Goto",X.Name,"Gravity",Vector3D.Zero,new Vector3D(
0.05,0,0));}ĺ("Down");Ƨ.Ƣ("FreeFall","Down");}else{Ŵ.Ŕ();Ŵ.ŕ("Planet unknow or so far more that 200km");Ŵ.ŕ(
"if it's a new planet add it with : ");Ŵ.ŕ("AddPlanet:name command");}if(Ą)ý();}public static void ă(string Ă=""){Ƨ.ƹ();if(ţ.Ţ("Planet_"+Ă)){Vector3D Ñ=
Vector3D.Normalize(Ə.Ǝ.GetPosition()-ţ.š("Planet_"+Ă).Coords);Ƨ.Ƣ("Align","Planet","",-Ñ,Vector3D.Zero);Ƨ.Ƣ("Info","Planet",Ă);}
ý();}public static void Ë(string Ă=""){Ƨ.ƹ();Ƨ.Ƣ("AddPlanet","Forward",Ă);ý();}public static void ā(){Ƨ.Ƣ("Goto",
"Mining_Access");Ƨ.Ƣ("AutopilotOrientation","Drill");Ļ("Mining");Ƨ.Ƣ("Gyros","Off");Ƨ.Ƣ("Goto","Mining_Access");}public static void Ā(
string ÿ="",string þ="",string Ď="",bool Ą=false){string A=(!string.IsNullOrEmpty(ÿ))?ÿ:ţ.ş(Ə.Ǝ.GetPosition()).Name;if(!string
.IsNullOrEmpty(A)){if(Ą){Ƨ.ƹ();}Č("Leaving");Ƨ.Ƣ("Gyros","Off");Ƨ.Ƣ("UnLock",þ,Ď);Ƨ.Ƣ("AutopilotOrientation","-Connector"
);Ƨ.Ƣ("Goto",A+"_Access");Ƨ.Ƣ("AutopilotOrientation","Reset");Ƨ.Ƣ("Gyros","On");Č("Cruising");if(Ą)Ƨ.Ƣ("Stop","");}}
public static void ğ(string e){Ğ(e,Vector3D.Distance(Ə.Ǝ.GetPosition(),ö.h(e).Coords)>Vector3D.Distance(Ə.Ǝ.GetPosition(),ö.f(
e).Coords),true);}public static void Ğ(string e,bool ĝ=false,bool Ą=false){if(Ą)Ƨ.ƹ();var Ĝ=(ĝ)?100:0;var O=(ĝ)?0:100;var
Ç=(ĝ)?-1:1;if(!ĝ)for(var Õ=0;Õ<=100;Õ++)if(ţ.Ţ("Path_"+e+"_"+Õ.ToString()))Ƨ.Ƣ("Goto","Path_"+e+"_"+Õ.ToString());if(ĝ)
for(var Õ=100;Õ>=0;Õ--)if(ţ.Ţ("Path_"+e+"_"+Õ.ToString()))Ƨ.Ƣ("Goto","Path_"+e+"_"+Õ.ToString());if(Ą)ý();}public static
void ě(){if(ţ.Ţ("Mining_Retry"))ţ.ſ("Mining_Retry");ţ.ŝ("Mining_Retry",Ʒ.ƶ(Ə.Ǝ.GetPosition(),-Ə.ã[0].WorldMatrix.Forward,5))
;I("Mining_Retry","ForceAlign");Ƨ.Ƣ("InitProcedure","Mining","6");}public static void Ě(){ĉ("Base",true,"Battery");Ā(
"Base","empty","Battery");Ğ("Loader");ĉ("Target",true);Ā("Target","full");Ğ("Loader",true);}public static void ę(){ĉ("Base",
true,"Battery");Ā("Base","full","Battery");Ğ("Loader");ĉ("Target",true);Ā("Target","empty");Ğ("Loader",true);}public static
void Ę(){Č("Cruising");ċ("Mining");Č("Docking");Ġ("Drills");I("Mining_Memory","ForceAlign");Ƨ.Ƣ("Mining","");Ļ("Mining");č(
false);Ġ("-Drills");I("Mining_Access","ForceAlign");ċ("Mining",true);Ƨ.Ƣ("CheckMining","");Č("Cruising");Ġ("Reset");č(true);Ğ
("Mining",true);ĉ("Base",true,"Battery");Ā("Base","empty","Battery");Ğ("Mining");}public static void ė(){Ğ("Patrol");Ƨ.Ƣ(
"CheckBattery","");}}static void Ė(string å){string Ē=Ŀ.Ľ(å)[0];string đ=Ŀ.Ľ(å)[1];string ë=Ŀ.Ľ(å)[2];if(Ƨ.ê!=0&&Ē!="Stop")return;if(Ŀ
.ľ.Contains(Ē)){Ŵ.Ŷ("Process",å);Ŵ.ű.Runtime.UpdateFrequency=UpdateFrequency.Update10;}switch(Ē){case"AddConnector":ö.î(đ
);break;case"SaveConnector":ö.í(đ,ë);break;case"AddAccessPoint":ö.ñ(đ);break;case"AddPlanet":Ŀ.Ë(đ);Ƨ.Ƹ();break;case
"AddMiningPoint":ö.Y(đ);break;case"AddPath":ö.ç(đ,Ə.Ǝ.GetPosition());break;case"Remove":ö.é(đ);break;case"AlignGravity":Ŀ.ĺ(đ,true);Ƨ.Ƹ(
);break;case"LookPlanet":Ŀ.ă(đ);Ƨ.Ƹ();break;case"FollowPath":Ŀ.ğ(đ);Ƨ.Ƹ();break;case"GotoConnector":Ŀ.ĉ(đ,true,"",true);Ƨ
.Ƹ();break;case"Goto":break;case"Leave":Ŀ.Ā(đ,"","",true);Ƨ.Ƹ();break;case"LandPlanet":Ŀ.ą();Ƨ.Ƹ();break;case"GotoOrbit":
Ŀ.Ć();Ƨ.Ƹ();break;case"Unloader":Ŀ.Ě();Ƨ.Ƹ();break;case"Loader":Ŀ.ę();Ƨ.Ƹ();break;case"Miner":Ŀ.Ę();Ƨ.Ƹ();break;case
"Next":Ƨ.Ƹ();break;case"Debug":Ŵ.Ŕ();Ŵ.ŕ("Angle F: "+Ʒ.ƴ(Ə.Ǝ.WorldMatrix.Forward,ţ.š("Base_Access_Forward").Coords));Ŵ.ŕ(
"Angle U: "+Ʒ.ƴ(Ə.Ǝ.WorldMatrix.Up,ţ.š("Base_Access_Up").Coords));Ŵ.ŕ("Distance : "+Vector3D.Distance(Ə.Ǝ.GetPosition(),ţ.š("Base")
.Coords));break;case"Storage":Ŵ.ű.Storage="init | Grid";ö.Ú();break;case"Stop":ö.ý();break;case"Clear":Ə.ƍ.ClearWaypoints
();break;}}Program(){bool ĕ=Me.CustomData.Contains("Reset");if(ĕ){Storage="";int Õ=Me.CustomData.IndexOf("Reset");Me.
CustomData=Me.CustomData.Remove(Õ,5);}Ŵ.Ú(GridTerminalSystem,Me,this);ö.Ú();ö.ý(false);string Ĕ=Ŵ.Ś("Process");Ŵ.Ŕ();Ŵ.ŕ(
"Initialisation [Reset Memory "+ĕ+"]");Ŵ.ŕ("");Ə.ø();string ē="-Screens : ";ē+=(Ŵ.Ű!=null)?"Action":"None";ē+=" | ";ē+=(Ŵ.ů!=null)?"Point":"None";ē+=
" | ";ē+=(Ŵ.Ů!=null)?"Info":"None";Ŵ.ŕ(ē);if(Ĕ!=null){string Ē=Ŀ.Ľ(Ĕ)[0];string đ=Ŀ.Ľ(Ĕ)[1];if(Ŀ.ľ.Contains(Ē)){Ƨ.Ƥ=int.Parse
(Ŵ.Ś("Cursor"))-1;Ė(Ĕ);}}}void Save(){}void Main(string å,UpdateType Đ){if(!string.IsNullOrEmpty(å))Ė(å);Ŵ.ŭ++;Ŵ.ŭ=Ŵ.ŭ%
Config.MaximumTick;ö.ŀ();Ŵ.ŗ();Ŵ.Ŗ();}