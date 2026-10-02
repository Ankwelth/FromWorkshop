const string LCD_KEYWORD = "Monitoring";

// Performance settings
// SCAN_INTERVAL_RUNS = after how many script runs the grid inventory is scanned again.
// DISPLAYS_PER_RUN = how many LCDs/cockpit surfaces are rendered per run.
const int SCAN_INTERVAL_RUNS = 6;
const int DISPLAYS_PER_RUN = 1;
const bool DEBUG_ECHO = true;

Color BG = new Color(56, 59, 67);
Color TEXT = Color.White;
Color LINE = new Color(95, 100, 112);
Color HEADER = new Color(35, 37, 43);
Color TILE = new Color(46, 49, 57);

Dictionary<long, string> customDataCache = new Dictionary<long, string>();
Dictionary<long, MonitorConfig> configCache = new Dictionary<long, MonitorConfig>();

Dictionary<string, double> cachedItems = new Dictionary<string, double>();
List<IMyTerminalBlock> cachedMonitorBlocks = new List<IMyTerminalBlock>();

int runsSinceScan = 999999;
int nextDisplayIndex = 0;
int lastItemCount = 0;
int lastLcdCount = 0;
int lastRenderedCount = 0;
bool hasScannedOnce = false;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    if (argument == "rescan" || argument == "refresh")
        runsSinceScan = 999999;

    if (!hasScannedOnce || runsSinceScan >= SCAN_INTERVAL_RUNS)
    {
        cachedItems = ScanGridItems();
        RefreshMonitorBlocks();

        hasScannedOnce = true;
        runsSinceScan = 0;
        lastItemCount = cachedItems.Count;
        lastLcdCount = cachedMonitorBlocks.Count;

        EchoDebug("SCAN");
        return;
    }

    runsSinceScan++;
    lastRenderedCount = DrawNextDisplays();

    EchoDebug("DRAW");
}

void RefreshMonitorBlocks()
{
    cachedMonitorBlocks.Clear();
    GridTerminalSystem.GetBlocksOfType(cachedMonitorBlocks, b => b.CustomName.Contains(LCD_KEYWORD));

    if (nextDisplayIndex >= cachedMonitorBlocks.Count)
        nextDisplayIndex = 0;
}

int DrawNextDisplays()
{
    if (cachedMonitorBlocks.Count == 0)
        return 0;

    int rendered = 0;
    int max = DISPLAYS_PER_RUN;
    if (max < 1) max = 1;

    for (int i = 0; i < max; i++)
    {
        if (cachedMonitorBlocks.Count == 0)
            break;

        if (nextDisplayIndex >= cachedMonitorBlocks.Count)
            nextDisplayIndex = 0;

        IMyTerminalBlock block = cachedMonitorBlocks[nextDisplayIndex];
        nextDisplayIndex++;

        EnsureCustomData(block);

        MonitorConfig cfg = GetCachedConfig(block);
        IMyTextSurface surface = GetSurface(block, cfg.Surface);

        if (surface == null)
            continue;

        Dictionary<string, double> filtered = FilterItems(cachedItems, cfg);
        DrawLCD(surface, filtered, cfg);
        rendered++;
    }

    return rendered;
}

void EchoDebug(string phase)
{
    if (!DEBUG_ECHO)
        return;
    double usage =
    Runtime.CurrentInstructionCount * 100.0 /
    Runtime.MaxInstructionCount;
    Echo("Phase: " + phase);
    Echo("Instr: " + Runtime.CurrentInstructionCount + " / " + Runtime.MaxInstructionCount);
    Echo("Time: " + Runtime.LastRunTimeMs.ToString("0.000") + " ms");
    Echo("Usage: " + usage.ToString("0.00") + "%");
    Echo("Items found: " + lastItemCount);
    Echo("Displays: " + lastLcdCount);
    Echo("Rendered: " + lastRenderedCount + " / run");
    Echo("Next Display: " + nextDisplayIndex);
}

MonitorConfig GetCachedConfig(IMyTerminalBlock block)
{
    long id = block.EntityId;
    string data = block.CustomData;

    if (configCache.ContainsKey(id) &&
        customDataCache.ContainsKey(id) &&
        customDataCache[id] == data)
    {
        return configCache[id];
    }

    MonitorConfig cfg = LoadConfig(block);

    customDataCache[id] = data;
    configCache[id] = cfg;

    return cfg;
}

class MonitorConfig
{
    public string Title = "STORAGE MONITOR";
    public string Layout = "Grid";
    public int Columns = 3;
    public float TileHeight = 115;
    public bool ShowNamesInGrid = true;
    public int Surface = 0;

