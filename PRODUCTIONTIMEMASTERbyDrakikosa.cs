// =========================================================
// PRODUCTION TIME MASTER by Drakikosa
// Montage + Raffinerie Kombi Script
// DE / EN | Multi-LCD | Einzel-LCDs
// =========================================================

string LANGUAGE = "DE"; // DE oder EN

// Montage LCDs
const string ASM_MAIN_LCD_NAME = "!Montage_Time";
const string ASM_LCD_PREFIX = "!Montage_LCD ";
const string ASM_LINK_MARKER = ":Link=M";

// Raffinerie LCDs
const string REF_MAIN_LCD_NAME = "!Raffinerie_Time";
const string REF_LCD_PREFIX = "!Raffinerie_LCD ";
const string REF_LINK_MARKER = ":Link=R";

const float ASM_LCD_FONT_SIZE = 0.66f;
const float REF_LCD_FONT_SIZE = 0.55f;

const int ASM_LINES_PER_LCD = 26;
const int REF_LINES_PER_LCD = 24;

const int SERVER_REFINERY_SPEED = 1;
const int BAR_WIDTH = 16;

// LCDs
IMyTextPanel _asmMainLcd;
IMyTextPanel _refMainLcd;

List<IMyTextPanel> _asmLinkedLcds = new List<IMyTextPanel>();
List<IMyTextPanel> _asmPageLcds = new List<IMyTextPanel>();

List<IMyTextPanel> _refLinkedLcds = new List<IMyTextPanel>();
List<IMyTextPanel> _refPageLcds = new List<IMyTextPanel>();

// Montage
List<IMyAssembler> _assemblers = new List<IMyAssembler>();
List<MyProductionItem> _queue = new List<MyProductionItem>();

Dictionary<string, double> _asmOutputItems = new Dictionary<string, double>();
Dictionary<string, double> _asmQueuedItems = new Dictionary<string, double>();

Dictionary<string, string> _namesDE = new Dictionary<string, string>();
Dictionary<string, string> _namesEN = new Dictionary<string, string>();

// Raffinerie
List<IMyRefinery> _refineries = new List<IMyRefinery>();
List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
List<MyInventoryItem> _items = new List<MyInventoryItem>();

Dictionary<string, double> _oreAmounts = new Dictionary<string, double>();
Dictionary<string, double> _oreRates = new Dictionary<string, double>();

public Program()
{
    InitItemLibrary();
    InitOreRates();

    _asmMainLcd = GridTerminalSystem.GetBlockWithName(ASM_MAIN_LCD_NAME) as IMyTextPanel;
    _refMainLcd = GridTerminalSystem.GetBlockWithName(REF_MAIN_LCD_NAME) as IMyTextPanel;

    if (_asmMainLcd != null)
        ConfigureLCD(_asmMainLcd, ASM_LCD_FONT_SIZE, new Color(0, 180, 255), 1);

    if (_refMainLcd != null)
        ConfigureLCD(_refMainLcd, REF_LCD_FONT_SIZE, new Color(0, 255, 120), 2);

    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    // Montage
    FindAssemblers();
    FindLinkedLCDs(ASM_LINK_MARKER, _asmLinkedLcds, ASM_LCD_FONT_SIZE, new Color(0, 180, 255), 1);
    BuildPageLCDList(_asmMainLcd, _asmLinkedLcds, _asmPageLcds);
    ScanAssemblerData();
    WriteAssemblyOverviewLCD();
    WriteAssemblerLCDs();

    // Raffinerie
    FindRefineries();
    FindLinkedLCDs(REF_LINK_MARKER, _refLinkedLcds, REF_LCD_FONT_SIZE, new Color(0, 255, 120), 2);
    BuildPageLCDList(_refMainLcd, _refLinkedLcds, _refPageLcds);
    ScanAllOres();
    WriteRefineryOverviewLCD();
    WriteRefineryLCDs();

    Echo("Production Time Master");
    Echo("----------------------");
    Echo("Language: " + LANGUAGE);
    Echo("Assembler: " + _assemblers.Count);
    Echo("Raffinerien: " + _refineries.Count);
    Echo("ASM LCDs: " + _asmPageLcds.Count);
    Echo("REF LCDs: " + _refPageLcds.Count);
    Echo("Runtime: " + Runtime.LastRunTimeMs.ToString("0.00") + " ms");
}

// =========================================================
// ITEMLISTE
// =========================================================

