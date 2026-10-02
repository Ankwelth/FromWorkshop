// === Ship Status Monitor with Mass Debugging ===
// Displays power, hydrogen, cargo, ammo, and detailed mass
// Compatible with C# 6 (Space Engineers)

string lcdName = "Power LCD";

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    var lcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextPanel;
    if (lcd == null)
    {
        Echo("LCD not found");
        return;
    }

    // === Battery Stats ===
    var batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType(batteries, b => b.IsSameConstructAs(Me));
    float batteryStored = 0f, batteryMax = 0f, batteryInput = 0f, batteryOutput = 0f;
    foreach (var bat in batteries)
    {
        batteryStored += bat.CurrentStoredPower;
        batteryMax += bat.MaxStoredPower;
        batteryInput += bat.CurrentInput;
        batteryOutput += bat.CurrentOutput;
    }

    float netChange = batteryInput - batteryOutput;
    string timeEstText = "";

    if (Math.Abs(netChange) > 0.01f)
    {
        double hours = 0;
        if (netChange > 0)
        {
            hours = (batteryMax - batteryStored) / netChange;
            timeEstText = $"Est:     {FormatTime(hours)} to Full";
        }
        else
        {
            hours = batteryStored / Math.Abs(netChange);
            timeEstText = $"Est:     {FormatTime(hours)} to Empty";
        }
    }
    else
    {
        timeEstText = "Est:     Stable";
    }

    // === Hydrogen Tanks ===
    var tanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType(tanks, t => t.IsSameConstructAs(Me) && t.DefinitionDisplayNameText.Contains("Hydrogen"));
    float hydroStored = 0f, hydroMax = 0f;
    foreach (var t in tanks)
    {
        hydroStored += (float)(t.FilledRatio * t.Capacity);
        hydroMax += (float)t.Capacity;
    }

    // === Hydrogen Usage Rate ===
    double hydroUseRate = 0;
    var thrusters = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType(thrusters, t => t.IsSameConstructAs(Me) && t.BlockDefinition.SubtypeName.Contains("Hydrogen"));
    foreach (var thruster in thrusters)
    {
        if (thruster.IsWorking && thruster.CurrentThrust > 0)
        {
            hydroUseRate += 0.000014 * thruster.CurrentThrust;
        }
    }

    var engines = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType(engines, e => e.IsSameConstructAs(Me) && e.BlockDefinition.SubtypeName.Contains("HydrogenEngine"));
    foreach (var engine in engines)
    {
        if (engine.IsWorking)
        {
            hydroUseRate += 0.5;
        }
    }

    string hydroTimeText = "Hydro:   Stable";
    if (hydroUseRate > 0)
    {
        double seconds = hydroStored / hydroUseRate;
        hydroTimeText = $"Hydro:   {FormatTime(seconds / 3600)} left";
    }

    // === Ice and Cargo Stats ===
    var blocksWithInventory = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocksWithInventory, b => b.HasInventory && b.IsSameConstructAs(Me));
    double totalIceKg = 0;
    double cargoVolCurrent = 0;
    double cargoVolMax = 0;

    foreach (var block in blocksWithInventory)
    {
        var inv = block.GetInventory();
        var items = new List<MyInventoryItem>();
        inv.GetItems(items);
        cargoVolCurrent += (double)inv.CurrentVolume * 1000;
        cargoVolMax += (double)inv.MaxVolume * 1000;

        foreach (var item in items)
        {
            if (item.Type.ToString().Contains("Ice"))
                totalIceKg += (double)item.Amount;
        }
    }

    double cargoFillPercent = cargoVolMax > 0 ? (cargoVolCurrent / cargoVolMax) * 100 : 0;

    // === Ship Mass ===
    double baseMass = 0;
    double totalMass = 0;
    double payloadMass = 0;

    var controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, c => c.IsSameConstructAs(Me));

    if (controllers.Count > 0)
    {
        var mainController = controllers.FirstOrDefault(c => c.IsMainCockpit) ?? controllers[0];
        var massData = mainController.CalculateShipMass();
        baseMass = massData.BaseMass;
        totalMass = massData.TotalMass;
        payloadMass = totalMass - baseMass;
    }

    // === Ammo Counts ===
    var turrets = new List<IMyUserControllableGun>();
    GridTerminalSystem.GetBlocksOfType(turrets, t => t.HasInventory && t.IsSameConstructAs(Me));
    Dictionary<string, double> ammoCounts = new Dictionary<string, double>();
    foreach (var turret in turrets)
    {
        var inv = turret.GetInventory();
        var items = new List<MyInventoryItem>();
        inv.GetItems(items);

        foreach (var item in items)
        {
            string ammoName = item.Type.SubtypeId;
            double amount = (double)item.Amount;

            if (ammoCounts.ContainsKey(ammoName))
                ammoCounts[ammoName] += amount;
            else
                ammoCounts[ammoName] = amount;
        }
    }

    // === Display Output ===
    var sb = new StringBuilder();
    sb.AppendLine("=== Power ===");
    sb.AppendLine($"Bat %:    {(batteryStored / batteryMax * 100):F0}% ({batteryStored:F1}/{batteryMax:F1} MWh)");
    sb.AppendLine(timeEstText);

    sb.AppendLine($"\nHydro %: {(hydroStored / hydroMax * 100):F0}% ({hydroStored:F0}/{hydroMax:F0} L)");
    sb.AppendLine(hydroTimeText);
    sb.AppendLine($"Ice:     {totalIceKg:N0} kg");

    sb.AppendLine("\n=== Cargo ===");
    sb.AppendLine($"Vol:     {cargoVolCurrent:N0} / {cargoVolMax:N0} L");
    sb.AppendLine($"Fill:    {cargoFillPercent:F0}%");

    sb.AppendLine("\n=== Mass ===");
    sb.AppendLine($"Base:    {baseMass:N0} kg");
    sb.AppendLine($"Total:   {totalMass:N0} kg");
    sb.AppendLine($"Payload: {payloadMass:N0} kg");

    sb.AppendLine("\n=== Ammo ===");
    if (ammoCounts.Count == 0)
    {
        sb.AppendLine("No ammo detected.");
    }
    else
    {
        foreach (var kvp in ammoCounts)
        {
            sb.AppendLine($"{kvp.Key,-10}: {kvp.Value:N0}");
        }
    }

    lcd.WriteText(sb.ToString());
}

// === Format time as HH:MM:SS ===
string FormatTime(double hours)
{
    int totalSec = (int)(hours * 3600);
    int h = totalSec / 3600;
    int m = (totalSec % 3600) / 60;
    int s = totalSec % 60;
    return $"{h:D2}:{m:D2}:{s:D2}";
}
