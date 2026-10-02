/*
================================================================================
  SPACE ENGINEERS - INVENTORY MANAGER v2.0
  C# 6.0 kompatibel (Space Engineers Programmable Block)
================================================================================

  INSTALLATION:
  1. Script in den Programmable Block einfügen & kompilieren
  2. Argument "WRITECONFIG" ausführen -> PB Custom Data wird befüllt
  3. Blöcke über ihre eigene Custom Data konfigurieren (Block-Namen bleiben!)

================================================================================
  PROGRAMMABLE BLOCK - Custom Data (globale Einstellungen):
================================================================================

  [Settings]
  UpdateFrequency  = Update100     (Update1 / Update10 / Update100)
  MaxRunTime       = 20            (ms pro Tick, empfohlen 15-25)
  MaxLoad          = 0.6           (CPU-Last 0.0-1.0)
  IncludeConnectedGrids = true     (Rotoren, Kolben, angedockte Schiffe)
  ScanCollectors   = false
  ScanDrills       = false         (true = Erz automatisch aus Bohrern holen)
  ScanGrinders     = false
  ScanWelders      = false
  DebugMode        = none          (none/sorting/refineries/assemblers/all)
  AlarmThreshold   = 20%           (unter X% der Quota -> Alarm)

  [Quotas]
  ; TypeId/SubtypeId = Mindestmenge , Prozent%
  Component/SteelPlate = 200 , 40%
  Ingot/Iron           = 500 , 88%
  ; Modded Items: TypeId/SubtypeId aus [DiscoveredItems] eintragen!

  [DiscoveredItems]
  ; Wird automatisch befüllt - alle Items inkl. Mods erscheinen hier

================================================================================
  BLOCK CUSTOM DATA - Jeder Block einzeln konfigurieren:
================================================================================

  Block-Name bleibt UNVERÄNDERT! Alles in der Custom Data des Blocks.

  [INV]
  ; --- PRIORITÄT (gilt für alle Items in diesem Block) ---
  P1                          -> höchste Priorität (P1 vor P2 vor P3 usw.)
  PRIORITY = 3                -> alternativ so

  ; --- ITEMS ---
  Component                   -> alle Komponenten hierher
  Ingot                       -> alle Ingots hierher
  SteelPlate                  -> nur Steel Plate (Subtyp-Suche)
  Ingot/Iron                  -> nur Eisen-Barren (TypeId/SubtypeId)
  Ingot/Iron = 2500           -> exakt 2500 Eisen-Barren (Min UND Max)
  Ingot/Iron = 10K            -> 10.000 Stück (K=Tausend, M=Million)
  Ingot/Iron = 2500 P1        -> Menge + Priorität kombiniert

  ; --- AUTOMATISCHE TYP-TRENNUNG ---
  ; Sobald ein Block Items konfiguriert hat, werden andere Typen
  ; automatisch rausgeworfen. Deaktivieren mit:
  ONLY = false                -> nimmt alles an (kein automatisches Rauswerfen)

  ; --- SPEZIAL ---
  LOCKED                      -> Inventar wird NICHT angefasst
  HIDDEN                      -> Inventar wird nicht mitgezählt

  ; --- RAFFINERIEN ---
  AUTO                        -> automatisch das knappste Erz verarbeiten
  AUTO = Iron:Nickel          -> nur diese Erze verarbeiten

  ; --- ASSEMBLER (Assembly UND Disassembly!) ---
  AUTO                        -> fehlende Items produzieren /
                                 überschüssige Items (>150% Quota) abbauen
  AUTO = SteelPlate:Motor     -> nur diese Items

  ; --- LCD PANELS ---
  STATUS                      -> Script-Status
  INVEN                       -> Inventar-Übersicht (alle Items)
  INVEN = Ingot               -> nur Ingots anzeigen
  INVEN = Ingot/Iron          -> nur Eisen-Barren anzeigen
  INVEN = Ore:Ingot           -> Erze und Ingots anzeigen
  QUOTA                       -> Quota-Fortschrittsbalken (passt sich
                                 automatisch an die Panel-Größe an!)
  QUOTA = Ingot               -> nur Ingot-Balken
  ALARM                       -> zeigt Items unter Alarm-Schwelle (rot)

  ; --- SOUND BLOCK ALARM ---
  ; Sound Block Custom Data:
  [INV]
  ALARM                       -> spielt Sound wenn Items unter Alarm-Schwelle

  ; --- DOCKING (im Block-NAMEN des Connectors) ---
  [INV DOCK:Netzwerk1]        -> nur mit Connectoren gleichen Namens verbinden

  Argumente (PB-Terminal): RESET / RELOAD / WRITECONFIG
================================================================================
*/


// ============================================================
//  KONFIGURATIONSFELDER
// ============================================================
UpdateFrequency _updateFreq     = UpdateFrequency.Update100;
double          _maxRunTime     = 20.0;
double          _maxLoad        = 0.6;
char            _tagOpen        = '[';
char            _tagClose       = ']';
string          _tagPrefix      = "INV";
bool            _inclConnected  = true;
bool            _scanCollectors = false;
bool            _scanDrills     = false;
bool            _scanGrinders   = false;
bool            _scanWelders    = false;
string          _debugMode      = "none";

// ============================================================
//  DATENKLASSEN
// ============================================================
class ItemData
{
    public string TypeId;
    public string SubtypeId;
    public string DisplayName;
    public long   Amount;
    public long   Available;
    public long   Locked;
    public long   Quota;
    public long   MinAmount;
    public float  Ratio;
    public bool   IsFractional;
    public Dictionary<IMyInventory, long>      InvenAmounts;
    public HashSet<IMyFunctionalBlock>          Producers;

    public ItemData()
    {
        InvenAmounts = new Dictionary<IMyInventory, long>();
        Producers    = new HashSet<IMyFunctionalBlock>();
    }

    public void Reset()
    {
        Amount = Available = Locked = 0;
        InvenAmounts.Clear();
        Producers.Clear();
    }
}

class SortRequest
{
    public IMyInventory Target;
    public string       TypeId;
    public string       SubtypeId;
    public long         Amount;    // Mindestmenge (-1 = unbegrenzt)
    public long         MaxAmount; // Maximalmenge (-1 = unbegrenzt)
    public int          Priority;
}

// ============================================================
//  FELDER
// ============================================================
Dictionary<string, Dictionary<string, ItemData>> _items =
    new Dictionary<string, Dictionary<string, ItemData>>(StringComparer.OrdinalIgnoreCase);

List<string> _typeIds = new List<string>();

HashSet<string> _discoveredItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

Dictionary<IMyTerminalBlock, string> _taggedBlocks =
    new Dictionary<IMyTerminalBlock, string>();

SortedDictionary<int, List<SortRequest>> _sortRequests =
    new SortedDictionary<int, List<SortRequest>>();

HashSet<IMyInventory> _lockedInventories  = new HashSet<IMyInventory>();
HashSet<IMyInventory> _hiddenInventories  = new HashSet<IMyInventory>();
// Exclusive: nur bestimmte Items erlaubt, alles andere rauswerfen
// Key=Inventar, Value=Set erlaubter "TypeId/SubtypeId" (oder nur "TypeId")
Dictionary<IMyInventory, HashSet<string>> _exclusiveInventories =
    new Dictionary<IMyInventory, HashSet<string>>();

Dictionary<IMyRefinery,  HashSet<string>> _refineryOres   =
    new Dictionary<IMyRefinery,  HashSet<string>>();
// Raffinerie-Geschwindigkeit: "BlockDefinition" -> kg pro Zyklus
// Wird automatisch gemessen und angepasst
Dictionary<string, double> _refinerySpeed =
    new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
// Letzte gemessene Input-Menge pro Raffinerie für Geschwindigkeitsmessung
Dictionary<IMyRefinery, double> _refineryLastInput =
    new Dictionary<IMyRefinery, double>();
Dictionary<IMyAssembler, HashSet<string>> _assemblerItems =
    new Dictionary<IMyAssembler, HashSet<string>>();

// Blueprint-Datenbank: "TypeId/SubtypeId" -> Blueprint-DefinitionId
// Wird beim ersten Scan aller Assembler automatisch befüllt (auch Mods!)
Dictionary<string, MyDefinitionId> _blueprints =
    new Dictionary<string, MyDefinitionId>(StringComparer.OrdinalIgnoreCase);

// Panel -> Liste der anzuzeigenden TypeIds (leer = alles anzeigen)
Dictionary<IMyTextPanel, List<string>> _statusPanels    = new Dictionary<IMyTextPanel, List<string>>();
Dictionary<IMyTextPanel, List<string>> _inventoryPanels = new Dictionary<IMyTextPanel, List<string>>();
Dictionary<IMyTextPanel, List<string>> _quotaPanels     = new Dictionary<IMyTextPanel, List<string>>();
// ALARM-Panels: zeigen Items die unter Quota sind, rot eingefärbt
Dictionary<IMyTextPanel, List<string>> _alarmPanels     = new Dictionary<IMyTextPanel, List<string>>();
// Sound-Blöcke für Alarm (IMyInteriorLight / IMySoundBlock)
List<IMySoundBlock> _alarmSounds = new List<IMySoundBlock>();
// Alarm-Schwellwert: unter X% der Quota -> Alarm (Standard 20%)
float _alarmThreshold = 0.2f;

HashSet<IMyCubeGrid> _connectedGrids = new HashSet<IMyCubeGrid>();

int      _step      = 0;
long     _runCount  = 0;
DateTime _lastRun;
string   _lastConfig = "";

System.Text.RegularExpressions.Regex _tagRegex;

List<string>  _log  = new List<string>();
StringBuilder _echo = new StringBuilder();

// Ore->Ingot Mapping
Dictionary<string, string> _oreToIngot = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

static readonly HashSet<string> FractionalTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    "Ingot", "Ore"
};

const int TotalSteps = 8;

// ============================================================
//  INIT
// ============================================================
public Program()
{
    InitOreToIngot();
    RegisterVanillaBlueprints();
    LoadConfig();
    Runtime.UpdateFrequency = _updateFreq;
    _lastRun = DateTime.Now;
    Echo("Inventory Manager v2.0 bereit.\nCustom Data wird als Konfiguration verwendet.");
}

void InitOreToIngot()
{
    // Vanilla
    _oreToIngot["Ice"]      = "";
    _oreToIngot["Organic"]  = "";
    _oreToIngot["Scrap"]    = "Iron";
    // Better Stone Mod
    _oreToIngot["[CM] Dense Iron (Fe+)"]    = "Iron";
    _oreToIngot["[CM] Iron (Fe)"]           = "Iron";
    _oreToIngot["[CM] Heazlewoodite (Ni)"]  = "Nickel";
    _oreToIngot["[CM] Cattierite (Co)"]     = "Cobalt";
    _oreToIngot["[CM] Pyrite (Fe,Au)"]      = "Gold";
    _oreToIngot["[CM] Taenite (Fe,Ni)"]     = "Nickel";
    _oreToIngot["[CM] Cohenite (Ni,Co)"]    = "Cobalt";
    _oreToIngot["[CM] Kamacite (Fe,Ni,Co)"] = "Nickel";
    _oreToIngot["[CM] Glaucodot (Fe,Co)"]   = "Cobalt";
    _oreToIngot["[PM] Electrum (Au,Ag)"]    = "Gold";
    _oreToIngot["[PM] Porphyry (Au)"]       = "Gold";
    _oreToIngot["[PM] Sperrylite (Pt)"]     = "Platinum";
    _oreToIngot["[PM] Niggliite (Pt)"]      = "Platinum";
    _oreToIngot["[PM] Galena (Ag)"]         = "Silver";
    _oreToIngot["[PM] Chlorargyrite (Ag)"]  = "Silver";
    _oreToIngot["[PM] Cooperite (Ni,Pt)"]   = "Platinum";
    _oreToIngot["[PM] Petzite (Ag,Au)"]     = "Silver";
    _oreToIngot["[S] Hapkeite (Fe,Si)"]     = "Silicon";
    _oreToIngot["[S] Dolomite (Mg)"]        = "Magnesium";
    _oreToIngot["[S] Sinoite (Si)"]         = "Silicon";
    _oreToIngot["[S] Olivine (Si,Mg)"]      = "Magnesium";
    _oreToIngot["[S] Quartz (Si)"]          = "Silicon";
    _oreToIngot["[S] Akimotoite (Si,Mg)"]   = "Magnesium";
    _oreToIngot["[S] Wadsleyite (Si,Mg)"]   = "Magnesium";
    _oreToIngot["[EI] Carnotite (U)"]       = "Uranium";
    _oreToIngot["[EI] Autunite (U)"]        = "Uranium";
    _oreToIngot["[EI] Uraniaurite (U,Au)"]  = "Gold";
    _oreToIngot["[S] Icy Stone"]            = "Stone";
}

