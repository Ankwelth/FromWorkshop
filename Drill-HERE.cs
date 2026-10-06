/*  -= Drill=>HERE! =- */

/* WHAT IS THIS?
 * see the description on the Steam Workshop https://steamcommunity.com/sharedfiles/filedetails/?id=3296158808
 */

/* WHAT's NEW?
 * VERSION vTC1.1.2
 * bug fix for argument
 * VERSION vTC1.1.1
 * Fixed an issue with multiple LCDs
 * VERSION vTC1.1.0
 * Added ability to get "ORBIT HERE" data by adding a 4th part to the argument
 * VERSION vTC1.0.0
 * Initial release of Drill=>HERE!
*/

/* HOW IT WORKS  
 
Drill=>HERE! can be used from any point there is planet gravity - from any ship and at any ship angle, etc.

It is recommended you run this script in an 'explorer' type ship - one that you can use to easily view GPS points on the planet, 
so it is highly recommended you have a 'balanced' ship - aside from your mothership/drone(s), etc.

You probably want a ship that can "stop anywhere at any angle" 
(i.e., it has been designed with enough thrusters of the right type to 'hold' at any point you expect to scan from)

Your ship MUST have:
a. Cockpit (this can likely be used with drone/Cameras {UNTESTED/UNSUPPORTED/FUTURE?}, but it is much simpler to use a Cockpit)
b. Remote Control (even though you will be flying this ship to do your scans, the RC IS required to access the 'internal' software!)
c. LCD (it can be a 'corner' or something small {cockpit screens are a possible future feature})
*/

/* SETUP 
1. PROGRAMMABLE BLOCK (PB) - install this script (you probably downloaded it from the Workshop and this is already done - CONGRATS!)
2. Install and setup your Drill=>HERE! LCD.  It does not have to be accessible other than through the Control Panel 
   (so you can 'hide' it if you want)
   The only thing that is required is that the LCD Name or the Custom Data of the LCD MUST contain "Drill=>HERE!" (quotes are not required)
3. If you are going to use 'flying' miners, in the USER CONFIGURATION below set the drillpad (this places the GPS point above the EST. SURFACE point)
4. Follow the instructions on the Workshop to use!
 */

/* USE
 * Set the variable 'drillpad' below for your flying miner point (this is likely a standard for your miners so it set in the code)
 * Align your explorer with the ore you want to tag (crosshairs over the 'box' means you are aligned properly)
 * Go to the 'Argument' part of the PB and enter "Drill ORENAME DISTANCE ORBITDISTANCE" (note that ORBITDISTANCE is optional)
 * Click 'Run'
 * View your LCD via 'Edit Text'
 * Go to your GPS tab and select the ones you want to see by clicking 'Show on HUD'
 * Use your new GPS points as you see fit!
 */

/* EXPECTED OUTPUTS
1. Position of the ORE
2. EST. SURFACE position just above the ORE (i.e., put your drill rig here - you WILL hit the ore you want!)  
   NOTE: this is an ESTIMATED surface point, calculated from the surface point below your original ship.
         Therefore if your ship is at a different elevation than the elevation above the ore (hills/valleys and such), it may not be on the 'surface'!
3. DRILL HERE! where you can send a flying miner and drill STRAIGHT DOWN for a guaranteed 'payday'!
4. If you add a 4th number to the 'Argument' before clicking 'Run', you will get an ORBIT GPS point to choose to show

(these are all available in your GPS tab after running Drill=>HERE!  
You can choose to see any of them that you like by selecting the point and clicking the 'Show on HUD' option.)
 */

/* USER CONFIGURATION */


double drillpad = 100; // METERS up from the surface you want to have your drill start (should be high enough to compensate for Center Of Mass to bottom of ship drills - better 'safe' than 'sorry'!)

