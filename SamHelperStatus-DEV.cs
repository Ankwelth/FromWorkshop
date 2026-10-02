// /*
// * S.A.M.HelperStatus Script v1.0.0
// *
// * Tired of complex setups for all your S.A.M. (Space Air Mobile / Static Attack Missile / Self-Assembling Machines - depending on your use!)
// * ships? This script is designed as one global helper with a really easy setup to manage your fleet status!
// *
// * ----------------------------------------------------------------------------------------------------
// * SCRIPT SETUP & CONFIGURATION
// * ----------------------------------------------------------------------------------------------------
// *
// * 1. Installation:
// * - Copy the entire script content into a Space Engineers Programmable Block.
// * - Click "Check Code" and then "Remember & Exit".
// * - Set the "Run" action to run with the argument `Update10` (or `Update100` for less frequent updates).
// * - Run the script once (or wait for the next `Update` cycle).
// *
// * 2. Programmable Block Custom Data (Configuration):
// * Edit the Custom Data of this Programmable Block to configure its behavior.
// * If you delete Custom Data, default values will be applied on next run.
// *
// * [SAMHELPERSTATUS]
// * collector=false             // Set to 'true' for a Collector PB (receives data)
// * // Set to 'false' for a Sender PB (broadcasts its own ship's data)
// * send_delay=10.0             // (Sender only) How often (in seconds) the ship broadcasts its status.
// * refresh_delay=0.5           // (Collector only) How often (in seconds) the LCDs refresh their display.
// * showBatteries=true          // (Collector only) Show battery level in status.
// * showCargo=true              // (Collector only) Show cargo fill level in status.
// * showHydrogen=true           // (Collector only) Show hydrogen tank level in status.
// * showDest=true               // (Collector only) Show ship's destination in status.
// * showPos=true                // (Collector only) Show ship's GPS position in status.
// * toggleAntennaForSend=true   // (Sender only) Temporarily enable antenna during broadcast, then disable if it was off.
// *
// * 3. LCD Setup (for display):
// * To display ship status, rename any Text Panel (LCD) on the same grid as this Programmable Block.
// * Add the tag `[SAMHELPERSTATUS]` to its custom name.
// * Example: `LCD Panel [SAMHELPERSTATUS] Bridge Display`
// *
// * 4. LCD Cloning (for Sender-side content sharing):
// * If this PB is a SENDER, you can make it share the content of other LCDs on its grid.
// * - To clone an LCD's content, add ONE of these tags to its custom name OR Custom Data:
// * - In Custom Name: `[SAMHELPERSTATUS CLONE]`
// * - In Custom Data: `SAMHELPERSTATUS.CLONE`
// * - Example Name: `My Data LCD [SAMHELPERSTATUS CLONE]`
// * - Example Custom Data: `SAMHELPERSTATUS.CLONE=true`
// * - The content of these LCDs will be transmitted and viewable on Collector displays.
// * - Note: Cloned text content is currently limited to 1KB per LCD due to IGC message size limits.
// *
// * 5. Programmable Block Arguments (for Collector LCD Navigation):
// * For Collectors, you can use PB arguments to navigate the LCD display:
// * - `next`: Selects the next ship in the summary list OR the next cloned LCD in the list.
// * - `prev`: Selects the previous ship in the summary list OR the previous cloned LCD in the list.
// * - `select`:
// * - From Ship Summary: Goes to the detailed view of the selected ship.
// * - From Ship Detail: If available, goes to the list of cloned LCDs for that ship.
// * - From Cloned LCD List: Displays the content of the selected cloned LCD.
// * - `screen`: Goes back to the previous screen level (e.g., Cloned LCD Content -> Cloned LCD List -> Ship Detail -> Ship Summary).
// *
// * You can bind these arguments to button panels, timers, or other programmable blocks.
// *
// * ----------------------------------------------------------------------------------------------------
// * ENJOY YOUR ENHANCED FLEET MANAGEMENT!
// * ----------------------------------------------------------------------------------------------------
// */
// Managers and Modules
SamHelperStatusBlockManager _blockManager;
ConfigManager _config;
ShipMonitor _shipMonitor;
ShipTransmitter _transmitter;
ShipReceiver _receiver;
ShipRegistry _shipRegistry;
LCDDisplayManager _lcdDisplay;
Logger _logger;
DestinationManager _destinationManager;

public Program()
{
    _logger = new Logger(Echo);

    _blockManager = new SamHelperStatusBlockManager(GridTerminalSystem, Me, Constants.LCD_TAG);

    _config = new ConfigManager(_blockManager, Constants.INI_SECTION);
    _config.Load();

    _shipRegistry = new ShipRegistry();

    _destinationManager = new DestinationManager(Me, new Random(), _logger);

    _shipMonitor = new ShipMonitor(_blockManager, _destinationManager, new Random());

    _transmitter = new ShipTransmitter(
        IGC,
        _shipMonitor,
        Constants.BROADCAST_TAG,
        _config.SendDelay,
        _logger,
        _config,
        _blockManager
    );

    _receiver = new ShipReceiver(IGC, _shipRegistry, Constants.BROADCAST_TAG, _blockManager.GetProgrammableBlock().GetPosition(), _logger);
    _receiver.RegisterListener();

    _lcdDisplay = new LCDDisplayManager(_blockManager, _shipRegistry, _config, _logger);

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
    double deltaTime = Runtime.TimeSinceLastRun.TotalSeconds;

    // --- Handle incoming arguments for LCD navigation ---
    if (!string.IsNullOrWhiteSpace(argument))
    {
        _lcdDisplay.HandleArgument(argument);
    }
    // --- End argument handling ---

    // --- PB Surface Display (Always update regardless of mode) ---
    Animation.RunPbRotator();

    IMyTextSurface pbSurface = Me.GetSurface(0);
    if (pbSurface != null)
    {
        pbSurface.ContentType = ContentType.TEXT_AND_IMAGE;
        pbSurface.Font = "Monospace";
        pbSurface.FontSize = 0.8f;

        string pbMode = _config.IsCollector ? "COLLECTOR" : "SENDER";
        string pbStatusLine1 = $"S.A.M.HELPERSTATUS V1.0.0 {pbMode} {Animation.PbRunning()}";
        string pbStatusLine2 = "";

        if (_config.IsCollector)
        {
            pbStatusLine2 = $"Known Ships: {_shipRegistry.GetShipCount()}";
        }
        else
        {
            pbStatusLine2 = $"Next broadcast in: {(_config.SendDelay - _transmitter.TimeSinceLastSend):0.0}s";
        }
        pbSurface.WriteText($"{pbStatusLine1}\n{pbStatusLine2}", false);
    }
    // --- End PB Surface Display ---

    if (!_config.IsCollector) // Sender Mode
    {
        _transmitter.Update(deltaTime);

        // Update external LCDs (if any) with sender specific message
        var lcds = _blockManager.GetShipStatusLCDs();
        if (lcds.Any())
        {
            foreach (var lcd in lcds)
            {
                if (lcd.IsFunctional)
                {
                    lcd.WriteText($"This PB is configured as a Sender.\n\n" +
                                  $"Currently broadcasting ship status every {_config.SendDelay:0.0} seconds.\n\n" +
                                  $"Check PB Surface (Front) for detailed status.", false);
                    lcd.ContentType = ContentType.TEXT_AND_IMAGE;
                    lcd.Font = "Debug";
                    lcd.FontSize = 1.0f;
                }
            }
        }
        else
        {
            _logger.Log($"Warning: No LCDs found with tag '{Constants.LCD_TAG}' on this construct to display sender status on.");
        }
    }
    else // Collector Mode
    {
        _receiver.ProcessMessages();
        _receiver.Update(deltaTime, _blockManager.GetProgrammableBlock().GetPosition());
        _lcdDisplay.Update(deltaTime); // This will update the main LCD tables with collected data
    }

    _logger.Log($"Config: IsCollector={_config.IsCollector}");
    _logger.Log($"Send Delay: {_config.SendDelay}s | Refresh Delay: {_config.RefreshDelay}s");
}

