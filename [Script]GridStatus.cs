/*
 * R e a d m e
 * -----------
 * 
 * GRID STATUS
 * 
 * SUMMARY:
 * 
 * This script gathers and displays all important info about the following blocks:
 * 
 * - Batteries
 * - Reactors
 * - Hydrogen Engines
 * - Hydrogen Tanks
 * - Oxygen Tanks
 * - Solar Panels
 * - Wind Turbines
 * - Cargo Containers
 * 
 * Special Features:
 * Maintenance LCD: summarizes all critical data about all blocks on one interactive screen
 * Compact LCD: you can switch between all block categories on one interactive screen
 * //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * 
 * SETUP:
 * 
 * You need:
 * - any of the above-named blocks
 * - LCDs
 * 
 * How to make the script work:
 * - insert the script in your Programmable Block
 * - click on Custom Data and change the SETTINGS to your liking
 * - name your LCDs according to the name tag you can see or edit in the settings
 * 
 * 
 * You want to use the interactive screens?
 * Control them via script arguments (found in the Info Box of the Programmable Block)
 * //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * 
 * FURTHER INFO:
 * 
 * - every block category has its own LCD screen and some block categories can be shown together on one LCD screen
 *   (check the settings!)
 * - adding a name tag to multiple LCDs will display your block category on all of them.
 * - you can disable block categories in order to not show them on the (interactive) screens. I recommend disabling block categories
 *   that you either don't want to show or haven't build on your grid anyway. This will also improve performance.
 * 
 * //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * 
 * If you happen to experience any problems or have any feedback feel free to contact me.
 */

//Start of the script
public class ScriptSettings
{
    public ScriptSettings()
    {
        Search = new SearchSettings(false, 0, "Grid Status");

        ConsumptionTimeFrameInSeconds = 5;

        Battery = new CategorySettings("Battery");
        Reactor = new CategorySettings("Reactor");
        HydrogenEngine = new CategorySettings("Hydrogen Engine");
        HydrogenTank = new CategorySettings("Hydrogen Tank");
        OxygenTank = new CategorySettings("Oxygen Tank");
        SolarPanel = new CategorySettings("Solar Panel");
        WindTurbine = new CategorySettings("Wind Turbine");
        CargoContainer = new CategorySettings("Cargo Container");
        Maintenance = new CategorySettings("Maintenance");
        Compact = new CategorySettings("Compact");
        Energy = new CategorySettings("Energy");
        Gas = new CategorySettings("Gas");
        Renewables = new CategorySettings("Renewables");

        ListOreFirst = false;
    }

    public ScriptSettings(bool[] bools, int[] ints, string[] strings)
    {
        Search = new SearchSettings(bools[0], ints[0], strings[0]);
        ConsumptionTimeFrameInSeconds = ints[1];

        Battery = new CategorySettings(strings[1], bools[1]);
        Reactor = new CategorySettings(strings[2], bools[2]);
        HydrogenEngine = new CategorySettings(strings[3], bools[3]);
        HydrogenTank = new CategorySettings(strings[4], bools[4]);
        OxygenTank = new CategorySettings(strings[5], bools[5]);
        SolarPanel = new CategorySettings(strings[6], bools[6]);
        WindTurbine = new CategorySettings(strings[7], bools[7]);
        CargoContainer = new CategorySettings(strings[8], bools[8]);
        Maintenance = new CategorySettings(strings[9], bools[9]);
        Compact = new CategorySettings(strings[10], bools[10]);
        Energy = new CategorySettings(strings[11], Reactor.Show || HydrogenEngine.Show);
        Gas = new CategorySettings(strings[12], HydrogenTank.Show || OxygenTank.Show);
        Renewables = new CategorySettings(strings[13], SolarPanel.Show || WindTurbine.Show);

        ListOreFirst = bools[11];
    }

    public void GetValues(out bool[] bools, out int[] ints, out string[] strings)
    {
        bools = new bool[] { Search.Subgrids, Battery.Show, Reactor.Show, HydrogenEngine.Show, HydrogenTank.Show, OxygenTank.Show,
            SolarPanel.Show, WindTurbine.Show, CargoContainer.Show, Maintenance.Show, Compact.Show, ListOreFirst };
        ints = new int[] {Search.Mode, ConsumptionTimeFrameInSeconds, 100 };
        strings = new string[] { Search.NameTag, Battery.LCD, Reactor.LCD, HydrogenEngine.LCD, HydrogenTank.LCD, OxygenTank.LCD,
            SolarPanel.LCD, WindTurbine.LCD, CargoContainer.LCD, Maintenance.LCD, Compact.LCD, Energy.LCD, Gas.LCD, Renewables.LCD };
    }

    public CategorySettings[] GetCategories()
    {
        return new CategorySettings[] { Battery, Reactor, HydrogenEngine, HydrogenTank, OxygenTank, SolarPanel, WindTurbine, CargoContainer, Maintenance, Compact, Energy, Gas, Renewables };
    }

    public SearchSettings Search;
    public int ConsumptionTimeFrameInSeconds;
    public CategorySettings Battery;
    public CategorySettings Reactor;
    public CategorySettings HydrogenEngine;
    public CategorySettings HydrogenTank;
    public CategorySettings OxygenTank;
    public CategorySettings SolarPanel;
    public CategorySettings WindTurbine;
    public CategorySettings CargoContainer;
    public CategorySettings Maintenance;
    public CategorySettings Compact;
    public CategorySettings Energy;
    public CategorySettings Gas;
    public CategorySettings Renewables;
    public bool ListOreFirst;
}

public class SearchSettings
{
    public SearchSettings(bool subgrids, int mode, string nameTag)
    {
        Subgrids = subgrids;
        Mode = mode;
        NameTag = nameTag;
    }

    public int Mode;
    public bool Subgrids;
    public string NameTag;
}

public class CategorySettings
{
    public CategorySettings()
    {
        Show = false;
        LCD = "";
    }
    public CategorySettings(string lcd, bool show = true)
    {
        Show = show;
        LCD = lcd;
    }

    public bool Show;
    public string LCD;
}


public class BasicBlockValues
{
    public BasicBlockValues()
    {
        Settings = new CategorySettings();
    }
    public void GetData<T>(T block) where T : IMyCubeBlock
    {
        if (!block.IsFunctional)
        {
            DamagedBlocks++;
        }
        if (block.IsBeingHacked)
        {
            HackedBlocks++;
        }
    }

    public virtual int GetAlerts()
    {
        return DamagedBlocks + HackedBlocks;
    }

    public int Amount { get; set; }
    public int DamagedBlocks { get; set; }
    public int HackedBlocks { get; set; }
    public int EmptyBlocks { get; set; }
    public BlockCategory BlockName { get; set; }
    public CategorySettings Settings { get; set; }
}

public class FunctionalBlockValues : BasicBlockValues
{
    public void GetEnabledData<T>(T block) where T : IMyFunctionalBlock
    {
        if (!block.Enabled)
        {
            DisabledBlocks++;
        }
    }

    public override int GetAlerts()
    {
        int alerts = DamagedBlocks + HackedBlocks;

        if (BlockName == BlockCategory.SolarPanels || BlockName == BlockCategory.WindTurbines || BlockName == BlockCategory.HydrogenTanks || BlockName == BlockCategory.OxygenTanks)
        {
            alerts += DisabledBlocks;
        }
        if (BlockName == BlockCategory.Batteries)
        {
            alerts += DisabledBlocks;
            alerts += EmptyBlocks;
        }

        return alerts;
    }

    public int DisabledBlocks { get; set; }
}

public class PoweredBlockValues : FunctionalBlockValues
{
    public PoweredBlockValues()
    {
        MaxOutput = 0f;
        CurrentOutput = 0f;
    }
    public void GetOutputData<T>(T block) where T : IMyPowerProducer
    {
        MaxOutput += block.MaxOutput;
        CurrentOutput += block.CurrentOutput;
    }

    public float MaxOutput { get; set; }
    public float CurrentOutput { get; set; }
}

public class GasBlockValues : FunctionalBlockValues
{
    public GasBlockValues()
    {
        Capacity = 0f;
        CurrentStoredGas = 0.0;
    }

    public void GetGasData(IMyGasTank block)
    {
        Capacity += block.Capacity;
        CurrentStoredGas += block.Capacity * block.FilledRatio;

        if (block.FilledRatio == 0.0)
        {
            EmptyBlocks++;
        }
    }

    public float Capacity { get; set; }
    public double CurrentStoredGas { get; set; }
}

public class BatteryValues : PoweredBlockValues
{
    public BatteryValues()
    {
        MaxStoredPower = 0f;
        CurrentStoredPower = 0f;
        MaxInput = 0f;
        CurrentInput = 0f;
    }

    public void GetData(List<IMyBatteryBlock> blocks, CategorySettings settings)
    {
        Amount = blocks.Count;
        Settings = settings;
        BlockName = BlockCategory.Batteries;

        for (int i = 0; i < Amount; i++)
        {
            GetData(blocks[i]);
            GetEnabledData(blocks[i]);
            GetOutputData(blocks[i]);

            MaxStoredPower += blocks[i].MaxStoredPower;
            CurrentStoredPower += blocks[i].CurrentStoredPower;
            MaxInput += blocks[i].MaxInput;
            CurrentInput += blocks[i].CurrentInput;

            if (!blocks[i].HasCapacityRemaining)
            {
                EmptyBlocks++;
            }
        }
    }

    public float MaxStoredPower { get; set; }
    public float CurrentStoredPower { get; set; }
    public float MaxInput { get; set; }
    public float CurrentInput { get; set; }
}

public class ReactorValues : PoweredBlockValues
{
    public ReactorValues()
    {
        UraniumAmount = 0.0;
    }

    public void GetData(List<IMyReactor> blocks, CategorySettings settings)
    {
        MyItemType itemUranium = new MyItemType("MyObjectBuilder_Ingot", "Uranium");
        Amount = blocks.Count;
        Settings = settings;
        BlockName = BlockCategory.Reactors;


        for (int i = 0; i < Amount; i++)
        {
            GetData(blocks[i]);
            GetEnabledData(blocks[i]);
            GetOutputData(blocks[i]);

            double uraniumAmountTemp = (double)blocks[i].GetInventory().GetItemAmount(itemUranium);
            UraniumAmount += uraniumAmountTemp;

            if (uraniumAmountTemp == 0.0)
            {
                EmptyBlocks++;
            }
        }
    }

