/*
 * Mining System
 * -----------
 * 
 * This script is designed to work with the Mining System drone.
 * It moves the mining area of the nanobot drills to where the drone
 * is looking.
 * 
 * Icon Attribution:
 * https://www.flaticon.com/free-icons/construction-and-tools Construction-and-tools icons created by Smashicons - Flaticon
 * https://www.flaticon.com/free-icons/area Area icons created by Smashicons - Flaticon
 * https://www.flaticon.com/free-icons/box Box icons created by Good Ware - Flaticon
 */

ProgramScreen progScreen;
public Program()
{
    GridInfo.Init("Mining System",GridTerminalSystem, IGC, Me, Echo);
    if (Storage != "") GridInfo.Load(Storage);
    GridBlocks.Init(this);

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    GridInfo.SetVar("DrillsOn", GridBlocks.grid.drillsEnabled.ToString());
    GridInfo.SetVar("ShowArea", GridBlocks.grid.showArea.ToString());
    GridInfo.AddBroadcastListener("MiningArea");
    progScreen = new ProgramScreen(GridBlocks.grid.progScreen);
    ProgramScreen.Debug("Boot Success.");
    ProgramScreen.Debug("Drills Count: " + GridBlocks.grid.drills.Count);
    ProgramScreen.Debug("Cargo Count: " + GridBlocks.grid.cargoContainers.Count);
}

public void Save()
{
    Storage = GridInfo.Save();
}
public void Main(string args, UpdateType updateSource)
{
    List<MyIGCMessage> messages = GridInfo.CheckMessages();
    foreach(MyIGCMessage message in messages)
    {
        HandleMessage(message);
    }
    GridBlocks.CheckInventory();
    progScreen.Draw();
}

// hand network message
public void HandleMessage(MyIGCMessage message)
{
    if (message.Tag == "MiningArea")
    {
        DrillsMatrix.Apply(message.As<string>());
    }
    else if (message.Tag == "ShowMiningArea")
    {
        GridBlocks.grid.showArea = message.As<bool>();
    }
    else if (message.Tag == "DrillsOn")
    {
        GridBlocks.grid.drillsEnabled = message.As<bool>();
    }
    else if (message.Tag == "AreaSize")
    {
        DrillsMatrix.AreaSize(message.As<float>());
    }
    else if (message.Tag == "DroneOnline")
    {
        IGC.SendBroadcastMessage("DrillsEnabled", GridBlocks.grid.drillsEnabled);
        IGC.SendBroadcastMessage("ShowArea", GridBlocks.grid.showArea);
    }
}

//----------------------------------------------------------------------
// grid matrix class to make it easier to work with the mining area
//----------------------------------------------------------------------
public class DrillsMatrix
{
    public static string last_matrix = "";
    public static int last_vector_count = 0;
    // apply a matrix to the drills
    public static void Apply(string matrix)
    {
        if (matrix == last_matrix) return;
        last_matrix = matrix;
        List<Vector3D> vectors = StringToVectorList(matrix);
        last_vector_count = vectors.Count;
        if (vectors.Count == 0) return;
        int i = 0;
        foreach (IMyShipDrill drill in GridBlocks.grid.drills)
        {
            if (i >= vectors.Count) i = 0;
            Vector3D worldPosition = vectors[i];
            Vector3D referenceWorldPosition = drill.WorldMatrix.Translation; //block.WorldMatrix.Translation is the same as block.GetPosition() btw
                                                                             //Convert worldPosition into a world direction
            Vector3D worldDirection = worldPosition - referenceWorldPosition; //this is a vector starting at the reference block pointing at your desired position
                                                                              //Convert worldDirection into a local direction
            Vector3D bodyPosition = Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(drill.WorldMatrix)); //note that we transpose to go from world -> body
            List<ITerminalProperty> drillProps = new List<ITerminalProperty>();
            drill.GetProperties(drillProps);
            foreach (ITerminalProperty prop in drillProps)
            {
                if (prop.Id.Contains("AreaOffsetLeftRight"))
                {
                    drill.SetValue<float>(prop.Id, (float)bodyPosition.X * -1);
                }
                if (prop.Id.Contains("AreaOffsetUpDown"))
                {
                    drill.SetValue<float>(prop.Id, (float)bodyPosition.Y);
                }
                if (prop.Id.Contains("AreaOffsetFrontBack"))
                {
                    drill.SetValue<float>(prop.Id, (float)bodyPosition.Z);
                }
            }
            i++;
        }
    }
    // apply the area size to the drills
    public static void AreaSize(float size)
    {
        if(size < 1) size = 1;
        size = size * 2;
        foreach (IMyShipDrill drill in GridBlocks.grid.drills)
        {
            List<ITerminalProperty> drillProps = new List<ITerminalProperty>();
            drill.GetProperties(drillProps);
            foreach (ITerminalProperty prop in drillProps)
            {
                if (prop.Id.Contains("AreaWidth"))
                {
                    drill.SetValue<float>(prop.Id, size);
                }
                if (prop.Id.Contains("AreaHeight"))
                {
                    drill.SetValue<float>(prop.Id, size);
                }
                if (prop.Id.Contains("AreaDepth"))
                {
                    drill.SetValue<float>(prop.Id, size);
                }
            }
        }
    }
    // convert a list of vectors to a string
    public static string VectorListToString(List<Vector3D> vectors)
    {
        string result = "";
        bool first = true;
        foreach (Vector3D vector in vectors)
        {
            if (first) first = false;
            else result += "\n";
            result += vector.X.ToString() + "," + vector.Y.ToString() + "," + vector.Z.ToString();
        }
        return result;
    }
    // convert a string to a list of vectors
    public static List<Vector3D> StringToVectorList(string text)
    {
        List<Vector3D> result = new List<Vector3D>();
        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            if (parts.Length == 3)
            {
                double x = 0;
                double y = 0;
                double z = 0;
                if (double.TryParse(parts[0], out x) && double.TryParse(parts[1], out y) && double.TryParse(parts[2], out z))
                {
                    result.Add(new Vector3D(x, y, z));
                }
            }
        }
        return result;
    }
}

//----------------------------------------------------------------------
// grid blocks class to hold the blocks for the grid
//----------------------------------------------------------------------
// This class is used to hold the blocks for the grid. It is used to
// make it easier to access the blocks. and do things like turn off
// all the thrusters at once.
//----------------------------------------------------------------------
public class GridBlocks
{
    public static GridBlocks grid;

    public static IMyGridTerminalSystem GridTerminalSystem; // so it can be globally available
    public static IMyIntergridCommunicationSystem IGC; // so it can be globally available
    public static IMyProgrammableBlock Me; // so it can be globally available... lol
    public IMyTextSurface progScreen;

