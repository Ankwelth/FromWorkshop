const string MESSAGE_TAG = "JumpCoordsBroadcast";
private string jumpDriveName = "Jump Drive";

private IMyBroadcastListener listener;
private IMyJumpDrive jumpDrive;
private IMyShipController shipController;

public Program()
{
    listener = IGC.RegisterBroadcastListener(MESSAGE_TAG);
    listener.SetMessageCallback(MESSAGE_TAG);

    jumpDrive = GridTerminalSystem.GetBlockWithName(jumpDriveName) as IMyJumpDrive;
    if (jumpDrive == null)
    {
        var drives = new List<IMyJumpDrive>();
        GridTerminalSystem.GetBlocksOfType(drives);
        jumpDrive = drives.Count > 0 ? drives[0] : null;
    }

    // Initialize ship controller for position data
    shipController = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
    if (shipController == null)
    {
        var controllers = new List<IMyShipController>();
        GridTerminalSystem.GetBlocksOfType(controllers);
        shipController = controllers.FirstOrDefault();
    }

    Runtime.UpdateFrequency = UpdateFrequency.None; // Only run on IGC messages
}

public void Main(string argument, UpdateType updateSource)
{
    if ((updateSource & UpdateType.IGC) == 0)
    {
        Echo("Ignoring: Not triggered by IGC message.");
        return;
    }

    if (jumpDrive == null)
    {
        Echo("Error: No jump drive found.");
        return;
    }

    if (shipController == null)
    {
        Echo("Error: No ship controller found.");
        return;
    }

    while (listener.HasPendingMessage)
    {
        var msg = listener.AcceptMessage();
        if (!(msg.Data is MyTuple<string, Vector3D>))
        {
            Echo("Received invalid message data type. Ignoring.");
            continue;
        }

        var data = (MyTuple<string, Vector3D>)msg.Data;
        string cmd = data.Item1?.Trim();
        Vector3D senderPosition = data.Item2;

        if (string.IsNullOrWhiteSpace(cmd))
        {
            Echo("Received empty command. Ignoring.");
            continue;
        }

        Me.CustomData = cmd;

        if (cmd.Equals("abort", StringComparison.OrdinalIgnoreCase))
        {
            AbortJump();
            Me.CustomData = "";
        }
        else
        {
            bool jumped = Jump(cmd, senderPosition);
            if (jumped)
            {
                Me.CustomData = "";
            }
        }
    }
}

private bool Jump(string gpsString, Vector3D senderPosition)
{
    Echo("Parsing GPS...");

    if (jumpDrive.Status != MyJumpDriveStatus.Ready)
    {
        Echo("Jump drive not ready! Status: " + jumpDrive.Status);
        return false;
    }

    Vector3D gpsPosition;
    if (!TryParseGPS(gpsString, out gpsPosition))
    {
        Echo("Failed to parse GPS string.");
        return false;
    }

    // Calculate offset: difference between sender's position and receiver's position
    Vector3D receiverPosition = shipController.GetPosition();
    Vector3D offset = receiverPosition - senderPosition;

    // Apply offset to GPS position
    Vector3D targetPosition = gpsPosition + offset;

    Echo("Parsed GPS: " + gpsPosition.ToString());
    Echo("Offset: " + offset.ToString());
    Echo("Target with offset: " + targetPosition.ToString());

    jumpDrive.SetValue("ScriptJumpTarget", (Vector3D?)targetPosition);
    Echo("Target set. Executing jump...");
    jumpDrive.ApplyAction("ScriptJump");
    return true;
}

private void AbortJump()
{
    Echo("Aborting jump...");
    jumpDrive.ApplyAction("AbortJump");
}

private bool TryParseGPS(string argument, out Vector3D pos)
{
    pos = new Vector3D();

    string[] parts = argument.Split(':');
    if (parts.Length < 5)
        return false;

    if (!parts[0].Equals("GPS", StringComparison.OrdinalIgnoreCase))
        return false;

    var culture = System.Globalization.CultureInfo.InvariantCulture;
    double x, y, z;

    if (!double.TryParse(parts[2], System.Globalization.NumberStyles.Float, culture, out x)) return false;
    if (!double.TryParse(parts[3], System.Globalization.NumberStyles.Float, culture, out y)) return false;
    if (!double.TryParse(parts[4], System.Globalization.NumberStyles.Float, culture, out z)) return false;

    pos = new Vector3D(x, y, z);
    return true;
}