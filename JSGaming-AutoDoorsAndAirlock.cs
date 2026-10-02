/*
 * JSGaming Auto Doors and Airlock
 * -----------
 * URL: https://steamcommunity.com/sharedfiles/filedetails/?id=3476661448
 */

// =======================================================================================
// Instructions
// =======================================================================================
/*
 * Run this script to autoclose any door.
 * Modify block specific settings on its custom data. Dont forget to set UseGlobalSettings to false!
 * To use lights and airvents in an airlock add the Tag in its name.
 * Basic airlocks are easy to use. You open a door on one side, it will disable all doors on the other.
 * Advanced airlocks requires an airvent and works based on the pressure state of the room. Toggle the doors by changing the Depressurize mode on the airvent.
 */

// =======================================================================================
// Config - Global Settings
// Changes these properties require a recompile!
// =======================================================================================
static string TagAirlock = "[Airlock]"; // Used for air vents and lights
static int UpdateAllBlocksInterval = 300; // Interval for rechecking the grid for new blocks. Set to 0 to disable.
static UpdateFrequency Update = UpdateFrequency.Update10; // How quick the script updates. Use Update1, Update10 or Update100.

// =======================================================================================
// Config - Block Settings
// Globally used settings. Used to initiate default values for block specific settings.
// =======================================================================================

// If true, it uses global settings, otherwise it uses settings according to the custom data on the specific block
static bool UseGlobalSettings = true;

// Do specific door types auto close, and after how many seconds?
static bool AutoCloseDoors = true;
static float CloseDoorsAfter = 5.0f;

static bool AutoCloseHangarDoors = false;
static float CloseHangarDoorsAfter = 30.0f;

// To group blocks into an airlock. Empty means no group.
static string AirlockGroup = string.Empty;

// Is block part of the Interior side of an airlock? Only doors need an Interior or Exterior true value.
static bool IsInterior = false;

// Is block part of the Exterior side of an airlock? Only doors need an Interior or Exterior true value.
static bool IsExterior = false;

// Light Color settings
static Color LockedColor = new Color(255, 50, 50); // Color when the door is disabled
static Color UnlockedColor = new Color(255, 255, 220); // Color when the door is enabled

// Light settings
static float LockedBlinkIntervalSeconds = 3;
static float LockedBlinkLength = 50;
static float UnlockedBlinkIntervalSeconds = 0;
static float UnlockedBlinkLength = 50;

// Airvent settings
static float MinimumLevelPressurized = 0.9f; // The minimum level to consider the room being pressurized (0.0f - 1.0f)
static float MaximumLevelDepressurized = 0.1f; // The maximum level to consider the room being depressurized (0.0f - 1.0f)

// =======================================================================================
// MAIN - DONT EDIT BELOW
// =======================================================================================

static Properties GlobalDoorProperties;
static Properties GlobalHangarDoorProperties;
static Properties GlobalAirventProperties;
static Properties GlobalLightProperties;

static string SettingsHeader = "[JSGaming - Auto Doors and Airlock]";
static double AverageUsage = 0;
static double AverageUsagePerSecond = 0;

static List<IMyDoor> MyDoorList = new List<IMyDoor>();
static List<IMyAirVent> MyAirVentList = new List<IMyAirVent>();
static List<IMyLightingBlock> MyLightingBlockList = new List<IMyLightingBlock>();
static List<IMyProgrammableBlock> MyProgrammableBlockList = new List<IMyProgrammableBlock>();

static Dictionary<long, BlockDoorStatus> DoorStatusList = new Dictionary<long, BlockDoorStatus>();
static Dictionary<long, BlockAirventStatus> AirventStatusList = new Dictionary<long, BlockAirventStatus>();
static Dictionary<long, BlockLightStatus> LightStatusList = new Dictionary<long, BlockLightStatus>();

static Dictionary<string, Airlock> AirlockList = new Dictionary<string, Airlock>();

class BlockDoorStatus
{
    public IMyDoor Block;
    public string BlockType;
    public Properties BlockProperties;
    public bool IsOpen;
    public float ElapsedTime;

    public BlockDoorStatus(IMyDoor block)
    {
        Block = block;
        BlockType = block.GetType().Name;
        BlockProperties = CustomDataReader(block);
        IsOpen = false;
        ElapsedTime = 0;
    }

    public void AutoClose(float deltaTime)
    {
        if (Block == null || Block.Closed || Block.CubeGrid == null)
            return; // Does not exist!

        if (!IsOpen && Block.Status == DoorStatus.Closed)
            return; // No basic state update needed!

        // Get block settings
        string airlockGroup = BlockProperties.GetString("AirlockGroup");
        bool hasAirlockGroup = (airlockGroup != null);
        bool autoClose = false;
        float closeAfter = 0;

        if (BlockType.ToLower().Contains("hangar"))
        {
            closeAfter = BlockProperties.GetFloat("CloseHangarDoorAfter");
            autoClose = BlockProperties.GetBool("AutoCloseHangarDoor");
        }
        else
        {
            closeAfter = BlockProperties.GetFloat("CloseDoorAfter");
            autoClose = BlockProperties.GetBool("AutoCloseDoor");
        }

        if (Block.Status == DoorStatus.Open || Block.Status == DoorStatus.Opening)
        {
            if (IsOpen && autoClose && closeAfter > 0)
            {
                // Counter for closing
                ElapsedTime += deltaTime;

                if (ElapsedTime >= closeAfter)
                {
                    Block.CloseDoor();
                }
            }
            else if (!IsOpen)
            {
                // Update State
                IsOpen = true;
                if (hasAirlockGroup)
                    AutoAirlock(airlockGroup);
            }
        }
        else if (Block.Status == DoorStatus.Closed)
        {
            if (IsOpen)
            {
                // Update State
                ElapsedTime = 0f;
                IsOpen = false;
                if (hasAirlockGroup)
                    AutoAirlock(airlockGroup);
            }
        }
    }

