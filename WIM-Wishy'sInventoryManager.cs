/*
 * Instructions:
 * -------------
 * 
 * SETUP:
 * 
 * - Read the little guide below or watch the instructional video
 * - Have enough lcd's ready, I recommend Wide LCD's placed sideways, with the text rotated (-)90 degrees
 * - Put this script in a programmable block
 * - Edit the values below this introduction to match your lcd's, assemblers, groups etc...
 * - Be sure to have your sorting set up decently before using the Autocrafting
 * 
 * 1. Sorting
 * ==========
 * 
 * 	copy one or more category names inside your cargos customData to sort items there
 * 
 * 		Component
 *         Ore
 *         Ingot
 *         AmmoMagazine
 *         PhysicalGunObject
 *         GasContainerObject
 *         OxygenContainerObject
 *         PhysicalObject
 *         ConsumableItem
 * 
 * 	to ignore a container, write "Ignore" in the customData
 * 	use "Stop" on your connector to ignore grids !on the other side!
 * 
 * 	You can sort special ores and components with:
 * 
 * 	Manual
 * 	Iron
 * 	IronIngot
 * 	SteelPlate
 * 	Ice
 * 	....
 * 
 * 	Or put a fixed amount of items in a container:
 * 
 * 	Manual
 * 	Iron=5000
 * 	IronIngot=2000
 * 	SteelPlate=500
 * 	...
 * 
 * 2. LCD DISPLAY
 * ==============
 * 
 * 	Edit your LCD's customData with "WIM" followed by the corresponding categories to display items there
 * 	(each category on a new line)
 * 
 * 	categories:
 * 		Battery
 * 		Wind
 * 		Hydro
 * 		Solar
 * 		Reactor
 * 		Ore
 * 		Ingot
 * 		Component
 * 		Item
 * 
 * 	example:
 * 		"WIM
 * 		Ore
 * 		Ingot"
 * 
 * 		Background color can be changed on the lcd's blocksettings
 * 
 * 3. POWER MANAGEMENT
 * ===================
 * 	
 * 	This can be turned on or off in the Settings
 * 	Settings can be changed in the PB's Custom Data
 * 
 * 		PowerManagement: true/false
 * 
 * 		RatioHigh: Ratio (in %) when to turn Reactors OFF
 * 		RatioLow: Ratio (in %) when to turn Reactors ON
 * 		BatteryGroup: (OPTIONAL) only check batteries from this certain group
 * 		ReactorGroup: Name of the reactor group to toggle on/off
 * 		SkipRecharge: skip checking batteries set to Recharge
 * 
 * 
 * 4. AUTOCRAFTING
 * ===============
 * 
 * 	Asign an LCD with the name given below in the configuration or the Custom Data
 * 	The first time the lcd will populate with all the items you have right now.
 * 	Any new item crafted will be added to the list.
 * 	Items are listed in this format:
 * 	Itemname	Current Stock / Wanted Stock				
 * 	SteelPlate         500 /    600
 * 
 * 	You can edit the wanted value to your desired number
 * 
 * 	Be Sure to set up the name of your assemblers to match the name in the custom data
 * 
 * 
 * 5. AUTO DISASSEMBLE
 * ===================
 * 
 * 	There are 2 different auto disassamble modes
 * 
 * 	1. AutoDisassemble
 * 		This will disassemble ALL items given in the config "UnwantedItems"
 * 		Best used when you don't use DisassembleOverstock, if you do, be sure
 * 		to set the values of Autocrafting to 0/0, or it will assemble and disassemble continuesly!
 * 		
 * 	2. DisassembleOverstock
 * 		Disassembles everything over the Wanted Stock levels
 * 
 * 	Be Sure to set up the name of your disassemblers to match the name in the custom data
 * 
 * 6. REFINERY MANAGEMENT
 * ======================
 * 
 * 	When enabled, the script will look at all the refinery names and its arguments.
 * 
 * 	arguments:  +OreName
 * 				-OreName
 * 				|OreName
 * 
 * 	Examples: 
 * 		Refinery +Iron
 * 			this will only refine iron, nothing else
 * 
 * 		Refinery +Iron +Gold
 * 			this will only refine iron and gold
 * 
 * 		Refinery +All -Stone
 * 			this will refine everything except stone
 * 
 * 
 * 	
 */

// CONFIG START
// =======================================================================================
//
// !!!!!!!!!!!!!!!!!!!!
// Everything below is meant for the first setup, if you require to change something
// after the first setup, edit the values in the Custom Data of the Programmable Block
// !!!!!!!!!!!!!!!!!!!!
//
// Values below are how fast or slow the script take to run every cycle
// Lower values mean faster running time, but higher runtime cost
// If your programmable block overheats too much, be sure to set these values higher
//
// Amount of ticks to wait between instructions
public int waitTicks = 5;
// multiplier for heavier instructions
public int waitTicksMultiplier = 10;

// Does the script needs to assign Cargo categories?
public bool shouldCheckAndAssign = true;
// When does the script needs to assign a new cargo for a category (in %)
public float CargoOverFlowMax = 80f;

// Represents the number of hours to add or subtract to the current servertime (if you live in another timezone)
public int TimeDifference = 1;

// (Optional) Name of LCD to display status. If you don't want LCD set LCD on: false
public bool haveLCD = true;

public string LCDName = "WIM Status";

// (Optional) Name of LCD for Autocrafting. If you don't want Autocrafting set on: false
public bool Autocrafting = false;
public string LCDNameAutocrafting = "WIM Autocrafting";

// (Optional) Automatically dissasemble these items, split items with a ;
// this is best turned off if you use DisassembleOverstock
public bool AutoDisassemble = false;
public string UnwantedItems = "AngleGrinderItem;WelderItem;HandDrillItem";

// (Optional) Automatically dissasemble the overstock of items
public bool DisassembleOverstock = false;

// Name of the Assembler(s) and Disassembler(s),
// will change this to autodetect in a future update
public string Assembler = "WIM Assembler";
public string Disassembler = "WIM Disassembler";

// Size of the font (put an "f" behind, like 0.8f)
public float fontSize = 0.8f;

// Size of the font (put an "f" behind, like 0.8f)
public float debugfontSize = 0.6f;

// Size of the font (put an "f" behind, like 0.8f)
public float autocraftingfontSize = 0.6f;

// Size of the font (put an "f" behind, like 0.8f)
public float wantedfontSize = 1f;

public bool PowerManagement = false;
public float RatioLow = 0.25f;
public float RatioHigh = 0.75f;
public bool SkipRecharge = false;
public string BatteryGroup = "Batteries Base";
public string ReactorGroup = "Reactors Base";

string[] oreTypes = { "Cobalt", "Gold", "Iron", "Magnesium", "Nickel", "Platinum", "Silicon", "Silver", "Stone", "Uranium" };

// Conversion rates from ore to ingot
Dictionary<string, double> conversionRates = new Dictionary<string, double>()
{
    { "Stone", 0.014 },
    { "Scrap Metal", 0.8 },
    { "Iron", 0.7 },
    { "Silicon", 0.7 },
    { "Nickel", 0.4 },
    { "Cobalt", 0.3 },
    { "Silver", 0.1 },
    { "Gold", 0.01 },
    { "Uranium", 0.01 },
    { "Magnesium", 0.007 },
    { "Platinum", 0.005 },
    { "Unknown", 0.8 },
};

// Dictionary to store weapon blocks and their corresponding ammo types
Dictionary<string, string> weaponToAmmoMapping = new Dictionary<string, string>
{
    {"MyObjectBuilder_SmallMissileLauncher/LargeBlockLargeCalibreGun", "MyObjectBuilder_AmmoMagazine/LargeCalibreAmmo"},
    {"MyObjectBuilder_LargeMissileTurret/LargeCalibreTurret", "MyObjectBuilder_AmmoMagazine/LargeCalibreAmmo"},
    {"MyObjectBuilder_SmallMissileLauncherReload/SmallBlockMediumCalibreGun", "MyObjectBuilder_AmmoMagazine/MediumCalibreAmmo"},
    {"MyObjectBuilder_LargeMissileTurret/LargeBlockMediumCalibreTurret", "MyObjectBuilder_AmmoMagazine/MediumCalibreAmmo"},
    {"MyObjectBuilder_LargeMissileTurret/SmallBlockMediumCalibreTurret", "MyObjectBuilder_AmmoMagazine/MediumCalibreAmmo"},
    {"MyObjectBuilder_SmallGatlingGun/SmallBlockAutocannon", "MyObjectBuilder_AmmoMagazine/AutocannonClip"},
    {"MyObjectBuilder_LargeGatlingTurret/AutoCannonTurret", "MyObjectBuilder_AmmoMagazine/AutocannonClip"},
    {"MyObjectBuilder_SmallGatlingGun/", "MyObjectBuilder_AmmoMagazine/NATO_25x184mm"},
    {"MyObjectBuilder_LargeGatlingTurret/", "MyObjectBuilder_AmmoMagazine/NATO_25x184mm"},
    {"MyObjectBuilder_LargeGatlingTurret/SmallGatlingTurret", "MyObjectBuilder_AmmoMagazine/NATO_25x184mm"},
    {"MyObjectBuilder_LargeMissileTurret/", "MyObjectBuilder_AmmoMagazine/Missile200mm"},
    {"MyObjectBuilder_LargeMissileTurret/SmallMissileTurret", "MyObjectBuilder_AmmoMagazine/Missile200mm"},
    {"MyObjectBuilder_SmallMissileLauncherReload/LargeRailgun", "MyObjectBuilder_AmmoMagazine/LargeRailgunAmmo"},
    {"MyObjectBuilder_SmallMissileLauncherReload/SmallRailgun", "MyObjectBuilder_AmmoMagazine/SmallRailgunAmmo"},
    {"MyObjectBuilder_SmallGatlingGun/SmallGatlingGunWarfare2", "MyObjectBuilder_AmmoMagazine/NATO_25x184mm"},
    {"MyObjectBuilder_InteriorTurret/LargeInteriorTurret", "MyObjectBuilder_AmmoMagazine/NATO_5p56x45mm"},
    {"MyObjectBuilder_ConveyorSorter/LargeRailgun", "MyObjectBuilder_AmmoMagazine/LargeRailgunAmmo"},
    {"MyObjectBuilder_ConveyorSorter/SmallRailgun", "MyObjectBuilder_AmmoMagazine/SmallRailgunAmmo"},
    {"MyObjectBuilder_SmallMissileLauncher/LargeMissileLauncher", "MyObjectBuilder_AmmoMagazine/Missile200mm"},
};

// Define the low inventory threshold, adjust as needed
public double lowOreThreshold = 1000;

// CONFIG END
// Don't Touch the settings below unless you know what you're doing!!!
// =======================================================================================