void InitItemLibrary()
{
    AddItem("Construction", "Herstellungskomponenten", "Construction");
    AddItem("MetalGrid", "Metallgitter", "Metal Grid");
    AddItem("InteriorPlate", "Interne Panzerung", "Interior Plate");
    AddItem("SteelPlate", "Stahlplatte", "Steel Plate");
    AddItem("Girder", "Träger", "Girder");
    AddItem("SmallTube", "Kl. Stahlrohr", "Small Tube");
    AddItem("LargeTube", "Gr. Stahlrohr", "Large Tube");
    AddItem("Motor", "Motor", "Motor");
    AddItem("Display", "Anzeige", "Display");
    AddItem("BulletproofGlass", "Panzerglas", "Bulletproof Glass");
    AddItem("Computer", "Computer", "Computer");
    AddItem("Reactor", "Reaktorkomponenten", "Reactor");
    AddItem("Thrust", "Triebwerk-Komponenten", "Thruster");
    AddItem("GravityGenerator", "Schwerkraftgenerator-Komponenten", "Gravity Gen.");
    AddItem("Medical", "Medizinische Komponenten", "Medical");
    AddItem("RadioCommunication", "Kommunikationssystem-Komponenten", "Radio Comm.");
    AddItem("Detector", "Sensorkomponente", "Detector");
    AddItem("Explosives", "Sprengstoff", "Explosives");
    AddItem("SolarCell", "Solarzelle", "Solar Cell");
    AddItem("PowerCell", "Energiezelle", "Power Cell");
    AddItem("Superconductor", "Supraleiter", "Superconductor");
    AddItem("Canvas", "Fallschirmseide", "Canvas");

    AddItem("NATO_5p56x45mm", "5.56x45 NATO", "5.56x45 NATO");
    AddItem("NATO_25x184mm", "25x184 NATO", "25x184 NATO");
    AddItem("Missile200mm", "200mm Rakete", "200mm Missile");
    AddItem("AutocannonClip", "Maschinenkanone", "Autocannon");
    AddItem("MediumCalibreAmmo", "Sturmgeschütz", "Assault Cannon");
    AddItem("LargeCalibreAmmo", "Artillerie", "Artillery");
    AddItem("SmallRailgunAmmo", "Kl. Railgun", "Small Railgun");
    AddItem("LargeRailgunAmmo", "Gr. Railgun", "Large Railgun");
    AddItem("FlareClip", "Signalmunition", "Flare Clip");

    AddItem("SemiAutoPistolMagazine", "S-10 Magazin", "S-10 Mag");
    AddItem("ElitePistolMagazine", "S-10E Magazin", "S-10E Mag");
    AddItem("FullAutoPistolMagazine", "S-20A Magazin", "S-20A Mag");
    AddItem("AutomaticRifleGun_Mag_20rd", "MR-20 Magazin", "MR-20 Mag");
    AddItem("PreciseAutomaticRifleGun_Mag_5rd", "MR-8P Magazin", "MR-8P Mag");
    AddItem("RapidFireAutomaticRifleGun_Mag_50rd", "MR-50A Magazin", "MR-50A Mag");
    AddItem("UltimateAutomaticRifleGun_Mag_30rd", "MR-30E Magazin", "MR-30E Mag");

    AddItem("OxygenBottle", "Sauerstoffflasche", "Oxygen Bottle");
    AddItem("HydrogenBottle", "Wasserstoffflasche", "Hydrogen Bottle");
    AddItem("Medkit", "MedKit", "Medkit");
    AddItem("Powerkit", "Powerkit", "Powerkit");
    AddItem("ClangCola", "Clang Kola", "Clang Cola");
    AddItem("CosmicCoffee", "Cosmic Coffee", "Cosmic Coffee");

    AddItem("WelderItem", "Schweißgerät", "Welder");
    AddItem("Welder2Item", "Verb. Schweißgerät", "Proficient Welder");
    AddItem("Welder3Item", "Prof. Schweißgerät", "Professional Welder");
    AddItem("Welder4Item", "Elite-Schweißgerät", "Elite Welder");

    AddItem("AngleGrinderItem", "Schleifgerät", "Grinder");
    AddItem("AngleGrinder2Item", "Verb. Schleifgerät", "Proficient Grinder");
    AddItem("AngleGrinder3Item", "Prof. Schleifgerät", "Professional Grinder");
    AddItem("AngleGrinder4Item", "Elite-Schleifgerät", "Elite Grinder");

    AddItem("HandDrillItem", "Handbohrer", "Hand Drill");
    AddItem("HandDrill2Item", "Verb. Handbohrer", "Proficient Drill");
    AddItem("HandDrill3Item", "Prof. Handbohrer", "Professional Drill");
    AddItem("HandDrill4Item", "Elite-Handbohrer", "Elite Drill");

    AddItem("Datapad", "Datapad", "Datapad");
    AddItem("Package", "Paket", "Package");
}