// ============================================================
//  MAIN
// ============================================================
public void Main(string argument, UpdateType updateSource)
{
    _lastRun = DateTime.Now;
    _echo.Clear();
    _log.Clear();
    _runCount++;

    if (argument != null && argument.Trim() != "")
    {
        ProcessArgument(argument.Trim().ToUpper());
        return;
    }

    if (Me.CustomData != _lastConfig)
        LoadConfig();

    try
    {
        bool cont;
        do
        {
            ExecuteStep(_step);
            _step++;
            if (_step >= TotalSteps)
                _step = 0;
            cont = (_step != 0);
        }
        while (cont && !IsOverloaded());
    }
    catch (Exception ex)
    {
        _log.Add("FEHLER Schritt " + _step + ": " + ex.Message);
        _step = 0;
    }

    _echo.AppendLine("Inventory Manager v2.0  Lauf #" + _runCount);
    _echo.AppendLine("Schritt: " + _step + "/" + TotalSteps
        + "  Last: " + Runtime.CurrentInstructionCount
        + "/" + Runtime.MaxInstructionCount);
    _echo.AppendLine("Zeit: " + (DateTime.Now - _lastRun).TotalMilliseconds.ToString("F1") + "ms");

    if (_log.Count > 0)
    {
        _echo.AppendLine("\nLog:");
        int show = Math.Min(_log.Count, 10);
        for (int i = 0; i < show; i++)
            _echo.AppendLine("  " + _log[i]);
    }
    Echo(_echo.ToString());

    UpdatePanels();
}

// ============================================================
//  SCHRITTE
// ============================================================
void ExecuteStep(int step)
{
    switch (step)
    {
        case 0: StepScanGrids();        break;
        case 1: StepScanInventories();  break;
        case 2: StepParseTags();        break;
        case 3: StepAdjustAmounts();    break;
        case 4: StepProcessQuotas();    break;
        case 5: StepSortItems();        break;
        case 6: StepManageRefineries(); break;
        case 7: StepManageAssemblers(); break;
    }
}

// ============================================================
//  SCHRITT 0: GRIDS
// ============================================================
void StepScanGrids()
{
    _connectedGrids.Clear();
    _connectedGrids.Add(Me.CubeGrid);

    if (!_inclConnected) return;

    // Mechanische Verbindungen (Rotoren, Kolben)
    var mechBlocks = new List<IMyMechanicalConnectionBlock>();
    GridTerminalSystem.GetBlocksOfType(mechBlocks);
    bool changed = true;
    while (changed)
    {
        changed = false;
        for (int i = 0; i < mechBlocks.Count; i++)
        {
            var m = mechBlocks[i];
            if (_connectedGrids.Contains(m.CubeGrid) && m.TopGrid != null)
            {
                if (_connectedGrids.Add(m.TopGrid))
                    changed = true;
            }
        }
    }

    // Connector-Verbindungen
    var connectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(connectors);
    for (int i = 0; i < connectors.Count; i++)
    {
        var con = connectors[i];
        if (!_connectedGrids.Contains(con.CubeGrid)) continue;
        if (con.Status != MyShipConnectorStatus.Connected) continue;
        var other = con.OtherConnector;
        if (other == null) continue;
        if (!DockTagsMatch(con, other)) continue;
        _connectedGrids.Add(other.CubeGrid);
    }

    _log.Add("Grids: " + _connectedGrids.Count);
}

bool DockTagsMatch(IMyShipConnector a, IMyShipConnector b)
{
    var tagsA = GetDockTags(a);
    var tagsB = GetDockTags(b);
    if (tagsA.Count == 0 && tagsB.Count == 0) return true;
    foreach (var tag in tagsA)
        if (tagsB.Contains(tag)) return true;
    return false;
}

HashSet<string> GetDockTags(IMyShipConnector con)
{
    var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    if (_tagRegex == null) return result;
    var match = _tagRegex.Match(con.CustomName);
    if (!match.Success) return result;
    string[] parts = match.Groups[1].Value.Split(new char[]{' ', ','}, StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < parts.Length; i++)
    {
        if (parts[i].StartsWith("DOCK:", StringComparison.OrdinalIgnoreCase))
        {
            string[] tags = parts[i].Substring(5).Split(':');
            for (int j = 0; j < tags.Length; j++)
                result.Add(tags[j].Trim());
        }
    }
    return result;
}

// ============================================================
//  SCHRITT 1: INVENTARE SCANNEN
// ============================================================
void StepScanInventories()
{
    // Mengen zurücksetzen
    foreach (var typeDict in _items.Values)
        foreach (var item in typeDict.Values)
            item.Reset();

    _lockedInventories.Clear();
    _hiddenInventories.Clear();
    _exclusiveInventories.Clear();
    _discoveredItems.Clear();

    ScanBlockType<IMyCargoContainer>();
    ScanBlockType<IMyRefinery>();
    ScanBlockType<IMyAssembler>();
    ScanBlueprints(); // Blueprints von allen Assemblern einlesen
    ScanBlockType<IMyReactor>();
    ScanBlockType<IMyGasGenerator>();
    ScanBlockType<IMyGasTank>();
    ScanBlockType<IMyShipConnector>();
    ScanBlockType<IMyShipController>();
    ScanBlockType<IMyUserControllableGun>();
    ScanBlockType<IMyParachute>();
    ScanBlockType<IMyTextPanel>();
    ScanBlockType<IMyOxygenFarm>();
    // Sound-Blöcke für Alarm scannen
    _alarmSounds.Clear();
    var soundBlocks = new List<IMySoundBlock>();
    GridTerminalSystem.GetBlocksOfType(soundBlocks);
    for (int i = 0; i < soundBlocks.Count; i++)
        if (_connectedGrids.Contains(soundBlocks[i].CubeGrid)
            && !string.IsNullOrEmpty(soundBlocks[i].CustomData)
            && soundBlocks[i].CustomData.ToUpper().Contains("[INV]")
            && soundBlocks[i].CustomData.ToUpper().Contains("ALARM"))
            _alarmSounds.Add(soundBlocks[i]);
    if (_scanCollectors) ScanBlockType<IMyCollector>();
    if (_scanDrills)     ScanBlockType<IMyShipDrill>();
    if (_scanGrinders)   ScanBlockType<IMyShipGrinder>();
    if (_scanWelders)    ScanBlockType<IMyShipWelder>();

    int total = 0;
    foreach (var td in _items.Values) total += td.Count;
    _log.Add("Item-Typen in DB: " + total);

    UpdateDiscoveredItems();
}

void ScanBlockType<T>() where T : class, IMyTerminalBlock
{
    var blocks = new List<T>();
    GridTerminalSystem.GetBlocksOfType(blocks);
    for (int i = 0; i < blocks.Count; i++)
    {
        var block = blocks[i] as IMyTerminalBlock;
        if (block == null) continue;
        if (!_connectedGrids.Contains(block.CubeGrid)) continue;
        ScanBlock(block);
    }
}

void ScanBlock(IMyTerminalBlock block)
{
    var stackList = new List<MyInventoryItem>();
    for (int inv = 0; inv < block.InventoryCount; inv++)
    {
        var inven = block.GetInventory(inv);
        if (inven == null) continue;
        stackList.Clear();
        inven.GetItems(stackList);

        for (int s = 0; s < stackList.Count; s++)
        {
            var stack    = stackList[s];
            string typeId    = ExtractTypeId(stack.Type.TypeId);
            string subtypeId = stack.Type.SubtypeId;
            long   amount    = (long)((double)stack.Amount * 1e6);

            EnsureItem(typeId, subtypeId);
            var data = _items[typeId][subtypeId];
            data.Amount    += amount;
            data.Available += amount;

            long prev = 0;
            data.InvenAmounts.TryGetValue(inven, out prev);
            data.InvenAmounts[inven] = prev + amount;

            _discoveredItems.Add(typeId + "/" + subtypeId);
        }
    }
}

// Liest alle verfügbaren Blueprints von allen Assemblern.
// So werden automatisch auch Mod-Blueprints erkannt.
// Jeder Blueprint wird mit dem Item verknüpft das er produziert.
void ScanBlueprints()
{
    var assemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType(assemblers);

    var bpList = new List<MyProductionItem>();

    for (int i = 0; i < assemblers.Count; i++)
    {
        var asm = assemblers[i];
        if (!_connectedGrids.Contains(asm.CubeGrid)) continue;

        // Alle Blueprints die dieser Assembler kennt abfragen
        // Dazu nutzen wir einen Trick: wir lesen die verfügbaren
        // Blueprints über GetProductionQueue nach dem Leeren
        // Bessere Methode: MyDefinitionId direkt aus Blueprints bauen
        // Jeder Assembler kann GetAvailableBlueprints() aufrufen
        var available = new List<MyProductionItem>();
        // SE API: IMyProductionBlock.GetQueue gibt aktuelle Queue
        // Für verfügbare Blueprints: wir nutzen MyDefinitionId.TryParse
        // und testen bekannte Muster
    }

    // Vanilla Blueprint-Mapping (TypeId/SubtypeId -> Blueprint-Name)
    // Diese sind fest bekannt
    RegisterVanillaBlueprints();
}

