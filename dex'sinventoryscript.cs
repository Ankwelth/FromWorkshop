/*
 * R e a d m e
 * -----------
 * 
 * author: dexyr
 * version: 1.0
 * release: 8-19-20
 * 
 * what it does:
 * + inventories (except assemblers, reactors, etc.) can be tagged with category/item names and will sort themselves,
 *     assuming that there is enough space to move items around (think of it as a whitelist system)
 * + allows tags to be changed in the custom data (so you can use Fe instead of iron)
 *     (tags are case-sensitive; check the programming block's custom data if you are unsure of how a block is tagged)
 * 
 * what it does not do:
 * - work reliably (hasn't been tested enough)
 * - run well (startup is ~10-20ms and each subsequent run is ~0.2-0.8ms)
 * - interfere with assemblers, refineries, reactors, or o2/h2 gens (it ignores these blocks)
 * - work with sorters (haven't tested)
 * - work with unconnected inventories (i mean of course it wouldn't work)
 * 
 * instructions:
 * * ensure that your inventories are connected and not full
 * * add the script to a programming block on the same grid as the inventories you would like to 'manage'
 * * in the custom data of the blocks, add the relevant tags for items that you'd like whitelisted (each on a new line)
 *     (note: the tags are case (and space) sensitive! change the custom data in the programming block and recompile if you don't like them)
 * * sit back, enjoy the sorting, and report back if you encounter any bugs
 * 
 * extra notes:
 *   by default, the tags for the blocks are what you see when you hover over them, except fully lowercase
 *   you can use groups (check the top of the programming block's custom data for the names),
 *     but you can also create 'custom' groups by adding all the relevant tags to the custom data of an inventory
 *   i would not recommend using this on servers or large inventory systems (feel free to let me know it fares though)
 *   i can probably tweak or add functionality (at my discretion) if people actually use this, so let me know if you have any suggestions
 */
