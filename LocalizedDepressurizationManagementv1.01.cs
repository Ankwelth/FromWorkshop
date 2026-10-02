// ============================
// Localized Depressurization Management v1.10
// Optimized for Tick-Spreading (batch scans)
// ============================
private const string CompartmentPrefix = "Compartment"; // Prefix for compartment groups
private const string PrimaryGroupName = "Primary";      // Group for original light colors
private const string InfoLcdPanelName = "InfoLCD";      // Info panel
private const string StatusLcdPanelName = "StatusLCD";  // Status panel

private readonly Color LowPressureColor = new Color(255, 0, 0);

class CompartmentData
{
    public string Name;
    public List<IMyAirVent> AirVents = new List<IMyAirVent>();
    public List<IMyInteriorLight> Lights = new List<IMyInteriorLight>();
    public List<IMyDoor> Doors = new List<IMyDoor>();
    public bool LastPressureLow = false;
}

private List<CompartmentData> _compartments = new List<CompartmentData>();
private Color[] _originalLightColors;

private int _currentCompartmentIndex = 0;
private const int ScanBatchSize = 2;   // How many compartments to check per tick
private int _lcdCounter = 0;
private const int LcdUpdateInterval = 10; // Update LCD every 10 ticks

// Auto-scroll vars
private int _scrollPosition = 0;
private const int CompartmentDisplayLimit = 5;
private int _scrollCounter = 0;
private const int ScrollInterval = 30;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    InitializePrimaryColors();
    CacheCompartments();
}

public void Main(string arg, UpdateType src)
{
    // Process a small batch each tick
    ProcessNextCompartmentBatch();

    // LCD update every few ticks
    _lcdCounter++;
    if (_lcdCounter >= LcdUpdateInterval)
    {
        UpdateStatusLcd(StatusLcdPanelName);
        _lcdCounter = 0;
    }

    // Auto-scroll for StatusLCD
    _scrollCounter++;
    if (_scrollCounter >= ScrollInterval)
    {
        _scrollPosition += CompartmentDisplayLimit;
        if (_scrollPosition >= _compartments.Count)
            _scrollPosition = 0;
        _scrollCounter = 0;
    }
}

// =============================================
// Core Routines
// =============================================

// Process only a few compartments each tick
private void ProcessNextCompartmentBatch()
{
    if (_compartments.Count == 0) return;

    int end = Math.Min(_currentCompartmentIndex + ScanBatchSize, _compartments.Count);
    for (int i = _currentCompartmentIndex; i < end; i++)
    {
        var comp = _compartments[i];
        bool belowPressure = CheckPressure(comp);

        if (belowPressure != comp.LastPressureLow)
        {
            UpdateLightColors(comp, belowPressure);
            if (belowPressure) CloseDoors(comp);
            comp.LastPressureLow = belowPressure;
        }
    }

    _currentCompartmentIndex = end % _compartments.Count;
}

// Build compartment list at startup
private void CacheCompartments()
{
    _compartments.Clear();
    var allGroups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(allGroups);

    foreach (var group in allGroups)
    {
        if (!group.Name.StartsWith(CompartmentPrefix)) continue;

        var comp = new CompartmentData { Name = group.Name };
        group.GetBlocksOfType(comp.AirVents);
        group.GetBlocksOfType(comp.Lights);
        group.GetBlocksOfType(comp.Doors);
        _compartments.Add(comp);
    }
}

// Check pressure status of a compartment
private bool CheckPressure(CompartmentData comp)
{
    foreach (var vent in comp.AirVents)
    {
        if (vent.GetOxygenLevel() < 0.70f) return true;
    }
    return false;
}

// Close doors in a compartment
private void CloseDoors(CompartmentData comp)
{
    foreach (var door in comp.Doors)
    {
        if (door.Status == DoorStatus.Open) door.CloseDoor();
    }
}

// Save original colors from Primary group
private void InitializePrimaryColors()
{
    var primaryLights = new List<IMyInteriorLight>();
    var primaryGroup = GridTerminalSystem.GetBlockGroupWithName(PrimaryGroupName);
    if (primaryGroup != null)
    {
        primaryGroup.GetBlocksOfType(primaryLights);
        _originalLightColors = new Color[primaryLights.Count];
        for (int i = 0; i < primaryLights.Count; i++)
            _originalLightColors[i] = primaryLights[i].Color;
    }
}

// Update light colors for a compartment
private void UpdateLightColors(CompartmentData comp, bool belowPressure)
{
    for (int i = 0; i < comp.Lights.Count; i++)
    {
        var light = comp.Lights[i];
        var intendedColor = belowPressure
            ? LowPressureColor
            : ((i < _originalLightColors.Length) ? _originalLightColors[i] : light.Color);

        if (light.Color != intendedColor)
            light.Color = intendedColor;
    }
}

// =============================================
// LCD Updates
// =============================================

private void UpdateStatusLcd(string lcdName)
{
    var sb = new StringBuilder();
    sb.AppendLine("=== System Status ===");
    sb.AppendLine($"Localized Depressurization");
    sb.AppendLine($"Compartments: {_compartments.Count}");

    int end = Math.Min(_scrollPosition + CompartmentDisplayLimit, _compartments.Count);
    for (int i = _scrollPosition; i < end; i++)
    {
        var comp = _compartments[i];
        string status = comp.LastPressureLow ? "Leak Detected" : "Safe";
        sb.AppendLine($"{comp.Name}: {status}");
    }

    var lcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextPanel;
    if (lcd != null)
    {
        lcd.ContentType = ContentType.TEXT_AND_IMAGE;
        lcd.WriteText(sb.ToString());
    }
}
