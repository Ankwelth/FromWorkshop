// # STC Grid Manager v3.0 – User Guide
// 
// Author: Raidfire
// 
// ----------------------------------------------------------------------------------------------------
// | Features:
// |
// |   • Automatic reactor fuel management
// |       - Supports up to 3 fully independent fuel systems
// |       - Works with ANY reactor block and ANY modded fuel type
// |       - Dynamic reactor classification based on accepted fuel
// |       - Automatic refuelling based on configurable thresholds
// |       - Real-time fuel consumption and runtime estimates
// |       - Alarm system with visual and sound alerts for low fuel conditions
// |
// |   • Advanced refinery automation
// |       - Automatic conveyor-based refining
// |       - Priority-based refining (global ore priority list)
// |       - Name-based refining (per-refinery ore targeting)
// |       - Refinery safety logic and stuck detection
// |
// |   • Inventory logistics and automation
// |       - Menu-driven inventory sorting by category
// |       - Category-based container assignment (ores, ingots, components, ammo, misc)
// |       - Fixed-amount container logic via CustomData templates
// |       - Miner unloading automation
// |
// |   • Docked ship support
// |       - Automatically detects docked ships via connectors
// |       - Manages fuel and inventories on connected grids
// |
// |   • LCD-driven menu system
// |       - Fully configurable in-game menus
// |       - No recompiling required for normal operation
// |       - Live status, warnings, and diagnostics display
// |
// |   • Performance-optimized execution model
// |       - All heavy systems run in staged cycles
// |       - Prevents script termination on large grids
// |
// |   • Built-in profiling and diagnostics
// |       - Real-time performance profiling
// |       - Separate block refresh profiling
// |       - Easy identification of performance bottlenecks
// |
// |   • Master grid detection
// |       - Prevents duplicate automation across docked grids
// |       - Ensures only one Grid Manager controls a construct
// |
// |   • Station-focused design
// |       - Optimized for stations
// |       - Handles complex industrial setups reliably
// ----------------------------------------------------------------------------------------------------
// NB!! READ ALL SECTIONS CAREFULLY BEFORE USE!! THIS SCRIPT WILL ONLY RUN AS GOOD AS THE GRID IT RUNS ON
// ----------------------------------------------------------------------------------------------------
// | #1. Setup
// |
// |   1. LCD Display
// |   • Assign the LCD tag (default: [SPC]) to the LCD you want as your menu display.
// |   • The script will render all menus and information on this LCD.
// |
// |   2. Hotbar Controls
// |   • Add the following hotbar buttons to the programmable block:
// |       UP
// |       DOWN
// |       APPLY
// |       LEFT
// |       RIGHT
// |
// |   • LEFT / RIGHT are used to change pages on paginated information screens.
// |   • Run the programmable block once with each argument to initialize menu navigation.
// |
// |   3. Initial Configuration
// |   • Everything is configurable in the “Config Area” at the top of the script.
// |   • On first run, all features start OFF.
// |   • Reactor1, Reactor2 and Reactor3 can be mapped to ANY reactor block and ANY fuel type.
// |   • Recompiling preserves current settings (fuel targets, priorities, toggles).
// |   • Reset values through the Notification Messages menu when needed.
// |
// |   4. Tagged Containers & Blocks
// |   • Tag containers so the script knows their purpose:
// |       "Ores"        – Ore containers
// |       "Ingots"      – Ingot containers
// |       "Components"  – Component containers
// |       "Ammo"        – Ammo containers
// |       "Misc"        – Misc containers
// |
// |   • Add warning tags to lights/sound blocks for fuel alarms:
// |       [FUEL1-WARNING]
// |       [FUEL2-WARNING]
// |       [FUEL3-WARNING]
// |
// |   • Add "Special" to containers you want auto-templated CustomData for fixed-amount logic.
// |     After tagging, run inventory sorting once to generate a template.
// |     Fill in desired amounts and run sorting again to apply.
// ----------------------------------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------------------------------
// | #2. Menu Navigation
// |
// |   • Use UP, DOWN and APPLY to navigate the LCD menu.
// |   • Highlighted items are marked with `>`.
// |   • Most menu items toggle features ON/OFF.
// |   • Reactor1, Reactor2 and Reactor3 each have their own sub-menus.
// |
// |   • Additional sub-menus exist for:
// |       - Fuel rate pages
// |       - Refinery handling
// |       - Inventory management
// |       - Notification messages
// |
// |   • Some information screens are paginated (for example: Resource Overview).
// |       - When more than 20 item types exist, additional pages are created automatically.
// |       - Use LEFT / RIGHT to change pages.
// |       - Inventory management options appear only on the first page.
// |       - “Return to Main Menu” is always available on all pages.
// |
// |   • All menu settings and toggles are saved automatically.
// |     Settings persist through world reloads and script recompiles.
// ----------------------------------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------------------------------
// | #3. Reactor Handling
// |
// |   Grid Manager supports three fully independent fuel systems.
// |   All systems are fully generic — ANY fuel type and ANY reactor subtype can be assigned.
// |
// |   IMPORTANT PERFORMANCE NOTE:
// |   Reactor refuelling is processed in STAGES.
// |   Only ONE fuel system is handled per update cycle.
// |   This is intentional and prevents script termination on large grids.
// |
// |   Fuel Assignment:
// |   • A reactor is assigned to Reactor1/2/3 based ONLY on accepted fuel type.
// |   • No hardcoded subtype checks — everything is dynamic.
// |
// |   Fuel Management Logic:
// |   • Displays:
// |       - Current output (MW)
// |       - Consumption rate
// |       - Remaining fuel
// |       - Estimated runtime
// |   • Automatically refills when below configured thresholds.
// |   • "!ignore" in the block name excludes a reactor.
// |   • Docked ships are detected and refueled automatically.
// |
// |   LCD Display Notes:
// |   • Fuel availability and totals may update sequentially.
// |   • This is expected behavior due to staged processing.
// ----------------------------------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------------------------------
// | #4. Refinery Handling
// |
// |   Refinery operation modes:
// |
// |   1. Automatic Mode
// |      • Default SE conveyor behavior.
// |
// |   2. Priority Refining
// |      • Script scans ore containers and refinery inputs.
// |      • User-defined global ore priority order.
// |
// |   3. Name-Based Refining
// |      • Refinery processes ores based on names in its block name.
// |      • First match is highest priority.
// |
// |   PERFORMANCE NOTE:
// |   Refinery logic runs in STAGES:
// |       - Evaluation and filling
// |       - Draining when output is full
// |
// |   Because of this:
// |   • Status counts may update intermittently.
// |   • This is expected behavior and not an error.
// |
// |   Safety & Reliability:
// |   • Automatic removal of missing/broken refineries.
// |   • Stuck refinery detection and recovery.
// |   • Queue clearing when priorities change.
// ----------------------------------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------------------------------
// | #5. Inventory Sorting
// |
// |   • Sorting runs once per activation to avoid performance impact.
// |
// |   • Special containers:
// |       - Tagged with "Special"
// |       - CustomData template is auto-generated
// |       - Edit desired amounts and run sorting again to apply
// |
// |   • Supports:
// |       - Category overrides
// |       - Ignoring containers with "[NO-SORT]"
// |       - Maintaining fixed quantities
// ----------------------------------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------------------------------
// | #6. Special Tags
// |
// |   "Special"        • Fixed-amount CustomData logic
// |   "[MANUAL]"       • Refinery ignored by automation
// |   "!ignore"        • Reactor excluded from fuel handling
// |   Warning Tags     • Alarm system:
// |                      [FUEL1-WARNING]
// |                      [FUEL2-WARNING]
// |                      [FUEL3-WARNING]
// ----------------------------------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------------------------------
// | #7. Performance & Staging Notes (IMPORTANT)
// |
// |   • To prevent exceeding execution limits, Grid Manager spreads heavy work
// |     across multiple update cycles.
// |
// |   • Some values (ore totals, fuel availability, refinery counts)
// |     update sequentially over several seconds.
// |
// |   • This is intentional and required for large grids.
// |
// |   • Menu navigation and LCD rendering are NOT staged.
// |   • Informational values are cached and updated in the background.
// ----------------------------------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------------------------------
// | #8. Notes & Limitations
// |
// |   • Designed primarily for stations.
// |   • Handles reactors and inventories on directly docked ships.
// |   • Does NOT support ship → ship → station daisy chains.
// |   • Running on a ship manages only that ship.
// |
// |   • When multiple grids are docked together:
// |       - Only ONE Grid Manager actively controls the construct.
// |       - Other Grid Managers automatically pause to prevent conflicts.
// |
// |   • Does NOT manage:
// |       - Batteries
// |       - Solar panels
// |
// |   • These are intentionally excluded.
// ----------------------------------------------------------------------------------------------------
// 
// CONFIG AREA BELOW:
// 

    // Master Grid Tag \\ - Only needed if you running multiple grids the at some point connect to eachother. If sort inventory is used while connected master grid will pull everything from components, ingots, ammo and misc containers on connected grids.
    private const string MasterTag = "[STC-MASTER]"; // Tag to identify the master grid running Grid Manager. If a grid is connected to a grid with this tag, Grid Manager will pause its operations.

    // Refresh Block List Interval \\
    private const int REFRESH_INTERVAL_SECONDS = 5; // Interval in seconds to refresh block lists. Recommended to keep this at 5 or higher to reduce performance impact.

    // Slowdown script \\
    private const UpdateFrequency UF = UpdateFrequency.Update1; // Selections for slowdown is Update1(every tick), Update10(every 10 tick) and Update100(and ever 100 tick) 60 ticks = 1 second. 

    // LCD Designation Tag \\
    private const string LCDTAG = "[SPC]"; // Add this tag to an lcd. You can also rename the tag within the " ". This lcd will be used to display the menus and information.
    private static string notificationMenuTag = "Notification Messages"; // Menu Category Name. Menu title

    // Container Designation Tag \\
    private const string oresTag = "Ores"; // Add this tag to your ore containers
    private const string ingotsTag = "Ingots"; // Add this tag to your ingot containers
    private const string componentsTag = "Components"; // Add this tag to your component containers
    private const string ammoTag = "Ammo"; // Add this tag to your ammo containers
    private const string miscTag = "Misc"; // Add this tag to your misc containers
    private const bool DEFAULT_PRIORITY_SORTING = false; // Set to true to enable priority sorting. This will make the script fill containers based on the order of the tags.
    // Use [1],[2],[3] etc to designate multiple containers for the same category. Based on priority to fill first.
    // I am not adding containers for bottles and tools as those are usually in small amounts and can be put in misc.
    // You can not use both priority sorting and balancing at the same time. Choose one or the other because they will cancel eachother out if both are enabled.
    // NB!! Still needs a little more rework because it does not split stacks based on cargo volume yet.
    private const bool DEFAULT_GROUP_STACKS = false; // Set to true to enable grouping of identical item stacks within the same container.
    private const bool DEFAULT_BALANCE_ITEMS = false; // Set to true to enable balancing of items across multiple containers of the same type.
    private static string sortingMenuTag = "Sort Inventory"; // Menu Category Name. Menu title

    // Assembler Input Protection Settings       
    private const double ASSEMBLER_INPUT_MAX_AMOUNT = 10000;   // Assembler Input Maximum Allowed Amount Per Item
    private const int ASSEMBLER_CLEAN_INTERVAL = 300; // 300 ticks ≈ 5 seconds (Update1) or adjust to your update frequency

    // Refinery Handling Tags \\
    private static string refineryMenuTag = "Refinery Handling"; // Menu Category Name. Menu title. Handles refineries
    private static string resourceInfoTag = "Ingots/Ores Info"; // Menu Category Name. Menu title. Displays current amounts of ingots and ores.
    private static bool namebasedOnOff = false; // Set to true to enable name-based refining by default. This will make the script look for ore names in the refinery name to prioritize refining. 
    private static bool priorityRefiningOnOff = true; // Set to true to enable priority refining by default. This will make the script refine ores based on the order in the refinery list.

    private static bool connectedGrids = false; // Set to true to enable getting blocks like refineries and lcds(need tag) connected grids via connectors.
                                                // This allows refineries on connected ships or stations to be filled with ore from the main grid.

    // Frequency settings
    bool trackRefineryProduction = false; // Set to true to enable refinery production tracking
    const int REFINERY_SCAN_INTERVAL_SEC = 10;    // read output every 10 sec
    const int CUSTOM_DATA_WRITE_INTERVAL_SEC = 120; // write to PB customdata every 2 min
                                                    // NB!! Only one of these can be active at a time. If both are true, then both will be turned off.
                                                    // Also when using namebased refining, it only allows one ore to be prioritized per refinery. So if you have multiple ores in the refinery name, it will only prioritize the first one it finds in the refinery list.
    private const string manualTag = "[MANUAL]"; // This is a tag i want you to use on refineries for custom ore. 
                                                 // This is because some modders dont know how to name their ores properly and keeping it consistant.
                                                 // So if an ore doesnt go into a refinery its not the scripts fault its the modder who made the custom ore.

    // -- Hydrogen Refueling and Generation Configuration \\
    private static string h2TankH2GenMenuTag = "H2 Tank/Generator"; // Menu Category Name. Menu title
    private const bool HydrogenRefuelOnOff = false; // Set to true to enable hydrogen refueling and generation handling
    private const double h2RefillThreshold = 0.10; // Set the hydrogen refill threshold (0.0 to 1.0). Ships will be refueled when base hydrogen tanks are above this threshold.
    private const bool H2GenHandlingOnOff = false; // Set to true to enable hydrogen generator handling
    private const int iceFillAmount = 5;
    private const bool autoUndockShips = false; // Set to true to enable automatic undocking of ships when base hydrogen tanks are low on hydrogen. Or ship is full on hydrogen.
                                                // Ships will be undocked when hydrogen level is below the set threshold and will be redocked when hydrogen level is above the threshold.

    // Reactor1 Handling Tag \\
    private static string BlockTitle1 = "Uranium Reactor"; // messages
    private static string menuTag1 = "Uranium Reactor Handling"; // Menu Category Name.
    private static string menuTagFuelRates1 = "Uranium Cons Rates/Time"; // Menu Category Name. Menu for block power and time rates
    private const bool Block1OnOff = true; // Set to true to enable Reactor1 handling
    private const string fueltag1 = "Uranium"; // Fuel SubtypeId. NB!! must be exact to assign right reactor list.
    // Fallback values for Reactor1 handling (used when subtype is unknown)
    private const double DEFAULT_REACTOR1_MAX_POWER = 300.0; // Default fallback max power if dictionary lookup fails
    private const double DEFAULT_REACTOR1_CONSUMPTION_MAX = 0.08333; // Default fallback consumption at max power if dictionary lookup fails

    // Reactor1 Fuel Warning Tag \\
    private const string REACTOR1_WARNING_TAG = "[FUEL1-WARNING]"; // Add this tag to an lights and soundblocks to show reactor1 fuel warnings

    // Some parts below are for advanced users who want to add more reactor types.

    // Reactor1 Fuel Consumption Rates \\
    private static readonly Dictionary<string, double> MaxPowerReactor1Subtypes = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
    {
        { "LargeBlockLargeGenerator",    300.0 }, // Default vanilla reactor
    };
    // Reactor1 Total Consumption at Max Power \\
    private static readonly Dictionary<string, double> TotalConsumptionReactor1MaxBySubtypes = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
    {
        { "LargeBlockLargeGenerator",    0.08333 }, // Default vanilla reactor
    };
    // -- End Reactor1 Configuration \\


    // Reactor Type 2 Handling Tag \\ -- Optional as this is specific to HexServer where i play. Can be reused for diffrent reactor blocks. Change as needed.
    private static string BlockTitle2 = ""; // messages
    private static string menuTag2 = ""; // Menu Category Name. Menu title
    private static string menuTagFuelRates2 = ""; // Menu Category Name. Menu for block power and time rates
    private const bool Block2OnOff = false; // Set to true to enable Reactor2 handling. 
    private const string fueltag2 = ""; // Fuel SubtypeId. NB!! must be exact to assign right reactor list.
    private const double DEFAULT_REACTOR2_MAX_POWER = 0; // Default fallback max power if dictionary lookup fails
    private const double DEFAULT_REACTOR2_CONSUMPTION_MAX = 0; // Default fallback consumption at max power if dictionary lookup fails

    // Reactor2 Fuel Warning Tag \\
    private const string REACTOR2_WARNING_TAG = "[FUEL2-WARNING]"; // Add this tag to an lights and soundblocks to show reactor2 fuel warnings

    // Reactor2 Fuel Consumption Rates \\
    private static readonly Dictionary<string, double> MaxPowerReactor2Subtypes = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
    {
        // Example Reactor2 { "SubtypeId", 100.0 },  Find reactor subtypeids via your own script or looking at the mod itself in your workshop folder.
    };
    // Reactor2 Total Consumption at Max Power \\
    private static readonly Dictionary<string, double> TotalConsumptionReactor2MaxBySubtypes = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
    {
        // Example Reactor2 { "SubtypeId", 0.1000 }, Find reactor subtypeids via your own script or looking at the mod itself in your workshop folder.
    };
    // -- End Reactor2 Configuration \\

    // Reactor Type 3 Handling Tag \\ -- Optional as this is specific to HexServer where i play. Can be reused for diffrent reactor blocks. Change as needed.
    private static string BlockTitle3 = ""; // messages
    private static string menuTag3 = ""; // Menu Category Name. Menu title
    private static string menuTagFuelRates3 = ""; // Menu Category Name. Menu for block power and time rates
    private const bool Block3OnOff = false; // Set to true to enable Reactor3 handling. 
    private const string fueltag3 = ""; // Fuel SubtypeId. NB!! must be exact to assign right reactor list.
    private const double DEFAULT_REACTOR3_MAX_POWER = 0; // Default fallback max power if dictionary lookup fails
    private const double DEFAULT_REACTOR3_CONSUMPTION_MAX = 0; // Default fallback consumption at max power if dictionary lookup fails

    // Reactor3 Fuel Warning Tag \\
    private const string REACTOR3_WARNING_TAG = "[FUEL3-WARNING]"; // Add this tag to an lights and soundblocks to show reactor3 fuel warnings

    // Reactor3 Fuel Consumption Rates \\
    private static readonly Dictionary<string, double> MaxPowerReactor3Subtypes = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
    {
        // Example Reactor2 { "SubtypeId", 0.1000 }, Find reactor subtypeids via your own script or looking at the mod itself in your workshop folder.
    };

    // Reactor3 Total Consumption at Max Power \\
    private static readonly Dictionary<string, double> TotalConsumptionReactor3MaxBySubtypes = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
    {
        // Example Reactor2 { "SubtypeId", 0.1000 }, Find reactor subtypeids via your own script or looking at the mod itself in your workshop folder.
    };
    // -- End Reactor3 Configuration \\

    // Reactor Ignore Tag \\
    private const string IgnoreTag = "[Ignore]"; // Add this tag to reactor blocks you want the script to ignore completely. 
                                                 // Meaning no refueling or alarm checking will be done on reactors with this tag.

    // Inventory Sorting Tags \\
    private const string IgnoreSortingTag = "[NO-SORT]"; // Add this tag to connectors of grids you want the script to ignore completly.
                                                         // Meaning no sorting(aka unloading ore, refueling reactors or transferring special cargo) will be done when docked to this grid.

    // Special Cargo Tag \\
    private const string SpecialCargo = "Special"; // Add this tag to containers you want to use for special cargo.
                                                   // When applied to a container, the script will fill its customdata with the list of items.
                                                   // Then fill out amounts in the customdata to have the script move those items into the container during sorting.

    // Grid Type Tags \\
    private const string NonCombat = "[NONCOMBAT]"; // Use this to avoid counting them as combat ships

    // === PLAYER CUSTOM CATEGORY OVERRIDES ===
    // These override the *default classification*. When sorting
    // Add subtypeis only (not TypeId).
    // SubtypeIds can be full or partial matches. Because the method used is Contains(), these lists are mostly used to alter their sorting because the server defines them as something else.
    // 1 example being lets say the server has some planter seeds that are defined as ore but i want it in the misc container.
    // Be careful when partially matching names. Use full subtypeids when possible. Or use a unique identifier from the items subtypeid.
    // If and item is sorted wrong that means you either added a word that matches more items then the one you where trying to override.
    static readonly Dictionary<string, string> CategoryOverrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Ammo logistics disguised as components
        // Examples
        //{ "emptysmall", "ammo" },
        //{ "spentcartridge", "ammo" },
        //{ "shell", "ammo" },
        //{ "clip",  "ammo" },
        //{ "mag",   "ammo" },
        //{ "box",   "ammo" },
        //{ "rocket","ammo" },

        // Future-proof ammo terms
        //{ "casing",    "ammo" },
        //{ "cartridge", "ammo" },

        //{ "slag", "ore" },
        //{ "scrap", "misc" },
        //{  "seed", "misc"  },
        //{ "tool", "component" },
        //{  "welding", "component" },
        //{ "rebelscrap", "ingot" }
    };
    /*
    * 
    */
    // End Configuration Section \\
