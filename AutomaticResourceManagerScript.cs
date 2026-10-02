/*
 * R e a d m e
 * -----------
 * 
 * This script automatically sorts resources in the specified cargo containers, 
 * removes end products from assembles and refineries by putting them into those containers, 
 * and automattically puts ores, based on the amount of available storage for ingots, into refineries for refining
 * 
 * Instructions to use!
 * 	Create 3 groups (for blocks you want the script to use):
 * 		STORAGE - All Cargo Containers 
 * 		REFINERIES - All Refineries 
 * 		ASSEMBLERS - All Assemblers (you want to dump. DOES NOT input recipes)
 * 
 * 	Add Custom Data to Cargo Containers in STORAGE
 * 	- Every resource you want to add has one entry.
 * 	- Format is: ResourceName(:PercentOfContainer)
 * 		- Add a space after every entry (even after a new line)
 * 		- Capitalization doesn't matter
 * 	- ResourceName is required. PercentOfContainer is optional
 * 		- ALL numbers in a container can not equal more than 100
 * 
 * 	* For Ore and Ingots:
 * 		either name or periodic table name followed by ore or ingot
 * 	EXAMPLES: ironore ironingot feore FEINGOT silverore:50
 * 
 * 	*For Components:
 * 		the name of the component
 * 	EXAMPLES: steelplate construction constructionComponent(s) steelplates:1 gravity gravitycomponent(s):99
 * 			
 * 
 * 	Important Notes: 
 * 		- PLEASE turn OFF "Use Conveyor System" in every Refinery, otherwise they will suck random resources, and my script won't balance them.
 * 		- Script will repeat every so often
 * 		- Refineries will only add new ore when super low on current ore refining
 */



public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Save()
{

}

public void Main(string argument, UpdateType updateSource)
{

    ResourceManager resourceManager = new ResourceManager(GridTerminalSystem);
    resourceManager.runResourceManager();
    string echoStr = resourceManager.getEchoStr();
    if (echoStr.Length != 0)
    {
        Echo(echoStr);
    }
}

public class Assembler
{
    private IMyAssembler assembler;
    string echoStr;
    private CustomDataScanner scannerPtr;


    public Assembler(IMyAssembler assembler, CustomDataScanner scannerPtr)
    {
        this.assembler = assembler;
        this.scannerPtr = scannerPtr;
        echoStr = "";
    }

    public string getEchoStr()
    {
        string rturnStr = echoStr;
        echoStr = "";
        return rturnStr;
    }

    public Dictionary<string, Stack<InventoryResourceItem>> getDumpResourcesDictionary()
    {
        Dictionary<string, Stack<InventoryResourceItem>> dumpResources = new Dictionary<string, Stack<InventoryResourceItem>>();

        List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();
        IMyInventory outputInventory = assembler.OutputInventory;
        outputInventory.GetItems(inventoryItems);

        foreach (MyInventoryItem inventoryItem in inventoryItems)
        {
            string itemTypeStr = inventoryItem.Type.ToString();
            if (itemTypeStr.Contains("Component"))
            {
                double itemAmount = ((double)inventoryItem.Amount);
                scannerPtr.addToResourceDictionary(dumpResources, itemTypeStr, outputInventory, itemAmount, inventoryItem);
            }
        }

        return dumpResources;
    }

}


public class CargoContainer
{
    private IMyCargoContainer cargoContainer;
    private CustomDataScanner scannerPtr;
    string echoStr;
    private Dictionary<string, Stack<InventoryResourceItem>> requestResources = new Dictionary<string, Stack<InventoryResourceItem>>();
    private Dictionary<string, Stack<InventoryResourceItem>> haveResources = new Dictionary<string, Stack<InventoryResourceItem>>();
    private Dictionary<string, Stack<InventoryResourceItem>> dumpResources = new Dictionary<string, Stack<InventoryResourceItem>>();
    private double containerVolume;
    private const double NO_REQUEST_KG_THRESHOLD = 1.0;


    public CargoContainer(IMyCargoContainer cargoContainer, CustomDataScanner scannerPtr)
    {
        this.cargoContainer = cargoContainer;
        this.scannerPtr = scannerPtr;
        containerVolume = ((double)cargoContainer.GetInventory().MaxVolume) * 1000.0;
        echoStr = "";
    }

    public string getEchoStr()
    {
        string rturnStr = echoStr;
        echoStr = "";
        return rturnStr;
    }

