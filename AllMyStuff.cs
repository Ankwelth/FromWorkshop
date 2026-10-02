// All My Stuff
// ------------
// A script to keep track of your inventory
// 
// Configure using CustomData. Put an Inventory section into any screen. Here are
// the defaults, all of which can be empty:
// 
// [inventory]
// delay=3         # Set this to change the update frequency.
// display=0      # Use this to control a display for output (they start at 0)
// scale = 1.0     # Make things on the display larger or smaller
// skip = 0         # Use this on a second screen if the lines don't all fit on the first, to skip lines
// color = 00FF55 # Use this to customise the header color of a screen
// mono = false     # change to true to display numbers in the monospace font
// suppress_zeros = false # Change to true to hide (red) lines showing zero stock
// enablefilter = true  # Enable filtering
// format_amount = true  # Abbreviates numbers to K, M, or G
// filter = ConsumableItem, Datapad, PhysicalGunObject, AmmoMagazine, Ore, Ingot, Component
// savetypes = true
// 
// An alternative, for those who wish to use more than one screen on a given
// block at once, is to configure displays in the following manner (this example
// works on the Sci-Fi Button Panel):
// 
// [inventory_display0]
// scale=0.4
// 
// [inventory_display1]
// scale=0.4
// skip=5
// 
// [inventory_display2]
// scale=0.4
// skip=10
// 
// [inventory_display3]
// scale=0.4
// skip=15
// 
// This script will try to "prettify" the names of the items automatically
// example "PrototechFrame" to "Prototech Frame", you can disable this
// feature by placing this into the Custom Data of the programable block:
// 
// [inventory]
// format_names = false
// 
// This is not possible to some items, like "Gravel", since the in-Game name is
// different than the item name "Ingots/Stone". If you would like to manually 
// translate item names, place this config section into Custom Data as well. 
// A full version is available on GitHub and the Workshop page:
// 
// [translation]
// ingots/stone=Gravel

class ManagedDisplay
{
    IMyTextSurface surface;
    RectangleF viewport;
    MySpriteDrawFrame frame;
    float StartHeight = 0f;
    float HeadingHeight = 35f;
    float LineHeight = 30f;
    float HeadingFontSize = 1.3f;
    float RegularFontSize = 1.0f;
    float finalColumnWidth;
    Vector2 Position, _padding;
    int WindowSize;         // Number of lines shown on screen at once after heading
    Color HighlightColor;
    int linesToSkip;
    bool monospace;
    bool MakeSpriteCacheDirty = false;
    string previousType;
    string Heading = "Item";
    bool unfiltered = true;
    bool SuppressZeros;
    public bool UseFormatedAmount { get; }
    bool separators = true;
    string Filter;
    Color BackgroundColor, ForegroundColor;

    public ManagedDisplay(IMyTextSurface surface, float scale = 1.0f, 
        Color highlightColor = new Color(), 
        int linesToSkip = 0, bool monospace = false, 
        bool suppressZeros=false, bool separators=true, bool useFormatedAmount = false)
    {
        this.surface = surface;
        this.HighlightColor = highlightColor;
        this.linesToSkip = linesToSkip;
        this.monospace = monospace;
        this.separators = separators;
        this.SuppressZeros = suppressZeros;
        this.UseFormatedAmount = useFormatedAmount;
        this.BackgroundColor = surface.ScriptBackgroundColor;
        this.ForegroundColor = surface.ScriptForegroundColor;

        // Scale everything!
        StartHeight *= scale;
        HeadingHeight *= scale;
        LineHeight *= scale;
        HeadingFontSize *= scale;
        RegularFontSize *= scale;

        surface.ContentType = ContentType.SCRIPT;
        surface.Script = "TSS_FactionIcon";
        _padding = surface.TextureSize * (surface.TextPadding / 100);
        viewport = new RectangleF((surface.TextureSize - surface.SurfaceSize) / 2f + _padding, surface.SurfaceSize - (2*_padding));
        WindowSize = ((int)((viewport.Height - 10 * scale) / LineHeight));

        finalColumnWidth = HeadingFontSize * 80;
        // that thing above is rough - this is just used to stop headings colliding, nothing serious,
        // and is way cheaper than allocating a StringBuilder and measuring the width of the final
        // column heading text in pixels.
    }

