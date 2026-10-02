/*
 * Tera's Script Template v1.0
 * R e a d m e
 * -----------
 * 
 * This is  script Template made by Tera for Tera
 * if you have any questions or suggestions put them in the steam's description
 */

//Parameeters
//
//
//
//
//
//---------------Parameters Ends Here---------------//
//Custom Varibles & Types
//
//
//
//
//
//Required varibles
MyCommandLine commandLine = new MyCommandLine();
bool run = false; int pc = 0; int stage = 0; int oldStage = 0;
string spinner = "----------\\\\\\\\\\||||||||||//////////";
//Required types
IMyShipController controller;
List<IMyTerminalBlock> _foundblocks = new List<IMyTerminalBlock>();
List<IMyTextSurface> _displays = new List<IMyTextSurface>();

//Program Initator
public Program() {
    Echo("Startup");
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    GridTerminalSystem.GetBlocks(_foundblocks);
    SetupDisplay(Me.GetSurface(0), 2);
    SetupDisplay(Me.GetSurface(1), 4);
    foreach (IMyTerminalBlock block in _foundblocks) {
        if (block is IMyShipController && (block.DisplayNameText.IndexOf("Control") >= 0 || block.DisplayNameText.IndexOf("Cockpit") >= 0) && block.CubeGrid == Me.CubeGrid) {
            if (block is IMyCockpit) {
                controller = block as IMyShipController;
                for (int i = 0; i < (block as IMyCockpit).SurfaceCount; i++) {
                    SetupDisplay((block as IMyCockpit).GetSurface(i));
                }
            } else if (controller == null) {
                controller = block as IMyShipController;
            }
        }
        //Add Other Required Blocks Here
        //
        //
        //
        //
        //
    }
    if (controller == null) {
        Echo("No Controller found");
    } else { run = true; }
}

//Stage Setup
public string setup() {
    if (stage != oldStage) {
        switch (stage) {// Startup
            case 0: //Standby
                break;
                //Add State Setup here
                //
                //
                //
                //
                //
        }
        switch (oldStage) {// Terminate
            case 0: //Standby
                break;
                //Add State Terminate here
                //
                //
                //
                //
                //
        }
    }
    return null;
}

//Main Program
public void Main(string argument) {
    //Check if Program Initator successful
    if (!run) { return; }
    //Spinner
    if (pc == spinner.Length - 1) { pc = 0; }
    Echo(spinner[pc].ToString()); Display<char>(1, spinner[pc], false, '\0'); pc++;
    //Parsing Input data
    if (commandLine.TryParse(argument)) {
        if (commandLine.Argument(0) != null) {
            try { stage = Convert.ToInt32(commandLine.Argument(0)); } catch { }
            //Add More Arguements here
            //
            //
            //
            //
            //
        }
    }
    //Check for State Setup Errors
    if (setup() != null) { Display<String>(1, setup(), false); Echo(setup()); return; }
    oldStage = stage;

    //Main Switch
    switch (stage) {
        case 0: //Standby
            Display<String>(1, "Standby");
            Echo("Standby");
            break;
        //Add Other States Here
        //
        //
        //
        //
        //
        default:
            Display<String>(1, "Wrong State Spesified");
            Echo("Wrong State Spesified");
            break;
    }
}

//Display Setup
public void SetupDisplay(IMyTextSurface display, int font = 2, float padding = 4) {
    _displays.Add(display);
    display.TextPadding = padding;
    display.ContentType = ContentType.TEXT_AND_IMAGE;
    display.FontSize = font;
    display.TextPadding = padding;
    display.Alignment = TextAlignment.CENTER;
    display.WriteText("Display No." + (_displays.Count - 1).ToString());
}

//Display Api
public void Display<T>(int display, T text, bool append = true, char nl = '\n') {
    _displays[display].WriteText(Convert.ToString(text) + nl, append);
}