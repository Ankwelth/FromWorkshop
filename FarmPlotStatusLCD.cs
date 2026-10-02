// Farm Plot Status LCD Script v10.27 Tight Sprite Spacing
//
// Farm plots group/tag: [Farm Blocks]
// LCD name/tag:         [Farm LCD]
// Irrigation/water tag: [Farm Water]
//
// This version keeps the v10.7 sprite look, text sizes, and grey outline,
// but adds the dedicated-server sprite sync workaround used by other LCD scripts.
//
// Commands:
// setup     = write default Custom Data
// reload    = reload Custom Data and refresh blocks
// refresh   = refresh block list
// debug     = show persistent debug info
// normal    = return to normal color status mode
// harvested = manually mark current ready state as cleared/empty
// unharvest = clear manual harvested override
// forceon   = force all farm plots on
// lcdtest   = show a sprite viewport test on every found LCD

const int SETTINGS_VERSION = 27;

MyIni ini = new MyIni();

string FARM_TAG = "[Farm Blocks]";
string LCD_TAG = "[Farm LCD]";
string WATER_TAG = "[Farm Water]";

bool SAME_CONSTRUCT_ONLY = true;
bool REQUIRE_ALL_PLOTS_PLANTED_FOR_GREEN = false;
bool UNKNOWN_COUNTS_AS_GROWING = false;
bool SHOW_TEXT_ON_LCD = true;
bool AUTOFIT_TEXT = true;
bool USE_NATIVE_LCD_BACKGROUND = false;
bool USE_BACKGROUND_SPRITE = true;
bool CLEAR_LCD_SCRIPT_SELECTION = true;
bool ZERO_PERCENT_COUNTS_AS_GROWING = false;
bool SHOW_WATER_LINE = true;

bool AUTO_TOGGLE_REFRESH = true;
int TOGGLE_EVERY_TICKS = 18;
int TOGGLE_OFF_TICKS = 1;
bool ENABLE_FARMS_ON_START = true;
bool HOLD_DISPLAY_DURING_REFRESH = true;

float TEXT_SCALE = 2.0f;

bool DEBUG_MODE = false;
bool LCD_TEST_MODE = false;
bool TOGGLE_ACTIVE = false;
int toggleTimer = 0;
int offTimer = 0;

Color COLOR_EMPTY = new Color(255, 0, 0);
Color COLOR_GROWING = new Color(0, 80, 255);
Color COLOR_READY = new Color(0, 255, 0);
Color COLOR_TEXT = new Color(255, 255, 255);
Color COLOR_TEXT_OUTLINE = new Color(55, 55, 55);

bool USE_TEXT_OUTLINE = true;
float TEXT_OUTLINE_OFFSET = 1.0f;
int OUTLINE_MODE = 8; // 8 = old full outline, 4 = lighter server outline
bool USE_WIDE_LAYOUT = false;
float WIDE_LAYOUT_ASPECT = 1.65f;
float STACKED_LINE_SPACING = 0.82f;
float STACKED_VERTICAL_OFFSET = 0.0f;
bool SPRITE_CACHE_BUSTER = true;
bool AVOID_SCRIPT_COLOR_WRITES = true;

int renderFrame = 0;

System.Text.StringBuilder measureBuilder = new System.Text.StringBuilder();

List<IMyTerminalBlock> farmBlocks = new List<IMyTerminalBlock>();
List<IMyTextPanel> lcds = new List<IMyTextPanel>();
List<IMyTerminalBlock> waterBlocks = new List<IMyTerminalBlock>();
List<IMyBlockGroup> groups = new List<IMyBlockGroup>();

List<string> EMPTY_WORDS = new List<string>();
List<string> GROWING_WORDS = new List<string>();
List<string> READY_WORDS = new List<string>();
List<string> DEAD_WORDS = new List<string>();

Dictionary<long, double> lastGrowthById = new Dictionary<long, double>();
Dictionary<long, string> lastCropById = new Dictionary<long, string>();
Dictionary<long, PlotState> lastStateById = new Dictionary<long, PlotState>();
List<long> harvestedOverrideIds = new List<long>();

bool haveLastDisplay = false;
Color lastStatusColor = new Color(255, 0, 0);
string lastStatusText = "NO DATA";
int lastTotal = 0;
int lastEmpty = 0;
int lastGrowing = 0;
int lastReady = 0;
int lastDead = 0;
int lastUnknown = 0;
string lastWaterText = "Water: N/A";

struct FarmInfo
{
    public bool HasLogic;
    public bool Planted;
    public bool Alive;
    public bool Ready;
    public string Crop;
    public int OutputAmount;
    public int SeedsRequired;
    public double WaterRatio;
}

enum PlotState
{
    Empty,
    Growing,
    Ready,
    Dead,
    Unknown
}

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;

    LoadStorage();
    EnsureCustomData(false);
    LoadSettings();
    FindBlocks();

    if (ENABLE_FARMS_ON_START)
        SetFarmBlocksEnabled(true);
}

public void Main(string argument, UpdateType updateSource)
{
    argument = argument.Trim().ToLower();

    renderFrame++;
    if (renderFrame > 1000000)
        renderFrame = 0;

    if (argument == "setup")
    {
        EnsureCustomData(true);
        LoadSettings();
        FindBlocks();
        DEBUG_MODE = false;
        LCD_TEST_MODE = false;

        if (ENABLE_FARMS_ON_START)
            SetFarmBlocksEnabled(true);

        Echo("Default Custom Data written.");
        return;
    }

    if (argument == "reload")
    {
        EnsureCustomData(false);
        LoadSettings();
        FindBlocks();
        Echo("Settings reloaded.");
        return;
    }

    if (argument == "refresh")
    {
        FindBlocks();
        Echo("Blocks refreshed.");
        return;
    }

    if (argument == "forceon")
    {
        TOGGLE_ACTIVE = false;
        offTimer = 0;
        SetFarmBlocksEnabled(true);
        Echo("Farm blocks forced on.");
        return;
    }

    if (argument == "debug")
    {
        DEBUG_MODE = true;
        LCD_TEST_MODE = false;
        DebugFarmBlocks();
        return;
    }

    if (argument == "lcdtest")
    {
        DEBUG_MODE = false;
        LCD_TEST_MODE = true;
        FindBlocks();
        PaintLCDTest();
        Echo("LCD sprite test mode enabled.");
        Echo("Run argument 'normal' to exit.");
        return;
    }

    if (argument == "normal")
    {
        DEBUG_MODE = false;
        LCD_TEST_MODE = false;
        Echo("Returned to normal mode.");
        return;
    }

    if (argument == "harvested" || argument == "clear")
    {
        MarkCurrentReadyPlotsAsHarvested();
        DEBUG_MODE = false;
        Echo("Harvested override enabled for ready plots.");
        return;
    }

    if (argument == "unharvest" || argument == "unclear")
    {
        harvestedOverrideIds.Clear();
        SaveStorage();
        Echo("Harvested override cleared.");
        return;
    }

    if (DEBUG_MODE)
    {
        DebugFarmBlocks();
        return;
    }

    if (LCD_TEST_MODE)
    {
        PaintLCDTest();
        return;
    }

    bool currentlyRefreshing = HandleAutoToggleRefresh();

    if (currentlyRefreshing && HOLD_DISPLAY_DURING_REFRESH && haveLastDisplay)
    {
        PaintAllLCDs(lastStatusColor, lastStatusText, lastTotal, lastEmpty, lastGrowing, lastReady, lastDead, lastUnknown, lastWaterText);
        EchoStatus(true);
        return;
    }

    int empty = 0;
    int growing = 0;
    int ready = 0;
    int dead = 0;
    int unknown = 0;

    for (int i = 0; i < farmBlocks.Count; i++)
    {
        PlotState state = GetPlotState(farmBlocks[i]);

        if (state == PlotState.Empty)
            empty++;
        else if (state == PlotState.Growing)
            growing++;
        else if (state == PlotState.Ready)
            ready++;
        else if (state == PlotState.Dead)
            dead++;
        else
            unknown++;
    }

    SaveStorage();

    int total = farmBlocks.Count;
    int cropCount = growing + ready + dead;

    string statusText = "";
    Color statusColor = COLOR_EMPTY;

    if (total == 0)
    {
        statusText = "NO FARM BLOCKS FOUND";
        statusColor = COLOR_EMPTY;
    }
    else if (dead > 0)
    {
        statusText = "CROP PROBLEM";
        statusColor = COLOR_EMPTY;
    }
    else if (cropCount == 0)
    {
        statusText = "NO CROPS";
        statusColor = COLOR_EMPTY;
    }
    else
    {
        bool allKnownCropsReady = growing == 0 && dead == 0 && unknown == 0;
        bool allPlotsFilled = empty == 0;

        if (ready > 0 && allKnownCropsReady && (!REQUIRE_ALL_PLOTS_PLANTED_FOR_GREEN || allPlotsFilled))
        {
            statusText = "FULLY GROWN";
            statusColor = COLOR_READY;
        }
        else
        {
            statusText = "GROWING";
            statusColor = COLOR_GROWING;
        }
    }

    string waterText = GetWaterStatusText();

    SaveLastDisplay(statusColor, statusText, total, empty, growing, ready, dead, unknown, waterText);
    PaintAllLCDs(statusColor, statusText, total, empty, growing, ready, dead, unknown, waterText);
    EchoStatus(currentlyRefreshing);
}

