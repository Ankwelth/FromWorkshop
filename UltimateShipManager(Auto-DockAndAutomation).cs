/*
/ //////////////////////////////////////////////////////////////////////////////////////////////////
/ //                               ULTIMATE SHIP MANAGER (v5.3.1)                                 //
/ //////////////////////////////////////////////////////////////////////////////////////////////////
/
/  INSTRUCTIONS:
/  1. Place this script in a Programmable Block.
/  2. Add the tag "[USM LCD]" to any LCD panel you want to use as a HUD.
/  3. Configure the settings below to match your ship's needs.
/  4. Click 'Recompile'.
/
*/

public Program() 
{ 
    Runtime.UpdateFrequency = UpdateFrequency.Update10; 
    if (SHOW_ON_PB_SCREEN) SetupPbScreen();
    ScanGrid(); 
}

// ======================================================================================= 
//                                 CONFIGURATION SECTION
// ======================================================================================= 

// --- DISPLAY & HUD SETTINGS ------------------------------------------------------------
string TAG_LCD              = "[USM LCD]";      // Tag to identify HUD LCD panels
bool   SHOW_ON_PB_SCREEN    = true;             // Show HUD on the Programmable Block's screen?
float  LCD_FONT_SIZE        = 1.0f;             // Text size for main LCD panels
float  PB_FONT_SIZE         = 0.6f;             // Text size for the small PB screen

// --- GENERAL SETTINGS ------------------------------------------------------------------
string TAG_IGNORE           = "[Ignore]";       // Blocks with this tag are ignored by the script
int    RESCAN_FREQUENCY     = 200;              // How often to scan for new blocks (in ticks)

// --- SMART THRUSTERS -------------------------------------------------------------------
bool   AUTO_THRUSTER_SWITCH = true;             // Auto-toggle Atmo/Ion based on gravity?
bool   SMART_HYDROGEN       = true;             // Keep Hydro OFF unless needed for lift/speed?
bool   SPACE_HYDROGEN_ON    = true;             // Keep Hydro ON in zero-gravity (Space)?
float  CLIMB_TRIGGER_SPEED  = 5.0f;             // Vertical speed (m/s) to trigger Hydro Boost
float  GRAVITY_TRANSITION   = 0.20f;            // Gravity (g) threshold to switch Atmo <-> Ion

// --- HOVER MODE (PID CONTROLLER) -------------------------------------------------------
// Adjustment for the "Hover" command behavior
double HOVER_PID_P          = 2.0;              // Strength: High = Snappy, Low = Soft
double HOVER_PID_I          = 0.0;              // Drift Correction: Keep low (0.0 - 0.01)
double HOVER_PID_D          = 0.8;              // Dampener: Prevents bouncing
double HOVER_MAX_TILT       = 45.0;             // Safety Cutoff Angle (Degrees)

// --- UNDOCKING SAFETY INTERLOCKS -------------------------------------------------------
bool   SAFETY_INTERLOCK     = true;             // Prevent undock if critical resources are low?
float  MIN_BATT_TO_UNDOCK   = 0.05f;            // Min Battery % required (0.05 = 5%)
float  MIN_H2_TO_UNDOCK     = 0.05f;            // Min Hydrogen % required (0.05 = 5%)
bool   REQUIRE_URANIUM      = false;            // Require Uranium in reactors to undock?

// --- AUTOMATION & TIMERS ---------------------------------------------------------------
bool   AUTO_UNDOCK_ON_SIT   = true;             // Undock immediately when pilot sits?
bool   AUTO_LOCK_CONNECTORS = true;             // Lock connector when it turns yellow?
int    UNDOCK_SAFE_TIME     = 10;               // Seconds connectors stay OFF after undocking
int    SHUTDOWN_DELAY       = 5;                // Seconds after docking before "Sleep Mode"

// --- POWER MANAGEMENT ------------------------------------------------------------------
int    POWER_PRIORITY       = 1;                // 1 = Engines First, 2 = Reactors First
float  BATT_LOW_ON          = 0.30f;            // Generators ON if battery < 30%
float  BATT_HIGH_OFF        = 0.90f;            // Generators OFF if battery > 90%
float  EMERGENCY_POWER      = 0.15f;            // Force ALL power ON if battery < 15%