    void AddHeading()
    {
        if (surface.Script != "")
        {
            surface.Script = "";
            surface.ScriptBackgroundColor = BackgroundColor;
            surface.ScriptForegroundColor = ForegroundColor;
        }
        if (!unfiltered)
            Heading = Filter;
        Position = new Vector2(viewport.Width / 10f, StartHeight) + viewport.Position;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = "Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds",
            Position = Position + new Vector2(0f,LineHeight/2f),
            Size = new Vector2(LineHeight,LineHeight),
            RotationOrScale = HeadingFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.CENTER
        });
        Position.X += viewport.Width / 8f;
        frame.Add(MySprite.CreateClipRect(new Rectangle((int)Position.X, (int)Position.Y, (int)(viewport.Width - Position.X + (viewport.X - _padding.X/2) - finalColumnWidth), (int)(Position.Y + HeadingHeight))));
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = Heading,
            Position = Position,
            RotationOrScale = HeadingFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        frame.Add(MySprite.CreateClearClipRect());
        Position.X = viewport.Width + viewport.X - _padding.X / 2;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Stock",
            Position = Position,
            RotationOrScale = HeadingFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.RIGHT,
            FontId = "White"
        });

        Position.Y += HeadingHeight;
    }

    internal void SetFilter(string filter)
    {                
        if (null == filter || filter.Length == 0)
            return;
        Filter = filter;
        unfiltered = false;
    }

    void RenderRow(Program.Item item)
    {
        Color TextColor;
        if (item.Amount == 0)
        {
            TextColor = Color.Brown;
        }
        else
        {
            TextColor = surface.ScriptForegroundColor;
        }
        var beyondFirstRow = previousType != "FIRST";
        if (separators && (previousType != item.ItemType))
        {
            previousType = item.ItemType;
            if (beyondFirstRow)
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = new Vector2(viewport.X, Position.Y),
                    Size = new Vector2(viewport.Width, 1),
                    RotationOrScale = 0,
                    Color = HighlightColor,
                });
        }
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = item.Sprite,
            Position = Position + new Vector2(0f,LineHeight/2f),
            Size = new Vector2(LineHeight,LineHeight),
            RotationOrScale = 0,
            Color = TextColor,
            Alignment = TextAlignment.CENTER,
        });
        Position.X += viewport.Width / 8f;
        frame.Add(MySprite.CreateClipRect(new Rectangle((int)Position.X, (int)Position.Y, (int)(viewport.Width - Position.X + (viewport.X -_padding.X / 2) - finalColumnWidth), (int)(Position.Y + HeadingHeight))));
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = item.NaturalName,
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = TextColor,
            Alignment = TextAlignment.LEFT,
            FontId = monospace?"Monospace":"White"
        });
        frame.Add(MySprite.CreateClearClipRect());
        Position.X += viewport.Width * 6f / 8f;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = UseFormatedAmount ? item.FormatedAmount : item.Amount.ToString(),
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = TextColor,
            Alignment = TextAlignment.RIGHT,
            FontId = monospace?"Monospace":"White"
        });
    }

    internal void Render(SortedDictionary<String, Program.Item> Stock)
    {
        MakeSpriteCacheDirty = !MakeSpriteCacheDirty;
        frame = surface.DrawFrame();
        if (MakeSpriteCacheDirty)
        {
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Color = surface.BackgroundColor,
                Position = new Vector2(0, 0),
                Size=new Vector2(0,0)
            });                    
        }
        AddHeading();
        int renderLineCount = 0;
        previousType = "FIRST";
        foreach (var item in Stock.Keys)
        {
            // Contains with StringComparison.InvariantCultureIgnoreCase is prohibited )-:
            if (unfiltered || Filter.ToLower().Contains(Stock[item].ItemType.Substring(Program.characters_to_skip).ToLower()))
            {
                if ((Stock[item].Amount != 0 || !SuppressZeros) && ++renderLineCount > linesToSkip)
                {
                    Position.X = viewport.Width / 10f + viewport.Position.X;
                    if (renderLineCount >= linesToSkip && renderLineCount < linesToSkip + WindowSize)
                        RenderRow(Stock[item]);
                    Position.Y += LineHeight;
                }
            }
        }
        frame.Dispose();
    }
}
List<IMyTerminalBlock> Containers = new List<IMyTerminalBlock>();
static readonly string Version = "Version 1.6.1";
MyIni ini = new MyIni();
static readonly string ConfigSection = "Inventory";
static readonly string DisplaySectionPrefix = ConfigSection + "_Display";
private const string KnowItemsString = "KnownItems";
StringBuilder SectionCandidateName = new StringBuilder();
StringBuilder knownItemsSb = new StringBuilder();
List<string> SectionNames = new List<string>();
SortedDictionary<string, Item> Stock = new SortedDictionary<string, Item>();
SortedDictionary<string, string> Translation = new SortedDictionary<string, string>();
List<MyInventoryItem> Items = new List<MyInventoryItem>();
List<ManagedDisplay> Screens = new List<ManagedDisplay>();
StringBuilder echoBuffer = new StringBuilder();
StringBuilder persistentEchoBuffer = new StringBuilder();
string echoString = "Init...";
IEnumerator<bool> _stateMachine;
int delayCounter = 0;
int delay;

