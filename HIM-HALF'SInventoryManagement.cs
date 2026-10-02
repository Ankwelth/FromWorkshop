// =======================================================================================
// HALF'S Inventory Management - HIM
// Version: 1.1.1 PERFORMANCE SAFE
//
// TESTER WARNING:
// Back up worlds before using this on live servers.
//
// FEATURES:
// - Storage setup, sorting, dynamic storage expansion.
// - Resource, Storage, Ship, Unknown, AutoCraft, and Status LCDs.
// - AutoCraft with master assembler queue and cooperative helpers.
// - Duplicate queue protection using stock + already queued production.
// - Dedicated disassembler using [AUTOCRAFT] [DISASSEMBLER].
// - Fighter and miner ship servicing.
// - Refinery output cleanup.
// - Build & Repair and Self Maintenance support.
// - Fresh programmable block detection.
//
// LCD TAGS:
// [ResourceLCD], [StorageLCD], [ShipLCD], [UnknownLCD],
// [AutoCraftLCD], [AutoCraftLCD2], [AutoCraftLCD3], [AutoCraftLCD4],
// [AutoCraftLCD5], [STATUSLCD]
//
// STORAGE TAGS:
// HIM Storage [ORES]
// HIM Storage [ICE]
// HIM Storage [INGOTS] [URANIUM]
// HIM Storage [COMPONENTS]
// HIM Storage [TOOLS]
// HIM Storage [FOOD]
// HIM Storage [FARM]
// HIM Storage [AMMO]
// HIM Storage [BOTTLES]
// Optional: [PERSONAL], [LOCK], [IGNORE]
//
// PRODUCTION TAGS:
// Assembler 1 [AUTOCRAFT]
// Assembler 2 [AUTOCRAFT]
// Assembler Disassembler [AUTOCRAFT] [DISASSEMBLER]
//
// MAINTENANCE TAGS:
// Nano Build & Repair [BUILDREPAIR]
// Self Maintenance Unit [MAINTENANCE]
//
// SHIP TAGS:
// [FIGHTER], [MINER], [NOUNLOAD], [NOREFUEL]
//
// COMMANDS:
// refresh             Rediscover LCDs.
// setup               Assign missing storage categories.
// setup_force         Rebuild unlocked/non-personal storage names.
// storage_refresh     Check storage expansion and claim needed blank cargo.
// sort_now            Force cleanup sort.
// reset_all           setup_force + storage_refresh + deep sweep + refinery flush +
//                     autocraft_refresh + LCD refresh.
// reset_setup         Clear setup memory.
// autocraft_refresh   Rebuild AutoCraft Custom Data from detected inventory.
// autocraft_debug     Show AutoCraft assembler and blueprint cache info.
// refinery_flush      Empty refinery outputs.
//
// AUTOCraft CUSTOM DATA:
// SteelPlate=25000
// Motor=1500
// Computer=D500
// RadioComponent=0
//
// D allows disassembly above quota.
// Quota 0 means list/display only.
// =======================================================================================

// -------------------------------------
// CONFIG
// -------------------------------------

const string SCRIPT_NAME = "HALF'S Inventory Management";
const string SHORT_NAME = "HIM";
const string VERSION = "1.1.1 PERFORMANCE SAFE";

// Setup behavior
const double EXPAND_AT_FILL_PERCENT = 0.80;
const bool FIRST_RUN_AUTO_SETUP_IF_NO_TAGS = true;

// Performance scheduling.
// Update100 runs about every 1.6 seconds.
const int PERF_COUNT_INTERVAL = 10;        // about 16 seconds
const int PERF_SORT_INTERVAL = 40;         // about 64 seconds
const int PERF_REFINERY_INTERVAL = 10;     // about 16 seconds
const int PERF_SHIP_INTERVAL = 35;         // about 56 seconds
const int PERF_AUTOCRAFT_INTERVAL = 10;    // about 16 seconds
const int PERF_LCD_INTERVAL = 5;           // about 8 seconds
const int PERF_DEEP_SWEEP_INTERVAL = 188;   // kept for compatibility
const int PERF_DEEP_SWEEP_STAGE_INTERVAL = 38; // about 1 minute per cleanup stage
const int PERF_DEEP_SWEEP_MOVE_CAP = 35;       // max cleanup moves per staged pass
const int PERF_REFINERY_MOVE_CAP = 35;         // max refinery-output moves per pass
const int PERF_MAX_SORT_MOVES = 80;        // max inventory moves per sort cycle
const int PERF_MAX_SHIPS_PER_CYCLE = 1;    // service one docked ship per cycle

// Manual cleanup command limits
const int FORCE_SORT_MAX_PASSES = 12;
const int DEEP_SWEEP_MAX_PASSES = 4;

// Base reactor targets
const int BASE_LARGE_REACTOR_URANIUM = 1250;
const int BASE_SMALL_REACTOR_URANIUM = 250;

// Docked ship reactor targets
const int SHIP_LARGE_REACTOR_URANIUM = 100;
const int SHIP_SMALL_REACTOR_URANIUM = 25;

// Docked ship supplies
const int SHIP_H2GEN_ICE_BATCH = 5000;
const int SHIP_H2GEN_MAX_BATCH_LOOPS = 25;
const int BASE_ICE_FILL_BATCH = 5000;
const int BASE_ICE_FILL_MAX_LOOPS = 20;
const int SHIP_HYDROGEN_BOTTLE_MIN = 1;
const int SHIP_OXYGEN_BOTTLE_MIN = 1;

// Ammo defaults
const int SHIP_GATLING_MIN = 6;
const int SHIP_GATLING_ADD = 4;
const int SHIP_MISSILE_MIN = 10;
const int SHIP_MISSILE_ADD = 5;
const int SHIP_INTERIOR_MIN = 2;
const int SHIP_INTERIOR_ADD = 2;

// Fighter reserve ammo behavior
const double FIGHTER_AMMO_CARGO_TARGET_FILL = 0.90;
const int FIGHTER_WEAPON_TOPUP_BATCH = 5;
const int FIGHTER_AMMO_CARGO_PASS_LIMIT = 20;

// Tags
const string ORES_TAG = "[ORES]";
const string ICE_TAG = "[ICE]";
const string INGOTS_TAG = "[INGOTS]";
const string URANIUM_TAG = "[URANIUM]";
const string COMPONENTS_TAG = "[COMPONENTS]";
const string TOOLS_TAG = "[TOOLS]";
const string FOOD_TAG = "[FOOD]";
const string FARM_TAG = "[FARM]";
const string AMMO_TAG = "[AMMO]";
const string BOTTLES_TAG = "[BOTTLES]";
const string PERSONAL_TAG = "[PERSONAL]";
const string IGNORE_TAG = "[IGNORE]";
const string LOCK_TAG = "[LOCK]";

// Ship tags
const string FIGHTER_TAG = "[FIGHTER]";
const string MINER_TAG = "[MINER]";
const string NOUNLOAD_TAG = "[NOUNLOAD]";
const string NOREFUEL_TAG = "[NOREFUEL]";

// LCD tags
const string NANO_LCD_TAG = "[NanoLCD]";
const string RESOURCE_LCD_TAG = "[ResourceLCD]";
const string STORAGE_LCD_TAG = "[StorageLCD]";
const string SHIP_LCD_TAG = "[ShipLCD]";
const string UNKNOWN_LCD_TAG = "[UnknownLCD]";
const string AUTOCRAFT_LCD_TAG = "[AutoCraftLCD]";
const string STATUS_LCD_TAG = "[STATUSLCD]";
const string BUILDREPAIR_TAG = "[BUILDREPAIR]";
const string MAINTENANCE_TAG = "[MAINTENANCE]";
const string SELFMAINT_TAG = "[SELFMAINT]";
const int MAX_BUILDREPAIR_BLOCKS = 4;

// AutoCraft tags
const string AUTOCRAFT_ASSEMBLER_TAG = "[AUTOCRAFT]";
const string DISASSEMBLER_TAG = "[DISASSEMBLER]";

// AutoCraft settings
const double AUTOCRAFT_START_PERCENT = 0.95;
const int AUTOCRAFT_MAX_QUEUE_CHUNK = 0;
const int AUTOCRAFT_LINES_PER_LCD = 27;

// -------------------------------------
// GLOBALS
// -------------------------------------

int runCounter = 0;
bool setupDone = false;
int deepSweepStage = 0;
string lastSweepStage = "none";

StringBuilder log = new StringBuilder();
StringBuilder shipLog = new StringBuilder();
Dictionary<string, string> shipServiceSummary = new Dictionary<string, string>();

Dictionary<string, long> itemTotals = new Dictionary<string, long>();
Dictionary<string, long> ingotTotals = new Dictionary<string, long>();
Dictionary<string, long> oreTotals = new Dictionary<string, long>();
Dictionary<string, string> unknownItems = new Dictionary<string, string>();

Dictionary<string, AutoQuota> autoCraftQuotas = new Dictionary<string, AutoQuota>();
Dictionary<string, string> autoCraftStatus = new Dictionary<string, string>();
Dictionary<string, decimal> autoCraftJobs = new Dictionary<string, decimal>();
StringBuilder autoCraftLog = new StringBuilder();
Dictionary<string, long> buildRepairDemand = new Dictionary<string, long>();
StringBuilder buildRepairLog = new StringBuilder();
IMyTextPanel statusLcd;

// Blueprint ID cache: item name -> resolved MyDefinitionId.
// Resolving a blueprint by string is a real (if small) cost, and this dictionary
// used to be rebuilt by re-parsing the same strings every AutoCraft cycle.
// We resolve each item name once and reuse the result from then on.
// Items that fail to resolve (no matching blueprint, e.g. unsupported modded item)
// are cached as a known miss too, so we don't retry TryParse for them every cycle.
Dictionary<string, MyDefinitionId> blueprintIdCache = new Dictionary<string, MyDefinitionId>();
HashSet<string> blueprintIdMisses = new HashSet<string>();

class AutoQuota
{
    public string Item;
    public int Quota;
    public bool AllowDisassemble;
}

IMyTextPanel nanoLcd;
IMyTextPanel resourceLcd;
IMyTextPanel storageLcd;
IMyTextPanel shipLcd;
IMyTextPanel unknownLcd;
IMyTextPanel autocraftLcd;

MyDefinitionId uraniumId;
MyDefinitionId iceId;
MyDefinitionId hydrogenBottleId;
MyDefinitionId oxygenBottleId;
MyDefinitionId gatlingAmmoId;
MyDefinitionId missileAmmoId;
MyDefinitionId rifleAmmoId;

List<string> categoryTags = new List<string>
{
    ORES_TAG, ICE_TAG, INGOTS_TAG, COMPONENTS_TAG, TOOLS_TAG,
    FOOD_TAG, FARM_TAG, AMMO_TAG, BOTTLES_TAG
};

Dictionary<string, string> categoryDisplayNames = new Dictionary<string, string>
{
    { ORES_TAG, "Ores" },
    { ICE_TAG, "Ice" },
    { INGOTS_TAG, "Ingots" },
    { COMPONENTS_TAG, "Components" },
    { TOOLS_TAG, "Tools" },
    { FOOD_TAG, "Food" },
    { FARM_TAG, "Farm" },
    { AMMO_TAG, "Ammo" },
    { BOTTLES_TAG, "Bottles" }
};

// =======================================================================================
// PROGRAM
// =======================================================================================

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;

    MyDefinitionId.TryParse("MyObjectBuilder_Ingot/Uranium", out uraniumId);
    MyDefinitionId.TryParse("MyObjectBuilder_Ore/Ice", out iceId);
    MyDefinitionId.TryParse("MyObjectBuilder_GasContainerObject/HydrogenBottle", out hydrogenBottleId);
    MyDefinitionId.TryParse("MyObjectBuilder_OxygenContainerObject/OxygenBottle", out oxygenBottleId);
    MyDefinitionId.TryParse("MyObjectBuilder_AmmoMagazine/NATO_25x184mm", out gatlingAmmoId);
    MyDefinitionId.TryParse("MyObjectBuilder_AmmoMagazine/Missile200mm", out missileAmmoId);
    MyDefinitionId.TryParse("MyObjectBuilder_AmmoMagazine/NATO_5p56x45mm", out rifleAmmoId);

    LoadState();

    DiscoverLcds();

    if (FIRST_RUN_AUTO_SETUP_IF_NO_TAGS && !HasAnyTaggedStorage())
    {
        AutoSetupStorage(false);
        setupDone = true;
        SaveState();
    }

    Echo(SCRIPT_NAME + " " + VERSION + " started.");
}