// --- DOCKING BEHAVIOR (SLEEP MODE) -----------------------------------------------------
bool   DOCK_SLEEP_PROD      = true;             // Pause Refineries/Assemblers when docked?
bool   DOCK_RECHARGE_BATTS  = true;             // Set Batteries to Recharge?
bool   DOCK_STOCKPILE_TANKS = true;             // Set Tanks to Stockpile?
bool   DOCK_STOP_THRUSTERS  = true;             // Turn off Thrusters?
bool   DOCK_STOP_GYROS      = true;             // Turn off Gyros?

// --- FLIGHT PRODUCTION -----------------------------------------------------------------
bool   FLY_RUN_REFINERIES   = true;             // Allow Refineries while flying?
bool   FLY_RUN_ASSEMBLERS   = false;            // Allow Assemblers while flying?

// ======================================================================================= 
//                                  END CONFIGURATION
// ======================================================================================= 

// Global State
bool isDocked = false; 
bool wasSeated = false; 
bool firstRun = true;
bool hoverActive = false;
bool hydroBoostActive = false;
double currentAltitude = 0;
double targetAltitude = 0;
double undockTimer = 0; 
double stateTimer = 0; 
double h2Level = 1.0; 
int runCount = 0; 
string lastWarning = "NONE"; 
string shipState = "Initializing";
string thrusterMode = "Mixed";

// PID State
double pid_integral = 0;
double pid_lastError = 0;

// Lists
List<IMyTerminalBlock> gridBlocks = new List<IMyTerminalBlock>();
List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyPowerProducer> reactors = new List<IMyPowerProducer>();
List<IMyPowerProducer> h2Engines = new List<IMyPowerProducer>();
List<IMyRefinery> refineries = new List<IMyRefinery>();
List<IMyAssembler> assemblers = new List<IMyAssembler>();
List<IMyShipConnector> connectors = new List<IMyShipConnector>();
List<IMyShipController> cockpits = new List<IMyShipController>();
List<IMyGasTank> tanks = new List<IMyGasTank>();
List<IMyTextPanel> lcdPanels = new List<IMyTextPanel>();

// Thruster Categories
List<IMyThrust> allThrusters = new List<IMyThrust>();
List<IMyThrust> thrustAtmo = new List<IMyThrust>();
List<IMyThrust> thrustIon = new List<IMyThrust>();
List<IMyThrust> thrustHydro = new List<IMyThrust>();
List<IMyThrust> thrustHover = new List<IMyThrust>();
List<IMyThrust> thrustBaseUp = new List<IMyThrust>();
List<IMyGyro> gyros = new List<IMyGyro>();

void SetupPbScreen()
{
    var surface = Me.GetSurface(0);
    surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    surface.FontSize = PB_FONT_SIZE;
    surface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
}

public void Main(string arg, UpdateType source)
{
    if (arg.ToLower() == "hover") { ToggleHover(); }
    else if (arg.ToLower() == "setup" || runCount % RESCAN_FREQUENCY == 0) ScanGrid();
    else if (arg.ToLower() == "force") { ForceUndock(); return; }

    runCount++;
    double dt = Runtime.TimeSinceLastRun.TotalSeconds;
    if (undockTimer > 0) undockTimer -= dt;

    bool isSeated = CheckPilot();
    bool anyConnected = CheckAnyConnected();
    IMyShipController mainCockpit = GetMainController();

    if (firstRun) { wasSeated = isSeated; firstRun = false; }

    if (AUTO_UNDOCK_ON_SIT && isSeated && !wasSeated && anyConnected) PerformUndock();
    wasSeated = isSeated; 

    ManageConnectors();
    UpdateDockingState(anyConnected && !isSeated, dt);

    if (isDocked) 
    {
        RunDockedLogic();
        hoverActive = false; 
    }
    else 
    {
        RunFlightLogic(mainCockpit);
    }

    UpdateDisplays(mainCockpit);
}

void ToggleHover()
{
    hoverActive = !hoverActive;
    if (hoverActive)
    {
        IMyShipController c = GetMainController();
        if (c != null && c.TryGetPlanetElevation(MyPlanetElevation.Surface, out targetAltitude))
        {
            pid_integral = 0; 
            lastWarning = "Hover Locked: " + targetAltitude.ToString("0") + "m";
        }
        else { hoverActive = false; lastWarning = "Cannot Hover: No Gravity"; }
    }
    else { ResetThrustOverride(thrustHover); lastWarning = "Hover Disengaged"; }
}

