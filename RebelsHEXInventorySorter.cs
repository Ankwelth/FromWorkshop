/*
 * HEX Inventory Sorter by Neo.uc
 * A simple TAG-based inventory sorter without much bling bling
 * 
 * Available Tags are:
 * [ORE]  [COMP] [ING]  [ICE]  [BOTTLE]  [TOOLS]  [GUNS]  [AMMO]
 * [MOD_ORE]  [MOD_COMP]  [MOD_ING]  [FOOD]  [MOD_AMMO]  [PAD]  [$$]
 * 
 * For Question or Bug Reports visit Rebels-Games Discord Server
 * or contant me @ Neo.uc#8471
 */

public Program()
{
    /* Begin Config Section*/

    Config.Read_Tag_from_CustomData = false;    // True - read [TAG] from CustomData instead of block's name

    Config.Sort_Cargo = true;
    Config.Sort_Connector = true;
    Config.Sort_Grinder = true;
    Config.Sort_Welder = false;
    Config.Sort_Cockpit = true;
    Config.Sort_Medicals = true;

    Config.Clean_Assembler_Output = true;
    Config.Clean_Assembler_InputOverflow = true;
    Config.Clean_Refinery_Output = true;

    Config.Clean_Connected_Grids = false;

    // 6 Ticks/Minute * 60 Seconds * 5 = 5 Minutes
    Config.Sleep_Ticks_After_Cleaning = 6 * 60 * 5;

    /* End Config Section*/

    v=new W(this);Me.CustomData="";N.Z();Runtime.UpdateFrequency=UpdateFrequency.Update10;}IEnumerator<bool>z=null;List<