void SaveLastDisplay(Color color, string status, int total, int empty, int growing, int ready, int dead, int unknown, string waterText)
{
    haveLastDisplay = true;
    lastStatusColor = color;
    lastStatusText = status;
    lastTotal = total;
    lastEmpty = empty;
    lastGrowing = growing;
    lastReady = ready;
    lastDead = dead;
    lastUnknown = unknown;
    lastWaterText = waterText;
}

void PaintAllLCDs(Color color, string status, int total, int empty, int growing, int ready, int dead, int unknown, string waterText)
{
    for (int i = 0; i < lcds.Count; i++)
        PaintLCD(lcds[i], color, status, total, empty, growing, ready, dead, unknown, waterText);
}

void EchoStatus(bool currentlyRefreshing)
{
    Echo("Farm LCD Status v10.27");
    Echo("");
    Echo("Farm blocks: " + farmBlocks.Count);
    Echo("LCDs: " + lcds.Count);
    Echo("Water blocks: " + waterBlocks.Count);
    Echo("");
    Echo("Empty: " + lastEmpty);
    Echo("Growing: " + lastGrowing);
    Echo("Ready: " + lastReady);
    Echo("Dead: " + lastDead);
    Echo("Unknown: " + lastUnknown);
    Echo("HarvestedOverrides: " + harvestedOverrideIds.Count);
    Echo("");
    Echo("Water: " + lastWaterText);
    Echo("AutoToggleRefresh: " + AUTO_TOGGLE_REFRESH);
    Echo("Refreshing Now: " + currentlyRefreshing);
    Echo("Status: " + lastStatusText);
    Echo("Pure sprite renderer: true");
    Echo("");
    Echo("Commands:");
    Echo("setup, reload, refresh, debug, normal");
    Echo("harvested, unharvest, forceon, lcdtest");
}

string GetWaterStatusText()
{
    if (!SHOW_WATER_LINE)
        return "";

    if (waterBlocks.Count == 0)
        return "Water: N/A";

    double current = 0.0;
    double max = 0.0;

    for (int i = 0; i < waterBlocks.Count; i++)
    {
        IMyTerminalBlock block = waterBlocks[i];

        if (block == null)
            continue;

        int inventoryCount = block.InventoryCount;

        for (int invIndex = 0; invIndex < inventoryCount; invIndex++)
        {
            IMyInventory inv = block.GetInventory(invIndex);

            if (inv == null)
                continue;

            current += (double)(float)inv.CurrentVolume;
            max += (double)(float)inv.MaxVolume;
        }
    }

    if (max <= 0.0)
        return "Water: N/A";

    double percent = current / max * 100.0;

    if (percent < 0.0)
        percent = 0.0;

    if (percent > 100.0)
        percent = 100.0;

    return "Water: " + percent.ToString("0") + "%";
}

bool HandleAutoToggleRefresh()
{
    if (!AUTO_TOGGLE_REFRESH)
        return false;

    if (farmBlocks.Count == 0)
        return false;

    if (TOGGLE_EVERY_TICKS < 2)
        TOGGLE_EVERY_TICKS = 2;

    if (TOGGLE_OFF_TICKS < 1)
        TOGGLE_OFF_TICKS = 1;

    if (TOGGLE_ACTIVE)
    {
        offTimer--;

        if (offTimer <= 0)
        {
            SetFarmBlocksEnabled(true);
            TOGGLE_ACTIVE = false;
            return false;
        }

        return true;
    }

    toggleTimer++;

    if (toggleTimer >= TOGGLE_EVERY_TICKS)
    {
        toggleTimer = 0;
        offTimer = TOGGLE_OFF_TICKS;
        TOGGLE_ACTIVE = true;
        SetFarmBlocksEnabled(false);
        return true;
    }

    return false;
}

void SetFarmBlocksEnabled(bool enabled)
{
    for (int i = 0; i < farmBlocks.Count; i++)
    {
        IMyFunctionalBlock block = farmBlocks[i] as IMyFunctionalBlock;

        if (block != null)
        {
            block.Enabled = enabled;

            if (enabled)
                block.ApplyAction("OnOff_On");
            else
                block.ApplyAction("OnOff_Off");
        }
    }
}

void EnsureCustomData(bool forceRewrite)
{
    if (forceRewrite || string.IsNullOrWhiteSpace(Me.CustomData))
    {
        Me.CustomData = DefaultCustomData();
        return;
    }

    MyIniParseResult result;

    if (!ini.TryParse(Me.CustomData, out result))
        return;

    int version = ini.Get("Script", "Version").ToInt32(0);

    if (version != SETTINGS_VERSION)
        Me.CustomData = DefaultCustomData();
}