    public bool ShowIngots = true;
    public bool ShowOres = false;
    public bool ShowIce = true;
    public bool ShowComponents = false;
    public bool ShowTools = false;
    public bool ShowAmmo = false;
    public bool ShowBottles = false;
    public bool ShowConsumables = false;

    public bool Warning = true;
    public double DefaultWarningAmount = 5000;
    public bool SortByAmount = true;

    public Dictionary<string, double> Thresholds = new Dictionary<string, double>();
}

Dictionary<string, double> ScanGridItems()
{
    Dictionary<string, double> result = new Dictionary<string, double>();

    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => b.HasInventory && b.CubeGrid == Me.CubeGrid);

    foreach (IMyTerminalBlock block in blocks)
    {
        for (int i = 0; i < block.InventoryCount; i++)
        {
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            block.GetInventory(i).GetItems(items);

            foreach (MyInventoryItem item in items)
            {
                string key = item.Type.TypeId + "/" + item.Type.SubtypeId;

                if (!result.ContainsKey(key))
                    result[key] = 0;

                result[key] += (double)item.Amount;
            }
        }
    }

    return result;
}

void EnsureCustomData(IMyTextPanel lcd)
{
    if (!string.IsNullOrWhiteSpace(lcd.CustomData))
        return;

    lcd.CustomData =
@"[Monitor]
Title=STORAGE MONITOR
Layout=Grid
Columns=3
TileHeight=115
ShowNamesInGrid=True
Surface=0

ShowIngots=True
ShowOres=False
ShowIce=True
ShowComponents=False
ShowTools=False
ShowAmmo=False
ShowBottles=False
ShowConsumables=False

Warning=True
DefaultWarningAmount=5000
SortByAmount=True

[Thresholds]
Uranium=5000
Platinum=2000
Gold=10000
Ice=100000
SteelPlate=5000
Computer=500
Motor=1000";
}

IMyTextSurface GetSurface(IMyTerminalBlock block, int index)
{
    IMyTextPanel panel = block as IMyTextPanel;

    if (panel != null)
        return panel;

    IMyTextSurfaceProvider provider = block as IMyTextSurfaceProvider;

    if (provider == null)
        return null;

    if (index < 0)
        index = 0;

    if (index >= provider.SurfaceCount)
        index = provider.SurfaceCount - 1;

    return provider.GetSurface(index);
}

void EnsureCustomData(IMyTerminalBlock block)
{
    if (!string.IsNullOrWhiteSpace(block.CustomData))
        return;

    block.CustomData =
@"[Monitor]
Title=STORAGE MONITOR
Layout=Grid
Columns=3
TileHeight=115
Surface=0
ShowNamesInGrid=True

ShowIngots=True
ShowOres=False
ShowIce=True
ShowComponents=False
ShowTools=False
ShowAmmo=False
ShowBottles=False
ShowConsumables=False

Warning=True
DefaultWarningAmount=5000
SortByAmount=True

[Thresholds]
Uranium=5000
Platinum=2000
Gold=10000
Ice=100000
SteelPlate=5000
Computer=500
Motor=1000";
}

MonitorConfig LoadConfig(IMyTerminalBlock lcd)
{
    MonitorConfig cfg = new MonitorConfig();
    string section = "";

    string[] lines = lcd.CustomData.Split('\n');

    foreach (string rawLine in lines)
    {
        string line = rawLine.Trim();

        if (line == "" || line.StartsWith(";") || line.StartsWith("#"))
            continue;

        if (line.StartsWith("[") && line.EndsWith("]"))
        {
            section = line.Substring(1, line.Length - 2);
            continue;
        }

        int idx = line.IndexOf('=');
        if (idx < 0)
            continue;

        string key = line.Substring(0, idx).Trim();
        string value = line.Substring(idx + 1).Trim();

        if (section == "Monitor")
        {
            if (key == "Title") cfg.Title = value;
            else if (key == "Layout") cfg.Layout = value;
            else if (key == "Columns") cfg.Columns = (int)ToDouble(value, cfg.Columns);
            else if (key == "TileHeight") cfg.TileHeight = (float)ToDouble(value, cfg.TileHeight);
            else if (key == "ShowNamesInGrid") cfg.ShowNamesInGrid = ToBool(value);
            else if (key == "Surface") cfg.Surface = (int)ToDouble(value, cfg.Surface);

            else if (key == "ShowIngots") cfg.ShowIngots = ToBool(value);
            else if (key == "ShowOres") cfg.ShowOres = ToBool(value);
            else if (key == "ShowIce") cfg.ShowIce = ToBool(value);
            else if (key == "ShowComponents") cfg.ShowComponents = ToBool(value);
            else if (key == "ShowTools") cfg.ShowTools = ToBool(value);
            else if (key == "ShowAmmo") cfg.ShowAmmo = ToBool(value);
            else if (key == "ShowBottles") cfg.ShowBottles = ToBool(value);
            else if (key == "ShowConsumables") cfg.ShowConsumables = ToBool(value);

            else if (key == "Warning") cfg.Warning = ToBool(value);
            else if (key == "DefaultWarningAmount") cfg.DefaultWarningAmount = ToDouble(value, cfg.DefaultWarningAmount);
            else if (key == "SortByAmount") cfg.SortByAmount = ToBool(value);
        }
        else if (section == "Thresholds")
        {
            cfg.Thresholds[key] = ToDouble(value, cfg.DefaultWarningAmount);
        }
    }

    if (cfg.Columns < 1) cfg.Columns = 1;
    if (cfg.TileHeight < 80) cfg.TileHeight = 80;

    return cfg;
}