bool A=false;int B=0;private const double C=0.5;private const int D=125;private const int E=20;double F;double G;double
H;double I;double J;double K;double L;double M;double N;List<IMyReactor>O=new List<IMyReactor>();List<IMyReactor>P=new
List<IMyReactor>();List<IMyReactor>Q=new List<IMyReactor>();List<IMyReactor>R=new List<IMyReactor>();List<IMyCargoContainer>
S=new List<IMyCargoContainer>();List<IMyTextPanel>T=new List<IMyTextPanel>();List<IMyRefinery>U=new List<IMyRefinery>();
List<IMyShipConnector>V=new List<IMyShipConnector>();List<IMyAssembler>W=new List<IMyAssembler>();List<IMyGasGenerator>X=new
List<IMyGasGenerator>();List<IMyGasTank>Y=new List<IMyGasTank>();List<IMyGasTank>Z=new List<IMyGasTank>();bool a=Block1OnOff
;bool b=Block2OnOff;bool c=Block3OnOff;bool d=HydrogenRefuelOnOff;bool e=H2GenHandlingOnOff;bool f=autoUndockShips;bool g
;bool h;bool i;bool j=priorityRefiningOnOff;bool k=namebasedOnOff;bool l=DEFAULT_PRIORITY_SORTING;bool m=
DEFAULT_BALANCE_ITEMS;bool n=DEFAULT_GROUP_STACKS;int o=0;int p=0;int q=0;int r=-1;List<string>s=new List<string>();private const int t=0;
private const int u=1;private const int v=2;private const int w=3;private const int x=4;private const int y=5;private const int
z=6;private const int ª=7;private const int µ=8;private const int º=9;private const int À=10;List<string>Á=new List<
string>();string Â=null;DateTime Ã=DateTime.MinValue;string Ä=null;Dictionary<long,int>Å=new Dictionary<long,int>();int Æ=0;
int Ç=0;int È=0;int É=0;int Ê=0;List<string>Ë=new List<string>();bool Ì;int Í=0;string Î="mg/s";int Ï=-1;List<string>Ð=new
List<string>(20);List<MyInventoryItem>Ñ=new List<MyInventoryItem>(32);Dictionary<string,List<double>>Ò=new Dictionary<string
,List<double>>();Dictionary<string,List<double>>Ó=new Dictionary<string,List<double>>();Dictionary<long,int>Ô=new
Dictionary<long,int>();List<MyItemType>Õ=new List<MyItemType>();MyItemType Ö;MyItemType Ø;MyItemType Ù;int Ú=0;bool Û=true;int Ü=3
;bool Ý=false;int Þ=0;int ß=7;int à=0;int á=0;bool â=false;int ã=6;private const int ä=60;int å=60;int æ=0;int ç=0;int è=
0;int é=0;int ê=3;int ë=0;public int ì=3;int í=10;int î=0;int ï=0;private static readonly int ð=2;Dictionary<
IMyShipConnector,bool>ñ=new Dictionary<IMyShipConnector,bool>();HashSet<long>ò=new HashSet<long>();Dictionary<long,bool>ó=new Dictionary
<long,bool>();Dictionary<string,double>ô=new Dictionary<string,double>();Dictionary<long,Dictionary<string,double>>õ=new
Dictionary<long,Dictionary<string,double>>();DateTime ö=DateTime.MinValue;DateTime ø=DateTime.MinValue;Dictionary<long,int>ù=new
Dictionary<long,int>();HashSet<long>ú=new HashSet<long>();double û;double ü;double ý;int þ;int ÿ;int Ā;int ā=0;int Ă=0;int ă=0;int
Ą=0;int ą=0;int Ć=0;int ć=0;bool Ĉ;bool ĉ=false;private readonly List<MyInventoryItem>Ċ=new List<MyInventoryItem>(64);
private readonly Dictionary<string,List<IMyTerminalBlock>>ċ=new Dictionary<string,List<IMyTerminalBlock>>();private readonly
Dictionary<string,int>Č=new Dictionary<string,int>();int č=0;List<IMyCargoContainer>Ď=new List<IMyCargoContainer>();List<
IMyRefinery>ď=new List<IMyRefinery>();HashSet<string>Đ=new HashSet<string>();double đ=0;string Ē="Idle";string ē="";string Ĕ="";int
ĕ=-1;int Ė=-1;private const int ė=100;private const int Ę=80;private const int ę=60;private const int Ě=40;bool ě=false;
int Ĝ=0;private const int ĝ=20;private const int Ğ=4;private const int ğ=1;HashSet<IMyCubeGrid>Ġ=new HashSet<IMyCubeGrid>()
;int ġ=0;const int Ģ=120;StringBuilder ģ=new StringBuilder(256);public
 Program
(){Ĥ();if(!ĉ){ĥ();ĉ=true;}Ħ();Runtime.UpdateFrequency=UF;Me.CustomData="";}void ĥ(){s.Clear();s.Add(sortingMenuTag);s.Add
(notificationMenuTag);if(a){s.Add(menuTag1);s.Add(menuTagFuelRates1);}if(b){s.Add(menuTag2);s.Add(menuTagFuelRates2);}if(
c){s.Add(menuTag3);s.Add(menuTagFuelRates3);}s.Add(refineryMenuTag);s.Add(h2TankH2GenMenuTag);s.Add(resourceInfoTag);}
void Ĩ(){bool ħ=false;if(g!=a||h!=b||i!=c){ħ=true;}if(ħ){ĥ();}g=a;h=b;i=c;}public void
 Main
