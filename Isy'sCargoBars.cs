
// Isy's Cargo Bars
// =============
// Version: 1.0.5
// Date: 2022-02-21

// =======================================================================================
//                                                                            --- Configuration ---
// =======================================================================================

// --- LCD panels ---
// =======================================================================================

// The cargo bar will look best on a corner LCD. Just stick a corner LCD on a cargo container
// and add this keyword to its name! The script will automatically find the nearest cargo container.
string lcdKeyword = "[Cargobar]";

// The following options change the appearance of your cargo bars. These are the master options.
// All settings can also be done per LCD by editing the LCD's custom data.
const bool showContainerName = true;
const bool showExactValues = false;
const bool showPercentage = true;
const bool centerText = true;
const bool colorizeText = true;
const bool reverseColor = false;
const bool autoFontSize = true;

// When attached to a functional block show block stats instead of cargo?
// The following stats are shown:
// Tanks: Fill level
// Reactors, Hydrogen engines, Solar Panels, Wind Turbines: Current output
// Batteries: Stored power (Current output when false)
const bool showFunctionalBlockInfo = true;

// If you choose to colorize the text, the color will fade from green (0%) to yellow (75%) to red (100%).
// You can adjust the fill level where the color is pure yellow (default: 75%).
int yellowFillLevel = 75;

// Appearance of the bar
char bracketL = '[';
char bracketR = ']';
char emptyDot = '.';
char fullDot = 'I';


// --- Script execution ---
// =======================================================================================

// By default, the script is executed every seconds and refreshes 5 LCDs. If you want another timing
// or LCD amount, set the wanted values here.
int scriptExecutionTime = 1;
int lcdRefreshAmount = 5;


// =======================================================================================
//                                                                      --- End of Configuration ---
//                                                        Don't change anything beyond this point!
// =======================================================================================


