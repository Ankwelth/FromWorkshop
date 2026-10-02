// Docking Manager
// Made by: repairmant87
// --- Tunables -----------------------------------------------------
// How close (meters) a Light Panel must be to a connector to be
// assigned to it. Increase if your lights are further away.
const float SEARCH_RADIUS = 15f;

static readonly Color COLOR_OPEN      = new Color(0,   255, 0);   // Green  – no ship
static readonly Color COLOR_PROXIMITY = new Color(255, 200, 0);   // Yellow – ship nearby
static readonly Color COLOR_LOCKED    = new Color(255, 0,   0);   // Red    – locked

// Auto-unlock timer. Set both to 0 to disable (default).
// Each connector's lock time is tracked independently; Disconnect() is
// called automatically once the total duration has elapsed.
const int AUTO_UNLOCK_HOURS   = 24;
const int AUTO_UNLOCK_MINUTES = 0;

// Name of a text panel on this grid to use as a docking summary display.
// Set to "" to disable the LCD feature entirely.
const string LCD_NAME = "Docking LCD";
// ----------------------------------------------------------------

class DockInfo
{
    public IMyShipConnector       Connector;
    public List<IMyLightingBlock> Lights;
    public TimeSpan               LockElapsed;
    public bool                   WasConnected;
    public Color                  LastColor;    // skip light writes when color unchanged
}

readonly List<DockInfo> _docks          = new List<DockInfo>();
readonly List<DockInfo> _connectedDocks = new List<DockInfo>(); // reused each UpdateStatus
readonly StringBuilder  _sb             = new StringBuilder();  // reused by UpdateStatus + SaveStorage
bool         _initialized;
bool         _unlockEnabled;
TimeSpan     _unlockTarget;
IMyTextPanel _lcd;
bool         _anyConnected; // true when ≥1 dock is Connected; gates SaveStorage
bool         _fastStatus;   // true when any dock has <5 min remaining; forces per-tick status
int          _statusClock = 6; // starts at threshold so status updates on the very first tick

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    if (!_initialized || argument == "INIT")
        Initialize();

    UpdateDockLights();

    _statusClock++;
    if (_fastStatus || _statusClock >= 6)
    {
        UpdateStatus();
        _statusClock = 0;
        _fastStatus  = false;
    }

    if (_unlockEnabled && _anyConnected)
        SaveStorage();
}

// -----------------------------------------------------------------
//  One-time setup: discover connectors → lights, rename everything
// -----------------------------------------------------------------
void Initialize()
{
    _docks.Clear();
    Echo("Scanning for docks...");

    int totalMinutes = AUTO_UNLOCK_HOURS * 60 + AUTO_UNLOCK_MINUTES;
    _unlockEnabled = totalMinutes > 0;
    _unlockTarget  = _unlockEnabled
        ? TimeSpan.FromMinutes(totalMinutes)
        : TimeSpan.Zero;

    _lcd = LCD_NAME.Length > 0
        ? GridTerminalSystem.GetBlockWithName(LCD_NAME) as IMyTextPanel
        : null;
    if (LCD_NAME.Length > 0 && _lcd == null)
        Echo("WARNING: LCD panel not found: " + LCD_NAME);

    var connectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(connectors,
        b => b.IsSameConstructAs(Me));
    connectors.Sort((a, b) => string.Compare(a.CustomName, b.CustomName, StringComparison.Ordinal));

    // Light Panels have "Panel" in their subtype name.
    // This catches LargeBlockLightPanel and SmallBlockLightPanel.
    var lightPanels = new List<IMyLightingBlock>();
    GridTerminalSystem.GetBlocksOfType(lightPanels,
        b => b.IsSameConstructAs(Me) &&
             b.BlockDefinition.SubtypeName.Contains("Panel"));

    if (connectors.Count == 0)
    {
        Echo("No connectors found.");
        _initialized = true;
        return;
    }

    // Build a list per connector, then assign each light to its
    // nearest connector (if within SEARCH_RADIUS).
    var lightsByConnector = new Dictionary<IMyShipConnector, List<IMyLightingBlock>>();
    foreach (var c in connectors)
        lightsByConnector[c] = new List<IMyLightingBlock>();

    foreach (var light in lightPanels)
    {
        IMyShipConnector nearest     = null;
        double           nearestDist = SEARCH_RADIUS;

        foreach (var connector in connectors)
        {
            double dist = Vector3D.Distance(
                light.GetPosition(), connector.GetPosition());

            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest     = connector;
            }
        }

        if (nearest != null)
            lightsByConnector[nearest].Add(light);
    }

    // Name and register only connectors that have ≥1 light
    int dockNum = 1;
    foreach (var kvp in lightsByConnector)
    {
        if (kvp.Value.Count == 0) continue;

        string num = dockNum.ToString("D2");
        kvp.Key.CustomName = "Dock Connector " + num;

        foreach (var light in kvp.Value)
            light.CustomName = "Light Panel Dock " + num;

        _docks.Add(new DockInfo
        {
            Connector    = kvp.Key,
            Lights       = kvp.Value,
            LockElapsed  = TimeSpan.Zero,
            WasConnected = false,
        });
        dockNum++;
    }

    if (_unlockEnabled)
        Echo("Auto-unlock: " + totalMinutes + " min");

    Echo("Docks registered: " + _docks.Count);
    RestoreStorage();
    _statusClock = 6;   // force status update on the first tick after re-init
    _initialized = true;
}