public void Save()
{
    _config.Save();
}
}
static class Constants
{
    public const string BROADCAST_TAG = "SHIP_STATUS_IGC_JSON";
    public const string LCD_TAG = "[SAMHELPERSTATUS]";
    public const string CLONE_LCD_NAME_TAG = "[SAMHELPERSTATUS CLONE]";
    public const string CLONE_LCD_CUSTOMDATA_TAG = "SAMHELPERSTATUS.CLONE";
    public const string INI_SECTION = "SAMHELPERSTATUS";
}
class Logger
{
    Action<string> _echoAction;

    public Logger(Action<string> echoAction)
    {
        _echoAction = echoAction;
    }

    public void Log(string message)
    {
        _echoAction?.Invoke(message);
    }
}
class SamHelperStatusBlockManager
{
    IMyGridTerminalSystem _gridTerminalSystem;
    IMyProgrammableBlock _pb;
    string _lcdTag;

    public SamHelperStatusBlockManager(IMyGridTerminalSystem gts, IMyProgrammableBlock pb, string lcdTag)
    {
        _gridTerminalSystem = gts;
        _pb = pb;
        _lcdTag = lcdTag;
    }

    public IMyProgrammableBlock GetProgrammableBlock()
    {
        return _pb;
    }

    public IMyGridTerminalSystem GetGridTerminalSystem()
    {
        return _gridTerminalSystem;
    }

    public string GetLcdTag()
    {
        return _lcdTag;
    }

    public List<IMyTextPanel> GetShipStatusLCDs()
    {
        var lcds = new List<IMyTextPanel>();
        var allBlocksOnConstruct = new List<IMyTerminalBlock>();

        _gridTerminalSystem.GetBlocksOfType(allBlocksOnConstruct, b => b.IsSameConstructAs(_pb));

        foreach (IMyTerminalBlock block in allBlocksOnConstruct)
        {
            if (block is IMyTextPanel && block.CustomName.Contains(_lcdTag))
            {
                lcds.Add(block as IMyTextPanel);
            }
        }
        return lcds;
    }
}
class ConfigManager
{
    SamHelperStatusBlockManager _blockManager;
    MyIni _ini = new MyIni();
    string _sectionName;

    public bool IsCollector { get; private set; }
    public double SendDelay { get; private set; }
    public double RefreshDelay { get; private set; }
    public bool ShowBatteries { get; private set; }
    public bool ShowCargo { get; private set; }
    public bool ShowHydrogen { get; private set; }
    public bool ShowDest { get; private set; }
    public bool ShowPos { get; private set; }
    public bool ToggleAntennaForSend { get; private set; }

    public ConfigManager(SamHelperStatusBlockManager blockManager, string sectionName)
    {
        _blockManager = blockManager;
        _sectionName = sectionName;
    }

    public void Load()
    {
        MyIniParseResult result;
        if (!_ini.TryParse(_blockManager.GetProgrammableBlock().CustomData, out result))
        {
            _blockManager.GetProgrammableBlock().CustomData = string.Empty;
            _ini.Clear();
        }

        IsCollector = _ini.Get(_sectionName, "collector").ToBoolean(false);
        SendDelay = _ini.Get(_sectionName, "send_delay").ToDouble(10);
        RefreshDelay = _ini.Get(_sectionName, "refresh_delay").ToDouble(0.5);
        ShowBatteries = _ini.Get(_sectionName, "showBatteries").ToBoolean(true);
        ShowCargo = _ini.Get(_sectionName, "showCargo").ToBoolean(true);
        ShowHydrogen = _ini.Get(_sectionName, "showHydrogen").ToBoolean(true);
        ShowDest = _ini.Get(_sectionName, "showDest").ToBoolean(true);
        ShowPos = _ini.Get(_sectionName, "showPos").ToBoolean(true);
        ToggleAntennaForSend = _ini.Get(_sectionName, "toggleAntennaForSend").ToBoolean(true);

        Save();
    }

