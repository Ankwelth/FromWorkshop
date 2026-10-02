List<IMyTerminalBlock> inventoryBlocks = new List<IMyTerminalBlock>();
List<IMyAssembler> assemblerBlocks = new List<IMyAssembler>();
List<Display> cargoDisplays = new List<Display>();

SortMode sortMode = SortMode.Name;
bool isDescending;
bool connectedGrids;

string version = "1.0.3";

public Program()
        {
            Init();
        }

public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.Terminal && argument == "refresh")
            {
                inventoryBlocks.Clear();
                assemblerBlocks.Clear();
                cargoDisplays.Clear();
                Init();
                return;
            }

            Echo("Tasty's Inventory Display");
            Echo("Version " + version);
            Echo("");
            Echo("Use \"refresh\" as argument to re-initialize screens and configs");
            Echo("");
            Echo("Configuration for all screens in custom data of this PB.");
            Echo("You can also customize each screen via the custom data of the screen you want to configure.");

            if (inventoryBlocks.Count == 0 || cargoDisplays.Count == 0)
                return;

            List<Item> items = new List<Item>();

            #region Count stored items
            for (int i = 0; i < inventoryBlocks.Count; i++)
            {
                int inventories = inventoryBlocks[i].InventoryCount;
                for (int j = 0; j < inventories; j++)
                {
                    IMyInventory inventory = inventoryBlocks[i].GetInventory(j);
                    for (int k = 0; k < inventory.ItemCount; k++)
                    {
                        MyInventoryItem itemData = inventory.GetItemAt(k).Value;
                        if (itemData != null)
                        {
                            Item existing = items.FirstOrDefault(x => x.Matches(itemData));
                            if (existing != null)
                            {
                                existing.AddQuantity(itemData.Amount.RawValue);
                            }
                            else
                            {
                                items.Add(new Item(itemData));
                            }
                        }
                    }
                }
            }
            #endregion

            #region Count required ores from assembler
            for (int i = 0; i < assemblerBlocks.Count; i++)
            {
                List<MyProductionItem> queue = new List<MyProductionItem>();
                assemblerBlocks[i].GetQueue(queue);
                for (int j = 0; j < queue.Count; j++)
                {
                    Item existing = items.FirstOrDefault(x => x.Matches(queue[j]));
                    if (existing != null)
                    {
                        existing.ProducingValue += queue[j].Amount.RawValue;
                    }
                    else
                    {
                        items.Add(new Item(queue[j]));
                    }
                }
            }
            #endregion

            for (int i = 0; i < cargoDisplays.Count; i++)
            {
                switch (sortMode)
                {
                    case SortMode.Amount:
                        cargoDisplays[i].Refresh((isDescending ? items.OrderByDescending(x => x.AmountValue) : items.OrderBy(x => x.AmountValue)).ThenBy(x => x.Name));
                        break;
                    case SortMode.Name:
                        cargoDisplays[i].Refresh(isDescending ? items.OrderByDescending(x => x.Name) : items.OrderBy(x => x.Name));
                        break;
                }
            }
        }