    public void AutoAirlock(string airlockGroup)
    {
        // Use isStateUpdate for basic mode (less calculations)

        Airlock airlock;
        if (AirlockList.TryGetValue(airlockGroup, out airlock))
        {
            if (!airlock.IsComplete())
                return;

            if (airlock.CountInteriorDoors > 0 && airlock.CountExteriorDoors > 0 && airlock.CountAirvents <= 0)
                BasicAirlock(airlock);
            /*
            else if (airlock.CountInteriorDoors > 0 && airlock.CountExteriorDoors > 0 && airlock.CountAirvents > 0 && !isStateUpdate)
                AdvancedAirlock(airlock);
            */
        }
    }

    public void BasicAirlock(Airlock airlock)
    {
        bool AnyInteriorOpen = airlock.AirlockDoorList.Any(D => D.Value.BlockProperties.GetBool("IsInterior") && (D.Value.Block.Status == DoorStatus.Open || D.Value.Block.Status == DoorStatus.Opening));
        bool AnyExteriorOpen = airlock.AirlockDoorList.Any(D => D.Value.BlockProperties.GetBool("IsExterior") && (D.Value.Block.Status == DoorStatus.Open || D.Value.Block.Status == DoorStatus.Opening));

        // Check Doors
        foreach (var block in airlock.AirlockDoorList)
        {
            if (block.Value.BlockProperties.GetBool("IsExterior"))
            {
                if (block.Value.Block.Enabled == AnyInteriorOpen)
                    block.Value.Block.Enabled = !AnyInteriorOpen;
            }
            else if (block.Value.BlockProperties.GetBool("IsInterior"))
            {
                if (block.Value.Block.Enabled == AnyExteriorOpen)
                    block.Value.Block.Enabled = !AnyExteriorOpen;
            }
        }

        // Lights
        foreach (var block in airlock.AirlockLightingBlockList)
        {
            var lockedColor = block.Value.BlockProperties.GetColor("LockedColor", LockedColor);
            var lockedBlinkInterval = block.Value.BlockProperties.GetFloat("LockedBlinkIntervalSeconds");
            var lockedBlinkLength = block.Value.BlockProperties.GetFloat("LockedBlinkLength");

            var unlockedColor = block.Value.BlockProperties.GetColor("UnlockedColor", LockedColor);
            var unlockedBlinkInterval = block.Value.BlockProperties.GetFloat("UnlockedBlinkIntervalSeconds");
            var unlockedBlinkLength = block.Value.BlockProperties.GetFloat("UnlockedBlinkLength");

            if (!block.Value.Block.Enabled)
                block.Value.Block.Enabled = true;

            if (block.Value.BlockProperties.GetBool("IsExterior") && !block.Value.BlockProperties.GetBool("IsInterior"))
            {
                // Light set to Exterior
                if (AnyInteriorOpen)
                {
                    if (block.Value.Block.Color != lockedColor)
                        block.Value.Block.Color = lockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != lockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = lockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != lockedBlinkLength)
                        block.Value.Block.BlinkLength = lockedBlinkLength;
                }
                else
                {
                    if (block.Value.Block.Color != unlockedColor)
                        block.Value.Block.Color = unlockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != unlockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = unlockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != unlockedBlinkLength)
                        block.Value.Block.BlinkLength = unlockedBlinkLength;
                }
            }
            else if (!block.Value.BlockProperties.GetBool("IsExterior") && block.Value.BlockProperties.GetBool("IsInterior"))
            {
                // Light set to Interior
                if (AnyExteriorOpen)
                {
                    if (block.Value.Block.Color != lockedColor)
                        block.Value.Block.Color = lockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != lockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = lockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != lockedBlinkLength)
                        block.Value.Block.BlinkLength = lockedBlinkLength;
                }
                else
                {
                    if (block.Value.Block.Color != unlockedColor)
                        block.Value.Block.Color = unlockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != unlockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = unlockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != unlockedBlinkLength)
                        block.Value.Block.BlinkLength = unlockedBlinkLength;
                }
            }
            else if (!block.Value.BlockProperties.GetBool("IsExterior") && !block.Value.BlockProperties.GetBool("IsInterior"))
            {
                // Lights that respond to both
                if (AnyExteriorOpen || AnyInteriorOpen)
                {
                    if (block.Value.Block.Color != lockedColor)
                        block.Value.Block.Color = lockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != lockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = lockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != lockedBlinkLength)
                        block.Value.Block.BlinkLength = lockedBlinkLength;
                }
                else
                {
                    if (block.Value.Block.Color != unlockedColor)
                        block.Value.Block.Color = unlockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != unlockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = unlockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != unlockedBlinkLength)
                        block.Value.Block.BlinkLength = unlockedBlinkLength;
                }
            }
        }
    }

}

