/*
 *        H2/O2 Control Program Script
 *        version: 1.08 - updated 20. Jan 2021
 *        by AlfRomeo80
 * 
 *        deployed with MDK-SE - Malware's Development Kit for Space Engineers
 *        https://github.com/malware-dev/MDK-SE
 */

/* ---------------- LIST OF ALL COMMANDS ----------------
         *
         * !!! add " ~ BlocksCustomName" after the command !!!
         * to select the screen you want to interact with
         *
         * --- commands to use the control screen ---
         * menu-/menu+ => show previous/next control screen
         * menu=# => show control screen with number # (0 - 7)
         * select-/select+ => move cursor to the previous/next item
         * select=1 => select or deselect the item at current cursor position
         * switch=0 => activate or deactivate current item or all selected items
         * switch=1 => change Primary value of current item or all selected items
         * switch=2 => change Secondary value of current item or all selected items
         * cs-/cs+ => decrease/increase conveyor system number
         *
         * --- commands to use the instructions screen ---
         * tab=# => show instructions for button bar number #
         * tab=# > H2O2 => combination of tab=# and control=H2O2
         * tab=# > Room => combination of tab=# and control=Room
         *
         * --- commands to switch control screen to another program ---
         * control=H2O2 => uses the Control Screen of the ~ block for H2O2 Program Control
         * control=Room => uses the Control Screen of the ~ block for Room Program Control
         *
         * --- general commands --- no link to a screen needed ---
         * resetErrors => deletes all error messages
         * uninstall => deletes all entries at the name and the custom data field of all blocks the script made
         * >>> ONLY USE UNINSTALL IF YOU NEVER WANT TO USE THE SCRIPT AGAIN IN THIS WORLD <<<
         *
         * ------------------- SET PARAMETERS -------------------
         *
         * You can decide whether you want to use a name token to recognize blocks of a grid, or whether to let the script analyze the grid.
         * scanConstruct = true => the script is analyzing the grid, or if "false" the names get analysed.
         * If scanConstruct = false => Enter a specific part of all blocks name, only blocks with this keyword in there names are controlled by the script.
         * e.g.:
         * static readonly bool scanConstruct = false;
         * const string gridName = "Base One";
         */
static readonly bool scanConstruct = true;
const string gridName = "";
/*
         * If not all your blocks are connected with one single conveyor system set
         * maxCS to the number of systems you are going to use
         * e.g.: static int maxCS = 3;
         */
const int maxCS = 2;
/*
         * Oxygen Farm Alignment
         * If you want to use the included alignment script enter the names of the oxygen farm masts below.
         * This keyword has to be part of all rotors and all farms, connected to the different pylons.
         * If you want to use other alignment scripts with other programmable blocks enter the name of that block down here.
         * alignFarms set the default value, if you want to use the alignment to be active when the script starts the first time.
         * e.g.: static string[] farmMastNames = { "front", "left", right" };
         *       static string extAlignProgBlockName = "Base FarmScript Programmable Block";
         */
static readonly string[] farmMastNames = { "" };
static readonly string extAlignProgBlockName = "";
/*
         * -------------- MINOR IMPORTANT SETTINGS --------------
         */
// Language: english: en = true, translation: en = false
static readonly bool en = true;
// default values
static bool alignFarms = false;
const bool defaultMoveIce = false;
// depth of the output diagram on the screens
const int outputDiagramDepth = 100;
// enable or disable fastBoot
static readonly bool fastBoot = true;
// screens cycle speed - switch screens every # runs
const int cycleSpeed = 2;
// colors used in the script
static readonly Color
    mainColor = new Color(130, 130, 130), // main text and draw color
    warningColor = new Color(200, 200, 0), // warning outputs
    colorOff = new Color(65, 0, 0), // block is offline
    colorOn = new Color(0, 65, 0), // block is online
    colorFull = new Color(130, 65, 0), // inventory is full or gas tank is empty
    colorSpace = new Color(0, 65, 0), // space left in inventory
    h2Color = new Color(90, 25, 25), // hydrogen amounts
    o2Color = new Color(25, 25, 90), // oxygen amounts
    iceColor = new Color(35, 35, 50), // ice amounts
    cargoColor = new Color(10, 10, 10), // non-ice cargo load
    farmColor = new Color(60, 180, 112), // oxygen farm activity
    leakColor = new Color(130, 65, 0), // unpressurizeable rooms
    depressColor = new Color(0, 100, 130), // depressurize a room
    backgroundColor = new Color(0, 0, 0), // screen background
    selectionColor = new Color(130, 130, 130), // selection square
    selectedColor = new Color(65, 65, 65); // selected background

// output of the instruction screen - nine lists for each tab with one headline and eight instructions for each button
// This is my setup suggestion, using the H2O2 Control Program and the Room Control Program at the same control seat.
// I strongly recommend using button 1 of the G menu to change the instruction screen using the command "tab=#".
static readonly string[,] instructions = new string[9, 9]
{
    // tab=1 > H2O2 ~ BlockName
    { "H2O2 Program Control", "show previous screen", "show next screen", "move cursor backwards", "move cursor forwards", "select/deselect", "activate/deactivate block", "change Primary", "change Secondary" },
    // tab=2 > H2O2 ~ BlockName
    { "Conveyor System Block Settings", "show previous screen", "show next screen", "move cursor backwards", "move cursor forwards", "select/deselect", "select/deselect all", "decrease CS", "increase CS" },
    // tab=3 > Room ~ BlockName
    { "Room Program Settings", "show main control", "", "move cursor backwards", "move cursor forwards", "", "activate/deactivate", "change Primary", "change Secondary" },
    // tab=4 > Room ~ BlockName
    { "Room Program Control", "show previous room", "show next room", "move cursor backwards", "move cursor forwards", "switch auto. room control", "activate/deactivate block", "change Primary", "change Secondary" },
    // tab=5 > Room ~ BlockName
    { "Conveyor System Vent Settings", "show previous room", "show next room", "move cursor backwards", "move cursor forwards", "", "", "decrease CS", "increase CS" },
    // tab=6 > H2O2 ~ BlockName
    { "not set up yet", "", "", "", "", "", "", "", "" },
    // tab=7 > H2O2 ~ BlockName
    { "not set up yet", "", "", "", "", "", "", "", "" },
    // tab=8 > H2O2 ~ BlockName
    { "H2O2 Program System Settings", "", "", "", "switch to Room Program", "", "", "", "delete all error codes" },
    // tab=9 > Room ~ BlockName
    { "Room Program System Settings", "", "", "", "switch to H2O2 Program", "", "", "", "delete all error codes" },
};
// List of all german translations. You can edit them, if you want to use your own translations - { "script output", "translated term" }
static readonly Dictionary<string, string> translations = new Dictionary<string, string>
{
    {" y", " J"}, {" w", " W"}, {" d", " T"},
    {"deactivate", "ausschalten"}, {"activate", "einschalten"}, {"Primary", "Primär"}, {"Secondary", "Sekundär"}, {"decrease limit by ", "verringere Limit um "}, {"increase limit by ", "erhöhe Limit um "},
    {"H2 level: ", "H2 Level: "}, {"provide gas", "Gasversorgung"}, {"bottle refill", "Flaschen füllen"}, {"stored", "lagernd"}, {"storage", "Lagerraum"},
    {"ice", "Eis"}, {"available ice storage", "Raum für Eis"}, {"ice cargo", "Eislager"}, {"item movement", "Artikel verschieben"}, {"disabled", "abgeschaltet"}, {"Override", "Festsetzen"},
    {"override", "festsetzen"}, {"thrust", "Schub"}, {"Throttle", "Schubhebel"}, {"Consumption", "Verbrauch"}, {"O2 output", "O2 Produktion"}, {"using external alignment", "nutze externe Ausrichtung"},
    {"farm mast alignment", "Farmmast Ausrichtung"}, {"damaged", "defekt"}, {"level oxygen tanks", "Sauerstofftanks ausgleichen"},
    {"Ice level: ", "Eis Level: "}, {"collecting ice", "Eis beziehen"}, {"no blocks\navailable", "keine Blöcke\nverfügbar"}, {"Symbol area calculation failed", "Berechnung Symbolbereich fehlgeschlagen"},
    {" Status", "-Status"}, {" Control", "-Kontrolle"}, {"H2/O2 Generators", "H2/O2 Generatoren"}, {"Hydrogen Tank", "Wasserstofftank"}, {"Oxygen Tank", "Sauerstofftank"}, {"Air Vents", "Lüftungs"},
    {"Ice Cargo", "Eis Lager"}, {"Oxygen Farm", "Sauerstofffarm"}, {"Hydrogen Thrusters", "Wasserstofftriebwerke"}, {"Hydrogen Engines", "Wasserstoffmotoren"}, {"Ice/H2/O2 Status", "Eis/H2/O2 Status"},
    {"ice is automatically relocated", "Eis wird verschoben" }, {"no automatic relocation of items", "keine Verschiebung von Artikeln"}, { "possible amount of gases:", "mögliche Menge an Gasen:" },
    {"H2/O2 Control Program", "H2/O2 Kontroll-Programm"}, {"H2/O2 Program Manual", "H2/O2 Programm Anleitung"}, {"H2/O2/Ice", "H2/O2/Eis"}, {"H2/O2 Control", "H2/O2 Steuerung"},
    {"ice movement", "Eis verschieben"}, {"farm alignment", "Farm Ausrichtung"}, {"change upper hydrogen limit for automatic control", "ändere oberes Wasserstoff-Limit für automatische Steuerung"},
    {"change lower hydrogen limit for automatic control", "ändere unteres Wasserstoff-Limit für automatische Steuerung"}, {"change upper oxygen limit for automatic control", "ändere oberes Sauerstoff-Limit für automatische Steuerung"},
    {"change lower oxygen limit for automatic control", "ändere unteres Sauerstoff-Limit für automatische Steuerung"}, {"H2/O2 Program", "H2/O2 Stuerung"},
    {"change automatic control of generators", "schalte automatische Generatorenkontrolle um"}, {"change automatic storage ice movement", "schalte automatische Eis-Lagerkontrolle um"},
    {"change automatic farm alignment", "schalte automatische Farm Ausrichtung um"}, {"change automatic airlock oxygen leveling", "schalte automatischen Sauerstofftank Ausgleich um"}
};
// ------------ DON'T CHANGE ANYTHING BELOW THIS LINE ------------------------- DON'T CHANGE ANYTHING BELOW THIS LINE ------------- //

