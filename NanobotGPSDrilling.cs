/* Nanobot GPS Drilling */

/* Overview: This script makes it quick and easy to position the work area of
   the Nanobot Drill and Fill System at a GPS point.

   Instructions:
   1. Add one (or more) Nanobot Drill and Fill systems to a grid.
   2. Name the Group of Nanobot system(s) you want to contol as "Nanobot Drills". (without the quotes)
      (Optionally you can edit the nanobotSystemBlockName variable below and use whatever name you want.)
   3. Create a GPS point where you want to locate the work area. 
      (PRO TIP: Use my Drill=>HERE! script to get GPS coordinates FAST!)
   4. Copy the GPS data to the clipboard.
   5. Go to the programmable block (PB) and paste the GPS data from the clipboard into the Argument field.
   6. Click Run.

   The work area should now be centered at the GPS point. (+/- half a meter.)
*/

string nanobotSystemBlockName = "Nanobot Drills"; // Use this as the name of the Nanobot block Group.

// NO USER CONFIGURABLE PARTS BELOW THIS LINE!

bool SetupComplete = false;
public static readonly string scriptname = "Nanobot GPS Drilling";
public static readonly string VERSION = "vTC1.0.0";
DateTime updateTime = DateTime.Now;
int updateTimespan = 1000;
string workingIndicator = "/";
int workingCounter = 0;

public Program()
{
    Echo("\n" + scriptname + " Version " + VERSION + "\n");
    if (!SetupComplete)
    {
        Setup();
    }
    if (SetupComplete)
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }
    else
    {
        Runtime.UpdateFrequency = UpdateFrequency.None;
    }
}

List<IMyTerminalBlock> nanodrills = new List<IMyTerminalBlock>();

public void Setup()
{
    var group = GridTerminalSystem.GetBlockGroupWithName(nanobotSystemBlockName);
    if (group == null) {
        Echo("Error: Could not find Group with name " + nanobotSystemBlockName);
        return;
    }
    group.GetBlocksOfType<IMyTerminalBlock>(nanodrills, blk => blk.CubeGrid.IsSameConstructAs(Me.CubeGrid));
    if (nanodrills == null)
    {
        Echo("Error: Could not find Nanobots in Group " + nanobotSystemBlockName);
        return;
    }
    Echo("Setup complete");
    SetupComplete = true;
}
void UpdateWorkingCounter()
{
    if ((DateTime.Now - updateTime).TotalMilliseconds > updateTimespan)
    {
        updateTime = DateTime.Now;
        workingCounter = workingCounter >= 3 ? 0 : workingCounter + 1;
    }
    switch (workingCounter % 4)
    {
        case 0: workingIndicator = "/"; break;
        case 1: workingIndicator = "-"; break;
        case 2: workingIndicator = "\\"; break;
        case 3: workingIndicator = "|"; break;
    }
}
public void Main(string argument, UpdateType updateSource)
{
    UpdateWorkingCounter();
    Echo("\n" + scriptname + " Version " + VERSION + " " + workingIndicator + "\n");
    var group = GridTerminalSystem.GetBlockGroupWithName(nanobotSystemBlockName); 
    group.GetBlocksOfType<IMyTerminalBlock>(nanodrills, blk => blk.CubeGrid.IsSameConstructAs(Me.CubeGrid));
    if (nanodrills.Count == 0)
    {
        Echo("Error: Could not find Nanobots in Group " + nanobotSystemBlockName);
        return;
    }
    foreach (IMyTerminalBlock drillBlock in nanodrills)
    {
        Vector3D drillPos;
        Echo("found drill " + drillBlock.CustomName);
        drillPos = drillBlock.GetPosition();
        if (argument != "")
        {
            if (!argument.ToLower().Contains("gps"))
            {
                Echo("Error: GPS coordinate " + argument
                    + " could not be understood\nPlease input coordinates in the form\nGPS:[Name of waypoint]:[x]:[y]:[z]:");
                return;
            }
            string[] split_arg = argument.Split(':');
            if (split_arg.Length < 5)
            {
                Echo("Error: GPS coordinate " + argument
                + " could not be understood\nPlease input coordinates in the form\nGPS:[Name of waypoint]:[x]:[y]:[z]:");
                return;
            }
            else
            {
                string gpsPointName = split_arg[1];
                Vector3D gpsPoint;
                gpsPoint.X = StringToDouble(split_arg[2]);
                gpsPoint.Y = StringToDouble(split_arg[3]);
                gpsPoint.Z = StringToDouble(split_arg[4]);
                var dirVector = gpsPoint - drillPos;
                Vector3D bodyVector = Vector3D.TransformNormal(dirVector, MatrixD.Transpose(drillBlock.WorldMatrix));
                double H = 0, V = 0, F = 0;
                if (drillBlock.GetType().ToString().Contains("MyShipWelder"))
                {
                    Echo("Build and Repair Bot in Drill group!\n");
                }
                else if (drillBlock.GetType().ToString().Contains("MyShipDrill"))
                {
                    H = bodyVector.X * -1;
                    V = bodyVector.Y;
                    F = bodyVector.Z - 4.0;
                    Echo("Setting Drill and Fill " + drillBlock.CustomName + " to GPS coordinates\n");
                    drillBlock.SetValueFloat("Drill.AreaOffsetLeftRight", (float)H);
                    drillBlock.SetValueFloat("Drill.AreaOffsetUpDown", (float)V);
                    drillBlock.SetValueFloat("Drill.AreaOffsetFrontBack", (float)F);
                }
                else
                {
                    Echo("Error: " + drillBlock.GetType().ToString() + " is not a known type.");
                }
            }
        }
    }
}

double StringToDouble(string value)
{
    double n;
    bool isDouble = double.TryParse(value, out n);
    if (isDouble)
        return n;
    else
        return 0;
}