void Init()
        {
            if (string.IsNullOrWhiteSpace(Me.CustomData))
            {
                Me.CustomData = "// Name of the group for your LCD screens.\n" +
                    "//    Each LCD screen can be configured to show all or specific item\n" +
                    "//    categories via custom data in respective screen.\n" +
                    "DisplayGroup=Cargo Status\n" +
                    "// Font size for screens. Can also be set per screen.\n" + 
                    "FontSize=1\n" +
                    "// Font color for screens. First value is red, second is green,\n" +
                    "//    third is blue. Value range: 0 - 255. Can also be set per screen.\n" +
                    "FontColor=0,178,255\n" +
                    "// Background color for screens. First value is red, second is green,\n" +
                    "//    third is blue. Value range: 0 - 255. Can also be set per screen.\n" +
                    "BackColor=0,0,0\n" +
                    "// Text padding in screen from top. Can also be set per screen.\n" +
                    "TextPadding=2\n" +
                    "// Configures sorting mode for cargo.\n" +
                    "//    Possible sorting modes: Name, Amount\n" +
                    "SortBy=Name\n" +
                    "// When this value is true, values are sorted in descending order.\n" +
                    "IsDescending=false\n" +
                    "// When true, also counts items from connected grids\n" +
                    "ConnectedGrids=false";
            }
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            int fontR = 0;
            int fontG = 178;
            int fontB = 255;

            int backR = 0;
            int backG = 0;
            int backB = 0;
            string displayGroupName = "Cargo Status";
            float displayFontSize = 1;
            float displayTextPadding = 2;

            string[] properties = Me.CustomData.Split('\n');
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].StartsWith("//"))
                {
                    continue;
                }

                string[] propertyData = properties[i].Split('=');
                switch (propertyData[0])
                {
                    case "DisplayGroup":
                        displayGroupName = propertyData[1];
                        break;
                    case "FontSize":
                        displayFontSize = float.Parse(propertyData[1]);
                        break;
                    case "TextPadding":
                        displayTextPadding = float.Parse(propertyData[1]);
                        break;
                    case "SortBy":
                        switch (propertyData[1])
                        {
                            case "Name":
                                sortMode = SortMode.Name;
                                break;
                            case "Amount":
                                sortMode = SortMode.Amount;
                                break;
                        }
                        break;
                    case "IsDescending":
                        isDescending = bool.Parse(propertyData[1]);
                        break;
                    case "ConnectedGrids":
                        connectedGrids = bool.Parse(propertyData[1]);
                        break;
                    case "FontColor":
                        string[] fontRgb = propertyData[1].Split(',');
                        if (fontRgb.Length >= 3)
                        {
                            fontR = int.Parse(fontRgb[0]);
                            fontG = int.Parse(fontRgb[1]);
                            fontB = int.Parse(fontRgb[2]);
                        }
                        break;
                    case "BackColor":
                        string[] backRgb = propertyData[1].Split(',');
                        if (backRgb.Length >= 3)
                        {
                            backR = int.Parse(backRgb[0]);
                            backG = int.Parse(backRgb[1]);
                            backB = int.Parse(backRgb[2]);
                        }
                        break;
                }
            }

            if (!connectedGrids)
            {
                GridTerminalSystem.GetBlocksOfType(inventoryBlocks, x => x.CubeGrid == Me.CubeGrid);
                GridTerminalSystem.GetBlocksOfType(assemblerBlocks, x => x.CubeGrid == Me.CubeGrid);
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType(inventoryBlocks);
                GridTerminalSystem.GetBlocksOfType(assemblerBlocks);
            }
            inventoryBlocks = inventoryBlocks.Where(x => x.HasInventory).ToList();

            if (displayGroupName == null)
                return;

            var displayGroup = GridTerminalSystem.GetBlockGroupWithName(displayGroupName);

            if (displayGroup == null)
                return;

            List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
            displayGroup.GetBlocksOfType(textPanels, x => x.CubeGrid == Me.CubeGrid);

            for (int i = 0; i < textPanels.Count; i++)
            {
                bool fontColorOverridden = false;
                bool backColorOverridden = false;
                bool paddingOverridden = false;
                bool textSizeOverridden = false;
                string screenGroups = "";

                textPanels[i].ContentType = ContentType.TEXT_AND_IMAGE;
                textPanels[i].Alignment = TextAlignment.LEFT;

                if (!string.IsNullOrWhiteSpace(textPanels[i].CustomData))
                {
                    string[] screenProperties = textPanels[i].CustomData.Split('\n');
                    for (int j = 0; j < screenProperties.Length; j++)
                    {
                        if (screenProperties[j].StartsWith("//"))
                        {
                            continue;
                        }

                        string[] screenProperty = screenProperties[j].Split('=');
                        switch (screenProperty[0])
                        {
                            case "Filter":
                                screenGroups = screenProperty[1];
                                break;
                            case "FontColor":
                                string[] fontRgb = screenProperty[1].Split(',');
                                if (fontRgb.Length >= 3)
                                {
                                    textPanels[i].FontColor = new Color(int.Parse(fontRgb[0]), int.Parse(fontRgb[1]), int.Parse(fontRgb[2]));
                                    fontColorOverridden = true;
                                }
                                break;
                            case "BackColor":
                                string[] backRgb = screenProperty[1].Split(',');
                                if (backRgb.Length >= 3)
                                {
                                    textPanels[i].BackgroundColor = new Color(int.Parse(backRgb[0]), int.Parse(backRgb[1]), int.Parse(backRgb[2]));
                                    backColorOverridden = true;
                                }
                                break;
                            case "FontSize":
                                textPanels[i].FontSize = float.Parse(screenProperty[1]);
                                textSizeOverridden = true;
                                break;
                            case "TextPadding":
                                textPanels[i].TextPadding = float.Parse(screenProperty[1]);
                                paddingOverridden = true;
                                break;
                        }
                    }
                }
                else
                {
                    textPanels[i].CustomData =
                    "// Possible screen filters: Ore, Ingot, Component, Ammo, Tools\n" +
                    "//    To show multiple categories, split each filter with a comma.\n" +
                    "//    Leave empty to show all categories.\n" +
                    "Filter=\n" + 
                    "// Font size for this screen. Uncomment to override\n" +
                    "// FontSize=1\n" +
                    "// Font color for this screen. First value is red, second is green,\n" +
                    "//    third is blue. Value range: 0 - 255. Uncomment to override\n" +
                    "// FontColor=0,178,255\n" +
                    "// Background color this screen. First value is red, second is green,\n" +
                    "//    third is blue. Value range: 0 - 255. Uncomment to override\n" +
                    "// BackColor=0,0,0\n" +
                    "// Text padding in screen from top. Uncomment to override\n" +
                    "// TextPadding=2";
                }

                if (!textSizeOverridden)
                    textPanels[i].FontSize = displayFontSize;

                if (!paddingOverridden)
                    textPanels[i].TextPadding = displayTextPadding;

                if (!fontColorOverridden)
                    textPanels[i].FontColor = new Color(fontR, fontG, fontB);

                if (!backColorOverridden)
                    textPanels[i].BackgroundColor = new Color(backR, backG, backB);
                textPanels[i].WriteText("Initializing...");

                string[] groups = screenGroups.ToLower().Split(',');
                List<ItemCategory> categories = new List<ItemCategory>();
                for (int k = 0; k < groups.Length; k++)
                {
                    switch (groups[k].Trim())
                    {
                        case "ammo":
                            categories.Add(ItemCategory.Ammo);
                            break;
                        case "component":
                            categories.Add(ItemCategory.Component);
                            break;
                        case "ingot":
                            categories.Add(ItemCategory.Ingot);
                            break;
                        case "ore":
                            categories.Add(ItemCategory.Ore);
                            break;
                        case "tools":
                            categories.Add(ItemCategory.Tool);
                            break;
                        default:
                            categories.Add(ItemCategory.All);
                            break;
                    }
                }

                cargoDisplays.Add(new Display(textPanels[i], categories));
            }
        }

