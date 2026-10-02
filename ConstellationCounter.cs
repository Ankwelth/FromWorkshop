// R e a d m e
// -----------
// 
// In this file you can include any instructions or other comments you want to have injected onto the 
// top of your final script. You can safely delete this file if you do not want any such comments.
// 
// Configuration
private const string ORE_LCD_TAG = "[Ore Display]";
private const string INGOT_LCD_TAG = "[Ingot Display]";
private const int UPDATE_FREQUENCY_TICKS = 60; // Update every 60 ticks (1 second)
private const float FONT_SIZE = 0.8f;
        
// Runtime variables
List<IMyCargoContainer> cargoContainers = new List<IMyCargoContainer>();
List<IMyRefinery> refineries = new List<IMyRefinery>();
List<IMyAssembler> assemblers = new List<IMyAssembler>();
List<IMyTextPanel> oreLCDs = new List<IMyTextPanel>();
List<IMyTextPanel> ingotLCDs = new List<IMyTextPanel>();
        
Dictionary<string, double> oreAmounts = new Dictionary<string, double>();
Dictionary<string, double> ingotAmounts = new Dictionary<string, double>();
Dictionary<string, double> previousOreAmounts = new Dictionary<string, double>();
Dictionary<string, double> previousIngotAmounts = new Dictionary<string, double>();
        
int tickCounter = 0;
bool forceUpdate = false;
        
// Ore and ingot definitions with sprites
private readonly Dictionary<string, string> oreSprites = new Dictionary<string, string>
{
    {"Iron", "MyObjectBuilder_Ore/Iron"},
    {"Nickel", "MyObjectBuilder_Ore/Nickel"},
    {"Cobalt", "MyObjectBuilder_Ore/Cobalt"},
    {"Magnesium", "MyObjectBuilder_Ore/Magnesium"},
    {"Silicon", "MyObjectBuilder_Ore/Silicon"},
    {"Silver", "MyObjectBuilder_Ore/Silver"},
    {"Gold", "MyObjectBuilder_Ore/Gold"},
    {"Platinum", "MyObjectBuilder_Ore/Platinum"},
    {"Uranium", "MyObjectBuilder_Ore/Uranium"},
    {"Stone", "MyObjectBuilder_Ore/Stone"},
    {"Ice", "MyObjectBuilder_Ore/Ice"}
};
        
private readonly Dictionary<string, string> ingotSprites = new Dictionary<string, string>
{
    {"Iron", "MyObjectBuilder_Ingot/Iron"},
    {"Nickel", "MyObjectBuilder_Ingot/Nickel"},
    {"Cobalt", "MyObjectBuilder_Ingot/Cobalt"},
    {"Magnesium", "MyObjectBuilder_Ingot/Magnesium"},
    {"Silicon", "MyObjectBuilder_Ingot/Silicon"},
    {"Silver", "MyObjectBuilder_Ingot/Silver"},
    {"Gold", "MyObjectBuilder_Ingot/Gold"},
    {"Platinum", "MyObjectBuilder_Ingot/Platinum"},
    {"Uranium", "MyObjectBuilder_Ingot/Uranium"},
    {"Stone", "MyObjectBuilder_Ingot/Stone"},
    {"Gravel", "MyObjectBuilder_Ingot/Stone"}
};

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    InitializeBlocks();
    Echo("Constellation Counter initialized!");
}

public void Save()
{
    // Save state if needed
}

public void Main(string argument, UpdateType updateSource)
{
    try
    {
        // Handle manual triggers
        if (!string.IsNullOrEmpty(argument))
        {
            switch (argument.ToLower())
            {
                case "refresh":
                case "update":
                    forceUpdate = true;
                    break;
                case "init":
                    InitializeBlocks();
                    break;
            }
        }
                
        tickCounter++;
                
        // Event-based update: only scan and update when needed
        if (tickCounter >= UPDATE_FREQUENCY_TICKS || forceUpdate || 
            (updateSource & UpdateType.Terminal) != 0 ||
            (updateSource & UpdateType.Trigger) != 0)
        {
            ScanInventories();
                    
            if (HasInventoryChanged() || forceUpdate)
            {
                UpdateDisplays();
                UpdatePreviousAmounts();
                Echo($"Updated displays - Ores: {oreAmounts.Count}, Ingots: {ingotAmounts.Count}");
            }
                    
            tickCounter = 0;
            forceUpdate = false;
        }
                
        Echo($"Tick: {tickCounter}/{UPDATE_FREQUENCY_TICKS}");
    }
    catch (Exception e)
    {
        Echo($"Error: {e.Message}");
    }
}
        
