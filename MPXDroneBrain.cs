/*
========================================
MPX DEFENSE SYSTEM - DRONE BRAIN
2 Recorder Blocks Version
========================================

REQUIRED DRONE BLOCKS
---------------------
- 1 Programmable Block
- 1 Antenna
- 1 Connector
- 1 Remote Control
- 1 AI Flight block
- 1 AI Offensive block
- 2 AI Recorder blocks:
  - Launch Recorder
  - Return Recorder
- 2 Timer Blocks:
  - Launch Timer
  - Return Timer
- Battery group
- Thruster group
- Gyro group
- Light group
- Weapon group

IMPORTANT
---------
This script does NOT directly control the recorder blocks.
It triggers the timer blocks.

Launch Timer should trigger:
- AI Recorder Launch -> Play

Return Timer should trigger:
- AI Recorder Return -> Play
*/


// ========================================
// CONFIG - USER EDITABLE PER DRONE
// ========================================
string GUARD_NAME = "Guard 1";
string GUARD_TYPE = "Hover";

string ANTENNA_NAME      = "MPX Guard 1 Antenna";
string CONNECTOR_NAME    = "MPX Guard 1 Dock";
string RC_NAME           = "MPX Guard 1 RC";
string FLIGHT_NAME       = "MPX Guard 1 Flight";
string OFFENSIVE_NAME    = "MPX Guard 1 Offensive";
string LAUNCH_TIMER_NAME = "MPX Guard 1 Launch Timer";
string RETURN_TIMER_NAME = "MPX Guard 1 Return Timer";

string BATTERY_GROUP  = "MPX Guard 1 Batteries";
string THRUSTER_GROUP = "MPX Guard 1 Thrusters";
string GYRO_GROUP     = "MPX Guard 1 Gyros";
string LIGHT_GROUP    = "MPX Guard 1 Lights";
string WEAPON_GROUP   = "MPX Guard 1 Weapons";

string CMD_TAG    = "MPX_CMD";
string STATUS_TAG = "MPX_STATUS";

float IDLE_ANTENNA_RANGE  = 200f;
float ALERT_ANTENNA_RANGE = 50000f;

double LOW_BATTERY_PCT = 0.15;
double THRUSTER_LOSS_RETURN_PCT = 0.25;
double WEAPON_LOSS_RETURN_PCT = 0.50;
// ========================================


// ========================================
// BLOCK REFERENCES
// ========================================
IMyRadioAntenna antenna;
IMyShipConnector connector;
IMyRemoteControl rc;
IMyFunctionalBlock flight;
IMyFunctionalBlock offensive;
IMyTimerBlock launchTimer;
IMyTimerBlock returnTimer;

List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyThrust> thrusters = new List<IMyThrust>();
List<IMyGyro> gyros = new List<IMyGyro>();
List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
List<IMyTerminalBlock> weapons = new List<IMyTerminalBlock>();

IMyBroadcastListener cmdListener;


// ========================================
// STATE
// ========================================
const string STATE_DOCKED    = "DOCKED";
const string STATE_LAUNCHED  = "LAUNCHED";
const string STATE_AIRBORNE  = "AIRBORNE";
const string STATE_RETURNING = "RETURNING";
const string STATE_DAMAGED   = "DAMAGED";

string currentState = STATE_DOCKED;
int reportTick = 0;
int dockTryTick = 0;

int initialThrusterCount = 0;
int initialWeaponCount = 0;


// ========================================
// INIT
// ========================================
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

    antenna     = GridTerminalSystem.GetBlockWithName(ANTENNA_NAME) as IMyRadioAntenna;
    connector   = GridTerminalSystem.GetBlockWithName(CONNECTOR_NAME) as IMyShipConnector;
    rc          = GridTerminalSystem.GetBlockWithName(RC_NAME) as IMyRemoteControl;
    flight      = GridTerminalSystem.GetBlockWithName(FLIGHT_NAME) as IMyFunctionalBlock;
    offensive   = GridTerminalSystem.GetBlockWithName(OFFENSIVE_NAME) as IMyFunctionalBlock;
    launchTimer = GridTerminalSystem.GetBlockWithName(LAUNCH_TIMER_NAME) as IMyTimerBlock;
    returnTimer = GridTerminalSystem.GetBlockWithName(RETURN_TIMER_NAME) as IMyTimerBlock;

    var bg = GridTerminalSystem.GetBlockGroupWithName(BATTERY_GROUP);
    if (bg != null) bg.GetBlocksOfType(batteries);

    var tg = GridTerminalSystem.GetBlockGroupWithName(THRUSTER_GROUP);
    if (tg != null) tg.GetBlocksOfType(thrusters);

    var gg = GridTerminalSystem.GetBlockGroupWithName(GYRO_GROUP);
    if (gg != null) gg.GetBlocksOfType(gyros);

    var lg = GridTerminalSystem.GetBlockGroupWithName(LIGHT_GROUP);
    if (lg != null) lg.GetBlocksOfType(lights);

    var wg = GridTerminalSystem.GetBlockGroupWithName(WEAPON_GROUP);
    if (wg != null) wg.GetBlocksOfType(weapons);

    initialThrusterCount = CountAlive(thrusters);
    initialWeaponCount = CountAlive(weapons);

    cmdListener = IGC.RegisterBroadcastListener(CMD_TAG);
    cmdListener.SetMessageCallback(CMD_TAG);

    if (connector != null && connector.Status == MyShipConnectorStatus.Connected)
        SetDockedMode();
    else
        SetAirborneIdleMode();

    ReportStatus();
}