void ScanGrid()
{
    gridBlocks.Clear(); batteries.Clear(); reactors.Clear(); h2Engines.Clear();
    allThrusters.Clear(); thrustAtmo.Clear(); thrustIon.Clear(); thrustHydro.Clear(); 
    thrustHover.Clear(); thrustBaseUp.Clear();
    gyros.Clear(); refineries.Clear(); assemblers.Clear();
    connectors.Clear(); cockpits.Clear(); tanks.Clear(); lcdPanels.Clear();

    HashSet<long> ignoredIds = new HashSet<long>();
    List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups, g => g.Name.Contains(TAG_IGNORE));
    foreach (var g in groups) {
        List<IMyTerminalBlock> bks = new List<IMyTerminalBlock>();
        g.GetBlocks(bks); foreach (var b in bks) ignoredIds.Add(b.EntityId);
    }

    GridTerminalSystem.GetBlocksOfType(gridBlocks, b => b.IsSameConstructAs(Me));
    IMyShipController refCockpit = null;

    foreach (var b in gridBlocks) {
        if (b is IMyShipController && !ignoredIds.Contains(b.EntityId)) {
            cockpits.Add(b as IMyShipController);
            if (refCockpit == null && (b as IMyShipController).IsUnderControl) refCockpit = b as IMyShipController;
        }
    }
    if (refCockpit == null && cockpits.Count > 0) refCockpit = cockpits[0];

    foreach (var b in gridBlocks)
    {
        if (b.CustomName.Contains(TAG_IGNORE) || ignoredIds.Contains(b.EntityId)) continue;

        if (b is IMyThrust) 
        {
            var t = b as IMyThrust;
            allThrusters.Add(t);
            bool isAtmo = t.BlockDefinition.SubtypeId.Contains("Atmospheric");
            bool isHydro = t.BlockDefinition.SubtypeId.Contains("Hydrogen");
            if (isAtmo) thrustAtmo.Add(t); else if (isHydro) thrustHydro.Add(t); else thrustIon.Add(t);

            if (refCockpit != null) {
                if (Base6Directions.GetClosestDirection(refCockpit.WorldMatrix.Down) == 
                    refCockpit.WorldMatrix.GetClosestDirection(t.WorldMatrix.Forward)) {
                    thrustHover.Add(t); 
                    if (!isHydro) thrustBaseUp.Add(t);
                }
            }
        }
        else if (b is IMyBatteryBlock) batteries.Add(b as IMyBatteryBlock);
        else if (b is IMyGyro) gyros.Add(b as IMyGyro);
        else if (b is IMyRefinery) refineries.Add(b as IMyRefinery);
        else if (b is IMyAssembler) assemblers.Add(b as IMyAssembler);
        else if (b is IMyShipConnector) connectors.Add(b as IMyShipConnector);
        else if (b is IMyGasTank) tanks.Add(b as IMyGasTank);
        else if (b is IMyTextPanel && b.CustomName.Contains(TAG_LCD)) lcdPanels.Add(b as IMyTextPanel);
        else if (b is IMyPowerProducer && !(b is IMySolarPanel)) {
            var p = b as IMyPowerProducer;
            if (p.BlockDefinition.ToString().Contains("HydrogenEngine")) h2Engines.Add(p); else reactors.Add(p);
        }
    }
}

