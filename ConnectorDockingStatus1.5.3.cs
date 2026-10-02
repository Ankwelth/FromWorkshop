/*
 * Connector Docking Status 1.5.3
 * by AltonV
 *
 * This is a script that sets the name of connectors according to the status,
 * if they are locked, in proximity, unoccupied or disabled.
 *
 * Features:
 *  -- Show on hud (configurable, default is only when unoccupied)
 *     !! Currently not working as of the release of this version due to game changes
 *
 *  -- Lights. Diffrent color depending on status.
 *       Unoccupied: Green
 *       In Proximity: Yellow
 *       Connected: Red
 *       Disabled: Cyan (Option to turn the lights of if connector is disabled)
 *     If you want to change colors you need to change them in the getColors method inside the script.
 *     The programmable block didn't like it when I tried putting those arrays in the beginning.
 *     Lights should have the same name as the connector but have the word "Light" at the end. (Configurable)
 *     (You can also put other stuff after that if you want e.g. numbers)
 *     All lights should work as long as they implements the "IMyLightingBlock" class. (Only interior lights tested)
 *
 *  -- LCD screens. Displays all connectors and their status.
 *     Default name is "[Prefix]  Connector LCD" (configurable, can put other stuff behind the name e.g. numbers)
 *
 *  -- Timer blocks. Timer blocks can be triggered upon status changes.
 *        Name the timerblock "CONNECTORNAME Timer STATUS"
 *        Eg if you have a connector named "[Station] Connector 1 - STATUS" and name the timer block
 *        "[Station] Connector 1 Timer Connected" it will trigger when you lock the connector.
 */

bool PREFIX_AUTO = true; // Set to false to disable automatic addition of prefixes
string LCD_NAME = "Connector LCD"; // The name of lcds to locate, exluding the prefix

List<string> PREFIXES = new List<string>{
    //"[Prefix] ",
};

// Status strings
string UNOCCUPIED = "Unoccupied";
string PROXIMITY = "In Proximity";
string CONNECTED = "Connected";
string DISABLED = "Disabled";

// Feature toggles
bool FEATURE_TIMER = true;  // Set to false to disable the trigger timer feature
bool FEATURE_LCD = true;  // Set to false to disable the LCD feature. If PREFIX_AUTO is true it overrides this.
bool FEATURE_LIGHT = true;   // Set to false to disable the light feature

// Show on hud toggles
bool SHOW_UNOCCUPIED_ON_HUD = true;
bool SHOW_PROXIMITY_ON_HUD = false; 
bool SHOW_LOCKED_ON_HUD = false;
bool SHOW_DISABLED_ON_HUD = false;

// Light config
string LIGHT_SUFFIX = " Light"; 
bool LIGHTS_OFF_ON_DISABLED = false; // Set to true to turn off lights if connector is disabled

// Timer config
bool TIMER_USE_START = false; // Set to true to use start insted of trigger now on timer blocks

// LCD config
string LCD_TITLE = "Connector status";
bool LCD_TITLE_SHOW_PREFIX = false;
bool LCD_CONNECTOR_SHOW_PREFIX = true;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}
void Main(string argument, UpdateType updateType)
{
    if (PREFIX_AUTO)
    {
        if (MANUAL_PREFIXES == null)
            MANUAL_PREFIXES = new List<string>(PREFIXES);
        findPrefixes();
    }

    int connected = 0;
    int proximity = 0;
    int unoccupied = 0;
    int disabled = 0;

    // Find all connectors connected to the grid
    List<IMyTerminalBlock> connectorList = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectorList, b => getPrefix(b) != "");

    clearLCD();

    //Iterate connectors
    for (int i = 0; i < connectorList.Count; i++)
    {
        IMyShipConnector connector = connectorList[i] as IMyShipConnector;
        string status = "";

        // If the connector is disabled
        if (!connector.Enabled)
        {
            status = DISABLED;
            //connector.RequestShowOnHUD(SHOW_DISABLED_ON_HUD);
            disabled++;
        }
        // If the connector is connected
        else if (connector.IsConnected)
        {
            status = CONNECTED;
            //connector.RequestShowOnHUD(SHOW_LOCKED_ON_HUD);
            connected++;
        }
        // If the connector is in proximity with another but not connected
        else if (connector.IsLocked)
        {
            status = PROXIMITY;
            //connector.RequestShowOnHUD(SHOW_PROXIMITY_ON_HUD);
            proximity++;
        }
        // If the connector is unoccupied
        else
        {
            status = UNOCCUPIED;
            //connector.RequestShowOnHUD(SHOW_UNOCCUPIED_ON_HUD);
            unoccupied++;
        }

        bool update = !connector.CustomName.EndsWith(status);
        setName(connector, status);
        if (FEATURE_LCD || PREFIX_AUTO) writeLCD(connector, status);
        if (FEATURE_LIGHT) setLight(connector, status, update);
        if (update && FEATURE_TIMER) triggerTimer(connector, status);
    }

    // Display some info in the programmable block
    Echo("Docking Status\n==============");
    Echo(CONNECTED + ": " + connected + "\n" + PROXIMITY + ": " + proximity + "\n" +
        UNOCCUPIED + ": " + unoccupied + "\n" + DISABLED + ": " + disabled);
    Echo("\nPrefixes:");
    if (PREFIXES.Count > 0)
        Echo("'" + string.Join("'\n'", PREFIXES.ToArray()) + "'");
    else
        Echo("No prefixes defined");
}