void LoadSettings()
{
    MyIniParseResult result;

    if (!ini.TryParse(Me.CustomData, out result))
    {
        Echo("Custom Data parse error:");
        Echo(result.ToString());
        Echo("Run with argument: setup");
        return;
    }

    FARM_TAG = ini.Get("Tags", "FarmBlocks").ToString("[Farm Blocks]");
    LCD_TAG = ini.Get("Tags", "FarmLCD").ToString("[Farm LCD]");
    WATER_TAG = ini.Get("Tags", "FarmWater").ToString("[Farm Water]");

    SAME_CONSTRUCT_ONLY = ini.Get("Options", "SameConstructOnly").ToBoolean(true);
    REQUIRE_ALL_PLOTS_PLANTED_FOR_GREEN = ini.Get("Options", "RequireAllPlotsPlantedForGreen").ToBoolean(false);
    UNKNOWN_COUNTS_AS_GROWING = ini.Get("Options", "UnknownCountsAsGrowing").ToBoolean(false);
    SHOW_TEXT_ON_LCD = ini.Get("Options", "ShowTextOnLCD").ToBoolean(true);
    AUTOFIT_TEXT = ini.Get("Options", "AutoFitText").ToBoolean(true);
    USE_NATIVE_LCD_BACKGROUND = ini.Get("Options", "UseNativeLCDBackground").ToBoolean(true);
    USE_BACKGROUND_SPRITE = ini.Get("Options", "UseBackgroundSprite").ToBoolean(true);
    CLEAR_LCD_SCRIPT_SELECTION = ini.Get("Options", "ClearLCDScriptSelection").ToBoolean(true);
    USE_TEXT_OUTLINE = ini.Get("Options", "TextOutline").ToBoolean(true);
    TEXT_OUTLINE_OFFSET = (float)ini.Get("Options", "TextOutlineOffset").ToDouble(1.0);
    ZERO_PERCENT_COUNTS_AS_GROWING = ini.Get("Options", "ZeroPercentCountsAsGrowing").ToBoolean(false);
    SHOW_WATER_LINE = ini.Get("Options", "ShowWaterLine").ToBoolean(true);
    OUTLINE_MODE = ini.Get("Options", "OutlineMode").ToInt32(8);
    USE_WIDE_LAYOUT = ini.Get("Options", "UseWideLayout").ToBoolean(true);
    WIDE_LAYOUT_ASPECT = (float)ini.Get("Options", "WideLayoutAspect").ToDouble(1.65);
    STACKED_LINE_SPACING = (float)ini.Get("Options", "StackedLineSpacing").ToDouble(0.82);
    STACKED_VERTICAL_OFFSET = (float)ini.Get("Options", "StackedVerticalOffset").ToDouble(0.0);
    SPRITE_CACHE_BUSTER = ini.Get("Options", "SpriteCacheBuster").ToBoolean(true);
    AVOID_SCRIPT_COLOR_WRITES = ini.Get("Options", "AvoidScriptColorWrites").ToBoolean(true);

    TEXT_SCALE = (float)ini.Get("Options", "TextScale").ToDouble(2.0);

    AUTO_TOGGLE_REFRESH = ini.Get("Refresh", "AutoToggleFarmPlots").ToBoolean(true);
    TOGGLE_EVERY_TICKS = ini.Get("Refresh", "ToggleEveryTicks").ToInt32(18);
    TOGGLE_OFF_TICKS = ini.Get("Refresh", "ToggleOffTicks").ToInt32(1);
    ENABLE_FARMS_ON_START = ini.Get("Refresh", "EnableFarmsOnStart").ToBoolean(true);
    HOLD_DISPLAY_DURING_REFRESH = ini.Get("Refresh", "HoldDisplayDuringRefresh").ToBoolean(true);

    if (TEXT_SCALE < 0.5f)
        TEXT_SCALE = 0.5f;

    if (TEXT_SCALE > 5.0f)
        TEXT_SCALE = 5.0f;

    if (TEXT_OUTLINE_OFFSET < 0.0f)
        TEXT_OUTLINE_OFFSET = 0.0f;

    if (TEXT_OUTLINE_OFFSET > 3.0f)
        TEXT_OUTLINE_OFFSET = 3.0f;

    if (OUTLINE_MODE != 4 && OUTLINE_MODE != 8)
        OUTLINE_MODE = 8;

    if (WIDE_LAYOUT_ASPECT < 1.20f)
        WIDE_LAYOUT_ASPECT = 1.20f;

    if (WIDE_LAYOUT_ASPECT > 4.00f)
        WIDE_LAYOUT_ASPECT = 4.00f;

    if (STACKED_LINE_SPACING < 0.55f)
        STACKED_LINE_SPACING = 0.55f;

    if (STACKED_LINE_SPACING > 1.20f)
        STACKED_LINE_SPACING = 1.20f;

    if (STACKED_VERTICAL_OFFSET < -0.25f)
        STACKED_VERTICAL_OFFSET = -0.25f;

    if (STACKED_VERTICAL_OFFSET > 0.25f)
        STACKED_VERTICAL_OFFSET = 0.25f;

    if (TOGGLE_EVERY_TICKS < 2)
        TOGGLE_EVERY_TICKS = 2;

    if (TOGGLE_EVERY_TICKS > 600)
        TOGGLE_EVERY_TICKS = 600;

    if (TOGGLE_OFF_TICKS < 1)
        TOGGLE_OFF_TICKS = 1;

    if (TOGGLE_OFF_TICKS > 10)
        TOGGLE_OFF_TICKS = 10;

    COLOR_EMPTY = ParseColor(ini.Get("Colors", "EmptyRed").ToString("255,0,0"), new Color(255, 0, 0));
    COLOR_GROWING = ParseColor(ini.Get("Colors", "GrowingBlue").ToString("0,80,255"), new Color(0, 80, 255));
    COLOR_READY = ParseColor(ini.Get("Colors", "ReadyGreen").ToString("0,255,0"), new Color(0, 255, 0));
    COLOR_TEXT = ParseColor(ini.Get("Colors", "TextColor").ToString("255,255,255"), new Color(255, 255, 255));
    COLOR_TEXT_OUTLINE = ParseColor(ini.Get("Colors", "TextOutlineColor").ToString("55,55,55"), new Color(55, 55, 55));

    EMPTY_WORDS = SplitWords(ini.Get("Keywords", "Empty").ToString("empty plot,no crop,no crops,nothing planted,unplanted,not planted,none"));
    GROWING_WORDS = SplitWords(ini.Get("Keywords", "Growing").ToString("growing,planted,sprout,seedling"));
    READY_WORDS = SplitWords(ini.Get("Keywords", "Ready").ToString("ready,ready to harvest,fully grown,mature,harvest,harvestable,grown,complete,completed,100%,100.00%,yield"));
    DEAD_WORDS = SplitWords(ini.Get("Keywords", "Dead").ToString("dead,withered,died"));
}

void FindBlocks()
{
    farmBlocks.Clear();
    lcds.Clear();
    waterBlocks.Clear();

    List<IMyTerminalBlock> directFarmBlocks = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType(directFarmBlocks, b =>
        HasTag(b.CustomName, FARM_TAG) &&
        (!SAME_CONSTRUCT_ONLY || Me.IsSameConstructAs(b))
    );

    for (int i = 0; i < directFarmBlocks.Count; i++)
        AddFarmBlockIfMissing(directFarmBlocks[i]);

    List<IMyTextPanel> directLCDs = new List<IMyTextPanel>();

    GridTerminalSystem.GetBlocksOfType(directLCDs, b =>
        HasTag(b.CustomName, LCD_TAG) &&
        (!SAME_CONSTRUCT_ONLY || Me.IsSameConstructAs(b))
    );

    for (int i = 0; i < directLCDs.Count; i++)
        AddLCDIfMissing(directLCDs[i]);

    List<IMyTerminalBlock> directWaterBlocks = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType(directWaterBlocks, b =>
        HasTag(b.CustomName, WATER_TAG) &&
        (!SAME_CONSTRUCT_ONLY || Me.IsSameConstructAs(b))
    );

    for (int i = 0; i < directWaterBlocks.Count; i++)
        AddWaterBlockIfMissing(directWaterBlocks[i]);

    groups.Clear();
    GridTerminalSystem.GetBlockGroups(groups);

    for (int g = 0; g < groups.Count; g++)
    {
        IMyBlockGroup group = groups[g];

        if (HasTag(group.Name, FARM_TAG))
        {
            List<IMyTerminalBlock> groupFarmBlocks = new List<IMyTerminalBlock>();

            group.GetBlocks(groupFarmBlocks, b =>
                !SAME_CONSTRUCT_ONLY || Me.IsSameConstructAs(b)
            );

            for (int i = 0; i < groupFarmBlocks.Count; i++)
                AddFarmBlockIfMissing(groupFarmBlocks[i]);
        }

        if (HasTag(group.Name, LCD_TAG))
        {
            List<IMyTextPanel> groupLCDs = new List<IMyTextPanel>();

            group.GetBlocksOfType(groupLCDs, b =>
                !SAME_CONSTRUCT_ONLY || Me.IsSameConstructAs(b)
            );

            for (int i = 0; i < groupLCDs.Count; i++)
                AddLCDIfMissing(groupLCDs[i]);
        }

        if (HasTag(group.Name, WATER_TAG))
        {
            List<IMyTerminalBlock> groupWaterBlocks = new List<IMyTerminalBlock>();

            group.GetBlocks(groupWaterBlocks, b =>
                !SAME_CONSTRUCT_ONLY || Me.IsSameConstructAs(b)
            );

            for (int i = 0; i < groupWaterBlocks.Count; i++)
                AddWaterBlockIfMissing(groupWaterBlocks[i]);
        }
    }

    for (int i = 0; i < lcds.Count; i++)
        PrepareLCD(lcds[i]);
}