public void Main(string argument, UpdateType updateSource)
{
    log.Clear();
    shipLog.Clear();
    shipServiceSummary.Clear();

    string arg = (argument ?? "").Trim().ToLower();

    if (arg == "refresh")
    {
        DiscoverLcds();
        log.AppendLine("LCDs refreshed.");
    }
    else if (arg == "setup")
    {
        AutoSetupStorage(false);
        setupDone = true;
        SaveState();
        log.AppendLine("Setup complete.");
    }
    else if (arg == "setup_force")
    {
        AutoSetupStorage(true);
        setupDone = true;
        SaveState();
        log.AppendLine("Force setup complete.");
    }
    else if (arg == "reset_setup")
    {
        setupDone = false;
        SaveState();
        log.AppendLine("Setup memory reset.");
    }
    else if (arg == "autocraft_refresh")
    {
        RefreshAutoCraftCustomDataFromInventory();
        blueprintIdCache.Clear();
        blueprintIdMisses.Clear();
        log.AppendLine("AutoCraft list refreshed.");
    }
    else if (arg == "refinery_flush")
    {
        EmptyRefineryOutputs();
        log.AppendLine("Refinery flush command ran.");
    }
    else if (arg == "autocraft_debug")
    {
        var a = GetAutoCraftAssemblers(true);
        log.AppendLine("AutoCraft assemblers found: " + a.Count);
        foreach (var asm in a)
            log.AppendLine("- " + asm.CustomName + " / " + asm.BlockDefinition.SubtypeId);
        log.AppendLine("Blueprint cache: " + blueprintIdCache.Count + " resolved, " + blueprintIdMisses.Count + " miss");
    }
    else if (arg == "reset_all")
    {
        AutoSetupStorage(true);
        StorageRefresh();
        CountAllItems();

        RunDeepSweepStage(true);
        RunDeepSweepStage(true);
        RunDeepSweepStage(true);
        RunDeepSweepStage(true);
        EmptyRefineryOutputs();

        RefreshAutoCraftCustomDataFromInventory();
        blueprintIdCache.Clear();
        blueprintIdMisses.Clear();
        CountAllItems();
        UpdateLcds();
        setupDone = true;
        SaveState();
        log.AppendLine("Full reset complete.");
    }


    else if (arg == "storage_refresh")
    {
        StorageRefresh();
        log.AppendLine("Storage refresh complete.");
    }
    else if (arg == "sort_now")
    {
        CountAllItems();
        RunDeepSweepStage(true);
        EmptyRefineryOutputs();
        CountAllItems();
        UpdateLcds();
        log.AppendLine("Manual cleanup stage complete.");
    }


    if (runCounter % PERF_COUNT_INTERVAL == 0)
        CountAllItems();

    if (runCounter % PERF_REFINERY_INTERVAL == 0)
    {
        EmptyRefineryOutputs();
    }

    if (runCounter % PERF_SORT_INTERVAL == 0)
    {
        DynamicExpansionCheck();
        SortBaseInventory();
        ManageBaseReactors();
        ManageBaseIceConsumers();
    }

    if (runCounter % PERF_SHIP_INTERVAL == 0)
    {
        ManageDockedShips();
    }

    if (runCounter % PERF_AUTOCRAFT_INTERVAL == 0)
    {
        ManageBuildRepair();
        ManageAutoCraft();
    }

    if (runCounter > 0 && runCounter % PERF_DEEP_SWEEP_STAGE_INTERVAL == 0)
    {
        RunDeepSweepStage(false);
    }

    if (runCounter % PERF_LCD_INTERVAL == 0)
        UpdateLcds();

    Echo(log.ToString());
    runCounter++;
}

void SaveState()
{
    Storage =
        "HIM_PB_ID=" + Me.EntityId + "\n" +
        "HIM_VERSION=" + VERSION + "\n" +
        "SETUP_DONE=" + (setupDone ? "true" : "false");
}

void LoadState()
{
    string storage = Storage ?? "";
    string currentId = "HIM_PB_ID=" + Me.EntityId;

    if (!storage.Contains(currentId))
    {
        setupDone = false;
        blueprintIdCache.Clear();
        blueprintIdMisses.Clear();
        Storage = "";
        return;
    }

    setupDone = storage.Contains("SETUP_DONE=true");
}


// =======================================================================================
// BASIC HELPERS
// =======================================================================================

bool IsSameMainGrid(IMyTerminalBlock block)
{
    return block != null && block.CubeGrid == Me.CubeGrid;
}

bool IsIgnored(IMyTerminalBlock block)
{
    if (block == null) return true;
    return block.CustomName.Contains(IGNORE_TAG) || block.CustomName.Contains(PERSONAL_TAG);
}

bool IsLocked(IMyTerminalBlock block)
{
    return block != null && block.CustomName.Contains(LOCK_TAG);
}

bool IsManagedInventoryBlock(IMyTerminalBlock block)
{
    if (block == null || !block.HasInventory || !IsSameMainGrid(block) || IsIgnored(block))
        return false;

    return true;
}


void DiscoverLcds()
{
    var lcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(lcds, p => IsSameMainGrid(p));

    nanoLcd = lcds.FirstOrDefault(p => p.CustomName.Contains(NANO_LCD_TAG));
    resourceLcd = lcds.FirstOrDefault(p => p.CustomName.Contains(RESOURCE_LCD_TAG));
    storageLcd = lcds.FirstOrDefault(p => p.CustomName.Contains(STORAGE_LCD_TAG));
    shipLcd = lcds.FirstOrDefault(p => p.CustomName.Contains(SHIP_LCD_TAG));
    unknownLcd = lcds.FirstOrDefault(p => p.CustomName.Contains(UNKNOWN_LCD_TAG));
    autocraftLcd = lcds.FirstOrDefault(p => p.CustomName.Contains(AUTOCRAFT_LCD_TAG));
    statusLcd = lcds.FirstOrDefault(p => p.CustomName.Contains(STATUS_LCD_TAG));
}

string FormatNum(double n)
{
    if (n >= 1000000000) return (n / 1000000000).ToString("0.0") + "G";
    if (n >= 1000000) return (n / 1000000).ToString("0.0") + "M";
    if (n >= 1000) return (n / 1000).ToString("0.#") + "k";
    return n.ToString("0");
}

double FillRatio(IMyInventory inv)
{
    if (inv == null || inv.MaxVolume.RawValue == 0) return 1;
    return (double)inv.CurrentVolume.RawValue / (double)inv.MaxVolume.RawValue;
}

string Bar(double pct, int len = 18)
{
    pct = Math.Max(0, Math.Min(1, pct));
    int filled = (int)Math.Round(pct * len);
    return "[" + new string('|', filled) + new string('-', len - filled) + "]";
}

long GetTotal(string subtype)
{
    long v = 0;
    itemTotals.TryGetValue(subtype, out v);
    return v;
}

bool HasCategoryTag(string name)
{
    foreach (var t in categoryTags)
        if (name.Contains(t)) return true;

    if (name.Contains(URANIUM_TAG)) return true;
    return false;
}

bool HasAnyTaggedStorage()
{
    var containers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(containers, c => IsSameMainGrid(c) && !c.CustomName.Contains(IGNORE_TAG) && !c.CustomName.Contains(PERSONAL_TAG));
    return containers.Any(c => HasCategoryTag(c.CustomName));
}

List<IMyCargoContainer> GetAllUsableCargo()
{
    var containers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(containers, c => IsSameMainGrid(c) && !c.CustomName.Contains(IGNORE_TAG) && !c.CustomName.Contains(PERSONAL_TAG));
    return containers;
}

List<IMyInventory> FindStorage(string tag)
{
    var containers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(containers, c => IsSameMainGrid(c) && !IsIgnored(c) && c.CustomName.Contains(tag));

    // Fill fullest first, but skip completely full ones during transfer.
    return containers.Select(c => c.GetInventory())
                     .OrderByDescending(i => FillRatio(i))
                     .ToList();
}

List<IMyCargoContainer> FindStorageContainers(string tag)
{
    var containers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(containers, c => IsSameMainGrid(c) && !IsIgnored(c) && c.CustomName.Contains(tag));
    return containers.OrderByDescending(c => FillRatio(c.GetInventory())).ToList();
}

bool TransferItemToStorage(IMyInventory source, List<IMyInventory> destinations, MyInventoryItem item)
{
    if (source == null || destinations == null || destinations.Count == 0) return false;

    bool movedAny = false;
    int safety = 0;

    while (safety < 25)
    {
        safety++;

        MyInventoryItem? currentStack = source.FindItem(item.Type);
        if (!currentStack.HasValue) break;

        int amountLeft = currentStack.Value.Amount.ToIntSafe();
        if (amountLeft <= 0) break;

        bool movedThisPass = false;

        foreach (var dest in destinations)
        {
            if (dest == null || dest.IsFull) continue;
            if (dest.Owner.EntityId == source.Owner.EntityId) continue;

            int moved = TryMovePartialStack(source, dest, currentStack.Value, amountLeft);
            if (moved > 0)
            {
                movedAny = true;
                movedThisPass = true;
                break;
            }
        }

        if (!movedThisPass) break;
    }

    return movedAny;
}

int TryMovePartialStack(IMyInventory source, IMyInventory dest, MyInventoryItem item, int wanted)
{
    if (source == null || dest == null || wanted <= 0) return 0;

    int tryAmount = wanted;

    while (tryAmount > 0)
    {
        var fixedAmount = (VRage.MyFixedPoint)tryAmount;

        if (dest.CanItemsBeAdded(fixedAmount, item.Type))
        {
            if (source.TransferItemTo(dest, item, fixedAmount))
                return tryAmount;
        }

        tryAmount = tryAmount / 2;
    }

    return 0;
}

bool PullAmount(MyDefinitionId id, List<IMyInventory> sources, IMyInventory dest, int amount)
{
    if (dest == null || sources == null || amount <= 0) return false;

    int remaining = amount;

    foreach (var source in sources)
    {
        if (source == null) continue;

        var item = source.FindItem(id);
        if (!item.HasValue) continue;

        int available = item.Value.Amount.ToIntSafe();
        int move = Math.Min(available, remaining);
        if (move <= 0) continue;

        var fixedMove = (VRage.MyFixedPoint)move;
        if (!dest.CanItemsBeAdded(fixedMove, item.Value.Type)) continue;

        if (source.TransferItemTo(dest, item.Value, fixedMove))
        {
            remaining -= move;
            if (remaining <= 0) return true;
        }
    }

    return remaining < amount;
}

bool LooksLikeLargeReactor(IMyReactor reactor)
{
    if (reactor == null) return false;

    string subtype = reactor.BlockDefinition.SubtypeId.ToString().ToLower();
    string name = reactor.CustomName.ToLower();

    // Vanilla block subtype names usually identify large/small clearly.
    // Large grid small reactors often contain "small", so check small FIRST.
    if (subtype.Contains("small") || name.Contains("small reactor"))
        return false;

    if (subtype.Contains("large") || name.Contains("large reactor"))
        return true;

    // Fallback: compare MaxOutput. Large reactors have much higher max output.
    // This is safer than inventory volume, because small-grid large reactors can still
    // have large inventory capacity depending on settings/mods.
    if (reactor.MaxOutput >= 100f)
        return true;

    return false;
}


// =======================================================================================
// SETUP / DYNAMIC EXPANSION
// =======================================================================================

void AutoSetupStorage(bool force)
{
    var containers = GetAllUsableCargo();

    if (force)
    {
        foreach (var c in containers)
        {
            if (IsLocked(c)) continue;
            string clean = c.CustomName;
            foreach (var t in categoryTags) clean = clean.Replace(t, "");
            clean = clean.Replace(URANIUM_TAG, "");
            clean = clean.Replace("HIM Storage", "");
            clean = System.Text.RegularExpressions.Regex.Replace(clean, @"\s+", " ").Trim();
            if (string.IsNullOrWhiteSpace(clean)) clean = "Cargo Container";
            c.CustomName = clean;
        }
    }

    AssignCategoryIfMissing(COMPONENTS_TAG);
    AssignCategoryIfMissing(INGOTS_TAG, URANIUM_TAG);
    AssignCategoryIfMissing(ORES_TAG);
    AssignCategoryIfMissing(ICE_TAG);
    AssignCategoryIfMissing(TOOLS_TAG);
    AssignCategoryIfMissing(FOOD_TAG);
    AssignCategoryIfMissing(FARM_TAG);
    AssignCategoryIfMissing(AMMO_TAG);
    AssignCategoryIfMissing(BOTTLES_TAG);

    log.AppendLine("- Storage setup checked.");
}

void AssignCategoryIfMissing(string mainTag, string extraTag = "")
{
    if (FindStorageContainers(mainTag).Count > 0) return;

    var blank = FindBestBlankCargo();
    if (blank == null)
    {
        log.AppendLine("! No blank cargo for " + mainTag);
        return;
    }

    RenameStorage(blank, mainTag, extraTag);
}

IMyCargoContainer FindBestBlankCargo()
{
    var containers = GetAllUsableCargo();

    return containers
        .Where(c => !IsLocked(c) && !HasCategoryTag(c.CustomName))
        .OrderByDescending(c => c.GetInventory().MaxVolume.RawValue)
        .FirstOrDefault();
}

void RenameStorage(IMyCargoContainer c, string mainTag, string extraTag = "")
{
    if (c == null || IsLocked(c)) return;

    string tags = mainTag;
    if (!string.IsNullOrWhiteSpace(extraTag)) tags += " " + extraTag;

    c.CustomName = "HIM Storage " + tags;
}

void StorageRefresh()
{
    AutoSetupStorage(false);
    DynamicExpansionCheck();
}

