/*
 * Patrick's code
 * 
 * DESCRIPTION
 * Adjusts thrust overrides for fuel efficient ascent and descent
 *     to / from orbit
 *     to / from landing site
 *     to / from cruise altitude
 * Automatically uses whichever thrusters are pointed "down," so ships
 *     can take off horizontally and then pitch up to point at the sky
 *     and control will automatically switch from the bottom thrusters
 *     to the back thrusters. This will use subgrid thrusters.
 * 
 * 
 * USAGE
 * RUN with argument "cruise #" (e.g., "cruise 1500")
 *     Ascend or descend to the provided altitude above ground level (AGL).
 *     When the number is omitted, the ship moves to the most recent set
 *     altitude.
 * RUN with argument "land"
 *     Safely and efficiently lands the ship. This command adjusts
 *     the descent speed based on gravity and distance to the ground.
 *     Ship automatically slows for a safe landing. Upon contact with the
 *     ground, thrusters are set to 0% override, and dampeners are
 *     turned off.
 *     WARNING: Remain alert to keep the ship level or ship will drift.
 * RUN with argument "escape"
 *     Ascend from gravity well until planetary gravity is 0.
 *     Once gravity is 0, thrusters are set to 0% override, and
 *     dampeners are turned on.
 * RUN with argument "idle"
 *     Turn off current mode, return thrusters to 0% override, and
 *     turn on dampeners.
 * RUN with argument "speed #" (e.g., "speed 75")
 *     Set the maximum ascent speed. (When descending, a calculation is
 *     used to temporarily reduce this speed for safety.) This command
 *     can be combined with other commands, e.g., "cruise 2500 speed 80"
 * RUN with argument "refresh"
 *     When custom data exists, read settings.
 *     When custom data does not exist, writes default settings.
 *     (Re) detects main control systems and thrusters.
 * 
 * 
 * SETUP
 * Add script to Programmable Block
 * At least one Cockpit / Control Seat must be set to "Main cockpit."
 * In order to use Remote Control blocks, at least one must be set as 
 *     "Main Remote Control." (Bug: RC currently does not work)
 * Update the CONTROL HEIGHT in the Custom Data (see below).
 * 
 * IMPORTANT
 * Set the CONTROL HEIGHT value to the "block elevation" of
 * the control seat from the bottom of the ship.
 *     For example, if the control is sitting on a deck that is 1 block 
 *     thick and there is a landing gear under the deck, then this value
 *     must be 4 (1 for the control block, plus 1 block of deck
 *     thickness, plus 2 for the block-height of the landing gear).
 * If landing system cuts off too early, reduce this number.
 * If the landing system does not cut off after contact with the ground,
 *     increase this number.
 * 
 * OPTIONAL
 * Add the tag [AutoThrottle] to any LCD or Text Panel to display status.
 * Add the tag to any cockpit or block with multiple screens, AND
 *     add @0 [AutoThrottle] to the block's custom data to display 
 *     status on the indicated screen.
 */
