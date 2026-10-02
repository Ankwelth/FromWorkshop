float ArgDis;
int NOD;
double JDls,lsc,Distance;
long AJD,Total;
bool Active;
string ex;
IMyJumpDrive MJD;
public Program()
{
   Echo("Launching script...");
   Runtime.UpdateFrequency = UpdateFrequency.Update100;
   if(Setup())
   {Echo("Script Launched");Active = true;ex = "";}
}
public bool Setup()
{
    List<IMyJumpDrive> Drives = new List<IMyJumpDrive>();
    List<IMyJumpDrive> Main = new List<IMyJumpDrive>();
    GridTerminalSystem.GetBlocksOfType<IMyJumpDrive>(Drives);
    GridTerminalSystem.GetBlocksOfType<IMyJumpDrive>(Main, x => x.CustomName.Contains("Main"));
    if(Drives.Count == 0){ex ="No Jump Drives found\nPlease add a Jump driveto the grid\nThen run the argument 'Scan'";return false;}
    if(Main.Count == 0){ex ="No Main Jump Drive\nPlease add 'Main' to a Jump drive's name\nThen run the argument 'Scan'";return false;}
    
    MJD = Main[0];
    NOD = Drives.Count; //Number Of Drives
    AJD = 9600000000; //Kilometers
    Total = NOD * AJD;
    lsc = 299792.458;
    JDls = Total / lsc;
    return true;
}

public void Main(string argument, UpdateType updateSource)
{
    if(!Active){Echo($"Error : {ex}");}
    if(!string.IsNullOrWhiteSpace(argument))
    {
        if(argument.ToLower().Contains("scan")){Echo("Scanning");if(Setup()){Echo($"Main Jump Drive Found");Active = true;}}
        if(!Active){Echo($"Error : {ex}");return;}
        string[] args = argument.Split(';');
        if (args.Length == 2)
        {
            if(!float.TryParse(args[0], out ArgDis)){Echo($"Format should be 'Distance;Unit'");return;}
            if(args[1].ToLower()== "km"){if(ArgDis > Total){Echo($"{Format(ArgDis - Total,2)} Km too Far");return;}else{Distance = ArgDis / Total;}}
            if(args[1].ToLower()== "ls"){if(ArgDis > JDls){Echo($"{Format(ArgDis - JDls,2)} ls Too Far");return;}else{Distance = ArgDis / JDls;}}
            if(args[1].ToLower() != "km" && args[1].ToLower() != "ls"){Echo($"Error: {args[1]} is not valid\nValid Distances are km and ls");return;}
            MJD.JumpDistanceRatio = (float)Distance;
            Echo($"Jumpdistance: {Format(ArgDis,3)} {args[1]}\nJumpRatio : {Math.Round(Distance,3)}");
        }
    }
}
string Format(double N, int DP)
{
    if(N >= 1000000000){return(Math.Round(N / 1000000000,DP)).ToString() + "B";}
    if(N >= 1000000){return(Math.Round(N / 1000000,DP)).ToString() + "M";}
    if(N >= 1000){return(Math.Round(N / 1000,DP)).ToString() + "K";}
    return N.ToString();
}