IMyTerminalBlock>y=new List<IMyTerminalBlock>();List<IMyTerminalBlock>x=new List<IMyTerminalBlock>();List<MyInventoryItem>w=new List<
MyInventoryItem>();W v;bool u=false;void Main(string s,UpdateType r){if(z==null)z=q();if(!z.MoveNext()){z.Dispose();z=q();}}IEnumerator
<bool>q(){x.Clear();y.Clear();v.P("\n\nFetching blocks");if(!Config.Clean_Connected_Grids){if(Config.Sort_Cargo){
GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(y,A=>A.HasInventory&&!A.BlockDefinition.SubtypeId.Contains("Locker")&&!A.
BlockDefinition.SubtypeId.Contains("Rack")&&A.IsSameConstructAs(Me));x.AddRange(y);for(int p=0;p<5;p++)yield return true;}if(Config.
Sort_Connector){GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(y,A=>A.IsSameConstructAs(Me));x.AddRange(y);for(int p=0;p<5;p++)
yield return true;}if(Config.Sort_Grinder){GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(y,A=>A.IsSameConstructAs(Me));x
.AddRange(y);for(int p=0;p<5;p++)yield return true;}if(Config.Sort_Welder){GridTerminalSystem.GetBlocksOfType<
IMyShipWelder>(y,A=>A.IsSameConstructAs(Me));x.AddRange(y);for(int p=0;p<5;p++)yield return true;}if(Config.Sort_Cockpit){
GridTerminalSystem.GetBlocksOfType<IMyCockpit>(y,A=>A.HasInventory&&A.IsSameConstructAs(Me));x.AddRange(y);for(int p=0;p<5;p++)yield
return true;}if(Config.Sort_Medicals){GridTerminalSystem.GetBlocksOfType<IMyMedicalRoom>(y,A=>A.HasInventory&&A.
IsSameConstructAs(Me));x.AddRange(y);for(int p=0;p<5;p++)yield return true;}}else{if(Config.Sort_Cargo){GridTerminalSystem.
GetBlocksOfType<IMyCargoContainer>(y,A=>A.HasInventory&&!A.BlockDefinition.SubtypeId.Contains("Locker")&&!A.BlockDefinition.SubtypeId.
Contains("Rack"));x.AddRange(y);for(int p=0;p<5;p++)yield return true;}if(Config.Sort_Connector){GridTerminalSystem.
GetBlocksOfType<IMyShipConnector>(y);x.AddRange(y);for(int p=0;p<5;p++)yield return true;}if(Config.Sort_Grinder){GridTerminalSystem.
GetBlocksOfType<IMyShipGrinder>(y);x.AddRange(y);for(int p=0;p<5;p++)yield return true;}if(Config.Sort_Welder){GridTerminalSystem.
GetBlocksOfType<IMyShipWelder>(y);x.AddRange(y);for(int p=0;p<5;p++)yield return true;}if(Config.Sort_Cockpit){GridTerminalSystem.
GetBlocksOfType<IMyCockpit>(y,A=>A.HasInventory);x.AddRange(y);for(int p=0;p<5;p++)yield return true;}if(Config.Sort_Medicals){
GridTerminalSystem.GetBlocksOfType<IMyMedicalRoom>(y,A=>A.HasInventory);x.AddRange(y);for(int p=0;p<5;p++)yield return true;}}v.P(
"\n\nRead [TAGs]");N.d();foreach(var À in x){foreach(string j in N.c){if(Config.Read_Tag_from_CustomData){if(À.CustomData!=null||!string.
IsNullOrEmpty(À.CustomData))if(À.CustomData.Contains(j))N.k(j,À.GetInventory());}else{if(À.CustomName.Contains(j))N.k(j,À.
GetInventory());}}}for(int p=0;p<5;p++)yield return true;N.g();for(int p=0;p<5;p++)yield return true;v.P("\n\nSorting...");foreach(
var À in x){if(u)Me.CustomData+=$"Start sorting '{À.CustomName}'\n";if(!À.IsFunctional){if(u)Me.CustomData+=
$"Stop sorting '{À.CustomName} (not functional)'\n";continue;}if(Config.Read_Tag_from_CustomData){if(À.CustomData.Contains("[IGNORE]"))continue;}else{if(À.CustomName.
Contains("[IGNORE]"))continue;}IMyInventory J=À.GetInventory();if(J==null)continue;if(J.ItemCount==0){if(u)Me.CustomData+=
$"Stop sorting '{À.CustomName} (Empty)'\n";continue;}w.Clear();J.GetItems(w);if(u)Me.CustomData+=$"Found {w.Count} items\n";foreach(var I in w){if(u)Me.CustomData
+=$"Start sorting item {I.Type} | ";bool Á=K(J,I);if(Á){if(u)Me.CustomData+=$"Sort '{I.Type.ToString()}' successfull\n";}
else{if(u)Me.CustomData+=$"Sort '{I.Type}' failed\n";continue;}yield return true;}}if(Config.Clean_Assembler_Output){v.P(
"\n\nCleaning assemblers");y.Clear();if(Config.Clean_Connected_Grids){GridTerminalSystem.GetBlocksOfType<IMyAssembler>(y);}else{
GridTerminalSystem.GetBlocksOfType<IMyAssembler>(y,A=>A.IsSameConstructAs(Me));}for(int p=0;p<5;p++)yield return true;foreach(var À in y){
if(!À.IsFunctional)continue;IMyInventory º;if(Config.Clean_Assembler_InputOverflow){º=((IMyAssembler)À).InputInventory;if(
(double)º.VolumeFillFactor>0.95f){µ(º);yield return true;}}º=((IMyAssembler)À).OutputInventory;if(º.ItemCount==0)continue
;µ(º);yield return true;}}if(Config.Clean_Refinery_Output){v.P("\n\nCleaning refineries");y.Clear();if(Config.
Clean_Connected_Grids){GridTerminalSystem.GetBlocksOfType<IMyRefinery>(y);}else{GridTerminalSystem.GetBlocksOfType<IMyRefinery>(y,A=>A.
IsSameConstructAs(Me));}for(int p=0;p<5;p++)yield return true;foreach(var À in y){if(!À.IsFunctional)continue;IMyInventory º=((
IMyRefinery)À).OutputInventory;if(º.ItemCount==0)continue;µ(º);yield return true;}}for(int p=Config.Sleep_Ticks_After_Cleaning;p>0;
p--){if(p%6==0)v.P($"\n\nSleeping\n< {(int)p/6} sec >");yield return true;}}void µ(IMyInventory J){w.Clear();J.GetItems(w
);foreach(var I in w){K(J,I);}}List<IMyTerminalBlock>ª(bool o=false){List<IMyTerminalBlock>n=new List<IMyTerminalBlock>()
;GridTerminalSystem.GetBlocks(n);if(o)return(List<IMyTerminalBlock>)n.Where(A=>A.BlockDefinition.TypeIdString.Equals(
"MyObjectBuilder_SurvivalKit"));return(List<IMyTerminalBlock>)n.Where(A=>A.IsSameConstructAs(Me)&&A.BlockDefinition.TypeIdString.Equals(
"MyObjectBuilder_SurvivalKit"));}bool K(IMyInventory J,MyInventoryItem I){IMyCubeBlock H=J.Owner as IMyCubeBlock;if(!N.m.ContainsKey(I.Type)){if(u)Me
.CustomData+=$"{I.Type.ToString()} not found in Data.ItemDefinitions\n";return false;}List<IMyInventory>G=N.m[I.Type];if(
G==null){if(u)Me.CustomData+=$"{I.Type.ToString()} has no destination inventories\n";return false;}if(G.Find(F=>F.Owner.
EntityId==J.Owner.EntityId)!=null)return true;var E=I.Type.GetItemInfo().Volume;foreach(var D in G){if(D.IsFull)continue;
IMyCubeBlock C=D.Owner as IMyCubeBlock;if(!J.IsConnectedTo(D)){if(u)Me.CustomData+=
$"Error: '{H.DisplayNameText}' is not connected to '{C}'\n";continue;}if(!J.CanTransferItemTo(D,I.Type)){if(u)Me.CustomData+=
$"Error: '{I.Type.ToString()}' can't be moved into '{C}'\n";continue;}var B=1000*I.Amount*E;var L=(D.MaxVolume-D.CurrentVolume)*1000;J.TransferItemTo(D,I);if(B<=L)return true;}if(
u)Me.CustomData+=$"Warning: '{I.Type.ToString()}' was not moved completly - all inventories full?'\n";return false;}
public static class Config
{
    public static bool Read_Tag_from_CustomData;
    public static bool Sort_Cargo;
    public static bool Sort_Connector;
    public static bool Sort_Grinder;
    public static bool Sort_Welder;
    public static bool Sort_Cockpit;
    public static bool Sort_Medicals;

