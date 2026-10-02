// GOOSE
// Organizes Objects & Sorts Everything
// 
// Created by Duke Skyloafer
// Version 0.33.0
// 
// Goose sorts items between containers, keeps stock containers topped up,
// and feeds reactors, gas generators, and weapons.
// 
// Full setup and usage guide:
// https://github.com/briansokol/SE-Goose/wiki/Goose-Setup-and-Configuration
// 
public enum E{A,B,C,D}private static MyItemType?F;private static MyItemType G{get{if(!F.HasValue){F=new MyItemType(
"MyObjectBuilder_Ingot","Uranium");}return F.Value;}}private static MyItemType?H;private static MyItemType I{get{if(!H.HasValue){H=new
MyItemType("MyObjectBuilder_Ore","Ice");}return H.Value;}}private static MyItemType?J;private static MyItemType K{get{if(!J.
HasValue){J=new MyItemType("MyObjectBuilder_Component","SteelPlate");}return J.Value;}}internal static float N(float L,int M){
return L*(M/100f);}internal static long S(double O,int P){if(P<=0){return 0L;}double Q=O*1000.0;if(Q<0.0){Q=0.0;}long R=(long)
(Q/1000.0)+1L;return R*(long)P;}private static readonly string[]T=new string[]{"NATO_25x184mm","NATO_5p56x45mm",
"Missile200mm","AutocannonClip","MediumCalibreAmmo","LargeCalibreAmmo"};private readonly U V=new U();private readonly W X=new W();
private readonly Dictionary<MyItemType,float>Y=new Dictionary<MyItemType,float>();private readonly Z a=new Z();float b;int c=-1
;bool f(){if(c==d){return false;}e();c=d;return true;}void e(){V.Clear();X.Clear();for(int g=0;g<T.Length;g++){string h=T
[g];V.Add(new MyItemType("MyObjectBuilder_AmmoMagazine",h));X.Add("AmmoMagazine/"+h);}foreach(KeyValuePair<string,
MyItemType>j in i){if(j.Key.StartsWith("AmmoMagazine/",StringComparison.Ordinal)&&!X.Contains(j.Key)){V.Add(j.Value);X.Add(j.Key);
}}}IEnumerator<k>Ã(){if(f()){foreach(KeyValuePair<IMyTerminalBlock,l>j in m){if(j.Value!=null){j.Value.n=true;}}}int o=0;
foreach(KeyValuePair<IMyTerminalBlock,l>j in m){l p=j.Value;if(p==null){continue;}IMyTerminalBlock r=p.q;if(r==null){continue;}
if(p.n){p.n=false;IMyInventory t=p.s;p.u=v(r.CustomName);if(t==null){p.E=E.A;p.w=null;}else if(x.y(r.CustomName,
"[NoBalance]")){p.E=E.A;p.w=null;}else{z(p,t);if(p.E==E.A&&!(p.q is IMyCargoContainer)&&t.CurrentVolume>=t.MaxVolume){p.n=true;}}if(ª
(p.E,p.u,µ(p.E))){º(r);}}o++;if(o%25==0){yield return k.À;}if(Á()){yield return k.Â;}}}internal static bool ª(E Ä,long Å,
int Æ){if(Ä==E.A){return false;}if(Å>=0){return true;}return Æ>0;}int µ(E Ä){switch(Ä){case E.B:return Ç.È;case E.C:return
Ç.É;case E.D:return Ç.Ê;default:return 0;}}void º(IMyTerminalBlock r){if(r==null){return;}try{if(r.GetValueBool(
"UseConveyor")){r.SetValueBool("UseConveyor",false);if(Ç.Ë){Ì("balance: disabled UseConveyor on "+r.CustomName);}}}catch{}}void z(l p
,IMyInventory t){p.E=E.A;p.w=null;if(p.q is IMyReactor){p.E=E.B;return;}if(p.q is IMyGasGenerator){p.E=E.C;return;}
MyFixedPoint Í=MyFixedPoint.SmallestPossibleValue;if(t.CanItemsBeAdded(Í,K)){return;}U Î=null;for(int g=0;g<V.Count;g++){MyItemType
Ï=V[g];if(t.CanItemsBeAdded(Í,Ï)){if(Î==null){Î=new U();}Î.Add(Ï);}}if(Î!=null){p.E=E.D;p.w=Î;}}IEnumerator<k>Õ(){a.Clear
();foreach(KeyValuePair<IMyTerminalBlock,l>j in m){l p=j.Value;if(p==null){continue;}if(p.s==null){continue;}if(Ð!=null&&
p.q!=null&&Ð.Ñ(p.q.EntityId)){continue;}if(p.E!=E.A&&!Ò(p.q)){continue;}a.Add(p);}IEnumerator<k>Ó;Ó=Ô(E.B,Ç.È);while(Ó.
MoveNext()){yield return Ó.Current;}Ó=Ô(E.C,Ç.É);while(Ó.MoveNext()){yield return Ó.Current;}Ó=Ô(E.D,Ç.Ê);while(Ó.MoveNext()){
yield return Ó.Current;}}void æ(IMyInventory Ö,IMyInventory Ø,MyItemType Ù,float Ú,bool Û){if(Û?(float)Ö.CurrentVolume<=Ú:(
float)Ö.CurrentVolume>=Ú){return;}IMyInventory Ü=Û?Ö:Ø;IMyInventory Ý=Û?Ø:Ö;string Þ=Û?"balance-excess":"balance";float ß;if(
!Y.TryGetValue(Ù,out ß)||ß<=0f){float à=(float)Ö.CurrentVolume;long â=á(Ü,Ý,Ù,1,Þ);if(â==0){return;}float ã=(float)Ö.
CurrentVolume;ß=(Û?à-ã:ã-à)/â;if(ß>0f){Y[Ù]=ß;}if(Û?(float)Ö.CurrentVolume<=Ú:(float)Ö.CurrentVolume>=Ú){return;}}if(ß<=0f){return;}
float ä=Û?(float)Ö.CurrentVolume-Ú:Ú-(float)Ö.CurrentVolume;long å=(long)System.Math.Ceiling(ä/ß);if(å<=0){return;}á(Ü,Ý,Ù,å,
Þ);}Z î(ç è,string é){Z ê;if(!ë.TryGetValue(è,out ê)||ê==null||ê.Count==0){ì("balancer:no-route:"+è,
"Balancer cannot push excess "+é+": no container tagged "+í(è));return null;}return ê;}internal static float ó(float ï,float ð,float ñ){if(ñ<=0f){
return 1f;}float ò=ï-ð;if(ò<=0f){return 0f;}if(ò>=ñ){return 1f;}return ò/ñ;}float ý(E Ä,int Æ){if(Æ==0){return 1f;}if(Ä==E.A){
return 1f;}float ï;float ð;float ñ;if(Ä==E.B){MyItemType Ù=G;float ß;if(!Y.TryGetValue(Ù,out ß)||ß<=0f){return 1f;}long ô=0;
long õ=0;foreach(KeyValuePair<IMyTerminalBlock,l>j in m){l p=j.Value;if(p==null||p.E!=Ä){continue;}if(p.s==null){continue;}
if(p.u>=0){õ+=p.u;}else{ô+=S((double)p.s.MaxVolume,Æ);}}if(ô<=0){return 1f;}long ø=ö(Ù);ï=ø*ß;ð=õ*ß;ñ=ô*ß;}else{ñ=0f;
foreach(KeyValuePair<IMyTerminalBlock,l>j in m){l p=j.Value;if(p==null||p.E!=Ä){continue;}if(p.u>=0){continue;}if(p.s==null){
continue;}ñ+=(float)p.s.MaxVolume*(Æ/100f);}if(ñ<=0f){return 1f;}if(Ä==E.D){ï=ù();if(ï<=0f){return 1f;}float û=ú();long ü=0;
foreach(KeyValuePair<IMyTerminalBlock,l>j in m){l p=j.Value;if(p==null||p.E!=E.D){continue;}if(p.u<0){continue;}ü+=p.u;}ð=ü*û;}
else{MyItemType Ù=I;float ß;if(!Y.TryGetValue(Ù,out ß)||ß<=0f){return 1f;}long ø=ö(Ù);ï=ø*ß;long õ=0;foreach(KeyValuePair<
IMyTerminalBlock,l>j in m){l p=j.Value;if(p==null||p.E!=Ä){continue;}if(p.u<0){continue;}õ+=p.u;}ð=õ*ß;}}return ó(ï,ð,ñ);}long ö(
MyItemType Ù){long þ=0;foreach(KeyValuePair<IMyTerminalBlock,l>j in m){l p=j.Value;if(p==null||p.s==null){continue;}þ+=ÿ(p.s,Ù);}
return þ;}float ù(){return b;}float ú(){int Ā=0;float þ=0f;foreach(KeyValuePair<MyItemType,float>j in Y){if(j.Key.TypeId!=
"MyObjectBuilder_AmmoMagazine"){continue;}if(j.Value<=0f){continue;}þ+=j.Value;Ā++;}return Ā>0?þ/Ā:0f;}IEnumerator<k>Ô(E Ä,int Æ){float ā=ý(Ä,Æ);int o
=0;foreach(KeyValuePair<IMyTerminalBlock,l>j in m){l p=j.Value;if(p==null||p.E!=Ä){continue;}if(!Ò(p.q)){continue;}
IMyInventory Ý=p.s;if(Ý==null){continue;}if(p.u>=0){Ă(p,Ä,Ý,p.u);}else if(Ä==E.B&&Æ>0){long ă=S((double)Ý.MaxVolume,Æ);long Ą=(long)
Math.Round(ă*(double)ā,MidpointRounding.AwayFromZero);if(Ą>0){Ă(p,Ä,Ý,Ą);}}else if(Æ>0){ą(p,Ä,Ý,Æ,ā);}o++;if(o%5==0){yield
return k.À;}if(Á()){yield return k.Â;}}}void Ă(l p,E Ä,IMyInventory Ý,long Ć){if(Ä==E.D){ć(p,Ý,Ć);return;}MyItemType Ù=(Ä==E.B
)?G:I;long Ĉ=ÿ(Ý,Ù);if(Ĉ<Ć){ĉ(p,Ý,Ù,Ć-Ĉ);}else if(Ĉ>Ć){Ċ(p,Ý,Ù,Ĉ-Ć);}}void ą(l p,E Ä,IMyInventory Ý,int M,float ā){float
Ú=N((float)Ý.MaxVolume,M)*ā;if(Ä==E.D){U ċ=p.w;if(ċ==null||ċ.Count==0){return;}if((float)Ý.CurrentVolume<Ú){Č(p,Ý,ċ,Ú);}
if((float)Ý.CurrentVolume>Ú){č(p,Ý,Ú);}return;}MyItemType Ù=(Ä==E.B)?G:I;if((float)Ý.CurrentVolume<Ú){Ď(p,Ý,Ù,Ú);}if((
float)Ý.CurrentVolume>Ú){ď(p,Ý,Ù,Ú);}}void Ď(l Đ,IMyInventory Ý,MyItemType Ù,float Ú){for(int đ=0;đ<a.Count;đ++){if((float)Ý.
CurrentVolume>=Ú){return;}if(Á()){return;}l Ē=a[đ];if(Ē==Đ){continue;}æ(Ý,Ē.s,Ù,Ú,false);}}void ď(l Đ,IMyInventory Ý,MyItemType Ù,
float Ú){Z ê=î(ē(Ù),Ù.SubtypeId);if(ê==null){return;}for(int Ĕ=0;Ĕ<ê.Count;Ĕ++){if((float)Ý.CurrentVolume<=Ú){return;}if(Á())
{return;}l ĕ=ê[Ĕ];if(ĕ==null||ĕ.q==Đ.q){continue;}IMyInventory Ė=ĕ.s;if(Ė==null){continue;}æ(Ý,Ė,Ù,Ú,true);}}void ć(l p,
IMyInventory Ý,long Ć){U ċ=p.w;if(ċ==null||ċ.Count==0){return;}long ė=0;for(int Ę=0;Ę<ċ.Count;Ę++){ė+=ÿ(Ý,ċ[Ę]);}if(ė<Ć){long ę=Ć-ė;
for(int Ę=0;Ę<ċ.Count&&ę>0;Ę++){MyItemType Ï=ċ[Ę];foreach(KeyValuePair<IMyTerminalBlock,l>j in m){if(ę<=0){break;}l Ē=j.
Value;if(Ē==null||Ē==p){continue;}if(Ē.E!=E.A&&!Ò(Ē.q)){continue;}IMyInventory Ě=Ē.s;if(Ě==null){continue;}long â=á(Ě,Ý,Ï,ę,
"balance");ę-=â;}}}else if(ė>Ć){long ě=ė-Ć;Z ê=î(ç.Ĝ,"ammo");if(ê==null){return;}ĝ.Clear();Ý.GetItems(ĝ);for(int g=0;g<ĝ.Count&&ě
>0;g++){MyItemType Ğ=ĝ[g].Type;long ğ=(long)ĝ[g].Amount;long Ġ=ğ<ě?ğ:ě;for(int Ĕ=0;Ĕ<ê.Count&&Ġ>0;Ĕ++){l ĕ=ê[Ĕ];if(ĕ==
null||ĕ.q==p.q){continue;}IMyInventory Ė=ĕ.s;if(Ė==null){continue;}long â=á(Ý,Ė,Ğ,Ġ,"balance-excess");Ġ-=â;ě-=â;}}}}void ĉ(l
Đ,IMyInventory Ý,MyItemType Ù,long ę){for(int đ=0;đ<a.Count;đ++){if(ę<=0){return;}l Ē=a[đ];if(Ē==Đ){continue;}long â=á(Ē.
s,Ý,Ù,ę,"balance");ę-=â;}}void Ċ(l Đ,IMyInventory Ý,MyItemType Ù,long ě){Z ê=î(ē(Ù),Ù.SubtypeId);if(ê==null){return;}for(
int Ĕ=0;Ĕ<ê.Count&&ě>0;Ĕ++){l ĕ=ê[Ĕ];if(ĕ==null||ĕ.q==Đ.q){continue;}IMyInventory Ė=ĕ.s;if(Ė==null){continue;}long â=á(Ý,Ė,
Ù,ě,"balance-excess");ě-=â;}}void Č(l Đ,IMyInventory Ý,U ċ,float Ú){for(int Ę=0;Ę<ċ.Count;Ę++){if((float)Ý.CurrentVolume
>=Ú){return;}Ď(Đ,Ý,ċ[Ę],Ú);}}void č(l Đ,IMyInventory Ý,float Ú){Z ê=î(ç.Ĝ,"ammo");if(ê==null){return;}U ċ=Đ.w;if(ċ==null||
ċ.Count==0){return;}for(int Ę=0;Ę<ċ.Count;Ę++){if((float)Ý.CurrentVolume<=Ú){return;}if(Á()){return;}MyItemType Ğ=ċ[Ę];
for(int Ĕ=0;Ĕ<ê.Count;Ĕ++){if((float)Ý.CurrentVolume<=Ú){return;}if(Á()){return;}l ĕ=ê[Ĕ];if(ĕ==null||ĕ.q==Đ.q){continue;}
IMyInventory Ė=ĕ.s;if(Ė==null){continue;}æ(Ý,Ė,Ğ,Ú,true);}}}public enum ç{ġ,Ģ,ģ,Ĥ,ĥ,Ħ,ħ,Ĝ,Ĩ,ĩ,Ī}private static readonly string[]ī={
"Ingots","Ores","Components","Prototech","Tools","Bottles","Weapons","Ammo","Consumables","Seeds","Misc"};internal static string
í(ç Ĭ){return ī[(int)Ĭ];}public class l{public IMyTerminalBlock q;public IMyInventory s;public int ĭ=100;public List<ç>Į=
new List<ç>();public bool į,n=true;public Dictionary<MyItemType,İ>ı;public E E=E.A;public U w;public long u=-1;public
string Ĳ,ĳ;}public class Z:List<l>{}private readonly Dictionary<ç,Z>ë=new Dictionary<ç,Z>();private readonly Z Ĵ=new Z();
private readonly Dictionary<IMyTerminalBlock,l>m=new Dictionary<IMyTerminalBlock,l>();bool ĵ=true;private readonly Dictionary<
string,ç>Ķ=new Dictionary<string,ç>();private readonly Dictionary<IMyTerminalBlock,int>ķ=new Dictionary<IMyTerminalBlock,int>(
);private readonly ĸ Ĺ=new ĸ(),ĺ=new ĸ();private readonly Ļ ĝ=new Ļ();private readonly ļ Ľ=new ļ(StringComparer.Ordinal);
private readonly StringBuilder ľ=new StringBuilder(1024);private readonly W Ŀ=new W(StringComparer.Ordinal);private readonly ŀ
Ł=new ŀ();internal static int Ň(string ł){if(string.IsNullOrEmpty(ł)){return 100;}int Ń=ł.IndexOf("[P:",StringComparison.
Ordinal);if(Ń<0){return 100;}int ń=ł.IndexOf(']',Ń+3);if(ń<0){return 100;}string Ņ=ł.Substring(Ń+3,ń-Ń-3);int ņ;if(int.TryParse
(Ņ,out ņ)){return ņ;}return 100;}internal static long v(string ł){if(string.IsNullOrEmpty(ł)){return-1;}int Ń=ł.IndexOf(
"[Balance=",StringComparison.Ordinal);if(Ń<0){return-1;}int ń=ł.IndexOf(']',Ń+9);if(ń<0){return-1;}string Ņ=ł.Substring(Ń+9,ń-Ń-9).
Trim();long ň;if(!long.TryParse(Ņ,out ň)){return-1;}if(ň<0){return-1;}return ň;}public static bool ŋ(string ŉ,string Ŋ){
return!string.Equals(ŉ,Ŋ,StringComparison.Ordinal);}IEnumerator<k>Ř(){bool Ō=false;int o=0;for(int ō=0;ō<Ŏ.Count;ō++){
IMyTerminalBlock r=Ŏ[ō];if(!ŏ(r)){if(m.Remove(r)){Ō=true;}continue;}string ł=r.CustomName;l p;if(!m.TryGetValue(r,out p)){p=new l();p.q=
r;p.s=r.GetInventory(0);m[r]=p;}else if(p.s==null){p.s=r.GetInventory(0);}if(ŋ(p.Ĳ,ł)){Ō=true;p.Ĳ=ł;p.n=true;bool Ő=Ò(r);
p.ĭ=Ň(ł);p.į=Ő&&x.y(ł,"[Stock]");p.Į.Clear();if(Ő){for(int ő=0;ő<ī.Length;ő++){if(x.y(ł,ī[ő])){p.Į.Add((ç)ő);}}}if(!p.į){
p.ı=null;p.ĳ=null;ķ.Remove(r);}}if(p.į){int Œ;if(!ķ.TryGetValue(r,out Œ)||Œ!=d){œ(r);ķ[r]=d;p.ĳ=null;}string Ŕ=r.
CustomData;if(p.ı==null||!string.Equals(p.ĳ,Ŕ,StringComparison.Ordinal)){Ō=true;p.ĳ=Ŕ;p.ı=new Dictionary<MyItemType,İ>();ŕ(r,p);Ŗ(
p);}}o++;if(o%25==0){yield return k.À;}if(Á()){yield return k.Â;}}if(!Ō&&!ĵ){yield break;}ĵ=false;foreach(KeyValuePair<ç,
Z>j in ë){j.Value.Clear();}Ĵ.Clear();for(int ō=0;ō<Ŏ.Count;ō++){l p;if(!m.TryGetValue(Ŏ[ō],out p)){continue;}for(int ő=0;
ő<p.Į.Count;ő++){ç è=p.Į[ő];Z ŗ;if(!ë.TryGetValue(è,out ŗ)){ŗ=new Z();ë[è]=ŗ;}ŗ.Add(p);}if(p.į){Ĵ.Add(p);}}foreach(
KeyValuePair<ç,Z>j in ë){j.Value.Sort((Ę,ō)=>Ę.ĭ.CompareTo(ō.ĭ));}Ĵ.Sort((Ę,ō)=>Ę.ĭ.CompareTo(ō.ĭ));}internal static bool Ś(string ř
){if(string.IsNullOrEmpty(ř)){return false;}return ř.IndexOf('/')>=0&&ř.IndexOf(':')>=0;}internal static bool Ŧ(string ř,
out string ś,out string Ŝ,out long ŝ,out Ş ş){ś=null;Ŝ=null;ŝ=0;ş=Ş.Š;if(string.IsNullOrEmpty(ř)){return false;}int š=ř.
IndexOf(':');if(š<=0||š>=ř.Length-1){return false;}string Ţ=ř.Substring(0,š).Trim();string ţ=ř.Substring(š+1).Trim();if(Ţ.
Length==0||ţ.Length==0){return false;}if(!Ť(Ţ,out ś,out Ŝ)){return false;}return ť(ţ,out ŝ,out ş);}internal static ŀ Ű(string
ł,Dictionary<MyItemType,İ>ŧ,Func<string,MyItemType?>Ũ){if(string.IsNullOrEmpty(ł)||ŧ==null||Ũ==null){return null;}ŀ ũ=
null;int Ū=0;while(Ū<ł.Length){int ū=ł.IndexOf('[',Ū);if(ū<0){break;}int Ŭ=ł.IndexOf(']',ū+1);if(Ŭ<0){break;}string ř=ł.
Substring(ū+1,Ŭ-ū-1);Ū=Ŭ+1;if(!Ś(ř)){continue;}string ś,Ŝ;long ŝ;Ş ş;if(!Ŧ(ř,out ś,out Ŝ,out ŝ,out ş)){if(ũ==null){ũ=new ŀ();}ũ.
Add(ř);continue;}MyItemType?ŭ=Ũ(ś+"/"+Ŝ);if(!ŭ.HasValue){if(ũ==null){ũ=new ŀ();}ũ.Add(ř);continue;}ŧ[ŭ.Value]=new İ{Ů=ŝ,ů=ş
};}return ũ;}void Ŗ(l p){if(p==null||p.q==null||p.ı==null){return;}ŀ ũ=Ű(p.q.CustomName,p.ı,ű);if(ũ==null){return;}for(
int g=0;g<ũ.Count;g++){string ř=ũ[g];ì("nametag:parse:"+p.q.CustomName+":"+ř,"Stock container '"+p.q.CustomName+
"' name tag '"+ř+"' did not parse as a quota override. Skipped.");}}void ŕ(IMyTerminalBlock r,l p){MyIniParseResult Ų;if(!ų.TryParse(r
.CustomData,out Ų)){Ŵ("Stock CustomData parse failed on '"+r.CustomName+"': "+Ų.ToString());return;}var ŵ=new List<
MyIniKey>();ų.GetKeys("Goose",ŵ);for(int g=0;g<ŵ.Count;g++){string Ŷ=ŵ[g].Name;string Ņ=ų.Get(ŵ[g]).ToString();if(Ņ.Equals("x",
StringComparison.OrdinalIgnoreCase)){continue;}MyItemType ŷ;İ Ÿ;if(Ź(Ŷ,Ņ,out ŷ,out Ÿ)){p.ı[ŷ]=Ÿ;}}}void œ(IMyTerminalBlock r){string Ĉ=r
.CustomData??string.Empty;Ľ.Clear();string[]ź=Ĉ.Split('\n');bool Ż=false;for(int g=0;g<ź.Length;g++){string ż=ź[g].
TrimEnd('\r').Trim();if(ż.Length==0){continue;}if(ż.StartsWith("[",StringComparison.Ordinal)&&ż.EndsWith("]",StringComparison.
Ordinal)){Ż=ż.Equals("[Goose]",StringComparison.Ordinal);continue;}if(!Ż){continue;}if(ż.StartsWith(";",StringComparison.
Ordinal)){continue;}int Ž=ż.IndexOf('=');if(Ž<=0){continue;}string Ŷ=ż.Substring(0,Ž).Trim();string ž=ż.Substring(Ž+1).Trim();
if(Ŷ.Length==0||ž.Length==0){continue;}if(!ſ(Ŷ)){continue;}Ľ[Ŷ]=Ŷ+"="+ž;}StringBuilder ƀ=ľ;ƀ.Length=0;ƀ.Append("[Goose]\n"
);ƀ.Append(";Stock quotas. Replace x with a number to manage an item; leave x to ignore it.\n");ƀ.Append(";Format: <Type>/<Subtype>=<value>[suffix]. M=min/pull-only, L=limit/push-only, none=exact, All=uncapped pull. E.g. Ingot/Iron=500M\n"
);ƀ.Append("\n; --- Manage Items Below ---\n");Ŀ.Clear();foreach(string Ɓ in i.Keys){Ŀ.Add(Ɓ);}foreach(string Ɓ in Ľ.Keys
){Ŀ.Add(Ɓ);}if(Ŀ.Count>0){Ł.Clear();foreach(string Ɓ in Ŀ){Ł.Add(Ɓ);}Ł.Sort(StringComparer.Ordinal);for(int g=0;g<Ł.Count
;g++){string Ƃ=Ł[g];string ƃ;if(Ľ.TryGetValue(Ƃ,out ƃ)){ƀ.Append(ƃ);ƀ.Append("\n");}else{ƀ.Append(Ƃ);ƀ.Append("=x\n");}}}
string Ƅ=ƀ.ToString();if(!string.Equals(Ĉ,Ƅ,StringComparison.Ordinal)){r.CustomData=Ƅ;}}bool ſ(string Ŷ){int ƅ=Ŷ.IndexOf('/');
if(ƅ<=0||ƅ>=Ŷ.Length-1){return false;}string Ɔ=Ŷ.Substring(0,ƅ);string Ƈ=Ŷ.Substring(ƅ+1);return ƈ(Ɔ)&&ƈ(Ƈ);}internal
static bool ƈ(string đ){if(string.IsNullOrEmpty(đ)){return false;}char ő=đ[0];if(!(char.IsLetter(ő)||ő=='_')){return false;}
for(int g=1;g<đ.Length;g++){ő=đ[g];if(!(char.IsLetterOrDigit(ő)||ő=='_')){return false;}}return true;}private static
readonly W Ɖ=new W{"PrototechCapacitor","PrototechCircuitry","PrototechCoolingUnit","PrototechFrame","PrototechMachinery",
"PrototechPanel","PrototechPropulsionUnit","PrototechScanner"};internal ç ē(MyItemType ŷ){string Ɗ=ŷ.TypeId;string Ƌ=ŷ.SubtypeId??"";
string ƌ=Ɗ+"/"+Ƌ;ç ƍ;if(Ķ.TryGetValue(ƌ,out ƍ)){return ƍ;}ç Ə=Ǝ(Ɗ,Ƌ);if(Ə==ç.Ī&&Ɗ!="MyObjectBuilder_Datapad"&&Ɗ!=
"MyObjectBuilder_PhysicalObject"){ì("unkType:"+Ɗ,"Unknown TypeId '"+Ɗ+"' classified as Misc");}return Ə;}internal static ç Ǝ(string Ɗ,string Ƌ){if(Ɗ==
"MyObjectBuilder_Ore"){return ç.Ģ;}if(Ɗ=="MyObjectBuilder_Ingot"){return ç.ġ;}if(Ɗ=="MyObjectBuilder_AmmoMagazine"){return ç.Ĝ;}if(Ɗ==
"MyObjectBuilder_Datapad"){return ç.Ī;}if(Ɗ=="MyObjectBuilder_SeedItem"){return ç.ĩ;}if(Ɗ=="MyObjectBuilder_Component"){if(Ɖ.Contains(Ƌ)){return
ç.Ĥ;}if(Ƌ.StartsWith("Prototech",StringComparison.Ordinal)){return ç.Ĥ;}return ç.ģ;}if(Ɗ==
"MyObjectBuilder_PhysicalGunObject"){if(Ƌ.IndexOf("Welder",StringComparison.OrdinalIgnoreCase)>=0||Ƌ.IndexOf("Grinder",StringComparison.OrdinalIgnoreCase)
>=0||Ƌ.IndexOf("Drill",StringComparison.OrdinalIgnoreCase)>=0){return ç.ĥ;}return ç.ħ;}if(Ɗ==
"MyObjectBuilder_OxygenContainerObject"||Ɗ=="MyObjectBuilder_GasContainerObject"){return ç.Ħ;}if(Ɗ=="MyObjectBuilder_ConsumableItem"){return ç.Ĩ;}if(Ɗ==
"MyObjectBuilder_PhysicalObject"&&(Ƌ=="Algae"||Ƌ=="Grain")){return ç.Ĩ;}return ç.Ī;}IEnumerator<k>Ɣ(){Ĺ.Clear();b=0f;int o=0;for(int ō=0;ō<Ŏ.Count;ō++){
IMyTerminalBlock r=Ŏ[ō];if(!ŏ(r)){continue;}for(int Ɛ=0;Ɛ<r.InventoryCount;Ɛ++){IMyInventory t=r.GetInventory(Ɛ);if(t==null){continue;}ĝ
.Clear();t.GetItems(ĝ);for(int g=0;g<ĝ.Count;g++){MyInventoryItem Ù=ĝ[g];long Ĉ;Ĺ.TryGetValue(Ù.Type,out Ĉ);Ĺ[Ù.Type]=Ĉ+(
long)Ù.Amount;if(Ƒ(Ù.Type)&&Ð!=null){Ð.ƒ(Ɠ(Ù.Type));}if(Ù.Type.TypeId=="MyObjectBuilder_AmmoMagazine"){float ß;if(Y.
TryGetValue(Ù.Type,out ß)&&ß>0f){b+=(float)Ù.Amount*ß;}}}}o++;if(o%10==0){yield return k.À;}if(Á()){yield return k.Â;}}}ƕ Ð;Ɩ Ɨ;
long Ƙ;private readonly StringBuilder ƙ=new StringBuilder();void Ƨ(){string Ɲ=Ç!=null&&!string.IsNullOrEmpty(Ç.ƚ)?Ç.ƚ:ƛ.Ɯ;
try{Ɨ=new Ɩ(IGC,Ɲ);Ð=new ƕ(Ɨ,ƞ.Ɵ,Ơ,ơ,Ƣ,ƣ);Ƥ();Ð.ƥ();}catch(System.Exception Ʀ){Ŵ("Bridge init failed: "+Ʀ.Message);Ð=null;Ɨ
=null;}}void Ƥ(){if(Ð==null){return;}Ð.ƨ=Ç.Ʃ;Ð.ƪ=Ç.ƫ;Ð.Ƭ=Ç.ƭ;}int ơ(){return i.Count;}IEnumerable<string>Ƣ(){return i.
Keys;}void ƣ(string Ʈ){ì("bridge","bridge: "+Ʈ);}void ƶ(StringBuilder ƀ){if(Ð==null||!Ð.ƨ){return;}Ư Ʊ=Ð.ư;if(Ʊ.Ʋ<0){return;
}ƀ.Append("Crane: ");if(Ʊ.Ƴ){long ƴ=Ƙ-Ʊ.Ʋ;if(ƴ<0){ƴ=0;}ƀ.Append("linked ").Append(ƴ).Append("t ago");if(Ð.Ƶ>0){ƀ.Append(
" (").Append(Ð.Ƶ).Append(" holds)");}}else{ƀ.Append("stale");}ƀ.Append('\n');}private readonly Ʒ i=new Ʒ();int d;internal
static string ƹ(string Ɗ){const string Ƹ="MyObjectBuilder_";if(!string.IsNullOrEmpty(Ɗ)&&Ɗ.StartsWith(Ƹ,StringComparison.
Ordinal)){return Ɗ.Substring(Ƹ.Length);}return Ɗ??string.Empty;}internal static string Ɠ(MyItemType ŷ){return ƹ(ŷ.TypeId)+"/"+(
ŷ.SubtypeId??string.Empty);}bool Ƒ(MyItemType ŷ){if(string.IsNullOrEmpty(ŷ.SubtypeId)){return false;}string Ŷ=Ɠ(ŷ);if(i.
ContainsKey(Ŷ)){return false;}i[Ŷ]=ŷ;d++;return true;}void Ơ(string Ŷ){if(string.IsNullOrEmpty(Ŷ)){return;}int ƅ=Ŷ.IndexOf('/');if(
ƅ<=0||ƅ>=Ŷ.Length-1){return;}if(i.ContainsKey(Ŷ)){return;}string ƺ="MyObjectBuilder_"+Ŷ;MyItemType ŷ;try{ŷ=MyItemType.
Parse(ƺ);}catch(Exception){return;}i[Ŷ]=ŷ;d++;}void ƽ(){string ƻ=Storage;if(string.IsNullOrEmpty(ƻ)){return;}string[]ź=ƻ.
Split('\n');for(int g=0;g<ź.Length;g++){string Ƽ=ź[g].Trim();if(Ƽ.Length==0){continue;}int ƅ=Ƽ.IndexOf('/');if(ƅ<=0||ƅ>=Ƽ.
Length-1){continue;}string ƺ="MyObjectBuilder_"+Ƽ;MyItemType ŷ;try{ŷ=MyItemType.Parse(ƺ);}catch(Exception){continue;}if(!i.
ContainsKey(Ƽ)){i[Ƽ]=ŷ;d++;}}}string ƿ(){if(i.Count==0){return string.Empty;}var ƀ=new StringBuilder();bool ƾ=true;foreach(
KeyValuePair<string,MyItemType>j in i){if(!ƾ){ƀ.Append('\n');}ƀ.Append(j.Key);ƾ=false;}return ƀ.ToString();}public enum Ş{Š,ǀ,ǁ,ǂ}
public class İ{public long Ů;public Ş ů;}public class Ǒ{public int ǃ=60,Ǆ=48,ǅ=32,È=0,É=0,Ê=0,ǆ=50,ƫ=6,ƭ=64,Ǉ=6;public float ǈ
=0.8f;public double ǉ=0.5;public bool Ë=false,Ǌ=true,ǋ=true,ǌ=false,Ʃ=true,Ǎ=true;public string ǎ="",ƚ=ƛ.Ɯ,ǐ=Ǐ.Ɯ;}private
readonly MyIni ų=new MyIni();private readonly Ǒ Ç=new Ǒ();bool ǒ=true;string Ǔ=null;private const string ǔ="Goose";private
static readonly object[][]Ǖ={new object[]{"reactorUraniumIngotsPer1000L",0,
"Uranium ingots per 1000L of reactor inventory (e.g. 10); 0 disables. Per-block: [Balance=N] or [NoBalance]."},new object[]{"gasIceFillPercent",0,"Percent (0-100) of each gas/irrigation block to fill with Ice; 0 disables."},new
object[]{"weaponAmmoFillPercent",0,"Percent (0-100) of each weapon to fill with ammo; 0 disables."},new object[]{
"enableRefineryManagement",true,"Master switch for Goose refinery feeding; auto-disabled when Crane is detected over the bridge. Uncomment ores in a refinery's [Goose] data to manage it."
},new object[]{"refineryInputFillPercent",50,
"Percent (0-100) of each managed refinery's input to keep filled with priority ore; 0 disables feeding."},new object[]{"blockGroup","","Optional group name; when set Goose manages only that group (ignores [Federate]/traversal). Empty = grid scope. Run 'rescan' after editing."
},new object[]{"enableSameRoleBalancing",false,"When true, evens each item across [P:NN] tiers in non-Stock category containers (higher tiers fill first). Default false."
},new object[]{"enableBridge",true,"Master kill-switch for the Goose-Crane bridge; false fully disables it."},new object[
]{"bridgeChannelTag",ƛ.Ɯ,"IGC tag for the bridge. Change if running multiple Goose/Crane pairs on one grid."},new object[
]{"bridgeHeartbeatTicks",6,"Heartbeat cadence in ticks (default 6 ~ 10s); also drives hello resend."},new object[]{
"bridgeMaxHoldsTracked",64,"Max concurrent assembler holds tracked; oldest evicted FIFO."},new object[]{"enableMultiGooseArbitration",true,"Multi-Goose coordination. true: two Geese on one grid both halt; federation needs [Federate] on both connectors. [Federate P:n], lower n = higher priority; a higher-priority docked Goose makes others stand down."
},new object[]{"federationChannelTag",Ǐ.Ɯ,
"IGC tag for the Goose-to-Goose presence beacon; coordinating Geese must share it."},new object[]{"federationHeartbeatTicks",6,"Presence-announce cadence in ticks (default 6 ~ 10s); min 6."},};bool ǘ(){
bool ǖ=false;foreach(object[]Ǘ in Ǖ){string Ŷ=(string)Ǘ[0];if(ų.ContainsKey(ǔ,Ŷ)){continue;}object ň=Ǘ[1];if(ň is bool){ų.
Set(ǔ,Ŷ,(bool)ň);}else if(ň is int){ų.Set(ǔ,Ŷ,(int)ň);}else{ų.Set(ǔ,Ŷ,(string)ň);}ų.SetComment(ǔ,Ŷ,(string)Ǘ[2]);ǖ=true;}
return ǖ;}IEnumerator<k>Ǧ(){if(Me.CustomData!=Ǔ){ǒ=true;}if(!ǒ){yield return k.À;yield break;}Ǔ=Me.CustomData;m.Clear();ĵ=true
;MyIniParseResult Ə;if(!ų.TryParse(Me.CustomData,out Ə)){Ŵ("CustomData parse failed: "+Ə.ToString());yield return k.À;
yield break;}Ç.ǃ=ų.Get(ǔ,"rescanIntervalTicks").ToInt32(60);Ç.ǈ=(float)ų.Get(ǔ,"budgetFraction").ToDouble(0.8);Ç.ǉ=ų.Get(ǔ,
"targetRunTimeMs").ToDouble(0.5);Ç.Ë=ų.Get(ǔ,"debugLogging").ToBoolean(false);Ç.Ǆ=ų.Get(ǔ,"maxActionLogEntries").ToInt32(48);Ç.ǅ=ų.Get(ǔ,
"maxWarningEntries").ToInt32(32);Ç.ǋ=ų.Get(ǔ,"enableConnectorFederation").ToBoolean(true);Ç.ǎ=(ų.Get(ǔ,"blockGroup").ToString("")??"").Trim
();Ç.ǌ=ų.Get(ǔ,"enableSameRoleBalancing").ToBoolean(false);Ç.Ʃ=ų.Get(ǔ,"enableBridge").ToBoolean(true);Ç.ƚ=ų.Get(ǔ,
"bridgeChannelTag").ToString(ƛ.Ɯ);int Ǚ=ų.Get(ǔ,"bridgeHeartbeatTicks").ToInt32(6);Ç.ƫ=Ǚ<6?6:Ǚ;int ǚ=ų.Get(ǔ,"bridgeMaxHoldsTracked").
ToInt32(64);Ç.ƭ=ǚ<1?1:ǚ;Ç.Ǎ=ų.Get(ǔ,"enableMultiGooseArbitration").ToBoolean(true);Ç.ǐ=ų.Get(ǔ,"federationChannelTag").ToString
(Ǐ.Ɯ);int Ǜ=ų.Get(ǔ,"federationHeartbeatTicks").ToInt32(6);Ç.Ǉ=Ǜ<6?6:Ǜ;int ǜ=ų.Get(ǔ,"reactorUraniumIngotsPer1000L").
ToInt32(0);int ǝ=ǜ<0?0:ǜ;if(ǜ!=ǝ){ì("balancer:bad-ratio:reactorUraniumIngotsPer1000L",
"reactorUraniumIngotsPer1000L must be >= 0; clamped to 0 (was "+ǜ+")");}Ç.È=ǝ;if(ų.ContainsKey(ǔ,"reactorUraniumFillPercent")){ì("balancer:deprecated:reactorUraniumFillPercent","reactorUraniumFillPercent is deprecated and ignored. Use reactorUraniumIngotsPer1000L instead (suggested value: 10). You can delete the old key from CustomData."
);}int Ǟ=ų.Get(ǔ,"gasIceFillPercent").ToInt32(0);int Ǡ=ǟ(Ǟ);if(Ǟ!=Ǡ){ì("balancer:bad-percent:gasIceFillPercent",
"gasIceFillPercent must be 0-100; clamped to "+Ǡ+" (was "+Ǟ+")");}Ç.É=Ǡ;int ǡ=ų.Get(ǔ,"weaponAmmoFillPercent").ToInt32(0);int Ǣ=ǟ(ǡ);if(ǡ!=Ǣ){ì(
"balancer:bad-percent:weaponAmmoFillPercent","weaponAmmoFillPercent must be 0-100; clamped to "+Ǣ+" (was "+ǡ+")");}Ç.Ê=Ǣ;Ç.Ǌ=ų.Get(ǔ,"enableRefineryManagement").
ToBoolean(true);int ǣ=ų.Get(ǔ,"refineryInputFillPercent").ToInt32(50);int Ǥ=ǟ(ǣ);if(ǣ!=Ǥ){ì(
"balancer:bad-percent:refineryInputFillPercent","refineryInputFillPercent must be 0-100; clamped to "+Ǥ+" (was "+ǣ+")");}Ç.ǆ=Ǥ;Ķ.Clear();var ŵ=new List<MyIniKey>();ų.
GetKeys(ǔ,ŵ);for(int g=0;g<ŵ.Count;g++){string ł=ŵ[g].Name;if(!ł.StartsWith("Override.")){continue;}string ƌ=ł.Substring(
"Override.".Length);string ž=ų.Get(ŵ[g]).ToString();ç è;if(Enum.TryParse(ž,true,out è)){Ķ[ƌ]=è;}else{Ŵ("Unknown category '"+ž+
"' for override "+ł);}}ǥ();Ƥ();ǒ=false;yield return k.À;}internal static bool Ť(string Ŷ,out string ś,out string Ŝ){ś=null;Ŝ=null;if(
string.IsNullOrEmpty(Ŷ)){return false;}int ƅ=Ŷ.IndexOf('/');if(ƅ<=0||ƅ>=Ŷ.Length-1){return false;}string Ɔ=Ŷ.Substring(0,ƅ);
string Ƈ=Ŷ.Substring(ƅ+1);ś=Ɔ.StartsWith("MyObjectBuilder_",StringComparison.Ordinal)?Ɔ:"MyObjectBuilder_"+Ɔ;Ŝ=Ƈ;return true;}
internal static MyItemType?ű(string ƺ){try{return MyItemType.Parse(ƺ);}catch{return null;}}internal static bool ť(string Ņ,out
long ŝ,out Ş ş){ŝ=0;ş=Ş.Š;if(string.IsNullOrEmpty(Ņ)){return false;}if(Ņ.Equals("All",StringComparison.OrdinalIgnoreCase)){ş
=Ş.ǂ;return true;}char ǧ=Ņ[Ņ.Length-1];string Ǩ=Ņ;if(ǧ=='M'||ǧ=='m'){ş=Ş.ǀ;Ǩ=Ņ.Substring(0,Ņ.Length-1);}else if(ǧ=='L'||ǧ
=='l'){ş=Ş.ǁ;Ǩ=Ņ.Substring(0,Ņ.Length-1);}return long.TryParse(Ǩ,out ŝ);}internal static int ǟ(int Ņ){if(Ņ<0){return 0;}if
(Ņ>100){return 100;}return Ņ;}bool Ź(string Ŷ,string Ņ,out MyItemType ŷ,out İ Ÿ){ŷ=default(MyItemType);Ÿ=null;if(string.
IsNullOrEmpty(Ŷ)||string.IsNullOrEmpty(Ņ)){return false;}string ś,Ŝ;if(!Ť(Ŷ,out ś,out Ŝ)){ì("stockq:legacy:"+Ŷ,"Stock quota key '"+Ŷ+
"' must be fully qualified as Type/Subtype (e.g. Component/SteelPlate). Skipped.");return false;}MyItemType?ŭ=ű(ś+"/"+Ŝ);if(!ŭ.HasValue){ì("stockq:parse:"+Ŷ,"Stock quota key '"+Ŷ+
"' did not resolve to a valid item type.");return false;}ŷ=ŭ.Value;long ŝ;Ş ş;if(!ť(Ņ,out ŝ,out ş)){return false;}Ÿ=new İ{Ů=ŝ,ů=ş};return true;}void ǥ(){if(ǘ()){
Me.CustomData=ų.ToString();Ǔ=Me.CustomData;}}bool ǩ;private readonly MyCommandLine Ǫ=new MyCommandLine();Dictionary<string
,Action<MyCommandLine>>ǫ;public
 Program