    public double UraniumAmount { get; set; }
}

public class HydrogenEngineValues : PoweredBlockValues
{
    public HydrogenEngineValues()
    {
        Capacity = 0;
        CurrentStoredGas = 0;
    }

    void GetCapacity(string detailedInfo)
    {
        string[] detailedInfoLines = detailedInfo.Split('\n');

        if (detailedInfoLines[3] == null)
        {
            return;
        }

        string[] capacityLine = detailedInfoLines[3].Split(' ');

        if (capacityLine[2] == null)
        {
            return;
        }

        string[] capacityInfo = capacityLine[2].Split('/');

        if (capacityInfo[0] == null || capacityInfo[0].Length < 3)
        {
            return;
        }

        int currentStoredGas;

        if (Int32.TryParse(capacityInfo[0].Substring(1, capacityInfo[0].Length - 2), out currentStoredGas))
        {
            CurrentStoredGas += currentStoredGas;

            if (currentStoredGas == 0)
            {
                EmptyBlocks++;
            }
        }

        if (capacityInfo[1] == null || capacityInfo[1].Length < 3)
        {
            return;
        }

        int capacity;

        if (Int32.TryParse(capacityInfo[1].Substring(0, capacityInfo[1].Length - 2), out capacity))
        {
            Capacity += capacity;
        }
    }

    public void GetData(List<IMyPowerProducer> blocks, CategorySettings settings)
    {
        Amount = blocks.Count;
        Settings = settings;
        BlockName = BlockCategory.HydrogenEngines;

        for (int i = 0; i < Amount; i++)
        {
            GetData(blocks[i]);
            GetEnabledData(blocks[i]);
            GetOutputData(blocks[i]);
            GetCapacity(blocks[i].DetailedInfo);
        }
    }

    public float Capacity { get; set; }
    public float CurrentStoredGas { get; set; }
}

public class GasTankValues : GasBlockValues
{
    public GasTankValues()
    {
        IsUpdating = false;
        CapacityChangePerSec = 0.0;
    }

    public void GetData(List<IMyGasTank> blocks, CategorySettings settings, BlockCategory blockCategory, ref List<double> capacities, int timeFrameInSeconds, int ticksPerTimeFrame)
    {
        Amount = blocks.Count;
        Settings = settings;
        BlockName = blockCategory;

        for (int i = 0; i < Amount; i++)
        {
            GetData(blocks[i]);
            GetEnabledData(blocks[i]);
            GetGasData(blocks[i]);
        }

        if (capacities.Count == ticksPerTimeFrame)
        {
            for (int i = 0; i < capacities.Count - 1; i++)
            {
                capacities[i] = capacities[i + 1];
            }

            capacities[ticksPerTimeFrame - 1] = CurrentStoredGas;

            IsUpdating = false;
            CapacityChangePerSec = (capacities[0] - CurrentStoredGas) / timeFrameInSeconds;
        }
        else
        {
            capacities.Add(CurrentStoredGas);

            IsUpdating = true;
            CapacityChangePerSec = 0.0;
        }
    }

    public bool IsUpdating { get; set; }
    public double CapacityChangePerSec { get; set; }
}

public class RenewablesValues : PoweredBlockValues
{
    public RenewablesValues()
    {
        PotentialOutput = 0f;
    }

    void GetEmptyState(IMyPowerProducer block)
    {
        if (block.CurrentOutput == 0f)
        {
            EmptyBlocks++;
        }
    }

    public void GetData(List<IMySolarPanel> blocks, CategorySettings settings)
    {
        Amount = blocks.Count;
        Settings = settings;
        BlockName = BlockCategory.SolarPanels;
        const float maximumSingleOutputLarge = 0.16f;
        const float maximumSingleOutputSmall = 0.04f;
        int panelsLarge = 0;
        int panelsSmall = 0;

        for (int i = 0; i < Amount; i++)
        {
            GetData(blocks[i]);
            GetEnabledData(blocks[i]);
            GetOutputData(blocks[i]);
            GetEmptyState(blocks[i]);

            if (blocks[i].BlockDefinition.SubtypeId.StartsWith("Large"))
            {
                panelsLarge++;
            }
            else
            {
                panelsSmall++;
            }
        }

        PotentialOutput = (maximumSingleOutputLarge * panelsLarge) + (maximumSingleOutputSmall * panelsSmall);
    }

    public void GetData(List<IMyWindTurbine> blocks, CategorySettings settings)
    {
        Amount = blocks.Count;
        Settings = settings;
        BlockName = BlockCategory.WindTurbines;
        const float maximumSingleOutput = 0.4f;

        for (int i = 0; i < Amount; i++)
        {
            GetData(blocks[i]);
            GetEnabledData(blocks[i]);
            GetOutputData(blocks[i]);
            GetEmptyState(blocks[i]);
        }

        PotentialOutput = maximumSingleOutput * Amount;
    }

    public float PotentialOutput { get; set; }
}

public class CargoValues : BasicBlockValues
{
    public void GetData<T>(List<T> blocks, CategorySettings settings, bool oreFirst = false) where T : IMyCubeBlock
    {
        Amount = blocks.Count;
        Settings = settings;
        BlockName = BlockCategory.CargoContainers;

        Dictionary<MyItemType, double> items = new Dictionary<MyItemType, double>();

        for (int i = 0; i < Amount; i++)
        {
            GetData(blocks[i]);

            IMyInventory inventory = blocks[i].GetInventory();

            CurrentVolume += (double)inventory.CurrentVolume;
            MaxVolume += (double)inventory.MaxVolume;
            Mass += (double)inventory.CurrentMass;

            List<MyInventoryItem> itemsTemp = new List<MyInventoryItem>();
            inventory.GetItems(itemsTemp);

            for (int k = 0; k < itemsTemp.Count; k++)
            {
                if (items.ContainsKey(itemsTemp[k].Type))
                {
                    items[itemsTemp[k].Type] += (double)itemsTemp[k].Amount;
                }
                else
                {
                    items.Add(itemsTemp[k].Type, (double)itemsTemp[k].Amount);
                }
            }
        }

        CurrentVolume *= 1000;
        MaxVolume *= 1000;
        Items = new List<ItemValue>();

        foreach (var item in items)
        {
            Items.Add(new ItemValue(item.Key, item.Value));
        }

        if (oreFirst)
        {
            Items.Sort(new SortItemsComponentFirst());
        }
        else
        {
            Items.Sort(new SortItemsOreFirst());
        }

        Items.Reverse();
    }

    public double CurrentVolume;
    public double MaxVolume;
    public double Mass;
    public List<ItemValue> Items;
}


public struct LCDValue
{
    public void DisplayString(string text)
    {
        LCD.ContentType = ContentType;
        LCD.ClearImagesFromSelection();
        LCD.Alignment = TextAlignment;
        LCD.TextPadding = TextPadding;
        LCD.Font = Font;
        LCD.FontSize = FontSize;
        LCD.WriteText(text);
    }

    public IMyTextPanel LCD { get; set; }
    public ContentType ContentType { get; set; }
    public TextAlignment TextAlignment { get; set; }
    public float TextPadding { get; set; }
    public string Font { get; set; }
    public float FontSize { get; set; }
    public Vector2 RealScreenSize { get; set; }
    public int BarLength { get; set; }
    public float SpaceWidth { get; set; }
    public float LineHeight { get; set; }
    public bool SmallPanel { get; set; }
    public bool SplitPossible { get; set; }
    public bool SqueezeSpecialInfo { get; set; }
}

public struct ItemValue
{
    public ItemValue(MyItemType type, double amount)
    {
        Type = type;
        Amount = amount;
    }

    public MyItemType Type;
    public double Amount;
}

public class SortItemsOreFirst : IComparer<ItemValue>
{
    public int Compare(ItemValue x, ItemValue y)
    {
        if (x.Type.TypeId.EndsWith("_Ore") || x.Type.TypeId.EndsWith("_Ingot"))
        {
            if (y.Type.TypeId.EndsWith("_Ore") || y.Type.TypeId.EndsWith("_Ingot"))
            {
                return x.Amount.CompareTo(y.Amount);
            }

            return -1;
        }
        else if (y.Type.TypeId.EndsWith("_Ore") || y.Type.TypeId.EndsWith("_Ingot"))
        {
            return 1;
        }
        return x.Amount.CompareTo(y.Amount);
    }
}

public class SortItemsComponentFirst : IComparer<ItemValue>
{
    public int Compare(ItemValue x, ItemValue y)
    {
        if (x.Type.TypeId.EndsWith("_Ore") || x.Type.TypeId.EndsWith("_Ingot"))
        {
            if (y.Type.TypeId.EndsWith("_Ore") || y.Type.TypeId.EndsWith("_Ingot"))
            {
                return x.Amount.CompareTo(y.Amount);
            }

            return 1;
        }
        else if (y.Type.TypeId.EndsWith("_Ore") || y.Type.TypeId.EndsWith("_Ingot"))
        {
            return -1;
        }
        return x.Amount.CompareTo(y.Amount);
    }
}

public enum BlockCategory
{
    Batteries,
    Reactors,
    HydrogenEngines,
    HydrogenTanks,
    OxygenTanks,
    SolarPanels,
    WindTurbines,
    CargoContainers
}

//Globals
UpdateFrequency updateFrequency;
MyIni gridStatus = new MyIni();

List<double> hydrogenCapacities = new List<double>();
List<double> oxygenCapacities = new List<double>();
bool[] clearedLCDs = new bool[13];

int selectedMaintenanceInfo = 0;
bool expandMaintenanceInfo = false;

int selectedCompactInfo = 0;
bool expandCompactInfo = false;

static readonly Dictionary<string, string> itemComponent = new Dictionary<string, string>
{
    {"BulletproofGlass", "Bulletproof Glass"},
    {"Construction", "Construction Comp."},
    {"Detector", "Detector Comp."},
    {"EngineerPlushie", "Engineer Plushie"},
    {"SabiroidPlushie", "Saberoid Plushie"},
    {"GravityGenerator", "Gravity Comp."},
    {"InteriorPlate", "Interior Plate"},
    {"LargeTube", "Large Steel Tube"},
    {"Medical", "Medical Comp."},
    {"MetalGrid", "Metal Grid"},
    {"PowerCell", "Power Cell"},
    {"RadioCommunication", "Radio-comm Comp."},
    {"Reactor", "Reactor Comp."},
    {"SmallTube", "Small Steel Tube"},
    {"SolarCell", "Solar Cell"},
    {"SteelPlate", "Steel Plate"},
    {"Thrust", "Thruster Comp."},
    {"ZoneChip", "Zone Chip"}
};

