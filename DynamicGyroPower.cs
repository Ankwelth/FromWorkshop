/*
 * R e a d m e
 * -----------
 * 
 * Dynamic Gyro Power Levels
 * Patrick Heney
 * 
 * After installing in a Programmable block, check the Custom Data area.
 * 
 * The "Base power" setting controls the default power level of the gyros,
 * which is used when the ship is empty.
 * 
 * The "Max load" setting sets the cargo mass at which the gyros should be
 * operating at full power. 
 * "auto" is the default value. The script will calculate the max load from your base power. 
 * To override, just provide a number (in kilograms).
 * 
 * The "Fidelity" setting controls how often the power level is updated.
 * "High" fidelity will update the gyros about 3~6 times per second.
 * "Low" fidelity will update the gyros about once every 1.5 seconds.
 */
static class r{private const string q="Dynamic Gyro Power";private const int p=1;private const int o=0;private const int
n=0;private const int m=2;private const string l="Version {0}.{1}.{2}.{3}";public static string s=string.Format(l,p,o,n,m
);private const string k="{0}\n{1}";public static string i=string.Format(k,q,s);}IMyShipController h;List<IMyGyro>g;float
f;const float e=0.3f;const string d="Base power:";string c;const string j=
"WARNING: Cargo exceeds operational envelope. ({0} kg max weight)\nShip functioning at suboptimal performance.";const string b="Fidelity:";const string t="high";const string Å="low";int Ä;const string Ã="Max load:";const string Â=
"auto";const char Á='\n';const char À=' ';const string º="n0";Program(){Me.GetSurface(0).WriteText(r.i,append:true);Me.
GetSurface(0).WriteText(Á.ToString(),append:true);Me.GetSurface(0).WriteText(Á.ToString(),append:true);Echo(string.Empty);Echo(r.i
);Echo(string.Empty);long µ=Me.CubeGrid.EntityId;g=new List<IMyGyro>();GridTerminalSystem.GetBlocksOfType(g,ª=>{return ª.
CubeGrid.EntityId==µ;});h=B;if(!N(Me.CustomData)){A();Echo(
"Default config written to CustomData.\nRecompile if the config is changed.");N(Me.CustomData);}c=string.Format(j,Ä.ToString(º));Runtime.UpdateFrequency=UpdateFrequency.Update100;}void Main(string
P,UpdateType z){Echo(C.ToLongTimeString());if(z==UpdateType.Terminal&&!string.IsNullOrWhiteSpace(P)){a(P);}float y=f+((1-
f)*w(h));g.ForEach(x=>{x.GyroPower=y;});if(y>1){Echo(c);}}float w(IMyShipController h){var v=h.CalculateShipMass();float
u=v.TotalMass-v.BaseMass;return u/Ä;}bool a(string P){if(P.IndexOf(Å,StringComparison.OrdinalIgnoreCase)>-1){Runtime.
UpdateFrequency=UpdateFrequency.Update100;return true;}if(P.IndexOf(t,StringComparison.OrdinalIgnoreCase)>-1){Runtime.UpdateFrequency=
UpdateFrequency.Update10;return true;}return false;}void A(){StringBuilder M=new StringBuilder();M.Append(d);M.Append(À);M.Append(e);M.
AppendLine();M.Append(Ã);M.Append(À);M.Append(Â);M.AppendLine();M.Append(b);M.Append(À);M.Append(Å);M.AppendLine();Me.CustomData=M
.ToString();}bool N(string M){bool L=false;bool K=false;bool J=false;M.Split(Á).ToList().ForEach(I=>{int O=I.IndexOf(d,
StringComparison.OrdinalIgnoreCase);if(O>-1){int F=O+d.Length;float.TryParse(I.Substring(F),out f);L=true;return;}O=I.IndexOf(b,
StringComparison.OrdinalIgnoreCase);if(O>-1){int F=O+b.Length;string G=I.Substring(F);K=a(G);return;}O=I.IndexOf(Ã,StringComparison.
OrdinalIgnoreCase);if(O>-1){int F=O+Ã.Length;float E;if(float.TryParse(I.Substring(F),out E)){Ä=(int)E;J=true;}else if(I.IndexOf(Â,
StringComparison.OrdinalIgnoreCase)>-1){float D=h.CalculateShipMass().BaseMass;Ä=(int)((D/f)-D);J=true;}return;}});if(f<=0||f>1){L=false
;}if(float.IsNaN(Ä)){J=false;}return L&&K&&J;}private DateTime C{get{return DateTime.Now;}}private IMyShipController B{
get{return H(Q:true,S:true);}}IMyShipController H(bool Q=true,bool S=true){var Z=new List<IMyShipController>();
GridTerminalSystem.GetBlocksOfType(Z);IMyShipController Y=null;IMyShipController X=null;IMyShipController W=null;IMyShipController V=null;
IMyShipController U=null;IMyShipController T=null;if(Q){Y=Z.Where(R=>(R as IMyCockpit)!=null).FirstOrDefault(R=>R.IsUnderControl);X=Z.
Where(R=>(R as IMyCockpit)!=null).FirstOrDefault(R=>R.IsMainCockpit);W=Z.Where(R=>(R as IMyCockpit)!=null).FirstOrDefault(R=>
!R.IsMainCockpit);}if(S){V=Z.Where(R=>(R as IMyRemoteControl)!=null).FirstOrDefault(R=>R.IsUnderControl);U=Z.Where(R=>(R
as IMyRemoteControl)!=null).FirstOrDefault(R=>R.IsMainCockpit);T=Z.Where(R=>(R as IMyRemoteControl)!=null).FirstOrDefault(
R=>!R.IsMainCockpit);}return Y??V??X??U??W??T;}