void DynamicExpansionCheck()
{
    foreach (var tag in categoryTags)
    {
        if (tag == INGOTS_TAG)
        {
            CheckExpandCategory(INGOTS_TAG, URANIUM_TAG);
        }
        else
        {
            CheckExpandCategory(tag);
        }
    }
}

void CheckExpandCategory(string tag, string extraTag = "")
{
    if (tag == PERSONAL_TAG) return;

    var containers = FindStorageContainers(tag);
    if (containers.Count == 0) return;

    var leastFull = containers.OrderBy(c => FillRatio(c.GetInventory())).FirstOrDefault();
    if (leastFull == null) return;

    // If even the emptiest category container is over threshold, claim a new blank one.
    if (FillRatio(leastFull.GetInventory()) >= EXPAND_AT_FILL_PERCENT)
    {
        var blank = FindBestBlankCargo();
        if (blank != null)
        {
            RenameStorage(blank, tag, extraTag);
            log.AppendLine("- Expanded storage for " + tag);
        }
    }
}

// =======================================================================================
// ITEM CATEGORY
// =======================================================================================

string GetCategory(MyInventoryItem item)
{
    string typeId = item.Type.TypeId.ToString();
    string subtype = item.Type.SubtypeId.ToString();

    if (subtype == "Datapad" || subtype == "ZoneChip" || subtype == "SpaceCredit")
        return TOOLS_TAG;

    if (typeId.EndsWith("_Ore"))
    {
        if (subtype == "Ice") return ICE_TAG;
        return ORES_TAG;
    }

    if (typeId.EndsWith("_Ingot"))
    {
        // Uranium still sorts into [INGOTS] [URANIUM] storage.
        return INGOTS_TAG;
    }

    if (typeId.EndsWith("_AmmoMagazine"))
        return AMMO_TAG;

    if (typeId.EndsWith("_GasContainerObject") || typeId.EndsWith("_OxygenContainerObject"))
        return BOTTLES_TAG;

    if (typeId.EndsWith("_PhysicalGunObject"))
        return TOOLS_TAG;

    if (typeId.EndsWith("_Component"))
    {
        return COMPONENTS_TAG;
    }

    string ls = subtype.ToLower();
    string lt = typeId.ToLower();

    // FARM comes before FOOD so seeds/spores/crops do not get caught as consumables.
    if (ls.Contains("seed") || ls.Contains("spore") || ls.Contains("crop") ||
        ls.Contains("fertilizer") || ls.Contains("soil") || ls.Contains("farm") ||
        ls.Contains("plant") || ls.Contains("mushroom") || ls.Contains("fruit") ||
        ls.Contains("vegetable") || ls.Contains("grain") || ls.Contains("apple") ||
        ls.Contains("wheat") || ls.Contains("corn") || ls.Contains("potato") ||
        ls.Contains("carrot") || ls.Contains("berry") || ls.Contains("leaf"))
        return FARM_TAG;

    // FOOD is for finished consumables, med items, inhibitors, and meat/creature food.
    if (lt.Contains("consumable") || ls.Contains("meal") || ls.Contains("ration") ||
        ls.Contains("medkit") || ls.Contains("med kit") || ls.Contains("medical") ||
        ls.Contains("inhibitor") || ls.Contains("meat") || ls.Contains("cooked") ||
        ls.Contains("raw") || ls.Contains("serapod") || ls.Contains("protein"))
        return FOOD_TAG;

    AddUnknown(item);
    return "";
}

void AddUnknown(MyInventoryItem item)
{
    string key = item.Type.TypeId.ToString() + "/" + item.Type.SubtypeId.ToString();
    if (!unknownItems.ContainsKey(key))
        unknownItems[key] = key;
}

// =======================================================================================
// COUNT / SORT
// =======================================================================================

void CountAllItems()
{
    itemTotals.Clear();
    ingotTotals.Clear();
    oreTotals.Clear();
    unknownItems.Clear();

    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => IsManagedInventoryBlock(b));

    var items = new List<MyInventoryItem>();

    foreach (var block in blocks)
    {
        for (int i = 0; i < block.InventoryCount; i++)
        {
            var inv = block.GetInventory(i);
            items.Clear();
            inv.GetItems(items);

            foreach (var item in items)
            {
                string subtype = item.Type.SubtypeId.ToString();
                string typeId = item.Type.TypeId.ToString();
                long amount = item.Amount.ToIntSafe();

                if (!itemTotals.ContainsKey(subtype))
                    itemTotals[subtype] = 0;

                itemTotals[subtype] += amount;

                if (typeId.EndsWith("_Ingot"))
                {
                    if (!ingotTotals.ContainsKey(subtype))
                        ingotTotals[subtype] = 0;

                    ingotTotals[subtype] += amount;
                }
                else if (typeId.EndsWith("_Ore"))
                {
                    if (!oreTotals.ContainsKey(subtype))
                        oreTotals[subtype] = 0;

                    oreTotals[subtype] += amount;
                }

                GetCategory(item);
            }
        }
    }
}


bool IsOperationalInventoryProtected(IMyTerminalBlock block)
{
    if (block == null) return false;

    if (block is IMyReactor) return true;
    if (block is IMyGasGenerator) return true;

    string subtype = block.BlockDefinition.SubtypeId.ToString().ToLower();
    string name = block.CustomName.ToLower();

    if (subtype.Contains("hydrogenengine")) return true;
    if (subtype.Contains("irrigation") || name.Contains("irrigation")) return true;
    if (subtype.Contains("farm") || name.Contains("farm plot")) return true;

    return false;
}


void EmptyRefineryOutputs()
{
    int moves = EmptyRefineryOutputsCapped(PERF_REFINERY_MOVE_CAP);
    if (moves > 0) log.AppendLine("- Refinery outputs cleared. Moves: " + moves);
}


bool TransferAnyAmountToStorage(IMyInventory source, List<IMyInventory> destinations, MyInventoryItem item)
{
    if (source == null || destinations == null || destinations.Count == 0) return false;

    foreach (var dest in destinations)
    {
        if (dest == null || dest.IsFull) continue;
        if (dest.Owner.EntityId == source.Owner.EntityId) continue;

        if (source.TransferItemTo(dest, item))
            return true;
    }

    foreach (var dest in destinations)
    {
        if (dest == null || dest.IsFull) continue;
        if (dest.Owner.EntityId == source.Owner.EntityId) continue;

        int moved = TryMovePartialStack(source, dest, item, Math.Max(1, item.Amount.ToIntSafe()));
        if (moved > 0)
            return true;
    }

    return false;
}


void SortBaseInventory()
{
    var map = BuildStorageMap();

    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => IsManagedInventoryBlock(b));

    var items = new List<MyInventoryItem>();
    bool moved = false;
    int moveCount = 0;

    foreach (var block in blocks)
    {
        if (moveCount >= PERF_MAX_SORT_MOVES) break;
        if (IsLocked(block)) continue;
        if (IsOperationalInventoryProtected(block)) continue;

        var prod = block as IMyProductionBlock;

        for (int i = 0; i < block.InventoryCount; i++)
        {
            if (moveCount >= PERF_MAX_SORT_MOVES) break;

            // Do not skip refinery/assembler OUTPUT just because the block is producing.
            // Inventory 0 is usually input and inventory 1 is usually output.
            if (prod != null && prod.IsProducing && i == 0)
                continue;

            var source = block.GetInventory(i);
            items.Clear();
            source.GetItems(items);

            for (int x = items.Count - 1; x >= 0; x--)
            {
                if (moveCount >= PERF_MAX_SORT_MOVES) break;

                var item = items[x];
                string cat = GetCategory(item);

                if (string.IsNullOrWhiteSpace(cat)) continue;
                if (!map.ContainsKey(cat) || map[cat].Count == 0) continue;

                if (TransferItemToStorage(source, map[cat], item))
                {
                    moved = true;
                    moveCount++;
                }
            }
        }
    }

    if (moved) log.AppendLine("- Inventory sorted. Moves: " + moveCount);
}



void DeepSweepInventory(bool fullReset)
{
    RunDeepSweepStage(fullReset);
}



void RunDeepSweepStage(bool manual)
{
    int stage = deepSweepStage % 4;
    deepSweepStage++;

    if (stage == 0)
    {
        int moved = CleanProductionInventories(PERF_DEEP_SWEEP_MOVE_CAP);
        lastSweepStage = "Assemblers/production cleanup";
        if (moved > 0) log.AppendLine("- Sweep production moved: " + moved);
    }
    else if (stage == 1)
    {
        int moved = CleanConnectorInventories(PERF_DEEP_SWEEP_MOVE_CAP);
        lastSweepStage = "Connector cleanup";
        if (moved > 0) log.AppendLine("- Sweep connectors moved: " + moved);
    }
    else if (stage == 2)
    {
        int moved = EmptyRefineryOutputsCapped(PERF_REFINERY_MOVE_CAP);
        lastSweepStage = "Refinery output cleanup";
        if (moved > 0) log.AppendLine("- Sweep refineries moved: " + moved);
    }
    else
    {
        int moved = CleanGeneralInventories(PERF_DEEP_SWEEP_MOVE_CAP);
        lastSweepStage = "General inventory cleanup";
        if (moved > 0) log.AppendLine("- Sweep general moved: " + moved);
    }

    if (manual)
        log.AppendLine("- Manual sweep stage: " + lastSweepStage);
}

int CleanProductionInventories(int moveCap)
{
    var map = BuildStorageMap();

    var blocks = new List<IMyProductionBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b =>
        IsSameMainGrid(b) &&
        !IsIgnored(b) &&
        !IsLocked(b));

    return CleanBlocksToStorage(blocks.Cast<IMyTerminalBlock>().ToList(), map, moveCap);
}

int CleanConnectorInventories(int moveCap)
{
    var map = BuildStorageMap();

    var blocks = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(blocks, b =>
        IsSameMainGrid(b) &&
        !IsIgnored(b) &&
        !IsLocked(b));

    return CleanBlocksToStorage(blocks.Cast<IMyTerminalBlock>().ToList(), map, moveCap);
}

int CleanGeneralInventories(int moveCap)
{
    var map = BuildStorageMap();

    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b =>
        IsManagedInventoryBlock(b) &&
        !IsLocked(b) &&
        !(b is IMyCargoContainer) &&
        !(b is IMyReactor) &&
        !(b is IMyGasGenerator));

    return CleanBlocksToStorage(blocks, map, moveCap);
}

int CleanBlocksToStorage(List<IMyTerminalBlock> blocks, Dictionary<string, List<IMyInventory>> map, int moveCap)
{
    int moved = 0;
    var items = new List<MyInventoryItem>();

    foreach (var block in blocks)
    {
        if (moved >= moveCap) break;
        if (block == null || !block.HasInventory) continue;
        if (IsOperationalInventoryProtected(block)) continue;

        for (int i = 0; i < block.InventoryCount; i++)
        {
            if (moved >= moveCap) break;

            var source = block.GetInventory(i);
            items.Clear();
            source.GetItems(items);

            for (int x = items.Count - 1; x >= 0; x--)
            {
                if (moved >= moveCap) break;

                var item = items[x];
                string cat = GetCategory(item);
                if (string.IsNullOrWhiteSpace(cat)) continue;
                if (!map.ContainsKey(cat) || map[cat].Count == 0) continue;
                if (InventoryBelongsToCategory(source, cat)) continue;

                if (TransferItemToStorage(source, map[cat], item))
                    moved++;
            }
        }
    }

    return moved;
}

int EmptyRefineryOutputsCapped(int moveCap)
{
    var ingotStorage = FindStorage(INGOTS_TAG);
    if (ingotStorage.Count == 0) return 0;

    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b =>
        IsSameMainGrid(b) &&
        !IsIgnored(b) &&
        !IsLocked(b) &&
        b.HasInventory &&
        b.InventoryCount >= 2);

    var items = new List<MyInventoryItem>();
    int moves = 0;

    foreach (var block in blocks)
    {
        if (moves >= moveCap) break;

        var output = block.GetInventory(1);
        items.Clear();
        output.GetItems(items);

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (moves >= moveCap) break;

            var item = items[i];
            string typeId = item.Type.TypeId.ToString();
            if (!typeId.EndsWith("_Ingot")) continue;

            if (TransferAnyAmountToStorage(output, ingotStorage, item))
                moves++;
        }
    }

    return moves;
}

void ForceSortAllInventory()
{
    RunDeepSweepStage(true);
}



int CountMisplacedStacks()
{
    return 0;
}


bool InventoryBelongsToCategory(IMyInventory inv, string category)
{
    if (inv == null || string.IsNullOrWhiteSpace(category)) return false;

    var owner = inv.Owner as IMyTerminalBlock;
    if (owner == null) return false;

    return owner.CustomName.Contains(category);
}

Dictionary<string, List<IMyInventory>> BuildStorageMap()
{
    var map = new Dictionary<string, List<IMyInventory>>();

    map[ORES_TAG] = FindStorage(ORES_TAG);
    map[ICE_TAG] = FindStorage(ICE_TAG);
    map[INGOTS_TAG] = FindStorage(INGOTS_TAG);
    map[COMPONENTS_TAG] = FindStorage(COMPONENTS_TAG);
    map[TOOLS_TAG] = FindStorage(TOOLS_TAG);
    map[FOOD_TAG] = FindStorage(FOOD_TAG);
    map[FARM_TAG] = FindStorage(FARM_TAG);
    map[AMMO_TAG] = FindStorage(AMMO_TAG);
    map[BOTTLES_TAG] = FindStorage(BOTTLES_TAG);

    return map;
}

