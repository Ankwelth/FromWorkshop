/*
 * This simple script monitors the fill level of your cargo. As inventory fills or empties,
 * the script changes the color of any light you have tagged. Color progresses from green 
 * (empty) to red (cargo full) to flashing red (now you are filling other things like connectors).
 * The script will also display a simple percentage on any tagged LCD.
 * 
 * Customization
 * Check the Custom Data to adjust settings. Recompile the block after making changes.
 * 
 * SCRIPT TAG
 * Place this text anywhere in the custom name field of any light or LCD that you want this
 * script to manage. Default value: [INVLT]
 * 
 * CARGO KEYWORD
 * Place this text anywhere in the custom name field of any cargo you want this script to
 * monitor. Default value: cargo
 * 
 * INCLUDE DRILLS
 * Determines if drill inventory is included in the calculations. Default value: false
 * 
 * COLOR STEPS
 * This is the number of steps between the "empty" color (green) and the "full" color (red).
 * This count includes both the "empty" and "full" color. So if you want "green -> yellow -> red,"
 * then you would put 3. Default value: 5.
 */
static class Ý{private const string Ü="Inventory Light";private const int Û=1;private const int Ú=0;private const int Ù=
17;private const int Ø=55;private const string Ö="Version {0}.{1}.{2}.{3}";public static string Õ=string.Format(Ö,Û,Ú,Ù,Ø)
;private const string Ô="{0}\n{1}";public static string Ó=string.Format(Ô,Ü,Õ);}const string Ò="SCRIPT TAG";const string
Ñ="[INVLT]";string Ð;const string Ï="CARGO KEYWORD";const string Î="cargo";string Í;const string Ì="INCLUDE DRILLS";const
bool Ë=false;bool Ê;const string É="COLOR STEPS";const int È=5;int x;List<IMyTextPanel>Ç=new List<IMyTextPanel>();List<
IMyLightingBlock>y=new List<IMyLightingBlock>();List<IMyCargoContainer>Æ=new List<IMyCargoContainer>();List<IMyShipDrill>Å=new List<
IMyShipDrill>();const char Ä='\n';const char Þ=':';const char Ã=' ';const string ß="%";const string ö=
"[color=#ff80c0ff]x{0}[/color]";const string ô="Displays ({0})";const string ó="Lights ({0})";const string ò="Cargo ({0})";const string ñ=
"Drills ({0})";const string ð="> {0}";const string ï="{0}: {1}";const string î="Fill: {0}";Program(){í();Me.GetSurface(0).WriteText(Ý.
Ó,append:true);Me.GetSurface(0).WriteText(Ä.ToString(),append:true);Me.GetSurface(0).WriteText(Ä.ToString(),append:true);
Echo(string.Empty);Echo(Ý.Ó);Echo(string.Empty);Runtime.UpdateFrequency=UpdateFrequency.Update10;B(Ç,Ð);B(y,Ð);B(Æ,Í);B(Å,
string.Empty);}void í(){Ð=Ñ;Í=Î;Ê=Ë;x=È;bool ì=false;bool ë=false;bool ê=false;bool é=false;var õ=Me.CustomData;var è=õ.Split(
Ä);foreach(var ç in è){if(ç.IndexOf(Ò,StringComparison.OrdinalIgnoreCase)==0){int æ=ç.IndexOf(Þ)+1;Ð=ç.Substring(æ).Trim(
);ì=true;continue;}if(ç.IndexOf(Ï,StringComparison.OrdinalIgnoreCase)==0){int æ=ç.IndexOf(Þ)+1;Í=ç.Substring(æ).Trim();ë=
true;continue;}if(ç.IndexOf(Ì,StringComparison.OrdinalIgnoreCase)==0){int æ=ç.IndexOf(Þ)+1;string ã=ç.Substring(æ);bool.
TryParse(ã.Trim(),out Ê);ê=true;}if(ç.IndexOf(É,StringComparison.OrdinalIgnoreCase)==0){int æ=ç.IndexOf(Þ)+1;string ã=ç.
Substring(æ);int.TryParse(ã.Trim(),out x);if(x<2){x=2;}é=true;}}if(!ì){å(Ò,Ñ);}if(!ë){å(Ï,Î);}if(!ê){å(Ì,Ë.ToString());}if(!é){å(
É,È.ToString());}}void å(string ä,string ã){if(!string.IsNullOrEmpty(Me.CustomData)&&Me.CustomData.Length>0&&Me.
CustomData[Me.CustomData.Length-1]!=Ä){Me.CustomData+=Ä;}Me.CustomData+=ä+Þ+Ã+ã+Ä;}void â(float á){if(á<0.95){Runtime.
UpdateFrequency=UpdateFrequency.Update100;return;}Runtime.UpdateFrequency=UpdateFrequency.Update10;}void Main(string à,UpdateType Â){
Echo(DateTime.Now.ToLongTimeString());Echo(Ý.Õ);bool c=N(y);if(c){B(y,Ð);}º(y,Q);bool A=N(Ç);if(A){B(Ç,Ð);}º(Ç,R);bool b=N(Æ
);if(b){B(Æ,Í);}º(Æ,P);if(Ê){bool a=N(Å);if(a){B(Å,string.Empty);}º(Å,O);}float Z,Y;i(Æ,out Z,out Y);float X=0,W=0;if(Ê){
i(Å,out X,out W);}float V=(Z+X)/(Y+W);string U=V.ToString("P0");Echo(Ã.ToString());Echo(string.Format(î,U));foreach(var S
in Ç){S.WriteText(U);}z(y,x,V);â(V);Echo(Runtime.UpdateFrequency.ToString());}private string R{get{return string.Format(ô,
I(Ç.Count.ToString()));}}private string Q{get{return string.Format(ó,I(y.Count.ToString()));}}private string P{get{return
string.Format(ò,I(Æ.Count.ToString()));}}private string O{get{return string.Format(ñ,I(Å.Count.ToString()));}}bool N<M>(List<M
>L)where M:IMyTerminalBlock,IMyEntity{bool K=false;foreach(var J in L){if(J==null||J.Closed){K=true;continue;}}return K;}
string I(string C){return string.Format(ö,C);}const string H="[";const string G="]";const string F="[[";const string E="]]";
string D(string C){return C.Replace(H,F).Replace(G,E);}void B<M>(List<M>d,string À)where M:class,IMyTerminalBlock{d.Clear();
bool Á=!string.IsNullOrWhiteSpace(À);GridTerminalSystem.GetBlocksOfType(d,ª=>{if(!ª.IsSameConstructAs(Me)){return false;}if(
!Á){return true;}return ª.CustomName.IndexOf(À,StringComparison.OrdinalIgnoreCase)>-1;});}void º<M>(List<M>d,string µ)
where M:class,IMyTerminalBlock{Echo(Ã.ToString());Echo(µ);foreach(var ª in d){Echo(string.Format(ð,D(ª.CustomName.ToString())
));}}void z(List<IMyLightingBlock>y,int x,float V){float w=0.99f/x;int v=(int)Math.Floor(V/w);float u=v/(float)(x-1);int
t=(int)(85*Math.Pow(v,3)-425*Math.Pow(v,2)+595*v);int s=(int)Math.Round(o(255*(1-u),0,255));Color r=new Color(t,s,0);
float q=V>0.99?0.5f:0;foreach(var p in y){p.Color=r;p.BlinkIntervalSeconds=q;}}float o(float n,float l,float j){return n<l?l:
n>j?j:n;}bool i<M>(List<M>h,out float g,out float f)where M:IMyEntity{g=0;f=0;if(h==null||h.Count==0){return false;}
foreach(var ª in h){if(!ª.HasInventory){continue;}IMyInventory e=ª.GetInventory(0);g+=(float)e.CurrentVolume;f+=(float)e.
MaxVolume;}return true;}