    public static bool Clean_Refinery_Output;
    public static bool Clean_Assembler_Output;
    public static bool Clean_Assembler_InputOverflow;

    public static bool Clean_Connected_Grids;

    public static int Sleep_Ticks_After_Cleaning;
 }
static class N{public static void Z(){Y.Clear();Y.Add("[ORE]",new List<string>{"Ore/Cobalt","Ore/Stone","Ore/Iron",
"Ore/Nickel","Ore/Gold","Ore/Magnesium","Ore/Uranium","Ore/Silver","Ore/Silicon","Ore/Platinum","Ore/Scrap"});Y.Add("[ICE]",new List
<string>{"Ore/Ice"});Y.Add("[ING]",new List<string>{"Ingot/Cobalt","Ingot/Stone","Ingot/Iron","Ingot/Nickel","Ingot/Gold"
,"Ingot/Magnesium","Ingot/Scrap","Ingot/Platinum","Ingot/Silicon","Ingot/Silver","Ingot/Uranium","Ingot/PrototechScrap",}
);Y.Add("[COMP]",new List<string>{"Component/Thrust","Component/Superconductor","Component/SteelPlate",
"Component/SolarCell","Component/SmallTube","Component/Reactor","Component/RadioCommunication","Component/PowerCell","Component/Motor",
"Component/MetalGrid","Component/Medical","Component/LargeTube","Component/InteriorPlate","Component/GravityGenerator","Component/Girder",
"Component/Explosives","Component/Display","Component/Detector","Component/Construction","Component/Computer","Component/Canvas",
"Component/BulletproofGlass","Component/CarbonPlate","Component/PrototechCoolingUnit","Component/PrototechFrame","Component/PrototechPanel",
"Component/PrototechCapacitor","Component/PrototechPropulsionUnit","Component/PrototechMachinery","Component/PrototechCircuitry",});Y.Add("[BOTTLE]",
new List<string>{"OxygenContainerObject/OxygenBottle","GasContainerObject/HydrogenBottle",
"OxygenContainerObject/OxygenBottleADV","OxygenContainerObject/OxygenBottleMK","GasContainerObject/HydrogenBottleADV","GasContainerObject/HydrogenBottleMK"});Y
.Add("[TOOLS]",new List<string>{"PhysicalGunObject/AngleGrinder4Item","PhysicalGunObject/HandDrill4Item",
"PhysicalGunObject/Welder4Item","PhysicalGunObject/AngleGrinder2Item","PhysicalGunObject/HandDrill2Item","PhysicalGunObject/Welder2Item",
"PhysicalGunObject/AngleGrinderItem","PhysicalGunObject/HandDrillItem","PhysicalGunObject/HandDrill3Item","PhysicalGunObject/AngleGrinder3Item",
"PhysicalGunObject/Welder3Item","PhysicalGunObject/WelderItem","PhysicalGunObject/AngleGrinder5Item","PhysicalGunObject/Welder5Item",
"PhysicalGunObject/HandDrill5Item"});Y.Add("[GUNS]",new List<string>{"PhysicalGunObject/AutomaticRifleItem","PhysicalGunObject/UltimateAutomaticRifleItem"
,"PhysicalGunObject/RapidFireAutomaticRifleItem","PhysicalGunObject/PreciseAutomaticRifleItem",
"PhysicalGunObject/AdvancedHandHeldLauncherItem","PhysicalGunObject/BasicHandHeldLauncherItem","PhysicalGunObject/SemiAutoPistolItem",
"PhysicalGunObject/ElitePistolItem","PhysicalGunObject/FullAutoPistolItem","PhysicalGunObject/FlareGunItem"});Y.Add("[AMMO]",new List<string>{
"AmmoMagazine/NATO_5p56x45m","AmmoMagazine/LargeCalibreAmmo","AmmoMagazine/MediumCalibreAmmo","AmmoMagazine/AutocannonClip",
"AmmoMagazine/NATO_25x184mm","AmmoMagazine/LargeRailgunAmmo","AmmoMagazine/Missile200mm","AmmoMagazine/AutomaticRifleGun_Mag_20rd",
"AmmoMagazine/UltimateAutomaticRifleGun_Mag_30rd","AmmoMagazine/RapidFireAutomaticRifleGun_Mag_50rd","AmmoMagazine/PreciseAutomaticRifleGun_Mag_5rd",
"AmmoMagazine/SemiAutoPistolMagazine","AmmoMagazine/ElitePistolMagazine","AmmoMagazine/FullAutoPistolMagazine","AmmoMagazine/SmallRailgunAmmo",
"MyObjectBuilder_AmmoMagazine/FlareClip","AmmoMagazine/FireworksBoxBlue","AmmoMagazine/FireworksBoxGreen","AmmoMagazine/FireworksBoxPink",
"AmmoMagazine/FireworksBoxRainbow","AmmoMagazine/FireworksBoxRed","AmmoMagazine/FireworksBoxYellow"});Y.Add("[MOD_ORE]",new List<string>{
"Ore/Coal High Quality","Ore/Coal Low Quality","Ore/Coal Medium Quality","Ore/Cobalt High Quality","Ore/Cobalt Low Quality",
"Ore/Cobalt Medium Quality","Ore/Gold High Quality","Ore/Gold Low Quality","Ore/Gold Medium Quality","Ore/HighElementZero","Ore/LowElementZero",
"Ore/Organic","Ore/Hexan","Ore/VolcanicRock","Ore/Iron High Quality","Ore/Iron Low Quality","Ore/Iron Medium Quality",
"Ore/Lateryt High Quality","Ore/Lateryt Low Quality","Ore/Lateryt Medium Quality","Ore/Magnesium High Quality","Ore/Magnesium Low Quality",
"Ore/Magnesium Medium Quality","Ore/Malachit High Quality","Ore/Malachit Low Quality","Ore/Malachit Medium Quality","Ore/Nickel High Quality",
"Ore/Nickel Low Quality","Ore/Nickel Medium Quality","Ore/Osmium High Quality","Ore/Osmium Low Quality","Ore/Osmium Medium Quality",
"Ore/Osmium  Low  Quality","Ore/Palladium High Quality","Ore/Palladium Low Quality","Ore/Palladium Medium Quality","Ore/∑UnknownMaterial",
"Ore/∏UnknownMaterial","Ore/∇UnknownMaterial","Ore/⌊UnknownMaterial","Ore/Platinum High Quality","Ore/Platinum Low Quality",
"Ore/Platinum Medium Quality","Ore/Uranium High Quality","Ore/Uranium Low Quality","Ore/Uranium Medium Quality","Ore/Silicon High Quality",
"Ore/Silicon Low Quality","Ore/Silicon Medium Quality","Ore/Silver High Quality","Ore/Silver Low Quality","Ore/Silver Medium Quality",
"Ore/Rubidium High Quality","Ore/Rubidium Low Quality","Ore/Rubidium Medium Quality","Ore/Volcanic Rock"});Y.Add("[MOD_COMP]",new List<string>{
"Component/PalladiumComponent","Component/Stabilizer","Component/VolumeExtender","Component/WeaponComponent","Component/ToolComponent",
"Component/PalladiumComponentBox","Component/Coolant","Component/AdvancedComponent","Component/AdvancedProcessor","Component/AdvancedTransmitter",
"Component/CoolantBox","Component/GridCoreComponentTier1","Component/GridCoreComponentTier2","Component/GridCoreComponentTier3",
"Component/NewStartshipEasySpace","Component/InteriorPlateBox","Component/Robotics","Component/PowerCore","Component/PowerCoreBox","Component/Screw",
"Component/ScrewBox","Component/StabilizerBox","Component/StartshipEasySpace","Component/StartshipEasySpaceMoon","Component/SteelPlateBox",
"Component/Pumpkin","Component/Skull","Package/Package","Component/ControlChip","Component/Cerambet","Component/ShieldComponent",
"Component/TitaniumPlate","Component/DiamondPlate"});Y.Add("[MOD_ING]",new List<string>{"Ingot/OsmiumIngot","Ingot/Rad","Ingot/PalladiumIngot",
"Ingot/RubidiumIngot","Ingot/Kaliforn","Ingot/CopperIngot","Ingot/AluminumIngot","Ingot/Cez","Ingot/CoalPowder","Ingot/Diamond",
"Ingot/HexanIngot","Ingot/Isotop-5",});Y.Add("[FOOD]",new List<string>{"Ore/TomatoSeeds","ConsumableItem/StimPack",
"ConsumableItem/AlienCake","ConsumableItem/AlienFruit","Ore/AlienFruitSeeds","ConsumableItem/AlienJuice","ConsumableItem/AlienSmoothie",
"ConsumableItem/BerryCookies","ConsumableItem/BerryJuice","ConsumableItem/BrownBerry","Ore/BrownBerrySeeds","ConsumableItem/Carrot",
"ConsumableItem/CarrotCocktail","ConsumableItem/CarrotJuice","ConsumableItem/ChiliConCarne","ConsumableItem/Chocolate",
"ConsumableItem/ChocolateSmoothie","Ore/CarrotSeeds","ConsumableItem/Corn","Ore/CornSeeds","ConsumableItem/CosmicBar","ConsumableItem/CosmicMix",
"Ore/CosmicPeat","ConsumableItem/CosmicPita","Ore/DragonFruitSeeds","ConsumableItem/DragonFruit","Ore/GlowingMushroomSeeds",
"ConsumableItem/GlowingMushroom","ConsumableItem/Gnocchi","ConsumableItem/DragonCake","ConsumableItem/EmergencyRation","ConsumableItem/Fries",
"ConsumableItem/GalaxySmoothie","ConsumableItem/InterBeer","ConsumableItem/MeatballswithPotato","ConsumableItem/Meat","ConsumableItem/Moonshine",
"ConsumableItem/Mushroom","ConsumableItem/Stew","ConsumableItem/Tomato","ConsumableItem/TomatoJuice","ConsumableItem/TomatoCocktail",
"ConsumableItem/MushroomSoup","ConsumableItem/Pumpkin","Ore/MushroomSeeds","ConsumableItem/Potato","Ore/Peat","ConsumableItem/Pita",
"Ingot/PumpkinSeeds","ConsumableItem/SpaceDinner","ConsumableItem/Spagetti","ConsumableItem/SparklingWater","ConsumableItem/SpiderMeat",
"ConsumableItem/Salad","ConsumableItem/Sandwich","ConsumableItem/PureewithMeat","ConsumableItem/PurifiedWater","ConsumableItem/VegeBurito",
"ConsumableItem/Stolitchnaya","ConsumableItem/RebelBeer","ConsumableItem/WeirdDinner","ConsumableItem/CosmicCoffee","ConsumableItem/Powerkit",
"ConsumableItem/ClangCola","ConsumableItem/Medkit"});Y.Add("[MOD_AMMO]",new List<string>{"AmmoMagazine/URMBossNATO_25x184mm2",
"AmmoMagazine/URMBossMissile200mm","AmmoMagazine/URMMissile200mm","AmmoMagazine/URMNATO_25x184mm2","AmmoMagazine/URMNATO_25x184mm",
"AmmoMagazine/URMBossNATO_25x184mm","AmmoMagazine/Defensive_Ammunition","AmmoMagazine/NATO_5p56x45mm","AmmoMagazine/Energy","AmmoMagazine/LaserDefend",
"AmmoMagazine/AutocannonClip","AmmoMagazine/AmmoMag_AutoCannonBox","AmmoMagazine/AmmoMag_Med_HE_5","AmmoMagazine/AmmoMag_Med_AP_1",
"AmmoMagazine/AmmoMag_Med_HE_1","AmmoMagazine/AmmoMag_Med_AP_5","AmmoMagazine/AmmoMediumBasic_1","AmmoMagazine/AmmoMediumBasic_5",
"AmmoMagazine/AmmoMag_Heavy_HE","AmmoMagazine/AmmoMag_Heavy_APDS","AmmoMagazine/Tactic_Rocket_Basic","AmmoMagazine/LargeCalibreAmmo",
"AmmoMagazine/MediumCalibreAmmo_5","AmmoMagazine/MediumCalibreAmmo","AmmoMagazine/AmmoHeavy_Medium","AmmoMagazine/Tactic_Rocket_HS",
"AmmoMagazine/AmmoMag_Heavy_APHE","AmmoMagazine/Tactic_Rocket_Shitty","AmmoMagazine/Hybrid_Rocket_HE","AmmoMagazine/Hybrid_Rocket_AP",
"AmmoMagazine/Hybrid_Rocket_SG","AmmoMagazine/Hybrid_Rocket_MIRV","AmmoMagazine/Hybrid_Rocket_Smoke","AmmoMagazine/VMLS_MagazineLarge",
"AmmoMagazine/VMLS_MagazineLargeBox","AmmoMagazine/SwarmVMLS_MagazineLarge","AmmoMagazine/DecoySwarmMissle","AmmoMagazine/APModularMissleBox",
"AmmoMagazine/APModularMissle","AmmoMagazine/DecoySwarmMissleBox","AmmoMagazine/SwarmMissle","AmmoMagazine/SwarmMissleBox",
"AmmoMagazine/SGModularMissle","AmmoMagazine/SGModularMissleBox","AmmoMagazine/FuelCapsule","AmmoMagazine/LaserDefend","AmmoMagazine/SpentLarge",
"Component/EmptySmallBox","Component/EmptyShell_Med","Component/SpentMedClip","Component/EmptyRocketBox"});Y.Add("[PAD]",new List<string>{
"Datapad/Datapad"});Y.Add("[$$]",new List<string>{"PhysicalObject/SpaceCredit","Ingot/Credit","Ingot/Rebelium","Ingot/Irium",
"Component/PCUUpgrade","Component/KeyForBankLocker","Component/KeyFragmentsVanilla","Component/KeyFragmentsMK","Component/KeyFragmentsADV",
"Component/KeyVanilla","Component/KeyMK","Component/KeyADV","Component/Token","Component/RewardComponentRare","Component/RewardComponentEpic",
"Component/RewardComponentLegendary","Component/ZoneChip","Component/NewZoneChip","Component/RebelComponent","Component/GoldStar"});Y.Add("[MISC]",new List<
string>{"Component/EngineerPlushie","Component/SabiroidPlushie"});Q();}static void Q(){c.Clear();a.Clear();foreach(var j in Y.
Keys){c.Add(j);foreach(var l in Y[j]){MyItemType f=MyItemType.Parse(l);if(!a.ContainsKey(f))a.Add(f,j);}}}public static void
k(string j,IMyInventory h){if(!X.ContainsKey(j))X[j]=new List<IMyInventory>();X[j].Add(h);}public static void g(){m.Clear
();foreach(MyItemType f in a.Keys){string e=a[f];if(!X.ContainsKey(e))m.Add(f,null);else m.Add(f,X[e]);}}public static
void d(){X.Clear();}public static List<string>c=new List<string>();public static Dictionary<MyItemType,List<IMyInventory>>m=
new Dictionary<MyItemType,List<IMyInventory>>();static Dictionary<MyItemType,string>a=new Dictionary<MyItemType,string>();
static Dictionary<string,List<string>>Y=new Dictionary<string,List<string>>();static Dictionary<string,List<IMyInventory>>X=
new Dictionary<string,List<IMyInventory>>();}class W{IMyTextSurface V;IMyTextSurface U;IMyProgrammableBlock T;string S=
"InventorySorterScript";public W(Program R){T=R.Me;Q();}void Q(){V=T?.GetSurface(0);if(V==null)return;V.ContentType=ContentType.TEXT_AND_IMAGE;
V.ClearImagesFromSelection();V.AddImageToSelection(S,true);V.Alignment=TextAlignment.CENTER;V.FontSize=2.0f;if(T.
BlockDefinition.SubtypeId.EndsWith("Reskin")){U=T.GetSurface(1);U.ContentType=ContentType.TEXT_AND_IMAGE;U.ClearImagesFromSelection();U
.Alignment=TextAlignment.CENTER;U.FontSize=2.0f;}else{U=V;}}public void P(string M,bool O=false){U?.WriteText(M,O);}}