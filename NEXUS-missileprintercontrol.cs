// NEXUS Missile Printer v1.15
// Changelog:
// - Enlarges the LOADED heading in the right dashboard column.
// - Gives the heading a taller fit box so UiScale can affect it visibly.
// - Keeps v1.14 branding, migration, and dashboard layout.
// Space Engineers Programmable Block / MDK-SE script.
//
// Commands:
//   rescan          - rebuild block lists from CustomData
//   setup           - start learning missile material cost
//   setup N         - finish learning and divide spent materials by N missiles
//   restock         - unload surplus, then pull components into target cargos
//   weld_start      - force projector + welders on
//   weld_stop       - force welders off
//   auto_on/off     - enable or disable automatic welding
//   clear_cost      - remove learned missile cost
//
// First run writes a configuration template into this programmable block CustomData.

const string VERSION = "1.15";
const float DEFAULT_VISUAL_SCALE = 2f;
const string CONFIG_VERSION = "NEXUSMissilePrinterConfigV1";
const string LEGACY_CONFIG_VERSION = "MissilePrinterControllerConfigV1";
const double IDLE_REFRESH_SECONDS = 30;
const double DISPLAY_REFRESH_SECONDS = 10;
const double FUNCTIONAL_CHECK_SECONDS = 1;

MyIni _ini = new MyIni();
MyIni _store = new MyIni();
StringBuilder _echo = new StringBuilder();

List<IMyProjector> _projectors = new List<IMyProjector>();
List<IMyShipWelder> _welders = new List<IMyShipWelder>();
List<IMyCargoContainer> _sourceCargos = new List<IMyCargoContainer>();
List<IMyCargoContainer> _targetCargos = new List<IMyCargoContainer>();
List<IMyTerminalBlock> _launchPoints = new List<IMyTerminalBlock>();
List<IMyTextSurface> _surfaces = new List<IMyTextSurface>();
IMyLightingBlock _statusLight;

Dictionary<string, double> _missileCost = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
Dictionary<string, double> _learnStart = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
Dictionary<string, int> _functionalTargets = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

string _projectorQuery, _sourceCargoQuery, _targetCargoQuery, _launchPointQuery, _cockpitName, _lcdQuery, _lightName;
int _cockpitSurfaceIndex, _desiredMissiles, _partialDelaySeconds, _finishWeldSeconds;
bool _autoWeld, _autoRestock, _showOnCockpit, _showOnLcd, _setupActive, _forceWelding;
double _partialTimer, _finishWeldTimer;
int _lastEmptyLaunchPoints, _readyFlashTicks;
bool _lastReady, _hadError, _materialSafetyLockout;
string _errorText = "";
string _safetyText = "";
string _statusText = "";
int _tick;
double _rescanTimer, _inventoryTimer, _restockTimer, _displayTimer, _functionalCheckTimer;
int _cachedMissileCount;
bool _inventoryDirty = true;
bool _functionalReadyCache = true;
string _lastDisplayKey = "";
Color _uiAccent = new Color(160, 80, 0), _uiBackground = new Color(0, 0, 0);
float _uiScale = 1f;
bool _wasWelderActive;
double _currentWelderRunSeconds, _lastWelderRunSeconds;
int _lastLightMode = -1;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    EnsureConfig();
    LoadStorage();
    LoadConfig();
    Rescan();
}

public void Save() { SaveStorage(); }

public void Main(string argument, UpdateType updateSource)
{
    _tick++;
    _echo.Clear();
    double dt = Runtime.TimeSinceLastRun.TotalSeconds;

    if ((updateSource & (UpdateType.Terminal | UpdateType.Trigger | UpdateType.Script)) != 0)
        HandleCommand((argument ?? "").Trim());

    _rescanTimer += dt;
    _inventoryTimer += dt;
    _restockTimer += dt;
    _displayTimer += dt;
    _functionalCheckTimer += dt;

    bool idleForHeavyWork = !_setupActive && !_forceWelding;

    if (idleForHeavyWork && _rescanTimer >= IDLE_REFRESH_SECONDS)
    {
        Rescan();
        _rescanTimer = 0;
    }

    ValidateState();

    if (idleForHeavyWork && (_inventoryDirty || _inventoryTimer >= IDLE_REFRESH_SECONDS))
        RefreshMissileCount();

    if (!_hadError && _autoRestock && idleForHeavyWork && _restockTimer >= IDLE_REFRESH_SECONDS)
    {
        Restock(false);
        RefreshMissileCount();
        _restockTimer = 0;
    }

    UpdateMaterialSafety(_cachedMissileCount);

    if (_materialSafetyLockout)
    {
        _forceWelding = false;
        _partialTimer = 0;
        _finishWeldTimer = 0;
        SetWelders(false);
    }

    if (!_hadError && !_materialSafetyLockout)
    {
        UpdateLaunchAutomation();
        UpdateWelding();
    }

    bool ready = AreProjectorsReady();

    if (ready && !_lastReady)
        _readyFlashTicks = 8;
    _lastReady = ready;

    bool welderActive = AnyWelderEnabled();
    UpdateWelderTelemetry(dt, welderActive);
    UpdateDisplays(_cachedMissileCount, ready, welderActive);
    UpdateLight(welderActive);
    UpdateRuntimeFrequency(welderActive);

    Echo(_echo.ToString());
}