    public void checkContents()
    {

        Dictionary<string, int> customDataMap = scannerPtr.scanCustomData(cargoContainer.CustomData, cargoContainer.CustomName);
        echoStr += scannerPtr.getEchoStr();

        List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();
        IMyInventory cargoInventory = cargoContainer.GetInventory();
        cargoInventory.GetItems(inventoryItems);


        if(inventoryItems.Count == 0)
        {
            if(customDataMap.Count == 0) {
                return;
            } else
            {
                foreach (string resourceStr in customDataMap.Keys)
                {
                    double requestResourceAmount = calculateAmountOfMaxResourceInContainer(resourceStr, customDataMap[resourceStr]);
                    scannerPtr.addToResourceDictionary(requestResources, resourceStr, cargoInventory, requestResourceAmount, new MyInventoryItem());
                }
            }
        } else
        {
            foreach (MyInventoryItem inventoryItem in inventoryItems)
            {
                string itemTypeStr = inventoryItem.Type.ToString();
                double itemAmountInCargo = (double)inventoryItem.Amount;
                if (customDataMap.ContainsKey(itemTypeStr))
                {
                    double maxAllowedItems = calculateAmountOfMaxResourceInContainer(itemTypeStr, customDataMap[inventoryItem.Type.ToString()]);
                    if (itemAmountInCargo > maxAllowedItems)
                    {
                        scannerPtr.addToResourceDictionary(haveResources, itemTypeStr, cargoInventory, maxAllowedItems, inventoryItem);
                        double dumpResourcesAmount = itemAmountInCargo - maxAllowedItems;
                        scannerPtr.addToResourceDictionary(dumpResources, itemTypeStr, cargoInventory, maxAllowedItems, inventoryItem);
                    }
                    else if (itemAmountInCargo == maxAllowedItems)
                    {
                        scannerPtr.addToResourceDictionary(haveResources, itemTypeStr, cargoInventory, itemAmountInCargo, inventoryItem);
                    }
                    else
                    {
                        scannerPtr.addToResourceDictionary(haveResources, itemTypeStr, cargoInventory, maxAllowedItems, inventoryItem);
                        double requestResourcesAmount = maxAllowedItems - itemAmountInCargo;
                        if(requestResourcesAmount >= NO_REQUEST_KG_THRESHOLD)
                        {
                            scannerPtr.addToResourceDictionary(requestResources, itemTypeStr, cargoInventory, requestResourcesAmount, new MyInventoryItem());
                        }
                    }
                }
                else if(itemTypeStr.Contains("Ore") || itemTypeStr.Contains("Ingot") || itemTypeStr.Contains("Component"))
                {
                    scannerPtr.addToResourceDictionary(dumpResources, itemTypeStr, cargoInventory, itemAmountInCargo, inventoryItem);
                } else
                {
                }
            }

            foreach (string itemTypeStr in customDataMap.Keys)
            {
                if (!haveResources.ContainsKey(itemTypeStr))
                {
                    double maxAllowedItems = calculateAmountOfMaxResourceInContainer(itemTypeStr, customDataMap[itemTypeStr]);
                    scannerPtr.addToResourceDictionary(requestResources, itemTypeStr, cargoInventory, maxAllowedItems, new MyInventoryItem());
                }
            }
        }
    }

    private double calculateAmountOfMaxResourceInContainer(string itemTypeStr, int percentOfContainerInt)
    {
        double percentOfContainer = (double)percentOfContainerInt / 100.0;
        double maxVolumeResource = ((double)containerVolume) * percentOfContainer;

        if (itemTypeStr.Contains("Component"))
        {
            int maxAllowedItemns = scannerPtr.getAmountOfComponentsFromLiters(itemTypeStr, maxVolumeResource);
            return (double)maxAllowedItemns + 0.0000000001;
        }
        else
        {
            double maxAllowedKg = scannerPtr.getKilogramsFromVolumeInLiters(itemTypeStr, maxVolumeResource);
            return maxAllowedKg;
        }
    }
    public Dictionary<string, Stack<InventoryResourceItem>> getRequestResources()
    {
        return requestResources;
    }

    public Dictionary<string, Stack<InventoryResourceItem>> getHaveResources()
    {
        return haveResources;
    }

    public Dictionary<string, Stack<InventoryResourceItem>> getDumpResources()
    {
        return dumpResources;
    }

}

public class CustomDataScanner
{

    private string echoStr;

    public CustomDataScanner() {
        echoStr = "";
    }

    public string getEchoStr()
    {
        string rturnStr = echoStr;
        echoStr = "";
        return rturnStr;
    }