    public void Save()
    {
        _ini.Set(_sectionName, "collector", IsCollector);
        _ini.Set(_sectionName, "send_delay", SendDelay);
        _ini.Set(_sectionName, "refresh_delay", RefreshDelay);
        _ini.Set(_sectionName, "showBatteries", ShowBatteries);
        _ini.Set(_sectionName, "showCargo", ShowCargo);
        _ini.Set(_sectionName, "showHydrogen", ShowHydrogen);
        _ini.Set(_sectionName, "showDest", ShowDest);
        _ini.Set(_sectionName, "showPos", ShowPos);
        _ini.Set(_sectionName, "toggleAntennaForSend", ToggleAntennaForSend);
        _blockManager.GetProgrammableBlock().CustomData = _ini.ToString();
    }
}
class ShipInfo
{
    public string Name;
    public double BatteryLevel;
    public double CargoFill;
    public double HydrogenLevel;
    public string Destination;
    public Vector3D Position;
    public DateTime LastReceived;
    public double Distance;
    public Dictionary<string, string> ClonedLcds; // New: To store cloned LCD content
}
class ShipMonitor
{
    SamHelperStatusBlockManager _blockManager;
    IMyGridTerminalSystem _gts;
    IMyProgrammableBlock _pb;
    Random _rand;
    DestinationManager _destinationManager;

    public ShipMonitor(SamHelperStatusBlockManager blockManager, DestinationManager destinationManager, Random rand)
    {
        _blockManager = blockManager;
        _gts = blockManager.GetGridTerminalSystem();
        _pb = blockManager.GetProgrammableBlock();
        _rand = rand;
        _destinationManager = destinationManager;
    }

    public string GetShipName()
    {
        return _pb.CubeGrid.CustomName;
    }

    public Vector3D GetMyPosition()
    {
        return _pb.GetPosition();
    }

    public double GetBatteryLevel()
    {
        var batteries = new List<IMyBatteryBlock>();
        _gts.GetBlocksOfType(batteries, b => b.IsSameConstructAs(_pb));
        if (batteries.Count == 0) return 0.0;

        double currentPower = 0;
        double maxPower = 0;
        foreach (var battery in batteries)
        {
            currentPower += battery.CurrentStoredPower;
            maxPower += battery.MaxStoredPower;
        }

        return maxPower == 0 ? 0.0 : (currentPower / maxPower) * 100.0;
    }

    public double GetCargoFill()
    {
        var cargoBlocks = new List<IMyTerminalBlock>();
        _gts.GetBlocksOfType(cargoBlocks, b => b.IsSameConstructAs(_pb) && b.HasInventory);
        double current = 0, max = 0;
        foreach (var block in cargoBlocks)
        {
            var inv = block.GetInventory(0);
            if (inv != null)
            {
                current += (double)inv.CurrentVolume;
                max += (double)inv.MaxVolume;
            }
        }
        return max == 0 ? 0.0 : (current / max) * 100.0;
    }

    public double GetHydrogenLevel()
    {
        var tanks = new List<IMyGasTank>();
        _gts.GetBlocksOfType(tanks, t => t.IsSameConstructAs(_pb) && t.DefinitionDisplayNameText.Contains("Hydrogen"));
        if (tanks.Count == 0) return 0.0;

        double totalFilledRatio = 0;
        foreach (var tank in tanks)
        {
            totalFilledRatio += tank.FilledRatio;
        }

        return (totalFilledRatio / tanks.Count) * 100.0;
    }

    public string GetShipDestination()
    {
        return _destinationManager.GetCurrentDestination();
    }

    /// <summary>
    /// Gathers content from LCDs tagged for cloning.
    /// </summary>
    /// <returns>A dictionary where key is LCD name and value is its content.</returns>
    public Dictionary<string, string> GetClonedLcdContents()
    {
        var clonedLcds = new Dictionary<string, string>();
        var allLcds = new List<IMyTextPanel>();
        _gts.GetBlocksOfType(allLcds, b => b.IsSameConstructAs(_pb));

        foreach (var lcd in allLcds)
        {
            // Check if LCD is identified for cloning
            if (lcd.CustomName.Contains(Constants.CLONE_LCD_NAME_TAG) ||
                lcd.CustomData.Contains(Constants.CLONE_LCD_CUSTOMDATA_TAG))
            {
                // Use a clean version of the name or just the custom name
                string lcdId = lcd.CustomName.Replace(Constants.CLONE_LCD_NAME_TAG, "").Trim();
                if (string.IsNullOrWhiteSpace(lcdId))
                {
                    lcdId = "UnnamedClonedLCD"; // Fallback if name is just the tag
                }

                // Limit text length to avoid IGC message size limits
                string content = lcd.GetText();
                if (content.Length > 1024) content = content.Substring(0, 1024) + "..."; // Max 1KB per LCD

                clonedLcds[lcdId] = content;
            }
        }
        return clonedLcds;
    }
}
class ShipTransmitter
{
    IMyIntergridCommunicationSystem _igc;
    ShipMonitor _monitor;
    string _broadcastTag;
    double _sendDelay;
    double _timeSinceLastSend = 0;
    Logger _logger;
    ConfigManager _config;
    SamHelperStatusBlockManager _blockManager;

    List<IMyRadioAntenna> _activeAntennas = new List<IMyRadioAntenna>();
    Dictionary<IMyRadioAntenna, bool> _initialAntennaStates = new Dictionary<IMyRadioAntenna, bool>();

    public double TimeSinceLastSend => _timeSinceLastSend;

    public ShipTransmitter(IMyIntergridCommunicationSystem igc, ShipMonitor monitor, string broadcastTag, double sendDelay, Logger logger, ConfigManager config, SamHelperStatusBlockManager blockManager)
    {
        _igc = igc;
        _monitor = monitor;
        _broadcastTag = broadcastTag;
        _sendDelay = sendDelay;
        _logger = logger;
        _config = config;
        _blockManager = blockManager;
    }

