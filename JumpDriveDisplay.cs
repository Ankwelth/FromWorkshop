
//======================================================================= 
//////////////////////////BEGIN////////////////////////////////////////// 
//======================================================================= 

//Jump Power Script - uses LCD's and Lights to show the current charge status of all jump drives 
//As well as the time for them to be fully recharged 

/* 
***Use Instructions*** 
1. Place jump drives onto ship 
2. Place lights and LCD's - Wide or Standard work with this script - Add #JUMPLCD to the Custom Data of each (this tag can be changed below) 
3. Ensure that the LCD's are set to show "Text and Images" otherwise they will not display the information 
4. Run the script 
*/

//NOTE - If you wish to ignore a Jump Drive add the tag #JUMPLCD_IGNORE to its custom data (this tag can be changed below) 
//NOTE - If you add / remove any of the elements in the script you must recompile the script for it to detect the new blocks 


//======================================================================= 
//EDIT THE TAG TO IGNORE A JUMPDRIVE FROM THE SYSTEM HERE 
String ignoreTag = "#JUMPLCD_IGNORE";
//EDIT THE TAG REQUIRED IN THE CUSTOM DATA OF SCREENS AND LIGHTS FOR THEM TO BE INCLUDED IN THE SYSTEM 
String addTag = "#JUMPLCD";
//======================================================================= 

//!!====================================================================!! 
//    CODE BELOW - DON'T MESS WITH UNLESS YOU KNOW WHAT YOU'RE DOING! 
//!!====================================================================!! 













//Initialise objects
List<IMyJumpDrive> drives = new List<IMyJumpDrive>();
List<IMyTextPanel> screens = new List<IMyTextPanel>();
List<IMyInteriorLight> lights = new List<IMyInteriorLight>();
const decimal fullPower = 30000000;
Dictionary<IMyJumpDrive, decimal> drivePower = new Dictionary<IMyJumpDrive, decimal>();
Dictionary<IMyJumpDrive, String> driveTime = new Dictionary<IMyJumpDrive, String>();
private int count = 0;
//Constructor - populates lists on initial run
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(lights);
    GridTerminalSystem.GetBlocksOfType<IMyJumpDrive>(drives);
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(screens);
    for (int i = drives.Count - 1; i >= 0; i--)
    {
        IMyJumpDrive drive = drives[i];
        if (drive.CustomData.Contains(ignoreTag)) drives.Remove(drive);
    }
    Echo("Drives Loaded");
    Echo(drives.Count + "");
    for (int i = screens.Count - 1; i >= 0; i--)
    {
        IMyTextPanel screen = screens[i];
        if (!screen.CustomData.Contains(addTag)) screens.Remove(screen);
    }
    Echo("Screens Loaded");
    Echo(screens.Count + "");
    for (int i = lights.Count - 1; i >= 0; i--)
    {
        IMyInteriorLight light = lights[i];
        if (!light.CustomData.Contains(addTag)) lights.Remove(light);
    }
    Echo("Lights Loaded");
    Echo(lights.Count + "");
}
//Main method that runs the program
public void Main(string argument, UpdateType updateSource)
{
    foreach (IMyJumpDrive drive in drives)
    {
        string storedRow = drive.DetailedInfo.ToString().Split('\n')[4];
        string power = storedRow.Split(':')[1];
        string timeRemaining = drive.DetailedInfo.ToString().Split('\n')[5];
        decimal totalPower = GetNum(power);
        drivePower[drive] = totalPower;
        driveTime[drive] = timeRemaining;
    }
    //If count reaches max value reset it to 0
    WriteToScreens();
    count+=2;
    if (count >= drives.Count - 1) count = 0;
}
private void WriteToScreens()
{
    foreach (IMyTextPanel screen in screens)
    {
        int percentageMultiplier = 40;
        String jumpOutput = "";
        String screenType = screen.DetailedInfo.ToString().Split('\n')[0];
        if (screenType.Contains("Corner"))
        {
            PrintCornerScreen(screen);
            continue;
        }
        if (screenType.Contains("Wide")) percentageMultiplier = 100;
        foreach (IMyJumpDrive drive in drives)
        {
            jumpOutput += drive.CustomName + ":  ";
            int percentageCharged = (int)(drivePower[drive] / fullPower * 100);
            int ratioCharge = (int)(drivePower[drive] / fullPower * percentageMultiplier);
            jumpOutput += percentageCharged + "% Charged  ";
            if (percentageMultiplier < 100) jumpOutput += "\n";
            jumpOutput += driveTime[drive] + "\n\n[";
            for (int n = 0; n <= ratioCharge; n++)
            {
                jumpOutput += "I";
            }
            for (int n = 0; n <= percentageMultiplier - ratioCharge; n++)
            {
                jumpOutput += " ";
            }
            jumpOutput += "]\n\n";
        }
        screen.WriteText(jumpOutput);
    }
    foreach (IMyInteriorLight light in lights)
    {
        light.Color = Color.Red;
        foreach (IMyJumpDrive drive in drives)
        {
            if (drivePower[drive] == fullPower) light.Color = Color.Green;
        }
    }
}
private void PrintCornerScreen(IMyTextPanel screen)
{
    IMyJumpDrive drive1 = drives[count];
    IMyJumpDrive drive2 = drives[count + 1];
    String jumpOutput = "";
    jumpOutput += drive1.CustomName + ":  ";
    int percentageCharged = (int)(drivePower[drive1] / fullPower * 100);
    jumpOutput += percentageCharged + "% Charged  ";
    if (drive2 != null)
    {
        jumpOutput += "\n";
        jumpOutput += drive2.CustomName + ":  ";
        percentageCharged = (int)(drivePower[drive2] / fullPower * 100);
        jumpOutput += percentageCharged + "% Charged  ";
    }
    screen.WriteText(jumpOutput);
}
//Removes all non-number elements from a string
private Decimal GetNum(string input)
{
    const string regExpr = @"(?<Num>[0-9.]+) (?<Unit>[a-zA-Z]+)";
    var match = System.Text.RegularExpressions.Regex.Match(input, regExpr);
    if (!match.Success)
        throw new Exception("Input has an invalid format");
    return Decimal.Parse(match.Groups["Num"].Value) * UnitToDecimal(match.Groups["Unit"].Value);
}
//Converts string to decimal
private Decimal UnitToDecimal(string unit)
{
    if (unit.StartsWith("k"))
    {
        return new Decimal(10e3);
    }
    if (unit.StartsWith("M"))
    {
        return new Decimal(10e6);
    }
    return new Decimal(0);
}