Dictionary<string, double> FilterItems(Dictionary<string, double> allItems, MonitorConfig cfg)
{
    Dictionary<string, double> result = new Dictionary<string, double>();

    foreach (KeyValuePair<string, double> pair in allItems)
    {
        string typeId = GetTypeId(pair.Key);
        string subtype = GetSubtype(pair.Key);

        bool show = false;

        if (IsIngot(typeId) && cfg.ShowIngots) show = true;
        if (IsOre(typeId) && subtype != "Ice" && cfg.ShowOres) show = true;
        if (IsOre(typeId) && subtype == "Ice" && cfg.ShowIce) show = true;
        if (IsComponent(typeId) && cfg.ShowComponents) show = true;
        if (IsTool(typeId) && cfg.ShowTools) show = true;
        if (IsAmmo(typeId) && cfg.ShowAmmo) show = true;
        if (IsBottle(typeId, subtype) && cfg.ShowBottles) show = true;
        if (IsConsumable(typeId) && cfg.ShowConsumables) show = true;

        if (!show)
            continue;

        result[pair.Key] = pair.Value;
    }

    return result;
}

void DrawLCD(IMyTextSurface lcd, Dictionary<string, double> items, MonitorConfig cfg)
{
    lcd.ContentType = ContentType.SCRIPT;
    lcd.ScriptBackgroundColor = BG;

    Vector2 size = lcd.TextureSize;
    MySpriteDrawFrame frame = lcd.DrawFrame();

    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", size / 2f, size, BG));

    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
        new Vector2(size.X / 2f, 42),
        new Vector2(size.X, 84),
        HEADER));

    frame.Add(new MySprite(SpriteType.TEXTURE, "IconInventory",
        new Vector2(40, 42),
        new Vector2(42, 42),
        Color.White));

    AddText(frame, cfg.Title, new Vector2(78, 18), 1.05f, TEXT, TextAlignment.LEFT);
    AddLine(frame, 20, 92, size.X - 20);

    List<KeyValuePair<string, double>> sorted = new List<KeyValuePair<string, double>>(items);

    if (cfg.SortByAmount)
        sorted.Sort((a, b) => b.Value.CompareTo(a.Value));
    else
        sorted.Sort((a, b) => GetDisplayName(GetSubtype(a.Key)).CompareTo(GetDisplayName(GetSubtype(b.Key))));

    int shown = 0;

    if (cfg.Layout.ToLower() == "grid")
        shown = DrawGrid(frame, sorted, cfg, size);
    else
        shown = DrawList(frame, sorted, cfg, size);

    AddLine(frame, 20, size.Y - 48, size.X - 20);

    string gridName = Me.CubeGrid.CustomName;
    string gridType = Me.CubeGrid.IsStatic ? "[STATION]" : "[SHIP]";

    AddText(frame, gridName, new Vector2(25, size.Y - 36), 0.55f, TEXT, TextAlignment.LEFT);
    AddText(frame, gridType, new Vector2(size.X - 25, size.Y - 36), 0.55f, TEXT, TextAlignment.RIGHT);

    frame.Dispose();
}

