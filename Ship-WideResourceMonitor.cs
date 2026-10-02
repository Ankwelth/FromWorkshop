/*
    SHIP-WIDE RESOURCE DISPLAY SYSTEM
    Author: Steeveeo
    
    Gathers all storages and tallies their contents, which then
    displays on any number of screens.
    
    INSTRUCTIONS:
    1. Load onto a ship/station with storage blocks of any kind.
    2. Place a timer that calls this block periodically (I recommend 1 - 3 seconds).
    3. Place a timer that calls this block occasionally with argument "-r" (I recommend 1 minute).
        This will recollate all connected storages an screens.
    4. Place pairs of non-wide LCD screens around the ship.
    5. Name the left screen(s) "Resource Panel Left" along with any unique identifier.
    6. Do the above for the right screen(s) with the name "Resource Panel Right".
    7. Set both LCD Panels to Content: Text and Images.
    
    FEATURES:
    - To manually recollate connected storages and screens, simply run the script with argument "-r".
    - Any new screen named as per the instructions will display the proper data, as long as the script
        is occasionally run with "-r".
        
    CHANGELOG:
        1.0.9:
        - After 7 years of not touching this script, it has come time to finally fix it for modern Space Engineers.
          Note: There are much better screens out there, but if you just want a text summary of your resources, this
          should now work again!
    
    VERSION:
        1.0.9
        2026-2-14
*/

bool firstRun = true;

//Screen Settings
float textSize_Left = 0.50f;
Color textColor_Left = new Color(0, 128, 0, 255);
Color bgColor_Left = new Color(0, 0, 0, 255);

float textSize_Right = 0.50f;
Color textColor_Right = new Color(255, 255, 0, 255);
Color bgColor_Right = new Color(0, 0, 0, 255);

//String Conversions
Dictionary<string, string> niceNames = new Dictionary<string, string>();
public void InitNameDictionary()
{
    niceNames.Clear();
    
    niceNames.Add("Construction", "Construction Components");
    niceNames.Add("MetalGrid", "Metal Grids");
    niceNames.Add("InteriorPlate", "Interior Plates");
    niceNames.Add("SteelPlate", "Steel Plates");
    niceNames.Add("SmallTube", "Small Tubes");
    niceNames.Add("LargeTube", "Large Tubes");
    niceNames.Add("BulletproofGlass", "Bulletproof Glass");
    niceNames.Add("Reactor", "Reactor Components");
    niceNames.Add("Thrust", "Thruster Components");
    niceNames.Add("GravityGenerator", "Gravity Generator Components");
    niceNames.Add("Medical", "Medical Components");
    niceNames.Add("RadioCommunication", "Radio Communication Components");
    niceNames.Add("Detector", "Detector Components");
    niceNames.Add("SolarCell", "Solar Cells");
    niceNames.Add("PowerCell", "Power Cells");
    niceNames.Add("AutomaticRifleItem", "Automatic Rifles");
    niceNames.Add("AutomaticRocketLauncher", "Rocket Launchers");
    niceNames.Add("WelderItem", "Welders");
    niceNames.Add("AngleGrinderItem", "Grinders");
    niceNames.Add("HandDrillItem", "Hand Drills");
    niceNames.Add("NATO_25x184mm", "25x184mm NATO Ammo Containers");
    niceNames.Add("Missile200mm", "200mm Missiles");
    niceNames.Add("NATO_5p56x45mm", "5.56x45mm NATO Magazines");
    niceNames.Add("Superconductor", "Superconductors");
    niceNames.Add("OxygenBottle", "Oxygen Bottles");
    niceNames.Add("HydrogenBottle", "Hydrogen Bottles");
    niceNames.Add("Computer", "Computers");
    niceNames.Add("Motor", "Motors");
    niceNames.Add("Display", "Displays");
    niceNames.Add("Girder", "Girders");
}

//Screens
List<IMyTerminalBlock> lcdScreens_Left = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> lcdScreens_Right = new List<IMyTerminalBlock>();
private bool SearchMethod_Screens(IMyTerminalBlock block)
{
    IMyTextPanel panel = block as IMyTextPanel;
    return panel != null;
}