// ========================================
// MAIN LOOP
// ========================================
public void Main(string argument, UpdateType updateSource)
{
    if ((updateSource & UpdateType.IGC) != 0)
    {
        while (cmdListener.HasPendingMessage)
        {
            var msg = cmdListener.AcceptMessage();
            var cmd = msg.Data as string;

            if (string.IsNullOrWhiteSpace(cmd))
                continue;

            if (cmd == "LAUNCH")
                LaunchMode();
            else if (cmd == "RETURN")
                ReturnMode();
        }
    }

    AutoStateCheck();

    reportTick++;
    if (reportTick >= 2)
    {
        reportTick = 0;
        ReportStatus();
    }

    Echo("MPX Guard Drone Brain");
    Echo("Guard: " + GUARD_NAME);
    Echo("State: " + currentState);
    Echo("Battery: " + (GetBatteryPercent() * 100.0).ToString("0") + "%");
}


// ========================================
// MODES
// ========================================
void LaunchMode()
{
    dockTryTick = 0;

    if (rc != null)
        rc.SetAutoPilotEnabled(false);

    if (connector != null && connector.Status == MyShipConnectorStatus.Connected)
        connector.Disconnect();

    SetBatteriesAuto();
    SetThrusters(true);
    SetGyros(true);

    SetBlockEnabled(flight, true);
    SetBlockEnabled(offensive, true);

    if (antenna != null)
    {
        antenna.Enabled = true;
        antenna.Radius = ALERT_ANTENNA_RANGE;
    }

    SetLightsActive(true);

    TriggerTimer(launchTimer);

    currentState = STATE_LAUNCHED;
    ReportStatus();
}

void ReturnMode()
{
    dockTryTick = 0;

    SetBatteriesAuto();
    SetThrusters(true);
    SetGyros(true);

    SetBlockEnabled(flight, true);
    SetBlockEnabled(offensive, false);

    if (antenna != null)
    {
        antenna.Enabled = true;
        antenna.Radius = ALERT_ANTENNA_RANGE;
    }

    SetLightsActive(false);

    TriggerTimer(returnTimer);

    currentState = STATE_RETURNING;
    ReportStatus();
}

void SetDockedMode()
{
    if (rc != null)
        rc.SetAutoPilotEnabled(false);

    SetBlockEnabled(offensive, false);
    SetBlockEnabled(flight, false);

    SetThrusters(false);
    SetGyros(false);
    SetBatteriesRecharge();

    if (antenna != null)
    {
        antenna.Enabled = true;
        antenna.Radius = IDLE_ANTENNA_RANGE;
    }

    SetLightsActive(false);

    currentState = STATE_DOCKED;
    dockTryTick = 0;
}

void SetAirborneIdleMode()
{
    SetBatteriesAuto();
    SetThrusters(true);
    SetGyros(true);

    if (antenna != null)
    {
        antenna.Enabled = true;
        antenna.Radius = ALERT_ANTENNA_RANGE;
    }

    if (currentState == STATE_DOCKED)
        currentState = STATE_AIRBORNE;
}


// ========================================
// AUTO STATE LOGIC
// ========================================
void AutoStateCheck()
{
    if (connector != null && connector.Status == MyShipConnectorStatus.Connected)
    {
        if (currentState != STATE_DOCKED)
        {
            SetDockedMode();
            ReportStatus();
        }
        return;
    }

    if (currentState == STATE_DOCKED)
    {
        SetAirborneIdleMode();
        ReportStatus();
    }

    if (ShouldForceReturn())
    {
        if (currentState != STATE_RETURNING && currentState != STATE_DOCKED)
        {
            ReturnMode();
            return;
        }
    }

    if (currentState == STATE_RETURNING)
    {
        HandleReturningState();
        return;
    }

    if (ShouldShowDamaged())
    {
        if (currentState != STATE_DAMAGED)
        {
            currentState = STATE_DAMAGED;
            ReportStatus();
        }
    }
    else
    {
        if (currentState != STATE_LAUNCHED && currentState != STATE_AIRBORNE)
            currentState = STATE_AIRBORNE;
    }
}

