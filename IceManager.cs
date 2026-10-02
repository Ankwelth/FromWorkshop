/*
 * R e a d m e
 * -----------
 * 
 * Manage H2 and O2 generation
 * Patrick Heney
 * 
 * Ice-storage cargo must have "Cargo" in the name somewhere. This can be changed in Custom Data.
 * Tech'd O2/H2 generators must have the tech-type in the name somewhere, i.e., enhanced, proficient, elite.
 * Add the keyword "exclude" to any O2/H2 Generator, cargo container, hydrogen tank, and oxygen tanks that you don't want managed.
 * All keywords are case insensitive.
 * 
 * There is a function to have the manager periodically check for new O2/H2 generators.
 * To enable this, set REFRESH RATE in Custom Data to any value above 0. This value is in seconds.
 * This function can cause performance issues, so I recommend using a value of 900 (15 minutes).
 * To disable this behavior, leave this setting blank or put a 0 there, and recompile.
 * 
 * This script will also remove any excess ice from O2/H2 generators. 
 * Newly added generators will have their excess ice put back in cargo.
 * 
 * WARNING
 * When using in conjunction with Isy's Inventory Manager, you should disable ice balancing in IIM.
 * This can be done by setting the following option to false. Note this is a setting in IIM.
 * // Enable balancing of ice in O2/H2 generators?
 * // All O2/H2 generators will be used. To use one manually, add the manualMachineKeyword to it (by default: "!manual")
 * bool enableIceBalancing = false;
 */
