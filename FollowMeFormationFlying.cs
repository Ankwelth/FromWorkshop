public SortedList<string, SortedList<string, double>> settingListDoubles = new SortedList<string, SortedList<string, double>>
{
    { "Client", new SortedList<string, double>
        {
            { "Relative Right", 0 }, { "Relative Up", 0}, { "Relative Forward", 0 },
            { "Prediction Multiplier", 6 }, { "Waypoint Update Distance", 25 },
            { "Weapon Accuracy", 15 }
        }
    },
    { "Host", new SortedList<string, double>
        {
            { "Formation Offset", 100 }, { "Length Multiplier", 2.5 }, { "Broadcast Frequency", 2.5 }
        }
    },
    { "Universal", new SortedList<string, double>
        {
            { "Overheat Average", 0.065 }, { "Action Usage", 0.05 }, { "Runtime Limit", 0.99 }
        }
    }
};

public SortedList<string, SortedList<string, bool>> settingListBools = new SortedList<string, SortedList<string, bool>>
{
    { "Client", new SortedList<string, bool>
        {
            { "Queue Waypoints", false }, { "Use Collision Avoidance", true },
            { "Dampen Between Waypoints", false }, { "Always Dampen In Gravity", true }
        }
    },
    { "Universal", new SortedList<string, bool>
        {
            { "Host", false }, { "Client", false }, { "Active", false }
        }
    }
};

public SortedList<string, SortedList<string, string>> settingListStrings = new SortedList<string, SortedList<string, string>>
{
    { "Universal", new SortedList<string, string>
        {
            { "Broadcast Password", "default" }, { "Remote Control Filter", "" }
        }
    }
};

public SortedList<string, SortedList<string, int>> settingListInts = new SortedList<string, SortedList<string, int>>
{
    { "Host", new SortedList<string, int>
        {
            { "Formation", 0 }, { "Formation Generation", 50 }
        }
    }
};

public Dictionary<string, List<string>> settingExplanations = new Dictionary<string, List<string>>
{
    { "Formation", new List<string> { "0: Horizontal Diamond", "1: Vertical Diamond", "2: Forward Wall", "3: Backward Wall", "4: Top Wall", "5: Bottom Wall", "6: Cube", "7: Sphere" } },
    { "Overheat Average", new List<string> { "Forcibly limits average runtime with idle ticks", { "While the average runtime exceeds this value, the script will become inactive" } } },
    { "Action Usage", new List<string> { "Percentage of actions available to script, 0.5 = 50%" } },
    { "Broadcast Frequency", new List<string> { "Host position broadcast frequency in seconds" } }
};

public IMyBroadcastListener
    answeringMachineLocation,
    answeringMachineVector,
    answeringMachineUp,
    answeringMachineRight,
    answeringMachineForward,
    anseringMachineCommand;

public Vector3D
hostPosition = Vector3D.Zero,
lastVector = Vector3D.Zero,
lastUp = Vector3D.Zero,
lastRight = Vector3D.Zero,
lastForward = Vector3D.Zero,
currentVector = Vector3D.Zero,
currentPosition = Vector3D.Zero,
lastDestination = Vector3D.Zero,
aimVector = Vector3D.Zero;

public MatrixD matrix = new MatrixD();

public bool
    host = false, client = false,
    updated = false, active = false,
    waypointQueue = false,
    reset = false,
    correctScript = true, correctVersion = true,
    autoRegistered = false,
    useCollisionAvoidance = true, dampenBetweenWaypoints = true,
    dampenersAlwayOnInGravity = true, inGravity = false,
    saving = false, loading = false, generatingFormation = false,
    commandDisplay = false, assignmentDisplay = false,
    hasTarget = false, customDisplay = false, presetDisplay = false,
    arrangingFormation = false;

public IMyRemoteControl genRemote;

public List<IMyGyro> gyroList = new List<IMyGyro>();

public List<IMyUserControllableGun> gunList = new List<IMyUserControllableGun>();

public List<string>
    commandQueue = new List<string>(),
    currentEcho = new List<string>(),
    messageEcho = new List<string>();

public double
relativeRight = 0, relativeUp = 0,
relativeForward = 0, broadcastFrequency = 2.5,
shipLength = 0, predictionMultiplier = 6,
version = 1.02, formationDistanceOffset = 100,
lastRuntime = 0, averageRuntime = 0,
torchAverage = 0, tickWeight = 0.005, overheatAverage = 0.2,
actionLimiterMultiplier = 0.25, runTimeLimiter = 0.99,
waypointUpdateDistance = 25, lengthMultiplier = 2.5,
targetAccuracy = 0, requiredAccuracy = 15;

public DateTime
tickStartTime = DateTime.Now;

public string
broadcastSalt = "default",
broadcastLocation = "location",
broadcastVector = "vector",
broadcastUp = "up",
broadcastRight = "right",
broadcastForward = "forward",
broadcastCommand = "command",
myUniqueID = "",
settingBackup = "",
script = "Follow Me Formation Flying",
remoteControlKey = "";

//X = right, Y = Up, Z = Forward

public int
    formationMode = 0, //0 = horizontal diamond, 1 = vertical diamond, 2 = forward wall, 3 = backward wall, 4 = top wall, 5 = bottom wall, 6 = cube, 7 = sphere
    overheatTicks = 0, echoTicks = 7, generatedPositions = 50,
    transmissions = 0, receivals = 0, targets = 0;

public long
    currentTurretTargetID = -1;

public IMyLargeTurretBase currentTargetingTurret;

public List<Vector3D>
    customPositions = new List<Vector3D>(),
    presetPositions = new List<Vector3D>();

public HashSet<string>
    clearedIDs = new HashSet<string>();

public HashSet<Vector3D>
    customPositionSet = new HashSet<Vector3D>(),
    presetPositionSet = new HashSet<Vector3D>();

public Dictionary<Vector3D, string>
    occupiedPresetDictionary = new Dictionary<Vector3D, string>(),
    occupiedCustomDictionary = new Dictionary<Vector3D, string>();

public Dictionary<string, Vector3D>
    assignedPresetDictionary = new Dictionary<string, Vector3D>(),
    assignedCustomDictionary = new Dictionary<string, Vector3D>();

public SortedList<string, ClientRecord> clientRecordDictionary = new SortedList<string, ClientRecord>();

public List<bool> infoChecklist = new List<bool> { false, false, false, false, false };

public TimeSpan
nextUpdate = new TimeSpan(0),
planetCheckSpan = new TimeSpan(0),
nextLogIn = new TimeSpan(0),
nextLoad = new TimeSpan(10);

public Dictionary<string, IEnumerator<bool>> stateList = new Dictionary<string, IEnumerator<bool>>();

public string[] functionList = new string[] {
    "Loading", "Saving", "Gyro Override", // 0 - 2
    "Horizontal Diamond", "Front Wall", "Top Wall", // 3 - 5
    "Cube", "Sphere", "Purge", // 6 - 8
    "Client", "Gyro", "Arranging", // 9 - 11
    "Deployment", "Targeting", "Shooting" // 12 - 14
};

public string[,] commandList = new string[,] {
    { "Register", "Update Position With Host" },
    { "Relative", $"Usage: Relative Right Up Forward{Environment.NewLine}-Set Relative Position" },
    { "Commands,Help", "Display Commands" },
    { "Save Relative", "Save Current Position Relative To Host" },
    { "Queue", "Enable Waypoint Queue" },
    { "Direct", "Disable Waypoint Queue" },
    { "Salt", $"Usage: Salt Password{Environment.NewLine}-Set Transmission Password" },
    { "Send", $"Usage: Send Command{Environment.NewLine}-Send Command To Network" },
    { "Host", "Enable Host Mode" },
    { "Client", "Enable Client Mode" }, //9
    { "Go", "Enable Control" },
    { "Stop", "Disable Control" },
    { "Save", "Save Settings" },
    { "Load", "Load Settings" },
    { "Reset", "Reset Settings" },
    { "Collision Avoidance On,Collision On", "Enable Remote Control Collision Avoidance" },
    { "Collision Avoidance Off,Collision Off", "Disable Remote Control Collision Avoidance" },
    { "Prediction", $"Usage: Prediction Seconds{Environment.NewLine}-Set Prediction In Seconds" },
    { "Multiply", $"Usage: Multiply Value{Environment.NewLine}-Multiply Offsets By Value" }, //18
    { "Update", $"Usage: Update Seconds{Environment.NewLine}-Set Broadcast Frequency In Seconds" },
    { "Assignment,Position,Pos", "Display Assignments" },
    { "Custom,Cus", "Display Custom Positions" },
    { "Preset,Pre", "Display Preset Positions" },
    { "Reassign,Renew", "Force Renew Formation" }
};

public RuntimeAverager runtimeAverager = new RuntimeAverager();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;

    if (Me.CustomData.Length > 0) loading = true;
    else saving = true;

    SetControllers();
    SetListeners();
    shipLength = Vector3D.Distance(new Vector3D(Me.CubeGrid.Min), new Vector3D(Me.CubeGrid.Max)) * (double)Me.CubeGrid.GridSize;
    if (genRemote != null) currentPosition = genRemote.GetPosition();

    nextUpdate = TimeSpan.FromSeconds(broadcastFrequency);
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyroList, b => b.CubeGrid == Me.CubeGrid);
    GridTerminalSystem.GetBlocksOfType<IMyUserControllableGun>(gunList, b => b.CubeGrid == Me.CubeGrid && !(b is IMyLargeTurretBase));
}

public void UnloadConstants()
{
    try
    {
        SetDouble("Relative Right", relativeRight);
        SetDouble("Relative Up", relativeUp);
        SetDouble("Relative Forward", relativeForward);
        SetDouble("Formation Offset", formationDistanceOffset);
        SetDouble("Length Multiplier", lengthMultiplier);
        SetDouble("Broadcast Frequency", broadcastFrequency);
        SetDouble("Prediction Multiplier", predictionMultiplier);
        SetDouble("Waypoint Update Distance", waypointUpdateDistance);
        SetDouble("Weapon Accuracy", requiredAccuracy);
        SetDouble("Overheat Average", overheatAverage);
        SetDouble("Action Usage", actionLimiterMultiplier);
        SetDouble("Runtime Limit", runTimeLimiter);

        SetBool("Queue Waypoints", waypointQueue);
        SetBool("Use Collision Avoidance", useCollisionAvoidance);
        SetBool("Dampen Between Waypoints", dampenBetweenWaypoints);
        SetBool("Always Dampen In Gravity", dampenersAlwayOnInGravity);
        SetBool("Host", host);
        SetBool("Client", client);
        SetBool("Active", active);

        SetString("Broadcast Password", broadcastSalt);
        SetString("Remote Control Filter", remoteControlKey);

        SetInt("Formation", formationMode);
        SetInt("Formation Generation", generatedPositions);
    }
    catch { EchoMessage("Error Caught Saving Constants"); }
}

