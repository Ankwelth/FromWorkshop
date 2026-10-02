/*
 * Mining Camera Drone
 * -----------
 * 
 * This script is designed to be run on a drone with a camera for the
 * nanobot drill system mod. It will send back a matrix of points for
 * the mining area for the nanobot drill system mod to mine.
 * 
 * Icon Attribution:
 * https://www.flaticon.com/free-icons/construction-and-tools Construction-and-tools icons created by Smashicons - Flaticon
 * https://www.flaticon.com/free-icons/area Area icons created by Smashicons - Flaticon
 * https://www.flaticon.com/free-icons/box Box icons created by Good Ware - Flaticon
 * https://www.flaticon.com/free-icons/battery Battery icons created by rwgusev - Flaticon
 */

GridMatrix gridMatrix; // grid matrix class to make it easier to work with the mining area
HeadsUpDisplay hud; // screen class to make it easier to output text to the screen

public Program()
{
    GridInfo.Init("Mining Cam Drone",GridTerminalSystem,IGC,Me,Echo);
    if(Storage != "") GridInfo.Load(Storage);
    GridBlocks.RefreshGridBlocks(this);
    gridMatrix = new GridMatrix();
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    hud = new HeadsUpDisplay(GridBlocks.grid.hud);
    if (Me.CustomData == "")
    {
        Me.CustomData = "Name:" + Me.CubeGrid.CustomName;
        Me.CustomData += "\nType:Drone";
        Me.CustomData += "\nSoftware:Mining Camera Drone";
        Me.CustomData += "\n" + gridMatrix.save();
    }
    GridInfo.AddChangeBroadcaster("Mining System", "DrillsOn");
    GridInfo.AddChangeBroadcaster("Mining System", "ShowArea");
    GridInfo.AddChangeBroadcaster("Mining System", "AreaSize");
}

public void Save()
{
    //Storage = gridMatrix.save();
    Storage = GridInfo.Save();
}
public void Main(string args, UpdateType updateSource)
{
    GridBlocks.grid.TryConnect();
    GridInfo.CheckMessages();
    // if drone is being controlled, send mining area matrix
    if (GridBlocks.grid.remotePilotPresent && (updateSource == UpdateType.Update10 || updateSource == UpdateType.Update1 || updateSource == UpdateType.Update100))
    {
        IGC.SendBroadcastMessage<string>("MiningArea", gridMatrix.VectorListToString(gridMatrix.GetMiningArea(GridMatrix.matrix.drillCount)));
        //IGC.SendBroadcastMessage<float>("AreaSize", (float)GridMatrix.matrix.AreaSize * 2);
        GridInfo.SetVar("BatteryPercent", GridBlocks.grid.batteryPercent.ToString());
        GridInfo.SetVar("Velocity", Math.Round(GridBlocks.grid.velocity,2).ToString());
        if(GridBlocks.grid.isAutoDocking) GridInfo.SetVar("AutoPilot", "Auto Docking");
        else GridInfo.SetVar("AutoPilot", GridBlocks.grid.autoPilot ? "Auto Pilot" : "");
        hud.Draw();
    }
    // handle input from the user
    if (updateSource == UpdateType.Trigger || updateSource == UpdateType.Terminal || updateSource == UpdateType.Script)
    {
        HandleInput(args);
    }
}

// handle input from the user
public void HandleInput(string input)
{
    string action = hud.HandleInput(input);
    if (action == "") action = input.ToLower();
    if (action == "area")
    {
        // send the message to show or hide the drill area

        //GridMatrix.matrix.showMiningArea = !GridMatrix.matrix.showMiningArea;
        //IGC.SendBroadcastMessage<bool>("ShowMiningArea", GridMatrix.matrix.showMiningArea);
    }
    else if (action == "launch")
    {
        GridBlocks.grid.LaunchFromDock();
        HeadsUpDisplay.Undocked();
    }
    else if (action == "return")
    {
        GridBlocks.grid.AutoDock();
    }
    else if (action == "dock")
    {
        GridBlocks.grid.ConnectToDock();
    }
    else if (action == "drills")
    {
        GridMatrix.matrix.drillsOn = !GridMatrix.matrix.drillsOn;
        IGC.SendBroadcastMessage<bool>("DrillsOn", GridMatrix.matrix.drillsOn);
    }
}

