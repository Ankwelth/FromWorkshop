// Space Engineers - Missile Launching Assistant
// By Arguswastaken

// Attach one or more missiles to your ship, rover or station using merge blocks, and make sure that they're
// equipped with a timer block that performs all the actions necessary to launch it (i.e. turn off merge block, enable thruster etc).

// You should make sure that these missile timer blocks have "[MLA]" included in its name. This is necessary so the program
// can detect them, and once you run the script it'll select the first one it detects and trigger it, launching one of your missiles.

// I made this script to eliminate the tedium of having to reset your hotbar actions after you reload a missile.
// The programmable block containing the script should stay on the main ship so it can find the timer blocks for you,
// and at least that way you won't need to reassign the action after every reload.

// Code

public void Main(string arguments)
{   
    Fire();
}

public void Fire()
{
    var gts = GridTerminalSystem;

    IEnumerable<IMyTerminalBlock> timers = GetGridBlocks().Where(x => x as IMyTimerBlock != null && x.CustomName.Contains("[MLA]"));
    if(timers.Any())
    {
        timers.ElementAt(0).ApplyAction("TriggerNow");
        Echo("Successfully fired a missile!");
    }
    else
    {
        Echo("Warning: No missiles found! Do your timer blocks have [MLA] in their name? If not, you have probably run out of missiles.");
    }
}

public List<IMyTerminalBlock> GetGridBlocks()
{
    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(blocks);

    return blocks;
}