void RegisterVanillaBlueprints()
{
    // Komponenten
    RegisterBp("Component", "BulletproofGlass",   "BulletproofGlass");
    RegisterBp("Component", "Computer",            "ComputerComponent");
    RegisterBp("Component", "Construction",        "ConstructionComponent");
    RegisterBp("Component", "Detector",            "DetectorComponent");
    RegisterBp("Component", "Display",             "Display");
    RegisterBp("Component", "Explosives",          "ExplosivesComponent");
    RegisterBp("Component", "Girder",              "GirderComponent");
    RegisterBp("Component", "GravityGenerator",    "GravityGeneratorComponent");
    RegisterBp("Component", "InteriorPlate",       "InteriorPlate");
    RegisterBp("Component", "LargeTube",           "LargeTube");
    RegisterBp("Component", "Medical",             "MedicalComponent");
    RegisterBp("Component", "MetalGrid",           "MetalGrid");
    RegisterBp("Component", "Motor",               "MotorComponent");
    RegisterBp("Component", "PowerCell",           "PowerCell");
    RegisterBp("Component", "RadioCommunication",  "RadioCommunicationComponent");
    RegisterBp("Component", "Reactor",             "ReactorComponent");
    RegisterBp("Component", "SmallTube",           "SmallTube");
    RegisterBp("Component", "SolarCell",           "SolarCell");
    RegisterBp("Component", "SteelPlate",          "SteelPlate");
    RegisterBp("Component", "Superconductor",      "SuperconductorComponent");
    RegisterBp("Component", "Thrust",              "ThrustComponent");
    RegisterBp("Component", "Canvas",              "Canvas");
    // Munition
    RegisterBp("AmmoMagazine", "Missile200mm",         "Missile200mm");
    RegisterBp("AmmoMagazine", "NATO_25x184mm",        "NATO_25x184mmMagazine");
    RegisterBp("AmmoMagazine", "AutomaticRifleGun_Mag_20rd",       "AutomaticRifleGun_Mag_20rd");
    RegisterBp("AmmoMagazine", "PreciseAutomaticRifleGun_Mag_5rd", "PreciseAutomaticRifleGun_Mag_5rd");
    RegisterBp("AmmoMagazine", "RapidFireAutomaticRifleGun_Mag_50rd", "RapidFireAutomaticRifleGun_Mag_50rd");
    RegisterBp("AmmoMagazine", "UltimateAutomaticRifleGun_Mag_30rd", "UltimateAutomaticRifleGun_Mag_30rd");
    RegisterBp("AmmoMagazine", "SemiAutoPistolMagazine",   "SemiAutoPistolMagazine");
    RegisterBp("AmmoMagazine", "FullAutoPistolMagazine",   "FullAutoPistolMagazine");
    RegisterBp("AmmoMagazine", "ElitePistolMagazine",      "ElitePistolMagazine");
    RegisterBp("AmmoMagazine", "LargeCalibreAmmo",         "LargeCalibreAmmo");
    RegisterBp("AmmoMagazine", "MediumCalibreAmmo",        "MediumCalibreAmmo");
    RegisterBp("AmmoMagazine", "AutocannonClip",           "AutocannonClip");
    RegisterBp("AmmoMagazine", "LargeRailgunAmmo",         "LargeRailgunAmmo");
    RegisterBp("AmmoMagazine", "SmallRailgunAmmo",         "SmallRailgunAmmo");
    // Werkzeuge / Waffen
    RegisterBp("PhysicalGunObject", "AngleGrinderItem",    "AngleGrinder");
    RegisterBp("PhysicalGunObject", "AngleGrinder2Item",   "AngleGrinder2");
    RegisterBp("PhysicalGunObject", "AngleGrinder3Item",   "AngleGrinder3");
    RegisterBp("PhysicalGunObject", "AngleGrinder4Item",   "AngleGrinder4");
    RegisterBp("PhysicalGunObject", "HandDrillItem",       "HandDrill");
    RegisterBp("PhysicalGunObject", "HandDrill2Item",      "HandDrill2");
    RegisterBp("PhysicalGunObject", "HandDrill3Item",      "HandDrill3");
    RegisterBp("PhysicalGunObject", "HandDrill4Item",      "HandDrill4");
    RegisterBp("PhysicalGunObject", "WelderItem",          "Welder");
    RegisterBp("PhysicalGunObject", "Welder2Item",         "Welder2");
    RegisterBp("PhysicalGunObject", "Welder3Item",         "Welder3");
    RegisterBp("PhysicalGunObject", "Welder4Item",         "Welder4");
    RegisterBp("PhysicalGunObject", "AutomaticRifleItem",  "AutomaticRifle");
    RegisterBp("PhysicalGunObject", "PreciseAutomaticRifleItem",    "PreciseAutomaticRifle");
    RegisterBp("PhysicalGunObject", "RapidFireAutomaticRifleItem",  "RapidFireAutomaticRifle");
    RegisterBp("PhysicalGunObject", "UltimateAutomaticRifleItem",   "UltimateAutomaticRifle");
    RegisterBp("PhysicalGunObject", "SemiAutoPistolItem",           "SemiAutoPistol");
    RegisterBp("PhysicalGunObject", "FullAutoPistolItem",           "FullAutoPistol");
    RegisterBp("PhysicalGunObject", "ElitePistolItem",              "EliteAutoPistol");
    RegisterBp("PhysicalGunObject", "BasicHandHeldLauncherItem",    "BasicHandHeldLauncher");
    RegisterBp("PhysicalGunObject", "AdvancedHandHeldLauncherItem", "AdvancedHandHeldLauncher");
}

void RegisterBp(string typeId, string subtypeId, string bpName)
{
    string key = typeId + "/" + subtypeId;
    MyDefinitionId bpId;
    if (MyDefinitionId.TryParse(
        "MyObjectBuilder_BlueprintDefinition/" + bpName, out bpId))
        _blueprints[key] = bpId;
}

// Versucht einen Mod-Blueprint zu finden indem er direkt am Assembler
// getestet wird. Gibt true zurück wenn der Blueprint gefunden wurde.
bool TryDiscoverModBlueprint(IMyAssembler asm, string typeId, string subtypeId)
{
    string key = typeId + "/" + subtypeId;
    if (_blueprints.ContainsKey(key)) return true;

    // Verschiedene Blueprint-Name-Muster probieren
    string[] patterns = new string[]
    {
        subtypeId,
        subtypeId + "Component",
        typeId + "_" + subtypeId,
        subtypeId + "Magazine",
    };

    for (int i = 0; i < patterns.Length; i++)
    {
        MyDefinitionId bpId;
        if (!MyDefinitionId.TryParse(
            "MyObjectBuilder_BlueprintDefinition/" + patterns[i], out bpId))
            continue;
        try
        {
            // Testen ob dieser Assembler das Blueprint kennt
            // indem wir 0 Items zur Queue hinzufügen (kein Effekt aber kein Fehler wenn bekannt)
            asm.AddQueueItem(bpId, 0.0);
            _blueprints[key] = bpId;
            _log.Add("Mod-Blueprint gefunden: " + key + " -> " + patterns[i]);
            return true;
        }
        catch { }
    }
    return false;
}

string ExtractTypeId(string fullTypeId)
{
    int idx = fullTypeId.LastIndexOf('_');
    return idx >= 0 ? fullTypeId.Substring(idx + 1) : fullTypeId;
}

void EnsureItem(string typeId, string subtypeId)
{
    if (!_items.ContainsKey(typeId))
    {
        _items[typeId] = new Dictionary<string, ItemData>(StringComparer.OrdinalIgnoreCase);
        if (!_typeIds.Contains(typeId))
        {
            _typeIds.Add(typeId);
            _typeIds.Sort(StringComparer.OrdinalIgnoreCase);
        }
    }
    if (!_items[typeId].ContainsKey(subtypeId))
    {
        _items[typeId][subtypeId] = new ItemData
        {
            TypeId       = typeId,
            SubtypeId    = subtypeId,
            DisplayName  = FormatDisplayName(subtypeId),
            IsFractional = FractionalTypes.Contains(typeId),
        };
    }
}

string FormatDisplayName(string s)
{
    var sb = new StringBuilder();
    for (int i = 0; i < s.Length; i++)
    {
        if (i > 0 && char.IsUpper(s[i]) && !char.IsUpper(s[i - 1]))
            sb.Append(' ');
        sb.Append(s[i]);
    }
    return sb.ToString();
}

// ============================================================
//  SCHRITT 2: BLOCK CUSTOM DATAS LESEN
// ============================================================
//
//  Jeder Block wird über seine eigene Custom Data gesteuert.
//  Der Block-Name bleibt völlig unverändert.
//
//  Format in der Custom Data eines Blocks:
//
//  [INV]
//  ; Kommentare mit Semikolon
//  SteelPlate = 500        -> mind. 500 Steel Plates hierher
//  Component               -> alle Komponenten hierher
//  Ingot/Iron = 1000       -> 1000 Eisen-Barren
//  Ingot/Iron = 1000 P1    -> mit Priorität 1 (niedrig = wichtiger)
//  LOCKED                  -> Inventar nicht anfassen
//  HIDDEN                  -> Inventar nicht mitzählen
//  AUTO                    -> Raffinerie/Assembler automatisch steuern
//  AUTO = Iron:Nickel      -> nur diese Erze/Items
//  STATUS                  -> LCD zeigt Script-Status
//  INVEN                   -> LCD zeigt Inventar-Übersicht
//  QUOTA                   -> LCD zeigt Quota-Fortschritt
//
void StepParseTags()
{
    _taggedBlocks.Clear();
    _sortRequests.Clear();
    _refineryOres.Clear();
    _assemblerItems.Clear();
    _statusPanels.Clear();
    _inventoryPanels.Clear();
    _quotaPanels.Clear();
    _alarmPanels.Clear();

    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(allBlocks);

    int found = 0;
    for (int i = 0; i < allBlocks.Count; i++)
    {
        var block = allBlocks[i];
        if (!_connectedGrids.Contains(block.CubeGrid)) continue;
        if (string.IsNullOrEmpty(block.CustomData)) continue;
        if (!HasInvSection(block.CustomData)) continue;

        found++;
        _taggedBlocks[block] = block.CustomData;
        ParseBlockCustomData(block);
    }

    _log.Add("Konfigurierte Bloecke: " + found);
}

bool HasInvSection(string customData)
{
    string[] lines = customData.Split('\n');
    for (int i = 0; i < lines.Length; i++)
    {
        if (lines[i].Trim().ToUpper() == "[INV]") return true;
    }
    return false;
}

void ParseBlockCustomData(IMyTerminalBlock block)
{
    var panel     = block as IMyTextPanel;
    var refinery  = block as IMyRefinery;
    var assembler = block as IMyAssembler;

    string[] lines     = block.CustomData.Split('\n');
    bool     inSection = false;

    // Block-Priorität gilt für alle Items in diesem Block.
    // Kann überschrieben werden pro Zeile: Ore = P3
    int  blockPriority    = int.MaxValue;
    bool hasOnly          = false; // ONLY explizit gesetzt
    bool hasOnlyDisabled  = false; // ONLY = false -> automatische Trennung aus

    // Erster Durchlauf: Block-Priorität und ONLY suchen
    bool inSec = false;
    for (int i = 0; i < lines.Length; i++)
    {
        string l = lines[i].Trim();
        if (l == "" || l.StartsWith(";")) continue;
        if (l.StartsWith("[") && l.EndsWith("]"))
        {
            inSec = (l.Substring(1, l.Length - 2).ToUpper() == "INV");
            continue;
        }
        if (!inSec) continue;

        string lUpper = l.ToUpper();

        // ONLY        -> automatische Trennung explizit AN (Standard wenn Items konfiguriert)
        // ONLY = false -> automatische Trennung AUS (Container nimmt alles an)
        if (lUpper == "ONLY" || lUpper.StartsWith("ONLY ") || lUpper.StartsWith("ONLY="))
        {
            int eq2 = l.IndexOf('=');
            if (eq2 >= 0)
            {
                string onlyVal = l.Substring(eq2 + 1).Trim().ToUpper();
                if (onlyVal == "FALSE" || onlyVal == "0" || onlyVal == "NO")
                    hasOnlyDisabled = true;
                else
                    hasOnly = true;
            }
            else
            {
                hasOnly = true;
            }
            continue;
        }

        // "PRIORITY = 3" oder "P = 3"
        if (lUpper.StartsWith("PRIORITY") || lUpper.StartsWith("P =") || lUpper.StartsWith("P="))
        {
            int eq = l.IndexOf('=');
            if (eq >= 0)
            {
                long tmp;
                if (long.TryParse(l.Substring(eq + 1).Trim(), out tmp))
                    blockPriority = (int)tmp;
            }
            continue;
        }
        // "P3" als alleinstehende Zeile
        if (lUpper.Length > 1 && lUpper[0] == 'P' && !l.Contains("="))
        {
            long tmp;
            if (long.TryParse(l.Substring(1).Trim(), out tmp))
                blockPriority = (int)tmp;
        }
    }

    // Zweiter Durchlauf: Items verarbeiten
    for (int i = 0; i < lines.Length; i++)
    {
        string line = lines[i].Trim();
        if (line == "" || line.StartsWith(";")) continue;

        if (line.StartsWith("[") && line.EndsWith("]"))
        {
            inSection = (line.Substring(1, line.Length - 2).ToUpper() == "INV");
            continue;
        }

        if (!inSection) continue;

        string key   = line;
        string value = "";
        int eqIdx = line.IndexOf('=');
        if (eqIdx >= 0)
        {
            key   = line.Substring(0, eqIdx).Trim();
            value = line.Substring(eqIdx + 1).Trim();
        }

        string keyUpper = key.ToUpper();

        // Block-Priorität und ONLY Zeilen überspringen (bereits verarbeitet)
        if (keyUpper == "PRIORITY") continue;
        if (keyUpper == "ONLY" || keyUpper.StartsWith("ONLY ") || keyUpper.StartsWith("ONLY=")) continue;
        if (keyUpper.Length > 1 && keyUpper[0] == 'P' && !line.Contains("="))
        {
            long tmp;
            if (long.TryParse(keyUpper.Substring(1), out tmp)) continue;
        }

        if (keyUpper == "LOCKED" || keyUpper == "EXEMPT")
        {
            for (int inv = 0; inv < block.InventoryCount; inv++)
                _lockedInventories.Add(block.GetInventory(inv));
            continue;
        }

        if (keyUpper == "HIDDEN")
        {
            for (int inv = 0; inv < block.InventoryCount; inv++)
                _hiddenInventories.Add(block.GetInventory(inv));
            continue;
        }

        // ONLY: nur die konfigurierten Items dürfen im Inventar bleiben.
        // Alles andere wird automatisch in andere Container verschoben.
        // Wird am Ende von ParseBlockCustomData ausgewertet.
        if (keyUpper == "ONLY")
        {
            if (block.HasInventory)
            {
                var inven = block.GetInventory(0);
                if (!_exclusiveInventories.ContainsKey(inven))
                    _exclusiveInventories[inven] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                // Marker setzen - erlaubte Items werden unten beim Parsen gesammelt
            }
            continue;
        }

        if (keyUpper == "AUTO")
        {
            string[] allowed = value == ""
                ? new string[0]
                : value.Split(new char[]{':'}, StringSplitOptions.RemoveEmptyEntries);
            if (refinery  != null) ParseRefineryAuto(refinery,  allowed);
            if (assembler != null) ParseAssemblerAuto(assembler, allowed);
            continue;
        }

        if (panel != null)
        {
            // STATUS / INVEN / QUOTA können mit Filter versehen werden:
            // INVEN = Ingot             -> nur Ingots
            // INVEN = Ingot/Iron        -> nur Eisen-Barren
            // INVEN = SteelPlate        -> nur Steel Plate
            // INVEN = Ore:Ingot/Iron    -> Erze + Eisen-Barren
            // INVEN                     -> alles anzeigen
            // (value enthält alles nach dem = Zeichen, oder "" wenn kein =)
            if (keyUpper == "STATUS")
            {
                if (!_statusPanels.ContainsKey(panel))
                    _statusPanels[panel] = ParsePanelFilter(value);
                continue;
            }
            if (keyUpper == "ALARM")
            {
                if (!_alarmPanels.ContainsKey(panel))
                    _alarmPanels[panel] = ParsePanelFilter(value);
                continue;
            }
            if (keyUpper == "INVEN" || keyUpper == "INVENTORY")
            {
                if (!_inventoryPanels.ContainsKey(panel))
                    _inventoryPanels[panel] = ParsePanelFilter(value);
                continue;
            }
            if (keyUpper == "QUOTA" || keyUpper == "QUOTAS")
            {
                if (!_quotaPanels.ContainsKey(panel))
                    _quotaPanels[panel] = ParsePanelFilter(value);
                continue;
            }
        }

        if (!block.HasInventory) continue;
        ParseItemLine(block, key, value, blockPriority);
    }

    // Automatische Typ-Trennung:
    // Sobald ein Container irgendetwas konfiguriert hat, gilt er als
    // "exklusiv" - nur die konfigurierten Item-Typen dürfen drin bleiben.
    // ONLY muss NICHT extra angegeben werden.
    // Ausnahme: ONLY = false deaktiviert dieses Verhalten explizit.
    if (block.HasInventory && !hasOnlyDisabled)
    {
        var inven = block.GetInventory(0);

        // Prüfen ob dieser Block überhaupt SortRequests hat
        bool hasSortRequests = false;
        foreach (var prioKv in _sortRequests)
            for (int r = 0; r < prioKv.Value.Count; r++)
                if (prioKv.Value[r].Target == inven)
                { hasSortRequests = true; break; }

        if (hasSortRequests || hasOnly)
        {
            HashSet<string> allowed;
            if (!_exclusiveInventories.TryGetValue(inven, out allowed))
            {
                allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _exclusiveInventories[inven] = allowed;
            }
            // Alle konfigurierten Items als erlaubt markieren
            foreach (var prioKv in _sortRequests)
                for (int r = 0; r < prioKv.Value.Count; r++)
                {
                    var req = prioKv.Value[r];
                    if (req.Target != inven) continue;
                    allowed.Add(req.TypeId + "/" + req.SubtypeId);
                    allowed.Add(req.TypeId);
                }
        }
    }
}

