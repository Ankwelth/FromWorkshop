// =_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-
// Orion Base Management AI - Total Auto
//
// OBJECTIVE:
// 1. (Nanobots) Manages the production queue for "Nanobot Build and Repair".
// 2. (Inventory) Automatically organizes and BALANCES base items (IIM logic).
// 3. (Refueling) Automatically refuels docked ships and the base itself.
// 4. (Autonomous) Runs autonomously, without a Timer Block.
// 5. (Auto-Config) Automatically names and balances cargo containers.
// 6. (Advanced Display) Shows LCD panels with detailed base status.
// 7. (Autocrafting) Maintains a defined stock level, with a 100%
//    manual and stable quota system, respecting user settings.
//
// IMPROVEMENTS (v28 - Tagged Subgrid Control):
// - ADDED: Connector Tag '[AllowManage]' to opt-in specific connected subgrids.
//   Place '[AllowManage]' on the SUBGRID'S connector connected to the main base.
//   This allows management even if MANAGE_SUBGRIDS is false.
// - SUBGRID MANAGEMENT IS OPT-IN: Added 'MANAGE_SUBGRIDS' setting (default false).
//   Manages piston/rotor subgrids only if true.
// - CONFIGURATION CLEANUP: Moved Autocraft margins to the top settings area.
// - ADDED: [IngotLCD] panel.
// =======================================================================================
// CREDITS & REFERENCES:
// - Original Author: Zaari
// - Original Name: Orion Base Management AI - Total Auto
// - IIM Logics adapted from Isy's Inventory Manager
// =======================================================================================

// --- CONFIGURATION - PART 1: CORE BEHAVIOR ---

// --- NEW: Grid Management ---
// Does the script manage inventories on subgrids attached via PISTONS/ROTORS?
// Default is FALSE. Does NOT affect grids connected via tagged Connectors.
const bool MANAGE_SUBGRIDS = false;

// --- NEW: Subgrid Connector Tag ---
// Add this tag to a CONNECTOR ON THE SUBGRID (not the base connector)
// to allow the script to manage inventories on THAT specific subgrid,
// even if MANAGE_SUBGRIDS is false.
const string SUBGRID_ALLOW_TAG = "[AllowManage]";

// --- Autocrafting Margins ---
const double ASSEMBLE_MARGIN_PERCENT = 0.05; // Produces if stock < 95% of quota
const double DISASSEMBLE_MARGIN_PERCENT = 0.10; // Disassembles if stock > 110% of quota

// --- CONFIGURATION - PART 2: NAMES AND TAGS ---

// --- NANOBOTS MODULE ---
const string NBR_TAG = "[Nano]";

// --- INVENTORY ORGANIZATION MODULE ---
const string ORES_CONTAINER_TAG = "[ORES]";
const string INGOTS_CONTAINER_TAG = "[INGOTS]";
const string COMPONENTS_CONTAINER_TAG = "[COMPONENTS]";
const string AMMO_CONTAINER_TAG = "[AMMO]";
const string BOTTLES_CONTAINER_TAG = "[BOTTLES]";
const string IGNORE_CONTAINER_TAG = "[IGNORE]";

// --- AUTOCRAFTING MODULE ---
const string AUTOCRAFT_ASSEMBLER_TAG = "[AutoCraft]";
const string LEARN_ASSEMBLER_TAG = "!learn";
const string LEARN_MANY_ASSEMBLER_TAG = "!learnMany";

// --- REFUELING MODULE ---
const string URANIUM_SOURCE_TAG = "[FUEL-URANIUM]";
const string ICE_SOURCE_TAG = "[FUEL-ICE]";

// --- DISPLAY MODULE ---
const string LCD_TAG = "[NanoLCD]";
const string AUTOCRAFT_LCD_TAG = "[AutoCraftLCD]";
const string POWER_LCD_TAG = "[PowerLCD]";
const string INGOT_LCD_TAG = "[IngotLCD]";

// --- END OF CONFIGURATION ---

// --- CONFIGURATION - PART 3: STATIC LOGIC (Advanced) ---

// Refining Priority List (highest priority at the top)
private readonly List<string> fixedRefiningList = new List<string> {
    "Stone", "Uranium", "Ice", "Iron", "Nickel", "Cobalt",
    "Silicon", "Silver", "Gold", "Platinum", "Magnesium", "Scrap"
};

// --- END OF CONFIGURATION ---

// --- AVAILABLE COMMANDS (Arguments) ---
// reset_quotas      : Clears quotas in the [AutoCraftLCD]'s CustomData.
// reset_blueprints  : Clears the memory of learned blueprints.

// =======================================================================================

// --- GLOBAL VARIABLES ---
private IMyTerminalBlock nanobotSystem;
private List<IMyAssembler> autocraftAssemblers = new List<IMyAssembler>();
private List<IMyRefinery> allRefineries = new List<IMyRefinery>();
private IMyTextPanel lcdPanel;
private IMyTextPanel autocraftLcdPanel;
private IMyTextPanel powerLcdPanel;
private IMyTextPanel ingotLcdPanel;
private readonly Dictionary<string, int> requiredComponents = new Dictionary<string, int>();
private readonly Dictionary<string, int> autocraftQuotas = new Dictionary<string, int>();
private readonly Dictionary<string, int> _userDefinedQuotas = new Dictionary<string, int>();
private readonly Dictionary<string, long> itemCounts = new Dictionary<string, long>();
private readonly HashSet<string> _craftableItems = new HashSet<string>();
private readonly System.Text.RegularExpressions.Regex componentRegex = new System.Text.RegularExpressions.Regex(@"^(\w+)\s+(-?\d+)\s*$", System.Text.RegularExpressions.RegexOptions.Multiline);
private readonly System.Text.RegularExpressions.Regex quotaCustomDataRegex = new System.Text.RegularExpressions.Regex(@"^\s*([a-zA-Z0-9_]+)\s*=\s*([0-9]+)\s*$", System.Text.RegularExpressions.RegexOptions.Multiline);
private StringBuilder lcdLogOutput = new StringBuilder();
private StringBuilder lcdStatusDisplay = new StringBuilder();
private int runCounter = 0;
private int balancingStep = 0;
private List<string> _storageTags = new List<string> { ORES_CONTAINER_TAG, INGOTS_CONTAINER_TAG, COMPONENTS_CONTAINER_TAG, AMMO_CONTAINER_TAG, BOTTLES_CONTAINER_TAG };
private readonly Dictionary<string, string> _learnedBlueprintMap = new Dictionary<string, string>();
private IMyAssembler _learningAssembler = null;
private readonly Dictionary<string, int> _refiningPriorityMap = new Dictionary<string, int>();
private Dictionary<string, double> _ingotDeficit = new Dictionary<string, double>();

// Ingot Panel Variables
private readonly Dictionary<string, long> _ingotQuotas = new Dictionary<string, long>();
private readonly Dictionary<string, long> _ingotCounts = new Dictionary<string, long>();
private readonly Dictionary<string, long> _oreCounts = new Dictionary<string, long>();

// --- NEW: Cache for tagged subgrids ---
private HashSet<long> _allowedSubgridIds = new HashSet<long>();
// --- END NEW ---

// Component -> Required Ingots Map
private readonly Dictionary<string, List<string>> _componentIngotMap = new Dictionary<string, List<string>>()
{
    { "SteelPlate", new List<string> { "Iron" } }, { "InteriorPlate", new List<string> { "Iron" } },
    { "Construction", new List<string> { "Iron", "Nickel" } }, { "Motor", new List<string> { "Iron", "Nickel" } },
    { "Computer", new List<string> { "Iron", "Silicon" } }, { "Display", new List<string> { "Iron", "Silicon" } },
    { "Girder", new List<string> { "Iron", "Nickel" } }, { "SmallTube", new List<string> { "Iron" } },
    { "LargeTube", new List<string> { "Iron" } }, { "RadioComponent", new List<string> { "Iron", "Silicon" } },
    { "Detector", new List<string> { "Iron", "Nickel" } }, { "Thrust", new List<string> { "Cobalt", "Gold", "Iron", "Nickel", "Platinum" } },
    { "GravityGenerator", new List<string> { "Cobalt", "Gold", "Iron" } }, { "PowerCell", new List<string> { "Iron", "Silicon", "Nickel" } },
    { "Superconductor", new List<string> { "Iron", "Gold" } }, { "Explosives", new List<string> { "Silicon", "Magnesium" } },
    { "Medical", new List<string> { "Silver", "Nickel", "Iron" } }, { "Reactor", new List<string> { "Silver", "Gold", "Stone", "Nickel" } },
    { "SolarCell", new List<string> { "Silicon", "Nickel" } }, { "BulletproofGlass", new List<string> { "Silicon" } },
    { "Canvas", new List<string> { "Silicon" } }, { "ZoneChip", new List<string> { "Iron", "Nickel", "Silicon", "Gold" } },
    { "Datapad", new List<string> { "Iron", "Silicon" } }, { "AutomaticRifle", new List<string> { "Iron", "Nickel" } },
    { "Welder", new List<string> { "Iron", "Nickel", "Cobalt" } }, { "Grinder", new List<string> { "Iron", "Nickel", "Cobalt" } },
    { "HandDrill", new List<string> { "Iron", "Nickel", "Cobalt" } }
};