class Item
{
    float amount;
    string name;
    ItemCategory category;
    string subTypeId;
    string typeId;

    public string Amount
            {
                get
                {
                    return ConvertRawValue(amount);
                }
            }

    public string Producing
            {
                get
                {
                    if (ProducingValue > 0)
                    {
                        return string.Format("     (x{0} in production)", ConvertRawValue(ProducingValue));
                    }
                    else
                    {
                        return null;
                    }
                }
            }

    public float ProducingValue { get; set; }

    public float AmountValue => amount;

    public string Name => name;

    public ItemCategory Category => category;

    public Item(MyInventoryItem item)
    {
        amount = item.Amount.RawValue;
        subTypeId = item.Type.SubtypeId;
        typeId = item.Type.TypeId;

        var itemInfo = item.Type.GetItemInfo();
        if (itemInfo.IsAmmo)
        {
            category = ItemCategory.Ammo;
        }
        else if (itemInfo.IsComponent)
        {
            category = ItemCategory.Component;
        }
        else if (itemInfo.IsIngot)
        {
            category = ItemCategory.Ingot;
        }
        else if (itemInfo.IsOre)
        {
            category = ItemCategory.Ore;
        }
        else
        {
            category = ItemCategory.Tool;
        }

        SetName();
    }

    public Item(MyProductionItem item)
            {
                subTypeId = item.BlueprintId.SubtypeId.ToString();
                ProducingValue = item.Amount.RawValue;

                switch (subTypeId)
                {
                    #region Components
                    case "Construction":
                    case "SmallTube":
                    case "LargeTube":
                    case "Detector":
                    case "Medical":
                    case "RadioCommunication":
                    case "Reactor":
                    case "Thrust":
                    case "GravityGenerator":
                    case "BulletproofGlass":
                    case "Computer":
                    case "Display":
                    case "Explosives":
                    case "Girder":
                    case "InteriorPlate":
                    case "MetalGrid":
                    case "Motor":
                    case "PowerCell":
                    case "SolarCell":
                    case "SteelPlate":
                    case "Superconductor":
                        category = ItemCategory.Component;
                        break;
                    #endregion
                    #region Tools
                    case "HandDrill":
                    case "HandDrill2":
                    case "HandDrill3":
                    case "HandDrill4":
                    case "Welder":
                    case "Welder2":
                    case "Welder3":
                    case "Welder4":
                    case "AngleGrinder":
                    case "AngleGrinder2":
                    case "AngleGrinder3":
                    case "AngleGrinder4":
                    case "BasicHandHeldLauncher":
                    case "AdvancedHandHeldLauncher":
                    case "FullAutoPistol":
                    case "SemiAutoPistol":
                    case "AutomaticRifle":
                    case "PreciseAutomaticRifle":
                    case "UltimateAutomaticRifle":
                    case "RapidFireAutomaticRifle":
                        subTypeId += "Item";
                        category = ItemCategory.Tool;
                        break;
                    case "EliteAutoPistol":
                        subTypeId = "ElitePistolItem";
                        category = ItemCategory.Tool;
                        break;
                    case "Datapad":
                    case "HydrogenBottle":
                    case "OxygenBottle":
                        category = ItemCategory.Tool;
                        break;
                    #endregion
                    #region Consumables
                    case "NATO_25x184mmMagazine":
                        subTypeId = "NATO_25x184mm";
                        category = ItemCategory.Ammo;
                        break;
                    case "ElitePistolMagazine":
                    case "FullAutoPistolMagazine":
                    case "MediumCalibreAmmo":
                    case "AutocannonClip":
                    case "AutomaticRifleGun_Mag_20rd":
                    case "PreciseAutomaticRifleGun_Mag_5rd":
                    case "RapidFireAutomaticRifleGun_Mag_50rd":
                    case "SemiAutoPistolMagazine":
                    case "UltimateAutomaticRifleGun_Mag_30rd":
                    case "Missile200mm":
                    case "SmallRailgunAmmo":
                    case "LargeRailgunAmmo":
                    case "LargeCalibreAmmo":
                    case "Canvas":
                        category = ItemCategory.Ammo;
                        break;
                        #endregion
                }

                SetName();
            }

