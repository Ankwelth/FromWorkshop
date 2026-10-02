/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

//List<IMyAssembler> assemblers = new List<IMyAssembler>();
//GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
//    foreach(IMyAssembler assembler in assemblers)
//    {
//        if (assembler.CustomName.Contains("[ASSIST]"))
//        {
//            assembler.CooperativeMode = true;
//        }
//        }


public int runNr = 1;

//Quota
public static MyFixedPoint bule = 0;      //BulletproofGlassQuota
public static MyFixedPoint canvas = 0;      //Canvas    NEW
public static MyFixedPoint comp = 0;        //ComputerQuota
public static MyFixedPoint cons = 0;      //ConstructionQuota
public static MyFixedPoint dete = 0;        //DetectorQuota
public static MyFixedPoint disp = 0;       //DisplayQuota
public static MyFixedPoint expl = 0;        //ExplosivesQuota
public static MyFixedPoint gird = 0;        //GirderQuota
public static MyFixedPoint grav = 0;        //GravityGeneratorQuota
public static MyFixedPoint inte = 0;       //InteriorPlateQuota
public static MyFixedPoint larg = 0;       //LargeTubeQuota
public static MyFixedPoint medi = 0;        //MedicalQuota
public static MyFixedPoint meta = 0;        //MetalGridQuota
public static MyFixedPoint moto = 0;      //MotorQuota
public static MyFixedPoint powe = 0;        //PowerCellQuota
public static MyFixedPoint radi = 0;        //RadioCommunicationQuota
public static MyFixedPoint reac = 0;        //Reactor   NEW
public static MyFixedPoint smal = 0;   //SmallTubeQuota
public static MyFixedPoint sola = 0;        //SolarCellQuota
public static MyFixedPoint stee = 0;    //SteelPlateQuota
public static MyFixedPoint supe = 0;        //SuperconductorQuota
public static MyFixedPoint thru = 0;		//ThrustQuota
public static MyFixedPoint zone = 0;        //ZoneChip  NEW

public static MyFixedPoint gat = 0; //GatlingAmmoBox
public static MyFixedPoint art = 0; //ArtilleryAmmo
public static MyFixedPoint ass = 0; //AssaultCannonAmmo
public static MyFixedPoint aut = 0; //AutoCannonAmmo
public static MyFixedPoint sra = 0; //SmallRailgunAmmo
public static MyFixedPoint lra = 0; //LargeRailgunAmmo
public static MyFixedPoint mis = 0; //Missile200mm

private Dictionary<string, MyFixedPoint> qoutas = new Dictionary<string, MyFixedPoint>()
{
    {"BulletproofGlass", bule}, {"Canvas", canvas },{"Computer", comp}, {"Construction", cons}, {"Detector",dete},
    {"Display", disp}, {"Explosives", expl}, {"Girder", gird}, {"GravityGenerator", grav}, {"InteriorPlate", inte},
    {"LargeTube", larg}, {"Medical", medi}, {"MetalGrid", meta}, {"Motor", moto}, {"PowerCell", powe},
    {"RadioCommunication", radi}, {"Reactor", reac }, {"SmallTube", smal}, {"SolarCell", sola}, {"SteelPlate", stee}, {"Superconductor", supe}, {"Thrust", thru}, {"ZoneChip", zone},
    {"LargeCalibreAmmo", art}, {"MediumCalibreAmmo", ass}, {"AutocannonClip", aut}, {"NATO_25x184mm", gat}, {"LargeRailgunAmmo", lra}, {"Missile200mm", mis}, {"SmallRailgunAmmo", sra}
};

private Dictionary<string, string> blueprintNames = new Dictionary<string, string>()
{
    {"BulletproofGlass", "BulletproofGlass"}, {"Canvas", "Canvas" },{"Computer", "ComputerComponent"}, {"Construction", "ConstructionComponent"}, {"Detector", "DetectorComponent"},
    {"Display", "Display"}, {"Explosives", "ExplosivesComponent"}, {"Girder", "GirderComponent"}, {"GravityGenerator", "GravityGeneratorComponent"}, {"InteriorPlate", "InteriorPlate"},
    {"LargeTube", "LargeTube"}, {"Medical", "MedicalComponent"}, {"MetalGrid", "MetalGrid"}, {"Motor", "MotorComponent"}, {"PowerCell", "PowerCell"},
    {"RadioCommunication", "RadioCommunicationComponent"}, {"Reactor", "ReactorComponent" }, {"SmallTube", "SmallTube"}, {"SolarCell", "SolarCell"}, {"SteelPlate", "SteelPlate"},
    {"Superconductor", "Superconductor"}, {"Thrust", "ThrustComponent"}, {"ZoneChip", "ZoneChip"}
};

private string[] AmmoNames = new string[7] { "LargeCalibreAmmo", "MediumCalibreAmmo", "AutocannonClip", "NATO_25x184mm", "LargeRailgunAmmo", "Missile200mm", "SmallRailgunAmmo" };
//NATO_25x184mmMagazine

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
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Save()
{
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means.
    //
    // This method is optional and can be removed if not
    // needed.

}