void InitializeBlocks()
{
    cargoContainers.Clear();
    refineries.Clear();
    assemblers.Clear();
    oreLCDs.Clear();
    ingotLCDs.Clear();
            
    // Get all cargo containers
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers);
            
    // Get refineries and assemblers (they have inventories too)
    GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineries);
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
            
    // Get LCD panels
    List<IMyTextPanel> allLCDs = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(allLCDs);
            
    foreach (var lcd in allLCDs)
    {
        if (lcd.DisplayNameText.Contains(ORE_LCD_TAG) || lcd.CustomName.Contains(ORE_LCD_TAG))
        {
            oreLCDs.Add(lcd);
            SetupLCD(lcd, "ORE INVENTORY");
        }
        else if (lcd.DisplayNameText.Contains(INGOT_LCD_TAG) || lcd.CustomName.Contains(INGOT_LCD_TAG))
        {
            ingotLCDs.Add(lcd);
            SetupLCD(lcd, "INGOT INVENTORY");
        }
    }
            
    Echo($"Found: {cargoContainers.Count} cargo, {refineries.Count} refineries, {assemblers.Count} assemblers");
    Echo($"LCDs: {oreLCDs.Count} ore, {ingotLCDs.Count} ingot");
            
    // Provide helpful setup information
    if (oreLCDs.Count == 0 && ingotLCDs.Count == 0)
    {
        Echo("⚠ No tagged LCD panels found!");
        Echo($"Tag LCDs with '{ORE_LCD_TAG}' or '{INGOT_LCD_TAG}'");
    }
            
    if (cargoContainers.Count == 0 && refineries.Count == 0 && assemblers.Count == 0)
    {
        Echo("⚠ No storage blocks found!");
        Echo("Add cargo containers, refineries, or assemblers");
    }
            
    // Force an immediate update to show initial state
    forceUpdate = true;
}
        
void SetupLCD(IMyTextPanel lcd, string title)
{
    lcd.ContentType = ContentType.SCRIPT;
    lcd.Script = "";
    lcd.ScriptBackgroundColor = Color.Black;
    lcd.ScriptForegroundColor = Color.Cyan;
}
        
void ScanInventories()
{
    oreAmounts.Clear();
    ingotAmounts.Clear();
            
    // Check if we have any blocks to scan
    if (cargoContainers.Count == 0 && refineries.Count == 0 && assemblers.Count == 0)
    {
        Echo("Warning: No cargo containers, refineries, or assemblers found!");
        return;
    }
            
    // Scan cargo containers
    foreach (var container in cargoContainers)
    {
        if (container?.GetInventory(0) != null)
        {
            ScanInventory(container.GetInventory(0));
        }
    }
            
    // Scan refineries
    foreach (var refinery in refineries)
    {
        // Input inventory (ores)
        if (refinery?.GetInventory(0) != null)
            ScanInventory(refinery.GetInventory(0));
                    
        // Output inventory (ingots)
        if (refinery?.GetInventory(1) != null)
            ScanInventory(refinery.GetInventory(1));
    }
            
    // Scan assemblers
    foreach (var assembler in assemblers)
    {
        // Input inventory (ingots)
        if (assembler?.GetInventory(0) != null)
            ScanInventory(assembler.GetInventory(0));
                    
        // Output inventory (components)
        if (assembler?.GetInventory(1) != null)
            ScanInventory(assembler.GetInventory(1));
    }
            
    Echo($"Scanned inventories - Found {oreAmounts.Count} ore types, {ingotAmounts.Count} ingot types");
}
        
void ScanInventory(IMyInventory inventory)
{
    var items = new List<MyInventoryItem>();
    inventory.GetItems(items);
            
    foreach (var item in items)
    {
        string itemName = item.Type.SubtypeId;
        double amount = (double)item.Amount;
                
        if (item.Type.TypeId == "MyObjectBuilder_Ore")
        {
            if (oreAmounts.ContainsKey(itemName))
                oreAmounts[itemName] += amount;
            else
                oreAmounts[itemName] = amount;
        }
        else if (item.Type.TypeId == "MyObjectBuilder_Ingot")
        {
            if (ingotAmounts.ContainsKey(itemName))
                ingotAmounts[itemName] += amount;
            else
                ingotAmounts[itemName] = amount;
        }
    }
}
        