    public void Update(double deltaTime)
    {
        _timeSinceLastSend += deltaTime;
        if (_timeSinceLastSend >= _sendDelay)
        {
            if (_config.ToggleAntennaForSend)
            {
                _activeAntennas.Clear();
                _initialAntennaStates.Clear();

                _blockManager.GetGridTerminalSystem().GetBlocksOfType(
                    _activeAntennas,
                    a => a.IsSameConstructAs(_blockManager.GetProgrammableBlock()) && a is IMyRadioAntenna
                );

                if (_activeAntennas.Any())
                {
                    foreach (var antenna in _activeAntennas)
                    {
                        _initialAntennaStates[antenna] = antenna.Enabled;
                        if (!antenna.Enabled)
                        {
                            antenna.Enabled = true;
                            _logger.Log($"Antenna '{antenna.CustomName}' turned ON for broadcast.");
                        }
                    }
                }
                else
                {
                    _logger.Log("Antenna Control: No antennas found on construct to toggle.");
                }
            }

            string statusJson = BuildStatusJson();
            _igc.SendBroadcastMessage(_broadcastTag, statusJson);
            _timeSinceLastSend = 0;
            _logger.Log($"Sender: Sent status. Next in {_sendDelay:0.0}s");

            if (_config.ToggleAntennaForSend && _activeAntennas.Any())
            {
                foreach (var antenna in _activeAntennas)
                {
                    if (_initialAntennaStates.ContainsKey(antenna) && !_initialAntennaStates[antenna] && antenna.Enabled)
                    {
                        antenna.Enabled = false;
                        _logger.Log($"Antenna '{antenna.CustomName}' turned OFF after broadcast (was initially off).");
                    }
                }
                _activeAntennas.Clear();
                _initialAntennaStates.Clear();
            }
        }
    }

    string BuildStatusJson()
    {
        var sb = new StringBuilder();
        sb.Append("{");

        sb.Append($"\"Name\":\"{JsonHelper.JsonEscape(_monitor.GetShipName())}\"");
        sb.Append($",\"Batt\":{_monitor.GetBatteryLevel():0.0}");
        sb.Append($",\"Cargo\":{_monitor.GetCargoFill():0.0}");
        sb.Append($",\"H2\":{_monitor.GetHydrogenLevel():0.0}");
        sb.Append($",\"Dest\":\"{JsonHelper.JsonEscape(_monitor.GetShipDestination())}\"");

        Vector3D pos = _monitor.GetMyPosition();
        sb.Append($",\"Pos\":{{\"X\":{pos.X:0.0},\"Y\":{pos.Y:0.0},\"Z\":{pos.Z:0.0}}}");

        // New: Add cloned LCD content
        var clonedLcds = _monitor.GetClonedLcdContents();
        if (clonedLcds != null && clonedLcds.Any())
        {
            sb.Append($",\"ClonedLcds\":{JsonHelper.SerializeDictionary(clonedLcds)}");
        }

        sb.Append("}");
        return sb.ToString();
    }
}
class ShipRegistry
{
    Dictionary<string, ShipInfo> _shipStatusDict = new Dictionary<string, ShipInfo>();

    public void AddOrUpdateShip(ShipInfo info)
    {
        _shipStatusDict[info.Name] = info;
    }

    public IEnumerable<ShipInfo> GetAllShips()
    {
        return _shipStatusDict.Values.OrderBy(s => s.Name);
    }

    public void CleanExpiredShips(double timeoutSeconds = 60)
    {
        var keysToRemove = _shipStatusDict.Where(kvp => (DateTime.UtcNow - kvp.Value.LastReceived).TotalSeconds > timeoutSeconds).Select(kvp => kvp.Key).ToList();
        foreach (var key in keysToRemove)
        {
            _shipStatusDict.Remove(key);
        }
    }

    public int GetShipCount()
    {
        return _shipStatusDict.Count;
    }
}
class ShipReceiver
{
    IMyIntergridCommunicationSystem _igc;
    IMyBroadcastListener _listener;
    ShipRegistry _registry;
    string _broadcastTag;
    Vector3D _collectorPosition;
    int _messagesProcessedCount = 0;
    Logger _logger;

    public ShipReceiver(IMyIntergridCommunicationSystem igc, ShipRegistry registry, string broadcastTag, Vector3D initialCollectorPos, Logger logger)
    {
        _igc = igc;
        _registry = registry;
        _broadcastTag = broadcastTag;
        _collectorPosition = initialCollectorPos;
        _logger = logger;
    }

    public void RegisterListener()
    {
        _listener = _igc.RegisterBroadcastListener(_broadcastTag);
        _listener.SetMessageCallback(_broadcastTag);
    }

    public void Update(double deltaTime, Vector3D currentCollectorPos)
    {
        _collectorPosition = currentCollectorPos;
    }

    public void ProcessMessages()
    {
        _messagesProcessedCount = 0;
        while (_listener.HasPendingMessage)
        {
            var msg = _listener.AcceptMessage();
            if (msg.Tag == _broadcastTag && msg.Data is string)
            {
                string data = msg.Data.ToString();
                ParseAndRegisterShipStatus(data);
                _messagesProcessedCount++;
            }
        }
        if (_messagesProcessedCount > 0)
        {
            _logger.Log($"Collector: Processed {_messagesProcessedCount} messages this tick.");
        }
    }

    void ParseAndRegisterShipStatus(string json)
    {
        var parsedJson = JsonHelper.ParseSimpleJson(json);

        string name = parsedJson.ContainsKey("Name") ? parsedJson["Name"].Trim('"') : "Unknown Ship";

        double batteryLevel;
        if (!parsedJson.ContainsKey("Batt") || !double.TryParse(parsedJson["Batt"], out batteryLevel))
            batteryLevel = 0;

        double cargoFill;
        if (!parsedJson.ContainsKey("Cargo") || !double.TryParse(parsedJson["Cargo"], out cargoFill))
            cargoFill = 0;

        double hydrogenLevel;
        if (!parsedJson.ContainsKey("H2") || !double.TryParse(parsedJson["H2"], out hydrogenLevel))
            hydrogenLevel = 0;

        string destination = parsedJson.ContainsKey("Dest") ? parsedJson["Dest"].Trim('"') : "N/A";

        Vector3D senderPos = Vector3D.Zero;
        double distance = -1;

        if (parsedJson.ContainsKey("Pos"))
        {
            string posJsonString = parsedJson["Pos"];

            double x = 0, y = 0, z = 0;

            string content = posJsonString.Trim('{', '}');
            string[] components = content.Split(',');

            foreach (var component in components)
            {
                string[] keyValue = component.Split(':');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim().Trim('"');
                    string value = keyValue[1].Trim();

                    if (key == "X") double.TryParse(value, out x);
                    else if (key == "Y") double.TryParse(value, out y);
                    else if (key == "Z") double.TryParse(value, out z);
                }
            }
            senderPos = new Vector3D(x, y, z);
            distance = Vector3D.Distance(_collectorPosition, senderPos);
        }