// -----------------------------------------------------------------
//  Runs every Update100 (~1.7 s): sets light colour per dock status
// -----------------------------------------------------------------
void UpdateDockLights()
{
    TimeSpan delta = Runtime.TimeSinceLastRun;
    _anyConnected  = false;
    _fastStatus    = false;

    foreach (var dock in _docks)
    {
        var  status      = dock.Connector.Status;
        bool isConnected = status == MyShipConnectorStatus.Connected;

        if (_unlockEnabled)
        {
            if (isConnected)
            {
                _anyConnected = true;

                if (!dock.WasConnected)
                    dock.LockElapsed = TimeSpan.Zero;   // reset on fresh lock

                dock.LockElapsed += delta;

                if (dock.LockElapsed >= _unlockTarget)
                {
                    dock.Connector.Disconnect();
                    dock.LockElapsed = TimeSpan.Zero;
                    isConnected      = false;
                    status           = dock.Connector.Status; // re-read after disconnect
                }
                else if (_unlockTarget - dock.LockElapsed < TimeSpan.FromMinutes(5))
                {
                    _fastStatus = true;
                }
            }
            else
            {
                dock.LockElapsed = TimeSpan.Zero;
            }
        }

        dock.WasConnected = isConnected;

        Color color;
        switch (status)
        {
            case MyShipConnectorStatus.Connected:   color = COLOR_LOCKED;    break;
            case MyShipConnectorStatus.Connectable: color = COLOR_PROXIMITY; break;
            default:                                color = COLOR_OPEN;      break;
        }

        if (color != dock.LastColor)
        {
            foreach (var light in dock.Lights)
                light.Color = color;
            dock.LastColor = color;
        }
    }
}

// -----------------------------------------------------------------
//  Builds docking summary and sends it to the PB Echo and LCD panel.
//  Runs every ~10 s normally; every tick when any dock has <5 min left.
// -----------------------------------------------------------------
void UpdateStatus()
{
    _connectedDocks.Clear();
    int freeCount = 0;

    foreach (var dock in _docks)
    {
        var status = dock.Connector.Status;
        if      (status == MyShipConnectorStatus.Connected)   _connectedDocks.Add(dock);
        else if (status == MyShipConnectorStatus.Unconnected) freeCount++;
        // Connectable (ship nearby but not locked) counts as neither
    }

    int dockedCount = _connectedDocks.Count;

    _sb.Clear();

    if (dockedCount > 0)
        _sb.AppendLine(dockedCount + (dockedCount == 1 ? " Ship Docked" : " Ships Docked"));

    _sb.AppendLine(freeCount + " of " + _docks.Count + (freeCount == 1 ? " Free Dock" : " Free Docks"));

    if (dockedCount > 0)
    {
        _sb.AppendLine();   // blank line before ship list

        for (int i = 0; i < _connectedDocks.Count; i++)
        {
            if (i > 0)
                _sb.AppendLine();   // blank line between ships

            var    dock     = _connectedDocks[i];
            var    other    = dock.Connector.OtherConnector;
            string gridName = other != null ? other.CubeGrid.CustomName : "Unknown";
            _sb.AppendLine(gridName);

            if (_unlockEnabled)
                _sb.AppendLine(FormatTime(_unlockTarget - dock.LockElapsed));
        }
    }

    string text = _sb.ToString();
    Echo(text);
    if (_lcd != null)
        _lcd.WriteText(text);
}

// -----------------------------------------------------------------
//  Saves each dock's lock elapsed time to Storage (keyed by connector name).
//  Only called when _anyConnected to avoid writes on idle servers.
// -----------------------------------------------------------------
void SaveStorage()
{
    _sb.Clear();
    foreach (var dock in _docks)
    {
        _sb.Append(dock.Connector.CustomName);
        _sb.Append('=');
        _sb.Append(dock.LockElapsed.Ticks);
        _sb.Append(';');
    }
    Storage = _sb.ToString();
}

// -----------------------------------------------------------------
//  Restores lock elapsed times from Storage after (re)initialisation
// -----------------------------------------------------------------
void RestoreStorage()
{
    if (!_unlockEnabled || Storage.Length == 0) return;

    string[] entries = Storage.Split(';');
    foreach (string entry in entries)
    {
        if (entry.Length == 0) continue;
        int eq = entry.IndexOf('=');
        if (eq < 0) continue;

        string name = entry.Substring(0, eq);
        long   ticks;
        if (!long.TryParse(entry.Substring(eq + 1), out ticks)) continue;

        foreach (var dock in _docks)
        {
            if (dock.Connector.CustomName != name) continue;
            dock.LockElapsed  = TimeSpan.FromTicks(ticks);
            // Reflect current connection state so the first tick doesn't reset the timer
            dock.WasConnected = dock.Connector.Status == MyShipConnectorStatus.Connected;
            break;
        }
    }
}

string FormatTime(TimeSpan t)
{
    if (t.TotalMinutes >= 5)
    {
        if (t.TotalHours >= 1)
            return string.Format("{0}h {1}m", (int)t.TotalHours, t.Minutes);
        return (int)t.TotalMinutes + "m";
    }
    if (t.TotalMinutes >= 1)
        return string.Format("{0}m {1}s", (int)t.TotalMinutes, t.Seconds);
    return t.Seconds + "s";
}