// |  Mother Autopilot System (MAPS) - v0.1.0 - 12 May 2026
// |  Agentluke
// |
// |  Mother Documentation
// |  https://lukejamesmorrison.github.io/mother-docs/
// |
// |  Mother Discord
// |  https://discord.com/invite/PrrmBujmXQ
// 
A B;public
 Program(){B=new A(this){C="MAPS",};B.D(new List<E>{new F(B),new G(B),new H(B),new I(B),new J(B),new K(B),new
L(B),});}public void
 Save
(){Storage=B.M();}public void
 Main
(string N,UpdateType O){B.P(N,O);}
}
public class L:Q{public const string R="AlmanacView";S S;T T;U U;List<IMyTextSurface>V=new List<IMyTextSurface>
();public L(A B):base(B){}public override void c(){S=A.W<S>();T=A.W<T>();U=A.W<U>();Y<X>();Z();S.a(b);}public override
void g(d e,object f){if(e is X&&(f is IMyTextPanel||f is IMyTextSurfaceProvider)){Z();}}void Z(){V.Clear();h();i();}void h()
{U.j<IMyTextPanel>()?.ForEach(k=>{MyIni m=U.l(k);foreach(n q in o.p(m)){if(q.r==0&&string.Equals(q.s,R,StringComparison.
OrdinalIgnoreCase)){k.ContentType=ContentType.TEXT_AND_IMAGE;V.Add(k);break;}}});}void i(){var t=new List<IMyTerminalBlock>();t.AddRange(
U.j<IMyCockpit>());t.AddRange(U.j<IMyProgrammableBlock>());t.AddRange(U.j<IMySoundBlock>());t.ForEach(u=>{
IMyTextSurfaceProvider v=u as IMyTextSurfaceProvider;if(v==null)return;MyIni m=U.l(u);foreach(n q in o.p(m)){if(!string.Equals(q.s,R,
StringComparison.OrdinalIgnoreCase))continue;if(q.r>=v.SurfaceCount)continue;IMyTextSurface w=v.GetSurface(q.r);w.ContentType=
ContentType.TEXT_AND_IMAGE;V.Add(w);}});}void b(){var ª=T.x.OrderBy(y=>y.z).ToList();string Ë=string.Join("\n",ª.Select(y=>{bool º=
y.µ();bool Á=y.À();string Ã=y.z??y.Â;string Ä=º?"F":Á?"N":"U";double Ç=Vector3D.Distance(A.Å.GetPosition(),y.Æ);string Ê=
È.É($"({Ä}) {Ã}",$"{Ç:F0}m",30);return Ê;}));V.ForEach(w=>{w.WriteText($"{Ë}",false);});}}public class G:Q{
IMyShipController Ì;List<IMyGyro>Í;bool Î=false;const double Ï=1;const double Ð=180/Math.PI;const double Ñ=1;IMyTerminalBlock Ò=null;
const double Ó=0.4;Vector3D?Ô=null;Õ Ø=Õ.Ö;public enum Õ{Ö,Ù,Ú,Û,Ü}public G(A B):base(B){}public override void c(){Ì=A.Ý;Ò=Ì;
Í=A.W<U>().j<IMyGyro>();Þ(new ß(this));}public override void P(){if(Ô.HasValue&&Î)à(Ò,Ô.Value);}public void Ù(double á){
if(á==0)return;Ø=Õ.Ù;Vector3D ã=â(Ì.WorldMatrix.Up,á);Ü(ã);}public void Ú(double á){if(á==0)return;Ø=Õ.Ú;Vector3D ã=â(Ì.
WorldMatrix.Right,-á);Ü(ã);}public void Û(double á){if(á==0)return;Ø=Õ.Û;Vector3D ã=â(Ì.WorldMatrix.Forward,á);Ü(ã);}Vector3D â(
Vector3D ä,double á){Vector3D å=Ì.GetPosition();Vector3D æ=Ì.WorldMatrix.Forward;Vector3D ç;ä=Vector3D.Normalize(ä);double è=
MathHelper.ToRadians(-á);if(Math.Abs(á)>=179.9){ç=-æ;Vector3D é;if(Ø==Õ.Ù)é=Ì.WorldMatrix.Right;else if(Ø==Õ.Ú)é=Ì.WorldMatrix.Up;
else é=Ì.WorldMatrix.Forward;ç+=é*0.01;ç=Vector3D.Normalize(ç);return å+(ç*100);}MatrixD ê=MatrixD.CreateFromAxisAngle(ä,è);
ç=Vector3D.TransformNormal(æ,ê);if(Ø==Õ.Ù)ç=Vector3D.Reject(ç,Ì.WorldMatrix.Up);else if(Ø==Õ.Ú)ç=Vector3D.Reject(ç,Ì.
WorldMatrix.Right);else ç=Vector3D.Reject(ç,Ì.WorldMatrix.Forward);ç=Vector3D.Normalize(ç);return å+(ç*100);}public void Ü(Vector3D
ë,IMyTerminalBlock ì=null){Ô=ë;Ò=ì??Ì;Ø=Õ.Ü;Î=true;à(Ò,ë);}void à(IMyTerminalBlock Ò,Vector3D í){if(!Ô.HasValue||!Î){î();
return;}if(Ò==null){î();return;}ï(new ð(),null);Vector3D ñ=Ò.GetPosition();Vector3D ò=Vector3D.Normalize(í-ñ);Vector3D ó=Ò.
WorldMatrix.Forward;Vector3D ô=Ò.WorldMatrix.Up;Vector3D õ=Ò.WorldMatrix.Right;if(Ø==Õ.Ù)ò=Vector3D.Reject(ò,ô);else if(Ø==Õ.Ú)ò=
Vector3D.Reject(ò,õ);else if(Ø==Õ.Û)ò=Vector3D.Reject(ò,ó);ò=Vector3D.Normalize(ò);double ö=0,ø=0,ù=0;ú(ò,Ò.WorldMatrix,out ö,
out ø);if(Ø==Õ.Ù){ø=0;ù=0;}else if(Ø==Õ.Ú){ö=0;ù=0;}else if(Ø==Õ.Û){ö=0;ø=0;}double û=ö*Ï*Ñ;double ü=ø*Ï*Ñ;double ý=ù*Ï*Ñ;þ
(ü,û,ý,Ò);double ā=ÿ.Ā(ó,ò)*Ð;if(ā<Ó){Ă();}}void Ă(){î();Ô=null;ï(new ă(),null);}void ú(Vector3D ë,MatrixD Ą,out double ą
,out double Ć){var ć=Vector3D.TransformNormal(ë,MatrixD.Transpose(Ą));var Ĉ=new Vector3D(0,ć.Y,ć.Z);Ć=ÿ.Ā(Vector3D.
Forward,Ĉ)*Math.Sign(ć.Y);if(Math.Abs(Ć)<1E-6&&ć.Z>0)Ć=Math.PI;if(Vector3D.IsZero(Ĉ))ą=MathHelper.PiOver2*Math.Sign(ć.X);else ą
=ÿ.Ā(ć,Ĉ)*Math.Sign(ć.X);}public void î(){foreach(var ĉ in Í){ĉ.GyroOverride=false;}Î=false;Ø=Õ.Ö;}void þ(double Ċ,double
ċ,double Č,IMyTerminalBlock Ò){var č=new Vector3D(-Ċ,ċ,Č);var Ď=Ò.WorldMatrix;var ď=Vector3D.TransformNormal(č,Ď);foreach
(var Đ in Í){var đ=Vector3D.TransformNormal(ď,Matrix.Transpose(Đ.WorldMatrix));Đ.Pitch=(float)đ.X;Đ.Yaw=(float)đ.Y;Đ.Roll
=(float)đ.Z;Đ.GyroOverride=true;}}}public class ß:Ē{public G ē;public override string Ĕ=>"gyro/face";public ß(G
ĕ){ē=ĕ;}public override string Ġ(Ė ė){if(ė.Ę.Count==0)return ę.Ě.ě;Ĝ Ğ=new ĝ(ė.Ę[0]);ē.Ü(Ğ.ğ());return
"Rotating towards waypoint.";}}public class ă:d{}public class ð:d{}public class ġ:Ē{J ē;public override string Ĕ{get;}="dock";public ġ(J ĕ)
{ē=ĕ;}public override string Ġ(Ė ė){if(ė.Ę.Count==0)return ę.Ě.ě;else{string Ģ=ė.Ę[0];string Ĥ=ė.ģ("local");string ĥ=ė.ģ(
"remote");ē.Ħ(Ĥ);if(ē.ħ!=null){ē.Ĩ(Ģ,ĥ);return$"Requesting docking: {Ģ}@{ĥ}";}return$"Connector not found: {Ĥ}";}}}public class
J:Q{ĩ ī=ĩ.Ī;Vector3D Ĭ;Vector3D ĭ;Vector3D Į;string į;public IMyShipConnector ħ;G İ;ę ę;U U;T T;H ı;Ĳ Ĳ;ĳ ĳ;List<
IMyThrust>Ĵ=new List<IMyThrust>();double ĵ=0.5;int Ķ=0;Dictionary<ĩ,string>Ŀ=new Dictionary<ĩ,string>(){{ĩ.Ī,"Idle"},{ĩ.ķ,
"Incoming"},{ĩ.ĸ,"Approaching"},{ĩ.Ĺ,"Aligning"},{ĩ.ĺ,"Closing"},{ĩ.Ļ,"Opening"},{ĩ.ļ,"Departing"},{ĩ.Ľ,"Docked"},{ĩ.ľ,"Jolt"}};
enum ĩ{Ī,ķ,ĸ,Ĺ,ĺ,Ļ,ļ,Ľ,ľ}public J(A B):base(B){}public override void c(){ę=W<ę>();U=W<U>();T=W<T>();Ĳ=W<Ĳ>();ı=W<H>();ĳ=W<ĳ>
();İ=W<G>();Ĵ=U.j<IMyThrust>();Þ(new ġ(this));Y<ŀ>();Y<Ł>();Y<ł>();Y<ð>();Y<ă>();Y<Ń>();ń("dock",(Ņ)=>W<J>().ņ(Ņ));}
public override void P(){if(ī!=ĩ.Ī&&ī!=ĩ.Ľ)ĳ.Ň($"State: Docking.{Ŀ[ī]}");if(ī==ĩ.ľ){}else if(ī==ĩ.Ĺ){İ.Ü(Ĭ,ħ);}else if(ī==ĩ.ĺ)
{ň();İ.Ü(Ĭ,ħ);}}Vector3D ŋ(Dictionary<string,object>ŉ,string Ŋ){return new Vector3D(double.Parse($"{ŉ[$"{Ŋ}.x"]}"),double
.Parse($"{ŉ[$"{Ŋ}.y"]}"),double.Parse($"{ŉ[$"{Ŋ}.z"]}"));}public Ō ņ(ō Ņ){A.Ŏ(
$"Docking request: {Ņ.ŏ("OriginName")}@{Ņ.Ő("remote")}");Dictionary<string,object>œ=ő(Ņ.Œ("remote"));if(œ==null)return Ĳ.Ŕ(Ņ,Ō.ŕ.Ŗ);return Ĳ.Ŕ(Ņ,Ō.ŕ.ŗ,œ);}void ň(){Vector3D Ř=
Vector3D.Normalize(Ĭ-ĭ);Vector3D ř=A.Ý?.GetShipVelocities().LinearVelocity??Vector3D.Zero;Vector3D ś=A.Ś();double Ŝ=Vector3D.Dot
(ś,Ř);double ŝ=A.Ý?.CalculateShipMass().PhysicalMass??0;double Ş=ŝ*Ŝ;const double ş=20000;double Š=ŝ/ş;double š=Vector3D.
Dot(ř,Ř);double Ţ=ĵ-š;double ţ=1.1*ĵ;double Ť=10000.0;double ť=Ť*Š;double Ŧ=Ţ>0?Math.Min(ť*Ţ,100000):0;Ŧ+=Ş;Ŧ=MathHelper.
Clamp(Ŧ,0,100000);ŧ(Ř,Ŧ);Vector3D Ũ=ĭ+Ř*Vector3D.Dot(ħ.GetPosition()-ĭ,Ř);Vector3D ũ=ħ.GetPosition()-Ũ;if(ũ.LengthSquared()<
0.0001){A.Ŏ("No significant drift detected.");return;}Vector3D Ū,ū;ÿ.Ŭ(Ř,out Ū,out ū);double ŭ=Vector3D.Dot(ũ,Ū);double Ů=
Vector3D.Dot(ũ,ū);double ů=Vector3D.Dot(ř,Ū);double Ű=Vector3D.Dot(ř,ū);double ű=20000000.0;double Ų=500000.0;double ų=ű*Š;
double Ŵ=Ų*Š;double ŵ=-ų*ŭ-Ŵ*ů;double Ŷ=-ų*Ů-Ŵ*Ű;ŵ=MathHelper.Clamp(ŵ,-100000,100000);Ŷ=MathHelper.Clamp(Ŷ,-100000,100000);ŧ(Ū
,ŵ);ŧ(ū,Ŷ);Vector3D ŷ=ħ.GetPosition()-Ĭ;double Ÿ=Vector3D.Distance(ħ.GetPosition(),Ĭ);bool Ź=Ÿ<2;Vector3D Ż=ħ.GetPosition
()-A.ź();double ż=Ż.Length();Vector3D Ž=ĭ-Ĭ;double ž=Ž.Length();float ſ=(float)Math.Atan(ż/ž);float ƀ=ſ*(180f/(float)Math
.PI);double Ɓ=Ÿ*Math.Sin(ſ);Vector3D Ƃ=Vector3D.Normalize(Į);Vector3D ƃ=Vector3D.Reject(ŷ,Ƃ);double Ƅ=ƃ.Length();if(!Ź&&Ƅ
>Ɓ){ƅ($"Drift exceeds {ƀ}° cone — Resetting.");return;}if(!Ź&&š>ţ){ƅ($"Speed {š:F2} m/s exceeds safe limit — Resetting.")
;return;}const double Ɔ=0.02;const int Ƈ=30;if(š<Ɔ){Ķ++;if(Ķ>=Ƈ){Ķ=0;ľ();return;}}else{Ķ=0;}string ƈ=Ķ>0?
$"\n{new String('>',Ķ)}":"";ĳ.Ň($""+$"Distance: {Ÿ:F2} m\n"+$"Drift: {Ƅ:F2} m / {Ɓ:F2} m ({ƀ:F2}°)\n"+$"Fwd: {š:F2} m/s ({Ŧ:F0} N)"+$"{ƈ}");}
public void Ɯ(Ɖ Ɗ){string Ģ=Ɗ.Œ("grid.name");if(Ɗ.Ƌ("status")!=$"{Ō.ƌ(Ō.ŕ.ŗ)}"){A.Ŏ(
$"Docking request failed: {Ɗ.Ƌ("status")} {Ɗ?.Œ("message")??""}");return;}ƍ Ƒ=ı.Ǝ().Find(Ə=>Ə.Ɛ.Â==Ɗ.Ƌ("OriginId"));if(Ƒ==null){A.Ŏ("Docking procedure not found",false);return;}Ƒ.ƒ=Ɗ.Œ
("connector.name");Ƒ.ī=Ɠ.Ɣ;A.Ŏ($"Docking: {ħ.CustomName} ->\n{Ģ}@{Ɗ.Ő("connector.name")}",false);ĭ=ŋ(Ɗ.ƕ,"app");Ĭ=ŋ(Ɗ.ƕ,
"connector");Į=ŋ(Ɗ.ƕ,"connector.fwd");Vector3D Ɩ=A.ź();Vector3D Ɨ=Ĭ-ħ.GetPosition();Vector3D Ƙ=Vector3D.Normalize(Ɨ);Vector3D ƙ=ĭ+Į
*8;string ƚ=$""+$"GPS:"+$"APP.{Ģ.Replace(" ","")}:"+$"{ƙ.X}:"+$"{ƙ.Y}:"+$"{ƙ.Z}:"+$"#FF75C9F1:";ī=ĩ.ķ;į=
$"nav/set-flight-plan \"{ƚ}\"; fcs/start";ę.ƛ(į);ĳ.Ň($"MOVING TO DOCK - Grid: {Ģ}");}void ƅ(string Ɲ=""){ī=ĩ.ķ;ƞ();İ.î();Ķ=0;if(Ɲ!=""){A.Ŏ(Ɲ,false);A.W<Ɵ>()?.Ơ(Ɲ
);}ę.ƛ(į);}void ŧ(Vector3D ơ,double Ƣ){if(Ƣ==0)return;foreach(var ƣ in Ĵ){double Ƥ=Vector3D.Dot(ƣ.WorldMatrix.Backward,ơ)
;if(Ƥ>0.1){float ƥ=(float)(Ƣ*Ƥ);ƣ.ThrustOverride=ƥ;}}}void ƞ(){foreach(var ƣ in Ĵ){ƣ.ThrustOverride=0f;}}public void Ħ(
string Ĥ=""){ħ=U.j<IMyShipConnector>().Where(Ʀ=>(string.IsNullOrEmpty(Ĥ)||Ʀ.CustomName==Ĥ)&&Ʀ.Status==MyShipConnectorStatus.
Unconnected&&Ʀ.IsWorking).FirstOrDefault();}public void Ĩ(string Ƨ,string ĥ=""){ƨ ƪ=T.Ʃ(Ƨ);if(ƪ!=null){ō Ņ=Ĳ.ƫ("dock",new
Dictionary<string,object>(){{"local",ħ?.CustomName??""},{"remote",ĥ}});Ņ.Ƭ(ƪ);ƍ Ƒ=new ƍ(){Ɛ=ƪ,ƒ=ĥ,ħ=ħ,ī=Ɠ.ƭ};ı.Ʈ(Ƒ);Ĳ.Ư(ƪ.ư(),Ņ,Ɯ)
;}}public Dictionary<string,object>ő(string Ĥ=""){Vector3D å=A.Å.GetPosition();Ħ(Ĥ);IMyShipConnector Ʊ=ħ;if(Ʊ==null)
return null;Vector3D Ʋ=Ʊ.GetPosition();MatrixD Ƴ=Ʊ.WorldMatrix;Vector3D ƴ=Ƴ.Forward;Vector3D Ƶ=Ƴ.Up;Vector3D ƶ=A.Å.WorldMatrix
.Up;double Ʒ;double Ƹ=double.TryParse(U.l(ħ)?.Get("general","appDistance").ToString(),out Ʒ)?Ʒ:30.0;Vector3D Ô=Ʋ+(ƴ*Ƹ);
Dictionary<string,object>ƹ=new Dictionary<string,object>{{"grid.x",$"{å.X}"},{"grid.y",å.Y.ToString()},{"grid.z",å.Z.ToString()},{
"grid.name",A.Ĕ},{"grid.id",A.Â.ToString()},{"connector.x",Ʋ.X.ToString()},{"connector.y",Ʋ.Y.ToString()},{"connector.z",Ʋ.Z.
ToString()},{"connector.fwd.x",ƴ.X.ToString()},{"connector.fwd.y",ƴ.Y.ToString()},{"connector.fwd.z",ƴ.Z.ToString()},{
"connector.name",Ʊ.CustomName},{"app.x",Ô.X.ToString()},{"app.y",Ô.Y.ToString()},{"app.z",Ô.Z.ToString()},{"approach.distance",Ƹ.
ToString()}};return ƹ;}public override void g(d e,object f){if(e is Ł&&ī==ĩ.ĺ){var Ʊ=(IMyShipConnector)f;Ʊ.Connect();ī=ĩ.Ľ;Ĭ=
Vector3D.Zero;T.x.RemoveAll(y=>y.z!=null&&y.z.StartsWith("APP."));A.ƺ(()=>{ƞ();İ.î();ı.ƻ.Clear();},1);}else if((e is ł||e is Ń)
&&ī==ĩ.ķ){ī=ĩ.Ĺ;}else if(ī==ĩ.Ĺ&&e is ă){ī=ĩ.ĺ;}}public void ľ(){ī=ĩ.ľ;İ.Ù(10);A.ƺ(()=>İ.Ù(-10),2);A.ƺ(()=>ī=ĩ.ĺ,4);}}
public enum Ɠ{Ƽ,ƭ,Ɣ,ƽ,ƾ,ƿ}public class ƍ{public ƨ Ɛ;public string ƒ;public IMyShipConnector ħ;public Ɠ ī=Ɠ.Ƽ;}public class Ń:d
{public ƍ ǀ{get;}public Ń(ƍ Ƒ){ǀ=Ƒ;}}public class ǁ:Ē{I ē;public override string Ĕ=>"fcs/start";public ǁ(I ĕ){ē=
ĕ;}public override string Ġ(Ė ė){string ǂ=ė.ģ("speed");if(ǂ!="")ē.ǃ(float.Parse(ǂ));ē.Ǆ();return"Autopilot engaged";}}
public class ǅ:Ē{I ē;public override string Ĕ=>"fcs/stop";public ǅ(I ĕ){ē=ĕ;}public override string Ġ(Ė ė){ē.ǆ();
return"Autopilot disengaged";}}public class Ǉ:d{}public class ǈ:d{}public class I:Q{public IMyCockpit ǉ;H ı;IMyRemoteControl Ì
;public bool Ǌ=false;public bool ǋ=false;float ǌ=0f;public I(A B):base(B){}public override void c(){ı=A.W<H>();Þ(new ǁ(
this));Þ(new ǅ(this));Ì=A.Ý as IMyRemoteControl;Ǎ();if(Ì!=null){Ì.ClearWaypoints();Ì.SetAutoPilotEnabled(false);}else{A.Ŏ(
"WARNING: No Remote Control block found. Autopilot unavailable.");}}public override void P(){ǎ();}void Ǎ(){var Ǐ=W<U>().j<IMyCockpit>();ǉ=Ǐ.FirstOrDefault(Ʀ=>Ʀ.IsMainCockpit)??Ǐ.
FirstOrDefault();}public void ǃ(float ǐ){ǌ=ǐ;}void ǎ(){if(Ì==null)return;MyShipVelocities Ǒ=Ì.GetShipVelocities();double ǒ=Math.Round(
Ǒ.LinearVelocity.X,2);double Ǔ=Math.Round(Ǒ.LinearVelocity.Y,2);double ǔ=Math.Round(Ǒ.LinearVelocity.Z,2);bool Ǖ=ǒ!=0||Ǔ
!=0||ǔ!=0;if(Ǖ&&!Ǌ){Ǌ=true;ï<Ǉ>();}else if(!Ǖ&&Ǌ){Ǌ=false;ï<ǈ>();}}public void Ǘ(bool ǖ=true){if(ǉ==null)return;ǉ.
DampenersOverride=ǖ;}public void Ǆ(){if(ǌ>0)Ì.SpeedLimit=ǌ;Ì.FlightMode=FlightMode.OneWay;Ì.SetAutoPilotEnabled(true);Ì.
SetCollisionAvoidance(true);ı?.ǘ();ǋ=true;A.ǋ=true;}public void ǆ(){Ì?.SetAutoPilotEnabled(false);Ǘ();A.W<F>()?.Ǚ();A.W<G>()?.î();ı?.ǚ();ǋ=
false;A.ǋ=false;}}public class Ǜ:Ē{H ē;public override string Ĕ=>"fp/set";public Ǜ(H ĕ){ē=ĕ;}public override string
Ġ(Ė ė){if(ė.Ę.Count==0)return ę.Ě.ě;ē.ǜ(ė.Ę[0]);return"Flight plan set.";}}public class ł:d{}public class ǝ:d{}public
class Ǟ:d{}public class ǟ:d{}public class Ǡ:d{}public class H:Q{ǡ ǡ;Ɵ Ɵ;T T;public Ǣ Ǣ;Dictionary<string,ǣ>Ǥ=new
Dictionary<string,ǣ>();public List<ƍ>ƻ=new List<ƍ>();public ǣ ǥ=null;List<Ĝ>Ǧ=new List<Ĝ>();public Ĝ ǧ=null;public Ĝ Ǩ=null;int ǩ=
0;public const double Ǫ=1;const string ǫ="cfgwp:";Vector3D?Ǭ;Ĝ ǭ;public Ǯ.ǯ ǰ;IMyRemoteControl Ì;public H(A B):base(B){Ǣ=
new Ǣ();}public override void c(){ǡ=A.W<ǡ>();Ɵ=A.W<Ɵ>();T=A.W<T>();Ì=A.W<U>().j<IMyRemoteControl>().FirstOrDefault();Ǳ();ǡ.
Y<Ǉ>(this);ǡ.Y<ǈ>(this);ǡ.Y<Ǟ>(this);ǡ.Y<ǟ>(this);ǡ.Y<ǝ>(this);A.W<ę>().Þ(new Ǜ(this));}void Ǳ(){ǲ ǳ=A.W<ǲ>();int ǵ=Ǵ();
if(ǵ>0)Ƕ();if(ǳ==null)return;List<MyIniKey>Ƿ=new List<MyIniKey>();ǳ.Ǹ.GetKeys("waypoints",Ƿ);foreach(MyIniKey ǹ in Ƿ){
string Ǻ=ǹ.Name.Trim();string ǻ=$"{ǳ.Ǹ.Get("waypoints",ǹ.Name)}".Replace("\r","").Trim();if(string.IsNullOrWhiteSpace(Ǻ)||
string.IsNullOrWhiteSpace(ǻ))continue;ĝ Ğ;try{Ğ=new ĝ(ǻ);}catch(Exception){Ɵ.Ǽ($"Invalid config waypoint: {Ǻ}");continue;}if(
string.IsNullOrWhiteSpace(Ğ.ǽ())){Ɵ.Ǽ($"Invalid config waypoint: {Ǻ}");continue;}int ǿ=Ǿ(Ǻ);if(ǿ>0)Ƕ();T.Ȁ(new ƨ($"{ǫ}{Ǻ}",
"waypoint",Ğ.ğ(),0){z=Ǻ});}}int Ǵ(){int ȁ=0;for(int Ȃ=T.x.Count-1;Ȃ>=0;Ȃ--){ƨ ƪ=T.x[Ȃ];if(ƪ.ȃ=="waypoint"&&ƪ.Â.StartsWith(ǫ)){T.x.
RemoveAt(Ȃ);ȁ++;}}return ȁ;}int Ǿ(string Ǻ){int ȁ=0;for(int Ȃ=T.x.Count-1;Ȃ>=0;Ȃ--){ƨ ƪ=T.x[Ȃ];if(ƪ.ȃ=="waypoint"&&!ƪ.Â.
StartsWith(ǫ)&&(ƪ.Â==Ǻ||ƪ.z==Ǻ)){T.x.RemoveAt(Ȃ);ȁ++;}}return ȁ;}void Ƕ(){Dictionary<string,object>Ȅ=new Dictionary<string,object>
();foreach(ƨ ƪ in T.x)Ȅ[ƪ.Â]=ƪ;A.W<ȅ>().Ȇ("almanac",ȇ.Ȉ(Ȅ));}public ƨ Ȋ(string Ǻ){return T.ȉ("waypoint").FirstOrDefault(ƪ
=>ƪ.Â==$"{ǫ}{Ǻ}");}public override void P(){if(!ȋ()){Ȍ();return;}if(ȍ(Ǩ)){Ȍ();ǡ.ï<ǟ>(Ǩ.ǽ());}}bool ȋ(){return Ǩ!=null&&ǥ!=
null&&ǥ.Ȏ==Ǯ.ȏ.Ȑ;}void Ȍ(){Ǭ=null;ǭ=null;}bool ȍ(Ĝ Ğ){if(Ğ==null)return false;Vector3D å=A.ź();if(ǭ!=Ğ){ǭ=Ğ;Ǭ=å;}bool Ȓ=ȑ(Ğ)
<Ǫ;if(!Ȓ&&Ǭ.HasValue)Ȓ=ȓ(Ǭ.Value,å,Ğ.ğ());Ǭ=å;return Ȓ;}bool ȓ(Vector3D Ȕ,Vector3D ȕ,Vector3D Ȗ){Vector3D ȗ=ȕ-Ȕ;double Ș=
ȗ.LengthSquared();if(Ș<=0.000001)return false;double ș=MathHelper.Clamp(Vector3D.Dot(Ȗ-Ȕ,ȗ)/Ș,0,1);Vector3D Ț=Ȕ+(ȗ*ș);
return Vector3D.Distance(Ț,Ȗ)<=ț();}double ț(){BoundingBoxD Ȝ=A.Ý?.WorldAABB??new BoundingBoxD();Vector3D ȝ=(Ȝ.Max-Ȝ.Min)/2;
return Ǫ+ȝ.Length();}public void Ʈ(ƍ Ƒ){ƻ.Add(Ƒ);}public bool Ȟ()=>ƻ.Count>0;public List<ƍ>Ǝ()=>ƻ;public void ǜ(string ȟ){Ǣ.Ƞ(
);ȡ(new Ǯ(ȟ,this));}void ȡ(ǣ Ȣ){ǥ=Ȣ;ȣ(Ǯ.ȏ.Ȥ);ȥ();Ǧ=Ȣ.Ȧ();Ǧ.ForEach(Ğ=>ȧ((ĝ)Ğ));Ȩ();if(Ì!=null)Ì.FlightMode=FlightMode.
OneWay;ȩ(Ǯ.ǯ.Ƭ);}void ȧ(ĝ Ğ){ƨ Ȫ=T.Ʃ(Ğ.ǽ());string ȫ=Ȫ!=null&&Ȫ.ȃ=="waypoint"?Ȫ.Â:Ğ.ǽ();string Ȭ=Ȫ!=null&&!string.
IsNullOrWhiteSpace(Ȫ.z)?Ȫ.z:Ğ.ǽ();ƨ ƪ=new ƨ(ȫ,"waypoint",Ğ.ğ(),0);ƪ.z=Ȭ;T.Ȁ(ƪ);}public void Ȩ(){ȭ();Ȯ();}void ȭ(){ǧ=Ǩ;}void Ȯ(){while(ǩ<Ǧ.
Count){Ǩ=Ǧ[ǩ];ǩ++;if(ȑ(Ǩ)<Ǫ){ȯ(Ǩ.ǽ());continue;}Ì?.ClearWaypoints();Ì?.AddWaypoint(new MyWaypointInfo(Ǩ.ǽ(),Ǩ.ğ()));if(ǥ!=
null&&ǥ.Ȏ==Ǯ.ȏ.Ȑ)Ì?.SetAutoPilotEnabled(true);ǡ.ï<Ǡ>(Ǩ);Ɵ.Ơ($"Next wpt: {Ǩ.ǽ()}");return;}}public void Ȱ(string Ã,ǣ Ȣ){Ǥ.Add
(Ã,Ȣ);}void ȱ(){ǥ=null;}public ĝ ȳ(string Ȳ){ĝ Ğ=new ĝ(Ȳ);ȧ(Ğ);return Ğ;}public void ȥ(){Ǩ=null;ǧ=null;ǩ=0;Ȍ();}public
bool ȴ(){return ǩ==Ǧ.Count&&Ǧ.Count!=0;}void ȩ(Ǯ.ǯ ơ){ǰ=ơ;}void ȣ(Ǯ.ȏ ȵ){if(ǥ!=null)ǥ.Ȏ=ȵ;}public void ǘ(){ȣ(Ǯ.ȏ.Ȑ);}public
void ǚ(){ȣ(Ǯ.ȏ.Ƽ);}public void ȷ(){ȣ(Ǯ.ȏ.ȶ);ǡ.ï<ł>(ǥ);ȥ();Ǣ.Ƞ();Ì?.SetAutoPilotEnabled(false);A.ǋ=false;Ì?.ClearWaypoints();
ȱ();Ɵ.Ơ("Flight plan complete");}void Ƚ(){ǡ.ï<ǝ>(ǥ);if(ǥ.ȸ())ȹ();else if(ǥ.Ⱥ())Ȼ();else if(Ȟ())ȼ();else ȷ();}void Ȼ(){if(
ǰ==Ǯ.ǯ.Ƭ){ȥ();Ǧ=ǥ.Ⱦ();ȩ(Ǯ.ǯ.ȿ);Ȩ();}else if(ǰ==Ǯ.ǯ.ȿ)ȷ();}void ȹ(){ȥ();if(ǰ==Ǯ.ǯ.Ƭ){Ǧ=ǥ.Ⱦ();ȩ(Ǯ.ǯ.ȿ);}else if(ǰ==Ǯ.ǯ.ȿ){Ǧ
=ǥ.Ȧ();ȩ(Ǯ.ǯ.Ƭ);}Ȩ();}void ȼ(){ƍ Ƒ=Ǝ()[0];Ƒ.ī=Ɠ.ƽ;ǡ.ï(new Ń(Ƒ),null);}double ȑ(Ĝ Ğ){if(Ğ==null)return double.MaxValue;
BoundingBoxD Ȝ=A.Ý?.WorldAABB??new BoundingBoxD();Vector3D Ț=ÿ.ɀ(Ğ.ğ(),Ȝ);return Vector3D.Distance(Ț,Ğ.ğ());}public override void g(
d e,object f){if(e is ǟ){A.W<Ɵ>()?.Ơ($"Wpt reached: {f}");if(ǥ!=null&&ǥ.Ȏ==Ǯ.ȏ.Ȑ){ȯ($"{f}");if(ȴ())Ƚ();else Ȩ();}}}public
void ɂ(Ĝ Ğ,string Ɂ){Ǣ.ɂ(Ğ,Ɂ);}public void ȯ(string Ƀ){string ė=Ǣ.Ʉ(Ƀ);if(ė!=""){A.W<ę>().ƛ(ė);Ǣ.Ʌ(Ƀ);}}}public class Ǯ:ǣ{
H ı;T T;public enum ǯ{Ƭ,ȿ}public enum ȏ{Ȥ,Ȑ,Ƽ,ȶ}List<string>Ɇ;bool ɇ=false;bool Ɉ=false;const string ɉ=
"#FF75C9F1";string Ɋ;public List<Ĝ>Ǧ{get;}=new List<Ĝ>();public ȏ Ȏ{get;set;}=ȏ.Ƽ;public Ǯ(string ȟ,H ɋ){ı=ɋ;T=ı.A.W<T>();Ɍ(ȟ);}
public bool Ⱥ()=>ɇ;public bool ȸ()=>Ɉ;public void Ɍ(string ȟ){Ɋ=ȟ.Trim();Ɇ=ɍ(ȟ);Ɏ();}List<string>ɍ(string ȟ){ȟ=System.Text.
RegularExpressions.Regex.Replace(ȟ,@"\s+"," ").Trim();List<string>ɏ=new List<string>();StringBuilder ɐ=new StringBuilder();int ɑ=0;foreach
(char Ʀ in ȟ){if(Ʀ=='{'){if(ɑ==0&&ɐ.Length>0){ɏ.Add(ɐ.ToString().Trim());ɐ.Clear();}ɑ++;}ɐ.Append(Ʀ);if(Ʀ=='}'){ɑ--;if(ɑ
==0){ɏ.Add(ɐ.ToString().Trim());ɐ.Clear();}}else if(ɑ==0&&Ʀ==' '){if(ɐ.Length>0){ɏ.Add(ɐ.ToString().Trim());ɐ.Clear();}}}
if(ɐ.Length>0)ɏ.Add(ɐ.ToString().Trim());return ɏ;}void Ɏ(){Ĝ ɒ=null;foreach(string ɓ in Ɇ){string ɔ=ɓ.Trim();if(ɔ.
StartsWith("{")&&ɔ.EndsWith("}")){string ɕ=ɔ.Substring(1,ɔ.Length-2).Trim();if(ɒ!=null)ɖ(ɒ,ɕ);else ɗ(ɕ);}else if(ɘ(ɔ)){ə(ɔ);}else
ɒ=ɚ(ɔ);}}Ĝ ɚ(string ɛ){if(string.IsNullOrWhiteSpace(ɛ))return null;Ĝ Ğ=null;if(ɛ.StartsWith("GPS:"))Ğ=ı.ȳ(ɛ);else if(ɛ.
Length>1){ƨ ƪ=ı.Ȋ(ɛ)??T.Ʃ(ɛ);if(ƪ!=null){if(ƪ.ȃ=="waypoint")Ğ=ɜ(ƪ);else if(ƪ.ȃ=="grid")Ğ=ɝ(ƪ);}}if(Ğ!=null)Ǧ.Add(Ğ);return Ğ;}
Ĝ ɜ(ƨ ƪ){return new ĝ($"GPS:"+$"{ƪ.z??ƪ.Â}:"+$"{ƪ.Æ.X}:"+$"{ƪ.Æ.Y}:"+$"{ƪ.Æ.Z}:"+$"{ɉ}:");}public Ĝ ɝ(ƨ ƪ){string Ƀ="S."+
(ƪ.z??ƪ.Â);Ĝ ɞ=Ǧ.LastOrDefault();Vector3D ɟ=ɞ==null?ı.A.Å.GetPosition():ɞ.ğ();var ɢ=ÿ.ɠ(new VRageMath.BoundingSphereD(ƪ.Æ
,ƪ.ɡ),ɟ);return ı.ȳ($"GPS:"+$"{Ƀ}:"+$"{ɢ.X}:"+$"{ɢ.Y}:"+$"{ɢ.Z}:"+$"{ɉ}:");}void ɖ(Ĝ Ğ,string ɣ){ɤ(Ğ,ɣ);}void ɗ(string ɣ)
{ı.A.W<ę>().ƛ(ɣ);}bool ɘ(string ɓ)=>(ɓ=="C"||ɓ=="R");void ə(string ɓ){if(ɓ=="C")Ɉ=true;else if(ɓ=="R")ɇ=true;}void ɤ(Ĝ Ğ,
string ė){ı.ɂ(Ğ,ė);}public List<Ĝ>Ȧ()=>Ǧ;public List<Ĝ>Ⱦ(){List<Ĝ>ɥ=new List<Ĝ>();for(int ɦ=Ǧ.Count-1;ɦ>=0;ɦ--)ɥ.Add(Ǧ[ɦ]);
return ɥ;}}public interface ǣ{Ǯ.ȏ Ȏ{get;set;}bool Ⱥ();bool ȸ();List<Ĝ>Ȧ();List<Ĝ>Ⱦ();}public class Ǣ{public Dictionary<Ĝ,
string>ɧ=new Dictionary<Ĝ,string>();public Ǣ(){}Ĝ ɩ(string Ƀ){return ɧ.Keys.FirstOrDefault(ɨ=>ɨ.ǽ()==Ƀ);}public string Ʉ(
string Ƀ){Ĝ Ğ=ɩ(Ƀ);return Ğ!=null?ɧ[Ğ]:"";}public bool ɪ(string Ƀ){return ɩ(Ƀ)!=null;}public void ɂ(Ĝ Ğ,string Ɂ){ɧ[Ğ]=Ɂ;}
public void Ʌ(string Ƀ){Ĝ Ğ=ɩ(Ƀ);if(Ğ!=null)ɧ.Remove(Ğ);}public bool ɫ()=>ɧ.Count==0;public void Ƞ()=>ɧ.Clear();}class ɳ:È{
public float ɬ=100f;public bool ɭ=false;public Vector3D?ɮ;HashSet<string>ɯ=new HashSet<string>();HashSet<
string>ɰ=new HashSet<string>();public ɳ(IMyTextSurface w,IMyTerminalBlock u,MyIni m):base(w,u,m){ɱ.ContentType=ContentType.
SCRIPT;ɱ.Script="";ɲ();}public void ɵ(Vector3D?ɴ){ɮ=ɴ;}public void ɲ(){if(ǲ.ContainsSection("general")){ɬ=ǲ.Get("general",
"mapScale").ToSingle();ɭ=$"{ǲ.Get("general","mode")}"=="3D";ɶ(ǲ.Get("general","filter").ToString());string ɷ=ǲ.Get("general",
"center").ToString();if(ɷ.Contains(":"))ɵ(ÿ.ɸ(ɷ));}}void ɶ(string ɹ){var ɏ=ɹ.Split(new[]{' '},StringSplitOptions.
RemoveEmptyEntries);foreach(var ɓ in ɏ){if(ɓ.StartsWith("+"))ɯ.Add(ɓ.Substring(1).Trim());else if(ɓ.StartsWith("-"))ɰ.Add(ɓ.Substring(1).
Trim());}}public void ʀ(ƨ ƪ,Vector2 Ȗ,Vector3D å){float ɻ=ɺ();ɼ(Ȗ,10*ɻ,Color.Yellow,45*ɻ);Vector2 ɽ=Ȗ+new Vector2(10,10)*ɻ;ɾ
(ƪ.Â,ɽ,Color.White,"White");var Ç=Vector3D.Distance(å,ƪ.Æ);Vector2 ɿ=Ȗ+new Vector2(10,34)*ɻ;ɾ($"{Ç:F0}m",ɿ,Color.White,
"White");}public void ʌ(ƨ ƪ,Vector2 ɢ,Vector3D å,string Ģ){string Ã=ƪ.z??ƪ.Â;bool ʁ=Ã==Ģ;bool ʄ=ɯ.Count==0||ɯ.Contains(Ã)||ƪ.ʂ.
Any(ʃ=>ɯ.Contains(ʃ));bool ʅ=ɰ.Contains(Ã)||ƪ.ʂ.Any(ʃ=>ɰ.Contains(ʃ));if((!ʄ||ʅ)&&!ʁ)return;string ʆ=Ã==Ģ?"SquareSimple":
"Circle";float ʇ=12;if(Ã==Ģ)ʈ(ɢ,(ʇ+6)*ɺ(),Color.Green);else{Color ʉ=Color.White;if(ƪ.µ())ʉ=Color.Green;else if(ƪ.ʊ())ʉ=Color.Red
;else if(ƪ.À())ʉ=Color.RoyalBlue;ʋ(ɢ,ʇ*ɺ(),ʉ);}ɾ(Ã,ɢ+new Vector2(10,10)*ɺ(),Color.White,"White");var Ç=Vector3D.Distance(
å,ƪ.Æ);if(Ã!=Ģ)ɾ($"{Ç:F0}m",ɢ+new Vector2(10,34)*ɺ(),Color.White,"White");}public void ʗ(ǣ Ȣ,BoundingBoxD Ȝ,IMyCubeGrid ʍ
,string ʎ,Vector3D?ɷ=null){Vector3D ʏ=ɷ??ɮ??ʍ.GetPosition();MatrixD ʐ=MatrixD.CreateFromDir(ʍ.WorldMatrix.Forward,ʍ.
WorldMatrix.Up);var ʑ=Ȣ.Ȧ();if(ʑ==null||ʑ.Count<2)return;for(int ɦ=1;ɦ<ʑ.Count;ɦ++){var ʓ=ʒ(ʏ,ʑ[ɦ].ğ(),ʐ);var ʔ=ʒ(ʏ,ʑ[ɦ-1].ğ(),ʐ);ʕ
(ʔ,ʓ,Color.White,2);}ɾ(ʎ,ʖ,Color.White,"White");}public Vector2 ʒ(Vector3D ɷ,Vector3D ɢ,MatrixD ʘ){float ʙ=ɬ;float ʛ=ʚ.
Height/ʙ;float ʜ=(ʚ.Width/ʚ.Height)*ʙ;Vector3D ʝ=ɢ-ɷ;Vector3D ʞ=ɭ?Vector3D.Transform(ʝ,MatrixD.Transpose(ʘ)):new Vector3D(ʝ.X,
0,ʝ.Z);double ʟ=ʞ.X/ʜ;double ʠ=ʞ.Z/ʙ;float ʡ=(float)(ʚ.X+(0.5+ʟ)*ʚ.Width);float ʢ=(float)(ʚ.Y+(0.5-ʠ)*ʚ.Height);return
new Vector2(ʡ,ʢ);}}public class K:Q{public const string ʣ="MapView";S S;T T;U U;H H;HashSet<ɳ>ʤ=new HashSet<ɳ>();
public K(A B):base(B){}public override void c(){S=A.W<S>();T=A.W<T>();U=A.W<U>();H=A.W<H>();Y<X>();ʥ();S.a(ʦ);}public override
void g(d e,object f){if(e is X&&(f is IMyTextPanel||f is IMyTextSurfaceProvider)){var u=(IMyTerminalBlock)f;var m=U.l(u);var
ʩ=ʤ.FirstOrDefault(ʧ=>ʧ.ʨ.EntityId==u.EntityId);bool ʫ=ʪ(m);if(ʩ!=null&&ʫ){ʩ.ʬ(m);ʩ.ɲ();}else if(ʩ!=null&&!ʫ)ʤ.Remove(ʩ);
else if(ʩ==null&&ʫ)ʥ();}}void ʥ(){ʤ.Clear();ʭ();ʮ();}void ʭ(){U.j<IMyTextPanel>()?.ForEach(k=>{MyIni m=U.l(k);foreach(n q in
o.p(m)){if(q.r==0&&string.Equals(q.s,ʣ,StringComparison.OrdinalIgnoreCase)){ʯ(k,k);break;}}});}void ʮ(){var t=new List<
IMyTerminalBlock>();t.AddRange(U.j<IMyCockpit>());t.AddRange(U.j<IMyProgrammableBlock>());t.AddRange(U.j<IMySoundBlock>());t.ForEach(u=>
{IMyTextSurfaceProvider v=u as IMyTextSurfaceProvider;if(v==null)return;MyIni m=U.l(u);foreach(n q in o.p(m)){if(!string.
Equals(q.s,ʣ,StringComparison.OrdinalIgnoreCase))continue;if(q.r>=v.SurfaceCount)continue;IMyTextSurface w=v.GetSurface(q.r);ʯ
(w,u);}});}bool ʪ(MyIni m){foreach(n q in o.p(m)){if(string.Equals(q.s,ʣ,StringComparison.OrdinalIgnoreCase))return true;
}return false;}void ʯ(IMyTextSurface w,IMyTerminalBlock u){MyIni m=U.l(u);string ʰ=$"{m.Get("general","center")}";ɳ ʱ=new
ɳ(w,u,m);if(ʱ.ɮ==null){ƨ ƪ=T.Ʃ(ʰ);if(ƪ!=null)ʱ.ɵ(ƪ.Æ);}ʤ.Add(ʱ);}void ʦ(){var ʲ=T.x.Select(y=>y.Æ).ToList();foreach(var ʱ
in ʤ){ʱ.ʳ();ʱ.ʴ();if(H?.ǥ!=null)ʲ.AddRange(H.ǥ.Ȧ().Select(ɨ=>ɨ.ğ()));if(ʲ.Count>0){var Ȝ=ÿ.ʵ(ʲ);ʶ(ʱ,Ȝ,A.Å.GetPosition());}
ʱ.ʷ();ʱ.ʸ.Dispose();}}void ʶ(ɳ ʱ,BoundingBoxD Ȝ,Vector3D?ɷ=null){IMyCubeGrid ʍ=A.Å;Vector3D ʏ=ɷ??ʱ.ɮ??ʍ.GetPosition();
MatrixD ʐ=MatrixD.CreateFromDir(ʍ.WorldMatrix.Forward,ʍ.WorldMatrix.Up);if(H?.ǥ!=null){var ʎ=H.ǧ!=null?
$"{H.ǧ?.ǽ()} -> {H.Ǩ?.ǽ()}":$"{H.Ǩ?.ǽ()}";ʱ.ʗ(H.ǥ,Ȝ,ʍ,ʎ,ʏ);}foreach(var ƪ in T.x){var ʹ=ʱ.ʒ(ʏ,ƪ.Æ,ʐ);if(ƪ.ȃ=="grid"&&!ƪ.ʺ())ʱ.ʌ(ƪ,ʹ,A.Å.GetPosition
(),A.Ĕ);else if(ƪ.ȃ=="waypoint")ʱ.ʀ(ƪ,ʹ,A.Å.GetPosition());}Vector2 ɽ=ʱ.ʻ?ʱ.ʼ-new Vector2(0,1.5f*ʱ.ʽ):ʱ.ʼ-new Vector2(0,
2.5f*ʱ.ʽ);ʱ.ɾ($"{ʱ.ɬ}m",ɽ,Color.White,"White");}}public class ʾ:Ē{F ē;public override string Ĕ=>"thruster/thrust";
public ʾ(F ĕ){ē=ĕ;}public override string Ġ(Ė ė){if(ė.Ę.Count==0)return ę.Ě.ě;else if(ė.Ę.Count==2){string ʿ=ė.Ę[0];string ˀ=ė
.Ę[1];string ˁ=ˀ.EndsWith("N")?"N":"%";List<IMyThrust>ˇ=ē.ˆ<IMyThrust>(ʿ);if(ˇ.Count==0)return ˈ.ˉ(ˊ.ˋ,ʿ);ˇ.ForEach(ƣ=>ē.
ˌ(ƣ,ˀ));return ˈ.ˉ(ˊ.ˍ,ʿ,$"thrust={ˀ}{ˁ}");}return ę.Ě.ˎ;}}public class F:Q{List<IMyThrust>Ĵ;public F(A B):base(B){}
public override void c(){Ĵ=A.W<U>().j<IMyThrust>();Þ(new ʾ(this));}public void ˌ(IMyThrust ƣ,string ˏ){if(ˏ.Contains("N")){
float ː=float.Parse(ˏ.Replace("N",""));ƣ.ThrustOverride=ː;}else{float ˑ=float.Parse(ˏ.Replace("%",""))/100;ƣ.
ThrustOverridePercentage=ˑ;}}void ˠ(IMyThrust ƣ){ƣ.ThrustOverride=0;}public void Ǚ(){Ĵ.ForEach(ƣ=>ˠ(ƣ));}}public class ˡ:Ē{A A;public
override string Ĕ=>"boot";public ˡ(A B){A=B;}public override string Ġ(Ė ė){A.ˢ=true;return"Rebooting...";}}public abstract class
Ē:ˣ{public abstract string Ĕ{get;}public abstract string Ġ(Ė ė);public string ˤ()=>Ĕ;protected float ʹ(string ˬ,
Dictionary<string,string>ˮ){float Ͱ;if(!float.TryParse(ˬ,out Ͱ))throw new ArgumentException("Invalid numerical value provided.");
bool ͱ=ˮ.ContainsKey("add");bool Ͳ=ˮ.ContainsKey("sub");float ͳ=0;if(ͱ)ͳ=Ͱ;else if(Ͳ)ͳ=-Ͱ;return ͳ;}protected float ͻ(float
Ͷ,int ͷ,bool ͺ){if(ͺ&&ͷ>1)return Ͷ/ͷ;return Ͷ;}protected bool ͼ(Dictionary<string,string>ˮ){return ˮ.ContainsKey("share")
;}}public interface ˣ{string ˤ();string Ġ(Ė ė);}public class Ή:Ē{A A;public override string Ĕ=>"purge";bool ͽ=
false;List<string>Ά=new List<string>();List<string>Έ=new List<string>();public Ή(A B){A=B;}void Ί(string ĕ)
{switch(ĕ){case"almanac":A.W<T>().Ƞ();Έ.Add(ĕ);break;case"storage":A.W<ȅ>().Ƞ();Έ.Add(ĕ);break;default:break;}}public
override string Ġ(Ė ė){Ά.Clear();Έ.Clear();foreach(var Ύ in ė.Ό){string ǹ=Ύ.Key;string Ώ=Ύ.Value;if(ǹ=="force"&&(Ώ=="true"||Ώ==
"1"))ͽ=true;}if(!ͽ)return"Run command with --force to purge";if(ė.Ę.Count==0)return ę.Ě.ě;else{string ΐ=ė.Ę[0];List<string>
Α=ΐ.Split(',').ToList();if(Α.Contains("*")){Ά.Add("almanac");Ά.Add("storage");}if(Α.Contains("storage"))Ά.Add("storage");
if(Α.Contains("almanac"))Ά.Add("almanac");Ά.ForEach(ĕ=>Ί(ĕ));return Έ.Count==0?"No modules purged":
$"Purged {Έ.Count} modules: {string.Join(", ",Έ)}";}}}public class Γ:Ē{A A;Random Β=new Random();public override string Ĕ=>"rename";public Γ(A B){A=B;}
public override string Ġ(Ė ė){if(ė.Ę.Count<1)return ę.Ě.ě;string Δ=ė.Ę[0];bool Ζ=Ė.Ε(ė.ģ("unique"));if(Ζ)Δ=
$"{Δ}-{Β.Next(10000,99999)}";A.Å.CustomName=Δ;A.Ĕ=Δ;var m=A.W<ǲ>();m.Η.Set("general","name",Δ);A.Θ.CustomData=m.Η.ToString();var Ι=A.W<Ĳ>();Ι.Κ();Ι.
Λ();return$"Grid name set to: {Δ}";}}public class Τ:Μ{public Dictionary<IMyTerminalBlock,Ν>Ξ{get;}public struct Ν{public
Func<IMyTerminalBlock,bool>Ο;public Action<IMyTerminalBlock>Π;public Ν(Func<IMyTerminalBlock,bool>Ρ,Action<IMyTerminalBlock>
Σ){Ο=Ρ;Π=Σ;}}public Τ(A B):base(B){Ξ=new Dictionary<IMyTerminalBlock,Ν>();}public override void P(){var Υ=new List<
IMyTerminalBlock>();foreach(var q in Ξ){IMyTerminalBlock u=q.Key;Ν Φ=q.Value;if(Φ.Ο(u)){Φ.Π?.Invoke(u);Υ.Add(u);}}Υ.ForEach(u=>Χ(u));}
public void Ϊ(IMyTerminalBlock u,Func<IMyTerminalBlock,bool>Ψ,Action<IMyTerminalBlock>Ω){if(!Ξ.ContainsKey(u))Ξ[u]=new Ν(Ψ,Ω);
}public void Χ(IMyTerminalBlock u){if(Ξ.ContainsKey(u))Ξ.Remove(u);}}public class T:Μ{const double Ϋ=300;public List<ƨ>x=
new List<ƨ>();public T(A B):base(B){A=B;}public override void c(){ά();var S=A.W<S>();S.a(έ,1);S.a(ή,30);}public void έ(){ƨ
ƪ=Ʃ($"{A.Â}");MatrixD Ą=A.ί();Vector3D ɢ=Ą.Translation;if(ƪ==null){ƪ=new ƨ($"{A.Â}","grid",ɢ,A.Å.Speed);}Vector3D?ΰ=(
Vector3D?)Ą.Forward;Vector3D?α=(Vector3D?)Ą.Up;ƪ.β(ɢ,A.Å.Speed,A.Ĕ,A.γ.Radius,ΰ,α);Ȁ(ƪ);}void ά(){string ε=A.W<ȅ>().δ("almanac")
??"";if(ε=="")return;Dictionary<string,object>ζ;try{ζ=ȇ.η(ε);}catch{return;}foreach(var ƪ in ζ){try{Dictionary<string,
object>θ=(Dictionary<string,object>)ƪ.Value;ƨ κ=ƨ.ι(θ);if(λ(κ))x.Add(κ);}catch{}}}bool λ(ƨ ƪ){if(ƪ==null||string.IsNullOrEmpty
(ƪ.Â))return false;if(ƪ.ȃ=="waypoint")return true;if(ƪ.ȃ=="grid"&&ƪ.μ==0)return false;if(ν(ƪ))return false;return true;}
bool ν(ƨ ƪ){if(ƪ.ȃ=="waypoint")return false;double ο=(DateTime.Now-ƪ.ξ).TotalSeconds;return ο>Ϋ;}void ή(){int ȁ=x.RemoveAll(
ƪ=>ƪ.Â!=$"{A.Â}"&&ν(ƪ));if(ȁ>0)M();}void M(){var ζ=new Dictionary<string,object>();foreach(var ƪ in x)ζ[ƪ.Â]=ƪ;A.W<ȅ>().Ȇ
("almanac",ȇ.Ȉ(ζ));}public void Ƞ(){x.Clear();M();}public ƨ ϊ(string ȫ,long π,string Ã,Vector3D ɢ,float ρ,HashSet<string>
ς,bool σ,Vector3D?ΰ=null,Vector3D?α=null){ƨ ƪ;ƨ Ȫ=Ʃ(ȫ);if(Ȫ!=null){Ȫ.β(ɢ,ρ,Ã,0,ΰ,α);Ȫ.μ=π;Ȫ.ʂ.UnionWith(ς);ƪ=Ȫ;}else{ƪ=
new ƨ(ȫ,"grid",ɢ,ρ){μ=π,z=Ã,ʂ=ς};if(ΰ.HasValue)ƪ.τ=ΰ.Value;if(α.HasValue)ƪ.υ=α.Value;if(σ)ƪ.Ä=ƨ.φ.χ;}if(!ς.Contains("*")&&ƪ
.Ä==ƨ.φ.ψ)ƪ.Ä=ƨ.φ.ω;Ȁ(ƪ);return ƪ;}public ƨ Ʃ(string ϋ){return x.Find(ƪ=>ƪ.Â==ϋ||ƪ.z==ϋ);}public List<ƨ>ȉ(string ό){
return x.FindAll(ƪ=>ƪ.ȃ==ό);}public void Ȁ(ƨ ƪ){ƨ Ȫ=x.Find(y=>y.Â==ƪ.Â);if(Ȫ==null)x.Add(ƪ);else{if(ƪ.ξ>Ȫ.ξ){x.Remove(Ȫ);x.Add
(ƪ);}}M();}}public class ƨ:ύ{public static Dictionary<string,string>ώ=new Dictionary<string,string>{{"grid",
"grid"},{"waypoint","waypoint"}};public enum φ{χ,ω,ψ,Ϗ}public string Â{get;}public long μ{get;set;}public DateTime ξ;public
Vector3D Æ;public Vector3D τ=Vector3D.Forward;public Vector3D υ=Vector3D.Up;public float ϐ;public double ɡ;public string ȃ{get;
set;}public string z{get;set;}public φ Ä{get;set;}public HashSet<string>ʂ=new HashSet<string>();public ƨ(string ϑ,string ϒ,
Vector3D ɢ,float ρ=0){Â=ϑ;ξ=DateTime.Now;Æ=ɢ;ϐ=ρ;ȃ=ϒ;Ä=φ.ψ;}public void β(Vector3D ɢ,float ρ,string Ȭ=null,double ϓ=0,Vector3D?ΰ
=null,Vector3D?α=null){Æ=ɢ;ϐ=ρ;ξ=DateTime.Now;z=Ȭ??z;ɡ=ϓ>0?ϓ:ɡ;τ=ΰ??τ;υ=α??υ;}public bool µ()=>Ä==φ.ω;public bool ʊ()=>Ä
==φ.Ϗ;public bool À()=>Ä==φ.ψ;public bool ʺ()=>Ä==φ.χ;public string ϕ(){Dictionary<string,object>ϔ=new Dictionary<string,
object>{{"Id",$"{Â}"},{"UnicastId",$"{μ}"},{"DisplayName",$"{z}"},{"UpdatedAt",$"{ξ.Ticks}"},{"pos",$"{Æ}"},{"LastKnownSpeed",
$"{ϐ}"},{"EntityType",ώ[ȃ]},{"SafeRadius",$"{ɡ}"},{"fx",$"{τ.X}"},{"fy",$"{τ.Y}"},{"fz",$"{τ.Z}"},{"ux",$"{υ.X}"},{"uy",
$"{υ.Y}"},{"uz",$"{υ.Z}"}};return ȇ.Ȉ(ϔ);}public static ƨ ι(Dictionary<string,object>ϖ){string[]ϗ=$"{ϖ["pos"]}".Split(' ');
double ǒ=double.Parse(ϗ[0].Substring(2));double Ǔ=double.Parse(ϗ[1].Substring(2));double ǔ=double.Parse(ϗ[2].Substring(2));
string ϒ=$"{ϖ["EntityType"]}";ƨ Ϙ=new ƨ($"{ϖ["Id"]}",ώ[ϒ],new Vector3D(ǒ,Ǔ,ǔ),0);object ϙ;long Ϛ;if(ϖ.TryGetValue("UnicastId",
out ϙ)&&ϙ!=null&&long.TryParse(ϙ.ToString(),out Ϛ)){Ϙ.μ=Ϛ;}object ϛ;if(ϖ.TryGetValue("DisplayName",out ϛ)&&ϛ!=null&&!string
.IsNullOrEmpty(ϛ.ToString())){Ϙ.z=ϛ.ToString();}double ϓ;object Ϝ;if(ϖ.TryGetValue("SafeRadius",out Ϝ)&&Ϝ!=null&&double.
TryParse(Ϝ.ToString(),out ϓ)){Ϙ.ɡ=ϓ;}double ϝ,Ϟ,ϟ,Ϡ,ϡ,Ϣ;object ϣ,Ϥ,ϥ,Ϧ,ϧ,Ϩ;if(ϖ.TryGetValue("fx",out ϣ)&&double.TryParse($"{ϣ}",
out ϝ)&&ϖ.TryGetValue("fy",out Ϥ)&&double.TryParse($"{Ϥ}",out Ϟ)&&ϖ.TryGetValue("fz",out ϥ)&&double.TryParse($"{ϥ}",out ϟ))
{Ϙ.τ=new Vector3D(ϝ,Ϟ,ϟ);}if(ϖ.TryGetValue("ux",out Ϧ)&&double.TryParse($"{Ϧ}",out Ϡ)&&ϖ.TryGetValue("uy",out ϧ)&&double.
TryParse($"{ϧ}",out ϡ)&&ϖ.TryGetValue("uz",out Ϩ)&&double.TryParse($"{Ϩ}",out Ϣ)){Ϙ.υ=new Vector3D(Ϡ,ϡ,Ϣ);}return Ϙ;}public long
ư()=>long.Parse(Â);}public class ĝ:Ĝ{long Â;string Ĕ;string ϩ;Vector3D Ϫ;public ĝ(string Ȳ){Â=
new Random().Next(0,1000000);string[]ϫ=Ȳ.Split(':');if(ϫ.Length<6)return;Ĕ=ϫ[1];Ϫ=new Vector3D(double.Parse(ϫ[2]),double.
Parse(ϫ[3]),double.Parse(ϫ[4]));ϩ=ϫ[5];}public Vector3D ğ()=>Ϫ;public string ǽ()=>Ĕ;}public interface Ĝ{Vector3D ğ();string ǽ
();}public abstract class ϲ:Ϭ,ϭ{public A A;Dictionary<IMyTerminalBlock,Func<IMyTerminalBlock,object>>Ϯ=
new Dictionary<IMyTerminalBlock,Func<IMyTerminalBlock,object>>();Dictionary<IMyTerminalBlock,Action<
IMyTerminalBlock,object>>ϯ=new Dictionary<IMyTerminalBlock,Action<IMyTerminalBlock,object>>();public Dictionary<long,object>ϰ=
new Dictionary<long,object>();public List<ˣ>ϱ=new List<ˣ>();public ϲ(A B){A=B;}public virtual void P(){}public virtual void
c(){}public virtual IEnumerator<double>ϳ(){c();yield return 0.0;}public virtual void g(d e,object f){}public virtual
string ϴ()=>$"{GetType()}";public virtual void Þ(ˣ ė){ϱ.Add(ė);}protected void ϻ<ϵ>(Func<ϵ,object>Ϸ,Action<IMyTerminalBlock,
object>ϸ)where ϵ:class,IMyTerminalBlock{U U=A.W<U>();foreach(var u in U.j<ϵ>()){Ϯ[u]=(Ϲ)=>Ϸ(Ϲ as ϵ);ϯ[u]=ϸ;U.Ϻ(u,this);ϰ[u.
EntityId]=Ϸ(u);}}public object ϼ(IMyTerminalBlock u){return Ϯ.ContainsKey(u)?Ϯ[u](u):null;}public bool Ͽ(IMyTerminalBlock u,
object Ͻ){if(!Ϯ.ContainsKey(u))return false;object Ͼ=Ϯ[u](u);return Ͻ==null||!Equals(Ͻ,Ͼ);}public void Ё(IMyTerminalBlock u){
if(!Ϯ.ContainsKey(u))return;long Ѐ=u.EntityId;object Ͼ=Ϯ[u](u);if(!ϰ.ContainsKey(Ѐ)||!Equals(ϰ[Ѐ],Ͼ)){if(ϯ.ContainsKey(u))
ϯ[u](u,Ͼ);ϰ[Ѐ]=Ͼ;}}public List<ϵ>ˆ<ϵ>(string Ã)where ϵ:class,IMyTerminalBlock{return A.W<U>().ˆ<ϵ>(Ã);}public ϵ W<ϵ>()
where ϵ:class,Ϭ{return A.W<ϵ>();}public void Y<Ђ>()where Ђ:d{A.W<ǡ>().Y<Ђ>(this);}public void ï(d e,object f){A.W<ǡ>().ï(e,f)
;}public void ï<Ђ>(object f=null)where Ђ:d,new(){ï(new Ђ(),f);}public List<ˣ>Ѓ()=>ϱ;public void ń(string Є,Func<ō,Ō>Ѕ){W<
Ĳ>().І.Ї(Є,Ѕ);}}public abstract class Μ:ϲ,Ј{public Μ(A B):base(B){}}public abstract class Q:ϲ,E{public Q(A B):base(B){}}
public class U:Μ{S S;ǲ ǲ;ǡ ǡ;public HashSet<long>Љ=new HashSet<long>();List<IMyBlockGroup>Њ=new List<IMyBlockGroup>()
;public List<IMyTerminalBlock>Ћ=new List<IMyTerminalBlock>();public Dictionary<IMyTerminalBlock,MyIni>Ќ=new
Dictionary<IMyTerminalBlock,MyIni>();public Dictionary<string,HashSet<IMyTerminalBlock>>Ѝ=new Dictionary<string,HashSet<
IMyTerminalBlock>>();Dictionary<IMyTerminalBlock,Dictionary<string,string>>Ў=new Dictionary<IMyTerminalBlock,Dictionary<string,
string>>();Dictionary<long,object>Џ=new Dictionary<long,object>();Dictionary<IMyTerminalBlock,ϭ>А=new
Dictionary<IMyTerminalBlock,ϭ>();int Б=0;const int В=50;const string Г="general";const string Д="tags";const string Е="hooks";bool
Ж=false;public U(A B):base(B){A=B;}public override IEnumerator<double>ϳ(){c();foreach(var И in З(A.Å))yield return И;Й();
S.К(Л());yield break;}public override void c(){ǲ=A.W<ǲ>();ǡ=A.W<ǡ>();S=A.W<S>();ǡ.Y<ŀ>(this);ǡ.Y<М>(this);ǡ.Y<Н>(this);ǡ.
Y<О>(this);ǡ.Y<П>(this);ǡ.Y<Р>(this);ǡ.Y<С>(this);}IEnumerable<double>Л(){Т();S.К(Л(),1);yield return 0;}public override
void P(){У();}void У(){var Ф=А.Keys.ToList();int Х=Ф.Count;var Ц=Ф.Skip(Б).Take(В).ToList();foreach(var u in Ц){if(!А.
ContainsKey(u))continue;ϭ Ч=А[u];object Ͻ=Џ.ContainsKey(u.EntityId)?Џ[u.EntityId]:null;object Ͼ=Ч.ϼ(u);if(Ч.Ͽ(u,Ͻ)){Ч.Ё(u);Џ[u.
EntityId]=Ͼ;}}Б+=В;if(Б>=Х)Б=0;}public override void g(d e,object f){if(e is ŀ||e is М)Ш();if(e is Р||e is С)Ш();if(e is Н||e is
О)Щ();if(e is П)Й();}public IMyTerminalBlock Э(IMyTerminalBlock u,string Ъ){MyIni Ы=l(u);string Ь=$"{Ы.Get(Г,Д)}";if(Ь==
"")Ь=Ъ;else if(!Ь.Contains(Ъ))Ь+=$",{Ъ}";Ы.Set(Г,Д,Ь);u.CustomData=Ы.ToString();if(!Ѝ.ContainsKey(Ъ))Ѝ[Ъ]=new HashSet<
IMyTerminalBlock>();Ѝ[Ъ].Add(u);return u;}public void Ϻ(IMyTerminalBlock u,ϭ Ч){А[u]=Ч;Џ[u.EntityId]=Ч.ϼ(u);}public List<ϵ>j<ϵ>(Func<ϵ,
bool>Ю=null)where ϵ:class,IMyTerminalBlock{List<ϵ>Я=Ћ.OfType<ϵ>().Where(u=>Ю==null||Ю(u)).ToList();return Я;}bool б(
IMyTerminalBlock u,MyIni а){return Ќ.ContainsKey(u)&&а.ToString()!=Ќ[u].ToString();}void Т(){foreach(var u in Ћ){MyIni Ы=new MyIni();
MyIniParseResult в;if(!Ы.TryParse(u.CustomData,out в))continue;if(A.г==A.д.е)Ќ[u]=Ы;else if(A.г==A.д.ж&&б(u,Ы)){Ќ[u]=Ы;ï<X>(u);if(u.
EntityId==A.Â)ï<П>(u);A.Ŏ($"Config changed: {u.CustomName}",false);}if(Ы.ToString()=="")з(u,Ы);if(Ы.ContainsSection(Г))и(u,Ы);if
(Ы.ContainsSection(Е))й(u,Ы);}}void з(IMyTerminalBlock u,MyIni Ы){foreach(var ǹ in ǲ.к.Keys){string[]л=ǹ.Split('.');Ы.Set
(л[0],л[1],ǲ.к[ǹ]);}u.CustomData=Ы.ToString();}void и(IMyTerminalBlock u,MyIni Ы){string Ь=$"{Ы.Get(Г,Д)}";if(Ь=="")
return;foreach(var Ъ in Ь.Split(',')){string м=Ъ.Trim();if(!Ѝ.ContainsKey(м))Ѝ[м]=new HashSet<IMyTerminalBlock>();Ѝ[м].Add(u);
}}void й(IMyTerminalBlock u,MyIni Ы){var н=new Dictionary<string,string>();List<MyIniKey>Ƿ=new List<MyIniKey>();Ы.GetKeys
(Е,Ƿ);foreach(var о in Ƿ){string п=$"{Ы.Get(Е,о.Name)}";string с=р(u,п);н[о.Name]=с;}Ў[u]=н;}string р(IMyTerminalBlock u,
string п){StringBuilder т=new StringBuilder();int у=0;while(у<п.Length){int Ȃ=п.IndexOf("this",у);if(Ȃ==-1){т.Append(п.
Substring(у));break;}if(Ȃ>0&&(п[Ȃ-1]==' '||п[Ȃ-1]=='=')&&(Ȃ+4==п.Length||п[Ȃ+4]==' '||п[Ȃ+4]==';')){т.Append(п.Substring(у,Ȃ-у));
т.Append($"\"{u.CustomName}\"");}else{т.Append(п.Substring(у,Ȃ-у+4));}у=Ȃ+4;}return$"{т}";}void Й(){MyIni ф=ǲ.Ǹ;List<
MyIniKey>Ƿ=new List<MyIniKey>();ф.GetKeys(Е,Ƿ);foreach(var ǹ in Ƿ){if(ǹ.Name.Contains(".")){string[]л=ǹ.Name.Split('.');string х
=л[0].Trim('\"');string о=л[1];foreach(var u in ˆ<IMyTerminalBlock>(х)){if(!Ў.ContainsKey(u))Ў[u]=new Dictionary<string,
string>();Ў[u][о]=ф.Get(Е,ǹ.Name).ToString();}}}}public void ч(IMyTerminalBlock u,string о){if(Ў.ContainsKey(u)&&Ў[u].
ContainsKey(о)){string ц=Ў[u][о];A.W<ę>().ƛ(ц);}}public MyIni l(IMyTerminalBlock u){return Ќ.ContainsKey(u)?Ќ[u]:new MyIni();}
public new List<ϵ>ˆ<ϵ>(string Ã)where ϵ:class,IMyTerminalBlock{List<ϵ>ш=new List<ϵ>();List<IMyBlockGroup>ъ=щ(Ã);if(ъ.Count>0){
foreach(var ь in ъ){List<ϵ>ы=new List<ϵ>();ь.GetBlocksOfType(ы);ш.AddRange(ы.Where(u=>Љ.Contains(u.CubeGrid.EntityId)));}}else
if(Ã.StartsWith("#")){if(Ѝ.ContainsKey(Ã.Substring(1))){Ѝ[Ã.Substring(1)]?.ToList().ForEach(u=>{if(u is ϵ&&Љ.Contains(u.
CubeGrid.EntityId))ш.Add(u as ϵ);});}else{A.Ŏ($"Tag not found: {Ã}");return ш;}}else{ϵ u=э(Ã)as ϵ;if(u!=null&&Љ.Contains(u.
CubeGrid.EntityId))ш.Add(u);}return ш;}public void Ш(){Њ.Clear();A.ю.GetBlockGroups(Њ);}List<IMyBlockGroup>щ(string я){return Њ.
Where(ь=>string.Equals(ь.Name,я,StringComparison.OrdinalIgnoreCase)).ToList();}IMyTerminalBlock э(string Ã)=>Ћ.FirstOrDefault
(ǒ=>ǒ.DisplayNameText==Ã);void ѐ(){A.Ý=Ћ.OfType<IMyShipController>().OrderByDescending(Ʀ=>Ʀ.IsMainCockpit).FirstOrDefault
();}List<IMyMechanicalConnectionBlock>ё=new List<IMyMechanicalConnectionBlock>();Queue<IMyCubeGrid>ђ=
new Queue<IMyCubeGrid>();HashSet<long>ѓ=new HashSet<long>();Dictionary<long,HashSet<long>>є=new Dictionary<long,
HashSet<long>>();const int ѕ=40;const int і=500;IEnumerable<double>З(IMyCubeGrid Ȕ){ї();Љ.Clear();foreach(var И in ј(Ȕ,Љ))yield
return И;foreach(var И in љ())yield return И;}void ї(){ё.Clear();є.Clear();A.ю.GetBlocksOfType(ё);for(int ɦ=0;ɦ<ё.Count;ɦ++){
var њ=ё[ɦ];var ћ=њ.CubeGrid;var Ϲ=њ.TopGrid;if(ћ==null||Ϲ==null)continue;long ќ=ћ.EntityId,ѝ=Ϲ.EntityId;HashSet<long>ў;if(!
є.TryGetValue(ќ,out ў))є[ќ]=ў=new HashSet<long>();ў.Add(ѝ);HashSet<long>џ;if(!є.TryGetValue(ѝ,out џ))є[ѝ]=џ=new HashSet<
long>();џ.Add(ќ);}}IMyCubeGrid ѡ(long Ѡ){for(int ɦ=0;ɦ<ё.Count;ɦ++){var Ϲ=ё[ɦ];if(Ϲ.CubeGrid!=null&&Ϲ.CubeGrid.EntityId==Ѡ)
return Ϲ.CubeGrid;if(Ϲ.TopGrid!=null&&Ϲ.TopGrid.EntityId==Ѡ)return Ϲ.TopGrid;}return null;}void Ѥ(IMyTerminalBlock u){var Ѣ=
new MyIni();MyIniParseResult ѣ;if(!Ѣ.TryParse(u.CustomData,out ѣ))return;Ќ[u]=Ѣ;if(Ѣ.ToString().Length==0)з(u,Ѣ);if(Ѣ.
ContainsSection(Г))и(u,Ѣ);if(Ѣ.ContainsSection(Е))й(u,Ѣ);}IEnumerable<double>ѩ(HashSet<long>ѥ){var Ѧ=new List<IMyTerminalBlock>();A.ю.
GetBlocks(Ѧ);var ѧ=Ѧ.Where(Ϲ=>ѥ.Contains(Ϲ.CubeGrid.EntityId)).ToList();int Ȃ=0;while(Ȃ<ѧ.Count){int Ѩ=Math.Min(і,ѧ.Count-Ȃ);for(
int ɦ=0;ɦ<Ѩ;ɦ++){var u=ѧ[Ȃ+ɦ];Ћ.Add(u);Ѥ(u);}Ȃ+=Ѩ;yield return 0;}}void ѫ(HashSet<long>ѥ){Ћ.RemoveAll(Ϲ=>{if(!ѥ.Contains(Ϲ.
CubeGrid.EntityId))return false;Ќ.Remove(Ϲ);А.Remove(Ϲ);Џ.Remove(Ϲ.EntityId);Ў.Remove(Ϲ);foreach(var Ѫ in Ѝ.Values)Ѫ.Remove(Ϲ);
return true;});}IEnumerable<double>ј(IMyCubeGrid Ȕ,HashSet<long>в){ђ.Clear();ѓ.Clear();ђ.Enqueue(Ȕ);while(ђ.Count>0){int Ѭ=0;
while(ђ.Count>0&&Ѭ<ѕ){var ѭ=ђ.Dequeue();long Ѯ=ѭ.EntityId;if(ѓ.Contains(Ѯ)){Ѭ++;continue;}ѓ.Add(Ѯ);в.Add(Ѯ);HashSet<long>ѯ;if
(є.TryGetValue(Ѯ,out ѯ)){foreach(long Ѱ in ѯ){var ѱ=ѡ(Ѱ);if(ѱ!=null&&!ѓ.Contains(Ѱ))ђ.Enqueue(ѱ);}}Ѭ++;}yield return 0;}}
IEnumerable<double>љ(){Ѝ.Clear();Ў.Clear();Ќ.Clear();Ћ.Clear();var Ѳ=new List<IMyTerminalBlock>();A.ю.GetBlocks(Ѳ);for(int ɦ=0;ɦ<Ѳ.
Count;ɦ++){var ѳ=Ѳ[ɦ];if(Љ.Contains(ѳ.CubeGrid.EntityId))Ћ.Add(ѳ);}ѐ();int Ȃ=0;while(Ȃ<Ћ.Count){int Ѩ=Math.Min(і,Ћ.Count-Ȃ);
for(int ɦ=0;ɦ<Ѩ;ɦ++)Ѥ(Ћ[Ȃ+ɦ]);Ȃ+=Ѩ;yield return 0;}Ш();yield return 0;}public void Щ(){if(Ж)return;Ж=true;S.К(Ѵ());}public
void ѷ(IMyCubeGrid ѵ){if(ѵ==null||Ж)return;if(Љ.Contains(ѵ.EntityId))return;Ж=true;S.К(Ѷ(ѵ));}public void ѹ(){if(Ж)return;Ж=
true;S.К(Ѹ());}IEnumerable<double>Ѷ(IMyCubeGrid Ѻ){ї();ђ.Clear();ѓ.Clear();var ѻ=new HashSet<long>();ђ.Enqueue(Ѻ);while(ђ.
Count>0){int Ѭ=0;while(ђ.Count>0&&Ѭ<ѕ){var ѭ=ђ.Dequeue();long Ѯ=ѭ.EntityId;if(ѓ.Contains(Ѯ)){Ѭ++;continue;}ѓ.Add(Ѯ);if(!Љ.
Contains(Ѯ))ѻ.Add(Ѯ);HashSet<long>ѯ;if(є.TryGetValue(Ѯ,out ѯ)){foreach(long Ѱ in ѯ){if(Љ.Contains(Ѱ))continue;var ѱ=ѡ(Ѱ);if(ѱ!=
null&&!ѓ.Contains(Ѱ))ђ.Enqueue(ѱ);}}Ѭ++;}yield return 0;}if(ѻ.Count>0){foreach(var Ѯ in ѻ)Љ.Add(Ѯ);foreach(var И in ѩ(ѻ))
yield return И;}Ш();Ж=false;yield return 0;}IEnumerable<double>Ѹ(){ї();var Ѽ=new HashSet<long>();foreach(var И in ј(A.Å,Ѽ))
yield return И;var ѽ=new HashSet<long>(Љ);ѽ.ExceptWith(Ѽ);Љ=Ѽ;if(ѽ.Count>0)ѫ(ѽ);Ш();Ж=false;yield return 0;}IEnumerable<
double>Ѵ(){ї();var Ѿ=new HashSet<long>();foreach(var И in ј(A.Å,Ѿ))yield return И;var ѿ=new HashSet<long>(Ѿ);ѿ.ExceptWith(Љ);
var ѽ=new HashSet<long>(Љ);ѽ.ExceptWith(Ѿ);Љ=Ѿ;if(ѽ.Count>0)ѫ(ѽ);if(ѿ.Count>0){foreach(var И in ѩ(ѿ))yield return И;}Ш();Ж=
false;yield return 0;}}public class X:d{}public interface ϭ{object ϼ(IMyTerminalBlock u);bool Ͽ(IMyTerminalBlock u,object Ͻ);
void Ё(IMyTerminalBlock u);}public class S:Μ{class ҋ{public double Ҁ;public double ҁ;public Action Ҋ;}List<ҋ>Ҍ=new
List<ҋ>();List<ҋ>ҍ=new List<ҋ>();class Ґ{public IEnumerator<double>Ҏ;public double ҏ;}List<Ґ>ґ=new List<Ґ>
();MyGridProgram Ғ;bool ғ=true;public S(A B):base(B){Ғ=B.Ғ;Ғ.Runtime.UpdateFrequency=UpdateFrequency.Update10;}
public void Ҕ(){ґ.Clear();Ҍ.Clear();ҍ.Clear();}public override void c(){a(ҕ,1);}public void a(Action Җ,double җ=0){Ҍ.Add(new ҋ
{Ҋ=Җ,Ҁ=җ,ҁ=җ});}public void Ҙ()=>Ҍ.Clear();public void Қ(Action Җ,double ҙ){ҍ.Add(new ҋ{Ҋ=Җ,Ҁ=ҙ,ҁ=ҙ});}public void К(
IEnumerable<double>Ɂ,double ҙ=0){ґ.Add(new Ґ{Ҏ=Ɂ.GetEnumerator(),ҏ=ҙ});}public override void P(){double қ=Ғ.Runtime.
TimeSinceLastRun.TotalSeconds;foreach(var Җ in Ҍ){Җ.ҁ-=қ;if(Җ.ҁ<=0){Җ.Ҋ.Invoke();Җ.ҁ=Җ.Ҁ;}}for(int ɦ=ҍ.Count-1;ɦ>=0;ɦ--){var Җ=ҍ[ɦ];Җ.ҁ
-=қ;if(Җ.ҁ<=0){Җ.Ҋ.Invoke();ҍ.RemoveAt(ɦ);}}for(int ɦ=ґ.Count-1;ɦ>=0;ɦ--){var Ҝ=ґ[ɦ];Ҝ.ҏ-=қ;if(Ҝ.ҏ<=0){if(Ҝ.Ҏ.MoveNext())Ҝ
.ҏ=Ҝ.Ҏ.Current;else{Ҝ.Ҏ.Dispose();ґ.RemoveAt(ɦ);}}}}public int ҝ{get{return ҍ.Count;}}public int Ҟ{get{return ґ.Count;}}
void ҕ()=>ғ=!ғ;public string ҟ()=>ғ?"/":"\\";}public static class ˊ{public const string ˋ="Block not found: {0}";public
const string Ҡ="Invalid argument for block: {0}";public const string ҡ="Invalid command option: {0}";public const string ˍ=
"Block updated: {0} -> {1}";public const string Ң="Block resetting: {0}";public const string ң="Block moving: {0}";public const string Ҥ=
"Block started: {0}";public const string ҥ="Block stopped: {0}";public const string Ҧ="Block locked: {0}";public const string ҧ=
"Block unlocked: {0}";public const string Ҩ="Block open: {0}";public const string ҩ="Block charging: {0}";public const string Ҫ=
"Block discharging: {0}";public const string ҫ="Block auto: {0}";public const string Ҭ="Block on: {0}";public const string ҭ="Block off: {0}";
public const string Ү="Block closed: {0}";public const string ү="Block toggled: {0}";public const string Ұ=
"Block stockpiling: {0}";public const string ұ="Block sharing: {0}";public const string Ҳ="Block action: {0} -> {1}";public const string ҳ=
"Block: {0} -> {1}";}public class ę:Μ{public static class Ě{public const string Ҵ="Command not found: {0}";public const string ě=
"No arguments provided";public const string ˎ="Invalid command format.";}S S;Ɵ Ɵ;Ĳ ҵ;public List<ˣ>Ҷ=new List<ˣ>();public Dictionary<long,HashSet<string>>ҷ=new Dictionary<long,HashSet<string>>();public Dictionary<long,HashSet<string>>Ҹ=new
Dictionary<long,HashSet<string>>();public ę(A B):base(B){}public override void c(){S=A.W<S>();Ɵ=A.W<Ɵ>();ҵ=A.W<Ĳ>();ҷ.Clear();Ҹ.
Clear();Þ(new ҹ(this));ń("command",Ņ=>Һ(Ņ));ń("localcmd",Ņ=>һ(Ņ));}Ō Һ(ō Ņ){if(!ҵ.Ҽ)return null;string ҽ=Ņ.Œ("Command").Trim(
);if(string.IsNullOrEmpty(ҽ))return ҵ.Ŕ(Ņ,Ō.ŕ.Ҿ);ҿ("REQ",Ņ.Ƌ("OriginName"),ҽ);var Ӏ=new Ė(ҽ);long ӂ=Ӂ(Ӏ.Ĕ);if(ӂ==0)ӂ=Ӄ(Ӏ.
Ĕ);if(ӂ!=0){ҵ.ӄ(ӂ,ҽ);return ҵ.Ŕ(Ņ,Ō.ŕ.Ӆ);}return ӆ(Ņ,ҽ);}Ō һ(ō Ņ){string ė=Ņ.Œ("Command").Trim();if(string.IsNullOrEmpty(
ė))return ҵ.Ŕ(Ņ,Ō.ŕ.Ҿ);ҿ("CREQ",Ņ.Ƌ("OriginName"),ė);return ӆ(Ņ,ė);}Ō ӆ(ō Ņ,string ė){bool Ӈ=ƛ(ė);var ӈ=Ӈ?Ō.ŕ.Ӆ:Ō.ŕ.Ҿ;
return ҵ.Ŕ(Ņ,ӈ);}void ҿ(string Ŋ,string Ӊ,string ė){Ɵ.Ơ($"{Ŋ}: {Ӊ}> {ė}");A.Ŏ($"{Ŋ}: {Ӊ}> {ė}",false);}new public void Þ(ˣ ė){
Ҷ.Add(ė);}public bool ƛ(string ӊ){if(ӊ.Length>0){ӊ=A.Ӌ(ӊ);ӌ(new Ӎ(ӊ));return true;}return false;}void ӌ(Ӎ ӎ){var í=ӎ.ӏ;if
(S==null)S=A.W<S>();if(í=="self"||string.IsNullOrEmpty(í)){Ӑ(ӎ);}else{ӎ.ӑ(A.Ӓ);var Ӕ=$"> @{í} {ӎ.ӓ}";if(í=="*")ҵ.ӕ(ӎ);
else ҵ.Ӗ(í,ӎ);Ɵ.Ơ(Ӕ);A.Ŏ(Ӕ);}}void Ӑ(Ӎ Ɂ){if(Ɂ.ӗ){foreach(var ь in Ɂ.Ә)S.К(ә(ь));}else{S.К(ә(Ɂ.ϱ));}}IEnumerable<double>ә(
List<Ė>Ӛ){foreach(var ė in Ӛ){foreach(double Ӝ in ӛ(ė))yield return Ӝ;}}IEnumerable<double>ӛ(Ė ė){if(ė.Ĕ.ToLower()=="wait"&&
ė.Ę.Count>0){double ҙ;if(double.TryParse(ė.Ę[0],out ҙ)){A.Ŏ($"> wait {ҙ}");yield return ҙ;}yield break;}string Ӟ=ӝ(ė);if(
Ӟ!=null){string ӟ=A.Ӌ(Ӟ);var Ɂ=new Ӎ(ӟ);if(Ɂ.ӗ){foreach(var ь in Ɂ.Ә)S.К(ә(ь));}else{foreach(double Ӝ in ә(Ɂ.ϱ))yield
return Ӝ;}yield break;}Ӡ(ė);yield return 0;}string ӝ(Ė ė){string ӊ=ė.ӡ;if(ӊ.StartsWith("_")){string Ӣ=ӊ.Substring(1);if(A.Ӓ.
ContainsKey(Ӣ)){A.Ŏ($"Executing local command: {Ӣ}",false);return A.ӣ(A.Ӓ[Ӣ],ė.Ό);}}if(!ė.Ӥ&&Ӂ(ė.Ĕ)!=0)return null;if(A.Ӓ.
ContainsKey(ė.Ĕ))return A.ӣ(A.Ӓ[ė.Ĕ],ė.Ό);string ӥ="!"+ė.Ĕ;if(A.Ӓ.ContainsKey(ӥ))return A.ӣ(A.Ӓ[ӥ],ė.Ό);return null;}void Ӡ(Ė ė){
string ӊ=ė.ӡ;if(!ė.Ӥ){long Ӧ=Ӂ(ė.Ĕ);if(Ӧ!=0){ҵ.ӄ(Ӧ,ӊ);return;}}foreach(ˣ ӧ in Ҷ){if(ӧ.ˤ()==ė.Ĕ){var Ӕ="> "+ӊ;A.Ŏ(Ӕ);string ͳ=ӧ
.Ġ(ė);A.Ŏ(ͳ,false);return;}}if(!ė.Ӥ){long Ө=Ӄ(ė.Ĕ);if(Ө!=0){ҵ.ӄ(Ө,ӊ);return;}}A.Ŏ(ˈ.ˉ(Ě.Ҵ,ė.ӡ),false);}public List<string
>Ӫ(){var ө=new List<string>(Ҷ.Count+A.Ӓ.Count);for(int ɦ=0;ɦ<Ҷ.Count;ɦ++)ө.Add(Ҷ[ɦ].ˤ());foreach(var ǹ in A.Ӓ.Keys)ө.Add(
ǹ);return ө;}public void ӯ(long ӫ,List<string>Ӛ){if(ӫ==A.Â)return;var Ӭ=new HashSet<string>();var ӭ=new HashSet<string>()
;Ӛ.ForEach(Ӯ=>{if(Ӯ.StartsWith("!"))ӭ.Add(Ӯ.Substring(1));else Ӭ.Add(Ӯ);});ҷ[ӫ]=Ӭ;Ҹ[ӫ]=ӭ;}public long Ӄ(string Ӱ){foreach
(var q in ҷ){if(q.Value.Contains(Ӱ))return q.Key;}return 0;}public long Ӂ(string Ӱ){foreach(var q in Ҹ)if(q.Value.
Contains(Ӱ))return q.Key;return 0;}}public class ҹ:Ē{ę ē;public ҹ(ę ĕ){ē=ĕ;}public override string Ĕ=>"help";public
override string Ġ(Ė ė){var ӱ=new StringBuilder();ē.ϱ.ForEach(ӧ=>{ӱ.Append(ӧ.ˤ()).Append('\n');});return$"{ӱ}";}}public class Ė{
public string ӡ;public string Ĕ;public List<string>Ę=new List<string>();public Dictionary<string,string>Ό=new Dictionary<
string,string>();public bool Ӳ=false;public bool ӳ=false;public bool Ӥ=false;public Ė(string ӊ){ӡ=ӊ.Replace("\r","").Trim();Ӵ(
);}public string ģ(string ǹ){if(Ό.ContainsKey(ǹ))return Ό[ǹ];return"";}public static bool Ε(string Ώ){return Ώ?.Trim().
ToLower()=="true"||Ώ?.Trim()=="1";}void Ӵ(){foreach(string ɓ in ӵ(ӡ)){if(ɓ.StartsWith("--")){string[]ϫ=ɓ.Split('=');string ǹ=ϫ[
0].Substring(2);if(ϫ.Length==2){Ό.Add(ǹ,ϫ[1]);}else{Ό.Add(ǹ,"true");}}else{Ę.Add(ɓ);}}Ĕ=Ę[0];if(Ĕ.StartsWith("!!")){Ӥ=
true;Ĕ=Ĕ.Substring(2);}Ę.RemoveAt(0);}public static List<string>ӵ(string Ӷ){var ɏ=new List<string>();int ɦ=0;while(ɦ<Ӷ.
Length){if(Ӷ[ɦ]==' '){ɦ++;continue;}if(Ӷ[ɦ]=='"'){int ȕ=Ӷ.IndexOf('"',ɦ+1);if(ȕ==-1)ȕ=Ӷ.Length;ɏ.Add(Ӷ.Substring(ɦ+1,ȕ-ɦ-1));ɦ
=ȕ+1;}else{int ȕ=Ӷ.IndexOf(' ',ɦ);if(ȕ==-1)ȕ=Ӷ.Length;ɏ.Add(Ӷ.Substring(ɦ,ȕ-ɦ));ɦ=ȕ+1;}}return ɏ;}}public class Ӎ{string
ӷ;public string ӏ="self";public string ӓ="";public List<Ė>ϱ=new List<Ė>();public List<List<Ė>>Ә=new List<List<Ė>>();
public bool ӗ=>Ә.Count>0;List<Ė>Ӹ=new List<Ė>();public Ӎ(string Ɂ){ӷ=Ɂ.Trim();ӹ();}void Ӽ(string ė){string[]Ӻ=ė.Split(' ');
string ӻ=Ӻ[0];if(ӻ.StartsWith("@")){ӏ=ӻ.Substring(1);ӷ=ė.Substring(ӻ.Length);}if(ӻ=="*"){ӏ=ӻ;ӷ=ė.Substring(1);}}void ӹ(){Ӽ(ӷ);
if(ӽ(ӷ)){Ӿ(ӷ);}else{List<string>Ԁ=ӿ(ӷ);foreach(var ӊ in Ԁ){if(!string.IsNullOrWhiteSpace(ӊ))ϱ.Add(new Ė(ӊ.Trim()));}}}
static bool ӽ(string Ɂ){if(!Ɂ.Contains("{")||!Ɂ.Contains("}"))return false;int ɑ=0;foreach(char Ʀ in Ɂ){if(Ʀ=='{')ɑ++;else if(
Ʀ=='}')ɑ--;else if(ɑ==0&&!char.IsWhiteSpace(Ʀ))return false;}return true;}void Ӿ(string Ɂ){List<Ė>ԁ=null;StringBuilder Ԃ=
new StringBuilder();bool ԃ=false;int ɑ=0;for(int ɦ=0;ɦ<Ɂ.Length;ɦ++){char Ʀ=Ɂ[ɦ];if(Ʀ=='"')ԃ=!ԃ;if(!ԃ){if(Ʀ=='{'){ɑ++;if(ɑ
==1){ԁ=new List<Ė>();Ԃ.Clear();continue;}}else if(Ʀ=='}'){ɑ--;if(ɑ==0){string Ӯ=Ԃ.ToString().Trim();if(!string.
IsNullOrWhiteSpace(Ӯ))ԁ.Add(new Ė(Ӯ));if(ԁ.Count>0)Ә.Add(ԁ);ԁ=null;Ԃ.Clear();continue;}}else if(Ʀ==';'&&ɑ==1){string Ӯ=Ԃ.ToString().Trim()
;if(!string.IsNullOrWhiteSpace(Ӯ))ԁ.Add(new Ė(Ӯ));Ԃ.Clear();continue;}}if(ɑ>0)Ԃ.Append(Ʀ);}}public Ӎ ӑ(Dictionary<string,
string>Ԅ){foreach(Ė ė in ϱ){string Ԇ=ԅ(ė,Ԅ);Ӹ.Add(new Ė(Ԇ));}var ӱ=new StringBuilder();foreach(Ė ė in Ӹ)ӱ.Append(ė.ӡ).Append(
';');ӓ=ӱ.ToString();return this;}static string ԅ(Ė ė,Dictionary<string,string>Ԅ){string ӊ=ė.ӡ;Dictionary<string,string>Ԉ=ԇ(
Ԅ);ӊ=ԉ(ӊ,Ԉ);ӊ=Ԋ(ӊ);return ӊ;}List<string>ӿ(string Ɂ){bool ԃ=false;int ɑ=0;List<string>Ӛ=new List<string>();StringBuilder
Ԃ=new StringBuilder();foreach(char Ʀ in Ɂ){if(Ʀ=='"')ԃ=!ԃ;if(!ԃ){if(Ʀ=='{')ɑ++;else if(Ʀ=='}')ɑ--;}if(Ʀ==';'&&!ԃ&&ɑ==0){Ӛ
.Add(Ԃ.ToString().Trim());Ԃ.Clear();}else{Ԃ.Append(Ʀ);}}if(Ԃ.Length>0)Ӛ.Add(Ԃ.ToString().Trim());return Ӛ;}static
Dictionary<string,string>ԇ(Dictionary<string,string>Ԅ){Dictionary<string,string>Ԉ=new Dictionary<string,string>();var Ԍ=Ԅ.Keys.
OrderByDescending(ԋ=>ԋ.Length).ToList();Ԍ.ForEach(ǹ=>{Ԉ[ǹ]=ԍ(Ԅ[ǹ],Ԉ);});return Ԉ;}static string ԍ(string Ώ,Dictionary<string,string>Ԉ){
StringBuilder ӱ=new StringBuilder(Ώ);bool Ԏ;do{Ԏ=false;foreach(var q in Ԉ){string ǹ=q.Key;string ԏ=q.Value;if(Ԑ(ӱ.ToString(),ǹ)){ӱ.
Replace(ǹ,ԏ);Ԏ=true;}}}while(Ԏ);return$"{ӱ}";}static string ԉ(string ė,Dictionary<string,string>Ԉ){StringBuilder ӱ=new
StringBuilder(ė);foreach(var q in Ԉ){if(Ԑ($"{ӱ}",q.Key))ӱ.Replace(q.Key,q.Value);}return$"{ӱ}";}static bool Ԑ(string ʎ,string ԑ){
return System.Text.RegularExpressions.Regex.IsMatch(ʎ,$@"\b{ԑ}\b");}static string Ԋ(string ė){while(ė.Contains(";;"))ė=ė.
Replace(";;",";");return ė.Trim(';');}}public class Ԓ:Ē{ǲ ē;public override string Ĕ=>"var/set";public Ԓ(ǲ ĕ){ē=ĕ;}
public override string Ġ(Ė ė){if(ė.Ę.Count<2)return ę.Ě.ě;string Ã=ė.Ę[0];string Ώ=ė.Ę[1];bool ԓ=ė.Ό.ContainsKey("save");ē.Ԕ(Ã
,Ώ,ԓ);return ԓ?$"${Ã} = \"{Ώ}\" (saved)":$"${Ã} = \"{Ώ}\"";}}public class ԕ:d{}public class П:d{}public class ԗ{MyIni Ԗ;public ԗ(MyIni m){Ԗ=m;}public MyIni P(){Ԙ();return Ԗ;}void Ԙ(){if(Ԗ.ContainsSection("Commands")){string ԙ=Ԗ.ToString(
);ԙ=ԙ.Replace("[Commands]","[commands]");Ԗ.TryParse(ԙ);}if(!Ԗ.ContainsSection("channels")){string Ԛ=Ԗ.Get("security",
"passcodes").ToString();Ԗ.Set("channels","default",Ԛ);if(Ԗ.ContainsSection("security"))Ԗ.DeleteSection("security");}}}public class
ǲ:Μ{Dictionary<string,string>ԛ=new Dictionary<string,string>(){};public Dictionary<string,string>к=new
Dictionary<string,string>(){};string[]Ԝ=new string[]{"general","channels","variables","commands","hooks",};public
MyIni Ǹ=new MyIni();public MyIni Η=>Ǹ;public ǲ(A B):base(B){}void ԝ(){MyIniParseResult в;if(!Ǹ.TryParse(A.Θ.CustomData,
out в))throw new Exception($"{в}");}public override void c(){ɲ();A.Θ.CustomData=$"{new ԗ(Ǹ).P()}";A.W<ǡ>()?.Y<П>(this);Þ(
new Ԓ(this));}public void Ԟ(){ɲ();}void ɲ(){ԝ();з();ԟ();Ԡ();A.ԡ=δ("general.debug").ToLower()=="true";var Ԣ=δ("general.name"
);A.Ĕ=!string.IsNullOrEmpty(Ԣ)?ԣ(Ԣ):A.Å.CustomName;}public override void g(d e,object f){if(e is П)Ԟ();}public string δ(
string Ԥ){List<string>ԥ=new List<string>(Ԥ.Split('.'));if(ԥ.Count!=2)return$"";else{string Ԧ=ԥ[0];string ǹ=ԥ[1];return
$"{Ǹ.Get(Ԧ,ǹ)}";}}string ԣ(string Ӷ){if(Ӷ.Length>=2&&Ӷ[0]=='"'&&Ӷ[Ӷ.Length-1]=='"')return Ӷ.Substring(1,Ӷ.Length-2);return Ӷ;}void ԟ(){
A.ԧ.Clear();var Ա="variables";List<MyIniKey>Ƿ=new List<MyIniKey>();Ǹ.GetKeys(Ա,Ƿ);foreach(var ǹ in Ƿ){string Բ=ǹ.Name;
string Գ=$"{Ǹ.Get(Ա,Բ)}";if(Բ.StartsWith("$"))Բ=Բ.Substring(1);Գ=ԣ(Գ);A.ԧ[Բ]=Գ;}}void Ԡ(){A.Ӓ.Clear();List<MyIniKey>Ƿ=new List
<MyIniKey>();Ǹ.GetKeys("Commands",Ƿ);foreach(var ǹ in Ƿ){string Ӱ=ǹ.Name;string Դ=$"{Ǹ.Get("Commands",Ӱ)}".Replace("\r",
"").Replace("\n"," ").Trim();Դ=System.Text.RegularExpressions.Regex.Replace(Դ,@"\s+"," ");Դ=ԣ(Դ);A.Ӓ[Ӱ]=Դ;}}public void Ԕ(
string Ã,string Ώ,bool ԓ=false){A.ԧ[Ã]=Ώ;if(ԓ){Ǹ.Set("variables",Ã,Ώ);A.Θ.CustomData=$"{Ǹ}";}}void з(){foreach(string Ԧ in Ԝ)
if(!Ǹ.ContainsSection(Ԧ))Ǹ.AddSection(Ԧ);foreach(KeyValuePair<string,string>Ե in ԛ){string[]л=Ե.Key.Split('.');string Ԧ=л[
0];string ǹ=л[1];string Ώ=Ե.Value;if(Ǹ.Get(Ԧ,ǹ).IsEmpty)Ǹ.Set(Ԧ,ǹ,Ώ);}A.Θ.CustomData=$"{Ǹ}";}}public class Է:Ē{Զ
ē;public override string Ĕ=>"connector/lock";public Է(Զ ĕ){ē=ĕ;}public override string Ġ(Ė ė){if(ė.Ę.Count==0)return ę.Ě.
ˎ;else{string Ը=ė.Ę[0];List<IMyShipConnector>Թ=ē.ˆ<IMyShipConnector>(Ը);if(Թ.Count==0)return ˈ.ˉ(ˊ.ˋ,Ը);Թ.ForEach(Ʊ=>ē.Ժ(
Ʊ));return ˈ.ˉ(ˊ.Ҧ,Ը);}}}public class Ի:Ē{Զ ē;public override string Ĕ=>"connector/toggle";public Ի(Զ ĕ){ē=ĕ;}
public override string Ġ(Ė ė){if(ė.Ę.Count==0)return ę.Ě.ě;else{string Ը=ė.Ę[0];List<IMyShipConnector>Թ=ē.ˆ<IMyShipConnector>(
Ը);if(Թ.Count==0)return ˈ.ˉ(ˊ.ˋ,Ը);Թ.ForEach(Ʊ=>ē.Լ(Ʊ));return ˈ.ˉ(ˊ.ү,Ը);}}}public class Խ:Ē{Զ ē;public
override string Ĕ=>"connector/unlock";public Խ(Զ ĕ){ē=ĕ;}public override string Ġ(Ė ė){if(ė.Ę.Count==0)return ę.Ě.ě;else{string
Ը=ė.Ę[0];List<IMyShipConnector>Թ=ē.ˆ<IMyShipConnector>(Ը);if(Թ.Count==0)return ˈ.ˉ(ˊ.ˋ,Ը);Թ.ForEach(Ʊ=>ē.Ծ(Ʊ));return ˈ.ˉ
(ˊ.ҧ,Ը);}}}public class Զ:Μ{U U;public Զ(A B):base(B){}public override void c(){U=A.W<U>();Þ(new Է(this));Þ(new Խ(this));
Þ(new Ի(this));ϻ<IMyShipConnector>(Ʊ=>Ʊ.Status,(u,ȵ)=>Կ(u as IMyShipConnector,ȵ));}protected void Կ(IMyShipConnector Ʊ,
object Հ){var ӈ=Հ as MyShipConnectorStatus?;var Ձ=ϰ.ContainsKey(Ʊ.EntityId)?ϰ[Ʊ.EntityId]as MyShipConnectorStatus?:null;if(ӈ==
MyShipConnectorStatus.Connected){ï<ŀ>(Ʊ);U.ч(Ʊ,"onLock");}else if((ӈ==MyShipConnectorStatus.Connectable&&Ձ==MyShipConnectorStatus.Connected)
||ӈ==MyShipConnectorStatus.Unconnected){ï<М>(Ʊ);U.ч(Ʊ,"onUnlock");}else if(ӈ==MyShipConnectorStatus.Connectable){ï<Ł>(Ʊ);U
.ч(Ʊ,"onReady");}}public void Ժ(IMyShipConnector Ʊ){Ʊ.Connect();}public void Ծ(IMyShipConnector Ʊ){Ʊ.Disconnect();}public
void Լ(IMyShipConnector Ʊ){if(Ʊ.Status==MyShipConnectorStatus.Connected)Ծ(Ʊ);else Ժ(Ʊ);}}public class ŀ:d{}public class Ł:d{
}public class М:d{}public class È{public IMyTextSurface ɱ;public IMyTerminalBlock ʨ;public RectangleF ʚ;
protected float Ղ=4f;protected float Ճ=0;protected const float Մ=16f;protected const float Յ=1f;public float Ն=Յ;protected const
float Շ=1f;public float ʽ;public MySpriteDrawFrame ʸ;public bool ʻ;public MyIni ǲ;public Vector2 ʖ,Ո,ʼ,Չ,Պ;public È(
IMyTextSurface w,IMyTerminalBlock u,MyIni Ы,bool Ջ=false){ɱ=w;ʨ=u;ǲ=Ы;ʻ=Ջ;Ռ();Ս();}public virtual void ʬ(MyIni m){ǲ=m;}protected
virtual void Ս(){ʚ=Վ(ɱ,Ղ);float Տ=ʻ?Ճ:Ղ;float Ր=ʻ?Ճ:Ղ;ʖ=ʚ.Position+new Vector2(Տ,Ր);Ո=new Vector2(ʚ.X+ʚ.Width-Տ,ʚ.Y+Ր);ʼ=new
Vector2(ʚ.X+Տ,ʚ.Y+ʚ.Height-Ր);Չ=new Vector2(ʚ.X+ʚ.Width-Տ,ʚ.Y+ʚ.Height-Ր);Պ=new Vector2(ʚ.X+(ʚ.Width/2f),ʚ.Y+(ʚ.Height/2f));ʽ=Ց
();}protected virtual void Ռ(){}public virtual void ʳ(){ʸ=ɱ.DrawFrame();Random Ւ=new Random();Vector2 Փ=new Vector2((
float)Ւ.NextDouble(),(float)Ւ.NextDouble());ʸ.Add(Ք.Օ("SquareSimple",Փ,Փ,Color.Transparent));}protected RectangleF Վ(
IMyTextSurface w,float Ֆ){var ՙ=w.SurfaceSize;return new RectangleF(w.TextureSize.X/2f-ՙ.X/2f+Ֆ,w.TextureSize.Y/2f-ՙ.Y/2f+Ֆ,ՙ.X-(4*Ֆ),
ՙ.Y-(4*Ֆ));}public void զ(Vector3D ա,Vector3D բ){var գ=բ-ա;var դ=ʚ.Width/գ.X;var ե=ʚ.Height/գ.Y;Ն=Յ*(float)Math.Min(դ,ե);
}public virtual float Ց(){float է=ʚ.Width/1024f;float ը=Մ*է*Շ;ը=Math.Max(ը,8f);ը=Math.Min(ը,32f);return ը;}public float ɺ
()=>Math.Min(1f,ʽ/Մ);public bool թ=>ʚ.Width/ʚ.Height>1.8f;public RectangleF լ(RectangleF ժ){RectangleF ի=ʚ;ʚ=ժ;return ի;}
public void ɼ(Vector2 ɷ,float ʇ,Color ʉ,float խ=0){float ծ=MathHelper.ToRadians(խ);ʸ.Add(Ք.Օ("SquareSimple",ɷ,new Vector2(ʇ,ʇ)
,ʉ,ծ));}public void ʕ(Vector2 Ȕ,Vector2 ȕ,Color ʉ,float կ){var հ=(Ȕ+ȕ)/2;var ձ=Vector2.Distance(Ȕ,ȕ);var ղ=(float)Math.
Atan2(ȕ.Y-Ȕ.Y,ȕ.X-Ȕ.X);ʸ.Add(Ք.Օ("SquareSimple",հ,new Vector2(ձ,կ),ʉ,ղ));}public void չ(Vector2 ɷ,float ʇ,Color ճ,Color մ){
float յ=ʇ/2f;Vector2[]ն=new Vector2[8];float շ=MathHelper.PiOver4;float ո=MathHelper.Pi/8;for(int ɦ=0;ɦ<8;ɦ++){float ղ=ɦ*շ+ո;
ն[ɦ]=ɷ+new Vector2((float)Math.Cos(ղ),(float)Math.Sin(ղ))*յ;}ɼ(ɷ,ʇ,մ);for(int ɦ=0;ɦ<8;ɦ++){ʕ(ն[ɦ],ն[(ɦ+1)%8],ճ,1f);}}
public void ʋ(Vector2 ɷ,float պ,Color ʉ){ʸ.Add(Ք.Օ("Circle",ɷ,new Vector2(պ,պ),ʉ));}public void ʈ(Vector2 ɷ,float ʇ,Color ʉ,
float խ=0){float ծ=MathHelper.ToRadians(խ);ʸ.Add(Ք.Օ("Triangle",ɷ,new Vector2(ʇ,ʇ),ʉ,ծ));}public void ʴ(){ʸ.Add(Ք.Օ(
"SquareSimple",ɱ.TextureSize/2f,ɱ.TextureSize,Color.Black));}public void ɾ(string ʎ,Vector2 ɢ,Color ʉ,string ջ,float ռ=1f){ʸ.Add(new
MySprite{Type=SpriteType.TEXT,Data=ʎ,Position=ɢ,RotationOrScale=ɺ()*ռ,Color=ʉ,Alignment=TextAlignment.LEFT,FontId=ջ});}public
void ɾ(string ʎ,Vector2 ɢ,Color ʉ,float ռ=1f)=>ɾ(ʎ,ɢ,ʉ,"Monospace",ռ);public void ɾ(string ʎ,Vector2 ɢ,float ռ=1f)=>ɾ(ʎ,ɢ,
Color.White,"Monospace",ռ);public void ս(string ʎ,Vector2 ɢ,Color ʉ,float ռ=1f){ʸ.Add(new MySprite{Type=SpriteType.TEXT,Data=
ʎ,Position=ɢ,RotationOrScale=ɺ()*ռ,Color=ʉ,Alignment=TextAlignment.CENTER,FontId="Monospace"});}public void ʷ(){Vector2 ɷ
=new Vector2(ʚ.X+ʚ.Width,ʚ.Y+ʚ.Height);float ʇ=ʻ?20:40;ɷ-=new Vector2(ʇ/2,ʇ/2);չ(ɷ,ʇ,Color.White,Color.Black);ʋ(ɷ,ʇ*0.4f,
Color.Red);}public void վ(){float ռ=ɺ();ʋ(ʖ,5*ռ,Color.Red);ʋ(Ո,5*ռ,Color.Red);ʋ(ʼ,5*ռ,Color.Red);ʋ(Չ,5*ռ,Color.Red);ʋ(Պ,5*ռ,
Color.Red);Vector2 ɽ=ʻ?ʼ-new Vector2(0,1.5f*ʽ):ʼ-new Vector2(0,2f*ʽ);ɾ($"scale={Ն:F2}",ɽ,Color.White,"White");}public static
string É(string տ,string ր,int ց){const int ւ=5;int փ=ց-ր.Length-ւ;if(փ<0)return ր.Substring(Math.Max(0,ր.Length-ց));string ք=
տ.Length>փ?տ.Substring(0,փ):տ;int օ=ց-ք.Length-ր.Length;if(օ<ւ){int ֆ=ւ-օ;ք=ք.Substring(0,Math.Max(0,ք.Length-ֆ));օ=ց-ք.
Length-ր.Length;}string և=new string('.',օ);return$"{ք}{և}{ր}";}}public class ו:Μ{public const string א="LogView";S S;Ɵ Ɵ;U U;
HashSet<IMyTextSurface>ב=new HashSet<IMyTextSurface>();List<IMyTextSurface>ג=new List<IMyTextSurface>();
Dictionary<string,List<IMyTextSurface>>ד=new Dictionary<string,List<IMyTextSurface>>();public static float ה=1;public ו
(A B):base(B){}public override void c(){S=A.W<S>();Ɵ=A.W<Ɵ>();U=A.W<U>();Y<X>();ז();}public override void g(d e,object f)
{if(e is X&&(f is IMyTextPanel||f is IMyTextSurfaceProvider)){ז();}}string ך(string ח=""){return
$" {A.C} - {ח}     ({S.ҟ()})\n"+$" {A.Ĕ} *{A.ט}                                  {י()}\n"+"------------------------------------------------------";}
public string י(){string כ=A.W<Τ>().Ξ.Count()>0?"M":"   ";string ל=A.W<Ĳ>().ל.Count()>0?"C":"    ";string ם=
$"{A.W<T>()?.x.Count()??0}";string מ=A.ǋ?"A":"   ";string ן=S.ҝ>0?"W":"   ";return String.Join("  ",ן,מ,ל,כ,ם);}public List<IMyTextSurface>ף(string
Ã){var נ=Ã.Split(':');var ס=new List<IMyTextSurface>();if(נ.Length>1){string х=נ[0].Trim();int ע=int.Parse(נ[1].Trim());ס
=U.ˆ<IMyTerminalBlock>(х).Where(u=>u is IMyTextSurfaceProvider).Select(u=>((IMyTextSurfaceProvider)u).GetSurface(ע)).
Where(w=>w!=null).ToList();}else{ס=U.ˆ<IMyTerminalBlock>(Ã).Where(u=>u is IMyTextSurface).Select(u=>(IMyTextSurface)u).ToList
();}return ס;}void ץ(){U.j<IMyTextPanel>()?.ForEach(k=>פ(k));}void פ(IMyTextPanel k){MyIni m=U.l(k);foreach(n q in o.p(m)
){if(q.r!=0)continue;ב.Add(k);if(string.Equals(q.s,א,StringComparison.OrdinalIgnoreCase)&&o.צ(q.ק,A)){k.ContentType=
ContentType.TEXT_AND_IMAGE;ג.Add(k);}ר(q.s,k);break;}}void ש(){var t=new List<IMyTerminalBlock>();t.AddRange(U.j<IMyCockpit>());t.
AddRange(U.j<IMyProgrammableBlock>());t.AddRange(U.j<IMySoundBlock>());t.ForEach(u=>ש(u));}void ש(IMyTerminalBlock u){
IMyTextSurfaceProvider v=u as IMyTextSurfaceProvider;if(v==null)return;MyIni m=U.l(u);foreach(n q in o.p(m)){if(q.r>=v.SurfaceCount)continue;
IMyTextSurface w=v.GetSurface(q.r);ב.Add(w);if(string.Equals(q.s,א,StringComparison.OrdinalIgnoreCase)&&o.צ(q.ק,A)){w.ContentType=
ContentType.TEXT_AND_IMAGE;ג.Add(w);}ר(q.s,w);}}public void ז(){ת();ץ();ש();}void ת(){ב.Clear();ג.Clear();foreach(var װ in ד.Values
)װ.Clear();}public void ײ(){ױ();}string ء(){string ؠ=string.Join("\n",Ɵ.x);return ך("LOG")+"\n"+ؠ;}void ױ(){string ؠ=ء();
ג.ForEach(w=>w.WriteText($"{ؠ}",false));}public List<IMyTextSurface>أ(string آ){string ǹ=آ.ToLower();if(ד.ContainsKey(ǹ))
return ד[ǹ];return new List<IMyTextSurface>();}void ר(string آ,IMyTextSurface w){string ǹ=آ.ToLower();if(!ד.ContainsKey(ǹ))ד[ǹ
]=new List<IMyTextSurface>();ד[ǹ].Add(w);}}public struct n{public int r;public string s;public string ק;}public static
class o{const string ؤ="surfaces";public static List<n>p(MyIni إ){var ئ=new List<n>();var Ƿ=new List<MyIniKey>();إ.GetKeys(ؤ,
Ƿ);foreach(MyIniKey ǹ in Ƿ){int Ȃ;if(!int.TryParse(ǹ.Name,out Ȃ))continue;string ا=إ.Get(ؤ,ǹ.Name).ToString().Trim();if(
string.IsNullOrEmpty(ا))continue;List<string>ɏ=Ė.ӵ(ا);string آ=ɏ[0];string ب=ɏ.Count>1?ɏ[1]:null;ئ.Add(new n{r=Ȃ,s=آ,ק=ب});}
return ئ;}public static bool צ(string ة,A B){if(string.IsNullOrEmpty(ة))return true;return string.Equals(ة,B.C,
StringComparison.OrdinalIgnoreCase)||string.Equals(ة,B.ט,StringComparison.OrdinalIgnoreCase);}}public static class Ք{public static
MySprite Օ(string ت,Vector2 ɢ,Vector2 ʇ,Color ʉ,float ծ=0f){return new MySprite{Type=SpriteType.TEXTURE,Data=ت,Position=ɢ,Size=ʇ
,Color=ʉ,Alignment=TextAlignment.CENTER,RotationOrScale=ծ};}}public class ǡ:Μ{public ǡ(A B):base(B){}Dictionary<Type,HashSet<Ϭ>>ث=new Dictionary<Type,HashSet<Ϭ>>();public void Y<Ђ>(Ϭ ĕ)where Ђ:d{var ج=typeof(Ђ);if(!ث.ContainsKey(ج)
)ث[ج]=new HashSet<Ϭ>();ث[ج].Add(ĕ);}public bool ح<Ђ>(Ϭ ĕ){var ج=typeof(Ђ);if(ث.ContainsKey(ج))return ث[ج].Contains(ĕ);
return false;}public void خ<Ђ>(Ϭ ĕ)where Ђ:d{var ج=typeof(Ђ);if(ث.ContainsKey(ج))ث[ج].Remove(ĕ);}public new void ï<Ђ>(object f
=null)where Ђ:d,new(){ï(new Ђ(),f);}public new void ï(d e,object f=null){var ج=e.GetType();if(ث.ContainsKey(ج))ث[ج].
ToList().ForEach(ĕ=>ĕ.g(e,f));}}public interface d{}public interface Ј:Ϭ{}public interface E:Ϭ{}public interface Ϭ{void c();
IEnumerator<double>ϳ();void P();void g(d e,object f);string ϴ();List<ˣ>Ѓ();}public class د:Ē{A A;public override string Ĕ
=>"ping";public د(A B){A=B;}public override string Ġ(Ė ė){A.W<Ĳ>().Λ();return"Pinging all grids";}}public class ذ:d{}
public class ر:d{}public class ز:d{}public abstract class Ɖ{public Dictionary<string,object>س=new Dictionary<string,object>();
public Dictionary<string,object>ƕ=new Dictionary<string,object>();public HashSet<string>ʂ=new HashSet<string>();public string
Â{get;}=ش();public Ɖ(Dictionary<string,object>œ,Dictionary<string,object>ص){ƕ=œ;س=ص;if(!س.ContainsKey("Id"))س["Id"]=ش();}
public object Ő(string ǹ){object Ώ;return ƕ.TryGetValue(ǹ,out Ώ)?Ώ??"":"";}public string Œ(string ǹ)=>$"{Ő(ǹ)}";public float ض
(string ǹ){float Ώ;float.TryParse(Œ(ǹ),out Ώ);return Ώ;}public double ط(string ǹ){double Ώ;double.TryParse(Œ(ǹ),out Ώ);
return Ώ;}public object ŏ(string ǹ){object Ώ;return س.TryGetValue(ǹ,out Ώ)?Ώ??"":"";}public string Ƌ(string ǹ)=>$"{ŏ(ǹ)}";
public float ظ(string ǹ){float Ώ;float.TryParse(Ƌ(ǹ),out Ώ);return Ώ;}public double ع(string ǹ){double Ώ;double.TryParse(Ƌ(ǹ),
out Ώ);return Ώ;}public long غ(string ǹ){long Ώ;long.TryParse(Ƌ(ǹ),out Ώ);return Ώ;}static string ش(){long ػ=DateTime.
UtcNow.Ticks;int ؼ=new Random().Next(0,1000);return$"{ػ}_{ؼ}";}public virtual string ϕ(){string ؽ="header";string ؾ="body";
string ؿ=ȇ.Ȉ(س);string ـ=ȇ.Ȉ(ƕ);return$"<{ؽ}>{ؿ}</{ؽ}>"+$"<{ؾ}>{ـ}</{ؾ}>";}public static string م(string Ɲ,string ف){string ق=
$"<{ف}>";string ك=$"</{ف}>";int у=Ɲ.IndexOf(ق)+ق.Length;int ل=Ɲ.IndexOf(ك);if(у==-1||ل==-1||у>=ل)return"";return Ɲ.Substring(у,ل
-у).Trim();}}public class Ĳ:Μ{class Ě{public const string ن="Cannot de-serialize message.";public const string ه=
"No active request found for RespondingToId: {0}";}S S;Ɵ Ɵ;T T;ǡ ǡ;public І І;public Dictionary<string,Action<Ɖ>>ל=new Dictionary<string,Action<Ɖ>>();const
string و=".construct";IMyUnicastListener ى;List<IMyBroadcastListener>ي=new List<IMyBroadcastListener>();public
Dictionary<string,string>ʂ=new Dictionary<string,string>();long ٮ=0;public bool Ҽ=>ٯ()==A.Â;public Ĳ(A B):base(B){І=new І();}
public override void c(){S=A.W<S>();Ɵ=A.W<Ɵ>();T=A.W<T>();ǡ=A.W<ǡ>();A.Þ(new د(A));ٱ();ٲ();ǡ.Y<П>(this);І.Ї("ping",Ņ=>Ŕ(Ņ,Ō.ŕ.
ٳ));І.Ї("sync",Ņ=>ٴ(Ņ));І.Ї("almanac",Ņ=>ٵ(Ņ));S.Қ(()=>Κ(),0.5);S.a(Κ,5);S.a(Λ,2);}Ō ٴ(ō Ņ){ٶ(Ņ);var ٷ=A.W<ę>().Ӫ();var Ɗ
=Ŕ(Ņ,Ō.ŕ.ٳ,new Dictionary<string,object>{{"Commands",string.Join(",",ٷ)}});Ɗ.ʂ.Add(و);return Ɗ;}void ٱ(){ʂ.Clear();var m=
A.W<ǲ>();var Ƿ=new List<MyIniKey>();m.Η.GetKeys("channels",Ƿ);Ƿ.ForEach(ǹ=>{var Ώ=m.Η.Get(ǹ.Section,ǹ.Name);ʂ[ǹ.Name]=
$"{Ώ}";});}void ٲ(){ى=A.ٸ.UnicastListener;ى.SetMessageCallback();IMyBroadcastListener ٹ=A.ٸ.RegisterBroadcastListener(و);ٹ.
SetMessageCallback();ي.Add(ٹ);foreach(var ٺ in ʂ){IMyBroadcastListener ٻ=A.ٸ.RegisterBroadcastListener(ٺ.Key);ٻ.SetMessageCallback();ي.Add
(ٻ);}}public override void g(d e,object f){if(e is П){ٱ();ٲ();S.Қ(Κ,0);}}public void پ(){while(ى?.HasPendingMessage==true
)ټ(ى.AcceptMessage());ي.ForEach(ٽ=>{while(ٽ?.HasPendingMessage==true)ټ(ٽ.AcceptMessage());});ל.Clear();}string ڄ(
MyIGCMessage Ɲ){string ٿ=$"{Ɲ.Data}";string ٺ=Ɲ.Tag;string ځ=ڀ(ٺ);if(ځ=="")return ٿ;else return ڂ.ڃ(ٿ,ځ);}public void ټ(MyIGCMessage
Ɲ){string ٿ=$"{Ɲ.Data}";ٿ=ڂ.څ(ٿ)?ڄ(Ɲ):ٿ;if(ٿ.StartsWith("REQUEST::")){چ(ō.ڇ(ٿ),Ɲ.Tag,ڈ=>ډ((ō)ڈ));}else if(ٿ.StartsWith(
"RESPONSE::")){چ(Ō.ڇ(ٿ),Ɲ.Tag,ڈ=>ڊ((Ō)ڈ));}}void چ(Ɖ ڋ,string ڌ,Action<Ɖ>Ч){if(ڋ!=null){ڋ.ʂ.Add(ڌ);bool ڍ=ڌ!=و;if(ڍ){ڎ(ڋ);ڏ(ڋ);}Ч(ڋ)
;}else{Ɵ.Ǽ(Ě.ن);}}ƨ ڎ(Ɖ Ɲ){long π=Ɲ.غ("OriginId");string ڐ=Ɲ.Ƌ("GridId");string ȫ=!string.IsNullOrEmpty(ڐ)?ڐ:$"{π}";
Vector3D?ΰ=null;Vector3D?α=null;double ϝ,Ϟ,ϟ,Ϡ,ϡ,Ϣ;if(double.TryParse(Ɲ.Ƌ("Fx"),out ϝ)&&double.TryParse(Ɲ.Ƌ("Fy"),out Ϟ)&&double
.TryParse(Ɲ.Ƌ("Fz"),out ϟ)){ΰ=new Vector3D(ϝ,Ϟ,ϟ);}if(double.TryParse(Ɲ.Ƌ("Ux"),out Ϡ)&&double.TryParse(Ɲ.Ƌ("Uy"),out ϡ)
&&double.TryParse(Ɲ.Ƌ("Uz"),out Ϣ)){α=new Vector3D(Ϡ,ϡ,Ϣ);}return T.ϊ(ȫ,π,Ɲ.Ƌ("OriginName"),new Vector3D(Ɲ.ظ("X"),Ɲ.ظ("Y")
,Ɲ.ظ("Z")),Ɲ.ظ("Speed"),Ɲ.ʂ,ڑ(π),ΰ,α);}void ډ(ō Ņ){if(Ņ==null)return;ǡ.ï<ر>();Ō Ɗ=І.ڒ(Ņ.Ƌ("Path"),Ņ);if(Ɗ!=null)Ư(Ɗ.غ(
"TargetId"),Ɗ,null);}void ڊ(Ō Ɗ){if(Ɗ==null)return;if(Ɗ.س.ContainsKey("RespondingToId")){string ړ=Ɗ.Ƌ("RespondingToId");if(ל.
ContainsKey(ړ))ל[ړ]?.Invoke(Ɗ);}else Ɵ.Ǽ($"Response missing 'RespondingToId' header: {ȇ.Ȉ(Ɗ.س)}");}public void Ư(long ڔ,Ɖ Ɲ,Action<
Ɖ>ڕ){ל[$"{Ɲ.س["Id"]}"]=ڕ;bool Ӈ=false;var ږ=Ɲ.ʂ.OrderBy(Ʀ=>Ʀ=="*").ToHashSet();if(ږ.Count==0)ږ.Add(و);foreach(string ٺ in
ږ){string ژ=ڂ.ڗ(Ɲ.ϕ(),ڀ(ٺ));Ӈ=A.ٸ.SendUnicastMessage(ڔ,ٺ,ژ);if(Ӈ)break;}if(Ӈ)ǡ.ï<ز>();else ǡ.ï<ذ>();}public void ڙ(ō Ņ,
Action<Ɖ>ڕ){ל[Ņ.Â]=ڕ;foreach(var ٺ in ʂ){string ژ=ڂ.ڗ(Ņ.ϕ(),ڀ(ٺ.Key));A.ٸ.SendBroadcastMessage(ٺ.Key,ژ);}ǡ.ï<ز>();}public void
Ӗ(string í,Ӎ Ɂ){ƨ ƪ=T.Ʃ(í);if(ƪ==null){Ɵ.Ǽ($"Target '{í}' not found in Almanac.");return;}if(ƪ.ȃ!=ƨ.ώ["grid"]){Ɵ.Ǽ(
$"Target '{í}' is not a grid (type: {ƪ.ȃ}).");return;}ō Ņ=ښ(Ɂ.ӓ).Ƭ(ƪ);long ڛ=ƪ.μ!=0?ƪ.μ:ƪ.ư();Ư(ڛ,Ņ,null);}public void ӕ(Ӎ Ɂ){T.ȉ("grid").ForEach(ʍ=>Ӗ(ʍ.Â,Ɂ));}ō ښ(
string ė){return ƫ("command",new Dictionary<string,object>{{"Command",ė}});}Dictionary<string,object>ڞ(){MatrixD Ą=A.ί();
Vector3D å=A.ź();return new Dictionary<string,object>{{"OriginId",$"{A.Â}"},{"GridId",$"{A.ڜ}"},{"OriginName",A.Ĕ},{"X",$"{å.X}"
},{"Y",$"{å.Y}"},{"Z",$"{å.Z}"},{"SafeRadius",$"{A.γ.Radius}"},{"Gravity",$"{A?.Ś()}"},{"Speed",$"{A?.ڝ()}"},{"Fx",
$"{Ą.Forward.X}"},{"Fy",$"{Ą.Forward.Y}"},{"Fz",$"{Ą.Forward.Z}"},{"Ux",$"{Ą.Up.X}"},{"Uy",$"{Ą.Up.Y}"},{"Uz",$"{Ą.Up.Z}"}};}public ō ƫ(
string Є,Dictionary<string,object>ڟ=null,Dictionary<string,object>ڠ=null){Dictionary<string,object>ص=ڞ();ص["Path"]=Є;
Dictionary<string,object>œ=new Dictionary<string,object>();ڡ(ص,ڠ);ڡ(œ,ڟ);return new ō(œ,ص);}public Ō Ŕ(ō Ņ,Ō.ŕ ڢ,Dictionary<string
,object>ڟ=null,Dictionary<string,object>ڠ=null){Dictionary<string,object>ڣ=ڞ();Dictionary<string,object>ڤ=new Dictionary<
string,object>(){{"Status",$"{Ō.ƌ(ڢ)}"},{"TargetId",Ņ.س["OriginId"]},{"TargetName",Ņ.س["OriginName"]},{"RespondingToId",Ņ.س[
"Id"]},};Dictionary<string,object>ŉ=new Dictionary<string,object>();ڡ(ڤ,ڣ);ڡ(ڤ,ڠ);ڡ(ŉ,ڟ);return new Ō(ŉ,ڤ);}public void Λ(){
if(!Ҽ)return;var ς=ʂ.Keys.ToList();ō Ņ=ƫ("ping");Ņ.ʂ=new HashSet<string>(ς);ڙ(Ņ,null);}public void Κ(){var ڥ=A.W<ę>().Ӫ();
ō Ņ=ƫ("sync",new Dictionary<string,object>{{"Commands",string.Join(",",ڥ)}},new Dictionary<string,object>{{"OriginName",A
.Ĕ}});ڦ(Ņ,Ɗ=>ڧ(Ɗ));}void ڧ(Ɖ Ɗ){ٶ(Ɗ);ٮ=0;}public void ӄ(long ڛ,string ė){ō Ņ=ƫ("localcmd",new Dictionary<string,object>{{
"Command",ė}});A.ٸ.SendUnicastMessage(ڛ,و,Ņ.ϕ());A.Ŏ($"> @local {ė}");}public void ڦ(ō Ņ,Action<Ɖ>ڕ){ל[Ņ.Â]=ڕ;A.ٸ.
SendBroadcastMessage(و,Ņ.ϕ(),TransmissionDistance.CurrentConstruct);}string ڀ(string ٺ){return ʂ.ContainsKey(ٺ)?ʂ[ٺ]:"";}void ڡ(Dictionary<
string,object>í,Dictionary<string,object>ة){if(ة==null)return;foreach(KeyValuePair<string,object>q in ة)í[q.Key]=q.Value;}void
ٶ(Ɖ Ɲ){long π=Ɲ.غ("OriginId");string ڨ=Ɲ.Œ("Commands");if(!string.IsNullOrEmpty(ڨ)&&π!=A.Â){var Ӛ=new List<string>(ڨ.
Split(','));A.W<ę>().ӯ(π,Ӛ);}}long ٯ(){if(ٮ!=0)return ٮ;long ک=A.Â;var ڥ=A.W<ę>().ҷ;foreach(var ӫ in ڥ.Keys)if(ӫ<ک)ک=ӫ;if(ڥ.
Count>0)ٮ=ک;return ک;}Ō ٵ(ō Ņ){if(!Ҽ)ڎ(Ņ);return null;}static string[]ڪ={"OriginId","GridId","OriginName","X","Y",
"Z","Speed","SafeRadius"};void ڏ(Ɖ Ɲ){if(!Ҽ)return;var ص=new Dictionary<string,object>();foreach(string ǹ in ڪ)ص[ǹ]=Ɲ.Ƌ(ǹ);
ō Ņ=ƫ("almanac",null,ص);A.ٸ.SendBroadcastMessage(و,Ņ.ϕ(),TransmissionDistance.CurrentConstruct);}bool ڑ(long π){return A.
W<U>().j<IMyProgrammableBlock>().Any(ګ=>ګ.EntityId==π);}}public class ō:Ɖ{public string ڔ;public string ڬ;public ō(
Dictionary<string,object>œ,Dictionary<string,object>ص):base(œ,ص){}public ō Ƭ(ƨ ƪ){س["TargetId"]=$"{ƪ.Â}";س["TargetName"]=ƪ.z??ƪ.Â;
ʂ=ƪ.ʂ;return this;}public override string ϕ()=>"REQUEST::"+base.ϕ();public static ō ڇ(string Ɲ){Ɲ=Ɲ.Replace("REQUEST::",
"");string ڭ=م(Ɲ,"header");string ڮ=م(Ɲ,"body");return new ō(ȇ.η(ڮ),ȇ.η(ڭ));}}public class Ō:Ɖ{public enum ŕ{ٳ=200,Ӆ=201,گ
=401,ڰ=404,Ҿ=500,ŗ=600,ڱ=601,ڲ=602,ڳ=603,Ŗ=604,}public Ō(Dictionary<string,object>œ,Dictionary<string,object>ص):base(œ,ص)
{}public override string ϕ()=>"RESPONSE::"+base.ϕ();public static Ō ڇ(string Ɲ){Ɲ=Ɲ.Replace("RESPONSE::","");string ڭ=م(Ɲ
,"header");string ڮ=م(Ɲ,"body");return new Ō(ȇ.η(ڮ),ȇ.η(ڭ));}static public int ƌ(ŕ ڢ)=>(int)ڢ;}public class ڶ{public
string ڴ{get;}public Func<ō,Ō>ڵ{get;}public ڶ(string Є,Func<ō,Ō>Ч){ڴ=Є;ڵ=Ч;}}public class І{public List<ڶ>ڷ=new List<
ڶ>();public Ō ڒ(string Є,ō Ņ){var Ѕ=ڷ.FirstOrDefault(y=>y.ڴ==Є);return Ѕ?.ڵ?.Invoke(Ņ);}public void Ї(string Є,Func<ō,Ō>Ѕ
){ڷ.Add(new ڶ(Є,Ѕ));}}public class ڸ:Ē{ȅ ē;public override string Ĕ=>"get";public ڸ(ȅ ĕ){ē=ĕ;}public override
string Ġ(Ė ė){if(ė.Ę.Count==0)return ę.Ě.ě;string ǹ=ė.Ę[0];string Ώ=ē.δ(ǹ);return Ώ;}}public class ڹ:Ē{ȅ ē;public
override string Ĕ=>"set";public ڹ(ȅ ĕ){ē=ĕ;}public override string Ġ(Ė ė){if(ė.Ę.Count==0)return ę.Ě.ě;else if(ė.Ę.Count>=2){
string ǹ=ė.Ę[0];string Ώ=ė.Ę[1];ē.Ȇ(ǹ,Ώ);return$"{ǹ}={Ώ}";}return ę.Ě.ˎ;}}public class ȅ:Μ{public string ں{get;set;}=
"";Dictionary<string,object>ڻ=new Dictionary<string,object>();bool ڼ=false;public ȅ(A B):base(B){ڻ=ȇ.η(A.Ғ.
Storage);}public override void c(){Þ(new ڹ(this));Þ(new ڸ(this));}public string ڽ(){ں=ȇ.Ȉ(ڻ);ڼ=false;return ں;}public bool Ƞ(){
ڻ.Clear();ں="";return ڼ=true;}public string δ(string ǹ){string Ώ="";if(ڻ.ContainsKey(ǹ))Ώ+=$"{ڻ[ǹ]}";return Ώ;}public
bool Ȇ(string ǹ,string Ώ){ڻ[ǹ]=Ώ;ڼ=true;return ڼ;}}public class Ɵ:Μ{const int ھ=30;public List<string>x{get;}=new List<
string>();public Ɵ(A B):base(B){}public void Ơ(string ƪ){Ȁ(ƪ,"Info");}public void Ǽ(string ƪ){Ȁ(ƪ,"Error");}void Ȁ(string ƪ,
string Ŋ=""){string ڿ=DateTime.Now.ToString("HH:mm:ss");string ۀ=Ŋ!=""?"."+Ŋ:"";if(x.Count>ھ)x.RemoveAt(x.Count-1);x.Insert(0,
$"{ڿ}{ۀ} {ƪ}");}}public class Р:d{}public class С:d{}public class ہ:Μ{U U;public ہ(A B):base(B){}public override void c(){U=A.W<U>();
ϻ<IMyMechanicalConnectionBlock>(ۂ=>ۂ.IsAttached,(u,ȵ)=>ۃ(u as IMyMechanicalConnectionBlock,ȵ));}protected void ۃ(
IMyMechanicalConnectionBlock ۂ,object Հ){var ۄ=Հ as bool?;var Ͻ=ϰ.ContainsKey(ۂ.EntityId)?ϰ[ۂ.EntityId]as bool?:null;if(ۄ==true&&Ͻ!=true){ï<Р>(ۂ);U.
ч(ۂ,"onAttach");U.ѷ(ۂ.TopGrid);}else if(ۄ==false&&Ͻ==true){ï<С>(ۂ);U.ч(ۂ,"onDetach");U.ѹ();}}}public class Н:d{}public
class О:d{}public class ۅ:Μ{U U;public ۅ(A B):base(B){}public override void c(){U=A.W<U>();ϻ<IMyShipMergeBlock>(ۆ=>ۆ.State,(u
,ȵ)=>ۇ(u as IMyShipMergeBlock,ȵ));}protected void ۇ(IMyShipMergeBlock ۆ,object Հ){var ӈ=Հ as MergeState?;var Ͻ=ϰ.
ContainsKey(ۆ.EntityId)?ϰ[ۆ.EntityId]as MergeState?:null;if(ӈ.HasValue){switch(ӈ){case MergeState.None:if(Ͻ==MergeState.Locked){ï<О
>(ۆ);U.ч(ۆ,"onUnmerge");U.Щ();}break;case MergeState.Locked:if(Ͻ!=MergeState.Locked){ï<Н>(ۆ);U.ч(ۆ,"onMerge");U.Щ();}
break;}}}public void ۈ(IMyShipMergeBlock ۆ){ۆ.Enabled=true;}public void ۉ(IMyShipMergeBlock ۆ){ۆ.Enabled=false;}public void ۊ
(IMyShipMergeBlock ۆ){if(ۆ.Enabled)ۉ(ۆ);else ۈ(ۆ);}}public class ڂ{static string ۋ="##";public static bool څ(
string Ɲ)=>Ɲ.StartsWith(ۋ);public static string ڗ(string Ӷ,string ځ=""){if(ځ=="")return Ӷ;else{var ی=new char[Ӷ.Length];for(
int ɦ=0;ɦ<Ӷ.Length;ɦ++)ی[ɦ]=(char)(Ӷ[ɦ]^ځ[ɦ%ځ.Length]);return ۋ+new string(ی);}}public static string ڃ(string ۍ,string ځ){ۍ
=ۍ.Substring(ۋ.Length);var ێ=new char[ۍ.Length];for(int ɦ=0;ɦ<ۍ.Length;ɦ++)ێ[ɦ]=(char)(ۍ[ɦ]^ځ[ɦ%ځ.Length]);return new
string(ێ);}}public class ۏ:Ē{ĳ ē;public override string Ĕ=>"clear";public ۏ(ĳ ĕ){ē=ĕ;}public override string Ġ(Ė ė){ē
.ې();return"";}}public class ۑ:Ē{ĳ ē;public override string Ĕ=>"print";public ۑ(ĳ ĕ){ē=ĕ;}public override string
Ġ(Ė ė){return ė.Ę[0];}}public class ĳ:Μ{S S;List<string>ے=new List<string>();List<string>ۓ=new List<
string>();public ĳ(A B):base(B){}public override void c(){S=A.W<S>();ę ę=A.W<ę>();ę.Þ(new ۏ(this));ę.Þ(new ۑ(this));}string ە=
"";public string ۥ()=>ە;public void Ň(string Ɲ){ە+=Ɲ+"\n";}public virtual string ۮ(){string ͳ="";string ۦ=(A.C??"").
PadRight(15).Substring(0,15);ͳ+=$" {ۦ}{י()}   ({A.W<S>().ҟ()})\n"+$" {A.Ĕ} *{A.ט}\n"+
$"------------------------------------------------------\n"+"";if(ە!="")ͳ+=$"{ۥ()}"+$"------------------------------------------------------\n"+$"";return ͳ;}public void ۻ(){
string ۯ=$"{ۮ()}\n"+$"{String.Join("\n",ۓ.AsEnumerable().Reverse())}";ۺ(ۯ);ە="";}public virtual string י(){string כ=A.W<Τ>().Ξ
.Count()>0?"M":"   ";string ל=A.W<Ĳ>().ל.Count()>0?"C":"    ";string ם=$"{A.W<T>().x.Count()}";string מ=A.ǋ?"A":"   ";
string ן=A.W<S>().ҝ>0?"W":"   ";int ۼ=A.W<ę>().ҷ.Count;string ۿ=ۼ>0?$"R{ۼ}":"   ";return String.Join("  ",ן,מ,ל,כ,ם,ۿ);}public
void Ŏ(string Ɲ,bool ܐ=true){ے.Add(Ɲ);string ܒ=ܐ&&Ɲ.Length>37?Ɲ.Substring(0,32)+"..."+Ɲ.Substring(Ɲ.Length-5):Ɲ;ۓ.Add(ܒ);if(
ۓ.Count>20)ۓ.RemoveRange(0,ۓ.Count-20);}public virtual void ۺ(string Ɲ){A.Ғ.Echo(Ɲ);}public bool ې(){ے.Clear();ۓ.Clear();
return ے.Count==0&&ۓ.Count==0;}}public class A{public MyGridProgram Ғ;public string C="Mother Program";public MyCommandLine ܓ;
public bool ǋ=false;public IMyCubeGrid Å;public IMyGridTerminalSystem ю;public IMyIntergridCommunicationSystem ٸ;public
IMyProgrammableBlock Θ;public IMyShipController Ý;public IMyGridProgramRuntimeInfo ܔ;public long Â;public string ט;public long ڜ;public
string Ĕ;public BoundingSphereD γ;public enum д{ܕ,е,ж,ܖ,ܗ,}public д г=д.ܕ;public bool ԡ=false;public Dictionary<string,Ϭ>ܘ=new
Dictionary<string,Ϭ>();List<Ϭ>ܙ=new List<Ϭ>();public Dictionary<string,Ј>ܚ=new Dictionary<string,Ј>();public
Dictionary<string,E>ܛ=new Dictionary<string,E>();public List<ˣ>ϱ=new List<ˣ>();public Dictionary<string,string>Ӓ=new Dictionary<
string,string>();public Dictionary<string,string>ԧ=new Dictionary<string,string>();public bool ˢ=false;public A(MyGridProgram
ܜ){ܝ(ܜ);}public void ܝ(MyGridProgram ܜ){Ғ=ܜ;ٸ=Ғ.IGC;Θ=Ғ.Me;Å=Θ.CubeGrid;ю=Ғ.GridTerminalSystem;ܔ=Ғ.Runtime;Â=ٸ.Me;ט=
$"{Â}".Substring($"{Â}".Length-5);ڜ=Å.EntityId;Ĕ=Θ.CubeGrid.CustomName;ܞ();}public void ܞ(){List<Ј>Α=new List<Ј>{new Ɵ(this),
new ǲ(this),new S(this),new ǡ(this),new ę(this),new ȅ(this),new U(this),new Τ(this),new T(this),new Ĳ(this),new ו(this),new
ĳ(this),new Զ(this),new ہ(this),new ۅ(this),};Α.ForEach(ĕ=>ܟ(ĕ));}void ܠ(д ȵ){г=ȵ;}public void c(){ܠ(д.е);W<S>().Ҕ();Ŏ(
$"Booting {C}...");ܡ();ܢ();W<S>().К(ܣ());}IEnumerable<double>ܣ(){foreach(var И in ܤ())yield return И;ܠ(д.ж);W<U>().ч(Θ,"onBoot");W<ǡ>().ï
<ԕ>();Ŏ($"{C} is online.");Ŏ("Clearing console in 2 seconds...");Ŏ("The Empire must grow.");W<S>().Қ(()=>W<ĳ>()?.ې(),2.0)
;}IEnumerable<double>ܤ(){int ܥ=ܙ.Count;for(int ɦ=0;ɦ<ܥ;ɦ++){var ĕ=ܙ[ɦ];Ŏ($"Booting modules: ({ɦ+1} / {ܥ})");var ܦ=ĕ.ϳ();
while(ܦ.MoveNext())yield return ܦ.Current;Ԡ(ĕ.Ѓ());}Ŏ("All modules booted.");}void ܡ(){double ܧ=50;γ=new BoundingSphereD(Å.
WorldVolume.Center,Å.WorldVolume.Radius+ܧ);}void ܢ(){Þ(new Ή(this));Þ(new ˡ(this));Þ(new Γ(this));}public void P(string N,
UpdateType O){if(ˢ){ˢ=false;c();return;}if(г==д.ܕ)c();else if(г==д.е){W<S>().P();W<ĳ>()?.ۻ();}else if(г==д.ж){if((O&(UpdateType.
Trigger|UpdateType.Terminal|UpdateType.Script))!=0){W<ę>().ƛ(N);W<S>().P();}else if(O==UpdateType.IGC)W<Ĳ>().پ();else{ܨ();ܩ();}
W<ĳ>().ۻ();W<ו>().ײ();}if(ԡ){W<ĳ>().Ň("Complexity:  "+Ғ.Runtime.CurrentInstructionCount.ToString()+"/50000");}}void ܨ()=>
ܘ.Values.ToList().ForEach(ĕ=>ĕ.P());void ܩ(){}public string M()=>W<ȅ>()?.ڽ();public void ܪ(E ĕ){ܛ[ĕ.ϴ()]=ĕ;ܘ[ĕ.ϴ()]=ĕ;ܙ.
Add(ĕ);}public void D(List<E>Α){Α.ForEach(ĕ=>ܪ(ĕ));}public string ϴ<ϵ>()where ϵ:Ϭ{foreach(var q in ܘ)if(q.Value is ϵ)return
q.Key;return typeof(ϵ).Name;}public ϵ W<ϵ>()where ϵ:class,Ϭ{var ܫ=ϴ<ϵ>();Ϭ ĕ;if(ܘ.TryGetValue(ܫ,out ĕ))return ĕ as ϵ;
return null;}Ј ܟ(Ј ĕ){ܚ[ĕ.ϴ()]=ĕ;ܘ[ĕ.ϴ()]=ĕ;ܙ.Add(ĕ);return ĕ;}public void Þ(ˣ ė){W<ę>().Þ(ė);}public void Ԡ(List<ˣ>Ӛ){Ӛ.
ForEach(ė=>Þ(ė));}public void ƺ(Action ܬ,double ܭ){W<S>().Қ(ܬ,ܭ);}public void Ŏ(string Ɲ,bool ܐ=true){ĳ ܮ=W<ĳ>();if(ܮ==null)Ғ.
Echo(Ɲ);else ܮ.Ŏ(Ɲ,ܐ);W<Ɵ>()?.Ơ(Ɲ);}public MatrixD ί(){return Ý?.WorldMatrix??Å.WorldMatrix;}public Vector3D ź()=>Å.
GetPosition();public Vector3D Ś(){if(Ý==null)return Vector3D.Zero;Vector3D ś=Ý.GetArtificialGravity();if(ś.LengthSquared()==0)ś=Ý.
GetNaturalGravity();return ś;}public double?ڝ()=>Ý?.GetShipSpeed();public string Ӌ(string Ӷ){if(ԧ.Count==0)return Ӷ;var ݍ=ԧ.
OrderByDescending(ܯ=>ܯ.Key.Length);foreach(var ݎ in ݍ)Ӷ=Ӷ.Replace("$"+ݎ.Key,ݎ.Value);return Ӷ;}public string ӣ(string ݏ,Dictionary<string
,string>ˮ){if(ݏ.IndexOf("{{")==-1)return Ӌ(ݏ);var в=new StringBuilder();int ɦ=0;while(ɦ<ݏ.Length){int ݐ=ݏ.IndexOf("{{",ɦ)
;if(ݐ==-1){в.Append(ݏ,ɦ,ݏ.Length-ɦ);break;}if(ݐ>ɦ)в.Append(ݏ,ɦ,ݐ-ɦ);int ݑ=ݏ.IndexOf("}}",ݐ+2);if(ݑ==-1){в.Append(ݏ,ݐ,ݏ.
Length-ݐ);break;}string ݒ=ݏ.Substring(ݐ+2,ݑ-ݐ-2);string ݓ;string ݔ="";int ݕ=ݒ.IndexOf(':');if(ݕ>=0){ݓ=ݒ.Substring(0,ݕ).Trim();
ݔ=ݒ.Substring(ݕ+1).Trim();}else{ݓ=ݒ.Trim();}string Ώ;if(ˮ!=null&&ˮ.ContainsKey(ݓ))Ώ=ˮ[ݓ];else Ώ=ݔ;в.Append(Ώ);ɦ=ݑ+2;}
return Ӌ(в.ToString());}}public class ÿ{public static BoundingBoxD ʵ(IEnumerable<Vector3D>ݖ){if(!ݖ.Any())return new
BoundingBoxD(Vector3D.Zero,Vector3D.Zero);double ݗ=double.MaxValue;double ݘ=double.MinValue;Vector3D ա=new Vector3D(ݗ,ݗ,ݗ);Vector3D
բ=new Vector3D(ݘ,ݘ,ݘ);foreach(var ɢ in ݖ){ա=Vector3D.Min(ա,ɢ);բ=Vector3D.Max(բ,ɢ);}return new BoundingBoxD(ա,բ);}public
static Vector3D ɀ(Vector3D ɴ,BoundingBoxD ݙ){return new Vector3D(MathHelper.Clamp(ɴ.X,ݙ.Min.X,ݙ.Max.X),MathHelper.Clamp(ɴ.Y,ݙ.
Min.Y,ݙ.Max.Y),MathHelper.Clamp(ɴ.Z,ݙ.Min.Z,ݙ.Max.Z));}public static Vector3D ɠ(BoundingSphereD ݚ,Vector3D ݛ){Vector3D ơ=
Vector3D.Normalize(ݚ.Center-ݛ);return ݚ.Center-(ơ*ݚ.Radius);}public static double Ā(Vector3D ћ,Vector3D Ϲ){if(Vector3D.IsZero(ћ)
||Vector3D.IsZero(Ϲ))return 0;return Math.Acos(MathHelper.Clamp(Vector3D.Dot(ћ,Ϲ)/(ћ.Length()*Ϲ.Length()),-1,1));}public
static void Ŭ(Vector3D ݜ,out Vector3D ݝ,out Vector3D ݞ){if(Math.Abs(ݜ.X)>Math.Abs(ݜ.Y))ݝ=new Vector3D(-ݜ.Z,0,ݜ.X);else ݝ=new
Vector3D(0,ݜ.Z,-ݜ.Y);ݝ=Vector3D.Normalize(ݝ);ݞ=Vector3D.Normalize(Vector3D.Cross(ݜ,ݝ));}public static float ݟ(float ղ,float ա,
float բ){return Math.Max(ա,Math.Min(բ,ղ));}public static Vector3D ɸ(string ǻ){string[]ϫ=ǻ.Split(':');int ݠ=(ϫ[0]=="GPS")?2:0;
return new Vector3D(double.Parse(ϫ[ݠ]),double.Parse(ϫ[ݠ+1]),double.Parse(ϫ[ݠ+2]));}}public interface ύ{string ϕ();}public
static class ˈ{public static string ˉ(string Ɲ,params object[]ݡ){return string.Format(Ɲ,ݡ);}}public class ȇ{public static
string Ȉ(Dictionary<string,object>ϖ){var ӱ=new StringBuilder();ӱ.Append("{");foreach(var ݣ in ϖ){ӱ.Append("\"").Append(ݢ(ݣ.Key
)).Append("\":");var ݤ=ݣ.Value as ύ;if(ݤ!=null)ӱ.Append(ݤ.ϕ());else if(ݣ.Value is Dictionary<string,object>)ӱ.Append(Ȉ((
Dictionary<string,object>)ݣ.Value));else if(ݣ.Value is List<object>)ӱ.Append(ݥ((List<object>)ݣ.Value));else if(ݣ.Value is string)ӱ
.Append("\"").Append(ݢ((string)ݣ.Value)).Append("\"");else throw new InvalidOperationException("Unsupported value type: "
+ݣ.Value?.GetType().Name);ӱ.Append(",");}if(ӱ.Length>1)ӱ.Length--;ӱ.Append("}");return ӱ.ToString();}public static
Dictionary<string,object>η(string Ԧ){var ݦ=new Dictionary<string,object>();if(string.IsNullOrEmpty(Ԧ)||Ԧ[0]!='{'||Ԧ[Ԧ.Length-1]!=
'}')return ݦ;Ԧ=Ԧ.Substring(1,Ԧ.Length-2);int ձ=Ԧ.Length;int ɦ=0;while(ɦ<ձ){int ݧ=Ԧ.IndexOf('"',ɦ);if(ݧ==-1)break;int ݨ=Ԧ.
IndexOf('"',ݧ+1);if(ݨ==-1)break;string ǹ=ݩ(Ԧ.Substring(ݧ+1,ݨ-ݧ-1));int ݪ=Ԧ.IndexOf(':',ݨ)+1;if(ݪ==0)break;object Ώ;if(Ԧ[ݪ]=='{'
){int ݬ=ݫ(Ԧ,ݪ,'{','}');if(ݬ==-1)break;string ݭ=Ԧ.Substring(ݪ,ݬ-ݪ+1);Ώ=η(ݭ);ɦ=ݬ+1;}else if(Ԧ[ݪ]=='['){int ݬ=ݫ(Ԧ,ݪ,'[',']')
;if(ݬ==-1)break;string ݮ=Ԧ.Substring(ݪ,ݬ-ݪ+1);Ώ=ݯ(ݮ);ɦ=ݬ+1;}else if(Ԧ[ݪ]=='"'){int ݬ=Ԧ.IndexOf('"',ݪ+1);while(ݬ!=-1&&Ԧ[ݬ-
1]=='\\'){ݬ=Ԧ.IndexOf('"',ݬ+1);}if(ݬ==-1)break;Ώ=ݩ(Ԧ.Substring(ݪ+1,ݬ-ݪ-1));ɦ=ݬ+1;}else{int ݬ=Ԧ.IndexOf(',',ݪ);if(ݬ==-1)ݬ=
Ԧ.Length;Ώ=Ԧ.Substring(ݪ,ݬ-ݪ).Trim();ɦ=ݬ;}ݦ.Add(ǹ,Ώ);ɦ=Ԧ.IndexOf(',',ɦ)+1;if(ɦ==0)break;}return ݦ;}public static string ݥ
(IEnumerable<object>װ){var ӱ=new StringBuilder();ӱ.Append("[");foreach(var ݰ in װ){if(ݰ is List<object>)ӱ.Append(ݥ((List<
object>)ݰ));else if(ݰ is Dictionary<string,object>)ӱ.Append(Ȉ((Dictionary<string,object>)ݰ));else if(ݰ is string)ӱ.Append("\""
).Append(ݢ((string)ݰ)).Append("\"");else if(ݰ is ύ)ӱ.Append(((ύ)ݰ).ϕ());else ӱ.Append("\"").Append(ݢ(ݰ!=null?ݰ.ToString()
:string.Empty)).Append("\"");ӱ.Append(",");}if(ӱ.Length>1)ӱ.Length--;ӱ.Append("]");return ӱ.ToString();}public static
List<object>ݯ(string Ԧ){var װ=new List<object>();if(string.IsNullOrEmpty(Ԧ)||Ԧ[0]!='['||Ԧ[Ԧ.Length-1]!=']')return װ;Ԧ=Ԧ.
Substring(1,Ԧ.Length-2);int ձ=Ԧ.Length;int ɦ=0;while(ɦ<ձ){char ݱ=Ԧ[ɦ];if(ݱ=='"'){int ݬ=Ԧ.IndexOf('"',ɦ+1);while(ݬ!=-1&&Ԧ[ݬ-1]==
'\\'){ݬ=Ԧ.IndexOf('"',ݬ+1);}if(ݬ==-1)break;װ.Add(ݩ(Ԧ.Substring(ɦ+1,ݬ-ɦ-1)));ɦ=ݬ+1;}else if(ݱ=='{'){int ݬ=ݫ(Ԧ,ɦ,'{','}');if(ݬ
==-1)break;string ݲ=Ԧ.Substring(ɦ,ݬ-ɦ+1);װ.Add(η(ݲ));ɦ=ݬ+1;}else if(ݱ=='['){int ݬ=ݫ(Ԧ,ɦ,'[',']');if(ݬ==-1)break;string ݮ=Ԧ
.Substring(ɦ,ݬ-ɦ+1);װ.Add(ݯ(ݮ));ɦ=ݬ+1;}else ɦ++;if(ɦ<ձ&&Ԧ[ɦ]==',')ɦ++;}return װ;}static int ݫ(string Ԧ,int Ȕ,char ݳ,char
ݴ){int ݵ=0;for(int ɦ=Ȕ;ɦ<Ԧ.Length;ɦ++){if(Ԧ[ɦ]==ݳ)ݵ++;else if(Ԧ[ɦ]==ݴ)ݵ--;if(ݵ==0)return ɦ;}return-1;}static string ݢ(
string ݶ)=>ݶ.Replace("\\","\\\\").Replace("\"","\\\"");static string ݩ(string ݶ)=>ݶ.Replace("\\\"","\"").Replace("\\\\","\\");

