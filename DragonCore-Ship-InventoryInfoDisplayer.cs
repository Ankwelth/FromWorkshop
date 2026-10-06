// Tags:
// [DCPM] for production blocks
// [LCD] for displays
// [REF] for a seat or remote reference

//===================SETTINGS===================//

// Auto Close Doors: Here you can set if you want Hangar Door to be ignored by default and you can also Exclude doors.
readonly bool ignoreHangarDoors = true;     // true = Ingore Hangar Doors, false = Don't ingore Hangar Doors
readonly string doorExcludeStr = "Exclude"; // Excluded doors should contain this in ther name.

// Auto Door Settings: Here you can set how long a door should be open before the scripts closes it automatically.
// These settings are in seconds
readonly double doorTimerReset = 5;    // For Doors
readonly double hangarTimerReset = 30; // For Hangar Doors

// Here you can change the Tag for blocks to be searched:
// Reference seat alternative tag:
const string altRefName = "!Ref!"; // Alternative Tag

// LCD/Panel alternative tag:
const string altPanelName = "!LCD!"; // Alternative Tag

// Power Managment System: Here you can toggle PMS. (true = use PMS, false = don't use PMS)
readonly bool usePMS = false;

// Battery Charging Settings: The script will automatically charge your batteries when needed.
// (The values are multiplied by 100 so 0.9 = 90%)
// Do NOT delete the "F" after the values.
readonly float chargeOff = 0.9F; // = Turn off generators
readonly float chargeOn = 0.3F;  // = Turn on generators

// LCD Settings: Here you can set the percentage for the automatic danger pop-up on your LCDs.
// (The values are multiplied by 100 so 0.1 = 10%)
readonly bool ShowLowGasLevel = true;
readonly float SpriteDangerLevel = 0.1F;

// Script Settings: This is a small time period where the scripts grabs the first data
// and is hidden by a load screen, this can be disabled here.
readonly bool DoLoad = true;

// If you want the script to search other connected grids too, set this to true:
readonly bool SearchConnectedGrids = false;

//===================END OF SETTINGS===================//













readonly IEnumerator<bool>[] StateMachines = new IEnumerator<bool>[6];

readonly List<IMyTerminalBlock> AllBlocks = new List<IMyTerminalBlock>();
readonly List<IMyShipController> Controllers = new List<IMyShipController>();
readonly List<IMyThrust> AllThrust = new List<IMyThrust>();
readonly List<IMyThrust> UpwardsThrust = new List<IMyThrust>();
readonly List<IMyTerminalBlock> PowerProducers = new List<IMyTerminalBlock>();
readonly List<IMyGasTank> GasTanks = new List<IMyGasTank>();
readonly List<DoorPair> DoorPairs = new List<DoorPair>();
readonly List<SpritePair> LCDSpritePairs = new List<SpritePair>();
readonly List<IMyAssembler> Controller_Assemblers = new List<IMyAssembler>();

readonly List<IMyTerminalBlock> CargoList = new List<IMyTerminalBlock>();
readonly List<IMyProductionBlock> ProductionBlockList = new List<IMyProductionBlock>();
readonly List<InventoryStorage> CargoStorageList = new List<InventoryStorage>();
readonly List<ItemStorage> GridInv = new List<ItemStorage>();
List<ItemProductionTag> LowIngotList = new List<ItemProductionTag>();

readonly Dictionary<string, string> ItemNameReplaceDict = new Dictionary<string, string>();
readonly Dictionary<string, int> ItemLowLimitsDict = new Dictionary<string, int>();
readonly Dictionary<string, string> ProdItemDict = new Dictionary<string, string>();
readonly List<string> ALLOWED_TYPES = new List<string> { "AmmoMagazine", "Component", "Ore", "Ingot", "PhysicalGunObject" };

IMyShipController Reference;
IMyAssembler Main_Assembler = null;

readonly float[] PowerData = new float[16];

readonly float[] GasData = new float[4];

readonly double[] ThrustData = new double[2];

readonly MyFixedPoint[] CargoData = new MyFixedPoint[2];

bool charge = false;
bool Loading = true;
bool ScriptResetBool = false;
bool SpriteDangerOn = false;
bool SilentRun = false;
const bool ForceRefineryControl = false;

Vector3D Gravity = new Vector3D();

// Debug && run animation int
int HighestInstructionCount = 0;

// Enumerator limits
readonly int MaxPowerProducerCount = 10;
readonly int MaxCargoContainerCount = 10;
readonly int MaxGasTankCount = 10;
readonly int MaxThrusterCount = 10;
readonly int MaxSortCount = 20;
readonly int MaxCargoCount = 10;

// Display scrolls
ScrollController Scroll_Comp = new ScrollController("Component");
ScrollController Scroll_Ammo = new ScrollController("AmmoMagazine");
ScrollController Scroll_Tool = new ScrollController("PhysicalGunObject");
ScrollController Scroll_Ore = new ScrollController("Ore");
ScrollController Scroll_Ingot = new ScrollController("Ingot");

int ComponentDisplayScroll = 0;
int ToolDisplayScroll = 0;
int AmmoDisplayScroll = 0;
int IngotDisplayScroll = 0;
int OreDisplayScroll = 0;
const int ItemPixelLength = 38;

// Time containers
TimeSpan RunTimeMain;
TimeSpan loadTime = TimeSpan.Zero;
TimeSpan SpriteDangerTimer = TimeSpan.Zero;
TimeSpan SpriteDangerTimerReset = TimeSpan.FromSeconds(1);
TimeSpan DoorReset = new TimeSpan();
TimeSpan HangarReset = new TimeSpan();
TimeSpan RefreshTimer = new TimeSpan();

float CapableCargoLift = 0;
readonly float[] percentageArray = new float[12];

MyShipMass ShipMass;

const string BP_DEF_TAG = "MyObjectBuilder_BlueprintDefinition/";
const string PRODUCTION_TAG = "[DCPM]";
const string panelName = "[LCD]";
const string refName = "[Ref]";
readonly string dateVersion = "Version: 5.0.0 ALPHA 24/04/02"; // YY/MM/DD

// Positions for Graphs
Vector2 startPos = new Vector2(20, 80);
Vector2 endPos = new Vector2(500, 500);

Vector2 pos2_2 = new Vector2(245, 500);
Vector2 pos3_2 = new Vector2(255, 80);

Vector2 pos2_3 = new Vector2(170, 500);
Vector2 pos3_3 = new Vector2(180, 80);
Vector2 pos4_3 = new Vector2(330, 500);
Vector2 pos5_3 = new Vector2(340, 80);

Vector2 pos2_4 = new Vector2(130, 500);
Vector2 pos3_4 = new Vector2(140, 80);
Vector2 pos4_4 = new Vector2(250, 500);
Vector2 pos5_4 = new Vector2(260, 80);
Vector2 pos6_4 = new Vector2(370, 500);
Vector2 pos7_4 = new Vector2(380, 80);

Color MainTextColor = new Color(Color.Gray.ToVector3());
Color MainBarColor = new Color(Color.Gray.ToVector3());
Color MainInvTextColor = new Color(Color.Gray.ToVector3());
Color MainInvAmountColor = new Color(Color.Yellow.ToVector3());