class BlockAirventStatus
{
    public IMyAirVent Block;
    public string BlockType;
    public Properties BlockProperties;

    public BlockAirventStatus(IMyAirVent block)
    {
        Block = block;
        BlockType = block.GetType().Name;
        BlockProperties = CustomDataReader(block);
    }

    public void AutoAirlock()
    {
        string airlockGroup = BlockProperties.GetString("AirlockGroup");
        if (airlockGroup == null)
            return;

        Airlock airlock;
        if (AirlockList.TryGetValue(airlockGroup, out airlock))
        {
            if (!airlock.IsComplete())
                return;

            if (airlock.CountInteriorDoors > 0 && airlock.CountExteriorDoors > 0 && airlock.CountAirvents > 0)
                AdvancedAirlock(airlock);

        }
    }

    public void AdvancedAirlock(Airlock airlock)
    {
        bool RoomPressurized = airlock.AirlockAirVentList.Any(AL => AL.Value.Block.Status == VentStatus.Pressurized || AL.Value.Block.GetOxygenLevel() >= AL.Value.BlockProperties.GetFloat("MinimumLevelPressurized"));
        bool RoomDepressurized = airlock.AirlockAirVentList.Any(AL => AL.Value.Block.Status == VentStatus.Depressurized || AL.Value.Block.GetOxygenLevel() <= AL.Value.BlockProperties.GetFloat("MaximumLevelDepressurized"));
        bool AnyInteriorOpen = airlock.AirlockDoorList.Any(D => D.Value.BlockProperties.GetBool("IsInterior") && (D.Value.Block.Status == DoorStatus.Open || D.Value.Block.Status == DoorStatus.Opening));
        bool AnyExteriorOpen = airlock.AirlockDoorList.Any(D => D.Value.BlockProperties.GetBool("IsExterior") && (D.Value.Block.Status == DoorStatus.Open || D.Value.Block.Status == DoorStatus.Opening));

        // Check Doors
        foreach (var block in airlock.AirlockDoorList)
        {
            if (!airlock.IsEnabled)
            {
                if (block.Value.Block.Enabled != airlock.IsEnabled)
                    block.Value.Block.Enabled = airlock.IsEnabled;
            }
            else if (block.Value.BlockProperties.GetBool("IsExterior"))
            {
                if (block.Value.Block.Enabled != RoomDepressurized)
                    block.Value.Block.Enabled = RoomDepressurized;
            }
            else if (block.Value.BlockProperties.GetBool("IsInterior"))
            {
                if (block.Value.Block.Enabled != RoomPressurized)
                    block.Value.Block.Enabled = RoomPressurized;
            }
        }

        // Lights
        foreach (var block in airlock.AirlockLightingBlockList)
        {
            var lockedColor = block.Value.BlockProperties.GetColor("LockedColor", LockedColor);
            var lockedBlinkInterval = block.Value.BlockProperties.GetFloat("LockedBlinkIntervalSeconds");
            var lockedBlinkLength = block.Value.BlockProperties.GetFloat("LockedBlinkLength");

            var unlockedColor = block.Value.BlockProperties.GetColor("UnlockedColor", LockedColor);
            var unlockedBlinkInterval = block.Value.BlockProperties.GetFloat("UnlockedBlinkIntervalSeconds");
            var unlockedBlinkLength = block.Value.BlockProperties.GetFloat("UnlockedBlinkLength");

            if (block.Value.Block.Enabled != airlock.IsEnabled)
                block.Value.Block.Enabled = airlock.IsEnabled;

            if (airlock.IsEnabled == false)
            {
                continue;
            }
            else if (block.Value.BlockProperties.GetBool("IsExterior") && !block.Value.BlockProperties.GetBool("IsInterior"))
            {
                // Light set to Exterior
                if (RoomPressurized)
                {
                    if (block.Value.Block.Color != lockedColor)
                        block.Value.Block.Color = lockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != lockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = lockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != lockedBlinkLength)
                        block.Value.Block.BlinkLength = lockedBlinkLength;
                }
                else
                {
                    if (block.Value.Block.Color != unlockedColor)
                        block.Value.Block.Color = unlockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != unlockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = unlockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != unlockedBlinkLength)
                        block.Value.Block.BlinkLength = unlockedBlinkLength;
                }
            }
            else if (!block.Value.BlockProperties.GetBool("IsExterior") && block.Value.BlockProperties.GetBool("IsInterior"))
            {
                // Light set to Interior
                if (RoomDepressurized)
                {
                    if (block.Value.Block.Color != lockedColor)
                        block.Value.Block.Color = lockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != lockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = lockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != lockedBlinkLength)
                        block.Value.Block.BlinkLength = lockedBlinkLength;
                }
                else
                {
                    if (block.Value.Block.Color != unlockedColor)
                        block.Value.Block.Color = unlockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != unlockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = unlockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != unlockedBlinkLength)
                        block.Value.Block.BlinkLength = unlockedBlinkLength;
                }
            }
            else if (!block.Value.BlockProperties.GetBool("IsExterior") && !block.Value.BlockProperties.GetBool("IsInterior"))
            {
                // Lights that respond to both
                if (AnyInteriorOpen || AnyExteriorOpen)
                {
                    if (block.Value.Block.Color != lockedColor)
                        block.Value.Block.Color = lockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != lockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = lockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != lockedBlinkLength)
                        block.Value.Block.BlinkLength = lockedBlinkLength;
                }
                else
                {
                    if (block.Value.Block.Color != unlockedColor)
                        block.Value.Block.Color = unlockedColor;
                    if (block.Value.Block.BlinkIntervalSeconds != unlockedBlinkInterval)
                        block.Value.Block.BlinkIntervalSeconds = unlockedBlinkInterval;
                    if (block.Value.Block.BlinkLength != unlockedBlinkLength)
                        block.Value.Block.BlinkLength = unlockedBlinkLength;
                }
            }
        }

        // Airvents
        foreach (var block in airlock.AirlockAirVentList)
        {
            if (AnyInteriorOpen)
            {
                if (block.Value.Block.Depressurize) block.Value.Block.Depressurize = false;
            }
            else if (AnyExteriorOpen)
            {
                if (!block.Value.Block.Depressurize) block.Value.Block.Depressurize = true;
            }
        }
    }
}