bool HasInventoryChanged()
{
    // Check if ore amounts changed
    if (oreAmounts.Count != previousOreAmounts.Count)
        return true;
                
    foreach (var kvp in oreAmounts)
    {
        if (!previousOreAmounts.ContainsKey(kvp.Key) || 
            Math.Abs(previousOreAmounts[kvp.Key] - kvp.Value) > 0.1)
            return true;
    }
            
    // Check if ingot amounts changed
    if (ingotAmounts.Count != previousIngotAmounts.Count)
        return true;
                
    foreach (var kvp in ingotAmounts)
    {
        if (!previousIngotAmounts.ContainsKey(kvp.Key) || 
            Math.Abs(previousIngotAmounts[kvp.Key] - kvp.Value) > 0.1)
            return true;
    }
            
    return false;
}
        
void UpdatePreviousAmounts()
{
    previousOreAmounts.Clear();
    previousIngotAmounts.Clear();
            
    foreach (var kvp in oreAmounts)
        previousOreAmounts[kvp.Key] = kvp.Value;
                
    foreach (var kvp in ingotAmounts)
        previousIngotAmounts[kvp.Key] = kvp.Value;
}
        
void UpdateDisplays()
{
    // Update ore displays
    foreach (var lcd in oreLCDs)
    {
        UpdateOreDisplay(lcd);
    }
            
    // Update ingot displays
    foreach (var lcd in ingotLCDs)
    {
        UpdateIngotDisplay(lcd);
    }
}
        
void UpdateOreDisplay(IMyTextPanel lcd)
{
    var viewport = new RectangleF(
        (lcd.TextureSize - lcd.SurfaceSize) / 2f,
        lcd.SurfaceSize);
            
    using (var frame = lcd.DrawFrame())
    {
        // Background
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = "Grid",
            Position = viewport.Center,
            Size = viewport.Size,
            Color = lcd.ScriptForegroundColor * 0.1f,
            Alignment = TextAlignment.CENTER
        });
                
        // Header
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "ORE INVENTORY",
            Position = viewport.Position + new Vector2(viewport.Width / 2, 30),
            RotationOrScale = 1.2f,
            FontId = "White",
            Color = Color.Cyan,
            Alignment = TextAlignment.CENTER
        });
                
        if (cargoContainers.Count == 0 && refineries.Count == 0 && assemblers.Count == 0)
        {
            // No storage blocks found
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "NO STORAGE BLOCKS FOUND",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2 - 30),
                RotationOrScale = 1.0f,
                FontId = "White",
                Color = Color.Orange,
                Alignment = TextAlignment.CENTER
            });
                    
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Add cargo containers, refineries,",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2),
                RotationOrScale = 0.8f,
                FontId = "White",
                Color = Color.Yellow,
                Alignment = TextAlignment.CENTER
            });
                    
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "or assemblers to your grid",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2 + 25),
                RotationOrScale = 0.8f,
                FontId = "White",
                Color = Color.Yellow,
                Alignment = TextAlignment.CENTER
            });
        }
        else if (oreAmounts.Count == 0)
        {
            // No ores found
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "NO ORES DETECTED",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2 - 30),
                RotationOrScale = 1.0f,
                FontId = "White",
                Color = Color.Orange,
                Alignment = TextAlignment.CENTER
            });
                    
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Mine some ore or check your",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2),
                RotationOrScale = 0.8f,
                FontId = "White",
                Color = Color.Yellow,
                Alignment = TextAlignment.CENTER
            });
                    
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "refinery input inventories",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2 + 25),
                RotationOrScale = 0.8f,
                FontId = "White",
                Color = Color.Yellow,
                Alignment = TextAlignment.CENTER
            });
        }
        else
        {
            // Display ores with actual sprites
            var sortedOres = oreAmounts.OrderByDescending(x => x.Value).ToList();
            float yStart = viewport.Position.Y + 60;
            float ySpacing = 40;
            int maxItems = (int)((viewport.Height - 110) / ySpacing); // Dynamic max based on screen size
                    
            for (int i = 0; i < Math.Min(sortedOres.Count, maxItems); i++)
            {
                var ore = sortedOres[i];
                float yPos = yStart + (i * ySpacing);
                        
                // Measure text height for perfect alignment
                var textSize = lcd.MeasureStringInPixels(new StringBuilder(ore.Key), "White", 1.0f);
                float iconSize = textSize.Y;
                        
                // Draw ore sprite - perfectly aligned with text baseline
                if (oreSprites.ContainsKey(ore.Key))
                {
                    frame.Add(new MySprite()
                    {
                        Type = SpriteType.TEXTURE,
                        Data = oreSprites[ore.Key],
                        Position = new Vector2(viewport.Position.X + 40, yPos + textSize.Y * 0.5f),
                        Size = new Vector2(iconSize, iconSize),
                        Color = Color.White,
                        Alignment = TextAlignment.CENTER
                    });
                }
                        
                // Draw ore name - aligned to left edge of text area
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = ore.Key,
                    Position = new Vector2(viewport.Position.X + 70, yPos),
                    RotationOrScale = 1.0f,
                    FontId = "White",
                    Color = Color.White,
                    Alignment = TextAlignment.LEFT
                });
                        
                // Draw amount with status color - right aligned, no decimals
                string amount = FormatAmount(ore.Value);
                Color statusColor = GetOreStatusColor(ore.Value);
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = amount,
                    Position = new Vector2(viewport.Position.X + viewport.Width - 25, yPos),
                    RotationOrScale = 1.0f,
                    FontId = "Monospace",
                    Color = statusColor,
                    Alignment = TextAlignment.RIGHT
                });
            }
                    
            if (sortedOres.Count > maxItems)
            {
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = $"... and {sortedOres.Count - maxItems} more ore types",
                    Position = new Vector2(viewport.Position.X + viewport.Width / 2, yStart + (maxItems * ySpacing)),
                    RotationOrScale = 0.7f,
                    FontId = "White",
                    Color = Color.Gray,
                    Alignment = TextAlignment.CENTER
                });
            }
        }
                
        // Footer info
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = $"Last Update: {DateTime.Now:HH:mm:ss}",
            Position = new Vector2(viewport.Position.X + 10, viewport.Position.Y + viewport.Height - 40),
            RotationOrScale = 0.6f,
            FontId = "Monospace",
            Color = Color.Gray,
            Alignment = TextAlignment.LEFT
        });
                
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = $"Storage Blocks: {cargoContainers.Count + refineries.Count + assemblers.Count}",
            Position = new Vector2(viewport.Position.X + viewport.Width - 10, viewport.Position.Y + viewport.Height - 40),
            RotationOrScale = 0.6f,
            FontId = "Monospace",
            Color = Color.Gray,
            Alignment = TextAlignment.RIGHT
        });
    }
}
        
