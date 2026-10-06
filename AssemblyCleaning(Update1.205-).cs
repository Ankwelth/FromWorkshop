// 


    // How often the main logic runs, in seconds.
    // For example, if timer = 6, the script logic runs every 6 seconds.
    int timer = 6;

    // If true, the script will remove components from assemblers every cycle.
    bool removeComponents = true;

    // If true, the script will prioritize specific cargo containers 
    // based on matching names (see below).
    bool prioritizeCargo = true;

    // If true, the script will only interact with blocks on the same grid 
    // as the Programmable Block. Subgrids (e.g., rotors, pistons, docked ships) are ignored.
    bool ignoreSubgrids = true;

    // If prioritizeCargo is enabled, ingots will be moved into cargo containers
    // whose names contain this string (case-insensitive).
    string ingotsCargo = "ingot";

    // If prioritizeCargo is enabled, components will be moved into cargo containers
    // whose names contain this string (case-insensitive).
    string componentCargo = "component";

    /*

    No need to edit Below this point.

    */

    const int A=6;int B=>timer*A;List<IMyTerminalBlock>C=new List<IMyTerminalBlock>();List<IMyTerminalBlock>D=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>E=new List<IMyTerminalBlock>();int F=0;readonly G H;public
 Program
(){H=new G(this,Me);Runtime.UpdateFrequency=UpdateFrequency.Update10;I();}void I(){GridTerminalSystem.GetBlocksOfType<
IMyAssembler>(D,J);GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(C,J);GridTerminalSystem.GetBlocksOfType<IMyRefinery>(E,J);}
bool J(IMyTerminalBlock K){return!ignoreSubgrids||K.CubeGrid==Me.CubeGrid;}public void
 Main
(string L,UpdateType M){F++;if(F%A==0){N();}if(F<B)return;F=0;var R=new[]{new{O=D,P=0,Q=ingotsCargo},new{O=D,P=1,Q=
componentCargo},new{O=E,P=1,Q=ingotsCargo}};foreach(var S in R){foreach(var K in S.O){try{if(K is IMyAssembler){if(S.P==0&&T((
IMyAssembler)K))continue;if(S.P==1&&!removeComponents)continue;}U(K.GetInventory(S.P),S.Q);}catch(NullReferenceException){I();}}}}
void N(){H.V($"Tick: {F} / {B}");H.V($"Next logic run in: {(B-F)/A}s");H.W("Clean Assembly",F);H.X();H.V(
$"Containers found: {C.Count}");H.V($"Assemblers found: {D.Count}");H.V($"Refineries found: {E.Count}");}void U(IMyInventory Y,string Z){C.Sort((a,b)
=>{bool c=prioritizeCargo&&!string.IsNullOrWhiteSpace(Z)&&a.CustomName.ToLower().Contains(Z);bool d=prioritizeCargo&&!
string.IsNullOrWhiteSpace(Z)&&b.CustomName.ToLower().Contains(Z);return d.CompareTo(c);});foreach(var e in C){IMyInventory f=e
.GetInventory(0);if(g(f))continue;List<MyInventoryItem>h=new List<MyInventoryItem>();Y.GetItems(h);foreach(var i in h){Y.
TransferItemTo(f,0,null,true);}if(h.Count>0)return;}}float k(IMyInventory j){return(float)j.CurrentVolume/(float)j.MaxVolume*100f;}
bool g(IMyInventory j){return k(j)>=99f;}bool T(IMyAssembler l){return!l.IsQueueEmpty;}
}
public class G{private const string m="2025-04-17 01:59";private readonly Program n;private readonly
IMyProgrammableBlock o;bool p=false;private readonly string[]q=new[]{"( *       )","(  *      )","(   *     )","(    *    )","(     *   )",
"(      *  )","(       * )","(      *  )","(     *   )","(    *    )","(   *     )","(  *      )",};int r=0;public G(Program s,
IMyProgrammableBlock t){n=s;o=t;}public string y(){try{var u=DateTime.ParseExact(m,"yyyy-MM-dd HH:mm",null);int v=u.Year%100;int w=u.
DayOfYear;string x=$"{u.Hour:D2}{u.Minute:D2}";return$"v{v:D2}-{w:D3}-{x}";}catch{return"v00-000-0000";}}public void X(){r=(r+1)%
q.Length;V(q[r]);}public void W(string z,int ª){V($"{z} - {y()}");}public void V(string µ){n.Echo(µ);}public void Ã(
IMyTextSurface º,string µ,bool À=false){if(º==null)return;try{Á(º,µ);º.WriteText(µ+'\n',À);p=true;}catch(Exception Â){V(
"Surface write error:");V(Â.Message);}}public void Å(string µ,List<IMyTextSurface>Ä){p=false;foreach(var º in Ä){Ã(º,µ,À:p);p=true;}}void Á(
IMyTextSurface º,string µ){Vector2 Æ=º.MeasureStringInPixels(new StringBuilder(µ),º.Font,1f);Vector2 Ç=º.SurfaceSize;float È=(Ç.X*(1-º
.TextPadding*0.02f))/Æ.X;if(º.FontSize>È||!p){º.FontSize=È;}}public void É(){p=false;}