/*
 * Instruction Manual
 * -----------
 * 
 * 1. Place 2 LCD Screens, name one of them Automatic Drill LCD Panel, the second one Stine Log
 * 2. Place down your cargo containers, in the name of the cargos put [Ores]
 * 3. Place your Programming Block, add script.
 * 4. Modify values of production that you want, Stone, Ice, que amount ect.
 * 5. Run and enjoy!
 * 6. Leave feedback Please
 */

// Name of the Debug Log Console
private static string _logConsole = "Stine Log";

// Name of the Status Console
private static string _consoleName = "Automatic Drill LCD Panel";

// If using Ivy Inventory Manager, you will want to name the block with !manual on the end of it.
// Must use the tag [Stine] for it to find the drill for checking gravel levels.
private static string _cargoContainerOre = "[Ores]";

// ORE/STONE Production
private bool _queStone = true; // Turn on or off Stone Production
private bool _queOre = true; // Turn on or off Automatic Ore Production
private bool _queIce = true; // Turn on or off Automatic Ice production
double _queAmount = 25; // How much ore should be in the que?
double _iceAmount = 5; // How much Ice to que?
// Turns the drill off automatically
private float _maxCargoFill = 98.0f; // How much cargo to fill 98%?

// EDIT ABOVE ONLY PLEASE UNLESS YOU KNOW WHAT YOU ARE DOING.
static string BINNUMB = "10101100110101";
private string outputText = "";
float k=0.0f;float j=0.0f;float h=0.0f;public Color g{get;set;}IMyTextPanel l;IMyTextPanel m;Program(){Runtime.
UpdateFrequency=UpdateFrequency.Update100;ª();}void ª(){l=GridTerminalSystem.GetBlockWithName(_logConsole)as IMyTextPanel;m=
GridTerminalSystem.GetBlockWithName(_consoleName)as IMyTextPanel;}void Save(){}void Main(string z,UpdateType y){if(l==null){Echo(
"Place LCD Screen Named: Stine Log\n If you want to have some additional information!");return;}if(m==null){Echo("Couldn't find LCD named: Automatic Drill LCD Panel!\n");return;}w();}void w(){Echo(
"Stine Drill Platform-\n Manager Running!");List<MyInventoryItem>v=new List<MyInventoryItem>();List<IMyTerminalBlock>u=new List<IMyTerminalBlock>();
GridTerminalSystem.SearchBlocksOfName("[Stine]",u);for(int P=0;P<u.Count;P++){var t=u[P]as IMyInventoryOwner;var s=t is IMyCubeBlock?(t as
IMyCubeBlock).DisplayNameText:"<No Name>";t.GetInventory(0).GetItems(v);string r="";string q="";string p=string.Empty;foreach(
MyInventoryItem o in v){if((u[P]as IMyFunctionalBlock).Enabled==true){if(u.Count>0){u.ForEach(G=>{r=B(G.GetInventory().CurrentMass.
RawValue);q=B(150000000000);p+="\n * "+G.DisplayNameText+"\n";p+="  - Current Gravel: "+r.ToString()+"\n";p+=
"  - Expected Gravel: "+q.ToString()+"\n";if(G.GetInventory().CurrentMass.RawValue>=150000000000){f(G as IMyProductionBlock);}else{J(G as
IMyProductionBlock);}});}}}string n=p;n=Y(n,1,"\n[ Monitoring Drills - "+v.Count+"]\n---------------\n");b(n);}}void f(IMyProductionBlock
L){MyDefinitionId d=MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/P3Dstonetoice");double N=1;if(!L.
CanUseBlueprint(d)){A(" - Blueprint Error",false);}else{if(L.IsQueueEmpty){L.AddQueueItem(d,N);}}}void M(IMyProductionBlock L){List<
MyDefinitionId>K=new List<MyDefinitionId>();K.Add(MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/IronOre"));K.Add(
MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/NickelOre"));K.Add(MyDefinitionId.Parse(
"MyObjectBuilder_BlueprintDefinition/CobaltOre"));K.Add(MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/MagnesiumOre"));K.Add(MyDefinitionId.Parse(
"MyObjectBuilder_BlueprintDefinition/SiliconOre"));K.Add(MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/SilverOre"));K.Add(MyDefinitionId.Parse(
"MyObjectBuilder_BlueprintDefinition/GoldOre"));K.Add(MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/PlatinumOre"));K.Add(MyDefinitionId.Parse(
"MyObjectBuilder_BlueprintDefinition/UraniumOre"));if(_queStone){K.Add(MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/StoneOre"));}if(_queIce){K.Add(
MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/x100P3Dstonetoice"));}if(L.IsQueueEmpty){K.ForEach(G=>{if(!L.CanUseBlueprint
(G)){A("*** Blueprint Error ***\n"+" -- "+G.SubtypeName.ToString(),false);}else{if(_queOre){if(G.SubtypeName.ToString().
Contains("x100P3Dstonetoice")){L.AddQueueItem(G,_iceAmount);}else{L.AddQueueItem(G,_queAmount);}}}});}}void J(IMyProductionBlock
L){List<IMyTerminalBlock>I=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName(_cargoContainerOre,I);if(I.
Count==0){Echo("Can't find cargo containers\n with name [Ores]");return;}I.ForEach(G=>{k+=(float)G.GetInventory().
CurrentVolume;j+=(float)G.GetInventory().MaxVolume;h=k/j;if(h<=_maxCargoFill){M(L);}else{L.Enabled=false;Echo(BINNUMB);A(String.
Format("\nPower Monitor: Off\n"+"  - [x] Cargo Used: {0:P0}\n",h),false);}});F(_maxCargoFill,h,I.Count);}void F(float E,float
D,int C){outputText+="Drill Platform Log \n"+"-------------------------------\n";outputText+=
"  - [x] Processing Ore: On\n";outputText+="  - [x] Checking Cargo Space\n";outputText+=String.Format("  - [x] Total Containers: {0} \n",C);outputText
+=String.Format("  - [x] Max Fill: {0}% \n",E);outputText+="\n Power Monitor: On\n";outputText+=String.Format(
"  - Cargo Used: {0:P2}\n",D);l.WriteText(outputText,false);outputText=string.Empty;}string B(long H){return((float)H/1000000).ToString("#,##0.00"
)+" kg";}void A(string O,bool e){if(l==null){return;}g=Color.Orange;l.SetValue("FontColor",g);l.WriteText(O+"\n",e);}void
c(string O){if(l==null){return;}l.WriteText(O,false);}void b(string X){if(m!=null){m.WriteText(X);}}int a=0;int Z=17;
string Y(string X,int W=1,string V=null,string U=null){int T=Z;string S="";string[]R=X.Split('\n');int Q=R.Length;int G=0;if(V
!=null){S+=V+"\n";T=T-1;}if(U!=null)T=T-1;if(T<Q){for(int P=0;P<T;P++){if(P+a<Q){S+=R[P+a]+"\n";G=0;}else{S+=R[G]+"\n";G++
;}}}else{S=S+X;}if(U!=null)S+=U+"\n";a+=W;if((Q)-1<a)a=0;else if(a<0)a=Q-1;return S;}