public void LoadConstants()
{
    try
    {
        GetDouble("Relative Right", ref relativeRight);
        GetDouble("Relative Up", ref relativeUp);
        GetDouble("Relative Forward", ref relativeForward);
        relativeRight = RoundNumber(relativeRight);
        relativeUp = RoundNumber(relativeUp);
        relativeForward = RoundNumber(relativeForward);
        GetDouble("Formation Offset", ref formationDistanceOffset);
        GetDouble("Length Multiplier", ref lengthMultiplier);
        GetDouble("Broadcast Frequency", ref broadcastFrequency);
        GetDouble("Prediction Multiplier", ref predictionMultiplier);
        GetDouble("Waypoint Update Distance", ref waypointUpdateDistance);
        GetDouble("Weapon Accuracy", ref requiredAccuracy);
        GetDouble("Overheat Average", ref overheatAverage);
        GetDouble("Action Usage", ref actionLimiterMultiplier);
        GetDouble("Runtime Limit", ref runTimeLimiter);

        GetBool("Queue Waypoints", ref waypointQueue);
        GetBool("Use Collision Avoidance", ref useCollisionAvoidance);
        GetBool("Dampen Between Waypoints", ref dampenBetweenWaypoints);
        GetBool("Always Dampen In Gravity", ref dampenersAlwayOnInGravity);
        GetBool("Host", ref host);
        GetBool("Client", ref client);
        GetBool("Active", ref active);

        GetString("Broadcast Password", ref broadcastSalt);
        GetString("Remote Control Filter", ref remoteControlKey);

        GetInt("Formation", ref formationMode);
        GetInt("Formation Generation", ref generatedPositions);

        if (myUniqueID.Length == 0 || !myUniqueID.StartsWith(Me.EntityId.ToString() + "/")) SetUniqueId();

        SetListeners();
    }
    catch { EchoMessage("Error Caught Loading Constants"); }
}

public void Main(string argument)
{
    try
    {
        if (reset) Echo("Please recompile");
        else
        {
            tickStartTime = DateTime.Now;
            if (argument.Length > 0) Commands(argument);
            if (reset) return;
            string command = "";
            while (ReceiveMessageString(anseringMachineCommand, ref command))
                commandQueue.Add(command);

            if (OverheatCheck())
            {
                if (!saving && !loading) CheckSettings();
                if (loading) LoadData();
                else if (saving) SaveData();
                else MainScript();
            }
            MainEcho();
        }
    }
    catch { EchoMessage("Error Caught In Main"); }
}

public void MainEcho()
{
    try
    {
        echoTicks++;
        if (echoTicks >= 7)
        {
            echoTicks = 0;
            if (commandDisplay) CommandEcho();
            else if (assignmentDisplay) AssignmentEcho();
            else if (customDisplay || presetDisplay) PositionDisplay();
            else StatusEcho();
            if (messageEcho.Count > 0)
            {
                currentEcho.Add("");
                currentEcho.Add("--Messages--");
                currentEcho.AddRange(messageEcho);
            }
            for (int i = 0; i < currentEcho.Count; i++)
                Echo(currentEcho[i]);
            if (currentEcho.Count < 10)
                for (int i = 0; i < 10 - currentEcho.Count; i++)
                    Echo("");
            currentEcho.Clear();
        }
    }
    catch { EchoMessage("Error Caught In Main Echo"); }
}

public void PositionDisplay()
{
    if (customDisplay)
    {
        EchoStatus($"--Hide With 'Cus'--");
        for (int i = 0; i < customPositions.Count; i++)
            EchoStatus($"{customPositions[i]}: {occupiedCustomDictionary.ContainsKey(customPositions[i])}");
    }
    if (presetDisplay)
    {
        EchoStatus($"--Hide With 'Pre'--");
        for (int i = 0; i < presetPositions.Count; i++)
            EchoStatus($"{presetPositions[i]}: {occupiedPresetDictionary.ContainsKey(presetPositions[i])}");
    }
}

public void AssignmentEcho()
{
    EchoStatus($"--Hide With 'Pos'--");
    int index = 1;
    foreach (KeyValuePair<string, Vector3D> kvp in assignedCustomDictionary)
    {
        EchoStatus($"{index}: {kvp.Value}");
        if (clientRecordDictionary.ContainsKey(kvp.Key))
            EchoStatus($"Last Login: {TimeSpanString(TimeSpan.FromMinutes(1) - clientRecordDictionary[kvp.Key].clientLifeSpan)}");
        index++;
    }
    foreach (KeyValuePair<string, Vector3D> kvp in assignedPresetDictionary)
    {
        EchoStatus($"{index}: {kvp.Value}");
        if (clientRecordDictionary.ContainsKey(kvp.Key))
            EchoStatus($"Last Login: {TimeSpanString(TimeSpan.FromMinutes(1) - clientRecordDictionary[kvp.Key].clientLifeSpan)}");
        index++;
    }
}

public void StatusEcho()
{
    if (loading)
        EchoStatus("Loading Settings");
    else if (saving)
        EchoStatus("Saving Settings");
    if (generatingFormation)
        EchoStatus("Generating Formation");
    if (arrangingFormation)
        EchoStatus("Arranging Formation");
    EchoStatus($"Average: {averageRuntime.ToString("N4")}");
    EchoStatus($"Last: {lastRuntime.ToString("N4")}");
    if (overheatTicks > 0)
        EchoStatus($"Overheat Ticks: {overheatTicks}");
    if (!client && !host)
    {
        EchoStatus("Not client or host");
        EchoStatus("Run command 'client' or 'host'");
    }
    else
    {
        if (client)
        {
            EchoStatus("Client");
            if (targets > 0)
            {
                EchoStatus($"Targets: {targets}");
                EchoStatus($"Accuracy: {targetAccuracy.ToString("N1")}");
                EchoStatus($"Guns: {gunList.Count}");
            }
        }
        else if (host)
        {
            EchoStatus("Host");
            EchoStatus($"Preset Positions: {presetPositionSet.Count}");
            if (customPositionSet.Count > 0)
                EchoStatus($"Custom Positions: {customPositionSet.Count}");
            EchoStatus($"Assigned Positions: {assignedCustomDictionary.Count + assignedPresetDictionary.Count}");
            EchoStatus($"Active Clients: {clientRecordDictionary.Count}");
        }
        if (genRemote == null)
            EchoStatus("No remote controller found");
        else
        {
            EchoStatus($"RC: {genRemote.CustomName}");
            EchoStatus("Current Speed: " + currentVector.Length().ToString("N1") + "/s");
            if (active) EchoStatus("Activated");
            else
            {
                EchoStatus("Deactivated");
                EchoStatus("Run command 'go'");
            }
        }
    }
    EchoStatus($"Sent: {transmissions}");
    EchoStatus($"Received: {receivals}");
}

public void CommandEcho()
{
    EchoStatus($"--Hide With 'Commands'--");
    for (int i = 0; i < commandList.GetLength(0); i++)
    {
        EchoStatus(commandList[i, 0].Replace(",", ", "));
        EchoStatus($"-{commandList[i, 1]}");
    }
}

public void MainScript()
{
    if (commandQueue.Count > 0)
    {
        try
        {
            Commands(commandQueue[0]);
        }
        catch { }
        commandQueue.RemoveAt(0);
    }

    if (genRemote != null)
    {
        if (host) HostScript();
        else if (client) ClientScript();
    }
    else SetControllers();
}

public void EchoStatus(string text)
{
    currentEcho.Add(text);
}

public void EchoMessage(string text)
{
    if (!messageEcho.Contains(text))
        messageEcho.Insert(0, text);
    else
    {
        messageEcho.Remove(text);
        messageEcho.Insert(0, text);
    }
    if (messageEcho.Count > 20)
    {
        messageEcho.RemoveRange(20, messageEcho.Count - 20);
    }
}

public void Commands(string sourceArgument)
{
    try
    {
        string argument = sourceArgument, arg, IDKey = $"id:{myUniqueID}:";
        if (argument.StartsWith("id:"))
        {
            if (argument.StartsWith(IDKey)) argument = argument.Substring(IDKey.Length);
            else return;
        }
        arg = argument.ToLower().Replace(" ", "");
        string[] commandAliasArray;

        int index = -1;

        for (int i = 0; i < commandList.GetLength(0) && index < 0; i++)
        {
            commandAliasArray = commandList[i, 0].Split(',');
            for (int x = 0; x < commandAliasArray.Length && index < 0; x++)
                if (StringsMatch(commandAliasArray[x].Replace(" ", ""), arg))
                    index = i;
        }
        if (index >= 0)
        {
            switch (index)
            {
                case 0:
                    RegisterWithHost();
                    break;
                case 2:
                    commandDisplay = !commandDisplay;
                    break;
                case 3:
                    SaveRelative();
                    break;
                case 4:
                    waypointQueue = true;
                    break;
                case 5:
                    waypointQueue = false;
                    break;
                case 8:
                    host = true;
                    client = false;
                    break;
                case 9:
                    client = true;
                    host = false;
                    break;
                case 10:
                    active = true;
                    break;
                case 11:
                    active = false;
                    break;
                case 12:
                    saving = true;
                    break;
                case 13:
                    loading = true;
                    break;
                case 14:
                    reset = true;
                    Me.CustomData = "";
                    break;
                case 15:
                    useCollisionAvoidance = true;
                    break;
                case 16:
                    useCollisionAvoidance = false;
                    break;
                case 20:
                    assignmentDisplay = !assignmentDisplay;
                    break;
                case 21:
                    customDisplay = !customDisplay;
                    break;
                case 22:
                    presetDisplay = !presetDisplay;
                    break;
                case 23:
                    occupiedPresetDictionary.Clear();
                    assignedPresetDictionary.Clear();

                    presetPositions.Clear();
                    presetPositionSet.Clear();
                    break;

            }
            saving = true;
        }
        else SpacedCommands(argument);
    }
    catch { EchoMessage($"Error Caught Running Command: {sourceArgument}"); }
}

public void SpacedCommands(string argument)
{
    string key, data;
    double value;
    SplitData(argument, out key, out data, " ");
    int index = -1;
    string[] commandAliasArray, argArray;

    if (key.Length > 0 && data.Length > 0)
    {
        key = key.ToLower();

        for (int i = 0; i < commandList.GetLength(0) && index < 0; i++)
        {
            commandAliasArray = commandList[i, 0].Split(',');
            for (int x = 0; x < commandAliasArray.Length && index < 0; x++)
                if (StringsMatch(commandAliasArray[x].Replace(" ", ""), key))
                    index = i;
        }
    }
    if (index >= 0)
    {
        switch (index)
        {
            case 1:
                argArray = data.Split(' ');
                double.TryParse(argArray[0], out relativeRight);
                double.TryParse(argArray[1], out relativeUp);
                double.TryParse(argArray[2], out relativeForward);
                break;
            case 6:
                broadcastSalt = data;
                SetListeners();
                break;
            case 7:
                BroadcastString(broadcastCommand + broadcastSalt, data);
                break;
            case 17:
                if (double.TryParse(data, out value))
                    predictionMultiplier = value;
                break;
            case 18:
                if (double.TryParse(data, out value))
                {
                    relativeForward *= value;
                    relativeRight *= value;
                    relativeUp *= value;
                }
                break;
            case 19:
                if (double.TryParse(data, out value))
                    broadcastFrequency = value;
                break;
        }
        saving = true;
    }
    else LeadingCommands(argument.ToLower().Replace(" ", ""));
}