public Program()
{
    DoorReset = new TimeSpan((long)doorTimerReset * TimeSpan.TicksPerSecond);
    HangarReset = new TimeSpan((long)hangarTimerReset * TimeSpan.TicksPerSecond);

    ItemNameReplaceDict.Add("AutomaticRifleGun_Mag_20rd", "MR-20 Magazine");
    ItemNameReplaceDict.Add("PreciseAutomaticRifleGun_Mag_5rd", "MR-8P Magazine");
    ItemNameReplaceDict.Add("RapidFireAutomaticRifleGun_Mag_50rd", "MR-50A Magazine");
    ItemNameReplaceDict.Add("UltimateAutomaticRifleGun_Mag_30rd", "MR-30E Magazine");
    ItemNameReplaceDict.Add("SemiAutoPistolMagazine", "S-10 Magazine");
    ItemNameReplaceDict.Add("FullAutoPistolMagazine", "S-20A Magazine");
    ItemNameReplaceDict.Add("ElitePistolMagazine", "S-10E Magazine");

    ItemNameReplaceDict.Add("LargeCalibreAmmo", "Artillery Shell");
    ItemNameReplaceDict.Add("MediumCalibreAmmo", "Assault Shell");
    ItemNameReplaceDict.Add("LargeRailgunAmmo", "Large Sabot");
    ItemNameReplaceDict.Add("SmallRailgunAmmo", "Small Sabot");
    ItemNameReplaceDict.Add("AutocannonClip", "Autocannon Clip");

    ItemNameReplaceDict.Add("AngleGrinderItem", "Grinder");
    ItemNameReplaceDict.Add("AngleGrinder2Item", "Grinder mk.1");
    ItemNameReplaceDict.Add("AngleGrinder3Item", "Grinder mk.2");
    ItemNameReplaceDict.Add("AngleGrinder4Item", "Grinder mk.3");

    ItemNameReplaceDict.Add("HandDrillItem", "Hand Drill");
    ItemNameReplaceDict.Add("HandDrill2Item", "Hand Drill mk.1");
    ItemNameReplaceDict.Add("HandDrill3Item", "Hand Drill mk.2");
    ItemNameReplaceDict.Add("HandDrill4Item", "Hand Drill mk.3");

    ItemNameReplaceDict.Add("WelderItem", "Welder");
    ItemNameReplaceDict.Add("Welder2Item", "Welder mk.1");
    ItemNameReplaceDict.Add("Welder3Item", "Welder mk.2");
    ItemNameReplaceDict.Add("Welder4Item", "Welder mk.3");

    ItemNameReplaceDict.Add("SemiAutoPistolItem", "S-10 Pistol");
    ItemNameReplaceDict.Add("FullAutoPistolItem", "S-20A Pistol");
    ItemNameReplaceDict.Add("ElitePistolItem", "S-10E Pistol");
    ItemNameReplaceDict.Add("AutomaticRifleItem", "MR-20 Rifle");
    ItemNameReplaceDict.Add("PreciseAutomaticRifleItem", "MR-8P Rifle");
    ItemNameReplaceDict.Add("RapidFireAutomaticRifleItem", "MR-50A Rifle");
    ItemNameReplaceDict.Add("UltimateAutomaticRifleItem", "MR-30E Rifle");
    ItemNameReplaceDict.Add("BasicHandHeldLauncherItem", "RO-1 Laucher");
    ItemNameReplaceDict.Add("AdvancedHandHeldLauncherItem", "PRO-1 Laucher");

    ItemNameReplaceDict.Add("RadioCommunication", "Radio");

    ItemNameReplaceDict.Add("FireworksBoxBlue", "Fireworks (B)");
    ItemNameReplaceDict.Add("FireworksBoxGreen", "Fireworks (G)");
    ItemNameReplaceDict.Add("FireworksBoxPink", "Fireworks (P)");
    ItemNameReplaceDict.Add("FireworksBoxRainbow", "Fireworks (RGB)");
    ItemNameReplaceDict.Add("FireworksBoxRed", "Fireworks (R)");
    ItemNameReplaceDict.Add("FireworksBoxYellow", "Fireworks (Y)");

    // Lists from lowest to highest limit
    // Ingot limits
    ItemLowLimitsDict.Add("Silver", 1000);
    ItemLowLimitsDict.Add("Uranium", 1000);
    ItemLowLimitsDict.Add("Platinum", 1000);
    ItemLowLimitsDict.Add("Gold", 2000);
    ItemLowLimitsDict.Add("Magnesium", 2000);
    ItemLowLimitsDict.Add("Silicon", 2000);
    ItemLowLimitsDict.Add("Stone", 4000);
    ItemLowLimitsDict.Add("Cobalt", 5000);
    ItemLowLimitsDict.Add("Nickel", 5000);
    ItemLowLimitsDict.Add("Iron", 20000);

    // Component limits
    ItemLowLimitsDict.Add("Canvas", 20);
    ItemLowLimitsDict.Add("GravityGenerator", 20);
    ItemLowLimitsDict.Add("Medical", 30);
    ItemLowLimitsDict.Add("Detector", 50);
    ItemLowLimitsDict.Add("Explosives", 50);
    ItemLowLimitsDict.Add("RadioCommunication", 80);
    ItemLowLimitsDict.Add("SolarCell", 200);
    ItemLowLimitsDict.Add("Superconductor", 200);
    ItemLowLimitsDict.Add("PowerCell", 400);
    ItemLowLimitsDict.Add("LargeTube", 500);
    ItemLowLimitsDict.Add("Reactor", 500);
    ItemLowLimitsDict.Add("Display", 1000);
    ItemLowLimitsDict.Add("Computer", 1000);
    ItemLowLimitsDict.Add("Girder", 1000);
    ItemLowLimitsDict.Add("BulletproofGlass", 1000);
    ItemLowLimitsDict.Add("Thrust", 1000);
    ItemLowLimitsDict.Add("MetalGrid", 2000);
    ItemLowLimitsDict.Add("Motor", 2000);
    ItemLowLimitsDict.Add("SmallTube", 2000);
    ItemLowLimitsDict.Add("InteriorPlate", 5000);
    ItemLowLimitsDict.Add("SteelPlate", 10000);
    ItemLowLimitsDict.Add("Construction", 10000);

    ItemLowLimitsDict.Add("LargeCalibreAmmo", 50);
    ItemLowLimitsDict.Add("MediumCalibreAmmo", 50);
    ItemLowLimitsDict.Add("AutocannonClip", 80);
    ItemLowLimitsDict.Add("NATO_25x184mm", 80);
    ItemLowLimitsDict.Add("Missile200mm", 50);
    ItemLowLimitsDict.Add("SmallRailgunAmmo", 20);
    ItemLowLimitsDict.Add("LargeRailgunAmmo", 20);

    ProdItemDict.Add("LargeCalibreAmmo", "Position0120_LargeCalibreAmmo");
    ProdItemDict.Add("MediumCalibreAmmo", "Position0110_MediumCalibreAmmo");
    ProdItemDict.Add("AutocannonClip", "Position0090_AutocannonClip");
    ProdItemDict.Add("NATO_25x184mm", "Position0080_NATO_25x184mmMagazine");
    ProdItemDict.Add("Missile200mm", "Position0100_Missile200mm");
    ProdItemDict.Add("SmallRailgunAmmo", "Position0130_SmallRailgunAmmo");
    ProdItemDict.Add("LargeRailgunAmmo", "Position0140_LargeRailgunAmmo");

    // Same names should return item name
    ProdItemDict.Add("Canvas", "Position0030_Canvas");
    ProdItemDict.Add("Computer", "ComputerComponent");
    ProdItemDict.Add("Construction", "ConstructionComponent");
    ProdItemDict.Add("Detector", "DetectorComponent");
    ProdItemDict.Add("Explosives", "ExplosivesComponent");
    ProdItemDict.Add("Girder", "GirderComponent");
    ProdItemDict.Add("GravityGenerator", "GravityGeneratorComponent");
    ProdItemDict.Add("Medical", "MedicalComponent");
    ProdItemDict.Add("Motor", "MotorComponent");
    ProdItemDict.Add("RadioCommunication", "RadioCommunicationComponent");
    ProdItemDict.Add("Reactor", "ReactorComponent");
    ProdItemDict.Add("Thrust", "ThrustComponent");


    if (DoLoad)
    {
        loadTime = TimeSpan.FromSeconds(2);
    }
    else
    {
        Loading = false;
    }

    SetStateMachines();

    GrabBlocks();

    Runtime.UpdateFrequency |= UpdateFrequency.Once;

    Runtime.UpdateFrequency |= UpdateFrequency.Update10;
}

public void GrabBlocks()
{
    if (SearchConnectedGrids)
    {
        GridTerminalSystem.GetBlocksOfType(AllBlocks, b => b.IsFunctional);
    }
    else
    {
        GridTerminalSystem.GetBlocksOfType(AllBlocks, b => b.IsSameConstructAs(Me) && b.IsFunctional);
    }

    PowerProducers.Clear();
    Controllers.Clear();
    AllThrust.Clear();
    DoorPairs.Clear();
    LCDSpritePairs.Clear();
    GasTanks.Clear();
    CargoList.Clear();
    Controller_Assemblers.Clear();

    CargoStorageList.Clear();
    ProductionBlockList.Clear();

    List<string> allowed_types = new List<string>();
    foreach (var block in AllBlocks)
    {
        if (block is IMyPowerProducer) PowerProducers.Add(block);
        if (block is IMyShipController && (block.CustomName.Contains(refName) || block.CustomName.Contains(altRefName)))
            Controllers.Add((IMyShipController)block);
        if (block is IMyThrust) AllThrust.Add((IMyThrust)block);
        if (block is IMyDoor && ShouldGrabDoor(block))
        {
            DoorPair dp;
            var b = (IMyDoor)block;
            if (b is IMyAirtightHangarDoor)
                dp = new DoorPair(b, HangarReset, true);
            else
                dp = new DoorPair(b, DoorReset, false);
            DoorPairs.Add(dp);
        }
        if (block is IMyTextPanel && (block.CustomName.Contains(panelName) || block.CustomName.Contains(altPanelName)))
        {
            var b = (IMyTextPanel)block;
            SpritePair sp = new SpritePair(new List<MySprite>(), b);
            LCDSpritePairs.Add(sp);
        }
        if (block is IMyGasTank) GasTanks.Add((IMyGasTank)block);
        if (block is IMyAssembler && block.CustomName.Contains(PRODUCTION_TAG))
        {
            var asm = (IMyAssembler)block;
            if(!asm.CooperativeMode) Controller_Assemblers.Add(asm);
        }
        if (IsImportantInv(block))
        {
            CargoList.Add(block);
            if (block is IMyProductionBlock) ProductionBlockList.Add((IMyProductionBlock)block);

            if (block is IMyCargoContainer)
            {
                var b = (IMyCargoContainer)block;

                allowed_types = new List<string>();
                foreach (var t_check in ALLOWED_TYPES) if (b.CustomName.Contains(t_check)) allowed_types.Add(t_check);

                var inv = b.GetInventory();
                if (inv != null)
                {
                    CargoStorageList.Add(new InventoryStorage(inv, allowed_types, block.CustomName));
                }
            }
        }
    }
    if (Reference != null) GetUpwardsThrust(Reference, AllThrust, UpwardsThrust);
    if (Controller_Assemblers.Count != 0) Main_Assembler = Controller_Assemblers[0];
}