static class Ĩ{private const string ħ="Ice Manager";private const int Ħ=1;private const int ĥ=3;private const int Ĥ=0;
private const int ģ=15;private const string Ģ="Version {0}.{1}.{2}.{3}";public static string ġ=string.Format(Ģ,Ħ,ĥ,Ĥ,ģ);private
const string Ġ="{0}\n{1}";public static string ğ=string.Format(Ġ,ħ,ġ);}MyDefinitionId Ğ=MyDefinitionId.Parse(
"MyObjectBuilder_Ore/Ice");List<IMyCargoContainer>E=new List<IMyCargoContainer>();List<IMyGasGenerator>F=new List<IMyGasGenerator>();List<
IMyGasTank>ĝ=new List<IMyGasTank>();List<IMyGasTank>Ĝ=new List<IMyGasTank>();float ĩ;bool Ī;bool Ĺ;float ĺ;int ĸ;DateTime ķ;bool Ķ
;bool ĵ;bool Ĵ;bool ĳ;bool Ĳ;bool ı;string İ;string į;ë Į;Program(){Me.GetSurface(0).WriteText(Ĩ.ğ,append:true);Me.
GetSurface(0).WriteText(ù.ToString(),append:true);Me.GetSurface(0).WriteText(ù.ToString(),append:true);Echo(string.Empty);Echo(Ĩ.ğ
);Echo(string.Empty);ķ=DateTime.Now;y();ĭ();Runtime.UpdateFrequency=UpdateFrequency.Update100;}void ĭ(){č();Š();ş();œ();š
();G(F,E);Ĭ();}void Ĭ(){if(Į!=null){Į=null;}if(!string.IsNullOrWhiteSpace(į)){Į=new ë(this,į);}}const char ù='\n';const
char ī=':';const char ú=' ';const string Č="ICE STORAGE";const string ę="Cargo";const string Ċ="DISPLAY TAG";const string ĉ=
"IceManager";const string Ĉ="FILL";string[]ć=new string[]{ā,"H2"};string[]Ć=new string[]{Ā,"O2"};const string ą=ā+" "+Ā;const string
ċ="CONSTRUCTION REFRESH RATE";const float Ą=15*60;const string Ă="Exclude";const string ā="Hydrogen";const string Ā=
"Oxygen";const string ÿ="SHOW ICE INVENTORY";const string þ="OVERRIDE ICE QUANTITY";const string ý="MULTIGRID TANKS";const
string ă="MULTIGRID GENERATORS";const string ü="MULTIGRID STORAGE";void č(){į=ĉ;İ=ę;Ī=true;Ĺ=true;ĺ=0;ĸ=0;bool Ě=false;bool Ę=
false;bool ė=false;bool Ė=false;bool ĕ=false;bool Ĕ=false;bool ē=false;bool Ē=false;bool đ=false;var Đ=Me.CustomData;var Ô=Đ.
Split(ù);foreach(var ä in Ô){if(ä.IndexOf(Ċ,StringComparison.OrdinalIgnoreCase)==0){int ď=ä.IndexOf(ī)+1;į=ä.Substring(ď).
Trim();Ě=true;continue;}if(ä.IndexOf(Č,StringComparison.OrdinalIgnoreCase)==0){int ď=ä.IndexOf(ī)+1;İ=ä.Substring(ď).Trim();
Ę=true;continue;}if(ä.IndexOf(Ĉ,StringComparison.OrdinalIgnoreCase)==0){int ď=ä.IndexOf(ī)+1;string Ç=ä.Substring(ď);Ī=ć.
Any(ě=>Ç.IndexOf(ě,StringComparison.OrdinalIgnoreCase)>-1);Ĺ=Ć.Any(ě=>Ç.IndexOf(ě,StringComparison.OrdinalIgnoreCase)>-1);ė
=true;continue;}if(ä.IndexOf(ċ,StringComparison.OrdinalIgnoreCase)==0){int ď=ä.IndexOf(ī)+1;string Ç=ä.Substring(ď);float
.TryParse(Ç.Trim(),out ĺ);y();Ė=true;}if(ä.IndexOf(ÿ,StringComparison.OrdinalIgnoreCase)==0){int ď=ä.IndexOf(ī)+1;string
Ç=ä.Substring(ď);bool.TryParse(Ç.Trim(),out Ĵ);ĕ=true;}if(ä.IndexOf(þ,StringComparison.OrdinalIgnoreCase)==0){int ď=ä.
IndexOf(ī)+1;string Ç=ä.Substring(ď);float Ŗ;float.TryParse(Ç.Trim(),out Ŗ);ĸ=(int)Ŗ;Ĕ=true;}if(ä.IndexOf(ý,StringComparison.
OrdinalIgnoreCase)==0){int ď=ä.IndexOf(ī)+1;string Ç=ä.Substring(ď);bool.TryParse(Ç.Trim(),out ĳ);ē=true;}if(ä.IndexOf(ă,StringComparison
.OrdinalIgnoreCase)==0){int ď=ä.IndexOf(ī)+1;string Ç=ä.Substring(ď);bool.TryParse(Ç.Trim(),out Ĳ);Ē=true;}if(ä.IndexOf(ü
,StringComparison.OrdinalIgnoreCase)==0){int ď=ä.IndexOf(ī)+1;string Ç=ä.Substring(ď);bool.TryParse(Ç.Trim(),out ı);đ=
true;}}Ī&=!string.IsNullOrWhiteSpace(ā);Ĺ&=!string.IsNullOrWhiteSpace(Ā);if(!Ě){ŕ(Ċ,ĉ);}if(!Ę){ŕ(Č,ę);}if(!ė){ŕ(Ĉ,ą);}if(!Ė)
{ŕ(ċ,string.Empty);}if(!ĕ){ŕ(ÿ,false.ToString());}if(!Ĕ){ŕ(þ,string.Empty);}if(!ē){ŕ(ý,false.ToString());}if(!Ē){ŕ(ă,
false.ToString());}if(!đ){ŕ(ü,false.ToString());}}void ŕ(string Ŕ,string Ç){if(!string.IsNullOrEmpty(Me.CustomData)&&Me.
CustomData.Length>0&&Me.CustomData[Me.CustomData.Length-1]!=ù){Me.CustomData+=ù;}Me.CustomData+=Ŕ+ī+ú+Ç+ù;}void œ(){E.Clear();E.
AddRange(Ś);E.Sort((ŗ,Ř)=>ŗ.CustomName.CompareTo(Ř.CustomName));}void š(){F.Clear();F.AddRange(Ş);F.Sort((ŗ,Ř)=>ŗ.CustomName.
CompareTo(Ř.CustomName));}void Š(){ĝ.Clear();ĝ.AddRange(Ŝ(ā));ĝ.Sort((ŗ,Ř)=>ŗ.CustomName.CompareTo(Ř.CustomName));}void ş(){Ĝ.
Clear();Ĝ.AddRange(Ŝ(Ā));Ĝ.Sort((ŗ,Ř)=>ŗ.CustomName.CompareTo(Ř.CustomName));}private List<IMyGasGenerator>Ş{get{var ŝ=new
List<IMyGasGenerator>();GridTerminalSystem.GetBlocksOfType(ŝ,t=>{if(!Ĳ&&!t.IsSameConstructAs(Me)){return false;}if(t.
CustomName.IndexOf(Ă,StringComparison.OrdinalIgnoreCase)>-1){return false;}t.UseConveyorSystem=false;t.AutoRefill=false;return
true;});return ŝ;}}List<IMyGasTank>Ŝ(string ś){var Ŀ=new List<IMyGasTank>();GridTerminalSystem.GetBlocksOfType(Ŀ,ï=>{if(!ĳ&&
!ï.IsSameConstructAs(Me)){return false;}if(ï.CustomName.IndexOf(Ă,StringComparison.OrdinalIgnoreCase)>-1){return false;}
if(string.IsNullOrWhiteSpace(ï.BlockDefinition.SubtypeName)&&ś.Equals(Ā)){return true;}return ï.BlockDefinition.
SubtypeName.IndexOf(ś,StringComparison.OrdinalIgnoreCase)>-1;});return Ŀ;}private List<IMyCargoContainer>Ś{get{var ř=new List<
IMyCargoContainer>();GridTerminalSystem.GetBlocksOfType(ř,t=>{if(!ı&&!t.IsSameConstructAs(Me)){return false;}if(t.CustomName.IndexOf(Ă,
StringComparison.OrdinalIgnoreCase)>-1){return false;}if(string.IsNullOrWhiteSpace(İ)){return true;}return t.CustomName.IndexOf(İ,
StringComparison.OrdinalIgnoreCase)>-1;});return ř;}}bool Ņ(IMyGasTank ï){if(ï==null){return false;}return ï.FilledRatio<1;}private bool
ń{get{return ĝ.Any(ï=>Ņ(ï));}}private bool Ń{get{return Ĝ.Any(ï=>Ņ(ï));}}float ł(List<IMyGasTank>Ŀ){if(Ŀ.Count==0){return
0;}return(float)Ŀ.Sum(ï=>ï.Capacity*ï.FilledRatio)/Ŀ.Sum(ï=>ï.Capacity);}bool ŀ(List<IMyGasTank>Ŀ){return Ŀ.Any(ï=>ï==
null);}const string ľ="Hydrogen Tanks: {0:P1}";const string Ľ="Oxygen Tanks: {0:P1}";const string ļ="Multigrid Tanks: {0}";
const string Ł="Multigrid Generators: {0}";const string Ļ="Multigrid Storage: {0}";const string ņ="ENABLED";const string Œ=
"disabled";const string ő="Last Update: {0}";const string Ő="< < No activity > >";const string ŏ="Moved {0:.##} kg ice";const
string Ŏ="Total Ice: {0:N} kg ice";const string ō="REFRESH";StringBuilder Ō=new StringBuilder();StringBuilder ŋ=new
StringBuilder();void Main(string û,UpdateType Ŋ){if(Ň(û)||å){ĭ();return;}Ķ=false;ĵ=false;if(Ī){if(ŀ(ĝ)){Š();}Ķ=ń;}if(Ĺ){if(ŀ(Ĝ)){ş();
}ĵ=Ń;}ŋ.Clear();ŋ.AppendStringBuilder(V(U:true));Echo(ŋ.ToString());Ō.Clear();Ō.AppendStringBuilder(V(U:false));í(Ō);if(Į
!=null){Į.ò(Ō.ToString(),ã:true);}if(ĩ>0){ĩ=0;}if(!Ķ&&!ĵ){x(F,false);return;}bool ŉ=false;bool ň=false;foreach(var D in F)
{if(D==null){ŉ=true;continue;}D.Enabled=true;D.UseConveyorSystem=false;D.AutoRefill=false;if(!n(D)){continue;}foreach(var
C in E){if(C==null){ň=true;continue;}if(!Å(C)){continue;}float À=o(D);if(Ã(C,D,À)){break;}}}if(ŉ){š();}if(ň){œ();}}bool Ň
(string û){return!string.IsNullOrWhiteSpace(û)&&û.IndexOf(ō,StringComparison.OrdinalIgnoreCase)==0;}void y(){ķ=ķ.
AddSeconds(ĺ);}private bool å{get{if(ĺ==0){return false;}return DateTime.Now>ķ;}}void x<w>(List<w>v,bool u)where w:
IMyFunctionalBlock{v.ForEach(t=>{if(t==null){return;}t.Enabled=u;});}private float s{get{return Ś.Sum(C=>z(C));}}const string r="enhanced"
;const string q="proficient";const string p="elite";int o(IMyGasGenerator m){if(m==null){return 0;}if(ĸ>0){return ĸ;}if(m
.CustomName.IndexOf(q,StringComparison.OrdinalIgnoreCase)>0){return 4;}if(m.CustomName.IndexOf(p,StringComparison.
OrdinalIgnoreCase)>0){return 8;}return 2;}bool n(IMyGasGenerator m){if(m==null){return false;}IMyInventory k=m.GetInventory(0);if(k==null
){return true;}int O=o(m);return k.GetItemAmount(Ğ)<O;}float z(IMyCargoContainer C){if(C==null){return 0;}IMyInventory k=
C.GetInventory();if(k==null){return 0;}MyInventoryItem?J=k.FindItem(Ğ);if(J==null){return 0;}return(float)J.
GetValueOrDefault().Amount;}bool Å(IMyCargoContainer C){return z(C)>0;}const float Ä=1f/1000;bool Ã(IMyCargoContainer Â,IMyGasGenerator Á
,float À){if(Â==null){return false;}IMyInventory K=Â.GetInventory();if(K==null){return false;}MyInventoryItem?J=K.
FindItem(Ğ);if(J==null){return false;}if(Á==null){return false;}IMyInventory P=Á.GetInventory();if(P==null){return false;}
MyInventoryItem?º=P.FindItem(Ğ);if(º!=null){À-=(float)º.GetValueOrDefault().Amount;if(À<=0){return false;}}float I=Math.Min((float)J.
GetValueOrDefault().Amount,À);bool µ=Math.Abs(I-À)<Ä;bool ª=P.TransferItemFrom(K,(MyInventoryItem)J,(MyFixedPoint)I);if(ª){ĩ+=I;}return ª
&&µ;}bool Æ(IMyGasGenerator m){if(m==null){return false;}IMyInventory k=m.GetInventory(0);if(k==null){return false;}int O=
o(m);return k.GetItemAmount(Ğ)>O*2;}bool N(IMyGasGenerator M,IMyCargoContainer L){if(M==null){return false;}IMyInventory
K=M.GetInventory();if(K==null){return false;}MyInventoryItem?J=K.FindItem(Ğ);if(J==null){return false;}if(L==null){return
false;}IMyInventory P=L.GetInventory();if(P==null){return false;}float I=(float)J.GetValueOrDefault().Amount;return P.
TransferItemFrom(K,(MyInventoryItem)J,(MyFixedPoint)I);}void G(List<IMyGasGenerator>F,List<IMyCargoContainer>E){foreach(var D in F){if(D
==null){continue;}if(Æ(D)){foreach(var C in E){if(C==null){continue;}if(N(D,C)){break;}}}}}const string B="ICE STORAGE";
const string H="O2/H2 GENERATORS";const string A="H2 TANKS";const string R="O2 TANKS";const string l="{0} (x{1})";const
string j=" - {0} [{1}]";const string h=" - {0} [[ {1} ]]";const string g="Good";const string f="Fault";const string e="Off";
const string d="ffff8080";const string c="ff00ff00";const string b="ffffff00";const string a="ffffffd0";const string Z=
"[color=#{0}]{1}[/color]";string Y(string X,string W){return string.Format(Z,W,X);}StringBuilder V(bool U=false){StringBuilder Q=new
StringBuilder();Q.AppendLine(Ĩ.ġ);Q.AppendLine(string.Format(ő,DateTime.Now.ToLongTimeString()));if(Ī){Q.AppendLine(string.Format(ľ,ł
(ĝ)));}if(Ĺ){Q.AppendLine(string.Format(Ľ,ł(Ĝ)));}if(ĳ){Q.AppendLine(string.Format(ļ,ņ));}if(Ĳ){Q.AppendLine(string.
Format(Ł,ņ));}if(ı){Q.AppendLine(string.Format(Ļ,ņ));}if(Ĵ){Q.AppendLine(string.Format(Ŏ,s));}if(ĩ>0){Q.AppendLine(string.
Format(ŏ,ĩ));}else{Q.AppendLine(Ő);}Q.AppendLine();Q.AppendLine(string.Format(l,B,E.Count));string S=U?h:j;foreach(var C in E)
{if(C==null){continue;}Q.AppendLine(string.Format(S,C.CustomName,î(C,U)));}Q.AppendLine();Q.AppendLine(string.Format(l,H,
F.Count));foreach(var D in F){if(D==null){continue;}Q.AppendLine(string.Format(S,D.CustomName,î(D,U)));}if(Ī){Q.
AppendLine();Q.AppendLine(string.Format(l,A,ĝ.Count));foreach(var ï in ĝ){if(ï==null){continue;}Q.AppendLine(string.Format(S,ï.
CustomName,î(ï,U)));}}if(Ĺ){Q.AppendLine();Q.AppendLine(string.Format(l,R,Ĝ.Count));foreach(var ï in Ĝ){if(ï==null){continue;}Q.
AppendLine(string.Format(S,ï.CustomName,î(ï,U)));}}return Q;}string î(IMyTerminalBlock t,bool U=false){if(!t.IsFunctional){return
U?Y(f,d):f;}if(t.IsWorking){return U?Y(g,c):g;}else{return U?Y(e,b):e;}}void í(StringBuilder ì){var È=Me as
IMyTextSurfaceProvider;IMyTextSurface à=È.GetSurface(0);à.ContentType=ContentType.TEXT_AND_IMAGE;à.WriteText(ì.ToString(),false);}class ë{
private string ê{set{if(string.IsNullOrWhiteSpace(value)){ô=string.Empty;}else{ô=string.Format(é,value);}}get{return ô;}}
private const string é="[{0}]";private const string è="Script tag: {0}";private const string ç="Found {0} LCDs";private const
string æ="Found {0} Screens";private const string ð="Identified displays: {0}";private const string ñ="{0} (surface {1})";
private const char ù='\n';private const char ú=' ';private const char ø='@';private string ô;private List<IMyTextSurface>Ê=new
List<IMyTextSurface>();private StringBuilder ì=new StringBuilder();private MyGridProgram ö;public ë(MyGridProgram ö,string ô
){this.ö=ö;õ(ô);}public void õ(){Ê.Clear();if(string.IsNullOrWhiteSpace(ê)){return;}Î(Ê);Ë(Ê);}public void õ(string ô){if
(string.IsNullOrWhiteSpace(ô)){return;}ê=ô;õ();}public void ó(){try{Ê.ForEach(Ð=>Ð.WriteText(string.Empty,false));}catch(
NullReferenceException){õ();}}public void ò(string Þ,bool ã=false){try{if(ã){ó();}Ê.ForEach(Ð=>Ð.WriteText(Þ,false));}catch(
NullReferenceException){õ();}}public void Ó(bool ã=false){ò(Ò().ToString(),ã);}public StringBuilder Ò(){var Ñ=Ê.Count(Ð=>Ð as IMyTextPanel!=
null);var Ï=Ê.Count()-Ñ;ì.AppendFormat(è,ê);ì.AppendLine();ì.AppendFormat(ç,Ñ);ì.AppendLine();ì.AppendFormat(æ,Ï);ì.
AppendLine();return ì;}private void Î(List<IMyTextSurface>Ê){ö.GridTerminalSystem.GetBlocksOfType(Ê,(t)=>{var Í=(t as
IMyTerminalBlock).CustomName.ToLower();bool Ì=Í.Contains(ô.ToLower());if(Ì){t.ContentType=ContentType.TEXT_AND_IMAGE;ö.Echo(string.
Format(ð,Í));}return Ì;});}private void Ë(List<IMyTextSurface>Ê){List<IMyTerminalBlock>É=new List<IMyTerminalBlock>();ö.
GridTerminalSystem.GetBlocksOfType(É,(t)=>{var È=t as IMyTextSurfaceProvider;if(È==null){return false;}bool Ì=t.CustomName.ToLower().
Contains(ô.ToLower());if(!Ì){return false;}var Ç=t.CustomData;var Ô=Ç.Split(ù);foreach(var ä in Ô){if(!ä.StartsWith(ø.ToString()
)){continue;}string â=ä.Substring(1).Split(ú)[0];int á;if(!int.TryParse(â,out á)){continue;}IMyTextSurface à=È.GetSurface
(á);à.ContentType=ContentType.TEXT_AND_IMAGE;string Ð=string.Format(ñ,t.CustomName,á);ö.Echo(string.Format(ð,Ð));Ê.Add(à)
;break;}return false;});}private void ß(IMyTextSurface Ð,string Þ){ì.Clear();ì.Append(Þ);string Ý=Ð.Font;float Ü=Ð.
FontSize;float Û=Ð.MeasureStringInPixels(ì,Ý,Ü).Y;float Ú=Ð.SurfaceSize.Y;int Ù=(int)Math.Floor(Ú/Û);string Ø=Ð.GetText()+ù+Þ;
var Ô=Ø.Split(ù);int Ö=Ô.Length;int Ď=Math.Max(0,Ö-Ù);ì.Clear();for(int Õ=Ď;Õ<Ö;Õ++){ì.Append(Ô[Õ]);if(Õ<Ö-1){ì.Append(ù);}
}Ð.WriteText(ì.ToString(),false);}}