// =======================================================================================
// SCRIPT START
// =======================================================================================
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;

    LoadBlueprintMap();

    for(int i = 0; i < fixedRefiningList.Count; i++)
    {
        _refiningPriorityMap[fixedRefiningList[i]] = i;
    }

    DiscoverBlocks();
    UpdateAllowedSubgrids(); // Initial population of allowed grids

    if (autocraftLcdPanel != null)
    {
        ParseQuotasFromCustomData(autocraftLcdPanel.CustomData);
    }

    CountAllItems();
    AssignContainerRoles(null, true);

    Echo($"Manager Script v28 (Tagged Subgrids) started. {_learnedBlueprintMap.Count} BPs loaded.");
}

IMyTextSurface GetTextSurface(IMyTerminalBlock block, int index = 0)
{
    if (block == null) return null;
    var provider = block as IMyTextSurfaceProvider;
    if (provider != null && provider.SurfaceCount > index)
    {
        return provider.GetSurface(index);
    }
    var surface = block as IMyTextSurface;
    if (surface != null && index == 0)
    {
        return surface;
    }
    return null;
}

// --- UPDATED HELPER: Checks main grid, global subgrid flag, OR tagged connector list ---
private bool IsOnManagedGrid(IMyTerminalBlock block)
{
    if (block == null) return false;
    // Main grid is always managed
    if (block.CubeGrid == Me.CubeGrid) return true;
    // Manage piston/rotor subgrids if flag is true AND it's on the same construct
    if (MANAGE_SUBGRIDS && block.IsSameConstructAs(Me)) return true;
    // Manage specific connector-attached subgrids if their ID is in the allowed list
    if (_allowedSubgridIds.Contains(block.CubeGrid.EntityId)) return true;

    return false;
}
// --- END UPDATED HELPER ---

// --- NEW FUNCTION: Finds subgrids connected via tagged connectors ---
void UpdateAllowedSubgrids()
{
    _allowedSubgridIds.Clear();
    var connectors = new List<IMyShipConnector>();
    // Get ALL connectors on the entire construct (main + all subgrids)
    GridTerminalSystem.GetBlocksOfType(connectors, c => c.IsSameConstructAs(Me));

    foreach(var c in connectors)
    {
        // Check if THIS connector has the tag, is connected, AND its partner is on the MAIN grid
        if (c.CustomName.Contains(SUBGRID_ALLOW_TAG) &&
            c.Status == MyShipConnectorStatus.Connected &&
            c.OtherConnector != null &&
            c.OtherConnector.CubeGrid == Me.CubeGrid)
        {
            // If yes, add the CubeGrid ID of THIS connector (the one on the subgrid) to the allowed list
            _allowedSubgridIds.Add(c.CubeGrid.EntityId);
        }
    }
    // Echo($"Allowed subgrid IDs: {_allowedSubgridIds.Count}"); // Optional debug
}
// --- END NEW FUNCTION ---

void DiscoverBlocks()
{
    var tempBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(NBR_TAG, tempBlocks, block => IsOnManagedGrid(block));
    nanobotSystem = tempBlocks.FirstOrDefault();

    GridTerminalSystem.GetBlocksOfType(autocraftAssemblers, b =>
        b.CustomName.Contains(AUTOCRAFT_ASSEMBLER_TAG) &&
        !b.CustomName.Contains(LEARN_ASSEMBLER_TAG) &&
        !b.CustomName.Contains(LEARN_MANY_ASSEMBLER_TAG) &&
        IsOnManagedGrid(b));

    GridTerminalSystem.GetBlocksOfType(allRefineries, b =>
        IsOnManagedGrid(b) &&
        b.IsWorking &&
        !b.CustomName.Contains(IGNORE_CONTAINER_TAG));

    var tempLcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(tempLcds, b => b.CustomName.Contains(LCD_TAG) && IsOnManagedGrid(b));
    lcdPanel = tempLcds.FirstOrDefault();

    var tempAutocraftLcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(tempAutocraftLcds, b => b.CustomName.Contains(AUTOCRAFT_LCD_TAG) && IsOnManagedGrid(b));
    autocraftLcdPanel = tempAutocraftLcds.FirstOrDefault();

    var tempPowerLcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(tempPowerLcds, b => b.CustomName.Contains(POWER_LCD_TAG) && IsOnManagedGrid(b));
    powerLcdPanel = tempPowerLcds.FirstOrDefault();

    var tempIngotLcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(tempIngotLcds, b => b.CustomName.Contains(INGOT_LCD_TAG) && IsOnManagedGrid(b));
    ingotLcdPanel = tempIngotLcds.FirstOrDefault();

    Echo($"Blocks rediscovered: {autocraftAssemblers.Count} Assemblers, {allRefineries.Count} Refin.");
}

public void Main(string argument, UpdateType updateSource)
{
    if (!string.IsNullOrEmpty(argument))
    {
        switch (argument.ToLower())
        {
            case "reset_quotas":
                ResetQuotas();
                return;
            case "reset_blueprints":
                Storage = "";
                _learnedBlueprintMap.Clear();
                Echo("Learned blueprint memory has been cleared.");
                return;
        }
    }

    var currentTickLog = new StringBuilder();

    // --- Update Allowed Subgrids periodically ---
    if (runCounter % 60 == 5) // Offset slightly from AssignContainerRoles
    {
        UpdateAllowedSubgrids();
    }
    // --- End Update ---

    if (runCounter % 60 == 0)
    {
        AssignContainerRoles(currentTickLog, false);
    }

    if (runCounter % 3 == 0)
    {
        CountAllItems();
    }

    if (runCounter % 10 == 0)
    {
        SortBaseInventory(currentTickLog);
        RefuelDockedShips(currentTickLog);
        ManageBasePower(currentTickLog);
    }

    if (runCounter % 20 == 0)
    {
        SortRefineryQueues(currentTickLog);

        if (allRefineries.Count > 0)
        {
            balancingStep = (balancingStep + 1) % allRefineries.Count;
            ManageRefineryBalancing(balancingStep, currentTickLog);
        }
    }

    if (runCounter % 5 == 0)
    {
        UpdateNanobotRequirements(currentTickLog);
        UpdateAndParseAutocraftLcd();
        ManageBlueprintLearning(currentTickLog);
        ManageAutocrafting(currentTickLog);
    }

    if (runCounter % 2 == 0 || runCounter == 0)
    {
        UpdateLcdStatus();
        UpdatePowerLcd();
        UpdateIngotLcd();
    }

    if(currentTickLog.Length > 0)
    {
        lcdLogOutput.Clear();
        lcdLogOutput.Append(currentTickLog);
    }

    if (runCounter > 0 && runCounter % 120 == 0)
    {
        DiscoverBlocks();
    }

    UpdateLcdDisplay();
    runCounter++;
}

// =======================================================================================
// HELPER FUNCTIONS (Remaining)
// =======================================================================================

bool IsManagedInventoryBlock(IMyTerminalBlock block)
{
    // Uses updated IsOnManagedGrid
    if (block == null || !block.HasInventory || !IsOnManagedGrid(block))
        return false;

    if (block.CustomName.Contains(IGNORE_CONTAINER_TAG))
        return false;

    return block is IMyCargoContainer ||
           block is IMyProductionBlock ||
           block is IMyGasGenerator ||
           block is IMyReactor ||
           block is IMyShipConnector ||
           block is IMyCollector ||
           block is IMyShipController ||
           block is IMyRemoteControl;
}

List<IMyInventory> FindInventoriesWithTag(string tag) { var c = new List<IMyCargoContainer>(); GridTerminalSystem.GetBlocksOfType(c, b => b.CustomName.Contains(tag) && IsOnManagedGrid(b)); return c.Select(b => b.GetInventory()).ToList(); }
bool NeedsFuel(IMyInventory i, float t) { if (i == null) return false; return i.CurrentVolume.RawValue < (i.MaxVolume.RawValue * t); }

bool MoveFuel(MyDefinitionId i, List<IMyInventory> s, IMyInventory d, int a)
{
    if (d == null || s == null || s.Count == 0) return false;
    foreach (var src in s) {
        if (src == null) continue;
        var it = src.FindItem(i);
        if (it.HasValue && it.Value.Amount > 0) {
            var t = (VRage.MyFixedPoint)Math.Min(a, it.Value.Amount.ToIntSafe());
            if (d.CanItemsBeAdded(t, it.Value.Type)) {
                if (src.TransferItemTo(d, it.Value, t)) return true;
            }
        }
    }
    return false;
}