class BlockLightStatus
{
    public IMyLightingBlock Block;
    public string BlockType;
    public Properties BlockProperties;

    public BlockLightStatus(IMyLightingBlock block)
    {
        Block = block;
        BlockType = block.GetType().Name;
        BlockProperties = CustomDataReader(block);
    }
}

class Airlock
{
    public string AirlockTag;
    public Dictionary<long, AirlockDoor> AirlockDoorList = new Dictionary<long, AirlockDoor>();
    public Dictionary<long, AirlockAirVent> AirlockAirVentList = new Dictionary<long, AirlockAirVent>();
    public Dictionary<long, AirlockLights> AirlockLightingBlockList = new Dictionary<long, AirlockLights>();

    public int CountInteriorDoors = 0;
    public int CountExteriorDoors = 0;
    public int CountAirvents = 0;
    public int CountLights = 0;
    public bool IsEnabled = false;

    public Airlock(string airlockTag)
    {
        AirlockTag = airlockTag;
    }

    public void Add(IMyDoor airlockDoor)
    {
        AirlockDoorList[airlockDoor.EntityId] = new AirlockDoor(airlockDoor);
    }

    public void Add(IMyAirVent airlockAirvent)
    {
        AirlockAirVentList[airlockAirvent.EntityId] = new AirlockAirVent(airlockAirvent);
    }

    public void Add(IMyLightingBlock airlockLight)
    {
        AirlockLightingBlockList[airlockLight.EntityId] = new AirlockLights(airlockLight);
    }

    public string GetStatus()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Managing airlock: {AirlockTag}");
        sb.AppendLine($"Contains {CountInteriorDoors} interior doors.");
        sb.AppendLine($"Contains {CountExteriorDoors} exterior doors.");
        sb.AppendLine($"Contains {CountAirvents} airvents.");
        sb.AppendLine($"Contains {CountLights} lights.");
        sb.AppendLine($"Airlock enabled: {IsEnabled}");

        if (CountInteriorDoors <= 0 && CountAirvents <= 0)
            sb.AppendLine($"[WARNING] Airlock needs atleast 1 interior door to function!");

        else if (CountExteriorDoors <= 0)
            sb.AppendLine($"[WARNING] Airlock needs atleast 1 exterior door to function!");

        else if (CountInteriorDoors > 0 && CountExteriorDoors > 0 && CountAirvents <= 0)
            sb.AppendLine($"Airlock is running in basic mode!");

        else if (CountInteriorDoors > 0 && CountExteriorDoors > 0 && CountAirvents > 0)
            sb.AppendLine($"Airlock is running in advanced mode controlled by airvents!");

        return sb.ToString();
    }

    public bool IsComplete()
    {
        UpdateProperties();

        if (CountInteriorDoors <= 0 && CountAirvents <= 0)
            return false;

        if (CountExteriorDoors <= 0)
            return false;

        return true;
    }

    public void UpdateProperties()
    {
        CountInteriorDoors = AirlockDoorList.Count(D => D.Value.BlockProperties.GetBool("IsInterior") && !D.Value.BlockProperties.GetBool("IsExterior"));
        CountExteriorDoors = AirlockDoorList.Count(D => !D.Value.BlockProperties.GetBool("IsInterior") && D.Value.BlockProperties.GetBool("IsExterior"));
        CountAirvents = AirlockAirVentList.Count;
        CountLights = AirlockLightingBlockList.Count;

        if (AirlockAirVentList.Count > 0)
        {
            bool check = false;
            foreach (var airvent in AirlockAirVentList)
            {
                if (airvent.Value.Block.Enabled)
                {
                    check = true;
                    break;
                }
            }
            IsEnabled = check;
        }
        else
        {
            IsEnabled = true;
        }
    }
}