static readonly Dictionary<string, string> itemTool = new Dictionary<string, string>
{
    {"AngleGrinder4Item", "Elite Grinder"},
    {"HandDrill4Item", "Elite Hand Drill" },
    {"Welder4Item", "Elite Welder" },
    {"AngleGrinder2Item", "Enhanced Grinder"},
    {"HandDrill2Item", "Enhanced Hand Drill"},
    {"Welder2Item", "Enhanced Welder"},
    {"AngleGrinderItem", "Grinder"},
    {"HandDrillItem", "Hand Drill"},
    {"AutomaticRifleItem", "MR-20 Rifle"},
    {"UltimateAutomaticRifleItem", "MR-30E Rifle"},
    {"RapidFireAutomaticRifleItem", "MR-50A Rifle"},
    {"PreciseAutomaticRifleItem", "MR-8P Rifle"},
    {"AdvancedHandHeldLauncherItem", "PRO-1 Rocket Launcher"},
    {"AngleGrinder3Item", "Proficient Grinder"},
    {"HandDrill3Item", "Proficient Hand Drill"},
    {"Welder3Item", "Proficient Welder"},
    {"BasicHandHeldLauncherItem", "RO-1 Rocket Launcher"},
    {"SemiAutoPistolItem", "S-10 Pistol"},
    {"ElitePistolItem", "S-10E Pistol"},
    {"FullAutoPistolItem", "S-20A Pistol"},
    {"WelderItem", "Welder"}
};

static readonly Dictionary<string, string> itemAmmo = new Dictionary<string, string>
{
    {"NATO_5p56x45mm", "5.56x45mm NATO magazine"},
    {"LargeCalibreAmmo", "Artillery Shell"},
    {"MediumCalibreAmmo", "Assault Cannon Shell"},
    {"AutocannonClip", "Autocannon Magazine"},
    {"NATO_25x184mm", "Gatling Ammo Box"},
    {"LargeRailgunAmmo", "Large Railgun Sabot"},
    {"Missile200mm", "Rocket"},
    {"AutomaticRifleGun_Mag_20rd", "MR-20 Rifle Magazine"},
    {"UltimateAutomaticRifleGun_Mag_30rd", "MR-30E Rifle Magazine"},
    {"RapidFireAutomaticRifleGun_Mag_50rd", "MR-50A Rifle Magazine"},
    {"PreciseAutomaticRifleGun_Mag_5rd", "MR-8P Rifle Magazine"},
    {"SemiAutoPistolMagazine", "S-10 Pistol Magazine"},
    {"ElitePistolMagazine", "S-10E Pistol Magazine"},
    {"FullAutoPistolMagazine", "S-20A Pistol Magazine"},
    {"SmallRailgunAmmo", "Small Railgun Sabot"}
};

static readonly Dictionary<string, string> itemConsumable = new Dictionary<string, string>
{
    {"ClangCola", "Clang Kola"},
    {"CosmicCoffee", "Cosmic Coffee"}
};

static readonly Dictionary<string, string> itemMisc = new Dictionary<string, string>
{
    {"HydrogenBottle", "Hydrogen Bottle"},
    {"OxygenBottle", "Oxygen Bottle"},
    {"SpaceCredit", "Space Credit"}
};

static readonly Dictionary<string, string> itemOre = new Dictionary<string, string>
{
    {"Cobalt", "Cobalt Ore"},
    {"Gold", "Gold Ore"},
    {"Iron", "Iron Ore"},
    {"Magnesium", "Magnesium Ore"},
    {"Nickel", "Nickel Ore"},
    {"Platinum", "Platinum Ore"},
    {"Scrap", "Scrap Metal"},
    {"Silicon", "Silicon Ore"},
    {"Silver", "Silver Ore"},
    {"Uranium", "Uranium Ore"}
};

static readonly Dictionary<string, string> itemIngot = new Dictionary<string, string>
{
    {"Cobalt", "Cobalt Ingot"},
    {"Gold", "Gold Ingot"},
    {"Stone", "Gravel"},
    {"Iron", "Iron Ingot"},
    {"Magnesium", "Magnesium Powder"},
    {"Nickel", "Nickel Ingot"},
    {"Scrap", "Old Scrap Metal"},
    {"Platinum", "Platinum Ingot"},
    {"Silicon", "Silicon Wafer"},
    {"Silver", "Silver Ingot"},
    {"Uranium", "Uranium Ingot"}
};

static readonly Dictionary<string, Dictionary<string, string>> item = new Dictionary<string, Dictionary<string, string>>
{
    {"MyObjectBuilder_Component", itemComponent},
    {"MyObjectBuilder_Ingot", itemIngot},
    {"MyObjectBuilder_Ore", itemOre},
    {"MyObjectBuilder_PhysicalGunObject", itemTool},
    {"MyObjectBuilder_AmmoMagazine", itemAmmo},
    {"MyObjectBuilder_ConsumableItem", itemConsumable}
};

public Program()
{
    gridStatus.TryParse(Storage);
    string blockID = Me.EntityId.ToString() + ":";

    updateFrequency = IntToUpdateFrequency(gridStatus.Get(blockID + TypeToString(updateFrequency.GetType()), nameof(updateFrequency)).ToInt32());
    selectedMaintenanceInfo = gridStatus.Get(blockID + TypeToString(selectedMaintenanceInfo.GetType()), nameof(selectedMaintenanceInfo)).ToInt32();
    expandMaintenanceInfo = gridStatus.Get(blockID + TypeToString(expandMaintenanceInfo.GetType()), nameof(expandMaintenanceInfo)).ToBoolean();
    selectedCompactInfo = gridStatus.Get(blockID + TypeToString(selectedCompactInfo.GetType()), nameof(selectedCompactInfo)).ToInt32();
    expandCompactInfo = gridStatus.Get(blockID + TypeToString(expandCompactInfo.GetType()), nameof(expandCompactInfo)).ToBoolean();

    Array.Clear(clearedLCDs, 0, clearedLCDs.Length);

    for (int i = 0; i < clearedLCDs.Length; i++)
    {
        clearedLCDs[i] = gridStatus.Get(blockID + TypeToString(clearedLCDs.GetType()), nameof(clearedLCDs) + i.ToString()).ToBoolean();
    }

    Runtime.UpdateFrequency = updateFrequency;
}

public void Save()
{
    gridStatus.Clear();

    string blockID = Me.EntityId.ToString() + ":";


    gridStatus.Set(blockID + TypeToString(updateFrequency.GetType()), nameof(updateFrequency), UpdateFrequencyToString(updateFrequency));
    gridStatus.Set(blockID + TypeToString(selectedMaintenanceInfo.GetType()), nameof(selectedMaintenanceInfo), selectedMaintenanceInfo);
    gridStatus.Set(blockID + TypeToString(expandMaintenanceInfo.GetType()), nameof(expandMaintenanceInfo), expandMaintenanceInfo);
    gridStatus.Set(blockID + TypeToString(selectedCompactInfo.GetType()), nameof(selectedCompactInfo), selectedCompactInfo);
    gridStatus.Set(blockID + TypeToString(expandCompactInfo.GetType()), nameof(expandCompactInfo), expandCompactInfo);

    for (int i = 0; i < clearedLCDs.Length; i++)
    {
        gridStatus.Set(blockID + TypeToString(clearedLCDs.GetType()), nameof(clearedLCDs) + i.ToString(), clearedLCDs[i]);
    }

    Storage = gridStatus.ToString();
}

public void Main(string argument, UpdateType updateSource)
{
    Echo(DateTime.Now.ToString());

    switch (argument.ToLower())
    {
        case "":
            break;

        case "nextmaintenance":
            if (!expandMaintenanceInfo)
            {
                selectedMaintenanceInfo++;
            }
            break;

        case "previousmaintenance":
            if (!expandMaintenanceInfo)
            {
                selectedMaintenanceInfo--;
            }
            break;

        case "selectmaintenance":
            expandMaintenanceInfo = !expandMaintenanceInfo;
            break;

        case "nextcompact":
            if (!expandCompactInfo)
            {
                selectedCompactInfo++;
            }
            break;

        case "previouscompact":
            if (!expandCompactInfo)
            {
                selectedCompactInfo--;
            }
            break;

        case "selectcompact":
            expandCompactInfo = !expandCompactInfo;
            break;

        case "softreset":
            Reset();
            break;

        case "hardreset":
            Reset(true);
            break;

        default:
            break;
    }


    if (updateSource == UpdateType.Update100 || updateSource == UpdateType.Update10 || updateSource == UpdateType.Update1)
    {
        ScriptSettings scriptSettings = GetScriptSettings();
        DisplayData(scriptSettings);
        ClearLCDs(scriptSettings);
    }

    Echo("IMPORTANT: For the LCD name tags and other settings check the Custom Data of this block!\n\n\n" +
        "ARGUMENTS:\n(ignore upper and lower casing)\n\n- \"NextMaintenance\": Switches cursor to the next category on the maintenance screen.\n\n" +
        "- \"PreviousMaintenance\": Switches cursor to the previous category on the maintenance screen.\n\n" +
        "- \"SelectMaintenance\": Shows additional info about the selected block category on the maintenance screen. Also used to return to the overview.\n\n" +
        "- \"NextCompact\": Switches cursor to the next category on the compact screen.\n\n" +
        "- \"PreviousCompact\": Switches cursor to the previous category on the maintenance screen.\n\n" +
        "- \"SelectCompact\": Shows the full info about the selected block category on the compact screen. Also used to return to the overview.\n\n" +
        "- \"SoftReset\": Resets standard values. Use this if you come across any problems.\n\n" +
        "- \"HardReset\": Resets standard values and all settings. Use this if you come across any problems.");
}

/*====
         Tasks
         ====*/

