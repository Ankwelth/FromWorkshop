List<IMyMotorStator> rotors = new List<IMyMotorStator>(); // Use a typed list, and reuse.

public Program()
{
    // Try to do as much as you can at construction so that you dont need to do as much at runtime.
    IMyBlockGroup blocks = GridTerminalSystem.GetBlockGroupWithName("rotors");
    if (blocks != null)
    {
        rotors.Clear();
        blocks.GetBlocksOfType(rotors);
    }
}

public void Main(string argument, UpdateType updateSource)
{
    //float desiredDisplacement = float.Parse(argument); // This will crash if argument isn't a float
    // Instead try this:
    float desiredDisplacement;
    if (!float.TryParse(argument, out desiredDisplacement))
    {
        return; // Early exit if arg is not float, dont crash.
    }

    foreach (var rotor in rotors)
    {
        // Prefer interface properties to terminal actions
        rotor.Displacement = desiredDisplacement;
    }

    // Lets add a debug print so we know stuff is working :)
    Echo($"Set {rotors.Count} rotors to displacement of {desiredDisplacement}");
}