    public Dictionary<string, int> scanCustomData(string customData, string blockName)
    {
        Dictionary<string, int> customDataMaping = new Dictionary<string, int>();

        string parseString = customData.ToLower();
        parseString += " ";
        string resourceWord = "";
        string codeResourceWord = "$%$";
        int codeResourceInt = -1;

        if (parseString.Length > 0)
        {
            while (parseString[0] == ' ' && parseString.Length > 1)
            {
                parseString = parseString.Substring(1);
            }
        }


        while (parseString.Length > 0)
        {
            short asciiShort;
            if (parseString != "")
            {
                asciiShort = (short)parseString[0];
            } else
            {
                break;
            }


            if (asciiShort == 32)
            {
                if(resourceWord == "")
                {
                    codeResourceWord = "$%$";
                }

                if (codeResourceWord == "")
                {
                    echoStr += "Error:_WordNotRecognizedFromObjectDictionary:_" + resourceWord + "_ ";
                }
                else if (codeResourceWord == "$%$")
                {
                }
                else
                {
                    customDataMaping.Add(codeResourceWord, codeResourceInt);
                }
                resourceWord = "";
                codeResourceInt = -1;
                codeResourceWord = "";
                parseString = parseString.Substring(1);
                continue;

            }
            else if ((asciiShort == 58) || (asciiShort == 10))
            {

                if(asciiShort == 58)
                {
                } else
                {
                }
                parseString = parseString.Substring(1);
                continue;

            }
            else if ((asciiShort >= 97) && (asciiShort <= 122))
            {

                int i = 0;
                while (((asciiShort >= 97) && (asciiShort <= 122)) || ((asciiShort >= 48) && (asciiShort <= 57)))
                {
                    resourceWord += parseString[i];
                    i++;
                    asciiShort = (short)parseString[i];
                }
                codeResourceWord = getDictionaryStr(resourceWord);
                parseString = parseString.Substring(i);
                continue;

            }
            else if ((asciiShort >= 48) && (asciiShort <= 57))
            {

                int i = 0;
                string intStr = "";
                while ((asciiShort >= 48) && (asciiShort <= 57))
                {
                    intStr += parseString[i];
                    i++;
                    asciiShort = (short)parseString[i];
                }
                try
                {
                    codeResourceInt = Int32.Parse(intStr);
                }
                catch (FormatException e)
                {
                    echoStr += "ERROR:_UnableToTurnStringIntoInteger ";
                }
                if ((codeResourceInt > 100) || (codeResourceInt <= 0))
                {
                    codeResourceInt = -1;
                }
                parseString = parseString.Substring(i);
                continue;

            }
            else
            {
                echoStr += "ERROR:_CharacterNotRecgonizedInCustomDataMapping:_" + parseString[0] + "_ ";
                codeResourceWord = "$%$";
            }
                parseString = parseString.Substring(1);
        }

        bool lessThanEqual100Percent = processMappingInts(customDataMaping);
        if (!lessThanEqual100Percent)
        {
            echoStr += "ERROR:_CargoContainer:_" + blockName + "_CustomDataAddsUpToMoreThan100 ";
            return new Dictionary<string, int>();
        }


        return customDataMaping;
    }

    private bool processMappingInts(Dictionary<string, int> customDataMaping)
    {
        int sum = 100;
        foreach(int resourcePercent in customDataMaping.Values)
        {
            if (resourcePercent > 0)
            {
                sum -= resourcePercent;
            }
        }

        if (sum < 0)
        {
            return false;
        }

        int unassignedValuesNum = 0;
        foreach (int unassignedInt in customDataMaping.Values)
        {
            if (unassignedInt < 0)
            {
                unassignedValuesNum++;
            }
        }

        if (unassignedValuesNum == 0)
        {
            return true;
        }

        int percentEach = (int)((double)sum / (double)unassignedValuesNum);
        List<string> keyList = new List<string>();
        foreach(string key in  customDataMaping.Keys)
        {
            keyList.Add(key);
        }
        int length = customDataMaping.Count;
        for (int i = 0; i < length; i++)
        {
            if (customDataMaping[keyList[i]] < 0)
            {
                customDataMaping[keyList[i]] = percentEach;
            }
        }

        return true;
    }