public void Main(string argument, UpdateType updateType)
{
    RunTimeMain = Runtime.TimeSinceLastRun;
    RefreshTimer += RunTimeMain;
    if (loadTime > TimeSpan.Zero) loadTime -= RunTimeMain; else { loadTime = TimeSpan.Zero; Loading = false; }

    if (RefreshTimer.TotalSeconds >= 10)
    {
        RefreshTimer = TimeSpan.Zero;
        ScriptResetBool = true;
    }

    if (ShowLowGasLevel)
    {
        SpriteDangerTimer -= Runtime.TimeSinceLastRun;
        if (SpriteDangerTimer <= TimeSpan.Zero)
        {
            SpriteDangerOn = !SpriteDangerOn;
            SpriteDangerTimer = SpriteDangerTimerReset;
        }
    }
    else
    {
        SpriteDangerOn = false;
    }

    ComponentDisplayScroll = Scroll_Comp.Run(this, GridInv);
    AmmoDisplayScroll = Scroll_Ammo.Run(this, GridInv);
    ToolDisplayScroll = Scroll_Tool.Run(this, GridInv);
    IngotDisplayScroll = Scroll_Ingot.Run(this, GridInv);
    OreDisplayScroll = Scroll_Ore.Run(this, GridInv);
                
    // Handle Arguments
    switch (argument.ToLower())
    {
        default:
            break;
        case "silent":
            SilentRun = !SilentRun;
            break;
        case "refresh":
            ScriptResetBool = true;
            break;
    }

    AutoUpdateDoor(DoorPairs);

    /// <summary>
    /// Runs all State Machines.
    /// </summary>
    if ((updateType & UpdateType.Once) == UpdateType.Once)
    {
        RunTimeMain = Runtime.TimeSinceLastRun;
        if (RunStateMachines(false))
        {
            Runtime.UpdateFrequency |= UpdateFrequency.Once;
        }
        else
        {
            if (!ScriptResetBool)
            {
                for (int i = 0; i < StateMachines.Length - 1; i++)
                {
                    StateMachines[i].Dispose();
                }

                SetStateMachines();

                Runtime.UpdateFrequency |= UpdateFrequency.Once;
            }
        }
    }
    else
    {
        Runtime.UpdateFrequency |= UpdateFrequency.Once;
    }

    /// <summary>
    /// Safely reset State Machines if argument "Refresh" is run. | Big thanks to Inflex for helping solve this issue.
    /// </summary>
    if (ScriptResetBool && RunStateMachines(true))
    {
        foreach (var machine in StateMachines)
            if (machine != null) machine.Dispose();

        GrabBlocks();

        SetStateMachines();

        Runtime.UpdateFrequency |= UpdateFrequency.Once;

        Runtime.UpdateFrequency |= UpdateFrequency.Update10;

        ScriptResetBool = false;
    }

    if ((updateType & UpdateType.Update10) == UpdateType.Update10)
    {
        if (Reference != null)
        {
            ShipMass = Reference.CalculateShipMass();
            Gravity = Reference.GetNaturalGravity();

            if (UpwardsThrust == null || UpwardsThrust.Count() < 1)
                GetUpwardsThrust(Reference, AllThrust, UpwardsThrust);
        }
        else if (Controllers.Count() >= 1) Reference = Controllers[0];

        if (usePMS) PMS();

        // Calc
        if (CargoData[0].RawValue > 0)
        {
            var max = CargoData[0].RawValue * 0.001F;
            var cur = CargoData[1].RawValue * 0.001F;

            percentageArray[0] = cur / max;
        }

        float hydMaxCap = GasData[0];
        float hydCurrCap = GasData[1];
        float hydPercentage = hydCurrCap / hydMaxCap;

        float oxyMaxCap = GasData[2];
        float oxyCurrCap = GasData[3];
        float oxyPercentage = oxyCurrCap / oxyMaxCap;

        if (!float.IsNaN(oxyPercentage)) percentageArray[1] = oxyPercentage;
        else percentageArray[1] = 0;

        if (!float.IsNaN(hydPercentage)) percentageArray[2] = hydPercentage;
        else percentageArray[2] = 0;

        if (ThrustData[0] > 0)
        {
            float p = (float)ThrustData[1] / (float)ThrustData[0];
            percentageArray[3] = p;

            CapableCargoLift = (float)ThrustData[0] / (float)Gravity.Length();

            if (CargoData[0].RawValue > 0 && CargoData.Count() == 2)
            {
                float CurrentWeight = ShipMass.PhysicalMass;

                if (CurrentWeight < CapableCargoLift)
                {
                    percentageArray[4] = CurrentWeight / CapableCargoLift;
                }
                else
                {
                    percentageArray[4] = 1;
                }
            }
        }

        percentageArray[5] = PowerData[1] / PowerData[0]; // Charge
        percentageArray[6] = PowerData[3] / PowerData[2]; // Input
        percentageArray[7] = PowerData[5] / PowerData[4]; // Output

        if (PowerData[10] != 0) percentageArray[8] = PowerData[11] / PowerData[10]; // Solar
        else percentageArray[8] = 0;

        if (PowerData[12] != 0) percentageArray[9] = PowerData[13] / PowerData[12]; // Turbine
        else percentageArray[9] = 0;

        if (PowerData[6] != 0) percentageArray[10] = PowerData[7] / PowerData[6]; // Reactor
        else percentageArray[10] = 0;

        if (PowerData[8] != 0) percentageArray[11] = PowerData[9] / PowerData[8]; // Engine
        else percentageArray[11] = 0;

        foreach (SpritePair pair in LCDSpritePairs)
        {
            IMyTextSurfaceProvider provider = (IMyTextSurfaceProvider)pair.TextPanel;
            IMyTextSurface surface = provider.GetSurface(0);
            PrepareSurface(surface);
            RectangleF viewPort = GetDrawingSurface(surface);
            DrawFrame(surface, viewPort, pair.TextPanel.CustomData, pair.Sprites);
        }

        if (!SilentRun)
        {
            var instruction_count = Runtime.CurrentInstructionCount;
            if (instruction_count > HighestInstructionCount) HighestInstructionCount = instruction_count;
            Echo($"Dragon Core\n{dateVersion}\n\n" +
                $"Block Resfresh In {10 - RefreshTimer.Seconds}s\n\n" +
                $"----- PB Runtime Info -----\n" +
                $"Instruction Count: {instruction_count}\n" +
                $"Highest Instr. Count: {HighestInstructionCount}\n" +
                $"[[ms]] Time Since Last Run: {Runtime.TimeSinceLastRun.Milliseconds}\n" +
                $"[[ms]] Last Runtime: {Runtime.LastRunTimeMs}");
            // Errors
            if (Controllers.Count == 0) Echo($"\nWarning: No controller found with name containing: [{refName}] or {altRefName}");
            if (Main_Assembler == null) Echo($"Warning: No main assembler found with tag: [{PRODUCTION_TAG}]");
        }
    }
}


public class ScrollController
{
    public int ScrollCounter { get; private set; } = 0;
            
    public int ItemCounter { get; private set; } = 0;

    private TimeSpan WaitTime = TimeSpan.Zero;
    private bool ScrollSwitch = true; // True -> scroll down
    private string SearchType = string.Empty;

    public ScrollController(string type)
    { 
        SearchType = type;
    }

    public int Run(MyGridProgram program, List<ItemStorage> gridInv)
    {
        int itemTypeCount = 0;
        foreach (var item in gridInv) if (item.Type == SearchType) itemTypeCount++;
        RunScrollFunc(itemTypeCount, program);
        return ScrollCounter;
    }

    public void RunScrollFunc(int item_count, MyGridProgram program)
    {
        if (WaitTime >= TimeSpan.Zero) WaitTime -= program.Runtime.TimeSinceLastRun;
        // 38 = ItemPixelLength
        if (ScrollCounter + 512 < 512 + ((item_count - 11) * 38) && WaitTime <= TimeSpan.Zero && ScrollSwitch)
        {
            ScrollCounter += 2;
        }
        else if (ScrollCounter >= 0 && WaitTime <= TimeSpan.Zero && !ScrollSwitch)
        {
            ScrollCounter -= 2;
        }
        else if (WaitTime <= TimeSpan.Zero)
        {
            WaitTime = TimeSpan.FromSeconds(2);
            ScrollSwitch = !ScrollSwitch;
        }
    }

}

public void GetUpwardsThrust(IMyShipController referenceBlock, List<IMyThrust> allThrusters, List<IMyThrust> upwardsThrust)
{
    var reference = referenceBlock.WorldMatrix.Up;
    upwardsThrust.Clear();

    foreach (IMyThrust thisBlock in allThrusters)
    {
        var thrusterOrientation = thisBlock.WorldMatrix.Backward;
        var same = thrusterOrientation == reference;

        if (same)
        {
            upwardsThrust.Add(thisBlock);
        }
    }
}

/// <summary>
/// Gets thrust from upwards thrusters
/// </summary>
public IEnumerator<bool> GetThrustPower(List<IMyThrust> list)
{
    IMyThrust block;
    int selector = 0;
    int counter = 0;

    double maxThrust = 0, currentThrust = 0;

    while (list.Count > 0 && selector < list.Count && MaxThrusterCount > counter)
    {
        counter++;

        block = list[selector];

        maxThrust += block.MaxEffectiveThrust;
        currentThrust += block.CurrentThrust;

        selector++;

        if (counter == MaxThrusterCount)
        {
            counter = 0;
            yield return true;
        }
    }

    if (selector >= list.Count - 1)
    {
        ThrustData.SetValue(maxThrust, 0);
        ThrustData.SetValue(currentThrust, 1);
    }
    else
    {
        yield return true;
    }
}

/// <summary>
/// Gets all data from Power Producers
/// </summary>
public IEnumerator<bool> GetPowerData()
{
    IMyPowerProducer block;
    int selector = 0;
    int counter = 0;

    float maxCh = 0, currCh = 0, maxIn = 0, currIn = 0, battMaxOut = 0, battCurrOut = 0;
    float reacMaxOut = 0, reacCurrentOut = 0, engMaxOut = 0, engCurrOut = 0, slrMaxOut = 0, slrCurrOut = 0, trbMaxOut = 0, trbCurrOut = 0;

    while (PowerProducers.Count > 0 && selector < PowerProducers.Count && MaxPowerProducerCount > counter)
    {
        counter++;

        block = (IMyPowerProducer)PowerProducers[selector];

        if (block is IMyBatteryBlock)
        {
            IMyBatteryBlock b = (IMyBatteryBlock)block;
            maxCh += b.MaxStoredPower;
            maxIn += b.MaxInput;
            battMaxOut += b.MaxOutput;

            currCh += b.CurrentStoredPower;
            currIn += b.CurrentInput;
            battCurrOut += b.CurrentOutput;
        }
        if (block is IMyReactor)
        {
            reacMaxOut += block.MaxOutput;
            reacCurrentOut += block.CurrentOutput;
        }
        if (block.BlockDefinition.ToString().Contains("HydrogenEngine"))
        {
            engMaxOut += block.MaxOutput;
            engCurrOut += block.CurrentOutput;
        }
        if (block is IMySolarPanel)
        {
            slrMaxOut += block.MaxOutput;
            slrCurrOut += block.CurrentOutput;
        }
        if (block.BlockDefinition.ToString().Contains("WindTurbine"))
        {
            trbMaxOut += block.MaxOutput;
            trbCurrOut += block.CurrentOutput;
        }

        selector++;

        if (counter == MaxPowerProducerCount)
        {
            counter = 0;
            yield return true;
        }
    }

    if (selector >= PowerProducers.Count - 1)
    {
        PowerData.SetValue(maxCh, 0);
        PowerData.SetValue(currCh, 1);
        PowerData.SetValue(maxIn, 2);
        PowerData.SetValue(currIn, 3);
        PowerData.SetValue(battMaxOut, 4);
        PowerData.SetValue(battCurrOut, 5);
        PowerData.SetValue(reacMaxOut, 6);
        PowerData.SetValue(reacCurrentOut, 7);
        PowerData.SetValue(engMaxOut, 8);
        PowerData.SetValue(engCurrOut, 9);
        PowerData.SetValue(slrMaxOut, 10);
        PowerData.SetValue(slrCurrOut, 11);
        PowerData.SetValue(trbMaxOut, 12);
        PowerData.SetValue(trbCurrOut, 13);
    }
    else
    {
        yield return true;
    }
}