IMyShipController ą;Â Ĺ;ö ĸ;StringBuilder K;const string ķ="SPEED";const string Ķ="LAND";const string ĵ="CRUISE";const
string Ĵ="ESCAPE";const string ĳ="REFRESH";const string Ĳ="IDLE";const string ı="DIAGNOSTIC";const int İ=30;const float Ĉ=
9.807f;const float į=20;const float Į=3.5f;const float ĭ=2.5f;const float Ĭ=0.5f;bool ĺ=false;float ī;private enum Ļ{Ō,Ŋ,ŉ,ň};
Ļ Ň;float ß;float ņ;float Ņ;float ń;float ŋ;DateTime Ń;float Ł;float ŀ;float Ŀ;TimeSpan ľ;string H;const string À="[{0}]"
;const string Ľ="AutoThrottle";private string Á{get{return string.Format(À,H);}set{H=value;}}DateTime ļ;Program(){K=new
StringBuilder();ė();}const string ę="WARNING: No ship controllers were found that are set to \"main\"";const string ċ=
"Execution halted.";void ė(){ģ();Echo(string.Empty);ą=s;if(ą==null){Echo(ę);Echo(ċ);Runtime.UpdateFrequency=UpdateFrequency.None;return;}ŀ=
É(Ŀ);Ĺ=new Â(this,H);Echo(string.Empty);ĸ=new ö(this,ą,true);Echo(ĸ.Ú());Runtime.UpdateFrequency=UpdateFrequency.Update10
;Ÿ(true);}void Main(string Đ,UpdateType Ĕ){ĕ(Đ,Ĕ);if(!Ŷ()){return;}var Ė=Ú();Echo(Ė);Ĺ.F(Ė,true);if(e){Ų();if((u&&Ê)||(g
&&w)||(f&&Ì)){Ň=Ļ.Ō;}}else{Ÿ();}}const char M=' ';void ĕ(string Đ,UpdateType Ĕ){if(string.IsNullOrWhiteSpace(Đ)){return;}đ
(Đ);bool ē=false;string Ē=Đ.Split(M)[0].Trim().ToUpper();switch(Ē){case Ķ:ē=Č();break;case ĵ:ē=Ę(Đ);break;case Ĵ:ē=ĩ();
break;case Ĳ:Ň=Ļ.Ō;Runtime.UpdateFrequency=UpdateFrequency.Update10;ļ=DateTime.Now;break;case ĳ:ė();break;case ı:ĺ=!ĺ;break;}
if(ē){Runtime.UpdateFrequency=UpdateFrequency.Update10;ß=ĸ.è(0);ļ=DateTime.Now;}}void đ(string Đ){var ď=Đ.IndexOf(ķ,
StringComparison.OrdinalIgnoreCase);if(ď>-1){string Ď=Đ.Substring(ď+ķ.Length).Trim().Split(M)[0];float č;if(float.TryParse(Ď,out č)&&č>0
){ņ=č;}}}bool Č(){if(Ň==Ļ.Ŋ){Ÿ();return false;}else{Ň=Ļ.Ŋ;ĸ.Ą();return true;}}bool Ę(string Đ){if(Ň==Ļ.ŉ){Ÿ();return
false;}else{Ň=Ļ.ŉ;ĸ.Ą();var Q=Đ.IndexOf(ĵ,StringComparison.OrdinalIgnoreCase);string Ī=Đ.Substring(Q+ĵ.Length).Trim().Split(M
)[0];float Ë;if(float.TryParse(Ī,out Ë)){Ł=Ɔ+Ë;}return true;}}bool ĩ(){if(Ň==Ļ.ň){Ÿ();return false;}else{Ň=Ļ.ň;ĸ.Ą();
return true;}}const string Ĩ="On orbit: {0}";const string ħ="Is landed: {0}";const string Ħ="At cruise: {0}";const string ĥ=
"Shutdown";const string Ĥ="Run time: {0} sec";void ģ(){var U=Me.CustomData;if(string.IsNullOrWhiteSpace(U)){Ź();}else{ƀ();}}const
string Ģ="Reading custom data ({0} lines)";const string ġ="> {0} :: {1}";const string Ġ="{0} {1}";const string ğ="SCRIPT TAG";
const string Ğ="MAX SPEED";const string ĝ="SAFETY FACTOR";const string Ĝ="CONTROL HEIGHT";const float ě=65;const float ł=0.5F
;const float ō=4;const string Ɖ="@0";void ƀ(){var U=Me.CustomData;var T=U.Split(d);Echo(string.Format(Ģ,T.Length));
foreach(var S in T){if(S.StartsWith(ğ,StringComparison.OrdinalIgnoreCase)){var ž=S.Substring(ğ.Length).Trim().Split(M).
FirstOrDefault();Echo(string.Format(ġ,ğ,ž));if(!string.IsNullOrEmpty(ž)){H=ž;}continue;}if(S.StartsWith(Ğ,StringComparison.
OrdinalIgnoreCase)){var Ž=S.Substring(Ğ.Length).Trim().Split(M).FirstOrDefault();Echo(string.Format(ġ,Ğ,Ž));float ź;if(float.TryParse(Ž,
out ź)){ņ=ź;}continue;}if(S.StartsWith(ĝ,StringComparison.OrdinalIgnoreCase)){var ż=S.Substring(ĝ.Length).Trim().Split(M).
FirstOrDefault();Echo(string.Format(ġ,ĝ,ż));float ź;if(float.TryParse(ż,out ź)&&ź>0){ń=ź;}else{ń=ł;}continue;}if(S.StartsWith(Ĝ,
StringComparison.OrdinalIgnoreCase)){var Ż=S.Substring(Ĝ.Length).Trim().Split(M).FirstOrDefault();Echo(string.Format(ġ,Ĝ,Ż));float ź;if(
float.TryParse(Ż,out ź)&&ź>0){Ŀ=ź;}else{Ŀ=ō;}continue;}}}void Ź(){K.Clear();K.AppendFormat(Ġ,ğ,Ľ);K.Append(d);K.AppendFormat(
Ġ,Ğ,ě);K.Append(d);K.AppendFormat(Ġ,ĝ,ł);K.Append(d);K.AppendFormat(Ġ,Ĝ,ō);K.Append(d);K.AppendFormat(Ġ,Ɖ,string.Format(À
,Ľ));Me.CustomData=K.ToString();}void Ÿ(bool ŷ=false){t(string.Format(Ĩ,Ê));t(string.Format(ħ,w));t(string.Format(Ħ,Ì));ĸ
.Ą();ą.DampenersOverride=!w;Ň=Ļ.Ō;Runtime.UpdateFrequency=UpdateFrequency.None;ī=İ;ŋ=0;if(ŷ){return;}Echo(ĥ);Echo(ċ);Echo
(string.Format(Ĥ,(DateTime.Now-ļ).TotalSeconds.ToString(ś)));}bool Ŷ(){var ŵ=Runtime.TimeSinceLastRun;if(ŵ.TotalSeconds<ľ
.TotalSeconds){ľ-=ŵ;return false;}ľ+=TimeSpan.FromMilliseconds(ī);if(ī<10){Runtime.UpdateFrequency=UpdateFrequency.
Update1;}else{Runtime.UpdateFrequency=UpdateFrequency.Update10;}return true;}const string Ŵ="High-g Landing Factor: {0}";const
string ų="Landing Proximity Factor: {0}";const string ſ="Safe distance: {0} m";void Ų(){Ņ=ņ;var Ɓ=-ą.GetNaturalGravity();if(f
&&Ł<Ɔ){Ň=Ļ.Ŋ;}if(g||(f&&(Ł<ƈ))){Ņ*=-ń;ą.DampenersOverride=false;}if(g){var Ɠ=å/ĸ.Ü;var Ƒ=É(3+(float)Math.Pow(2*(1+Ɠ),3));Ƒ
=Math.Max(Ƒ,į);t(string.Format(ſ,Ƒ.ToString(ĉ)));var Ɛ=ƍ();t(string.Format(Ŵ,Ɛ));Ņ*=Ɛ;var Ə=Ƌ(2*Ƒ);t(string.Format(ų,Ə));
Ņ*=Ə;if(Ə<1){ī=İ*Ə*Ə;ī=Math.Max(ī,1);}if(Ƈ<Ƒ){if(j){var Ǝ=ĸ.è(0);ĸ.Ñ(Ɓ,Ǝ);return;}if(Ş>0){return;}}}ƅ();ĸ.Ñ(Ɓ,ß);}float ƍ
(){var ƌ=å;if(ƌ<=1)return 1;return(float)(1/Math.Pow(ƌ,2));}float Ƌ(float Ɗ){var ƒ=(float)(Ƈ/Ɗ);ƒ=Å(ƒ,0,1);return ƒ;}
private float å{get{return(float)ą.GetNaturalGravity().Length()/Ĉ;}}private float ƈ{get{double Ë;if(ą!=null&&ą.
TryGetPlanetElevation(MyPlanetElevation.Sealevel,out Ë)){return(float)Ë-ŀ;}return float.NaN;}}private float Ƈ{get{double Ë;if(ą!=null&&ą.
TryGetPlanetElevation(MyPlanetElevation.Surface,out Ë)){return(float)Ë-ŀ;}return float.NaN;}}private float Ɔ{get{return ƈ-Ƈ;}}void ƅ(){var Ƅ=
Ş;var ƃ=DateTime.Now;TimeSpan Ƃ=ƃ-Ń;var ű=(Ƅ-ŋ)/(float)Ƃ.TotalSeconds;var Š=Ņ-Ƅ;var Ŏ=Math.Abs(Š/Ņ);if(ű>Š){ß-=Ŏ;}else{ß
+=Ŏ;}ß=Å(ß,0,1);ŋ=Ƅ;Ń=ƃ;}private float Ş{get{var ŝ=ą.GetShipVelocities().LinearVelocity;var Ŝ=Vector3D.TransformNormal(ŝ,
MatrixD.Transpose(ą.WorldMatrix));return(float)Ŝ.Y;}}const string ś="F3";const string ĉ="F1";const int Ś=100;const string ř=
"State: {0}";const string Ř="Override: {0} %";const string ŗ="Velocity: {0} m/s";const string Ŗ="Altitude (AGL): {0}";const string ŕ
="Altitude (ASL): {0}";const string Ŕ="Cruise Altitude ASL: {0} m";const string œ="Touchdown Offset: {0} m";const string
Œ="Last Update: {0}";const string ő="Execution Delay: {0} ms";const string Ő="Script tag: {0}";const string ŏ=
"Max Ascent Speed: {0} m/s";const string ş="{0} m";const string š="Touchdown";const string Ű="In space";private enum î{ů,Ů,ŭ,Ŭ,ū,Ū,ũ,Ũ,ŧ,Á,Ŧ,ť}
string Ú(){K.Clear();if(ĺ){K.AppendLine(Ú(î.Á));K.AppendLine(Ú(î.Ů));K.AppendLine(Ú(î.ũ));K.AppendLine(Ú(î.ŧ));K.AppendLine(Ú(
î.Ũ));}K.AppendLine(Ú(î.ů));K.AppendLine(Ú(î.ŭ));K.AppendLine(Ú(î.Ŧ));K.AppendLine(Ú(î.Ŭ));K.AppendLine(Ú(î.ū));K.
AppendLine(Ú(î.Ū));K.AppendLine(Ú(î.ť));return K.ToString();}string Ú(î Ù){switch(Ù){case î.Ū:return string.Format(Ŕ,Ł.ToString(ĉ)
);case î.Ŭ:string Ť=string.Empty;double ţ=Ƈ;if(w){Ť=š;}else if(Ê){Ť=Ű;}else if(!double.IsNaN(ţ)){Ť=string.Format(ş,ţ.
ToString(ĉ));}return string.Format(Ŗ,Ť);case î.ū:string Ţ=string.Empty;double Ě=ƈ;if(w){Ţ=š;}else if(Ê){Ţ=Ű;}else if(!double.
IsNaN(Ě)){Ţ=string.Format(ş,Ě.ToString(ĉ));}return string.Format(ŕ,Ţ);case î.ŧ:return string.Format(ő,ī.ToString(ĉ));case î.Ŧ
:return string.Format(ŏ,Ņ.ToString(ĉ));case î.Ũ:return string.Format(Œ,Ń.ToLongTimeString());case î.Á:return string.
Format(Ő,Á);case î.ů:return string.Format(ř,Ň.ToString());case î.ť:var Ċ=ĸ.Ú(ö.î.Ü);var O=ĸ.Ú(ö.î.Û);return Ċ+d+O;case î.Ů:
return string.Format(Ř,(Ś*ß).ToString(ĉ));case î.ũ:return string.Format(œ,ŀ);case î.ŭ:return string.Format(ŗ,ŋ.ToString(ĉ));}
return string.Empty;}const char d='\n';void t(string E){if(ĺ){Echo(E);}}private IMyShipController s{get{var r=new List<
IMyShipController>();GridTerminalSystem.GetBlocksOfType(r);IMyShipController q=r.FirstOrDefault(o=>{return(o as IMyCockpit)!=null&&o.
IsMainCockpit;});IMyShipController p=r.FirstOrDefault(o=>{return(o as IMyRemoteControl)!=null&&o.IsMainCockpit;});return q??p;}}
double n(Base6Directions.Direction m){var l=ą.Position+Base6Directions.GetIntVector(ą.Orientation.TransformDirection(m));var k
=Vector3D.Normalize(Vector3D.Subtract(ą.CubeGrid.GridIntegerToWorld(l),ą.GetPosition()));return Vector3D.Dot(ą.
GetShipVelocities().LinearVelocity,k);}private bool j{get{var h=-Ş;return h>0&&h<=Į;}}private bool g{get{return Ň==Ļ.Ŋ;}}private bool f{
get{return Ň==Ļ.ŉ;}}private bool u{get{return Ň==Ļ.ň;}}private bool e{get{return Ň!=Ļ.Ō;}}private bool w{get{var Ë=Ƈ;return
!double.IsNaN(Ë)&&Ë<=É(1)&&j;}}private bool Ì{get{var Ë=ƈ;return!double.IsNaN(Ë)&&Math.Abs(Ë-Ł)<ŀ;}}private bool Ê{get{
return ą.GetNaturalGravity().LengthSquared()==0;}}float É(float È){var Ç=ą as IMyCubeBlock;var Æ=Ç.CubeGrid.GridSizeEnum;if(Æ
==MyCubeSize.Small){È*=Ĭ;}if(Æ==MyCubeSize.Large){È*=ĭ;}return È;}float Å(float Í,float Ä,float Ã){return Math.Max(Math.
Min(Í,Ã),Ä);}class Â{private string Á{set{if(string.IsNullOrWhiteSpace(value)){H=string.Empty;}else{H=string.Format(À,value
);}}get{return H;}}private const string À="[{0}]";private const string º="Script tag: {0}";private const string µ=
"Found {0} LCDs";private const string ª="Found {0} Screens";private const string z="Identified displays: {0}";private const string y=
"{0} (surface {1})";private const char d='\n';private const char M=' ';private const char A='@';private string H;private List<
IMyTextSurface>L=new List<IMyTextSurface>();private StringBuilder K=new StringBuilder();private MyGridProgram J;public Â(MyGridProgram
J,string H){this.J=J;I(H);}public void I(){L.Clear();if(string.IsNullOrWhiteSpace(Á)){return;}b(L);Z(L);}public void I(
string H){if(string.IsNullOrWhiteSpace(H)){return;}Á=H;I();}public void G(){try{L.ForEach(C=>C.WriteText(string.Empty,false));
}catch(NullReferenceException){I();}}public void F(string E,bool D=false){try{if(D){G();}L.ForEach(C=>Î(C,E));}catch(
NullReferenceException){I();}}public void B(bool D=false){var N=L.Count(C=>C as IMyTextPanel!=null);var c=L.Count()-N;K.AppendFormat(º,Á);K.
AppendFormat(µ,N);K.AppendLine();K.AppendFormat(ª,c);K.AppendLine();F(K.ToString(),D);}private void b(List<IMyTextSurface>L){J.
GridTerminalSystem.GetBlocksOfType(L,(X)=>{var a=(X as IMyTerminalBlock).CustomName.ToLower();bool V=a.Contains(H.ToLower());if(V){X.
ContentType=ContentType.TEXT_AND_IMAGE;J.Echo(string.Format(z,a));}return V;});}private void Z(List<IMyTextSurface>L){List<
IMyTerminalBlock>Y=new List<IMyTerminalBlock>();J.GridTerminalSystem.GetBlocksOfType(Y,(X)=>{var W=X as IMyTextSurfaceProvider;if(W==
null){return false;}bool V=X.CustomName.ToLower().Contains(H.ToLower());if(!V){return false;}var U=X.CustomData;var T=U.
Split(d);foreach(var S in T){if(!S.StartsWith(A.ToString())){continue;}string R=S.Substring(1).Split(M)[0];int Q;if(!int.
TryParse(R,out Q)){continue;}IMyTextSurface P=W.GetSurface(Q);P.ContentType=ContentType.TEXT_AND_IMAGE;string C=string.Format(y,
X.CustomName,Q);J.Echo(string.Format(z,C));L.Add(P);break;}return false;});}private void Î(IMyTextSurface C,string E){K.
Clear();K.Append(E);string Ā=C.Font;float ÿ=C.FontSize;float þ=C.MeasureStringInPixels(K,Ā,ÿ).Y;float ý=C.SurfaceSize.Y;int ü
=(int)Math.Floor(ý/þ);string û=C.GetText()+d+E;var T=û.Split(d);int ú=T.Length;int ù=Math.Max(0,ú-ü);K.Clear();for(int ø=
ù;ø<ú;ø++){K.Append(T[ø]);if(ø<ú-1){K.Append(d);}}C.WriteText(K.ToString(),false);}}class ö{private const string õ=
"WARNING: Thrust Vector outside parameters";private const string ô="Thrusters on grid: {0}";private const string ó="Thrusters on subgrids: {0}";private const
string ò="Total thrusters: {0}";private const string ñ="Total Lift thrusters: {0}";private const string ð=
"Maximum take-off gravity: {0}";private const string ā="Maximum take-off weight (1g): {0} Mkg";private const int ï=1000000;private const float Ă=0.05F;
private const string ĉ="F1";private const float Ĉ=9.8f;private List<IMyThrust>ć=new List<IMyThrust>();private MyGridProgram J;
private IMyShipController ą;private StringBuilder K;private bool Õ;private long Ć;private float é;public ö(MyGridProgram J,
IMyShipController ą,bool Õ){this.J=J;this.ą=ą;this.K=new StringBuilder();Ö(Õ);ê(Ă);ă();}public void Ą(){ć.ForEach(Ò=>á(Ò));}public void ă
(){Ć=ą.CubeGrid.EntityId;ć.Clear();J.GridTerminalSystem.GetBlocksOfType(ć,X=>{return X.CubeGrid.EntityId==Ć||Õ;});}public
float Û{get{return ä/Ĉ;}}public float Ü{get{return ä/(æ*Ĉ);}}public string Ú(){K.Clear();K.AppendLine(Ú(î.à));K.AppendLine(Ú(
î.Ï));K.AppendLine(Ú(î.Þ));K.AppendLine(Ú(î.Ý));K.AppendLine(Ú(î.Ü));K.AppendLine(Ú(î.Û));return K.ToString();}public
enum î{à,Ï,Þ,Ý,Ü,Û};public string Ú(î Ù){switch(Ù){case î.à:return string.Format(ò,ć.Count());case î.Ï:return string.Format(
ô,ć.Count(Ø=>Ø.CubeGrid.EntityId==Ć));case î.Þ:return string.Format(ó,ć.Count(Ø=>Ø.CubeGrid.EntityId!=Ć));case î.Ý:return
string.Format(ñ,ć.Count(Ø=>Ø.GridThrustDirection==Vector3I.Down));case î.Ü:return string.Format(ð,Ü.ToString(ĉ));case î.Û:
return string.Format(ā,(Û/ï).ToString(ĉ));default:return string.Empty;}}public void Ö(bool Õ){this.Õ=Õ;}public void Ô(){ć.
ForEach(Ò=>á(Ò));}public void Ó(){ć.ForEach(Ò=>â(Ò));}public void Ñ(Vector3D Ð,float ß){if(ß<0||ß>1){J.Echo(õ);ć.ForEach(Ò=>{Ò.
ThrustOverridePercentage=0.1337f;Ò.Enabled=true;});return;}Ð.Normalize();ć.ForEach(Ò=>{if(ã(Ò)<é){â(Ò);return;}Vector3D í=Ò.WorldMatrix.
GetOrientation().Backward;double ì=Vector3D.Dot(Ð,í);var ë=ì*ß;if(ë<-0.2){â(Ò);return;}Ò.ThrustOverridePercentage=(float)Math.Max(ë,0)
;Ò.Enabled|=Ò.ThrustOverridePercentage>0;});}public void ê(float é){this.é=é;}public float è(float ç){return(å+ç)*æ/ä;}
private float æ{get{return ą.CalculateShipMass().TotalMass;}}private float å{get{return(float)ą.GetNaturalGravity().Length();}}
private float ä{get{return ć.Where(Ø=>Ø.GridThrustDirection==Vector3I.Down).Sum(Ò=>Ò.MaxEffectiveThrust);}}private float ã(
IMyThrust Ò){return Ò.MaxEffectiveThrust/Ò.MaxThrust;}private void â(IMyThrust Ò){Ò.ThrustOverridePercentage=0;Ò.Enabled=false;}
private void á(IMyThrust Ò){Ò.ThrustOverridePercentage=0;Ò.Enabled=true;}private void v(string E)=>J.Echo(E);}