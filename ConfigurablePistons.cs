/* ********************************************************************************************************** *

   
    Configurable Pistons

    @Author:    Gate
    @Date:      2023/05/22

    @Version:   1.4

    @Change-Log:
                v1.4:       Fixed logic bug, cause i'm an idiot
                            Auto retract and extend should now work for everyone
                            and not just our specific uses
                v1.3:       Fixed Documentation Spelling Error >.<
                v1.2:       Added Auto Retract and Extend support
                v1.1:       Bug-fix INI related
                v1.0:       Initial Release

    @Arguments:
        This script understands the following arguments
            reset   - This resets all internal caches.
                      Run this if you add/remove or change piston settings

    @Usage:
        Add (and edit) the following to the custom data of any piston you wish to add script functionality

        [Piston Settings]
        Ignore=False
        RetractSpeed=1
        ExtendSpeed=1
        AutoRetract=False
        AutoExtend=False

    @Note:
        Ignore       - If this is set to True, this piston will be (actively)ignored.
                     - Useful for preconfiguring a piston for "future" use.
        RetractSpeed - The speed (in m/s) the piston should move during Retraction
        ExtendSpeed  - The speed (in m/s) the piston should move during Extension
        AutoRetract  - Should the piston automatically retract once at full extension
        AutoExtend   - Should the piston automatically extend once at full retraction

    
        You MUST either recompile the script,
        or run with argument "reset"
        to populate piston cache

        The script will now optionally auto-update it's cache every 10,000 ticks (~166 seconds)
        set AutoCache = true


    Script (C)opyright 2021 Gate
    Some Rights Reserved.

    @License:
        This work is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.
        To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/4.0/
        or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.


    Space Engineers is (C)opyright Keen Software House
    https://www.spaceengineersgame.com/
    

    Created using the MDK-SE SDK
    Malware's Development Kit for Space Engineers
    https://github.com/malware-dev/MDK-SE

* ********************************************************************************************************** */

// Last Build: 2024-01-04 02:37

/* -----------------------------------------------------------
 * -- User Editable Variables --
 * ----------------------------------------------------------- */

/// <summary>
/// Set this to true to auto update the cache every 10,000 ticks
/// </summary>
private readonly bool AutoCache = false;

/// <summary>
/// Default Retraction Speed
/// Speed (in m/s) when not otherwise defined
/// </summary>
private readonly float defaultRetractSpeed = 0.5f;

/// <summary>
/// Default Extension Speed
/// Speed (in m/s) when not otherwise defined
/// </summary>
private readonly float defaultExtendSpeed = 0.5f;

/// <summary>
/// Default - Auto Retract
/// Set if not otherwise defined
/// </summary>
private readonly bool defaultAutoRetract = false;

/// <summary>
/// Default - Auto Extend
/// Set if not otherwise defined
/// </summary>
private readonly bool defaultAutoExtend = false;


/* -----------------------------------------------------------
 * -- Do Not Edit Below This Line! --
 * ----------------------------------------------------------- */

/// <summary>
/// Instance to the INI system
/// </summary>
// ReSharper disable once InconsistentNaming
private static readonly MyIni ini = new MyIni();

/// <summary>
/// The [section] to be read from in Custom Data
/// </summary>
private static readonly string SectionKey = "Piston Settings";

/// <summary>
/// List of commands our program will respond too via the arguments box
/// </summary>
private readonly Dictionary<string, Action> commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);

/// <summary>
/// This is the programs output to be printed on the programmable block pane
/// </summary>
private readonly StringBuilder outputText;

/// <summary>
/// Container for storing various speeds in addition to the piston
/// </summary>
private struct PistonData
{
    public bool AutoRetract;
    public bool AutoExtend;
    public float RetractSpeed;
    public float ExtendSpeed;
    public IMyPistonBase Piston;
}

/// <summary>
/// Linked list storing our affected pistons, as well as the various speeds
/// </summary>
private readonly List<PistonData> pistonDataList;

/// <summary>
/// The last message to be displayed, this allows persistence along with an activity indicator
/// </summary>
private readonly StringBuilder lastMessageText;

/// <summary>
/// </summary>
private int updateCounter = 0;

/// <summary>
/// Our program constructor
/// This is run on compilation (world load, script import, etc)
/// </summary>
public Program()
{
    // Command list
    commands["reset"] = Init;

    // property initializers
    pistonDataList = new List<PistonData>();
    outputText = new StringBuilder();
    lastMessageText = new StringBuilder();
    __activityIndicator = 0;

    // Run frequency
    Runtime.UpdateFrequency = UpdateFrequency.Once;
}