List<IMyTextPanel>û=new List<IMyTextPanel>();List<IMyTerminalBlock>þ=new List<IMyTerminalBlock>();bool ÿ=false;bool Ā=
false;int ā=0;int Ă=0;int ă=3600;string[]Ą={"/","-","\\","|"};int ą=0;string[]Ć={
"If the LCD doesn't show the right block, you can specify it here:","=================================================","blockName=","",
"Individual LCD options (derived from master options):","=========================================","showFunctionalBlockInfo="+showFunctionalBlockInfo,"showContainerName="+
showContainerName,"showExactValues="+showExactValues,"showPercentage="+showPercentage,"centerText="+centerText,"colorizeText="+
colorizeText,"reverseColor="+reverseColor,"autoFontSize="+autoFontSize,};string ć;HashSet<string>Ĉ=new HashSet<string>();HashSet<
string>ĉ=new HashSet<string>();Program(){yellowFillLevel=yellowFillLevel%100;scriptExecutionTime*=60;Runtime.UpdateFrequency=
UpdateFrequency.Update1;}void Main(){if(ă<scriptExecutionTime){ă++;return;}else{ą=ą>=3?0:ą+1;ă=0;}if(!ÿ){List<IMyCargoContainer>ü=new
List<IMyCargoContainer>();GridTerminalSystem.GetBlocksOfType(ü,u=>u.IsSameConstructAs(Me));if(ü.Count>0){if(ü[0].
GetInventory(0).MaxVolume==VRage.MyFixedPoint.MaxValue||ü[0].GetInventory(0).MaxVolume==0)Ā=true;ÿ=true;}}ú();ę("Main");ı();if(ā==0)
{Ĉ=new HashSet<string>(ĉ);ĉ.Clear();if(Ĉ.Count==0)ć=null;}}void ú(){GridTerminalSystem.GetBlocksOfType(û,f=>f.CustomName.
Contains(lcdKeyword)&&f.IsSameConstructAs(Me));if(û.Count==0){č("No LCDs with the keyword '"+lcdKeyword+"' found!");return;}for(
int m=ā;m<û.Count;m++){if(Ă>=lcdRefreshAmount){Ă=0;return;}else{if(ā>=û.Count-1){ā=0;Ă=0;}else{ā++;Ă++;}}Vector3 å=û[m].
Position;string æ=ċ(û[m],"blockName");if(æ!=""){GridTerminalSystem.GetBlocksOfType(þ,f=>f.CustomName.Contains(æ)&&(f.
HasInventory||(f is IMyBatteryBlock)||(f is IMyJumpDrive)||(f is IMyPowerProducer)));if(þ.Count==0){č("'"+æ+
"'\nspecified in the custom data of:\n'"+û[m].CustomName+"'\nwas not found!");İ(û[m],"The blockname specified in the custom data was not found:\n'"+æ+"'");
continue;}}else{GridTerminalSystem.GetBlocksOfType(þ,f=>Vector3.Distance(å,f.Position)<=3&&(f.HasInventory||(f is
IMyBatteryBlock)||(f is IMyJumpDrive)||(f is IMyPowerProducer))&&f.IsSameConstructAs(Me));if(þ.Count==0){č("'"+û[m].CustomName+
"'\nhas no useable blocks nearby!");İ(û[m],"There are no useable blocks nearby!");continue;}}þ.Sort((ç,f)=>Vector3.Distance(å,ç.Position).CompareTo(
Vector3.Distance(å,f.Position)));var Ô=þ[0];bool è=showFunctionalBlockInfo;bool é=showContainerName;bool ê=showExactValues;bool
ë=showPercentage;bool ì=centerText;bool í=colorizeText;bool î=reverseColor;bool ï=autoFontSize;è=ĳ(û[m],
"showFunctionalBlockInfo");é=ĳ(û[m],"showContainerName");ê=ĳ(û[m],"showExactValues");ë=ĳ(û[m],"showPercentage");ì=ĳ(û[m],"centerText");í=ĳ(û[m],
"colorizeText");î=ĳ(û[m],"reverseColor");ï=ĳ(û[m],"autoFontSize");string ð="";double ñ=0;double ò=1;if(Ô.HasInventory){var ó=Ô.
GetInventory(0);ñ=(double)ó.CurrentVolume;ò=(double)ó.MaxVolume;if(Ā)ò=int.MaxValue;ð=ñ.Á()+" / "+ò.Á();}else if(Ô is IMyJumpDrive){
var ô=Ô as IMyJumpDrive;ñ=ô.CurrentStoredPower;ò=ô.MaxStoredPower;ð=ñ.Ç(true)+" / "+ò.Ç(true);}if(è){if(Ô is IMyGasTank){
var õ=Ô as IMyGasTank;ò=õ.Capacity;ñ=ò*(float)õ.FilledRatio;ð=ñ.Ê()+" / "+ò.Ê();}else if(Ô is IMyBatteryBlock){var ö=Ô as
IMyBatteryBlock;ñ=ö.CurrentStoredPower;ò=ö.MaxStoredPower;ð=ñ.Ç(true)+" / "+ò.Ç(true);}else if(Ô is IMyPowerProducer){var ø=Ô as
IMyPowerProducer;ñ=ø.CurrentOutput;ò=ø.MaxOutput;ð=ñ.Ç()+" / "+ò.Ç();}}else{if(Ô is IMyBatteryBlock){var ö=Ô as IMyBatteryBlock;ñ=ö.
CurrentOutput;ò=ö.MaxOutput;ð=ñ.Ç()+" / "+ò.Ç();}}double ù=ñ/ò;StringBuilder P=new StringBuilder();if(é){P.Append(System.Text.
RegularExpressions.Regex.Replace(Ô.CustomName,@"\(\d+\.?\d*\%\)","")+"\n");}float º=û[m].H();if(ï){û[m].FontSize=0.1f;int ý=û[m].B();int Ċ
=1;if(é)Ċ+=1;if(ê)Ċ+=1;û[m].FontSize*=ý/Ċ;if(é){float Ĭ=û[m].z(P);if(Ĭ>º)û[m].FontSize*=º/Ĭ;}if(ê){float ġ=û[m].z(ð);if(ġ
>º)û[m].FontSize*=º/ġ;}}if(ê)P.Append(ð+"\n");StringBuilder Ģ=new StringBuilder();if(ë)Ģ.Append(" "+ñ.Ã(ò));StringBuilder
ģ=new StringBuilder();ģ=ĩ(û[m],º-û[m].z(Ģ),ù);ģ.Append(Ģ).Append("\n");P.Append(ģ);if(ì){û[m].Alignment=VRage.Game.GUI.
TextPanel.TextAlignment.CENTER;}else{û[m].Alignment=VRage.Game.GUI.TextPanel.TextAlignment.LEFT;}if(í){if((Ô is IMyBatteryBlock)
&&è||((Ô is IMyPowerProducer)&&!(Ô is IMyBatteryBlock)&&!è)||(Ô is IMyJumpDrive)||(Ô is IMyShipWelder)||Ô.BlockDefinition.
TypeIdString.Contains("Oxygen")||(Ô.GetType().ToString().Contains("Weapons")&&!(Ô is IMyShipDrill)&&!(Ô is IMyShipGrinder))){Ĥ(û[m],
ù,!î);}else{Ĥ(û[m],ù,î);}}û[m].WritePublicTitle("Cargo bar for: '"+Ô.CustomName+"'");û[m].WriteText(P);û[m].ContentType=
VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;}}void Ĥ(IMyTextPanel C,double ù,bool ĥ){ù*=100;int Ħ,ħ;if(ĥ){int Ĩ=100-
yellowFillLevel;if(ù<Ĩ){ħ=(int)(ù/Ĩ*255);Ħ=255;}else{ħ=255;Ħ=(int)((100-ù)/(100-Ĩ)*255);}}else{if(ù<yellowFillLevel){ħ=255;Ħ=(int)(ù/
yellowFillLevel*255);}else{ħ=(int)((100-ù)/(100-yellowFillLevel)*255);Ħ=255;}}C.FontColor=new Color(Ħ,ħ,0);}StringBuilder ĩ(
IMyTextSurface C,float x,double Ī){StringBuilder ī,ĭ;float Ĵ=C.µ(bracketL);float Į=C.µ(bracketR);float į=x-Ĵ-Į;ī=C.I(fullDot,į*Ī);ĭ=C.
I(emptyDot,į-C.z(ī));return new StringBuilder().Append(bracketL).Append(ī).Append(ĭ).Append(bracketR);}void İ(
IMyTextPanel C,string ć){float º=C.H();string[]P={"LCD configuration error!"};P=P.Concat(ć.Split('\n')).ToArray();C.FontSize=10f;
foreach(var Ë in P){float Í=C.z(Ë);if(Í>º)C.FontSize*=º/Í;}C.FontColor=new Color(255,0,0);C.WritePublicTitle("Cargo bar error")
;C.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.CENTER;C.WriteText(String.Join("\n",P)+"\n");C.TextPadding=0;C.
ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;}void ı(){Echo("Isy's Cargo Bars "+Ą[ą]+"\n==============\n");if(ć
!=null){Echo("Error!\n"+ć+"\n");}string Ĳ="";Ĳ+="Currently showing cargo bars on:\n"+û.Count+" LCDs\n\n";Ĳ+=
"Master options:\n";Ĳ+="Show container name: "+(showContainerName?"true":"false")+"\n";Ĳ+="Show exact value: "+(showExactValues?"true":
"false")+"\n";Ĳ+="Show percentage: "+(showPercentage?"true":"false")+"\n";Ĳ+="Center text: "+(centerText?"true":"false")+"\n";Ĳ
+="Colorize text: "+(colorizeText?"true":"false")+"\n";Ĳ+="Reverse Color: "+(reverseColor?"true":"false")+"\n";Ĳ+=
"Auto font size: "+(autoFontSize?"true":"false")+"\n\n";Echo(Ĳ+Ď);}bool ĳ(IMyTextPanel C,string â){Č(C);var Ý=C.CustomData.Replace(" ","")
.Split('\n');foreach(var Ë in Ý){if(Ë.Contains(â+"=")){try{return Convert.ToBoolean(Ë.Replace(â+"=",""));}catch{return
true;}}}return true;}string ċ(IMyTextPanel C,string â){Č(C);var Ý=C.CustomData.Split('\n');foreach(var Ë in Ý){if(Ë.Contains
(â+"=")){return Ë.Replace(â+"=","");}}return"";}void Č(IMyTextPanel C){var Ý=C.CustomData.Split('\n');if(Ý.Length!=Ć.
Length){C.CustomData=String.Join("\n",Ć);}}void č(string P){Ĉ.Add(P);ĉ.Add(P);ć=Ĉ.ElementAt(0);}StringBuilder Ď=new
StringBuilder("No performance Information available!");Dictionary<string,int>ď=new Dictionary<string,int>();List<int>Đ=new List<int>(
new int[600]);List<double>đ=new List<double>(new double[600]);double Ē,ē,Ĕ,ĕ,Ė;int ė,Ę=0;void ę(string Ě){Ę=Ę>=599?0:Ę+1;ė=
Runtime.CurrentInstructionCount;if(ė>ē)ē=ė;Đ[Ę]=ė;ĕ=Đ.Sum()/Đ.Count;Ď.Clear();Ď.Append("Instructions: "+ė+" / "+Runtime.
MaxInstructionCount+"\n");Ď.Append("Max. Instructions: "+ē+" / "+Runtime.MaxInstructionCount+"\n");Ď.Append("Avg. Instructions: "+Math.
Floor(ĕ)+" / "+Runtime.MaxInstructionCount+"\n\n");Ē=Runtime.LastRunTimeMs;if(Ē>Ĕ&&ď.ContainsKey(Ě))Ĕ=Ē;đ[Ę]=Ē;Ė=đ.Sum()/đ.
Count;Ď.Append("Last runtime: "+Math.Round(Ē,4)+" ms\n");Ď.Append("Max. runtime: "+Math.Round(Ĕ,4)+" ms\n");Ď.Append(
"Avg. runtime: "+Math.Round(Ė,4)+" ms\n\n");Ď.Append("Instructions per Method:\n");ď[Ě]=ė;foreach(var ě in ď.OrderByDescending(m=>m.
Value)){Ď.Append("- "+ě.Key+": "+ě.Value+"\n");}Ď.Append("\n");}List<IMyTerminalBlock>Ĝ(string Õ,string[]Ć=null,string ĝ=
"Debug",float Ğ=0.6f,float ğ=2f){string Ġ="[IsyLCD]";var û=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<
IMyTextSurfaceProvider>(û,f=>f.IsSameConstructAs(Me)&&(f.CustomName.Contains(Õ)||(f.CustomName.Contains(Ġ)&&f.CustomData.Contains(Õ))));var d=
û.FindAll(f=>f.CustomName.Contains(Õ));foreach(var C in d){C.CustomName=C.CustomName.Replace(Õ,"").Replace(" "+Õ,"").
TrimEnd(' ');bool h=false;bool j=false;int k=0;if(C is IMyTextSurface){if(!C.CustomName.Contains(Ġ))h=true;if(!C.CustomData.
Contains(Õ)){j=true;C.CustomData="@0 "+Õ+(Ć!=null?"\n"+String.Join("\n",Ć):"");}}else if(C is IMyTextSurfaceProvider){if(!C.
CustomName.Contains(Ġ))h=true;int l=(C as IMyTextSurfaceProvider).SurfaceCount;for(int m=0;m<l;m++){if(!C.CustomData.Contains("@"+
m)){j=true;k=m;C.CustomData+=(C.CustomData==""?"":"\n\n")+"@"+m+" "+Õ+(Ć!=null?"\n"+String.Join("\n",Ć):"");break;}}}else
{û.Remove(C);}if(h)C.CustomName+=" "+Ġ;if(j){var n=(C as IMyTextSurfaceProvider).GetSurface(k);n.Font=ĝ;n.FontSize=Ğ;n.
TextPadding=ğ;n.Alignment=TextAlignment.LEFT;n.ContentType=ContentType.TEXT_AND_IMAGE;}}return û;}
}public static partial class o{private static Dictionary<char,float>p=new Dictionary<char,float>();public static void q(
string s,float t){foreach(char u in s){p[u]=t;}}public static void w(){if(p.Count>0)return;q(
"3FKTabdeghknopqsuy£µÝàáâãäåèéêëðñòóôõöøùúûüýþÿāăąďđēĕėęěĝğġģĥħĶķńņňŉōŏőśŝşšŢŤŦũūŭůűųŶŷŸșȚЎЗКЛбдекруцяёђћўџ",18);q("ABDNOQRSÀÁÂÃÄÅÐÑÒÓÔÕÖØĂĄĎĐŃŅŇŌŎŐŔŖŘŚŜŞŠȘЅЊЖф□",22);q("#0245689CXZ¤¥ÇßĆĈĊČŹŻŽƒЁЌАБВДИЙПРСТУХЬ€",20);q(
"￥$&GHPUVY§ÙÚÛÜÞĀĜĞĠĢĤĦŨŪŬŮŰŲОФЦЪЯжы†‡",21);q("！ !I`ijl ¡¨¯´¸ÌÍÎÏìíîïĨĩĪīĮįİıĵĺļľłˆˇ˘˙˚˛˜˝ІЇії‹›∙",9);q("？7?Jcz¢¿çćĉċčĴźżžЃЈЧавийнопсъьѓѕќ",17);q(
"（）：《》，。、；【】(),.1:;[]ft{}·ţťŧț",10);q("+<=>E^~¬±¶ÈÉÊË×÷ĒĔĖĘĚЄЏЕНЭ−",19);q("L_vx«»ĹĻĽĿŁГгзлхчҐ–•",16);q("\"-rª­ºŀŕŗř",11);q("WÆŒŴ—…‰",32);q("'|¦ˉ‘’‚",7)
;q("@©®мшњ",26);q("mw¼ŵЮщ",28);q("/ĳтэє",15);q("\\°“”„",13);q("*²³¹",12);q("¾æœЉ",29);q("%ĲЫ",25);q("MМШ",27);q("½Щ",30);
q("ю",24);q("ј",8);q("љ",23);q("ґ",14);q("™",31);}public static Vector2 ª(this IMyTextSurface n,StringBuilder P){w();
Vector2 x=new Vector2();if(n.Font=="Monospace"){float y=n.FontSize;x.X=(float)(P.Length*19.4*y);x.Y=(float)(28.8*y);return x;}
else{float y=(float)(n.FontSize*0.779);foreach(char u in P.ToString()){try{x.X+=p[u]*y;}catch{}}x.Y=(float)(28.8*n.FontSize)
;return x;}}public static float z(this IMyTextSurface C,StringBuilder P){Vector2 G=C.ª(P);return G.X;}public static float
z(this IMyTextSurface C,string P){Vector2 G=C.ª(new StringBuilder(P));return G.X;}public static float µ(this
IMyTextSurface C,char A){float Z=z(C,new string(A,1));return Z;}public static int B(this IMyTextSurface C){Vector2 D=C.SurfaceSize;
float E=C.TextureSize.Y;if(D.X<512||E!=D.Y)D.Y*=512/E;float F=D.Y*(100-C.TextPadding*2)/100;Vector2 G=C.ª(new StringBuilder(
"T"));return(int)(F/G.Y);}public static float H(this IMyTextSurface C){Vector2 D=C.SurfaceSize;float E=C.TextureSize.Y;if(D
.X<512||E!=D.Y)D.X*=512/E;return D.X*(100-C.TextPadding*2)/100;}public static StringBuilder I(this IMyTextSurface C,char
J,double K){int L=(int)(K/µ(C,J));if(L<0)L=0;return new StringBuilder().Append(J,L);}private static DateTime M=DateTime.
Now;private static Dictionary<int,List<int>>N=new Dictionary<int,List<int>>();public static StringBuilder O(this
IMyTextSurface C,StringBuilder P,int Q=3,bool R=true,int S=0){int T=C.GetHashCode();if(!N.ContainsKey(T)){N[T]=new List<int>{1,3,Q,0};
}int U=N[T][0];int V=N[T][1];int W=N[T][2];int X=N[T][3];var Y=P.ToString().TrimEnd('\n').Split('\n');List<string>v=new
List<string>();if(S==0)S=C.B();float º=C.H();StringBuilder Ù,Ì=new StringBuilder();for(int m=0;m<Y.Length;m++){if(m<Q||m<W||
v.Count-W>S||C.z(Y[m])<=º){v.Add(Y[m]);}else{try{Ì.Clear();float Í,Î;var Ï=Y[m].Split(' ');string Ð=System.Text.
RegularExpressions.Regex.Match(Y[m],@"\d+(\.|\:)\ ").Value;Ù=C.I(' ',C.z(Ð));foreach(var Ñ in Ï){Í=C.z(Ì);Î=C.z(Ñ);if(Í+Î>º){v.Add(Ì.
ToString());Ì=new StringBuilder(Ù+Ñ+" ");}else{Ì.Append(Ñ+" ");}}v.Add(Ì.ToString());}catch{v.Add(Y[m]);}}}if(R){if(v.Count>S){
if(DateTime.Now.Second!=X){X=DateTime.Now.Second;if(V>0)V--;if(V<=0)W+=U;if(W+S-Q>=v.Count&&V<=0){U=-1;V=3;}if(W<=Q&&V<=0)
{U=1;V=3;}}}else{W=Q;U=1;V=3;}N[T][0]=U;N[T][1]=V;N[T][2]=W;N[T][3]=X;}else{W=Q;}StringBuilder Ò=new StringBuilder();for(
var Ë=0;Ë<Q;Ë++){Ò.Append(v[Ë]+"\n");}for(var Ë=W;Ë<v.Count;Ë++){Ò.Append(v[Ë]+"\n");}return Ò;}public static Dictionary<
IMyTextSurface,string>Ó(this IMyTerminalBlock Ô,string Õ,Dictionary<string,string>Ö=null){var Ø=new Dictionary<IMyTextSurface,string>(
);if(Ô is IMyTextSurface){Ø[Ô as IMyTextSurface]=Ô.CustomData;}else if(Ô is IMyTextSurfaceProvider){var Ú=System.Text.
RegularExpressions.Regex.Matches(Ô.CustomData,@"@(\d).*("+Õ+@")");int ä=(Ô as IMyTextSurfaceProvider).SurfaceCount;foreach(System.Text.
RegularExpressions.Match Û in Ú){int Ü=-1;if(int.TryParse(Û.Groups[1].Value,out Ü)){if(Ü>=ä)continue;string Ý=Ô.CustomData;int Þ=Ý.IndexOf
("@"+Ü);int ß=Ý.IndexOf("@",Þ+1)-Þ;string à=ß<=0?Ý.Substring(Þ):Ý.Substring(Þ,ß);Ø[(Ô as IMyTextSurfaceProvider).
GetSurface(Ü)]=à;}}}return Ø;}public static bool á(this string à,string â){var Ý=à.Replace(" ","").Split('\n');foreach(var Ë in Ý)
{if(Ë.StartsWith(â+"=")){try{return Convert.ToBoolean(Ë.Replace(â+"=",""));}catch{return true;}}}return true;}public
static string ã(this string à,string â){var Ý=à.Replace(" ","").Split('\n');foreach(var Ë in Ý){if(Ë.StartsWith(â+"=")){return
Ë.Replace(â+"=","");}}return"";}}public static partial class o{public static string Á(this float e){string À="kL";if(e<1)
{e*=1000;À="L";}else if(e>=1000&&e<1000000){e/=1000;À="ML";}else if(e>=1000000&&e<1000000000){e/=1000000;À="BL";}else if(
e>=1000000000){e/=1000000000;À="TL";}return Math.Round(e,1)+" "+À;}public static string Á(this double e){float Â=(float)e
;return Â.Á();}}public static partial class o{public static string Ã(this double Ä,double Å){double Æ=Math.Round(Ä/Å*100,
1);if(Å==0){return"0%";}else{return Æ+"%";}}public static string Ã(this float Ä,float Å){double Æ=Math.Round(Ä/Å*100,1);
if(Å==0){return"0%";}else{return Æ+"%";}}}public static partial class o{public static string Ç(this float e,bool È=false){
string À="MW";string É=e<0?"-":"";e=Math.Abs(e);if(e<1){e*=1000;À="kW";}else if(e>=1000&&e<1000000){e/=1000;À="GW";}else if(e
>=1000000&&e<1000000000){e/=1000000;À="TW";}else if(e>=1000000000){e/=1000000000;À="PW";}if(È)À+="h";return É+Math.Round(e
,1)+" "+À;}public static string Ç(this double e,bool È=false){float Â=(float)e;return Â.Ç(È);}}public static partial
class o{public static string Ê(this float e){string À="L";if(e>=1000&&e<1000000){e/=1000;À="KL";}else if(e>=1000000&&e<
1000000000){e/=1000000;À="ML";}else if(e>=1000000000&&e<1000000000000){e/=1000000000;À="BL";}else if(e>=1000000000000){e/=
1000000000000;À="TL";}return Math.Round(e,1)+" "+À;}public static string Ê(this double e){float Â=(float)e;return Â.Ê();}