int DrawList(MySpriteDrawFrame frame, List<KeyValuePair<string, double>> sorted, MonitorConfig cfg, Vector2 size)
{
    float y = 112;
    float rowHeight = 40;
    int shown = 0;

    if (sorted.Count == 0)
    {
        AddText(frame, "Keine passenden Items gefunden.", new Vector2(25, y), 0.75f, TEXT, TextAlignment.LEFT);
        return 0;
    }

    foreach (KeyValuePair<string, double> pair in sorted)
    {
        if (y > size.Y - 70)
            break;

        string typeId = GetTypeId(pair.Key);
        string subtype = GetSubtype(pair.Key);

        frame.Add(new MySprite(SpriteType.TEXTURE, GetIcon(typeId, subtype),
            new Vector2(42, y + 13),
            new Vector2(28, 28),
            Color.White));

        AddText(frame, GetDisplayName(subtype), new Vector2(78, y), 0.66f, GetItemColor(subtype), TextAlignment.LEFT);

        float amountX = cfg.Warning ? size.X - 55 : size.X - 28;
        AddText(frame, FormatAmount(pair.Value), new Vector2(amountX, y), 0.66f, TEXT, TextAlignment.RIGHT);

        if (cfg.Warning)
        {
            Color statusColor = pair.Value < GetThreshold(subtype, cfg) ? Color.Red : Color.Green;

            frame.Add(new MySprite(SpriteType.TEXTURE, "Circle",
                new Vector2(size.X - 25, y + 15),
                new Vector2(12, 12),
                statusColor));
        }

        AddLine(frame, 78, y + 34, size.X - 28);

        y += rowHeight;
        shown++;
    }

    return shown;
}

int DrawGrid(MySpriteDrawFrame frame, List<KeyValuePair<string, double>> sorted, MonitorConfig cfg, Vector2 size)
{
    float margin = 22;
    float gap = 10;
    float startY = 112;
    float footerY = size.Y - 55;

    int columns = cfg.Columns;
    float tileW = (size.X - margin * 2 - gap * (columns - 1)) / columns;
    float tileH = cfg.TileHeight;

    int shown = 0;

    if (sorted.Count == 0)
    {
        AddText(frame, "Keine passenden Items gefunden.", new Vector2(25, startY), 0.75f, TEXT, TextAlignment.LEFT);
        return 0;
    }

    foreach (KeyValuePair<string, double> pair in sorted)
    {
        int col = shown % columns;
        int row = shown / columns;

        float x = margin + col * (tileW + gap);
        float y = startY + row * (tileH + gap);

        if (y + tileH > footerY)
            break;

        string typeId = GetTypeId(pair.Key);
        string subtype = GetSubtype(pair.Key);
        string name = GetDisplayName(subtype);
        string amount = FormatAmount(pair.Value);

        frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
            new Vector2(x + tileW / 2f, y + tileH / 2f),
            new Vector2(tileW, tileH),
            TILE));

        if (cfg.ShowNamesInGrid)
{
    frame.Add(new MySprite(SpriteType.TEXTURE, GetIcon(typeId, subtype),
        new Vector2(x + tileW / 2f, y + 30),
        new Vector2(38, 38),
        Color.White));
}

        if (cfg.ShowNamesInGrid)
{
    DrawWrappedName(frame, name, x + 8, y + 55, tileW - 16, GetItemColor(subtype));

    AddText(frame, amount,
        new Vector2(x + tileW / 2f, y + tileH - 32),
        0.55f,
        TEXT,
        TextAlignment.CENTER);
}
else
{
    frame.Add(new MySprite(SpriteType.TEXTURE, GetIcon(typeId, subtype),
        new Vector2(x + tileW / 2f, y + 42),
        new Vector2(52, 52),
        Color.White));

    AddText(frame, amount,
        new Vector2(x + tileW / 2f, y + tileH - 30),
        0.6f,
        TEXT,
        TextAlignment.CENTER);
}

        if (cfg.Warning)
        {
            Color statusColor = pair.Value < GetThreshold(subtype, cfg) ? Color.Red : Color.Green;

            frame.Add(new MySprite(SpriteType.TEXTURE, "Circle",
                new Vector2(x + tileW - 16, y + 16),
                new Vector2(12, 12),
                statusColor));
        }

        shown++;
    }

    return shown;
}

void DrawWrappedName(MySpriteDrawFrame frame, string name, float x, float y, float width, Color color)
{
    int maxChars = (int)(width / 12f);
    if (maxChars < 4) maxChars = 4;

    if (name.Length <= maxChars)
    {
        AddText(frame, name, new Vector2(x + width / 2f, y), 0.48f, color, TextAlignment.CENTER);
        return;
    }

    string line1 = name.Substring(0, maxChars);
    string line2 = name.Substring(maxChars);

    if (line2.Length > maxChars)
        line2 = line2.Substring(0, maxChars - 2) + "..";

    AddText(frame, line1, new Vector2(x + width / 2f, y), 0.43f, color, TextAlignment.CENTER);
    AddText(frame, line2, new Vector2(x + width / 2f, y + 18), 0.43f, color, TextAlignment.CENTER);
}