/// <summary>
/// The program entry-point.
/// </summary>
/// <param name="arguments">The string supplied by the user</param>
/// <param name="updateSource">The source of this update,  whether by the user, a timer, or continous running</param>
public void Main(string arguments, UpdateType updateSource)
{
    outputText.Clear();
    outputText.AppendFormat(
        "-- Configurable Pistons --" +
        "\n" +
        "  Pistons Monitored: {0:d}" +
        "\n" +
        "\n" +
        "  - Stats -\n" +
        "  Runtime {0:F3} ms every {1:F0} ms\n" +
        "\n",
        pistonDataList.Count(),
        Runtime.LastRunTimeMs,
        Runtime.TimeSinceLastRun.TotalMilliseconds
    );

    if (( updateSource & UpdateType.Once ) != 0) {
        // Initialization - Populate the Lists
        Init();

        if (pistonDataList.Count > 0) {
            Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;
        }
    }

    // Handing user input
    else if (updateSource == UpdateType.Terminal) {
        MyCommandLine commandLine = new MyCommandLine();

        if (commandLine.TryParse(arguments)) {
            string command = commandLine.Argument(0);
            Action commandAction;

            if (commands.TryGetValue(command, out commandAction)) {
                SetMessage("Executing: {0:s}", command);
                commandAction();
            }
            else {
                StringBuilder msg = new StringBuilder();

                msg.AppendFormat(
                    "  Unknown Command: {0:s}\n" +
                    "\n" +
                    "  Available Commands:\n" +
                    "    reset     Reload settings\n",
                    command
                );

                SetMessage(msg.ToString());

                msg.Clear();
            }
        }
    }

    else if (( updateSource & UpdateType.Update10 ) != 0) {
        foreach (PistonData data in pistonDataList) {
            // skip if the block no longer exists in the world
            if (data.Piston.Closed) {
                if (pistonDataList.Remove(data)) {
                    SetMessage("A Piston was removed from grid\nDeleted Reference.");
                }

                break;
            }

            // Skip if powered off or in a non-functional state
            if (!data.Piston.IsWorking) {
                continue;
            }

            switch (data.Piston.Status) {
                case PistonStatus.Extended:
                    if (data.AutoRetract) {
                        data.Piston.Velocity = -data.RetractSpeed;
                    }
                    //data.Piston.Velocity = data.AutoRetract ? -data.RetractSpeed : data.RetractSpeed;
                    break;

                case PistonStatus.Extending:
                    data.Piston.Velocity = data.ExtendSpeed;
                    break;

                case PistonStatus.Retracted:
                    if (data.AutoExtend) {
                        data.Piston.Velocity = data.ExtendSpeed;
                    }
                    //data.Piston.Velocity = data.AutoExtend ? data.ExtendSpeed : -data.ExtendSpeed;
                    break;

                case PistonStatus.Retracting:
                    data.Piston.Velocity = -data.RetractSpeed;
                    break;
            }
        }

        // no pistons controlled,  stop continuously running
        if (pistonDataList.Count == 0) {
            Runtime.UpdateFrequency = UpdateFrequency.None;
        }
    }

    else if (( updateSource & UpdateType.Update100 ) != 0 && AutoCache) {
        // every 10,000 ticks (100 * 100),  update the piston list (every ~166 seconds)
        if ((updateCounter % 100) == 0) {
            Init();

            updateCounter = 0;
        }

        updateCounter++;
    }

    outputText.AppendStringBuilder(lastMessageText);
    outputText.AppendLine();
    outputText.Append(GetActivityIndicator());

    Print(outputText.ToString());
}

/// <summary>
/// Setup our internal list of pistons and associated data
/// </summary>
private void Init()
{
    pistonDataList.Clear();

    // temporary list
    List<IMyPistonBase> pistonList = new List<IMyPistonBase>();

    GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(pistonList, piston => MyIni.HasSection(piston.CustomData, SectionKey));

    foreach (IMyPistonBase piston in pistonList) {

        if (!ini.TryParse(piston.CustomData)) {
            continue;
        }

        if (ini.Get(SectionKey, "Ignore").ToBoolean()) {
            continue;
        }

        PistonData data = new PistonData
        {
            Piston = piston,
            RetractSpeed = ini.Get(SectionKey, "RetractSpeed").ToSingle(defaultRetractSpeed),
            ExtendSpeed = ini.Get(SectionKey, "ExtendSpeed").ToSingle(defaultExtendSpeed),
            AutoRetract = ini.Get(SectionKey, "AutoRetract").ToBoolean(defaultAutoRetract),
            AutoExtend = ini.Get(SectionKey, "AutoExtend").ToBoolean(defaultAutoExtend)
        };

        pistonDataList.Add(data);
    }

    pistonList.Clear();

    SetMessage("Piston cache updated...");
}