void AddItem(string id, string de, string en)
{
    _namesDE[id] = de;
    _namesEN[id] = en;
}

// =========================================================
// RAFFINERIE RATE LISTE
// =========================================================

void InitOreRates()
{
    _oreRates["Cobalt"] = 1170;
    _oreRates["Gold"] = 11700;
    _oreRates["Iron"] = 93600;
    _oreRates["Magnesium"] = 4680;
    _oreRates["Nickel"] = 2340;
    _oreRates["Silicon"] = 7800;
    _oreRates["Silver"] = 4680;
    _oreRates["Stone"] = 46800;
    _oreRates["Platinum"] = 1170;
    _oreRates["Uranium"] = 1170;
    _oreRates["Scrap"] = 117000;
}

// =========================================================
// BLOCKS FINDEN
// =========================================================

void FindAssemblers()
{
    _assemblers.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(
        _assemblers,
        a => a.CubeGrid == Me.CubeGrid && a.IsFunctional
    );
}

void FindRefineries()
{
    _refineries.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyRefinery>(
        _refineries,
        r => r.CubeGrid == Me.CubeGrid && r.IsFunctional && r.Enabled
    );
}

void FindLinkedLCDs(string marker, List<IMyTextPanel> target, float fontSize, Color color, int padding)
{
    target.Clear();

    List<IMyTextPanel> lcds = new List<IMyTextPanel>();

    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(
        lcds,
        l => l.CubeGrid == Me.CubeGrid && l.CustomName.Contains(marker)
    );

    lcds.Sort((a, b) => GetLinkNumber(a.CustomName, marker).CompareTo(GetLinkNumber(b.CustomName, marker)));

    for (int i = 0; i < lcds.Count; i++)
    {
        ConfigureLCD(lcds[i], fontSize, color, padding);
        target.Add(lcds[i]);
    }
}

void BuildPageLCDList(IMyTextPanel main, List<IMyTextPanel> linked, List<IMyTextPanel> pages)
{
    pages.Clear();

    if (main != null)
        pages.Add(main);

    for (int i = 0; i < linked.Count; i++)
    {
        if (linked[i] != main)
            pages.Add(linked[i]);
    }
}

int GetLinkNumber(string name, string marker)
{
    int index = name.IndexOf(marker);

    if (index < 0)
        return 9999;

    string number = name.Substring(index + marker.Length).Trim();

    int result;

    if (int.TryParse(number, out result))
        return result;

    return 9999;
}

// =========================================================
// MONTAGE SCAN
// =========================================================

void ScanAssemblerData()
{
    _asmOutputItems.Clear();
    _asmQueuedItems.Clear();

    for (int i = 0; i < _assemblers.Count; i++)
    {
        ScanQueue(_assemblers[i]);
        ScanOutputInventory(_assemblers[i]);
    }
}

void ScanQueue(IMyAssembler assembler)
{
    _queue.Clear();
    assembler.GetQueue(_queue);

    for (int i = 0; i < _queue.Count; i++)
    {
        MyProductionItem item = _queue[i];

        string name = CleanBlueprintName(item.BlueprintId.ToString());
        double amount = (double)item.Amount;

        if (!_asmQueuedItems.ContainsKey(name))
            _asmQueuedItems[name] = 0;

        _asmQueuedItems[name] += amount;
    }
}

void ScanOutputInventory(IMyAssembler assembler)
{
    IMyInventory output = assembler.GetInventory(1);

    _items.Clear();
    output.GetItems(_items);

    for (int i = 0; i < _items.Count; i++)
    {
        MyInventoryItem item = _items[i];

        string name = item.Type.SubtypeId;

        if (!IsKnownAssemblyItem(name))
            continue;

        double amount = (double)item.Amount;

        if (!_asmOutputItems.ContainsKey(name))
            _asmOutputItems[name] = 0;

        _asmOutputItems[name] += amount;
    }
}

bool IsKnownAssemblyItem(string subtype)
{
    foreach (var item in _namesDE)
    {
        if (subtype.Contains(item.Key))
            return true;
    }

    return false;
}

// =========================================================
// RAFFINERIE SCAN
// =========================================================