class AirlockDoor
{
    public IMyDoor Block;
    public Properties BlockProperties;

    public AirlockDoor(IMyDoor block)
    {
        Block = block;

        BlockDoorStatus status;
        if (DoorStatusList.TryGetValue(block.EntityId, out status))
            BlockProperties = status.BlockProperties;
    }
}

class AirlockAirVent
{
    public IMyAirVent Block;
    public Properties BlockProperties;

    public AirlockAirVent(IMyAirVent block)
    {
        Block = block;
        BlockProperties = AirventStatusList.GetValueOrDefault(block.EntityId).BlockProperties ?? null;
    }
}

class AirlockLights
{
    public IMyLightingBlock Block;
    public Properties BlockProperties;

    public AirlockLights(IMyLightingBlock block)
    {
        Block = block;
        BlockProperties = LightStatusList.GetValueOrDefault(block.EntityId).BlockProperties ?? null;
    }
}

public Program()
{
    // The constructor, called only once every session and
    // always before any other method is called. Use it to
    // initialize your script. 
    //     
    // The constructor is optional and can be removed if not
    // needed.
    // 
    // It's recommended to set Runtime.UpdateFrequency 
    // here, which will allow your script to run itself without a 
    // timer block.

    // Setup Global Settings
    SettingEntry[] entriesDoor = new SettingEntry[]
    {
        new SettingEntry("UseGlobalSettings", UseGlobalSettings),
        new SettingEntry("AutoCloseDoor", AutoCloseDoors),
        new SettingEntry("CloseDoorAfter", CloseDoorsAfter),
        new SettingEntry("AirlockGroup", AirlockGroup),
        new SettingEntry("IsInterior", IsInterior),
        new SettingEntry("IsExterior", IsExterior),
    };
    GlobalDoorProperties = new Properties(entriesDoor);

    SettingEntry[] entriesHangar = new SettingEntry[]
    {
        new SettingEntry("UseGlobalSettings", UseGlobalSettings),
        new SettingEntry("AutoCloseHangarDoor", AutoCloseHangarDoors),
        new SettingEntry("CloseHangarDoorAfter", CloseHangarDoorsAfter),
        new SettingEntry("AirlockGroup", AirlockGroup),
        new SettingEntry("IsInterior", IsInterior),
        new SettingEntry("IsExterior", IsExterior),
    };
    GlobalHangarDoorProperties = new Properties(entriesHangar);

    SettingEntry[] entriesAirvent = new SettingEntry[]
    {
        new SettingEntry("UseGlobalSettings", UseGlobalSettings),
        new SettingEntry("AirlockGroup", AirlockGroup),
        new SettingEntry("MinimumLevelPressurized", MinimumLevelPressurized),
        new SettingEntry("MaximumLevelDepressurized", MaximumLevelDepressurized),
    };
    GlobalAirventProperties = new Properties(entriesAirvent);

    SettingEntry[] entriesLight = new SettingEntry[]
    {
        new SettingEntry("UseGlobalSettings", UseGlobalSettings),
        new SettingEntry("AirlockGroup", AirlockGroup),
        new SettingEntry("IsInterior", IsInterior),
        new SettingEntry("IsExterior", IsExterior),
        new SettingEntry("LockedColor", LockedColor),
        new SettingEntry("LockedBlinkIntervalSeconds", LockedBlinkIntervalSeconds),
        new SettingEntry("LockedBlinkLength", LockedBlinkLength),
        new SettingEntry("UnlockedColor", UnlockedColor),
        new SettingEntry("UnlockedBlinkIntervalSeconds", UnlockedBlinkIntervalSeconds),
        new SettingEntry("UnlockedBlinkLength", UnlockedBlinkLength),

    };
    GlobalLightProperties = new Properties(entriesLight);

    MyProgrammableBlockList.Add(Me);

    // Get All Blocks
    GetBlocks();

    // Set update frequency
    Runtime.UpdateFrequency = Update;
}

public void Main(string argument, UpdateType updateSource)
{
    if ((updateSource & (UpdateType.Update1 | UpdateType.Update10 | UpdateType.Update100)) != 0)
    {
        float intervalSeconds = GetIntervalSeconds(updateSource);

        AverageUsage = AverageUsage * 0.99 + Runtime.LastRunTimeMs * 0.01;
        AverageUsagePerSecond = AverageUsage / intervalSeconds;

        UpdateTick(intervalSeconds);

        int timeLeft;

        if (UpdateAllBlocksInterval > 0)
        {
            if (UpdateAllBlocks.NeedUpdate(intervalSeconds, out timeLeft))
                GetBlocks();
            else
                Echo($"Refreshing all blocks on grid in {timeLeft} seconds...\n");
        }

        // End > show status
        ShowStatus();
    }
    else if (!string.IsNullOrWhiteSpace(argument))
    {
        HandleCommand(argument);
    }
}

float GetIntervalSeconds(UpdateType updateSource)
{
    if ((updateSource & UpdateType.Update1) != 0) return 1f / 60f;
    if ((updateSource & UpdateType.Update10) != 0) return 10f / 60f;
    if ((updateSource & UpdateType.Update100) != 0) return 100f / 60f;
    return 0f;
}