private const string nominalColor = "#FF00A800";
private const string warningColor = "#FFF2C55C";
private const string errorColor = "#FFDB5C5C";
        
const string enabled = "[Color=" + nominalColor +"]enabled[/Color]";
const string warning = "[Color=" + warningColor +"]warning[/Color]";
const string disabled = "[Color=" + errorColor +"]disabled[/Color]";
        
const string ob  = "MyObjectBuilder_";
const int characters_to_skip = 16; // same as "ob.Length"

bool StoreKnownTypes;  // Enable save known types globally
bool TranslateEnabled; // Enable translate feature globally
bool FilterEnabled; // Enable filter feature globally
bool FormatNames; // Enable name prettify feature globally
bool FormatAmount; // Enable Number Abbreviation feature globally
bool Separators;    // Control whether type separator lines are displayed on screen
bool rebuild;
bool clear;
List<MyIniKey> TranslationKeys = new List<MyIniKey>();

public class Item
{
    public Item(string itemType, Program program, int amount = 0)
    {
        var temp = itemType.Split('/');
        Sprite = itemType;
        Name = NaturalName = temp[1];
        ItemType = temp[0];
        Amount = amount;
        KeyString = program.TranslateEnabled ? itemType.Substring(characters_to_skip).ToLower() : "";
    }

    public string KeyString;
    public int Amount;
    public string Sprite;
    public string Name;
    public string ItemType;
    public string NaturalName;
    public string FormatedAmount;
}

public void GetBlocks()
{
    Containers.Clear();
    Screens.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(Containers, block =>
    {
        if (!block.IsSameConstructAs(Me))
            return false;
        if (!TryAddDiscreteScreens(block))
            TryAddScreen(block);
        return block.HasInventory & block.ShowInInventory;
    });
}