//Storages
List<IMyTerminalBlock> storages = new List<IMyTerminalBlock>();
Dictionary<string, float> resources = new Dictionary<string, float>();
Dictionary<string, float> components = new Dictionary<string, float>();
float usedStorageVolume = 0;
float maxStorageVolume = 0;
int numAssemblers = 0;
int numRefineries = 0;
int numArcFurnaces = 0;
int numReactors = 0;
int numOxyGens = 0;

//Applies screen settings to a list of screens
public void ApplyScreenSettings(List<IMyTerminalBlock> screens, bool isLeft)
{
    for(int i = 0; i < screens.Count; i++)
    {
        IMyTextPanel panel = (IMyTextPanel) screens[i];
        
        if(isLeft)
        {
            panel.SetValueFloat("FontSize", textSize_Left);
            panel.SetValue<Color>("FontColor", textColor_Left);
            panel.SetValue<Color>("BackgroundColor", bgColor_Left);
        }
        else
        {
            panel.SetValueFloat("FontSize", textSize_Right);
            panel.SetValue<Color>("FontColor", textColor_Right);
            panel.SetValue<Color>("BackgroundColor", bgColor_Right);
        }
    }
}

//Adds a string to a Dictionary and either increments existing value or writes a new one
public void UpdateItemEntry(Dictionary<string, float> dict, string key, float value)
{
    if(dict.ContainsKey(key))
    {
        dict[key] += value;
    }
    else
    {
        dict.Add(key, value);
    }
}

//Take an item, generate a display name, and bung it into the proper list
public void ParseItem(MyInventoryItem item)
{
    string resName = item.Type.SubtypeId.ToString();
    string resId = item.Type.TypeId.ToString();
    float amount = (float) item.Amount;
    
    //Convert Internal Name to User-Friendly String
    if(niceNames.ContainsKey(resName))
    {
        UpdateItemEntry(components, niceNames[resName], amount);
        return;
    }
    
    //Separate Ores from Processed Stuff
    string niceName = "";
    if(resId.EndsWith("_Ore"))
    {
        if(resName.Equals("Stone") || resName.Equals("Ice"))
        {
            niceName = resName;
        }
        else
        {
            niceName = resName + " Ore";
        }
        
        UpdateItemEntry(resources, niceName, amount);
        return;
    }
    else if(resId.EndsWith("_Ingot"))
    {
        if(resName.Equals("Stone"))
        {
            niceName = "Gravel";
        }
        else if(resName.Equals("Magnesium"))
        {
            niceName = "Magnesium Powder";
        }
        else if(resName.Equals("Silicon"))
        {
            niceName = "Silicon Wafer";
        }
        else
        {
            niceName = resName + " Ingot";
        }
        
        UpdateItemEntry(resources, niceName, amount);
        return;
    }
    else
    {
        UpdateItemEntry(components, resName, amount);
        return;
    }
}

//Compile resource list across all storages
public void UpdateStorage()
{
    //Reset
    resources.Clear();
    components.Clear();
    usedStorageVolume = 0;
    maxStorageVolume = 0;
    numAssemblers = 0;
    numRefineries = 0;
    numArcFurnaces = 0;
    numReactors = 0;
    numOxyGens = 0;

    //Loop through all inventories and append to dictionary
    for(int i = 0; i < storages.Count; i++)
    {
        IMyInventoryOwner storage = (IMyInventoryOwner) storages[i];
        
        //Check for tracked type
        if((storage as IMyAssembler) != null)
        {
            numAssemblers++;
        }
        else if((storage as IMyRefinery) != null)
        {
            //Arc Furnaces are also IMyRefinery with no class-name way of differentiating, using Name instead
            if((storage as IMyTerminalBlock).CustomName.Contains("furnace"))
            {
                numArcFurnaces++;
            }
            else
            {
                numRefineries++;
            }
        }
        else if((storage as IMyReactor) != null)
        {
            numReactors++;
        }
        else if((storage as IMyOxygenFarm) != null)
        {
            numOxyGens++;
        }
        
        for(int ii = 0; ii < storage.InventoryCount; ii++)
        {
            IMyInventory inventory = storage.GetInventory(ii);
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            inventory.GetItems(items, null);
            
            for(int iii = 0; iii < items.Count; iii++)
            {
                ParseItem(items[iii]);
            }
            
            //Increment Detected Storage
            usedStorageVolume += (float) inventory.CurrentVolume * 1000;    //Despite what the wiki says, this apparently returns Kiloliters instead of Liters, amazing...
            maxStorageVolume += (float) inventory.MaxVolume * 1000;
        }
    }
}
 