public void LeadingCommands(string arg)
{
    try
    {
        string[] subArray;

        if (arg.StartsWith("deployment:"))
        {
            subArray = arg.Split(':');
            relativeRight = RoundNumber(double.Parse(subArray[1]));
            relativeUp = RoundNumber(double.Parse(subArray[2]));
            relativeForward = RoundNumber(double.Parse(subArray[3]));
            saving = true;
        }
        else if (arg.StartsWith("login:"))
        {
            RegisterClient(arg);
        }
        saving = true;
    }
    catch { }
}

public bool GetAssignedPosition(string uniqueID, out Vector3D vector)
{
    if (assignedPresetDictionary.ContainsKey(uniqueID))
    {
        vector = assignedPresetDictionary[uniqueID];
    }
    else if (assignedCustomDictionary.ContainsKey(uniqueID))
    {
        vector = assignedCustomDictionary[uniqueID];
    }
    else vector = Vector3D.Zero;
    return vector != Vector3D.Zero;
}

public bool GetPositionOwner(Vector3D vector, out string uniqueID)
{
    if (occupiedPresetDictionary.ContainsKey(vector))
    {
        uniqueID = occupiedPresetDictionary[vector];
    }
    else if (occupiedCustomDictionary.ContainsKey(vector))
    {
        uniqueID = occupiedCustomDictionary[vector];
    }
    else uniqueID = "";
    return uniqueID.Length > 0;
}

public void AssignOwner(Vector3D vector, string uniqueID)
{
    RemoveOwner(uniqueID);
    RemoveAssignedPosition(vector);

    if (presetPositionSet.Contains(vector))
    {
        occupiedPresetDictionary[vector] = uniqueID;
        assignedPresetDictionary[uniqueID] = vector;
    }
    else if (customPositionSet.Contains(vector))
    {
        occupiedCustomDictionary[vector] = uniqueID;
        assignedCustomDictionary[uniqueID] = vector;
    }
    else
    {
        AddCustomPosition(vector);
        occupiedCustomDictionary[vector] = uniqueID;
        assignedCustomDictionary[uniqueID] = vector;
        arrangingFormation = true;
    }

    saving = true;
}

public void RemoveOwner(string uniqueID)
{
    if (assignedCustomDictionary.ContainsKey(uniqueID))
    {
        occupiedCustomDictionary.Remove(assignedCustomDictionary[uniqueID]);
        assignedCustomDictionary.Remove(uniqueID);
    }
    if (assignedPresetDictionary.ContainsKey(uniqueID))
    {
        occupiedPresetDictionary.Remove(assignedPresetDictionary[uniqueID]);
        assignedPresetDictionary.Remove(uniqueID);
    }
}

public void RemoveAssignedPosition(Vector3D vector)
{
    if (occupiedPresetDictionary.ContainsKey(vector))
    {
        assignedPresetDictionary.Remove(occupiedPresetDictionary[vector]);
        occupiedPresetDictionary.Remove(vector);
    }
    if (occupiedCustomDictionary.ContainsKey(vector))
    {
        assignedCustomDictionary.Remove(occupiedCustomDictionary[vector]);
        occupiedCustomDictionary.Remove(vector);
    }
}

public void RegisterClient(string argument)
{
    string[] subArgs = argument.Split(':');

    string uniqueID = subArgs[1], positionOwner;
    if (uniqueID.Length == 0) return;
    Vector3D offset = new Vector3D(double.Parse(subArgs[2]), double.Parse(subArgs[3]), double.Parse(subArgs[4])),
        assignedVector;
    offset = RoundVector(offset);
    if (!client && host)
    {
        if (!clientRecordDictionary.ContainsKey(uniqueID))
        {
            clientRecordDictionary[uniqueID] = new ClientRecord { uniqueID = uniqueID };
        }
        else
        {
            clientRecordDictionary[uniqueID].clientLifeSpan = TimeSpan.FromMinutes(1);
        }

        if (offset == Vector3D.Zero)
        {
            if (GetAssignedPosition(uniqueID, out assignedVector))
            {
                if (customPositionSet.Contains(assignedVector))
                {
                    RemoveOwner(uniqueID);
                    RemoveAssignedPosition(assignedVector);
                    customPositionSet.Remove(assignedVector);
                    customPositions.Remove(assignedVector);
                    AssignID(uniqueID);
                }
                else if (presetPositionSet.Contains(assignedVector)) UpdateClientAssignment(uniqueID, assignedVector);
            }
            else
            {
                AssignID(uniqueID);
            }
            return;
        }

        if (GetAssignedPosition(uniqueID, out assignedVector))
        {
            //You have assigned position
            if (assignedVector != offset)
            {
                //Your current position is different from your assigned position
                if (GetPositionOwner(offset, out positionOwner))
                {
                    //Current position has an owner
                    if (positionOwner == uniqueID)
                    {
                        //You own your position. Reset ownership
                        AssignOwner(offset, uniqueID);
                    }
                    else
                    {
                        //Someone else owns your position. Reassign
                        AssignID(uniqueID);
                    }
                }
                else
                {
                    //Current position has no owner
                    if (offset != Vector3D.Zero)
                    {
                        if (presetPositionSet.Contains(offset) || customPositionSet.Contains(offset))
                            AssignOwner(offset, uniqueID);
                        else
                        {
                            AddCustomPosition(offset);
                            AssignOwner(offset, uniqueID);
                        }
                    }
                    else AssignID(uniqueID);
                }
            }
        }
        else
        {
            //You have no assigned position
            if (presetPositionSet.Contains(offset))
            {
                //Your current position exists already
                if (!occupiedPresetDictionary.ContainsKey(offset))
                {
                    //Your position is unassigned, assigning to you now
                    AssignOwner(offset, uniqueID);
                }
                else
                {
                    //Your position is already assigned, assigning new position
                    AssignID(uniqueID);
                }
            }
            else if (customPositionSet.Contains(offset))
            {
                //Your current position exists already
                if (!occupiedCustomDictionary.ContainsKey(offset))
                {
                    //Your position is unassigned, assigning to you now
                    AssignOwner(offset, uniqueID);
                }
                else
                {
                    //Your position is already assigned, assigning new position
                    AssignID(uniqueID);
                }
            }
            else
            {
                AddCustomPosition(offset);
                AssignOwner(offset, uniqueID);
            }
        }
    }
    else if (client && !host)
    {
        if (uniqueID != myUniqueID && offset == new Vector3D(relativeRight, relativeUp, relativeForward))
        {
            relativeForward = 0;
            relativeRight = 0;
            relativeUp = 0;
            ResetAutopilot();
        }
    }
}

public void AssignID(string uniqueID)
{
    Vector3D assignedVector;
    RemoveOwner(uniqueID);
    if (NextAvailablePosition(out assignedVector))
    {
        AssignOwner(assignedVector, uniqueID);
        UpdateClientAssignment(uniqueID, assignedVector);
    }
}

public void RegisterWithHost()
{
    if (client && !host)
    {
        BroadcastString(broadcastCommand + broadcastSalt, $"setregistration:{myUniqueID}:{relativeRight}:{relativeUp}:{relativeForward}");
    }
}

public bool CheckAvailablePositions()
{
    if (!arrangingFormation && (occupiedPresetDictionary.Count >= presetPositions.Count || generatingFormation))
    {
        generatingFormation = true;
        switch (formationMode)
        {
            case 0:
                if (GenerateDiamondFormation(true))
                    generatingFormation = false;
                break;
            case 1:
                if (GenerateDiamondFormation(false))
                    generatingFormation = false;
                break;
            case 2:
                if (GenerateFrontWall(true))
                    generatingFormation = false;
                break;
            case 3:
                if (GenerateFrontWall(false))
                    generatingFormation = false;
                break;
            case 4:
                if (GenerateTopWall(true))
                    generatingFormation = false;
                break;
            case 5:
                if (GenerateTopWall(false))
                    generatingFormation = false;
                break;
            case 6:
                if (GenerateCube(true))
                    generatingFormation = false;
                break;
            case 7:
                if (GenerateCube(false))
                    generatingFormation = false;
                break;
        }
        if (!generatingFormation) arrangingFormation = true;
        return false;
    }
    return true;
}

public bool ArrangeFormation()
{
    if (arrangingFormation)
    {
        string key = functionList[11];
        if (!stateList.ContainsKey(key))
            stateList[key] = ArrangeState();

        if (StateManager(key))
            arrangingFormation = false;

        return false;
    }
    return true;
}

public IEnumerator<bool> ArrangeState()
{
    List<SortableObject> sortableList = new List<SortableObject>();

    if (customPositionSet.Count > 1)
    {
        foreach (Vector3D cusVector in customPositionSet)
        {
            if (!AvailableActions(0.5)) yield return true;
            sortableList.Add(new SortableObject { doubleValue = Vector3D.Distance(Vector3D.Zero, cusVector), vector = cusVector });
        }

        yield return true;
        sortableList = sortableList.OrderBy(x => x.doubleValue).ToList();
        customPositions.Clear();
        for (int i = 0; i < sortableList.Count; i++)
        {
            if (!AvailableActions()) yield return true;
            customPositions.Add(sortableList[i].vector);
        }
        sortableList.Clear();
    }

    if (presetPositionSet.Count > 1)
    {
        foreach (Vector3D cusVector in presetPositionSet)
        {
            if (!AvailableActions(0.5)) yield return true;
            sortableList.Add(new SortableObject { doubleValue = Vector3D.Distance(Vector3D.Zero, cusVector), vector = cusVector });
        }
        yield return true;
        sortableList = sortableList.OrderBy(x => x.doubleValue).ToList();
        presetPositions.Clear();
        for (int i = 0; i < sortableList.Count; i++)
        {
            if (!AvailableActions()) yield return true;
            presetPositions.Add(sortableList[i].vector);
        }
        sortableList.Clear();
    }

    yield return false;
}

public bool GenerateCube(bool cube = true)
{
    string key = functionList[6];
    if (!stateList.ContainsKey(key))
        stateList[key] = GenerateCubeState(cube);

    return StateManager(key);
}

