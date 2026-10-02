List<IMySolarPanel> solarPanels;
List<IMyPowerProducer> windTurbines;
List<IMyReactor> reactors;
List<IMyPowerProducer> hydrogenEngines;
List<IMyBatteryBlock> batteries;
bool blocksInitialized = false;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100; // Initial update frequency
    InitializeBlocks(); // Initialize blocks on first run
}

void InitializeBlocks()
{
    solarPanels = new List<IMySolarPanel>();
    windTurbines = new List<IMyPowerProducer>();
    reactors = new List<IMyReactor>();
    hydrogenEngines = new List<IMyPowerProducer>();
    batteries = new List<IMyBatteryBlock>();

    GridTerminalSystem.GetBlocksOfType(solarPanels);
    GridTerminalSystem.GetBlocksOfType(windTurbines, turbine => turbine.BlockDefinition.SubtypeName.Contains("Wind"));
    GridTerminalSystem.GetBlocksOfType(reactors);
    GridTerminalSystem.GetBlocksOfType(hydrogenEngines, engine => engine.BlockDefinition.SubtypeName.Contains("HydrogenEngine"));
    GridTerminalSystem.GetBlocksOfType(batteries);

    blocksInitialized = true;
}

public void Main(string argument, UpdateType updateSource)
{
    if (!blocksInitialized || argument.Trim().ToLower() == "reset") {
        InitializeBlocks(); // Reinitialize blocks if needed
    }

    try
    {
        string statusText = CreateStatusText(); // Generate the status text
        UpdatePowerStatus();
        Echo(statusText); // Display status text in programmable block's console
        UpdateLCD(statusText); // Update the LCD with the generated status text
    }
    catch (Exception e)
    {
        Echo("An error occurred: " + e.Message);
        InitializeBlocks(); // Attempt to reinitialize blocks if an error occurs
    }
}

void UpdatePowerStatus()
{
    double solarOutput = solarPanels.Sum(panel => panel.CurrentOutput);
    double windOutput = windTurbines.Sum(turbine => turbine.CurrentOutput);
    double reactorOutput = reactors.Sum(reactor => reactor.CurrentOutput);
    double hydrogenOutput = hydrogenEngines.Sum(engine => engine.CurrentOutput);
    double totalOutput = solarOutput + windOutput + reactorOutput + hydrogenOutput;

    // Calculate battery input as a proxy for current demand. This assumes all power to batteries is from consumption and not storage.
    double batteryInput = batteries.Sum(battery => battery.CurrentInput);  // Total power input into batteries

    // Assuming that all devices consuming power are batteries might be incorrect, but without additional APIs, this is a usable approximation.
    double currentDemand = batteryInput;

    if (solarOutput + windOutput >= currentDemand)
    {
        SetBlocksEnabled(reactors, false);
        SetBlocksEnabled(hydrogenEngines, false);
        Echo("Status: Running on renewable sources.");
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }
    else
    {
        double deficit = currentDemand - (solarOutput + windOutput);
        AdjustPowerUnits(reactors, deficit);
        AdjustPowerUnits(hydrogenEngines, deficit - reactors.Sum(r => r.CurrentOutput));

        if (deficit > 0)
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }
        else
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }
    }
}




void ManagePowerSources(double solarOutput, double windOutput, double reactorOutput, double hydrogenOutput, double currentDemand)
{
    if (solarOutput + windOutput >= currentDemand)
    {
        SetBlocksEnabled(reactors, false);
        SetBlocksEnabled(hydrogenEngines, false);
        Echo("Status: Running on renewable sources.");
    }
    else
    {
        double deficit = currentDemand - (solarOutput + windOutput);
        AdjustPowerUnits(reactors, deficit);
        AdjustPowerUnits(hydrogenEngines, deficit - reactors.Sum(r => r.CurrentOutput));
    }
}

void SetBlocksEnabled<T>(List<T> blocks, bool enabled) where T : IMyFunctionalBlock
{
    foreach (var block in blocks)
    {
        block.Enabled = enabled;
    }
}

void AdjustPowerUnits<T>(List<T> blocks, double deficit) where T : IMyFunctionalBlock, IMyPowerProducer
{
    foreach (var block in blocks.OrderBy(x => x.CustomName))
    {
        double blockOutputPotential = block.MaxOutput;
        // Enable block if there's a deficit and the block isn't already enabled
        if (deficit > 0 && !block.Enabled)
        {
            block.Enabled = true;
            deficit -= blockOutputPotential;
        }
    }
    
    // Disable blocks only if there's no deficit and after checking all blocks
    if (deficit <= 0)
    {
        foreach (var block in blocks.Where(b => b.Enabled))
        {
            double excessPower = -deficit + block.MaxOutput; // Calculate excess power if this block were disabled
            if (excessPower >= 0)
            {
                block.Enabled = false; // Disable the block only if turning it off won't create a new deficit
                deficit += block.MaxOutput;
            }
        }
    }
    
    if (deficit > 0)
    {
        Echo("Warning: Not enough power available. Deficit remains at " + deficit.ToString("F2") + " MW.");
    }
}





string CreateStatusText()
{
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Power Status Report:");
    sb.AppendLine($"- Solar Panels: {solarPanels.Count}, Output: {solarPanels.Sum(panel => panel.CurrentOutput):0.00} MW");
    sb.AppendLine($"- Wind Turbines: {windTurbines.Count}, Output: {windTurbines.Sum(turbine => turbine.CurrentOutput):0.00} MW");
    sb.AppendLine($"- Reactors: {reactors.Count}, Output: {reactors.Sum(reactor => reactor.CurrentOutput):0.00} MW");
    sb.AppendLine($"- Hydrogen Engines: {hydrogenEngines.Count}, Output: {hydrogenEngines.Sum(engine => engine.CurrentOutput):0.00} MW");
    sb.AppendLine($"- Total Power Output: {solarPanels.Sum(panel => panel.CurrentOutput) + windTurbines.Sum(turbine => turbine.CurrentOutput) + reactors.Sum(reactor => reactor.CurrentOutput) + hydrogenEngines.Sum(engine => engine.CurrentOutput):0.00} MW");
    sb.AppendLine($"- Current Power Demand: {solarPanels.Sum(panel => panel.CurrentOutput) + windTurbines.Sum(turbine => turbine.CurrentOutput) + reactors.Sum(reactor => reactor.CurrentOutput) + hydrogenEngines.Sum(engine => engine.CurrentOutput):0.00} MW");
    sb.AppendLine($"- Stored Power: {batteries.Sum(battery => battery.CurrentStoredPower):0.00} MWh / {batteries.Sum(battery => battery.MaxStoredPower):0.00} MWh");
    return sb.ToString();
}

void UpdateLCD(string text)
{
    var lcd = GridTerminalSystem.GetBlockWithName("[Power]") as IMyTextPanel;
    if (lcd != null)
    {
        string[] lines = text.Split('\n');
        string displayText = string.Join("\n", lines.Take(8)); // Display the first 8 lines of text

        lcd.WriteText(displayText, false);
        lcd.ContentType = ContentType.TEXT_AND_IMAGE;
        lcd.FontSize = 1.8f; // Smaller font size for better readability
        lcd.FontColor = Color.White;
        lcd.BackgroundColor = Color.Black;
    }
}