/// <summary>
/// </summary>
/// <param name="message"></param>
private void SetMessage(string message)
{
    lastMessageText.Clear();
    lastMessageText.AppendFormat("\n  - Message -\n{0}\n\n", message);
}

/// <summary>
/// </summary>
/// <param name="format"></param>
/// <param name="args"></param>
private void SetMessage(string format, params object[] args)
{
    lastMessageText.Clear();
    lastMessageText.AppendFormat(format, args);
}

/// <summary>
/// </summary>
private void ClearMessage()
{
    lastMessageText.Clear();
}

/// <summary>
/// This is an internal counter for the little . .. ... .... indicator
/// </summary>
// ReSharper disable once InconsistentNaming
private int __activityIndicator;

/// <summary>
/// This is the internal string array for the indicator
/// </summary>
// ReSharper disable once InconsistentNaming
private readonly string[] __indicators = { "    ", ".   ", " .  ", "  . ", "   .", };

/// <summary>
/// Returns an ever changing string to let the user know the script is "working"
/// </summary>
/// <returns>An ever changing string</returns>
private string GetActivityIndicator()
{
    if (__activityIndicator >= __indicators.Length) {
        __activityIndicator = 0;
    }

    return __indicators[__activityIndicator++];
}

/// <summary>
/// Print a message to the message pane
/// </summary>
/// <param name="message">The string to print</param>
private void Print(string message)
{
    Echo(message);
}

/// <summary>
/// </summary>
/// <param name="message"></param>
/// <param name="surface"></param>
private void Print(string message, IMyTextSurface surface)
{
    if (surface != null) {
        surface.WriteText(message);
    }

    Echo(message);
}

} // Break out of the PB environment,  this allows for "extension" utility classes.  This is an _allowed_ exploit of the PB programming environment


/// <summary>
/// Class containing extension methods for the following classes<br/>
/// <br/>
/// <see cref="IMyGridTerminalSystem">GridTerminalSystem</see>
/// </summary>
public static partial class Extensions
{
	/// <summary>
	/// Get a single block of type T, filtered by Func
	/// </summary>
	/// <remarks>
	/// Note: This function allocates a new List on every call
	/// </remarks>
	/// <typeparam name="T">type of the block to return</typeparam>
	/// <param name="me">reference to GridTerminalSystem</param>
	/// <param name="filter">function to determine if a block should be added to collection</param>
	/// <returns>T block, or null on no block found</returns>
    public static T GetBlockOfType<T>(this IMyGridTerminalSystem me, Func<T, bool> filter = null) where T : class
    {
        List<IMyTerminalBlock> blockList = new List<IMyTerminalBlock>();

        me.GetBlocksOfType<T>(blockList as List<T>, filter);

        return blockList.Count > 0 ? blockList[0] as T : null;
    }

	/// <summary>
	/// Get a single block to type T, whose name contains <see cref="param name">name</see>, and exists on the same grid (or sub-grid) as 'anyBlock'
	/// </summary>
	/// <remarks>
	/// Sub-Grids are those mechanically connected to a Grid (via rotors, pistons, etc), but not including those connected by connectors.
	/// </remarks>
	/// <typeparam name="T">type of the block to return</typeparam>
	/// <param name="me">reference to GridTerminalSystem</param>
	/// <param name="name">name of the block to search for</param>
	/// <param name="anyBlock">any block existing on the "grid" to filter with</param>
	/// <returns>T block</returns>
    public static T GetBlockOfTypeWithName<T>(this IMyGridTerminalSystem me, string name, IMyTerminalBlock anyBlock = null) where T : class
    {
        if (anyBlock == null) {
            return GetBlockOfType<IMyTerminalBlock>(me, block => block is T && block.CustomName.Contains(name)) as T;
        }

        return GetBlockOfType<IMyTerminalBlock>(me, block => block is T && block.IsSameConstructAs(anyBlock) && block.CustomName.Contains(name)) as T;
    }