public IEnumerator<bool> GenerateCubeState(bool cube)
{
    double initialDistance = Math.Sqrt(Math.Pow(formationDistanceOffset + (shipLength * lengthMultiplier), 2.0) / 3.0),
           spacing = formationDistanceOffset + (shipLength * lengthMultiplier);

    Vector3D aTemplate, bTemplate, cTemplate, dTemplate,
        currentVector,
        lineVector,
        nextPosition;

    aTemplate = bTemplate = cTemplate = dTemplate = new Vector3D(0, 0, 0);


    int maxPositions = presetPositions.Count + Math.Max(1, generatedPositions),
        start = presetPositions.Count + 1, row = 1,
        subPosition = 0, position = 1,
        index = 1, side = 1;

    for (int i = 0; i < presetPositions.Count; i++)
    {
        if (!AvailableActions()) yield return true;
        presetPositionSet.Add(presetPositions[i]);
    }


    while (presetPositions.Count < maxPositions)
    {
        if (!AvailableActions()) yield return true;

        switch (side)
        {
            case 1:
                aTemplate = new Vector3D(-initialDistance, initialDistance, initialDistance);
                bTemplate = new Vector3D(initialDistance, initialDistance, initialDistance);
                cTemplate = new Vector3D(-initialDistance, -initialDistance, initialDistance);
                dTemplate = new Vector3D(initialDistance, -initialDistance, initialDistance);
                break;
            case 2:
                aTemplate = new Vector3D(-initialDistance, initialDistance, -initialDistance);
                bTemplate = new Vector3D(initialDistance, initialDistance, -initialDistance);
                cTemplate = new Vector3D(-initialDistance, -initialDistance, -initialDistance);
                dTemplate = new Vector3D(initialDistance, -initialDistance, -initialDistance);
                break;
            case 3:
                aTemplate = new Vector3D(-initialDistance, initialDistance, initialDistance);
                bTemplate = new Vector3D(initialDistance, initialDistance, initialDistance);
                cTemplate = new Vector3D(-initialDistance, initialDistance, -initialDistance);
                dTemplate = new Vector3D(initialDistance, initialDistance, -initialDistance);
                break;
            case 4:
                aTemplate = new Vector3D(-initialDistance, -initialDistance, initialDistance);
                bTemplate = new Vector3D(initialDistance, -initialDistance, initialDistance);
                cTemplate = new Vector3D(-initialDistance, -initialDistance, -initialDistance);
                dTemplate = new Vector3D(initialDistance, -initialDistance, -initialDistance);
                break;
            case 5:
                aTemplate = new Vector3D(-initialDistance, initialDistance, initialDistance);
                bTemplate = new Vector3D(-initialDistance, -initialDistance, initialDistance);
                cTemplate = new Vector3D(-initialDistance, -initialDistance, -initialDistance);
                dTemplate = new Vector3D(-initialDistance, initialDistance, -initialDistance);
                break;
            case 6:
                aTemplate = new Vector3D(initialDistance, initialDistance, initialDistance);
                bTemplate = new Vector3D(initialDistance, -initialDistance, initialDistance);
                cTemplate = new Vector3D(initialDistance, -initialDistance, -initialDistance);
                dTemplate = new Vector3D(initialDistance, initialDistance, -initialDistance);
                break;
        }


        if (position == 1)
        {
            lineVector = ((bTemplate * (double)row) - (aTemplate * (double)row)) / ((double)row);
            currentVector = aTemplate * (double)row;
        }
        else if (position == 2)
        {
            lineVector = ((cTemplate * (double)row) - (bTemplate * (double)row)) / ((double)row);
            currentVector = bTemplate * (double)row;
        }
        else if (position == 3)
        {
            lineVector = ((dTemplate * (double)row) - (cTemplate * (double)row)) / ((double)row);
            currentVector = cTemplate * (double)row;
        }
        else if (position == 4)
        {
            lineVector = ((aTemplate * (double)row) - (dTemplate * (double)row)) / ((double)row);
            currentVector = dTemplate * (double)row;
        }
        else break;
        while (subPosition < row)
        {
            if (!AvailableActions()) yield return true;
            if (index >= start)
            {
                nextPosition = currentVector + (lineVector * (double)subPosition);
                if (!cube)
                    nextPosition = Vector3D.Normalize(nextPosition) * (spacing * (double)row);
                AddPresetPosition(nextPosition);
            }
            index++;
            subPosition++;
        }
        subPosition = 0;
        position++;
        if (position > 4)
        {
            position = 1;
            side++;
            if (side > 6)
            {
                side = 1;
                row++;
            }
        }
    }




    yield return false;
}

public bool GenerateDiamondFormation(bool horizontal = true)
{
    string key = functionList[3];
    if (!stateList.ContainsKey(key))
        stateList[key] = GenerateDiamondState(horizontal);

    return StateManager(key);
}

public IEnumerator<bool> GenerateDiamondState(bool horizontal)
{
    int maxPositions = presetPositions.Count + Math.Max(1, generatedPositions),
        start = presetPositions.Count + 1, row = 1,
        subPosition = 0, position = 1,
        index = 1;

    double initialDistance = formationDistanceOffset + (shipLength * lengthMultiplier);
    Vector3D
    forwardTemplate = new Vector3D(0, 0, initialDistance),
    rightTemplate = new Vector3D(initialDistance, 0, 0),
    currentVector,
    lineVector;
    Vector3D
    backwardTemplate = forwardTemplate * -1.0,
    leftTemplate = rightTemplate * -1.0;

    while (presetPositions.Count < maxPositions)
    {
        if (!AvailableActions()) yield return true;

        if (position == 1)
        {
            lineVector = ((rightTemplate * (double)row) - (forwardTemplate * (double)row)) / ((double)row);
            currentVector = forwardTemplate * (double)row;
        }
        else if (position == 2)
        {
            lineVector = ((backwardTemplate * (double)row) - (rightTemplate * (double)row)) / ((double)row);
            currentVector = rightTemplate * (double)row;
        }
        else if (position == 3)
        {
            lineVector = ((leftTemplate * (double)row) - (backwardTemplate * (double)row)) / ((double)row);
            currentVector = backwardTemplate * (double)row;
        }
        else if (position == 4)
        {
            lineVector = ((forwardTemplate * (double)row) - (leftTemplate * (double)row)) / ((double)row);
            currentVector = leftTemplate * (double)row;
        }
        else break;
        while (subPosition < row)
        {
            if (!AvailableActions()) yield return true;
            if (index >= start)
            {
                if (horizontal)
                    AddPresetPosition(currentVector + (lineVector * (double)subPosition));
                else AddPresetPosition(HorToVertVector(currentVector + (lineVector * (double)subPosition)));
            }
            index++;
            subPosition++;
        }
        subPosition = 0;
        position++;
        if (position > 4)
        {
            position = 1;
            row++;
        }
    }

    yield return false;
}

public void AddCustomPosition(Vector3D sourceVector)
{
    Vector3D vector = RoundVector(sourceVector);

    if (!presetPositionSet.Contains(vector) && customPositionSet.Add(vector))
        customPositions.Add(vector);
}

public void AddPresetPosition(Vector3D sourceVector)
{
    Vector3D vector = RoundVector(sourceVector);
    string ownerID = "";
    bool resetOwnership = false;

    if (customPositionSet.Remove(vector))
    {
        customPositions.Remove(vector);
        if (GetPositionOwner(vector, out ownerID)) resetOwnership = true;
        RemoveAssignedPosition(vector);
    }

    if (presetPositionSet.Add(vector))
        presetPositions.Add(vector);
    else
    {
        presetPositions.Remove(vector);
        presetPositions.Add(vector);
    }

    if (resetOwnership && ownerID.Length > 0)
        AssignOwner(vector, ownerID);
}

public bool GenerateFrontWall(bool front = true)
{
    string key = functionList[4];
    if (!stateList.ContainsKey(key))
        stateList[key] = GenerateFrontWallState(front);

    return StateManager(key);
}

public IEnumerator<bool> GenerateFrontWallState(bool front)
{
    double initialDistance = Math.Sqrt(Math.Pow(formationDistanceOffset + (shipLength * lengthMultiplier), 2.0) / 3.0);

    Vector3D
        topLeftTemplate = new Vector3D(-initialDistance, initialDistance, 0),
        topRightTemplate = new Vector3D(initialDistance, initialDistance, 0),
        botRightTemplate = new Vector3D(initialDistance, -initialDistance, 0),
        botLeftTemplate = new Vector3D(-initialDistance, -initialDistance, 0),
        currentVector,
        lineVector;


    int maxPositions = presetPositions.Count + Math.Max(1, generatedPositions),
        start = presetPositions.Count + 1, row = 1,
        subPosition = 0, position = 1,
        index = 1;


    while (presetPositions.Count < maxPositions)
    {
        if (!AvailableActions()) yield return true;

        if (position == 1)
        {
            lineVector = ((topRightTemplate * (double)row) - (topLeftTemplate * (double)row)) / ((double)row);
            currentVector = topLeftTemplate * (double)row;
        }
        else if (position == 2)
        {
            lineVector = ((botRightTemplate * (double)row) - (topRightTemplate * (double)row)) / ((double)row);
            currentVector = topRightTemplate * (double)row;
        }
        else if (position == 3)
        {
            lineVector = ((botLeftTemplate * (double)row) - (botRightTemplate * (double)row)) / ((double)row);
            currentVector = botRightTemplate * (double)row;
        }
        else if (position == 4)
        {
            lineVector = ((topLeftTemplate * (double)row) - (botLeftTemplate * (double)row)) / ((double)row);
            currentVector = botLeftTemplate * (double)row;
        }
        else break;
        while (subPosition < row)
        {
            if (!AvailableActions()) yield return true;
            if (index >= start)
            {
                if (front)
                    AddPresetPosition(currentVector + (lineVector * (double)subPosition) + new Vector3D(0, 0, initialDistance));
                else AddPresetPosition(currentVector + (lineVector * (double)subPosition) + new Vector3D(0, 0, -initialDistance));
            }
            index++;
            subPosition++;
        }
        subPosition = 0;
        position++;
        if (position > 4)
        {
            position = 1;
            row++;
        }
    }


    yield return false;
}

public bool GenerateTopWall(bool top = true)
{
    string key = functionList[5];
    if (!stateList.ContainsKey(key))
        stateList[key] = GenerateTopWallState(top);

    return StateManager(key);
}

public IEnumerator<bool> GenerateTopWallState(bool top)
{
    double initialDistance = Math.Sqrt(Math.Pow(formationDistanceOffset + (shipLength * lengthMultiplier), 2.0) / 3.0);

    Vector3D
        topLeftTemplate = new Vector3D(-initialDistance, 0, initialDistance),
        topRightTemplate = new Vector3D(initialDistance, 0, initialDistance),
        botRightTemplate = new Vector3D(initialDistance, 0, -initialDistance),
        botLeftTemplate = new Vector3D(-initialDistance, 0, -initialDistance),
        currentVector,
        lineVector;


    int maxPositions = presetPositions.Count + Math.Max(1, generatedPositions),
        start = presetPositions.Count + 1, row = 1,
        subPosition = 0, position = 1,
        index = 1;


    while (presetPositions.Count < maxPositions)
    {
        if (!AvailableActions()) yield return true;

        if (position == 1)
        {
            lineVector = ((topRightTemplate * (double)row) - (topLeftTemplate * (double)row)) / ((double)row);
            currentVector = topLeftTemplate * (double)row;
        }
        else if (position == 2)
        {
            lineVector = ((botRightTemplate * (double)row) - (topRightTemplate * (double)row)) / ((double)row);
            currentVector = topRightTemplate * (double)row;
        }
        else if (position == 3)
        {
            lineVector = ((botLeftTemplate * (double)row) - (botRightTemplate * (double)row)) / ((double)row);
            currentVector = botRightTemplate * (double)row;
        }
        else if (position == 4)
        {
            lineVector = ((topLeftTemplate * (double)row) - (botLeftTemplate * (double)row)) / ((double)row);
            currentVector = botLeftTemplate * (double)row;
        }
        else break;
        while (subPosition < row)
        {
            if (!AvailableActions()) yield return true;
            if (index >= start)
            {
                if (top)
                    AddPresetPosition(currentVector + (lineVector * (double)subPosition) + new Vector3D(0, initialDistance, 0));
                else AddPresetPosition(currentVector + (lineVector * (double)subPosition) + new Vector3D(0, -initialDistance, 0));
            }
            index++;
            subPosition++;
        }
        subPosition = 0;
        position++;
        if (position > 4)
        {
            position = 1;
            row++;
        }
    }


    yield return false;
}

