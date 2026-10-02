// Stu's Wind Turbine Monitoring Script
// v1.0 May 2021
//

// Wind turbines' names should contain this tag for the script to find wind turbines to monitor.
const string TurbineNameTag = "Wind Turbine";

// LCD Name should contain this tag for the script to find LCDs to display on.
const string LCDNameTag = "Turbines LCD";

// If you add turbines or monitor LCDs, run the script with the argument 'setup' (without the quotes).

// Probably not much to change below here.

// The list of turbines to be monitored
List<IMyPowerProducer> _turbines = new List<IMyPowerProducer>();

// The list of LCDs to be displayed on
List<IMyTextPanel> _outputLCDs = new List<IMyTextPanel>();

// Function used to sort the turbine list by their custom names
int TurbineComparer (IMyPowerProducer x, IMyPowerProducer y)
{
    return x.CustomName.CompareTo(y.CustomName);
}

void init()
{
    // Find the turbines
    GridTerminalSystem.GetBlocksOfType(_turbines, t => t.CustomName.Contains(TurbineNameTag));

    // Find the LCDs
    GridTerminalSystem.GetBlocksOfType(_outputLCDs, t => t.CustomName.Contains(LCDNameTag));

    // Initialize the LCDs
    foreach (var lcd in _outputLCDs)
    {
        lcd.Font = "Monospace";
        lcd.FontSize = 0.666F;
        lcd.ContentType = ContentType.TEXT_AND_IMAGE;
    }

    // Sort the turbine list for sensible display
    _turbines.Sort(TurbineComparer);

    // Dump the number of discovered turbines and LCDs to the prog block detailed-info area.
    Echo (String.Format("Found {0} wind turbines", _turbines.Count));
    Echo (String.Format("Found {0} LCDs", _outputLCDs.Count));
}

public Program()
{
    // The constructor, called only once every session and
    // always before any other method is called. Use it to
    // initialize your script. 
    //     
    // The constructor is optional and can be removed if not
    // needed.
    // 
    // It's recommended to set RuntimeInfo.UpdateFrequency 
    // here, which will allow your script to run itself without a 
    // timer block.

    init();
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}


public void Save()
{
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means. 
    // 
    // This method is optional and can be removed if not
    // needed.
}

public void Main(string argument, UpdateType updateSource)
{
    // The main entry point of the script, invoked every time
    // one of the programmable block's Run actions are invoked,
    // or the script updates itself. The updateSource argument
    // describes where the update came from.
    // 
    // The method itself is required, but the arguments above
    // can be removed if not needed.

    // Detect user request to rediscovered turbines and LCDs    
    if (argument.ToLower() == "setup")
    {
        init();
    }

    // Build a string of the entire text output
    // and then output that string to all screens
    string s2 = "";   
    
    // Gather info from all turbines
    foreach (var turbine in _turbines)
    {
        // Get the current and max outputs
        float currentOutput = turbine.CurrentOutput * 1000;
        float maxOutput = turbine.MaxOutput * 1000;   

        // To get the wind clearance, we need to read the DetailedInfo
        // as it appears in the lower right of the k-menu entry for each turbine
        // and split it apart into separate lines.
        string turbineDetail = turbine.DetailedInfo;
        string[] turbineDetailArray = turbineDetail.Split('\n');

        // Start building the string with name, current and max outputs, formatted into columns.
        s2 += String.Format("{0,-10} : {1,5:F1} / {2,5:F1}",
                                         turbine.CustomName.Replace("Wind ",""),
                                         currentOutput,
                                         maxOutput);
        // The clearance info is on the 4th line (0-based index 3)
        // Protect against illegal access by ensuring the array
        // has that many lines before we read there.
        if (turbineDetailArray.Length > 3)
        {
            // We've got enough array entries.
            // Take the 4th line and remove the prefix, leaving only the clearance "value" (e.g. "Optimal", "Good",...)
            s2 += String.Format(" {0,8}", turbineDetailArray[3].Replace("Wind Clearance:", ""));
        }
        s2 += "\n"; // carriage return at the end of the line for this turbine
    }

    // Now dump the big string with all turbines' info to all screens
    foreach (var lcd in _outputLCDs)
    {
        // Write the title to this screen.
        // 'false' means no-append, which clears screen
        lcd.WriteText ("WIND TURBINE OUTPUT\n", false); 

        // Write the column headers to this screen (appending)
        lcd.WriteText (String.Format("{0,-10} : {1,5} / {2,5} {3,8}\n",
                                                        "Name",  "CurKW", "MaxKW", "Clearance"), true);

        // Write the previously assembled turbine info, appending
        lcd.WriteText (s2, true);
    }
}