    private string getDictionaryStr(string userStr)
    {
        if ((!userStr.Contains("glass")) && (userStr != "explosives"))
        {
            if (userStr.EndsWith("s"))
            {
                userStr = userStr.Remove(userStr.Length - 1, 1);
            }
        }
        switch (userStr)
        {
            case "cobaltore":
            case "coore":
                return "MyObjectBuilder_Ore/Cobalt";
            case "goldore":
            case "auore":
                return "MyObjectBuilder_Ore/Gold";
            case "ice":
            case "h2o":
                return "MyObjectBuilder_Ore/Ice";
            case "ironore":
            case "feore":
                return "MyObjectBuilder_Ore/Iron";
            case "magnesiumore":
            case "mgore":
                return "MyObjectBuilder_Ore/Magnesium";
            case "nickelore":
            case "niore":
                return "MyObjectBuilder_Ore/Nickel";
            case "platinumore":
            case "ptore":
                return "MyObjectBuilder_Ore/Platinum";
            case "scrap":
            case "scrapore":
                return "MyObjectBuilder_Ore/Scrap";
            case "siliconore":
            case "siore":
                return "MyObjectBuilder_Ore/Silicon";
            case "silverore":
            case "agore":
                return "MyObjectBuilder_Ore/Silver";
            case "stone":
            case "stoneore":
                return "MyObjectBuilder_Ore/Stone";
            case "uraniumore":
            case "uore":
                return "MyObjectBuilder_Ore/Uranium";

            case "cobaltingot":
            case "coingot":
                return "MyObjectBuilder_Ingot/Cobalt";
            case "goldingot":
            case "auingot":
                return "MyObjectBuilder_Ingot/Gold";
            case "ironingot":
            case "feingot":
                return "MyObjectBuilder_Ingot/Iron";
            case "magnesiumingot":
            case "mgingot":
                return "MyObjectBuilder_Ingot/Magnesium";
            case "nickelingot":
            case "niingot":
                return "MyObjectBuilder_Ingot/Nickel";
            case "platinumingot":
            case "ptingot":
                return "MyObjectBuilder_Ingot/Platinum";
            case "siliconingot":
            case "siingot":
                return "MyObjectBuilder_Ingot/Silicon";
            case "silveringot":
            case "agingot":
                return "MyObjectBuilder_Ingot/Silver";
            case "stoneingot":
            case "gravel":
                return "MyObjectBuilder_Ingot/Stone";
            case "uraniumingot":
            case "uingot":
                return "MyObjectBuilder_Ingot/Uranium";

            case "steelplate":
                return "MyObjectBuilder_Component/SteelPlate";
            case "bulletproofglass":
            case "glass":
                return "MyObjectBuilder_Component/BulletproofGlass";
            case "computer":
                return "MyObjectBuilder_Component/Computer";
            case "construction":
            case "constructioncomponent":
                return "MyObjectBuilder_Component/Construction";
            case "detector":
            case "detectorcomponent":
                return "MyObjectBuilder_Component/Detector";
            case "display":
                return "MyObjectBuilder_Component/Display";
            case "explosives":
                return "MyObjectBuilder_Component/Explosives";
            case "girder":
                return "MyObjectBuilder_Component/Girder";
            case "gravitygenerator":
            case "gravity":
            case "gravitycomponent":
                return "MyObjectBuilder_Component/GravityGenerator";
            case "interiorplate":
            case "interior":
                return "MyObjectBuilder_Component/InteriorPlate";
            case "largetube":
            case "largesteeltube":
                return "MyObjectBuilder_Component/LargeTube";
            case "medical":
            case "medicalcomponents":
                return "MyObjectBuilder_Component/Medical";
            case "metalgrid":
            case "grid":
                return "MyObjectBuilder_Component/MetalGrid";
            case "motor":
                return "MyObjectBuilder_Component/Motor";
            case "powercell":
                return "MyObjectBuilder_Component/PowerCell";
            case "radio":
            case "radiocommunication":
            case "radiocomponent":
            case "radiocommunicationcomponent":
                return "MyObjectBuilder_Component/RadioCommunication";
            case "reactor":
                return "MyObjectBuilder_Component/Reactor";
            case "smalltube":
            case "smallsteeltube":
                return "MyObjectBuilder_Component/SmallTube";
            case "solarcell":
            case "solar":
                return "MyObjectBuilder_Component/SolarCell";
            case "super":
            case "superconductor":
                return "MyObjectBuilder_Component/Superconductor";
            case "thrust":
            case "thruster":
            case "thrustcomponent":
            case "thrustercomponent":
                return "MyObjectBuilder_Component/Thrust";

            default:
                return "";
        }
    }

    public double getKilogramsFromVolumeInLiters(string itemTypeStr, double volumeLiters)
    {
        double kilogramsPerLiter;
        switch (itemTypeStr)
        {
            case "MyObjectBuilder_Ore/Cobalt":
            case "MyObjectBuilder_Ore/Gold":
            case "MyObjectBuilder_Ore/Ice":
            case "MyObjectBuilder_Ore/Iron":
            case "MyObjectBuilder_Ore/Magnesium":
            case "MyObjectBuilder_Ore/Nickel":
            case "MyObjectBuilder_Ore/Platinum":
            case "MyObjectBuilder_Ore/Silicon":
            case "MyObjectBuilder_Ore/Silver":
            case "MyObjectBuilder_Ore/Stone":
            case "MyObjectBuilder_Ore/Uranium":
                kilogramsPerLiter = 2.702703;
                break;
            case "MyObjectBuilder_Ore/Scrap":
                kilogramsPerLiter = 4.0;
                break;

            case "MyObjectBuilder_Ingot/Cobalt":
                kilogramsPerLiter = 9.090909;
                break;
            case "MyObjectBuilder_Ingot/Gold":
                kilogramsPerLiter = 19.230769;
                break;
            case "MyObjectBuilder_Ingot/Iron":
                kilogramsPerLiter = 7.692308;
                break;
            case "MyObjectBuilder_Ingot/Magnesium":
                kilogramsPerLiter = 1.724138;
                break;
            case "MyObjectBuilder_Ingot/Nickel":
                kilogramsPerLiter = 9.090909;
                break;
            case "MyObjectBuilder_Ingot/Platinum":
                kilogramsPerLiter = 20.0;
                break;
            case "MyObjectBuilder_Ingot/Silicon":
                kilogramsPerLiter = 2.325581;
                break;
            case "MyObjectBuilder_Ingot/Silver":
                kilogramsPerLiter = 10.0;
                break;
            case "MyObjectBuilder_Ingot/Stone":
                kilogramsPerLiter = 2.702703;
                break;
            case "MyObjectBuilder_Ingot/Uranium":
                kilogramsPerLiter = 19.230769;
                break;

            default:
                kilogramsPerLiter = 10.0;
                break;
        }
        return kilogramsPerLiter * volumeLiters;
    }