// NO USER CONFIGURABLE PARTS BELOW THIS LINE!
public static readonly string scriptname = "Drill=>HERE!";
public static readonly string VERSION = "vTC1.1.2";
DateTime updateTime = DateTime.Now;
int updateTimespan = 1000;
string workingIndicator = "/";
int workingCounter = 0;
List<IMyTerminalBlock> temp = new List<IMyTerminalBlock>();
IMyTextPanel drillerlcd;
bool checkok = false;
public static readonly string lcdname = scriptname;
StringBuilder terminallinebuilder = new StringBuilder();
StringBuilder lcdlinebuilder = new StringBuilder();
MyIni _ini = new MyIni();
double tocenter = 0;
IMyRemoteControl remoteControl;
IMyRemoteControl RC;
private Vector3D planetCenter;
double altitude = 0;
string[] item;
MyPlanetElevation elevation = MyPlanetElevation.Surface;
List<IMyShipController> shipController = new List<IMyShipController>();
public Program()
{
    GridTerminalSystem.GetBlocksOfType<IMyShipController>(shipController, c => c.CubeGrid == Me.CubeGrid  && c.IsWorking);
    MyIniParseResult result;
    if (!_ini.TryParse(Me.CustomData, out result))
        throw new Exception(result.ToString());
    Setup();
    IMyTextSurface surface;
    surface = Me.GetSurface(0);
    surface.ContentType = ContentType.TEXT_AND_IMAGE;
    surface.Alignment = TextAlignment.CENTER;
    surface.TextPadding = 25;
    surface.FontSize = 5;
    surface.WriteText(scriptname + "\n" + VERSION + "\n");
    Me.CustomData = _ini.ToString();
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}
void Main(string arg, UpdateType updateSource)
{
    UpdateWorkingCounter();
    if (updateSource.HasFlag(UpdateType.Terminal))
    {
        Setup();
    }
    else {
        CheckSetup();
    }
    if (checkok && arg != "")
    {
        terminallinebuilder.Clear();
        item = arg.Split(' ');
        int itemLength = item.Length;
        if ((itemLength == 3 || itemLength == 4) && item[0]== "Drill")
        {
            Process(arg);  
            drillerlcd.WriteText(lcdlinebuilder.ToString());
            lcdlinebuilder.Clear();
        }
        else
        {
            terminallinebuilder.AppendLine("You need to use a command like 'Drill Gold 150 - see docs!'");
        }
    }
    Echo("\n" + scriptname + " Version " + VERSION + " " + workingIndicator + "\n");
    Echo(terminallinebuilder.ToString());
    if (!checkok) { terminallinebuilder.Clear(); }
}
private void CheckSetup()
{
    if (drillerlcd != null)
    {
        var panelName = panelHasName(drillerlcd, lcdname);
        checkok = panelName != "";
        if(!checkok) { terminallinebuilder.AppendLine("NO " + lcdname + " LCD found!\n"); }
    }
    else { checkok = false; terminallinebuilder.AppendLine("NO " + lcdname + " LCD found!\n"); }
}
void UpdateWorkingCounter()
{
    if ((DateTime.Now - updateTime).TotalMilliseconds > updateTimespan)
    {
        updateTime = DateTime.Now;
        workingCounter = workingCounter >= 3?0: workingCounter + 1;
    }
    switch (workingCounter % 4)
    {
        case 0: workingIndicator = "/"; break;
        case 1: workingIndicator = "-"; break;
        case 2: workingIndicator = "\\"; break;
        case 3: workingIndicator = "|"; break;
    }
}
public static double ConvertToDouble(String input)
{
    String inputCleaned = System.Text.RegularExpressions.Regex.Replace(input, "[^0-9]", "");
    double value = 0;
    if (double.TryParse(input, out value))
    {
        return Math.Round(value,0);
    }
    return 0;
}
private string panelHasName(IMyTextPanel panel, string test)
{
    if (panel.CustomName.Contains(test) || panel.CustomData.Contains(test)) { return test; }
    return "";
}
private void Setup()
{
    List<IMyTextPanel> panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels);
    foreach (IMyTextPanel panel in panels)
    {
        if (!panel.IsSameConstructAs(Me)) { continue; }
        string panelName = "";
        panelName = panelHasName(panel, lcdname);
        if (panelName != "")
        {
            drillerlcd = panel;
            drillerlcd.ContentType = ContentType.TEXT_AND_IMAGE;
            drillerlcd.FontSize = 1;
            drillerlcd.WriteText("");
            continue;
        }
    }
    if (drillerlcd == null)
    {
        terminallinebuilder.AppendLine("To use " + scriptname + " you MUST have a properly named LCD!");
        return;
    }
    List<IMyRemoteControl> remoteControls = new List<IMyRemoteControl>();
    GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(remoteControls);
    if (remoteControls.Count > 0)
    {
        remoteControl = RC = remoteControls[0];
    }
    else
    {
        terminallinebuilder.AppendLine("To use " + scriptname + " you MUST have a Remote Control!");
    }
}