void UpdateIngotDisplay(IMyTextPanel lcd)
{
    var viewport = new RectangleF(
        (lcd.TextureSize - lcd.SurfaceSize) / 2f,
        lcd.SurfaceSize);
            
    using (var frame = lcd.DrawFrame())
    {
        // Background
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = "Grid",
            Position = viewport.Center,
            Size = viewport.Size,
            Color = lcd.ScriptForegroundColor * 0.1f,
            Alignment = TextAlignment.CENTER
        });
                
        // Header
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "INGOT INVENTORY",
            Position = viewport.Position + new Vector2(viewport.Width / 2, 30),
            RotationOrScale = 1.2f,
            FontId = "White",
            Color = Color.Cyan,
            Alignment = TextAlignment.CENTER
        });
                
        if (cargoContainers.Count == 0 && refineries.Count == 0 && assemblers.Count == 0)
        {
            // No storage blocks found
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "NO STORAGE BLOCKS FOUND",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2 - 30),
                RotationOrScale = 1.0f,
                FontId = "White",
                Color = Color.Orange,
                Alignment = TextAlignment.CENTER
            });
                    
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Add cargo containers, refineries,",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2),
                RotationOrScale = 0.8f,
                FontId = "White",
                Color = Color.Yellow,
                Alignment = TextAlignment.CENTER
            });
                    
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "or assemblers to your grid",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2 + 25),
                RotationOrScale = 0.8f,
                FontId = "White",
                Color = Color.Yellow,
                Alignment = TextAlignment.CENTER
            });
        }
        else if (ingotAmounts.Count == 0)
        {
            // No ingots found
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "NO INGOTS DETECTED",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2 - 30),
                RotationOrScale = 1.0f,
                FontId = "White",
                Color = Color.Orange,
                Alignment = TextAlignment.CENTER
            });
                    
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Refine some ore or check your",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2),
                RotationOrScale = 0.8f,
                FontId = "White",
                Color = Color.Yellow,
                Alignment = TextAlignment.CENTER
            });
                    
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "refinery output inventories",
                Position = viewport.Position + new Vector2(viewport.Width / 2, viewport.Height / 2 + 25),
                RotationOrScale = 0.8f,
                FontId = "White",
                Color = Color.Yellow,
                Alignment = TextAlignment.CENTER
            });
        }
        else
        {
            // Display ingots with actual sprites
            var sortedIngots = ingotAmounts.OrderByDescending(x => x.Value).ToList();
            float yStart = viewport.Position.Y + 60;
            float ySpacing = 40;
            int maxItems = (int)((viewport.Height - 110) / ySpacing); // Dynamic max based on screen size
                    
            for (int i = 0; i < Math.Min(sortedIngots.Count, maxItems); i++)
            {
                var ingot = sortedIngots[i];
                float yPos = yStart + (i * ySpacing);
                        
                // Measure text height for perfect alignment
                var textSize = lcd.MeasureStringInPixels(new StringBuilder(ingot.Key), "White", 1.0f);
                float iconSize = textSize.Y;
                        
                // Draw ingot sprite - perfectly aligned with text baseline
                if (ingotSprites.ContainsKey(ingot.Key))
                {
                    frame.Add(new MySprite()
                    {
                        Type = SpriteType.TEXTURE,
                        Data = ingotSprites[ingot.Key],
                        Position = new Vector2(viewport.Position.X + 40, yPos + textSize.Y * 0.5f),
                        Size = new Vector2(iconSize, iconSize),
                        Color = Color.White,
                        Alignment = TextAlignment.CENTER
                    });
                }
                        
                // Draw ingot name - aligned to left edge of text area
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = ingot.Key,
                    Position = new Vector2(viewport.Position.X + 70, yPos),
                    RotationOrScale = 1.0f,
                    FontId = "White",
                    Color = Color.White,
                    Alignment = TextAlignment.LEFT
                });
                        
                // Draw amount with status color - right aligned, no decimals
                string amount = FormatAmount(ingot.Value);
                Color statusColor = GetIngotStatusColor(ingot.Value);
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = amount,
                    Position = new Vector2(viewport.Position.X + viewport.Width - 25, yPos),
                    RotationOrScale = 1.0f,
                    FontId = "Monospace",
                    Color = statusColor,
                    Alignment = TextAlignment.RIGHT
                });
            }
                    
            if (sortedIngots.Count > maxItems)
            {
                frame.Add(new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = $"... and {sortedIngots.Count - maxItems} more ingot types",
                    Position = new Vector2(viewport.Position.X + viewport.Width / 2, yStart + (maxItems * ySpacing)),
                    RotationOrScale = 0.7f,
                    FontId = "White",
                    Color = Color.Gray,
                    Alignment = TextAlignment.CENTER
                });
            }
        }
                
        // Footer info
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = $"Last Update: {DateTime.Now:HH:mm:ss}",
            Position = new Vector2(viewport.Position.X + 10, viewport.Position.Y + viewport.Height - 40),
            RotationOrScale = 0.6f,
            FontId = "Monospace",
            Color = Color.Gray,
            Alignment = TextAlignment.LEFT
        });
                
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = $"Storage Blocks: {cargoContainers.Count + refineries.Count + assemblers.Count}",
            Position = new Vector2(viewport.Position.X + viewport.Width - 10, viewport.Position.Y + viewport.Height - 40),
            RotationOrScale = 0.6f,
            FontId = "Monospace",
            Color = Color.Gray,
            Alignment = TextAlignment.RIGHT
        });
    }
}
        
