// ================================================================
// SUN SATELLITE TRACKER v4 - FULL SCRIPT
// Space Engineers Programmable Block script
//
// What it does:
// - Uses solar panel output to find the sun.
// - Uses gyros to rotate the whole satellite.
// - Stops when solar output is at least 95% of the best power found.
// - Turns gyro override OFF when idle.
// - Rechecks every 10 minutes.
// - If power drops below 95% of the saved best power, it searches again.
//
// Setup:
// - Put this in a Programmable Block.
// - Put solar panels and gyros on the same grid.
// - Optional: add "(sun)" to the names of the gyros/panels you want used.
// - Run with argument: start
//
// Commands:
// start  = enable tracker
// stop   = disable tracker and release gyros
// scan   = force new scan now
// reload = reload blocks
// reset  = reset saved best power and scan again
// ================================================================

const string TAG = "(sun)";

// Recheck timing
const double CHECK_INTERVAL_SECONDS = 600.0; // 10 minutes

// Search trigger and stop threshold
const double RECHECK_THRESHOLD = 0.95;       // scan if current power drops below 95% of saved best
const double STOP_AT_BEST_PERCENT = 0.95;   // stop scan once current power is 95% of best found in scan

// Gyro movement
const float GYRO_SPEED_FAST = 0.18f;
const float GYRO_SPEED_SLOW = 0.06f;

// Scan timing
const double SAMPLE_SECONDS = 1.5;
const int MAX_PHASE_STEPS = 24;

// If power changes less than this, treat it as no real improvement
const double POWER_DEADZONE_MW = 0.0005;

// Safety: after this many scan phases, stop anyway
const int MAX_TOTAL_PHASES = 20;

List<IMyGyro> gyros = new List<IMyGyro>();
List<IMySolarPanel> panels = new List<IMySolarPanel>();

bool enabled = false;
bool scanning = false;

double savedBestPower = 0.0;
double scanBestPower = 0.0;
double lastSamplePower = 0.0;

double checkTimer = 0.0;
double sampleTimer = 0.0;

int phase = 0;
int phaseStep = 0;
int totalPhases = 0;

// phase meaning:
// 0 yaw positive
// 1 yaw negative
// 2 pitch positive
// 3 pitch negative
// then repeat slower

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    LoadBlocks();
}

public void Main(string argument, UpdateType updateSource)
{
    argument = (argument ?? "").Trim().ToLower();

    if (argument == "reload")
    {
        LoadBlocks();
    }
    else if (argument == "start")
    {
        enabled = true;
        StartScan();
    }
    else if (argument == "stop")
    {
        enabled = false;
        scanning = false;
        StopGyros();
    }
    else if (argument == "scan")
    {
        enabled = true;
        StartScan();
    }
    else if (argument == "reset")
    {
        savedBestPower = 0.0;
        enabled = true;
        StartScan();
    }

    if (!enabled)
    {
        StopGyros();
        PrintStatus("OFF");
        return;
    }

    double dt = Runtime.TimeSinceLastRun.TotalSeconds;
    double currentPower = GetPower();

    if (savedBestPower <= 0.0 && currentPower > 0.0)
        savedBestPower = currentPower;

    if (scanning)
    {
        RunScan(dt, currentPower);
    }
    else
    {
        StopGyros();

        checkTimer += dt;

        bool timeToCheck = checkTimer >= CHECK_INTERVAL_SECONDS;
        bool powerDropped = savedBestPower > 0.0 && currentPower < savedBestPower * RECHECK_THRESHOLD;

        if (timeToCheck)
        {
            checkTimer = 0.0;

            if (powerDropped)
            {
                StartScan();
            }
        }
    }

    PrintStatus(scanning ? "SCANNING" : "IDLE");
}

void LoadBlocks()
{
    gyros.Clear();
    panels.Clear();

    var taggedGyros = new List<IMyGyro>();
    var taggedPanels = new List<IMySolarPanel>();

    GridTerminalSystem.GetBlocksOfType(taggedGyros, g =>
        g.CubeGrid == Me.CubeGrid &&
        g.CustomName.ToLower().Contains(TAG));

    GridTerminalSystem.GetBlocksOfType(taggedPanels, p =>
        p.CubeGrid == Me.CubeGrid &&
        p.CustomName.ToLower().Contains(TAG));

    if (taggedGyros.Count > 0)
        gyros.AddRange(taggedGyros);
    else
        GridTerminalSystem.GetBlocksOfType(gyros, g => g.CubeGrid == Me.CubeGrid);

    if (taggedPanels.Count > 0)
        panels.AddRange(taggedPanels);
    else
        GridTerminalSystem.GetBlocksOfType(panels, p => p.CubeGrid == Me.CubeGrid);

    savedBestPower = Math.Max(savedBestPower, GetPower());
}