bool MoveFuel(string fId, List<IMyInventory> s, IMyInventory d, int a)
{
    MyDefinitionId i;
    if (!MyDefinitionId.TryParse(fId, out i)) return false;
    return MoveFuel(i, s, d, a);
}

string FormatLargeNumber(double number)
{
    if (number >= 1000000000)
        return (number / 1000000000).ToString("0.0") + " G";
    if (number >= 1000000)
        return (number / 1000000).ToString("0.0") + " M";
    if (number >= 1000)
        return (number / 1000).ToString("0.#") + " k";
    return number.ToString("0");
}

// =======================================================================================
// MODULE 1: INVENTORY ORGANIZATION AND BALANCING
// =======================================================================================
void SortBaseInventory(StringBuilder log)
{
    var destinationMap = new Dictionary<string, List<IMyInventory>>();
    var uraniumDestinations = new List<IMyInventory>();
    var iceDestinations = new List<IMyInventory>();

    Action<string, string> mapDestination = (tag, category) => {
        var list = new List<IMyCargoContainer>();
        GridTerminalSystem.GetBlocksOfType(list, b => b.CustomName.Contains(tag) && IsOnManagedGrid(b));
        if (list.Count > 0)
        {
            destinationMap[category] = list.Select(c => c.GetInventory())
                                            .OrderBy(inv => (double)inv.CurrentVolume / (double)inv.MaxVolume)
                                            .ToList();
            if (category == "Ingot")
            {
                uraniumDestinations = list.Where(c => c.CustomName.Contains(URANIUM_SOURCE_TAG))
                                          .Select(c => c.GetInventory())
                                          .OrderBy(inv => (double)inv.CurrentVolume / (double)inv.MaxVolume)
                                          .ToList();
            }
            if (category == "Ore")
            {
                iceDestinations = list.Where(c => c.CustomName.Contains(ICE_SOURCE_TAG))
                                      .Select(c => c.GetInventory())
                                      .OrderBy(inv => (double)inv.CurrentVolume / (double)inv.MaxVolume)
                                      .ToList();
            }
        }
    };

    mapDestination(ORES_CONTAINER_TAG, "Ore"); mapDestination(INGOTS_CONTAINER_TAG, "Ingot");
    mapDestination(COMPONENTS_CONTAINER_TAG, "Component"); mapDestination(AMMO_CONTAINER_TAG, "AmmoMagazine");
    mapDestination(BOTTLES_CONTAINER_TAG, "GasContainer");

    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(allBlocks, block => IsManagedInventoryBlock(block));
    bool movedItem = false;

    foreach (var block in allBlocks)
    {
        var prodBlock = block as IMyProductionBlock;
        if(prodBlock != null && prodBlock.IsProducing) continue;

        for (int i = 0; i < block.InventoryCount; i++)
        {
            var sourceInventory = block.GetInventory(i);
            var items = new List<MyInventoryItem>();
            sourceInventory.GetItems(items);

            for (int j = items.Count - 1; j >= 0; j--)
            {
                var item = items[j];
                string generalCategory = item.Type.GetItemInfo().IsOre ? "Ore" : item.Type.TypeId.ToString().Replace("MyObjectBuilder_", "");
                if (generalCategory == "PhysicalGunObject") generalCategory = "Component";
                if (generalCategory == "OxygenContainerObject") generalCategory = "GasContainer";

                List<IMyInventory> targetDestinations = null;
                string listToSort = null;
                string itemSubtypeId = item.Type.SubtypeId.ToString();

                if (generalCategory == "Ingot" && itemSubtypeId == "Uranium" && uraniumDestinations.Count > 0)
                {
                    targetDestinations = uraniumDestinations;
                    listToSort = "Uranium";
                }
                else if (generalCategory == "Ore" && itemSubtypeId == "Ice" && iceDestinations.Count > 0)
                {
                    targetDestinations = iceDestinations;
                    listToSort = "Ice";
                }
                else if (destinationMap.ContainsKey(generalCategory))
                {
                    targetDestinations = destinationMap[generalCategory];
                    listToSort = generalCategory;
                }
                else
                {
                    continue;
                }

                bool alreadyInCorrectContainer = false;
                foreach(var destInv in targetDestinations)
                {
                    if (destInv.Owner.EntityId == sourceInventory.Owner.EntityId)
                    {
                        alreadyInCorrectContainer = true;
                        break;
                    }
                }
                if (alreadyInCorrectContainer) continue;

                foreach (var destInv in targetDestinations)
                {
                    if (destInv.IsFull) continue;
                    if(sourceInventory.TransferItemTo(destInv, item))
                    {
                        movedItem = true;
                        if (listToSort == "Uranium") {
                            uraniumDestinations = uraniumDestinations.OrderBy(inv => (double)inv.CurrentVolume / (double)inv.MaxVolume).ToList();
                        } else if (listToSort == "Ice") {
                            iceDestinations = iceDestinations.OrderBy(inv => (double)inv.CurrentVolume / (double)inv.MaxVolume).ToList();
                        } else if (listToSort != null && destinationMap.ContainsKey(listToSort)) {
                            destinationMap[listToSort] = destinationMap[listToSort].OrderBy(inv => (double)inv.CurrentVolume / (double)inv.MaxVolume).ToList();
                        }
                        break;
                    }
                }
            }
        }
    }
    if (movedItem) log.AppendLine("- Inventory sorted and balanced (w/ fuel prio.).");
}