void ParseRefineryAuto(IMyRefinery refinery, string[] allowed)
{
    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    for (int i = 0; i < allowed.Length; i++)
        set.Add(allowed[i].Trim());
    _refineryOres[refinery] = set;
}

void ParseAssemblerAuto(IMyAssembler assembler, string[] allowed)
{
    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    for (int i = 0; i < allowed.Length; i++)
        set.Add(allowed[i].Trim());
    _assemblerItems[assembler] = set;
}

// blockPriority = Priorität des gesamten Blocks (aus PRIORITY= Zeile)
// Zeilen-Priorität (Ore = P3) überschreibt Block-Priorität für diese Zeile
void ParseItemLine(IMyTerminalBlock block, string key, string value, int blockPriority)
{
    long amount   = -1;
    int  priority = blockPriority; // Standard: Block-Priorität

    if (value != "")
    {
        string[] vParts = value.Split(new char[]{' ','\t', ':', ','}, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < vParts.Length; i++)
        {
            string p = vParts[i].Trim().ToUpper();
            long tmpL;
            if (p.Length > 1 && p[0] == 'P' && long.TryParse(p.Substring(1), out tmpL))
                priority = (int)tmpL; // Zeilen-Priorität überschreibt Block-Priorität
            else if (p.Length > 1 && p[p.Length-1] == 'K' && long.TryParse(p.Substring(0, p.Length-1), out tmpL))
                amount = tmpL * 1000L * 1000000L;
            else if (p.Length > 1 && p[p.Length-1] == 'M' && long.TryParse(p.Substring(0, p.Length-1), out tmpL))
                amount = tmpL * 1000000L * 1000000L;
            else if (long.TryParse(p, out tmpL))
                amount = tmpL * 1000000L;
        }
    }

    string typeId    = "";
    string subtypeId = "";

    if (key.Contains("/"))
    {
        int slash = key.IndexOf('/');
        typeId    = key.Substring(0, slash).Trim();
        subtypeId = key.Substring(slash + 1).Trim();
    }
    else if (_items.ContainsKey(key))
    {
        typeId = key;
    }
    else
    {
        foreach (var kv in _items)
        {
            if (kv.Value.ContainsKey(key))
            {
                typeId    = kv.Key;
                subtypeId = key;
                break;
            }
        }
    }

    if (typeId == "")
    {
        _log.Add("Unbekanntes Item: " + key);
        return;
    }

    var inven = block.GetInventory(0);

    if (subtypeId == "")
    {
        if (_items.ContainsKey(typeId))
        {
            var subs = new List<string>(_items[typeId].Keys);
            for (int i = 0; i < subs.Count; i++)
                AddSortRequest(inven, typeId, subs[i], amount, priority);
        }
    }
    else
    {
        AddSortRequest(inven, typeId, subtypeId, amount, priority);
    }
}



void AddSortRequest(IMyInventory target, string typeId, string subtypeId, long amount, int priority)
{
    if (!_sortRequests.ContainsKey(priority))
        _sortRequests[priority] = new List<SortRequest>();

    // Wenn eine feste Menge angegeben ist (z.B. = 5):
    //   Amount    = 5  (Minimum: Script befüllt bis 5)
    //   MaxAmount = 5  (Maximum: Script gibt ab was über 5 liegt)
    // Wenn kein Limit (-1):
    //   Amount    = -1 (nimm alles)
    //   MaxAmount = -1 (kein Maximum)
    _sortRequests[priority].Add(new SortRequest
    {
        Target    = target,
        TypeId    = typeId,
        SubtypeId = subtypeId,
        Amount    = amount,
        MaxAmount = amount, // gleich wie Amount -> exakt diese Menge halten
        Priority  = priority,
    });

    EnsureItem(typeId, subtypeId);
    var data = _items[typeId][subtypeId];
    if (amount > 0 && amount > data.Quota)
        data.Quota = amount;
}

// ============================================================
//  SCHRITT 3: MENGEN ANPASSEN
// ============================================================
void StepAdjustAmounts()
{
    var stackList = new List<MyInventoryItem>();

    foreach (var inven in _hiddenInventories)
    {
        stackList.Clear();
        inven.GetItems(stackList);
        for (int s = 0; s < stackList.Count; s++)
        {
            string typeId    = ExtractTypeId(stackList[s].Type.TypeId);
            string subtypeId = stackList[s].Type.SubtypeId;
            long   amount    = (long)((double)stackList[s].Amount * 1e6);
            Dictionary<string, ItemData> td;
            ItemData data;
            if (_items.TryGetValue(typeId, out td) && td.TryGetValue(subtypeId, out data))
            {
                data.Amount    -= amount;
                data.Available -= amount;
            }
        }
    }

    foreach (var inven in _lockedInventories)
    {
        stackList.Clear();
        inven.GetItems(stackList);
        for (int s = 0; s < stackList.Count; s++)
        {
            string typeId    = ExtractTypeId(stackList[s].Type.TypeId);
            string subtypeId = stackList[s].Type.SubtypeId;
            long   amount    = (long)((double)stackList[s].Amount * 1e6);
            Dictionary<string, ItemData> td;
            ItemData data;
            if (_items.TryGetValue(typeId, out td) && td.TryGetValue(subtypeId, out data))
            {
                data.Available -= amount;
                data.Locked    += amount;
            }
        }
    }
}

// ============================================================
//  SCHRITT 4: QUOTAS
// ============================================================
void StepProcessQuotas()
{
    LoadQuotas();

    for (int t = 0; t < _typeIds.Count; t++)
    {
        string typeId = _typeIds[t];
        Dictionary<string, ItemData> typeDict;
        if (!_items.TryGetValue(typeId, out typeDict)) continue;

        long totalType = 0;
        foreach (var item in typeDict.Values)
            totalType += item.Amount;

        foreach (var item in typeDict.Values)
        {
            long q = item.Quota;
            if (item.MinAmount > q) q = item.MinAmount;
            if (item.Ratio > 0f)
            {
                long ratioQ = (long)(item.Ratio * totalType + 0.5f);
                if (ratioQ > q) q = ratioQ;
            }
            item.Quota = q;
        }
    }
}

