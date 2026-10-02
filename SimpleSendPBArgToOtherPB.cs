List<TerminalActionParameter> args = new List<TerminalActionParameter>();
public void Main(string argument)
{
    var otherPb = GridTerminalSystem.GetBlockWithName("HERE write your target programmable bloc name with quotes");
    var originalArg = argument;
  
    Echo(Me.CustomName);
    args.Clear();
    args.Add(TerminalActionParameter.Get(originalArg));
    otherPb.ApplyAction("Run", args);
}