// =======================================================================================
// BASE POWER
// =======================================================================================

void ManageBaseReactors()
{
    var sources = FindStorage(INGOTS_TAG);
    if (sources.Count == 0) return;

    var reactors = new List<IMyReactor>();
    GridTerminalSystem.GetBlocksOfType(reactors, r => IsSameMainGrid(r) && !IsIgnored(r));

    bool moved = false;

    foreach (var r in reactors)
    {
        var inv = r.GetInventory();
        int target = LooksLikeLargeReactor(r) ? BASE_LARGE_REACTOR_URANIUM : BASE_SMALL_REACTOR_URANIUM;
        int current = inv.GetItemAmount(uraniumId).ToIntSafe();

        if (current < target)
        {
            if (PullAmount(uraniumId, sources, inv, target - current))
                moved = true;
        }
    }

    if (moved) log.AppendLine("- Base reactors stocked.");
}

void ManageBaseIceConsumers()
{
    var sources = FindStorage(ICE_TAG);
    if (sources.Count == 0) return;

    bool moved = false;

    var gens = new List<IMyGasGenerator>();
    GridTerminalSystem.GetBlocksOfType(gens, g => IsSameMainGrid(g) && !IsIgnored(g));

    foreach (var gen in gens)
    {
        var inv = gen.GetInventory();
        int loops = 0;

        while (!inv.IsFull && loops < BASE_ICE_FILL_MAX_LOOPS)
        {
            loops++;
            if (!PullAmount(iceId, sources, inv, BASE_ICE_FILL_BATCH))
                break;

            moved = true;
        }
    }

    var possibleIrrigation = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(possibleIrrigation, b =>
        IsSameMainGrid(b) &&
        b.HasInventory &&
        !IsIgnored(b) &&
        (
            b.BlockDefinition.SubtypeId.ToString().ToLower().Contains("irrigation") ||
            b.CustomName.ToLower().Contains("irrigation")
        ));

    foreach (var block in possibleIrrigation)
    {
        var inv = block.GetInventory();
        int loops = 0;

        while (!inv.IsFull && loops < BASE_ICE_FILL_MAX_LOOPS)
        {
            loops++;
            if (!PullAmount(iceId, sources, inv, BASE_ICE_FILL_BATCH))
                break;

            moved = true;
        }
    }

    if (moved) log.AppendLine("- Base ice consumers filled.");
}

// =======================================================================================
// DOCKED SHIPS
// =======================================================================================


void ManageDockedShips()
{
    var baseConnectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(baseConnectors, c =>
        IsSameMainGrid(c) &&
        !IsIgnored(c) &&
        c.Status == MyShipConnectorStatus.Connected &&
        c.OtherConnector != null &&
        !c.OtherConnector.IsSameConstructAs(Me));

    int serviced = 0;

    foreach (var baseConnector in baseConnectors)
    {
        if (serviced >= PERF_MAX_SHIPS_PER_CYCLE) break;

        var shipConnector = baseConnector.OtherConnector;
        var shipGrid = shipConnector.CubeGrid;

        string roleText = shipConnector.CustomName + " " + shipGrid.CustomName;

        bool isFighter = roleText.Contains(FIGHTER_TAG);
        bool isMiner = roleText.Contains(MINER_TAG);
        bool noUnload = roleText.Contains(NOUNLOAD_TAG);
        bool noRefuel = roleText.Contains(NOREFUEL_TAG);

        string displayName = CleanShipDisplayName(shipGrid.CustomName);

        if (!isFighter && !isMiner)
        {
            shipLog.AppendLine("| " + TrimFit(displayName, 26) + " => no role tag");
            continue;
        }

        serviced++;

        if (isMiner && !noUnload)
        {
            UnloadMinerShip(shipGrid);
        }
        else if (isMiner && noUnload)
        {
            shipLog.AppendLine("| " + TrimFit(displayName, 26) + " => unload skipped");
        }

        if (!noRefuel)
        {
            RefuelShipReactors(shipGrid);
            FillShipIce(shipGrid);
            RefillShipBottles(shipGrid);

            if (isFighter)
            {
                RefillFighterAmmo(shipGrid);
            }
        }
        else
        {
            shipLog.AppendLine("| " + TrimFit(displayName, 26) + " => refuel skipped");
        }

        ShipReadyReport(shipGrid, isFighter, isMiner);
    }

    if (baseConnectors.Count > PERF_MAX_SHIPS_PER_CYCLE)
        shipLog.AppendLine("| Ship queue: servicing " + PERF_MAX_SHIPS_PER_CYCLE + " per cycle");
}




bool IsShipUnloadBlock(IMyTerminalBlock b)
{
    if (b == null || !b.HasInventory || IsIgnored(b)) return false;

    if (b is IMyCockpit) return false;
    if (b is IMyShipController) return false;
    if (b is IMyRemoteControl) return false;

    return b is IMyCargoContainer ||
           b is IMyShipConnector ||
           b is IMyShipDrill ||
           b is IMyRefinery ||
           b is IMyCollector;
}

void UnloadMinerShip(IMyCubeGrid shipGrid)
{
    var map = BuildStorageMap();

    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => b.CubeGrid == shipGrid && IsShipUnloadBlock(b));

    var items = new List<MyInventoryItem>();
    bool moved = false;

    foreach (var block in blocks)
    {
        for (int i = 0; i < block.InventoryCount; i++)
        {
            var inv = block.GetInventory(i);
            items.Clear();
            inv.GetItems(items);

            for (int x = items.Count - 1; x >= 0; x--)
            {
                var item = items[x];
                string cat = GetCategory(item);

                bool allowed = cat == ORES_TAG || cat == ICE_TAG || cat == INGOTS_TAG;
                if (!allowed) continue;
                if (!map.ContainsKey(cat) || map[cat].Count == 0) continue;

                if (TransferItemToStorage(inv, map[cat], item))
                    moved = true;
            }
        }
    }

    if (moved)
    {
        log.AppendLine("- Miner unloaded.");
        shipLog.AppendLine("  unloaded resources");
    }
}

void RefuelShipReactors(IMyCubeGrid shipGrid)
{
    var sources = FindStorage(INGOTS_TAG);
    if (sources.Count == 0) return;

    var reactors = new List<IMyReactor>();
    GridTerminalSystem.GetBlocksOfType(reactors, r => r.CubeGrid == shipGrid && !IsIgnored(r));

    bool moved = false;

    foreach (var r in reactors)
    {
        var inv = r.GetInventory();
        int target = LooksLikeLargeReactor(r) ? SHIP_LARGE_REACTOR_URANIUM : SHIP_SMALL_REACTOR_URANIUM;
        int current = inv.GetItemAmount(uraniumId).ToIntSafe();

        if (current < target)
        {
            if (PullAmount(uraniumId, sources, inv, target - current))
                moved = true;
        }
    }

    if (moved) shipLog.AppendLine("| reactors refueled");
}

void FillShipIce(IMyCubeGrid shipGrid)
{
    var sources = FindStorage(ICE_TAG);
    if (sources.Count == 0) return;

    var gens = new List<IMyGasGenerator>();
    GridTerminalSystem.GetBlocksOfType(gens, g => g.CubeGrid == shipGrid && !IsIgnored(g));

    bool moved = false;

    foreach (var gen in gens)
    {
        var inv = gen.GetInventory();

        int loops = 0;
        while (!inv.IsFull && loops < SHIP_H2GEN_MAX_BATCH_LOOPS)
        {
            loops++;
            if (!PullAmount(iceId, sources, inv, SHIP_H2GEN_ICE_BATCH))
                break;

            moved = true;
        }
    }

    if (moved) shipLog.AppendLine("| ice replenished");
}

void RefillShipBottles(IMyCubeGrid shipGrid)
{
    var sources = FindStorage(BOTTLES_TAG);
    if (sources.Count == 0) return;

    var cockpits = new List<IMyCockpit>();
    GridTerminalSystem.GetBlocksOfType(cockpits, c => c.CubeGrid == shipGrid && c.HasInventory && !IsIgnored(c));

    bool moved = false;

    foreach (var c in cockpits)
    {
        var inv = c.GetInventory();

        int currentH = inv.GetItemAmount(hydrogenBottleId).ToIntSafe();
        if (currentH < SHIP_HYDROGEN_BOTTLE_MIN)
        {
            if (PullAmount(hydrogenBottleId, sources, inv, SHIP_HYDROGEN_BOTTLE_MIN - currentH))
                moved = true;
        }

        int currentO = inv.GetItemAmount(oxygenBottleId).ToIntSafe();
        if (currentO < SHIP_OXYGEN_BOTTLE_MIN)
        {
            if (PullAmount(oxygenBottleId, sources, inv, SHIP_OXYGEN_BOTTLE_MIN - currentO))
                moved = true;
        }
    }

    if (moved) shipLog.AppendLine("| bottles replenished");
}


void RefillFighterAmmo(IMyCubeGrid shipGrid)
{
    var baseAmmoSources = FindStorage(AMMO_TAG);
    if (baseAmmoSources.Count == 0)
    {
        shipLog.AppendLine("  ammo: no base [AMMO] storage");
        return;
    }

    // ammo check started

    var allowedAmmo = DetectAmmoAlreadyOnShip(shipGrid);

    if (allowedAmmo.Count == 0)
    {
        shipLog.AppendLine("  ammo: no loaded ammo detected");
        shipLog.AppendLine("  seed each weapon with 1 ammo");
        return;
    }

    allowedAmmo = allowedAmmo
        .OrderByDescending(id => AmmoPriority(id.SubtypeName))
        .ThenBy(id => id.SubtypeName)
        .ToList();

    // ammo type count detected internally

    bool moved = false;

    moved = TopUpLoadedWeaponAmmo(shipGrid, baseAmmoSources, allowedAmmo) || moved;
    moved = RemoveWrongAmmoFromFighterCargo(shipGrid, allowedAmmo) || moved;
    moved = FillFighterAmmoCargoEvenly(shipGrid, baseAmmoSources, allowedAmmo) || moved;

    if (moved) shipLog.AppendLine("| fighter ammo replenished");
}




List<MyDefinitionId> DetectAmmoAlreadyOnShip(IMyCubeGrid shipGrid)
{
    var found = new Dictionary<string, MyDefinitionId>();

    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b =>
        b.CubeGrid == shipGrid &&
        b.HasInventory &&
        !IsIgnored(b));

    var items = new List<MyInventoryItem>();

    foreach (var b in blocks)
    {
        // Do not learn loadout from cockpits/player seats.
        if (b is IMyCockpit) continue;
        if (b is IMyShipController) continue;
        if (b is IMyRemoteControl) continue;

        for (int i = 0; i < b.InventoryCount; i++)
        {
            var inv = b.GetInventory(i);
            items.Clear();
            inv.GetItems(items);

            foreach (var item in items)
            {
                if (!item.Type.TypeId.ToString().EndsWith("_AmmoMagazine")) continue;

                string key = item.Type.ToString();
                if (!found.ContainsKey(key))
                    found[key] = item.Type;
            }
        }
    }

    return found.Values.ToList();
}

int AmmoPriority(string subtype)
{
    string s = subtype.ToLower();

    if (s.Contains(" he") || s.Contains("_he") || s.Contains("-he") ||
        s.Contains("heshell") || s.Contains("he_shell") || s.Contains("he shell") ||
        s.EndsWith("he") || s.Contains("highexplosive") || s.Contains("high_explosive"))
        return 100;

    if (s.Contains(" ap") || s.Contains("_ap") || s.Contains("-ap") ||
        s.Contains("apshell") || s.Contains("ap_shell") || s.Contains("ap shell") ||
        s.EndsWith("ap") || s.Contains("armor") || s.Contains("armour") || s.Contains("piercing"))
        return 50;

    return 10;
}

bool TopUpLoadedWeaponAmmo(IMyCubeGrid shipGrid, List<IMyInventory> baseAmmoSources, List<MyDefinitionId> allowedAmmo)
{
    var allowed = new HashSet<string>(allowedAmmo.Select(a => a.ToString()));

    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b =>
        b.CubeGrid == shipGrid &&
        b.HasInventory &&
        !IsIgnored(b));

    bool moved = false;
    var items = new List<MyInventoryItem>();

    foreach (var b in blocks)
    {
        // Top up inventories that already contain one of the allowed ammo types.
        // This works for modded WeaponCore weapons without needing block subtype names.
        if (b is IMyCockpit) continue;
        if (b is IMyShipController) continue;
        if (b is IMyRemoteControl) continue;
        if (b is IMyCargoContainer) continue;

        for (int i = 0; i < b.InventoryCount; i++)
        {
            var inv = b.GetInventory(i);
            items.Clear();
            inv.GetItems(items);

            MyInventoryItem? bestAmmo = null;
            int bestPriority = -1;

            foreach (var it in items)
            {
                if (!it.Type.TypeId.ToString().EndsWith("_AmmoMagazine")) continue;
                if (!allowed.Contains(it.Type.ToString())) continue;

                int prio = AmmoPriority(it.Type.SubtypeId.ToString());
                if (prio > bestPriority)
                {
                    bestPriority = prio;
                    bestAmmo = it;
                }
            }

            if (!bestAmmo.HasValue) continue;

            if (!inv.IsFull)
            {
                if (PullAmount(bestAmmo.Value.Type, baseAmmoSources, inv, FIGHTER_WEAPON_TOPUP_BATCH))
                    moved = true;
            }
        }
    }

    return moved;
}