    public int getAmountOfComponentsFromLiters(string itemTypeStr, double volumeLiters)
    {
        int litersPerItem;
        switch (itemTypeStr)
        {
            case "MyObjectBuilder_Component/SteelPlate":
                litersPerItem = 3;
                break;
            case "MyObjectBuilder_Component/BulletproofGlass":
                litersPerItem = 8;
                break;
            case "MyObjectBuilder_Component/Computer":
                litersPerItem = 1;
                break;
            case "MyObjectBuilder_Component/Construction":
                litersPerItem = 2;
                break;
            case "MyObjectBuilder_Component/Detector":
                litersPerItem = 6;
                break;
            case "MyObjectBuilder_Component/Display":
                litersPerItem = 6;
                break;
            case "MyObjectBuilder_Component/Explosives":
                litersPerItem = 2;
                break;
            case "MyObjectBuilder_Component/Girder":
                litersPerItem = 2;
                break;
            case "MyObjectBuilder_Component/GravityGenerator":
                litersPerItem = 200;
                break;
            case "MyObjectBuilder_Component/InteriorPlate":
                litersPerItem = 5;
                break;
            case "MyObjectBuilder_Component/LargeTube":
                litersPerItem = 38;
                break;
            case "MyObjectBuilder_Component/Medical":
                litersPerItem = 160;
                break;
            case "MyObjectBuilder_Component/MetalGrid":
                litersPerItem = 15;
                break;
            case "MyObjectBuilder_Component/Motor":
                litersPerItem = 8;
                break;
            case "MyObjectBuilder_Component/PowerCell":
                litersPerItem = 40;
                break;
            case "MyObjectBuilder_Component/RadioCommunication":
                litersPerItem = 70;
                break;
            case "MyObjectBuilder_Component/Reactor":
                litersPerItem = 8;
                break;
            case "MyObjectBuilder_Component/SmallTube":
                litersPerItem = 2;
                break;
            case "MyObjectBuilder_Component/SolarCell":
                litersPerItem = 12;
                break;
            case "MyObjectBuilder_Component/Superconductor":
                litersPerItem = 8;
                break;
            case "MyObjectBuilder_Component/Thrust":
                litersPerItem = 10;
                break;
            default:
                litersPerItem = 200;
                break;
        }
        return (int)volumeLiters / litersPerItem;
    }

    public void addToResourceDictionary(Dictionary<string, Stack<InventoryResourceItem>> myDictionary,
                                    string str,
                                    IMyInventory inventory,
                                    double myDouble,
                                    MyInventoryItem item)
    {
        InventoryResourceItem inventoryResource = new InventoryResourceItem(inventory, item, myDouble);
        if (myDictionary.ContainsKey(str))
        {
            myDictionary[str].Push(inventoryResource);
        }
        else
        {
            Stack<InventoryResourceItem> nwInventoryList = new Stack<InventoryResourceItem>();
            nwInventoryList.Push(inventoryResource);
            myDictionary.Add(str, nwInventoryList);
        }
    }

}

public class InventoryResourceItem
{
    private IMyInventory inventory;
    private MyInventoryItem item;
    private double amountOfItems;

    public InventoryResourceItem(IMyInventory inventory, MyInventoryItem item, double amountOfItems)
    {
        this.inventory = inventory;
        this.item = item;
        this.amountOfItems = amountOfItems;
    }

    public IMyInventory getInventory()
    {
        return inventory;
    }

    public MyInventoryItem getItem()
    {
        return item;
    }

    public double getAmountOfItems()
    {
        return amountOfItems;
    }

    public void setAmountOfItems(double amountOfItems)
    {
        this.amountOfItems = amountOfItems;
    }

}

public class Refinery
{

    private IMyRefinery refinery;
    string echoStr;
    private CustomDataScanner scannerPtr;
    private const double REFINERY_VOLUME_FILL_FACTOR_THRESHOLD = 0.001;