public Vector3D HorToVertVector(Vector3D vector)
{
    return new Vector3D(vector.X, vector.Z, 0);
}

public void GetString(string key, ref string value)
{
    for (int a = 0; a < settingListStrings.Count; a++)
        if (settingListStrings.Values[a].ContainsKey(key))
        {
            value = settingListStrings.Values[a][key];
            return;
        }
    EchoMessage($"Setting Not Found: {key}");
}

public bool SetString(string key, string value)
{
    for (int a = 0; a < settingListStrings.Count; a++)
    {
        if (settingListStrings.Values[a].ContainsKey(key))
        {
            settingListStrings[settingListStrings.Keys[a]][key] = value;
            return true;
        }
    }
    return false;
}

public void GetInt(string key, ref int value)
{
    for (int a = 0; a < settingListInts.Count; a++)
        if (settingListInts.Values[a].ContainsKey(key))
        {
            value = settingListInts.Values[a][key];
            return;
        }
    EchoMessage($"Setting Not Found: {key}");
}

public bool SetInt(string key, int value)
{
    for (int a = 0; a < settingListInts.Count; a++)
    {
        if (settingListInts.Values[a].ContainsKey(key))
        {
            settingListInts[settingListInts.Keys[a]][key] = value;
            return true;
        }
    }
    return false;
}

public void GetBool(string key, ref bool value)
{
    for (int a = 0; a < settingListBools.Count; a++)
        if (settingListBools.Values[a].ContainsKey(key))
        {
            value = settingListBools.Values[a][key];
            return;
        }
    EchoMessage($"Setting Not Found: {key}");
}

public bool SetBool(string key, bool value)
{
    for (int a = 0; a < settingListBools.Count; a++)
    {
        if (settingListBools.Values[a].ContainsKey(key))
        {
            settingListBools[settingListBools.Keys[a]][key] = value;
            return true;
        }
    }
    return false;
}

public void GetDouble(string key, ref double value)
{
    for (int a = 0; a < settingListDoubles.Count; a++)
        if (settingListDoubles.Values[a].ContainsKey(key))
        {
            value = settingListDoubles.Values[a][key];
            return;
        }
    EchoMessage($"Setting Not Found: {key}");
}

public bool SetDouble(string key, double value)
{
    for (int a = 0; a < settingListDoubles.Count; a++)
    {
        if (settingListDoubles.Values[a].ContainsKey(key))
        {
            settingListDoubles[settingListDoubles.Keys[a]][key] = value;
            return true;
        }
    }
    return false;
}

public void SplitData(string source, out string key, out string data, string divisor = "=")
{
    int index = source.IndexOf(divisor);
    if (index > 0)
    {
        key = source.Substring(0, index);
        data = source.Substring(index + divisor.Length);
    }
    else
    {
        key = "";
        data = "";
    }
}

public bool AvailableActions(double multiplier = 0.95)
{
    double mult = multiplier;
    if (overheatAverage > 0)
        mult = Math.Min(mult, 1.0 - (averageRuntime / (overheatAverage * 1.5)));
    if (mult <= 0) mult = 0.01;
    return
    (Runtime.CurrentInstructionCount < (Runtime.MaxInstructionCount * actionLimiterMultiplier * mult) &&
    (DateTime.Now - tickStartTime).TotalMilliseconds < runTimeLimiter * mult);
}

public void CheckSettings(bool force = false)
{
    if (force || nextLoad.TotalSeconds <= 0)
    {
        if (settingBackup.Length != Me.CustomData.Length || settingBackup != Me.CustomData)
        {
            loading = true;
            nextLoad = TimeSpan.FromSeconds(10);
        }
        else nextLoad = TimeSpan.FromSeconds(5);
    }
    else nextLoad -= Runtime.TimeSinceLastRun;
}

public bool OverheatCheck()
{
    lastRuntime = Runtime.LastRunTimeMs;

    runtimeAverager.AddRuntime(lastRuntime);

    torchAverage += lastRuntime * tickWeight;
    torchAverage *= 1.0 - tickWeight;

    averageRuntime = Math.Max(runtimeAverager.Average(), torchAverage);

    if (averageRuntime < overheatAverage)
    {
        overheatTicks = 0;
        return true;
    }
    overheatTicks++;
    return false;
}

public void CheckFire()
{
    string key = functionList[13];
    if (!stateList.ContainsKey(key))
        stateList[key] = TargetingState();

    StateManager(key);

    if (!TargetingEnemies() && !genRemote.IsAutoPilotEnabled)
    {
        aimVector = GetDirectionTo(currentPosition + (lastForward * 1500));
        aimVector.Z = GetRoll(currentPosition + (lastUp * -1500.0), genRemote) * -1.0;
    }
    else if (!TargetingEnemies()) aimVector = Vector3D.Zero;
    ApplyGyroOverride();
}

public bool TargetingEnemies()
{
    return hasTarget || targets > 0;
}

public IEnumerator<bool> TargetingState()
{
    List<IMyLargeTurretBase> turretList = new List<IMyLargeTurretBase>();
    while (true)
    {
        turretList.Clear();
        GridTerminalSystem.GetBlocksOfType<IMyLargeTurretBase>(turretList, b => b.HasTarget);
        if (turretList.Count > 0)
        {
            MyDetectedEntityInfo info;

            List<SortableObject> sortableList = new List<SortableObject>();
            for (int i = 0; i < turretList.Count; i++)
            {
                if (!AvailableActions()) yield return true;
                info = turretList[i].GetTargetedEntity();
                if (TargetGood(info))
                    sortableList.Add(new SortableObject { doubleValue = info.BoundingBox.Size.Length(), intValue = i });
            }

            if (sortableList.Count == 0)
            {
                yield return true;
                continue;
            }

            yield return true;
            sortableList = sortableList.OrderByDescending(x => x.doubleValue).ToList();
            yield return true;

            targets = sortableList.Count;

            currentTargetingTurret = turretList[sortableList[0].intValue];
            currentTurretTargetID = currentTargetingTurret.GetTargetedEntity().EntityId;

            while (true)
            {
                info = currentTargetingTurret.GetTargetedEntity();
                if (TargetGood(info) && info.EntityId == currentTurretTargetID)
                {
                    FireOnTarget(info);
                }
                else
                {
                    ResetTargetingTurret();
                    break;
                }
                yield return true;
            }
        }
        else
        {
            ResetTargetingTurret();
        }
        yield return true;
    }
}

public double GetRoll(Vector3D targetVector, IMyTerminalBlock originBlock)
{
    MatrixD matrix = originBlock.WorldMatrix;
    Vector3D originVector = originBlock.GetPosition(), forwardVector = matrix.Down * 5.0, rightVector = matrix.Right * 5.0, upVector = matrix.Forward * 5.0;
    forwardVector += originVector;
    rightVector += originVector;
    upVector += originVector;

    double
    originRange = Vector3D.Distance(originVector, targetVector),
    forwardRange = Vector3D.Distance(forwardVector, targetVector),
    upRange = Vector3D.Distance(upVector, targetVector),
    rightRange = Vector3D.Distance(rightVector, targetVector),
    upLength = Vector3D.Distance(upVector, originVector),
    rightLength = Vector3D.Distance(rightVector, originVector);

    double
    ThetaP = Math.Acos((upRange * upRange - upLength * upLength - originRange * originRange) / (-2.0 * upLength * originRange)),
    ThetaY = Math.Acos((rightRange * rightRange - rightLength * rightLength - originRange * originRange) / (-2.0 * rightLength * originRange));
    double outYaw = 90.0 - (ThetaY * 180.0 / Math.PI);

    if (originRange < forwardRange) outYaw = 180.0 - outYaw;
    if (outYaw > 180.0) outYaw = -1.0 * (360.0 - outYaw);

    outYaw = (outYaw / 180.0) * Math.PI;

    return outYaw;
}

public void FireOnTarget(MyDetectedEntityInfo info)
{
    Vector3D targetedVector = currentPosition + (genRemote.WorldMatrix.Forward * Vector3D.Distance(currentPosition, info.Position));
    targetAccuracy = Vector3D.Distance(targetedVector, info.Position);

    Fire(targetAccuracy < Math.Min(Math.Max(0.05, requiredAccuracy), Math.Max(20, info.BoundingBox.Size.Length() * 1.2)));
    aimVector = GetDirectionTo(info.Position);
    hasTarget = true;
}

public void ResetTargetingTurret()
{
    currentTurretTargetID = -1;
    currentTargetingTurret = null;
    aimVector = new Vector3D(0, 0, 0);
    hasTarget = false;
    targets--;
    Fire(false);
}

public bool TargetGood(MyDetectedEntityInfo info)
{
    return
        !info.IsEmpty() && info.Relationship != MyRelationsBetweenPlayerAndBlock.Owner &&
        info.Relationship != MyRelationsBetweenPlayerAndBlock.Friends &&
        info.Relationship != MyRelationsBetweenPlayerAndBlock.FactionShare &&
        (info.Type == MyDetectedEntityType.LargeGrid || info.Type == MyDetectedEntityType.SmallGrid);
}

public void Fire(bool shoot = true)
{
    string key = functionList[14];
    if (!stateList.ContainsKey(key))
        stateList[key] = FireState(shoot);

    StateManager(key);
}

public IEnumerator<bool> FireState(bool shoot)
{
    for (int i = 0; i < gunList.Count; i++)
    {
        if (!AvailableActions()) yield return true;
        if (shoot)
        {
            try
            {
                gunList[i].ApplyAction("ShootOnce");
                gunList[i].ApplyAction("Shoot_On");
            }
            catch { }
        }
        else
        {
            gunList[i].ApplyAction("Shoot_Off");
        }
    }
    yield return false;
}

public Vector3D RoundVector(Vector3D sourceVector, int decimals = 1)
{
    return new Vector3D(RoundNumber(sourceVector.X, decimals), RoundNumber(sourceVector.Y, decimals), RoundNumber(sourceVector.Z, decimals));
}

public double RoundNumber(double sourceNumber, int decimals = 1)
{
    double number = Math.Floor(Math.Abs(sourceNumber) * Math.Pow(10, decimals)) / Math.Pow(10, decimals);
    if (sourceNumber < 0)
        number *= -1.0;
    return number;
}

