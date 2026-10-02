public Program() {
    // Set to run frequently so the exhaust responds quickly when the refinery starts or stops
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource) {
    // Locate the specific refinery
    IMyRefinery refinery = GridTerminalSystem.GetBlockWithName("My Refinery A") as IMyRefinery;

    // Locate the specific exhaust block
    IMyFunctionalBlock exhaust = GridTerminalSystem.GetBlockWithName("My Exhaust A") as IMyFunctionalBlock;

    // Safety check to make sure both blocks exist on the grid
    if (refinery == null || exhaust == null) {
        Echo($"Error: Missing blocks!\nRefinery 'My Refinery A': {(refinery != null ? "FOUND" : "NOT FOUND")}\nExhaust 'My Exhaust A': {(exhaust != null ? "FOUND" : "NOT FOUND")}");
        return;
    }

    // IsProducing is true only when the refinery is actively processing ore
    if (refinery.IsProducing) {
        if (!exhaust.Enabled) {
            exhaust.Enabled = true;
        }
        Echo("Status: My Refinery A is active.\nResult: My Exhaust A turned ON.");
    } else {
        if (exhaust.Enabled) {
            exhaust.Enabled = false;
        }
        Echo("Status: My Refinery A is idle.\nResult: My Exhaust A turned OFF.");
    }
}
// --- SOUND BLOCK ADD-ON FUNCTION ---
// Corrected to use .IsProducing for the refinery check
bool _wasSmeltingSoundTrack = false;

public void RunSoundBlockAddon(IMyRefinery refinery)
{
    IMyTerminalBlock soundBlock = GridTerminalSystem.GetBlockWithName("Sound Block A") as IMyTerminalBlock;
    
    if (soundBlock == null)
    {
        Echo("Sound Addon Error: 'Sound Block A' not found!");
        return;
    }

    // Space Engineers uses .IsProducing to check if a refinery is active
    if (refinery.IsProducing)
    {
        _wasSmeltingSoundTrack = true;
    }
    else
    {
        if (_wasSmeltingSoundTrack)
        {
            soundBlock.ApplyAction("Play");
            Echo("Sound Addon: Job Done alert triggered.");
        }
        _wasSmeltingSoundTrack = false;
    }
}