    public Refinery(IMyRefinery refinery, CustomDataScanner scannerPtr)
    {
        this.refinery = refinery;
        this.scannerPtr = scannerPtr;
        echoStr = "";
    }

    public string getEchoStr()
    {
        string rturnStr = echoStr;
        echoStr = "";
        return rturnStr;
    }

    public Dictionary<string, Stack<InventoryResourceItem>> getDumpResourcesDictionary()
    {
        Dictionary<string, Stack<InventoryResourceItem>> dumpResources = new Dictionary<string, Stack<InventoryResourceItem>> ();

        List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();
        IMyInventory outputInventory = refinery.OutputInventory;
        outputInventory.GetItems(inventoryItems);

        foreach (MyInventoryItem inventoryItem in inventoryItems)
        {
            string itemTypeStr = inventoryItem.Type.ToString();
            double itemAmount = ((double)inventoryItem.Amount);
            scannerPtr.addToResourceDictionary(dumpResources, itemTypeStr, outputInventory, itemAmount, inventoryItem);
        }

        return dumpResources;
    }

    public bool checkIfInputIsReadyForOre()
    {
        if (refinery.InputInventory.VolumeFillFactor < REFINERY_VOLUME_FILL_FACTOR_THRESHOLD)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public IMyInventory getRefineryInputInventory()
    {
        return refinery.InputInventory;
    }

}


public class ResourceManager
{

    private const int REFINE_ORE_AMOUNT_MULTIPLIER = 3;

    private IMyGridTerminalSystem GridTerminalSystem;
    private string echoStr;
    CustomDataScanner scanner = new CustomDataScanner();
    private Dictionary<string, Stack<InventoryResourceItem>> requestResources = new Dictionary<string, Stack<InventoryResourceItem>>();
    private Dictionary<string, Stack<InventoryResourceItem>> haveResources = new Dictionary<string, Stack<InventoryResourceItem>>();
    private Dictionary<string, Stack<InventoryResourceItem>> dumpResources = new Dictionary<string, Stack<InventoryResourceItem>>();
    private double REFINERY_INPUT_INVENTORY_FILL_SIZE = 7000.0;

    public ResourceManager(IMyGridTerminalSystem gridTerminalSystem)
    {
        this.GridTerminalSystem = gridTerminalSystem;
        echoStr = "";
    }

    public string getEchoStr()
    {
        string rturnStr = echoStr;
        echoStr = "";
        return rturnStr;
    }

    public void runResourceManager()
    {
        echoStr += "StartProgram ";

        IMyBlockGroup cargoGroup = GridTerminalSystem.GetBlockGroupWithName("STORAGE");
        List<IMyCargoContainer> cargoContainers = new List<IMyCargoContainer>();
        cargoGroup.GetBlocksOfType(cargoContainers);
        if (cargoContainers.Count > 0)
        {
            scanCargoContainers(cargoContainers, true);
        } else
        {
            echoStr += "NoCargoContainersDetected ";
        }


        IMyBlockGroup refineryGroup = GridTerminalSystem.GetBlockGroupWithName("REFINERIES");
        List<IMyRefinery> refineries = new List<IMyRefinery>();
        refineryGroup.GetBlocksOfType(refineries);
        if (refineries.Count > 0)
        {
            scanRefineriesOutput(refineries);
        }
        else
        {
            echoStr += "NoRefineriesDetected ";
        }

        IMyBlockGroup assemblerGroup = GridTerminalSystem.GetBlockGroupWithName("ASSEMBLERS");
        List<IMyAssembler> assemblers = new List<IMyAssembler>();
        assemblerGroup.GetBlocksOfType(assemblers);
        if (assemblers.Count > 0)
        {
            foreach (IMyAssembler assembler in assemblers)
            {
                Assembler tempAssemblerClass = new Assembler(assembler, scanner);
                Dictionary<string, Stack<InventoryResourceItem>> tempDictionaryClass = tempAssemblerClass.getDumpResourcesDictionary();
                echoStr += tempAssemblerClass.getEchoStr();
                addToResourcesDictionary(tempDictionaryClass, dumpResources);
            }
        }

        List<string> neededIngotsRefined = new List<string>();
        foreach (string resourceStr in requestResources.Keys)
        {
            if (dumpResources.ContainsKey(resourceStr))
            {
                while ((requestResources[resourceStr].Count > 0) && (dumpResources[resourceStr].Count > 0))
                {
                    moveResourcesFromToOnce(dumpResources[resourceStr], requestResources[resourceStr]);
                }
            }
            if (requestResources[resourceStr].Count > 0)
            {
                if (resourceStr.Contains("Ingot"))
                {
                    neededIngotsRefined.Add(resourceStr);
                }
            }

        }


        scanAndMoveOreToRefineries(refineries);


        echoStr += "EndProgram ";
    }