//----------------------------------------------------------------------
// utility class for parsing CustomData variables
// VarName:value
//----------------------------------------------------------------------
public class CustomDataVars
{
    public static List<string> GetListVar(string key, string data)
    {
        List<string> vars = new List<string>();
        string[] lines = data.Split('\n');
        foreach (string line in lines)
        {
            if (line.StartsWith(key))
            {
                string[] var = line.Split(':');
                if (var.Length == 2)
                {
                    vars.Add(var[1]);
                }
            }
        }

        return vars;
    }
    public static string GetVar(string key, string data)
    {
        string[] lines = data.Split('\n');
        foreach (string line in lines)
        {
            if (line.StartsWith(key))
            {
                string[] var = line.Split(':');
                if (var.Length == 2)
                {
                    return var[1];
                }
            }
        }

        return "";
    }
    public static Dictionary<string, string> Parse(string data)
    {
        Dictionary<string, string> vars = new Dictionary<string, string>();
        string[] lines = data.Split('\n');
        foreach (string line in lines)
        {
            string[] var = line.Split(':');
            if (var.Length == 2)
            {
                if (vars.ContainsKey(var[0])) vars[var[0]] += ";" + var[1];
                else vars.Add(var[0], var[1]);
            }
        }
        return vars;
    }
}

//----------------------------------------------------------------------//
//          grid blocks class to hold the blocks for the grid           //
//----------------------------------------------------------------------//
// This class is used to hold the blocks for the grid. It is used to    //
// make it easier to access the blocks. and do things like turn off     //
// all the thrusters at once.                                           //
//----------------------------------------------------------------------//
public class GridBlocks
{
    public static GridBlocks grid;

