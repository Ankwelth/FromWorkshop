// Patrick's original code
// Gyro Assist v1.1.38.44
// 
// Check the CustomData field of the Programmable block after you 
// compile this code.
// 
// Uses gyro overrides to augment pitch/roll/yaw pilot input.
static class J{private const string A="Gyro Assist",B="Version {0}.{1}.{2}.{3}",C="{0}\n{1}";private const int D=1,E=1,F
=38,G=44;public readonly static string H=string.Format(B,D,E,F,G),I=string.Format(C,A,H);}private const string K=
"refresh",L="Input Scaling",M="Roll",N="Yaw",O="Pitch",P="Control",Q="Only MAIN control",R="Operation",S=": ",T="Display",U=
"No usable ship controller found.",V="Gyro override: <active>",W="Gyro override: >inactive<",X="Ship Controllers: ",Y="Gyros: ",Z="Active Controller: ",a=
"monospace";private readonly Color b=new Color(200,230,255),c=new Color(0,1,2);private const float d=0.01f,e=1f,f=1f,g=(float)Math.
PI*2;List<IMyShipController>h;private readonly List<IMyGyro>i;private readonly List<j>k;private readonly IMyTextSurface l;
private readonly StringBuilder m;private readonly MyIni n;IMyShipController o;p q;r s;bool t;public
 Program
(){h=new List<IMyShipController>();i=new List<IMyGyro>();k=new List<j>();l=Me.GetSurface(0);m=new StringBuilder();n=new
MyIni();q=new p();s=new r(0f,0f,0f);t=false;Runtime.UpdateFrequency=UpdateFrequency.Update10;u();v();w(W);}public void
 Main