void EnsureConfig()
{
    MyIniParseResult result;
    bool parsed = _ini.TryParse(Me.CustomData, out result);
    string configVersion = parsed ? _ini.Get("Config", "Version").ToString() : "";
    if (parsed && configVersion == LEGACY_CONFIG_VERSION)
    {
        _ini.Set("Config", "Version", CONFIG_VERSION);
        Me.CustomData = _ini.ToString();
        return;
    }
    if (!parsed || configVersion != CONFIG_VERSION)
    {
        Me.CustomData =
@"[Config]
Version=NEXUSMissilePrinterConfigV1

[Blocks]
; Query can be one fragment/tag, for example [MP], or exact names separated by commas.
ProjectorQuery=[MP_PROJECTOR]
SourceCargoQuery=[BASE_OUT]
TargetCargoQuery=[MPC]
LaunchPointQuery=[M]
CockpitName=
CockpitSurfaceIndex=0
LcdQuery=
StatusLightName=

[Printer]
DesiredMissilesInTargetCargo=10
PartialLaunchDelaySeconds=60
FinishWeldSeconds=10
AutoWeld=true
AutoRestock=true

[Display]
ShowOnCockpit=true
ShowOnLcd=true
AccentColor=160,80,0
BackgroundColor=0,0,0
UiScale=1.0

[LearnedCost]
; Filled by setup / setup N.
";
        _ini.TryParse(Me.CustomData, out result);
    }
}

void LoadConfig()
{
    MyIniParseResult result;
    if (!_ini.TryParse(Me.CustomData, out result))
    {
        _hadError = true;
        _errorText = "CustomData parse error: " + result.ToString();
        return;
    }

    if (!_ini.ContainsKey("Printer", "FinishWeldSeconds"))
    {
        _ini.Set("Printer", "FinishWeldSeconds", 10);
        Me.CustomData = _ini.ToString();
    }
    if (!_ini.ContainsKey("Display", "AccentColor")) _ini.Set("Display", "AccentColor", "160,80,0");
    if (!_ini.ContainsKey("Display", "BackgroundColor")) _ini.Set("Display", "BackgroundColor", "0,0,0");
    if (!_ini.ContainsKey("Display", "UiScale")) _ini.Set("Display", "UiScale", 1.0);
    Me.CustomData = _ini.ToString();

    _projectorQuery = _ini.Get("Blocks", "ProjectorQuery").ToString("[MP_PROJECTOR]");
    _sourceCargoQuery = _ini.Get("Blocks", "SourceCargoQuery").ToString("[BASE_OUT]");
    _targetCargoQuery = _ini.Get("Blocks", "TargetCargoQuery").ToString("[MPC]");
    _launchPointQuery = _ini.Get("Blocks", "LaunchPointQuery").ToString("[M]");
    _cockpitName = _ini.Get("Blocks", "CockpitName").ToString("");
    _cockpitSurfaceIndex = _ini.Get("Blocks", "CockpitSurfaceIndex").ToInt32(0);
    _lcdQuery = _ini.Get("Blocks", "LcdQuery").ToString("");
    _lightName = _ini.Get("Blocks", "StatusLightName").ToString("");

    _desiredMissiles = Math.Max(0, _ini.Get("Printer", "DesiredMissilesInTargetCargo").ToInt32(10));
    _partialDelaySeconds = Math.Max(0, _ini.Get("Printer", "PartialLaunchDelaySeconds").ToInt32(60));
    _finishWeldSeconds = Math.Max(0, _ini.Get("Printer", "FinishWeldSeconds").ToInt32(10));
    _autoWeld = _ini.Get("Printer", "AutoWeld").ToBoolean(true);
    _autoRestock = _ini.Get("Printer", "AutoRestock").ToBoolean(true);
    _showOnCockpit = _ini.Get("Display", "ShowOnCockpit").ToBoolean(true);
    _showOnLcd = _ini.Get("Display", "ShowOnLcd").ToBoolean(true);
    _uiAccent = ParseColor(_ini.Get("Display", "AccentColor").ToString("160,80,0"), new Color(160, 80, 0));
    _uiBackground = ParseColor(_ini.Get("Display", "BackgroundColor").ToString("0,0,0"), new Color(0, 0, 0));
    _uiScale = (float)Math.Max(0.5, Math.Min(3.0, _ini.Get("Display", "UiScale").ToDouble(1.0)));

    _missileCost.Clear();
    var keys = new List<MyIniKey>();
    _ini.GetKeys("LearnedCost", keys);
    for (int i = 0; i < keys.Count; i++)
    {
        double value = _ini.Get(keys[i]).ToDouble(0);
        if (value > 0)
            _missileCost[keys[i].Name] = value;
    }
}

void LoadStorage()
{
    MyIniParseResult result;
    if (!_store.TryParse(Storage ?? "", out result))
        _store.Clear();

    _setupActive = _store.Get("Learning", "Active").ToBoolean(false);
    _learnStart.Clear();
    var keys = new List<MyIniKey>();
    _store.GetKeys("LearnStart", keys);
    for (int i = 0; i < keys.Count; i++)
    {
        double value = _store.Get(keys[i]).ToDouble(0);
        if (value > 0)
            _learnStart[keys[i].Name] = value;
    }
}

void SaveStorage()
{
    _store.Clear();
    _store.Set("Learning", "Active", _setupActive);
    foreach (var pair in _learnStart)
        _store.Set("LearnStart", pair.Key, pair.Value);
    Storage = _store.ToString();
}