        // New: Parse ClonedLcds
        Dictionary<string, string> clonedLcds = null;
        if (parsedJson.ContainsKey("ClonedLcds"))
        {
            string clonedLcdsJson = parsedJson["ClonedLcds"];
            clonedLcds = JsonHelper.ParseJsonDictionary(clonedLcdsJson);
        }

        _registry.AddOrUpdateShip(new ShipInfo
        {
            Name = name,
            BatteryLevel = batteryLevel,
            CargoFill = cargoFill,
            HydrogenLevel = hydrogenLevel,
            Destination = destination,
            Position = senderPos,
            LastReceived = DateTime.UtcNow,
            Distance = distance,
            ClonedLcds = clonedLcds // Assign parsed cloned LCDs
        });
    }
}
class LCDDisplayManager
{
    // Enum for different display screens
        // Enum for different display screens
    enum DisplayScreen { ShipSummary, SingleShipDetail, ClonedLcdList, ClonedLcdContent }

    SamHelperStatusBlockManager _blockManager;
    ShipRegistry _registry;
    ConfigManager _config;
    double _refreshDelay;
    double _timeSinceLastRefresh = 0;
    Logger _logger;

    DisplayScreen _currentScreen = DisplayScreen.ShipSummary;
    int _selectedShipIndex = 0; // Index of the selected ship in summary view
    string _selectedShipName = null; // Name of the ship for detail view
    int _selectedClonedLcdIndex = 0; // Index of the selected cloned LCD in the list view

    public LCDDisplayManager(SamHelperStatusBlockManager blockManager, ShipRegistry registry, ConfigManager config, Logger logger)
    {
        _blockManager = blockManager;
        _registry = registry;
        _config = config;
        _refreshDelay = config.RefreshDelay;
        _logger = logger;
    }

    /// <summary>
    /// Handles navigation commands from programmable block arguments.
    /// </summary>
    /// <param name="argument">The argument string (e.g., "next", "prev", "select", "screen").</param>
    public void HandleArgument(string argument)
    {
        switch (argument.ToLower().Trim())
        {
            case "next":
                HandleNextCommand();
                break;
            case "prev":
                HandlePrevCommand();
                break;
            case "select":
                HandleSelectCommand();
                break;
            case "screen": // "screen" command to go back a screen
                HandleScreenCommand();
                break;
            default:
                // Log unknown arguments if necessary, but don't stop execution
                // _logger.Log($"Unknown LCD command: '{argument}'");
                break;
        }
    }

    void HandleNextCommand()
    {
        if (_currentScreen == DisplayScreen.ShipSummary)
        {
            var ships = _registry.GetAllShips().ToList();
            if (ships.Count > 0)
            {
                _selectedShipIndex = (_selectedShipIndex + 1) % ships.Count;
                _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
            }
            else
            {
                _selectedShipIndex = 0; // Reset if no ships
            }
        }
        else if (_currentScreen == DisplayScreen.ClonedLcdList)
        {
            ShipInfo selectedInfo = _registry.GetAllShips().FirstOrDefault(s => s.Name == _selectedShipName);
            if (selectedInfo?.ClonedLcds != null && selectedInfo.ClonedLcds.Any())
            {
                _selectedClonedLcdIndex = (_selectedClonedLcdIndex + 1) % selectedInfo.ClonedLcds.Count;
                _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
            }
            else
            {
                _selectedClonedLcdIndex = 0;
            }
        }
    }

    void HandlePrevCommand()
    {
        if (_currentScreen == DisplayScreen.ShipSummary)
        {
            var ships = _registry.GetAllShips().ToList();
            if (ships.Count > 0)
            {
                _selectedShipIndex = (_selectedShipIndex - 1 + ships.Count) % ships.Count;
                _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
            }
            else
            {
                _selectedShipIndex = 0; // Reset if no ships
            }
        }
        else if (_currentScreen == DisplayScreen.ClonedLcdList)
        {
            ShipInfo selectedInfo = _registry.GetAllShips().FirstOrDefault(s => s.Name == _selectedShipName);
            if (selectedInfo?.ClonedLcds != null && selectedInfo.ClonedLcds.Any())
            {
                _selectedClonedLcdIndex = (_selectedClonedLcdIndex - 1 + selectedInfo.ClonedLcds.Count) % selectedInfo.ClonedLcds.Count;
                _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
            }
            else
            {
                _selectedClonedLcdIndex = 0;
            }
        }
    }

    void HandleSelectCommand()
    {
        if (_currentScreen == DisplayScreen.ShipSummary)
        {
            GoToDetailScreen();
        }
        else if (_currentScreen == DisplayScreen.SingleShipDetail)
        {
            GoToClonedLcdListScreen();
        }
        else if (_currentScreen == DisplayScreen.ClonedLcdList)
        {
            GoToClonedLcdContentScreen();
        }
    }

    void HandleScreenCommand()
    {
        if (_currentScreen == DisplayScreen.ClonedLcdContent)
        {
            GoToClonedLcdListScreen(); // Back to list from content
        }
        else if (_currentScreen == DisplayScreen.ClonedLcdList)
        {
            GoToDetailScreen(); // Back to ship detail from list
        }
        else if (_currentScreen == DisplayScreen.SingleShipDetail)
        {
            GoToSummaryScreen(); // Back to summary from detail
        }
    }


    void GoToSummaryScreen()
    {
        _currentScreen = DisplayScreen.ShipSummary;
        _selectedShipName = null;
        _selectedClonedLcdIndex = 0;
        _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
    }

    void GoToDetailScreen()
    {
        var ships = _registry.GetAllShips().ToList();
        if (ships.Count > 0 && _selectedShipIndex >= 0 && _selectedShipIndex < ships.Count)
        {
            _selectedShipName = ships[_selectedShipIndex].Name;
            _currentScreen = DisplayScreen.SingleShipDetail;
            _selectedClonedLcdIndex = 0; // Reset cloned LCD selection
            _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
        }
        else
        {
            _logger.Log("Cannot go to detail screen: No ship selected or no ships available.");
            GoToSummaryScreen(); // Fallback
        }
    }