(string x,UpdateType y){if(x==K){u();v();s=new r(0f,0f,0f);t=false;w(W);return;}if(!z(o,q.ª)){o=µ(q.ª);}if(o==null){if(t)
{º();s=new r(0f,0f,0f);t=false;}w(U);return;}r Á=À(o);if(Á.Â){if(!t||!s.Ã(Á)){Ä(Á,o);s=Á;}t=true;w(V);return;}if(t){º();s
=Á;t=false;w(W);}}void u(){MyIniParseResult Å;if(!n.TryParse(Me.CustomData,out Å)){n.Clear();}Æ();q=new p();q.Ç=n.Get(L,M
).ToSingle(1f);q.È=n.Get(L,N).ToSingle(1f);q.É=n.Get(L,O).ToSingle(1f);q.ª=n.Get(P,Q).ToBoolean(true);q.Ê=n.Get(R,T).
ToBoolean(false);Me.CustomData=n.ToString();}void Æ(){Ë(L,M,1);Ë(L,N,1);Ë(L,O,1);Ì(P,Q,true);Ì(R,T,false);}void Ë(string Í,string
Î,float Ï){if(n.ContainsKey(Í,Î)){return;}n.Set(Í,Î,Ï);}void Ì(string Í,string Î,bool Ï){if(n.ContainsKey(Í,Î)){return;}n
.Set(Í,Î,Ï);}void v(){h.Clear();i.Clear();k.Clear();GridTerminalSystem.GetBlocksOfType(h,Ð);GridTerminalSystem.
GetBlocksOfType(i,Ð);for(int Ñ=0,Ò=i.Count;Ñ<Ò;Ñ++){k.Add(new j(i[Ñ]));}o=µ(q.ª);if(q.Ê){l.ContentType=ContentType.TEXT_AND_IMAGE;l.
Alignment=TextAlignment.LEFT;l.Font=a;l.FontColor=b;l.BackgroundColor=c;}else{l.ContentType=ContentType.NONE;}}bool Ð(
IMyTerminalBlock Ó){if(Ó==null){return false;}return Ó.CubeGrid==Me.CubeGrid;}bool z(IMyShipController Ô,bool Õ){bool Ö=Ô!=null;if(!Ö){
return false;}bool Ø=Ô.IsWorking;bool Ù=Ô.IsUnderControl;bool Ú=Ô.IsMainCockpit||!Õ;return Ø&&Ù&&Ú;}IMyShipController µ(bool Õ
){IMyShipController Û=null;for(int Ü=0,Ò=h.Count;Ü<Ò;Ü++){IMyShipController Ý=h[Ü];if(!Þ(Ý,Õ)){continue;}bool Ú=Ý.
IsMainCockpit||!Õ;if(Ý.IsUnderControl&&Ú){return Ý;}if(Û==null){Û=Ý;}}return Û;}bool Þ(IMyShipController Ý,bool Õ){if(Ý==null){return
false;}if(!Ý.IsFunctional){return false;}if(!Ý.IsWorking){return false;}if(!Ý.IsMainCockpit&&Õ){return false;}return true;}r
À(IMyShipController Ý){Vector2 ß=Ý.RotationIndicator;float á=à(ß.X);float â=à(ß.Y);float ã=à(-Ý.RollIndicator);á*=q.È;â*=
q.É;ã*=q.Ç;return new r(â,á,ã);}float à(float ä){if(Math.Abs(ä)<=d){return 0f;}if(ä>0f){return f;}return-f;}void Ä(r Á,
IMyShipController å){for(int Ñ=0,Ò=k.Count;Ñ<Ò;Ñ++){k[Ñ].æ(Á,å,q);}}void º(){for(int Ñ=0,Ò=k.Count;Ñ<Ò;Ñ++){k[Ñ].ç();}}void w(string è){m
.Clear();m.AppendLine(J.I);m.AppendLine(è);m.Append(X);m.AppendLine(h.Count.ToString());m.Append(Y);m.AppendLine(k.Count.
ToString());m.Append(M);m.Append(S);m.AppendLine(q.Ç.ToString());m.Append(N);m.Append(S);m.AppendLine(q.È.ToString());m.Append(O
);m.Append(S);m.AppendLine(q.É.ToString());if(o!=null){m.AppendLine(Z);m.AppendLine(o.CustomName);}Echo(m.ToString());if(
q.Ê){l.FontSize=1.0f;Vector2 é=l.MeasureStringInPixels(m,l.Font,l.FontSize);Vector2 ê=l.SurfaceSize;float ë=ê.X/é.X;float
ì=ê.Y/é.Y;l.FontSize=Math.Min(ë,ì);l.WriteText(m.ToString());}}sealed class p{public float Ç,È,É;public bool ª,Ê;public p
(){Ç=1;È=1;É=1;ª=false;Ê=false;}}sealed class r{public readonly float í,î,ï;public readonly bool Â;public r(float ð,float
ñ,float ò){í=ð;î=ñ;ï=ò;Â=false;if(í!=0f){Â=true;return;}if(î!=0f){Â=true;return;}if(ï!=0f){Â=true;}}public bool Ã(r ó){if
(ó==null){return false;}if(í!=ó.í){return false;}if(î!=ó.î){return false;}if(ï!=ó.ï){return false;}return true;}}sealed
class j{private readonly IMyGyro ô;private readonly float õ;public j(IMyGyro ö){ô=ö;õ=ö.GyroPower;}public void æ(r Á,
IMyShipController å,p ø){if(!ù()){return;}Matrix ú;å.Orientation.GetMatrix(out ú);Matrix û;ô.Orientation.GetMatrix(out û);Matrix ü;Matrix
.Invert(ref û,out ü);Vector3 ý=new Vector3(Á.î,Á.í,-Á.ï);Vector3 þ=Vector3.TransformNormal(ý,ú);Vector3 ÿ=Vector3.
TransformNormal(þ,ü);int ā=Ā(ÿ.X);int Ă=Ā(ÿ.Y);int ă=Ā(ÿ.Z);ô.GyroPower=e;ô.GyroOverride=true;ô.Pitch=g*ø.É*ā;ô.Yaw=g*ø.È*Ă;ô.Roll=g*ø.
Ç*ă;}public void ç(){if(ô==null){return;}ô.Pitch=0f;ô.Yaw=0f;ô.Roll=0f;ô.GyroOverride=false;ô.GyroPower=õ;}bool ù(){if(ô
==null){return false;}if(!ô.IsFunctional){return false;}if(!ô.Enabled){return false;}return true;}int Ā(float ä){if(Math.
Abs(ä)<=d){return 0;}if(ä>0f){return 1;}return-1;}}