void DisplayData(ScriptSettings scriptSettings)
{
    int ticksPerTimeFrame;
    if (!CalculateTicksPerTimeFrame(updateFrequency, scriptSettings.ConsumptionTimeFrameInSeconds, out ticksPerTimeFrame))
    {
        Echo("ERROR: Consumption calculation time frame and update frequency are asynchron\nPlease make sure the time frame is dividable by 5 while using \"UpdateFrequency 100\"\n" +
            "Change the settings accordingly and recompile the script after.\n\n");
        return;
    }

    List<BasicBlockValues> blockData = GatherAllBlockData(scriptSettings, ticksPerTimeFrame);

    BasicBlockValues[] energyData = new BasicBlockValues[] {new BasicBlockValues(), new BasicBlockValues() };
    BasicBlockValues[] gasData = new BasicBlockValues[] { new BasicBlockValues(), new BasicBlockValues() };
    BasicBlockValues[] renewablesData = new BasicBlockValues[] { new BasicBlockValues(), new BasicBlockValues() };

    for (int i = 0; i < blockData.Count; i++)
    {
        if (blockData[i].Settings.Show)
        {
            DisplaySingleInfo(blockData[i]);
        }
        switch (blockData[i].BlockName)
        {
            case BlockCategory.Reactors:
                energyData[0] = blockData[i];
                break;
            case BlockCategory.HydrogenEngines:
                energyData[1] = blockData[i];
                break;
            case BlockCategory.HydrogenTanks:
                gasData[0] = blockData[i];
                break;
            case BlockCategory.OxygenTanks:
                gasData[1] = blockData[i];
                break;
            case BlockCategory.SolarPanels:
                renewablesData[0] = blockData[i];
                break;
            case BlockCategory.WindTurbines:
                renewablesData[1] = blockData[i];
                break;
            default:
                break;
        }
    }


    DisplayDoubleInfo(scriptSettings.Energy.LCD, energyData[0], energyData[1]);
    DisplayDoubleInfo(scriptSettings.Gas.LCD, gasData[0], gasData[1]);
    DisplayDoubleInfo(scriptSettings.Renewables.LCD, renewablesData[0], renewablesData[1]);


    if (scriptSettings.Maintenance.Show)
    {
        if (selectedMaintenanceInfo > blockData.Count - 1)
        {
            selectedMaintenanceInfo = 0;
        }
        else if (selectedMaintenanceInfo < 0)
        {
            selectedMaintenanceInfo = blockData.Count - 1;
        }


        List<LCDValue> lcdValues = GetLCDValues(scriptSettings.Maintenance.LCD);

        for (int i = 0; i < lcdValues.Count; i++)
        {
            DisplaySpecialInfo(blockData, lcdValues[i], "MAINTENANCE", selectedMaintenanceInfo, expandMaintenanceInfo, true);
        }
    }
    if (scriptSettings.Compact.Show)
    {
        if (selectedCompactInfo > blockData.Count - 1)
        {
            selectedCompactInfo = 0;
        }
        else if (selectedCompactInfo < 0)
        {
            selectedCompactInfo = blockData.Count - 1;
        }


        List<LCDValue> lcdValues = GetLCDValues(scriptSettings.Compact.LCD);

        for (int i = 0; i < lcdValues.Count; i++)
        {
            DisplaySpecialInfo(blockData, lcdValues[i], "SYSTEM OVERVIEW", selectedCompactInfo, expandCompactInfo, false);
        }
    }
}


/*================
        Displaying Methods
        =================*/

void DisplaySingleInfo<T>(T value) where T : BasicBlockValues
{
    List<LCDValue> lcdValues = GetLCDValues(value.Settings.LCD);

    for (int i = 0; i < lcdValues.Count; i++)
    {
        lcdValues[i].DisplayString(ChooseCorrectInfoBuilder(value, lcdValues[i]));
    }
}

void DisplayDoubleInfo<T>(string lcdName, T value1, T value2) where T : BasicBlockValues
{
    List<LCDValue> lcdValues = GetLCDValues(lcdName, true);

    for (int i = 0; i < lcdValues.Count; i++)
    {
        string strInfo = "";

        if (value1.Settings.Show)
        {
            strInfo += ChooseCorrectInfoBuilder(value1, lcdValues[i], value2.Settings.Show && lcdValues[i].SplitPossible);
        }
        if (value2.Settings.Show)
        {
            if (value1.Settings.Show)
            {
                if (value1.BlockName != BlockCategory.Reactors || (value1.BlockName == BlockCategory.Reactors && value1.Amount == 0))
                {
                    strInfo += "\n";
                }
                strInfo += InfoSeperater(lcdValues[i]);
            }

            strInfo += ChooseCorrectInfoBuilder(value2, lcdValues[i], value1.Settings.Show && lcdValues[i].SplitPossible);
        }

        lcdValues[i].DisplayString(strInfo);
    }
}

void DisplaySpecialInfo<T>(List<T> blockData, LCDValue lcdValue, string textModule, int selectetInfo, bool expandInfo, bool maintenance) where T : BasicBlockValues
{
    string strInfo = textModule + ":";

    if (blockData.Count == 0)
    {
        lcdValue.DisplayString(strInfo + "\n\nNo Systems selected");
        return;
    }


    string arrow = "=> ";
    int spaces = (int)(lcdValue.LCD.MeasureStringInPixels(new StringBuilder(arrow), lcdValue.Font, lcdValue.FontSize).X / lcdValue.SpaceWidth);


    if (expandInfo)
    {
        if (maintenance)
        {
            lcdValue.DisplayString(strInfo + "\n\n" + BuildMaintenanceExpandInfo(blockData[selectetInfo], lcdValue, spaces));
            return;
        }

        lcdValue.DisplayString(ChooseCorrectInfoBuilder(blockData[selectetInfo], lcdValue));
        return;
    }


    if (!lcdValue.SmallPanel)
    {
        strInfo += "\n";
    }
    else
    {
        lcdValue.FontSize = 1.1f;
        lcdValue.SpaceWidth = lcdValue.LCD.MeasureStringInPixels(new StringBuilder("  "), lcdValue.Font, lcdValue.FontSize).X - lcdValue.LCD.MeasureStringInPixels(new StringBuilder(" "), lcdValue.Font, lcdValue.FontSize).X;
        spaces = (int)(lcdValue.LCD.MeasureStringInPixels(new StringBuilder(arrow), lcdValue.Font, lcdValue.FontSize).X / lcdValue.SpaceWidth);
    }


    string[] strSelectedInfo = new string[blockData.Count];

    for (int i = 0; i < strSelectedInfo.Length; i++)
    {
        if (i == selectetInfo)
        {
            strSelectedInfo[i] = arrow;
        }
        else
        {
            strSelectedInfo[i] = new string(' ', spaces);
        }
    }

    int newLines = 2;

    if (lcdValue.SqueezeSpecialInfo || blockData.Count > 7)
    {
        newLines = 1;
    }


    for (int i = 0; i < blockData.Count; i++)
    {
        string rightInfo;

        if (blockData[i].Amount == 0)
        {
            if (!lcdValue.SmallPanel)
            {
                rightInfo = "None found!";
            }
            else
            {
                rightInfo = "None!";
            }
        }
        else if (maintenance)
        {
            if (blockData[i] is FunctionalBlockValues)
            {
                FunctionalBlockValues blockDataTemp = blockData[i] as FunctionalBlockValues;
                rightInfo = blockDataTemp.GetAlerts().ToString() + " ALERTS";
            }
            else
            {
                rightInfo = blockData[i].GetAlerts().ToString() + " ALERTS";
            }
        }
        else if (!lcdValue.SmallPanel)
        {
            rightInfo = "Amount: " + blockData[i].Amount.ToString();
        }
        else
        {
            rightInfo = blockData[i].Amount.ToString();
        }


        string[] strLine = new string[] { strSelectedInfo[i] + AddSpaceBeforeUpperCase(blockData[i].BlockName.ToString()).ToUpper() + ": ", rightInfo };
        strInfo += new string('\n', newLines) + FormatLineToScreen(lcdValue, strLine, true, 5f);
    }

    lcdValue.DisplayString(strInfo);
}


/*===============
        CreateInfoStrings
        ===============*/

string ChooseCorrectInfoBuilder<T>(T value, LCDValue lcdValue, bool split = false) where T : BasicBlockValues
{
    string strBlockName = AddSpaceBeforeUpperCase(value.BlockName.ToString());

    string strInfo = strBlockName.ToUpper() + ":\n\n";

    if (value.Amount == 0)
    {
        return strInfo + "No " + strBlockName + " found!\n\n\n\n";
    }


    if (value.BlockName == BlockCategory.Batteries)
    {
        BatteryValues batteryValues = value as BatteryValues;
        if (batteryValues != null)
        {
            strInfo += BuildBatteryInfo(batteryValues, lcdValue);
        }
    }
    else if (value.BlockName == BlockCategory.Reactors)
    {
        ReactorValues reactorValues = value as ReactorValues;
        if (reactorValues != null)
        {
            strInfo += BuildReactorInfo(reactorValues, lcdValue, split);
        }
    }
    else if (value.BlockName == BlockCategory.HydrogenEngines)
    {
        HydrogenEngineValues hydrogenEngineValues = value as HydrogenEngineValues;
        if (hydrogenEngineValues != null)
        {
            strInfo += BuildHydrogenEngineInfo(hydrogenEngineValues, lcdValue);
        }
    }
    else if (value.BlockName == BlockCategory.CargoContainers)
    {
        CargoValues cargoValues = value as CargoValues;
        if (cargoValues != null)
        {
            strInfo += BuildCargoInfo(cargoValues, lcdValue);
        }
    }
    else if (value.BlockName == BlockCategory.HydrogenTanks || value.BlockName == BlockCategory.OxygenTanks)
    {
        GasTankValues gasTankValues = value as GasTankValues;
        if (gasTankValues != null)
        {
            strInfo += BuildGasTankInfo(gasTankValues, lcdValue);
        }
    }
    else if (value.BlockName == BlockCategory.SolarPanels || value.BlockName == BlockCategory.WindTurbines)
    {
        RenewablesValues renewablesValues = value as RenewablesValues;
        if (renewablesValues != null)
        {
            strInfo += BuildRenewablesInfo(renewablesValues, lcdValue, split);
        }
    }

    return strInfo;
}