    // lights blocks
    public List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
    public List<IMyLightingBlock> headlights
    {
        get
        {
            List<IMyLightingBlock> result = new List<IMyLightingBlock>();
            foreach (IMyLightingBlock light in lights)
            {
                if (light.CustomName.ToLower().Contains("headlight")) result.Add(light);
            }
            return result;
        }
    }
    public List<IMyLightingBlock> landingLights
    {
        get
        {
            List<IMyLightingBlock> result = new List<IMyLightingBlock>();
            foreach (IMyLightingBlock light in lights)
            {
                if (light.CustomName.ToLower().Contains("landing")) result.Add(light);
            }
            return result;
        }
    }
    public bool landingLightsEnabled
    {
        get
        {
            foreach (IMyLightingBlock light in landingLights)
            {
                if (light.Enabled) return true;
            }
            return false;
        }
        set
        {
            foreach (IMyLightingBlock light in landingLights)
            {
                light.Enabled = value;
            }
        }
    }
    public List<IMyLightingBlock> flightLights
    {
        get
        {
            List<IMyLightingBlock> result = new List<IMyLightingBlock>();
            foreach (IMyLightingBlock light in lights)
            {
                if (light.CustomName.ToLower().Contains("flight")) result.Add(light);
            }
            return result;
        }
    }
    public bool flightLightsEnabled
    {
        get
        {
            foreach (IMyLightingBlock light in flightLights)
            {
                if (light.Enabled) return true;
            }
            return false;
        }
        set
        {
            foreach (IMyLightingBlock light in flightLights)
            {
                light.Enabled = value;
            }
        }
    }
    // connector blocks
    public List<IMyShipConnector> connectors = new List<IMyShipConnector>();
    public bool connected
    {
        get
        {
            foreach (IMyShipConnector connector in connectors)
            {
                if (connector.Status == MyShipConnectorStatus.Connected) return true;
            }
            return false;
        }
    }
    // ore detector blocks
    public List<IMyOreDetector> oreDetectors = new List<IMyOreDetector>();
    public bool oreDetectorsEnabled
    {
        get
        {
            foreach (IMyOreDetector oreDetector in oreDetectors)
            {
                if (oreDetector.Enabled) return true;
            }
            return false;
        }
        set
        {
            foreach (IMyOreDetector oreDetector in oreDetectors)
            {
                oreDetector.Enabled = value;
            }
        }
    }
    // drills blocks
    public List<IMyShipDrill> drills = new List<IMyShipDrill>();
    public bool drillsEnabled
    {
        get
        {
            foreach (IMyShipDrill drill in drills)
            {
                if (drill.Enabled) return true;
            }
            return false;
        }
        set
        {
            foreach (IMyShipDrill drill in drills)
            {
                drill.Enabled = value;
            }
        }
    }
    public bool showArea
    {
        get
        {
            foreach (IMyShipDrill drill in drills)
            {
                List<ITerminalProperty> drillProps = new List<ITerminalProperty>();
                drill.GetProperties(drillProps);
                foreach (ITerminalProperty prop in drillProps)
                {
                    if (prop.Id.Contains("ShowArea"))
                    {
                        return drill.GetValue<bool>(prop.Id);
                    }
                }
            }
            return false;
        }
        set
        {
            foreach (IMyShipDrill drill in drills)
            {
                List<ITerminalProperty> drillProps = new List<ITerminalProperty>();
                drill.GetProperties(drillProps);
                foreach (ITerminalProperty prop in drillProps)
                {
                    if (prop.Id.Contains("ShowArea"))
                    {
                        drill.SetValue<bool>(prop.Id, value);
                    }
                }
            }
        }
    }
    // local cargo blocks
    public List<IMyCargoContainer> cargoContainers = new List<IMyCargoContainer>();
    // local inventories
    public List<IMyInventory> inventories = new List<IMyInventory>();
    public float cargoPercent
    {
        get
        {
            float totalVolume = 0;
            float totalCurrentVolume = 0;
            foreach(IMyInventory inventory in inventories) {
                if(inventory == null) continue; // skip null inventories
                totalVolume += (float)inventory.MaxVolume;
                totalCurrentVolume += (float)inventory.CurrentVolume;
            }
            return totalCurrentVolume / totalVolume;
        }
    }
    // get how much of a specific ore is in the cargo
    public float oreAmount(string oreName)
    {
        float totalAmount = 0;
        MyItemType oreType = MyItemType.MakeOre(oreName);
        foreach (IMyInventory inventory in inventories)
        {
            if (inventory == null) continue; // skip null inventories
            totalAmount += (float)inventory.GetItemAmount(oreType);
        }
        return totalAmount;
    }
    public bool hasOre(string oreName)
    {
        MyItemType oreType = MyItemType.MakeOre(oreName);
        foreach (IMyInventory inventory in inventories)
        {
            if (inventory == null) continue; // skip null inventories
            if (inventory.GetItemAmount(oreType) > 0) return true;
        }
        return false;
    }
    public List<string> oreTypes = new List<string>();