bool RemoveWrongAmmoFromFighterCargo(IMyCubeGrid shipGrid, List<MyDefinitionId> allowedAmmo)
{
    var baseAmmoStorage = FindStorage(AMMO_TAG);
    if (baseAmmoStorage.Count == 0) return false;

    var allowed = new HashSet<string>(allowedAmmo.Select(a => a.ToString()));

    var cargos = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(cargos, c =>
        c.CubeGrid == shipGrid &&
        c.CustomName.Contains(AMMO_TAG) &&
        !IsIgnored(c));

    bool moved = false;
    var items = new List<MyInventoryItem>();

    foreach (var c in cargos)
    {
        var inv = c.GetInventory();
        items.Clear();
        inv.GetItems(items);

        for (int i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            if (!item.Type.TypeId.ToString().EndsWith("_AmmoMagazine")) continue;

            if (!allowed.Contains(item.Type.ToString()))
            {
                if (TransferItemToStorage(inv, baseAmmoStorage, item))
                    moved = true;
            }
        }
    }

    return moved;
}

bool FillFighterAmmoCargoEvenly(IMyCubeGrid shipGrid, List<IMyInventory> baseAmmoSources, List<MyDefinitionId> allowedAmmo)
{
    var cargos = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(cargos, c =>
        c.CubeGrid == shipGrid &&
        c.CustomName.Contains(AMMO_TAG) &&
        !IsIgnored(c));

    if (cargos.Count == 0)
    {
        shipLog.AppendLine("  ammo: no ship [AMMO] cargo");
        return false;
    }

    if (allowedAmmo.Count == 0) return false;

    bool moved = false;

    int pass = 0;
    while (pass < FIGHTER_AMMO_CARGO_PASS_LIMIT)
    {
        pass++;

        double fill = CombinedFillRatio(cargos.Select(c => c.GetInventory()).ToList());
        if (fill >= FIGHTER_AMMO_CARGO_TARGET_FILL)
            break;

        bool movedThisPass = false;

        var orderedAmmo = allowedAmmo
            .OrderBy(id => CountAmmoInCargos(cargos, id))
            .ThenByDescending(id => AmmoPriority(id.SubtypeName))
            .ToList();

        foreach (var ammoId in orderedAmmo)
        {
            var targetInv = cargos
                .Select(c => c.GetInventory())
                .Where(i => !i.IsFull)
                .OrderBy(i => FillRatio(i))
                .FirstOrDefault();

            if (targetInv == null) return moved;

            if (PullAmount(ammoId, baseAmmoSources, targetInv, 1))
            {
                moved = true;
                movedThisPass = true;
                break;
            }
        }

        if (!movedThisPass)
            break;
    }

    return moved;
}

double CombinedFillRatio(List<IMyInventory> invs)
{
    long cur = 0;
    long max = 0;

    foreach (var inv in invs)
    {
        if (inv == null) continue;
        cur += inv.CurrentVolume.RawValue;
        max += inv.MaxVolume.RawValue;
    }

    if (max <= 0) return 1;
    return (double)cur / (double)max;
}

long CountAmmoInCargos(List<IMyCargoContainer> cargos, MyDefinitionId ammoId)
{
    long total = 0;

    foreach (var c in cargos)
    {
        var inv = c.GetInventory();
        total += inv.GetItemAmount(ammoId).ToIntSafe();
    }

    return total;
}



string CleanShipDisplayName(string name)
{
    if (string.IsNullOrWhiteSpace(name)) return "Unknown Ship";

    string n = name;
    n = n.Replace(FIGHTER_TAG, "");
    n = n.Replace(MINER_TAG, "");
    n = n.Replace(NOUNLOAD_TAG, "");
    n = n.Replace(NOREFUEL_TAG, "");
    n = System.Text.RegularExpressions.Regex.Replace(n, @"\s+", " ").Trim();

    if (string.IsNullOrWhiteSpace(n)) n = "Unknown Ship";
    return n;
}


void ShipReadyReport(IMyCubeGrid shipGrid, bool isFighter, bool isMiner)
{
    bool reactorsOk = ShipReactorsReady(shipGrid);
    bool iceOk = ShipIceReady(shipGrid);
    bool bottlesOk = ShipBottlesReady(shipGrid);
    bool ammoOk = !isFighter || FighterAmmoReady(shipGrid);

    string role = isFighter ? "FIGHTER" : "MINER";
    string status = (reactorsOk && iceOk && bottlesOk && ammoOk) ? "READY" : "SERVICE NEEDED";
    string displayName = CleanShipDisplayName(shipGrid.CustomName);

    var sb = new StringBuilder();
    sb.AppendLine(TrimFit(displayName, 28) + " [" + role + "]");
    sb.AppendLine("  Reactors : " + (reactorsOk ? "OK" : "LOW"));
    sb.AppendLine("  Ice      : " + (iceOk ? "OK" : "LOW"));
    sb.AppendLine("  Bottles  : " + (bottlesOk ? "OK" : "LOW"));
    if (isFighter)
        sb.AppendLine("  Ammo     : " + (ammoOk ? "OK" : "LOW"));
    sb.AppendLine("  Status   : " + status);

    shipServiceSummary[shipGrid.EntityId.ToString()] = sb.ToString();

    string action = isFighter ? "fighter serviced" : "miner serviced";
    shipLog.AppendLine("| " + TrimFit(displayName, 24) + " => " + status + " (" + action + ")");
}



bool ShipReactorsReady(IMyCubeGrid shipGrid)
{
    var reactors = new List<IMyReactor>();
    GridTerminalSystem.GetBlocksOfType(reactors, r => r.CubeGrid == shipGrid && !IsIgnored(r));

    if (reactors.Count == 0) return true;

    foreach (var r in reactors)
    {
        int target = LooksLikeLargeReactor(r) ? SHIP_LARGE_REACTOR_URANIUM : SHIP_SMALL_REACTOR_URANIUM;
        int current = r.GetInventory().GetItemAmount(uraniumId).ToIntSafe();

        if (current < target)
            return false;
    }

    return true;
}

bool ShipIceReady(IMyCubeGrid shipGrid)
{
    var gens = new List<IMyGasGenerator>();
    GridTerminalSystem.GetBlocksOfType(gens, g => g.CubeGrid == shipGrid && !IsIgnored(g));

    if (gens.Count == 0) return true;

    foreach (var g in gens)
    {
        if (g.GetInventory().IsFull)
            continue;

        // If it has at least some ice, call it usable. Full refill can take multiple cycles.
        if (g.GetInventory().GetItemAmount(iceId).ToIntSafe() <= 0)
            return false;
    }

    return true;
}

bool ShipBottlesReady(IMyCubeGrid shipGrid)
{
    var cockpits = new List<IMyCockpit>();
    GridTerminalSystem.GetBlocksOfType(cockpits, c => c.CubeGrid == shipGrid && c.HasInventory && !IsIgnored(c));

    if (cockpits.Count == 0) return true;

    foreach (var c in cockpits)
    {
        var inv = c.GetInventory();
        if (inv.GetItemAmount(hydrogenBottleId).ToIntSafe() >= SHIP_HYDROGEN_BOTTLE_MIN)
            return true;
    }

    return false;
}

bool FighterAmmoReady(IMyCubeGrid shipGrid)
{
    var cargos = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(cargos, c =>
        c.CubeGrid == shipGrid &&
        c.CustomName.Contains(AMMO_TAG) &&
        !IsIgnored(c));

    if (cargos.Count == 0) return false;

    double fill = CombinedFillRatio(cargos.Select(c => c.GetInventory()).ToList());
    return fill >= 0.25;
}

float GetBasePowerOutput()
{
    var powerBlocks = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType(powerBlocks, p => IsSameMainGrid(p) && !IsIgnored(p));

    float total = 0f;
    foreach (var p in powerBlocks)
        total += p.CurrentOutput;

    return total;
}

string GetIngotTotal(string subtype)
{
    long v = 0;
    ingotTotals.TryGetValue(subtype, out v);
    return FormatNum(v);
}

string GetOreTotal(string subtype)
{
    long v = 0;
    oreTotals.TryGetValue(subtype, out v);
    return FormatNum(v);
}

void AddResourcePair(StringBuilder sb, string leftName, string leftValue, string rightName, string rightValue)
{
    string left = " " + leftName.PadRight(10) + leftValue.PadLeft(8);
    string right = " " + rightName.PadRight(13) + rightValue.PadLeft(8);
    sb.AppendLine("║" + left.PadRight(22) + "║" + right.PadRight(29) + "║");
}

// =======================================================================================
// LCDS
// =======================================================================================

void UpdateLcds()
{
    UpdateNanoLcd();
    UpdateResourceLcd();
    UpdateStorageLcd();
    UpdateShipLcd();
    UpdateUnknownLcd();
    UpdateAutoCraftPlaceholder();
    UpdateStatusLcd();
}

void PrepareLcd(IMyTextPanel panel, float fontSize = 0.50f)
{
    if (panel == null) return;
    panel.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    panel.Font = "Monospace";
    panel.FontSize = fontSize;
}

string Line(char c, int len)
{
    return new string(c, len);
}

string Center(string text, int width)
{
    if (text == null) text = "";
    if (text.Length >= width) return text.Substring(0, width);
    int left = (width - text.Length) / 2;
    int right = width - text.Length - left;
    return new string(' ', left) + text + new string(' ', right);
}

string TrimFit(string text, int width)
{
    if (text == null) text = "";
    if (text.Length <= width) return text;
    if (width <= 3) return text.Substring(0, width);
    return text.Substring(0, width - 3) + "...";
}

void Header(StringBuilder sb, string title, int width = 48)
{
    sb.AppendLine("╔" + Line('═', width) + "╗");
    sb.AppendLine("║" + Center(title, width) + "║");
    sb.AppendLine("╚" + Line('═', width) + "╝");
}

void Section(StringBuilder sb, string title, int width = 48)
{
    sb.AppendLine();
    sb.AppendLine("┌─ " + title + " " + Line('─', Math.Max(0, width - title.Length - 4)));
}

void StatLine(StringBuilder sb, string label, string value, int width = 48)
{
    string left = "│ " + label.PadRight(14) + ": ";
    string line = left + value;
    sb.AppendLine(TrimFit(line, width));
}

void UpdateNanoLcd()
{
    if (nanoLcd == null) return;
    PrepareLcd(nanoLcd, 0.50f);

    var sb = new StringBuilder();
    Header(sb, "HALF'S INVENTORY MANAGEMENT");
    sb.AppendLine(Center("HIM  •  BASE COMMAND", 50));

    Section(sb, "SYSTEM");
    StatLine(sb, "Status", "ONLINE");
    StatLine(sb, "Version", VERSION);
    StatLine(sb, "Unknown", unknownItems.Count.ToString());
    StatLine(sb, "Power Out", GetBasePowerOutput().ToString("0.0") + " MW");

    Section(sb, "KEY RESOURCES");
    StatLine(sb, "Ice", FormatNum(GetTotal("Ice")));
    StatLine(sb, "Uranium", FormatNum(GetTotal("Uranium")));
    StatLine(sb, "Iron", FormatNum(GetTotal("Iron")));
    StatLine(sb, "Stone", FormatNum(GetTotal("Stone")));

    Section(sb, "ACTIVITY");
    if (log.Length == 0) sb.AppendLine("│ No new actions.");
    else
    {
        var lines = log.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int count = 0;
        foreach (var l in lines)
        {
            if (count++ >= 6) break;
            sb.AppendLine("│ " + TrimFit(l.Trim(), 44));
        }
    }

    nanoLcd.WriteText(sb.ToString());
}