void ScanAllOres()
{
    _oreAmounts.Clear();
    _blocks.Clear();

    GridTerminalSystem.GetBlocks(_blocks);

    _blocks.RemoveAll(b =>
        b.CubeGrid != Me.CubeGrid ||
        !b.HasInventory
    );

    for (int b = 0; b < _blocks.Count; b++)
    {
        IMyTerminalBlock block = _blocks[b];

        for (int i = 0; i < block.InventoryCount; i++)
        {
            IMyInventory inv = block.GetInventory(i);

            _items.Clear();
            inv.GetItems(_items);

            for (int x = 0; x < _items.Count; x++)
            {
                MyInventoryItem item = _items[x];

                if (!item.Type.TypeId.EndsWith("_Ore"))
                    continue;

                string ore = item.Type.SubtypeId;
                double amountKg = (double)item.Amount;

                if (!_oreAmounts.ContainsKey(ore))
                    _oreAmounts[ore] = 0;

                _oreAmounts[ore] += amountKg;
            }
        }
    }
}

// =========================================================
// MONTAGE LCD ÜBERSICHT
// =========================================================

void WriteAssemblyOverviewLCD()
{
    List<string> lines = new List<string>();

    int working = 0;
    int enabled = 0;
    int queueJobs = 0;

    for (int i = 0; i < _assemblers.Count; i++)
    {
        if (_assemblers[i].Enabled)
            enabled++;

        if (_assemblers[i].IsProducing)
            working++;

        _queue.Clear();
        _assemblers[i].GetQueue(_queue);
        queueJobs += _queue.Count;
    }

    lines.Add("╔════════════════════════════════════╗");
    lines.Add("║        ◆ " + PadRightSmart(T("MONTAGE ZENTRALE", "ASSEMBLY CENTER"), 26) + "║");
    lines.Add("╠════════════════════════════════════╣");
    lines.Add("║ " + PadRightSmart(T("Status", "Status") + " : " + (working > 0 ? T("PRODUKTION", "PRODUCTION") : T("BEREIT", "READY")), 35) + "║");
    lines.Add("║ " + PadRightSmart(T("Anlagen", "Units") + ": " + _assemblers.Count + " | " + T("Aktiv", "Active") + " " + enabled + " | Work " + working, 35) + "║");
    lines.Add("║ " + PadRightSmart("Queue  : " + queueJobs + " Jobs | LCDs " + _asmPageLcds.Count, 35) + "║");
    lines.Add("╚════════════════════════════════════╝");

    lines.Add("╔════════════════════════════════════╗");
    lines.Add("║ " + PadRightSmart(T("ANLAGEN", "UNITS"), 35) + "║");
    lines.Add("╠══════════════════╦════════╦════════╣");

    for (int i = 0; i < _assemblers.Count; i++)
    {
        IMyAssembler a = _assemblers[i];

        _queue.Clear();
        a.GetQueue(_queue);

        lines.Add(
            "║ " +
            PadRightSmart(ShortName(a.CustomName, 16), 16) +
            " ║ " +
            PadRightSmart(GetAssemblerStatus(a), 6) +
            " ║ " +
            PadLeftSmart(_queue.Count + "J", 6) +
            " ║"
        );
    }

    lines.Add("╚══════════════════╩════════╩════════╝");

    lines.Add("╔════════════════════════════════════╗");
    lines.Add("║ " + PadRightSmart(T("PRODUKTIONSQUEUE", "PRODUCTION QUEUE"), 35) + "║");
    lines.Add("╠════════════════════════╦═══════════╣");

    if (_asmQueuedItems.Count == 0)
    {
        lines.Add("║ " + PadRightSmart(T("Keine Produktion eingereiht.", "No production queued."), 35) + "║");
    }
    else
    {
        List<KeyValuePair<string, double>> queueList = SortByAmount(_asmQueuedItems);

        for (int i = 0; i < queueList.Count; i++)
        {
            lines.Add(
                "║ " +
                PadRightSmart(ShortItem(queueList[i].Key), 22) +
                " ║ " +
                PadLeftSmart(FormatAmount(queueList[i].Value), 9) +
                " ║"
            );
        }
    }

    lines.Add("╚════════════════════════╩═══════════╝");

    lines.Add("╔════════════════════════════════════╗");
    lines.Add("║ " + PadRightSmart(T("OUTPUT-LAGER", "OUTPUT STORAGE"), 35) + "║");
    lines.Add("╠════════════════════════╦═══════════╣");

    if (_asmOutputItems.Count == 0)
    {
        lines.Add("║ " + PadRightSmart(T("Kein Output vorhanden.", "No output available."), 35) + "║");
    }
    else
    {
        List<KeyValuePair<string, double>> outputList = SortByAmount(_asmOutputItems);

        for (int i = 0; i < outputList.Count; i++)
        {
            lines.Add(
                "║ " +
                PadRightSmart(ShortItem(outputList[i].Key), 22) +
                " ║ " +
                PadLeftSmart(FormatAmount(outputList[i].Value), 9) +
                " ║"
            );
        }
    }

    lines.Add("╚════════════════════════╩═══════════╝");

    WritePagedLCDs(_asmPageLcds, lines, ASM_LINES_PER_LCD);
}