    public string currentOres
    {
        get
        {
            string result = "";
            bool first = true;
            foreach (IMyInventory inventory in inventories)
            {
                if (inventory == null) continue; // skip null inventories
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                inventory.GetItems(items);
                foreach (MyInventoryItem item in items)
                {
                    if (item.Type.TypeId == "MyObjectBuilder_Ore")
                    {
                        string oreName = item.Type.SubtypeId;
                        if (!result.Contains(oreName))
                        {
                            if (!first) result += ", ";
                            result += oreName;
                        }
                    }
                }
            }
            return result;
        }
    }
    // constructor
    public GridBlocks(Program program)
    {
        GridTerminalSystem = program.GridTerminalSystem;
        IGC = program.IGC;
        Me = program.Me;
        // get all the blocks
        progScreen = program.Me.GetSurface(0);
        program.GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(lights, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyOreDetector>(oreDetectors, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drills, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers, b => b.IsSameConstructAs(program.Me));
        // get all the inventories
        foreach (IMyCargoContainer cargoContainer in cargoContainers)
        {
            inventories.Add(cargoContainer.GetInventory());
        }
        // add all the default ore types
        oreTypes.Add("Stone"); oreTypes.Add("Iron"); oreTypes.Add("Nickel");
        oreTypes.Add("Cobalt"); oreTypes.Add("Magnesium"); oreTypes.Add("Silicon");
        oreTypes.Add("Silver"); oreTypes.Add("Gold"); oreTypes.Add("Platinum");
        oreTypes.Add("Uranium"); oreTypes.Add("Ice");
        // set up the grid
        grid = this;
    }
    public static void RefreshGridBlocks(Program program)
    {
        grid = new GridBlocks(program);
    }
    public static void Init(Program program)
    {
        if (grid == null)
        {
            grid = new GridBlocks(program);
            GridInfo.AddChangeListener("DrillsOn", Update);
            GridInfo.AddChangeListener("AreaSize", Update);
            GridInfo.AddChangeBroadcaster("Mining Cam Drone", "CargoPercent");
            GridInfo.AddChangeBroadcaster("Mining Cam Drone", "topOres");
        }
    }
    static void Update(string key, string value)
    {
        if (key == "DrillsOn")
        {
            GridBlocks.grid.drillsEnabled = value.ToLower() == "true";
        }
        else if (key == "ShowArea")
        {
            GridBlocks.grid.showArea = value.ToLower() == "true";
        }
        else if (key == "AreaSize")
        {
            DrillsMatrix.AreaSize(float.Parse(value));
        }
    }
    public static void CheckInventory()
    {
        if (grid == null) return;
        GridInfo.SetVar("CargoPercent", grid.cargoPercent.ToString());
        Dictionary<string, float> oreAmounts = new Dictionary<string, float>();
        foreach (string oreName in grid.oreTypes)
        {
            float amount = grid.oreAmount(oreName);
            GridInfo.SetVar(oreName, amount.ToString());
            GridInfo.SetVar("has" + oreName, (amount > 0).ToString());
            oreAmounts.Add(oreName, amount);
        }
        oreAmounts = oreAmounts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        string topOres = "";
        bool first = true;
        int count = 0;
        foreach (KeyValuePair<string, float> oreAmount in oreAmounts)
        {
            if (oreAmount.Value <= 0) break;
            if (first) first = false;
            else topOres += ", ";
            topOres += oreAmount.Key;
            count++;
            if (count >= 6) break;
        }
        GridInfo.SetVar("topOres", topOres);
    }
}

//---------------------------------------------------------------//
// Grid Info                                                     //
//---------------------------------------------------------------//
// holds some basic info about the grid and other useful stuff   //
// to have globally can also report changes to variables and     //
// send them to other grids.                                     //
//---------------------------------------------------------------//
// add to Program():                                             //
// GridInfo.Init("Program Name",GridTerminalSystem,IGC,Me,Echo); //
// if(Storage != "") GridInfo.Load(Storage);                     //
//                                                               //
// add to Save():                                                //
// GridInfo.Save();                                              //
//                                                               //
// add to Main():                                                //
// GridInfo.CheckMessages();                                     //
//                                                               //
// usage:                                                        //
// GridInfo.SetVar("varname","value");                           //
// GridInfo.GetVarAs<T>("varname","optionalDefault");            //
//                                                               //
// change listener:                                              //
// GridInfo.AddVarChangedHandler("varname",MyHandler);           //
//                                                               //
// change broadcasting:                                          //
// GridInfo.AddChangeBroadcaster("program","varname");           //
// GridInfo.AddChangeUnicaster(igcAddress,"varname");            //
//---------------------------------------------------------------//
public class GridInfo
{
    public static long RunCount = 0; // to store how many times the script has run since compiling
    public static string ProgramName = "Program"; // the name of the program
    public static IMyGridTerminalSystem GridTerminalSystem; // so it can be globally available
    public static IMyIntergridCommunicationSystem IGC; // so it can be globally available
    public static IMyProgrammableBlock Me; // so it can be globally available... lol
    public static Action<string> EchoAction; // EchoAction?.Invoke("hello");
    private static IMyBroadcastListener broadcastListener; // so it can be globally available
    private static List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>(); // so it can be globally available
    private static string bound_vars = ""; // a list of vars that have been bound to the grid
    private static Dictionary<string, string> broadcast_vars = new Dictionary<string, string>(); // a list of vars that have been bound to the grid
    private static Dictionary<string, long> unicast_vars = new Dictionary<string, long>(); // a list of vars that have been bound to the grid
    public static bool handleUnicastMessages = false;
    public static void Echo(string message)
    {
        EchoAction?.Invoke(message);
    }
    public static Dictionary<string, string> GridVars = new Dictionary<string, string>();
    //-------------------------------------------//
    // setup GridInfo                            //
    //-------------------------------------------//
    public static void Init(string name, IMyGridTerminalSystem gts, IMyIntergridCommunicationSystem igc, IMyProgrammableBlock me, Action<string> echo)
    {
        ProgramName = name;
        GridTerminalSystem = gts;
        IGC = igc;
        Me = me;
        EchoAction = echo;
        broadcastListener = IGC.RegisterBroadcastListener(ProgramName);
        Me.CustomName = "Program: " + ProgramName + " @"+IGC.Me.ToString();
    }
    //-------------------------------------------//
    // handle broadcast messages                 //
    //-------------------------------------------//
    public static List<MyIGCMessage> CheckMessages()
    {
        List<MyIGCMessage> messages = new List<MyIGCMessage>();
        while (broadcastListener.HasPendingMessage)
        {
            MyIGCMessage message = broadcastListener.AcceptMessage();
            string[] data = message.As<string>().Split('║');
            if (data.Length == 2)
            {
                SetVar(data[0], data[1]);
            }
        }
        while (IGC.UnicastListener.HasPendingMessage && handleUnicastMessages)
        {
            MyIGCMessage message = IGC.UnicastListener.AcceptMessage();
            if(GridVars.ContainsKey(message.Tag)) SetVar(message.Tag, message.As<string>());
        }
        foreach (IMyBroadcastListener listener in listeners)
        {
            while (listener.HasPendingMessage)
            {
                messages.Add(listener.AcceptMessage());
            }
        }
        return messages;
    }
    public static IMyBroadcastListener AddBroadcastListener(string name)
    {
        IMyBroadcastListener listener = IGC.RegisterBroadcastListener(name);
        listeners.Add(listener);
        return listener;
    }
    //-------------------------------------------//
    // Get a var as a specific type of variable  //
    //                                           //
    // key - the id of the variable to get       //
    // defaultValue - the value to return if     //
    //                the variable doesn't exist //
    //-------------------------------------------//
    public static T GetVarAs<T>(string key, T defaultValue = default(T))
    {
        if (!GridVars.ContainsKey(key)) return defaultValue; //(T)Convert.ChangeType(null,typeof(T));
        return (T)Convert.ChangeType(GridVars[key], typeof(T));
    }
    //-------------------------------------------//
    // set a grid info var                       //
    //                                           //
    // key - the id of the variable to set       //
    // value - the value (converted to a string) //
    //-------------------------------------------//
    public static void SetVar(string key, string value)
    {
        if (GridVars.ContainsKey(key)) GridVars[key] = value;
        else GridVars.Add(key, value);
        if (bound_vars.Contains(key + "║")) OnVarChanged(key, value);
        if (broadcast_vars.ContainsKey(key)) IGC.SendBroadcastMessage(broadcast_vars[key], key + "║" + value);
        if (unicast_vars.ContainsKey(key)) IGC.SendUnicastMessage(unicast_vars[key], key, value);
    }
    //------------------------------------------------------------//
    // converts the grid info vars to a string to save in Storage //
    //------------------------------------------------------------//
    public static string Save()
    {
        StringBuilder storage = new StringBuilder();
        foreach (KeyValuePair<string, string> var in GridVars)
        {
            storage.Append(var.Key + "║" + var.Value + "\n");
        }
        return storage.ToString();
    }
    //----------------------------------------------//
    // parse the Storage string into grid info vars //
    //----------------------------------------------//
    public static void Load(string storage)
    {
        string[] lines = storage.Split('\n');
        foreach (string line in lines)
        {
            string[] var = line.Split('║');
            if (var.Length == 2)
            {
                GridVars.Add(var[0], var[1]);
            }
        }
    }
    //----------------------------------//
    // event for when a var is changed  //
    //----------------------------------//
    public static event Action<string, string> VarChanged;
    private static void OnVarChanged(string key, string value)
    {
        VarChanged?.Invoke(key, value);
    }
    public static void AddChangeListener(string key, Action<string, string> handler)
    {
        bound_vars += key + "║";
        VarChanged += handler;
    }
    // send changes to a prog by its name
    public static void AddChangeBroadcaster(string progName, string key)
    {
        broadcast_vars.Add(key, progName);
    }
    // send changes to a prog by its igc address
    public static void AddChangeUnicaster(string key, long id)
    {
        unicast_vars.Add(key, id);
        handleUnicastMessages = true;
    }