string BuildBatteryInfo(BatteryValues batteryValues, LCDValue lcdValue)
{
    string[] strBattery = new string[3];

    string[] strStoredPowerHeader = new string[] { "Stored Power: ", $"{AmountFormatterPower(batteryValues.CurrentStoredPower)}h / {AmountFormatterPower(batteryValues.MaxStoredPower)}h" };
    strBattery[0] = FormatLineToScreen(lcdValue, strStoredPowerHeader) + "\n" + BuildPercentageBar(lcdValue, batteryValues.CurrentStoredPower / batteryValues.MaxStoredPower);


    float totalOutput = batteryValues.CurrentOutput - batteryValues.CurrentInput;
    float outputRatio;
    string[] strOutputHeader;

    if (totalOutput < 0)
    {
        outputRatio = (totalOutput * -1) / batteryValues.MaxInput;
        strOutputHeader = new string[] { "Input: ", $"{AmountFormatterPower(totalOutput * -1)} / {AmountFormatterPower(batteryValues.MaxInput)}" };
    }
    else
    {
        if (batteryValues.MaxOutput == 0f)
        {
            outputRatio = 0f;
        }
        else
        {
            outputRatio = totalOutput / batteryValues.MaxOutput;
        }
        strOutputHeader = new string[] { "Output: ", $"{AmountFormatterPower(totalOutput)} / {AmountFormatterPower(batteryValues.MaxOutput)}" };
    }

    strBattery[1] = FormatLineToScreen(lcdValue, strOutputHeader) + "\n" + BuildPercentageBar(lcdValue, outputRatio);


    string[] strTimeLeft = BuildConsumptionTime(totalOutput, batteryValues.MaxStoredPower, batteryValues.CurrentStoredPower, 3600);
    strBattery[2] = strTimeLeft[0] + "\n" + strTimeLeft[1] + " " + strTimeLeft[2];


    return strBattery[0] + "\n\n" + strBattery[1] + "\n\n" + strBattery[2];
}

string BuildReactorInfo(ReactorValues reactorValues, LCDValue lcdValue, bool split = false)
{
    string[] strReactor = new string[3];

    string[] strOutputHeader = new string[] { "Output: ", $"{AmountFormatterPower(reactorValues.CurrentOutput)} / {AmountFormatterPower(reactorValues.MaxOutput)}" };
    strReactor[0] = FormatLineToScreen(lcdValue, strOutputHeader) + "\n" + BuildPercentageBar(lcdValue, reactorValues.CurrentOutput / reactorValues.MaxOutput);


    string[] uraniumAmount = AmountFormatterUranium(reactorValues.UraniumAmount);
    string[] strUranium = new string[] { "Stored Uranium: ", $"{uraniumAmount[0]} (={uraniumAmount[1]})" };
    strReactor[1] = FormatLineToScreen(lcdValue, strUranium);


    string[] strTimeLeft = BuildConsumptionTime(reactorValues.CurrentOutput, 0.0, reactorValues.UraniumAmount, 3600);
    strReactor[2] = strTimeLeft[0] + "\n" + strTimeLeft[1] + " " + strTimeLeft[2];


    string strReactorFinal = strReactor[0] + "\n\n" + strReactor[1] + "\n";

    if (!split)
    {
        strReactorFinal += "\n";
    }

    strReactorFinal += strReactor[2];

    return strReactorFinal;
}

string BuildGasTankInfo(GasTankValues gasTankValues, LCDValue lcdValue)
{
    string[] strGasTanks = new string[2];

    string[] strCapacityHeader = new string[] { "Capacity: ", $"{AmountFormatterVolume(gasTankValues.CurrentStoredGas)} / {AmountFormatterVolume(gasTankValues.Capacity)}" };
    strGasTanks[0] = FormatLineToScreen(lcdValue, strCapacityHeader) + "\n" + BuildPercentageBar(lcdValue, (float)gasTankValues.CurrentStoredGas / gasTankValues.Capacity);


    string[] strTimeLeft = BuildConsumptionTime(gasTankValues.CapacityChangePerSec, gasTankValues.Capacity, gasTankValues.CurrentStoredGas, 1, gasTankValues.IsUpdating);
    strGasTanks[1] = strTimeLeft[0] + "\n" + strTimeLeft[1] + " " + strTimeLeft[2];


    return strGasTanks[0] + "\n\n" + strGasTanks[1];
}

string BuildHydrogenEngineInfo(HydrogenEngineValues hydrogenEngineValues, LCDValue lcdValue)
{
    string[] strHydrogenEngines = new string[2];

    string[] strOutputHeader = new string[] { "Output: ", $"{AmountFormatterPower(hydrogenEngineValues.CurrentOutput)} / {AmountFormatterPower(hydrogenEngineValues.MaxOutput)}" };
    strHydrogenEngines[0] = FormatLineToScreen(lcdValue, strOutputHeader) + "\n" + BuildPercentageBar(lcdValue, hydrogenEngineValues.CurrentOutput / hydrogenEngineValues.MaxOutput);


    string[] strCapacityHeader = new string[] { "Capacity: ", $"{AmountFormatterVolume(hydrogenEngineValues.CurrentStoredGas)} / {AmountFormatterVolume(hydrogenEngineValues.Capacity)}" };
    strHydrogenEngines[1] = FormatLineToScreen(lcdValue, strCapacityHeader) + "\n" + BuildPercentageBar(lcdValue, hydrogenEngineValues.CurrentStoredGas / hydrogenEngineValues.Capacity);


    return strHydrogenEngines[0] + "\n\n" + strHydrogenEngines[1];
}

string BuildRenewablesInfo(RenewablesValues renewablesValues, LCDValue lcdValue, bool split = false)
{
    string[] strRenewables = new string[3];

    string strEfficiency = "Unknown";

    if (renewablesValues.BlockName == BlockCategory.SolarPanels)
    {
        strEfficiency = "Sun Strength";
    }
    else if (renewablesValues.BlockName == BlockCategory.WindTurbines)
    {
        strEfficiency = "Vicinity Condition";
    }


    strRenewables[0] = strEfficiency + ":\n" + BuildPercentageBar(lcdValue, renewablesValues.MaxOutput / renewablesValues.PotentialOutput);


    string[] strOutput = new string[] { "Output:", AmountFormatterPower(renewablesValues.CurrentOutput) + " / " + AmountFormatterPower(renewablesValues.MaxOutput) };
    strRenewables[1] = FormatLineToScreen(lcdValue, strOutput);

    string[] strNormalOutput = new string[] { "Optimal Output:", AmountFormatterPower(renewablesValues.PotentialOutput) };
    strRenewables[2] = FormatLineToScreen(lcdValue, strNormalOutput);


    string strRenewablesFinal = strRenewables[0] + "\n\n" + strRenewables[1] + "\n";

    if (!split)
    {
        strRenewablesFinal += "\n";
    }

    strRenewablesFinal += strRenewables[2];


    return strRenewablesFinal;
}

string BuildCargoInfo(CargoValues cargoValues, LCDValue lcdValue)
{
    string[] strCargo = new string[3];

    string[] strVolumeHeader = new string[] { "Volume: ", $"{AmountFormatterVolume(cargoValues.CurrentVolume)} / {AmountFormatterVolume(cargoValues.MaxVolume)}" };
    strCargo[0] = FormatLineToScreen(lcdValue, strVolumeHeader) + "\n" + BuildPercentageBar(lcdValue, (float)(cargoValues.CurrentVolume / cargoValues.MaxVolume));


    string[] strMassHeader = new string[] { "Mass: ", AmountFormatterWeight(cargoValues.Mass) };
    strCargo[1] = FormatLineToScreen(lcdValue, strMassHeader, true);

    string strCargoFinal = strCargo[0] + "\n\n" + strCargo[1] + "\n";


    int lines = (int)((lcdValue.RealScreenSize.Y - lcdValue.LCD.MeasureStringInPixels(new StringBuilder(strCargoFinal), lcdValue.Font, lcdValue.FontSize).Y) / lcdValue.LineHeight);
    strCargo[2] = "Items:\n" + ItemList(cargoValues.Items, lcdValue, lines - 1);


    return strCargoFinal + strCargo[2];
}

string BuildMaintenanceExpandInfo<T>(T blockValue, LCDValue lcdValue, int spaces) where T : BasicBlockValues
{
    string[] strMaintenanceInfo = new string[5];
    strMaintenanceInfo[0] = AddSpaceBeforeUpperCase(blockValue.BlockName.ToString()).ToUpper() + ":";

    string emptyState = "Empty";

    if (blockValue.BlockName == BlockCategory.SolarPanels || blockValue.BlockName == BlockCategory.WindTurbines)
    {
        emptyState = "Not Producing";
    }

    string strSpaces = new string(' ', spaces * 2);


    string[] strDisabled = new string[2];
    strDisabled[0] = strSpaces + "Disabled: ";

    if (blockValue is FunctionalBlockValues)
    {
        FunctionalBlockValues blockValueTemp = blockValue as FunctionalBlockValues;
        strDisabled[1] = blockValueTemp.DisabledBlocks.ToString() + " / " + blockValue.Amount.ToString();
    }
    else
    {
        strDisabled[1] = "-----";
    }

    strMaintenanceInfo[1] = FormatLineToScreen(lcdValue, strDisabled, true, 5f);

    string[] strDamaged = new string[] { strSpaces + "Damaged: ", blockValue.DamagedBlocks.ToString() + " / " + blockValue.Amount.ToString() };
    strMaintenanceInfo[2] = FormatLineToScreen(lcdValue, strDamaged, true, 5f);

    string[] strHacked = new string[] { strSpaces + "Being Hacked: ", blockValue.HackedBlocks.ToString() + " / " + blockValue.Amount.ToString() };
    strMaintenanceInfo[3] = FormatLineToScreen(lcdValue, strHacked, true, 5f);

    string[] strEmpty = new string[] { strSpaces + emptyState + ": ", blockValue.EmptyBlocks.ToString() + " / " + blockValue.Amount.ToString() };
    strMaintenanceInfo[4] = FormatLineToScreen(lcdValue, strEmpty, true, 5f);


    string strMaintenanceInfoFinal = new string(' ', spaces) + strMaintenanceInfo[0];

    for (int i = 1; i < strMaintenanceInfo.Length; i++)
    {
        strMaintenanceInfoFinal += "\n" + strMaintenanceInfo[i];
    }

    return strMaintenanceInfoFinal;
}


/*============
        Search methods
        =============*/

List<T> SearchForBlocks<T>(SearchSettings searchSettings) where T : class, IMyTerminalBlock
{
    List<T> blocks = new List<T>();

    switch (searchSettings.Mode)
    {
        case 1:
            IMyBlockGroup blockGroup = GridTerminalSystem.GetBlockGroupWithName(searchSettings.NameTag);
            if (blockGroup != null)
            {
                blockGroup.GetBlocksOfType(blocks);
            }
            break;
        case 2:
            GridTerminalSystem.GetBlocksOfType(blocks, x => x.CustomName.Contains(searchSettings.NameTag));
            break;
        default:
            GridTerminalSystem.GetBlocksOfType(blocks);
            break;
    }

    if (!searchSettings.Subgrids)
    {
        blocks = KeepOnlyBlocksOnSameGrid(blocks);
    }

    return blocks;
}