void RunFlightLogic(IMyShipController controller)
{
    shipState = undockTimer > 0 ? "UNDOCKING" : "FLIGHT";
    MonitorGasLevels();
    ToggleList(gyros, true);
    
    if (AUTO_THRUSTER_SWITCH && controller != null)
    {
        double gravity = controller.GetNaturalGravity().Length() / 9.81;
        bool inGravity = gravity > GRAVITY_TRANSITION;
        thrusterMode = inGravity ? "ATMOSPHERE" : "SPACE";
        
        ToggleList(thrustAtmo, inGravity);
        ToggleList(thrustIon, !inGravity);

        bool hydroOn = false;
        hydroBoostActive = false;
        if (SMART_HYDROGEN) {
            if (inGravity) {
                double shipMass = controller.CalculateShipMass().PhysicalMass;
                double weightForce = shipMass * controller.GetNaturalGravity().Length();
                double baseLift = 0;
                foreach(var t in thrustBaseUp) if(t.IsWorking) baseLift += t.MaxEffectiveThrust;
                if (baseLift < (weightForce * 1.1)) hydroOn = true;

                Vector3D velocity = controller.GetShipVelocities().LinearVelocity;
                Vector3D gVec = controller.GetNaturalGravity();
                if (gVec.LengthSquared() > 0 && Vector3D.Dot(velocity, Vector3D.Normalize(-gVec)) > CLIMB_TRIGGER_SPEED) 
                    { hydroOn = true; hydroBoostActive = true; }
            } else { hydroOn = SPACE_HYDROGEN_ON; }
        } else { hydroOn = true; }
        ToggleList(thrustHydro, hydroOn);
    }
    else { thrusterMode = "MANUAL"; ToggleList(allThrusters, true); }

    if (hoverActive && controller != null) { RunHoverLogic(controller); shipState = "FLIGHT (HOVER)"; }
    else { ResetThrustOverride(thrustHover); }

    foreach (var b in batteries) b.ChargeMode = ChargeMode.Auto;
    foreach (var t in tanks) t.Stockpile = false;

    float charge = GetAverageCharge();
    bool emergencyPower = charge < EMERGENCY_POWER;
    if (charge < BATT_LOW_ON) {
        if (POWER_PRIORITY == 1) { ToggleList(h2Engines, true); ToggleList(reactors, emergencyPower); }
        else { ToggleList(reactors, true); ToggleList(h2Engines, emergencyPower); }
    } else if (charge > BATT_HIGH_OFF) { ToggleList(h2Engines, false); ToggleList(reactors, false); }

    foreach (var r in refineries) r.Enabled = FLY_RUN_REFINERIES && r.InputInventory.ItemCount > 0;
    foreach (var a in assemblers) a.Enabled = FLY_RUN_ASSEMBLERS && !a.IsQueueEmpty;
}

void RunHoverLogic(IMyShipController controller)
{
    controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out currentAltitude);
    bool pilotInput = Math.Abs(controller.MoveIndicator.Y) > 0.1;

    if (pilotInput) { ResetThrustOverride(thrustHover); targetAltitude = currentAltitude; pid_integral = 0; }
    else {
        double gravityDot = Vector3D.Dot(controller.WorldMatrix.Down, Vector3D.Normalize(controller.GetNaturalGravity()));
        if (gravityDot < Math.Cos(Math.PI * HOVER_MAX_TILT / 180.0)) { hoverActive = false; lastWarning = "Hover ABORT: Tilted"; ResetThrustOverride(thrustHover); return; }

        double error = targetAltitude - currentAltitude;
        pid_integral += error * (1.0/6.0);
        double derivative = (error - pid_lastError) / (1.0/6.0);
        pid_lastError = error;
        double output = (HOVER_PID_P * error) + (HOVER_PID_I * pid_integral) + (HOVER_PID_D * derivative);

        double mass = controller.CalculateShipMass().PhysicalMass;
        double gravity = controller.GetNaturalGravity().Length();
        double maxThrust = 0;
        foreach(var t in thrustHover) if(t.IsWorking) maxThrust += t.MaxEffectiveThrust;
        if(maxThrust <= 0) return;

        float overridePercent = MathHelper.Clamp((float)((mass * gravity + output) / maxThrust), 0.0f, 1.0f);
        foreach(var t in thrustHover) t.ThrustOverridePercentage = overridePercent;
    }
}

void RunDockedLogic()
{
    shipState = "DOCKED (SLEEP)";
    if (DOCK_STOP_THRUSTERS) ToggleList(allThrusters, false); 
    if (DOCK_STOP_GYROS) ToggleList(gyros, false);
    if (DOCK_RECHARGE_BATTS) foreach (var b in batteries) b.ChargeMode = ChargeMode.Recharge;
    if (DOCK_STOCKPILE_TANKS) foreach (var t in tanks) t.Stockpile = true;
    if (DOCK_SLEEP_PROD) { ToggleList(refineries, false); ToggleList(assemblers, false); }
    ToggleList(h2Engines, false); ToggleList(reactors, false);
    ResetThrustOverride(thrustHover);
}

void ResetThrustOverride(List<IMyThrust> list) { foreach(var t in list) t.ThrustOverridePercentage = 0f; }