    //----------------------------------//
    // the world position for the block //
    //----------------------------------//
    public static Vector3D BlockWorldPosition(IMyFunctionalBlock block, Vector3D offset = new Vector3D())
    {
        return Vector3D.Transform(offset, block.WorldMatrix);
    }
}

public class ProgramScreen : Screen
{
    public static void Debug(string message)
    {
        if (DebugAction != null)
        {
            DebugAction(message);
        }
    }
    static event Action<string> DebugAction;
    string drillIcon = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
    string areaIcon = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
    string cargoIcon = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
    ScreenSprite title;
    ScreenSprite DebugText;
    public int DebugLineMax = 10;
    public bool ShowDebug
    {
        get { return DebugText.Visible; }
        set { DebugText.Visible = value; }
    }
    public bool ShowTitle
    {
        get { return title.Visible; }
        set { title.Visible = value; }
    }
    public string Title
    {
        get { return title.Data; }
        set { title.Data = value; }
    }
    public ProgramScreen(IMyTextSurface drawingSurface)
        : base(drawingSurface)
    {
        title = AddTextSprite(ScreenSprite.ScreenSpriteAnchor.TopCenter,new Vector2(0,20),1.2f,Color.White,"White",GridInfo.ProgramName,TextAlignment.CENTER);
        DebugText = AddTextSprite(ScreenSprite.ScreenSpriteAnchor.CenterLeft, new Vector2(0, -130), 0.75f, Color.White, "White", "", TextAlignment.LEFT);
        AddTogglePixelIcon(new Vector2(-10, 0), "", drillIcon, "DrillsOn", Color.White, new Vector2(64, 64), 0.02f, ScreenSprite.ScreenSpriteAnchor.CenterRight, TextAlignment.RIGHT);
        AddTogglePixelIcon(new Vector2(-10, -50), "", areaIcon, "ShowArea", Color.White, new Vector2(64, 64), 0.02f, ScreenSprite.ScreenSpriteAnchor.CenterRight, TextAlignment.RIGHT);

        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Ice", "hasIce",new Vector2(-50,25), new Vector2(25,25), ScreenSprite.ScreenSpriteAnchor.TopRight,TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Stone", "hasStone", new Vector2(-25, 25), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Iron", "hasIron", new Vector2(0, 25), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Nickel", "hasNickel", new Vector2(-50, 50), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Silicon", "hasSilicon", new Vector2(-25, 50), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Cobalt", "hasCobalt", new Vector2(0, 50), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Magnesium", "hasMagnesium", new Vector2(-50, 75), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Silver", "hasSilver", new Vector2(-25, 75), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Gold", "hasGold", new Vector2(0, 75), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Platinum", "hasPlatinum", new Vector2(0, 100), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);
        AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "MyObjectBuilder_Ore/Uranium", "hasUranium", new Vector2(-25, 100), new Vector2(25, 25), ScreenSprite.ScreenSpriteAnchor.TopRight, TextAlignment.RIGHT);

        AddPixelIcon(new Vector2(-110,-50), cargoIcon, Color.White, new Vector2(64, 64), 0.02f, ScreenSprite.ScreenSpriteAnchor.BottomCenter, TextAlignment.RIGHT);
        AddPercentageBar("CargoPercent", new Vector2(-100, -30), new Vector2(200, 25), Color.White, Color.LightGreen,Color.Red,Color.DarkTurquoise,0f, ScreenSprite.ScreenSpriteAnchor.BottomCenter);
        ProgramScreen.DebugAction += DebugMessage;
    }
    void DebugMessage(string message)
    {
        GridInfo.Echo(message);
        // if the debug text has more than 10 lines, remove the first line
        if(DebugText.Data.Split('\n').Length > DebugLineMax)
        {
            DebugText.Data = DebugText.Data.Substring(DebugText.Data.IndexOf('\n') + 1);
        }
        DebugText.Data += "\n" + message;
    }
}