public void Main(string argument, UpdateType updateSource)
{
    List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();  //initialize list of alll blocks
    Dictionary<string, MyFixedPoint> Components = new Dictionary<string, MyFixedPoint>(); // components that are currently in the inventory of the grid
    Dictionary<string, MyFixedPoint> Ammo = new Dictionary<string, MyFixedPoint>(); // Ammo in the grid
    Dictionary<string, MyFixedPoint> toBuild = new Dictionary<string, MyFixedPoint>(); //components that we want to build
    Dictionary<string, MyFixedPoint> ammoToBuild = new Dictionary<string, MyFixedPoint>(); //ammo that we want to build

    Echo("RunNr:" + runNr);
    runNr += 1;
    //GridTerminalSystem.GetBlockGroups(BlockGroups);
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(Blocks); // Get all blocks of System

    var allInventories = Blocks.Where(block => block.HasInventory).SelectMany(AllInventories); //gives a list of all inventories in the grid

    foreach (IMyInventory inv in allInventories)
    {
        List<MyInventoryItem> items = new List<MyInventoryItem>();
        inv.GetItems(items);
        foreach (MyInventoryItem item in items)
        {
            if (item.Type.TypeId.ToString().Equals("MyObjectBuilder_Component") && item.Amount > 0)
            {
                if (Components.ContainsKey(item.Type.SubtypeId.ToString())) // if our dictionary already contains the item, add it ontop else add it to the dictionary
                {
                    Components[item.Type.SubtypeId.ToString()] += item.Amount;
                }
                else
                {
                    Components.Add(item.Type.SubtypeId.ToString(), item.Amount);
                }
            }

            if (item.Type.TypeId.ToString().Equals("MyObjectBuilder_AmmoMagazine") && item.Amount > 0)
            {
                if (Ammo.ContainsKey(item.Type.SubtypeId.ToString())) // if our dictionary already contains the ammo, add it ontop else add it to the dictionary
                {
                    Ammo[item.Type.SubtypeId.ToString()] += item.Amount;
                }
                else
                {
                    Ammo.Add(item.Type.SubtypeId.ToString(), item.Amount);
                }
            }
        }
    }

    Echo("Got inventories of blocks");
    Echo("----------------------------------");


    // get all things in the assembler queue
    List<IMyAssembler> assemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);

    foreach(IMyAssembler ass in assemblers)
    {
        List<MyProductionItem> prod = new List<MyProductionItem>();
        ass.GetQueue(prod);
        foreach(MyProductionItem item in prod)
        {
            var name = item.BlueprintId.ToString().Substring(36);
            name = name == "NATO_25x184mmMagazine" ? "NATO_25x184mm" : name;
            if (AmmoNames.Contains(name))
            {
                if (Ammo.ContainsKey(name))
                {
                    Ammo[name] += item.Amount;
                }
                else
                {
                    Ammo.Add(name, item.Amount);
                }
            }
            else
            {
                name = blueprintNames.FirstOrDefault(str => str.Value == name).Key; // translate blueprint names to component names
                if (Components.ContainsKey(name))
                {
                    Components[name] += item.Amount;
                }
                else
                {
                    Components.Add(name, item.Amount);
                }
            }

        }

    }

    Echo("Got assembler queues");
    Echo("----------------------------------");

    //now we have all components and we can check against quotes

    //find out how much to build
    foreach (KeyValuePair<string, MyFixedPoint> entry in Components)
    {
        //Sometimes Component names have the word Component at the End, we need to strip that
        var key = entry.Key.EndsWith("Component") ? entry.Key.Substring(0, entry.Key.Length - 9) : entry.Key;
        var quota = qoutas[key];
        //Echo("Checking: " + entry.Key + " in storage or in production: " + entry.Value + " goal: " + quota);
        if (entry.Value < quota)
        {
            var amnt = MyFixedPoint.MultiplySafe(quota, 1.1f) - entry.Value;
            toBuild.Add(key, amnt);
            //Echo("Need to build " + amnt + " of: " + key);
        }
    }

    foreach(KeyValuePair<string, MyFixedPoint> entry in Ammo)
    {
        var quota = qoutas[entry.Key];
        if (entry.Value < quota)
        {
            var amnt = MyFixedPoint.MultiplySafe(quota, 1.1f) - entry.Value;
            ammoToBuild.Add(entry.Key, amnt);
        }
    }

    Echo("Know how much to build");
    Echo("----------------------------------");

    //now get the assemblers tagged [SCRIPTED]
    List<IMyAssembler> scriptedAssemblers = assemblers.FindAll(ass => ass.CustomName.Contains("[SCRIPTED]"));

    if (toBuild.Count == 0)
    {
        Echo("Nothing to build exiting.");
    }

    //put the blocks into production
    foreach (KeyValuePair<string, MyFixedPoint> entry in toBuild)
    {
        var defString = "MyObjectBuilder_BlueprintDefinition/" + blueprintNames[entry.Key];
        foreach(IMyAssembler assembler in scriptedAssemblers)
        {
            assembler.AddQueueItem(MyDefinitionId.Parse(defString), ((double)entry.Value) / scriptedAssemblers.Count);
        }

        Echo("Putting " + entry.Value + " of " + entry.Key + " into production.");
    }

    foreach (KeyValuePair<string, MyFixedPoint> entry in ammoToBuild)
    {
        var defString = entry.Key == "NATO_25x184mm" ? "MyObjectBuilder_BlueprintDefinition/NATO_25x184mmMagazine" : "MyObjectBuilder_BlueprintDefinition/" + entry.Key;
        foreach (IMyAssembler assembler in scriptedAssemblers)
        {
            assembler.AddQueueItem(MyDefinitionId.Parse(defString), ((double)entry.Value) / scriptedAssemblers.Count);
        }

        Echo("Putting " + entry.Value + " of " + entry.Key + " into production.");
    }
}

// Get all inventories of a block. Most have just one. Refineries and Assemblers have two.
static IEnumerable<IMyInventory> AllInventories(IMyTerminalBlock block)
{
    for (int i = 0; i < block.InventoryCount; i++)
    {
        yield return block.GetInventory(i);
    }
}