string FormatAmount(double amount)
{
    if (amount >= 1000000)
        return $"{amount / 1000000:F0}M";
    else if (amount >= 1000)
        return $"{amount / 1000:F0}K";
    else
        return $"{amount:F0}";
}
        
string GetOreStatus(double amount)
{
    if (amount < 100) return "⚠";
    if (amount < 1000) return "▲";
    return "✓";
}
        
string GetIngotStatus(double amount)
{
    if (amount < 50) return "⚠";
    if (amount < 500) return "▲";
    return "✓";
}
        
Color GetOreStatusColor(double amount)
{
    if (amount < 100) return Color.Red;
    if (amount < 1000) return Color.Yellow;
    return Color.Green;
}
        
Color GetIngotStatusColor(double amount)
{
    if (amount < 50) return Color.Red;
    if (amount < 500) return Color.Yellow;
    return Color.Green;
}
        
// Add a progress bar to show inventory fullness
void DrawProgressBar(MySpriteDrawFrame frame, Vector2 position, float width, float height, float fillPercentage, Color fillColor)
{
    // Background
    frame.Add(new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        Position = position,
        Size = new Vector2(width, height),
        Color = Color.Black * 0.5f,
        Alignment = TextAlignment.CENTER
    });
            
    // Border
    frame.Add(new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareHollow",
        Position = position,
        Size = new Vector2(width, height),
        Color = Color.White * 0.8f,
        Alignment = TextAlignment.CENTER
    });
            
    // Fill
    if (fillPercentage > 0)
    {
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = "SquareSimple",
            Position = new Vector2(position.X - width/2 + (width * fillPercentage)/2, position.Y),
            Size = new Vector2(width * fillPercentage, height - 2),
            Color = fillColor,
            Alignment = TextAlignment.CENTER
        });
    }
}