//----------------------------------------------------------------------
// Screen - encapsulates a drawing surface
//----------------------------------------------------------------------
public class Screen
{
    private IMyTextSurface _drawingSurface;
    private RectangleF _viewport;
    List<ScreenSprite> _sprites = new List<ScreenSprite>();
    public Color BackgroundColor
    {
        get { return _drawingSurface.ScriptBackgroundColor; }
        set { _drawingSurface.ScriptBackgroundColor = value; }
    }
    public Color ForegroundColor
    {
        get { return _drawingSurface.ScriptForegroundColor; }
        set { _drawingSurface.ScriptForegroundColor = value; }
    }
    //
    // constructor
    //
    public Screen(IMyTextSurface drawingSurface)
    {
        _drawingSurface = drawingSurface;
        _drawingSurface.ContentType = ContentType.SCRIPT;
        _drawingSurface.Script = "";
        // calculate the viewport offset by centering the surface size onto the texture size
        _viewport = new RectangleF(
                               (_drawingSurface.TextureSize - _drawingSurface.SurfaceSize) / 2f,
                                                  _drawingSurface.SurfaceSize
                                                                 );
    }
    //
    // DrawSprites - draw sprites to the screen
    //
    public void Draw()
    {
        var frame = _drawingSurface.DrawFrame();
        DrawSprites(ref frame);
        frame.Dispose();
    }
    //
    // DrawSprites - draw sprites to the screen
    //
    private void DrawSprites(ref MySpriteDrawFrame frame)
    {
        // draw all the sprites
        foreach (ScreenSprite sprite in _sprites)
        {
            if (sprite.Visible) frame.Add(sprite.ToMySprite(_viewport));
        }
    }
    // add a text sprite
    public ScreenSprite AddTextSprite(ScreenSprite.ScreenSpriteAnchor anchor, Vector2 position, float rotationOrScale, Color color, string fontId, string data, TextAlignment alignment)
    {
        ScreenSprite sprite = new ScreenSprite(anchor, position, rotationOrScale, new Vector2(0, 0), color, fontId, data, alignment, SpriteType.TEXT);
        _sprites.Add(sprite);
        return sprite;
    }
    // add a texture sprite
    public ScreenSprite AddTextureSprite(ScreenSprite.ScreenSpriteAnchor anchor, Vector2 position, float rotationOrScale, Vector2 size, Color color, string data)
    {
        ScreenSprite sprite = new ScreenSprite(anchor, position, rotationOrScale, size, color, "", data, TextAlignment.CENTER, SpriteType.TEXTURE);
        _sprites.Add(sprite);
        return sprite;
    }
    // add an Action Bar
    public ScreenActionBar AddActionBar(int actionCount, string actionString = "", float fontSize = 1f)
    {
        ScreenActionBar actionBar = new ScreenActionBar(actionCount, actionString);
        // calculate how much width each action has
        Vector2 actionWidth = new Vector2(_viewport.Width / actionCount, 0);
        Vector2 actionPosition = new Vector2(actionWidth.X / 2f, -30 * fontSize);
        for (int i = 0; i < actionCount; i++)
        {
            ScreenSprite sprite = AddTextSprite(ScreenSprite.ScreenSpriteAnchor.BottomLeft, actionPosition, fontSize, Color.White, "White", "", TextAlignment.CENTER);
            actionPosition += actionWidth;
            actionBar.AddSprite(sprite);
            _sprites.Add(sprite);
        }
        return actionBar;
    }
    // add a menu
    public void AddMenu(ScreenMenu menu)
    {
        menu.AddSprites(ref _sprites);
    }
    public ScreenToggleIcon AddToggleIcon(string offIcon, string onIcon, string variableName, Vector2 position, Vector2 size, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
    {
        ScreenToggleIcon toggleIcon = new ScreenToggleIcon(offIcon, onIcon, variableName, position, size, anchor, alignment);
        _sprites.Add(toggleIcon.Sprite);
        return toggleIcon;
    }
    public PixelIcon AddPixelIcon(Vector2 position, string data, Color color, Vector2 size, float scale = 0.01f, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
    {
        PixelIcon pixelIcon = new PixelIcon(position, data, color, size, scale, anchor, alignment);
        _sprites.Add(pixelIcon);
        return pixelIcon;
    }
    public PixelIcon AddTogglePixelIcon(Vector2 position, string offIcon, string onIcon, string variableName, Color color, Vector2 size, float scale = 0.01f, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
    {
        PixelIcon pixelIcon = new PixelIcon(position, offIcon, onIcon, variableName, color, size, scale, anchor, alignment);
        _sprites.Add(pixelIcon);
        return pixelIcon;
    }
    public PixelIcon AddVariablePixelIcon(Vector2 position, List<string> variableIcons, string variableName, Color color, Vector2 size, float scale = 0.01f, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
    {
        PixelIcon pixelIcon = new PixelIcon(position, variableIcons, variableName, color, size, scale, anchor, alignment);
        _sprites.Add(pixelIcon);
        return pixelIcon;
    }
    public ScreenPercentageBar AddPercentageBar(string varName, Vector2 position, Vector2 size, Color borderColor, Color barColor,Color barFull, Color backColor, float percent, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.BottomCenter)
    {
        ScreenPercentageBar percentageBar = new ScreenPercentageBar(varName, position, size, borderColor, barColor,barFull,backColor, percent, anchor);
        percentageBar.AddSprites(ref _sprites);
        return percentageBar;
    }
}
//----------------------------------------------------------------------
// screen sprite - encapsulates a sprite
//----------------------------------------------------------------------
public class ScreenSprite
{
    public enum ScreenSpriteAnchor
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
    public ScreenSpriteAnchor Anchor { get; set; }
    public Vector2 Position { get; set; }
    public float RotationOrScale { get; set; }
    public Vector2 Size { get; set; }
    public Color Color { get; set; }
    public string FontId { get; set; }
    public string Data { get; set; }
    public TextAlignment Alignment { get; set; }
    public SpriteType Type { get; set; }
    public bool Visible { get; set; } = true;
    public ScreenSprite()
    {
        Anchor = ScreenSpriteAnchor.Center;
        Position = Vector2.Zero;
        RotationOrScale = 1f;
        Size = Vector2.Zero;
        Color = Color.White;
        FontId = "White";
        Data = "";
        Alignment = TextAlignment.CENTER;
        Type = SpriteType.TEXT;
    }
    public ScreenSprite(ScreenSpriteAnchor anchor, Vector2 position, float rotationOrScale, Vector2 size, Color color, string fontId, string data, TextAlignment alignment, SpriteType type)
    {
        Anchor = anchor;
        Position = position;
        RotationOrScale = rotationOrScale;
        Size = size;
        Color = color;
        FontId = fontId;
        Data = data;
        Alignment = alignment;
        Type = type;
    }
    // convert the sprite to a MySprite
    public MySprite ToMySprite(RectangleF _viewport)
    {
        if (Type == SpriteType.TEXT)
        {
            return new MySprite()
            {
                Type = Type,
                Data = Data,
                Position = GetPosition(_viewport),
                RotationOrScale = RotationOrScale,
                Color = Color,
                Alignment = Alignment,
                FontId = FontId
            };
        }
        return new MySprite()
        {
            Type = Type,
            Data = Data,
            Position = GetPosition(_viewport),
            RotationOrScale = RotationOrScale,
            Color = Color,
            Alignment = Alignment,
            Size = Size,
            FontId = FontId
        };
    }
    private Vector2 GetPosition(RectangleF _viewport)
    {
        Vector2 _position = Position + _viewport.Position;
        switch (Anchor)
        {
            case ScreenSpriteAnchor.TopCenter:
                _position = Position + new Vector2(_viewport.Center.X, _viewport.Y);
                break;
            case ScreenSpriteAnchor.TopRight:
                _position = Position + new Vector2(_viewport.Right, _viewport.Y);
                break;
            case ScreenSpriteAnchor.CenterLeft:
                _position = Position + new Vector2(_viewport.X, _viewport.Center.Y);
                break;
            case ScreenSpriteAnchor.Center:
                _position = Position + _viewport.Center;
                break;
            case ScreenSpriteAnchor.CenterRight:
                _position = Position + new Vector2(_viewport.Right, _viewport.Center.Y);
                break;
            case ScreenSpriteAnchor.BottomLeft:
                _position = Position + new Vector2(_viewport.X, _viewport.Bottom);
                break;
            case ScreenSpriteAnchor.BottomCenter:
                _position = Position + new Vector2(_viewport.Center.X, _viewport.Bottom);
                break;
            case ScreenSpriteAnchor.BottomRight:
                _position = Position + new Vector2(_viewport.Right, _viewport.Bottom);
                break;
        }
        return _position;
    }

}

//----------------------------------------------------------------------
// ScreenActionBar
//----------------------------------------------------------------------
public class ScreenActionBar
{
    private string[] actions;
    List<ScreenSprite> sprites = new List<ScreenSprite>();
    public int Count { get { return actions.Length; } }

    public ScreenActionBar(int actionCount, string actionString = "")
    {
        actions = new string[actionCount];
        if (actionString != "")
        {
            SetActions(actionString);
        }
    }
    // set actions from a string separated by spaces
    public void SetActions(string actionString)
    {
        string[] _actions = actionString.Split(' ');
        for (int i = 0; i < actions.Length; i++)
        {
            if (i < _actions.Length)
            {
                actions[i] = _actions[i];
            }
            else
            {
                actions[i] = "";
            }
            if (sprites.Count > i)
            {
                sprites[i].Data = actions[i];
            }

        }
    }
    // set a sprite for an action
    public void AddSprite(ScreenSprite sprite)
    {
        if (sprites.Count < actions.Length)
        {
            sprites.Add(sprite);
            sprite.Data = actions[sprites.Count - 1];
        }
    }
    // handle action bar input takes an int and returns a string
    public string HandleInput(int input)
    {
        if (input >= 0 && input < actions.Length)
        {
            return actions[input].ToLower();
        }
        return "";
    }
}

//----------------------------------------------------------------------
// ScreenMenu
//----------------------------------------------------------------------
public class ScreenMenu
{
    ScreenSprite title;
    List<ScreenMenuItem> menuItems = new List<ScreenMenuItem>();
    int selectedIndex = 0;
    public float ItemHeight = 30f;
    public float ItemIndent = 20f;
    float _width = 100f;
    float headerHeight = 1.2f;
    ScreenActionBar actionBar;
    public string menuScrollActions
    {
        get
        {
            if (actionBar != null)
            {
                if (actionBar.Count == 5) return "Up Down Select  Back";
                if (actionBar.Count == 4) return "Up Down Select Back";
            }
            return "Up Down Select";
        }
    }
    public string menuEditActions
    {
        get
        {
            if (actionBar != null)
            {
                if (actionBar.Count == 5) return "+ ++ - -- Done";
                if (actionBar.Count == 4) return "+ -  Done";
            }
            return "+ - Done";
        }
    }
    public float Width
    {
        get { return _width; }
        set
        {
            _width = value;
            foreach (ScreenMenuItem item in menuItems)
            {
                item.Width = value;
            }
        }
    }
    public float Height
    {
        get
        {
            if (menuItems.Count == 0) return 0;
            return ItemHeight * menuItems.Count + (ItemHeight * headerHeight);
        }
    }
    public int SelectedIndex
    {
        get { return selectedIndex; }
        set
        {
            if (value >= 0 && value < menuItems.Count)
            {
                if (menuItems.Count > 0) menuItems[selectedIndex].Selected = false;
                selectedIndex = value;
                menuItems[selectedIndex].Selected = true;
            }
        }
    }
    public bool Visible
    {
        get
        {
            return title.Visible;
        }
        set
        {
            title.Visible = value;
            foreach (ScreenMenuItem item in menuItems)
            {
                item.Visible = value;
            }
        }
    }
    public ScreenMenuItem SelectedItem
    {
        get
        {
            if (menuItems.Count == 0) return null;
            return menuItems[selectedIndex];
        }
    }
    public ScreenMenu(string title, float width, ScreenActionBar actionBar)
    {
        _width = width;
        this.title = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.CenterLeft, Vector2.Zero, headerHeight, Vector2.Zero, Color.White, "White", title, TextAlignment.LEFT, SpriteType.TEXT);
        this.actionBar = actionBar;
    }
    // add a label to the menu
    public void AddLabel(string label)
    {
        menuItems.Add(new ScreenMenuItem(label, _width));
    }
    // add variable to the menu
    public void AddVariable(string label, string varName, string defaultValue = "")
    {
        menuItems.Add(new ScreenMenuItem(label, varName, _width, defaultValue));
    }
    // add the sprites to the render list and positions the menu items
    public void AddSprites(ref List<ScreenSprite> sprites)
    {
        Vector2 position = new Vector2(ItemIndent, Height / -2);
        title.Position = position;
        sprites.Add(title);
        position.Y += ItemHeight * headerHeight;
        int i = 0;
        foreach (ScreenMenuItem item in menuItems)
        {
            item.Selected = (i++ == selectedIndex);
            item.Position = position;
            position.Y += ItemHeight;
            item.AddSprites(ref sprites);
        }
    }
    // remove the sprites from the render list
    public void RemoveSprites(ref List<ScreenSprite> sprites)
    {
        foreach (ScreenMenuItem item in menuItems)
        {
            item.RemoveSprites(ref sprites);
        }
    }
    public string HandleInput(string argument)
    {
        GridInfo.Me.CustomData += "\nHandleInput " + argument;
        if (argument.ToLower().StartsWith("btn"))
        {
            GridInfo.Me.CustomData += "\nbtn " + argument;
            // handle button press
            string[] args = argument.Split(' ');
            int btn = -1;
            if (args.Length > 1)
            {
                int.TryParse(args[1], out btn);
            }
            if (btn > 0)
            {
                string action = actionBar.HandleInput(btn - 1);
                GridInfo.Me.CustomData += "\naction " + action;
                if (action == "up")
                {
                    SelectedItem.Editing = false;
                    SelectedIndex--;
                }
                else if (action == "down")
                {
                    SelectedItem.Editing = false;
                    SelectedIndex++;
                }
                else if (action == "select")
                {
                    if (SelectedItem.IsAction)
                    {
                        return (SelectedItem.Label.ToLower());
                    }
                    else if (SelectedItem.IsToggle)
                    {
                        SelectedItem.Editing = true;
                        SelectedItem.DataAsBool = !SelectedItem.DataAsBool;
                    }
                    else
                    {
                        SelectedItem.Editing = true;
                        actionBar.SetActions(menuEditActions);
                    }
                }
                else if (action == "+")
                {
                    SelectedItem.DataAsInt++;
                }
                else if (action == "++")
                {
                    SelectedItem.DataAsInt += 10;
                }
                else if (action == "-")
                {
                    SelectedItem.DataAsInt--;
                }
                else if (action == "--")
                {
                    SelectedItem.DataAsInt -= 10;
                }
                else if (action == "done")
                {
                    SelectedItem.Editing = false;
                    actionBar.SetActions(menuScrollActions);
                }
                else
                {
                    return action;
                }
            }
        }
        return "";
    }

}

//----------------------------------------------------------------------
// ScreenMenuItem
//----------------------------------------------------------------------
public class ScreenMenuItem
{
    ScreenSprite bullet;
    ScreenSprite label;
    ScreenSprite text;
    string variableName;
    Color Color = Color.White;
    Color selectedColor = Color.LightYellow;
    Color editingColor = Color.Orange;
    string bulletIcon = "";
    string bulletSelectedIcon = ">";
    string trueValue = "on";
    string falseValue = "off";
    bool selected = false;
    bool editing = false;
    bool isToggle = false;
    public bool IsToggle { get { return isToggle; } }
    public bool IsAction { get { return text == null; } }
    // update the label of the variable
    public string Label
    {
        get { return label.Data; }
        set { label.Data = value; }
    }
    // update the displayed value of the variable
    public string Data
    {
        get
        {
            if (text == null) return "";
            return text.Data;
        }
        set
        {
            if (text != null) text.Data = value;
        }
    }
    // for when the menu is editing the value of the variable
    // updates the global variable when set
    public int DataAsInt
    {
        get
        {
            if (text == null) return 0;
            return GridInfo.GetVarAs<int>(variableName);
        }
        set
        {
            if (variableName != "") GridInfo.SetVar(variableName, value.ToString());
        }
    }
    // for when the menu is editing toggling the value of the variable
    // updates the global variable when set
    public bool DataAsBool
    {
        get
        {
            if (text == null) return false;
            return GridInfo.GetVarAs<bool>(variableName);
        }
        set
        {
            if (variableName != "") GridInfo.SetVar(variableName, value.ToString());
        }
    }
    // update the icon of the variable
    public string Icon
    {
        get { return bullet.Data; }
        set { bullet.Data = value; }
    }
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            if (selected)
            {
                bullet.Data = bulletSelectedIcon;
                label.Color = selectedColor;
                if (text != null)
                {
                    if (editing)
                    {
                        text.Color = editingColor;
                    }
                    else
                    {
                        text.Color = selectedColor;
                    }
                }
            }
            else
            {
                bullet.Data = bulletIcon;
                label.Color = Color;
                if (text != null) text.Color = Color;
            }
        }
    }
    public bool Editing
    {
        get { return editing; }
        set
        {
            editing = value;
            if (text != null)
            {
                if (editing)
                {
                    text.Color = editingColor;
                }
                else
                {
                    text.Color = selectedColor;
                }

            }
        }
    }
    public bool Visible
    {
        get { return bullet.Visible; }
        set
        {
            bullet.Visible = value;
            label.Visible = value;
            if (text != null) text.Visible = value;
        }
    }
    private Vector2 _postion;
    private float _width;
    public float Width
    {
        get { return _width; }
        set
        {
            _width = value;
            _postion = bullet.Position;
            _postion += new Vector2(_width, 0);
            if (text != null) text.Position = _postion;
        }
    }
    public Vector2 Position
    {
        get { return bullet.Position; }
        set
        {
            bullet.Position = value;
            label.Position = value;
            if (text != null) text.Position = value + new Vector2(_width, 0);
        }
    }
    public ScreenMenuItem(string label, string varName, float width, string defaultValue)
    {
        _postion = new Vector2(0, 0);
        _width = width;
        variableName = varName;
        GridInfo.AddChangeListener(varName, Update);
        bullet = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.CenterLeft, _postion, 1f, new Vector2(0, 0), Color, "White", bulletIcon, TextAlignment.RIGHT, SpriteType.TEXT);
        this.label = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.CenterLeft, _postion, 1f, new Vector2(1, 0), Color, "White", label, TextAlignment.LEFT, SpriteType.TEXT);
        _postion += new Vector2(width, 0);
        text = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.CenterLeft, _postion, 1f, new Vector2(0, 0), Color, "White", GridInfo.GetVarAs<string>(variableName), TextAlignment.LEFT, SpriteType.TEXT);
        DisplayValue(GridInfo.GetVarAs<string>(variableName, defaultValue));
    }
    public ScreenMenuItem(string label, float width)
    {
        _postion = new Vector2(0, 0);
        _width = width;
        bullet = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.CenterLeft, _postion, 1f, new Vector2(0, 0), Color, "White", bulletIcon, TextAlignment.RIGHT, SpriteType.TEXT);
        this.label = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.CenterLeft, _postion, 1f, new Vector2(1, 0), Color, "White", label, TextAlignment.LEFT, SpriteType.TEXT);
    }
    // add the sprites to the render list
    public void AddSprites(ref List<ScreenSprite> sprites)
    {
        sprites.Add(bullet);
        sprites.Add(label);
        if (text != null)
        {
            sprites.Add(text);
        }
    }
    // remove the sprites from the render list
    public void RemoveSprites(ref List<ScreenSprite> sprites)
    {
        sprites.Remove(bullet);
        sprites.Remove(label);
        if (text != null)
        {
            sprites.Remove(text);
        }
    }
    // update the text sprite with the current value of the variable
    private void Update(string key, string value)
    {
        if (text != null && key == variableName)
        {
            DisplayValue(value);
        }
    }
    private void DisplayValue(string value)
    {
        // if the variable is a bool, parse it and set the text to the bool value
        bool boolValue;
        if (bool.TryParse(value, out boolValue))
        {
            isToggle = true;
            text.Data = boolValue ? trueValue : falseValue;
        }
        else
        {
            isToggle = false;
            text.Data = value;
        }
    }
    public void Style(string bullet_unselected, string bullet_selected, Color color, Color selectedColor, Color editingColor)
    {
        bulletIcon = bullet_unselected;
        bulletSelectedIcon = bullet_selected;
        Color = color;
        this.selectedColor = selectedColor;
        this.editingColor = editingColor;
        bullet.Data = bulletIcon;
        if (Selected)
        {
            bullet.Data = bulletSelectedIcon;
            label.Color = selectedColor;
            if (text != null)
            {
                if (editing)
                {
                    text.Color = editingColor;
                }
                else
                {
                    text.Color = selectedColor;
                }
            }
        }
        else
        {
            bullet.Data = bulletIcon;
            label.Color = Color;
            if (text != null) text.Color = Color;
        }
    }
}