bool HasTag(string text, string tag)
{
    if (text == null || tag == null)
        return false;

    return text.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0;
}

void AddFarmBlockIfMissing(IMyTerminalBlock block)
{
    if (block == null)
        return;

    if (!IsFarmPlot(block) && !HasTag(block.CustomName, FARM_TAG))
        return;

    for (int i = 0; i < farmBlocks.Count; i++)
    {
        if (farmBlocks[i].EntityId == block.EntityId)
            return;
    }

    farmBlocks.Add(block);
}

bool IsFarmPlot(IMyTerminalBlock block)
{
    if (block == null)
        return false;

    IMyFunctionalBlock functional = block as IMyFunctionalBlock;

    if (functional == null)
        return false;

    foreach (MyComponentBase component in functional.Components)
    {
        if (component as IMyFarmPlotLogic != null)
            return true;
    }

    return block.BlockDefinition.SubtypeName.IndexOf("FarmPlot", StringComparison.OrdinalIgnoreCase) >= 0;
}

void AddLCDIfMissing(IMyTextPanel lcd)
{
    if (lcd == null)
        return;

    for (int i = 0; i < lcds.Count; i++)
    {
        if (lcds[i].EntityId == lcd.EntityId)
            return;
    }

    lcds.Add(lcd);
}

void AddWaterBlockIfMissing(IMyTerminalBlock block)
{
    if (block == null)
        return;

    if (block.InventoryCount <= 0)
        return;

    for (int i = 0; i < waterBlocks.Count; i++)
    {
        if (waterBlocks[i].EntityId == block.EntityId)
            return;
    }

    waterBlocks.Add(block);
}

PlotState GetPlotState(IMyTerminalBlock block)
{
    if (block == null)
        return PlotState.Unknown;

    FarmInfo info = GetFarmInfo(block);

    if (info.HasLogic)
    {
        long id = block.EntityId;

        if (!info.Planted)
        {
            RemoveHarvestedOverride(id);
            RememberFarmState(id, "", 0.0);
            lastStateById[id] = PlotState.Empty;
            return PlotState.Empty;
        }

        if (!info.Alive)
        {
            RememberFarmState(id, info.Crop, 0.0);
            lastStateById[id] = PlotState.Dead;
            return PlotState.Dead;
        }

        if (IsHarvestedOverride(id) && info.Ready)
        {
            RememberFarmState(id, info.Crop, 100.0);
            lastStateById[id] = PlotState.Empty;
            return PlotState.Empty;
        }

        if (!info.Ready)
            RemoveHarvestedOverride(id);

        RememberFarmState(id, info.Crop, info.Ready ? 100.0 : 1.0);

        PlotState state = info.Ready ? PlotState.Ready : PlotState.Growing;
        lastStateById[id] = state;
        return state;
    }

    return GetPlotStateFromText(block);
}

FarmInfo GetFarmInfo(IMyTerminalBlock block)
{
    FarmInfo info = new FarmInfo();
    info.HasLogic = false;
    info.Planted = false;
    info.Alive = false;
    info.Ready = false;
    info.Crop = "";
    info.OutputAmount = 0;
    info.SeedsRequired = 0;
    info.WaterRatio = 0.0;

    IMyFunctionalBlock functional = block as IMyFunctionalBlock;

    if (functional == null)
        return info;

    IMyFarmPlotLogic logic = null;
    IMyResourceStorageComponent storage = null;

    foreach (MyComponentBase component in functional.Components)
    {
        if (logic == null)
            logic = component as IMyFarmPlotLogic;

        if (storage == null)
            storage = component as IMyResourceStorageComponent;
    }

    if (logic == null)
        return info;

    info.HasLogic = true;

    try
    {
        info.Planted = logic.IsPlantPlanted;
        info.Alive = logic.IsAlive;
        info.Ready = logic.IsPlantFullyGrown;

        if (logic.OutputItem != null)
            info.Crop = logic.OutputItem.SubtypeName;

        if (info.Planted && info.Alive)
            info.OutputAmount = logic.OutputItemAmount;

        if (!info.Planted || !info.Alive)
            info.SeedsRequired = logic.AmountOfSeedsRequired;
    }
    catch
    {
    }

    try
    {
        if (storage != null)
            info.WaterRatio = storage.FilledRatio;
    }
    catch
    {
        info.WaterRatio = 0.0;
    }

    return info;
}

PlotState GetPlotStateFromText(IMyTerminalBlock block)
{
    if (block == null)
        return PlotState.Unknown;

    long id = block.EntityId;

    string text = "";
    text += block.CustomName + "\n";
    text += block.DetailedInfo + "\n";
    text += block.CustomData + "\n";

    string lower = text.ToLower();

    if (ContainsAny(lower, DEAD_WORDS))
        return PlotState.Dead;

    bool cropLineExists = HasCropTypeLine(lower);
    string cropType = GetCropType(lower);
    double growth = GetGrowthProgress(lower);

    double previousGrowth = -1.0;
    bool hasPreviousGrowth = lastGrowthById.TryGetValue(id, out previousGrowth);

    string previousCrop = "";
    bool hasPreviousCrop = lastCropById.TryGetValue(id, out previousCrop);

    if (cropLineExists)
    {
        if (IsEmptyCropType(cropType))
        {
            RemoveHarvestedOverride(id);
            RememberFarmState(id, cropType, growth);
            return PlotState.Empty;
        }

        if (IsHarvestedOverride(id))
        {
            if (growth >= 0.0 && growth < 99.9)
                RemoveHarvestedOverride(id);
            else
            {
                RememberFarmState(id, cropType, growth);
                return PlotState.Empty;
            }
        }

        if (hasPreviousGrowth && previousGrowth >= 99.9 && growth >= 0.0 && growth < 99.9)
        {
            RemoveHarvestedOverride(id);
            RememberFarmState(id, cropType, growth);
            return PlotState.Growing;
        }

        if (hasPreviousCrop && previousCrop.Length > 0 && previousCrop != cropType)
            RemoveHarvestedOverride(id);

        RememberFarmState(id, cropType, growth);

        if (growth >= 99.9)
            return PlotState.Ready;

        return PlotState.Growing;
    }

    if (ContainsAny(lower, READY_WORDS))
        return PlotState.Ready;

    if (ContainsAny(lower, EMPTY_WORDS))
        return PlotState.Empty;

    if (growth >= 99.9)
        return PlotState.Ready;

    if (growth > 0.0)
        return PlotState.Growing;

    if (ZERO_PERCENT_COUNTS_AS_GROWING && growth == 0.0)
        return PlotState.Growing;

    if (ContainsAny(lower, GROWING_WORDS))
        return PlotState.Growing;

    if (UNKNOWN_COUNTS_AS_GROWING)
        return PlotState.Growing;

    return PlotState.Unknown;
}

void RememberFarmState(long id, string cropType, double growth)
{
    if (growth >= 0.0)
        lastGrowthById[id] = growth;

    if (cropType != null)
        lastCropById[id] = cropType;
}