void SaveLearnedCostToCustomData()
{
    var oldKeys = new List<MyIniKey>();
    _ini.GetKeys("LearnedCost", oldKeys);
    foreach (var key in oldKeys)
        _ini.Delete(key.Section, key.Name);

    foreach (var pair in _missileCost)
        _ini.Set("LearnedCost", pair.Key, Math.Round(pair.Value, 3));

    Me.CustomData = _ini.ToString();
}

void HandleCommand(string argument)
{
    if (argument.Length == 0)
        return;

    string[] parts = argument.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    string cmd = parts[0].ToLowerInvariant();

    if (cmd == "rescan")
    {
        LoadConfig();
        Rescan();
        Add("Rescan complete.");
    }
    else if (cmd == "setup")
    {
        if (!_setupActive)
            StartSetup();
        else
        {
            int count = 1;
            if (parts.Length > 1)
                int.TryParse(parts[1], out count);
            FinishSetup(Math.Max(1, count));
        }
    }
    else if (cmd == "restock")
    {
        Restock(true);
        RefreshMissileCount();
    }
    else if (cmd == "weld_start")
    {
        RefreshMissileCount();
        UpdateMaterialSafety(_cachedMissileCount);
        if (_materialSafetyLockout)
        {
            SetWelders(false);
            Add("Weld start blocked: " + _safetyText);
            return;
        }

        _forceWelding = true;
        _finishWeldTimer = 0;
        SetProjectors(true);
        CaptureFunctionalWeldPlan();
        SetWelders(true);
        Add("Forced welding started.");
    }
    else if (cmd == "weld_stop")
    {
        _forceWelding = false;
        _finishWeldTimer = 0;
        SetWelders(false);
        Add("Welding stopped.");
    }
    else if (cmd == "auto_on")
    {
        _autoWeld = true;
        _ini.Set("Printer", "AutoWeld", true);
        Me.CustomData = _ini.ToString();
        Add("AutoWeld enabled.");
    }
    else if (cmd == "auto_off")
    {
        _autoWeld = false;
        _forceWelding = false;
        _finishWeldTimer = 0;
        SetWelders(false);
        _ini.Set("Printer", "AutoWeld", false);
        Me.CustomData = _ini.ToString();
        Add("AutoWeld disabled.");
    }
    else if (cmd == "clear_cost")
    {
        _missileCost.Clear();
        SaveLearnedCostToCustomData();
        Add("Learned cost cleared.");
    }
}

void Rescan()
{
    LoadConfig();
    _projectors.Clear();
    _welders.Clear();
    _sourceCargos.Clear();
    _targetCargos.Clear();
    _launchPoints.Clear();
    _surfaces.Clear();
    _statusLight = null;

    GridTerminalSystem.GetBlocksOfType(_projectors, b => MatchQuery(b.CustomName, _projectorQuery));
    GridTerminalSystem.GetBlocksOfType(_sourceCargos, b => MatchQuery(b.CustomName, _sourceCargoQuery));
    GridTerminalSystem.GetBlocksOfType(_targetCargos, b => MatchQuery(b.CustomName, _targetCargoQuery));
    GridTerminalSystem.GetBlocksOfType(_welders, b => b.IsSameConstructAs(Me));

    var connectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(connectors, b => MatchQuery(b.CustomName, _launchPointQuery));
    _launchPoints.AddRange(connectors);

    var merges = new List<IMyShipMergeBlock>();
    GridTerminalSystem.GetBlocksOfType(merges, b => MatchQuery(b.CustomName, _launchPointQuery));
    _launchPoints.AddRange(merges);

    if (_showOnCockpit && _cockpitName.Length > 0)
    {
        var cockpit = GridTerminalSystem.GetBlockWithName(_cockpitName) as IMyTextSurfaceProvider;
        if (cockpit != null && _cockpitSurfaceIndex >= 0 && _cockpitSurfaceIndex < cockpit.SurfaceCount)
            _surfaces.Add(cockpit.GetSurface(_cockpitSurfaceIndex));
    }

    if (_showOnLcd && _lcdQuery.Length > 0)
    {
        var panels = new List<IMyTextPanel>();
        GridTerminalSystem.GetBlocksOfType(panels, b => MatchQuery(b.CustomName, _lcdQuery));
        for (int i = 0; i < panels.Count; i++)
            _surfaces.Add(panels[i]);
    }

    if (_lightName.Length > 0)
        _statusLight = GridTerminalSystem.GetBlockWithName(_lightName) as IMyLightingBlock;

    _lastEmptyLaunchPoints = CountEmptyLaunchPoints();
    _inventoryDirty = true;
    _lastDisplayKey = "";
    _lastLightMode = -1;
    _rescanTimer = 0;
}

