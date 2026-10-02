// ------------------------------------------------------------------------------------------
//       R A Y C A S T   D I S P L A Y   S C R I P T
// ------------------------------------------------------------------------------------------

// --- Configuration ---

string cameraName = "AAA Camera Raycast"; // Name of the camera block to use for raycasting
string lcdName = "Raycast Info LCD"; // Name of the LCD panel to display results
float raycastDistance = 1000f; // Max distance for the raycast (meters)
UpdateFrequency updateRate = UpdateFrequency.Update10; // How often the script updates (Update1, Update10, Update100)
// Update1 = every tick (60 times/sec) - most frequent, highest sim speed impact
// Update10 = every 10 ticks (6 times/sec)
// Update100 = every 100 ticks (0.6 times/sec) - least frequent, lowest sim speed impact

// --- End Configuration ---

IMyCameraBlock raycastCamera;
IMyTextPanel outputLcd;

public Program()
{
    Runtime.UpdateFrequency = updateRate;
    Echo("Raycast Display Script Initialized.");
    InitializeBlocks();
}

public void Main(string argument, UpdateType updateSource)
{
    // Re-initialize blocks if they are null or if a terminal/script command was received
    if (raycastCamera == null || outputLcd == null || (updateSource & (UpdateType.Terminal | UpdateType.Script | UpdateType.Once)) != 0)
    {
        InitializeBlocks();
    }

    if (raycastCamera == null)
    {
        Echo($"Error: Camera '{cameraName}' not found. Please check configuration.");
        if (outputLcd != null) outputLcd.WriteText($"Error: Camera '{cameraName}' not found.", false);
        return;
    }

    if (outputLcd == null)
    {
        Echo($"Error: LCD '{lcdName}' not found. Please check configuration.");
        // Cannot write to LCD if it's null, but will echo to PB directly.
        return;
        
    }

    // Ensure camera is enabled for raycasting
    if (!raycastCamera.Enabled) raycastCamera.Enabled = true;
    if (!raycastCamera.EnableRaycast) raycastCamera.EnableRaycast = true;

    // Check if the camera is ready to scan
    if (raycastCamera.CanScan(raycastDistance))
    {
        // Perform raycast directly downwards (pitch 0, yaw 0 relative to camera's orientation)
        MyDetectedEntityInfo hitInfo = raycastCamera.Raycast(raycastDistance, 0, 0);

        System.Text.StringBuilder display = new System.Text.StringBuilder();
        display.AppendLine("--- Raycast Scan Results ---");
        display.AppendLine("Scan Rate: " + updateRate.ToString()); // Use ToString() for enum
        display.AppendLine("Camera: " + raycastCamera.CustomName);
        display.AppendLine("Max Distance: " + raycastDistance.ToString("F1") + "m"); // Using F1 for one decimal place
        display.AppendLine("-----------------------------");

        if (!hitInfo.IsEmpty())
        {
            display.AppendLine("Hit Detected!");
            
            // Refactored problematic lines
            string distanceStr = "N/A";
            if (hitInfo.HitPosition.HasValue)
            {
                distanceStr = Vector3D.Distance(raycastCamera.GetPosition(), hitInfo.HitPosition.Value).ToString("F2"); // F2 for two decimal places
            }
            display.AppendLine("Distance: " + distanceStr + "m");

            display.AppendLine("Entity Name: " + hitInfo.Name);
            display.AppendLine("Entity Type: " + hitInfo.Type.ToString()); // Use ToString() for enum type
            
            if (hitInfo.HitPosition.HasValue)
            {
                // Refactored hit position display
                display.AppendLine("Hit Pos: X:" + hitInfo.HitPosition.Value.X.ToString("F1") + 
                                 " Y:" + hitInfo.HitPosition.Value.Y.ToString("F1") + 
                                 " Z:" + hitInfo.HitPosition.Value.Z.ToString("F1"));
            }
            
            if (hitInfo.Velocity.LengthSquared() > 0.01) // Only show velocity if significant
            {
                 // Refactored velocity display
                 display.AppendLine("Velocity: " + hitInfo.Velocity.Length().ToString("F2") + " m/s");
            }
            
            if (hitInfo.Type == MyDetectedEntityType.FloatingObject || hitInfo.Type == MyDetectedEntityType.SmallGrid || hitInfo.Type == MyDetectedEntityType.LargeGrid)
            {
                // Refactored grid speed display
                display.AppendLine("Grid Speed: " + hitInfo.Velocity.Length().ToString("F1") + " m/s");
            }
        }
        else
        {
            display.AppendLine("No hit detected within range.");
            display.AppendLine("-----------------------------");
        }
        
        outputLcd.WriteText(display.ToString(), false);
        Echo("Raycast scan updated on LCD.");
    }
    else
    {
        string status = "Camera '" + cameraName + "' not ready for scan.";
        Echo("Warning: " + status);
        if (outputLcd != null) outputLcd.WriteText(status + "\n(Waiting for cooldown...)", false);
    }
}

void InitializeBlocks()
{
    raycastCamera = GridTerminalSystem.GetBlockWithName(cameraName) as IMyCameraBlock;
    if (raycastCamera != null)
    {
        Echo("Camera '" + cameraName + "' found.");
        raycastCamera.Enabled = true;
        raycastCamera.EnableRaycast = true;
    }
    else
    {
        Echo("Warning: Camera '" + cameraName + "' not found. Please ensure it exists and is named correctly.");
    }

    outputLcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextPanel;
    if (outputLcd != null)
    {
        Echo("LCD '" + lcdName + "' found.");
        outputLcd.ContentType = ContentType.TEXT_AND_IMAGE;
        outputLcd.Font = "DEBUG"; // Good for readability
        outputLcd.FontSize = 0.8f;
        outputLcd.Alignment = TextAlignment.LEFT;
        outputLcd.WriteText("Initializing Raycast Display...\n", false);
    }
    else
    {
        Echo("Warning: LCD '" + lcdName + "' not found. Please ensure it exists and is named correctly.");
    }
}