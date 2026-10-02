List<IMyTextPanel> lcds = new List<IMyTextPanel>();
float updateInterval = 10f;
int tickCounter = 0;
int currentTurretIndex = 0;
List<IMyTurretControlBlock> solarTurrets = new List<IMyTurretControlBlock>();
List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyMotorAdvancedStator> hinges = new List<IMyMotorAdvancedStator>();
List<IMyMotorStator> rotors = new List<IMyMotorStator>();

string lcdName = "Solar Tracker LCD";
float hingeMinDeg = -80f;
float hingeMaxDeg = 80f;
bool firstRun = true;

float rotorTorqueIdle = 2f;
float rotorTorqueActive = 7f;
float hingeTorqueIdle = 2f;
float hingeTorqueActive = 7f;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    Echo("[OK] Solar Diagnostic Initialized");

    SaveDefaultCustomData();
    ParseCustomData();

    GridTerminalSystem.GetBlocksOfType(lcds, l => l.CustomName.Contains(lcdName) && l.IsSameConstructAs(Me));
    foreach (var lcd in lcds)
        lcd.ContentType = ContentType.TEXT_AND_IMAGE;

    List<IMyTurretControlBlock> allTurrets = new List<IMyTurretControlBlock>();
    GridTerminalSystem.GetBlocksOfType(allTurrets, t => t.IsSameConstructAs(Me));
    solarTurrets = allTurrets.Where(t => t.CustomName.Contains("Solar")).ToList();

    GridTerminalSystem.GetBlocksOfType(batteries, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(hinges, h => h.CustomName.Contains("Solar") && h.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(rotors, r => r.CustomName.Contains("Solar") && r.IsSameConstructAs(Me));

    float hingeMinRad = MathHelper.ToRadians(hingeMinDeg);
    float hingeMaxRad = MathHelper.ToRadians(hingeMaxDeg);
    foreach (var hinge in hinges)
    {
        hinge.LowerLimitRad = hingeMinRad;
        hinge.UpperLimitRad = hingeMaxRad;
        hinge.Torque = hingeTorqueIdle;
    }

    foreach (var rotor in rotors)
    {
        rotor.Torque = rotorTorqueIdle;
    }

    if (solarTurrets.Count == 0)
        Echo("[WARN] No solar turrets found.");
}

public void Main(string argument, UpdateType updateSource)
{
    tickCounter++;
    int intervalTicks = (int)(updateInterval * 6);
    if (!firstRun && (tickCounter % intervalTicks != 0 || solarTurrets.Count == 0)) return;
    firstRun = false;

    string fullStatus = "";
    List<string> diagnostics = new List<string>();
    int displayed = 0;
    int shown = 0;
    int startIndex = currentTurretIndex;

    while (displayed < 3 && shown < solarTurrets.Count)
    {
        if (currentTurretIndex >= solarTurrets.Count)
            currentTurretIndex = 0;

        var turret = solarTurrets[currentTurretIndex++];
        shown++;

        if (turret == null) continue;

        List<IMySolarPanel> panels = new List<IMySolarPanel>();
        GridTerminalSystem.GetBlocksOfType(panels, p => p.IsSameConstructAs(turret));

        if (panels.Count == 0)
        {
            diagnostics.Add($"[WARN] No solar panels found for {turret.CustomName}");
            continue;
        }

        var azimuth = turret.AzimuthRotor;
        var elevation = turret.ElevationRotor;

        if (azimuth == null)
            diagnostics.Add($"[WARN] Missing azimuth rotor on {turret.CustomName}");
        else
            azimuth.Torque = rotorTorqueActive;

        if (elevation == null)
            diagnostics.Add($"[WARN] Missing elevation hinge on {turret.CustomName}");
        else
            elevation.Torque = hingeTorqueActive;

        float totalOutput = panels.Sum(p => p.CurrentOutput);
        float maxOutput = panels.Sum(p => p.MaxOutput);
        float efficiency = (maxOutput > 0f) ? (totalOutput / maxOutput) * 100f : 0f;

        fullStatus += $"[Turret: {turret.CustomName}]\n";
        fullStatus += $"Current Output: {totalOutput:F2} kW\n";
        fullStatus += $"Max Output: {maxOutput:F2} kW\n";
        fullStatus += $"Efficiency: {efficiency:F1}%\n";

        displayed++;
    }

    if (diagnostics.Count > 0)
        fullStatus += "Diagnostics:\n" + string.Join("\n", diagnostics) + "\n";

    Echo(fullStatus);
    foreach (var lcd in lcds)
        lcd.WriteText(fullStatus);

    List<IMyTextPanel> turretLCDs = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(turretLCDs, l => l.CustomName.Contains(lcdName));
    foreach (var lcd in turretLCDs)
    {
        lcd.ContentType = ContentType.TEXT_AND_IMAGE;
        lcd.WriteText(fullStatus);
    }
}

void ParseCustomData()
{
    var config = Me.CustomData.Split('\n');
    foreach (var line in config)
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith("LCDName="))
            lcdName = trimmed.Substring("LCDName=".Length).Trim();
        else if (trimmed.StartsWith("UpdateInterval="))
            float.TryParse(trimmed.Substring("UpdateInterval=".Length).Trim(), out updateInterval);
        else if (trimmed.StartsWith("HingeMinDeg="))
            float.TryParse(trimmed.Substring("HingeMinDeg=".Length).Trim(), out hingeMinDeg);
        else if (trimmed.StartsWith("HingeMaxDeg="))
            float.TryParse(trimmed.Substring("HingeMaxDeg=".Length).Trim(), out hingeMaxDeg);
        else if (trimmed.StartsWith("RotorTorqueIdle="))
            float.TryParse(trimmed.Substring("RotorTorqueIdle=".Length).Trim(), out rotorTorqueIdle);
        else if (trimmed.StartsWith("RotorTorqueActive="))
            float.TryParse(trimmed.Substring("RotorTorqueActive=".Length).Trim(), out rotorTorqueActive);
        else if (trimmed.StartsWith("HingeTorqueIdle="))
            float.TryParse(trimmed.Substring("HingeTorqueIdle=".Length).Trim(), out hingeTorqueIdle);
        else if (trimmed.StartsWith("HingeTorqueActive="))
            float.TryParse(trimmed.Substring("HingeTorqueActive=".Length).Trim(), out hingeTorqueActive);
    }
}

void SaveDefaultCustomData()
{
    if (!string.IsNullOrWhiteSpace(Me.CustomData)) return;

    Me.CustomData =
@"LCDName=Solar Tracker LCD
UpdateInterval=10
HingeMinDeg=-80
HingeMaxDeg=80
RotorTorqueIdle=2
RotorTorqueActive=7
HingeTorqueIdle=2
HingeTorqueActive=7";
}