void HandleReturningState()
{
    if (connector == null)
        return;

    if (connector.Status == MyShipConnectorStatus.Connectable)
    {
        connector.Connect();
        return;
    }

    dockTryTick++;

    if (dockTryTick > 10)
        connector.Connect();
}


// ========================================
// RETURN / DAMAGE LOGIC
// ========================================
bool ShouldForceReturn()
{
    if (!IsAlive(antenna) || !IsAlive(connector) || !IsAlive(rc) || !IsAlive(flight) || !IsAlive(offensive))
        return true;

    if (GetBatteryPercent() <= LOW_BATTERY_PCT)
        return true;

    if (initialThrusterCount > 0)
    {
        int aliveThrusters = CountAlive(thrusters);
        double lossPct = 1.0 - ((double)aliveThrusters / (double)initialThrusterCount);
        if (lossPct >= THRUSTER_LOSS_RETURN_PCT)
            return true;
    }

    if (initialWeaponCount > 0)
    {
        int aliveWeapons = CountAlive(weapons);
        double lossPct = 1.0 - ((double)aliveWeapons / (double)initialWeaponCount);
        if (lossPct >= WEAPON_LOSS_RETURN_PCT)
            return true;
    }

    if (IsAmmoLow())
        return true;

    return false;
}

bool ShouldShowDamaged()
{
    if (!IsAlive(antenna) || !IsAlive(connector) || !IsAlive(rc) || !IsAlive(flight) || !IsAlive(offensive))
        return true;

    if (initialThrusterCount > 0)
    {
        int aliveThrusters = CountAlive(thrusters);
        if (aliveThrusters < initialThrusterCount)
            return true;
    }

    if (initialWeaponCount > 0)
    {
        int aliveWeapons = CountAlive(weapons);
        if (aliveWeapons < initialWeaponCount)
            return true;
    }

    return false;
}

bool IsAmmoLow()
{
    return false;
}


// ========================================
// HELPERS
// ========================================
void TriggerTimer(IMyTimerBlock timer)
{
    if (timer == null || timer.Closed)
        return;

    timer.Trigger();
}

int CountAlive<T>(List<T> blocks) where T : class, IMyTerminalBlock
{
    int count = 0;

    foreach (var b in blocks)
    {
        if (b != null && !b.Closed && b.IsFunctional)
            count++;
    }

    return count;
}

bool IsAlive(IMyTerminalBlock b)
{
    return b != null && !b.Closed && b.IsFunctional;
}

double GetBatteryPercent()
{
    if (batteries.Count == 0)
        return 1.0;

    double current = 0;
    double max = 0;

    foreach (var b in batteries)
    {
        if (b == null || b.Closed)
            continue;

        current += (double)b.CurrentStoredPower;
        max += (double)b.MaxStoredPower;
    }

    if (max <= 0)
        return 0;

    return current / max;
}


// ========================================
// BLOCK CONTROL
// ========================================
void SetBatteriesRecharge()
{
    foreach (var b in batteries)
    {
        if (b == null || b.Closed)
            continue;

        b.ChargeMode = ChargeMode.Recharge;
    }
}

void SetBatteriesAuto()
{
    foreach (var b in batteries)
    {
        if (b == null || b.Closed)
            continue;

        b.ChargeMode = ChargeMode.Auto;
    }
}

void SetThrusters(bool on)
{
    foreach (var t in thrusters)
    {
        if (t == null || t.Closed)
            continue;

        t.Enabled = on;
    }
}

void SetGyros(bool on)
{
    foreach (var g in gyros)
    {
        if (g == null || g.Closed)
            continue;

        g.Enabled = on;
    }
}

void SetLightsActive(bool on)
{
    foreach (var l in lights)
    {
        if (l == null || l.Closed)
            continue;

        if (on)
        {
            l.Enabled = true;
            l.Color = new Color(255, 0, 0);
        }
        else
        {
            l.Enabled = false;
        }
    }
}

void SetBlockEnabled(IMyFunctionalBlock b, bool on)
{
    if (b == null || b.Closed)
        return;

    b.Enabled = on;
}


// ========================================
// STATUS REPORTING
// ========================================
void ReportStatus()
{
    Vector3D pos = Me.GetPosition();

    string payload =
        GUARD_NAME + "|" +
        GUARD_TYPE + "|" +
        currentState + "|" +
        pos.X.ToString("0.00") + "|" +
        pos.Y.ToString("0.00") + "|" +
        pos.Z.ToString("0.00") + "|" +
        GetBatteryPercent().ToString("0.00");

    IGC.SendBroadcastMessage(STATUS_TAG, payload);
}