	/// <summary>
	/// </summary>
	/// <param name="cache">An existing List to make use of (will be cleared!)</param>
	/// <param name="grid">The grid to filter against, or NULL to search all</param>
	/// <param name="collect">The filter function</param>
	/// <returns>A list of <typeparamref name="T"/></returns>
    public static List<T> Blocks<T>(this IMyGridTerminalSystem system, List<T> cache = null, IMyCubeGrid grid = null, Func<T, bool> collect = null) where T : class, IMyCubeBlock
    {
        List<T> blocks;

        if (cache != null) {
            cache.Clear();
            blocks = cache;
        }
        else {
            blocks = new List<T>();
        }

        if (grid == null) {
            system.GetBlocksOfType(blocks, collect);
        }
        else {
            system.GetBlocksOfType(blocks, block => block.CubeGrid.Equals(grid) && ( collect?.Invoke(block) ?? true ));
        }

        return blocks;
    }

	/// <summary>
	/// </summary>
	/// <param name="cache">An existing List to make use of (will be cleared!)</param>
	/// <param name="grid">The grid to filter against, or NULL to search all</param>
	/// <param name="collect">The filter function</param>
	/// <returns>A list of <typeparamref name="T"/></returns>
    public static List<T> Blocks<T>(this IMyBlockGroup group, List<T> cache = null, IMyCubeGrid grid = null, Func<T, bool> collect = null) where T : class, IMyCubeBlock
    {
        List<T> blocks;

        if (cache != null) {
            cache.Clear();
            blocks = cache;
        }
        else {
            blocks = new List<T>();
        }

        if (grid == null) {
            group.GetBlocksOfType(blocks, collect);
        }
        else {
            group.GetBlocksOfType(blocks, block => block.CubeGrid.Equals(grid) && ( collect?.Invoke(block) ?? true ));
        }

        return blocks;
    }

	/// <summary>
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="collect"></param>
	/// <returns>A list of IMyBlockGroups</returns>
    public static List<IMyBlockGroup> Groups(this IMyGridTerminalSystem system, List<IMyBlockGroup> cache = null, Func<IMyBlockGroup, bool> collect = null)
    {
        List<IMyBlockGroup> groups;

        if (cache != null) {
            cache.Clear();
            groups = cache;
        }
        else {
            groups = new List<IMyBlockGroup>();
        }

        system.GetBlockGroups(groups, collect);

        return groups;
    }
}

/// <summary>
/// Class containing extension methods for the following classes<br/>
/// <br/>
/// <see cref="System.String">String</see>
/// </summary>
public static partial class Extensions
{
    [Flags]
    public enum StringSplitExOptions
    {
		/// <summary>
		/// The return value includes array elements that contain an empty string.
		/// </summary>
        None = 0x0,

		/// <summary>
		/// The return value does not include array elements that contain an empty string.
		/// </summary>
        RemoveEmptyEntries = 0x1,

		/// <summary>
		/// Specifies that whitespace should be trimmed from each substring in the result.
		/// </summary>
        TrimEntries = 0x2,

		/// <summary>
		/// Specifies that "Quoted" substrings will be preserved on split operations
		/// </summary>
        PreserveQuotedSubstrings = 0x4
    }

	/// <summary>
	/// Splits a string into substrings based on the characters in an array.
	/// </summary>
	/// <param name="str">the instance</param>
	/// <param name="separator">A character that delimits the substrings in this string, or null.</param>
	/// <param name="options">
	/// RemoveEmptyEntries - omit empty array elements from the array returned.<br/>
	/// TrimEntries - remove leading and trailing whitespace from split sub-strings.<br/>
	/// PreserveQuotedSubstrings - ignore seperators inside double quotes.
	/// </param>
	/// <returns>An array whose elements contain the substrings in this string that are delimited by one or more characters in separator.</returns>
	/// <exception cref="ArgumentException"/>
    public static string[] Split(this string str, char[] separator, StringSplitExOptions options = StringSplitExOptions.None)
    {
        // we're not being asked to preserve quoted substrings,  return the result of the regular string.Split()
        if (( options & StringSplitExOptions.PreserveQuotedSubstrings ) == 0) {
            return str.Split(separator, (StringSplitOptions)( ( options & StringSplitExOptions.RemoveEmptyEntries ) | ( options & StringSplitExOptions.TrimEntries ) ));
        }

        List<string> subStrings = new List<string>();

        bool inQuotedString = false;
        int start = 0, len = 0;

        for (int i = 0; i < str.Length; i++) {
            char c = str[i];

            if (c.Equals('"')) {
                inQuotedString = !inQuotedString;
            }

            if (c.Equals(separator) && !inQuotedString) {
                if (( options & StringSplitExOptions.TrimEntries ) != 0) {
                    string t = str.Substring(start, len).Trim();

                    if (( t.Length == 0 ) && ( options & StringSplitExOptions.RemoveEmptyEntries ) != 0) {
                        continue;
                    }

                    subStrings.Add(t);
                }
                else {
                    subStrings.Add(str.Substring(start, len));
                }

                start = i + 1;
                len = 0;
            }
            else {
                len++;
            }
        }

        return subStrings.ToArray();
    }