double GetThreshold(string subtype, MonitorConfig cfg)
{
    if (cfg.Thresholds.ContainsKey(subtype))
        return cfg.Thresholds[subtype];

    return cfg.DefaultWarningAmount;
}

void AddText(MySpriteDrawFrame frame, string text, Vector2 pos, float scale, Color color, TextAlignment align)
{
    frame.Add(new MySprite(SpriteType.TEXT, text, pos, null, color, "Monospace", align, scale));
}

void AddLine(MySpriteDrawFrame frame, float x1, float y, float x2)
{
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
        new Vector2((x1 + x2) / 2f, y),
        new Vector2(x2 - x1, 2),
        LINE));
}

bool ToBool(string value)
{
    value = value.ToLower();
    return value == "true" || value == "1" || value == "yes" || value == "ja";
}

double ToDouble(string value, double fallback)
{
    double result;
    if (double.TryParse(value, out result))
        return result;

    return fallback;
}

string GetTypeId(string key)
{
    int idx = key.IndexOf('/');
    if (idx < 0) return "";
    return key.Substring(0, idx);
}

string GetSubtype(string key)
{
    int idx = key.IndexOf('/');
    if (idx < 0) return key;
    return key.Substring(idx + 1);
}

bool IsIngot(string typeId)
{
    return typeId.Contains("Ingot");
}

bool IsOre(string typeId)
{
    return typeId.Contains("Ore");
}

bool IsComponent(string typeId)
{
    return typeId.Contains("Component");
}

bool IsTool(string typeId)
{
    return typeId.Contains("PhysicalGunObject") || typeId.Contains("HandTool");
}

bool IsAmmo(string typeId)
{
    return typeId.Contains("AmmoMagazine");
}

bool IsBottle(string typeId, string subtype)
{
    return subtype.Contains("Bottle");
}

bool IsConsumable(string typeId)
{
    return typeId.Contains("ConsumableItem");
}

string GetIcon(string typeId, string subtype)
{
    if (IsOre(typeId)) return "MyObjectBuilder_Ore/" + subtype;
    if (IsIngot(typeId)) return "MyObjectBuilder_Ingot/" + subtype;
    if (IsComponent(typeId)) return "MyObjectBuilder_Component/" + subtype;
    if (IsAmmo(typeId)) return "MyObjectBuilder_AmmoMagazine/" + subtype;
    if (IsTool(typeId)) return "MyObjectBuilder_PhysicalGunObject/" + subtype;

    return "IconInventory";
}

Color GetItemColor(string subtype)
{
    switch (subtype)
    {
        case "Ice": return new Color(120, 220, 255);
        case "Uranium": return new Color(80, 255, 80);
        case "Gold": return new Color(255, 215, 0);
        case "Silver": return new Color(220, 220, 220);
        case "Cobalt": return new Color(80, 170, 255);
        case "Nickel": return new Color(180, 220, 255);
        case "Magnesium": return new Color(255, 140, 0);
        case "Silicon": return Color.White;
        case "Iron": return new Color(210, 210, 210);
        case "Stone": return new Color(170, 170, 170);
        default: return Color.White;
    }
}

string GetDisplayName(string subtype)
{
    if (subtype == "Silicon") return "Si";
    if (subtype == "Iron") return "Fe";
    if (subtype == "Cobalt") return "Co";
    if (subtype == "Nickel") return "Ni";
    if (subtype == "Magnesium") return "Mg";
    if (subtype == "Uranium") return "U";
    if (subtype == "Gold") return "Au";
    if (subtype == "Silver") return "Ag";
    if (subtype == "Platinum") return "Pt";
    if (subtype == "Titanium") return "Ti";
    if (subtype == "Aluminum") return "Al";
    if (subtype == "Copper") return "Cu";
    if (subtype == "Stone") return "Stein";
    if (subtype == "Ice") return "Eis";

    return subtype;
}

string FormatAmount(double amount)
{
    if (amount >= 1000000000)
        return (amount / 1000000000d).ToString("0.##") + " B";

    if (amount >= 1000000)
        return (amount / 1000000d).ToString("0.##") + " M";

    if (amount >= 1000)
        return (amount / 1000d).ToString("0.##") + " k";

    return amount.ToString("0");
}