void UpdateResourceLcd()
{
    if (resourceLcd == null) return;
    PrepareLcd(resourceLcd, 0.46f);

    float power = GetBasePowerOutput();

    var sb = new StringBuilder();

    sb.AppendLine("╔══════════════════ HIM RESOURCE OVERVIEW ══════════════════╗");
    sb.AppendLine("║                                                          ║");
    sb.AppendLine("║  ICE " + FormatNum(GetTotal("Ice")).PadLeft(14) +
                  "  │  POWER " + power.ToString("0.0").PadLeft(10) + " MW              ║");
    sb.AppendLine("║                                                          ║");
    sb.AppendLine("╠════════════════════════════╦═════════════════════════════╣");
    sb.AppendLine("║           INGOTS           ║            ORES             ║");
    sb.AppendLine("╠════════════════════════════╬═════════════════════════════╣");

    AddResourcePairPretty(sb, "Iron", GetIngotTotal("Iron"), "Iron Ore", GetOreTotal("Iron"));
    AddResourcePairPretty(sb, "Nickel", GetIngotTotal("Nickel"), "Nickel Ore", GetOreTotal("Nickel"));
    AddResourcePairPretty(sb, "Cobalt", GetIngotTotal("Cobalt"), "Cobalt Ore", GetOreTotal("Cobalt"));
    AddResourcePairPretty(sb, "Silicon", GetIngotTotal("Silicon"), "Silicon Ore", GetOreTotal("Silicon"));
    AddResourcePairPretty(sb, "Silver", GetIngotTotal("Silver"), "Silver Ore", GetOreTotal("Silver"));
    AddResourcePairPretty(sb, "Gold", GetIngotTotal("Gold"), "Gold Ore", GetOreTotal("Gold"));
    AddResourcePairPretty(sb, "Platinum", GetIngotTotal("Platinum"), "Platinum Ore", GetOreTotal("Platinum"));
    AddResourcePairPretty(sb, "Magnesium", GetIngotTotal("Magnesium"), "Magnesium Ore", GetOreTotal("Magnesium"));
    AddResourcePairPretty(sb, "Uranium", GetIngotTotal("Uranium"), "Uranium Ore", GetOreTotal("Uranium"));
    AddResourcePairPretty(sb, "Gravel", GetIngotTotal("Stone"), "Stone", GetOreTotal("Stone"));

    sb.AppendLine("╚════════════════════════════╩═════════════════════════════╝");
    sb.AppendLine(" POWER: " + power.ToString("0.0") + " MW | ICE: Dedicated Storage");

    resourceLcd.WriteText(sb.ToString());
}

void AddResourcePairPretty(StringBuilder sb, string leftName, string leftValue, string rightName, string rightValue)
{
    string left = " " + leftName.PadRight(12) + " " + leftValue.PadLeft(10) + " ";
    string right = " " + rightName.PadRight(13) + " " + rightValue.PadLeft(10) + " ";
    sb.AppendLine("║" + left.PadRight(28) + "║" + right.PadRight(29) + "║");
}

void AddTotal(StringBuilder sb, string subtype)
{
    sb.AppendLine(" " + subtype.PadRight(10) + FormatNum(GetTotal(subtype)));
}

void UpdateStorageLcd()
{
    if (storageLcd == null) return;
    PrepareLcd(storageLcd, 0.48f);

    var sb = new StringBuilder();
    Header(sb, "HIM STORAGE MAP");

    AddStorageStatusPretty(sb, ORES_TAG);
    AddStorageStatusPretty(sb, ICE_TAG);
    AddStorageStatusPretty(sb, INGOTS_TAG);
    AddStorageStatusPretty(sb, COMPONENTS_TAG);
    AddStorageStatusPretty(sb, TOOLS_TAG);
    AddStorageStatusPretty(sb, FOOD_TAG);
    AddStorageStatusPretty(sb, FARM_TAG);
    AddStorageStatusPretty(sb, AMMO_TAG);
    AddStorageStatusPretty(sb, BOTTLES_TAG);

    Section(sb, "RULES");
    sb.AppendLine("│ [PERSONAL] = player-only / ignored");
    sb.AppendLine("│ [LOCK]     = no rename, still usable");
    sb.AppendLine("│ Expands when storage passes " + (EXPAND_AT_FILL_PERCENT * 100).ToString("0") + "%");

    storageLcd.WriteText(sb.ToString());
}

void AddStorageStatusPretty(StringBuilder sb, string tag)
{
    var invs = FindStorage(tag);
    string name = categoryDisplayNames.ContainsKey(tag) ? categoryDisplayNames[tag] : tag;

    if (invs.Count == 0)
    {
        sb.AppendLine("│ " + name.PadRight(12) + " MISSING " + tag);
        return;
    }

    long cur = 0;
    long max = 0;

    foreach (var inv in invs)
    {
        cur += inv.CurrentVolume.RawValue;
        max += inv.MaxVolume.RawValue;
    }

    double pct = max > 0 ? (double)cur / (double)max : 0;
    sb.AppendLine("│ " + name.PadRight(11) + Bar(pct, 16) + " " + (pct * 100).ToString("0").PadLeft(3) + "%  x" + invs.Count);
}


void UpdateShipLcd()
{
    if (shipLcd == null) return;
    PrepareLcd(shipLcd, 0.47f);

    var sb = new StringBuilder();
    Header(sb, "HIM SHIP SERVICE");

    Section(sb, "SHIP TAGS");
    sb.AppendLine("| [FIGHTER]  full service + ammo");
    sb.AppendLine("| [MINER]    unload + refuel");
    sb.AppendLine("| [NOUNLOAD] skip unloading");
    sb.AppendLine("| [NOREFUEL] skip refueling");

    Section(sb, "SERVICE STATUS");

    if (shipServiceSummary.Count == 0)
    {
        sb.AppendLine("| No managed ships this cycle.");
    }
    else
    {
        int shipCount = 0;
        foreach (var kvp in shipServiceSummary)
        {
            if (shipCount++ >= 4)
            {
                sb.AppendLine("| More ships docked...");
                break;
            }

            var lines = kvp.Value.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in lines)
                sb.AppendLine("| " + TrimFit(l.Trim(), 44));

            sb.AppendLine("|");
        }
    }

    Section(sb, "SERVICE LOG");
    if (shipLog.Length == 0)
    {
        sb.AppendLine("| No ship actions.");
    }
    else
    {
        var rawLines = shipLog.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var cleanLines = new List<string>();

        foreach (var raw in rawLines)
        {
            string l = raw.Trim();
            if (l.Length == 0) continue;
            if (l.Contains("ammo: fighter ammo check")) continue;
            if (l.Contains("service complete")) continue;
            if (cleanLines.Contains(l)) continue;
            cleanLines.Add(l);
        }

        if (cleanLines.Count == 0)
        {
            sb.AppendLine("| Service complete.");
        }
        else
        {
            int count = 0;
            foreach (var l in cleanLines)
            {
                if (count++ >= 7) break;
                sb.AppendLine("| " + TrimFit(l, 44));
            }
        }
    }

    shipLcd.WriteText(sb.ToString());
}



void UpdateUnknownLcd()
{
    if (unknownLcd == null) return;
    PrepareLcd(unknownLcd, 0.46f);

    var sb = new StringBuilder();
    Header(sb, "HIM UNKNOWN ITEMS");

    if (unknownItems.Count == 0)
    {
        Section(sb, "STATUS");
        sb.AppendLine("│ No unknown items found.");
        sb.AppendLine("│ All detected items have rules.");
    }
    else
    {
        Section(sb, "NEEDS RULES");
        foreach (var key in unknownItems.Keys.OrderBy(k => k))
            sb.AppendLine("│ " + TrimFit(key, 46));
    }

    unknownLcd.WriteText(sb.ToString());
}



// =======================================================================================
// MAINTENANCE v1.0
// =======================================================================================

void ManageBuildRepair()
{
    buildRepairDemand.Clear();
    buildRepairLog.Clear();

    var blocks = GetBuildRepairBlocks();
    if (blocks.Count == 0)
    {
        buildRepairLog.AppendLine("No maintenance blocks found.");
        return;
    }

    int count = 0;
    foreach (var block in blocks)
    {
        if (count++ >= MAX_BUILDREPAIR_BLOCKS) break;

        bool selfMaint = IsSelfMaintenanceBlock(block);
        buildRepairLog.AppendLine(CleanBuildRepairName(block.CustomName) + ": ONLINE");

        var demand = ReadBuildRepairDemand(block);

        // Always try to feed the unit from existing stock first.
        RefillBuildRepairInventory(block, demand);

        // Nanobot Build & Repair demand becomes extra HIM AutoCraft demand.
        // Self Maintenance Unit can command assemblers itself, so HIM does not double-order it.
        if (!selfMaint)
        {
            foreach (var kvp in demand)
            {
                if (!buildRepairDemand.ContainsKey(kvp.Key))
                    buildRepairDemand[kvp.Key] = 0;

                buildRepairDemand[kvp.Key] += kvp.Value;
            }
        }
    }

    if (buildRepairDemand.Count == 0)
        buildRepairLog.AppendLine("No HIM extra demand detected.");
}



bool IsBuildRepairManagedBlock(IMyTerminalBlock b)
{
    if (b == null) return false;
    if (!IsSameMainGrid(b) || !b.HasInventory || IsIgnored(b)) return false;

    string name = b.CustomName.ToUpper();
    return name.Contains(BUILDREPAIR_TAG) ||
           name.Contains(MAINTENANCE_TAG) ||
           name.Contains(SELFMAINT_TAG);
}

bool IsSelfMaintenanceBlock(IMyTerminalBlock b)
{
    if (b == null) return false;
    string name = b.CustomName.ToUpper();
    return name.Contains(MAINTENANCE_TAG) || name.Contains(SELFMAINT_TAG);
}

List<IMyTerminalBlock> GetBuildRepairBlocks()
{
    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => IsBuildRepairManagedBlock(b));

    return blocks.OrderBy(b => b.CustomName).ToList();
}


string CleanBuildRepairName(string name)
{
    if (string.IsNullOrWhiteSpace(name)) return "Maintenance";
    string n = name;
    n = n.Replace(BUILDREPAIR_TAG, "");
    n = n.Replace(MAINTENANCE_TAG, "");
    n = n.Replace(SELFMAINT_TAG, "");
    n = System.Text.RegularExpressions.Regex.Replace(n, @"\s+", " ").Trim();
    if (string.IsNullOrWhiteSpace(n)) n = "Maintenance";
    return n;
}


Dictionary<string, long> ReadBuildRepairDemand(IMyTerminalBlock block)
{
    var demand = new Dictionary<string, long>();

    // Many mods expose useful text through DetailedInfo.
    // If Nanobot Build & Repair exposes missing components there, HIM reads it.
    string info = "";
    try { info += block.DetailedInfo + "\n"; } catch {}
    try { info += block.CustomData + "\n"; } catch {}

    var knownItems = BuildAutoCraftDetectedItemList();

    foreach (var item in knownItems.Keys)
    {
        long amount = ParseDemandAmountForItem(info, item);
        if (amount > 0)
            demand[item] = amount;
    }

    return demand;
}

long ParseDemandAmountForItem(string info, string itemName)
{
    if (string.IsNullOrWhiteSpace(info) || string.IsNullOrWhiteSpace(itemName)) return 0;

    long total = 0;
    var lines = info.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (var raw in lines)
    {
        string line = raw.Trim();
        if (line.Length == 0) continue;

        if (line.IndexOf(itemName, StringComparison.OrdinalIgnoreCase) < 0)
            continue;

        // Pull the first useful number from the line.
        string num = "";
        bool reading = false;

        foreach (char c in line)
        {
            if ((c >= '0' && c <= '9') || c == ',' || c == '.')
            {
                num += c;
                reading = true;
            }
            else if (reading)
            {
                break;
            }
        }

        if (num.Length > 0)
        {
            num = num.Replace(",", "");
            double d;
            if (double.TryParse(num, out d))
                total += (long)Math.Ceiling(d);
        }
    }

    return total;
}

void RefillBuildRepairInventory(IMyTerminalBlock block, Dictionary<string, long> demand)
{
    if (block == null || block.InventoryCount == 0) return;

    var inv = block.GetInventory(0);
    var sources = FindStorage(COMPONENTS_TAG);

    if (sources.Count == 0) return;

    bool moved = false;
    var keys = demand.Keys.ToList();

    foreach (var itemName in keys)
    {
        long need = demand[itemName];

        long current = inv.GetItemAmount(MakeComponentId(itemName)).ToIntSafe();
        long moveNeed = Math.Max(0, need - current);

        if (moveNeed <= 0)
        {
            demand[itemName] = 0;
            continue;
        }

        // Pull from existing [COMPONENTS] stock first before AutoCraft handles leftovers.
        long movedAmount = PullNamedItemToInventory(itemName, sources, inv, moveNeed);
        if (movedAmount > 0)
        {
            moved = true;
            demand[itemName] = Math.Max(0, moveNeed - movedAmount);
        }
        else
        {
            demand[itemName] = moveNeed;
        }
    }

    var remove = demand.Where(k => k.Value <= 0).Select(k => k.Key).ToList();
    foreach (var key in remove)
        demand.Remove(key);

    if (moved)
        buildRepairLog.AppendLine("Components moved from stock.");
}

MyDefinitionId MakeComponentId(string subtype)
{
    MyDefinitionId id;
    if (!MyDefinitionId.TryParse("MyObjectBuilder_Component/" + subtype, out id))
        MyDefinitionId.TryParse("MyObjectBuilder_Component/SteelPlate", out id);

    return id;
}

long PullNamedItemToInventory(string subtype, List<IMyInventory> sources, IMyInventory dest, long amount)
{
    long moved = 0;
    if (dest == null || sources == null || amount <= 0) return 0;

    foreach (var source in sources)
    {
        var items = new List<MyInventoryItem>();
        source.GetItems(items);

        for (int i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            if (item.Type.SubtypeId.ToString() != subtype) continue;

            int available = item.Amount.ToIntSafe();
            int move = (int)Math.Min(available, amount - moved);
            if (move <= 0) continue;

            var fixedMove = (VRage.MyFixedPoint)move;
            if (!dest.CanItemsBeAdded(fixedMove, item.Type)) continue;

            if (source.TransferItemTo(dest, item, fixedMove))
                moved += move;

            if (moved >= amount) return moved;
        }
    }

    return moved;
}

