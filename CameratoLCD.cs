public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1; // Run every tick for precise timing
}

// Configuration - Change these names to match your blocks
private const string CAMERA_NAME = "Iron Mine Camera";
private const string LCD_NAME = "Iron Mine Camera LCD";

private const double MAX_SCAN_DISTANCE = 10.0; // Maximum raycast distance
private const int RESOLUTION_X = 50; // ASCII art width
private const int RESOLUTION_Y = 50; // ASCII art height
private const float MAX_PITCH = 55.0f; // Maximum pitch angle in degrees
private const float MAX_YAW = 55.0f; // Maximum yaw angle in degrees
private const double BIT_SPACING = 32.0; // For color function

// Camera scanning parameters
private const double SCAN_RANGE_PER_MS = 2.0; // 2 meters per millisecond idle time
private const double MS_PER_TICK = 16.67; // Approximate milliseconds per game tick
private const double BASE_SCAN_RANGE = 32.0; // Base range available each tick (32m)

private IMyCameraBlock camera = null;
private IMyTextPanel lcd = null;

// Progressive scanning state
private char[,] scanBuffer = new char[RESOLUTION_X, RESOLUTION_Y];
//private string[,] distanceBuffer = new string[RESOLUTION_X, RESOLUTION_Y];
private int currentScanX = 0;
private int currentScanY = 0;
private int ticksSinceLastScan = 0;
private bool scanComplete = false;
private DateTime lastUpdateTime = DateTime.Now;

public void Main(string argument, UpdateType updateSource)
{
    // Find camera and LCD if not already found
    if (camera == null)
    {
        camera = GridTerminalSystem.GetBlockWithName(CAMERA_NAME) as IMyCameraBlock;
        if (camera == null)
        {
            Echo($"Camera '{CAMERA_NAME}' not found!");
            return;
        }
    }

    if (lcd == null)
    {
        lcd = GridTerminalSystem.GetBlockWithName(LCD_NAME) as IMyTextPanel;
        if (lcd == null)
        {
            Echo($"LCD '{LCD_NAME}' not found!");
            return;
        }
        
        // Configure LCD
        lcd.ContentType = ContentType.TEXT_AND_IMAGE;
        lcd.FontSize = 0.4f; // Small font for more detail
        lcd.Font = "Monospace";
    }

    // Enable camera
    camera.EnableRaycast = true;

    // Calculate how many ticks we need to wait based on desired scan distance
    int requiredWaitTicks = CalculateRequiredWaitTicks(MAX_SCAN_DISTANCE);
    
    // Perform progressive scanning
    PerformProgressiveScan(requiredWaitTicks);
    
    // Update display if we have data
    UpdateDisplay();
}

private int CalculateRequiredWaitTicks(double desiredDistance)
{
    if (desiredDistance <= BASE_SCAN_RANGE)
    {
        return 1; // Can scan immediately
    }
    
    // Calculate additional range needed beyond base
    double additionalRange = desiredDistance - BASE_SCAN_RANGE;
    
    // Calculate milliseconds needed for additional range
    double msNeeded = additionalRange / SCAN_RANGE_PER_MS;
    
    // Convert to ticks (round up to ensure we have enough range)
    int ticksNeeded = (int)Math.Ceiling(msNeeded / MS_PER_TICK) + 1;
    
    return Math.Max(1, ticksNeeded);
}

private void PerformProgressiveScan(int waitTicks)
{
    ticksSinceLastScan++;
    
    // Only perform raycast if enough time has passed
    if (ticksSinceLastScan >= waitTicks)
    {
        // Calculate current pixel position
        float pitch = ((float)currentScanY / RESOLUTION_Y - 0.5f) * MAX_PITCH * 2.0f;
        float yaw = ((float)currentScanX / RESOLUTION_X - 0.5f) * MAX_YAW * 2.0f;
        
        // Perform raycast
        MyDetectedEntityInfo hitInfo = camera.Raycast(MAX_SCAN_DISTANCE, pitch, yaw);
        
        // Store result in buffer
        char pixelChar;
        //double distance = 0;
        if (hitInfo.IsEmpty())
        {
            // No hit - empty space (very dark blue)
            pixelChar = ColorToChar(0, 0, 30);
        }
        else
        {
            // Hit something - calculate distance and type-based visualization
            double distance = Vector3D.Distance(hitInfo.HitPosition ?? new Vector3D(0f,0f,0f), camera.WorldMatrix.Translation);
            pixelChar = GetVisualizationChar(distance, hitInfo);
        }
        
        scanBuffer[currentScanX, currentScanY] = pixelChar;
        //distanceBuffer[currentScanX, currentScanY] = distance.ToString("F0") + " ";
        
        // Move to next pixel
        currentScanX++;
        if (currentScanX >= RESOLUTION_X)
        {
            currentScanX = 0;
            currentScanY++;
            if (currentScanY >= RESOLUTION_Y)
            {
                currentScanY = 0;
                scanComplete = true;
            }
        }
        
        // Reset tick counter
        ticksSinceLastScan = 0;
    }
}