(){ƽ();Runtime.UpdateFrequency=UpdateFrequency.Update100;Ǭ();ǭ=Ǯ();Ƨ();Ì("Goose v1 initialized");}public void
 Save
(){Storage=ƿ();}public void
 Main
(string ǯ,UpdateType ǰ){try{if(!string.IsNullOrEmpty(ǯ)){Ǳ(ǯ);}Ƙ++;ǲ=ǳ(Runtime.LastRunTimeMs,Ç.ǉ,ǲ,Ç.ǈ);Ǵ(Ƙ);if(Ð!=null&&
!ǵ){Ð.Ƕ(Ƙ);}if((ǰ&UpdateType.Update100)!=0&&!ǩ){Ƿ();}}catch(Exception Ʀ){Ǹ("Main",Ʀ);}ǹ();}void Ǭ(){ǫ=new Dictionary<
string,Action<MyCommandLine>>(StringComparer.OrdinalIgnoreCase){{"rescan",ő=>{Ǻ=true;ǒ=true;Ì("cmd: rescan");}},{"pause",ő=>{ǩ
=true;Ì("cmd: pause");}},{"resume",ő=>{ǩ=false;Ì("cmd: resume");}},{"debug",ő=>{if(ő.ArgumentCount>1){bool ǻ=string.
Equals(ő.Argument(1),"on",StringComparison.OrdinalIgnoreCase);Ç.Ë=ǻ;Ì("cmd: debug "+(ǻ?"on":"off"));}}},{"reset-scope",ő=>{Ǽ.
Clear();ǽ.Clear();Ǿ=false;Ǻ=true;Ì("cmd: reset-scope");}}};}void Ǳ(string ǯ){if(!Ǫ.TryParse(ǯ)){Ŵ("Unparseable argument: "+ǯ)
;return;}if(Ǫ.ArgumentCount==0){return;}string ǿ=Ǫ.Argument(0);Action<MyCommandLine>Ȁ;if(ǫ.TryGetValue(ǿ,out Ȁ)){Ȁ(Ǫ);}
else{Ŵ("Unknown command: "+ǿ);}}private readonly ȁ Ŏ=new ȁ();private readonly Ȃ ȃ=new Ȃ();private readonly Ȅ ȅ=new Ȅ();
private readonly List<IMyProductionBlock>Ȇ=new List<IMyProductionBlock>();private readonly ȇ Ȉ=new ȇ(),ȉ=new ȇ();private
readonly List<IMyTextSurfaceProvider>Ȋ=new List<IMyTextSurfaceProvider>();int ȋ=int.MaxValue;bool Ǻ=false;bool ȏ(
IMyTerminalBlock r){return Ȍ.ȍ(r,Ǽ,Me)&&!Ȏ(r.CustomName);}bool ȓ(IMyTextSurfaceProvider Ȑ){var ȑ=Ȑ as IMyTerminalBlock;if(ȑ==null||ȑ.
Closed||ȑ==Me){return false;}if(!Ȓ(ȑ)||Ȏ(ȑ.CustomName)){return false;}return x.y(ȑ.CustomName,"[GError]")||x.y(ȑ.CustomName,
"[GStatus]");}bool ŏ(IMyTerminalBlock r){return r!=null&&!r.Closed&&Ȓ(r);}bool Ȏ(string ł){if(string.IsNullOrEmpty(ł)){return false
;}return ł.IndexOf("[Ignore]",StringComparison.Ordinal)>=0||ł.IndexOf("[Locked]",StringComparison.Ordinal)>=0;}
IEnumerator<k>Ȗ(){if(!Ǻ&&ȋ<Ç.ǃ){ȋ++;yield return k.À;yield break;}Ǻ=false;ȋ=0;ǒ=true;m.Clear();ĵ=true;Ŏ.Clear();GridTerminalSystem.
GetBlocksOfType(Ŏ,ȏ);yield return k.À;if(Á()){yield return k.Â;}ȃ.Clear();GridTerminalSystem.GetBlocksOfType(ȃ,ō=>!ō.Closed&&Ȓ(ō));
yield return k.À;ȅ.Clear();GridTerminalSystem.GetBlocksOfType(ȅ,ō=>!ō.Closed&&Ȓ(ō));yield return k.À;Ȇ.Clear();
GridTerminalSystem.GetBlocksOfType(Ȇ,ō=>!ō.Closed&&Ȓ(ō));yield return k.À;Ȕ.Clear();GridTerminalSystem.GetBlocksOfType(Ȕ,ō=>!ō.Closed&&Ȓ(ō
));yield return k.À;Ȉ.Clear();ȉ.Clear();Ȋ.Clear();GridTerminalSystem.GetBlocksOfType(Ȋ,ȓ);for(int g=0;g<Ȋ.Count;g++){
IMyTextSurfaceProvider Ȑ=Ȋ[g];if(Ȑ.SurfaceCount<=0){continue;}var ȑ=Ȑ as IMyTerminalBlock;if(x.y(ȑ.CustomName,"[GError]")){Ȉ.Add(Ȑ.GetSurface(
0));}if(x.y(ȑ.CustomName,"[GStatus]")){ȉ.Add(Ȑ.GetSurface(0));}}yield return k.À;ȕ="Rescan: "+Ŏ.Count+" inv, "+ȃ.Count+
" cargo, "+Ȇ.Count+" prod, "+Ȕ.Count+" refn, "+ȉ.Count+" GStatus, "+Ȉ.Count+" GError";}int ȗ;int Ș;string ș="init";private static
readonly string[]Ț={"RebuildScope","RescanIfDue","ParseConfigIfDirty","CategorizeContainers","CategorizeConsumers",
"ScanInventories","FulfillStockQuotas","SortGenericCargo","BalanceConsumers","BalanceSameRoleContainers","ManageRefineries",
"RenderStatus"};IEnumerator<k>ǭ;private const float ț=0.1f;float ǲ=0.8f;public static float ǳ(double Ȝ,double ȝ,float Ĉ,float Ȟ){if(ȝ
<=0.0){return Ȟ;}float ȟ=Ĉ;if(Ȝ>ȝ){ȟ=Ĉ*0.75f;}else if(Ȝ<ȝ*0.5){ȟ=Ĉ*1.1f;}if(ȟ>Ȟ){ȟ=Ȟ;}if(ȟ<ț){ȟ=ț;}return ȟ;}bool Á(){
return Runtime.CurrentInstructionCount>Runtime.MaxInstructionCount*ǲ;}IEnumerator<k>Ǯ(){while(true){Ƞ();for(int g=0;g<Ț.Length
;g++){ȗ=g;Ș=0;ș=Ț[g];IEnumerator<k>Ȣ=ȡ(g);while(Ȣ.MoveNext()){yield return Ȣ.Current;}yield return k.À;}}}IEnumerator<k>ȡ
(int g){if(ǵ&&g>=6&&g<=10){return ȣ();}switch(g){case 0:return Ȥ();case 1:return Ȗ();case 2:return Ǧ();case 3:return Ř();
case 4:return Ã();case 5:return Ɣ();case 6:return ȥ();case 7:return Ȧ();case 8:return Õ();case 9:return ȧ();case 10:return Ȩ
();case 11:return ȩ();default:return ȣ();}}IEnumerator<k>ȣ(){yield return k.À;}void Ƿ(){try{do{if(ǭ==null||!ǭ.MoveNext())
{ǭ=Ǯ();break;}}while(!Á());}catch(Exception Ʀ){Ǹ("step "+ȗ+"."+Ș+" "+ș,Ʀ);ǭ=Ǯ();}}enum Ȭ{A,Ȫ,ȫ}Ȭ ȭ=Ȭ.A;string Ȯ="";ȯ Ȱ;ȱ
Ȳ;bool ȳ;private readonly ȴ ȵ=new ȴ(),ȶ=new ȴ();private readonly ȷ ȸ=new ȷ();private readonly Ȅ ȹ=new Ȅ();private
readonly Ⱥ Ȼ=new Ⱥ();private readonly ȼ Ƚ=new ȼ();ulong Ⱦ;ulong ȿ;bool ɀ;bool ǵ{get{return ȭ!=Ȭ.A;}}public static bool Ʌ(long Ɂ,
int ɂ,bool Ƀ,bool Ʉ){if(!Ʉ||Ƀ||ɂ<=0){return true;}return Ɂ%ɂ==0;}void Ǵ(long Ɂ){if(!Ç.Ǎ){if(ȳ||ȭ!=Ȭ.A){ȳ=false;ȭ=Ȭ.A;ȵ.
Clear();Ǻ=true;}if(Ȱ!=null){Ȱ.ƨ=false;}return;}if(Ʌ(Ɂ,Ç.Ǉ,Ǻ,ɀ)){Ɇ.ɇ(GridTerminalSystem,ȸ,ȹ,Ȼ,Ƚ);Ɉ.ɉ(Me.CubeGrid.EntityId,Ȼ,(ȴ
)null,ȶ);Ⱦ=Ɉ.Ɋ(ȶ);ɀ=true;}if(Ȱ==null){ɋ();}if(Ȱ==null){ȳ=false;return;}ȳ=true;Ȱ.ƨ=true;Ȱ.ƪ=Ç.Ǉ;Ȱ.Ƕ(Ɂ);Ɍ Ɏ=Ȱ.ɍ(Ɂ);IList<ɏ>
ɐ=Ç.ǋ?(IList<ɏ>)Ƚ:null;ɑ Ə=ɒ.ɓ(Me.CubeGrid.EntityId,Me.EntityId,Ⱦ,ɐ,Ɏ);ȵ.Clear();foreach(long ɕ in Ə.ɔ){ȵ.Add(ɕ);}Ȭ ɗ=Ə.Ȫ
?Ȭ.Ȫ:(Ə.ɖ?Ȭ.ȫ:Ȭ.A);if(ɗ!=ȭ){ȭ=ɗ;Ȯ=ɘ(ɗ);Ì(ɗ==Ȭ.A?"Federation: resumed":"Federation: "+Ȯ);}ulong ɚ=ə(ɗ,ȵ);if(ɚ!=ȿ){ȿ=ɚ;Ǻ=
true;}}void ɋ(){string Ɲ=Ç!=null&&!string.IsNullOrEmpty(Ç.ǐ)?Ç.ǐ:Ǐ.Ɯ;try{Ȳ=new ɛ(IGC,Ɲ);Ȱ=new ȯ(Ȳ,Me.EntityId,()=>Ⱦ,()=>ȶ,ɜ)
;Ȱ.ƪ=Ç.Ǉ;Ȱ.ƥ();}catch(System.Exception Ʀ){Ŵ("Federation init failed: "+Ʀ.Message);Ȱ=null;Ȳ=null;}}void ɜ(string Ʈ){ì(
"federation","federation: "+Ʈ);}private static string ɘ(Ȭ Þ){switch(Þ){case Ȭ.Ȫ:return
"HALT: another Goose runs on this exact grid. Disable one of them.";case Ȭ.ȫ:return"STANDBY: deferring to a higher-priority docked Goose.";default:return"";}}private static ulong ə(Ȭ Þ,ȴ
ɝ){ulong ɞ=Ɉ.Ɋ(ɝ);ɞ^=(ulong)((int)Þ+1);ɞ*=1099511628211UL;return ɞ;}private readonly Queue<string>ɟ=new Queue<string>();
string ȕ="";private readonly ŀ ɠ=new ŀ(),ɡ=new ŀ();private readonly ɢ ɣ=new ɢ();private readonly W ɤ=new W();void Ì(string ɥ){
ɟ.Enqueue(ɥ);while(ɟ.Count>Ç.Ǆ){ɟ.Dequeue();}}void Ŵ(string ɥ){int Ā;if(ɣ.TryGetValue(ɥ,out Ā)){ɣ[ɥ]=Ā+1;}else{if(ɣ.Count
>=Ç.ǅ){string ɦ=ɠ[0];ɠ.RemoveAt(0);ɣ.Remove(ɦ);}ɣ[ɥ]=1;ɠ.Add(ɥ);}}void ì(string Ŷ,string ɥ){if(ɤ.Add(Ŷ)){ɡ.Add(ɥ);Ŵ(ɥ);}}
void ɨ(string Ŷ,string ɥ){if(ɧ.Add(Ŷ)){Ì(ɥ);}}private readonly W ɧ=new W();void Ƞ(){for(int g=0;g<ɡ.Count;g++){string ɥ=ɡ[g]
;if(ɣ.Remove(ɥ)){ɠ.Remove(ɥ);}}ɡ.Clear();ɤ.Clear();}void Ǹ(string ɩ,Exception Ʀ){Ŵ(ɩ+": "+Ʀ.GetType().Name+": "+Ʀ.Message
);}private readonly StringBuilder ɪ=new StringBuilder();void ǹ(){ɪ.Clear();if(ǵ){ɪ.Append("** ").Append(Ȯ).Append(" **\n"
);}ɪ.Append("Goose v1 ");ɪ.Append(ǩ?"PAUSED ":"");ɪ.Append("step ").Append(ȗ).Append('.').Append(Ș).Append(' ').Append(ș)
.Append('\n');ɪ.Append("Instr: ").Append(Runtime.CurrentInstructionCount).Append('/').Append(Runtime.MaxInstructionCount)
.Append('\n');ɪ.Append("LastRunMs: ").Append(Runtime.LastRunTimeMs.ToString("F2")).Append('\n');ƶ(ɪ);if(ȕ.Length>0){ɪ.
Append(ȕ).Append('\n');}if(ɟ.Count>0){ɪ.Append("Last:\n");foreach(string đ in ɟ){ɪ.Append("  ").Append(đ).Append('\n');}}if(ɣ.
Count>0){ɪ.Append("Warnings(").Append(ɣ.Count).Append("):\n");int ɫ=0;foreach(string Ŷ in ɠ){if(ɫ++>=5){break;}ɪ.Append("  ")
.Append(Ŷ).Append('\n');}}Echo(ɪ.ToString());}private readonly List<Sandbox.ModAPI.Ingame.IMyRefinery>Ȕ=new List<Sandbox.
ModAPI.Ingame.IMyRefinery>();private readonly List<string>ɬ=new List<string>();internal static readonly string[]ɭ={"Scrap",
"Stone","Platinum","Uranium","Gold","Silver","Magnesium","Cobalt","Silicon","Nickel","Iron"};private const string ɮ=
"MyObjectBuilder_Ore";private static bool ɯ(string h){for(int g=0;g<ɭ.Length;g++){if(string.Equals(ɭ[g],h,StringComparison.Ordinal)){return
true;}}return false;}internal static void ɱ(string Ŕ,List<string>Ə){Ə.Clear();if(string.IsNullOrEmpty(Ŕ)){return;}string[]ź=
Ŕ.Split('\n');bool ɰ=false;for(int g=0;g<ź.Length;g++){string ż=ź[g].TrimEnd('\r').Trim();if(ż.Length==0){continue;}if(ż.
StartsWith("[",StringComparison.Ordinal)&&ż.EndsWith("]",StringComparison.Ordinal)){ɰ=ż.Equals("[Goose]",StringComparison.Ordinal)
;continue;}if(!ɰ||ż.StartsWith(";",StringComparison.Ordinal)){continue;}if(!ɯ(ż)||Ə.Contains(ż)){continue;}Ə.Add(ż);}}
internal static bool ɲ(string Ŕ){if(string.IsNullOrEmpty(Ŕ)){return true;}return Ŕ.IndexOf("[Goose]",StringComparison.Ordinal)<0
;}internal static string ɴ(string ɳ){var ƀ=new StringBuilder();if(!string.IsNullOrEmpty(ɳ)){ƀ.Append(ɳ);if(!ɳ.EndsWith(
"\n",StringComparison.Ordinal)){ƀ.Append('\n');}ƀ.Append('\n');}ƀ.Append("[Goose]\n");ƀ.Append(
"; Refinery ore priority (top = highest). Uncomment ores to feed this refinery.\n");ƀ.Append("; Highest available ore fills first; when it is gone grid-wide, the next fills in.\n");ƀ.Append(
"; Commented-out ores are returned to storage. Leave all commented to ignore this refinery.\n");for(int g=0;g<ɭ.Length;g++){ƀ.Append("; ");ƀ.Append(ɭ[g]);ƀ.Append('\n');}return ƀ.ToString();}bool ɵ(){return Ð!=null
&&Ð.ƨ&&Ð.ư.Ƴ;}IEnumerator<k>Ȩ(){if(!Ç.Ǌ||ɵ()){yield break;}for(int ō=0;ō<Ȕ.Count;ō++){IMyRefinery Ĕ=Ȕ[ō];if(Ĕ==null||Ĕ.
Closed){continue;}string ɶ=Ĕ.CustomData??string.Empty;if(ɲ(ɶ)){ɶ=ɴ(ɶ);Ĕ.CustomData=ɶ;}ɱ(ɶ,ɬ);if(ɬ.Count==0){continue;}if(Ĕ.
UseConveyorSystem){Ĕ.UseConveyorSystem=false;}IMyInventory ɷ=Ĕ.InputInventory;if(ɷ==null){continue;}ɸ(ɷ,ɬ);if(Á()){yield return k.Â;}ɹ(ɷ,
ɬ);if(Á()){yield return k.Â;}yield return k.À;}}void ɹ(IMyInventory ɷ,List<string>ɺ){double ɻ=(double)ɷ.MaxVolume;double
Ć=ɻ*Ç.ǆ/100.0;if(Ć<=0.0){return;}for(int ɼ=0;ɼ<ɺ.Count;ɼ++){if((double)ɷ.CurrentVolume>=Ć){return;}var ŷ=MyItemType.
MakeOre(ɺ[ɼ]);ɽ(ɷ,ŷ,Ć);if(Á()){return;}}}void ɽ(IMyInventory ɷ,MyItemType ŷ,double Ć){for(int ō=0;ō<Ŏ.Count;ō++){if((double)ɷ.
CurrentVolume>=Ć){return;}IMyTerminalBlock r=Ŏ[ō];if(r==null||r.Closed||r is IMyRefinery){continue;}IMyInventory Ü=r.GetInventory(0);
if(Ü==null||Ü==ɷ){continue;}bool ɾ;try{ɾ=Ü.CanTransferItemTo(ɷ,ŷ);}catch{continue;}if(!ɾ){continue;}ĝ.Clear();try{Ü.
GetItems(ĝ);}catch{continue;}for(int g=ĝ.Count-1;g>=0;g--){MyInventoryItem Ù=ĝ[g];if(Ù.Type!=ŷ){continue;}MyFixedPoint ɿ=Ù.
Amount;bool ʀ;try{ʀ=ɷ.CanItemsBeAdded(ɿ,ŷ);}catch{ʀ=false;}if(!ʀ){break;}try{if(Ü.TransferItemTo(ɷ,g,null,true,ɿ)&&(double)ɷ.
CurrentVolume>=Ć){return;}}catch{}if(Á()){return;}}if(Á()){return;}}}void ɸ(IMyInventory ɷ,List<string>ɺ){Z ê;if(!ë.TryGetValue(ç.Ģ,
out ê)||ê.Count==0){ì("norefore","No container with the Ores tag to return de-prioritized refinery ore.");return;}ĝ.Clear()
;try{ɷ.GetItems(ĝ);}catch{return;}for(int g=ĝ.Count-1;g>=0;g--){MyInventoryItem Ù=ĝ[g];if(!string.Equals(Ù.Type.TypeId,ɮ,
StringComparison.Ordinal)){continue;}if(ɺ.Contains(Ù.Type.SubtypeId)){continue;}for(int ʁ=0;ʁ<ê.Count;ʁ++){l Ý=ê[ʁ];if(!ʂ(Ý)||Ý.s==ɷ){
continue;}if(!ɷ.CanTransferItemTo(Ý.s,Ù.Type)){continue;}if(!Ý.s.CanItemsBeAdded(Ù.Amount,Ù.Type)){continue;}if(ɷ.TransferItemTo
(Ý.s,g,null,true,Ù.Amount)){break;}}if(Á()){return;}}}private readonly Z ʃ=new Z(),ʄ=new Z(),ʅ=new Z();private readonly
Dictionary<MyItemType,long[]>ʆ=new Dictionary<MyItemType,long[]>();private const long ʇ=1000000000L;IEnumerator<k>ȧ(){if(!Ç.ǌ){
yield return k.À;yield break;}foreach(KeyValuePair<ç,Z>j in ë){Z ê=j.Value;if(ê==null||ê.Count<2){continue;}ʃ.Clear();for(int
Ĕ=0;Ĕ<ê.Count;Ĕ++){l ʈ=ê[Ĕ];if(ʈ==null){continue;}if(ʈ.į){continue;}if(ʈ.E!=E.A){continue;}if(!ŏ(ʈ.q)){continue;}if(ʈ.s==
null){continue;}ʃ.Add(ʈ);}if(!ʉ(true,ʃ.Count)){continue;}IEnumerator<k>Ó=ʊ(j.Key,ʃ);while(Ó.MoveNext()){yield return Ó.
Current;}yield return k.À;if(Á()){yield return k.Â;}}}IEnumerator<k>ʊ(ç Ĭ,Z ʋ){int ɫ=ʋ.Count;int g=0;while(g<ɫ){int ʌ=g;int ʍ=ʋ
[g].ĭ;while(g<ɫ&&ʋ[g].ĭ==ʍ){g++;}int ʎ=g;ʄ.Clear();for(int ʏ=ʌ;ʏ<ʎ;ʏ++){ʄ.Add(ʋ[ʏ]);}ʅ.Clear();for(int ʏ=ʎ;ʏ<ɫ;ʏ++){ʅ.Add
(ʋ[ʏ]);}if(ʅ.Count>0){IEnumerator<k>ʑ=ʐ(Ĭ,ʄ,ʅ);while(ʑ.MoveNext()){yield return ʑ.Current;}}if(ʄ.Count>=2){IEnumerator<k>
ʓ=ʒ(Ĭ,ʄ);while(ʓ.MoveNext()){yield return ʓ.Current;}}if(Á()){yield return k.Â;}}}IEnumerator<k>ʐ(ç Ĭ,Z ʔ,Z ʕ){ʆ.Clear();
for(int ʖ=0;ʖ<ʕ.Count;ʖ++){l ʗ=ʕ[ʖ];if(ʗ.s==null){continue;}ĝ.Clear();ʗ.s.GetItems(ĝ);for(int Ɓ=0;Ɓ<ĝ.Count;Ɓ++){MyItemType
ʏ=ĝ[Ɓ].Type;if(ē(ʏ)!=Ĭ){continue;}if(!ʆ.ContainsKey(ʏ)){ʆ[ʏ]=null;}}}foreach(KeyValuePair<MyItemType,long[]>j in ʆ){
MyItemType ŷ=j.Key;for(int ʘ=0;ʘ<ʔ.Count;ʘ++){l ʙ=ʔ[ʘ];if(ʙ.s==null){continue;}for(int ʖ=0;ʖ<ʕ.Count;ʖ++){l ʗ=ʕ[ʖ];if(ʗ.s==null){
continue;}long ʚ=ÿ(ʗ.s,ŷ);if(ʚ<=0){continue;}á(ʗ.s,ʙ.s,ŷ,ʚ,"rolebal-pullup");if(Á()){yield return k.Â;}}if(Á()){yield return k.Â
;}}}}IEnumerator<k>ʒ(ç Ĭ,Z ʛ){ʜ(Ĭ,ʛ,ʆ);foreach(KeyValuePair<MyItemType,long[]>j in ʆ){MyItemType ŷ=j.Key;long[]ʝ=j.Value;
int Ā=ʝ.Length;long[]ʞ=new long[Ā];long[]ʟ=new long[Ā];for(int g=0;g<Ā;g++){ʞ[g]=ʠ(ʛ[g].s,ŷ);}ʡ(ʝ,ʞ,ʟ);for(int đ=0;đ<Ā;đ++)
{long ʢ=ʝ[đ]-ʟ[đ];if(ʢ<=0){continue;}for(int ʣ=0;ʣ<Ā&&ʢ>0;ʣ++){if(ʣ==đ){continue;}long ʤ=ʟ[ʣ]-ʝ[ʣ];if(ʤ<=0){continue;}
long ʥ=ʢ<ʤ?ʢ:ʤ;long â=á(ʛ[đ].s,ʛ[ʣ].s,ŷ,ʥ,"rolebal-split");if(â<=0){break;}ʝ[đ]-=â;ʝ[ʣ]+=â;ʢ-=â;if(Á()){yield return k.Â;}}}
if(Á()){yield return k.Â;}}}void ʜ(ç Ĭ,Z ʛ,Dictionary<MyItemType,long[]>ʦ){ʦ.Clear();int ɫ=ʛ.Count;for(int g=0;g<ɫ;g++){
IMyInventory t=ʛ[g].s;if(t==null){continue;}ĝ.Clear();t.GetItems(ĝ);for(int Ɓ=0;Ɓ<ĝ.Count;Ɓ++){MyItemType ŷ=ĝ[Ɓ].Type;if(ē(ŷ)!=Ĭ){
continue;}long[]ʧ;if(!ʦ.TryGetValue(ŷ,out ʧ)){ʧ=new long[ɫ];ʦ[ŷ]=ʧ;}ʧ[g]+=(long)ĝ[Ɓ].Amount;}}}private static readonly double[]ʨ
={10000000.0,100000.0,1000.0,10.0,1.0};long ʠ(IMyInventory t,MyItemType ŷ){if(t==null){return 0;}if(t.CanItemsBeAdded((
MyFixedPoint)(double)ʇ,ŷ)){return long.MaxValue;}for(int g=0;g<ʨ.Length;g++){if(t.CanItemsBeAdded((MyFixedPoint)ʨ[g],ŷ)){return(long
)ʨ[g];}}return 0;}internal static void ʡ(long[]ʩ,long[]ʪ,long[]ʫ){int ɫ=ʩ.Length;long þ=0;for(int g=0;g<ɫ;g++){þ+=ʩ[g];}
long[]ʬ=new long[ɫ];for(int g=0;g<ɫ;g++){long ʭ=ʪ[g];ʬ[g]=(ʭ==long.MaxValue)?long.MaxValue:ʩ[g]+ʭ;ʫ[g]=0;}bool[]ʮ=new bool[ɫ
];long ʯ=þ;int ʰ=ɫ;while(ʰ>0&&ʯ>0){long ʱ=ʯ/ʰ;long ʲ=ʯ-ʱ*ʰ;bool ǖ=false;int ʳ=0;for(int g=0;g<ɫ;g++){if(ʮ[g]){continue;}
long ʥ=ʱ+(ʳ<ʲ?1:0);ʳ++;if(ʬ[g]<ʥ){ʫ[g]=ʬ[g];ʯ-=ʬ[g];ʮ[g]=true;ʰ--;ǖ=true;}}if(ǖ){continue;}ʳ=0;for(int g=0;g<ɫ;g++){if(ʮ[g])
{continue;}long ʥ=ʱ+(ʳ<ʲ?1:0);ʳ++;ʫ[g]=ʥ;ʯ-=ʥ;}break;}}internal static bool ʉ(bool ʴ,int ʵ){return ʴ&&ʵ>=2;}private
readonly ȴ Ǽ=new ȴ(),ǽ=new ȴ();private readonly ȷ ʶ=new ȷ();private readonly Ȅ ʷ=new Ȅ();private readonly Ⱥ ʸ=new Ⱥ();private
readonly ȼ ʹ=new ȼ();ulong ʺ;private readonly ȁ ʻ=new ȁ();bool Ǿ=false;bool Ȓ(IMyTerminalBlock r){return r!=null&&r.CubeGrid!=
null&&Ǽ.Contains(r.CubeGrid.EntityId);}bool Ò(IMyTerminalBlock r){return r!=null&&Ɉ.ʼ(Ǿ,ǽ,r.EntityId);}void ˀ(){Ɇ.ɇ(
GridTerminalSystem,ʶ,ʷ,ʸ,ʹ);if(ȳ){Ɉ.ɉ(Me.CubeGrid.EntityId,ʸ,ȵ,Ǽ);}else{Ɉ.ɉ(Me.CubeGrid.EntityId,ʸ,ʹ,Ç.ǋ,Ǽ);}ǽ.Clear();Ǿ=false;string ʽ=Ç.
ǎ;if(!string.IsNullOrEmpty(ʽ)){Ǿ=true;IMyBlockGroup ʾ=GridTerminalSystem.GetBlockGroupWithName(ʽ);if(ʾ==null){ì(
"scope:group-missing:"+ʽ,"Block group '"+ʽ+"' not found. Managing nothing until it exists. Check the blockGroup name in CustomData.");}else{ʻ.
Clear();ʾ.GetBlocks(ʻ);for(int g=0;g<ʻ.Count;g++){IMyTerminalBlock ō=ʻ[g];if(ō!=null&&!ō.Closed){ǽ.Add(ō.EntityId);}}}}ʺ=Ɉ.ʿ(
ʸ,ʹ);if(Ǿ){ɨ("scope:group:"+ʽ+":"+ǽ.Count,"Scope: group '"+ʽ+"', "+ǽ.Count+" block(s)");}else{ɨ("scope:size:"+Ǽ.Count,
"Scope: "+Ǽ.Count+" grid(s)");}}IEnumerator<k>Ȥ(){bool ˁ=Ǻ||Ǽ.Count==0||(Ǿ&&ǽ.Count==0)||ȋ>=Ç.ǃ;if(!ˁ&&Ǽ.Count>0){ʶ.Clear();
GridTerminalSystem.GetBlocksOfType(ʶ,ˆ=>!ˆ.Closed);ʷ.Clear();GridTerminalSystem.GetBlocksOfType(ʷ,ő=>!ő.Closed);ulong ˈ=Ɉ.ˇ(ʶ,ʷ);if(ˈ!=ʺ){
Ì("Scope drift detected");Ǻ=true;ˁ=true;}}if(!ˁ){yield return k.À;yield break;}ˀ();yield return k.À;}bool ˉ(
IMyTerminalBlock ō){return x.y(ō.CustomName,"[Stock]");}internal static IMyInventory ˌ(IMyTerminalBlock r){var ˊ=r as IMyAssembler;if(ˊ
!=null){return ˊ.Mode==MyAssemblerMode.Disassembly?ˊ.InputInventory:ˊ.OutputInventory;}var ˋ=r as IMyProductionBlock;if(ˋ
!=null){return ˋ.OutputInventory;}return r.GetInventory(0);}bool ˏ(IMyInventory Ü,IMyInventory Ý,MyItemType ŷ,long ˍ,
string ˎ){if(Ü==null||Ý==null){return false;}if(Ü==Ý){return false;}if(!Ü.CanTransferItemTo(Ý,ŷ)){return false;}long â=0;ĝ.
Clear();Ü.GetItems(ĝ);for(int g=ĝ.Count-1;g>=0&&â<ˍ;g--){MyInventoryItem Ù=ĝ[g];if(Ù.Type!=ŷ){continue;}long ʯ=ˍ-â;
MyFixedPoint ŝ=Ù.Amount;if((long)ŝ>ʯ){ŝ=(MyFixedPoint)(double)ʯ;}if(!Ý.CanItemsBeAdded(ŝ,ŷ)){break;}if(Ü.TransferItemTo(Ý,g,null,
true,ŝ)){â+=(long)ŝ;if(Ç.Ë){Ì(ˎ+" "+ŝ+"x"+ŷ.SubtypeId);}}}return â>0;}long ÿ(IMyInventory t,MyItemType ŷ){return(long)t.
GetItemAmount(ŷ);}void ː(IMyInventory t){ĺ.Clear();ĝ.Clear();t.GetItems(ĝ);for(int g=0;g<ĝ.Count;g++){MyInventoryItem Ù=ĝ[g];long Ĉ;ĺ
.TryGetValue(Ù.Type,out Ĉ);ĺ[Ù.Type]=Ĉ+(long)Ù.Amount;}}long á(IMyInventory Ü,IMyInventory Ý,MyItemType ŷ,long ˑ,string ˎ
){if(Ü==null||Ý==null||Ü==Ý||ˑ<=0){return 0;}long ˠ=ÿ(Ý,ŷ);ˏ(Ü,Ý,ŷ,ˑ,ˎ);long ˡ=ÿ(Ý,ŷ);return ˡ-ˠ;}bool ʂ(l ʈ){return ʈ!=
null&&ŏ(ʈ.q)&&ʈ.s!=null;}long ˣ(l Đ,Z ê,MyItemType ŷ,long ŝ,string ˎ,bool ʑ){long ʯ=ŝ;for(int g=0;g<ê.Count&&ʯ>0;g++){l Ø=ê[
g];if(Ø==Đ||Ø.į){continue;}if(!ʂ(Ø)){continue;}IMyInventory Ě=ʑ?Ø.s:Đ.s;IMyInventory ˢ=ʑ?Đ.s:Ø.s;if(!Ě.CanTransferItemTo(
ˢ,ŷ)){continue;}ʯ-=á(Ě,ˢ,ŷ,ʯ,ˎ);if(Á()){break;}}return ʯ;}IEnumerator<k>ȥ(){for(int đ=0;đ<Ĵ.Count;đ++){l Ý=Ĵ[đ];if(!ʂ(Ý))
{continue;}if(Ý.ı==null){continue;}ː(Ý.s);foreach(KeyValuePair<MyItemType,İ>ˤ in Ý.ı){MyItemType ŷ=ˤ.Key;İ ˬ=ˤ.Value;long
Ĉ;ĺ.TryGetValue(ŷ,out Ĉ);long ˮ=0;long ě=0;switch(ˬ.ů){case Ş.Š:if(Ĉ<ˬ.Ů){ˮ=ˬ.Ů-Ĉ;}else if(Ĉ>ˬ.Ů){ě=Ĉ-ˬ.Ů;}break;case Ş.ǀ
:if(Ĉ<ˬ.Ů){ˮ=ˬ.Ů-Ĉ;}break;case Ş.ǁ:if(Ĉ>ˬ.Ů){ě=Ĉ-ˬ.Ů;}break;case Ş.ǂ:ˮ=long.MaxValue;break;}long Ͱ=0;long ͱ=0;if(ˮ>0){Ͱ=Ͳ
(Ý,ŷ,ˮ);}if(ě>0){ͱ=ͳ(Ý,ŷ,ě);}if(ˬ.ů==Ş.Š||ˬ.ů==Ş.ǀ){long ˡ=Ĉ+Ͱ-ͱ;if(ˡ<ˬ.Ů){string ʹ=ŷ.TypeId!=null?ŷ.TypeId.Replace(
"MyObjectBuilder_",""):"";ì("quota:"+Ý.q.EntityId+":"+ʹ+"/"+ŷ.SubtypeId,Ý.q.CustomName+" short on "+ʹ+"/"+ŷ.SubtypeId+" ("+ˡ+"/"+ˬ.Ů+")");
}}if(Á()){yield return k.Â;}}yield return k.À;}}public static bool Ͷ(l p,ç è){if(p==null){return true;}if(p.į){return
false;}for(int ő=0;ő<p.Į.Count;ő++){if(p.Į[ő]==è){return false;}}return true;}long Ͳ(l Ý,MyItemType ŷ,long ˮ){long ʯ=ˮ;for(
int g=0;g<Ĵ.Count&&ʯ>0;g++){l Ü=Ĵ[g];if(Ü==Ý){continue;}if(!ʂ(Ü)){continue;}if(Ü.ı==null){continue;}İ ͷ;if(!Ü.ı.TryGetValue
(ŷ,out ͷ)){continue;}if(ͷ.ů!=Ş.ǁ&&ͷ.ů!=Ş.Š){continue;}long ͺ=ÿ(Ü.s,ŷ)-ͷ.Ů;if(ͺ<=0){continue;}if(!Ü.s.CanTransferItemTo(Ý.
s,ŷ)){continue;}ʯ-=á(Ü.s,Ý.s,ŷ,Math.Min(ͺ,ʯ),"stock<-stock");if(Á()){return ˮ-ʯ;}}ç è=ē(ŷ);Z ê;if(ʯ>0&&ë.TryGetValue(è,
out ê)){ʯ=ˣ(Ý,ê,ŷ,ʯ,"stock<-cat",true);if(Á()){return ˮ-ʯ;}}for(int ō=0;ō<Ŏ.Count&&ʯ>0;ō++){IMyTerminalBlock r=Ŏ[ō];if(r==Ý
.q){continue;}if(!ŏ(r)){continue;}l Ē;m.TryGetValue(r,out Ē);if(!Ͷ(Ē,è)){continue;}IMyInventory Ě=ˌ(r);if(Ě==null){
continue;}if(!Ě.CanTransferItemTo(Ý.s,ŷ)){continue;}ʯ-=á(Ě,Ý.s,ŷ,ʯ,"stock<-gen");if(Á()){return ˮ-ʯ;}}return ˮ-ʯ;}long ͳ(l Ü,
MyItemType ŷ,long ě){ç è=ē(ŷ);Z ê;if(!ë.TryGetValue(è,out ê)){ì("noroute:"+è,"Excess "+ŷ.SubtypeId+" has no "+í(è)+" route");
return 0;}long ʯ=ˣ(Ü,ê,ŷ,ě,"stock->cat",false);return ě-ʯ;}IEnumerator<k>Ȧ(){int o=0;for(int ō=0;ō<Ŏ.Count;ō++){
IMyTerminalBlock r=Ŏ[ō];if(!ŏ(r)){continue;}if(ˉ(r)){continue;}l Ē;m.TryGetValue(r,out Ē);if(Ē!=null&&Ē.E!=E.A){continue;}IMyInventory Ü
=ˌ(r);if(Ü==null){continue;}ĝ.Clear();Ü.GetItems(ĝ);for(int g=ĝ.Count-1;g>=0;g--){MyInventoryItem Ù=ĝ[g];ç è=ē(Ù.Type);Z
ê;if(!ë.TryGetValue(è,out ê)||ê.Count==0){ì("nocat:"+è,"No container tagged for category "+í(è));continue;}bool ͻ=false;
if(Ē!=null){for(int ő=0;ő<Ē.Į.Count;ő++){if(Ē.Į[ő]==è){ͻ=true;break;}}}if(ͻ){continue;}for(int Ĕ=0;Ĕ<ê.Count;Ĕ++){l Ý=ê[Ĕ]
;if(Ý.q==r){continue;}if(Ý.į){continue;}if(!ʂ(Ý)){continue;}if(!Ü.CanTransferItemTo(Ý.s,Ù.Type)){continue;}MyFixedPoint ŝ
=Ù.Amount;if(!Ý.s.CanItemsBeAdded(ŝ,Ù.Type)){continue;}if(Ü.TransferItemTo(Ý.s,g,null,true,ŝ)){if(Ç.Ë){Ì("sort "+ŝ+"x"+Ù.
Type.SubtypeId+" ->"+Ý.q.CustomName);}break;}}if(Á()){yield return k.Â;}}o++;if(o%10==0){yield return k.À;}}}private const
string ͼ="Debug";private const float ͽ=1.3f,Ά=0.4f,Έ=1.0f,Ή=0.34f,Ί=0.44f,Ό=0.55f;private static readonly Color Ύ=Color.White,
Ώ=Color.White,ΐ=new Color(40,40,40),Α=new Color(60,160,220),Β=Color.Orange;private readonly HashSet<ç>Γ=new HashSet<ç>();
IEnumerator<k>ȩ(){Δ();Ε();if(Ȉ.Count==0&&ȉ.Count==0){yield return k.À;yield break;}Ζ();Η();yield return k.À;}void Δ(){Γ.Clear();
foreach(KeyValuePair<MyItemType,long>j in Ĺ){if(j.Value>0){Γ.Add(ē(j.Key));}}}void Ε(){foreach(ç Ĭ in Γ){if(!Θ(Ĭ)){string ł=í(Ĭ
);ì("status:no-container:"+Ĭ,$"{ł} present but no container tagged. Tag a container with the {ł} tag.");}}}bool Θ(ç Ĭ){Z
Ι;return ë.TryGetValue(Ĭ,out Ι)&&Ι.Count>0;}void Ζ(){for(int g=0;g<Ȉ.Count;g++){IMyTextSurface Κ=Ȉ[g];if(Κ!=null){Λ(Κ);}}
}void Η(){for(int g=0;g<ȉ.Count;g++){IMyTextSurface Κ=ȉ[g];if(Κ!=null){Μ(Κ);}}}void Λ(IMyTextSurface Κ){var Ξ=new Ν(Κ);Ο
Σ=Π.Ρ(Κ);int Τ=ǵ?1:0;int Υ=1+Τ+Math.Max(1,ɣ.Count);float Ψ=Ξ.Φ("Ag",ͼ,1f).Χ;float ά=Ω.Ϊ(Σ.Ϋ.Y,Υ,Ψ,ͽ,Ά,Έ);float ή=Ω.έ(Ψ,ά,
ͽ);var ΰ=new ί(Σ,Ξ);ΰ.α("Goose Errors",Σ.Ϋ.X/2f,0f,β.γ,ͼ,ά,Ύ);int δ=1;if(ǵ){ΰ.α(Ȯ,0f,ε.ζ(0f,ή,δ),β.η,ͼ,ά,Β);δ++;}if(ɣ.
Count==0){ΰ.α("(no errors)",0f,ε.ζ(0f,ή,δ),β.η,ͼ,ά,Ώ);}else{foreach(KeyValuePair<string,int>j in ɣ){string Ƽ=j.Value>1?
$"{j.Key} (x{j.Value})":j.Key;ΰ.α(Ƽ,0f,ε.ζ(0f,ή,δ),β.η,ͼ,ά,Β);δ++;}}Π.θ(Κ,ΰ.ι);}void Μ(IMyTextSurface Κ){var Ξ=new Ν(Κ);Ο Σ=Π.Ρ(Κ);int Υ=1+κ();
float Ψ=Ξ.Φ("Ag",ͼ,1f).Χ;float ά=Ω.Ϊ(Σ.Ϋ.Y,Υ,Ψ,ͽ,Ά,Έ);float ή=Ω.έ(Ψ,ά,ͽ);float λ=Ψ*ά;float μ=Σ.Ϋ.X*Ή;float ν=Σ.Ϋ.X*Ί;float ξ=
λ*Ό;float ο=Σ.Ϋ.X;var ΰ=new ί(Σ,Ξ);ΰ.α("Goose Status",Σ.Ϋ.X/2f,0f,β.γ,ͼ,ά,Ύ);int δ=1;for(int ő=0;ő<=(int)ç.Ī;ő++){var Ĭ=(
ç)ő;bool π=Θ(Ĭ);if(!π&&!Γ.Contains(Ĭ)){continue;}float ρ=ε.ζ(0f,ή,δ);ΰ.α(ς(Ĭ),0f,ρ,β.η,ͼ,ά,Ώ);if(π){int M=σ(Ĭ);float τ=ρ+
(λ-ξ)/2f;ΰ.υ(μ,τ,ν,ξ,M,ΐ,Α);ΰ.α($"{M}%",ο,ρ,β.φ,ͼ,ά,Ώ);}else{ΰ.α("NO CONTAINER",μ,ρ,β.η,ͼ,ά,Β);}δ++;}Π.θ(Κ,ΰ.ι);}int κ(){
int Ā=0;for(int ő=0;ő<=(int)ç.Ī;ő++){var Ĭ=(ç)ő;if(Θ(Ĭ)||Γ.Contains(Ĭ)){Ā++;}}return Ā;}int σ(ç Ĭ){Z Ι;if(!ë.TryGetValue(Ĭ,
out Ι)){return 0;}double χ=0.0;double ψ=0.0;foreach(l p in Ι){if(p!=null&&p.s!=null){χ+=(double)p.s.CurrentVolume;ψ+=(
double)p.s.MaxVolume;}}return ω(χ,ψ);}internal static int ω(double χ,double ψ){if(ψ<=0.0){return 0;}int M=(int)Math.Round(χ/ψ*
100.0);if(M<0){return 0;}if(M>100){return 100;}return M;}internal static string ς(ç Ĭ){switch(Ĭ){case ç.ġ:return"Ingots";case
ç.Ģ:return"Ores";case ç.ģ:return"Comps";case ç.Ĥ:return"Proto";case ç.ĥ:return"Tools";case ç.Ħ:return"Bottles";case ç.ħ:
return"Weapons";case ç.Ĝ:return"Ammo";case ç.Ĩ:return"Consum";case ç.ĩ:return"Seeds";default:return"Misc";}}
}
public class ŀ:List<string>{public ŀ(){}}public class W:HashSet<string>{public W(){}public W(IEqualityComparer<string>ϊ)
:base(ϊ){}}public class ȴ:HashSet<long>{public ȴ(){}}public class ļ:Dictionary<string,string>{public ļ(){}public ļ(
IEqualityComparer<string>ϊ):base(ϊ){}}public class ɢ:Dictionary<string,int>{public ɢ(){}}public class Ʒ:Dictionary<string,MyItemType>{
public Ʒ(){}}public class ĸ:Dictionary<MyItemType,long>{}public class U:List<MyItemType>{}public class Ļ:List<MyInventoryItem>
{}public class ȁ:List<IMyTerminalBlock>{}public class Ȅ:List<IMyShipConnector>{}public class Ȃ:List<IMyCargoContainer>{
public Ȃ(){}}public class ȇ:List<IMyTextSurface>{}public class ȷ:List<IMyMechanicalConnectionBlock>{}public class ȼ:List<ɏ>{}
public class Ⱥ:List<ϋ>{}public class Ɍ:List<ό>{}public struct ɏ{public long ύ,ώ;public bool Ϗ,ϐ,ϑ;public int ϒ,ϓ;}public
struct ϋ{public long ϔ,ϕ;public bool ϖ,ϗ;}public class ό{public long Ϙ;public ulong ϙ;public ȴ Ϛ;}public class ɑ{public bool Ȫ
,ɖ;public ȴ ɔ=new ȴ();}public static class ɒ{public static ɑ ɓ(long ϛ,long Ϝ,ulong ϝ,IList<ɏ>Ϟ,IList<ό>Ɏ){var Ə=new ɑ();
if(Ɏ!=null){for(int g=0;g<Ɏ.Count;g++){ό ņ=Ɏ[g];if(ņ==null||ņ.Ϙ==Ϝ){continue;}if(ņ.ϙ==ϝ){Ə.Ȫ=true;break;}}}if(Ϟ!=null){for
(int g=0;g<Ϟ.Count;g++){ɏ ő=Ϟ[g];if(ő.ύ!=ϛ||!ő.Ϗ||ő.ώ==0){continue;}if(!ő.ϐ||!ő.ϑ){continue;}if(!ϟ(Ɏ,Ϝ,ő.ώ)){Ə.ɔ.Add(ő.ώ)
;continue;}if(ő.ϓ<ő.ϒ){Ə.ɖ=true;}else if(ő.ϓ>ő.ϒ){Ə.ɔ.Add(ő.ώ);}}}if(Ə.ɖ){Ə.ɔ.Clear();}return Ə;}private static bool ϟ(
IList<ό>Ɏ,long Ϝ,long Ϡ){if(Ɏ==null){return false;}for(int g=0;g<Ɏ.Count;g++){ό ņ=Ɏ[g];if(ņ==null||ņ.Ϙ==Ϝ||ņ.Ϛ==null){
continue;}if(ņ.Ϛ.Contains(Ϡ)){return true;}}return false;}}public static class Ɉ{public static void ɉ(long ϛ,IList<ϋ>ϡ,IList<ɏ>Ϟ
,bool Ϣ,ȴ ϣ){ϣ.Clear();ϣ.Add(ϛ);var Ϥ=new Queue<long>();Ϥ.Enqueue(ϛ);if(Ϣ&&Ϟ!=null){for(int g=0;g<Ϟ.Count;g++){ɏ ő=Ϟ[g];
if(ő.ύ!=ϛ||!ő.Ϗ||!ő.ϐ||ő.ώ==0){continue;}if(ϣ.Add(ő.ώ)){Ϥ.Enqueue(ő.ώ);}}}ϥ(Ϥ,ϡ,ϣ);}public static void ɉ(long ϛ,IList<ϋ>ϡ,
ȴ Ϧ,ȴ ϣ){ϣ.Clear();ϣ.Add(ϛ);var Ϥ=new Queue<long>();Ϥ.Enqueue(ϛ);if(Ϧ!=null){foreach(long ɕ in Ϧ){if(ɕ!=0&&ϣ.Add(ɕ)){Ϥ.
Enqueue(ɕ);}}}ϥ(Ϥ,ϡ,ϣ);}private static void ϥ(Queue<long>Ϥ,IList<ϋ>ϡ,ȴ ϣ){while(Ϥ.Count>0){long Ϡ=Ϥ.Dequeue();if(ϡ==null){
continue;}for(int g=0;g<ϡ.Count;g++){ϋ ʈ=ϡ[g];if(ʈ.ϔ!=Ϡ||!ʈ.ϖ||ʈ.ϗ||ʈ.ϕ==0){continue;}if(ϣ.Add(ʈ.ϕ)){Ϥ.Enqueue(ʈ.ϕ);}}}}public
static bool ʼ(bool ϧ,ȴ Ϩ,long ϩ){return!ϧ||(Ϩ!=null&&Ϩ.Contains(ϩ));}public static ulong Ɋ(IEnumerable<long>Ϫ){var ϫ=new List<
long>();if(Ϫ!=null){foreach(long Ϭ in Ϫ){ϫ.Add(Ϭ);}}ϫ.Sort();ulong ɞ=1469598103934665603UL;for(int g=0;g<ϫ.Count;g++){ɞ^=(
ulong)ϫ[g];ɞ*=1099511628211UL;}return ɞ;}public static ulong ʿ(IList<ϋ>ϭ,IList<ɏ>Ϯ){ulong ɞ=1469598103934665603UL;if(ϭ!=null)
{for(int g=0;g<ϭ.Count;g++){ϋ ʈ=ϭ[g];ɞ^=(ulong)ʈ.ϔ;ɞ^=((ulong)ʈ.ϕ)<<1;ɞ^=ʈ.ϖ?0x1UL:0x0UL;ɞ^=ʈ.ϗ?0x2UL:0x0UL;ɞ*=
1099511628211UL;}}if(Ϯ!=null){for(int g=0;g<Ϯ.Count;g++){ɏ ő=Ϯ[g];ɞ^=(ulong)ő.ύ;ɞ^=((ulong)ő.ώ)<<1;ɞ^=ő.Ϗ?0x4UL:0x0UL;ɞ^=ő.ϐ?0x8UL:
0x0UL;ɞ^=ő.ϑ?0x10UL:0x0UL;ɞ^=((ulong)(uint)ő.ϒ)<<16;ɞ^=((ulong)(uint)ő.ϓ)<<24;ɞ*=1099511628211UL;}}return ɞ;}public static
ulong ˇ(IList<IMyMechanicalConnectionBlock>ϭ,IList<IMyShipConnector>Ϯ){ulong ɞ=1469598103934665603UL;if(ϭ!=null){for(int g=0;
g<ϭ.Count;g++){IMyMechanicalConnectionBlock ˆ=ϭ[g];if(ˆ.Closed){continue;}long ϯ=ˆ.CubeGrid!=null?ˆ.CubeGrid.EntityId:0;
long ϰ=(ˆ.IsAttached&&ˆ.TopGrid!=null)?ˆ.TopGrid.EntityId:0;ɞ^=(ulong)ϯ;ɞ^=((ulong)ϰ)<<1;ɞ^=ˆ.IsAttached?0x1UL:0x0UL;ɞ^=x.y(
ˆ.CustomName,x.ϗ)?0x2UL:0x0UL;ɞ*=1099511628211UL;}}if(Ϯ!=null){for(int g=0;g<Ϯ.Count;g++){IMyShipConnector ő=Ϯ[g];if(ő.
Closed){continue;}long ϱ=ő.CubeGrid!=null?ő.CubeGrid.EntityId:0;IMyShipConnector Ø=ő.OtherConnector;long ϲ=(Ø!=null&&Ø.
CubeGrid!=null)?Ø.CubeGrid.EntityId:0;string ϳ=Ø!=null?Ø.CustomName:null;ɞ^=(ulong)ϱ;ɞ^=((ulong)ϲ)<<1;ɞ^=ő.Status==
MyShipConnectorStatus.Connected?0x4UL:0x0UL;int ϵ=x.ϴ(ő.CustomName);int Ϸ=x.ϴ(ϳ);ɞ^=ϵ>=0?0x8UL:0x0UL;ɞ^=Ϸ>=0?0x10UL:0x0UL;ɞ^=((ulong)(uint)ϵ)
<<16;ɞ^=((ulong)(uint)Ϸ)<<24;ɞ*=1099511628211UL;}}return ɞ;}}public static class Ɇ{public static void ɇ(
IMyGridTerminalSystem ϸ,ȷ Ϲ,Ȅ Ϻ,Ⱥ ϻ,ȼ ϼ){Ϲ.Clear();ϸ.GetBlocksOfType(Ϲ,ˆ=>!ˆ.Closed);ϻ.Clear();for(int g=0;g<Ϲ.Count;g++){
IMyMechanicalConnectionBlock ˆ=Ϲ[g];ϋ Ͻ;Ͻ.ϔ=ˆ.CubeGrid!=null?ˆ.CubeGrid.EntityId:0;Ͻ.ϕ=(ˆ.IsAttached&&ˆ.TopGrid!=null)?ˆ.TopGrid.EntityId:0;Ͻ.ϖ=ˆ.
IsAttached;Ͻ.ϗ=x.y(ˆ.CustomName,x.ϗ);ϻ.Add(Ͻ);}Ϻ.Clear();ϸ.GetBlocksOfType(Ϻ,ő=>!ő.Closed);ϼ.Clear();for(int g=0;g<Ϻ.Count;g++){
IMyShipConnector ő=Ϻ[g];ɏ Ͻ;Ͻ.ύ=ő.CubeGrid!=null?ő.CubeGrid.EntityId:0;IMyShipConnector Ø=ő.OtherConnector;Ͻ.ώ=(Ø!=null&&Ø.CubeGrid!=
null)?Ø.CubeGrid.EntityId:0;string ϳ=Ø!=null?Ø.CustomName:null;Ͻ.Ϗ=ő.Status==MyShipConnectorStatus.Connected;int ϵ=x.ϴ(ő.
CustomName);int Ϸ=x.ϴ(ϳ);Ͻ.ϐ=ϵ>=0;Ͻ.ϑ=Ϸ>=0;Ͻ.ϒ=ϵ;Ͻ.ϓ=Ϸ;ϼ.Add(Ͻ);}}}public static class x{public const string ϗ="[NoSubgrid]",Ͼ=
"[Federate",Ͽ="[Ignore]",Ѐ="[Manual]",Ё="[Locked]";public static bool y(string ł,string Ɲ){return!string.IsNullOrEmpty(ł)&&ł.
IndexOf(Ɲ,StringComparison.Ordinal)>=0;}public static bool Ȏ(string ł){if(string.IsNullOrEmpty(ł)){return false;}return ł.
IndexOf(Ͽ,StringComparison.Ordinal)>=0||ł.IndexOf(Ѐ,StringComparison.Ordinal)>=0||ł.IndexOf(Ё,StringComparison.Ordinal)>=0;}
public static int ϴ(string ł){if(string.IsNullOrEmpty(ł)){return-1;}int Ń=ł.IndexOf(Ͼ,StringComparison.Ordinal);if(Ń<0){return
-1;}int ˡ=Ń+Ͼ.Length;if(ˡ>=ł.Length){return-1;}char ȟ=ł[ˡ];if(ȟ==']'){return 0;}if(ȟ!=' '){return-1;}int ņ=ˡ+1;while(ņ<ł.
Length&&ł[ņ]==' '){ņ++;}if(ņ+1>=ł.Length||ł[ņ]!='P'||ł[ņ+1]!=':'){return-1;}ņ+=2;int Ђ=ņ;while(ņ<ł.Length&&ł[ņ]>='0'&&ł[ņ]<=
'9'){ņ++;}if(ņ==Ђ){return-1;}int Ѓ;if(!int.TryParse(ł.Substring(Ђ,ņ-Ђ),out Ѓ)){return-1;}return Ѓ;}}public static class Ȍ{
public static bool ȍ(IMyTerminalBlock r,ȴ Є,IMyTerminalBlock Ѕ){return r!=null&&!r.Closed&&r.CubeGrid!=null&&Є.Contains(r.
CubeGrid.EntityId)&&r.HasInventory&&r!=Ѕ&&!(r is IMyShipController);}}public static class ε{public static float Љ(float І,float
Ї,β Ј){if(Ј==β.φ){return І-Ї;}if(Ј==β.γ){return І-Ї/2f;}return І;}public static float ζ(float Њ,float ή,int Ћ){return Њ+ή
*Ћ;}}public class ί{private readonly List<Ќ>ǫ=new List<Ќ>();private readonly Ο Ѝ;private readonly Ў Џ;public ί(Ο А,Ў Ξ){Ѝ
=А;Џ=Ξ;}public IReadOnlyList<Ќ>ι=>ǫ;public void α(string Б,float І,float ρ,β Ј,string В,float ά,Color Г){float Е=Џ.Φ(Б,В,
ά).Д;var Ж=new Ќ();Ж.З=И.Й;Ж.Й=Б;Ж.К=Ѝ.Л.X+ε.Љ(І,Е,Ј);Ж.М=Ѝ.Л.Y+ρ;Ж.Н=В;Ж.О=ά;Ж.П=Ј;Ж.Р=Г;ǫ.Add(Ж);}public void Ф(float С
,float ρ,float Е,float Т,Color Г){var Ж=new Ќ();Ж.З=И.У;Ж.К=Ѝ.Л.X+С;Ж.М=Ѝ.Л.Y+ρ;Ж.Д=Е;Ж.Χ=Т;Ж.Р=Г;ǫ.Add(Ж);}public void υ
(float С,float ρ,float Е,float Т,int M,Color Х,Color Ц){Ф(С,ρ,Е,Т,Х);float Щ=Ч.Ш(Е,M);if(Щ>0f){Ф(С,ρ,Щ,Т,Ц);}}}public
struct Ο{public Vector2 Л,Ϋ;public static Ο Я(Vector2 Ъ,Vector2 Ы,float Ь){float Э=Ь/100f;var Ю=new Vector2(Ъ.X*Э,Ъ.Y*Э);var ň
=new Ο();ň.Л=(Ы-Ъ)/2f+Ю;ň.Ϋ=Ъ-Ю*2f;return ň;}}public enum β{η,γ,φ}public enum И{Й,У}public struct Ќ{public И З;public
float К,М,Д,Χ,О;public string Й,Н;public β П;public Color Р;}public interface Ў{а Φ(string Б,string В,float ά);}public static
class Ω{public static float Ϊ(float б,int Υ,float в,float г,float д,float е){float ж=Υ*в*г;if(ж<=0f){return е;}float Ņ=б/ж;if
(Ņ<д){return д;}if(Ņ>е){return е;}return Ņ;}public static float έ(float в,float ά,float г){return в*ά*г;}}public struct а
{public float Д,Χ;public а(float Е,float Т){Д=Е;Χ=Т;}}public static class Ч{public static float Ш(float з,int M){int ņ=M;
if(ņ<0){ņ=0;}if(ņ>100){ņ=100;}return з*(ņ/100f);}}public class Ν:Ў{private readonly IMyTextSurface и;private readonly
StringBuilder й=new StringBuilder();public Ν(IMyTextSurface Κ){и=Κ;}public а Φ(string Б,string В,float ά){й.Clear();й.Append(Б);var к
=и.MeasureStringInPixels(й,В,ά);return new а(к.X,к.Y);}}public static class Π{public static Ο Ρ(IMyTextSurface Κ){return
Ο.Я(Κ.SurfaceSize,Κ.TextureSize,Κ.TextPadding);}public static void θ(IMyTextSurface Κ,IReadOnlyList<Ќ>л){Κ.ContentType=
ContentType.SCRIPT;Κ.Script="";MySpriteDrawFrame м=Κ.DrawFrame();for(int g=0;g<л.Count;g++){Ќ ő=л[g];if(ő.З==И.Й){м.Add(new
MySprite(SpriteType.TEXT,ő.Й,new Vector2(ő.К,ő.М),null,ő.Р,ő.Н,TextAlignment.LEFT,ő.О));}else{var н=new Vector2(ő.К+ő.Д/2f,ő.М+ő
.Χ/2f);м.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",н,new Vector2(ő.Д,ő.Χ),ő.Р,null,TextAlignment.CENTER,0f));}}м
.Dispose();}}public class ƕ{private readonly ȱ о;private readonly ƞ п;private readonly Action<string>р,с;private readonly
Func<int>т;private readonly Func<IEnumerable<string>>у;private readonly ŀ ф=new ŀ();private readonly Dictionary<long,х>ц=new
Dictionary<long,х>();private readonly Queue<long>ч=new Queue<long>();private readonly List<long>ш=new List<long>();bool щ;bool ъ,ы
;long ь;long э,ю=-1,я;int ѐ;public int ƪ{get;set;}public int Ƭ{get;set;}public int ё{get;set;}public int ђ{get;set;}
public int ѓ{get;set;}public bool ƨ{get{return щ;}set{щ=value;}}public ƕ(ȱ є,ƞ ѕ,Action<string>і,Func<int>ї,Func<IEnumerable<
string>>ј,Action<string>љ){о=є;п=ѕ;р=і;т=ї;у=ј;с=љ;щ=true;ƪ=60;Ƭ=64;ё=360;ђ=3;ѓ=5;}public void ƥ(){if(!щ||ъ||о==null){return;}
ъ=true;э=ь+ƪ;њ(ћ.ќ(п));}public void Ƕ(long ѝ){if(!щ||!ъ||о==null){return;}ь=ѝ;ў();џ();Ѡ();ѡ();}public void ƒ(string Ŷ){if
(!щ||!ъ||о==null||string.IsNullOrEmpty(Ŷ)){return;}њ(ћ.Ѣ(Ŷ));}public bool Ñ(long ѣ){if(!щ){return false;}х Ѥ;return ц.
TryGetValue(ѣ,out Ѥ)&&Ѥ.ѥ>=ь;}public Ư ư{get{var đ=new Ư();đ.Ƴ=ы;đ.Ʋ=ю;đ.Ѧ=ѐ;return đ;}}public int Ƶ{get{return ц.Count;}}void ў(){
ф.Clear();try{о.ў(ф);}catch(Exception Ʀ){ѧ("DrainInbox: "+Ʀ.Message);return;}for(int g=0;g<ф.Count;g++){Ѩ(ф[g]);}}void Ѩ(
string ѩ){ћ ɥ;try{ɥ=ћ.Ѫ(ѩ);}catch(Exception Ʀ){ѧ("parse: "+Ʀ.Message);return;}if(ɥ==null){return;}int ѭ=ɥ.ѫ(ƛ.Ѭ,-1);string Ä=ɥ
.З;switch(Ä){case ƛ.Ѯ:if(ѭ!=ƛ.ѯ){ѧ("hello: version mismatch "+ѭ);return;}Ѱ(ɥ.ѫ(ƛ.ѱ,0));Ѳ();break;case ƛ.ѳ:if(ѭ!=ƛ.ѯ){ѧ(
"heartbeat: version mismatch "+ѭ);return;}int Ѵ=ɥ.ѫ(ƛ.ѱ,0);Ѱ(Ѵ);int Ѷ=ѵ();if(Ѷ-Ѵ>ѓ){Ѳ();}break;case ƛ.ѷ:Ѱ(ѐ);Ѹ(ɥ.ѹ(ƛ.Ѻ,null));break;case ƛ.ѻ:Ѱ(ѐ);
string ŵ=ɥ.ѹ(ƛ.Ѽ,string.Empty);if(!string.IsNullOrEmpty(ŵ)){string[]Ѿ=ŵ.Split(ƛ.ѽ);for(int g=0;g<Ѿ.Length;g++){Ѹ(Ѿ[g]);}}break
;case ƛ.ѿ:if(п!=ƞ.Ɵ){return;}Ѱ(ѐ);long Ϭ=ɥ.Ҁ(ƛ.ҁ,0);int ҋ=ɥ.ѫ(ƛ.Ҋ,0);if(Ϭ==0||ҋ<=0){return;}Ҍ(Ϭ,ҋ,ɥ.ѹ(ƛ.ҍ,string.Empty));
break;}}void Ѱ(int Ҏ){ю=ь;ѐ=Ҏ;ы=true;}void Ѡ(){if(ю<0){ы=false;return;}long ҏ=ю+(long)ƪ*ђ;ы=ь<=ҏ;}void ѡ(){if(ь<э){return;}э=
ь+ƪ;int Ā=ѵ();ћ ɥ=ы?ћ.Ґ(п,Ā):ћ.ќ(п);њ(ɥ);}void Ѳ(){if(ь<я){return;}IEnumerable<string>ŵ=ґ.Ғ<IEnumerable<string>>(у,null,
"getLocalCatalogKeys",ѧ);if(ŵ==null){return;}IEnumerable<ћ>ҕ=ћ.ғ(ŵ,ƛ.Ҕ);int Җ=0;foreach(ћ җ in ҕ){њ(җ);Җ++;}if(Җ>0){я=ь+ё;}}void Ҍ(long ѣ,int
Ҙ,string ҙ){х Ѥ;if(ц.TryGetValue(ѣ,out Ѥ)){Ѥ.ѥ=ь+Ҙ;Ѥ.Қ=ҙ;return;}if(ц.Count>=Ƭ&&ч.Count>0){long қ=ч.Dequeue();ц.Remove(қ)
;}Ѥ=new х();Ѥ.Ҝ=ѣ;Ѥ.ѥ=ь+Ҙ;Ѥ.Қ=ҙ;ц[ѣ]=Ѥ;ч.Enqueue(ѣ);}void џ(){if(ц.Count==0){return;}ш.Clear();foreach(KeyValuePair<long,
х>j in ц){if(j.Value.ѥ<ь){ш.Add(j.Key);}}for(int g=0;g<ш.Count;g++){ц.Remove(ш[g]);}}void Ѹ(string Ŷ){if(string.
IsNullOrEmpty(Ŷ)||р==null){return;}try{р(Ŷ);}catch(Exception Ʀ){ѧ("onPeerCatalogKey("+Ŷ+"): "+Ʀ.Message);}}int ѵ()=>ґ.Ғ(т,0,
"getLocalCatalogCount",ѧ);void њ(ћ ɥ){if(о==null){return;}try{о.ҝ(ɥ.Ҟ());}catch(Exception Ʀ){ѧ("send("+ɥ.З+"): "+Ʀ.Message);}}void ѧ(string Б)
{if(с==null){return;}try{с(Б);}catch(Exception){}}}public class Ɩ:ȱ{private readonly IMyIntergridCommunicationSystem ҟ;
private readonly string Ҡ;private readonly IMyBroadcastListener ҡ;public Ɩ(IMyIntergridCommunicationSystem Ң,string Ɲ){ҟ=Ң;Ҡ=Ɲ;
ҡ=Ң.RegisterBroadcastListener(Ɲ);}public void ҝ(string ѩ){ҟ.SendBroadcastMessage(Ҡ,ѩ,TransmissionDistance.
CurrentConstruct);}public void ў(ŀ ң){while(ҡ.HasPendingMessage){MyIGCMessage ɥ=ҡ.AcceptMessage();string Б=ɥ.Data as string;if(!string.
IsNullOrEmpty(Б)){ң.Add(Б);}}}}public class ћ{private readonly ļ Ҥ=new ļ();public ћ(string Ä){Ҥ[ƛ.ҥ]=Ä;}public string З{get{string Ɓ;
return Ҥ.TryGetValue(ƛ.ҥ,out Ɓ)?Ɓ:null;}}public ћ Ҧ(string Ŷ,string Ѓ){Ҥ[Ŷ]=Ѓ??string.Empty;return this;}public ћ Ҧ(string Ŷ,
int Ѓ){Ҥ[Ŷ]=Ѓ.ToString(System.Globalization.CultureInfo.InvariantCulture);return this;}public ћ Ҧ(string Ŷ,long Ѓ){Ҥ[Ŷ]=Ѓ.
ToString(System.Globalization.CultureInfo.InvariantCulture);return this;}public string ѹ(string Ŷ,string ҧ){string ň;return Ҥ.
TryGetValue(Ŷ,out ň)?ň:ҧ;}public int ѫ(string Ŷ,int ҧ){string ň;if(!Ҥ.TryGetValue(Ŷ,out ň)){return ҧ;}int Ҩ;return int.TryParse(ň,
System.Globalization.NumberStyles.Integer,System.Globalization.CultureInfo.InvariantCulture,out Ҩ)?Ҩ:ҧ;}public long Ҁ(string Ŷ
,long ҧ){string ň;if(!Ҥ.TryGetValue(Ŷ,out ň)){return ҧ;}long Ҩ;return long.TryParse(ň,System.Globalization.NumberStyles.
Integer,System.Globalization.CultureInfo.InvariantCulture,out Ҩ)?Ҩ:ҧ;}public string Ҟ(){var ƀ=new StringBuilder();ƀ.Append(ƛ.ҥ)
;ƀ.Append(ƛ.ҩ);ƀ.Append(З??string.Empty);foreach(KeyValuePair<string,string>j in Ҥ){if(j.Key==ƛ.ҥ){continue;}ƀ.Append(ƛ.Ҫ
);ƀ.Append(j.Key);ƀ.Append(ƛ.ҩ);ƀ.Append(j.Value);}return ƀ.ToString();}public static ћ Ѫ(string ѩ){if(string.
IsNullOrEmpty(ѩ)){return null;}var ɥ=new ћ(string.Empty);ɥ.Ҥ.Clear();string[]ҫ=ѩ.Split(ƛ.Ҫ);for(int g=0;g<ҫ.Length;g++){string ˤ=ҫ[g]
;if(ˤ.Length==0){continue;}int Ž=ˤ.IndexOf(ƛ.ҩ);if(Ž<=0){return null;}string Ŷ=ˤ.Substring(0,Ž);string Ѓ=ˤ.Substring(Ž+1)
;ɥ.Ҥ[Ŷ]=Ѓ;}return ɥ.Ҥ.ContainsKey(ƛ.ҥ)?ɥ:null;}public static ћ ќ(ƞ ѕ){return new ћ(ƛ.Ѯ).Ҧ(ƛ.Ҭ,ƛ.ҭ(ѕ)).Ҧ(ƛ.Ѭ,ƛ.ѯ);}public
static ћ Ґ(ƞ ѕ,int Ү){return new ћ(ƛ.ѳ).Ҧ(ƛ.Ҭ,ƛ.ҭ(ѕ)).Ҧ(ƛ.Ѭ,ƛ.ѯ).Ҧ(ƛ.ѱ,Ү);}public static ћ Ѣ(string Ŷ){return new ћ(ƛ.ѷ).Ҧ(ƛ.Ѻ
,Ŷ);}public static IEnumerable<ћ>ғ(IEnumerable<string>ŵ,int ү){var ҕ=new List<ћ>();if(ŵ==null){return ҕ;}string Ұ=ƛ.ҥ+"="
+ƛ.ѻ+ƛ.Ҫ+ƛ.Ѽ+"=";int ұ=Ұ.Length;var Ĉ=new StringBuilder();foreach(string Ŷ in ŵ){if(string.IsNullOrEmpty(Ŷ)){continue;}
int Ҳ=Ĉ.Length==0?Ŷ.Length:1+Ŷ.Length;if(Ĉ.Length>0&&ұ+Ĉ.Length+Ҳ>ү){ҕ.Add(new ћ(ƛ.ѻ).Ҧ(ƛ.Ѽ,Ĉ.ToString()));Ĉ.Length=0;Ҳ=Ŷ.
Length;}if(Ĉ.Length>0){Ĉ.Append(ƛ.ѽ);}Ĉ.Append(Ŷ);}if(Ĉ.Length>0){ҕ.Add(new ћ(ƛ.ѻ).Ҧ(ƛ.Ѽ,Ĉ.ToString()));}return ҕ;}}public
enum ƞ{Ɵ,ҳ,}public static class ƛ{public const string Ɯ="se.goose.bridge.v1",Ѯ="hello",ѳ="heartbeat",ѷ="catalogAdd",ѻ=
"catalogSnapshot",ѿ="assemblerHold",ҥ="kind",Ҭ="role",Ѭ="v",ѱ="cat",Ѻ="key",Ѽ="keys",ҁ="id",Ҋ="ttl",ҍ="need",Ҵ="goose",ҵ="crane";public
const int ѯ=1,Ҕ=2048;public const char ѽ='|',Ҫ=';',ҩ='=';public static string ҭ(ƞ ѕ){return ѕ==ƞ.Ɵ?Ҵ:ҵ;}}public interface ȱ{
void ҝ(string ѩ);void ў(ŀ ң);}public class Ư{public bool Ƴ;public long Ʋ;public int Ѧ;}public class х{public long Ҝ,ѥ;public
string Қ;}internal static class ґ{public static Ҷ Ғ<Ҷ>(Func<Ҷ>ҷ,Ҷ ҧ,string ɩ,Action<string>Ҹ){if(ҷ==null){return ҧ;}try{return
ҷ();}catch(Exception Ʀ){if(Ҹ!=null){Ҹ(ɩ+": "+Ʀ.Message);}return ҧ;}}}public static class Ǐ{public const string Ɯ=
"se.goose.fed.v1",ҹ="fedAnnounce",Һ="pb",һ="sig",Ҽ="grids";public const int ѯ=1;public const char ҽ='|';}public class ȯ{private readonly
ȱ о;private readonly long Ҿ;private readonly Func<ulong>ҿ;private readonly Func<IEnumerable<long>>Ӏ;private readonly
Action<string>с;private readonly ŀ ф=new ŀ();private readonly Dictionary<long,Ӂ>ӂ=new Dictionary<long,Ӂ>();private readonly
List<long>Ӄ=new List<long>();private readonly Ɍ ӄ=new Ɍ();bool щ;bool ъ;long ь,э;public int ƪ{get;set;}public int ђ{get;set;
}public bool ƨ{get{return щ;}set{щ=value;}}public ȯ(ȱ є,long Ϝ,Func<ulong>Ӆ,Func<IEnumerable<long>>ӆ,Action<string>љ){о=є
;Ҿ=Ϝ;ҿ=Ӆ;Ӏ=ӆ;с=љ;щ=true;ƪ=6;ђ=3;}public void ƥ(){if(!щ||ъ||о==null){return;}ъ=true;э=ь+ƪ;Ӈ();}public void Ƕ(long ѝ){if(!щ
||!ъ||о==null){return;}ь=ѝ;ў();ӈ();if(ь>=э){э=ь+ƪ;Ӈ();}}public Ɍ ɍ(long ѝ){ӄ.Clear();if(!щ){return ӄ;}long Ӊ=(long)ƪ*ђ;
foreach(KeyValuePair<long,Ӂ>j in ӂ){if(ѝ-j.Value.Ʋ<=Ӊ){ӄ.Add(j.Value.ӊ);}}return ӄ;}public static ћ ӎ(long Ӌ,ulong ӌ,
IEnumerable<long>Ӎ){var ƀ=new StringBuilder();if(Ӎ!=null){bool ƾ=true;foreach(long ɕ in Ӎ){if(!ƾ){ƀ.Append(Ǐ.ҽ);}ƀ.Append(ɕ.
ToString(System.Globalization.CultureInfo.InvariantCulture));ƾ=false;}}return new ћ(Ǐ.ҹ).Ҧ(ƛ.Ѭ,Ǐ.ѯ).Ҧ(Ǐ.Һ,Ӌ).Ҧ(Ǐ.һ,ӌ.ToString(
System.Globalization.CultureInfo.InvariantCulture)).Ҧ(Ǐ.Ҽ,ƀ.ToString());}void Ӈ(){ulong ӏ=ґ.Ғ<ulong>(ҿ,0,"getSignature",ѧ);
IEnumerable<long>Ӎ=ґ.Ғ<IEnumerable<long>>(Ӏ,null,"getConstructGrids",ѧ);њ(ӎ(Ҿ,ӏ,Ӎ));}void ў(){ф.Clear();try{о.ў(ф);}catch(Exception
Ʀ){ѧ("DrainInbox: "+Ʀ.Message);return;}for(int g=0;g<ф.Count;g++){Ѩ(ф[g]);}}void Ѩ(string ѩ){ћ ɥ;try{ɥ=ћ.Ѫ(ѩ);}catch(
Exception Ʀ){ѧ("parse: "+Ʀ.Message);return;}if(ɥ==null||ɥ.З!=Ǐ.ҹ){return;}if(ɥ.ѫ(ƛ.Ѭ,-1)!=Ǐ.ѯ){return;}long Ӌ=ɥ.Ҁ(Ǐ.Һ,0);if(Ӌ==0
||Ӌ==Ҿ){return;}ulong ӌ;if(!ulong.TryParse(ɥ.ѹ(Ǐ.һ,string.Empty),System.Globalization.NumberStyles.Integer,System.
Globalization.CultureInfo.InvariantCulture,out ӌ)){return;}Ӂ Ӑ;if(!ӂ.TryGetValue(Ӌ,out Ӑ)){Ӑ=new Ӂ{ӊ=new ό{Ϛ=new ȴ()}};ӂ[Ӌ]=Ӑ;}Ӑ.ӊ.Ϙ=
Ӌ;Ӑ.ӊ.ϙ=ӌ;Ӑ.ӊ.Ϛ.Clear();ӑ(ɥ.ѹ(Ǐ.Ҽ,string.Empty),Ӑ.ӊ.Ϛ);Ӑ.Ʋ=ь;}private static void ӑ(string Ņ,ȴ ϣ){if(string.IsNullOrEmpty
(Ņ)){return;}string[]Ѿ=Ņ.Split(Ǐ.ҽ);for(int g=0;g<Ѿ.Length;g++){long Ϭ;if(long.TryParse(Ѿ[g],System.Globalization.
NumberStyles.Integer,System.Globalization.CultureInfo.InvariantCulture,out Ϭ)&&Ϭ!=0){ϣ.Add(Ϭ);}}}void ӈ(){if(ӂ.Count==0){return;}
long Ӊ=(long)ƪ*ђ;Ӄ.Clear();foreach(KeyValuePair<long,Ӂ>j in ӂ){if(ь-j.Value.Ʋ>Ӊ){Ӄ.Add(j.Key);}}for(int g=0;g<Ӄ.Count;g++){ӂ
.Remove(Ӄ[g]);}}void њ(ћ ɥ){if(о==null){return;}try{о.ҝ(ɥ.Ҟ());}catch(Exception Ʀ){ѧ("send: "+Ʀ.Message);}}void ѧ(string
Б){if(с==null){return;}try{с(Б);}catch(Exception){}}class Ӂ{public ό ӊ;public long Ʋ;}}public class ɛ:ȱ{private readonly
IMyIntergridCommunicationSystem ҟ;private readonly string Ҡ;private readonly IMyBroadcastListener ҡ;public ɛ(IMyIntergridCommunicationSystem Ң,string Ɲ
){ҟ=Ң;Ҡ=Ɲ;ҡ=Ң.RegisterBroadcastListener(Ɲ);}public void ҝ(string ѩ){ҟ.SendBroadcastMessage(Ҡ,ѩ,TransmissionDistance.
ConnectedConstructs);}public void ў(ŀ ң){while(ҡ.HasPendingMessage){MyIGCMessage ɥ=ҡ.AcceptMessage();string Б=ɥ.Data as string;if(!string.
IsNullOrEmpty(Б)){ң.Add(Б);}}}}public enum k{Â,À,Ӓ