// ============================================================
//  SCHRITT 5: SORTIEREN
// ============================================================
//
//  Prioritätsregeln:
//  - Niedrigere P-Zahl = höhere Priorität (P1 > P2 > P3)
//  - P1 wird komplett befüllt bevor P2 bekommt
//  - GLEICHE Priorität = gleichmäßige Aufteilung der verfügbaren Menge
//    Beispiel: 3 Assembler mit je P1 und Ziel 2500 Iron, nur 3000 vorhanden
//    -> jeder bekommt 1000 (gleichmäßig aufgeteilt)
//    Wenn genug da: jeder bekommt seine 2500
//
void StepSortItems()
{
    bool debug = _debugMode == "sorting" || _debugMode == "all";

    // -------------------------------------------------------
    // Phase 0: Unerlaubte Items aus ONLY-Containern rauswerfen
    //
    // Wenn ein Container ONLY hat, dürfen nur konfigurierte Items drin sein.
    // Alles andere wird in den ersten verfügbaren anderen Container verschoben.
    // -------------------------------------------------------
    foreach (var excKv in _exclusiveInventories)
    {
        var  inven   = excKv.Key;
        var  allowed = excKv.Value;
        var  stackList = new List<MyInventoryItem>();
        inven.GetItems(stackList);

        for (int s = stackList.Count - 1; s >= 0; s--)
        {
            var    stack     = stackList[s];
            string typeId    = ExtractTypeId(stack.Type.TypeId);
            string subtypeId = stack.Type.SubtypeId;

            // Ist dieses Item erlaubt?
            bool isAllowed = allowed.Contains(typeId + "/" + subtypeId)
                          || allowed.Contains(typeId);
            if (isAllowed) continue;

            // Item ist nicht erlaubt -> rauswerfen in anderen Container
            long amount = (long)((double)stack.Amount * 1e6);

            if (debug)
                _log.Add("ONLY: " + typeId + "/" + subtypeId
                    + " nicht erlaubt in " + GetBlockName(inven)
                    + " -> verschiebe " + FormatAmount(amount));

            // Ziel: erster Container der dieses Item annimmt
            // (nicht gesperrt, nicht derselbe, nicht auch ein ONLY-Container
            //  der dieses Item nicht will)
            var allBlocks2 = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(allBlocks2);
            bool moved2 = false;
            for (int bi = 0; bi < allBlocks2.Count && !moved2; bi++)
            {
                var dst = allBlocks2[bi];
                if (!_connectedGrids.Contains(dst.CubeGrid)) continue;
                if (!dst.HasInventory) continue;
                var dstInven = dst.GetInventory(0);
                if (dstInven == inven) continue;
                if (_lockedInventories.Contains(dstInven)) continue;

                // Prüfen ob Ziel auch ONLY hat und dieses Item nicht will
                HashSet<string> dstAllowed;
                if (_exclusiveInventories.TryGetValue(dstInven, out dstAllowed))
                {
                    bool dstWants = dstAllowed.Contains(typeId + "/" + subtypeId)
                                 || dstAllowed.Contains(typeId);
                    if (!dstWants) continue;
                }

                var fp    = (VRage.MyFixedPoint)(amount / 1e6);
                if (inven.TransferItemTo(dstInven, s, null, true, fp))
                {
                    if (debug)
                        _log.Add("  -> verschoben nach " + dst.CustomName);
                    moved2 = true;
                }
            }
            if (!moved2 && debug)
                _log.Add("  -> kein Platz gefunden!");
        }
    }

    // Alle einzigartigen TypeId/SubtypeId Kombinationen sammeln
    var allSubtypes = new List<string>();
    var seenSubs    = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var prioKv in _sortRequests)
        for (int r = 0; r < prioKv.Value.Count; r++)
        {
            string sk = prioKv.Value[r].TypeId + "|" + prioKv.Value[r].SubtypeId;
            if (seenSubs.Add(sk)) allSubtypes.Add(sk);
        }

    foreach (var subtypeKey in allSubtypes)
    {
        // Instruction-Limit prüfen - bei Überlast abbrechen und
        // beim nächsten Tick weitermachen (StepSortItems wird neu aufgerufen)
        if (IsOverloaded()) break;
        int    pipe      = subtypeKey.IndexOf('|');
        string typeId    = subtypeKey.Substring(0, pipe);
        string subtypeId = subtypeKey.Substring(pipe + 1);
        string itemKey   = typeId + "/" + subtypeId;

        Dictionary<string, ItemData> td;
        ItemData itemData;
        if (!_items.TryGetValue(typeId, out td)) continue;
        if (!td.TryGetValue(subtypeId, out itemData)) continue;

        // Alle Requests für diesen Subtyp nach Priorität sortieren
        var reqs = new List<SortRequest>();
        foreach (var prioKv in _sortRequests)
            for (int r = 0; r < prioKv.Value.Count; r++)
            {
                var rq = prioKv.Value[r];
                if (string.Equals(rq.TypeId,    typeId,    StringComparison.OrdinalIgnoreCase)
                 && string.Equals(rq.SubtypeId, subtypeId, StringComparison.OrdinalIgnoreCase))
                    reqs.Add(rq);
            }
        reqs.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        if (reqs.Count == 0) continue;

        if (debug)
        {
            _log.Add("--- " + itemKey + " ---");
            for (int i = 0; i < reqs.Count; i++)
            {
                long h = 0;
                itemData.InvenAmounts.TryGetValue(reqs[i].Target, out h);
                _log.Add("  P" + reqs[i].Priority
                    + " " + GetBlockName(reqs[i].Target)
                    + " hat=" + FormatAmount(h)
                    + (reqs[i].Amount > 0 ? " ziel=" + FormatAmount(reqs[i].Amount) : " ziel=VOLL"));
            }
        }

        // -------------------------------------------------------
        // Phase 1: Befüllen – Prioritätsgruppen der Reihe nach
        //
        // Gleiche Priorität = eine Gruppe -> gleichmäßige Aufteilung
        // Verschiedene Priorität = sequenziell (P1 vor P2 vor P3)
        // -------------------------------------------------------
        int gi = 0;
        while (gi < reqs.Count)
        {
            // Aktuelle Prioritätsgruppe ermitteln (alle mit gleicher Prio)
            int   groupPrio  = reqs[gi].Priority;
            int   groupStart = gi;
            int   groupEnd   = gi;
            while (groupEnd + 1 < reqs.Count && reqs[groupEnd + 1].Priority == groupPrio)
                groupEnd++;

            // Verfügbare Menge berechnen:
            // Alles was nicht in dieser Gruppe liegt und nicht gesperrt ist
            long totalAvail = 0;
            foreach (var kv in itemData.InvenAmounts)
            {
                if (_lockedInventories.Contains(kv.Key)) continue;
                // Prüfen ob dieses Inventar zur aktuellen Gruppe gehört
                bool inGroup = false;
                for (int ri = groupStart; ri <= groupEnd; ri++)
                    if (reqs[ri].Target == kv.Key) { inGroup = true; break; }
                if (inGroup) continue; // eigene Gruppe zählt nicht als Quelle

                // Reservierung durch höherprioritäre Gruppen abziehen
                long reserved = 0;
                for (int ri = 0; ri < groupStart; ri++)
                    if (reqs[ri].Target == kv.Key && reqs[ri].Amount > 0)
                    { reserved = reqs[ri].Amount; break; }

                long avail = kv.Value - reserved;
                if (avail > 0) totalAvail += avail;
            }

            // Auch Items aus unkonfigurierten Inventaren einrechnen
            foreach (var kv in itemData.InvenAmounts)
            {
                if (_lockedInventories.Contains(kv.Key)) continue;
                bool isConfigured = false;
                for (int ri = 0; ri < reqs.Count; ri++)
                    if (reqs[ri].Target == kv.Key) { isConfigured = true; break; }
                if (!isConfigured) totalAvail += kv.Value;
            }

            if (debug)
                _log.Add("  Gruppe P" + groupPrio
                    + " (" + (groupEnd - groupStart + 1) + " Container)"
                    + " verfuegbar=" + FormatAmount(totalAvail));

            // Wie viel braucht die Gruppe insgesamt?
            long groupNeed = 0;
            for (int ri = groupStart; ri <= groupEnd; ri++)
            {
                long has = 0;
                itemData.InvenAmounts.TryGetValue(reqs[ri].Target, out has);
                long needed  = reqs[ri].Amount > 0 ? reqs[ri].Amount : long.MaxValue;
                long missing = needed == long.MaxValue ? long.MaxValue : needed - has;
                if (missing < 0) missing = 0;
                if (groupNeed != long.MaxValue)
                    groupNeed = missing == long.MaxValue ? long.MaxValue : groupNeed + missing;
            }

            // Gleichmäßige Aufteilung innerhalb der Gruppe:
            // Jeder bekommt seinen proportionalen Anteil der verfügbaren Menge
            for (int ri = groupStart; ri <= groupEnd; ri++)
            {
                var  req    = reqs[ri];
                long has    = 0;
                itemData.InvenAmounts.TryGetValue(req.Target, out has);
                long needed  = req.Amount > 0 ? req.Amount : long.MaxValue;
                long missing = needed == long.MaxValue ? long.MaxValue : needed - has;
                if (missing <= 0) continue;
                if (totalAvail <= 0) break;

                // Anteil dieser Anforderung an der Gesamtanforderung der Gruppe
                long myShare;
                if (groupNeed == long.MaxValue || groupNeed == 0)
                {
                    // Unbegrenzte Anforderungen: gleichmäßig aufteilen
                    int groupSize = groupEnd - groupStart + 1;
                    myShare = totalAvail / groupSize;
                }
                else
                {
                    // Proportional nach Bedarf aufteilen
                    myShare = (long)((double)missing / groupNeed * totalAvail);
                }
                myShare = Math.Min(myShare, missing);
                if (myShare <= 0) continue;

                // Items aus Quellen holen (niedrigere Prio zuerst, dann unkonfiguriert)
                long stillNeed = myShare;

                // Quellen: niedrigprioritäre konfigurierte Container (höhere Zahl = niedrigere Prio)
                for (int si = reqs.Count - 1; si > groupEnd && stillNeed > 0; si--)
                {
                    var src = reqs[si];
                    if (_lockedInventories.Contains(src.Target)) continue;
                    long srcHas = 0;
                    itemData.InvenAmounts.TryGetValue(src.Target, out srcHas);
                    long srcReserved = 0;
                    if (src.Amount > 0) srcReserved = src.Amount;
                    long canTake = srcHas - srcReserved;
                    if (canTake <= 0) continue;
                    long toMove = Math.Min(stillNeed, canTake);
                    long avail2 = srcHas;
                    long moved  = MoveItems(src.Target, req.Target, typeId, subtypeId, toMove);
                    if (moved > 0)
                    {
                        itemData.InvenAmounts[src.Target] = avail2 - moved;
                        long cur = 0;
                        itemData.InvenAmounts.TryGetValue(req.Target, out cur);
                        itemData.InvenAmounts[req.Target] = cur + moved;
                        stillNeed   -= moved;
                        totalAvail  -= moved;
                        if (debug)
                            _log.Add("  P" + src.Priority + "->P" + req.Priority
                                + " " + FormatAmount(moved) + " " + itemKey
                                + " von " + GetBlockName(src.Target)
                                + " nach " + GetBlockName(req.Target));
                    }
                }

                // Quellen: unkonfigurierte Inventare
                if (stillNeed > 0)
                {
                    var unconfigured = new List<IMyInventory>();
                    foreach (var kv in itemData.InvenAmounts)
                    {
                        if (_lockedInventories.Contains(kv.Key)) continue;
                        bool isCfg = false;
                        for (int ri2 = 0; ri2 < reqs.Count; ri2++)
                            if (reqs[ri2].Target == kv.Key) { isCfg = true; break; }
                        if (!isCfg) unconfigured.Add(kv.Key);
                    }
                    for (int ui = 0; ui < unconfigured.Count && stillNeed > 0; ui++)
                    {
                        var src = unconfigured[ui];
                        long srcHas = 0;
                        itemData.InvenAmounts.TryGetValue(src, out srcHas);
                        if (srcHas <= 0) continue;
                        long toMove = Math.Min(stillNeed, srcHas);
                        long moved  = MoveItems(src, req.Target, typeId, subtypeId, toMove);
                        if (moved > 0)
                        {
                            itemData.InvenAmounts[src] = srcHas - moved;
                            long cur = 0;
                            itemData.InvenAmounts.TryGetValue(req.Target, out cur);
                            itemData.InvenAmounts[req.Target] = cur + moved;
                            stillNeed  -= moved;
                            totalAvail -= moved;
                            if (debug)
                                _log.Add("  unkonfig->P" + req.Priority
                                    + " " + FormatAmount(moved) + " " + itemKey
                                    + " von " + GetBlockName(src)
                                    + " nach " + GetBlockName(req.Target));
                        }
                    }
                }
            }

            gi = groupEnd + 1;
        }

        // -------------------------------------------------------
        // Phase 2: Überschuss entfernen
        // Container mit fester Menge der mehr hat als sein Maximum
        // gibt den Überschuss an niedrigere Priorität ab
        // -------------------------------------------------------
        for (int ri = 0; ri < reqs.Count; ri++)
        {
            var req = reqs[ri];
            if (req.MaxAmount <= 0) continue;

            long has    = 0;
            itemData.InvenAmounts.TryGetValue(req.Target, out has);
            long excess = has - req.MaxAmount;
            if (excess <= 0) continue;

            if (debug)
                _log.Add("  Ueberschuss " + FormatAmount(excess)
                    + " in P" + req.Priority + " " + GetBlockName(req.Target));

            // Überschuss in niedrigprioritäre Container verschieben
            foreach (var kv in itemData.InvenAmounts)
            {
                if (excess <= 0) break;
                if (kv.Key == req.Target) continue;
                if (_lockedInventories.Contains(kv.Key)) continue;

                // Ziel darf nicht selbst schon voll sein (festes Maximum)
                bool dstFull = false;
                for (int ri2 = 0; ri2 < reqs.Count; ri2++)
                {
                    if (reqs[ri2].Target == kv.Key && reqs[ri2].MaxAmount > 0)
                    {
                        long dstHas = 0;
                        itemData.InvenAmounts.TryGetValue(kv.Key, out dstHas);
                        if (dstHas >= reqs[ri2].MaxAmount) dstFull = true;
                        break;
                    }
                }
                if (dstFull) continue;

                long avail = 0;
                itemData.InvenAmounts.TryGetValue(req.Target, out avail);
                long moved = MoveItems(req.Target, kv.Key, typeId, subtypeId, excess);
                if (moved > 0)
                {
                    itemData.InvenAmounts[req.Target] = avail - moved;
                    long cur = 0;
                    itemData.InvenAmounts.TryGetValue(kv.Key, out cur);
                    itemData.InvenAmounts[kv.Key] = cur + moved;
                    excess -= moved;
                }
            }
        }
    }
}