    public void AddQuantity(long amount)
            {
                this.amount += amount;
            }

    public bool Matches(MyInventoryItem item)
    {
        return typeId != null ? typeId == item.Type.TypeId && subTypeId == item.Type.SubtypeId : subTypeId == item.Type.SubtypeId;
    }

    public bool Matches(MyProductionItem item)
            {
                return subTypeId == item.BlueprintId.SubtypeId.ToString();
            }

    string ConvertRawValue(float rawValue)
            {
                float kgAmount = Util.RawToKg(rawValue);
                if (category != ItemCategory.Ingot && category != ItemCategory.Ore)
                {
                    if (kgAmount > 0)
                    {
                        kgAmount = (float)Math.Round(kgAmount);
                        return kgAmount.ToString("#,##");
                    }
                    else
                    {
                        return "0";
                    }
                }
                else
                {
                    if (kgAmount > 0)
                    {
                        return kgAmount.ToString("#,##0.00") + " kg";
                    }
                    else
                    {
                        return "0 kg";
                    }
                }
            }

    void SetName()
            {
                switch (subTypeId)
                {
                    case "Stone":
                        if (category == ItemCategory.Ingot)
                        {
                            name = "Gravel";
                        }
                        else
                        {
                            name = "Stone";
                        }
                        break;
                    #region Components
                    case "Construction":
                        name = "Construction Comp.";
                        break;
                    case "SmallTube":
                        name = "Small Steel Tube";
                        break;
                    case "LargeTube":
                        name = "Large Steel Tube";
                        break;
                    case "Detector":
                        name = "Detector Comp.";
                        break;
                    case "Medical":
                        name = "Medical Comp.";
                        break;
                    case "RadioCommunication":
                        name = "Radio-comm Comp.";
                        break;
                    case "Reactor":
                        name = "Reactor Comp.";
                        break;
                    case "Thrust":
                        name = "Thruster Comp.";
                        break;
                    case "GravityGenerator":
                        name = "Gravity Comp.";
                        break;
                    #endregion
                    #region Tools
                    case "HandDrillItem":
                        name = "Hand Drill";
                        break;
                    case "HandDrill2Item":
                        name = "Enhanced Hand Drill";
                        break;
                    case "HandDrill3Item":
                        name = "Proficient Hand Drill";
                        break;
                    case "HandDrill4Item":
                        name = "Elite Hand Drill";
                        break;
                    case "WelderItem":
                        name = "Welder";
                        break;
                    case "Welder2Item":
                        name = "Enhanced Welder";
                        break;
                    case "Welder3Item":
                        name = "Proficient Welder";
                        break;
                    case "Welder4Item":
                        name = "Elite Welder";
                        break;
                    case "AngleGrinderItem":
                        name = "Grinder";
                        break;
                    case "AngleGrinder2Item":
                        name = "Enhanced Grinder";
                        break;
                    case "AngleGrinder3Item":
                        name = "Proficient Grinder";
                        break;
                    case "AngleGrinder4Item":
                        name = "Elite Grinder";
                        break;
                    case "BasicHandHeldLauncherItem":
                        name = "RO-1 (Launcher)";
                        break;
                    case "AdvancedHandHeldLauncherItem":
                        name = "PRO-1 (Launcher)";
                        break;
                    case "ElitePistolItem":
                        name = "S-10E (Pistol)";
                        break;
                    case "FullAutoPistolItem":
                        name = "S-20A (Pistol)";
                        break;
                    case "SemiAutoPistolItem":
                        name = "S-10 (Pistol)";
                        break;
                    case "AutomaticRifleItem":
                        name = "MR-20 (Rifle)";
                        break;
                    case "PreciseAutomaticRifleItem":
                        name = "MR-8P (Rifle)";
                        break;
                    case "UltimateAutomaticRifleItem":
                        name = "MR-30E (Rifle)";
                        break;
                    case "RapidFireAutomaticRifleItem":
                        name = "MR-50A (Rifle)";
                        break;
                    #endregion
                    #region Consumables
                    case "NATO_25x184mm":
                        name = "Gatling Ammo Box";
                        break;
                    case "ElitePistolMagazine":
                        name = "S-10E Magazine";
                        break;
                    case "FullAutoPistolMagazine":
                        name = "S-20A Magazine";
                        break;
                    case "MediumCalibreAmmo":
                        name = "Assault Cannon Shell";
                        break;
                    case "AutocannonClip":
                        name = "Autocannon Magazine";
                        break;
                    case "AutomaticRifleGun_Mag_20rd":
                        name = "MR-20 Magazine";
                        break;
                    case "PreciseAutomaticRifleGun_Mag_5rd":
                        name = "MR-8P Magazine";
                        break;
                    case "RapidFireAutomaticRifleGun_Mag_50rd":
                        name = "MR-50A Magazine";
                        break;
                    case "SemiAutoPistolMagazine":
                        name = "S-10 Magazine";
                        break;
                    case "UltimateAutomaticRifleGun_Mag_30rd":
                        name = "MR-30E Magazine";
                        break;
                    case "Missile200mm":
                        name = "Missile";
                        break;
                    case "SmallRailgunAmmo":
                        name = "Small Railgun Sabot";
                        break;
                    case "LargeRailgunAmmo":
                        name = "Large Railgun Sabot";
                        break;
                    case "LargeCalibreAmmo":
                        name = "Artillery Shell";
                        break;
                    #endregion
                    default:
                        name = Util.SplitByUpperCase(subTypeId);
                        break;
                }
            }
}

