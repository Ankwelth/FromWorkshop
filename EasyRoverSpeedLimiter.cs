// Speed limit rover script for Space Engineers
// Default speed is 45 m/s
// The cockpit name is "Rover Cockpit [Driver]"
// Toggle speed limit with "setspeed" or set a new speed with "setspeed <number>"

public Program()
{
    // Set the script to update every 100 ticks (roughly 1.67 seconds)
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

// Variable to track whether speed limit is enabled
bool speedLimitEnabled = true;
float speedLimit = 45f;

public void Main(string argument, UpdateType updateSource)
{
    // Get the cockpit block named "Rover Cockpit [Driver]"
    var cockpit = GridTerminalSystem.GetBlockWithName("Rover Cockpit [Driver]") as IMyCockpit;

    if (cockpit == null)
    {
        Echo("Cockpit not found!");
        return;
    }

    // Get current velocity from the cockpit
    Vector3D velocity = cockpit.GetShipVelocities().LinearVelocity;
    double currentSpeed = velocity.Length(); // Get speed in m/s

    // Handle "setspeed" argument
    if (!string.IsNullOrEmpty(argument) && argument.ToLower().StartsWith("setspeed"))
    {
        string[] args = argument.Split(' ');
        float newSpeed = speedLimit; // Assign a default value to prevent unassigned variable error

        if (args.Length > 1 && float.TryParse(args[1], out newSpeed))
        {
            speedLimit = newSpeed;
            speedLimitEnabled = true;
            Echo($"Speed limit set to: {speedLimit} m/s");
        }
        else
        {
            speedLimitEnabled = !speedLimitEnabled;
            Echo(speedLimitEnabled ? $"Speed limit enabled: {speedLimit} m/s" : "Speed limit disabled (max speed allowed).");
        }
    }

    // Apply speed limit if enabled
    SetSpeedLimit(speedLimitEnabled ? speedLimit : float.MaxValue);

    Echo($"Current Speed: {Math.Round(currentSpeed, 2)} m/s");
}

private void SetSpeedLimit(float speed)
{
    // Get all wheels in the grid
    var wheels = new List<IMyMotorSuspension>();
    GridTerminalSystem.GetBlocksOfType(wheels);

    foreach (var wheel in wheels)
    {
        wheel.SetValue("Speed Limit", speed); // Set speed limit for each wheel
    }
}