void UpdateTick(float deltaTime)
{
    // tick-based logic here (e.g. countdowns, door checks)

    // Check doors
    foreach (var door in MyDoorList)
    {
        // Check for auto close/airlock
        BlockDoorStatus blockDoorStatus;
        if (DoorStatusList.TryGetValue(door.EntityId, out blockDoorStatus))
        {
            blockDoorStatus.AutoClose(deltaTime);
        }
    }

    // Check airvents
    foreach (var airvent in MyAirVentList)
    {
        BlockAirventStatus blockAirventStatus;
        if (AirventStatusList.TryGetValue(airvent.EntityId, out blockAirventStatus))
        {
            blockAirventStatus.AutoAirlock();
        }
    }
}

void HandleCommand(string arg)
{
    // argument-based logic here (e.g. "OpenAirlock")
}

void GetBlocks()
{
    DoorStatusList = new Dictionary<long, BlockDoorStatus>();
    AirventStatusList = new Dictionary<long, BlockAirventStatus>();
    LightStatusList = new Dictionary<long, BlockLightStatus>();
    AirlockList = new Dictionary<string, Airlock>();

    MyDoorList.Clear();
    MyAirVentList.Clear();
    MyLightingBlockList.Clear();

    // Doors
    GridTerminalSystem.GetBlocksOfType<IMyDoor>(MyDoorList, D => D.IsSameConstructAs(Me) && D.CubeGrid == Me.CubeGrid);
    foreach (var myDoor in MyDoorList)
    {
        // Make sure all setings are in custom data
        CustomDataWriter(myDoor);

        // Set initial state
        // myDoor.CloseDoor();

        // Create status
        BlockDoorStatus blockDoorStatus = new BlockDoorStatus(myDoor);
        DoorStatusList[myDoor.EntityId] = blockDoorStatus;

        // Get settings
        bool useGlobalSettings = blockDoorStatus.BlockProperties.GetBool("UseGlobalSettings", true);

        // Check for airlockgroup
        string airlockGroup = AirlockGroup;
        if (!useGlobalSettings)
            airlockGroup = blockDoorStatus.BlockProperties.GetString("AirlockGroup");

        if (!string.IsNullOrWhiteSpace(airlockGroup))
        {
            if (!AirlockList.ContainsKey(airlockGroup))
                AirlockList[airlockGroup] = new Airlock(airlockGroup);

            AirlockList[airlockGroup].Add(myDoor);
        }
    }

    // AirVents
    GridTerminalSystem.GetBlocksOfType<IMyAirVent>(MyAirVentList, AV => AV.CustomName.Contains(TagAirlock) && AV.IsSameConstructAs(Me) && AV.CubeGrid == Me.CubeGrid);
    foreach (var myAirVent in MyAirVentList)
    {
        // Make sure all setings are in custom data
        CustomDataWriter(myAirVent);

        // Set initial state
        // myAirVent.Depressurize = true;

        // Create status
        BlockAirventStatus blockAirventStatus = new BlockAirventStatus(myAirVent);
        AirventStatusList[myAirVent.EntityId] = blockAirventStatus;

        // Get settings
        bool useGlobalSettings = blockAirventStatus.BlockProperties.GetBool("UseGlobalSettings", true);

        // Check for airlockgroup
        string airlockGroup = AirlockGroup;
        if (!useGlobalSettings)
            airlockGroup = blockAirventStatus.BlockProperties.GetString("AirlockGroup");

        if (!string.IsNullOrWhiteSpace(airlockGroup))
        {
            if (!AirlockList.ContainsKey(airlockGroup))
                AirlockList[airlockGroup] = new Airlock(airlockGroup);

            AirlockList[airlockGroup].Add(myAirVent);
        }
    }

    // Lights
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(MyLightingBlockList, LB => LB.CustomName.Contains(TagAirlock) && LB.IsSameConstructAs(Me) && LB.CubeGrid == Me.CubeGrid);
    foreach (var myLight in MyLightingBlockList)
    {
        // Make sure all setings are in custom data
        CustomDataWriter(myLight);

        // Set initial state

        // Create status
        BlockLightStatus blockLightStatus = new BlockLightStatus(myLight);
        LightStatusList[myLight.EntityId] = blockLightStatus;

        // Get settings
        bool useGlobalSettings = blockLightStatus.BlockProperties.GetBool("UseGlobalSettings", true);

        // Check for airlockgroup
        string airlockGroup = AirlockGroup;
        if (!useGlobalSettings)
            airlockGroup = blockLightStatus.BlockProperties.GetString("AirlockGroup");

        if (!string.IsNullOrWhiteSpace(airlockGroup))
        {
            if (!AirlockList.ContainsKey(airlockGroup))
                AirlockList[airlockGroup] = new Airlock(airlockGroup);

            AirlockList[airlockGroup].Add(myLight);
        }
    }

}