class É{public string Ê{get;}public string Ë{get;}public string Ì{get;set;}public MyFixedPoint Í{get;set;}public
MyFixedPoint Î{get;set;}public List<IMyTerminalBlock>Ï{get;}=new List<IMyTerminalBlock>();public É(string Ð,string Ñ){Ê=Ð;Ë=Ñ;}}
class Ò{Program I;MyIni Ó=new MyIni();public Dictionary<string,É>Ô{get;}=new Dictionary<string,É>{["Ore"]=new É("general",
"MyObjectBuilder_Ore"),["Ingot"]=new É("general","MyObjectBuilder_Ingot"),["Component"]=new É("general","MyObjectBuilder_Component"),["Ammo"]
=new É("general","MyObjectBuilder_AmmoMagazine"),["Gun"]=new É("general","MyObjectBuilder_PhysicalGunObject"),[
"Consumable"]=new É("general","MyObjectBuilder_ConsumableItem"),["Gas Container"]=new É("general",
"MyObjectBuilder_GasContainerObject"),["Oxygen Container"]=new É("general","MyObjectBuilder_OxygenContainerObject"),["Datapad"]=new É("general",
"MyObjectBuilder_Datapad"),["Package"]=new É("general","MyObjectBuilder_Package"),["Physical Object"]=new É("general",
"MyObjectBuilder_PhysicalObject"),["Stone"]=new É("ores","MyObjectBuilder_Ore/Stone"),["Scrap Metal"]=new É("ores","MyObjectBuilder_Ore/Scrap"),["Ice"]=
new É("ores","MyObjectBuilder_Ore/Ice"),["Iron Ore"]=new É("ores","MyObjectBuilder_Ore/Iron"),["Nickel Ore"]=new É("ores",
"MyObjectBuilder_Ore/Nickel"),["Silicon Ore"]=new É("ores","MyObjectBuilder_Ore/Silicon"),["Cobalt Ore"]=new É("ores","MyObjectBuilder_Ore/Cobalt"),
["Magnesium Ore"]=new É("ores","MyObjectBuilder_Ore/Magnesium"),["Silver Ore"]=new É("ores","MyObjectBuilder_Ore/Silver")
,["Gold Ore"]=new É("ores","MyObjectBuilder_Ore/Gold"),["Uranium Ore"]=new É("ores","MyObjectBuilder_Ore/Uranium"),[
"Platinum Ore"]=new É("ores","MyObjectBuilder_Ore/Platinum"),["Organic"]=new É("ores","MyObjectBuilder_Ore/Organic"),["Gravel"]=new É(
"ingots","MyObjectBuilder_Ingot/Stone"),["Iron Ingot"]=new É("ingots","MyObjectBuilder_Ingot/Iron"),["Nickel Ingot"]=new É(
"ingots","MyObjectBuilder_Ingot/Nickel"),["Silicon Wafer"]=new É("ingots","MyObjectBuilder_Ingot/Silicon"),["Cobalt Ingot"]=new
É("ingots","MyObjectBuilder_Ingot/Cobalt"),["Magnesium Powder"]=new É("ingots","MyObjectBuilder_Ingot/Magnesium"),[
"Silver Ingot"]=new É("ingots","MyObjectBuilder_Ingot/Silver"),["Gold Ingot"]=new É("ingots","MyObjectBuilder_Ingot/Gold"),[
"Uranium Ingot"]=new É("ingots","MyObjectBuilder_Ingot/Uranium"),["Platinum Ingot"]=new É("ingots","MyObjectBuilder_Ingot/Platinum"),[
"Old Scrap Metal"]=new É("ingots","MyObjectBuilder_Ingot/Scrap"),["Bulletproof Glass"]=new É("components",
"MyObjectBuilder_Component/BulletproofGlass"),["Canvas"]=new É("components","MyObjectBuilder_Component/Canvas"),["Computer"]=new É("components",
"MyObjectBuilder_Component/Computer"),["Construction Comp."]=new É("components","MyObjectBuilder_Component/Construction"),["Detector"]=new É("components",
"MyObjectBuilder_Component/Detector"),["Display"]=new É("components","MyObjectBuilder_Component/Display"),["Explosives"]=new É("components",
"MyObjectBuilder_Component/Explosives"),["Girder"]=new É("components","MyObjectBuilder_Component/Girder"),["Gravity Generator"]=new É("components",
"MyObjectBuilder_Component/GravityGenerator"),["Interior Plate"]=new É("components","MyObjectBuilder_Component/InteriorPlate"),["Large Tube"]=new É("components",
"MyObjectBuilder_Component/LargeTube"),["Medical Comp."]=new É("components","MyObjectBuilder_Component/Medical"),["Metal Grid"]=new É("components",
"MyObjectBuilder_Component/MetalGrid"),["Motor"]=new É("components","MyObjectBuilder_Component/Motor"),["Power Cell"]=new É("components",
"MyObjectBuilder_Component/PowerCell"),["Radio-comm Comp."]=new É("components","MyObjectBuilder_Component/RadioCommunication"),["Reactor Comp."]=new É(
"components","MyObjectBuilder_Component/Reactor"),["Small Tube"]=new É("components","MyObjectBuilder_Component/SmallTube"),[
"Solar Cell"]=new É("components","MyObjectBuilder_Component/SolarCell"),["Steel Plate"]=new É("components",
"MyObjectBuilder_Component/SteelPlate"),["Superconductor"]=new É("components","MyObjectBuilder_Component/Superconductor"),["Thruster Comp."]=new É(
"components","MyObjectBuilder_Component/Thrust"),["Zone Chip"]=new É("components","MyObjectBuilder_Component/ZoneChip"),[
"200mm missile"]=new É("ammo","MyObjectBuilder_AmmoMagazine/Missile200mm"),["25x184mm NATO"]=new É("ammo",
"MyObjectBuilder_AmmoMagazine/NATO_25x184mm"),["5.56x45mm NATO"]=new É("ammo","MyObjectBuilder_AmmoMagazine/NATO_5p56x45mm"),["Welder"]=new É("guns",
"MyObjectBuilder_PhysicalGunObject/WelderItem"),["Enhanced Welder"]=new É("guns","MyObjectBuilder_PhysicalGunObject/Welder2Item"),["Proficient Welder"]=new É("guns",
"MyObjectBuilder_PhysicalGunObject/Welder3Item"),["Elite Welder"]=new É("guns","MyObjectBuilder_PhysicalGunObject/Welder4Item"),["Grinder"]=new É("guns",
"MyObjectBuilder_PhysicalGunObject/AngleGrinderItem"),["Enhanced Grinder"]=new É("guns","MyObjectBuilder_PhysicalGunObject/AngleGrinder2Item"),["Proficient Grinder"]=new É(
"guns","MyObjectBuilder_PhysicalGunObject/AngleGrinder3Item"),["Elite Grinder"]=new É("guns",
"MyObjectBuilder_PhysicalGunObject/AngleGrinder4Item"),["Hand Drill"]=new É("guns","MyObjectBuilder_PhysicalGunObject/HandDrillItem"),["Enhanced Hand Drill"]=new É("guns",
"MyObjectBuilder_PhysicalGunObject/HandDrill2Item"),["Proficient Hand Drill"]=new É("guns","MyObjectBuilder_PhysicalGunObject/HandDrill3Item"),["Elite Hand Drill"]=new É(
"guns","MyObjectBuilder_PhysicalGunObject/HandDrill4Item"),["Automatic Rifle"]=new É("guns",
"MyObjectBuilder_PhysicalGunObject/AutomaticRifleItem"),["Elite Automatic Rifle"]=new É("guns","MyObjectBuilder_PhysicalGunObject/UltimateAutomaticRifleItem"),[
"Precise Automatic Rifle"]=new É("guns","MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem"),["Rapid-Fire Automatic Rifle"]=new É(
"guns","MyObjectBuilder_PhysicalGunObject/RapidFireAutomaticRifleItem"),["Clang Cola"]=new É("consumables",
"MyObjectBuilder_ConsumableItem/ClangCola"),["Cosmic Coffee"]=new É("consumables","MyObjectBuilder_ConsumableItem/CosmicCoffee"),["Medkit"]=new É("consumables",
"MyObjectBuilder_ConsumableItem/Medkit"),["Powerkit"]=new É("consumables","MyObjectBuilder_ConsumableItem/Powerkit"),["Hydrogen Bottle"]=new É("general",
"MyObjectBuilder_GasContainerObject/HydrogenBottle"),["Oxygen Bottle"]=new É("general","MyObjectBuilder_OxygenContainerObject/OxygenBottle"),["Package Item"]=new É(
"general","MyObjectBuilder_Package/Package"),["Datapad Item"]=new É("datapad","MyObjectBuilder_Datapad/Datapad"),["Space Credit"]
=new É("physical object","MyObjectBuilder_PhysicalObject/SpaceCredit")};public Dictionary<string,É>Õ{get;}=new Dictionary
<string,É>();List<IMyTerminalBlock>È=new List<IMyTerminalBlock>();public MyFixedPoint Ç{get;set;}public MyFixedPoint Æ{
get;set;}Dictionary<string,string>Å=new Dictionary<string,string>();Dictionary<string,string>Ä=new Dictionary<string,string
>();Dictionary<long,HashSet<string>>Ã=new Dictionary<long,HashSet<string>>();List<IMyTerminalBlock>Â=new List<
IMyTerminalBlock>();List<MyInventoryItem>Á=new List<MyInventoryItem>();char[]À={'\n'};public Ò(Program I){this.I=I;º();Ö();foreach(
string w in Ô.Keys){É x=Ô[w];Å[x.Ë]=w;Ä[x.Ì]=w;}}public void º(){if(Ó.TryParse(I.Me.CustomData)){foreach(string w in Ô.Keys){
string µ=w.ToLower().Replace(' ','_');Ô[w].Ì=Ó.Get(Ô[w].Ê,$"{µ}_tag").ToString($"{w.ToLower()}");}}else{foreach(string w in Ô.
Keys)Ô[w].Ì=$"{w.ToLower()}";}}public void Ö(){foreach(string w in Ô.Keys){string µ=w.ToLower().Replace(' ','_');Ó.Set(Ô[w].
Ê,$"{µ}_tag",Ô[w].Ì);}Ó.SetSectionComment("general","tag must be included in block name\n"+
"untagged inventories will be used for misc. storage\n"+"recompile to update tags");I.Me.CustomData=Ó.ToString();}public void á(){ä();Â.Clear();I.GridTerminalSystem.
GetBlocksOfType(Â,â=>ã(â));foreach(IMyTerminalBlock R in Â){if(!Ã.ContainsKey(R.EntityId))Ã[R.EntityId]=new HashSet<string>();if(!æ(R))
{È.Add(R);IMyInventory à=R.GetInventory();Ç+=à.CurrentVolume;Æ+=à.MaxVolume;}}}private bool ã(IMyTerminalBlock R){return
R.HasInventory&&!(R is IMyProductionBlock||R is IMyPowerProducer||R is IMyGasGenerator);}private void ä(){Â.Clear();
foreach(É x in Õ.Values){x.Í=0;x.Î=0;x.Ï.Clear();}Õ.Clear();È.Clear();Ç=MyFixedPoint.Zero;Æ=MyFixedPoint.Zero;foreach(HashSet<
string>å in Ã.Values)å.Clear();}private bool æ(IMyTerminalBlock R){bool ç=false;string[]è=R.CustomData.Split(À);foreach(string
é in è){string Ú;if(Ä.TryGetValue(é,out Ú)){É x=Ô[Ú];Õ[Ú]=x;x.Ï.Add(R);IMyInventory à=R.GetInventory();x.Í+=à.
CurrentVolume;x.Î+=à.MaxVolume;Ã[R.EntityId].Add(Ú);ç=true;}}return ç;}public void ß(){foreach(IMyTerminalBlock R in Â){Á.Clear();R.
GetInventory().GetItems(Á);foreach(MyInventoryItem Q in Á)Þ(R,Q);}}private void Þ(IMyTerminalBlock Ý,MyInventoryItem Q){HashSet<
string>Ü=Ã[Ý.EntityId];string Û=Q.Type.ToString();string Ñ=Q.Type.TypeId;string Ú;if(Å.TryGetValue(Û,out Ú)){if(Ü.Contains(Ú))
return;if(Ô[Ú].Ï.Count>0){if(Ù(Ý.GetInventory(),Ô[Ú],Q))return;}}if(Å.TryGetValue(Ñ,out Ú)){if(Ü.Contains(Ú))return;if(Ô[Ú].Ï.
Count>0){if(Ù(Ý.GetInventory(),Ô[Ú],Q))return;}}if(Ü.Count>0)ª(Ý.GetInventory(),Q);}private bool Ù(IMyInventory A,É Ø,
MyInventoryItem Q){foreach(IMyTerminalBlock R in Ø.Ï){IMyInventory S=R.GetInventory();if(A.CanTransferItemTo(S,Q.Type)&&!S.IsFull){if(A
.TransferItemTo(S,Q))return true;}}return false;}private void ª(IMyInventory A,MyInventoryItem Q){foreach(
IMyTerminalBlock R in È){IMyInventory S=R.GetInventory();if(A.CanTransferItemTo(S,Q.Type)&&!S.IsFull){if(A.TransferItemTo(S,Q))return;}}
}}class T{Color U=Color.LightGreen;Color V=Color.Black;public IMyTextSurface W{get;}public RectangleF a{get;}=new
RectangleF();public T(IMyTextSurface X,RectangleF F){W=X;a=F;Y();}private void Y(){W.ContentType=ContentType.SCRIPT;W.Script=
string.Empty;W.ScriptForegroundColor=U;W.ScriptBackgroundColor=V;}}class Z{Program I;Ò H;T O;int N=8;int M=5;Vector2 L=new
Vector2();MySprite K;List<IMyTerminalBlock>J=new List<IMyTerminalBlock>();public Z(Program I,Ò H){this.I=I;this.H=H;G();}public
void G(){IMyTextSurface C=I.Me.GetSurface(0);RectangleF F=new RectangleF((C.TextureSize-C.SurfaceSize)/2f,C.SurfaceSize);O=
new T(C,F);}public void E(){D(O);}private void D(T C){MySpriteDrawFrame B=C.W.DrawFrame();RectangleF F=C.a;Color c=C.W.
ScriptForegroundColor;L=F.Center;L.Y-=20;K=new MySprite(){Type=SpriteType.TEXT,Data=$"last run: {I.i}ms ({I.g})"+
$"\nlast average: {I.h}ms ({I.k} runs)",Position=L,RotationOrScale=0.6f,Color=c,Alignment=TextAlignment.CENTER,FontId="Monospace"};B.Add(K);B.Dispose();}
private void t(T C){MySpriteDrawFrame B=C.W.DrawFrame();RectangleF F=C.a;float p=F.Size.X/(M*2);float q=F.Size.X/M;float r=F.
Size.Y/(N*2);float s=F.Size.Y/N;double u;int z;int v=0;foreach(string w in H.Õ.Keys){É x=H.Õ[w];L.X=F.X+p+(v/N)*q;L.Y=F.Y+r+
(v%N)*s;u=(double)x.Í.RawValue/x.Î.RawValue;z=(int)Math.Ceiling(u*9);y(ref B,z,w);v++;}L.X=F.X+p+(v/N)*q;L.Y=F.Y+r+(v%N)*
s;u=(double)H.Ç.RawValue/H.Æ.RawValue;z=(int)Math.Ceiling(u*9);y(ref B,z,"Other");B.Dispose();}private void y(ref
MySpriteDrawFrame B,int z,string w){Color c=z==10?Color.Yellow:Color.White;string o=$"[{new String('|',z)}{new String('-',10-z)}]";L.Y-=
12;K=new MySprite(){Type=SpriteType.TEXT,Data=$"{o}",Position=L,RotationOrScale=0.4f,Color=c,Alignment=TextAlignment.
CENTER,FontId="Monospace"};B.Add(K);L.Y+=12;K=new MySprite(){Type=SpriteType.TEXT,Data=$"{w}",Position=L,RotationOrScale=0.4f,
Color=Color.White,Alignment=TextAlignment.CENTER,FontId="Monospace"};B.Add(K);}}IEnumerator<bool>n;Ò H;Z m;public int l{get;
set;}=0;public int k{get;}=10;public double j{get;set;}=0;public double i{get;set;}public double h{get;set;}public string g
{get;set;}Program(){H=new Ò(this);m=new Z(this,H);Runtime.UpdateFrequency=UpdateFrequency.Update100;n=d();}void Save(){H.
º();H.Ö();}void Main(string f,UpdateType e){if((e&UpdateType.Update100)!=0){if(!n.MoveNext()){n.Dispose();n=d();}m.E();P(
);}}void P(){i=Runtime.LastRunTimeMs;j+=i;l++;if(l%k==0){h=j/k;l=0;j=0;}}IEnumerator<bool>d(){g="sorting";H.á();m.G();
yield return true;g="updating blocks";H.ß();}