// =========================================================
// RAFFINERIE LCD ÜBERSICHT
// =========================================================

void WriteRefineryOverviewLCD()
{
    List<string> lines = new List<string>();

    double totalTimeHours = 0;
    double totalOreKg = 0;

    foreach (var ore in _oreAmounts)
    {
        totalOreKg += ore.Value;

        if (ore.Key == "Ice")
            continue;

        double rate = GetTotalRefineryRateForOre(ore.Key);
        if (rate > 0)
            totalTimeHours += ore.Value / rate;
    }

    lines.Add("╔══════════════════════════════════════════╗");
    lines.Add("║            ◆ " + PadRightSmart(T("RAFFINERIE ZEIT", "REFINERY TIME"), 28) + "║");
    lines.Add("╠══════════════════════════════════════════╣");
    lines.Add("║ " + PadRightSmart(T("Status", "Status") + "       " + (_refineries.Count > 0 ? T("ONLINE", "ONLINE") : T("OFFLINE", "OFFLINE")), 40) + " ║");
    lines.Add("║ " + PadRightSmart(T("Raffinerien", "Refineries") + "  " + _refineries.Count, 40) + " ║");
    lines.Add("║ " + PadRightSmart(T("Erzarten", "Ore types") + "     " + _oreAmounts.Count, 40) + " ║");
    lines.Add("║ " + PadRightSmart(T("Gesamtmenge", "Total amount") + "  " + FormatKg(totalOreKg) + " kg", 40) + " ║");
    lines.Add("╚══════════════════════════════════════════╝");

    if (_refineries.Count == 0)
    {
        lines.Add("╔══════════════════════════════════════════╗");
        lines.Add("║ " + PadRightSmart(T("WARNUNG", "WARNING"), 40) + " ║");
        lines.Add("║ " + PadRightSmart(T("Keine aktive Raffinerie gefunden.", "No active refinery found."), 40) + " ║");
        lines.Add("╚══════════════════════════════════════════╝");

        WritePagedLCDs(_refPageLcds, lines, REF_LINES_PER_LCD);
        return;
    }

    if (_oreAmounts.Count == 0)
    {
        lines.Add("╔══════════════════════════════════════════╗");
        lines.Add("║ " + PadRightSmart(T("Kein Erz im System gefunden.", "No ore found in system."), 40) + " ║");
        lines.Add("╚══════════════════════════════════════════╝");

        WritePagedLCDs(_refPageLcds, lines, REF_LINES_PER_LCD);
        return;
    }

    lines.Add("╔══════════════╦════════════╦══════════════╗");
    lines.Add("║ " + PadRightSmart(T("ERZ", "ORE"), 12) + " ║ " + PadRightSmart(T("MENGE", "AMOUNT"), 10) + " ║ " + PadRightSmart(T("ZEIT", "TIME"), 12) + " ║");
    lines.Add("╠══════════════╬════════════╬══════════════╣");

    List<KeyValuePair<string, double>> ores = SortByAmount(_oreAmounts);

    for (int i = 0; i < ores.Count; i++)
    {
        string oreName = ores[i].Key;
        double amount = ores[i].Value;
        string timeText;

        if (oreName == "Ice")
        {
            timeText = "H2/O2";
        }
        else
        {
            double totalRate = GetTotalRefineryRateForOre(oreName);
            timeText = totalRate > 0 ? GetTimeString(amount / totalRate) : T("keine Rate", "no rate");
        }

        lines.Add(
            "║ " +
            PadRightSmart(GetOreIcon(oreName) + " " + GetOreName(oreName), 12) +
            " ║ " +
            PadLeftSmart(FormatShortKg(amount), 10) +
            " ║ " +
            PadLeftSmart(timeText, 12) +
            " ║"
        );
    }

    lines.Add("╚══════════════╩════════════╩══════════════╝");

    lines.Add("╔══════════════════════════════════════════╗");
    lines.Add("║ " + PadRightSmart(T("GESAMTZEIT", "TOTAL TIME"), 40) + " ║");
    lines.Add("║ " + PadRightSmart(GetTimeString(totalTimeHours), 40) + " ║");
    lines.Add("║ " + PadRightSmart(BuildProgressBar(totalTimeHours), 40) + " ║");
    lines.Add("╚══════════════════════════════════════════╝");

    WritePagedLCDs(_refPageLcds, lines, REF_LINES_PER_LCD);
}