    void GoToClonedLcdListScreen()
    {
        ShipInfo selectedInfo = _registry.GetAllShips().FirstOrDefault(s => s.Name == _selectedShipName);
        if (selectedInfo?.ClonedLcds != null && selectedInfo.ClonedLcds.Any())
        {
            _currentScreen = DisplayScreen.ClonedLcdList;
            _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
        }
        else
        {
            _logger.Log($"No cloned LCDs available for '{_selectedShipName}'. Remaining on detail screen.");
            // Optionally: Go back to detail screen or summary if forced
            _currentScreen = DisplayScreen.SingleShipDetail; // Stay on detail if no LCDs
            _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
        }
    }

    void GoToClonedLcdContentScreen()
    {
        ShipInfo selectedInfo = _registry.GetAllShips().FirstOrDefault(s => s.Name == _selectedShipName);
        if (selectedInfo?.ClonedLcds != null && selectedInfo.ClonedLcds.Any())
        {
            if (_selectedClonedLcdIndex >= 0 && _selectedClonedLcdIndex < selectedInfo.ClonedLcds.Count)
            {
                _currentScreen = DisplayScreen.ClonedLcdContent;
                _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
            }
            else
            {
                _logger.Log($"No cloned LCD selected at index {_selectedClonedLcdIndex}. Remaining on list screen.");
                _currentScreen = DisplayScreen.ClonedLcdList; // Stay on list if invalid index
                _timeSinceLastRefresh = _refreshDelay; // Force immediate refresh
            }
        }
        else
        {
            _logger.Log($"No cloned LCDs available for '{_selectedShipName}'. Returning to detail screen.");
            GoToDetailScreen(); // Fallback if suddenly no LCDs
        }
    }


    public void Update(double deltaTime)
    {
        _timeSinceLastRefresh += deltaTime;
        if (_timeSinceLastRefresh >= _refreshDelay)
        {
            Animation.Run();
            DisplayCurrentScreen();
            _registry.CleanExpiredShips();
            _timeSinceLastRefresh = 0;
        }
    }

    void DisplayCurrentScreen()
    {
        string textToDisplay;
        switch (_currentScreen)
        {
            case DisplayScreen.ShipSummary:
                textToDisplay = GenerateShipSummaryText();
                break;
            case DisplayScreen.SingleShipDetail:
                textToDisplay = GenerateSingleShipDetailText(_selectedShipName);
                break;
            case DisplayScreen.ClonedLcdList:
                textToDisplay = GenerateClonedLcdListText(_selectedShipName);
                break;
            case DisplayScreen.ClonedLcdContent:
                textToDisplay = GenerateClonedLcdContentText(_selectedShipName, _selectedClonedLcdIndex);
                break;
            default:
                textToDisplay = "Error: Unknown screen state.";
                break;
        }

        WriteToLCDs(textToDisplay);
    }


    string GenerateShipSummaryText()
    {
        var rows = new List<Dictionary<string, string>>();
        var headers = new List<string> { "SHIP" };

        if (_config.ShowBatteries) headers.Add("Batt");
        if (_config.ShowCargo) headers.Add("Cargo");
        if (_config.ShowHydrogen) headers.Add("H2");
        if (_config.ShowDest) headers.Add("Dest");
        headers.Add("Dist");
        headers.Add("Up");
        if (_config.ShowPos) headers.Add("Pos");

        var colWidths = new Dictionary<string, int>();
        foreach (var h in headers) colWidths[h] = h.Length;

        var allShips = _registry.GetAllShips().ToList();

        // Adjust selected index if it's out of bounds (e.g., ship removed)
        if (_selectedShipIndex >= allShips.Count)
        {
            _selectedShipIndex = allShips.Count > 0 ? allShips.Count - 1 : 0;
        }
        if (_selectedShipIndex < 0 && allShips.Count > 0)
        {
            _selectedShipIndex = 0;
        }

        for (int i = 0; i < allShips.Count; i++)
        {
            var info = allShips[i];
            var row = new Dictionary<string, string>();

            string shipName = info.Name;
            if (i == _selectedShipIndex)
            {
                shipName = "-> " + shipName;
            }
            else
            {
                shipName = "   " + shipName;
            }
            row["SHIP"] = shipName;

            if (_config.ShowBatteries) row["Batt"] = $"{info.BatteryLevel:0}%";
            if (_config.ShowCargo) row["Cargo"] = $"{info.CargoFill:0}%";
            if (_config.ShowHydrogen) row["H2"] = $"{info.HydrogenLevel:0}%";
            if (_config.ShowDest) row["Dest"] = info.Destination;
            row["Dist"] = info.Distance >= 0 ? $"{info.Distance:0}m" : "N/A";
            row["Up"] = $"{(DateTime.UtcNow - info.LastReceived).TotalSeconds:0}s";
            if (_config.ShowPos) row["Pos"] = $"GPS:{info.Position.X:0},{info.Position.Y:0},{info.Position.Z:0}";

            foreach (var header in headers)
            {
                if (row.ContainsKey(header))
                    colWidths[header] = Math.Max(colWidths[header], row[header].Length);
            }

            rows.Add(row);
        }

        string modeText = _config.IsCollector ? "Collecting" : "Sending";
        var lines = new List<string>
        {
            $"S.A.M.HELPERSTATUS V1.0.0 {modeText} ... {Animation.Rotator()}",
            $"Total Ships: {_registry.GetShipCount()}",
            "",
            string.Join(" | ", headers.Select(h => h.PadRight(colWidths[h]))),
            string.Join("-+-", headers.Select(h => new string('-', colWidths[h])))
        };

        foreach (var row in rows)
        {
            string line = string.Join(" | ",
                headers.Select(h =>
                    row.ContainsKey(h)
                    ? (h == "SHIP" || h == "Dest" || h == "Pos"
                        ? row[h].PadRight(colWidths[h])
                        : row[h].PadLeft(colWidths[h]))
                    : new string(' ', colWidths[h])
                ));
            lines.Add(line);
        }
        lines.Add("");
        lines.Add("Commands: 'next', 'prev' to select. 'select' for details.");
        return string.Join("\n", lines);
    }