bool MatchQuery(string name, string query)
{
    if (string.IsNullOrWhiteSpace(query))
        return false;

    string[] exactNames = query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    if (exactNames.Length > 1)
    {
        for (int i = 0; i < exactNames.Length; i++)
        {
            if (string.Equals(name.Trim(), exactNames[i].Trim(), StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    return name.IndexOf(query.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
}

void ValidateState()
{
    _hadError = false;
    _errorText = "";
    _materialSafetyLockout = false;
    _safetyText = "";

    if (_projectors.Count == 0)
        Error("No projectors found by ProjectorQuery.");
    else if (_targetCargos.Count == 0)
        Error("No target cargos found by TargetCargoQuery.");
    else if (_surfaces.Count == 0)
        Add("Warning: no display surfaces found.");
    else if (_missileCost.Count == 0)
        Add("Warning: learned missile cost is empty. Use setup, weld missiles, then setup N.");

    Add("NEXUS Missile Printer v" + VERSION);
    Add("Projectors: " + _projectors.Count + "  Welders: " + _welders.Count);
    Add("Target cargos: " + _targetCargos.Count + "  Source cargos: " + _sourceCargos.Count);
    Add("Launch points: " + _launchPoints.Count);
    Add("Setup active: " + _setupActive);
    Add("AutoWeld: " + _autoWeld + "  AutoRestock: " + _autoRestock);
    Add("Projector remaining blocks: " + TotalRemainingBlocks());
    if (_errorText.Length > 0)
        Add("ERROR: " + _errorText);
}

void UpdateMaterialSafety(int missileCount)
{
    _materialSafetyLockout = false;
    _safetyText = "";

    if (_hadError || _setupActive || _missileCost.Count == 0 || _launchPoints.Count == 0)
        return;

    int required = Math.Max(1, _launchPoints.Count);
    if (missileCount <= 0)
    {
        _materialSafetyLockout = true;
        _safetyText = "no complete missiles worth of components in target cargo";
    }
    else if (missileCount < required)
    {
        _materialSafetyLockout = true;
        _safetyText = "target cargo has " + missileCount + " missiles, launch points require " + required;
    }

    if (_materialSafetyLockout)
        Add("SAFETY: " + _safetyText + ".");
}

void Error(string text)
{
    _hadError = true;
    _errorText = text;
}

void StartSetup()
{
    _learnStart = SumComponents(_targetCargos);
    _setupActive = true;
    SaveStorage();
    Add("Setup started. Auto restock is ignored while learning.");
}

void FinishSetup(int missileCount)
{
    var end = SumComponents(_targetCargos);
    var learned = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

    foreach (var pair in _learnStart)
    {
        double spent = pair.Value - GetAmount(end, pair.Key);
        if (spent > 0.001)
            learned[pair.Key] = spent / missileCount;
    }

    _missileCost = learned;
    _setupActive = false;
    _learnStart.Clear();
    SaveStorage();
    SaveLearnedCostToCustomData();
    _inventoryDirty = true;
    Add("Setup finished. Learned components for 1 missile: " + _missileCost.Count);
}

void Restock(bool verbose)
{
    if (_forceWelding)
    {
        if (verbose)
            Add("Restock skipped: welding is active.");
        return;
    }

    if (!_setupActive && (_sourceCargos.Count == 0 || !InventoriesUsable(_sourceCargos)))
        Rescan();

    if (_missileCost.Count == 0 || _targetCargos.Count == 0 || _sourceCargos.Count == 0)
    {
        if (verbose)
            Add("Restock skipped: learned cost, target cargo, or source cargo is missing.");
        return;
    }

    int unloadedTypes = UnloadSurplusComponents();
    var target = SumComponents(_targetCargos);
    int movedTypes = 0;

    foreach (var pair in _missileCost)
    {
        double desired = pair.Value * _desiredMissiles;
        double need = desired - GetAmount(target, pair.Key);
        if (need <= 0.001)
            continue;

        double moved = MoveComponent(pair.Key, need);
        if (moved > 0)
            movedTypes++;
    }

    if (verbose)
        Add("Restock complete. Loaded types: " + movedTypes + "  Unloaded types: " + unloadedTypes);

    _inventoryDirty = true;
}

void RefreshMissileCount() { _cachedMissileCount = CountAvailableMissilesInTarget(); _inventoryDirty = false; _inventoryTimer = 0; }

void UpdateRuntimeFrequency(bool welderActive)
{
    bool launchWatchActive = _partialTimer > 0;
    bool active = _forceWelding || welderActive || launchWatchActive || _readyFlashTicks > 0;
    Runtime.UpdateFrequency = active ? UpdateFrequency.Update10 : UpdateFrequency.Update100;
}

bool InventoriesUsable(List<IMyCargoContainer> cargos)
{
    for (int i = cargos.Count - 1; i >= 0; i--) if (cargos[i] == null || !cargos[i].HasInventory) return false;
    return true;
}

int UnloadSurplusComponents()
{
    var target = SumComponents(_targetCargos);
    int movedTypes = 0;

    foreach (var pair in target)
    {
        double allowed = 0;
        double cost;
        if (_missileCost.TryGetValue(pair.Key, out cost))
            allowed = cost * _desiredMissiles;

        double surplus = pair.Value - allowed;
        if (surplus <= 0.001)
            continue;

        double moved = MoveComponentBetween(_targetCargos, _sourceCargos, pair.Key, surplus);
        if (moved > 0)
            movedTypes++;
    }

    return movedTypes;
}

double MoveComponent(string subtype, double amount)
{
    return MoveComponentBetween(_sourceCargos, _targetCargos, subtype, amount);
}

double MoveComponentBetween(List<IMyCargoContainer> fromCargos, List<IMyCargoContainer> toCargos, string subtype, double amount)
{
    double remaining = amount;
    var items = new List<MyInventoryItem>();

    for (int s = 0; s < fromCargos.Count && remaining > 0.001; s++)
    {
        bool movedFromThisCargo = true;
        while (remaining > 0.001 && movedFromThisCargo)
        {
            movedFromThisCargo = false;
            var srcInv = fromCargos[s].GetInventory();
            items.Clear();
            srcInv.GetItems(items);

            for (int i = items.Count - 1; i >= 0 && remaining > 0.001; i--)
            {
                var item = items[i];
                if (item.Type.TypeId != "MyObjectBuilder_Component")
                    continue;
                if (!string.Equals(item.Type.SubtypeId, subtype, StringComparison.OrdinalIgnoreCase))
                    continue;

                double available = AmountToDouble(item.Amount);
                double take = Math.Min(available, remaining);
                MyFixedPoint fpTake = (MyFixedPoint)take;

                for (int t = 0; t < toCargos.Count && take > 0.001; t++)
                {
                    var dstInv = toCargos[t].GetInventory();
                    if (srcInv.TransferItemTo(dstInv, i, null, true, fpTake))
                    {
                        remaining -= take;
                        movedFromThisCargo = true;
                        break;
                    }
                }

                if (movedFromThisCargo)
                    break;
            }
        }
    }

    return amount - remaining;
}

Dictionary<string, double> SumComponents(List<IMyCargoContainer> cargos)
{
    var sum = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
    var items = new List<MyInventoryItem>();

    for (int c = 0; c < cargos.Count; c++)
    {
        var inv = cargos[c].GetInventory();
        items.Clear();
        inv.GetItems(items);

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item.Type.TypeId != "MyObjectBuilder_Component")
                continue;

            string subtype = item.Type.SubtypeId;
            if (!sum.ContainsKey(subtype))
                sum[subtype] = 0;
            sum[subtype] += AmountToDouble(item.Amount);
        }
    }

    return sum;
}

double AmountToDouble(MyFixedPoint value) { return (double)((decimal)value); }

double GetAmount(Dictionary<string, double> dict, string key) { double value; return dict.TryGetValue(key, out value) ? value : 0; }

int CountAvailableMissilesInTarget()
{
    if (_missileCost.Count == 0 || _targetCargos.Count == 0)
        return 0;

    var target = SumComponents(_targetCargos);
    int possible = int.MaxValue;

    foreach (var pair in _missileCost)
    {
        if (pair.Value <= 0)
            continue;

        int count = (int)Math.Floor(GetAmount(target, pair.Key) / pair.Value);
        if (count < possible)
            possible = count;
    }

    return possible == int.MaxValue ? 0 : possible;
}

void UpdateLaunchAutomation()
{
    if (!_autoWeld || _launchPoints.Count == 0 || _forceWelding)
        return;

    int empty = CountEmptyLaunchPoints();
    int total = _launchPoints.Count;

    if (empty > _lastEmptyLaunchPoints)
        _partialTimer = 0;

    _lastEmptyLaunchPoints = empty;

    if (empty == 0)
    {
        _partialTimer = 0;
        return;
    }

    if (empty >= total)
    {
        StartAutoWelding("all launch points are empty");
        return;
    }

    if (_partialDelaySeconds == 0)
    {
        StartAutoWelding("partial launch detected");
        return;
    }

    _partialTimer += Runtime.TimeSinceLastRun.TotalSeconds;
    if (_partialTimer >= _partialDelaySeconds)
        StartAutoWelding("partial launch delay elapsed");
}

void StartAutoWelding(string reason)
{
    RefreshMissileCount();
    UpdateMaterialSafety(_cachedMissileCount);
    if (_materialSafetyLockout)
    {
        SetWelders(false);
        Add("Auto welding blocked: " + _safetyText + ".");
        return;
    }

    _forceWelding = true;
    _partialTimer = 0;
    _finishWeldTimer = 0;
    SetProjectors(true);
    CaptureFunctionalWeldPlan();
    SetWelders(true);
    Add("Auto welding started: " + reason + ".");
}

void UpdateWelding()
{
    if (!_forceWelding)
        return;

    SetProjectors(true);

    if (AreProjectorsReady())
    {
        if (_functionalCheckTimer >= FUNCTIONAL_CHECK_SECONDS)
        {
            _functionalReadyCache = AreFunctionalTargetsReady();
            _functionalCheckTimer = 0;
        }

        if (!_functionalReadyCache)
        {
            _finishWeldTimer = 0;
            SetWelders(true);
            return;
        }

        _finishWeldTimer += Runtime.TimeSinceLastRun.TotalSeconds;
        SetWelders(true);

        if (_finishWeldTimer >= _finishWeldSeconds)
        {
            SetWelders(false);
            _forceWelding = false;
            _finishWeldTimer = 0;
            _readyFlashTicks = 8;
            _inventoryDirty = true;
            Add("Welding complete.");
        }
    }
    else
    {
        _finishWeldTimer = 0;
        SetWelders(true);
    }
}

void CaptureFunctionalWeldPlan()
{
    _functionalTargets.Clear();
    _functionalReadyCache = false;
    _functionalCheckTimer = FUNCTIONAL_CHECK_SECONDS;
    var remainingByDefinition = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    for (int i = 0; i < _projectors.Count; i++)
    {
        foreach (var pair in _projectors[i].RemainingBlocksPerType)
        {
            string key = DefinitionKey(pair.Key);
            if (!ShouldCheckFunctionalBlock(key))
                continue;

            int count;
            remainingByDefinition.TryGetValue(key, out count);
            remainingByDefinition[key] = count + pair.Value;
        }
    }

    var functionalCounts = CountFunctionalBlocksByDefinition(remainingByDefinition);
    foreach (var pair in remainingByDefinition)
        _functionalTargets[pair.Key] = GetCount(functionalCounts, pair.Key) + pair.Value;

    Add("Functional check targets: " + _functionalTargets.Count);
}

bool AreFunctionalTargetsReady()
{
    if (_functionalTargets.Count == 0)
        return true;

    int missingTypes = 0;
    var functionalCounts = CountFunctionalBlocksByDefinition(_functionalTargets);
    foreach (var pair in _functionalTargets)
    {
        int functional = GetCount(functionalCounts, pair.Key);
        if (functional < pair.Value)
        {
            missingTypes++;
            Add("Waiting functional: " + ShortDefinitionName(pair.Key) + " " + functional + "/" + pair.Value);
        }
    }

    return missingTypes == 0;
}

Dictionary<string, int> CountFunctionalBlocksByDefinition(Dictionary<string, int> definitions)
{
    var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => b.IsFunctional);
    for (int i = 0; i < blocks.Count; i++)
    {
        string key = DefinitionKey(blocks[i].BlockDefinition);
        if (!definitions.ContainsKey(key)) continue;
        counts[key] = GetCount(counts, key) + 1;
    }
    return counts;
}

int GetCount(Dictionary<string, int> dict, string key) { int value; return dict.TryGetValue(key, out value) ? value : 0; }

string DefinitionKey(object definition) { return definition == null ? "" : definition.ToString(); }

bool ShouldCheckFunctionalBlock(string definitionKey)
{
    string key = definitionKey.ToLowerInvariant();

    return key.IndexOf("thrust") >= 0
        || key.IndexOf("gyro") >= 0
        || key.IndexOf("warhead") >= 0
        || key.IndexOf("battery") >= 0
        || key.IndexOf("reactor") >= 0
        || key.IndexOf("remote") >= 0
        || key.IndexOf("cockpit") >= 0
        || key.IndexOf("control") >= 0
        || key.IndexOf("merge") >= 0
        || key.IndexOf("connector") >= 0
        || key.IndexOf("antenna") >= 0
        || key.IndexOf("camera") >= 0
        || key.IndexOf("timer") >= 0
        || key.IndexOf("programmable") >= 0
        || key.IndexOf("tank") >= 0
        || key.IndexOf("jumpdrive") >= 0;
}

string ShortDefinitionName(string definitionKey)
{
    int slash = definitionKey.LastIndexOf('/');
    if (slash >= 0 && slash < definitionKey.Length - 1)
        return definitionKey.Substring(slash + 1);

    int space = definitionKey.LastIndexOf(' ');
    if (space >= 0 && space < definitionKey.Length - 1)
        return definitionKey.Substring(space + 1);

    return definitionKey;
}

bool AreProjectorsReady()
{
    if (_projectors.Count == 0) return false;
    for (int i = 0; i < _projectors.Count; i++) if (_projectors[i].RemainingBlocks > 0) return false;
    return true;
}

int TotalRemainingBlocks()
{
    int total = 0;
    for (int i = 0; i < _projectors.Count; i++) total += _projectors[i].RemainingBlocks;
    return total;
}

int CountEmptyLaunchPoints()
{
    int empty = 0;

    for (int i = 0; i < _launchPoints.Count; i++)
    {
        var connector = _launchPoints[i] as IMyShipConnector;
        if (connector != null)
        {
            if (connector.Status == MyShipConnectorStatus.Unconnected)
                empty++;
            continue;
        }

        var merge = _launchPoints[i] as IMyShipMergeBlock;
        if (merge != null && !merge.IsConnected)
            empty++;
    }

    return empty;
}

void SetProjectors(bool enabled) { for (int i = 0; i < _projectors.Count; i++) if (_projectors[i].Enabled != enabled) _projectors[i].Enabled = enabled; }

void SetWelders(bool enabled) { for (int i = 0; i < _welders.Count; i++) if (_welders[i].Enabled != enabled) _welders[i].Enabled = enabled; }

bool AnyWelderEnabled()
{
    for (int i = 0; i < _welders.Count; i++) if (_welders[i].Enabled) return true;
    return false;
}

void UpdateDisplays(int missileCount, bool ready, bool welderActive)
{
    int loaded = CountConnectedLaunchPoints();
    string status = _hadError ? "ERROR" : _materialSafetyLockout ? "LOW MATERIAL" : (_forceWelding || welderActive) ? "WELDING" : ready ? "READY" : "NOT READY";
    string forceTimer = _partialTimer > 0 ? FormatDuration(Math.Max(0, _partialDelaySeconds - _partialTimer)) : "--:--";
    string currentRun = welderActive ? FormatDuration(_currentWelderRunSeconds) : "--:--";
    string displayKey = loaded + "|" + missileCount + "|" + status + "|" + forceTimer + "|" + currentRun + "|" + FormatDuration(_lastWelderRunSeconds);
    _statusText = status;
    if (displayKey == _lastDisplayKey && _displayTimer < DISPLAY_REFRESH_SECONDS) return;
    _lastDisplayKey = displayKey;
    _displayTimer = 0;
    for (int i = 0; i < _surfaces.Count; i++) DrawSurface(_surfaces[i], loaded, missileCount, status, forceTimer, currentRun, welderActive);
}

void DrawSurface(IMyTextSurface surface, int loaded, int available, string status, string forceTimer, string currentRun, bool welderActive)
{
    surface.ContentType = ContentType.SCRIPT;
    surface.Script = "";
    surface.BackgroundColor = _uiAccent;
    surface.ScriptBackgroundColor = _uiAccent;
    RectangleF viewport = new RectangleF((surface.TextureSize - surface.SurfaceSize) * 0.5f, surface.SurfaceSize);
    float baseScale = Math.Max(0.25f, Math.Min(viewport.Width, viewport.Height) / 512f);
    float scale = baseScale * _uiScale * DEFAULT_VISUAL_SCALE, frameScale = baseScale;
    float outer = Math.Max(6f, 14f * frameScale), inset = Math.Max(4f, 10f * frameScale), radius = Math.Max(6f, 16f * frameScale);
    RectangleF panel = new RectangleF(viewport.X + outer, viewport.Y + outer, viewport.Width - outer * 2f, viewport.Height - outer * 2f);
    RectangleF content = new RectangleF(panel.X + inset, panel.Y + inset, panel.Width - inset * 2f, panel.Height - inset * 2f);
    float gap = Math.Max(6f, 10f * frameScale), rightW = Math.Max(105f * frameScale, content.Width * 0.41f), leftW = content.Width - rightW - gap;
    RectangleF left = new RectangleF(content.X, content.Y, leftW, content.Height);
    RectangleF right = new RectangleF(left.X + left.Width + gap, content.Y, rightW, content.Height);
    string font = "Debug";

    using (var frame = surface.DrawFrame())
    {
        DrawRect(frame, viewport.Center, new Vector2(viewport.Width, viewport.Height), _uiAccent);
        DrawRoundedRect(frame, panel, radius, _uiBackground);
        float titleScale = FitTextScale(surface, "NEXUS // MISSILE PRINTER", font, left.Width, 32f * frameScale, Math.Max(0.42f, 1.05f * scale));
        DrawText(frame, "NEXUS // MISSILE PRINTER", new Vector2(left.X, left.Y + 2f * frameScale), titleScale, _uiAccent);
        float rowTextScale = FitInfoRowScale(surface, left.Width, Math.Max(0.3f, 0.82f * scale));
        float rowStart = left.Y + Math.Max(27f, 47f * frameScale), rowBottom = left.Y + left.Height - Math.Max(10f, 16f * frameScale);
        float rowH = Math.Min(Math.Max(15f, 35f * frameScale), Math.Max(15f, (rowBottom - rowStart) / 4f));
        DrawInfoRow(frame, left, rowStart, "AVAILABLE", available.ToString(), rowTextScale); rowStart += rowH;
        DrawInfoRow(frame, left, rowStart, "FORCE TIMER", forceTimer, rowTextScale); rowStart += rowH;
        DrawInfoRow(frame, left, rowStart, "WELDERS", welderActive ? "ON" : "OFF", rowTextScale); rowStart += rowH;
        DrawInfoRow(frame, left, rowStart, "LAST RUN", FormatDuration(_lastWelderRunSeconds), rowTextScale); rowStart += rowH;
        DrawInfoRow(frame, left, rowStart, "WELD TIME", currentRun, rowTextScale);

        float loadedTitleScale = FitTextScale(surface, "LOADED", font, right.Width - 4f * frameScale, 48f * frameScale, Math.Max(0.62f, 1.55f * scale));
        DrawText(frame, "LOADED", new Vector2(right.Center.X, right.Y + Math.Max(4f, 10f * frameScale)), loadedTitleScale, _uiAccent, TextAlignment.CENTER);
        string loadedText = loaded.ToString();
        float statusH = Math.Min(right.Height * 0.25f, Math.Max(25f, 48f * frameScale)), statusY = right.Y + right.Height - statusH - Math.Max(4f, 8f * frameScale);
        RectangleF statusOuter = new RectangleF(right.X, statusY, right.Width, statusH);
        float digitTop = right.Y + Math.Max(22f, 39f * frameScale), digitH = Math.Max(35f, statusY - digitTop - 4f * frameScale);
        float digitScale = FitTextScale(surface, loadedText, font, right.Width - 6f * frameScale, digitH, 24f * scale);
        DrawText(frame, loadedText, new Vector2(right.Center.X, digitTop), digitScale, _uiAccent, TextAlignment.CENTER);

        DrawRoundedRect(frame, statusOuter, Math.Max(5f, 9f * frameScale), _uiAccent);
        float statusScale = FitTextScale(surface, status, font, statusOuter.Width - 12f * frameScale, statusOuter.Height - 10f * frameScale, 1.35f * scale);
        DrawText(frame, status, new Vector2(statusOuter.Center.X, statusOuter.Y + (statusOuter.Height - surface.MeasureStringInPixels(new StringBuilder(status), font, statusScale).Y) * 0.5f), statusScale, _uiBackground, TextAlignment.CENTER);
    }
}

void DrawInfoRow(MySpriteDrawFrame frame, RectangleF area, float y, string label, string value, float textScale)
{
    DrawText(frame, label, new Vector2(area.X, y), textScale, _uiAccent);
    DrawText(frame, value, new Vector2(area.X + area.Width, y), textScale, _uiAccent, TextAlignment.RIGHT);
}

float FitInfoRowScale(IMyTextSurface surface, float width, float maxScale)
{
    float labelScale = FitTextScale(surface, "FORCE TIMER", "Debug", width * 0.62f, 40f, maxScale);
    float valueScale = FitTextScale(surface, "00:00", "Debug", width * 0.34f, 40f, maxScale);
    return Math.Min(labelScale, valueScale);
}

void DrawText(MySpriteDrawFrame frame, string text, Vector2 position, float scale, Color color, TextAlignment alignment = TextAlignment.LEFT) { frame.Add(new MySprite(SpriteType.TEXT, text, position, null, color, "Debug", alignment, scale)); }
void DrawRect(MySpriteDrawFrame frame, Vector2 position, Vector2 size, Color color) { frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", position, size, color)); }

void DrawRoundedRect(MySpriteDrawFrame frame, RectangleF rect, float radius, Color color)
{
    float left = (float)Math.Round(rect.X), top = (float)Math.Round(rect.Y), right = (float)Math.Round(rect.X + rect.Width), bottom = (float)Math.Round(rect.Y + rect.Height);
    float width = right - left, height = bottom - top, r = Math.Max(1f, (float)Math.Round(Math.Min(radius, Math.Min(width, height) / 2f)));
    if (r < 7f) { float cut = Math.Min(2f, r); DrawRect(frame, new Vector2((left + right) / 2f, (top + bottom) / 2f), new Vector2(width - cut * 2f, height), color); DrawRect(frame, new Vector2((left + right) / 2f, (top + bottom) / 2f), new Vector2(width, height - cut * 2f), color); return; }
    float diameter = r * 2f; Vector2 size = new Vector2(diameter, diameter);
    frame.Add(MySprite.CreateClipRect(new Rectangle((int)left, (int)top, (int)width, (int)height)));
    frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(left + r, top + r), size, color));
    frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(right - r, top + r), size, color));
    frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(left + r, bottom - r), size, color));
    frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(right - r, bottom - r), size, color));
    DrawRect(frame, new Vector2((left + right) / 2f, (top + bottom) / 2f), new Vector2(width - diameter, height), color);
    DrawRect(frame, new Vector2((left + right) / 2f, (top + bottom) / 2f), new Vector2(width, height - diameter), color);
    frame.Add(MySprite.CreateClearClipRect());
}