/// <summary>
/// Gets cargo usage from Containers
/// </summary>
public IEnumerator<bool> GetCargoUsage(List<IMyTerminalBlock> list)
{
    IMyTerminalBlock block = null;
    IMyInventory inventory = null;
    int selector = 0;
    int counter = 0;

    MyFixedPoint maxVolume = 0, currentVolume = 0;

    while (list.Count > 0 && selector < list.Count && MaxCargoContainerCount > counter)
    {
        counter++;

        block = list[selector];
        if (block != null)
        {
            inventory = block.GetInventory();

            if (inventory != null)
            {
                maxVolume += inventory.MaxVolume;
                currentVolume += inventory.CurrentVolume;
            }
        }

        selector++;

        if (counter == MaxCargoContainerCount)
        {
            counter = 0;
            yield return true;
        }
    }

    if (selector >= list.Count - 1)
    {
        CargoData.SetValue(maxVolume, 0);
        CargoData.SetValue(currentVolume, 1);
    }
    else
    {
        yield return true;
    }
}

public IEnumerator<bool> GetGasData()
{
    IMyGasTank block;
    int selector = 0;
    int counter = 0;

    float oxyMaxCap = 0, oxyCurrCap = 0, hydMaxCap = 0, hydCurrCap = 0;

    while (GasTanks.Count > 0 && selector < GasTanks.Count && MaxGasTankCount > counter)
    {
        counter++;

        block = GasTanks[selector];

        if (block.BlockDefinition.SubtypeId.Contains("HydrogenTank"))
        {
            hydMaxCap += block.Capacity;
            hydCurrCap += block.Capacity * (float)block.FilledRatio;
        }
        if (!block.BlockDefinition.SubtypeId.Contains("HydrogenTank"))
        {
            oxyMaxCap += block.Capacity;
            oxyCurrCap += block.Capacity * (float)block.FilledRatio;
        }

        selector++;

        if (counter == MaxGasTankCount)
        {
            counter = 0;
            yield return true;
        }
    }

    if (selector >= GasTanks.Count - 1)
    {
        GasData.SetValue(hydMaxCap, 0);
        GasData.SetValue(hydCurrCap, 1);
        GasData.SetValue(oxyMaxCap, 2);
        GasData.SetValue(oxyCurrCap, 3);
    }
    else
    {
        yield return true;
    }
}

public void TogglePowerGen(bool toggle)
{
    foreach (IMyPowerProducer block in PowerProducers)
    {
        if (block is IMyReactor || block.BlockDefinition.ToString().Contains("HydrogenEngine"))
        {
            if (block.Enabled != toggle)
            {
                block.Enabled = toggle;
            }
        }
    }
}

public void PMS()
{
    float percentage = PowerData[1] / PowerData[0];

    bool batteryOverload = PowerData[5] >= PowerData[4] - 0.1;

    if (batteryOverload) charge = true;
    else
    {
        if(percentage < chargeOn) charge = true;
        if (percentage > chargeOff) charge = false;
    }

    TogglePowerGen(charge);
}

public bool ShouldGrabDoor(IMyTerminalBlock block)
{
    if (block is IMyAirtightHangarDoor && ignoreHangarDoors)
        return false;
    else if (block.CustomName.ToLower().Contains(doorExcludeStr.ToLower()))
        return false;
    else
        return true;
}

public void AutoUpdateDoor(List<DoorPair> doorPairs)
{
    for (int i = 0; i < doorPairs.Count; i++)
    {
        var pair = doorPairs[i];

        DoorStatus status = pair.Door.Status;
        if (status == DoorStatus.Open)
            pair.Timer -= RunTimeMain;
        else if (pair.LongTimer)
            pair.Timer = HangarReset;
        else
            pair.Timer = DoorReset;

        if (pair.Timer <= TimeSpan.Zero)
            if (status != DoorStatus.Closed)
            {
                pair.Door.CloseDoor();
                if (pair.LongTimer)
                    pair.Timer = HangarReset;
                else
                    pair.Timer = DoorReset;
            }
        doorPairs[i] = pair;
    }
}

public struct DoorPair
{
    public IMyDoor Door;
    public TimeSpan Timer;
    public bool LongTimer;
    public DoorPair(IMyDoor door, TimeSpan timer, bool longTimer)
    {
        Door = door;
        Timer = timer;
        LongTimer = longTimer;
    }
}

public bool IsImportantInv(IMyTerminalBlock block)
{
    return block is IMyShipConnector || block is IMyShipToolBase || block is IMyCargoContainer ||
        block is IMyProductionBlock || block is IMyReactor || block.BlockDefinition.SubtypeId.Contains("SurvivalKit") ||
        block is IMyLargeTurretBase || block is IMyLargeConveyorTurretBase || block is IMyUserControllableGun ||
        block is IMyCollector || block is IMyGasGenerator;
}

public RectangleF GetDrawingSurface(IMyTextSurface surface)
{
    RectangleF viewPort;
    viewPort = new RectangleF((surface.TextureSize - surface.SurfaceSize) / 2f, surface.SurfaceSize);
    return viewPort;
}

