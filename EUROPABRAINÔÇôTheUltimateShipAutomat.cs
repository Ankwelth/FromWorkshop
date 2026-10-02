// =======================================
// EUROPA BRAIN – FULL POWER & FUEL LCD
// Space Engineers PB SAFE
// =======================================

// -------- Ship Core --------
IMyShipController controller;

// -------- Block Lists --------
List<IMyThrust> thrusters = new List<IMyThrust>();
List<IMyDoor> doors = new List<IMyDoor>();
List<IMySensorBlock> sensors = new List<IMySensorBlock>();
List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyReactor> reactors = new List<IMyReactor>();
List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();

// -------- LCDs --------
IMyTextPanel lcdDebug;
IMyTextPanel lcdStatus;
IMyTextPanel lcdPower;

// -------- State --------
int tick = 0;
string lastActivity = "Idle";
bool attackMode = false;

// =======================================
// PROGRAM START
// =======================================
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    SetupBlocks();
    SetupLCDs();
}

// =======================================
// MAIN LOOP
// =======================================
public void Main(string argument)
{
    if (argument == "ATTACK_ON") attackMode = true;
    else if (argument == "ATTACK_OFF") attackMode = false;

    HandleSensors();
    ApplyAttackMode();

    lastActivity = "Updating systems";
    UpdateLCDs();
}

// =======================================
// BLOCK DISCOVERY
// =======================================
void SetupBlocks()
{
    controller = null;
    thrusters.Clear();
    doors.Clear();
    sensors.Clear();
    batteries.Clear();
    reactors.Clear();
    hydrogenTanks.Clear();

    List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(allBlocks);

    for (int i = 0; i < allBlocks.Count; i++)
    {
        IMyTerminalBlock b = allBlocks[i];
        if (b.CubeGrid != Me.CubeGrid) continue;

        if (controller == null)
        {
            IMyShipController sc = b as IMyShipController;
            if (sc != null) controller = sc;
        }

        IMyThrust t = b as IMyThrust;
        if (t != null) thrusters.Add(t);

        IMyDoor d = b as IMyDoor;
        if (d != null) doors.Add(d);

        IMySensorBlock s = b as IMySensorBlock;
        if (s != null) sensors.Add(s);

        IMyBatteryBlock bat = b as IMyBatteryBlock;
        if (bat != null) batteries.Add(bat);

        IMyReactor r = b as IMyReactor;
        if (r != null) reactors.Add(r);

        IMyGasTank g = b as IMyGasTank;
        if (g != null && g.BlockDefinition.SubtypeName.Contains("Hydrogen")) hydrogenTanks.Add(g);
    }
}

// =======================================
// LCD DISCOVERY (NAME CONTAINS)
// =======================================
void SetupLCDs()
{
    lcdDebug = null;
    lcdStatus = null;
    lcdPower = null;

    List<IMyTextPanel> panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels);

    for (int i = 0; i < panels.Count; i++)
    {
        IMyTextPanel p = panels[i];
        if (p.CubeGrid != Me.CubeGrid) continue;

        string name = p.CustomName.ToUpper();

        if (!name.Contains("EUROPA")) continue;

        if (name.Contains("DEBUG")) lcdDebug = p;
        else if (name.Contains("STATUS")) lcdStatus = p;
        else if (name.Contains("POWER")) lcdPower = p;
    }

    if (lcdDebug != null) lcdDebug.ContentType = ContentType.TEXT_AND_IMAGE;
    if (lcdStatus != null) lcdStatus.ContentType = ContentType.TEXT_AND_IMAGE;
    if (lcdPower != null) lcdPower.ContentType = ContentType.TEXT_AND_IMAGE;
}

// =======================================
// SENSOR → DOOR LOGIC
// =======================================
void HandleSensors()
{
    bool detected = false;
    for (int i = 0; i < sensors.Count; i++)
    {
        if (sensors[i].IsActive)
        {
            detected = true;
            break;
        }
    }

    for (int j = 0; j < doors.Count; j++)
    {
        if (detected)
        {
            if (doors[j].Status != DoorStatus.Open) doors[j].OpenDoor();
        }
        else
        {
            if (doors[j].Status != DoorStatus.Closed) doors[j].CloseDoor();
        }
    }
}

// =======================================
// ATTACK MODE (SAFE PLACEHOLDER)
// =======================================
void ApplyAttackMode()
{
    for (int i = 0; i < thrusters.Count; i++)
        thrusters[i].Enabled = true;
}

// =======================================
// LCD OUTPUT
// =======================================
void UpdateLCDs()
{
    tick++;

    // ---------- DEBUG LCD ----------
    if (lcdDebug != null)
    {
        lcdDebug.WriteText(
            "EUROPA BRAIN\nSTATE: RUNNING\nTICK: " + tick +
            "\nACTIVITY:\n" + lastActivity
        );
    }

    // ---------- STATUS LCD ----------
    if (lcdStatus != null)
    {
        lcdStatus.WriteText(
            "SHIP STATUS\n" +
            "Controller: " + (controller != null ? "OK" : "MISSING") + "\n" +
            "Thrusters: " + thrusters.Count + "\n" +
            "Doors: " + doors.Count + "\n" +
            "Sensors: " + sensors.Count + "\n" +
            "Attack Mode: " + (attackMode ? "ON" : "OFF")
        );
    }

    // ---------- POWER LCD (All-in-one) ----------
    if (lcdPower != null)
    {
        double batteryCurrent = 0;
        double batteryMax = 0;
        for (int i = 0; i < batteries.Count; i++)
        {
            batteryCurrent += batteries[i].CurrentStoredPower;
            batteryMax += batteries[i].MaxStoredPower;
        }
        double batteryPercent = batteryMax > 0 ? (batteryCurrent / batteryMax) * 100 : 0;

        int reactorsOn = 0;
        for (int i = 0; i < reactors.Count; i++)
        {
            if (reactors[i].IsWorking) reactorsOn++;
        }

        double uraniumTotal = 0;
        for (int i = 0; i < reactors.Count; i++)
        {
            uraniumTotal += reactors[i].CurrentOutput; // placeholder for real fuel, PB limitation
        }

        double hydrogenPercent = 0;
        double hydrogenMax = 0;
        double hydrogenCurrent = 0;
        for (int i = 0; i < hydrogenTanks.Count; i++)
        {
            hydrogenCurrent += hydrogenTanks[i].FilledRatio * hydrogenTanks[i].Capacity;
            hydrogenMax += hydrogenTanks[i].Capacity;
        }
        hydrogenPercent = hydrogenMax > 0 ? (hydrogenCurrent / hydrogenMax) * 100 : 0;

        lcdPower.WriteText(
            "POWER CORE STATUS\n" +
            "Batteries: " + Math.Round(batteryCurrent, 1) + "/" + Math.Round(batteryMax, 1) +
            " MWh (" + Math.Round(batteryPercent, 1) + "%)\n" +
            "Reactors: " + reactorsOn + "/" + reactors.Count + " ON\n" +
            "Uranium: " + Math.Round(uraniumTotal, 1) + " (raw output)\n" +
            "Hydrogen: " + Math.Round(hydrogenPercent, 1) + "%"
        );
    }
}