    string GenerateSingleShipDetailText(string shipName)
    {
        ShipInfo selectedInfo = _registry.GetAllShips().FirstOrDefault(s => s.Name == shipName);

        if (selectedInfo == null)
        {
            _logger.Log($"Error: Selected ship '{shipName}' not found in registry. Returning to summary.");
            GoToSummaryScreen();
            return GenerateShipSummaryText();
        }

        var lines = new List<string>
        {
            $"S.A.M.HELPERSTATUS V1.0.0 Detail ... {Animation.Rotator()}",
            "",
            $"Ship: {selectedInfo.Name}",
            $"Last Update: {(DateTime.UtcNow - selectedInfo.LastReceived).TotalSeconds:0}s ago",
            "",
            "--- Status ---"
        };

        if (_config.ShowBatteries) lines.Add($"Battery: {selectedInfo.BatteryLevel:0.0}%");
        if (_config.ShowCargo) lines.Add($"Cargo: {selectedInfo.CargoFill:0.0}%");
        if (_config.ShowHydrogen) lines.Add($"Hydrogen: {selectedInfo.HydrogenLevel:0.0}%");
        if (_config.ShowDest) lines.Add($"Destination: {selectedInfo.Destination} {Animation.Destination()}");
        lines.Add($"Distance: {selectedInfo.Distance:0.0}m");
        if (_config.ShowPos) lines.Add($"Position: GPS:{selectedInfo.Position.X:0.0},{selectedInfo.Position.Y:0.0},{selectedInfo.Position.Z:0.0}");

        lines.Add("");
        lines.Add("--- Commands ---");
        if (selectedInfo.ClonedLcds != null && selectedInfo.ClonedLcds.Any())
        {
            lines.Add($"'select' to view {selectedInfo.ClonedLcds.Count} cloned LCD(s).");
        }
        else
        {
            lines.Add("No cloned LCDs from this ship.");
        }
        lines.Add("'screen' to go back to summary.");

        return string.Join("\n", lines);
    }

    string GenerateClonedLcdListText(string shipName)
    {
        ShipInfo selectedInfo = _registry.GetAllShips().FirstOrDefault(s => s.Name == shipName);

        if (selectedInfo == null || selectedInfo.ClonedLcds == null || !selectedInfo.ClonedLcds.Any())
        {
            _logger.Log($"No cloned LCDs found for '{shipName}'. Returning to detail.");
            GoToDetailScreen();
            return GenerateSingleShipDetailText(shipName);
        }

        var lines = new List<string>
        {
            $"S.A.M.HELPERSTATUS V1.0.0 Cloned LCDs for {shipName} ... {Animation.Rotator()}",
            "",
            "--- Available LCDs ---"
        };

        var lcdKeys = selectedInfo.ClonedLcds.Keys.ToList();

        // Adjust selected index if out of bounds
        if (_selectedClonedLcdIndex >= lcdKeys.Count)
        {
            _selectedClonedLcdIndex = lcdKeys.Count > 0 ? lcdKeys.Count - 1 : 0;
        }
        if (_selectedClonedLcdIndex < 0 && lcdKeys.Count > 0)
        {
            _selectedClonedLcdIndex = 0;
        }


        for (int i = 0; i < lcdKeys.Count; i++)
        {
            string key = lcdKeys[i];
            string line = i == _selectedClonedLcdIndex ? "-> " : "   ";
            line += key;
            lines.Add(line);
        }

        lines.Add("");
        lines.Add("--- Commands ---");
        lines.Add("'next', 'prev' to select. 'select' to view content.");
        lines.Add("'screen' to go back to ship details.");

        return string.Join("\n", lines);
    }

    string GenerateClonedLcdContentText(string shipName, int lcdIndex)
    {
        ShipInfo selectedInfo = _registry.GetAllShips().FirstOrDefault(s => s.Name == shipName);

        if (selectedInfo == null || selectedInfo.ClonedLcds == null || !selectedInfo.ClonedLcds.Any())
        {
            _logger.Log($"No cloned LCDs found for '{shipName}'. Returning to list.");
            GoToClonedLcdListScreen();
            return GenerateClonedLcdListText(shipName);
        }

        var lcdKeys = selectedInfo.ClonedLcds.Keys.ToList();
        if (lcdIndex < 0 || lcdIndex >= lcdKeys.Count)
        {
            _logger.Log($"Invalid LCD index {lcdIndex}. Returning to list.");
            GoToClonedLcdListScreen();
            return GenerateClonedLcdListText(shipName);
        }

        string lcdKey = lcdKeys[lcdIndex];
        string lcdContent = selectedInfo.ClonedLcds[lcdKey];

        var lines = new List<string>
        {
            $"S.A.M.HELPERSTATUS V1.0.0 Content: {lcdKey} ... {Animation.Rotator()}",
            $"From Ship: {shipName}",
            "--------------------",
            lcdContent, // The actual LCD content
            "--------------------",
            "--- Commands ---",
            "'screen' to go back to LCD list."
        };

        return string.Join("\n", lines);
    }

    void WriteToLCDs(string text)
    {
        var lcds = _blockManager.GetShipStatusLCDs();
        if (lcds.Count == 0)
        {
            _logger.Log($"Error: No LCDs found with tag '{_blockManager.GetLcdTag()}' on this construct to display status on.");
            return;
        }

        foreach (var lcd in lcds)
        {
            if (lcd.IsFunctional)
            {
                lcd.WriteText(text);
                lcd.ContentType = ContentType.TEXT_AND_IMAGE;
                lcd.Font = "Monospace";
                lcd.FontSize = 0.6f;
            }
        }
    }
}
class DestinationManager
{
    IMyProgrammableBlock _pb;
    Random _rand;
    Logger _logger;

    private readonly string[] _defaultRandomDestinations = new string[]
    {
        "Titan Outpost", "Asteroid Belt Hub", "Moon Relay Node", "Deep Core Station",
        "Gamma Mining Site", "Neptune’s Watch", "Lost Freight Node", "Epsilon Dockyard",
        "Orleans Command", "Jupiter Mining Corp", "Mars Colony Alpha", "Saturn Gas Station"
    };

    public DestinationManager(IMyProgrammableBlock pb, Random rand, Logger logger)
    {
        _pb = pb;
        _rand = rand;
        _logger = logger;
    }

    public string GetCurrentDestination()
    {
        return _defaultRandomDestinations[_rand.Next(_defaultRandomDestinations.Length)];
    }