// =========================================================
// EINZEL LCDs
// =========================================================

void WriteAssemblerLCDs()
{
    for (int i = 0; i < _assemblers.Count; i++)
    {
        IMyAssembler assembler = _assemblers[i];

        string lcdName = ASM_LCD_PREFIX + assembler.CustomName;
        IMyTextPanel lcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextPanel;

        if (lcd == null)
            continue;

        ConfigureLCD(lcd, ASM_LCD_FONT_SIZE, new Color(0, 180, 255), 1);

        StringBuilder sb = new StringBuilder();

        _queue.Clear();
        assembler.GetQueue(_queue);

        sb.AppendLine("╔════════════════════════════════════╗");
        sb.AppendLine("║ " + PadRightSmart(T("MONTAGEANLAGE", "ASSEMBLER"), 35) + "║");
        sb.AppendLine("╠════════════════════════════════════╣");
        sb.AppendLine("║ " + PadRightSmart(T("Name", "Name") + "  : " + assembler.CustomName, 35) + "║");
        sb.AppendLine("║ " + PadRightSmart(T("Status", "Status") + ": " + GetAssemblerStatus(assembler), 35) + "║");
        sb.AppendLine("║ " + PadRightSmart(T("Modus", "Mode") + " : " + assembler.Mode.ToString(), 35) + "║");
        sb.AppendLine("║ " + PadRightSmart("Queue : " + _queue.Count + " Jobs", 35) + "║");
        sb.AppendLine("╚════════════════════════════════════╝");

        sb.AppendLine("╔════════════════════════╦═══════════╗");
        sb.AppendLine("║ " + PadRightSmart(T("PRODUKTION", "PRODUCTION"), 22) + " ║ " + PadRightSmart(T("MENGE", "AMOUNT"), 9) + " ║");
        sb.AppendLine("╠════════════════════════╬═══════════╣");

        if (_queue.Count == 0)
        {
            sb.AppendLine("║ " + PadRightSmart(T("Keine Produktion.", "No production."), 35) + "║");
        }
        else
        {
            int maxShown = Math.Min(_queue.Count, 12);

            for (int q = 0; q < maxShown; q++)
            {
                MyProductionItem item = _queue[q];

                string name = CleanBlueprintName(item.BlueprintId.ToString());
                double amount = (double)item.Amount;

                sb.AppendLine(
                    "║ " +
                    PadRightSmart(ShortItem(name), 22) +
                    " ║ " +
                    PadLeftSmart(FormatAmount(amount), 9) +
                    " ║"
                );
            }
        }

        sb.AppendLine("╚════════════════════════╩═══════════╝");

        sb.AppendLine("╔════════════════════════╦═══════════╗");
        sb.AppendLine("║ " + PadRightSmart("OUTPUT", 22) + " ║ " + PadRightSmart(T("MENGE", "AMOUNT"), 9) + " ║");
        sb.AppendLine("╠════════════════════════╬═══════════╣");

        IMyInventory output = assembler.GetInventory(1);

        _items.Clear();
        output.GetItems(_items);

        int shown = 0;

        for (int x = 0; x < _items.Count; x++)
        {
            MyInventoryItem item = _items[x];

            string name = item.Type.SubtypeId;

            if (!IsKnownAssemblyItem(name))
                continue;

            double amount = (double)item.Amount;

            sb.AppendLine(
                "║ " +
                PadRightSmart(ShortItem(name), 22) +
                " ║ " +
                PadLeftSmart(FormatAmount(amount), 9) +
                " ║"
            );

            shown++;

            if (shown >= 10)
                break;
        }

        if (shown == 0)
            sb.AppendLine("║ " + PadRightSmart(T("Leer.", "Empty."), 35) + "║");

        sb.AppendLine("╚════════════════════════╩═══════════╝");

        lcd.WriteText(sb.ToString());
    }
}