void UpdateStatusLcd()
{
    if (statusLcd == null) return;
    PrepareLcd(statusLcd, 0.48f);

    var sb = new StringBuilder();
    Header(sb, "HIM STATUS");

    Section(sb, "MAINTENANCE");
    var blocks = GetBuildRepairBlocks();

    if (blocks.Count == 0)
    {
        sb.AppendLine("| No maintenance blocks found.");
    }
    else
    {
        int shown = 0;
        foreach (var b in blocks)
        {
            if (shown++ >= MAX_BUILDREPAIR_BLOCKS) break;
            sb.AppendLine("| " + TrimFit(CleanBuildRepairName(b.CustomName), 34) + " : ONLINE");
        }
    }

    Section(sb, "COMPONENTS NEEDED");
    if (buildRepairDemand.Count == 0)
    {
        sb.AppendLine("| No demand detected.");
    }
    else
    {
        int count = 0;
        foreach (var kvp in buildRepairDemand.OrderBy(k => k.Key))
        {
            if (count++ >= 16) break;
            sb.AppendLine("| " + TrimFit(kvp.Key, 34));
        }
    }

    Section(sb, "PERFORMANCE");
    sb.AppendLine("| Last sweep: " + TrimFit(lastSweepStage, 28));

    Section(sb, "NOTES");
    sb.AppendLine("| Needed parts are added above quotas.");
    sb.AppendLine("| Existing stock is used first.");

    statusLcd.WriteText(sb.ToString());
}

// =======================================================================================
// AUTOCRAFT v0.9
// =======================================================================================

void ManageAutoCraft()
{
    autoCraftLog.Clear();
    autoCraftJobs.Clear();
    autoCraftStatus.Clear();

    var panels = GetAutoCraftLcdPanels();
    if (panels.Count == 0) return;

    var mainPanel = panels[0];
    ParseAutoCraftCustomData(mainPanel);

    if (autoCraftQuotas.Count == 0)
    {
        autoCraftLog.AppendLine("No quotas set.");
        return;
    }

    var craftingAssemblers = GetAutoCraftAssemblers(false);
    var disassembler = GetDisassembler();
    var masterAssembler = GetAutoCraftMasterAssembler(craftingAssemblers);

    if (craftingAssemblers.Count == 0 || masterAssembler == null)
    {
        foreach (var q in autoCraftQuotas.Values)
            autoCraftStatus[q.Item] = "MISS";
        autoCraftLog.AppendLine("Missing [AUTOCRAFT] master assembler.");
        return;
    }

    ConfigureAssemblerCoop(craftingAssemblers, masterAssembler);

    var deficits = new Dictionary<string, decimal>();
    var surplus = new Dictionary<string, decimal>();

    foreach (var q in autoCraftQuotas.Values)
    {
        long current = GetTotal(q.Item);

        // Quota 0 means display/list only. Do not craft it from normal quota math.
        if (q.Quota <= 0)
        {
            autoCraftStatus[q.Item] = "FULL";
            if (q.AllowDisassemble && current > 0)
                surplus[q.Item] = current;
            continue;
        }

        MyDefinitionId bpId;
        decimal alreadyQueued = 0;

        if (TryGetCachedBlueprintId(q.Item, out bpId))
            alreadyQueued = GetQueuedAmountForBlueprint(craftingAssemblers, bpId);

        decimal effectiveStock = current + alreadyQueued;

        if (effectiveStock >= q.Quota)
        {
            if (alreadyQueued > 0 && current < q.Quota)
            {
                autoCraftStatus[q.Item] = "WORK";
                autoCraftJobs[q.Item] = alreadyQueued;
            }
            else
            {
                autoCraftStatus[q.Item] = "FULL";
            }

            if (q.AllowDisassemble && current > q.Quota)
                surplus[q.Item] = current - q.Quota;
        }
        else if (current < (long)Math.Floor(q.Quota * AUTOCRAFT_START_PERCENT))
        {
            decimal trueNeed = q.Quota - effectiveStock;

            if (trueNeed > 0)
            {
                deficits[q.Item] = trueNeed;
                autoCraftStatus[q.Item] = alreadyQueued > 0 ? "WORK" : "LOW";

                if (alreadyQueued > 0)
                    autoCraftJobs[q.Item] = alreadyQueued;
            }
        }
        else
        {
            autoCraftStatus[q.Item] = "FULL";
        }
    }

    // Build & Repair demand is extra and gets priority.
    // Self Maintenance Unit demand is not included here because that block can command assemblers itself.
    foreach (var kvp in buildRepairDemand)
    {
        MyDefinitionId bpId;
        decimal alreadyQueued = 0;

        if (TryGetCachedBlueprintId(kvp.Key, out bpId))
            alreadyQueued = GetQueuedAmountForBlueprint(craftingAssemblers, bpId);

        decimal trueNeed = Math.Max(0, kvp.Value - alreadyQueued);

        if (trueNeed > 0)
        {
            if (!deficits.ContainsKey(kvp.Key))
                deficits[kvp.Key] = 0;

            deficits[kvp.Key] += trueNeed;
        }

        autoCraftStatus[kvp.Key] = "WORK";
        if (alreadyQueued > 0)
            autoCraftJobs[kvp.Key] = alreadyQueued;
    }

    QueueCraftingJobs(craftingAssemblers, deficits);
    QueueDisassemblyJobs(disassembler, surplus);
}


List<IMyTextPanel> GetAutoCraftLcdPanels()
{
    var panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, p =>
        IsSameMainGrid(p) &&
        (
            p.CustomName.Contains(AUTOCRAFT_LCD_TAG) ||
            p.CustomName.Contains("[AUTOCRAFTLCD]") ||
            p.CustomName.Contains("[AutoCraftLCD2]") ||
            p.CustomName.Contains("[AUTOCRAFTLCD2]") ||
            p.CustomName.Contains("[AutoCraftLCD3]") ||
            p.CustomName.Contains("[AUTOCRAFTLCD3]") ||
            p.CustomName.Contains("[AutoCraftLCD4]") ||
            p.CustomName.Contains("[AUTOCRAFTLCD4]") ||
            p.CustomName.Contains("[AutoCraftLCD5]") ||
            p.CustomName.Contains("[AUTOCRAFTLCD5]")
        ));

    return panels.OrderBy(p => p.CustomName).ToList();
}


bool HasTagIgnoreCase(IMyTerminalBlock block, string tag)
{
    if (block == null || string.IsNullOrWhiteSpace(tag)) return false;
    return block.CustomName.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0;
}

bool IsAutoCraftAssemblerBlock(IMyAssembler asm, bool includeDisassembler)
{
    if (asm == null) return false;
    if (IsIgnored(asm)) return false;
    if (!IsSameMainGrid(asm) && !asm.IsSameConstructAs(Me)) return false;
    if (!HasTagIgnoreCase(asm, AUTOCRAFT_ASSEMBLER_TAG)) return false;

    bool isDisassembler = HasTagIgnoreCase(asm, DISASSEMBLER_TAG);
    if (!includeDisassembler && isDisassembler) return false;

    return true;
}

List<IMyAssembler> GetAutoCraftAssemblers(bool includeDisassembler)
{
    var assemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType(assemblers, a => IsAutoCraftAssemblerBlock(a, includeDisassembler));
    return assemblers.OrderBy(a => a.CustomName).ToList();
}


IMyAssembler GetDisassembler()
{
    var assemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType(assemblers, a =>
        IsAutoCraftAssemblerBlock(a, true) &&
        HasTagIgnoreCase(a, DISASSEMBLER_TAG));

    return assemblers.OrderBy(a => a.CustomName).FirstOrDefault();
}


void ParseAutoCraftCustomData(IMyTextPanel panel)
{
    autoCraftQuotas.Clear();
    if (panel == null) return;

    string data = panel.CustomData ?? "";
    var lines = data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (var raw in lines)
    {
        string line = raw.Trim();
        if (line.Length == 0 || line.StartsWith("#") || !line.Contains("=")) continue;

        int eq = line.IndexOf('=');
        string item = line.Substring(0, eq).Trim();
        string value = line.Substring(eq + 1).Trim().ToUpper();

        bool disassemble = false;
        if (value.StartsWith("D"))
        {
            disassemble = true;
            value = value.Substring(1).Trim();
        }

        int quota;
        if (!int.TryParse(value, out quota)) continue;
        if (quota < 0) quota = 0;
        if (string.IsNullOrWhiteSpace(item)) continue;

        autoCraftQuotas[item] = new AutoQuota { Item = item, Quota = quota, AllowDisassemble = disassemble };
    }
}

bool IsAssemblerBusyWithPlayerQueue(IMyAssembler asm)
{
    // HIM-owned assemblers are tagged [AUTOCRAFT].
    // Earlier versions skipped assemblers with any queue, which caused HIM to skip its own work.
    // For v1.0.9, tagged autocraft assemblers are allowed to keep receiving HIM jobs.
    if (asm != null && HasTagIgnoreCase(asm, AUTOCRAFT_ASSEMBLER_TAG))
        return false;

    var q = new List<MyProductionItem>();
    asm.GetQueue(q);
    return q.Count > 0;
}



IMyAssembler GetAutoCraftMasterAssembler(List<IMyAssembler> assemblers)
{
    if (assemblers == null || assemblers.Count == 0) return null;

    return assemblers
        .Where(a => a != null && !HasTagIgnoreCase(a, DISASSEMBLER_TAG))
        .OrderBy(a => a.CustomName)
        .FirstOrDefault();
}

void ConfigureAssemblerCoop(List<IMyAssembler> craftingAssemblers, IMyAssembler master)
{
    if (craftingAssemblers == null || master == null) return;

    foreach (var asm in craftingAssemblers)
    {
        if (asm == null) continue;
        asm.CooperativeMode = asm.EntityId != master.EntityId;
    }
}

decimal GetQueuedAmountForBlueprint(List<IMyAssembler> assemblers, MyDefinitionId bpId)
{
    decimal total = 0;
    var queue = new List<MyProductionItem>();
    string target = bpId.ToString();
    string targetSubtype = bpId.SubtypeId.ToString();

    foreach (var asm in assemblers)
    {
        queue.Clear();
        asm.GetQueue(queue);

        foreach (var q in queue)
        {
            string qFull = q.BlueprintId.ToString();
            string qSubtype = q.BlueprintId.SubtypeId.ToString();

            if (qFull == target || qSubtype == targetSubtype)
                total += (decimal)q.Amount;
        }
    }

    return total;
}


decimal GetQueuedAmountForBlueprint(IMyAssembler asm, MyDefinitionId bpId)
{
    if (asm == null) return 0;

    decimal total = 0;
    var queue = new List<MyProductionItem>();
    string target = bpId.ToString();
    string targetSubtype = bpId.SubtypeId.ToString();

    asm.GetQueue(queue);

    foreach (var q in queue)
    {
        string qFull = q.BlueprintId.ToString();
        string qSubtype = q.BlueprintId.SubtypeId.ToString();

        if (qFull == target || qSubtype == targetSubtype)
            total += (decimal)q.Amount;
    }

    return total;
}


void QueueCraftingJobs(List<IMyAssembler> assemblers, Dictionary<string, decimal> deficits)
{
    if (deficits.Count == 0) return;

    var ordered = deficits
        .OrderByDescending(kvp => autoCraftQuotas.ContainsKey(kvp.Key) && autoCraftQuotas[kvp.Key].Quota > 0 ? (double)kvp.Value / autoCraftQuotas[kvp.Key].Quota : 999)
        .ThenBy(kvp => kvp.Key)
        .ToList();

    if (assemblers.Count == 0)
    {
        autoCraftLog.AppendLine("No autocraft assemblers.");
        return;
    }

    var master = GetAutoCraftMasterAssembler(assemblers);
    if (master == null)
    {
        autoCraftLog.AppendLine("No autocraft master assembler.");
        return;
    }

    ConfigureAssemblerCoop(assemblers, master);

    foreach (var job in ordered)
    {
        string item = job.Key;
        decimal amountNeeded = Math.Ceiling(job.Value);
        if (amountNeeded <= 0) continue;

        MyDefinitionId bpId;
        if (!TryGetCachedBlueprintId(item, out bpId))
        {
            autoCraftStatus[item] = "MISS";
            autoCraftLog.AppendLine("BP invalid: " + item);
            continue;
        }

        if (!master.CanUseBlueprint(bpId))
        {
            autoCraftStatus[item] = "MISS";
            autoCraftLog.AppendLine("Master cannot craft " + item);
            continue;
        }

        decimal alreadyQueued = GetQueuedAmountForBlueprint(assemblers, bpId);
        if (alreadyQueued > 0)
        {
            autoCraftStatus[item] = "WORK";
            autoCraftJobs[item] = alreadyQueued;
            continue;
        }

        try
        {
            if (master.Mode != MyAssemblerMode.Assembly)
                master.Mode = MyAssemblerMode.Assembly;

            master.AddQueueItem(bpId, amountNeeded);
            autoCraftStatus[item] = "WORK";
            autoCraftJobs[item] = amountNeeded;
            autoCraftLog.AppendLine("Craft " + item + " +" + FormatNum((double)amountNeeded));
        }
        catch
        {
            autoCraftStatus[item] = "MISS";
            autoCraftLog.AppendLine("Queue failed: " + item);
        }
    }
}