// =======================================================================================
// MODULE 2: (SMART) SHIP REFUELING
// =======================================================================================
void RefuelDockedShips(StringBuilder log)
{
    var baseConnectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(baseConnectors, c => IsOnManagedGrid(c) && c.Status == MyShipConnectorStatus.Connected && c.OtherConnector != null && !c.OtherConnector.IsSameConstructAs(Me));
    if (baseConnectors.Count == 0) return;

    var uraniumSources = FindInventoriesWithTag(URANIUM_SOURCE_TAG);
    var iceSources = FindInventoriesWithTag(ICE_SOURCE_TAG);
    var bottleSources = FindInventoriesWithTag(BOTTLES_CONTAINER_TAG);
    var ammoSources = FindInventoriesWithTag(AMMO_CONTAINER_TAG);
    bool refueledSomething = false;

    MyDefinitionId uId; MyDefinitionId.TryParse("MyObjectBuilder_Ingot/Uranium", out uId);
    MyDefinitionId iId; MyDefinitionId.TryParse("MyObjectBuilder_Ore/Ice", out iId);
    MyDefinitionId bId; MyDefinitionId.TryParse("MyObjectBuilder_GasContainerObject/HydrogenBottle", out bId);
    MyDefinitionId gAmmoId; MyDefinitionId.TryParse("MyObjectBuilder_AmmoMagazine/NATO_25x184mm", out gAmmoId);
    MyDefinitionId mAmmoId; MyDefinitionId.TryParse("MyObjectBuilder_AmmoMagazine/Missile200mm", out mAmmoId);
    MyDefinitionId iAmmoId; MyDefinitionId.TryParse("MyObjectBuilder_AmmoMagazine/NATO_5p56x45mm", out iAmmoId);

    foreach (var connector in baseConnectors)
    {
        var shipGrid = connector.OtherConnector.CubeGrid;

        // --- 1. URANIUM REFUELING ---
        var shipReactors = new List<IMyReactor>();
        GridTerminalSystem.GetBlocksOfType(shipReactors, r => r.CubeGrid == shipGrid);
        if (shipReactors.Count > 0 && uraniumSources.Count > 0)
        {
            long totalShipUranium = 0;
            foreach (var r in shipReactors) { totalShipUranium += r.GetInventory().GetItemAmount(uId).ToIntSafe(); }
            const long SHIP_URANIUM_MIN_TARGET = 50;
            const int SHIP_URANIUM_BATCH_SIZE = 20;
            if (totalShipUranium < SHIP_URANIUM_MIN_TARGET)
            {
                var targetReactor = shipReactors.FirstOrDefault(r => !r.GetInventory().IsFull);
                if (targetReactor != null)
                {
                    if (MoveFuel(uId, uraniumSources, targetReactor.GetInventory(), SHIP_URANIUM_BATCH_SIZE))
                        refueledSomething = true;
                }
            }
        }

        // --- 2. ICE REFUELING ---
        var shipHydrogenTanks = new List<IMyGasTank>(); GridTerminalSystem.GetBlocksOfType(shipHydrogenTanks, t => t.CubeGrid == shipGrid && t.BlockDefinition.SubtypeId.Contains("Hydrogen"));
        var allBlocksOnShip = new List<IMyTerminalBlock>(); GridTerminalSystem.GetBlocksOfType(allBlocksOnShip, b => b.CubeGrid == shipGrid);
        bool hasHydrogenEngine = allBlocksOnShip.Any(b => b.BlockDefinition.SubtypeId.Contains("HydrogenEngine"));
        bool needsHydrogen = shipHydrogenTanks.Any(t => t.FilledRatio < 0.95f) || hasHydrogenEngine;
        if(needsHydrogen && iceSources.Count > 0)
        {
            var shipGasGens = new List<IMyGasGenerator>();
            GridTerminalSystem.GetBlocksOfType(shipGasGens, g => g.CubeGrid == shipGrid);
            if (shipGasGens.Count > 0)
            {
                long totalShipIce = 0;
                foreach (var g in shipGasGens) { totalShipIce += g.GetInventory().GetItemAmount(iId).ToIntSafe(); }
                const long SHIP_ICE_MIN_TARGET = 10000;
                const int SHIP_ICE_BATCH_SIZE = 5000;
                if (totalShipIce < SHIP_ICE_MIN_TARGET)
                {
                    var targetGen = shipGasGens.FirstOrDefault(g => !g.GetInventory().IsFull);
                    if (targetGen != null)
                    {
                        if (MoveFuel(iId, iceSources, targetGen.GetInventory(), SHIP_ICE_BATCH_SIZE))
                            refueledSomething = true;
                    }
                }
            }
        }

        // --- 3. BOTTLE REFUELING ---
        var shipCockpits = new List<IMyCockpit>();
        GridTerminalSystem.GetBlocksOfType(shipCockpits, c => c.CubeGrid == shipGrid && c.HasInventory);
        if (shipCockpits.Count > 0 && bottleSources.Count > 0)
        {
            bool shipHasBottle = false;
            foreach(var cockpit in shipCockpits)
            {
                if (cockpit.GetInventory().GetItemAmount(bId) > 0)
                {
                    shipHasBottle = true;
                    break;
                }
            }
            if (!shipHasBottle)
            {
                var targetCockpit = shipCockpits.FirstOrDefault(c => !c.GetInventory().IsFull);
                if (targetCockpit != null)
                {
                    if (MoveFuel(bId, bottleSources, targetCockpit.GetInventory(), 1))
                        refueledSomething = true;
                }
            }
        }

        // --- 4. AMMUNITION REFUELING ---
        if (ammoSources.Count > 0)
        {
            const long SHIP_GATLING_AMMO_MIN = 4;
            const int SHIP_GATLING_AMMO_BATCH = 2;
            var gatlingWeapons = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType(gatlingWeapons, b => b.CubeGrid == shipGrid && b.HasInventory && (b.BlockDefinition.SubtypeId.Contains("GatlingTurret") || b.BlockDefinition.SubtypeId.Contains("GatlingGun")));
            if(gatlingWeapons.Count > 0)
            {
                long totalGatlingAmmo = 0;
                foreach (var w in gatlingWeapons) { totalGatlingAmmo += w.GetInventory().GetItemAmount(gAmmoId).ToIntSafe(); }
                if (totalGatlingAmmo < SHIP_GATLING_AMMO_MIN)
                {
                    var targetWeapon = gatlingWeapons.FirstOrDefault(w => !w.GetInventory().IsFull);
                    if (targetWeapon != null) {
                        if (MoveFuel(gAmmoId, ammoSources, targetWeapon.GetInventory(), SHIP_GATLING_AMMO_BATCH))
                            refueledSomething = true;
                    }
                }
            }

            const long SHIP_MISSILE_AMMO_MIN = 10;
            const int SHIP_MISSILE_AMMO_BATCH = 5;
            var missileWeapons = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType(missileWeapons, b => b.CubeGrid == shipGrid && b.HasInventory && (b.BlockDefinition.SubtypeId.Contains("MissileTurret") || b.BlockDefinition.SubtypeId.Contains("MissileLauncher")));
            if(missileWeapons.Count > 0)
            {
                long totalMissileAmmo = 0;
                foreach (var w in missileWeapons) { totalMissileAmmo += w.GetInventory().GetItemAmount(mAmmoId).ToIntSafe(); }
                if (totalMissileAmmo < SHIP_MISSILE_AMMO_MIN)
                {
                    var targetWeapon = missileWeapons.FirstOrDefault(w => !w.GetInventory().IsFull);
                    if (targetWeapon != null) {
                        if (MoveFuel(mAmmoId, ammoSources, targetWeapon.GetInventory(), SHIP_MISSILE_AMMO_BATCH))
                            refueledSomething = true;
                    }
                }
            }

            const long SHIP_INTERIOR_AMMO_MIN = 2;
            const int SHIP_INTERIOR_AMMO_BATCH = 1;
            var interiorTurrets = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType(interiorTurrets, t => t.CubeGrid == shipGrid && t.HasInventory && t.BlockDefinition.SubtypeId.Contains("InteriorTurret"));
            if(interiorTurrets.Count > 0)
            {
                long totalInteriorAmmo = 0;
                foreach (var t in interiorTurrets) { totalInteriorAmmo += t.GetInventory().GetItemAmount(iAmmoId).ToIntSafe(); }
                if (totalInteriorAmmo < SHIP_INTERIOR_AMMO_MIN)
                {
                    var targetTurret = interiorTurrets.FirstOrDefault(t => !t.GetInventory().IsFull);
                    if (targetTurret != null) {
                        if (MoveFuel(iAmmoId, ammoSources, targetTurret.GetInventory(), SHIP_INTERIOR_AMMO_BATCH))
                            refueledSomething = true;
                    }
                }
            }
        }
    }
    if (refueledSomething) log.AppendLine("- Ships refueled (Safe Mode).");
}


// =======================================================================================
// MODULE 3: BASE POWER MANAGEMENT
// =======================================================================================
void ManageBasePower(StringBuilder log)
{
    bool actionTaken = false;
    var baseReactors = new List<IMyReactor>(); GridTerminalSystem.GetBlocksOfType(baseReactors, r => IsOnManagedGrid(r));
    if (baseReactors.Count > 0)
    {
        var uraniumSources = FindInventoriesWithTag(URANIUM_SOURCE_TAG); MyDefinitionId uId; MyDefinitionId.TryParse("MyObjectBuilder_Ingot/Uranium", out uId);
        foreach (var r in baseReactors) { var inv = r.GetInventory(); if (inv.GetItemAmount(uId) < 5) { if (MoveFuel(uId, uraniumSources, inv, 10)) actionTaken = true; } }
    }
    var baseH2Gens = new List<IMyGasGenerator>(); GridTerminalSystem.GetBlocksOfType(baseH2Gens, g => IsOnManagedGrid(g));
    var baseH2Tanks = new List<IMyGasTank>(); GridTerminalSystem.GetBlocksOfType(baseH2Tanks, t => IsOnManagedGrid(t) && t.BlockDefinition.SubtypeId.Contains("Hydrogen"));
    bool needsH = baseH2Tanks.Any(t => t.FilledRatio < 0.98f);
    if (baseH2Gens.Count > 0 && needsH)
    {
        var iceSources = FindInventoriesWithTag(ICE_SOURCE_TAG);
        foreach (var g in baseH2Gens) { if (NeedsFuel(g.GetInventory(), 0.8f)) { if(MoveFuel("MyObjectBuilder_Ore/Ice", iceSources, g.GetInventory(), 5000)) actionTaken = true; } }
    }
    var batteries = new List<IMyBatteryBlock>(); GridTerminalSystem.GetBlocksOfType(batteries, b => IsOnManagedGrid(b));
    foreach(var b in batteries) { if (b.ChargeMode != ChargeMode.Auto) { b.ChargeMode = ChargeMode.Auto; actionTaken = true; } }
    if(actionTaken) log.AppendLine("- Base power managed.");
}