float FitTextScale(IMyTextSurface surface, string text, string font, float maxWidth, float maxHeight, float maxScale)
{
    if (string.IsNullOrEmpty(text))
        return 1f;

    StringBuilder sb = new StringBuilder(text);
    float low = 0.1f;
    float high = maxScale;

    for (int i = 0; i < 14; i++)
    {
        float mid = (low + high) * 0.5f;
        Vector2 measured = surface.MeasureStringInPixels(sb, font, mid);
        if (measured.X <= maxWidth && measured.Y <= maxHeight)
            low = mid;
        else
            high = mid;
    }

    return low;
}

void UpdateWelderTelemetry(double dt, bool active)
{
    if (active) _currentWelderRunSeconds += dt;
    else if (_wasWelderActive) { _lastWelderRunSeconds = _currentWelderRunSeconds; _currentWelderRunSeconds = 0; }
    _wasWelderActive = active;
}

int CountConnectedLaunchPoints()
{
    int connected = 0;
    for (int i = 0; i < _launchPoints.Count; i++)
    {
        var connector = _launchPoints[i] as IMyShipConnector; if (connector != null) { if (connector.Status == MyShipConnectorStatus.Connected) connected++; continue; }
        var merge = _launchPoints[i] as IMyShipMergeBlock; if (merge != null && merge.IsConnected) connected++;
    }
    return connected;
}