public string TimeSpanString(TimeSpan span)
{
    string text = "";
    if (span.Days > 0)
        text = $"{span.Days} d";
    if (span.Hours > 0)
    {
        if (text.Length > 0) text += ": ";
        text += $"{span.Hours} h";
    }
    if (span.Minutes > 0)
    {
        if (text.Length > 0) text += ": ";
        text += $"{span.Minutes} m";
    }
    if (span.Seconds > 0)
    {
        if (text.Length > 0) text += ": ";
        text += $"{span.Seconds} s";
    }
    if (text.Length == 0) text = "0 s";
    return text;
}

public void ApplyGyroOverride()
{
    string key = functionList[10];
    if (!stateList.ContainsKey(key))
        stateList[key] = ApplyGyroOverrideState();

    StateManager(key);
}

//~Whip's ApplyGyroOverride Method v9 - 8/19/17
public IEnumerator<bool> ApplyGyroOverrideState()
{
    var rotationVec = new Vector3D(-aimVector.X, aimVector.Y, aimVector.Z);
    var shipMatrix = genRemote.WorldMatrix;
    var relativeRotationVec = Vector3D.TransformNormal(rotationVec, shipMatrix);
    double reps = 0, limit = 10;
    int index = 0;
    while (true)
    {
        try
        {
            if (gyroList.Count > 0)
            {
                if (index >= gyroList.Count) index = 0;
                IMyGyro thisGyro = gyroList[index];

                if (relativeRotationVec != Vector3D.Zero)
                {
                    var gyroMatrix = thisGyro.WorldMatrix;
                    var transformedRotationVec = Vector3D.TransformNormal(relativeRotationVec, Matrix.Transpose(gyroMatrix));
                    thisGyro.Pitch = (float)transformedRotationVec.X;
                    thisGyro.Yaw = (float)transformedRotationVec.Y;
                    thisGyro.Roll = (float)transformedRotationVec.Z;
                    thisGyro.GyroOverride = true;
                }
                else
                {
                    thisGyro.Pitch = 0f;
                    thisGyro.Yaw = 0f;
                    thisGyro.Roll = 0f;
                    thisGyro.GyroOverride = false;
                }
            }
        }
        catch { }
        reps++;
        index++;
        if (reps >= limit || !AvailableActions())
        {
            yield return true;
            reps = 0;
            rotationVec = new Vector3D(-aimVector.X, aimVector.Y, aimVector.Z);
            shipMatrix = genRemote.WorldMatrix;
            relativeRotationVec = Vector3D.TransformNormal(rotationVec, shipMatrix);
        }
    }
}

public Vector3D GetDirectionTo(Vector3D targetVector)
{
    MatrixD matrix = genRemote.WorldMatrix;
    Vector3D originVector = genRemote.GetPosition(), forwardVector = matrix.Forward * 5.0, rightVector = matrix.Right * 5.0, upVector = matrix.Up * 5.0;
    forwardVector += originVector;
    rightVector += originVector;
    upVector += originVector;

    double
    originRange = Vector3D.Distance(originVector, targetVector),
    forwardRange = Vector3D.Distance(forwardVector, targetVector),
    upRange = Vector3D.Distance(upVector, targetVector),
    rightRange = Vector3D.Distance(rightVector, targetVector),
    upLength = Vector3D.Distance(upVector, originVector),
    rightLength = Vector3D.Distance(rightVector, originVector);

    double
    ThetaP = Math.Acos((upRange * upRange - upLength * upLength - originRange * originRange) / (-2.0 * upLength * originRange)),
    ThetaY = Math.Acos((rightRange * rightRange - rightLength * rightLength - originRange * originRange) / (-2.0 * rightLength * originRange));
    double outPitch = 90.0 - (ThetaP * 180.0 / Math.PI), outYaw = 90.0 - (ThetaY * 180.0 / Math.PI);

    if (originRange < forwardRange) outPitch = 180 - outPitch;
    if (outPitch > 180.0) outPitch = -1 * (360 - outPitch);
    if (originRange < forwardRange) outYaw = 180.0 - outYaw;
    if (outYaw > 180.0) outYaw = -1.0 * (360.0 - outYaw);

    outPitch = (outPitch / 180.0) * Math.PI;
    outYaw = (outYaw / 180.0) * Math.PI;

    return new Vector3D(outPitch, outYaw, 0);
}

public void LogIn()
{
    if (nextLogIn.TotalSeconds <= 0)
    {
        BroadcastString(broadcastCommand + broadcastSalt, $"login:{myUniqueID}:{relativeRight}:{relativeUp}:{relativeForward}");
        nextLogIn = TimeSpan.FromSeconds(7.5);
    }
    else nextLogIn -= Runtime.TimeSinceLastRun;
}

public void SetUniqueId()
{
    myUniqueID = $"{Me.EntityId}/{VectorId(Me.GetPosition())}/{DateId()}";
    saving = true;
}

public string VectorId(Vector3D sourceVector)
{
    Vector3D vector = RoundVector(sourceVector, 0);

    return $"{CutText($"{vector.X}")}-{CutText($"{vector.Y}")}-{CutText($"{vector.Z}")}";
}

public string CutText(string text, int length = 6)
{
    return text.Substring(0, Math.Min(length, text.Length));
}

public string DateId()
{
    DateTime temp = DateTime.Now;
    return temp.Day.ToString() + temp.Hour.ToString() + temp.Millisecond.ToString();
}

public Vector3D SaveRelative()
{
    if (genRemote != null && hostPosition != Vector3D.Zero)
    {
        Vector3D measPosA, measPosB;
        bool posRight, posForward, posUp;
        measPosA = hostPosition + (lastRight * 1000.0);
        measPosB = hostPosition + (lastRight * -1000.0);
        posRight = Vector3D.Distance(measPosA, currentPosition) < Vector3D.Distance(measPosB, currentPosition);
        measPosA = hostPosition + (lastForward * 1000.0);
        measPosB = hostPosition + (lastForward * -1000.0);
        posForward = Vector3D.Distance(measPosA, currentPosition) < Vector3D.Distance(measPosB, currentPosition);
        measPosA = hostPosition + (lastUp * 1000.0);
        measPosB = hostPosition + (lastUp * -1000.0);
        posUp = Vector3D.Distance(measPosA, currentPosition) < Vector3D.Distance(measPosB, currentPosition);
        double mult = 1.0;
        if (!posRight) mult = -1.0;
        relativeRight = NearestRelative(mult, lastRight, currentPosition);
        if (posForward) mult = 1.0;
        else mult = -1.0;
        relativeForward = NearestRelative(mult, lastForward, currentPosition);
        if (posUp) mult = 1.0;
        else mult = -1.0;
        relativeUp = NearestRelative(mult, lastUp, currentPosition);
        return new Vector3D(relativeRight, relativeForward, relativeUp);
    }
    return new Vector3D(0, 0, 0);
}

public double NearestRelative(double mult, Vector3D vector, Vector3D currentVector)
{
    double multK = 0, multH = 0, multS = 0;
    while (Vector3D.Distance(hostPosition + (vector * mult * multK), currentVector) > Vector3D.Distance(hostPosition + (vector * mult * (multK + 1000.0)), currentVector))
        multK += 1000.0;
    if (Vector3D.Distance(hostPosition + (vector * mult * multK), currentVector) > Vector3D.Distance(hostPosition + (vector * mult * (multK - 1000.0)), currentVector))
        multK -= 1000.0;
    while (Vector3D.Distance(hostPosition + (vector * mult * (multK + multH)), currentVector) > Vector3D.Distance(hostPosition + (vector * mult * (multK + multH + 100.0)), currentVector))
        multH += 100.0;
    if (Vector3D.Distance(hostPosition + (vector * mult * (multK + multH)), currentVector) > Vector3D.Distance(hostPosition + (vector * mult * (multK + multH - 100.0)), currentVector))
        multH -= 100.0;
    while (Vector3D.Distance(hostPosition + (vector * mult * (multK + multH + multS)), currentVector) > Vector3D.Distance(hostPosition + (vector * mult * (multK + multH + multS + 1.0)), currentVector))
        multS += 1.0;
    if (Vector3D.Distance(hostPosition + (vector * mult * (multK + multH + multS)), currentVector) > Vector3D.Distance(hostPosition + (vector * mult * (multK + multH + multS - 1.0)), currentVector))
        multS -= 1.0;
    if (Vector3D.Distance(hostPosition + (vector * mult * (multK + multH + multS)), currentVector) > Vector3D.Distance(hostPosition + (vector * mult * (multK + multH + multS + 0.5)), currentVector))
        multS += 0.5;
    if (Vector3D.Distance(hostPosition + (vector * mult * (multK + multH + multS)), currentVector) > Vector3D.Distance(hostPosition + (vector * mult * (multK + multH + multS - 0.5)), currentVector))
        multS -= 0.5;
    return (multK + multH + multS) * mult;
}

public void SetListeners()
{
    List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
    IGC.GetBroadcastListeners(listeners);
    for (int i = 0; i < listeners.Count; i++)
        IGC.DisableBroadcastListener(listeners[i]);
    answeringMachineLocation = IGC.RegisterBroadcastListener(broadcastLocation + broadcastSalt);
    answeringMachineVector = IGC.RegisterBroadcastListener(broadcastVector + broadcastSalt);
    answeringMachineUp = IGC.RegisterBroadcastListener(broadcastUp + broadcastSalt);
    answeringMachineRight = IGC.RegisterBroadcastListener(broadcastRight + broadcastSalt);
    answeringMachineForward = IGC.RegisterBroadcastListener(broadcastForward + broadcastSalt);
    anseringMachineCommand = IGC.RegisterBroadcastListener(broadcastCommand + broadcastSalt);
}

public void HostScript()
{
    try
    {
        currentPosition = genRemote.GetPosition();
        if (planetCheckSpan.TotalSeconds <= 0.0)
        {
            Vector3D planetPosition = new Vector3D(0, 0, 0);
            inGravity = ((IMyShipController)genRemote).TryGetPlanetPosition(out planetPosition);
            planetCheckSpan = TimeSpan.FromSeconds(5);
        }
        else planetCheckSpan -= Runtime.TimeSinceLastRun;
        if (active)
        {
            currentVector = genRemote.GetShipVelocities().LinearVelocity;
            if (CheckAvailablePositions() && ArrangeFormation())
            {
                Purge();
                AutoDeployment();
            }
            RelayInformation();
        }
    }
    catch { EchoMessage("Error caught running host script"); }
}

public void AutoDeployment()
{
    string key = functionList[12];
    if (!stateList.ContainsKey(key))
        stateList[key] = AutoDeployState();

    StateManager(key);
}

public IEnumerator<bool> AutoDeployState()
{
    TimeSpan delay;
    int reps = 0;
    while (true)
    {
        for (int i = 0; i < clientRecordDictionary.Count; i++)
        {
            reps++;
            if (!UpdateClientAssignment(clientRecordDictionary.Keys[i]))
            {
                AssignID(clientRecordDictionary.Keys[i]);
                yield return true;
            }
            else if (reps >= 2)
            {
                reps = 0;
                yield return true;
            }
        }


        delay = TimeSpan.FromSeconds(2.5);
        while (delay.TotalSeconds > 0)
        {
            yield return true;
            delay -= Runtime.TimeSinceLastRun;
        }
    }
}

