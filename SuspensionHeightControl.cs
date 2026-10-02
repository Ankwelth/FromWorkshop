float low  = -0.3f; // default
float high =  0.5f; // default
float currentTarget = 0.5f;

const float rate = 0.08f; // step per update (~1s)

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10; // 0.1s
}

public void Main(string argument, UpdateType updateSource)
{
    if (!string.IsNullOrWhiteSpace(argument))
    {
        var parts = argument.Split(' ');
        foreach (var part in parts)
        {
            var kv = part.Split('=');
            if (kv.Length == 2)
            {
                float value;
                if (float.TryParse(kv[1], out value))
                {
                    if (kv[0].ToLower() == "low")
                        low = value;
                    else if (kv[0].ToLower() == "high")
                        high = value;
                }
            }
        }
    }

    // Get cockpits
    var cockpits = new List<IMyCockpit>();
    GridTerminalSystem.GetBlocksOfType(cockpits, c => c.IsSameConstructAs(Me));
    bool occupied = cockpits.Any(c => c.IsUnderControl);

    // Check if any connector is connected to a static large grid (station)
    bool connectedToStation = false;
    var connectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(connectors, c => c.IsSameConstructAs(Me));

    foreach (var connector in connectors)
    {
        if (connector.Status == MyShipConnectorStatus.Connected)
        {
            var other = connector.OtherConnector;
            if (other != null && other.CubeGrid.IsStatic)
            {
                connectedToStation = true;
                break;
            }
        }
    }

    // Decide target only if NOT connected to station
    if (!connectedToStation)
        currentTarget = occupied ? low : high;
    else
        return; // skip adjusting wheels if docked to a station

    // Adjust wheels
    var wheels = new List<IMyMotorSuspension>();
    GridTerminalSystem.GetBlocksOfType(wheels, w => w.IsSameConstructAs(Me));

    foreach (var wheel in wheels)
    {
        float current = wheel.Height;

        if (Math.Abs(currentTarget - current) > 0.01f)
        {
            if (current < currentTarget)
                wheel.Height = Math.Min(current + rate, currentTarget);
            else
                wheel.Height = Math.Max(current - rate, currentTarget);
        }
    }
}