private void UpdateDisplay()
{
    StringBuilder display = new StringBuilder();
    
    // Build display from scan buffer
    for (int y = 0; y < RESOLUTION_Y; y++)
    {
        for (int x = 0; x < RESOLUTION_X; x++)
        {
            // Use scanned data if available, otherwise show placeholder
            if (scanBuffer[x, y] != '\0')
            {
                display.Append(scanBuffer[x, y]);
                //display.Append(distanceBuffer[x,y]);
            }
            else
            {
                // Show scanning progress indicator
                if (y == currentScanY && x == currentScanX)
                {
                    display.Append(ColorToChar(255, 255, 0)); // Bright yellow scanning indicator
                }
                else if (y < currentScanY || (y == currentScanY && x < currentScanX))
                {
                    display.Append(ColorToChar(60, 60, 60)); // Dark gray for scanned but empty
                }
                else
                {
                    display.Append(ColorToChar(0, 0, 0)); // Black for unscanned area
                }
            }
        }
        display.AppendLine();
    }
    
    // Update LCD
    lcd.WriteText(display.ToString());
    
    // Calculate scan progress
    int totalPixels = RESOLUTION_X * RESOLUTION_Y;
    int scannedPixels = currentScanY * RESOLUTION_X + currentScanX;
    float progress = (float)scannedPixels / totalPixels * 100f;
    
    // Calculate estimated scan time
    int waitTicks = CalculateRequiredWaitTicks(MAX_SCAN_DISTANCE);
    double scanTimeSeconds = (totalPixels * waitTicks * MS_PER_TICK) / 1000.0;
    
    // Update status
    Echo($"Camera Visualizer Active");
    Echo($"Resolution: {RESOLUTION_X}x{RESOLUTION_Y}");
    Echo($"Max Distance: {MAX_SCAN_DISTANCE}m");
    Echo($"Wait Ticks per Scan: {waitTicks}");
    Echo($"Scan Progress: {progress:F1}%");
    Echo($"Est. Full Scan Time: {scanTimeSeconds:F1}s");
    Echo($"Current Position: ({currentScanX}, {currentScanY})");
    
    if (scanComplete)
    {
        Echo("Scan Complete - Starting New Scan");
        scanComplete = false;
    }
}

private char GetVisualizationChar(double distance, MyDetectedEntityInfo hitInfo)
{
    // Normalize distance (0 = close, 1 = far)
    double normalizedDistance = Math.Min(distance / MAX_SCAN_DISTANCE, 1.0);
    
    // Calculate color based on distance and entity type
    byte red, green, blue;
    
    // Color coding based on what we hit
    if (hitInfo.Type == MyDetectedEntityType.Asteroid)
    {
        // Brown/orange for asteroids, darker when farther
        red = (byte)(200 * (1.0 - normalizedDistance * 0.7));
        green = (byte)(100 * (1.0 - normalizedDistance * 0.7));
        blue = (byte)(20 * (1.0 - normalizedDistance * 0.7));
    }
    else if (hitInfo.Type == MyDetectedEntityType.SmallGrid || hitInfo.Type == MyDetectedEntityType.LargeGrid)
    {
        // Blue for grids, darker when farther
        red = (byte)(30 * (1.0 - normalizedDistance * 0.8));
        green = (byte)(100 * (1.0 - normalizedDistance * 0.8));
        blue = (byte)(255 * (1.0 - normalizedDistance * 0.6));
    }
    else if (hitInfo.Type == MyDetectedEntityType.Planet)
    {
        // Green for planets/voxels, darker when farther
        red = (byte)(20 * (1.0 - normalizedDistance * 0.8));
        green = (byte)(200 * (1.0 - normalizedDistance * 0.6));
        blue = (byte)(30 * (1.0 - normalizedDistance * 0.8));
    }
    else if (hitInfo.Type == MyDetectedEntityType.FloatingObject)
    {
        // Bright red for floating objects
        red = (byte)(255 * (1.0 - normalizedDistance * 0.6));
        green = (byte)(30 * (1.0 - normalizedDistance * 0.8));
        blue = (byte)(30 * (1.0 - normalizedDistance * 0.8));
    }
    else
    {
        // White/gray for unknown objects, darker when farther
        byte brightness = (byte)(200 * (1.0 - normalizedDistance * 0.8));
        red = brightness;
        green = brightness;
        blue = brightness;
    }
    
    // Return just the color-encoded character
    return ColorToChar(red, green, blue);
}

// Space Engineers color encoding function
public static char ColorToChar(byte r, byte g, byte b) 
{
    return (char)(0xe100 + ((int)Math.Round(r / BIT_SPACING) << 6) + ((int)Math.Round(g / BIT_SPACING) << 3) + (int)Math.Round(b / BIT_SPACING));
}