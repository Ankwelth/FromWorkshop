// |  Mother GUI - v0.1.0 - 12 May 2026
// |  Agentluke
// |
// |  Docs
// |  https://lukejamesmorrison.github.io/mother-docs/
// |
// |  Discord
// |  https://discord.com/invite/PrrmBujmXQ
// 
A B;public
 Program(){B=new A(this){C="Mother GUI",};B.D(new List<E>{new F(B),new G(B),new H(B),new I(B)
,new J(B),new K(B),new L(B),new M(B),});}public void
 Save
(){Storage=B.N();}public void
 Main
(string O,UpdateType P){B.Q(O,P);}
}
public class I:R{public I(A B):base(B){}public override void V(){A.S<G>().T(new U());}}public class U:W{public string X
=>"CircleView";const int Y=6;public void z(Z a){RectangleF c=a.b;a.d("Circle View",new Vector2(c.X+e.f.g,c.Y+e.f.h));float
i=c.Height*0.12f;float j=c.Height*0.18f;float k=c.Height-i-j;float l=c.Y+i+k/2f;Vector2 m=new Vector2(c.X+c.Width/2f,l);
float n=Math.Min(c.Width,k)*0.4f;Color[]t=new Color[]{e.o.p,e.o.q,e.o.r,e.o.s,e.o.p,e.o.q};for(int u=Y;u>=1;u--){float v=n*2f
*((float)u/Y);Color w=t[(Y-u)%t.Length];Color x=new Color(w.R,w.G,w.B,(byte)(60+u*30));a.y(m,v,x);}}}public class À:ª{
F µ;public override string X=>"screen/scripts";public À(F º){µ=º;}public override string Ò(Á Â){if(Â.Ã.Count<1)return Ä
.Å.Æ;string Ç=Â.Ã[0];var É=µ.È(Ç);if(!É.Any())return Ê.Ë(Ì.Í,Ç);var Î=new List<string>();É[0].GetScripts(Î);string Ï=
"SCRIPTS\n"+string.Join("\n",Î.ToArray());É.ForEach(Ð=>{Ð.ContentType=ContentType.TEXT_AND_IMAGE;Ð.WriteText(Ï,false);});return Ê.Ë
(Ì.Ñ,Ç,"scripts listed");}}public class Ó:ª{F µ;public override string X=>"screen/content";public Ó(F º){µ=º;}
public override string Ò(Á Â){if(Â.Ã.Count<2)return"Not enough args";string Ç=Â.Ã[0];string Ô=Â.Ã[1];ContentType Õ;if(!µ.Ö(Ô,
out Õ))return"Invalid content type. Use one of: none, script, text.";var É=µ.È(Ç);if(!É.Any())return Ê.Ë(Ì.Í,Ç);É.ForEach(Ð
=>µ.Ø(Ð,Ô));return Ê.Ë(Ì.Ñ,Ç,$"contentType={Ô}");}}public class Ù:ª{F µ;public override string X=>"screen/script"
;public Ù(F º){µ=º;}public override string Ò(Á Â){if(Â.Ã.Count<2)return Ä.Å.Æ;string Ç=Â.Ã[0];string Ú=Â.Ã[1];var É=µ.È(Ç
);if(!É.Any())return Ê.Ë(Ì.Í,Ç);É.ForEach(Ð=>µ.Û(Ð,Ú));return Ê.Ë(Ì.Ñ,Ç,$"script={Ú}");}}public class F:R{Ü Ü;public F(A
B):base(B){}public override void V(){Ü=A.S<Ü>();Ý(new Ó(this));Ý(new Ù(this));Ý(new À(this));}public List<IMyTextSurface>
È(string Þ)=>Ü.È(Þ);public bool Ø(IMyTextSurface Ð,string Ô){ContentType ß;if(!Ö(Ô,out ß))return false;Ð.ContentType=ß;
return true;}public bool Û(IMyTextSurface Ð,string Ú){Ð.ContentType=ContentType.SCRIPT;Ð.Script=Ú;return true;}public bool Ö(
string Ô,out ContentType ß){ß=ContentType.NONE;switch(Ô){case"none":ß=ContentType.NONE;return true;case"script":ß=ContentType.
SCRIPT;return true;case"text":ß=ContentType.TEXT_AND_IMAGE;return true;}return false;}}public class M:R{public M(A B):base(B){
}public override void V(){à á=A.S<à>();A.S<G>().T(new â(á));}}public static class č{public struct è{public float ã;public
string ä;public string å;public Color?æ;public float?ç;}const float é=1.5f;public static void Č(Z a,Vector2 ê,float ë,float ì,
è í){float î=e.f.g;float ï=Math.Max(0f,Math.Min(1f,í.ã));float ð=52f;float ñ=Math.Max(ì-ð,40f);float ò=ë-î*2f;Vector2 ó=
new Vector2(ê.X,ê.Y-ð/2f);float ô=Math.Min(ò*0.52f,ñ/é);float õ=ô*é;if(õ>ñ){õ=ñ;ô=õ/é;}float ö=Math.Max(4f,ô*0.09f);float ø
=ô-ö*2f;float ù=õ-ö*2f;ú.û(a,ó.X,ó.Y,ô+4f,õ+4f,ú.ü(e.o.ý,180));ú.û(a,ó.X,ó.Y,ô,õ,ú.ü(e.o.þ,220));ú.û(a,ó.X,ó.Y,ô-2f,õ-2f,
ú.ü(e.o.ý,220));ú.û(a,ó.X,ó.Y,ø,ù,ú.ü(e.o.ÿ,255));float Ā=ø/2f;float ā=Ā*(1f-ï);if(ā>1f){float Ă=(ó.X-Ā)+ā/2f;float ă=(ó.
X+Ā)-ā/2f;ú.û(a,Ă,ó.Y,ā,ù,ú.ü(e.o.þ,200));ú.û(a,ă,ó.Y,ā,ù,ú.ü(e.o.þ,200));if(ā>6f){ú.û(a,Ă,ó.Y,ā-4f,ù-4f,ú.ü(e.o.ý,240));
ú.û(a,ă,ó.Y,ā-4f,ù-4f,ú.ü(e.o.ý,240));}}float Ą=ø*ï;if(Ą>2f){float ą=ó.Y+ù/2f-3f;ú.û(a,ó.X,ą,Ą,3f,ú.ü(e.o.p,200));}float
Ć=Math.Max(0.55f,Math.Min(1.2f,ì/300f))*(í.ç??1f);float ć=ó.Y+õ/2f+8f;float Ĉ=ó.X-ô/2f;float ĉ=ó.X+ô/2f;if(í.ä!=null)a.Ċ.
Add(ú.ċ(í.ä,new Vector2(Ĉ,ć),Ć,e.o.r,TextAlignment.LEFT));if(í.å!=null)a.Ċ.Add(ú.ċ(í.å,new Vector2(ĉ,ć),Ć*0.85f,í.æ??e.o.s,
TextAlignment.RIGHT));}}public class â:Ď<IMyDoor>{public override string X=>"DoorView";protected override string ď=>"No door found.";
public â(à á):base(á){}protected override List<IMyDoor>Đ()=>à.Đ<IMyDoor>();protected override void Ę(Z a,IMyDoor đ,Ē ē){string
Ĕ;Color ĕ;Ė(đ.Status,out Ĕ,out ĕ);ė(a,ē,đ.OpenRatio,Ĕ,ĕ);}static void Ė(DoorStatus ę,out string Ě,out Color w){switch(ę){
case DoorStatus.Opening:Ě="OPENING";w=e.o.q;return;case DoorStatus.Open:Ě="OPEN";w=e.o.ě;return;case DoorStatus.Closing:Ě=
"CLOSING";w=e.o.q;return;case DoorStatus.Closed:Ě="CLOSED";w=e.o.Ĝ;return;default:Ě="UNKNOWN";w=e.o.s;return;}}}public class K:R{
public K(A B):base(B){}public override void V(){à á=A.S<à>();A.S<G>().T(new ĝ(á));}}public class ĝ:Ď<IMyMotorAdvancedStator>{
public override string X=>"HingeView";protected override string ď=>"No hinge found.";const float Ğ=-MathHelper.PiOver2;const
float ğ=MathHelper.PiOver2;public ĝ(à á):base(á){}protected override List<IMyMotorAdvancedStator>Đ()=>à.Đ<
IMyMotorAdvancedStator>();protected override void Ę(Z a,IMyMotorAdvancedStator Ġ,Ē ē){Color ģ=ġ.Ģ(Ġ);Ĥ(a,ē,Ġ.Angle,Ġ.LowerLimitRad,Ġ.
UpperLimitRad,Ġ.TargetVelocityRPM,Ġ.Torque,Ġ.BrakingTorque,Ġ.RotorLock,"TENSORS",ģ,ĥ:Ğ,Ħ:ğ);}}public class H:R,ħ,Ĩ{Dictionary<string,ĩ>Ī=new Dictionary<string,ĩ>();ī Ĭ;Ä ĭ;G Į;į İ;string ı;const string Ĳ="general";const string ĳ="defaultMenu";
const string Ĵ="menu";const string ĵ="menu:";public H(A B):base(B){}public override void V(){ĭ=A.S<Ä>();İ=A.S<į>();Ĭ=new ī(A.
C,A.X,A.Ķ,İ);ķ();ĸ();Į=A.S<G>();Ĺ ĺ=new Ĺ(Ĭ);Į.T(ĺ);Į.Ļ(ĺ);Į.ļ(this);Į.Ľ(this);Ŀ<ľ>();}public override void ń(ŀ Ł,object
ł){if(Ł is ľ){Ī.Clear();ķ();ĸ();Į.Ń();}}public ĩ Ŋ(IMyTerminalBlock Ņ,string ņ,out string Ň){Ň=ņ;if(!string.IsNullOrEmpty
(ņ)&&Ī.ContainsKey(ņ))return Ī[ņ];ĩ ŉ=ň(Ņ);if(ŉ!=null){Ň="";return ŉ;}if(!string.IsNullOrEmpty(ı)&&Ī.ContainsKey(ı)){Ň=ı;
return Ī[ı];}return null;}public bool ŋ(string ņ){return!string.IsNullOrEmpty(ņ)&&Ī.ContainsKey(ņ);}void ķ(){MyIni Ō=new MyIni
();if(!Ō.TryParse(A.ō.CustomData))return;List<string>Ŏ=new List<string>();Ō.GetSections(Ŏ);foreach(string ŏ in Ŏ){if(ŏ.
StartsWith(ĵ)){string ņ=ŏ.Substring(ĵ.Length);ĩ Ő=new ĩ(ņ);List<MyIniKey>ő=new List<MyIniKey>();Ō.GetKeys(ŏ,ő);List<string>Œ=new
List<string>();foreach(var œ in ő){string Ŕ=Ō.Get(ŏ,œ.Name).ToString();Œ.Add(œ.Name+"="+Ŕ);}ŕ(Œ,Ő);Ī[ņ]=Ő;}}}void ĸ(){MyIni
Ō=new MyIni();if(Ō.TryParse(A.ō.CustomData))ı=Ō.Get(Ĳ,ĳ).ToString().Trim();}ĩ ň(IMyTerminalBlock Ņ){MyIni Ō=new MyIni();
if(!Ō.TryParse(Ņ.CustomData))return null;if(!Ō.ContainsSection(Ĵ))return null;ĩ Ő=new ĩ(Ņ.CustomName);List<MyIniKey>ő=new
List<MyIniKey>();Ō.GetKeys(Ĵ,ő);List<string>Œ=new List<string>();foreach(var œ in ő){string Ŕ=Ō.Get(Ĵ,œ.Name).ToString();Œ.
Add(œ.Name+"="+Ŕ);}ŕ(Œ,Ő);return Ő;}void ŕ(List<string>Œ,ĩ Ő){ĩ[]Ŗ=new ĩ[20];Ŗ[0]=Ő;for(int u=0;u<Œ.Count;u++){string ŗ=Œ[u
];if(ŗ.StartsWith(";")||ŗ.StartsWith("#"))continue;int Ř=0;while(Ř<ŗ.Length&&ŗ[Ř]=='.')Ř++;string ř=ŗ.Substring(Ř).Trim()
;if(string.IsNullOrEmpty(ř))continue;string Ě;string Ŕ;int Ś=ř.IndexOf('=');if(Ś>=0){Ě=ř.Substring(0,Ś).Trim();Ŕ=ř.
Substring(Ś+1).Trim();}else{Ě=ř.Trim();Ŕ=null;}if(string.IsNullOrEmpty(Ě))continue;int ś=Ě.IndexOf(':');if(ś>=0)Ě=Ě.Substring(ś+1
).Trim();ĩ Ŝ=Ŗ[Ř]??Ő;bool ŝ=!string.IsNullOrEmpty(Ŕ);bool Ş=false;if(u+1<Œ.Count){string ş=Œ[u+1];int Š=0;while(Š<ş.
Length&&ş[Š]=='.')Š++;Ş=(Š>Ř);}if(Ş){ĩ Ţ=š(Ŝ,Ě);if(Ţ==null){Ţ=ŝ?new ĩ(Ě,Ŕ):new ĩ(Ě);Ŝ.ţ(Ţ);}else if(ŝ){Ţ.Ť=Ŕ;}Ŗ[Ř+1]=Ţ;}else
if(ŝ){Ŝ.ţ(new ť(Ě,Ŕ));}else{if(š(Ŝ,Ě)==null)Ŝ.ţ(new ĩ(Ě));}for(int Ŧ=Ř+2;Ŧ<Ŗ.Length;Ŧ++)Ŗ[Ŧ]=null;}}ĩ š(ĩ Ŝ,string Ě){for(
int u=0;u<Ŝ.ŧ.Count;u++){ĩ Ţ=Ŝ.ŧ[u]as ĩ;if(Ţ!=null&&Ţ.Ũ==Ě)return Ţ;}return null;}public void Ŭ(string Ç){ũ a=Į.Ū(Ç);if(a==
null)return;a.ū.Ŭ();Į.ŭ(a);}public void Ů(string Ç){ũ a=Į.Ū(Ç);if(a==null)return;a.ū.Ů();Į.ŭ(a);}public void ź(string Ç){ũ a
=Į.Ū(Ç);if(a==null)return;Į.ů(Ç);Ű ű=a.ū;if(ű.Ų){a.ų();Į.ŭ(a);return;}Ŵ Ŷ=ű.ŵ();if(Ŷ==null)return;ĩ Ţ=Ŷ as ĩ;if(Ţ!=null){
ű.ŷ(Ţ);if(Ţ.Ť!=null)ĭ.Ÿ(Ţ.Ť);Į.ŭ(a);return;}ť Ź=Ŷ as ť;if(Ź!=null)ĭ.Ÿ(Ź.Ť);Į.ŭ(a);}public void ų(string Ç){ũ a=Į.Ū(Ç);if(
a==null)return;a.ų();Į.ŭ(a);}public string ż(string Ç,string Ż){ũ a=Į.Ū(Ç);if(a==null)return"Display '"+Ç+"' not found";
bool Ž=a.ū.ż(Ż);if(!Ž)return"Menu path '"+Ż+"' not found on display '"+Ç+"'";Į.ŭ(a);return"";}public bool ƀ(string Ç,string
ž){if(!Ī.ContainsKey(ž))return false;ũ a=Į.Ū(Ç);if(a==null)return false;a.ſ(Ī[ž],ž);Į.ŭ(a);return true;}public bool Ƅ(
string Ç,string ž){if(string.IsNullOrEmpty(ž))return false;int Ɓ=ž.IndexOf(" > ",StringComparison.Ordinal);if(Ɓ<=0)return
false;string ņ=ž.Substring(0,Ɓ).Trim();if(string.IsNullOrEmpty(ņ)||!Ī.ContainsKey(ņ))return false;ũ a=Į.Ū(Ç);if(a==null)
return false;ĩ Ƃ=Ī[ņ];Ű ƃ=new Ű(Ƃ);if(!ƃ.ż(ž))return false;a.ſ(Ƃ,ņ);a.ū.ż(ž);Į.ŭ(a);return true;}public bool Ɔ(string Ç,string
ž){if(!Ī.ContainsKey(ž))return false;ũ a=Į.Ū(Ç);if(a==null)return false;a.ſ(Ī[ž],ž,ƅ:false);return true;}public string Ɗ(
string Ç,string Ƈ,string ƈ=null){return Į.Ɖ(Ç,Ƈ,ƈ);}}public interface Ŵ{string Ũ{get;set;}Ƌ ƌ{get;}event Action<Ŵ>ƍ;}public
enum Ƌ{Ǝ,Ə,Ũ,}public class ť:Ɛ{public override Ƌ ƌ{get{return Ƌ.Ə;}}public string Ť{get;set;}public ť(string Ě,string Ƒ):
base(Ě){Ť=Ƒ;}}public class ũ:Z{public class ƙ{public string ƒ;public int Ɠ;public int Ɣ;public W ƕ;public string Ɩ;public W
Ɨ;public string Ƙ;}class Ɯ{public ĩ ƚ;public string ƛ;public Ű ū;public Ɯ(ĩ Ő,string ņ,Ű ű){ƚ=Ő;ƛ=ņ;ū=ű;}}public string X
{get;set;}public Ű ū{get;set;}public string ƛ{get;set;}ĩ Ɲ;List<Ɯ>ƞ=new List<Ɯ>();W Ɵ;
string Ơ;List<W>ơ=new List<W>();List<string>Ƣ=new List<string>();public W ƕ=>Ɵ;public string Ɩ=>Ơ;W ƣ;string
Ƥ;public W Ɨ=>ƣ;public string Ƙ=>Ƥ;public void Ʀ(W ƥ,string ƈ=null){ƣ=ƥ;Ƥ=ƈ;}public void Ƨ()=>ƣ=null;public ƙ Ʃ(){return
new ƙ{ƒ=ū.ƨ,Ɠ=ū.Ɠ,Ɣ=ū.Ɣ,ƕ=Ɵ,Ɩ=Ơ,Ɨ=ƣ,Ƙ=Ƥ,};}public void Ƭ(ƙ ƪ){if(ƪ==null)return;ū.ƫ(ƪ.ƒ,ƪ.Ɠ,ƪ.Ɣ);Ɵ=ƪ.ƕ;Ơ=ƪ.Ɩ;ƣ=ƪ.Ɨ;Ƥ=ƪ.Ƙ;}
public int ƭ{get;set;}const int Ʈ=2;const int Ư=2;public const float ư=28.8f;public const float Ʊ=4f;float Ʋ;float Ƴ;
public ũ(string Þ,IMyTextSurface Ð,IMyTerminalBlock Ņ,MyIni ƴ,ĩ Ƃ,string ņ,float Ƶ,float ƶ=2f):base(Ð,Ņ,ƴ,Ņ is IMyCockpit){X=Þ
;ƛ=ņ;Ʋ=Ƶ;Ƴ=ƶ>0f?ƶ:1f;Ɲ=Ƃ;Ʒ.ContentType=ContentType.SCRIPT;Ʒ.Script="";Ƹ();ū=new Ű(Ƃ);}void Ƹ(){float ƺ=ư*ƹ();int ƻ=(int)
Math.Floor(b.Height/ƺ);int Ƽ=Ʈ+Ư;float ƽ=ƺ+Ʊ*2f;float ƾ=b.Height-Ƽ*ƺ;ƭ=(int)Math.Floor(ƾ/ƽ);if(ƭ<1)ƭ=1;}public void ǃ(){
Vector2 ƿ=Ʒ.TextureSize;Ċ.Add(ǀ.ǁ("SquareSimple",new Vector2(ƿ.X/2f,ƿ.Y/2f),ƿ,e.o.ǂ));}public void ǈ(){float ǅ=Ǆ?20f:40f;float
ǆ=Ǆ?4f:8f;Vector2 ê=new Vector2(b.X+b.Width-ǅ/2f-ǆ,b.Y+ǅ/2f+ǆ);Ǉ(ê,ǅ,VRageMath.Color.White,e.o.ǂ);y(ê,ǅ*0.4f,e.o.p);}
public float ƹ(){float ǅ;if(Ʋ>0f)ǅ=Ʋ;else{var ǉ=20f;ǅ=b.Height/(ư*ǉ);ǅ=Math.Max(0.4f,Math.Min(2.0f,ǅ));ǅ=(float)(Math.Round(ǅ*
ǉ)/ǉ);}return ǅ*Ƴ;}public void d(string Ǌ,Vector2 ǋ,float ǌ,VRageMath.Color w){Ċ.Add(ú.ċ(Ǌ,ǋ,ǌ,w));}public void ǐ(float Ǎ
,VRageMath.Color w,float ǎ=2f){Vector2 ê=new Vector2(b.X+b.Width/2f,Ǎ);Ċ.Add(ú.Ǐ(ê,b.Width,w,ǎ));}public float ǒ(){float
ǌ=ƹ()*0.8f;float ƺ=ư*ǌ;float Ǒ=Ǆ?4f:e.f.g;return ƺ*1.45f+Ǒ;}public void Ǚ(){var c=b;float ǌ=ƹ()*0.8f;float ƺ=ư*ǌ;float Ǒ=
Ǆ?4f:e.f.g;float Ǔ=c.Y+c.Height-ƺ-Ǒ;float ǔ=Ǔ-ƺ*0.45f;ǐ(ǔ,e.o.Ǖ);string[]ǖ=new string[]{"\u2191 Up","\u2193 Down",
"\u25CF Select","\u25C4 Back"};float Ǘ=c.Width/ǖ.Length;for(int u=0;u<ǖ.Length;u++){float ǘ=c.X+Ǘ*(u+0.5f);Ċ.Add(ú.ċ(ǖ[u],new VRageMath
.Vector2(ǘ,Ǔ),ǌ,e.o.q,TextAlignment.CENTER));}}public void ſ(ĩ Ƃ,string ņ,bool ƅ=true){if(ƅ){if(ƞ.Count>=ǚ)ƞ.RemoveAt(0);
ƞ.Add(new Ɯ(Ɲ,ƛ,ū));}ƛ=ņ;Ɲ=Ƃ;ū=new Ű(Ƃ);}const int ǚ=16;public void Ǜ(W ƥ,string ƈ=null){if(Ɵ!=null){if(ơ.Count>=ǚ){ơ.
RemoveAt(0);Ƣ.RemoveAt(0);}ơ.Add(Ɵ);Ƣ.Add(Ơ);}Ɵ=ƥ;Ơ=ƈ;}public bool ų(){if(Ɵ!=null){if(ơ.Count>0){Ɵ=ơ[ơ.Count-1];ơ.RemoveAt(ơ.
Count-1);Ơ=Ƣ[Ƣ.Count-1];Ƣ.RemoveAt(Ƣ.Count-1);}else{Ɵ=null;Ơ=null;}return true;}if(ǜ&&ƣ!=null){ƣ=null;return true;}bool ǝ=ū.ų
();if(ǝ)return true;if(ƞ.Count>0){Ɯ Ǟ=ƞ[ƞ.Count-1];ƞ.RemoveAt(ƞ.Count-1);ƛ=Ǟ.ƛ;Ɲ=Ǟ.ƚ;ū=Ǟ.ū;return true;}return false;}}
public class ĩ:Ɛ{public override Ƌ ƌ{get{return Ƌ.Ǝ;}}List<Ŵ>ǟ=new List<Ŵ>();public ĩ Ǡ{get;set;}public string Ť{get;
set;}public ĩ(string Ě):base(Ě){}public ĩ(string Ě,string Ƒ):base(Ě){Ť=Ƒ;}public List<Ŵ>ŧ{get{return ǟ;}}public void ţ(Ŵ ǡ)
{if(ǟ.Contains(ǡ))return;ǟ.Add(ǡ);ĩ Ǣ=ǡ as ĩ;if(Ǣ!=null)Ǣ.Ǡ=this;}public void ǣ(Ŵ ǡ){if(!ǟ.Contains(ǡ))return;ǟ.Remove(ǡ)
;ĩ Ǣ=ǡ as ĩ;if(Ǣ!=null)Ǣ.Ǡ=null;}public void Ǥ(){ǟ.Clear();}}public abstract class Ɛ:Ŵ{public string Ũ{get{return ǥ;}set{
if(ǥ==value)return;ǥ=value;if(ƍ!=null)ƍ(this);}}string ǥ;public abstract Ƌ ƌ{get;}public event Action<Ŵ>ƍ;protected Ɛ(
string Ě){ǥ=Ě;}}public class ī{const string Ǧ=" >";const string ǧ="<< Back";string Ǩ;string ǩ;string Ǫ;į İ;public ī(string ǫ,string Ǭ,string ǭ,į Ǯ){Ǩ=ǫ;ǩ=Ǭ;Ǫ=ǭ;İ=Ǯ;}public void z(MySpriteDrawFrame ǯ,ũ a){Ű ű=a.ū
;int ǰ=a.ƭ;ű.Ǳ(ǰ);RectangleF c=a.b;float ǌ=a.ƹ();float ƺ=ũ.ư*ǌ;float ǳ=ǲ.Č(a,a.ƛ);float Ǵ=c.X+e.f.g;float Ǎ=ǳ;string[]Ƕ=ǵ
(ű);int Ƿ=ű.Ɣ;int Ǹ=Ƿ+ǰ;if(Ǹ>Ƕ.Length)Ǹ=Ƕ.Length;float ǹ=ũ.Ʊ;for(int u=Ƿ;u<Ǹ;u++){bool Ǻ=u==ű.Ɠ;float ǻ=ƺ+ǹ*2f;if(Ǻ){
Vector2 Ǽ=new Vector2(c.X+c.Width/2f,Ǎ+ǻ/2f);ǯ.Add(ǀ.ǁ("SquareSimple",Ǽ,new Vector2(c.Width,ǻ),e.o.ǽ));}bool Ǿ=Ƕ[u]==ǧ;Color ǿ=
Ǻ?e.o.r:Ǿ?e.o.q:e.o.r;float Ȁ=Ǿ?ǹ*2f:ǹ;float ȁ=Ǿ?ǹ*2f:ǹ;Ǎ+=Ȁ;string Ȃ=Ǻ?"> ":"  ";ȃ(a,Ȃ+Ƕ[u],Ǵ,ref Ǎ,ƺ,ǌ,ǿ);Ǎ+=ȁ;}}void ȃ
(ũ a,string Ǌ,float Ǵ,ref float Ǎ,float ƺ,float ǌ,Color w){a.d(Ǌ,new Vector2(Ǵ,Ǎ),ǌ,w);Ǎ+=ƺ;}void Ȅ(ũ a,ref float Ǎ,float
ƺ){float ǔ=Ǎ+ƺ*0.45f;a.ǐ(ǔ,e.o.Ǖ);Ǎ+=ƺ;}string[]ǵ(Ű ű){int Ȇ=ű.ȅ.ŧ.Count;bool ȇ=ű.ȅ.Ǡ!=null;int Ȉ=Ȇ+(ȇ?1:0);string[]Ƕ=new
string[Ȉ];for(int u=0;u<Ȇ;u++){Ŵ ǡ=ű.ȅ.ŧ[u];string ȉ=ǡ.ƌ==Ƌ.Ǝ?Ǧ:"";Ƕ[u]=ǡ.Ũ+ȉ;}if(ȇ)Ƕ[Ȇ]=ǧ;return Ƕ;}}public class Ű{public ĩ
ȅ{get;set;}public int Ɠ{get;set;}public int Ɣ{get;set;}List<ĩ>Ȋ=new List<ĩ>();string ȋ;
bool Ȍ=true;public Ű(ĩ Ő){ŷ(Ő);}public void ŷ(ĩ Ţ){ȅ=Ţ;Ɠ=0;Ɣ=0;if(!Ȋ.Contains(Ţ))Ȋ.Add(Ţ);Ȍ=true;}public bool ų(){if(Ȋ.Count
<=1)return false;Ȋ.RemoveAt(Ȋ.Count-1);ĩ ȍ=Ȋ[Ȋ.Count-1];ȅ=ȍ;Ɠ=0;Ɣ=0;Ȍ=true;return true;}public void Ŭ(){int Ȏ=ȅ.ŧ.Count-1;
if(ȅ.Ǡ!=null)Ȏ++;if(Ɠ>0)Ɠ--;else Ɠ=Ȏ;if(Ɠ<Ɣ)Ɣ=Ɠ;}public void Ů(){int Ȏ=ȅ.ŧ.Count-1;if(ȅ.Ǡ!=null)Ȏ++;if(Ɠ<Ȏ)Ɠ++;else Ɠ=0;}
public void Ǳ(int ǰ){if(Ɠ<Ɣ)Ɣ=Ɠ;else if(Ɠ>=Ɣ+ǰ)Ɣ=Ɠ-ǰ+1;}public Ŵ ŵ(){if(Ɠ<ȅ.ŧ.Count)return ȅ.ŧ[Ɠ];return null;}public bool Ų{
get{return ȅ.Ǡ!=null&&Ɠ>=ȅ.ŧ.Count;}}public string Ȑ{get{if(Ȍ){string[]ȏ=new string[Ȋ.Count];for(int u=0;u<Ȋ.Count;u++)ȏ[u]
=Ȋ[u].Ũ;ȋ=string.Join(" > ",ȏ);Ȍ=false;}return ȋ;}}public string ƨ{get{if(Ȋ.Count<=1)return"";string[]ȏ=new string[Ȋ.
Count-1];for(int u=1;u<Ȋ.Count;u++)ȏ[u-1]=Ȋ[u].Ũ;return string.Join(" > ",ȏ);}}ĩ ȑ(){return Ȋ.Count>0?Ȋ[0]:null;}int Ȓ(ĩ Ţ){
int Ȏ=Ţ.ŧ.Count-1;if(Ţ.Ǡ!=null)Ȏ++;return Ȏ;}bool ȗ(string ȏ,out ĩ ž){ĩ Ő=ȑ();ž=Ő;if(Ő==null)return false;Ȋ.Clear();Ȋ.Add(Ő
);if(string.IsNullOrEmpty(ȏ))return true;string[]ȓ=ȏ.Split(new[]{" > "},StringSplitOptions.RemoveEmptyEntries);for(int u=
0;u<ȓ.Length;u++){string Ȕ=ȓ[u].Trim();if(u==0&&Ȕ==Ő.Ũ)continue;ĩ ȕ=null;for(int Ȗ=0;Ȗ<ž.ŧ.Count;Ȗ++){ĩ Ǣ=ž.ŧ[Ȗ]as ĩ;if(Ǣ
!=null&&Ǣ.Ũ==Ȕ){ȕ=Ǣ;break;}}if(ȕ==null){ž=Ő;Ȋ.Clear();Ȋ.Add(Ő);return false;}ž=ȕ;Ȋ.Add(ž);}return true;}public bool ƫ(
string Ș,int ș,int Ț){ĩ ț;if(!ȗ(Ș,out ț)){ȅ=ȑ();Ɠ=0;Ɣ=0;Ȍ=true;return false;}ȅ=ț;int Ȝ=Ȓ(ț);Ɠ=Ȝ<0?0:Math.Max(0,Math.Min(ș,Ȝ));
Ɣ=Math.Max(0,Math.Min(Ț,Ɠ));Ȍ=true;return true;}public bool ż(string ȏ){if(string.IsNullOrEmpty(ȏ))return false;ĩ ț;if(!ȗ
(ȏ,out ț))return false;ȅ=ț;Ɠ=0;Ɣ=0;Ȍ=true;return true;}}public static class ɔ{public struct è{public float ȝ;public float
Ȟ;public float ȟ;public bool Ƞ;public string ä;public string ȡ;public string å;public Color?æ;public float?ç;public float
?Ȣ;public float?ȣ;public float?Ğ;public float?ğ;}const float Ȥ=33600000f;public static void Č(Z a,Vector2 ê,float ȥ,è í){
a.y(ê,ȥ*2f,ú.ü(e.o.ý,220));a.y(ê,ȥ*2f-4f,ú.ü(e.o.ÿ,255));if(í.Ğ.HasValue)Ȧ(a,ê,ȥ,í.Ğ.Value,í.ğ.Value);else ȧ(a,ê,ȥ);Ȩ(a,ê
,ȥ,í.Ȟ,í.Ȟ>float.MinValue/2f);Ȩ(a,ê,ȥ,í.ȟ,í.ȟ<float.MaxValue/2f);ȩ(a,ê,ȥ,í);}static void ȩ(Z a,Vector2 ê,float ȥ,è í){
float Ȫ=Math.Max(0.55f,ȥ/80f)*(í.ç??1f);if(í.Ȣ.HasValue){float ȫ=Math.Max(0f,Math.Min(1f,í.Ȣ.Value/Ȥ));Ȭ(a,ê,ȥ,ȫ,MathHelper.
ToRadians(130f),MathHelper.ToRadians(100f),true,new Color(0,180,220,220),"\u03c4",Ȫ*0.70f);}if(í.ȣ.HasValue){float ȫ=Math.Max(0f,
Math.Min(1f,í.ȣ.Value/Ȥ));Ȭ(a,ê,ȥ,ȫ,MathHelper.ToRadians(50f),MathHelper.ToRadians(100f),false,new Color(220,140,0,220),
"\u03c4b",Ȫ*0.70f);}Vector2 ȭ=ê+new Vector2((float)Math.Cos(í.ȝ),(float)Math.Sin(í.ȝ))*(ȥ*0.80f);a.Ȯ(ê,ȭ,e.o.p,e.f.ȯ);a.y(ê,e.f.Ȱ
,e.o.p);if(í.ä!=null)a.ȱ(í.ä,new Vector2(ê.X,ê.Y-ȥ*0.65f),e.o.r,Ȫ);if(í.ȡ!=null)a.ȱ(í.ȡ,new Vector2(ê.X,ê.Y+ȥ*0.30f),e.o.
s,Ȫ*0.85f);if(í.å!=null)a.ȱ(í.å,new Vector2(ê.X,ê.Y+ȥ*0.55f),í.æ??e.o.s,Ȫ*0.75f);if(í.Ƞ)a.d(Math.Round(MathHelper.
ToDegrees(í.ȝ),1).ToString("F1")+" deg \u00B0",new Vector2(ê.X-30f,ê.Y+ȥ+6f),e.o.q,e.Ȳ.ȳ);}static void ȧ(Z a,Vector2 ê,float ȥ){
Color ȴ=ú.ü(e.o.s,160);for(int ȵ=0;ȵ<360;ȵ+=45){float ȶ=MathHelper.ToRadians(ȵ);Vector2 ȷ=new Vector2((float)Math.Cos(ȶ),(
float)Math.Sin(ȶ));a.Ȯ(ê+ȷ*(ȥ*0.75f),ê+ȷ*(ȥ*0.92f),ȴ,e.f.ȸ);}}static void Ȧ(Z a,Vector2 ê,float ȥ,float ĥ,float Ħ){Color ȹ=ú.
ü(e.o.s,200);float Ⱥ=Ħ-ĥ;const int Ȼ=32;for(int u=0;u<Ȼ;u++){float ȼ=ĥ+Ⱥ*(u/(float)Ȼ);float Ƚ=ĥ+Ⱥ*((u+1)/(float)Ȼ);a.Ȯ(ê+
new Vector2((float)Math.Cos(ȼ),(float)Math.Sin(ȼ))*(ȥ*0.95f),ê+new Vector2((float)Math.Cos(Ƚ),(float)Math.Sin(Ƚ))*(ȥ*0.95f)
,ȹ,3f);}Vector2 Ⱦ=new Vector2((float)Math.Cos(ĥ),(float)Math.Sin(ĥ));Vector2 ȿ=new Vector2((float)Math.Cos(Ħ),(float)Math
.Sin(Ħ));a.Ȯ(ê+Ⱦ*(ȥ*0.55f),ê+Ⱦ*(ȥ*0.95f),ȹ,3f);a.Ȯ(ê+ȿ*(ȥ*0.55f),ê+ȿ*(ȥ*0.95f),ȹ,3f);Color ȴ=ú.ü(e.o.s,160);float ɀ=
MathHelper.ToRadians(30f);int Ɂ=(int)Math.Round(Ⱥ/ɀ);for(int u=0;u<=Ɂ;u++){float ȶ=ĥ+u*ɀ;if(ȶ>Ħ+0.001f)break;Vector2 ȷ=new Vector2
((float)Math.Cos(ȶ),(float)Math.Sin(ȶ));a.Ȯ(ê+ȷ*(ȥ*0.75f),ê+ȷ*(ȥ*0.92f),ȴ,e.f.ȸ);}}static void Ȭ(Z a,Vector2 ê,float ȥ,
float ɂ,float Ƀ,float Ʉ,bool Ʌ,Color Ɇ,string Ě,float Ć){const int ɇ=20;const float Ɉ=4f;float ɉ=ȥ*0.62f;Color Ɋ=new Color(60
,60,60,120);for(int u=0;u<ɇ;u++){float ɋ=Ʌ?1f:-1f;float ȼ=Ƀ+ɋ*Ʉ*(u/(float)ɇ);float Ƚ=Ƀ+ɋ*Ʉ*((u+1)/(float)ɇ);a.Ȯ(ê+new
Vector2((float)Math.Cos(ȼ),(float)Math.Sin(ȼ))*ɉ,ê+new Vector2((float)Math.Cos(Ƚ),(float)Math.Sin(Ƚ))*ɉ,Ɋ,Ɉ);}if(ɂ>0.001f){int
Ɍ=Math.Max(1,(int)Math.Ceiling(ɇ*ɂ));for(int u=0;u<Ɍ;u++){float ɋ=Ʌ?1f:-1f;float ȼ=Ƀ+ɋ*Ʉ*ɂ*(u/(float)Ɍ);float Ƚ=Ƀ+ɋ*Ʉ*ɂ*(
(u+1)/(float)Ɍ);a.Ȯ(ê+new Vector2((float)Math.Cos(ȼ),(float)Math.Sin(ȼ))*ɉ,ê+new Vector2((float)Math.Cos(Ƚ),(float)Math.
Sin(Ƚ))*ɉ,Ɇ,Ɉ);}}float ɍ=Ʌ?1f:-1f;float Ɏ=Ƀ+ɍ*Ʉ;Vector2 ɏ=new Vector2((float)Math.Cos(Ɏ),(float)Math.Sin(Ɏ));a.ȱ(Ě,ê+ɏ*(ɉ*
0.78f),Ɇ,Ć);}static void Ȩ(Z a,Vector2 ê,float ȥ,float ɐ,bool ɑ){if(!ɑ)return;Vector2 ȷ=new Vector2((float)Math.Cos(ɐ),(float
)Math.Sin(ɐ));a.Ȯ(ê+ȷ*(ȥ*0.45f),ê+ȷ*(ȥ*1.00f),e.o.Ĝ,e.f.ɒ);a.y(ê+ȷ*ȥ,e.f.ɓ,e.o.Ĝ);}}public static class ǲ{public static
float Č(ũ a,string ɕ){RectangleF c=a.b;float ǌ=a.ƹ();float ƺ=ũ.ư*ǌ;float ɖ=e.f.g;float ɗ=c.Y+ɖ;float ǔ=ɗ+ƺ+6f;float ɘ=ǔ+ƺ*
0.75f;a.d(ɕ,new Vector2(c.X+e.f.g,ɗ),ǌ,e.o.r);a.ǐ(ǔ,e.o.Ǖ);return ɘ;}}public abstract class Ď<ə>:ɚ,ɛ where ə:class,
IMyTerminalBlock{public abstract string X{get;}protected abstract string ď{get;}protected à à;protected string ɜ;protected Ď(à
á){à=á;}public void ɝ(string ƈ)=>ɜ=ƈ;protected abstract List<ə>Đ();protected abstract void Ę(Z a,ə Ņ,Ē ē);public void z(Z
a){RectangleF c=a.b;ũ ɞ=a as ũ;var ɟ=Đ();if(ɟ.Count==0){if(ɞ!=null)ǲ.Č(ɞ,ď);return;}ə Ņ=ɠ(ɟ);if(Ņ==null){if(ɞ!=null)ǲ.Č(ɞ
,"Block not found: "+ɜ);return;}float ǳ=ɞ!=null?ǲ.Č(ɞ,Ņ.CustomName):c.Y+e.f.ɡ;var ē=new Ē(c,ɢ:ǳ,ǌ:a.ɣ);Ę(a,Ņ,ē);}ə ɠ(List
<ə>ɟ){if(string.IsNullOrWhiteSpace(ɜ))return ɟ[0];string ɥ=ɤ(ɜ);List<ə>ɧ=à.ɦ<ə>(ɥ);if(ɧ!=null&&ɧ.Count>0)return ɧ[0];for(
int u=0;u<ɟ.Count;u++)if(string.Equals(ɟ[u].CustomName,ɥ,StringComparison.OrdinalIgnoreCase)||string.Equals(ɟ[u].
DisplayNameText,ɥ,StringComparison.OrdinalIgnoreCase))return ɟ[u];return null;}static string ɤ(string ɨ){string ɩ=ɨ.Trim();if(ɩ.Length
>=2&&ɩ[0]=='"'&&ɩ[ɩ.Length-1]=='"')ɩ=ɩ.Substring(1,ɩ.Length-2).Trim();return ɩ;}protected static string ɪ(float Ŕ){if(Ŕ>=
1000000f)return(Ŕ/1000000f).ToString("F1")+" M";if(Ŕ>=1000f)return(Ŕ/1000f).ToString("F1")+" k";return Ŕ.ToString("F0");}
protected void Ĥ(Z a,Ē ē,float ɫ,float ɬ,float ɭ,float ɮ,float ɯ,float ɰ,bool ɱ,string Ĕ,Color ɲ,float?ĥ,float?Ħ){bool ɳ=a.Ǆ||ē.b
.Height<250f;Color ɶ=ɱ?e.o.ɴ:e.o.ɵ;RectangleF c=ē.b;float ɷ,ɸ;Vector2 ɹ;if(ɳ){ɷ=5f;float ɺ=System.Math.Min(c.Width/2f,c.
Height/2f)-ɷ-e.f.g;ɸ=System.Math.Max(ɺ,15f);ɹ=new Vector2(c.X+c.Width/2f,c.Y+c.Height/2f);}else{var ɻ=a as ũ;float ɼ=ɻ!=null?ɻ
.ǒ():c.Height*0.18f;float k=(c.Y+c.Height-ɼ)-ē.ɽ;ɷ=8f;float ɺ=System.Math.Min(c.Width/2f,k/2f)-ɷ-e.f.g;ɸ=System.Math.Max(
ɺ,20f);ɹ=new Vector2(c.X+c.Width/2f,ē.ɽ+k/2f);}a.y(ɹ,(ɸ+ɷ)*2f,ɶ);ɔ.Č(a,ɹ,ɸ,new ɔ.è{ȝ=ɫ,Ȟ=ɬ,ȟ=ɭ,ä=MathHelper.ToDegrees(ɫ).
ToString("F1")+"\u00B0",ȡ=ɮ.ToString("F1")+" RPM",ç=ɳ?1.5f:1f,å=Ĕ,æ=ɲ,Ȣ=ɯ,ȣ=ɰ,Ğ=ĥ,ğ=Ħ});}protected void ʌ(Z a,Ē ē,float ɾ,float
ɿ,float ʀ,float ʁ,float ʂ,float ʃ,string Ĕ,Color ĕ,Color ʄ){RectangleF c=ē.b;var ɻ=a as ũ;float ɼ=ɻ!=null?ɻ.ǒ():c.Height*
0.18f;float k=System.Math.Max(c.Y+c.Height-ɼ-ē.ɽ,40f);ʅ.Č(a,new Vector2(c.X+c.Width/2f,ē.ɽ+k/2f),c.Width,k,new ʅ.è{ʆ=ɾ,ʇ=ɿ,ʈ=
ʀ,ʉ=ʁ,ʊ=ʂ,ä=ɾ.ToString("F2")+" m",ȡ=ʃ.ToString("F2")+" m/s",å=Ĕ,æ=ĕ,ʋ=ʄ,});}protected void ė(Z a,Ē ē,float ʍ,string Ĕ,
Color ĕ){RectangleF c=ē.b;var ɻ=a as ũ;float ɼ=ɻ!=null?ɻ.ǒ():c.Height*0.18f;float k=System.Math.Max(c.Y+c.Height-ɼ-ē.ɽ,40f);č
.Č(a,new Vector2(c.X+c.Width/2f,ē.ɽ+k/2f),c.Width,k,new č.è{ã=ʍ,ä=(ʍ*100f).ToString("F0")+"%",å=Ĕ,æ=ĕ,});}}public class Ĺ
:W{public string X{get{return"MenuView";}}ī Ĭ;public Ĺ(ī ʎ){Ĭ=ʎ;}public void z(Z a){ũ ɞ=(ũ)a;Ĭ.z(a.Ċ,ɞ);}}public
static class ʢ{const int ʏ=4;public static void Č(Z a,Vector2 ʐ,float ë,float ì,float ʑ,float ʒ,float ʓ,float ʁ=float.
MinValue,float ʂ=float.MaxValue,bool ʔ=true,bool ʕ=true){float ʖ=ʓ-ʒ;if(ʖ<=0f)ʖ=1f;float ï=Math.Max(0f,Math.Min(1f,(ʑ-ʒ)/ʖ));
Vector2 ê=ʐ+new Vector2(ë/2f,ì/2f);a.Ċ.Add(ǀ.ǁ("SquareSimple",ê,new Vector2(ë,ì),ú.ü(e.o.ý,220)));a.Ċ.Add(ǀ.ǁ("SquareSimple",ê,
new Vector2(ë-4f,ì-4f),ú.ü(e.o.ÿ,255)));float ø=ë-8f;float ʗ=ø*ï;if(ʗ>0f){Vector2 ʘ=new Vector2(ʐ.X+4f+ʗ/2f,ê.Y);a.Ċ.Add(ǀ.
ǁ("SquareSimple",ʘ,new Vector2(ʗ,ì-8f),ú.ü(e.o.p,180)));}if(ʔ){Color ȴ=ú.ü(e.o.s,160);for(int u=0;u<=ʏ;u++){float ʙ=ʐ.X+
4f+ø*(u/(float)ʏ);bool ʚ=(u==0||u==ʏ);float ʛ=ʚ?ì*0.65f:ì*0.35f;a.Ȯ(new Vector2(ʙ,ê.Y-ʛ/2f),new Vector2(ʙ,ê.Y+ʛ/2f),ȴ,e.f.
ȸ);}ʜ(a,ʐ,ø,ì,ʁ,ʒ,ʖ,ʁ>float.MinValue/2f,ʕ);ʜ(a,ʐ,ø,ì,ʂ,ʒ,ʖ,ʂ<float.MaxValue/2f,ʕ);float ʝ=ʐ.X+4f+ø*ï;a.Ȯ(new Vector2(ʝ,ʐ.
Y+2f),new Vector2(ʝ,ʐ.Y+ì-2f),e.o.p,e.f.ȯ);if(ʕ){float ʞ=ʐ.Y+ì+4f;a.d(ʒ.ToString("F1")+"m",new Vector2(ʐ.X,ʞ),e.o.s,e.Ȳ.ȳ
);a.d(ʓ.ToString("F1")+"m",new Vector2(ʐ.X+ë-32f,ʞ),e.o.s,e.Ȳ.ȳ);string ʟ=ʑ.ToString("F2")+" m";a.d(ʟ,new Vector2(ê.X-22f
,ʞ),e.o.q,e.Ȳ.ȳ);}}}static void ʜ(Z a,Vector2 ʐ,float ø,float ì,float ʠ,float ʒ,float ʖ,bool ɑ,bool ʕ=true){if(!ɑ)return;
float ï=Math.Max(0f,Math.Min(1f,(ʠ-ʒ)/ʖ));float Ǵ=ʐ.X+4f+ø*ï;a.Ȯ(new Vector2(Ǵ,ʐ.Y),new Vector2(Ǵ,ʐ.Y+ì),e.o.Ĝ,e.f.ɒ);a.y(new
Vector2(Ǵ,ʐ.Y+ì),e.f.ɓ,e.o.Ĝ);if(ʕ){string ʡ=ʠ.ToString("F1")+"m";a.d(ʡ,new Vector2(Ǵ-14f,ʐ.Y-14f),e.o.Ĝ,e.Ȳ.ȳ);}}}public
struct Ē{public RectangleF b;public float ʣ;public float ʤ;public float ʥ;public float ɽ;
public Ē(RectangleF c,float ɢ,float ǌ,float ǆ=e.f.ʦ){b=c;ʣ=c.X+ǆ;ʤ=c.X+c.Width*0.45f;ʥ=ǌ+e.f.ʦ;ɽ=ɢ;}public float ʧ(){float Ǎ=ɽ
;ɽ+=ʥ;return Ǎ;}public void ʪ(Z a,string Ě,string Ŕ,Color ʨ){float Ǎ=ʧ();a.d(Ě,new Vector2(ʣ,Ǎ),e.o.ʩ);a.d(Ŕ,new Vector2(
ʤ,Ǎ),ʨ);}public void ʯ(Z a,float ɸ,float ʫ,float ʬ,float ʭ,float?Ƀ=null,float?ʮ=null){ɽ+=ʥ*0.5f;Vector2 ê=new Vector2(b.X
+b.Width/2f,ɽ+ɸ);ɔ.Č(a,ê,ɸ,new ɔ.è{ȝ=ʫ,Ȟ=ʬ,ȟ=ʭ,Ğ=Ƀ,ğ=ʮ});ɽ+=ɸ*2f+ʥ;}public void ʲ(Z a,float ʰ,float ʱ,float ʑ,float ʒ,
float ʓ,float ʁ=float.MinValue,float ʂ=float.MaxValue){ɽ+=ʥ*0.5f;Vector2 ʐ=new Vector2(b.X+(b.Width-ʰ)/2f,ɽ);ʢ.Č(a,ʐ,ʰ,ʱ,ʑ,ʒ,
ʓ,ʁ,ʂ);ɽ+=ʱ+ʥ;}}public class L:R{public L(A B):base(B){}public override void V(){à á=A.S<à>();A.S<G>().T(new ʳ(á));}}
public static class ʅ{public struct è{public float ʆ;public float ʇ;public float ʈ;public float ʉ;public float ʊ;public string
ä;public string ȡ;public string å;public Color?æ;public Color?ʋ;public float?ç;}const float ʴ=0.16f;const float ʵ=0.46f;
public static void Č(Z a,Vector2 ê,float ë,float ì,è í){float î=e.f.g;float ø=ë-î*2f;float ʶ=ê.X-ë/2f+î;float ʖ=í.ʈ-í.ʇ;if(ʖ<=
0f)ʖ=1f;float ï=Math.Max(0f,Math.Min(1f,(í.ʆ-í.ʇ)/ʖ));float ʷ=Math.Min(ï*2f,1f);float ʸ=Math.Max(ï*2f-1f,0f);float ʹ=ø*ʴ;
float ʺ=ø-ʹ;float ʻ=ʺ*0.5f;float ʼ=ʺ*0.5f;float ʽ=ʻ*ʷ;float ʾ=ʼ*ʸ;float ʿ=ì*ʵ;float ˀ=ʿ;float ˁ=ʿ*0.68f;float ˆ=ʿ*0.40f;float
ˇ=ê.Y-ì*0.14f;ˈ(a,ʶ,ʹ,ʻ,ʼ,ʽ,ʾ,ʿ,ˀ,ˁ,ˆ,ˇ);float Ć=Math.Max(0.55f,ì/220f)*(í.ç??1f);ˉ(a,ʶ,ʹ,ʺ,ˇ,ʿ,í,Ć);}static void ˈ(Z a,
float ʶ,float ʹ,float ʻ,float ʼ,float ʽ,float ʾ,float ʿ,float ˀ,float ˁ,float ˆ,float ˇ){float ˊ=ʶ+ʹ+ʻ/2f;ú.û(a,ˊ,ˇ,ʻ,ˁ,ú.ü(e
.o.ý,200));ú.û(a,ˊ,ˇ,ʻ-4f,ˁ-4f,ú.ü(e.o.ÿ,240));if(ʽ>2f){float ˋ=ʶ+ʹ+ʽ/2f;ú.û(a,ˋ,ˇ,ʽ,ˁ,ú.ü(e.o.þ,200));ú.û(a,ˋ,ˇ,ʽ-4f,ˁ-
4f,ú.ü(e.o.ý,240));}float ˌ=ʶ+ʹ+ʻ+ʼ/2f;ú.û(a,ˌ,ˇ,ʼ,ˆ,ú.ü(e.o.ý,140));ú.û(a,ˌ,ˇ,ʼ-4f,ˆ-4f,ú.ü(e.o.ÿ,200));if(ʾ>2f){float ˍ=
ʶ+ʹ+ʻ+ʾ/2f;ú.û(a,ˍ,ˇ,ʾ,ˆ,ú.ü(e.o.s,220));ú.û(a,ˍ,ˇ,ʾ-4f,ˆ-4f,ú.ü(e.o.ý,200));}float ˎ=ʶ+ʹ/2f;ú.û(a,ˎ,ˇ,ʹ,ˀ,ú.ü(e.o.þ,220)
);ú.û(a,ˎ,ˇ,ʹ-4f,ˀ-4f,ú.ü(e.o.ý,255));float ˏ=ʶ+ʹ+ʽ+ʾ;float ː=(ʾ>2f?ˆ:(ʽ>2f?ˁ:ˀ))+6f;a.Ȯ(new Vector2(ˏ,ˇ-ː/2f),new
Vector2(ˏ,ˇ+ː/2f),e.o.p,e.f.ȯ);a.y(new Vector2(ˏ,ˇ),e.f.Ȱ,e.o.p);}static void ˉ(Z a,float ʶ,float ʹ,float ʺ,float ˇ,float ʿ,è í
,float Ć){float ˑ=ʶ+ʹ+ʺ;float ˠ=ˇ-ʿ/2f;float ˡ=ˇ+ʿ/2f;float ă=ʶ+ʹ+ʺ*0.75f;if(í.ä!=null)a.ȱ(í.ä,new Vector2(ă,ˠ-14f),e.o.r
,Ć);if(í.ȡ!=null)a.ȱ(í.ȡ,new Vector2(ă,ˡ+14f),e.o.s,Ć*0.85f);float ˢ=ˡ+48f;float Ă=ʶ+ʹ+ʺ*0.25f;float ˣ=ʶ+ʹ+ʺ*0.75f;if(í.å
!=null)a.ȱ(í.å,new Vector2(Ă,ˢ),í.æ??e.o.s,Ć*0.75f);if(í.ʋ.HasValue)a.ȱ("TENSORS",new Vector2(ˣ,ˢ),í.ʋ.Value,Ć*0.75f);}}
public class ʳ:Ď<IMyPistonBase>{public override string X=>"PistonView";protected override string ď=>"No piston found.";public
ʳ(à á):base(á){}protected override List<IMyPistonBase>Đ()=>à.Đ<IMyPistonBase>();protected override void Ę(Z a,
IMyPistonBase ˤ,Ē ē){PistonStatus ę=ˤ.Status;string Ĕ;Color ĕ;Color ʄ=ġ.Ģ(ˤ);Ė(ę,out Ĕ,out ĕ);ʌ(a,ē,ˤ.CurrentPosition,ˤ.
LowestPosition,ˤ.HighestPosition,ˤ.MinLimit,ˤ.MaxLimit,ˤ.Velocity,Ĕ,ĕ,ʄ);}static void Ė(PistonStatus ę,out string Ě,out Color w){
switch(ę){case PistonStatus.Extending:Ě="EXTENDING";w=e.o.q;return;case PistonStatus.Extended:Ě="EXTENDED";w=e.o.Ĝ;return;case
PistonStatus.Retracting:Ě="RETRACTING";w=e.o.q;return;case PistonStatus.Retracted:Ě="RETRACTED";w=e.o.Ĝ;return;default:Ě="STOPPED";w
=e.o.s;return;}}}public class J:R{public J(A B):base(B){}public override void V(){à á=A.S<à>();var ˬ=A.S<G>();ˬ.T(new ˮ(á
));ˬ.T(new Ͱ(á));}}public class Ͱ:ɚ,ɛ{public string X=>"RotorGridView";à ͱ;int Ͳ;int ͳ;List<string>ʹ=
new List<string>();public Ͱ(à á){ͱ=á;}public void ɝ(string ƈ){Ͳ=0;ͳ=0;ʹ.Clear();if(string.IsNullOrEmpty(ƈ))return;ƈ=ƈ.Trim(
).Trim('"');if(string.IsNullOrEmpty(ƈ))return;string Ͷ=ƈ;string ͷ=null;int ͺ=ƈ.IndexOf(':');if(ͺ>=0){Ͷ=ƈ.Substring(0,ͺ);ͷ
=ƈ.Substring(ͺ+1);}bool ͻ=false;if(!string.IsNullOrEmpty(Ͷ)){int ͼ=Ͷ.IndexOf('x');if(ͼ>0){int ͽ,Ά;if(int.TryParse(Ͷ.
Substring(0,ͼ),out ͽ)&&int.TryParse(Ͷ.Substring(ͼ+1),out Ά)){Ͳ=Math.Max(1,ͽ);ͳ=Math.Max(1,Ά);ͻ=true;}}else{int ͽ;if(int.TryParse(
Ͷ,out ͽ)){Ͳ=Math.Max(1,ͽ);ͻ=true;}}}if(ͺ<0&&!ͻ)ͷ=Ͷ;if(!string.IsNullOrEmpty(ͷ)){foreach(string Έ in ͷ.Split(',')){string
Þ=Έ.Trim();if(Þ.Length>0)ʹ.Add(Þ);}}}public void z(Z a){var ɞ=a as ũ;float ɘ=ɞ!=null?ǲ.Č(ɞ,"ROTOR GRID"):a.b.Y;List<
IMyMotorStator>Ί=Ή();if(Ί.Count==0){a.d("No rotors found.",new Vector2(a.b.X+e.f.g,ɘ),e.o.ʩ);return;}int Ύ=Ͳ>0?Ͳ:Ό(Ί.Count);int Ώ=ͳ>0?
ͳ:(int)Math.Ceiling(Ί.Count/(float)Ύ);RectangleF c=a.b;float ɼ=ɞ!=null?ɞ.ǒ():c.Height*0.12f;float k=(c.Y+c.Height-ɼ)-ɘ;
float ΐ=c.Width/Ύ;float Α=k/Ώ;float Β=Math.Max(6f,14f-Ύ*2f);const float ɷ=4f;int Γ=0;for(int Δ=0;Δ<Ώ&&Γ<Ί.Count;Δ++){for(int
Ε=0;Ε<Ύ&&Γ<Ί.Count;Ε++){IMyMotorStator Ζ=Ί[Γ++];float Η=c.X+Ε*ΐ+Β;float Θ=ɘ+Δ*Α+Β;float ø=ΐ-Β*2f;float ù=Α-Β*2f;float n=
Math.Min(ø,ù)/2f-ɷ;float ɸ=Math.Max(n,10f);Vector2 ê=new Vector2(Η+ø/2f,Θ+ù/2f);bool ɱ=Ζ.RotorLock;Color Ι=ɱ?e.o.ɴ:e.o.ɵ;a.y
(ê,(ɸ+ɷ)*2f,Ι);string Κ=MathHelper.ToDegrees(Ζ.Angle).ToString("F1")+"\u00B0";string Λ=Ζ.TargetVelocityRPM.ToString("F1")
+" RPM";Color ģ=ġ.Ģ(Ζ);ɔ.Č(a,ê,ɸ,new ɔ.è{ȝ=Ζ.Angle,Ȟ=Ζ.LowerLimitRad,ȟ=Ζ.UpperLimitRad,ä=Κ,ȡ=Λ,å="TENSORS",æ=ģ,Ȣ=Ζ.Torque
,ȣ=Ζ.BrakingTorque});float Μ=Math.Max(0.40f,ɸ/90f);float Ν=ê.Y-ɸ-ɷ-Μ*16f;a.ȱ(Ξ(Ζ.CustomName,Ύ),new Vector2(ê.X,Ν),e.o.s,Μ
);}}}List<IMyMotorStator>Ή(){if(ʹ.Count==0)return ͱ.Đ<IMyMotorStator>();List<IMyMotorStator>Ο=ͱ.Đ<IMyMotorStator>();
Dictionary<string,IMyMotorStator>Π=new Dictionary<string,IMyMotorStator>(Ο.Count);foreach(IMyMotorStator Ά in Ο)Π[Ά.CustomName]=Ά;
List<IMyMotorStator>Ρ=new List<IMyMotorStator>(ʹ.Count);foreach(string Þ in ʹ){IMyMotorStator Σ;if(Π.TryGetValue(Þ,out Σ))Ρ.
Add(Σ);}return Ρ;}static int Ό(int Τ){if(Τ<=1)return 1;if(Τ<=4)return 2;if(Τ<=9)return 3;return 4;}static string Ξ(string Þ
,int Ύ){int Υ=Math.Max(9,(int)Math.Round(20f*1.5f/Ύ));if(Þ.Length<=Υ)return Þ;const int Φ=2;int Χ=Υ-1-Φ;if(Χ<1)Χ=1;return
Þ.Substring(0,Χ)+"\u2026"+Þ.Substring(Þ.Length-Φ);}}public class ˮ:Ď<IMyMotorStator>{public override string X=>
"RotorView";protected override string ď=>"No rotor found.";public ˮ(à á):base(á){}protected override List<IMyMotorStator>Đ()=>à.Đ<
IMyMotorStator>();protected override void Ę(Z a,IMyMotorStator Ζ,Ē ē){Color ģ=ġ.Ģ(Ζ);Ĥ(a,ē,Ζ.Angle,Ζ.LowerLimitRad,Ζ.UpperLimitRad,Ζ.
TargetVelocityRPM,Ζ.Torque,Ζ.BrakingTorque,Ζ.RotorLock,"TENSORS",ģ,ĥ:null,Ħ:null);}}public class Ψ:ª{G µ;public override string
X{get{return"view/back";}}public Ψ(G º){µ=º;}public override string Ò(Á Â){if(Â.Ã.Count<1)return
"Usage: view/back <display name>";µ.ų(Â.Ã[0]);return"";}}public class Ω:ª{G µ;public override string X{get{return"view/down";}}public Ω(G º){µ=º
;}public override string Ò(Á Â){if(Â.Ã.Count<1)return"Usage: view/down <display name>";µ.Ů(Â.Ã[0]);return"";}}public
class Ϊ:ª{G µ;public override string X{get{return"view/go";}}public Ϊ(G º){µ=º;}public override string Ò(Á Â){if(Â.Ã
.Count<2)return"Usage: view/go <display name> <view, menu name, or path> [parameter]\nExamples:\n  view/go \"Bridge LCD\" \"RotorView\" \"My Rotor\" (switch view with parameter)\n  view/go \"Bridge LCD\" \"LightsMenu\" (switch menu)\n  view/go \"Bridge LCD\" \"Power > Reactors\" (navigate path)\n  view/go self \"CircleView\" (target the display that triggered this command)"
;string Ç=Â.Ã[0];string ž=Â.Ã[1];string ƈ=Â.Ã.Count>=3?Â.Ã[2]:null;if(Ç=="self"){Ç=µ.Ϋ();}string Ρ=µ.Ɖ(Ç,ž,ƈ);if(!string.
IsNullOrEmpty(Ρ))return Ρ;return"";}}public class ά:ª{G µ;public override string X{get{return"view/select";}}public ά(G º){µ
=º;}public override string Ò(Á Â){if(Â.Ã.Count<1)return"Usage: view/select <display name>";µ.ź(Â.Ã[0]);return"";}}public
class έ:ª{G µ;public override string X{get{return"view/up";}}public έ(G º){µ=º;}public override string Ò(Á Â){if(Â.Ã
.Count<1)return"Usage: view/up <display name>";µ.Ŭ(Â.Ã[0]);return"";}}public interface ɚ:W{}public interface ħ{ĩ Ŋ(
IMyTerminalBlock Ņ,string ņ,out string Ň);bool ŋ(string ņ);}public interface Ĩ{void Ŭ(string Ç);void Ů(string Ç);void ź(string Ç);void ų
(string Ç);bool ƀ(string Ç,string ž);bool Ƅ(string Ç,string ž);bool Ɔ(string Ç,string ž);}public interface ɛ:W{void ɝ(
string ƈ);}public interface W{string X{get;}void z(Z a);}public class G:R{Dictionary<string,ũ>ή=new Dictionary<string
,ũ>();Dictionary<string,ũ.ƙ>ί=new Dictionary<string,ũ.ƙ>();public int ΰ=>ή.Count;Dictionary<string,W>α=
new Dictionary<string,W>();W β;ħ γ;Ĩ δ;string ε;à ͱ;const string Ĳ="general";const string ζ="surfaces";const string η=
"size";const string θ="scale";public G(A B):base(B){}public void ů(string Ç){ε=Ç;}public string Ϋ()=>ε;public void ļ(ħ ι){γ=ι;
}public void Ľ(Ĩ ι){δ=ι;}public override void V(){ͱ=A.S<à>();Ŀ<κ>();Ŀ<λ>();Ý(new έ(this));Ý(new Ω(this));Ý(new ά(this));Ý
(new Ψ(this));Ý(new Ϊ(this));}public override void Q(){foreach(ũ a in ή.Values){bool μ=a.ƕ is ɚ;bool ν=a.ǜ&&a.Ɨ is ɚ;if(μ
||ν)ŭ(a);}if(A.ξ)A.S<ο>().π($"Active Displays: {ΰ}");}public void Ń(){ρ();ς();}public override void ń(ŀ Ł,object ł){if(Ł
is λ){Ń();return;}if(Ł is κ&&(ł is IMyTextPanel||ł is IMyTextSurfaceProvider)){ρ();ς();}}public void T(W ƥ){α[ƥ.X]=ƥ;}
public bool σ(string Þ)=>α.ContainsKey(Þ);public W τ(string Þ){W ƥ;return α.TryGetValue(Þ,out ƥ)?ƥ:null;}public void Ļ(W ƥ){β=
ƥ;}public ũ Ū(string Þ){if(Þ==null)return null;ũ a;if(ή.TryGetValue(Þ,out a))return a;string υ=Þ;int φ=-1;int ͺ=Þ.
LastIndexOf(':');if(ͺ>0){int χ;if(int.TryParse(Þ.Substring(ͺ+1),out χ)){υ=Þ.Substring(0,ͺ);φ=χ;}}foreach(var ψ in ή){string ω=ψ.Key
;if(string.Equals(ω,υ,StringComparison.OrdinalIgnoreCase))return ψ.Value;if(φ>=0){string ϊ=υ+":"+φ;if(string.Equals(ω,ϊ,
StringComparison.OrdinalIgnoreCase))return ψ.Value;continue;}if(ω.StartsWith(υ+":",StringComparison.OrdinalIgnoreCase))return ψ.Value;}
var ɟ=ͱ?.ɦ<IMyTerminalBlock>(υ);if(ɟ!=null){foreach(var Ņ in ɟ){if(ή.TryGetValue(Ņ.CustomName,out a))return a;
IMyTextSurfaceProvider ϋ=Ņ as IMyTextSurfaceProvider;if(ϋ!=null){if(φ>=0){string ό=Ņ.CustomName+":"+φ;if(ή.TryGetValue(ό,out a))return a;}else
{for(int u=0;u<ϋ.SurfaceCount;u++){string ό=Ņ.CustomName+":"+u;if(ή.TryGetValue(ό,out a))return a;}}}}}return null;}
public void ς(){foreach(ũ a in ή.Values)ŭ(a);}public void ŭ(ũ a){a.ύ();a.ǃ();if(a.ǜ&&a.ƕ==null&&a.Ɨ!=null)ώ(a);else{W ƥ=a.ƕ??β
;if(ƥ!=null){(ƥ as ɛ)?.ɝ(a.Ɩ);ƥ.z(a);}}a.Ǚ();a.ǈ();a.Ċ.Dispose();}void ώ(ũ a){RectangleF Ϗ=a.b;float ϐ=Ϗ.X+Ϗ.Width/2f;
RectangleF ϑ=new RectangleF(Ϗ.X,Ϗ.Y,Ϗ.Width/2f,Ϗ.Height);RectangleF ϒ=new RectangleF(ϐ,Ϗ.Y,Ϗ.Width/2f,Ϗ.Height);if(β!=null){a.ϓ(ϑ)
;β.z(a);a.ϓ(Ϗ);}Vector2 ϔ=new Vector2(ϐ,Ϗ.Y+Ϗ.Height/2f);a.Ċ.Add(ǀ.ǁ("SquareSimple",ϔ,new Vector2(2f,Ϗ.Height),e.o.Ǖ));a.
ϓ(ϒ);(a.Ɨ as ɛ)?.ɝ(a.Ƙ);a.Ɨ.z(a);a.ϓ(Ϗ);}public void Ŭ(string Ç){δ?.Ŭ(Ç);}public void Ů(string Ç){δ?.Ů(Ç);}public void ź(
string Ç){δ?.ź(Ç);}public void ų(string Ç){δ?.ų(Ç);}public string Ɖ(string Ç,string ž,string ƈ=null){ũ a=Ū(Ç);if(a==null)
return"Display '"+Ç+"' not found";W ƥ=τ(ž);if(ƥ!=null){if(a.ǜ)a.Ʀ(ƥ,string.IsNullOrEmpty(ƈ)?null:ƈ);else a.Ǜ(ƥ,string.
IsNullOrEmpty(ƈ)?null:ƈ);ŭ(a);return"";}if(δ!=null&&δ.ƀ(Ç,ž))return"";if(δ!=null&&δ.Ƅ(Ç,ž))return"";bool Ž=a.ū.ż(ž);if(!Ž)return
"View, menu, or path '"+ž+"' not found on display '"+Ç+"'";ŭ(a);return"";}void ρ(){ί.Clear();foreach(var ψ in ή)ί[ψ.Key]=ψ.Value.Ʃ();ή.Clear();
ϕ();ϖ();ί.Clear();}bool Ϙ(string ϗ){return σ(ϗ)||(γ!=null&&γ.ŋ(ϗ));}void ϕ(){ͱ.Đ<IMyTextPanel>()?.ForEach(ϙ=>{MyIni ƴ=ͱ.Ϛ
(ϙ);foreach(ϛ Ϟ in Ϝ.ϝ(ƴ)){if(Ϟ.ϟ!=0)continue;if(!Ϙ(Ϟ.Ϡ))continue;string Ϣ=ϡ(Ϟ);ϣ(ϙ,ϙ,ƴ,-1,Ϣ);break;}});}void ϖ(){var Ϥ=
new List<IMyTerminalBlock>();Ϥ.AddRange(ͱ.Đ<IMyCockpit>());Ϥ.AddRange(ͱ.Đ<IMyProgrammableBlock>());Ϥ.AddRange(ͱ.Đ<
IMySoundBlock>());Ϥ.ForEach(Ņ=>{IMyTextSurfaceProvider ϋ=Ņ as IMyTextSurfaceProvider;if(ϋ==null)return;MyIni ƴ=ͱ.Ϛ(Ņ);foreach(ϛ Ϟ in
Ϝ.ϝ(ƴ)){if(!Ϙ(Ϟ.Ϡ))continue;if(Ϟ.ϟ>=ϋ.SurfaceCount)continue;IMyTextSurface Ð=ϋ.GetSurface(Ϟ.ϟ);ϣ(Ð,Ņ,ƴ,Ϟ.ϟ,ϡ(Ϟ));}});}
string ϡ(ϛ Ϟ){string Þ=Ϟ.Ϡ.Contains(" ")?"\""+Ϟ.Ϡ+"\"":Ϟ.Ϡ;if(string.IsNullOrEmpty(Ϟ.ϥ))return Þ;return Þ+" "+Ϟ.ϥ;}void ϣ(
IMyTextSurface Ð,IMyTerminalBlock Ņ,MyIni ƴ,int Ϧ=-1,string Ϣ=null){Ð.ContentType=ContentType.TEXT_AND_IMAGE;float Ƶ=0f;string ϧ=ƴ.Get
(Ĳ,η).ToString().Trim();if(!string.IsNullOrEmpty(ϧ))float.TryParse(ϧ,out Ƶ);float ƶ=1.2f;string Ϩ=ƴ.Get(Ĳ,θ).ToString().
Trim();if(!string.IsNullOrEmpty(Ϩ))float.TryParse(Ϩ,out ƶ);string ņ="";string Ň=ņ;ĩ Ƃ=null;if(γ!=null)Ƃ=γ.Ŋ(Ņ,ņ,out Ň);if(Ƃ
==null){Ƃ=new ĩ("main");Ň="";}string Ç=Ϧ>=0?Ņ.CustomName+":"+Ϧ:Ņ.CustomName;ũ a=new ũ(Ç,Ð,Ņ,ƴ,Ƃ,Ň,Ƶ,ƶ);ή[Ç]=a;if(!string.
IsNullOrEmpty(Ϣ))ϩ(a,Ϣ);ũ.ƙ Ϫ;if(ί.TryGetValue(Ç,out Ϫ))a.Ƭ(Ϫ);}void ϩ(ũ a,string Ϣ){if(string.IsNullOrEmpty(Ϣ))return;Ϣ=Ϣ.TrimEnd(
';').Trim();if(string.IsNullOrEmpty(Ϣ))return;string ϗ;string ƈ=null;if(Ϣ.StartsWith("\"")){int ϫ=Ϣ.IndexOf('"',1);if(ϫ>1){
ϗ=Ϣ.Substring(1,ϫ-1);string Ϭ=Ϣ.Substring(ϫ+1).Trim();if(!string.IsNullOrEmpty(Ϭ))ƈ=Ϭ;}else{ϗ=Ϣ.Trim('"');}}else{int ϭ=Ϣ.
IndexOf(' ');if(ϭ>0){ϗ=Ϣ.Substring(0,ϭ).Trim();ƈ=Ϣ.Substring(ϭ+1).Trim();}else{ϗ=Ϣ.Trim();}}if(string.IsNullOrEmpty(ϗ))return;W
ƥ=τ(ϗ);if(ƥ!=null){a.Ǜ(ƥ,string.IsNullOrEmpty(ƈ)?null:ƈ);return;}δ?.Ɔ(a.X,ϗ);}}public static class ġ{public static bool Ϯ
(IMyTerminalBlock Ņ)=>(Ņ.GetProperty("ShareInertiaTensor")as ITerminalProperty<bool>)?.GetValue(Ņ)??false;public static
Color Ģ(IMyTerminalBlock Ņ)=>Ϯ(Ņ)?e.o.ɴ:e.o.ϯ;}public static class e{public static class o{public static Color ÿ=new
Color(0,0,0,255);public static Color ϰ=new Color(194,0,0,255);public static Color þ=new Color(166,166,166,
255);public static Color ý=new Color(38,38,38,255);public static Color ϱ=new Color(181,147,54,255);public
static Color ϲ=new Color(128,25,25,255);public static Color r=Color.White;public static Color ʩ=þ;
public static Color q=ϱ;public static Color ǂ=ÿ;public static Color Ǖ=ý;public static Color ǽ=ϲ;public static Color p=ϰ;public static Color Ĝ=ϱ;public static Color s=þ;public static
Color ϯ=new Color(85,85,85,255);public static Color ě=new Color(0,200,80,255);public static Color ɴ=
new Color(220,50,50,255);public static Color ɵ=new Color(0,140,50,180);}public static class Ȳ{public const string
ȳ="Monospace";public const string ϳ="White";}public static class f{public const float g=8f;public const float ʦ=12f;
public const float h=8f;public const float ϴ=22f;public const float ɡ=32f;public const float ȯ=3f;public const float Ȱ=8f;
public const float ɒ=4f;public const float ɓ=5f;public const float ȸ=1.5f;}}public static class ú{public static Color ü(Color
w,byte ϵ){return new Color(w.R,w.G,w.B,ϵ);}public static void û(Z a,float Ϸ,float ϸ,float ë,float ì,Color w){if(ë>0.5f&&ì
>0.5f){a.Ċ.Add(ǀ.ǁ("SquareSimple",new Vector2(Ϸ,ϸ),new Vector2(ë,ì),w));}}public static MySprite ċ(string Ǌ,Vector2 ǋ,
float ǌ,Color w,TextAlignment Ϲ=TextAlignment.LEFT,string Ϻ="Monospace"){return new MySprite{Type=SpriteType.TEXT,Data=Ǌ,
Position=ǋ,Color=w,FontId=Ϻ,RotationOrScale=ǌ,Alignment=Ϲ};}public static MySprite Ǐ(Vector2 ê,float ë,Color w,float ǎ=2f){
return ǀ.ǁ("SquareSimple",ê,new Vector2(ë,ǎ),w);}}public class ϻ:ª{A A;public override string X=>"boot";public ϻ(A B)
{A=B;}public override string Ò(Á Â){A.ϼ=true;return"Rebooting...";}}public abstract class ª:Ͻ{public abstract string X{
get;}public abstract string Ò(Á Â);public string Ͼ()=>X;protected float Є(string Ͽ,Dictionary<string,string>Ѐ){float Ё;if(!
float.TryParse(Ͽ,out Ё))throw new ArgumentException("Invalid numerical value provided.");bool Ђ=Ѐ.ContainsKey("add");bool Ѓ=Ѐ
.ContainsKey("sub");float Ï=0;if(Ђ)Ï=Ё;else if(Ѓ)Ï=-Ё;return Ï;}protected float Ј(float Ѕ,int І,bool Ї){if(Ї&&І>1)return
Ѕ/І;return Ѕ;}protected bool Љ(Dictionary<string,string>Ѐ){return Ѐ.ContainsKey("share");}}public interface Ͻ{string Ͼ();
string Ò(Á Â);}public class Ѝ:ª{A A;public override string X=>"purge";bool Њ=false;List<string>Ћ=new List<
string>();List<string>Ќ=new List<string>();public Ѝ(A B){A=B;}void Б(string º){switch(º){case"almanac":A.S<Ў>().Џ();Ќ
.Add(º);break;case"storage":A.S<А>().Џ();Ќ.Add(º);break;default:break;}}public override string Ò(Á Â){Ћ.Clear();Ќ.Clear()
;foreach(var Г in Â.В){string œ=Г.Key;string Ŕ=Г.Value;if(œ=="force"&&(Ŕ=="true"||Ŕ=="1"))Њ=true;}if(!Њ)return
"Run command with --force to purge";if(Â.Ã.Count==0)return Ä.Å.Д;else{string Е=Â.Ã[0];List<string>Ж=Е.Split(',').ToList();if(Ж.Contains("*")){Ћ.Add(
"almanac");Ћ.Add("storage");}if(Ж.Contains("storage"))Ћ.Add("storage");if(Ж.Contains("almanac"))Ћ.Add("almanac");Ћ.ForEach(º=>Б(º
));return Ќ.Count==0?"No modules purged":$"Purged {Ќ.Count} modules: {string.Join(", ",Ќ)}";}}}public class И:ª{A A;Random З=new Random();public override string X=>"rename";public И(A B){A=B;}public override string Ò(Á Â){if
(Â.Ã.Count<1)return Ä.Å.Д;string Й=Â.Ã[0];bool М=Á.К(Â.Л("unique"));if(М)Й=$"{Й}-{З.Next(10000,99999)}";A.Н.CustomName=Й;
A.X=Й;var ƴ=A.S<О>();ƴ.П.Set("general","name",Й);A.ō.CustomData=ƴ.П.ToString();var С=A.S<Р>();С.Т();С.У();return
$"Grid name set to: {Й}";}}public class Ы:Ф{public Dictionary<IMyTerminalBlock,Х>Ц{get;}public struct Х{public Func<IMyTerminalBlock,bool>Ч;
public Action<IMyTerminalBlock>Ш;public Х(Func<IMyTerminalBlock,bool>Щ,Action<IMyTerminalBlock>Ъ){Ч=Щ;Ш=Ъ;}}public Ы(A B):base
(B){Ц=new Dictionary<IMyTerminalBlock,Х>();}public override void Q(){var Ь=new List<IMyTerminalBlock>();foreach(var Ϟ in
Ц){IMyTerminalBlock Ņ=Ϟ.Key;Х Э=Ϟ.Value;if(Э.Ч(Ņ)){Э.Ш?.Invoke(Ņ);Ь.Add(Ņ);}}Ь.ForEach(Ņ=>Ю(Ņ));}public void б(
IMyTerminalBlock Ņ,Func<IMyTerminalBlock,bool>Я,Action<IMyTerminalBlock>а){if(!Ц.ContainsKey(Ņ))Ц[Ņ]=new Х(Я,а);}public void Ю(
IMyTerminalBlock Ņ){if(Ц.ContainsKey(Ņ))Ц.Remove(Ņ);}}public class Ў:Ф{const double в=300;public List<г>д=new List<г>();public Ў(A B):
base(B){A=B;}public override void V(){е();var į=A.S<į>();į.ж(з,1);į.ж(и,30);}public void з(){г л=й($"{A.к}");MatrixD н=A.м()
;Vector3D ǋ=н.Translation;if(л==null){л=new г($"{A.к}","grid",ǋ,A.Н.Speed);}Vector3D?о=(Vector3D?)н.Forward;Vector3D?п=(
Vector3D?)н.Up;л.р(ǋ,A.Н.Speed,A.X,A.с.Radius,о,п);т(л);}void е(){string ф=A.S<А>().у("almanac")??"";if(ф=="")return;Dictionary<
string,object>х;try{х=ц.ч(ф);}catch{return;}foreach(var л in х){try{Dictionary<string,object>ш=(Dictionary<string,object>)л.
Value;г ъ=г.щ(ш);if(ы(ъ))д.Add(ъ);}catch{}}}bool ы(г л){if(л==null||string.IsNullOrEmpty(л.к))return false;if(л.ь=="waypoint"
)return true;if(л.ь=="grid"&&л.э==0)return false;if(ю(л))return false;return true;}bool ю(г л){if(л.ь=="waypoint")return
false;double ѐ=(DateTime.Now-л.я).TotalSeconds;return ѐ>в;}void и(){int ё=д.RemoveAll(л=>л.к!=$"{A.к}"&&ю(л));if(ё>0)N();}
void N(){var х=new Dictionary<string,object>();foreach(var л in д)х[л.к]=л;A.S<А>().ђ("almanac",ц.ѓ(х));}public void Џ(){д.
Clear();N();}public г ѣ(string є,long ѕ,string Þ,Vector3D ǋ,float і,HashSet<string>ї,bool ј,Vector3D?о=null,Vector3D?п=null){
г л;г љ=й(є);if(љ!=null){љ.р(ǋ,і,Þ,0,о,п);љ.э=ѕ;љ.њ.UnionWith(ї);л=љ;}else{л=new г(є,"grid",ǋ,і){э=ѕ,ћ=Þ,њ=ї};if(о.
HasValue)л.ќ=о.Value;if(п.HasValue)л.ѝ=п.Value;if(ј)л.ў=г.џ.Ѡ;}if(!ї.Contains("*")&&л.ў==г.џ.ѡ)л.ў=г.џ.Ѣ;т(л);return л;}public г
й(string Ѥ){return д.Find(л=>л.к==Ѥ||л.ћ==Ѥ);}public List<г>Ѧ(string ѥ){return д.FindAll(л=>л.ь==ѥ);}public void т(г л){г
љ=д.Find(Ά=>Ά.к==л.к);if(љ==null)д.Add(л);else{if(л.я>љ.я){д.Remove(љ);д.Add(л);}}N();}}public class г:ѧ{public static
Dictionary<string,string>Ѩ=new Dictionary<string,string>{{"grid","grid"},{"waypoint","waypoint"}};public enum џ{Ѡ,Ѣ,ѡ,ѩ
}public string к{get;}public long э{get;set;}public DateTime я;public Vector3D Ѫ;public Vector3D ќ=Vector3D.Forward;
public Vector3D ѝ=Vector3D.Up;public float ѫ;public double Ѭ;public string ь{get;set;}public string ћ{get;set;}public џ ў{get;
set;}public HashSet<string>њ=new HashSet<string>();public г(string ѭ,string Ѯ,Vector3D ǋ,float і=0){к=ѭ;я=DateTime.Now;Ѫ=ǋ;
ѫ=і;ь=Ѯ;ў=џ.ѡ;}public void р(Vector3D ǋ,float і,string Ç=null,double ѯ=0,Vector3D?о=null,Vector3D?п=null){Ѫ=ǋ;ѫ=і;я=
DateTime.Now;ћ=Ç??ћ;Ѭ=ѯ>0?ѯ:Ѭ;ќ=о??ќ;ѝ=п??ѝ;}public bool Ѱ()=>ў==џ.Ѣ;public bool ѱ()=>ў==џ.ѩ;public bool Ѳ()=>ў==џ.ѡ;public bool
ѳ()=>ў==џ.Ѡ;public string ѵ(){Dictionary<string,object>Ѵ=new Dictionary<string,object>{{"Id",$"{к}"},{"UnicastId",$"{э}"}
,{"DisplayName",$"{ћ}"},{"UpdatedAt",$"{я.Ticks}"},{"pos",$"{Ѫ}"},{"LastKnownSpeed",$"{ѫ}"},{"EntityType",Ѩ[ь]},{
"SafeRadius",$"{Ѭ}"},{"fx",$"{ќ.X}"},{"fy",$"{ќ.Y}"},{"fz",$"{ќ.Z}"},{"ux",$"{ѝ.X}"},{"uy",$"{ѝ.Y}"},{"uz",$"{ѝ.Z}"}};return ц.ѓ(Ѵ);
}public static г щ(Dictionary<string,object>Ѷ){string[]ѷ=$"{Ѷ["pos"]}".Split(' ');double Ǵ=double.Parse(ѷ[0].Substring(2)
);double Ǎ=double.Parse(ѷ[1].Substring(2));double Ѹ=double.Parse(ѷ[2].Substring(2));string Ѯ=$"{Ѷ["EntityType"]}";г ѹ=new
г($"{Ѷ["Id"]}",Ѩ[Ѯ],new Vector3D(Ǵ,Ǎ,Ѹ),0);object Ѻ;long ѻ;if(Ѷ.TryGetValue("UnicastId",out Ѻ)&&Ѻ!=null&&long.TryParse(Ѻ.
ToString(),out ѻ)){ѹ.э=ѻ;}object Ѽ;if(Ѷ.TryGetValue("DisplayName",out Ѽ)&&Ѽ!=null&&!string.IsNullOrEmpty(Ѽ.ToString())){ѹ.ћ=Ѽ.
ToString();}double ѯ;object ѽ;if(Ѷ.TryGetValue("SafeRadius",out ѽ)&&ѽ!=null&&double.TryParse(ѽ.ToString(),out ѯ)){ѹ.Ѭ=ѯ;}double
Ѿ,ѿ,Ҁ,ҁ,Ҋ,ҋ;object Ҍ,ҍ,Ҏ,ҏ,Ґ,ґ;if(Ѷ.TryGetValue("fx",out Ҍ)&&double.TryParse($"{Ҍ}",out Ѿ)&&Ѷ.TryGetValue("fy",out ҍ)&&
double.TryParse($"{ҍ}",out ѿ)&&Ѷ.TryGetValue("fz",out Ҏ)&&double.TryParse($"{Ҏ}",out Ҁ)){ѹ.ќ=new Vector3D(Ѿ,ѿ,Ҁ);}if(Ѷ.
TryGetValue("ux",out ҏ)&&double.TryParse($"{ҏ}",out ҁ)&&Ѷ.TryGetValue("uy",out Ґ)&&double.TryParse($"{Ґ}",out Ҋ)&&Ѷ.TryGetValue(
"uz",out ґ)&&double.TryParse($"{ґ}",out ҋ)){ѹ.ѝ=new Vector3D(ҁ,Ҋ,ҋ);}return ѹ;}public long Ғ()=>long.Parse(к);}public
abstract class ҙ:ғ,Ҕ{public A A;Dictionary<IMyTerminalBlock,Func<IMyTerminalBlock,object>>ҕ=new Dictionary<
IMyTerminalBlock,Func<IMyTerminalBlock,object>>();Dictionary<IMyTerminalBlock,Action<IMyTerminalBlock,object>>Җ=new
Dictionary<IMyTerminalBlock,Action<IMyTerminalBlock,object>>();public Dictionary<long,object>җ=new Dictionary<long,object
>();public List<Ͻ>Ҙ=new List<Ͻ>();public ҙ(A B){A=B;}public virtual void Q(){}public virtual void V(){}public virtual
IEnumerator<double>Қ(){V();yield return 0.0;}public virtual void ń(ŀ Ł,object ł){}public virtual string қ()=>$"{GetType()}";public
virtual void Ý(Ͻ Â){Ҙ.Add(Â);}protected void Ҡ<ə>(Func<ə,object>Ҝ,Action<IMyTerminalBlock,object>ҝ)where ə:class,
IMyTerminalBlock{à à=A.S<à>();foreach(var Ņ in à.Đ<ə>()){ҕ[Ņ]=(Ҟ)=>Ҝ(Ҟ as ə);Җ[Ņ]=ҝ;à.ҟ(Ņ,this);җ[Ņ.EntityId]=Ҝ(Ņ);}}public object ҡ(
IMyTerminalBlock Ņ){return ҕ.ContainsKey(Ņ)?ҕ[Ņ](Ņ):null;}public bool Ҥ(IMyTerminalBlock Ņ,object Ң){if(!ҕ.ContainsKey(Ņ))return false;
object ң=ҕ[Ņ](Ņ);return Ң==null||!Equals(Ң,ң);}public void Ҧ(IMyTerminalBlock Ņ){if(!ҕ.ContainsKey(Ņ))return;long ҥ=Ņ.EntityId
;object ң=ҕ[Ņ](Ņ);if(!җ.ContainsKey(ҥ)||!Equals(җ[ҥ],ң)){if(Җ.ContainsKey(Ņ))Җ[Ņ](Ņ,ң);җ[ҥ]=ң;}}public List<ə>ɦ<ə>(string
Þ)where ə:class,IMyTerminalBlock{return A.S<à>().ɦ<ə>(Þ);}public ə S<ə>()where ə:class,ғ{return A.S<ə>();}public void Ŀ<ҧ
>()where ҧ:ŀ{A.S<Ҩ>().Ŀ<ҧ>(this);}public void ҩ(ŀ Ł,object ł){A.S<Ҩ>().ҩ(Ł,ł);}public void ҩ<ҧ>(object ł=null)where ҧ:ŀ,
new(){ҩ(new ҧ(),ł);}public List<Ͻ>Ҫ()=>Ҙ;public void Ұ(string ȏ,Func<ҫ,Ҭ>ҭ){S<Р>().Ү.ү(ȏ,ҭ);}}public abstract class Ф:ҙ,ұ{
public Ф(A B):base(B){}}public abstract class R:ҙ,E{public R(A B):base(B){}}public class à:Ф{į į;О О;Ҩ Ҩ;public HashSet<long>Ҳ
=new HashSet<long>();List<IMyBlockGroup>ҳ=new List<IMyBlockGroup>();public List<IMyTerminalBlock>Ҵ=new List<
IMyTerminalBlock>();public Dictionary<IMyTerminalBlock,MyIni>ҵ=new Dictionary<IMyTerminalBlock,MyIni>();public Dictionary<string,HashSet<IMyTerminalBlock>>Ҷ=new Dictionary<string,HashSet<IMyTerminalBlock>>();Dictionary<
IMyTerminalBlock,Dictionary<string,string>>ҷ=new Dictionary<IMyTerminalBlock,Dictionary<string,string>>();Dictionary<long,
object>Ҹ=new Dictionary<long,object>();Dictionary<IMyTerminalBlock,Ҕ>ҹ=new Dictionary<IMyTerminalBlock,Ҕ>();int Һ=0;
const int һ=50;const string Ҽ="general";const string ҽ="tags";const string Ҿ="hooks";bool ҿ=false;public à(A B):base(B){A=B;}
public override IEnumerator<double>Қ(){V();foreach(var ï in Ӏ(A.Н))yield return ï;Ӂ();į.ӂ(Ӄ());yield break;}public override
void V(){О=A.S<О>();Ҩ=A.S<Ҩ>();į=A.S<į>();Ҩ.Ŀ<ӄ>(this);Ҩ.Ŀ<Ӆ>(this);Ҩ.Ŀ<ӆ>(this);Ҩ.Ŀ<Ӈ>(this);Ҩ.Ŀ<ľ>(this);Ҩ.Ŀ<ӈ>(this);Ҩ.Ŀ<
Ӊ>(this);}IEnumerable<double>Ӄ(){ӊ();į.ӂ(Ӄ(),1);yield return 0;}public override void Q(){Ӌ();}void Ӌ(){var ӌ=ҹ.Keys.
ToList();int Ӎ=ӌ.Count;var ӎ=ӌ.Skip(Һ).Take(һ).ToList();foreach(var Ņ in ӎ){if(!ҹ.ContainsKey(Ņ))continue;Ҕ ӏ=ҹ[Ņ];object Ң=Ҹ.
ContainsKey(Ņ.EntityId)?Ҹ[Ņ.EntityId]:null;object ң=ӏ.ҡ(Ņ);if(ӏ.Ҥ(Ņ,Ң)){ӏ.Ҧ(Ņ);Ҹ[Ņ.EntityId]=ң;}}Һ+=һ;if(Һ>=Ӎ)Һ=0;}public override
void ń(ŀ Ł,object ł){if(Ł is ӄ||Ł is Ӆ)Ӑ();if(Ł is ӈ||Ł is Ӊ)Ӑ();if(Ł is ӆ||Ł is Ӈ)ӑ();if(Ł is ľ)Ӂ();}public
IMyTerminalBlock ӕ(IMyTerminalBlock Ņ,string Ӓ){MyIni ӓ=Ϛ(Ņ);string Ӕ=$"{ӓ.Get(Ҽ,ҽ)}";if(Ӕ=="")Ӕ=Ӓ;else if(!Ӕ.Contains(Ӓ))Ӕ+=$",{Ӓ}";ӓ.
Set(Ҽ,ҽ,Ӕ);Ņ.CustomData=ӓ.ToString();if(!Ҷ.ContainsKey(Ӓ))Ҷ[Ӓ]=new HashSet<IMyTerminalBlock>();Ҷ[Ӓ].Add(Ņ);return Ņ;}public
void ҟ(IMyTerminalBlock Ņ,Ҕ ӏ){ҹ[Ņ]=ӏ;Ҹ[Ņ.EntityId]=ӏ.ҡ(Ņ);}public List<ə>Đ<ə>(Func<ə,bool>Ӗ=null)where ə:class,
IMyTerminalBlock{List<ə>ӗ=Ҵ.OfType<ə>().Where(Ņ=>Ӗ==null||Ӗ(Ņ)).ToList();return ӗ;}bool ә(IMyTerminalBlock Ņ,MyIni Ә){return ҵ.
ContainsKey(Ņ)&&Ә.ToString()!=ҵ[Ņ].ToString();}void ӊ(){foreach(var Ņ in Ҵ){MyIni ӓ=new MyIni();MyIniParseResult Ρ;if(!ӓ.TryParse(Ņ
.CustomData,out Ρ))continue;if(A.Ӛ==A.ӛ.Ӝ)ҵ[Ņ]=ӓ;else if(A.Ӛ==A.ӛ.ӝ&&ә(Ņ,ӓ)){ҵ[Ņ]=ӓ;ҩ<κ>(Ņ);if(Ņ.EntityId==A.к)ҩ<ľ>(Ņ);A.
Ӟ($"Config changed: {Ņ.CustomName}",false);}if(ӓ.ToString()=="")ӟ(Ņ,ӓ);if(ӓ.ContainsSection(Ҽ))Ӡ(Ņ,ӓ);if(ӓ.
ContainsSection(Ҿ))ӡ(Ņ,ӓ);}}void ӟ(IMyTerminalBlock Ņ,MyIni ӓ){foreach(var œ in О.Ӣ.Keys){string[]ӣ=œ.Split('.');ӓ.Set(ӣ[0],ӣ[1],О.Ӣ[œ]
);}Ņ.CustomData=ӓ.ToString();}void Ӡ(IMyTerminalBlock Ņ,MyIni ӓ){string Ӕ=$"{ӓ.Get(Ҽ,ҽ)}";if(Ӕ=="")return;foreach(var Ӓ
in Ӕ.Split(',')){string Ӥ=Ӓ.Trim();if(!Ҷ.ContainsKey(Ӥ))Ҷ[Ӥ]=new HashSet<IMyTerminalBlock>();Ҷ[Ӥ].Add(Ņ);}}void ӡ(
IMyTerminalBlock Ņ,MyIni ӓ){var ӥ=new Dictionary<string,string>();List<MyIniKey>ő=new List<MyIniKey>();ӓ.GetKeys(Ҿ,ő);foreach(var Ӧ in ő
){string ӧ=$"{ӓ.Get(Ҿ,Ӧ.Name)}";string ө=Ө(Ņ,ӧ);ӥ[Ӧ.Name]=ө;}ҷ[Ņ]=ӥ;}string Ө(IMyTerminalBlock Ņ,string ӧ){StringBuilder
Ӫ=new StringBuilder();int ӫ=0;while(ӫ<ӧ.Length){int Ӭ=ӧ.IndexOf("this",ӫ);if(Ӭ==-1){Ӫ.Append(ӧ.Substring(ӫ));break;}if(Ӭ>
0&&(ӧ[Ӭ-1]==' '||ӧ[Ӭ-1]=='=')&&(Ӭ+4==ӧ.Length||ӧ[Ӭ+4]==' '||ӧ[Ӭ+4]==';')){Ӫ.Append(ӧ.Substring(ӫ,Ӭ-ӫ));Ӫ.Append(
$"\"{Ņ.CustomName}\"");}else{Ӫ.Append(ӧ.Substring(ӫ,Ӭ-ӫ+4));}ӫ=Ӭ+4;}return$"{Ӫ}";}void Ӂ(){MyIni Ӯ=О.ӭ;List<MyIniKey>ő=new List<MyIniKey>();Ӯ
.GetKeys(Ҿ,ő);foreach(var œ in ő){if(œ.Name.Contains(".")){string[]ӣ=œ.Name.Split('.');string ɨ=ӣ[0].Trim('\"');string Ӧ=
ӣ[1];foreach(var Ņ in ɦ<IMyTerminalBlock>(ɨ)){if(!ҷ.ContainsKey(Ņ))ҷ[Ņ]=new Dictionary<string,string>();ҷ[Ņ][Ӧ]=Ӯ.Get(Ҿ,œ
.Name).ToString();}}}}public void Ӱ(IMyTerminalBlock Ņ,string Ӧ){if(ҷ.ContainsKey(Ņ)&&ҷ[Ņ].ContainsKey(Ӧ)){string ӯ=ҷ[Ņ][
Ӧ];A.S<Ä>().Ÿ(ӯ);}}public MyIni Ϛ(IMyTerminalBlock Ņ){return ҵ.ContainsKey(Ņ)?ҵ[Ņ]:new MyIni();}public new List<ə>ɦ<ə>(
string Þ)where ə:class,IMyTerminalBlock{List<ə>ɟ=new List<ə>();List<IMyBlockGroup>Ӳ=ӱ(Þ);if(Ӳ.Count>0){foreach(var Ţ in Ӳ){
List<ə>ӳ=new List<ə>();Ţ.GetBlocksOfType(ӳ);ɟ.AddRange(ӳ.Where(Ņ=>Ҳ.Contains(Ņ.CubeGrid.EntityId)));}}else if(Þ.StartsWith(
"#")){if(Ҷ.ContainsKey(Þ.Substring(1))){Ҷ[Þ.Substring(1)]?.ToList().ForEach(Ņ=>{if(Ņ is ə&&Ҳ.Contains(Ņ.CubeGrid.EntityId))
ɟ.Add(Ņ as ə);});}else{A.Ӟ($"Tag not found: {Þ}");return ɟ;}}else{ə Ņ=Ӵ(Þ)as ə;if(Ņ!=null&&Ҳ.Contains(Ņ.CubeGrid.EntityId
))ɟ.Add(Ņ);}return ɟ;}public void Ӑ(){ҳ.Clear();A.ӵ.GetBlockGroups(ҳ);}List<IMyBlockGroup>ӱ(string Ӷ){return ҳ.Where(Ţ=>
string.Equals(Ţ.Name,Ӷ,StringComparison.OrdinalIgnoreCase)).ToList();}IMyTerminalBlock Ӵ(string Þ)=>Ҵ.FirstOrDefault(Ǵ=>Ǵ.
DisplayNameText==Þ);void Ӹ(){A.ӷ=Ҵ.OfType<IMyShipController>().OrderByDescending(ͽ=>ͽ.IsMainCockpit).FirstOrDefault();}List<
IMyMechanicalConnectionBlock>ӹ=new List<IMyMechanicalConnectionBlock>();Queue<IMyCubeGrid>Ӻ=new Queue<IMyCubeGrid>();HashSet<long>
ӻ=new HashSet<long>();Dictionary<long,HashSet<long>>Ӽ=new Dictionary<long,HashSet<long>>();const int ӽ=40;const int Ӿ=500
;IEnumerable<double>Ӏ(IMyCubeGrid Ƿ){ӿ();Ҳ.Clear();foreach(var ï in Ԁ(Ƿ,Ҳ))yield return ï;foreach(var ï in ԁ())yield
return ï;}void ӿ(){ӹ.Clear();Ӽ.Clear();A.ӵ.GetBlocksOfType(ӹ);for(int u=0;u<ӹ.Count;u++){var Ԃ=ӹ[u];var ԃ=Ԃ.CubeGrid;var Ҟ=Ԃ.
TopGrid;if(ԃ==null||Ҟ==null)continue;long Ԅ=ԃ.EntityId,ԅ=Ҟ.EntityId;HashSet<long>Ԇ;if(!Ӽ.TryGetValue(Ԅ,out Ԇ))Ӽ[Ԅ]=Ԇ=new
HashSet<long>();Ԇ.Add(ԅ);HashSet<long>ԇ;if(!Ӽ.TryGetValue(ԅ,out ԇ))Ӽ[ԅ]=ԇ=new HashSet<long>();ԇ.Add(Ԅ);}}IMyCubeGrid ԉ(long Ԉ){
for(int u=0;u<ӹ.Count;u++){var Ҟ=ӹ[u];if(Ҟ.CubeGrid!=null&&Ҟ.CubeGrid.EntityId==Ԉ)return Ҟ.CubeGrid;if(Ҟ.TopGrid!=null&&Ҟ.
TopGrid.EntityId==Ԉ)return Ҟ.TopGrid;}return null;}void ԋ(IMyTerminalBlock Ņ){var Ō=new MyIni();MyIniParseResult Ԋ;if(!Ō.
TryParse(Ņ.CustomData,out Ԋ))return;ҵ[Ņ]=Ō;if(Ō.ToString().Length==0)ӟ(Ņ,Ō);if(Ō.ContainsSection(Ҽ))Ӡ(Ņ,Ō);if(Ō.ContainsSection(
Ҿ))ӡ(Ņ,Ō);}IEnumerable<double>Ԑ(HashSet<long>Ԍ){var ԍ=new List<IMyTerminalBlock>();A.ӵ.GetBlocks(ԍ);var Ԏ=ԍ.Where(Ҟ=>Ԍ.
Contains(Ҟ.CubeGrid.EntityId)).ToList();int Ӭ=0;while(Ӭ<Ԏ.Count){int ԏ=Math.Min(Ӿ,Ԏ.Count-Ӭ);for(int u=0;u<ԏ;u++){var Ņ=Ԏ[Ӭ+u];Ҵ
.Add(Ņ);ԋ(Ņ);}Ӭ+=ԏ;yield return 0;}}void Ԓ(HashSet<long>Ԍ){Ҵ.RemoveAll(Ҟ=>{if(!Ԍ.Contains(Ҟ.CubeGrid.EntityId))return
false;ҵ.Remove(Ҟ);ҹ.Remove(Ҟ);Ҹ.Remove(Ҟ.EntityId);ҷ.Remove(Ҟ);foreach(var ԑ in Ҷ.Values)ԑ.Remove(Ҟ);return true;});}
IEnumerable<double>Ԁ(IMyCubeGrid Ƿ,HashSet<long>Ρ){Ӻ.Clear();ӻ.Clear();Ӻ.Enqueue(Ƿ);while(Ӻ.Count>0){int ԓ=0;while(Ӻ.Count>0&&ԓ<ӽ){
var Ԕ=Ӻ.Dequeue();long ԕ=Ԕ.EntityId;if(ӻ.Contains(ԕ)){ԓ++;continue;}ӻ.Add(ԕ);Ρ.Add(ԕ);HashSet<long>Ԗ;if(Ӽ.TryGetValue(ԕ,out
Ԗ)){foreach(long ԗ in Ԗ){var Ԙ=ԉ(ԗ);if(Ԙ!=null&&!ӻ.Contains(ԗ))Ӻ.Enqueue(Ԙ);}}ԓ++;}yield return 0;}}IEnumerable<double>ԁ(
){Ҷ.Clear();ҷ.Clear();ҵ.Clear();Ҵ.Clear();var Ο=new List<IMyTerminalBlock>();A.ӵ.GetBlocks(Ο);for(int u=0;u<Ο.Count;u++){
var ԙ=Ο[u];if(Ҳ.Contains(ԙ.CubeGrid.EntityId))Ҵ.Add(ԙ);}Ӹ();int Ӭ=0;while(Ӭ<Ҵ.Count){int ԏ=Math.Min(Ӿ,Ҵ.Count-Ӭ);for(int u=
0;u<ԏ;u++)ԋ(Ҵ[Ӭ+u]);Ӭ+=ԏ;yield return 0;}Ӑ();yield return 0;}public void ӑ(){if(ҿ)return;ҿ=true;į.ӂ(Ԛ());}public void ԝ(
IMyCubeGrid ԛ){if(ԛ==null||ҿ)return;if(Ҳ.Contains(ԛ.EntityId))return;ҿ=true;į.ӂ(Ԝ(ԛ));}public void ԟ(){if(ҿ)return;ҿ=true;į.ӂ(Ԟ());
}IEnumerable<double>Ԝ(IMyCubeGrid Ԡ){ӿ();Ӻ.Clear();ӻ.Clear();var ԡ=new HashSet<long>();Ӻ.Enqueue(Ԡ);while(Ӻ.Count>0){int
ԓ=0;while(Ӻ.Count>0&&ԓ<ӽ){var Ԕ=Ӻ.Dequeue();long ԕ=Ԕ.EntityId;if(ӻ.Contains(ԕ)){ԓ++;continue;}ӻ.Add(ԕ);if(!Ҳ.Contains(ԕ))
ԡ.Add(ԕ);HashSet<long>Ԗ;if(Ӽ.TryGetValue(ԕ,out Ԗ)){foreach(long ԗ in Ԗ){if(Ҳ.Contains(ԗ))continue;var Ԙ=ԉ(ԗ);if(Ԙ!=null&&
!ӻ.Contains(ԗ))Ӻ.Enqueue(Ԙ);}}ԓ++;}yield return 0;}if(ԡ.Count>0){foreach(var ԕ in ԡ)Ҳ.Add(ԕ);foreach(var ï in Ԑ(ԡ))yield
return ï;}Ӑ();ҿ=false;yield return 0;}IEnumerable<double>Ԟ(){ӿ();var Ԣ=new HashSet<long>();foreach(var ï in Ԁ(A.Н,Ԣ))yield
return ï;var ԣ=new HashSet<long>(Ҳ);ԣ.ExceptWith(Ԣ);Ҳ=Ԣ;if(ԣ.Count>0)Ԓ(ԣ);Ӑ();ҿ=false;yield return 0;}IEnumerable<double>Ԛ(){ӿ
();var Ԥ=new HashSet<long>();foreach(var ï in Ԁ(A.Н,Ԥ))yield return ï;var ԥ=new HashSet<long>(Ԥ);ԥ.ExceptWith(Ҳ);var ԣ=
new HashSet<long>(Ҳ);ԣ.ExceptWith(Ԥ);Ҳ=Ԥ;if(ԣ.Count>0)Ԓ(ԣ);if(ԥ.Count>0){foreach(var ï in Ԑ(ԥ))yield return ï;}Ӑ();ҿ=false;
yield return 0;}}public class κ:ŀ{}public interface Ҕ{object ҡ(IMyTerminalBlock Ņ);bool Ҥ(IMyTerminalBlock Ņ,object Ң);void Ҧ
(IMyTerminalBlock Ņ);}public class į:Ф{class Բ{public double Ԧ;public double ԧ;public Action Ա;}List<Բ>Գ=new
List<Բ>();List<Բ>Դ=new List<Բ>();class Է{public IEnumerator<double>Ե;public double Զ;}List<Է>Ը=new List<Է>
();MyGridProgram Թ;bool Ժ=true;public į(A B):base(B){Թ=B.Թ;Թ.Runtime.UpdateFrequency=UpdateFrequency.Update10;}
public void Ի(){Ը.Clear();Գ.Clear();Դ.Clear();}public override void V(){ж(Լ,1);}public void ж(Action Խ,double Ծ=0){Գ.Add(new Բ
{Ա=Խ,Ԧ=Ծ,ԧ=Ծ});}public void Կ()=>Գ.Clear();public void Ձ(Action Խ,double Հ){Դ.Add(new Բ{Ա=Խ,Ԧ=Հ,ԧ=Հ});}public void ӂ(
IEnumerable<double>Ղ,double Հ=0){Ը.Add(new Է{Ե=Ղ.GetEnumerator(),Զ=Հ});}public override void Q(){double Ճ=Թ.Runtime.
TimeSinceLastRun.TotalSeconds;foreach(var Խ in Գ){Խ.ԧ-=Ճ;if(Խ.ԧ<=0){Խ.Ա.Invoke();Խ.ԧ=Խ.Ԧ;}}for(int u=Դ.Count-1;u>=0;u--){var Խ=Դ[u];Խ.ԧ
-=Ճ;if(Խ.ԧ<=0){Խ.Ա.Invoke();Դ.RemoveAt(u);}}for(int u=Ը.Count-1;u>=0;u--){var Մ=Ը[u];Մ.Զ-=Ճ;if(Մ.Զ<=0){if(Մ.Ե.MoveNext())Մ
.Զ=Մ.Ե.Current;else{Մ.Ե.Dispose();Ը.RemoveAt(u);}}}}public int Յ{get{return Դ.Count;}}public int Ն{get{return Ը.Count;}}
void Լ()=>Ժ=!Ժ;public string Շ()=>Ժ?"/":"\\";}public static class Ì{public const string Í="Block not found: {0}";public
const string Ո="Invalid argument for block: {0}";public const string Չ="Invalid command option: {0}";public const string Ñ=
"Block updated: {0} -> {1}";public const string Պ="Block resetting: {0}";public const string Ջ="Block moving: {0}";public const string Ռ=
"Block started: {0}";public const string Ս="Block stopped: {0}";public const string Վ="Block locked: {0}";public const string Տ=
"Block unlocked: {0}";public const string Ր="Block open: {0}";public const string Ց="Block charging: {0}";public const string Ւ=
"Block discharging: {0}";public const string Փ="Block auto: {0}";public const string Ք="Block on: {0}";public const string Օ="Block off: {0}";
public const string Ֆ="Block closed: {0}";public const string ՙ="Block toggled: {0}";public const string ա=
"Block stockpiling: {0}";public const string բ="Block sharing: {0}";public const string գ="Block action: {0} -> {1}";public const string դ=
"Block: {0} -> {1}";}public class Ä:Ф{public static class Å{public const string ե="Command not found: {0}";public const string Д=
"No arguments provided";public const string Æ="Invalid command format.";}į į;զ զ;Р է;public List<Ͻ>ը=new List<Ͻ>();public Dictionary<long,HashSet<string>>թ=new Dictionary<long,HashSet<string>>();public Dictionary<long,HashSet<string>>ժ=new
Dictionary<long,HashSet<string>>();public Ä(A B):base(B){}public override void V(){į=A.S<į>();զ=A.S<զ>();է=A.S<Р>();թ.Clear();ժ.
Clear();Ý(new ի(this));Ұ("command",լ=>խ(լ));Ұ("localcmd",լ=>ծ(լ));}Ҭ խ(ҫ լ){if(!է.կ)return null;string ձ=լ.հ("Command").Trim(
);if(string.IsNullOrEmpty(ձ))return է.ղ(լ,Ҭ.ճ.մ);յ("REQ",լ.ն("OriginName"),ձ);var շ=new Á(ձ);long չ=ո(շ.X);if(չ==0)չ=պ(շ.
X);if(չ!=0){է.ջ(չ,ձ);return է.ղ(լ,Ҭ.ճ.ռ);}return ս(լ,ձ);}Ҭ ծ(ҫ լ){string Â=լ.հ("Command").Trim();if(string.IsNullOrEmpty(
Â))return է.ղ(լ,Ҭ.ճ.մ);յ("CREQ",լ.ն("OriginName"),Â);return ս(լ,Â);}Ҭ ս(ҫ լ,string Â){bool Ž=Ÿ(Â);var ę=Ž?Ҭ.ճ.ռ:Ҭ.ճ.մ;
return է.ղ(լ,ę);}void յ(string Ȃ,string վ,string Â){զ.տ($"{Ȃ}: {վ}> {Â}");A.Ӟ($"{Ȃ}: {վ}> {Â}",false);}new public void Ý(Ͻ Â){
ը.Add(Â);}public bool Ÿ(string Ƒ){if(Ƒ.Length>0){Ƒ=A.ր(Ƒ);ց(new ւ(Ƒ));return true;}return false;}void ց(ւ փ){var ž=փ.ք;if
(į==null)į=A.S<į>();if(ž=="self"||string.IsNullOrEmpty(ž)){օ(փ);}else{փ.ֆ(A.և);var ב=$"> @{ž} {փ.א}";if(ž=="*")է.ג(փ);
else է.ד(ž,փ);զ.տ(ב);A.Ӟ(ב);}}void օ(ւ Ղ){if(Ղ.ה){foreach(var Ţ in Ղ.ו)į.ӂ(ז(Ţ));}else{į.ӂ(ז(Ղ.Ҙ));}}IEnumerable<double>ז(
List<Á>ח){foreach(var Â in ח){foreach(double י in ט(Â))yield return י;}}IEnumerable<double>ט(Á Â){if(Â.X.ToLower()=="wait"&&
Â.Ã.Count>0){double Հ;if(double.TryParse(Â.Ã[0],out Հ)){A.Ӟ($"> wait {Հ}");yield return Հ;}yield break;}string כ=ך(Â);if(
כ!=null){string ל=A.ր(כ);var Ղ=new ւ(ל);if(Ղ.ה){foreach(var Ţ in Ղ.ו)į.ӂ(ז(Ţ));}else{foreach(double י in ז(Ղ.Ҙ))yield
return י;}yield break;}ם(Â);yield return 0;}string ך(Á Â){string Ƒ=Â.Ť;if(Ƒ.StartsWith("_")){string מ=Ƒ.Substring(1);if(A.և.
ContainsKey(מ)){A.Ӟ($"Executing local command: {מ}",false);return A.ן(A.և[מ],Â.В);}}if(!Â.נ&&ո(Â.X)!=0)return null;if(A.և.
ContainsKey(Â.X))return A.ן(A.և[Â.X],Â.В);string ס="!"+Â.X;if(A.և.ContainsKey(ס))return A.ן(A.և[ס],Â.В);return null;}void ם(Á Â){
string Ƒ=Â.Ť;if(!Â.נ){long ע=ո(Â.X);if(ע!=0){է.ջ(ע,Ƒ);return;}}foreach(Ͻ ף in ը){if(ף.Ͼ()==Â.X){var ב="> "+Ƒ;A.Ӟ(ב);string Ï=ף
.Ò(Â);A.Ӟ(Ï,false);return;}}if(!Â.נ){long פ=պ(Â.X);if(פ!=0){է.ջ(פ,Ƒ);return;}}A.Ӟ(Ê.Ë(Å.ե,Â.Ť),false);}public List<string
>צ(){var ץ=new List<string>(ը.Count+A.և.Count);for(int u=0;u<ը.Count;u++)ץ.Add(ը[u].Ͼ());foreach(var œ in A.և.Keys)ץ.Add(
œ);return ץ;}public void װ(long ק,List<string>ח){if(ק==A.к)return;var ר=new HashSet<string>();var ש=new HashSet<string>()
;ח.ForEach(ת=>{if(ת.StartsWith("!"))ש.Add(ת.Substring(1));else ר.Add(ת);});թ[ק]=ר;ժ[ק]=ש;}public long պ(string ױ){foreach
(var Ϟ in թ){if(Ϟ.Value.Contains(ױ))return Ϟ.Key;}return 0;}public long ո(string ױ){foreach(var Ϟ in ժ)if(Ϟ.Value.
Contains(ױ))return Ϟ.Key;return 0;}}public class ի:ª{Ä µ;public ի(Ä º){µ=º;}public override string X=>"help";public
override string Ò(Á Â){var ײ=new StringBuilder();µ.Ҙ.ForEach(ף=>{ײ.Append(ף.Ͼ()).Append('\n');});return$"{ײ}";}}public class Á{
public string Ť;public string X;public List<string>Ã=new List<string>();public Dictionary<string,string>В=new Dictionary<
string,string>();public bool ؠ=false;public bool ء=false;public bool נ=false;public Á(string Ƒ){Ť=Ƒ.Replace("\r","").Trim();آ(
);}public string Л(string œ){if(В.ContainsKey(œ))return В[œ];return"";}public static bool К(string Ŕ){return Ŕ?.Trim().
ToLower()=="true"||Ŕ?.Trim()=="1";}void آ(){foreach(string ؤ in أ(Ť)){if(ؤ.StartsWith("--")){string[]إ=ؤ.Split('=');string œ=إ[
0].Substring(2);if(إ.Length==2){В.Add(œ,إ[1]);}else{В.Add(œ,"true");}}else{Ã.Add(ؤ);}}X=Ã[0];if(X.StartsWith("!!")){נ=
true;X=X.Substring(2);}Ã.RemoveAt(0);}public static List<string>أ(string ئ){var ا=new List<string>();int u=0;while(u<ئ.
Length){if(ئ[u]==' '){u++;continue;}if(ئ[u]=='"'){int Ǹ=ئ.IndexOf('"',u+1);if(Ǹ==-1)Ǹ=ئ.Length;ا.Add(ئ.Substring(u+1,Ǹ-u-1));u
=Ǹ+1;}else{int Ǹ=ئ.IndexOf(' ',u);if(Ǹ==-1)Ǹ=ئ.Length;ا.Add(ئ.Substring(u,Ǹ-u));u=Ǹ+1;}}return ا;}}public class ւ{string
ب;public string ք="self";public string א="";public List<Á>Ҙ=new List<Á>();public List<List<Á>>ו=new List<List<Á>>();
public bool ה=>ו.Count>0;List<Á>ة=new List<Á>();public ւ(string Ղ){ب=Ղ.Trim();ت();}void ح(string Â){string[]ث=Â.Split(' ');
string ج=ث[0];if(ج.StartsWith("@")){ք=ج.Substring(1);ب=Â.Substring(ج.Length);}if(ج=="*"){ք=ج;ب=Â.Substring(1);}}void ت(){ح(ب);
if(خ(ب)){د(ب);}else{List<string>ر=ذ(ب);foreach(var Ƒ in ر){if(!string.IsNullOrWhiteSpace(Ƒ))Ҙ.Add(new Á(Ƒ.Trim()));}}}
static bool خ(string Ղ){if(!Ղ.Contains("{")||!Ղ.Contains("}"))return false;int ز=0;foreach(char ͽ in Ղ){if(ͽ=='{')ز++;else if(
ͽ=='}')ز--;else if(ز==0&&!char.IsWhiteSpace(ͽ))return false;}return true;}void د(string Ղ){List<Á>س=null;StringBuilder ش=
new StringBuilder();bool ص=false;int ز=0;for(int u=0;u<Ղ.Length;u++){char ͽ=Ղ[u];if(ͽ=='"')ص=!ص;if(!ص){if(ͽ=='{'){ز++;if(ز
==1){س=new List<Á>();ش.Clear();continue;}}else if(ͽ=='}'){ز--;if(ز==0){string ת=ش.ToString().Trim();if(!string.
IsNullOrWhiteSpace(ת))س.Add(new Á(ת));if(س.Count>0)ו.Add(س);س=null;ش.Clear();continue;}}else if(ͽ==';'&&ز==1){string ת=ش.ToString().Trim()
;if(!string.IsNullOrWhiteSpace(ת))س.Add(new Á(ת));ش.Clear();continue;}}if(ز>0)ش.Append(ͽ);}}public ւ ֆ(Dictionary<string,
string>Π){foreach(Á Â in Ҙ){string ط=ض(Â,Π);ة.Add(new Á(ط));}var ײ=new StringBuilder();foreach(Á Â in ة)ײ.Append(Â.Ť).Append(
';');א=ײ.ToString();return this;}static string ض(Á Â,Dictionary<string,string>Π){string Ƒ=Â.Ť;Dictionary<string,string>ع=ظ(
Π);Ƒ=غ(Ƒ,ع);Ƒ=ػ(Ƒ);return Ƒ;}List<string>ذ(string Ղ){bool ص=false;int ز=0;List<string>ח=new List<string>();StringBuilder
ش=new StringBuilder();foreach(char ͽ in Ղ){if(ͽ=='"')ص=!ص;if(!ص){if(ͽ=='{')ز++;else if(ͽ=='}')ز--;}if(ͽ==';'&&!ص&&ز==0){ח
.Add(ش.ToString().Trim());ش.Clear();}else{ش.Append(ͽ);}}if(ش.Length>0)ח.Add(ش.ToString().Trim());return ח;}static
Dictionary<string,string>ظ(Dictionary<string,string>Π){Dictionary<string,string>ع=new Dictionary<string,string>();var ؽ=Π.Keys.
OrderByDescending(ؼ=>ؼ.Length).ToList();ؽ.ForEach(œ=>{ع[œ]=ؾ(Π[œ],ع);});return ع;}static string ؾ(string Ŕ,Dictionary<string,string>ع){
StringBuilder ײ=new StringBuilder(Ŕ);bool ؿ;do{ؿ=false;foreach(var Ϟ in ع){string œ=Ϟ.Key;string ـ=Ϟ.Value;if(ف(ײ.ToString(),œ)){ײ.
Replace(œ,ـ);ؿ=true;}}}while(ؿ);return$"{ײ}";}static string غ(string Â,Dictionary<string,string>ع){StringBuilder ײ=new
StringBuilder(Â);foreach(var Ϟ in ع){if(ف($"{ײ}",Ϟ.Key))ײ.Replace(Ϟ.Key,Ϟ.Value);}return$"{ײ}";}static bool ف(string Ǌ,string ق){
return System.Text.RegularExpressions.Regex.IsMatch(Ǌ,$@"\b{ق}\b");}static string ػ(string Â){while(Â.Contains(";;"))Â=Â.
Replace(";;",";");return Â.Trim(';');}}public class ك:ª{О µ;public override string X=>"var/set";public ك(О º){µ=º;}
public override string Ò(Á Â){if(Â.Ã.Count<2)return Ä.Å.Д;string Þ=Â.Ã[0];string Ŕ=Â.Ã[1];bool ل=Â.В.ContainsKey("save");µ.م(Þ
,Ŕ,ل);return ل?$"${Þ} = \"{Ŕ}\" (saved)":$"${Þ} = \"{Ŕ}\"";}}public class λ:ŀ{}public class ľ:ŀ{}public class ن{MyIni è;public ن(MyIni ƴ){è=ƴ;}public MyIni Q(){ه();return è;}void ه(){if(è.ContainsSection("Commands")){string و=è.ToString(
);و=و.Replace("[Commands]","[commands]");è.TryParse(و);}if(!è.ContainsSection("channels")){string ى=è.Get("security",
"passcodes").ToString();è.Set("channels","default",ى);if(è.ContainsSection("security"))è.DeleteSection("security");}}}public class
О:Ф{Dictionary<string,string>ي=new Dictionary<string,string>(){};public Dictionary<string,string>Ӣ=new
Dictionary<string,string>(){};string[]ٮ=new string[]{"general","channels","variables","commands","hooks",};public
MyIni ӭ=new MyIni();public MyIni П=>ӭ;public О(A B):base(B){}void ٯ(){MyIniParseResult Ρ;if(!ӭ.TryParse(A.ō.CustomData,
out Ρ))throw new Exception($"{Ρ}");}public override void V(){ٱ();A.ō.CustomData=$"{new ن(ӭ).Q()}";A.S<Ҩ>()?.Ŀ<ľ>(this);Ý(
new ك(this));}public void ٲ(){ٱ();}void ٱ(){ٯ();ӟ();ٳ();ٴ();A.ξ=у("general.debug").ToLower()=="true";var ٵ=у("general.name"
);A.X=!string.IsNullOrEmpty(ٵ)?ٶ(ٵ):A.Н.CustomName;}public override void ń(ŀ Ł,object ł){if(Ł is ľ)ٲ();}public string у(
string ٷ){List<string>ٸ=new List<string>(ٷ.Split('.'));if(ٸ.Count!=2)return$"";else{string ŏ=ٸ[0];string œ=ٸ[1];return
$"{ӭ.Get(ŏ,œ)}";}}string ٶ(string ئ){if(ئ.Length>=2&&ئ[0]=='"'&&ئ[ئ.Length-1]=='"')return ئ.Substring(1,ئ.Length-2);return ئ;}void ٳ(){
A.ٹ.Clear();var ٺ="variables";List<MyIniKey>ő=new List<MyIniKey>();ӭ.GetKeys(ٺ,ő);foreach(var œ in ő){string ٻ=œ.Name;
string ټ=$"{ӭ.Get(ٺ,ٻ)}";if(ٻ.StartsWith("$"))ٻ=ٻ.Substring(1);ټ=ٶ(ټ);A.ٹ[ٻ]=ټ;}}void ٴ(){A.և.Clear();List<MyIniKey>ő=new List
<MyIniKey>();ӭ.GetKeys("Commands",ő);foreach(var œ in ő){string ױ=œ.Name;string ٽ=$"{ӭ.Get("Commands",ױ)}".Replace("\r",
"").Replace("\n"," ").Trim();ٽ=System.Text.RegularExpressions.Regex.Replace(ٽ,@"\s+"," ");ٽ=ٶ(ٽ);A.և[ױ]=ٽ;}}public void م(
string Þ,string Ŕ,bool ل=false){A.ٹ[Þ]=Ŕ;if(ل){ӭ.Set("variables",Þ,Ŕ);A.ō.CustomData=$"{ӭ}";}}void ӟ(){foreach(string ŏ in ٮ)
if(!ӭ.ContainsSection(ŏ))ӭ.AddSection(ŏ);foreach(KeyValuePair<string,string>پ in ي){string[]ӣ=پ.Key.Split('.');string ŏ=ӣ[
0];string œ=ӣ[1];string Ŕ=پ.Value;if(ӭ.Get(ŏ,œ).IsEmpty)ӭ.Set(ŏ,œ,Ŕ);}A.ō.CustomData=$"{ӭ}";}}public class ڀ:ª{ٿ
µ;public override string X=>"connector/lock";public ڀ(ٿ º){µ=º;}public override string Ò(Á Â){if(Â.Ã.Count==0)return Ä.Å.
Æ;else{string ځ=Â.Ã[0];List<IMyShipConnector>ڂ=µ.ɦ<IMyShipConnector>(ځ);if(ڂ.Count==0)return Ê.Ë(Ì.Í,ځ);ڂ.ForEach(ڃ=>µ.ڄ(
ڃ));return Ê.Ë(Ì.Վ,ځ);}}}public class څ:ª{ٿ µ;public override string X=>"connector/toggle";public څ(ٿ º){µ=º;}
public override string Ò(Á Â){if(Â.Ã.Count==0)return Ä.Å.Д;else{string ځ=Â.Ã[0];List<IMyShipConnector>ڂ=µ.ɦ<IMyShipConnector>(
ځ);if(ڂ.Count==0)return Ê.Ë(Ì.Í,ځ);ڂ.ForEach(ڃ=>µ.چ(ڃ));return Ê.Ë(Ì.ՙ,ځ);}}}public class ڇ:ª{ٿ µ;public
override string X=>"connector/unlock";public ڇ(ٿ º){µ=º;}public override string Ò(Á Â){if(Â.Ã.Count==0)return Ä.Å.Д;else{string
ځ=Â.Ã[0];List<IMyShipConnector>ڂ=µ.ɦ<IMyShipConnector>(ځ);if(ڂ.Count==0)return Ê.Ë(Ì.Í,ځ);ڂ.ForEach(ڃ=>µ.ڈ(ڃ));return Ê.Ë
(Ì.Տ,ځ);}}}public class ٿ:Ф{à à;public ٿ(A B):base(B){}public override void V(){à=A.S<à>();Ý(new ڀ(this));Ý(new ڇ(this));
Ý(new څ(this));Ҡ<IMyShipConnector>(ڃ=>ڃ.Status,(Ņ,ű)=>ډ(Ņ as IMyShipConnector,ű));}protected void ډ(IMyShipConnector ڃ,
object ڊ){var ę=ڊ as MyShipConnectorStatus?;var ڋ=җ.ContainsKey(ڃ.EntityId)?җ[ڃ.EntityId]as MyShipConnectorStatus?:null;if(ę==
MyShipConnectorStatus.Connected){ҩ<ӄ>(ڃ);à.Ӱ(ڃ,"onLock");}else if((ę==MyShipConnectorStatus.Connectable&&ڋ==MyShipConnectorStatus.Connected)
||ę==MyShipConnectorStatus.Unconnected){ҩ<Ӆ>(ڃ);à.Ӱ(ڃ,"onUnlock");}else if(ę==MyShipConnectorStatus.Connectable){ҩ<ڌ>(ڃ);à
.Ӱ(ڃ,"onReady");}}public void ڄ(IMyShipConnector ڃ){ڃ.Connect();}public void ڈ(IMyShipConnector ڃ){ڃ.Disconnect();}public
void چ(IMyShipConnector ڃ){if(ڃ.Status==MyShipConnectorStatus.Connected)ڈ(ڃ);else ڄ(ڃ);}}public class ӄ:ŀ{}public class ڌ:ŀ{
}public class Ӆ:ŀ{}public class Z{public IMyTextSurface Ʒ;public IMyTerminalBlock ڍ;public RectangleF b;
protected float ڎ=4f;protected float ڏ=0;protected const float ڐ=16f;protected const float ڑ=1f;public float ڒ=ڑ;protected const
float ړ=1f;public float ɣ;public MySpriteDrawFrame Ċ;public bool Ǆ;public MyIni О;public Vector2 ڔ,ڕ,ږ,ڗ,ژ;public Z(
IMyTextSurface Ð,IMyTerminalBlock Ņ,MyIni ӓ,bool ڙ=false){Ʒ=Ð;ڍ=Ņ;О=ӓ;Ǆ=ڙ;ښ();ڛ();}public virtual void ڜ(MyIni ƴ){О=ƴ;}protected
virtual void ڛ(){b=ڝ(Ʒ,ڎ);float ڞ=Ǆ?ڏ:ڎ;float ڟ=Ǆ?ڏ:ڎ;ڔ=b.Position+new Vector2(ڞ,ڟ);ڕ=new Vector2(b.X+b.Width-ڞ,b.Y+ڟ);ږ=new
Vector2(b.X+ڞ,b.Y+b.Height-ڟ);ڗ=new Vector2(b.X+b.Width-ڞ,b.Y+b.Height-ڟ);ژ=new Vector2(b.X+(b.Width/2f),b.Y+(b.Height/2f));ɣ=ڠ
();}protected virtual void ښ(){}public virtual void ύ(){Ċ=Ʒ.DrawFrame();Random ڡ=new Random();Vector2 ڢ=new Vector2((
float)ڡ.NextDouble(),(float)ڡ.NextDouble());Ċ.Add(ǀ.ǁ("SquareSimple",ڢ,ڢ,Color.Transparent));}protected RectangleF ڝ(
IMyTextSurface Ð,float ǆ){var ڣ=Ð.SurfaceSize;return new RectangleF(Ð.TextureSize.X/2f-ڣ.X/2f+ǆ,Ð.TextureSize.Y/2f-ڣ.Y/2f+ǆ,ڣ.X-(4*ǆ),
ڣ.Y-(4*ǆ));}public void ڨ(Vector3D ڤ,Vector3D ڥ){var ʖ=ڥ-ڤ;var ڦ=b.Width/ʖ.X;var ڧ=b.Height/ʖ.Y;ڒ=ڑ*(float)Math.Min(ڦ,ڧ);
}public virtual float ڠ(){float ک=b.Width/1024f;float ǌ=ڐ*ک*ړ;ǌ=Math.Max(ǌ,8f);ǌ=Math.Min(ǌ,32f);return ǌ;}public float ڪ
()=>Math.Min(1f,ɣ/ڐ);public bool ǜ=>b.Width/b.Height>1.8f;public RectangleF ϓ(RectangleF ګ){RectangleF ڬ=b;b=ګ;return ڬ;}
public void گ(Vector2 ê,float ǅ,Color w,float ڭ=0){float ڮ=MathHelper.ToRadians(ڭ);Ċ.Add(ǀ.ǁ("SquareSimple",ê,new Vector2(ǅ,ǅ)
,w,ڮ));}public void Ȯ(Vector2 Ƿ,Vector2 Ǹ,Color w,float ǎ){var ڰ=(Ƿ+Ǹ)/2;var ڱ=Vector2.Distance(Ƿ,Ǹ);var ɫ=(float)Math.
Atan2(Ǹ.Y-Ƿ.Y,Ǹ.X-Ƿ.X);Ċ.Add(ǀ.ǁ("SquareSimple",ڰ,new Vector2(ڱ,ǎ),w,ɫ));}public void Ǉ(Vector2 ê,float ǅ,Color ڲ,Color Ɇ){
float ȥ=ǅ/2f;Vector2[]ڳ=new Vector2[8];float ڴ=MathHelper.PiOver4;float ڵ=MathHelper.Pi/8;for(int u=0;u<8;u++){float ɫ=u*ڴ+ڵ;
ڳ[u]=ê+new Vector2((float)Math.Cos(ɫ),(float)Math.Sin(ɫ))*ȥ;}گ(ê,ǅ,Ɇ);for(int u=0;u<8;u++){Ȯ(ڳ[u],ڳ[(u+1)%8],ڲ,1f);}}
public void y(Vector2 ê,float v,Color w){Ċ.Add(ǀ.ǁ("Circle",ê,new Vector2(v,v),w));}public void ڶ(Vector2 ê,float ǅ,Color w,
float ڭ=0){float ڮ=MathHelper.ToRadians(ڭ);Ċ.Add(ǀ.ǁ("Triangle",ê,new Vector2(ǅ,ǅ),w,ڮ));}public void ǃ(){Ċ.Add(ǀ.ǁ(
"SquareSimple",Ʒ.TextureSize/2f,Ʒ.TextureSize,Color.Black));}public void d(string Ǌ,Vector2 ǋ,Color w,string Ϻ,float Ȫ=1f){Ċ.Add(new
MySprite{Type=SpriteType.TEXT,Data=Ǌ,Position=ǋ,RotationOrScale=ڪ()*Ȫ,Color=w,Alignment=TextAlignment.LEFT,FontId=Ϻ});}public
void d(string Ǌ,Vector2 ǋ,Color w,float Ȫ=1f)=>d(Ǌ,ǋ,w,"Monospace",Ȫ);public void d(string Ǌ,Vector2 ǋ,float Ȫ=1f)=>d(Ǌ,ǋ,
Color.White,"Monospace",Ȫ);public void ȱ(string Ǌ,Vector2 ǋ,Color w,float Ȫ=1f){Ċ.Add(new MySprite{Type=SpriteType.TEXT,Data=
Ǌ,Position=ǋ,RotationOrScale=ڪ()*Ȫ,Color=w,Alignment=TextAlignment.CENTER,FontId="Monospace"});}public void ڷ(){Vector2 ê
=new Vector2(b.X+b.Width,b.Y+b.Height);float ǅ=Ǆ?20:40;ê-=new Vector2(ǅ/2,ǅ/2);Ǉ(ê,ǅ,Color.White,Color.Black);y(ê,ǅ*0.4f,
Color.Red);}public void ڹ(){float Ȫ=ڪ();y(ڔ,5*Ȫ,Color.Red);y(ڕ,5*Ȫ,Color.Red);y(ږ,5*Ȫ,Color.Red);y(ڗ,5*Ȫ,Color.Red);y(ژ,5*Ȫ,
Color.Red);Vector2 ڸ=Ǆ?ږ-new Vector2(0,1.5f*ɣ):ږ-new Vector2(0,2f*ɣ);d($"scale={ڒ:F2}",ڸ,Color.White,"White");}public static
string ۂ(string ʶ,string ں,int ڻ){const int ڼ=5;int ڽ=ڻ-ں.Length-ڼ;if(ڽ<0)return ں.Substring(Math.Max(0,ں.Length-ڻ));string ھ=
ʶ.Length>ڽ?ʶ.Substring(0,ڽ):ʶ;int ڿ=ڻ-ھ.Length-ں.Length;if(ڿ<ڼ){int ۀ=ڼ-ڿ;ھ=ھ.Substring(0,Math.Max(0,ھ.Length-ۀ));ڿ=ڻ-ھ.
Length-ں.Length;}string ہ=new string('.',ڿ);return$"{ھ}{ہ}{ں}";}}public class Ü:Ф{public const string ۃ="LogView";į į;զ զ;à à;
HashSet<IMyTextSurface>ۄ=new HashSet<IMyTextSurface>();List<IMyTextSurface>ۅ=new List<IMyTextSurface>();
Dictionary<string,List<IMyTextSurface>>ۆ=new Dictionary<string,List<IMyTextSurface>>();public static float ۇ=1;public Ü
(A B):base(B){}public override void V(){į=A.S<į>();զ=A.S<զ>();à=A.S<à>();Ŀ<κ>();ۈ();}public override void ń(ŀ Ł,object ł)
{if(Ł is κ&&(ł is IMyTextPanel||ł is IMyTextSurfaceProvider)){ۈ();}}string ۋ(string ۉ=""){return
$" {A.C} - {ۉ}     ({į.Շ()})\n"+$" {A.X} *{A.Ķ}                                  {ۊ()}\n"+"------------------------------------------------------";}
public string ۊ(){string ی=A.S<Ы>().Ц.Count()>0?"M":"   ";string ۍ=A.S<Р>().ۍ.Count()>0?"C":"    ";string ێ=
$"{A.S<Ў>()?.д.Count()??0}";string ې=A.ۏ?"A":"   ";string ۑ=į.Յ>0?"W":"   ";return String.Join("  ",ۑ,ې,ۍ,ی,ێ);}public List<IMyTextSurface>È(string
Þ){var ے=Þ.Split(':');var ۓ=new List<IMyTextSurface>();if(ے.Length>1){string ɨ=ے[0].Trim();int Ϧ=int.Parse(ے[1].Trim());ۓ
=à.ɦ<IMyTerminalBlock>(ɨ).Where(Ņ=>Ņ is IMyTextSurfaceProvider).Select(Ņ=>((IMyTextSurfaceProvider)Ņ).GetSurface(Ϧ)).
Where(Ð=>Ð!=null).ToList();}else{ۓ=à.ɦ<IMyTerminalBlock>(Þ).Where(Ņ=>Ņ is IMyTextSurface).Select(Ņ=>(IMyTextSurface)Ņ).ToList
();}return ۓ;}void ۥ(){à.Đ<IMyTextPanel>()?.ForEach(ϙ=>ە(ϙ));}void ە(IMyTextPanel ϙ){MyIni ƴ=à.Ϛ(ϙ);foreach(ϛ Ϟ in Ϝ.ϝ(ƴ)
){if(Ϟ.ϟ!=0)continue;ۄ.Add(ϙ);if(string.Equals(Ϟ.Ϡ,ۃ,StringComparison.OrdinalIgnoreCase)&&Ϝ.ۦ(Ϟ.ϥ,A)){ϙ.ContentType=
ContentType.TEXT_AND_IMAGE;ۅ.Add(ϙ);}ۮ(Ϟ.Ϡ,ϙ);break;}}void ۺ(){var ۯ=new List<IMyTerminalBlock>();ۯ.AddRange(à.Đ<IMyCockpit>());ۯ.
AddRange(à.Đ<IMyProgrammableBlock>());ۯ.AddRange(à.Đ<IMySoundBlock>());ۯ.ForEach(Ņ=>ۺ(Ņ));}void ۺ(IMyTerminalBlock Ņ){
IMyTextSurfaceProvider ϋ=Ņ as IMyTextSurfaceProvider;if(ϋ==null)return;MyIni ƴ=à.Ϛ(Ņ);foreach(ϛ Ϟ in Ϝ.ϝ(ƴ)){if(Ϟ.ϟ>=ϋ.SurfaceCount)continue;
IMyTextSurface Ð=ϋ.GetSurface(Ϟ.ϟ);ۄ.Add(Ð);if(string.Equals(Ϟ.Ϡ,ۃ,StringComparison.OrdinalIgnoreCase)&&Ϝ.ۦ(Ϟ.ϥ,A)){Ð.ContentType=
ContentType.TEXT_AND_IMAGE;ۅ.Add(Ð);}ۮ(Ϟ.Ϡ,Ð);}}public void ۈ(){ۻ();ۥ();ۺ();}void ۻ(){ۄ.Clear();ۅ.Clear();foreach(var ۼ in ۆ.Values
)ۼ.Clear();}public void ܐ(){ۿ();}string ܓ(){string ܒ=string.Join("\n",զ.д);return ۋ("LOG")+"\n"+ܒ;}void ۿ(){string ܒ=ܓ();
ۅ.ForEach(Ð=>Ð.WriteText($"{ܒ}",false));}public List<IMyTextSurface>ܔ(string ϗ){string œ=ϗ.ToLower();if(ۆ.ContainsKey(œ))
return ۆ[œ];return new List<IMyTextSurface>();}void ۮ(string ϗ,IMyTextSurface Ð){string œ=ϗ.ToLower();if(!ۆ.ContainsKey(œ))ۆ[œ
]=new List<IMyTextSurface>();ۆ[œ].Add(Ð);}}public struct ϛ{public int ϟ;public string Ϡ;public string ϥ;}public static
class Ϝ{const string ܕ="surfaces";public static List<ϛ>ϝ(MyIni ܖ){var ܗ=new List<ϛ>();var ő=new List<MyIniKey>();ܖ.GetKeys(ܕ,
ő);foreach(MyIniKey œ in ő){int Ӭ;if(!int.TryParse(œ.Name,out Ӭ))continue;string Έ=ܖ.Get(ܕ,œ.Name).ToString().Trim();if(
string.IsNullOrEmpty(Έ))continue;List<string>ا=Á.أ(Έ);string ϗ=ا[0];string ƈ=ا.Count>1?ا[1]:null;ܗ.Add(new ϛ{ϟ=Ӭ,Ϡ=ϗ,ϥ=ƈ});}
return ܗ;}public static bool ۦ(string ܘ,A B){if(string.IsNullOrEmpty(ܘ))return true;return string.Equals(ܘ,B.C,
StringComparison.OrdinalIgnoreCase)||string.Equals(ܘ,B.Ķ,StringComparison.OrdinalIgnoreCase);}}public static class ǀ{public static
MySprite ǁ(string ܙ,Vector2 ǋ,Vector2 ǅ,Color w,float ڮ=0f){return new MySprite{Type=SpriteType.TEXTURE,Data=ܙ,Position=ǋ,Size=ǅ
,Color=w,Alignment=TextAlignment.CENTER,RotationOrScale=ڮ};}}public class Ҩ:Ф{public Ҩ(A B):base(B){}Dictionary<Type,HashSet<ғ>>ܚ=new Dictionary<Type,HashSet<ғ>>();public void Ŀ<ҧ>(ғ º)where ҧ:ŀ{var ܛ=typeof(ҧ);if(!ܚ.ContainsKey(ܛ)
)ܚ[ܛ]=new HashSet<ғ>();ܚ[ܛ].Add(º);}public bool ܜ<ҧ>(ғ º){var ܛ=typeof(ҧ);if(ܚ.ContainsKey(ܛ))return ܚ[ܛ].Contains(º);
return false;}public void ܝ<ҧ>(ғ º)where ҧ:ŀ{var ܛ=typeof(ҧ);if(ܚ.ContainsKey(ܛ))ܚ[ܛ].Remove(º);}public new void ҩ<ҧ>(object ł
=null)where ҧ:ŀ,new(){ҩ(new ҧ(),ł);}public new void ҩ(ŀ Ł,object ł=null){var ܛ=Ł.GetType();if(ܚ.ContainsKey(ܛ))ܚ[ܛ].
ToList().ForEach(º=>º.ń(Ł,ł));}}public interface ŀ{}public interface ұ:ғ{}public interface E:ғ{}public interface ғ{void V();
IEnumerator<double>Қ();void Q();void ń(ŀ Ł,object ł);string қ();List<Ͻ>Ҫ();}public class ܞ:ª{A A;public override string X
=>"ping";public ܞ(A B){A=B;}public override string Ò(Á Â){A.S<Р>().У();return"Pinging all grids";}}public class ܟ:ŀ{}
public class ܠ:ŀ{}public class ܡ:ŀ{}public abstract class ܧ{public Dictionary<string,object>ܢ=new Dictionary<string,object>();
public Dictionary<string,object>ܣ=new Dictionary<string,object>();public HashSet<string>њ=new HashSet<string>();public string
к{get;}=ܤ();public ܧ(Dictionary<string,object>ܥ,Dictionary<string,object>ܦ){ܣ=ܥ;ܢ=ܦ;if(!ܢ.ContainsKey("Id"))ܢ["Id"]=ܤ();}
public object ܨ(string œ){object Ŕ;return ܣ.TryGetValue(œ,out Ŕ)?Ŕ??"":"";}public string հ(string œ)=>$"{ܨ(œ)}";public float ܩ
(string œ){float Ŕ;float.TryParse(հ(œ),out Ŕ);return Ŕ;}public double ܪ(string œ){double Ŕ;double.TryParse(հ(œ),out Ŕ);
return Ŕ;}public object ܫ(string œ){object Ŕ;return ܢ.TryGetValue(œ,out Ŕ)?Ŕ??"":"";}public string ն(string œ)=>$"{ܫ(œ)}";
public float ܬ(string œ){float Ŕ;float.TryParse(ն(œ),out Ŕ);return Ŕ;}public double ܭ(string œ){double Ŕ;double.TryParse(ն(œ),
out Ŕ);return Ŕ;}public long ܮ(string œ){long Ŕ;long.TryParse(ն(œ),out Ŕ);return Ŕ;}static string ܤ(){long ܯ=DateTime.
UtcNow.Ticks;int ݍ=new Random().Next(0,1000);return$"{ܯ}_{ݍ}";}public virtual string ѵ(){string ݎ="header";string ݏ="body";
string ݐ=ц.ѓ(ܢ);string ݑ=ц.ѓ(ܣ);return$"<{ݎ}>{ݐ}</{ݎ}>"+$"<{ݏ}>{ݑ}</{ݏ}>";}public static string ݗ(string ݒ,string ݓ){string ݔ=
$"<{ݓ}>";string ݕ=$"</{ݓ}>";int ӫ=ݒ.IndexOf(ݔ)+ݔ.Length;int ݖ=ݒ.IndexOf(ݕ);if(ӫ==-1||ݖ==-1||ӫ>=ݖ)return"";return ݒ.Substring(ӫ,ݖ
-ӫ).Trim();}}public class Р:Ф{class Å{public const string ݘ="Cannot de-serialize message.";public const string ݙ=
"No active request found for RespondingToId: {0}";}į į;զ զ;Ў Ў;Ҩ Ҩ;public Ү Ү;public Dictionary<string,Action<ܧ>>ۍ=new Dictionary<string,Action<ܧ>>();const
string ݚ=".construct";IMyUnicastListener ݛ;List<IMyBroadcastListener>ݜ=new List<IMyBroadcastListener>();public
Dictionary<string,string>њ=new Dictionary<string,string>();long ݝ=0;public bool կ=>ݞ()==A.к;public Р(A B):base(B){Ү=new Ү();}
public override void V(){į=A.S<į>();զ=A.S<զ>();Ў=A.S<Ў>();Ҩ=A.S<Ҩ>();A.Ý(new ܞ(A));ݟ();ݠ();Ҩ.Ŀ<ľ>(this);Ү.ү("ping",լ=>ղ(լ,Ҭ.ճ.
ݡ));Ү.ү("sync",լ=>ݢ(լ));Ү.ү("almanac",լ=>ݣ(լ));į.Ձ(()=>Т(),0.5);į.ж(Т,5);į.ж(У,2);}Ҭ ݢ(ҫ լ){ݤ(լ);var ݥ=A.S<Ä>().צ();var ݦ
=ղ(լ,Ҭ.ճ.ݡ,new Dictionary<string,object>{{"Commands",string.Join(",",ݥ)}});ݦ.њ.Add(ݚ);return ݦ;}void ݟ(){њ.Clear();var ƴ=
A.S<О>();var ő=new List<MyIniKey>();ƴ.П.GetKeys("channels",ő);ő.ForEach(œ=>{var Ŕ=ƴ.П.Get(œ.Section,œ.Name);њ[œ.Name]=
$"{Ŕ}";});}void ݠ(){ݛ=A.ݧ.UnicastListener;ݛ.SetMessageCallback();IMyBroadcastListener ݨ=A.ݧ.RegisterBroadcastListener(ݚ);ݨ.
SetMessageCallback();ݜ.Add(ݨ);foreach(var ݩ in њ){IMyBroadcastListener ݪ=A.ݧ.RegisterBroadcastListener(ݩ.Key);ݪ.SetMessageCallback();ݜ.Add
(ݪ);}}public override void ń(ŀ Ł,object ł){if(Ł is ľ){ݟ();ݠ();į.Ձ(Т,0);}}public void ݭ(){while(ݛ?.HasPendingMessage==true
)ݫ(ݛ.AcceptMessage());ݜ.ForEach(ݬ=>{while(ݬ?.HasPendingMessage==true)ݫ(ݬ.AcceptMessage());});ۍ.Clear();}string ݳ(
MyIGCMessage ݒ){string ݮ=$"{ݒ.Data}";string ݩ=ݒ.Tag;string ݰ=ݯ(ݩ);if(ݰ=="")return ݮ;else return ݱ.ݲ(ݮ,ݰ);}public void ݫ(MyIGCMessage
ݒ){string ݮ=$"{ݒ.Data}";ݮ=ݱ.ݴ(ݮ)?ݳ(ݒ):ݮ;if(ݮ.StartsWith("REQUEST::")){ݵ(ҫ.ݶ(ݮ),ݒ.Tag,ݷ=>ݸ((ҫ)ݷ));}else if(ݮ.StartsWith(
"RESPONSE::")){ݵ(Ҭ.ݶ(ݮ),ݒ.Tag,ݷ=>ݹ((Ҭ)ݷ));}}void ݵ(ܧ ݺ,string ݻ,Action<ܧ>ӏ){if(ݺ!=null){ݺ.њ.Add(ݻ);bool ݼ=ݻ!=ݚ;if(ݼ){ݽ(ݺ);ݾ(ݺ);}ӏ(ݺ)
;}else{զ.ݿ(Å.ݘ);}}г ݽ(ܧ ݒ){long ѕ=ݒ.ܮ("OriginId");string ހ=ݒ.ն("GridId");string є=!string.IsNullOrEmpty(ހ)?ހ:$"{ѕ}";
Vector3D?о=null;Vector3D?п=null;double Ѿ,ѿ,Ҁ,ҁ,Ҋ,ҋ;if(double.TryParse(ݒ.ն("Fx"),out Ѿ)&&double.TryParse(ݒ.ն("Fy"),out ѿ)&&double
.TryParse(ݒ.ն("Fz"),out Ҁ)){о=new Vector3D(Ѿ,ѿ,Ҁ);}if(double.TryParse(ݒ.ն("Ux"),out ҁ)&&double.TryParse(ݒ.ն("Uy"),out Ҋ)
&&double.TryParse(ݒ.ն("Uz"),out ҋ)){п=new Vector3D(ҁ,Ҋ,ҋ);}return Ў.ѣ(є,ѕ,ݒ.ն("OriginName"),new Vector3D(ݒ.ܬ("X"),ݒ.ܬ("Y")
,ݒ.ܬ("Z")),ݒ.ܬ("Speed"),ݒ.њ,ށ(ѕ),о,п);}void ݸ(ҫ լ){if(լ==null)return;Ҩ.ҩ<ܠ>();Ҭ ݦ=Ү.ނ(լ.ն("Path"),լ);if(ݦ!=null)ރ(ݦ.ܮ(
"TargetId"),ݦ,null);}void ݹ(Ҭ ݦ){if(ݦ==null)return;if(ݦ.ܢ.ContainsKey("RespondingToId")){string ބ=ݦ.ն("RespondingToId");if(ۍ.
ContainsKey(ބ))ۍ[ބ]?.Invoke(ݦ);}else զ.ݿ($"Response missing 'RespondingToId' header: {ц.ѓ(ݦ.ܢ)}");}public void ރ(long ޅ,ܧ ݒ,Action<
ܧ>ކ){ۍ[$"{ݒ.ܢ["Id"]}"]=ކ;bool Ž=false;var އ=ݒ.њ.OrderBy(ͽ=>ͽ=="*").ToHashSet();if(އ.Count==0)އ.Add(ݚ);foreach(string ݩ in
އ){string މ=ݱ.ވ(ݒ.ѵ(),ݯ(ݩ));Ž=A.ݧ.SendUnicastMessage(ޅ,ݩ,މ);if(Ž)break;}if(Ž)Ҩ.ҩ<ܡ>();else Ҩ.ҩ<ܟ>();}public void ފ(ҫ լ,
Action<ܧ>ކ){ۍ[լ.к]=ކ;foreach(var ݩ in њ){string މ=ݱ.ވ(լ.ѵ(),ݯ(ݩ.Key));A.ݧ.SendBroadcastMessage(ݩ.Key,މ);}Ҩ.ҩ<ܡ>();}public void
ד(string ž,ւ Ղ){г л=Ў.й(ž);if(л==null){զ.ݿ($"Target '{ž}' not found in Almanac.");return;}if(л.ь!=г.Ѩ["grid"]){զ.ݿ(
$"Target '{ž}' is not a grid (type: {л.ь}).");return;}ҫ լ=ދ(Ղ.א).ތ(л);long ލ=л.э!=0?л.э:л.Ғ();ރ(ލ,լ,null);}public void ג(ւ Ղ){Ў.Ѧ("grid").ForEach(ގ=>ד(ގ.к,Ղ));}ҫ ދ(
string Â){return ޏ("command",new Dictionary<string,object>{{"Command",Â}});}Dictionary<string,object>ޔ(){MatrixD н=A.м();
Vector3D ɾ=A.ސ();return new Dictionary<string,object>{{"OriginId",$"{A.к}"},{"GridId",$"{A.ޑ}"},{"OriginName",A.X},{"X",$"{ɾ.X}"
},{"Y",$"{ɾ.Y}"},{"Z",$"{ɾ.Z}"},{"SafeRadius",$"{A.с.Radius}"},{"Gravity",$"{A?.ޒ()}"},{"Speed",$"{A?.ޓ()}"},{"Fx",
$"{н.Forward.X}"},{"Fy",$"{н.Forward.Y}"},{"Fz",$"{н.Forward.Z}"},{"Ux",$"{н.Up.X}"},{"Uy",$"{н.Up.Y}"},{"Uz",$"{н.Up.Z}"}};}public ҫ ޏ(
string ȏ,Dictionary<string,object>ޕ=null,Dictionary<string,object>ޖ=null){Dictionary<string,object>ܦ=ޔ();ܦ["Path"]=ȏ;
Dictionary<string,object>ܥ=new Dictionary<string,object>();ޗ(ܦ,ޖ);ޗ(ܥ,ޕ);return new ҫ(ܥ,ܦ);}public Ҭ ղ(ҫ լ,Ҭ.ճ ޘ,Dictionary<string
,object>ޕ=null,Dictionary<string,object>ޖ=null){Dictionary<string,object>ޙ=ޔ();Dictionary<string,object>ޛ=new Dictionary<
string,object>(){{"Status",$"{Ҭ.ޚ(ޘ)}"},{"TargetId",լ.ܢ["OriginId"]},{"TargetName",լ.ܢ["OriginName"]},{"RespondingToId",լ.ܢ[
"Id"]},};Dictionary<string,object>ޜ=new Dictionary<string,object>();ޗ(ޛ,ޙ);ޗ(ޛ,ޖ);ޗ(ޜ,ޕ);return new Ҭ(ޜ,ޛ);}public void У(){
if(!կ)return;var ї=њ.Keys.ToList();ҫ լ=ޏ("ping");լ.њ=new HashSet<string>(ї);ފ(լ,null);}public void Т(){var ޝ=A.S<Ä>().צ();
ҫ լ=ޏ("sync",new Dictionary<string,object>{{"Commands",string.Join(",",ޝ)}},new Dictionary<string,object>{{"OriginName",A
.X}});ޞ(լ,ݦ=>ޟ(ݦ));}void ޟ(ܧ ݦ){ݤ(ݦ);ݝ=0;}public void ջ(long ލ,string Â){ҫ լ=ޏ("localcmd",new Dictionary<string,object>{{
"Command",Â}});A.ݧ.SendUnicastMessage(ލ,ݚ,լ.ѵ());A.Ӟ($"> @local {Â}");}public void ޞ(ҫ լ,Action<ܧ>ކ){ۍ[լ.к]=ކ;A.ݧ.
SendBroadcastMessage(ݚ,լ.ѵ(),TransmissionDistance.CurrentConstruct);}string ݯ(string ݩ){return њ.ContainsKey(ݩ)?њ[ݩ]:"";}void ޗ(Dictionary<
string,object>ž,Dictionary<string,object>ܘ){if(ܘ==null)return;foreach(KeyValuePair<string,object>Ϟ in ܘ)ž[Ϟ.Key]=Ϟ.Value;}void
ݤ(ܧ ݒ){long ѕ=ݒ.ܮ("OriginId");string ޠ=ݒ.հ("Commands");if(!string.IsNullOrEmpty(ޠ)&&ѕ!=A.к){var ח=new List<string>(ޠ.
Split(','));A.S<Ä>().װ(ѕ,ח);}}long ݞ(){if(ݝ!=0)return ݝ;long ޡ=A.к;var ޝ=A.S<Ä>().թ;foreach(var ק in ޝ.Keys)if(ק<ޡ)ޡ=ק;if(ޝ.
Count>0)ݝ=ޡ;return ޡ;}Ҭ ݣ(ҫ լ){if(!կ)ݽ(լ);return null;}static string[]ޢ={"OriginId","GridId","OriginName","X","Y",
"Z","Speed","SafeRadius"};void ݾ(ܧ ݒ){if(!կ)return;var ܦ=new Dictionary<string,object>();foreach(string œ in ޢ)ܦ[œ]=ݒ.ն(œ);
ҫ լ=ޏ("almanac",null,ܦ);A.ݧ.SendBroadcastMessage(ݚ,լ.ѵ(),TransmissionDistance.CurrentConstruct);}bool ށ(long ѕ){return A.
S<à>().Đ<IMyProgrammableBlock>().Any(ޣ=>ޣ.EntityId==ѕ);}}public class ҫ:ܧ{public string ޅ;public string ޤ;public ҫ(
Dictionary<string,object>ܥ,Dictionary<string,object>ܦ):base(ܥ,ܦ){}public ҫ ތ(г л){ܢ["TargetId"]=$"{л.к}";ܢ["TargetName"]=л.ћ??л.к;
њ=л.њ;return this;}public override string ѵ()=>"REQUEST::"+base.ѵ();public static ҫ ݶ(string ݒ){ݒ=ݒ.Replace("REQUEST::",
"");string ޥ=ݗ(ݒ,"header");string ޱ=ݗ(ݒ,"body");return new ҫ(ц.ч(ޱ),ц.ч(ޥ));}}public class Ҭ:ܧ{public enum ճ{ݡ=200,ռ=201,ߊ
=401,ߋ=404,մ=500,ߌ=600,ߍ=601,ߎ=602,ߏ=603,ߐ=604,}public Ҭ(Dictionary<string,object>ܥ,Dictionary<string,object>ܦ):base(ܥ,ܦ)
{}public override string ѵ()=>"RESPONSE::"+base.ѵ();public static Ҭ ݶ(string ݒ){ݒ=ݒ.Replace("RESPONSE::","");string ޥ=ݗ(ݒ
,"header");string ޱ=ݗ(ݒ,"body");return new Ҭ(ц.ч(ޱ),ц.ч(ޥ));}static public int ޚ(ճ ޘ)=>(int)ޘ;}public class ߓ{public
string ߑ{get;}public Func<ҫ,Ҭ>ߒ{get;}public ߓ(string ȏ,Func<ҫ,Ҭ>ӏ){ߑ=ȏ;ߒ=ӏ;}}public class Ү{public List<ߓ>ߔ=new List<
ߓ>();public Ҭ ނ(string ȏ,ҫ լ){var ҭ=ߔ.FirstOrDefault(Ά=>Ά.ߑ==ȏ);return ҭ?.ߒ?.Invoke(լ);}public void ү(string ȏ,Func<ҫ,Ҭ>ҭ
){ߔ.Add(new ߓ(ȏ,ҭ));}}public class ߕ:ª{А µ;public override string X=>"get";public ߕ(А º){µ=º;}public override
string Ò(Á Â){if(Â.Ã.Count==0)return Ä.Å.Д;string œ=Â.Ã[0];string Ŕ=µ.у(œ);return Ŕ;}}public class ߖ:ª{А µ;public
override string X=>"set";public ߖ(А º){µ=º;}public override string Ò(Á Â){if(Â.Ã.Count==0)return Ä.Å.Д;else if(Â.Ã.Count>=2){
string œ=Â.Ã[0];string Ŕ=Â.Ã[1];µ.ђ(œ,Ŕ);return$"{œ}={Ŕ}";}return Ä.Å.Æ;}}public class А:Ф{public string ߗ{get;set;}=
"";Dictionary<string,object>ߘ=new Dictionary<string,object>();bool ߙ=false;public А(A B):base(B){ߘ=ц.ч(A.Թ.
Storage);}public override void V(){Ý(new ߖ(this));Ý(new ߕ(this));}public string ߚ(){ߗ=ц.ѓ(ߘ);ߙ=false;return ߗ;}public bool Џ(){
ߘ.Clear();ߗ="";return ߙ=true;}public string у(string œ){string Ŕ="";if(ߘ.ContainsKey(œ))Ŕ+=$"{ߘ[œ]}";return Ŕ;}public
bool ђ(string œ,string Ŕ){ߘ[œ]=Ŕ;ߙ=true;return ߙ;}}public class զ:Ф{const int ߛ=30;public List<string>д{get;}=new List<
string>();public զ(A B):base(B){}public void տ(string л){т(л,"Info");}public void ݿ(string л){т(л,"Error");}void т(string л,
string Ȃ=""){string ߜ=DateTime.Now.ToString("HH:mm:ss");string ߝ=Ȃ!=""?"."+Ȃ:"";if(д.Count>ߛ)д.RemoveAt(д.Count-1);д.Insert(0,
$"{ߜ}{ߝ} {л}");}}public class ӈ:ŀ{}public class Ӊ:ŀ{}public class ߞ:Ф{à à;public ߞ(A B):base(B){}public override void V(){à=A.S<à>();
Ҡ<IMyMechanicalConnectionBlock>(ߟ=>ߟ.IsAttached,(Ņ,ű)=>ߠ(Ņ as IMyMechanicalConnectionBlock,ű));}protected void ߠ(
IMyMechanicalConnectionBlock ߟ,object ڊ){var ߡ=ڊ as bool?;var Ң=җ.ContainsKey(ߟ.EntityId)?җ[ߟ.EntityId]as bool?:null;if(ߡ==true&&Ң!=true){ҩ<ӈ>(ߟ);à.
Ӱ(ߟ,"onAttach");à.ԝ(ߟ.TopGrid);}else if(ߡ==false&&Ң==true){ҩ<Ӊ>(ߟ);à.Ӱ(ߟ,"onDetach");à.ԟ();}}}public class ӆ:ŀ{}public
class Ӈ:ŀ{}public class ߢ:Ф{à à;public ߢ(A B):base(B){}public override void V(){à=A.S<à>();Ҡ<IMyShipMergeBlock>(ߣ=>ߣ.State,(Ņ
,ű)=>ߤ(Ņ as IMyShipMergeBlock,ű));}protected void ߤ(IMyShipMergeBlock ߣ,object ڊ){var ę=ڊ as MergeState?;var Ң=җ.
ContainsKey(ߣ.EntityId)?җ[ߣ.EntityId]as MergeState?:null;if(ę.HasValue){switch(ę){case MergeState.None:if(Ң==MergeState.Locked){ҩ<Ӈ
>(ߣ);à.Ӱ(ߣ,"onUnmerge");à.ӑ();}break;case MergeState.Locked:if(Ң!=MergeState.Locked){ҩ<ӆ>(ߣ);à.Ӱ(ߣ,"onMerge");à.ӑ();}
break;}}}public void ߥ(IMyShipMergeBlock ߣ){ߣ.Enabled=true;}public void ߦ(IMyShipMergeBlock ߣ){ߣ.Enabled=false;}public void ߧ
(IMyShipMergeBlock ߣ){if(ߣ.Enabled)ߦ(ߣ);else ߥ(ߣ);}}public class ݱ{static string ߨ="##";public static bool ݴ(
string ݒ)=>ݒ.StartsWith(ߨ);public static string ވ(string ئ,string ݰ=""){if(ݰ=="")return ئ;else{var ߩ=new char[ئ.Length];for(
int u=0;u<ئ.Length;u++)ߩ[u]=(char)(ئ[u]^ݰ[u%ݰ.Length]);return ߨ+new string(ߩ);}}public static string ݲ(string ߪ,string ݰ){ߪ
=ߪ.Substring(ߨ.Length);var ߴ=new char[ߪ.Length];for(int u=0;u<ߪ.Length;u++)ߴ[u]=(char)(ߪ[u]^ݰ[u%ݰ.Length]);return new
string(ߴ);}}public class ߵ:ª{ο µ;public override string X=>"clear";public ߵ(ο º){µ=º;}public override string Ò(Á Â){µ
.ߺ();return"";}}public class ࠀ:ª{ο µ;public override string X=>"print";public ࠀ(ο º){µ=º;}public override string
Ò(Á Â){return Â.Ã[0];}}public class ο:Ф{į į;List<string>ࠁ=new List<string>();List<string>ࠂ=new List<
string>();public ο(A B):base(B){}public override void V(){į=A.S<į>();Ä Ä=A.S<Ä>();Ä.Ý(new ߵ(this));Ä.Ý(new ࠀ(this));}string ࠃ=
"";public string ࠄ()=>ࠃ;public void π(string ݒ){ࠃ+=ݒ+"\n";}public virtual string ࠅ(){string Ï="";string ǫ=(A.C??"").
PadRight(15).Substring(0,15);Ï+=$" {ǫ}{ۊ()}   ({A.S<į>().Շ()})\n"+$" {A.X} *{A.Ķ}\n"+
$"------------------------------------------------------\n"+"";if(ࠃ!="")Ï+=$"{ࠄ()}"+$"------------------------------------------------------\n"+$"";return Ï;}public void ࠈ(){
string ࠆ=$"{ࠅ()}\n"+$"{String.Join("\n",ࠂ.AsEnumerable().Reverse())}";ࠇ(ࠆ);ࠃ="";}public virtual string ۊ(){string ی=A.S<Ы>().Ц
.Count()>0?"M":"   ";string ۍ=A.S<Р>().ۍ.Count()>0?"C":"    ";string ێ=$"{A.S<Ў>().д.Count()}";string ې=A.ۏ?"A":"   ";
string ۑ=A.S<į>().Յ>0?"W":"   ";int ࠉ=A.S<Ä>().թ.Count;string ࠊ=ࠉ>0?$"R{ࠉ}":"   ";return String.Join("  ",ۑ,ې,ۍ,ی,ێ,ࠊ);}public
void Ӟ(string ݒ,bool ࠋ=true){ࠁ.Add(ݒ);string ࠌ=ࠋ&&ݒ.Length>37?ݒ.Substring(0,32)+"..."+ݒ.Substring(ݒ.Length-5):ݒ;ࠂ.Add(ࠌ);if(
ࠂ.Count>20)ࠂ.RemoveRange(0,ࠂ.Count-20);}public virtual void ࠇ(string ݒ){A.Թ.Echo(ݒ);}public bool ߺ(){ࠁ.Clear();ࠂ.Clear();
return ࠁ.Count==0&&ࠂ.Count==0;}}public class A{public MyGridProgram Թ;public string C="Mother Program";public MyCommandLine ࠍ;
public bool ۏ=false;public IMyCubeGrid Н;public IMyGridTerminalSystem ӵ;public IMyIntergridCommunicationSystem ݧ;public
IMyProgrammableBlock ō;public IMyShipController ӷ;public IMyGridProgramRuntimeInfo ࠎ;public long к;public string Ķ;public long ޑ;public
string X;public BoundingSphereD с;public enum ӛ{ࠏ,Ӝ,ӝ,ࠐ,ࠑ,}public ӛ Ӛ=ӛ.ࠏ;public bool ξ=false;public Dictionary<string,ғ>ࠒ=new
Dictionary<string,ғ>();List<ғ>ࠓ=new List<ғ>();public Dictionary<string,ұ>ࠔ=new Dictionary<string,ұ>();public
Dictionary<string,E>ࠕ=new Dictionary<string,E>();public List<Ͻ>Ҙ=new List<Ͻ>();public Dictionary<string,string>և=new Dictionary<
string,string>();public Dictionary<string,string>ٹ=new Dictionary<string,string>();public bool ϼ=false;public A(MyGridProgram
ࠚ){Ń(ࠚ);}public void Ń(MyGridProgram ࠚ){Թ=ࠚ;ݧ=Թ.IGC;ō=Թ.Me;Н=ō.CubeGrid;ӵ=Թ.GridTerminalSystem;ࠎ=Թ.Runtime;к=ݧ.Me;Ķ=
$"{к}".Substring($"{к}".Length-5);ޑ=Н.EntityId;X=ō.CubeGrid.CustomName;ࠤ();}public void ࠤ(){List<ұ>Ж=new List<ұ>{new զ(this),
new О(this),new į(this),new Ҩ(this),new Ä(this),new А(this),new à(this),new Ы(this),new Ў(this),new Р(this),new Ü(this),new
ο(this),new ٿ(this),new ߞ(this),new ߢ(this),};Ж.ForEach(º=>ࠨ(º));}void ࡀ(ӛ ű){Ӛ=ű;}public void V(){ࡀ(ӛ.Ӝ);S<į>().Ի();Ӟ(
$"Booting {C}...");ࡁ();ࡂ();S<į>().ӂ(ࡃ());}IEnumerable<double>ࡃ(){foreach(var ï in ࡄ())yield return ï;ࡀ(ӛ.ӝ);S<à>().Ӱ(ō,"onBoot");S<Ҩ>().ҩ
<λ>();Ӟ($"{C} is online.");Ӟ("Clearing console in 2 seconds...");Ӟ("The Empire must grow.");S<į>().Ձ(()=>S<ο>()?.ߺ(),2.0)
;}IEnumerable<double>ࡄ(){int Ȉ=ࠓ.Count;for(int u=0;u<Ȉ;u++){var º=ࠓ[u];Ӟ($"Booting modules: ({u+1} / {Ȉ})");var ࡅ=º.Қ();
while(ࡅ.MoveNext())yield return ࡅ.Current;ٴ(º.Ҫ());}Ӟ("All modules booted.");}void ࡁ(){double ࡆ=50;с=new BoundingSphereD(Н.
WorldVolume.Center,Н.WorldVolume.Radius+ࡆ);}void ࡂ(){Ý(new Ѝ(this));Ý(new ϻ(this));Ý(new И(this));}public void Q(string O,
UpdateType ࡇ){if(ϼ){ϼ=false;V();return;}if(Ӛ==ӛ.ࠏ)V();else if(Ӛ==ӛ.Ӝ){S<į>().Q();S<ο>()?.ࠈ();}else if(Ӛ==ӛ.ӝ){if((ࡇ&(UpdateType.
Trigger|UpdateType.Terminal|UpdateType.Script))!=0){S<Ä>().Ÿ(O);S<į>().Q();}else if(ࡇ==UpdateType.IGC)S<Р>().ݭ();else{ࡈ();ࡉ();}
S<ο>().ࠈ();S<Ü>().ܐ();}if(ξ){S<ο>().π("Complexity:  "+Թ.Runtime.CurrentInstructionCount.ToString()+"/50000");}}void ࡈ()=>
ࠒ.Values.ToList().ForEach(º=>º.Q());void ࡉ(){}public string N()=>S<А>()?.ߚ();public void ࡊ(E º){ࠕ[º.қ()]=º;ࠒ[º.қ()]=º;ࠓ.
Add(º);}public void D(List<E>Ж){Ж.ForEach(º=>ࡊ(º));}public string қ<ə>()where ə:ғ{foreach(var Ϟ in ࠒ)if(Ϟ.Value is ə)return
Ϟ.Key;return typeof(ə).Name;}public ə S<ə>()where ə:class,ғ{var ࡋ=қ<ə>();ғ º;if(ࠒ.TryGetValue(ࡋ,out º))return º as ə;
return null;}ұ ࠨ(ұ º){ࠔ[º.қ()]=º;ࠒ[º.қ()]=º;ࠓ.Add(º);return º;}public void Ý(Ͻ Â){S<Ä>().Ý(Â);}public void ٴ(List<Ͻ>ח){ח.
ForEach(Â=>Ý(Â));}public void ࡎ(Action ࡌ,double ࡍ){S<į>().Ձ(ࡌ,ࡍ);}public void Ӟ(string ݒ,bool ࠋ=true){ο ࡏ=S<ο>();if(ࡏ==null)Թ.
Echo(ݒ);else ࡏ.Ӟ(ݒ,ࠋ);S<զ>()?.տ(ݒ);}public MatrixD м(){return ӷ?.WorldMatrix??Н.WorldMatrix;}public Vector3D ސ()=>Н.
GetPosition();public Vector3D ޒ(){if(ӷ==null)return Vector3D.Zero;Vector3D ࡐ=ӷ.GetArtificialGravity();if(ࡐ.LengthSquared()==0)ࡐ=ӷ.
GetNaturalGravity();return ࡐ;}public double?ޓ()=>ӷ?.GetShipSpeed();public string ր(string ئ){if(ٹ.Count==0)return ئ;var ࡒ=ٹ.
OrderByDescending(ࡑ=>ࡑ.Key.Length);foreach(var ࡓ in ࡒ)ئ=ئ.Replace("$"+ࡓ.Key,ࡓ.Value);return ئ;}public string ן(string ࡔ,Dictionary<string
,string>Ѐ){if(ࡔ.IndexOf("{{")==-1)return ր(ࡔ);var Ρ=new StringBuilder();int u=0;while(u<ࡔ.Length){int ࡕ=ࡔ.IndexOf("{{",u)
;if(ࡕ==-1){Ρ.Append(ࡔ,u,ࡔ.Length-u);break;}if(ࡕ>u)Ρ.Append(ࡔ,u,ࡕ-u);int ࡖ=ࡔ.IndexOf("}}",ࡕ+2);if(ࡖ==-1){Ρ.Append(ࡔ,ࡕ,ࡔ.
Length-ࡕ);break;}string ࡗ=ࡔ.Substring(ࡕ+2,ࡖ-ࡕ-2);string ࡘ;string ࢠ="";int ś=ࡗ.IndexOf(':');if(ś>=0){ࡘ=ࡗ.Substring(0,ś).Trim();
ࢠ=ࡗ.Substring(ś+1).Trim();}else{ࡘ=ࡗ.Trim();}string Ŕ;if(Ѐ!=null&&Ѐ.ContainsKey(ࡘ))Ŕ=Ѐ[ࡘ];else Ŕ=ࢠ;Ρ.Append(Ŕ);u=ࡖ+2;}
return ր(Ρ.ToString());}}public interface ѧ{string ѵ();}public static class Ê{public static string Ë(string ݒ,params object[]ࢢ
){return string.Format(ݒ,ࢢ);}}public class ц{public static string ѓ(Dictionary<string,object>Ѷ){var ײ=new StringBuilder()
;ײ.Append("{");foreach(var ψ in Ѷ){ײ.Append("\"").Append(ࢣ(ψ.Key)).Append("\":");var ࢤ=ψ.Value as ѧ;if(ࢤ!=null)ײ.Append(ࢤ
.ѵ());else if(ψ.Value is Dictionary<string,object>)ײ.Append(ѓ((Dictionary<string,object>)ψ.Value));else if(ψ.Value is
List<object>)ײ.Append(ࢥ((List<object>)ψ.Value));else if(ψ.Value is string)ײ.Append("\"").Append(ࢣ((string)ψ.Value)).Append(
"\"");else throw new InvalidOperationException("Unsupported value type: "+ψ.Value?.GetType().Name);ײ.Append(",");}if(ײ.
Length>1)ײ.Length--;ײ.Append("}");return ײ.ToString();}public static Dictionary<string,object>ч(string ŏ){var ࢦ=new Dictionary
<string,object>();if(string.IsNullOrEmpty(ŏ)||ŏ[0]!='{'||ŏ[ŏ.Length-1]!='}')return ࢦ;ŏ=ŏ.Substring(1,ŏ.Length-2);int ڱ=ŏ.
Length;int u=0;while(u<ڱ){int ࢧ=ŏ.IndexOf('"',u);if(ࢧ==-1)break;int ࢨ=ŏ.IndexOf('"',ࢧ+1);if(ࢨ==-1)break;string œ=ࢩ(ŏ.Substring
(ࢧ+1,ࢨ-ࢧ-1));int ࢪ=ŏ.IndexOf(':',ࢨ)+1;if(ࢪ==0)break;object Ŕ;if(ŏ[ࢪ]=='{'){int ࢬ=ࢫ(ŏ,ࢪ,'{','}');if(ࢬ==-1)break;string ऄ=ŏ
.Substring(ࢪ,ࢬ-ࢪ+1);Ŕ=ч(ऄ);u=ࢬ+1;}else if(ŏ[ࢪ]=='['){int ࢬ=ࢫ(ŏ,ࢪ,'[',']');if(ࢬ==-1)break;string अ=ŏ.Substring(ࢪ,ࢬ-ࢪ+1);Ŕ=
आ(अ);u=ࢬ+1;}else if(ŏ[ࢪ]=='"'){int ࢬ=ŏ.IndexOf('"',ࢪ+1);while(ࢬ!=-1&&ŏ[ࢬ-1]=='\\'){ࢬ=ŏ.IndexOf('"',ࢬ+1);}if(ࢬ==-1)break;Ŕ
=ࢩ(ŏ.Substring(ࢪ+1,ࢬ-ࢪ-1));u=ࢬ+1;}else{int ࢬ=ŏ.IndexOf(',',ࢪ);if(ࢬ==-1)ࢬ=ŏ.Length;Ŕ=ŏ.Substring(ࢪ,ࢬ-ࢪ).Trim();u=ࢬ;}ࢦ.Add(
œ,Ŕ);u=ŏ.IndexOf(',',u)+1;if(u==0)break;}return ࢦ;}public static string ࢥ(IEnumerable<object>ۼ){var ײ=new StringBuilder()
;ײ.Append("[");foreach(var इ in ۼ){if(इ is List<object>)ײ.Append(ࢥ((List<object>)इ));else if(इ is Dictionary<string,
object>)ײ.Append(ѓ((Dictionary<string,object>)इ));else if(इ is string)ײ.Append("\"").Append(ࢣ((string)इ)).Append("\"");else if
(इ is ѧ)ײ.Append(((ѧ)इ).ѵ());else ײ.Append("\"").Append(ࢣ(इ!=null?इ.ToString():string.Empty)).Append("\"");ײ.Append(",");
}if(ײ.Length>1)ײ.Length--;ײ.Append("]");return ײ.ToString();}public static List<object>आ(string ŏ){var ۼ=new List<object>
();if(string.IsNullOrEmpty(ŏ)||ŏ[0]!='['||ŏ[ŏ.Length-1]!=']')return ۼ;ŏ=ŏ.Substring(1,ŏ.Length-2);int ڱ=ŏ.Length;int u=0;
while(u<ڱ){char ई=ŏ[u];if(ई=='"'){int ࢬ=ŏ.IndexOf('"',u+1);while(ࢬ!=-1&&ŏ[ࢬ-1]=='\\'){ࢬ=ŏ.IndexOf('"',ࢬ+1);}if(ࢬ==-1)break;ۼ.
Add(ࢩ(ŏ.Substring(u+1,ࢬ-u-1)));u=ࢬ+1;}else if(ई=='{'){int ࢬ=ࢫ(ŏ,u,'{','}');if(ࢬ==-1)break;string उ=ŏ.Substring(u,ࢬ-u+1);ۼ.
Add(ч(उ));u=ࢬ+1;}else if(ई=='['){int ࢬ=ࢫ(ŏ,u,'[',']');if(ࢬ==-1)break;string अ=ŏ.Substring(u,ࢬ-u+1);ۼ.Add(आ(अ));u=ࢬ+1;}else
u++;if(u<ڱ&&ŏ[u]==',')u++;}return ۼ;}static int ࢫ(string ŏ,int Ƿ,char ऊ,char ऋ){int Ř=0;for(int u=Ƿ;u<ŏ.Length;u++){if(ŏ[
u]==ऊ)Ř++;else if(ŏ[u]==ऋ)Ř--;if(Ř==0)return u;}return-1;}static string ࢣ(string ऌ)=>ऌ.Replace("\\","\\\\").Replace("\"",
"\\\"");static string ࢩ(string ऌ)=>ऌ.Replace("\\\"","\"").Replace("\\\\","\\");