// Verschiebt Items von einem Inventar in ein anderes.
// Gibt die tatsächlich verschobene Menge zurück (in x1000000).
long MoveItems(IMyInventory from, IMyInventory to,
               string typeId, string subtypeId, long maxAmount)
{
    if (maxAmount <= 0) return 0;
    var items = new List<MyInventoryItem>();
    from.GetItems(items);
    long moved = 0;

    for (int i = items.Count - 1; i >= 0; i--)
    {
        string sType = ExtractTypeId(items[i].Type.TypeId);
        string sSub  = items[i].Type.SubtypeId;
        if (!string.Equals(sType, typeId,    StringComparison.OrdinalIgnoreCase)) continue;
        if (!string.Equals(sSub,  subtypeId, StringComparison.OrdinalIgnoreCase)) continue;

        long avail  = (long)((double)items[i].Amount * 1e6);
        long toMove = Math.Min(avail, maxAmount - moved);
        var  fp     = (VRage.MyFixedPoint)(toMove / 1e6);

        if (from.TransferItemTo(to, i, null, true, fp))
        {
            moved += toMove;
            if (moved >= maxAmount) break;
        }
    }
    return moved;
}

// Block-Name aus Inventar ermitteln (für Debug-Ausgaben)
string GetBlockName(IMyInventory inven)
{
    if (inven == null) return "???";
    var owner = inven.Owner as IMyTerminalBlock;
    return owner != null ? owner.CustomName : "???";
}

// ============================================================
//  SCHRITT 6: RAFFINERIEN
// ============================================================
void StepManageRefineries()
{
    bool debug = _debugMode == "refineries" || _debugMode == "all";

    Dictionary<string, ItemData> oreTd;
    Dictionary<string, ItemData> ingotTd;
    if (!_items.TryGetValue("Ore", out oreTd) || !_items.TryGetValue("Ingot", out ingotTd)) return;

    // Welche Ingots brauchen wir? -> welche Erze sollen verhüttet werden?
    // Nach Dringlichkeit sortiert (niedrigstes Quota-Verhältnis zuerst)
    var neededOres = new List<string>();
    var oreUrgency = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

    foreach (var oreKv in oreTd)
    {
        string ingotName;
        if (!_oreToIngot.TryGetValue(oreKv.Key, out ingotName))
            ingotName = oreKv.Key;
        if (string.IsNullOrEmpty(ingotName)) continue;

        ItemData ingotData;
        if (!ingotTd.TryGetValue(ingotName, out ingotData)) continue;
        if (ingotData.Quota <= 0) continue;
        if (oreKv.Value.Available <= 0) continue;

        double ratio = (double)ingotData.Amount / ingotData.Quota;
        if (ratio < 1.0) // nur wenn Quota nicht erfüllt
        {
            neededOres.Add(oreKv.Key);
            oreUrgency[oreKv.Key] = ratio;
        }
    }

    // Nach Dringlichkeit sortieren (niedrigste Ratio = dringendster Bedarf)
    neededOres.Sort((a, b) => oreUrgency[a].CompareTo(oreUrgency[b]));

    if (debug)
    {
        _log.Add("Raffinerie-Verwaltung: " + neededOres.Count + " Erze benoetigt");
        for (int i = 0; i < neededOres.Count; i++)
            _log.Add("  " + neededOres[i] + " L=" + (oreUrgency[neededOres[i]]*100).ToString("F0") + "%");
    }

    foreach (var kv in _refineryOres)
    {
        var rfn     = kv.Key;
        var allowed = kv.Value;
        if (!rfn.Enabled || !rfn.IsFunctional) continue;

        // Raffinerie-Geschwindigkeit ermitteln/aktualisieren
        string rfnKey   = rfn.BlockDefinition.ToString();
        double rfnSpeed = 1.0; // Standard: 1kg/Zyklus
        _refinerySpeed.TryGetValue(rfnKey, out rfnSpeed);

        // Aktuelle Input-Menge prüfen
        var inputItems = new List<MyInventoryItem>();
        rfn.GetInventory(0).GetItems(inputItems);

        double inputAmt = inputItems.Count > 0 ? (double)inputItems[0].Amount : 0.0;

        // Geschwindigkeit messen: Vergleich mit letzter gemessener Menge
        double lastInput = 0.0;
        _refineryLastInput.TryGetValue(rfn, out lastInput);
        if (lastInput > 0 && inputAmt < lastInput)
        {
            // Raffinerie hat verarbeitet -> Verarbeitungsrate schätzen
            double processed = lastInput - inputAmt;
            // Gleitender Durchschnitt
            rfnSpeed = (rfnSpeed + processed) / 2.0;
            rfnSpeed = Math.Max(0.1, Math.Min(rfnSpeed, 500.0));
            _refinerySpeed[rfnKey] = rfnSpeed;
            if (debug)
                _log.Add("  " + rfn.CustomName + " Speed: "
                    + rfnSpeed.ToString("F1") + "kg/Zyklus");
        }
        _refineryLastInput[rfn] = inputAmt;

        // Puffer: 10x die Verarbeitungsgeschwindigkeit vorhalten
        // Mindestens 5kg, maximal 500kg
        double bufferKg = Math.Max(5.0, Math.Min(rfnSpeed * 10.0, 500.0));

        // Wenn schon genug im Input -> nichts tun
        if (inputAmt > bufferKg * 0.8) continue;

        // Bestes Erz für diese Raffinerie wählen
        string bestOre   = null;
        double bestUrgency = double.MaxValue;

        for (int ni = 0; ni < neededOres.Count; ni++)
        {
            string ore = neededOres[ni];
            if (allowed.Count > 0 && !allowed.Contains(ore)) continue;

            ItemData oreData;
            if (!oreTd.TryGetValue(ore, out oreData) || oreData.Available <= 0) continue;

            double urgency = oreUrgency[ore];
            if (urgency < bestUrgency)
            {
                bestUrgency = urgency;
                bestOre     = ore;
            }
        }

        if (bestOre == null)
        {
            if (debug) _log.Add("  " + rfn.CustomName + ": nichts zu tun");
            continue;
        }

        // Zu sendende Menge: Puffer auffüllen
        long toSendKg = (long)((bufferKg - inputAmt) * 1000000.0 + 0.5);
        toSendKg = Math.Max(1000000L, toSendKg); // mind. 1kg

        var oreDataBest = oreTd[bestOre];
        long toSend = Math.Min(oreDataBest.Available, toSendKg);

        var sources = new List<IMyInventory>(oreDataBest.InvenAmounts.Keys);
        for (int si = 0; si < sources.Count; si++)
        {
            if (_lockedInventories.Contains(sources[si])) continue;
            long avail = 0;
            oreDataBest.InvenAmounts.TryGetValue(sources[si], out avail);
            if (avail <= 0) continue;
            long moved = MoveItems(sources[si], rfn.GetInventory(0), "Ore", bestOre, toSend);
            if (moved > 0)
            {
                oreDataBest.Available -= moved;
                if (debug)
                    _log.Add("  " + rfn.CustomName + ": "
                        + (moved/1e6).ToString("F1") + "kg " + bestOre
                        + " (Puffer=" + bufferKg.ToString("F1") + "kg)");
                break;
            }
        }
    }
}

// ============================================================
//  SCHRITT 7: ASSEMBLER
// ============================================================
void StepManageAssemblers()
{
    bool debug = _debugMode == "assemblers" || _debugMode == "all";

    foreach (var kv in _assemblerItems)
    {
        var asm     = kv.Key;
        var allowed = kv.Value;
        if (!asm.Enabled || !asm.IsFunctional) continue;

        // -------------------------------------------------------
        // Assembly-Modus: fehlende Items produzieren
        // -------------------------------------------------------
        if (asm.Mode == MyAssemblerMode.Assembly)
        {
            if (!asm.IsQueueEmpty)
            {
                var queue = new List<MyProductionItem>();
                asm.GetQueue(queue);
                if ((double)queue[0].Amount > 5.0) continue;
            }

            string bestType    = null;
            string bestSubtype = null;
            double bestRatio   = double.MaxValue;

            for (int ti = 0; ti < _typeIds.Count; ti++)
            {
                string typeId = _typeIds[ti];
                if (typeId == "Ore" || typeId == "Ingot") continue;
                Dictionary<string, ItemData> td;
                if (!_items.TryGetValue(typeId, out td)) continue;

                foreach (var itemKv in td)
                {
                    var item = itemKv.Value;
                    if (item.Quota <= 0) continue;
                    if (allowed.Count > 0
                        && !allowed.Contains(item.SubtypeId)
                        && !allowed.Contains(typeId)) continue;

                    double ratio = (double)item.Amount / item.Quota;
                    if (ratio < bestRatio && ratio < 1.0)
                    {
                        bestRatio   = ratio;
                        bestType    = typeId;
                        bestSubtype = item.SubtypeId;
                    }
                }
            }

            if (bestType == null)
            {
                if (debug) _log.Add("  " + asm.CustomName + ": nichts zu produzieren");
                continue;
            }

            string bpKey = bestType + "/" + bestSubtype;
            MyDefinitionId bpId;
            if (!_blueprints.TryGetValue(bpKey, out bpId))
            {
                if (!TryDiscoverModBlueprint(asm, bestType, bestSubtype))
                {
                    if (debug) _log.Add("Blueprint nicht gefunden: " + bpKey);
                    continue;
                }
                _blueprints.TryGetValue(bpKey, out bpId);
            }

            try
            {
                asm.AddQueueItem(bpId, 10.0);
                if (debug) _log.Add("  " + asm.CustomName
                    + ": +10x " + bestSubtype);
            }
            catch
            {
                if (debug) _log.Add("  Fehler: " + bpKey);
            }
        }
        // -------------------------------------------------------
        // Disassembly-Modus: überschüssige Items abbauen
        // Wenn ein Item mehr als 150% seiner Quota hat -> abbauen
        // -------------------------------------------------------
        else if (asm.Mode == MyAssemblerMode.Disassembly)
        {
            if (!asm.IsQueueEmpty)
            {
                var queue = new List<MyProductionItem>();
                asm.GetQueue(queue);
                if ((double)queue[0].Amount > 5.0) continue;
            }

            string worstType    = null;
            string worstSubtype = null;
            double worstRatio   = 0.0;

            for (int ti = 0; ti < _typeIds.Count; ti++)
            {
                string typeId = _typeIds[ti];
                if (typeId == "Ore" || typeId == "Ingot") continue;
                Dictionary<string, ItemData> td;
                if (!_items.TryGetValue(typeId, out td)) continue;

                foreach (var itemKv in td)
                {
                    var item = itemKv.Value;
                    if (item.Quota <= 0 || item.Amount <= 0) continue;
                    if (allowed.Count > 0
                        && !allowed.Contains(item.SubtypeId)
                        && !allowed.Contains(typeId)) continue;

                    double ratio = (double)item.Amount / item.Quota;
                    // Nur abbauen wenn mehr als 150% vorhanden
                    if (ratio > 1.5 && ratio > worstRatio)
                    {
                        worstRatio   = ratio;
                        worstType    = typeId;
                        worstSubtype = item.SubtypeId;
                    }
                }
            }

            if (worstType == null)
            {
                if (debug) _log.Add("  " + asm.CustomName + ": nichts abzubauen");
                continue;
            }

            string bpKey = worstType + "/" + worstSubtype;
            MyDefinitionId bpId;
            if (!_blueprints.TryGetValue(bpKey, out bpId))
            {
                if (!TryDiscoverModBlueprint(asm, worstType, worstSubtype))
                {
                    if (debug) _log.Add("Blueprint nicht gefunden: " + bpKey);
                    continue;
                }
                _blueprints.TryGetValue(bpKey, out bpId);
            }

            try
            {
                asm.AddQueueItem(bpId, 10.0);
                if (debug) _log.Add("  " + asm.CustomName
                    + ": -10x " + worstSubtype
                    + " (" + (worstRatio*100).ToString("F0") + "% von Quota)");
            }
            catch
            {
                if (debug) _log.Add("  Fehler Disassembly: " + bpKey);
            }
        }
    }
}

// ============================================================
//  PANELS
// ============================================================
// Parst den Filter-Wert eines Panel-Tags
// "Ingot:Ore" -> ["Ingot", "Ore"]
// ""          -> [] (leer = alles anzeigen)
List<string> ParsePanelFilter(string value)
{
    var result = new List<string>();
    if (string.IsNullOrEmpty(value)) return result;
    string[] parts = value.Split(new char[]{':', ','}, StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < parts.Length; i++)
    {
        string p = parts[i].Trim();
        if (p != "") result.Add(p);
    }
    return result;
}