// =======================================================================================
// MODULE 4: AUTOMATIC CONTAINER MANAGEMENT
// =======================================================================================
void AssignContainerRoles(StringBuilder log, bool force = false)
{
    var allC = new List<IMyCargoContainer>(); GridTerminalSystem.GetBlocksOfType(allC, c => IsOnManagedGrid(c) && !c.CustomName.Contains(IGNORE_CONTAINER_TAG));
    bool needsReassign = force || allC.Any(c => !_storageTags.Any(t => c.CustomName.Contains(t)) && !c.CustomName.Contains(URANIUM_SOURCE_TAG) && !c.CustomName.Contains(ICE_SOURCE_TAG));
    if (!needsReassign) return;
    if (log != null) log.AppendLine("- Reconfiguring storage..."); Echo("Reconfiguring storage...");
    if (allC.Count == 0) { Echo("No cargo containers found to configure."); return; }
    foreach (var c in allC)
    {
        string n = c.CustomName; foreach (string t in _storageTags) n = n.Replace(t, ""); n = n.Replace(URANIUM_SOURCE_TAG, "").Replace(ICE_SOURCE_TAG, ""); n = System.Text.RegularExpressions.Regex.Replace(n, @"\s*-\s*(C|Container)\d+\s*$", "").Trim(); if (string.IsNullOrWhiteSpace(n)) n = "Cargo Container"; c.CustomName = n;
    }
    var sortedC = allC.OrderByDescending(c => c.GetInventory().MaxVolume.RawValue).ToList(); var a = new Dictionary<IMyCargoContainer, List<string>>(); foreach(var c in sortedC) a[c] = new List<string>();
    var pTags = new List<string> { COMPONENTS_CONTAINER_TAG, INGOTS_CONTAINER_TAG, ORES_CONTAINER_TAG }; var sTags = new List<string> { AMMO_CONTAINER_TAG, BOTTLES_CONTAINER_TAG };
    for(int i = 0; i < sortedC.Count; i++) a[sortedC[i]].Add(pTags[i % pTags.Count]);
    var oC = a.FirstOrDefault(kvp => kvp.Value.Contains(ORES_CONTAINER_TAG)).Key; if (oC != null) a[oC].Add(ICE_SOURCE_TAG);
    var iC = a.FirstOrDefault(kvp => kvp.Value.Contains(INGOTS_CONTAINER_TAG)).Key; if (iC != null) a[iC].Add(URANIUM_SOURCE_TAG);
    var smallC = sortedC.OrderBy(c => c.GetInventory().MaxVolume.RawValue).ToList(); for(int i = 0; i < sTags.Count && i < smallC.Count; i++) if(a[smallC[i]].Count < 2) a[smallC[i]].Add(sTags[i]);
    int num = 1; foreach(var kvp in a) { var c = kvp.Key; var bN = c.CustomName; var t = kvp.Value; var nN = new StringBuilder(bN); foreach(var tag in t) nN.Append(" ").Append(tag); nN.Append($" - C{num++:D2}"); c.CustomName = nN.ToString().Replace("  "," ").Trim(); }
    Echo("Container configuration complete!");
}

// =======================================================================================
// MODULE 5: AUTOCRAFTING AND LEARNING
// =======================================================================================
void LoadBlueprintMap() { _lBM.Clear(); var l = Storage.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); foreach (var ln in l) { var p = ln.Split(';'); if (p.Length == 2) _lBM[p[0]] = p[1]; } }
void SaveBlueprintMap() { var sb = new StringBuilder(); foreach (var kvp in _lBM) sb.AppendLine($"{kvp.Key};{kvp.Value}"); Storage = sb.ToString(); }
string GetBlueprintSubtype(string iS) { if (_lBM.ContainsKey(iS)) return _lBM[iS]; switch (iS) { case "Computer": case "Construction": case "Motor": case "Thrust": case "Explosives": case "Girder": case "Detector": return iS + "Component"; default: return iS; } }
void ManageBlueprintLearning(StringBuilder log)
{
    var cQ = new List<MyProductionItem>(); if (_lA != null) { if (_lA.Closed || !_lA.IsFunctional) { _lA = null; return; } var oI = _lA.OutputInventory; if (oI.ItemCount > 0) { var items = new List<MyInventoryItem>(); oI.GetItems(items); if (items.Count == 0) return; var pI = items[0]; _lA.GetQueue(cQ); if (cQ.Count == 0) { _lA = null; return; } var bU = cQ[0].BlueprintId; string iS = pI.Type.SubtypeId.ToString(); string bS = bU.SubtypeName.ToString(); _lBM[iS] = bS; SaveBlueprintMap(); log.AppendLine($"! LEARNED: {iS} -> {bS}"); Echo($"! LEARNED: {iS} -> {bS}"); _lA.ClearQueue(); MoveLearnedItem(oI); bool isLM = _lA.CustomName.Contains(LEARN_MANY_ASSEMBLER_TAG); if (!isLM) _lA.CustomName = _lA.CustomName.Replace(LEARN_ASSEMBLER_TAG, "").Trim(); _lA = null; } else { _lA.GetQueue(cQ); if (!_lA.IsProducing && cQ.Count == 0) _lA = null; } return; }
    var lAsm = new List<IMyAssembler>(); GridTerminalSystem.GetBlocksOfType(lAsm, b => b.CustomName.Contains(AUTOCRAFT_ASSEMBLER_TAG) && (b.CustomName.Contains(LEARN_ASSEMBLER_TAG) || b.CustomName.Contains(LEARN_MANY_ASSEMBLER_TAG)) && IsOnManagedGrid(b)); if (lAsm.Count == 0) return; var aTL = lAsm[0]; var lQ = new List<MyProductionItem>(); aTL.GetQueue(lQ);
    if (lQ.Count == 1 && lQ[0].Amount >= 100) { if (aTL.OutputInventory.ItemCount > 0) { if (!MoveLearnedItem(aTL.OutputInventory)) { log.AppendLine($"! Learn ERR: Did not clear output of {aTL.CustomName}."); return; } log.AppendLine($"- Clearing output of {aTL.CustomName}..."); } _lA = aTL; log.AppendLine($"- Learning started on {aTL.CustomName}."); Echo($"- Learning started on {aTL.CustomName}."); } else if (lQ.Count > 0) { log.AppendLine($"! Learn ERR: Invalid queue on {aTL.CustomName}."); }
}
bool MoveLearnedItem(IMyInventory oI) { var cC = FindInventoriesWithTag(COMPONENTS_CONTAINER_TAG); if (cC.Count == 0) return false; bool moved = false; foreach(var dI in cC) { if (dI.IsFull) continue; var items = new List<MyInventoryItem>(); oI.GetItems(items); if (items.Count == 0) break; for (int i = items.Count - 1; i >= 0; i--) if (oI.TransferItemTo(dI, items[i])) moved = true; if (oI.ItemCount == 0) break; } return moved || oI.ItemCount == 0; }
void UpdateNanobotRequirements(StringBuilder log)
{
    reqComps.Clear(); if (nanobotSystem == null) return; string dI = nanobotSystem.DetailedInfo; var matches = compRegex.Matches(dI);
    if (matches.Count > 0) { log.AppendLine("- Nanobots Require:"); foreach (System.Text.RegularExpressions.Match m in matches) { string cN = m.Groups[1].Value; int rA = 0; try { rA = int.Parse(m.Groups[2].Value); } catch { continue; } if (rA > 0) { reqComps[cN] = rA; log.AppendLine($"       - {cN}: {rA}"); if (!_cI.Contains(cN)) { _cI.Add(cN); if (!_uDQ.ContainsKey(cN)) _uDQ[cN] = 0; } } } }
}
void ParseQuotasFromCustomData(string cD) { _uDQ.Clear(); var m = quotaCustomDataRegex.Matches(cD); foreach (System.Text.RegularExpressions.Match match in m) { if (match.Groups.Count == 3) { string cN = match.Groups[1].Value; int wA = 0; if (int.TryParse(match.Groups[2].Value, out wA)) if (!string.IsNullOrWhiteSpace(cN)) _uDQ[cN] = wA; } } }
void UpdateAndParseAutocraftLcd()
{
    if (aLcdP == null) { Echo("ERROR: Panel [AutoCraftLCD] not found."); return; } IMyTextSurface s = GetTextSurface(aLcdP); if (s == null) { Echo("ERROR: Surface of [AutoCraftLCD] not found."); return; }
    ParseQuotasFromCustomData(aLcdP.CustomData); autoQuotas.Clear(); var nLC = new StringBuilder(); var nCDC = new StringBuilder(); var allMI = new HashSet<string>(_uDQ.Keys); foreach (var iN in _cI) allMI.Add(iN); foreach (var iN in reqComps.Keys) allMI.Add(iN); var sortedK = new List<string>(allMI); sortedK.Sort();
    nLC.AppendLine("== AUTOMATIC PRODUCTION MANAGER =="); nLC.AppendLine("!! Edit quotas in the panel's 'CUSTOM DATA' !!"); string hdr = "Status  | Component".PadRight(34) + "| Stock / Quota"; nLC.AppendLine(hdr); nLC.AppendLine(new String('-', hdr.Length));
    if (sortedK.Count == 0) nLC.AppendLine("\nNo trackable items."); else { foreach(var cN in sortedK) { int wA = 0; if (_uDQ.ContainsKey(cN)) wA = _uDQ[cN]; autoQuotas[cN] = wA; nCDC.AppendLine($"{cN}={wA}"); long cA = itemCounts.ContainsKey(cN) ? itemCounts[cN] : 0; string status; decimal aT = (decimal)wA * (1.0m - (decimal)ASM_MARGIN); decimal dT = (decimal)wA * (1.0m + (decimal)DSM_MARGIN); if (wA == 0) status = "[-----]"; else if (cA > dT) status = "[SURPLUS]"; else if (cA < aT) status = "[PROD ]"; else status = "[ OK ] "; string sAN = $"{status} | {cN}"; string sI = $"{cA}/{wA}"; string uL = sAN.PadRight(34) + "| " + sI; nLC.AppendLine(uL); } }
    if (aLcdP.CustomData != nCDC.ToString()) aLcdP.CustomData = nCDC.ToString(); s.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; s.Font = "Monospace"; s.FontSize = 0.5f; s.WriteText(nLC.ToString());
}