void MarkCurrentReadyPlotsAsHarvested()
{
    harvestedOverrideIds.Clear();

    for (int i = 0; i < farmBlocks.Count; i++)
    {
        IMyTerminalBlock block = farmBlocks[i];

        if (block == null)
            continue;

        FarmInfo info = GetFarmInfo(block);

        if (info.HasLogic)
        {
            if (info.Planted && info.Alive && info.Ready)
                AddHarvestedOverride(block.EntityId);
        }
        else
        {
            string lower = (block.CustomName + "\n" + block.DetailedInfo + "\n" + block.CustomData).ToLower();
            double growth = GetGrowthProgress(lower);

            if (growth >= 99.9)
                AddHarvestedOverride(block.EntityId);
        }
    }

    SaveStorage();
}

bool IsHarvestedOverride(long id)
{
    for (int i = 0; i < harvestedOverrideIds.Count; i++)
    {
        if (harvestedOverrideIds[i] == id)
            return true;
    }

    return false;
}

void AddHarvestedOverride(long id)
{
    if (!IsHarvestedOverride(id))
        harvestedOverrideIds.Add(id);
}

void RemoveHarvestedOverride(long id)
{
    for (int i = harvestedOverrideIds.Count - 1; i >= 0; i--)
    {
        if (harvestedOverrideIds[i] == id)
            harvestedOverrideIds.RemoveAt(i);
    }
}

bool HasCropTypeLine(string text)
{
    return text.IndexOf("current crop type:") >= 0 || text.IndexOf("crop type:") >= 0 || text.IndexOf("crop:") >= 0;
}

string GetCropType(string text)
{
    string crop = GetLabelValue(text, "current crop type:");

    if (crop.Length == 0)
        crop = GetLabelValue(text, "crop type:");

    if (crop.Length == 0)
        crop = GetLabelValue(text, "crop:");

    return crop;
}

string GetLabelValue(string text, string label)
{
    int index = text.IndexOf(label);

    if (index < 0)
        return "";

    int start = index + label.Length;
    int end = text.IndexOf('\n', start);

    if (end < 0)
        end = text.Length;

    return text.Substring(start, end - start).Trim().ToLower();
}

bool IsEmptyCropType(string crop)
{
    if (crop.Length == 0)
        return true;

    if (crop == "none")
        return true;

    if (crop == "n/a")
        return true;

    if (crop.Contains("no crop"))
        return true;

    if (crop.Contains("empty"))
        return true;

    if (crop.Contains("nothing"))
        return true;

    return false;
}

double GetGrowthProgress(string text)
{
    double value = GetPercentAfterLabel(text, "growth progress:");

    if (value >= 0.0)
        return value;

    value = GetPercentAfterLabel(text, "growth:");

    if (value >= 0.0)
        return value;

    value = GetPercentAfterLabel(text, "progress:");

    return value;
}

double GetPercentAfterLabel(string text, string label)
{
    int index = text.IndexOf(label);

    if (index < 0)
        return -1.0;

    int start = index + label.Length;
    int end = text.IndexOf('%', start);

    if (end < 0)
        return -1.0;

    string numberText = text.Substring(start, end - start).Trim();

    double value;

    if (double.TryParse(numberText, out value))
        return value;

    return -1.0;
}

bool ContainsAny(string text, List<string> words)
{
    for (int i = 0; i < words.Count; i++)
    {
        string word = words[i].Trim().ToLower();

        if (word.Length == 0)
            continue;

        if (text.Contains(word))
            return true;
    }

    return false;
}

void PrepareLCD(IMyTextPanel lcd)
{
    if (lcd == null)
        return;

    if (lcd.ContentType != ContentType.SCRIPT)
        lcd.ContentType = ContentType.SCRIPT;

    if (CLEAR_LCD_SCRIPT_SELECTION)
        lcd.Script = "";

    lcd.TextPadding = 0f;
}

void PaintLCD(IMyTextPanel lcd, Color color, string status, int total, int empty, int growing, int ready, int dead, int unknown, string waterText)
{
    if (lcd == null)
        return;

    PrepareLCD(lcd);

    // On dedicated servers, changing ScriptBackgroundColor or ScriptForegroundColor can
    // make only some sprites reach the client. Default behavior avoids these writes and
    // paints the background as a sprite instead.
    if (USE_NATIVE_LCD_BACKGROUND && !AVOID_SCRIPT_COLOR_WRITES)
    {
        lcd.BackgroundColor = color;
        lcd.ScriptBackgroundColor = color;
        lcd.ScriptForegroundColor = COLOR_TEXT;
        lcd.FontColor = COLOR_TEXT;
    }

    Vector2 surfaceSize;
    Vector2 textureSize;
    Vector2 viewportOffset;
    Vector2 center;

    if (!GetSurfaceMetrics(lcd, out surfaceSize, out textureSize, out viewportOffset, out center))
        return;

    MySpriteDrawFrame frame = lcd.DrawFrame();

    AddFrameSyncBreaker(frame, lcd.EntityId);

    if (USE_BACKGROUND_SPRITE)
    {
        // Use the real visible viewport size, not the full texture. This avoids wide/corner/sloped
        // LCDs being filled or centered incorrectly when TextureSize and SurfaceSize differ.
        AddFilledRect(frame, center, surfaceSize + new Vector2(4f, 4f), color);
    }

    if (SHOW_TEXT_ON_LCD)
    {
        string line1 = status;
        string line2 = "Plots: " + total;
        string line3 = "E:" + empty + " G:" + growing + " R:" + ready + " U:" + unknown;
        string line4 = waterText;

        float aspect = surfaceSize.X / surfaceSize.Y;

        if (USE_WIDE_LAYOUT && aspect >= WIDE_LAYOUT_ASPECT)
            PaintWideSpriteText(frame, lcd, center, surfaceSize, line1, line2, line3, line4);
        else
            PaintStackedSpriteText(frame, lcd, center, surfaceSize, line1, line2, line3, line4);
    }

    frame.Dispose();
}

bool GetSurfaceMetrics(IMyTextPanel lcd, out Vector2 surfaceSize, out Vector2 textureSize, out Vector2 viewportOffset, out Vector2 center)
{
    surfaceSize = lcd.SurfaceSize;
    textureSize = lcd.TextureSize;
    viewportOffset = Vector2.Zero;
    center = Vector2.Zero;

    if (surfaceSize.X <= 0f || surfaceSize.Y <= 0f)
        return false;

    if (textureSize.X <= 0f || textureSize.Y <= 0f)
        textureSize = surfaceSize;

    viewportOffset = (textureSize - surfaceSize) / 2f;
    center = viewportOffset + (surfaceSize / 2f);
    return true;
}

float GetBaseSpriteScale(Vector2 surfaceSize)
{
    float scale = TEXT_SCALE;

    if (AUTOFIT_TEXT)
    {
        float minDimension = Math.Min(surfaceSize.X, surfaceSize.Y);
        float scaleLimit = minDimension / 512f;

        if (scaleLimit < 0.45f)
            scaleLimit = 0.45f;

        if (scaleLimit > 1.15f)
            scaleLimit = 1.15f;

        scale *= scaleLimit;
    }

    if (scale < 0.35f)
        scale = 0.35f;

    if (scale > 4.0f)
        scale = 4.0f;

    return scale;
}

float FitTextScale(IMyTextPanel lcd, string text, float wantedScale, float maxWidth, float maxHeight)
{
    if (!AUTOFIT_TEXT)
        return wantedScale;

    if (text == null || text.Length == 0)
        return wantedScale;

    if (maxWidth <= 1f || maxHeight <= 1f)
        return wantedScale;

    measureBuilder.Clear();
    measureBuilder.Append(text);

    Vector2 measured = lcd.MeasureStringInPixels(measureBuilder, "White", wantedScale);

    if (measured.X <= 0f || measured.Y <= 0f)
        return wantedScale;

    float widthFit = maxWidth / measured.X;
    float heightFit = maxHeight / measured.Y;
    float fit = Math.Min(widthFit, heightFit);

    if (fit < 1.0f)
        wantedScale *= fit;

    if (wantedScale < 0.22f)
        wantedScale = 0.22f;

    if (wantedScale > 4.0f)
        wantedScale = 4.0f;

    return wantedScale;
}

