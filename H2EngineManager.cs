/*
 * R e a d m e
 * -----------
 * 
 * Manage H2 Engine Operation
 * Patrick Heney
 * 
 * After installing in a Programmable block, check the Custom Data area
 * The settings control the level at which the engines automatically turn on or off.
 * Setting values must be between 0 and 1.
 * Engines will start when hydrogen fill level is ABOVE the start value.
 * Engines will shutdown when hydrogen fill level is BELOW the stop value.
 */
static class č{private const string Ď="H2 Engine Manager";private const int ď=1;private const int Đ=3;private const int
đ=4;private const int Ē=5;private const string ē="Version {0}.{1}.{2}.{3}";public static string Ĕ=string.Format(ē,ď,Đ,đ,Ē
);private const string ĕ="{0}\n{1}";public static string Ė=string.Format(ĕ,Ď,Ĕ);}List<IMyPowerProducer>ė=new List<
IMyPowerProducer>();List<IMyGasTank>Ę=new List<IMyGasTank>();List<IMyBatteryBlock>j=new List<IMyBatteryBlock>();float ę;float Ě;float ě;
bool Ĝ;const float ĝ=0.65f;bool Ğ;bool ğ;bool Ġ;string ġ;Û Ģ;private enum ģ{Ĥ,ĥ,Ħ}ģ ħ;Program(){Me.GetSurface(0).WriteText(č
.Ė,append:true);Me.GetSurface(0).WriteText(ã.ToString(),append:true);Me.GetSurface(0).WriteText(ã.ToString(),append:true)
;Echo(string.Empty);Echo(č.Ė);Echo(string.Empty);Ĩ();Runtime.UpdateFrequency=UpdateFrequency.Update100;}void Ĩ(){ā();Ń();
ł();ń();if(ė.Any(M=>M.Enabled)){ħ=ģ.ĥ;}ï();}void ï(){if(Ģ!=null){Ģ=null;}if(!string.IsNullOrWhiteSpace(ġ)){Ģ=new Û(this,ġ
);}}const char ã='\n';const char ð=':';const char ä=' ';const string ñ="DISPLAY TAG";const string ò="H2EngineManager";
const string ó="MINIMUM REQUIRED FUEL";const float ô=0.15f;const string õ="START CHARGE POINT";const float ö=0.25f;const
string ø="STOP CHARGE POINT";const float ù=0.95f;const string ú="LOAD BALANCE";const bool û=true;const string ü=
"MULTIGRID TANKS";const string ý="MULTIGRID ENGINES";const string ċ="MULTIGRID BATTERIES";const bool þ=false;const string ÿ="Hydrogen";
const string Ā="Exclude";void ā(){bool Ă=true;bool ă=true;bool Ą=true;bool ą=true;bool Ć=true;bool ć=true;bool Ĉ=true;bool ĉ=
true;var Ċ=Me.CustomData;var Ê=Ċ.Split(ã);foreach(var Ë in Ê){if(Ë.IndexOf(ñ,StringComparison.OrdinalIgnoreCase)==0){int Ľ=Ë
.IndexOf(ð)+1;ġ=Ë.Substring(Ľ).Trim();Ă=false;continue;}if(Ë.IndexOf(õ,StringComparison.OrdinalIgnoreCase)==0){int Ľ=Ë.
IndexOf(ð)+1;string ľ=Ë.Substring(Ľ).Trim();if(!float.TryParse(ľ,out Ě)){Ě=ö;}Ą=false;continue;}if(Ë.IndexOf(ø,StringComparison
.OrdinalIgnoreCase)==0){int Ľ=Ë.IndexOf(ð)+1;string ľ=Ë.Substring(Ľ).Trim();if(!float.TryParse(ľ,out ě)){ě=ù;}ą=false;
continue;}if(Ë.IndexOf(ó,StringComparison.OrdinalIgnoreCase)==0){int Ľ=Ë.IndexOf(ð)+1;string ľ=Ë.Substring(Ľ).Trim();if(!float.
TryParse(ľ,out ę)){ę=ô;}ă=false;continue;}if(Ë.IndexOf(ú,StringComparison.OrdinalIgnoreCase)==0){int Ľ=Ë.IndexOf(ð)+1;string ľ=Ë
.Substring(Ľ).Trim();if(!bool.TryParse(ľ,out Ĝ)){Ĝ=û;}Ć=false;continue;}if(Ë.IndexOf(ü,StringComparison.OrdinalIgnoreCase
)==0){int Ľ=Ë.IndexOf(ð)+1;string ľ=Ë.Substring(Ľ).Trim();if(!bool.TryParse(ľ,out Ğ)){Ğ=þ;}ć=false;continue;}if(Ë.IndexOf
(ý,StringComparison.OrdinalIgnoreCase)==0){int Ľ=Ë.IndexOf(ð)+1;string ľ=Ë.Substring(Ľ).Trim();if(!bool.TryParse(ľ,out ğ)
){ğ=þ;}Ĉ=false;continue;}if(Ë.IndexOf(ċ,StringComparison.OrdinalIgnoreCase)==0){int Ľ=Ë.IndexOf(ð)+1;string ľ=Ë.Substring
(Ľ).Trim();if(!bool.TryParse(ľ,out Ġ)){Ġ=þ;}ĉ=false;continue;}}if(Ă){ŀ(ñ,ò);}if(Ą){ŀ(õ,ö);}if(ą){ŀ(ø,ù);}if(ă){ŀ(ó,ô);}if
(Ć){ŀ(ú,û);}if(ć){ŀ(ü,þ);}if(Ĉ){ŀ(ý,þ);}if(ĉ){ŀ(ċ,þ);}}const string Ŀ="0.##";void ŀ(string Ł,float ľ){if(!string.
IsNullOrEmpty(Me.CustomData)&&Me.CustomData.Length>0&&Me.CustomData[Me.CustomData.Length-1]!=ã){Me.CustomData+=ã;}Me.CustomData+=Ł+ð+
ä+ľ.ToString(Ŀ)+ã;}void ŀ(string Ł,bool ľ){if(!string.IsNullOrEmpty(Me.CustomData)&&Me.CustomData.Length>0&&Me.CustomData
[Me.CustomData.Length-1]!=ã){Me.CustomData+=ã;}Me.CustomData+=Ł+ð+ä+ľ.ToString()+ã;}void ŀ(string Ł,string É){if(!string.
IsNullOrEmpty(Me.CustomData)&&Me.CustomData.Length>0&&Me.CustomData[Me.CustomData.Length-1]!=ã){Me.CustomData+=ã;}Me.CustomData+=Ł+ð+
ä+É+ã;}void ł(){ė.Clear();ė.AddRange(Ī);ė.Sort((ļ,ĩ)=>ļ.CustomName.CompareTo(ĩ.CustomName));}void Ń(){Ę.Clear();Ę.
AddRange(Ĭ(ÿ));Ę.Sort((ļ,ĩ)=>ļ.CustomName.CompareTo(ĩ.CustomName));}void ń(){j.Clear();j.AddRange(į());j.Sort((ļ,ĩ)=>ļ.
CustomName.CompareTo(ĩ.CustomName));}const string Ĳ="HydrogenEngine";private List<IMyPowerProducer>Ī{get{var ī=new List<
IMyPowerProducer>();GridTerminalSystem.GetBlocksOfType(ī,r=>{if(!ğ&&!r.IsSameConstructAs(Me)){return false;}if(r.BlockDefinition.
SubtypeName.EndsWith(Ĳ,StringComparison.OrdinalIgnoreCase)){if(r.CustomName.IndexOf(Ā,StringComparison.OrdinalIgnoreCase)>-1){
return false;}return true;}return false;});return ī;}}List<IMyGasTank>Ĭ(string ĭ){var Į=new List<IMyGasTank>();
GridTerminalSystem.GetBlocksOfType(Į,n=>{if(!Ğ&&!n.IsSameConstructAs(Me)){return false;}if(n.CustomName.IndexOf(Ā,StringComparison.
OrdinalIgnoreCase)>-1){return false;}return n.CustomName.IndexOf(ĭ,StringComparison.OrdinalIgnoreCase)>-1;});return Į;}List<
IMyBatteryBlock>į(){var j=new List<IMyBatteryBlock>();GridTerminalSystem.GetBlocksOfType(j,g=>{if(!Ġ&&!g.IsSameConstructAs(Me)){return
false;}if(g.CustomName.IndexOf(Ā,StringComparison.OrdinalIgnoreCase)>-1){return false;}return true;});return j;}private bool
İ{get{if(ė.All(M=>M.Enabled)){return false;}if(Ĵ(Ę)<ę){ħ=ģ.Ĥ;return false;}if(ķ(j)<Ě){ħ=ģ.ĥ;return true;}if(Ĝ&&Ļ(j)>ĝ){ħ=
ģ.Ħ;return true;}return false;}}private bool ı{get{if(ė.All(M=>!M.Enabled)){return false;}if(Ĵ(Ę)<ę){ħ=ģ.Ĥ;return true;}
if(ĺ&&ķ(j)>ě){ħ=ģ.Ĥ;return true;}if(ĳ&&Ļ(j)<0.65f*ĝ){ħ=ģ.Ĥ;return true;}return false;}}private bool ĺ{get{return ħ==ģ.ĥ;}}
private bool ĳ{get{return ħ==ģ.Ħ;}}float Ĵ(List<IMyGasTank>Į){if(Į.Count==0){return 0;}float ĵ=(float)Į.Sum(n=>n.Capacity*n.
FilledRatio);float Ķ=Į.Sum(n=>n.Capacity);if(Ķ>0){return ĵ/Ķ;}return 1;}float ķ(List<IMyBatteryBlock>j){if(j.Count==0){return 0;}
float ĸ=j.Sum(g=>{if(g.ChargeMode==ChargeMode.Discharge){return 0;}return g.CurrentStoredPower;});float Ĺ=j.Sum(g=>{if(g.
ChargeMode==ChargeMode.Discharge){return 0;}return g.MaxStoredPower;});if(Ĺ>0){return ĸ/Ĺ;}return 1;}float Ļ(List<IMyBatteryBlock>
j){if(j.Count==0){return 0;}float e=j.Sum(g=>{if(g.ChargeMode==ChargeMode.Recharge){return 0;}return g.CurrentOutput;});
float f=j.Sum(g=>{if(g.ChargeMode==ChargeMode.Recharge){return 0;}return g.MaxOutput;});if(f>0){return e/f;}return 0;}float h
(List<IMyBatteryBlock>j){float k=1+j.Sum(g=>g.CurrentOutput);float l=1+j.Sum(g=>g.CurrentInput);if(l>0){return k/l;}
return 0;}private bool m{get{return Ę.Any(n=>n==null);}}private bool o{get{return ė.Any(M=>M==null);}}private bool p{get{
return j.Any(g=>g==null);}}string À(IMyPowerProducer r,bool T=false){if(!r.IsFunctional){return T?H(q,C):q;}if(r.Enabled){if(Ĵ
(Ę)==0){return T?H(Á,E):Á;}if(r.IsWorking){return T?H(µ,D):µ;}else{return T?H(q,C):q;}}return T?H(º,E):º;}const string s=
"Hydrogen Tanks: {0:P1}";const string t="Battery Charge: {0:P1}";const string u="Battery Load: {0:P1}";const string v="Multigrid Tanks: {0}";
const string w="Multigrid Engines: {0}";const string x="Multigrid Batteries: {0}";const string y="ENABLED";const string z=
"disabled";const string ª="Last Update: {0}";const string µ="Running";const string º="Shutdown";const string Á="Stalled";const
string q="Fault";const string d="REFRESH";StringBuilder N=new StringBuilder();StringBuilder B=new StringBuilder();const string
C="ff800000";const string D="ff00ff00";const string E="ffffff00";const string F="ffffffd0";const string G=
"[color=#{0}]{1}[/color]";string H(string I,string J){return string.Format(G,J,I);}void Main(string K,UpdateType L){if(!string.IsNullOrWhiteSpace
(K)&&K.IndexOf(d,StringComparison.OrdinalIgnoreCase)==0){ā();return;}if(m){Ń();}if(o){ł();}if(p){ń();}B.Clear();B.
AppendStringBuilder(S(T:true));Echo(B.ToString());N.Clear();N.AppendStringBuilder(S(T:false));a(N);if(Ģ!=null){Ģ.ê(N.ToString(),ë:true);}if
(ı){ė.ForEach(M=>M.Enabled=false);return;}if(İ){ė.ForEach(M=>M.Enabled=true);return;}}const string b="H2 ENGINES";const
string O="H2 TANKS";const string P="{0} (x{1}) {2}";const string Q=" - {0} [{1}]";const string R=" - {0} [[ {1} ]]";
StringBuilder S(bool T=false){StringBuilder U=new StringBuilder();U.AppendLine(č.Ĕ);U.AppendLine(string.Format(ª,DateTime.Now.
ToLongTimeString()));U.AppendLine(string.Format(t,ķ(j)));if(Ĝ){U.AppendLine(string.Format(u,Ļ(j)));}if(Ğ){U.AppendLine(string.Format(v,y
));}if(ğ){U.AppendLine(string.Format(w,y));}if(Ġ){U.AppendLine(string.Format(x,y));}U.AppendLine();U.AppendLine(string.
Format(P,O,Ę.Count,string.Empty));U.AppendLine(string.Format(s,Ĵ(Ę)));U.AppendLine();U.AppendLine(string.Format(P,b,ė.Count,Y(
ħ)));foreach(var V in ė){if(V==null){continue;}U.AppendLine(string.Format(T?R:Q,V.CustomName,À(V,T)));}return U;}const
string W="Recharging";const string X="Load balancing";string Y(ģ Z){switch(Z){case ģ.ĥ:return W;case ģ.Ħ:return X;case ģ.Ĥ:
default:return string.Empty;}}void a(StringBuilder A){var c=Me as IMyTextSurfaceProvider;IMyTextSurface Â=c.GetSurface(0);Â.
ContentType=ContentType.TEXT_AND_IMAGE;Â.WriteText(A.ToString(),false);}class Û{private string Ü{set{if(string.IsNullOrWhiteSpace(
value)){æ=string.Empty;}else{æ=string.Format(Ý,value);}}get{return æ;}}private const string Ý="[{0}]";private const string Þ=
"Script tag: {0}";private const string ß="Found {0} LCDs";private const string à="Found {0} Screens";private const string á=
"Identified displays: {0}";private const string â="{0} (surface {1})";private const char ã='\n';private const char ä=' ';private const char å='@';
private string æ;private List<IMyTextSurface>Ä=new List<IMyTextSurface>();private StringBuilder A=new StringBuilder();private
MyGridProgram ç;public Û(MyGridProgram ç,string æ){this.ç=ç;è(æ);}public void è(){Ä.Clear();if(string.IsNullOrWhiteSpace(Ü)){return;}
Ì(Ä);Ç(Ä);}public void è(string æ){if(string.IsNullOrWhiteSpace(æ)){return;}Ü=æ;è();}public void é(){try{Ä.ForEach(Î=>Î.
WriteText(string.Empty,false));}catch(NullReferenceException){è();}}public void ê(string Ð,bool ë=false){try{if(ë){é();}Ä.ForEach
(Î=>Î.WriteText(Ð,false));}catch(NullReferenceException){è();}}public void ì(bool ë=false){ê(í().ToString(),ë);}public
StringBuilder í(){var î=Ä.Count(Î=>Î as IMyTextPanel!=null);var Ã=Ä.Count()-î;A.AppendFormat(Þ,Ü);A.AppendLine();A.AppendFormat(ß,î);
A.AppendLine();A.AppendFormat(à,Ã);A.AppendLine();return A;}private void Ì(List<IMyTextSurface>Ä){ç.GridTerminalSystem.
GetBlocksOfType(Ä,(r)=>{var Å=(r as IMyTerminalBlock).CustomName.ToLower();bool Æ=Å.Contains(æ.ToLower());if(Æ){r.ContentType=
ContentType.TEXT_AND_IMAGE;ç.Echo(string.Format(á,Å));}return Æ;});}private void Ç(List<IMyTextSurface>Ä){List<IMyTerminalBlock>È=
new List<IMyTerminalBlock>();ç.GridTerminalSystem.GetBlocksOfType(È,(r)=>{var c=r as IMyTextSurfaceProvider;if(c==null){
return false;}bool Æ=r.CustomName.ToLower().Contains(æ.ToLower());if(!Æ){return false;}var É=r.CustomData;var Ê=É.Split(ã);
foreach(var Ë in Ê){if(!Ë.StartsWith(å.ToString())){continue;}string Í=Ë.Substring(1).Split(ä)[0];int Ù;if(!int.TryParse(Í,out
Ù)){continue;}IMyTextSurface Â=c.GetSurface(Ù);Â.ContentType=ContentType.TEXT_AND_IMAGE;string Î=string.Format(â,r.
CustomName,Ù);ç.Echo(string.Format(á,Î));Ä.Add(Â);break;}return false;});}private void Ï(IMyTextSurface Î,string Ð){A.Clear();A.
Append(Ð);string Ñ=Î.Font;float Ò=Î.FontSize;float Ó=Î.MeasureStringInPixels(A,Ñ,Ò).Y;float Ô=Î.SurfaceSize.Y;int Õ=(int)Math.
Floor(Ô/Ó);string Ö=Î.GetText()+ã+Ð;var Ê=Ö.Split(ã);int Ø=Ê.Length;int Č=Math.Max(0,Ø-Õ);A.Clear();for(int Ú=Č;Ú<Ø;Ú++){A.
Append(Ê[Ú]);if(Ú<Ø-1){A.Append(ã);}}Î.WriteText(A.ToString(),false);}}