void WriteRefineryLCDs()
{
    for (int i = 0; i < _refineries.Count; i++)
    {
        IMyRefinery refinery = _refineries[i];

        string lcdName = REF_LCD_PREFIX + refinery.CustomName;
        IMyTextPanel lcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextPanel;

        if (lcd == null)
            continue;

        ConfigureLCD(lcd, REF_LCD_FONT_SIZE, new Color(0, 255, 120), 2);

        StringBuilder sb = new StringBuilder();

        double productivity = GetProductivityMultiplier(refinery) * 100.0;

        sb.AppendLine("╔══════════════════════════════════════════╗");
        sb.AppendLine("║             ◆ " + PadRightSmart(T("RAFFINERIE", "REFINERY"), 24) + "║");
        sb.AppendLine("╠══════════════════════════════════════════╣");
        sb.AppendLine("║ " + PadRightSmart(T("Name", "Name") + "         " + refinery.CustomName, 40) + " ║");
        sb.AppendLine("║ " + PadRightSmart(T("Status", "Status") + "       " + (refinery.IsProducing ? T("ARBEITET", "WORKING") : T("BEREIT", "READY")), 40) + " ║");
        sb.AppendLine("║ " + PadRightSmart(T("Produktiv.", "Productivity") + "   " + productivity.ToString("0") + "%", 40) + " ║");
        sb.AppendLine("╚══════════════════════════════════════════╝");

        sb.AppendLine("╔══════════════╦════════════╦══════════════╗");
        sb.AppendLine("║ " + PadRightSmart(T("ERZ", "ORE"), 12) + " ║ " + PadRightSmart(T("MENGE", "AMOUNT"), 10) + " ║ " + PadRightSmart(T("ZEIT", "TIME"), 12) + " ║");
        sb.AppendLine("╠══════════════╬════════════╬══════════════╣");

        IMyInventory inv = refinery.GetInventory(0);

        _items.Clear();
        inv.GetItems(_items);

        bool foundOre = false;

        for (int x = 0; x < _items.Count; x++)
        {
            MyInventoryItem item = _items[x];

            if (!item.Type.TypeId.EndsWith("_Ore"))
                continue;

            foundOre = true;

            string ore = item.Type.SubtypeId;
            double amountKg = (double)item.Amount;

            double rate = GetRefineryRateForOre(refinery, ore);
            string time = rate > 0 ? GetTimeString(amountKg / rate) : T("keine Rate", "no rate");

            sb.AppendLine(
                "║ " +
                PadRightSmart(GetOreIcon(ore) + " " + GetOreName(ore), 12) +
                " ║ " +
                PadLeftSmart(FormatShortKg(amountKg), 10) +
                " ║ " +
                PadLeftSmart(time, 12) +
                " ║"
            );
        }

        if (!foundOre)
            sb.AppendLine("║ " + PadRightSmart(T("Kein Erz in dieser Raffinerie.", "No ore in this refinery."), 40) + " ║");

        sb.AppendLine("╚══════════════╩════════════╩══════════════╝");

        lcd.WriteText(sb.ToString());
    }
}

// =========================================================
// RAFFINERIE BERECHNUNG
// =========================================================

double GetTotalRefineryRateForOre(string ore)
{
    double total = 0;

    for (int i = 0; i < _refineries.Count; i++)
        total += GetRefineryRateForOre(_refineries[i], ore);

    return total;
}

double GetRefineryRateForOre(IMyRefinery refinery, string ore)
{
    if (!_oreRates.ContainsKey(ore))
        return 0;

    double baseRate = _oreRates[ore];
    double productivity = GetProductivityMultiplier(refinery);

    return baseRate * SERVER_REFINERY_SPEED * productivity;
}

double GetProductivityMultiplier(IMyRefinery refinery)
{
    string info = refinery.DetailedInfo;
    string[] lines = info.Split('\n');

    for (int i = 0; i < lines.Length; i++)
    {
        string line = lines[i];

        if (!line.Contains("Productivity") &&
            !line.Contains("Produktivität") &&
            !line.Contains("Продуктивность"))
            continue;

        int colon = line.IndexOf(":");
        int percent = line.IndexOf("%");

        if (colon < 0 || percent < 0 || percent <= colon)
            continue;

        string valueText = line.Substring(colon + 1, percent - colon - 1).Trim();

        double percentValue;
        if (double.TryParse(valueText, out percentValue))
            return Math.Max(0.01, percentValue / 100.0);
    }

    return 1.0;
}

// =========================================================
// STATUS / FORMAT
// =========================================================

string GetAssemblerStatus(IMyAssembler assembler)
{
    if (!assembler.Enabled)
        return T("AUS", "OFF");

    if (!assembler.IsFunctional)
        return T("DEFEKT", "BROKEN");

    if (assembler.IsProducing)
        return T("ARBEIT", "WORK");

    _queue.Clear();
    assembler.GetQueue(_queue);

    if (_queue.Count > 0)
        return T("WARTET", "WAIT");

    return T("BEREIT", "READY");
}

void ConfigureLCD(IMyTextPanel lcd, float fontSize, Color color, int padding)
{
    lcd.ContentType = ContentType.TEXT_AND_IMAGE;
    lcd.Font = "Monospace";
    lcd.FontSize = fontSize;
    lcd.TextPadding = padding;
    lcd.FontColor = color;
    lcd.BackgroundColor = new Color(0, 0, 0);
}