void getAltitude()
{
    altitude = 0;
    if (RC != null)
    {
        RC.TryGetPlanetElevation(elevation, out altitude);
    }
    if (double.IsInfinity(altitude))
    {
        altitude = 0;
    }
    if (RC.TryGetPlanetPosition(out planetCenter))
    {
        Vector3D shipPosition = RC.GetPosition();
        tocenter = (shipPosition - planetCenter).Length();
    }
}
private string buildGPS(string orename,Vector3D v3d, string textstring)
{
    return "GPS:Drill HERE " + orename + " " + textstring + v3d.ToString().Replace("X", "").Replace(" Y", "").Replace(" Z", "") + ":#F1BA75";
}
public static int ConvertToInt(String input)
{
    String inputCleaned = System.Text.RegularExpressions.Regex.Replace(input, "[^0-9]", "");
    int value = 0;
    if (int.TryParse(inputCleaned, out value))
    {
        return value;
    }
    return 0;
}
private void Process(string arg)
{
    Vector3D RCPosition = RC.GetPosition();
    if (RC.TryGetPlanetPosition(out planetCenter))
    {
        string orename = item[1]; 
        double xNext = ConvertToDouble(item[2]) ;
        Vector3D forwardHeading = RC.WorldMatrix.Forward;
        Vector3D orePosition = RC.GetPosition() + forwardHeading * xNext;
        lcdlinebuilder.AppendLine(buildGPS(orename, orePosition, "ORE"));
        getAltitude();
        double estimatedRadius = tocenter - altitude;
        Vector3D firstPos = planetCenter;
        Vector3D secondPos = orePosition;
        double distance = Vector3D.Distance(firstPos, secondPos);
        double extDist = estimatedRadius - distance;
        double scaledExtDist = (extDist) / distance;
        Vector3D newPos = Vector3D.Lerp(firstPos, secondPos, 1 + scaledExtDist);
        lcdlinebuilder.AppendLine(buildGPS(orename, newPos, "EST. SURFACE"));
        if (item.Length > 3)
        {
            if (item[3] != null)
            {
                double xOrbit = ConvertToDouble(item[3]);
                Vector3D newOrbit = Vector3D.Lerp(firstPos, secondPos, 1 + scaledExtDist + ((Double)drillpad / distance) + (xOrbit / distance));
                lcdlinebuilder.AppendLine(buildGPS(orename, newOrbit, "ORBIT HERE!"));
            }
        }
        Vector3D newDrill = Vector3D.Lerp(firstPos, secondPos, 1 + scaledExtDist + ((Double)drillpad / distance));
        lcdlinebuilder.AppendLine(buildGPS(orename, newDrill, "DRILL HERE!"));
        terminallinebuilder.AppendLine("Processed " + arg);
    }
    else
    {
        terminallinebuilder.AppendLine("You must be within a planet's gravity well to use this script");
    }
}