public bool NextAvailablePosition(out Vector3D vector)
{
    vector = Vector3D.Zero;
    if (!generatingFormation && !arrangingFormation)
    {
        if (customPositions.Count > occupiedCustomDictionary.Count)
        {
            for (int i = 0; i < customPositions.Count; i++)
            {
                if (!occupiedCustomDictionary.ContainsKey(customPositions[i]))
                {
                    vector = customPositions[i];
                    break;
                }
            }
        }
        else if (presetPositions.Count > occupiedPresetDictionary.Count)
        {
            for (int i = 0; i < presetPositions.Count; i++)
            {
                if (!occupiedPresetDictionary.ContainsKey(presetPositions[i]))
                {
                    vector = presetPositions[i];
                    break;
                }
            }
        }
    }
    return vector != Vector3D.Zero;
}

public bool UpdateClientAssignment(string uniqueID)
{
    Vector3D vector;
    if (GetAssignedPosition(uniqueID, out vector))
    {
        UpdateClientAssignment(uniqueID, vector);
        return true;
    }
    return false;
}

public void UpdateClientAssignment(string uniqueID, Vector3D vector)
{
    if (uniqueID.Length > 0)
    {
        BroadcastString(broadcastCommand + broadcastSalt, $"id:{uniqueID}:deployment:{vector.X}:{vector.Y}:{vector.Z}");
    }
}

public void Purge()
{
    string key = functionList[8];
    if (!stateList.ContainsKey(key))
        stateList[key] = PurgeState();

    StateManager(key);
}

public IEnumerator<bool> PurgeState()
{
    HashSet<Vector3D> verifiedPositions = new HashSet<Vector3D>();
    HashSet<string> verifiedIDs = new HashSet<string>();
    List<Vector3D> clearedPositions = new List<Vector3D>();
    List<string> clearedAssignments = new List<string>();
    Vector3D assignedVector;
    TimeSpan timeSinceLastRun;

    while (active)
    {
        timeSinceLastRun = Runtime.TimeSinceLastRun;
        for (int i = 0; i < clientRecordDictionary.Count; i++)
        {
            ClientRecord clientRecord = clientRecordDictionary.Values[i];
            if (!AvailableActions(0.25))
            {
                yield return true;
                timeSinceLastRun += Runtime.TimeSinceLastRun;
            }

            if (GetAssignedPosition(clientRecord.uniqueID, out assignedVector))
            {
                clientRecord.clientLifeSpan -= timeSinceLastRun;
                if ((clientRecord.clientLifeSpan).TotalSeconds <= 0) RemoveOwner(clientRecord.uniqueID);
                else
                {
                    verifiedPositions.Add(assignedVector);
                    verifiedIDs.Add(clientRecord.uniqueID);
                }
            }
        }

        foreach (KeyValuePair<Vector3D, string> kvp in occupiedCustomDictionary)
        {
            if (!AvailableActions(0.25)) yield return true;
            if (!verifiedPositions.Contains(kvp.Key))
            {
                clearedPositions.Add(kvp.Key);
                clearedAssignments.Add(kvp.Value);
            }
        }

        foreach (KeyValuePair<Vector3D, string> kvp in occupiedPresetDictionary)
        {
            if (!AvailableActions(0.25)) yield return true;
            if (!verifiedPositions.Contains(kvp.Key))
            {
                clearedPositions.Add(kvp.Key);
                clearedAssignments.Add(kvp.Value);
            }
        }

        foreach (KeyValuePair<string, Vector3D> kvp in assignedCustomDictionary)
        {
            if (!AvailableActions(0.25)) yield return true;
            if (!verifiedIDs.Contains(kvp.Key))
            {
                clearedPositions.Add(kvp.Value);
                clearedAssignments.Add(kvp.Key);
            }
        }

        foreach (KeyValuePair<string, Vector3D> kvp in assignedPresetDictionary)
        {
            if (!AvailableActions(0.25)) yield return true;
            if (!verifiedIDs.Contains(kvp.Key))
            {
                clearedPositions.Add(kvp.Value);
                clearedAssignments.Add(kvp.Key);
            }
        }

        for (int i = 0; i < clearedAssignments.Count; i++)
        {
            if (!AvailableActions(0.25)) yield return true;
            clearedIDs.Add(clearedAssignments[i]);
            RemoveOwner(clearedAssignments[i]);
            clientRecordDictionary.Remove(clearedAssignments[i]);
        }

        for (int i = 0; i < clearedPositions.Count; i++)
        {
            if (!AvailableActions(0.25)) yield return true;
            RemoveAssignedPosition(clearedPositions[i]);
        }
        saving = saving || clearedAssignments.Count > 0 || clearedPositions.Count > 0;
        clearedAssignments.Clear();
        clearedPositions.Clear();

        yield return true;
    }
    yield return false;
}

public void RelayInformation()
{
    if (nextUpdate.TotalSeconds <= 0)
    {
        BroadcastVector3D(broadcastLocation + broadcastSalt, currentPosition);
        matrix = genRemote.WorldMatrix;
        BroadcastVector3D(broadcastVector + broadcastSalt, currentVector);
        BroadcastVector3D(broadcastUp + broadcastSalt, matrix.Up);
        BroadcastVector3D(broadcastRight + broadcastSalt, matrix.Right);
        BroadcastVector3D(broadcastForward + broadcastSalt, matrix.Forward);
        nextUpdate = TimeSpan.FromSeconds(broadcastFrequency);
    }
    else nextUpdate -= Runtime.TimeSinceLastRun;
}

public void ClientScript()
{
    string key = functionList[9];
    if (!stateList.ContainsKey(key))
        stateList[key] = ClientState();

    StateManager(key);
}

public IEnumerator<bool> ClientState()
{
    while (true)
    {
        currentPosition = genRemote.GetPosition();
        LogIn();

        if (!updated)
        {
            infoChecklist[0] = ReceiveMessageVector3D(answeringMachineLocation, ref hostPosition) || infoChecklist[0];
            infoChecklist[1] = ReceiveMessageVector3D(answeringMachineVector, ref lastVector) || infoChecklist[1];
            infoChecklist[2] = ReceiveMessageVector3D(answeringMachineUp, ref lastUp) || infoChecklist[2];
            infoChecklist[3] = ReceiveMessageVector3D(answeringMachineRight, ref lastRight) || infoChecklist[3];
            infoChecklist[4] = ReceiveMessageVector3D(answeringMachineForward, ref lastForward) || infoChecklist[4];
            updated = infoChecklist[0] && infoChecklist[1] && infoChecklist[2] && infoChecklist[3] && infoChecklist[4];
        }
        else
        {
            ReceiveMessageVector3D(answeringMachineLocation, ref hostPosition);
            ReceiveMessageVector3D(answeringMachineVector, ref lastVector);
            ReceiveMessageVector3D(answeringMachineUp, ref lastUp);
            ReceiveMessageVector3D(answeringMachineRight, ref lastRight);
            ReceiveMessageVector3D(answeringMachineForward, ref lastForward);
        }

        CheckFire();

        if (active && updated)
        {
            if (!TargetingEnemies())
            {
                if (relativeRight != 0 || relativeForward != 0 || relativeUp != 0)
                {
                    Vector3D currentDestination = hostPosition + (lastVector * predictionMultiplier) + (lastUp * relativeUp) + (lastRight * relativeRight) + (lastForward * relativeForward);

                    if ((lastDestination != Vector3D.Zero && Vector3D.Distance(currentDestination, lastDestination) >= waypointUpdateDistance) || (!genRemote.IsAutoPilotEnabled && Vector3D.Distance(currentPosition, currentDestination) >= waypointUpdateDistance))
                    {
                        lastDestination = currentDestination;
                        SetWaypoint(currentDestination, !waypointQueue);
                    }
                }
            }
            else ResetAutopilot();
        }
        else ResetAutopilot();
        yield return true;
    }
}

public void ResetAutopilot()
{
    genRemote.SetAutoPilotEnabled(false);
    genRemote.ClearWaypoints();
    genRemote.DampenersOverride = true;
}

public void SettingExplanation(string key, ref StringBuilder builder)
{
    if (settingExplanations.ContainsKey(key))
    {
        foreach (string line in settingExplanations[key])
            builder.AppendLine($"-{line}");
    }
}

public void SaveData()
{
    string key = functionList[1];
    if (!stateList.ContainsKey(key))
    {
        CheckSettings(true);
        if (loading) return;
        stateList[key] = SaveState();
    }

    if (StateManager(key))
    {
        saving = false;
        EchoMessage("Save Complete");
    }
}

public IEnumerator<bool> SaveState()
{
    UnloadConstants();

    StringBuilder hostBuilder = new StringBuilder(),
                  clientBuilder = new StringBuilder(),
                  universalBuilder = new StringBuilder(),
                  finalBuilder = new StringBuilder();

    foreach (KeyValuePair<string, SortedList<string, double>> kvpA in settingListDoubles)
    {
        if (!AvailableActions()) yield return true;
        foreach (KeyValuePair<string, double> kvpB in kvpA.Value)
        {
            if (!AvailableActions()) yield return true;
            if (kvpA.Key == "Client")
            {
                clientBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref clientBuilder);
            }
            else if (kvpA.Key == "Host")
            {
                hostBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref hostBuilder);
            }
            else
            {
                universalBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref universalBuilder);
            }
        }
    }

    foreach (KeyValuePair<string, SortedList<string, bool>> kvpA in settingListBools)
    {
        if (!AvailableActions()) yield return true;
        foreach (KeyValuePair<string, bool> kvpB in kvpA.Value)
        {
            if (!AvailableActions()) yield return true;
            if (kvpA.Key == "Client")
            {
                clientBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref clientBuilder);
            }
            else if (kvpA.Key == "Host")
            {
                hostBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref hostBuilder);
            }
            else
            {
                universalBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref universalBuilder);
            }
        }
    }

    foreach (KeyValuePair<string, SortedList<string, string>> kvpA in settingListStrings)
    {
        if (!AvailableActions()) yield return true;
        foreach (KeyValuePair<string, string> kvpB in kvpA.Value)
        {
            if (!AvailableActions()) yield return true;
            if (kvpA.Key == "Client")
            {
                clientBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref clientBuilder);
            }
            else if (kvpA.Key == "Host")
            {
                hostBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref hostBuilder);
            }
            else
            {
                universalBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref universalBuilder);
            }
        }
    }

    foreach (KeyValuePair<string, SortedList<string, int>> kvpA in settingListInts)
    {
        if (!AvailableActions()) yield return true;
        foreach (KeyValuePair<string, int> kvpB in kvpA.Value)
        {
            if (!AvailableActions()) yield return true;
            if (kvpA.Key == "Client")
            {
                clientBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref clientBuilder);
            }
            else if (kvpA.Key == "Host")
            {
                hostBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref hostBuilder);
            }
            else
            {
                universalBuilder.AppendLine($"{kvpB.Key}={kvpB.Value}");
                SettingExplanation(kvpB.Key, ref universalBuilder);
            }
        }
    }

    finalBuilder.AppendLine("--Client Settings--");
    finalBuilder.AppendLine();
    finalBuilder.AppendLine(clientBuilder.ToString());
    finalBuilder.AppendLine();

    finalBuilder.AppendLine("--Host Settings--");
    finalBuilder.AppendLine();
    finalBuilder.AppendLine(hostBuilder.ToString());
    finalBuilder.AppendLine();

    finalBuilder.AppendLine("--Universal Settings--");
    finalBuilder.AppendLine();
    finalBuilder.AppendLine(universalBuilder.ToString());
    finalBuilder.AppendLine();

    if (customPositions.Count > 0 || assignedPresetDictionary.Count > 0)
    {
        finalBuilder.AppendLine("--Presets--");
        finalBuilder.AppendLine();
        HashSet<Vector3D> savedVectors = new HashSet<Vector3D>();

        for (int i = 0; i < customPositions.Count; i++)
        {
            if (!AvailableActions()) yield return true;
            if (occupiedCustomDictionary.ContainsKey(customPositions[i]))
                finalBuilder.AppendLine($"Custom Preset={ConvertVector(customPositions[i], occupiedCustomDictionary[customPositions[i]])}");
            else finalBuilder.AppendLine($"Custom Preset={ConvertVector(customPositions[i])}");
        }

        foreach (KeyValuePair<string, Vector3D> kvp in assignedPresetDictionary)
        {
            if (!AvailableActions()) yield return true;
            finalBuilder.AppendLine($"Preset={kvp.Key}:{kvp.Value.X}:{kvp.Value.Y}:{kvp.Value.Z}");
        }
        finalBuilder.AppendLine();
    }

    finalBuilder.AppendLine("--Data--");
    finalBuilder.AppendLine();
    finalBuilder.AppendLine("Unique ID=" + myUniqueID);
    finalBuilder.AppendLine("Script=" + script);
    finalBuilder.Append("Version=" + version);

    Me.CustomData = finalBuilder.ToString().Trim();
    settingBackup = Me.CustomData;
    correctVersion = true;
    correctScript = true;

    yield return false;
}