	/// <summary>
	/// Determines whether the ending of this string instance matches the specified string.
	/// </summary>
	/// <param name="str">this instance.</param>
	/// <param name="value">The string to compare.</param>
	/// <returns>true if value matches the ending of this string; otherwise, false.</returns>
	/// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool EndsWith(this string str, string value)
    {
        int len = value.Length;

        return string.Compare(str.Substring(len).Trim(), value) == 0;
    }

	/// <summary>
	/// Determines whether the ending of this string instance matches the specified<br/>
	/// string when compared using the specified comparison option.
	/// </summary>
	/// <param name="str">this instance.</param>
	/// <param name="value">The string to compare.</param>
	/// <param name="comparisonType">One of the enumeration values that determines how this string and value are compared.</param>
	/// <returns>true if this instance ends with value; otherwise, false.</returns>
	/// <exception cref="ArgumentNullException">value is null.</exception>
	/// <exception cref="ArgumentException">comparisonType is not a System.StringComparison value.</exception>
    public static bool EndsWith(this string str, string value, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        int len = value.Length;

        return string.Compare(str.Substring(len).Trim(), value, comparisonType) == 0;
    }
}

/// <summary>
/// Class for outputting text to both the terminal, as well as any TextSurface's (LCDs, Cockpit displays, etc),
/// as well as optionally to the CustomData as persistent log
/// </summary>
public class IOHandler
{
    private readonly MyGridProgram parent;
    private readonly List<IMyTextPanel> panelList;
    private readonly List<IMyTextSurface> surfaceList;
    private readonly bool echoToCustomData;

	/// <summary>
	/// Initializes a new instance of the <see cref="IOHandler"/> class.
	/// </summary>
	/// <param name="parentProgram">The parent program</param>
	/// <param name="echoToCustomData">If true, echo to custom data</param>
    public IOHandler(MyGridProgram parentProgram, bool echoToCustomData = false)
    {
        parent = parentProgram;

        panelList = new List<IMyTextPanel>();
        surfaceList = new List<IMyTextSurface>();

        this.echoToCustomData = echoToCustomData;

        if (echoToCustomData) {
            //parent.Me.CustomData = "";
            parent.Me.CustomData += "=====\n";
            parent.Me.CustomData += $"{DateTime.Now.ToString("HH:mm:ss")} - Logging Started\n";
            parent.Me.CustomData += "=====\n";
        }
    }

	/// <summary>
	/// </summary>
	/// <param name="message">The message.</param>
    public void Echo(string message)
    {
        string filteredMessage = message.Replace("[", "[[").Replace("]", "]]");

        parent.Echo(filteredMessage);

        if (panelList.Count > 0) {
            foreach (IMyTextPanel panel in panelList) {
                if (panel.IsWorking) {
                    panel.WriteText(message + "\n", true);
                }
            }
        }

        if (surfaceList.Count > 0) {
            foreach (IMyTextSurface surface in surfaceList) {
                surface.WriteText(message + "\n", true);
            }
        }

        if (echoToCustomData) {
            parent.Me.CustomData += $"{DateTime.Now.ToString("HH:mm:ss")} - {message}\n";
        }
    }

	/// <summary>
	/// </summary>
	/// <param name="format">The format.</param>
	/// <param name="args">The args.</param>
    public void Echo(string format, params object[] args)
    {
        this.Echo(string.Format(format, args));
    }

	/// <summary>
	/// </summary>
	/// <param name="panel"></param>
    public void AddTextPanel(IMyTextPanel panel)
    {
        panelList.Add(panel);

        panel.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        panel.WriteText("");
    }

	/// <summary>
	/// </summary>
	/// <param name="surface"></param>
    public void AddTextSurface(IMyTextSurface surface)
    {
        surfaceList.Add(surface);

        surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        surface.WriteText("");
    }

	/// <summary>
	/// </summary>
	/// <param name="provider"></param>
	/// <param name="index"></param>
    public void AddTextSurfaceProvider(IMyTextSurfaceProvider provider, int index = 0)
    {
        if (index < provider.SurfaceCount) {
            AddTextSurface(provider.GetSurface(index));
        }
        else {
            this.Echo("Error - AddTextSurfaceProvider : Argument '{0}' was Out of Range, Expected <= '{1]', requested '{2}'", "index", provider.SurfaceCount - 1, index);
        }
    }