(string ĩ){double Ī=Runtime.TimeSinceLastRun.TotalSeconds;if(đ>0)đ-=Ī;ě=false;ī();if(!A){Ĭ();return;}if(ĭ()){Echo(
"Paused: Docked to Master grid.");return;}if(Runtime.CurrentInstructionCount>48000){Echo("Execution fuse tripped — skipping tick");return;}Į(
"STC GridManager");į(ĩ);if(ĩ=="sort"){İ("Sort Inventory",ė);int ĳ=ı("SortInventory",Ĳ);Ĵ(ĳ);return;}if(!Ý&&ĵ()){}if(!ě&&Ý){İ(
"Block Refresh",Ę);Ķ();ě=true;}if(!ě&&Û){İ("Grid Maintenance",ę);switch(Ú){case 0:ķ(0,"Discover Ores");ĸ("Discover Ores",Ĺ);break;case
1:ķ(1,"Reactor Refueling");ĺ();break;case 2:Ĩ();break;}Ú=(Ú+1)%Ü;if(Ú==0){Û=false;â=true;}ě=true;}if(!ě&&++è>=
ASSEMBLER_CLEAN_INTERVAL){è=0;İ("Assembler Maintenance",Ě);Ļ();ě=true;}if(++æ>=60){æ=0;ļ();return;}if(++ç>=30){ç=0;Ľ();return;}if(++î>=í){î=0;ľ(
);return;}if(!ě&&â){İ("Production Tasks",Ě);Ŀ();ě=true;}ŀ();if(Ì){ï=0;Ł();Ì=false;}ĸ("LCD Update",ł);}void Ķ(){switch(Þ){
case 0:ķ(0,"Cargo");Ń("Refresh Cargo Containers",ń);break;case 1:ķ(1,"Refineries");Ń("Refresh Refineries",Ņ);break;case 2:ķ(
2,"Reactors");Ń("Refresh Reactors",ņ);break;case 3:ķ(3,"LCDs");Ń("Refresh LCDs",Ň);break;case 4:ķ(4,"Connectors");Ń(
"Refresh Connectors",ň);break;case 5:ķ(5,"Assemblers");Ń("Refresh Assemblers",ŉ);break;case 6:ķ(6,"H2 Gens/Tanks");Ń("Refresh H2 Gens/Tanks"
,Ŋ);break;}Þ++;if(Þ>=ß){Þ=0;Ý=false;Û=true;â=false;}}void Ŀ(){switch(á){case 0:{int Ō=ŋ();if(Ō==0)break;ķ(0,
"Miner Unload");ĸ("Miner Unload",ō);break;}case 1:ķ(1,"Refinery Handling");ĸ("Update Refineries",Ŏ);break;case 2:if(!Ĉ)break;ķ(2,
"Refinery Capacity Check");ŏ();break;case 3:if(trackRefineryProduction){ķ(3,"Refinery Production Tracking");ĸ("Refinery Prod. Tracking",Ő);}break
;case 4:if(e){ķ(4,"H2 Generator Handling");ĸ("H2 Generation",ő);}break;case 5:if(d){ķ(5,"H2 Refueling");ĸ("H2 Refueling",
Œ);}break;}á=(á+1)%ã;if(á==0){â=false;Û=true;}}void ŀ(){for(int œ=0;œ<U.Count;œ++){var Ŕ=U[œ];if(Ŕ==null)continue;long ŕ=
Ŕ.EntityId;int Ŗ=Ŕ.CustomName.GetHashCode();int ŗ;if(!Å.TryGetValue(ŕ,out ŗ)){Å[ŕ]=Ŗ;continue;}if(ŗ!=Ŗ){Å[ŕ]=Ŗ;if(k)Ì=
true;}}}void ř(StringBuilder Ř){Ř.AppendLine("SYSTEM MODES:");Ř.AppendLine($"• Inventory Balance: {(m?"ON":"OFF")}");Ř.
AppendLine($"• Inventory Stacking: {(n?"ON":"OFF")}");Ř.AppendLine($"• Sorting Priority: {(l?"ON":"OFF")} ");if(
priorityRefiningOnOff)Ř.AppendLine("• Refinery Mode: PRIORITY");else if(namebasedOnOff)Ř.AppendLine("• Refinery Mode: NAME-BASED");else Ř.
AppendLine("• Refinery Mode: AUTO");}void İ(string Ś,int ś){if(ś<Ė)return;Ė=ś;Ē=Ś;ĕ=-1;ē="";Ĕ="";}void ķ(int Ŝ,string ŝ){ĕ=Ŝ;ē=ŝ;Ĕ
="";}void ş(string Ş){if(Ė<0)return;Ĕ=Ş;}void ī(){Ė=-1;Ē="Idle";ĕ=-1;ē="";Ĕ="";}void š(StringBuilder Š){Š.AppendLine(
"=== Current Task ===");if(Ė<0||Ē=="Idle"){Š.AppendLine("Idle");Š.AppendLine("");return;}if(!string.IsNullOrEmpty(ē)){Š.Append(Ē).Append(" → "
).AppendLine(ē);if(!string.IsNullOrEmpty(Ĕ))Š.Append("• ").AppendLine(Ĕ);}else{Š.AppendLine(Ē);}Š.AppendLine("");}void ŏ(
){Ĉ=false;foreach(var Ţ in U){if(!Ţ.IsProducing)continue;Ĉ=true;if(Ţ.OutputInventory.VolumeFillFactor>=0.75){ò.Add(Ţ.
EntityId);}}}bool ţ(){return Me.CubeGrid.IsStatic;}void ń(){S.Clear();GridTerminalSystem.GetBlocksOfType(S,Ť=>Ť.CubeGrid==Me.
CubeGrid&&Ť.IsFunctional);}void Ũ(HashSet<IMyCubeGrid>ť){ť.Clear();ť.Add(Me.CubeGrid);if(!connectedGrids)return;var V=new List<
IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(V,Ť=>Ť.CubeGrid==Me.CubeGrid);foreach(var Ť in V){if(Ť.Status!=
MyShipConnectorStatus.Connected)continue;var Ŧ=Ť.OtherConnector?.CubeGrid;if(Ŧ==null)continue;if(!Ŧ.IsStatic)continue;if(ŧ(Ŧ))continue;ť.Add(
Ŧ);}}void Ņ(){U.Clear();var Ġ=new HashSet<IMyCubeGrid>();Ũ(Ġ);GridTerminalSystem.GetBlocksOfType(U,Ŕ=>Ŕ.IsFunctional&&Ġ.
Contains(Ŕ.CubeGrid));}void Ň(){T.Clear();var ũ=new HashSet<IMyCubeGrid>();Ũ(ũ);GridTerminalSystem.GetBlocksOfType(T,Ū=>Ū.
IsFunctional&&ũ.Contains(Ū.CubeGrid)&&Ū.CustomName.Contains(LCDTAG));}void ň(){V.Clear();GridTerminalSystem.GetBlocksOfType(V,Ť=>Ť.
CubeGrid==Me.CubeGrid&&Ť.IsFunctional);}void ņ(){O.Clear();P.Clear();Q.Clear();R.Clear();GridTerminalSystem.GetBlocksOfType(O,Ŕ
=>Ŕ.CubeGrid==Me.CubeGrid&&Ŕ.IsFunctional&&!Ŕ.CustomName.Contains(IgnoreTag));foreach(var Ŭ in O){int ū;if(!Ô.TryGetValue(
Ŭ.EntityId,out ū)){ū=ŭ(Ŭ);Ô[Ŭ.EntityId]=ū;}switch(ū){case 1:P.Add(Ŭ);break;case 2:Q.Add(Ŭ);break;case 3:R.Add(Ŭ);break;}}
Ů(P);Ů(Q);Ů(R);}void ŉ(){W.Clear();GridTerminalSystem.GetBlocksOfType(W,ů=>ů.CubeGrid==Me.CubeGrid&&ů.IsFunctional);}void
Ŋ(){X.Clear();Y.Clear();GridTerminalSystem.GetBlocksOfType(X,Ű=>Ű.IsFunctional&&Ű.CubeGrid==Me.CubeGrid);
GridTerminalSystem.GetBlocksOfType(Y,ű=>ű.IsFunctional&&ű.CubeGrid==Me.CubeGrid);}int ŭ(IMyReactor Ŭ){Õ.Clear();var Ų=Ŭ.GetInventory();Ų.
GetAcceptedItems(Õ);if(Õ.Contains(Ö))return 1;if(Õ.Contains(Ø))return 2;if(Õ.Contains(Ù))return 3;return 0;}void Ħ(){Ö=new MyItemType(
"MyObjectBuilder_Ingot",fueltag1);Ø=new MyItemType("MyObjectBuilder_Ingot",fueltag2);Ù=new MyItemType("MyObjectBuilder_Ingot",fueltag3);}bool ĵ
(){if(à>0){à--;return false;}à=REFRESH_INTERVAL_SECONDS*ä;Þ=0;Ý=true;return true;}private static bool ŵ(string ų,string Ŵ
){if(string.IsNullOrEmpty(ų)||string.IsNullOrEmpty(Ŵ))return false;return ų.IndexOf(Ŵ,StringComparison.OrdinalIgnoreCase)
>=0;}bool ŷ(IMyCubeGrid Ŷ){List<IMyShipConnector>V=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(V,Ť=>Ť.
IsFunctional&&Ť.Status==MyShipConnectorStatus.Connected&&Ť.OtherConnector!=null&&Ť.OtherConnector.CubeGrid==Ŷ);return V.Count>0;}
void ĺ(){switch(é){case 0:if(string.IsNullOrEmpty(fueltag1))break;ş(fueltag1);Ÿ();break;case 1:if(string.IsNullOrEmpty(
fueltag2))break;ş(fueltag2);Ź();break;case 2:if(string.IsNullOrEmpty(fueltag3))break;ş(fueltag3);ź();break;}é=(é+1)%ê;}void Ÿ(){
if(!a||P.Count==0)return;int Ż=0;double ż=0;var Ž=new HashSet<long>();var ž=Ö;ĸ($"Refill {fueltag1}:\n(All {BlockTitle1})"
,()=>{if(ā>=P.Count)ā=0;var Ŭ=P[ā];ā++;if(Ŭ==null||!Ŭ.IsFunctional)return;if(Ŭ.CubeGrid.IsStatic){ſ(Ŭ,ž,F,H,S,ref Ż);}
else{long ƀ=Ŭ.CubeGrid.EntityId;if(!Ž.Contains(ƀ)){Ž.Add(ƀ);Ɓ(Ŭ,ž,G,S);}}ż+=(double)Ŭ.GetInventory().GetItemAmount(ž);});}
void Ź(){if(!b||Q.Count==0)return;int Ƃ=0;var ƃ=new HashSet<long>();var ž=Ø;ĸ($"Refill {fueltag2}:\n(All {BlockTitle2})",()
=>{if(Ă>=Q.Count)Ă=0;var Ŭ=Q[Ă];Ă++;if(Ŭ==null||!Ŭ.IsFunctional)return;if(Ŭ.CubeGrid.IsStatic){ſ(Ŭ,ž,I,K,S,ref Ƃ);}else{
long ƀ=Ŭ.CubeGrid.EntityId;if(!ƃ.Contains(ƀ)){ƃ.Add(ƀ);Ɓ(Ŭ,ž,J,S);}}});}void ź(){if(!c||R.Count==0)return;int Ƅ=0;double ż=0
;var ƅ=new HashSet<long>();var ž=Ù;ĸ($"Refill {fueltag3}:\n(All {BlockTitle3})",()=>{if(ă>=R.Count)ă=0;var Ŭ=R[ă];ă++;if(
Ŭ==null||!Ŭ.IsFunctional)return;if(Ŭ.CubeGrid.IsStatic){ſ(Ŭ,ž,L,N,S,ref Ƅ);}else{long ƀ=Ŭ.CubeGrid.EntityId;if(!ƅ.
Contains(ƀ)){ƅ.Add(ƀ);Ɓ(Ŭ,ž,M,S);}}ż+=(double)Ŭ.GetInventory().GetItemAmount(ž);});}void ļ(){û=Ɔ(P,Ö);ü=Ɔ(Q,Ø);ý=Ɔ(R,Ù);}void Œ(
){if(!d)return;if(V.Count==0)return;if(!Ƈ(h2RefillThreshold))return;foreach(var ƈ in V){if(ƈ?.Status!=
MyShipConnectorStatus.Connected)continue;var Ŧ=ƈ.OtherConnector?.CubeGrid;if(Ŧ==null||Ɖ(Ŧ))continue;Ɗ(Ŧ,ƈ);}}void Ɗ(IMyCubeGrid Ƌ,
IMyShipConnector ƈ){Z.Clear();GridTerminalSystem.GetBlocksOfType(Z,ű=>ű.IsFunctional&&ű.CubeGrid==Ƌ);if(Z.Count==0)return;bool ƌ=true;
foreach(var ƍ in Z){if(ƍ.FilledRatio<0.95){ƍ.Stockpile=true;ƌ=false;}else{ƍ.Stockpile=false;}}foreach(var Ǝ in Y){if(Ǝ.CubeGrid
==Me.CubeGrid)Ǝ.Stockpile=false;}bool Ə=ƈ.Status==MyShipConnectorStatus.Connected;bool Ɛ=!Ƈ(h2RefillThreshold);if(Ə&&((ƌ&&
autoUndockShips)||Ɛ)){ƈ.Disconnect();}}bool Ɣ(){if(Ď.Count==0)return false;for(int œ=0;œ<Ď.Count;œ++){var Ƒ=Ď[œ];if(Ƒ==null)continue;
var Ų=Ƒ.GetInventory(0);if(Ų==null)continue;Ñ.Clear();Ų.GetItems(Ñ);for(int ƒ=0;ƒ<Ñ.Count;ƒ++){var Ɠ=Ñ[ƒ];if(Ɠ.Type.TypeId
=="MyObjectBuilder_Ore"&&Ɠ.Type.SubtypeId=="Ice")return true;}}return false;}double Ƙ(){if(Y.Count==0)return 0.0;double ƕ=
0.0;double Ɩ=0.0;for(int œ=0;œ<Y.Count;œ++){var Ɨ=Y[œ];if(Ɨ==null)continue;if(Ɨ.CubeGrid!=Me.CubeGrid)continue;ƕ+=Ɨ.
FilledRatio;Ɩ+=1.0;}if(Ɩ==0)return 0.0;return ƕ/Ɩ;}string ƛ(){double ƙ=Ƙ();int ƚ=(int)(ƙ*100);return ƚ+"%";}string Ɯ(){double ƙ=Ƙ()
;if(ƙ<0.10)return"CRITICAL";if(ƙ<0.30)return"LOW";if(ƙ<0.80)return"OK";return"FULL";}bool Ƈ(double Ɲ){if(Y.Count==0)
return false;double ƞ=0;foreach(var Ɨ in Y){if(Ɨ.CubeGrid!=Me.CubeGrid)continue;ƞ+=Ɨ.FilledRatio;}return(ƞ/Y.Count)>=Ɲ;}void ő
(){if(!e)return;if(X.Count==0||Y.Count==0)return;if(!Ɵ())return;if(Ď.Count==0)return;foreach(var Ơ in X){Ơ.
UseConveyorSystem=false;var Ų=Ơ.GetInventory();if(Ų.CurrentVolume.RawValue!=0)continue;foreach(var Ƒ in Ď){var ơ=Ƒ.GetInventory(0);if(ơ==
null)continue;Ñ.Clear();ơ.GetItems(Ñ);for(int œ=0;œ<Ñ.Count;œ++){var Ɠ=Ñ[œ];if(Ɠ.Type.TypeId=="MyObjectBuilder_Ore"&&Ɠ.Type.
SubtypeId=="Ice"){ơ.TransferItemTo(Ų,œ,null,true,iceFillAmount);goto Ƣ;}}}Ƣ:continue;}}bool Ɵ(){double ƣ=0;double Ƥ=0;foreach(var
Ɨ in Y){if(!Ɨ.IsWorking)continue;Ƥ++;ƣ+=Ɨ.FilledRatio;}if(Ƥ==0)return false;double ƥ=ƣ/Ƥ;return ƥ<0.25;}double Ɔ(List<
IMyReactor>Ʀ,MyItemType ž){double ƞ=0;foreach(var Ŕ in Ʀ){if(!Ŕ.CubeGrid.IsStatic)continue;if(Ŕ==null||!Ŕ.IsFunctional)continue;ƞ
+=(double)Ŕ.GetInventory().GetItemAmount(ž);}foreach(var Ť in S){if(!Ť.CubeGrid.IsStatic)continue;var Ų=Ť.GetInventory();
if(Ų==null)continue;ƞ+=(double)Ų.GetItemAmount(ž);}return ƞ;}void Ľ(){þ=Ƨ(P,Ö,F,H);ÿ=Ƨ(Q,Ø,I,K);Ā=Ƨ(R,Ù,L,N);}int Ƨ(List<
IMyReactor>Ʀ,MyItemType ž,double ƨ,double Ʃ){int Ɩ=0;foreach(var Ŕ in Ʀ){if(Ŕ==null||!Ŕ.IsFunctional)continue;var Ų=Ŕ.GetInventory
();if(Ų==null)continue;double ƪ=(double)Ų.GetItemAmount(ž);double ƫ=Ŕ.CubeGrid.IsStatic?Ʃ+1000:Ʃ;if(ƪ>ƫ||ƪ>=ƨ)Ɩ++;}return
Ɩ;}void Ƭ(){if(a&&û>=0){Į($"Avail. {fueltag1}: {û:F2}");Echo($"{þ} {BlockTitle1} don't need {fueltag1}.");}if(b&&ü>=0){Į(
$"Avail. {fueltag2}: {ü:F2}");Echo($"{ÿ} {BlockTitle2} don't need {fueltag2}.");}if(c&&ý>=0){Į($"Avail. {fueltag3}: {ý:F2}");Echo(
$"{Ā} {BlockTitle3} don't need {fueltag3}.");}}void ľ(){switch(ë){case 0:if(a)ƭ(P,Ö,H,REACTOR1_WARNING_TAG,Color.Yellow,fueltag1);break;case 1:if(b)ƭ(Q,Ø,K,
REACTOR2_WARNING_TAG,Color.Yellow,fueltag2);break;case 2:if(c)ƭ(R,Ù,N,REACTOR3_WARNING_TAG,Color.Yellow,fueltag3);break;}ë=(ë+1)%ì;}void ƭ(
List<IMyReactor>Ʀ,MyItemType ž,double Ʃ,string Ʈ,Color Ư,string ư){if(Ʀ==null||Ʀ.Count==0)return;int Ʊ=0;foreach(var Ŕ in Ʀ)
{if(Ŕ==null||!Ŕ.IsFunctional)continue;var Ų=Ŕ.GetInventory();if(Ų==null)continue;double Ʋ=(double)Ų.GetItemAmount(ž);
double ƫ=Ŕ.CubeGrid.IsStatic?Ʃ+50:Ʃ;if(Ʋ<=ƫ)Ʊ++;}if(Ʊ>0)Į($"{Ʊ} {ư} reactors low on fuel");List<IMyTerminalBlock>Ƴ;if(!ċ.
TryGetValue(Ʈ,out Ƴ)){Ƴ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(Ƴ,ƴ=>ƴ.CubeGrid==Me.CubeGrid&&ŵ(ƴ.
CustomName,Ʈ));ċ[Ʈ]=Ƴ;}int Ƶ=Č.ContainsKey(Ʈ)?Č[Ʈ]:-1;if(Ƶ==Ʊ)return;Č[Ʈ]=Ʊ;foreach(var ƴ in Ƴ){var ƶ=ƴ as IMyLightingBlock;if(ƶ!=
null)ƶ.Enabled=Ʊ>0;var Ʒ=ƴ as IMySoundBlock;if(Ʒ!=null){if(Ʊ>0)Ʒ.Play();else Ʒ.Stop();}}}void Ů(List<IMyReactor>Ƹ){for(int œ
=Ƹ.Count-1;œ>=0;œ--){var Ŕ=Ƹ[œ];if(Ŕ==null||Ŕ.Closed||Ŕ.CubeGrid==null)Ƹ.RemoveAt(œ);}}void ō(){if(V.Count==0)return;bool
ƹ=false;for(int œ=0;œ<V.Count;œ++){var ƺ=V[œ];if(ƺ!=null&&ƺ.Status==MyShipConnectorStatus.Connected){ƹ=true;break;}}if(!ƹ
)return;var ƻ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(ƻ,ƴ=>(ƴ is IMyCargoContainer||ƴ is
IMyShipConnector)&&ƴ.CubeGrid==Me.CubeGrid&&ƴ.CustomName.Contains(oresTag));bool Ƽ=ƻ.Count>0;foreach(var ƈ in V){if(ƈ==null)continue;var
Ŧ=ƈ.OtherConnector?.CubeGrid;if(Ŧ==null)continue;if(Ɖ(Ŧ))continue;bool ƽ=ñ.ContainsKey(ƈ)&&ñ[ƈ];if(Ƽ&&ƾ(Ŧ)&&ƈ.Status==
MyShipConnectorStatus.Connected){ƿ(Ŧ,ƻ);}if(Ƽ&&ƈ.Status!=MyShipConnectorStatus.Connected&&ƽ){ǀ(ƻ);}ñ[ƈ]=ƈ.Status==MyShipConnectorStatus.
Connected;}}bool ĭ(){if(Me.CustomName.Contains(MasterTag))return false;var V=new List<IMyShipConnector>();GridTerminalSystem.
GetBlocksOfType(V,Ť=>Ť.CubeGrid==Me.CubeGrid);foreach(var ƺ in V){if(ƺ.Status!=MyShipConnectorStatus.Connected)continue;var Ŧ=ƺ.
OtherConnector?.CubeGrid;if(Ŧ==null)continue;var ǁ=new List<IMyProgrammableBlock>();GridTerminalSystem.GetBlocksOfType(ǁ,ǂ=>ǂ.CubeGrid
==Ŧ);bool ǃ=ǁ.Any(ǂ=>ǂ.CustomName.Contains(MasterTag));if(ǃ)return true;if(Ŧ.IsStatic)return true;}return false;}int ǅ(
string Ǆ){if(Ǆ==menuTag1)return v;if(Ǆ==menuTag2)return w;if(Ǆ==menuTag3)return x;if(Ǆ==menuTagFuelRates1)return y;if(Ǆ==
menuTagFuelRates2)return z;if(Ǆ==menuTagFuelRates3)return ª;if(Ǆ==notificationMenuTag)return u;if(Ǆ==refineryMenuTag)return µ;if(Ǆ==
h2TankH2GenMenuTag)return º;if(Ǆ==resourceInfoTag)return À;return t;}void į(string ĩ){switch(q){case t:switch(ĩ.ToLower()){case"up":o=Math
.Max(0,o-1);break;case"down":o=Math.Min(s.Count-1,o+1);break;case"apply":var ǆ=s[o];if(ǆ==sortingMenuTag){if(đ<=0){İ(
"Sort Inventory",ė);int Ǉ=ı(sortingMenuTag,Ĳ);đ=10.0;string ǈ=$"Sorting Complete: {Ǉ} items";if(!string.IsNullOrEmpty(Ä))ǈ+="\n⚠ "+Ä;ǉ(ǈ
,6);p=0;ł();}else{ǉ($"Sorting on cooldown: {Math.Ceiling(đ)}s",1);ł();}}else{q=ǅ(ǆ);p=0;ł();}break;}break;case u:switch(ĩ
.ToLower()){case"up":if(r==-1)p=Math.Max(0,p-1);break;case"down":if(r==-1)p=Math.Min(1,p+1);break;case"apply":if(r==-1){
if(p==0)Ǌ();else if(p==1)q=t;r=-1;p=0;}else r=-1;ł();break;}break;case v:ǋ(ĩ,ref a,4);break;case w:ǋ(ĩ,ref b,4);break;case
x:ǋ(ĩ,ref c,4);break;case y:if(ĩ.ToLower()=="apply"){q=t;p=0;ł();}break;case z:if(ĩ.ToLower()=="apply"){q=t;p=0;ł();}
break;case ª:if(ĩ.ToLower()=="apply"){q=t;p=0;ł();}break;case µ:switch(ĩ.ToLower()){case"apply":if(Ï==-1){if(p==0)j=!j;else
if(p==1)k=!k;else if(p==Ë.Count+2){q=t;p=0;}else Ï=p-2;}else Ï=-1;Ì=true;ł();break;case"up":if(Ï!=-1){if(Ï>0){var ǌ=Ë[Ï-1]
;Ë[Ï-1]=Ë[Ï];Ë[Ï]=ǌ;Ï--;p=Ï+2;Ǎ();}}else p=Math.Max(0,p-1);ł();break;case"down":if(Ï!=-1){if(Ï<Ë.Count-1){var ǌ=Ë[Ï+1];Ë[
Ï+1]=Ë[Ï];Ë[Ï]=ǌ;Ï++;p=Ï+2;Ǎ();}}else p=Math.Min(Ë.Count+2,p+1);ł();break;}break;case º:switch(ĩ.ToLower()){case"up":p=
Math.Max(0,p-1);ł();break;case"down":p=Math.Min(3,p+1);ł();break;case"apply":if(p==0)d=!d;else if(p==1)e=!e;else if(p==2)f=!
f;else if(p==3){q=t;p=0;}ł();break;}break;case À:{int ǎ=(Ĝ==0)?Ğ:ğ;if(Ĝ<0)Ĝ=0;if(Ĝ>1)Ĝ=1;p=Math.Max(0,Math.Min(p,ǎ-1));
switch(ĩ.ToLower()){case"up":p=(p-1+ǎ)%ǎ;ł();break;case"down":p=(p+1)%ǎ;ł();break;case"left":if(Ĝ>0){Ĝ--;p=0;}ł();break;case
"right":if(Ĝ<1){Ĝ++;p=0;}ł();break;case"apply":if(Ĝ==0){switch(p){case 0:m=!m;break;case 1:n=!n;break;case 2:l=!l;break;case 3:
q=t;p=0;Ĝ=0;break;}}else{q=t;p=0;Ĝ=0;}ł();break;}break;}}}void ǋ(string ĩ,ref bool Ǐ,int ǐ){switch(ĩ.ToLower()){case"up":
if(r!=-1)Ǒ(1);else p=Math.Max(0,p-1);break;case"down":if(r!=-1)Ǒ(-1);else p=Math.Min(ǐ,p+1);break;case"apply":if(r==-1){if
(p==0){Ǐ=!Ǐ;Ǎ();ĥ();q=t;o=0;p=0;r=-1;ł();break;}else if(p==ǐ){q=t;r=-1;p=0;ł();break;}else{r=p;}}else{r=-1;}ł();break;}}
void ĸ(string ǒ,Action Ǔ){const long ǔ=45000;const int Ǖ=10;if(Runtime.CurrentInstructionCount>ǔ||Runtime.
CurrentCallChainDepth>Ǖ)return;long ǖ=DateTime.UtcNow.Ticks;long Ǘ=Runtime.CurrentInstructionCount;Ǔ();long ǘ=DateTime.UtcNow.Ticks;long Ǚ=
Runtime.CurrentInstructionCount;double ǚ=(ǘ-ǖ)/10000.0;long Ǜ=Ǚ-Ǘ;if(ǚ>2.8){string ǜ=
$"{DateTime.UtcNow}: {ǒ} - {ǚ:F3} ms - InstrDelta: {Ǜ} (s:{Ǘ},e:{Ǚ}) - Call Depth: {Runtime.CurrentCallChainDepth}";string ǝ=Me.CustomData??string.Empty;Me.CustomData=$"{ǝ}\n{ǜ}\n---";}List<double>Ƹ;if(!Ò.TryGetValue(ǒ,out Ƹ)){Ƹ=new
List<double>();Ò[ǒ]=Ƹ;}Ƹ.Add(ǚ);if(Ƹ.Count>å)Ƹ.RemoveAt(0);}void Ń(string ǒ,Action Ǔ){const long ǔ=45000;const int Ǖ=10;if(
Runtime.CurrentInstructionCount>ǔ||Runtime.CurrentCallChainDepth>Ǖ)return;long ǖ=DateTime.UtcNow.Ticks;long Ǘ=Runtime.
CurrentInstructionCount;Ǔ();long ǘ=DateTime.UtcNow.Ticks;long Ǚ=Runtime.CurrentInstructionCount;double ǚ=(ǘ-ǖ)/10000.0;long Ǜ=Ǚ-Ǘ;if(ǚ>2.5){
string ǜ=$"{DateTime.UtcNow}: {ǒ} - {ǚ:F3} ms - InstrDelta: {Ǜ} (s:{Ǘ},e:{Ǚ}) - Call Depth: {Runtime.CurrentCallChainDepth}";
string ǝ=Me.CustomData??string.Empty;Me.CustomData=$"{ǝ}\n{ǜ}\n---";}List<double>Ƹ;if(!Ó.TryGetValue(ǒ,out Ƹ)){Ƹ=new List<
double>();Ó[ǒ]=Ƹ;}Ƹ.Add(ǚ);if(Ƹ.Count>å)Ƹ.RemoveAt(0);}void Ǣ(){Į("------------------------------------",true);Į(
"=== PROFILER RESULTS ===",true);var Ǟ=new List<KeyValuePair<string,double>>();foreach(var Ǡ in Ò){double ǟ=0;foreach(var ǡ in Ǡ.Value)ǟ+=ǡ;double
ƥ=ǟ/Ǡ.Value.Count;Ǟ.Add(new KeyValuePair<string,double>(Ǡ.Key,ƥ));}Ǟ.Sort((ů,ƴ)=>ƴ.Value.CompareTo(ů.Value));foreach(var
Ŕ in Ǟ)Į($"{Ŕ.Key}: {Ŕ.Value:F3} ms avg");}void ǥ(){Į("------------------------------------",true);Į(
$"=== BLOCK REFRESH === Refresh Interval: {REFRESH_INTERVAL_SECONDS}",true);string[]ǣ={"Refresh Cargo Containers","Refresh Refineries","Refresh Reactors","Refresh LCDs","Refresh Connectors"
,"Refresh Assemblers"};foreach(var Ǥ in ǣ){List<double>Ƹ;if(!Ó.TryGetValue(Ǥ,out Ƹ)||Ƹ.Count==0)continue;double ǟ=0;for(
int œ=0;œ<Ƹ.Count;œ++)ǟ+=Ƹ[œ];double ƥ=ǟ/Ƹ.Count;Į($"{Ǥ}: {ƥ:F3} ms avg",true);}Į("",true);}Ǧ ı<Ǧ>(string ǒ,Func<Ǧ>ǧ){
double Ǩ=Runtime.LastRunTimeMs;Ǧ ǩ=ǧ();double Ǫ=Runtime.LastRunTimeMs;Į($"{ǒ}: {(Ǫ-Ǩ):F2} ms");return ǩ;}void Ĵ(int Ʋ){var T=
new List<IMyTextPanel>();GridTerminalSystem.GetBlocksOfType(T,ƴ=>ƴ.CubeGrid.EntityId==Me.CubeGrid.EntityId&&ƴ.CustomName.
Contains("[SORT]"));foreach(var ǫ in T){ǫ.WriteText("");var Ř=new StringBuilder();ǫ.ContentType=VRage.Game.GUI.TextPanel.
ContentType.TEXT_AND_IMAGE;ǫ.Alignment=TextAlignment.CENTER;ǫ.FontSize=1f;string Ǭ=$"Sorting Complete {Ʋ} Items moved";ǫ.WriteText(
Ǭ);}}void ǉ(string Ǭ,int ǭ=5){Â=Ǭ;Ã=DateTime.Now.AddSeconds(ǭ);}void Ĺ(){switch(č){case 0:ş("Collect Blocks");Ď.Clear();ď
.Clear();Đ.Clear();GridTerminalSystem.GetBlocksOfType(Ď,Ť=>Ť.CubeGrid==Me.CubeGrid&&ŵ(Ť.CustomName,oresTag));ď.AddRange(U
);č++;break;case 1:ş("Scan Cargo");Ǯ(Ď);č++;break;case 2:ş("Scan Refineries");ǯ(ď);č++;break;case 3:ş("Apply Results");ǰ(
);č=0;break;}}void Ǯ(List<IMyCargoContainer>Ǳ){foreach(var Ť in Ǳ){var Ų=Ť.GetInventory();if(Ų==null)continue;Ñ.Clear();Ų
.GetItems(Ñ);foreach(var Ɠ in Ñ){if(Ɠ.Type.TypeId!="MyObjectBuilder_Ore")continue;string ŕ=Ɠ.Type.SubtypeId;if(ŕ=="Ice"||
ŕ.Contains("Peat")||ŕ.Contains("Seeds"))continue;Đ.Add(ŕ);}}}void ǯ(List<IMyRefinery>ǲ){for(int œ=0;œ<ǲ.Count;œ++){var Ţ=
ǲ[œ];if(Ţ==null||Ţ.Closed)continue;var Ų=Ţ.InputInventory;if(Ų==null)continue;Ñ.Clear();Ų.GetItems(Ñ);for(int ƒ=0;ƒ<Ñ.
Count;ƒ++){var Ɠ=Ñ[ƒ];if(Ɠ.Type.TypeId!="MyObjectBuilder_Ore")continue;string ǳ=Ɠ.Type.SubtypeId;if(ǳ=="Ice")continue;if(ǳ==
"RebelComponentScrap")continue;if(ǳ.Contains("Peat"))continue;if(ǳ.Contains("Seeds"))continue;Đ.Add(ǳ);}}}void ǰ(){bool Ǵ=false;Ë.RemoveAll(ǵ
=>{bool Ƕ=Đ.Contains(ǵ);if(!Ƕ)Ǵ=true;return!Ƕ;});foreach(var ǵ in Đ){if(!Ë.Contains(ǵ)){Ë.Add(ǵ);Ǵ=true;}}if(Ǵ){Ǎ();Ì=true
;}}void Ǒ(double Ƿ){double Ǹ=50;double ǹ=Ǹ*Ƿ;if(q==v){switch(r){case 1:F=Math.Max(50,F+ǹ);break;case 2:G=Math.Max(50,G+ǹ)
;break;case 3:H=Math.Max(50,H+ǹ);break;}Ǎ();}if(q==w){switch(r){case 1:I=Math.Max(50,I+ǹ);break;case 2:J=Math.Max(50,J+ǹ)
;break;case 3:K=Math.Max(50,K+ǹ);break;}Ǎ();}if(q==x){switch(r){case 1:L=Math.Max(50,L+ǹ);break;case 2:M=Math.Max(50,M+ǹ)
;break;case 3:N=Math.Max(50,N+ǹ);break;}Ǎ();}ł();}void Ǎ(){var Š=new StringBuilder();Š.AppendLine("[ORES]");Š.AppendLine(
string.Join(",",Ë));Š.AppendLine("[SETTINGS]");Š.AppendLine(string.Join(";",$"{F}|{G}|{H}",$"{I}|{J}|{K}",$"{L}|{M}|{N}"));
Storage=Š.ToString();}void Ĥ(){if(string.IsNullOrWhiteSpace(Storage)){Ǻ();}else{var ǻ=Storage.Split('\n');string Ǽ="";foreach(
var ǽ in ǻ){var Ǿ=ǽ.Trim();if(string.IsNullOrWhiteSpace(Ǿ))continue;if(Ǿ.StartsWith("[")&&Ǿ.EndsWith("]")){Ǽ=Ǿ;continue;}if
(Ǽ=="[ORES]"){Ë=new List<string>(Ǿ.Split(new[]{','},StringSplitOptions.RemoveEmptyEntries));}else if(Ǽ=="[SETTINGS]"){var
ǿ=Ǿ.Split(new[]{';'},StringSplitOptions.RemoveEmptyEntries);if(ǿ.Length<3)continue;Ȁ(ǿ[0],ref F,ref G,ref H);Ȁ(ǿ[1],ref I
,ref J,ref K);Ȁ(ǿ[2],ref L,ref M,ref N);}}ȁ();}}void Ǻ(){F=2000;G=1000;H=500;I=2000;J=1000;K=500;L=2000;M=1000;N=500;}
void ȁ(){if(F<=0||G<=0||H<=0||I<=0||J<=0||K<=0||L<=0||M<=0||N<=0){Ǻ();}}void Ȁ(string Ȃ,ref double ů,ref double ƴ,ref double
Ť){var ȃ=Ȃ.Split('|');if(ȃ.Length!=3)return;double Ȅ,ȅ,Ȇ;if(double.TryParse(ȃ[0],out Ȅ))ů=Ȅ;if(double.TryParse(ȃ[1],out ȅ
))ƴ=ȅ;if(double.TryParse(ȃ[2],out Ȇ))Ť=Ȇ;}void ȉ(StringBuilder Ř,string ȇ,int p,int Ȉ){Ř.AppendLine(
$"{(p==Ȉ?"> ":"")}{ȇ}");}void Ȋ(){ġ++;if(ġ<Ģ&&T.Count>0)return;ġ=0;T.Clear();Ġ.Clear();Ũ(Ġ);GridTerminalSystem.GetBlocksOfType(T,Ū=>Ū.
IsFunctional&&Ū.CustomName.Contains(LCDTAG)&&Ġ.Contains(Ū.CubeGrid));}void ȋ(IMyTextPanel ǫ){ǫ.ContentType=ContentType.
TEXT_AND_IMAGE;ǫ.Font="Monospace";ǫ.FontSize=0.5f;ǫ.FontColor=Color.Teal;ǫ.Alignment=TextAlignment.LEFT;}void ł(){if(!ĉ){ĥ();ĉ=true;}
if(!A){Ĭ();return;}Ȋ();if(T.Count==0)return;for(int œ=0;œ<T.Count;œ++){var ǫ=T[œ];ȋ(ǫ);ģ.Clear();switch(q){case t:Ȍ(ģ);
break;case u:ȍ(ģ);break;case v:Ȏ(ģ);break;case w:ȏ(ģ);break;case x:Ȑ(ģ);break;case y:ȑ(ģ);break;case z:Ȓ(ģ);break;case ª:ȓ(ģ)
;break;case µ:Ȕ(ģ);break;case º:ȕ(ģ);break;case À:Ȗ(ģ);break;default:Ȍ(ģ);break;}ǫ.WriteText(ģ.ToString(),false);}}void Ȍ
(StringBuilder Š){Š.AppendLine("Grid Manager");Š.AppendLine("================================================");int ȗ,Ș,ș
,Ț,ț,Ȝ;ȝ(out ȗ,out Ș,out ș,out Ț,out ț,out Ȝ);if(ȗ+Ș+ș+Ț+ț+Ȝ>0){Š.AppendLine("Connected Grids:");if(ȗ>0)Š.AppendLine(
$"  Miners:   {ȗ}");if(Ș>0)Š.AppendLine($"  Grinders: {Ș}");if(ș>0)Š.AppendLine($"  Welders:  {ș}");if(Ț>0)Š.AppendLine($"  Combat:   {Ț}"
);if(ț>0)Š.AppendLine($"  General:  {ț}");if(Ȝ>0)Š.AppendLine($"  Station:  {Ȝ}");Š.AppendLine(
"================================================");}if(!string.IsNullOrEmpty(Â)&&DateTime.Now<Ã){Š.AppendLine($"*** {Â} ***");Š.AppendLine(
"================================================");}for(int œ=0;œ<s.Count;œ++)Š.AppendLine($"{(o==œ?"> ":"  ")}{s[œ]}");Š.AppendLine(
"================================================");}void ȍ(StringBuilder Š){ȉ(Š,"Reset to default values",p,0);ȉ(Š,"Return to Main Menu",p,1);Š.AppendLine();š(Š);ř(Š);Ȟ(
);Ƭ();ǥ();Ǣ();for(int œ=0;œ<Á.Count;œ++)Š.AppendLine(Á[œ]);Á.Clear();}void Ȏ(StringBuilder Š){Š.AppendLine(
$"{fueltag1} Reactor Handling");Š.AppendLine("-------------------------------------------------");string ȟ=p==0?"> ":"  ";Š.AppendLine(
$"{ȟ}{menuTag1}: {(a?"ON":"OFF")}");ȉ(Š,$"Station Fuel Amount: {F}",p,1);ȉ(Š,$"Large Ship Fuel Amount: {G}",p,2);ȉ(Š,$"Alarm Threshold: {H}",p,3);ȉ(Š,
"Return to Main Menu",p,4);}void ȏ(StringBuilder Š){Š.AppendLine($"{fueltag2} Reactor Handling");Š.AppendLine(
"-------------------------------------------------");string ȟ=p==0?"> ":"  ";Š.AppendLine($"{ȟ}{menuTag2}: {(b?"ON":"OFF")}");ȉ(Š,$"Station Reactor Fuel Amount: {I}",p,1);
ȉ(Š,$"Large Ship Large Reactor Fuel Amount: {J}",p,2);ȉ(Š,$"Reactor Alarm Low Fuel: {K}",p,3);ȉ(Š,"Return to Main Menu",p
,4);}void Ȑ(StringBuilder Š){Š.AppendLine($"{fueltag3} Reactor Handling");Š.AppendLine(
"-------------------------------------------------");string ȟ=p==0?"> ":"  ";Š.AppendLine($"{ȟ}{menuTag3}: {(c?"ON":"OFF")}");ȉ(Š,$"Station Ice Engine Fuel Amount: {L}",p,
1);ȉ(Š,$"Large Ship Ice Engine Fuel Amount: {M}",p,2);ȉ(Š,$"Ice Engine Alarm Low Fuel: {N}",p,3);ȉ(Š,
"Return to Main Menu",p,4);}void ȑ(StringBuilder Š){if(P.Count==0){Š.AppendLine("No Reactors Found");return;}string ū=Ƞ(P[0].BlockDefinition.
SubtypeName);double ȡ=0;double Ȣ=0;long ȣ=0;foreach(var Ŕ in P){string ȥ=Ȥ(Ŕ,Ŕ.CurrentOutput);double Ȧ,ȧ;string Î;long ǭ;Ȩ(ȥ,out Ȧ,
out Î,out ȧ,out ǭ);ȡ+=ȧ;Ȣ+=Ȧ;ȣ+=ǭ;}Š.AppendLine($"Reactor Type: {ū}");Š.AppendLine($"Count: {P.Count}");Š.AppendLine(
$"Total Output: {ȡ:F2} MW");Š.AppendLine($"Consumption: {Ȣ:F3} {Î}");Š.AppendLine();Š.AppendLine($"{(p==0?"> ":"  ")}Return to Main Menu");}void Ȓ
(StringBuilder Š){if(Q.Count==0){Š.AppendLine("No Reactors Found");return;}string ȩ=Ƞ(Q[0].BlockDefinition.SubtypeName);
double ȡ=0;double Ȣ=0;long ȣ=0;foreach(var Ŭ in Q){string Ť=Ȫ(Ŭ,Ŭ.CurrentOutput);double Ȧ,ȧ;string Î;long ǭ;Ȩ(Ť,out Ȧ,out Î,
out ȧ,out ǭ);ȡ+=ȧ;Ȣ+=Ȧ;ȣ+=ǭ;}int ȫ=(int)(ȣ/(7*24*3600));int Ȭ=(int)((ȣ%(7*24*3600))/(24*3600));int ȭ=(int)((ȣ%(24*3600))/
3600);string Ȯ=$"{ȫ}w {Ȭ}d {ȭ}h";Š.AppendLine($"Reactor Type: {ȩ}");Š.AppendLine($"Count: {Q.Count}");Š.AppendLine(
$"Total Output: {ȡ:F2} MW");Š.AppendLine($"Total Consumption Rate: {Ȣ:F3} {Î}");Š.AppendLine($"Total Fuel Time Left: {Ȯ}");Š.AppendLine();Š.
AppendLine($"{(p==0?"> ":"  ")}Return to Main Menu");}void ȓ(StringBuilder Š){if(R.Count==0){Š.AppendLine("No Reactors Found");
return;}string ȩ=Ƞ(R[0].BlockDefinition.SubtypeName);double ȡ=0;double Ȣ=0;long ȣ=0;foreach(var Ŭ in R){string Ť=ȯ(Ŭ,Ŭ.
CurrentOutput);double Ȧ,ȧ;string Î;long ǭ;Ȩ(Ť,out Ȧ,out Î,out ȧ,out ǭ);ȡ+=ȧ;Ȣ+=Ȧ;ȣ+=ǭ;}int ȫ=(int)(ȣ/(7*24*3600));int Ȭ=(int)((ȣ%(7*
24*3600))/(24*3600));int ȭ=(int)((ȣ%(24*3600))/3600);string Ȯ=$"{ȫ}w {Ȭ}d {ȭ}h";Š.AppendLine($"Reactor Type: {ȩ}");Š.
AppendLine($"Count: {R.Count}");Š.AppendLine($"Total Output: {ȡ:F2} MW");Š.AppendLine($"Total Consumption Rate: {Ȣ:F3} {Î}");Š.
AppendLine($"Total Fuel Time Left: {Ȯ}");Š.AppendLine();Š.AppendLine($"{(p==0?"> ":"  ")}Return to Main Menu");}void Ȕ(
StringBuilder Š){Š.AppendLine("Refinery Ore Management");Š.AppendLine("-------------------------------------------------");Š.
AppendLine($"Mode: {(j?"Priority":k?"Name-based":"Auto")}");if(Ë.Count>0)Š.AppendLine($"Top Priority: {Ë[0]}");Š.AppendLine(
"-------------------------------------------------");Š.AppendLine($"{(p==0?"> ":"  ")}Priority Refining: {(j?"ON":"OFF")}");Š.AppendLine(
$"{(p==1?"> ":"  ")}Name-based Refining: {(k?"ON":"OFF")}");for(int œ=0;œ<Ë.Count;œ++)Š.AppendLine($"{(p==œ+2?"> ":"  ")}{œ+1}. {Ë[œ]}");Š.AppendLine(
$"{(p==Ë.Count+2?"> ":"  ")}Return to Main Menu");}void ȕ(StringBuilder Š){Š.AppendLine($"{h2TankH2GenMenuTag} Control");Š.AppendLine(
"--------------------------------------------");Š.AppendLine("H2 Management Options:");Š.AppendLine();Š.AppendLine(
$"{(p==0?"> ":"  ")}H2 Ship Refueling: {(d?"ON":"OFF")}");Š.AppendLine($"{(p==1?"> ":"  ")}H2 Generator/Tank Handling: {(e?"ON":"OFF")}");Š.AppendLine(
$"{(p==2?"> ":"  ")}Auto Undock Ships: {(f?"ON":"OFF")}");Š.AppendLine();Š.AppendLine($"{(p==3?"> ":"  ")}Return to Main Menu");Š.AppendLine();Š.AppendLine(
"--------------------------------------------");Š.AppendLine($"Base H2 Level: {ƛ()}   [{Ɯ()}]");bool Ȱ=Ɣ();bool ȱ=Ƈ(h2RefillThreshold);Š.AppendLine(
$"Ice Supply: {(Ȱ?"Available":"MISSING!")}");Š.AppendLine($"Can Refuel: {(ȱ?"YES":"NO")}");}void Ȗ(StringBuilder Š){Š.AppendLine("Resource Overview");Š.AppendLine(
"-------------------------------------------------");var ȳ=Ȳ(ingotsTag);var ȴ=Ȳ(oresTag);if(ȳ.Count==0&&ȴ.Count==0){Š.AppendLine("No ore or ingots found.");}else{ȵ(Š,ȳ,ȴ,
"INGOTS","ORES",Ĝ,ĝ);}Š.AppendLine("-------------------------------------------------");if(Ĝ==0){Š.AppendLine(
"Select Inventory Management Option:");Š.AppendLine($"{(p==0?"> ":"  ")}Balance Items: {(m?"ON":"OFF")}");Š.AppendLine(
$"{(p==1?"> ":"  ")}Group Identical Stacks: {(n?"ON":"OFF")}");Š.AppendLine($"{(p==2?"> ":"  ")}Priority Sorting: {(l?"ON":"OFF")}");Š.AppendLine(
$"{(p==3?"> ":"  ")}Return to Main Menu");}else{Š.AppendLine($"{(p==0?"> ":"  ")}Return to Main Menu");}}void ȵ(StringBuilder Š,Dictionary<string,MyFixedPoint>ȳ
,Dictionary<string,MyFixedPoint>ȴ,string ȶ,string ȷ,int ȸ,int ȹ){const int Ⱥ=14;const int Ȼ=12;Š.AppendLine("RESOURCE".
PadRight(Ⱥ)+"|"+ȶ.PadRight(Ȼ)+"|"+ȷ.PadRight(Ȼ));Š.AppendLine(new string('-',Ⱥ)+"-+-"+new string('-',Ȼ)+"-+-"+new string('-',Ȼ))
;var ȼ=new HashSet<string>(ȳ.Keys,StringComparer.OrdinalIgnoreCase);ȼ.UnionWith(ȴ.Keys);var Ⱦ=ȼ.OrderBy(Ƚ=>Ƚ).ToList();
int ȿ=Ⱦ.Count;int ɀ=Math.Max(1,(ȿ+ȹ-1)/ȹ);int Ɂ=Math.Max(0,Math.Min(ȸ,ɀ-1));if(Ɂ!=ȸ)Ĝ=Ɂ;int ɂ=Ɂ*ȹ;int Ƀ=Math.Min(ɂ+ȹ,ȿ);for
(int œ=ɂ;œ<Ƀ;œ++){string Ǥ=Ⱦ[œ];bool Ʌ=Ʉ(Ǥ);string ɇ=ȳ.ContainsKey(Ǥ)?Ɇ(ȳ[Ǥ]):"-";string Ɉ=ȴ.ContainsKey(Ǥ)?Ɇ(ȴ[Ǥ]):"-";
if(Ʌ)Ɉ+=" (Fuel)";Š.AppendLine(Ǥ.PadRight(Ⱥ)+"|"+ɇ.PadRight(Ȼ)+"|"+Ɉ.PadRight(Ȼ));}Š.AppendLine(
"-------------------------------------------------");Š.AppendLine($"Page {Ɂ+1}/{ɀ}"+" "+"Use left/right to turn page");}bool Ʉ(string ɉ){return ɉ.Equals(fueltag1,
StringComparison.OrdinalIgnoreCase)||ɉ.Equals(fueltag2,StringComparison.OrdinalIgnoreCase)||ɉ.Equals(fueltag3,StringComparison.
OrdinalIgnoreCase);}string Ɇ(MyFixedPoint Ʋ){double Ŵ=(double)Ʋ;if(Ŵ>=1000000)return(Ŵ/1000000d).ToString("0.0")+"M";if(Ŵ>=1000)return(Ŵ/
1000d).ToString("0.0")+"K";return Ŵ.ToString("0");}string Ƞ(string ǳ){if(string.IsNullOrWhiteSpace(ǳ))return"Unknown Reactor"
;string Ɋ=System.Text.RegularExpressions.Regex.Replace(ǳ,"(?<!^)([A-Z])"," $1");Ɋ=System.Text.RegularExpressions.Regex.
Replace(Ɋ,@"\s+"," ");return Ɋ.Trim();}void Ȩ(string ɋ,out double Ȧ,out string Î,out double ȧ,out long Ɍ){Ȧ=0;Î="g/s";ȧ=0;Ɍ=0;
var ǻ=ɋ.Split('\n');foreach(var Ǿ in ǻ){if(Ǿ.Contains("Consumption Rate:")){var ȃ=Ǿ.Split(' ');double.TryParse(ȃ[3],out Ȧ);
Î=ȃ[4];}else if(Ǿ.StartsWith("Output:")){var ȃ=Ǿ.Split(' ');double.TryParse(ȃ[1],out ȧ);}else if(Ǿ.StartsWith("Est:")){
string ɍ=Ǿ.Substring(5).Trim();Ɍ=Ɏ(ɍ);}}}long Ɏ(string ɍ){long ȫ=0,Ȭ=0,ȭ=0,ɏ=0,ǭ=0;var ȃ=ɍ.Split(' ');foreach(var ɐ in ȃ){if(ɐ
.EndsWith("w"))long.TryParse(ɐ.TrimEnd('w'),out ȫ);else if(ɐ.EndsWith("d"))long.TryParse(ɐ.TrimEnd('d'),out Ȭ);else if(ɐ.
EndsWith("h"))long.TryParse(ɐ.TrimEnd('h'),out ȭ);else if(ɐ.EndsWith("m"))long.TryParse(ɐ.TrimEnd('m'),out ɏ);else if(ɐ.EndsWith
("s"))long.TryParse(ɐ.TrimEnd('s'),out ǭ);}return ȫ*7*24*3600+Ȭ*24*3600+ȭ*3600+ɏ*60+ǭ;}void ȝ(out int Ō,out int ɑ,out int
ɒ,out int ɓ,out int ɔ,out int ɕ){Ō=0;ɑ=0;ɒ=0;ɓ=0;ɔ=0;ɕ=0;var V=new List<IMyShipConnector>();GridTerminalSystem.
GetBlocksOfType(V,Ť=>Ť.CubeGrid==Me.CubeGrid);foreach(var ƈ in V){if(ƈ.Status!=MyShipConnectorStatus.Connected)continue;var Ŧ=ƈ.
OtherConnector?.CubeGrid;if(Ŧ==null)continue;if(Ŧ.IsStatic){ɕ++;}else if(ƾ(Ŧ)){Ō++;}else if(ɖ(Ŧ)){ɑ++;}else if(ɗ(Ŧ)){ɒ++;}else if(ɘ(Ŧ)
&&!Ŧ.IsStatic){ɓ++;}else{ɔ++;}}}int ŋ(){int Ɩ=0;var V=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(V,Ť
=>Ť.CubeGrid==Me.CubeGrid);foreach(var ƈ in V){if(ƈ.Status!=MyShipConnectorStatus.Connected)continue;var Ŧ=ƈ.
OtherConnector?.CubeGrid;if(Ŧ==null||Ŧ.IsStatic)continue;if(ƾ(Ŧ))Ɩ++;}return Ɩ;}void Ǌ(){F=1000;G=500;H=1000;I=1000;J=500;K=1000;L=
1000;M=500;N=500;Ǎ();}void Į(string Ǭ,bool ə=false){base.Echo(Ǭ);if(!ə&&q!=u)return;if(Á.Count>0&&Á[Á.Count-1].EndsWith(Ǭ))
return;Á.Add("- "+Ǭ);}void Ļ(){if(W.Count==0)return;var ɚ=new List<IMyCargoContainer>();GridTerminalSystem.GetBlocksOfType(ɚ,Ť
=>Ť.CubeGrid==Me.CubeGrid&&Ť.CustomName.Contains(ingotsTag));if(ɚ.Count==0)return;foreach(var ɛ in W){var Ȃ=ɛ.
InputInventory;Ñ.Clear();Ȃ.GetItems(Ñ);for(int œ=Ñ.Count-1;œ>=0;œ--){var Ɠ=Ñ[œ];double Ʋ=(double)Ɠ.Amount;if(Ʋ<=
ASSEMBLER_INPUT_MAX_AMOUNT)continue;double ɜ=Ʋ-ASSEMBLER_INPUT_MAX_AMOUNT;ɝ(Ȃ,Ɠ,ɜ,ɚ);}}}void ɝ(IMyInventory ɞ,MyInventoryItem Ɠ,double ɟ,List<
IMyCargoContainer>ɚ){if(ɟ<=0)return;MyFixedPoint ɠ=(MyFixedPoint)ɟ;foreach(var ɡ in ɚ){var ɢ=ɡ.GetInventory();if(!ɢ.CanItemsBeAdded(ɠ,Ɠ.
Type))continue;bool ɣ=ɞ.TransferItemTo(ɢ,Ɠ,(MyFixedPoint?)ɠ);if(ɣ)return;}}Dictionary<string,MyFixedPoint>Ȳ(string ɤ){var ɥ=
new Dictionary<string,MyFixedPoint>(StringComparer.OrdinalIgnoreCase);var Ǳ=new List<IMyCargoContainer>();
GridTerminalSystem.GetBlocksOfType(Ǳ,Ť=>Ť.IsFunctional&&Ť.CustomName.Contains(ɤ)&&Ť.CubeGrid==Me.CubeGrid);foreach(var Ť in Ǳ){var Ų=Ť.
GetInventory();Ċ.Clear();Ų.GetItems(Ċ);foreach(var Ɠ in Ċ){string ɧ=ɦ(Ɠ.Type.SubtypeId);MyFixedPoint ɨ;ɥ.TryGetValue(ɧ,out ɨ);ɥ[ɧ]=ɨ
+Ɠ.Amount;}}return ɥ;}string ɦ(string ǳ){if(string.IsNullOrEmpty(ǳ))return ǳ;int Ç=ǳ.IndexOf("Ingot",StringComparison.
OrdinalIgnoreCase);if(Ç>=0)ǳ=ǳ.Substring(0,Ç);var ɩ=ǳ.Split(new[]{' ','_'},StringSplitOptions.RemoveEmptyEntries);return ɩ.Length>0?ɩ[0]:
ǳ;}void Ŏ(){switch(ï){case 0:Ł();break;case 1:if(ò.Count>0)ɪ();break;}ï=(ï+1)%ð;}void Ł(){int ɫ=0;int ɬ=0;int ɭ=0;int ɮ=0
;ɯ();if(S==null||S.Count==0)ń();var ɱ=ɰ();int ɳ=ɲ();bool ɴ=ɳ!=Í;foreach(var Ţ in U){if(Ţ==null)continue;ɵ(Ţ);if(Ţ.
CustomName.Contains(manualTag)){ɶ(Ţ);continue;}if(!Ţ.Enabled){ɮ++;ɶ(Ţ);continue;}var ś=ɷ(Ţ,ɱ);var Ǆ=ɸ(Ţ,ɱ);var Ǔ=ɹ(Ţ,ś,Ǆ,ɴ);switch
(Ǔ){case ɺ.ɻ:if(k&&!j)ɬ++;ɼ(Ţ,ɱ);ɽ(Ţ);break;case ɺ.ɾ:ɿ(Ţ,ɱ);break;case ɺ.ʀ:ʁ(Ţ,ɱ);break;case ɺ.ʂ:if(Ţ.IsProducing)ɫ++;
break;case ɺ.ʃ:ɭ++;break;}ɶ(Ţ);}Ą=ɫ;ą=ɬ;Ć=ɭ;ć=ɮ;Í=ɳ;}enum ɺ{ʂ,ɻ,ɾ,ʀ,ʃ}ɺ ɹ(IMyRefinery Ţ,ʄ ś,ʅ Ǆ,bool ɴ){bool ʆ=Ţ.
InputInventory.ItemCount>0;if(ś.ʇ){if(ɴ&&ʆ)return ɺ.ɻ;if(ś.ʈ)return ɺ.ɻ;if(ʆ&&ś.ʉ!=null&&!ʊ(Ţ.InputInventory.GetItemAt(0).Value.Type.
SubtypeId,ś.ʉ))return ɺ.ɻ;if(!ʆ)return ɺ.ɾ;return ɺ.ʂ;}if(Ǆ.ʇ){if(!Ǆ.ʋ)return ɺ.ʃ;if(!ʆ)return ɺ.ʀ;if(!Ǆ.ʌ)return ɺ.ɻ;return ɺ.ʂ;
}return ɺ.ʂ;}struct ʅ{public bool ʇ;public List<string>ʍ;public string ʉ;public bool ʋ;public bool ʌ;public bool ʎ;}ʅ ɸ(
IMyRefinery Ţ,List<IMyCargoContainer>ɱ){var ʏ=new ʅ{ʇ=k,ʍ=new List<string>(),ʉ=null,ʋ=false,ʌ=false,ʎ=false};if(!k)return ʏ;ʏ.ʍ=ʐ(Ţ
);if(ʏ.ʍ.Count==0)return ʏ;for(int œ=0;œ<ʏ.ʍ.Count;œ++){string ʑ=ʏ.ʍ[œ];if(ʒ(Ţ,ɱ,ʑ)){ʏ.ʉ=ʑ;ʏ.ʋ=true;break;}}if(!ʏ.ʋ)
return ʏ;if(Ţ.InputInventory.ItemCount==0)return ʏ;var ʓ=Ţ.InputInventory.GetItemAt(0);if(!ʓ.HasValue)return ʏ;string ʔ=ʓ.
Value.Type.SubtypeId;ʏ.ʌ=ʏ.ʍ.Any(ǵ=>ʊ(ǵ,ʔ));ʏ.ʎ=ʊ(ʏ.ʉ,ʔ);return ʏ;}struct ʄ{public bool ʇ;public bool ʈ;public string ʉ;}ʄ ɷ(
IMyRefinery Ţ,List<IMyCargoContainer>ɱ){var ʏ=new ʄ{ʇ=j,ʉ=null,ʈ=false};if(!j)return ʏ;ʏ.ʉ=ʕ(Ţ,ɱ);if(ʏ.ʉ==null)return ʏ;if(Ţ.
InputInventory.ItemCount>0)ʏ.ʈ=ʖ(Ţ,ɱ);return ʏ;}void ɯ(){for(int œ=U.Count-1;œ>=0;œ--){var Ŕ=U[œ];if(Ŕ==null||Ŕ.Closed||Ŕ.CubeGrid==
null){long ŕ=Ŕ?.EntityId??0;U.RemoveAt(œ);ó.Remove(ŕ);Å.Remove(ŕ);ú.Remove(ŕ);ù.Remove(ŕ);}}}List<IMyCargoContainer>ɰ(){var
ǩ=new List<IMyCargoContainer>();for(int œ=0;œ<S.Count;œ++){var Ť=S[œ];if(Ť!=null&&ŵ(Ť.CustomName,oresTag))ǩ.Add(Ť);}
return ǩ;}void ɵ(IMyRefinery Ţ){if(Ţ==null)return;if(j||k)ʗ(Ţ);else ʘ(Ţ);}List<string>ʐ(IMyRefinery Ţ){var ǩ=new List<string>(
);if(Ţ==null)return ǩ;var ʚ=ʙ(Ţ.CustomName);for(int ű=0;ű<ʚ.Count;ű++){string ʛ=ʚ[ű];if(ʛ.Length<3)continue;for(int œ=0;œ
<Ë.Count;œ++){string ʜ=Ë[œ];if(string.IsNullOrEmpty(ʜ))continue;if(ʝ(ʜ,ʛ)){if(!ǩ.Contains(ʜ))ǩ.Add(ʜ);}}}return ǩ;}bool ʝ
(string ʞ,string ʟ){if(string.IsNullOrEmpty(ʞ)||string.IsNullOrEmpty(ʟ))return false;var ʠ=ʙ(ʞ);var ʚ=ʙ(ʟ);for(int œ=0;œ<
ʚ.Count;œ++){if(ʚ[œ].Length<3)continue;for(int ƒ=0;ƒ<ʠ.Count;ƒ++){if(ʠ[ƒ].Contains(ʚ[œ]))return true;}}return false;}bool
ʊ(string ů,string ƴ){if(string.IsNullOrEmpty(ů)||string.IsNullOrEmpty(ƴ))return false;return ʡ(ů)==ʡ(ƴ);}string ʡ(string
ȥ){if(string.IsNullOrEmpty(ȥ))return null;return ȥ.ToLowerInvariant().Replace("_","").Replace(" ","");}List<string>ʙ(
string ȥ){var ǩ=new List<string>();if(string.IsNullOrEmpty(ȥ))return ǩ;var Š=new StringBuilder();char ʢ='\0';foreach(char Ť in
ȥ){if(char.IsLetter(Ť)){if(Š.Length>0&&char.IsUpper(Ť)&&char.IsLower(ʢ)){ǩ.Add(Š.ToString().ToLowerInvariant());Š.Clear()
;}Š.Append(Ť);}else{if(Š.Length>0){ǩ.Add(Š.ToString().ToLowerInvariant());Š.Clear();}}ʢ=Ť;}if(Š.Length>0)ǩ.Add(Š.ToString
().ToLowerInvariant());return ǩ;}bool ʖ(IMyRefinery Ţ,List<IMyCargoContainer>ɱ){if(Ţ==null)return false;var Ų=Ţ.
InputInventory;if(Ų.ItemCount==0)return false;var Ɠ=Ų.GetItemAt(0);if(!Ɠ.HasValue)return false;string ʔ=Ɠ.Value.Type.SubtypeId;int ʣ=-
1;for(int œ=0;œ<Ë.Count;œ++){if(ʊ(ʔ,Ë[œ])){ʣ=œ;break;}}if(ʣ==-1)return true;for(int œ=0;œ<ʣ;œ++){if(ʒ(Ţ,ɱ,Ë[œ]))return
true;}return false;}bool ʒ(IMyRefinery Ţ,List<IMyCargoContainer>ɱ,string ʤ){if(Ţ==null)return false;for(int ʥ=0;ʥ<ɱ.Count;ʥ
++){var Ť=ɱ[ʥ];if(Ť==null)continue;var Ų=Ť.GetInventory(0);if(Ų==null||Ų.ItemCount==0)continue;Ċ.Clear();Ų.GetItems(Ċ);for
(int œ=0;œ<Ċ.Count;œ++){var Ɠ=Ċ[œ];if(Ɠ.Type.TypeId!="MyObjectBuilder_Ore")continue;if(Ɠ.Type.SubtypeId=="Ice")continue;
if(ʊ(Ɠ.Type.SubtypeId,ʤ))return true;}}return false;}string ʧ(IMyRefinery Ţ,List<IMyCargoContainer>ɱ){var ʦ=ʐ(Ţ);if(ʦ.
Count==0)return null;for(int œ=0;œ<ʦ.Count;œ++){string ʤ=ʦ[œ];if(ʒ(Ţ,ɱ,ʤ))return ʤ;}return null;}string ʕ(IMyRefinery Ţ,List<
IMyCargoContainer>S){for(int œ=0;œ<Ë.Count;œ++){string ʑ=Ë[œ];if(ʑ.Equals("Ice",StringComparison.OrdinalIgnoreCase))continue;if(!ʨ(ʑ,S))
continue;if(!ʩ(Ţ,ʑ))continue;return ʑ;}return null;}void ʁ(IMyRefinery Ţ,List<IMyCargoContainer>S){if(Ţ==null)return;string ʤ=ʧ(
Ţ,S);if(string.IsNullOrEmpty(ʤ))return;var ʪ=Ţ.InputInventory;double ʫ=(double)ʪ.MaxVolume-(double)ʪ.CurrentVolume;if(ʫ<=
0)return;for(int ʥ=0;ʥ<S.Count&&ʫ>0;ʥ++){var ɡ=S[ʥ];if(ɡ==null)continue;var Ų=ɡ.GetInventory(0);if(Ų==null)continue;Ñ.
Clear();Ų.GetItems(Ñ);for(int œ=0;œ<Ñ.Count&&ʫ>0;œ++){var Ɠ=Ñ[œ];if(Ɠ.Type.TypeId!="MyObjectBuilder_Ore")continue;if(!ʊ(Ɠ.
Type.SubtypeId,ʤ))continue;var ʬ=Ɠ.Type.GetItemInfo();double ʭ=(double)ʬ.Volume;double ʮ=ʫ/ʭ;double ɠ=Math.Min((double)Ɠ.
Amount,ʮ);if(ɠ<=0)continue;Ų.TransferItemTo(ʪ,Ɠ,(MyFixedPoint)ɠ);ʫ-=ɠ*ʭ;}}}void ɿ(IMyRefinery Ţ,List<IMyCargoContainer>S){if(Ţ
==null)return;var ʪ=Ţ.InputInventory;double ʫ=(double)ʪ.MaxVolume-(double)ʪ.CurrentVolume;if(ʫ<=0)return;string ʯ=ʕ(Ţ,S);
if(string.IsNullOrEmpty(ʯ))return;for(int Ť=0;Ť<S.Count&&ʫ>0;Ť++){var ʰ=S[Ť].GetInventory(0);if(ʰ==null||ʰ.ItemCount==0)
continue;Ñ.Clear();ʰ.GetItems(Ñ);for(int ƒ=0;ƒ<Ñ.Count&&ʫ>0;ƒ++){var Ɠ=Ñ[ƒ];if(Ɠ.Type.TypeId!="MyObjectBuilder_Ore")continue;if(
!ʊ(Ɠ.Type.SubtypeId,ʯ))continue;var ʬ=Ɠ.Type.GetItemInfo();double ʭ=(double)ʬ.Volume;double ʮ=ʫ/ʭ;double ɠ=Math.Min((
double)Ɠ.Amount,ʮ);if(ɠ<=0)break;ʰ.TransferItemTo(ʪ,Ɠ,(MyFixedPoint)ɠ);ʫ-=ɠ*ʭ;}}}void ɶ(IMyRefinery Ţ){var ʱ=Ţ as
IMyTextSurfaceProvider;if(ʱ==null||ʱ.SurfaceCount==0)return;var ʲ=ʱ.GetSurface(0);ʲ.ContentType=ContentType.TEXT_AND_IMAGE;ʲ.Font="Monospace";
ʲ.FontSize=2f;ʲ.Alignment=TextAlignment.CENTER;string ʳ="Idle";string ʴ="";var ʵ=new List<MyProductionItem>();Ţ.GetQueue(
ʵ);if(ʵ.Count>0){string ʶ=ʵ[0].BlueprintId.SubtypeName;ʴ=ʷ(ʶ);ʳ="Refining";}else{Ñ.Clear();Ţ.InputInventory.GetItems(Ñ);
for(int œ=0;œ<Ñ.Count;œ++){var Ɠ=Ñ[œ];if(Ɠ.Type.TypeId!="MyObjectBuilder_Ore")continue;switch(Ɠ.Type.SubtypeId){case
"∑UnknownMaterial":ʴ="Caesium";break;case"∇UnknownMaterial":ʴ="Radium";break;case"⌊UnknownMaterial":ʴ="Diamond";break;case
"∏UnknownMaterial":ʴ="Californium";break;default:ʴ=Ɠ.Type.SubtypeId;break;}ʳ="Queued";break;}}string ʸ=
$"REFINERY\n\nStatus: {ʳ}\nOre: {(string.IsNullOrEmpty(ʴ)?"-":ʴ)}";ʲ.WriteText(ʸ);}string ʷ(string ʶ){int ʹ=ʶ.IndexOf("OreToIngot");if(ʹ>0)return ʶ.Substring(0,ʹ);return ʶ;}void Ȟ(){Į(
"Refineries:");if(Ą>0)Į($"• {Ą} Producing");if(ą>0)Į($"• {ą} Waiting (name-based)");if(Ć>0)Į($"• {Ć} No compatible ore");if(ć>0)Į(
$"• {ć} Turned Off");}void ʘ(IMyRefinery ʺ){ʺ.UseConveyorSystem=true;}void ʗ(IMyRefinery ʺ){ʺ.UseConveyorSystem=false;}void ɽ(IMyRefinery Ţ
){var ʵ=new List<MyProductionItem>();Ţ.GetQueue(ʵ);for(int œ=ʵ.Count-1;œ>=0;œ--){var Ɠ=ʵ[œ];Ţ.RemoveQueueItem(œ,Ɠ.Amount)
;}}void ɼ(IMyRefinery Ţ,List<IMyCargoContainer>S){if(Ţ==null)return;var ų=Ţ.InputInventory;if(ų==null||ų.VolumeFillFactor
==0f)return;if(ų.VolumeFillFactor==0f)return;Ñ.Clear();ų.GetItems(Ñ);for(int œ=Ñ.Count-1;œ>=0;œ--){var Ɠ=Ñ[œ];if(Ɠ.Type.
TypeId!="MyObjectBuilder_Ore")continue;for(int Ť=0;Ť<S.Count;Ť++){var ɡ=S[Ť];if(ɡ==null)continue;var ƨ=ɡ.GetInventory(0);if(ƨ
==null)continue;bool ʻ=ų.TransferItemTo(ƨ,Ɠ,null);if(ų.VolumeFillFactor==0f)return;if(!ų.ContainItems(Ɠ.Amount,Ɠ.Type))
break;}}}bool ʩ(IMyRefinery Ţ,string ʜ){if(Ţ==null||string.IsNullOrEmpty(ʜ))return false;var ʼ=new List<MyItemType>();Ţ.
InputInventory.GetAcceptedItems(ʼ);var ʽ=MyItemType.MakeOre(ʜ);return ʼ.Contains(ʽ);}bool ʨ(string ʑ,List<IMyCargoContainer>S){if(
string.IsNullOrEmpty(ʑ))return false;for(int Ť=0;Ť<S.Count;Ť++){var ɡ=S[Ť];if(ɡ==null)continue;var ʰ=ɡ.GetInventory(0);if(ʰ==
null||ʰ.ItemCount==0)continue;Ñ.Clear();ʰ.GetItems(Ñ);for(int œ=0;œ<Ñ.Count;œ++){var ʾ=Ñ[œ];if(ʾ.Type.TypeId==
"MyObjectBuilder_Ore"&&ʾ.Type.SubtypeId.Equals(ʑ,StringComparison.OrdinalIgnoreCase))return true;}}return false;}int ɲ(){unchecked{int ʿ=17;
for(int œ=0;œ<Ë.Count;œ++){ʿ=ʿ*31+Ë[œ].GetHashCode();ʿ=ʿ*31+œ;}return ʿ;}}void Ő(){DateTime ˀ=DateTime.Now;if((ˀ-ö).
TotalSeconds<REFINERY_SCAN_INTERVAL_SEC)return;ö=ˀ;foreach(var Ţ in U){var Ų=Ţ.OutputInventory;if(Ų==null)continue;var ˁ=new List<
MyInventoryItem>();Ų.GetItems(ˁ);Dictionary<string,double>ʓ=new Dictionary<string,double>();foreach(var Ɠ in ˁ){if(Ɠ.Type.TypeId!=
"MyObjectBuilder_Ingot")continue;string ǳ=Ɠ.Type.SubtypeId;double Ʋ=(double)Ɠ.Amount;ʓ[ǳ]=Ʋ;}Dictionary<string,double>ˆ;if(!õ.TryGetValue(Ţ.
EntityId,out ˆ)){õ[Ţ.EntityId]=ʓ;continue;}foreach(var ˇ in ʓ){string ǳ=ˇ.Key;double ˈ=ˇ.Value;double ˉ=ˆ.ContainsKey(ǳ)?ˆ[ǳ]:0;
double ˊ=ˈ-ˉ;if(ˊ>0){if(!ô.ContainsKey(ǳ))ô[ǳ]=0;ô[ǳ]+=ˊ;}}õ[Ţ.EntityId]=ʓ;}if((ˀ-ø).TotalSeconds>=
CUSTOM_DATA_WRITE_INTERVAL_SEC){ˋ();ø=ˀ;}}void ˋ(){var Š=new StringBuilder();Š.AppendLine("[Refinery Production Log]");Š.AppendLine(
$"Last Updated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");Š.AppendLine("");foreach(var ˌ in ô.OrderBy(Ƚ=>Ƚ.Key)){Š.AppendLine($"{ˌ.Key}: {ˌ.Value:F2}");}Me.CustomData=Š.
ToString();}string Ȥ(IMyReactor Ŭ,double ˍ){var ˎ=Ŭ.GetInventory();var ˏ=new MyItemType("MyObjectBuilder_Ingot",fueltag1);var ː=
ˏ.GetItemInfo();double ˑ=(double)ː.Mass;double ˠ=(double)ˎ.GetItemAmount(ˏ);double ˡ=ˠ*ˑ;string ǳ=Ŭ.BlockDefinition.
SubtypeId;double ˢ=MaxPowerReactor1Subtypes.ContainsKey(ǳ)?MaxPowerReactor1Subtypes[ǳ]:DEFAULT_REACTOR1_MAX_POWER;double ˣ=
TotalConsumptionReactor1MaxBySubtypes.ContainsKey(ǳ)?TotalConsumptionReactor1MaxBySubtypes[ǳ]:DEFAULT_REACTOR1_CONSUMPTION_MAX;double ˤ=Math.Max(0.0,Math.Min
(ˍ,ˢ));double ˬ=ˣ*(ˤ/ˢ);double ˮ=ˬ;string Î="kg/s";if(ˮ<1){ˮ*=1000;Î="g/s";}ˮ=Math.Round(ˮ,5);string Ͱ="N/A";if(ˬ>0.0){
double Ɍ=ˡ/ˬ;Ͱ=ͱ((long)Math.Floor(Ɍ));}return string.Format("{0} Consumption Rate: {1:F3} {2}\nEst: {3}\nOutput: {4:F2} MW",
fueltag1,ˮ,Î,Ͱ,ˍ);}string Ȫ(IMyReactor Ͳ,double ˍ){var ˎ=Ͳ.GetInventory();var ˏ=new MyItemType("MyObjectBuilder_Ingot",fueltag2)
;var ː=ˏ.GetItemInfo();double ˑ=(double)ː.Mass;double ˠ=(double)ˎ.GetItemAmount(ˏ);double ˡ=ˠ*ˑ;string ǳ=Ͳ.
BlockDefinition.SubtypeId;double ˢ=MaxPowerReactor2Subtypes.ContainsKey(ǳ)?MaxPowerReactor2Subtypes[ǳ]:DEFAULT_REACTOR2_MAX_POWER;
double ˣ=TotalConsumptionReactor2MaxBySubtypes.ContainsKey(ǳ)?TotalConsumptionReactor2MaxBySubtypes[ǳ]:
DEFAULT_REACTOR2_CONSUMPTION_MAX;double ˤ=Math.Max(0.0,Math.Min(ˍ,ˢ));double ˬ=ˣ*(ˤ/ˢ);double ͳ=Math.Round(ˬ*1000.0,5);string ʹ="N/A";if(ˬ>0.0){double Ɍ
=ˡ/ˬ;ʹ=ͱ((long)Math.Floor(Ɍ));}return string.Format("{0} Consumption Rate: {1:F3} g/s\nEst: {2}\nOutput: {3:F2} MW",
fueltag2,ͳ,ʹ,ˍ);}string ȯ(IMyReactor Ͷ,double ˍ){var ˎ=Ͷ.GetInventory();var ˏ=new MyItemType("MyObjectBuilder_Ingot",fueltag3);
var ː=ˏ.GetItemInfo();double ˑ=(double)ː.Mass;double ˠ=(double)ˎ.GetItemAmount(ˏ);double ˡ=ˠ*ˑ;string ǳ=Ͷ.BlockDefinition.
SubtypeId;double ˢ=MaxPowerReactor3Subtypes.ContainsKey(ǳ)?MaxPowerReactor3Subtypes[ǳ]:DEFAULT_REACTOR3_MAX_POWER;double ˣ=
TotalConsumptionReactor3MaxBySubtypes.ContainsKey(ǳ)?TotalConsumptionReactor3MaxBySubtypes[ǳ]:DEFAULT_REACTOR3_CONSUMPTION_MAX;double ˤ=Math.Max(0.0,Math.Min
(ˍ,ˢ));double ˬ=ˣ*(ˤ/ˢ);double ͳ=Math.Round(ˬ*1000.0,5);string ʹ="N/A";if(ˬ>0.0){double Ɍ=ˡ/ˬ;ʹ=ͱ((long)Math.Floor(Ɍ));}
return string.Format("{0} Consumption Rate: {1:F3} g/s\nEst: {2}\nOutput: {3:F2} MW",fueltag3,ͳ,ʹ,ˍ);}string ͱ(long ǭ){if(ǭ<=0
)return"0s";long ȫ=ǭ/(7*24*3600);ǭ%=(7*24*3600);long Ȭ=ǭ/(24*3600);ǭ%=(24*3600);long ȭ=ǭ/3600;ǭ%=3600;long ɏ=ǭ/60;ǭ%=60;
return string.Format("{0}w {1}d {2}h {3}m {4}s",ȫ,Ȭ,ȭ,ɏ,ǭ);}void ſ(IMyReactor Ŭ,MyItemType ͷ,double ͺ,double Ʃ,List<
IMyCargoContainer>S,ref int ͻ){if(Ŭ==null)return;if(Ŭ.CustomName==null)return;if(Ŭ.CubeGrid==null)return;if(!Ŭ.UseConveyorSystem)Ŭ.
UseConveyorSystem=true;var ˎ=Ŭ.GetInventory();if(ˎ==null)return;double ͼ=(double)ˎ.GetItemAmount(ͷ);bool ͽ=Ŭ.CubeGrid.IsStatic;double Ά=ͽ
?Ʃ+1000:Ʃ;if(ͼ>Ά){ͻ++;return;}double Έ=ͺ;double Ή=Έ-ͼ;if(Ή<=0){ͻ++;return;}double Ί=0;foreach(var ɡ in S){var Ų=ɡ.
GetInventory();if(Ų==null)continue;Ί+=(double)Ų.GetItemAmount(ͷ);}if(ͽ)Ό(ˎ,ͷ,ref Ή,ref Ί,S);if(Ή>0)Echo(
$"Skipping {Ŭ.CustomName}: Not enough {ͷ} available.");}void Ɓ(IMyReactor Ŭ,MyItemType ͷ,double Ύ,List<IMyCargoContainer>S){var Ų=Ŭ.GetInventory();if(Ų==null)return;if(!Ŭ.
UseConveyorSystem)Ŭ.UseConveyorSystem=true;double ͼ=(double)Ų.GetItemAmount(ͷ);double Ώ=Ύ-ͼ;if(ͼ>100){return;}if(Ώ<=0)return;double Ί=0;
foreach(var Ť in S)Ί+=(double)Ť.GetInventory().GetItemAmount(ͷ);if(Ί-Ώ>100)Ό(Ų,ͷ,ref Ώ,ref Ί,S);}void Ό(IMyInventory ΐ,
MyItemType ˏ,ref double Ή,ref double ż,List<IMyCargoContainer>Ǳ){foreach(var ɡ in Ǳ){var Α=ɡ.GetInventory();if(Α==null)continue;
double Β=(double)Α.GetItemAmount(ˏ);if(Β<=0)continue;double Γ=Math.Min(Β,Ή);var Δ=Α.FindItem(ˏ);if(!Δ.HasValue)continue;var Ε=
(MyFixedPoint)Γ;bool ɣ=Α.TransferItemTo(ΐ,Δ.Value,Ε);if(!ɣ){Echo(
$"Transfer FAILED from {ɡ.CustomName} -> dest (trying {Γ:F0})");if(Γ>1){var Ζ=(MyFixedPoint)1;bool Η=Α.TransferItemTo(ΐ,Δ.Value,Ζ);if(Η){Ή-=1;ż-=1;}}continue;}Ή-=Γ;ż-=Γ;if(Ή<=0)break
;}}void ɪ(){var ɚ=new List<IMyCargoContainer>();GridTerminalSystem.GetBlocksOfType(ɚ,Ť=>Ť.CubeGrid==Me.CubeGrid&&Ť.
CustomName.IndexOf(ingotsTag,StringComparison.OrdinalIgnoreCase)>=0);if(ɚ.Count==0)return;foreach(var Ţ in U){if(!ò.Contains(Ţ.
EntityId))continue;var Θ=Ţ.OutputInventory;if(Θ.ItemCount==0){ò.Remove(Ţ.EntityId);continue;}Ι(Ţ,Θ,ɚ);}}void Ι(IMyRefinery Ţ,
IMyInventory Κ,List<IMyCargoContainer>ɚ){if(Κ==null||ɚ==null||ɚ.Count==0)return;var ˁ=new List<MyInventoryItem>();Κ.GetItems(ˁ);if(ˁ
.Count==0)return;for(int œ=ˁ.Count-1;œ>=0;œ--){var Ɠ=ˁ[œ];if(Ɠ.Type.TypeId!="MyObjectBuilder_Ingot")continue;double Λ=(
double)Ɠ.Amount;for(int ű=0;ű<ɚ.Count&&Λ>0;ű++){var Μ=ɚ[ű];if(Μ==null)continue;var Ν=Μ.GetInventory(0);if(Ν==null)continue;var
Ξ=(MyFixedPoint)Λ;if(Ν.CanItemsBeAdded(Ξ,Ɠ.Type)){if(Κ.TransferItemTo(Ν,Ɠ,Ξ)){Λ=0;break;}}var ʬ=Ɠ.Type.GetItemInfo();
double ʭ=(double)ʬ.Volume;double Ο=(double)Ν.MaxVolume-(double)Ν.CurrentVolume;if(Ο<=0)continue;double Π=Math.Floor(Ο/ʭ);if(Π
<=0)continue;double ɠ=Math.Min(Π,Λ);var Ρ=(MyFixedPoint)ɠ;if(Κ.TransferItemTo(Ν,Ɠ,Ρ)){Λ-=ɠ;}}}}int Ĳ(){Ä=null;for(int œ=S.
Count-1;œ>=0;œ--){var Ť=S[œ];if(Ť==null||Ť.CubeGrid==null||Ť.Closed){long ŕ=Ť?.EntityId??0;S.RemoveAt(œ);Echo(
$"Removed invalid cargo: {ŕ}");}}var Ġ=new HashSet<IMyCubeGrid>();Ũ(Ġ);int Ǉ=0;var Σ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(
Σ,ƴ=>(ƴ is IMyCargoContainer||ƴ is IMyShipConnector)&&Ġ.Contains(ƴ.CubeGrid));var Τ=new List<IMyProductionBlock>();
GridTerminalSystem.GetBlocksOfType(Τ,ƴ=>Ġ.Contains(ƴ.CubeGrid));var ƻ=new List<IMyTerminalBlock>();var ɚ=new List<IMyTerminalBlock>();var
Υ=new List<IMyTerminalBlock>();var Φ=new List<IMyTerminalBlock>();var Χ=new List<IMyTerminalBlock>();foreach(var Ψ in Σ){
string Ǆ=Ψ.CustomName;if(ŵ(Ǆ,oresTag))ƻ.Add(Ψ);if(ŵ(Ǆ,ingotsTag))ɚ.Add(Ψ);if(ŵ(Ǆ,componentsTag))Υ.Add(Ψ);if(ŵ(Ǆ,ammoTag))Φ.Add
(Ψ);if(ŵ(Ǆ,miscTag))Χ.Add(Ψ);}if(l&&!m){Ω(ƻ,oresTag);Ω(ɚ,ingotsTag);Ω(Υ,componentsTag);Ω(Φ,ammoTag);Ω(Χ,miscTag);}var Ϊ=
new List<string>();if(ƻ.Count==0)Ϊ.Add("Ores");if(ɚ.Count==0)Ϊ.Add("Ingots");if(Υ.Count==0)Ϊ.Add("Components");if(Φ.Count==
0)Ϊ.Add("Ammo");if(Χ.Count==0)Ϊ.Add("Misc");if(Ϊ.Count>0){Ä="Missing containers: "+string.Join(", ",Ϊ);}foreach(var Ψ in
Σ){var Ų=Ψ.GetInventory(0);Ϋ(Ų,ƻ,ɚ,Υ,Φ,Χ,ref Ǉ,ref Æ,ref Ç,ref È,ref É,ref Ê);}foreach(var Ψ in Τ){var Ų=Ψ.
OutputInventory;if(Ų!=null)Ϋ(Ų,ƻ,ɚ,Υ,Φ,Χ,ref Ǉ,ref Æ,ref Ç,ref È,ref É,ref Ê);}if(m&&!l){if(ƻ.Count>1)ά(ƻ,έ);if(ɚ.Count>1)ά(ɚ,ή);if(Υ.
Count>1)ά(Υ,ί);if(Φ.Count>1)ά(Φ,ΰ);if(Χ.Count>1)ά(Χ,α);}if(n){foreach(var Ť in ƻ)β(Ť);foreach(var Ť in ɚ)β(Ť);foreach(var Ť
in Υ)β(Ť);foreach(var Ť in Φ)β(Ť);foreach(var Ť in Χ)β(Ť);}var V=new List<IMyShipConnector>();GridTerminalSystem.
GetBlocksOfType(V,Ť=>Ť.CubeGrid==Me.CubeGrid);foreach(var ƈ in V){if(ƈ.Status!=MyShipConnectorStatus.Connected)continue;var Ŧ=ƈ.
OtherConnector?.CubeGrid;if(Ŧ==null)continue;if(Ɖ(Me.CubeGrid)||Ɖ(Ŧ))continue;if(ŧ(Me.CubeGrid)){γ(Me.CubeGrid,ref Ǉ);}else if(ŧ(Ŧ)){γ
(Ŧ,ref Ǉ);}}δ(ƻ,ɚ,Υ,Φ,Χ);return Ǉ;}void β(IMyTerminalBlock Ψ){if(Ψ==null)return;var Ų=Ψ.GetInventory(0);if(Ų==null||Ų.
ItemCount<2)return;Ċ.Clear();Ų.GetItems(Ċ);for(int œ=Ċ.Count-1;œ>=0;œ--){Ų.TransferItemTo(Ų,œ,null,stackIfPossible:true);}}string
η(MyInventoryItem Ɠ){string ȥ=ε(Ɠ);string ζ=Ɠ.Type.TypeId.ToString();foreach(var ˌ in CategoryOverrides){if(ȥ.Contains(ˌ.
Key))return ˌ.Value;}switch(ζ){case"MyObjectBuilder_Ore":return"ore";case"MyObjectBuilder_Ingot":return"ingot";case
"MyObjectBuilder_AmmoMagazine":return"ammo";case"MyObjectBuilder_Component":return"component";default:return"misc";}}string ε(MyInventoryItem Ɠ){
string ȥ=Ɠ.Type.SubtypeId.ToString();int θ=ȥ.IndexOfAny("0123456789_".ToCharArray());if(θ>0)ȥ=ȥ.Substring(0,θ);return ȥ.
ToLowerInvariant();}void Ϋ(IMyInventory Κ,List<IMyTerminalBlock>ƻ,List<IMyTerminalBlock>ɚ,List<IMyTerminalBlock>Υ,List<IMyTerminalBlock>
Φ,List<IMyTerminalBlock>Χ,ref int Ǉ,ref int Æ,ref int Ç,ref int È,ref int É,ref int Ê){var ˁ=new List<MyInventoryItem>();
Κ.GetItems(ˁ);foreach(var Ɠ in ˁ){var ι=η(Ɠ);IMyTerminalBlock ƨ=null;if(!l&&κ(Κ,ι))continue;switch(ι){case"ore":if(ƻ.
Count>0){ƨ=l?λ(ƻ,Ɠ):ƻ[Æ++%ƻ.Count];}break;case"ingot":if(ɚ.Count>0){ƨ=l?λ(ɚ,Ɠ):ɚ[Ç++%ɚ.Count];}break;case"component":if(Υ.
Count>0){ƨ=l?λ(Υ,Ɠ):Υ[È++%Υ.Count];}break;case"ammo":if(Φ.Count>0){ƨ=l?λ(Φ,Ɠ):Φ[É++%Φ.Count];}break;default:if(Χ.Count>0){ƨ=l
?λ(Χ,Ɠ):Χ[Ê++%Χ.Count];}break;}if(ƨ!=null&&Κ.Owner.EntityId!=ƨ.EntityId){var Ν=ƨ.GetInventory(0);if(Ν!=null&&Κ.
TransferItemTo(Ν,Ɠ))Ǉ+=(int)Ɠ.Amount;}}}bool κ(IMyInventory Ų,string ι){var Ψ=Ų.Owner as IMyTerminalBlock;if(Ψ==null)return false;var
Ǆ=Ψ.CustomName.ToLower();switch(ι){case"ore":return Ǆ.Contains(oresTag.ToLower());case"ingot":return Ǆ.Contains(ingotsTag
.ToLower());case"component":return Ǆ.Contains(componentsTag.ToLower());case"ammo":return Ǆ.Contains(ammoTag.ToLower());
default:return Ǆ.Contains(miscTag.ToLower());}}int ξ(IMyTerminalBlock Ψ,string ɤ){string Ǆ=Ψ.CustomName;int μ=Ǆ.IndexOf(ɤ,
StringComparison.OrdinalIgnoreCase);if(μ<0)return int.MaxValue;int ɂ=Ǆ.IndexOf('[',μ);int Ƀ=Ǆ.IndexOf(']',ɂ+1);if(ɂ>=0&&Ƀ>ɂ){string ν=Ǆ.
Substring(ɂ+1,Ƀ-ɂ-1);int Ŵ;if(int.TryParse(ν,out Ŵ))return Ŵ;}return int.MaxValue;}void Ω(List<IMyTerminalBlock>Ǳ,string ɤ){Ǳ.
Sort((ů,ƴ)=>ξ(ů,ɤ).CompareTo(ξ(ƴ,ɤ)));}IMyTerminalBlock λ(List<IMyTerminalBlock>Ǳ,MyInventoryItem Ɠ){for(int œ=0;œ<Ǳ.Count;œ
++){var Ų=Ǳ[œ].GetInventory(0);if(Ų==null)continue;if(Ų.CanItemsBeAdded(Ɠ.Amount,Ɠ.Type))return Ǳ[œ];}return null;}bool έ(
MyInventoryItem Ɠ)=>η(Ɠ)=="ore";bool ή(MyInventoryItem Ɠ)=>η(Ɠ)=="ingot";bool ί(MyInventoryItem Ɠ)=>η(Ɠ)=="component";bool ΰ(
MyInventoryItem Ɠ)=>η(Ɠ)=="ammo";bool α(MyInventoryItem Ɠ)=>η(Ɠ)=="misc";void ά(List<IMyTerminalBlock>Ǳ,Func<MyInventoryItem,bool>ο){if
(Ǳ==null||Ǳ.Count<2)return;var π=new Dictionary<MyItemType,MyFixedPoint>();foreach(var Ψ in Ǳ){var Ų=Ψ.GetInventory(0);
var ˁ=new List<MyInventoryItem>();Ų.GetItems(ˁ);foreach(var Ɠ in ˁ){if(!ο(Ɠ))continue;MyFixedPoint Ʋ;if(!π.TryGetValue(Ɠ.
Type,out Ʋ))Ʋ=MyFixedPoint.Zero;π[Ɠ.Type]=MyFixedPoint.AddSafe(Ʋ,Ɠ.Amount);}}var ρ=(MyFixedPoint)5;foreach(var ˌ in π){var ū
=ˌ.Key;var ƞ=ˌ.Value;var ς=MyFixedPoint.MultiplySafe(ƞ,1.0f/Ǳ.Count);foreach(var Ψ in Ǳ){var Ų=Ψ.GetInventory(0);var ʓ=Ų.
GetItemAmount(ū);var σ=ς-ʓ;if(σ<=ρ&&σ>=-ρ)continue;if(σ>ρ){var Ώ=σ;foreach(var τ in Ǳ){if(τ==Ψ)continue;var υ=τ.GetInventory(0);var φ
=υ.GetItemAmount(ū);var χ=φ-ρ;if(χ<=MyFixedPoint.Zero)continue;var ψ=MyFixedPoint.Min(Ώ,χ);if(ψ<=MyFixedPoint.Zero)
continue;var ω=new List<MyInventoryItem>();Ċ.Clear();υ.GetItems(Ċ);for(int œ=0;œ<Ċ.Count&&ψ>MyFixedPoint.Zero;œ++){var ϊ=Ċ[œ];if
(!ϊ.Type.Equals(ū))continue;υ.TransferItemTo(Ų,ϊ,ψ);break;}Ώ=Ώ-ψ;if(Ώ<=ρ)break;}}}}}bool Ɖ(IMyCubeGrid Ŷ){var V=new List<
IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(V,Ť=>Ť.CubeGrid==Ŷ);foreach(var ƈ in V){if(ƈ.Status==MyShipConnectorStatus.
Connected&&ƈ.CustomName.Contains(IgnoreSortingTag)){return true;}}return false;}void γ(IMyCubeGrid ϋ,ref int Ǉ){if(Ɖ(ϋ))return;
var ό=new List<IMyTerminalBlock>();var ύ=new List<IMyTerminalBlock>();var ώ=new List<IMyTerminalBlock>();var Ϗ=new List<
IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(ό,ƴ=>(ƴ is IMyCargoContainer||ƴ is IMyShipConnector)&&ƴ.CubeGrid==ϋ&&ƴ.CustomName
.ToLower().Contains(ingotsTag.ToLower()));GridTerminalSystem.GetBlocksOfType(ύ,ƴ=>(ƴ is IMyCargoContainer||ƴ is
IMyShipConnector)&&ƴ.CubeGrid==ϋ&&ƴ.CustomName.ToLower().Contains(componentsTag.ToLower()));GridTerminalSystem.GetBlocksOfType(ώ,ƴ=>(ƴ
is IMyCargoContainer||ƴ is IMyShipConnector)&&ƴ.CubeGrid==ϋ&&ƴ.CustomName.ToLower().Contains(ammoTag.ToLower()));
GridTerminalSystem.GetBlocksOfType(Ϗ,ƴ=>(ƴ is IMyCargoContainer||ƴ is IMyShipConnector)&&ƴ.CubeGrid==ϋ&&ƴ.CustomName.ToLower().Contains(
miscTag.ToLower()));if(ό.Count==0&&ύ.Count==0&&ώ.Count==0&&Ϗ.Count==0)return;int Ç=0;int È=0;int É=0;int Ê=0;var ϐ=new List<
IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(ϐ,ƴ=>(ƴ is IMyCargoContainer||ƴ is IMyShipConnector)&&ƴ.CubeGrid!=ϋ&&(ƴ.
CustomName.ToLower().Contains(ingotsTag.ToLower())||ƴ.CustomName.ToLower().Contains(componentsTag.ToLower()))&&!ƴ.CustomName.
Contains(IgnoreSortingTag));foreach(var Ψ in ϐ){var Ų=Ψ.GetInventory(0);if(Ų==null)continue;var ˁ=new List<MyInventoryItem>();Ų.
GetItems(ˁ);foreach(var Ɠ in ˁ){string ι=η(Ɠ);IMyTerminalBlock ƨ=null;switch(ι){case"ingot":if(ό.Count>0)ƨ=ό[Ç++%ό.Count];break;
case"component":if(ύ.Count>0)ƨ=ύ[È++%ύ.Count];break;case"ammo":if(ώ.Count>0)ƨ=ώ[É++%ώ.Count];break;case"misc":if(Ϗ.Count>0)ƨ
=Ϗ[Ê++%Ϗ.Count];break;default:if(Ϗ.Count>0){ƨ=Ϗ[Ê%Ϗ.Count];Ê++;}continue;}if(ƨ!=null){var Ν=ƨ.GetInventory(0);if(Ν!=null
&&Ų.TransferItemTo(Ν,Ɠ)){Ǉ+=(int)Ɠ.Amount;}}}}}void ƿ(IMyCubeGrid ϑ,List<IMyTerminalBlock>ƻ){if(Ɖ(ϑ))return;if(ƻ==null||ƻ.
Count==0)return;var ϒ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(ϒ,ƴ=>(ƴ is IMyCargoContainer||ƴ is
IMyShipConnector)&&ƴ.CubeGrid==ϑ);foreach(var Ψ in ϒ){var Ų=Ψ.GetInventory(0);if(Ų==null)continue;var ϓ=new List<MyInventoryItem>();Ų.
GetItems(ϓ);if(ϓ.Count==0)continue;ϔ(Ų,ƻ);}}void ǀ(List<IMyTerminalBlock>ƻ){if(ƻ==null||ƻ.Count==0){Echo(
"BalanceOreContainers: No ore containers found!");return;}Echo($"BalanceOreContainers: Found {ƻ.Count} containers.");var ϕ=new HashSet<string>(StringComparer.
OrdinalIgnoreCase);var ϖ=new List<List<MyInventoryItem>>(ƻ.Count);for(int œ=0;œ<ƻ.Count;œ++){var Ų=ƻ[œ].GetInventory(0);ϖ.Add(new List<
MyInventoryItem>());if(Ų==null)continue;Ų.GetItems(ϖ[œ]);foreach(var ϗ in ϖ[œ]){if(η(ϗ)=="ore")ϕ.Add(ϗ.Type.SubtypeId);}}foreach(var ʽ
in ϕ){int Ϙ=ƻ.Count;double[]ʓ=new double[Ϙ];double ƞ=0.0;for(int œ=0;œ<Ϙ;œ++){foreach(var ϗ in ϖ[œ]){if(ϗ.Type.SubtypeId.
Equals(ʽ,StringComparison.OrdinalIgnoreCase))ʓ[œ]+=(double)ϗ.Amount;}ƞ+=ʓ[œ];}if(ƞ<=0.0)continue;double ƨ=ƞ/Ϙ;const double ϙ=
0.5;for(int ơ=0;ơ<Ϙ;ơ++){double ɜ=ʓ[ơ]-ƨ;if(ɜ<=ϙ)continue;var Ϛ=ƻ[ơ].GetInventory(0);if(Ϛ==null)continue;for(int ϛ=0;ϛ<Ϙ&&ɜ
>ϙ;ϛ++){if(ϛ==ơ)continue;double Ϝ=ƨ-ʓ[ϛ];if(Ϝ<=ϙ)continue;var ϝ=ƻ[ϛ].GetInventory(0);if(ϝ==null)continue;double Ϟ=Math.
Min(ɜ,Ϝ);for(int ϟ=0;ϟ<ϖ[ơ].Count&&Ϟ>0.0;ϟ++){var ϊ=ϖ[ơ][ϟ];if(!ϊ.Type.SubtypeId.Equals(ʽ,StringComparison.
OrdinalIgnoreCase))continue;double Β=(double)ϊ.Amount;if(Β<=0.0)continue;var Ϡ=(MyFixedPoint)Math.Floor(Math.Min(Β,Ϟ));if(Ϡ<=0)continue;
if(Ϛ.TransferItemTo(ϝ,ϊ,Ϡ)){double ĳ=(double)Ϡ;Ϟ-=ĳ;ɜ-=ĳ;ʓ[ơ]-=ĳ;ʓ[ϛ]+=ĳ;}}}}}}bool ŧ(IMyCubeGrid Ŷ){var ǁ=new List<
IMyProgrammableBlock>();GridTerminalSystem.GetBlocksOfType(ǁ,ǂ=>ǂ.CubeGrid==Ŷ);return ǁ.Any(ǂ=>ǂ.CustomName.Contains(MasterTag));}bool ƾ(
IMyCubeGrid Ŷ){var ϡ=new List<IMyShipDrill>();GridTerminalSystem.GetBlocksOfType(ϡ,Ϣ=>Ϣ.CubeGrid==Ŷ);return ϡ.Count>0;}bool ɖ(
IMyCubeGrid Ŷ){var ϣ=new List<IMyShipGrinder>();GridTerminalSystem.GetBlocksOfType(ϣ,Ű=>Ű.CubeGrid==Ŷ);return ϣ.Count>0;}bool ɗ(
IMyCubeGrid Ŷ){var Ϥ=new List<IMyShipWelder>();GridTerminalSystem.GetBlocksOfType(Ϥ,ϥ=>ϥ.CubeGrid==Ŷ);return Ϥ.Count>0;}bool ɘ(
IMyCubeGrid Ŷ){var ǂ=new List<IMyProgrammableBlock>();GridTerminalSystem.GetBlocksOfType(ǂ,ɐ=>ɐ.CubeGrid==Ŷ);foreach(var Ψ in ǂ){if
(Ψ.CustomName.ToLower().Contains(NonCombat)){return false;}}var ϡ=new List<IMyShipDrill>();GridTerminalSystem.
GetBlocksOfType(ϡ,Ϣ=>Ϣ.CubeGrid==Ŷ);if(ϡ.Count>0)return false;var ϣ=new List<IMyShipGrinder>();GridTerminalSystem.GetBlocksOfType(ϣ,Ű=>
Ű.CubeGrid==Ŷ);if(ϣ.Count>0)return false;var Ϥ=new List<IMyShipWelder>();GridTerminalSystem.GetBlocksOfType(Ϥ,ϥ=>ϥ.
CubeGrid==Ŷ);if(Ϥ.Count>0)return false;var Ϧ=new List<IMyConveyorSorter>();GridTerminalSystem.GetBlocksOfType(Ϧ,ȥ=>ȥ.CubeGrid==Ŷ
);foreach(var ϧ in Ϧ){string Ǆ=ϧ.CustomName.ToLower();if(Ǆ.Contains("turret")||Ǆ.Contains("laser")||Ǆ.Contains("cannon")
||Ǆ.Contains("torpedo")){return true;}}return false;}void ϔ(IMyInventory Κ,List<IMyTerminalBlock>ƻ){if(Κ==null||ƻ==null||ƻ
.Count==0)return;var ˁ=new List<MyInventoryItem>();Κ.GetItems(ˁ);if(ˁ.Count==0)return;for(int œ=0;œ<ˁ.Count;œ++){var Ɠ=ˁ[
œ];if(η(Ɠ)!="ore")continue;IMyInventory Ν=null;int Ϩ=ƻ.Count;while(Ϩ-->0){var ƨ=ƻ[Æ%ƻ.Count];Æ++;if(ƨ==null||ƨ.Closed)
continue;Ν=ƨ.GetInventory(0);if(Ν!=null)break;}if(Ν==null)return;Κ.TransferItemTo(Ν,Ɠ);}}void δ(List<IMyTerminalBlock>ƻ,List<
IMyTerminalBlock>ɚ,List<IMyTerminalBlock>Υ,List<IMyTerminalBlock>Φ,List<IMyTerminalBlock>Χ){var ϩ=new List<IMyCargoContainer>();
GridTerminalSystem.GetBlocksOfType(ϩ,Ť=>Ť.CustomName.Contains(SpecialCargo));foreach(var Ϫ in ϩ){if(Ɖ(Ϫ.CubeGrid))continue;var Ϭ=ϫ(Ϫ.
CustomData);ϭ(Ϭ,ƻ,ɚ,Υ,Φ,Χ);bool ϯ=Ϯ(Ϫ,Ϭ,ƻ,ɚ,Υ,Φ,Χ);Ϫ.CustomData=ϰ(Ϭ,ƻ,ɚ,Υ,Φ,Χ);if(!ϯ){Echo(
$"[SpecialCargo] {Ϫ.CustomName} is full or cannot be filled right now.");}}}Dictionary<string,int>ϫ(string ϱ){var ϲ=new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);var ǻ=ϱ.Split(
new[]{'\n','\r'},StringSplitOptions.RemoveEmptyEntries);foreach(var Ǿ in ǻ){var ȃ=Ǿ.Split(':');if(ȃ.Length==2){string Ǆ=ȃ[0
].Trim();int Ŵ;if(int.TryParse(ȃ[1].Trim(),out Ŵ))ϲ[Ǆ]=Ŵ;else ϲ[Ǆ]=0;}}return ϲ;}string ϰ(Dictionary<string,int>ϲ,List<
IMyTerminalBlock>ƻ,List<IMyTerminalBlock>ɚ,List<IMyTerminalBlock>Υ,List<IMyTerminalBlock>Φ,List<IMyTerminalBlock>Χ){var Š=new
StringBuilder();ϳ(Š,ϲ,ƻ);ϳ(Š,ϲ,ɚ);ϳ(Š,ϲ,Υ);ϳ(Š,ϲ,Φ);ϳ(Š,ϲ,Χ);return Š.ToString();}void ϳ(StringBuilder Š,Dictionary<string,int>ϲ,List
<IMyTerminalBlock>Ǳ){var ϴ=new HashSet<string>(StringComparer.OrdinalIgnoreCase);foreach(var Μ in Ǳ){var Ų=Μ.GetInventory
(0);if(Ų==null)continue;var ˁ=new List<MyInventoryItem>();Ų.GetItems(ˁ);foreach(var Ɠ in ˁ){if(!ϴ.Contains(Ɠ.Type.
SubtypeId)){ϴ.Add(Ɠ.Type.SubtypeId);int Ŵ=ϲ.ContainsKey(Ɠ.Type.SubtypeId)?ϲ[Ɠ.Type.SubtypeId]:0;Š.AppendLine(Ɠ.Type.SubtypeId+
": "+(Ŵ>0?Ŵ.ToString():""));}}}}void ϭ(Dictionary<string,int>Ϭ,List<IMyTerminalBlock>ƻ,List<IMyTerminalBlock>ɚ,List<
IMyTerminalBlock>Υ,List<IMyTerminalBlock>Φ,List<IMyTerminalBlock>Χ){ϵ(Ϭ,ƻ);ϵ(Ϭ,ɚ);ϵ(Ϭ,Υ);ϵ(Ϭ,Φ);ϵ(Ϭ,Χ);}void ϵ(Dictionary<string,int>Ϭ,
List<IMyTerminalBlock>Ǳ){foreach(var Μ in Ǳ){if(Ɖ(Μ.CubeGrid))continue;var Ų=Μ.GetInventory(0);if(Ų==null)continue;var ˁ=new
List<MyInventoryItem>();Ų.GetItems(ˁ);foreach(var Ɠ in ˁ){if(!Ϭ.ContainsKey(Ɠ.Type.SubtypeId))Ϭ[Ɠ.Type.SubtypeId]=0;}}}bool
Ϯ(IMyCargoContainer Ϫ,Dictionary<string,int>Ϭ,List<IMyTerminalBlock>ƻ,List<IMyTerminalBlock>ɚ,List<IMyTerminalBlock>Υ,
List<IMyTerminalBlock>Φ,List<IMyTerminalBlock>Χ){var Ν=Ϫ.GetInventory(0);if(Ν==null)return true;Ϸ(Ϫ,Ϭ,ƻ,ɚ,Υ,Φ,Χ);bool ϸ=true
;foreach(var ˌ in Ϭ){var ȇ=ˌ.Key;var Ϲ=ˌ.Value;if(Ϲ<=0)continue;var ˏ=Ϻ(ȇ);if(ˏ==null){Echo(
"[SpecialCargo] Unknown item: "+ȇ);ϸ=false;continue;}double ϻ=(double)Ν.GetItemAmount(ˏ.Value);double Ώ=Ϲ-ϻ;if(Ώ<=0)continue;bool ϼ=false;if(!ϼ)ϼ=Ͻ(Ν,ˏ
.Value,Ώ,ƻ);if(!ϼ)ϼ=Ͻ(Ν,ˏ.Value,Ώ,ɚ);if(!ϼ)ϼ=Ͻ(Ν,ˏ.Value,Ώ,Υ);if(!ϼ)ϼ=Ͻ(Ν,ˏ.Value,Ώ,Φ);if(!ϼ)ϼ=Ͻ(Ν,ˏ.Value,Ώ,Χ);if(!ϼ){ϸ=
false;Echo("[SpecialCargo] Transfer failed for "+ȇ+" ("+ˏ.Value.ToString()+")");}}return ϸ;}bool Ͻ(IMyInventory ų,MyItemType
ˏ,double Ώ,List<IMyTerminalBlock>Ǳ,bool Ͼ=false){foreach(var Μ in Ǳ){if(Ɖ(Μ.CubeGrid))continue;var Ų=Μ.GetInventory(0);if
(Ų==null)continue;if(!Ͼ){var ˁ=new List<MyInventoryItem>();Ċ.Clear();Ų.GetItems(Ċ);foreach(var Ɠ in Ċ){if(Ɠ.Type!=ˏ)
continue;double Β=(double)Ɠ.Amount;double Ͽ=Math.Min(Β,Ώ);if(Ͽ<=0)continue;if(Ų.TransferItemTo(ų,Ɠ,(MyFixedPoint)Ͽ)){Ώ-=Ͽ;if(Ώ<=
0)return true;}}}else{double Λ=Ώ;var ˁ=new List<MyInventoryItem>();ų.GetItems(ˁ);foreach(var Ɠ in ˁ){if(Ɠ.Type!=ˏ)
continue;double Β=(double)Ɠ.Amount;double Ͽ=Math.Min(Β,Λ);if(Ͽ<=0)continue;if(ų.TransferItemTo(Ų,Ɠ,(MyFixedPoint)Ͽ)){Λ-=Ͽ;if(Λ<=
0)return true;}}}}return Ώ<=0;}void Ϸ(IMyCargoContainer Ϫ,Dictionary<string,int>Ϭ,List<IMyTerminalBlock>ƻ,List<
IMyTerminalBlock>ɚ,List<IMyTerminalBlock>Υ,List<IMyTerminalBlock>Φ,List<IMyTerminalBlock>Χ){var Ų=Ϫ.GetInventory(0);if(Ų==null)return;
var ˁ=new List<MyInventoryItem>();Ų.GetItems(ˁ);foreach(var Ɠ in ˁ){string ǳ=Ɠ.Type.SubtypeId.ToString();double Ʋ=(double)Ɠ
.Amount;int Ѐ=Ϭ.ContainsKey(ǳ)?Ϭ[ǳ]:0;if(Ʋ>Ѐ){double ɜ=Ʋ-Ѐ;bool ĳ=false;string ζ=Ɠ.Type.TypeId;if(ζ==
"MyObjectBuilder_Ore")ĳ=Ͻ(Ų,new MyItemType(ζ,ǳ),ɜ,ƻ,Ͼ:true);else if(ζ=="MyObjectBuilder_Ingot")ĳ=Ͻ(Ų,new MyItemType(ζ,ǳ),ɜ,ɚ,Ͼ:true);else if(
ζ=="MyObjectBuilder_Component")ĳ=Ͻ(Ų,new MyItemType(ζ,ǳ),ɜ,Υ,Ͼ:true);else if(ζ=="MyObjectBuilder_AmmoMagazine")ĳ=Ͻ(Ų,new
MyItemType(ζ,ǳ),ɜ,Φ,Ͼ:true);else if(ζ=="MyObjectBuilder_PhysicalGunObject"||ζ=="MyObjectBuilder_ConsumableItem")ĳ=Ͻ(Ų,new
MyItemType(ζ,ǳ),ɜ,Χ,Ͼ:true);if(!ĳ){Echo($"[SpecialCargo] Could not move excess {ǳ} from {Ϫ.CustomName}");}}}}MyItemType?Ϻ(string ǳ
){if(string.IsNullOrWhiteSpace(ǳ))return null;ǳ=ǳ.Trim();var Δ=Ё(ǳ);if(Δ.HasValue)return Δ.Value;MyDefinitionId Ђ;string[
]Ѓ=new[]{"MyObjectBuilder_AmmoMagazine","MyObjectBuilder_Component","MyObjectBuilder_Ingot"};foreach(var ζ in Ѓ){if(
MyDefinitionId.TryParse(ζ+"/"+ǳ,out Ђ)){return new MyItemType(ζ,Ђ.SubtypeId.ToString());}}return null;}MyItemType?Ё(string ǳ){var ǿ=
new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocks(ǿ);for(int œ=0;œ<ǿ.Count;œ++){var Ų=ǿ[œ].GetInventory(0);if(Ų==
null)continue;var ˁ=new List<MyInventoryItem>();Ų.GetItems(ˁ);for(int ƒ=0;ƒ<ˁ.Count;ƒ++){var Ɠ=ˁ[ƒ];if(Ɠ.Type.SubtypeId.
ToString().Equals(ǳ,StringComparison.OrdinalIgnoreCase)){string ζ=Ɠ.Type.TypeId;if(ζ=="MyObjectBuilder_Component"||ζ==
"MyObjectBuilder_Ingot"||ζ=="MyObjectBuilder_AmmoMagazine"){return new MyItemType(ζ,Ɠ.Type.SubtypeId.ToString());}}}}return null;}void А(
MySpriteDrawFrame Є,string Ѕ,float І,float Ї,int Ј,int ɂ){const int Љ=8;int ˊ=Ј-ɂ;if(ˊ<0)return;int Њ=Math.Min(255,Math.Max(255,ˊ*20));if
(Њ<=0)return;bool Ћ=ˊ>=Љ;string ȟ=Ћ?"[ OK ] ":"[ .. ] ";Color Ќ=Ћ?new Color(0,255,120,Њ):new Color(255,0,0,Њ);Color Ѝ=new
Color(255,170,60,Њ);bool Ў=Ѕ.Contains("Master PB Detected")||Ѕ.Contains("Normal Mode")||Ѕ.Contains("Station Mode Detected")||
Ѕ.Contains("Ship Mode Detected");string Џ=Ў?Ѕ:Ћ?Ѕ.Replace("Initializing","").Trim():Ѕ;Є.Add(new MySprite(){Type=
SpriteType.TEXT,Data=ȟ,Position=new Vector2(І-220f,Ї),Color=Ќ,Alignment=TextAlignment.LEFT,RotationOrScale=0.75f});Є.Add(new
MySprite(){Type=SpriteType.TEXT,Data=Џ,Position=new Vector2(І-160f,Ї),Color=Ѝ,Alignment=TextAlignment.LEFT,RotationOrScale=0.75f
});}List<string>Б(){var ǻ=new List<string>();ǻ.Add("Initializing Core Systems");ǻ.Add("Initializing Control Modules");ǻ.
Add("Initializing OS Config");ǻ.Add("Initializing Resource Management Systems");ǻ.Add("Initializing Production Module");ǻ.
Add("Initializing Reactor Handling Module");ǻ.Add("Initializing Automation Module");ǻ.Add("Initializing LCD Menu Module");
if(ŧ(Me.CubeGrid))ǻ.Add("Master PB Detected");else ǻ.Add("Running Normal Mode");if(ţ())ǻ.Add("Station Mode Detected");else
ǻ.Add("Ship Mode Detected");return ǻ;}void Ĭ(){var Ġ=new HashSet<IMyCubeGrid>();Ũ(Ġ);var T=new List<IMyTextPanel>();
GridTerminalSystem.GetBlocksOfType(T,В=>В.IsFunctional&&В.CustomName.Contains(LCDTAG)&&Ġ.Contains(В.CubeGrid));foreach(var ǫ in T){ǫ.
ContentType=ContentType.SCRIPT;var Г=ǫ as IMyTextSurface;using(var Є=Г.DrawFrame()){Vector2 Д=Г.TextureSize;float І=Д.X*0.5f;float
Е=Д.Y*0.5f;double Ј=B/C;Є.Add(new MySprite(){Type=SpriteType.TEXTURE,Data="SquareSimple",Color=new Color(5,12,20,255),
Position=Д*0.5f,Size=Д,Alignment=TextAlignment.CENTER});float Ж=(float)((Ј*4)%Д.Y);Є.Add(new MySprite(){Type=SpriteType.TEXTURE,
Data="SquareSimple",Color=new Color(0,150,255,20),Position=new Vector2(І,Ж),Size=new Vector2(Д.X,5f),Alignment=TextAlignment
.CENTER});if(Ј>5){byte Њ=(byte)Math.Min(255,Ј*4);Є.Add(new MySprite(){Type=SpriteType.TEXT,Data="STC v3.0",Position=new
Vector2(І-220,15),RotationOrScale=2.1f,Color=new Color(0,255,255,Њ),Alignment=TextAlignment.LEFT});Є.Add(new MySprite(){Type=
SpriteType.TEXT,Data="Grid Manager",Position=new Vector2(І-220,75),RotationOrScale=1.3f,Color=new Color(0,200,255,Њ),Alignment=
TextAlignment.LEFT});}var З=Б();float И=Е-100;float Й=22;for(int œ=0;œ<З.Count;œ++){int ɂ=10+œ*10;float К=И+(œ*Й);А(Є,З[œ],І,К,(int)Ј
,ɂ);}if(Ј>110){byte Њ=(byte)Math.Min(255,Ј*4);Є.Add(new MySprite(){Type=SpriteType.TEXT,Data="Developed by Raidfire",
Position=new Vector2(І-(-80),0),RotationOrScale=0.65f,Color=new Color(0,200,255,Њ),Alignment=TextAlignment.LEFT});}if(Ј>20){
float Л=(float)Math.Min(1f,(Ј-20)/100f);const int М=12;int ƕ=(int)(Л*М);string Н="["+new string('█',ƕ)+new string('░',М-ƕ)+
$"] {(int)(Л*100)}%";Є.Add(new MySprite(){Type=SpriteType.TEXT,Data="BOOT PROGRESS: "+Н,Position=new Vector2(І,Е+200),Color=new Color(0,200,
255,255),Alignment=TextAlignment.CENTER,RotationOrScale=0.75f,FontId="Monospace"});}if(Ј>=D){int О=(int)(Ј-D);if(О<=E){
float П=(float)(Math.Sin(Runtime.TimeSinceLastRun.TotalSeconds*1f)*0.1f+0.1f);byte Р=(byte)(150+П*105);float Њ=1f;int С=(int)
(E*0.1f);if(О>С){int Т=О-С;Њ=Math.Max(0f,1f-(Т/(float)(E-С)));}byte У=(byte)(255*Њ);Є.Add(new MySprite(){Type=SpriteType.
TEXT,Data="SYSTEM ONLINE",Position=new Vector2(І,Е+160),Color=new Color(Р,255,200,У),Alignment=TextAlignment.CENTER,
RotationOrScale=1.2f});}else{A=true;}}}}B++;}
