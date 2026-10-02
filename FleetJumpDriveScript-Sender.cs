const string MESSAGE_TAG = "JumpCoordsBroadcast";
private const string jumpDriveName = "Jump Drive";

private IMyJumpDrive jumpDrive;
private IMyShipController shipController;

public Program()
{
    jumpDrive = GridTerminalSystem.GetBlockWithName(jumpDriveName) as IMyJumpDrive;
    if (jumpDrive == null)
    {
        var drives = new List<IMyJumpDrive>();
        GridTerminalSystem.GetBlocksOfType(drives);
        jumpDrive = drives.FirstOrDefault();
    }

    // Initialize ship controller for position data
    shipController = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
    if (shipController == null)
    {
        var controllers = new List<IMyShipController>();
        GridTerminalSystem.GetBlocksOfType(controllers);
        shipController = controllers.FirstOrDefault();
    }

    Runtime.UpdateFrequency = UpdateFrequency.None; // No auto-run; only when explicitly triggered
}

public void Main(string argument, UpdateType updateSource)
{
    argument = (argument ?? "").Trim();

    if (argument.Equals("abort", StringComparison.OrdinalIgnoreCase))
    {
        Echo("Abort command received via argument.");
        IGC.SendBroadcastMessage(MESSAGE_TAG, new MyTuple<string, Vector3D>("abort", Vector3D.Zero));
        AbortJump();
        return;
    }

    if (!string.IsNullOrWhiteSpace(argument))
    {
        Echo("Ignored: Only 'abort' or blank argument is valid.");
        return;
    }

    string gpsData = Me.CustomData.Trim();

    if (string.IsNullOrWhiteSpace(gpsData))
    {
        Echo("CustomData is empty, nothing to send or jump.");
        return;
    }

    if (shipController == null)
    {
        Echo("Error: No ship controller found.");
        return;
    }

    // Get sender's current position
    Vector3D senderPosition = shipController.GetPosition();
    IGC.SendBroadcastMessage(MESSAGE_TAG, new MyTuple<string, Vector3D>(gpsData, senderPosition));
    Echo("Broadcasted jump command with position.");

    if (jumpDrive == null)
    {
        Echo("No jump drive found on this grid!");
        return;
    }

    if (gpsData.Equals("abort", StringComparison.OrdinalIgnoreCase))
    {
        AbortJump();
    }
    else
    {
        Jump(gpsData);
    }
}

private void Jump(string argument)
{
    Echo("Parsing GPS...");
    if (jumpDrive.Status != MyJumpDriveStatus.Ready)
    {
        Echo("Jump drive not ready! Status: " + jumpDrive.Status.ToString());
        return;
    }

    Vector3D pos;
    if (TryParseGPS(argument, out pos))
    {
        Echo("Parsed target: " + pos.ToString());
        jumpDrive.SetValue("ScriptJumpTarget", (Vector3D?)pos);
        Echo("Target set. Sending ScriptJump action...");
        jumpDrive.ApplyAction("ScriptJump");
    }
    else
    {
        Echo("Failed to parse GPS string.");
    }
}

private void AbortJump()
{
    Echo("Aborted jump.");
    jumpDrive.ApplyAction("AbortJump");
}

private bool TryParseGPS(string argument, out Vector3D pos)
{
    pos = new Vector3D();

    string[] parts = argument.Trim().Split(':');
    if (parts.Length < 5 || !parts[0].Equals("GPS", StringComparison.OrdinalIgnoreCase))
        return false;

    double x, y, z;
    var culture = System.Globalization.CultureInfo.InvariantCulture;

    if (!double.TryParse(parts[2], System.Globalization.NumberStyles.Float, culture, out x)) return false;
    if (!double.TryParse(parts[3], System.Globalization.NumberStyles.Float, culture, out y)) return false;
    if (!double.TryParse(parts[4], System.Globalization.NumberStyles.Float, culture, out z)) return false;

    pos = new Vector3D(x, y, z);
    return true;
}