class Display
{
    float fontHeight = 28.4F;
    List<ItemCategory> categories;

    int maxLines = 17; // For 1 block, without padding, font size 1
    float fontSize = 1;
    float padding = 2;
    IMyTextPanel displayPanel;
    int lineOffset;
    float availableScreenHeight;

    public List<ItemCategory> Categories => categories;

    public Display(IMyTextPanel displayPanel, List<ItemCategory> categories)
            {
                this.displayPanel = displayPanel;
                if (!categories.Contains(ItemCategory.All))
                {
                    this.categories = categories;
                }
                else
                {
                    this.categories = new List<ItemCategory>() 
                    { 
                        ItemCategory.Ore,
                        ItemCategory.Ingot,
                        ItemCategory.Component,
                        ItemCategory.Ammo,
                        ItemCategory.Tool
                    };
                }
                fontSize = displayPanel.FontSize;
                padding = displayPanel.TextPadding;
                availableScreenHeight = displayPanel.SurfaceSize.Y - (displayPanel.SurfaceSize.Y * padding / 100);

                maxLines = (int)Math.Floor(availableScreenHeight / (fontHeight * fontSize));
            }

    public void Refresh(IEnumerable<Item> items)
            {
                List<string> lines = new List<string>();
                for (int i = 0; i < categories.Count; i++)
                {
                    List<Item> filteredItems = items.Where(x => x.Category == categories[i]).ToList();

                    lines.Add(Util.CategoryToString(categories[i]));
                    if (filteredItems.Count > 0)
                    {
                        for (int j = 0; j < filteredItems.Count; j++)
                        {
                            lines.Add(string.Format("  - {0} x{1}", filteredItems[j].Name, filteredItems[j].Amount));
                            if (filteredItems[j].Producing != null)
                            {
                                lines.Add(filteredItems[j].Producing);
                            }
                        }
                    }
                    else
                    {
                        lines.Add("  None");
                    }
                    lines.Add("");
                }

                WriteText(lines);
            }

