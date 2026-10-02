// Usstan's Quota Manager (UQM)
// Version 0.3.0
// 
// I built this script because I needed a way to turn off blocks like Algae Farms, 
// Collectors (water mod ice) and stone mining, but I'm also using it for other things.
// 
// UQM is a simple quota manager for Space Engineers, it's fully automated 
// script enables or disables functional blocks based on the total inventory 
// amount of a specified resources across all blocks with storage on the same 
// grid. It scans the main grid (where the PB is) for blocks that have "[Managed]"
// in their name, and reads quota settings from the blocks Custom Data. 
// 
// When you add [Managed] to a block's name, UQM will check if it have a UQM
// configuration in the Custom Data, and if not, it will add the config section
// with some examples and the block will show up in the list on the screen in 
// "Config" state. 
// 
// Just go to the custom Data and specify what resource type and resource you are
// looking for and specify the amount. Example below:
// 
// [Quota Managed]
//   Quota=MyObjectBuilder_PhysicalObject:Algae:100
// [/Quota Managed]
// 
// and if not it doesn't already have a managed section in Custom Data, 
// the script will add a template for you to configure, just give it a moment 
// to run after renaming the block.
// 
// If you want to display the list on LCD(s), then add [UQM-Status] to the LCD(s) 
// name.
// 

    // Keyword to identify managed blocks, if you want to use something else than [Managed], change it here.
    string UQM_Keyword = "[Managed]";

    public Program(){Runtime.UpdateFrequency=UpdateFrequency.Update100;}public void
 Main(string A,UpdateType B){Echo("")
;Dictionary<string,double>D=C();List<IMyTerminalBlock>E=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<
IMyTerminalBlock>(E,F=>F.IsSameConstructAs(Me)&&F.CustomName.Contains(UQM_Keyword));List<G>H=new List<G>();foreach(var F in E){G J=I(F,D
);H.Add(J);}H.Sort((K,L)=>string.Compare(K.M,L.M,StringComparison.OrdinalIgnoreCase));IMyTextSurface N=Me.GetSurface(0);
bool O=Me.BlockDefinition.SubtypeId.Contains("Reskin");long P=Me.EntityId;Q(N,P,H,E.Count,D.Count,O,R,0.55f);List<
IMyTextPanel>S=new List<IMyTextPanel>();GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(S,T=>T.IsSameConstructAs(Me)&&T.CustomName.
Contains("[UQM-Status]"));foreach(var T in S){float U,V;W(T,out U,out V);Q(T,T.EntityId,H,E.Count,D.Count,false,U,V);}X(D);Echo(
string.Format("Managed Blocks: {0}",E.Count));Echo(string.Format("UQM-Status Displays: {0}",S.Count));Echo(string.Format(
"Inventory Types: {0}",D.Count));}Dictionary<string,double>C(){Dictionary<string,double>Y=new Dictionary<string,double>();List<
IMyTerminalBlock>Z=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(Z,F=>F.CubeGrid==Me.CubeGrid&&F.
HasInventory);foreach(var b in Z){for(int a=0;a<b.InventoryCount;a++){IMyInventory c=b.GetInventory(a);List<MyInventoryItem>d=new
List<MyInventoryItem>();c.GetItems(d);foreach(var e in d){string f=string.Format("{0}:{1}",e.Type.TypeId,e.Type.SubtypeId);
double g=(double)e.Amount;if(Y.ContainsKey(f)){Y[f]+=g;}else{Y[f]=g;}}}}return Y;}void W(IMyTextPanel T,out float h,out float
i){h=R;i=0.55f;string j=T.CustomData;if(!j.Contains("[UQM-Config]")){string k="[UQM-Config]\n"+
"// Configuration for this display\n"+"// ScrollSpeed: Speed of scrolling (default 10)\n"+"// FontSize: Size of text (default 0.55)\n"+"ScrollSpeed=10\n"+
"FontSize=0.55\n"+"[/UQM-Config]\n";if(string.IsNullOrWhiteSpace(j)){T.CustomData=k;}else{T.CustomData=j.TrimEnd()+"\n\n"+k;}return;}int
l=j.IndexOf("[UQM-Config]");int m=j.IndexOf("[/UQM-Config]");if(l>=0&&m>l){string k=j.Substring(l,m-l);string[]n=k.Split(
'\n');foreach(string o in n){string p=o.Trim();if(p.StartsWith("ScrollSpeed=")){float q;if(float.TryParse(p.Substring(12),
out q)){h=q;}}else if(p.StartsWith("FontSize=")){float q;if(float.TryParse(p.Substring(9),out q)){i=q;}}}}}void X(
Dictionary<string,double>Y){string r=Me.CustomData;int l=r.IndexOf("[UQM]");int m=r.IndexOf("[/UQM]");string s="";string t="";if(l
>=0&&m>=0){s=r.Substring(0,l);t=r.Substring(m+6);}else{s=r;}StringBuilder u=new StringBuilder();u.AppendLine("[UQM]");u.
AppendLine("// Inventory Item Types");u.AppendLine("// Auto-generated list - do not edit");u.AppendLine();List<string>v=new List<
string>(Y.Keys);v.Sort();string w=string.Empty;foreach(string f in v){string x=f.Split(':')[0];if(x!=w&&w!=string.Empty){u.
AppendLine();}w=x;u.AppendLine(f);}u.AppendLine("[/UQM]");Me.CustomData=(s.TrimEnd()+"\n\n"+u.ToString()+t).Trim();}void Q(
IMyTextSurface N,long P,List<G>H,int y,int z,bool O,float h,float i){N.ContentType=ContentType.SCRIPT;N.Script="";N.
ScriptBackgroundColor=Color.Black;N.ScriptForegroundColor=Color.White;Vector2 ª=N.SurfaceSize;Vector2 µ=ª/2f;float º=O?50f:0f;const float À=
3f;float Á=i*55f+À;using(var Â=N.DrawFrame()){var Ã=new MySprite(SpriteType.TEXTURE,"SquareSimple",µ,ª,Color.Black);Â.Add(
Ã);float Ä;if(N is IMyTextPanel){Ä=0f;}else if(O){Ä=50f;}else{Ä=100f;}float Å=Ä;var Æ=new MySprite(SpriteType.TEXTURE,
"SquareSimple",new Vector2(µ.X+º,Ä+20),new Vector2(ª.X-20,50),new Color(20,40,60));Â.Add(Æ);var Ç=new MySprite(SpriteType.TEXT,
"Usstan's Quota Manager",new Vector2(µ.X+º,Ä),null,Color.White,"White",TextAlignment.CENTER,1.0f);Â.Add(Ç);Ä+=55f;var È=new MySprite(SpriteType.
TEXT,string.Format("Managed: {0}  |  Inventory: {1}",y,z),new Vector2(µ.X+º,Ä),null,new Color(200,200,200),"White",
TextAlignment.CENTER,0.6f);Â.Add(È);Ä+=35f;float É=Ä;if(!Ê.ContainsKey(P)){Ê[P]=new Ë();}Ë Ì=Ê[P];Ä-=Ì.Í;foreach(var J in H){if(Ä<É){
Ä+=Á;continue;}Color Î;string Ï;if(J.Ð=="RUNNING"){Î=new Color(50,200,50);Ï="IconEnergy";}else if(J.Ð=="STOPPED"){Î=new
Color(200,50,50);Ï="Cross";}else if(J.Ð=="CONFIG"){Î=new Color(200,200,50);Ï="Circle";}else{Î=new Color(200,100,0);Ï=
"IconWarning";}var Ñ=new MySprite(SpriteType.TEXTURE,"SquareSimple",new Vector2(µ.X+º,Ä+12),new Vector2(ª.X-40,28),new Color(Î.R/4,Î.
G/4,Î.B/4));Â.Add(Ñ);var Ò=new MySprite(SpriteType.TEXTURE,"SquareSimple",new Vector2(30+º,Ä+12),new Vector2(8,28),Î);Â.
Add(Ò);var Ó=new MySprite(SpriteType.TEXTURE,Ï,new Vector2(60+º,Ä+12),new Vector2(20,20),Î);Â.Add(Ó);string Ô=J.M.Replace(
UQM_Keyword,"").Trim();string Õ=Ô;if(J.Ð=="ERROR"&&J.Ö!=null){Õ=string.Format("{0} - {1}",Ô,J.Ö);}var Ø=new MySprite(SpriteType.
TEXT,Õ,new Vector2(90+º,Ä),null,Color.White,"White",TextAlignment.LEFT,i);Â.Add(Ø);if(J.Ð=="RUNNING"||J.Ð=="STOPPED"){string
Ú=J.Ù?">":"<";string Ý=string.Format("{0:N0}/{1}{2:N0}",J.Û,Ú,J.Ü);var Þ=new MySprite(SpriteType.TEXT,Ý,new Vector2(ª.X-
30+º,Ä),null,new Color(180,180,180),"White",TextAlignment.RIGHT,i*0.9f);Â.Add(Þ);}Ä+=Á;}float ß=Ä+Ì.Í-É;float à=ª.Y-É+Å;
float á=Math.Max(0,ß-à+40);if(á>0){Ì.Í+=h*Ì.â;if(Ì.Í>=á){Ì.Í=á;Ì.â=-1;}else if(Ì.Í<=0){Ì.Í=0;Ì.â=1;}}else{Ì.Í=0;}}}G I(
IMyTerminalBlock F,Dictionary<string,double>Y){G J=new G();J.M=F.CustomName;string j=F.CustomData;if(!ã(j)){ä(F);J.Ð="CONFIG";return J;}
string æ=å(j);if(string.IsNullOrEmpty(æ)){J.Ð="ERROR";J.Ö="No Quota";return J;}if(æ=="MyObjectBuilder_Ore:Foo:0"){J.Ð="CONFIG"
;return J;}string[]ç=æ.Split(':');if(ç.Length!=3){J.Ð="ERROR";J.Ö="Invalid Format";return J;}string f=string.Format(
"{0}:{1}",ç[0],ç[1]);string è=ç[1];string é=ç[2];bool ê=é.StartsWith(">");if(ê||é.StartsWith("<"))é=é.Substring(1);double ë;if(!
double.TryParse(é,out ë)){J.Ð="ERROR";J.Ö="Invalid Amount";return J;}double ì=Y.ContainsKey(f)?Y[f]:0;J.Û=ì;J.Ü=ë;J.Ù=ê;
IMyFunctionalBlock í=F as IMyFunctionalBlock;if(í==null){J.Ð="ERROR";J.Ö="Not Functional";return J;}if(ê){if(ì>ë){if(!í.Enabled)í.Enabled=
true;J.Ð="RUNNING";}else{if(í.Enabled)í.Enabled=false;J.Ð="STOPPED";}}else{if(ì>=ë){if(í.Enabled)í.Enabled=false;J.Ð=
"STOPPED";}else{if(!í.Enabled)í.Enabled=true;J.Ð="RUNNING";}}return J;}string å(string j){int l=j.IndexOf("[Quota Managed]");int
m=j.IndexOf("[/[Quota Managed]]");if(l<0||m<0||m<=l){return null;}string î=j.Substring(l,m-l);string[]n=î.Split('\n');
foreach(string o in n){string p=o.Trim();if(p.StartsWith("Quota=")&&!p.StartsWith("//")){return p.Substring(6);}}return null;}
bool ã(string j){return j.Contains("[Quota Managed]");}void ä(IMyTerminalBlock F){string r=F.CustomData;string î=
"[Quota Managed]\n"+"// Example configuration\n"+"// Quota=ResourceType:ResourceName:Amount\n"+"// Examples:\n"+
"// Quota=MyObjectBuilder_Ore:Ice:5000\n"+"// Quota=MyObjectBuilder_Ore:Stone:<2000\n"+"// Quota=MyObjectBuilder_Ingot:Stone:>2000\n"+
"// Quota=MyObjectBuilder_PhysicalObject:Algae:100\n"+"//\n"+"// You can get a full list of resource types by looking \n"+"// in the Programmable Blocks Custom Data.\n"+"\n"
+"Quota=MyObjectBuilder_Ore:Foo:0\n"+"[/[Quota Managed]]\n";if(string.IsNullOrWhiteSpace(r)){F.CustomData=î;}else{F.
CustomData=r.TrimEnd()+"\n\n"+î;}}Dictionary<long,Ë>Ê=new Dictionary<long,Ë>();const float R=10f;class Ë{public float Í=0f;public
int â=1;}
}
public class G{public string M;public string Ð;public string Ö;public double Û;public double Ü;public bool Ù;