    private void scanCargoContainers(List<IMyCargoContainer> cargoContainers, bool isNormalStorage = true)
    {
        foreach (IMyCargoContainer cargo in cargoContainers)
        {
            CargoContainer cargoClass = new CargoContainer(cargo, scanner);
            cargoClass.checkContents();
            echoStr += cargoClass.getEchoStr();

            Dictionary<string, Stack<InventoryResourceItem>> tempDictionaryClass;
            if (isNormalStorage == true)
            {
                tempDictionaryClass = cargoClass.getHaveResources();
                addToResourcesDictionary(tempDictionaryClass, haveResources);
                tempDictionaryClass = cargoClass.getRequestResources();
                addToResourcesDictionary(tempDictionaryClass, requestResources);
                tempDictionaryClass = cargoClass.getDumpResources();
                addToResourcesDictionary(tempDictionaryClass, dumpResources);
            }
        }
    }

    private void scanRefineriesOutput(List<IMyRefinery> refineries)
    {
        foreach (IMyRefinery refinery in refineries)
        {
            Refinery refineryClass = new Refinery(refinery, scanner);
            Dictionary<string, Stack<InventoryResourceItem>> tempDictionaryClass = refineryClass.getDumpResourcesDictionary();
            echoStr += refineryClass.getEchoStr();
            addToResourcesDictionary(tempDictionaryClass, dumpResources);
        }
    }

    private void scanAndMoveOreToRefineries(List<IMyRefinery> refineries)
    {
        List<string> oresToRefine = new List<string>();
        List<string> tempIngotStrings = new List<string>();
        foreach(string requestIngots in requestResources.Keys)
        {
            if(requestIngots.Contains("Ingot"))
            {
                tempIngotStrings.Add(requestIngots);
            }
        }
        addOreStrsFromNeededIngotStrs(oresToRefine, tempIngotStrings);

        List<string> haveRefineOres = new List<string>();
        foreach (string oreStr in oresToRefine)
        {
            if (haveResources.ContainsKey(oreStr))
            {
                haveRefineOres.Add(oreStr);
            }
        }

        List<IMyRefinery> processRefineries = new List<IMyRefinery>();
        foreach (IMyRefinery refinery in refineries)
        {
            Refinery tempRefineryClass = new Refinery(refinery, scanner);
            if (tempRefineryClass.checkIfInputIsReadyForOre() == true)
            {
                processRefineries.Add(refinery);
            }
        }

        if ((processRefineries.Count > 0) && (haveRefineOres.Count > 0))
        {


            Dictionary<string, Stack<InventoryResourceItem>> refineryRequestResources = new Dictionary<string, Stack<InventoryResourceItem>>();


            foreach (IMyRefinery refinery in processRefineries)
            {
                foreach (string oreStr in haveRefineOres)
                {
                    double kgOfOre = getOneMinuteOfOreToRefine(oreStr) * REFINE_ORE_AMOUNT_MULTIPLIER;
                    InventoryResourceItem nwItem = new InventoryResourceItem(refinery.InputInventory, new MyInventoryItem(), kgOfOre);
                    scanner.addToResourceDictionary(refineryRequestResources, oreStr, refinery.InputInventory, kgOfOre, new MyInventoryItem());
                }
            }

            foreach (string resourceStr in haveRefineOres)
            {
                if (haveResources.ContainsKey(resourceStr))
                {
                    while ((refineryRequestResources[resourceStr].Count > 0) && (haveResources[resourceStr].Count > 0))
                    {
                        moveResourcesFromToOnce(haveResources[resourceStr], refineryRequestResources[resourceStr]);
                    }
                }
            }
        }
    }

    private void addToResourcesDictionary(Dictionary<string, Stack<InventoryResourceItem>> nwDictionaryResources,
                                          Dictionary<string, Stack<InventoryResourceItem>> managerDictionaryResources)
    {
        foreach(string key in nwDictionaryResources.Keys)
        {
            if(managerDictionaryResources.ContainsKey(key))
            {
                int count = nwDictionaryResources[key].Count;
                for (int i = 0; i < count; i++)
                {
                    managerDictionaryResources[key].Push(nwDictionaryResources[key].Pop());
                }
            }
            else
            {
                managerDictionaryResources.Add(key, nwDictionaryResources[key]);
            }
        }
    }