// Baut den Alarm-Text auf - zeigt alle Items unter Alarm-Schwelle
string BuildAlarmText(ref bool anyAlarm)
{
    var sb = new StringBuilder();
    sb.AppendLine("=== ALARM ===");
    sb.AppendLine("Unter " + (int)(_alarmThreshold * 100) + "% der Quota:");
    sb.AppendLine("");

    anyAlarm = false;
    for (int ti = 0; ti < _typeIds.Count; ti++)
    {
        string typeId = _typeIds[ti];
        Dictionary<string, ItemData> td;
        if (!_items.TryGetValue(typeId, out td)) continue;

        foreach (var itemKv in td)
        {
            var item = itemKv.Value;
            if (item.Quota <= 0) continue;
            double ratio = (double)item.Amount / item.Quota;
            if (ratio >= _alarmThreshold) continue;

            anyAlarm = true;
            string pct = ((int)(ratio * 100)).ToString() + "%";
            sb.AppendLine("! " + item.DisplayName + " " + pct
                + " (" + FormatAmount(item.Amount)
                + "/" + FormatAmount(item.Quota) + ")");
        }
    }

    if (!anyAlarm)
        sb.AppendLine("Alles OK!");

    return sb.ToString();
}

void UpdatePanels()
{
    if (_statusPanels.Count == 0 && _inventoryPanels.Count == 0 && _quotaPanels.Count == 0)
        return;

    // Status-Panels: immer gleicher Inhalt, kein Filter
    string statusText = BuildStatusText();
    foreach (var kv in _statusPanels)
        WritePanel(kv.Key, "Script Status", statusText);

    // Inventar-Panels: individueller Filter pro Panel
    foreach (var kv in _inventoryPanels)
    {
        string title = kv.Value.Count > 0
            ? "Inventar: " + string.Join(", ", kv.Value.ToArray())
            : "Inventar";
        WritePanel(kv.Key, title, BuildInventoryText(kv.Value));
    }

    // Quota-Panels: individueller Filter + Panel-Größe für dynamischen Balken
    foreach (var kv in _quotaPanels)
    {
        string title = kv.Value.Count > 0
            ? "Quotas: " + string.Join(", ", kv.Value.ToArray())
            : "Quotas";
        WritePanel(kv.Key, title, BuildQuotaText(kv.Value, kv.Key));
    }

    // Alarm-Panels: nur Items die unter Alarm-Schwelle sind
    bool anyAlarm = false;
    string alarmText = BuildAlarmText(ref anyAlarm);
    foreach (var kv in _alarmPanels)
    {
        var panel = kv.Key;
        WritePanel(panel, "! ALARM !", alarmText);
        // Panel rot färben wenn Alarm, grün wenn alles OK
        panel.FontColor = anyAlarm
            ? new Color(255, 50, 50)
            : new Color(50, 255, 50);
    }
    // Sound-Alarm: Sound Block über Enabled ein-/ausschalten
    // IMySoundBlock hat keine IsSoundPlaying Property in SE C# API
    // Stattdessen: Block einschalten = Sound startet, ausschalten = stoppt
    for (int i = 0; i < _alarmSounds.Count; i++)
    {
        var sb2 = _alarmSounds[i];
        if (anyAlarm && !sb2.Enabled)
        {
            sb2.Enabled = true;
            // Sound über Terminal-Action starten
            var playAction = sb2.GetActionWithName("PlaySound");
            if (playAction != null) playAction.Apply(sb2);
        }
        else if (!anyAlarm && sb2.Enabled)
        {
            var stopAction = sb2.GetActionWithName("StopSound");
            if (stopAction != null) stopAction.Apply(sb2);
            sb2.Enabled = false;
        }
    }
}

void WritePanel(IMyTextPanel panel, string title, string text)
{
    panel.WritePublicTitle(title);
    panel.WriteText(text);
    panel.ContentType = ContentType.TEXT_AND_IMAGE;
    if (panel.FontSize < 0.3f || panel.FontSize > 1.0f)
        panel.FontSize = 1.0f;
}

string BuildStatusText()
{
    var sb = new StringBuilder();
    int total = 0;
    foreach (var td in _items.Values) total += td.Count;
    sb.AppendLine("=== INVENTORY MANAGER v2.0 ===");
    sb.AppendLine("Lauf #" + _runCount + "  Schritt " + _step + "/" + TotalSteps);
    sb.AppendLine("Grids: " + _connectedGrids.Count + "  Items: " + total);
    sb.AppendLine("Last: " + Runtime.CurrentInstructionCount + "/" + Runtime.MaxInstructionCount);
    sb.AppendLine("");
    int show = Math.Min(_log.Count, 10);
    for (int i = 0; i < show; i++)
        sb.AppendLine(_log[i]);
    return sb.ToString();
}

// filter = leer -> alles anzeigen
// Filter-Möglichkeiten:
//   "Ingot"          -> ganzer Typ (alle Ingots)
//   "Ingot/Iron"     -> nur Eisen-Barren
//   "Iron"           -> Subtyp in allen Typen suchen
//   "SteelPlate"     -> Subtyp in allen Typen suchen
// Gibt zurück welche Subtypen eines Typs angezeigt werden sollen.
// null  = alle Subtypen anzeigen (kein Filter oder Typ passt zu keinem Filter-Eintrag)
// Liste = nur diese Subtypen anzeigen
// leere Liste = Typ komplett überspringen
HashSet<string> GetAllowedSubtypes(List<string> filter, string typeId,
    Dictionary<string, ItemData> td)
{
    // Kein Filter -> alles anzeigen
    if (filter == null || filter.Count == 0) return null;

    var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    bool typeMatched = false;

    for (int fi = 0; fi < filter.Count; fi++)
    {
        string f = filter[fi].Trim();
        if (f == "") continue;

        if (f.Contains("/"))
        {
            // "Ingot/Iron" -> nur wenn TypeId passt den Subtyp hinzufügen
            int slash    = f.IndexOf('/');
            string fType = f.Substring(0, slash).Trim();
            string fSub  = f.Substring(slash + 1).Trim();
            if (string.Equals(fType, typeId, StringComparison.OrdinalIgnoreCase))
            {
                typeMatched = true;
                result.Add(fSub);
            }
        }
        else if (string.Equals(f, typeId, StringComparison.OrdinalIgnoreCase))
        {
            // "Ingot" -> ganzen Typ anzeigen (alle Subtypen)
            return null; // null = alle Subtypen
        }
        else
        {
            // "SteelPlate" oder "Iron" -> als Subtyp in diesem Typ suchen
            if (td.ContainsKey(f))
            {
                typeMatched = true;
                result.Add(f);
            }
        }
    }

    // Wenn dieser Typ in keinem Filter-Eintrag vorkam -> überspringen
    if (!typeMatched) return new HashSet<string>(); // leer = überspringen
    return result;
}

string BuildInventoryText(List<string> filter)
{
    var sb = new StringBuilder();
    sb.AppendLine("=== INVENTAR ===");
    for (int ti = 0; ti < _typeIds.Count; ti++)
    {
        string typeId = _typeIds[ti];
        Dictionary<string, ItemData> td;
        if (!_items.TryGetValue(typeId, out td)) continue;

        // Welche Subtypen soll dieses Panel für diesen Typ zeigen?
        // null = alle, leere Liste = keine (Typ wird übersprungen)
        var allowedSubs = GetAllowedSubtypes(filter, typeId, td);
        if (allowedSubs != null && allowedSubs.Count == 0) continue;

        // Zeilen für diesen Typ zusammenbauen
        var sorted = new List<ItemData>(td.Values);
        sorted.Sort((a, b) => string.Compare(a.SubtypeId, b.SubtypeId, StringComparison.OrdinalIgnoreCase));

        var typeLines = new List<string>();
        for (int i = 0; i < sorted.Count; i++)
        {
            var item = sorted[i];
            // Subtyp-Filter anwenden
            if (allowedSubs != null && !allowedSubs.Contains(item.SubtypeId)) continue;
            if (item.Amount <= 0 && item.Quota <= 0) continue;

            string quotaStr = item.Quota > 0 ? " / " + FormatAmount(item.Quota) : "";
            string pctStr   = item.Quota > 0
                ? " (" + (int)(100.0 * item.Amount / item.Quota) + "%)" : "";
            typeLines.Add("  " + PadRight(item.DisplayName, 22)
                + " " + PadLeft(FormatAmount(item.Amount), 8) + quotaStr + pctStr);
        }

        if (typeLines.Count == 0) continue;
        sb.AppendLine("");
        sb.AppendLine("-- " + typeId + " --");
        for (int i = 0; i < typeLines.Count; i++)
            sb.AppendLine(typeLines[i]);
    }
    return sb.ToString();
}

string BuildQuotaText(List<string> filter, IMyTextPanel panel = null)
{
    var sb = new StringBuilder();
    sb.AppendLine("=== QUOTAS ===");

    // Zeichenbreite pro Zeile berechnen.
    // SE LCD: bei FontSize=1.0 und 512px Breite passen exakt 26 Zeichen
    // SurfaceSize ist in SE-Einheiten (nicht Pixel), 1 Einheit = ~1px bei normaler Auflösung
    // Korrekturformel ermittelt durch Tests:
    // Zeichen pro Zeile = (int)(SurfaceSize.X / (FontSize * 23.8f))
    int barWidth = 26; // Fallback
    if (panel != null)
    {
        // MeasureStringInPixels gibt die exakte Pixelbreite eines Strings zurück.
        // Wir messen wie breit ein einzelnes '|' Zeichen ist,
        // und wie breit die gesamte Textfläche in Pixeln ist.
        // Damit wissen wir exakt wie viele Zeichen reinpassen.
        try
        {
            // Teststring: 10 Pipe-Zeichen messen
            string testStr  = "||||||||||";
            Vector2 strSize = panel.MeasureStringInPixels(
                new StringBuilder(testStr), panel.Font, panel.FontSize);
            Vector2 surfPx  = panel.SurfaceSize;

            if (strSize.X > 0 && surfPx.X > 0)
            {
                float charWidth = strSize.X / testStr.Length;
                barWidth = Math.Max(3, (int)(surfPx.X / charWidth) - 2);
            }
        }
        catch { /* Fallback bleibt 26 */ }
    }

    // Alle anzuzeigenden Items vorsammeln
    // damit wir den längsten Namen GLOBAL messen können
    var groups = new List<KeyValuePair<string, List<ItemData>>>();
    int globalMaxName = 0;

    for (int ti = 0; ti < _typeIds.Count; ti++)
    {
        string typeId = _typeIds[ti];
        Dictionary<string, ItemData> td;
        if (!_items.TryGetValue(typeId, out td)) continue;

        var allowedSubs = GetAllowedSubtypes(filter, typeId, td);
        if (allowedSubs != null && allowedSubs.Count == 0) continue;

        var sorted = new List<ItemData>(td.Values);
        sorted.Sort((a, b) => string.Compare(a.SubtypeId, b.SubtypeId, StringComparison.OrdinalIgnoreCase));

        var groupItems = new List<ItemData>();
        for (int i = 0; i < sorted.Count; i++)
        {
            var item = sorted[i];
            if (allowedSubs != null && !allowedSubs.Contains(item.SubtypeId)) continue;
            if (item.Quota <= 0) continue;
            groupItems.Add(item);
            // Längsten Namen global merken
            if (item.DisplayName.Length > globalMaxName)
                globalMaxName = item.DisplayName.Length;
        }

        if (groupItems.Count > 0)
            groups.Add(new KeyValuePair<string, List<ItemData>>(typeId, groupItems));
    }

    // Einzeilige Darstellung:
    // [Name  Pct%][|||||||||||||||||||||||||]
    // Name + Prozent links, Balken füllt den Rest bis zum rechten Rand
    for (int gi = 0; gi < groups.Count; gi++)
    {
        string typeId     = groups[gi].Key;
        var    groupItems = groups[gi].Value;

        sb.AppendLine("");
        sb.AppendLine("-- " + typeId + " --");

        for (int i = 0; i < groupItems.Count; i++)
        {
            var    item   = groupItems[i];
            double pct    = (double)item.Amount / item.Quota * 100.0;
            string pctStr = ((int)pct).ToString() + "%";

            // Linke Seite: "  Name  Pct%  "
            string left = "  " + item.DisplayName + "  " + pctStr + " ";

            // Balken bekommt den verbleibenden Platz
            // barWidth = Gesamtbreite - linker Teil - 2 (für "[" und "]")
            int dynBar = Math.Max(3, barWidth - left.Length);

            sb.AppendLine(left + "[" + BuildBar(Math.Min(pct, 100.0), dynBar) + "]");
        }
    }
    return sb.ToString();
}