List<IMyPowerProducer> SearchForHydrogenEngines(SearchSettings searchSettings)
{
    List<IMyPowerProducer> blocks = new List<IMyPowerProducer>();

    switch (searchSettings.Mode)
    {
        case 1:
            IMyBlockGroup blockGroup = GridTerminalSystem.GetBlockGroupWithName(searchSettings.NameTag);
            if (blockGroup != null)
            {
                blockGroup.GetBlocksOfType(blocks, x => x.BlockDefinition.TypeIdString.EndsWith("HydrogenEngine"));
            }
            break;
        case 2:
            GridTerminalSystem.GetBlocksOfType(blocks, x => x.CustomName.Contains(searchSettings.NameTag) && x.BlockDefinition.TypeIdString.EndsWith("HydrogenEngine"));
            break;
        default:
            GridTerminalSystem.GetBlocksOfType(blocks, x => x.BlockDefinition.TypeIdString.EndsWith("HydrogenEngine"));
            break;
    }

    if (!searchSettings.Subgrids)
    {
        blocks = KeepOnlyBlocksOnSameGrid(blocks);
    }

    return blocks;
}

List<IMyGasTank> SearchForHydrogenTanks(SearchSettings searchSettings)
{
    List<IMyGasTank> blocks = new List<IMyGasTank>();

    switch (searchSettings.Mode)
    {
        case 1:
            IMyBlockGroup blockGroup = GridTerminalSystem.GetBlockGroupWithName(searchSettings.NameTag);
            if (blockGroup != null)
            {
                blockGroup.GetBlocksOfType(blocks, x => x.BlockDefinition.SubtypeId.Contains("HydrogenTank"));
            }
            break;
        case 2:
            GridTerminalSystem.GetBlocksOfType(blocks, x => x.CustomName.Contains(searchSettings.NameTag) && x.BlockDefinition.SubtypeId.Contains("HydrogenTank"));
            break;
        default:
            GridTerminalSystem.GetBlocksOfType(blocks, x => x.BlockDefinition.SubtypeId.Contains("HydrogenTank"));
            break;
    }

    if (!searchSettings.Subgrids)
    {
        blocks = KeepOnlyBlocksOnSameGrid(blocks);
    }

    return blocks;
}

List<IMyGasTank> SearchForOxygenTanks(SearchSettings searchSettings)
{
    List<IMyGasTank> blocks = new List<IMyGasTank>();

    switch (searchSettings.Mode)
    {
        case 1:
            IMyBlockGroup blockGroup = GridTerminalSystem.GetBlockGroupWithName(searchSettings.NameTag);
            if (blockGroup != null)
            {
                blockGroup.GetBlocksOfType(blocks, x => !x.BlockDefinition.SubtypeId.Contains("HydrogenTank"));
            }
            break;
        case 2:
            GridTerminalSystem.GetBlocksOfType(blocks, x => x.CustomName.Contains(searchSettings.NameTag) && !x.BlockDefinition.SubtypeId.Contains("HydrogenTank"));
            break;
        default:
            GridTerminalSystem.GetBlocksOfType(blocks, x => !x.BlockDefinition.SubtypeId.Contains("HydrogenTank"));
            break;
    }

    if (!searchSettings.Subgrids)
    {
        blocks = KeepOnlyBlocksOnSameGrid(blocks);
    }

    return blocks;
}

List<T> KeepOnlyBlocksOnSameGrid<T>(List<T> allBlocks) where T : IMyTerminalBlock
{
    List<T> blocksOnSameGrid = new List<T>();

    for (int i = 0; i < allBlocks.Count; i++)
    {
        if (Me.CubeGrid.EntityId == allBlocks[i].CubeGrid.EntityId)
        {
            blocksOnSameGrid.Add(allBlocks[i]);
        }
    }

    return blocksOnSameGrid;
}


/*================
        Specific-Sub-Tasks
        =================*/

ScriptSettings GetScriptSettings(bool reset = false)
{
    string[] headers = new[]
    {
        "SETTINGS:",
        "Please enter the values between the brackets; Example: [value]",
        "Settings with a \"?\" need a [Yes] or [No] as value." ,
        "",
        "WHICH BLOCKS WILL BE SHOWN:",
        "Search mode",
        "This method is used to gather your block information.",
        "0 = All blocks; 1 = All blocks in a group;",
        "2 = All blocks with a custom name tag in their names",
        "",
        "Include Subgrids?",//10
        "Group name / Name tag",
        "Please enter the name of your group or name tag if the search",
        "mode is 1 or 2. If you selected 0 you can ignore this setting.",
        "",
        "LCD NAMES:",
        "Below you can see or change the LCD name tag you need to",
        "add to your LCDs. Every block category will have its own LCD.",
        "",
        "Batteries",
        "Reactors",//20
        "Hydrogen Engines",
        "Hydrogen Tanks",
        "Oxygen Tanks",
        "Solar Panels",
        "Wind Turbines",
        "Cargo Containers",
        "",
        "SPECIAL LCDS:",
        "",
        "Maintenance",//30
        "This interactive LCD will show you all basic information about",
        "your blocks on 1 screen. Check the script arguments to interact.",
        "",
        "Compact",
        "This interactive LCD will is able to switch between all the block",
        "categories on 1 screen. Check the script arguments to interact.",
        "",
        "Reactors + Hydrogen Engines",
        "Hydrogen + Oxygen Tanks",
        "Solar Panles + Wind Turbines",//40
        "The three LCDs above show 2 block categories at the same time.",
        "Doesn't work on: Text Panel (large & small), Transparent LCD (small).",
        "",
        "",
        "ADVANCED SETTINGS:",
        "",
        "You can disable block categories below. They won't be shown on",
        "your LCDs anymore. I recommend using this if you don't want a",
        "category to be visualized or don't have the blocks on your grid.",
        "This will prevent the script gathering info about that block",//50
        "category and thus improve performance.",
        "Show Batteries?",
        "Show Reactors?",
        "Show Hydrogen Engines?",
        "Show Hydrogen Tanks?",
        "Show Oxygen Tanks?",
        "Show Solar Panels?",
        "Show Wind Turbines?",
        "Show Cargo Containers?",
        "Show Maintenance Screen?",//60
        "Show Compact Screen?",
        "",
        "List Ores and Ingots First?",
        "This setting only affects the Cargo Container LCD screen(s). You",
        "can decide whether to list ores and ingots or other items first",
        "",
        "Consumption calculation time frame",
        "You can change the calculation time frame above. Higher values",
        "will result in more accurate estimations but won't react to",
        "changes of the consumption as fast. Enter the value in seconds",//70
        "",
        "Update Frequency",
        "You can change how often the script repeats itself here.",
        "100: Every 100 ticks: Low performance impact, but less fluent",
        "visualization. Consumption calculation is less accurate in small",
        "time frames.",
        "10: Every 10 ticks: Higher performance impact, but more fluent",
        "visualization. Very accurate consumption calculation.",
        "1: Every tick: Very high performance impact. You shouldn't use this!",
        "IMPORTANT: Press the 'Recompile' button to update this setting!",//80
        ""
    };

    int[] boolLines = new int[] { 10, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 63 };
    int[] intLines = new int[] { 5, 67, 72 };
    int[] stringLines = new int[] { 11, 19, 20, 21, 22, 23, 24, 25, 26, 30, 34, 38, 39, 40 };
    int[] valueLines = boolLines.Concat(intLines).Concat(stringLines).ToArray();

    string[] customData = Me.CustomData.Split('\n');

    if (customData.Length != headers.Length || reset)
    {
        return ResetSettings(headers, boolLines, intLines, stringLines);
    }

    int[] valuesInt = new int[intLines.Length];
    int counterInt = 0;
    bool[] valuesBool = new bool[boolLines.Length];
    int counterBool = 0;
    string[] valuesString = new string[stringLines.Length];
    int counterString = 0;
    bool error = false;

    for (int i = 0; i < customData.Length; i++)
    {
        if (!valueLines.Contains(i))
        {
            continue;
        }

        string[] line = customData[i].Split(':');

        if (line.Length != 2 || line[0] != headers[i])
        {
            return ResetSettings(headers, boolLines, intLines, stringLines);
        }

        string[] valueTemp = line[1].Split('[');

        if (valueTemp.Length != 2 )
        {
            return ResetSettings(headers, boolLines, intLines, stringLines);
        }

        string[] value = valueTemp[1].Split(']');

        if (value.Length != 2)
        {
            return ResetSettings(headers, boolLines, intLines, stringLines);
        }

        if (stringLines.Contains(i))
        {
            if (value[0] == "")
            {
                error = true;
            }
            valuesString[counterString] = value[0];
            counterString++;
        }
        else if (boolLines.Contains(i))
        {

            error = !TrySpecialParse(value[0], out valuesBool[counterBool]);
            counterBool++;
        }
        else if (intLines.Contains(i))
        {
            error = !Int32.TryParse(value[0], out valuesInt[counterInt]);
            counterInt++;
        }
        if (error)
        {
            return ResetSettings(headers, boolLines, intLines, stringLines);
        }
    }

    updateFrequency = IntToUpdateFrequency(valuesInt[2]);

    return new ScriptSettings(valuesBool, valuesInt, valuesString);
}

ScriptSettings ResetSettings(string[] headers, int[] boolLines, int[] intLines, int[] stringLines)
{
    bool[] defaultBools;
    int[] defaultInts;
    string[] defaultStrings;

    ScriptSettings scriptSettings = new ScriptSettings();
    scriptSettings.GetValues(out defaultBools, out defaultInts, out defaultStrings);

    int counterBool = 0;
    int counterInt = 0;
    int counterString = 0;

    string customData = "";

    for (int i = 0; i < headers.Length; i++)
    {
        customData += headers[i];

        if (stringLines.Contains(i))
        {
            customData += ": [" + defaultStrings[counterString] + "]";
            counterString++;
        }
        else if (boolLines.Contains(i))
        {
            customData += ": [" + SpecialParseToString(defaultBools[counterBool]) + "]";
            counterBool++;
        }
        else if (intLines.Contains(i))
        {
            customData += ": [" + defaultInts[counterInt] + "]";
            counterInt++;
        }

        if (i < headers.Length - 1)
        {
            customData += "\n";
        }
    }

    Me.CustomData = customData;
    updateFrequency = UpdateFrequency.Update100;
    return scriptSettings;
}