    private string addOreStrsFromNeededIngotStrs(List<string> oreStrs, List<string> ingotStrs)
    {
        string echoStr = "";
        foreach(string ingotStr in ingotStrs)
        {
            switch(ingotStr)
            {
                case "MyObjectBuilder_Ingot/Cobalt":
                    oreStrs.Add("MyObjectBuilder_Ore/Cobalt");
                    break;
                case "MyObjectBuilder_Ingot/Gold":
                    oreStrs.Add("MyObjectBuilder_Ore/Gold");
                    break;
                case "MyObjectBuilder_Ingot/Iron":
                    oreStrs.Add("MyObjectBuilder_Ore/Iron");
                    oreStrs.Add("MyObjectBuilder_Ore/Scrap");
                    oreStrs.Add("MyObjectBuilder_Ore/Stone");
                    break;
                case "MyObjectBuilder_Ingot/Magnesium":
                    oreStrs.Add("MyObjectBuilder_Ore/Magnesium");
                    break;
                case "MyObjectBuilder_Ingot/Nickel":
                    oreStrs.Add("MyObjectBuilder_Ore/Nickel");
                    oreStrs.Add("MyObjectBuilder_Ore/Stone");
                    break;
                case "MyObjectBuilder_Ingot/Platinum":
                    oreStrs.Add("MyObjectBuilder_Ore/Platinum");
                    break;
                case "MyObjectBuilder_Ingot/Silicon":
                    oreStrs.Add("MyObjectBuilder_Ore/Silicon");
                    oreStrs.Add("MyObjectBuilder_Ore/Stone");
                    break;
                case "MyObjectBuilder_Ingot/Silver":
                    oreStrs.Add("MyObjectBuilder_Ore/Silver");
                    break;
                case "MyObjectBuilder_Ingot/Stone":
                    oreStrs.Add("MyObjectBuilder_Ore/Stone");
                    break;
                case "MyObjectBuilder_Ingot/Uranium":
                    oreStrs.Add("MyObjectBuilder_Ore/Uranium");
                    break;

                default:
                    echoStr += "ERROR:_code_had_an_incorect_index_for_ingot_strings ";
                    break;
            }
        }
        return echoStr;
    }

    private double getOneMinuteOfOreToRefine(string oreString)
    {
        double kgOfOre;
        switch (oreString)
        {
            case "MyObjectBuilder_Ore/Cobalt":
                kgOfOre = 26;
                break;
            case "MyObjectBuilder_Ore/Gold":
                kgOfOre = 195;
                break;
            case "MyObjectBuilder_Ore/Iron":
                kgOfOre = 1560;
                break;
            case "MyObjectBuilder_Ore/Scrap":
                kgOfOre = 480;
                break;
            case "MyObjectBuilder_Ore/Magnesium":
                kgOfOre = 156;
                break;
            case "MyObjectBuilder_Ore/Nickel":
                kgOfOre = 118.2;
                break;
            case "MyObjectBuilder_Ore/Platinum":
                kgOfOre = 26;
                break;
            case "MyObjectBuilder_Ore/Silicon":
                kgOfOre = 130;
                break;
            case "MyObjectBuilder_Ore/Silver":
                kgOfOre = 78;
                break;
            case "MyObjectBuilder_Ore/Stone":
                kgOfOre = 1560;
                break;
            case "MyObjectBuilder_Ore/Uranium":
                kgOfOre = 19.5;
                break;

            default:
                kgOfOre = 0;
                echoStr += "ERROR:_OreStringHas:_" + oreString +"_HasNoKgValueAssociated ";
                break;
        }
        return kgOfOre;
    }

    private void moveResourcesFromToOnce(Stack<InventoryResourceItem> itemFromInventories, Stack<InventoryResourceItem> itemToInventories)
    {
        InventoryResourceItem itemTo = itemToInventories.Pop();
        InventoryResourceItem itemFrom = itemFromInventories.Pop();

        if (Math.Abs(itemTo.getAmountOfItems() - itemFrom.getAmountOfItems()) < 0.000001)
        {
            itemFrom.getInventory().TransferItemTo(itemTo.getInventory(), itemFrom.getItem(), itemFrom.getItem().Amount);
        }
        else if (itemTo.getAmountOfItems() < itemFrom.getAmountOfItems())
        {
            double requestItemAmount = itemTo.getAmountOfItems();
            double nwDumpAmount = itemFrom.getAmountOfItems() - requestItemAmount;

            itemFrom.getInventory().TransferItemTo(itemTo.getInventory(), itemFrom.getItem(), (MyFixedPoint)requestItemAmount);

            itemFrom.setAmountOfItems(nwDumpAmount);
            itemFromInventories.Push(itemFrom);
        }
        else
        {
            double dumpItemAmount = itemFrom.getAmountOfItems();
            double nwRequestAmount = itemTo.getAmountOfItems() - dumpItemAmount;

            itemFrom.getInventory().TransferItemTo(itemTo.getInventory(), itemFrom.getItem(), (MyFixedPoint)dumpItemAmount);

            itemTo.setAmountOfItems(nwRequestAmount);
            itemToInventories.Push(itemTo);
        }
    }


}