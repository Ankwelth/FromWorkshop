

public Program() { }

public void Main(string argument, UpdateType updateSource)
{
    if(argument.Length < 1)
    {
        Echo("Please provide a prefix in the argument text box.");
        return;
    }

    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => b.CubeGrid == Me.CubeGrid);
    string prefix = argument + " - ";
    int count = 0;
    foreach (IMyTerminalBlock block in blocks)
    {
        block.CustomName = prefix + block.DefinitionDisplayNameText;
        count++;
    }
    Echo(count + " Block(s) renamed!");
}