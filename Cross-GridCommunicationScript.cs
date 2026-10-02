//Cross-Grid Communication Script v1.5 by MrHam5
//2019.04.16.

//set your ship's master code:
readonly string code = "MasterCode";
//set the custom LCD beginning
readonly string selectorString = "Comm";

//sending a message
//syntax: targetName messageText

//sending a command to another ship
//syntax: targetName /command parameter

//using an internal command
//syntax: /command parameter

//Do NOT edit anything below this! (unless you know what you're doing)

Program()
{
    GridTerminalSystem.GetBlocksOfType(commTextPanels, IsControlledBlock);

    List<IMyRadioAntenna> antennaList = new List<IMyRadioAntenna>();
    GridTerminalSystem.GetBlocksOfType(antennaList);
    if (antennaList.Count > 0)
        antenna = antennaList[0] as IMyRadioAntenna;
    else
        throw new Exception("No Antenna found on the grid!");

    antenna.Enabled = true;
    antenna.EnableBroadcasting = true;
    antenna.IgnoreAlliedBroadcast = false;
    antenna.IgnoreOtherBroadcast = false;
    antenna.AttachedProgrammableBlock = Me.EntityId;

    IGC.RegisterBroadcastListener(ShipName());
    IGC.RegisterBroadcastListener("everyone");
    IGC.GetBroadcastListeners(listeners);

    if (!Me.CustomName.Contains(selectorString))
    {
        Me.CustomName += " " + selectorString;

        if (Me.CustomData == "" || !myIni.TryParse(Me.CustomData))
        {
            myIni.Clear();
            myIni.Set("CommScript", $"display0", "messages");
            myIni.Set("CommScript", $"display1", "status");
            Me.CustomData = myIni.ToString();
        }
        else if (!myIni.ContainsSection("CommScript"))
        {
            myIni.Set("CommScript", $"display0", "messages");
            myIni.Set("CommScript", $"display1", "status");
            Me.CustomData = myIni.ToString();
        }
    }

    GetTextSurfaceProviders();

    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    Echo(EchoText());
    UpdateTextSurfaces();

    foreach (var listener in listeners)
    {
        if (listener.HasPendingMessage)
        {
            MyIGCMessage message = new MyIGCMessage();
            message = listener.AcceptMessage();
            ReceiveTransmission(message);
        }
    }

    if (String.IsNullOrEmpty(argument))
    {
        return;
    }

    if (selfDestructPrompt)
    {
        if (argument == code)
        {
            SelfDestruct("engage");
        }
        else
        {
            UpdateMessages("Invalid code, request canceled!");
        }
        selfDestructPrompt = false;
        return;
    }

    // commands
    if (argument[0] == '/')
    {
        UpdateMessages("@: " + argument);

        string[] splitArray;
        splitArray = argument.Substring(1).Split(' ');

        if (splitArray.Length == 1)
            PerformInsideCommand(splitArray[0]);
        else
            PerformInsideCommand(splitArray[0], splitArray[1]);
    }
    // outgoing transmissions
    else SendTransmission(argument);
}

public void ReceiveTransmission(MyIGCMessage message)
{
    string input = message.Data.ToString();

    string[] splitArray;
    splitArray = input.Split(' ');

    string senderName = splitArray[0];
    string joinedText = "";
    for (int i = 1; i < splitArray.Length; i++)
        joinedText += $" {splitArray[i]}";

    if (splitArray[1][0] == '/' && splitArray.Length == 2)
    {
        PerformOutsideCommand(splitArray[1].Substring(1), senderName); // parameter == sender
    }
    else if (splitArray[1][0] == '/' && splitArray.Length > 2)
    {
        string parameters = "";
        for (int i = 2; i < splitArray.Length; i++)
        {
            if (i != 2) parameters += " ";
            parameters += $"{splitArray[i]}";
        }
        PerformOutsideCommand(splitArray[1].Substring(1), parameters);
    }
    else // only echo when normal message was received
    {
        UpdateMessages($"Received from {senderName}:\n{joinedText}");
    }
}

public void SendTransmission(string input)
{
    string[] splitArray = null;
    splitArray = input.Split(' ');
    string target = splitArray[0];

    string message = "";
    for (int i = 1; i < splitArray.Length; i++)
        message += $" {splitArray[i]}";

    UpdateMessages($"Sent to {splitArray[0]}:\n{message}");

    message = ShipName() + message;
    IGC.SendBroadcastMessage(target, message, TransmissionDistance.TransmissionDistanceMax);
}