public bool StateManager(string key)
{
    if (stateList.ContainsKey(key))
    {
        bool moreWork;
        try
        {
            moreWork = stateList[key].MoveNext();
        }
        catch
        {
            moreWork = false;
            EchoMessage($"Error caught running function: {key}");
        }
        if (!moreWork)
        {
            StateDisposal(key);
            return true;
        }
        return false;
    }
    return true;
}

public void StateDisposal(string key)
{
    try
    {
        stateList[key].Dispose();
        stateList[key] = null;
    }
    catch { }
    try
    {
        stateList.Remove(key);
    }
    catch { }
}

public string ConvertVector(Vector3D vector, string uniqueID = "None")
{
    return $"{uniqueID}:{vector.X}:{vector.Y}:{vector.Z}";
}

public bool ConvertStringToVector(string source, out string uniqueID, out Vector3D vector)
{
    try
    {
        string[] subArgs = source.Split(':');

        uniqueID = subArgs[0];
        if (StringsMatch(uniqueID, "none"))
            uniqueID = "";

        vector = new Vector3D(double.Parse(subArgs[1]), double.Parse(subArgs[2]), double.Parse(subArgs[3]));

        return vector != Vector3D.Zero;
    }
    catch { }
    uniqueID = "";
    vector = Vector3D.Zero;
    return false;
}

public void LoadData()
{
    string key = functionList[0];
    if (!stateList.ContainsKey(key))
        stateList[key] = LoadState();

    if (StateManager(key))
    {
        loading = false;
        EchoMessage("Load Complete");
    }
}

public IEnumerator<bool> LoadState()
{
    correctVersion = false;
    correctScript = false;
    presetPositions.Clear();
    customPositions.Clear();
    presetPositionSet.Clear();
    customPositionSet.Clear();
    if (Me.CustomData.Length > 0)
    {
        if (!AvailableActions(0.5)) yield return true;
        string[] settingArray = Me.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < settingArray.Length; i++)
        {
            if (!AvailableActions()) yield return true;
            if (settingArray[i].Length > 0 && !settingArray[i].StartsWith("-"))
                ProcessSetting(settingArray[i]);
        }
    }
    settingBackup = Me.CustomData;
    if (!AvailableActions(0.5)) yield return true;
    LoadConstants();
    clearedIDs.Clear();
    if (!reset && (!correctScript || !correctVersion))
    {
        EchoMessage("Updating Settings");
        saving = true;
    }
    yield return false;
}

public bool StringsMatch(string a, string b)
{
    return string.Compare(a, b, true) == 0;
}

public void ProcessSetting(string settingString)
{
    try
    {
        string key, data;
        double dataDouble;
        bool dataBool, hasNumber, hasBool;

        SplitData(settingString, out key, out data);

        hasNumber = double.TryParse(data, out dataDouble);
        dataBool = StringsMatch(data, "true");
        hasBool = dataBool || StringsMatch(data, "false");

        if (hasNumber)
        {
            if (SetDouble(key, dataDouble)) return;
            if (SetInt(key, (int)dataDouble)) return;
            if (StringsMatch(key, "version"))
            {
                correctVersion = version == dataDouble;
                return;
            }
        }
        else if (hasBool)
        {
            if (SetBool(key, dataBool)) return;
        }
        else
        {
            if (SetString(key, data)) return;
            if (StringsMatch(key, "unique id"))
            {
                myUniqueID = data;
                return;
            }
            if (StringsMatch(key, "script"))
            {
                correctScript = StringsMatch(script, data);
                return;
            }
            if (StringsMatch(key, "custom preset"))
            {
                LoadPreset(data, true);
                return;
            }
            if (StringsMatch(key, "preset"))
            {
                LoadPreset(data);
                return;
            }
        }
        EchoMessage($"Unhandled setting: {key} = {data}");
    }
    catch { }
}

public void LoadPreset(string data, bool custom = false)
{
    string uniqueID;
    Vector3D vector;

    if (ConvertStringToVector(data, out uniqueID, out vector))
    {
        if (clearedIDs.Contains(uniqueID)) uniqueID = "";
        if (uniqueID.Length > 0 && !StringsMatch(uniqueID, "none") && !clientRecordDictionary.ContainsKey(uniqueID))
            clientRecordDictionary[uniqueID] = new ClientRecord { uniqueID = uniqueID };
        if (custom)
        {
            AddCustomPosition(vector);
            if (uniqueID.Length > 0 && !StringsMatch(uniqueID, "none"))
                AssignOwner(vector, uniqueID);
        }
        else
        {
            AddPresetPosition(vector);
            if (uniqueID.Length > 0 && !StringsMatch(uniqueID, "none"))
                AssignOwner(vector, uniqueID);
        }
    }
}

public void SetControllers()
{
    string rcKey = remoteControlKey.ToLower();
    List<IMyRemoteControl> bKs = new List<IMyRemoteControl>();
    GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(bKs, (p => p.CubeGrid == Me.CubeGrid && (rcKey.Length == 0 || p.CustomName.ToLower().Contains(rcKey))));
    if (bKs.Count > 0) genRemote = bKs[0];
}

public void SetWaypoint(Vector3D point, bool clear = true)
{
    try
    {
        if (genRemote != null)
        {
            List<MyWaypointInfo> waypointList = new List<MyWaypointInfo>();
            genRemote.GetWaypointInfo(waypointList);

            if (waypointList.Count == 0 || Vector3D.Distance(waypointList[waypointList.Count - 1].Coords, point) >= waypointUpdateDistance)
            {
                if (clear || Vector3D.Distance(point, currentPosition) < waypointUpdateDistance) genRemote.ClearWaypoints();
                genRemote.AddWaypoint(point, "Waypoint");
            }
            if (!dampenBetweenWaypoints && (!inGravity || !dampenersAlwayOnInGravity))
                genRemote.DampenersOverride = false;
            else genRemote.DampenersOverride = true;
            if (Vector3D.Distance(currentPosition, point) >= waypointUpdateDistance)
            {
                genRemote.FlightMode = FlightMode.OneWay;
                genRemote.SetAutoPilotEnabled(true);
                genRemote.SetCollisionAvoidance(useCollisionAvoidance);
                genRemote.SetDockingMode(false);
            }
        }
    }
    catch { EchoMessage("Error setting waypoint"); }
}

public void BroadcastVector3D(string tag, Vector3D data, bool antenna = true, bool subGrids = false, bool sameGrid = false)
{
    transmissions++;
    if (antenna)
        IGC.SendBroadcastMessage<Vector3D>(tag, data, TransmissionDistance.AntennaRelay); // Send over antennas to programmable blocks
    if (subGrids)
        IGC.SendBroadcastMessage<Vector3D>(tag, data, TransmissionDistance.ConnectedConstructs); // Send to subgrids to programmable blocks
    if (sameGrid)
        IGC.SendBroadcastMessage<Vector3D>(tag, data, TransmissionDistance.CurrentConstruct); // Send to same-grid programmable blocks
}

public bool ReceiveMessageVector3D(IMyBroadcastListener answeringMachine, ref Vector3D vector)
{
    if (answeringMachine == null) return false;
    if (!answeringMachine.HasPendingMessage) return false; // No messages received
    else
    {
        MyIGCMessage message = answeringMachine.AcceptMessage();
        vector = (Vector3D)(message.Data);
        receivals++;
        return true;
    }
}

public void BroadcastString(string tag, string data, bool antenna = true, bool subGrids = false, bool sameGrid = false)
{
    transmissions++;
    if (antenna)
        IGC.SendBroadcastMessage<string>(tag, data, TransmissionDistance.AntennaRelay); // Send over antennas to programmable blocks
    if (subGrids)
        IGC.SendBroadcastMessage<string>(tag, data, TransmissionDistance.ConnectedConstructs); // Send to subgrids to programmable blocks
    if (sameGrid)
        IGC.SendBroadcastMessage<string>(tag, data, TransmissionDistance.CurrentConstruct); // Send to same-grid programmable blocks
}

public bool ReceiveMessageString(IMyBroadcastListener answeringMachine, ref string command)
{
    if (answeringMachine == null) return false;
    if (!answeringMachine.HasPendingMessage) return false; // No messages received
    else
    {
        MyIGCMessage message = answeringMachine.AcceptMessage();
        command = (string)(message.Data);
        receivals++;
        return true;
    }
}

public class SortableObject
{
    public Vector3D vector;
    public double doubleValue = 0;
    public int intValue = 0;
}

public class ClientRecord
{
    public TimeSpan clientLifeSpan = new TimeSpan(0, 1, 0);
    public string uniqueID = "";
}

public class RuntimeAverager
{
    private double count = 0;

    private double[] averageArray = new double[30];

    private int index = 0;

    public double Average()
    {
        if (count == 0) return 0;

        double average = 0;
        for (int i = 0; i < count; i++)
            average += averageArray[i];

        return average / count;
    }

    public void AddRuntime(double runtime)
    {
        if (count < 30) count++;

        averageArray[index] = runtime;

        index++;
        if (index >= 30) index = 0;
    }
}