List<BasicBlockValues> GatherAllBlockData(ScriptSettings scriptSettings, int ticksPerTimeFrame)
{
    List<BasicBlockValues> allBlockData = new List<BasicBlockValues>();

    if (scriptSettings.Battery.Show)
    {
        BatteryValues batteryValues = new BatteryValues();
        batteryValues.GetData(SearchForBlocks<IMyBatteryBlock>(scriptSettings.Search), scriptSettings.Battery);
        allBlockData.Add(batteryValues);
    }
    if (scriptSettings.Reactor.Show)
    {
        ReactorValues reactorValues = new ReactorValues();
        reactorValues.GetData(SearchForBlocks<IMyReactor>(scriptSettings.Search), scriptSettings.Reactor);
        allBlockData.Add(reactorValues);
    }
    if (scriptSettings.HydrogenEngine.Show)
    {
        HydrogenEngineValues hydrogenEngineValues = new HydrogenEngineValues();
        hydrogenEngineValues.GetData(SearchForHydrogenEngines(scriptSettings.Search), scriptSettings.HydrogenEngine);
        allBlockData.Add(hydrogenEngineValues);
    }
    if (scriptSettings.HydrogenTank.Show)
    {
        GasTankValues hydrogenTankValues = new GasTankValues();
        hydrogenTankValues.GetData(SearchForHydrogenTanks(scriptSettings.Search), scriptSettings.HydrogenTank, BlockCategory.HydrogenTanks, ref hydrogenCapacities, scriptSettings.ConsumptionTimeFrameInSeconds, ticksPerTimeFrame);
        allBlockData.Add(hydrogenTankValues);
    }
    if (scriptSettings.OxygenTank.Show)
    {
        GasTankValues oxygenTankValues = new GasTankValues();
        oxygenTankValues.GetData(SearchForOxygenTanks(scriptSettings.Search), scriptSettings.OxygenTank, BlockCategory.OxygenTanks, ref oxygenCapacities, scriptSettings.ConsumptionTimeFrameInSeconds, ticksPerTimeFrame);
        allBlockData.Add(oxygenTankValues);
    }
    if (scriptSettings.SolarPanel.Show)
    {
        RenewablesValues solarPanelValues = new RenewablesValues();
        solarPanelValues.GetData(SearchForBlocks<IMySolarPanel>(scriptSettings.Search), scriptSettings.SolarPanel);
        allBlockData.Add(solarPanelValues);
    }
    if (scriptSettings.WindTurbine.Show)
    {
        RenewablesValues windTurbiuneValues = new RenewablesValues();
        windTurbiuneValues.GetData(SearchForBlocks<IMyWindTurbine>(scriptSettings.Search), scriptSettings.WindTurbine);
        allBlockData.Add(windTurbiuneValues);
    }
    if (scriptSettings.CargoContainer.Show)
    {
        CargoValues cargoValues = new CargoValues();
        cargoValues.GetData(SearchForBlocks<IMyCargoContainer>(scriptSettings.Search), scriptSettings.CargoContainer, scriptSettings.ListOreFirst);
        allBlockData.Add(cargoValues);
    }

    return allBlockData;
}

List<LCDValue> GetLCDValues(string name, bool split = false)
{
    List<LCDValue> lcdValues = new List<LCDValue>();
    List<IMyTextPanel> lcds = new List<IMyTextPanel>();

    GridTerminalSystem.GetBlocksOfType(lcds, x => x.CustomName.Contains(name));


    for (int i = 0; i < lcds.Count; i++)
    {
        LCDValue lcdValue = new LCDValue();

        lcdValue.LCD = lcds[i];
        lcdValue.ContentType = ContentType.TEXT_AND_IMAGE;
        lcdValue.TextAlignment = TextAlignment.LEFT;
        lcdValue.TextPadding = 2f;
        lcdValue.FontSize = 1f;
        lcdValue.SmallPanel = false;
        lcdValue.SplitPossible = true;
        lcdValue.SqueezeSpecialInfo = false;

        if (lcdValue.LCD.Font != "Monospace")
        {
            lcdValue.Font = lcdValue.LCD.Font;
        }
        else
        {
            lcdValue.Font = "Debug";
        }


        if (lcdValue.LCD.BlockDefinition.SubtypeId == "SmallTextPanel" || lcdValue.LCD.BlockDefinition.SubtypeId == "TransparentLCDSmall")
        {
            lcdValue.TextPadding = 4f;
            lcdValue.FontSize = 1.4f;
            lcdValue.SmallPanel = true;
            lcdValue.SplitPossible = false;
        }
        else if (lcdValue.LCD.BlockDefinition.SubtypeId == "LargeTextPanel")
        {
            lcdValue.SplitPossible = false;
            lcdValue.SqueezeSpecialInfo = true;
        }
        else if (lcdValue.LCD.BlockDefinition.SubtypeId == "LargeLCDPanelWide" || lcdValue.LCD.BlockDefinition.SubtypeId == "SmallLCDPanelWide")
        {
            lcdValue.SqueezeSpecialInfo = true;

            if (!split)
            {
                lcdValue.FontSize = 1.65f;
            }
        }
        else if (lcdValue.LCD.BlockDefinition.SubtypeId == "LargeLCDPanel5x3")
        {
            lcdValue.SqueezeSpecialInfo = true;

            if (split)
            {
                lcdValue.FontSize = 0.6f;
            }
        }

        lcdValue.RealScreenSize = lcdValue.LCD.SurfaceSize * (1 - (2 * lcdValue.TextPadding / 100));
        lcdValue.BarLength = (int)((lcdValue.RealScreenSize.X - lcdValue.LCD.MeasureStringInPixels(new StringBuilder("[] 100.00 %"), lcdValue.Font, lcdValue.FontSize).X) / (lcdValue.LCD.MeasureStringInPixels(new StringBuilder("II"), lcdValue.Font, lcdValue.FontSize).X - lcdValue.LCD.MeasureStringInPixels(new StringBuilder("I"), lcdValue.Font, lcdValue.FontSize).X));

        if (lcdValue.BarLength < 0)
        {
            lcdValue.BarLength = 0;
        }

        lcdValue.SpaceWidth = lcdValue.LCD.MeasureStringInPixels(new StringBuilder("  "), lcdValue.Font, lcdValue.FontSize).X - lcdValue.LCD.MeasureStringInPixels(new StringBuilder(" "), lcdValue.Font, lcdValue.FontSize).X;
        lcdValue.LineHeight = lcdValue.LCD.MeasureStringInPixels(new StringBuilder("a\na"), lcdValue.Font, lcdValue.FontSize).Y - lcdValue.LCD.MeasureStringInPixels(new StringBuilder("a"), lcdValue.Font, lcdValue.FontSize).Y;

        lcdValues.Add(lcdValue);
    }

    return lcdValues;
}

string BuildPercentageBar(LCDValue lcdValue, float ratio)
{
    int barRatio = (int)(ratio * lcdValue.BarLength);

    if (barRatio > lcdValue.BarLength)
    {
        barRatio = lcdValue.BarLength;
    }

    string[] percentageBar = new string[] { $"[{new string('I', barRatio)}{new string('∙', lcdValue.BarLength - barRatio)}] ", $"{ Math.Floor(ratio * 10000) / 100 } %" };

    return FormatLineToScreen(lcdValue, percentageBar, true);
}

string[] BuildConsumptionTime(double change, double maxCapacity, double currentCapacity, int timeUnitInSeconds = 1, bool isUpdating = false)
{
    string[] strTimeLeft = new string[3];

    if (isUpdating)
    {
        strTimeLeft[0] = "Updating...";
        return strTimeLeft;
    }
    if (change == 0.0)
    {
        strTimeLeft[0] = "No Consumption";
        return strTimeLeft;
    }


    int timeInSeconds;

    if (change > 0.0)
    {
        timeInSeconds = (int)(currentCapacity / change * timeUnitInSeconds);
        strTimeLeft[0] = "Depleted in:";
    }
    else
    {
        timeInSeconds = (int)((maxCapacity - currentCapacity) / change * timeUnitInSeconds);
        strTimeLeft[0] = "Recharged in";
    }

    string[] strConsumption = TimeFormatter(timeInSeconds);
    strTimeLeft[1] = strConsumption[0];
    strTimeLeft[2] = strConsumption[1];

    return strTimeLeft;
}

string FormatLineToScreen(LCDValue lcdValue, string[] stringParts, bool overrideNewLine = false, float rightPadding = 0f)
{
    if (lcdValue.SmallPanel && !overrideNewLine)
    {
        return stringParts[0] + "\n" + stringParts[1];
    }


    float lineWidth = lcdValue.LCD.MeasureStringInPixels(new StringBuilder(stringParts[0] + stringParts[1]), lcdValue.Font, lcdValue.FontSize).X;
    int spaces = (int)((lcdValue.RealScreenSize.X * (1 - (rightPadding / 100)) - lineWidth) / lcdValue.SpaceWidth);

    if (spaces < 0)
    {
        spaces = 0;
    }

    string header = stringParts[0] + new string(' ', spaces) + stringParts[1];

    return header;
}

string ItemList(List<ItemValue> items, LCDValue lcdValue, int lines)
{
    string strItems = "";

    if (items.Count == 0)
    {
        return "<EMPTY>";
    }

    int min = Math.Min(items.Count, lines);

    for (int i = 0; i < min - 1; i++)
    {
        strItems += FormatLineToScreen(lcdValue, new string[] { "- " + DecodeItemName(items[i].Type.TypeId, items[i].Type.SubtypeId) + ": ", AmountFormatterItem(items[i].Amount, items[i].Type.TypeId) }, true) + "\n";
    }

    if (items.Count <= lines)
    {
        int index = min - 1;
        strItems += FormatLineToScreen(lcdValue, new string[] { "- " + DecodeItemName(items[index].Type.TypeId, items[index].Type.SubtypeId) + ": ", AmountFormatterItem(items[index].Amount, items[index].Type.TypeId) }, true);
    }
    else
    {
        strItems += $"  [And {items.Count - lines + 1} other Items]";
    }

    return strItems;
}