List<string> MANUAL_PREFIXES;
int RUNS = 0;
void findPrefixes()
{
    if (RUNS-- <= 0)
        RUNS = 5;
    else
        return;

    List<IMyTerminalBlock> lcdList = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(LCD_NAME, lcdList, b => b is IMyTextPanel);

    PREFIXES.Clear();
    for (int i = 0; i < lcdList.Count; i++)
    {
        IMyTextPanel lcd = lcdList[i] as IMyTextPanel;

        string prefix = lcd.CustomName.Replace(LCD_NAME, "");
        if(prefix.Trim() == "") continue;

        if (!PREFIXES.Contains(prefix))
            PREFIXES.Add(prefix);
    }
    PREFIXES.AddRange(MANUAL_PREFIXES);
}

string getPrefix(IMyTerminalBlock block)
{
    string prefix = "";
    for(int p = 0; p < PREFIXES.Count; p++)
    { 
        if (block.CustomName.StartsWith(PREFIXES[p])) prefix = PREFIXES[p];
    }
    return prefix;
}

string getName(IMyShipConnector connector)
{
    string [] name = connector.CustomName.Split('-');
    if (name.Length > 1) name[name.Length-1] = "";
    String nameString = String.Join("-", name);
    nameString = nameString.Remove(nameString.Length - 1);
    return nameString.Trim();
}

void setName(IMyShipConnector connector, String status)
{
    String name = getName(connector);
    if (status != "")
        connector.SetCustomName(name + " - " + status);
    else
        connector.SetCustomName(name);
}

//Format: Red, Green, Blue
byte[] getColors(String status)
{
    if (status == UNOCCUPIED)
    {
        return new byte[] { 0, 255, 0 };
    }
    else if (status == PROXIMITY)
    {
        return new byte[] { 255, 255, 0 };
    }
    else if (status == CONNECTED)
    {
        return new byte[] { 255, 0, 0 };
    }
    else if (status == DISABLED)
    {
        return new byte[] { 0, 255, 255 };
    }
    return new byte[] { 0, 0, 0 };
}

void setLight(IMyShipConnector connector, String status, bool update = false)
{
    String name = getName(connector);
    byte[] colors = getColors(status);

    List<IMyTerminalBlock> lightList = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(name + LIGHT_SUFFIX, lightList, b => b is IMyLightingBlock);

    for (int i = 0; i < lightList.Count; i++)
    {
        IMyLightingBlock light = lightList[i] as IMyLightingBlock;

        if (update || (status == DISABLED && LIGHTS_OFF_ON_DISABLED))
            light.ApplyAction("OnOff_Off");
        else
            light.ApplyAction("OnOff_On");

        Color color = light.GetValue<Color>("Color");
        color.R = colors[0];
        color.G = colors[1];
        color.B = colors[2];
        light.SetValue("Color", color);
    }
}

void clearLCD()
{
    // Find all lcd screens
    List<IMyTerminalBlock> lcdList = new List<IMyTerminalBlock>();
    for(int p = 0; p < PREFIXES.Count; p++)
    {
        List<IMyTerminalBlock> list = new List<IMyTerminalBlock>();
        GridTerminalSystem.SearchBlocksOfName(PREFIXES[p] + LCD_NAME, list, b => b is IMyTextPanel);
        lcdList.AddRange(list);
    }

    // Iterate lcd screens
    for (int i = 0; i < lcdList.Count; i++)
    {
        IMyTextPanel lcd = lcdList[i] as IMyTextPanel;
        string prefix = getPrefix(lcd);

        // Build the title and separator
        int length = LCD_TITLE.Trim().Length - 2;
        StringBuilder title = new StringBuilder();
        if (LCD_TITLE_SHOW_PREFIX)
        {
            title.Append(prefix);
            length += prefix.Length - 2;
        }
        title.Append(LCD_TITLE.Trim() + "\n" + new String('=', length) +"\n");

        // Write to the lcd
        lcd.WritePublicText(title.ToString(), false);
        lcd.ShowPublicTextOnScreen();
    }
}

void writeLCD(IMyShipConnector connector, String status)
{
    string prefix = getPrefix(connector);
    string name = getName(connector);

    // Removes the connector prefix
    if (!LCD_CONNECTOR_SHOW_PREFIX)
    {
        int index = name.IndexOf(prefix);
        name = (index < 0) ? name : name.Remove(index, prefix.Length);
    }

    List<IMyTerminalBlock> lcdList = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.SearchBlocksOfName(prefix + LCD_NAME, lcdList, b => b is IMyTextPanel);

    for (int i = 0; i < lcdList.Count; i++)
    {
        IMyTextPanel lcd = lcdList[i] as IMyTextPanel; 
        lcd.WritePublicText(name + ": " + status + "\n", true);
    }
}

void triggerTimer(IMyShipConnector connector, String status)
{
    String name = getName(connector);
    IMyTerminalBlock block = GridTerminalSystem.GetBlockWithName(name + " Timer " + status);
    if (block != null && block is IMyTimerBlock)
    {
        block.ApplyAction((TIMER_USE_START ? "Start" : "TriggerNow"));
    }
}