string FormatDuration(double seconds)
{
    int total = Math.Max(0, (int)Math.Round(seconds)), minutes = total / 60;
    return minutes.ToString("00") + ":" + (total % 60).ToString("00");
}

Color ParseColor(string text, Color fallback)
{
    string[] p = (text ?? "").Split(',');
    int r, g, b;
    return p.Length == 3 && int.TryParse(p[0], out r) && int.TryParse(p[1], out g) && int.TryParse(p[2], out b) ? new Color(Math.Max(0, Math.Min(255, r)), Math.Max(0, Math.Min(255, g)), Math.Max(0, Math.Min(255, b))) : fallback;
}

void UpdateLight(bool welderActive)
{
    int mode = (_hadError || _materialSafetyLockout) ? 1 : (_forceWelding || welderActive) ? 2 : _readyFlashTicks > 0 ? 3 : 0;
    if (_readyFlashTicks > 0 && mode == 3) _readyFlashTicks--;
    if (_statusLight == null) return;
    if (mode == _lastLightMode) return;
    _lastLightMode = mode;
    if (mode == 0) { _statusLight.Enabled = false; return; }
    _statusLight.Enabled = true;
    if (mode == 1) { _statusLight.Color = Color.Red; _statusLight.BlinkIntervalSeconds = 0; _statusLight.Intensity = 8f; return; }
    if (mode == 2) { _statusLight.Color = Color.Yellow; _statusLight.BlinkIntervalSeconds = 1f; _statusLight.BlinkLength = 50f; _statusLight.Intensity = 6f; return; }
    _statusLight.Color = Color.Green; _statusLight.BlinkIntervalSeconds = 0.5f; _statusLight.BlinkLength = 50f; _statusLight.Intensity = 8f;
}

void Add(string text) { _echo.AppendLine(text); }