void PaintStackedSpriteText(MySpriteDrawFrame frame, IMyTextPanel lcd, Vector2 center, Vector2 surfaceSize, string line1, string line2, string line3, string line4)
{
    float scale = GetBaseSpriteScale(surfaceSize);

    float statusScale = FitTextScale(lcd, line1, 1.00f * scale, surfaceSize.X * 0.94f, surfaceSize.Y * 0.26f);
    float mediumScale = FitTextScale(lcd, line2, 0.68f * scale, surfaceSize.X * 0.94f, surfaceSize.Y * 0.18f);
    float smallScale = FitTextScale(lcd, line3, 0.52f * scale, surfaceSize.X * 0.94f, surfaceSize.Y * 0.16f);
    float waterScale = FitTextScale(lcd, line4, 0.52f * scale, surfaceSize.X * 0.94f, surfaceSize.Y * 0.16f);

    Vector2 verticalOffset = new Vector2(0f, surfaceSize.Y * STACKED_VERTICAL_OFFSET);

    // v10.26 used fixed screen-height percentages, which made the rows too spread out.
    // This restores the older v10.7-style spacing, then lets Custom Data tighten or loosen it.
    float spacing = 34f * scale * STACKED_LINE_SPACING;
    float visibleLines = SHOW_WATER_LINE ? 4f : 3f;
    float maxSpacing = surfaceSize.Y / (visibleLines + 1.8f);

    if (spacing > maxSpacing)
        spacing = maxSpacing;

    if (spacing < 16f)
        spacing = 16f;

    Vector2 pos1;
    Vector2 pos2;
    Vector2 pos3;
    Vector2 pos4;

    if (SHOW_WATER_LINE)
    {
        pos1 = center + verticalOffset + new Vector2(0f, -1.50f * spacing);
        pos2 = center + verticalOffset + new Vector2(0f, -0.35f * spacing);
        pos3 = center + verticalOffset + new Vector2(0f,  0.55f * spacing);
        pos4 = center + verticalOffset + new Vector2(0f,  1.35f * spacing);
    }
    else
    {
        pos1 = center + verticalOffset + new Vector2(0f, -0.95f * spacing);
        pos2 = center + verticalOffset + new Vector2(0f,  0.00f * spacing);
        pos3 = center + verticalOffset + new Vector2(0f,  0.95f * spacing);
        pos4 = center + verticalOffset;
    }

    // Draw every outline first, then every main white text sprite last.
    // This keeps the visible text on top even when the renderer is picky.
    AddTextOutline(frame, line1, pos1, statusScale, TextAlignment.CENTER);
    AddTextOutline(frame, line2, pos2, mediumScale, TextAlignment.CENTER);
    AddTextOutline(frame, line3, pos3, smallScale, TextAlignment.CENTER);

    if (SHOW_WATER_LINE)
        AddTextOutline(frame, line4, pos4, waterScale, TextAlignment.CENTER);

    AddTextMain(frame, line1, pos1, statusScale, TextAlignment.CENTER);
    AddTextMain(frame, line2, pos2, mediumScale, TextAlignment.CENTER);
    AddTextMain(frame, line3, pos3, smallScale, TextAlignment.CENTER);

    if (SHOW_WATER_LINE)
        AddTextMain(frame, line4, pos4, waterScale, TextAlignment.CENTER);
}

void PaintWideSpriteText(MySpriteDrawFrame frame, IMyTextPanel lcd, Vector2 center, Vector2 surfaceSize, string line1, string line2, string line3, string line4)
{
    float scale = GetBaseSpriteScale(surfaceSize);

    Vector2 leftCenter = center + new Vector2(-surfaceSize.X * 0.20f, 0f);
    Vector2 rightCenter = center + new Vector2( surfaceSize.X * 0.29f, 0f);

    Vector2 pos1 = leftCenter + new Vector2(0f, -surfaceSize.Y * 0.13f);
    Vector2 pos2 = leftCenter + new Vector2(0f,  surfaceSize.Y * 0.18f);
    Vector2 pos3;
    Vector2 pos4;

    if (SHOW_WATER_LINE)
    {
        pos3 = rightCenter + new Vector2(0f, -surfaceSize.Y * 0.12f);
        pos4 = rightCenter + new Vector2(0f,  surfaceSize.Y * 0.18f);
    }
    else
    {
        pos3 = rightCenter + new Vector2(0f, 0f);
        pos4 = rightCenter;
    }

    float leftWidth = surfaceSize.X * 0.58f;
    float rightWidth = surfaceSize.X * 0.36f;

    float statusScale = FitTextScale(lcd, line1, 1.05f * scale, leftWidth, surfaceSize.Y * 0.36f);
    float mediumScale = FitTextScale(lcd, line2, 0.66f * scale, leftWidth, surfaceSize.Y * 0.22f);
    float countScale = FitTextScale(lcd, line3, 0.58f * scale, rightWidth, surfaceSize.Y * 0.22f);
    float waterScale = FitTextScale(lcd, line4, 0.58f * scale, rightWidth, surfaceSize.Y * 0.22f);

    // Wide LCDs get a real two-column sprite layout. The old stacked layout can place
    // the count and water rows too close together on very wide/short panels.
    AddTextOutline(frame, line1, pos1, statusScale, TextAlignment.CENTER);
    AddTextOutline(frame, line2, pos2, mediumScale, TextAlignment.CENTER);
    AddTextOutline(frame, line3, pos3, countScale, TextAlignment.CENTER);

    if (SHOW_WATER_LINE)
        AddTextOutline(frame, line4, pos4, waterScale, TextAlignment.CENTER);

    AddTextMain(frame, line1, pos1, statusScale, TextAlignment.CENTER);
    AddTextMain(frame, line2, pos2, mediumScale, TextAlignment.CENTER);
    AddTextMain(frame, line3, pos3, countScale, TextAlignment.CENTER);

    if (SHOW_WATER_LINE)
        AddTextMain(frame, line4, pos4, waterScale, TextAlignment.CENTER);
}

