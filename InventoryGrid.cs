/************************************************************************************************

 Supports only dedicated LCD panel blocks, embedded screens like in cockpit are not supported!
 
 1. Find the LCD panel you want to use
 2. Add [INVENTORYGRID] to it's name
 3. Open it's CusomData and write in the container's neme (group supported), or "[all]" to all available inventory (w/o quotes)
 4. Awe the marvels of scripting
 
 Advanced options
 PB settings:
 LCD_TAG=[INVENTORYGRID] : the name tag the script will look for to find the LCD panels you want to use 
 ShowNames=True : Show item names on screen
 OverConnector=False : Look for LCDs and inventories over connectors
 SameGridOnly=False : Disables LCDs and inventories on subgrids (disables overconnectors as well regardless of above setting)
 ShowGridLines=True : Enable grid lines
 IconSize=70 : Size of the icons.
 RefreshRate=1.8 : Paging frequency in sec
 FillBar=false : Show a bar instead of numeric info
 
 Colors:
 Background and text color can be set at your screen settings.
 You can add colors to the icons. Not real colorization, just like a tint.
 You can use any of the following in the customdata with your own RGB values:
 
 AllColor=255,255,255
 OreColor=255,255,255
 IngotColor=255,255,255
 ComponentColor=255,255,255
 AmmoColor=255,255,255
 ConsumableColor=255,255,255
 ToolColor=255,255,255
 GridColor=255,255,255
 
 Consumables include consumables, packages, and gas bottles.
 Tools include tools, weapons, datapads, and space credits
 GridColor only matters if you enable ShowGridLines
 Rest are self-explanatory.
 Individual categories override the All setting.
 You can use these even for each LCD. Screen setting overrides PB setting. Go ham!
 FillBar setting can be set for each lcd as well!
  
 If you want to see only certain item types add a filter parameter to your lcd's customdata with a comma separated list.
 Example: Filter=Ore,Ammo
 The valid categories are the same as for the icon colors (without the 'Color' postfix of course)

 By default the items are in the same order as on the in-game UI. You can override this by adding 'Order' to the lcd's customdata with possible values of 'Name' or 'Amount'.
 Example: Order=Amount
 
 !IMPORTANT! If you add any additional options to a screen make sure the inventory name stays the very first row!
 
 Linking:
 You can link multiple LCDs together by putting the primary LCD's name into the customdata followed by a colon and a page number, instead of an inventory name. All the linked lcd's will inherit all the settings from the primary lcd. Make sure you link only the same type of screens!
 
 Possible future additions:                    
  Embedded screen support
 
*************************************************************************************************

                      DON'T CHANGE ANYTHING HERE!

*************************************************************************************************/

const string VERSION = "1.6";
const float BOXSIZE = 85.3f;
const float MARGIN = 7.5f;
const float BASICSIZE = 512f;

static class Config
{
    public static bool ShowNames = true;
    public static string LCD_TAG = "[INVENTORYGRID]";
    public static bool OverConnector = false;
    public static bool SameGridOnly = false;
    public static bool ShowGridLines = true;
    public static float IconSize = 70f;
    public static float RefreshRate = 1.8f;
	public static bool FillBar = false;
    public static Dictionary<string, Color> TypeColorization = new Dictionary<string, Color>{
        {"AllColor", new Color(255,255,255,255)},
        {"GridColor", new Color(255,255,255,255)}
    };
}

static int tickCounter = 0;
static string Worklog = "";
static Dictionary<string, double> totalMass = new Dictionary<string, double>();
static Dictionary<string, double> totalVolume = new Dictionary<string, double>();
static Dictionary<string, double> totalPercentage = new Dictionary<string, double>();
static Dictionary<string, string> errors = new Dictionary<string, string>();
static Dictionary<string, int> paging = new Dictionary<string, int>();
static List<lcd> lcdList = new List<lcd>();

static Dictionary<string, string> TypeToCategory = new Dictionary<string, string> 
{
    {"MyObjectBuilder_Ore", "Ore"},
    {"MyObjectBuilder_Ingot", "Ingot"},
    {"MyObjectBuilder_Component", "Component"},
    {"MyObjectBuilder_ConsumableItem", "Consumable"},
    {"MyObjectBuilder_AmmoMagazine", "Ammo"},
    {"MyObjectBuilder_GasContainerObject", "Consumable"},
    {"MyObjectBuilder_OxygenContainerObject", "Consumable"},
    {"MyObjectBuilder_PhysicalGunObject", "Tool"},
    {"MyObjectBuilder_PhysicalObject", "Tool"},
    {"MyObjectBuilder_Datapad", "Tool"},
    {"MyObjectBuilder_Package", "Consumable"},
	{"MyObjectBuilder_SeedItem", "Consumable"}
};