public void PerformOutsideCommand(string command, string parameter = "")
{
    if (command == "open" || command == "close")
    {
        IMyBlockGroup hangarGroup = GridTerminalSystem.GetBlockGroupWithName(parameter);
        List<IMyAirtightHangarDoor> doors = new List<IMyAirtightHangarDoor>();
        try
        {
            hangarGroup.GetBlocksOfType(doors);
        }
        catch { return; }

        switch (command)
        {
            case "open":
                UpdateMessages($"Opening {parameter}");
                foreach (var door in doors)
                {
                    door.OpenDoor();
                }
                break;
            case "close":
                UpdateMessages($"Closing {parameter}");
                foreach (var door in doors)
                {
                    door.CloseDoor();
                }
                break;
        }
    }
    else if (command == "trigger")
    {
        var timerBlock = GridTerminalSystem.GetBlockWithName(parameter) as IMyTimerBlock;
        if (timerBlock != null)
        {
            UpdateMessages($"Triggering {parameter}");
            timerBlock.Trigger();
        }
    }
    else if (command == "ping")
    {
        string message = $"{ShipName()} /pong";
        IGC.SendBroadcastMessage(parameter, message, TransmissionDistance.TransmissionDistanceMax);
        UpdateMessages($"@: Responding to a ping\nfrom: {parameter}");
    }
    else if (command == "pong")
    {
        UpdateMessages($"Succesfully pinged {parameter}");
    }
}

public void PerformInsideCommand(string command, string parameter = "")
{
    switch (command.ToLower())
    {
        case "ping":
            string message = $"{ShipName()} /ping";
            IGC.SendBroadcastMessage(parameter, message, TransmissionDistance.TransmissionDistanceMax);
            break;
        case "selfdestruct":
            if (!selfDestructEnabled)
            {
                UpdateMessages("To start the self-destruct sequence,\nplease input the mastercode");
                selfDestructPrompt = true;
            }
            else
            {
                SelfDestruct("disengage");
            }
            break;
        case "listconfig":
            string output =
                $"Antenna enabled: {antenna.Enabled}" +
                $"\nAntenna broadcasting: {antenna.EnableBroadcasting}" +
                $"\nAntenna range: {antenna.Radius} m" +
                $"\nReceive friendly: {!antenna.IgnoreAlliedBroadcast}" +
                $"\nReceive other: {!antenna.IgnoreOtherBroadcast}";
            UpdateMessages(output);
            break;
        case "help":

            break;
        default:
            UpdateMessages("Invalid command!");
            break;
    }
}

public void GetTextSurfaceProviders()
{
    List<IMyTerminalBlock> terminalBlocks = new List<IMyTerminalBlock>();
    tsProviders = new List<TSProvider>();
    // get local terminal blocks
    GridTerminalSystem.GetBlocksOfType(terminalBlocks, IsControlledBlock);

    // get every terminal block that has some sort of TextSurface
    foreach (var block in terminalBlocks)
    {
        if (block as IMyTextSurface != null)
        {
            tsProviders.Add(new TSProvider(block, false));
            var surface = block as IMyTextSurface;
            surface.ContentType = ContentType.TEXT_AND_IMAGE;
            surface.FontSize = 0.875f;
            surface.TextPadding = 0f;
            continue;
        }

        if (block as IMyTextSurfaceProvider != null)
        {
            var provider = block as IMyTextSurfaceProvider;
            tsProviders.Add(new TSProvider(block, true, provider.SurfaceCount));

            if (block.CustomData == "" || !myIni.TryParse(block.CustomData))
            {
                myIni.Clear();
                for (int i = 0; i < provider.SurfaceCount; i++)
                {
                    myIni.Set("CommScript", $"display{i}", "none");
                }
                block.CustomData = myIni.ToString();
            }
            else if (!myIni.ContainsSection("CommScript"))
            {
                for (int i = 0; i < provider.SurfaceCount; i++)
                {
                    myIni.Set("CommScript", $"display{i}", "none");
                }
                block.CustomData = myIni.ToString();
            }
        }
    }
}