void CountAllItems()
{
    itemCounts.Clear();
    _cI.Clear();
    _ingotCounts.Clear();
    _oreCounts.Clear();

    var allIB = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(allIB, b => IsManagedInventoryBlock(b));

    var iB = new List<MyInventoryItem>();
    foreach(var blk in allIB)
    {
        for(int i = 0; i < blk.InventoryCount; i++)
        {
            var inv = blk.GetInventory(i);
            iB.Clear();
            inv.GetItems(iB);
            foreach(var item in iB)
            {
                string sN = item.Type.SubtypeId.ToString();
                string tId = item.Type.TypeId.ToString();
                long amount = item.Amount.ToIntSafe();

                if(itemCounts.ContainsKey(sN))
                    itemCounts[sN] += amount;
                else
                    itemCounts[sN] = amount;

                if (tId.EndsWith("_Component") || tId.EndsWith("_AmmoMagazine") || tId.EndsWith("_PhysicalGunObject") || tId.EndsWith("_GasContainerObject"))
                    _cI.Add(sN);

                if (tId.EndsWith("_Ingot"))
                {
                    if (_ingotCounts.ContainsKey(sN))
                        _ingotCounts[sN] += amount;
                    else
                        _ingotCounts[sN] = amount;
                }
                else if (tId.EndsWith("_Ore"))
                {
                    if (_oreCounts.ContainsKey(sN))
                        _oreCounts[sN] += amount;
                    else
                        _oreCounts[sN] = amount;
                }
            }
        }
    }
}

void ManageAutocrafting(StringBuilder log)
{
    if (autocraftAssemblers.Count == 0) return;

    foreach (var asm in autocraftAssemblers) { asm.CooperativeMode = false; }

    var itemsToProduce = new Dictionary<string, decimal>();
    var itemsToDisassemble = new Dictionary<string, decimal>();

    foreach(var quotaPair in autocraftQuotas)
    {
        long wanted = quotaPair.Value; if (wanted <= 0) continue;
        string itemName = quotaPair.Key; long current = itemCounts.ContainsKey(itemName) ? itemCounts[itemName] : 0;
        decimal assembleThreshold = (decimal)wanted * (1.0m - (decimal)ASSEMBLE_MARGIN_PERCENT);
        decimal disassembleThreshold = (decimal)wanted * (1.0m + (decimal)DISASSEMBLE_MARGIN_PERCENT);
        if (current < assembleThreshold) itemsToProduce[itemName] = (decimal)wanted - current;
        else if (current > disassembleThreshold) itemsToDisassemble[itemName] = current - (decimal)wanted;
    }
    foreach(var requiredPair in requiredComponents)
    {
        long wanted = requiredPair.Value; string itemName = requiredPair.Key; long current = itemCounts.ContainsKey(itemName) ? itemCounts[itemName] : 0; decimal delta = (decimal)wanted - current;
        if (delta > 0) { if (itemsToProduce.ContainsKey(itemName)) { if ((decimal)wanted - current > itemsToProduce[itemName]) itemsToProduce[itemName] = (decimal)wanted - current; } else itemsToProduce[itemName] = delta; }
    }

    UpdateIngotDeficit(itemsToProduce);

    var assemblerTasks = new Dictionary<long, int>();
    foreach (var asm in autocraftAssemblers) assemblerTasks[asm.EntityId] = 0;

    IMyAssembler designatedDisassembler = null;

    var validItemsToProduce = new Dictionary<string, decimal>();
    if (itemsToProduce.Count > 0)
    {
        foreach (var itemPair in itemsToProduce)
        {
            string itemName = itemPair.Key;
            string blueprintSubtype = GetBlueprintSubtype(itemName);
            MyDefinitionId blueprintId;
            if (MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + blueprintSubtype, out blueprintId))
            {
                bool canProduce = false;
                foreach (var asm in autocraftAssemblers)
                {
                    if (asm.CanUseBlueprint(blueprintId))
                    {
                        canProduce = true;
                        break;
                    }
                }
                if (canProduce)
                {
                    validItemsToProduce[itemName] = itemPair.Value;
                }
                else
                {
                    if(runCounter % 60 == 0)
                        log.AppendLine($"! Autocraft: No assembler can produce {itemName} ({blueprintSubtype}). Use !learn?");
                }
            }
            else
            {
                if(runCounter % 60 == 0)
                    log.AppendLine($"! Autocraft: BP not found/invalid {itemName} ({blueprintSubtype}).");
            }
        }
    }

    if (validItemsToProduce.Count > 0)
    {
        int assemblerIndex = 0;
        foreach(var itemPair in validItemsToProduce)
        {
            if (autocraftAssemblers.Count == 0) break;
            IMyAssembler currentAssembler = autocraftAssemblers[assemblerIndex % autocraftAssemblers.Count];
            assemblerTasks[currentAssembler.EntityId] = 1;
            assemblerIndex++;
        }
    }

    if (itemsToDisassemble.Count > 0)
    {
        designatedDisassembler = autocraftAssemblers.FirstOrDefault(asm => assemblerTasks[asm.EntityId] == 0);
        if (designatedDisassembler != null)
        {
            assemblerTasks[designatedDisassembler.EntityId] = 2;
        }
        else
        {
            if(runCounter % 60 == 0) log.AppendLine("! Autocraft: Surplus detected, but all assemblers are producing.");
        }
    }

    int productionAssemblerIndex = 0;
    var currentDisassemblyQueue = new List<MyProductionItem>();

    foreach (var asm in autocraftAssemblers)
    {
        int task = assemblerTasks[asm.EntityId];

        if (task == 1)
        {
            if (asm.Mode != MyAssemblerMode.Assembly) { asm.ClearQueue(); asm.Mode = MyAssemblerMode.Assembly; }
            else { asm.ClearQueue(); }
            var itemsForThisAssembler = validItemsToProduce
                .Where((pair, index) => index % autocraftAssemblers.Count == productionAssemblerIndex)
                .ToList();
            foreach (var itemPair in itemsForThisAssembler)
            {
                string itemName = itemPair.Key;
                decimal amountNeeded = itemPair.Value;
                string blueprintSubtype = GetBlueprintSubtype(itemName);
                MyDefinitionId blueprintId;
                if (MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + blueprintSubtype, out blueprintId) && asm.CanUseBlueprint(blueprintId))
                {
                    try
                    {
                        decimal amountToAdd = Math.Ceiling(amountNeeded);
                        asm.AddQueueItem(blueprintId, amountToAdd);
                    } catch (Exception e) { log.AppendLine($"! ERR Prod {itemName}"); Echo($"Exc Prod {itemName}: {e.Message}"); }
                }
            }
            productionAssemblerIndex++;
        }
        else if (task == 2)
        {
            if (asm.Mode != MyAssemblerMode.Disassembly)
            {
                asm.ClearQueue();
                asm.Mode = MyAssemblerMode.Disassembly;
            }
            currentDisassemblyQueue.Clear();
            asm.GetQueue(currentDisassemblyQueue);
            if (currentDisassemblyQueue.Count == 0)
            {
                string asmName = asm.CustomName; if (asmName.Length > 15) asmName = asmName.Substring(0, 15);
                log.AppendLine($"- Autocraft: {asmName}... queuing disassembly.");
                foreach(var itemPair in itemsToDisassemble)
                {
                    string itemName = itemPair.Key; decimal amountExcess = itemPair.Value; string blueprintSubtype = GetBlueprintSubtype(itemName); MyDefinitionId blueprintId;
                    if (MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + blueprintSubtype, out blueprintId) && asm.CanUseBlueprint(blueprintId))
                    {
                        long amountToMove = (long)Math.Ceiling(amountExcess);
                        long movedAmount = MoveItemsForDisassembly(itemName, amountToMove, asm.InputInventory, log);
                        if (movedAmount > 0)
                        {
                            try { asm.AddQueueItem(blueprintId, (decimal)movedAmount); log.AppendLine($"- Autocraft: Queue -> disassemble {movedAmount} of {itemName}."); }
                            catch (Exception e) { log.AppendLine($"! ERR Diss {itemName}"); Echo($"Exc Diss {itemName}: {e.Message}"); }
                        }
                    } else { log.AppendLine($"! Autocraft: BP not found to disassemble {itemName} ({blueprintSubtype})"); }
                }
            }
        }
        else
        {
            asm.ClearQueue();
            if (asm.Mode != MyAssemblerMode.Assembly)
            {
                asm.Mode = MyAssemblerMode.Assembly;
            }
        }
    }
}