public Program()
{
    var customData = this.Me.CustomData.Trim();
    if (customData != "")
        parseCustomData(customData.Split('\n'));
    else
        populateCustomData();

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

void parseCustomData(string[] customData)
{
    for(int g = 0 ; g < customData.Length ; g++)
    {
        var line = customData[g];
        int j = line.IndexOf('=');
        if (j < 0)
        {
            continue;
        }
        var varName = line.Substring(0,j).Trim();
        var varValue = line.Substring(j+1).Trim();
        switch (varName)
        {
            case "LCD_TAG":
                Config.LCD_TAG = varValue;
                break;
            case "IconSize":
                if (!validateCustomVar(varValue, "float"))
                    continue;
                var newValue = float.Parse(varValue);
                if (newValue < 1f || newValue > 85f)
                    continue;
                Config.IconSize = newValue;
                break;
            case "RefreshRate":
                if (!validateCustomVar(varValue, "float"))
                    continue;
                Config.RefreshRate = float.Parse(varValue);
                break;
            case "ShowNames":
                if (!validateCustomVar(varValue, "Bool"))
                    continue;
                Config.ShowNames = bool.Parse(varValue);
                break;
            case "SameGridOnly":
                if (!validateCustomVar(varValue, "Bool"))
                    continue;
                Config.SameGridOnly = bool.Parse(varValue);
                break;
            case "OverConnector":
                if (!validateCustomVar(varValue, "Bool"))
                    continue;
                Config.OverConnector = bool.Parse(varValue);
                break;
            case "ShowGridLines":
                if (!validateCustomVar(varValue, "Bool"))
                    continue;
                Config.ShowGridLines = bool.Parse(varValue);
                break;
			case "FillBar":
                if (!validateCustomVar(varValue, "Bool"))
                    continue;
                Config.FillBar = bool.Parse(varValue);
                break;		
            case "AllColor":
                if (!validateCustomVar(varValue, "Color"))
                    continue;
                Config.TypeColorization["AllColor"] = makeColor(varValue);
                break;
            case "GridColor":
                if (!validateCustomVar(varValue, "Color"))
                    continue;
                Config.TypeColorization["GridColor"] = makeColor(varValue);
                break;
            case "OreColor":
            case "IngotColor":
            case "ComponentColor":
            case "ConsumableColor":
            case "ToolColor":
            case "AmmoColor":
                if (!validateCustomVar(varValue, "Color"))
                    continue;
                Config.TypeColorization.Add(varName, makeColor(varValue));
                break;
             default:
                continue;
        }
    }
}

public static Color makeColor(string colorString)
{
    string[] rgbStrings = colorString.Split(',');
    int[] rgbValues = Array.ConvertAll(rgbStrings, s => int.Parse(s));
    return new Color(rgbValues[0], rgbValues[1], rgbValues[2]);
}

public static bool validateCustomVar(string input, string type)
{
    switch (type)
    {
        case "Color":
            string pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?),(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?),(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(input, pattern);
            return m.Success;
        case "Bool":
            return new string[]{"true","false"}.Contains(input.ToLower());
        case "float":
            float fresult;
            return float.TryParse(input, out fresult);
		case "int":
            int iresult;
            return int.TryParse(input, out iresult);
        case "category":
			return TypeToCategory.ContainsValue(input);
		default:
            return false;
    }
}

void populateCustomData()
{
    string[] customData = new string[]{
        "LCD_TAG=" + Config.LCD_TAG.ToString(),
        "ShowNames=" + Config.ShowNames.ToString(),
        "SameGridOnly=" + Config.SameGridOnly.ToString(),
        "OverConnector=" + Config.OverConnector.ToString(),
        "ShowGridLines=" + Config.ShowGridLines.ToString(),
        "IconSize=" + Config.IconSize.ToString(),
        "RefreshRate=" + Config.RefreshRate.ToString(),
		"FillBar=" + Config.FillBar.ToString()
    };
    this.Me.CustomData = string.Join("\n", customData);
}

 public void Main()
{
    tickCounter += 10;
    if (tickCounter < Math.Round(Config.RefreshRate * 6) * 10)
        return;
    
    errors.Clear();
	lcdList.Clear();
    List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(Config.LCD_TAG, Blocks);
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Inventory Grid " + VERSION);
    sb.AppendLine("------------------------");
    foreach (var block in Blocks)
    {
        if (Config.SameGridOnly && block.CubeGrid != Me.CubeGrid)
            continue;
        if (!Config.OverConnector && !block.IsSameConstructAs(Me))
            continue;

        if (block is IMyTextSurface)
        {
            if (!block.IsWorking)
                continue;
            var lcdBlock = new lcd();
            lcdBlock.block = block;
			lcdList.Add(lcdBlock);
		}
	}
	
	foreach (var lcdBlock in lcdList)
		lcdBlock.parseCustomData();
		
	foreach (var lcdBlock in lcdList)
	{
		if (lcdBlock.LinkedTo != "")
		{
			var master = lcdList.FirstOrDefault(lcd => lcd.block.CustomName == lcdBlock.LinkedTo);
			if (master == null)
			{
				continue;
			}
			copySettings(lcdBlock, master);
			master.Page = 1;
		}
	}
	
	foreach (var lcdBlock in lcdList)
	{
		var containerName = lcdBlock.InventoryName;
		if (containerName == "")
			continue;

		var inventory = getInventory(containerName, lcdBlock.Filter, lcdBlock.Order);
		
		if (inventory.Count == 0 && errors.ContainsKey(containerName))
			sb.AppendLine(lcdBlock.block.CustomName.Replace(Config.LCD_TAG, "").Trim() + " " + containerName + " " + errors[containerName]);
		else
			sb.AppendLine(lcdBlock.block.CustomName.Replace(Config.LCD_TAG, "").Trim() + " showing " + containerName + (lcdBlock.Filter.Count > 0 ? " ("+string.Join(",", lcdBlock.Filter)+")" : "") + (lcdBlock.Page > 0 ? " (page: " + lcdBlock.Page + ")" : ""));
		
		Draw(lcdBlock, inventory);
    }
    Worklog = sb.ToString();
    Echo(Worklog);
    WriteToPBScreen();
    tickCounter = 0;
}

void copySettings(lcd slave, lcd master)
{
	var slaveSurface = slave.block as IMyTextSurface;
	var masterSurface = master.block as IMyTextSurface;
	slaveSurface.ScriptBackgroundColor = masterSurface.ScriptBackgroundColor;
	slaveSurface.ScriptForegroundColor = masterSurface.ScriptForegroundColor;
	slave.InventoryName = master.InventoryName;
    slave.IconSize = master.IconSize;
    slave.TypeColorization = master.TypeColorization;
	slave.Filter = master.Filter;
}

List<InventoryItem> getInventory(string containerName, List<String> Filter, string Order)
{
    resetTotals(containerName);
    var blocks = new List<IMyTerminalBlock>();
    var inventory = new List<MyInventoryItem>();
    
    if (containerName == "[all]")
    {
        var allBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(allBlocks);
        foreach (var block in allBlocks)
        {
            if (block.HasInventory)
                blocks.Add(block);
        }
    }
    else
    {
        List<IMyBlockGroup> BlockGroups = new List<IMyBlockGroup>();      
        GridTerminalSystem.GetBlockGroups(BlockGroups);
        for (int gid = 0; gid < BlockGroups.Count; gid++)       
        {       
            IMyBlockGroup group = BlockGroups[gid];
            if (group.Name != containerName)
                continue;
            
            var groupBlocks = new List<IMyTerminalBlock>();  
            group.GetBlocks(groupBlocks);
            for (var i = 0; i < groupBlocks.Count; i++)
            {
                if (groupBlocks[i].HasInventory)
                    blocks.Add(groupBlocks[i]);
            }
        }
        var singleBlock = GridTerminalSystem.GetBlockWithName(containerName) as IMyTerminalBlock;
        if (singleBlock != null)
            blocks.Add(singleBlock);
    }
    if (blocks.Count == 0)
        errors.Add(containerName, "not found");
    double totalCapacity = 0;
    foreach (var block in blocks)
    {
        if (Config.SameGridOnly && (block.CubeGrid == Me.CubeGrid) == false)
            continue;
        if (Config.OverConnector == false && !block.IsSameConstructAs(Me))
            continue;
        
        for (var i = 0; i < block.InventoryCount; i++)
        {
            block.GetInventory(i).GetItems(inventory);
        }

        totalVolume[containerName] += (double)block.GetInventory().CurrentVolume;
        totalMass[containerName] += (double)block.GetInventory().CurrentMass;
		totalCapacity += (double)block.GetInventory().MaxVolume;
    }
	totalPercentage[containerName] = (totalVolume[containerName] / totalCapacity) * 100;
    
	if (Filter.Count > 0)
	{
		var typeIds = TypeToCategory.Where(pair => Filter.Contains(pair.Value))
                       .Select(pair => pair.Key);
		
		inventory = inventory.Where(item => typeIds.Contains(item.Type.TypeId))
					.ToList();
	}
	string name;
    var grouped = inventory.GroupBy(item => new {item.Type.TypeId, item.Type.SubtypeId})
				.Select(group => new InventoryItem
				{
					TypeId = group.Key.TypeId,
					SubtypeId = group.Key.SubtypeId,
					Amount = group.Sum(item => (int)item.Amount),
					Name = itemNames.TryGetValue(group.Key.TypeId + "/" + group.Key.SubtypeId, out name)
							? name
							: group.Key.SubtypeId
				});
	if (Order == "Amount")
		grouped = grouped.OrderBy(item => item.Amount);
	else if (Order == "Name")
		grouped = grouped.OrderBy(item => item.Name);
	return grouped.ToList();
}

void resetTotals(string containerName)
{
    if (totalVolume.ContainsKey(containerName))
        totalVolume[containerName] = 0;
    else
        totalVolume.Add(containerName, 0);
    if (totalMass.ContainsKey(containerName))
        totalMass[containerName] = 0;
    else
        totalMass.Add(containerName, 0);
}

void Draw(lcd block, List<InventoryItem> inventory)
{
    var screenSurface = block.block as IMyTextSurface;
    if (screenSurface.ContentType != ContentType.SCRIPT)
        screenSurface.ContentType = ContentType.SCRIPT;
    MySpriteDrawFrame frame;
    frame = screenSurface.DrawFrame();
    var screenOffset = 0f;
    if (screenSurface.SurfaceSize.Y != BASICSIZE)
        screenOffset = (BASICSIZE - screenSurface.SurfaceSize.Y)/2 ;
    var headerOffest = 27f;
    var gridLineOffset = 3f;
    var gridLineThickness = 1f;
    if (Config.ShowGridLines)
    {
        frame.Add(new MySprite(
            SpriteType.TEXTURE, 
            "SquareSimple", 
            position: new Vector2(screenSurface.SurfaceSize.X / 2, (headerOffest - gridLineOffset * 2) / 2 + screenOffset + gridLineOffset),
            size: new Vector2(screenSurface.SurfaceSize.X - gridLineOffset * 2, headerOffest - gridLineOffset * 2),
            color: getGridColor(block),
            fontId: null,
            alignment: TextAlignment.CENTER
            )
        );
        frame.Add(new MySprite(
            SpriteType.TEXTURE, 
            "SquareSimple", 
            position: new Vector2(screenSurface.SurfaceSize.X / 2, (headerOffest - gridLineOffset * 2) / 2 + screenOffset + gridLineOffset),
            size: new Vector2((screenSurface.SurfaceSize.X - gridLineOffset * 2) - gridLineThickness * 2, (headerOffest - gridLineOffset * 2) - gridLineThickness * 2),
            color: screenSurface.ScriptBackgroundColor,
            fontId: null,
            alignment: TextAlignment.CENTER
            )
        );
    }
    
    frame.Add(new MySprite(
                SpriteType.TEXT,
                block.InventoryName + (block.Filter.Count > 0 ? " ("+string.Join(",", block.Filter)+")" : "") + (block.Page > -1 ? " (page " + block.Page + ")" :""),
                new Vector2(screenSurface.SurfaceSize.X / 2, 0f + screenOffset),
                null,
                screenSurface.ScriptForegroundColor,
                "DEBUG",
                TextAlignment.CENTER,
                0.7f
                )
            );
    var rowCount = 0;
    var IconSize = getIconSize(block);
    var Margin = (BOXSIZE - IconSize) / 2;
    var XCoord = 0f + Margin;
    var YCoord = 0f;
    var maxRows = Math.Floor((screenSurface.SurfaceSize.Y - headerOffest * 2) / BOXSIZE);
	if (block.Rows > 0 && block.Rows < maxRows)
		maxRows = block.Rows;
	
    var itemPerRow = Math.Floor(screenSurface.SurfaceSize.X / BOXSIZE);
    var maxPerPage = maxRows * itemPerRow;
    if (Config.ShowGridLines)
    {
        for (var y = 0 ; y < maxRows ; y++)
        {
            var BoxY = y * BOXSIZE + headerOffest + screenOffset + BOXSIZE / 2;
            for (var x = 0 ; x <= itemPerRow ; x++)
            {
                var BoxX = x * BOXSIZE + gridLineOffset;
                frame.Add(new MySprite(
                    SpriteType.TEXTURE, 
                    "SquareSimple", 
                    position: new Vector2(BoxX, BoxY),
                    size: new Vector2(BOXSIZE - gridLineOffset * 2, BOXSIZE - gridLineOffset * 2),
                    color: getGridColor(block),
                    fontId: null,
                    alignment: TextAlignment.LEFT
                    )
                );
                frame.Add(new MySprite(
                    SpriteType.TEXTURE, 
                    "SquareSimple", 
                    position: new Vector2(BoxX + gridLineThickness, BoxY),
                    size: new Vector2(BOXSIZE - (gridLineOffset + gridLineThickness) * 2, BOXSIZE - (gridLineOffset + gridLineThickness) * 2),
                    color: screenSurface.ScriptBackgroundColor,
                    fontId: null,
                    alignment: TextAlignment.LEFT
                    )
                );
            }
        }
    }
    
    
    var pages = (int)Math.Floor((inventory.Count - 1) / maxPerPage);
	var startPage = 0;
    if (block.Page == -1)
	{
		if (!paging.ContainsKey(block.block.CustomName))
			paging.Add(block.block.CustomName, 0);
		startPage = (int)paging[block.block.CustomName];
	}
	else
	{
		startPage = block.Page - 1;
	}
    for (var i = (int)(startPage * maxPerPage) ; i < inventory.Count ; i++)
    {
        YCoord = rowCount * BOXSIZE + screenOffset + headerOffest + gridLineOffset;
        // Echo((inventory[i].TypeId + "/" + inventory[i].SubtypeId).ToString());
        frame.Add(new MySprite(
            SpriteType.TEXTURE,
            (inventory[i].TypeId + "/" + inventory[i].SubtypeId).ToString(),
            new Vector2(XCoord + IconSize / 2, YCoord + BOXSIZE / 2 - MARGIN + gridLineOffset),
            new Vector2(IconSize, IconSize),
            getIconColor(block, inventory[i].TypeId),
            null,
            TextAlignment.CENTER,
            0f
            )
        );
        if (Config.ShowNames)
        {
            frame.Add(new MySprite(
                SpriteType.TEXT,
                inventory[i].getName(),
                new Vector2(XCoord - Margin + MARGIN, YCoord),
                null,
                screenSurface.ScriptForegroundColor,
                "DEBUG",
                TextAlignment.LEFT,
                0.45f
                )
            );
        }
        frame.Add(new MySprite(
            SpriteType.TEXT,
            FormatNumber((double)inventory[i].Amount),
            new Vector2(XCoord - Margin + MARGIN, YCoord + BOXSIZE - (MARGIN + gridLineOffset + gridLineThickness) * 2),
            null,
            screenSurface.ScriptForegroundColor,
            "DEBUG",
            TextAlignment.LEFT,
            0.6f
            )
        );
        XCoord += BOXSIZE;
        if (XCoord >= screenSurface.SurfaceSize.X - BOXSIZE / 2)
        {
            XCoord = 0f + Margin;
            rowCount++;
            if (rowCount >= maxRows)
                break;
        }
    }
    if (Config.ShowGridLines)
    {
        frame.Add(new MySprite(
            SpriteType.TEXTURE, 
            "SquareSimple", 
            position: new Vector2(screenSurface.SurfaceSize.X / 2, screenSurface.SurfaceSize.Y - headerOffest / 2 + screenOffset - gridLineOffset),
            size: new Vector2(screenSurface.SurfaceSize.X - gridLineOffset * 2, headerOffest - gridLineOffset * 2),
            color: getGridColor(block),
            fontId: null,
            alignment: TextAlignment.CENTER
            )
        );
        frame.Add(new MySprite(
            SpriteType.TEXTURE, 
            "SquareSimple", 
            position: new Vector2(screenSurface.SurfaceSize.X / 2, screenSurface.SurfaceSize.Y - headerOffest / 2 + screenOffset - gridLineOffset),
            size: new Vector2((screenSurface.SurfaceSize.X - gridLineOffset * 2) - gridLineThickness * 2, (headerOffest - gridLineOffset * 2) - gridLineThickness * 2),
            color: screenSurface.ScriptBackgroundColor,
            fontId: null,
            alignment: TextAlignment.CENTER
            )
        );
    }
	if (block.FillBar ?? Config.FillBar)
	{
		frame.Add(new MySprite(
            SpriteType.TEXTURE, 
            "SquareSimple", 
            position: new Vector2(5.5f, screenSurface.SurfaceSize.Y - headerOffest / 2 + screenOffset - gridLineOffset),
            size: new Vector2(((screenSurface.SurfaceSize.X - gridLineOffset * 2) - gridLineThickness * 2) * (float)totalPercentage[block.InventoryName] / 100, (headerOffest - gridLineOffset * 2) - gridLineThickness * 2)-2,
            color: screenSurface.ScriptForegroundColor,
            fontId: null,
            alignment: TextAlignment.LEFT
            )
        );
	}
	else
	{
		frame.Add(new MySprite(
			SpriteType.TEXT,
			"Mass: " + Math.Round(totalMass[block.InventoryName]).ToString() + " kg / Volume: " + Math.Round(totalVolume[block.InventoryName]*1000).ToString() + " L (" + Math.Round(totalPercentage[block.InventoryName], 2) + "%)",
			new Vector2(Margin, screenSurface.SurfaceSize.Y - headerOffest + screenOffset),
			null,
			screenSurface.ScriptForegroundColor,
			"DEBUG",
			TextAlignment.LEFT,
			0.7f
			)
		);
	}
    frame.Dispose();
	if (block.Page == -1)
	{
		paging[block.block.CustomName]++;
		if (paging[block.block.CustomName] > pages)
			paging[block.block.CustomName] = 0;
	}
}

Color getIconColor(lcd block, string itemTypeId)
{
    if (block.TypeColorization.ContainsKey(TypeToCategory[itemTypeId]+"Color"))
    {
        return block.TypeColorization[TypeToCategory[itemTypeId]+"Color"];
    }
    else if (block.TypeColorization.ContainsKey("AllColor"))
    {
        return block.TypeColorization["AllColor"];
    }
    
    if (Config.TypeColorization.ContainsKey(TypeToCategory[itemTypeId]+"Color"))
        return Config.TypeColorization[TypeToCategory[itemTypeId]+"Color"];
    return Config.TypeColorization["AllColor"];
}

Color getGridColor(lcd block)
{
    if (block.TypeColorization.ContainsKey("GridColor"))
    {
        return block.TypeColorization["GridColor"];
    }
    else
    {
        return Config.TypeColorization["GridColor"];
    }
}

float getIconSize(lcd block)
{
    if (block.IconSize > 0f)
    {
        return block.IconSize;
    }
    else
    {
        return Config.IconSize;
    }
}

static string FormatNumber(double num)
{
    if (num >= 100000)
        return FormatNumber(num/1000)+"k";
    if (num >= 1000)
        return (num/1000D).ToString("0.#")+"k";
    return num.ToString("#,0");
}



    
void WriteToPBScreen()
{
    var selfLCD = Me.GetSurface(0);
    if (selfLCD.ContentType != ContentType.TEXT_AND_IMAGE)
        selfLCD.ContentType = ContentType.TEXT_AND_IMAGE;
    selfLCD.Alignment = TextAlignment.LEFT;
    selfLCD.WriteText(Worklog);
}

class lcd
{
    public IMyTerminalBlock block;
    public string InventoryName = "";
    public float IconSize = 0f;
    public Dictionary<string, Color> TypeColorization = new Dictionary<string, Color>{};
	public List<string> Filter = new List<string>();
	public string LinkedTo = "";
	public int Page = -1;
	public bool? FillBar = null;
	public string Order = "Default";
	public int Rows = 0;
        
    public void parseCustomData()
    {
        var customData = block.CustomData.Split('\n');
        for (int g = 0 ; g < customData.Length ; g++)
        {
            var line = customData[g].Trim();
            if (g == 0)
            {
				var firstLine = line.Split(':');
				if (firstLine.Length > 1 && lcdList.Any(lcd => lcd.block.CustomName == firstLine[0]))
				{
					this.LinkedTo = firstLine[0];
					this.Page = int.Parse(firstLine[1]);
				}
				else
				{
					this.InventoryName = line;
				}
                continue;
            }
            int j = line.IndexOf('=');
            if (j < 0)
            {
                continue;
            }
            var varName = line.Substring(0,j).Trim();
            var varValue = line.Substring(j+1).Trim();
            switch (varName)
            {
                case "IconSize":
                    if (!validateCustomVar(varValue, "float"))
                        continue;
                    var newfValue = float.Parse(varValue);
                    if (newfValue < 1f || newfValue > 85f)
                        continue;
                    this.IconSize = newfValue;
                    break;
                case "GridColor":
                case "AllColor":
                case "OreColor":
                case "IngotColor":
                case "ComponentColor":
                case "ConsumableColor":
                case "ToolColor":
                case "AmmoColor":
                    if (!validateCustomVar(varValue, "Color"))
                        continue;
                    this.TypeColorization.Add(varName, makeColor(varValue));
                    break;
                case "Filter":
					var filters = varValue.Split(',');
					for (var k = 0 ; k < filters.Length ; k++)
					{
						var filterCategory = filters[k].Trim();
						if (validateCustomVar(filterCategory, "category"))
							this.Filter.Add(filterCategory);
					}
					break;
				case "FillBar":
					if (!validateCustomVar(varValue, "Bool"))
						continue;
					this.FillBar = bool.Parse(varValue);
					break;
				case "Order":
					this.Order = varValue;
					break;
				case "Rows":
                    if (!validateCustomVar(varValue, "int"))
                        continue;
                    var newiValue = int.Parse(varValue);
                    if (newiValue < 1)
                        continue;
                    this.Rows = newiValue;
                    break;
				default:
                    continue;
            }
        }
    }
}

class InventoryItem
{
    public string TypeId {get;set;}
    public string SubtypeId {get;set;}
    public int Amount {get;set;}
	public string Name;
    
    public string getName()
    {
        var type = this.TypeId + "/" + this.SubtypeId;
        if (itemNames.ContainsKey(type))
            return itemNames[type];
        else
            return this.SubtypeId;
    }
}


static Dictionary<string, string> itemNames = new Dictionary<string, string>{
{"MyObjectBuilder_Ingot/Stone", "Gravel"},
{"MyObjectBuilder_PhysicalGunObject/AutomaticRifleItem", "MR-20 Rifle"},
{"MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem", "MR-8P Rifle"},
{"MyObjectBuilder_PhysicalGunObject/RapidFireAutomaticRifleItem", "MR-50A Rifle"},
{"MyObjectBuilder_PhysicalGunObject/UltimateAutomaticRifleItem", "MR-30E Rifle"},
{"MyObjectBuilder_PhysicalGunObject/BasicHandHeldLauncherItem", "Rocket Launcher"},
{"MyObjectBuilder_PhysicalGunObject/SemiAutoPistolItem", "S-10 Pistol"},
{"MyObjectBuilder_PhysicalGunObject/ElitePistolItem", "S-10E Pistol"},
{"MyObjectBuilder_PhysicalGunObject/FullAutoPistolItem", "S-20A Pistol"},
{"MyObjectBuilder_PhysicalGunObject/WelderItem", "Welder"},
{"MyObjectBuilder_PhysicalGunObject/Welder2Item", "Enh. Welder"},
{"MyObjectBuilder_PhysicalGunObject/Welder3Item", "Prof. Welder"},
{"MyObjectBuilder_PhysicalGunObject/Welder4Item", "Elite Welder"},
{"MyObjectBuilder_PhysicalGunObject/AngleGrinderItem", "Angle Grinder"},
{"MyObjectBuilder_PhysicalGunObject/AngleGrinder2Item", "Enh. Grinder"},
{"MyObjectBuilder_PhysicalGunObject/AngleGrinder3Item", "Prof. Grinder"},
{"MyObjectBuilder_PhysicalGunObject/AngleGrinder4Item", "Elite Grinder"},
{"MyObjectBuilder_PhysicalGunObject/HandDrillItem", "Hand Drill"},
{"MyObjectBuilder_PhysicalGunObject/HandDrill2Item", "Enh. Drill"},
{"MyObjectBuilder_PhysicalGunObject/HandDrill3Item", "Prof. Drill"},
{"MyObjectBuilder_PhysicalGunObject/HandDrill4Item", "Elite Drill"},
{"MyObjectBuilder_Component/MetalGrid", "Metal Grid"},
{"MyObjectBuilder_Component/InteriorPlate", "Interior Plate"},
{"MyObjectBuilder_Component/SteelPlate", "Steel Plate"},
{"MyObjectBuilder_Component/SmallTube", "Small Tube"},
{"MyObjectBuilder_Component/LargeTube", "Large Tube"},
{"MyObjectBuilder_Component/BulletproofGlass", "Bulletp. Glass"},
{"MyObjectBuilder_Component/Thrust", "Thruster"},
{"MyObjectBuilder_Component/GravityGenerator", "GravGen"},
{"MyObjectBuilder_Component/RadioCommunication", "Radio-comm"},
{"MyObjectBuilder_Component/SolarCell", "Solar Cell"},
{"MyObjectBuilder_Component/PowerCell", "Power Cell"},
{"MyObjectBuilder_Component/ZoneChip", "Zone Chip"},
{"MyObjectBuilder_Component/SabiroidPlushie", "Sabiroid Plushie"},
{"MyObjectBuilder_Component/EngineerPlushie", "Engineer Plushie"},
{"MyObjectBuilder_Component/PrototechCapacitor", "Prt. Capacitor"},
{"MyObjectBuilder_Component/PrototechCircuitry", "Prt. Circuitry"},
{"MyObjectBuilder_Component/PrototechCoolingUnit", "Prt. Cooling"},
{"MyObjectBuilder_Component/PrototechFrame", "Prt. Frame"},
{"MyObjectBuilder_Component/PrototechMachinery", "Prt. Machinery"},
{"MyObjectBuilder_Component/PrototechPanel", "Prt. Panel"},
{"MyObjectBuilder_Component/PrototechPropulsionUnit", "Prt. Propulsion"},
{"MyObjectBuilder_Ingot/PrototechScrap", "Prt. Scrap"},
{"MyObjectBuilder_PhysicalObject/SpaceCredit", "Space Credit"},
{"MyObjectBuilder_AmmoMagazine/NATO_5p56x45mm", "5.56x45mm"},
{"MyObjectBuilder_AmmoMagazine/NATO_25x184mm", "25x184mm"},
{"MyObjectBuilder_AmmoMagazine/Missile200mm", "200mm Missile"},
{"MyObjectBuilder_AmmoMagazine/LargeCalibreAmmo", "Artillery Shell"},
{"MyObjectBuilder_AmmoMagazine/MediumCalibreAmmo", "Assault Shell"},
{"MyObjectBuilder_AmmoMagazine/AutocannonClip", "Autocannon Mag."},
{"MyObjectBuilder_AmmoMagazine/FlareClip", "Flare clip"},
{"MyObjectBuilder_AmmoMagazine/AutomaticRifleGun_Mag_20rd", "MR-20 Mag."},
{"MyObjectBuilder_AmmoMagazine/UltimateAutomaticRifleGun_Mag_30rd", "MR-30E Mag."},
{"MyObjectBuilder_AmmoMagazine/RapidFireAutomaticRifleGun_Mag_50rd", "MR-50A Mag."},
{"MyObjectBuilder_AmmoMagazine/PreciseAutomaticRifleGun_Mag_5rd", "MR-8P Mag."},
{"MyObjectBuilder_AmmoMagazine/SemiAutoPistolMagazine", "S-10 Mag."},
{"MyObjectBuilder_AmmoMagazine/ElitePistolMagazine", "S-10A Mag."},
{"MyObjectBuilder_AmmoMagazine/FullAutoPistolMagazine", "S-20A Mag."},
{"MyObjectBuilder_AmmoMagazine/SmallRailgunAmmo", "Small RG Sabot"},
{"MyObjectBuilder_AmmoMagazine/LargeRailgunAmmo", "Large RG Sabot"},
{"MyObjectBuilder_AmmoMagazine/FireworksBoxBlue", "FW Blue"},
{"MyObjectBuilder_AmmoMagazine/FireworksBoxGreen", "FW Green"},
{"MyObjectBuilder_AmmoMagazine/FireworksBoxPink", "FW Pink"},
{"MyObjectBuilder_AmmoMagazine/FireworksBoxRainbow", "FW Rainbow"},
{"MyObjectBuilder_AmmoMagazine/FireworksBoxRed", "FW Red"},
{"MyObjectBuilder_AmmoMagazine/FireworksBoxYellow", "FW Yellow"},
{"MyObjectBuilder_GasContainerObject/OxygenBottle", "O2 Bottle"},
{"MyObjectBuilder_GasContainerObject/HydrogenBottle","H2 Bottle"},
{"MyObjectBuilder_ConsumableItem/InsectMeatRaw","Meat Raw"},
{"MyObjectBuilder_ConsumableItem/InsectMeatCooked","Meat Cooked"},
{"MyObjectBuilder_ConsumableItem/MammalMeatRaw","Meat Raw"},
{"MyObjectBuilder_ConsumableItem/MammalMeatCooked","Meat Cooked"},
{"MyObjectBuilder_ConsumableItem/MealPack_Burrito", "Burrito"},
{"MyObjectBuilder_ConsumableItem/MealPack_Chili", "Chili"},
{"MyObjectBuilder_ConsumableItem/MealPack_ClangCrunchies", "Clang Crunch"},
{"MyObjectBuilder_ConsumableItem/MealPack_Curry", "Curry"},
{"MyObjectBuilder_ConsumableItem/MealPack_Dumplings", "Dumplings"},
{"MyObjectBuilder_ConsumableItem/MealPack_Flatbread", "Flatbread"},
{"MyObjectBuilder_ConsumableItem/MealPack_FrontierStew", "Frontier Stew"},
{"MyObjectBuilder_ConsumableItem/MealPack_FruitBar", "Fruit Bar"},
{"MyObjectBuilder_ConsumableItem/MealPack_FruitPastry", "Fruit Pastry"},
{"MyObjectBuilder_ConsumableItem/MealPack_GardenSlaw", "Garden Slaw"},
{"MyObjectBuilder_ConsumableItem/MealPack_GreenPellets", "Green Pellets"},
{"MyObjectBuilder_ConsumableItem/MealPack_Hardtack", "Hardtack"},
{"MyObjectBuilder_ConsumableItem/MealPack_KelpCrisp", "Kelp Crisp"},
{"MyObjectBuilder_ConsumableItem/MealPack_Lasagna", "Lasagna"},
{"MyObjectBuilder_ConsumableItem/MealPack_Ramen", "Ramen"},
{"MyObjectBuilder_ConsumableItem/MealPack_RedPellets", "Red Pellets"},
{"MyObjectBuilder_ConsumableItem/MealPack_SearedSabiroid", "Seared Sabi"},
{"MyObjectBuilder_ConsumableItem/MealPack_Spaghetti", "Spaghetti"},
{"MyObjectBuilder_ConsumableItem/MealPack_SteakDinner", "Steak"},
{"MyObjectBuilder_ConsumableItem/MealPack_VeggieBurger", "Veggie Burger"},
{"MyObjectBuilder_SeedItem/Grain", "Grain Seed"},
{"MyObjectBuilder_SeedItem/Fruit", "Fruit Seed"},
{"MyObjectBuilder_SeedItem/Mushrooms", "Spores"}
};