void PaintLCDTest()
{
    Echo("LCD SPRITE TEST MODE");
    Echo("Run argument 'normal' to exit.");
    Echo("LCDs found: " + lcds.Count);

    for (int i = 0; i < lcds.Count; i++)
    {
        IMyTextPanel lcd = lcds[i];

        if (lcd == null)
            continue;

        PrepareLCD(lcd);

        if (!AVOID_SCRIPT_COLOR_WRITES)
        {
            lcd.BackgroundColor = Color.Black;
            lcd.ScriptBackgroundColor = Color.Black;
        }

        Vector2 surfaceSize;
        Vector2 textureSize;
        Vector2 viewportOffset;
        Vector2 center;

        if (!GetSurfaceMetrics(lcd, out surfaceSize, out textureSize, out viewportOffset, out center))
            continue;

        MySpriteDrawFrame frame = lcd.DrawFrame();

        AddFrameSyncBreaker(frame, lcd.EntityId);

        AddFilledRect(frame, center, surfaceSize * 1.25f, new Color(0, 0, 0));
        AddFilledRect(frame, center + new Vector2(0, -surfaceSize.Y * 0.47f), new Vector2(surfaceSize.X * 0.94f, 4f), Color.White);
        AddFilledRect(frame, center + new Vector2(0, surfaceSize.Y * 0.47f), new Vector2(surfaceSize.X * 0.94f, 4f), Color.White);
        AddFilledRect(frame, center + new Vector2(-surfaceSize.X * 0.47f, 0), new Vector2(4f, surfaceSize.Y * 0.94f), Color.White);
        AddFilledRect(frame, center + new Vector2(surfaceSize.X * 0.47f, 0), new Vector2(4f, surfaceSize.Y * 0.94f), Color.White);
        AddFilledRect(frame, center, new Vector2(surfaceSize.X * 0.75f, 3f), new Color(0, 180, 255));
        AddFilledRect(frame, center, new Vector2(3f, surfaceSize.Y * 0.75f), new Color(0, 180, 255));

        float scale = TEXT_SCALE;

        if (AUTOFIT_TEXT)
        {
            float minDimension = Math.Min(surfaceSize.X, surfaceSize.Y);
            float scaleLimit = minDimension / 512f;

            if (scaleLimit < 0.45f)
                scaleLimit = 0.45f;

            if (scaleLimit > 1.15f)
                scaleLimit = 1.15f;

            scale *= scaleLimit;
        }

        if (scale < 0.4f)
            scale = 0.4f;

        if (scale > 3.0f)
            scale = 3.0f;

        string name = lcd.CustomName;

        if (name.Length > 28)
            name = name.Substring(0, 28);

        AddTextOutline(frame, "LCD SPRITE TEST", center + new Vector2(0, -52f * scale), 0.75f * scale, TextAlignment.CENTER);
        AddTextOutline(frame, "#" + (i + 1) + "  " + name, center, 0.48f * scale, TextAlignment.CENTER);
        AddTextOutline(frame, "Surface " + surfaceSize.X.ToString("0") + "x" + surfaceSize.Y.ToString("0") + "  Texture " + textureSize.X.ToString("0") + "x" + textureSize.Y.ToString("0"), center + new Vector2(0, 42f * scale), 0.36f * scale, TextAlignment.CENTER);
        AddTextMain(frame, "LCD SPRITE TEST", center + new Vector2(0, -52f * scale), 0.75f * scale, TextAlignment.CENTER);
        AddTextMain(frame, "#" + (i + 1) + "  " + name, center, 0.48f * scale, TextAlignment.CENTER);
        AddTextMain(frame, "Surface " + surfaceSize.X.ToString("0") + "x" + surfaceSize.Y.ToString("0") + "  Texture " + textureSize.X.ToString("0") + "x" + textureSize.Y.ToString("0"), center + new Vector2(0, 42f * scale), 0.36f * scale, TextAlignment.CENTER);

        frame.Dispose();

        Echo("LCD " + (i + 1) + ": " + lcd.CustomName);
        Echo("  Surface: " + surfaceSize.X.ToString("0") + "x" + surfaceSize.Y.ToString("0"));
        Echo("  Texture: " + textureSize.X.ToString("0") + "x" + textureSize.Y.ToString("0"));
    }
}

void AddFrameSyncBreaker(MySpriteDrawFrame frame, long surfaceId)
{
    if (!SPRITE_CACHE_BUSTER)
        return;

    // This is the Whiplash-style sprite sync workaround: changing the number of
    // empty sprites at the start shifts the sprite indices, which forces clients
    // to re-receive the real sprites instead of only showing the last changed row.
    int blanks = 1 + (int)((renderFrame + surfaceId) % 3);

    if (blanks < 1)
        blanks = 1;

    for (int i = 0; i < blanks; i++)
        frame.Add(new MySprite());
}

void AddTextOutline(MySpriteDrawFrame frame, string text, Vector2 position, float scale, TextAlignment alignment)
{
    if (!USE_TEXT_OUTLINE || TEXT_OUTLINE_OFFSET <= 0.0f)
        return;

    float d = TEXT_OUTLINE_OFFSET;
    float h = d * 0.7f;

    AddTextSprite(frame, text, position + new Vector2(-d, 0), scale, alignment, COLOR_TEXT_OUTLINE);
    AddTextSprite(frame, text, position + new Vector2(d, 0), scale, alignment, COLOR_TEXT_OUTLINE);
    AddTextSprite(frame, text, position + new Vector2(0, -d), scale, alignment, COLOR_TEXT_OUTLINE);
    AddTextSprite(frame, text, position + new Vector2(0, d), scale, alignment, COLOR_TEXT_OUTLINE);

    if (OUTLINE_MODE >= 8)
    {
        AddTextSprite(frame, text, position + new Vector2(-h, -h), scale, alignment, COLOR_TEXT_OUTLINE);
        AddTextSprite(frame, text, position + new Vector2(h, -h), scale, alignment, COLOR_TEXT_OUTLINE);
        AddTextSprite(frame, text, position + new Vector2(-h, h), scale, alignment, COLOR_TEXT_OUTLINE);
        AddTextSprite(frame, text, position + new Vector2(h, h), scale, alignment, COLOR_TEXT_OUTLINE);
    }
}

void AddTextMain(MySpriteDrawFrame frame, string text, Vector2 position, float scale, TextAlignment alignment)
{
    AddTextSprite(frame, text, position, scale, alignment, COLOR_TEXT);
}

void AddTextSprite(MySpriteDrawFrame frame, string text, Vector2 position, float scale, TextAlignment alignment, Color color)
{
    float finalScale = scale;

    if (SPRITE_CACHE_BUSTER)
    {
        // Tiny scale pulse is visually invisible, but it makes every text sprite
        // slightly different each update so DS/client sprite caching does not
        // leave old or missing text on odd LCD surfaces.
        if ((renderFrame & 1) == 0)
            finalScale += 0.0007f;
        else
            finalScale -= 0.0007f;
    }

    MySprite sprite = MySprite.CreateText(text, "White", color, finalScale, alignment);
    sprite.Position = position;
    frame.Add(sprite);
}

void AddFilledRect(MySpriteDrawFrame frame, Vector2 position, Vector2 size, Color color)
{
    MySprite rect = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        Position = position,
        Size = size,
        Color = color,
        Alignment = TextAlignment.CENTER
    };

    frame.Add(rect);
}

