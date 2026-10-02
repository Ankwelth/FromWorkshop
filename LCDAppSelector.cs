/*
 * App Selector for LCD screens
 * 
 * - When inside your ship's terminal, drag the PB to your hotbar and select "Run".
 * - Enter "<block name>" <screen number> "<app name>" where
 *   - `block name` Name of the block with the LCD screen (e.g. Transparent LCD).
 *   - `screen number` Zero based number of the screen (usually 0).
 *   - `app name` Internal name of the app (not its display name!).
 *   - `content type` What content to display. Can be either "none", "text" or "image" (can be omitted
 *      entirely for apps). Only taken into account when app name is "".
 * - Example that works with a block named "MyLCD" and displays the clock:
 *     "MyLCD" 0 "TSS_ClockAnalog"
 * - Example that works with a block named "Transparent LCD" and displays the 
 *   [Connector Align App](https://github.com/Kiminaze/SEConnectorAlignApp):
 *     "Transparent LCD" 0 "ConnectorAlignApp"
 * - Example that works with a block named "MyLCD" and sets the screen to display text:
 *     "MyLCD" 0 "" "text"
 * - Setting the app name to an empty string ("") removes any currently present app.
 *     "MyLCD" 0 ""
 * - Setting a non-existent app name results in the PB displaying all available app names in its 
 *   Info Panel in the bottom right of the terminal.
 */
private readonly MyCommandLine commandLine = new MyCommandLine();
public void Main(string argument)
{
    if (!commandLine.TryParse(argument))
    {
        Echo($"[ERR] Could not parse arguments!");
        return;
    }

    string blockName = commandLine.Argument(0);
    if (blockName == null)
    {
        Echo($"[ERR] Could not parse first argument! Should be name of the block with the LCD panel.");
        return;
    }

    int panelNum;
    if (!int.TryParse(commandLine.Argument(1), out panelNum))
    {
        Echo($"[ERR] Could not parse second argument! Should be the number of the screen.");
        return;
    }

    string appName = commandLine.Argument(2);
    if (appName == null)
    {
        Echo($"[ERR] Could not parse third argument! Should be the app name.");
        return;
    }

    string content = commandLine.Argument(3);

    IMyTerminalBlock block = GridTerminalSystem.GetBlockWithName(blockName);
    if (block == null || !(block is IMyTextSurfaceProvider))
    {
        Echo($"[ERR] Could not find block named {blockName}!");
        return;
    }

    IMyTextSurfaceProvider lcdBlock = (IMyTextSurfaceProvider)block;

    if (panelNum >= lcdBlock.SurfaceCount)
    {
        Echo($"[ERR] Screen number is higher than the amount of LCD panels of this block. Starts at 0.");
        return;
    }

    IMyTextSurface surface = lcdBlock.GetSurface(panelNum);

    List<string> apps = new List<string>();
    surface.GetScripts(apps);
    if (appName != ""&& !apps.Contains(appName))
    {
        Echo($"[ERR] App \"{appName}\" not found!\nCurrent: \"" + surface.Script + "\"\nAvailable app names:");
        foreach (string app in apps)
            Echo(app);

        return;
    }

    if (appName == ""&& content != null && content != "")
    {
        switch (content)
        {
            case "text":
            case "image":
                surface.ContentType = ContentType.TEXT_AND_IMAGE;
                break;

            case "none":
                surface.ContentType = ContentType.NONE;
                break;

            default:
                Echo($"[ERR] Content option \"{content}\" not found!");
                return;
        }
    }
    else
    {
        surface.ContentType = ContentType.SCRIPT;
        surface.Script = appName;
    }
}