public void DrawFrame(IMyTextSurface surface, RectangleF viewPort, string customData, List<MySprite> frames)
{
    MySpriteDrawFrame frame = surface.DrawFrame();
    float surfaceMultiplier = GetSurfaceMultiplier(surface);

    Vector2 txePos1, txePos2, txePos3;

    if (surfaceMultiplier != 2)
    {
        txePos1 = new Vector2(95, 390);
        txePos2 = new Vector2(255, 390);
        txePos3 = new Vector2(415, 390);
    }
    else
    {
        txePos1 = new Vector2(130, 95);
        txePos2 = new Vector2(130, 255);
        txePos3 = new Vector2(130, 415);
    }

    List<ItemStorage> Ammo = new List<ItemStorage>();
    List<ItemStorage> Components = new List<ItemStorage>();
    // List<ItemStorage> ConsumableItems = new List<ItemStorage>();
    List<ItemStorage> Ingots = new List<ItemStorage>();
    List<ItemStorage> Ores = new List<ItemStorage>();
    List<ItemStorage> Tools = new List<ItemStorage>();

    if (Loading)
    {
        DrawText(ref frame, viewPort, new Vector2(250, 100), $"Loading...\nPlease wait :)", Color.LawnGreen, 2, TextAlignment.CENTER);
    }
    else
    {
        switch (customData)
        {
            default:
                DrawText(ref frame, viewPort, new Vector2(10, 0), "Dragon Core", Color.CornflowerBlue, 2, TextAlignment.LEFT);
                DrawText(ref frame, viewPort, new Vector2(10, 53), dateVersion, Color.Gray, 0.8F, TextAlignment.LEFT);
                DrawText(ref frame, viewPort, new Vector2(10, 110),
                    "Avalible Commands:\n" +
                    "- Ammo\n" +
                    "- Battery\n" +
                    "- Cargo\n" +
                    "- Component\n" +
                    "- Environmental\n" +
                    "- Generator\n" +
                    "- Ingot\n" +
                    "- Ore\n" +
                    "- Power\n" +
                    "- Thrust\n" +
                    "- Tool", Color.Yellow, 1, TextAlignment.LEFT);
                break;
            case "UVTest":
                DrawCenteredObject(frame, new Vector2(256 * surfaceMultiplier, 256), new Vector2(512, 512), Color.White, "UVChecker", 0);
                break;
            case "Cargo":
                DrawBar_Triple(frame, viewPort, percentageArray[0], percentageArray[1], percentageArray[2], $"Cargo:\n{percentageArray[0]:0.00 %}", $"Oxygen:\n{percentageArray[1]:0.00 %}", $"Hydrogen:\n{percentageArray[2]:0.00 %}", 1.2F, TextAlignment.CENTER, MainTextColor, Color.DarkGreen, Color.DeepSkyBlue, Color.OrangeRed, surfaceMultiplier);

                DrawCenteredObject(frame, txePos1, new Vector2(130, 120), Color.WhiteSmoke, "Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds", 0);
                DrawCenteredObject(frame, txePos2, new Vector2(130, 120), Color.WhiteSmoke, "IconOxygen", 0);
                DrawCenteredObject(frame, txePos3, new Vector2(130, 120), Color.WhiteSmoke, "IconHydrogen", 0);

                if (percentageArray[1] < SpriteDangerLevel && SpriteDangerOn)
                {
                    Vector2 dngPos = txePos2; if (surfaceMultiplier != 2) dngPos.Y -= 150; else dngPos.X += 150;
                    DrawCenteredObject(frame, dngPos, new Vector2(120, 120), Color.WhiteSmoke, "Danger", 0);
                }
                if (percentageArray[2] < SpriteDangerLevel && SpriteDangerOn)
                {
                    Vector2 dngPos = txePos3; if (surfaceMultiplier != 2) dngPos.Y -= 150; else dngPos.X += 150;
                    DrawCenteredObject(frame, dngPos, new Vector2(120, 120), Color.WhiteSmoke, "Danger", 0);
                }
                break;
            case "Thrust":
                int graphOffset;
                if (surfaceMultiplier != 2)
                {
                    graphOffset = 0;
                }
                else
                {
                    DrawText(ref frame, viewPort, new Vector2(250, 50),
                        $"Max Upwards Thrust:\n" +
                        $"{GetDoubleSymbol(ThrustData[0], "N")}\n" +
                        $"Current Upwards\nThrust:\n" +
                        $"{GetDoubleSymbol(ThrustData[1], "N")}\n" +
                        $"Current Cargo:\n" +
                        $"{GetDoubleSymbol(double.Parse((CargoData[1].RawValue * 0.001f).ToString()), "kg")}"
                        , MainTextColor, 1.5F, TextAlignment.CENTER);

                    graphOffset = 512;
                }

                DrawBar(frame, new Vector2(startPos.X + graphOffset, startPos.Y), new Vector2(pos2_2.X + graphOffset, pos2_2.Y), MainBarColor, Color.DeepSkyBlue, percentageArray[3]); // Current Thrust
                DrawCenteredObject(frame, new Vector2(125 + graphOffset, 230), new Vector2(100, 100), Color.WhiteSmoke, "Triangle", 0);
                DrawCenteredObject(frame, new Vector2(125 + graphOffset, 350), new Vector2(100, 100), Color.WhiteSmoke, "Triangle", 0);
                DrawText(ref frame, viewPort, new Vector2(125 + graphOffset, 5), $"Current Thrust:\n{percentageArray[3]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);

                DrawBar(frame, new Vector2(pos3_2.X + graphOffset, pos3_2.Y), new Vector2(endPos.X + graphOffset, endPos.Y), MainBarColor, Color.Green, percentageArray[4]); // Liftable cargo
                DrawCenteredObject(frame, new Vector2(380 + graphOffset, 210), new Vector2(110, 80), Color.WhiteSmoke, "Triangle", 0);
                DrawCenteredObject(frame, new Vector2(380 + graphOffset, 260), new Vector2(110, 80), Color.WhiteSmoke, "Triangle", 0);
                DrawCenteredObject(frame, new Vector2(380 + graphOffset, 400), new Vector2(170, 160), Color.WhiteSmoke, "Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds", 0);
                DrawText(ref frame, viewPort, new Vector2(375 + graphOffset, 5), $"Thrust Lift\nUsage: {percentageArray[4]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);
                break;
            case "Battery":
                DrawBar_Triple(frame, viewPort, percentageArray[5], percentageArray[6], percentageArray[7], $"Charge\n{percentageArray[5]:0.00 %}", $"Input\n{percentageArray[6]:0.00 %}", $"Output\n{percentageArray[7]:0.00 %}", 1.2F, TextAlignment.CENTER, MainTextColor, new Color(0, 20, 180), Color.DarkGreen, Color.Red, surfaceMultiplier);

                DrawCenteredObject(frame, txePos1, new Vector2(130, 120), Color.White, "IconEnergy", 0);
                DrawCenteredObject(frame, txePos2, new Vector2(110, 120), Color.White, "AH_PullUp", 0);
                DrawCenteredObject(frame, txePos3, new Vector2(110, 120), Color.White, "AH_PullUp", float.Parse(Math.PI.ToString()));

                if (percentageArray[5] < SpriteDangerLevel && SpriteDangerOn)
                {
                    Vector2 dngPos = txePos1; if (surfaceMultiplier != 2) dngPos.Y -= 150; else dngPos.X += 150;
                    DrawCenteredObject(frame, dngPos, new Vector2(120, 120), Color.WhiteSmoke, "Danger", 0);
                }
                break;
            case "Environmental":
                if (surfaceMultiplier != 2)
                {
                    DrawBar(frame, startPos, pos2_2, MainBarColor, Color.DarkGreen, percentageArray[8]); // Solar
                    DrawText(ref frame, viewPort, new Vector2(125, 5), $"Solar Output:\n{percentageArray[8]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);

                    DrawBar(frame, pos3_2, endPos, MainBarColor, Color.DarkGreen, percentageArray[9]); // Wind
                    DrawText(ref frame, viewPort, new Vector2(375, 5), $"Wind Output:\n{percentageArray[9]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);
                }
                else
                {
                    DrawBar(frame, new Vector2(20, 20), new Vector2(440 * surfaceMultiplier, 240), MainBarColor, Color.DarkGreen, percentageArray[8]); // Solar
                    DrawText(ref frame, viewPort, new Vector2(475 * surfaceMultiplier, 90), $"Solar\nOutput:\n{percentageArray[8]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);
                            
                    DrawBar(frame, new Vector2(20, 260), new Vector2(440 * surfaceMultiplier, 500), MainBarColor, Color.DarkGreen, percentageArray[9]); // Wind
                    DrawText(ref frame, viewPort, new Vector2(475 * surfaceMultiplier, 330), $"Wind\nOutput:\n{percentageArray[9]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);
                }
                break;
            case "Generator":
                if (surfaceMultiplier != 2)
                {
                    DrawBar(frame, startPos, pos2_2, MainBarColor, Color.DarkGreen, percentageArray[10]); // Reactors
                    DrawText(ref frame, viewPort, new Vector2(125, 5), $"Reactor Output:\n{percentageArray[10]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);

                    DrawBar(frame, pos3_2, endPos, MainBarColor, Color.DarkGreen, percentageArray[11]); // Engines
                    DrawText(ref frame, viewPort, new Vector2(375, 5), $"Engine Output:\n{percentageArray[11]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);
                }
                else
                {
                    DrawBar(frame, new Vector2(20, 20), new Vector2(440 * surfaceMultiplier, 240), MainBarColor, Color.DarkGreen, percentageArray[10]); // Reactors
                    DrawText(ref frame, viewPort, new Vector2(475 * surfaceMultiplier, 90), $"Reactor\nOutput:\n{percentageArray[10]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);

                    DrawBar(frame, new Vector2(20, 260), new Vector2(440 * surfaceMultiplier, 500), MainBarColor, Color.DarkGreen, percentageArray[11]); // Engines
                    DrawText(ref frame, viewPort, new Vector2(475 * surfaceMultiplier, 330), $"Engine\nOutput:\n{percentageArray[11]:0.00 %}", MainTextColor, 1.2F, TextAlignment.CENTER);
                }
                break;
            case "Power":
                DrawBar(frame, startPos, new Vector2(pos2_4.X * surfaceMultiplier, pos2_4.Y), MainBarColor, Color.DarkGreen, percentageArray[8]);
                DrawText(ref frame, viewPort, new Vector2(75 * surfaceMultiplier, 5), $"Solar:\n{percentageArray[8]:0.00 %}", MainTextColor, 1.1F, TextAlignment.CENTER);

                DrawBar(frame, new Vector2(pos3_4.X * surfaceMultiplier, pos3_4.Y), new Vector2(pos4_4.X * surfaceMultiplier, pos4_4.Y), MainBarColor, Color.DarkGreen, percentageArray[9]);
                DrawText(ref frame, viewPort, new Vector2(195 * surfaceMultiplier, 5), $"Wind:\n{percentageArray[9]:0.00 %}", MainTextColor, 1.1F, TextAlignment.CENTER);

                DrawBar(frame, new Vector2(pos5_4.X * surfaceMultiplier, pos5_4.Y), new Vector2(pos6_4.X * surfaceMultiplier, pos6_4.Y), MainBarColor, Color.DarkGreen, percentageArray[10]);
                DrawText(ref frame, viewPort, new Vector2(315 * surfaceMultiplier, 5), $"Reactor:\n{percentageArray[10]:0.00 %}", MainTextColor, 1.1F, TextAlignment.CENTER);
                        
                DrawBar(frame, new Vector2(pos7_4.X * surfaceMultiplier, pos7_4.Y), new Vector2(endPos.X * surfaceMultiplier, endPos.Y), MainBarColor, Color.DarkGreen, percentageArray[11]);
                DrawText(ref frame, viewPort, new Vector2(440 * surfaceMultiplier, 5), $"Engine:\n{percentageArray[11]:0.00 %}", MainTextColor, 1.1F, TextAlignment.CENTER);
                break;
            case "Ore":
                foreach (var item in GridInv) if (item.Type == "Ore") Ores.Add(item);
                DrawText(ref frame, viewPort, new Vector2(10, 10 - OreDisplayScroll), "Ore\n\n" + GetListNames(Ores), MainInvTextColor, 1.3F, TextAlignment.LEFT);
                DrawText(ref frame, viewPort, new Vector2(300, 10 - OreDisplayScroll), $"Amount\n\n{GetListSymbol(Ores, "kg")}", MainInvAmountColor, 1.3F, TextAlignment.LEFT);
                DrawLine(frame, new Vector2(290, 0), new Vector2(290, 512 + OreDisplayScroll), 2, Color.Gray);
                DrawLine(frame, new Vector2(0, 65 - OreDisplayScroll), new Vector2(512 * surfaceMultiplier, 65 - OreDisplayScroll), 2, Color.Gray);
                break;
            case "Ingot":
                foreach (var item in GridInv) if (item.Type == "Ingot") Ingots.Add(item);
                DrawText(ref frame, viewPort, new Vector2(10, 10 - IngotDisplayScroll), "Ingot\n\n" + GetListNames(Ingots), MainInvTextColor, 1.3F, TextAlignment.LEFT);
                DrawText(ref frame, viewPort, new Vector2(300, 10 - IngotDisplayScroll), $"Amount\n\n{GetListSymbol(Ingots, "kg")}", MainInvAmountColor, 1.3F, TextAlignment.LEFT);
                DrawLine(frame, new Vector2(290, 0), new Vector2(290, 512 + IngotDisplayScroll), 2, Color.Gray);
                DrawLine(frame, new Vector2(0, 65 - IngotDisplayScroll), new Vector2(512 * surfaceMultiplier, 65 - IngotDisplayScroll), 2, Color.Gray);
                break;
            case "Component":
                foreach (var item in GridInv) if (item.Type == "Component") Components.Add(item);
                DrawText(ref frame, viewPort, new Vector2(10, 10 - ComponentDisplayScroll), "Component\n\n" + GetListNames(Components), MainInvTextColor, 1.3F, TextAlignment.LEFT);
                DrawText(ref frame, viewPort, new Vector2(300, 10 - ComponentDisplayScroll), $"Amount\n\n{GetListSymbol(Components, "")}", MainInvAmountColor, 1.3F, TextAlignment.LEFT);
                DrawLine(frame, new Vector2(290, 0), new Vector2(290, 512 + ComponentDisplayScroll), 2, Color.Gray);
                DrawLine(frame, new Vector2(0, 65 - ComponentDisplayScroll), new Vector2(512 * surfaceMultiplier, 65 - ComponentDisplayScroll), 2, Color.Gray);
                break;
            case "Ammo":
                foreach (var item in GridInv) if (item.Type == "AmmoMagazine") Ammo.Add(item);
                DrawText(ref frame, viewPort, new Vector2(10, 10 - AmmoDisplayScroll), "Ammo\n\n" + GetListNames(Ammo), MainInvTextColor, 1.3F, TextAlignment.LEFT);
                DrawText(ref frame, viewPort, new Vector2(320, 10 - AmmoDisplayScroll), $"Amount\n\n{GetListSymbol(Ammo, "")}", MainInvAmountColor, 1.3F, TextAlignment.LEFT);
                DrawLine(frame, new Vector2(310, 0), new Vector2(310, 512 + AmmoDisplayScroll), 2, Color.Gray);
                DrawLine(frame, new Vector2(0, 65 - AmmoDisplayScroll), new Vector2(512 * surfaceMultiplier, 65 - AmmoDisplayScroll), 2, Color.Gray);
                break;
            case "Tool":
                foreach (var item in GridInv) if (item.Type == "PhysicalGunObject") Tools.Add(item);
                DrawText(ref frame, viewPort, new Vector2(10, 10 - ToolDisplayScroll), "Tool\n\n" + GetListNames(Tools), MainInvTextColor, 1.3F, TextAlignment.LEFT);
                DrawText(ref frame, viewPort, new Vector2(330, 10 - ToolDisplayScroll), $"Amount\n\n{GetListSymbol(Tools, "")}", MainInvAmountColor, 1.3F, TextAlignment.LEFT);
                DrawLine(frame, new Vector2(310, 0), new Vector2(310, 512 + ToolDisplayScroll), 2, Color.Gray);
                DrawLine(frame, new Vector2(0, 65 - ToolDisplayScroll), new Vector2(512 * surfaceMultiplier, 65 - ToolDisplayScroll), 2, Color.Gray);
                break;
        }
    }

    foreach (MySprite thisFrame in frames)
    {
        if (!frames.Contains(thisFrame))
        {
            frame.Add(thisFrame);
        }
    }
    frame.Dispose();
}

public void PrepareSurface(IMyTextSurface surface)
{
    surface.ContentType = ContentType.SCRIPT;
    surface.Script = "";
    surface.ScriptBackgroundColor = Color.Black;
}

public void DrawText(ref MySpriteDrawFrame frame, RectangleF viewPort, Vector2 pos, string text, Color color, float scale, TextAlignment alignment)
{
    var position = pos + viewPort.Position;

    var sprite = new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = text,
        Position = position,
        RotationOrScale = scale,
        Color = color,
        Alignment = alignment,
        FontId = "White"
    };

    frame.Add(sprite);
}

public void DrawObject(MySpriteDrawFrame frame, Vector2 point1, Vector2 point2, Color color, string SpriteData, float angle)
{
    Vector2 position = 0.5f * (point1 + point2);
    Vector2 size = new Vector2(point2.X - point1.X, point2.Y - point1.Y);

    MySprite sprite = MySprite.CreateSprite(SpriteData, position, size);
    sprite.Color = color;
    sprite.RotationOrScale = angle;
    frame.Add(sprite);
}

public void DrawCenteredObject(MySpriteDrawFrame frame, Vector2 pos, Vector2 size, Color color, string SpriteData, float angle)
{
    MySprite sprite = MySprite.CreateSprite(SpriteData, pos, size);
    sprite.Color = color;
    sprite.RotationOrScale = angle;
    frame.Add(sprite);
}

public struct SpritePair
{
    public List<MySprite> Sprites;
    public IMyTextPanel TextPanel;

    public SpritePair(List<MySprite> sprites, IMyTextPanel textPanel)
    {
        Sprites = sprites;
        TextPanel = textPanel;
    }
}

public bool IsVertical(Vector2 point1, Vector2 point2)
{
    Vector2 diff = new Vector2(point2.X - point1.X, point2.Y - point1.Y);
    if (diff.Y > diff.X) return true;
    else return false;
}

public void DrawBar(MySpriteDrawFrame frame, Vector2 point1, Vector2 point2, Color FrameColor, Color BarColor, float percentage)
{
    Vector2 diff = new Vector2(point2.X - point1.X, point2.Y - point1.Y);
    Vector2 BarAdjust = new Vector2(3, 3);
    Vector2 barPoint = new Vector2();
    bool vertical = IsVertical(point1, point2);
    DrawObject(frame, point1, point2, FrameColor, "SquareSimple", 0);

    if (vertical)
    {
        barPoint.X = point2.X;
        barPoint.Y = point1.Y + diff.Y * (1 - percentage);

        if (percentage < 0.98F)
        {
            DrawObject(frame, point1 + BarAdjust, point2 - BarAdjust, BarColor, "SquareSimple", 0);
            if (percentage > 0.02F) DrawObject(frame, point1 + BarAdjust, barPoint - BarAdjust, Color.Black, "SquareSimple", 0);
            else DrawObject(frame, point1 + BarAdjust, point2 - BarAdjust, Color.Black, "SquareSimple", 0);
        }
        else DrawObject(frame, point1 + BarAdjust, point2 - BarAdjust, BarColor, "SquareSimple", 0);
    }
    else
    {
        barPoint.X = point1.X + diff.X * percentage;
        barPoint.Y = point2.Y;

        DrawObject(frame, point1 + BarAdjust, point2 - BarAdjust, Color.Black, "SquareSimple", 0);

        if (percentage > 0.02F) DrawObject(frame, point1 + BarAdjust, barPoint - BarAdjust, BarColor, "SquareSimple", 0);
    }
}

public void DrawBar_Triple(MySpriteDrawFrame frame, RectangleF viewPort, float val1, float val2, float val3, string text1, string text2, string text3, float scale, TextAlignment textAlignment, Color textColor, Color gColor1, Color gColor2, Color gColor3, float surfaceMultiplier)
{
    if (surfaceMultiplier != 2)
    {
        DrawBar(frame, startPos, pos2_3, MainBarColor, gColor1, val1);
        DrawText(ref frame, viewPort, new Vector2(85, 5), text1, textColor, scale, textAlignment);

        DrawBar(frame, pos3_3, pos4_3, MainBarColor, gColor2, val2);
        DrawText(ref frame, viewPort, new Vector2(250, 5), text2, textColor, scale, textAlignment);

        DrawBar(frame, pos5_3, endPos, MainBarColor, gColor3, val3);
        DrawText(ref frame, viewPort, new Vector2(415, 5), text3, textColor, scale, textAlignment);
    }
    else
    {
        DrawBar(frame, new Vector2(20, 20), new Vector2(440 * surfaceMultiplier, 170), MainBarColor, gColor1, val1);
        DrawText(ref frame, viewPort, new Vector2(475 * surfaceMultiplier, 45), text1, textColor, scale, textAlignment);

        DrawBar(frame, new Vector2(20, 180), new Vector2(440 * surfaceMultiplier, 330), MainBarColor, gColor2, val2);
        DrawText(ref frame, viewPort, new Vector2(475 * surfaceMultiplier, 205), text2, textColor, scale, textAlignment);

        DrawBar(frame, new Vector2(20, 340), new Vector2(440 * surfaceMultiplier, 500), MainBarColor, gColor3, val3);
        DrawText(ref frame, viewPort, new Vector2(475 * surfaceMultiplier, 370), text3, textColor, scale, textAlignment);
    }
}

public void ConvertCountToOutputString(ref string out_str, int item_count, string unit)
{
    if (item_count <= 10E2)
        out_str += Math.Round((double)item_count, 2).ToString() + $" {unit}";
    else if (item_count <= 10E5)
        out_str += Math.Round(item_count * 0.001, 2).ToString() + $"K {unit}";
    else
        out_str += Math.Round(item_count * 0.000001, 2).ToString() + $"M {unit}";
}

// TODO: Fix Output sort bullshit
public string GetListSymbol(List<ItemStorage> items, string unit)
{
    string output = "";

    foreach (var item in items)
    {
        ConvertCountToOutputString(ref output, (int)item.Count, unit);

        if(item.InProductionCount > 0)
        {
            output += " + ";
            ConvertCountToOutputString(ref output, item.InProductionCount, unit);     
        }
        output += "\n";
    }

    return output;
}

public string GetListNames(List<ItemStorage> items)
{
    string output = "";

    foreach (var item in items)
    {
        output += $"{ReplaceName(item.Name)}\n";
    }
    return output;
}

public string ReplaceName(string name)
{
    return ItemNameReplaceDict.GetValueOrDefault(name, name);
}

public string GetDoubleSymbol(double value, string str)
{
    if (value <= 10E2) return Math.Round(value, 2).ToString() + $" {str}\n";
    else if (value <= 10E5) return Math.Round(value * 0.001, 2).ToString() + $"K {str}\n";
    else return Math.Round(value * 0.00001, 2).ToString() + $"M {str}\n";
}

public void DrawLine(MySpriteDrawFrame frame, Vector2 point1, Vector2 point2, float width, Color color)
{
    Vector2 position = 0.5f * (point1 + point2);
    Vector2 diff = point1 - point2;
    float length = diff.Length();
    if (length > 0)
        diff /= length;

    Vector2 size = new Vector2(length, width);
    float angle = (float)Math.Acos(Vector2.Dot(diff, Vector2.UnitX));
    angle *= Math.Sign(Vector2.Dot(diff, Vector2.UnitY));

    MySprite sprite = MySprite.CreateSprite("SquareSimple", position, size);
    sprite.RotationOrScale = angle;
    sprite.Color = color;
    frame.Add(sprite);
}

public float GetSurfaceMultiplier(IMyTextSurface surface)
{
    return surface.SurfaceSize.X / 512;
}


List<ItemStorage> ItemCollectorList = new List<ItemStorage>();
List<ItemStorage> SortingList = new List<ItemStorage>();
IEnumerator<bool> RunItemCollector()
{
    IMyTerminalBlock block;
    int selector = 0;
    int counter = 0;
    SortingList.Clear();

    while (CargoList.Count > 0 && selector < CargoList.Count && MaxCargoCount > counter)
    {
        counter++;
        block = CargoList[selector];
        List<MyInventoryItem> Inv_Items = new List<MyInventoryItem>();
        List<MyProductionItem> InProdItems = new List<MyProductionItem>();
        if (block != null && !block.Closed)
        {
            if (block is IMyProductionBlock)
            {
                IMyProductionBlock pBlk = (IMyProductionBlock)block;
                pBlk.InputInventory.GetItems(Inv_Items);
                AddItemsToMainSortList(ref Inv_Items);

                pBlk.OutputInventory.GetItems(Inv_Items);
                AddItemsToMainSortList(ref Inv_Items);

                if (pBlk is IMyAssembler)
                {
                    pBlk = (IMyAssembler)pBlk;
                    pBlk.GetQueue(InProdItems);
                    foreach (var item in InProdItems)
                    {
                        MyInventoryItem converted_item = new MyInventoryItem(item.BlueprintId, item.ItemId, item.Amount);
                        Inv_Items.Add(converted_item);
                    }
                    AddItemsToMainSortList(ref Inv_Items, true);
                    counter += 2; // compensate for potential high load
                }
            }
            else
            {
                block.GetInventory(0).GetItems(Inv_Items);
                AddItemsToMainSortList(ref Inv_Items);
            }
        }
        selector++;
        if (counter >= MaxCargoCount)
        {
            counter = 0;
            yield return true;
        }
    }

    SortingList = SortingList.OrderBy(i => i.Type).ToList();
    SortingList = SortingList.OrderBy(i => i.Name).ToList();
    // foreach(var item in SortingList) Echo($"N:{item.Name}\nT:{item.Type}\nC:{item.Count}\nP:{item.InProductionCount}");

    counter = 0;
    ItemCollectorList.Clear();
    yield return true;

    while (SortingList.Count != 0)
    {
        counter++;

        ItemStorage current_item = SortingList[0];
        ItemStorage next_item = null; if(SortingList.Count > 1) next_item = SortingList[1];

        if(next_item != null)
        {
            if (current_item.Name == next_item.Name && current_item.Type == next_item.Type)
            {
                current_item.Count += next_item.Count;
                current_item.InProductionCount += next_item.InProductionCount;
                SortingList.RemoveAt(1);
            }
            else
            {
                ItemCollectorList.Add(current_item);
                SortingList.RemoveAt(0);
            }
        }
        else
        {
            ItemCollectorList.Add(current_item);
            SortingList.RemoveAt(0);
        }

        if (counter == MaxSortCount)
        {
            counter = 0;
            yield return true;
        }
    }

    GridInv.Clear();
    GridInv.AddList(ItemCollectorList);
    yield return true;
}

void AddItemsToMainSortList(ref List<MyInventoryItem> inv_item_list, bool is_in_prod = false)
{
    foreach (var item in inv_item_list)
    {
        MyFixedPoint amount = 0;
        MyFixedPoint in_prod_amount = 0;
        if (is_in_prod) in_prod_amount = item.Amount;
        else amount = item.Amount;

        string type_id = GetItemType(item.Type.TypeId);
        string subtype_id = item.Type.SubtypeId;
                
        if(is_in_prod)
        {
            var new_id = TryGuessItemType(item.Type);
            if (new_id != default(MyDefinitionId))
            {
                type_id = GetItemType(new_id.TypeId.ToString());
                subtype_id = new_id.SubtypeId.ToString();
            }
        }

        ItemStorage newItem = new ItemStorage(subtype_id, amount, type_id, (int)in_prod_amount); 
        SortingList.Add(newItem);
    }            
    inv_item_list.Clear();
}

MyDefinitionId TryGuessItemType(MyDefinitionId def_id)
{
    bool found = false;
    string sub = def_id.SubtypeId.ToString();
    string type = "";
    if (sub.Contains("Position")) sub = sub.Remove(0, 13);

    if (sub.Contains("Rifle") | sub.Contains("Launcher") && !found)
    {
        if (sub.Contains("Mag"))
            type = "AmmoMagazine";
        else
        {
            type = "PhysicalGunObject";
            sub += "Item";
        }
        found = true;
    }

    if (sub.Contains("Welder") | sub.Contains("Grinder") | sub.Contains("Drill") && !found)
    {
        type = "PhysicalGunObject";
        sub = sub + "Item";
        found = true;
    }

    if (sub.Contains("Ammo") | sub.Contains("Clip") | sub.Contains("Missile200mm") | sub.Contains("NATO") && !found)
    {
        type = "AmmoMagazine";
        sub = sub.Replace("Magazine", "");
        found = true;
    }

    if (sub.Contains("Bottle") && !found)
    {
        type = "GasContainerObject";
        found = true;
    }

    if (sub.Contains("Pistol") && !found)
    {
        if (sub.Contains("Magazine"))
        {
            type = "AmmoMagazine";
        }
        else
        {
            type = "PhysicalGunObject";
            if (sub.Contains("Elite")) sub = sub.Replace("Auto", "");
            sub += "Item";
        }
        found = true;
    }

    if (!found) type = "Component";
    sub = sub.Replace("Component", "");
    MyDefinitionId new_id;
    MyDefinitionId.TryParse("MyObjectBuilder_" + type + "/" + sub, out new_id);
    return new_id;
}

public string GetItemType(string itemDef)
{
    if (itemDef.Contains("AmmoMagazine")) return "AmmoMagazine";
    if (itemDef.Contains("Component")) return "Component";
    if (itemDef.Contains("ConsumableItem") | itemDef.Contains("Container")) return "Consumable";
    if (itemDef.Contains("Ingot")) return "Ingot";
    if (itemDef.Contains("Ore")) return "Ore";
    if (itemDef.Contains("PhysicalGunObject")) return "PhysicalGunObject";
    if (itemDef.Contains("BlueprintDefinition")) return "BlueprintDefinition";
    return "Other";
}

public bool RunStateMachines(bool initial) // if init = false, then it will stay false until one can move. if init = true, then it will only stay true if none can move
{
    bool check = initial;
    for (int i = 0; i < StateMachines.Length; i++)
    {
        if (StateMachines[i] != null && StateMachines[i].MoveNext()) check = !initial;
    }
    return check;
}

// NOTE: INV MANAGER DISABLED
public void SetStateMachines()
{
    StateMachines[0] = GetPowerData();
    StateMachines[1] = GetCargoUsage(CargoList);
    StateMachines[2] = GetGasData();
    StateMachines[3] = GetThrustPower(UpwardsThrust);
    StateMachines[4] = RunItemCollector();
    StateMachines[5] = RunAssemblerManager();
    // StateMachines[6] = RunInventoryManager();
}

List<ItemProductionTag> MissingComponents = new List<ItemProductionTag>();
TimeSpan CheckAssemblerQueueTimer = TimeSpan.Zero;
IEnumerator<bool> RunAssemblerManager()
{
    // Avoid crash with assembler check
    if (Main_Assembler == null || Main_Assembler.Closed ||
        !Main_Assembler.CustomName.Contains(PRODUCTION_TAG) || Main_Assembler.CooperativeMode)
    {
        Main_Assembler = null;
        yield break;
    }

    List<MyProductionItem> items_in_prod = new List<MyProductionItem>();
    Main_Assembler.GetQueue(items_in_prod);

    CheckAssemblerQueueTimer += RunTimeMain;
    if (CheckAssemblerQueueTimer < TimeSpan.FromSeconds(1)) yield break;
    CheckAssemblerQueueTimer = TimeSpan.Zero;

    MissingComponents.Clear();
    List<string> t_check = new List<string> { "Ammo", "Component" };
    GetLowRunningItemByTypes(t_check, ref MissingComponents, 1, false);
    if (MissingComponents.Count == 0 && items_in_prod.Count != 0) Main_Assembler.ClearQueue(); // No missing items but some are in production

    MissingComponents.Clear();
    GetLowRunningItemByTypes(t_check, ref MissingComponents);

    if (items_in_prod.Count == 0 && MissingComponents.Count > 0)
    {
        foreach (var item in MissingComponents)
        {
            if (!Main_Assembler.CanUseBlueprint(item.ProductionId)) continue;
            Main_Assembler.AddQueueItem(item.ProductionId, item.ToMakeAmount);
        }
        MissingComponents.Clear();
    }
            
    yield return true;
}

void GetAvailableContainers(List<string> allowed_types, ref List<InventoryStorage> main, ref List<InventoryStorage> backup)
{
    main.Clear();
    backup.Clear();

    foreach (var container in CargoStorageList)
    {
        if (container.IsInvNull() || container.HasAvailableSpace() < 1) continue; // space is in m^3

        if (container.AllowedTypes.Count == 0)
        {
            backup.Add(container);
            continue;
        }

        bool containsAllTypes = true;
        foreach (var type in allowed_types)
        {
            if (!container.AllowedTypes.Contains(type))
            {
                containsAllTypes = false;
            }
        }

        if (containsAllTypes) main.Add(container);
    }
}

List<InventoryStorage> GetContainersWithSpecificItem(ItemStorage search_item)
{
    MyDefinitionId search_type; MyDefinitionId.TryParse($"MyObjectBuilder_{search_item.Type}/{search_item.Name}", out search_type);
    List<InventoryStorage> found_inventories = new List<InventoryStorage>();
    // Get invs from CargoList
    foreach (var block in CargoList)
    {
        if (block == null || block is IMyProductionBlock) continue;

        var inv = block.GetInventory();
        if (inv == null) continue;

        if (inv.ContainItems(1, search_type))
        {
            List<string> types = new List<string>();
            found_inventories.Add(new InventoryStorage(inv, types));
        }
    }
    return found_inventories;
}

void GetLowRunningItemByTypes(List<string> types, ref List<ItemProductionTag> low_items, int get_limit = 5, bool account_prod = true, bool return_prod_type = true)
{
    int counter = 0;
    for (int i = 0; i < GridInv.Count; i++)
    {
        if (counter >= get_limit) break;

        var item = GridInv[i];
        int missing;
        item.GetMissingAmount(ItemLowLimitsDict, out missing, account_prod);
        if (missing != 0 && types.Contains(item.Type))
        {
            string id;
            if (return_prod_type) id = item.GetProductionTag(ProdItemDict);
            else id = $"MyObjectBuilder_{item.Type}/{item.Name}";

            low_items.Add(new ItemProductionTag(id, missing));
            counter++;
        }
    }
}

int RefineryCheckCounter = 0;
bool CanManageRefineriesFreely = true;
IEnumerator<bool> RunInventoryManager()
{
    if (ProductionBlockList.Count == 0) yield break;

    List<InventoryStorage> available_inv = new List<InventoryStorage>();
    List<InventoryStorage> backup_inv = new List<InventoryStorage>();
    List<IMyRefinery> refineries = new List<IMyRefinery>();
    MyDefinitionId item_test_type; MyDefinitionId.TryParse("MyObjectBuilder_Ores/Stone", out item_test_type);

    RefineryCheckCounter++;
    if (RefineryCheckCounter < 25) yield break;

    RefineryCheckCounter = 0;
    List<string> types = new List<string> { "Ore" };
    GetAvailableContainers(types, ref available_inv, ref backup_inv);
    if (available_inv.Count == 0 & backup_inv.Count == 0) yield break;
    yield return true;

    types = new List<string> { "Ingot" };
    LowIngotList.Clear();
    GetLowRunningItemByTypes(types, ref LowIngotList, 20, false, false);
    LowIngotList = LowIngotList.OrderByDescending(i => i.ToMakeAmount.RawValue).ToList();

    bool should_manage = (LowIngotList.Count != 0 & CanManageRefineriesFreely) | ForceRefineryControl;
    foreach (var b in ProductionBlockList) if (b is IMyRefinery && b != null) b.UseConveyorSystem = !should_manage;
    if (!should_manage) yield break;
    yield return true;

    refineries.Clear();
    foreach (var b in ProductionBlockList)
    {
        if (b is IMyRefinery) refineries.Add((IMyRefinery)b);
    }

    bool canFillRefsFully = false;
    MyFixedPoint totalRefSpace = 0;
    MyFixedPoint maxMoveAmount = 0;
    foreach (var r in refineries)
    {
        if (r != null && r.InputInventory != null) totalRefSpace += r.InputInventory.MaxVolume;
    }

    while (LowIngotList.Count != 0)
    {
        var id = LowIngotList[0].OriginalID();
        var name = id.Substring(id.IndexOf("/") + 1);

        var item = GridInv.Find(i => i.Name == name && i.Type == "Ore");
        if (item != null)
        {
            canFillRefsFully = totalRefSpace > item.Count;
            maxMoveAmount = item.Count * (MyFixedPoint)(1.00 / refineries.Count);
            break;
        }
        else
        {
            LowIngotList.RemoveAt(0);
        }
    }
            
    foreach (var block in refineries)
    {
        if (block == null || block.InputInventory == null) continue;

        var inv = block.InputInventory;
        var items = new List<MyInventoryItem>();
        inv.GetItems(items);
        var free_inventories = GetUsableInventory(available_inv, inv);
        if (free_inventories.Count == 0) free_inventories = GetUsableInventory(backup_inv, inv);

        if (LowIngotList.Count == 0) yield break;
        ItemStorage checkItem = new ItemStorage($"Ore/{LowIngotList[0].ProductionId.SubtypeId}");
        List<InventoryStorage> oreInvList = GetContainersWithSpecificItem(checkItem);

        for (int i = 0; i < oreInvList.Count; i++)
        {
            var item = oreInvList[i];
            if (item.IsInvNull() || !item.Container.CanTransferItemTo(inv, item_test_type)) oreInvList.RemoveAt(i);
        }

        CanManageRefineriesFreely = free_inventories.Count != 0 & oreInvList.Count != 0;
        if (!CanManageRefineriesFreely) yield break; // no space || no ore

        // Clear inventory
        int list_select = 0;
        for (int i = 0; i < items.Count; i++)
        {
            var selected_inv = free_inventories[list_select];
            var inv_item = items[i];
            var available_space = GetAvailableSpaceInInventory(inv_item, selected_inv.Container);
            if (!CanTransferItemToInventory(inv, selected_inv.Container, available_space, item_test_type) && list_select < free_inventories.Count - 1)
            {
                list_select++;
                continue;
            }
            if (list_select >= free_inventories.Count) continue;
            inv.TransferItemTo(selected_inv.Container, inv_item, available_space);
        }
        yield return true;

        // Refill inventory if empty
        foreach (var src_inv in oreInvList)
        {
            if (src_inv.Container != null)
            {
                items.Clear();
                src_inv.Container.GetItems(items);
                foreach (var inv_item in items)
                {
                    if (!inv_item.Type.SubtypeId.Contains(checkItem.Name)) continue;

                    var available_space = GetAvailableSpaceInInventory(inv_item, inv);
                    if (!CanTransferItemToInventory(src_inv.Container, inv, available_space, item_test_type)) continue;

                    if (!canFillRefsFully && available_space > maxMoveAmount) available_space = maxMoveAmount;
                    src_inv.Container.TransferItemTo(inv, inv_item, available_space);
                }
            }
        }
    }

    yield return true;
}

bool CanAddToInventory(IMyInventory inv, MyFixedPoint amount)
{
    return amount <= (inv.MaxVolume - inv.CurrentVolume) * 1000;
}

MyFixedPoint GetAvailableSpaceInInventory(MyInventoryItem item, IMyInventory inv)
{
    MyFixedPoint available_space = (inv.MaxVolume - inv.CurrentVolume) * 1000;
    if (available_space > item.Amount) available_space = item.Amount;
    return available_space;
}

bool CanTransferItemToInventory(IMyInventory source, IMyInventory destination, MyFixedPoint amount, MyItemType type)
{
    return CanAddToInventory(destination, amount) && source.CanTransferItemTo(destination, type);
}

List<InventoryStorage> GetUsableInventory(List<InventoryStorage> list, IMyInventory destination)
{
    List<InventoryStorage> inv_list = new List<InventoryStorage>();
    for (int i = 0; i < list.Count; i++)
    {
        var inv = list[i];
        MyDefinitionId type; MyDefinitionId.TryParse("MyObjectBuilder_Ores/Stone", out type);
        if (!inv.IsInvNull() && inv.HasAvailableSpace() != -1 && inv.Container.CanTransferItemTo(destination, type))
        {
            inv_list.Add(inv);
        }
    }
    return inv_list;
}

public struct ItemProductionTag
{
    public MyDefinitionId ProductionId;
    public MyFixedPoint ToMakeAmount;
    private string Original_ID;

    public ItemProductionTag(string id, MyFixedPoint amount)
    {
        MyDefinitionId.TryParse(id, out ProductionId);
        ToMakeAmount = amount;
        Original_ID = id;
    }

    public override string ToString()
    {
        return $"T:{ProductionId.TypeId}/S:{ProductionId.SubtypeName}A:{ToMakeAmount.ToIntSafe()}";
    }

    public string OriginalID()
    {
        return Original_ID;
    }
}

public class ItemStorage
{
    public string Name { get; set; }
    public MyFixedPoint Count { get; set; } = 0;
    public string Type { get; set; }
    public int InProductionCount { get; set; } = 0;

    public ItemStorage(string name, MyFixedPoint count, string type, int in_prod_count = 0)
    {
        Name = name;
        Count = count;
        Type = type;
        InProductionCount = in_prod_count;
    }

    public ItemStorage(string full_id)
    {
        string[] str = full_id.Split('/');
        if (str.Length > 1)
        Name = str[1];
        Type = str[0];
        Count = 0;
        InProductionCount = 0;
    }

    public string GetProductionTag(Dictionary<string, string> prod_item_dict)
    {         
        string output = "";
        if (prod_item_dict.TryGetValue(Name, out output))
        {
            return BP_DEF_TAG + output;
        }
                
        return BP_DEF_TAG + Name;
    }

    public bool GetMissingAmount(Dictionary<string, int> limit_dict, out int missing, bool account_prod = true)
    {
        int limit;
        if (limit_dict.TryGetValue(Name, out limit))
        {
            int diff;
            if (!account_prod) diff = Count.ToIntSafe() - limit;
            else diff = Count.ToIntSafe() + InProductionCount - limit;

            diff -= limit / 10;

            if (diff < 0) missing = -diff;
            else missing = 0;

            return true;
        }

        missing = 0;
        return false;
    }
}

public class InventoryStorage
{
    public IMyInventory Container;
    public List<string> AllowedTypes;
    public string BlockName;

    public InventoryStorage(IMyInventory c, List<string> allowedList, string blockName = "")
    {
        Container = c;
        AllowedTypes = allowedList;
        BlockName = blockName;
    }

    public bool IsInvNull()
    {
        return Container == null;
    }

    /// <summary>
    /// Gets available space inside container
    /// </summary>
    /// <returns>If inv is null: -1, otherwise the available space.</returns>
    public MyFixedPoint HasAvailableSpace()
    {
        if (IsInvNull()) return -1;
        return Container.MaxVolume - Container.CurrentVolume;
    }

    public override string ToString()
    {
        string output = $"Block: {BlockName} | Types: ";
        foreach(var type in AllowedTypes) output += $"{type} ";
        return output;
    }
}