    void WriteText(List<string> lines)
            {
                if (lines.Count > maxLines)
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i <= maxLines; i++)
                    {
                        builder.Append(lines[i + lineOffset] + "\n");

                        if (lines.Count == i + lineOffset + 1)
                        {
                            break;
                        }
                    }

                    displayPanel.WriteText(builder.ToString());

                    if (lineOffset < lines.Count - maxLines)
                    {
                        lineOffset++;
                    }
                    else
                    {
                        lineOffset = 0;
                    }
                }
                else
                {
                    displayPanel.WriteText(string.Join("\n", lines));
                }
            }
}

enum ItemCategory
{
    All,
    Ammo,
    Component,
    Ingot,
    Ore,
    Tool
}

enum SortMode
{
    Name,
    Amount
}

static class Util
{
    public static string CategoryToString(ItemCategory category)
            {
                switch (category)
                {
                    case ItemCategory.Ammo:
                        return "Ammo:";
                    case ItemCategory.Component:
                        return "Components:";
                    case ItemCategory.Ingot:
                        return "Ingots:";
                    case ItemCategory.Ore:
                        return "Ores:";
                    case ItemCategory.Tool:
                        return "Tools:";
                    default:
                        return "";
                }
            }

    public static float RawToKg(float rawAmount)
            {
                return (rawAmount / 1000000);
            }

    public static string SplitByUpperCase(string input)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < input.Length; i++)
                {
                    if (char.IsUpper(input[i]) && builder.Length > 0)
                    {
                        builder.Append(' ');
                    }
                    builder.Append(input[i]);
                }

                return builder.ToString();
            }
}