// Hydrogen Engine & Battery Manager - Compact Status Edition

float Const1 = 70.00f;                     // Threshold at which engines turn on (< 70%)
float Const2 = 95.00f;                     // Threshold at which engines turn off (> 95%)

List<IMyBatteryBlock> Batteries;           // List of all batteries
List<IMyPowerProducer> HydroEngines;       // List of all hydrogen engines
string result = "";                        // Variable for recording the engine state (on/off)
string ERR_TXT = "";                       // Variable for outputting error
int tickCounter = 0;                       // Counter to track time between rescans
const int rescanInterval = 600;            // Rescan every 600 ticks (roughly 10 seconds)

public Program()
{
    ScanGrid();
    
    // Configure the Programmable Block's own built-in screen
    var surface = Me.GetSurface(0);
    surface.ContentType = ContentType.TEXT_AND_IMAGE;
    surface.FontSize = 0.8f;
    surface.Font = "Monospace";
    surface.FontColor = new Color(0, 255, 217);
    surface.BackgroundColor = new Color(0, 0, 0);

    Runtime.UpdateFrequency = UpdateFrequency.Update100; // Update every 100 ticks (~1.67 seconds)
}

public void Main(string args)
{
    tickCounter += 100;

    if (tickCounter >= rescanInterval)
    {
        ScanGrid();
        tickCounter = 0; 
    }

    ERR_TXT = "";

    if (HydroEngines.Count == 0)
    {
        ERR_TXT = "Attention!\n\nNo hydrogen engines found on your grid!";
    }

    if (Batteries.Count == 0)
    {
        ERR_TXT = "Attention!\n\nNo batteries found on your ship!";
    }

    var surface = Me.GetSurface(0);

    if (ERR_TXT != "")
    {
        surface.WriteText(ERR_TXT);
        Echo(ERR_TXT);
        return;
    }

    // Calculate average battery charge
    float totalPower = 0;
    float totalMaxPower = 0;

    foreach (var battery in Batteries)
    {
        totalPower += battery.CurrentStoredPower;
        totalMaxPower += battery.MaxStoredPower;
    }

    float avgPower = (totalMaxPower > 0) ? (totalPower / totalMaxPower) * 100 : 0; 
    float inPercent = (float)Math.Round(avgPower, 2);     // Round to two decimal places

    // Start and stop hydrogen engines based on hysteresis thresholds (On < 70%, Off > 95%)
    foreach (var engine in HydroEngines)
    {
        if (inPercent < Const1 && !engine.Enabled)
        {
            engine.ApplyAction("OnOff_On");
        }
        else if (inPercent > Const2 && engine.Enabled)
        {
            engine.ApplyAction("OnOff_Off");
        }
    }

    // Check if the engines are on or off
    bool enginesOn = false;
    foreach (var engine in HydroEngines)
    {
        if (engine.Enabled)
        {
            enginesOn = true;
            break;
        }
    }

    if (enginesOn)
    {
        result = "Charging";
    }
    else
    {
        result = "Standby";
    }

    // Output all runtime info directly to the Programmable Block's screen
    StringBuilder statusOutput = new StringBuilder();
    statusOutput.AppendLine("=== HYDROGEN ENGINE AUTO ===");
    statusOutput.AppendLine($"Battery Charge:  {inPercent:0.00} %");
    statusOutput.AppendLine($"Engines Found:   {HydroEngines.Count}");
    statusOutput.AppendLine($"Batteries Found: {Batteries.Count}");
    statusOutput.AppendLine("--------------------------");
    statusOutput.AppendLine($"Turn-on:         < {Const1} %");
    statusOutput.AppendLine($"Turn-off:        > {Const2} %");
    statusOutput.AppendLine("--------------------------");
    statusOutput.AppendLine($"Status: {result}");

    surface.WriteText(statusOutput.ToString());
    Echo(statusOutput.ToString());
}

public void ScanGrid()
{
    Batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType(Batteries);
    
    HydroEngines = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType(HydroEngines, engine => engine.BlockDefinition.ToString().Contains("HydrogenEngine"));
}