    public static IMyGridTerminalSystem GridTerminalSystem; // so it can be globally available
    public static IMyIntergridCommunicationSystem IGC; // so it can be globally available
    public static IMyProgrammableBlock Me; // so it can be globally available... lol
    public double velocity
    {
        get
        {
            if (remote == null) return 0;
            return remote.GetShipSpeed();
        }
    }
    // camera blocks
    public List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
    public IMyCameraBlock flightCamera
    {
        get
        {
            foreach (IMyCameraBlock camera in cameras)
            {
                if (camera.CustomName.ToLower().Contains("flight")) return camera;
            }
            return null;
        }
    }
    // text panel blocks
    public List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
    public IMyTextSurface hud
    {
        get
        {
            foreach (IMyTextPanel textPanel in textPanels)
            {
                if (textPanel.CustomName.ToLower().Contains("hud")) return textPanel as IMyTextSurface;
            }
            return null;
        }
    }
    public IMyTextSurface progScreen;
    // remote control block
    public IMyRemoteControl remote;
    public bool remotePilotPresent
    {
        get
        {
            if (remote == null) return false;
            return remote.IsUnderControl;
        }
    }  // flight controller block
    public IMyFlightMovementBlock flightController;
    public bool autoPilot
    {
        get
        {
            return flightController.IsAutoPilotEnabled;
        }
        set
        {
            if (value) flightController.ApplyAction("ActivateBehavior_On");
            else flightController.ApplyAction("ActivateBehavior_Off");
        }
    }
    // path recorder blocks
    List<IMyPathRecorderBlock> pathRecorders = new List<IMyPathRecorderBlock>();
    public IMyPathRecorderBlock dockingPath
    {
        get
        {
            foreach (IMyPathRecorderBlock pathRecorder in pathRecorders)
            {
                if (pathRecorder.CustomName.ToLower().Contains("docking")) return pathRecorder;
            }
            return null;
        }
    }
    private bool dockingPathRunning = false;
    public void StartDockingPath()
    {
        if (dockingPath == null) return;
        autoPilot = true;
        dockingPathRunning = true;
        IMyPathRecorderBlock dp = dockingPath;
        dp.ApplyAction("ActivateBehavior_On");
        dp.ApplyAction("ID_PLAY_CHECKBOX");
        Sandbox.ModAPI.Interfaces.ITerminalProperty<bool> playing = dp.GetProperty("ID_PLAY_CHECKBOX") as Sandbox.ModAPI.Interfaces.ITerminalProperty<bool>;
        if (!playing.GetValue(dp)) dp.ApplyAction("ID_PLAY_CHECKBOX");
    }
    // try to connect while running the docking path
    public void TryConnect()
    {
        if (!dockingPathRunning) return;
        ConnectToDock();
    }
    public bool isAutoDocking
    {
        get
        {
            return (dockingPathRunning && autoPilot);
        }
    }
    // thruster blocks
    public List<IMyThrust> thrusters = new List<IMyThrust>();
    public bool thrustersEnabled
    {
        get
        {
            foreach (IMyThrust thruster in thrusters)
            {
                if (thruster.Enabled) return true;
            }
            return false;
        }
        set
        {
            foreach (IMyThrust thruster in thrusters)
            {
                thruster.Enabled = value;
            }
        }
    }
    // gyro blocks
    public List<IMyGyro> gyros = new List<IMyGyro>();
    public bool gyrosEnabled
    {
        get
        {
            foreach (IMyGyro gyro in gyros)
            {
                if (gyro.Enabled) return true;
            }
            return false;
        }
        set
        {
            foreach (IMyGyro gyro in gyros)
            {
                gyro.Enabled = value;
            }
        }
    }
    // battery blocks
    public List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
    public float batteryCharge
    {
        get
        {
            float result = 0;
            foreach (IMyBatteryBlock battery in batteries)
            {
                result += battery.CurrentStoredPower;
            }
            return result;
        }
    }
    public float batteryPercent
    {
        get
        {
            float current = 0;
            float max = 0;
            foreach (IMyBatteryBlock battery in batteries)
            {
                current += battery.CurrentStoredPower;
                max += battery.MaxStoredPower;
            }
            return current / max;
        }
    }
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
    public bool headlightsEnabled
    {
        get
        {
            foreach (IMyLightingBlock light in headlights)
            {
                if (light.Enabled) return true;
            }
            return false;
        }
        set
        {
            foreach (IMyLightingBlock light in headlights)
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
    // constructor
    public GridBlocks(Program program)
    {
        GridTerminalSystem = program.GridTerminalSystem;
        IGC = program.IGC;
        Me = program.Me;
        // get all the blocks
        progScreen = program.Me.GetSurface(0);
        program.GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(cameras, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(textPanels, b => b.IsSameConstructAs(program.Me));
        List<IMyRemoteControl> remotes = new List<IMyRemoteControl>();
        program.GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(remotes, b => b.IsSameConstructAs(program.Me));
        if (remotes.Count > 0) remote = remotes[0];
        List<IMyFlightMovementBlock> flightControllers = new List<IMyFlightMovementBlock>();
        program.GridTerminalSystem.GetBlocksOfType<IMyFlightMovementBlock>(flightControllers, b => b.IsSameConstructAs(program.Me));
        if (flightControllers.Count > 0) flightController = flightControllers[0];
        program.GridTerminalSystem.GetBlocksOfType<IMyPathRecorderBlock>(pathRecorders, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(lights, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, b => b.IsSameConstructAs(program.Me));
        program.GridTerminalSystem.GetBlocksOfType<IMyOreDetector>(oreDetectors, b => b.IsSameConstructAs(program.Me));
        // set up the grid
        grid = this;
    }
    public static void RefreshGridBlocks(Program program)
    {
        grid = new GridBlocks(program);
    }
    // connect the drone to the docking connector and disable drone systems
    public void ConnectToDock()
    {
        if (connectors.Count == 0) return;
        if (connected) return;
        foreach (IMyShipConnector connector in connectors)
        {
            connector.Connect();
        }
        if (connected)
        {
            thrustersEnabled = false;
            gyrosEnabled = false;
            flightLightsEnabled = false;
            headlightsEnabled = false;
            landingLightsEnabled = true;
            autoPilot = false;
            oreDetectorsEnabled = false;
            SetBatteriesMode(ChargeMode.Recharge);
            dockingPathRunning = false;
            HeadsUpDisplay.Docked();
        }
    }
    // auto dock the drone
    public void AutoDock()
    {
        if (connected) return;
        if (dockingPath == null) return;
        StartDockingPath();
    }
    // launch drone from docking connector
    public void LaunchFromDock()
    {
        if (!connected) return;
        SetBatteriesMode(ChargeMode.Auto);
        foreach (IMyShipConnector connector in connectors)
        {
            connector.Disconnect();
        }
        thrustersEnabled = true;
        gyrosEnabled = true;
        flightLightsEnabled = true;
        headlightsEnabled = true;
        landingLightsEnabled = false;
        oreDetectorsEnabled = true;
        autoPilot = false;
        IGC.SendBroadcastMessage<string>("DroneOnline", "Mining Camera Drone");
    }
    // set batteries charge mode to recharge
    public void SetBatteriesMode(ChargeMode mode)
    {
        foreach (IMyBatteryBlock battery in batteries)
        {
            battery.ChargeMode = mode;
        }
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
        Me.CustomName = "Program: " + ProgramName + " @" + IGC.Me.ToString();
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
            if (GridVars.ContainsKey(message.Tag)) SetVar(message.Tag, message.As<string>());
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

//----------------------------------------------------------------------
// grid matrix class to make it easier to work with the mining area
//----------------------------------------------------------------------
public class GridMatrix
{
    public static GridMatrix matrix;
    public int ForwardOffset = 100; // how many blocks forward to build mining area matrix
    public int HorizontalOffset = 4; // how many blocks to the left and right to build mining area matrix
    public int VerticalOffset = 0; // how many blocks up and down to build mining area matrix
    public int AreaSize = 25; // how many blocks to the left and right to build mining area matrix
    public int drillCount = 9; // how many drills are on the drone
    public bool showMiningArea = false; // show the mining area on the hud
    public bool drillsOn = false; // are the drills on
    public GridMatrix()
    {
        matrix = this;
        ForwardOffset = GridInfo.GetVarAs<int>("ForwardOffset", ForwardOffset);
        HorizontalOffset = GridInfo.GetVarAs<int>("HorizontalOffset", HorizontalOffset);
        VerticalOffset = GridInfo.GetVarAs<int>("VerticalOffset", VerticalOffset);
        AreaSize = GridInfo.GetVarAs<int>("AreaSize", AreaSize);
        drillCount = GridInfo.GetVarAs<int>("DrillCount", drillCount);
        showMiningArea = GridInfo.GetVarAs<bool>("ShowArea", showMiningArea);
        drillsOn = GridInfo.GetVarAs<bool>("DrillsOn", drillsOn);
        GridInfo.AddChangeListener("ForwardOffset", OnVarUpdated);
    }
    void OnVarUpdated(string key, string value)
    {
        if (key == "ForwardOffset") ForwardOffset = GridInfo.GetVarAs<int>("ForwardOffset");
        else if (key == "HorizontalOffset") HorizontalOffset = GridInfo.GetVarAs<int>("HorizontalOffset");
        else if (key == "VerticalOffset") VerticalOffset = GridInfo.GetVarAs<int>("VerticalOffset");
        else if (key == "AreaSize") AreaSize = GridInfo.GetVarAs<int>("AreaSize");
        else if (key == "DrillCount") drillCount = GridInfo.GetVarAs<int>("DrillCount");
        else if (key == "ShowArea") showMiningArea = GridInfo.GetVarAs<bool>("ShowArea");
        else if (key == "DrillsOn") drillsOn = GridInfo.GetVarAs<bool>("DrillsOn");
    }
    // save properties to string
    public string save()
    {
        string result = "";
        result += "ForwardOffset:" + ForwardOffset.ToString() + "\n";
        result += "HorizontalOffset:" + HorizontalOffset.ToString() + "\n";
        result += "VerticalOffset:" + VerticalOffset.ToString() + "\n";
        result += "AreaSize:" + AreaSize.ToString() + "\n";
        result += "DrillCount:" + drillCount.ToString() + "\n";
        return result;
    }
    //----------------------------------------------------------------------
    // get a list of points for the mining area using the camera and forward
    // offset and area size
    //----------------------------------------------------------------------
    public List<Vector3D> GetMiningArea(int count = 1)
    {
        List<Vector3D> miningArea = new List<Vector3D>();
        // nice prebuilt mining area matrixes for 1, 5, 7, and 8 drills
        if (count == 1)
        {
            miningArea.Add(GetWorldPosition(new Vector3D(HorizontalOffset, VerticalOffset, -1 * ForwardOffset))); // center
            return miningArea;
        }
        else if (count == 5)
        {
            AddCrossPlain(miningArea);
            return miningArea;
        }
        else if (count == 7)
        {
            AddCrossGrid(miningArea);
            return miningArea;
        }
        else if (count == 8)
        {
            AddBoxGrid(miningArea);
            return miningArea;
        }
        // build out the mining area matrix
        miningArea.Add(GetWorldPosition(new Vector3D(HorizontalOffset, VerticalOffset, -1 * ForwardOffset))); // center
        if (count-- <= 0) return miningArea;
        // add box around center
        AddBox(miningArea); count -= 4;
        if (count <= 0) return miningArea;
        AddCross(miningArea); count -= 4;
        if (count <= 0) return miningArea;
        // add front plain
        miningArea.Add(GetWorldPosition(new Vector3D(HorizontalOffset, VerticalOffset, (AreaSize * 1) - ForwardOffset))); // center
        if (count-- <= 0) return miningArea;
        AddBox(miningArea, 1); count -= 4;
        if (count <= 0) return miningArea;
        AddCross(miningArea, 1); count -= 4;
        if (count <= 0) return miningArea;
        // add back plain
        miningArea.Add(GetWorldPosition(new Vector3D(HorizontalOffset, VerticalOffset, (AreaSize * -1) - ForwardOffset))); // center
        if (count-- <= 0) return miningArea;
        AddBox(miningArea, -1); count -= 4;
        if (count <= 0) return miningArea;
        AddCross(miningArea, -1); count -= 4;
        return miningArea;
    }
    // Add a box grid for 8 drills
    public void AddBoxGrid(List<Vector3D> miningArea)
    {
        // box grid
        AddBox(miningArea, 1); // front
        AddBox(miningArea, -1); // back
    }
    // add a grid of crosses for 7 drills
    public void AddCrossGrid(List<Vector3D> miningArea)
    {
        AddCrossPlain(miningArea); // center
        miningArea.Add(GetWorldPosition(new Vector3D(HorizontalOffset, VerticalOffset, (AreaSize * 1) - ForwardOffset))); // front
        miningArea.Add(GetWorldPosition(new Vector3D(HorizontalOffset, VerticalOffset, (AreaSize * -1) - ForwardOffset))); // back
    }
    //  add cross plain for 5 drills
    public void AddCrossPlain(List<Vector3D> miningArea)
    {
        // cross plain
        miningArea.Add(GetWorldPosition(new Vector3D(HorizontalOffset, VerticalOffset, -1 * ForwardOffset))); // center
        AddCross(miningArea);
    }
    // add the box around the camera
    public void AddBox(List<Vector3D> miningArea, int offset = 0)
    {
        // box around camera
        miningArea.Add(GetWorldPosition(new Vector3D(AreaSize + HorizontalOffset, AreaSize + VerticalOffset, (AreaSize * offset) - ForwardOffset))); // top right
        miningArea.Add(GetWorldPosition(new Vector3D(-1 * AreaSize + HorizontalOffset, AreaSize + VerticalOffset, (AreaSize * offset) - ForwardOffset))); // top left
        miningArea.Add(GetWorldPosition(new Vector3D(AreaSize + HorizontalOffset, -1 * AreaSize + VerticalOffset, (AreaSize * offset) - ForwardOffset))); // bottom right
        miningArea.Add(GetWorldPosition(new Vector3D(-1 * AreaSize + HorizontalOffset, -1 * AreaSize + VerticalOffset, (AreaSize * offset) - ForwardOffset))); // bottom left
    }
    // add the cross in front of the camera
    public void AddCross(List<Vector3D> miningArea, int offset = 0)
    {
        // cross in front of camera
        miningArea.Add(GetWorldPosition(new Vector3D(HorizontalOffset, AreaSize + VerticalOffset, (AreaSize * offset) - ForwardOffset))); // up
        miningArea.Add(GetWorldPosition(new Vector3D(HorizontalOffset, -1 * AreaSize + VerticalOffset, (AreaSize * offset) - ForwardOffset))); // down
        miningArea.Add(GetWorldPosition(new Vector3D(AreaSize + HorizontalOffset, VerticalOffset, (AreaSize * offset) - ForwardOffset))); // right
        miningArea.Add(GetWorldPosition(new Vector3D(-1 * AreaSize + HorizontalOffset, VerticalOffset, (AreaSize * offset) - ForwardOffset))); // left
    }
    // get a world position from camera position
    public Vector3D GetWorldPosition(Vector3D localPosition)
    {
        return Vector3D.Transform(localPosition, GridBlocks.grid.flightCamera.WorldMatrix);
    }

    // convert a list of vectors to a string
    public string VectorListToString(List<Vector3D> vectors)
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
}

//----------------------------------------------------------------------
// hud display with sections and hot bar stuff
//----------------------------------------------------------------------
public class HeadsUpDisplay : Screen
{
    static HeadsUpDisplay instance;
    ScreenMenu _optionsMenu;
    ScreenActionBar _actionBar;
    string defaultActions = "Area Drills Options  Return";
    string dockedActions = "Launch";
    string drillIcon = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
    string areaIcon = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
    string cargoIcon = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
    string batteryIcon = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
    public HeadsUpDisplay(IMyTextSurface drawingSurface)
        :base(drawingSurface)
    {
        // finsish setting up screen for hud stuff
        drawingSurface.BackgroundColor = Color.Black;
        bottomPadding = 20f;
        leftPadding = 10f;
        rightPadding = 10f;
        // add the action bar
        if(GridBlocks.grid.connected)
        {
            _actionBar = AddActionBar(5, dockedActions);
        }
        else
        {
            _actionBar = AddActionBar(5, defaultActions);
        }
        // add options menu
        _optionsMenu = new ScreenMenu("Mining System",200f,_actionBar);
        _optionsMenu.AddVariable("Forward Offset", "ForwardOffset", "100");
        _optionsMenu.AddVariable("Horizontal Offset", "HorizontalOffset", "0");
        _optionsMenu.AddVariable("Vertical Offset", "VerticalOffset", "0");
        _optionsMenu.AddVariable("Area Size", "AreaSize", "25");
        _optionsMenu.AddVariable("Show Area", "ShowArea", "false");
        _optionsMenu.AddVariable("Drills On", "DrillsOn", "false");
        _optionsMenu.AddVariable("Drill Count", "DrillCount", "9");
        AddMenu(_optionsMenu);
        _optionsMenu.Visible = false;
        // add pixel icons for ShowArea and DrillsOn
        AddTogglePixelIcon(new Vector2(-10, 0), "", drillIcon, "DrillsOn", Color.White, new Vector2(64, 64), 0.02f, ScreenSprite.ScreenSpriteAnchor.CenterRight, TextAlignment.RIGHT);
        AddTogglePixelIcon(new Vector2(-10, -50), "", areaIcon, "ShowArea", Color.White, new Vector2(64, 64), 0.02f, ScreenSprite.ScreenSpriteAnchor.CenterRight, TextAlignment.RIGHT);
        // add cargo fill percentage bar
        AddPixelIcon(new Vector2(-100,30), cargoIcon, Color.White, new Vector2(64, 64), 0.01f, ScreenSprite.ScreenSpriteAnchor.TopCenter, TextAlignment.RIGHT);
        AddPercentageBar("CargoPercent", new Vector2(-95,40), new Vector2(200, 10), Color.White, Color.Tan, Color.Green, Color.Red, new Color(Color.DarkGoldenrod,0.5f), 0f, ScreenSprite.ScreenSpriteAnchor.TopCenter);
        // add battery fill percentage bar
        AddPixelIcon(new Vector2(-95, 50), batteryIcon, Color.White, new Vector2(64, 64), 0.005f, ScreenSprite.ScreenSpriteAnchor.TopCenter, TextAlignment.RIGHT);
        AddPercentageBar("BatteryPercent", new Vector2(-90, 55), new Vector2(190, 5), Color.White, Color.LightGreen, Color.Red, Color.Green, new Color(Color.DarkOliveGreen, 0.5f), 0f, ScreenSprite.ScreenSpriteAnchor.TopCenter);
        // add top ores icons
        ScreenIconSelect topOre = AddIconSelect("TopOre0", new Vector2(-32, 40), "", 0f, new Vector2(54, 54), Color.White, TextAlignment.RIGHT, ScreenSprite.ScreenSpriteAnchor.TopRight);
        topOre.AddIcon("None", "");
        string prefix = hasColorfulIcons ? "ColorfulIcons_Ore/" : "MyObjectBuilder_Ore/";
        topOre.AddIcon("Iron", prefix+"Iron");
        topOre.AddIcon("Nickel", prefix+"Nickel");
        topOre.AddIcon("Cobalt", prefix + "Cobalt");
        topOre.AddIcon("Magnesium", prefix + "Magnesium");
        topOre.AddIcon("Silicon", prefix + "Silicon");
        topOre.AddIcon("Silver", prefix + "Silver");
        topOre.AddIcon("Gold", prefix + "Gold");
        topOre.AddIcon("Platinum", prefix + "Platinum");
        topOre.AddIcon("Uranium", "prefix+\"Uranium");
        topOre.AddIcon("Ice", prefix + "Ice");
        topOre.AddIcon("Stone", prefix + "Stone");
        topOre.AddIcon("Scrap", prefix + "Scrap");
        Dictionary<string, string> oreIcons = topOre.Icons;
        //GridInfo.Echo("oreIcons.Count: " + oreIcons.Count);
        // add 5 more top ore icons
        AddIconSelect("TopOre1", new Vector2(-90, 30), "", 0f, new Vector2(32, 32), Color.White, TextAlignment.RIGHT, ScreenSprite.ScreenSpriteAnchor.TopRight).AddIcons(oreIcons);
        AddIconSelect("TopOre2", new Vector2(-85, 60), "", 0f, new Vector2(28, 28), Color.White, TextAlignment.RIGHT, ScreenSprite.ScreenSpriteAnchor.TopRight).AddIcons(oreIcons);
        AddIconSelect("TopOre3", new Vector2(-65, 80), "", 0f, new Vector2(24, 24), Color.White, TextAlignment.RIGHT, ScreenSprite.ScreenSpriteAnchor.TopRight).AddIcons(oreIcons);
        AddIconSelect("TopOre4", new Vector2(-44, 82), "", 0f, new Vector2(20, 20), Color.White, TextAlignment.RIGHT, ScreenSprite.ScreenSpriteAnchor.TopRight).AddIcons(oreIcons);
        AddIconSelect("TopOre5", new Vector2(-26, 76), "", 0f, new Vector2(16, 16), Color.White, TextAlignment.RIGHT, ScreenSprite.ScreenSpriteAnchor.TopRight).AddIcons(oreIcons);
        GridInfo.AddChangeListener("topOres", OnTopOresUpdated);
        AddTextVarSprite("Velocity", new Vector2(0, 60), 0.75f, Color.White, "{0} m/s", TextAlignment.CENTER, ScreenSprite.ScreenSpriteAnchor.TopCenter);
        // auto pilot notice
        AddTextVarSprite("AutoPilot", new Vector2(0, 80), 0.75f, Color.White, "{0}", TextAlignment.CENTER, ScreenSprite.ScreenSpriteAnchor.TopCenter);
        instance = this;
    }
    void OnTopOresUpdated(string key, string value)
    {
        //GridInfo.Echo("OnTopOresUpdated: " + key + " = " + value);
        if(key == "topOres")
        {
            string[] topOres = value.Split(',');
            for(int i = 0; i < 6; i++)
            {
                if(i < topOres.Length)
                {
                    GridInfo.SetVar("TopOre" + i, topOres[i].Trim());
                }
                else
                {
                    GridInfo.SetVar("TopOre" + i, "None");
                }
            }
        }
    }
    // handle button presses
    public string HandleInput(string argument)
    {
        string action = _optionsMenu.HandleInput(argument);
        if (action == "area")
        {
            GridInfo.SetVar("ShowArea", GridInfo.GetVarAs<bool>("ShowArea") ? "false" : "true");
        }
        else if (action == "drills")
        {
            GridInfo.SetVar("DrillsOn", GridInfo.GetVarAs<bool>("DrillsOn") ? "false" : "true");
        }
        else if(action == "options")
        {
            _optionsMenu.Visible = true;
            _actionBar.SetActions(_optionsMenu.menuScrollActions);
        }
        else if(action == "back")
        {
            _optionsMenu.Visible = false;
            if(GridBlocks.grid.connected)
            {
                _actionBar.SetActions(dockedActions);
            }
            else
            {
                _actionBar.SetActions(defaultActions);
            }
        }
        else
        {
            return action;
        }
        return "";
    }
    public static void Docked()
    {
        instance._actionBar.SetActions(instance.dockedActions);
    }
    public static void Undocked()
    {
        if(instance._optionsMenu.Visible)
        {
            instance._actionBar.SetActions(instance._optionsMenu.menuScrollActions);
        }
        else
        {
            instance._actionBar.SetActions(instance.defaultActions);
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
}

//----------------------------------------------------------------------
// Screen - encapsulates a drawing surface
//----------------------------------------------------------------------
public class Screen
{
    private IMyTextSurface _drawingSurface;
    private RectangleF _viewport;
    private List<ScreenSprite> _sprites = new List<ScreenSprite>();
    public float bottomPadding = 0f;
    public float topPadding = 0f;
    public float leftPadding = 0f;
    public float rightPadding = 0f;
    public bool hasColorfulIcons
    {
        get
        {
            List<string> sprites = new List<string>();
            _drawingSurface.GetSprites(sprites);
            return sprites.Contains("ColorfulIcons_Ore/Iron");
        }
    }
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
        Vector2 actionWidth = new Vector2((_viewport.Width-leftPadding-rightPadding) / actionCount, 0);
        Vector2 actionPosition = new Vector2((actionWidth.X / 2f) + leftPadding, -30 * fontSize-bottomPadding);
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
    // add a variable text sprite
    public ScreenTextVarSprite AddTextVarSprite(string varName, Vector2 position, float rotation, Color color, string format, TextAlignment alignment, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft)
    {
        ScreenTextVarSprite sprite = new ScreenTextVarSprite(varName, position, rotation, new Vector2(0, 0), color, format, alignment, anchor);
        _sprites.Add(sprite);
        return sprite;
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
    public ScreenPercentageBar AddPercentageBar(string varName, Vector2 position, Vector2 size, Color borderColor, Color barColor, Color barEmpty, Color barFull, Color backColor, float percent, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.BottomCenter)
    {
        ScreenPercentageBar percentageBar = new ScreenPercentageBar(varName, position, size, borderColor, barColor,barEmpty, barFull, backColor, percent, anchor);
        percentageBar.AddSprites(ref _sprites);
        return percentageBar;
    }
    public ScreenIconSelect AddIconSelect(string varName, Vector2 position, string icon, float rotation, Vector2 size, Color color, TextAlignment alignment, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.BottomCenter)
    {
        ScreenIconSelect iconSelect = new ScreenIconSelect(varName, position, icon, rotation, size, color, alignment, anchor);
        _sprites.Add(iconSelect);
        return iconSelect;
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
// ScreenIconSelect
//----------------------------------------------------------------------
public class ScreenIconSelect : ScreenSprite
{
    Dictionary<string,string> icons = new Dictionary<string,string>();
    public Dictionary<string,string> Icons { get { return icons; } }
    string varName;
    string selected = "";
    public string selectedIcon
    {
        get { return selected; }
        set
        {
            if (icons.ContainsKey(value))
            {
                Data = icons[value];
                selected = value;
            }
        }
    }
    public ScreenIconSelect(string varName, Vector2 position, string icon, float rotation, Vector2 size, Color color, TextAlignment alignment, ScreenSpriteAnchor anchor = ScreenSpriteAnchor.BottomCenter) : base(anchor, position, rotation, size, color, "", icon, alignment, SpriteType.TEXTURE)
    {
        this.varName = varName;
        GridInfo.AddChangeListener(varName, OnVarUpdated);
    }
    public void AddIcon(string name, string icon)
    {
        icons.Add(name,icon);
    }
    public void AddIcons(Dictionary<string,string> icons)
    {
        foreach(var icon in icons)
        {
            GridInfo.Echo("Icon: " + icon.Key + " = " + icon.Value);
            AddIcon(icon.Key,icon.Value);
        }
    }
    void OnVarUpdated(string name, string value)
    {

        if (name == varName && icons.ContainsKey(value)) selectedIcon = value;
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
    Color barEmpty;
    Color barColor;
    Vector2 Size;

    public ScreenPercentageBar(string varName, Vector2 position, Vector2 size, Color borderColor, Color barColor,Color barEmpty, Color barFull, Color backColor, float percent, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.BottomCenter, float borderSize = 1f)
    {
        this.percent = GridInfo.GetVarAs<float>(varName, percent);
        this.barFull = barFull;
        this.barColor = barColor;
        this.barEmpty = barEmpty;
        Size = size;
        bar = new ScreenSprite(anchor, position, 0, new Vector2(size.X * percent, size.Y), barColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        back = new ScreenSprite(anchor, position, 0, size, backColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        border_top = new ScreenSprite(anchor, new Vector2(position.X, position.Y - (size.Y / 2)), 0, new Vector2(size.X, borderSize), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        border_bottom = new ScreenSprite(anchor, new Vector2(position.X, position.Y + (size.Y / 2)), 0, new Vector2(size.X, borderSize), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        border_left = new ScreenSprite(anchor, position, 0, new Vector2(borderSize, size.Y), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
        border_right = new ScreenSprite(anchor, new Vector2(position.X + size.X, position.Y), 0, new Vector2(borderSize, size.Y), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
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
            if (percent > 0.75f)
            {
                bar.Color = barFull;
            }
            else if (percent < 0.25f)
            {
                bar.Color = barEmpty;
            }
            else
            {
                bar.Color = barColor;
            }
        }
    }
}

public class ScreenTextVarSprite : ScreenSprite
{
    string varName;
    string format;
    public ScreenTextVarSprite(string varName, Vector2 position, float rotation, Vector2 size, Color color, string format, TextAlignment alignment, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft) : base(anchor, position, rotation, size, color, "White", "", alignment, SpriteType.TEXT)
    {
        this.varName = varName;
        this.format = format;
        GridInfo.AddChangeListener(varName, OnVarUpdated);
    }
    public void OnVarUpdated(string name, string value)
    {
        if (name == varName)
        {
            Data = string.Format(format, value);
        }
    }
}