string InfoSeperater(LCDValue lcdValue)
{
    float underlineWidth = lcdValue.LCD.MeasureStringInPixels(new StringBuilder("__"), lcdValue.Font, lcdValue.FontSize).X - lcdValue.LCD.MeasureStringInPixels(new StringBuilder("_"), lcdValue.Font, lcdValue.FontSize).X;
    int underlineAmount = (int)(lcdValue.RealScreenSize.X / underlineWidth);

    return "\n" + new string('_', underlineAmount) + "\n\n";
}

void ClearLCDs(ScriptSettings scriptSettings)
{
    CategorySettings[] categorySettings = scriptSettings.GetCategories();

    for (int i = 0; i < categorySettings.Length; i++)
    {
        if (categorySettings[i].Show)
        {
            clearedLCDs[i] = false;
        }
    }

    for (int i = 0; i < clearedLCDs.Length; i++)
    {
        if (!categorySettings[i].Show && !clearedLCDs[i])
        {
            ClearLCDsExtension(categorySettings[i].LCD);
            clearedLCDs[i] = true;
        }
    }
}

void ClearLCDsExtension(string name)
{
    List<IMyTextPanel> lcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(lcds, x => x.CustomName.Contains(name));

    for (int i = 0; i < lcds.Count; i++)
    {
        lcds[i].WriteText("");
    }
}

void Reset(bool hardReset = false)
{
    hydrogenCapacities.Clear();
    oxygenCapacities.Clear();
    Array.Clear(clearedLCDs, 0, clearedLCDs.Length);
    selectedMaintenanceInfo = 0;
    expandMaintenanceInfo = false;
    selectedCompactInfo = 0;
    expandCompactInfo = false;

    if (hardReset)
    {
        GetScriptSettings(true);
    }
}


/*==================
         Additional Functions
         ===================*/

bool CalculateTicksPerTimeFrame(UpdateFrequency updateFrequency, int timeFrameInSeconds, out int ticksPerTimeFrame)
{
    double ticksPerSecond = 0.0;
    ticksPerTimeFrame = 0;

    if (updateFrequency == UpdateFrequency.Update1)
    {
        ticksPerSecond = 60.0;
    }
    else if (updateFrequency == UpdateFrequency.Update10)
    {
        ticksPerSecond = 6.0;
    }
    else if (updateFrequency == UpdateFrequency.Update100 && timeFrameInSeconds % 5 == 0) //Ensure the ticks being synchron to full seconds
    {
        ticksPerSecond = 0.6;
    }
    else
    {
        return false;
    }

    ticksPerTimeFrame = (int)Math.Round(ticksPerSecond * timeFrameInSeconds);
    return true;
}

UpdateFrequency IntToUpdateFrequency(int value)
{
    if (value == 100)
    {
        return UpdateFrequency.Update100;
    }
    if (value == 10)
    {
        return UpdateFrequency.Update10;
    }
    if (value == 1)
    {
        return UpdateFrequency.Update1;
    }

    return UpdateFrequency.Update100;
}

string UpdateFrequencyToString(UpdateFrequency updateFrequency)
{
    if (updateFrequency == UpdateFrequency.Update100)
    {
        return "100";
    }
    if (updateFrequency == UpdateFrequency.Update10)
    {
        return "10";
    }
    if (updateFrequency == UpdateFrequency.Update1)
    {
        return "1";
    }

    return "100";
}

string AddSpaceBeforeUpperCase(string text)
{
    string result = "";
    List<int> spacePositions = new List<int>();

    for (int i = 1; i < text.Length; i++)
    {
        if (Char.IsUpper(text, i))
        {
            spacePositions.Add(i);
        }
    }

    if (spacePositions.Count == 0)
    {
        return text;
    }

    result = text.Substring(0, spacePositions[0]);

    for (int i = 0; i < spacePositions.Count - 1; i++)
    {
        result += " " + text.Substring(spacePositions[i], spacePositions[i + 1] - spacePositions[i]);
    }

    result += " " + text.Substring(spacePositions[spacePositions.Count - 1], text.Length - spacePositions[spacePositions.Count - 1]);

    return result;
}

string TypeToString(Type type)
{
    if (type == typeof(bool))
    {
        return "Bool";
    }
    if (type == typeof(int))
    {
        return "Int";
    }
    if (type == typeof(string))
    {
        return "String";
    }
    if (type == typeof(bool[]))
    {
        return "Array:Bool";
    }
    if (type == typeof(int[]))
    {
        return "Array:Int";
    }
    if (type == typeof(string[]))
    {
        return "Array:String";
    }
    if (type == typeof(UpdateFrequency))
    {
        return "UpdateFrequency";
    }

    return type.ToString();
}

bool TrySpecialParse(string stringValue, out bool boolValue)
{
    string value = stringValue.ToLower();

    if (value == "yes")
    {
        boolValue = true;
    }
    else if (value == "no")
    {
        boolValue = false;
    }
    else
    {
        boolValue = false;
        return false;
    }

    return true;
}

string SpecialParseToString(bool boolValue)
{
    if (boolValue)
    {
        return "Yes";
    }

    return "No";
}

string[] TimeFormatter(int timeInSeconds)
{
    string[] strtime = new string[2];

    if (timeInSeconds < 0)
    {
        timeInSeconds = -timeInSeconds;
    }

    if (timeInSeconds < 60)
    {
        strtime[0] = TimeFormatterExtension2(timeInSeconds, " Second");
    }

    else if (timeInSeconds < 3600)
    {
        strtime = TimeFormatterExtension1(timeInSeconds, 60, 0, " Minute", " Second");
    }

    else if (timeInSeconds < 86400)
    {
        strtime = TimeFormatterExtension1(timeInSeconds, 3600, 60, " Hour", " Minute");
    }

    else if (timeInSeconds < 31536000)
    {
        strtime = TimeFormatterExtension1(timeInSeconds, 86400, 3600, " Day", " Hour");
    }

    else
    {
        strtime = TimeFormatterExtension1(timeInSeconds, 31536000, 86400, " Year", " Day");
    }

    return strtime;
}

string[] TimeFormatterExtension1(int timeInSeconds, int factor1, int factor2, string unit1, string unit2)
{
    string[] strtime = new string[2];
    strtime[0] = TimeFormatterExtension2(timeInSeconds / factor1, unit1);

    if ((timeInSeconds % factor1) >= factor2)
    {
        if (factor2 == 0)
        {
            factor2 = 1;
        }

        strtime[1] = TimeFormatterExtension2(timeInSeconds % factor1 / factor2, unit2);
    }

    return strtime;
}

string TimeFormatterExtension2(int timeInSeconds, string unit)
{
    if (timeInSeconds != 1)
    {
        return timeInSeconds.ToString() + unit + "s";
    }

    else
    {
        return "1" + unit;
    }
}

string DecodeItemName(string typeId, string subTypeId)
{
    if (item.ContainsKey(typeId))
    {
        return (item[typeId].ContainsKey(subTypeId)) ? item[typeId][subTypeId] : subTypeId;
    }
    if (itemMisc.ContainsKey(subTypeId))
    {
        return itemMisc[subTypeId];
    }

    return subTypeId;
}

string AmountFormatterItem(double amount, string typeId, int decimals = 2)
{
    if (!typeId.EndsWith("_Ore") && !typeId.EndsWith("_Ingot"))
    {
        return AmountFormatter(amount, 0) + "x";
    }

    return AmountFormatterWeight(amount, decimals);
}

string AmountFormatterWeight(double amount, int decimals = 2)
{
    if (amount < 1000)
    {
        return Math.Round(amount, decimals).ToString() + " kg";
    }

    double ton = Math.Round(amount / 1000, decimals);

    if (ton != 1)
    {
        return AmountFormatter(ton, decimals) + " tons";
    }

    return "1 ton";
}

string[] AmountFormatterUranium(double amount, int decimals = 2)
{
    string[] Uranium = new string[2];

    if (amount < 1000)
    {
        Uranium[0] = Math.Round(amount, decimals).ToString() + " kg";
        Uranium[1] = "MWh";
        if (amount < 1)
        {
            Uranium[1] = "KWh";
        }
        return Uranium;
    }

    double ton = Math.Round(amount / 1000, decimals);
    Uranium[1] = "GWh";

    if (ton != 1)
    {
        Uranium[0] = AmountFormatter(ton, decimals) + " tons";
        return Uranium;
    }

    Uranium[0] = "1 ton";
    return Uranium;

}

string AmountFormatterPower(double amount, int decimals = 2)
{
    if (amount < 0.001)
    {
        return Math.Round(amount * 1000000, decimals) + " W";
    }
    if (amount < 1)
    {
        return Math.Round(amount * 1000, decimals) + " KW";
    }
    if (amount < 1000)
    {
        return Math.Round(amount, decimals) + " MW";
    }

    return AmountFormatter(amount / 1000, decimals) + " GW";
}

string AmountFormatterVolume(double amount, int decimals = 2)
{
    if (amount < 1000)
    {
        return Math.Round(amount, decimals) + " L";
    }
    if (amount < 1000000)
    {
        return Math.Round(amount / 1000, decimals) + " KL";
    }
    if (amount < 1000000000)
    {
        return Math.Round(amount / 1000000, decimals) + " ML";
    }

    return AmountFormatter(amount / 1000000000, decimals) + " GL";
}

//Rounded to 2 decimals because of inaccuracies in saving the decimals (0.05 => 0.049999999); (decimal)?
string AmountFormatter(double amount, int decimals = 2)
{
    amount = Math.Round(amount, decimals);

    if (amount < 1000 && amount > -1000)
    {
        return amount.ToString();
    }

    double amountAbs = Math.Abs(amount);
    int intAmount = (int)amountAbs;
    string digits = intAmount.ToString();
    int digitBlocks = (int)Math.Ceiling(digits.Length / 3.0);
    int remainder = digits.Length % 3;

    if (remainder == 0)
    {
        remainder = 3;
    }

    string strAmount = digits.Substring(0, remainder);

    for (int i = 0; i < digitBlocks - 1; i++)
    {
        strAmount += ',' + digits.Substring(remainder, 3);
        remainder += 3;
    }

    if (amountAbs != intAmount)
    {
        strAmount += Math.Round(amountAbs - intAmount, decimals).ToString().Substring(1);
    }

    if (amount < 0)
    {
        strAmount = '-' + strAmount;
    }

    return strAmount;
}