void WritePagedLCDs(List<IMyTextPanel> lcds, List<string> lines, int linesPerLcd)
{
    if (lcds.Count == 0)
        return;

    for (int lcdIndex = 0; lcdIndex < lcds.Count; lcdIndex++)
    {
        StringBuilder page = new StringBuilder();

        int start = lcdIndex * linesPerLcd;
        int end = Math.Min(start + linesPerLcd, lines.Count);

        if (start < lines.Count)
        {
            for (int i = start; i < end; i++)
                page.AppendLine(lines[i]);
        }

        lcds[lcdIndex].WriteText(page.ToString());
    }
}

List<KeyValuePair<string, double>> SortByAmount(Dictionary<string, double> dict)
{
    List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();

    foreach (var item in dict)
        list.Add(item);

    list.Sort((a, b) => b.Value.CompareTo(a.Value));

    return list;
}

string CleanBlueprintName(string blueprint)
{
    blueprint = blueprint.Replace("MyObjectBuilder_BlueprintDefinition/", "");

    if (blueprint.EndsWith("Blueprint"))
        blueprint = blueprint.Substring(0, blueprint.Length - 9);

    return blueprint;
}

string ShortItem(string name)
{
    foreach (var item in _namesDE)
    {
        if (name.Contains(item.Key))
        {
            string display = LANGUAGE == "EN" ? _namesEN[item.Key] : _namesDE[item.Key];
            return ShortName(display, 22);
        }
    }

    string clean = name.Replace("Component", "");
    return ShortName(clean, 22);
}

string GetOreName(string ore)
{
    if (LANGUAGE == "EN")
        return ore;

    switch (ore)
    {
        case "Iron": return "Eisen";
        case "Nickel": return "Nickel";
        case "Cobalt": return "Kobalt";
        case "Silicon": return "Silizium";
        case "Silver": return "Silber";
        case "Gold": return "Gold";
        case "Platinum": return "Platin";
        case "Uranium": return "Uran";
        case "Magnesium": return "Magnesium";
        case "Stone": return "Stein";
        case "Scrap": return "Schrott";
        case "Ice": return "Eis";
        default: return ore;
    }
}

string GetOreIcon(string ore)
{
    switch (ore)
    {
        case "Iron": return "Fe";
        case "Nickel": return "Ni";
        case "Cobalt": return "Co";
        case "Silicon": return "Si";
        case "Silver": return "Ag";
        case "Gold": return "Au";
        case "Platinum": return "Pt";
        case "Uranium": return "Ur";
        case "Magnesium": return "Mg";
        case "Stone": return "St";
        case "Scrap": return "Sc";
        case "Ice": return "Ic";
        default: return "??";
    }
}

string BuildProgressBar(double hours)
{
    double value = Math.Min(hours / 24.0, 1.0);
    int filled = (int)Math.Round(value * BAR_WIDTH);

    string bar = "";

    for (int i = 0; i < BAR_WIDTH; i++)
        bar += i < filled ? "█" : "░";

    return "[" + bar + "] " + Math.Min(hours, 24).ToString("0.0") + "h";
}

string GetTimeString(double hours)
{
    if (double.IsNaN(hours) || double.IsInfinity(hours) || hours < 0)
        return T("Fehler", "Error");

    TimeSpan time = TimeSpan.FromHours(hours);

    return time.Days.ToString("00") + "d:" +
           time.Hours.ToString("00") + "h:" +
           time.Minutes.ToString("00") + "m";
}

string T(string de, string en)
{
    return LANGUAGE == "EN" ? en : de;
}

string FormatAmount(double amount)
{
    if (amount >= 1000000)
        return (amount / 1000000.0).ToString("0.0") + "M";

    if (amount >= 1000)
        return (amount / 1000.0).ToString("0.0") + "k";

    return amount.ToString("0");
}

string FormatKg(double kg)
{
    return kg.ToString("N0");
}

string FormatShortKg(double kg)
{
    if (kg >= 1000000)
        return (kg / 1000000.0).ToString("0.0") + "M";

    if (kg >= 1000)
        return (kg / 1000.0).ToString("0.0") + "k";

    return kg.ToString("0");
}

string ShortName(string text, int max)
{
    if (text.Length <= max)
        return text;

    return text.Substring(0, max);
}

string PadRightSmart(string text, int width)
{
    if (text.Length > width)
        return text.Substring(0, width);

    return text.PadRight(width);
}

string PadLeftSmart(string text, int width)
{
    if (text.Length > width)
        return text.Substring(0, width);

    return text.PadLeft(width);
}