public bool useSprites = true;
public bool Debug = false;
string[]ǡ={".","..","...","...."};int Ǣ=0;string ǣ="v0.177";string Ǥ="WIM Debug";string ǥ="WIM Wanted";List<string>Ǧ=new
List<string>(){"Component","Ore","Ingot","AmmoMagazine","PhysicalGunObject","GasContainerObject","OxygenContainerObject",
"PhysicalObject","ConsumableItem"};Ɗ ǧ;bool Ǩ=false;Dictionary<string,double>ǩ=new Dictionary<string,double>();Dictionary<string,double>
Ǫ=new Dictionary<string,double>();Dictionary<string,double>P=new Dictionary<string,double>();Dictionary<string,double>ǫ=
new Dictionary<string,double>();string ǎ=String.Empty;string Ǹ=String.Empty;string Ǭ=String.Empty;string ǭ=String.Empty;
string Ǯ=String.Empty;string ǯ=String.Empty;string ǰ=String.Empty;string Ǳ=String.Empty;bool ǲ=true;bool ǳ=true;bool Ǵ=false;
int ǵ=0;int Ƕ=0;List<IMyTerminalBlock>Ƿ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ǡ=new List<IMyTerminalBlock>();
List<IMyTerminalBlock>ǹ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ǻ=new List<IMyTerminalBlock>();List<
IMyCargoContainer>Ș=new List<IMyCargoContainer>();List<IMyCargoContainer>ș=new List<IMyCargoContainer>();List<MyInventoryItem>Ț=new List<
MyInventoryItem>();List<IMyCubeGrid>ț=new List<IMyCubeGrid>();List<ƹ>Ȝ=new List<ƹ>();List<IMyTerminalBlock>ȝ=new List<IMyTerminalBlock>
();List<string>Ȟ=new List<string>();List<IMyAssembler>ȟ=new List<IMyAssembler>();IMyTextPanel Ƞ;IMyTextPanel ȡ;
IMyTextPanel Ȣ;IMyTextPanel ȣ;IEnumerator<bool>Ȥ;IMyInventory ȥ;IMyInventory Ȧ;IMyInventory ȧ;const string Ȩ="White";List<
IMyTextPanel>ȩ=new List<IMyTextPanel>();List<IMyBatteryBlock>Ȫ=new List<IMyBatteryBlock>();List<IMyPowerProducer>ȫ=new List<
IMyPowerProducer>();List<IMyPowerProducer>Ȭ=new List<IMyPowerProducer>();List<IMySolarPanel>ȭ=new List<IMySolarPanel>();List<IMyReactor>
ã=new List<IMyReactor>();List<IMyCargoContainer>Ȯ=new List<IMyCargoContainer>();List<MyInventoryItem>ȯ=new List<
MyInventoryItem>();List<IMyCargoContainer>Ȱ=new List<IMyCargoContainer>();List<MyInventoryItem>ȱ=new List<MyInventoryItem>();Dictionary
<string,double>Ȳ=new Dictionary<string,double>();Dictionary<string,double>ȴ=new Dictionary<string,double>();Dictionary<
string,int>ȗ=new Dictionary<string,int>();Dictionary<string,int>ǻ=new Dictionary<string,int>();Dictionary<string,int>ȇ=new
Dictionary<string,int>();Color Ǽ=Color.SteelBlue;Color ǽ=Color.SpringGreen;Color Ǿ=Color.BlanchedAlmond;Color ǿ=Color.DarkSalmon;
Color Ȁ=Color.White;RectangleF ȁ;MySpriteDrawFrame Ā;Vector2 Ȃ;Vector2 ȃ;Color Ȅ;int ȅ;int Ȇ;int Ȉ;List<IMyReactor>ȕ=new List
<IMyReactor>();List<IMyTerminalBlock>ȉ=new List<IMyTerminalBlock>();bool Ȋ=false;bool ȋ=false;MyIni Ȍ=new MyIni();List<
string>ȍ;List<IMyAssembler>Ȏ=new List<IMyAssembler>();int ȏ=0;int Ȑ=0;bool ȑ=false;bool Ȓ=false;bool ȓ=false;List<string>Ȕ=new
List<string>();List<string>Ȗ=new List<string>();List<string>ȳ=new List<string>();List<IMyTerminalBlock>ƅ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>Ǟ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ʀ=new List<IMyTerminalBlock>();List<
IMyRefinery>Ƨ=new List<IMyRefinery>();List<IMyRefinery>ƨ=new List<IMyRefinery>();Dictionary<string,List<IMyRefinery>>Ʃ=new
Dictionary<string,List<IMyRefinery>>();List<string>ƪ=new List<string>();List<IMyAssembler>ƫ=new List<IMyAssembler>();Dictionary<
string,int>Ƭ=new Dictionary<string,int>();Dictionary<string,MyFixedPoint>N=new Dictionary<string,MyFixedPoint>();Dictionary<
string,MyFixedPoint>ƭ=new Dictionary<string,MyFixedPoint>();List<MyInventoryItem>Ʈ=new List<MyInventoryItem>();int Ư=0;class ư
{public string Ʋ{get;set;}public MyFixedPoint Ƽ{get;set;}public float Ƴ{get;set;}}class ƴ<Ĭ>:IEnumerable<Ĭ>{private Queue
<Ĭ>Ƶ;private int ƶ;public ƴ(int Ʒ){ƶ=Ʒ;Ƶ=new Queue<Ĭ>(Ʒ);}public void Ƹ(Ĭ A){if(Ƶ.Count==ƶ)Ƶ.Dequeue();Ƶ.Enqueue(A);}
public IEnumerator<Ĭ>GetEnumerator(){return Ƶ.GetEnumerator();}IEnumerator IEnumerable.GetEnumerator(){return GetEnumerator();
}}class ƹ{public string ƺ{get;set;}public float ƻ{get;set;}public int ƽ{get;set;}}ƴ<double>Ʊ=new ƴ<double>(500);void ƥ(){
Ǣ++;if(Ǣ>=ǡ.Count()){Ǣ=0;}}void ƞ(string ƙ,string ƚ,ref string ƛ){if(!Ȍ.ContainsKey(ƙ,ƚ))return;string Q=Ȍ.Get(ƙ,ƚ).
ToString();if(Q!=null){ƛ=Q;}}void Ɯ(string ƙ,string ƚ,ref bool ƛ){if(!Ȍ.ContainsKey(ƙ,ƚ))return;string Q=Ȍ.Get(ƙ,ƚ).ToString().
ToLowerInvariant();if(Q!=null){switch(Q){case"yes":case"true":ƛ=true;break;case"no":case"false":ƛ=false;break;}}}void Ɲ(string ƙ,string
ƚ,ref float ƛ){if(!Ȍ.ContainsKey(ƙ,ƚ))return;string Q=Ȍ.Get(ƙ,ƚ).ToString();float Ɵ=0;if(Q!=null){if(float.TryParse(Q,out
Ɵ)){ƛ=Ɵ;}}}void Ơ(string ƙ,string ƚ,ref int ƛ){if(!Ȍ.ContainsKey(ƙ,ƚ))return;string Q=Ȍ.Get(ƙ,ƚ).ToString();int Ɵ=0;if(Q
!=null){if(int.TryParse(Q,out Ɵ)){ƛ=Ɵ;}}}void ơ(IMyTextPanel Ƣ,string ú){Ƣ.WriteText(ú+"\n",true);}void ƣ(IMyTextPanel Ƣ){
Ƣ.WriteText("",false);}void Ƙ(){ƥ();if(ȡ!=null){ȡ.FontSize=debugfontSize;ȡ.WriteText("",false);ȡ.WritePublicTitle(
"Wishy's Inventory Manager",false);ơ(ȡ,"Wishy's Inventory Manager "+ǣ);ơ(ȡ,"===========================");ơ(ȡ,ǭ+ǡ[Ǣ]);ơ(ȡ,"");ơ(ȡ,Ǹ+Ǭ);ơ(ȡ,Ǯ);ơ(ȡ,
$"Autocrafting: {Autocrafting}");ơ(ȡ,$"Disassemble Overstock: {DisassembleOverstock}");ơ(ȡ,$"Disassemble list: {AutoDisassemble}");ơ(ȡ,
$"Items disassembled: {ȏ}");ơ(ȡ,$"Manage Refineries: {ȓ}");ơ(ȡ,"");ơ(ȡ,ǎ);}}void Ƥ(){string[]ƾ=Storage.Split('@');if(ƾ.Length>=1){bool.TryParse(ƾ[
0],out ȑ);}if(ƾ.Length>=2){int Ɩ=2;string[]ŷ=ƾ[1].Split(Environment.NewLine.ToCharArray()).Skip(Ɩ).ToArray();string ǎ=
string.Join(Environment.NewLine,ŷ);Ĉ(ǎ);}}void Ǐ(){Echo("Writing config values...");if(Me.CustomData.Length==0){ǘ();}ȡ=(
IMyTextPanel)GridTerminalSystem.GetBlockWithName(LCDName);if(ȡ==null){Echo(
$"no display named {LCDName} found, name a display {LCDName} and recompile");Runtime.UpdateFrequency|=UpdateFrequency.None;}else{ȡ.ContentType=ContentType.TEXT_AND_IMAGE;}if(Autocrafting){Ȣ=(
IMyTextPanel)GridTerminalSystem.GetBlockWithName(LCDNameAutocrafting);if(Ȣ==null){Echo(
$"no display named {LCDNameAutocrafting} found");Runtime.UpdateFrequency|=UpdateFrequency.None;}}if(Debug){ȣ=(IMyTextPanel)GridTerminalSystem.GetBlockWithName(Ǥ);if(ȣ
==null){Echo($"no display named {Ǥ} found");Runtime.UpdateFrequency|=UpdateFrequency.None;}}if(ȣ!=null){ȣ.ContentType=
ContentType.TEXT_AND_IMAGE;ȣ.Script="";ȣ.Font="Monospace";ȣ.FontSize=autocraftingfontSize;}if(Ȣ!=null){Ȣ.ContentType=ContentType.
SCRIPT;Ȣ.Script="";Ȣ.Font="Monospace";Ȣ.FontSize=autocraftingfontSize;Ƥ();}Ƞ=(IMyTextPanel)GridTerminalSystem.GetBlockWithName
(ǥ);if(Ƞ!=null){Ƞ.ContentType=ContentType.SCRIPT;Ƞ.Script="";Ƞ.Font="Monospace";Ƞ.FontSize=fontSize;Ƞ.WriteText("");Ǫ.
Clear();string Æ=Ƞ.CustomData;if(string.IsNullOrEmpty(Æ)){foreach(string Á in oreTypes){string Å=Á;if(!Ǫ.ContainsKey(Å)){Ǫ.
Add(Å,10000);Ƞ.CustomData+=Å+": 10000\n";}}}else{Ǫ=ŵ(Æ);}}if(haveLCD&&ȡ!=null&&Autocrafting&&Ȣ!=null){Runtime.
UpdateFrequency|=UpdateFrequency.Once;}if(haveLCD&&ȡ!=null){Runtime.UpdateFrequency|=UpdateFrequency.Once;}if(!haveLCD&&!Autocrafting){
Runtime.UpdateFrequency|=UpdateFrequency.Once;}}Program(){ǭ="Starting";ǧ=new Ɗ();Ǐ();}void Save(){if(Autocrafting&&Ȣ!=null){if(
Ȣ.GetText().Length>0){Storage=string.Join("@",ȑ,Ȣ.GetText());}}}void Main(string ǐ,UpdateType Ǒ){if((Ǒ&UpdateType.Once)==
UpdateType.Once){if(Ȥ==null){Ȥ=ǖ();}if(Ȥ.MoveNext()==false){if(ǵ==0){Ȥ.Dispose();Ȥ=ǖ();ǵ++;}else if(ǵ==1){Ȥ.Dispose();Ȥ=é();ǵ++;}
else if(ǵ==2){if(shouldCheckAndAssign==true){Ȥ.Dispose();Ȥ=ɓ();Ǵ=true;ǵ++;}else{ǵ++;}}else if(ǵ==3){Ȥ.Dispose();Ȥ=Ǚ();ǵ++;}
else if(ǵ==4){Ȥ.Dispose();Ȥ=ɝ();ǵ++;}else if(ǵ==5){Ȥ.Dispose();Ȥ=Ǉ();ǵ++;}else if(ǵ==6){Ȥ.Dispose();Ȥ=ɪ();ǵ++;}else if(ǵ==7)
{if(Autocrafting){Ȥ.Dispose();Ȥ=Ċ();}ǵ++;}else if(ǵ==8){Ȥ.Dispose();Ȥ=ó();ǵ++;}else if(ǵ==9){Ȥ.Dispose();Ȥ=Ż();ǵ++;}else
if(ǵ==10){if(Ƞ!=null){Ȥ.Dispose();Ȥ=â();ǵ++;}else{ǵ++;}}else{ǵ=0;}}if(haveLCD)Ƙ();Runtime.UpdateFrequency|=UpdateFrequency
.Once;}else if(Ǒ==UpdateType.Terminal||Ǒ==UpdateType.Trigger){Runtime.UpdateFrequency|=UpdateFrequency.Once;}}void ǒ(
DateTime µ){var Ǔ=Runtime.LastRunTimeMs;Ʊ.Ƹ(Ǔ);double ǔ=Ʊ.Count()>0?Ʊ.Average():0.0;var Ǖ=DateTime.Now;Echo(
$"Time Elapsed: {(Ǖ-µ).TotalMilliseconds.ToString("#0.00")} ms");Echo($"Last Runtime: {Ǔ.ToString("#0.00")} ms");Echo($"Average Runtime: {ǔ.ToString("#0.00")} ms (last 500 runs)");
Echo($"Iterations: {Ƕ}");ǎ=$"Time Elapsed: {((Ǖ-µ).TotalMilliseconds/1000).ToString("#0.00")} sec\n"+
$"Last Runtime: {Ǔ.ToString("#0.00")} ms\n"+$"Average Runtime: {ǔ.ToString("#0.00")} ms (last 500 runs)\n"+$"Iterations: {Ƕ}\n";}IEnumerator<bool>ǖ(){ǭ=
"Getting config values";if(Me.CustomData.Length>0){MyIniParseResult Ǘ;if(!Ȍ.TryParse(Me.CustomData,out Ǘ)){Echo($"CustomData error:\nLine {Ǘ}")
;}else{Ɯ("Main","haveLCD",ref haveLCD);ƞ("Main","LCDName",ref LCDName);Ơ("Main","waitTicks",ref waitTicks);Ơ("Main",
"waitTicksMultiplier",ref waitTicksMultiplier);Ɲ("Main","fontSize",ref fontSize);Ɲ("Main","debugfontSize",ref debugfontSize);Ɯ("Main",
"PowerManagement",ref PowerManagement);Ɯ("Main","Autocrafting",ref Autocrafting);Ɯ("Main","AutoDisassemble",ref AutoDisassemble);Ɯ("Main"
,"ManageRefineries",ref ȓ);Ɲ("Power","RatioHigh",ref RatioHigh);Ɲ("Power","RatioLow",ref RatioLow);ƞ("Power",
"BatteryGroup",ref BatteryGroup);ƞ("Power","ReactorGroup",ref ReactorGroup);Ɯ("Power","SkipRecharge",ref SkipRecharge);ƞ(
"AutoDisassembler","UnwantedItems",ref UnwantedItems);ƞ("AutoDisassembler","Disassembler",ref Disassembler);ƞ("AutoCrafting","Assembler",
ref Assembler);ƞ("AutoCrafting","AutocraftingLCDName",ref LCDNameAutocrafting);Ɲ("AutoCrafting","autocraftingfontSize",ref
autocraftingfontSize);Ɯ("AutoCrafting","DisassembleOverstock",ref DisassembleOverstock);ǘ();}}else{ǘ();}if(waitTicks==0)waitTicks=1;if(
waitTicksMultiplier==0)waitTicksMultiplier=1;if(haveLCD)Ƙ();if(Debug){ȣ.WriteText(
$"{ǣ}/{waitTicks}/{waitTicksMultiplier}/{(Ʊ.Count()>0?Ʊ.Average():0.0)}\n",true);if(ȣ.GetText().Split('\n').Length>=500){ȣ.WriteText("");}}for(int L=0;L<waitTicks;++L)yield return true;}void ǘ()
{Me.CustomData=$"[Main]\nhaveLCD = {haveLCD}\nLCDName = {LCDName}\nwaitTicks = {waitTicks}\nwaitTicksMultiplier = {waitTicksMultiplier}\nfontSize = {fontSize}\ndebugfontSize = {debugfontSize}\nPowerManagement = {PowerManagement}\nAutocrafting = {Autocrafting}\nAutoDisassemble = {AutoDisassemble}\nManageRefineries = {ȓ}\n[Power]\n"
+$"RatioHigh = {RatioHigh}\nRatioLow = {RatioLow}\nBatteryGroup = {BatteryGroup}\nReactorGroup = {ReactorGroup}\nSkipRecharge = {SkipRecharge}\n[AutoDisassembler]\nUnwantedItems = {UnwantedItems}"
+$"\nDisassembler = {Disassembler}\n[AutoCrafting]\nAssembler = {Assembler}\nAutocraftingLCDName = {LCDNameAutocrafting}\nautocraftingfontSize = {autocraftingfontSize}\nDisassembleOverstock = {DisassembleOverstock}"
;}IEnumerator<bool>Ǚ(){var µ=DateTime.Now;Ȝ.Clear();ɤ();for(int L=0;L<waitTicks*waitTicksMultiplier;++L)yield return true
;ǭ="Getting Cargo Data";foreach(IMyCargoContainer ǚ in ș){char[]Ǜ=new[]{'\r','\n'};string[]ǜ=ǚ.CustomData.Split(Ǜ,
StringSplitOptions.RemoveEmptyEntries);foreach(string ǝ in ǜ){if(Ǧ.Any(ǝ.Contains)){float ǟ=(float)ǚ.GetInventory(0).CurrentVolume;float Ǎ
=(float)ǚ.GetInventory(0).MaxVolume;float ƿ=100.0f*ǟ/Ǎ;var Ǆ=new ƹ();Ǆ.ƺ=ǝ;Ǆ.ƽ=1;Ǆ.ƻ=ƿ;Ȝ.Add(Ǆ);}ǒ(µ);}for(int L=0;L<
waitTicks;++L)yield return true;}for(int L=0;L<waitTicks;++L)yield return true;Ǭ="Cargo Found:\n";var ǀ=Ȝ.GroupBy(ǁ=>ǁ.ƺ,(S,ǁ)=>{
var ǂ=ǁ as ƹ[]??ǁ.ToArray();return new{ƺ=S,ƽ=ǂ.Length,ƻ=ǂ.Sum(ǃ=>ǃ.ƻ),};}).ToList();for(int L=0;L<waitTicks;++L)yield
return true;ǀ.ForEach(ǅ=>Ǭ+=$"Category: {ǅ.ƺ}, Count: {ǅ.ƽ}, Filled: {(ǅ.ƻ/ǅ.ƽ).ToString("#0.00")}%\n");for(int L=0;L<
waitTicks;++L)yield return true;foreach(var ǆ in Ǧ.Except(Ȟ)){Ǭ+="No cargo found for: "+ǆ+"\n";}for(int L=0;L<waitTicks;++L)yield
return true;if(haveLCD)Ƙ();}IEnumerator<bool>Ǉ(){var µ=DateTime.Now;ǭ="Checking Turrets";if(!Ǩ){try{Ǩ=ǧ.ń(Me);}catch{Ǩ=false;}
}if(Debug){if(Ǩ)ȣ.WriteText($"===================\nWeaponCore is available!\n===================\n",true);else ȣ.
WriteText($"===================\nWeaponCore not available, using vanilla turrets!\n===================\n",true);}ǹ.Clear();Ǻ.
Clear();GridTerminalSystem.GetBlocksOfType(Ǻ,ð=>{if(Ǩ){if(ǧ.ŋ(ð)){Ǻ.Add(ð);return true;}}return false;});for(int E=0;E<
waitTicks;++E)yield return true;if(!Ǩ){Ǻ.Clear();GridTerminalSystem.GetBlocksOfType(Ǻ);ǹ.AddRange(Ǻ.Where(O=>O is
IMyUserControllableGun));}else{ǹ=Ǻ;}Ș.Clear();GridTerminalSystem.GetBlocksOfType(Ș);foreach(var O in ǹ){if(Debug){if(Ǩ)ȣ.WriteText(
$"{O.BlockDefinition} uses {ǧ.ō(O,0)}\n",true);}if((O.BlockDefinition.ToString().Contains("InteriorTurret"))||!O.CustomData.Contains("Manual"))continue;var ǈ=O.
CustomData.Split(Environment.NewLine.ToCharArray());if(ǈ.Length<2)continue;int ǉ=0;if(!int.TryParse(ǈ[1],out ǉ))continue;string Ǌ=
O.BlockDefinition.ToString();if(!weaponToAmmoMapping.ContainsKey(Ǌ))continue;string ǋ=weaponToAmmoMapping[Ǌ];if(Debug){ȣ.
WriteText($"{Ǌ} has {ǋ} as ammo in the list\n",true);}foreach(IMyCargoContainer Ė in Ș){if(Ė==null||!Ė.IsWorking||!O.IsWorking)
continue;if(Ė.CustomData.Contains("Ignore")||O.CustomData.Contains("Ignore"))continue;if(ț.Contains(Ė.CubeGrid)||ț.Contains(O.
CubeGrid))continue;var Ȧ=Ė.GetInventory();var ȼ=O.GetInventory();if(Ȧ==null||ȼ==null)continue;var ǻ=new List<MyInventoryItem>();
Ȧ.GetItems(ǻ);var ɜ=ǻ.FirstOrDefault(A=>A.Type.ToString()==ǋ);if(ɜ==null)continue;MyFixedPoint ɒ=MyFixedPoint.Min(ǉ-ȼ.
GetItemAmount(ɜ.Type),ɜ.Amount);if(ɒ>0){if(Debug){ȣ.WriteText($"tranferring {ɒ} of {ɜ.Type} to {Ǌ}\n",true);}try{Ȧ.TransferItemTo(ȼ,ɜ
,ɒ);}catch{if(Debug){ȣ.WriteText($"can't tranfer {ɒ} of {ɜ.Type} to {Ǌ}\n",true);}}}if(haveLCD)Ƙ();ǒ(µ);for(int L=0;L<
waitTicks;++L)yield return true;}}if(haveLCD)Ƙ();for(int L=0;L<waitTicks;++L)yield return true;}IEnumerator<bool>ɝ(){var µ=
DateTime.Now;ǭ="Sorting";Ƿ.Clear();GridTerminalSystem.GetBlocks(Ƿ);for(int E=0;E<waitTicks;++E)yield return true;Ǡ.Clear();Ǡ.
AddRange(Ƿ.Where(O=>O is IMyCargoContainer||O is IMyShipConnector||O is IMyAssembler||O is IMyRefinery||O is IMyGasGenerator||O
is IMyGasTank));ț.Clear();ț.AddRange(Ǡ.Where(O=>O is IMyShipConnector&&O.CustomData.Contains("Stop")&&(O as
IMyShipConnector).Status==MyShipConnectorStatus.Connected).Select(O=>(O as IMyShipConnector).OtherConnector.CubeGrid));for(int E=0;E<
waitTicks;++E)yield return true;Ș.Clear();GridTerminalSystem.GetBlocksOfType(Ș);for(int E=0;E<waitTicks;++E)yield return true;
foreach(IMyTerminalBlock O in Ǡ){if(O is IMyProductionBlock){if(O is IMyAssembler&&((IMyAssembler)O).Mode==MyAssemblerMode.
Disassembly){ȥ=((IMyProductionBlock)O).InputInventory;}else{ȥ=((IMyProductionBlock)O).OutputInventory;}}else{if(O==null)continue;ȥ=
O.GetInventory();}for(int E=0;E<waitTicks;++E)yield return true;if(ȥ.ItemCount<1)continue;Ț.Clear();ȥ.GetItems(Ț);for(int
E=0;E<waitTicks;++E)yield return true;foreach(MyInventoryItem A in Ț){string Ô=A.Type.TypeId.Split('_')[1];var R=A.Amount
;foreach(IMyCargoContainer Ė in Ș){if(Ė==null||O==null||!Ė.IsWorking||!O.IsWorking)continue;if(Ė.CustomData.Contains(
"Ignore")||O.CustomData.Contains("Ignore"))continue;if(ț.Contains(Ė.CubeGrid)||ț.Contains(O.CubeGrid))continue;if(Ė.CustomData.
Contains("Manual")&&Ė.CustomData.Contains(A.Type.SubtypeId)||O.CustomData.Contains("Manual")&&O.CustomData.Contains(A.Type.
SubtypeId)){if(!Ė.CustomData.Contains(A.Type.SubtypeId)||O.CustomData.Contains(A.Type.SubtypeId)||Ė.Equals(O))continue;}else{if(!
Ė.CustomData.Contains(Ô)||O.CustomData.Contains(Ô)||Ė.Equals(O)){continue;}}Ȧ=Ė.GetInventory();for(int E=0;E<waitTicks;++
E)yield return true;if(Ȧ==null||ȥ==null||A==null||Ȧ.IsFull)continue;if((Ė.CustomData.Contains("Manual")&&Ė.CustomData.
Contains(A.Type.SubtypeId)||O.CustomData.Contains("Manual")&&O.CustomData.Contains(A.Type.SubtypeId))&&Ė.CustomData.Contains("="
)||O.CustomData.Contains("=")){string[]ŷ={""};if(Ė.CustomData.Contains("Manual")&&Ė.CustomData.Contains("=")){ŷ=Ė.
CustomData.Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray();}else if(O.CustomData.Contains("Manual")&&O.CustomData.
Contains("=")){ŷ=O.CustomData.Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray();}foreach(string ú in ŷ){if(ú.Contains(A
.Type.SubtypeId)){var ɟ=ú.Trim().Split('=');var ɠ=ɟ[0];var ɡ=0;int.TryParse(ɟ[1],out ɡ);MyFixedPoint ɢ=0;Ʈ.Clear();Ȧ=Ė.
GetInventory(0);Ȧ.GetItems(Ʈ);if(Debug){ȣ.WriteText($"found {Ʈ.Count} cargoitems\n",true);}int ɞ=0;foreach(var ɛ in Ʈ){if(Debug){ȣ.
WriteText($"name: {ɛ.Type.SubtypeId} | itemname: {ɠ} | amount: {ɛ.Amount}\n",true);}if(ɛ.Type.SubtypeId.ToLower().Trim()==ɠ.
ToLower().Trim()){ɢ+=ɛ.Amount;if(Debug){ȣ.WriteText($"found {ɛ.Amount} {ɠ} to total {ɢ}\n",true);}}else{if(Debug){ȣ.WriteText(
$"{ɛ.Type.SubtypeId} =/= {ɠ}\n",true);}}}for(int E=0;E<waitTicks;++E)yield return true;MyFixedPoint ɑ=MyFixedPoint.MultiplySafe(Ȧ.MaxVolume-Ȧ.
CurrentVolume,(1/A.Type.GetItemInfo().Volume));MyFixedPoint Ü=MyFixedPoint.Min(ɑ.ToIntSafe(),ɡ);Ü-=ɢ;if(Ü<=0||!ɧ(ȥ,Ȧ,A,Ü))continue;if
(Debug){ȣ.WriteText($"{A.Type.SubtypeId}/{ɠ} has {ɢ} in cargo, moving {Ü} to reach {ɡ}\n",true);}for(int E=0;E<waitTicks;
++E)yield return true;if(Ȧ==null||ȥ==null||A==null)continue;while(ɞ<Ü){var ɒ=MyFixedPoint.Min(Ü-ɞ,A.Amount);ȥ.
TransferItemTo(Ȧ,A,ɒ);ɞ+=(int)ɒ;if(haveLCD)Ƙ();ǒ(µ);for(int E=0;E<waitTicks;++E)yield return true;if(ɞ>=Ü)break;}}for(int E=0;E<
waitTicks;++E)yield return true;}}else{MyFixedPoint ɑ=MyFixedPoint.MultiplySafe(Ȧ.MaxVolume-Ȧ.CurrentVolume,(1/A.Type.GetItemInfo
().Volume));MyFixedPoint Ü=MyFixedPoint.Min(ɑ.ToIntSafe(),A.Amount);if(Ü==0||!ɧ(ȥ,Ȧ,A,Ü))continue;for(int E=0;E<waitTicks
;++E)yield return true;if(Ȧ==null||ȥ==null||A==null)continue;ȥ.TransferItemTo(Ȧ,A,Ü);if(haveLCD)Ƙ();ǒ(µ);}for(int E=0;E<
waitTicks;++E)yield return true;}for(int E=0;E<waitTicks;++E)yield return true;}for(int E=0;E<waitTicks;++E)yield return true;}if
(haveLCD)Ƙ();Ƕ++;}IEnumerator<bool>ɓ(){ǭ="Check And Assign Cargo Containers";ȝ.Clear();GridTerminalSystem.GetBlocksOfType
<IMyCargoContainer>(ȝ,ɚ=>ɚ.CubeGrid==Me.CubeGrid);for(int E=0;E<waitTicks;++E)yield return true;if(Debug){ȣ.WriteText(
$"found {Ǧ.Count} categories\n",true);}foreach(string ɔ in Ǧ){List<IMyCargoContainer>ɕ=new List<IMyCargoContainer>();List<IMyCargoContainer>ɖ=new List<
IMyCargoContainer>();List<IMyCargoContainer>ɗ=new List<IMyCargoContainer>();if(Debug){ȣ.WriteText($"category: {ɔ}\n",true);}foreach(
IMyCargoContainer ɘ in ȝ){if(ɘ.BlockDefinition.SubtypeName.Contains("Locker")||ɘ.BlockDefinition.SubtypeName.Contains("WeaponRack")){
continue;}char[]Ǜ=new[]{'\r','\n'};string[]ǜ=ɘ.CustomData.Split(Ǜ,StringSplitOptions.RemoveEmptyEntries);if(Debug){}bool ə=false
;foreach(string ǝ in ǜ){if(Ǧ.Contains(ǝ)||ǝ.ToLower()=="ignore"||ǝ.ToLower()=="manual"){ɕ.Add(ɘ);if(Debug){ȣ.WriteText(
$"    {ɘ.DisplayNameText} has {ǝ} as category\n",true);}if(ǝ.ToLower()==ɔ.ToLower()){ɖ.Add(ɘ);}ə=true;}}for(int E=0;E<waitTicks;++E)yield return true;if(!ə){ɗ.Add(ɘ);if
(Debug){ȣ.WriteText($"  {ɘ.DisplayNameText} has no categories\n",true);}}}for(int E=0;E<waitTicks;++E)yield return true;
if(Ǵ){float ɐ=ɖ.Sum(ɚ=>(float)ɚ.GetInventory(0).CurrentVolume/(float)ɚ.GetInventory(0).MaxVolume)*100f/ɖ.Count;if(Debug){ȣ
.WriteText($"{ɔ} uses {ɐ} cargo space\n",true);}if(ɐ>=CargoOverFlowMax){IMyCargoContainer ɣ=null;foreach(
IMyCargoContainer ɘ in ɗ){if(Debug){ȣ.WriteText($"{ɘ.DisplayNameText} has no category assigned\n",true);}ɣ=ɘ;break;}for(int E=0;E<
waitTicks;++E)yield return true;if(ɣ==null){foreach(IMyCargoContainer ɘ in ɕ.OrderBy(ɚ=>(float)ɚ.GetInventory(0).CurrentVolume/(
float)ɚ.GetInventory(0).MaxVolume)){if(Debug){ȣ.WriteText($"{ɘ.DisplayNameText} has low fill percentage\n",true);}ɣ=ɘ;break;}
for(int E=0;E<waitTicks;++E)yield return true;}if(ɣ!=null){if(Debug){ȣ.WriteText($"adding {ɔ} to {ɣ.DisplayNameText}\n",
true);}ɣ.CustomData+=ɔ+"\n";}}}if(ɖ.Count==0){if(Debug){ȣ.WriteText($"found {ɖ.Count} assignedWithThisContainers\n",true);}
IMyCargoContainer ɣ=null;foreach(IMyCargoContainer ɘ in ɗ){if(Debug){ȣ.WriteText($"{ɘ.DisplayNameText} has no category assigned\n",true);
}ɣ=ɘ;break;}for(int E=0;E<waitTicks;++E)yield return true;if(ɣ==null){foreach(IMyCargoContainer ɘ in ɕ.OrderBy(ɚ=>(float)
ɚ.GetInventory(0).CurrentVolume/(float)ɚ.GetInventory(0).MaxVolume)){if(Debug){ȣ.WriteText(
$"{ɘ.DisplayNameText} has low fill percentage\n",true);}ɣ=ɘ;break;}for(int E=0;E<waitTicks;++E)yield return true;}if(ɣ!=null){if(Debug){ȣ.WriteText(
$"++ adding {ɔ} to {ɣ.DisplayNameText}\n",true);}ɣ.CustomData+=ɔ+"\n";}}Ư=ɗ.Count;for(int E=0;E<waitTicks;++E)yield return true;}}void ɤ(){var µ=DateTime.Now;Ȟ.
Clear();ș.Clear();ȝ.Clear();GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(ȝ,ɚ=>ɚ.CubeGrid==Me.CubeGrid);ǭ=
"Getting Cargo Types";foreach(IMyCargoContainer ɥ in ȝ){bool ɦ=false;char[]Ǜ=new[]{'\r','\n'};string[]ǜ=ɥ.CustomData.Split(Ǜ,
StringSplitOptions.RemoveEmptyEntries);foreach(string ǝ in ǜ){if(Ǧ.Any(ǝ.Contains)){Ȟ.Add(ǝ);ɦ=true;}}if(ɦ){ș.Add(ɥ);}ǒ(µ);}if(haveLCD)Ƙ()
;}bool ɧ(IMyInventory ɨ,IMyInventory ɩ,MyInventoryItem A,MyFixedPoint Ü){return ɨ.IsConnectedTo(ɩ)&&ɨ.CanTransferItemTo(ɩ
,A.Type)&&ɩ.CanItemsBeAdded(Ü,A.Type);}IEnumerator<bool>ɪ(){int ɫ=Math.Max(waitTicksMultiplier,1);var µ=DateTime.Now;ǭ=
"Listing";ȩ.Clear();GridTerminalSystem.GetBlocksOfType(ȩ,ȷ=>ȷ.IsWorking&&!ȷ.CustomData.Contains("Ignore"));for(int E=0;E<
waitTicks;++E)yield return true;Ȫ.Clear();GridTerminalSystem.GetBlocksOfType(Ȫ,ȷ=>ȷ.IsWorking);for(int E=0;E<waitTicks;++E)yield
return true;ȫ.Clear();GridTerminalSystem.GetBlocksOfType(ȫ,ȷ=>ȷ.IsWorking&&ȷ.BlockDefinition.SubtypeId.Contains("Wind"));for(
int E=0;E<waitTicks;++E)yield return true;Ȭ.Clear();GridTerminalSystem.GetBlocksOfType(Ȭ,ȷ=>ȷ.IsWorking&&ȷ.BlockDefinition.
SubtypeId.Contains("Hydrogen"));for(int E=0;E<waitTicks;++E)yield return true;ȭ.Clear();GridTerminalSystem.GetBlocksOfType(ȭ,ȷ=>ȷ
.IsWorking);for(int E=0;E<waitTicks;++E)yield return true;ã.Clear();GridTerminalSystem.GetBlocksOfType(ã,ȷ=>ȷ.IsWorking);
for(int E=0;E<waitTicks;++E)yield return true;Ȯ.Clear();GridTerminalSystem.GetBlocksOfType(Ȯ);for(int E=0;E<waitTicks;++E)
yield return true;Ǡ.Clear();Ǡ.AddRange(Ƿ.Where(O=>O is IMyCargoContainer||O is IMyShipConnector||O is IMyAssembler||O is
IMyRefinery));for(int E=0;E<waitTicks;++E)yield return true;Ȳ.Clear();ȴ.Clear();ȗ.Clear();ǻ.Clear();foreach(IMyTerminalBlock O in Ǡ
){if(O is IMyProductionBlock){if(O is IMyAssembler&&((IMyAssembler)O).Mode==MyAssemblerMode.Disassembly){ȥ=((
IMyProductionBlock)O).InputInventory;}else{ȥ=((IMyProductionBlock)O).OutputInventory;}if(O is IMyRefinery&&((IMyRefinery)O).GetInventory()
.ItemCount>0){ȥ=((IMyRefinery)O).GetInventory();}}else{if(O==null)continue;ȥ=O.GetInventory();}for(int E=0;E<waitTicks;++
E)yield return true;if(ȥ.ItemCount<1)continue;ȯ.Clear();ȥ.GetItems(ȯ);for(int E=0;E<waitTicks;++E)yield return true;
foreach(var A in ȯ.OrderBy(E=>E.Type.SubtypeId)){var Ô=A.Type.TypeId.Split('_')[1];var Ö=A.Type.SubtypeId;var Q=A.Amount.
ToString().Split('.');int Ü;int.TryParse(Q[0],out Ü);if(Ô=="Ore"){if(!Ȳ.ContainsKey(Ö))Ȳ.Add(Ö,0);Ȳ[Ö]+=Ü;}else if(Ô=="Ingot"){
if(!ȴ.ContainsKey(Ö))ȴ.Add(Ö,0);ȴ[Ö]+=Ü;}else if(Ô=="Component"){if(!ȗ.ContainsKey(Ö))ȗ.Add(Ö,0);ȗ[Ö]+=Ü;}else if(Ô==
"ConsumableItem"||Ô=="PhysicalObject"||Ô=="AmmoMagazine"||Ô=="GasContainerObject"||Ô=="OxygenContainerObject"||Ô=="PhysicalGunObject"){
if(!ǻ.ContainsKey(Ö))ǻ.Add(Ö,0);ǻ[Ö]+=Ü;}}if(haveLCD)Ƙ();ǒ(µ);for(int E=0;E<waitTicks;++E)yield return true;}if(haveLCD)Ƙ(
);for(int E=0;E<waitTicks;++E)yield return true;ȅ=0;Ȇ=0;Ȉ=0;foreach(var ȶ in ȩ.OrderBy(ȷ=>ȷ.DisplayNameText)){if(ȶ.
CustomData.Contains("WIM")){Ȅ=ȶ.ScriptForegroundColor;ȶ.ContentType=ContentType.SCRIPT;ȶ.Script="";ȁ=new RectangleF((ȶ.TextureSize
-ȶ.SurfaceSize)/2f,ȶ.SurfaceSize);Ā=ȶ.DrawFrame();var m=new Vector2(5,5)+ȁ.Position;Ȃ=new Vector2(ȶ.SurfaceSize.X-10,0);ȃ
=new Vector2(0,30*fontSize);Ⱦ(ref Ā,ref m,ȶ);for(int E=0;E<2;++E)yield return true;Ā.Dispose();for(int E=0;E<2;++E)yield
return true;Ā=ȶ.DrawFrame();var r=µ.AddHours(TimeDifference);var u=r.ToString("H:mm:ss");ÿ(ref Ā,u,m+Ȃ,TextAlignment.RIGHT);
for(int E=0;E<2;++E)yield return true;if(ȶ.CustomData.Contains("WIM")&&ȶ.CustomData.Contains("Battery"))ȿ(ref Ā,ref m,ȶ);
for(int E=0;E<2;++E)yield return true;if(ȶ.CustomData.Contains("WIM")&&ȶ.CustomData.Contains("Wind"))Ȼ(ref Ā,ref m,ȶ);for(
int E=0;E<2;++E)yield return true;if(ȶ.CustomData.Contains("WIM")&&ȶ.CustomData.Contains("Hydro"))ȸ(ref Ā,ref m,ȶ);for(int
E=0;E<2;++E)yield return true;if(ȶ.CustomData.Contains("WIM")&&ȶ.CustomData.Contains("Solar"))ȹ(ref Ā,ref m,ȶ);for(int E=
0;E<2;++E)yield return true;if(ȶ.CustomData.Contains("WIM")&&ȶ.CustomData.Contains("Reactor"))Ⱥ(ref Ā,ref m,ȶ);for(int E=
0;E<2;++E)yield return true;if(ȶ.CustomData.Contains("WIM")&&ȶ.CustomData.Contains("Ore"))Ɋ(ref Ā,ref m,ȶ);for(int E=0;E<
2;++E)yield return true;if(ȶ.CustomData.Contains("WIM")&&ȶ.CustomData.Contains("Ingot"))ɋ(ref Ā,ref m,ȶ);for(int E=0;E<2;
++E)yield return true;if(ȶ.CustomData.Contains("WIM")&&ȶ.CustomData.Contains("Component"))Ɍ(ref Ā,ref m,ȶ);for(int E=0;E<2
;++E)yield return true;if(ȶ.CustomData.Contains("WIM")&&ȶ.CustomData.Contains("Item"))ɍ(ref Ā,ref m,ȶ);for(int E=0;E<2;++
E)yield return true;Ā.Dispose();if(haveLCD)Ƙ();for(int E=0;E<waitTicks;++E)yield return true;}}var Ƚ=DateTime.Now;Echo(
$"Total Runtime: {(Ƚ-µ).TotalMilliseconds.ToString("#0.00")} ms");for(int E=0;E<waitTicks;++E)yield return true;}void Ⱦ(ref MySpriteDrawFrame Ā,ref Vector2 m,IMyTextPanel ȶ){ÿ(ref Ā,
"Loading...",m,TextAlignment.LEFT);}void ȿ(ref MySpriteDrawFrame Ā,ref Vector2 m,IMyTextPanel ȶ){var ȵ=Ȫ.Sum(ȷ=>ȷ.CurrentStoredPower
);var ƃ=Ȫ.Sum(ȷ=>ȷ.MaxStoredPower);var G=Ȫ.Sum(ȷ=>ȷ.CurrentInput);var ǎ=Ȫ.Sum(ȷ=>ȷ.CurrentOutput);ÿ(ref Ā,"Battery",m,
TextAlignment.LEFT);m+=ȃ;ÿ(ref Ā,"Current Stored Power:",m,TextAlignment.LEFT);ÿ(ref Ā,ȵ.ToString()+" MWh",m+Ȃ,TextAlignment.RIGHT);m
+=ȃ;ÿ(ref Ā,"Max Stored Power:",m,TextAlignment.LEFT);ÿ(ref Ā,ƃ.ToString()+" MWh",m+Ȃ,TextAlignment.RIGHT);m+=ȃ;ÿ(ref Ā,
"Current Input:",m,TextAlignment.LEFT);ÿ(ref Ā,G.ToString("#0.00")+" MW",m+Ȃ,TextAlignment.RIGHT);m+=ȃ;ÿ(ref Ā,"Current Output:",m,
TextAlignment.LEFT);ÿ(ref Ā,ǎ.ToString("#0.00")+" MW",m+Ȃ,TextAlignment.RIGHT);m+=ȃ+ȃ;}void Ȼ(ref MySpriteDrawFrame Ā,ref Vector2 m,
IMyTextPanel ȶ){var ȵ=ȫ.Sum(ȷ=>ȷ.CurrentOutput);var ƃ=ȫ.Sum(ȷ=>ȷ.Components.Get<MyResourceSourceComponent>().DefinedOutput);ÿ(ref Ā,
"Wind Turbine",m,TextAlignment.LEFT);m+=ȃ;ÿ(ref Ā,"Current Output:",m,TextAlignment.LEFT);ÿ(ref Ā,ȵ.ToString("#0.00")+" MW",m+Ȃ,
TextAlignment.RIGHT);m+=ȃ;ÿ(ref Ā,"Max Output:",m,TextAlignment.LEFT);ÿ(ref Ā,ƃ.ToString("#0.00")+" MW",m+Ȃ,TextAlignment.RIGHT);m+=ȃ
+ȃ;}void ȸ(ref MySpriteDrawFrame Ā,ref Vector2 m,IMyTextPanel ȶ){var ȵ=Ȭ.Sum(ȷ=>ȷ.CurrentOutput);var ƃ=Ȭ.Sum(ȷ=>ȷ.
MaxOutput);ÿ(ref Ā,"Hydrogen Engines",m,TextAlignment.LEFT);m+=ȃ;ÿ(ref Ā,"Current Output:",m,TextAlignment.LEFT);ÿ(ref Ā,ȵ.
ToString("#0.00")+" MW",m+Ȃ,TextAlignment.RIGHT);m+=ȃ;ÿ(ref Ā,"Max Output:",m,TextAlignment.LEFT);ÿ(ref Ā,ƃ.ToString("#0.00")+
" MW",m+Ȃ,TextAlignment.RIGHT);m+=ȃ+ȃ;}void ȹ(ref MySpriteDrawFrame Ā,ref Vector2 m,IMyTextPanel ȶ){var ȵ=ȭ.Sum(ȷ=>ȷ.
CurrentOutput);var ƃ=ȭ.Sum(ȷ=>ȷ.Components.Get<MyResourceSourceComponent>().DefinedOutput);ÿ(ref Ā,"Solar Panels",m,TextAlignment.
LEFT);m+=ȃ;ÿ(ref Ā,"Current Output:",m,TextAlignment.LEFT);ÿ(ref Ā,ȵ.ToString("#0.00")+" MW",m+Ȃ,TextAlignment.RIGHT);m+=ȃ;ÿ
(ref Ā,"Max Output:",m,TextAlignment.LEFT);ÿ(ref Ā,ƃ.ToString("#0.00")+" MW",m+Ȃ,TextAlignment.RIGHT);m+=ȃ+ȃ;}void Ⱥ(ref
MySpriteDrawFrame Ā,ref Vector2 m,IMyTextPanel ȶ){var ȵ=ã.Sum(ȷ=>ȷ.CurrentOutput);var ƃ=ã.Sum(ȷ=>ȷ.MaxOutput);ÿ(ref Ā,"Reactors",m,
TextAlignment.LEFT);m+=ȃ;ÿ(ref Ā,"Current Output:",m,TextAlignment.LEFT);ÿ(ref Ā,ȵ.ToString("#0.00")+" MW",m+Ȃ,TextAlignment.RIGHT);m
+=ȃ;ÿ(ref Ā,"Max Output:",m,TextAlignment.LEFT);ÿ(ref Ā,ƃ.ToString("#0.00")+" MW",m+Ȃ,TextAlignment.RIGHT);m+=ȃ+ȃ;}void Ɋ(
ref MySpriteDrawFrame Ā,ref Vector2 m,IMyTextPanel ȶ){ÿ(ref Ā,"Ores",m,TextAlignment.LEFT,Ǽ);ÿ(ref Ā,"===============",m+=ȃ
,TextAlignment.LEFT,Ǽ);m+=ȃ;foreach(var A in Ȳ.Skip(ȅ)){ÿ(ref Ā,A.Key,m,TextAlignment.LEFT,Ǽ);ÿ(ref Ā,Ɇ(A.Value),m+Ȃ,
TextAlignment.RIGHT,Ǽ);m+=ȃ;ȅ++;if(m.Y+ȃ.Y>=ȶ.SurfaceSize.Y)break;}m+=ȃ;if(ȅ>=Ȳ.Count())ȅ=0;}void ɋ(ref MySpriteDrawFrame Ā,ref
Vector2 m,IMyTextPanel ȶ){ÿ(ref Ā,"Ingots",m,TextAlignment.LEFT,ǽ);ÿ(ref Ā,"===============",m+=ȃ,TextAlignment.LEFT,ǽ);m+=ȃ;
foreach(var A in ȴ.Skip(Ȇ)){ÿ(ref Ā,A.Key.Replace("Stone","Gravel"),m,TextAlignment.LEFT,ǽ);ÿ(ref Ā,Ɇ(A.Value),m+Ȃ,
TextAlignment.RIGHT,ǽ);m+=ȃ;Ȇ++;if(m.Y+ȃ.Y>=ȶ.SurfaceSize.Y)break;}m+=ȃ;if(Ȇ>=ȴ.Count())Ȇ=0;}void Ɍ(ref MySpriteDrawFrame Ā,ref
Vector2 m,IMyTextPanel ȶ){ÿ(ref Ā,"Components",m,TextAlignment.LEFT,Ǿ);ÿ(ref Ā,"===============",m+=ȃ,TextAlignment.LEFT,Ǿ);
Color Ɨ=Ǿ;Color ö=Color.ForestGreen;Color ĭ=Color.SteelBlue;Color ø=Color.MediumPurple;m+=ȃ;foreach(var A in ȗ.Skip(Ȉ).
OrderBy(E=>E.Key)){Ǿ=Ɨ;if(A.Key.ToString().Contains("Enhanced"))Ǿ=ö;if(A.Key.ToString().Contains("Proficient"))Ǿ=ĭ;if(A.Key.
ToString().Contains("Elite"))Ǿ=ø;ÿ(ref Ā,A.Key,m,TextAlignment.LEFT,Ǿ);ÿ(ref Ā,Ɇ(A.Value),m+Ȃ,TextAlignment.RIGHT,Ǿ);m+=ȃ;Ȉ++;if
(m.Y+ȃ.Y>=ȶ.SurfaceSize.Y)break;}m+=ȃ;if(Ȉ>=ȗ.Count())Ȉ=0;}void ɍ(ref MySpriteDrawFrame Ā,ref Vector2 m,IMyTextSurface Ɏ)
{ÿ(ref Ā,"Items",m,TextAlignment.LEFT,ǿ);ÿ(ref Ā,"===============",m+=ȃ,TextAlignment.LEFT,ǿ);m+=ȃ;foreach(var A in ǻ.
OrderBy(E=>E.Key)){ÿ(ref Ā,A.Key,m,TextAlignment.LEFT,ǿ);ÿ(ref Ā,Ɇ(A.Value),m+Ȃ,TextAlignment.RIGHT,ǿ);m+=ȃ;}m+=ȃ;}void ɏ(ref
MySpriteDrawFrame Ā,ref Vector2 m,IMyTextPanel ȶ,string ĉ){ÿ(ref Ā,"WIM Autocrafting",m,TextAlignment.LEFT,Ȁ);ÿ(ref Ā,"===============",m
+=ȃ,TextAlignment.LEFT,Ȁ);Color Ɨ=Color.White;Color ö=Color.ForestGreen;Color ĭ=Color.SteelBlue;Color ø=Color.MediumPurple
;int ɀ=70;int Ɂ=60;int ɂ=90;m+=ȃ;var ù=ĉ.Trim().Split(';');foreach(var ú in ù.OrderBy(ð=>ð)){if(ú.Contains('|')){var A=ú.
Split('|');if(A.Length<=3){Ȁ=Ɨ;ɀ=(ɀ+15)%256;Ɂ=(Ɂ+41)%256;ɂ=(ɂ+83)%256;Color û=new Color(ɀ,Ɂ,ɂ,10);if(A[0].ToString().Contains
("Enhanced"))Ȁ=ö;if(A[0].ToString().Contains("Proficient"))Ȁ=ĭ;if(A[0].ToString().Contains("Elite"))Ȁ=ø;int Ƀ=A[1].
IndexOf('(');int ɉ=A[1].IndexOf(')');string ǎ=A[1];if(Ƀ!=-1&&ɉ!=-1){ǎ=A[1].Replace(A[1].Substring(Ƀ,ɉ-Ƀ+1),"").Trim();}int Z;
int.TryParse(ǎ,out Z);int Ʉ;int.TryParse(A[2],out Ʉ);float Ʌ;if(Ʉ==0){Ʌ=0f;}else if(Z>=Ʉ){Ʌ=1f;}else{Ʌ=(float)Z/(float)Ʉ;}
var ü=(float)(ȶ.SurfaceSize.X*2)*(float)Ʌ;if(Debug){ȣ.WriteText($"{A[0]} | fillcolor: {û.ToString()} CurrentStock: {Z} / WantedtStock: {Ʉ} ({A[1]}/{A[2]}) (%: {(float)Ʌ}, width: {(float)ü})\n"
,true);}var ý=new MySprite(){Type=SpriteType.TEXTURE,Data="SquareSimple",Position=m+(ȃ.Y/2),Size=new Vector2(ü,ȃ.Y),Color
=û,Alignment=TextAlignment.CENTER};Ā.Add(ý);ÿ(ref Ā,A[0],m,TextAlignment.LEFT,Ȁ,autocraftingfontSize);ÿ(ref Ā,
$"{A[1]}/{Ɇ(int.Parse(A[2]))}",m+Ȃ,TextAlignment.RIGHT,Ȁ,autocraftingfontSize);m+=ȃ;}if(m.Y+ȃ.Y>=ȶ.SurfaceSize.Y)break;}}m+=ȃ;}static string Ɇ(double
ɇ){if(ɇ>=100000000)return(ɇ/1000000).ToString("#,0 M");if(ɇ>=10000000)return(ɇ/1000000).ToString("0.#")+" M";if(ɇ>=100000
)return(ɇ/1000).ToString("#,0 K");if(ɇ>=10000)return(ɇ/1000).ToString("0.#")+" K";return ɇ.ToString("#,0");}void Ɉ(ref
MySpriteDrawFrame Ā,ref Vector2 m,IMyTextPanel ȶ,string ĉ){ÿ(ref Ā,"WIM Task List",m,TextAlignment.LEFT,Ȁ);ÿ(ref Ā,"===============",m+=ȃ
,TextAlignment.LEFT,Ȁ);Color Ɨ=Color.White;Color ö=Color.ForestGreen;Color ĭ=Color.SteelBlue;Color ø=Color.MediumPurple;m
+=ȃ;if(ĉ!=""){ÿ(ref Ā,"Most needed ore:",m,TextAlignment.LEFT,Ȁ,wantedfontSize);m+=ȃ;var ù=ĉ.Trim().Split('\n');var E=1;
foreach(var ú in ù){Ȁ=Ɨ;Color û=Color.Gray;switch(ú){case"Cobalt":û=Color.DeepSkyBlue;break;case"Gold":û=Color.Gold;Ȁ=Color.
Black;break;case"Iron":û=Color.SlateGray;break;case"Magnesium":û=Color.LightSteelBlue;Ȁ=Color.Black;break;case"Nickel":û=
Color.LightGoldenrodYellow;Ȁ=Color.Black;break;case"Platinum":û=Color.Coral;Ȁ=Color.Black;break;case"Silicon":û=Color.
DarkSlateBlue;break;case"Silver":û=Color.Silver;Ȁ=Color.Black;break;case"Stone":û=Color.SandyBrown;break;case"Uranium":û=Color.
DarkGray;break;default:break;}var ü=(float)(ȶ.SurfaceSize.X*2);var ý=new MySprite(){Type=SpriteType.TEXTURE,Data="SquareSimple",
Position=m+(ȃ.Y/2),Size=new Vector2(ü,ȃ.Y),Color=û.Alpha(0.2f),Alignment=TextAlignment.CENTER};Ā.Add(ý);ÿ(ref Ā,$"{E}. {ú}",m,
TextAlignment.LEFT,Ȁ,wantedfontSize);E++;m+=ȃ;}}else{ÿ(ref Ā,"No ore needed at the moment.",m,TextAlignment.LEFT,Ȁ,wantedfontSize);m
+=ȃ;}m+=ȃ;if(shouldCheckAndAssign){if(Ư<=2){var þ=new MySprite(){Type=SpriteType.TEXTURE,Data="SquareSimple",Position=m+ȃ.
Y,Size=new Vector2((ȶ.SurfaceSize.X*2),ȃ.Y*2),Color=Color.Red.Alpha(0.2f),Alignment=TextAlignment.CENTER};Ā.Add(þ);}Ȁ=Ɨ;
if(Ư==0){ÿ(ref Ā,"No unassigned cargo containers left",m,TextAlignment.LEFT,Ȁ,wantedfontSize);m+=ȃ;ÿ(ref Ā,
"build new ones ASAP!",m,TextAlignment.LEFT,Ȁ,wantedfontSize);}else if(Ư<=2){ÿ(ref Ā,"Almost running out of cargo",m,TextAlignment.LEFT,Ȁ,
wantedfontSize);m+=ȃ;ÿ(ref Ā,"build new ones soon!",m,TextAlignment.LEFT,Ȁ,wantedfontSize);}}}void ÿ(ref MySpriteDrawFrame Ā,string ā,
Vector2 m,TextAlignment ă){var õ=new MySprite{Type=SpriteType.TEXT,Data=ā,Position=m,RotationOrScale=fontSize,Color=Ȅ,Alignment
=ă,FontId=Ȩ};Ā.Add(õ);}void ÿ(ref MySpriteDrawFrame Ā,string ā,Vector2 m,TextAlignment ă,Color Ą){var õ=new MySprite{Type
=SpriteType.TEXT,Data=ā,Position=m,RotationOrScale=fontSize,Color=Ą,Alignment=ă,FontId=Ȩ};Ā.Add(õ);}void ÿ(ref
MySpriteDrawFrame Ā,string ā,Vector2 m,TextAlignment ă,Color Ą,float Ă){var õ=new MySprite{Type=SpriteType.TEXT,Data=ā,Position=m,
RotationOrScale=Ă,Color=Ą,Alignment=ă,FontId=Ȩ};Ā.Add(õ);}void í(List<IMyReactor>ã,bool ä){foreach(var å in ã){if(ä){å.Enabled=true;}
else{å.Enabled=false;}}}void æ(List<IMyTerminalBlock>ç){foreach(var è in ç){è.ApplyAction("TriggerNow");}}IEnumerator<bool>é
(){if(PowerManagement){ǭ="Checking power";ȉ.Clear();if(BatteryGroup.Length>0){var ê=GridTerminalSystem.
GetBlockGroupWithName(BatteryGroup);ê.GetBlocksOfType<IMyBatteryBlock>(ȉ,(ë)=>ë.IsSameConstructAs(Me));}else{GridTerminalSystem.
GetBlocksOfType<IMyBatteryBlock>(ȉ,(ë)=>ë.IsSameConstructAs(Me));}if(ReactorGroup.Length>0){var ê=GridTerminalSystem.
GetBlockGroupWithName(ReactorGroup);ê.GetBlocksOfType<IMyReactor>(ȕ,(ì)=>ì.IsSameConstructAs(Me));}else{GridTerminalSystem.GetBlocksOfType<
IMyReactor>(ȕ,(ì)=>ì.IsSameConstructAs(Me));}for(int E=0;E<waitTicks;++E)yield return true;double î=0;double ï=0;foreach(
IMyBatteryBlock ð in ȉ){if(SkipRecharge&&ð.ChargeMode==ChargeMode.Recharge)continue;î+=ð.CurrentStoredPower;ï+=ð.MaxStoredPower;}for(
int E=0;E<waitTicks;++E)yield return true;double ñ=î/ï;if(ñ>RatioHigh&&!ȋ){í(ȕ,false);ǯ="Reactors OFF";Ȋ=false;ȋ=true;}for(
int E=0;E<waitTicks;++E)yield return true;if(ñ<RatioLow&&!Ȋ){í(ȕ,true);ǯ="Reactors ON";Ȋ=true;ȋ=false;}for(int E=0;E<
waitTicks;++E)yield return true;string ò=$"Usable Energy: {ñ*100:0.#}%\nLast Action: "+ǯ+"\n";Ǯ=ò;for(int E=0;E<waitTicks;++E)
yield return true;}else{Ǯ="PowerManagement OFF\n";for(int E=0;E<waitTicks;++E)yield return true;}if(haveLCD)Ƙ();}IEnumerator<
bool>ó(){ǭ="Auto Disassembling";if(AutoDisassemble||DisassembleOverstock){ȍ=new List<string>();var V=UnwantedItems.ToString(
).Split(';');foreach(var A in V){var ô=A.Trim();if(!string.IsNullOrEmpty(ô)&&!ȍ.Contains(ô))ȍ.Add(ô);}for(int E=0;E<
waitTicks;++E)yield return true;ė();for(int E=0;E<waitTicks;++E)yield return true;Ȱ.Clear();GridTerminalSystem.GetBlocksOfType(Ȱ)
;for(int E=0;E<waitTicks;++E)yield return true;foreach(var Ė in Ȱ){ȧ=Ė.GetInventory();ȱ.Clear();ȧ.GetItems(ȱ);if(ȧ==null
||ȧ.ItemCount==0)continue;foreach(MyInventoryItem A in ȱ){if(AutoDisassemble&&A.Amount>=1&&ȍ.Contains(ċ(A.Type))){Ę(ȧ,A);}
if(DisassembleOverstock){foreach(var y in Ƭ){if(y.Key==ċ(A.Type)){var z=N.ContainsKey(y.Key)?N[y.Key]:0;var X=ƭ.
ContainsKey(y.Key)?ƭ[y.Key]:0;if(z>y.Value){var Y=(z+X)-y.Value>A.Amount?A.Amount:(z+X)-y.Value;Ę(ȧ,A,Y);N[y.Key]=(z+X)-Y;z=(z+X)-Y
;}}}}if(haveLCD)Ƙ();for(int E=0;E<waitTicks;++E)yield return true;}}for(int E=0;E<waitTicks;++E)yield return true;foreach
(IMyAssembler D in Ȏ){if(D.IsQueueEmpty)D.Mode=MyAssemblerMode.Assembly;}}}void ė(){Ȏ=ȟ.FindAll(Ò=>Ò.CustomName.Contains(
Disassembler));if(Ȏ.Count<=0){Echo($"No disassemblers found");}if(ǳ){foreach(var D in Ȏ){if(D.Mode!=MyAssemblerMode.Disassembly){D.
Mode=MyAssemblerMode.Disassembly;D.ClearQueue();}}ǳ=false;}}void Ę(IMyInventory J,MyInventoryItem A){var E=0;MyFixedPoint Y=
A.Amount;int B=(int)A.Amount/Ȏ.Count;int C=(int)A.Amount%Ȏ.Count;foreach(IMyAssembler D in Ȏ){Y=B;E++;if(E>=Ȏ.Count){Y=B+
C;E=0;}if(Y>=1){if(D.Mode!=MyAssemblerMode.Disassembly){D.Mode=MyAssemblerMode.Disassembly;D.ClearQueue();}if(D.
OutputInventory.CanItemsBeAdded(Y,A.Type)){if(D.CanUseBlueprint(ą(ċ(A.Type)))){try{J.TransferItemTo(D.OutputInventory,A,Y);D.
AddQueueItem(ą(ċ(A.Type)),Y);ȏ++;}catch{Echo($"Failure disassembling item: {A.Type}");if(Debug)ȣ.WriteText(
$"Failure disassembling item: {A.Type}\n{ċ(A.Type)} does not exist\n",true);}}else{Echo(
$"Failure disassembling item: {A.Type} in {D.CustomName}\nAre you sure this assembler can process this blueprint?");if(Debug)ȣ.WriteText(
$"Failure disassembling item: {A.Type} in {D.CustomName}\nAre you sure this assembler can process this blueprint?",true);}}}}}void Ę(IMyInventory J,MyInventoryItem A,MyFixedPoint Ü){var E=0;MyFixedPoint Y=Ü;int B=(int)Ü/Ȏ.Count;int C=
(int)Ü%Ȏ.Count;foreach(IMyAssembler D in Ȏ){Y=B;E++;if(E>=Ȏ.Count){Y=B+C;E=0;}if(Y>=1){if(D.Mode!=MyAssemblerMode.
Disassembly){D.Mode=MyAssemblerMode.Disassembly;D.ClearQueue();}if(D.OutputInventory.CanItemsBeAdded(Y,A.Type)){if(D.
CanUseBlueprint(ą(ċ(A.Type)))){try{J.TransferItemTo(D.OutputInventory,A,Y);D.AddQueueItem(ą(ċ(A.Type)),Y);ȏ++;}catch{Echo(
$"Failure disassembling item: {A.Type}");if(Debug)ȣ.WriteText($"Failure disassembling item: {A.Type}\n{ċ(A.Type)} does not exist\n",true);}}else{Echo(
$"Failure disassembling item: {A.Type} in {D.CustomName}\nAre you sure this assembler can process this blueprint?");if(Debug)ȣ.WriteText(
$"Failure disassembling item: {A.Type} in {D.CustomName}\nAre you sure this assembler can process this blueprint?",true);}}}}}static string ċ(MyItemType Ô){return Ô.SubtypeId;}static MyDefinitionId ą(string A){var Ć=
"MyObjectBuilder_BlueprintDefinition/";switch(A){case"Construction":case"Computer":case"Motor":case"Girder":case"GravityGenerator":case"Explosives":case
"Detector":case"Medical":case"Radio":case"Reactor":case"Thrust":case"RadioCommunication":case"HackingChip":case"Mainframe":Ć+=A+
"Component";break;case"NATO_25x184mm":case"NATO_5p56x45mm":Ć+=A+"Magazine";break;case"AngleGrinderItem":case"HandDrillItem":case
"WelderItem":case"AngleGrinder2Item":case"HandDrill2Item":case"Welder2Item":case"AngleGrinder4Item":case"HandDrill4Item":case
"Welder4Item":case"AngleGrinder3Item":case"HandDrill3Item":case"Welder3Item":case"AdvancedHandHeldLauncherItem":case
"AutomaticRifleItem":case"UltimateAutomaticRifleItem":case"RapidFireAutomaticRifleItem":case"PreciseAutomaticRifleItem":case
"BasicHandHeldLauncherItem":case"SemiAutoPistolItem":case"FullAutoPistolItem":Ć+=A.Replace("Item","");break;case"ShieldComponent":Ć+=A+"BP";break;
case"ElitePistolItem":Ć+=A.Replace("ElitePistolItem","EliteAutoPistol");break;default:Ć+=A;break;}return MyDefinitionId.
Parse(Ć);}static string ć(MyDefinitionId Ć){var Ö=Ć.SubtypeId.ToString();switch(Ö){case"GravityGeneratorComponent":case
"ExplosivesComponent":case"DetectorComponent":case"MedicalComponent":case"RadioComponent":case"ReactorComponent":case"ThrustComponent":case
"ConstructionComponent":case"ComputerComponent":case"MotorComponent":case"GirderComponent":case"RadioCommunicationComponent":case
"HackingChipComponent":case"MainframeComponent":Ö=Ö.Replace("Component","");break;case"ShieldComponentBP":Ö=Ö.Replace("BP","");break;case
"NATO_25x184mmMagazine":case"NATO_5p56x45mmMagazine":Ö=Ö.Replace("Magazine","");break;case"AngleGrinder":case"HandDrill":case"Welder":case
"AngleGrinder2":case"HandDrill2":case"Welder2":case"AngleGrinder4":case"HandDrill4":case"Welder4":case"AngleGrinder3":case"HandDrill3":
case"Welder3":case"AdvancedHandHeldLauncher":case"AutomaticRifle":case"UltimateAutomaticRifle":case"RapidFireAutomaticRifle"
:case"PreciseAutomaticRifle":case"BasicHandHeldLauncher":case"SemiAutoPistol":case"FullAutoPistol":Ö+="Item";break;case
"EliteAutoPistol":Ö="ElitePistolItem";break;}return Ö;}void Ĉ(string ĉ){if(Ȣ!=null){Ȣ.FontSize=autocraftingfontSize;Ȣ.WriteText("",false)
;Ȣ.WritePublicTitle("WIM Autocrafting",false);ơ(Ȣ,"WIM Autocrafting");ơ(Ȣ,"==============");ơ(Ȣ,ĉ);}}IEnumerator<bool>Ċ()
{ǭ="AutoCrafting";Ǳ=String.Empty;ǰ=String.Empty;for(int E=0;E<waitTicks;++E)yield return true;Ƭ.Clear();N.Clear();ƭ.Clear
();W(N);if(Ȑ>=25){º();Ȑ=0;}Ȑ++;for(int E=0;E<waitTicks;++E)yield return true;if(Ȣ==null){Ȣ=(IMyTextPanel)
GridTerminalSystem.GetBlockWithName(LCDNameAutocrafting);if(Ȣ==null){Echo($"no display named {LCDNameAutocrafting} found");Runtime.
UpdateFrequency|=UpdateFrequency.None;}Ȣ.ContentType=ContentType.SCRIPT;Ȣ.Script="";Ȣ.Font="Monospace";Ȣ.FontSize=autocraftingfontSize;
}if(!Ȓ){Ƥ();Ȓ=true;}ȇ.Clear();ȇ=ȗ.Where(y=>!Õ(y.Key)).Union(ǻ.Where(y=>!Õ(y.Key))).ToDictionary(Č=>Č.Key,č=>č.Value);if(!
ȑ){foreach(var Ď in ȇ.OrderBy(ð=>ð.Key)){Ǳ+=Ø(Ď.Key,Ď.Value.ToString(),Ď.Value.ToString());ǰ+=
$"{Ď.Key}|{Ď.Value}|{Ď.Value};";}Ĉ(Ǳ);ȑ=true;for(int E=0;E<waitTicks;++E)yield return true;}else{var ď=Ȣ.GetText();var Đ=ď.Split('\n');var đ=new List<
string>();foreach(var A in Đ.OrderBy(Ē=>Ē).ToList()){if(A.Contains('/')){var ē=F(A);var Ĕ=ē.Split('/');if(Ĕ.Length>=2){var ĕ=Ĕ
[0].Split(' ');đ.Add(ĕ[0].Trim());var Q=ĕ[1].Trim().ToString().Split('.');int Z;int.TryParse(Q[0],out Z);Ƭ.Add(ĕ[0].Trim(
),int.Parse(Ĕ[1]));var X=ƭ.ContainsKey(ĕ[0])?ƭ[ĕ[0]]:0;if(Debug)if(X>0)ȣ.WriteText($"{ĕ[0]}|(A:{X}) {Z}|{Ĕ[1].Trim()};\n"
,true);var a=N.FirstOrDefault(e=>e.Key==ĕ[0]);if(a.Key!=null){Z=(int)a.Value;}else{Z=0;}if(X>0){ǰ+=
$"{ĕ[0]}|(A:{X}) {Z}|{Ĕ[1].Trim()};";}else{ǰ+=$"{ĕ[0]}|{Z}|{Ĕ[1].Trim()};";}Ǳ+=Ø(ĕ[0],(Z+X).ToString(),Ĕ[1].Trim());}}for(int E=0;E<waitTicks;++E)yield
return true;}if(haveLCD)Ƙ();for(int E=0;E<waitTicks*waitTicksMultiplier;++E)yield return true;var f=ȇ.Keys.ToList();var h=f.
Except(đ).ToList();foreach(var l in h){ǰ+=$"{l}|{ȇ[l]}|{ȇ[l]};";Ǳ+=Ø(l,ȇ[l].ToString(),ȇ[l].ToString());}for(int E=0;E<
waitTicks*waitTicksMultiplier;++E)yield return true;Ĉ(Ǳ);for(int E=0;E<waitTicks*waitTicksMultiplier;++E)yield return true;Save()
;for(int E=0;E<waitTicks;++E)yield return true;}Ȅ=Ȣ.ScriptForegroundColor;Ȣ.ContentType=ContentType.SCRIPT;Ȣ.Script="";ȁ=
new RectangleF((Ȣ.TextureSize-Ȣ.SurfaceSize)/2f,Ȣ.SurfaceSize);Ā=Ȣ.DrawFrame();var m=new Vector2(5,5)+ȁ.Position;Ȃ=new
Vector2(Ȣ.SurfaceSize.X-10,0);ȃ=new Vector2(0,30*autocraftingfontSize);Ⱦ(ref Ā,ref m,Ȣ);for(int E=0;E<2;++E)yield return true;Ā
.Dispose();for(int E=0;E<2;++E)yield return true;Ā=Ȣ.DrawFrame();var µ=DateTime.Now;var r=µ.AddHours(TimeDifference);var
u=r.ToString("H:mm:ss");ÿ(ref Ā,u,m+Ȃ,TextAlignment.RIGHT);for(int E=0;E<2;++E)yield return true;ɏ(ref Ā,ref m,Ȣ,ǰ);for(
int E=0;E<2;++E)yield return true;Ā.Dispose();Ñ();for(int E=0;E<waitTicks;++E)yield return true;var w=new List<ư>();foreach
(var y in Ƭ){try{var X=ƭ.ContainsKey(y.Key)?ƭ[y.Key]:0;var z=N.ContainsKey(y.Key)?N[y.Key]:0;if(z<y.Value){var ª=new ư();
ª.Ʋ=y.Key;ª.Ƽ=y.Value-(z+X);ª.Ƴ=(float)z/y.Value;w.Add(ª);}}catch{Echo($"Failure processing item: {y.Key}");throw;}for(
int E=0;E<waitTicks;++E)yield return true;}for(int E=0;E<waitTicks;++E)yield return true;foreach(var A in w.OrderBy(o=>o.Ƴ)
){MyFixedPoint Y=(MyFixedPoint)((double)A.Ƽ/ƫ.Count);var L=0;int B=(int)A.Ƽ/ƫ.Count;int C=(int)A.Ƽ%ƫ.Count;foreach(
IMyAssembler D in ƫ){Y=B;L++;if(L>=ƫ.Count){Y=B+C;L=0;}if(Y>=1){try{if(D.CanUseBlueprint(ą(A.Ʋ))){D.AddQueueItem(ą(A.Ʋ),Y);}else{
Echo($"Failure queue'ing item: {A.Ʋ} in {D.CustomName}\n Are you sure this assembler can craft this?");if(Debug){ȣ.WriteText
($"Failure queue'ing item: {A.Ʋ} in {D.CustomName}\n{ą(A.Ʋ)} Are you sure this assembler can craft this?\n",true);}}}
catch{Echo($"Failure processing item: {A.Ʋ}");if(Debug){ȣ.WriteText(
$"Failure assembling item: {A.Ʋ}\n{ą(A.Ʋ)} does not exist\n",true);}}}for(int E=0;E<waitTicks;++E)yield return true;}}for(int E=0;E<waitTicks;++E)yield return true;w.Clear();}
static string F(string G){int H=G.Length,I=0,E=0;var J=G.ToCharArray();bool K=false;char M;for(;E<H;E++){M=J[E];switch(M){case
'\u0020':case'\u00A0':case'\u1680':case'\u2000':case'\u2001':case'\u2002':case'\u2003':case'\u2004':case'\u2005':case'\u2006':
case'\u2007':case'\u2008':case'\u2009':case'\u200A':case'\u202F':case'\u205F':case'\u3000':case'\u2028':case'\u2029':case
'\u0009':case'\u000A':case'\u000B':case'\u000C':case'\u000D':case'\u0085':if(K)continue;J[I++]=M;K=true;continue;default:K=false
;J[I++]=M;continue;}}return new string(J,0,I);}void W(Dictionary<string,MyFixedPoint>N){foreach(var O in Ǡ){if(!O.
HasInventory)continue;var P=O.GetInventory();if(O is IMyProductionBlock){if(O is IMyAssembler&&((IMyAssembler)O).Mode==
MyAssemblerMode.Disassembly){P=((IMyProductionBlock)O).InputInventory;}else{P=((IMyProductionBlock)O).OutputInventory;}}else{if(O==null
)continue;P=O.GetInventory();}if(P==null||P.ItemCount==0)continue;for(int E=0;E<P.ItemCount;E++){var A=P.GetItemAt(E);if(
!A.HasValue||!Ó(A.Value.Type))continue;var Q=A.Value.Amount.ToString().Split('.');int R;int.TryParse(Q[0],out R);var S=ċ(
A.Value.Type);Û(N,S,R);}}U(ƭ);}void U(Dictionary<string,MyFixedPoint>N){ȟ.Clear();GridTerminalSystem.GetBlocksOfType(ȟ);
foreach(var D in ȟ){if(D.IsQueueEmpty)continue;if(D.Mode==MyAssemblerMode.Disassembly)continue;var V=new List<MyProductionItem>
();D.GetQueue(V);foreach(var A in V){var S=ć(A.BlueprintId);Û(N,S,A.Amount);}}}void º(){foreach(IMyAssembler D in ƫ){if(D
.IsQueueEmpty)continue;D.ClearQueue();}}void Ñ(){ƫ=ȟ.FindAll(Ò=>Ò.CustomName.Contains(Assembler));if(ƫ.Count<=0){Echo(
$"No assemblers found");}if(ǲ){foreach(var D in ƫ){if(D.Mode!=MyAssemblerMode.Assembly){D.Mode=MyAssemblerMode.Assembly;D.ClearQueue();}}ǲ=
false;}}static bool Ó(MyItemType Ô){if(Ô.SubtypeId.Contains("ClangCola")||Ô.SubtypeId.Contains("CosmicCoffee")||Ô.SubtypeId.
Contains("Medkit")||Ô.SubtypeId.Contains("Package")||Ô.SubtypeId.Contains("Powerkit")||Ô.SubtypeId.Contains("SpaceCredit")||Ô.
SubtypeId.Contains("SEDEOreCredit")||Ô.SubtypeId.Contains("ZoneChip")||Ô.SubtypeId.Contains("NATO_5p56x45mm"))return false;if(Ô.
TypeId.Contains("Component"))return true;if(Ô.TypeId.Contains("AmmoMagazine"))return true;if(Ô.TypeId.Contains(
"PhysicalGunObject"))return true;if(Ô.TypeId.Contains("ContainerObject"))return true;else return false;}static bool Õ(string Ö){if(Ö.
Contains("ClangCola")||Ö.Contains("CosmicCoffee")||Ö.Contains("Medkit")||Ö.Contains("Package")||Ö.Contains("Powerkit")||Ö.
Contains("SpaceCredit")||Ö.Contains("SEDEOreCredit")||Ö.Contains("ZoneChip")||Ö.Contains("NATO_5p56x45mm")){return true;}else{
return false;}}static string Ø(string Ù,string N,string Ú){return$"{Ù} {N} / {Ú}\n";}static void Û(Dictionary<string,
MyFixedPoint>N,string A,MyFixedPoint Ü){if(!N.ContainsKey(A))N.Add(A,0);N[A]+=Ü;}static string Ý(string Þ,int H){while(Þ.Length<H)Þ
+=" ";return Þ;}class ß{public string à{get;set;}public double á{get;set;}public ß(string Á,double Ã){à=Á;á=Ã;}}
IEnumerator<bool>â(){ǭ="Checking Most Wanted...";ű();foreach(KeyValuePair<string,double>Ð in Ȳ){string Á=Ð.Key;double À=Ð.Value;if(
P.ContainsKey(Á)){P[Á]+=À;}}foreach(KeyValuePair<string,double>Ð in ȴ){string Å=Ð.Key;double À=Ð.Value;string Á=Å.Replace
(" Ingot","");double Â=conversionRates["Unknown"];conversionRates.TryGetValue(Á,out Â);if(P.ContainsKey(Á)){P[Á]+=À/Â;}}
foreach(string Á in oreTypes){double Â=conversionRates["Unknown"];conversionRates.TryGetValue(Á,out Â);double Ã=P[Á];double Ä=(
Ã*Â);string Å=Á;if(ǩ.ContainsKey(Å)){ǩ[Å]=Ä;}}string Æ=Ƞ.CustomData;Ǫ=ŵ(Æ);foreach(KeyValuePair<string,double>Ç in Ǫ){
string Å=Ç.Key;double È=Ç.Value;double É=ǩ.ContainsKey(Å)?ǩ[Å]:0;double Ì=È-É;if(ǫ.ContainsKey(Å)){ǫ[Å]=Ì;}}List<ß>Ê=new List<
ß>(3){new ß(null,0),new ß(null,0),new ß(null,0),};foreach(KeyValuePair<string,double>Ë in ǫ){string Å=Ë.Key;double Ì=Ë.
Value;string Á=Å.Replace(" Ingot","");double Â=conversionRates["Unknown"];conversionRates.TryGetValue(Á,out Â);double Ã=(Ì/Â)
;bool Í=false;for(int E=0;E<3;++E){bool Î=P[Á]<=lowOreThreshold;bool Ï=Ê[E].à!=null&&P[Ê[E].à]<=lowOreThreshold;if(!Í&&(!
Ï||Î)&&(Ê[E].à==null||Ê[E].á<Ã)){Ê.Insert(E,new ß(Á,Ã));Ê.RemoveAt(3);Í=true;break;}}}ȁ=new RectangleF((Ƞ.TextureSize-Ƞ.
SurfaceSize)/2f,Ƞ.SurfaceSize);Ā=Ƞ.DrawFrame();Ƞ.WriteText("");var m=new Vector2(5,5)+ȁ.Position;Ȃ=new Vector2(Ƞ.SurfaceSize.X-10,0
);ȃ=new Vector2(0,30*wantedfontSize);var µ=DateTime.Now;var r=µ.AddHours(TimeDifference);var u=r.ToString("H:mm:ss");ÿ(
ref Ā,u,m+Ȃ,TextAlignment.RIGHT);var ů="";foreach(var Ű in Ê){if(Ű.à!=null){ů+=$"{Ű.à}\n";}}Ɉ(ref Ā,ref m,Ƞ,ů);Ā.Dispose();
for(int E=0;E<waitTicks;++E)yield return true;}void ű(){P.Clear();ǩ.Clear();ǫ.Clear();foreach(string Ų in oreTypes){P.Add(Ų
.Trim(),0);ǩ.Add(Ų.Trim(),0);ǫ.Add(Ų.Trim(),0);}}string ų(Dictionary<string,double>ņ){StringBuilder Ŵ=new StringBuilder()
;foreach(KeyValuePair<string,double>y in ņ){Ŵ.Append($"{y.Key}: {y.Value}\n");}return Ŵ.ToString();}Dictionary<string,
double>ŵ(string Ŷ){Dictionary<string,double>ņ=new Dictionary<string,double>();string[]ŷ=Ŷ.Split(new[]{'\n'},StringSplitOptions
.RemoveEmptyEntries);foreach(string ú in ŷ){string[]ż=ú.Split(':');string S=ż[0].Trim();double Q=double.Parse(ż[1].Trim()
);ņ.Add(S,Q);}return ņ;}void Ÿ(string[]Ź){Ȕ.Clear();Ȗ.Clear();foreach(string ź in Ź){if(ź.StartsWith("+")){string Ų=ź.
Substring(1).Trim();if(Ų.Equals("All",StringComparison.OrdinalIgnoreCase)){foreach(string Á in oreTypes){Ȕ.Add(Á);}}else{Ȕ.Add(Ų)
;}}else if(ź.StartsWith("-")){string Ų=ź.Substring(1).Trim();if(Ų.Equals("All",StringComparison.OrdinalIgnoreCase)){
foreach(string Á in oreTypes){Ȗ.Add(Á);}}else{Ȗ.Add(Ų);}}else if(ź.StartsWith("|")){string Ų=ź.Substring(1).Trim();if(Ų.Equals(
"All",StringComparison.OrdinalIgnoreCase)){foreach(string Á in oreTypes){ȳ.Add(Á);}}else{ȳ.Add(Ų);}}}}IEnumerator<bool>Ż(){
var µ=DateTime.Now;ǭ="Refinery Manager";if(ȓ){ƪ.Clear();Ʃ.Clear();ƅ.Clear();Ǟ.Clear();Ʀ.Clear();Ƨ.Clear();foreach(string Ų
in oreTypes){ƪ.Add(Ų.Trim());}GridTerminalSystem.GetBlocksOfType<IMyRefinery>(ƅ);GridTerminalSystem.GetBlocksOfType<
IMyCargoContainer>(Ǟ);GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(Ʀ);for(int E=0;E<ƅ.Count;E++){if(ƅ[E].CustomName.Contains("+")
||ƅ[E].CustomName.Contains("-")||ƅ[E].CustomName.Contains("|"))Ƨ.Add(ƅ[E]as IMyRefinery);}if(Ƨ.Count==0)yield break;Ů();
for(int E=0;E<waitTicks;++E)yield return true;foreach(IMyRefinery Ŧ in Ƨ){Ȕ.Clear();Ȗ.Clear();ȳ.Clear();var ŧ=Ŧ.
GetInventory(0);List<MyInventoryItem>Ũ=new List<MyInventoryItem>();ŧ.GetItems(Ũ,null);if(Ŧ.UseConveyorSystem)Ŧ.UseConveyorSystem=
false;if(Ɖ(ŧ))ƨ.Add(Ŧ);else Ŧ.Enabled=true;string[]ũ=Ŧ.CustomName.Split(' ');Ÿ(ũ);for(int Č=0;Č<waitTicks;++Č)yield return
true;for(int L=0;L<ƪ.Count;L++){Ȕ.Remove(ƪ[L]);}for(int Č=0;Č<waitTicks;++Č)yield return true;for(int L=0;L<Ũ.Count;L++){
string Ū=Ũ[L].Type.SubtypeId;var Ô=Ũ[L].Type.TypeId.Split('_')[1];if(Ȗ.Contains(Ū)){continue;}if(Ŧ.CustomName.Contains("+"+Ū))
{List<IMyRefinery>ū;if(!Ʃ.TryGetValue(Ū,out ū))Ʃ.Add(Ū,ū=new List<IMyRefinery>());Ƅ(ū,Ŧ);Ȕ.Add(Ū);}}for(int Č=0;Č<
waitTicks;++Č)yield return true;for(int L=0;L<Ȕ.Count;L++){List<IMyRefinery>ū;if(!Ʃ.TryGetValue(Ȕ[L],out ū))Ʃ.Add(Ȕ[L],ū=new List
<IMyRefinery>());Ƅ(ū,Ŧ);}if(Ȕ.Count==0&&Ȗ.Count>0){for(int Č=0;Č<oreTypes.Length;Č++){if(Ȗ.Contains(oreTypes[Č]))continue
;List<IMyRefinery>ū;if(!Ʃ.TryGetValue(oreTypes[Č],out ū))Ʃ.Add(oreTypes[Č],ū=new List<IMyRefinery>());Ƅ(ū,Ŧ);Ȕ.Add(
oreTypes[Č]);}}if(Ȕ.Count==0){for(int Č=0;Č<ȳ.Count;Č++){List<IMyRefinery>ū;if(!Ʃ.TryGetValue(ȳ[Č],out ū))Ʃ.Add(ȳ[Č],ū=new List<
IMyRefinery>());Ƅ(ū,Ŧ);}}List<IMyRefinery>Ŭ=new List<IMyRefinery>();if(Ʃ.ContainsKey("All")){if(Ʃ.TryGetValue("All",out Ŭ)){for(int
L=0;L<Ŭ.Count;L++){for(int Č=0;Č<oreTypes.Length;Č++){if(Ȗ.Contains(oreTypes[Č])){continue;}List<IMyRefinery>ū;if(!Ʃ.
TryGetValue(oreTypes[Č],out ū))Ʃ.Add(oreTypes[Č],ū=new List<IMyRefinery>());Ƅ(ū,Ŭ[L]);}}}}Ɠ(Ŧ);for(int Č=0;Č<waitTicks;++Č)yield
return true;if(haveLCD)Ƙ();}ƒ();for(int E=0;E<waitTicks;++E)yield return true;for(int E=0;E<ƨ.Count;E++){var ŭ=(ƨ[E]as
IMyEntity).GetInventory(0);if(Ɖ(ŭ))ƨ[E].Enabled=false;}}if(haveLCD)Ƙ();if(Debug){ȣ.WriteText($"Refineries done\n",true);}for(int
E=0;E<waitTicks;++E)yield return true;}void Ů(){ƪ.Clear();foreach(string Ų in oreTypes){ƪ.Add(Ų.Trim());}for(int E=0;E<Ǟ.
Count;E++){var Ǝ=(Ǟ[E]as IMyEntity).GetInventory(0);List<MyInventoryItem>Ə=new List<MyInventoryItem>();Ǝ.GetItems(Ə,null);for
(int L=0;L<Ə.Count;L++){string Ū=Ə[L].Type.SubtypeId;var Ô=Ə[L].Type.TypeId.Split('_')[1];if(ƪ.Contains(Ū)&&Ô.Contains(
"Ore"))ƪ.Remove(Ū);}}for(int E=0;E<Ʀ.Count;E++){var Ɛ=(Ʀ[E]as IMyEntity).GetInventory(0);List<MyInventoryItem>Ƒ=new List<
MyInventoryItem>();Ɛ.GetItems(Ƒ,null);for(int L=0;L<Ƒ.Count;L++){string Ū=Ƒ[L].Type.SubtypeId;var Ô=Ƒ[L].Type.TypeId.Split('_')[1];if(ƪ
.Contains(Ū)&&Ô.Contains("Ore"))ƪ.Remove(Ū);}}}void ƒ(){if(Ʃ.Count==0)return;for(int E=0;E<Ǟ.Count;E++){var Ǝ=Ǟ[E].
GetInventory(0);List<MyInventoryItem>Ə=new List<MyInventoryItem>();Ǝ.GetItems(Ə,null);for(int L=Ə.Count-1;L>=0;L--){string Ū=Ə[L].
Type.SubtypeId;var Ô=Ə[L].Type.TypeId.Split('_')[1];if(Ʃ.ContainsKey(Ū)&&Ô.Contains("Ore")&&!Ȗ.Contains(Ū)){ž(Ʃ[Ū],Ǝ,Ə[L],L)
;}}}for(int E=0;E<Ʀ.Count;E++){var Ɛ=Ʀ[E].GetInventory(0);List<MyInventoryItem>Ƒ=new List<MyInventoryItem>();Ɛ.GetItems(Ƒ
,null);for(int L=Ƒ.Count-1;L>=0;L--){string Ū=Ƒ[L].Type.SubtypeId;var Ô=Ƒ[L].Type.TypeId.Split('_')[1];if(Ʃ.ContainsKey(Ū
)&&Ô.Contains("Ore")&&!Ȗ.Contains(Ū)){ž(Ʃ[Ū],Ɛ,Ƒ[L],L);}}}}void Ɠ(IMyRefinery Ŧ){IMyInventory Ɣ=null;for(int Ɩ=0;Ɩ<Ǟ.
Count;Ɩ++){var Ǝ=(Ǟ[Ɩ]as IMyEntity).GetInventory(0);if(!ƈ(Ǝ)){Ɣ=Ǝ;break;}}if(Ɣ==null)return;var Ž=(Ŧ as IMyEntity).
GetInventory(0);List<MyInventoryItem>Ũ=new List<MyInventoryItem>();Ž.GetItems(Ũ,null);for(int L=Ũ.Count-1;L>=0;L--){string Ū=Ũ[L].
Type.SubtypeId;List<IMyRefinery>ū;if(Ʃ.TryGetValue(Ū,out ū)){if(!ū.Contains(Ŧ)){Ž.TransferItemTo(Ɣ,L,null,true,null);
continue;}}else{Ž.TransferItemTo(Ɣ,L,null,true,null);continue;}if(Ʃ.Count==0){Ž.TransferItemTo(Ɣ,L,null,true,null);}if(Ȗ.
Contains(Ū)){Ž.TransferItemTo(Ɣ,L,null,true,null);}}}void ž(List<IMyRefinery>ſ,IMyInventory ƀ,MyInventoryItem A,int Ɓ){string Ū=
A.Type.SubtypeId;string Ô=A.Type.TypeId.Split('_')[1];float Ü=(float)A.Amount;if(Ȗ.Contains(Ū)){return;}if(!Ʃ.ContainsKey
(Ū)||!Ô.Contains("Ore")){return;}if(Ü<1){VRage.MyFixedPoint Ƃ=(VRage.MyFixedPoint)(Ü);ƀ.TransferItemTo((ſ[0]as IMyEntity)
.GetInventory(0),Ɓ,null,true,Ƃ);return;}float ƃ=0;if(ſ.Count==1)ƃ=Ü;else ƃ=Ü/ſ.Count;VRage.MyFixedPoint ƌ=(VRage.
MyFixedPoint)(ƃ);for(int E=0;E<ſ.Count;E++){if(ſ[E].CustomName.Contains("-"+Ū)){continue;}ƀ.TransferItemTo((ſ[E]as IMyEntity).
GetInventory(0),Ɓ,null,true,ƌ);}}void Ƅ(List<IMyRefinery>ƅ,IMyRefinery Ŧ){if(!ƅ.Contains(Ŧ))ƅ.Add(Ŧ);}float Ɔ(IMyInventory Ƈ){return
((float)Ƈ.CurrentVolume/(float)Ƈ.MaxVolume)*100f;}bool ƈ(IMyInventory Ƈ){if(Ɔ(Ƈ)>=CargoOverFlowMax)return true;else
return false;}bool Ɖ(IMyInventory Ƈ){if((float)Ƈ.CurrentVolume>0)return false;else return true;}
}class Ɗ{private Action<ICollection<MyDefinitionId>>Ƌ;private Action<ICollection<MyDefinitionId>>ƍ;private Action<
ICollection<MyDefinitionId>>ƕ;private Action<IMyTerminalBlock,IDictionary<MyDetectedEntityInfo,float>>ť;private Action<
IMyTerminalBlock,ICollection<MyDetectedEntityInfo>>Ť;private Func<long,int,MyDetectedEntityInfo>Į;private Func<IMyTerminalBlock,long,int
,bool>į;private Func<IMyTerminalBlock,int,MyDetectedEntityInfo>İ;private Action<IMyTerminalBlock,long,int>ı;private
Action<IMyTerminalBlock,bool,int>Ĳ;private Action<IMyTerminalBlock,bool,bool,int>ĳ;private Func<IMyTerminalBlock,int,bool,bool
,bool>Ĵ;private Func<IMyTerminalBlock,int,float>ĵ;private Func<IMyTerminalBlock,long,int,bool>Ķ;private Func<
IMyTerminalBlock,long,int,MyTuple<bool,VRageMath.Vector3D?>>ķ;private Func<IMyTerminalBlock,long,int,bool>ĸ;private Func<
IMyTerminalBlock,long,int,VRageMath.Vector3D?>Ĺ;private Func<long,bool>Ļ;private Func<IMyTerminalBlock,bool>ň;private Func<long,float>ļ;
private Func<IMyTerminalBlock,int,string>Ľ;private Action<IMyTerminalBlock,int,string>ľ;private Func<long,float>Ŀ;private Func<
IMyTerminalBlock,long>ŀ;private Func<IMyTerminalBlock,long,bool,bool,bool>Ł;private Func<IMyTerminalBlock,int,MyTuple<VRageMath.Vector3D
,VRageMath.Vector3D>>ł;private Func<IMyTerminalBlock,MyTuple<bool,bool>>Ń;public bool ń(IMyTerminalBlock Ņ){var ņ=Ņ.
GetProperty("WcPbAPI")?.As<IReadOnlyDictionary<string,Delegate>>().GetValue(Ņ);if(ņ==null)throw new Exception(
"WcPbAPI failed to activate");return Ň(ņ);}public bool Ň(IReadOnlyDictionary<string,Delegate>Ģ){if(Ģ==null)return false;ĺ(Ģ,"GetCoreWeapons",ref Ƌ);
ĺ(Ģ,"GetCoreStaticLaunchers",ref ƍ);ĺ(Ģ,"GetCoreTurrets",ref ƕ);ĺ(Ģ,"GetSortedThreats",ref ť);ĺ(Ģ,"GetObstructions",ref Ť
);ĺ(Ģ,"GetAiFocus",ref Į);ĺ(Ģ,"SetAiFocus",ref į);ĺ(Ģ,"GetWeaponTarget",ref İ);ĺ(Ģ,"SetWeaponTarget",ref ı);ĺ(Ģ,
"FireWeaponOnce",ref Ĳ);ĺ(Ģ,"ToggleWeaponFire",ref ĳ);ĺ(Ģ,"IsWeaponReadyToFire",ref Ĵ);ĺ(Ģ,"GetMaxWeaponRange",ref ĵ);ĺ(Ģ,
"IsTargetAligned",ref Ķ);ĺ(Ģ,"IsTargetAlignedExtended",ref ķ);ĺ(Ģ,"CanShootTarget",ref ĸ);ĺ(Ģ,"GetPredictedTargetPosition",ref Ĺ);ĺ(Ģ,
"HasGridAi",ref Ļ);ĺ(Ģ,"HasCoreWeapon",ref ň);ĺ(Ģ,"GetOptimalDps",ref ļ);ĺ(Ģ,"GetActiveAmmo",ref Ľ);ĺ(Ģ,"SetActiveAmmo",ref ľ);ĺ(Ģ,
"GetConstructEffectiveDps",ref Ŀ);ĺ(Ģ,"GetPlayerController",ref ŀ);ĺ(Ģ,"IsTargetValid",ref Ł);ĺ(Ģ,"GetWeaponScope",ref ł);ĺ(Ģ,"IsInRange",ref Ń);
return true;}private void ĺ<Ĭ>(IReadOnlyDictionary<string,Delegate>Ģ,string Ö,ref Ĭ Ě)where Ĭ:class{if(Ģ==null){Ě=null;return;
}Delegate ě;if(!Ģ.TryGetValue(Ö,out ě))throw new Exception(
$"{GetType().Name} Couldnt find {Ö} delegate of type {typeof(Ĭ)}");Ě=ě as Ĭ;if(Ě==null)throw new Exception(
$"{GetType().Name} Delegate {Ö} is not type {typeof(Ĭ)} instead its {ě.GetType()}");}public void Ĝ(ICollection<MyDefinitionId>ĝ)=>Ƌ?.Invoke(ĝ);public void Ğ(ICollection<MyDefinitionId>ĝ)=>ƍ?.Invoke(ĝ);
public void ğ(ICollection<MyDefinitionId>ĝ)=>ƕ?.Invoke(ĝ);public void Ġ(IMyTerminalBlock ġ,IDictionary<MyDetectedEntityInfo,
float>ĝ)=>ť?.Invoke(ġ,ĝ);public void ģ(IMyTerminalBlock ġ,ICollection<MyDetectedEntityInfo>ĝ)=>Ť?.Invoke(ġ,ĝ);public
MyDetectedEntityInfo?Ĥ(long ĥ,int Ħ=0)=>Į?.Invoke(ĥ,Ħ);public bool ħ(IMyTerminalBlock ġ,long Ĩ,int Ħ=0)=>į?.Invoke(ġ,Ĩ,Ħ)??false;public
MyDetectedEntityInfo?ĩ(IMyTerminalBlock ę,int Ī=0)=>İ?.Invoke(ę,Ī);public void ī(IMyTerminalBlock ę,long Ĩ,int Ī=0)=>ı?.Invoke(ę,Ĩ,Ī);public
void ř(IMyTerminalBlock ę,bool Ś=true,int Ī=0)=>Ĳ?.Invoke(ę,Ś,Ī);public void ś(IMyTerminalBlock ę,bool Ŝ,bool Ś,int Ī=0)=>ĳ?
.Invoke(ę,Ŝ,Ś,Ī);public bool ŝ(IMyTerminalBlock ę,int Ī=0,bool Ş=true,bool ş=false)=>Ĵ?.Invoke(ę,Ī,Ş,ş)??false;public
float Š(IMyTerminalBlock ę,int Ī)=>ĵ?.Invoke(ę,Ī)??0f;public bool š(IMyTerminalBlock ę,long ŏ,int Ī)=>Ķ?.Invoke(ę,ŏ,Ī)??false
;public MyTuple<bool,VRageMath.Vector3D?>Ţ(IMyTerminalBlock ę,long ŏ,int Ī)=>ķ?.Invoke(ę,ŏ,Ī)??new MyTuple<bool,VRageMath
.Vector3D?>();public bool ţ(IMyTerminalBlock ę,long ŏ,int Ī)=>ĸ?.Invoke(ę,ŏ,Ī)??false;public VRageMath.Vector3D?Ř(
IMyTerminalBlock ę,long ŏ,int Ī)=>Ĺ?.Invoke(ę,ŏ,Ī)??null;public bool ŉ(long Ŋ)=>Ļ?.Invoke(Ŋ)??false;public bool ŋ(IMyTerminalBlock ę)=>ň
?.Invoke(ę)??false;public float Ō(long Ŋ)=>ļ?.Invoke(Ŋ)??0f;public string ō(IMyTerminalBlock ę,int Ī)=>Ľ?.Invoke(ę,Ī)??
null;public void Ŏ(IMyTerminalBlock ę,int Ī,string ŗ)=>ľ?.Invoke(ę,Ī,ŗ);public float Ő(long Ŋ)=>Ŀ?.Invoke(Ŋ)??0f;public long
ő(IMyTerminalBlock ę)=>ŀ?.Invoke(ę)??-1;public bool Œ(IMyTerminalBlock ę,long œ,bool Ŕ,bool ŕ)=>Ł?.Invoke(ę,œ,Ŕ,ŕ)??false
;public MyTuple<VRageMath.Vector3D,VRageMath.Vector3D>Ŗ(IMyTerminalBlock ę,int Ī)=>ł?.Invoke(ę,Ī)??new MyTuple<VRageMath.
Vector3D,VRageMath.Vector3D>();public MyTuple<bool,bool>ǌ(IMyTerminalBlock O)=>Ń?.Invoke(O)??new MyTuple<bool,bool>();