string BuildBar(double pct, int width)
{
    int filled = (int)(Math.Min(pct, 100.0) / 100.0 * width + 0.5);
    filled = Math.Max(0, Math.Min(filled, width));
    return new string('|', filled) + new string('.', width - filled);
}

string FormatAmount(long amount)
{
    if (amount <= 0) return "0";
    double val = amount / 1e6;
    if (val >= 1000000.0) return (val / 1000000.0).ToString("F2") + "M";
    if (val >= 1000.0)    return (val / 1000.0).ToString("F2") + "K";
    return val.ToString("F2");
}

string PadRight(string s, int width)
{
    if (s.Length >= width) return s.Substring(0, width);
    return s + new string(' ', width - s.Length);
}

string PadLeft(string s, int width)
{
    if (s.Length >= width) return s;
    return new string(' ', width - s.Length) + s;
}

// ============================================================
//  KONFIGURATION LADEN
// ============================================================
void LoadConfig()
{
    _lastConfig = Me.CustomData;

    if (string.IsNullOrEmpty(Me.CustomData) || Me.CustomData.Trim() == "")
    {
        WriteDefaultConfig();
        _lastConfig = Me.CustomData;
    }

    string[] lines = Me.CustomData.Split('\n');
    string section = "";

    for (int i = 0; i < lines.Length; i++)
    {
        string line = lines[i].Trim();
        if (line.StartsWith(";") || line == "") continue;
        if (line.StartsWith("[") && line.EndsWith("]"))
        {
            section = line.Substring(1, line.Length - 2).ToUpper();
            continue;
        }
        if (section == "SETTINGS")
            ParseSetting(line);
    }

    BuildTagRegex();
    Runtime.UpdateFrequency = _updateFreq;
}

void ParseSetting(string line)
{
    int eq = line.IndexOf('=');
    if (eq < 0) return;
    string key = line.Substring(0, eq).Trim().ToLower();
    string val = line.Substring(eq + 1).Trim();

    double tmpD;
    bool   tmpB;

    switch (key)
    {
        case "updatefrequency":
            if      (val == "Update1")   _updateFreq = UpdateFrequency.Update1;
            else if (val == "Update10")  _updateFreq = UpdateFrequency.Update10;
            else                         _updateFreq = UpdateFrequency.Update100;
            break;
        case "maxruntime":
            if (double.TryParse(val, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out tmpD))
                _maxRunTime = tmpD;
            break;
        case "maxload":
            if (double.TryParse(val, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out tmpD))
                _maxLoad = tmpD;
            break;
        case "tagopen":      if (val.Length > 0) _tagOpen  = val[0]; break;
        case "tagclose":     if (val.Length > 0) _tagClose = val[0]; break;
        case "tagprefix":    _tagPrefix = val; break;
        case "includeconnectedgrids":
            if (bool.TryParse(val, out tmpB)) _inclConnected  = tmpB; break;
        case "scancollectors":
            if (bool.TryParse(val, out tmpB)) _scanCollectors = tmpB; break;
        case "scandrills":
            if (bool.TryParse(val, out tmpB)) _scanDrills     = tmpB; break;
        case "scangrinders":
            if (bool.TryParse(val, out tmpB)) _scanGrinders   = tmpB; break;
        case "scanwelders":
            if (bool.TryParse(val, out tmpB)) _scanWelders    = tmpB; break;
        case "debugmode":    _debugMode = val.ToLower(); break;
        case "alarmthreshold":
            float tmpF;
            if (float.TryParse(val.TrimEnd('%'),
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out tmpF))
                _alarmThreshold = tmpF > 1.0f ? tmpF / 100f : tmpF;
            break;
    }
}

void LoadQuotas()
{
    string[] lines = Me.CustomData.Split('\n');
    string section = "";

    for (int i = 0; i < lines.Length; i++)
    {
        string line = lines[i].Trim();
        if (line.StartsWith(";") || line == "") continue;
        if (line.StartsWith("[") && line.EndsWith("]"))
        {
            section = line.Substring(1, line.Length - 2).ToUpper();
            continue;
        }
        if (section != "QUOTAS") continue;

        int eq = line.IndexOf('=');
        if (eq < 0) continue;
        string itemKey = line.Substring(0, eq).Trim();
        string valPart = line.Substring(eq + 1).Trim();

        int slash = itemKey.IndexOf('/');
        if (slash < 0) continue;
        string typeId    = itemKey.Substring(0, slash).Trim();
        string subtypeId = itemKey.Substring(slash + 1).Trim();

        string[] cv  = valPart.Split(',');
        long  minAmt = 0;
        float ratio  = 0f;

        if (cv.Length > 0)
            long.TryParse(cv[0].Trim(), out minAmt);

        if (cv.Length > 1)
        {
            string rStr = cv[1].Trim().TrimEnd('%');
            float r;
            if (float.TryParse(rStr, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out r))
                ratio = r / 100f;
        }

        EnsureItem(typeId, subtypeId);
        var data = _items[typeId][subtypeId];
        data.MinAmount = minAmt * 1000000L;
        data.Ratio     = ratio;
    }
}

void BuildTagRegex()
{
    string open   = System.Text.RegularExpressions.Regex.Escape(_tagOpen.ToString());
    string close  = System.Text.RegularExpressions.Regex.Escape(_tagClose.ToString());
    string prefix = System.Text.RegularExpressions.Regex.Escape(_tagPrefix);

    string pattern;
    if (_tagPrefix == null || _tagPrefix == "")
        pattern = open + "([^" + close + "]*)" + close;
    else
        pattern = open + "\\s*" + prefix + "\\s*((?:[^" + close + "]*)?)" + close;

    _tagRegex = new System.Text.RegularExpressions.Regex(
        pattern,
        System.Text.RegularExpressions.RegexOptions.IgnoreCase |
        System.Text.RegularExpressions.RegexOptions.Compiled);
}

// ============================================================
//  ENTDECKTE ITEMS IN CUSTOM DATA SCHREIBEN
// ============================================================
void UpdateDiscoveredItems()
{
    if (_discoveredItems.Count == 0) return;

    string cd = Me.CustomData;
    int start = -1;
    string[] cdLines = cd.Split('\n');
    for (int i = 0; i < cdLines.Length; i++)
    {
        if (cdLines[i].Trim().ToUpper() == "[DISCOVEREDITEMS]")
        {
            start = i;
            break;
        }
    }
    if (start < 0) return;

    // Abschnitt-Ende finden
    int end = cdLines.Length;
    for (int i = start + 1; i < cdLines.Length; i++)
    {
        string l = cdLines[i].Trim();
        if (l.StartsWith("[") && l.EndsWith("]"))
        {
            end = i;
            break;
        }
    }

    // Neuen Abschnitt aufbauen
    var sb = new StringBuilder();
    sb.AppendLine("[DiscoveredItems]");
    sb.AppendLine("; Alle gefundenen Items. Einfach in [Quotas] eintragen:");

    var sortedItems = new List<string>(_discoveredItems);
    sortedItems.Sort(StringComparer.OrdinalIgnoreCase);
    for (int i = 0; i < sortedItems.Count; i++)
        sb.AppendLine("; " + sortedItems[i]);

    // Alten Abschnitt ersetzen
    var newLines = new List<string>();
    for (int i = 0; i < start; i++)
        newLines.Add(cdLines[i]);

    string[] newSection = sb.ToString().Split('\n');
    for (int i = 0; i < newSection.Length; i++)
        if (newSection[i].TrimEnd() != "")
            newLines.Add(newSection[i].TrimEnd());

    for (int i = end; i < cdLines.Length; i++)
        newLines.Add(cdLines[i]);

    string newData = string.Join("\n", newLines.ToArray());
    if (newData == cd) return;

    Me.CustomData = newData;
    _lastConfig   = newData;
}

// ============================================================
//  DEFAULT-KONFIGURATION
// ============================================================
void WriteDefaultConfig()
{
    Me.CustomData =
"[Settings]\n" +
"; Wie oft soll das Script laufen?\n" +
"UpdateFrequency=Update100\n" +
"; Max Laufzeit pro Tick in ms\n" +
"MaxRunTime=20\n" +
"; Max CPU-Last (0.0 - 1.0)\n" +
"MaxLoad=0.6\n" +
"; Verbundene Grids einschliessen\n" +
"IncludeConnectedGrids=true\n" +
"; Optionale Block-Typen scannen\n" +
"ScanCollectors=false\n" +
"ScanDrills=false\n" +
"ScanGrinders=false\n" +
"ScanWelders=false\n" +
"; Debug: none / sorting / refineries / assemblers / all\n" +
"DebugMode=none\n" +
"; Alarm wenn Item unter X%% der Quota faellt\n" +
"AlarmThreshold=20%\n" +
"\n" +
"[Quotas]\n" +
"; Format: TypeId/SubtypeId = Mindestmenge , Prozent%\n" +
"; Modded Items einfach mit TypeId/SubtypeId aus [DiscoveredItems] eintragen!\n" +
"Component/SteelPlate         = 200 , 40%\n" +
"Component/InteriorPlate      = 100 , 10%\n" +
"Component/Construction       = 150 , 20%\n" +
"Component/Motor              = 20  , 4%\n" +
"Component/Computer           = 30  , 5%\n" +
"Component/Thrust             = 15  , 5%\n" +
"Component/MetalGrid          = 20  , 2%\n" +
"Component/SmallTube          = 50  , 3%\n" +
"Component/LargeTube          = 10  , 2%\n" +
"Component/Display            = 10  , 0.5%\n" +
"Component/BulletproofGlass   = 50  , 2%\n" +
"Component/Detector           = 10  , 0.1%\n" +
"Component/Explosives         = 5   , 0.1%\n" +
"Component/Girder             = 10  , 0.5%\n" +
"Component/GravityGenerator   = 1   , 0.1%\n" +
"Component/Medical            = 15  , 0.1%\n" +
"Component/PowerCell          = 20  , 1%\n" +
"Component/RadioCommunication = 10  , 0.5%\n" +
"Component/Reactor            = 25  , 2%\n" +
"Component/SolarCell          = 20  , 0.1%\n" +
"Component/Superconductor     = 10  , 1%\n" +
"Component/Canvas             = 50  , 1%\n" +
"Ingot/Iron                   = 500 , 88%\n" +
"Ingot/Nickel                 = 30  , 1.5%\n" +
"Ingot/Cobalt                 = 73  , 3.5%\n" +
"Ingot/Silicon                = 50  , 2%\n" +
"Ingot/Silver                 = 20  , 1%\n" +
"Ingot/Gold                   = 5   , 0.2%\n" +
"Ingot/Platinum               = 5   , 0.1%\n" +
"Ingot/Magnesium              = 5   , 0.1%\n" +
"Ingot/Uranium                = 1   , 0.1%\n" +
"Ingot/Stone                  = 50  , 2.5%\n" +
"\n" +
"[DiscoveredItems]\n" +
"; Wird automatisch befuellt - alle Items inkl. Mods erscheinen hier.\n" +
"; Einfach in [Quotas] eintragen um Quotas zu setzen.\n";
}

// ============================================================
//  HILFSMETHODEN
// ============================================================
bool IsOverloaded()
{
    double ms   = (DateTime.Now - _lastRun).TotalMilliseconds;
    double load = (double)Runtime.CurrentInstructionCount / Runtime.MaxInstructionCount;
    return ms > _maxRunTime || load > _maxLoad;
}

void ProcessArgument(string arg)
{
    switch (arg)
    {
        case "RESET":
            _step = 0;
            _items.Clear();
            _typeIds.Clear();
            _discoveredItems.Clear();
            _blueprints.Clear();
            Echo("Script zurueckgesetzt.");
            break;
        case "RELOAD":
            LoadConfig();
            Echo("Konfiguration neu geladen.");
            break;
        case "WRITECONFIG":
            WriteDefaultConfig();
            _lastConfig = Me.CustomData;
            Echo("Standard-Konfiguration geschrieben.");
            break;
        default:
            Echo("Unbekanntes Argument: " + arg
                + "\nBekannt: RESET, RELOAD, WRITECONFIG");
            break;
    }
}