//----------------------------------------------------------------------
// ScreenPercentageBar
//----------------------------------------------------------------------
public class ScreenPercentageBar
{
    ScreenSprite border_top;
    ScreenSprite border_bottom;
    ScreenSprite border_left;
    ScreenSprite border_right;
    ScreenSprite bar;
    ScreenSprite back;
    float percent;
    string varName;
    Color barFull;
    Color barColor;
    Vector2 Size;

    public ScreenPercentageBar(string varName, Vector2 position, Vector2 size, Color borderColor, Color barColor, Color barFull, Color backColor, float percent, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.BottomCenter, float borderSize = 2f)
    {
        this.percent = GridInfo.GetVarAs<float>(varName,percent);
        this.barFull = barFull;
        this.barColor = barColor;
        Size = size;
        bar = new ScreenSprite(anchor, position, 0, new Vector2(size.X*percent,size.Y), barColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        back = new ScreenSprite(anchor, position, 0, size, backColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        border_top = new ScreenSprite(anchor, new Vector2(position.X,position.Y-(size.Y/2)), 0, new Vector2(size.X,borderSize), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        border_bottom = new ScreenSprite(anchor, new Vector2(position.X,position.Y + (size.Y/2)), 0, new Vector2(size.X,borderSize), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        border_left = new ScreenSprite(anchor, position, 0, new Vector2(borderSize,size.Y), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        border_right = new ScreenSprite(anchor, new Vector2(position.X + size.X,position.Y), 0, new Vector2(borderSize,size.Y), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        this.varName = varName;
        GridInfo.AddChangeListener(varName, OnVarUpdated);
    }
    public void AddSprites(ref List<ScreenSprite> sprites)
    {
        sprites.Add(back);
        sprites.Add(bar);
        sprites.Add(border_top);
        sprites.Add(border_bottom);
        sprites.Add(border_left);
        sprites.Add(border_right);
    }
    void OnVarUpdated(string varName, string value)
    {
        if (varName == this.varName)
        {
            percent = float.Parse(value);
            bar.Size = new Vector2(Size.X * percent, Size.Y);
            if(percent > 0.75f)
            {
                bar.Color = barFull;
            }
            else
            {
                bar.Color = barColor;
            }
        }
    }
}

//----------------------------------------------------------------------
// PixelIcon
//----------------------------------------------------------------------
public class PixelIcon : ScreenSprite
{
    Vector2 _size;
    Vector2 _addPosition = Vector2.Zero;
    string onIcon = "";
    string offIcon = "";
    List<string> variableIcons = new List<string>();
    bool _state = false;
    int _variableIndex = 0;
    string _variableName = "";
    public bool State
    {
        get { return _state; }
        set
        {
            _state = value;
            Data = _state ? onIcon : offIcon;
        }
    }
    public int Index
    {
        get { return _variableIndex; }
        set
        {
            _variableIndex = value;
            if (_variableIndex < 0)
            {
                _variableIndex = 0;
            }
            else if (_variableIndex >= variableIcons.Count)
            {
                _variableIndex = variableIcons.Count - 1;
            }
            Data = variableIcons[_variableIndex];
        }
    }
    // static pixel icon
    public PixelIcon(Vector2 position, string data, Color color, Vector2 size, float scale = 0.01f, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
        : base(anchor, position, scale, Vector2.Zero, color, "Monospace", data, alignment, SpriteType.TEXT)
    {
        _size = size;
    }
    // toggle pixel icon
    public PixelIcon(Vector2 position, string offIcon, string onIcon, string variableName, Color color, Vector2 size, float scale = 0.01f, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
        : base(anchor, position, scale, Vector2.Zero, color, "Monospace", offIcon, alignment, SpriteType.TEXT)
    {
        _size = size;
        this.onIcon = onIcon;
        this.offIcon = offIcon;
        _variableName = variableName;
        GridInfo.AddChangeListener(_variableName, OnBoolChanged);
        State = GridInfo.GetVarAs<bool>(_variableName);
    }
    // variable pixel icon
    public PixelIcon(Vector2 position, List<string> variableIcons, string variableName, Color color, Vector2 size, float scale = 0.01f, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
        : base(anchor, position, scale, Vector2.Zero, color, "Monospace", variableIcons[0], alignment, SpriteType.TEXT)
    {
        _size = size;
        this.variableIcons = variableIcons;
        _variableName = variableName;
        GridInfo.AddChangeListener(_variableName, OnIntChanged);
        Index = GridInfo.GetVarAs<int>(_variableName);
    }
    void OnBoolChanged(string key, string value)
    {
        bool newState = GridInfo.GetVarAs<bool>(_variableName);
        if (newState != State)
        {
            State = newState;
        }
    }
    void OnIntChanged(string key, string value)
    {
        int newIndex = GridInfo.GetVarAs<int>(_variableName);
        if (newIndex != Index)
        {
            Index = newIndex;
        }
    }
    // bytes between 0 - 7
    // usage: PixelIcon.rgb(0, 0, 7);
    public static char rgb(byte r, byte g, byte b)
    {
        return (char)(0xE100 + (r << 6) + (g << 3) + b);
    }
    // remap an int from 0-255 to a byte from 0-7
    public static byte remap(int value)
    {
        if (value < 0) return 0;
        if (value > 255) return 7;
        return (byte)(value / 32);
    }
    //
    // draw functions
    //
    // add a pixel to the icon
    // ints between 0 - 255
    public void addPixelRGB(int r, int g, int b)
    {
        if (_addPosition.Y >= _size.Y) return;
        Data += rgb(remap(r), remap(g), remap(b));
        _addPosition.X += 1f;
        if (_addPosition.X >= _size.X)
        {
            _addPosition.X = 0f;
            _addPosition.Y += 1f;
            Data += "\n";
        }
    }
    // fill the icon with a color
    // ints between 0 - 255
    public void fillRGB(int r, int g, int b)
    {
        Data = "";
        for (int y = 0; y < _size.Y; y++)
        {
            for (int x = 0; x < _size.X; x++)
            {
                addPixelRGB(r, g, b);
            }
        }
    }
    public void fillRGB(Color color)
    {
        fillRGB(color.R, color.G, color.B);
    }
    // set a pixel to a color at a position
    // ints between 0 - 255
    public void setPixelRGB(int x, int y, int r, int g, int b)
    {
        if (x < 0 || x >= _size.X || y < 0 || y >= _size.Y) return;
        Data = Data.Remove((int)(y * (_size.X + 1) + x), 1);
        Data = Data.Insert((int)(y * (_size.X + 1) + x), rgb(remap(r), remap(g), remap(b)).ToString());
    }
    // draw a line from x1,y1 to x2,y2
    // ints between 0 - 255
    public void drawLineRGB(int x1, int y1, int x2, int y2, int r, int g, int b)
    {
        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);
        int sx = (x1 < x2) ? 1 : -1;
        int sy = (y1 < y2) ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            setPixelRGB(x1, y1, r, g, b);
            if ((x1 == x2) && (y1 == y2)) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x1 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y1 += sy;
            }
        }
    }
    public void drawLineRGB(Vector2 start, Vector2 end, Color color)
    {
        drawLineRGB((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color.R, color.G, color.B);
    }
    // draw a rectangle from x1,y1 to x2,y2
    // ints between 0 - 255
    public void drawRectRGB(int x1, int y1, int x2, int y2, int r, int g, int b)
    {
        drawLineRGB(x1, y1, x2, y1, r, g, b);
        drawLineRGB(x2, y1, x2, y2, r, g, b);
        drawLineRGB(x2, y2, x1, y2, r, g, b);
        drawLineRGB(x1, y2, x1, y1, r, g, b);
    }
    public void drawRectRGB(Vector2 start, Vector2 end, Color color)
    {
        drawRectRGB((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color.R, color.G, color.B);
    }
    // fill a rectangle from x1,y1 to x2,y2
    // ints between 0 - 255
    public void fillRectRGB(int x1, int y1, int x2, int y2, int r, int g, int b)
    {
        for (int y = y1; y <= y2; y++)
        {
            drawLineRGB(x1, y, x2, y, r, g, b);
        }
    }
    public void fillRectRGB(Vector2 start, Vector2 end, Color color)
    {
        fillRectRGB((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color.R, color.G, color.B);
    }
    // draw a circle at x,y with radius r
    // ints between 0 - 255
    public void drawCircleRGB(int x, int y, int r, int red, int green, int blue)
    {
        int f = 1 - r;
        int ddF_x = 1;
        int ddF_y = -2 * r;
        int x1 = 0;
        int y1 = r;
        setPixelRGB(x, y + r, red, green, blue);
        setPixelRGB(x, y - r, red, green, blue);
        setPixelRGB(x + r, y, red, green, blue);
        setPixelRGB(x - r, y, red, green, blue);
        while (x1 < y1)
        {
            if (f >= 0)
            {
                y1--;
                ddF_y += 2;
                f += ddF_y;
            }
            x1++;
            ddF_x += 2;
            f += ddF_x;
            setPixelRGB(x + x1, y + y1, red, green, blue);
            setPixelRGB(x - x1, y + y1, red, green, blue);
            setPixelRGB(x + x1, y - y1, red, green, blue);
            setPixelRGB(x - x1, y - y1, red, green, blue);
            setPixelRGB(x + y1, y + x1, red, green, blue);
            setPixelRGB(x - y1, y + x1, red, green, blue);
            setPixelRGB(x + y1, y - x1, red, green, blue);
            setPixelRGB(x - y1, y - x1, red, green, blue);
        }
    }
    // fill a circle at x,y with radius r
    // ints between 0 - 255
    public void fillCircleRGB(int x, int y, int r, int red, int green, int blue)
    {
        for (int y1 = -r; y1 <= r; y1++)
            for (int x1 = -r; x1 <= r; x1++)
                if (x1 * x1 + y1 * y1 <= r * r)
                    setPixelRGB(x + x1, y + y1, red, green, blue);
    }
}

//----------------------------------------------------------------------
// ScreenToggleIcon
//----------------------------------------------------------------------
public class ScreenToggleIcon
{
    string off_icon;
    string on_icon;
    bool _state = false;
    ScreenSprite sprite;
    string variableName;
    public bool State
    {
        get { return _state; }
        set
        {
            _state = value;
            if (_state)
            {
                sprite.Data = on_icon;
            }
            else
            {
                sprite.Data = off_icon;
            }
        }
    }
    public ScreenSprite Sprite
    {
        get { return sprite; }
    }
    public ScreenToggleIcon(string offIcon, string onIcon, string variableName, Vector2 position, Vector2 size, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
    {
        off_icon = offIcon;
        on_icon = onIcon;
        sprite = new ScreenSprite(anchor, position, 0f, size, Color.White, "", offIcon, alignment, SpriteType.TEXTURE);
        this.variableName = variableName;
        GridInfo.AddChangeListener(variableName, OnVariableChanged);
        State = GridInfo.GetVarAs<bool>(variableName, false);
    }
    public void OnVariableChanged(string name, string value)
    {
        bool newState = GridInfo.GetVarAs<bool>(variableName);
        if (newState != State)
        {
            State = newState;
        }
    }
}