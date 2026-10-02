/* v:2.0207 (Apex Survival compatibility)
* Automatic LCDs 2 - In-game script by MMaster
*
* Thank all of you for making amazing creations with this script, using it and helping each other use it.
* It's been 10 years already since I uploaded first Automatic LCDs script and you are still using it (in "a bit" upgraded form).
* That's just amazing!
*
* LATEST UPDATE: Details command shows status of farm plots more reliably and removes Color tags that don't work in standard LCD text
*  Working command now shows percentage of number of blocks welded on projectors
*  Added title override feature for Oxygen and Tanks commands (check guide for those commands)
*  Some minor optimizations
*  Oxygen command variants OxygenLevel (shows pressure of air vents outside) and OxygenX/OxygenLevelX that doesn't show progress bars.
*  Food items will now be shown in inventory lists even if you have 0
*  Added Food items (check Inventory command guide for full list)
* 
* Previous updates: Look at Change notes tab on Steam workshop page. */

public string VERSION = "2.0207";
/* Customize these: */

// Use this tag to identify LCDs managed by this script
// Name filtering rules can be used here so you can use even G:Group or T:[My LCD]
public string LCD_TAG = "T:[LCD]";

// Set to true to slow down the script
public bool SlowMode = false;

// How many lines to scroll per step
public int SCROLL_LINES = 1;

// if you use custom font scroll down to the bottom, then scroll a bit up until you find AddCharsSize lines

// Enable initial boot sequence
public bool ENABLE_BOOT = true;

// Set to true to stop the script from changing the content type of the screens
public bool SKIP_CONTENT_TYPE = false;

/* READ THIS FULL GUIDE
http://steamcommunity.com/sharedfiles/filedetails/?id=407158161

Basic video guide

https://youtu.be/vqpPQ_20Xso

Please carefully read the FULL GUIDE before asking questions.
Please DO NOT publish this script or its derivations without my permission! Feel free to use it in blueprints!

Special Thanks
Keen Software House for awesome Space Engineers game
Malware for contributing to programmable blocks game code and MDK!
*/
int mdk=1;
// (for developer)
public const int DebugLevel = 0;

// (for modded lcds) Affects all LCDs managed by this programmable block
/* LCD height modifier
0.5f only 1/2 the lines of normal LCD
2.0f 2x more lines on LCD */
public const float HEIGHT_MOD = 1.0f;

/* line width modifier
0.5f move right edge to 50% of normal LCD width
2.0f 200% more text on line */
public const float WIDTH_MOD = 1.0f;

List<string> BOOT_FRAMES = new List<string>() {
/* BOOT FRAMES
* Each @"<text>" marks single frame, add as many as you want each will be displayed for one second
* @"" is multiline string so you can write multiple lines */
@"
Initializing systems"
,
@"
Verifying connections"
,
@"
Loading commands"
};

void ItemsConf() {
// ITEMS AND QUOTAS LIST
// (subType, mainType, quota, display name, short name)
// VANILLA ITEMS
Add("Stone", "Ore");
Add("Iron", "Ore");
Add("Nickel", "Ore");
Add("Cobalt", "Ore");
Add("Magnesium", "Ore");
Add("Silicon", "Ore");
Add("Silver", "Ore");
Add("Gold", "Ore");
Add("Platinum", "Ore");
Add("Uranium", "Ore");
Add("Ice", "Ore");
Add("Scrap", "Ore");
Add("Stone", "Ingot", 40000, "Gravel", "gravel");
Add("Iron", "Ingot", 300000);
Add("Nickel", "Ingot", 200000);
Add("Cobalt", "Ingot", 120000);
Add("Magnesium", "Ingot", 80000);
Add("Silicon", "Ingot", 150000);
Add("Silver", "Ingot", 80000);
Add("Gold", "Ingot", 80000);
Add("Platinum", "Ingot", 45000);
Add("Uranium", "Ingot", 3000);
AddU("PrototechScrap", "Ingot", 0, "Proto. Scrap", "protoscrap");

var t = "Tool";
Add("SemiAutoPistolItem", t, 0, "S-10 Pistol", "s-10");
Add("ElitePistolItem", t, 0, "S-10E Pistol", "s-10e");
Add("FullAutoPistolItem", t, 0, "S-20A Pistol", "s-20a");
Add("AutomaticRifleItem", t, 0, "MR-20 Rifle", "mr-20");
Add("PreciseAutomaticRifleItem", t, 0, "MR-8P Rifle", "mr-8p");
Add("RapidFireAutomaticRifleItem", t, 0, "MR-50A Rifle", "mr-50a");
Add("UltimateAutomaticRifleItem", t, 0, "MR-30E Rifle", "mr-30e");
Add("BasicHandHeldLauncherItem", t, 0, "RO-1 Launcher", "ro-1");
Add("AdvancedHandHeldLauncherItem", t, 0, "PRO-1 Launcher", "pro-1");
Add("WelderItem", t, 0, "Welder");
Add("Welder2Item", t, 0, "* Enh. Welder");
Add("Welder3Item", t, 0, "** Prof. Welder");
Add("Welder4Item", t, 0, "*** Elite Welder");
Add("AngleGrinderItem", t, 0, "Angle Grinder");
Add("AngleGrinder2Item", t, 0, "* Enh. Grinder");
Add("AngleGrinder3Item", t, 0, "** Prof. Grinder");
Add("AngleGrinder4Item", t, 0, "*** Elite Grinder");
Add("HandDrillItem", t, 0, "Hand Drill");
Add("HandDrill2Item", t, 0, "* Enh. Drill");
Add("HandDrill3Item", t, 0, "** Prof. Drill");
Add("HandDrill4Item", t, 0, "*** Elite Drill");
Add("FlareGunItem", t, 0, "Flare Gun");

t = "Component";
Add("Construction", t, 50000);
Add("MetalGrid", t, 15500);
Add("InteriorPlate", t, 55000);
Add("SteelPlate", t, 300000);
Add("Girder", t, 3500);
Add("SmallTube", t, 26000);
Add("LargeTube", t, 6000);
Add("Motor", t, 16000);
Add("Display", t, 500);
Add("BulletproofGlass", t, 12000, "Bulletp. Glass", "bpglass");
Add("Computer", t, 6500);
Add("Reactor", t, 10000);
Add("Thrust", t, 16000, "Thruster", "thruster");
Add("GravityGenerator", t, 250, "GravGen", "gravgen");
Add("Medical", t, 120);
Add("RadioCommunication", t, 250, "Radio-comm", "radio");
Add("Detector", t, 400);
Add("Explosives", t, 500);
Add("SolarCell", t, 2800);
Add("PowerCell", t, 2800);
Add("Superconductor", t, 3000);
Add("Canvas", t, 300);
AddU("PrototechFrame", t, 0, "Proto. Frame", "protoframe");
AddU("PrototechPanel", t, 0, "Proto. Panel", "protopanel");
AddU("PrototechCapacitor", t, 0, "Proto. Capacitor", "protocapacitor");
AddU("PrototechPropulsionUnit", t, 0, "Proto. Prop.Unit", "protopropunit");
AddU("PrototechMachinery", t, 0, "Proto. Machinery", "protomachinery");
AddU("PrototechCircuitry", t, 0, "Proto. Circuitry", "protocircuitry");
AddU("PrototechCoolingUnit", t, 0, "Proto. Cool.Unit", "protocoolunit");
Add("EngineerPlushie", t, 0);
Add("SabiroidPlushie", t, 0);
Add("ZoneChip", t, 100);
Add("Datapad", "Datapad", 0);
Add("Package", "Package", 0);

t = "Object";
Add("SpaceCredit", t, 0);
Add("Algae", t, 0);
Add("Grain", t, 0);

t = "Consumable";
Add("Medkit", t, 0);
Add("Powerkit", t, 0);
Add("ClangCola", t, 0);
Add("CosmicCoffee", t, 0);
Add("MealPack_Burrito", t, 0, "Burrito", "burrito");
Add("MealPack_Chili", t, 0, "Chili", "chili");
Add("MealPack_Curry", t, 0, "Curry", "curry");
Add("MealPack_Dumplings", t, 0, "Dumplings", "dumplings");
Add("MealPack_Flatbread", t, 0, "Flatbread", "flatbread");
Add("MealPack_FrontierStew", t, 0, "Frontier Stew", "frontierstew");
Add("MealPack_FruitBar", t, 0, "Fruit Bar", "fruitbar");
Add("MealPack_FruitPastry", t, 0, "Fruit Pastry", "fruitpastry");
Add("MealPack_GardenSlaw", t, 0, "Garden Slaw", "gardenslaw");
Add("MealPack_GreenPellets", t, 0, "Green Pellets", "greenpellets");
Add("MealPack_KelpCrisp", t, 0, "Kelp Crisp", "kelpcrisp");
Add("MealPack_Lasagna", t, 0, "Lasagna", "lasagna");
Add("MealPack_Ramen", t, 0, "Ramen", "ramen");
Add("MealPack_RedPellets", t, 0, "Red Pellets", "redpellets");
Add("MealPack_SearedSabiroid", t, 0, "Seared Sabiroid", "searedsabiroid");
Add("MealPack_Spaghetti", t, 0, "Spaghetti", "spaghetti");
Add("MealPack_SteakDinner", t, 0, "Steak Dinner", "steakdinner");
Add("MealPack_VeggieBurger", t, 0, "Veggie Burger", "veggieburger");
AddU("MealPack_BananaBeef", t, 0, "Banana Beef", "bananabeef");
AddU("MealPack_ClangCrunchies", t, 0, "Clang Crunchies", "clangcrunchies");
AddU("MealPack_ExpiredSlop", t, 0, "Expired Slop", "expiredslop");
AddU("MealPack_FoodPaste", t, 0, "Food Paste", "foodpaste");
AddU("MealPack_Hardtack", t, 0, "Hardtack", "hardtack");
AddU("MealPack_SynthLoaf", t, 0, "Synth Loaf", "synthloaf");
AddU("MealPack_Unknown", t, 0, "Unknown Meal", "unknownmeal");
Add("MammalMeatRaw", t, 0);
Add("MammalMeatCooked", t, 0);
Add("InsectMeatRaw", t, 0);
Add("InsectMeatCooked", t, 0);
Add("Fruit", t, 0);
Add("Mushrooms", t, 0);
Add("Vegetables", t, 0);

t = "Seed";
Add("Fruit", t, 0, "Fruit Seeds", "fruitseeds");
Add("Grain", t, 0, "Grain Seeds", "grainseeds");
Add("Mushrooms", t, 0, "Mushroom Spores", "mushroomspores");
Add("Vegetables", t, 0, "Vegetable Seeds", "vegetableseeds");

t = "Ammo";
AddU("NATO_5p56x45mm", t, 8000, "5.56x45mm", "5.56x45mm");
Add("SemiAutoPistolMagazine", t, 500, "S-10 Mag", "s-10mag");
Add("ElitePistolMagazine", t, 500, "S-10E Mag", "s-10emag");
Add("FullAutoPistolMagazine", t, 500, "S-20A Mag", "s-20amag");
Add("AutomaticRifleGun_Mag_20rd", t, 1000, "MR-20 Mag", "mr-20mag");
Add("PreciseAutomaticRifleGun_Mag_5rd", t, 1000, "MR-8P Mag", "mr-8pmag");
Add("RapidFireAutomaticRifleGun_Mag_50rd", t, 8000, "MR-50A Mag", "mr-50amag");
Add("UltimateAutomaticRifleGun_Mag_30rd", t, 1000, "MR-30E Mag", "mr-30emag");
Add("NATO_25x184mm", t, 2500, "Gatling Ammo", "25x184mm", "gatlingammo");
Add("Missile200mm", t, 1600, "Rockets", "200mmmissile", "rockets");
Add("AutocannonClip", t, 50, "Autocannon Mag", "acmag", "autocannonmag");
Add("MediumCalibreAmmo", t, 50, "Assault Cannon Shell", "acshell", "assaultcannonshell");
Add("SmallRailgunAmmo", t, 50, "S. Railgun Sabot", "smallsabot", "srailgunsabot");
Add("LargeRailgunAmmo", t, 50, "L. Railgun Sabot", "largesabot", "lrailgunsabot");
Add("LargeCalibreAmmo", t, 50, "Artillery Shell", "artyshell", "artilleryshell");
Add("FlareClip", t, 0);

Add("OxygenBottle", "Oxygen", 5);
Add("HydrogenBottle", "Gas", 5);
// MODDED ITEMS
// (subType, mainType, quota, display name, short name, alt. short name, used)
// * if used is true, item will be shown in inventory even for 0 items
// * if used is false, item will be used only for display name and short name
// AzimuthSupercharger example
Add("AzimuthSupercharger", "Component", 1600, "Supercharger", "supercharger", "", false);

// REALLY REALLY REALLY
// DO NOT MODIFY ANYTHING BELOW THIS (TRANSLATION STRINGS ARE AT THE BOTTOM)
}
void Add(string sT, string mT, int q = 0, string dN = "", string sN = "", string sN2 = "", bool u = true) { ƭ.Ó(sT, mT, q, dN, sN, sN2, u); }
void AddU(string sT, string mT, int q = 0, string dN = "", string sN = "", string sN2 = "") { ƭ.Ó(sT, mT, q, dN, sN, sN2, false); }
/* Customize characters used by script */
class MMStyle {
    // Monospace font characters (\uXXXX is special character code)
    public const char BAR_MONO_START = '[';
    public const char BAR_MONO_END = ']';
    public const char BAR_MONO_EMPTY = '\u2591'; // 25% rect
    public const char BAR_MONO_FILL = '\u2588'; // full rect