void AddScreen(IMyTextSurfaceProvider provider, int displayNumber, string section)
{
    var display = ((IMyTextSurfaceProvider)provider).GetSurface(displayNumber);
    var linesToSkip = ini.Get(section, "skip").ToInt16();
    bool monospace = ini.Get(section, "mono").ToBoolean();
    bool suppressZeros = ini.Get(section, "suppress_zeros").ToBoolean();
    bool separators = ini.Get(section, "separators").ToBoolean(this.Separators);
    float scale = ini.Get(section, "scale").ToSingle(1.0f);
    bool formatAmount = ini.Get(section, "format_amount").ToBoolean(FormatAmount);
    string DefaultColor = "FF4500";
    string ColorStr = ini.Get(section, "color").ToString(DefaultColor);
    if (ColorStr.Length < 6)
        ColorStr = DefaultColor;
    Color color = new Color()
    {
        R = byte.Parse(ColorStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
        G = byte.Parse(ColorStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
        B = byte.Parse(ColorStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
        A = 255
    };
    var managedDisplay = new ManagedDisplay(display, scale, color, linesToSkip, monospace, suppressZeros, separators, formatAmount);
    if (FilterEnabled)
    {
        managedDisplay.SetFilter(ini.Get(section, "filter").ToString(null));
    }
    Screens.Add(managedDisplay);
}

bool TryAddDiscreteScreens(IMyTerminalBlock block)
{
    bool retval = false;
    IMyTextSurfaceProvider Provider = block as IMyTextSurfaceProvider;
    if (null == Provider || Provider.SurfaceCount == 0)
        return true;
    StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
    ini.TryParse(block.CustomData);
    ini.GetSections(SectionNames);
    foreach (var section in SectionNames)
    {
        if (section.StartsWith(DisplaySectionPrefix, ignoreCase))
        {
            bool found = false;
            for (int displayNumber = 0; displayNumber < Provider.SurfaceCount; ++displayNumber)
            {
                if (displayNumber < Provider.SurfaceCount)
                {
                    SectionCandidateName.Clear();
                    SectionCandidateName.Append(DisplaySectionPrefix).Append(displayNumber.ToString());
                    if (section.Equals(SectionCandidateName.ToString(), ignoreCase))
                    {
                        AddScreen(Provider, displayNumber, section);
                        retval = true;
                        found = true;
                    }
                }
            }

            if (!found)
            {
                var displayNumber = section.Substring(DisplaySectionPrefix.Length);
                persistentEchoBuffer.AppendLine($"\n[Color={warningColor}]Warning: {block.CustomName} doesn't have a display number {displayNumber}[/Color]");
            }
        }
    }
    return retval;
}

void TryAddScreen(IMyTerminalBlock block)
{
    IMyTextSurfaceProvider Provider = block as IMyTextSurfaceProvider;
    if (null == Provider || Provider.SurfaceCount == 0 || !MyIni.HasSection(block.CustomData, ConfigSection))
        return;
    ini.TryParse(block.CustomData);
    var displayNumber = ini.Get(ConfigSection, "display").ToUInt16();
    if (displayNumber < Provider.SurfaceCount || Provider.SurfaceCount == 0)
    {
        AddScreen(Provider, displayNumber, ConfigSection);
    }
    else
    {
        persistentEchoBuffer.AppendLine($"\n[Color={warningColor}]Warning: {block.CustomName} doesn't have a display number {displayNumber}[/Color]");
    }
}

public void RunItemCounter()
{
    if (_stateMachine != null)
    {
        bool hasMoreSteps = _stateMachine.MoveNext();

        if (hasMoreSteps)
        {
            Runtime.UpdateFrequency |= UpdateFrequency.Once;
        }
        else
        {
            _stateMachine.Dispose();
            _stateMachine = null;
        }
    }
}

public IEnumerator<bool> LoadStorage()
{
    ini.TryParse(Storage);

    if (ini.ContainsKey(ConfigSection, KnowItemsString))
    {
        var items = ini.Get(ConfigSection, KnowItemsString).ToString();

        if(string.IsNullOrWhiteSpace(items))
            yield break;

        var itemTypes = items.Split('\n');

        foreach (var item in itemTypes)
        {
            if (!Stock.ContainsKey(item))
            {
                Item newItem = new Item(item, this);
                UpdateNaturalName(newItem);
                Stock.Add(item, newItem);
            }
            yield return true;
        }
    }
}

public IEnumerator<bool> CountItems()
{
    foreach (var Item in Stock.Keys)
        Stock[Item].Amount = 0;
    yield return true;
    ReadConfig();
    yield return true;
    yield return true;
    foreach (var container in Containers)
    {
        for (int i = 0; i < container.InventoryCount; ++i)
        {
            var inventory = container.GetInventory(i);
            if (inventory.ItemCount > 0)
            {
                Items.Clear();
                inventory.GetItems(Items);
                foreach (var item in Items)
                {
                    string key = item.Type.ToString();
                    if (!Stock.ContainsKey(key))
                    {
                        Item newItem = new Item(item.Type.ToString(), this);
                        UpdateNaturalName(newItem);
                        Stock.Add(key, newItem);
                    }
                    Stock[key].Amount += item.Amount.ToIntSafe();
                }
                yield return true;
            }
        }
    }
            
    if(FormatAmount || Screens.Any(s => s.UseFormatedAmount))
    {
        foreach (var item in Stock.Values) 
            item.FormatedAmount = MetricFormat(item.Amount);
    }
            
    RenderScreens();
}

public void UpdateNaturalName(Item item)
{
    string value;
    item.NaturalName = Translation.TryGetValue(item.KeyString, out value) ? value :
        FormatNames ? PrettifyString(item.Name) : item.Name;
}

public IEnumerator<bool> RemoveEmptyItems()
{
    var keys = Stock.Keys.ToList();

    for (int i = Stock.Keys.Count - 1; i >= 0; i--)
    {
        if (Stock[keys[i]].Amount == 0)
        {
            Stock.Remove(keys[i]);
            yield return true;
        }
    }
    RenderScreens();
}

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100 | UpdateFrequency.Once;
    ReadConfig();
    GetBlocks();

    if (StoreKnownTypes)
        _stateMachine = LoadStorage();
}

void ReadConfig()
{
    if (ini.TryParse(Me.CustomData))
    {
        delay = ini.Get(ConfigSection, "delay").ToInt32(3);
        var translateEnabled = ini.ContainsSection("translation");
        FilterEnabled = ini.Get(ConfigSection, "enablefilter").ToBoolean(true);
        Separators = ini.Get(ConfigSection, "separators").ToBoolean(true);
        StoreKnownTypes = ini.Get(ConfigSection, "savetypes").ToBoolean(true);
        FormatAmount = ini.Get(ConfigSection, "format_amount").ToBoolean(true);
        var formatNames = ini.Get(ConfigSection, "format_names").ToBoolean(true);

        var updateAllNames = formatNames != FormatNames || TranslateEnabled != translateEnabled;
                
        FormatNames = formatNames;
        TranslateEnabled = translateEnabled;
                
        if (TranslateEnabled)
        {
            TranslationKeys.Clear();
            ini.GetKeys("translation", TranslationKeys);
            foreach (var key in TranslationKeys)
            {
                var lowerKey = key.Name.ToLower();
                if (Translation.ContainsKey(lowerKey))
                {
                    var translated = ini.Get(key).ToString();
                    if (Translation[lowerKey] == translated) 
                        continue;

                    Translation[lowerKey] = translated;
                }
                else
                {
                    Translation.Add(lowerKey, ini.Get(key).ToString());
                }
                        
                if (!updateAllNames && Stock.ContainsKey(ob + key.Name))
                {
                    UpdateNaturalName(Stock[ob + key.Name]);
                }
            }
        }
        else
        {
            Translation.Clear();
        }

        if (updateAllNames)
            foreach (var item in Stock.Values) 
                UpdateNaturalName(item);
    }
}

void RenderScreens()
{
    echoBuffer.Clear();
            
    echoBuffer.AppendLine(Version);
    echoBuffer.AppendLine(Screens.Count + " screens");
    echoBuffer.AppendLine(Containers.Count + " blocks with inventories");
    echoBuffer.AppendLine(Stock.Count + " items being tracked");
    echoBuffer.AppendLine("Saving " + (StoreKnownTypes ? enabled : disabled));
    echoBuffer.AppendLine("Filtering " + (FilterEnabled ? enabled : disabled));
    echoBuffer.AppendLine("Separators (global default) " + (Separators ? enabled : disabled));
    echoBuffer.AppendLine("Translation " + (TranslateEnabled ? Translation.Any() ? enabled : warning : disabled));
    echoBuffer.AppendLine("Name Auto-Format " + (FormatNames ? enabled : disabled));
    echoBuffer.AppendLine("Amount Auto-Format " + (FormatAmount ? enabled : disabled));

    if (TranslateEnabled && !Translation.Any())
    {
        echoBuffer.AppendLine($"\n[Color={warningColor}]Warning: Translations is enabled but no translations have been found[/Color]");
    }
            
    echoBuffer.Append(persistentEchoBuffer);
            
    echoString = echoBuffer.ToString();
            
    foreach (var display in Screens)
    {
        display.Render(Stock);
    }
}

public void Main(string argument, UpdateType updateSource)
{
    Echo(echoString);
            
    if ((updateSource & UpdateType.Once) == UpdateType.Once)
    {
        RunItemCounter();
    }
    if ((updateSource & UpdateType.Update100) == UpdateType.Update100)
    {
        if (delayCounter > delay && _stateMachine == null)
        {
            if (rebuild)
            {
                persistentEchoBuffer.Clear();
                rebuild = false;
                ReadConfig();
                GetBlocks();
            }

            if (clear)
            {
                clear = false;
                _stateMachine = RemoveEmptyItems();
            }
            else
            {
                _stateMachine = CountItems();
            }

            delayCounter = 0;
            RunItemCounter();
        }
        else
        {
            ++delayCounter;
        }
    }
    if (argument == "count" && _stateMachine == null)
    {
        _stateMachine = CountItems();
        RunItemCounter();
    }
    switch (argument)
    {
        case "rebuild":
            rebuild = true;
            break;
        case "clear":
            clear = true;
            break;
    }
}

public void Save()
{
    if(!StoreKnownTypes)
        return;

    ini.TryParse(Storage);
    knownItemsSb.Clear();
    Stock.Keys.ToList().ForEach(key => knownItemsSb.AppendLine(key));
    ini.Set(ConfigSection, KnowItemsString, knownItemsSb.ToString());
    Storage = ini.ToString();
}
        
public static string PrettifyString(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return input;

    var result = new StringBuilder();
    result.Append(input[0]);

    for (int i = 1; i < input.Length; i++)
    {
        if (char.IsUpper(input[i]) && !char.IsWhiteSpace(input[i - 1]))
        {
            result.Append(' ');
        }
        result.Append(input[i]);
    }

    return result.ToString();
}
        
public static string MetricFormat(int input)
{
    if (input >= 1000000000)
        // Congratulations, you've successfully created a singularity
        return (input / 1000000000d).ToString("0.00") + "G"; 
    if (input >= 1000000)
        return (input / 1000000d).ToString("0.00") + "M";
    if (input >= 10000)
        return (input / 1000d).ToString("0.00") + "k";
            
    return input.ToString();
}