void StartScan()
{
    scanning = true;
    checkTimer = 0.0;
    sampleTimer = 0.0;

    phase = 0;
    phaseStep = 0;
    totalPhases = 0;

    scanBestPower = GetPower();
    lastSamplePower = scanBestPower;

    if (savedBestPower <= 0.0)
        savedBestPower = scanBestPower;
}

void RunScan(double dt, double currentPower)
{
    if (gyros.Count == 0 || panels.Count == 0)
    {
        scanning = false;
        StopGyros();
        return;
    }

    if (currentPower > scanBestPower)
        scanBestPower = currentPower;

    if (currentPower > savedBestPower)
        savedBestPower = currentPower;

    // Stop early when we are already close enough to best power found.
    // This prevents endless hunting.
    if (scanBestPower > 0.0 &&
        currentPower >= scanBestPower * STOP_AT_BEST_PERCENT &&
        totalPhases >= 4)
    {
        scanning = false;
        StopGyros();
        savedBestPower = Math.Max(savedBestPower, scanBestPower);
        return;
    }

    sampleTimer += dt;

    float speed = totalPhases < 8 ? GYRO_SPEED_FAST : GYRO_SPEED_SLOW;
    ApplyPhaseGyro(phase, speed);

    if (sampleTimer < SAMPLE_SECONDS)
        return;

    sampleTimer = 0.0;
    phaseStep++;

    double improvement = currentPower - lastSamplePower;

    // If power got worse or didn't improve enough, move to next axis/direction.
    if (improvement < POWER_DEADZONE_MW || phaseStep >= MAX_PHASE_STEPS)
    {
        phase = (phase + 1) % 4;
        phaseStep = 0;
        totalPhases++;
    }

    lastSamplePower = currentPower;

    // Safety stop
    if (totalPhases >= MAX_TOTAL_PHASES)
    {
        scanning = false;
        StopGyros();
        savedBestPower = Math.Max(savedBestPower, scanBestPower);
    }
}

void ApplyPhaseGyro(int p, float speed)
{
    foreach (var g in gyros)
    {
        g.GyroOverride = true;
        g.GyroPower = 1f;

        g.Pitch = 0f;
        g.Yaw = 0f;
        g.Roll = 0f;

        if (p == 0) g.Yaw = speed;
        if (p == 1) g.Yaw = -speed;
        if (p == 2) g.Pitch = speed;
        if (p == 3) g.Pitch = -speed;
    }
}

void StopGyros()
{
    foreach (var g in gyros)
    {
        g.Pitch = 0f;
        g.Yaw = 0f;
        g.Roll = 0f;
        g.GyroOverride = false;
    }
}

double GetPower()
{
    double total = 0.0;

    foreach (var p in panels)
        total += p.MaxOutput;

    return total;
}

void PrintStatus(string state)
{
    double currentPower = GetPower();
    double percent = 0.0;

    if (savedBestPower > 0.0)
        percent = currentPower / savedBestPower * 100.0;

    Echo("SUN SATELLITE TRACKER v4");
    Echo("State: " + state);
    Echo("Enabled: " + enabled);
    Echo("Gyros: " + gyros.Count);
    Echo("Panels: " + panels.Count);
    Echo("");
    Echo("Power: " + currentPower.ToString("0.0000") + " MW");
    Echo("Saved Best: " + savedBestPower.ToString("0.0000") + " MW");
    Echo("Current %: " + percent.ToString("0.0") + "%");
    Echo("");
    Echo("Recheck: " + Math.Max(0, CHECK_INTERVAL_SECONDS - checkTimer).ToString("0") + " sec");
    Echo("Trigger: below 95%");
    Echo("");
    Echo("Commands:");
    Echo("start / stop / scan / reload / reset");
}