void Main(string argument)
{
    //Recompile Listings
    if(argument == "-r" || firstRun)
    {
        firstRun = false;
        
        //Initialize Dictionaries
        InitNameDictionary();
         
        //Get All Screens 
        GridTerminalSystem.SearchBlocksOfName("Resource Panel Left", lcdScreens_Left, SearchMethod_Screens);
        GridTerminalSystem.SearchBlocksOfName("Resource Panel Right", lcdScreens_Right, SearchMethod_Screens);
        
        ApplyScreenSettings(lcdScreens_Left, true);
        ApplyScreenSettings(lcdScreens_Right, false);
        
        //Find all storage blocks on the ship
        GridTerminalSystem.GetBlocksOfType<IMyInventoryOwner>(storages);
    }
    
    //Update Storage Amounts
    UpdateStorage();
    
    //Build Left Panel Display String
    StringBuilder leftPanelText = new StringBuilder();
    leftPanelText.Append("     - STORED COMPONENTS - \n");
    leftPanelText.Append("----------------------------------------------------------------------------------------------------------------------------------\n");
    
    //--Print Components in Storage
    List<string> keys = new List<string>(components.Keys);
    for(int ii = 0; ii < keys.Count; ii++)
    {
        string resourceName = keys[ii];
        float amount = components[keys[ii]];
        leftPanelText.Append("          " + resourceName + ": " + amount + "\n");
    }
    string leftPanelTextString = leftPanelText.ToString();
    
    //Build Right Panel Display String
    StringBuilder rightPanelText = new StringBuilder();
    rightPanelText.Append("     - RESOURCING SUMMARY -\n");
    rightPanelText.Append("----------------------------------------------------------------------------------------------------------------------------------\n");
    
    //--Print Device Counts
    rightPanelText.Append("Connected Devices:\n");
    rightPanelText.Append(" |    Reactors: " + numReactors + "\n");
    rightPanelText.Append(" |    Assemblers: " + numAssemblers + "\n");
    rightPanelText.Append(" |    Refineries: " + numRefineries + "\n");
    rightPanelText.Append(" |    Arc Furnaces: " + numArcFurnaces + "\n");
    rightPanelText.Append(" |    Oxygen Generators: " + numOxyGens + "\n");
    
    rightPanelText.Append("----------------------------------------------------------------------------------------------------------------------------------\n");
    
    //--Print Storage Status
    rightPanelText.Append("Storage Status:\n");
    rightPanelText.Append(" |    Number of Storage Blocks: " + storages.Count + "\n");
    rightPanelText.Append(" |    Storage Volume: " + usedStorageVolume + " L\n");
    rightPanelText.Append(" |    Storage Capacity: " + maxStorageVolume + " L\n");
    
    rightPanelText.Append("----------------------------------------------------------------------------------------------------------------------------------\n");
    
    //--Print Stored Ores/Ingots
    rightPanelText.Append("\nStored Resources:\n");
    keys = new List<string>(resources.Keys);
    for(int ii = 0; ii < keys.Count; ii++)
    {
        string resourceName = keys[ii];
        float amount = resources[keys[ii]];
        rightPanelText.Append("          " + resourceName + ": " + amount + "\n");
    }
    string rightPanelTextString = rightPanelText.ToString();
    
    
    //Update Left Screens
    for(int i = 0; i < lcdScreens_Left.Count; i++)
    {
        IMyTextPanel panel = (IMyTextPanel) lcdScreens_Left[i];
        panel.WritePublicTitle("Resource Panel Left", false);
        
        panel.WriteText(leftPanelTextString, false);
    }
    
    //Update Right Screens
    for(int i = 0; i < lcdScreens_Right.Count; i++)
    {
        IMyTextPanel panel = (IMyTextPanel) lcdScreens_Right[i];
        panel.WritePublicTitle("Resource Panel Right", false);
        
        panel.WriteText(rightPanelTextString, false);
    }
}