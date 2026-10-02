List<IMyThrust> thrusters = new List<IMyThrust>();
List<IMyCockpit> controlSeats = new List<IMyCockpit>();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10; // Run the script every 100ms
    Initialize();
}

public void Initialize()
{
    thrusters.Clear();
    controlSeats.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters, thruster => thruster.CubeGrid == Me.CubeGrid);
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(controlSeats, seat => seat.CubeGrid == Me.CubeGrid);
    Echo(controlSeats.Count > 0 ? "Found control seats." : "Warning: No control seats found.");

    // Set initial update frequency based on whether you're in the control seat
    if (controlSeats.Any(seat => seat.IsUnderControl))
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }
    else
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }
}


public void Main(string argument, UpdateType updateSource)
{
    // Determine if thrusters should be on and control them accordingly
    ControlThrusters();
}

private void ControlThrusters()
{
    bool isInControlSeat = controlSeats.Any(seat => seat.IsUnderControl);
    Vector3D velocity = controlSeats.Count > 0 ? controlSeats[0].GetShipVelocities().LinearVelocity : Vector3D.Zero;
    double speed = velocity.Length();

    // Define the speed threshold for hydrogen thrusters
    const double hydrogenSpeedThreshold = 25.0; // Speed in m/s

    foreach (var thruster in thrusters)
    {
        // Check if the thruster is a hydrogen thruster by checking its subtype ID
        bool isHydrogenThruster = thruster.BlockDefinition.SubtypeName.Contains("Hydrogen");

        if (isHydrogenThruster)
        {
            // Control hydrogen thrusters based on the speed threshold
            thruster.Enabled = speed > hydrogenSpeedThreshold && isInControlSeat;
        }
        else
        {
            // Control other types of thrusters normally
            thruster.Enabled = isInControlSeat || speed > 0.01;
        }
    }

    // Display the status
    Echo($"Thrusters: {(isInControlSeat ? "ON" : "OFF")}\nSpeed: {speed:0.00} m/s\nIn control seat: {(isInControlSeat ? "YES" : "NO")}");
}