public void UpdateTextSurfaces()
{
    foreach (var item in tsProviders)
    {
        if (item.isProvider == false)
        {
            var surface = item.block as IMyTextSurface;
            surface.FontColor = new Color(255, 255, 255);
            surface.WriteText(MessageText());
        }
        else
        {
            if (!myIni.TryParse(item.block.CustomData))
            {
                GetTextSurfaceProviders();
                return;
            }
            var provider = item.block as IMyTextSurfaceProvider;
            for (int i = 0; i < provider.SurfaceCount; i++)
            {
                if (myIni.Get("CommScript", $"display{i}").ToString() != "none")
                {
                    var surface = provider.GetSurface(i);
                    surface.ContentType = ContentType.SCRIPT;
                    surface.ScriptBackgroundColor = new Color(0, 88, 151);
                    using (var frame = surface.DrawFrame())
                    {
                        Vector2 scale = surface.SurfaceSize / 512f;
                        float size = scale.X;

                        MySprite sprite;
                        switch (myIni.Get("CommScript", $"display{i}").ToString())
                        {
                            case "status":
                                sprite = MySprite.CreateText(StatusText(), "Monospace", new Color(179, 237, 255), size * 1.20f, TextAlignment.CENTER);
                                break;
                            default:
                                sprite = MySprite.CreateText(MessageText(), "Debug", new Color(179, 237, 255), size * 0.9f, TextAlignment.CENTER);
                                break;
                        }
                        sprite.Position = surface.SurfaceSize * 0.5f - new Vector2(0, (surface.SurfaceSize * 0.15f).Y);
                        frame.Add(sprite);
                    }
                }
            }
        }
    }
}

public string EchoText()
{
    string echoText;
    echoText = "Comm system: ";
    if (antenna.Enabled)
        echoText += "Online";
    else
        echoText += "Offline";

    if (DateTime.Now.Second % 2 == 0)
        echoText += " [#  ]\n";
    else
        echoText += " [  #]\n";

    echoText += $"Vessel: {ShipName()}\n\n";

    for (int i = 0; i < 9; i++)
    {
        echoText += messageList[i] + "\n";
    }

    return echoText;
}

public string MessageText()
{
    string messageText;
    messageText = "Communications:\n\n";
    for (int i = 0; i < 10; i++)
    {
        messageText += messageList[i] + "\n";
    }
    return messageText;
}

public string StatusText()
{
    string statusText;
    statusText = "Comm system: ";
    if (antenna.Enabled && antenna.IsFunctional)
        statusText += "Online\n\n";
    else
        statusText += "Offline\n\n";

    statusText += $"Vessel:\n{ShipName()}\n\n";

    return statusText;
}

public void UpdateMessages(string message = "")
{
    if (message != "")
    {
        for (int i = 9; i > 0; i--)
        {
            messageList[i] = messageList[i - 1];
        }
        messageList[0] = $"{DateTime.Now.ToString("HH:mm")} {message}";
    }

    UpdateTextSurfaces();
}

public void SelfDestruct(string mode)
{
    List<IMyWarhead> warheads = new List<IMyWarhead>();
    GridTerminalSystem.GetBlocksOfType(warheads);
    Random r = new Random();
    switch (mode)
    {
        case "engage":
            if (warheads.Count == 0)
            {
                UpdateMessages("No warheads found, Self-destruct not possible!");
                return;
            }

            selfDestructEnabled = true;
            foreach (var warhead in warheads)
            {
                warhead.DetonationTime = r.Next(5950, 6050) / 100f;
                warhead.StartCountdown();
            }

            string output =
                "######################" +
                "\nSELF DESTRUCT STARTED!" +
                "\nYou have 1 minute to abandone the ship" +
                "\n######################";
            UpdateMessages(output);
            break;
        case "disengage":
            selfDestructEnabled = false;
            foreach (var warhead in warheads)
            {
                warhead.StopCountdown();
                warhead.DetonationTime = r.Next(2910, 3100) / 100f;
            }
            UpdateMessages("Self destruct CANCELLED!");
            break;
    }
}

public string ShipName()
{
    string[] array = null;
    array = Me.CubeGrid.CustomName.Split(' ');
    return String.Concat(array);
}

public bool IsControlledBlock(IMyTerminalBlock block)
{
    if (block.CustomName.Contains(selectorString) && block.CubeGrid == Me.CubeGrid)
        return true;
    else
        return false;
}

public class TSProvider
{
    public IMyTerminalBlock block { get; }
    public bool isProvider { get; }
    public int surfaceCount { get; }

    public TSProvider(IMyTerminalBlock block, bool isProvider, int surfaceCount = 0)
    {
        this.block = block;
        this.isProvider = isProvider;
        this.surfaceCount = surfaceCount;
    }
}

readonly IMyRadioAntenna antenna = null;
MyIni myIni = new MyIni();

bool selfDestructPrompt = false;
string[] messageList = new string[10];
bool selfDestructEnabled = false;

List<TSProvider> tsProviders = new List<TSProvider>();
List<IMyTextPanel> commTextPanels = new List<IMyTextPanel>();
List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