    public void SetDestination(string newDestination)
    {
        _logger.Log($"Destination requested to be set: '{newDestination}'. (Not yet persistent)");
    }
}
static class JsonHelper
{
    public static string JsonEscape(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        // Escape backslashes first, then double quotes. Newlines/tabs might also be needed
        s = s.Replace("\\", "\\\\");
        s = s.Replace("\"", "\\\"");
        s = s.Replace("\n", "\\n");
        s = s.Replace("\r", "\\r");
        s = s.Replace("\t", "\\t");
        return s;
    }

    /// <summary>
    /// Parses a flat JSON object string (e.g., {"key":"value", "nestedKey":"{...}"}) into a dictionary.
    /// It does not recursively parse nested JSON strings.
    /// </summary>
    public static Dictionary<string, string> ParseSimpleJson(string json)
    {
        var result = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(json) || json.Length < 2 || json[0] != '{' || json[json.Length - 1] != '}')
        {
            return result;
        }

        string content = json.Substring(1, json.Length - 2); // Remove outer braces
        var pairs = SplitJsonPairs(content); // Split into "key":"value" strings

        foreach (var pair in pairs)
        {
            int colonIndex = -1;
            bool inQuote = false;
            for (int i = 0; i < pair.Length; i++)
            {
                if (pair[i] == '"' && (i == 0 || pair[i - 1] != '\\'))
                    inQuote = !inQuote;

                if (pair[i] == ':' && !inQuote)
                {
                    colonIndex = i;
                    break;
                }
            }

            if (colonIndex == -1)
            {
                continue;
            }

            string key = pair.Substring(0, colonIndex).Trim().Trim('"');
            string value = pair.Substring(colonIndex + 1).Trim();

            // If value starts and ends with a quote, remove them.
            // But only if it's not a nested JSON object/array itself
            if (value.StartsWith("\"") && value.EndsWith("\"") && !(value.StartsWith("{\"") && value.EndsWith("}")))
            {
                value = value.Substring(1, value.Length - 2);
                value = JsonUnescape(value); // Unescape the content
            }

            result[key] = value;
        }
        return result;
    }

    /// <summary>
    /// Unescapes common JSON escape sequences.
    /// </summary>
    public static string JsonUnescape(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        s = s.Replace("\\\"", "\"");
        s = s.Replace("\\\\", "\\");
        s = s.Replace("\\n", "\n");
        s = s.Replace("\\r", "\r");
        s = s.Replace("\\t", "\t");
        return s;
    }


    /// <summary>
    /// Splits a JSON object's content string into key-value pair strings, handling nested structures.
    /// </summary>
    public static List<string> SplitJsonPairs(string content)
    {
        var pairs = new List<string>();
        int depth = 0;
        int startIndex = 0;
        bool inQuote = false;

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];

            // Handle escaped quotes within strings
            if (c == '"' && (i == 0 || content[i - 1] != '\\'))
            {
                inQuote = !inQuote;
            }

            // Split only at top-level commas and outside of quotes
            if (c == ',' && depth == 0 && !inQuote)
            {
                pairs.Add(content.Substring(startIndex, i - startIndex).Trim());
                startIndex = i + 1;
            }
            else if (c == '{' || c == '[') // Increase depth for objects/arrays
            {
                if (!inQuote) depth++;
            }
            else if (c == '}' || c == ']') // Decrease depth for objects/arrays
            {
                if (!inQuote) depth--;
            }
        }
        if (startIndex < content.Length) // Add the last pair
        {
            pairs.Add(content.Substring(startIndex, content.Length - startIndex).Trim());
        }
        return pairs;
    }

    /// <summary>
    /// Serializes a dictionary into a JSON object string (e.g., {"key1":"value1", "key2":"value2"}).
    /// Values are also JSON escaped.
    /// </summary>
    public static string SerializeDictionary(Dictionary<string, string> dict)
    {
        if (dict == null || !dict.Any())
        {
            return "{}";
        }

        var sb = new StringBuilder();
        sb.Append("{");
        bool first = true;
        foreach (var kvp in dict)
        {
            if (!first)
            {
                sb.Append(",");
            }
            sb.Append($"\"{JsonEscape(kvp.Key)}\":\"{JsonEscape(kvp.Value)}\"");
            first = false;
        }
        sb.Append("}");
        return sb.ToString();
    }

    /// <summary>
    /// Parses a simple JSON object string containing only string key-value pairs (e.g., {"key":"value"}).
    /// This is intended for the ClonedLcds dictionary.
    /// </summary>
    public static Dictionary<string, string> ParseJsonDictionary(string json)
    {
        var result = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(json) || json.Length < 2 || json[0] != '{' || json[json.Length - 1] != '}')
        {
            return result;
        }

        string content = json.Substring(1, json.Length - 2); // Remove outer braces
        var pairs = SplitJsonPairs(content); // Use the robust splitter

        foreach (var pair in pairs)
        {
            int colonIndex = -1;
            bool inQuote = false;
            for (int i = 0; i < pair.Length; i++)
            {
                if (pair[i] == '"' && (i == 0 || pair[i - 1] != '\\'))
                    inQuote = !inQuote;

                if (pair[i] == ':' && !inQuote)
                {
                    colonIndex = i;
                    break;
                }
            }

            if (colonIndex == -1)
            {
                continue;
            }

            string key = pair.Substring(0, colonIndex).Trim().Trim('"');
            string value = pair.Substring(colonIndex + 1).Trim();

            // Remove outer quotes and unescape value
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
                value = JsonUnescape(value);
            }

            result[key] = value;
        }
        return result;
    }
}
internal static class Animation
{
    private static string[] ROTATOR = new string[] { "|", "/", "-", "\\" };
    private static string[] DESTINATION = new string[] { ">--", "->-", "-->", "---" };
    private static int rotatorCount = 0; // Used for general LCD animation
    private static int runningCount = 0; // Used for PB surface animation

    public static void Run()
    {
        if (++rotatorCount >= ROTATOR.Length)
        {
            rotatorCount = 0;
        }
    }

    public static void RunPbRotator()
    {
        if (++runningCount >= ROTATOR.Length)
        {
            runningCount = 0;
        }
    }

    public static string Rotator()
    {
        return ROTATOR[rotatorCount];
    }

    public static string Destination()
    {
        return DESTINATION[rotatorCount];
    }

    public static string PbRunning()
    {
        return ROTATOR[runningCount];
    }