void UpdateIngotDeficit(Dictionary<string, decimal> iTP) { _iD.Clear(); if (iTP.Count == 0) return; foreach (var iP in iTP) { string iS = iP.Key; double aN = (double)iP.Value; List<string> ingots; if (_cIM.TryGetValue(iS, out ingots)) foreach (var ingot in ingots) if (_iD.ContainsKey(ingot)) _iD[ingot] += aN; else _iD[ingot] = aN; } }

long MoveItemsForDisassembly(string itemName, long amountToMove, IMyInventory destination, StringBuilder log)
{
    long totalMoved = 0;
    var relevantSourceTags = new List<string> { COMPONENTS_CONTAINER_TAG, AMMO_CONTAINER_TAG, BOTTLES_CONTAINER_TAG };
    var itemsInSourceInv = new List<MyInventoryItem>();
    foreach (var sourceTag in relevantSourceTags)
    {
        var sourceInventories = FindInventoriesWithTag(sourceTag);
        if (sourceInventories.Count == 0) continue;
        foreach(var sourceInv in sourceInventories)
        {
            itemsInSourceInv.Clear();
            sourceInv.GetItems(itemsInSourceInv);
            MyInventoryItem? itemToMove = null;
            foreach (var item in itemsInSourceInv)
            {
                if (item.Type.SubtypeId.ToString() == itemName)
                {
                    itemToMove = item;
                    break;
                }
            }
            if (itemToMove.HasValue)
            {
                var actualItem = itemToMove.Value;
                long amountInStack = actualItem.Amount.ToIntSafe();
                long amountToTransfer = Math.Min(amountToMove - totalMoved, amountInStack);
                if (amountToTransfer > 0)
                {
                    if (sourceInv.TransferItemTo(destination, actualItem, (VRage.MyFixedPoint)(decimal)amountToTransfer))
                    {
                        totalMoved += amountToTransfer;
                    }
                }
                if (totalMoved >= amountToMove) break;
            }
        }
        if (totalMoved >= amountToMove) break;
    }
    if (totalMoved == 0 && amountToMove > 0 && runCounter % 60 == 0)
    {
       log.AppendLine($"! Disassembly: Could not find/move {itemName} to assembler.");
    }
    return totalMoved;
}

void ResetQuotas()
{
    Echo("Command 'reset_quotas' received.");
    if (aLcdP != null) { aLcdP.CustomData = ""; IMyTextSurface s = GetTextSurface(aLcdP); if (s != null) s.WriteText(""); Echo("Quota panel and CustomData cleared."); }
    _uDQ.Clear(); autoQuotas.Clear();
}


// =======================================================================================
// MODULE 6: REFINERY MANAGEMENT
// =======================================================================================
void ManageRefineryBalancing(int rIdx, StringBuilder log)
{
    if (allRefs.Count < 2 || rIdx >= allRefs.Count) return; var refinery = allRefs[rIdx]; var sI = refinery.GetInventory(0); var items = new List<MyInventoryItem>(); sI.GetItems(items); if (items.Count == 0) return;
    foreach(var item in items) { if (!item.Type.GetItemInfo().IsOre) continue; long total = 0; foreach (var r in allRefs) total += r.GetInventory(0).GetItemAmount(item.Type).ToIntSafe(); long target = total / allRefs.Count; long current = sI.GetItemAmount(item.Type).ToIntSafe();
    if (current > target) { long aTM = current - target; foreach (var dR in allRefs) { if (dR == refinery) continue; var dI = dR.GetInventory(0); long dA = dI.GetItemAmount(item.Type).ToIntSafe(); if (dA < target) { long space = target - dA; long move = Math.Min(aTM, space); if (sI.TransferItemTo(dI, item, (VRage.MyFixedPoint)(decimal)move)) { log.AppendLine($"- Balancing Ore: {move} {item.Type.SubtypeId} -> {dR.CustomName.Substring(0, 5)}..."); aTM -= move; if (aTM <= 0) break; } } } } }
}
void SortRefineryQueues(StringBuilder log)
{
    foreach (var refinery in allRefs) { var inv = refinery.GetInventory(0); var items = new List<MyInventoryItem>(); inv.GetItems(items); if (items.Count < 2) continue; MyInventoryItem topPItem = items[0]; int topPRank = GetRefiningRank(items[0].Type.SubtypeId.ToString());
    for (int i = 1; i < items.Count; i++) { int cRank = GetRefiningRank(items[i].Type.SubtypeId.ToString()); if (cRank < topPRank) { topPRank = cRank; topPItem = items[i]; } }
    if (topPItem.ItemId != items[0].ItemId) { inv.TransferItemTo(inv, topPItem, 0); string asmN = refinery.CustomName; if (asmN.Length > 15) asmN = asmN.Substring(0, 15); log.AppendLine($"- Refinery '{asmN}...' reordered. Prio: {topPItem.Type.SubtypeId}"); } }
}
int GetRefiningRank(string oSId)
{
    string iSId = oSId; if (oSId == "Stone") iSId = "Stone";
    var dList = _iD.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToList(); int dRank = dList.IndexOf(iSId); if (dRank != -1) return -200 + dRank;
    int fRank; if (_rPM.TryGetValue(oSId, out fRank)) return fRank;
    return 999;
}