void ShowStatus()
{
    Echo($"Managing {MyDoorList.Count} doors...");
    Echo($"Managing {MyAirVentList.Count} airvents...");
    Echo($"Managing {MyLightingBlockList.Count} lights...");
    Echo($"Managing {AirlockList.Count} airlocks...");

    foreach (var airlock in AirlockList)
    {
        Echo($"\n{airlock.Value.GetStatus()}");
    }

    Echo($"Average Runtime per second: {AverageUsagePerSecond} ms");
    Echo($"Average Runtime per update: {AverageUsage} ms");
    Echo($"Last Runtime: {Runtime.LastRunTimeMs} ms");
}

public static class UpdateAllBlocks
{
    public static float ElapsedTime = 0;

    static UpdateAllBlocks()
    {

    }

    public static bool NeedUpdate(float deltaTime, out int timeLeft)
    {
        ElapsedTime += deltaTime;
        timeLeft = 0;

        if (ElapsedTime >= UpdateAllBlocksInterval)
        {
            ElapsedTime = 0;
            return true;
        }

        timeLeft = (int)(UpdateAllBlocksInterval - ElapsedTime);
        return false;
    }
}

// =======================================================================================
// SETTINGS MANAGEMENT - DONT EDIT BELOW
// =======================================================================================

enum SettingType
{
    String,
    Bool,
    Int,
    Float,
    Color,
    Unknown
}

class SettingEntry
{
    public string Key;
    public SettingType T;
    public string StringValue;
    public bool? BoolValue;
    public int? IntValue;
    public float? FloatValue;
    public Color? ColorValue;

    public SettingEntry(string key, string value) { Key = key; T = SettingType.String; StringValue = value; BoolValue = null; IntValue = null; FloatValue = null; ColorValue = null; }
    public SettingEntry(string key, bool value) { Key = key; T = SettingType.Bool; StringValue = null; BoolValue = value; IntValue = null; FloatValue = null; ColorValue = null; }
    public SettingEntry(string key, int value) { Key = key; T = SettingType.Int; StringValue = null; BoolValue = null; IntValue = value; FloatValue = null; ColorValue = null; }
    public SettingEntry(string key, float value) { Key = key; T = SettingType.Float; StringValue = null; BoolValue = null; IntValue = null; FloatValue = value; ColorValue = null; }
    public SettingEntry(string key, Color value) { Key = key; T = SettingType.Color; StringValue = null; BoolValue = null; IntValue = null; FloatValue = null; ColorValue = value; }
    public SettingEntry(string key) { Key = key; T = SettingType.Unknown; StringValue = null; BoolValue = null; IntValue = null; FloatValue = null; ColorValue = null; }
}

class Properties
{
    public Dictionary<string, SettingEntry> Settings = new Dictionary<string, SettingEntry>();

    public Properties() { }

    public Properties(SettingEntry[] settingEntries)
    {
        foreach (var settingEntry in settingEntries)
        {
            Set(settingEntry);
        }
    }

    public Properties(Properties source)
    {
        foreach (var key in source.Keys)
        {
            Set(source.Get(key));
        }
    }

    public void Set(SettingEntry value)
    {
        Settings[value.Key] = value;
    }

    public SettingEntry Get(string key)
    {
        return Settings.ContainsKey(key) ? Settings[key] : new SettingEntry(key);
    }

    // Specifieke helpers voor gemak
    public string GetString(string key, string fallback = "")
    {
        var entry = Get(key);

        if (entry.StringValue != null)
        {
            return entry.StringValue;
        }
        else if (entry.BoolValue != null)
        {
            return entry.BoolValue.ToString();
        }
        else if (entry.IntValue != null)
        {
            return entry.IntValue.ToString();
        }
        else if (entry.FloatValue != null)
        {
            return entry.FloatValue.ToString();
        }
        else if (entry.ColorValue != null)
        {
            return entry.ColorValue.ToString();
        }

        return fallback;
    }

    public bool GetBool(string key, bool fallback = false)
    {
        var entry = Get(key);
        return entry.BoolValue ?? fallback;
    }

    public int GetInt(string key, int fallback = 0)
    {
        var entry = Get(key);
        return entry.IntValue ?? fallback;
    }

    public float GetFloat(string key, float fallback = 0f)
    {
        var entry = Get(key);
        return entry.FloatValue ?? fallback;
    }

    public Color GetColor(string key, Color fallback)
    {
        var entry = Get(key);
        return entry.ColorValue ?? fallback;
    }

    public IEnumerable<string> Keys => Settings.Keys;
}

static bool TryParseColor(string input, out Color color)
{
    color = new Color(); // default waarde
    string[] parts = input.Split(',');

    if (parts.Length != 4)
        return false;

    int r, g, b, a;

    if (int.TryParse(parts[0], out r) &&
        int.TryParse(parts[1], out g) &&
        int.TryParse(parts[2], out b) &&
        int.TryParse(parts[3], out a))
    {
        color = new Color(r, g, b, a);
        return true;
    }

    return false;
}