void MonitorGasLevels()
{
    double h2Cap = 0, h2Cur = 0;
    foreach (var t in tanks) { if (t.BlockDefinition.SubtypeId.Contains("Hydro")) { h2Cur += t.FilledRatio * t.Capacity; h2Cap += t.Capacity; } }
    h2Level = h2Cap > 0 ? h2Cur / h2Cap : 1.0;
}

void UpdateDockingState(bool isPhysicallyDocked, double dt)
{
    if (undockTimer > 0) { isDocked = false; return; }
    if (isPhysicallyDocked) { stateTimer += dt; if (stateTimer >= (double)SHUTDOWN_DELAY) { if (!isDocked) isDocked = true; } else shipState = $"DOCKING... {Math.Max(0, (double)SHUTDOWN_DELAY - stateTimer):0}s"; }
    else { isDocked = false; stateTimer = 0; }
}

void PerformUndock()
{
    bool battLow = GetAverageCharge() < MIN_BATT_TO_UNDOCK;
    bool h2Low = (thrustHydro.Count > 0) && (h2Level < MIN_H2_TO_UNDOCK);
    bool reactorEmpty = REQUIRE_URANIUM && reactors.Count > 0 && reactors.TrueForAll(r => r.GetInventory().ItemCount == 0);

    if (SAFETY_INTERLOCK && (battLow || h2Low || reactorEmpty)) {
        if (battLow) lastWarning = "UNDOCK BLOCKED: LOW BATT"; else if (h2Low) lastWarning = "UNDOCK BLOCKED: LOW H2"; else lastWarning = "UNDOCK BLOCKED: NO URANIUM";
        return;
    }
    ForceUndock();
}

void ForceUndock()
{
    foreach (var c in connectors) { c.Disconnect(); c.Enabled = false; }
    undockTimer = (double)UNDOCK_SAFE_TIME; isDocked = false; lastWarning = "NONE";
}

void ManageConnectors()
{
    if (undockTimer > 0) { if (undockTimer < 2) foreach(var c in connectors) c.Enabled = true; return; }
    if (AUTO_LOCK_CONNECTORS) foreach (var c in connectors) if (c.Status == MyShipConnectorStatus.Connectable) c.Connect();
}

void ToggleList<T>(List<T> list, bool state) where T : IMyFunctionalBlock { foreach (var b in list) if (b.Enabled != state) b.Enabled = state; }
float GetAverageCharge() { float m = 0, c = 0; foreach (var b in batteries) { m += b.MaxStoredPower; c += b.CurrentStoredPower; } return m > 0 ? c / m : 1f; }
bool CheckPilot() { foreach (var c in cockpits) if (c.IsUnderControl) return true; return false; }
bool CheckAnyConnected() { foreach (var c in connectors) if (c.Status == MyShipConnectorStatus.Connected) return true; return false; }
IMyShipController GetMainController() { foreach (var c in cockpits) if (c.IsUnderControl) return c; return cockpits.Count > 0 ? cockpits[0] : null; }

void UpdateDisplays(IMyShipController c)
{
    StringBuilder sb = new StringBuilder();
    sb.AppendLine($"-- USM v5.3.1 --");
    sb.AppendLine($"State: {shipState}");
    sb.AppendLine($"Env:   {thrusterMode}");
    
    if (hydroBoostActive) sb.AppendLine($"HYDRO: BOOSTING");
    else if (thrustHydro.Count > 0) sb.AppendLine($"HYDRO: {(thrustHydro.Count > 0 && thrustHydro[0].Enabled ? "ON" : "STBY")}");
    
    if (hoverActive) sb.AppendLine($"Hover: {currentAltitude:0}m / {targetAltitude:0}m");
    
    sb.AppendLine($"Batt:  {GetAverageCharge():P0} | H2: {h2Level:P0}");
    if (lastWarning != "NONE") sb.AppendLine($"\n!! {lastWarning} !!");
    
    foreach (var p in lcdPanels) { p.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; p.WriteText(sb.ToString()); p.FontSize = LCD_FONT_SIZE; }
    
    if (SHOW_ON_PB_SCREEN)
    {
        var s = Me.GetSurface(0);
        s.WriteText(sb.ToString());
    }
    Echo(sb.ToString());
}