// =======================================================================================
// MODULE 7: ADVANCED LCD DISPLAY
// =======================================================================================
void UpdateLcdStatus() { lcdStatDisp.Clear(); lcdStatDisp.AppendLine(GetPowerStatus()); lcdStatDisp.AppendLine(GetProductionStatus()); lcdStatDisp.AppendLine(GetInventoryStatus()); lcdStatDisp.AppendLine(GetDockedShipsStatus()); }
string GetPowerStatus() { var pP = new List<IMyPowerProducer>(); GridTerminalSystem.GetBlocksOfType(pP, p => IsOnManagedGrid(p)); float cO = 0f; float mO = 0f; foreach(var p in pP) { cO += p.CurrentOutput; mO += p.MaxOutput; } return $"== POWER ==\nUse: {cO:F2}/{mO:F2} MW\n{GetProgressBar(cO, mO)}\n"; }
string GetProductionStatus()
{
    int activeRefs = allRefs.Count(r => r.IsProducing);
    int activeAsms = autoAsms.Count(a => a.IsProducing);
    int disassemblingCnt = autoAsms.Count(a => a.Mode == MyAssemblerMode.Disassembly && a.IsProducing);
    var sb = new StringBuilder(); sb.AppendLine("== PRODUCTION =="); sb.AppendLine($" Refineries: {activeRefs}/{allRefs.Count} Active"); sb.AppendLine($" Assemblers: {activeAsms}/{autoAsms.Count} Active ({disassemblingCnt} disassembling)\n"); return sb.ToString();
}
string GetInventoryStatus() { var sb = new StringBuilder("== STORAGE ==\n"); Action<string, string> gFL = (t, n) => { var conts = new List<IMyCargoContainer>(); GridTerminalSystem.GetBlocksOfType(conts, c => c.CustomName.Contains(t) && IsOnManagedGrid(c)); if (conts.Count > 0) { long cV = 0; long mV = 0; foreach(var c in conts) { var inv = c.GetInventory(); cV += inv.CurrentVolume.RawValue; mV += inv.MaxVolume.RawValue; } float fR = mV > 0 ? (float)cV / mV : 0; sb.AppendLine($" {n.PadRight(12)}: {fR:P1}\n {GetProgressBar(cV, mV)}"); } else sb.AppendLine($" {n.PadRight(12)}: No containers"); }; gFL(ORES_CONTAINER_TAG, "Ores"); gFL(INGOTS_CONTAINER_TAG, "Ingots"); gFL(COMPONENTS_CONTAINER_TAG, "Components"); return sb.ToString(); }
string GetDockedShipsStatus() { var conns = new List<IMyShipConnector>(); GridTerminalSystem.GetBlocksOfType(conns, c => IsOnManagedGrid(c) && c.Status == MyShipConnectorStatus.Connected && c.OtherConnector != null && !c.OtherConnector.IsSameConstructAs(Me)); if (conns.Count == 0) return "== DOCKED SHIPS ==\n None\n"; var sb = new StringBuilder("== DOCKED SHIPS ==\n"); foreach(var c in conns) sb.AppendLine($"- {c.OtherConnector.CubeGrid.CustomName}"); return sb.ToString(); }
void UpdatePowerLcd() { IMyTextSurface s = GetTextSurface(pLcdP); if (s == null) return; s.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; s.Font = "Monospace"; s.FontSize = 0.5f; var sb = new StringBuilder("== DETAILED POWER ==\n"); var allP = new List<IMyPowerProducer>(); GridTerminalSystem.GetBlocksOfType(allP, p => IsOnManagedGrid(p)); var batts = new List<IMyBatteryBlock>(); GridTerminalSystem.GetBlocksOfType(batts, b => IsOnManagedGrid(b)); float tP = 0f; foreach(var p in allP) tP += p.CurrentOutput; float bD = 0f; foreach(var b in batts) bD += (b.CurrentInput - b.CurrentOutput); float tC = tP - bD; sb.AppendLine("\n-- BALANCE --"); sb.AppendLine($"Production: {tP:F2} MW"); sb.AppendLine($"Consumption:  {tC:F2} MW"); string bS = Math.Abs(bD) < 0.001f ? "Idle" : bD > 0 ? $"Charging ({bD:F2} MW)" : $"Discharging ({-bD:F2} MW)"; sb.AppendLine($"Batteries: {bS}"); sb.AppendLine("\n-- GENERATION --"); float sO=0f, mS=0f; var sP = new List<IMySolarPanel>(); GridTerminalSystem.GetBlocksOfType(sP, p => IsOnManagedGrid(p)); foreach(var p in sP) { sO += p.CurrentOutput; mS += p.MaxOutput; } sb.AppendLine($"Solar: {sO:F2} MW {GetProgressBar(sO, mS)}"); float rO=0f, mR=0f; var reactors = new List<IMyReactor>(); GridTerminalSystem.GetBlocksOfType(reactors, r => IsOnManagedGrid(r)); foreach(var r in reactors) { rO += r.CurrentOutput; mR += r.MaxOutput; } sb.AppendLine($"Reactors: {rO:F2} MW {GetProgressBar(rO, mR)}"); float hO=0f, mH=0f; var hE = new List<IMyPowerProducer>(); GridTerminalSystem.GetBlocksOfType(hE, h => IsOnManagedGrid(h) && h.BlockDefinition.SubtypeId.Contains("HydrogenEngine")); foreach(var h in hE) { hO += h.CurrentOutput; mH += h.MaxOutput;} sb.AppendLine($"Hydrogen: {hO:F2} MW {GetProgressBar(hO, mH)}"); float wO=0f, mW=0f; var wT = new List<IMyWindTurbine>(); GridTerminalSystem.GetBlocksOfType(wT, w => IsOnManagedGrid(w)); foreach(var w in wT) { wO += w.CurrentOutput; mW += w.MaxOutput; } sb.AppendLine($"Wind: {wO:F2} MW {GetProgressBar(wO, mW)}"); sb.AppendLine("\n-- STORAGE --"); if (batts.Count == 0) sb.AppendLine("No batteries."); else { float tCS = 0; float tMS = 0; foreach(var b in batts) { tCS += b.CurrentStoredPower; tMS += b.MaxStoredPower; string cM = b.ChargeMode == ChargeMode.Auto ? "" : $"[{b.ChargeMode.ToString().Substring(0,1)}]"; sb.AppendLine($"- Bat {batts.IndexOf(b) + 1:D2}: {b.CurrentStoredPower:F2}/{b.MaxStoredPower:F2} MWh {cM}"); } sb.AppendLine($"Total: {tCS:F2}/{tMS:F2} MWh"); sb.AppendLine(GetProgressBar(tCS, tMS)); } s.WriteText(sb.ToString()); }

void UpdateIngotLcd()
{
    if (ingotLcdPanel == null) return;
    IMyTextSurface surface = GetTextSurface(ingotLcdPanel);
    if (surface == null) return;

    _ingotQuotas.Clear();
    var matches = quotaCustomDataRegex.Matches(ingotLcdPanel.CustomData);
    foreach (System.Text.RegularExpressions.Match match in matches)
    {
        if (match.Groups.Count == 3)
        {
            string name = match.Groups[1].Value;
            long quota = 0;
            if (long.TryParse(match.Groups[2].Value, out quota))
            {
                if (!string.IsNullOrWhiteSpace(name))
                    _ingotQuotas[name] = quota;
            }
        }
    }

    StringBuilder sb = new StringBuilder();
    sb.AppendLine("        << Ingots Summary >>");
    sb.AppendLine();

    var allIngotNames = new HashSet<string>(_ingotQuotas.Keys);
    foreach(var ingotName in _ingotCounts.Keys) { allIngotNames.Add(ingotName); }
    if (!_ingotCounts.ContainsKey("Gravel") && !_ingotQuotas.ContainsKey("Gravel")) { allIngotNames.Add("Gravel"); }
    var sortedNames = new List<string>(allIngotNames);
    sortedNames.Sort();

    foreach (string name in sortedNames)
    {
        long currentStock = 0; _ingotCounts.TryGetValue(name, out currentStock);
        long currentQuota = 0; _ingotQuotas.TryGetValue(name, out currentQuota);
        long currentOre = 0; _oreCounts.TryGetValue(name, out currentOre);

        if(currentStock == 0 && currentQuota == 0 && !fixedRefiningList.Contains(name) && name != "Gravel") continue;

        string stockStr = FormatLargeNumber(currentStock);
        string quotaStr = FormatLargeNumber(currentQuota > 0 ? currentQuota : 1);
        if(currentQuota == 0) quotaStr = "0";
        string stockQuotaStr = $"{stockStr} / {quotaStr}";
        string oreStr = (currentOre > 0) ? $"+{FormatLargeNumber(currentOre)} ore" : "+0 ore";

        sb.Append(name.PadRight(10));
        sb.Append(GetProgressBar(currentStock, currentQuota, 18));
        sb.Append(" ");
        sb.Append(stockQuotaStr.PadRight(13));
        sb.Append(oreStr.PadLeft(9));
        sb.AppendLine();
    }

    surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    surface.Font = "Monospace";
    surface.FontSize = 0.7f;
    surface.WriteText(sb.ToString());
}

string GetProgressBar(float c, float m, int l = 20) { if (m <= 0) m = c > 0 ? c : 1; int b = (int)Math.Round((c / m) * l); b = Math.Max(0, Math.Min(l, b)); return "[" + new String('|', b) + new String('-', l - b) + "]"; }
string GetProgressBar(long c, long m, int l = 20) { return GetProgressBar((float)c, (float)m, l); }
void UpdateLcdDisplay() { IMyTextSurface s = GetTextSurface(lcdP); if (s == null) { Echo("ERROR: [NanoLCD] not found."); return; } s.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; s.Font = "Monospace"; s.FontSize = 0.5f; var fO = new StringBuilder($"--- Manager v28 (Tagged Subgrids) ---\n"); fO.AppendLine("=============================="); fO.Append(lcdStatDisp); fO.AppendLine("=============================="); if (lcdLogOutput.Length > 0) { fO.AppendLine("RECENT LOG:"); fO.Append(lcdLogOutput); } s.WriteText(fO.ToString()); Echo(lcdLogOutput.ToString()); }
// END OF SCRIPT

// 'Alias' Properties
private List<IMyAssembler> autoAsms { get { return autocraftAssemblers; } }
private List<IMyRefinery> allRefs { get { return allRefineries; } }
private IMyTextPanel lcdP { get { return lcdPanel; } }
private IMyTextPanel aLcdP { get { return autocraftLcdPanel; } }
private IMyTextPanel pLcdP { get { return powerLcdPanel; } }
private IMyTextPanel iLcdP { get { return ingotLcdPanel; } }
private Dictionary<string, int> reqComps { get { return requiredComponents; } }
private Dictionary<string, int> autoQuotas { get { return autocraftQuotas; } }
private Dictionary<string, int> _uDQ { get { return _userDefinedQuotas; } }
private HashSet<string> _cI { get { return _craftableItems; } }
private System.Text.RegularExpressions.Regex compRegex { get { return componentRegex; } }
private StringBuilder lcdLogOut { get { return lcdLogOutput; } }
private StringBuilder lcdStatDisp { get { return lcdStatusDisplay; } }
private Dictionary<string, string> _lBM { get { return _learnedBlueprintMap; } }
private IMyAssembler _lA { get { return _learningAssembler; } set { _learningAssembler = value; } }
private Dictionary<string, int> _rPM { get { return _refiningPriorityMap; } }
private Dictionary<string, double> _iD { get { return _ingotDeficit; } }
private Dictionary<string, List<string>> _cIM { get { return _componentIngotMap; } }
private const double ASM_MARGIN = ASSEMBLE_MARGIN_PERCENT;
private const double DSM_MARGIN = DISASSEMBLE_MARGIN_PERCENT;