public enum LayoutTypes { H2O2ControlScreen, GeneratorsScreen, H2TanksScreen, H2ThrustersScreen, H2EnginesScreen, IceCargoScreen, O2TanksScreen, O2FarmsScreen, AirVentsScreen, OverallH2O2IceScreen, H2O2InstructionsScreen, H2O2ProgramScreen, NONE }
public enum ScreenSetupTags { Sensor, Cockpit, DrawChart };
public enum BlockSetupTags { Name, Conveyor, ItemMove, IceCargo, Room };
static readonly string[] roomScreens = { "RoomControlScreen", "RoomScreen", "AirlockScreen", "RoomInstructionsScreen", "RoomProgramScreen" };
public enum ControlProgram { noControl, H2O2, Room };
static List<ϧ>ǂ=new List<ϧ>();static List<Ж>ǃ=new List<Ж>();static List<Ј>Έ=new List<Ј>();static List<ŋ>Ά=new List<ŋ>();
static List<М>ͽ=new List<М>();static List<ϳ>ͼ=new List<ϳ>();static List<ϯ>ͻ=new List<ϯ>();static List<ĭ>ͺ=new List<ĭ>();static
List<ĵ>ͷ=new List<ĵ>();static List<Ɨ>Ͷ=new List<Ɨ>();static List<IMySensorBlock>ʹ=new List<IMySensorBlock>();static List<
IMyCockpit>ͳ=new List<IMyCockpit>();static List<ƕ>Ͳ=new List<ƕ>();static ʵ ͱ=new ʵ();static bool Ή=false;static float[]Ͱ=new float
[maxCS],Ί=new float[maxCS],Λ=new float[maxCS],Ι=new float[maxCS],Θ=new float[maxCS],Η=new float[maxCS],Ζ=new float[maxCS]
;static float Ε,Δ,Ń,Œ,Γ,Β,Κ,Ĩ,ħ,Α,ΐ,Ł,ũ,Ώ,Ύ,Ό,Ħ,ĳ,ņ,ń,ī,Ī,ĩ;const float ρ=0.37f;static bool π=false;static List<ȃ>ο=new
List<ȃ>(farmMastNames.Length);static bool ξ=false;static IMyProgrammableBlock ν=null;static bool μ=false;static float λ=0.3f
,κ=0.6f;static float ι=0.2f,ς=0.8f;static bool[]θ=new bool[maxCS];static bool[]ζ=new bool[maxCS];static bool ε=
defaultMoveIce;static List<щ>[]δ=new List<щ>[maxCS];static List<щ>[]γ=new List<щ>[maxCS];static List<щ>[]β=new List<щ>[maxCS];static
List<щ>[]α=new List<щ>[maxCS];const string ΰ="@H2O2Control";const string ί="[SETUP NEEDED]";const string ή="[screens]";const
string έ="[/screens]";const string η="[setup]";const string σ="[/setup]";const string ϋ="[readmeControlProgram]";const string
ϕ="[/readmeControlProgram]";static bool ϔ=true;static System.Globalization.CultureInfo ϓ=System.Globalization.CultureInfo
.GetCultureInfo("en-US");static int ϒ;static int ϑ;static bool V;static int ϐ=int.MaxValue;static int Ϗ=int.MaxValue;
static bool ώ=false;static int ύ=0;static int ό=1;static bool Ϋ=true;static string ǟ="";static Func<float,string>ϊ=(ω)=>{if(
Math.Abs(ω)<1000)return Math.Round(ω,0).ToString("#,0.#",ϓ)+" kg";else return Math.Round(ω/1000,1).ToString("#,0.0",ϓ)+" t";
};static Func<float,string>ψ=ʏ=>{if(Math.Abs(ʏ)<1000)return ʏ.ToString("#,0.",ϓ)+" l";else return(ʏ/1000).ToString("#,0."
,ϓ)+" m³";};static Func<float,string>χ=ȏ=>{if(ȏ>=(365*24*60))return Math.Round(ȏ/365/24/60,1).ToString("#,0.0",ϓ)+ʦ(" y")
;if(ȏ>=(7*24*60))return Math.Round(ȏ/7/24/60,1).ToString("#,0.0",ϓ)+ʦ(" w");if(ȏ>=(24*60))return Math.Round(ȏ/24/60,1).
ToString("#,0.0",ϓ)+ʦ(" d");if(ȏ>=60)return Math.Round(ȏ/60,1).ToString("#,0.0",ϓ)+" h";if(ȏ>1)return Math.Round(ȏ,1).ToString(
"#,0.0",ϓ)+" min";if(ȏ>0)return"< 1 min";return"-";};static void φ(float[]υ)=>Array.Clear(υ,0,maxCS);static void φ(bool[]υ)=>
Array.Clear(υ,0,maxCS);bool τ<Ɵ>(Ɵ ű)where Ɵ:IMyTerminalBlock{if(scanConstruct)return ű.IsSameConstructAs(Me);else return ű.
CustomName.Contains(gridName);}static int[]Φ(float[]Ž,int[]ɖ,int Ρ,Queue<float[]>Υ){if(Υ.Count>=outputDiagramDepth)Υ.Dequeue();Υ.
Enqueue(Ž.ToArray());int[]Ξ=new int[maxCS+1];for(int ŕ=0;ŕ<maxCS;ŕ++){Ξ[ŕ]=(int)Math.Ceiling(Math.Abs(Ž[ŕ])/Ρ)*Ρ;if(Ξ[ŕ]>ɖ[ŕ])ɖ
[ŕ]=Ξ[ŕ];}Ξ[maxCS]=(int)Math.Ceiling(Math.Abs(Ž.Sum())/Ρ)*Ρ;if(Ξ[maxCS]>ɖ[maxCS])ɖ[maxCS]=Ξ[maxCS];return ɖ;}static int[]
Τ(float[]Ž,float[]Σ,int[]ɖ,int Ρ,Queue<float[]>Π,Queue<float[]>Ο){if(Π.Count>=outputDiagramDepth)Π.Dequeue();if(Ο.Count>=
outputDiagramDepth)Ο.Dequeue();Π.Enqueue(Ž.ToArray());Ο.Enqueue(Σ.ToArray());int[]Ξ=new int[maxCS+1];for(int ŕ=0;ŕ<maxCS;ŕ++){Ξ[ŕ]=(int)
Math.Ceiling(Math.Abs(Σ[ŕ])/Ρ)*Ρ;if(Ξ[ŕ]>ɖ[ŕ])ɖ[ŕ]=Ξ[ŕ];}Ξ[maxCS]=(int)Math.Ceiling(Math.Abs(Σ.Sum())/Ρ)*Ρ;if(Ξ[maxCS]>ɖ[
maxCS])ɖ[maxCS]=Ξ[maxCS];return ɖ;}static bool Ν(bool Μ){if(!Μ){Ά.ForEach(ʿ=>ʿ.Ĳ());ǃ.ForEach(ˍ=>ˍ.Ĳ());ǂ.ForEach(ˊ=>ˊ.Ĳ());ͽ
.ForEach(ˑ=>ˑ.д=true);φ(ζ);φ(θ);}return μ=Μ;}Program(){if(maxCS>1)ϔ=false;if(!en)ϓ=System.Globalization.CultureInfo.
GetCultureInfo("de-DE");Runtime.UpdateFrequency=UpdateFrequency.Update100;V=true;ϒ=0;ϑ=14;}void Save(){foreach(var ʓ in ο)ʓ.Ǹ();if(Ϋ)
Storage=ά(true);}string ά(bool Ϋ=false){if(Ϋ){ǟ=alignFarms+"\n"+μ+"\n"+λ+"\n"+κ+"\n"+ι+"\n"+ς+"\n"+ε+"\n"+ȃ.ǡ(ο);}else{string[]
Ϊ=Storage.Split('\n');if(Ϊ.Length>=8){try{alignFarms=bool.Parse(Ϊ[0]);μ=bool.Parse(Ϊ[1]);λ=float.Parse(Ϊ[2]);κ=float.
Parse(Ϊ[3]);ι=float.Parse(Ϊ[4]);ς=float.Parse(Ϊ[5]);ε=bool.Parse(Ϊ[6]);ȃ.ǳ(Ϊ[7],ο);}catch{ͱ.с(
"Loading failed\ndefault values used");return"";}}}return ǟ;}void Main(string Ω,UpdateType Ψ){ͱ.Ľ();if(ό>cycleSpeed)ό=1;if(Ω.Any())ʂ(Ω);if(ϑ<14){ʣ();Echo(ͱ.ж
());Ͳ.ForEach(Ǒ=>Ǒ.ƫ(V));if(ϑ==13){Ͳ.ForEach(Ǒ=>Ǒ.Ʀ(false));Ͳ.Clear();Me.Enabled=false;}return;}if(ϒ>=Ϗ-1){ˉ();if(ε)ʾ();
if(μ)ˮ();if(Ή)ƻ.Ǆ(ǃ,ǂ);Ή=false;ˋ();ː();}if(ϒ<=Ϗ)Χ();if(ϒ>ϐ){ʘ();ʈ();Ͳ.ForEach(Ǒ=>Ǒ.ƫ(V));}Echo(ͱ.ж());if(ό>=cycleSpeed)ύ++
;ό++;}void Χ(){switch(ϒ){case 0:ͱ.ʫ();ͱ.ќ("booting...\n");ͱ.ќ("initialization...done\n");ͱ.ʪ(
"loading all sensors/cockpits");break;case 1:GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(ʹ,U=>τ(U));GridTerminalSystem.GetBlocksOfType<
IMyCockpit>(ͳ,Ä=>τ(Ä));ͱ.ʨ();ͱ.ʪ("loading all screens");break;case 2:ʘ();if(fastBoot)Runtime.UpdateFrequency=UpdateFrequency.
Update10;Ͳ.ForEach(Ǒ=>Ǒ.ƫ(V));ϐ=ϒ;ͱ.ʨ();ͱ.ʪ("loading all generators");break;case 3:Ά.Clear();List<IMyGasGenerator>ʢ=new List<
IMyGasGenerator>();GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(ʢ,ɭ=>τ(ɭ));ʢ.ForEach(ɭ=>Ά.Add(new ŋ(ɭ)));Ά=Ά.OrderBy(ʿ=>ʿ.Ы).
ToList();ɢ.ƺ=true;ͱ.ʨ();ͱ.ʪ("loading all gas tanks");break;case 4:ǃ.Clear();ǂ.Clear();List<IMyGasTank>ʡ=new List<IMyGasTank>()
;GridTerminalSystem.GetBlocksOfType<IMyGasTank>(ʡ,ȏ=>τ(ȏ));ʡ.ForEach(ȏ=>{if(ȏ.BlockDefinition.SubtypeId.Contains("Hydro")
)ǂ.Add(new ϧ(ȏ));else ǃ.Add(new Ж(ȏ));});ǂ=ǂ.OrderBy(ȏ=>ȏ.Ы).ToList();ɜ.ƺ=true;ǃ=ǃ.OrderBy(ȏ=>ȏ.Ы).ToList();ɡ.ƺ=true;ͱ.ʨ(
);ͱ.ʪ("loading all hydrogen thrusters");break;case 5:ͻ.Clear();List<IMyThrust>ʠ=new List<IMyThrust>();GridTerminalSystem.
GetBlocksOfType<IMyThrust>(ʠ,ȏ=>τ(ȏ)&&ȏ.BlockDefinition.SubtypeId.Contains("Hydrogen"));ʠ.ForEach(ȏ=>ͻ.Add(new ϯ(ȏ)));ͻ=ͻ.OrderBy(ȏ=>ȏ.
Ы).ToList();ɞ.ƺ=true;ͱ.ʨ();ͱ.ʪ("loading all hydrogen engines");break;case 6:ͼ.Clear();List<IMyPowerProducer>ʅ=new List<
IMyPowerProducer>();GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(ʅ,ʑ=>τ(ʑ)&&ʑ.BlockDefinition.SubtypeId.Contains("Hydrogen"));ʅ.
ForEach(ʑ=>ͼ.Add(new ϳ(ʑ)));ͼ=ͼ.OrderBy(ʑ=>ʑ.Ы).ToList();ǐ.ƺ=true;ͱ.ʨ();ͱ.ʪ("loading all air vents");break;case 7:Έ.Clear();
List<IMyAirVent>ʐ=new List<IMyAirVent>();GridTerminalSystem.GetBlocksOfType<IMyAirVent>(ʐ,ʏ=>τ(ʏ));ʐ.ForEach(ʏ=>Έ.Add(new Ј(
ʏ)));Έ=Έ.OrderBy(ʏ=>ʏ.Ы).ToList();Ǐ.ƺ=true;ͱ.ʨ();ͱ.ʪ("loading all cargo containers");break;case 8:ͺ.Clear();List<
IMyCargoContainer>ʎ=new List<IMyCargoContainer>();GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(ʎ,Ä=>τ(Ä));ʎ.ForEach(Ä=>ͺ.Add(new
ĭ(Ä)));ͺ=ͺ.OrderBy(Ä=>Ä.Ы).ToList();ͱ.ʨ();ͱ.ʪ("loading all connectors/collectors");break;case 9:ͷ.Clear();List<
IMyShipConnector>ʍ=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(ʍ,Ä=>τ(Ä));ʍ.ForEach(Ä=>ͷ.Add(new ĵ
(Ä)));ͷ=ͷ.OrderBy(Ä=>Ä.Ы).ToList();Ͷ.Clear();List<IMyCollector>ʌ=new List<IMyCollector>();GridTerminalSystem.
GetBlocksOfType<IMyCollector>(ʌ,Ä=>τ(Ä));ʌ.ForEach(Ä=>Ͷ.Add(new Ɨ(Ä)));Ͷ=Ͷ.OrderBy(Ä=>Ä.Ы).ToList();ɠ.ƺ=true;ͱ.ʨ();ͱ.ʪ(
"loading all gas farms");break;case 10:ο.Clear();ͽ.Clear();List<IMyOxygenFarm>ʋ=new List<IMyOxygenFarm>();GridTerminalSystem.GetBlocksOfType<
IMyOxygenFarm>(ʋ,ǹ=>τ(ǹ));ͱ.ʨ();ͱ.ʪ("loading all alignment rotors");List<IMyMotorStator>ʒ=new List<IMyMotorStator>();
GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(ʒ,Ƿ=>τ(Ƿ));foreach(var ʓ in farmMastNames){List<IMyOxygenFarm>ʞ=ʋ.Where(ǹ=>ǹ.CustomName
.Contains(ʓ)).ToList();List<IMyMotorStator>ʝ=ʒ.Where(ǹ=>ǹ.CustomName.Contains(ʓ)).ToList();if(ʞ.Any()&&ʒ.Any()){ȃ ʜ=new ȃ
(ʓ,ʞ,ʝ);ʞ.ForEach(ǹ=>ͽ.Add(new М(ǹ,ʜ)));ο.Add(ʜ);ʞ.ForEach(ʛ=>ʋ.Remove(ʛ));}}ʋ.ForEach(ǹ=>ͽ.Add(new М(ǹ)));ͽ=ͽ.OrderBy(ǹ
=>ǹ.Ы).ToList();if(ο.Count>0)ͱ.ʨ();else ͱ.ʨ("no masts found");ɝ.ƺ=true;ͱ.ʪ("loading external alignment script");ν=null;if(
extAlignProgBlockName.Length>1)ν=GridTerminalSystem.GetBlockWithName(extAlignProgBlockName)as IMyProgrammableBlock;ξ=ν!=null;if(ξ)ͱ.ʨ();else
ͱ.ʨ("no script found");ͱ.ʪ("load saved data");break;case 11:ά();ͱ.ʨ();break;case 12:ͱ.ќ("booting...done");Runtime.
UpdateFrequency=UpdateFrequency.Update100;V=false;break;case 13:ƻ.ƺ=ǜ.ƺ=Ǔ.ƺ=ǚ.ƺ=true;ͱ.ʫ();ώ=false;Ϗ=ϒ;break;}ϒ++;}void ʚ<Ɵ>(List<Ɵ>ɫ)
where Ɵ:ь{List<Ɵ>ʙ=new List<Ɵ>();ɫ.ForEach(ű=>{if(GridTerminalSystem.GetBlockWithId(ű.Ƙ)!=null)try{ű.я();}catch{ͱ.с(
"Conveyor system number error\nCS: "+(ű.Ƣ+1)+" outside the limits.\n"+ű.Ы+"\nnot used by the script.");ʙ.Add(ű);}else ʙ.Add(ű);});foreach(var ű in ʙ)ɫ.
Remove(ű);}void ʘ(){List<IMyTerminalBlock>ʗ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<
IMyTextSurfaceProvider>(ʗ,ɭ=>τ(ɭ)&&ɭ.CustomData.Contains(ΰ));List<int>ʖ=new List<int>();for(int w=0;w<Ͳ.Count;w++)if(!ʗ.Any(ʔ=>Ͳ[w].Ƙ==ʔ.
EntityId))ʖ.Add(w);ʖ.ForEach(w=>{try{Ͳ[w].Ʀ(true);}catch{ͱ.с("missing a screen");}Ͳ.RemoveAt(w);});List<IMyTerminalBlock>ʕ=new
List<IMyTerminalBlock>();ʗ.ForEach(ʔ=>{if(!Ͳ.Any(Ǒ=>Ǒ.Ƙ==ʔ.EntityId))ʕ.Add(ʔ);});ʕ.ForEach(ʊ=>Ͳ.Add(new Ŵ(ʊ)));}void ʈ(){
List<Ƥ>ʄ;foreach(var Ǒ in Ͳ){ʄ=Ǒ.Ƨ();ʄ.ForEach(ʃ=>{if(ʃ!=null)switch((int)ʃ.ƣ){case 0:Ή|=true;break;case 1:ʃ.Ơ(Ά);break;case
2:ʃ.Ơ(ǂ);break;case 3:ʃ.Ơ(ͻ);break;case 4:ʃ.Ơ(ͼ);break;case 5:ʃ.Ơ(ͺ);break;case 6:ʃ.Ơ(ǃ);break;case 7:ʃ.Ơ(ͽ);break;case 8
:ʃ.Ơ(Έ);break;}});}}void ʂ(string ʁ){string ɷ=null;string[]ʀ=ʁ.Split('=');if(ʀ[0].Equals("resetBlocks",StringComparison.
OrdinalIgnoreCase)){ώ=true;return;}else if(ʀ[0].Equals("resetErrors",StringComparison.OrdinalIgnoreCase)){ͱ.ʬ();return;}else if(ʀ[0].
Equals("uninstall",StringComparison.OrdinalIgnoreCase)){ϑ=0;return;}try{string[]ɿ;ɿ=ʁ.Split('~');ɷ=ɿ[1].Trim();ʀ=ɿ[0].Trim().
Split('=');}catch{ͱ.с("command ignored\nuse \"... ~ block name\" to\nselect the control screen");return;}if(ʀ[0].StartsWith(
"tab",StringComparison.OrdinalIgnoreCase)){try{string[]ɾ=ʀ[1].Trim().Split('>');switch(ɾ.Length){case 1:ʇ(int.Parse(ɾ[0]),1,ɷ
);break;case 2:int ɽ=(int)Enum.Parse(typeof(ControlProgram),ɾ[1]);ʇ(int.Parse(ɾ[0]),ɽ,ɷ);break;default:throw new System.
FormatException();}}catch{ͱ.с("unknown tab command");}}else if(ʀ[0].StartsWith("menu",StringComparison.OrdinalIgnoreCase)){try{if(ʀ[0].
EndsWith("+"))ɻ(1,-1,ɷ);else if(ʀ[0].EndsWith("-"))ɻ(-1,-1,ɷ);else if(ʀ[1].Length>0)ɻ(0,int.Parse(ʀ[1]),ɷ);}catch{ͱ.с(
"unknown menu command");ɻ(0,0,ɷ);}}else if(ʀ[0].StartsWith("select",StringComparison.OrdinalIgnoreCase)){try{if(ʀ[0].EndsWith("+"))ɹ(1,0,ɷ);
else if(ʀ[0].EndsWith("-"))ɹ(-1,0,ɷ);else if(ʀ[1].Length>0)ɹ(0,int.Parse(ʀ[1]),ɷ);}catch{ͱ.с("unknown select command");ɹ(0,0
,ɷ);}}else if(ʀ[0].StartsWith("switch",StringComparison.OrdinalIgnoreCase)){int ɼ;try{ɼ=int.Parse(ʀ[1].Trim());}catch{ͱ.с
("unknown switch command");ɼ=-1;}if(ɼ==0)ż(ɷ);else if(ɼ==1)Ż(ɷ);else if(ɼ==2)ź(ɷ);}else if(ʀ[0].StartsWith("cs",
StringComparison.OrdinalIgnoreCase)){if(ʀ[0].EndsWith("+"))ʉ(1,ɷ);else if(ʀ[0].EndsWith("-"))ʉ(-1,ɷ);else ͱ.с("unknown cs command");}
else if(ʀ[0].StartsWith("control",StringComparison.OrdinalIgnoreCase)){if(ʀ[1].Equals("Room")||ʀ[1].Equals("H2O2"))ʆ((int)
Enum.Parse(typeof(ControlProgram),ʀ[1]),ɷ);else ͱ.с("unknown control command");}}void ɻ(int ɸ,int ɺ,string ɷ){foreach(var Ǒ
in Ͳ.Where(Ǒ=>Ǒ.Ɠ!=null&&Ǒ.Ɣ.Equals(ɷ))){if(ɺ<0)Ǒ.Ƭ+=ɸ;else Ǒ.Ƭ=ɺ;}}void ɹ(int ɸ,int ɥ,string ɷ){foreach(var Ǒ in Ͳ.Where(
Ǒ=>Ǒ.Ɠ!=null&&Ǒ.Ɣ.Equals(ɷ)))if(ɸ==ɥ){Ǒ.Ɠ.ƥ(0);Ǒ.Ɠ.ž(false);}else if(ɥ==0)Ǒ.Ɠ.ƥ(ɸ);else if(ɥ==1)Ǒ.Ɠ.Ɖ();else if(ɥ==-1)Ǒ.Ɠ
.ſ();}void ż(string ɷ=null){foreach(var Ǒ in Ͳ.Where(Ǒ=>Ǒ.Ɠ!=null&&Ǒ.Ɣ.Equals(ɷ)))Ǒ.Ɠ.ż();}void Ż(string ɷ=null){foreach(
var Ǒ in Ͳ.Where(Ǒ=>Ǒ.Ɠ!=null&&Ǒ.Ɣ.Equals(ɷ)))Ǒ.Ɠ.Ż();}void ź(string ɷ=null){foreach(var Ǒ in Ͳ.Where(Ǒ=>Ǒ.Ɠ!=null&&Ǒ.Ɣ.
Equals(ɷ)))Ǒ.Ɠ.ź();}void ʉ(int Ÿ,string ɷ=null){foreach(var Ǒ in Ͳ.Where(Ǒ=>Ǒ.Ɠ!=null&&Ǒ.Ɣ.Equals(ɷ)))Ǒ.Ɠ.Ź(Ÿ,maxCS);}void ʇ(
int Û,int Ü,string ɷ=null){foreach(var Ǒ in Ͳ.Where(Ǒ=>Ǒ.Ɣ.Equals(ɷ)))Ǒ.ƪ(Û,Ü);}void ʆ(int ɽ,string ɷ=null){foreach(var Ǒ
in Ͳ.Where(Ǒ=>Ǒ.Ɣ.Equals(ɷ)))Ǒ.ƨ(ɽ);}void ˉ(){float[]ˈ=new float[maxCS],ˇ=new float[maxCS];float ī=0f,ˆ=0f;bool ˁ=false,ˀ=
false;φ(Ͱ);φ(Ί);φ(Λ);φ(Ι);φ(Θ);φ(Η);φ(Ζ);for(int Ï=0;Ï<maxCS;Ï++){γ[Ï]=new List<щ>();δ[Ï]=new List<щ>();α[Ï]=new List<щ>();β[
Ï]=new List<щ>();}ĭ.Ľ();ʚ(ͺ);ĵ.Ľ();ʚ(ͷ);Ɨ.Ľ();ʚ(Ͷ);ϧ.Ľ();ʚ(ǂ);Ж.Ľ();ʚ(ǃ);ϳ.Ľ();ʚ(ͼ);ϯ.Ľ();ʚ(ͻ);Ј.Ľ();ʚ(Έ);М.Ľ();ʚ(ͽ);ŋ.Ľ(
);ʚ(Ά);for(int Ï=0;Ï<maxCS;Ï++){ˈ[Ï]+=ϧ.ˈ[Ï]+ϳ.ˈ[Ï]+ϯ.ˈ[Ï];ˇ[Ï]+=Ж.ˇ[Ï]+Ј.Ѐ[Ï];foreach(var ǹ in ͽ.Where(ǹ=>ǹ.Ƣ==Ï&&ǹ.ы))
if(ˇ[Ï]>0){М.Ł[Ï]+=Math.Min(ǹ.Ũ,ˇ[Ï]);ˇ[Ï]-=Math.Min(ǹ.Ũ,ˇ[Ï]);}foreach(var ʿ in Ά.Where(ʿ=>ʿ.Ƣ==Ï&&ʿ.ы)){if(ˇ[Ï]>0){ŋ.Ł[Ï
]+=ʿ.ś=Math.Min(ˇ[Ï],ʿ.Ũ);ŋ.Ŕ[Ï]+=ʿ.Š;ˇ[Ï]-=ʿ.ś;}if(ˈ[Ï]>0){ŋ.Ń[Ï]+=ʿ.Ŝ=Math.Min(ˈ[Ï],ʿ.Ş);ŋ.ł[Ï]+=ʿ.ş;ˈ[Ï]-=ʿ.Ŝ;}}ŋ.ņ[Ï]
-=(ŋ.Ł[Ï]/ŋ.Ŋ)+(ŋ.Ń[Ï]/ŋ.ŉ);ŋ.Ņ[Ï]-=(ŋ.Ŕ[Ï]/ŋ.Ŋ)+(ŋ.ł[Ï]/ŋ.ŉ);if(ŋ.Ň[Ï]){ĭ.ī[Ï]=ĭ.Ī[Ï];ĵ.ī[Ï]=ĵ.Ī[Ï];Ɨ.ī[Ï]=Ɨ.Ī[Ï];}if(ϧ.Ϥ
[Ï])ϧ.Ε[Ï]+=ŋ.Ń[Ï];if(ϧ.ϥ[Ï])ϧ.Ε[Ï]+=ϯ.Ε[Ï];if(ϧ.Γ[Ï]>0)ϧ.Ε[Ï]+=ϳ.Ε[Ï]-ϳ.ϣ[Ï];Ж.Α[Ï]=ŋ.Ł[Ï]+М.Ł[Ï]+Ј.Ѕ[Ï]+Ј.Ѓ[Ï];ˁ=ϧ.ϣ[Ï]
>0||ϳ.ϱ[Ï]||ϯ.Ϡ[Ï];ˀ=Ж.Е[Ï]>0;ī=ŋ.ī[Ï]+ĭ.ī[Ï]+ĵ.ī[Ï]+Ɨ.ī[Ï];ˆ=ŋ.ĩ[Ï]+ĭ.ĩ[Ï]+ĵ.ĩ[Ï]+Ɨ.ĩ[Ï];if(ˁ&&!ˀ){ĭ.Ĩ[Ï]=ī*ŋ.ŉ;ĭ.ħ[Ï]=ˆ
*ŋ.ŉ;}if(ˀ&&!ˁ){ĭ.Ħ[Ï]=ī*ŋ.Ŋ;ĭ.ĳ[Ï]=ˆ*ŋ.Ŋ;}if(ˁ&&ˀ){ĭ.Ĩ[Ï]=ī*ŋ.ŉ/2;ĭ.ħ[Ï]=ˆ*ŋ.ŉ/2;ĭ.Ħ[Ï]=ī*ŋ.Ŋ/2;ĭ.ĳ[Ï]=ˆ*ŋ.Ŋ/2;}Ͱ[Ï]=ϳ.Ε
[Ï]+ϯ.Ε[Ï];if(ϳ.ϱ[Ï])Ί[Ï]=ϳ.Ε[Ï]+ϯ.Ε[Ï];if(ϯ.Ϡ[Ï])Λ[Ï]=ϳ.Ε[Ï]+ϯ.Ε[Ï];Ι[Ï]=ϳ.Γ[Ï]+ϧ.Γ[Ï];Θ[Ï]=ϧ.ϣ[Ï]+ϳ.ϣ[Ï];Η[Ï]=Ј.Ѕ[Ï]+ŋ.
Ł[Ï]+М.Ł[Ï];Ζ[Ï]=ĭ.ī[Ï]+ŋ.ī[Ï]+ĵ.ī[Ï]+Ɨ.ī[Ï];}ϧ.ř();ϳ.ř();ϯ.ř();Ж.ř();М.ř();Ј.ř();ŋ.ř();}void ʾ(){ͱ.ʲ="running...";List<
MyInventoryItem>ʽ=new List<MyInventoryItem>();щ ʼ;for(int Ï=0;Ï<maxCS;Ï++){foreach(var ʻ in γ[Ï]){if(δ[Ï].Count>0){try{ʼ=δ[Ï].First(Ä=>
ʻ.Я(Ä));}catch{ͱ.с("no ice movement possible\nCS: "+(ʻ.Ƣ+1)+" no destination found!");ʼ=ʻ;ͱ.ʲ="check errors";}ʻ.б.
GetItems(ʽ);foreach(var ʺ in ʽ.Where(w=>w.Type.SubtypeId.Equals("Ice")))ʻ.б.TransferItemTo(ʼ.б,ʺ,ʺ.Amount);}}foreach(var ʻ in α[
Ï]){ʻ.б.GetItems(ʽ);if(β[Ï].Count>0){try{ʼ=β[Ï].First(Ä=>ʻ.б.IsConnectedTo(Ä.б));}catch{ͱ.с(
"no item movement possible\nCS: "+(ʻ.Ƣ+1)+" no destination found!");ʼ=ʻ;ͱ.ʲ="check errors";}foreach(var ʺ in ʽ.Where(w=>!w.Type.SubtypeId.Equals("Ice")))
ʻ.б.TransferItemTo(ʼ.б,ʺ,ʺ.Amount);}}}}void ˮ(){int Ï;bool[]ˬ=new bool[maxCS];bool[]ˤ=new bool[maxCS];bool[]ˣ=new bool[
maxCS];bool[]ˢ=new bool[maxCS];bool[]ˡ=new bool[maxCS];bool[]ˠ=new bool[maxCS];for(Ï=0;Ï<maxCS;Ï++){if(М.Й[Ï]){if(Ж.ϲ[Ï]>=κ){
ζ[Ï]=false;π=true;foreach(var ˑ in ͽ.Where(ˑ=>ˑ.Ƣ==Ï))ˑ.д=false;}else{foreach(var ˑ in ͽ.Where(ˑ=>ˑ.Ƣ==Ï))ˑ.д=true;π=
false;}}θ[Ï]|=ϧ.ϲ[Ï]<ι;θ[Ï]&=ϧ.ϲ[Ï]<ς;ζ[Ï]|=Ж.ϲ[Ï]<λ;ζ[Ï]&=Ж.ϲ[Ï]<κ;ˬ[Ï]=Ј.ȋ[Ï]&&Ј.Ѐ[Ï]>Ј.Ё[Ï];ˤ[Ï]=ϯ.ϡ[Ï]+ϳ.ϡ[Ï]>ŋ.Ń[Ï];ˣ[Ï]
=Ј.ȋ[Ï]&&Ј.Ѐ[Ï]<Ј.Ё[Ï];if(Ј.Ї>0.6f){foreach(var ˏ in Ј.І.Where(ʏ=>ʏ.Ƣ==Ï)){ˏ.Џ=!ζ[Ï];ˠ[Ï]|=!ˏ.Џ&&ζ[Ï];}}if(ŋ.ň[Ï]&&(θ[Ï]
||ζ[Ï])&&ĭ.Ī[Ï]+ĵ.Ī[Ï]+Ɨ.Ī[Ï]+ŋ.Ī[Ï]<0.1)ͱ.с("O2/H2 production not possible\nCS: "+(Ï+1)+" - no more ice stored");else{ˢ[Ï
]=ŋ.ň[Ï]&&(θ[Ï]||ζ[Ï]);ˡ[Ï]=!ˢ[Ï];}}foreach(var ˎ in ǂ.Where(ˊ=>ŋ.ň[ˊ.Ƣ])){Ï=ˎ.Ƣ;ˎ.д=ˤ[Ï]||θ[Ï]||!ˢ[Ï];ˡ[Ï]|=ˤ[Ï]&&ζ[Ï]&&
!θ[Ï]&&ϧ.ϲ[Ï]>ς&&Ж.ϲ[Ï]>λ;}foreach(var ˎ in ǃ.Where(ˍ=>ŋ.ň[ˍ.Ƣ])){Ï=ˎ.Ƣ;ˡ[Ï]|=ˢ[Ï]&&!ζ[Ï]&&((ˣ[Ï]&&Ж.ϲ[Ï]>=κ)||(ˬ[Ï]&&Ж.ϲ
[Ï]>=κ));ˎ.д=!ˢ[Ï]||ζ[Ï]||ˡ[Ï]||(Ж.ϲ[Ï]<κ&&(ˬ[Ï]||ˣ[Ï]));}foreach(var ˌ in Ά)ˌ.г=ˌ.д=ˢ[ˌ.Ƣ]&&!ˡ[ˌ.Ƣ];if(ˢ.Any(ɭ=>ɭ==true)
)ͱ.ʴ="generating...";else ͱ.ʴ="running...";}void ˋ(){if(ͳ.Any(Ä=>Ä.IsMainCockpit&&Ä.IsUnderControl))ͻ.ForEach(ˊ=>ˊ.ϭ());}
void ː(){if(ο.Any()){if(alignFarms&&!π)ο.ForEach(ʰ=>ͱ.о(ʰ.Ǽ,ʰ.ǻ()));else ο.ForEach(ʰ=>ͱ.о(ʰ.Ǽ,ʰ.Ǹ()));}else ͱ.о(
"no masts found","");if(ξ){ν.Enabled=alignFarms;ͱ.о(ν.CustomName,"online: "+ν.Enabled);}}static string ʦ(string ʥ){string ʤ;if(!en)if(
translations.TryGetValue(ʥ,out ʤ))return ʤ;return ʥ;}void ʣ(){switch(ϑ){case 0:ͱ.ʫ();ƻ.ƺ=ǜ.ƺ=Ǔ.ƺ=ǚ.ƺ=false;ͱ.ќ("uninstalling...\n");
ͱ.ʪ("cleaning all generators");break;case 1:Ά.Clear();ɢ.ƺ=false;List<IMyGasGenerator>ʢ=new List<IMyGasGenerator>();
GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(ʢ);ʟ(ʢ);ͱ.ʨ();ͱ.ʪ("cleaning all gas tanks");break;case 2:ǃ.Clear();ǂ.Clear();ɜ.ƺ=ɡ.ƺ=
false;List<IMyGasTank>ʡ=new List<IMyGasTank>();GridTerminalSystem.GetBlocksOfType<IMyGasTank>(ʡ);ʟ(ʡ);ͱ.ʨ();ͱ.ʪ(
"cleaning all hydrogen thrusters");break;case 3:ͻ.Clear();ɞ.ƺ=false;List<IMyThrust>ʠ=new List<IMyThrust>();GridTerminalSystem.GetBlocksOfType<IMyThrust>(
ʠ,ȏ=>ȏ.BlockDefinition.SubtypeId.Contains("Hydrogen"));ʟ(ʠ);ͱ.ʨ();ͱ.ʪ("cleaning all hydrogen engines");break;case 4:ͼ.
Clear();ǐ.ƺ=false;List<IMyPowerProducer>ʅ=new List<IMyPowerProducer>();GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(ʅ
,ʑ=>ʑ.BlockDefinition.SubtypeId.Contains("Hydrogen"));ʟ(ʅ);ͱ.ʨ();ͱ.ʪ("cleaning all air vents");break;case 5:Έ.Clear();Ǐ.ƺ
=false;List<IMyAirVent>ʐ=new List<IMyAirVent>();GridTerminalSystem.GetBlocksOfType<IMyAirVent>(ʐ);ʟ(ʐ);ͱ.ʨ();ͱ.ʪ(
"cleaning all cargo containers");break;case 6:ͺ.Clear();ɠ.ƺ=false;List<IMyCargoContainer>ʎ=new List<IMyCargoContainer>();GridTerminalSystem.
GetBlocksOfType<IMyCargoContainer>(ʎ);ʟ(ʎ);ͱ.ʨ();ͱ.ʪ("cleaning all connectors/collectors");break;case 7:ͷ.Clear();Ͷ.Clear();List<
IMyShipConnector>ʍ=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(ʍ);ʟ(ʍ);List<IMyCollector>ʌ=new
List<IMyCollector>();GridTerminalSystem.GetBlocksOfType<IMyCollector>(ʌ);ʟ(ʌ);ͱ.ʨ();ͱ.ʪ("cleaning all gas farms");break;case
8:ͽ.Clear();ɝ.ƺ=false;List<IMyOxygenFarm>ʋ=new List<IMyOxygenFarm>();GridTerminalSystem.GetBlocksOfType<IMyOxygenFarm>(ʋ)
;ʟ(ʋ);ͱ.ʨ();ͱ.ʪ("cleaning all alignment settings");foreach(var ʓ in ο)ʓ.Ǹ();ο.Clear();ν=null;ξ=false;ͱ.ʨ();ͱ.ʪ(
"cleaning saved data");break;case 9:Ϋ=false;Storage=string.Empty;ͱ.ʨ();ͱ.ʪ("cleaning all screens");break;case 10:List<IMyTerminalBlock>ʗ=new
List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(ʗ);ʟ(ʗ);ͱ.ʨ();ͱ.ʪ("cleaning all sensors"
);break;case 11:ʹ.Clear();ͳ.Clear();ͱ.ʨ();break;case 12:ͱ.ќ("uninstalling...done\n");break;case 13:ͱ.ќ(
"script running terminated");Runtime.UpdateFrequency=UpdateFrequency.Once;break;default:break;}ϑ++;}void ʟ<Ɵ>(List<Ɵ>ɲ)where Ɵ:IMyTerminalBlock{
foreach(var ű in ɲ){if(ű.CustomName.EndsWith(ί))ű.CustomName=ű.CustomName.Remove(ű.CustomName.Length-ί.Length-1,ί.Length+1);if(
ű.CustomData.Contains(ΰ)){StringBuilder Ƈ=new StringBuilder();int Ɔ=0;string[]Ø=ű.CustomData.Split('\n');while(Ɔ<Ø.Length
&&!Ø[Ɔ].Contains(ΰ)){Ƈ.AppendLine(Ø[Ɔ]);Ɔ++;}if(Ɔ>=Ø.Length)return;string[]ʹ=Ø[Ɔ].Split(',');if(ʹ.Length>1){string ʷ="";
for(int w=0;w<ʹ.Length;w++){if(!ʹ[w].Contains(ΰ))ʷ+=ʹ[w].Trim(',',' ')+", ";}Ƈ.Append(ʷ.TrimEnd(',',' ')+"\n");Ɔ++;while(Ɔ<
Ø.Length&&!Ø[Ɔ].Contains(η)){if(Ø[Ɔ].Contains('=')){string[]ƃ=Ø[Ɔ].Split('=',',');LayoutTypes ʶ;if(Enum.TryParse<
LayoutTypes>(ƃ[1],out ʶ))Ƈ.Append(ƃ[0]+"=\n");else Ƈ.AppendLine(Ø[Ɔ]);}else Ƈ.AppendLine(Ø[Ɔ]);Ɔ++;}Ƈ.AppendLine(η);Ɔ++;while(Ɔ<Ø.
Length&&!Ø[Ɔ].Contains(σ)){if(Ø[Ɔ].Contains('=')){string[]ƃ=Ø[Ɔ].Split('=');BlockSetupTags ʶ;if(Enum.TryParse<BlockSetupTags>(
ƃ[0],out ʶ)){switch((int)ʶ){case 0:Ƈ.AppendLine(Ø[Ɔ]);break;case 1:Ƈ.AppendLine(Ø[Ɔ]);break;case 2:break;case 3:break;
case 4:Ƈ.AppendLine(Ø[Ɔ]);break;}}else Ƈ.AppendLine(Ø[Ɔ]);}else Ƈ.AppendLine(Ø[Ɔ]);Ɔ++;}Ƈ.AppendLine(σ);}while(Ɔ<Ø.Length&&!
Ø[Ɔ].Contains(ϕ))Ɔ++;Ɔ++;while(Ɔ<Ø.Length){Ƈ.AppendLine(Ø[Ɔ]);Ɔ++;}ű.CustomData=Ƈ.ToString().Trim('\n');}}}class ʵ{public
string ʴ{get{return"H2 / O2 Control: "+ʳ+"\n";}set{ʳ=value;}}string ʳ;public string ʲ{get{return"shifting ice: "+ʸ+"\n";}set{ʸ
=value;}}string ʸ;StringBuilder ʱ=new StringBuilder();StringBuilder ʯ=new StringBuilder();Dictionary<int,string>ʮ=new
Dictionary<int,string>();StringBuilder ʭ=new StringBuilder();public ʵ(){ʳ=ʸ="disabled";}public void Ľ(){ʱ.Clear();ʭ.Clear();ʳ=ʸ=
"disabled";}public void ʬ()=>ʮ.Clear();public void ʫ()=>ʯ.Clear();public void ʪ(string ʩ)=>ʯ.Append(ʩ+"...");public void ʨ()=>ʨ(
"done");public void ʨ(string Ă)=>ʯ.AppendLine(Ă);public void ќ(string ʩ)=>ʯ.Append(ʩ);public void с(string р){int п=р.
GetHashCode();if(!ʮ.ContainsKey(п))ʮ.Add(п,р);}public void о(string ɬ,string ʩ){if(farmMastNames.Length>0||extAlignProgBlockName.
Length>0)ʱ.AppendLine(ɬ+":\n"+ʩ);else ʱ.Clear();}public string н(){if(ύ>=ʮ.Count)ύ=0;if(ʮ.Count>0)return
"argument \"resetErrors\"\ndeletes all error messages\nERROR: "+(ύ+1)+" of "+ʮ.Count+"\n"+ʮ.ElementAt(ύ).Value+"\n";else return"";}public string м(){StringBuilder л=new StringBuilder(
"");if(ʮ.Count>0)л.Append("argument \"resetErrors\"\ndeletes all error messages\n");string[]к=ʮ.Values.ToArray();for(int w
=0;w<к.Length;w++)л.Append("ERROR: "+(w+1)+" of "+к.Length+"\n"+к[w]+"\n");return л.ToString();}public string й(){if(ʯ.
Length>0)return ʯ+"\n";else return"";}public string и(){if(ʭ.Length>0)return ʭ+"\n";else return"";}public string з()=>
"max. Conveyors available: "+maxCS+"\n"+ʴ+ʲ+"\n"+ʱ;public string ж()=>"-------- H2/O2 Control Program --------\n"+м()+"\n"+й()+з()+"\n"+и();}
interface е{long Ƙ{get;}int Ƣ{get;}bool д{get;set;}bool г{get;set;}bool в{get;set;}Vector2 т(RectangleF ŝ,ü à);bool ѐ{get;}void ю
();void э(bool ɥ);Vector2 Ġ(Vector2 ÿ,ü à);}interface ь:е{string Ы{get;}new int Ƣ{get;set;}bool ы{get;}void я();Vector2 Ġ
(Vector2 ÿ,Vector2 Ą,ü à,е ê);}interface щ:ь{bool ш{get;set;}bool ч{get;set;}float ц{get;}float х{get;}float ф{get;}bool
у{get;}bool ъ{get;}IMyInventory б{get;}bool Я(щ ϴ);}interface Ъ:ь{float Ś{get;}float Щ{get;}float Ш{get;}bool Ч{get;}bool
Ц{get;}}class Х:е{public long Ƙ{get;}public int Ƣ{get;}Action<bool>Ф;Func<bool>У;string Т;string С;public bool д{get{
return У();}set{Ф(value);}}public bool г{get{return У();}set{Ф(value);}}public bool в{get{return У();}set{Ф(value);}}public
bool ѐ{get{return false;}}public Х(Action<bool>Р,Func<bool>П,string О,string ʩ){Ƣ=-1;Ф=Р;У=П;С=ʩ;Т=О;Ƙ=GetHashCode();}public
void э(bool ɥ){return;}public void ю(){return;}public Vector2 т(RectangleF ŝ,ü à){if(!à.P)à.o(0.65f);Vector2 è=new Vector2(ŝ
.Center.X,ŝ.Y);è.Y+=à.X(è,С,mainColor,TextAlignment.CENTER).Y;if(д)è.Y+=à.X(è,ʦ("deactivate"),mainColor,TextAlignment.
CENTER).Y;else è.Y+=à.X(è,ʦ("activate"),mainColor,TextAlignment.CENTER).Y;à.k();return è;}public Vector2 Ġ(Vector2 ÿ,ü à){
Vector2 è=ÿ,Ą=new Vector2(à.F);float Ă=à.E/2;è.X+=à.ą(ÿ,à.F,д).X+(à.E/2);Ą.X+=à.X(è,Т,mainColor,TextAlignment.LEFT).X+(à.E/2);
if(à.ƛ!=null&&Ƙ==à.ƛ.Ƙ)à.ģ(ÿ-new Vector2(Ă*2),Ą.X+(4*Ă),Ą.Y+(4*Ă),selectionColor,false,Ă);return Ą;}}class а:е{public long
Ƙ{get;}public int Ƣ{get;}Func<float,float>ʺ;float Ć;const float Ÿ=0.05f;string С;public bool д{get{return false;}set{
return;}}public bool г{get{return false;}set{if(value)Ć=ʺ(-Ÿ);}}public bool в{get{return false;}set{if(value)Ć=ʺ(Ÿ);}}public
bool ѐ{get{return false;}}public а(Func<float,float>Р,float П,string ʩ){Ƣ=-1;ʺ=Р;Ć=П;С=ʩ;Ƙ=GetHashCode();}public void э(bool
ɥ){return;}public void ю(){return;}public Vector2 т(RectangleF ŝ,ü à){if(!à.P)à.o(0.65f);Vector2 è=new Vector2(ŝ.Center.X
,ŝ.Y);è.Y+=à.X(è,С,mainColor,TextAlignment.CENTER).Y;è.X=ŝ.X;if(à.O){à.X(è,(-Ÿ).ToString("P0",ϓ),mainColor,TextAlignment.
LEFT);è.X=ŝ.Right;à.X(è,"+"+Ÿ.ToString("P0",ϓ),mainColor,TextAlignment.RIGHT);}else{à.X(è,ʦ("Primary")+": "+ʦ(
"decrease limit by ")+Ÿ.ToString("P0",ϓ),mainColor,TextAlignment.LEFT);è.X=ŝ.Right;à.X(è,ʦ("Secondary")+": "+ʦ("increase limit by ")+Ÿ.
ToString("P0",ϓ),mainColor,TextAlignment.RIGHT);}à.k();return è;}public Vector2 Ġ(Vector2 Ю,ü à){Vector2 è=Ю;Vector2 Ą=new
Vector2(à.R.MeasureStringInPixels(new StringBuilder(1f.ToString("P0",ϓ)),à.I,à.G).X,à.F);float Ă=à.E/2;à.X(è,Ć.ToString("P0",ϓ)
,mainColor,TextAlignment.CENTER);if(à.ƛ!=null&&Ƙ==à.ƛ.Ƙ){è.X=Ю.X-(Ą.X/2);à.ģ(è-new Vector2(Ă*2),Ą.X+(4*Ă),Ą.Y+(4*Ă),
selectionColor,false,Ă);}return Ą;}}abstract class Э{protected IMyTerminalBlock ű;protected List<BlockSetupTags>Ь=new List<
BlockSetupTags>();static string[]Ų=new string[Enum.GetValues(typeof(BlockSetupTags)).Length];public string Ы{get{return Ѭ();}}string ɬ
="";public long Ƙ{get{return ű.EntityId;}}public int Ƣ{get{return ŕ;}set{ѩ(value);}}int ŕ=0;protected bool Ѡ=false;
protected bool ѕ=false;protected int џ=-1;protected int ū=0;bool ў=false;static Э(){Ų[0]=" - shown name in main screen";Ų[1]=
" - conveyor connected system number";Ų[2]=" - false, cargo ignored by auto. items movement";Ų[3]=" - true, if used as ice storage";Ų[4]=
" - room number - 0 for outside vent";}public Э(IMyTerminalBlock ű){if(ű!=null){this.ű=ű;if(ώ)ű.CustomData="";}}protected void Ƌ(params int[]Ɗ){foreach(var Ê
in Ɗ)if(!Ь.Contains((BlockSetupTags)Ê))Ь.Add((BlockSetupTags)Ê);}protected void ѝ(){if(ώ)ű.CustomData="";if(ū!=ű.
CustomData.GetHashCode()){ў=false;ű.CustomData=ƈ();ū=ű.CustomData.GetHashCode();if(!ў&&ű.CustomName.EndsWith(ί))ű.CustomName=ű.
CustomName.Remove(ű.CustomName.Length-ί.Length-1,ί.Length+1);}}string ƈ(){var Ƈ=new StringBuilder();string[]Ø=ű.CustomData.Trim().
Trim('\n').Split('\n');int w,Ɔ=0;ў=false;while(Ɔ<Ø.Length&&!Ø[Ɔ].Trim().StartsWith("@")){Ƈ.AppendLine(Ø[Ɔ]);Ɔ++;}if(Ɔ<Ø.
Length){if(!Ø[Ɔ].Contains(ΰ))Ƈ.Append(Ø[Ɔ].Trim()+", "+ΰ+"\n");else Ƈ.Append(Ø[Ɔ].Trim()+"\n");Ɔ++;}else Ƈ.Append(ΰ+"\n");List
<BlockSetupTags>Ƅ=Ь.ToList();string[]ƃ;Ƈ.Append(η+"\n");if(Ɔ<Ø.Length&&Ø[Ɔ].StartsWith(η)){Ɔ++;while(Ɔ<Ø.Length&&!Ø[Ɔ].
StartsWith(σ)){if(Ø[Ɔ].Contains('=')){ƃ=Ø[Ɔ].Split('=');w=Ƅ.IndexOf((BlockSetupTags)Enum.Parse(typeof(BlockSetupTags),ƃ[0]));if(w
>=0){Ë(Ƅ[w],ƃ[1]);Ƅ.RemoveAt(w);}}Ƈ.Append(Ø[Ɔ]+"\n");Ɔ++;}Ɔ++;}if(Ƅ.Count>0){ў|=true;foreach(var Ê in Ƅ)Ƈ.Append(Ê+Ã(Ê)+
"\n");}Ƈ.Append(σ+"\n"+ϋ+"\n");Ƅ=Ь.ToList();if(Ɔ<Ø.Length&&Ø[Ɔ].StartsWith(ϋ)){Ɔ++;while(Ɔ<Ø.Length&&!Ø[Ɔ].StartsWith(ϕ)){if
(Ø[Ɔ].Contains('-')){ƃ=Ø[Ɔ].Trim().Split('-');w=Ƅ.IndexOf((BlockSetupTags)Enum.Parse(typeof(BlockSetupTags),ƃ[0]));if(w>=
0)Ƅ.RemoveAt(w);}Ƈ.Append(Ø[Ɔ]+"\n");Ɔ++;}Ɔ++;}if(Ƅ.Count>0)foreach(var Ê in Ƅ)Ƈ.Append(Ê+Ų[(int)Ê]+"\n");Ƈ.Append(ϕ+"\n"
);while(Ɔ<Ø.Length){if(Ø[Ɔ].StartsWith("[readme")){do Ɔ++;while(Ɔ<Ø.Length&&!Ø[Ɔ].StartsWith("[/readme"));Ɔ++;}if(Ɔ<Ø.
Length)Ƈ.Append(Ø[Ɔ]+"\n");Ɔ++;}if(ў&&!ű.CustomName.EndsWith(ί)){ű.CustomName+=" "+ί;ͱ.с(
"block setup needed\nsearch for blocks with\n"+ί+" at its Custom Name");}return Ƈ.ToString().Trim('\n').Trim();}void Ë(BlockSetupTags Ê,string É){bool ѭ=false;switch(
(int)Ê){case 0:try{ɬ=É.Trim();}catch{ɬ="";ѭ|=true;}break;case 1:try{ŕ=Int32.Parse(É)-1;}catch{ŕ=-1;ѭ|=true;}break;case 2:
try{Ѡ=Boolean.Parse(É);}catch{Ѡ=ű is IMyCargoContainer;ѭ|=true;}break;case 3:try{ѕ=Boolean.Parse(É);}catch{ѕ=ű is
IMyCargoContainer;ѭ|=true;}break;case 4:try{џ=int.Parse(É);}catch{џ=-1;ѭ|=true;}break;}if(ѭ){ͱ.с("couldn't read Custom Data\n"+ű.
CustomName+"\n"+Ê+"="+É);}ў|=ѭ;}string Ã(BlockSetupTags Ê){ў=true;string л="=";switch((int)Ê){case 1:л+=(Ƣ+1);break;case 2:Ѡ=false
;л+=Ѡ;break;case 3:ѕ=true;л+=ѕ;break;case 4:џ=-1;break;}return л;}string Ѭ(){if(ɬ.Trim().Length>0)return ɬ;return ű.
CustomName;}protected string ѫ(BlockSetupTags Ê,string Ѫ){var Ù=new StringBuilder();string[]Ø=ű.CustomData.Split('\n');for(int w=0
;w<Ø.Length;w++){if(Ø[w].StartsWith(Ê.ToString(),StringComparison.Ordinal))Ù.Append(Ê+"="+Ѫ+"\n");else Ù.Append(Ø[w]+"\n"
);}return Ù.ToString().TrimStart('\n').TrimEnd('\n');}void ѩ(int ɰ){ŕ=ɰ;ű.CustomData=ѫ(BlockSetupTags.Conveyor,(ɰ+1).
ToString());ū=ű.CustomData.GetHashCode();}}abstract class Ѩ:Э,ь{protected IMyInventory ѧ{get{return ű.GetInventory(0);}}
protected float Ѧ{get{return(float)(ѧ.MaxVolume*1000);}}virtual protected float Ш{get{return(float)(ѧ.CurrentVolume*1000);}}
protected float ѥ;protected bool Ѥ;protected bool ѣ;protected Vector3I Ѣ;public bool д{get{return(ű as IMyFunctionalBlock).
Enabled;}set{(ű as IMyFunctionalBlock).Enabled=value;}}public virtual bool г{get{return false;}set{return;}}public virtual bool
в{get{return false;}set{return;}}public bool ы{get{return ű.IsWorking;}}protected Color Ȅ;protected bool ѡ{get{return!ű.
IsFunctional;}}public bool ѐ{get;private set;}protected Ѩ(IMyTerminalBlock ű,params int[]Ɗ):base(ű){if(ű!=null){ѥ=ű.CubeGrid.
GridSize;Ѥ=this.ű.CubeGrid.GridSize>2f;Ѣ=this.ű.Max-this.ű.Min+new Vector3I(1,1,1);ѐ=false;Ƌ(0);Ƌ(Ɗ);ѝ();}}public virtual
Vector2 т(RectangleF ŝ,ü à){if(!à.P)à.o(0.65f);Vector2 è=new Vector2(ŝ.Center.X,ŝ.Y);è.Y+=à.X(è,Ы+", CS: "+(Ƣ+1),mainColor,
TextAlignment.CENTER).Y;return è;}protected void ї(bool ɸ){Ѡ=ɸ;ű.CustomData=ѫ(BlockSetupTags.ItemMove,ɸ.ToString());ū=ű.CustomData.
GetHashCode();}protected void і(bool ѕ){this.ѕ=ѕ;ű.CustomData=ѫ(BlockSetupTags.IceCargo,ѕ.ToString());ū=ű.CustomData.GetHashCode();
}virtual protected bool є(){if(ű.IsWorking&&!ѡ){Ȅ=colorOn;return true;}else{Ȅ=colorOff;return false;}}public void э(bool
ɥ)=>ѐ=ɥ;public void ю()=>ѐ=!ѐ;virtual public void я()=>ѝ();virtual public Vector2 Ġ(Vector2 ÿ,ü à){return ÿ;}abstract
public Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê);}abstract class ѓ:Ѩ,Ъ{public const float Ĭ=1/1.1618f;IMyGasTank ˎ;public float Щ
{get{return ˎ.Capacity;}}new public float Ш{get{return Щ*(float)Ś;}}public bool Ч{get{return ы&&Ś<1;}}public bool Ц{get{
return!ђ&&ы&&Ś>0;}}public float Ś{get{return(float)ˎ.FilledRatio;}}public bool ђ{get{return ˎ.Stockpile;}set{ˎ.Stockpile=value
;}}protected bool ё{get{return ˎ.AutoRefillBottles;}set{ˎ.AutoRefillBottles=value;}}protected Color ȅ;override public
bool г{get{return ђ;}set{ђ=value;}}override public bool в{get{return ё;}set{ё=value;}}protected ѓ(IMyGasTank ˎ):base(ˎ,1){
this.ˎ=ˎ;if(Ѥ)ѣ=(Ѣ.X*Ѣ.Y*Ѣ.Z*Math.Pow(ѥ,3))>370f;else ѣ=(Ѣ.X*Ѣ.Y*Ѣ.Z*Math.Pow(ѥ,3))>15f;}override public Vector2 т(
RectangleF ŝ,ü à){Vector2 è=base.т(ŝ,à);è.X=ŝ.X;è.Y+=à.X(è,ʦ("H2 level: ")+Ś.ToString("P1",ϓ)+" = "+ψ(Ш),mainColor,TextAlignment.
LEFT).Y;if(à.O)è.X+=à.X(è,ʦ("provide gas"),mainColor,TextAlignment.LEFT).X+(à.E/2);else è.X+=à.X(è,ʦ("Primary")+": "+ʦ(
"provide gas"),mainColor,TextAlignment.LEFT).X+(à.E/2);à.ą(è,à.F,!г);è.X=ŝ.Right-à.F;à.ą(è,à.F,в);è.X-=à.E/2;if(à.O)à.X(è,ʦ(
"bottle refill"),mainColor,TextAlignment.RIGHT);else à.X(è,ʦ("Secondary")+": "+ʦ("auto.")+" "+ʦ("bottle refill"),mainColor,
TextAlignment.RIGHT);à.k();return è;}public void Ĳ(){д=true;ђ=false;}public override Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê){є();if(Ȅ
==colorOn&&Ś<0.002)Ȅ=colorFull;float İ=à.G;à.u(0.8f*(Ą.X/100));float Ă=Math.Min(Ą.X,Ą.Y)*0.05f;float ћ=à.E/2;if(!Ѥ){ÿ.Y+=Ą
.Y*0.3f;Ą.Y*=0.7f;}if(!ѣ){ÿ.X+=Ą.X*0.2f;Ą.X*=0.6f;}Vector2 è=ÿ,į=ÿ+new Vector2(Ą.X,0);Vector2 Ė=ÿ+new Vector2(0,Ą.Y),ġ=į+
new Vector2(0,Ą.Y);if(à.ơ&&ê!=null&&Ƙ==ê.Ƙ)à.ģ(ÿ-new Vector2(ћ*2),Ą.X+(4*ћ),Ą.Y+(4*ћ),selectionColor,false,ћ);if(à.ơ&&ѐ)à.ģ
(ÿ-new Vector2(ћ),Ą.X+(2*ћ),Ą.Y+(2*ћ),selectedColor,true);è=ÿ;è.Y+=Ą.Y*0.2f;à.ģ(è,Ą.X,Ą.Y*0.8f,Ȅ,false,Ă);è.X+=Ă;è.Y+=Ă;à
.ģ(è,Ą.X-(2*Ă),(Ą.Y*0.8f)-(2*Ă),ȅ,false,Ă);è=ÿ;è.X+=Ą.X*0.35f;if(!ђ)à.ģ(è+new Vector2(Ă,0),(Ą.X*0.3f)-(2*Ă),(Ą.Y*0.2f)+(2
*Ă),ȅ,true);à.Ę(è,Ă,Ą.Y*0.2f,Ȅ);if(!ϔ){è-=new Vector2(Ă);à.X(è,(Ƣ+1).ToString(),mainColor,TextAlignment.RIGHT);è+=new
Vector2(Ă);}è=į;è.X-=Ą.X*0.35f;à.Ę(è,-Ă,Ą.Y*0.2f,Ȅ);è=Ė;è.X+=2*Ă;è.Y-=2*Ă;à.ģ(è,Ą.X-(4*Ă),((-Ą.Y*0.8f)+(4*Ă))*Ś,ȅ,true);if(ѡ)à.
Ġ(ÿ+new Vector2(Ą.X/2,Ą.Y/2),Ą.X*0.8f,"Danger",mainColor);à.u(İ);return new Vector2(Ą.X,Ą.Y);}}abstract class њ:Ѩ,щ{
virtual public bool ш{get{return Ѡ;}set{ї(value);}}virtual public bool ч{get{return ѕ;}set{і(value);}}public float ц{get{return
Н("Ice");}}public float х{get{return ф+ц;}}public float ф{get{return((float)(Ѧ-Ш)/ρ);}}virtual public bool у{get{return ы
&&ѕ&&ф>0;}}virtual public bool ъ{get{return ы&&ц>0;}}public IMyInventory б{get{return ű.GetInventory(0);}}public override
bool г{get{return ѕ;}set{і(value);}}public override bool в{get{return Ѡ;}set{ї(value);}}public њ(IMyTerminalBlock ű,params
int[]Ɗ):base(ű,Ɗ){}override public Vector2 т(RectangleF ŝ,ü à){Vector2 è=base.т(ŝ,à);if(à.O)è.Y+=à.X(è,ʦ("stored")+": "+ϊ(ц
)+" / "+ʦ("storage")+": "+ϊ(х),mainColor,TextAlignment.CENTER).Y;else è.Y+=à.X(è,ʦ("ice")+" "+ʦ("stored")+": "+ϊ(ц)+" / "
+ʦ("available ice storage")+": "+ϊ(х),mainColor,TextAlignment.CENTER).Y;è.X=ŝ.X;if(à.O)è.X+=à.X(è,ʦ("ice cargo"),
mainColor,TextAlignment.LEFT).X+(à.E/2);else è.X+=à.X(è,ʦ("Primary")+": "+ʦ("ice cargo"),mainColor,TextAlignment.LEFT).X+(à.E/2);
à.ą(è,à.F,г);è.X=ŝ.Right-à.F;à.ą(è,à.F,в);è.X-=à.E/2;if(à.O)à.X(è,ʦ("item movement"),mainColor,TextAlignment.RIGHT);else
à.X(è,ʦ("Secondary")+": "+ʦ("auto.")+" "+ʦ("item movement"),mainColor,TextAlignment.RIGHT);à.k();return è;}protected void
љ(){ѝ();if(Ѡ&&у)δ[Ƣ].Add(this);if(Ѡ&&ъ&&!ѕ)γ[Ƣ].Add(this);}List<MyInventoryItem>ј(){List<MyInventoryItem>ʽ=new List<
MyInventoryItem>(ѧ.ItemCount);ѧ.GetItems(ʽ);return ʽ;}float Н(string Ϲ){MyFixedPoint Ϣ=0;List<MyInventoryItem>Ϸ=ј();Ϸ.ForEach(ʺ=>{if(ʺ.
Type.SubtypeId.Equals(Ϲ))Ϣ+=ʺ.Amount;});return(float)Ϣ;}protected void ϵ(){Ȅ=colorFull;if(Ѧ-Ш>0)Ȅ=colorSpace;}public bool Я(
щ ϴ)=>ϴ.у&&ъ&&Ƣ==ϴ.Ƣ&&б.IsConnectedTo(ϴ.б);}class ϳ:Ѩ,Ъ{public const float Ĭ=2f;public static float[]Γ=new float[maxCS];
public static float[]Β=new float[maxCS];public static float[]Κ=new float[maxCS];public static float[]ϣ=new float[maxCS];public
static float[]Ε=new float[maxCS];public static float[]ϡ=new float[maxCS];public static float[]Δ=new float[maxCS];public static
float[]ˈ=new float[maxCS];public static float[]ϲ=new float[maxCS];public static bool[]ϱ=new bool[maxCS];public static Queue<
float[]>ϟ=new Queue<float[]>(outputDiagramDepth);public static Queue<float[]>Ϟ=new Queue<float[]>(outputDiagramDepth);public
static int[]ϝ=new int[maxCS+1];const int Ϝ=100000;IMyPowerProducer ϰ;public float Ś{get;private set;}public float Щ{get;
private set;}new public float Ш{get{return Щ*(float)Ś;}}public float ϸ{get{return Щ*(float)(1-Ś);}}public bool Ч{get{return ы&&
Ś<1;}}public bool Ц{get{return false;}}public float Ϛ{get;private set;}public float ϙ{get{return Ϛ*ϛ;}}float ϛ{get{return
ϰ.CurrentOutput/ϰ.MaxOutput;}}public static void Ľ(){φ(Γ);φ(Β);φ(ϣ);φ(Κ);φ(ϲ);φ(Ε);φ(ϡ);φ(Δ);φ(ˈ);φ(ϱ);}public static
void ř()=>ϝ=Τ(Ε,ϡ,ϝ,Ϝ,ϟ,Ϟ);public ϳ(IMyPowerProducer ϰ):base(ϰ,1){this.ϰ=ϰ;ѣ=true;if(Ѥ)Ϛ=500*60;else Ϛ=50*60;for(int ŕ=0;ŕ<
maxCS+1;ŕ++)ϝ[ŕ]=Ϝ;}override public Vector2 т(RectangleF ŝ,ü à){Vector2 è=base.т(ŝ,à);è.Y+=à.X(è,"H2 level: "+Ś.ToString("P1"
,ϓ)+" = "+ψ(Ш),mainColor,TextAlignment.CENTER).Y;à.k();return è;}override public void я(){ѝ();string[]Ø=ϰ.DetailedInfo.
Split('\n');int Ͼ=Ø[3].IndexOf('/')+1;int Ͽ=Ø[3].Length-2;int Ĝ=Ͽ-Ͼ;Κ[Ƣ]+=Щ=float.Parse(Ø[3].Substring(Ͼ,Ĝ));Ͼ=Ø[3].IndexOf(
':')+1;Ͽ=Ø[3].IndexOf('%');Ĝ=Ͽ-Ͼ;Ś=float.Parse(Ø[3].Substring(Ͼ,Ĝ))/100;Β[Ƣ]+=Ш;Δ[Ƣ]+=Ϛ;if(ы){ϱ[Ƣ]|=true;Γ[Ƣ]+=Ш;Ε[Ƣ]-=ϙ;ϡ[
Ƣ]-=Ϛ;ϣ[Ƣ]+=ϸ;ˈ[Ƣ]+=ϙ+ϸ;}ϲ[Ƣ]=Β[Ƣ]/Κ[Ƣ];}public override Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê){є();if(Ȅ==colorOn&&Ś<
0.002)Ȅ=colorFull;int ϼ=5;float İ=à.G;à.u(0.8f*(Ą.X/100));float Ă=à.E/2;if(à.ơ&&ѐ)à.ģ(ÿ-new Vector2(Ă),Ą.X+(2*Ă),Ą.Y+(2*Ă),
selectedColor,true);if(à.ơ&&ê!=null&&Ƙ==ê.Ƙ)à.ģ(ÿ-new Vector2(Ă*2),Ą.X+(4*Ă),Ą.Y+(4*Ă),selectionColor,false,Ă);Ă=Math.Min(Ą.X,Ą.Y)*
0.1f;if(!Ѥ){ÿ.X+=Ą.X*0.2f;Ą.X*=0.6f;ÿ.Y+=Ą.Y*0.15f;Ą.Y*=0.85f;ϼ=2;}Vector2 è=ÿ,į=ÿ+new Vector2(Ą.X,0);Vector2 ϻ=new Vector2(
è.X+(Ą.X*0.55f),è.Y+(Ą.Y*0.55f)-(à.F/2));à.ģ(è,Ą.X*0.1f,Ą.Y,Ȅ,true);è.X+=Ą.X*0.1f;è.Y+=Ą.Y*0.1f;à.ģ(è,Ą.X*0.9f,Ą.Y*0.9f,Ȅ
,false,Ă);è.X+=Ă;è.Y+=Ă;à.ģ(è,(Ą.X*0.9f)-(2*Ă),(Ą.Y*0.9f)-(2*Ă),h2Color,false,Ă);è.X+=Ă;è.Y+=Ă;à.ģ(è,((Ą.X*0.9f)-(4*Ă))*Ś
,(Ą.Y*0.9f)-(4*Ă),h2Color,true);Vector2 Ϻ=new Vector2(Ą.X*0.9f/((ϼ*2)+1),Ą.Y*0.1f);è=į;for(int Ͻ=0;Ͻ<ϼ;Ͻ++){è.X-=Ϻ.X;à.ģ(
è,-Ϻ.X,Ϻ.Y,Ȅ,true);è.X-=Ϻ.X;}if(!ϔ)à.X(ϻ,(Ƣ+1).ToString(),mainColor,TextAlignment.CENTER);à.u(İ);if(ѡ)à.Ġ(ÿ+new Vector2(Ą
.X/2,Ą.Y/2),Ą.Y*0.8f,"Danger",mainColor);return new Vector2(Ą.X,Ą.Y);}}class ϯ:Ѩ{public const float Ĭ=1/1.1618f;const
float Ÿ=0.05f;public static float[]Ε=new float[maxCS];public static float[]ϡ=new float[maxCS];public static float[]Δ=new
float[maxCS];public static float[]ˈ=new float[maxCS];public static bool[]Ϡ=new bool[maxCS];public static Queue<float[]>ϟ=new
Queue<float[]>(outputDiagramDepth);public static Queue<float[]>Ϟ=new Queue<float[]>(outputDiagramDepth);public static int[]ϝ=
new int[maxCS+1];const int Ϝ=100000;IMyThrust ϖ;public float ϛ{get{return ϖ.CurrentThrust/ϖ.MaxThrust;}}public float Ϛ{get;
}public float ϙ{get{return Ϛ*ϛ;}}public float Ϙ{get{return ϖ.ThrustOverridePercentage;}set{ϖ.ThrustOverridePercentage=
Math.Max(Math.Min(value,1),0);}}Vector3I ϗ;public override bool г{get{return false;}set{if(value)Ϙ-=Ÿ;}}public override bool
в{get{return false;}set{if(value)Ϙ+=Ÿ;}}public static void Ľ(){φ(Ε);φ(ϡ);φ(Δ);φ(ˈ);φ(Ϡ);}public static void ř()=>ϝ=Τ(Ε,ϡ,
ϝ,Ϝ,ϟ,Ϟ);public ϯ(IMyThrust ϖ):base(ϖ,1){this.ϖ=ϖ;if(Ѥ){ѣ=(Ѣ.X*Ѣ.Y*Ѣ.Z*Math.Pow(ѥ,3))>370f;if(ѣ)Ϛ=4820*60;else Ϛ=803*60;}
else{ѣ=(Ѣ.X*Ѣ.Y*Ѣ.Z*Math.Pow(ѥ,3))>3f;if(ѣ)Ϛ=386*60;else Ϛ=80*60;}ϗ=new Vector3I(0);for(int ŕ=0;ŕ<maxCS+1;ŕ++)ϝ[ŕ]=Ϝ;}
override public Vector2 т(RectangleF ŝ,ü à){Vector2 è=base.т(ŝ,à);string Ϯ=ʦ("disabled");if(Ϙ>0.001f)Ϯ=Ϙ.ToString("P0",ϓ);if(à.O
){è.Y+=à.X(è,ʦ("Override")+": "+Ϯ+", "+ϛ.ToString("P1",ϓ)+" = "+ψ(ϙ)+"/min",mainColor,TextAlignment.CENTER).Y;è.X=ŝ.X;à.X
(è,(-Ÿ).ToString("P0",ϓ)+" "+ʦ("thrust"),mainColor,TextAlignment.LEFT);è.X=ŝ.Right;à.X(è,"+"+Ÿ.ToString("P0",ϓ)+" "+ʦ(
"thrust"),mainColor,TextAlignment.RIGHT);}else{è.Y+=à.X(è,ʦ("Override")+": "+Ϯ+", "+ʦ("Throttle")+": "+ϛ.ToString("P1",ϓ)+" = "+
ʦ("Consumption")+": "+ψ(ϙ)+"/min",mainColor,TextAlignment.CENTER).Y;è.X=ŝ.X;à.X(è,ʦ("Primary")+": "+(-Ÿ).ToString("P0",ϓ)
+" "+ʦ("thrust")+" "+ʦ("override"),mainColor,TextAlignment.LEFT);è.X=ŝ.Right;à.X(è,ʦ("Secondary")+": +"+Ÿ.ToString("P0",ϓ
)+" "+ʦ("thrust")+" "+ʦ("override"),mainColor,TextAlignment.RIGHT);}à.k();return è;}public void ϭ(){if(!ы&&ϖ.
ThrustOverridePercentage>0){float Ϭ=ϖ.ThrustOverridePercentage;ϖ.ThrustOverride=0;ϖ.ThrustOverridePercentage=Ϭ;}if(!ϖ.GridThrustDirection.Equals
(new Vector3I(0)))ϗ=ϖ.GridThrustDirection;}public override void я(){ѝ();float ϫ=ϖ.ThrustOverridePercentage;if(ы){Ϡ[Ƣ]|=
true;Ε[Ƣ]-=ϙ;ˈ[Ƣ]+=ϙ;ϡ[Ƣ]-=Ϛ;}else ϖ.ThrustOverridePercentage=0;Δ[Ƣ]+=Ϛ;ϖ.ThrustOverridePercentage=ϫ;}public override
Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê){є();float Ă=à.E/2;float İ=à.G;à.u(0.8f*(Ą.X/100));if(à.ơ&&ê!=null&&Ƙ==ê.Ƙ)à.ģ(ÿ-new
Vector2(Ă*2),Ą.X+(4*Ă),Ą.Y+(4*Ă),selectionColor,false,Ă);if(à.ơ&&ѐ)à.ģ(ÿ-new Vector2(Ă),Ą.X+(2*Ă),Ą.Y+(2*Ă),selectedColor,true)
;Ă=Math.Min(Ą.X,Ą.Y)*0.05f;Vector2 Ϫ=new Vector2(ÿ.X+(Ą.X/2),ÿ.Y);if(!Ѥ){Ą.Y*=0.75f;}if(!ѣ){ÿ.X+=Ą.X*0.2f;Ą.X*=0.6f;}
Vector2 è=ÿ,į=ÿ+new Vector2(Ą.X,0),Ė=ÿ+new Vector2(0,Ą.Y);à.Ę(è,Ą.X,Ą.Y*0.2f,Ȅ);è.X=ÿ.X;è.Y=ÿ.Y+(Ą.Y*0.5f);à.Ę(è,Ą.X,-Ă,Ȅ);à.Ę(
è,Ă,-Ą.Y*0.1f,Ȅ);è.X=į.X;à.Ę(è,-Ă,-Ą.Y*0.1f,Ȅ);à.ė(ÿ+new Vector2((Ą.X*0.25f)+(Ă/2),Ą.Y*0.1f),ÿ+new Vector2(Ă/2,Ą.Y*0.4f),
Ă,Ȅ);à.ė(į+new Vector2((-Ą.X*0.25f)-(Ă/2),Ą.Y*0.1f),į+new Vector2(-Ă/2,Ą.Y*0.4f),-Ă,Ȅ);float ϩ=Ą.Y*0.5f*ϛ;è.X=ÿ.X+(Ą.X*
0.5f);è.Y=ÿ.Y+(Ą.Y*0.5f)+(ϩ/2);à.ē(new MySprite(SpriteType.TEXTURE,"Triangle",è,new Vector2((Ą.X-(2*Ă))*ϛ,ϩ),h2Color,null,
TextAlignment.CENTER,(float)Math.PI));string Ϩ="-";à.o(0.8f);if(ϗ.Z==1)Ϩ="accel";else if(ϗ.Y==-1)Ϩ="up";else if(ϗ.Z==-1)Ϩ="brake";
else if(ϗ.X==1)Ϩ="left";else if(ϗ.X==-1)Ϩ="right";else if(ϗ.Y==1)Ϩ="down";à.X(Ϫ,Ϩ,mainColor,TextAlignment.CENTER);à.o(1/0.8f
);if(ѡ)à.Ġ(ÿ+new Vector2(Ą.X/2,Ą.Y*0.25f),Math.Min(Ą.X,Ą.Y)*0.9f,"Danger",mainColor);if(!ϔ)à.X(ÿ+new Vector2(Ą.X/2,Ą.Y*
0.2f),(Ƣ+1).ToString(),mainColor,TextAlignment.CENTER);à.u(İ);return new Vector2(Ą.X,Ą.Y);}}class ϧ:ѓ{public static bool[]Ϧ=
new bool[maxCS];public static bool[]ϥ=new bool[maxCS];public static bool[]Ϥ=new bool[maxCS];public static float[]Γ=new
float[maxCS];public static float[]ϣ=new float[maxCS];public static float[]Β=new float[maxCS];public static float[]Κ=new float
[maxCS];public static float[]Ε=new float[maxCS];public static float[]ˈ=new float[maxCS];public static float[]ϲ=new float[
maxCS];public static Queue<float[]>ŧ=new Queue<float[]>(outputDiagramDepth);public static int[]ϝ=new int[maxCS+1];const int Ϝ
=10000;public static void Ľ(){φ(ϣ);φ(Β);φ(Κ);φ(ˈ);φ(Ε);φ(Γ);φ(ϲ);φ(Ϧ);φ(ϥ);φ(Ϥ);}public static void ř()=>ϝ=Φ(Ε,ϝ,Ϝ,ŧ);
public ϧ(IMyGasTank ˎ):base(ˎ){ȅ=h2Color;for(int ŕ=0;ŕ<maxCS+1;ŕ++)ϝ[ŕ]=Ϝ;}public override void я(){ѝ();Ϧ[Ƣ]=true;Β[Ƣ]+=Ш;Κ[Ƣ]
+=Щ;ϲ[Ƣ]=Β[Ƣ]/Κ[Ƣ];if(Ч){Ϥ[Ƣ]|=true;ϣ[Ƣ]+=Щ-Ш;ˈ[Ƣ]+=Щ-Ш;}if(Ц){ϥ[Ƣ]|=true;Γ[Ƣ]+=Ш;}}}class Ж:ѓ{public static bool[]Ϧ=new
bool[maxCS];public static float[]Ώ=new float[maxCS];public static float[]Е=new float[maxCS];public static float[]Ύ=new float
[maxCS];public static float[]Ό=new float[maxCS];public static float[]Α=new float[maxCS];public static float[]ˇ=new float[
maxCS];public static float[]ϲ=new float[maxCS];public static bool[]Ϥ=new bool[maxCS];public static bool[]ϥ=new bool[maxCS];
public static Queue<float[]>Ŧ=new Queue<float[]>(outputDiagramDepth);public static int[]ϝ=new int[maxCS+1];const int Ϝ=10000;
public static void Ľ(){φ(Е);φ(Ύ);φ(Ό);φ(ˇ);φ(Α);φ(Ώ);φ(ϲ);φ(Ϧ);φ(Ϥ);φ(ϥ);}public static void ř()=>ϝ=Φ(Α,ϝ,Ϝ,Ŧ);public Ж(
IMyGasTank ˎ):base(ˎ){ȅ=o2Color;for(int ŕ=0;ŕ<maxCS+1;ŕ++)ϝ[ŕ]=Ϝ;}public override void я(){ѝ();Ϧ[Ƣ]=true;Ύ[Ƣ]+=Ш;Ό[Ƣ]+=Щ;ϲ[Ƣ]=Ύ[Ƣ]
/Ό[Ƣ];if(Ч){Е[Ƣ]+=Щ-Ш;ˇ[Ƣ]+=Щ-Ш;Ϥ[Ƣ]|=true;}if(Ц){Ώ[Ƣ]+=Ш;ϥ[Ƣ]|=true;}}}class М:Ѩ{public const float Ĭ=0.6f;public const
float Л=1.8f;public static float[]Ł=new float[maxCS];public static float[]Ŕ=new float[maxCS];public static float[]ũ=new float
[maxCS];public static bool[]К=new bool[maxCS];public static bool[]Й=new bool[maxCS];public static Queue<float[]>Ŧ=new
Queue<float[]>(outputDiagramDepth);public static Queue<float[]>И=new Queue<float[]>(outputDiagramDepth);public static int[]ϝ=
new int[maxCS+1];const int Ϝ=10;IMyOxygenFarm З;ȃ ʓ;public float Ũ{get{return Л*З.GetOutput();}}public override bool в{get{
return ʓ.Ǳ;}set{ʓ.Ǳ=value;}}public static void Ľ(){φ(Ł);φ(Ŕ);φ(ũ);φ(К);φ(Й);}public static void ř()=>ϝ=Τ(Ł,Ŕ,ϝ,Ϝ,Ŧ,И);public М
(IMyOxygenFarm З,ȃ ʓ=null):base(З,1){this.З=З;this.ʓ=ʓ;ѣ=true;for(int ŕ=0;ŕ<maxCS+1;ŕ++)ϝ[ŕ]=Ϝ;}override public Vector2 т
(RectangleF ŝ,ü à){Vector2 è=base.т(ŝ,à);è.X=ŝ.X;if(à.O)à.X(è,Ũ.ToString("0.00",ϓ)+" l/min",mainColor,TextAlignment.LEFT)
;else à.X(è,ʦ("O2 output")+": "+Ũ.ToString("0.00",ϓ)+" l/min",mainColor,TextAlignment.LEFT);è.X=ŝ.Right;if(alignFarms){if
(ξ)è.Y+=à.X(è,ʦ("using external alignment"),mainColor,TextAlignment.RIGHT).Y;else if(ʓ!=null)è.Y+=à.X(è,ʓ.ǥ,mainColor,
TextAlignment.RIGHT).Y;}else è.Y+=à.F;if(ο.Count>0){è.X-=à.F;à.ą(è,à.F,в);è.X-=à.E/2;if(à.O)à.X(è,ʦ("farm mast alignment"),mainColor,
TextAlignment.RIGHT);else à.X(è,ʦ("Secondary")+": "+ʦ("farm mast alignment"),mainColor,TextAlignment.RIGHT);}à.k();return è;}public
override void я(){ѝ();Й[Ƣ]=true;if(ы)Ŕ[Ƣ]+=Л;ũ[Ƣ]+=Л;}public override Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê){є();float Ă=à.E/2;à
.o(0.8f*(Ą.Y/100));if(à.ơ&&ê!=null&&Ƙ==ê.Ƙ)à.ģ(ÿ-new Vector2(Ă*2),Ą.X+(4*Ă),Ą.Y+(4*Ă),selectionColor,false,Ă);if(à.ơ&&ѐ)à
.ģ(ÿ-new Vector2(Ă),Ą.X+(2*Ă),Ą.Y+(2*Ă),selectedColor,true);Ă=Math.Min(Ą.X,Ą.X)*0.05f;Vector2 è=ÿ,į=ÿ+new Vector2(Ą.X,0);
Vector2 Ė=ÿ+new Vector2(0,Ą.Y),ġ=į+new Vector2(0,Ą.Y);è=Ė;à.ģ(è,Ą.X,-Ą.Y*0.3f,Ȅ,false,Ă);è.X+=Ą.X*0.2f;è.Y-=Ą.Y*0.3f;Vector2 ñ=
new Vector2(Ą.X*0.6f,(Ą.Y*0.7f)-Ă);à.ģ(è,ñ.X,-ñ.Y*Ũ/Л,farmColor,true);è.X-=Ă;è.Y=ÿ.Y;à.ģ(è,(Ą.X*0.6f)+(2*Ă),Ą.Y*0.7f,Ȅ,
false,Ă);if(!ϔ){è=ġ;è.X-=Ą.X*0.2f;è.Y-=Ą.Y*0.25f;à.X(è,(Ƣ+1).ToString(),mainColor,TextAlignment.RIGHT);}if(ѡ)à.Ġ(ÿ+new
Vector2(Ą.X/2,Ą.Y/2),Ą.X*0.9f,"Danger",mainColor);à.k();return new Vector2(Ą.X,Ą.Y);}}class Ј:Ѩ{public const float Ĭ=1f;static
public float Ї{get;private set;}static public List<Ј>І{get;private set;}=new List<Ј>();public static float[]Ѕ=new float[maxCS]
;public static float[]Є=new float[maxCS];public static float[]Ѓ=new float[maxCS];public static float[]Ђ=new float[maxCS];
public static float[]Ё=new float[maxCS];public static float[]Ѐ=new float[maxCS];public static bool[]ȋ=new bool[maxCS];public
static Queue<float[]>Љ=new Queue<float[]>(outputDiagramDepth);public static int[]ϝ=new int[maxCS+1];const int Ϝ=10000;
IMyAirVent ˏ;Color Д;public float Г{get{return Ж.Ώ[Ƣ]/Ж.Ό[Ƣ];}}public float В{get;}public float Б{get{return ˏ.GetOxygenLevel();}}
public bool А{get{return ˏ.CanPressurize;}}public bool Џ{get{return!ˏ.Depressurize;}set{ˏ.Depressurize=!value;}}public bool Ў{
get{return ы&&ˏ.Status==VentStatus.Pressurizing;}}public bool Ѝ{get{return ы&&ˏ.Status==VentStatus.Pressurized;}}public
bool Ќ{get{return ы&&ˏ.Status==VentStatus.Depressurizing;}}public bool Ћ{get{return ы&&ˏ.Status==VentStatus.Depressurized;}}
public static void Ľ(){φ(Ѕ);φ(Є);φ(Ѓ);φ(Ђ);φ(Ѐ);φ(ȋ);}public static void ř(){float[]Њ=new float[maxCS];for(int w=0;w<maxCS;w++
)Њ[w]=Ѕ[w]+Ѓ[w];ϝ=Φ(Њ,ϝ,Ϝ,Љ);}public Ј(IMyAirVent ˏ):base(ˏ,1,4){this.ˏ=ˏ;ѣ=false;if(Ѥ)В=300*60;else В=30*60;for(int ŕ=0;
ŕ<maxCS+1;ŕ++)ϝ[ŕ]=Ϝ;я();if(џ==0)Џ=true;}override protected bool є(){bool ȋ=ˏ.IsWorking&&!ѡ;if(ȋ){Ȅ=colorOn;if(!ˏ.
CanPressurize){if(џ==0)Ȅ=colorOn;else Ȅ=leakColor;}if(!Џ)Ȅ=depressColor;}else Ȅ=colorOff;if(ˏ.GetOxygenLevel()<0.8){if(ˏ.Depressurize
)Д=depressColor;else Д=leakColor;}else Д=colorOn;if(!ȋ)Д=colorOff;return ȋ;}override public void я(){ѝ();є();І.Clear();if
(џ==0){Ї=ˏ.GetOxygenLevel();І.Add(this);if(ы&&!Џ&&Ї>0.2)Є[Ƣ]+=В;}else if(ы){if(Џ){Ђ[Ƣ]-=В;if(Б<0.95){Ѐ[Ƣ]+=В;if(Ж.Ώ[Ƣ]>0)
{if(Ж.Ώ[Ƣ]<В)Ѓ[Ƣ]-=Ж.Ώ[Ƣ];else Ѓ[Ƣ]-=В;}}}else{Є[Ƣ]+=В;if(Б>0.05){Ё[Ƣ]+=В;if(Ж.Е[Ƣ]>0){if(Ж.Е[Ƣ]<В)Ѕ[Ƣ]+=Ж.Е[Ƣ];else Ѕ[Ƣ]
+=В;}}}ȋ[Ƣ]|=(Ў&&Б<0.95)||(Ќ&&Б>0.05);}}public override Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê){є();float Ă=à.E/2;à.o(Ą.X/
100);float ɶ=à.G;float œ=à.F;if(à.ơ&&this==à.ƛ)à.ģ(ÿ-new Vector2(Ă*2),Ą.X+(4*Ă),Ą.Y+(4*Ă),selectionColor,false,Ă);if(à.ơ&&ѐ
)à.ģ(ÿ-new Vector2(Ă),Ą.X+(2*Ă),Ą.Y+(2*Ă),selectedColor,true);Ă=Math.Min(Ą.X,Ą.Y)*0.05f;Vector2 è=ÿ,ġ=ÿ+new Vector2(Ą.X,Ą
.Y);;float ő=(Ą.X-(2*Ă))*0.6f;if(à.ƣ==LayoutTypes.AirVentsScreen){if(џ>0)à.ģ(è,Ą.X,Ą.Y,Д,false,Ă);è+=new Vector2(Ă);à.ģ(è
,Ą.X-(2*Ă),(Ą.Y-(2*Ă))*ˏ.GetOxygenLevel(),o2Color,true);è=ġ-new Vector2((Ą.X-ő)/2,0);if(!ˏ.CanPressurize){if(à.ơ&&ѐ)à.ģ(è
,-ő,-Ă,selectedColor,true);else à.ģ(è,-ő,-Ă,backgroundColor,true);}if(џ>0){è=ġ-new Vector2(Ă*2);è.Y-=œ;à.X(è,џ.ToString()
,mainColor,TextAlignment.RIGHT);}}else ő=Ą.X-(2*Ă);if(!Ѥ){ő*=0.75f;ɶ*=0.75f;}è.Y=ÿ.Y+Ă;è.X=ÿ.X+((Ą.X-ő)/2);à.ģ(è,ő,ő,Ȅ,
false,Ă);à.ģ(è+new Vector2(Ă),ő-(Ă*2),ő-(Ă*2),backgroundColor,true);à.ģ(è+new Vector2(Ă,Ă+((ő-(2*Ă))*(1-Г))),ő-(Ă*2),(ő-(Ă*2)
)*Г,o2Color,true);if(!ϔ){à.u(ɶ);à.X(è+new Vector2(Ă)," "+(Ƣ+1).ToString(),mainColor,TextAlignment.LEFT);}è+=new Vector2(ő
/2);if(!ѡ){float Ő=ő*0.2f;Vector2 ŏ=new Vector2(ő*0.22f,Ő*2);Vector2 Ŏ=è-new Vector2(0,Ő);à.ē(new MySprite(SpriteType.
TEXTURE,"Triangle",Ŏ,ŏ,Ȅ,null,TextAlignment.CENTER,(float)Math.PI));Vector2 ō=new Vector2(è.X-(float)(Math.Cos(Math.PI*2*30/360
)*Ő),è.Y+(float)(Math.Sin(Math.PI*2*30/360)*Ő));à.ē(new MySprite(SpriteType.TEXTURE,"Triangle",ō,ŏ,Ȅ,null,TextAlignment.
CENTER,(float)Math.PI/3));Vector2 Ō=new Vector2(è.X+(float)(Math.Cos(Math.PI*2*30/360)*Ő),è.Y+(float)(Math.Sin(Math.PI*2*30/
360)*Ő));à.ē(new MySprite(SpriteType.TEXTURE,"Triangle",Ō,ŏ,Ȅ,null,TextAlignment.CENTER,(float)Math.PI/-3));if((Џ&&!Ж.ϥ[Ƣ])
||(!Џ&&!Ж.Ϥ[Ƣ]))à.Ġ(è,ő*0.75f,"Danger",mainColor);}else à.Ġ(è,ő,"Danger",mainColor);à.k();return Ą;}}class ŋ:њ{public
const float Ĭ=1.1618f;public const int Ŋ=10;public const int ŉ=20;public static bool[]ň=new bool[maxCS];public static bool[]Ň
=new bool[maxCS];public static float[]ī=new float[maxCS];public static float[]Ī=new float[maxCS];public static float[]ĩ=
new float[maxCS];public static float[]ņ=new float[maxCS];public static float[]Ņ=new float[maxCS];public static float[]ń=new
float[maxCS];public static float[]Ń=new float[maxCS];public static float[]ł=new float[maxCS];public static float[]Œ=new float
[maxCS];public static float[]Ł=new float[maxCS];public static float[]Ŕ=new float[maxCS];public static float[]ũ=new float[
maxCS];public static Queue<float[]>ŧ=new Queue<float[]>(outputDiagramDepth);public static Queue<float[]>Ŧ=new Queue<float[]>(
outputDiagramDepth);public static Queue<float[]>ť=new Queue<float[]>(outputDiagramDepth);public static int[]Ť=new int[maxCS+1];const int ţ
=100000;public static int[]Ţ=new int[maxCS+1];const int š=10000;IMyGasGenerator Ŗ;public float Š{get;}public float ş{get;
}public float Ũ{get;private set;}public float Ş{get;private set;}public float Ŝ{get;set;}public float ś{get;set;}public
float Ś{get{return ц/х;}}override public bool у{get{return base.у&&Ŗ.UseConveyorSystem&&ы;}}override public bool ъ{get{return
!д&&ц>0;;}}public override bool г{get{return Ŗ.UseConveyorSystem;}set{Ŗ.UseConveyorSystem=value;}}public override bool в{
get{return Ŗ.AutoRefill;}set{Ŗ.AutoRefill=value;}}public static void Ľ(){φ(ī);φ(Ī);φ(ĩ);φ(ņ);φ(Ņ);φ(ń);φ(Ń);φ(ł);φ(Œ);φ(Ł);
φ(Ŕ);φ(ũ);Array.Clear(Ň,0,maxCS);Array.Clear(ň,0,maxCS);}public static void ř(){if(ŧ.Count>=outputDiagramDepth)ŧ.Dequeue(
);if(Ŧ.Count>=outputDiagramDepth)Ŧ.Dequeue();if(ť.Count>=outputDiagramDepth)ť.Dequeue();ŧ.Enqueue(Ń.ToArray());Ŧ.Enqueue(
Ł.ToArray());ť.Enqueue(ņ.ToArray());int[]Ř=new int[maxCS+1],ŗ=new int[maxCS+1];for(int ŕ=0;ŕ<maxCS;ŕ++){ŗ[ŕ]=(int)Math.
Ceiling((Math.Abs(Ń[ŕ])+Math.Abs(Ł[ŕ]))/ţ)*ţ;Ř[ŕ]=(int)Math.Ceiling(Math.Abs(ņ[ŕ])/š)*š;if(Ř[ŕ]>Ţ[ŕ])Ţ[ŕ]=Ř[ŕ];if(ŗ[ŕ]>Ť[ŕ])Ť[ŕ
]=ŗ[ŕ];}ŗ[maxCS]=(int)Math.Ceiling((Math.Abs(Ń.Sum())+Math.Abs(Ł.Sum()))/ţ)*ţ;Ř[maxCS]=(int)Math.Ceiling(Math.Abs(ņ.Sum()
)/š)*š;if(Ř[maxCS]>Ţ[maxCS])Ţ[maxCS]=Ř[maxCS];if(ŗ[maxCS]>Ť[maxCS])Ť[maxCS]=ŗ[maxCS];}public ŋ(IMyGasGenerator Ŗ):base(Ŗ,
1){this.Ŗ=Ŗ;ѕ=true;if(Ѥ){Š=250*60;ş=500*60;}else{Š=25*60;ş=50*60;}Ŝ=ś=0f;for(int ŕ=0;ŕ<maxCS+1;ŕ++){Ť[ŕ]=ţ;Ţ[ŕ]=š;}ı();}
override public Vector2 т(RectangleF ŝ,ü à){if(!à.P)à.o(0.65f);Vector2 è=new Vector2(ŝ.Center.X,ŝ.Y);è.Y+=à.X(è,Ы+", CS: "+(Ƣ+1)
,mainColor,TextAlignment.CENTER).Y;è.X=ŝ.X;è.Y+=à.X(è,ʦ("Ice level: ")+Ś.ToString("P1",ϓ)+" = "+ϊ(ц),mainColor,
TextAlignment.LEFT).Y;if(à.O)è.X+=à.X(è,ʦ("collecting ice"),mainColor,TextAlignment.LEFT).X+(à.E/2);else è.X+=à.X(è,ʦ("Primary")+": "
+ʦ("collecting ice"),mainColor,TextAlignment.LEFT).X+(à.E/2);à.ą(è,à.F,г);è.X=ŝ.Right-à.F;à.ą(è,à.F,в);è.X-=à.E/2;if(à.O)
à.X(è,ʦ("bottle refill"),mainColor,TextAlignment.RIGHT);else à.X(è,ʦ("Secondary")+": "+ʦ("auto.")+" "+ʦ("bottle refill"),
mainColor,TextAlignment.RIGHT);à.k();return è;}public override void я(){ѝ();if(у)δ[Ƣ].Add(this);if(ъ)γ[Ƣ].Add(this);ı();Ŝ=ś=0;ň[Ƣ
]=true;ĩ[Ƣ]+=х;Ī[Ƣ]+=ц;Œ[Ƣ]+=ş;ũ[Ƣ]+=Š;ń[Ƣ]+=(Š/Ŋ)+(ş/ŉ);if(ы){if(Ŗ.UseConveyorSystem)Ň[Ƣ]=true;ī[Ƣ]+=ц;}}public void Ĳ()
{д=false;Ŗ.UseConveyorSystem=true;}void ı(){if(ы&&ц>0.02){Ũ=Š;Ş=ş;if(ϧ.ϣ[Ƣ]+ϳ.ˈ[Ƣ]+ϯ.ˈ[Ƣ]<2)Ş=0;if(Ж.Е[Ƣ]+Ј.Ѐ[Ƣ]-Ј.Ѕ[Ƣ]<2
)Ũ=0;}else Ũ=Ş=0;}public override Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê){є();if(Ȅ==colorOn&&Ś<0.002)Ȅ=colorFull;float İ=à
.G;à.u(0.8f*(Ą.Y/100));float Ă=à.E/2;if(à.ơ&&ê!=null&&Ƙ==ê.Ƙ)à.ģ(ÿ-new Vector2(Ă*2),Ą.X+(4*Ă),Ą.Y+(4*Ă),selectionColor,
false,Ă);if(à.ơ&&ѐ)à.ģ(ÿ-new Vector2(Ă),Ą.X+(2*Ă),Ą.Y+(2*Ă),selectedColor,true);Ă=Math.Min(Ą.X,Ą.Y)*0.05f;if(!Ѥ){ÿ.X+=Ą.X*
0.1f;Ą.X*=0.8f;ÿ.Y+=Ą.Y*0.25f;Ą.Y*=0.75f;}Vector2 è=ÿ,į=ÿ+new Vector2(Ą.X,0);Vector2 Ė=ÿ+new Vector2(0,Ą.Y),ġ=į+new Vector2(
0,Ą.Y);è=Ė;à.Ę(è,Ă,-Ą.Y,Ȅ);è.Y-=Ă;à.Ę(è,Ą.X,Ă,Ȅ);è=ġ;à.Ę(è,-Ă,-Ą.Y,Ȅ);è=ÿ;è.X+=(Ą.X*0.25f)+Ă;à.Ę(è,Ă,(Ą.Y*0.2f)+Ă,Ȅ);if(Ŗ
.UseConveyorSystem)à.ģ(è+new Vector2(Ă,0),(Ą.X/2)-(Ă*4),(Ą.Y*0.2f)+Ă,iceColor,true);else à.Ę(è+new Vector2(Ă,Ą.Y*0.2f),(Ą
.X/2)-(Ă*4),Ă,Ȅ);è.X=ÿ.X+Ă;è.Y=ÿ.Y;if(Ŝ>0)à.ģ(è,Ą.X*0.25f,(Ą.Y*0.2f)+Ă,h2Color,true);è=į;è.X-=(Ą.X*0.25f)+Ă;à.Ę(è,-Ă,(Ą.Y
*0.2f)+Ă,Ȅ);if(ś>0)à.ģ(è,Ą.X*0.25f,(Ą.Y*0.2f)+Ă,o2Color,true);è.Y-=Ă;è.X-=Ă*2;if(!ϔ)à.X(è,(Ƣ+1).ToString(),mainColor,
TextAlignment.RIGHT);è.Y+=Ă;à.u(İ);if(ц>0){Vector2 Į=new Vector2(Ą.X-(Ă*2),(Ą.Y*0.8f)-(Ă*2));Į.Y*=Ś;è=new Vector2(ÿ.X+Ă,Ė.Y-Į.Y-Ă);à.
ģ(è,Į.X,Į.Y,iceColor,true);}è.X=ÿ.X+Ă+(Ą.X*0.125f);è.Y=ÿ.Y+(Ą.Y*0.525f);à.ę(è,Ą.X*0.1f,Ą.Y*0.75f,Ȅ);è.X=į.X-Ă-(Ą.X*0.125f
);à.ę(è,Ą.X*0.1f,Ą.Y*0.75f,Ȅ);if(ѡ)à.Ġ(ÿ+new Vector2(Ą.X/2,Ą.Y/2),Ą.X*0.75f,"Danger",mainColor);return new Vector2(Ą.X,Ą.
Y);}}class ĭ:њ{public const float Ĭ=1f;public static float[]ī=new float[maxCS];public static float[]Ī=new float[maxCS];
public static float[]ĩ=new float[maxCS];public static float[]Ĩ=new float[maxCS];public static float[]ħ=new float[maxCS];public
static float[]Ħ=new float[maxCS];public static float[]ĳ=new float[maxCS];enum ĥ{Ĵ,ŀ,Ŀ};IMyCargoContainer ļ;ĥ ľ;public static
void Ľ(){φ(ī);φ(Ī);φ(ĩ);φ(Ĩ);φ(ħ);φ(Ħ);φ(ĳ);}public ĭ(IMyCargoContainer ļ):base(ļ,1,2,3){this.ļ=ļ;ľ=ĥ.Ĵ;if(Ѥ){if(Ѣ.AbsMax()
>=3)ľ=ĥ.Ŀ;}else{if(Ѣ.AbsMax()>=3)ľ=ĥ.ŀ;if(Ѣ.AbsMax()>=5)ľ=ĥ.Ŀ;}}public override void я(){љ();if(Ѡ&&!ѕ&&!ļ.GetInventory(0).
IsFull)β[Ƣ].Add(this);if(Ѡ&&ѕ)α[Ƣ].Add(this);Ī[Ƣ]+=ц;ĩ[Ƣ]+=х;}public override Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê){ϵ();float
İ=à.G;à.u(0.8f*(Ą.Y/100));float Ă=à.E/2;if(à.ơ&&ê!=null&&Ƙ==ê.Ƙ)à.ģ(ÿ-new Vector2(Ă*2),Ą.X+(4*Ă),Ą.Y+(4*Ă),selectionColor
,false,Ă);if(à.ơ&&ѐ)à.ģ(ÿ-new Vector2(Ă),Ą.X+(2*Ă),Ą.Y+(2*Ă),selectedColor,true);Ă=Ą.X*0.05f;RectangleF Ļ;Vector2 è,ġ;
float ĺ;if(Ѥ)Ļ=new RectangleF(ÿ+new Vector2(Ą.X*0.1f,Ą.Y*0.1f),new Vector2(Ą.X*0.8f,Ą.Y*0.8f));else if(ľ!=ĥ.Ĵ)Ļ=new
RectangleF(ÿ+new Vector2(Ą.X*0.25f,Ą.Y*0.25f),new Vector2(Ą.X*0.5f,Ą.Y*0.5f));else Ļ=new RectangleF(ÿ+new Vector2(Ą.X*0.3f,Ą.Y*
0.3f),new Vector2(Ą.X*0.4f,Ą.Y*0.4f));è=Ļ.Position-new Vector2(Ă);ĺ=Ļ.Width+(2*Ă);ġ=è+new Vector2(ĺ);à.ģ(è,ĺ,ĺ,Ȅ,false,Ă);if
(ľ==ĥ.Ŀ){è.X-=Ă;è.Y+=Ă;à.Ę(è,Ă,ĺ-(Ă*2),Ȅ);è.X=Ļ.Right+Ă;à.Ę(è,Ă,ĺ-(Ă*2),Ȅ);è=Ļ.Position;è.Y-=Ă;à.Ę(è,ĺ-(Ă*2),-Ă,Ȅ);è.Y=Ļ.
Bottom+Ă;à.Ę(è,ĺ-(Ă*2),Ă,Ȅ);}float Ĺ=ц*ρ;float ĸ=(Ш-Ĺ)/Ѧ;è=Ļ.Position;è.Y+=Ļ.Height*(1-ĸ);à.ģ(è,Ļ.Width,Ļ.Height*ĸ,cargoColor,
true);float ķ=Ĺ/Ѧ;è.Y-=Ļ.Height*ķ;à.ģ(è,Ļ.Width,Ļ.Height*ķ,iceColor,true);if(!ϔ){float Ķ=à.F;à.X(Ļ.Center-new Vector2(0,Ķ/2)
,(Ƣ+1).ToString(),mainColor,TextAlignment.CENTER);}if(ѡ)à.Ġ(ÿ+new Vector2(Ą.X/2,Ą.Y/2),Ļ.Width,"Danger",mainColor);à.u(İ)
;return new Vector2(Ą.X,Ą.Y);}}class ĵ:њ{public static float[]ī=new float[maxCS];public static float[]Ī=new float[maxCS];
public static float[]ĩ=new float[maxCS];IMyShipConnector Ɓ;bool ƙ;public override bool г{get{return Ɓ.ThrowOut;}set{if(ƙ)Ɓ.
ThrowOut=value;}}public static void Ľ(){φ(ī);φ(Ī);φ(ĩ);}public ĵ(IMyShipConnector Ɓ):base(Ɓ,1,3){this.Ɓ=Ɓ;if(Ɓ.BlockDefinition.
SubtypeId.StartsWith("ConnectorSmall",StringComparison.Ordinal))ƙ=true;else ƙ=false;}public override void я(){љ();Ī[Ƣ]+=ц;ĩ[Ƣ]+=х
;}public override Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê)=>new Vector2(Ą.X,Ą.Y);}class Ɨ:њ{public static float[]ī=new
float[maxCS];public static float[]Ī=new float[maxCS];public static float[]ĩ=new float[maxCS];IMyCollector Ɩ;public Ɨ(
IMyCollector Ɩ):base(Ɩ,1,3){this.Ɩ=Ɩ;}public static void Ľ(){φ(ī);φ(Ī);φ(ĩ);}public override void я(){љ();Ī[Ƣ]+=ц;ĩ[Ƣ]+=х;}public
override Vector2 Ġ(Vector2 ÿ,Vector2 Ą,ü à,е ê)=>new Vector2(Ą.X,Ą.Y);}interface ƕ{string Ɣ{get;}long Ƙ{get;}Ɯ Ɠ{get;}Ŷ ƚ{get;}
int Ƭ{get;set;}void ƪ(int Û,int Ü);void Ʃ(int Û);void ƨ(int Ú);List<Ƥ>Ƨ();void Ʀ(bool Þ);void ƫ(bool V=false);}interface Ƥ{
LayoutTypes ƣ{get;}int Ƣ{get;}bool ơ{get;}void Ơ<Ɵ>(List<Ɵ>ƞ)where Ɵ:ь;void Ɲ(bool?Ŭ);}interface Ɯ:Ƥ{е ƛ{get;}void ƥ(int ƒ);void Ɖ(
);void ſ();void ž(bool Ž);void ż();void Ż();void ź();void Ź(int Ÿ,int ŷ);}interface Ŷ:Ƥ{int ŵ{get;set;}}class Ŵ:ƕ{
protected List<ScreenSetupTags>ų=new List<ScreenSetupTags>();static string[]Ų=new string[Enum.GetValues(typeof(ScreenSetupTags)).
Length];public string Ɣ{get{return ű.CustomName;}}public long Ƙ{get{return ű.EntityId;}}public Ɯ Ɠ{get{if(Ū<0)return null;else
return ů[Ū]as Ɯ;}}public Ŷ ƚ{get{if(Ƃ<0)return null;else return ů[Ƃ]as Ŷ;}}public int Ƭ{get{return Ǝ;}set{Ǝ=ç(value);}}
IMyTerminalBlock ű{get;}IMyTextSurfaceProvider Ű;Ƥ[]ů;List<IMySensorBlock>Ů=new List<IMySensorBlock>();List<IMyCockpit>ŭ=new List<
IMyCockpit>();bool?Ŭ=null;int ū=0;Ɯ[]ƀ=new Ɯ[9];int Ū=-1;int Ƃ=-1;bool Ƒ=false;bool Ə=false;int Ǝ=0;int ƍ=1;int ƌ=0;static Ŵ(){Ų[0
]=" - names of the sensors to activate this LCD, separated by ,";Ų[1]=
" - names of the cockpits used to activate this LCD, separated by ,";Ų[2]=" - true draws a chart, false draws a bar";}public Ŵ(IMyTerminalBlock ű){this.ű=ű;Ű=ű as IMyTextSurfaceProvider;ů=
new Ƥ[Ű.SurfaceCount];if(ű is IMyCockpit||ű is IMyButtonPanel)Ŭ=false;Ƌ(0,1);Ū=-1;this.ű.CustomData=ƈ();ū=this.ű.CustomData
.GetHashCode();}void Ƌ(params int[]Ɗ){foreach(var Ê in Ɗ)if(!ų.Contains((ScreenSetupTags)Ê))ų.Add((ScreenSetupTags)Ê);}
void Ɛ(ScreenSetupTags Ê)=>ų.Remove(Ê);string ƈ(){Ƒ=Ə=false;Ƃ=Ū=-1;var Ƈ=new StringBuilder();string[]Ø=ű.CustomData.Trim().
Trim('\n').Split('\n');int w,Ì=0,Ɔ=0;bool ƅ=false;while(Ɔ<Ø.Length&&!Ø[Ɔ].Trim().StartsWith("@")){Ƈ.AppendLine(Ø[Ɔ]);Ɔ++;}if
(Ɔ<Ø.Length){ƅ=Ø[Ɔ].Contains("@Room");if(!Ø[Ɔ].Contains(ΰ))Ƈ.Append(Ø[Ɔ].Trim()+", "+ΰ+"\n");else Ƈ.Append(Ø[Ɔ].Trim()+
"\n");Ɔ++;}else Ƈ.Append(ΰ+"\n");Ƈ.Append(ή+"\n");if(Ɔ<Ø.Length&&Ø[Ɔ].StartsWith(ή)){Ɔ++;for(Ì=0;Ì<Ű.SurfaceCount;Ì++){if(Ɔ<
Ø.Length&&!Ø[Ɔ].StartsWith(έ)){if(Ø[Ɔ].StartsWith(Ű.GetSurface(Ì).DisplayName))Ƈ.Append(Ñ(Ì,Ø[Ɔ].Split('=')[1])+"\n");
else Ƈ.Append(Í(Ì)+"\n");}else break;Ɔ++;}}for(;Ì<Ű.SurfaceCount;Ì++)Ƈ.Append(Í(Ì)+"\n");Ɔ++;Ƈ.Append(έ+"\n"+η+"\n");List<
ScreenSetupTags>Ƅ=ų.ToList();string[]ƃ;if(Ɔ<Ø.Length&&Ø[Ɔ].StartsWith(η)){Ɔ++;while(Ɔ<Ø.Length&&!Ø[Ɔ].StartsWith(σ)){if(Ø[Ɔ].Contains(
'=')){ƃ=Ø[Ɔ].Split('=');try{w=Ƅ.IndexOf((ScreenSetupTags)Enum.Parse(typeof(ScreenSetupTags),ƃ[0]));}catch{w=-1;}if(w>=0){Ƈ.
Append(Ë(Ƅ[w],ƃ[1])+"\n");Ƅ.RemoveAt(w);}else Ƈ.Append(Ø[Ɔ]+"\n");}Ɔ++;}Ɔ++;}if(Ƅ.Count>0)foreach(var Ê in Ƅ)Ƈ.Append(Ê+Ã(Ê)+
"\n");Ƈ.AppendLine(σ+"\n"+ϋ+"\n--- setup section info ---");foreach(var Ê in ų)Ƈ.AppendLine(Ê+Ų[(int)Ê]);Ƈ.AppendLine(
"--- available screens ---");foreach(string Ĥ in Enum.GetNames(typeof(LayoutTypes)))Ƈ.AppendLine(Ĥ);if(ƅ)foreach(string Ĥ in roomScreens)Ƈ.
AppendLine(Ĥ);Ƈ.AppendLine(ϕ);bool Y=true;while(Ɔ<Ø.Length){Y&=!Ø[Ɔ].StartsWith("[readme");if(Y)Ƈ.Append(Ø[Ɔ]+"\n");Y|=Ø[Ɔ].
StartsWith("[/readme");Ɔ++;}return Ƈ.ToString().Trim('\n').Trim();}string Ñ(int Ì,string É){string Â=Ű.GetSurface(Ì).DisplayName+
"=";LayoutTypes Ð;int Ï;string[]Î=É.Split(',');try{Ï=int.Parse(Î[1])-1;}catch{Ï=-1;}try{if(Enum.TryParse(Î[0],out Ð)){ů[Ì]=
Ó(Ì,Ð,Ï);if((int)ů[Ì].ƣ==10){Ə=true;Ƃ=Ì;}if(ů[Ì].ơ){Ƒ=true;Â+=(LayoutTypes)0;}else{if(Ï>=0)Â+=ů[Ì].ƣ+", "+(Ï+1);else Â+=ů
[Ì].ƣ;}}else throw new System.InvalidCastException();}catch{ů[Ì]=null;Â+=É;}return Â;}string Í(int Ì){ů[Ì]=Ó(Ì,
LayoutTypes.NONE);return Ű.GetSurface(Ì).DisplayName+"="+ů[Ì].ƣ;}string Ë(ScreenSetupTags Ê,string É){string Â=Ê+"=";string[]È;
switch((int)Ê){case 0:Ů.Clear();È=É.Split(',');try{foreach(string Ç in È.Where(Å=>Å.Length>0))Ů.Add(ʹ.First(U=>U.CustomName.
Equals(Ç.Trim())));Â+=É;}catch{ͱ.с("Sensor for "+ű.CustomName+
" missing\nsensor detection disabled\nPlease check names and recompile!");}break;case 1:ŭ.Clear();È=É.Split(',');try{foreach(string Æ in È.Where(Å=>Å.Length>0))ŭ.Add(ͳ.First(Ä=>Ä.CustomName.
Equals(Æ.Trim())));Â+=É;}catch{ͱ.с("Cockpit for "+ű.CustomName+
" missing\ncockpit detection disabled\nPlease check names and recompile!");}break;case 2:try{Ŭ=Boolean.Parse(É);}catch{Ŭ=!(ű is IMyCockpit||ű is IMyButtonPanel);}Â+=Ŭ.ToString();break;}return Â
;}string Ã(ScreenSetupTags Ê){string Â="=";switch((int)Ê){case 0:Ů.Clear();break;case 1:ŭ.Clear();break;case 2:Â+=Ŭ.
ToString();break;}return Â;}Ƥ Ó(int Ì,LayoutTypes æ=LayoutTypes.NONE,int Ï=-1){bool å=false;IMyTextSurface R=Ű.GetSurface(Ì);Ƥ ä
;switch((int)æ){case 0:if(!Ƒ){Ū=Ì;ƀ=ã(Ì,Ï);ä=ƀ[Ǝ];ƌ=1;}else{ͱ.с("a controllable screen allready exist\n"+ű.CustomName+
"- Screen: "+Ū);ä=new Ȟ(R);}break;case 1:ä=new ɢ(R,Ï);å=true;break;case 2:ä=new ɜ(R,Ï);å=true;break;case 3:ä=new ɞ(R,Ï);å=true;break
;case 4:ä=new ǐ(R,Ï);å=true;break;case 5:ä=new ɠ(R,Ï);break;case 6:ä=new ɡ(R,Ï);å=true;break;case 7:ä=new ɝ(R,Ï);å=true;
break;case 8:ä=new Ǐ(R,Ï);å=true;break;case 9:ä=new Ǔ(R,Ï);break;case 10:if(!Ə){Ƃ=Ì;ä=new ǚ(R,ƍ);}else{ͱ.с(
"a instructions screen allready exist\n"+ű.CustomName+"- Screen: "+Ū);ä=new Ȟ(R);}break;case 11:ä=new ǜ(R);break;case 12:ä=new Ȟ(R);break;default:ä=null;break;}
if(å)Ƌ(2);else Ɛ(ScreenSetupTags.DrawChart);return ä;}Ɯ[]ã(int â,int Ý){Ɯ[]á=new Ɯ[9];IMyTextSurface à=Ű.GetSurface(â);á[0
]=new ƻ(à);á[1]=new ɢ(à,Ý,true);á[2]=new ɜ(à,Ý,true);á[3]=new ɞ(à,Ý,true);á[4]=new ǐ(à,Ý,true);á[5]=new ɠ(à,Ý,true);á[6]=
new ɡ(à,Ý,true);á[7]=new ɝ(à,Ý,true);return á;}int ç(int ß){Ǝ=ß;if(Ǝ<0)Ǝ=7;if(Ǝ>7)Ǝ=0;if(Ū>=0){int Ý=ů[Ū].Ƣ;;ů[Ū]=ƀ[Ǝ];}
return Ǝ;}public void ƪ(int Û,int Ü){try{throw new System.FormatException();}catch{};if(Ə)Ʃ(Û);ƨ(Ü);}public void Ʃ(int Û){try{
(ů[Ƃ]as Ŷ).ŵ=ƍ=Û;}catch{ͱ.с("no InstructionScreen found\ncheck ~ BlockName of the command");}}public void ƨ(int Ú){if(Ú>0
&&ƌ!=Ú){var Ù=new StringBuilder();string[]Ø=ű.CustomData.Split('\n');int w=0;while(w<Ø.Length&&!Ø[w].Contains(ΰ)){Ù.Append
(Ø[w]+"\n");w++;}while(w<Ø.Length&&!Ø[w].StartsWith(έ)){if(Ø[w].Contains(ΰ)&&!Ø[w].Contains("@"+(ControlProgram)Ú+
"Control"))Ù.Append(Ø[w]+", @"+(ControlProgram)Ú+"Control\n");else if(Ø[w].Trim().EndsWith("ControlScreen"))Ù.Append(Ø[w].Split(
'=')[0]+"="+(ControlProgram)Ú+"ControlScreen"+"\n");else if(Ø[w].Trim().EndsWith("ProgramScreen"))Ù.Append(Ø[w].Split('=')[
0]+"="+(ControlProgram)Ú+"ProgramScreen"+"\n");else Ù.Append(Ø[w]+"\n");w++;}while(w<Ø.Length){Ù.Append(Ø[w]+"\n");w++;}ű
.CustomData=Ù.ToString().TrimStart('\n').TrimEnd('\n');ƌ=Ú;}}bool Ö(){if(Ů.Count==0&&ŭ.Count==0)return true;try{return Ů.
Any(Õ=>Õ.IsActive)||ŭ.Any(Ô=>Ô.IsUnderControl);}catch{ͱ.с("Sensor or cockpit not found at "+ű.CustomName+
"\nSensor and cockpit detection disabled\nCheck names and recompile!");return true;}}public void Ʀ(bool Þ){if(Þ){for(int w=0;w<Ű.SurfaceCount;w++){Ű.GetSurface(w).ContentType=ContentType.
NONE;Ű.GetSurface(w).WriteText("");}}else{for(int w=0;w<ů.Length;w++){if(ů[w]!=null&&ů[w].ƣ!=LayoutTypes.NONE){Ű.GetSurface(
w).ContentType=ContentType.NONE;Ű.GetSurface(w).WriteText("");}}}}public List<Ƥ>Ƨ(){if(ű.CustomData.GetHashCode()!=ū){ű.
CustomData=ƈ();ū=ű.CustomData.GetHashCode();}return ů.ToList();}public void ƫ(bool V=false){if(V||Ö())foreach(var U in ů.Where(U=>
U!=null))U.Ɲ(Ŭ);}}abstract class S{public IMyTextSurface R;protected RectangleF Q;public bool P{get;private set;}public
bool O{get;private set;}Vector2 N=new Vector2(512,512);Vector2 M;protected float L,K,J;public string I{get;private set;}
protected float H;public float G{get;private set;}public float F{get;private set;}public float E{get;}MySpriteDrawFrame D;List<
MySprite>C=new List<MySprite>();Color B;MySprite W;protected Vector2 A;public S(IMyTextSurface R,Color h){this.R=R;M=new Vector2
((Math.Max(R.TextureSize.X,R.SurfaceSize.X)-Math.Min(R.TextureSize.X,R.SurfaceSize.X))/2,(Math.Max(R.TextureSize.Y,R.
SurfaceSize.Y)-Math.Min(R.TextureSize.Y,R.SurfaceSize.Y))/2);Q=new RectangleF(M,new Vector2(Math.Min(R.TextureSize.X,R.SurfaceSize.
X),Math.Min(R.TextureSize.Y,R.SurfaceSize.Y)));P=Q.Height/Q.Width<0.65f;O=Q.Height<=256;E=Math.Min(Q.Width,Q.Height)/50f;
L=M.X+E;K=M.X+Q.Width-E;J=M.X+(Q.Width/2);float Á=Q.Width/N.X,À=Q.Height/N.Y;u("Debug",Math.Min(Á,À));H=G;j(h);º();}
protected void º(){D=R.DrawFrame();C.Clear();R.WriteText("");R.ContentType=ContentType.SCRIPT;R.Script="";A=M;k();}protected void
µ()=>R.ContentType=ContentType.NONE;public void ª(){D.Add(W);C.ForEach(U=>D.Add(U));D.Dispose();}public void u(string z,
float q){I=z;G=q;G=Math.Max(G,0.45f);F=R.MeasureStringInPixels(new StringBuilder("0"),I,G).Y;}public void u(string z)=>u(z,G)
;public void u(float q)=>u(I,q);public void o(float n)=>u(I,G*n);public void k()=>u(H);protected void j(Color h){B=h;W=
new MySprite(SpriteType.TEXTURE,"SquareSimple",Q.Center,Q.Size,B);}protected void d(float a,float Z)=>A+=new Vector2(a,Z);
public Vector2 X(Vector2 è,string ý,Color ĕ,TextAlignment Ě){C.Add(new MySprite(SpriteType.TEXT,ý,è,null,ĕ,I,Ě,G));return R.
MeasureStringInPixels(new StringBuilder(ý),I,G);}public void ě(string ý,Color ĕ,TextAlignment Ě)=>A.Y+=X(A,ý,ĕ,Ě).Y;public Vector2 ę(Vector2
è,float a,float Z,Color ĕ){C.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",è,new Vector2(a,Z),ĕ,null,TextAlignment.
CENTER));return new Vector2(a,Z);}public Vector2 Ę(Vector2 è,float a,float Z,Color ĕ)=>ę(new Vector2(è.X+(a/2),è.Y+(Z/2)),a,Z,
ĕ);public void ė(Vector2 ÿ,Vector2 Ė,float Ă,Color ĕ){double Ĕ=Math.Atan((ÿ.X-Ė.X)/(Ė.Y-ÿ.Y));float Ĝ=(float)((Ė.Y-ÿ.Y)/
Math.Cos(Ĕ));Vector2 è=new Vector2(Ė.X+((ÿ.X-Ė.X)/2),ÿ.Y+((Ė.Y-ÿ.Y)/2));C.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple"
,è,new Vector2(Ĝ+Math.Abs(Ă),Ă),ĕ,null,TextAlignment.CENTER,(float)(Ĕ+(Math.PI/2))));}protected Vector2 ĝ(Vector2 è,float
a,float Z,Color ĕ,bool Ģ,params float[]Ă){if(Ģ)C.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",è,new Vector2(a,Z),ĕ,
null,TextAlignment.CENTER));else{Vector2 ÿ=new Vector2(è.X-(a/2),è.Y-(Z/2));ģ(ÿ,a,Z,ĕ,Ģ,Ă);}return new Vector2(a,Z);}public
void ģ(Vector2 è,float a,float Z,Color ĕ,bool Ģ,params float[]Ă){if(Ģ)ĝ(new Vector2(è.X+(a/2),è.Y+(Z/2)),a,Z,ĕ,Ģ);else{
Vector2 ġ=new Vector2(è.X+a,è.Y+Z);Ę(è,Ă[0],Z,ĕ);Ę(è,a,Ă[0],ĕ);Ę(è+new Vector2(a-Ă[0],0),Ă[0],Z,ĕ);Ę(è+new Vector2(0,Z-Ă[0]),a,
Ă[0],ĕ);}}public void Ġ(Vector2 è,float Ą,string Ğ,Color ĕ)=>C.Add(new MySprite(SpriteType.TEXTURE,Ğ,è,new Vector2(Ą),ĕ,
null,TextAlignment.CENTER));protected void ğ(Vector2 è,float Ą,string Ğ,Color ĕ)=>Ġ(new Vector2(è.X+(Ą/2),è.Y+(Ą/2)),Ą,Ğ,ĕ);
public void ē(MySprite ć)=>C.Add(ć);}abstract class ü:S,Ƥ,Ɯ{public LayoutTypes ƣ{get;}public bool ơ{get;}protected RectangleF
û,ú,ù,ø,ö,õ,ô;protected bool ó=false;protected Vector2 ò;protected Vector2 ñ;protected Vector2 ð;protected int Ø=0,ï=0;
protected string î;protected string í=ʦ(" Status");const float ì=1.15f;public int Ƣ{get;private set;}protected List<ь>ë=new List<
ь>();public е ƛ{get{return ɴ();}}protected е ê;public ü(IMyTextSurface R,int Ï,int é,string î,bool Ē=false):base(R,
backgroundColor){if(Ï>=maxCS)Ƣ=-1;else Ƣ=Ï;ƣ=(LayoutTypes)é;ơ=Ē;this.î=î;if(ơ)í=ʦ(" Control");ò=R.MeasureStringInPixels(new
StringBuilder("000,000.0 m³"),I,G);ñ.X=Q.Width-ò.X-(2*E);ñ.Y=ò.Y;û.Position=new Vector2(L,Q.Y);û.Size=new Vector2(Q.Width-(2*E),0);}
protected void Đ(string ď){if(Ƣ>=0)ď=ď+", CS: "+(Ƣ+1);o(ì);A=new Vector2(J,Q.Y);ě(ď,mainColor,TextAlignment.CENTER);k();A.Y+=Ď();
A.X=L;û.Height=A.Y-Q.Y;}protected float Ď(){ę(new Vector2(Q.Center.X,A.Y+(E/2)),Q.Width-(E*2),E/2,mainColor);return E;}
protected void č(float Č,Vector2 Ą,Color ċ,Func<float,string>Ċ,float ĉ,float Ĉ,float đ){Vector2 è=new Vector2(L+Č,A.Y);float Ă=Ą.
Y*0.1f;ģ(è,Ą.X,Ą.Y,mainColor,false,Ă);è+=new Vector2(Ă);Ą.X-=2*Ă;Ą.Y-=2*Ă;float Ć;Ć=Ĉ/ĉ;if(Ć>=0)Ę(è,Ą.X*Ć,Ą.Y,ċ);else Ę(è
+new Vector2(0,Ą.Y/3),Ą.X*-Ć,Ą.Y/3,ċ);Ć=đ/ĉ;Ę(è+new Vector2(Ą.X*Math.Abs(Ć),0),Ă/2,Ą.Y,mainColor);A.Y+=Ą.Y+Ă+(E/2);A.X=L;
}public Vector2 ą(Vector2 ÿ,float Ą,bool ă){float Ă=Ą*0.1f;ģ(ÿ,Ą,Ą,mainColor,false,Ă);Color ā=colorOff;if(ă)ģ(ÿ+new
Vector2(2*Ă),Ą*0.6f,Ą*0.6f,colorOn,true);else ğ(ÿ+new Vector2(2*Ă),Ą*0.6f,"Cross",colorOff);return new Vector2(Ą);}protected
void Ā(Vector2 ÿ,float Ò,float þ,float ƭ,Color ĕ,Func<float,string>Ċ,string Ğ){Vector2 Ą=new Vector2(ñ.Y);Vector2 è=ÿ;ğ(è,Ą.
X,Ğ,ĕ);è.X+=Ą.X;A=è;Ą.X=ñ.X-Ą.X;č(è.X,Ą,ĕ,Ċ,ƭ,þ,Ò);o(0.75f);X(new Vector2(è.X+(ñ.X/2),è.Y+(ñ.Y*0.1f)),Ċ(Ò)+" / "+Ċ(ƭ),
mainColor,TextAlignment.CENTER);k();è.X=K;è.Y+=X(è,Ċ(þ),mainColor,TextAlignment.RIGHT).Y;è.X=L;A=è;}protected void Ɏ(float Č,
Vector2 Ą,Color ċ,float ɍ,float ĉ,float Ĉ){Vector2 è=new Vector2(L+Č,A.Y);float Ă=Ą.Y*0.1f;ģ(è,Ą.X,Ą.Y,mainColor,false,Ă);è+=
new Vector2(Ă);Ą.X-=2*Ă;Ą.Y-=2*Ă;è.X+=Ą.X*(Math.Abs(ɍ)/(Math.Abs(ɍ)+ĉ));float Ć=Ĉ/(Math.Abs(ɍ)+ĉ);if(Ĉ<0)ģ(è,Ą.X*Ć,Ą.Y,ċ,
true);if(Ĉ>0)ģ(è,Ą.X*Ć,Ą.Y,ċ,true);Ę(è,Ă,Ą.Y,mainColor);A.Y+=Ą.Y+Ă+E;A.X=L;}protected void Ɍ(Color ĕ,float ɋ,float Ɋ,float ɉ
,Func<float,string>Ċ,bool Ɉ=false,string Ğ="",float ð=0f){if(Ɉ&&Ğ.Length>0&&ð>0){o(0.75f);X(A,Ċ(ɋ),mainColor,
TextAlignment.LEFT);A.X+=ñ.X/2;X(A,Ċ(Ɋ),mainColor,TextAlignment.CENTER);A.X+=ñ.X/2;X(A,Ċ(ɉ),mainColor,TextAlignment.RIGHT);A.Y+=X(A,
"/min",mainColor,TextAlignment.LEFT).Y+(E/2);A.X=K-ð;ğ(A,ð,Ğ,ĕ);k();}else{X(A,Ċ(ɋ),mainColor,TextAlignment.LEFT);A.X+=ñ.X;A.Y
+=X(A,Ċ(ɉ),mainColor,TextAlignment.RIGHT).Y;A.X=K;X(A,Ċ(Ɋ)+"/min",mainColor,TextAlignment.RIGHT);}A.X=L;Ɏ(0,ñ,ĕ,ɋ,ɉ,Ɋ);}
protected void ɏ(Queue<float[]>ɛ,Queue<float[]>ɚ,int ŕ,int[]ə,Color ĕ,bool ɘ,bool ɗ,Func<float,string>Ċ){if(O)return;int ɖ;if(ŕ>=
0)ɖ=ə[ŕ];else ɖ=ə[maxCS];float Ă=E/4;Vector2 ÿ=ø.Position,į=new Vector2(ø.Right,ø.Y);Vector2 Ȣ=į+new Vector2(0,ø.Height/2
),ġ=new Vector2(ø.Right,ø.Bottom);float ȡ=ø.Width-ò.X;Vector2 è;è=ÿ;è.X+=ȡ;Ę(è,Ă,ø.Height,mainColor);è=new Vector2(ÿ.X,Ȣ.
Y-(Ă/2));float Ƀ=Ȣ.Y,Ȱ=ȡ/outputDiagramDepth,Ƞ=(ø.Height-Ă)/2,ɔ;if(ɘ){X(į,Ċ(ɖ),mainColor,TextAlignment.RIGHT);if(!ɗ){X(ġ-
new Vector2(0,F),Ċ(0),mainColor,TextAlignment.RIGHT);è=new Vector2(ÿ.X,ġ.Y-Ă);Ƀ=ġ.Y-(Ă/2);Ƞ*=2;}}if(ɗ){X(ġ-new Vector2(0,ò.
Y),Ċ(-ɖ),mainColor,TextAlignment.RIGHT);if(!ɘ){è=new Vector2(ÿ.X,į.Y);Ƀ=į.Y+(Ă/2);Ƞ*=2;}}Ę(è,ȡ,Ă,mainColor);è.X=ÿ.X+ȡ-(ɛ.
Count*Ȱ);IEnumerator<float[]>ɓ=ɚ.GetEnumerator();float ɒ=0f,ɑ=0f,ɐ,ɕ;foreach(var ɇ in ɛ){if(ŕ>=0)ɐ=ɇ[ŕ];else ɐ=ɇ.Sum();ɔ=Math
.Abs(ɐ*(Ƞ/ɖ));if(ɐ>0){è.Y=Ƀ-(Ă/2);ģ(è,Ȱ,-ɔ,ĕ,true);}if(ɐ<0){è.Y=Ƀ+(Ă/2);ģ(è,Ȱ,ɔ,ĕ,true);}if(ɚ.Any()){ɓ.MoveNext();if(ŕ>=0
)ɕ=ɓ.Current[ŕ];else ɕ=ɓ.Current.Sum();ɔ=Math.Abs(ɕ*(Ƞ/ɖ));if(ɕ>0){è.Y=Ƀ-(Ă/2)-ɔ;Ę(è,Ȱ,-Ă,ĕ);}if(ɕ<0){è.Y=Ƀ+(Ă/2)+ɔ;Ę(è,Ȱ
,Ă,ĕ);}ɑ=ɕ;}ɒ=ɐ;è.X+=Ȱ;}if(ɚ.Any()){Vector2 ȹ=R.MeasureStringInPixels(new StringBuilder(Ċ(ɑ)),I,G);X(Ȣ-new Vector2(0,(Ă*2
)+ȹ.Y),Ċ(ɑ),ĕ,TextAlignment.RIGHT);Ę(Ȣ-new Vector2(ȹ.X,Ă/2),ȹ.X,Ă*2,ĕ);}X(Ȣ+new Vector2(0,Ă*2),Ċ(ɒ),ĕ,TextAlignment.RIGHT
);X(ø.Position,"/min",mainColor,TextAlignment.LEFT);}protected void ȯ(Queue<float[]>Ȯ,Color ȭ,Queue<float[]>Ȭ,Color ȫ,
Queue<float[]>Ȫ,Color ȩ,int ŕ,int[]Ȩ,int[]ȧ,Func<float,string>Ȧ,Func<float,string>ȥ){if(O)return;int Ȥ,ȣ;if(ŕ>=0){Ȥ=Ȩ[ŕ];ȣ=ȧ[
ŕ];}else{Ȥ=Ȩ[maxCS];ȣ=ȧ[maxCS];}float Ă=E/4;Vector2 ÿ=ø.Position,į=new Vector2(ø.Right,ø.Y);Vector2 Ȣ=į+new Vector2(0,ø.
Height/2),ġ=new Vector2(ø.Right,ø.Bottom);float ȡ=ø.Width-ò.X;Vector2 è=new Vector2(ÿ.X,Ȣ.Y-(Ă/2));Ę(è,ȡ,Ă,mainColor);è=ÿ;è.X
+=ȡ;Ę(è,Ă,ø.Height,mainColor);X(į,Ȧ(Ȥ),mainColor,TextAlignment.RIGHT);float Ȱ=ȡ/outputDiagramDepth,Ƞ,Ȳ,Ɇ,Ʉ;Ƞ=ø.Height-Ă;
float Ƀ=Ȣ.Y;Ƞ/=2;X(ġ-new Vector2(0,F),ȥ(-ȣ),mainColor,TextAlignment.RIGHT);è.X=ÿ.X+ȡ-(Ȯ.Count*Ȱ);float ɂ=0f,Ɂ=0f,ɀ=0f,ȿ;
IEnumerator<float[]>Ⱦ=Ȭ.GetEnumerator();IEnumerator<float[]>Ƚ=Ȫ.GetEnumerator();float ȼ,Ȼ,Ʌ;foreach(var Ⱥ in Ȯ){Ⱦ.MoveNext();Ƚ.
MoveNext();if(ŕ>=0){ȼ=Ⱥ[ŕ];Ȼ=Ⱦ.Current[ŕ];Ʌ=Ƚ.Current[ŕ];}else{ȼ=Ⱥ.Sum();Ȼ=Ⱦ.Current.Sum();Ʌ=Ƚ.Current.Sum();}è.Y=Ƀ-(Ă/2);Ȳ=ȼ*(Ƞ
/Ȥ);Ɇ=Ȼ*(Ƞ/Ȥ);Ʉ=Ʌ*(Ƞ/ȣ);ȿ=è.Y-Ȳ;ɂ=ȼ;Ɂ=Ȼ;ɀ=Ʌ;ģ(è,Ȱ,-Ȳ,ȭ,true);ģ(new Vector2(è.X,ȿ),Ȱ,-Ɇ,ȫ,true);è.Y=Ƀ+(Ă/2);ģ(è,Ȱ,-Ʉ,ȩ,
true);è.X+=Ȱ;}X(Ȣ-new Vector2(0,(F/2)+F),Ȧ(Ɂ),ȫ,TextAlignment.RIGHT);X(Ȣ-new Vector2(0,F/2),Ȧ(ɂ),ȭ,TextAlignment.RIGHT);X(Ȣ+
new Vector2(0,F/2),ȥ(-ɀ),ȩ,TextAlignment.RIGHT);X(ø.Position,"/min",mainColor,TextAlignment.LEFT);}float ȸ(float Ò,float ȴ,
float ȱ,float ȳ){float ȷ=ȱ+ȳ;float ȶ=0f;if(ȷ==0)return 0;if(ȷ>0){if(ȴ<0.1f)return 0;ȶ=ȴ/Math.Abs(ȷ);}if(ȷ<0){if(Ò<0.1f)return
0;ȶ=Ò/Math.Abs(ȷ);}return ȶ;}protected void ȵ(float[]Ò,float[]ȴ,float[]ȱ,float[]ȳ,int ŕ){float[]ɩ=new float[maxCS+1];if(ŕ
<0)for(int w=0;w<maxCS;w++)ɩ[w]=ȸ(Ò[w],ȴ[w],ȱ[w],ȳ[w]);else ɩ[0]=ȸ(Ò[ŕ],ȴ[ŕ],ȱ[ŕ],ȳ[ŕ]);A.X=K;X(A,χ(ɩ.Max()),mainColor,
TextAlignment.RIGHT);A.X=L;}protected void ɨ(bool ɧ,bool å){ù=ö=õ=ô=ø=ú=new RectangleF(0,0,0,0);ù=new RectangleF(new Vector2(Q.X,û.
Bottom),new Vector2(Q.Width,Q.Bottom-û.Bottom-E));if(ơ){ö.Size=new Vector2(Q.Width-(2*E),3*F);if(!P&&!O)ö.Height*=0.65f;ù.
Height-=ö.Height;ö.Position=new Vector2(L,ù.Bottom);}else{if(ɧ){ú.Position=new Vector2(L,û.Bottom+(E/2));ú.Size=new Vector2(Q.
Width-(E*2),ñ.Y);ù.Height-=ú.Height;ù.Position.Y=ú.Bottom;}if(å&&!O)ø.Size=new Vector2(Q.Width-(2*E),Q.Height*0.3f);else ø.
Size=new Vector2(Q.Width-(2*E),ñ.Y+F);ù.Height-=ø.Height;ø.Position=new Vector2(L,ù.Bottom);}ó=true;}protected void ɦ(){ù=ö=
õ=ô=ø=ú=new RectangleF(0,0,0,0);ö.Size=new Vector2(Q.Width-(2*E),F*2);if(!P&&!O)ö.Height*=0.65f;ö.Position=new Vector2(L,
Q.Bottom-ö.Height-E);ù.Size=ô.Size=new Vector2((Q.Width/2)-(1.5f*E),ö.Y-û.Bottom-(2*E));ù.Position=new Vector2(L,û.Bottom
+E);ô.Position=new Vector2(K-ô.Width,û.Bottom+E);ó=true;}virtual public void ƥ(int ƒ){if(ë.Count==0){ê=null;return;}int Ȏ
=0,ȍ;if(ê!=null&&ê is ь)Ȏ=ë.IndexOf(ê as ь);if(Ȏ<0){ê=ë.First();return;}else{ȍ=Ȏ+ƒ;while(ȍ<0||ȍ>=ë.Count){if(ȍ<0)ȍ+=ë.
Count;else ȍ-=ë.Count;}}ê=ë[ȍ];}public void Ɖ(){if(ƛ!=null)ƛ.ю();}public void ſ(){bool ɥ=ƛ.ѐ;foreach(var Ǒ in ë.Where(Ǒ=>Ǒ.ѐ
==ɥ))Ǒ.э(!ɥ);}public void ž(bool ɥ)=>ë.ForEach(Ǒ=>Ǒ.э(ɥ));public virtual void ż(){bool ɣ=!ƛ.д;if(ƛ.ѐ)foreach(var ű in ë.
Where(Ǒ=>Ǒ.ѐ))ű.д=ɣ;else ƛ.д=ɣ;}public void Ż(){bool ɤ=!ƛ.г;if(ƛ.ѐ)foreach(var ű in ë.Where(Ǒ=>Ǒ.ѐ))ű.г=ɤ;else ƛ.г=ɤ;}public
void ź(){bool ɣ=!ƛ.в;if(ƛ.ѐ)foreach(var ű in ë.Where(Ǒ=>Ǒ.ѐ))ű.в=ɣ;else ƛ.в=ɣ;}public void Ź(int Ÿ,int ŷ){int ɰ=ƛ.Ƣ+Ÿ;if(ɰ>(
ŷ-1))ɰ=0;if(ɰ<0)ɰ=ŷ-1;if(ƛ.ѐ)foreach(var ű in ë.Where(Ǒ=>Ǒ.ѐ))ű.Ƣ=ɰ;else(ƛ as ь).Ƣ=ɰ;}public virtual void Ơ<Ɵ>(List<Ɵ>ƞ)
where Ɵ:ь{if(ƞ.Count>0){if(Ƣ>=0)ë=ƞ.Where(ɭ=>ɭ.Ƣ==Ƣ).Select(ɭ=>ɭ as ь).ToList();else ë=ƞ.Select(ɭ=>ɭ as ь).ToList();}else{ë.
Clear();ê=null;}if(ë.Count>0)if(!ë.Contains(ê))ê=ë.First();}е ɴ(){if(ơ&&ê!=null)return ê;else return null;}protected void ɳ(
List<ь>ɲ,float ɱ,bool ɵ=false){if(ɲ.Count==0&&!ɵ){X(ù.Center-new Vector2(0,F),ʦ("no blocks\navailable"),warningColor,
TextAlignment.CENTER);return;}if(!ó){ě(ʦ("Symbol area calculation failed"),mainColor,TextAlignment.LEFT);return;}float ȴ=E*1.5f;Ø=0;ï
=1;while(Ø*ï<ɲ.Count){Ø++;ð.Y=(ù.Height-ȴ-(Ø*ȴ))/Ø;ð.X=ð.Y*ɱ;ï=(int)Math.Floor((ù.Width-ȴ)/(ð.X+ȴ));}Ø=(int)Math.Ceiling(
ɲ.Count/(double)ï);ï=(int)Math.Ceiling(ɲ.Count/(double)Ø);float ɯ=(ù.Width-(ð.X*ï))/(ï+1);float ɮ=(ù.Height-(ð.Y*Ø))/(Ø+1
);A=ù.Position+new Vector2(ɯ,ɮ);int Ä=0;foreach(var ɭ in ɲ){ɭ.Ġ(A,ð,this,ƛ);A.X+=ð.X+ɯ;Ä++;if(Ä>=ï){A.X=ù.X+ɯ;A.Y+=ð.Y+ɮ;
Ä=0;}}A=new Vector2(L,ù.Bottom);}protected void ɪ<Ɵ>(string ɬ,List<Ɵ>ɫ)where Ɵ:ь{A.X=J;ě(ɬ+" found: "+ɫ.Count,mainColor,
TextAlignment.CENTER);ě(ɬ+" shown: "+ë.Where(Ǒ=>Ǒ is Ɵ).ToList().Count,mainColor,TextAlignment.CENTER);}protected void ɪ(){o(0.7f);A.
X=L;ě(ͱ.н(),warningColor,TextAlignment.LEFT);ě(ͱ.й(),mainColor,TextAlignment.LEFT);k();}abstract public void Ɲ(bool?Ŭ);}
class ɢ:ü,Ƥ,Ɯ{public static bool ƺ{private get;set;}static ɢ(){ƺ=!V;}public ɢ(IMyTextSurface R,int Ï,bool Ē=false):base(R,Ï,1
,ʦ("H2/O2 Generators"),Ē){}public override void Ɲ(bool?Ŭ){bool å=!O;if(Ŭ!=null)å=(bool)Ŭ;º();if(!ƺ){ɪ("generators",Ά);ɪ()
;}else{if(Ƣ<0){ī=ŋ.ī.Sum();Ī=ŋ.Ī.Sum();ĩ=ŋ.ĩ.Sum();}else{ī=ŋ.ī[Ƣ];Ī=ŋ.Ī[Ƣ];ĩ=ŋ.ĩ[Ƣ];}Đ(î+í);if(!ó)ɨ(true,å);ɳ(ë,ŋ.Ĭ);if(!
ơ){Ā(ú.Position,ī,Ī,ĩ,iceColor,ϊ,"MyObjectBuilder_Ore/Ice");if(å)ȯ(ŋ.ŧ,h2Color,ŋ.Ŧ,o2Color,ŋ.ť,iceColor,Ƣ,ŋ.Ť,ŋ.Ţ,ψ,ϊ);
else{A=ø.Position;ȵ(Ж.Ώ,Ж.Е,ŋ.Ł,Ј.Ѓ,Ƣ);Ɍ(o2Color,ΐ,Ł+Α,ũ,ψ,false,null,0);}}else{if(ƛ!=null)ê.т(ö,this);}}ª();}}class ɝ:ü,Ƥ,Ɯ
{public static bool ƺ{private get;set;}static ɝ(){ƺ=!V;}public ɝ(IMyTextSurface R,int Ï,bool Ē=false):base(R,Ï,7,ʦ(
"Oxygen Farm"),Ē){}public override void Ɲ(bool?Ŭ){bool å=!O;if(Ŭ!=null)å=(bool)Ŭ;º();if(!ƺ){ɪ("O2 farms",ͽ);ɪ();}else{if(Ƣ<0){Ł=М.Ł.
Sum();ũ=М.ũ.Sum();}else{Ł=М.Ł[Ƣ];ũ=М.ũ[Ƣ];}Đ(î+í);if(!ó)ɨ(false,å);ɳ(ë,М.Ĭ);if(!ơ){if(å)ɏ(М.Ŧ,М.И,Ƣ,М.ϝ,o2Color,true,false,
ψ);else{A=ø.Position;ȵ(new float[maxCS],Ж.Е,М.Ł,new float[maxCS],Ƣ);Ɍ(o2Color,0,Ł,ũ,ψ,false,null,0);}}else{if(ƛ!=null)ê.т
(ö,this);}}ª();}}class ɜ:ü,Ƥ,Ɯ{public static bool ƺ{private get;set;}static ɜ(){ƺ=!V;}public ɜ(IMyTextSurface R,int Ï,
bool Ē=false):base(R,Ï,2,ʦ("Hydrogen Tank"),Ē){}public override void Ɲ(bool?Ŭ){bool å=!O;if(Ŭ!=null)å=(bool)Ŭ;º();if(!ƺ){ɪ(
"H2 tanks",ǂ);ɪ();}else{if(Ƣ<0){Γ=ϧ.Γ.Sum();Β=ϧ.Β.Sum();Κ=ϧ.Κ.Sum();Ε=ϳ.Ε.Sum()+ϯ.Ε.Sum();Δ=ϳ.Δ.Sum()+ϯ.Δ.Sum();Ń=ŋ.Ń.Sum();Œ=ŋ.Œ.
Sum();}else{Γ=ϧ.Γ[Ƣ];Β=ϧ.Β[Ƣ];Κ=ϧ.Κ[Ƣ];Ε=ϳ.Ε[Ƣ]+ϯ.Ε[Ƣ];Δ=ϳ.Δ[Ƣ]+ϯ.Δ[Ƣ];Ń=ŋ.Ń[Ƣ];Œ=ŋ.Œ[Ƣ];}Đ(î+í);if(!ó)ɨ(true,å);ɳ(ë,Ж.Ĭ);
if(!ơ){Ā(ú.Position,Γ,Β,Κ,h2Color,ψ,"IconHydrogen");if(å)ɏ(ϧ.ŧ,new Queue<float[]>(0),Ƣ,ϧ.ϝ,h2Color,true,true,ψ);else{A=ø.
Position;ȵ(ϧ.Γ,ϧ.ϣ,ŋ.Ń,Ͱ,Ƣ);Ɍ(h2Color,Δ,Ń+Ε,Œ,ψ,false,null,0);}}else{if(ƛ!=null)ê.т(ö,this);}}ª();}}class ɡ:ü,Ƥ,Ɯ{public static
bool ƺ{private get;set;}static ɡ(){ƺ=!V;}public ɡ(IMyTextSurface R,int Ï,bool Ē=false):base(R,Ï,6,ʦ("Oxygen Tank"),Ē){}
public override void Ɲ(bool?Ŭ){bool å=!O;if(Ŭ!=null)å=(bool)Ŭ;º();if(!ƺ){ɪ("O2 tanks",ǃ);ɪ();}else{if(Ƣ<0){Α=Ј.Ѓ.Sum();ΐ=Ј.Ђ.
Sum();Ł=ŋ.Ł.Sum()+М.Ł.Sum();ũ=ŋ.ũ.Sum()+М.ũ.Sum();Ώ=Ж.Ώ.Sum();Ύ=Ж.Ύ.Sum();Ό=Ж.Ό.Sum();}else{Α=Ј.Ѓ[Ƣ];ΐ=Ј.Ђ[Ƣ];Ł=ŋ.Ł[Ƣ]+М.Ł[
Ƣ];ũ=ŋ.ũ[Ƣ]+М.ũ[Ƣ];Ώ=Ж.Ώ[Ƣ];Ύ=Ж.Ύ[Ƣ];Ό=Ж.Ό[Ƣ];}Đ(î+í);if(!ó)ɨ(true,å);ɳ(ë,Ж.Ĭ);if(!ơ){Ā(ú.Position,Ώ,Ύ,Ό,o2Color,ψ,
"IconOxygen");if(å)ɏ(Ж.Ŧ,new Queue<float[]>(0),Ƣ,Ж.ϝ,o2Color,true,true,ψ);else{A=ø.Position;ȵ(Ж.Ώ,Ж.Е,ŋ.Ł,Ј.Ѓ,Ƣ);Ɍ(o2Color,ΐ,Ł+Α,ũ,ψ
,false,null,0);}}else{if(ƛ!=null)ê.т(ö,this);}}ª();}}class ɠ:ü,Ƥ,Ɯ{public static bool ƺ{private get;set;}static ɠ(){ƺ=!V;
}public ɠ(IMyTextSurface R,int Ï,bool Ē=false):base(R,Ï,5,ʦ("Ice Cargo"),Ē){}public override void Ơ<Ɵ>(List<Ɵ>ƞ){if(ε&&!ơ
)try{ƞ=ƞ.Where(ɟ=>(ɟ as њ).ч).ToList();}catch{ƞ=new List<Ɵ>();}base.Ơ<Ɵ>(ƞ);}public override void Ɲ(bool?Ŭ){º();if(!ƺ){ɪ(
"Containers",ͺ);ɪ();}else{if(Ƣ<0){ī=ĭ.ī.Sum();Ī=ĭ.Ī.Sum();ĩ=ĭ.ĩ.Sum();Ń=ŋ.Ń.Sum();Ł=ŋ.Ł.Sum();Ĩ=ĭ.Ĩ.Sum();ħ=ĭ.ħ.Sum();Ħ=ĭ.Ħ.Sum();ĳ=
ĭ.ĳ.Sum();}else{ī=ĭ.ī[Ƣ];Ī=ĭ.Ī[Ƣ];ĩ=ĭ.ĩ[Ƣ];Ń=ŋ.Ń[Ƣ];Ł=ŋ.Ł[Ƣ];Ĩ=ĭ.Ĩ[Ƣ];ħ=ĭ.ħ[Ƣ];Ħ=ĭ.Ħ[Ƣ];ĳ=ĭ.ĳ[Ƣ];}Đ(î+í);if(!ó){if(ơ)ɨ(
true,false);else{ú.Position=new Vector2(L,û.Bottom+(E/2));ú.Size=new Vector2(Q.Width-(E*2),ñ.Y);ö.Size=new Vector2(Q.Width-(
2*E),F+E);if(!O)ö.Height+=F+(2*ñ.Y)+(2*E);ö.Position=new Vector2(L,Q.Bottom-ö.Height);ù.Position=new Vector2(Q.X,ú.Bottom
);ù.Size=new Vector2(Q.Width,ö.Y-ú.Bottom);ó=true;}}ɳ(ë,ĭ.Ĭ);if(!ơ){Ā(ú.Position,ī,Ī,ĩ,iceColor,ϊ,
"MyObjectBuilder_Ore/Ice");A=new Vector2(ö.Center.X,ö.Y);if(ε)ě(ʦ("ice is automatically relocated"),colorOn,TextAlignment.CENTER);else ě(ʦ(
"no automatic relocation of items"),colorOff,TextAlignment.CENTER);if(!O){A.X=L;ě(ʦ("possible amount of gases:"),mainColor,TextAlignment.LEFT);A.Y+=E;Ā(A,
Ń,Ĩ,ħ,h2Color,ψ,"IconHydrogen");A.Y+=E;Ā(A,Ł,Ħ,ĳ,o2Color,ψ,"IconOxygen");}}else{if(ƛ!=null)ê.т(ö,this);}}ª();}}class ɞ:ü,
Ƥ,Ɯ{public static bool ƺ{private get;set;}static ɞ(){ƺ=!V;}public ɞ(IMyTextSurface R,int Ï,bool Ē=false):base(R,Ï,3,ʦ(
"Hydrogen Thrusters"),Ē){}public void ǒ(float Ÿ){if(ƛ.ѐ)foreach(var ű in ë.Where(Ǒ=>Ǒ.ѐ&&Ǒ is ϯ))(ű as ϯ).Ϙ+=Ÿ;else if(ƛ is ϯ)(ƛ as ϯ).Ϙ+=Ÿ;
}public override void Ɲ(bool?Ŭ){bool å=!O;if(Ŭ!=null)å=(bool)Ŭ;º();if(!ƺ){ɪ("H2 thruster",ͻ);ɪ();}else{if(Ƣ<0){Δ=ϯ.Δ.Sum(
);Ε=ϯ.Ε.Sum();}else{Δ=ϯ.Δ[Ƣ];Ε=ϯ.Ε[Ƣ];}Đ(î+í);if(!ó)ɨ(true,å);ɳ(ë,ϯ.Ĭ);if(!ơ){ȵ(ϧ.Γ,new float[maxCS],ŋ.Ń,Λ,Ƣ);if(å)ɏ(ϯ.ϟ,
ϯ.Ϟ,Ƣ,ϯ.ϝ,h2Color,false,true,ψ);else Ɍ(h2Color,Δ,Ε,0,ψ,false,null,0);}else{if(ƛ!=null)ê.т(ö,this);}}ª();}}class ǐ:ü,Ƥ,Ɯ{
public static bool ƺ{private get;set;}static ǐ(){ƺ=!V;}public ǐ(IMyTextSurface R,int Ï,bool Ē=false):base(R,Ï,4,ʦ(
"Hydrogen Engines"),Ē){}public override void Ɲ(bool?Ŭ){bool å=!O;if(Ŭ!=null)å=(bool)Ŭ;º();if(!ƺ){ɪ("H2 engines",ͼ);ɪ();}else{if(Ƣ<0){Ε=ϳ.Ε
.Sum();Δ=ϳ.Δ.Sum();Γ=ϳ.Γ.Sum();Β=ϳ.Β.Sum();Κ=ϳ.Κ.Sum();}else{Ε=ϳ.Ε[Ƣ];Δ=ϳ.Δ[Ƣ];Γ=ϳ.Γ[Ƣ];Β=ϳ.Β[Ƣ];Κ=ϳ.Κ[Ƣ];}Đ(î+í);if(!ó)ɨ
(true,å);ɳ(ë,ϳ.Ĭ);if(!ơ){Ā(ú.Position,Γ,Β,Κ,h2Color,ψ,"IconHydrogen");A=ø.Position;ȵ(Ι,ϳ.ϣ,ŋ.Ń,Ί,Ƣ);if(å)ɏ(ϳ.ϟ,ϳ.Ϟ,Ƣ,ϳ.ϝ,
h2Color,false,true,ψ);else Ɍ(h2Color,Δ,Ε,0,ψ,false,null,0);}else{if(ƛ!=null)ê.т(ö,this);}}ª();}}class Ǐ:ü,Ƥ,Ɯ{public static
bool ƺ{private get;set;}static Ǐ(){ƺ=!V;}public Ǐ(IMyTextSurface R,int Ï,bool Ē=false):base(R,Ï,8,ʦ("Air Vents"),Ē){}public
override void Ɲ(bool?Ŭ){bool å=!O;if(Ŭ!=null)å=(bool)Ŭ;º();if(!ƺ){ɪ("vents",Έ);ɪ();}else{if(Ƣ<0){Α=Ј.Ѓ.Sum();Ł=Ј.Ѕ.Sum();ΐ=Ј.Ђ.
Sum();ũ=Ј.Є.Sum();}else{Α=Ј.Ѓ[Ƣ];Ł=Ј.Ѕ[Ƣ];ΐ=Ј.Ђ[Ƣ];ũ=Ј.Є[Ƣ];}Đ(î+í);if(!ó)ɨ(false,å);ɳ(ë,Ј.Ĭ);if(!ơ){if(å)ɏ(Ј.Љ,new Queue<
float[]>(0),Ƣ,Ј.ϝ,o2Color,true,true,ψ);else{A=ø.Position;ȵ(Ж.Ώ,Ж.Е,Ј.Ѕ,Ј.Ѓ,Ƣ);Ɍ(o2Color,ΐ,Ł+Α,ũ,ψ,false,null,0);}}else{if(ƛ!=
null)ê.т(ö,this);}}ª();}}class ǜ:ü,Ƥ{public static bool ƺ{private get;set;}static ǜ(){ƺ=!V;}public ǜ(IMyTextSurface R,bool Ē
=false):base(R,-1,11,ʦ("H2/O2 Control Program"),Ē){u("Monospace",G);}public override void Ɲ(bool?Ŭ=false){º();Đ(î);ě(ͱ.н(
),warningColor,TextAlignment.LEFT);ě(ͱ.з(),mainColor,TextAlignment.LEFT);if(!ƺ)ɪ();ª();k();R.WriteText(ͱ.ж(),false);}}
class ǚ:ü,Ƥ,Ŷ{public static bool ƺ{private get;set;}public int ŵ{get{return Ǚ+1;}set{Ǚ=value-1;}}int Ǚ=0;RectangleF ǘ;float Ǜ
;static ǚ(){ƺ=!V;}public ǚ(IMyTextSurface R,int Ǘ):base(R,-1,10,ʦ("H2/O2 Program Manual"),false){ǘ=Q;Ǜ=1;Ǚ=Math.Min(Math.
Max(Ǘ-1,0),8);}void ǖ(){ǘ.Position=û.Position+new Vector2(0,û.Height+(E/2));ǘ.Height=Q.Height-û.Height-(1.5f*E);ǘ.Width=û.
Width;float Ǖ=(ǘ.Height-(E*1.5f))/10;Ǜ=Ǖ/F;if(G*Ǜ>1.2f)Ǜ=1.2f/G;ó=true;}public override void Ɲ(bool?Ŭ=false){º();Đ(î);if(!ó)ǖ
();if(!ƺ)ɪ();else{o(Ǜ);A=new Vector2(ǘ.Center.X,A.Y);ě("Current Tab: "+(Ǚ+1),mainColor,TextAlignment.CENTER);ě(
instructions[Ǚ,0],mainColor,TextAlignment.CENTER);A.X=ǘ.X+F;A.Y+=(ǘ.Bottom-A.Y-(8*F))/2;for(int ǔ=1;ǔ<9;ǔ++){if(instructions[Ǚ,ǔ].
Length>1){X(A,(ǔ+1)+":",mainColor,TextAlignment.RIGHT);A.Y+=X(new Vector2(A.X+(F*0.5f),A.Y),instructions[Ǚ,ǔ],mainColor,
TextAlignment.LEFT).Y;}else A.Y+=F;}}ª();}}class Ǔ:ü,Ƥ{public static bool ƺ{private get;set;}static Ǔ(){ƺ=!V;}public Ǔ(IMyTextSurface
R,int Ï,bool Ē=false):base(R,Ï,9,ʦ("H2/O2/Ice"),Ē){ð=new Vector2((ñ.Y*1.8f)+(E/2));}void Ƽ(Color ĕ,float Ò,float þ,float
ƭ,Func<float,string>Ċ){č(0,ñ,ĕ,Ċ,ƭ,þ,Ò);X(A,Ċ(Ò)+" / "+Ċ(ƭ),mainColor,TextAlignment.LEFT);A.X=K;ě(Ċ(þ),mainColor,
TextAlignment.RIGHT);A.X=L;A.Y+=E/2;}public override void Ɲ(bool?Ŭ=false){bool å=!O;if(Ŭ!=null)å=(bool)Ŭ;º();if(!ƺ)ɪ();else{if(Ƣ<0){ņ
=ŋ.ņ.Sum();ī=Ζ.Sum();ń=ŋ.ń.Sum();Ī=ŋ.Ī.Sum()+ĭ.Ī.Sum()+ĵ.Ī.Sum()+Ɨ.Ī.Sum();ĩ=ŋ.ĩ.Sum()+ĭ.ĩ.Sum()+ĵ.ĩ.Sum()+Ɨ.ĩ.Sum();Ε=Ͱ.
Sum();Δ=ϳ.Δ.Sum()+ϯ.Δ.Sum();Ń=ŋ.Ń.Sum();Œ=ŋ.Œ.Sum();Γ=ϧ.Γ.Sum();Β=ϧ.Β.Sum()+ϳ.Β.Sum();Κ=ϧ.Κ.Sum()+ϳ.Κ.Sum();Α=Ј.Ѓ.Sum();ΐ=Ј
.Ђ.Sum();Ł=Η.Sum();ũ=Ј.Є.Sum()+ŋ.ũ.Sum()+М.Ŕ.Sum();Ώ=Ж.Ώ.Sum();Ύ=Ж.Ύ.Sum();Ό=Ж.Ό.Sum();}else{ņ=ŋ.ņ[Ƣ];ī=Ζ[Ƣ];ń=ŋ.ń[Ƣ];Ī=ŋ
.Ī[Ƣ]+ĭ.Ī[Ƣ]+ĵ.Ī[Ƣ]+Ɨ.Ī[Ƣ];ĩ=ŋ.ĩ[Ƣ]+ĭ.ĩ[Ƣ]+ĵ.ĩ[Ƣ]+Ɨ.ĩ[Ƣ];Ε=Ͱ[Ƣ];Δ=ϳ.Δ[Ƣ]+ϯ.Δ[Ƣ];Ń=ŋ.Ń[Ƣ];Œ=ŋ.Œ[Ƣ];Γ=ϧ.Γ[Ƣ];Β=ϧ.Β[Ƣ]+ϳ.Β[Ƣ
];Κ=ϧ.Κ[Ƣ]+ϳ.Κ[Ƣ];Α=Ј.Ѓ[Ƣ];ΐ=Ј.Ђ[Ƣ];Ł=Η[Ƣ];ũ=Ј.Є[Ƣ]+ŋ.ũ[Ƣ]+М.Ŕ[Ƣ];Ώ=Ж.Ώ[Ƣ];Ύ=Ж.Ύ[Ƣ];Ό=Ж.Ό[Ƣ];}if(Q.Height>310)Đ(î+í);ȵ(Ζ,
new float[maxCS],new float[maxCS],ŋ.ņ,Ƣ);Ɍ(iceColor,ń,ņ,0,ϊ,true,"MyObjectBuilder_Ore/Ice",ð.X);Ƽ(iceColor,ī,Ī,ĩ,ϊ);A.Y+=Ď(
);ȵ(ϧ.Γ,Θ,ŋ.Ń,Ͱ,Ƣ);Ɍ(h2Color,Δ,Ń+Ε,Œ,ψ,true,"IconHydrogen",ð.X);Ƽ(h2Color,Γ,Β,Κ,ψ);A.Y+=Ď();ȵ(Ж.Ώ,Ж.Е,Η,Ј.Ѓ,Ƣ);Ɍ(o2Color,
ΐ,Ł+Α,ũ,ψ,true,"IconOxygen",ð.X);Ƽ(o2Color,Ώ,Ύ,Ό,ψ);A.Y+=Ď();A.X=Q.Center.X;if(Ƣ>=0)ě("Conveyor System: "+(Ƣ+1),mainColor
,TextAlignment.CENTER);}ª();}}class ƻ:ü,Ƥ,Ɯ{public static bool ƺ{private get;set;}static List<int>ƹ=new List<int>();
static int Ƹ=-1;static List<е>Ʒ=new List<е>();static bool[]ƶ=new bool[maxCS];static bool[]Ƶ=new bool[maxCS];static float[]ƴ=
new float[maxCS];static float[]Ƴ=new float[maxCS];static bool[]Ʋ=new bool[maxCS];static bool[]Ʊ=new bool[maxCS];static
float[]ư=new float[maxCS];static float[]Ư=new float[maxCS];static Action<bool>ƽ=(Ʈ)=>{Ν(Ʈ);};static Func<bool>ƿ=()=>{return μ
;};static Action<bool>ǎ=(Ʈ)=>{ε=Ʈ;};static Func<bool>Ǎ=()=>{return ε;};static Action<bool>ǌ=(Ʈ)=>{alignFarms=Ʈ;};static
Func<bool>ǋ=()=>{return alignFarms;};static Func<float,float>Ǌ=(ǆ)=>{if(ǆ>0||κ-λ>0.1)κ+=ǆ;κ=Math.Min(κ,1);κ=Math.Max(κ,0);
Array.Clear(ζ,0,maxCS);return κ;};static Func<float,float>ǉ=(ǆ)=>{if(ǆ<0||κ-λ>0.1)λ+=ǆ;λ=Math.Min(λ,1);λ=Math.Max(λ,0);Array.
Clear(ζ,0,maxCS);return λ;};static Func<float,float>ǈ=(ǆ)=>{if(ǆ>0||ς-ι>0.1)ς+=ǆ;ς=Math.Min(ς,1);ς=Math.Max(ς,0);Array.Clear(
θ,0,maxCS);return ς;};static Func<float,float>Ǉ=(ǆ)=>{if(ǆ<0||ς-ι>0.1)ι+=ǆ;ι=Math.Min(ι,1);ι=Math.Max(ι,0);Array.Clear(θ,
0,maxCS);return ι;};Vector2 ǅ;static ƻ(){ƺ=!V;Ʒ.Add(new Х(ƽ,ƿ,ʦ("H2/O2 Control"),ʦ(
"change automatic control of generators")));Ʒ.Add(new Х(ǎ,Ǎ,ʦ("ice movement"),ʦ("change automatic storage ice movement")));Ʒ.Add(new Х(ǌ,ǋ,ʦ("farm alignment"),ʦ
("change automatic farm alignment")));Ʒ.Add(new а(ǈ,ς,ʦ("change upper hydrogen limit for automatic control")));Ʒ.Add(new
а(Ǉ,ι,ʦ("change lower hydrogen limit for automatic control")));Ʒ.Add(new а(Ǌ,κ,ʦ(
"change upper oxygen limit for automatic control")));Ʒ.Add(new а(ǉ,λ,ʦ("change lower oxygen limit for automatic control")));}public static void Ǆ(List<Ж>ǃ,List<ϧ>ǂ){ƹ.
Clear();for(int Ï=0;Ï<maxCS;Ï++){ƶ[Ï]=Ʋ[Ï]=false;Ƶ[Ï]=Ʊ[Ï]=true;ư[Ï]=Ư[Ï]=ƴ[Ï]=Ƴ[Ï]=0;if(ŋ.ň[Ï])ƹ.Add(Ï);}if(ό>=cycleSpeed)Ƹ
++;ǁ(ǃ,ref Ʋ,ref Ʊ,ref Ư,ref ư);ǁ(ǂ,ref ƶ,ref Ƶ,ref Ƴ,ref ƴ);if(Ƹ>=ƹ.Count){if(ƹ.Count>0)Ƹ=0;else Ƹ=-1;}Ʒ[1].д=ε;Ʒ[2].д=
alignFarms;if(Ʒ[0].д!=μ)Ʒ[0].д=μ;}static void ǁ<Ɵ>(List<Ɵ>ǀ,ref bool[]ƾ,ref bool[]ǝ,ref float[]Ǵ,ref float[]ȉ)where Ɵ:ѓ{foreach(
var ȏ in ǀ)if(ŋ.ň[ȏ.Ƣ]){ƾ[ȏ.Ƣ]|=ȏ.ы;ǝ[ȏ.Ƣ]&=ȏ.ђ;ȉ[ȏ.Ƣ]+=ȏ.Щ;Ǵ[ȏ.Ƣ]+=ȏ.Ш;}}public ƻ(IMyTextSurface R):base(R,-1,0,ʦ(
"H2/O2 Program"),true){if(ê==null)ê=Ʒ.First();}override public void ƥ(int ƒ){if(Ʒ.Count==0){ê=null;return;}int Ȏ=0,ȍ;if(ê!=null)Ȏ=Ʒ.
IndexOf(ê);if(Ȏ<0){ê=Ʒ.First();return;}else{ȍ=Ȏ+ƒ;while(ȍ<0||ȍ>=Ʒ.Count){if(ȍ<0)ȍ+=Ʒ.Count;else ȍ-=Ʒ.Count;}}ê=Ʒ[ȍ];}void Ȍ(
Vector2 ÿ,Vector2 Ą,bool ȋ,bool Ȋ,float Ǵ,float ȉ,float Ȉ,float ȇ,bool Ȇ){Color ȅ=h2Color,Ȅ=colorOff;if(Ȇ){ȅ=o2Color;ÿ.X+=Ą.X*
0.2f;Ą.X*=0.6f;}if(ȋ)Ȅ=colorOn;float Ă=Math.Min(Ą.X,Ą.Y)*0.05f;Vector2 è=ÿ,į=ÿ+new Vector2(Ą.X,0);Vector2 Ė=ÿ+new Vector2(0,
Ą.Y),ġ=į+new Vector2(0,Ą.Y);è=ÿ;è.Y+=Ą.Y*0.2f;ģ(è,Ą.X,Ą.Y*0.8f,Ȅ,false,Ă);è.X+=Ă;è.Y+=Ă;ģ(è,Ą.X-(2*Ă),(Ą.Y*0.8f)-(2*Ă),ȅ,
false,Ă);è=ÿ;è.X+=Ą.X*0.35f;if(!Ȋ)ģ(è+new Vector2(Ă,0),(Ą.X*0.3f)-(2*Ă),(Ą.Y*0.2f)+(2*Ă),ȅ,true);Ę(è,Ă,Ą.Y*0.2f,Ȅ);è=į;è.X-=Ą
.X*0.35f;Ę(è,-Ă,Ą.Y*0.2f,Ȅ);è=Ė;è.X+=2*Ă;è.Y-=2*Ă;ģ(è,Ą.X-(4*Ă),((-Ą.Y*0.8f)+(4*Ă))*(Ǵ/ȉ),ȅ,true);if(μ){Ę(è-new Vector2(Ă
,((Ą.Y*0.8f)-(4*Ă))*Ȉ),Ą.X-(2*Ă),E/3,mainColor);Ę(è-new Vector2(Ă,((Ą.Y*0.8f)-(4*Ă))*ȇ),Ą.X-(2*Ă),E/3,mainColor);}}public
override void Ɲ(bool?Ŭ=false){º();if(!ƺ){ɪ("O2 tanks",ǃ);ɪ("H2 tanks",ǂ);ɪ("generators",Ά);ɪ();}else{Đ(î+í);if(!ó){ɦ();ǅ.X=(ù.
Width/2)-E;ǅ.Y=ǅ.X/ѓ.Ĭ;}if(Ƹ>=0){A=ù.Position+new Vector2(E/2,(ù.Height-ǅ.Y)/2);if(ϧ.Ϧ[ƹ[Ƹ]])Ȍ(A,ǅ,ƶ[ƹ[Ƹ]],Ƶ[ƹ[Ƹ]],Ƴ[ƹ[Ƹ]],ƴ[
ƹ[Ƹ]],ι,ς,false);A.X+=ǅ.X/2;A.Y-=F*2;Ʒ[3].Ġ(A,this);A.Y+=(3*F)+ǅ.Y;Ʒ[4].Ġ(A,this);A=new Vector2(ù.Right-(E/2)-ǅ.X,ù.Y+((ù
.Height-ǅ.Y)/2));if(Ж.Ϧ[ƹ[Ƹ]])Ȍ(A,ǅ,Ʋ[ƹ[Ƹ]],Ʊ[ƹ[Ƹ]],Ư[ƹ[Ƹ]],ư[ƹ[Ƹ]],λ,κ,true);A.X+=ǅ.X/2;A.Y-=F*2;Ʒ[5].Ġ(A,this);A.Y+=(3*
F)+ǅ.Y;Ʒ[6].Ġ(A,this);A=ô.Position+new Vector2(E);if(P)o(1.2f);else o(0.8f);o(1.3f);Ʒ.First().Ġ(A,this);o(1/1.3f);A.Y=ô.
Bottom-F-E;for(int w=2;w>0;w--)A.Y-=Ʒ[w].Ġ(A,this).Y+F;A.Y-=F;k();ě("Conveyor: "+(ƹ[Ƹ]+1),mainColor,TextAlignment.LEFT);}else{
X(ù.Center-new Vector2(0,F),"no generators\nfound",warningColor,TextAlignment.CENTER);}if(ƛ!=null)ê.т(ö,this);}ª();}}
class Ȟ:ü,Ƥ{public Ȟ(IMyTextSurface R,int Ï=-1,bool Ē=false):base(R,-1,12,"",false){µ();}public override void Ɲ(bool?Ŭ){
return;}}class ȝ{public IMyMotorStator Ȝ{get;}public float ț{get;set;}bool Ț=true;float ȟ;public int ș{get;set;}public float ȗ
{get{return(float)(Ȝ.Angle*180/Math.PI);}}public float Ȗ{get;}public float ȕ{get;}public ȝ(IMyMotorStator Ȕ){Ȝ=Ȕ;ȟ=ț=(
float)(Ȕ.Angle*180/Math.PI);ș=1;if(Ȕ.CustomName.Contains("Inv"))ș*=-1;Ȗ=Ȕ.LowerLimitDeg;ȕ=Ȕ.UpperLimitDeg;Ȕ.Torque=33599988;Ȕ
.BrakingTorque=33599988;Ȕ.TargetVelocityRPM=0;Ȕ.RotorLock=false;}public void ǵ(float Ȓ){Ȓ=Math.Abs(Ȓ);Ȝ.TargetVelocityRPM
=0;if(!Ț){if(ț<=ȟ){if(ț>Ȗ)Ȝ.LowerLimitDeg=ț;Ȝ.TargetVelocityRPM=-Ȓ;}if(ț>ȟ){if(ț>ȕ)Ȝ.UpperLimitDeg=ț;Ȝ.TargetVelocityRPM=
Ȓ;}if(ț==ȗ){Ȝ.TargetVelocityRPM=0f;Ȝ.LowerLimitDeg=Ȗ;Ȝ.UpperLimitDeg=ȕ;Ț=true;}}}public void ȓ(float Ȓ){Ț=false;Ȝ.
TargetVelocityRPM=Ȓ*ș;}public void ȑ()=>Ȝ.TargetVelocityRPM=0f;public void Ȑ()=>ș*=-1;public void Ș()=>ȟ=(float)(Ȝ.Angle*180/Math.PI);}
class ȃ{public string Ǽ{get;}public List<IMyOxygenFarm>ǲ{get;}public bool Ǳ{get;set;}=true;List<ȝ>ǰ;ȝ ǯ=null;float Ǯ;float ǭ;
float Ǭ;float ǫ;bool Ǫ=false;bool ǩ=false;const float Ǩ=0.2f;int ǧ=0;bool Ǧ=false;public string ǥ{get;private set;}bool Ǥ=
true;bool ǣ=true;int Ǣ=0;static public string ǡ(List<ȃ>Ǡ){StringBuilder ǟ=new StringBuilder(Ǡ.Count+">");for(int w=0;w<Ǡ.
Count;w++)if(w<Ǡ.Count-1)ǟ.Append(Ǡ[w].Ǳ+",");else ǟ.Append(Ǡ[w].Ǳ);return ǟ.ToString();}static public void ǳ(string Ǟ,List<ȃ
>Ǡ){try{int Ȃ=Ǡ.Count;int ȁ=int.Parse(Ǟ.Split('>')[0]);if(ȁ!=Ȃ)throw new System.FormatException();string[]Ȁ=Ǟ.Split('>')[
1].Split(',');for(int w=0;w<Ǡ.Count;w++)Ǡ[w].Ǳ=bool.Parse(Ȁ[w]);}catch{ͱ.с(
"Loading farm mast data failed\ndefault values used");return;}}public ȃ(string ǿ,List<IMyOxygenFarm>ǲ,List<IMyMotorStator>Ǿ){Ǽ=ǿ;this.ǲ=ǲ;Ǯ=ǲ.Max(ǹ=>ǹ.GetOutput())*ǲ.Count;
ǫ=ǲ.Sum(ǽ=>ǽ.GetOutput());ǭ=Ǭ=ǫ;if(Ǿ.Any()){ǰ=Ǿ.Where(Ƿ=>Ƿ.CustomName.Contains("Angle")).Select(Ƿ=>new ȝ(Ƿ)).ToList<ȝ>();
if(Ǿ.Any(Ƿ=>Ƿ.CustomName.Contains("Rotat")))ǯ=new ȝ(Ǿ.First(Ƿ=>Ƿ.CustomName.Contains("Rotat")));}}public string ǻ(){if(ǯ==
null&&!ǰ.Any())return"no rotors found";if(!Ǳ)return Ǹ();ǥ="no alignment needed";ǫ=ǲ.Sum(ǹ=>ǹ.GetOutput());float Ǻ=ǲ.Max(ǹ=>ǹ
.GetOutput())*ǲ.Count;if(Ǻ>Ǯ)Ǯ=Ǻ;if(ǫ>(Ǯ*0.1f)){if(ǫ<(Ǭ*0.998f)&&!ǩ&&!Ǫ){ǭ=ǫ;ǥ="starting alignment...";ǧ=0;Ǧ=false;if(ǯ!=
null)ǩ=true;else Ǫ=true;Ǣ=0;}if(ǩ){ǥ="rotating...";if(Ǣ<1)Ƕ(ǯ);else{ǩ=false;Ǫ=true;Ǣ=0;ǭ=ǫ;Ǧ=false;ǧ=0;}}if(Ǫ){ǥ=
"align angle...";if(Ǣ<ǰ.Count)Ƕ(ǰ[Ǣ]);else{Ǫ=false;Ǣ=0;ǭ=ǫ;Ǧ=false;ǧ=0;}}if(ǫ>=Ǭ&&!ǩ&&!Ǫ){if(ǯ!=null)ǯ.ȑ();ǰ.ForEach(Ƿ=>Ƿ.ȑ());ǩ=Ǫ=false
;Ǭ=ǭ=ǫ;ǥ="alignment done";}}else{ǩ=Ǫ=false;ǰ.ForEach(Ƿ=>Ƿ.ȑ());if(ǯ!=null){ǯ.ȑ();ǵ();}ǥ="night mode -> sunrise";}return ǥ
;}public string Ǹ(){if(!ǣ){if(ǯ!=null)ǯ.ȑ();ǰ.ForEach(Ƿ=>Ƿ.ȑ());ǧ=0;ǣ=true;}if(π)return"alignment paused";return
"alignment offline";}void Ƕ(ȝ ʧ){Ǥ=false;ǣ=false;ʧ.ȓ(Ǩ);if(ǧ>=2&&ǫ<ǭ&&!Ǧ){ʧ.Ȑ();Ǧ=true;ǧ=-1;ǭ=ǫ;}if(ǧ>=2&&ǫ>=ǭ){ǭ=ǫ;}if((ǫ<ǭ&&Ǧ)||(ʧ.ȗ==ʧ.Ȗ
||ʧ.ȗ==ʧ.ȕ)){ʧ.ȑ();Ǣ++;ǭ=ǫ;ǧ=-1;Ǧ=false;}ǧ++;}void ǵ(){if(!Ǥ){Ǥ=true;ǯ.Ș();if(ǯ.ȗ>0)ǯ.ț=ǯ.ȗ-180f;else ǯ.ț=ǯ.ȗ+180f;}ǯ.ǵ(
1.0f);}}