static Properties GetPropertiesByType(IMyTerminalBlock Block)
{
    Properties TypeProperties = null;

    string blockTypeName = Block.GetType().Name.ToLower();

    if (blockTypeName.Contains("hangar"))
    {
        TypeProperties = new Properties(GlobalHangarDoorProperties);
    }
    else if (blockTypeName.Contains("door"))
    {
        TypeProperties = new Properties(GlobalDoorProperties);
    }
    else if (blockTypeName.Contains("vent"))
    {
        TypeProperties = new Properties(GlobalAirventProperties);
    }
    else if (blockTypeName.Contains("light"))
    {
        TypeProperties = new Properties(GlobalLightProperties);
    }

    return TypeProperties;
}

static void CustomDataWriter(IMyTerminalBlock Block)
{
    // Clear CustomData (temporary reset for testing)
    // Block.CustomData = string.Empty;

    Properties Properties = GetPropertiesByType(Block);

    if (Properties == null) return;

    string[] blockCustomDataLines = Block.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    var sbBlockCustomData = new StringBuilder();
    var sbBlockSettings = new StringBuilder();

    // Check if settings header is already present
    bool hasHeader = blockCustomDataLines.Any(line => line.Trim() == SettingsHeader);

    if (!hasHeader)
    {
        // No settings header found > preserve all non-empty lines as general CustomData
        foreach (string line in blockCustomDataLines)
        {
            if (!string.IsNullOrWhiteSpace(line))
                sbBlockCustomData.AppendLine(line);
        }

        // Start a fresh settings section
        sbBlockSettings.AppendLine(SettingsHeader);
    }
    else
    {
        // Settings header found > split CustomData into general data and settings
        int headerIndex = Array.FindIndex(blockCustomDataLines, l => l.Trim() == SettingsHeader);
        int settingsEndIndex = -1;

        for (int i = 0; i < blockCustomDataLines.Length; i++)
        {
            string line = blockCustomDataLines[i];

            if (i < headerIndex)
            {
                // Everything before the header goes to general CustomData
                sbBlockCustomData.AppendLine(line);
            }
            else if (i == headerIndex)
            {
                // Add the header itself to the settings block
                sbBlockSettings.AppendLine(line);
            }
            else if (settingsEndIndex == -1)
            {
                // Read settings until an empty or invalid line is found
                if (string.IsNullOrWhiteSpace(line) || !line.Contains(":"))
                {
                    settingsEndIndex = i;
                    sbBlockCustomData.AppendLine(line);
                }
                else
                {
                    sbBlockSettings.AppendLine(line);
                }
            }
            else
            {
                // Everything after the settings block continues as general CustomData
                sbBlockCustomData.AppendLine(line);
            }
        }
    }

    // Parse current settings to find existing keys
    var existingKeys = sbBlockSettings.ToString()
        .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
        .Where(l => l.Contains(":"))
        .Select(l => l.Split(':')[0])
        .ToHashSet();

    // Append any missing settings keys from GlobalProperties
    foreach (string key in Properties.Keys)
    {
        if (!existingKeys.Contains(key))
            sbBlockSettings.AppendLine(key + ":" + Properties.GetString(key));
    }

    // Combine everything and assign to CustomData
    if (string.IsNullOrWhiteSpace(sbBlockCustomData.ToString()))
        Block.CustomData = sbBlockSettings.ToString().TrimEnd();
    else
        Block.CustomData = sbBlockCustomData.ToString().TrimEnd() + "\n\n" + sbBlockSettings.ToString().TrimEnd();
}

static Properties CustomDataReader(IMyTerminalBlock Block)
{
    Properties properties = GetPropertiesByType(Block);

    if (properties == null) return null;

    string[] blockCustomDataLines = Block.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    var sbBlockSettings = new StringBuilder();

    // Check if settings header is already present
    bool hasHeader = blockCustomDataLines.Any(line => line.Trim() == SettingsHeader);

    if (!hasHeader)
        return null;

    int headerIndex = Array.FindIndex(blockCustomDataLines, l => l.Trim() == SettingsHeader);

    for (int i = headerIndex + 1; i < blockCustomDataLines.Length; i++)
    {
        var line = blockCustomDataLines[i];

        if (string.IsNullOrWhiteSpace(line) || !line.Contains(":"))
            break;

        string key = line.Split(':')[0].Trim();
        string rawValue = line.Split(':')[1].Trim();

        SettingEntry globalBlockSetting = properties.Get(key);

        // Check Type and cast to
        switch (globalBlockSetting.T)
        {
            case SettingType.Unknown:
                continue;
            // break;

            case SettingType.String:
                properties.Set(new SettingEntry(key, rawValue));
                break;

            case SettingType.Bool:
                bool boolVal;
                if (bool.TryParse(rawValue, out boolVal))
                    properties.Set(new SettingEntry(key, boolVal));
                break;

            case SettingType.Int:
                int intVal;
                if (int.TryParse(rawValue, out intVal))
                    properties.Set(new SettingEntry(key, intVal));
                break;

            case SettingType.Float:
                float floatVal;
                if (float.TryParse(rawValue, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out floatVal))
                    properties.Set(new SettingEntry(key, floatVal));
                break;

            case SettingType.Color:
                Color colorVal;
                if (TryParseColor(rawValue, out colorVal))
                    properties.Set(new SettingEntry(key, colorVal));
                break;
        }
    }

    if (properties.GetBool("UseGlobalSettings"))
        properties = GetPropertiesByType(Block);

    return properties;
}