void QueueDisassemblyJobs(IMyAssembler disassembler, Dictionary<string, decimal> surplus)
{
    if (surplus.Count == 0) return;

    if (disassembler == null)
    {
        foreach (var s in surplus.Keys) autoCraftStatus[s] = "MISS";
        autoCraftLog.AppendLine("Missing [DISASSEMBLER].");
        return;
    }

    foreach (var job in surplus.OrderByDescending(k => k.Value))
    {
        string item = job.Key;
        decimal amount = Math.Ceiling(job.Value);
        if (amount <= 0) continue;

        MyDefinitionId bpId;
        if (!TryGetCachedBlueprintId(item, out bpId))
        {
            autoCraftStatus[item] = "MISS";
            continue;
        }

        if (!disassembler.CanUseBlueprint(bpId))
        {
            autoCraftStatus[item] = "MISS";
            continue;
        }

        decimal alreadyQueued = GetQueuedAmountForBlueprint(disassembler, bpId);
        if (alreadyQueued > 0)
        {
            autoCraftStatus[item] = "DUMP";
            autoCraftJobs[item] = -alreadyQueued;
            autoCraftLog.AppendLine("Waiting disasm " + item + " -" + FormatNum((double)alreadyQueued));
            continue;
        }

        long moved = MoveItemToDisassembler(item, (long)amount, disassembler.InputInventory);
        if (moved <= 0)
        {
            autoCraftStatus[item] = "MISS";
            continue;
        }

        try
        {
            disassembler.Mode = MyAssemblerMode.Disassembly;
            disassembler.AddQueueItem(bpId, (decimal)moved);
            autoCraftStatus[item] = "DUMP";
            autoCraftJobs[item] = -moved;
            autoCraftLog.AppendLine("Disassemble " + item + " -" + FormatNum(moved));
        }
        catch
        {
            autoCraftStatus[item] = "MISS";
        }

        break;
    }
}



long MoveItemToDisassembler(string itemName, long amount, IMyInventory target)
{
    long moved = 0;
    var sourceTags = new List<string> { COMPONENTS_TAG, AMMO_TAG, BOTTLES_TAG, TOOLS_TAG };

    foreach (var tag in sourceTags)
    {
        var sources = FindStorage(tag);
        foreach (var source in sources)
        {
            var items = new List<MyInventoryItem>();
            source.GetItems(items);

            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                if (item.Type.SubtypeId.ToString() != itemName) continue;

                int available = item.Amount.ToIntSafe();
                int move = (int)Math.Min(available, amount - moved);
                if (move <= 0) continue;

                var fixedMove = (VRage.MyFixedPoint)move;
                if (!target.CanItemsBeAdded(fixedMove, item.Type)) continue;

                if (source.TransferItemTo(target, item, fixedMove)) moved += move;
                if (moved >= amount) return moved;
            }
        }
    }
    return moved;
}

// Resolves item name -> blueprint MyDefinitionId using the cache.
// Returns false if no valid blueprint could be resolved (cached as a miss).
// This replaces calling GetBlueprintSubtype() + MyDefinitionId.TryParse() fresh every cycle.
bool TryGetCachedBlueprintId(string item, out MyDefinitionId bpId)
{
    if (blueprintIdCache.TryGetValue(item, out bpId))
        return true;

    if (blueprintIdMisses.Contains(item))
        return false;

    string bp = GetBlueprintSubtype(item);

    if (MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + bp, out bpId))
    {
        blueprintIdCache[item] = bpId;
        return true;
    }

    blueprintIdMisses.Add(item);
    return false;
}

string GetBlueprintSubtype(string item)
{
    switch (item)
    {
        case "Computer":
        case "Construction":
        case "Motor":
        case "Thrust":
        case "Explosives":
        case "Girder":
        case "Detector":
            return item + "Component";
        case "RadioComponent":
            return "RadioCommunicationComponent";
        case "GravityGenerator":
            return "GravityGeneratorComponent";
        case "Medical":
            return "MedicalComponent";
        case "Reactor":
            return "ReactorComponent";
        default:
            return item;
    }
}


void RefreshAutoCraftCustomDataFromInventory()
{
    var panels = GetAutoCraftLcdPanels();
    if (panels.Count == 0) return;

    var existing = new Dictionary<string, string>();
    string oldData = panels[0].CustomData ?? "";
    var lines = oldData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (var raw in lines)
    {
        string line = raw.Trim();
        if (line.Length == 0 || line.StartsWith("#") || !line.Contains("=")) continue;

        int eq = line.IndexOf('=');
        string item = line.Substring(0, eq).Trim();
        string val = line.Substring(eq + 1).Trim();

        if (!string.IsNullOrWhiteSpace(item) && !existing.ContainsKey(item))
            existing[item] = val;
    }

    var items = BuildAutoCraftDetectedItemList();

    foreach (var kvp in existing)
        if (!items.ContainsKey(kvp.Key))
            items[kvp.Key] = kvp.Value;

    var sb = new StringBuilder();
    sb.AppendLine("# HIM AutoCraft quotas");
    sb.AppendLine("# Format:");
    sb.AppendLine("# ItemName=Quota");
    sb.AppendLine("# ItemName=DQuota   allows disassembly above quota");
    sb.AppendLine("#");
    sb.AppendLine("# Command: run PB with autocraft_refresh to rebuild this list");
    sb.AppendLine("# Existing quota values are kept when possible.");
    sb.AppendLine("#");

    foreach (var item in items.Keys.OrderBy(x => x))
    {
        string val = "0";
        if (existing.ContainsKey(item)) val = existing[item];
        sb.AppendLine(item + "=" + val);
    }

    panels[0].CustomData = sb.ToString();
}

Dictionary<string, string> BuildAutoCraftDetectedItemList()
{
    var items = new Dictionary<string, string>();

    AddVanillaAutoCraftItems(items);
    AddDetectedInventoryItems(items);

    return items;
}

void AddAutoCraftItem(Dictionary<string, string> items, string item)
{
    if (string.IsNullOrWhiteSpace(item)) return;
    if (!items.ContainsKey(item))
        items[item] = "0";
}

void AddVanillaAutoCraftItems(Dictionary<string, string> items)
{
    AddAutoCraftItem(items, "BulletproofGlass");
    AddAutoCraftItem(items, "Canvas");
    AddAutoCraftItem(items, "Computer");
    AddAutoCraftItem(items, "Construction");
    AddAutoCraftItem(items, "Detector");
    AddAutoCraftItem(items, "Display");
    AddAutoCraftItem(items, "Explosives");
    AddAutoCraftItem(items, "Girder");
    AddAutoCraftItem(items, "GravityGenerator");
    AddAutoCraftItem(items, "InteriorPlate");
    AddAutoCraftItem(items, "LargeTube");
    AddAutoCraftItem(items, "Medical");
    AddAutoCraftItem(items, "MetalGrid");
    AddAutoCraftItem(items, "Motor");
    AddAutoCraftItem(items, "PowerCell");
    AddAutoCraftItem(items, "RadioComponent");
    AddAutoCraftItem(items, "Reactor");
    AddAutoCraftItem(items, "SmallTube");
    AddAutoCraftItem(items, "SolarCell");
    AddAutoCraftItem(items, "SteelPlate");
    AddAutoCraftItem(items, "Superconductor");
    AddAutoCraftItem(items, "Thrust");

    AddAutoCraftItem(items, "NATO_5p56x45mm");
    AddAutoCraftItem(items, "NATO_25x184mm");
    AddAutoCraftItem(items, "Missile200mm");
    AddAutoCraftItem(items, "LargeCalibreAmmo");
    AddAutoCraftItem(items, "MediumCalibreAmmo");
    AddAutoCraftItem(items, "AutocannonClip");
    AddAutoCraftItem(items, "SemiAutoPistolMagazine");
    AddAutoCraftItem(items, "ElitePistolMagazine");
    AddAutoCraftItem(items, "FullAutoPistolMagazine");
    AddAutoCraftItem(items, "AutomaticRifleGun_Mag_20rd");
    AddAutoCraftItem(items, "PreciseAutomaticRifleGun_Mag_5rd");
    AddAutoCraftItem(items, "RapidFireAutomaticRifleGun_Mag_50rd");
    AddAutoCraftItem(items, "UltimateAutomaticRifleGun_Mag_30rd");

    AddAutoCraftItem(items, "HydrogenBottle");
    AddAutoCraftItem(items, "OxygenBottle");
    AddAutoCraftItem(items, "WelderItem");
    AddAutoCraftItem(items, "AngleGrinderItem");
    AddAutoCraftItem(items, "HandDrillItem");
}

void AddDetectedInventoryItems(Dictionary<string, string> items)
{
    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b =>
        b.HasInventory &&
        IsSameMainGrid(b) &&
        !IsIgnored(b));

    var invItems = new List<MyInventoryItem>();

    foreach (var block in blocks)
    {
        for (int i = 0; i < block.InventoryCount; i++)
        {
            var inv = block.GetInventory(i);
            invItems.Clear();
            inv.GetItems(invItems);

            foreach (var item in invItems)
            {
                string typeId = item.Type.TypeId.ToString();
                string subtype = item.Type.SubtypeId.ToString();

                if (typeId.EndsWith("_Component") ||
                    typeId.EndsWith("_AmmoMagazine") ||
                    typeId.EndsWith("_PhysicalGunObject") ||
                    typeId.EndsWith("_GasContainerObject") ||
                    typeId.EndsWith("_OxygenContainerObject"))
                {
                    AddAutoCraftItem(items, subtype);
                }
            }
        }
    }
}

void UpdateAutoCraftPlaceholder()
{
    var panels = GetAutoCraftLcdPanels();
    if (panels.Count == 0) return;

    if (panels[0].CustomData.Trim().Length == 0)
    {
        RefreshAutoCraftCustomDataFromInventory();
    }

    var rows = new List<string>();
    rows.Add("ITEM".PadRight(19) + "STOCK".PadLeft(8) + "  " + "QUOTA".PadLeft(8) + "  STATUS");
    rows.Add(new string('-', 49));

    foreach (var q in autoCraftQuotas.Values.OrderBy(q => q.Item))
    {
        long current = GetTotal(q.Item);
        string quota = (q.AllowDisassemble ? "D" : "") + FormatNum(q.Quota);
        string status = autoCraftStatus.ContainsKey(q.Item) ? autoCraftStatus[q.Item] : "FULL";

        rows.Add(
            TrimFit(q.Item, 19).PadRight(19) +
            FormatNum(current).PadLeft(8) + "  " +
            quota.PadLeft(8) + "  " +
            status
        );
    }

    if (autoCraftQuotas.Count == 0)
    {
        rows.Add("No quotas set.");
        rows.Add("Edit Custom Data on main LCD.");
    }

    var active = new List<string>();
    foreach (var job in autoCraftJobs)
    {
        string prefix = job.Value >= 0 ? "+" : "";
        active.Add(TrimFit(job.Key, 20).PadRight(20) + prefix + FormatNum((double)job.Value));
    }

    int totalPages = Math.Max(1, panels.Count);
    int lastPage = panels.Count - 1;

    for (int p = 0; p < panels.Count; p++)
    {
        PrepareLcd(panels[p], 0.42f);
        var sb = new StringBuilder();

        Header(sb, "HIM AUTOCRAFT " + (p + 1) + "/" + panels.Count);

        int start = p * AUTOCRAFT_LINES_PER_LCD;
        int end = Math.Min(rows.Count, start + AUTOCRAFT_LINES_PER_LCD);

        Section(sb, "STOCK / QUOTA");

        if (start >= rows.Count)
        {
            sb.AppendLine("| No more quota items.");
        }
        else
        {
            for (int i = start; i < end; i++)
                sb.AppendLine("| " + rows[i]);
        }

        // Keep page 1 and middle pages clean. Put system info only on the last page.
        if (p == lastPage)
        {
            Section(sb, "ACTIVE JOBS");
            if (active.Count == 0)
            {
                sb.AppendLine("| None");
            }
            else
            {
                int count = 0;
                foreach (var a in active)
                {
                    if (count++ >= 8) break;
                    sb.AppendLine("| " + a);
                }
            }

            Section(sb, "ASSEMBLERS");
            sb.AppendLine("| Crafting     : " + GetAutoCraftAssemblers(false).Count);
            sb.AppendLine("| Disassembler : " + (GetDisassembler() != null ? "1" : "0"));
            sb.AppendLine("| Queue mode   : Full deficit");
            sb.AppendLine("| Start below  : " + (AUTOCRAFT_START_PERCENT * 100).ToString("0") + "%");
        }

        panels[p].WriteText(sb.ToString());
    }
}