    // Classic (Debug) font characters
    // Start and end characters of progress bar need to be the same width!
    public const char BAR_START = '[';
    public const char BAR_END = ']';
    // Empty and fill characters of progress bar need to be the same width!
    public const char BAR_EMPTY = '\'';
    public const char BAR_FILL = '|';
}
ķ ƭ;Ȩ ƪ;ĥ Ϲ;ʅ f=null;void ϸ(string ƚ){}bool Ϸ(string ϳ){return ϳ.ǿ("true");}void ϵ(string ϴ,string ϳ){string Ȍ=ϴ.ToLower
();switch(Ȍ){case"lcd_tag":LCD_TAG=ϳ;break;case"slowmode":SlowMode=Ϸ(ϳ);break;case"enable_boot":ENABLE_BOOT=Ϸ(ϳ);break;
case"skip_content_type":SKIP_CONTENT_TYPE=Ϸ(ϳ);break;case"scroll_lines":int ϲ=0;if(int.TryParse(ϳ,out ϲ)){SCROLL_LINES=ϲ;}
break;}}void ϱ(){string[]ŧ=Me.CustomData.Split('\n');for(int A=0;A<ŧ.Length;A++){string Ŭ=ŧ[A];int Ł=Ŭ.IndexOf('=');if(Ł<0){ϸ
(Ŭ);continue;}string Ϟ=Ŭ.Substring(0,Ł).Trim();string ǽ=Ŭ.Substring(Ł+1).Trim();ϵ(Ϟ,ǽ);}}int ϝ=0;bool Ϝ(){if(ϝ<6){switch(
ϝ){case 0:ƭ=new ķ();ItemsConf();break;case 1:ϱ();f=new ʅ(this,DebugLevel,ƪ){ƭ=ƭ,ɿ=LCD_TAG,ɾ=SCROLL_LINES,ɽ=ENABLE_BOOT,ɼ=
BOOT_FRAMES,ɺ=HEIGHT_MOD,ɻ=WIDTH_MOD};f.ǌ();break;case 2:f.ǋ();break;case 3:f.Ǌ();f.Ǩ();break;case 4:Ϲ=new ĥ(f);ƪ.Ȱ(Ϲ,0);break;case
5:var ϛ=new List<IMyTerminalBlock>();f.Œ.Є(ref ϛ,"reactor");break;}ϝ++;return false;}return true;}void Ϛ(){ƪ.ǔ=this;f.ǔ=
this;}Program(){Runtime.UpdateFrequency=UpdateFrequency.Update1;}void Main(string Ć,UpdateType ϙ){try{if(ƪ==null){ƪ=new Ȩ(
this,DebugLevel,SlowMode);}if(!Ϝ())return;Ϛ();f.Œ.Љ();if(Ć.Length==0&&(ϙ&(UpdateType.Update1|UpdateType.Update10|UpdateType.
Update100))==0){ƪ.ȷ();return;}if(Ć!=""){if(Ϲ.ć(Ć)){ƪ.ȷ();return;}}Ϲ.č=0;ƪ.ȶ();}catch(Exception ex){Echo("ERROR DESCRIPTION:\n"+ex
.ToString());Me.Enabled=this.mdk==0;}}class Ϙ:ɬ{ĥ Ĕ;ʅ f;string Ć="";public Ϙ(ʅ X,ĥ Ē,string Ŕ){ɧ=-1;ɫ="ArgScroll";Ć=Ŕ;Ĕ=Ē
;f=X;}int ĸ;ϯ ϗ;public override void ɯ(){ϗ=new ϯ(Ʋ,f.Œ);}int ϖ=0;int Ĝ=0;ΐ ƚ;public override bool ʇ(bool õ){if(!õ){Ĝ=0;ϗ.
ų();ƚ=new ΐ(Ʋ);ϖ=0;}if(Ĝ==0){if(!ƚ.ʲ(Ć,õ))return false;if(ƚ.ͽ.Count>0){if(!int.TryParse(ƚ.ͽ[0].Ŕ,out ĸ))ĸ=1;else if(ĸ<1)ĸ
=1;}if(ƚ.Ώ.EndsWith("up"))ĸ=-ĸ;else if(!ƚ.Ώ.EndsWith("down"))ĸ=0;Ĝ++;õ=false;}if(Ĝ==1){if(!ϗ.Ъ("textpanel",ƚ.Ύ,õ))return
false;Ĝ++;õ=false;}å e;for(;ϖ<ϗ.Ч();ϖ++){if(!Ʋ.ȯ(20))return false;var ϰ=ϗ.Ϭ[ϖ]as IMyTextPanel;if(!Ĕ.û.TryGetValue(ϰ,out e))
continue;if(e==null||e.â!=ϰ)continue;if(e.Ü)e.ã.ļ=10;if(ĸ>0)e.ã.Ŀ(ĸ);else if(ĸ<0)e.ã.ŀ(-ĸ);else e.ã.Ļ();e.F();}return true;}}
class ϯ{Ȩ Ʋ;Ц Ϯ;IMyCubeGrid ϭ{get{return Ʋ.ǔ.Me.CubeGrid;}}IMyGridTerminalSystem Ǒ{get{return Ʋ.ǔ.GridTerminalSystem;}}public
List<IMyTerminalBlock>Ϭ=new List<IMyTerminalBlock>(30);public ϯ(Ȩ ƪ,Ц ϫ){Ʋ=ƪ;Ϯ=ϫ;}int Ϫ=0;public double ϩ(ref double Ϩ,ref
double ϧ,bool õ){if(!õ)Ϫ=0;for(;Ϫ<Ϭ.Count;Ϫ++){if(!Ʋ.ȯ(4))return Double.NaN;IMyInventory Ϣ=Ϭ[Ϫ].GetInventory(0);if(Ϣ==null)
continue;Ϩ+=(double)Ϣ.CurrentVolume;ϧ+=(double)Ϣ.MaxVolume;}Ϩ*=1000;ϧ*=1000;return(ϧ>0?Ϩ/ϧ*100:100);}int Ϧ=0;double ϥ=0;public
double Ϥ(bool õ){if(!õ){Ϧ=0;ϥ=0;}for(;Ϧ<Ϭ.Count;Ϧ++){if(!Ʋ.ȯ(6))return Double.NaN;for(int ϣ=0;ϣ<2;ϣ++){IMyInventory Ϣ=Ϭ[Ϧ].
GetInventory(ϣ);if(Ϣ==null)continue;ϥ+=(double)Ϣ.CurrentMass;}}return ϥ*1000;}int ϡ=0;private bool Ϡ(bool õ=false){if(!õ)ϡ=0;while(ϡ
<Ϭ.Count){if(!Ʋ.ȯ(400))return false;if(Ϭ[ϡ].CubeGrid!=ϭ){Ϭ.RemoveAt(ϡ);continue;}ϡ++;}return true;}int Ϻ=0;private bool Ћ
(bool õ=false){if(!õ)Ϻ=0;var Ф=Ʋ.ǔ.Me;while(Ϻ<Ϭ.Count){if(!Ʋ.ȯ(400))return false;if(!Ϭ[Ϻ].IsSameConstructAs(Ф)){Ϭ.
RemoveAt(Ϻ);continue;}Ϻ++;}return true;}List<IMyBlockGroup>У=new List<IMyBlockGroup>(5);List<IMyTerminalBlock>Т=new List<
IMyTerminalBlock>(30);int С=0;public bool Р(string Ύ,bool õ){int П=Ύ.IndexOf(':');string О=(П>=1&&П<=2?Ύ.Substring(0,П):"");bool Ж=О.
Contains("T");bool Е=О.Contains("C");if(О!="")Ύ=Ύ.Substring(П+1);if(Ύ==""||Ύ=="*"){if(!õ){Т.Clear();Ǒ.GetBlocks(Т);Ϭ.AddList(Т);
Ʋ.ȳ(100);}if(Ж){if(!Ϡ(õ))return false;}else if(Е){if(!Ћ(õ))return false;}return true;}string З=(О.Contains("G")?Ύ.Trim():
"");if(З!=""){if(!õ){У.Clear();Ǒ.GetBlockGroups(У);С=0;}for(;С<У.Count;С++){IMyBlockGroup Н=У[С];if(string.Compare(Н.Name,
З,true)==0){if(!õ){Т.Clear();Н.GetBlocks(Т);Ϭ.AddList(Т);Ʋ.ȳ(100);}if(Ж){if(!Ϡ(õ))return false;}else if(Е){if(!Ћ(õ))
return false;}return true;}}return true;}if(!õ){Т.Clear();Ǒ.SearchBlocksOfName(Ύ,Т);Ϭ.AddList(Т);Ʋ.ȳ(100);}if(Ж){if(!Ϡ(õ))
return false;}else if(Е){if(!Ћ(õ))return false;}return true;}List<IMyBlockGroup>М=new List<IMyBlockGroup>(5);List<
IMyTerminalBlock>Л=new List<IMyTerminalBlock>(10);int К=0;int Й=0;public bool И(string ʺ,string З,bool Ж,bool Е,bool õ){if(!õ){М.Clear()
;Ǒ.GetBlockGroups(М);К=0;}var Ф=Ʋ.ǔ.Me;for(;К<М.Count;К++){IMyBlockGroup Н=М[К];if(string.Compare(Н.Name,З,true)==0){if(!
õ){Й=0;Л.Clear();Н.GetBlocks(Л);Ʋ.ȳ(100);}else õ=false;for(;Й<Л.Count;Й++){if(!Ʋ.ȯ(400))return false;if(Ж&&Л[Й].CubeGrid
!=ϭ)continue;if(Е&&!Л[Й].IsSameConstructAs(Ф))continue;if(Ϯ.Ѓ(Л[Й],ʺ))Ϭ.Add(Л[Й]);Ʋ.ȳ(5);}return true;}}return true;}List<
IMyTerminalBlock>Ы=new List<IMyTerminalBlock>(50);int Ь=0;public bool Ъ(string ʺ,string Ύ,bool õ){int П=Ύ.IndexOf(':');string О=(П>=1&&П
<=2?Ύ.Substring(0,П):"");bool Ж=О.Contains("T");bool Е=О.Contains("C");if(О!="")Ύ=Ύ.Substring(П+1);if(!õ){Ы.Clear();Ь=0;}
string З=(О.Contains("G")?Ύ.Trim():"");if(З!=""){if(!И(ʺ,З,Ж,Е,õ))return false;return true;}if(!õ){Ϯ.Є(ref Ы,ʺ);Ʋ.ȳ(100);}if(Ύ
==""||Ύ=="*"){if(!õ)Ϭ.AddList(Ы);if(Ж){if(!Ϡ(õ))return false;}else if(Е){if(!Ћ(õ))return false;}return true;}var Ф=Ʋ.ǔ.Me;
for(;Ь<Ы.Count;Ь++){if(!Ʋ.ȯ(400))return false;if(Ж&&Ы[Ь].CubeGrid!=ϭ)continue;if(Е&&!Ы[Ь].IsSameConstructAs(Ф))continue;if(
Ы[Ь].CustomName.Contains(Ύ))Ϭ.Add(Ы[Ь]);Ʋ.ȳ(5);}return true;}public void Щ(ϯ Ш){Ϭ.AddList(Ш.Ϭ);}public void ų(){Ϭ.Clear()
;}public int Ч(){return Ϭ.Count;}}class Ц{Ȩ Ʋ;ʅ f;public MyGridProgram ǔ{get{return Ʋ.ǔ;}}public IMyGridTerminalSystem Ǒ{
get{return Ʋ.ǔ.GridTerminalSystem;}}public Ц(Ȩ ƪ,ʅ X){Ʋ=ƪ;f=X;}void Х<Ǧ>(List<IMyTerminalBlock>Д,Func<IMyTerminalBlock,bool
>Г=null)where Ǧ:class,IMyTerminalBlock{Ǒ.GetBlocksOfType<Ǧ>(Д,Г);}public Dictionary<string,Action<List<IMyTerminalBlock>,
Func<IMyTerminalBlock,bool>>>ϻ;public void Љ(){if(ϻ!=null)return;ϻ=new Dictionary<string,Action<List<IMyTerminalBlock>,Func<
IMyTerminalBlock,bool>>>(){{"CargoContainer",Х<IMyCargoContainer>},{"TextPanel",Х<IMyTextPanel>},{"Assembler",Х<IMyAssembler>},{
"Refinery",Х<IMyRefinery>},{"Reactor",Х<IMyReactor>},{"SolarPanel",Х<IMySolarPanel>},{"BatteryBlock",Х<IMyBatteryBlock>},{"Beacon"
,Х<IMyBeacon>},{"RadioAntenna",Х<IMyRadioAntenna>},{"AirVent",Х<IMyAirVent>},{"ConveyorSorter",Х<IMyConveyorSorter>},{
"OxygenTank",Х<IMyGasTank>},{"OxygenGenerator",Х<IMyGasGenerator>},{"OxygenFarm",Х<IMyOxygenFarm>},{"LaserAntenna",Х<IMyLaserAntenna
>},{"Thrust",Х<IMyThrust>},{"Gyro",Х<IMyGyro>},{"SensorBlock",Х<IMySensorBlock>},{"ShipConnector",Х<IMyShipConnector>},{
"ReflectorLight",Х<IMyReflectorLight>},{"InteriorLight",Х<IMyInteriorLight>},{"LandingGear",Х<IMyLandingGear>},{"ProgrammableBlock",Х<
IMyProgrammableBlock>},{"TimerBlock",Х<IMyTimerBlock>},{"MotorStator",Х<IMyMotorStator>},{"PistonBase",Х<IMyPistonBase>},{"Projector",Х<
IMyProjector>},{"ShipMergeBlock",Х<IMyShipMergeBlock>},{"SoundBlock",Х<IMySoundBlock>},{"Collector",Х<IMyCollector>},{"JumpDrive",Х<
IMyJumpDrive>},{"Door",Х<IMyDoor>},{"GravityGeneratorSphere",Х<IMyGravityGeneratorSphere>},{"GravityGenerator",Х<IMyGravityGenerator
>},{"ShipDrill",Х<IMyShipDrill>},{"ShipGrinder",Х<IMyShipGrinder>},{"ShipWelder",Х<IMyShipWelder>},{"Parachute",Х<
IMyParachute>},{"LargeGatlingTurret",Х<IMyLargeGatlingTurret>},{"LargeInteriorTurret",Х<IMyLargeInteriorTurret>},{
"LargeMissileTurret",Х<IMyLargeMissileTurret>},{"SmallGatlingGun",Х<IMySmallGatlingGun>},{"SmallMissileLauncherReload",Х<
IMySmallMissileLauncherReload>},{"SmallMissileLauncher",Х<IMySmallMissileLauncher>},{"VirtualMass",Х<IMyVirtualMass>},{"Warhead",Х<IMyWarhead>},{
"FunctionalBlock",Х<IMyFunctionalBlock>},{"LightingBlock",Х<IMyLightingBlock>},{"ControlPanel",Х<IMyControlPanel>},{"Cockpit",Х<
IMyCockpit>},{"TransponderBlock",Х<IMyTransponder>},{"BroadcastController",Х<IMyBroadcastController>},{"CryoChamber",Х<
IMyCryoChamber>},{"MedicalRoom",Х<IMyMedicalRoom>},{"RemoteControl",Х<IMyRemoteControl>},{"ButtonPanel",Х<IMyButtonPanel>},{
"CameraBlock",Х<IMyCameraBlock>},{"OreDetector",Х<IMyOreDetector>},{"ShipController",Х<IMyShipController>},{"SafeZoneBlock",Х<
IMySafeZoneBlock>},{"Decoy",Х<IMyDecoy>}};}public void Ј(ref List<IMyTerminalBlock>ă,string Ї){Action<List<IMyTerminalBlock>,Func<
IMyTerminalBlock,bool>>І;if(Ї=="SurfaceProvider"){Ǒ.GetBlocksOfType<IMyTextSurfaceProvider>(ă);return;}if(ϻ.TryGetValue(Ї,out І))І(ă,
null);else{if(Ї=="WindTurbine"){Ǒ.GetBlocksOfType<IMyPowerProducer>(ă,(Ѕ)=>Ѕ.BlockDefinition.TypeIdString.EndsWith(
"WindTurbine"));return;}if(Ї=="HydrogenEngine"){Ǒ.GetBlocksOfType<IMyPowerProducer>(ă,(Ѕ)=>Ѕ.BlockDefinition.TypeIdString.EndsWith(
"HydrogenEngine"));return;}if(Ї=="StoreBlock"){Ǒ.GetBlocksOfType<IMyFunctionalBlock>(ă,(Ѕ)=>Ѕ.BlockDefinition.TypeIdString.EndsWith(
"StoreBlock"));return;}if(Ї=="ContractBlock"){Ǒ.GetBlocksOfType<IMyFunctionalBlock>(ă,(Ѕ)=>Ѕ.BlockDefinition.TypeIdString.EndsWith(
"ContractBlock"));return;}if(Ї=="VendingMachine"){Ǒ.GetBlocksOfType<IMyFunctionalBlock>(ă,(Ѕ)=>Ѕ.BlockDefinition.TypeIdString.EndsWith(
"VendingMachine"));return;}}}public void Є(ref List<IMyTerminalBlock>ă,string Ђ){Ј(ref ă,Ѐ(Ђ.Trim()));}public bool Ѓ(IMyTerminalBlock â,
string Ђ){string Ё=Ѐ(Ђ);switch(Ё){case"FunctionalBlock":return true;case"ShipController":return(â as IMyShipController!=null);
default:return â.BlockDefinition.TypeIdString.Contains(Ѐ(Ђ));}}public string Ѐ(string Ͽ){if(Ͽ=="surfaceprovider")return
"SurfaceProvider";if(Ͽ.Ȁ("carg")||Ͽ.Ȁ("conta"))return"CargoContainer";if(Ͽ.Ȁ("text")||Ͽ.Ȁ("lcd"))return"TextPanel";if(Ͽ.Ȁ("coc"))return
"Cockpit";if(Ͽ.Ȁ("ass"))return"Assembler";if(Ͽ.Ȁ("refi"))return"Refinery";if(Ͽ.Ȁ("reac"))return"Reactor";if(Ͽ.Ȁ("solar"))return
"SolarPanel";if(Ͽ.Ȁ("wind"))return"WindTurbine";if(Ͽ.Ȁ("hydro")&&Ͽ.Contains("eng"))return"HydrogenEngine";if(Ͽ.Ȁ("bat"))return
"BatteryBlock";if(Ͽ.Ȁ("bea"))return"Beacon";if(Ͽ.ǿ("vent"))return"AirVent";if(Ͽ.ǿ("sorter"))return"ConveyorSorter";if(Ͽ.ǿ("tank"))
return"OxygenTank";if(Ͽ.ǿ("farm")&&Ͽ.ǿ("oxy"))return"OxygenFarm";if(Ͽ.ǿ("gene")&&Ͽ.ǿ("oxy"))return"OxygenGenerator";if(Ͽ.ǿ(
"cryo"))return"CryoChamber";if(string.Compare(Ͽ,"laserantenna",true)==0)return"LaserAntenna";if(Ͽ.ǿ("antenna"))return
"RadioAntenna";if(Ͽ.Ȁ("thrust"))return"Thrust";if(Ͽ.Ȁ("gyro"))return"Gyro";if(Ͽ.Ȁ("sensor"))return"SensorBlock";if(Ͽ.ǿ("connector"))
return"ShipConnector";if(Ͽ.Ȁ("reflector")||Ͽ.Ȁ("spotlight"))return"ReflectorLight";if((Ͽ.Ȁ("inter")&&Ͽ.Ǿ("light")))return
"InteriorLight";if(Ͽ.Ȁ("land"))return"LandingGear";if(Ͽ.Ȁ("program"))return"ProgrammableBlock";if(Ͽ.Ȁ("timer"))return"TimerBlock";if(Ͽ.
Ȁ("motor")||Ͽ.Ȁ("rotor"))return"MotorStator";if(Ͽ.Ȁ("piston"))return"PistonBase";if(Ͽ.Ȁ("proj"))return"Projector";if(Ͽ.ǿ(
"merge"))return"ShipMergeBlock";if(Ͽ.Ȁ("sound"))return"SoundBlock";if(Ͽ.Ȁ("col"))return"Collector";if(Ͽ.ǿ("jump"))return
"JumpDrive";if(string.Compare(Ͽ,"door",true)==0)return"Door";if((Ͽ.ǿ("grav")&&Ͽ.ǿ("sphe")))return"GravityGeneratorSphere";if(Ͽ.ǿ(
"grav"))return"GravityGenerator";if(Ͽ.Ǿ("drill"))return"ShipDrill";if(Ͽ.ǿ("grind"))return"ShipGrinder";if(Ͽ.Ǿ("welder"))return
"ShipWelder";if(Ͽ.Ȁ("parach"))return"Parachute";if((Ͽ.ǿ("turret")&&Ͽ.ǿ("gatl")))return"LargeGatlingTurret";if((Ͽ.ǿ("turret")&&Ͽ.ǿ(
"inter")))return"LargeInteriorTurret";if((Ͽ.ǿ("turret")&&Ͽ.ǿ("miss")))return"LargeMissileTurret";if(Ͽ.ǿ("gatl"))return
"SmallGatlingGun";if((Ͽ.ǿ("launcher")&&Ͽ.ǿ("reload")))return"SmallMissileLauncherReload";if((Ͽ.ǿ("launcher")))return
"SmallMissileLauncher";if(Ͽ.ǿ("mass"))return"VirtualMass";if(string.Compare(Ͽ,"warhead",true)==0)return"Warhead";if(Ͽ.Ȁ("func"))return
"FunctionalBlock";if(string.Compare(Ͽ,"shipctrl",true)==0)return"ShipController";if(Ͽ.StartsWith("broadcast"))return"BroadcastController"
;if(Ͽ.Contains("transponder")||Ͽ.Contains("relay"))return"TransponderBlock";if(Ͽ.Ȁ("light"))return"LightingBlock";if(Ͽ.Ȁ(
"contr"))return"ControlPanel";if(Ͽ.Ȁ("medi"))return"MedicalRoom";if(Ͽ.Ȁ("remote"))return"RemoteControl";if(Ͽ.Ȁ("but"))return
"ButtonPanel";if(Ͽ.Ȁ("cam"))return"CameraBlock";if(Ͽ.ǿ("detect"))return"OreDetector";if(Ͽ.Ȁ("safe"))return"SafeZoneBlock";if(Ͽ.Ȁ(
"store"))return"StoreBlock";if(Ͽ.Ȁ("contract"))return"ContractBlock";if(Ͽ.Ȁ("vending"))return"VendingMachine";if(Ͽ.Ȁ("decoy"))
return"Decoy";return"Unknown";}public string Ͼ(IMyBatteryBlock Ŏ){string Ͻ="";if(Ŏ.ChargeMode==ChargeMode.Recharge)Ͻ="(+) ";
else if(Ŏ.ChargeMode==ChargeMode.Discharge)Ͻ="(-) ";else Ͻ="(±) ";return Ͻ+f.ȕ((Ŏ.CurrentStoredPower/Ŏ.MaxStoredPower)*
100.0f)+"%";}Dictionary<MyLaserAntennaStatus,string>ϼ=new Dictionary<MyLaserAntennaStatus,string>(){{MyLaserAntennaStatus.Idle
,"IDLE"},{MyLaserAntennaStatus.Connecting,"CONNECTING"},{MyLaserAntennaStatus.Connected,"CONNECTED"},{
MyLaserAntennaStatus.OutOfRange,"OUT OF RANGE"},{MyLaserAntennaStatus.RotatingToTarget,"ROTATING"},{MyLaserAntennaStatus.
SearchingTargetForAntenna,"SEARCHING"}};public string Њ(IMyLaserAntenna Ō){return ϼ[Ō.Status];}public double В(IMyJumpDrive ō,out double ʱ,out
double Ɠ){ʱ=ō.CurrentStoredPower;Ɠ=ō.MaxStoredPower;return(Ɠ>0?ʱ/Ɠ*100:0);}public double Б(IMyJumpDrive ō){double ʱ=ō.
CurrentStoredPower;double Ɠ=ō.MaxStoredPower;return(Ɠ>0?ʱ/Ɠ*100:0);}}class А:ɬ{ʅ f;ĥ Ĕ;public int Џ=0;public А(ʅ X,ĥ Z){ɫ="BootPanelsTask"
;ɧ=1;f=X;Ĕ=Z;if(!f.ɽ){Џ=int.MaxValue;Ĕ.ú=true;}}ǧ Đ;public override void ɯ(){Đ=f.Đ;}public override bool ʇ(bool õ){if(Џ>f
.ɼ.Count){ɮ();return true;}if(!õ&&Џ==0){Ĕ.ú=false;}if(!Ќ(õ))return false;Џ++;return true;}public override void ʆ(){Ĕ.ú=
true;}public void Ў(){ɋ ä=Ĕ.ä;for(int A=0;A<ä.z();A++){å e=ä.r(A);e.Å();}Џ=(f.ɽ?0:int.MaxValue);}int A;ž Ѝ=null;public bool
Ќ(bool õ){ɋ ä=Ĕ.ä;if(!õ)A=0;int ϟ=0;for(;A<ä.z();A++){if(!Ʋ.ȯ(40)||ϟ>5)return false;å e=ä.r(A);Ѝ=f.ǟ(Ѝ,e);float?ʻ=e.Û?.
FontSize;if(ʻ!=null&&ʻ>3f)continue;if(Ѝ.Ÿ.Count<=0)Ѝ.Ŵ(f.ǡ(null,e));else f.ǡ(Ѝ.Ÿ[0],e);f.Ŧ();f.Ƶ(Đ.Ǧ("B1"));double ʭ=(double)Џ/f
.ɼ.Count*100;f.ǁ(ʭ);if(Џ==f.ɼ.Count){f.Ǟ("");f.Ƶ("Automatic LCDs 2");f.Ƶ("by MMaster");f.Ƶ("v"+f.ǔ.VERSION);}else f.ǝ(f.ɼ
[Џ]);bool Ü=e.Ü;e.Ü=false;f.ƾ(e,Ѝ);e.Ü=Ü;ϟ++;}return true;}public bool ʹ(){return Џ<=f.ɼ.Count;}}public enum ͳ{Ͳ=0,ͱ=1,Ͱ=
2,ˮ=3,ˬ=4,ˤ=5,ˣ=6,ˢ=7,ˡ=8,ˠ=9,ˑ=10,ː=11,ˏ=12,ˎ=13,ˍ=14,ˌ=15,ˋ=16,ˊ=17,ˉ=18,Ͷ=19,ͷ=20,Κ=21,Λ=22,Ι=23,Θ=24,Η=25,Ζ=26,Ε=27,Δ
=28,Γ=29,Β=30,Α=31,}class ΐ{Ȩ Ʋ;public string Ώ="";public string Ύ="";public string Ό="";public string Ί="";public ͳ Ή=ͳ.
Ͳ;public ΐ(Ȩ ƪ){Ʋ=ƪ;}ͳ Έ(){if(Ώ=="echo"||Ώ=="center"||Ώ=="right")return ͳ.ͱ;if(Ώ.StartsWith("hscroll"))return ͳ.Β;if(Ώ.
StartsWith("inventory")||Ώ.StartsWith("missing")||Ώ.StartsWith("invlist"))return ͳ.Ͱ;if(Ώ.StartsWith("working"))return ͳ.ˉ;if(Ώ.
StartsWith("cargo"))return ͳ.ˮ;if(Ώ.StartsWith("mass"))return ͳ.ˬ;if(Ώ.StartsWith("shipmass"))return ͳ.Ι;if(Ώ.StartsWith("oxygen")
)return ͳ.ˤ;if(Ώ.StartsWith("tanks"))return ͳ.ˣ;if(Ώ.StartsWith("powertime"))return ͳ.ˢ;if(Ώ.StartsWith("powerused"))
return ͳ.ˡ;if(Ώ.StartsWith("power"))return ͳ.ˠ;if(Ώ.StartsWith("speed"))return ͳ.ˑ;if(Ώ.StartsWith("accel"))return ͳ.ː;if(Ώ.
StartsWith("alti"))return ͳ.Η;if(Ώ.StartsWith("charge"))return ͳ.ˏ;if(Ώ.StartsWith("docked"))return ͳ.Α;if(Ώ.StartsWith("time")||Ώ
.StartsWith("date"))return ͳ.ˎ;if(Ώ.StartsWith("countdown"))return ͳ.ˍ;if(Ώ.StartsWith("textlcd"))return ͳ.ˌ;if(Ώ.
EndsWith("count"))return ͳ.ˋ;if(Ώ.StartsWith("dampeners")||Ώ.StartsWith("occupied"))return ͳ.ˊ;if(Ώ.StartsWith("damage"))return
ͳ.Ͷ;if(Ώ.StartsWith("amount"))return ͳ.ͷ;if(Ώ.StartsWith("pos"))return ͳ.Κ;if(Ώ.StartsWith("distance"))return ͳ.Θ;if(Ώ.
StartsWith("details"))return ͳ.Λ;if(Ώ.StartsWith("stop"))return ͳ.Ζ;if(Ώ.StartsWith("gravity"))return ͳ.Ε;if(Ώ.StartsWith(
"customdata"))return ͳ.Δ;if(Ώ.StartsWith("prop"))return ͳ.Γ;return ͳ.Ͳ;}public ƛ Ά(){switch(Ή){case ͳ.ͱ:return new Ӓ();case ͳ.Ͱ:
return new ҫ();case ͳ.ˮ:return new σ();case ͳ.ˬ:return new ә();case ͳ.ˤ:return new Ә();case ͳ.ˣ:return new Ҋ();case ͳ.ˢ:return
new ѣ();case ͳ.ˡ:return new ф();case ͳ.ˠ:return new ӡ();case ͳ.ˑ:return new ѱ();case ͳ.ː:return new ʯ();case ͳ.ˏ:return new
μ();case ͳ.ˎ:return new β();case ͳ.ˍ:return new Φ();case ͳ.ˌ:return new ĺ();case ͳ.ˋ:return new ˈ();case ͳ.ˊ:return new Ѹ
();case ͳ.ˉ:return new Ŋ();case ͳ.Ͷ:return new Μ();case ͳ.ͷ:return new ӷ();case ͳ.Κ:return new ӣ();case ͳ.Λ:return new έ(
);case ͳ.Ι:return new ѷ();case ͳ.Θ:return new ӂ();case ͳ.Η:return new ʬ();case ͳ.Ζ:return new ҍ();case ͳ.Ε:return new ӑ()
;case ͳ.Δ:return new Ξ();case ͳ.Γ:return new Ҡ();case ͳ.Β:return new Ӑ();case ͳ.Α:return new Ӗ();default:return new ƛ();}
}public List<ʷ>ͽ=new List<ʷ>();string[]ͼ=null;string ͻ="";bool ͺ=false;int Ŝ=1;public bool ʲ(string ʫ,bool õ){if(!õ){Ή=ͳ.
Ͳ;Ύ="";Ώ="";Ό=ʫ.TrimStart(' ');ͽ.Clear();if(Ό=="")return true;int ʸ=Ό.IndexOf(' ');if(ʸ<0||ʸ>=Ό.Length-1)Ί="";else Ί=Ό.
Substring(ʸ+1);ͼ=Ό.Split(' ');ͻ="";ͺ=false;Ώ=ͼ[0].ToLower();Ŝ=1;}for(;Ŝ<ͼ.Length;Ŝ++){if(!Ʋ.ȯ(40))return false;string Ŕ=ͼ[Ŝ];if(Ŕ
=="")continue;if(Ŕ[0]=='{'&&Ŕ[Ŕ.Length-1]=='}'){Ŕ=Ŕ.Substring(1,Ŕ.Length-2);if(Ŕ=="")continue;if(Ύ=="")Ύ=Ŕ;else ͽ.Add(new
ʷ(Ŕ));continue;}if(Ŕ[0]=='{'){ͺ=true;ͻ=Ŕ.Substring(1);continue;}if(Ŕ[Ŕ.Length-1]=='}'){ͺ=false;ͻ+=' '+Ŕ.Substring(0,Ŕ.
Length-1);if(Ύ=="")Ύ=ͻ;else ͽ.Add(new ʷ(ͻ));continue;}if(ͺ){if(ͻ.Length!=0)ͻ+=' ';ͻ+=Ŕ;continue;}if(Ύ=="")Ύ=Ŕ;else ͽ.Add(new ʷ
(Ŕ));}Ή=Έ();return true;}}class ʷ{public string ʶ="";public string ʵ="";public string Ŕ="";public List<string>ʴ=new List<
string>();public ʷ(string ʳ){Ŕ=ʳ;}public void ʲ(){if(Ŕ==""||ʶ!=""||ʵ!=""||ʴ.Count>0)return;string ʱ=Ŕ.Trim();if(ʱ[0]=='+'||ʱ[0
]=='-'){ʶ+=ʱ[0];ʱ=Ŕ.Substring(1);}string[]Ƨ=ʱ.Split('/');string ʰ=Ƨ[0];if(Ƨ.Length>1){ʵ=Ƨ[0];ʰ=Ƨ[1];}else ʵ="";if(ʰ.
Length>0){string[]Ĉ=ʰ.Split(',');for(int A=0;A<Ĉ.Length;A++)if(Ĉ[A]!="")ʴ.Add(Ĉ[A]);}}}class ʯ:ƛ{public ʯ(){ɧ=0.5;ɫ="CmdAccel"
;}public override bool Ɨ(bool õ){double ʮ=0;if(ƚ.Ύ!="")double.TryParse(ƚ.Ύ.Trim(),out ʮ);f.Ó(Đ.Ǧ("AC1")+" ");f.ƽ(f.ǒ.ʘ.
ToString("F1")+" m/s²");if(ʮ>0){double ʭ=f.ǒ.ʘ/ʮ*100;f.ǁ(ʭ);}return true;}}class ʬ:ƛ{public ʬ(){ɧ=1;ɫ="CmdAltitude";}public
override bool Ɨ(bool õ){string ʺ=(ƚ.Ώ.EndsWith("sea")?"sea":"ground");switch(ʺ){case"sea":f.Ó(Đ.Ǧ("ALT1"));f.ƽ(f.ǒ.ʎ.ToString(
"F0")+" m");break;default:f.Ó(Đ.Ǧ("ALT2"));f.ƽ(f.ǒ.ʌ.ToString("F0")+" m");break;}return true;}}class ˈ:ƛ{public ˈ(){ɧ=15;ɫ=
"CmdBlockCount";}ϯ ŝ;public override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);}bool ʼ;bool ʹ;int Ŝ=0;int Ĝ=0;public override bool Ɨ(bool õ){if(!õ){ʼ=(ƚ.
Ώ=="enabledcount");ʹ=(ƚ.Ώ=="prodcount");Ŝ=0;Ĝ=0;}if(ƚ.ͽ.Count==0){if(Ĝ==0){if(!õ)ŝ.ų();if(!ŝ.Р(ƚ.Ύ,õ))return false;Ĝ++;õ=
false;}if(!ʽ(ŝ,"blocks",ʼ,ʹ,õ))return false;return true;}for(;Ŝ<ƚ.ͽ.Count;Ŝ++){ʷ Ŕ=ƚ.ͽ[Ŝ];if(!õ)Ŕ.ʲ();if(!ŕ(Ŕ,õ))return false
;õ=false;}return true;}int ř=0;int Ś=0;bool ŕ(ʷ Ŕ,bool õ){if(!õ){ř=0;Ś=0;}for(;ř<Ŕ.ʴ.Count;ř++){if(Ś==0){if(!õ)ŝ.ų();if(!
ŝ.Ъ(Ŕ.ʴ[ř],ƚ.Ύ,õ))return false;Ś++;õ=false;}if(!ʽ(ŝ,Ŕ.ʴ[ř],ʼ,ʹ,õ))return false;Ś=0;õ=false;}return true;}Dictionary<
string,int>ˇ=new Dictionary<string,int>();Dictionary<string,int>ˆ=new Dictionary<string,int>();List<string>ˁ=new List<string>(
30);int Ċ=0;int ˀ=0;int ʿ=0;ʦ ʾ=new ʦ();bool ʽ(ϯ ă,string ʺ,bool ʼ,bool ʹ,bool õ){if(ă.Ч()==0){ʾ.ų().ʢ(char.ToUpper(ʺ[0]))
.ʢ(ʺ.ToLower(),1,ʺ.Length-1);f.Ó(ʾ.ʢ(" ").ʢ(Đ.Ǧ("C1")).ʢ(" "));string Ω=(ʼ||ʹ?"0 / 0":"0");f.ƽ(Ω);return true;}if(!õ){ˇ.
Clear();ˆ.Clear();ˁ.Clear();Ċ=0;ˀ=0;ʿ=0;}if(ʿ==0){for(;Ċ<ă.Ч();Ċ++){if(!Ʋ.ȯ(15))return false;var ő=ă.Ϭ[Ċ]as
IMyProductionBlock;ʾ.ų().ʢ(ă.Ϭ[Ċ].DefinitionDisplayNameText);string Ȍ=ʾ.ɛ();if(ˁ.Contains(Ȍ)){ˇ[Ȍ]++;if((ʼ&&ă.Ϭ[Ċ].IsWorking)||(ʹ&&ő!=null
&&ő.IsProducing))ˆ[Ȍ]++;}else{ˇ.Add(Ȍ,1);ˁ.Add(Ȍ);if(ʼ||ʹ)if((ʼ&&ă.Ϭ[Ċ].IsWorking)||(ʹ&&ő!=null&&ő.IsProducing))ˆ.Add(Ȍ,1)
;else ˆ.Add(Ȍ,0);}}ʿ++;õ=false;}for(;ˀ<ˇ.Count;ˀ++){if(!Ʋ.ȯ(8))return false;f.Ó(ˁ[ˀ]+" "+Đ.Ǧ("C1")+" ");string Ω=(ʼ||ʹ?ˆ[
ˁ[ˀ]]+" / ":"")+ˇ[ˁ[ˀ]];f.ƽ(Ω);}return true;}}class σ:ƛ{ϯ ŝ;public σ(){ɧ=2;ɫ="CmdCargo";}public override void ɯ(){ŝ=new ϯ
(Ʋ,f.Œ);}bool ς=true;bool Ϊ=false;bool ρ=false;bool κ=false;double π=0;double ο=0;string ξ;int Ĝ=0;public override bool Ɨ
(bool õ){if(!õ){ŝ.ų();ς=ƚ.Ώ.Contains("all");κ=ƚ.Ώ.EndsWith("bar");Ϊ=(ƚ.Ώ[ƚ.Ώ.Length-1]=='x');ρ=(ƚ.Ώ[ƚ.Ώ.Length-1]=='p');π
=0;ο=0;ξ="";Ĝ=0;}if(Ĝ==0){if(ς){if(!ŝ.Р(ƚ.Ύ,õ))return false;}else{if(!ŝ.Ъ("cargocontainer",ƚ.Ύ,õ))return false;}Ĝ++;õ=
false;}double ν=ŝ.ϩ(ref π,ref ο,õ);if(Double.IsNaN(ν))return false;if(κ){f.ǁ(ν);return true;}if(ƚ.ͽ.Count>0){if(ƚ.ͽ[0].Ŕ.
Length>0)ξ=ƚ.ͽ[0].Ŕ;}f.Ó((ξ==""?Đ.Ǧ("C2"):ξ)+" ");if(!Ϊ&&!ρ){f.ƽ(f.ȍ(π)+"L / "+f.ȍ(ο)+"L");f.ǆ(ν,1.0f,f.ɱ);f.Ǟ(' '+f.ȕ(ν)+"%")
;}else if(ρ){f.ƽ(f.ȕ(ν)+"%");f.ǁ(ν);}else f.ƽ(f.ȕ(ν)+"%");return true;}}class μ:ƛ{public μ(){ɧ=3;ɫ="CmdCharge";}ϯ ŝ;bool
Ϊ=false;bool λ=false;bool κ=false;bool ι=false;public override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);if(ƚ.ͽ.Count>0)ϓ=ƚ.ͽ[0].Ŕ;κ=ƚ.Ώ.
EndsWith("bar");Ϊ=ƚ.Ώ.Contains("x");λ=ƚ.Ώ.Contains("time");ι=ƚ.Ώ.Contains("sum");}int Ĝ=0;int Ċ=0;double τ=0;double ϔ=0;TimeSpan
ϕ=TimeSpan.Zero;string ϓ="";Dictionary<long,double>ģ=new Dictionary<long,double>();Dictionary<long,double>ϒ=new
Dictionary<long,double>();Dictionary<long,double>ϑ=new Dictionary<long,double>();Dictionary<long,double>ϐ=new Dictionary<long,
double>();Dictionary<long,double>Ϗ=new Dictionary<long,double>();double ώ(long ύ,double ʱ,double Ɠ){double ό=0;double ϋ=0;
double ϊ=0;double ω=0;if(ϒ.TryGetValue(ύ,out ϊ)){ω=ϐ[ύ];}if(ģ.TryGetValue(ύ,out ό)){ϋ=ϑ[ύ];}double ψ=(Ʋ.Ȥ-ϊ);double χ=0;if(ψ>0
)χ=(ʱ-ω)/ψ;if(χ<0){if(!Ϗ.TryGetValue(ύ,out χ))χ=0;}else Ϗ[ύ]=χ;if(ό>0){ϒ[ύ]=ģ[ύ];ϐ[ύ]=ϑ[ύ];}ģ[ύ]=Ʋ.Ȥ;ϑ[ύ]=ʱ;return(χ>0?(Ɠ
-ʱ)/χ:0);}private void φ(string Ȍ,double ʭ,double ʱ,double Ɠ,TimeSpan υ){if(κ){f.ǁ(ʭ);}else{f.Ó(Ȍ+" ");if(λ){f.ƽ(f.Ǔ.Ȫ(υ)
);if(!Ϊ){f.ǆ(ʭ,1.0f,f.ɱ);f.ƽ(' '+ʭ.ToString("0.0")+"%");}}else{if(!Ϊ){f.ƽ(f.ȍ(ʱ)+"Wh / "+f.ȍ(Ɠ)+"Wh");f.ǆ(ʭ,1.0f,f.ɱ);}f.
ƽ(' '+ʭ.ToString("0.0")+"%");}}}public override bool Ɨ(bool õ){if(!õ){ŝ.ų();Ċ=0;Ĝ=0;τ=0;ϔ=0;ϕ=TimeSpan.Zero;}if(Ĝ==0){if(
!ŝ.Ъ("jumpdrive",ƚ.Ύ,õ))return false;if(ŝ.Ч()<=0){f.Ǟ("Charge: "+Đ.Ǧ("D2"));return true;}Ĝ++;õ=false;}for(;Ċ<ŝ.Ч();Ċ++){
if(!Ʋ.ȯ(25))return false;var ō=ŝ.Ϭ[Ċ]as IMyJumpDrive;double ʱ,Ɠ,ʭ;ʭ=f.Œ.В(ō,out ʱ,out Ɠ);TimeSpan Ψ;if(λ)try{Ψ=TimeSpan.
FromSeconds(ώ(ō.EntityId,ʱ,Ɠ));}catch{Ψ=new TimeSpan(-1);}else Ψ=TimeSpan.Zero;if(!ι){φ(ō.CustomName,ʭ,ʱ,Ɠ,Ψ);}else{τ+=ʱ;ϔ+=Ɠ;if(ϕ<
Ψ)ϕ=Ψ;}}if(ι){double Χ=(ϔ>0?τ/ϔ*100:0);φ(ϓ,Χ,τ,ϔ,ϕ);}return true;}}class Φ:ƛ{public Φ(){ɧ=1;ɫ="CmdCountdown";}public
override bool Ɨ(bool õ){bool Υ=ƚ.Ώ.EndsWith("c");bool Τ=ƚ.Ώ.EndsWith("r");string Ç="";int Σ=ƚ.Ό.IndexOf(' ');if(Σ>=0)Ç=ƚ.Ό.
Substring(Σ+1).Trim();DateTime Ρ=DateTime.Now;DateTime Π;if(!DateTime.TryParseExact(Ç,"H:mm d.M.yyyy",System.Globalization.
CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None,out Π)){f.Ǟ(Đ.Ǧ("C3"));f.Ǟ("  Countdown 19:02 28.2.2015");
return true;}TimeSpan Ο=Π-Ρ;string Ĺ="";if(Ο.Ticks<=0)Ĺ=Đ.Ǧ("C4");else{if((int)Ο.TotalDays>0)Ĺ+=(int)Ο.TotalDays+" "+Đ.Ǧ("C5")
+" ";if(Ο.Hours>0||Ĺ!="")Ĺ+=Ο.Hours+"h ";if(Ο.Minutes>0||Ĺ!="")Ĺ+=Ο.Minutes+"m ";Ĺ+=Ο.Seconds+"s";}if(Υ)f.Ƶ(Ĺ);else if(Τ)
f.ƽ(Ĺ);else f.Ǟ(Ĺ);return true;}}class Ξ:ƛ{public Ξ(){ɧ=1;ɫ="CmdCustomData";}public override bool Ɨ(bool õ){string Ĺ="";
if(ƚ.Ύ!=""&&ƚ.Ύ!="*"){var Ν=f.Ǒ.GetBlockWithName(ƚ.Ύ)as IMyTerminalBlock;if(Ν==null){f.Ǟ("CustomData: "+Đ.Ǧ("CD1")+ƚ.Ύ);
return true;}Ĺ=Ν.CustomData;}else{f.Ǟ("CustomData:"+Đ.Ǧ("CD2"));return true;}if(Ĺ.Length==0)return true;f.ǝ(Ĺ);return true;}}
class Μ:ƛ{public Μ(){ɧ=5;ɫ="CmdDamage";}ϯ ŝ;public override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);}bool Ɖ=false;int Ċ=0;public override
bool Ɨ(bool õ){bool Ϊ=ƚ.Ώ.StartsWith("damagex");bool η=ƚ.Ώ.EndsWith("noc");bool θ=(!η&&ƚ.Ώ.EndsWith("c"));float ζ=100;if(!õ)
{ŝ.ų();Ɖ=false;Ċ=0;}if(!ŝ.Р(ƚ.Ύ,õ))return false;if(ƚ.ͽ.Count>0){if(!float.TryParse(ƚ.ͽ[0].Ŕ,out ζ))ζ=100;}ζ-=0.00001f;for
(;Ċ<ŝ.Ч();Ċ++){if(!Ʋ.ȯ(30))return false;IMyTerminalBlock â=ŝ.Ϭ[Ċ];IMySlimBlock ε=â.CubeGrid.GetCubeBlock(â.Position);if(ε
==null)continue;float δ=(η?ε.MaxIntegrity:ε.BuildIntegrity);if(!θ)δ-=ε.CurrentDamage;float ʭ=100*(δ/ε.MaxIntegrity);if(ʭ>=
ζ)continue;Ɖ=true;string γ=f.ǲ(ε.FatBlock.DisplayNameText,f.ɸ*0.69f-f.ɱ);f.Ó(γ+' ');if(!Ϊ){f.ƻ(f.ȍ(δ)+" / ",0.69f);f.Ó(f.
ȍ(ε.MaxIntegrity));}f.ƽ(' '+ʭ.ToString("0.0")+'%');f.ǁ(ʭ);}if(!Ɖ)f.Ǟ(Đ.Ǧ("D3"));return true;}}class β:ƛ{public β(){ɧ=1;ɫ=
"CmdDateTime";}public override bool Ɨ(bool õ){bool α=(ƚ.Ώ.StartsWith("datetime"));bool ΰ=(ƚ.Ώ.StartsWith("date"));bool Υ=ƚ.Ώ.Contains
("c");int ί=ƚ.Ώ.IndexOf('+');if(ί<0)ί=ƚ.Ώ.IndexOf('-');float ή=0;if(ί>=0)float.TryParse(ƚ.Ώ.Substring(ί),out ή);DateTime
Ο=DateTime.Now.AddHours(ή);string Ĺ="";int Σ=ƚ.Ό.IndexOf(' ');if(Σ>=0)Ĺ=ƚ.Ό.Substring(Σ+1);if(!α){if(!ΰ)Ĺ+=Ο.
ToShortTimeString();else Ĺ+=Ο.ToShortDateString();}else{if(Ĺ=="")Ĺ=String.Format("{0:d} {0:t}",Ο);else{Ĺ=Ĺ.Replace("/","\\/");Ĺ=Ĺ.Replace
(":","\\:");Ĺ=Ĺ.Replace("\"","\\\"");Ĺ=Ĺ.Replace("'","\\'");Ĺ=Ο.ToString(Ĺ+' ');Ĺ=Ĺ.Substring(0,Ĺ.Length-1);}}if(Υ)f.Ƶ(Ĺ)
;else f.Ǟ(Ĺ);return true;}}class έ:ƛ{public έ(){ɧ=5;ɫ="CmdDetails";}string ά="";string Ϋ="";int š=0;ϯ ŝ;public override
void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);if(ƚ.ͽ.Count>0)ά=ƚ.ͽ[0].Ŕ.Trim();if(ƚ.ͽ.Count>1){string Ŕ=ƚ.ͽ[1].Ŕ.Trim();if(!int.TryParse(Ŕ,out š))
{š=0;Ϋ=Ŕ;}}}int Ĝ=0;int Ċ=1;bool ӌ=false;IMyTerminalBlock â;public override bool Ɨ(bool õ){if(ƚ.Ύ==""||ƚ.Ύ=="*"){f.Ǟ(
"Details: "+Đ.Ǧ("D1"));return true;}if(!õ){ŝ.ų();ӌ=ƚ.Ώ.Contains("non");Ĝ=0;Ċ=1;}if(Ĝ==0){if(!ŝ.Р(ƚ.Ύ,õ))return true;if(ŝ.Ч()<=0){f.
Ǟ("Details: "+Đ.Ǧ("D2"));return true;}Ĝ++;õ=false;}int ӊ=(ƚ.Ώ.EndsWith("x")?1:0);if(Ĝ==1){if(!õ){â=ŝ.Ϭ[0];if(!ӌ)f.Ǟ(â.
CustomName);}if(!Ӆ(â,ӊ,š,õ))return false;Ĝ++;õ=false;}for(;Ċ<ŝ.Ч();Ċ++){if(!õ){â=ŝ.Ϭ[Ċ];if(!ӌ){f.Ǟ("");f.Ǟ(â.CustomName);}}if(!Ӆ(â
,ӊ,š,õ))return false;õ=false;}return true;}string[]ŧ;int Ӊ=0;int ӈ=0;bool Ӈ=false;Ǥ ӆ=new Ǥ("\\[/?Color[^]]*\\]");ʦ Ƽ=new
ʦ();bool Ӆ(IMyTerminalBlock â,int ӄ,int ľ,bool õ){if(!õ){IMyFarmPlotLogic Ӄ;if(â.Components.TryGet(out Ӄ))Ƽ.ų().ʢ(ӆ.ȅ(Ӄ.
GetDetailedInfoWithoutRequiredInput(),"")).ʢ('\n');else Ƽ.ų().ʢ(ӆ.ȅ(â.DetailedInfo,"")).ʢ('\n');Ƽ.ʢ(ӆ.ȅ(â.CustomInfo,""));ŧ=Ƽ.ɛ().Split('\n');Ӊ=ӄ;Ӈ=(ά.
Length==0);ӈ=0;}for(;Ӊ<ŧ.Length;Ӊ++){if(!Ʋ.ȯ(5))return false;if(ŧ[Ӊ].Length==0)continue;if(!Ӈ){if(!ŧ[Ӊ].Contains(ά))continue;Ӈ
=true;}if(Ϋ.Length>0&&ŧ[Ӊ].Contains(Ϋ))return true;f.Ǟ(Ƽ.ų().ʢ("  ").ʢ(ŧ[Ӊ]));ӈ++;if(ľ>0&&ӈ>=ľ)return true;}return true;}
}class ӂ:ƛ{public ӂ(){ɧ=1;ɫ="CmdDistance";}string Ӂ="";string[]Ӏ;Vector3D ҿ;string Ҿ="";bool ҽ=false;public override void
ɯ(){ҽ=false;if(ƚ.ͽ.Count<=0)return;Ӂ=ƚ.ͽ[0].Ŕ.Trim();Ӏ=Ӂ.Split(':');if(Ӏ.Length<5||Ӏ[0]!="GPS")return;double Ҽ,Ӌ,Ӎ;if(!
double.TryParse(Ӏ[2],out Ҽ))return;if(!double.TryParse(Ӏ[3],out Ӌ))return;if(!double.TryParse(Ӏ[4],out Ӎ))return;ҿ=new
Vector3D(Ҽ,Ӌ,Ӎ);Ҿ=Ӏ[1];ҽ=true;}public override bool Ɨ(bool õ){if(!ҽ){f.Ǟ("Distance: "+Đ.Ǧ("DTU")+" '"+Ӂ+"'.");return true;}
IMyTerminalBlock â=Z.Y.â;if(ƚ.Ύ!=""&&ƚ.Ύ!="*"){â=f.Ǒ.GetBlockWithName(ƚ.Ύ);if(â==null){f.Ǟ("Distance: "+Đ.Ǧ("P1")+": "+ƚ.Ύ);return true;
}}double ҋ=Vector3D.Distance(â.GetPosition(),ҿ);f.Ó(Ҿ+": ");f.ƽ(f.ȍ(ҋ)+"m ");return true;}}class Ӗ:ƛ{ϯ ŝ;public Ӗ(){ɧ=2;ɫ
="CmdDocked";}public override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);}int Ĝ=0;int ӕ=0;bool Ӕ=false;bool ӓ=false;IMyShipConnector ŉ;
public override bool Ɨ(bool õ){if(!õ){if(ƚ.Ώ.EndsWith("e"))Ӕ=true;if(ƚ.Ώ.Contains("cn"))ӓ=true;ŝ.ų();Ĝ=0;}if(Ĝ==0){if(!ŝ.Ъ(
"connector",ƚ.Ύ,õ))return false;Ĝ++;ӕ=0;õ=false;}if(ŝ.Ч()<=0){f.Ǟ("Docked: "+Đ.Ǧ("DO1"));return true;}for(;ӕ<ŝ.Ч();ӕ++){ŉ=ŝ.Ϭ[ӕ]as
IMyShipConnector;if(ŉ.Status==MyShipConnectorStatus.Connected){if(ӓ){f.Ó(ŉ.CustomName+":");f.ƽ(ŉ.OtherConnector.CubeGrid.CustomName);}
else{f.Ǟ(ŉ.OtherConnector.CubeGrid.CustomName);}}else{if(Ӕ){if(ӓ){f.Ó(ŉ.CustomName+":");f.ƽ("-");}else f.Ǟ("-");}}}return
true;}}class Ӓ:ƛ{public Ӓ(){ɧ=30;ɫ="CmdEcho";}public override bool Ɨ(bool õ){string ʺ=(ƚ.Ώ=="center"?"c":(ƚ.Ώ=="right"?"r":
"n"));switch(ʺ){case"c":f.Ƶ(ƚ.Ί);break;case"r":f.ƽ(ƚ.Ί);break;default:f.Ǟ(ƚ.Ί);break;}return true;}}class ӑ:ƛ{public ӑ(){ɧ=
1;ɫ="CmdGravity";}public override bool Ɨ(bool õ){string ʺ=(ƚ.Ώ.Contains("nat")?"n":(ƚ.Ώ.Contains("art")?"a":(ƚ.Ώ.Contains
("tot")?"t":"s")));Vector3D Н;if(f.ǒ.ʉ==null){f.Ǟ("Gravity: "+Đ.Ǧ("GNC"));return true;}switch(ʺ){case"n":f.Ó(Đ.Ǧ("G2")+
" ");Н=f.ǒ.ʉ.GetNaturalGravity();f.ƽ(Н.Length().ToString("F1")+" m/s²");break;case"a":f.Ó(Đ.Ǧ("G3")+" ");Н=f.ǒ.ʉ.
GetArtificialGravity();f.ƽ(Н.Length().ToString("F1")+" m/s²");break;case"t":f.Ó(Đ.Ǧ("G1")+" ");Н=f.ǒ.ʉ.GetTotalGravity();f.ƽ(Н.Length().
ToString("F1")+" m/s²");break;default:f.Ó(Đ.Ǧ("GN"));f.ƻ(" | ",0.33f);f.ƻ(Đ.Ǧ("GA")+" | ",0.66f);f.ƽ(Đ.Ǧ("GT"),1.0f);f.Ó("");Н=f
.ǒ.ʉ.GetNaturalGravity();f.ƻ(Н.Length().ToString("F1")+" | ",0.33f);Н=f.ǒ.ʉ.GetArtificialGravity();f.ƻ(Н.Length().
ToString("F1")+" | ",0.66f);Н=f.ǒ.ʉ.GetTotalGravity();f.ƽ(Н.Length().ToString("F1")+" ");break;}return true;}}class Ӑ:ƛ{public Ӑ
(){ɧ=0.5;ɫ="CmdHScroll";}ʦ ӏ=new ʦ();int ӎ=1;public override bool Ɨ(bool õ){if(ӏ.ʤ==0){string Ĺ=ƚ.Ί+"  ";if(Ĺ.Length==0)
return true;float һ=f.ɸ;float ƹ=f.ǳ(Ĺ,f.Ǚ);float Ѽ=һ/ƹ;if(Ѽ>1)ӏ.ʢ(string.Join("",Enumerable.Repeat(Ĺ,(int)Math.Ceiling(Ѽ))));
else ӏ.ʢ(Ĺ);if(Ĺ.Length>40)ӎ=3;else if(Ĺ.Length>5)ӎ=2;else ӎ=1;f.Ǟ(ӏ);return true;}bool Τ=ƚ.Ώ.EndsWith("r");if(Τ){ӏ.Ƽ.Insert
(0,ӏ.ɛ(ӏ.ʤ-ӎ,ӎ));ӏ.ʡ(ӏ.ʤ-ӎ,ӎ);}else{ӏ.ʢ(ӏ.ɛ(0,ӎ));ӏ.ʡ(0,ӎ);}f.Ǟ(ӏ);return true;}}class ҫ:ƛ{public ҫ(){ɧ=7;ɫ="CmdInvList";
}float Ҫ=-1;float ҩ=-1;public override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);ҳ=new ƥ(Ʋ,f);}ʦ Ƽ=new ʦ(100);Dictionary<string,string>Ҩ=
new Dictionary<string,string>();void ҧ(string Ȼ,double ҥ,int Ò){if(Ò>0){if(!ү)f.ǆ(Math.Min(100,100*ҥ/Ò),0.3f);string γ;if(Ҩ
.ContainsKey(Ȼ)){γ=Ҩ[Ȼ];}else{if(!ҭ)γ=f.ǲ(Ȼ,f.ɸ*0.5f-ң-ҩ);else{if(!ү)γ=f.ǲ(Ȼ,f.ɸ*0.69f);else γ=f.ǲ(Ȼ,f.ɸ*0.99f);}Ҩ[Ȼ]=γ;}
Ƽ.ų();if(!ү)Ƽ.ʢ(' ');if(!ҭ){f.Ó(Ƽ.ʢ(γ).ʢ(' '));f.ƻ(f.ȍ(ҥ),1.0f,ң+ҩ);f.Ǟ(Ƽ.ų().ʢ(" / ").ʢ(f.ȍ(Ò)));}else{f.Ǟ(Ƽ.ʢ(γ));}}
else{if(!ҭ){f.Ó(Ƽ.ų().ʢ(Ȼ).ʢ(':'));f.ƽ(f.ȍ(ҥ),1.0f,Ҫ);}else f.Ǟ(Ƽ.ų().ʢ(Ȼ));}}void Ҧ(string Ȼ,double ҥ,double Ҥ,int Ò){if(Ò>
0){if(!ҭ){f.Ó(Ƽ.ų().ʢ(Ȼ).ʢ(' '));f.ƻ(f.ȍ(ҥ),0.51f);f.Ó(Ƽ.ų().ʢ(" / ").ʢ(f.ȍ(Ò)));f.ƽ(Ƽ.ų().ʢ(" +").ʢ(f.ȍ(Ҥ)).ʢ(" ").ʢ(Đ.Ǧ
("I1")),1.0f);}else f.Ǟ(Ƽ.ų().ʢ(Ȼ));if(!ү)f.ǁ(Math.Min(100,100*ҥ/Ò));}else{if(!ҭ){f.Ó(Ƽ.ų().ʢ(Ȼ).ʢ(':'));f.ƻ(f.ȍ(ҥ),0.51f
);f.ƽ(Ƽ.ų().ʢ(" +").ʢ(f.ȍ(Ҥ)).ʢ(" ").ʢ(Đ.Ǧ("I1")),1.0f);}else{f.Ǟ(Ƽ.ų().ʢ(Ȼ));}}}float ң=0;bool Ң(ƕ Ɓ){int Ò=(ұ?Ɓ.Ɣ:Ɓ.Ɠ);
if(Ò<0)return true;float Ǆ=f.ǳ(f.ȍ(Ò),f.Ǚ);if(Ǆ>ң)ң=Ǆ;return true;}List<ƕ>Ҭ;int Ү=0;int ҹ=0;bool Һ(bool õ,bool Ҹ,string Ê,
string у){if(!õ){ҹ=0;Ү=0;}if(ҹ==0){if(ӫ){if((Ҭ=ҳ.ƃ(Ê,õ,Ң))==null)return false;}else{if((Ҭ=ҳ.ƃ(Ê,õ))==null)return false;}ҹ++;õ=
false;}if(Ҭ.Count>0){if(!Ҹ&&!õ){if(!f.Ǜ)f.Ǟ();f.Ƶ(Ƽ.ų().ʢ("<< ").ʢ(у).ʢ(" ").ʢ(Đ.Ǧ("I2")).ʢ(" >>"));}for(;Ү<Ҭ.Count;Ү++){if(!
Ʋ.ȯ(30))return false;double ҥ=Ҭ[Ү].Ƒ;if(ұ&&ҥ>=Ҭ[Ү].Ɣ)continue;int Ò=Ҭ[Ү].Ɠ;if(ұ)Ò=Ҭ[Ү].Ɣ;string Ȼ=f.Ȉ(Ҭ[Ү].Ë,Ҭ[Ү].Ê);ҧ(Ȼ,
ҥ,Ò);}}return true;}List<ƕ>ҷ;int Ҷ=0;int ҵ=0;bool Ҵ(bool õ){if(!õ){Ҷ=0;ҵ=0;}if(ҵ==0){if((ҷ=ҳ.ƃ("Ingot",õ))==null)return
false;ҵ++;õ=false;}if(ҷ.Count>0){if(!Ұ&&!õ){if(!f.Ǜ)f.Ǟ();f.Ƶ(Ƽ.ų().ʢ("<< ").ʢ(Đ.Ǧ("I4")).ʢ(" ").ʢ(Đ.Ǧ("I2")).ʢ(" >>"));}for(
;Ҷ<ҷ.Count;Ҷ++){if(!Ʋ.ȯ(40))return false;double ҥ=ҷ[Ҷ].Ƒ;if(ұ&&ҥ>=ҷ[Ҷ].Ɣ)continue;int Ò=ҷ[Ҷ].Ɠ;if(ұ)Ò=ҷ[Ҷ].Ɣ;string Ȼ=f.Ȉ
(ҷ[Ҷ].Ë,ҷ[Ҷ].Ê);if(ҷ[Ҷ].Ë!="Scrap"){double Ҥ=ҳ.Ƈ(ҷ[Ҷ].Ë+" Ore",ҷ[Ҷ].Ë,"Ore").Ƒ;Ҧ(Ȼ,ҥ,Ҥ,Ò);}else ҧ(Ȼ,ҥ,Ò);}}return true;}ϯ
ŝ=null;ƥ ҳ;List<ʷ>ͽ;bool Ҳ,Ϊ,ұ,Ұ,ҭ,ү;int Ŝ,ř;string ӭ="";float Ӭ=0;bool ӫ=true;void Ӫ(){if(f.Ǚ!=ӭ||Ӭ!=f.ɸ){Ҩ.Clear();Ӭ=f.
ɸ;}if(f.Ǚ!=ӭ){ҩ=f.ǳ(" / ",f.Ǚ);Ҫ=f.ȑ(' ',f.Ǚ);ӭ=f.Ǚ;}ŝ.ų();Ҳ=ƚ.Ώ.EndsWith("x")||ƚ.Ώ.EndsWith("xs");Ϊ=ƚ.Ώ.EndsWith("s")||ƚ
.Ώ.EndsWith("sx");ұ=ƚ.Ώ.StartsWith("missing");Ұ=ƚ.Ώ.Contains("list");ү=ƚ.Ώ.Contains("nb");ҭ=ƚ.Ώ.Contains("nn");ӫ=true;ҳ.ų
();ͽ=ƚ.ͽ;if(ͽ.Count==0)ͽ.Add(new ʷ("all"));}bool ө(bool õ){if(!õ)Ŝ=0;for(;Ŝ<ͽ.Count;Ŝ++){ʷ Ŕ=ͽ[Ŝ];Ŕ.ʲ();string Ê=Ŕ.ʵ;if(!
õ)ř=0;else õ=false;for(;ř<Ŕ.ʴ.Count;ř++){if(!Ʋ.ȯ(30))return false;string[]Ĉ=Ŕ.ʴ[ř].Split(':');double ȏ;if(string.Compare(
Ĉ[0],"all",true)==0)Ĉ[0]="";int Ɣ=1;int Ɠ=-1;if(Ĉ.Length>1){if(Double.TryParse(Ĉ[1],out ȏ)){if(ұ)Ɣ=(int)Math.Ceiling(ȏ);
else Ɠ=(int)Math.Ceiling(ȏ);}}string ƨ=Ĉ[0];if(!string.IsNullOrEmpty(Ê))ƨ+=' '+Ê;ҳ.Ʃ(ƨ,Ŕ.ʶ=="-",Ɣ,Ɠ);}}return true;}int ҕ=0;
int ϣ=0;int Ө=0;List<MyInventoryItem>b=new List<MyInventoryItem>(50);bool ӧ(bool õ){ϯ Ш=ŝ;if(!õ)ҕ=0;for(;ҕ<Ш.Ϭ.Count;ҕ++){
if(!õ)ϣ=0;for(;ϣ<Ш.Ϭ[ҕ].InventoryCount;ϣ++){IMyInventory Ϣ=Ш.Ϭ[ҕ].GetInventory(ϣ);if(!õ){Ө=0;b.Clear();Ϣ.GetItems(b);}else
õ=false;for(;Ө<b.Count;Ө++){if(!Ʋ.ȯ(40))return false;MyInventoryItem ª=b[Ө];string Í=f.ȋ(ª);string Ë,Ê;f.ȉ(Í,out Ë,out Ê)
;if(string.Compare(Ê,"ore",true)==0){if(ҳ.Ɩ(Ë+" ingot",Ë,"Ingot")&&ҳ.Ɩ(Í,Ë,Ê))continue;}else{if(ҳ.Ɩ(Í,Ë,Ê))continue;}f.ȉ(
Í,out Ë,out Ê);ƕ Ɔ=ҳ.Ƈ(Í,Ë,Ê);Ɔ.Ƒ+=(double)ª.Amount;}}}return true;}int Ĝ=0;public override bool Ɨ(bool õ){if(!õ){Ӫ();Ĝ=0
;}for(;Ĝ<=14;Ĝ++){switch(Ĝ){case 0:if(!ŝ.Р(ƚ.Ύ,õ))return false;break;case 1:if(!ө(õ))return false;if(Ҳ)Ĝ++;break;case 2:
if(!ҳ.Ɗ(õ))return false;break;case 3:if(!ӧ(õ))return false;break;case 4:if(!Һ(õ,Ұ,"Ore",Đ.Ǧ("I3")))return false;break;case
5:if(Ϊ){if(!Һ(õ,Ұ,"Ingot",Đ.Ǧ("I4")))return false;}else{if(!Ҵ(õ))return false;}break;case 6:if(!Һ(õ,Ұ,"Component",Đ.Ǧ(
"I5")))return false;break;case 7:if(!Һ(õ,Ұ,"OxygenContainerObject",Đ.Ǧ("I6")))return false;break;case 8:if(!Һ(õ,true,
"GasContainerObject",""))return false;break;case 9:if(!Һ(õ,Ұ,"AmmoMagazine",Đ.Ǧ("I7")))return false;break;case 10:if(!Һ(õ,Ұ,
"PhysicalGunObject",Đ.Ǧ("I8")))return false;break;case 11:if(!Һ(õ,true,"Datapad",""))return false;break;case 12:if(!Һ(õ,Ұ,"PhysicalObject",
Đ.Ǧ("I9")))return false;break;case 13:if(!Һ(õ,Ұ,"ConsumableItem",Đ.Ǧ("I10")))return false;break;case 14:if(!Һ(õ,Ұ,
"SeedItem",Đ.Ǧ("I11")))return false;break;}õ=false;}ӫ=false;return true;}}class ӷ:ƛ{public ӷ(){ɧ=2;ɫ="CmdAmount";}ϯ ŝ;public
override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);}bool Ӷ;bool ӵ=false;int Ś=0;int Ŝ=0;int ř=0;public override bool Ɨ(bool õ){if(!õ){Ӷ=!ƚ.Ώ.
EndsWith("x");ӵ=ƚ.Ώ.EndsWith("bar");if(ӵ)Ӷ=true;if(ƚ.ͽ.Count==0)ƚ.ͽ.Add(new ʷ(
"reactor,gatlingturret,missileturret,interiorturret,gatlinggun,launcherreload,launcher,oxygenerator"));Ŝ=0;}for(;Ŝ<ƚ.ͽ.Count;Ŝ++){ʷ Ŕ=ƚ.ͽ[Ŝ];if(!õ){Ŕ.ʲ();Ś=0;ř=0;}for(;ř<Ŕ.ʴ.Count;ř++){if(Ś==0){if(!õ){if(Ŕ.ʴ[ř]=="")
continue;ŝ.ų();}string ŗ=Ŕ.ʴ[ř];if(!ŝ.Ъ(ŗ,ƚ.Ύ,õ))return false;Ś++;õ=false;}if(!Ӯ(õ))return false;õ=false;Ś=0;}}return true;}int
Ӵ=0;int İ=0;double Ɔ=0;double ӳ=0;double Ӳ=0;int Ө=0;IMyTerminalBlock ӱ;IMyInventory Ӱ;List<MyInventoryItem>b=new List<
MyInventoryItem>(50);string ӯ="";bool Ӯ(bool õ){if(!õ){Ӵ=0;İ=0;}for(;Ӵ<ŝ.Ч();Ӵ++){if(İ==0){if(!Ʋ.ȯ(50))return false;ӱ=ŝ.Ϭ[Ӵ];Ӱ=ӱ.
GetInventory(0);if(Ӱ==null)continue;İ++;õ=false;}if(!õ){b.Clear();Ӱ.GetItems(b);ӯ=(b.Count>0?b[0].Type.ToString():"");Ө=0;Ɔ=0;ӳ=0;Ӳ=
0;}for(;Ө<b.Count;Ө++){if(!Ʋ.ȯ(30))return false;MyInventoryItem ª=b[Ө];if(ª.Type.ToString()!=ӯ)Ӳ+=(double)ª.Amount;else Ɔ
+=(double)ª.Amount;}string Ӧ=Đ.Ǧ("A1");string Ɯ=ӱ.CustomName;if(Ɔ>0&&(double)Ӱ.CurrentVolume>0){double ӗ=Ӳ*(double)Ӱ.
CurrentVolume/(Ɔ+Ӳ);ӳ=Math.Floor(Ɔ*((double)Ӱ.MaxVolume-ӗ)/((double)Ӱ.CurrentVolume-ӗ));Ӧ=f.ȍ(Ɔ)+" / "+(Ӳ>0?"~":"")+f.ȍ(ӳ);}if(!ӵ||ӳ
<=0){Ɯ=f.ǲ(Ɯ,f.ɸ*0.8f);f.Ó(Ɯ);f.ƽ(Ӧ);}if(Ӷ&&ӳ>0){double ʭ=100*Ɔ/ӳ;f.ǁ(ʭ);}İ=0;õ=false;}return true;}}class ә:ƛ{ϯ ŝ;public
ә(){ɧ=2;ɫ="CmdMass";}public override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);}bool Ϊ=false;bool ρ=false;int Ĝ=0;public override bool Ɨ(
bool õ){if(!õ){ŝ.ų();Ϊ=(ƚ.Ώ[ƚ.Ώ.Length-1]=='x');ρ=(ƚ.Ώ[ƚ.Ώ.Length-1]=='p');Ĝ=0;}if(Ĝ==0){if(!ŝ.Р(ƚ.Ύ,õ))return false;Ĝ++;õ=
false;}double Î=ŝ.Ϥ(õ);if(Double.IsNaN(Î))return false;double ʮ=0;int ѵ=ƚ.ͽ.Count;if(ѵ>0){double.TryParse(ƚ.ͽ[0].Ŕ.Trim(),out
ʮ);if(ѵ>1){string Ѵ=ƚ.ͽ[1].Ŕ.Trim();char ѳ=' ';if(Ѵ.Length>0)ѳ=Char.ToLower(Ѵ[0]);int Ѳ="kmgtpezy".IndexOf(ѳ);if(Ѳ>=0)ʮ*=
Math.Pow(1000.0,Ѳ);}ʮ*=1000.0;}f.Ó(Đ.Ǧ("M1")+" ");if(ʮ<=0){f.ƽ(f.Ȝ(Î));return true;}double ʭ=Î/ʮ*100;if(!Ϊ&&!ρ){f.ƽ(f.Ȝ(Î)+
" / "+f.Ȝ(ʮ));f.ǆ(ʭ,1.0f,f.ɱ);f.Ǟ(' '+f.ȕ(ʭ)+"%");}else if(ρ){f.ƽ(f.ȕ(ʭ)+"%");f.ǁ(ʭ);}else f.ƽ(f.ȕ(ʭ)+"%");return true;}}
class Ә:ƛ{ɉ Ǔ;ϯ ŝ;public Ә(){ɧ=3;ɫ="CmdOxygen";}public override void ɯ(){Ǔ=f.Ǔ;ŝ=new ϯ(Ʋ,f.Œ);}int Ĝ=0;int Ċ=0;bool Ɖ=false;
bool Ϊ=false;bool Ӛ=false;int ӛ=0;double Ʉ=0;double Ʌ=0;double ǀ;string ξ;public override bool Ɨ(bool õ){if(!õ){ŝ.ų();Ĝ=0;Ċ=
0;ǀ=0;Ϊ=ƚ.Ώ.EndsWith("x");Ӛ=ƚ.Ώ.ǿ("level");if(ξ==null&&ƚ.ͽ.Count>0){if(ƚ.ͽ[0].Ŕ.Length>0)ξ=ƚ.ͽ[0].Ŕ;}}if(Ĝ==0){if(!ŝ.Ъ(
"airvent",ƚ.Ύ,õ))return false;Ɖ=(ŝ.Ч()>0);Ĝ++;õ=false;}if(Ĝ==1){for(;Ċ<ŝ.Ч();Ċ++){if(!Ʋ.ȯ(8))return false;var Ő=ŝ.Ϭ[Ċ]as
IMyAirVent;ǀ=Math.Max(Ő.GetOxygenLevel()*100,0f);f.Ó(ξ??Ő.CustomName);if(Ő.CanPressurize||Ӛ)f.ƽ(f.ȕ(ǀ)+"%");else f.ƽ(Đ.Ǧ("O1"));if
(!Ϊ)f.ǁ(ǀ);}Ĝ++;õ=false;}if(Ĝ==2){if(!õ)ŝ.ų();if(!ŝ.Ъ("oxyfarm",ƚ.Ύ,õ))return false;ӛ=ŝ.Ч();Ĝ++;õ=false;}if(Ĝ==3){if(ӛ>0)
{if(!õ)Ċ=0;double ӥ=0;for(;Ċ<ӛ;Ċ++){if(!Ʋ.ȯ(4))return false;var Ӥ=ŝ.Ϭ[Ċ]as IMyOxygenFarm;ӥ+=Ӥ.GetOutput()*100;}ǀ=ӥ/ӛ;if(Ɖ
)f.Ǟ("");Ɖ|=(ӛ>0);f.Ó(ξ??Đ.Ǧ("O2"));f.ƽ(f.ȕ(ǀ)+"%");if(!Ϊ)f.ǁ(ǀ);}Ĝ++;õ=false;}if(Ĝ==4){if(!õ)ŝ.ų();if(!ŝ.Ъ("oxytank",ƚ.Ύ
,õ))return false;ӛ=ŝ.Ч();if(ӛ==0){if(!Ɖ)f.Ǟ(Đ.Ǧ("O3"));return true;}Ĝ++;õ=false;}if(Ĝ==5){if(!õ){Ʉ=0;Ʌ=0;Ċ=0;}if(!Ǔ.Ɏ(ŝ.Ϭ
,"oxygen",ref Ʌ,ref Ʉ,õ))return false;if(Ʉ==0){if(!Ɖ)f.Ǟ(Đ.Ǧ("O3"));return true;}ǀ=Ʌ/Ʉ*100;if(Ɖ)f.Ǟ("");f.Ó(ξ??Đ.Ǧ("O4"))
;f.ƽ(f.ȕ(ǀ)+"%");if(!Ϊ)f.ǁ(ǀ);Ĝ++;}return true;}}class ӣ:ƛ{public ӣ(){ɧ=1;ɫ="CmdPosition";}public override bool Ɨ(bool õ)
{bool Ӣ=(ƚ.Ώ=="posxyz");bool Ӂ=(ƚ.Ώ=="posgps");IMyTerminalBlock â=Z.Y.â;if(ƚ.Ύ!=""&&ƚ.Ύ!="*"){â=f.Ǒ.GetBlockWithName(ƚ.Ύ)
;if(â==null){f.Ǟ("Pos: "+Đ.Ǧ("P1")+": "+ƚ.Ύ);return true;}}if(Ӂ){Vector3D ņ=â.GetPosition();f.Ǟ("GPS:"+Đ.Ǧ("P2")+":"+ņ.
GetDim(0).ToString("F2")+":"+ņ.GetDim(1).ToString("F2")+":"+ņ.GetDim(2).ToString("F2")+":");return true;}f.Ó(Đ.Ǧ("P2")+": ");
if(!Ӣ){f.ƽ(â.GetPosition().ToString("F0"));return true;}f.Ǟ("");f.Ó(" X: ");f.ƽ(â.GetPosition().GetDim(0).ToString("F0"));
f.Ó(" Y: ");f.ƽ(â.GetPosition().GetDim(1).ToString("F0"));f.Ó(" Z: ");f.ƽ(â.GetPosition().GetDim(2).ToString("F0"));
return true;}}class ӡ:ƛ{public ӡ(){ɧ=3;ɫ="CmdPower";}ɉ Ǔ;ϯ Ӡ;ϯ ӟ;ϯ Ӟ;ϯ ѩ;ϯ ӝ;ϯ ŝ;public override void ɯ(){Ӡ=new ϯ(Ʋ,f.Œ);ӟ=new
ϯ(Ʋ,f.Œ);Ӟ=new ϯ(Ʋ,f.Œ);ѩ=new ϯ(Ʋ,f.Œ);ӝ=new ϯ(Ʋ,f.Œ);ŝ=new ϯ(Ʋ,f.Œ);Ǔ=f.Ǔ;}string у;bool Ӝ;string Ѩ;string ξ;int с;int Ĝ
=0;bool њ=true;public override bool Ɨ(bool õ){if(!õ){у=(ƚ.Ώ.EndsWith("x")?"s":(ƚ.Ώ.EndsWith("p")?"p":(ƚ.Ώ.EndsWith("v")?
"v":(ƚ.Ώ.EndsWith("bar")?"b":"n"))));Ӝ=(ƚ.Ώ.StartsWith("powersummary"));Ѩ="a";ξ="";if(ƚ.Ώ.Contains("stored"))Ѩ="s";else if(
ƚ.Ώ.Contains("in"))Ѩ="i";else if(ƚ.Ώ.Contains("out"))Ѩ="o";Ĝ=0;Ӡ.ų();ӟ.ų();Ӟ.ų();ѩ.ų();ӝ.ų();if(њ)return false;}else{if(њ
){њ=false;õ=false;}}if(Ѩ=="a"){if(Ĝ==0){if(!Ӡ.Ъ("reactor",ƚ.Ύ,õ))return false;õ=false;Ĝ++;}if(Ĝ==1){if(!ӟ.Ъ(
"hydrogenengine",ƚ.Ύ,õ))return false;õ=false;Ĝ++;}if(Ĝ==2){if(!Ӟ.Ъ("solarpanel",ƚ.Ύ,õ))return false;õ=false;Ĝ++;}if(Ĝ==3){if(!ӝ.Ъ(
"windturbine",ƚ.Ύ,õ))return false;õ=false;Ĝ++;}}else if(Ĝ==0)Ĝ=4;if(Ĝ==4){if(!ѩ.Ъ("battery",ƚ.Ύ,õ))return false;õ=false;Ĝ++;}int љ=Ӡ.
Ч();int ј=ӟ.Ч();int ї=Ӟ.Ч();int і=ѩ.Ч();int ѕ=ӝ.Ч();if(Ĝ==5){с=0;if(љ>0)с++;if(ј>0)с++;if(ї>0)с++;if(ѕ>0)с++;if(і>0)с++;
if(с<1){f.Ǟ(Đ.Ǧ("P6"));return true;}if(ƚ.ͽ.Count>0){if(ƚ.ͽ[0].Ŕ.Length>0)ξ=ƚ.ͽ[0].Ŕ;}Ĝ++;õ=false;}if(Ѩ!="a"){if(!Ѫ(ѩ,(ξ==
""&&у!="b"?Đ.Ǧ("P7"):ξ),Ѩ,у,õ))return false;return true;}string я=Đ.Ǧ("P8");if(!Ӝ){if(Ĝ==6){if(љ>0)if(!ё(Ӡ,(ξ==""?Đ.Ǧ("P9"
):ξ),у,õ))return false;Ĝ++;õ=false;}if(Ĝ==7){if(ј>0)if(!ё(ӟ,(ξ==""?Đ.Ǧ("P12"):ξ),у,õ))return false;Ĝ++;õ=false;}if(Ĝ==8){
if(ї>0)if(!ё(Ӟ,(ξ==""?Đ.Ǧ("P10"):ξ),у,õ))return false;Ĝ++;õ=false;}if(Ĝ==9){if(ѕ>0)if(!ё(ӝ,(ξ==""?Đ.Ǧ("P13"):ξ),у,õ))
return false;Ĝ++;õ=false;}if(Ĝ==10){if(і>0)if(!Ѫ(ѩ,(ξ==""?Đ.Ǧ("P7"):ξ),Ѩ,у,õ))return false;Ĝ++;õ=false;}}else{я=(ξ==""?Đ.Ǧ(
"P11"):ξ);с=10;if(Ĝ==6)Ĝ=11;}if(с==1)return true;if(!õ){ŝ.ų();ŝ.Щ(Ӡ);ŝ.Щ(ӟ);ŝ.Щ(Ӟ);ŝ.Щ(ӝ);ŝ.Щ(ѩ);}if(!ё(ŝ,я,у,õ))return false
;return true;}void п(double ʱ,double Ɠ){double ю=(Ɠ>0?ʱ/Ɠ*100:0);switch(у){case"s":f.ƽ(Ƽ.ų().ʢ(' ').ʢ(ю.ToString("F1")).ʢ
("%"));break;case"v":f.ƽ(Ƽ.ų().ʢ(f.ȍ(ʱ)).ʢ("W / ").ʢ(f.ȍ(Ɠ)).ʢ("W"));break;case"c":f.ƽ(Ƽ.ų().ʢ(f.ȍ(ʱ)).ʢ("W"));break;case
"p":f.ƽ(Ƽ.ų().ʢ(' ').ʢ(ю.ToString("F1")).ʢ("%"));f.ǁ(ю);break;case"b":f.ǁ(ю);break;default:f.ƽ(Ƽ.ų().ʢ(f.ȍ(ʱ)).ʢ("W / ").ʢ(
f.ȍ(Ɠ)).ʢ("W"));f.ǆ(ю,1.0f,f.ɱ);f.ƽ(Ƽ.ų().ʢ(' ').ʢ(ю.ToString("F1")).ʢ("%"));break;}}double є=0;double в=0,ѓ=0;int ђ=0;
bool ё(ϯ ѐ,string я,string ʺ,bool õ){if(!õ){в=0;ѓ=0;ђ=0;}if(ђ==0){if(!Ǔ.ɕ(ѐ.Ϭ,Ǔ.Ɉ,ref є,ref є,ref в,ref ѓ,õ))return false;ђ
++;õ=false;}if(!Ʋ.ȯ(250))return false;double ю=(ѓ>0?в/ѓ*100:0);if(я!="")f.Ó(я+": ");п(в*1000000,ѓ*1000000);return true;}
double ћ=0,ќ=0,Ѯ=0,ѯ=0;double ѭ=0,Ѭ=0;int ѫ=0;ʦ Ƽ=new ʦ(100);bool Ѫ(ϯ ѩ,string я,string Ѩ,string ʺ,bool õ){if(!õ){ћ=ќ=0;Ѯ=ѯ=0;
ѭ=Ѭ=0;ѫ=0;}if(ѫ==0){if(!Ǔ.Ɍ(ѩ.Ϭ,ref Ѯ,ref ѯ,ref ћ,ref ќ,ref ѭ,ref Ѭ,õ))return false;Ѯ*=1000000;ѯ*=1000000;ћ*=1000000;ќ*=
1000000;ѭ*=1000000;Ѭ*=1000000;ѫ++;õ=false;}double ѧ=(Ѭ>0?ѭ/Ѭ*100:0);double Ѧ=(ќ>0?ћ/ќ*100:0);double ѥ=(ѯ>0?Ѯ/ѯ*100:0);bool Ѥ=Ѩ
=="a";if(ѫ==1){if(!Ʋ.ȯ(200))return false;if(Ѥ){if(ʺ!="p"){if(я!="")f.Ó(Ƽ.ų().ʢ(я).ʢ(": "));f.ƽ(Ƽ.ų().ʢ("(IN ").ʢ(f.ȍ(Ѯ)).ʢ
("W / OUT ").ʢ(f.ȍ(ћ)).ʢ("W)"));}else if(я!="")f.Ǟ(Ƽ.ų().ʢ(я).ʢ(": "));f.Ó(Ƽ.ų().ʢ("  ").ʢ(Đ.Ǧ("P3")).ʢ(": "));}else if(я
!="")f.Ó(Ƽ.ų().ʢ(я).ʢ(": "));if(Ѥ||Ѩ=="s")switch(ʺ){case"s":f.ƽ(Ƽ.ų().ʢ(' ').ʢ(ѧ.ToString("F1")).ʢ("%"));break;case"v":f.ƽ
(Ƽ.ų().ʢ(f.ȍ(ѭ)).ʢ("Wh / ").ʢ(f.ȍ(Ѭ)).ʢ("Wh"));break;case"p":f.ƽ(Ƽ.ų().ʢ(' ').ʢ(ѧ.ToString("F1")).ʢ("%"));f.ǁ(ѧ);break;
case"b":f.ǁ(ѧ);break;default:f.ƽ(Ƽ.ų().ʢ(f.ȍ(ѭ)).ʢ("Wh / ").ʢ(f.ȍ(Ѭ)).ʢ("Wh"));f.ǆ(ѧ,1.0f,f.ɱ);f.ƽ(Ƽ.ų().ʢ(' ').ʢ(ѧ.ToString
("F1")).ʢ("%"));break;}if(Ѩ=="s")return true;ѫ++;õ=false;}if(ѫ==2){if(!Ʋ.ȯ(150))return false;if(Ѥ)f.Ó(Ƽ.ų().ʢ("  ").ʢ(Đ.Ǧ
("P4")).ʢ(": "));if(Ѥ||Ѩ=="o")switch(ʺ){case"s":f.ƽ(Ƽ.ų().ʢ(' ').ʢ(Ѧ.ToString("F1")).ʢ("%"));break;case"v":f.ƽ(Ƽ.ų().ʢ(f.
ȍ(ћ)).ʢ("W / ").ʢ(f.ȍ(ќ)).ʢ("W"));break;case"p":f.ƽ(Ƽ.ų().ʢ(' ').ʢ(Ѧ.ToString("F1")).ʢ("%"));f.ǁ(Ѧ);break;case"b":f.ǁ(Ѧ);
break;default:f.ƽ(Ƽ.ų().ʢ(f.ȍ(ћ)).ʢ("W / ").ʢ(f.ȍ(ќ)).ʢ("W"));f.ǆ(Ѧ,1.0f,f.ɱ);f.ƽ(Ƽ.ų().ʢ(' ').ʢ(Ѧ.ToString("F1")).ʢ("%"));
break;}if(Ѩ=="o")return true;ѫ++;õ=false;}if(!Ʋ.ȯ(150))return false;if(Ѥ)f.Ó(Ƽ.ų().ʢ("  ").ʢ(Đ.Ǧ("P5")).ʢ(": "));if(Ѥ||Ѩ=="i"
)switch(ʺ){case"s":f.ƽ(Ƽ.ų().ʢ(' ').ʢ(ѥ.ToString("F1")).ʢ("%"));break;case"v":f.ƽ(Ƽ.ų().ʢ(f.ȍ(Ѯ)).ʢ("W / ").ʢ(f.ȍ(ѯ)).ʢ(
"W"));break;case"p":f.ƽ(Ƽ.ų().ʢ(' ').ʢ(ѥ.ToString("F1")).ʢ("%"));f.ǁ(ѥ);break;case"b":f.ǁ(ѥ);break;default:f.ƽ(Ƽ.ų().ʢ(f.ȍ(
Ѯ)).ʢ("W / ").ʢ(f.ȍ(ѯ)).ʢ("W"));f.ǆ(ѥ,1.0f,f.ɱ);f.ƽ(Ƽ.ų().ʢ(' ').ʢ(ѥ.ToString("F1")).ʢ("%"));break;}return true;}}class ѣ
:ƛ{public ѣ(){ɧ=7;ɫ="CmdPowerTime";}class Ѣ{public TimeSpan ī=new TimeSpan(-1);public double ы=-1;public double ѡ=0;}Ѣ Ѡ=
new Ѣ();ϯ џ;ϯ ў;public override void ɯ(){џ=new ϯ(Ʋ,f.Œ);ў=new ϯ(Ʋ,f.Œ);}int ѝ=0;double э=0;double ь=0,Э=0;double н=0,м=0,л=
0;double к=0,й=0;int и=0;private bool з(string Ύ,out TimeSpan ж,out double υ,bool õ){MyResourceSourceComponent ȃ;
MyResourceSinkComponent ȭ;double е=ɨ;Ѣ д=Ѡ;ж=д.ī;υ=д.ы;if(!õ){џ.ų();ў.ų();д.ы=0;ѝ=0;э=0;ь=Э=0;н=0;м=л=0;к=й=0;и=0;}if(ѝ==0){if(!џ.Ъ("reactor",Ύ
,õ))return false;õ=false;ѝ++;}if(ѝ==1){for(;и<џ.Ϭ.Count;и++){if(!Ʋ.ȯ(200))return false;var â=џ.Ϭ[и]as IMyReactor;if(â==
null||!â.IsWorking)continue;if(â.Components.TryGet<MyResourceSourceComponent>(out ȃ)){ь+=ȃ.CurrentOutputByType(f.Ǔ.Ɉ);Э+=ȃ.
MaxOutputByType(f.Ǔ.Ɉ);}э+=(double)â.GetInventory(0).CurrentMass;}õ=false;ѝ++;}if(ѝ==2){if(!ў.Ъ("battery",Ύ,õ))return false;õ=false;ѝ++
;}if(ѝ==3){if(!õ)и=0;for(;и<ў.Ϭ.Count;и++){if(!Ʋ.ȯ(300))return false;var â=ў.Ϭ[и]as IMyBatteryBlock;if(â==null||!â.
IsWorking)continue;if(â.Components.TryGet<MyResourceSourceComponent>(out ȃ)){м=ȃ.CurrentOutputByType(f.Ǔ.Ɉ);л=ȃ.MaxOutputByType(f
.Ǔ.Ɉ);}if(â.Components.TryGet<MyResourceSinkComponent>(out ȭ)){м-=ȭ.CurrentInputByType(f.Ǔ.Ɉ);}double г=(м<0?(â.
MaxStoredPower-â.CurrentStoredPower)/(-м/3600):0);if(г>д.ы)д.ы=г;if(â.ChargeMode==ChargeMode.Recharge)continue;к+=м;й+=л;н+=â.
CurrentStoredPower;}õ=false;ѝ++;}double в=ь+к;if(в<=0)д.ī=TimeSpan.FromSeconds(-1);else{double б=д.ī.TotalSeconds;double а;double Я=(д.ѡ-э
)/е;if(ь<=0)Я=Math.Min(в,Э)/3600000;double Ю=0;if(й>0)Ю=Math.Min(в,й)/3600;if(Я<=0&&Ю<=0)а=-1;else if(Я<=0)а=н/Ю;else if(
Ю<=0)а=э/Я;else{double о=Ю;double р=(ь<=0?в/3600:Я*в/ь);а=н/о+э/р;}if(б<=0||а<0)б=а;else б=(б+а)/2;try{д.ī=TimeSpan.
FromSeconds(б);}catch{д.ī=TimeSpan.FromSeconds(-1);}}д.ѡ=э;υ=д.ы;ж=д.ī;return true;}int Ĝ=0;bool κ=false;bool Ϊ=false;bool ρ=false;
double ы=0;TimeSpan ȩ;int ъ=0,щ=0,ш=0;int Ȑ=0;int ч=0;public override bool Ɨ(bool õ){if(!õ){κ=ƚ.Ώ.EndsWith("bar");Ϊ=(ƚ.Ώ[ƚ.Ώ.
Length-1]=='x');ρ=(ƚ.Ώ[ƚ.Ώ.Length-1]=='p');Ĝ=0;ъ=щ=ш=Ȑ=0;ч=0;ы=0;}if(Ĝ==0){if(ƚ.ͽ.Count>0){for(;ч<ƚ.ͽ.Count;ч++){if(!Ʋ.ȯ(100))
return false;ƚ.ͽ[ч].ʲ();if(ƚ.ͽ[ч].ʴ.Count<=0)continue;string Ŕ=ƚ.ͽ[ч].ʴ[0];int.TryParse(Ŕ,out Ȑ);if(ч==0)ъ=Ȑ;else if(ч==1)щ=Ȑ;
else if(ч==2)ш=Ȑ;}}Ĝ++;õ=false;}if(Ĝ==1){if(!з(ƚ.Ύ,out ȩ,out ы,õ))return false;Ĝ++;õ=false;}if(!Ʋ.ȯ(150))return false;double
ī=0;TimeSpan ц;try{ц=new TimeSpan(ъ,щ,ш);}catch{ц=TimeSpan.FromSeconds(-1);}string Ĺ;if(ȩ.TotalSeconds>0||ы<=0){if(!κ)f.Ó
(Đ.Ǧ("PT1")+" ");Ĺ=f.Ǔ.Ȫ(ȩ);ī=ȩ.TotalSeconds;}else{if(!κ)f.Ó(Đ.Ǧ("PT2")+" ");TimeSpan х;try{х=TimeSpan.FromSeconds(ы);}
catch{х=new TimeSpan(-1);}Ĺ=f.Ǔ.Ȫ(х);if(ц.TotalSeconds>=ы)ī=ц.TotalSeconds-ы;else ī=0;}if(ц.Ticks<=0){f.ƽ(Ĺ);return true;}
double ʭ=ī/ц.TotalSeconds*100;if(ʭ>100)ʭ=100;if(κ){f.ǁ(ʭ);return true;}if(!Ϊ&&!ρ){f.ƽ(Ĺ);f.ǆ(ʭ,1.0f,f.ɱ);f.Ǟ(' '+ʭ.ToString(
"0.0")+"%");}else if(ρ){f.ƽ(ʭ.ToString("0.0")+"%");f.ǁ(ʭ);}else f.ƽ(ʭ.ToString("0.0")+"%");return true;}}class ф:ƛ{public ф()
{ɧ=7;ɫ="CmdPowerUsed";}ɉ Ǔ;ϯ ŝ;public override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);Ǔ=f.Ǔ;}string у;string т;string Ͻ;void п(double ʱ,
double Ɠ){double ю=(Ɠ>0?ʱ/Ɠ*100:0);switch(у){case"s":f.ƽ(ю.ToString("0.0")+"%",1.0f);break;case"v":f.ƽ(f.ȍ(ʱ)+"W / "+f.ȍ(Ɠ)+
"W",1.0f);break;case"c":f.ƽ(f.ȍ(ʱ)+"W",1.0f);break;case"p":f.ƽ(ю.ToString("0.0")+"%",1.0f);f.ǁ(ю);break;default:f.ƽ(f.ȍ(ʱ)+
"W / "+f.ȍ(Ɠ)+"W");f.ǆ(ю,1.0f,f.ɱ);f.ƽ(' '+ю.ToString("0.0")+"%");break;}}double ɓ=0,ɒ=0;int ҕ=0;int Ĝ=0;Ґ Ҕ=new Ґ();public
override bool Ɨ(bool õ){if(!õ){у=(ƚ.Ώ.EndsWith("x")?"s":(ƚ.Ώ.EndsWith("usedp")||ƚ.Ώ.EndsWith("topp")?"p":(ƚ.Ώ.EndsWith("v")?"v":
(ƚ.Ώ.EndsWith("c")?"c":"n"))));т=(ƚ.Ώ.Contains("top")?"top":"");Ͻ=(ƚ.ͽ.Count>0?ƚ.ͽ[0].Ŕ:Đ.Ǧ("PU1"));ɓ=ɒ=0;Ĝ=0;ҕ=0;ŝ.ų();Ҕ
.n();}if(Ĝ==0){if(!ŝ.Р(ƚ.Ύ,õ))return false;õ=false;Ĝ++;}MyResourceSinkComponent ȭ;MyResourceSourceComponent ȃ;switch(т){
case"top":if(Ĝ==1){for(;ҕ<ŝ.Ϭ.Count;ҕ++){if(!Ʋ.ȯ(200))return false;IMyTerminalBlock â=ŝ.Ϭ[ҕ];if(â.Components.TryGet<
MyResourceSinkComponent>(out ȭ)){ListReader<MyDefinitionId>ȫ=ȭ.AcceptedResources;if(ȫ.IndexOf(Ǔ.Ɉ)<0)continue;ɓ=ȭ.CurrentInputByType(Ǔ.Ɉ)*
1000000;}else continue;Ҕ.À(ɓ,â);}õ=false;Ĝ++;}if(Ҕ.z()<=0){f.Ǟ("PowerUsedTop: "+Đ.Ǧ("D2"));return true;}int ľ=10;if(ƚ.ͽ.Count>0
)if(!int.TryParse(Ͻ,out ľ)){ľ=10;}if(ľ>Ҕ.z())ľ=Ҕ.z();if(Ĝ==2){if(!õ){ҕ=Ҕ.z()-1;Ҕ.k();}for(;ҕ>=Ҕ.z()-ľ;ҕ--){if(!Ʋ.ȯ(200))
return false;IMyTerminalBlock â=Ҕ.r(ҕ);string Ɯ=f.ǲ(â.CustomName,f.ɸ*0.4f);if(â.Components.TryGet<MyResourceSinkComponent>(out
ȭ)){ɓ=ȭ.CurrentInputByType(Ǔ.Ɉ)*1000000;ɒ=ȭ.MaxRequiredInputByType(Ǔ.Ɉ)*1000000;var ґ=(â as IMyRadioAntenna);if(ґ!=null)ɒ
*=ґ.Radius/500;}f.Ó(Ɯ+" ");п(ɓ,ɒ);}}break;default:for(;ҕ<ŝ.Ϭ.Count;ҕ++){if(!Ʋ.ȯ(200))return false;double ғ;
IMyTerminalBlock â=ŝ.Ϭ[ҕ];if(â.Components.TryGet<MyResourceSinkComponent>(out ȭ)){ListReader<MyDefinitionId>ȫ=ȭ.AcceptedResources;if(ȫ.
IndexOf(Ǔ.Ɉ)<0)continue;ғ=ȭ.CurrentInputByType(Ǔ.Ɉ);double Ғ=ȭ.MaxRequiredInputByType(Ǔ.Ɉ);var ґ=(â as IMyRadioAntenna);if(ґ!=
null){Ғ*=ґ.Radius/500;}ɒ+=Ғ;}else continue;if(â.Components.TryGet<MyResourceSourceComponent>(out ȃ)&&(â as IMyBatteryBlock!=
null)){ғ-=ȃ.CurrentOutputByType(Ǔ.Ɉ);if(ғ<=0)continue;}ɓ+=ғ;}f.Ó(Ͻ);п(ɓ*1000000,ɒ*1000000);break;}return true;}public class
Ґ{List<KeyValuePair<double,IMyTerminalBlock>>ҏ=new List<KeyValuePair<double,IMyTerminalBlock>>();public void À(double Ҏ,
IMyTerminalBlock â){ҏ.Add(new KeyValuePair<double,IMyTerminalBlock>(Ҏ,â));}public int z(){return ҏ.Count;}public IMyTerminalBlock r(int
o){return ҏ[o].Value;}public void n(){ҏ.Clear();}public void k(){ҏ.Sort((Ѕ,ҡ)=>(Ѕ.Key.CompareTo(ҡ.Key)));}}}class Ҡ:ƛ{ϯ ŝ
;public Ҡ(){ɧ=1;ɫ="CmdProp";}public override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);}int Ĝ=0;int ҕ=0;string җ="b";bool ҟ=false;string Ҟ=
null;string ҝ=null;string Ҝ=null;string қ=null;string Қ=null;public override bool Ɨ(bool õ){if(!õ){ҟ=ƚ.Ώ.StartsWith("props")
;җ=ƚ.Ώ.Contains("float")?"f":"b";Ҟ=ҝ=қ=Қ=null;ҕ=0;Ĝ=0;}if(ƚ.ͽ.Count<1){f.Ǟ(ƚ.Ώ+": "+"Missing property name.");return true
;}if(Ĝ==0){if(!õ)ŝ.ų();if(!ŝ.Р(ƚ.Ύ,õ))return false;Ҙ(җ);Ĝ++;õ=false;}if(Ĝ==1){int ľ=ŝ.Ч();if(ľ==0){f.Ǟ(ƚ.Ώ+": "+
"No blocks found.");return true;}for(;ҕ<ľ;ҕ++){if(!Ʋ.ȯ(50))return false;IMyTerminalBlock â=ŝ.Ϭ[ҕ];if(â.GetProperty(Ҟ)!=null){if(ҝ==null){
string Ͻ=f.ǲ(â.CustomName,f.ɸ*0.7f);f.Ó(Ͻ);}else f.Ó(ҝ);string ҙ="N/A";if(җ=="b")ҙ=Ѱ(â,Ҟ,қ,Қ);else if(җ=="f")ҙ=Җ(â,Ҟ);if(Ҝ!=
null)ҙ+=" "+Ҝ;f.ƽ(ҙ);if(!ҟ)return true;}}}return true;}void Ҙ(string җ){Ҟ=ƚ.ͽ[0].Ŕ;if(ƚ.ͽ.Count>1){if(җ=="b"){if(!ҟ)ҝ=ƚ.ͽ[1]
.Ŕ;else қ=ƚ.ͽ[1].Ŕ;}else if(җ=="f"){if(!ҟ)ҝ=ƚ.ͽ[1].Ŕ;else Ҝ=ƚ.ͽ[1].Ŕ;}if(ƚ.ͽ.Count>2){if(җ=="b"){if(!ҟ)қ=ƚ.ͽ[2].Ŕ;else Қ=
ƚ.ͽ[2].Ŕ;if(ƚ.ͽ.Count>3&&!ҟ)Қ=ƚ.ͽ[3].Ŕ;}else if(җ=="f"){if(!ҟ)Ҝ=ƚ.ͽ[2].Ŕ;}}}}string Җ(IMyTerminalBlock â,string ѻ){return
f.ȍ(â.GetValue<float>(ѻ));}string Ѱ(IMyTerminalBlock â,string ѻ,string Ѻ=null,string ѹ=null){return(â.GetValue<bool>(ѻ)?(
Ѻ??Đ.Ǧ("W9")):(ѹ??Đ.Ǧ("W1")));}}class Ѹ:ƛ{public Ѹ(){ɧ=5;ɫ="CmdShipCtrl";}ϯ ŝ;public override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);}
public override bool Ɨ(bool õ){if(!õ)ŝ.ų();if(!ŝ.Ъ("shipctrl",ƚ.Ύ,õ))return false;if(ŝ.Ч()<=0){if(ƚ.Ύ!=""&&ƚ.Ύ!="*")f.Ǟ(ƚ.Ώ+
": "+Đ.Ǧ("SC1")+" ("+ƚ.Ύ+")");else f.Ǟ(ƚ.Ώ+": "+Đ.Ǧ("SC1"));return true;}if(ƚ.Ώ.StartsWith("damp")){bool ш=(ŝ.Ϭ[0]as
IMyShipController).DampenersOverride;f.Ó(Đ.Ǧ("SCD"));f.ƽ(ш?"ON":"OFF");}else{bool ш=(ŝ.Ϭ[0]as IMyShipController).IsUnderControl;f.Ó(Đ.Ǧ(
"SCO"));f.ƽ(ш?"YES":"NO");}return true;}}class ѷ:ƛ{public ѷ(){ɧ=1;ɫ="CmdShipMass";}public override bool Ɨ(bool õ){bool Ѷ=ƚ.Ώ.
EndsWith("base");double ʮ=0;if(ƚ.Ύ!="")double.TryParse(ƚ.Ύ.Trim(),out ʮ);int ѵ=ƚ.ͽ.Count;if(ѵ>0){string Ѵ=ƚ.ͽ[0].Ŕ.Trim();char ѳ
=' ';if(Ѵ.Length>0)ѳ=Char.ToLower(Ѵ[0]);int Ѳ="kmgtpezy".IndexOf(ѳ);if(Ѳ>=0)ʮ*=Math.Pow(1000.0,Ѳ);}double ʒ=(Ѷ?f.ǒ.ʐ:f.ǒ.
ʑ);if(!Ѷ)f.Ó(Đ.Ǧ("SM1")+" ");else f.Ó(Đ.Ǧ("SM2")+" ");f.ƽ(f.Ȝ(ʒ,true,'k')+" ");if(ʮ>0)f.ǁ(ʒ/ʮ*100);return true;}}class ѱ:
ƛ{public ѱ(){ɧ=0.5;ɫ="CmdSpeed";}public override bool Ɨ(bool õ){double ʮ=0;double Ѽ=1;string ѳ="m/s";if(ƚ.Ώ.Contains(
"kmh")){Ѽ=3.6;ѳ="km/h";}else if(ƚ.Ώ.Contains("mph")){Ѽ=2.23694;ѳ="mph";}if(ƚ.Ύ!="")double.TryParse(ƚ.Ύ.Trim(),out ʮ);f.Ó(Đ.Ǧ(
"S1")+" ");f.ƽ((f.ǒ.ʚ*Ѽ).ToString("F1")+" "+ѳ+" ");if(ʮ>0)f.ǁ(f.ǒ.ʚ/ʮ*100);return true;}}class ҍ:ƛ{public ҍ(){ɧ=1;ɫ=
"CmdStopTask";}public override bool Ɨ(bool õ){double Ҍ;if(ƚ.Ώ.Contains("best"))Ҍ=f.ǒ.ʚ/f.ǒ.ʖ;else Ҍ=f.ǒ.ʚ/f.ǒ.ʓ;double ҋ=f.ǒ.ʚ/2*Ҍ;if
(ƚ.Ώ.Contains("time")){f.Ó(Đ.Ǧ("ST"));if(double.IsNaN(Ҍ)){f.ƽ("N/A");return true;}string Ĺ="";try{var Ο=TimeSpan.
FromSeconds(Ҍ);if((int)Ο.TotalDays>0)Ĺ=" > 24h";else{if(Ο.Hours>0)Ĺ=Ο.Hours+"h ";if(Ο.Minutes>0||Ĺ!="")Ĺ+=Ο.Minutes+"m ";Ĺ+=Ο.
Seconds+"s";}}catch{Ĺ="N/A";}f.ƽ(Ĺ);return true;}f.Ó(Đ.Ǧ("SD"));if(!double.IsNaN(ҋ)&&!double.IsInfinity(ҋ))f.ƽ(f.ȍ(ҋ)+"m ");
else f.ƽ("N/A");return true;}}class Ҋ:ƛ{ɉ Ǔ;ϯ ŝ;public Ҋ(){ɧ=2;ɫ="CmdTanks";}public override void ɯ(){Ǔ=f.Ǔ;ŝ=new ϯ(Ʋ,f.Œ);}
int Ĝ=0;char у='n';string ҁ;double Ҁ=0;double ѿ=0;double ǀ;string ξ;bool κ=false;public override bool Ɨ(bool õ){List<ʷ>ͽ=ƚ.
ͽ;if(ͽ.Count==0){f.Ǟ(Đ.Ǧ("T4"));return true;}if(!õ){у=(ƚ.Ώ.EndsWith("x")?'s':(ƚ.Ώ.EndsWith("p")?'p':(ƚ.Ώ.EndsWith("v")?
'v':'n')));κ=ƚ.Ώ.EndsWith("bar");Ĝ=0;if(ҁ==null){ҁ=ͽ[0].Ŕ.Trim();ҁ=char.ToUpper(ҁ[0])+ҁ.Substring(1).ToLower();}if(ξ==null
&&ƚ.ͽ.Count>1){if(ƚ.ͽ[1].Ŕ.Length>0)ξ=ƚ.ͽ[1].Ŕ;}ŝ.ų();Ҁ=0;ѿ=0;}if(Ĝ==0){if(!ŝ.Ъ("oxytank",ƚ.Ύ,õ))return false;õ=false;Ĝ++;
}if(Ĝ==1){if(!ŝ.Ъ("hydrogenengine",ƚ.Ύ,õ))return false;õ=false;Ĝ++;}if(Ĝ==2){if(!Ǔ.Ɏ(ŝ.Ϭ,ҁ,ref Ҁ,ref ѿ,õ))return false;õ=
false;Ĝ++;}if(ѿ==0){f.Ǟ(String.Format(Đ.Ǧ("T5"),ҁ));return true;}ǀ=Ҁ/ѿ*100;if(κ){f.ǁ(ǀ);return true;}f.Ó(ξ??ҁ);switch(у){case
's':f.ƽ(' '+f.ȕ(ǀ)+"%");break;case'v':f.ƽ(f.ȍ(Ҁ)+"L / "+f.ȍ(ѿ)+"L");break;case'p':f.ƽ(' '+f.ȕ(ǀ)+"%");f.ǁ(ǀ);break;default:
f.ƽ(f.ȍ(Ҁ)+"L / "+f.ȍ(ѿ)+"L");f.ǆ(ǀ,1.0f,f.ɱ);f.ƽ(' '+f.ȕ(ǀ)+"%");break;}return true;}}class Ѿ{public string B="Debug";
public float ѽ=1.0f;public List<ʦ>ŧ=new List<ʦ>(30);public int ŷ=0;public float ʪ=0;public Ѿ(){ŧ.Add(new ʦ());}public void ů(
string Ĺ){ŧ[ŷ].ʢ(Ĺ);}public void ů(ʦ Ů){ŧ[ŷ].ʢ(Ů);}public void ŭ(){ŧ.Add(new ʦ());ŷ++;ʪ=0;}public void ŭ(string Ŭ){ŧ[ŷ].ʢ(Ŭ);ŭ
();}public void ū(List<ʦ>Ū){if(ŧ[ŷ].ʤ==0)ŧ.RemoveAt(ŷ);else ŷ++;ŧ.AddList(Ū);ŷ+=Ū.Count-1;ŭ();}public List<ʦ>ţ(){if(ŧ[ŷ].
ʤ==0)return ŧ.GetRange(0,ŷ);else return ŧ;}public void ũ(string Ũ,string H=""){string[]ŧ=Ũ.Split('\n');for(int A=0;A<ŧ.
Length;A++)ŭ(H+ŧ[A]);}public void Ŧ(){ŧ.Clear();ŭ();ŷ=0;}public int ť(){return ŷ+(ŧ[ŷ].ʤ>0?1:0);}public string Ť(){return
String.Join("\n",ŧ);}public void ţ(List<ʦ>Ţ,int Ł,int š){int Š=Ł+š;int Ń=ť();if(Š>Ń)Š=Ń;for(int A=Ł;A<Š;A++)Ţ.Add(ŧ[A]);}}
class ž{ʅ f=null;public float Ž=1.0f;public int ż=17;public int Ż=0;int ź=1;int Ź=1;public List<Ѿ>Ÿ=new List<Ѿ>(10);public
int ŷ=0;public ž(ʅ X){f=X;}public void Ŷ(int ľ){Ź=ľ;}public void ŵ(){ż=(int)Math.Floor(ʅ.ʂ*Ž*Ź/ʅ.ʀ);}public void Ŵ(Ѿ Ĺ){Ÿ.
Add(Ĺ);}public void ų(){Ÿ.Clear();}public int ť(){int ľ=0;foreach(var Ĺ in Ÿ){ľ+=Ĺ.ť();}return ľ;}ʦ Ų=new ʦ(256);public ʦ Ť
(){Ų.ų();int ľ=Ÿ.Count;for(int A=0;A<ľ-1;A++){Ų.ʢ(Ÿ[A].Ť());Ų.ʢ("\n");}if(ľ>0)Ų.ʢ(Ÿ[ľ-1].Ť());return Ų;}List<ʦ>ű=new List
<ʦ>(20);public ʦ Ű(int ş=0){Ų.ų();ű.Clear();if(Ź<=0)return Ų;int Ş=Ÿ.Count;int ĸ=0;int ň=(ż/Ź);int Ň=(ş*ň);int ņ=Ż+Ň;int
Ņ=ņ+ň;bool ń=false;for(int A=0;A<Ş;A++){Ѿ Ĺ=Ÿ[A];int Ń=Ĺ.ť();int ł=ĸ;ĸ+=Ń;if(!ń&&ĸ>ņ){int Ł=ņ-ł;if(ĸ>=Ņ){Ĺ.ţ(ű,Ł,Ņ-ł-Ł);
break;}ń=true;Ĺ.ţ(ű,Ł,Ń);continue;}if(ń){if(ĸ>=Ņ){Ĺ.ţ(ű,0,Ņ-ł);break;}Ĺ.ţ(ű,0,Ń);}}int ľ=ű.Count;for(int A=0;A<ľ-1;A++){Ų.ʢ(ű
[A]);Ų.ʢ("\n");}if(ľ>0)Ų.ʢ(ű[ľ-1]);return Ų;}public bool ŀ(int ľ=-1){if(ľ<=0)ľ=f.ɾ;if(Ż-ľ<=0){Ż=0;return true;}Ż-=ľ;
return false;}public bool Ŀ(int ľ=-1){if(ľ<=0)ľ=f.ɾ;int Ľ=ť();if(Ż+ľ+ż>=Ľ){Ż=Math.Max(Ľ-ż,0);return true;}Ż+=ľ;return false;}
public int ļ=0;public void Ļ(){if(ļ>0){ļ--;return;}if(ť()<=ż){Ż=0;ź=1;return;}if(ź>0){if(Ŀ()){ź=-1;ļ=2;}}else{if(ŀ()){ź=1;ļ=2;
}}}}class ĺ:ƛ{public ĺ(){ɧ=1;ɫ="CmdTextLCD";}public override bool Ɨ(bool õ){string Ĺ="";if(ƚ.Ύ!=""&&ƚ.Ύ!="*"){var Ā=f.Ǒ.
GetBlockWithName(ƚ.Ύ)as IMyTextPanel;if(Ā==null){f.Ǟ("TextLCD: "+Đ.Ǧ("T1")+ƚ.Ύ);return true;}Ĺ=Ā.GetText();}else{f.Ǟ("TextLCD:"+Đ.Ǧ("T2"
));return true;}if(Ĺ.Length==0)return true;f.ǝ(Ĺ);return true;}}class Ŋ:ƛ{public Ŋ(){ɧ=5;ɫ="CmdWorking";}ϯ ŝ;public
override void ɯ(){ŝ=new ϯ(Ʋ,f.Œ);}int Ĝ=0;int Ŝ=0;bool ś;public override bool Ɨ(bool õ){if(!õ){Ĝ=0;ś=(ƚ.Ώ=="workingx");Ŝ=0;}if(ƚ
.ͽ.Count==0){if(Ĝ==0){if(!õ)ŝ.ų();if(!ŝ.Р(ƚ.Ύ,õ))return false;Ĝ++;õ=false;}if(!Ơ(ŝ,ś,"",õ))return false;return true;}for(
;Ŝ<ƚ.ͽ.Count;Ŝ++){ʷ Ŕ=ƚ.ͽ[Ŝ];if(!õ)Ŕ.ʲ();if(!ŕ(Ŕ,õ))return false;õ=false;}return true;}int Ś=0;int ř=0;string[]Ř;string ŗ
;string Ŗ;bool ŕ(ʷ Ŕ,bool õ){if(!õ){Ś=0;ř=0;}for(;ř<Ŕ.ʴ.Count;ř++){if(Ś==0){if(!õ){if(string.IsNullOrEmpty(Ŕ.ʴ[ř]))
continue;ŝ.ų();Ř=Ŕ.ʴ[ř].Split(':');ŗ=Ř[0];Ŗ=(Ř.Length>1?Ř[1]:"");}if(!string.IsNullOrEmpty(ŗ)){if(!ŝ.Ъ(ŗ,ƚ.Ύ,õ))return false;}
else{if(!ŝ.Р(ƚ.Ύ,õ))return false;}Ś++;õ=false;}if(!Ơ(ŝ,ś,Ŗ,õ))return false;Ś=0;õ=false;}return true;}string œ(
IMyTerminalBlock â){Ц Œ=f.Œ;if(!â.IsWorking)return Đ.Ǧ("W1");var ő=â as IMyProductionBlock;if(ő!=null)if(ő.IsProducing)return Đ.Ǧ("W2");
else return Đ.Ǧ("W3");var Ő=â as IMyAirVent;if(Ő!=null){if(Ő.CanPressurize)return(Ő.GetOxygenLevel()*100).ToString("F1")+"%"
;else return Đ.Ǧ("W4");}var ŏ=â as IMyGasTank;if(ŏ!=null)return(ŏ.FilledRatio*100).ToString("F1")+"%";var Ŏ=â as
IMyBatteryBlock;if(Ŏ!=null)return Œ.Ͼ(Ŏ);var ō=â as IMyJumpDrive;if(ō!=null)return Œ.Б(ō).ToString("0.0")+"%";var Ō=â as IMyLandingGear
;if(Ō!=null){switch((int)Ō.LockMode){case 0:return Đ.Ǧ("W8");case 1:return Đ.Ǧ("W10");case 2:return Đ.Ǧ("W7");}}var ŋ=â
as IMyDoor;if(ŋ!=null){if(ŋ.Status==DoorStatus.Open)return Đ.Ǧ("W5");return Đ.Ǧ("W6");}var ŉ=â as IMyShipConnector;if(ŉ!=
null){if(ŉ.Status==MyShipConnectorStatus.Unconnected)return Đ.Ǧ("W8");if(ŉ.Status==MyShipConnectorStatus.Connected)return Đ.
Ǧ("W7");else return Đ.Ǧ("W10");}var ſ=â as IMyLaserAntenna;if(ſ!=null)return Œ.Њ(ſ);var Ƌ=â as IMyRadioAntenna;if(Ƌ!=null
)return f.ȍ(Ƌ.Radius)+"m";var Ƥ=â as IMyBeacon;if(Ƥ!=null)return f.ȍ(Ƥ.Radius)+"m";var ƣ=â as IMyThrust;if(ƣ!=null&&ƣ.
ThrustOverride>0)return f.ȍ(ƣ.ThrustOverride)+"N";var Ƣ=â as IMyProjector;if(Ƣ!=null&&Ƣ.TotalBlocks>0)return(100f-100f*Ƣ.
RemainingBlocks/Ƣ.TotalBlocks).ToString("F1")+"%";return Đ.Ǧ("W9");}int ơ=0;bool Ơ(ϯ ă,bool Ɵ,string ƞ,bool õ){if(!õ)ơ=0;for(;ơ<ă.Ч();ơ
++){if(!Ʋ.ȯ(20))return false;IMyTerminalBlock â=ă.Ϭ[ơ];string Ɲ=(Ɵ?(â.IsWorking?Đ.Ǧ("W9"):Đ.Ǧ("W1")):œ(â));if(!string.
IsNullOrEmpty(ƞ)&&String.Compare(Ɲ,ƞ,true)!=0)continue;if(Ɵ)Ɲ=œ(â);string Ɯ=â.CustomName;Ɯ=f.ǲ(Ɯ,f.ɸ*0.7f);f.Ó(Ɯ);f.ƽ(Ɲ);}return true
;}}class ƛ:ɬ{public Ѿ Ĺ=null;protected ΐ ƚ;protected ʅ f;protected ę Z;protected ǧ Đ;public ƛ(){ɧ=3600;ɫ="CommandTask";}
public void ƙ(ę ÿ,ΐ Ƙ){Z=ÿ;f=Z.f;ƚ=Ƙ;Đ=f.Đ;}public virtual bool Ɨ(bool õ){f.Ǟ(Đ.Ǧ("UC")+": '"+ƚ.Ό+"'");return true;}public
override bool ʇ(bool õ){Ĺ=f.ǡ(Ĺ,Z.Y);if(!õ)f.Ŧ();return Ɨ(õ);}}class ƥ{Dictionary<string,string>Ʊ=new Dictionary<string,string>(
StringComparer.InvariantCultureIgnoreCase){{"ingot","ingot"},{"ore","ore"},{"component","component"},{"tool","physicalgunobject"},{
"ammo","ammomagazine"},{"consumable","consumableitem"},{"seed","seeditem"},{"object","physicalobject"},{"oxygen",
"oxygencontainerobject"},{"gas","gascontainerobject"}};Ȩ Ʋ;ʅ f;ƍ ư;ƍ Ư;ƍ Ʈ;ķ ƭ;bool Ƭ;public ƍ ƫ;public ƥ(Ȩ ƪ,ʅ X){ư=new ƍ();Ư=new ƍ();Ʈ=new ƍ(
);Ƭ=false;ƫ=new ƍ();Ʋ=ƪ;f=X;ƭ=f.ƭ;}public void ų(){Ʈ.n();Ư.n();ư.n();Ƭ=false;ƫ.n();}public void Ʃ(string ƨ,bool ƒ=false,
int Ɣ=1,int Ɠ=-1){if(string.IsNullOrEmpty(ƨ)){Ƭ=true;return;}string[]Ƨ=ƨ.Split(' ');string Ê="";var Ɓ=new ƕ(ƒ,Ɣ,Ɠ);if(Ƨ.
Length==2){if(!Ʊ.TryGetValue(Ƨ[1],out Ê))Ê=Ƨ[1];}string Ë=Ƨ[0];if(Ʊ.TryGetValue(Ë,out Ɓ.Ê)){Ư.À(Ɓ.Ê,Ɓ);return;}f.ȇ(ref Ë,ref Ê
);if(string.IsNullOrEmpty(Ê)){Ɓ.Ë=Ë;ư.À(Ɓ.Ë,Ɓ);return;}Ɓ.Ë=Ë;Ɓ.Ê=Ê;Ʈ.À(Ë+' '+Ê,Ɓ);}public ƕ Ʀ(string Í,string Ë,string Ê)
{ƕ Ɓ;Ɓ=Ʈ.w(Í);if(Ɓ!=null)return Ɓ;Ɓ=ư.w(Ë);if(Ɓ!=null)return Ɓ;Ɓ=Ư.w(Ê);if(Ɓ!=null)return Ɓ;return null;}public bool Ɩ(
string Í,string Ë,string Ê){ƕ Ɓ;bool Ɖ=false;Ɓ=Ư.w(Ê);if(Ɓ!=null){if(Ɓ.ƒ)return true;Ɖ=true;}Ɓ=ư.w(Ë);if(Ɓ!=null){if(Ɓ.ƒ)
return true;Ɖ=true;}Ɓ=Ʈ.w(Í);if(Ɓ!=null){if(Ɓ.ƒ)return true;Ɖ=true;}return!(Ƭ||Ɖ);}public ƕ ƈ(string Í,string Ë,string Ê){var
Ɔ=new ƕ();ƕ Ɓ=Ʀ(Í,Ë,Ê);if(Ɓ!=null){Ɔ.Ɣ=Ɓ.Ɣ;Ɔ.Ɠ=Ɓ.Ɠ;}Ɔ.Ë=Ë;Ɔ.Ê=Ê;ƫ.À(Í,Ɔ);return Ɔ;}public ƕ Ƈ(string Í,string Ë,string Ê)
{ƕ Ɔ=ƫ.w(Í);if(Ɔ==null)Ɔ=ƈ(Í,Ë,Ê);return Ɔ;}int ƅ=0;List<ƕ>Ƅ;public List<ƕ>ƃ(string Ê,bool õ,Func<ƕ,bool>Ƃ=null){if(!õ){Ƅ
=new List<ƕ>(5);ƅ=0;}for(;ƅ<ƫ.z();ƅ++){if(!Ʋ.ȯ(5))return null;ƕ Ɓ=ƫ.r(ƅ);if(Ɩ(Ɓ.Ë+' '+Ɓ.Ê,Ɓ.Ë,Ɓ.Ê))continue;if((string.
Compare(Ɓ.Ê,Ê,true)==0)&&(Ƃ==null||Ƃ(Ɓ)))Ƅ.Add(Ɓ);}return Ƅ;}int ƀ=0;public bool Ɗ(bool õ){if(!õ){ƀ=0;}for(;ƀ<ƭ.Á.Count;ƀ++){if
(!Ʋ.ȯ(10))return false;Æ ª=ƭ.b[ƭ.Á[ƀ]];if(!ª.Î)continue;string Í=ª.Ö+' '+ª.Ø;if(Ɩ(Í,ª.Ö,ª.Ø))continue;ƕ Ɔ=Ƈ(Í,ª.Ö,ª.Ø);if
(Ɔ.Ɠ==-1)Ɔ.Ɠ=ª.í;}return true;}}class ƕ{public int Ɣ;public int Ɠ;public string Ë="";public string Ê="";public bool ƒ;
public double Ƒ;public ƕ(bool Ɛ=false,int Ə=1,int Ǝ=-1){Ɣ=Ə;ƒ=Ɛ;Ɠ=Ǝ;}}class ƍ{Dictionary<string,ƕ>ƌ=new Dictionary<string,ƕ>(
StringComparer.InvariantCultureIgnoreCase);List<string>Á=new List<string>();public void À(string v,ƕ ª){if(!ƌ.ContainsKey(v)){Á.Add(v)
;ƌ.Add(v,ª);}}public int z(){return ƌ.Count;}public ƕ w(string v){if(ƌ.ContainsKey(v))return ƌ[v];return null;}public ƕ r
(int o){return ƌ[Á[o]];}public void n(){Á.Clear();ƌ.Clear();}public void k(){Á.Sort();}}class ķ{public Dictionary<string,
Æ>b=new Dictionary<string,Æ>(StringComparer.InvariantCultureIgnoreCase);Dictionary<string,Æ>Õ=new Dictionary<string,Æ>(
StringComparer.InvariantCultureIgnoreCase);public List<string>Á=new List<string>(50);public Dictionary<string,Æ>Ô=new Dictionary<
string,Æ>(StringComparer.InvariantCultureIgnoreCase);public void Ó(string Ë,string Ê,int Ò,string Ñ,string Ð,string Ï,bool Î){
if(Ê=="Ammo")Ê="AmmoMagazine";else if(Ê=="Tool")Ê="PhysicalGunObject";else if(Ê=="Consumable")Ê="ConsumableItem";else if(Ê
=="Seed")Ê="SeedItem";else if(Ê=="Object")Ê="PhysicalObject";if(Ñ.Length<=0)Ñ=this.È(Ë);string Í=Ë+' '+Ê;var ª=new Æ(Ë,Ê,Ò
,Ñ,Ð,Î);b.Add(Í,ª);if(!Õ.ContainsKey(Ë))Õ.Add(Ë,ª);if(Ð.Length>0)Ô.Add(Ð,ª);if(Ï.Length>0)Ô.Add(Ï,ª);Á.Add(Í);}public Æ Ì
(string Ë="",string Ê=""){if(b.ContainsKey(Ë+" "+Ê))return b[Ë+" "+Ê];if(string.IsNullOrEmpty(Ê)){Æ ª=null;Õ.TryGetValue(
Ë,out ª);return ª;}if(string.IsNullOrEmpty(Ë))for(int A=0;A<b.Count;A++){Æ ª=b[Á[A]];if(string.Compare(Ê,ª.Ø,true)==0)
return ª;}return null;}Ǥ É=new Ǥ("([a-z])([A-Z])");public string È(string Ç){return É.ȅ(Ç,"$1 $2");}}class Æ{public string Ö;
public string Ø;public int í;public string î;public string ì;public bool Î;public Æ(string ë,string ê,int é=0,string è="",
string ç="",bool æ=true){Ö=ë;Ø=ê;í=é;î=è;ì=ç;Î=æ;}}class å{ʅ f=null;public Ä ä=new Ä();public ž ã;public IMyTerminalBlock â;
public IMyTextSurface á;public int à=0;public int ß=0;public string Þ="";public string Ý="";public bool Ü=true;public
IMyTextSurface Û=>(Ù?á:â as IMyTextSurface);public int Ú=>(Ù?(f.Ǣ(â)?0:1):ä.z());public bool Ù=false;public å(ʅ X,string W){f=X;Ý=W;}
public å(ʅ X,string W,IMyTerminalBlock V,IMyTextSurface D,int U){f=X;Ý=W;â=V;á=D;à=U;Ù=true;}public bool S(){return ã.ť()>ã.ż
||ã.Ż!=0;}float R=1.0f;bool Q=false;public float P(){if(Q)return R;Q=true;return R;}float O=1.0f;bool N=false;public float
L(){if(N)return O;N=true;return O;}bool K=false;public void J(){if(K)return;if(!Ù){ä.k();â=ä.r(0);}int I=â.CustomName.
IndexOf("!MARGIN:");if(I<0||I+8>=â.CustomName.Length){ß=1;Þ=" ";}else{string H=â.CustomName.Substring(I+8);int G=H.IndexOf(" ")
;if(G>=0)H=H.Substring(0,G);if(!int.TryParse(H,out ß))ß=1;Þ=new String(' ',ß);}if(â.CustomName.Contains("!NOSCROLL"))Ü=
false;else Ü=true;K=true;}public void F(ž E=null){if(ã==null||â==null)return;if(E==null)E=ã;if(!Ù){var D=â as IMyTextSurface;
if(D!=null){float C=D.FontSize;string B=D.Font;for(int A=0;A<ä.z();A++){var Y=ä.r(A)as IMyTextSurface;if(Y==null)continue;
Y.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.LEFT;Y.FontSize=C;Y.Font=B;string a=E.Ű(A).ɛ();if(!f.ǔ.
SKIP_CONTENT_TYPE)Y.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;Y.WriteText(a);}}}else{á.Alignment=VRage.Game.GUI.
TextPanel.TextAlignment.LEFT;if(!f.ǔ.SKIP_CONTENT_TYPE)á.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;á.
WriteText(E.Ű().ɛ());}K=false;}public void Å(){if(â==null)return;if(Ù){á.WriteText("");return;}var D=â as IMyTextSurface;if(D==
null)return;for(int A=0;A<ä.z();A++){var Y=ä.r(A)as IMyTextSurface;if(Y==null)continue;Y.WriteText("");}}}class Ä{Dictionary
<string,IMyTerminalBlock>Ã=new Dictionary<string,IMyTerminalBlock>();Dictionary<IMyTerminalBlock,string>Â=new Dictionary<
IMyTerminalBlock,string>();List<string>Á=new List<string>(10);public void À(string v,IMyTerminalBlock ª){if(!Á.Contains(v)){Á.Add(v);Ã.
Add(v,ª);Â.Add(ª,v);}}public void º(string v){if(Á.Contains(v)){Á.Remove(v);Â.Remove(Ã[v]);Ã.Remove(v);}}public void µ(
IMyTerminalBlock ª){if(Â.ContainsKey(ª)){Á.Remove(Â[ª]);Ã.Remove(Â[ª]);Â.Remove(ª);}}public int z(){return Ã.Count;}public
IMyTerminalBlock w(string v){if(Á.Contains(v))return Ã[v];return null;}public IMyTerminalBlock r(int o){return Ã[Á[o]];}public void n(){
Á.Clear();Ã.Clear();Â.Clear();}public void k(){Á.Sort();}}class j:ɬ{public ʅ f;public å e;ę Z;public j(ę ÿ){Z=ÿ;f=Z.f;e=Z
.Y;ɧ=0.5;ɫ="PanelDisplay";ɡ=true;}double ģ=0;public void ġ(){ģ=0;}int Ġ=0;int ğ=0;bool Ğ=true;double ĝ=double.MaxValue;
int Ĝ=0;public override bool ʇ(bool õ){ƛ ě;if(!õ&&(Z.ē==false||Z.ĕ==null||Z.ĕ.Count<=0))return true;if(Z.Ĕ.č>1)return ɟ(0);
if(!õ){ğ=0;Ğ=false;ĝ=double.MaxValue;Ĝ=0;}if(Ĝ==0){while(ğ<Z.ĕ.Count){if(!Ʋ.ȯ(5))return false;if(Z.Ė.TryGetValue(Z.ĕ[ğ],
out ě)){if(!ě.ɤ)return ɟ(ě.ɪ-Ʋ.Ȥ+0.001);if(ě.ɩ>ģ)Ğ=true;if(ě.ɪ<ĝ)ĝ=ě.ɪ;}ğ++;}Ĝ++;õ=false;}double Ě=ĝ-Ʋ.Ȥ+0.001;if(!Ğ&&!e.S(
))return ɟ(Ě);f.ǟ(e.ã,e);if(Ğ){if(!õ){ģ=Ʋ.Ȥ;e.ã.ų();Ġ=0;}while(Ġ<Z.ĕ.Count){if(!Ʋ.ȯ(7))return false;if(!Z.Ė.TryGetValue(Z
.ĕ[Ġ],out ě)){e.ã.Ÿ.Add(f.ǡ(null,e));f.Ŧ();f.Ǟ("ERR: No cmd task ("+Z.ĕ[Ġ]+")");Ġ++;continue;}e.ã.Ŵ(ě.Ĺ);Ġ++;}}f.ƾ(e);Z.Ĕ
.č++;if(ɧ<Ě&&!e.S())return ɟ(Ě);return true;}}class ę:ɬ{public ʅ f;public å Y;public j Ę=null;string ė="N/A";public
Dictionary<string,ƛ>Ė=new Dictionary<string,ƛ>();public List<string>ĕ=null;public ĥ Ĕ;public bool ē{get{return Ĕ.ú;}}public ę(ĥ Ē,
å đ){ɧ=5;Y=đ;Ĕ=Ē;f=Ē.f;ɫ="PanelProcess";}ǧ Đ;public override void ɯ(){Đ=f.Đ;}Random ď=new Random();ΐ Ģ=null;ƛ Ĥ(string Ķ,
bool õ){if(!õ)Ģ=new ΐ(Ʋ);if(!Ģ.ʲ(Ķ,õ))return null;ƛ ī=Ģ.Ά();ī.ƙ(this,Ģ);Ʋ.Ȱ(ī,0.1+ď.Next(1,20)/10.0);return ī;}string ĵ="";
void Ĵ(){try{ĵ=Y.â.Ǻ(Y.à,f.ɿ);}catch{ĵ="";return;}ĵ=ĵ?.Replace("\\\n","");}int Ġ=0;int ĳ=0;List<string>Ĳ=null;HashSet<string
>ı=new HashSet<string>();int İ=0;bool į(bool õ){if(!õ){char[]Į={';','\n'};string ĭ=ĵ.Replace("\\;","\f");if(ĭ.StartsWith(
"@")){int Ĭ=ĭ.IndexOf("\n");if(Ĭ<0){ĭ="";}else{ĭ=ĭ.Substring(Ĭ+1);}}Ĳ=new List<string>(ĭ.Split(Į,StringSplitOptions.
RemoveEmptyEntries));ı.Clear();Ġ=0;ĳ=0;İ=0;Ʋ.ȳ(50);}while(Ġ<Ĳ.Count){if(!Ʋ.ȯ(100))return false;if(Ĳ[Ġ].StartsWith("//")){Ĳ.RemoveAt(Ġ);
continue;}Ĳ[Ġ]=Ĳ[Ġ].Replace('\f',';');if(!Ė.ContainsKey(Ĳ[Ġ])){if(İ!=1)õ=false;İ=1;ƛ ě=Ĥ(Ĳ[Ġ],õ);if(ě==null)return false;õ=false
;Ė.Add(Ĳ[Ġ],ě);İ=0;}if(!ı.Contains(Ĳ[Ġ]))ı.Add(Ĳ[Ġ]);Ġ++;}if(ĕ!=null){ƛ ī;while(ĳ<ĕ.Count){if(!Ʋ.ȯ(100))return false;if(!
ı.Contains(ĕ[ĳ]))if(Ė.TryGetValue(ĕ[ĳ],out ī)){ī.ɮ();Ė.Remove(ĕ[ĳ]);}ĳ++;}}ĕ=Ĳ;return true;}public override void ʆ(){if(ĕ
!=null){ƛ ī;for(int Ī=0;Ī<ĕ.Count;Ī++){if(Ė.TryGetValue(ĕ[Ī],out ī))ī.ɮ();}ĕ=null;}if(Ę!=null){Ę.ɮ();Ę=null;}else{}}ž ĩ=
null;string Ĩ="";bool ħ=false;int Ĝ=0;public override bool ʇ(bool õ){if(Y.Ú<=0){ɮ();return true;}if(!õ)Ĝ=0;if(Ĝ==0){if(!õ){Y
.ã=f.ǟ(Y.ã,Y);ĩ=f.ǟ(ĩ,Y);return false;}Ĝ++;õ=false;}if(Ĝ==1){if(!õ){Ĵ();if(ĵ==null){if(Y.Ù){Ĕ.ò(Y.á,Y.â as IMyTextPanel);
}else{ɮ();}return true;}if(Y.â.CustomName!=Ĩ){ħ=true;}else{ħ=false;}Ĩ=Y.â.CustomName;return false;}Ĝ++;õ=false;}if(Ĝ==2){
if(ĵ!=ė){if(!į(õ))return false;if(ĵ==""){ė="";if(Ĕ.ú){if(ĩ.Ÿ.Count<=0)ĩ.Ÿ.Add(f.ǡ(null,Y));else f.ǡ(ĩ.Ÿ[0],Y);f.Ŧ();f.Ǟ(Đ.
Ǧ("H1"));bool Ħ=Y.Ü;Y.Ü=false;f.ƾ(Y,ĩ);Y.Ü=Ħ;return true;}return this.ɟ(2);}ħ=true;}ė=ĵ;Ĝ++;õ=false;}if(Ę!=null&&ħ){Ʋ.Ⱦ(Ę
);Ę.ġ();Ʋ.Ȱ(Ę,0);}else if(Ę==null){Ę=new j(this);Ʋ.Ȱ(Ę,0);}return true;}}class ĥ:ɬ{const string Ď="T:!LCD!";public int č=
0;public ʅ f;public ɋ ä=new ɋ();ϯ þ;ϯ ý;Dictionary<å,ę>ü=new Dictionary<å,ę>();public Dictionary<IMyTextSurface,å>û=new
Dictionary<IMyTextSurface,å>();public bool ú=false;А ù=null;public ĥ(ʅ X){ɧ=5;f=X;ɫ="ProcessPanels";ɢ=true;}public override void ɯ
(){þ=new ϯ(Ʋ,f.Œ);ý=new ϯ(Ʋ,f.Œ);ù=new А(f,this);}int ø=0;bool ö(bool õ){if(!õ)ø=0;if(ø==0){if(!þ.Р(f.ɿ,õ))return false;ø
++;õ=false;}if(ø==1){if(f.ɿ=="T:[LCD]"&&Ď!="")if(!þ.Р(Ď,õ))return false;ø++;õ=false;}return true;}string ô(
IMyTerminalBlock â){int ó=â.CustomName.IndexOf("!LINK:");if(ó>=0&&â.CustomName.Length>ó+6){return â.CustomName.Substring(ó+6)+' '+â.
Position.ToString();}return â.EntityId.ToString();}public void ò(IMyTextSurface D,IMyTextPanel Y){å e;if(D==null)return;if(!û.
TryGetValue(D,out e))return;if(Y!=null){e.ä.µ(Y);}û.Remove(D);if(e.Ú<=0||e.Ù){ę ñ;if(ü.TryGetValue(e,out ñ)){ä.µ(e.Ý);ü.Remove(e);ñ
.ɮ();}}}void ð(IMyTerminalBlock â){var ï=â as IMyTextSurfaceProvider;var D=â as IMyTextSurface;if(D!=null){ò(D,â as
IMyTextPanel);return;}if(ï==null)return;for(int A=0;A<ï.SurfaceCount;A++){D=ï.GetSurface(A);ò(D,null);}}string W;string Č;bool ċ;int
Ċ=0;int ĉ=0;public override bool ʇ(bool õ){if(!õ){þ.ų();Ċ=0;ĉ=0;}if(!ö(õ))return false;while(Ċ<þ.Ч()){if(!Ʋ.ȯ(100))return
false;var â=(þ.Ϭ[Ċ]as IMyTerminalBlock);if(â==null||!â.IsWorking){þ.Ϭ.RemoveAt(Ċ);continue;}var ï=â as IMyTextSurfaceProvider
;var D=â as IMyTextSurface;var Y=â as IMyTextPanel;å e;W=ô(â);string[]Ĉ=W.Split(' ');Č=Ĉ[0];ċ=Ĉ.Length>1;if(Y!=null){if(û
.ContainsKey(D)){e=û[D];if(e.Ý==W+"@0"||(ċ&&e.Ý==Č)){Ċ++;continue;}ð(â);}if(!ċ){e=new å(f,W+"@0",â,D,0);var ñ=new ę(this,
e);Ʋ.Ȱ(ñ,0,false,true);ü.Add(e,ñ);ä.À(e.Ý,e);û.Add(D,e);Ċ++;continue;}e=ä.w(Č);if(e==null){e=new å(f,Č);ä.À(Č,e);var ñ=
new ę(this,e);Ʋ.Ȱ(ñ,0,false,true);ü.Add(e,ñ);}e.ä.À(W,â);û.Add(D,e);}else{if(ï==null){Ċ++;continue;}for(int A=0;A<ï.
SurfaceCount;A++){D=ï.GetSurface(A);if(û.ContainsKey(D)){e=û[D];if(e.Ý==W+'@'+A.ToString()){continue;}ò(D,null);}if(â.Ǻ(A,f.ɿ)==null
)continue;e=new å(f,W+"@"+A.ToString(),â,D,A);var ñ=new ę(this,e);Ʋ.Ȱ(ñ,0,false,true);ü.Add(e,ñ);ä.À(e.Ý,e);û.Add(D,e);}}
Ċ++;}while(ĉ<ý.Ч()){if(!Ʋ.ȯ(100))return false;IMyTerminalBlock â=ý.Ϭ[ĉ];if(â==null)continue;if(!þ.Ϭ.Contains(â)){ð(â);}ĉ
++;}ý.ų();ý.Щ(þ);if(!ù.ɥ&&ù.ʹ())Ʋ.Ȱ(ù,0,false,true);return true;}public bool ć(string Ć){if(string.Compare(Ć,"clear",true)
==0){ù.Ў();if(!ù.ɥ)Ʋ.Ȱ(ù,0);return true;}if(string.Compare(Ć,"boot",true)==0){ù.Џ=0;if(!ù.ɥ)Ʋ.Ȱ(ù,0);return true;}if(Ć.Ȁ(
"scroll")){var ą=new Ϙ(f,this,Ć);Ʋ.Ȱ(ą,0);return true;}if(string.Compare(Ć,"props",true)==0){Ц Ą=f.Œ;var ă=new List<
IMyTerminalBlock>();var Ă=new List<ITerminalAction>();var ā=new List<ITerminalProperty>();var Ā=Ʋ.ǔ.GridTerminalSystem.GetBlockWithName(
"DEBUG")as IMyTextPanel;if(Ā==null){return true;}Ā.WriteText("Properties: ");foreach(var ª in Ą.ϻ){Ā.WriteText(ª.Key+
" =============="+"\n",true);ª.Value(ă,null);if(ă.Count<=0){Ā.WriteText("No blocks\n",true);continue;}ă[0].GetProperties(ā,(e)=>{return e
.Id!="Name"&&e.Id!="OnOff"&&!e.Id.StartsWith("Show");});foreach(var Ƴ in ā){Ā.WriteText("P "+Ƴ.Id+" "+Ƴ.TypeName+"\n",
true);}ā.Clear();ă.Clear();}}return false;}}class ɋ{Dictionary<string,å>ƌ=new Dictionary<string,å>();List<string>Á=new List<
string>(30);public void À(string v,å ª){if(!ƌ.ContainsKey(v)){Á.Add(v);ƌ.Add(v,ª);}}public int z(){return ƌ.Count;}public å w(
string v){if(ƌ.ContainsKey(v))return ƌ[v];return null;}public å r(int o){return ƌ[Á[o]];}public void µ(string v){ƌ.Remove(v);Á
.Remove(v);}public void n(){Á.Clear();ƌ.Clear();}public void k(){Á.Sort();}}class ɉ{Ȩ Ʋ;ʅ f;public MyDefinitionId Ɉ=new
MyDefinitionId(typeof(VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_GasProperties),"Electricity");public MyDefinitionId ɇ=new
MyDefinitionId(typeof(VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_GasProperties),"Oxygen");public MyDefinitionId Ɇ=new
MyDefinitionId(typeof(VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_GasProperties),"Hydrogen");public ɉ(Ȩ ƪ,ʅ X){Ʋ=ƪ;f=X;}int
Ɋ=0;public bool Ɍ(List<IMyTerminalBlock>ă,ref double ɓ,ref double ɒ,ref double ɑ,ref double ɐ,ref double ə,ref double ɘ,
bool õ){if(!õ)Ɋ=0;MyResourceSinkComponent ȭ;MyResourceSourceComponent ȃ;for(;Ɋ<ă.Count;Ɋ++){if(!Ʋ.ȯ(600))return false;if(ă[Ɋ
].Components.TryGet<MyResourceSinkComponent>(out ȭ)){ɓ+=ȭ.CurrentInputByType(Ɉ);ɒ+=ȭ.MaxRequiredInputByType(Ɉ);}if(ă[Ɋ].
Components.TryGet<MyResourceSourceComponent>(out ȃ)){ɑ+=ȃ.CurrentOutputByType(Ɉ);ɐ+=ȃ.MaxOutputByType(Ɉ);}var ɗ=(ă[Ɋ]as
IMyBatteryBlock);ə+=ɗ.CurrentStoredPower;ɘ+=ɗ.MaxStoredPower;}return true;}int ɖ=0;public bool ɕ(List<IMyTerminalBlock>ă,MyDefinitionId
ɔ,ref double ɓ,ref double ɒ,ref double ɑ,ref double ɐ,bool õ){if(!õ)ɖ=0;MyResourceSinkComponent ȭ;
MyResourceSourceComponent ȃ;for(;ɖ<ă.Count;ɖ++){if(!Ʋ.ȯ(600))return false;if(ă[ɖ].Components.TryGet<MyResourceSinkComponent>(out ȭ)){ɓ+=ȭ.
CurrentInputByType(ɔ);ɒ+=ȭ.MaxRequiredInputByType(ɔ);}if(ă[ɖ].Components.TryGet<MyResourceSourceComponent>(out ȃ)){ɑ+=ȃ.
CurrentOutputByType(ɔ);ɐ+=ȃ.MaxOutputByType(ɔ);}}return true;}int ɏ=0;public bool Ɏ(List<IMyTerminalBlock>ă,string ɍ,ref double Ʌ,ref
double Ʉ,bool õ){if(!õ){ɏ=0;Ʉ=0;Ʌ=0;}MyResourceSinkComponent ȭ;for(;ɏ<ă.Count;ɏ++){if(!Ʋ.ȯ(600))return false;var ŏ=ă[ɏ]as
IMyGasTank;if(ŏ==null)continue;double Ȭ=0;if(ŏ.Components.TryGet<MyResourceSinkComponent>(out ȭ)){ListReader<MyDefinitionId>ȫ=ȭ.
AcceptedResources;int A=0;for(;A<ȫ.Count;A++){if(string.Compare(ȫ[A].SubtypeId.ToString(),ɍ,true)==0){Ȭ=ŏ.Capacity;Ʉ+=Ȭ;Ʌ+=Ȭ*ŏ.
FilledRatio;break;}}}}return true;}public string Ȫ(TimeSpan ȩ){string Ĺ="";if(ȩ.Ticks<=0)return"-";if((int)ȩ.TotalDays>0)Ĺ+=(long)ȩ
.TotalDays+" "+f.Đ.Ǧ("C5")+" ";if(ȩ.Hours>0||Ĺ!="")Ĺ+=ȩ.Hours+"h ";if(ȩ.Minutes>0||Ĺ!="")Ĺ+=ȩ.Minutes+"m ";return Ĺ+ȩ.
Seconds+"s";}}class Ȩ{public const double ȧ=0.005;public const int Ȧ=1000;public const int ȥ=5000;public double Ȥ{get{return Ȣ;
}}int ȣ=Ȧ;double Ȣ=0;List<ɬ>ȡ=new List<ɬ>(100);public MyGridProgram ǔ;public bool Ƞ=false;int ȟ=0;int Ȟ=0;public Ȩ(
MyGridProgram ǎ,int Ǎ=1,bool Ȯ=false){ǔ=ǎ;ȟ=Ǎ;Ƞ=Ȯ;}public void Ȱ(ɬ ñ,double Ƀ,bool ɂ=false,bool Ɂ=false){ñ.ɥ=true;ñ.ɠ(this);if(ɂ){ñ.ɪ
=Ȥ;ȡ.Insert(0,ñ);return;}var ɀ=ȧ*(Ƞ?5:1);if(Ƀ<=0)Ƀ=ɀ;ñ.ɪ=Ȥ+Ƀ;for(int A=0;A<ȡ.Count;A++){if(ȡ[A].ɪ>ñ.ɪ){ȡ.Insert(A,ñ);
return;}var ȿ=ȡ[A].ɪ;if(!Ɂ&&ñ.ɪ-ȿ<ɀ)ñ.ɪ=ȿ+ɀ;}ȡ.Add(ñ);}public void Ⱦ(ɬ ñ){if(ȡ.Contains(ñ)){ȡ.Remove(ñ);ñ.ɥ=false;}}public
void ȼ(ʦ Ƚ,int Ⱥ=1){if(ȟ==Ⱥ)ǔ.Echo(Ƚ.ɛ());}public void ȼ(string Ȼ,int Ⱥ=1){if(ȟ==Ⱥ)ǔ.Echo(Ȼ);}const double ȹ=(16.66666666/16
);double ȸ=0;public void ȷ(){ȸ+=ǔ.Runtime.TimeSinceLastRun.TotalSeconds*ȹ;}public void ȶ(){double ȵ=ǔ.Runtime.
TimeSinceLastRun.TotalSeconds*ȹ+ȸ;ȸ=0;Ȟ=0;Ȣ+=ȵ;ȣ=(int)Math.Min((ȵ*60)*Ȧ,ȥ-1000);if(Ƞ&&ȣ>Ȧ*2)ȣ=Ȧ*2;while(ȡ.Count>=1){ɬ ñ=ȡ[0];if((ȣ-ǔ.
Runtime.CurrentInstructionCount<=0)||(ñ.ɪ>Ȣ)){int ȴ=(int)(60*(ñ.ɪ-Ȣ));if(ȴ>=100){ǔ.Runtime.UpdateFrequency=UpdateFrequency.
Update100;}else{if(ȴ>=10||Ƞ)ǔ.Runtime.UpdateFrequency=UpdateFrequency.Update10;else ǔ.Runtime.UpdateFrequency=UpdateFrequency.
Update1;}return;}ȡ.Remove(ñ);if(!ñ.ɝ()){ǔ.Runtime.UpdateFrequency=UpdateFrequency.Update1;break;}}}public void ȳ(int š){Ȟ+=š;}
public int Ȳ(){return(ȥ-ǔ.Runtime.CurrentInstructionCount-Ȟ);}public bool ȯ(int ȱ){return((ȣ-ǔ.Runtime.CurrentInstructionCount
-Ȟ)>=ȱ);}public void ɚ(){}}class ʟ:ɬ{MyShipVelocities ʞ;public Vector3D ʝ{get{return ʞ.LinearVelocity;}}public Vector3D ʜ
{get{return ʞ.AngularVelocity;}}double ʛ=0;public double ʚ{get{if(ʋ!=null)return ʋ.GetShipSpeed();else return ʛ;}}double
ʙ=0;public double ʘ{get{return ʙ;}}double ʗ=0;public double ʖ{get{return ʗ;}}double ʕ=0;double ʔ=0;public double ʓ{get{
return ʕ;}}MyShipMass ʒ;public double ʑ{get{return ʒ.TotalMass;}}public double ʐ{get{return ʒ.BaseMass;}}double ʏ=double.NaN;
public double ʎ{get{return ʏ;}}double ʍ=double.NaN;public double ʌ{get{return ʍ;}}IMyShipController ʋ=null;IMySlimBlock ʊ=null
;public IMyShipController ʉ{get{return ʋ;}}public ʟ(Ȩ ƪ){ɫ="ShipMgr";Ʋ=ƪ;ɧ=0.5;}List<IMyTerminalBlock>ʈ=new List<
IMyTerminalBlock>(5);int ʠ=0;public override bool ʇ(bool õ){if(!õ){ʈ.Clear();Ʋ.ǔ.GridTerminalSystem.GetBlocksOfType<IMyShipController>(ʈ
);ʠ=0;if(ʋ!=null&&ʋ.CubeGrid.GetCubeBlock(ʋ.Position)!=ʊ)ʋ=null;}if(ʈ.Count>0){for(;ʠ<ʈ.Count;ʠ++){if(!Ʋ.ȯ(20))return
false;var ʨ=ʈ[ʠ]as IMyShipController;if(ʨ.IsMainCockpit||ʨ.IsUnderControl){ʋ=ʨ;ʊ=ʨ.CubeGrid.GetCubeBlock(ʨ.Position);if(ʨ.
IsMainCockpit){ʠ=ʈ.Count;break;}}}if(ʋ==null){ʋ=ʈ[0]as IMyShipController;ʊ=ʋ.CubeGrid.GetCubeBlock(ʋ.Position);}ʒ=ʋ.CalculateShipMass
();if(!ʋ.TryGetPlanetElevation(MyPlanetElevation.Sealevel,out ʏ))ʏ=double.NaN;if(!ʋ.TryGetPlanetElevation(
MyPlanetElevation.Surface,out ʍ))ʍ=double.NaN;ʞ=ʋ.GetShipVelocities();}double ʧ=ʛ;ʛ=ʝ.Length();ʙ=(ʛ-ʧ)/ɨ;if(-ʙ>ʗ)ʗ=-ʙ;if(-ʙ>ʕ){ʕ=-ʙ;ʔ=Ʋ.Ȥ
;}if(Ʋ.Ȥ-ʔ>5&&-ʙ>0.1)ʕ-=(ʕ+ʙ)*0.3f;return true;}}class ʦ{public StringBuilder Ƽ;public ʦ(int ʥ=0){Ƽ=new StringBuilder(ʥ);
}public int ʤ{get{return Ƽ.Length;}}public ʦ ų(){Ƽ.Clear();return this;}public ʦ ʢ(string ĭ){Ƽ.Append(ĭ);return this;}
public ʦ ʢ(double ʩ){Ƽ.Append(ʩ);return this;}public ʦ ʢ(char Ȑ){Ƽ.Append(Ȑ);return this;}public ʦ ʢ(ʦ ʣ){Ƽ.Append(ʣ.Ƽ);return
this;}public ʦ ʢ(string ĭ,int ȗ,int ɭ){Ƽ.Append(ĭ,ȗ,ɭ);return this;}public ʦ ʢ(char Ȑ,int š){Ƽ.Append(Ȑ,š);return this;}
public ʦ ʡ(int ȗ,int ɭ){Ƽ.Remove(ȗ,ɭ);return this;}public string ɛ(){return Ƽ.ToString();}public string ɛ(int ȗ,int ɭ){return
Ƽ.ToString(ȗ,ɭ);}public char this[int v]{get{return Ƽ[v];}}}class ɬ{public string ɫ="MMTask";public double ɪ=0;public
double ɩ=0;public double ɨ=0;public double ɧ=-1;double ɦ=-1;public bool ɥ=false;public bool ɤ=false;double ɣ=0;public bool ɢ=
false;public bool ɡ=false;protected Ȩ Ʋ;public void ɠ(Ȩ ƪ){Ʋ=ƪ;if(Ʋ.Ƞ){if(ɦ==-1){ɦ=ɧ;ɧ*=2;}else{ɧ=ɦ*2;}}else{if(ɦ!=-1){ɧ=ɦ;ɦ=
-1;}}}protected bool ɟ(double Ƀ){ɣ=Math.Max(Ƀ,0.0001);return true;}bool ɞ=false;public bool ɝ(){if(ɩ>0){ɨ=Ʋ.Ȥ-ɩ;ɤ=ʇ(!ɤ);}
else{var ɜ=false;if(!ɞ){ɨ=0;ɯ();ɞ=true;ɜ=true;ɤ=false;}if(!ɜ||!ɢ){ɤ=ʇ(false);if(!ɤ)ɩ=0.001;}}if(ɤ){ɩ=Ʋ.Ȥ;if((ɧ>=0||ɣ>0)&&ɥ)Ʋ
.Ȱ(this,(ɣ>0?ɣ:ɧ),false,ɡ);else{ɥ=false;ɩ=0;}}else{if(ɥ)Ʋ.Ȱ(this,0,true);}ɣ=0;return ɤ;}public void ɮ(){Ʋ.Ⱦ(this);ʆ();ɥ=
false;ɤ=false;ɩ=0;}public virtual void ɯ(){}public virtual bool ʇ(bool õ){return true;}public virtual void ʆ(){}}class ʅ{
public const float ʄ=512;public const float ʃ=ʄ/0.7783784f;public const float ʂ=ʄ/0.7783784f;public const float ʁ=ʃ;public
const float ʀ=37;public string ɿ="T:[LCD]";public int ɾ=1;public bool ɽ=true;public List<string>ɼ=null;public int ȟ=0;public
float ɻ=1.0f;public float ɺ=1.0f;public float ɹ{get{return ʁ*ǉ.ѽ;}}public float ɸ{get{return(float)ɹ-2*ɰ[Ǚ]*ß;}}string ɷ;
string ɶ;float ɵ=-1;Dictionary<string,float>ɴ=new Dictionary<string,float>(2);Dictionary<string,float>ɳ=new Dictionary<string,
float>(2);Dictionary<string,float>ɲ=new Dictionary<string,float>(2);public float ɱ{get{return ɲ[Ǚ];}}Dictionary<string,float>
ɰ=new Dictionary<string,float>(2);Dictionary<string,float>ȝ=new Dictionary<string,float>(2);Dictionary<string,float>ǚ=new
Dictionary<string,float>(2);int ß=0;string Þ="";Dictionary<string,char>ǘ=new Dictionary<string,char>(2);Dictionary<string,char>Ǘ=
new Dictionary<string,char>(2);Dictionary<string,char>ǖ=new Dictionary<string,char>(2);Dictionary<string,char>Ǖ=new
Dictionary<string,char>(2);public Ȩ Ʋ;public Program ǔ;public ɉ Ǔ;public Ц Œ;public ʟ ǒ;public ķ ƭ;public ǧ Đ;public
IMyGridTerminalSystem Ǒ{get{return ǔ.GridTerminalSystem;}}public IMyProgrammableBlock ǐ{get{return ǔ.Me;}}public Action<string>Ǐ{get{return ǔ
.Echo;}}public ʅ(Program ǎ,int Ǎ,Ȩ ƪ){Ʋ=ƪ;ȟ=Ǎ;ǔ=ǎ;Đ=new ǧ();}public void ǌ(){Ǔ=new ɉ(Ʋ,this);}public void ǋ(){Œ=new Ц(Ʋ,
this);Œ.Љ();}public void Ǌ(){ǒ=new ʟ(Ʋ);Ʋ.Ȱ(ǒ,0);}Ѿ ǉ=null;public string Ǚ{get{return ǉ.B;}}public bool Ǜ{get{return(ǉ.ť()==
0);}}public bool Ǣ(IMyTerminalBlock â){if(â==null||â.WorldMatrix==MatrixD.Identity)return true;return Ǒ.GetBlockWithId(â.
EntityId)==null;}public Ѿ ǡ(Ѿ Ǡ,å e){e.J();IMyTextSurface D=e.Û;if(Ǡ==null)Ǡ=new Ѿ();Ǡ.B=D.Font;if(!ɰ.ContainsKey(Ǡ.B))Ǡ.B=ɷ;Ǡ.ѽ
=e.L()*(D.SurfaceSize.X/D.TextureSize.X)*Math.Max(1.0f,D.TextureSize.X/D.TextureSize.Y)*ɻ/D.FontSize*(100f-D.TextPadding*
2)/100;Þ=e.Þ;ß=e.ß;ǉ=Ǡ;return Ǡ;}public ž ǟ(ž ã,å e){e.J();IMyTextSurface D=e.Û;if(ã==null)ã=new ž(this);ã.Ŷ(e.Ú);ã.Ž=e.P
()*(D.SurfaceSize.Y/D.TextureSize.Y)*Math.Max(1.0f,D.TextureSize.Y/D.TextureSize.X)*ɺ/D.FontSize*(100f-D.TextPadding*2)/
100;ã.ŵ();Þ=e.Þ;ß=e.ß;return ã;}public void Ǟ(){ǉ.ŭ();}public void Ǟ(ʦ Ŭ){if(ǉ.ʪ<=0)ǉ.ů(Þ);ǉ.ů(Ŭ);ǉ.ŭ();}public void Ǟ(
string Ŭ){if(ǉ.ʪ<=0)ǉ.ů(Þ);ǉ.ŭ(Ŭ);}public void ǝ(string Ũ){ǉ.ũ(Ũ,Þ);}public void ǜ(List<ʦ>ŧ){ǉ.ū(ŧ);}public void Ó(ʦ Ů,bool ǅ=
true){if(ǉ.ʪ<=0)ǉ.ů(Þ);ǉ.ů(Ů);if(ǅ)ǉ.ʪ+=ǳ(Ů,ǉ.B);}public void Ó(string Ĺ,bool ǅ=true){if(ǉ.ʪ<=0)ǉ.ů(Þ);ǉ.ů(Ĺ);if(ǅ)ǉ.ʪ+=ǳ(Ĺ,
ǉ.B);}public void ƽ(ʦ Ů,float ƺ=1.0f,float ƴ=0f){ƻ(Ů,ƺ,ƴ);ǉ.ŭ();}public void ƽ(string Ĺ,float ƺ=1.0f,float ƴ=0f){ƻ(Ĺ,ƺ,ƴ)
;ǉ.ŭ();}ʦ Ƽ=new ʦ();public void ƻ(ʦ Ů,float ƺ=1.0f,float ƴ=0f){float ƹ=ǳ(Ů,ǉ.B);float Ƹ=ƺ*ʁ*ǉ.ѽ-ǉ.ʪ-ƴ;if(ß>0)Ƹ-=2*ɰ[ǉ.B]*
ß;if(Ƹ<ƹ){ǉ.ů(Ů);ǉ.ʪ+=ƹ;return;}Ƹ-=ƹ;int Ʒ=(int)Math.Floor(Ƹ/ɰ[ǉ.B]);float ƶ=Ʒ*ɰ[ǉ.B];Ƽ.ų().ʢ(' ',Ʒ).ʢ(Ů);ǉ.ů(Ƽ);ǉ.ʪ+=ƶ+ƹ
;}public void ƻ(string Ĺ,float ƺ=1.0f,float ƴ=0f){float ƹ=ǳ(Ĺ,ǉ.B);float Ƹ=ƺ*ʁ*ǉ.ѽ-ǉ.ʪ-ƴ;if(ß>0)Ƹ-=2*ɰ[ǉ.B]*ß;if(Ƹ<ƹ){ǉ.ů
(Ĺ);ǉ.ʪ+=ƹ;return;}Ƹ-=ƹ;int Ʒ=(int)Math.Floor(Ƹ/ɰ[ǉ.B]);float ƶ=Ʒ*ɰ[ǉ.B];Ƽ.ų().ʢ(' ',Ʒ).ʢ(Ĺ);ǉ.ů(Ƽ);ǉ.ʪ+=ƶ+ƹ;}public void
Ƶ(ʦ Ů){ǈ(Ů);ǉ.ŭ();}public void Ƶ(string Ĺ){ǈ(Ĺ);ǉ.ŭ();}public void ǈ(ʦ Ů){float ƹ=ǳ(Ů,ǉ.B);float Ǉ=ʁ/2*ǉ.ѽ-ǉ.ʪ;if(Ǉ<ƹ/2){
ǉ.ů(Ů);ǉ.ʪ+=ƹ;return;}Ǉ-=ƹ/2;int Ʒ=(int)Math.Round(Ǉ/ɰ[ǉ.B],MidpointRounding.AwayFromZero);float ƶ=Ʒ*ɰ[ǉ.B];Ƽ.ų().ʢ(' ',Ʒ
).ʢ(Ů);ǉ.ů(Ƽ);ǉ.ʪ+=ƶ+ƹ;}public void ǈ(string Ĺ){float ƹ=ǳ(Ĺ,ǉ.B);float Ǉ=ʁ/2*ǉ.ѽ-ǉ.ʪ;if(Ǉ<ƹ/2){ǉ.ů(Ĺ);ǉ.ʪ+=ƹ;return;}Ǉ-=ƹ
/2;int Ʒ=(int)Math.Round(Ǉ/ɰ[ǉ.B],MidpointRounding.AwayFromZero);float ƶ=Ʒ*ɰ[ǉ.B];Ƽ.ų().ʢ(' ',Ʒ).ʢ(Ĺ);ǉ.ů(Ƽ);ǉ.ʪ+=ƶ+ƹ;}
public void ǆ(double ǀ,float ƿ=1.0f,float ƴ=0f,bool ǅ=true){if(ß>0)ƴ+=2*ß*ɰ[ǉ.B];float Ǆ=ʁ*ƿ*ǉ.ѽ-ǉ.ʪ-ƴ;if(Double.IsNaN(ǀ))ǀ=0;
int ǃ=(int)(Ǆ/ȝ[ǉ.B])-2;if(ǃ<=0)ǃ=2;int ǂ=Math.Min((int)(ǀ*ǃ)/100,ǃ);if(ǂ<0)ǂ=0;if(ǉ.ʪ<=0)ǉ.ů(Þ);Ƽ.ų().ʢ(ǘ[ǉ.B]).ʢ(Ǖ[ǉ.B],ǂ
).ʢ(ǖ[ǉ.B],ǃ-ǂ).ʢ(Ǘ[ǉ.B]);ǉ.ů(Ƽ);if(ǅ)ǉ.ʪ+=ȝ[ǉ.B]*ǃ+2*ǚ[ǉ.B];}public void ǁ(double ǀ,float ƿ=1.0f,float ƴ=0f){ǆ(ǀ,ƿ,ƴ,
false);ǉ.ŭ();}public void Ŧ(){ǉ.Ŧ();}public void ƾ(å Y,ž E=null){Y.F(E);if(Y.Ü)Y.ã.Ļ();}public void Ȏ(string Ȍ,string Ĺ){var
Y=ǔ.GridTerminalSystem.GetBlockWithName(Ȍ)as IMyTextPanel;if(Y==null)return;Y.WriteText(Ĺ+"\n",true);}public string ȋ(
MyInventoryItem ª){string Ȋ=ª.Type.TypeId.ToString();Ȋ=Ȋ.Substring(Ȋ.LastIndexOf('_')+1);return ª.Type.SubtypeId+" "+Ȋ;}public void ȉ(
string Í,out string Ë,out string Ê){int Ł=Í.LastIndexOf(' ');if(Ł>=0){Ë=Í.Substring(0,Ł);Ê=Í.Substring(Ł+1);return;}Ë=Í;Ê="";}
public string Ȉ(string Í){string Ë,Ê;ȉ(Í,out Ë,out Ê);return Ȉ(Ë,Ê);}public string Ȉ(string Ë,string Ê){Æ ª=ƭ.Ì(Ë,Ê);if(ª!=
null){if(ª.î.Length>0)return ª.î;return ª.Ö;}return ƭ.È(Ë);}public void ȇ(ref string Ë,ref string Ê){Æ ª;if(ƭ.Ô.TryGetValue(
Ë,out ª)){Ë=ª.Ö;Ê=ª.Ø;return;}ª=ƭ.Ì(Ë,Ê);if(ª!=null){Ë=ª.Ö;if(string.IsNullOrEmpty(Ê)&&(string.Compare(ª.Ø,"Ore",true)==0
)||(string.Compare(ª.Ø,"Ingot",true)==0))return;Ê=ª.Ø;}}public string ȍ(double ȏ,bool ț=true,char Ț=' '){if(!ț)return ȏ.
ToString("#,###,###,###,###,###,###,###,###,###");string ș=" kMGTPEZY";double Ș=ȏ;int ȗ=ș.IndexOf(Ț);var Ȗ=(ȗ<0?0:ȗ);while(Ș>=
1000&&Ȗ+1<ș.Length){Ș/=1000;Ȗ++;}Ƽ.ų().ʢ(Math.Round(Ș,1,MidpointRounding.AwayFromZero));if(Ȗ>0)Ƽ.ʢ(" ").ʢ(ș[Ȗ]);return Ƽ.ɛ()
;}public string Ȝ(double ȏ,bool ț=true,char Ț=' '){if(!ț)return ȏ.ToString("#,###,###,###,###,###,###,###,###,###");
string ș=" ktkMGTPEZY";double Ș=ȏ;int ȗ=ș.IndexOf(Ț);var Ȗ=(ȗ<0?0:ȗ);while(Ș>=1000&&Ȗ+1<ș.Length){Ș/=1000;Ȗ++;}Ƽ.ų().ʢ(Math.
Round(Ș,1,MidpointRounding.AwayFromZero));if(Ȗ==1)Ƽ.ʢ(" kg");else if(Ȗ==2)Ƽ.ʢ(" t");else if(Ȗ>2)Ƽ.ʢ(" ").ʢ(ș[Ȗ]).ʢ("t");
return Ƽ.ɛ();}public string ȕ(double ǀ){return(Math.Floor(ǀ*10)/10).ToString("F1");}Dictionary<char,float>Ȕ=new Dictionary<
char,float>();void ȓ(string Ȓ,float C){C+=1;for(int A=0;A<Ȓ.Length;A++){if(C>ɴ[ɷ])ɴ[ɷ]=C;Ȕ.Add(Ȓ[A],C);}}public float ȑ(char
Ȑ,string B){float Ǆ;if(B==ɶ||!Ȕ.TryGetValue(Ȑ,out Ǆ))return ɴ[B];return Ǆ;}public float ǳ(ʦ Ȇ,string B){if(B==ɶ)return Ȇ.
ʤ*ɴ[B];float ǣ=0;for(int A=0;A<Ȇ.ʤ;A++)ǣ+=ȑ(Ȇ[A],B);return ǣ;}public float ǳ(string ĭ,string B){if(B==ɶ)return ĭ.Length*ɴ
[B];float ǣ=0;for(int A=0;A<ĭ.Length;A++)ǣ+=ȑ(ĭ[A],B);return ǣ;}public string ǲ(string Ĺ,float Ǳ){if(Ǳ/ɴ[ǉ.B]>=Ĺ.Length)
return Ĺ;float ǰ=ǳ(Ĺ,ǉ.B);if(ǰ<=Ǳ)return Ĺ;float ǯ=ǰ/Ĺ.Length;Ǳ-=ɳ[ǉ.B];int Ǯ=(int)Math.Max(Ǳ/ǯ,1);if(Ǯ<Ĺ.Length/2){Ƽ.ų().ʢ(Ĺ,
0,Ǯ);ǰ=ǳ(Ƽ,ǉ.B);}else{Ƽ.ų().ʢ(Ĺ);Ǯ=Ĺ.Length;}while(ǰ>Ǳ&&Ǯ>1){Ǯ--;ǰ-=ȑ(Ĺ[Ǯ],ǉ.B);}if(Ƽ.ʤ>Ǯ)Ƽ.ʡ(Ǯ,Ƽ.ʤ-Ǯ);return Ƽ.ʢ("..").ɛ
();}void ǭ(string Ǭ){ɷ=Ǭ;ǘ[ɷ]=MMStyle.BAR_START;Ǘ[ɷ]=MMStyle.BAR_END;ǖ[ɷ]=MMStyle.BAR_EMPTY;Ǖ[ɷ]=MMStyle.BAR_FILL;ɴ[ɷ]=0f
;}void ǫ(string Ǫ,float ǩ){ɶ=Ǫ;ɵ=ǩ;ɴ[ɶ]=ɵ+1;ɳ[ɶ]=2*(ɵ+1);ǘ[ɶ]=MMStyle.BAR_MONO_START;Ǘ[ɶ]=MMStyle.BAR_MONO_END;ǖ[ɶ]=
MMStyle.BAR_MONO_EMPTY;Ǖ[ɶ]=MMStyle.BAR_MONO_FILL;ɰ[ɶ]=ȑ(' ',ɶ);ȝ[ɶ]=ȑ(ǖ[ɶ],ɶ);ǚ[ɶ]=ȑ(ǘ[ɶ],ɶ);ɲ[ɶ]=ǳ(" 100.0%",ɶ);}public void
Ǩ(){if(Ȕ.Count>0)return;
// Monospace font name, width of single character
// Change this if you want to use different (modded) monospace font
ǫ("Monospace", 24f);

// Classic/Debug font name (uses widths of characters below)
// Change this if you want to use different font name (non-monospace)
ǭ("Debug");
// Font characters width (font "aw" values here)
ȓ("3FKTabdeghknopqsuy£µÝàáâãäåèéêëðñòóôõöøùúûüýþÿāăąďđēĕėęěĝğġģĥħĶķńņňŉōŏőśŝşšŢŤŦũūŭůűųŶŷŸșȚЎЗКЛбдекруцяёђћўџ", 17f);
ȓ("ABDNOQRSÀÁÂÃÄÅÐÑÒÓÔÕÖØĂĄĎĐŃŅŇŌŎŐŔŖŘŚŜŞŠȘЅЊЖф□", 21f);
ȓ("#0245689CXZ¤¥ÇßĆĈĊČŹŻŽƒЁЌАБВДИЙПРСТУХЬ€", 19f);
ȓ("￥$&GHPUVY§ÙÚÛÜÞĀĜĞĠĢĤĦŨŪŬŮŰŲОФЦЪЯжы†‡", 20f);
ȓ("！ !I`ijl ¡¨¯´¸ÌÍÎÏìíîïĨĩĪīĮįİıĵĺļľłˆˇ˘˙˚˛˜˝ІЇії‹›∙", 8f);
ȓ("？7?Jcz¢¿çćĉċčĴźżžЃЈЧавийнопсъьѓѕќ", 16f);
ȓ("（）：《》，。、；【】(),.1:;[]ft{}·ţťŧț", 9f);
ȓ("+<=>E^~¬±¶ÈÉÊË×÷ĒĔĖĘĚЄЏЕНЭ−", 18f);
ȓ("L_vx«»ĹĻĽĿŁГгзлхчҐ–•", 15f);
ȓ("\"-rª­ºŀŕŗř", 10f);
ȓ("WÆŒŴ—…‰", 31f);
ȓ("'|¦ˉ‘’‚", 6f);
ȓ("@©®мшњ", 25f);
ȓ("mw¼ŵЮщ", 27f);
ȓ("/ĳтэє", 14f);
ȓ("\\°“”„", 12f);
ȓ("*²³¹", 11f);
ȓ("¾æœЉ", 28f);
ȓ("%ĲЫ", 24f);
ȓ("MМШ", 26f);
ȓ("½Щ", 29f);
ȓ("ю", 23f);
ȓ("ј", 7f);
ȓ("љ", 22f);
ȓ("ґ", 13f);
ȓ("™", 30f);
// End of font characters width
        ɰ[ɷ]=ȑ(' ',ɷ);ȝ[ɷ]=ȑ(ǖ[ɷ],ɷ);ǚ[ɷ]=ȑ(ǘ[ɷ],ɷ);ɲ[ɷ]=ǳ(" 100.0%",ɷ);ɳ[ɷ]=ȑ('.',ɷ)*2;}}class ǧ{public string Ǧ(string
ǥ){return TT[ǥ];}
readonly Dictionary<string, string> TT = new Dictionary<string, string>
{
// TRANSLATION STRINGS
// msg id, text
{ "AC1", "Acceleration:" },
// amount
{ "A1", "EMPTY" },
{ "ALT1", "Altitude:"},
{ "ALT2", "Ground:"},
{ "B1", "Booting up..." },
{ "C1", "count:" },
{ "C2", "Cargo Used:" },
{ "C3", "Invalid countdown format, use:" },
{ "C4", "EXPIRED" },
{ "C5", "days" },
// customdata
{ "CD1", "Block not found: " },
{ "CD2", "Missing block name" },
{ "D1", "You need to enter name." },
{ "D2", "No blocks found." },
{ "D3", "No damaged blocks found." },
{ "DO1", "No connectors found." },
{ "DTU", "Invalid GPS format" },
{ "GA", "Artif."}, // (not more than 5 characters)
{ "GN", "Natur."}, // (not more than 5 characters)
{ "GT", "Total"}, // (not more than 5 characters)
{ "G1", "Total Gravity:"},
{ "G2", "Natur. Gravity:"},
{ "G3", "Artif. Gravity:"},
{ "GNC", "No cockpit!"},
{ "H1", "Write commands to Custom Data of this panel." },
// inventory
{ "I1", "ore" },
{ "I2", "summary" },
{ "I3", "Ores" },
{ "I4", "Ingots" },
{ "I5", "Components" },
{ "I6", "Gas" },
{ "I7", "Ammo" },
{ "I8", "Tools" },
{ "I9", "Objects" }, // NEW
{ "I10", "Consumables" }, // NEW
{ "I11", "Seeds" }, // NEW
{ "M1", "Cargo Mass:" },
// oxygen
{ "O1", "Leaking" },
{ "O2", "Oxygen Farms" },
{ "O3", "No oxygen blocks found." },
{ "O4", "Oxygen Tanks" },
// position
{ "P1", "Block not found" },
{ "P2", "Location" },
// power
{ "P3", "Stored" },
{ "P4", "Output" },
{ "P5", "Input" },
{ "P6", "No power source found!" },
{ "P7", "Batteries" },
{ "P8", "Total Output" },
{ "P9", "Reactors" },
{ "P10", "Solars" },
{ "P11", "Power" },
{ "P12", "Engines" },
{ "P13", "Turbines" },
{ "PT1", "Power Time:" },
{ "PT2", "Charge Time:" },
{ "PU1", "Power Used:" },
{ "S1", "Speed:" },
{ "SM1", "Ship Mass:" },
{ "SM2", "Ship Base Mass:" },
{ "SD", "Stop Distance:" },
{ "ST", "Stop Time:" },
// text
{ "T1", "Source LCD not found: " },
{ "T2", "Missing source LCD name" },
// tanks
{ "T4", "Missing tank type. eg: 'Tanks * Hydrogen'" },
{ "T5", "No {0} tanks found." }, // {0} is tank type
{ "UC", "Unknown command" },
// occupied & dampeners
{ "SC1", "Cannot find control block." },
{ "SCD", "Dampeners: " },
{ "SCO", "Occupied: " },
// working
{ "W1", "OFF" },
{ "W2", "WORK" },
{ "W3", "IDLE" },
{ "W4", "LEAK" },
{ "W5", "OPEN" },
{ "W6", "CLOSED" },
{ "W7", "LOCK" },
{ "W8", "UNLOCK" },
{ "W9", "ON" },
{ "W10", "READY" }
};
    }
}class Ǥ{public System.Text.RegularExpressions.Regex Ǵ;public Ǥ(string Ȅ){Ǵ=new System.Text.RegularExpressions.Regex(Ȅ);}
public string ȅ(string ȃ,string Ȃ){return Ǵ.Replace(ȃ,Ȃ);}}static class ȁ{public static bool Ȁ(this string ĭ,string ǽ){return
ĭ.StartsWith(ǽ,StringComparison.InvariantCultureIgnoreCase);}public static bool ǿ(this string ĭ,string ǽ){if(ĭ==null)
return false;return ĭ.IndexOf(ǽ,StringComparison.InvariantCultureIgnoreCase)>=0;}public static bool Ǿ(this string ĭ,string ǽ){
return ĭ.EndsWith(ǽ,StringComparison.InvariantCultureIgnoreCase);}}static class Ǽ{public static string ǻ(this IMyTerminalBlock
â){int ņ=â.CustomData.IndexOf("\n---\n");if(ņ<0){if(â.CustomData.StartsWith("---\n"))return â.CustomData.Substring(4);
return â.CustomData;}return â.CustomData.Substring(ņ+5);}public static string Ǻ(this IMyTerminalBlock â,int Ł,string ǹ){string
Ǹ=â.ǻ();string Ƿ="@"+Ł.ToString()+" AutoLCD";string Ƕ='\n'+Ƿ;int ņ=0;if(!Ǹ.StartsWith(Ƿ,StringComparison.
InvariantCultureIgnoreCase)){ņ=Ǹ.IndexOf(Ƕ,StringComparison.InvariantCultureIgnoreCase);}if(ņ<0){if(Ł==0){if(Ǹ.Length==0)return"";if(Ǹ[0]=='@')
return null;ņ=Ǹ.IndexOf("\n@");if(ņ<0)return Ǹ;return Ǹ.Substring(0,ņ);}else return null;}int ǵ=Ǹ.IndexOf("\n@",ņ+1);if(ǵ<0){
if(ņ==0)return Ǹ;return Ǹ.Substring(ņ+1);}if(ņ==0)return Ǹ.Substring(0,ǵ);return Ǹ.Substring(ņ+1,ǵ-ņ);}