void DebugFarmBlocks()
{
    Echo("DEBUG MODE");
    Echo("Run argument 'normal' to exit.");
    Echo("");
    Echo("Farm blocks found: " + farmBlocks.Count);
    Echo("LCDs found: " + lcds.Count);
    Echo("Water blocks found: " + waterBlocks.Count);
    Echo("Harvested overrides: " + harvestedOverrideIds.Count);
    Echo("AutoToggleRefresh: " + AUTO_TOGGLE_REFRESH);
    Echo("ToggleActive: " + TOGGLE_ACTIVE + " OffTimer: " + offTimer);
    Echo("AutoFitText: " + AUTOFIT_TEXT);
    Echo("NativeBackground: " + USE_NATIVE_LCD_BACKGROUND);
    Echo("BackgroundSprite: " + USE_BACKGROUND_SPRITE);
    Echo("TextOutline: " + USE_TEXT_OUTLINE);
    Echo("TextOutlineOffset: " + TEXT_OUTLINE_OFFSET.ToString("0.00"));
    Echo("OutlineMode: " + OUTLINE_MODE);
    Echo("UseWideLayout: " + USE_WIDE_LAYOUT);
    Echo("WideAspect: " + WIDE_LAYOUT_ASPECT.ToString("0.00"));
    Echo("StackedLineSpacing: " + STACKED_LINE_SPACING.ToString("0.00"));
    Echo("StackedVerticalOffset: " + STACKED_VERTICAL_OFFSET.ToString("0.00"));
    Echo("SpriteCacheBuster: " + SPRITE_CACHE_BUSTER);
    Echo("AvoidScriptColorWrites: " + AVOID_SCRIPT_COLOR_WRITES);
    Echo("");

    string debugText = "FARM DEBUG v10.27\n";
    debugText += "Run 'normal' to exit.\n\n";
    debugText += "Farm blocks found: " + farmBlocks.Count + "\n";
    debugText += "LCDs found: " + lcds.Count + "\n";
    debugText += "Water blocks found: " + waterBlocks.Count + "\n";
    debugText += "Water status: " + GetWaterStatusText() + "\n";
    debugText += "Harvested overrides: " + harvestedOverrideIds.Count + "\n";
    debugText += "AutoToggleRefresh: " + AUTO_TOGGLE_REFRESH + "\n";
    debugText += "ToggleActive: " + TOGGLE_ACTIVE + " OffTimer: " + offTimer + "\n";
    debugText += "Pure sprite renderer: true\n";
    debugText += "TextScale: " + TEXT_SCALE.ToString("0.00") + "\n";
    debugText += "TextOutline: " + USE_TEXT_OUTLINE + "\n";
    debugText += "TextOutlineOffset: " + TEXT_OUTLINE_OFFSET.ToString("0.00") + "\n";
    debugText += "OutlineMode: " + OUTLINE_MODE + "\n";
    debugText += "UseWideLayout: " + USE_WIDE_LAYOUT + "\n";
    debugText += "WideAspect: " + WIDE_LAYOUT_ASPECT.ToString("0.00") + "\n";
    debugText += "StackedLineSpacing: " + STACKED_LINE_SPACING.ToString("0.00") + "\n";
    debugText += "StackedVerticalOffset: " + STACKED_VERTICAL_OFFSET.ToString("0.00") + "\n";
    debugText += "SpriteCacheBuster: " + SPRITE_CACHE_BUSTER + "\n";
    debugText += "AvoidScriptColorWrites: " + AVOID_SCRIPT_COLOR_WRITES + "\n\n";

    int max = Math.Min(farmBlocks.Count, 3);

    for (int i = 0; i < max; i++)
    {
        IMyTerminalBlock block = farmBlocks[i];
        PlotState state = GetPlotState(block);
        FarmInfo info = GetFarmInfo(block);

        debugText += "---- FARM " + (i + 1) + " ----\n";
        debugText += "Name: " + block.CustomName + "\n";
        debugText += "State: " + state.ToString() + "\n";
        debugText += "HasFarmLogic: " + info.HasLogic + "\n";
        debugText += "Planted: " + info.Planted + " Alive: " + info.Alive + " Ready: " + info.Ready + "\n";
        debugText += "Crop: " + info.Crop + " Output: " + info.OutputAmount + " SeedsReq: " + info.SeedsRequired + "\n";
        debugText += "WaterRatio: " + info.WaterRatio.ToString("0.000") + "\n";
        debugText += "Override: " + IsHarvestedOverride(block.EntityId) + "\n";
        debugText += "Enabled: " + ((block as IMyFunctionalBlock) != null ? ((IMyFunctionalBlock)block).Enabled.ToString() : "N/A") + "\n";
        debugText += "Subtype: " + block.BlockDefinition.SubtypeName + "\n";
        debugText += "DetailedInfoLen: " + block.DetailedInfo.Length + "\n";
        debugText += "CustomDataLen: " + block.CustomData.Length + "\n\n";

        Echo("Farm " + (i + 1) + ": " + state.ToString());
        Echo(block.CustomName);
        Echo("HasFarmLogic: " + info.HasLogic);
    }

    for (int i = 0; i < lcds.Count; i++)
    {
        lcds[i].ContentType = ContentType.TEXT_AND_IMAGE;
        lcds[i].FontSize = 0.6f;
        lcds[i].FontColor = Color.White;
        lcds[i].BackgroundColor = Color.Black;
        lcds[i].WriteText(debugText);
    }
}

void LoadStorage()
{
    lastGrowthById.Clear();
    lastCropById.Clear();
    harvestedOverrideIds.Clear();

    if (string.IsNullOrWhiteSpace(Storage))
        return;

    string[] lines = Storage.Split('\n');

    for (int i = 0; i < lines.Length; i++)
    {
        string line = lines[i].Trim();

        if (line.Length == 0)
            continue;

        string[] parts = line.Split('|');

        if (parts.Length < 2)
            continue;

        if (parts[0] == "G" && parts.Length >= 3)
        {
            long id;
            double growth;

            if (long.TryParse(parts[1], out id) && double.TryParse(parts[2], out growth))
                lastGrowthById[id] = growth;
        }
        else if (parts[0] == "C" && parts.Length >= 3)
        {
            long id;

            if (long.TryParse(parts[1], out id))
                lastCropById[id] = parts[2];
        }
        else if (parts[0] == "H")
        {
            long id;

            if (long.TryParse(parts[1], out id))
                AddHarvestedOverride(id);
        }
    }
}

void SaveStorage()
{
    string output = "";

    foreach (KeyValuePair<long, double> item in lastGrowthById)
        output += "G|" + item.Key + "|" + item.Value.ToString("0.00") + "\n";

    foreach (KeyValuePair<long, string> item in lastCropById)
    {
        string crop = item.Value.Replace("|", "");
        output += "C|" + item.Key + "|" + crop + "\n";
    }

    for (int i = 0; i < harvestedOverrideIds.Count; i++)
        output += "H|" + harvestedOverrideIds[i] + "\n";

    Storage = output;
}

List<string> SplitWords(string input)
{
    List<string> result = new List<string>();
    string[] parts = input.Split(',');

    for (int i = 0; i < parts.Length; i++)
    {
        string part = parts[i].Trim();

        if (part.Length > 0)
            result.Add(part);
    }

    return result;
}

Color ParseColor(string input, Color fallback)
{
    try
    {
        string[] parts = input.Split(',');

        if (parts.Length < 3)
            return fallback;

        int r = ClampInt(int.Parse(parts[0].Trim()), 0, 255);
        int g = ClampInt(int.Parse(parts[1].Trim()), 0, 255);
        int b = ClampInt(int.Parse(parts[2].Trim()), 0, 255);

        return new Color((byte)r, (byte)g, (byte)b);
    }
    catch
    {
        return fallback;
    }
}

int ClampInt(int value, int min, int max)
{
    if (value < min)
        return min;

    if (value > max)
        return max;

    return value;
}

string DefaultCustomData()
{
    return
@"[Script]
Version=27

[Tags]
FarmBlocks=[Farm Blocks]
FarmLCD=[Farm LCD]
FarmWater=[Farm Water]

[Options]
SameConstructOnly=true
RequireAllPlotsPlantedForGreen=false
UnknownCountsAsGrowing=false
ShowTextOnLCD=true
AutoFitText=true
UseNativeLCDBackground=false
UseBackgroundSprite=true
ClearLCDScriptSelection=true
TextOutline=true
TextOutlineOffset=1
OutlineMode=8
UseWideLayout=false
WideLayoutAspect=1.65
StackedLineSpacing=0.82
StackedVerticalOffset=0
SpriteCacheBuster=true
AvoidScriptColorWrites=true
ShowWaterLine=true
TextScale=2
ZeroPercentCountsAsGrowing=false

[Refresh]
AutoToggleFarmPlots=true
ToggleEveryTicks=18
ToggleOffTicks=1
EnableFarmsOnStart=true
HoldDisplayDuringRefresh=true

[Colors]
EmptyRed=255,0,0
GrowingBlue=0,80,255
ReadyGreen=0,255,0
TextColor=255,255,255
TextOutlineColor=55,55,55

[Keywords]
Empty=empty plot,no crop,no crops,nothing planted,unplanted,not planted,none
Growing=growing,planted,sprout,seedling
Ready=ready,ready to harvest,fully grown,mature,harvest,harvestable,grown,complete,completed,100%,100.00%,yield
Dead=dead,withered,died
";
}
